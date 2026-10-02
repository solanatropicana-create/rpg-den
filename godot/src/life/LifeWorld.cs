using System;
using System.Collections.Generic;
using Godot;
using FD.Actors;
using FD.Core;
using FD.Sim.Life;
using FD.World;

namespace FD.Life;

/// <summary>
/// Runs the life simulation (<see cref="LifeSim"/>) on the game clock and materialises it: a
/// <see cref="PersonActor"/> per person, the sheep flock, the merchant's ox cart, lit windows and chimney smoke of
/// occupied homes, greetings, and the goblin strike feedback. Player interaction (E) and the UI live in
/// <see cref="FD.UI.Hud"/>.
/// </summary>
public partial class LifeWorld : Node3D
{
    public static LifeWorld Instance { get; private set; }
    public LifeSim Sim { get; private set; }
    public readonly List<PersonActor> Actors = new();
    Region _region;
    readonly List<MeshInstance3D> _sheep = new();
    Node3D _cart, _ox, _wheelL, _wheelR;
    float _cartYaw;
    float _windowTimer;
    readonly Dictionary<int, (MeshInstance3D[] geoms, CpuParticles3D smoke)> _homes = new();
    public float DebugRealDt;
    public static double SimMs, SyncMs, OtherMs;
    readonly System.Diagnostics.Stopwatch _sw = new();
    public event Action<Person> Struck;

    public void Init(Region region, LifeSim sim)
    {
        Instance = this;
        _region = region;
        Sim = sim;
        Name = "LifeWorld";
        foreach (var p in sim.People)
        {
            var a = PersonActor.Create(p);
            AddChild(a);
            Actors.Add(a);
        }
        BuildSheep();
        BuildCart();
        BuildHomes();
        sim.GoblinStrike += g => Struck?.Invoke(g);
        sim.DoorUsed += (p, entered) => { };
    }

    public override void _ExitTree() { if (Instance == this) Instance = null; }

    void BuildSheep()
    {
        var mesh = Models.Mesh("village", "sheep", MatKind.Static);
        var root = new Node3D { Name = "Sheep" };
        AddChild(root);
        for (int i = 0; i < Sim.Flock.Pos.Count; i++)
        {
            MeshInstance3D mi;
            if (mesh != null) mi = new MeshInstance3D { Mesh = mesh, Name = $"Sheep{i}" };
            else mi = new MeshInstance3D { Mesh = new BoxMesh { Size = new Vector3(0.7f, 0.8f, 1.2f) }, Name = $"Sheep{i}" };
            mi.Scale = Vector3.One * (0.85f + H.Hash(i, 3) * 0.3f);
            mi.VisibilityRangeEnd = 160f;
            root.AddChild(mi);
            _sheep.Add(mi);
        }
    }

    void BuildCart()
    {
        if (!Models.Exists("village")) return;
        var meshes = Models.Meshes("village", MatKind.Static);
        _cart = new Node3D { Name = "MerchantCart", Visible = false };
        AddChild(_cart);
        if (meshes.TryGetValue("cart", out var cm)) _cart.AddChild(new MeshInstance3D { Mesh = cm, VisibilityRangeEnd = 180f });
        _wheelL = new Node3D { Name = "WL", Position = new Vector3(-0.8f, 0.62f, -0.8f) };
        _wheelR = new Node3D { Name = "WR", Position = new Vector3(0.8f, 0.62f, -0.8f) };
        _cart.AddChild(_wheelL); _cart.AddChild(_wheelR);
        // wheel meshes are modelled at their place in the cart; re-centre them on the hub pivot
        if (meshes.TryGetValue("cart_wheel_L", out var wl)) _wheelL.AddChild(new MeshInstance3D { Mesh = wl, Position = -_wheelL.Position, VisibilityRangeEnd = 120f });
        if (meshes.TryGetValue("cart_wheel_R", out var wr)) _wheelR.AddChild(new MeshInstance3D { Mesh = wr, Position = -_wheelR.Position, VisibilityRangeEnd = 120f });
        var hitch = PropKit.Point("village", "cart", "hitch_point") ?? new Vector3(0, 0, 2.45f);
        _ox = new Node3D { Name = "Ox", Position = hitch };
        _cart.AddChild(_ox);
        if (meshes.TryGetValue("ox", out var om)) _ox.AddChild(new MeshInstance3D { Mesh = om, VisibilityRangeEnd = 180f });
        // cargo: sacks and a crate on the bed
        var cargo = PropKit.Point("village", "cart", "cargo") ?? new Vector3(0, 0.84f, -0.8f);
        if (meshes.TryGetValue("sack", out var sk)) { _cart.AddChild(new MeshInstance3D { Mesh = sk, Position = cargo + new Vector3(-0.3f, 0, 0.2f), VisibilityRangeEnd = 90f }); }
        if (meshes.TryGetValue("crate", out var cr)) { _cart.AddChild(new MeshInstance3D { Mesh = cr, Position = cargo + new Vector3(0.25f, 0, -0.3f), Rotation = new Vector3(0, 0.3f, 0), VisibilityRangeEnd = 90f }); }
    }

