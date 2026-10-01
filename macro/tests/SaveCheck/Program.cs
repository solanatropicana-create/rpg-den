using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using FD.Macro;

namespace SaveCheck;

/// <summary>
/// Save/load equivalence check (DESIGN-FAZ1.md A2). Run from macro/ after <c>dotnet build tests/SaveCheck -c Release</c>:
/// <c>dotnet tests/SaveCheck/bin/Release/net8.0/SaveCheck.dll &lt;mode&gt; [options]</c>
/// <list type="bullet">
/// <item><c>selftest</c>: serializer unit checks (bit-exact doubles, JsObj key order, sharing, cycles, strictness,
///   unsupported shapes) + small Sim round trips + hidden-state audits.</item>
/// <item><c>matrix [--seeds 1-4] [--days 3600] [--ks 0,1,37,500,1234,2400,3599] [--jobs 2] [--inproc 2400] [--out DIR]</c>:
///   per seed one <c>reference</c> process (uninterrupted run, per-day digests, saves at every K, in-process
///   load + graph check at every K, one in-process resume from a stream save at --inproc), then one <c>resume</c>
///   process per (seed, K): loads the file written by the other process, continues to --days and compares every day.</item>
/// <item><c>negative [--seeds 1-4 | --seed S] [--k auto|K] [--days 3600]</c>: breaks reference sharing on purpose (this serializer with
///   preserveReferences=false, and a plain System.Text.Json round trip) and shows that the graph check and the
///   per-day digests detect it, next to a correct load as control.</item>
/// <item><c>bench [--seed 1] [--days 2400]</c>: save size and save/load time of a day-N world (+ round-trip checks; 2400 = 60 years of 40 days).</item>
/// </list>
/// Exit code 0 = all checks passed, 1 = a check failed, 2 = usage, 3 = crash.
/// </summary>
public static class Program
{
    public static int Main(string[] args)
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        try
        {
            D.Init();
            if (args.Length == 0) return Usage();
            var o = new Opts(args.Skip(1).ToArray());
            switch (args[0])
            {
                case "selftest": return SelfTest.Run();
                case "matrix": return Matrix.Run(o);
                case "reference": return Matrix.Reference(o);
                case "resume": return Matrix.Resume(o);
                case "negative": return Negative.Run(o);
                case "bench": return Bench.Run(o);
                default: return Usage();
            }
        }
        catch (UsageException e) { Console.Error.WriteLine(e.Message); return 2; }
        catch (Exception e) { Console.Error.WriteLine("CRASH " + e); return 3; }
    }

    private static int Usage()
    {
        Console.Error.WriteLine("usage: SaveCheck selftest | matrix [--seeds 1-4] [--days 3600] [--ks 0,1,37,500,1234,2400,3599] [--jobs 2] [--inproc 2400] [--out DIR]"
            + " | negative [--seeds 1-4 | --seed S] [--k auto|K] [--days 3600] | bench [--seed 1] [--days 2400] [--out DIR]");
        return 2;
    }

    public static string Ms(double ms) => ms.ToString(ms < 10 ? "0.0" : "0", CultureInfo.InvariantCulture);
    public static string Kb(long b) => (b / 1024.0).ToString("0", CultureInfo.InvariantCulture);
    public static string Mb(long b) => (b / 1048576.0).ToString("0.00", CultureInfo.InvariantCulture);
}

internal sealed class UsageException : Exception { public UsageException(string m) : base(m) { } }

/// <summary>--name value options.</summary>
internal sealed class Opts
{
    private readonly Dictionary<string, string> _v = new();

    public Opts(string[] a)
    {
        for (int i = 0; i < a.Length; i++)
        {
            if (!a[i].StartsWith("--", StringComparison.Ordinal) || i + 1 >= a.Length) throw new UsageException($"bad option {a[i]}");
            _v[a[i].Substring(2)] = a[++i];
        }
    }

    public string Str(string k, string def) => _v.TryGetValue(k, out var s) ? s : def;
    public bool Has(string k) => _v.ContainsKey(k);

