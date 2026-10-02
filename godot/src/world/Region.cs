using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using FD.Actors;
using FD.Life;
using FD.Sim.Life;
using FD.UI;

namespace FD.World;

/// <summary>
/// A feature that shapes terrain before <see cref="Heightfield.Bake"/> and builds its nodes afterwards
/// (village builder, inn, goblin camp, …). Add instances in <see cref="Region.CreateFeatures"/>.
/// </summary>
public interface IRegionFeature
{
    /// <summary>Register pads, lanes and vegetation exclusions (terrain is not built yet).</summary>
    void Shape(Heightfield hf);
    /// <summary>Create nodes; terrain, water, vegetation and the day-night cycle exist.</summary>
    void Build(Region region);
}

/// <summary>
/// Main scene root (scenes/Region.tscn): builds the whole slice in a fixed order —
/// heightfield → features shape it → bake → sky/lights → terrain → water → vegetation → grass → features build →
/// player. <see cref="Current"/> gives other systems access to the shared pieces.
/// </summary>
public partial class Region : Node3D
{
    public static Region Current { get; private set; }
    /// <summary>Raised once everything (including the player) exists.</summary>
    public static event Action<Region> Built;

    public Heightfield Heightfield { get; private set; }
    public DayNight DayNight { get; private set; }
    public Terrain Terrain { get; private set; }
    public Water Water { get; private set; }
    public Vegetation Vegetation { get; private set; }
    public GrassField Grass { get; private set; }
    public Player Player { get; private set; }
    /// <summary>Life simulation of the region (people, places, paths). Features register places while shaping.</summary>
    public LifeSim Life { get; private set; }
    public LifeWorld LifeWorld { get; private set; }
    public Hud Hud { get; private set; }
    public VillageSite Village { get; private set; }
    /// <summary>Faz 2: the macro world and its link to this region (village, inn, camp)</summary>
    public FD.Game.Session Session { get; private set; }
    public FD.Game.Director Director { get; private set; }
    /// <summary>Faz 2: the bodies of the party members other than the player (following the leader; the fight drives them)</summary>
    public readonly List<Companion> Companions = new();
    public FD.Combat.CombatDirector Combat { get; private set; }
    public FD.Game.Captivity Captivity { get; private set; }
    public FD.Game.PartyManager Party { get; private set; }
    public FD.Game.Gathering Gathering { get; private set; }
    public FD.Game.Services Services { get; private set; }
    public InventoryPanel Inventory { get; private set; }
    public TradePanel Trade { get; private set; }

    /// <summary>E: a body for a new party member at (x, z), following the controlled one.</summary>
    public Companion AddCompanion(FD.Rpg.Character c, Vector2 at)
    {
        int slot = 0;
        while (Companions.Exists(x => x.Slot == slot && !x.Char.Dead)) slot++;
        var comp = Companion.Create(c, slot);
        AddChild(comp);
        comp.Init(Player, Heightfield);
        comp.GlobalPosition = new Vector3(at.X, Heightfield.Height(at.X, at.Y), at.Y);
        Companions.Add(comp);
        return comp;
    }
    public readonly List<IRegionFeature> Features = new();
    public bool IsReady { get; private set; }

    /// <summary>The list of region features. The ScaleProbe is TEMPORARY (see ScaleProbe.cs) — replace it with the
    /// real village builder.</summary>
    protected virtual void CreateFeatures()
    {
        Features.Add(new RoadBridge());
        Village = new VillageSite();
        Features.Add(Village);
        Features.Add(new OutskirtsSite(Village));
        Features.Add(new InnSite());
        Features.Add(new CampSite());
        Features.Add(new RuinSite());
    }

