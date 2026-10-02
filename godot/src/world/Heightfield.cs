using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using FD.Core;
using static FD.Core.FMath;

namespace FD.World;

/// <summary>Effective ground-cover weights at a point (sum ≈ 1), matching what the terrain shader paints.</summary>
public struct BiomeWeights
{
    public float Grass, Meadow, Forest, Dirt, Rock, Sand, Field;
    public override readonly string ToString() =>
        $"grass {Grass:F2} meadow {Meadow:F2} forest {Forest:F2} dirt {Dirt:F2} rock {Rock:F2} sand {Sand:F2} field {Field:F2}";
}

/// <summary>What a vegetation clearance query is for (different kinds keep different distances).</summary>
public enum ClearKind { Tree, Bush, Ground }

/// <summary>
/// Deterministic terrain for the 1200 × 1200 m region: 601 × 601 height samples at 2 m spacing, x/z ∈ [−600, 600].
/// <para>Life cycle: <c>new Heightfield()</c> shapes the natural terrain and every spec feature (hills, stream,
/// roads, pads for village/inn/camp/ruin, pond). Builders then call <see cref="AddPad"/>, <see cref="AddPadCircle"/>,
/// <see cref="AddPath"/> and <see cref="ExcludeVegetation"/>; finally <see cref="Bake"/> computes the ground-cover
/// splat maps. Terrain meshes, vegetation and water must be built AFTER Bake.</para>
/// <para><see cref="Height"/> interpolates exactly like the rendered/collision mesh (two triangles per 2 m cell,
/// split along the (x+1, z)–(x, z+1) diagonal), so objects placed with it sit exactly on the visible ground.</para>
/// </summary>
public sealed class Heightfield
{
    public const int N = 601;
    public const float Cell = 2f;
    public const float Min = -RegionSpec.HalfSize;
    public const float Max = RegionSpec.HalfSize;
    /// <summary>Splat (ground cover) texture resolution: 1 texel per metre.</summary>
    public const int SplatRes = 1200;

    /// <summary>Height samples, row-major: H[iz * N + ix] at (Min + ix*Cell, Min + iz*Cell).</summary>
    public readonly float[] H = new float[N * N];

    public readonly DistanceField RoadField, TrailField, SpurField, StreamField;

    // Stream profile, sampled every StreamDs metres of arc length along RegionSpec.Stream.
    public const float StreamDs = 2f;
    public float[] StreamBank, StreamDepth, StreamHalfWidth, StreamWater, StreamWaterHalfWidth;
    /// <summary>Pond water surface height and outline radius function.</summary>
    public float PondLevel { get; private set; }

    /// <summary>Splat maps (RGBA8, SplatRes², row-major, row = z). A: dirt, field, rock, sand. B: forest, meadow,
    /// furrow angle (0..1 → 0..π), cavity (0.5 neutral, lower = crease). C: crop index/4, worn, wet, reserved.</summary>
    public byte[] SplatA, SplatB, SplatC;
    public bool IsBaked { get; private set; }

    /// <summary>Height of the flat inn plot (40 × 30 m at <see cref="RegionSpec.Inn"/>).</summary>
    public float InnPadHeight { get; private set; }
    /// <summary>Ruin plateau height (flat top of the SW hill).</summary>
    public float RuinTopHeight { get; private set; }
    /// <summary>Road height at the stream crossing (deck level for a future bridge).</summary>
    public float BridgeRoadHeight { get; private set; }

    readonly Noise2 _nRoll, _nRoll2, _nHill, _nRock, _nEdge, _nForest, _nGrove, _nGlade, _nMeadow, _nStream, _nPond, _nMisc;
    readonly byte[] _exclusion = new byte[N * N];         // 255 = no vegetation at all
    readonly List<(Polyline line, float halfWidth, float wear)> _strokes = new();
    readonly List<(Vector2 c, Vector2 half, float angleDeg, float wear, float margin)> _padWear = new();
    float[] _profRoad, _profTrail, _profSpur;

    public Heightfield(int seed = RegionSpec.Seed)
    {
        _nRoll = new Noise2(seed); _nRoll2 = new Noise2(seed + 1); _nHill = new Noise2(seed + 2);
        _nRock = new Noise2(seed + 3); _nEdge = new Noise2(seed + 4); _nForest = new Noise2(seed + 5);
        _nGrove = new Noise2(seed + 6); _nGlade = new Noise2(seed + 7); _nMeadow = new Noise2(seed + 8);
        _nStream = new Noise2(seed + 9); _nPond = new Noise2(seed + 10); _nMisc = new Noise2(seed + 11);

        DistanceField road = null, trail = null, spur = null, stream = null;
        Parallel.Invoke(
            () => road = new DistanceField(RegionSpec.Road, N, Cell, Min, 40f),
            () => { trail = new DistanceField(RegionSpec.Trail, N, Cell, Min, 30f); spur = new DistanceField(RegionSpec.CampSpur, N, Cell, Min, 16f); },
            () => stream = new DistanceField(RegionSpec.Stream, N, Cell, Min, 52f));
        RoadField = road; TrailField = trail; SpurField = spur; StreamField = stream;
        BuildBase();
        ShapeVillage();
        InnPadHeight = AddPad(RegionSpec.Inn, RegionSpec.InnPadHalfSize, RegionSpec.InnAngleDeg, float.NaN, 14f, wear: 0f, vegetationMargin: -1f);
        CompressDisc(RegionSpec.GoblinCamp, RegionSpec.CampClearingRadius, 22f, 0.3f);
        ShapeRuinTop();
        ShapeStreamValley();
        _profRoad = CarvePath(RegionSpec.Road, RoadField, RegionSpec.RoadWidth * 0.5f, 9f, 14f, 0.12f,
                              pin: (RegionSpec.BridgeRoadS, StreamBankAt(RegionSpec.BridgeStreamS), 9f, 42f));
        _profTrail = CarvePath(RegionSpec.Trail, TrailField, RegionSpec.TrailWidth * 0.5f, 5f, 7f, 0.08f);
        _profSpur = CarvePath(RegionSpec.CampSpur, SpurField, RegionSpec.CampSpurWidth * 0.5f, 3f, 4f, 0.05f);
        // the inn plot is re-flattened after the roads (the road shoulder passes ~5 m in front of it)
        AddPad(RegionSpec.Inn, RegionSpec.InnPadHalfSize, RegionSpec.InnAngleDeg, InnPadHeight, 5f, wear: 0f, vegetationMargin: -1f);
        CarveStreamChannel();
        ShapePond();
        BridgeRoadHeight = ProfileAt(_profRoad, RegionSpec.BridgeRoadS, 2f);
    }

    // =====================================================================================================
    // Queries
    // =====================================================================================================

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static int Idx(int ix, int iz) => iz * N + ix;
    public static float GX(int ix) => Min + ix * Cell;

    /// <summary>Terrain height at world (x, z), interpolated exactly like the terrain mesh triangles.</summary>
    public float Height(float x, float z)
    {
        float fx = (x - Min) / Cell, fz = (z - Min) / Cell;
        int ix = Math.Clamp((int)MathF.Floor(fx), 0, N - 2), iz = Math.Clamp((int)MathF.Floor(fz), 0, N - 2);
        float tx = Math.Clamp(fx - ix, 0f, 1f), tz = Math.Clamp(fz - iz, 0f, 1f);
        int k = iz * N + ix;
        float a = H[k], b = H[k + 1], c = H[k + N], d = H[k + N + 1];
        if (tx + tz <= 1f) return a + (b - a) * tx + (c - a) * tz;
        return d + (c - d) * (1f - tx) + (b - d) * (1f - tz);
    }