    public int Int(string k, int def)
    {
        if (!_v.TryGetValue(k, out var s)) return def;
        return int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out int x) ? x : throw new UsageException($"--{k}: integer expected");
    }

    /// <summary>"1-4,7" → [1,2,3,4,7]</summary>
    public List<int> Ints(string k, string def)
    {
        var outp = new List<int>();
        foreach (var part in Str(k, def).Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            int dash = part.IndexOf('-', 1);
            if (dash > 0) { int a = int.Parse(part.Substring(0, dash), CultureInfo.InvariantCulture), b = int.Parse(part.Substring(dash + 1), CultureInfo.InvariantCulture); for (int x = a; x <= b; x++) outp.Add(x); }
            else outp.Add(int.Parse(part, CultureInfo.InvariantCulture));
        }
        return outp;
    }
}

/// <summary>RESULT lines: "RESULT key=value ..." printed by workers, parsed by the orchestrator.</summary>
internal static class Result
{
    public static void Print(params (string K, object V)[] kv)
    {
        var sb = new StringBuilder("RESULT");
        foreach (var (k, v) in kv) sb.Append(' ').Append(k).Append('=').Append(Convert.ToString(v, CultureInfo.InvariantCulture).Replace(' ', '_'));
        Console.WriteLine(sb.ToString());
        Console.Out.Flush();
    }

    public static Dictionary<string, string> Parse(string line)
    {
        var d = new Dictionary<string, string>();
        foreach (var p in line.Substring("RESULT ".Length).Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            int e = p.IndexOf('=');
            if (e > 0) d[p.Substring(0, e)] = p.Substring(e + 1);
        }
        return d;
    }
}

// ==================================================================== matrix
internal static class Matrix
{
    private const string DefaultKs = "0,1,37,500,1234,2400,3599";

    private static string DefaultOut() => Path.Combine(Path.GetTempPath(), "fd-savecheck");
    public static string SavePath(string dir, int seed, int k) => Path.Combine(dir, $"s{seed}-k{k}.sav");
    public static string RefPath(string dir, int seed) => Path.Combine(dir, $"ref-s{seed}.txt");

    // ---------------------------------------------------------------- orchestrator
    public static int Run(Opts o)
    {
        var seeds = o.Ints("seeds", "1-4");
        int days = o.Int("days", 3600), jobs = Math.Max(1, o.Int("jobs", 2)), inproc = o.Int("inproc", 2400);
        var ks = o.Ints("ks", DefaultKs);
        if (ks.Any(k => k < 0 || k >= days)) throw new UsageException("every K must be in [0, days)");
        string dir = o.Str("out", DefaultOut());
        Directory.CreateDirectory(dir);
        string ksArg = string.Join(",", ks);
        Console.WriteLine($"save/load matrix: seeds {string.Join(",", seeds)}, N = {days} days, K = {ksArg}, {jobs} parallel processes, files in {dir}");
        Console.WriteLine($"phase 1: one reference process per seed (uninterrupted run, digest every day, Save at each K + in-process Load/graph check, in-process stream resume at K = {inproc})");
        Console.WriteLine("phase 2: one resume process per (seed, K): Sim.Load(file from phase 1) → continue to N → compare every day");
        var sw = Stopwatch.StartNew();
        var tasks = new List<(string Label, string Args)>();
        foreach (int s in seeds) tasks.Add(($"reference seed {s}", $"reference --seed {s} --days {days} --ks {ksArg} --inproc {inproc} --out \"{dir}\""));
        var refTasks = tasks.Count;
        foreach (int s in seeds) foreach (int k in ks) tasks.Add(($"resume seed {s} K {k}", $"resume --seed {s} --k {k} --days {days} --out \"{dir}\""));
        var results = new List<Dictionary<string, string>>();
        var failures = new List<string>();
        int next = 0;
        var gate = new object();
        // references first (phase 1), then resumes (phase 2) — a resume needs the files of its reference
        void Worker(int phaseEnd, int phaseStart)
        {
            while (true)
            {
                int i;
                lock (gate) { if (next >= phaseEnd) return; i = next++; }
                var (label, args) = tasks[i];
                var t0 = Stopwatch.StartNew();
                var (code, lines, err) = Child.Run(args);
                lock (gate)
                {
                    foreach (var l in lines.Where(l => l.StartsWith("RESULT ", StringComparison.Ordinal))) results.Add(Result.Parse(l));
                    bool bad = code != 0;
                    Console.WriteLine($"  [{sw.Elapsed:hh\\:mm\\:ss}] {label}: {(bad ? $"FAILED (exit {code})" : "ok")} in {t0.Elapsed.TotalSeconds:0}s");
                    if (bad)
                    {
                        failures.Add(label);
                        foreach (var l in lines.Where(l => !l.StartsWith("RESULT ", StringComparison.Ordinal)).Take(30)) Console.WriteLine("      " + l);
                        foreach (var l in err.Split('\n').Take(30)) if (l.Length > 0) Console.WriteLine("      ! " + l);
                    }
                }
            }
        }
        void Phase(int start, int end)
        {
            next = start;
            var th = Enumerable.Range(0, jobs).Select(_ => new Thread(() => Worker(end, start))).ToList();
            th.ForEach(t => t.Start());
            th.ForEach(t => t.Join());
        }
        Phase(0, refTasks);
        Phase(refTasks, tasks.Count);
        File.WriteAllLines(Path.Combine(dir, "results.txt"), results.Select(r => "RESULT " + string.Join(" ", r.Select(kv => kv.Key + "=" + kv.Value))));
        Console.WriteLine();
        PrintTables(results, seeds, ks, days);
        Console.WriteLine();
        bool ok = failures.Count == 0 && results.Where(r => r.TryGetValue("status", out var st) && st != "ok").ToList() is { Count: 0 };
        int expect = seeds.Count * (1 + 2 * ks.Count + (ks.Contains(inproc) ? 1 : 0));
        if (results.Count != expect) { ok = false; Console.WriteLine($"expected {expect} RESULT lines, got {results.Count}"); }
        Console.WriteLine($"matrix: {(ok ? "PASS" : "FAIL")} in {sw.Elapsed.TotalSeconds:0}s" + (failures.Count > 0 ? $" (failed: {string.Join("; ", failures)})" : ""));
        return ok ? 0 : 1;
    }

