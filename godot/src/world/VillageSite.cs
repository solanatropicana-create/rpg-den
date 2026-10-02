using System;
using System.Collections.Generic;
using Godot;
using FD.Core;
using FD.Sim.Life;
using Rng = FD.Sim.Life.Rng;
using V2 = System.Numerics.Vector2;

namespace FD.World;

/// <summary>
/// Sessiztepe: a road village at 1:1 scale. Ten households (a headman's hall, the smith's, the shepherd's, the
/// woodcutter's and six farming families) along the main road and two lanes, a small plaza with the well and a
/// market stall, a chapel, the smithy, a sheep pen by the pasture, gardens behind the houses.
/// <para>Shape(): flattens building pads, carves lanes/door paths and registers every place, spot and path in
/// the life simulation (<see cref="Region.Life"/>). Build(): places the Blender models, props, lights and
/// collision.</para>
/// Layout is written in a local frame: u along the road (east-ish), v across it (south-ish), origin at
/// <see cref="RegionSpec.VillageCenter"/> on the road.
/// </summary>
public sealed class VillageSite : IRegionFeature
{
    // ------------------------------------------------------------------ frame
    Vector2 _c, _u, _v;
    public Vector2 W(float u, float v) => _c + _u * u + _v * v;
    Vector2 Dir(float du, float dv) => (_u * du + _v * dv).Normalized();
    static V2 N(Vector2 p) => new(p.X, p.Y);

    public static readonly Color RoofTint = FMath.Hex(0xa8452f);   // terracotta — human villages
    static readonly Color[] RoofVariety = { FMath.Hex(0xa8452f), FMath.Hex(0x9a3f2c), FMath.Hex(0xb4553a), FMath.Hex(0x8c4a36), FMath.Hex(0xa65a3c) };

    enum BKind { SmallA, SmallB, Large, Smithy, Chapel }

    sealed class Bld
    {
        public BKind Kind;
        public Vector2 Pos, Face;     // ground position and the direction its front faces
        public float Y;               // pad height
        public int HomeIndex = -1;    // census order (0 headman, 1 smith, 2 shepherd, 3 woodcutter, 4+ farmers)
        public Place Place;
        public float Yaw => RegionSpec.YawFacing(Face);
        public Vector2 LocalToWorld(float x, float z)
        {
            // model local (x, z) → world; +X = (face.z, -face.x), +Z = face
            Vector2 xAxis = new(Face.Y, -Face.X);
            return Pos + xAxis * x + Face * z;
        }
    }

    readonly List<Bld> _blds = new();
    Heightfield _hf;
    LifeSim _life;
    Vector2 _wellPos, _penCenter, _penGate;
    float _penYaw;

    public Place Plaza, Well, Market, Chapel, Smithy, Pen, Pasture;

    // ------------------------------------------------------------------ layout table
    static readonly (BKind kind, float u, float v, float du, float dv, int home)[] Layout =
    {
        (BKind.Large,  24f, -15f,  0, +1, 0),   // headman's hall, north of the road, faces the road
        (BKind.SmallA, 41f,  12f,  0, -1, 1),   // smith's family, next to the smithy
        (BKind.SmallB, 12f,  32f, -1,  0, 2),   // shepherd, south lane (east side)
        (BKind.SmallB,-32f, -30f, +1,  0, 3),   // woodcutter, north lane (west side)
        (BKind.SmallA, -8f, -40f, -1,  0, 4),   // farmers …
        (BKind.SmallA,-30f,-10.5f, 0, +1, 5),
        (BKind.SmallB,-47f, -11f,  0, +1, 6),
        (BKind.SmallB,-26f,  11f,  0, -1, 7),
        (BKind.SmallA,-44f,  12f,  0, -1, 8),
        (BKind.SmallA,-12f,  40f, +1,  0, 9),
        (BKind.Smithy, 27f, 12.5f, 0, -1, -1),
        (BKind.Chapel,  0f, -21f,  0, +1, -1),
    };

    static (float w, float d) Footprint(BKind k) => k switch
    {
        BKind.Large => (8.5f, 10.9f),
        BKind.Smithy => (6.9f, 7.8f),
        BKind.Chapel => (8.2f, 12.75f),
        _ => (5.3f, 6.5f),
    };