    public override void _Ready()
    {
        Current = this;
        var total = Stopwatch.StartNew();
        var sw = Stopwatch.StartNew();
        void Step(string what) { GD.Print($"[Region] {what}: {sw.ElapsedMilliseconds} ms"); sw.Restart(); }

        Session = FD.Game.Session.Current ??= FD.Game.Bootstrap.DevSession();
        Step("session");
        Heightfield = new Heightfield();
        Step("heightfield");
        // life simulation: the path network starts with the road and the forest trail; features add lanes/places
        Life = new LifeSim((ulong)RegionSpec.Seed);
        // Faz 2: names from the macro world (village, inn, camp)
        (Life.VillageName, Life.InnName, Life.CampName) = Session.Names();
        Life.Graph.AddPolyline(ToSim(RegionSpec.Road.Points), 6f, 2.0f);
        Life.Graph.AddPolyline(ToSim(RegionSpec.Trail.Points), 5f, 1.2f);
        Life.Graph.AddPolyline(ToSim(RegionSpec.CampSpur.Points), 4f, 0.8f);
        CreateFeatures();
        foreach (var f in Features) f.Shape(Heightfield);
        Heightfield.Bake();
        Step("features+bake");

        DayNight = new DayNight();
        AddChild(DayNight);
        bool headless = DisplayServer.GetName() == "headless";
        Terrain = new Terrain();
        AddChild(Terrain);
        Terrain.Build(Heightfield);
        Step("terrain");
        Water = new Water();
        AddChild(Water);
        Water.Build(Heightfield);
        Vegetation = new Vegetation();
        AddChild(Vegetation);
        Vegetation.Build(Heightfield, collision: true);
        Step("water+vegetation");
        if (!headless)
        {
            Grass = new GrassField();
            AddChild(Grass);
            Grass.Init(Heightfield, Vegetation);
        }
        foreach (var f in Features) f.Build(this);
        Step("features");

        Player = GD.Load<PackedScene>("res://scenes/Player.tscn").Instantiate<Player>();
        AddChild(Player);
        Player.Teleport(RegionSpec.PlayerStart, RegionSpec.YawFacing(RegionSpec.PlayerStartFacing), Heightfield);
        // Faz 2: the player's character (creation screen, save, or a dev default)
        if (Session.Player == null)
        {
            var dev = FD.Rpg.CharacterFactory.Player("Deneme Yolcu", "human", "fighter", "neutral", FD.Rpg.Rules.SuggestedBase("fighter"), new FD.Rpg.Look(), null, 10);
            Session.AddPlayer(dev);
        }
        Player.SetCharacter(Session.Controlled ?? Session.Player);
        var pending = FD.Game.SaveGame.Pending;
        if (pending != null && pending.HasPosition)
            Player.Teleport(new Vector2(pending.PlayerX, pending.PlayerZ), pending.PlayerYaw, Heightfield);
        FD.Game.SaveGame.Pending = null;
        // dev: a party to try fights with (--party=wizard,cleric)
        if (FD.Dev.Dev.Instance?.PartySpec is string ps && Session.Party.Count <= 1)
            foreach (var cls in ps.Split(',', StringSplitOptions.RemoveEmptyEntries)) Session.Party.Add(FD.Rpg.CharacterFactory.DevCompanion(cls.Trim(), Session.NextCharId()));
        int slot = 0;
        foreach (var c in Session.Party)
        {
            if (c == (Session.Controlled ?? Session.Player) || c.Dead) continue;
            var comp = Companion.Create(c, slot++);
            AddChild(comp);
            comp.Init(Player, Heightfield);
            comp.SnapToLeader();
            Companions.Add(comp);
        }

        // people
        Census.Populate(Life, (ulong)RegionSpec.Seed + 11, FD.Game.RegionBind.Spec(Session));
        Life.FinishLayout();
        LifeWorld = new LifeWorld();
        AddChild(LifeWorld);
        LifeWorld.Init(this, Life);
        Hud = new Hud { Name = "Hud" };
        AddChild(Hud);
        Hud.Init(this, LifeWorld);
        Director = new FD.Game.Director();
        AddChild(Director);
        Director.Init(Session);
        Combat = new FD.Combat.CombatDirector();
        AddChild(Combat);
        Combat.Init(this);
        Captivity = new FD.Game.Captivity();
        AddChild(Captivity);
        Captivity.Init(this);
        if (Player.Character.Captive) Captivity.Cage();
        foreach (var comp in Companions) comp.Hold = Player.Character.Captive;
        Party = new FD.Game.PartyManager();
        AddChild(Party);
        Party.Init(this);
        Gathering = new FD.Game.Gathering();
        AddChild(Gathering);
        Gathering.Init(this);
        Inventory = new InventoryPanel();
        AddChild(Inventory);
        Inventory.Init(this);
        Trade = new TradePanel();
        AddChild(Trade);
        Trade.Init(this);
        Services = new FD.Game.Services();
        AddChild(Services);
        Services.Init(this);
        Step("life");
        IsReady = true;
        GD.Print($"[Region] ready in {total.ElapsedMilliseconds} ms");
        Built?.Invoke(this);
    }

    static System.Collections.Generic.List<System.Numerics.Vector2> ToSim(System.Collections.Generic.IReadOnlyList<Vector2> pts)
    {
        var r = new System.Collections.Generic.List<System.Numerics.Vector2>(pts.Count);
        foreach (var p in pts) r.Add(new System.Numerics.Vector2(p.X, p.Y));
        return r;
    }

    public override void _ExitTree()
    {
        if (Current == this) Current = null;
    }
}