    private static void PrintTables(List<Dictionary<string, string>> rs, List<int> seeds, List<int> ks, int days)
    {
        string G(Dictionary<string, string> r, string k) => r != null && r.TryGetValue(k, out var v) ? v : "-";
        Dictionary<string, string> Find(string kind, int s, int k) => rs.FirstOrDefault(r => G(r, "kind") == kind && G(r, "seed") == s.ToString(CultureInfo.InvariantCulture) && (k < 0 || G(r, "k") == k.ToString(CultureInfo.InvariantCulture)));
        Console.WriteLine("reference runs (uninterrupted; static-data digest before/after; hidden-state audits):");
        Console.WriteLine($"  {"seed",4} {"days",5} {"run s",6} {"status",-6} audits");
        foreach (int s in seeds)
        {
            var r = Find("ref", s, -1);
            Console.WriteLine($"  {s,4} {G(r, "days"),5} {G(r, "s"),6} {G(r, "status"),-6} {G(r, "audit")}");
        }
        Console.WriteLine();
        Console.WriteLine("saves (reference process: Save(path) at day K; Sim.Load in the same process; graph = values + sharing isomorphic; digest at K):");
        Console.WriteLine($"  {"seed",4} {"K",5} {"gz KB",7} {"json KB",8} {"objects",8} {"shared",7} {"$refs",7} {"save ms",8} {"load ms",8} {"graph",-6} {"digestK",-7} {"static",-6} shared objects by type");
        foreach (int s in seeds) foreach (int k in ks)
            {
                var r = Find("save", s, k);
                Console.WriteLine($"  {s,4} {k,5} {G(r, "gzkb"),7} {G(r, "jsonkb"),8} {G(r, "objects"),8} {G(r, "shared"),7} {G(r, "refs"),7} {G(r, "savems"),8} {G(r, "loadms"),8} {G(r, "graph"),-6} {G(r, "digestk"),-7} {G(r, "staticshare"),-6} {G(r, "sharedtypes")}");
            }
        Console.WriteLine();
        Console.WriteLine($"resumes (separate process: Sim.Load(file) → Step to {days}; fingerprint compared on every day K..N, STJ digest on the reference's sparse days; 'inproc' = same process, Save/Load via MemoryStream):");
        Console.WriteLine($"  {"seed",4} {"K",5} {"how",-7} {"load ms",8} {"days",5} {"fp days",7} {"stj days",8} {"status",-8} first mismatch");
        foreach (int s in seeds)
        {
            foreach (int k in ks)
            {
                var r = Find("resume", s, k);
                Console.WriteLine($"  {s,4} {k,5} {"process",-7} {G(r, "loadms"),8} {G(r, "days"),5} {G(r, "compared"),7} {G(r, "stj"),8} {G(r, "status"),-8} {G(r, "mismatch")}");
            }
            var ip = Find("inproc", s, -1);
            if (ip != null) Console.WriteLine($"  {s,4} {G(ip, "k"),5} {"inproc",-7} {G(ip, "loadms"),8} {G(ip, "days"),5} {G(ip, "compared"),7} {G(ip, "stj"),8} {G(ip, "status"),-8} {G(ip, "mismatch")}");
        }
    }

