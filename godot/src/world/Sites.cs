using System;
using System.Collections.Generic;
using Godot;
using FD.Core;
using FD.Sim.Life;
using Rng = FD.Sim.Life.Rng;
using V2 = System.Numerics.Vector2;

namespace FD.World;

static class SiteUtil
{
    public static V2 N(Vector2 p) => new(p.X, p.Y);
    public static List<V2> N(IEnumerable<Vector2> pts) { var r = new List<V2>(); foreach (var p in pts) r.Add(N(p)); return r; }
    public static Vector3 At(Heightfield hf, Vector2 p, float lift = 0) => new(p.X, hf.Height(p.X, p.Y) + lift, p.Y);
    public static Vector2 LocalToWorld(Vector2 origin, float yaw, float x, float z)
    {
        var w = new Vector3(x, 0, z).Rotated(Vector3.Up, yaw);
        return origin + new Vector2(w.X, w.Z);
    }
}

/// <summary>The "Yorgun Katır" inn by the road: model, porch seats, stable, trough, lights; sim places.</summary>
public sealed class InnSite : IRegionFeature
{
    Vector2 _origin;
    float _yaw;
    Place _inn, _yard;

    public void Shape(Heightfield hf)
    {
        var life = Region.Current.Life;
        _yaw = RegionSpec.YawFacing(RegionSpec.InnFront);
        // centre the model's footprint (offset 2.975, 1.775) on the pad
        Vector2 off = SiteUtil.LocalToWorld(Vector2.Zero, _yaw, 2.975f, 1.775f);
        _origin = RegionSpec.Inn - off + RegionSpec.InnFront * 2.5f;
        Vector2 steps = SiteUtil.LocalToWorld(_origin, _yaw, 0, 8.4f);
        RegionSpec.Road.Distance(steps, out float s, out _);
        Vector2 road = RegionSpec.Road.PointAt(s);
        var path = new[] { steps, steps.Lerp(road, 0.5f), road };
        hf.AddPath(path, 2.6f, 0.5f, 0.9f);
        life.Graph.AddPolyline(SiteUtil.N(path), 3f, 1.2f);

        _inn = life.AddPlace(PlaceKind.Inn, life.InnName, SiteUtil.N(steps), SiteUtil.N(RegionSpec.InnFront));
        {
            // main body (walls) of the inn, without porch and stable, for route tests
            Vector2 bodyC = SiteUtil.LocalToWorld(_origin, _yaw, 0f, -0.2f);
            float ang = MathF.Atan2(-RegionSpec.InnFront.X, RegionSpec.InnFront.Y);
            life.Obstacles.Add((SiteUtil.N(bodyC), new V2(7.8f, 5.0f), ang, "inn"));
        }
        V2 L(float x, float z) => SiteUtil.N(SiteUtil.LocalToWorld(_origin, _yaw, x, z));
        V2 F(float lx, float lz) => SiteUtil.N(PropKit.DirToWorld(new Vector2(lx, lz), _yaw));
        life.AddSpot(_inn, "keeper", L(2.8f, 5.0f), F(0, 1), 0.45f);
        foreach (var n in PropKit.PointNames("buildings", "inn"))
        {
            if (!n.StartsWith("porch_seat")) continue;
            var p = PropKit.Point("buildings", "inn", n).Value;
            var f = PropKit.PointFace("buildings", "inn", n);
            var root = new Vector2(p.X, p.Z) + f * 0.58f;
            life.AddSpot(_inn, "sit", L(root.X, root.Y), F(f.X, f.Y), MathF.Max(0, p.Y - 0.45f));
        }
        foreach (var (x, z, fz) in new[] { (-4.9f, 7.7f, -1f), (5.2f, 7.7f, -1f), (-2.4f, 6.1f, 1f), (1.2f, 6.9f, 1f), (-6.9f, 6.0f, 1f) })
            life.AddSpot(_inn, "serve", L(x, z), F(0, fz), 0.45f);

        // yard: the whole plot is walkable; stable stalls and the trough
        _yard = life.AddPlace(PlaceKind.InnYard, "han avlusu", L(11.6f, 5.5f), F(0, 1));
        _yard.Center = SiteUtil.N(RegionSpec.Inn); _yard.IsRect = true; _yard.Half = new V2(RegionSpec.InnPadHalfSize.X - 1, RegionSpec.InnPadHalfSize.Y - 1);
        _yard.AngleRad = Mathf.DegToRad(RegionSpec.InnAngleDeg); _yard.Area = 4;
        _inn.Area = 4;
        foreach (var n in new[] { "stable_stall_1", "stable_stall_2", "stable_stall_3" })
        {
            var p = PropKit.Point("buildings", "inn", n) ?? Vector3.Zero;
            life.AddSpot(_yard, "stall", L(p.X, p.Z + 1.1f), F(0, -1));
        }
        life.AddSpot(_yard, "trough", L(18.4f, 6.9f), F(-1, 0));
    }