    // ------------------------------------------------------------------ shape

    public void Shape(Heightfield hf)
    {
        _hf = hf;
        _life = Region.Current.Life;
        _c = RegionSpec.VillageCenter;
        RegionSpec.Road.Distance(_c, out float s, out _);
        _c = RegionSpec.Road.PointAt(s);
        _u = RegionSpec.Road.TangentAt(s).Normalized();
        _v = new Vector2(-_u.Y, _u.X);
        if (_v.Y < 0) _v = -_v;   // v points south-ish (toward the pasture and the south fields)

        // lanes (local u,v)
        var north = Path(new[] { (-18f, -1f), (-20f, -22f), (-21f, -47f) });
        var south = Path(new[] { (0f, 0f), (0f, 20f), (1f, 34f), (0f, 48f), (1.5f, 72f), (0f, 108f) });
        hf.AddPath(north, 3f, 0.6f, 0.9f);
        hf.AddPath(Path(new[] { (0f, 20f), (1f, 34f), (0f, 48f) }), 3f, 0.6f, 0.9f);
        hf.AddPath(Path(new[] { (0f, 48f), (1.5f, 72f), (0f, 108f) }), 2.4f, 0.4f, 0.75f);
        _life.Graph.AddPolyline(ToV2(north), 5f, 1.5f);
        _life.Graph.AddPolyline(ToV2(south), 5f, 1.5f);

        // plaza: trodden ground around the well
        Vector2 plazaC = W(0, 9);
        hf.AddPadCircle(plazaC, 13f, float.NaN, 5f, 0.85f, 4f);
        _wellPos = W(0, 9);
        Plaza = _life.AddPlace(PlaceKind.Plaza, $"{_life.VillageName} meydanı", N(plazaC), N(-_v));
        Plaza.Center = N(plazaC); Plaza.Radius = 13f; Plaza.Area = 1;
        Well = _life.AddPlace(PlaceKind.Well, "köy kuyusu", N(_wellPos), N(-_v));
        Well.Area = 1;
        Market = _life.AddPlace(PlaceKind.Market, "pazar tezgâhı", N(W(-9, 12)), N(_u));
        Market.Area = 1;

        // buildings
        foreach (var (kind, u, v, du, dv, home) in Layout)
        {
            var b = new Bld { Kind = kind, Pos = W(u, v), Face = Dir(du, dv), HomeIndex = home };
            var (w, d) = Footprint(kind);
            float angle = Mathf.RadToDeg(MathF.Atan2(-b.Face.X, b.Face.Y));
            b.Y = hf.AddPad(b.Pos, new Vector2(w / 2 + 1.4f, d / 2 + 1.6f), angle, float.NaN, blend: 4.5f, wear: 0.3f, vegetationMargin: 4f);
            _blds.Add(b);
            // the solid part of the building (walls; porches/overhangs excluded) for route tests
            var (sw, sd) = kind switch { BKind.Large => (7.0f, 9.0f), BKind.Smithy => (6.0f, 2.6f), BKind.Chapel => (7.0f, 11.0f), _ => (5.0f, 6.25f) };
            // the smithy is an open workshop: only its closed back room is solid
            Vector2 oc = kind == BKind.Smithy ? b.LocalToWorld(0, -2.2f) : b.Pos;
            _life.Obstacles.Add((N(oc), new V2(sw / 2, sd / 2), Mathf.DegToRad(angle), kind.ToString() + (home >= 0 ? home.ToString() : "")));
            // door path to the nearest lane/road point
            Vector2 door = DoorPoint(b);
            Vector2 target = NearestWay(door, north, south);
            if (door.DistanceTo(target) > 1.2f)
            {
                var spur = new[] { door, door.Lerp(target, 0.5f) + b.Face * 0.6f, target };
                hf.AddPath(spur, kind == BKind.Chapel ? 2.4f : 1.4f, 0.3f, 0.8f);
                _life.Graph.AddPolyline(ToV2(spur), 3f, 0.7f, snap: 1.5f, attach: 10f);
            }
            // a trodden way round the house to the back yard and garden (so nobody walks through walls)
            if (kind is BKind.SmallA or BKind.SmallB or BKind.Large)
            {
                bool big = kind == BKind.Large;
                var round = big
                    ? new[] { b.LocalToWorld(0, 7.0f), b.LocalToWorld(-5.5f, 6.2f), b.LocalToWorld(-5.5f, -6.4f), b.LocalToWorld(0, -7.0f) }
                    : new[] { DoorPoint(b), b.LocalToWorld(-4.0f, 3.6f), b.LocalToWorld(-4.0f, -4.6f), b.LocalToWorld(0, -4.9f) };
                hf.AddPath(round, 0.9f, 0.1f, 0.45f);
                _life.Graph.AddPolyline(ToV2(round), 2.5f, 0.4f, snap: 1.2f, attach: 3f);
            }
            // gardens behind small houses (tilled strips)
            if (kind is BKind.SmallA or BKind.SmallB)
            {
                for (int r = 0; r < 3; r++)
                {
                    float x = -1.6f + r * 1.6f;
                    hf.AddPath(new[] { b.LocalToWorld(x, -5.6f), b.LocalToWorld(x, -10.6f) }, 1.0f, 0.1f, 1f);
                }
            }
        }

        // sheep pen (between the south lane and the pasture) and the pasture itself
        _penCenter = W(24, 24);
        _penYaw = RegionSpec.YawFacing(_v);
        hf.AddPad(_penCenter, new Vector2(8f, 6f), Mathf.RadToDeg(MathF.Atan2(_v.Y, _v.X)), float.NaN, 4f, 0.55f, 3f);
        _penGate = W(24, 29.6f);
        Pen = _life.AddPlace(PlaceKind.Pen, "ağıl", N(_penGate + _v * 1.2f), N(-_v));
        Pen.Center = N(_penCenter); Pen.IsRect = true; Pen.AngleRad = MathF.Atan2(_u.Y, _u.X); Pen.Half = new V2(6.2f, 4.2f); Pen.Area = 2;
        var penPath = new[] { W(1.2f, 36f), W(12f, 37f), _penGate + _v * 1.6f };
        hf.AddPath(penPath, 1.3f, 0.2f, 0.7f);
        _life.Graph.AddPolyline(ToV2(penPath), 3f, 0.6f);

        Pasture = _life.AddPlace(PlaceKind.Pasture, "mera", N(RegionSpec.Pasture), N(-_v));
        Pasture.Center = N(RegionSpec.Pasture); Pasture.Radius = RegionSpec.PastureRadius - 3f; Pasture.Area = 3;
        Vector2 pastureEdge = RegionSpec.Pasture + (_penGate - RegionSpec.Pasture).Normalized() * (RegionSpec.PastureRadius - 4f);
        Pasture.Door = N(pastureEdge);

        // chapel, smithy places + spots are created in Build once positions of model points are known
        RegisterSpots();
    }

