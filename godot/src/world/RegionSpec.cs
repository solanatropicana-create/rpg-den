using System;
using Godot;
using FD.Core;

namespace FD.World;

/// <summary>Crop grown on a field plot (drives the terrain shader's field tint).</summary>
public enum CropKind { Fallow = 0, Wheat = 1, Barley = 2, Vegetables = 3, Flax = 4 }

/// <summary>
/// A rectangular field plot. <see cref="AngleDeg"/> is the direction of the plot's long axis (U) measured in the
/// ground plane from +x toward +z (i.e. atan2(z, x)); furrows run along U. HalfSize = (half length along U,
/// half width along V), V = U rotated +90° (toward +z when U = +x).
/// </summary>
public readonly record struct FieldPlot(Vector2 Center, Vector2 HalfSize, float AngleDeg, CropKind Crop)
{
    public Vector2 U => new(MathF.Cos(Mathf.DegToRad(AngleDeg)), MathF.Sin(Mathf.DegToRad(AngleDeg)));
    public Vector2 V => new(-U.Y, U.X);

    /// <summary>Signed distance to the rectangle (negative inside).</summary>
    public float SignedDistance(Vector2 p)
    {
        Vector2 d = p - Center;
        float lu = MathF.Abs(d.Dot(U)) - HalfSize.X, lv = MathF.Abs(d.Dot(V)) - HalfSize.Y;
        float outside = new Vector2(MathF.Max(lu, 0), MathF.Max(lv, 0)).Length();
        return outside + MathF.Min(MathF.Max(lu, lv), 0);
    }
}

/// <summary>A smooth hill bump added to the base terrain.</summary>
public readonly record struct HillSpec(Vector2 Center, float Radius, float Height, float Roughness);

/// <summary>
/// Authoritative layout of the vertical-slice region ("Bölge"), in metres at 1:1 scale.
/// <para>Axes (Godot): x = east, z = south, y = up. The region spans [-600, 600] on x and z.
/// All 2D points are <c>Vector2(x, z)</c>. Source: docs/slice-spec-shared.md, section "Bölge yerleşimi".</para>
/// <para>Every system (terrain, water, vegetation, village/inn/camp builders, NPC AI) reads positions from here —
/// never hard-code layout numbers elsewhere. Smoothed paths (<see cref="Road"/>, <see cref="Trail"/>,
/// <see cref="Stream"/>) are the ones actually carved into the terrain; NPCs should walk those.</para>
/// <para>Facing convention: a model's FRONT is its local +Z (Blender −Y). To make a model face ground direction
/// d = (dx, dz) use <c>RotationY = <see cref="YawFacing"/>(d)</c>.</para>
/// </summary>
public static class RegionSpec
{
    // ------------------------------------------------------------------ extent
    public const float HalfSize = 600f;
    public const float Size = 2f * HalfSize;
    /// <summary>World seed for terrain/vegetation noise. Changing it reshapes hills and forest edges but never
    /// moves any spec'd feature.</summary>
    public const int Seed = 20260930;
    /// <summary>Hills rise toward the map border (starting this far from the centre) so the edge reads as a
    /// natural horizon; invisible walls stop the player at <see cref="WalkLimit"/>.</summary>
    public const float EdgeHillsStart = 470f, EdgeHillsHeight = 16f, WalkLimit = 596f;

    // ------------------------------------------------------------------ village
    public static readonly Vector2 VillageCenter = new(-180, 120);
    /// <summary>Houses, yards and the square live inside this radius.</summary>
    public const float VillageRadius = 85f;
    /// <summary>Terrain is gently flattened (±<see cref="VillageMaxUndulation"/> m) inside this radius…</summary>
    public const float VillagePadRadius = 90f;
    /// <summary>…and blends back to natural terrain over this distance beyond it.</summary>
    public const float VillagePadBlend = 45f;
    public const float VillageMaxUndulation = 2f;

    // ------------------------------------------------------------------ inn
    public static readonly Vector2 Inn = new(300, 20);
    /// <summary>Inn plot half extents: X along the road (40 m total), Y = depth (30 m total).</summary>
    public static readonly Vector2 InnPadHalfSize = new(20, 15);
    /// <summary>Unit ground direction the inn's front faces (toward the road, roughly north). Computed from the road.</summary>
    public static readonly Vector2 InnFront;
    /// <summary>Angle (deg, atan2(z,x)) of the inn plot's long axis (parallel to the road).</summary>
    public static readonly float InnAngleDeg;