    public void Build(Region region)
    {
        var hf = region.Heightfield;
        var kit = new PropKit(region, "Inn");
        float y = hf.InnPadHeight;
        var pos = new Vector3(_origin.X, y, _origin.Y);
        kit.Place("buildings", "inn", pos, _yaw, FMath.Hex(0x9c3f2b), 1f, hiEnd: 200f, name: "Han");
        foreach (var n in new[] { "lamp_1", "lamp_2" })
            kit.Light(PropKit.ToWorld(PropKit.Point("buildings", "inn", n) ?? new Vector3(0, 2.3f, 4.9f), pos, _yaw), FMath.Hex(0xffb35a), 1.2f, 8f, flicker: true);
        Fx.Smoke(kit.Root, PropKit.ToWorld(PropKit.Point("buildings", "inn", "chimney_top") ?? new Vector3(-8.5f, 12.9f, -1f), pos, _yaw), 1.1f);
        Vector3 W(float x, float z) { var p = SiteUtil.LocalToWorld(_origin, _yaw, x, z); return SiteUtil.At(hf, p); }
        kit.Place("village", "trough", W(17.6f, 6.9f), _yaw + MathF.PI / 2, null, 1f, 120f);
        kit.Batch("village", "hay_bale", W(18.6f, 3.2f), _yaw + 0.2f);
        kit.Batch("village", "hay_bale", W(19.1f, 4.1f), _yaw - 0.4f);
        kit.Batch("village", "barrel", W(-9.4f, 5.6f), 0.3f);
        kit.Batch("village", "barrel", W(-9.9f, 4.9f), 1.1f);
        kit.Batch("village", "crate", W(-10.1f, 6.4f), 0.6f);
        kit.Batch("village", "woodpile", W(-12.5f, -3.5f), _yaw + MathF.PI / 2);
        // torches along the path to the road and a signpost
        Vector2 steps = SiteUtil.LocalToWorld(_origin, _yaw, 0, 8.4f);
        RegionSpec.Road.Distance(steps, out float s, out _);
        Vector2 road = RegionSpec.Road.PointAt(s);
        foreach (var t in new[] { 0.35f, 0.75f })
        {
            Vector2 p = steps.Lerp(road, t);
            Vector2 side = new Vector2(-(road - steps).Normalized().Y, (road - steps).Normalized().X) * 2.2f;
            var tp = SiteUtil.At(hf, p + side);
            kit.Place("village", "torch_post", tp, _yaw, null, 1f, 100f);
            var lp = PropKit.ToWorld(PropKit.Point("village", "torch_post", "light_point") ?? new Vector3(0.21f, 2.12f, 0), tp, _yaw);
            kit.Light(lp, FMath.Hex(0xff9a40), 1.4f, 8f, flicker: true);
            Fx.Fire(kit.Root, lp, 0.12f);
        }
        kit.Batch("village", "signpost", SiteUtil.At(hf, road + (road - steps).Normalized() * -3.5f + RegionSpec.Road.TangentAt(s) * 4f), _yaw);
        kit.Flush(150f);
    }
}

/// <summary>The goblin camp in the forest ("Kırık Diş kampı"): palisade, tents, fire, spit, totem; sim places,
/// plus the night ambush spot by the forest trail.</summary>
public sealed class CampSite : IRegionFeature
{
    Vector2 _c, _entrance;
    float _entAng;
    /// <summary>Faz 2 D: the cage (prisoner point inside, door point outside its front bars) and its yaw</summary>
    public static Vector2 CagePrisoner, CageDoor;
    public static float CageYaw;
    Place _camp, _lurk;
    const float Palisade = 13f;