    // ---------------------------------------------------------------- reference worker
    public static int Reference(Opts o)
    {
        int seed = o.Int("seed", 1), days = o.Int("days", 3600), inproc = o.Int("inproc", -1);
        var ks = new HashSet<int>(o.Ints("ks", DefaultKs));
        string dir = o.Str("out", DefaultOut());
        Directory.CreateDirectory(dir);
        bool ok = true;

        // hidden state audits (things a save could not see)
        var audit = new List<string>();
        var unknownSim = Audit.UnknownSimFields();
        var unknownRng = Audit.UnknownRngFields();
        var mutableStatics = Audit.MutableStatics();
        if (unknownSim.Count > 0) { audit.Add("SIM-FIELDS:" + string.Join("+", unknownSim)); ok = false; }
        if (unknownRng.Count > 0) { audit.Add("RNG-FIELDS:" + string.Join("+", unknownRng)); ok = false; }
        if (mutableStatics.Count > 0) { audit.Add("MUTABLE-STATICS:" + string.Join("+", mutableStatics)); ok = false; }
        string staticBefore = Audit.StaticContentDigest();
        var staticObjs = Audit.StaticObjects();

        var sw = Stopwatch.StartNew();
        var sim = new Sim(seed);
        var dig = new Ref(days);
        byte[] inprocBytes = null;
        void Record(int d)
        {
            dig.Fp[d] = Fingerprint.Of(sim);
            if (Ref.Sparse(d, ks, days)) dig.Stj[d] = Digest.Of(sim);
            dig.Caches[d] = $"{sim.PathCacheSize} {sim.NavCache.Count}";
            if (!ks.Contains(d)) return;
            string path = SavePath(dir, seed, d);
            var t = Stopwatch.StartNew();
            sim.Save(path);
            double saveMs = t.Elapsed.TotalMilliseconds;
            bool unchanged = Fingerprint.Of(sim) == dig.Fp[d] && Digest.Of(sim) == dig.Stj[d];
            var info = SaveCodec.WriteSave(Stream.Null, sim.CaptureSave(), compress: false);
            t.Restart();
            var loaded = Sim.Load(path);
            double loadMs = t.Elapsed.TotalMilliseconds;
            var g = GraphCompare.Run(sim.CaptureSave(), loaded.CaptureSave());
            bool dk = Fingerprint.Of(loaded) == dig.Fp[d] && Digest.Of(loaded) == dig.Stj[d];
            var shareStatic = Audit.SharedWithStatics(sim.CaptureSave(), staticObjs);
            if (!g.Ok || !dk || !unchanged || shareStatic.Count > 0) ok = false;
            if (!g.Ok) foreach (var e in g.Errors) Console.WriteLine($"graph seed {seed} K {d}: {e}");
            foreach (var e in shareStatic) Console.WriteLine($"world object reachable from a static field, seed {seed} K {d}: {e}");
            if (!unchanged) Console.WriteLine($"seed {seed} K {d}: Save changed the simulation state!");
            Result.Print(("kind", "save"), ("seed", seed), ("k", d), ("gzkb", Program.Kb(new FileInfo(path).Length)), ("jsonkb", Program.Kb(info.JsonBytes)),
                ("objects", info.Objects), ("shared", info.Shared), ("refs", info.Refs), ("savems", Program.Ms(saveMs)), ("loadms", Program.Ms(loadMs)),
                ("graph", g.Ok ? "ok" : "FAIL"), ("digestk", dk && unchanged ? "ok" : "FAIL"), ("staticshare", shareStatic.Count == 0 ? "none" : shareStatic.Count.ToString(CultureInfo.InvariantCulture)),
                ("sharedtypes", g.SharedSummary().Replace(", ", ",")));
            if (d == inproc)
            {
                var ms = new MemoryStream();
                sim.Save(ms);                       // stream overload (gzip)
                inprocBytes = ms.ToArray();
            }
        }
        Record(0);
        for (int d = 1; d <= days; d++) { sim.Step(); Record(d); }
        double runS = sw.Elapsed.TotalSeconds;
        dig.Write(RefPath(dir, seed));

        // same-process resume from the stream save
        if (inprocBytes != null)
        {
            var t = Stopwatch.StartNew();
            var s2 = Sim.Load(new MemoryStream(inprocBytes));
            double loadMs = t.Elapsed.TotalMilliseconds;
            var (status, mismatch, compared, stjCompared) = Continue(s2, dig, days);
            if (status != "ok") ok = false;
            Result.Print(("kind", "inproc"), ("seed", seed), ("k", inproc), ("loadms", Program.Ms(loadMs)), ("days", days - inproc), ("compared", compared), ("stj", stjCompared), ("status", status), ("mismatch", mismatch));
        }

        string staticAfter = Audit.StaticContentDigest();
        if (staticAfter != staticBefore) { audit.Add("STATIC-DATA-CHANGED"); ok = false; }
        Result.Print(("kind", "ref"), ("seed", seed), ("days", days), ("s", runS.ToString("0", CultureInfo.InvariantCulture)), ("status", ok ? "ok" : "FAIL"),
            ("audit", audit.Count == 0 ? "ok(sim-fields,rng-fields,no-mutable-statics,static-data-unchanged)" : string.Join(",", audit)));
        return ok ? 0 : 1;
    }