    List<Vector2> Path((float u, float v)[] pts)
    {
        var r = new List<Vector2>();
        foreach (var (u, v) in pts) r.Add(W(u, v));
        return r;
    }

    static List<V2> ToV2(IEnumerable<Vector2> pts)
    {
        var r = new List<V2>();
        foreach (var p in pts) r.Add(N(p));
        return r;
    }

    Vector2 DoorPoint(Bld b) => b.Kind switch
    {
        BKind.Large => b.LocalToWorld(0, 5.7f + 1.3f),
        BKind.Chapel => b.LocalToWorld(0, 6.8f + 1.0f),
        BKind.Smithy => b.LocalToWorld(0.35f, 3.9f + 1.2f),
        _ => b.LocalToWorld(0, 3.125f + 1.3f),
    };

    Vector2 NearestWay(Vector2 p, List<Vector2> north, List<Vector2> south)
    {
        float best = RegionSpec.Road.Distance(p, out float s, out _);
        Vector2 bp = RegionSpec.Road.PointAt(s);
        foreach (var lane in new[] { north, south })
            for (int i = 0; i + 1 < lane.Count; i++)
            {
                Vector2 q = Geometry2D.GetClosestPointToSegment(p, lane[i], lane[i + 1]);
                float d = q.DistanceTo(p);
                if (d < best) { best = d; bp = q; }
            }
        return bp;
    }

