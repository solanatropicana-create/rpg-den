using System;
using System.Collections.Generic;
using Godot;
using FD.Core;
using static FD.Core.FMath;

namespace FD.World;

/// <summary>
/// Dense grass, flowers and reeds in a ~55 m radius around the active camera. The ground is divided into
/// 24 m tiles; tiles entering the radius are generated deterministically (same tile → same clumps, so nothing
/// swims when the camera moves) and dropped when they leave. Clumps shrink into the ground toward the edge
/// (grass.gdshader) so there is no visible border. Density follows <see cref="Heightfield.Biome"/>: thick in
/// meadows, normal on grass, sparse under trees, none on roads, fields, rock or water; reeds line the stream
/// and pond edges.
/// </summary>
public partial class GrassField : Node3D
{
    public const float TileSize = 24f;
    [Export] public float Radius = 54f;
    [Export] public float Spacing = 0.7f;
    [Export] public int MaxTileBuildsPerFrame = 2;

    enum Kind { Grass1, Grass2, Flower1, Flower2, Reeds, Wheat, Cabbage, Count }
    static readonly string[] Names = { "grass_clump", "grass_clump_2", "flower_clump", "flower_clump_2", "reeds", "field_crop_wheat", "field_crop_cabbage" };

    readonly Mesh[] _mesh = new Mesh[(int)Kind.Count];
    readonly ShaderMaterial[] _mat = new ShaderMaterial[(int)Kind.Count];
    readonly Dictionary<Vector2I, Node3D> _tiles = new();
    Heightfield _hf;
    bool _first = true;
    public int LiveInstances { get; private set; }

    public void Init(Heightfield hf, Vegetation veg)
    {
        Name = "Grass";
        _hf = hf;
        var placeholders = Models.Exists("nature") ? null : PlaceholderVeg.Build();
        var terrain = Region.Current?.Terrain;
        var baseMat = (ShaderMaterial)Models.MaterialFor(MatKind.Grass);
        for (int k = 0; k < (int)Kind.Count; k++)
        {
            bool crop = (Kind)k is Kind.Wheat or Kind.Cabbage;
            if (crop) _mesh[k] = Models.Exists("village") ? Models.Mesh("village", Names[k], MatKind.Grass) : null;
            else _mesh[k] = placeholders != null ? placeholders[Names[k]] : Models.Mesh("nature", Names[k], MatKind.Grass);
            var m = (ShaderMaterial)baseMat.Duplicate();
            if (terrain?.Material != null)
            {
                m.SetShaderParameter("noise_tex", terrain.Material.GetShaderParameter("noise_tex"));
                m.SetShaderParameter("splat_b", terrain.SplatB);
                foreach (var p in new[] { "grass_lush", "grass_dry", "grass_deep", "meadow_col" })
                {
                    var v = terrain.Material.GetShaderParameter(p);
                    if (v.VariantType != Variant.Type.Nil) m.SetShaderParameter(p, v);
                }
            }
            m.SetShaderParameter("fade_start", Radius * 0.72f);
            m.SetShaderParameter("fade_end", Radius);
            bool reeds = (Kind)k == Kind.Reeds;
            m.SetShaderParameter("sway_height", reeds ? 1.7f : (Kind)k == Kind.Wheat ? 1.05f : (Kind)k == Kind.Cabbage ? 50f : 0.55f);
            m.SetShaderParameter("wind_amount", reeds ? 0.3f : (Kind)k == Kind.Wheat ? 0.35f : 0.22f);
            if (crop) { m.SetShaderParameter("base_dark", 0.75f); m.SetShaderParameter("up_normal", 0.5f); }
            _mat[k] = m;
        }
    }