    /// <summary>Reference digests of an uninterrupted run: fingerprint every day, STJ SHA-256 on sparse days.</summary>
    public sealed class Ref
    {
        public readonly string[] Fp, Stj, Caches;
        public Ref(int days) { Fp = new string[days + 1]; Stj = new string[days + 1]; Caches = new string[days + 1]; }

        /// <summary>days with the (slower, value-only) System.Text.Json digest: every 100th, every K, the last</summary>
        public static bool Sparse(int d, ICollection<int> ks, int days) => d % 100 == 0 || ks.Contains(d) || d == days;

        public void Write(string path)
        {
            var sb = new StringBuilder();
            for (int d = 0; d < Fp.Length; d++) sb.Append(d).Append(' ').Append(Fp[d]).Append(' ').Append(Stj[d] ?? "-").Append(' ').Append(Caches[d]).Append('\n');
            File.WriteAllText(path, sb.ToString());
        }

        public static Ref Read(string path, int days)
        {
            var r = new Ref(days);
            foreach (var line in File.ReadAllLines(path))
            {
                var p = line.Split(' ');
                int d = int.Parse(p[0], CultureInfo.InvariantCulture);
                if (d > days) continue;
                r.Fp[d] = p[1];
                r.Stj[d] = p[2] == "-" ? null : p[2];
            }
            if (r.Fp.Any(x => x == null)) throw new InvalidDataException($"reference file {path} does not cover days 0..{days}");
            return r;
        }
    }

    /// <summary>Steps a loaded sim to <paramref name="days"/>, comparing the fingerprint every day (from the load day
    /// on) and the STJ digest wherever the reference has one.</summary>
    public static (string Status, string Mismatch, int Compared, int StjCompared) Continue(Sim s, Ref dig, int days)
    {
        int k = s.Day, compared = 0, stj = 0;
        for (int d = k; d <= days; d++)
        {
            if (d > k) s.Step();
            compared++;
            string at = d == k ? $"day{d}(at-load)" : $"day{d}";
            if (Fingerprint.Of(s) != dig.Fp[d]) return ("FAIL", at + "(fingerprint)", compared, stj);
            if (dig.Stj[d] != null)
            {
                stj++;
                if (Digest.Of(s) != dig.Stj[d]) return ("FAIL", at + "(stj)", compared, stj);
            }
        }
        return ("ok", "-", compared, stj);
    }

