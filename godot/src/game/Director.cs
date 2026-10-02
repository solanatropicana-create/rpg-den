using System;
using Godot;
using FD.World;

namespace FD.Game;

/// <summary>
/// Faz 2: keeps the macro world in step with the region's clock (one macro day per local day, see
/// <see cref="Session.SyncDay"/>) and tells the rest of the game when a new macro day has dawned.
/// </summary>
public partial class Director : Node
{
    public static Director Instance { get; private set; }
    public Session Session { get; private set; }
    /// <summary>raised on the main thread after the macro world advanced (arg: macro day)</summary>
    public event Action<int> NewDay;
    public double LastStepMs { get; private set; }

    public void Init(Session s)
    {
        Instance = this;
        Name = "Director";
        Session = s;
        s.MacroDayStepped += d => NewDay?.Invoke(d);
    }

    public override void _ExitTree() { if (Instance == this) Instance = null; }

    public override void _Process(double delta)
    {
        if (Session == null) return;
        var sw = System.Diagnostics.Stopwatch.StartNew();
        int n = Session.SyncDay(GameClock.Day);
        if (n > 0)
        {
            LastStepMs = sw.Elapsed.TotalMilliseconds;
            GD.Print($"[Director] makro gün {Session.Macro.W.Day} ({n} adım, {LastStepMs:F0} ms)");
        }
    }
}

/// <summary>Where a session comes from when the Region scene is opened directly (dev, tests): a fresh world from the
/// dev seed (--seed=N, default 1) with --prehistory=N days (default full).</summary>
public static class Bootstrap
{
    public static Session DevSession()
    {
        var dev = FD.Dev.Dev.Instance;
        double seed = dev?.Seed ?? 1;
        int pre = dev?.Prehistory ?? FD.Macro.Sim.PREHISTORY_DAYS;
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var s = Session.NewWorld(seed, null, pre);
        s.BaseLocalDay = GameClock.Day;
        GD.Print($"[Bootstrap] dünya {sw.ElapsedMilliseconds} ms: {s.Describe()}");
        return s;
    }
}