    public void Shape(Heightfield hf)
    {
        var life = Region.Current.Life;
        _c = RegionSpec.GoblinCamp;
        Vector2 spurEnd = RegionSpec.CampSpurControl[^1];
        Vector2 e = (spurEnd - _c).Normalized();
        _entAng = MathF.Atan2(e.Y, e.X);
        _entrance = _c + e * (Palisade - 0.5f);
        hf.AddPadCircle(_c, Palisade + 2f, float.NaN, 7f, 0.9f, 3f);
        var spur = new[] { spurEnd, _entrance + e * 2f, _entrance, _c + e * 4f };
        hf.AddPath(spur, 1.8f, 0.3f, 0.8f);
        life.Graph.AddPolyline(SiteUtil.N(spur), 3f, 0.8f);

        _camp = life.AddPlace(PlaceKind.Camp, life.CampName, SiteUtil.N(_entrance), SiteUtil.N(e));
        _camp.Center = SiteUtil.N(_c); _camp.Radius = Palisade - 1.5f; _camp.Area = 5;
        // campfire seats (manifest) — campfire at the centre, yaw so seats face the fire
        float fyaw = RegionSpec.YawFacing(e);
        foreach (var n in new[] { "seat_1", "seat_2" })
        {
            var p = PropKit.Point("buildings", "campfire", n) ?? Vector3.Zero;
            var f = PropKit.PointFace("buildings", "campfire", n);
            Vector2 root = new Vector2(p.X, p.Z) + f * 0.55f;
            var w = SiteUtil.LocalToWorld(_c, fyaw, root.X, root.Y);
            life.AddSpot(_camp, "sit", SiteUtil.N(w), SiteUtil.N(PropKit.DirToWorld(f, fyaw)), MathF.Max(0, p.Y - 0.3f));
        }
        foreach (var n in new[] { "stand_1", "stand_2", "stand_3" })
        {
            var p = PropKit.Point("buildings", "campfire", n) ?? Vector3.Zero;
            var w = SiteUtil.LocalToWorld(_c, fyaw, p.X, p.Z);
            life.AddSpot(_camp, "sit", SiteUtil.N(w), SiteUtil.N((_c - w).Normalized()));
        }
        // spit roast beside the fire
        Vector2 spit = _c + new Vector2(-e.Y, e.X) * 4.2f;
        float syaw = RegionSpec.YawFacing((_c - spit).Normalized());
        var tp = PropKit.Point("buildings", "spit_roast", "turn_point") ?? new Vector3(1.3f, 0, 0.55f);
        life.AddSpot(_camp, "spit", SiteUtil.N(SiteUtil.LocalToWorld(spit, syaw, tp.X, tp.Z)), SiteUtil.N(PropKit.DirToWorld(new Vector2(0, -1), syaw)));
        // tents (entrances)
        foreach (var (ang, _) in TentSlots())
        {
            Vector2 tpos = _c + new Vector2(MathF.Cos(ang), MathF.Sin(ang)) * 8f;
            float ty = RegionSpec.YawFacing((_c - tpos).Normalized());
            var ent = PropKit.Point("buildings", "goblin_tent_1", "entrance") ?? new Vector3(0, 0, 2f);
            life.AddSpot(_camp, "tent", SiteUtil.N(SiteUtil.LocalToWorld(tpos, ty, ent.X, ent.Z + 0.4f)), SiteUtil.N((_c - tpos).Normalized()));
        }
        // patrol ring inside the palisade + two posts outside the gate
        for (int i = 0; i < 8; i++)
        {
            float a = _entAng + MathF.PI / 8 + i * MathF.Tau / 8f;
            Vector2 p = _c + new Vector2(MathF.Cos(a), MathF.Sin(a)) * (Palisade - 2.8f);
            life.AddSpot(_camp, "patrol", SiteUtil.N(p), SiteUtil.N(new Vector2(MathF.Cos(a), MathF.Sin(a))));
        }
        foreach (float k in new[] { 4f, 9f })
            life.AddSpot(_camp, "patrol", SiteUtil.N(_entrance + e * k + new Vector2(-e.Y, e.X) * 1.5f), SiteUtil.N(e));

        // ambush spot by the forest trail (night raiders)
        float sT = RegionSpec.Trail.Length * 0.45f;
        Vector2 tpt = RegionSpec.Trail.PointAt(sT), tt = RegionSpec.Trail.TangentAt(sT), tn = new(-tt.Y, tt.X);
        Vector2 hide = tpt + tn * 6.5f;
        if (hf.ForestDensity(hide.X, hide.Y) < hf.ForestDensity((tpt - tn * 6.5f).X, (tpt - tn * 6.5f).Y)) { tn = -tn; hide = tpt + tn * 6.5f; }
        _lurk = life.AddPlace(PlaceKind.Lurk, "pusu yeri", SiteUtil.N(hide), SiteUtil.N(-tn));
        _lurk.Center = SiteUtil.N(hide); _lurk.Radius = 6f; _lurk.Area = 6;
        for (int i = -1; i <= 1; i++)
            life.AddSpot(_lurk, "lurk", SiteUtil.N(hide + tt * (i * 3.2f) + tn * (MathF.Abs(i) * 0.8f)), SiteUtil.N(-tn));
        var lurkPath = new[] { tpt, tpt + tn * 3f, hide };
        life.Graph.AddPolyline(SiteUtil.N(lurkPath), 2.5f, 0.5f);
    }