    // ---------------------------------------------------------------- resume worker
    public static int Resume(Opts o)
    {
        int seed = o.Int("seed", 1), k = o.Int("k", 0), days = o.Int("days", 3600);
        string dir = o.Str("out", DefaultOut());
        var dig = Ref.Read(RefPath(dir, seed), days);
        var t = Stopwatch.StartNew();
        var sim = Sim.Load(SavePath(dir, seed, k));
        double loadMs = t.Elapsed.TotalMilliseconds;
        if (sim.Day != k) throw new InvalidDataException($"save of K {k} holds day {sim.Day}");
        var (status, mismatch, compared, stjCompared) = Continue(sim, dig, days);
        Result.Print(("kind", "resume"), ("seed", seed), ("k", k), ("loadms", Program.Ms(loadMs)), ("days", days - k), ("compared", compared), ("stj", stjCompared), ("status", status), ("mismatch", mismatch));
        return status == "ok" ? 0 : 1;
    }
}

/// <summary>Runs this program as a child process (dotnet SaveCheck.dll ...), capturing output.</summary>
internal static class Child
{
    public static (int Code, List<string> Out, string Err) Run(string args)
    {
        string dll = typeof(Program).Assembly.Location;
        var psi = new ProcessStartInfo("dotnet", $"\"{dll}\" {args}")
        {
            RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false,
            StandardOutputEncoding = Encoding.UTF8, StandardErrorEncoding = Encoding.UTF8,
        };
        using var p = Process.Start(psi);
        var err = new StringBuilder();
        p.ErrorDataReceived += (_, e) => { if (e.Data != null) lock (err) err.Append(e.Data).Append('\n'); };
        p.BeginErrorReadLine();
        var lines = new List<string>();
        string l;
        while ((l = p.StandardOutput.ReadLine()) != null) lines.Add(l);
        p.WaitForExit();
        return (p.ExitCode, lines, err.ToString());
    }
}

// ==================================================================== negative test
internal static class Negative
{
    private sealed class Variant
    {
        public string Name;
        public bool Broken;
        public Sim S;
        public GraphCompare G;
        public int FpDay = -1, StjDay = -1, Days;
        public bool Active = true;
    }