    /// <summary>Sim places/spots for homes, chapel, smithy, plaza (positions from the layout and manifests).</summary>
    void RegisterSpots()
    {
        // plaza: talk ring around the well + benches
        for (int i = 0; i < 8; i++)
        {
            float a = i * MathF.Tau / 8f + 0.2f;
            Vector2 p = _wellPos + new Vector2(MathF.Cos(a), MathF.Sin(a)) * 4.6f;
            Vector2 toWell = (_wellPos - p).Normalized();
            // stand a little sideways so pairs face each other rather than the well
            _life.AddSpot(Plaza, "talk", N(p), N(toWell.Rotated(i % 2 == 0 ? 0.9f : -0.9f)));
        }
        foreach (var (u, v) in new[] { (-4f, 1.5f), (-2.5f, 3f), (6f, 15.5f), (7.4f, 14.2f) })
            _life.AddSpot(Plaza, "talk", N(W(u, v)), V2.Zero);
        foreach (var (bu, bv) in new[] { (5f, 17.6f), (-3.5f, 18.2f) })
            foreach (var sx in new[] { -0.4f, 0.4f })
            {
                // bench faces north (-v); seat root in front of the seat
                Vector2 face = -_v;
                Vector2 seat = W(bu, bv) + new Vector2(face.Y, -face.X) * sx;
                _life.AddSpot(Plaza, "sit", N(seat + face * 0.58f), N(face));
            }
        // well: two draw points (north and south side of the crank)
        float wyaw = RegionSpec.YawFacing(_u);
        foreach (var side in new[] { 1f, -1f })
        {
            var lp = new Vector3(1.85f * side, 0, 0.1f);
            Vector2 pos = PropKit.ToWorld2(lp, new Vector3(_wellPos.X, 0, _wellPos.Y), wyaw);
            _life.AddSpot(Well, "draw", N(pos), N((_wellPos - pos).Normalized()));
        }
        // market stall at (-9, 12) facing +u
        float myaw = RegionSpec.YawFacing(_u);
        var mpos = new Vector3(W(-9, 12).X, 0, W(-9, 12).Y);
        _life.AddSpot(Market, "vendor", N(PropKit.ToWorld2(new Vector3(0, 0, -0.35f), mpos, myaw)), N(_u));
        for (int i = -1; i <= 1; i++)
            _life.AddSpot(Market, "customer", N(PropKit.ToWorld2(new Vector3(i * 0.9f, 0, 1.6f + MathF.Abs(i) * 0.3f), mpos, myaw)), N(-_u));

        foreach (var b in _blds)
        {
            switch (b.Kind)
            {
                case BKind.Chapel:
                {
                    Chapel = _life.AddPlace(PlaceKind.Chapel, "Işık Tapınağı", N(DoorPoint(b)), N(b.Face));
                    b.Place = Chapel;
                    _life.AddSpot(Chapel, "priest", N(b.LocalToWorld(0, 7.8f)), N(b.Face));
                    for (int i = 0; i < 6; i++)
                        _life.AddSpot(Chapel, "pray", N(b.LocalToWorld(-2.4f + (i % 3) * 2.4f, 10.6f + (i / 3) * 1.5f)), N(-b.Face));
                    break;
                }
                case BKind.Smithy:
                {
                    Smithy = _life.AddPlace(PlaceKind.Smithy, "demirhane", N(DoorPoint(b)), N(b.Face));
                    b.Place = Smithy;
                    _life.AddSpot(Smithy, "anvil", N(b.LocalToWorld(0.35f, 1.12f)), N(b.Face));
                    _life.AddSpot(Smithy, "forge", N(b.LocalToWorld(2.6f, 1.05f)), N(-b.Face));
                    _life.AddSpot(Smithy, "forge", N(b.LocalToWorld(1.55f, 2.35f)), N(-b.Face));
                    Vector2 gf = PropKit.DirToWorld(new Vector2(MathF.Sin(Mathf.DegToRad(-20f)), MathF.Cos(Mathf.DegToRad(-20f))), b.Yaw);
                    _life.AddSpot(Smithy, "grind", N(b.LocalToWorld(-0.93f, 2.14f)), N(gf));
                    _life.AddSpot(Smithy, "customer", N(b.LocalToWorld(0.6f, 5.4f)), N(-b.Face));
                    _life.AddSpot(Smithy, "customer", N(b.LocalToWorld(-0.8f, 5.6f)), N(-b.Face));
                    break;
                }
                default:
                {
                    var home = _life.AddPlace(PlaceKind.Home, $"ev {b.HomeIndex}", N(DoorPoint(b)), N(b.Face));
                    b.Place = home;
                    home.Tag = b.HomeIndex;
                    if (b.Kind == BKind.Large)
                    {
                        foreach (var sx in new[] { -2.3f, -1.5f })
                            _life.AddSpot(home, "sit", N(b.LocalToWorld(sx, 5.05f + 0.58f)), N(b.Face));
                        _life.AddSpot(home, "yard", N(b.LocalToWorld(3.6f, -6.8f)), N(-b.Face));
                        foreach (var (gx, gz) in new[] { (-2f, -8.2f), (0.5f, -9f), (2.4f, -8.4f) })
                            _life.AddSpot(home, "garden", N(b.LocalToWorld(gx, gz)), N(b.Face));
                    }
                    else
                    {
                        // bench along the side wall (left of the front, model -X side), seat faces -X outward
                        Vector2 sideFace = -new Vector2(b.Face.Y, -b.Face.X);
                        foreach (var sz in new[] { 1.4f, 2.2f })
                            _life.AddSpot(home, "sit", N(b.LocalToWorld(-3.05f, sz) + sideFace * 0.58f), N(sideFace));
                        _life.AddSpot(home, "yard", N(b.Kind == BKind.SmallA ? b.LocalToWorld(1.6f, -4.6f) : b.LocalToWorld(-0.6f, -5.2f)), N(b.Face));
                        foreach (var (gx, gz) in new[] { (-1.6f, -7f), (0f, -8.6f), (1.6f, -7.6f) })
                            _life.AddSpot(home, "garden", N(b.LocalToWorld(gx, gz)), N(-b.Face));
                    }
                    break;
                }
            }
        }
        // census wants homes ordered by HomeIndex: the sim sorts by place order, so re-order ids here
        var homes = _life.PlacesOf(PlaceKind.Home);
        homes.Sort((a, c) => ((int)a.Tag).CompareTo((int)c.Tag));
        HomeOrder = homes;
    }