    IEnumerable<(float ang, string model)> TentSlots()
    {
        yield return (_entAng + Mathf.DegToRad(105), "goblin_tent_1");
        yield return (_entAng + Mathf.DegToRad(180), "goblin_tent_2");
        yield return (_entAng + Mathf.DegToRad(255), "goblin_tent_1");
    }

    public void Build(Region region)
    {
        var hf = region.Heightfield;
        var kit = new PropKit(region, "GoblinCamp");
        var rng = new Rng(777);
        Vector2 e = new(MathF.Cos(_entAng), MathF.Sin(_entAng));
        float fyaw = RegionSpec.YawFacing(e);
        var fire = SiteUtil.At(hf, _c);
        kit.Place("buildings", "campfire", fire, fyaw, null, 1f, 120f);
        Vector3 fp = fire + Vector3.Up * 0.35f;
        kit.Light(fp + Vector3.Up * 0.4f, FMath.Hex(0xff7a2a), 3.2f, 13f, flicker: true, alwaysOn: true);
        Fx.Fire(kit.Root, fp, 0.55f);
        Fx.Smoke(kit.Root, fp + Vector3.Up * 1.2f, 0.7f);
        Vector2 spit = _c + new Vector2(-e.Y, e.X) * 4.2f;
        kit.Place("buildings", "spit_roast", SiteUtil.At(hf, spit), RegionSpec.YawFacing((_c - spit).Normalized()), null, 1f, 100f);
        Fx.Fire(kit.Root, SiteUtil.At(hf, spit, 0.3f), 0.25f);
        int t = 0;
        foreach (var (ang, model) in TentSlots())
        {
            Vector2 tpos = _c + new Vector2(MathF.Cos(ang), MathF.Sin(ang)) * 8f;
            kit.Place("buildings", t == 2 ? "goblin_tent_2" : model, SiteUtil.At(hf, tpos), RegionSpec.YawFacing((_c - tpos).Normalized()), null, 1f, 140f);
            t++;
        }
        Vector2 lean = _c + new Vector2(MathF.Cos(_entAng - 1.9f), MathF.Sin(_entAng - 1.9f)) * 8.5f;
        kit.Place("buildings", "goblin_leanto", SiteUtil.At(hf, lean), RegionSpec.YawFacing((_c - lean).Normalized()), null, 1f, 120f);
        Vector2 totem = _c + new Vector2(e.Y, -e.X) * 3.5f - e * 2.5f;
        kit.Place("buildings", "totem", SiteUtil.At(hf, totem), fyaw, null, 1f, 150f);
        kit.Place("buildings", "bone_pile", SiteUtil.At(hf, _c - e * 6f + new Vector2(-e.Y, e.X) * 2f), 0.6f, null, 1f, 90f);
        Vector2 cage = _c + new Vector2(MathF.Cos(_entAng + 2.6f), MathF.Sin(_entAng + 2.6f)) * 9.5f;
        kit.Place("buildings", "cage", SiteUtil.At(hf, cage), fyaw + 2.2f, null, 1f, 100f);
        CageYaw = fyaw + 2.2f;
        var pp = PropKit.Point("buildings", "cage", "prisoner_point") ?? new Vector3(0, 0, -0.1f);
        var dp = PropKit.Point("buildings", "cage", "door_point") ?? new Vector3(0, 0, 1.35f);
        CagePrisoner = SiteUtil.LocalToWorld(cage, CageYaw, pp.X, pp.Z);
        CageDoor = SiteUtil.LocalToWorld(cage, CageYaw, dp.X, dp.Z);
        foreach (float side in new[] { -1f, 1f })
            kit.Place("buildings", "goblin_banner", SiteUtil.At(hf, _entrance + new Vector2(-e.Y, e.X) * side * 3.2f + e * 0.8f), fyaw, FMath.Hex(0x6a1e1a), 1f, 150f);
        // palisade ring with a gap at the entrance
        float gap = 2.9f / Palisade;
        int n = (int)(MathF.Tau * Palisade / 0.52f);
        for (int i = 0; i < n; i++)
        {
            float a = i * MathF.Tau / n;
            float da = MathF.Abs(Mathf.AngleDifference(a, _entAng));
            if (da < gap) continue;
            Vector2 p = _c + new Vector2(MathF.Cos(a), MathF.Sin(a)) * (Palisade + (rng.Next01() - 0.5f) * 0.25f);
            kit.Batch("buildings", "palisade_stake", SiteUtil.At(hf, p, -0.15f), rng.Range(0, MathF.Tau), null, 0.85f + rng.Next01() * 0.3f);
        }
        kit.Flush(180f);
    }
}