    public static int Run(Opts o)
    {
        int days = o.Int("days", 3600);
        // K: given, or "auto" = the first day from 600 on with a war going on (a War object is shared by both
        // directions of a relation and mutated through either, so losing that sharing changes values, not only
        // structure). Without a war in --seeds, the last seed's day (days - 300) is used.
        bool auto = o.Str("k", "auto") == "auto";
        var seeds = o.Has("seed") ? new List<int> { o.Int("seed", 1) } : o.Ints("seeds", "1-4");
        int k = -1, seed = -1;
        Sim sim = null;
        foreach (int sd in seeds)
        {
            seed = sd;
            sim = new Sim(sd);
            bool war = false;
            for (int d = 1; ; d++)
            {
                sim.Step();
                if (!auto) { if (d == o.Int("k", 1234)) { k = d; break; } continue; }
                if (d >= 600 && sim.W.Relations.Any(row => row.Any(r => r.War != null))) { k = d; war = true; break; }
                if (d >= days - 300) { k = d; break; }
            }
            if (!auto || war) break;
            Console.WriteLine($"  (seed {sd}: no war between day 600 and {days - 300})");
        }
        Console.WriteLine($"negative test: seed {seed}; the day-{k} state{(auto ? " (first day ≥ 600 with an active war)" : "")} is restored three ways and each copy is stepped");
        Console.WriteLine($"to day {days} in lockstep with the uninterrupted original (fingerprint = values + key order + sharing; stj = values only).");
        var good = new MemoryStream();
        sim.Save(good);
        var noref = new MemoryStream();
        var info = SaveCodec.WriteSave(noref, sim.CaptureSave(), compress: true, preserveReferences: false);
        var control = Sim.Load(new MemoryStream(good.ToArray()));
        var stj = Sim.FromSave(new SaveState
        {
            World = Json.Deserialize<World>(Json.Serialize(sim.W)), RngState = sim.Rng.State(), RngCalls = sim.Rng.Calls,
            PathCache = Json.Deserialize<Dictionary<string, List<int>>>(Json.Serialize(new Dictionary<string, List<int>>(sim.PathCacheView))),
            NavCache = Json.Deserialize<Dictionary<string, List<int>>>(Json.Serialize(sim.NavCache)),
            ShoreW = sim.ShoreW == null ? null : (byte[])sim.ShoreW.Clone(),
        });
        var vs = new List<Variant>
        {
            new() { Name = "correct save/load (control)", S = control },
            new() { Name = "this serializer with preserveReferences=false", Broken = true, S = Sim.Load(new MemoryStream(noref.ToArray())) },
            new() { Name = "plain System.Text.Json round trip (Json.Serialize/Deserialize)", Broken = true, S = stj },
        };
        Console.WriteLine($"  day-{k} save: {Program.Kb(good.Length)} KB gzip with sharing preserved; {Program.Kb(info.Bytes)} KB with every occurrence written out");
        foreach (var v in vs) v.G = GraphCompare.Run(sim.CaptureSave(), v.S.CaptureSave());
        for (int d = k; d <= days && vs.Any(v => v.Active); d++)
        {
            if (d > k) { sim.Step(); foreach (var v in vs) if (v.Active) v.S.Step(); }
            string fp = Fingerprint.Of(sim);
            bool needStj = vs.Any(v => v.Active && (v.StjDay < 0 && (v.Broken || d % 100 == 0 || d == days)));
            string sj = needStj ? Digest.Of(sim) : null;
            foreach (var v in vs)
            {
                if (!v.Active) continue;
                v.Days++;
                if (v.FpDay < 0 && Fingerprint.Of(v.S) != fp) v.FpDay = d;
                if (sj != null && v.StjDay < 0 && (v.Broken || d % 100 == 0 || d == days) && Digest.Of(v.S) != sj) v.StjDay = d;
                if (v.Broken && v.FpDay >= 0 && v.StjDay >= 0) v.Active = false;   // both detections seen
                if (!v.Broken && (v.FpDay >= 0 || v.StjDay >= 0)) v.Active = false;
            }
        }
        bool ok = true;
        foreach (var v in vs)
        {
            Console.WriteLine($"  {v.Name}:");
            Console.WriteLine($"    graph check vs the original at day {k}: {(v.G.Ok ? "identical (values and sharing)" : $"{v.G.ErrorCount} differences")}");
            foreach (var e in v.G.Errors.Take(3)) Console.WriteLine($"      {e}");
            if (v.G.LostByType.Count > 0) Console.WriteLine($"      objects whose sharing was lost, by type: {string.Join(", ", v.G.LostByType.Select(kv => $"{kv.Key}×{kv.Value}"))}");
            Console.WriteLine($"    per-day fingerprint: {(v.FpDay < 0 ? $"identical on all {v.Days} days ({k}..{days})" : v.FpDay == k ? $"differs at load (day {k})" : $"first difference on day {v.FpDay}")}");
            Console.WriteLine($"    values only (stj digest): {(v.StjDay < 0 ? (v.Broken ? $"identical through day {days}" : "identical on every compared day") : $"first difference on day {v.StjDay} ({v.StjDay - k} days after the load)")}");
            bool pass = v.Broken ? !v.G.Ok && v.FpDay == k : v.G.Ok && v.FpDay < 0 && v.StjDay < 0;
            Console.WriteLine($"    → {(pass ? (v.Broken ? "detected, as expected" : "identical, as expected") : "UNEXPECTED")}");
            ok &= pass;
        }
        Console.WriteLine($"negative: {(ok ? "PASS" : "FAIL")}");
        return ok ? 0 : 1;
    }
}

