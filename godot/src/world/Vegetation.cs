using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using FD.Core;
using static FD.Core.FMath;

namespace FD.World;

/// <summary>What a species is, for placement rules.</summary>
public enum VegFamily { Oak, Pine, Birch, Bush, Fern, RockS, RockM, RockL, Cliff, Stump, Log }

/// <summary>One plant/rock type: meshes, LOD ranges, look and collision.</summary>
public sealed class VegSpecies
{
    public string Name;
    public VegFamily Family;
    public Mesh Hi, Lo;
    /// <summary>Hi mesh drawn up to HiRange (m); Lo mesh from there up to LoRange (0 = unlimited).</summary>
    public float HiRange, LoRange;
    public float ScaleMin = 0.85f, ScaleMax = 1.2f;
    public float Height = 10f;
    public Color Tint = new(1, 1, 1);          // linear, for A=1 parts
    public float TintJitter = 0.1f;
    public float Wind, Flutter, Backlight;
    public bool CastShadow = true, AlignToSlope;
    public float Sink = 0.1f;
    // collision (local, before instance scale): cylinder (radius/height) or box
    public float ColRadius, ColHeight;
    public Vector3 ColBoxSize, ColBoxCenter;
    public int Index;
}

/// <summary>
/// Forests, groves, lone trees, bushes, rocks, stumps, logs and ferns placed deterministically per 80 m chunk from
/// <see cref="Heightfield.ForestDensity"/>, <see cref="Heightfield.Clearance"/>, slope and rockiness.
/// <para>Rendering: per chunk and species a "hi" MultiMesh (visible up to HiRange), per 240 m group and species a
/// "lo" MultiMesh from HiRange on; the switch is per instance in fd_vc_mm.gdshader (dithered crossfade driven by
/// the global fd_cam_pos). Leaves are tinted per instance via MultiMesh custom data.</para>
/// <para>Collision: one static PhysicsServer3D body per chunk (layer "props") with trunk cylinders / rock boxes.</para>
/// <para>Uses meshes from assets/models/nature.glb when present, otherwise procedural placeholders.</para>
/// <para>Builders keep areas clear with <see cref="Heightfield.AddPad"/>, <see cref="Heightfield.AddPath"/> or
/// <see cref="Heightfield.ExcludeVegetation"/> BEFORE <see cref="Heightfield.Bake"/>.</para>
/// </summary>
public partial class Vegetation : Node3D
{
    public const int GroupChunks = 3;               // lo MultiMeshes group 3×3 chunks (240 m)
    public readonly List<VegSpecies> Species = new();
    public bool UsingPlaceholders { get; private set; }
    public int TotalInstances { get; private set; }
    public int TreeCount { get; private set; }

    struct Inst { public Transform3D X; public Color Custom; }

    readonly List<Rid> _bodies = new();
    readonly Dictionary<string, Shape3D> _shapes = new();
    // tree positions for verification (x, z, species index)
    readonly List<(Vector2 p, int sp)> _trees = new();
    Heightfield _hf;
    Noise2 _nMix, _nTint;

