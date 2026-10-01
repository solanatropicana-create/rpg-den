using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using FD.Macro;

namespace SeaCheck;

/// <summary>
/// C# twin of golden/ts/sea_check.ts. For every TS line: 'snap' restores the query Sim (Sim.FromWorld of the snapshot);
/// 'q' replays that query's parameters on it (in order, so the nav / path caches and shoreW evolve like in TS);
/// 'case' runs one Sea entry point on a fresh Sim.FromWorld(snapshot) and dumps the post-state.
/// Output lines mirror the TS ones (without the parameters); compare with sea_compare.py.
/// </summary>
public static class Program
{
    private static List<int> Ints(JsonElement e)
    {
        var o = new List<int>();
        foreach (var x in e.EnumerateArray()) o.Add(x.GetInt32());
        return o;
    }

    private static List<int> OptInts(JsonElement root, string name) =>
        root.TryGetProperty(name, out var e) && e.ValueKind == JsonValueKind.Array ? Ints(e) : null;

    private static int? OptInt(JsonElement root, string name) =>
        root.TryGetProperty(name, out var e) && e.ValueKind == JsonValueKind.Number ? e.GetInt32() : null;

    private static string OptStr(JsonElement root, string name) =>
        root.TryGetProperty(name, out var e) && e.ValueKind == JsonValueKind.String ? e.GetString() : null;

    /// <summary>Fake agent from the TS parameters (only the fields TS set).</summary>
    private static Agent AgentOf(JsonElement a)
    {
        var ag = new Agent
        {
            Id = OptInt(a, "id") ?? 0,
            Kind = OptStr(a, "kind"),
            Civ = OptInt(a, "civ") ?? 0,
            Step = OptInt(a, "step") ?? 0,
            Progress = a.TryGetProperty("progress", out var pr) ? pr.GetDouble() : 0,
            Speed = a.TryGetProperty("speed", out var sp) ? sp.GetDouble() : 0,
            Purpose = OptStr(a, "purpose"),
            Monster = OptStr(a, "monster"),
            Hull = OptInt(a, "hull"),
            Landing = OptInt(a, "landing"),
        };
        var path = OptInts(a, "path");
        if (path != null) ag.Path = path;
        return ag;
    }

