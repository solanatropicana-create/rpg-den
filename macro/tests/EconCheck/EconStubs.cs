using System;
using System.Collections.Generic;

// Harness-only stand-ins for the modules that EconCheck does not compile (their real files may be mid-edit).
// Sim.cs references all of them; the economy harness never calls the throwing ones.
// Sea.PickPort / Fleet / HullName are called by Economy, so they are faithful copies of src/sim/sea.ts.

namespace FD.Macro;

public static class Sea
{
    private static readonly double[] PORT_D = { 9, 2, 0, 1, 1.5, 2 };

    public static int PickPort(Sim s, Settlement st)
    {
        var w = s.W;
        int best = -1; double bd = double.PositiveInfinity;
        foreach (int i in s.G.Within(st.Tile, s.RadiusOf(st)))
        {
            var t = w.Tiles[i];
            if (J.T(t.Sea) || t.Terrain == "water" || t.Terrain == "mountain" || (t.Owner != st.Id && i != st.Tile)) continue;
            if (!J.Some(s.G.Neighbors(i), n => J.T(w.Tiles[n].Sea))) continue;
            int dd = s.G.Dist(i, st.Tile);
            double d = (dd < PORT_D.Length ? PORT_D[dd] : double.NaN) + (t.Ext != null ? 1.5 : 0) + (t.Deposit >= 0 ? 1 : 0);
            if (d < bd) { bd = d; best = i; }
        }
        return best;
    }

    public static (double Ships, double Galleys) Fleet(Sim s, Civ c)
    {
        double ships = 0, galleys = 0;
        foreach (var x in s.CivSettlements(c)) { ships += x.Ships ?? 0; galleys += x.Galleys ?? 0; }
        return (ships, galleys);
    }

    public static string HullName(Sim s, Civ c, bool plural = false) => s.Has(c, "shipbuilding") ? (plural ? "kogalar" : "koga") : (plural ? "tekneler" : "tekne");

    public static void SeaTick(Sim s) => throw new NotImplementedException("Sea.SeaTick (harness stub)");
}

public static class Diplomacy
{
    public static void RelationsTick(Sim s) => throw new NotImplementedException("harness stub");
    public static void WorldTick(Sim s) => throw new NotImplementedException("harness stub");
    public static void ConsiderExpansion(Sim s, Civ c) => throw new NotImplementedException("harness stub");
    public static void ConsiderScout(Sim s, Civ c) => throw new NotImplementedException("harness stub");
    public static void ConsiderTrade(Sim s, Civ c) => throw new NotImplementedException("harness stub");
    public static void ConsiderWarAction(Sim s, Civ c) => throw new NotImplementedException("harness stub");
    public static void ConsiderRaid(Sim s, Civ c) => throw new NotImplementedException("harness stub");
}

public static class Monsters
{
    public static void CampsTick(Sim s) => throw new NotImplementedException("harness stub");
}

public static class Heroes
{
    public static void TavernsTick(Sim s) => throw new NotImplementedException("harness stub");
    public static void HeroesTick(Sim s) => throw new NotImplementedException("harness stub");
    public static void ConsiderHero(Sim s, Civ c) => throw new NotImplementedException("harness stub");
    public static void ConsiderQuest(Sim s, Civ c) => throw new NotImplementedException("harness stub");
    public static void GainXp(Sim s, Hero h, double xp) => throw new NotImplementedException("Heroes.GainXp (harness stub)");
}

public static class Inns
{
    public static void InnsTick(Sim s) => throw new NotImplementedException("harness stub");
    public static void ConsiderInnBids(Sim s, Civ c) => throw new NotImplementedException("harness stub");
}

public static class Will
{
    public static void ReturnToBase(Sim s, Hero h) => throw new NotImplementedException("Will.ReturnToBase (harness stub)");
    public static string HeroLabel(Hero h) => throw new NotImplementedException("Will.HeroLabel (harness stub)");
}

public static class Agents
{
    public static void AgentsTick(Sim s) => throw new NotImplementedException("harness stub");
    public static void RoutesTick(Sim s) => throw new NotImplementedException("harness stub");
    public static void RoadsTick(Sim s) => throw new NotImplementedException("harness stub");
}