    public void Build(Heightfield hf, bool collision = true)
    {
        Name = "Vegetation";
        _hf = hf;
        _nMix = new Noise2(RegionSpec.Seed + 301);
        _nTint = new Noise2(RegionSpec.Seed + 302);
        DefineSpecies();

        int nc = Terrain.Chunks;
        var perChunk = new List<Inst>[nc * nc][];
        var treesPerChunk = new List<(Vector2, int)>[nc * nc];
        Parallel.For(0, nc * nc, c =>
        {
            var lists = new List<Inst>[Species.Count];
            for (int i = 0; i < lists.Length; i++) lists[i] = new List<Inst>();
            var trees = new List<(Vector2, int)>();
            PlaceChunk(c % nc, c / nc, lists, trees);
            perChunk[c] = lists;
            treesPerChunk[c] = trees;
        });
        foreach (var t in treesPerChunk) _trees.AddRange(t);
        TreeCount = _trees.Count;

        // --- hi MultiMeshes per chunk
        var hiRoot = new Node3D { Name = "Hi" };
        AddChild(hiRoot);
        var hiMats = new Material[Species.Count];
        var loMats = new Material[Species.Count];
        foreach (var sp in Species)
        {
            hiMats[sp.Index] = MakeMaterial(sp, 0f, sp.HiRange);
            if (sp.Lo != null) loMats[sp.Index] = MakeMaterial(sp, sp.HiRange, sp.LoRange > 0 ? sp.LoRange : 100000f);
        }
        for (int c = 0; c < nc * nc; c++)
        {
            for (int s = 0; s < Species.Count; s++)
            {
                var list = perChunk[c][s];
                if (list.Count == 0) continue;
                var sp = Species[s];
                TotalInstances += list.Count;
                // chunk-level cull: every instance within HiRange must be drawn (chunk half-diagonal ≈ 57 m);
                // small plants accept a slightly early cut at chunk corners
                float chunkEnd = sp.HiRange + (sp.HiRange <= 55f ? 40f : 58f);
                hiRoot.AddChild(MakeMmi($"{sp.Name}_{c}", sp.Hi, list, hiMats[s], chunkEnd, sp.CastShadow));
            }
        }
        // --- lo MultiMeshes per group
        var loRoot = new Node3D { Name = "Lo" };
        AddChild(loRoot);
        int ng = (nc + GroupChunks - 1) / GroupChunks;
        for (int gz = 0; gz < ng; gz++)
            for (int gx = 0; gx < ng; gx++)
                for (int s = 0; s < Species.Count; s++)
                {
                    var sp = Species[s];
                    if (sp.Lo == null) continue;
                    var all = new List<Inst>();
                    for (int cz = gz * GroupChunks; cz < Math.Min(nc, (gz + 1) * GroupChunks); cz++)
                        for (int cx = gx * GroupChunks; cx < Math.Min(nc, (gx + 1) * GroupChunks); cx++)
                            all.AddRange(perChunk[cz * nc + cx][s]);
                    if (all.Count == 0) continue;
                    float end = sp.LoRange > 0 ? sp.LoRange + 175f : 0f;
                    loRoot.AddChild(MakeMmi($"{sp.Name}_lo_{gx}_{gz}", sp.Lo, all, loMats[s], end, sp.CastShadow));
                }

        if (collision) BuildCollision(perChunk);
        GD.Print($"[Vegetation] {(UsingPlaceholders ? "placeholder" : "nature.glb")} species={Species.Count} trees={TreeCount} instances={TotalInstances}");
    }

    public override void _ExitTree()
    {
        foreach (var b in _bodies) PhysicsServer3D.FreeRid(b);
        _bodies.Clear();
    }

    // ------------------------------------------------------------------------------------------ species