    /// <summary>Bilinear height (smoother than <see cref="Height"/>, not exactly on the mesh).</summary>
    public float HeightBilinear(float x, float z)
    {
        float fx = (x - Min) / Cell, fz = (z - Min) / Cell;
        int ix = Math.Clamp((int)MathF.Floor(fx), 0, N - 2), iz = Math.Clamp((int)MathF.Floor(fz), 0, N - 2);
        float tx = Math.Clamp(fx - ix, 0f, 1f), tz = Math.Clamp(fz - iz, 0f, 1f);
        int k = iz * N + ix;
        float a = H[k] + (H[k + 1] - H[k]) * tx;
        float b = H[k + N] + (H[k + N + 1] - H[k + N]) * tx;
        return a + (b - a) * tz;
    }

    /// <summary>Smooth surface normal (central differences over ±1 cell, bilinear).</summary>
    public Vector3 Normal(float x, float z)
    {
        float dx = HeightBilinear(x + Cell, z) - HeightBilinear(x - Cell, z);
        float dz = HeightBilinear(x, z + Cell) - HeightBilinear(x, z - Cell);
        return new Vector3(-dx, 2f * Cell, -dz).Normalized();
    }

    /// <summary>Normal at grid sample (ix, iz) (used for mesh vertices).</summary>
    public Vector3 GridNormal(int ix, int iz)
    {
        int x0 = Math.Max(ix - 1, 0), x1 = Math.Min(ix + 1, N - 1), z0 = Math.Max(iz - 1, 0), z1 = Math.Min(iz + 1, N - 1);
        float dx = (H[Idx(x1, iz)] - H[Idx(x0, iz)]) / ((x1 - x0) * Cell);
        float dz = (H[Idx(ix, z1)] - H[Idx(ix, z0)]) / ((z1 - z0) * Cell);
        return new Vector3(-dx, 1f, -dz).Normalized();
    }

    /// <summary>Slope in degrees (0 = flat).</summary>
    public float Slope(float x, float z) => Mathf.RadToDeg(MathF.Acos(Math.Clamp(Normal(x, z).Y, -1f, 1f)));

    public Vector3 Ground(float x, float z) => new(x, Height(x, z), z);
    public Vector3 Ground(Vector2 p) => new(p.X, Height(p.X, p.Y), p.Y);

    // ---- stream profile -------------------------------------------------------------------------------

    static float ProfileAt(float[] prof, float s, float ds)
    {
        float f = s / ds;
        int i = Math.Clamp((int)f, 0, prof.Length - 2);
        float t = Math.Clamp(f - i, 0f, 1f);
        return prof[i] + (prof[i + 1] - prof[i]) * t;
    }

    public float StreamBankAt(float s) => ProfileAt(StreamBank, s, StreamDs);
    public float StreamDepthAt(float s) => ProfileAt(StreamDepth, s, StreamDs);
    public float StreamHalfWidthAt(float s) => ProfileAt(StreamHalfWidth, s, StreamDs);
    /// <summary>Water surface height of the stream at arc length s (flows toward s = 0, the north edge).</summary>
    public float StreamWaterAt(float s) => ProfileAt(StreamWater, s, StreamDs);
    /// <summary>Half width of the water surface at arc length s.</summary>
    public float StreamWaterHalfWidthAt(float s) => ProfileAt(StreamWaterHalfWidth, s, StreamDs);
    public float RoadProfileAt(float s) => ProfileAt(_profRoad, s, 2f);

    /// <summary>Distance from (x, z) to the stream centreline and arc length of the nearest point.</summary>
    public float StreamDistance(float x, float z, out float s) { s = StreamField.ArcAt(x, z); return StreamField.Dist(x, z); }
    public float RoadDistance(float x, float z) => RoadField.Dist(x, z);
    public float TrailDistance(float x, float z) => MathF.Min(TrailField.Dist(x, z), SpurField.Dist(x, z));

    /// <summary>True if (x, z) is under stream or pond water.</summary>
    public bool IsWater(float x, float z)
    {
        float d = StreamDistance(x, z, out float s);
        if (d < StreamWaterHalfWidthAt(s) && Height(x, z) < StreamWaterAt(s)) return true;
        return (new Vector2(x, z) - RegionSpec.Pond).Length() < PondRadiusAt(new Vector2(x, z)) + 2f && Height(x, z) < PondLevel;
    }

    public float PondRadiusAt(Vector2 p)
    {
        Vector2 d = (p - RegionSpec.Pond);
        float a = MathF.Atan2(d.Y, d.X);
        return RegionSpec.PondRadius * (1f + 0.13f * _nPond.Get(MathF.Cos(a) * 1.3f + 5f, MathF.Sin(a) * 1.3f + 5f)
                                          + 0.05f * _nPond.Get(MathF.Cos(a) * 3.1f - 2f, MathF.Sin(a) * 3.1f));
    }

    // =====================================================================================================
    // Builder API (call before Bake)
    // =====================================================================================================

    void CheckNotBaked(string what)
    {
        if (IsBaked) GD.PushError($"Heightfield.{what} called after Bake(): terrain meshes will not reflect it.");
    }

    /// <summary>
    /// Flatten a rotated rectangle (a building plot). Returns the pad height actually used.
    /// </summary>
    /// <param name="center">Pad centre (x, z).</param>
    /// <param name="halfExtents">Half size along the pad's own axes (U = angle direction, V = U rotated +90°).</param>
    /// <param name="angleDeg">Direction of the U axis in the ground plane, atan2(z, x) in degrees.</param>
    /// <param name="height">Target height; NaN = mean terrain height under the footprint.</param>
    /// <param name="blend">Distance (m) over which the pad blends back into natural terrain.</param>
    /// <param name="wear">0..1 trampled dirt painted on the footprint (+1 m).</param>
    /// <param name="vegetationMargin">Vegetation is cleared this far beyond the footprint (negative = don't clear).</param>
    public float AddPad(Vector2 center, Vector2 halfExtents, float angleDeg = 0f, float height = float.NaN,
                        float blend = 3f, float wear = 0f, float vegetationMargin = 2f)
    {
        CheckNotBaked(nameof(AddPad));
        float a = Mathf.DegToRad(angleDeg);
        Vector2 u = new(MathF.Cos(a), MathF.Sin(a)), v = new(-u.Y, u.X);
        float reach = halfExtents.Length() + blend + Cell;
        if (float.IsNaN(height))
        {
            double sum = 0; int cnt = 0;
            ForSamples(center, halfExtents.Length() + Cell, (k, p) =>
            {
                Vector2 d = p - center;
                if (MathF.Abs(d.Dot(u)) <= halfExtents.X && MathF.Abs(d.Dot(v)) <= halfExtents.Y) { sum += H[k]; cnt++; }
            });
            height = cnt > 0 ? (float)(sum / cnt) : Height(center.X, center.Y);
        }
        float h = height;
        ForSamples(center, reach, (k, p) =>
        {
            Vector2 d = p - center;
            float lu = MathF.Abs(d.Dot(u)) - halfExtents.X, lv = MathF.Abs(d.Dot(v)) - halfExtents.Y;
            float sd = new Vector2(MathF.Max(lu, 0), MathF.Max(lv, 0)).Length() + MathF.Min(MathF.Max(lu, lv), 0);
            float w = 1f - Smoothstep(0f, MathF.Max(blend, 0.01f), sd);
            if (w > 0) H[k] = Lerp(H[k], h, w);
        });
        if (vegetationMargin >= 0)
            MarkExclusionRect(center, halfExtents + new Vector2(vegetationMargin, vegetationMargin), u, v);
        if (wear > 0) _padWear.Add((center, halfExtents, angleDeg, wear, 1f));
        return h;
    }

