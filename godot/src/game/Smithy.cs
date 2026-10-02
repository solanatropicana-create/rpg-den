using System;
using System.Collections.Generic;
using System.Linq;
using FD.Rpg;
using M = FD.Macro;

namespace FD.Game;

/// <summary>
/// Tur 1 E: the smith's own civilian workshop, apart from the realm's armoury. Each day he buys iron and wood for charcoal from the
/// realm's stockpile (paying its treasury — the macro world's own smiths burn charcoal too; there is no coal good in the world) and
/// forges what villagers and travellers need: knives (hançer), hand axes, spears, now and then a sword. No iron or no charcoal: the
/// forge goes cold and nothing new is made. What he forged waits in his workshop (kept in the save) until someone buys it; the
/// martial weapons and mail the realm keeps for its soldiers are a different shelf (the shop's armoury part).
/// </summary>
public static class Smithy
{
    /// <summary>at most this many pieces wait for a buyer</summary>
    public const int Shelf = 6;
    const string Iron = "smithIron", Coal = "smithCoal", Last = "smithDay", Made = "smithMade", Cold = "smithCold";

    public static double IronLeft(Session s) => s.Flags.GetValueOrDefault(Iron);
    public static double CoalLeft(Session s) => s.Flags.GetValueOrDefault(Coal);
    /// <summary>why the forge is cold today (null: it burns)</summary>
    public static string ColdReason(Session s) => s.Flags.GetValueOrDefault(Cold) switch { 1 => "demir", 2 => "kömür", 3 => "demir ve kömür", _ => null };
    public static string LastMade(Session s) => s.Flags.TryGetValue(Made, out var m) ? MadeNames[(int)m % MadeNames.Length] : null;
    static readonly string[] MadeNames = { "dagger", "handaxe", "spear", "shortsword", "longsword" };

    /// <summary>A day at the forge (the director calls it every new day). Returns what was made (item ids).</summary>
    public static System.Collections.Generic.List<string> Day(Session s, int? dayOverride = null)
    {
        var made = new System.Collections.Generic.List<string>();
        int day = dayOverride ?? GameDay(s);
        if (s.Flags.GetValueOrDefault(Last, -1) >= day) return made;
        s.Flags[Last] = day;
        // buy what is short from the realm: two bars of iron, wood for two loads of charcoal (2 wood → 1 charcoal)
        double inRealm = Math.Floor(Math.Min(2, M.Local.Stock(s.Macro, "iron")));
        if (IronLeft(s) < 2 && inRealm >= 1) { Take(s, "iron", inRealm); s.Flags[Iron] = IronLeft(s) + inRealm; }
        if (CoalLeft(s) < 2 && M.Local.Stock(s.Macro, "wood") >= 6) { Take(s, "wood", 4); s.Flags[Coal] = CoalLeft(s) + 2; }
        int cold = (IronLeft(s) < 1 ? 1 : 0) + (CoalLeft(s) < 1 ? 2 : 0);
        s.Flags[Cold] = cold;
        if (cold != 0) return made;
        var rng = new M.Rng(s.Seed * 4099 + day * 31 + 7);
        for (int k = 0; k < 2; k++)
        {
            if (s.SmithStock.Items.Sum(i => i.Count) >= Shelf) break;
            if (k == 1 && !rng.Chance(0.45)) break;
            double r = rng.Next();
            string id = r < 0.36 ? "dagger" : r < 0.62 ? "handaxe" : r < 0.9 ? "spear" : rng.Chance(0.6) ? "shortsword" : "longsword";
            int need = id is "shortsword" or "longsword" ? 2 : 1;
            if (IronLeft(s) < need || CoalLeft(s) < need) { if (need == 2) id = "dagger"; need = 1; }
            if (IronLeft(s) < need || CoalLeft(s) < need) break;
            s.Flags[Iron] = IronLeft(s) - need; s.Flags[Coal] = CoalLeft(s) - need;
            s.SmithStock.Add(id);
            s.Flags[Made] = Array.IndexOf(MadeNames, id);
            made.Add(id);
        }
        return made;
    }

    /// <summary>A new game: the shelf as the last few days left it.</summary>
    public static void Warm(Session s)
    {
        if (s.Flags.ContainsKey(Last)) return;
        // what was left on the shelf from before (a knife, an axe, a spear), then the last few days at the forge
        foreach (var id in new[] { "dagger", "handaxe", "spear" }) s.SmithStock.Add(id);
        int d = GameDay(s);
        for (int k = 3; k >= 0; k--) Day(s, d - k);
    }

    static int GameDay(Session s) => (int)Math.Floor(FD.World.GameClock.TotalHours / 24.0);

    /// <summary>the smith pays the realm for its iron/wood (the treasury gets the money, the stockpile loses the goods)</summary>
    static void Take(Session s, string good, double units)
    {
        double gold = units * M.Local.Price(s.Macro, good);
        M.Local.Trade(s.Macro, good, units, gold);
    }
}