    // ------------------------------------------------------------------ roads & trails
    /// <summary>Main road control points (west → east). 4 m wide. Crosses the stream at x≈−42 (bridge).</summary>
    public static readonly Vector2[] RoadControl =
    {
        new(-330, 150), new(-180, 120), new(-60, 95), new(80, 60), new(220, 30), new(300, -2), new(420, -40), new(590, -120),
    };
    public const float RoadWidth = 4f;
    /// <summary>Forest trail (branches off the road at (80, 60), heads north to the goblin woods). 2.5 m wide.</summary>
    public static readonly Vector2[] TrailControl =
    {
        new(80, 60), new(100, -60), new(170, -200), new(230, -300),
    };
    public const float TrailWidth = 2.5f;
    /// <summary>Faint goblin footpath from the trail end into the camp clearing (not in the spec's polyline; 1.6 m).</summary>
    public static readonly Vector2[] CampSpurControl = { new(230, -300), new(244, -316), new(254, -326) };
    public const float CampSpurWidth = 1.6f;

    // ------------------------------------------------------------------ stream
    /// <summary>Stream control points, north → south. Water flows from the southern hills (last point) toward the
    /// north edge (first point).</summary>
    public static readonly Vector2[] StreamControl =
    {
        new(-20, -600), new(-50, -300), new(-30, -100), new(-45, 95), new(-90, 300), new(-140, 600),
    };
    public const float StreamWidthMin = 6f, StreamWidthMax = 10f;
    /// <summary>Stream bed depth below the banks.</summary>
    public const float StreamDepthMin = 1.5f, StreamDepthMax = 2.5f;

    // ------------------------------------------------------------------ places
    public static readonly Vector2 GoblinCamp = new(260, -340);
    public const float CampClearingRadius = 30f;

    /// <summary>Ruined watchtower on top of the highest south-western hill.</summary>
    public static readonly Vector2 Ruin = new(-430, 380);
    public const float RuinHillHeight = 40f;
    /// <summary>Flat top of the ruin hill: 12 m across.</summary>
    public const float RuinTopRadius = 6f;

    /// <summary>High south-western hills (the ruin hill is the first/highest).</summary>
    public static readonly HillSpec[] SouthWestHills =
    {
        new(Ruin, 190f, RuinHillHeight, 0.35f),
        new(new Vector2(-545, 250), 120f, 24f, 0.5f),
        new(new Vector2(-330, 520), 115f, 20f, 0.5f),
        new(new Vector2(-560, 500), 130f, 24f, 0.45f),
        new(new Vector2(-330, 420), 90f, 12f, 0.5f),
    };

    /// <summary>Rocky north-western hills (ridged, with exposed rock).</summary>
    public static readonly Vector2 RockyHills = new(-440, -420);
    public const float RockyHillsRadius = 230f, RockyHillsHeight = 28f;

    /// <summary>Optional pond (organic outline, ±5 m around the radius).</summary>
    public static readonly Vector2 Pond = new(60, 330);
    public const float PondRadius = 35f, PondDepth = 2.4f;

    /// <summary>Pasture (mera) between the village and the stream: open meadow, no trees.</summary>
    public static readonly Vector2 Pasture = new(-120, 180);
    public const float PastureRadius = 42f;

    /// <summary>Field plots west and south of the village (tilled soil + crops). Gaps between plots are
    /// grass balks/lanes (4–6 m) where a village builder can run fences and paths.</summary>
    public static readonly FieldPlot[] Fields =
    {
        // west, north of the road (long axis parallel to the road, ≈ −11°)
        new(new Vector2(-300, 117), new Vector2(24, 15), -11.3f, CropKind.Wheat),
        new(new Vector2(-356, 128), new Vector2(28, 15), -11.3f, CropKind.Barley),
        new(new Vector2(-330, 88),  new Vector2(34, 11), -11.3f, CropKind.Vegetables),
        new(new Vector2(-400, 100), new Vector2(20, 26), -11.3f, CropKind.Fallow),
        // west, south of the road
        new(new Vector2(-298, 178), new Vector2(24, 16), -11.3f, CropKind.Vegetables),
        new(new Vector2(-355, 190), new Vector2(29, 17), -11.3f, CropKind.Fallow),
        new(new Vector2(-415, 172), new Vector2(26, 20), -11.3f, CropKind.Flax),
        // south of the village
        new(new Vector2(-222, 248), new Vector2(30, 15), -4f, CropKind.Wheat),
        new(new Vector2(-156, 252), new Vector2(26, 15), -4f, CropKind.Vegetables),
        new(new Vector2(-230, 290), new Vector2(32, 18), -4f, CropKind.Barley),
        new(new Vector2(-160, 294), new Vector2(27, 18), -4f, CropKind.Fallow),
    };

    // ------------------------------------------------------------------ forest (mask rules; evaluated in Heightfield.ForestDensity)
    /// <summary>Main forest: x &gt; ForestWestX and z &lt; ForestSouthZ, plus the eastern strip x &gt; ForestEastStripX,
    /// plus scattered groves. Edges wobble by ±ForestEdgeWobble m.</summary>
    public const float ForestWestX = -250f, ForestSouthZ = -120f, ForestEastStripX = 380f;
    public const float ForestEdgeWobble = 55f, ForestEdgeSoftness = 26f;

