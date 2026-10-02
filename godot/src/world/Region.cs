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

        Heightfield = new Heightfield();
        Step("heightfield");
        // life simulation: the path network starts with the road and the forest trail; features add lanes/places
        Life = new LifeSim((ulong)RegionSpec.Seed);
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

        // people
        Census.Populate(Life, (ulong)RegionSpec.Seed + 11);
        Life.FinishLayout();
        LifeWorld = new LifeWorld();
        AddChild(LifeWorld);
        LifeWorld.Init(this, Life);
        Hud = new Hud { Name = "Hud" };
        AddChild(Hud);
        Hud.Init(this, LifeWorld);
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
