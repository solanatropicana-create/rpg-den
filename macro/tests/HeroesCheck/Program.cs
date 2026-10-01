using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using FD.Macro;

namespace HeroesCheck;

/// <summary>
/// Heroes / Will / Inns port check (C# twin of golden/ts/heroes_sim.ts): runs Sim.Step() and writes one JSON line
/// per day: rng state, nextId, the day's new events and battles (+ every &lt;every&gt; days a snapshot of heroes,
/// inns, quests, agents, camps, civs, settlements, routes, metrics). Args: seed days [every] [scen].
/// Mode "unit &lt;seed&gt;": the Heroes/Will/Inns unit check (Unit.cs, twin of golden/ts/heroes_unit.ts).
/// </summary>
public static class Program
{
    public static int Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "unit")
        {
            using var uo = new StreamWriter(Console.OpenStandardOutput(), new UTF8Encoding(false));
            return Unit.Run(args.Length > 1 ? double.Parse(args[1], CultureInfo.InvariantCulture) : 1, uo);
        }
        double seed = args.Length > 0 ? double.Parse(args[0], CultureInfo.InvariantCulture) : 1;
        int days = args.Length > 1 ? int.Parse(args[1], CultureInfo.InvariantCulture) : 1000;
        int every = args.Length > 2 ? int.Parse(args[2], CultureInfo.InvariantCulture) : 25;
        int scen = args.Length > 3 ? int.Parse(args[3], CultureInfo.InvariantCulture) : 0;
        var sim = new Sim(seed);
        var w = sim.W;
        using var o = new StreamWriter(Console.OpenStandardOutput(), new UTF8Encoding(false));
        if (scen == 1 || scen == 2) Inject(sim, scen);
        int lastEv = -1, lastBattle = -1;
        void Dump(bool full)
        {
            var events = J.Filter(w.Events, e => e.Id > lastEv);
            foreach (var e in events) lastEv = Math.Max(lastEv, e.Id);
            var battles = J.Filter(w.Battles, b => b.Id > lastBattle);
            foreach (var b in battles) lastBattle = Math.Max(lastBattle, b.Id);
            var line = new Dictionary<string, object> { ["day"] = w.Day, ["rng"] = sim.Rng.State(), ["nextId"] = w.NextId, ["events"] = events, ["battles"] = battles };
            if (full)
            {
                line["heroes"] = w.Heroes; line["inns"] = w.Inns; line["quests"] = w.Quests; line["agents"] = w.Agents; line["camps"] = w.Camps;
                line["civs"] = w.Civs; line["settlements"] = w.Settlements; line["routes"] = w.Routes; line["metrics"] = w.Metrics; line["innPlan"] = w.InnPlan;
            }
            o.Write(Json.Serialize(line));
            o.Write('\n');
        }
        Dump(true);
        for (int d = 1; d <= days; d++)
        {
            if (scen == 2) InjectDaily(sim, d);
            sim.Step();
            Dump(d % every == 0);
        }
        return 0;
    }

    private static void Inject(Sim sim, int scen)
    {
        var w = sim.W;
        if (scen == 2) w.Civs[0].Align.Good = -0.8;
        foreach (var a in w.Civs) foreach (var b in w.Civs) if (a.Id != b.Id) w.Relations[a.Id][b.Id].Contact = true;
        for (int i = 0; i < w.Settlements.Count; i++)
        {
            var st = w.Settlements[i];
            st.Civics.Set("tavern", 1);
            if (i % 3 == 0) st.Civics.Set("temple", 1);
            if (i % 4 == 1) st.Civics.Set("library", 1);
        }
        for (int i = 0; i < w.Civs.Count; i++)
        {
            var c = w.Civs[i];
            sim.Add(c, "gold", 400); sim.Add(c, "grain", 300); sim.Add(c, "potion", 6);
            if (i % 2 == 0) c.Threat = 0.5;
        }
        if (w.Settlements.Count > 1) w.Settlements[1].Plague = new PlagueInfo { Since = 0, Until = 400, Severity = 0.6, Dead = 0 };
    }

    /// <summary>scen 2 daily injections (same order as the TS twin): revive effect, XP boosts, onHomeBurned.</summary>
    private static void InjectDaily(Sim sim, int d)
    {
        var w = sim.W;
        foreach (var c in w.Civs) c.Eff.Set("revive", 1);
        if (d % 100 == 50) foreach (var h in w.Heroes) if (h.State != "dead" && h.State != "gone" && h.State != "retired" && h.Level < 5) Heroes.GainXp(sim, h, 1200);
        if (d % 150 == 75)
        {
            var alive = J.Filter(w.Settlements, x => x.Alive);
            if (alive.Count > 0)
            {
                var st = alive[(d / 75) % alive.Count];
                var cp = J.Find(w.Camps, c => c.Alive);
                Will.OnHomeBurned(sim, st, cp, w.Civs[(st.Civ + 1) % w.Civs.Count]);
            }
        }
    }
}