/// <summary>The ruined watchtower on the south-western hilltop (a place to look out over the region).</summary>
public sealed class RuinSite : IRegionFeature
{
    public void Shape(Heightfield hf) { }

    public void Build(Region region)
    {
        var hf = region.Heightfield;
        var kit = new PropKit(region, "Ruin");
        Vector2 look = (RegionSpec.VillageCenter - RegionSpec.Ruin).Normalized();
        kit.Place("buildings", "ruin_tower", new Vector3(RegionSpec.Ruin.X, hf.RuinTopHeight, RegionSpec.Ruin.Y), RegionSpec.YawFacing(look), null, 1f, 260f, name: "EskiGozcuKulesi");
        var life = region.Life;
        var pl = life.AddPlace(PlaceKind.Ruin, "Eski Gözcü Kulesi", SiteUtil.N(RegionSpec.Ruin + look * 7f), SiteUtil.N(look));
        pl.Center = SiteUtil.N(RegionSpec.Ruin); pl.Radius = 9f;
        kit.Flush();
    }
}

/// <summary>
/// Everything around the village that gives people somewhere to go: the woodcutters' clearing at the forest
/// edge (with a trodden footpath from the north lane), the washing place on the stream bank, the field plots,
/// the road ends where travellers come and go, and a signpost at the forest-trail junction.
/// </summary>
public sealed class OutskirtsSite : IRegionFeature
{
    public static readonly Vector2 LoggingCenter = new(-186, -121);
    readonly VillageSite _village;
    Place _logging, _wash;
    Vector2 _washSpot, _washFace;

    public OutskirtsSite(VillageSite village) { _village = village; }

    static readonly string[] CropNames = { "nadas tarlası", "buğday tarlası", "arpa tarlası", "sebze tarlası", "keten tarlası" };