    void DefineSpecies()
    {
        UsingPlaceholders = !Models.Exists("nature");
        var meshes = UsingPlaceholders ? PlaceholderVeg.Build() : new Dictionary<string, Mesh>(Models.Meshes("nature", MatKind.Multi));
        var manifest = UsingPlaceholders ? null : Models.Manifest("nature");
        var objects = manifest != null && manifest.ContainsKey("objects") ? manifest["objects"].AsGodotDictionary() : null;

        VegSpecies Add(string name, VegFamily fam, float hi, float lo, uint tintHex, float height)
        {
            if (!meshes.TryGetValue(name, out var m) || m == null) return null;
            var sp = new VegSpecies
            {
                Name = name, Family = fam, Hi = m, HiRange = hi, LoRange = lo, Height = height,
                Tint = FMath.HexLinear(tintHex), Index = Species.Count,
            };
            meshes.TryGetValue(name + "_lo", out sp.Lo);
            if (fam is VegFamily.RockM or VegFamily.RockL or VegFamily.Cliff)
            {
                var bb = m.GetAabb();
                sp.ColBoxSize = bb.Size * 0.8f; sp.ColBoxCenter = bb.GetCenter();
            }
            if (objects != null && objects.ContainsKey(name))
            {
                var o = objects[name].AsGodotDictionary();
                if (o.ContainsKey("height")) sp.Height = (float)o["height"].AsDouble();
                if (o.ContainsKey("collision") && o["collision"].VariantType == Variant.Type.Dictionary)
                {
                    var col = o["collision"].AsGodotDictionary();
                    string shape = col.ContainsKey("shape") ? col["shape"].AsString() : "";
                    if (shape == "cylinder")
                    {
                        sp.ColRadius = (float)col["radius"].AsDouble();
                        sp.ColHeight = (float)col["height"].AsDouble();
                    }
                    else if (col.ContainsKey("box"))
                    {
                        var box = col["box"].AsGodotDictionary();
                        var c = box["center"].AsFloat32Array(); var s = box["size"].AsFloat32Array();
                        sp.ColBoxCenter = new Vector3(c[0], c[1], c[2]);
                        sp.ColBoxSize = new Vector3(s[0], s[1], s[2]);
                    }
                }
            }
            Species.Add(sp);
            return sp;
        }

        foreach (var n in new[] { "tree_oak_1", "tree_oak_2", "tree_oak_3" })
        {
            var sp = Add(n, VegFamily.Oak, 85f, 0f, 0x5d8a3c, 11f);
            if (sp != null) { sp.Wind = 0.35f; sp.Flutter = 0.05f; sp.Backlight = 0.25f; sp.ScaleMin = 0.85f; sp.ScaleMax = 1.25f; sp.Sink = 0.2f; sp.TintJitter = 0.14f; if (sp.ColRadius == 0) { sp.ColRadius = 0.45f; sp.ColHeight = 3.5f; } }
        }
        foreach (var n in new[] { "tree_pine_1", "tree_pine_2", "tree_pine_3" })
        {
            var sp = Add(n, VegFamily.Pine, 85f, 0f, 0x3f6e46, 15f);
            if (sp != null) { sp.Wind = 0.28f; sp.Flutter = 0.02f; sp.Backlight = 0.12f; sp.ScaleMin = 0.85f; sp.ScaleMax = 1.3f; sp.Sink = 0.2f; sp.TintJitter = 0.1f; if (sp.ColRadius == 0) { sp.ColRadius = 0.38f; sp.ColHeight = 3f; } }
        }
        foreach (var n in new[] { "tree_birch_1", "tree_birch_2" })
        {
            var sp = Add(n, VegFamily.Birch, 85f, 0f, 0x8db04e, 11f);
            if (sp != null) { sp.Wind = 0.45f; sp.Flutter = 0.07f; sp.Backlight = 0.35f; sp.ScaleMin = 0.85f; sp.ScaleMax = 1.2f; sp.Sink = 0.2f; sp.TintJitter = 0.12f; if (sp.ColRadius == 0) { sp.ColRadius = 0.2f; sp.ColHeight = 4.5f; } }
        }
        foreach (var n in new[] { "bush_1", "bush_2", "bush_3" })
        {
            var sp = Add(n, VegFamily.Bush, 50f, 260f, 0x55803a, 1.5f);
            if (sp != null) { sp.Wind = 0.25f; sp.Flutter = 0.04f; sp.Backlight = 0.3f; sp.ScaleMin = 0.75f; sp.ScaleMax = 1.35f; sp.Sink = 0.08f; sp.TintJitter = 0.14f; sp.ColRadius = 0; }
        }
        var fern = Add("fern", VegFamily.Fern, 48f, 0f, 0x5a8a3a, 0.45f);
        if (fern != null) { fern.Wind = 0.35f; fern.Backlight = 0.35f; fern.ScaleMin = 0.8f; fern.ScaleMax = 1.5f; fern.CastShadow = false; fern.Sink = 0.02f; fern.TintJitter = 0.12f; }
        var rs = Add("rock_s", VegFamily.RockS, 45f, 0f, 0xffffff, 0.5f);
        if (rs != null) { rs.AlignToSlope = true; rs.ScaleMin = 0.8f; rs.ScaleMax = 1.6f; rs.Sink = 0.05f; rs.CastShadow = false; rs.ColBoxSize = Vector3.Zero; }
        var rm = Add("rock_m", VegFamily.RockM, 110f, 0f, 0xffffff, 1.2f);
        if (rm != null) { rm.AlignToSlope = true; rm.ScaleMin = 0.8f; rm.ScaleMax = 1.5f; rm.Sink = 0.12f; }
        var rl = Add("rock_l", VegFamily.RockL, 90f, 0f, 0xffffff, 3f);
        if (rl != null) { rl.AlignToSlope = true; rl.ScaleMin = 0.7f; rl.ScaleMax = 1.3f; rl.Sink = 0.3f; }
        var cl = Add("rock_cliff", VegFamily.Cliff, 110f, 0f, 0xffffff, 5f);
        if (cl != null) { cl.AlignToSlope = true; cl.ScaleMin = 0.7f; cl.ScaleMax = 1.25f; cl.Sink = 0.6f; }
        var st = Add("stump", VegFamily.Stump, 55f, 0f, 0xffffff, 0.6f);
        if (st != null) { st.ScaleMin = 0.8f; st.ScaleMax = 1.3f; st.Sink = 0.05f; st.CastShadow = false; }
        var lg = Add("log_fallen", VegFamily.Log, 70f, 0f, 0xffffff, 1f);
        if (lg != null) { lg.AlignToSlope = true; lg.ScaleMin = 0.8f; lg.ScaleMax = 1.25f; lg.Sink = 0.08f; if (lg.ColBoxSize == Vector3.Zero) { lg.ColBoxSize = new Vector3(5.2f, 0.55f, 0.6f); lg.ColBoxCenter = new Vector3(0, 0.26f, 0); } }
    }