    /// <summary>End of the north lane (the woodcutters' footpath continues from here).</summary>
    public Vector2 NorthLaneEnd => W(-21f, -47f);
    /// <summary>End of the south lane's farm track (toward the southern fields).</summary>
    public Vector2 SouthLaneEnd => W(0f, 108f);

    /// <summary>Homes in census order (0 headman, 1 smith, 2 shepherd, 3 woodcutter, 4+ farmers).</summary>
    public List<Place> HomeOrder { get; private set; }

    // ------------------------------------------------------------------ build

    public void Build(Region region)
    {
        var hf = region.Heightfield;
        var kit = new PropKit(region, "Village");
        var small = new PropKit(kit.Root, "VillageProps");
        Vector3 At(Vector2 p, float lift = 0) => new(p.X, hf.Height(p.X, p.Y) + lift, p.Y);
        var rng = new Rng(4242);

        int ri = 0;
        foreach (var b in _blds)
        {
            var pos = new Vector3(b.Pos.X, b.Y, b.Pos.Y);
            Color roof = RoofVariety[ri++ % RoofVariety.Length];
            switch (b.Kind)
            {
                case BKind.SmallA:
                case BKind.SmallB:
                {
                    string m = b.Kind == BKind.SmallA ? "human_house_a" : "human_house_b";
                    var node = kit.Place("human_house", m + "_hi", pos, b.Yaw, roof, 1f, hiEnd: 140f, collide: false, name: $"Ev{b.HomeIndex}");
                    if (node != null && Models.Meshes("human_house").TryGetValue(m, out var lo))
                    {
                        // the house glb has "<m>" as the low version
                        node.GetNode<MeshInstance3D>("hi").VisibilityRangeEnd = 140f;
                        var loMi = new MeshInstance3D { Name = "lo", Mesh = lo, VisibilityRangeBegin = 140f };
                        loMi.SetInstanceShaderParameter("tint", LinVec(roof));
                        node.AddChild(loMi);
                    }
                    HouseCollision(kit, pos, b.Yaw);
                    b.Place.Tag = node;
                    // back yard: woodpile (A houses) / clothesline / garden with cabbages and a low fence
                    if (b.Kind == BKind.SmallA) small.Batch("village", "woodpile", At(b.LocalToWorld(1.6f, -3.9f)), b.Yaw + MathF.PI);
                    for (int r = 0; r < 3; r++)
                        for (int k = 0; k < 7; k++)
                        {
                            Vector2 g = b.LocalToWorld(-1.6f + r * 1.6f + (rng.Next01() - 0.5f) * 0.2f, -6.0f - k * 0.72f);
                            small.Batch("village", "field_crop_cabbage", At(g, -0.02f), rng.Range(0, MathF.Tau), null, 0.9f + rng.Next01() * 0.3f, collide: false);
                        }
                    FenceRect(small, b.LocalToWorld(0, -8.35f), 2.9f, 3.05f, b.Yaw, gateSide: 0);
                    if (b.HomeIndex % 3 == 0) small.Batch("village", "clothesline", At(b.LocalToWorld(4.6f, -2.5f)), b.Yaw + MathF.PI / 2);
                    // Faz 2: köpek kulübesi kalktı (çocuk ve köpek yok)
                    // bench along the side wall
                    Vector2 sideFace = -new Vector2(b.Face.Y, -b.Face.X);
                    small.Batch("village", "bench", At(b.LocalToWorld(-3.05f, 1.8f)), RegionSpec.YawFacing(sideFace));
                    break;
                }
                case BKind.Large:
                {
                    var node = kit.Place("buildings", "human_house_large", pos, b.Yaw, roof, 1f, hiEnd: 160f, name: "Muhtar");
                    b.Place.Tag = node;
                    small.Batch("village", "woodpile", At(b.LocalToWorld(3.6f, -6.0f)), b.Yaw + MathF.PI);
                    for (int r = 0; r < 4; r++)
                        for (int k = 0; k < 6; k++)
                            small.Batch("village", "field_crop_cabbage", At(b.LocalToWorld(-2.4f + r * 1.6f, -7.6f - k * 0.7f), -0.02f), rng.Range(0, MathF.Tau), null, 1f, collide: false);
                    small.Batch("village", "haystack", At(b.LocalToWorld(-6.5f, -4f)), b.Yaw);
                    break;
                }
                case BKind.Smithy:
                {
                    var node = kit.Place("buildings", "smithy", pos, b.Yaw, RoofTint, 1f, hiEnd: 160f, name: "Demirhane");
                    Vector3 fire = PropKit.ToWorld(PropKit.Point("buildings", "smithy", "fire_point") ?? new Vector3(1.35f, 0.97f, 0.35f), pos, b.Yaw);
                    kit.Light(fire + Vector3.Up * 0.3f, FMath.Hex(0xff8a3a), 2.2f, 7f, flicker: true, alwaysOn: true);
                    Fx.Fire(kit.Root, fire, 0.35f);
                    Fx.Smoke(kit.Root, PropKit.ToWorld(PropKit.Point("buildings", "smithy", "chimney_top") ?? new Vector3(1.35f, 7.2f, 0.25f), pos, b.Yaw), 0.8f);
                    small.Batch("village", "barrel", At(b.LocalToWorld(-3.9f, 2.6f)), b.Yaw);
                    small.Batch("village", "crate", At(b.LocalToWorld(-4.0f, 1.6f)), b.Yaw + 0.4f);
                    small.Batch("village", "cart", At(b.LocalToWorld(5.6f, 1.2f)), b.Yaw + 1.9f);
                    break;
                }
                case BKind.Chapel:
                {
                    kit.Place("buildings", "chapel", pos, b.Yaw, FMath.Hex(0x6d6f78), 1f, hiEnd: 180f, name: "Tapınak");
                    Vector3 lamp = PropKit.ToWorld(PropKit.Point("buildings", "chapel", "lamp") ?? new Vector3(1.25f, 2.65f, 5.84f), pos, b.Yaw);
                    kit.Light(lamp, FMath.Hex(0xffc070), 1.1f, 7f);
                    break;
                }
            }
        }

        // plaza: well, market, benches, lamps
        float wyaw = RegionSpec.YawFacing(_u);
        kit.Place("village", "well", At(_wellPos), wyaw, null, 1f, 120f, name: "Kuyu");
        var mpos = At(W(-9, 12));
        kit.Place("village", "market_stall", mpos, RegionSpec.YawFacing(_u), FMath.Hex(0x3f6fa0), 1f, 120f, name: "Tezgah");
        small.Batch("village", "sack_pile", At(W(-11.5f, 14.6f)), wyaw + 0.3f);
        small.Batch("village", "barrel", At(W(-11f, 9.2f)), 0.2f);
        small.Batch("village", "crate", At(W(-10.4f, 8.6f)), 0.7f);
        foreach (var (bu, bv) in new[] { (5f, 17.6f), (-3.5f, 18.2f) })
            small.Batch("village", "bench", At(W(bu, bv)), RegionSpec.YawFacing(-_v));
        foreach (var (lu, lv) in new[] { (13.5f, 1.8f), (-13.5f, 3f), (1f, 22.4f), (-40f, -4f), (40f, 4.2f), (-19.5f, -24f) })
        {
            var lp = At(W(lu, lv));
            float ly = RegionSpec.YawFacing(-_v);
            kit.Place("village", "lamp_post", lp, ly, null, 1f, 120f, name: "Fener");
            kit.Light(PropKit.ToWorld(PropKit.Point("village", "lamp_post", "light_point") ?? new Vector3(0.72f, 2.3f, 0), lp, ly), FMath.Hex(0xffb35a), 1.3f, 9f, flicker: true);
        }
        small.Batch("village", "signpost", At(W(-62f, -4f)), RegionSpec.YawFacing(_u));
        small.Batch("village", "signpost", At(W(58f, -4.5f)), RegionSpec.YawFacing(-_u));
        small.Batch("village", "trough", At(W(9f, 4f)), RegionSpec.YawFacing(-_v));

        // sheep pen: rail fence with a gate on the pasture side, trough and hay inside
        FenceRect(small, _penCenter, 6.5f, 4.5f, _penYaw + MathF.PI / 2, gateSide: 2, gateModel: true);
        small.Batch("village", "trough", At(_penCenter + _u * 3.5f), _penYaw);
        small.Batch("village", "hay_bale", At(_penCenter - _u * 4f + _v * 2f), _penYaw + 0.3f);

        // fields: scarecrows and haystacks, a cart at the south track
        foreach (var f in RegionSpec.Fields)
        {
            if (f.Crop == CropKind.Fallow) continue;
            Vector2 at = f.Center + f.U * (f.HalfSize.X * 0.35f) + f.V * (f.HalfSize.Y * 0.2f);
            small.Batch("village", "scarecrow", At(at), rng.Range(0, MathF.Tau));
            Vector2 hay = f.Center - f.U * (f.HalfSize.X + 3.5f) + f.V * (f.HalfSize.Y * 0.6f);
            if (hf.RoadDistance(hay.X, hay.Y) > 6f) small.Batch("village", "haystack", At(hay), rng.Range(0, MathF.Tau), null, 0.9f);
        }
        small.Batch("village", "cart", At(W(4.5f, 96f)), RegionSpec.YawFacing(_v) + 0.5f);

        // shade trees on the plaza and by the chapel, fruit trees in a few gardens, bushes along the lanes
        void Tree(string obj, Vector2 at, float scale, Color leaf)
        {
            var node = kit.Place("nature", obj, At(at, -0.1f), rng.Range(0, MathF.Tau), leaf, 1f, hiEnd: 150f, collide: false);
            if (node != null) node.Scale = Vector3.One * scale;
            kit.AddBox(new Transform3D(Basis.Identity, At(at, 2f)), new Vector3(0.7f * scale, 4f, 0.7f * scale));
        }
        Tree("tree_oak_3", W(-15f, 19.5f), 0.95f, FMath.Hex(0x5f8f3c));
        Tree("tree_oak_2", W(16.5f, -8.5f), 0.85f, FMath.Hex(0x6a9440));
        Tree("tree_birch_1", W(9f, -27f), 0.9f, FMath.Hex(0x86a84a));
        Tree("tree_oak_1", W(-52f, 26f), 0.9f, FMath.Hex(0x5a8a3a));
        Tree("tree_birch_2", W(52f, -18f), 0.85f, FMath.Hex(0x8aac50));
        foreach (var b in _blds)
            if (b.Kind is BKind.SmallA or BKind.SmallB && b.HomeIndex % 2 == 0)
                Tree("tree_oak_1", b.LocalToWorld(5.2f, -8.8f), 0.55f, FMath.Hex(0x6f9a3e));
        foreach (var (bu, bv) in new[] { (-9f, 5f), (7f, 22f), (-22f, -7f), (33f, -6f), (-38f, 18f), (4f, 58f), (-4f, 64f) })
            small.Batch("nature", rng.Chance(0.5f) ? "bush_1" : "bush_3", At(W(bu, bv)), rng.Range(0, MathF.Tau), FMath.Hex(0x5f8a3a), 0.8f + rng.Next01() * 0.3f, collide: false);

        small.Flush(160f);
        kit.Flush(160f);
    }