    void BuildHomes()
    {
        foreach (var pl in Sim.Places)
        {
            if (pl.Kind != PlaceKind.Home || pl.Tag is not Node3D node) continue;
            var geoms = new List<MeshInstance3D>();
            foreach (var c in node.GetChildren()) if (c is MeshInstance3D mi) geoms.Add(mi);
            // chimney: small house A (model x 1.2, z -1.3, top 8.63 m), large house from its manifest
            CpuParticles3D smoke = null;
            string model = null;
            foreach (var g in geoms) { var n = g.Mesh?.ResourceName ?? ""; }
            bool large = node.Name.ToString() == "Muhtar";
            bool houseA = !large && geoms.Count > 0 && (geoms[0].Mesh == Models.Mesh("human_house", "human_house_a_hi"));
            Vector3? top = large ? PropKit.Point("buildings", "human_house_large", "chimney_top") : houseA ? new Vector3(1.2f, 8.63f, -1.3f) : null;
            if (top.HasValue)
            {
                smoke = Fx.Smoke(this, PropKit.ToWorld(top.Value, node.Position, node.Rotation.Y), 0.55f);
                smoke.Emitting = false;
            }
            _homes[pl.Id] = (geoms.ToArray(), smoke);
            _ = model;
        }
    }

    public override void _Process(double delta)
    {
        float dt = (float)Math.Min(delta, 0.1);
        DebugRealDt = dt;
        var player = _region.Player;
        if (player != null)
        {
            var pp = player.GlobalPosition;
            Sim.PlayerPos = new System.Numerics.Vector2(pp.X, pp.Z);
            Sim.PlayerPresent = player.InputEnabled || FD.Dev.Dev.Instance?.CamPos == null;
            Sim.PlayerSprinting = player.Velocity.Length() > 5.5f;
        }
        _sw.Restart();
        Sim.Update(dt, GameClock.TotalHours * 60.0);
        SimMs = SimMs * 0.9 + _sw.Elapsed.TotalMilliseconds * 0.1;
        _sw.Restart();

        var cam = GetViewport().GetCamera3D();
        Vector3 camPos = cam?.GlobalPosition ?? Vector3.Zero;
        Vector3 playerPos = player?.GlobalPosition ?? camPos;
        var space = GetWorld3D().DirectSpaceState;
        var hf = _region.Heightfield;
        foreach (var a in Actors)
        {
            var p = a.P;
            float d = new Vector2(p.Pos.X - camPos.X, p.Pos.Y - camPos.Z).Length();
            if (d > 240f && p.Motion != Motion.Inside) { a.Visible = false; continue; }
            a.Sync(dt, hf, space, camPos, Sim, playerPos);
        }
        SyncMs = SyncMs * 0.9 + _sw.Elapsed.TotalMilliseconds * 0.1;
        _sw.Restart();
        SyncSheep(dt, hf);
        SyncCart(dt, hf);
        _windowTimer -= dt;
        if (_windowTimer <= 0) { _windowTimer = 1f; UpdateHomes(); }
        OtherMs = OtherMs * 0.9 + _sw.Elapsed.TotalMilliseconds * 0.1;
    }