    List<VegSpecies> OfFamily(VegFamily f)
    {
        var r = new List<VegSpecies>();
        foreach (var s in Species) if (s.Family == f) r.Add(s);
        return r;
    }

    // ------------------------------------------------------------------------------------------ placement

    void PlaceChunk(int cx, int cz, List<Inst>[] lists, List<(Vector2, int)> trees)
    {
        var hf = _hf;
        float x0 = Heightfield.Min + cx * Terrain.ChunkSize, z0 = Heightfield.Min + cz * Terrain.ChunkSize;
        var oaks = OfFamily(VegFamily.Oak); var pines = OfFamily(VegFamily.Pine); var birches = OfFamily(VegFamily.Birch);
        var bushes = OfFamily(VegFamily.Bush);
        VegSpecies First(VegFamily f) { var l = OfFamily(f); return l.Count > 0 ? l[0] : null; }
        var fern = First(VegFamily.Fern); var rockS = First(VegFamily.RockS); var rockM = First(VegFamily.RockM);
        var rockL = First(VegFamily.RockL); var cliff = First(VegFamily.Cliff); var stump = First(VegFamily.Stump); var log = First(VegFamily.Log);
        int seed = RegionSpec.Seed;

        // --- trees: jittered grid
        const float tc = 6.2f;
        int nt = (int)MathF.Ceiling(Terrain.ChunkSize / tc);
        for (int j = 0; j < nt; j++)
            for (int i = 0; i < nt; i++)
            {
                var r = new Rng(Hash.U32(cx * 1000 + i, cz * 1000 + j, seed, 1));
                float x = x0 + (i + 0.1f + 0.8f * r.F()) * tc, z = z0 + (j + 0.1f + 0.8f * r.F()) * tc;
                if (x >= x0 + Terrain.ChunkSize || z >= z0 + Terrain.ChunkSize) continue;
                float clear = hf.Clearance(x, z, ClearKind.Tree);
                if (clear < 0.08f) continue;
                float forest = hf.ForestDensity(x, z);
                // riparian trees line the stream banks (kept open around the road crossing)
                float sdist = hf.StreamDistance(x, z, out float ss);
                float shw = hf.StreamHalfWidthAt(ss);
                float riparian = clear * 0.5f * (1f - Smoothstep(shw + 9f, shw + 26f, sdist))
                                 * Smoothstep(28f, 45f, (new Vector2(x, z) - RegionSpec.BridgePoint).Length())
                                 * (0.6f + 0.8f * Saturate(_nMix.Get(x / 40f + 7f, z / 40f)));
                forest = MathF.Max(forest, riparian);
                float lone = clear * 0.02f * (1f - forest);
                float roll = r.F();
                bool core = roll < forest * forest * 0.92f;
                bool edge = !core && roll < forest * 0.8f;
                if (!core && !edge && roll > forest * 0.8f + lone) continue;
                if (hf.Slope(x, z) > 30f || hf.IsWater(x, z)) continue;
                var b = hf.Biome(x, z);
                if (b.Dirt > 0.3f || b.Field > 0.3f || b.Sand > 0.5f) continue;
                // species mix: pines to the north and on hills, oaks south/east, birches on edges and near water
                float h = hf.Height(x, z);
                float n = _nMix.Fbm(x / 160f, z / 160f, 2);
                float wPine = 0.25f + 0.55f * Smoothstep(-160f, -460f, z) + 0.35f * Smoothstep(8f, 25f, h) + 0.5f * n;
                float wOak = 0.45f + 0.3f * Smoothstep(-250f, 150f, z) - 0.35f * n;
                float wBirch = 0.12f + (edge || !core ? 0.35f : 0f) + 0.9f * (1f - Smoothstep(shw + 8f, shw + 30f, sdist));
                wPine = MathF.Max(wPine, 0.02f); wOak = MathF.Max(wOak, 0.02f); wBirch = MathF.Max(wBirch, 0.02f);
                float pick = r.F() * (wPine + wOak + wBirch);
                var fam = pick < wPine ? pines : pick < wPine + wOak ? oaks : birches;
                if (fam.Count == 0) fam = oaks.Count > 0 ? oaks : pines.Count > 0 ? pines : birches;
                if (fam.Count == 0) continue;
                // bigger variants in the forest core
                int vi = Math.Min(fam.Count - 1, (int)(MathF.Pow(r.F(), core ? 0.6f : 1.4f) * fam.Count));
                var sp = fam[vi];
                float sc = Lerp(sp.ScaleMin, sp.ScaleMax, core ? 0.35f + 0.65f * r.F() : 0.7f * r.F());
                Emit(lists, sp, x, z, sc, ref r, TreeTint(sp, x, z, ref r), tilt: 0.04f);
                trees.Add((new Vector2(x, z), sp.Index));
            }

        // --- understory: ferns, bushes, stumps, logs
        const float uc = 3.3f;
        int nu = (int)MathF.Ceiling(Terrain.ChunkSize / uc);
        for (int j = 0; j < nu; j++)
            for (int i = 0; i < nu; i++)
            {
                var r = new Rng(Hash.U32(cx * 1000 + i, cz * 1000 + j, seed, 2));
                float x = x0 + (i + r.F()) * uc, z = z0 + (j + r.F()) * uc;
                if (x >= x0 + Terrain.ChunkSize || z >= z0 + Terrain.ChunkSize) continue;
                float forestMask = hf.ForestCover(x, z);
                float bushClear = hf.Clearance(x, z, ClearKind.Bush);
                if (bushClear <= 0.02f) continue;
                float f = forestMask * bushClear;
                float edgeness = 1f - MathF.Abs(f - 0.45f) * 2.2f;
                // shrubs along the stream banks
                float bsd = hf.StreamDistance(x, z, out float bss);
                float bhw = hf.StreamHalfWidthAt(bss);
                edgeness = MathF.Max(edgeness, 0.9f * (1f - Smoothstep(bhw + 4f, bhw + 16f, bsd))
                                               * Smoothstep(20f, 35f, (new Vector2(x, z) - RegionSpec.BridgePoint).Length()));
                float roll = r.F();
                float pFern = 0.46f * Smoothstep(0.35f, 0.8f, f);
                float pBush = 0.07f * f + 0.10f * Saturate(edgeness) + 0.006f * bushClear;
                float pStump = 0.006f * f, pLog = 0.004f * f;
                if (roll < pFern)
                {
                    if (fern != null && hf.Slope(x, z) < 35f && !hf.IsWater(x, z)) Emit(lists, fern, x, z, Lerp(fern.ScaleMin, fern.ScaleMax, r.F()), ref r, PlantTint(fern, x, z, ref r));
                }
                else if (roll < pFern + pBush)
                {
                    if (bushes.Count > 0 && hf.Slope(x, z) < 32f && !hf.IsWater(x, z) && hf.Biome(x, z).Dirt < 0.2f)
                    {
                        var sp = bushes[r.Int(bushes.Count)];
                        Emit(lists, sp, x, z, Lerp(sp.ScaleMin, sp.ScaleMax, r.F()), ref r, PlantTint(sp, x, z, ref r));
                    }
                }
                else if (roll < pFern + pBush + pStump)
                {
                    if (stump != null && hf.Clearance(x, z, ClearKind.Tree) > 0.5f) Emit(lists, stump, x, z, Lerp(stump.ScaleMin, stump.ScaleMax, r.F()), ref r, new Color(1, 1, 1, 0));
                }
                else if (roll < pFern + pBush + pStump + pLog)
                {
                    if (log != null && hf.Clearance(x, z, ClearKind.Tree) > 0.6f && hf.Slope(x, z) < 20f) Emit(lists, log, x, z, Lerp(log.ScaleMin, log.ScaleMax, r.F()), ref r, new Color(1, 1, 1, 0));
                }
            }

        // --- rocks
        const float rc = 10f;
        int nr = (int)MathF.Ceiling(Terrain.ChunkSize / rc);
        for (int j = 0; j < nr; j++)
            for (int i = 0; i < nr; i++)
            {
                var r = new Rng(Hash.U32(cx * 1000 + i, cz * 1000 + j, seed, 3));
                float x = x0 + (i + r.F()) * rc, z = z0 + (j + r.F()) * rc;
                if (x >= x0 + Terrain.ChunkSize || z >= z0 + Terrain.ChunkSize) continue;
                // stones along the stream edges (may sit partly in the water)
                float rsd = hf.StreamDistance(x, z, out float rss);
                float rwhw = hf.StreamWaterHalfWidthAt(rss);
                if (rsd > rwhw - 0.6f && rsd < rwhw + 2.5f && (new Vector2(x, z) - RegionSpec.BridgePoint).Length() > 14f)
                {
                    if (r.F() < 0.35f)
                    {
                        var bs = r.F() < 0.8f ? rockS : rockM;
                        if (bs != null) Emit(lists, bs, x, z, Lerp(bs.ScaleMin, bs.ScaleMax, r.F()), ref r, RockTint(x, z, ref r));
                    }
                    continue;
                }
                float clear = hf.Clearance(x, z, ClearKind.Bush);
                if (clear < 0.3f || hf.IsWater(x, z)) continue;
                var b = hf.Biome(x, z);
                if (b.Dirt > 0.4f || b.Field > 0.3f) continue;
                float rocky = hf.RockinessAt(x, z);
                float slope = hf.Slope(x, z);
                float forest = hf.ForestCover(x, z);
                float p = 0.012f + 0.75f * rocky + 0.05f * forest + 0.12f * Smoothstep(14f, 28f, slope);
                if (r.F() > p) continue;
                VegSpecies sp;
                float k = r.F();
                if (cliff != null && rocky > 0.35f && slope > 8f && k < 0.5f) sp = cliff;
                else if (rockL != null && (k < 0.12f + 0.5f * rocky)) sp = rockL;
                else if (rockM != null && k < 0.7f) sp = rockM;
                else sp = rockS ?? rockM;
                if (sp == null) continue;
                if ((sp == rockL || sp == cliff) && hf.Clearance(x, z, ClearKind.Tree) < 0.5f) sp = rockM ?? sp;
                Emit(lists, sp, x, z, Lerp(sp.ScaleMin, sp.ScaleMax, r.F()), ref r, RockTint(x, z, ref r));
                // small scree around big rocks
                if ((sp == rockL || sp == cliff) && rockS != null)
                    for (int q = 0; q < 3; q++)
                    {
                        float a = r.F() * MathF.Tau, d = 2.5f + r.F() * 4f;
                        float sx = x + MathF.Cos(a) * d, sz = z + MathF.Sin(a) * d;
                        if (hf.Clearance(sx, sz, ClearKind.Ground) > 0.5f)
                            Emit(lists, rockS, sx, sz, Lerp(rockS.ScaleMin, rockS.ScaleMax, r.F()), ref r, RockTint(sx, sz, ref r));
                    }
            }
    }