    /// <summary>Flatten a disc. Returns the height used (NaN = mean under the disc).</summary>
    public float AddPadCircle(Vector2 center, float radius, float height = float.NaN, float blend = 3f,
                              float wear = 0f, float vegetationMargin = 2f)
    {
        CheckNotBaked(nameof(AddPadCircle));
        if (float.IsNaN(height))
        {
            double sum = 0; int cnt = 0;
            ForSamples(center, radius, (k, p) => { if ((p - center).Length() <= radius) { sum += H[k]; cnt++; } });
            height = cnt > 0 ? (float)(sum / cnt) : Height(center.X, center.Y);
        }
        float h = height;
        ForSamples(center, radius + blend + Cell, (k, p) =>
        {
            float w = 1f - Smoothstep(radius, radius + MathF.Max(blend, 0.01f), (p - center).Length());
            if (w > 0) H[k] = Lerp(H[k], h, w);
        });
        if (vegetationMargin >= 0) ExcludeVegetation(center, radius + vegetationMargin);
        if (wear > 0) _padWear.Add((center, new Vector2(radius, radius), float.NaN, wear, 1f));
        return h;
    }

    /// <summary>
    /// Register a dirt path/lane stroke (village streets, yards, footpaths). The polyline is smoothed
    /// (Catmull-Rom); its bed is flattened across and smoothed along by <paramref name="flatten"/> (0..1),
    /// painted as dirt with strength <paramref name="wear"/>, and vegetation is cleared within width/2 + 1.5 m.
    /// </summary>
    public Polyline AddPath(IReadOnlyList<Vector2> points, float width, float flatten = 0.6f, float wear = 1f)
    {
        CheckNotBaked(nameof(AddPath));
        var line = points.Count > 2 ? Polyline.CatmullRom(points, 1.5f) : new Polyline(points);
        float hw = width * 0.5f;
        if (flatten > 0)
        {
            var field = new DistanceField(line, N, Cell, Min, hw + 6f);
            CarvePath(line, field, hw, 3f + hw, 5f, 0.04f, flatten);
        }
        _strokes.Add((line, hw, wear));
        // clear vegetation along the stroke
        for (float s = 0; s <= line.Length; s += 1f)
            ExcludeVegetation(line.PointAt(s), hw + 1.5f);
        return line;
    }

    /// <summary>Keep all vegetation (trees, bushes, grass) out of a disc.</summary>
    public void ExcludeVegetation(Vector2 center, float radius)
    {
        ForSamples(center, radius + Cell, (k, p) =>
        {
            float d = (p - center).Length();
            byte v = (byte)(255f * (1f - Smoothstep(radius - Cell, radius + Cell * 0.5f, d)));
            if (v > _exclusion[k]) _exclusion[k] = v;
        });
    }

    void MarkExclusionRect(Vector2 c, Vector2 half, Vector2 u, Vector2 v)
    {
        ForSamples(c, half.Length() + Cell, (k, p) =>
        {
            Vector2 d = p - c;
            float lu = MathF.Abs(d.Dot(u)) - half.X, lv = MathF.Abs(d.Dot(v)) - half.Y;
            float sd = MathF.Max(lu, lv);
            byte val = (byte)(255f * (1f - Smoothstep(-Cell, Cell * 0.5f, sd)));
            if (val > _exclusion[k]) _exclusion[k] = val;
        });
    }

    /// <summary>0 = vegetation allowed, 1 = excluded by a pad/path/exclusion registered by a builder.</summary>
    public float CustomExclusion(float x, float z)
    {
        float fx = (x - Min) / Cell, fz = (z - Min) / Cell;
        int ix = Math.Clamp((int)fx, 0, N - 2), iz = Math.Clamp((int)fz, 0, N - 2);
        float tx = Math.Clamp(fx - ix, 0, 1), tz = Math.Clamp(fz - iz, 0, 1);
        int k = iz * N + ix;
        float a = _exclusion[k] + (_exclusion[k + 1] - _exclusion[k]) * tx;
        float b = _exclusion[k + N] + (_exclusion[k + N + 1] - _exclusion[k + N]) * tx;
        return (a + (b - a) * tz) / 255f;
    }

    void ForSamples(Vector2 c, float r, Action<int, Vector2> f)
    {
        int x0 = Math.Max(0, (int)MathF.Floor((c.X - r - Min) / Cell)), x1 = Math.Min(N - 1, (int)MathF.Ceiling((c.X + r - Min) / Cell));
        int z0 = Math.Max(0, (int)MathF.Floor((c.Y - r - Min) / Cell)), z1 = Math.Min(N - 1, (int)MathF.Ceiling((c.Y + r - Min) / Cell));
        for (int iz = z0; iz <= z1; iz++)
            for (int ix = x0; ix <= x1; ix++)
                f(iz * N + ix, new Vector2(GX(ix), GX(iz)));
    }

    // =====================================================================================================
    // Natural terrain
    // =====================================================================================================

