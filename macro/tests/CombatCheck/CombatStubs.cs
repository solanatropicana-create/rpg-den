using System;
using System.Collections.Generic;

// Harness-only stand-ins so Modules/Monsters.cs compiles without Core/Sim.cs and the other modules (which may be
// mid-edit). CombatCheck only calls Monsters.MonsterSide, which touches none of these.

namespace FD.Macro;

public sealed class Sim
{
    public const double FOOD_PER_POP = 0.1;
    public const int YEAR = 120;
    public World W;
    public HexGrid G;
    public Rng Rng;
    public int Day => W.Day;
    public int Year => W.Day / YEAR + 1;
    public int Id() => W.NextId++;
    public GameEvent Log(string kind, string text, string cause = null, int? civ = null, int? tile = null, int? battle = null, bool? major = null) => throw new NotImplementedException("harness stub");
    public void Metric(string k, double v = 1) => throw new NotImplementedException("harness stub");
    public List<int> Path(int from, int to) => throw new NotImplementedException("harness stub");
    public double Pop(Settlement s) => throw new NotImplementedException("harness stub");
    public bool ExtWorking(Tile t) => throw new NotImplementedException("harness stub");
    public Settlement Settlement(int id) => throw new NotImplementedException("harness stub");
    public double E(Civ c, string k) => throw new NotImplementedException("harness stub");
}

public static class Sea
{
    public static string NewCaptain(Sim s) => throw new NotImplementedException("harness stub");
    public static void LaunchPirates(Sim s, Camp cp) => throw new NotImplementedException("harness stub");
}
