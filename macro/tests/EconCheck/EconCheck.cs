using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using FD.Macro;

// C# twin of golden/ts/econ_diff.ts: same tick sequence, same per-day JSON layout (see EconCheck.csproj).
namespace EconCheck;

public static class Program
{
    public static int Main(string[] args)
    {
        double seed = double.Parse(args.Length > 0 ? args[0] : "1", CultureInfo.InvariantCulture);
        int days = int.Parse(args.Length > 1 ? args[1] : "300", CultureInfo.InvariantCulture);
        int scen = int.Parse(args.Length > 2 ? args[2] : "0", CultureInfo.InvariantCulture);
        int tilesEvery = int.Parse(args.Length > 3 ? args[3] : "10", CultureInfo.InvariantCulture);
        D.Init();
        var o = new StreamWriter(Console.OpenStandardOutput(), new UTF8Encoding(false), 1 << 16);
        Sim sim = null;
        try
        {
            sim = new Sim(seed);
            var w = sim.W;
            int lastEv = -1;
            (string, double)[] bonus = { ("gold", 300), ("iron", 60), ("copper", 40), ("tin", 25), ("leather", 40), ("wood", 150), ("stone", 150), ("bricks", 40), ("tools", 12), ("mana", 12), ("mithril", 6), ("herbs", 25), ("potion", 12), ("salt", 6), ("horses", 6), ("arms", 12) };
            void Inject(int day)
            {
                if (scen == 0) return;
                if (day == 0)
                {
                    foreach (var a in w.Civs) foreach (var b in w.Civs) if (a.Id != b.Id) w.Relations[a.Id][b.Id].Contact = true;
                    var war = new War { Since = 0, Attacker = 0, Target = 1, Attacks = 0, LastArmy = 0, Goal = "land" };
                    w.Relations[0][1].War = war; w.Relations[1][0].War = war;
                    foreach (var c in w.Civs)
                    {
                        if (c.Id % 2 == 0) c.Threat = 0.8;
                        foreach (var (g, v) in bonus) sim.Add(c, g, v);
                        var cap = sim.Capital(c); if (cap != null) sim.AddPop(cap, c.Race, 30);
                    }
                }
                if (day % 400 == 200) { var c = w.Civs[(day / 400) % w.Civs.Count]; foreach (var g in new[] { "bread", "fish", "meat", "grain" }) c.Stock.Set(g, 0); }
            }
            void Dump(bool full)
            {
                var events = J.Filter(w.Events, e => e.Id > lastEv);
                foreach (var e in events) lastEv = Math.Max(lastEv, e.Id);
                JsObj<Tile> ext = null;
                if (!full)
                {
                    ext = new JsObj<Tile>();
                    for (int i = 0; i < w.Tiles.Count; i++) { var t = w.Tiles[i]; if (t.Ext != null || t.CutDay != null) ext.Set(i.ToString(CultureInfo.InvariantCulture), t); }
                }
                var doc = new Dictionary<string, object>
                {
                    ["day"] = w.Day, ["rng"] = sim.Rng.State(), ["nextId"] = w.NextId, ["metrics"] = w.Metrics,
                    ["civs"] = w.Civs, ["settlements"] = w.Settlements, ["agents"] = w.Agents, ["events"] = events,
                    ["deposits"] = full ? w.Deposits : null, ["tiles"] = full ? w.Tiles : null, ["ext"] = ext,
                    ["relations"] = full ? w.Relations : null,
                };
                o.Write(Json.Serialize(doc));
                o.Write('\n');
            }
            Inject(0);
            Dump(true);
            for (int d = 1; d <= days; d++)
            {
                w.Day++;
                Inject(w.Day);
                foreach (var c in w.Civs) if (c.Alive) Economy.EconomyTick(sim, c);
                if (w.Day % 5 == 0) foreach (var c in w.Civs) if (c.Alive) Economy.RepairTick(sim, c);
                foreach (var c in w.Civs)
                {
                    if (!(c.Alive && (w.Day + c.Id * 3) % 10 == 0)) continue;
                    if (sim.CivSettlements(c).Count == 0) { sim.Extinct(c); continue; }
                    if (!J.T(c.Research.Current)) Research.ChooseResearch(sim, c);
                    Research.EraCheck(sim, c);
                    Economy.ChooseBuilds(sim, c);
                    Economy.RecruitTick(sim, c);
                }
                if (w.Day % 10 == 0) sim.UpdateTerritory();
                if (w.Day % 30 == 0) { sim.Discover(); Economy.RegrowForests(sim); Events.DisastersTick(sim); }
                if (w.Day % Sim.YEAR == 0) foreach (var c in w.Civs) if (c.Alive) Research.ClassYearly(sim, c);
                w.RngState = sim.Rng.State();
                Dump(w.Day % tilesEvery == 0);
            }
        }
        catch (Exception e)
        {
            o.Flush();
            Console.Error.WriteLine($"EconCheck stopped on day {sim?.W.Day}: {e}");
            return 2;
        }
        o.Flush();
        return 0;
    }
}