    private static object Query(Sim s, JsonElement q, string op)
    {
        var w = s.W;
        switch (op)
        {
            case "shore":
            {
                var sh = Sea.ShoreWater(s);
                var idx = new List<int>();
                for (int k = 0; k < sh.Length; k++) if (sh[k] != 0) idx.Add(k);
                return idx;
            }
            case "portWater":
            {
                var pw = new List<int>();
                for (int k = 0; k < w.Tiles.Count; k++) pw.Add(Sea.PortWater(s, k));
                return pw;
            }
            case "pickPort":
            {
                int sid = q.GetProperty("sid").GetInt32();
                var st = J.Find(w.Settlements, x => x.Id == sid);
                int t0 = st.Tier;
                st.Tier = q.GetProperty("tier").GetInt32();
                int r = Sea.PickPort(s, st);
                st.Tier = t0;
                return r;
            }
            case "pickPortF":
                return Sea.PickPort(s, new Settlement { Id = q.GetProperty("id").GetInt32(), Tile = q.GetProperty("tile").GetInt32(), Tier = q.GetProperty("tier").GetInt32() });
            case "hulls":
            {
                int sid = q.GetProperty("sid").GetInt32();
                var st = J.Find(w.Settlements, x => x.Id == sid);
                return new List<double> { Sea.HullsOut(s, st), Sea.GalleysOut(s, st), Sea.FreeHulls(s, st), Sea.FreeGalleys(s, st) };
            }
            case "fleet":
            {
                var c = w.Civs[q.GetProperty("civ").GetInt32()];
                var f = Sea.Fleet(s, c);
                return new List<object> { f.Ships, f.Galleys, Sea.HullName(s, c), Sea.HullName(s, c, true), J.Map(Sea.Ports(s, c), x => x.Id) };
            }
            case "isleOf":
                return Sea.IsleOf(s, q.GetProperty("tile").GetInt32())?.Id;
            case "speedA":
            {
                int aid = q.GetProperty("aid").GetInt32();
                return Sea.ShipSpeed(s, J.Find(w.Agents, x => x.Id == aid));
            }
            case "speedF":
                return Sea.ShipSpeed(s, AgentOf(q.GetProperty("a")));
            case "nav":
                return Sea.NavPath(s, q.GetProperty("from").GetInt32(), q.GetProperty("to").GetInt32(),
                    new NavOpts { Embark = Ints(q.GetProperty("embark")), Open = q.GetProperty("open").GetBoolean(), LandOnly = OptInts(q, "landOnly") });
            case "embarkPort":
                return Sea.EmbarkPort(s, w.Civs[q.GetProperty("civ").GetInt32()], Ints(q.GetProperty("p")))?.Id;
            case "civPath":
            {
                var r = Sea.CivPath(s, w.Civs[q.GetProperty("civ").GetInt32()], q.GetProperty("from").GetInt32(), q.GetProperty("to").GetInt32(), OptInts(q, "landOnly"));
                return r == null ? null : new Dictionary<string, object> { ["path"] = r.Path, ["hull"] = r.Hull?.Id };
            }
            case "seaReturn":
                return Sea.SeaReturnPath(s, AgentOf(q.GetProperty("a")), q.GetProperty("to").GetInt32());
            case "pirateReturn":
                return Sea.PirateReturnPath(s, AgentOf(q.GetProperty("a")), q.GetProperty("cove").GetInt32());
            default:
                throw new InvalidOperationException("unknown query " + op);
        }
    }

    private static Agent AgentById(Sim s, string id) { int i = int.Parse(id); return J.Find(s.W.Agents, x => x.Id == i); }