    public override void _Process(double delta)
    {
        if (_hf == null) return;
        var cam = GetViewport().GetCamera3D();
        if (cam == null) return;
        Vector3 cp = cam.GlobalPosition;
        // don't grow grass for a camera high above the ground (overview shots)
        float above = cp.Y - _hf.Height(cp.X, cp.Z);
        float radius = above > Radius ? 0f : Radius;

        var want = new List<(Vector2I key, float d)>();
        if (radius > 0)
        {
            int r = (int)MathF.Ceiling(radius / TileSize) + 1;
            int tx0 = (int)MathF.Floor(cp.X / TileSize), tz0 = (int)MathF.Floor(cp.Z / TileSize);
            for (int dz = -r; dz <= r; dz++)
                for (int dx = -r; dx <= r; dx++)
                {
                    var key = new Vector2I(tx0 + dx, tz0 + dz);
                    float cx = (key.X + 0.5f) * TileSize, cz = (key.Y + 0.5f) * TileSize;
                    // distance from the camera to the tile rectangle
                    float ex = MathF.Max(MathF.Abs(cp.X - cx) - TileSize * 0.5f, 0), ez = MathF.Max(MathF.Abs(cp.Z - cz) - TileSize * 0.5f, 0);
                    float d = MathF.Sqrt(ex * ex + ez * ez);
                    if (d <= radius) want.Add((key, d));
                }
        }
        var wanted = new HashSet<Vector2I>();
        foreach (var w in want) wanted.Add(w.key);
        // drop tiles that left (with a little hysteresis)
        var drop = new List<Vector2I>();
        foreach (var (key, node) in _tiles)
        {
            if (wanted.Contains(key)) continue;
            float cx = (key.X + 0.5f) * TileSize, cz = (key.Y + 0.5f) * TileSize;
            if (radius == 0 || new Vector2(cp.X - cx, cp.Z - cz).Length() > radius + TileSize * 1.2f) drop.Add(key);
        }
        foreach (var k in drop)
        {
            LiveInstances -= _tiles[k].GetMeta("n", 0).AsInt32();
            _tiles[k].QueueFree(); _tiles.Remove(k);
        }
        // build missing tiles nearest first
        want.Sort((a, b) => a.d.CompareTo(b.d));
        int built = 0;
        foreach (var (key, _) in want)
        {
            if (_tiles.ContainsKey(key)) continue;
            _tiles[key] = BuildTile(key);
            if (!_first && ++built >= MaxTileBuildsPerFrame) break;
        }
        _first = false;
    }