    void Emit(List<Inst>[] lists, VegSpecies sp, float x, float z, float scale, ref Rng r, Color custom, float tilt = 0f)
    {
        var hf = _hf;
        float yaw = r.F() * MathF.Tau;
        Basis basis = Basis.FromEuler(new Vector3(0, yaw, 0));
        if (sp.AlignToSlope)
        {
            Vector3 n = hf.Normal(x, z);
            Vector3 up = Vector3.Up.Lerp(n, 0.7f).Normalized();
            var q = new Quaternion(Vector3.Up, up);
            basis = new Basis(q) * basis;
        }
        else if (tilt > 0)
        {
            basis = Basis.FromEuler(new Vector3((r.F() - 0.5f) * tilt, 0, (r.F() - 0.5f) * tilt)) * basis;
        }
        basis = basis.Scaled(new Vector3(scale, scale, scale));
        // sit on the lowest ground under the footprint so nothing floats on slopes
        float rad = sp.Family is VegFamily.Oak or VegFamily.Pine or VegFamily.Birch ? 0.5f * scale : 0.35f * scale;
        float y = MathF.Min(hf.Height(x, z), MathF.Min(MathF.Min(hf.Height(x + rad, z), hf.Height(x - rad, z)),
                                                        MathF.Min(hf.Height(x, z + rad), hf.Height(x, z - rad))));
        y -= sp.Sink * scale;
        lists[sp.Index].Add(new Inst { X = new Transform3D(basis, new Vector3(x, y, z)), Custom = custom });
    }

