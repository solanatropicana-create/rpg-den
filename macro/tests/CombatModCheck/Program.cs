using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using FD.Macro;

namespace CombatModCheck;

/// <summary>
/// C# twin of golden/ts/combat_modcheck.ts: for every TS case line, restores Sim.FromWorld(snapshot of that day),
/// runs the same Diplomacy / Monsters function and prints { kind, day, name, rng, extra, err, w } (or skipped).
/// </summary>
public static class Program
{
    private sealed class CaseLine
    {
        public string Kind;
        public int Day;
        public string Tag;
        public string Name;
        public double? Rng;
        public double? Extra;
        public string Err;
        public string Skipped;
        public List<List<int>> CampTiles;
        public World W;
    }

    private static Civ CivOf(Sim s, string name) => s.W.Civs[int.Parse(name.Substring(name.IndexOf(':') + 1))];
    private static Camp CampOf(Sim s, string name) { int id = int.Parse(name.Substring(name.IndexOf(':') + 1)); return J.Find(s.W.Camps, x => x.Id == id); }

    /// <summary>Runs one case; returns (extra, include tiles in the dump).</summary>
    private static (double? Extra, bool Tiles) Run(Sim s, string name)
    {
        string op = name.Contains(':') ? name.Substring(0, name.IndexOf(':')) : name;
        switch (op)
        {
            case "relationsTick": Diplomacy.RelationsTick(s); return (null, false);
            case "worldTick": Diplomacy.WorldTick(s); return (null, true);
            case "campsTick": Monsters.CampsTick(s); return (null, true);
            case "campsR": s.Rng.SetState(s.W.RngState + int.Parse(name.Substring(name.IndexOf(':') + 1)) * 7777777.0); Monsters.CampsTick(s); return (null, false);
            case "scout": Diplomacy.ConsiderScout(s, CivOf(s, name)); return (null, false);
            case "trade": Diplomacy.ConsiderTrade(s, CivOf(s, name)); return (null, false);
            case "expand": Diplomacy.ConsiderExpansion(s, CivOf(s, name)); return (null, false);
            case "war": Diplomacy.ConsiderWarAction(s, CivOf(s, name)); return (null, false);
            case "raid": Diplomacy.ConsiderRaid(s, CivOf(s, name)); return (null, false);
            case "mil": return (Diplomacy.MilitaryPower(s, CivOf(s, name)), false);
            case "away": return (Monsters.CampAway(s, CampOf(s, name)), false);
            case "launch": Monsters.LaunchRaid(s, CampOf(s, name)); return (null, false);
            default: throw new InvalidOperationException("unknown case " + name);
        }
    }

    private static bool IsStub(Exception e)
    {
        for (var x = e; x != null; x = x.InnerException) if (x is NotImplementedException) return true;
        return false;
    }

    public static int Main(string[] args)
    {
        D.Init();
        string path = args.Length > 0 ? args[0] : "/tmp/cm_ts.ndjson";
        using var o = new StreamWriter(Console.OpenStandardOutput(), new UTF8Encoding(false), 1 << 16);
        string snap = null;
        foreach (string line in File.ReadLines(path))
        {
            if (line.Length == 0) continue;
            using var doc = JsonDocument.Parse(line);
            var root = doc.RootElement;
            string kind = root.GetProperty("kind").GetString();
            int day = root.GetProperty("day").GetInt32();
            if (kind == "snap") { snap = root.GetProperty("snap").GetString(); continue; }
            string tag = root.TryGetProperty("tag", out var tg) ? tg.GetString() : null;
            string name = root.GetProperty("name").GetString();
            var w = Json.Deserialize<World>(snap);
            int preNext = w.NextId;
            var s = Sim.FromWorld(w);
            var res = new CaseLine { Kind = "case", Day = day, Tag = tag, Name = name };
            bool tiles = false;
            try { (res.Extra, tiles) = Run(s, name); }
            catch (Exception e) when (IsStub(e)) { res.Skipped = e.Message; }
            catch (Exception e) { res.Err = e.ToString(); }
            if (res.Skipped == null)
            {
                res.Rng = s.Rng.State();
                if (name.StartsWith("campsR", StringComparison.Ordinal) && res.Err == null)
                {
                    res.CampTiles = new List<List<int>>();
                    for (int i = 0; i < w.Tiles.Count; i++) if (w.Tiles[i].Camp != null) res.CampTiles.Add(new List<int> { i, w.Tiles[i].Camp.Value, w.Tiles[i].Owner });
                }
                if (!tiles && res.Err == null) w.Tiles = null;
                if (res.Err != null) w.Tiles = null;
                w.Battles = null; w.Inns = null; w.Isles = null; w.Deposits = null;
                w.Events = J.Filter(w.Events, e => e.Id >= preNext);
                res.W = w;
            }
            o.Write(Json.Serialize(res));
            o.Write('\n');
        }
        return 0;
    }
}