    /// <summary>Runs one case; returns (extra, include tiles in the dump).</summary>
    private static (object Extra, bool Tiles) Run(Sim s, string name)
    {
        var p = name.Split(':');
        switch (p[0])
        {
            case "tick":
            {
                int d = int.Parse(p[1]);
                s.W.Day = d; Sea.SeaTick(s);
                return (null, d % 120 == 60 || d % 120 == 90);
            }
            case "tickR":
            {
                int d = int.Parse(p[1]);
                s.W.Day = d; s.Rng.SetState(s.W.RngState + int.Parse(p[2]) * 7777777.0); Sea.SeaTick(s);
                return (null, d % 120 == 60 || d % 120 == 90);
            }
            case "explore": Sea.ConsiderSeaExplore(s, s.W.Civs[int.Parse(p[1])]); return (null, false);
            case "fleet": Sea.ConsiderFleet(s, s.W.Civs[int.Parse(p[1])]); return (null, false);
            case "launch": { int id = int.Parse(p[1]); Sea.LaunchPirates(s, J.Find(s.W.Camps, c => c.Id == id)); return (null, false); }
            case "launchR": { int id = int.Parse(p[1]); s.Rng.SetState(s.W.RngState + int.Parse(p[2]) * 7777777.0); Sea.LaunchPirates(s, J.Find(s.W.Camps, c => c.Id == id)); return (null, false); }
            case "sight": Sea.ExploreSight(s, AgentById(s, p[1])); return (null, false);
            case "turn": return (Sea.ExploreTurn(s, AgentById(s, p[1])), false);
            case "arrive": return (Sea.FleetArrive(s, AgentById(s, p[1])), false);
            case "disembark": { var a = AgentById(s, p[1]); Sea.Disembark(s, a, Agents.TileOf(a), int.Parse(p[2])); return (null, false); }
            case "smuggle": Sea.PirateSmuggle(s, AgentById(s, p[1])); return (null, false);
            case "pirateReturn": return (Sea.PirateReturnPath(s, AgentById(s, p[1]), int.Parse(p[2])), false);
            case "seaReturn": return (Sea.SeaReturnPath(s, AgentById(s, p[1]), int.Parse(p[2])), false);
            case "captain": return (Sea.NewCaptain(s), false);
            case "captainR": s.Rng.SetState(s.W.RngState + int.Parse(p[1]) * 7777777.0); return (Sea.NewCaptain(s), false);
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
        string path = args.Length > 0 ? args[0] : "/tmp/sea_ts.ndjson";
        using var o = new StreamWriter(Console.OpenStandardOutput(), new UTF8Encoding(false), 1 << 16);
        string snap = null;
        Sim qs = null;
        foreach (string line in File.ReadLines(path))
        {
            if (line.Length == 0) continue;
            using var doc = JsonDocument.Parse(line);
            var root = doc.RootElement;
            string kind = root.GetProperty("kind").GetString();
            if (kind == "snap")
            {
                snap = root.GetProperty("snap").GetString();
                // day 0, natural: the TS snapshot is the world right after `new Sim(seed)`, so build the query Sim the
                // same way here (the queries never touch the Sim's RNG, so its state does not matter)
                qs = root.GetProperty("day").GetInt32() == 0 && root.GetProperty("tag").GetString() == "nat"
                    ? new Sim(root.GetProperty("seed").GetDouble())
                    : Sim.FromWorld(Json.Deserialize<World>(snap));
                o.Write(Json.Serialize(new Dictionary<string, object> { ["kind"] = "snap", ["seed"] = root.GetProperty("seed").GetDouble(), ["day"] = root.GetProperty("day").GetInt32(), ["tag"] = root.GetProperty("tag").GetString() }));
                o.Write('\n');
                continue;
            }
            if (kind == "q")
            {
                string op = root.GetProperty("op").GetString();
                var res = new Dictionary<string, object> { ["kind"] = "q", ["i"] = root.GetProperty("i").GetInt32(), ["op"] = op };
                try
                {
                    res["r"] = Query(qs, root, op);
                    res["n"] = qs.NavCache.Count;
                    res["pn"] = qs.PathCacheSize;
                }
                catch (Exception e) when (IsStub(e)) { res["skipped"] = e.Message; }
                catch (Exception e) { res["err"] = e.ToString(); }
                o.Write(Json.Serialize(res));
                o.Write('\n');
                continue;
            }
            if (kind == "case")
            {
                string name = root.GetProperty("name").GetString();
                var w = Json.Deserialize<World>(snap);
                int preNext = w.NextId;
                var s = Sim.FromWorld(w);
                var res = new Dictionary<string, object> { ["kind"] = "case", ["day"] = root.GetProperty("day").GetInt32(), ["tag"] = root.GetProperty("tag").GetString(), ["name"] = name };
                bool tiles = false;
                try { (res["extra"], tiles) = Run(s, name); }
                catch (Exception e) when (IsStub(e)) { res["skipped"] = e.Message; }
                catch (Exception e) { res["err"] = e.ToString(); }
                if (!res.ContainsKey("skipped"))
                {
                    res["rng"] = s.Rng.State();
                    res["n"] = s.NavCache.Count;
                    if (tiles && !res.ContainsKey("err"))
                    {
                        var tl = new List<object>();
                        for (int i = 0; i < w.Tiles.Count; i++) if (w.Tiles[i].Ext != null || w.Tiles[i].Camp != null) tl.Add(new List<object> { i, w.Tiles[i] });
                        res["tiles"] = tl;
                    }
                    w.Tiles = null;
                    w.Relations = null; w.Inns = null; w.Isles = null;
                    w.Events = J.Filter(w.Events, e => e.Id >= preNext);
                    w.Battles = J.Filter(w.Battles, b => b.Id >= preNext);
                    res["w"] = w;
                }
                o.Write(Json.Serialize(res));
                o.Write('\n');
            }
        }
        return 0;
    }
}