    Color TreeTint(VegSpecies sp, float x, float z, ref Rng r)
    {
        Color c = sp.Tint;
        float patch = _nTint.Fbm(x / 90f, z / 90f, 2);      // stands of lighter/darker trees
        float v = 1f + (r.F() - 0.5f) * 2f * sp.TintJitter + patch * 0.18f;
        Color warm = sp.Family == VegFamily.Pine ? FMath.HexLinear(0x557a3e) : FMath.HexLinear(0x9aa044);
        Color cool = sp.Family == VegFamily.Pine ? FMath.HexLinear(0x2f5a44) : FMath.HexLinear(0x3f7040);
        float t = r.F();
        c = c.Lerp(t < 0.5f ? warm : cool, MathF.Abs(t - 0.5f) * 0.5f + MathF.Max(patch, 0) * 0.15f);
        // a few early-autumn trees for charm
        if (sp.Family != VegFamily.Pine && r.F() < 0.035f)
            c = FMath.HexLinear(sp.Family == VegFamily.Birch ? 0xd8b040u : 0xc8782au).Lerp(c, 0.25f);
        return new Color(c.R * v, c.G * v, c.B * v, 0);
    }

    Color PlantTint(VegSpecies sp, float x, float z, ref Rng r)
    {
        Color c = sp.Tint;
        float v = 1f + (r.F() - 0.5f) * 2f * sp.TintJitter + _nTint.Fbm(x / 40f, z / 40f, 2) * 0.15f;
        c = c.Lerp(FMath.HexLinear(0x9aa044), r.F() * 0.25f);
        return new Color(c.R * v, c.G * v, c.B * v, 0);
    }