    // ------------------------------------------------------------------ vegetation exclusions (tree-free distances)
    public const float ClearRoad = 8f;        // from the road centreline
    public const float ClearTrail = 3.2f;     // from the trail centreline (keeps the trail enclosed by trees)
    public const float ClearVillage = 110f;   // from VillageCenter
    public const float ClearInn = 45f;        // from Inn
    public const float ClearCamp = 30f;       // from GoblinCamp
    public const float ClearStream = 6f;      // from the stream bank (half width + this)
    public const float ClearPond = 8f;        // from the pond shore
    public const float ClearRuin = 48f;       // from Ruin (open hilltop: the watchtower overlooks the region)
    public const float ClearField = 4f;       // from field plot edges

    // ------------------------------------------------------------------ player
    /// <summary>Player spawn: on the road just east of the village, looking west up the village street.</summary>
    public static readonly Vector2 PlayerStart = new(-110, 105);
    public static readonly Vector2 PlayerStartFacing = new Vector2(-1f, 0.18f).Normalized();

    // ------------------------------------------------------------------ smoothed paths (what is actually carved)
    public static readonly Polyline Road = Polyline.CatmullRom(RoadControl, 2f);
    public static readonly Polyline Trail = Polyline.CatmullRom(TrailControl, 2f);
    public static readonly Polyline CampSpur = Polyline.CatmullRom(CampSpurControl, 1.5f);
    /// <summary>The stream as carved: the spec polyline, smoothed, with natural meanders (±~8 m, damped near the
    /// road crossing so the bridge stays at x≈−42).</summary>
    public static readonly Polyline Stream = Meander(Polyline.CatmullRom(StreamControl, 2f), 8f, 110f, Seed + 77);

    /// <summary>At the bridge the channel is narrowed to this half width and exactly this deep, so the village.glb
    /// "bridge" (12 m span, deck top 1.8 m above its origin at the bed centre) is flush with both banks.</summary>
    public const float BridgeChannelHalfWidth = 3.6f, BridgeDeckAboveBed = 1.8f;

    /// <summary>Where the road crosses the stream (bridge). Arc lengths along both paths are provided too.</summary>
    public static readonly Vector2 BridgePoint;
    public static readonly float BridgeRoadS, BridgeStreamS;

    static RegionSpec()
    {
        // Inn orientation: long axis parallel to the road tangent at the closest road point, front toward the road.
        Road.Distance(Inn, out float s, out _);
        Vector2 t = Road.TangentAt(s);
        Vector2 toRoad = (Road.PointAt(s) - Inn).Normalized();
        Vector2 n = new(-t.Y, t.X);
        InnFront = n.Dot(toRoad) >= 0 ? n : -n;
        InnAngleDeg = Mathf.RadToDeg(MathF.Atan2(t.Y, t.X));

        var hit = Road.Intersect(Stream, out BridgeRoadS, out BridgeStreamS);
        BridgePoint = hit ?? new Vector2(-42, 91);
    }

    static Polyline Meander(Polyline p, float amp, float wavelength, int seed)
    {
        var n = new Noise2(seed);
        var cross = p.Intersect(Road, out float sCross, out _);
        var pts = new System.Collections.Generic.List<Vector2>();
        for (float s = 0; ; s += 2f)
        {
            float ss = MathF.Min(s, p.Length);
            Vector2 q = p.PointAt(ss), t = p.TangentAt(ss), nrm = new(-t.Y, t.X);
            float damp = cross.HasValue ? FMath.Smoothstep(15f, 90f, MathF.Abs(ss - sCross)) : 1f;
            float off = amp * damp * (1.4f * n.Fbm(ss / wavelength, 0.37f, 2) + 0.35f * n.Get(ss / (wavelength * 0.3f), 5.1f));
            pts.Add(q + nrm * off);
            if (ss >= p.Length) break;
        }
        return Polyline.CatmullRom(Decimate(pts, 3), 2f);
    }

    static Vector2[] Decimate(System.Collections.Generic.List<Vector2> pts, int k)
    {
        var r = new System.Collections.Generic.List<Vector2>();
        for (int i = 0; i < pts.Count; i += k) r.Add(pts[i]);
        if ((pts.Count - 1) % k != 0) r.Add(pts[^1]);
        return r.ToArray();
    }

    /// <summary>Rotation about +Y (radians) that turns a model's front (+Z) to face ground direction d = (x, z).</summary>
    public static float YawFacing(Vector2 d) => MathF.Atan2(d.X, d.Y);

    /// <summary>True if (x, z) lies inside the walkable region.</summary>
    public static bool InRegion(float x, float z) => MathF.Abs(x) <= HalfSize && MathF.Abs(z) <= HalfSize;
}