    static Vector3 LinVec(Color c) { var l = c.SrgbToLinear(); return new Vector3(l.R, l.G, l.B); }

    /// <summary>Collision for the small human house: walls box + roof prism (model 5 × 6.25 m, ridge 7.4 m).</summary>
    static void HouseCollision(PropKit kit, Vector3 pos, float yaw)
    {
        var basis = Basis.FromEuler(new Vector3(0, yaw, 0));
        kit.AddBox(new Transform3D(basis, pos + basis * new Vector3(0, 2.0f, 0)), new Vector3(5.3f, 4.0f, 6.35f));
        kit.AddBox(new Transform3D(basis, pos + basis * new Vector3(0, 5.2f, 0)), new Vector3(3.2f, 2.6f, 7.0f));
    }

    /// <summary>Rail fence around a rectangle (local half sizes hx along the rect's X, hz along Z), gate on one side
    /// (0 = +Z, 1 = -Z, 2 = +X, 3 = -X).</summary>
    void FenceRect(PropKit kit, Vector2 center, float hx, float hz, float yaw, int gateSide = -1, bool gateModel = false)
    {
        var basis = Basis.FromEuler(new Vector3(0, yaw, 0));
        Vector3 L(float x, float z) { var w = basis * new Vector3(x, 0, z); float px = center.X + w.X, pz = center.Y + w.Z; return new Vector3(px, _hf.Height(px, pz), pz); }
        void Side(float x0, float z0, float x1, float z1, bool gate)
        {
            float len = MathF.Sqrt((x1 - x0) * (x1 - x0) + (z1 - z0) * (z1 - z0));
            int n = Math.Max(1, (int)MathF.Round(len / 2f));
            float segYaw = yaw + MathF.Atan2(-(z1 - z0), x1 - x0);   // rail runs along its local +X
            int gateSeg = gate ? n / 2 : -1;
            for (int i = 0; i < n; i++)
            {
                float t = (i + 0.5f) / n;
                var p = L(x0 + (x1 - x0) * t, z0 + (z1 - z0) * t);
                if (i == gateSeg)
                {
                    if (gateModel) kit.Batch("village", "gate", p, segYaw, collide: false);
                    continue;
                }
                kit.Batch("village", "fence_rail", p, segYaw, null, len / n / 2f);
            }
        }
        Side(-hx, hz, hx, hz, gateSide == 0);
        Side(hx, -hz, -hx, -hz, gateSide == 1);
        Side(hx, hz, hx, -hz, gateSide == 2);
        Side(-hx, -hz, -hx, hz, gateSide == 3);
    }
}