    void SyncSheep(float dt, Heightfield hf)
    {
        var f = Sim.Flock;
        for (int i = 0; i < _sheep.Count && i < f.Pos.Count; i++)
        {
            var mi = _sheep[i];
            var p = f.Pos[i];
            float bob = f.Speed[i] > 0.1f ? MathF.Abs(MathF.Sin((float)Time.GetTicksMsec() * 0.012f + i)) * 0.05f : 0f;
            mi.Position = new Vector3(p.X, hf.Height(p.X, p.Y) + bob, p.Y);
            float yaw = MathF.Atan2(f.Dir[i].X, f.Dir[i].Y);
            // grazing sheep dip their heads now and then
            float graze = f.Speed[i] < 0.1f && H.Hash(i, (int)(Time.GetTicksMsec() / 2500)) < 0.5f ? 0.12f : 0f;
            mi.Rotation = new Vector3(graze, yaw, 0);
        }
    }

    void SyncCart(float dt, Heightfield hf)
    {
        if (_cart == null) return;
        Person m = null;
        foreach (var p in Sim.People) if (p.HasCart) { m = p; break; }
        if (m == null || !m.Present || (m.Act?.Kind == ActKind.Travel && m.Motion == Motion.Inside)) { _cart.Visible = false; return; }
        _cart.Visible = true;
        if (m.Motion == Motion.Walking)
        {
            var dir = new Vector2(m.Dir.X, m.Dir.Y);
            float want = MathF.Atan2(dir.X, dir.Y);
            _cartYaw = Mathf.LerpAngle(_cartYaw, want, 1f - MathF.Exp(-dt * 2.5f));
            var drv = PropKit.Point("village", "cart", "driver_walk") ?? new Vector3(-1.05f, 0, 1.4f);
            var off = drv.Rotated(Vector3.Up, _cartYaw);
            float x = m.Pos.X - off.X, z = m.Pos.Y - off.Z;
            float y = hf.Height(x, z);
            _cart.Position = new Vector3(x, y, z);
            // pitch along the slope
            var fwd = new Vector3(MathF.Sin(_cartYaw), 0, MathF.Cos(_cartYaw));
            float yf = hf.Height(x + fwd.X * 1.5f, z + fwd.Z * 1.5f), yb = hf.Height(x - fwd.X * 1.5f, z - fwd.Z * 1.5f);
            _cart.Rotation = new Vector3(-MathF.Atan2(yf - yb, 3f), _cartYaw, 0);
            float ang = m.CartDist / 0.6f;
            _wheelL.Rotation = new Vector3(ang, 0, 0);
            _wheelR.Rotation = new Vector3(ang, 0, 0);
            _ox.Position = (PropKit.Point("village", "cart", "hitch_point") ?? new Vector3(0, 0, 2.45f)) + new Vector3(0, MathF.Abs(MathF.Sin(m.CartDist * 2.4f)) * 0.04f, 0);
        }
    }

    void UpdateHomes()
    {
        float h = GameClock.Hour;
        foreach (var (placeId, (geoms, smoke)) in _homes)
        {
            int inside = 0, awake = 0;
            foreach (var p in Sim.People)
            {
                if (p.Home != placeId || p.Motion != Motion.Inside) continue;
                inside++;
                if (p.Act?.Kind != ActKind.Sleep) awake++;
            }
            float lit = awake > 0 ? 1f : inside > 0 ? (h > 23f || h < 4.5f ? 0f : 0.25f) : 0f;
            foreach (var g in geoms) g.SetInstanceShaderParameter("lit", lit);
            if (smoke != null) smoke.Emitting = awake > 0 && ((h > 5.5f && h < 9f) || (h > 11f && h < 13f) || (h > 17f && h < 21.5f));
        }
    }

    // ------------------------------------------------------------------------------------------------ player interaction

    public PersonActor ActorOf(Person p) => p != null && p.Id < Actors.Count ? Actors[p.Id] : null;

    /// <summary>Nearest person in front of the player within reach (for the E prompt).</summary>
    public Person Facing(Vector3 playerPos, Vector3 forward, float reach = 3.4f)
    {
        Person best = null; float bs = float.MaxValue;
        foreach (var a in Actors)
        {
            if (!a.Visible || !a.P.Visible) continue;
            Vector3 d = a.GlobalPosition - playerPos; d.Y = 0;
            float L = d.Length();
            if (L > reach) continue;
            float dot = L > 0.01f ? d.Normalized().Dot(new Vector3(forward.X, 0, forward.Z).Normalized()) : 1f;
            if (dot < 0.25f) continue;
            float score = L * (1.6f - dot);
            if (score < bs) { bs = score; best = a.P; }
        }
        return best;
    }