    Color RockTint(float x, float z, ref Rng r)
    {
        float v = 0.9f + 0.2f * r.F();
        return new Color(v, v, v * 0.98f, 0);
    }

    // ------------------------------------------------------------------------------------------ rendering

    Material MakeMaterial(VegSpecies sp, float lodMin, float lodMax)
    {
        var m = (ShaderMaterial)Models.MaterialFor(MatKind.Multi).Duplicate();
        m.SetShaderParameter("lod_min", lodMin);
        m.SetShaderParameter("lod_max", lodMax);
        m.SetShaderParameter("lod_fade", MathF.Max(4f, lodMax > 1000 ? 8f : lodMax * 0.08f));
        m.SetShaderParameter("wind_amount", sp.Wind);
        m.SetShaderParameter("sway_height", MathF.Max(sp.Height, 0.5f));
        m.SetShaderParameter("flutter", sp.Flutter);
        m.SetShaderParameter("leaf_backlight", sp.Backlight);
        m.SetShaderParameter("roughness", sp.Family is VegFamily.RockS or VegFamily.RockM or VegFamily.RockL or VegFamily.Cliff ? 0.85f : 0.8f);
        return m;
    }

    static MultiMeshInstance3D MakeMmi(string name, Mesh mesh, List<Inst> list, Material mat, float visEnd, bool shadow)
    {
        var mm = new MultiMesh
        {
            TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
            UseCustomData = true,
            Mesh = mesh,
            InstanceCount = list.Count,
        };
        var buf = new float[list.Count * 16];
        for (int i = 0; i < list.Count; i++)
        {
            var t = list[i].X; var c = list[i].Custom;
            int o = i * 16;
            buf[o + 0] = t.Basis.X.X; buf[o + 1] = t.Basis.Y.X; buf[o + 2] = t.Basis.Z.X; buf[o + 3] = t.Origin.X;
            buf[o + 4] = t.Basis.X.Y; buf[o + 5] = t.Basis.Y.Y; buf[o + 6] = t.Basis.Z.Y; buf[o + 7] = t.Origin.Y;
            buf[o + 8] = t.Basis.X.Z; buf[o + 9] = t.Basis.Y.Z; buf[o + 10] = t.Basis.Z.Z; buf[o + 11] = t.Origin.Z;
            buf[o + 12] = c.R; buf[o + 13] = c.G; buf[o + 14] = c.B; buf[o + 15] = c.A;
        }
        mm.Buffer = buf;
        return new MultiMeshInstance3D
        {
            Name = name, Multimesh = mm, MaterialOverride = mat,
            CastShadow = shadow ? GeometryInstance3D.ShadowCastingSetting.On : GeometryInstance3D.ShadowCastingSetting.Off,
            GIMode = GeometryInstance3D.GIModeEnum.Disabled,
            VisibilityRangeEnd = visEnd,
        };
    }