    public void Shape(Heightfield hf)
    {
        var life = Region.Current.Life;
        // road ends (east first: travellers mostly arrive from the east)
        var east = RegionSpec.RoadControl[^1];
        var west = RegionSpec.RoadControl[0];
        life.AddPlace(PlaceKind.RoadEnd, "doğu yolu", SiteUtil.N(east), SiteUtil.N(new Vector2(-1, 0)));
        life.AddPlace(PlaceKind.RoadEnd, "batı yolu", SiteUtil.N(west), SiteUtil.N(new Vector2(1, 0)));

        // fields
        for (int i = 0; i < RegionSpec.Fields.Length; i++)
        {
            var f = RegionSpec.Fields[i];
            RegionSpec.Road.Distance(f.Center, out float s, out _);
            Vector2 roadP = RegionSpec.Road.PointAt(s);
            Vector2 edge = f.Center + (roadP - f.Center).Normalized() * MathF.Min(f.HalfSize.Y, (roadP - f.Center).Length());
            var pl = life.AddPlace(PlaceKind.Field, CropNames[(int)f.Crop], SiteUtil.N(edge), SiteUtil.N((f.Center - edge).Normalized()));
            pl.Center = SiteUtil.N(f.Center); pl.IsRect = true; pl.AngleRad = Mathf.DegToRad(f.AngleDeg); pl.Half = new V2(f.HalfSize.X, f.HalfSize.Y);
            pl.Area = 20 + i;
        }

        // farm track west from the road's end, between the western fields
        var track = new[] { RegionSpec.RoadControl[0], new Vector2(-382, 146.5f), new Vector2(-442, 141f) };
        hf.AddPath(track, 2.6f, 0.45f, 0.8f);
        life.Graph.AddPolyline(SiteUtil.N(track), 5f, 1.2f);

        // farm tracks between the southern fields (joined to the end of the village's south lane)
        var laneS = _village.SouthLaneEnd;
        var t1 = new[] { laneS, new Vector2(-188, 227), new Vector2(-187.5f, 269), new Vector2(-188, 314) };
        var t2a = new[] { new Vector2(-124, 269.5f), new Vector2(-187.5f, 269) };
        var t2b = new[] { new Vector2(-187.5f, 269), new Vector2(-264, 271) };
        foreach (var t in new[] { t1, t2a, t2b })
        {
            hf.AddPath(t, 2.2f, 0.35f, 0.7f);
            life.Graph.AddPolyline(SiteUtil.N(t), 5f, 1.0f, snap: 3f, attach: 10f);
        }

        // woodcutters' clearing at the forest edge north of the village + footpath from the north lane
        Vector2 lg = LoggingCenter;
        hf.AddPadCircle(lg, 8f, float.NaN, 5f, 0.55f, 1.5f);
        var laneEnd = _village.NorthLaneEnd;
        var foot = new[] { laneEnd, new Vector2(-206, 22), new Vector2(-197, -48), new Vector2(-190, -100), lg + new Vector2(0, 5) };
        hf.AddPath(foot, 1.5f, 0.35f, 0.7f);
        life.Graph.AddPolyline(SiteUtil.N(foot), 4f, 0.7f, snap: 2f, attach: 12f);
        _logging = life.AddPlace(PlaceKind.Logging, "odun kesim yeri", SiteUtil.N(lg + new Vector2(0, 5)), SiteUtil.N(new Vector2(0, -1)));
        _logging.Center = SiteUtil.N(lg); _logging.Radius = 11f; _logging.Area = 7;

        // washing place on the stream bank, ~26 m downstream (south) of the bridge, village side
        float ss = RegionSpec.BridgeStreamS + 26f;
        Vector2 sp = RegionSpec.Stream.PointAt(ss), st = RegionSpec.Stream.TangentAt(ss), sn = new(-st.Y, st.X);
        if ((sp + sn * 5f).X > (sp - sn * 5f).X) sn = -sn;      // west bank (toward the village)
        Vector2 bank = sp;
        for (float d = 1f; d < 12f; d += 0.25f) { bank = sp + sn * d; if (!hf.IsWater(bank.X, bank.Y)) break; }
        _washSpot = bank + sn * 0.5f;
        _washFace = -sn;
        RegionSpec.Road.Distance(_washSpot, out float rs, out _);
        Vector2 roadPt = RegionSpec.Road.PointAt(rs - 8f);
        var wpath = new[] { roadPt, roadPt.Lerp(_washSpot + sn * 3f, 0.5f), _washSpot + sn * 3f, _washSpot + sn * 1.2f };
        hf.AddPath(wpath, 1.3f, 0.25f, 0.75f);
        life.Graph.AddPolyline(SiteUtil.N(wpath), 3f, 0.6f);
        _wash = life.AddPlace(PlaceKind.Washing, "yıkama yeri", SiteUtil.N(_washSpot + sn * 1.2f), SiteUtil.N(_washFace));
        _wash.Center = SiteUtil.N(_washSpot + sn * 1f); _wash.Radius = 4.5f; _wash.Area = 8;
        for (int i = -1; i <= 1; i++)
            life.AddSpot(_wash, "wash", SiteUtil.N(_washSpot + st * (i * 1.7f)), SiteUtil.N(_washFace));

        // trail junction: clear a little for a signpost
        hf.ExcludeVegetation(RegionSpec.TrailControl[0] + new Vector2(4, -6), 3f);
    }