    /// <summary>The hero greets someone: they pause, turn and answer in character.</summary>
    public void Greet(Person p)
    {
        var a = ActorOf(p);
        if (a == null) return;
        if (p.Motion == Motion.Walking) p.Wait = 2.2f;
        if (Sim.Now - a.LastGreet < 0.5) return;
        a.LastGreet = Sim.Now;
        a.Say(Lines.Greeting(p, Sim));
    }
}

/// <summary>What people say when greeted (by role, time and what they are doing).</summary>
public static class Lines
{
    public static string Greeting(Person p, LifeSim sim)
    {
        float h = (float)(sim.Now % 1440) / 60f;
        int k = (int)(H.Hash(p.Id, (int)(sim.Now / 7)) * 3);
        string tod = h < 11 ? "Hayırlı sabahlar" : h < 17 ? "Hayırlı günler" : "Hayırlı akşamlar";
        if (p.Role == Role.Goblin) return new[] { "Grrah! Defol!", "Parlak şeylerini ver!", "Snik seni gördü!" }[k];
        if (p.Role == Role.Child) return new[] { "Sen şövalye misin?", "Kılıcın var mı? Göster!", "Annem yabancılarla konuşma dedi!" }[k];
        if (p.Act?.Kind == ActKind.Flee) return "Goblinler! Kaç!";
        return p.Role switch
        {
            Role.Farmer => new[] { $"{tod}, yolcu. Toprak bu yıl cömert.", "Yağmur yağarsa hasat iyi olur.", "Kuzeydeki goblinler geçen ay iki koyun çaldı." }[k],
            Role.Homemaker => new[] { $"{tod}! Kuyunun suyu tatlıdır, iç istersen.", "Pazar tezgâhına tüccar gelecekmiş.", "Çocuklar yine meydanda koşturuyor." }[k],
            Role.Woodcutter => new[] { "Ormana yalnız girme, goblin izi var.", "Meşe sert ama ateşi uzun yanar.", $"{tod}. Baltam kesmez olmuş, demirciye götürmeli." }[k],
            Role.Smith or Role.Apprentice => new[] { "Nal mı lazım, kılıç mı?", "Demir sıcakken dövülür!", $"{tod}. Ocak bugün iyi yanıyor." }[k],
            Role.Shepherd => new[] { "Koyunlarım goblinlerden korkuyor.", $"{tod}. Mera bu yıl gür.", "Gece ağılı kapatmayı unutmam." }[k],
            Role.Headman => new[] { $"{sim.VillageName}'ye hoş geldin, yabancı.", "Goblin kampı başımıza dert. Ödül koyabiliriz.", "Köyümüz küçük ama kalbi büyük." }[k],
            Role.Priest => new[] { "Işık yolunu aydınlatsın.", "Tapınağın kapısı herkese açık.", "Eski gözcü kulesinde tuhaf ışıklar görülmüş..." }[k],
            Role.Innkeeper => new[] { $"{sim.InnName}'a hoş geldin! Bira taze.", "Oda da var, sıcak yemek de.", "Maceracılar goblin kampını soruyor hep." }[k],
            Role.InnServant or Role.StableHand => new[] { "Hemen geliyorum!", "Atını ahıra alayım mı?", $"{tod}!" }[k],
            Role.Elder => new[] { "Benim gençliğimde kulede nöbet tutulurdu.", $"{tod}, evlat.", "Dereye yakın durma, taşar bazen." }[k],
            Role.Merchant => new[] { "Tuz, kumaş, baharat! Uygun fiyata!", "Doğudan geldim, yollar tekin değil.", "Pazarda görüşürüz." }[k],
            Role.Pilgrim => new[] { "Işık seninle olsun.", "Tapınaktan tapınağa yürüyorum.", "Yolun açık olsun." }[k],
            Role.Adventurer => new[] { "Goblin kampı patikanın sonunda.", "Birlikte gidelim mi? Şaka... şimdilik.", "Kılıcını bileyle, lazım olacak." }[k],
            _ => $"{tod}.",
        };
    }
}
