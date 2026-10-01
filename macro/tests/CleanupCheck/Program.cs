using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FD.Macro;

namespace CleanupCheck;

/// <summary>
/// A3b cleanup check (DESIGN-FAZ1.md § A3b). Runs the real <see cref="Sim.Step"/> for each seed and prints:
/// chronicle size vs. every major event logged; stolen techs (and steals of the node under research, duplicate
/// <c>Research.Done</c> entries, days spent researching an already known node); lumber output with prodWood;
/// winter farm yield with harvest; own-settlement trip times with teleport; old vs. new mint income; gold medians;
/// a final world hash per seed (with <c>--det</c> every seed runs twice and the hashes must match).
/// Built twice: against FD.Macro (after) and against a pre-A3b source tree via -p:FdSrc (before); features missing
/// from the baseline are read by reflection / metric name, so one source serves both.
/// Args: [seeds, e.g. 1-4 or 1,5,9] [years=60] [--det] [--jobs N]
/// </summary>
public static class Program
{
    private const int YEAR = 120;

    // ---- new mint formula (mirror of Economy.MintIncome; also evaluated on the baseline as a shadow figure)
    private const double MINT_BASE = 0.01, MINT_PER_POP = 0.0008, MINT_CAP = 0.12;
    private static readonly MethodInfo MintIncomeM = typeof(Economy).GetMethod("MintIncome", BindingFlags.Public | BindingFlags.Static);

    private static double MintNew(Sim s, Settlement st)
    {
        double n = st.Civics.Get("mint") ?? 0;
        if (!st.Alive || !(n > 0)) return 0;
        return n * Math.Min(MINT_CAP, MINT_BASE + MINT_PER_POP * s.Pop(st));
    }

    public sealed class Trip { public int Start; public bool Tele; public string Kind; }

    public sealed class Result
    {
        public double Seed;
        public string Classes = "";
        public string Hash;
        public double Ms;
        // chronicle
        public long Logged, MajorsLogged;
        public int EventsCount, MajorsInEvents, ChronicleCount = -1, ChronicleMajors = -1;
        public bool ChronicleExact;
        public string ChronicleFirst = "";
        // stolen tech
        public double StolenTech, StolenCurrent;
        public int CivsWithDup, DupEntries, CivsTotal;
        public long KnownCurrentCivDays;          // civ-days researching a node already in Research.Done
        // wood
        public double[] Lumber = new double[6];  // lumber wood per decade (world)
        public double LumberWD, LumberWDpw, LumberYpw, LumberYno; // worker-days (all / civs with prodWood) and yield sums
        public long PwCivDays;
        // harvest
        public long HarvestCivDays;
        public double WinterFarmWDh, WinterFarmYh, WinterFarmWDo, WinterFarmYo;
        // teleport
        public long TeleCivDays;
        public int TripsTele, TripsOther; public double TripDaysTele, TripDaysOther;
        public SortedDictionary<string, (int N, double Days)> TripKinds = new(StringComparer.Ordinal);
        // mint
        public double MintOld, MintNewSum; public long MintCivDays, MintMismatch;
        public List<List<double>> GoldByYear = new();   // [year-1] → gold of alive civs at year end
        public List<int> MintsByYear = new();
        public List<List<double>> MintPops = new();     // [decade] → pops of settlements with a mint at year ends
        public List<List<double>> CivMintNewByDecade = new(); // per civ-year new mint income
        public List<List<double>> CivMintOldByDecade = new();
        // forced unit checks on the final world (after the hash)
        public string UnitHarvest = "", UnitTeleport = "", UnitWood = "";
    }