// ==================================================================== bench
internal static class Bench
{
    public static int Run(Opts o)
    {
        int seed = o.Int("seed", 1), days = o.Int("days", 60 * Sim.YEAR);
        string dir = o.Str("out", Path.Combine(Path.GetTempPath(), "fd-savecheck"));
        Directory.CreateDirectory(dir);
        var sw = Stopwatch.StartNew();
        var sim = new Sim(seed);
        for (int d = 1; d <= days; d++) sim.Step();
        Console.WriteLine($"bench: seed {seed}, {days} days simulated in {sw.Elapsed.TotalSeconds:0.0}s");
        var w = sim.W;
        Console.WriteLine($"  world: {w.Tiles.Count} tiles, {w.Civs.Count} civs, {w.Settlements.Count} settlements, {w.Heroes.Count} heroes, {w.Agents.Count} agents, {w.Routes.Count} routes, {w.Events.Count} events, {w.Battles.Count} battles; path cache {sim.PathCacheSize}, nav cache {sim.NavCache.Count}");
        string before = Fingerprint.Of(sim), beforeStj = Digest.Of(sim);
        string path = Path.Combine(dir, $"bench-s{seed}-d{days}.sav");
        var info = SaveCodec.WriteSave(Stream.Null, sim.CaptureSave(), compress: false);
        var saveMs = new List<double>();
        var loadMs = new List<double>();
        Sim loaded = null;
        for (int i = 0; i < 5; i++)
        {
            var t = Stopwatch.StartNew();
            sim.Save(path);
            saveMs.Add(t.Elapsed.TotalMilliseconds);
            t.Restart();
            loaded = Sim.Load(path);
            loadMs.Add(t.Elapsed.TotalMilliseconds);
        }
        long size = new FileInfo(path).Length;
        string Med(List<double> xs) { var s = xs.OrderBy(x => x).ToList(); return $"{Program.Ms(s[s.Count / 2])} ms (min {Program.Ms(s[0])}, first {Program.Ms(xs[0])})"; }
        Console.WriteLine($"  save file: {Program.Mb(size)} MB gzip ({Program.Mb(info.JsonBytes)} MB JSON), {info.Objects} objects, {info.Shared} shared ($id), {info.Refs} $ref");
        Console.WriteLine($"  Save(path): median {Med(saveMs)}");
        Console.WriteLine($"  Load(path): median {Med(loadMs)}");
        // what the parts cost
        var t2 = Stopwatch.StartNew();
        var plain = new MemoryStream();
        sim.Save(plain, compress: false);
        double plainMs = t2.Elapsed.TotalMilliseconds;
        var json = plain.ToArray();
        t2.Restart();
        SaveCodec.ReadSave(json);
        double parseMs = t2.Elapsed.TotalMilliseconds;
        Console.WriteLine($"  without gzip: write {Program.Ms(plainMs)} ms, read {Program.Ms(parseMs)} ms");
        foreach (var lvl in new[] { CompressionLevel.Fastest, CompressionLevel.Optimal, CompressionLevel.SmallestSize })
        {
            var ms = new MemoryStream();
            t2.Restart();
            using (var z = new GZipStream(ms, lvl, leaveOpen: true)) z.Write(json);
            Console.WriteLine($"  gzip {lvl,-12}: {Program.Mb(ms.Length),6} MB in {Program.Ms(t2.Elapsed.TotalMilliseconds)} ms");
        }
        bool ok = true;
        var g = GraphCompare.Run(sim.CaptureSave(), loaded.CaptureSave());
        bool dk = Fingerprint.Of(loaded) == before && Fingerprint.Of(sim) == before && Digest.Of(loaded) == beforeStj && Digest.Of(sim) == beforeStj;
        Console.WriteLine($"  graph check: {(g.Ok ? "identical" : "FAIL")} ({g.Objects} objects; shared: {g.SharedSummary()}); fingerprint + stj digest at day {days}: {(dk ? "identical" : "FAIL")}");
        foreach (var e in g.Errors) Console.WriteLine("    " + e);
        ok &= g.Ok && dk;
        int extra = o.Int("continue", 120);
        int firstBad = -1;
        for (int d = 1; d <= extra && firstBad < 0; d++)
        {
            sim.Step(); loaded.Step();
            if (Fingerprint.Of(sim) != Fingerprint.Of(loaded) || (d % 30 == 0 && Digest.Of(sim) != Digest.Of(loaded))) firstBad = days + d;
        }
        Console.WriteLine($"  continue original and loaded {extra} more days: {(firstBad < 0 ? "identical every day" : $"FIRST DIFFERENCE on day {firstBad}")}");
        {
            // what the test's digests cost at this size (after the save measurements: this steps the world on)
            var t = Stopwatch.StartNew();
            for (int i = 0; i < 10; i++) Fingerprint.Of(sim);
            double fpMs = t.Elapsed.TotalMilliseconds / 10;
            t.Restart();
            for (int i = 0; i < 5; i++) Digest.Of(sim);
            double stjMs = t.Elapsed.TotalMilliseconds / 5;
            t.Restart();
            for (int i = 0; i < 60; i++) sim.Step();
            double stepMs = t.Elapsed.TotalMilliseconds / 60;
            Console.WriteLine($"  test digests at this size: fingerprint {Program.Ms(fpMs)} ms, System.Text.Json SHA-256 {Program.Ms(stjMs)} ms (one Step ≈ {Program.Ms(stepMs)} ms)");
        }
        ok &= firstBad < 0;
        Console.WriteLine($"bench: {(ok ? "PASS" : "FAIL")}");
        return ok ? 0 : 1;
    }
}