    Node3D BuildTile(Vector2I key)
    {
        var hf = _hf;
        var node = new Node3D { Name = $"T{key.X}_{key.Y}" };
        AddChild(node);
        var lists = new List<(Transform3D x, Color c)>[(int)Kind.Count];
        for (int k = 0; k < lists.Length; k++) lists[k] = new List<(Transform3D, Color)>();
        float x0 = key.X * TileSize, z0 = key.Y * TileSize;
        int n = (int)(TileSize / Spacing);
        for (int j = 0; j < n; j++)
            for (int i = 0; i < n; i++)
            {
                var r = new Rng(Hash.U32(key.X * 4096 + i, key.Y * 4096 + j, RegionSpec.Seed, 9));
                float x = x0 + (i + r.F()) * Spacing, z = z0 + (j + r.F()) * Spacing;
                if (!RegionSpec.InRegion(x, z)) continue;
                float clear = hf.Clearance(x, z, ClearKind.Ground);
                if (clear <= 0.05f) continue;
                // reeds along water edges
                float sd = hf.StreamDistance(x, z, out float s);
                float whw = hf.StreamWaterHalfWidthAt(s);
                float pr = (new Vector2(x, z) - RegionSpec.Pond).Length();
                float pondEdge = pr < RegionSpec.PondRadius * 1.5f ? pr - hf.PondRadiusAt(new Vector2(x, z)) : 99f;
                bool nearStream = sd > whw - 0.8f && sd < whw + 1.4f;
                bool nearPond = pondEdge > -1.2f && pondEdge < 1.8f;
                if ((nearStream || nearPond) && r.F() < 0.42f)
                {
                    float y = hf.Height(x, z);
                    if (nearStream && y > hf.StreamWaterAt(s) + 0.6f) continue;
                    var t = Place(x, z, y - 0.05f, Lerp(0.8f, 1.25f, r.F()), ref r, 0.1f);
                    float v = 0.9f + 0.2f * r.F();
                    var tint = FMath.HexLinear(0x7f9a4a).Lerp(FMath.HexLinear(0xa09a58), r.F() * 0.35f);
                    lists[(int)Kind.Reeds].Add((t, new Color(tint.R * v, tint.G * v, tint.B * v, 1f)));
                    continue;
                }
                if (hf.IsWater(x, z)) continue;
                var b = hf.Biome(x, z);
                float dens = (b.Grass * 0.85f + b.Meadow * 1.0f + b.Forest * 0.22f + b.Field * 0.08f + b.Sand * 0.1f) * clear;
                // tufts along the edges of dirt paths
                dens += b.Dirt * (1f - b.Dirt) * 1.2f;
                dens *= 1f - Smoothstep(25f, 38f, hf.Slope(x, z));
                if (r.F() > dens) continue;
                float pFlower = 0.025f + 0.22f * b.Meadow;
                float yy = hf.Height(x, z) - 0.02f;
                Kind kind;
                float roll = r.F();
                if (roll < pFlower) kind = r.F() < 0.5f ? Kind.Flower1 : Kind.Flower2;
                else kind = r.F() < 0.62f ? Kind.Grass1 : Kind.Grass2;
                float sc = kind is Kind.Flower1 or Kind.Flower2 ? Lerp(0.8f, 1.2f, r.F()) : Lerp(0.85f, 1.45f, r.F()) * (0.8f + 0.4f * b.Meadow);
                var xf = Place(x, z, yy, sc, ref r, 0.25f);
                float var = 0.9f + 0.22f * r.F();
                lists[(int)kind].Add((xf, new Color(var, var * (0.98f + 0.04f * r.F()), var * (0.95f + 0.08f * r.F()), 0f)));
            }
        AddCrops(x0, z0, lists);
        int total = 0;
        for (int k = 0; k < lists.Length; k++)
        {
            var list = lists[k];
            if (list.Count == 0 || _mesh[k] == null) continue;
            total += list.Count;
            var mm = new MultiMesh
            {
                TransformFormat = MultiMesh.TransformFormatEnum.Transform3D, UseCustomData = true,
                Mesh = _mesh[k], InstanceCount = list.Count,
            };
            var buf = new float[list.Count * 16];
            for (int i = 0; i < list.Count; i++)
            {
                var (t, c) = list[i];
                int o = i * 16;
                buf[o] = t.Basis.X.X; buf[o + 1] = t.Basis.Y.X; buf[o + 2] = t.Basis.Z.X; buf[o + 3] = t.Origin.X;
                buf[o + 4] = t.Basis.X.Y; buf[o + 5] = t.Basis.Y.Y; buf[o + 6] = t.Basis.Z.Y; buf[o + 7] = t.Origin.Y;
                buf[o + 8] = t.Basis.X.Z; buf[o + 9] = t.Basis.Y.Z; buf[o + 10] = t.Basis.Z.Z; buf[o + 11] = t.Origin.Z;
                buf[o + 12] = c.R; buf[o + 13] = c.G; buf[o + 14] = c.B; buf[o + 15] = c.A;
            }
            mm.Buffer = buf;
            node.AddChild(new MultiMeshInstance3D
            {
                Name = Names[k], Multimesh = mm, MaterialOverride = _mat[k],
                CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
                GIMode = GeometryInstance3D.GIModeEnum.Disabled,
            });
        }
        LiveInstances += total;
        node.SetMeta("n", total);
        return node;
    }