    public static int Main(string[] args)
    {
        D.Init();
        var seeds = new List<double> { 1, 2, 3, 4 };
        int years = 60; bool det = false; int jobs = Math.Max(1, Math.Min(4, Environment.ProcessorCount));
        var pos = new List<string>();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--det") det = true;
            else if (args[i] == "--jobs") jobs = int.Parse(args[++i], CultureInfo.InvariantCulture);
            else pos.Add(args[i]);
        }
        if (pos.Count > 0) seeds = ParseSeeds(pos[0]);
        if (pos.Count > 1) years = int.Parse(pos[1], CultureInfo.InvariantCulture);
        bool baseline = typeof(World).GetField("Chronicle") == null;
        Console.WriteLine($"# CleanupCheck ({(baseline ? "BASELINE: before A3b" : "after A3b")}) seeds {string.Join(",", seeds.Select(x => x.ToString(CultureInfo.InvariantCulture)))} × {years} years, jobs {jobs}");
        var res = RunAll(seeds, years, jobs);
        Report(res, years, baseline);
        int rc = 0;
        if (det)
        {
            var again = RunAll(seeds, years, jobs);
            Console.WriteLine();
            Console.WriteLine("## Determinism (each seed run twice, SHA-256 of the whole World JSON at the end)");
            for (int i = 0; i < res.Count; i++)
            {
                bool same = res[i].Hash == again[i].Hash;
                if (!same) rc = 1;
                Console.WriteLine($"seed {F(res[i].Seed)}: {res[i].Hash.Substring(0, 16)} vs {again[i].Hash.Substring(0, 16)} {(same ? "identical" : "DIFFERENT")}");
            }
        }
        return rc;
    }

    private static List<double> ParseSeeds(string s)
    {
        var o = new List<double>();
        foreach (var part in s.Split(','))
        {
            int dash = part.IndexOf('-', 1);
            if (dash > 0)
            {
                int a = int.Parse(part.Substring(0, dash), CultureInfo.InvariantCulture), b = int.Parse(part.Substring(dash + 1), CultureInfo.InvariantCulture);
                for (int x = a; x <= b; x++) o.Add(x);
            }
            else o.Add(double.Parse(part, CultureInfo.InvariantCulture));
        }
        return o;
    }

    private static List<Result> RunAll(List<double> seeds, int years, int jobs)
    {
        var res = new Result[seeds.Count];
        Parallel.For(0, seeds.Count, new ParallelOptions { MaxDegreeOfParallelism = jobs }, i => res[i] = RunSeed(seeds[i], years));
        return res.ToList();
    }

    private static Result RunSeed(double seed, int years)
    {
        var r = new Result { Seed = seed };
        var sw = Stopwatch.StartNew();
        var sim = new Sim(seed);
        var w = sim.W;
        r.Classes = string.Join(" ", w.Civs.Select(c => c.Cls));
        var majorIds = new List<int>();
        // the constructor logs "Dünya uyandı" before a hook can be attached
        foreach (var e in w.Events) { r.Logged++; if (e.Major == true) majorIds.Add(e.Id); }
        sim.OnEvent = e => { r.Logged++; if (e.Major == true) majorIds.Add(e.Id); };
        var trips = new Dictionary<int, Trip>();
        var seen = new HashSet<int>();
        var civOf = new Dictionary<int, int>();
        var tileOf = new Dictionary<int, int>();
        int days = years * YEAR;
        for (int d = 1; d <= days; d++)
        {
            sim.Step();
            civOf.Clear(); tileOf.Clear();
            foreach (var st in w.Settlements) if (st.Alive) { civOf[st.Id] = st.Civ; tileOf[st.Id] = st.Tile; }
            Daily(sim, r, civOf, tileOf, trips, seen);
            if (w.Day % YEAR == 0) Yearly(sim, r);
        }
        r.MajorsLogged = majorIds.Count;
        r.EventsCount = w.Events.Count;
        r.MajorsInEvents = w.Events.Count(e => e.Major == true);
        var chrField = typeof(World).GetField("Chronicle");
        if (chrField != null)
        {
            var chr = (List<GameEvent>)chrField.GetValue(w);
            r.ChronicleCount = chr.Count;
            r.ChronicleMajors = chr.Count(e => e.Major == true);
            r.ChronicleExact = chr.Count == majorIds.Count && chr.Select(e => e.Id).SequenceEqual(majorIds);
            if (chr.Count > 0) r.ChronicleFirst = $"day {chr[0].Day}: {Trunc(chr[0].Text, 60)}";
        }
        r.StolenTech = w.Metrics.Get("stolenTech") ?? 0;
        r.StolenCurrent = w.Metrics.Get("stolenCurrent") ?? double.NaN;
        r.CivsTotal = w.Civs.Count;
        foreach (var c in w.Civs)
        {
            int dup = c.Research.Done.Count - c.Research.Done.Distinct().Count();
            if (dup > 0) { r.CivsWithDup++; r.DupEntries += dup; }
        }
        r.Hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(Json.Serialize(w))));
        r.Ms = sw.ElapsedMilliseconds;
        // ---- forced unit checks on the final world (it is thrown away afterwards)
        UnitChecks(sim, r);
        return r;
    }

    private static void Daily(Sim sim, Result r, Dictionary<int, int> civOf, Dictionary<int, int> tileOf, Dictionary<int, Trip> trips, HashSet<int> seen)
    {
        var w = sim.W;
        int decade = Math.Min(5, (w.Day - 1) / (10 * YEAR));
        bool winter = sim.Season == 3;
        for (int i = 0; i < w.Tiles.Count; i++)
        {
            var t = w.Tiles[i]; var e = t.Ext;
            if (e == null || !(e.Workers > 0) || t.Owner < 0 || (e.Kind != "lumber" && e.Kind != "farm")) continue;
            if (!sim.ExtWorking(t) || !civOf.TryGetValue(t.Owner, out int ci)) continue;
            var c = w.Civs[ci];
            if (e.Kind == "farm" && !winter) continue;
            var (good, y) = Economy.ExtractYield(sim, c, t);
            if (e.Kind == "lumber")
            {
                r.Lumber[decade] += y * e.Workers;
                r.LumberWD += e.Workers;
                if (sim.E(c, "prodWood") > 0) { r.LumberWDpw += e.Workers; r.LumberYpw += y * e.Workers; }
                else r.LumberYno += y * e.Workers;
            }
            else if (sim.E(c, "harvest") > 0) { r.WinterFarmWDh += e.Workers; r.WinterFarmYh += y * e.Workers; }
            else { r.WinterFarmWDo += e.Workers; r.WinterFarmYo += y * e.Workers; }
        }
        foreach (var c in w.Civs)
        {
            if (!c.Alive) continue;
            if (sim.E(c, "prodWood") > 0) r.PwCivDays++;
            if (sim.E(c, "harvest") > 0) r.HarvestCivDays++;
            if (sim.E(c, "teleport") > 0) r.TeleCivDays++;
            if (J.T(c.Research.Current) && c.Research.Done.Contains(c.Research.Current)) r.KnownCurrentCivDays++;
            // mint: old rule (0.12 once per civ) vs. new rule (every mint, scaled by its settlement)
            bool any = false; double nw = 0;
            foreach (var st in w.Settlements)
            {
                if (!st.Alive || st.Civ != c.Id) continue;
                if (J.T(st.Civics.Get("mint"))) any = true;
                double m = MintNew(sim, st);
                nw += m;
                if (MintIncomeM != null && (double)MintIncomeM.Invoke(null, new object[] { sim, st }) != m) r.MintMismatch++;
            }
            if (any) { r.MintOld += 0.12; r.MintCivDays++; }
            r.MintNewSum += nw;
        }
        // own-settlement trips (path from one own living settlement centre to another): departure → arrival/death in days
        bool OwnAt(int tile, int civ) { int o = w.Tiles[tile].Owner; return o >= 0 && civOf.TryGetValue(o, out int oc) && oc == civ && tileOf[o] == tile; }
        var now = new HashSet<int>();
        foreach (var a in w.Agents)
        {
            now.Add(a.Id);
            if (seen.Contains(a.Id)) continue;
            seen.Add(a.Id);
            bool own = a.Civ >= 0 && a.Path.Count >= 2 && OwnAt(a.Path[0], a.Civ) && OwnAt(a.Path[a.Path.Count - 1], a.Civ);
            if (own) trips[a.Id] = new Trip { Start = w.Day - 1, Tele = sim.E(w.Civs[a.Civ], "teleport") > 0, Kind = a.Kind };
        }
        var done = new List<int>();
        foreach (var kv in trips) if (!now.Contains(kv.Key)) done.Add(kv.Key);
        foreach (int id in done)
        {
            var tr = trips[id]; trips.Remove(id);
            double days = w.Day - tr.Start;
            if (tr.Tele) { r.TripsTele++; r.TripDaysTele += days; } else { r.TripsOther++; r.TripDaysOther += days; }
            string key = (tr.Tele ? "teleport " : "other ") + tr.Kind;
            r.TripKinds.TryGetValue(key, out var v);
            r.TripKinds[key] = (v.N + 1, v.Days + days);
        }
    }

    private static void Yearly(Sim sim, Result r)
    {
        var w = sim.W;
        int decade = Math.Min(5, (w.Day - 1) / (10 * YEAR));
        while (r.MintPops.Count <= decade) { r.MintPops.Add(new List<double>()); r.CivMintNewByDecade.Add(new List<double>()); r.CivMintOldByDecade.Add(new List<double>()); }
        var gold = new List<double>();
        int mints = 0;
        foreach (var c in w.Civs)
        {
            if (!c.Alive) continue;
            gold.Add(sim.St(c, "gold"));
            bool any = false; double nw = 0;
            foreach (var st in w.Settlements)
            {
                if (!st.Alive || st.Civ != c.Id) continue;
                if (J.T(st.Civics.Get("mint"))) { any = true; mints++; r.MintPops[decade].Add(sim.Pop(st)); }
                nw += MintNew(sim, st);
            }
            r.CivMintNewByDecade[decade].Add(nw * YEAR);
            r.CivMintOldByDecade[decade].Add(any ? 0.12 * YEAR : 0);
        }
        r.GoldByYear.Add(gold);
        r.MintsByYear.Add(mints);
    }

    private static void UnitChecks(Sim sim, Result r)
    {
        var w = sim.W;
        int day0 = w.Day;
        // harvest: winter yield of a farm with and without the effect
        for (int i = 0; i < w.Tiles.Count; i++)
        {
            var t = w.Tiles[i];
            if (t.Ext == null || t.Ext.Kind != "farm" || t.Owner < 0 || !sim.ExtWorking(t)) continue;
            int ci = sim.TileCiv(i); if (ci < 0) continue;
            var c = w.Civs[ci];
            if (sim.E(c, "winterImmune") > 0) continue;
            double had = c.Eff.Get("harvest") ?? 0;
            w.Day = (day0 / YEAR) * YEAR + 100; // winter
            c.Eff.Set("harvest", 0); double off = Economy.ExtractYield(sim, c, t).Y;
            c.Eff.Set("harvest", 1); double on = Economy.ExtractYield(sim, c, t).Y;
            w.Day = (day0 / YEAR) * YEAR + 40;  // summer
            double summer = Economy.ExtractYield(sim, c, t).Y;
            c.Eff.Set("harvest", 0); double summerOff = Economy.ExtractYield(sim, c, t).Y;
            c.Eff.Set("harvest", had);
            w.Day = day0;
            r.UnitHarvest = $"{c.Name} L{t.Ext.Level} farm, winter yield/worker {F3(off)} → {F3(on)} with harvest (×{F2(on / off)}); summer {F3(summerOff)} → {F3(summer)}";
            break;
        }
        // prodWood: lumber yield with prodWood forced 0 vs the civ's own value / 0.25
        for (int i = 0; i < w.Tiles.Count; i++)
        {
            var t = w.Tiles[i];
            if (t.Ext == null || t.Ext.Kind != "lumber" || t.Owner < 0 || !sim.ExtWorking(t)) continue;
            int ci = sim.TileCiv(i); if (ci < 0) continue;
            var c = w.Civs[ci];
            double had = c.Eff.Get("prodWood") ?? 0;
            c.Eff.Set("prodWood", 0); double off = Economy.ExtractYield(sim, c, t).Y;
            c.Eff.Set("prodWood", had); double on = Economy.ExtractYield(sim, c, t).Y;
            r.UnitWood = $"{c.Name} L{t.Ext.Level} lumber, forestry={sim.Has(c, "forestry")}, prodWood={F2(had)}: yield/worker {F3(off)} without → {F3(on)} with (×{F2(on / off)})";
            break;
        }
        // teleport: a lone caravan between two own settlements, AgentsTick only, with and without the effect
        foreach (var c in w.Civs)
        {
            if (!c.Alive) continue;
            var ss = sim.CivSettlements(c);
            if (ss.Count < 2) continue;
            Settlement a = null, b = null; List<int> path = null;
            foreach (var x in ss) { foreach (var y in ss) { if (x == y) continue; var p = sim.Path(x.Tile, y.Tile); if (p != null && p.Count >= 8) { a = x; b = y; path = p; break; } } if (a != null) break; }
            if (a == null) continue;
            var keep = w.Agents;
            double had = c.Eff.Get("teleport") ?? 0;
            int Trip(double tele)
            {
                c.Eff.Set("teleport", tele);
                var ag = new Agent { Id = 999999, Kind = "caravan", Civ = c.Id, Path = new List<int>(path), Step = 0, Progress = 0, Speed = 0.65, Cargo = new JsObj<double>(), Troops = 2, From = a.Id, To = b.Id };
                w.Agents = new List<Agent> { ag };
                int n = 0;
                w.Day = day0;
                while (n < 1000 && w.Agents.Contains(ag)) { w.Day++; n++; Agents.AgentsTick(sim); }
                return n;
            }
            int off = Trip(0), on = Trip(1);
            c.Eff.Set("teleport", had);
            w.Agents = keep; w.Day = day0;
            r.UnitTeleport = $"{c.Name}: caravan {a.Name} → {b.Name} ({path.Count - 1} steps) arrives in {off} days without, {on} days with teleport";
            break;
        }
    }

    // ------------------------------------------------------------ report
    private static void Report(List<Result> res, int years, bool baseline)
    {
        Console.WriteLine();
        Console.WriteLine("## Seeds");
        foreach (var r in res) Console.WriteLine($"seed {F(r.Seed)}: {r.Ms / 1000:0.0} s, civs {r.CivsTotal} [{r.Classes}], hash {r.Hash.Substring(0, 16)}");

        Console.WriteLine();
        Console.WriteLine("## 1. Persistent chronicle");
        foreach (var r in res)
            Console.WriteLine($"seed {F(r.Seed)}: logged {r.Logged}, major {r.MajorsLogged}; World.Events {r.EventsCount} (majors still there {r.MajorsInEvents}); " +
                (r.ChronicleCount < 0 ? "World.Chronicle: (none)" : $"World.Chronicle {r.ChronicleCount} (all major: {r.ChronicleMajors == r.ChronicleCount}, == every major in order: {r.ChronicleExact}); first: {r.ChronicleFirst}"));
        Console.WriteLine($"total: majors logged {res.Sum(r => r.MajorsLogged)}, majors left in the capped World.Events {res.Sum(r => r.MajorsInEvents)}, chronicle {(res[0].ChronicleCount < 0 ? "-" : res.Sum(r => r.ChronicleCount).ToString(CultureInfo.InvariantCulture))}");

        Console.WriteLine();
        Console.WriteLine("## 2. Stolen technology");
        foreach (var r in res)
            Console.WriteLine($"seed {F(r.Seed)}: stolenTech {F(r.StolenTech)}, of the node under research {(double.IsNaN(r.StolenCurrent) ? "(not counted)" : F(r.StolenCurrent))}; civs with duplicate Done entries {r.CivsWithDup}/{r.CivsTotal} ({r.DupEntries} entries); civ-days researching an already known node {r.KnownCurrentCivDays}");
        Console.WriteLine($"total: stolen {F(res.Sum(r => r.StolenTech))}, civs with duplicates {res.Sum(r => r.CivsWithDup)}/{res.Sum(r => r.CivsTotal)}, wasted civ-days {res.Sum(r => r.KnownCurrentCivDays)}");

        Console.WriteLine();
        Console.WriteLine("## 3a. prodWood (lumber)");
        foreach (var r in res)
            Console.WriteLine($"seed {F(r.Seed)}: lumber wood by decade [{string.Join(", ", r.Lumber.Select(x => F0(x)))}]; civ-days with prodWood {r.PwCivDays}; " +
                $"yield/worker-day: prodWood civs {F3(r.LumberWDpw > 0 ? r.LumberYpw / r.LumberWDpw : 0)} ({F0(r.LumberWDpw)} wd), others {F3(r.LumberWD - r.LumberWDpw > 0 ? r.LumberYno / (r.LumberWD - r.LumberWDpw) : 0)} ({F0(r.LumberWD - r.LumberWDpw)} wd)");
        var lum = new double[6]; foreach (var r in res) for (int i = 0; i < 6; i++) lum[i] += r.Lumber[i];
        Console.WriteLine($"total lumber wood by decade: [{string.Join(", ", lum.Select(x => F0(x)))}] = {F0(lum.Sum())}");
        foreach (var r in res) if (r.UnitWood != "") Console.WriteLine($"  unit (seed {F(r.Seed)}): {r.UnitWood}");

        Console.WriteLine();
        Console.WriteLine("## 3b. harvest (winter farm yield)");
        foreach (var r in res)
            Console.WriteLine($"seed {F(r.Seed)}: civ-days with harvest {r.HarvestCivDays}; winter yield/worker-day: harvest civs {F3(r.WinterFarmWDh > 0 ? r.WinterFarmYh / r.WinterFarmWDh : 0)} ({F0(r.WinterFarmWDh)} wd), others {F3(r.WinterFarmWDo > 0 ? r.WinterFarmYo / r.WinterFarmWDo : 0)}");
        foreach (var r in res) if (r.UnitHarvest != "") Console.WriteLine($"  unit (seed {F(r.Seed)}): {r.UnitHarvest}");

        Console.WriteLine();
        Console.WriteLine("## 3c. teleport (trips whose path runs from one own living settlement to another)");
        foreach (var r in res)
            Console.WriteLine($"seed {F(r.Seed)}: civ-days with teleport {r.TeleCivDays}; trips: teleport civs {r.TripsTele} (avg {F2(r.TripsTele > 0 ? r.TripDaysTele / r.TripsTele : 0)} d), others {r.TripsOther} (avg {F2(r.TripsOther > 0 ? r.TripDaysOther / r.TripsOther : 0)} d)" +
                $"; by kind: {string.Join(", ", r.TripKinds.Select(kv => $"{kv.Key} {kv.Value.N}×{F1(kv.Value.Days / kv.Value.N)}d"))}");
        foreach (var r in res) if (r.UnitTeleport != "") Console.WriteLine($"  unit (seed {F(r.Seed)}): {r.UnitTeleport}");

        Console.WriteLine();
        Console.WriteLine($"## 4. Mint (old: 0.12/day once per civ; new: per mint min({F2(MINT_CAP)}, {F2(MINT_BASE)} + {MINT_PER_POP.ToString(CultureInfo.InvariantCulture)}×pop))");
        foreach (var r in res)
            Console.WriteLine($"seed {F(r.Seed)}: mint income over the run: old rule {F0(r.MintOld)}, new rule {F0(r.MintNewSum)} (×{F2(r.MintOld > 0 ? r.MintNewSum / r.MintOld : 0)}); civ-days with a mint {r.MintCivDays}; mints at year 10/20/30/40/50/60: {string.Join("/", Enumerable.Range(1, 6).Select(k => k * 10 <= r.MintsByYear.Count ? r.MintsByYear[k * 10 - 1].ToString(CultureInfo.InvariantCulture) : "-"))}" +
                (MintIncomeM != null ? $"; Economy.MintIncome mismatches {r.MintMismatch}" : ""));
        int dec = res.Max(r => r.MintPops.Count);
        for (int dI = 0; dI < dec; dI++)
        {
            var pops = res.SelectMany(r => dI < r.MintPops.Count ? r.MintPops[dI] : new List<double>()).ToList();
            var nw = res.SelectMany(r => dI < r.CivMintNewByDecade.Count ? r.CivMintNewByDecade[dI] : new List<double>()).ToList();
            var od = res.SelectMany(r => dI < r.CivMintOldByDecade.Count ? r.CivMintOldByDecade[dI] : new List<double>()).ToList();
            Console.WriteLine($"  years {dI * 10 + 1}-{dI * 10 + 10}: minted settlements' pop p10/p50/p90 {F0(Pct(pops, 0.1))}/{F0(Pct(pops, 0.5))}/{F0(Pct(pops, 0.9))} (n {pops.Count}); per civ-year mint gold: old mean {F1(Mean(od))}, new mean {F1(Mean(nw))}, new p50/p90 {F1(Pct(nw, 0.5))}/{F1(Pct(nw, 0.9))}");
        }

        Console.WriteLine();
        Console.WriteLine("## 5. Gold (alive civs at year end; pooled over seeds: p50 [p10–p90]; per-seed medians)");
        for (int y = 5; y <= years; y += 5)
        {
            var all = res.SelectMany(r => y <= r.GoldByYear.Count ? r.GoldByYear[y - 1] : new List<double>()).ToList();
            var per = res.Select(r => y <= r.GoldByYear.Count ? Pct(r.GoldByYear[y - 1], 0.5) : double.NaN).ToList();
            Console.WriteLine($"year {y,2}: {F0(Pct(all, 0.5)),7} [{F0(Pct(all, 0.1))}–{F0(Pct(all, 0.9))}]   per seed {string.Join(" ", per.Select(F0))}");
        }
    }

    private static double Pct(List<double> xs, double q)
    {
        if (xs.Count == 0) return double.NaN;
        var s = xs.OrderBy(x => x).ToList();
        double pos = q * (s.Count - 1);
        int lo = (int)Math.Floor(pos), hi = (int)Math.Ceiling(pos);
        return s[lo] + (s[hi] - s[lo]) * (pos - lo);
    }
    private static double Mean(List<double> xs) => xs.Count == 0 ? double.NaN : xs.Average();
    private static string Trunc(string s, int n) => s.Length <= n ? s : s.Substring(0, n) + "…";
    private static string F(double x) => x.ToString(CultureInfo.InvariantCulture);
    private static string F0(double x) => double.IsNaN(x) ? "-" : x.ToString("0", CultureInfo.InvariantCulture);
    private static string F1(double x) => double.IsNaN(x) ? "-" : x.ToString("0.0", CultureInfo.InvariantCulture);
    private static string F2(double x) => double.IsNaN(x) ? "-" : x.ToString("0.00", CultureInfo.InvariantCulture);
    private static string F3(double x) => double.IsNaN(x) ? "-" : x.ToString("0.000", CultureInfo.InvariantCulture);
}