    static float Bump(float t) { if (t >= 1f) return 0f; float q = 1f - t * t; return q * q; }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveOptimization)]
    float BaseHeight(float x, float z)
    {
        // gentle rolling ground (±~7 m) with a slight regional tilt up toward the south-western hills
        float h = 10.5f * _nRoll.Fbm(x / 340f, z / 340f, 3) + 3.6f * _nRoll2.Fbm(x / 110f, z / 110f, 3)
                + 0.9f * _nRoll2.Get(x / 34f + 11f, z / 34f - 7f);
        h += 0.005f * (-x + z);

        // high south-western hills (the ruin hill is the highest)
        foreach (var hill in RegionSpec.SouthWestHills)
        {
            Vector2 d = new(x - hill.Center.X, z - hill.Center.Y);
            float warp = 1f + 0.22f * _nHill.Fbm(x / 90f + hill.Center.X, z / 90f, 2);
            float t = d.Length() / (hill.Radius * warp);
            float m = Bump(t);
            if (m <= 0) continue;
            float rough = hill.Roughness * (_nRock.Ridged(x / 60f, z / 60f, 3) - 0.45f) * 7f * Smoothstep(0.05f, 0.5f, t);
            h += hill.Height * m + rough * m;
        }

        // rocky north-western hills: ridged crests with exposed rock
        {
            Vector2 d = new(x - RegionSpec.RockyHills.X, z - RegionSpec.RockyHills.Y);
            float warp = 1f + 0.3f * _nHill.Fbm(x / 120f - 40f, z / 120f + 13f, 2);
            float m = Bump(d.Length() / (RegionSpec.RockyHillsRadius * warp));
            if (m > 0)
            {
                float r = _nRock.Ridged(x / 115f + 7f, z / 115f - 3f, 4);
                h += RegionSpec.RockyHillsHeight * MathF.Pow(m, 0.8f) * (0.3f + 1.0f * r);
            }
        }

        // hills toward the map border so the edge reads as a natural horizon
        float e = MathF.Max(MathF.Abs(x), MathF.Abs(z));
        float edge = Smoothstep(RegionSpec.EdgeHillsStart, RegionSpec.HalfSize + 20f, e);
        if (edge > 0) h += RegionSpec.EdgeHillsHeight * edge * (0.65f + 0.55f * _nEdge.Fbm(x / 70f, z / 70f, 3));
        return h;
    }

    void BuildBase()
    {
        Parallel.For(0, N, iz =>
        {
            float z = GX(iz);
            for (int ix = 0; ix < N; ix++) H[iz * N + ix] = BaseHeight(GX(ix), z);
        });
    }

    // =====================================================================================================
    // Spec features
    // =====================================================================================================

    /// <summary>Village: gentle, slightly tilted plane with undulations compressed to ±2 m.</summary>
    void ShapeVillage()
    {
        Vector2 c = RegionSpec.VillageCenter;
        float r = RegionSpec.VillagePadRadius;
        // least-squares plane over the disc
        double sw = 0, sx = 0, sz = 0, sxx = 0, szz = 0, sxz = 0, sh = 0, sxh = 0, szh = 0;
        ForSamples(c, r, (k, p) =>
        {
            Vector2 d = p - c;
            if (d.Length() > r) return;
            float h = H[k];
            sw += 1; sx += d.X; sz += d.Y; sxx += d.X * d.X; szz += d.Y * d.Y; sxz += d.X * d.Y;
            sh += h; sxh += d.X * h; szh += d.Y * h;
        });
        double mean = sh / sw;
        // centred coordinates: slope a = cov(x,h)/var(x) (x and z are nearly uncorrelated on a disc)
        double a = (sxh - sx * mean) / Math.Max(sxx - sx * sx / sw, 1e-6);
        double b = (szh - sz * mean) / Math.Max(szz - sz * sz / sw, 1e-6);
        Vector2 grad = new((float)a, (float)b);
        if (grad.Length() > 0.03f) grad = grad.Normalized() * 0.03f;      // at most ~1.7°
        float m = (float)mean;
        ForSamples(c, r + RegionSpec.VillagePadBlend, (k, p) =>
        {
            Vector2 d = p - c;
            float w = 1f - Smoothstep(r, r + RegionSpec.VillagePadBlend, d.Length());
            if (w <= 0) return;
            float plane = m + grad.Dot(d);
            float dev = Math.Clamp((H[k] - plane) * 0.3f, -RegionSpec.VillageMaxUndulation, RegionSpec.VillageMaxUndulation);
            H[k] = Lerp(H[k], plane + dev, w);
        });
    }

    /// <summary>Compress height deviations inside a disc (clearings).</summary>
    void CompressDisc(Vector2 c, float r, float blend, float keep)
    {
        double sum = 0; int cnt = 0;
        ForSamples(c, r, (k, p) => { if ((p - c).Length() <= r) { sum += H[k]; cnt++; } });
        float mean = (float)(sum / Math.Max(cnt, 1));
        ForSamples(c, r + blend, (k, p) =>
        {
            float w = 1f - Smoothstep(r, r + blend, (p - c).Length());
            if (w > 0) H[k] = Lerp(H[k], mean + (H[k] - mean) * keep, w);
        });
    }

    void ShapeRuinTop()
    {
        Vector2 c = RegionSpec.Ruin;
        // the plateau sits at the smoothed local maximum
        float top = 0; int cnt = 0;
        ForSamples(c, RegionSpec.RuinTopRadius + 4, (k, p) => { if ((p - c).Length() <= RegionSpec.RuinTopRadius + 4) { top += H[k]; cnt++; } });
        top /= Math.Max(cnt, 1);
        RuinTopHeight = top + 0.3f;
        float r0 = RegionSpec.RuinTopRadius + 1.5f;
        // neighbouring crests stay a few metres below the watchtower hill
        float capR = 300f;
        ForSamples(c, capR, (k, p) =>
        {
            float d = (p - c).Length();
            if (d < r0 + 14f || d > capR) return;
            float cap = RuinTopHeight - 2.5f - 0.015f * (d - r0);
            if (H[k] > cap - 3f) H[k] = SmoothMin(H[k], cap, 3f);
        });
        ForSamples(c, r0 + 14f, (k, p) =>
        {
            float d = (p - c).Length();
            float w = 1f - Smoothstep(r0, r0 + 11f, d);
            if (w > 0) H[k] = Lerp(H[k], RuinTopHeight, w);
        });
        ExcludeVegetation(c, RegionSpec.RuinTopRadius + 3f);
    }

    /// <summary>Profile of the stream's banks along its length (monotonic, flowing north) and valley shaping.</summary>
    void ShapeStreamValley()
    {
        var line = RegionSpec.Stream;
        int n = (int)MathF.Ceiling(line.Length / StreamDs) + 1;
        var raw = new float[n];
        for (int i = 0; i < n; i++)
        {
            float s = MathF.Min(i * StreamDs, line.Length);
            Vector2 p = line.PointAt(s), t = line.TangentAt(s), nrm = new(-t.Y, t.X);
            float h0 = HeightBilinear(p.X, p.Y);
            float h1 = HeightBilinear(p.X + nrm.X * 10, p.Y + nrm.Y * 10);
            float h2 = HeightBilinear(p.X - nrm.X * 10, p.Y - nrm.Y * 10);
            raw[i] = MathF.Min(h0, (h1 + h2) * 0.5f);
        }
        var prof = Gaussian(raw, 30f / StreamDs);
        // water flows from the south (last sample) toward the north edge (sample 0): bank never rises downstream
        for (int pass = 0; pass < 2; pass++)
        {
            for (int i = n - 2; i >= 0; i--) prof[i] = MathF.Min(prof[i], prof[i + 1] - 0.003f * StreamDs);
            if (pass == 0) prof = Gaussian(prof, 5f / StreamDs);
        }
        StreamBank = prof;
        StreamDepth = new float[n]; StreamHalfWidth = new float[n]; StreamWater = new float[n]; StreamWaterHalfWidth = new float[n];
        for (int i = 0; i < n; i++)
        {
            float s = i * StreamDs;
            float wn = 0.5f + 0.5f * Math.Clamp(_nStream.Fbm(s / 70f, 3.3f, 2) * 1.6f, -1f, 1f);
            float dn = 0.5f + 0.5f * Math.Clamp(_nStream.Fbm(s / 90f, 17.1f, 2) * 1.6f, -1f, 1f);
            StreamHalfWidth[i] = Lerp(RegionSpec.StreamWidthMin, RegionSpec.StreamWidthMax, wn) * 0.5f;
            StreamDepth[i] = Lerp(RegionSpec.StreamDepthMin, RegionSpec.StreamDepthMax, dn);
            // bridge crossing: a 7.2 m wide, 1.8 m deep channel so the 12 m bridge span (village.glb "bridge",
            // deck 1.8 m above the bed) sits flush with both banks
            float bw = 1f - Smoothstep(8f, 22f, MathF.Abs(s - RegionSpec.BridgeStreamS));
            StreamHalfWidth[i] = Lerp(StreamHalfWidth[i], RegionSpec.BridgeChannelHalfWidth, bw);
            StreamDepth[i] = Lerp(StreamDepth[i], RegionSpec.BridgeDeckAboveBed, bw);
            float freeboard = 0.55f + 0.3f * (StreamDepth[i] - RegionSpec.StreamDepthMin);
            StreamWater[i] = StreamBank[i] - freeboard;
            // solve the channel cross-section for the water edge
            float hw = StreamHalfWidth[i], e0 = hw * 0.35f, e1 = hw + 1.2f;
            float v = 1f - freeboard / StreamDepth[i];                     // smoothstep value at the water line
            float inv = 0.5f - MathF.Sin(MathF.Asin(1f - 2f * v) / 3f);   // inverse smoothstep
            StreamWaterHalfWidth[i] = e0 + (e1 - e0) * inv;
        }
        // pull the terrain toward bank level near the stream (a shallow valley)
        var sf = StreamField;
        Parallel.For(0, N, iz =>
        {
            for (int ix = 0; ix < N; ix++)
            {
                int k = iz * N + ix;
                float d = sf.D[k];
                if (d >= sf.MaxDist) continue;
                float s = sf.S[k];
                float hw = StreamHalfWidthAt(s);
                float valley = 26f + 10f * _nStream.Get(GX(ix) / 60f, GX(iz) / 60f);
                float w = 1f - Smoothstep(hw + 1.5f, hw + 1.5f + valley, d);
                if (w <= 0) continue;
                float bank = StreamBankAt(s);
                // lowering keeps a little of the natural relief, raising (rare) is limited to avoid dykes
                float target = H[k] > bank ? bank + (H[k] - bank) * 0.12f : Lerp(H[k], bank, 0.6f);
                H[k] = Lerp(H[k], target, w * w * (3 - 2 * w));
            }
        });
    }

    /// <summary>Flatten and smooth a path bed. Returns the smoothed height profile sampled every 2 m.</summary>
    float[] CarvePath(Polyline line, DistanceField field, float halfWidth, float blend, float smoothSigma, float sink, float strength = 1f,
                      (float s, float h, float inner, float outer)? pin = null)
    {
        const float ds = 2f;
        int n = (int)MathF.Ceiling(line.Length / ds) + 1;
        var raw = new float[n];
        for (int i = 0; i < n; i++)
        {
            Vector2 p = line.PointAt(MathF.Min(i * ds, line.Length));
            raw[i] = HeightBilinear(p.X, p.Y);
        }
        var prof = Gaussian(raw, smoothSigma / ds);
        if (pin.HasValue)
        {
            // hold the bed at a given height around arc length s (e.g. bridge abutments), easing in/out
            var (ps, ph, inner, outer) = pin.Value;
            for (int i = 0; i < n; i++)
            {
                float w = 1f - Smoothstep(inner, outer, MathF.Abs(i * ds - ps));
                prof[i] = Lerp(prof[i], ph, w);
            }
        }
        Parallel.For(0, N, iz =>
        {
            for (int ix = 0; ix < N; ix++)
            {
                int k = iz * N + ix;
                float d = field.D[k];
                if (d >= halfWidth + blend) continue;
                float target = ProfileAt(prof, field.S[k], ds) - sink * (1f - Smoothstep(halfWidth * 0.4f, halfWidth + 0.3f, d));
                float w = (1f - Smoothstep(halfWidth + 0.4f, halfWidth + blend, d)) * strength;
                H[k] = Lerp(H[k], target, w);
            }
        });
        return prof;
    }

    /// <summary>Carve the stream channel itself (after roads, so the road dips to the future bridge).</summary>
    void CarveStreamChannel()
    {
        var sf = StreamField;
        Parallel.For(0, N, iz =>
        {
            for (int ix = 0; ix < N; ix++)
            {
                int k = iz * N + ix;
                float d = sf.D[k];
                if (d > 9f) continue;
                float s = sf.S[k];
                float hw = StreamHalfWidthAt(s), depth = StreamDepthAt(s), bank = StreamBankAt(s);
                // irregular banks: wiggle the lateral distance (±0.7 m) away from the bridge
                float bw = Smoothstep(10f, 25f, MathF.Abs(s - RegionSpec.BridgeStreamS));
                d += bw * 0.7f * _nStream.Fbm(GX(ix) / 9f + 3.7f, GX(iz) / 9f, 2);
                if (d > hw + 1.2f) continue;
                float wob = 0.12f * _nStream.Get(GX(ix) / 3.1f, GX(iz) / 3.1f);   // uneven bed
                float cross = bank - depth * (1f - Smoothstep(hw * 0.35f, hw + 1.2f, d)) + wob * (1f - Smoothstep(hw * 0.5f, hw, d));
                if (cross < H[k]) H[k] = cross;
            }
        });
    }

    void ShapePond()
    {
        Vector2 c = RegionSpec.Pond;
        float rim = float.MaxValue;
        for (int i = 0; i < 64; i++)
        {
            float a = i * MathF.Tau / 64f;
            Vector2 dir = new(MathF.Cos(a), MathF.Sin(a));
            Vector2 p = c + dir * (PondRadiusAt(c + dir) + 5f);
            rim = MathF.Min(rim, HeightBilinear(p.X, p.Y));
        }
        // settle the pond into a gentle hollow: pull the surroundings toward the rim level first
        float mean = rim;
        ForSamples(c, RegionSpec.PondRadius * 2.2f, (k, p) =>
        {
            float t = (p - c).Length() / PondRadiusAt(p);
            float w = 1f - Smoothstep(1.0f, 2.0f, t);
            if (w > 0 && H[k] > mean) H[k] = Lerp(H[k], mean + (H[k] - mean) * 0.35f, w);
        });
        PondLevel = rim - 0.3f;
        float level = PondLevel, depth = RegionSpec.PondDepth;
        ForSamples(c, RegionSpec.PondRadius * 1.9f, (k, p) =>
        {
            float t = (p - c).Length() / PondRadiusAt(p);
            if (t < 1.3f)
            {
                float basin = t < 1f
                    ? level - 0.3f - depth * (1f - Smoothstep(0.25f, 1.0f, t))
                    : Lerp(level - 0.3f, level + 0.45f, Smoothstep(1.0f, 1.3f, t));
                basin += 0.1f * _nPond.Get(p.X / 4f, p.Y / 4f);
                if (basin < H[k]) H[k] = basin;
            }
            if (t >= 1.08f && t < 1.7f)
            {
                float floorH = level + 0.3f * (1f - Smoothstep(1.35f, 1.7f, t));
                if (H[k] < floorH) H[k] = Lerp(H[k], floorH, 1f - Smoothstep(1.35f, 1.7f, t));
            }
        });
    }

    static float[] Gaussian(float[] src, float sigmaSamples)
    {
        int n = src.Length, r = Math.Max(1, (int)MathF.Ceiling(sigmaSamples * 2.5f));
        var w = new float[2 * r + 1];
        for (int i = -r; i <= r; i++) w[i + r] = MathF.Exp(-0.5f * i * i / (sigmaSamples * sigmaSamples));
        var dst = new float[n];
        for (int i = 0; i < n; i++)
        {
            float s = 0, ws = 0;
            for (int j = -r; j <= r; j++)
            {
                int q = Math.Clamp(i + j, 0, n - 1);
                s += src[q] * w[j + r]; ws += w[j + r];
            }
            dst[i] = s / ws;
        }
        return dst;
    }

    // =====================================================================================================
    // Masks shared with vegetation and the splat maps
    // =====================================================================================================

    // Caches on the 2 m grid, filled by Bake() (exact functions are used before that).
    float[] _cForest, _cClearTree, _cClearBush, _cClearGround, _cRock, _cMeadow;

    /// <summary>Bilinear sample of a cached 2 m grid.</summary>
    static float Bilerp(float[] g, float x, float z)
    {
        float fx = (x - Min) / Cell, fz = (z - Min) / Cell;
        int ix = Math.Clamp((int)fx, 0, N - 2), iz = Math.Clamp((int)fz, 0, N - 2);
        float tx = Math.Clamp(fx - ix, 0f, 1f), tz = Math.Clamp(fz - iz, 0f, 1f);
        int k = iz * N + ix;
        float a = g[k] + (g[k + 1] - g[k]) * tx;
        float b = g[k + N] + (g[k + N + 1] - g[k + N]) * tx;
        return a + (b - a) * tz;
    }

    /// <summary>Forest canopy density 0..1 from the spec's forest rules (wobbly edges, groves, glades),
    /// already multiplied by <see cref="Clearance"/> for trees.</summary>
    public float ForestDensity(float x, float z) =>
        IsBaked ? Bilerp(_cForest, x, z) * Bilerp(_cClearTree, x, z) : ForestMask(x, z) * ClearanceExact(x, z, ClearKind.Tree);

    /// <summary>
    /// 0 = nothing of this kind may grow here, 1 = unrestricted. Encodes the spec's exclusions (road 8 m,
    /// village 110 m, inn 45 m, camp 30 m, stream bank 6 m, pond, ruin, fields, pasture) plus anything builders
    /// registered with <see cref="AddPad"/>, <see cref="AddPath"/> or <see cref="ExcludeVegetation"/>.
    /// After <see cref="Bake"/> this reads a 2 m cache (bilinear).
    /// </summary>
    public float Clearance(float x, float z, ClearKind kind)
    {
        if (!IsBaked) return ClearanceExact(x, z, kind);
        return Bilerp(kind == ClearKind.Tree ? _cClearTree : kind == ClearKind.Bush ? _cClearBush : _cClearGround, x, z);
    }

    /// <summary>Forest rules without clearances, read from the 2 m cache after Bake (fast).</summary>
    public float ForestCover(float x, float z) => IsBaked ? Bilerp(_cForest, x, z) : ForestMask(x, z);

    /// <summary>Rockiness from the 2 m cache after Bake (fast).</summary>
    public float RockinessAt(float x, float z) => IsBaked ? Bilerp(_cRock, x, z) : Rockiness(x, z);

    /// <summary>Forest rules only (no clearances): where the woods would be if nothing were built.</summary>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveOptimization)]
    public float ForestMask(float x, float z)
    {
        // wobbly edges: a broad bend plus finer tongues of trees/meadow
        float wob = RegionSpec.ForestEdgeWobble * (1.3f * _nForest.Fbm(x / 210f, z / 210f, 2) + 0.55f * _nForest.Get(x / 55f + 9f, z / 55f - 4f));
        float wob2 = RegionSpec.ForestEdgeWobble * (1.3f * _nForest.Fbm(x / 210f + 31f, z / 210f - 17f, 2) + 0.55f * _nForest.Get(x / 55f - 21f, z / 55f + 6f));
        float soft = RegionSpec.ForestEdgeSoftness;
        float main = Saturate((x - RegionSpec.ForestWestX + wob) / soft) * Saturate((RegionSpec.ForestSouthZ - z + wob2) / soft);
        float east = Saturate((x - RegionSpec.ForestEastStripX + wob2) / soft);
        // scattered groves: few, fairly large clumps
        float gn = _nGrove.Fbm(x / 120f, z / 120f, 3) + 0.25f * _nGrove.Get(x / 30f, z / 30f);
        float grove = Smoothstep(0.30f, 0.40f, gn);
        float f = MathF.Max(MathF.Max(main, east), grove * 0.9f);
        // glades inside the woods
        float glade = Smoothstep(0.30f, 0.42f, _nGlade.Fbm(x / 60f, z / 60f, 2));
        return f * (1f - 0.9f * glade);
    }

    /// <summary>Exact (uncached) clearance, see <see cref="Clearance"/>.</summary>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveOptimization)]
    public float ClearanceExact(float x, float z, ClearKind kind)
    {
        Vector2 p = new(x, z);
        float k = kind == ClearKind.Tree ? 1f : kind == ClearKind.Bush ? 0.55f : 0.25f;
        float c = 1f - CustomExclusion(x, z);
        if (c <= 0) return 0;
        float road = RoadField.Dist(x, z);
        c *= Smoothstep(RegionSpec.ClearRoad * k + 1f, RegionSpec.ClearRoad * k + 4f, road);
        if (c <= 0) return 0;
        c *= Smoothstep(RegionSpec.ClearTrail * k + 0.3f, RegionSpec.ClearTrail * k + 2.5f, TrailField.Dist(x, z));
        c *= Smoothstep(1.2f, 2.5f, SpurField.Dist(x, z));
        // m is a safety margin so the bilinear 2 m cache never lets anything inside the spec distances
        const float m = 1.5f;
        if (kind != ClearKind.Ground)
        {
            c *= Smoothstep(RegionSpec.ClearVillage * k + m, RegionSpec.ClearVillage * k + 14f, (p - RegionSpec.VillageCenter).Length());
            c *= Smoothstep(RegionSpec.ClearInn * k + m, RegionSpec.ClearInn * k + 9f, (p - RegionSpec.Inn).Length());
            c *= Smoothstep(RegionSpec.ClearCamp * k + m, RegionSpec.ClearCamp * k + 7f, (p - RegionSpec.GoblinCamp).Length());
            c *= Smoothstep(RegionSpec.ClearRuin * k + m, RegionSpec.ClearRuin * k + 9f, (p - RegionSpec.Ruin).Length());
            c *= Smoothstep(RegionSpec.PastureRadius * k + m, RegionSpec.PastureRadius * k + 12f, (p - RegionSpec.Pasture).Length());
        }
        if (c <= 0) return 0;
        float sd = StreamDistance(x, z, out float s);
        float hw = StreamHalfWidthAt(s);
        float streamClear = kind == ClearKind.Ground ? 0.6f : RegionSpec.ClearStream * k + m;
        c *= Smoothstep(hw + streamClear, hw + streamClear + 3f, sd);
        float pr = (p - RegionSpec.Pond).Length();
        if (pr < RegionSpec.PondRadius * 1.6f + 20f)
        {
            float pc = kind == ClearKind.Ground ? 0.5f : RegionSpec.ClearPond * k + m;
            c *= Smoothstep(PondRadiusAt(p) + pc, PondRadiusAt(p) + pc + 4f, pr);
        }
        float fieldSd = FieldDistance(p, out _);
        float fc = kind == ClearKind.Ground ? -0.5f : RegionSpec.ClearField * k;
        c *= Smoothstep(fc, fc + 3f, fieldSd);
        return c;
    }

    /// <summary>Signed distance to the nearest field plot (negative inside); index of that plot.</summary>
    public float FieldDistance(Vector2 p, out int plot)
    {
        float best = float.MaxValue; plot = -1;
        var fields = RegionSpec.Fields;
        for (int i = 0; i < fields.Length; i++)
        {
            if ((p - fields[i].Center).LengthSquared() > 90f * 90f) continue;
            float d = fields[i].SignedDistance(p);
            if (d < best) { best = d; plot = i; }
        }
        return best;
    }

    /// <summary>How "rocky" the ground is apart from slope (NW hills, ruin hilltop).</summary>
    public float Rockiness(float x, float z)
    {
        Vector2 p = new(x, z);
        float nw = Bump((p - RegionSpec.RockyHills).Length() / (RegionSpec.RockyHillsRadius * 0.95f));
        float r = _nRock.Ridged(x / 115f + 7f, z / 115f - 3f, 4);
        float outcrop = nw * Smoothstep(0.42f, 0.62f, r + 0.25f * _nMisc.Get(x / 14f, z / 14f));
        float ruin = (1f - Smoothstep(12f, 40f, (p - RegionSpec.Ruin).Length())) * Smoothstep(0.1f, 0.5f, _nMisc.Get(x / 9f + 3f, z / 9f));
        return Saturate(MathF.Max(outcrop, ruin * 0.8f));
    }

    // =====================================================================================================
    // Bake: splat maps
    // =====================================================================================================

    /// <summary>Finalise: compute ground-cover splat maps. Call once, after all pads/paths are registered.</summary>
    public void Bake()
    {
        if (IsBaked) return;
        // cavity (Laplacian of a lightly blurred height) at grid resolution
        var cav = new float[N * N];
        Parallel.For(1, N - 1, iz =>
        {
            for (int ix = 1; ix < N - 1; ix++)
            {
                int k = iz * N + ix;
                float lap = (H[k - 1] + H[k + 1] + H[k - N] + H[k + N] - 4f * H[k]) / (Cell * Cell);
                float lap2 = ix > 2 && iz > 2 && ix < N - 3 && iz < N - 3
                    ? (H[k - 3] + H[k + 3] + H[k - 3 * N] + H[k + 3 * N] - 4f * H[k]) / (9f * Cell * Cell) : lap;
                cav[k] = Math.Clamp(0.5f - (lap * 0.6f + lap2 * 1.4f) * 2.2f, 0f, 1f);
            }
        });

        // expensive masks on the 2 m grid (the 1 m splat interpolates them)
        _cForest = new float[N * N]; _cClearTree = new float[N * N]; _cClearBush = new float[N * N];
        _cClearGround = new float[N * N]; _cRock = new float[N * N]; _cMeadow = new float[N * N];
        Parallel.For(0, N, iz =>
        {
            float z = GX(iz);
            for (int ix = 0; ix < N; ix++)
            {
                float x = GX(ix);
                int k = iz * N + ix;
                _cForest[k] = ForestMask(x, z);
                _cClearTree[k] = ClearanceExact(x, z, ClearKind.Tree);
                _cClearBush[k] = ClearanceExact(x, z, ClearKind.Bush);
                _cClearGround[k] = ClearanceExact(x, z, ClearKind.Ground);
                _cRock[k] = Rockiness(x, z);
                _cMeadow[k] = _nMeadow.Fbm(x / 95f, z / 95f, 3);
            }
        });

        int R = SplatRes;
        SplatA = new byte[R * R * 4]; SplatB = new byte[R * R * 4]; SplatC = new byte[R * R * 4];
        var fields = RegionSpec.Fields;
        Parallel.For(0, R, j =>
        {
            float z = Min + j + 0.5f;
            for (int i = 0; i < R; i++)
            {
                float x = Min + i + 0.5f;
                Vector2 p = new(x, z);
                int o = (j * R + i) * 4;

                // --- dirt: road, trail, spur, builder strokes and pad wear
                float dirt = 1f - Smoothstep(RegionSpec.RoadWidth * 0.5f - 0.6f, RegionSpec.RoadWidth * 0.5f + 0.9f, RoadField.Dist(x, z));
                dirt = MathF.Max(dirt, 0.92f * (1f - Smoothstep(RegionSpec.TrailWidth * 0.5f - 0.45f, RegionSpec.TrailWidth * 0.5f + 0.6f, TrailField.Dist(x, z))));
                dirt = MathF.Max(dirt, 0.8f * (1f - Smoothstep(RegionSpec.CampSpurWidth * 0.5f - 0.3f, RegionSpec.CampSpurWidth * 0.5f + 0.6f, SpurField.Dist(x, z))));
                float worn = 0;
                foreach (var st in _strokes)
                {
                    if (!st.line.Bounds.Grow(st.halfWidth + 2f).HasPoint(p)) continue;
                    float d = st.line.Distance(p);
                    dirt = MathF.Max(dirt, st.wear * (1f - Smoothstep(st.halfWidth - 0.5f, st.halfWidth + 0.7f, d)));
                    worn = MathF.Max(worn, st.wear * (1f - Smoothstep(st.halfWidth, st.halfWidth + 3f, d)));
                }
                foreach (var pw in _padWear)
                {
                    float sd;
                    if (float.IsNaN(pw.angleDeg)) sd = (p - pw.c).Length() - pw.half.X;
                    else
                    {
                        float a = Mathf.DegToRad(pw.angleDeg);
                        Vector2 u = new(MathF.Cos(a), MathF.Sin(a)), v = new(-u.Y, u.X);
                        Vector2 d = p - pw.c;
                        sd = MathF.Max(MathF.Abs(d.Dot(u)) - pw.half.X, MathF.Abs(d.Dot(v)) - pw.half.Y);
                    }
                    if (sd > pw.margin + 3f) continue;
                    dirt = MathF.Max(dirt, pw.wear * (1f - Smoothstep(pw.margin - 0.5f, pw.margin + 0.8f, sd)));
                    worn = MathF.Max(worn, pw.wear * (1f - Smoothstep(pw.margin, pw.margin + 4f, sd)));
                }
                // trampled goblin camp and ruin top
                float camp = (p - RegionSpec.GoblinCamp).Length();
                dirt = MathF.Max(dirt, 0.75f * (1f - Smoothstep(13f, 21f, camp + 5f * _nMisc.Get(x / 7f, z / 7f))));
                worn = MathF.Max(worn, 1f - Smoothstep(18f, 30f, camp));

                // --- fields
                float fsd = FieldDistance(p, out int plot);
                float field = plot >= 0 ? 1f - Smoothstep(-0.8f, 0.5f, fsd) : 0f;
                float furrow = 0, crop = 0;
                if (plot >= 0 && fsd < 2f)
                {
                    float ang = fields[plot].AngleDeg;
                    if (ang < 0) ang += 180f;
                    furrow = ang / 180f;
                    crop = (int)fields[plot].Crop / 4f;
                }

                // --- stream and pond banks
                float sdist = StreamDistance(x, z, out float s);
                float whw = StreamWaterHalfWidthAt(s);
                // gravel only in patches (bars on bends); elsewhere grass reaches the water's edge
                float bars = Smoothstep(-0.15f, 0.35f, _nMisc.Fbm(x / 14f + 9f, z / 14f - 4f, 2));
                float sand = (1f - Smoothstep(whw - 0.2f, whw + 0.3f + (0.4f + 1.4f * bars) * (0.6f + 0.4f * _nMisc.Get(x / 5f, z / 5f)), sdist))
                             * (0.35f + 0.65f * bars);
                float wet = 1f - Smoothstep(whw - 0.6f, whw + 0.9f, sdist);
                float pr = (p - RegionSpec.Pond).Length();
                if (pr < RegionSpec.PondRadius * 1.6f)
                {
                    float prr = PondRadiusAt(p);
                    sand = MathF.Max(sand, 1f - Smoothstep(prr + 0.5f, prr + 3.5f + _nMisc.Get(x / 5f, z / 5f), pr));
                    wet = MathF.Max(wet, 1f - Smoothstep(prr - 1f, prr + 1f, pr));
                }

                // --- rock, forest floor, meadow
                float rock = Bilerp(_cRock, x, z);
                float forest = Bilerp(_cForest, x, z) * Bilerp(_cClearBush, x, z);
                float pasture = 1f - Smoothstep(RegionSpec.PastureRadius - 8f, RegionSpec.PastureRadius + 14f, (p - RegionSpec.Pasture).Length());
                float patches = Smoothstep(0.24f, 0.40f, Bilerp(_cMeadow, x, z)) * (1f - forest);
                float pondMeadow = 1f - Smoothstep(RegionSpec.PondRadius + 5f, RegionSpec.PondRadius + 30f, pr);
                float meadow = MathF.Max(MathF.Max(pasture, patches), pondMeadow * 0.8f);

                // --- cavity (bilinear from the grid)
                float gx = (x - Min) / Cell, gz = (z - Min) / Cell;
                int ix = Math.Min((int)gx, N - 2), iz = Math.Min((int)gz, N - 2);
                float tx = gx - ix, tz = gz - iz;
                int kk = iz * N + ix;
                float cv = Lerp(Lerp(cav[kk], cav[kk + 1], tx), Lerp(cav[kk + N], cav[kk + N + 1], tx), tz);

                SplatA[o] = B(dirt); SplatA[o + 1] = B(field); SplatA[o + 2] = B(rock); SplatA[o + 3] = B(sand);
                SplatB[o] = B(forest); SplatB[o + 1] = B(meadow); SplatB[o + 2] = B(furrow); SplatB[o + 3] = B(cv);
                SplatC[o] = B(crop); SplatC[o + 1] = B(worn); SplatC[o + 2] = B(wet); SplatC[o + 3] = 0;
            }
        });
        IsBaked = true;
    }

    static byte B(float v) => (byte)Math.Clamp((int)(v * 255f + 0.5f), 0, 255);

    /// <summary>Raw splat channels at (x, z) (bilinear): A = dirt, field, rock, sand; B = forest, meadow, furrow, cavity.</summary>
    public (Vector4 a, Vector4 b, Vector4 c) SplatAt(float x, float z)
    {
        if (!IsBaked) return default;
        float fx = x - Min - 0.5f, fz = z - Min - 0.5f;
        int i = Math.Clamp((int)MathF.Floor(fx), 0, SplatRes - 2), j = Math.Clamp((int)MathF.Floor(fz), 0, SplatRes - 2);
        float tx = Math.Clamp(fx - i, 0, 1), tz = Math.Clamp(fz - j, 0, 1);
        return (Sample(SplatA, i, j, tx, tz), Sample(SplatB, i, j, tx, tz), Sample(SplatC, i, j, tx, tz));
    }

    static Vector4 Sample(byte[] m, int i, int j, float tx, float tz)
    {
        int R = SplatRes;
        Vector4 Get(int ii, int jj) { int o = (jj * R + ii) * 4; return new Vector4(m[o], m[o + 1], m[o + 2], m[o + 3]) / 255f; }
        return Get(i, j).Lerp(Get(i + 1, j), tx).Lerp(Get(i, j + 1).Lerp(Get(i + 1, j + 1), tx), tz);
    }

    /// <summary>Effective ground cover at (x, z) — the same layering as terrain.gdshader (without its fine noise).</summary>
    public BiomeWeights Biome(float x, float z)
    {
        var (a, b, _) = SplatAt(x, z);
        float slope = 1f - Normal(x, z).Y;
        float rem = 1f;
        var w = new BiomeWeights();
        // top-most layers first (painter's order reversed)
        float rock = Saturate(MathF.Max(Smoothstep(0.35f, 0.65f, a.Z), Smoothstep(0.30f, 0.42f, slope)));
        w.Rock = rock * rem; rem -= w.Rock;
        float dirt = Smoothstep(0.35f, 0.65f, a.X);
        w.Dirt = dirt * rem; rem -= w.Dirt;
        float sand = Smoothstep(0.3f, 0.7f, a.W);
        w.Sand = sand * rem; rem -= w.Sand;
        float field = Smoothstep(0.35f, 0.65f, a.Y);
        w.Field = field * rem; rem -= w.Field;
        float forest = Smoothstep(0.2f, 0.7f, b.X);
        w.Forest = forest * rem; rem -= w.Forest;
        w.Meadow = Smoothstep(0.2f, 0.8f, b.Y) * rem; rem -= w.Meadow;
        w.Grass = MathF.Max(rem, 0f);
        return w;
    }

    // =====================================================================================================
    // Debug
    // =====================================================================================================

    /// <summary>Write a shaded overview map (heights + ground cover + features) for eyeballing the layout.</summary>
    public void SaveDebugMap(string path, int size = 1200)
    {
        var img = Image.CreateEmpty(size, size, false, Image.Format.Rgb8);
        float scale = RegionSpec.Size / size;
        Vector3 L = new Vector3(-1, 1.6f, -1).Normalized();
        for (int j = 0; j < size; j++)
            for (int i = 0; i < size; i++)
            {
                float x = Min + (i + 0.5f) * scale, z = Min + (j + 0.5f) * scale;
                var bw = Biome(x, z);
                Color c = new Color(0.45f, 0.62f, 0.3f) * bw.Grass + new Color(0.62f, 0.7f, 0.35f) * bw.Meadow
                        + new Color(0.22f, 0.35f, 0.18f) * bw.Forest + new Color(0.62f, 0.5f, 0.34f) * bw.Dirt
                        + new Color(0.55f, 0.53f, 0.5f) * bw.Rock + new Color(0.75f, 0.68f, 0.5f) * bw.Sand
                        + new Color(0.5f, 0.36f, 0.24f) * bw.Field;
                float shade = Math.Clamp(Normal(x, z).Dot(L) * 1.1f, 0.35f, 1.2f);
                c = new Color(c.R * shade, c.G * shade, c.B * shade);
                if (IsWater(x, z)) c = new Color(0.25f, 0.45f, 0.65f);
                float h = Height(x, z);
                if (MathF.Abs(h / 5f - MathF.Round(h / 5f)) < 0.04f) c = c.Darkened(0.25f);   // 5 m contours
                img.SetPixel(i, j, c);
            }
        img.SavePng(path);
    }
}