    // ------------------------------------------------------------------------------------------ collision

    void BuildCollision(List<Inst>[][] perChunk)
    {
        var space = GetWorld3D().Space;
        for (int c = 0; c < perChunk.Length; c++)
        {
            Rid body = default;
            for (int s = 0; s < Species.Count; s++)
            {
                var sp = Species[s];
                Shape3D shape = null; Transform3D local = Transform3D.Identity;
                if (sp.ColRadius > 0 && sp.ColHeight > 0)
                {
                    string key = $"cyl{sp.ColRadius:F3}_{sp.ColHeight:F2}";
                    if (!_shapes.TryGetValue(key, out shape)) _shapes[key] = shape = new CylinderShape3D { Radius = sp.ColRadius, Height = sp.ColHeight };
                    local = new Transform3D(Basis.Identity, new Vector3(0, sp.ColHeight * 0.5f, 0));
                }
                else if (sp.ColBoxSize != Vector3.Zero && sp.Family != VegFamily.RockS)
                {
                    string key = $"box{sp.Name}";
                    if (!_shapes.TryGetValue(key, out shape)) _shapes[key] = shape = new BoxShape3D { Size = sp.ColBoxSize };
                    local = new Transform3D(Basis.Identity, sp.ColBoxCenter);
                }
                if (shape == null) continue;
                foreach (var inst in perChunk[c][s])
                {
                    if (!body.IsValid)
                    {
                        body = PhysicsServer3D.BodyCreate();
                        PhysicsServer3D.BodySetMode(body, PhysicsServer3D.BodyMode.Static);
                        PhysicsServer3D.BodySetSpace(body, space);
                        PhysicsServer3D.BodySetCollisionLayer(body, App.LayerProps);
                        PhysicsServer3D.BodySetCollisionMask(body, 0);
                        _bodies.Add(body);
                    }
                    // shapes don't take non-uniform scale: keep the instance rotation+uniform scale on the offset only
                    var x = inst.X;
                    float sc = x.Basis.Scale.X;
                    var rot = new Basis(x.Basis.GetRotationQuaternion());
                    var t = new Transform3D(rot, x.Origin) * new Transform3D(Basis.Identity, local.Origin * sc);
                    Shape3D useShape = shape;
                    if (MathF.Abs(sc - 1f) > 0.08f)
                    {
                        string key = $"{sp.Name}@{MathF.Round(sc * 10f) / 10f:F1}";
                        if (!_shapes.TryGetValue(key, out useShape))
                        {
                            float k = MathF.Round(sc * 10f) / 10f;
                            useShape = shape is CylinderShape3D cy ? new CylinderShape3D { Radius = cy.Radius * k, Height = cy.Height * k }
                                     : shape is BoxShape3D bx ? new BoxShape3D { Size = bx.Size * k } : shape;
                            _shapes[key] = useShape;
                        }
                    }
                    PhysicsServer3D.BodyAddShape(body, useShape.GetRid(), t);
                }
            }
        }
    }

    // ------------------------------------------------------------------------------------------ queries

    /// <summary>Tree trunk positions (x, z) — e.g. for NPC avoidance or woodcutter jobs.</summary>
    public IReadOnlyList<(Vector2 p, int sp)> Trees => _trees;

    /// <summary>Selftest helper: trees must respect the spec exclusions.</summary>
    public (int checkedN, int violations, string info) VerifyExclusions(Heightfield hf)
    {
        int bad = 0; string first = "";
        foreach (var (p, _) in _trees)
        {
            string why = null;
            if (hf.RoadDistance(p.X, p.Y) < RegionSpec.ClearRoad) why = "road";
            else if ((p - RegionSpec.VillageCenter).Length() < RegionSpec.ClearVillage) why = "village";
            else if ((p - RegionSpec.Inn).Length() < RegionSpec.ClearInn) why = "inn";
            else if ((p - RegionSpec.GoblinCamp).Length() < RegionSpec.ClearCamp) why = "camp";
            else
            {
                float d = hf.StreamDistance(p.X, p.Y, out float s);
                if (d < hf.StreamHalfWidthAt(s) + RegionSpec.ClearStream) why = "stream";
            }
            if (why != null) { bad++; if (first == "") first = $"(first: {why} at {p})"; }
        }
        return (_trees.Count, bad, first);
    }
}