    /// <summary>Crop rows in field plots overlapping this tile (wheat/barley stalks, cabbages), aligned with the
    /// furrows the terrain shader paints (0.9 m apart).</summary>
    void AddCrops(float x0, float z0, List<(Transform3D x, Color c)>[] lists)
    {
        if (_mesh[(int)Kind.Wheat] == null && _mesh[(int)Kind.Cabbage] == null) return;
        var hf = _hf;
        var tile = new Rect2(x0, z0, TileSize, TileSize);
        for (int fi = 0; fi < RegionSpec.Fields.Length; fi++)
        {
            var f = RegionSpec.Fields[fi];
            Kind kind = f.Crop is CropKind.Wheat or CropKind.Barley ? Kind.Wheat : f.Crop == CropKind.Vegetables ? Kind.Cabbage : Kind.Count;
            if (kind == Kind.Count || _mesh[(int)kind] == null) continue;
            float reach = f.HalfSize.Length();
            if (!tile.Grow(reach).HasPoint(f.Center)) continue;
            Vector2 u = f.U, v = f.V;
            float along = kind == Kind.Wheat ? 0.42f : 0.6f, across = kind == Kind.Wheat ? 0.45f : 0.9f;
            // rows run along U; only visit the part of the plot overlapping the tile
            int nu = (int)(f.HalfSize.X * 2f / along), nv = (int)(f.HalfSize.Y * 2f / across);
            // row index offset so the rows sit in the furrow crests of the shader's pattern (across = dot(p, V))
            for (int j = 0; j < nv; j++)
            {
                float vv = -f.HalfSize.Y + 0.6f + j * across;
                if (vv > f.HalfSize.Y - 0.6f) break;
                for (int i = 0; i < nu; i++)
                {
                    float uu = -f.HalfSize.X + 0.6f + i * along;
                    if (uu > f.HalfSize.X - 0.6f) break;
                    Vector2 p = f.Center + u * uu + v * vv;
                    if (!tile.HasPoint(p)) continue;
                    var r = new Rng(Hash.U32(fi * 7919 + i, j, RegionSpec.Seed, 17));
                    p += u * (r.F() - 0.5f) * along * 0.5f + v * (r.F() - 0.5f) * 0.08f;
                    if (hf.Clearance(p.X, p.Y, ClearKind.Ground) < 0.3f) continue;
                    if (kind == Kind.Cabbage && r.F() < 0.12f) continue;
                    float sc = kind == Kind.Wheat ? Lerp(0.85f, 1.15f, r.F()) : Lerp(0.8f, 1.2f, r.F());
                    var xf = Place(p.X, p.Y, hf.Height(p.X, p.Y) - 0.03f, sc, ref r, 0.12f);
                    Color c;
                    if (kind == Kind.Wheat)
                    {
                        var ripe = FMath.HexLinear(f.Crop == CropKind.Wheat ? 0xd8b44au : 0xc6c060u);
                        var young = FMath.HexLinear(0x9aa84au);
                        var t = ripe.Lerp(young, r.F() * 0.25f);
                        float vv2 = 0.9f + 0.2f * r.F();
                        c = new Color(t.R * vv2, t.G * vv2, t.B * vv2, 1f);
                    }
                    else c = new Color(1, 1, 1, 1f);
                    lists[(int)kind].Add((xf, c));
                }
            }
        }
    }

    Transform3D Place(float x, float z, float y, float scale, ref Rng r, float tiltAmount)
    {
        Vector3 nrm = _hf.Normal(x, z);
        Vector3 up = Vector3.Up.Lerp(nrm, 0.5f).Normalized();
        up = (up + new Vector3((r.F() - 0.5f) * tiltAmount, 0, (r.F() - 0.5f) * tiltAmount)).Normalized();
        var basis = new Basis(new Quaternion(Vector3.Up, up)) * Basis.FromEuler(new Vector3(0, r.F() * MathF.Tau, 0));
        return new Transform3D(basis.Scaled(new Vector3(scale, scale, scale)), new Vector3(x, y, z));
    }

    public int TileCount => _tiles.Count;
}