    public void Build(Region region)
    {
        var hf = region.Heightfield;
        var life = region.Life;
        var kit = new PropKit(region, "Outskirts");
        var rng = new Rng(99);
        Vector2 lg = LoggingCenter;
        // clearing props: log pile, chopping block, stumps
        kit.Place("village", "log_pile", SiteUtil.At(hf, lg + new Vector2(-4, 1.5f)), 0.25f, null, 1f, 120f);
        float byaw = RegionSpec.YawFacing(new Vector2(0, 1));
        var block = lg + new Vector2(3.2f, 2.2f);
        kit.Place("village", "chopping_block", SiteUtil.At(hf, block), byaw, null, 1f, 100f);
        var wp = PropKit.Point("village", "chopping_block", "work_point") ?? new Vector3(0, 0, 0.72f);
        life.AddSpot(_logging, "block", SiteUtil.N(SiteUtil.LocalToWorld(block, byaw, wp.X, wp.Z)), SiteUtil.N(PropKit.DirToWorld(PropKit.PointFace("village", "chopping_block", "work_point"), byaw)));
        foreach (var o in new[] { new Vector2(5.5f, -3f), new Vector2(-6.5f, -4.5f), new Vector2(1.5f, -6.8f), new Vector2(7f, 3.5f) })
            kit.Batch("nature", "stump", SiteUtil.At(hf, lg + o), rng.Range(0, MathF.Tau), null, 0.9f + rng.Next01() * 0.25f);
        kit.Batch("nature", "log_fallen", SiteUtil.At(hf, lg + new Vector2(-1.5f, 6.5f)), 1.2f, null, 0.8f);

        // chop spots at real trees around the clearing
        var trees = new List<(Vector2 p, float d)>();
        foreach (var (p, _) in region.Vegetation.Trees)
        {
            float d = (p - lg).Length();
            if (d > 9f && d < 26f) trees.Add((p, d));
        }
        trees.Sort((a, b) => a.d.CompareTo(b.d));
        int added = 0;
        foreach (var (p, _) in trees)
        {
            Vector2 toC = (lg - p).Normalized();
            Vector2 stand = p + toC * 1.25f;
            life.AddSpot(_logging, "chop", SiteUtil.N(stand), SiteUtil.N(-toC));
            if (++added >= 6) break;
        }
        if (added == 0)
            for (int i = 0; i < 4; i++) life.AddSpot(_logging, "chop", SiteUtil.N(lg + new Vector2(MathF.Cos(i * 1.6f), MathF.Sin(i * 1.6f)) * 9f), V2.Zero);

        // washing place: a flat stone and a basket
        kit.Batch("nature", "rock_m", SiteUtil.At(hf, _washSpot + _washFace * 0.9f, -0.5f), 0.4f, null, 0.8f, collide: false);
        kit.Batch("village", "bucket", SiteUtil.At(hf, _washSpot - _washFace * 1.4f + new Vector2(_washFace.Y, -_washFace.X) * 1.2f), 0.3f, collide: false);

        // signposts
        Vector2 j = RegionSpec.TrailControl[0] + new Vector2(4, -6);
        kit.Batch("village", "signpost", SiteUtil.At(hf, j), RegionSpec.YawFacing(new Vector2(-1, 0.4f).Normalized()));
        kit.Flush(160f);
    }
}
