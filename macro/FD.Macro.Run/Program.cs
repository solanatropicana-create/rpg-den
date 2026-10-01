using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;

namespace FD.Macro.Run;

/// <summary>
/// Golden-test harness, C# side. Twin of <c>golden/ts/trace.ts</c> with byte-identical output formats; driven by
/// <c>golden/compare.py</c> (see <c>golden/README.md</c>). Run from macro/:
/// <c>dotnet FD.Macro.Run/bin/Release/net8.0/FD.Macro.Run.dll &lt;mode&gt; ...</c>
/// <list type="bullet">
/// <item><c>hash &lt;seed&gt; &lt;days&gt;</c>: one line per day (0 = right after new Sim): day hash rngState rngCalls pathCache navCache</item>
/// <item><c>cps &lt;seed&gt; &lt;day&gt;</c>: days 1..day-1 silently, then one line per checkpoint of day: label hash rngState rngCalls</item>
/// <item><c>dump &lt;seed&gt; &lt;day&gt; [label]</c>: flat canonical listing (path=value) at checkpoint label of day (default end)</item>
/// <item><c>rng &lt;seed&gt; &lt;day&gt;</c>: every RNG call of day: index lastCheckpointLabel value callerFunction</item>
/// <item><c>selftest</c>: canonical-form test vectors (must equal <c>trace.js selftest</c>)</item>
/// <item><c>stats --seeds 1-16 --years 60 --out &lt;dir&gt; [--jobs N] ...</c>: headless measurement (Faz 1 A1), see <see cref="StatsMode"/></item>
/// </list>
/// Errors: "CRASH &lt;day&gt; &lt;lastCheckpoint&gt; &lt;ExceptionType&gt;: &lt;message&gt;" on stderr, exit code 3.
/// </summary>
public static class Program
{
    private static StreamWriter Out;
    private static int CurDay;
    private static string CurLabel = "init";

    public static int Main(string[] args)
    {
        Out = new StreamWriter(Console.OpenStandardOutput(), new UTF8Encoding(false), 1 << 16) { NewLine = "\n" };
        try
        {
            D.Init();
            Fault = ParseFault(Environment.GetEnvironmentVariable("GOLDEN_FAULT"));
            return Run(args);
        }
        catch (UsageException e)
        {
            Flush();
            Console.Error.WriteLine(e.Message);
            return 2;
        }
        catch (Exception e)
        {
            Flush();
            var x = e;
            while ((x is TargetInvocationException || x is TypeInitializationException) && x.InnerException != null) x = x.InnerException;
            Console.Error.WriteLine($"CRASH {CurDay} {CurLabel} {x.GetType().Name}: {FirstLine(x.Message)}");
            Console.Error.WriteLine(x.ToString());
            return 3;
        }
        finally { Flush(); }
    }

    private static void Flush() { try { Out.Flush(); } catch (IOException) { } }
    private static string FirstLine(string s) { int i = s.IndexOf('\n'); return (i < 0 ? s : s.Substring(0, i)).TrimEnd('\r'); }

    internal sealed class UsageException : Exception { public UsageException(string m) : base(m) { } }
    private sealed class StopException : Exception { }

    private static int Run(string[] a)
    {
        string mode = a.Length > 0 ? a[0] : "";
        switch (mode)
        {
            case "hash": ModeHash(Seed(a, 1), Day(a, 2)); return 0;
            case "cps": ModeCps(Seed(a, 1), Day(a, 2)); return 0;
            case "dump": ModeDump(Seed(a, 1), Day(a, 2), a.Length > 3 ? a[3] : null); return 0;
            case "rng": ModeRng(Seed(a, 1), Day(a, 2)); return 0;
            case "selftest": ModeSelfTest(); return 0;
            case "bench": ModeBench(Seed(a, 1), a.Length > 2 ? Day(a, 2) : 1800); return 0;
            case "stats": return StatsMode.Run(a, Out);
            default:
                throw new UsageException("usage: FD.Macro.Run hash <seed> <days> | cps <seed> <day> | dump <seed> <day> [label] | rng <seed> <day> | selftest | bench <seed> [days]\n"
                    + "       FD.Macro.Run " + StatsMode.Usage);
        }
    }

    private static double Seed(string[] a, int i)
    {
        if (a.Length <= i || !double.TryParse(a[i], NumberStyles.Float, CultureInfo.InvariantCulture, out double x) || double.IsNaN(x))
            throw new UsageException($"bad seed: {(a.Length > i ? a[i] : "(missing)")}");
        return x;
    }

    private static int Day(string[] a, int i)
    {
        if (a.Length <= i || !int.TryParse(a[i], NumberStyles.None, CultureInfo.InvariantCulture, out int d))
            throw new UsageException($"bad day: {(a.Length > i ? a[i] : "(missing)")}");
        return d;
    }

    // ================================================================ modes
    // Harness self-test only: GOLDEN_FAULT=<kind>@<day>:<label> perturbs the C# run at that checkpoint
    // (crash: NotImplementedException; rng: one extra Rng.Next(); state: adds metrics.goldenFault; num: seed += 0.5).
    // crash@0 throws right after new Sim().
    private static string[] Fault;           // set in Main

    private static string[] ParseFault(string f)
    {
        if (string.IsNullOrEmpty(f)) return null;
        int at = f.IndexOf('@'), colon = f.IndexOf(':', at + 1);
        if (at < 0) throw new UsageException("GOLDEN_FAULT: <crash|rng|state|num>@<day>[:<label>]");
        string day = colon < 0 ? f.Substring(at + 1) : f.Substring(at + 1, colon - at - 1);
        return new[] { f.Substring(0, at), day, colon >= 0 ? f.Substring(colon + 1) : day == "0" ? "init" : "end" };
    }

    private static void InjectFault(Sim sim, string label)
    {
        if (Fault == null || label != Fault[2] || CurDay.ToString(CultureInfo.InvariantCulture) != Fault[1]) return;
        switch (Fault[0])
        {
            case "crash": throw new NotImplementedException("GOLDEN_FAULT injected crash");
            case "rng": sim.Rng.Next(); break;
            case "state": sim.W.Metrics.Set("goldenFault", 1); break;
            case "num": sim.W.Seed += 0.5; break;
            default: throw new UsageException($"GOLDEN_FAULT: unknown kind {Fault[0]}");
        }
    }

    /// <summary>Installs the checkpoint hook: tracks the label (crash reports), applies GOLDEN_FAULT, then calls <paramref name="then"/>.</summary>
    private static void Hook(Sim sim, Action<string> then = null)
    {
        sim.Cp = l => { CurLabel = l; InjectFault(sim, l); then?.Invoke(l); };
    }

    private static Sim NewSim(double seed)
    {
        CurDay = 0; CurLabel = "init";
        var sim = new Sim(seed);
        InjectFault(sim, "init");
        return sim;
    }

    /// <summary>Days 1..d-1 (checkpoints only tracked); CurDay = d afterwards.</summary>
    private static void RunTo(Sim sim, int d)
    {
        Hook(sim);
        for (int x = 1; x < d; x++) { CurDay = x; CurLabel = "start"; sim.Step(); }
        CurDay = d; CurLabel = "start";
    }

    private static string State(Sim sim) => $"{Canon.Hash(sim.W)} {JsMath.Str(sim.Rng.State())} {sim.Rng.Calls.ToString(CultureInfo.InvariantCulture)}";

    private static void ModeHash(double seed, int days)
    {
        var sim = NewSim(seed);
        void Line(int d)
        {
            Out.Write($"{d} {State(sim)} {sim.PathCacheSize} {sim.NavCache.Count}\n");
            Out.Flush();
        }
        Line(0);
        Hook(sim);
        for (int d = 1; d <= days; d++) { CurDay = d; CurLabel = "start"; sim.Step(); Line(d); }
    }

    private static void ModeCps(double seed, int d)
    {
        var sim = NewSim(seed);
        if (d == 0) { Out.Write($"init {State(sim)}\n"); return; }
        RunTo(sim, d);
        Hook(sim, l => Out.Write($"{l} {State(sim)}\n"));
        sim.Step();
    }

    /// <summary>
    /// labels: one checkpoint label of day d (default "end"), or a comma-separated list (then each listing is preceded
    /// by a line "@label"). "start" = before the step of day d (the end of day d-1); day 0 only has init (= start = end).
    /// </summary>
    private static void ModeDump(double seed, int d, string labels)
    {
        var want = new List<string>((labels ?? "end").Split(','));
        var got = new Dictionary<string, List<string>>();
        var sim = NewSim(seed);
        if (d == 0)
        {
            foreach (var l in want)
                if (l != "init" && l != "start" && l != "end") throw new UsageException($"day 0 has no checkpoint '{l}' (only init)");
            var flat = Canon.Flat(sim.W);
            foreach (var l in want) got[l] = flat;
        }
        else
        {
            RunTo(sim, d);
            if (want.Contains("start")) got["start"] = Canon.Flat(sim.W);
            int n = new HashSet<string>(want).Count;
            if (got.Count < n)
            {
                Hook(sim, l =>
                {
                    if (!want.Contains(l) || got.ContainsKey(l)) return;
                    got[l] = Canon.Flat(sim.W);
                    if (got.Count == n) throw new StopException();
                });
                try { sim.Step(); } catch (StopException) { }
            }
            foreach (var l in want)
                if (!got.ContainsKey(l)) throw new UsageException($"checkpoint '{l}' not reached on day {d}");
        }
        foreach (var l in want)
        {
            if (want.Count > 1) Out.Write($"@{l}\n");
            foreach (var line in got[l]) { Out.Write(line); Out.Write('\n'); }
        }
    }

    private static void ModeRng(double seed, int d)
    {
        if (d < 1) throw new UsageException("rng: day must be >= 1 (the RNG calls of new Sim() cannot be traced on the C# side)");
        var sim = NewSim(seed);
        RunTo(sim, d);
        string label = "start";
        Hook(sim, l => label = l);
        sim.Rng.Trace = v => Out.Write($"{sim.Rng.Calls.ToString(CultureInfo.InvariantCulture)} {label} {JsMath.Str(v)} {CallerName()}\n");
        sim.Step();
        sim.Rng.Trace = null;
    }

    private static void ModeBench(double seed, int days)
    {
        var sw = Stopwatch.StartNew();
        var sim = NewSim(seed);
        Out.WriteLine($"new Sim: {sw.ElapsedMilliseconds} ms"); Out.Flush();
        sw.Restart();
        long hashTicks = 0;
        for (int d = 1; d <= days; d++)
        {
            CurDay = d; sim.Step();
            long t0 = Stopwatch.GetTimestamp(); Canon.Hash(sim.W); hashTicks += Stopwatch.GetTimestamp() - t0;
        }
        double hashMs = hashTicks * 1000.0 / Stopwatch.Frequency;
        Out.WriteLine($"{days} x (Step + world hash): {sw.ElapsedMilliseconds} ms, of which hashing {hashMs:0} ms");
        sw.Restart();
        for (int i = 0; i < 20; i++) Canon.Hash(sim.W);
        Out.WriteLine($"final world: {Canon.Text(sim.W).Length} canonical chars, hash {sw.Elapsed.TotalMilliseconds / 20:0.0} ms each");
    }

    // Test vectors shared with trace.ts (`selftest` must print the same lines on both sides).
    private static void ModeSelfTest()
    {
        string[] strs = { "", "a", "foobar", "Dünya uyandı. İğne ışık Çağı ÖŞÜ", "q\"b\\s/\n\t\u0001\u001f\u007f", "😀 \ud800x", "0", "{\"a\":1}" };
        foreach (var s in strs) Out.Write($"str {Canon.StrHash(s)} {Canon.Hash(s)} {Canon.Quote(s)}\n");
        double[] nums = { 0, -0.0, 1, -1, 7, 10, 99, 100, 2147483647, -2147483648, 2147483648, 4294967296, 1e21, 1e-7, 123456789012345680000.0,
            0.1, 0.30000000000000004, -1.5, 5e-324, 1.7976931348623157e308, double.NaN, double.PositiveInfinity, double.NegativeInfinity, 1.0 / 3,
            9007199254740992.0, -9007199254740992.0 - 2, 0.000001, 1e-6 * 3 };
        foreach (var x in nums) Out.Write($"num {Canon.Hash(x)} {Canon.NumStr(x)}\n");
        var inner = new JsObj<object>();
        inner.Set("z", true); inner.Set("y", false); inner.Set("_skip", 5.0);
        var obj = new JsObj<object>();
        obj.Set("b", 1.0);
        obj.Set("a", new List<object> { 1.0, null, null, "x", inner });
        obj.Set("c", null); obj.Set("d", null);
        obj.Set("é", new JsObj<object>()); obj.Set("B", new List<object>());
        obj.Set("10", 1.0); obj.Set("9", 2.0); obj.Set("k\"q", -0.0);
        Out.Write($"obj {Canon.Hash(obj)} {Canon.Text(obj)}\n");
        foreach (var l in Canon.Flat(obj)) Out.Write($"flat {l}\n");
    }

    // ================================================================ RNG caller (informational)
    /// <summary>First simulation method on the stack above the RNG (Rng, harness, System.* and J/Js* helpers skipped;
    /// lambdas → their containing method, local functions → their own name, iterators → the iterator method).
    /// JIT inlining can hide small methods (then the caller's caller is reported).</summary>
    private static string CallerName()
    {
        var st = new StackTrace(1, false);
        for (int i = 0; i < st.FrameCount; i++)
        {
            MethodBase m = st.GetFrame(i)?.GetMethod();
            Type t = m?.DeclaringType;
            if (t == null) continue;
            if (t == typeof(Rng) || t.Assembly == typeof(Program).Assembly) continue;
            string ns = t.Namespace ?? "";
            if (ns == "System" || ns.StartsWith("System.", StringComparison.Ordinal)) continue;
            Type top = t;
            while (top.DeclaringType != null) top = top.DeclaringType;
            if (top == typeof(J) || top == typeof(JsSort) || top == typeof(JsMath) || top.Name.StartsWith("JsObj", StringComparison.Ordinal) || top.Name.StartsWith("JsNumObj", StringComparison.Ordinal) || top.Name.StartsWith("JsMap", StringComparison.Ordinal)) continue;
            string name = m.Name;
            if (name.StartsWith("<", StringComparison.Ordinal))
            {
                int gt = name.IndexOf('>');
                if (gt > 1 && string.CompareOrdinal(name, gt + 1, "g__", 0, 3) == 0)
                {
                    int bar = name.IndexOf('|', gt);
                    name = bar > 0 ? name.Substring(gt + 4, bar - gt - 4) : name.Substring(gt + 4);       // local function
                }
                else name = gt > 1 ? name.Substring(1, gt - 1) : name;                                      // lambda
            }
            else if (t.Name.StartsWith("<", StringComparison.Ordinal) && t.Name.IndexOf('>') > 1 && (name == "MoveNext" || name == "Invoke"))
                name = t.Name.Substring(1, t.Name.IndexOf('>') - 1);                                        // iterator / closure class
            if (name.StartsWith(".ctor", StringComparison.Ordinal)) name = t.Name;
            if (name.Length == 0) continue;
            return name;
        }
        return "?";
    }
}

// ==================================================================== stats: headless ölçüm (Faz 1 A1)
/// <summary>
/// Headless ölçüm aracı (DESIGN-FAZ1.md A1). Dünyaları paralel koşar (iş parçacıkları; simülasyonda statik değişken
/// durum yok), her birine <see cref="WorldStats"/> bağlar ve yazar:
/// <list type="bullet">
/// <item><c>&lt;runs&gt;/seed-N.json</c>: dünya başına yıllık değerler (varsayılan <c>&lt;out&gt;/worlds</c>);</item>
/// <item><c>&lt;out&gt;/report.json</c>, <c>&lt;out&gt;/report.md</c>: yıllık medyan/p10/p90, on yıllık özet, bitiş ölçütleri (✓/✗).</item>
/// </list>
/// Denetimler: <c>--verify N</c> ilk N seed'i toplayıcısız yeniden koşar ve yıl sonu hash'lerini karşılaştırır (determinizm +
/// toplayıcının salt okunurluğu); <c>--saveload N</c> Sim.Save(string)/Sim.Load(string) varsa (A2) yarıda kaydet → yükle → devam
/// eder ve hash'leri karşılaştırır. Çıkış kodu: 0; bir dünya çökerse 3 (raporlar yine yazılır).
/// </summary>
internal static class StatsMode
{
    public const string Usage = "stats --seeds 1-16 --years 60 --out <dir> [--jobs N] [--runs <dir>] [--label <text>] [--verify N] [--saveload N]";

    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private static readonly object Gate = new();
    private const int YEAR = Sim.YEAR;

    private sealed class Opts
    {
        public string SeedsText = "1-16", Out, Runs, Label;
        public List<double> Seeds;
        public int Years = 60, Jobs = Math.Max(1, Environment.ProcessorCount), Verify = 1, SaveLoad = 1;
    }

    private sealed class WorldRun
    {
        public double Seed;
        public WorldStats Stats;
        public readonly List<string> YearHashes = new();
        public string FinalHash, Error, Stack, JsonPath;
        public int ErrorDay;
        public double Sec;
        public bool Ok => Stats != null && Error == null;
    }

    private sealed class CheckRun
    {
        public string Kind;                                  // "verify" | "saveload"
        public double Seed;
        public int SaveDay;
        public readonly List<string> YearHashes = new();     // [yıl - 1]; saveload: kayıttan önceki yıllar null
        public string Error, HashBeforeSave, HashAfterLoad;
        public double Sec;
    }

    public static int Run(string[] a, TextWriter stdout)
    {
        var o = Parse(a);
        var wall = Stopwatch.StartNew();
        Directory.CreateDirectory(o.Out);
        Directory.CreateDirectory(o.Runs);
        var runs = o.Seeds.Select(s => new WorldRun { Seed = s }).ToList();
        var checks = new List<CheckRun>();
        for (int i = 0; i < Math.Min(o.Verify, runs.Count); i++) checks.Add(new CheckRun { Kind = "verify", Seed = runs[i].Seed });
        var (save, load) = SaveLoadApi();
        if (save != null)
            for (int i = 0; i < Math.Min(o.SaveLoad, runs.Count); i++)
                checks.Add(new CheckRun { Kind = "saveload", Seed = runs[i].Seed, SaveDay = (o.Years / 2) * YEAR + 37 });
        int done = 0, total = runs.Count + checks.Count;
        var jobs = new List<Action>();
        foreach (var r in runs)
            jobs.Add(() =>
            {
                RunWorld(r, o);
                Say($"[{Interlocked.Increment(ref done)}/{total}] seed {S(r.Seed)}: {(r.Error != null ? $"HATA gün {r.ErrorDay}: {r.Error}" : $"{o.Years} yıl, {r.Sec.ToString("0.0", Inv)} sn, hash {r.FinalHash}")}");
            });
        foreach (var c in checks)
            jobs.Add(() =>
            {
                if (c.Kind == "verify") RunVerify(c, o); else RunSaveLoad(c, o, save, load);
                Say($"[{Interlocked.Increment(ref done)}/{total}] denetim {(c.Kind == "verify" ? "toplayıcısız tekrar" : $"kaydet/yükle (gün {c.SaveDay})")} seed {S(c.Seed)}: {(c.Error ?? $"{c.Sec.ToString("0.0", Inv)} sn")}");
            });
        Say($"stats: {runs.Count} dünya × {o.Years} yıl, {o.Jobs} iş parçacığı{(checks.Count > 0 ? $", +{checks.Count} denetim koşusu" : "")}; çıktı {o.Out}");
        RunParallel(jobs, o.Jobs);
        wall.Stop();
        var rep = new Report(o, runs, checks, save != null, wall.Elapsed.TotalSeconds);
        var utf8 = new UTF8Encoding(false);
        File.WriteAllText(Path.Combine(o.Out, "report.json"), StatsJson.Write(rep.Json()), utf8);
        File.WriteAllText(Path.Combine(o.Out, "report.md"), rep.Markdown(), utf8);
        stdout.Write(rep.Summary());
        stdout.Flush();
        return runs.All(r => r.Ok) ? 0 : 3;
    }

    private static void Say(string s) { lock (Gate) Console.Error.WriteLine(s); }

    private static string S(double seed) => JsMath.Str(seed);

    private static string FirstLine(string s) { int i = s.IndexOf('\n'); return (i < 0 ? s : s.Substring(0, i)).TrimEnd('\r'); }

    // ---------------------------------------------------------------- arguments
    private static Opts Parse(string[] a)
    {
        var o = new Opts();
        for (int i = 1; i < a.Length; i++)
        {
            string k = a[i];
            string Next() => i + 1 < a.Length ? a[++i] : throw new Program.UsageException($"stats: {k} bir değer ister\nusage: FD.Macro.Run {Usage}");
            switch (k)
            {
                case "--seeds": o.SeedsText = Next(); break;
                case "--years": o.Years = Int(Next(), k, 1); break;
                case "--out": o.Out = Next(); break;
                case "--runs": o.Runs = Next(); break;
                case "--jobs": o.Jobs = Int(Next(), k, 1); break;
                case "--label": o.Label = Next(); break;
                case "--verify": o.Verify = Int(Next(), k, 0); break;
                case "--saveload": o.SaveLoad = Int(Next(), k, 0); break;
                default: throw new Program.UsageException($"stats: bilinmeyen seçenek {k}\nusage: FD.Macro.Run {Usage}");
            }
        }
        if (string.IsNullOrEmpty(o.Out)) throw new Program.UsageException($"stats: --out gerekli\nusage: FD.Macro.Run {Usage}");
        o.Seeds = ParseSeeds(o.SeedsText);
        o.Runs ??= Path.Combine(o.Out, "worlds");
        o.Label ??= Path.GetFileName(Path.TrimEndingDirectorySeparator(Path.GetFullPath(o.Out)));
        return o;
    }

    private static int Int(string s, string k, int min)
    {
        if (!int.TryParse(s, NumberStyles.None, Inv, out int v) || v < min) throw new Program.UsageException($"stats: {k} için geçersiz değer: {s}");
        return v;
    }

    /// <summary>"1-16", "1,3,5-8", "42": aralıklar tam sayı, tekil seed'ler herhangi bir sayı; tekrarlar atılır.</summary>
    private static List<double> ParseSeeds(string s)
    {
        var o = new List<double>();
        foreach (var part0 in s.Split(','))
        {
            string part = part0.Trim();
            if (part.Length == 0) continue;
            int dash = part.IndexOf('-', 1);
            if (dash > 0 && int.TryParse(part.Substring(0, dash), NumberStyles.AllowLeadingSign, Inv, out int lo) && int.TryParse(part.Substring(dash + 1), NumberStyles.AllowLeadingSign, Inv, out int hi) && lo <= hi)
            { for (int x = lo; x <= hi; x++) if (!o.Contains(x)) o.Add(x); }
            else if (double.TryParse(part, NumberStyles.Float, Inv, out double v) && !double.IsNaN(v)) { if (!o.Contains(v)) o.Add(v); }
            else throw new Program.UsageException($"stats: geçersiz seed listesi: {s}");
        }
        if (o.Count == 0) throw new Program.UsageException("stats: seed listesi boş");
        return o;
    }

    // ---------------------------------------------------------------- runs
    private static void RunParallel(List<Action> jobs, int n)
    {
        int next = -1;
        var threads = new List<Thread>();
        for (int t = 0; t < Math.Min(n, jobs.Count); t++)
        {
            var th = new Thread(() =>
            {
                for (int i; (i = Interlocked.Increment(ref next)) < jobs.Count;)
                {
                    try { jobs[i](); }
                    catch (Exception e) { Say($"stats: iş {i} beklenmedik hata: {e}"); }
                }
            }, 64 << 20) { Name = "stats-" + t, IsBackground = false };
            th.Start();
            threads.Add(th);
        }
        foreach (var th in threads) th.Join();
    }

    private static string Describe(Exception e)
    {
        var x = e;
        while ((x is TargetInvocationException || x is TypeInitializationException) && x.InnerException != null) x = x.InnerException;
        return $"{x.GetType().Name}: {FirstLine(x.Message)}";
    }

    private static void RunWorld(WorldRun r, Opts o)
    {
        var sw = Stopwatch.StartNew();
        WorldStats st = null;
        int d = 0;
        try
        {
            var sim = new Sim(r.Seed);
            st = new WorldStats(sim);
            int days = o.Years * YEAR;
            for (d = 1; d <= days; d++)
            {
                sim.Step();
                st.AfterStep();
                if (d % YEAR == 0) r.YearHashes.Add(Canon.Hash(sim.W));
            }
            r.FinalHash = r.YearHashes.Count > 0 ? r.YearHashes[r.YearHashes.Count - 1] : Canon.Hash(sim.W);
        }
        catch (Exception e) { r.Error = Describe(e); r.ErrorDay = d; r.Stack = e.ToString(); }
        try { st?.Finish(); } catch (Exception e) { r.Error ??= "Finish: " + Describe(e); }
        r.Stats = st;
        r.Sec = sw.Elapsed.TotalSeconds;
        try { WriteWorldJson(r, o); } catch (Exception e) { Say($"stats: seed {S(r.Seed)} JSON yazılamadı: {Describe(e)}"); }
    }

    private static void RunVerify(CheckRun c, Opts o)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var sim = new Sim(c.Seed);
            for (int d = 1; d <= o.Years * YEAR; d++) { sim.Step(); if (d % YEAR == 0) c.YearHashes.Add(Canon.Hash(sim.W)); }
        }
        catch (Exception e) { c.Error = Describe(e); }
        c.Sec = sw.Elapsed.TotalSeconds;
    }

    private static void RunSaveLoad(CheckRun c, Opts o, MethodInfo save, MethodInfo load)
    {
        var sw = Stopwatch.StartNew();
        string path = Path.Combine(o.Runs, $"saveload-seed-{S(c.Seed)}.sav");
        try
        {
            var sim = new Sim(c.Seed);
            int days = o.Years * YEAR;
            for (int d = 1; d <= c.SaveDay; d++) { sim.Step(); if (d % YEAR == 0) c.YearHashes.Add(null); }
            c.HashBeforeSave = Canon.Hash(sim.W);
            save.Invoke(sim, new object[] { path });
            var sim2 = (Sim)load.Invoke(null, new object[] { path });
            c.HashAfterLoad = Canon.Hash(sim2.W);
            for (int d = c.SaveDay + 1; d <= days; d++) { sim2.Step(); if (d % YEAR == 0) c.YearHashes.Add(Canon.Hash(sim2.W)); }
        }
        catch (Exception e) { c.Error = Describe(e); }
        finally { try { File.Delete(path); } catch (IOException) { } catch (UnauthorizedAccessException) { } }
        c.Sec = sw.Elapsed.TotalSeconds;
    }

    /// <summary>A2'nin <c>Sim.Save(string)</c> / <c>static Sim Sim.Load(string)</c> yöntemleri (henüz yoksa null).</summary>
    private static (MethodInfo Save, MethodInfo Load) SaveLoadApi()
    {
        var save = typeof(Sim).GetMethod("Save", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(string) }, null);
        var load = typeof(Sim).GetMethod("Load", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null);
        if (save == null || load == null || !typeof(Sim).IsAssignableFrom(load.ReturnType)) return (null, null);
        return (save, load);
    }

    private static void WriteWorldJson(WorldRun r, Opts o)
    {
        JObj j;
        if (r.Stats != null)
        {
            j = r.Stats.ToJson();
            j.InsertRange(1, new JObj
            {
                { "label", o.Label }, { "ok", r.Ok }, { "error", r.Error }, { "errorDay", r.Error != null ? (object)r.ErrorDay : null }, { "sec", r.Sec },
                { "finalHash", r.FinalHash },
            });
        }
        else j = new JObj { { "seed", r.Seed }, { "label", o.Label }, { "ok", false }, { "error", r.Error }, { "errorDay", r.ErrorDay }, { "sec", r.Sec } };
        j.Add("yearHashes", r.YearHashes);
        if (r.Stack != null) j.Add("stack", r.Stack);
        r.JsonPath = Path.Combine(o.Runs, $"seed-{S(r.Seed)}.json");
        File.WriteAllText(r.JsonPath, StatsJson.Write(j), new UTF8Encoding(false));
    }

    // ---------------------------------------------------------------- report
    private sealed class Crit
    {
        public string Id, Name, Rule, Measured;
        public bool? Pass;          // null: ölçülemedi
        public JObj Data = new();
    }

    private sealed class Known
    {
        public string Name, Old, Now;
        public bool? Persists;
    }

    private struct Dst
    {
        public double Med, P10, P90;
        public int N;
    }

    private sealed class Report
    {
        private readonly Opts O;
        private readonly List<WorldRun> All, Ok;
        private readonly List<CheckRun> Checks;
        private readonly bool HasSaveLoad;
        private readonly double Wall;
        private readonly int Y;
        private readonly List<(int From, int To)> Dec = new();
        private readonly List<Crit> Crits = new();
        private readonly List<Known> KnownRows = new();
        private readonly string Created = DateTime.Now.ToString("yyyy-MM-dd HH:mm", Inv);

        public Report(Opts o, List<WorldRun> all, List<CheckRun> checks, bool hasSaveLoad, double wall)
        {
            O = o; All = all; Ok = all.Where(r => r.Ok).ToList(); Checks = checks; HasSaveLoad = hasSaveLoad; Wall = wall;
            Y = Ok.Count > 0 ? Ok.Min(r => r.Stats.Years.Count(y => !y.Partial)) : 0;
            for (int f = 1; f <= Y; f += 10) Dec.Add((f, Math.Min(f + 9, Y)));
            if (Ok.Count > 0) { Evaluate(); KnownProblems(); }
        }

        // ------------------------------------------------------------ statistics
        private static int K(string key)
        {
            int k = WorldStats.IndexOf(key);
            if (k < 0) throw new InvalidOperationException("unknown stat " + key);
            return k;
        }

        private static double Q(List<double> xs, double q)
        {
            double h = (xs.Count - 1) * q;
            int lo = (int)Math.Floor(h), hi = Math.Min(lo + 1, xs.Count - 1);
            return xs[lo] + (h - lo) * (xs[hi] - xs[lo]);
        }

        /// <summary>medyan, p10, p90 (doğrusal ara değer; NaN'lar atılır).</summary>
        private static Dst Dist(IEnumerable<double> src)
        {
            var xs = src.Where(x => !double.IsNaN(x)).ToList();
            if (xs.Count == 0) return new Dst { Med = double.NaN, P10 = double.NaN, P90 = double.NaN, N = 0 };
            xs.Sort();
            return new Dst { Med = Q(xs, 0.5), P10 = Q(xs, 0.1), P90 = Q(xs, 0.9), N = xs.Count };
        }

        private static double Median(IEnumerable<double> src) => Dist(src).Med;

        private static double Val(WorldRun r, int year, int k) => year >= 1 && year <= r.Stats.Years.Count ? r.Stats.Years[year - 1].V[k] : double.NaN;

        private Dst Yearly(int k, int year) => Dist(Ok.Select(r => Val(r, year, k)));

        private static double WorldMean(WorldRun r, int k, int from, int to)
        {
            double s = 0; int n = 0;
            for (int y = from; y <= to; y++) { double v = Val(r, y, k); if (!double.IsNaN(v)) { s += v; n++; } }
            return n > 0 ? s / n : double.NaN;
        }

        private Dst Decade(int k, (int From, int To) d) => Dist(Ok.Select(r => WorldMean(r, k, d.From, d.To)));

        /// <summary>Aralıktaki bütün (dünya, yıl) değerlerinin medyanı.</summary>
        private double Pooled(int k, int from, int to) => Median(Ok.SelectMany(r => Enumerable.Range(from, Math.Max(0, to - from + 1)).Select(y => Val(r, y, k))));

        private static double MapVal(WorldRun r, int year, string map, string key)
        {
            if (year < 1 || year > r.Stats.Years.Count) return double.NaN;
            var m = r.Stats.Years[year - 1].Map(map);
            return m != null && m.TryGetValue(key, out double v) ? v : 0;
        }

        private List<string> MapKeys(string map)
        {
            var set = new SortedSet<string>(WorldStats.KeyOrder);
            foreach (var r in Ok) foreach (var y in r.Stats.Years) { var m = y.Map(map); if (m != null) foreach (var k in m.Keys) set.Add(k); }
            return set.ToList();
        }

        private Dst MapDecade(string map, string key, (int From, int To) d) =>
            Dist(Ok.Select(r => Enumerable.Range(d.From, d.To - d.From + 1).Select(y => MapVal(r, y, map, key)).Where(v => !double.IsNaN(v)).DefaultIfEmpty(double.NaN).Average()));

        /// <summary>Aralıktaki bütün dünya ve yılların sayımlarının toplamı.</summary>
        private SortedDictionary<string, double> PoolMap(string map, int from, int to)
        {
            var o = new SortedDictionary<string, double>(WorldStats.KeyOrder);
            foreach (var r in Ok)
                for (int y = from; y <= to && y <= r.Stats.Years.Count; y++)
                {
                    var m = r.Stats.Years[y - 1].Map(map);
                    if (m != null) foreach (var kv in m) o[kv.Key] = (o.TryGetValue(kv.Key, out double x) ? x : 0) + kv.Value;
                }
            return o;
        }

        private static (double N, double Mean) LevelMean(SortedDictionary<string, double> m)
        {
            double n = 0, s = 0;
            foreach (var kv in m) if (int.TryParse(kv.Key, NumberStyles.None, Inv, out int lv)) { n += kv.Value; s += lv * kv.Value; }
            return (n, n > 0 ? s / n : double.NaN);
        }

        private static string Range((int From, int To) d) => d.From == d.To ? $"{d.From}" : $"{d.From}–{d.To}";

        /// <summary>Yol, rapor klasörüne göre (rapor dosyasından tıklanabilir, makineden bağımsız).</summary>
        private string Rel(string path) => Path.GetRelativePath(Path.GetFullPath(O.Out), Path.GetFullPath(path)).Replace('\\', '/');

        // ------------------------------------------------------------ formatting (Türkçe: ondalık virgül)
        internal static string F(double x)
        {
            if (double.IsNaN(x) || double.IsInfinity(x)) return "–";
            double a = Math.Abs(x);
            string s = a >= 100 ? Math.Round(x).ToString("0", Inv) : a >= 10 ? x.ToString("0.#", Inv) : x.ToString("0.##", Inv);
            if (s == "-0") s = "0";
            return s.Replace('.', ',');
        }

        internal static string Pct(double x)
        {
            if (double.IsNaN(x) || double.IsInfinity(x)) return "–";
            double p = x * 100;
            return "%" + (Math.Abs(p) >= 10 || p == Math.Round(p) ? Math.Round(p).ToString("0", Inv) : p.ToString("0.#", Inv)).Replace('.', ',');
        }

        private static string Fmt(StatDef d, double x) => d.Share ? Pct(x) : F(x);

        private static string Cell(StatDef d, Dst t)
        {
            if (t.N == 0) return "–";
            string m = Fmt(d, t.Med), lo = Fmt(d, t.P10), hi = Fmt(d, t.P90);
            return lo == hi && lo == m ? m : $"{m} ({lo}–{hi})";
        }

        private static string Mark(bool? p) => p == null ? "—" : p.Value ? "✓" : "✗";

        // ------------------------------------------------------------ exit criteria (DESIGN-FAZ1.md "Bitiş ölçütleri")
        private void Evaluate()
        {
            int n = Ok.Count;
            int lateTo = Math.Min(60, Y);
            // 1. Donma yok
            {
                var c = new Crit { Id = "1", Name = "Donma yok", Rule = "41–60. yılların yıllık büyük olay medyanı ≥ 0,8 × 6–20. yılların medyanı" };
                if (Y >= 41)
                {
                    double late = Pooled(K("majorEvents"), 41, lateTo), early = Pooled(K("majorEvents"), 6, 20);
                    c.Pass = late >= 0.8 * early;
                    c.Measured = $"{F(late)} / {F(early)} = {F(late / early)} kat";
                    c.Data = new JObj { { "late", late }, { "early", early }, { "ratio", late / early }, { "lateYears", $"41-{lateTo}" } };
                }
                else c.Measured = $"ölçülemedi: koşu {Y} yıl (41. yıl gerekir)";
                Crits.Add(c);
            }
            // 2. Çöküş
            {
                var c = new Crit { Id = "2", Name = "Çöküş", Rule = $"dünyaların ≥ %75'inde {Math.Min(60, Y)} yılda ≥ 1 çöküş (yok olma ya da başkent kaybı)" };
                int with = Ok.Count(r => r.Stats.CollapseLog.Count(x => x.Year <= 60) > 0);
                int ext = Ok.Sum(r => r.Stats.CollapseLog.Count(x => x.Kind == "extinct" && x.Year <= 60)), cap = Ok.Sum(r => r.Stats.CollapseLog.Count(x => x.Kind == "capital" && x.Year <= 60));
                double share = with / (double)n;
                c.Pass = share >= 0.75;
                c.Measured = $"{Pct(share)} ({with}/{n} dünya); toplam {ext + cap} çöküş: {ext} yok olma, {cap} başkent kaybı";
                c.Data = new JObj { { "share", share }, { "worlds", with }, { "n", n }, { "extinct", ext }, { "capitalLost", cap } };
                Crits.Add(c);
            }
            // 3. Kamplar
            {
                var c = new Crit { Id = "3", Name = "Kamplar", Rule = "41–60. yıllarda yaşayan kamp medyanı ≥ 6–20. yılların medyanı" };
                if (Y >= 41)
                {
                    double late = Pooled(K("campsAliveAvg"), 41, lateTo), early = Pooled(K("campsAliveAvg"), 6, 20);
                    double lateEnd = Pooled(K("campsAlive"), 41, lateTo), earlyEnd = Pooled(K("campsAlive"), 6, 20);
                    c.Pass = late >= early;
                    c.Measured = $"{F(late)} {(late >= early ? "≥" : "<")} {F(early)} (yıl sonu sayımıyla {F(lateEnd)} / {F(earlyEnd)})";
                    c.Data = new JObj { { "late", late }, { "early", early }, { "lateYearEnd", lateEnd }, { "earlyYearEnd", earlyEnd } };
                }
                else c.Measured = $"ölçülemedi: koşu {Y} yıl";
                Crits.Add(c);
            }
            // 4a. doğuş seviyesi
            {
                var c = new Crit { Id = "4a", Name = "Kahraman: doğuş seviyesi", Rule = "her on yılda doğanların ortalama seviyesi ≤ 2" };
                var parts = new List<string>(); var arr = new List<object>();
                bool ok = true; int decs = 0;
                foreach (var d in Dec)
                {
                    var (cnt, mean) = LevelMean(PoolMap("levelBirth", d.From, d.To));
                    arr.Add(new JObj { { "years", Range(d) }, { "n", cnt }, { "mean", mean } });
                    if (cnt <= 0) continue;
                    decs++;
                    parts.Add(F(mean));
                    if (mean > 2) ok = false;
                }
                c.Pass = decs > 0 ? ok : null;
                c.Measured = decs > 0 ? string.Join(" · ", parts) + " (on yıllar sırasıyla)" : "doğan kahraman yok";
                c.Data = new JObj { { "decades", arr } };
                Crits.Add(c);
            }
            // 4b. Sv8+
            {
                var c = new Crit { Id = "4b", Name = "Kahraman: Sv8+", Rule = "dünyaların ≥ yarısında en az bir kahraman Sv8 ve üstüne çıkar" };
                int with = Ok.Count(r => r.Stats.MaxHeroLevel >= 8);
                double share = with / (double)n;
                var mx = Dist(Ok.Select(r => (double)r.Stats.MaxHeroLevel));
                c.Pass = share >= 0.5;
                c.Measured = $"{Pct(share)} ({with}/{n} dünya); dünyadaki en yüksek seviye: medyan Sv{F(mx.Med)}, en çok Sv{Ok.Max(r => r.Stats.MaxHeroLevel)}";
                c.Data = new JObj { { "share", share }, { "worlds", with }, { "maxLevelMedian", mx.Med }, { "maxLevel", Ok.Max(r => r.Stats.MaxHeroLevel) } };
                Crits.Add(c);
            }
            // 4c. efsane
            {
                var c = new Crit { Id = "4c", Name = "Kahraman: efsane", Rule = "dünya başına efsane medyanı 1–6" };
                var lg = Dist(Ok.Select(r => (double)r.Stats.LegendList.Count));
                c.Pass = lg.Med >= 1 && lg.Med <= 6;
                c.Measured = $"medyan {F(lg.Med)} (p10–p90: {F(lg.P10)}–{F(lg.P90)}; toplam {Ok.Sum(r => r.Stats.LegendList.Count)})";
                c.Data = new JObj { { "median", lg.Med }, { "p10", lg.P10 }, { "p90", lg.P90 }, { "perWorld", Ok.Select(r => (double)r.Stats.LegendList.Count).ToList() } };
                Crits.Add(c);
            }
            // 4d. ölüm payı
            {
                var c = new Crit { Id = "4d", Name = "Kahraman: ölüm payı", Rule = "doğan kahramanların %30–80'i ölür" };
                int born = Ok.Sum(r => r.Stats.HeroesBornTotal), dead = Ok.Sum(r => r.Stats.HeroesBornDead);
                double share = born > 0 ? dead / (double)born : double.NaN;
                var per = Dist(Ok.Select(r => r.Stats.HeroesBornTotal > 0 ? r.Stats.HeroesBornDead / (double)r.Stats.HeroesBornTotal : double.NaN));
                c.Pass = born > 0 ? share >= 0.3 && share <= 0.8 : null;
                c.Measured = $"{Pct(share)} ({dead}/{born}); dünya medyanı {Pct(per.Med)} ({Pct(per.P10)}–{Pct(per.P90)})";
                c.Data = new JObj { { "share", share }, { "dead", dead }, { "born", born }, { "worldMedian", per.Med } };
                Crits.Add(c);
            }
            // 5a. determinizm
            {
                var c = new Crit { Id = "5a", Name = "Determinizm", Rule = "aynı seed → aynı tarih (toplayıcılı ve toplayıcısız koşu, her yıl sonu hash'i)" };
                var vs = Checks.Where(x => x.Kind == "verify").ToList();
                if (vs.Count == 0) c.Measured = "denetlenmedi (--verify 0)";
                else
                {
                    var parts = new List<string>(); bool ok = true; var arr = new List<object>();
                    foreach (var v in vs)
                    {
                        var main = All.First(r => r.Seed == v.Seed);
                        int cmp = Math.Min(main.YearHashes.Count, v.YearHashes.Count), same = 0, first = -1;
                        for (int i = 0; i < cmp; i++) { if (main.YearHashes[i] == v.YearHashes[i]) same++; else if (first < 0) first = i + 1; }
                        bool good = v.Error == null && main.Error == null && cmp == Y && same == cmp;
                        ok &= good;
                        parts.Add(v.Error != null ? $"seed {S(v.Seed)}: HATA {v.Error}" : $"seed {S(v.Seed)}: {same}/{cmp} yıl sonu aynı{(first > 0 ? $" (ilk fark {first}. yıl)" : "")}");
                        arr.Add(new JObj { { "seed", v.Seed }, { "compared", cmp }, { "same", same }, { "firstDiffYear", first > 0 ? (object)first : null }, { "error", v.Error } });
                    }
                    c.Pass = ok;
                    c.Measured = string.Join("; ", parts);
                    c.Data = new JObj { { "runs", arr } };
                }
                Crits.Add(c);
            }
            // 5b. kayıt/yükleme
            {
                var c = new Crit { Id = "5b", Name = "Kayıt/yükleme", Rule = "kaydet → yükle → devam = kesintisiz koşu (her yıl sonu hash'i)" };
                var ss = Checks.Where(x => x.Kind == "saveload").ToList();
                if (!HasSaveLoad) c.Measured = "ölçülemedi: Sim.Save(string) / Sim.Load(string) yok (A2 bekleniyor)";
                else if (ss.Count == 0) c.Measured = "denetlenmedi (--saveload 0)";
                else
                {
                    var parts = new List<string>(); bool ok = true; var arr = new List<object>();
                    foreach (var v in ss)
                    {
                        var main = All.First(r => r.Seed == v.Seed);
                        int cmp = 0, same = 0, first = -1;
                        for (int i = 0; i < Math.Min(main.YearHashes.Count, v.YearHashes.Count); i++)
                        {
                            if (v.YearHashes[i] == null) continue;
                            cmp++;
                            if (main.YearHashes[i] == v.YearHashes[i]) same++; else if (first < 0) first = i + 1;
                        }
                        bool atLoad = v.HashBeforeSave != null && v.HashBeforeSave == v.HashAfterLoad;
                        bool good = v.Error == null && main.Error == null && atLoad && cmp > 0 && same == cmp;
                        ok &= good;
                        parts.Add(v.Error != null ? $"seed {S(v.Seed)}: HATA {v.Error}" : $"seed {S(v.Seed)}, gün {v.SaveDay}: yüklenen dünya {(atLoad ? "aynı" : "FARKLI")}, sonraki {same}/{cmp} yıl sonu aynı{(first > 0 ? $" (ilk fark {first}. yıl)" : "")}");
                        arr.Add(new JObj { { "seed", v.Seed }, { "saveDay", v.SaveDay }, { "sameAtLoad", atLoad }, { "compared", cmp }, { "same", same }, { "firstDiffYear", first > 0 ? (object)first : null }, { "error", v.Error } });
                    }
                    c.Pass = ok;
                    c.Measured = string.Join("; ", parts);
                    c.Data = new JObj { { "runs", arr } };
                }
                Crits.Add(c);
            }
        }

        // ------------------------------------------------------------ eski analizdeki sorunlar (TS v0.23)
        private double PooledShareAt(int year, int kShare, int kWeight)
        {
            double s = 0, w = 0;
            foreach (var r in Ok) { double v = Val(r, year, kShare), n = Val(r, year, kWeight); if (!double.IsNaN(v) && !double.IsNaN(n)) { s += v * n; w += n; } }
            return w > 0 ? s / w : double.NaN;
        }

        private void KnownProblems()
        {
            int y30 = Math.Min(30, Y), y60 = Math.Min(60, Y);
            // araştırma ağacı (Faz 1b-3: kaldırıldı; ilerleme yerleşim kademesinde)
            {
                var civs = Ok.SelectMany(r => r.Stats.Civs.Where(c => c.Founded == 0)).ToList();
                var t2 = civs.Where(c => c.TierDays.Count > 1 && c.TierDays[1] >= 0).Select(c => (double)WorldStats.YearOf((int)c.TierDays[1])).ToList();
                var t3 = civs.Where(c => c.TierDays.Count > 2 && c.TierDays[2] >= 0).Select(c => (double)WorldStats.YearOf((int)c.TierDays[2])).ToList();
                KnownRows.Add(new Known
                {
                    Name = "Araştırma ağacı erken bitiyor",
                    Old = "~19. yılda bitiyor; 30. yılda medeniyetlerin %98'i bitirmiş",
                    Now = $"ağaç ve çağlar kaldırıldı (Faz 1b-3); başlangıç medeniyetlerinin ilk kasabası medyan {F(Median(t2))}. yılda ({t2.Count}/{civs.Count}), ilk şehri {F(Median(t3))}. yılda ({t3.Count}/{civs.Count}); {y30}. yılda başkent kademesi ortalaması {F(Yearly(K("capTierMean"), y30).Med)}",
                    Persists = false,
                });
            }
            // 5 yerleşim (anakara sınırı, diplomacy.ts:223; denizaşırı koloniler ayrı)
            {
                (double Eq, double Ge) Share(SortedDictionary<string, double> m)
                {
                    double tot = m.Values.Sum(), eq = m.TryGetValue("5", out double e) ? e : 0, ge = m.Where(kv => int.TryParse(kv.Key, out int k) && k >= 5).Sum(kv => kv.Value);
                    return tot > 0 ? (eq / tot, ge / tot) : (double.NaN, double.NaN);
                }
                var s30 = Share(PoolMap("civLandSettlements", y30, y30)); var s60 = Share(PoolMap("civLandSettlements", y60, y60));
                var t30 = Share(PoolMap("civSettlements", y30, y30));
                KnownRows.Add(new Known
                {
                    Name = "Medeniyetler 5 yerleşimde takılıyor",
                    Old = "98 medeniyetin 74'ü (%76) tam 5 yerleşimde",
                    Now = $"tam 5 kara yerleşimli medeniyet payı {y30}. yılda {Pct(s30.Eq)}, {y60}. yılda {Pct(s60.Eq)} (denizaşırı koloniler dâhil {y30}. yılda tam 5: {Pct(t30.Eq)}, 5+: {Pct(t30.Ge)}); medeniyet başına {F(Yearly(K("settlementsPerCiv"), y30).Med)} yerleşim ({y30}. yıl)",
                    Persists = s30.Eq >= 0.5,
                });
            }
            // kamplar
            {
                int peakY = 1; double peak = double.NaN;
                for (int y = 1; y <= Y; y++) { double v = Yearly(K("campsAlive"), y).Med; if (double.IsNaN(peak) || v > peak) { peak = v; peakY = y; } }
                double c1 = Yearly(K("campsAlive"), 1).Med, c30 = Yearly(K("campsAlive"), y30).Med, c60 = Yearly(K("campsAlive"), y60).Med;
                KnownRows.Add(new Known
                {
                    Name = "Kamp sayısı düşüyor",
                    Old = "6,8'den 3,5'e iniyor",
                    Now = $"{F(c1)} (1. yıl) → en yüksek {F(peak)} ({peakY}. yıl) → {F(c30)} ({y30}. yıl) → {F(c60)} ({y60}. yıl); yıl sonu, yıllık dünya medyanı",
                    Persists = c30 < 0.75 * peak,
                });
            }
            // altın
            {
                double g1 = Yearly(K("goldMedian"), 1).Med, g30 = Yearly(K("goldMedian"), y30).Med, g60 = Yearly(K("goldMedian"), y60).Med;
                KnownRows.Add(new Known
                {
                    Name = "Altın birikiyor",
                    Old = "altın medyanı 78'den 6.503'e çıkıyor",
                    Now = $"{F(g1)} (1. yıl) → {F(g30)} ({y30}. yıl) → {F(g60)} ({y60}. yıl)",
                    Persists = g30 >= 10 * g1,
                });
            }
            // boştaki iş gücü
            {
                double i30 = Yearly(K("idleShare"), y30).Med, i60 = Yearly(K("idleShare"), y60).Med;
                KnownRows.Add(new Known
                {
                    Name = "İş gücü boşta",
                    Old = "iş gücünün %43'ü boşta",
                    Now = $"{y30}. yılda {Pct(i30)}, {y60}. yılda {Pct(i60)} (işe yerleşemeyen `zanaatçı` / bütün iş gücü, askerler dâhil; yıl içi ortalama)",
                    Persists = i30 >= 0.3,
                });
            }
            // büyük olaylar
            {
                int peakY = 1; double peak = double.NaN;
                for (int y = 1; y <= Y; y++) { double v = Yearly(K("majorEvents"), y).Med; if (double.IsNaN(peak) || v > peak) { peak = v; peakY = y; } }
                double m30 = Yearly(K("majorEvents"), y30).Med, m60 = Yearly(K("majorEvents"), y60).Med;
                KnownRows.Add(new Known
                {
                    Name = "Büyük olaylar seyreliyor",
                    Old = "yıllık büyük olay 59'dan 28'e düşüyor",
                    Now = $"en yüksek {F(peak)} ({peakY}. yıl) → {F(m30)} ({y30}. yıl) → {F(m60)} ({y60}. yıl), yıllık dünya medyanı",
                    Persists = m30 <= 0.6 * peak,
                });
            }
            // doğuş seviyesi
            {
                int from = Math.Min(25, Y);
                var m = PoolMap("levelBirth", from, Y);
                double tot = m.Values.Sum(), top = m.Where(kv => int.TryParse(kv.Key, out int k) && k >= 5).Sum(kv => kv.Value);
                var decs = Dec.Select(d => F(LevelMean(PoolMap("levelBirth", d.From, d.To)).Mean)).ToList();
                KnownRows.Add(new Known
                {
                    Name = "Doğuş seviyesi şişiyor",
                    Old = "24. yıldan sonra herkes Sv5 doğuyor; efsane mekaniği ölü",
                    Now = $"{from}–{Y}. yıllarda Sv5+ doğanların payı {Pct(tot > 0 ? top / tot : double.NaN)}; on yıllık doğuş seviyesi ortalaması {string.Join(" · ", decs)}",
                    Persists = tot > 0 && top / tot >= 0.5,
                });
            }
            // başkent
            {
                int cap = Ok.Sum(r => r.Stats.CollapseLog.Count(x => x.Kind == "capital")), ext = Ok.Sum(r => r.Stats.CollapseLog.Count(x => x.Kind == "extinct"));
                int conq = (int)Ok.Sum(r => r.Stats.Years.Sum(y => y.V[K("captured")]));
                KnownRows.Add(new Known
                {
                    Name = "Başkent düşmüyor",
                    Old = "başkent fethedilemiyor (agents.ts:673)",
                    Now = $"{Ok.Count} dünyada {cap} başkent kaybı, {ext} yok olma; {conq} yerleşim fethi",
                    Persists = cap == 0,
                });
            }
        }

        // ------------------------------------------------------------ report.json
        public JObj Json()
        {
            var o = new JObj
            {
                { "label", O.Label }, { "created", Created }, { "seeds", O.Seeds }, { "years", O.Years }, { "measuredYears", Y }, { "yearDays", YEAR },
                { "jobs", O.Jobs }, { "wallSec", Wall },
                { "worldSec", Ok.Count > 0 ? new JObj { { "median", Median(Ok.Select(r => r.Sec)) }, { "min", Ok.Min(r => r.Sec) }, { "max", Ok.Max(r => r.Sec) } } : null },
                { "runsDir", Rel(O.Runs) },
                { "definitions", new JObj
                    {
                        { "year", "1 yıl = 120 gün; y. yıl = (y-1)*120+1 ... y*120. günler" },
                        { "stats", "medyan, p10, p90: dünyalar arası, doğrusal ara değer" },
                        { "flow", "akış ölçüsü: yıl içindeki toplam" },
                        { "stock", "stok ölçüsü: yıl sonu değeri" },
                        { "mean", "ortalama ölçü: yıl içi ortalama ya da oran" },
                        { "decade", "on yıllık değer: her dünyada on yılın yıllık değerlerinin ortalaması, sonra dünyalar arası medyan/p10/p90" },
                    }
                },
            };
            o.Add("worlds", All.Select(r => (object)WorldRow(r)).ToList());
            var metrics = new JObj();
            for (int k = 0; k < WorldStats.Defs.Length; k++)
            {
                var d = WorldStats.Defs[k];
                var ys = Enumerable.Range(1, Y).Select(y => Yearly(k, y)).ToList();
                metrics.Add(d.Key, new JObj
                {
                    { "label", d.Label }, { "group", d.Group }, { "kind", d.Kind.ToString().ToLowerInvariant() }, { "share", d.Share },
                    { "median", ys.Select(t => t.Med).ToArray() }, { "p10", ys.Select(t => t.P10).ToArray() }, { "p90", ys.Select(t => t.P90).ToArray() },
                });
            }
            o.Add("metrics", metrics);
            var dec = new JObj { { "ranges", Dec.Select(Range).ToList() } };
            var dm = new JObj();
            for (int k = 0; k < WorldStats.Defs.Length; k++)
            {
                var ds = Dec.Select(d => Decade(k, d)).ToList();
                dm.Add(WorldStats.Defs[k].Key, new JObj { { "median", ds.Select(t => t.Med).ToArray() }, { "p10", ds.Select(t => t.P10).ToArray() }, { "p90", ds.Select(t => t.P90).ToArray() } });
            }
            dec.Add("metrics", dm);
            var maps = new JObj();
            foreach (var (key, label, kind) in WorldStats.MapDefs)
            {
                var keys = new JObj();
                foreach (var mk in MapKeys(key))
                {
                    var ds = Dec.Select(d => MapDecade(key, mk, d)).ToList();
                    keys.Add(mk, new JObj { { "median", ds.Select(t => t.Med).ToArray() }, { "p10", ds.Select(t => t.P10).ToArray() }, { "p90", ds.Select(t => t.P90).ToArray() } });
                }
                maps.Add(key, new JObj { { "label", label }, { "kind", kind.ToString().ToLowerInvariant() }, { "decadeYearly", keys } });
            }
            dec.Add("maps", maps);
            o.Add("decades", dec);
            o.Add("levels", new JObj { { "birth", LevelTable("levelBirth", false) }, { "death", LevelTable("levelDeath", false) }, { "alive", LevelTable("levelAlive", true) } });
            o.Add("criteria", Crits.Select(c => (object)new JObj { { "id", c.Id }, { "name", c.Name }, { "rule", c.Rule }, { "pass", c.Pass }, { "measured", c.Measured }, { "data", c.Data } }).ToList());
            o.Add("knownProblems", KnownRows.Select(k => (object)new JObj { { "name", k.Name }, { "old", k.Old }, { "now", k.Now }, { "persists", k.Persists } }).ToList());
            return o;
        }

        private JObj WorldRow(WorldRun r)
        {
            var o = new JObj { { "seed", r.Seed }, { "ok", r.Ok }, { "error", r.Error }, { "sec", r.Sec }, { "finalHash", r.FinalHash } };
            if (r.Stats == null) return o;
            var last = r.Stats.Years.Count > 0 ? r.Stats.Years[r.Stats.Years.Count - 1] : null;
            o.Add("civsAlive", last?.V[K("civsAlive")]);
            o.Add("settlements", last?.V[K("settlements")]);
            o.Add("population", last?.V[K("population")]);
            o.Add("collapses", r.Stats.CollapseLog.Count);
            o.Add("legends", r.Stats.LegendList.Count);
            o.Add("maxHeroLevel", r.Stats.MaxHeroLevel);
            o.Add("heroesBorn", r.Stats.HeroesBornTotal);
            o.Add("heroesBornDead", r.Stats.HeroesBornDead);
            var city = r.Stats.Civs.Where(c => c.Founded == 0 && c.TierDays.Count > 2 && c.TierDays[2] >= 0).Select(c => (double)WorldStats.YearOf((int)c.TierDays[2])).ToList();
            o.Add("firstCityYearMedian", city.Count > 0 ? Median(city) : double.NaN);
            o.Add("json", r.JsonPath != null ? Rel(r.JsonPath) : null);
            return o;
        }

        private List<object> LevelTable(string map, bool stockAtEnd)
        {
            var o = new List<object>();
            foreach (var d in Dec)
            {
                var m = stockAtEnd ? PoolMap(map, d.To, d.To) : PoolMap(map, d.From, d.To);
                var (n, mean) = LevelMean(m);
                var share = new JObj();
                foreach (var kv in m) share.Add(kv.Key, n > 0 ? kv.Value / n : double.NaN);
                o.Add(new JObj { { "years", stockAtEnd ? $"{d.To}" : Range(d) }, { "n", n }, { "mean", mean }, { "share", share } });
            }
            return o;
        }

        // ------------------------------------------------------------ report.md
        public string Markdown()
        {
            var sb = new StringBuilder();
            void L(string s = "") => sb.Append(s).Append('\n');
            int n = Ok.Count;
            L($"# Ölçüm raporu: {O.Label}");
            L();
            L($"{All.Count} dünya (seed {O.SeedsText}) × {O.Years} yıl ({O.Years * YEAR} gün) · {Created} · `FD.Macro.Run stats --seeds {O.SeedsText} --years {O.Years} --jobs {O.Jobs} --verify {O.Verify} --saveload {O.SaveLoad}`");
            L();
            if (n > 0)
                L($"Süre: {FmtSec(Wall)} duvar saati, {O.Jobs} iş parçacığı; dünya başına {F(Median(Ok.Select(r => r.Sec)))} sn (en az {F(Ok.Min(r => r.Sec))}, en çok {F(Ok.Max(r => r.Sec))}; yıl sonu hash'leri dâhil).");
            var bad = All.Where(r => !r.Ok).ToList();
            if (bad.Count > 0)
            {
                L();
                L($"**Uyarı:** {bad.Count} dünya hata verdi ve toplamlara girmedi: " + string.Join("; ", bad.Select(r => $"seed {S(r.Seed)} gün {r.ErrorDay}: {r.Error}")));
            }
            if (n == 0) { L(); L("Ölçülen dünya yok."); return sb.ToString(); }
            L();
            L("## Bitiş ölçütleri");
            L();
            L("DESIGN-FAZ1.md, \"Bitiş ölçütleri\". ✓ geçti · ✗ kaldı · — ölçülemedi.");
            L();
            L("| # | Ölçüt | Koşul | Ölçülen | Sonuç |");
            L("|---|---|---|---|---|");
            foreach (var c in Crits) L($"| {c.Id} | {c.Name} | {c.Rule} | {c.Measured} | {Mark(c.Pass)} |");
            L();
            L("Tanımlar:");
            L();
            L("- **1, 3:** \"41–60. yılların medyanı\": 41–60. yıllardaki bütün (dünya, yıl) değerlerinin medyanı (16 dünya × 20 yıl = 320 değer); 6–20. yıllar için de aynı. Büyük olay = `GameEvent.Major == true` olan olaylar (`Sim.OnEvent` ile sayılır, `World.Events` kırpılmasından etkilenmez). Kamp ölçütünde yıllık değer, o yıl her gün sayılan yaşayan kamp sayısının ortalamasıdır (korsan koyları dâhil; yıl sonu sayımıyla değer ayrıca verilmiştir).");
            L("- **2:** Çöküş: medeniyet yok olur (`Civ.Alive` false olur) ya da medeniyet yaşarken başkentini kaybeder: bir önceki gün sonunda başkenti olan yerleşim (`Sim.Capital`, en kalabalık yerleşim) artık yaşamıyor ya da başka medeniyetin; medeniyetin o gün hâlâ yerleşimi vardır. Son yerleşimin düşmesi yok olma olarak bir kez sayılır.");
            L("- **4a:** Her on yılda doğan bütün kahramanların (bütün dünyalar) doğuş seviyelerinin ortalaması; her on yıl ≤ 2 olmalı. Doğuş seviyesi kahraman `World.Heroes`'a girdiği anda okunur.");
            L("- **4b:** Dünyada koşu boyunca herhangi bir kahramanın ulaştığı en yüksek seviye ≥ 8 olan dünyaların payı.");
            L("- **4c:** Dünya başına koşu boyunca efsane olan (`Hero.Legend`) kahraman sayısının dünyalar arası medyanı.");
            L("- **4d:** Koşu boyunca doğan bütün kahramanlardan (bütün dünyalar) koşu sonunda ölü (`State == \"dead\"`) olanların payı; emekli olanlar ve diyarı terk edenler ölü sayılmaz.");
            L("- **5a:** İlk `--verify` seed'i aynı süreçte toplayıcı bağlanmadan yeniden koşulur; her yıl sonundaki kanonik dünya hash'i (golden testteki FNV-1a) toplayıcılı koşuyla karşılaştırılır. Bu, determinizmi ve toplayıcının simülasyonu değiştirmediğini birlikte denetler.");
            L("- **5b:** `Sim.Save(string)` ve `static Sim Sim.Load(string)` varsa ilk `--saveload` seed'i yıl ortasında (gün = yıl/2 × 120 + 37) kaydedilip yüklenir ve sonuna dek koşulur; yüklenen dünyanın hash'i ve sonraki yıl sonu hash'leri kesintisiz koşuyla karşılaştırılır.");
            L();
            L("## Eski analizdeki sorunlar");
            L();
            L("Eski analiz: TS v0.23, 12 seed × 30 yıl ve 3 seed × 60 yıl (Proje: `analiz-5-ajan-oneriler.md`). \"Sürüyor mu\" kaba bir eşiktir: araştırma ağacı Faz 1b-3'te kaldırıldı; 30. yılda tam 5 kara yerleşimli medeniyet ≥ %50; kamp (30. yıl) < 0,75 × en yüksek yıl; altın (30. yıl) ≥ 10 × altın (1. yıl); boştaki iş gücü (30. yıl) ≥ %30; büyük olay (30. yıl) ≤ 0,6 × en yüksek yıl; 25. yıldan sonra doğanların ≥ %50'si Sv5+; hiç başkent kaybı yok.");
            L();
            L("| Bulgu | Eski analiz | Bu ölçüm | Sürüyor mu? |");
            L("|---|---|---|---|");
            foreach (var k in KnownRows) L($"| {k.Name} | {k.Old} | {k.Now} | {(k.Persists == null ? "—" : k.Persists.Value ? "evet" : "hayır")} |");
            L();
            L("## On yıllık özet");
            L();
            L($"Hücre: dünyalar arası medyan (p10–p90). Her dünyada on yılın yıllık değerlerinin ortalaması alınır: akış ölçülerinde yıllık ortalama, stok ölçülerinde yıl sonu değerlerinin ortalaması. Yüzdeler 0–1 paylardır.");
            L();
            L("| Ölçü | " + string.Join(" | ", Dec.Select(Range)) + " |");
            L("|---|" + string.Concat(Dec.Select(_ => "---|")));
            string group = null;
            for (int k = 0; k < WorldStats.Defs.Length; k++)
            {
                var d = WorldStats.Defs[k];
                if (d.Group != group) { group = d.Group; L($"| **{group}** |" + string.Concat(Dec.Select(_ => " |"))); }
                L($"| {d.Label} | " + string.Join(" | ", Dec.Select(x => Cell(d, Decade(k, x)))) + " |");
            }
            L();
            L("## Kahraman seviyeleri");
            L();
            LevelMd(sb, "Doğuş seviyesi (bütün dünyalar, on yıl içinde doğanlar)", "levelBirth", false);
            LevelMd(sb, "Ölüm seviyesi (bütün dünyalar, on yıl içinde ölenler)", "levelDeath", false);
            LevelMd(sb, "Yaşayan kahramanların seviyesi (bütün dünyalar, on yılın son yılının sonunda)", "levelAlive", true);
            L("### Ölüm nedenleri (bütün dünyalar)");
            L();
            {
                var whole = PoolMap("deathCauses", 1, Y);
                var keys = MapKeys("deathCauses").OrderByDescending(k => whole.TryGetValue(k, out double t) ? t : 0).ToList();
                L("| Neden | " + string.Join(" | ", Dec.Select(Range)) + " | Toplam |");
                L("|---|" + string.Concat(Dec.Select(_ => "---|")) + "---|");
                double all = whole.Values.Sum();
                foreach (var key in keys)
                {
                    double tot = whole.TryGetValue(key, out double t) ? t : 0;
                    L($"| {WorldStats.LabelOf("deathCauses", key)} | " + string.Join(" | ", Dec.Select(d => F(PoolMap("deathCauses", d.From, d.To).TryGetValue(key, out double v) ? v : 0))) + $" | {F(tot)} ({Pct(all > 0 ? tot / all : double.NaN)}) |");
                }
                L();
                L("Neden, ölümün kaydedildiği andaki son muharebenin türünden (başlık ve taraflar) ya da suikast olayından çıkarılır.");
                L();
            }
            MapMd(sb, "Olay türleri", "eventsByKind", "Dünya başına yıllık olay sayısı: dünyalar arası medyan (p10–p90).");
            MapMd(sb, "Büyük olay türleri", "majorByKind", "Dünya başına yıllık büyük olay sayısı: dünyalar arası medyan (p10–p90).");
            MapMd(sb, "Muharebe türleri", "battleTypes", "Dünya başına yıllık muharebe sayısı: dünyalar arası medyan (p10–p90).");
            MapMd(sb, "Kamp türleri", "campsByKind", "Dünya başına yaşayan kamp (yıl sonu değerlerinin on yıllık ortalaması): dünyalar arası medyan (p10–p90).");
            L("## Kademe dağılımı");
            L();
            {
                // Faz 1b-3: yerleşim kademesi (0 Kamp, 1 Köy, 2 Kasaba, 3 Şehir), bütün dünyalar, on yılın sonunda
                string[] tierTr = { "Kamp", "Köy", "Kasaba", "Şehir" };
                string TierName(string k) => int.TryParse(k, out int i) && i >= 0 && i < tierTr.Length ? $"{i} {tierTr[i]}" : k;
                foreach (var (map, what) in new[] { ("tierDist", "Yaşayan yerleşimlerin"), ("capTierDist", "Başkentlerin") })
                {
                    var keys = new List<string> { "0", "1", "2", "3" };
                    L($"{what} kademelere dağılımı, bütün dünyalar (yıl sonu; parantezde sayı).");
                    L();
                    L("| Yıl | " + string.Join(" | ", keys.Select(TierName)) + " |");
                    L("|---|" + string.Concat(keys.Select(_ => "---|")));
                    for (int y = 10; y <= Y; y += 10)
                    {
                        var m = PoolMap(map, y, y);
                        double tot = m.Values.Sum();
                        L($"| {y} | " + string.Join(" | ", keys.Select(e => { double v = m.TryGetValue(e, out double x) ? x : 0; return $"{Pct(tot > 0 ? v / tot : double.NaN)} ({F(v)})"; })) + " |");
                    }
                    L();
                }
            }
            L("## Dünyalar");
            L();
            L("| Seed | Medeniyet | Yerleşim | Nüfus | Çöküş | Efsane | En yüksek Sv | Doğan / ölü kahraman | İlk şehir (yıl, medyan) | Süre (sn) | Son hash |");
            L("|---|---|---|---|---|---|---|---|---|---|---|");
            foreach (var r in All)
            {
                if (!r.Ok) { L($"| {S(r.Seed)} | HATA gün {r.ErrorDay}: {r.Error} | | | | | | | | {F(r.Sec)} | |"); continue; }
                var w = WorldRow(r);
                L($"| {S(r.Seed)} | {F(Num(w["civsAlive"]))} | {F(Num(w["settlements"]))} | {F(Num(w["population"]))} | {r.Stats.CollapseLog.Count} | {r.Stats.LegendList.Count} | {r.Stats.MaxHeroLevel} | {r.Stats.HeroesBornTotal} / {r.Stats.HeroesBornDead} | {F(Num(w["firstCityYearMedian"]))} | {F(r.Sec)} | `{r.FinalHash}` |");
            }
            L();
            var cl = Ok.SelectMany(r => r.Stats.CollapseLog.Select(c => (r.Seed, c))).ToList();
            if (cl.Count > 0)
            {
                L("Çöküşler:");
                L();
                foreach (var (seed, c) in cl)
                    L($"- seed {S(seed)}, {c.Year}. yıl (gün {c.Day}): {c.CivName} " + (c.Kind == "extinct" ? "yok oldu" : $"başkenti kaybetti: {c.Settlement} ({(c.By == "terk" ? "terk edildi" : c.By + " aldı")})"));
                L();
            }
            L("## Yıllık ayrıntı");
            L();
            L($"Hücre: medyan (p10–p90), {n} dünya. Yıl y = (y−1)·120+1 … y·120. günler. Bütün değerler `report.json` içinde (`metrics`), dünya başına değerler `{Rel(O.Runs)}` altında.");
            L();
            foreach (var grp in WorldStats.Defs.Select((d, k) => (d, k)).GroupBy(x => x.d.Group))
            {
                var items = grp.ToList();
                for (int part = 0; part * 6 < items.Count; part++)
                {
                    var chunk = items.Skip(part * 6).Take(6).ToList();
                    int parts = (items.Count + 5) / 6;
                    L($"### {grp.Key}{(parts > 1 ? $" ({part + 1}/{parts})" : "")}");
                    L();
                    L("| Yıl | " + string.Join(" | ", chunk.Select(x => x.d.Label)) + " |");
                    L("|---|" + string.Concat(chunk.Select(_ => "---|")));
                    for (int y = 1; y <= Y; y++) L($"| {y} | " + string.Join(" | ", chunk.Select(x => Cell(x.d, Yearly(x.k, y)))) + " |");
                    L();
                }
            }
            return sb.ToString();
        }

        private static double Num(object v) => v is double d ? d : v is int i ? i : double.NaN;

        private static string FmtSec(double s) => s >= 120 ? $"{Math.Floor(s / 60).ToString("0", Inv)} dk {Math.Round(s % 60).ToString("0", Inv)} sn" : $"{F(s)} sn";

        private void LevelMd(StringBuilder sb, string title, string map, bool stockAtEnd)
        {
            void L(string s = "") => sb.Append(s).Append('\n');
            var rows = LevelTable(map, stockAtEnd).Cast<JObj>().ToList();
            var lv = MapKeys(map);
            L($"### {title}");
            L();
            L("| Yıl | n | ort. | " + string.Join(" | ", lv.Select(x => "Sv" + x)) + " |");
            L("|---|---|---|" + string.Concat(lv.Select(_ => "---|")));
            foreach (var r in rows)
            {
                var share = (JObj)r["share"];
                string Sh(string x) { double v = Num(share[x]); return Pct(double.IsNaN(v) ? 0 : v); }
                L($"| {r["years"]} | {F(Num(r["n"]))} | {F(Num(r["mean"]))} | " + string.Join(" | ", lv.Select(Sh)) + " |");
            }
            L();
        }

        private void MapMd(StringBuilder sb, string title, string map, string note)
        {
            void L(string s = "") => sb.Append(s).Append('\n');
            var keys = MapKeys(map);
            if (keys.Count == 0) return;
            var rows = keys.Select(k => (k, ds: Dec.Select(d => MapDecade(map, k, d)).ToList())).OrderByDescending(x => x.ds.Sum(t => double.IsNaN(t.Med) ? 0 : t.Med)).ToList();
            L($"## {title}");
            L();
            L(note);
            L();
            L("| Tür | " + string.Join(" | ", Dec.Select(Range)) + " |");
            L("|---|" + string.Concat(Dec.Select(_ => "---|")));
            var def = new StatDef("x", "x", "x", StatKind.Flow);
            foreach (var (k, ds) in rows) L($"| {WorldStats.LabelOf(map, k)}{(WorldStats.LabelOf(map, k) != k ? $" (`{k}`)" : "")} | " + string.Join(" | ", ds.Select(t => Cell(def, t))) + " |");
            L();
        }

        // ------------------------------------------------------------ stdout özeti
        public string Summary()
        {
            var sb = new StringBuilder();
            sb.Append($"\n{O.Label}: {Ok.Count}/{All.Count} dünya × {O.Years} yıl, {FmtSec(Wall)}\n");
            foreach (var c in Crits) sb.Append($"  {Mark(c.Pass)} {c.Id,-3} {c.Name}: {c.Measured}\n");
            sb.Append($"rapor: {Path.Combine(O.Out, "report.md")}, {Path.Combine(O.Out, "report.json")}; dünyalar: {O.Runs}\n");
            return sb.ToString();
        }
    }
}

// ==================================================================== canonical form + FNV-1a 64
/// <summary>
/// Canonical text of a C# object graph, identical to the TS canonical form of the corresponding JS value
/// (see golden/README.md): public instance fields named like the JSON/TS names (first letter lowered unless
/// [JsonPropertyName]; [JsonIgnore] skipped), JsObj/JsNumObj as objects, List as arrays; object keys sorted by
/// ordinal UTF-16 order, null values and '_'-prefixed keys skipped; numbers as JS String(x) (-0 → 0).
/// The hash is FNV-1a 64 over the UTF-16 code units of that text, computed while walking.
/// </summary>
public static class Canon
{
    public const ulong Offset = 0xcbf29ce484222325UL, Prime = 0x100000001b3UL;

    public static string Hash(object v)
    {
        var h = new Fnv();
        if (v == null) h.Raw("null"); else For(v.GetType()).Hash(h, v);
        return h.Hex();
    }

    public static string StrHash(string s) { var h = new Fnv(); h.Raw(s); return h.Hex(); }

    public static string Text(object v)
    {
        var h = new Fnv { Text = new StringBuilder() };
        if (v == null) h.Raw("null"); else For(v.GetType()).Hash(h, v);
        return h.Text.ToString();
    }

    public static List<string> Flat(object v)
    {
        var outp = new List<string>();
        Writer.FlatValue(outp, "", v);
        return outp;
    }

    // ---------------------------------------------------------------- scalars
    [ThreadStatic] private static Dictionary<long, string> _numCache;

    /// <summary>JS String(x) with -0 → "0".</summary>
    public static string NumStr(double x)
    {
        if (x >= int.MinValue && x <= int.MaxValue) { int i = (int)x; if (i == x) return i.ToString(CultureInfo.InvariantCulture); }
        return JsMath.Str(x);
    }

    /// <summary>JsMath.Str with a cache (most non-integers, e.g. tile elevations, never change).</summary>
    internal static string NumStrCached(double x)
    {
        var c = _numCache ??= new Dictionary<long, string>();
        long bits = BitConverter.DoubleToInt64Bits(x);
        if (!c.TryGetValue(bits, out var s))
        {
            if (c.Count >= 1 << 20) c.Clear();
            c[bits] = s = JsMath.Str(x);
        }
        return s;
    }

    public static string Quote(string s)
    {
        var sb = new StringBuilder(s.Length + 2);
        sb.Append('"');
        foreach (char c in s)
        {
            if (c == '"' || c == '\\') sb.Append('\\').Append(c);
            else if (c < ' ') sb.Append("\\u00").Append(Hex[c >> 4]).Append(Hex[c & 15]);
            else sb.Append(c);
        }
        return sb.Append('"').ToString();
    }

    internal const string Hex = "0123456789abcdef";

    /// <summary>Flat-listing path of key k under path.</summary>
    internal static string KeyPath(string path, string k)
    {
        foreach (char c in k)
            if (c <= ' ' || c == '"' || c == '.' || c == '=' || c == '[' || c == '\\' || c == ']') return path + "[" + Quote(k) + "]";
        return path.Length > 0 ? path + "." + k : k;
    }

    internal static string FieldName(FieldInfo f)
    {
        var a = f.GetCustomAttribute<JsonPropertyNameAttribute>();
        if (a != null) return a.Name;
        string n = f.Name;
        return n.Length > 0 && char.IsUpper(n[0]) ? char.ToLowerInvariant(n[0]) + n.Substring(1) : n;
    }

    // ---------------------------------------------------------------- writers (cached per type)
    private static readonly Dictionary<Type, Writer> Writers = new();

    internal static Writer For(Type t)
    {
        lock (Writers)
        {
            if (Writers.TryGetValue(t, out var w)) return w;
            w = Create(t);
            return w;
        }
    }

    private static Writer Create(Type t)
    {
        Writer w;
        if (t == typeof(double) || t == typeof(int) || t == typeof(bool) || t == typeof(string) || t == typeof(object)) w = new BoxedW();
        else if (IsOtherNumber(t)) w = new BoxedW();
        else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>))
            w = (Writer)Activator.CreateInstance(typeof(ListW<>).MakeGenericType(t.GetGenericArguments()[0]));
        else if (t.IsArray && t.GetArrayRank() == 1)
            w = (Writer)Activator.CreateInstance(typeof(ArrayW<>).MakeGenericType(t.GetElementType()));
        else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(JsObj<>))
            w = (Writer)Activator.CreateInstance(typeof(JsObjW<>).MakeGenericType(t.GetGenericArguments()[0]));
        else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(JsNumObj<>))
            w = (Writer)Activator.CreateInstance(typeof(JsNumObjW<>).MakeGenericType(t.GetGenericArguments()[0]));
        else if (t.IsClass && t != typeof(string) && !typeof(IEnumerable).IsAssignableFrom(t) && !typeof(Delegate).IsAssignableFrom(t))
        {
            var cw = new ClassW(t);
            Writers[t] = cw;          // before building fields: recursive types
            cw.Build();
            return cw;
        }
        else throw new NotSupportedException($"canonical form: unsupported type {t}");
        Writers[t] = w;
        w.Init();
        return w;
    }

    /// <summary>Numeric primitives other than double/int (written as the JS number they convert to).</summary>
    internal static bool IsOtherNumber(Type t) =>
        t == typeof(long) || t == typeof(float) || t == typeof(short) || t == typeof(byte) || t == typeof(sbyte) ||
        t == typeof(ushort) || t == typeof(uint) || t == typeof(ulong) || t == typeof(decimal);

    /// <summary>Element/value writer for static type T (no boxing for double/int/bool/string).</summary>
    internal static Action<Fnv, T> Elem<T>()
    {
        var t = typeof(T);
        if (t == typeof(double)) return (Action<Fnv, T>)(object)new Action<Fnv, double>((h, x) => h.Num(x));
        if (t == typeof(int)) return (Action<Fnv, T>)(object)new Action<Fnv, int>((h, x) => h.Int(x));
        if (t == typeof(bool)) return (Action<Fnv, T>)(object)new Action<Fnv, bool>((h, x) => h.Raw(x ? "true" : "false"));
        if (t == typeof(string)) return (Action<Fnv, T>)(object)new Action<Fnv, string>((h, x) => h.Quoted(x));
        if (t == typeof(object) || Nullable.GetUnderlyingType(t) != null || IsOtherNumber(t)) return (h, x) => Writer.HashValue(h, x);
        if (t.IsValueType) throw new NotSupportedException($"canonical form: unsupported value type {t}");
        var w = For(t);
        return (h, x) => w.Hash(h, x);
    }
}

/// <summary>FNV-1a 64 over UTF-16 code units (optionally also recording the text, for tests).</summary>
internal sealed class Fnv
{
    public ulong H = Canon.Offset;
    public StringBuilder Text;

    public string Hex() => H.ToString("x16", CultureInfo.InvariantCulture);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void U(char c)
    {
        H = (H ^ c) * Canon.Prime;
        Text?.Append(c);
    }

    public void Raw(string s)
    {
        ulong h = H;
        foreach (char c in s) h = (h ^ c) * Canon.Prime;
        H = h;
        Text?.Append(s);
    }

    public void Quoted(string s)
    {
        U('"');
        bool plain = true;
        foreach (char c in s) if (c == '"' || c == '\\' || c < ' ') { plain = false; break; }
        if (plain) Raw(s);
        else
            foreach (char c in s)
            {
                if (c == '"' || c == '\\') { U('\\'); U(c); }
                else if (c < ' ') { U('\\'); U('u'); U('0'); U('0'); U(Canon.Hex[c >> 4]); U(Canon.Hex[c & 15]); }
                else U(c);
            }
        U('"');
    }

    public void Int(int i)
    {
        uint u;
        if (i < 0) { U('-'); u = (uint)(-(long)i); } else u = (uint)i;
        if (u < 10) { U((char)('0' + u)); return; }
        Span<char> buf = stackalloc char[10];
        int k = 0;
        while (u > 0) { buf[k++] = (char)('0' + u % 10); u /= 10; }
        while (k > 0) U(buf[--k]);
    }

    public void Num(double x)
    {
        if (x >= int.MinValue && x <= int.MaxValue) { int i = (int)x; if (i == x) { Int(i); return; } }   // also -0 → "0"
        Raw(Canon.NumStrCached(x));                                                                    // NaN, ±Infinity, fractions, big
    }
}

internal abstract class Writer
{
    public virtual void Init() { }
    /// <summary>Hashes a non-null value of this writer's type.</summary>
    public abstract void Hash(Fnv h, object v);
    /// <summary>Flat listing of a non-null, non-scalar value.</summary>
    public abstract void Flat(List<string> outp, string path, object v);

    public static bool IsScalar(object v) => v is double || v is int || v is bool || v is string || Canon.IsOtherNumber(v.GetType());

    public static string ScalarText(object v) => v switch
    {
        double d => Canon.NumStr(d),
        int i => i.ToString(CultureInfo.InvariantCulture),
        bool b => b ? "true" : "false",
        string s => Canon.Quote(s),
        _ when Canon.IsOtherNumber(v.GetType()) => Canon.NumStr(Convert.ToDouble(v, CultureInfo.InvariantCulture)),
        _ => throw new NotSupportedException($"canonical form: unsupported scalar {v.GetType()}"),
    };

    /// <summary>Any (possibly boxed / null) value; null → "null" (array element context).</summary>
    public static void HashValue(Fnv h, object v)
    {
        switch (v)
        {
            case null: h.Raw("null"); break;
            case double d: h.Num(d); break;
            case int i: h.Int(i); break;
            case bool b: h.Raw(b ? "true" : "false"); break;
            case string s: h.Quoted(s); break;
            default:
                if (Canon.IsOtherNumber(v.GetType())) h.Num(Convert.ToDouble(v, CultureInfo.InvariantCulture));
                else Canon.For(v.GetType()).Hash(h, v);
                break;
        }
    }

    public static void FlatValue(List<string> outp, string path, object v)
    {
        if (v == null) outp.Add(path + "=null");
        else if (IsScalar(v)) outp.Add(path + "=" + ScalarText(v));
        else Canon.For(v.GetType()).Flat(outp, path, v);
    }

    /// <summary>Flat listing of object members (already sorted, nulls skipped).</summary>
    protected static void FlatMembers(List<string> outp, string path, List<KeyValuePair<string, object>> members)
    {
        if (members.Count == 0) { outp.Add(path + "={}"); return; }
        foreach (var kv in members) FlatValue(outp, Canon.KeyPath(path, kv.Key), kv.Value);
    }
}

/// <summary>Declared type object/double/int/bool/string: dispatch on the runtime value.</summary>
internal sealed class BoxedW : Writer
{
    public override void Hash(Fnv h, object v) => HashValue(h, v);
    public override void Flat(List<string> outp, string path, object v) => FlatValue(outp, path, v);
}

internal sealed class ListW<T> : Writer
{
    private Action<Fnv, T> _elem;
    private static readonly bool CanBeNull = !typeof(T).IsValueType || Nullable.GetUnderlyingType(typeof(T)) != null;

    public override void Init() => _elem = Canon.Elem<T>();

    public override void Hash(Fnv h, object v)
    {
        var l = (List<T>)v;
        h.U('[');
        for (int i = 0; i < l.Count; i++)
        {
            if (i > 0) h.U(',');
            T e = l[i];
            if (CanBeNull && e == null) h.Raw("null"); else _elem(h, e);
        }
        h.U(']');
    }

    public override void Flat(List<string> outp, string path, object v)
    {
        var l = (List<T>)v;
        if (l.Count == 0) { outp.Add(path + "=[]"); return; }
        for (int i = 0; i < l.Count; i++) FlatValue(outp, path + "[" + i.ToString(CultureInfo.InvariantCulture) + "]", l[i]);
    }
}

internal sealed class ArrayW<T> : Writer
{
    private Action<Fnv, T> _elem;
    private static readonly bool CanBeNull = !typeof(T).IsValueType || Nullable.GetUnderlyingType(typeof(T)) != null;

    public override void Init() => _elem = Canon.Elem<T>();

    public override void Hash(Fnv h, object v)
    {
        var a = (T[])v;
        h.U('[');
        for (int i = 0; i < a.Length; i++)
        {
            if (i > 0) h.U(',');
            T e = a[i];
            if (CanBeNull && e == null) h.Raw("null"); else _elem(h, e);
        }
        h.U(']');
    }

    public override void Flat(List<string> outp, string path, object v)
    {
        var a = (T[])v;
        if (a.Length == 0) { outp.Add(path + "=[]"); return; }
        for (int i = 0; i < a.Length; i++) FlatValue(outp, path + "[" + i.ToString(CultureInfo.InvariantCulture) + "]", a[i]);
    }
}

internal abstract class RecordW<V> : Writer
{
    private Action<Fnv, V> _elem;
    private static readonly bool CanBeNull = !typeof(V).IsValueType || Nullable.GetUnderlyingType(typeof(V)) != null;
    private static readonly Comparison<KeyValuePair<string, V>> ByKey = (a, b) => string.CompareOrdinal(a.Key, b.Key);

    public override void Init() => _elem = Canon.Elem<V>();

    /// <summary>Entries with keys as JS property names, in any order.</summary>
    protected abstract List<KeyValuePair<string, V>> Entries(object v);

    private List<KeyValuePair<string, V>> Sorted(object v)
    {
        var es = Entries(v);
        es.Sort(ByKey);                // keys are unique: no stability concerns
        return es;
    }

    public override void Hash(Fnv h, object v)
    {
        h.U('{');
        bool first = true;
        foreach (var kv in Sorted(v))
        {
            if (kv.Key.Length > 0 && kv.Key[0] == '_') continue;
            if (CanBeNull && kv.Value == null) continue;
            if (first) first = false; else h.U(',');
            h.Quoted(kv.Key);
            h.U(':');
            _elem(h, kv.Value);
        }
        h.U('}');
    }

    public override void Flat(List<string> outp, string path, object v)
    {
        var members = new List<KeyValuePair<string, object>>();
        foreach (var kv in Sorted(v))
            if (!(kv.Key.Length > 0 && kv.Key[0] == '_') && !(CanBeNull && kv.Value == null)) members.Add(new KeyValuePair<string, object>(kv.Key, kv.Value));
        FlatMembers(outp, path, members);
    }
}

internal sealed class JsObjW<V> : RecordW<V>
{
    protected override List<KeyValuePair<string, V>> Entries(object v) => ((JsObj<V>)v).Entries();
}

internal sealed class JsNumObjW<V> : RecordW<V>
{
    protected override List<KeyValuePair<string, V>> Entries(object v)
    {
        var es = ((JsNumObj<V>)v).Entries();
        var o = new List<KeyValuePair<string, V>>(es.Count);
        foreach (var kv in es) o.Add(new KeyValuePair<string, V>(kv.Key.ToString(CultureInfo.InvariantCulture), kv.Value));
        return o;
    }
}

/// <summary>A class: public instance fields (minus [JsonIgnore]), sorted by canonical name, with compiled typed getters.</summary>
internal sealed class ClassW : Writer
{
    private readonly Type _t;
    private Field[] _fields;

    public ClassW(Type t) { _t = t; }

    public void Build()
    {
        var fs = new List<Field>();
        foreach (var f in _t.GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            if (f.GetCustomAttribute<JsonIgnoreAttribute>() != null) continue;
            string name = Canon.FieldName(f);
            if (name.Length > 0 && name[0] == '_') continue;
            fs.Add(Field.Make(f, name));
        }
        fs.Sort((a, b) => string.CompareOrdinal(a.Name, b.Name));
        for (int i = 1; i < fs.Count; i++)
            if (fs[i].Name == fs[i - 1].Name) throw new NotSupportedException($"canonical form: duplicate field name {fs[i].Name} in {_t}");
        _fields = fs.ToArray();
    }

    public override void Hash(Fnv h, object o)
    {
        h.U('{');
        bool first = true;
        foreach (var f in _fields) if (f.Hash(h, o, first)) first = false;
        h.U('}');
    }

    public override void Flat(List<string> outp, string path, object o)
    {
        var members = new List<KeyValuePair<string, object>>();
        foreach (var f in _fields)
        {
            object v = f.Box(o);
            if (v != null) members.Add(new KeyValuePair<string, object>(f.Name, v));
        }
        FlatMembers(outp, path, members);
    }
}

internal abstract class Field
{
    public string Name, Pre, PreC;

    /// <summary>Writes '"name":value' (with a leading comma unless first); false when the value is null (skipped).</summary>
    public abstract bool Hash(Fnv h, object o, bool first);
    public abstract object Box(object o);

    protected void Key(Fnv h, bool first) => h.Raw(first ? Pre : PreC);

    public static Field Make(FieldInfo f, string name)
    {
        Type t = f.FieldType;
        Field r;
        if (t == typeof(double)) r = new F<double>(Get<double>(f), (h, x) => h.Num(x));
        else if (t == typeof(int)) r = new F<int>(Get<int>(f), (h, x) => h.Int(x));
        else if (t == typeof(bool)) r = new F<bool>(Get<bool>(f), (h, x) => h.Raw(x ? "true" : "false"));
        else if (t == typeof(double?)) r = new N<double>(Get<double?>(f), (h, x) => h.Num(x));
        else if (t == typeof(int?)) r = new N<int>(Get<int?>(f), (h, x) => h.Int(x));
        else if (t == typeof(bool?)) r = new N<bool>(Get<bool?>(f), (h, x) => h.Raw(x ? "true" : "false"));
        else if (t == typeof(string)) r = new RefF(Get<object>(f), (h, x) => h.Quoted((string)x));
        else if (Canon.IsOtherNumber(t)) r = new F<double>(Get<double>(f), (h, x) => h.Num(x));
        else if (Nullable.GetUnderlyingType(t) is Type u && Canon.IsOtherNumber(u)) r = new N<double>(Get<double?>(f), (h, x) => h.Num(x));
        else if (!t.IsValueType)
        {
            var w = Canon.For(t);
            r = new RefF(Get<object>(f), w.Hash);
        }
        else throw new NotSupportedException($"canonical form: unsupported field type {t} ({f.DeclaringType}.{f.Name})");
        r.Name = name;
        r.Pre = Canon.Quote(name) + ":";
        r.PreC = "," + r.Pre;
        return r;
    }

    private static Func<object, TF> Get<TF>(FieldInfo f)
    {
        var p = Expression.Parameter(typeof(object), "o");
        Expression body = Expression.Field(Expression.Convert(p, f.DeclaringType), f);
        if (body.Type != typeof(TF)) body = Expression.Convert(body, typeof(TF));
        return Expression.Lambda<Func<object, TF>>(body, p).Compile();
    }

    private sealed class F<T> : Field where T : struct
    {
        private readonly Func<object, T> _g;
        private readonly Action<Fnv, T> _w;
        public F(Func<object, T> g, Action<Fnv, T> w) { _g = g; _w = w; }
        public override bool Hash(Fnv h, object o, bool first) { Key(h, first); _w(h, _g(o)); return true; }
        public override object Box(object o) => _g(o);
    }

    private sealed class N<T> : Field where T : struct
    {
        private readonly Func<object, T?> _g;
        private readonly Action<Fnv, T> _w;
        public N(Func<object, T?> g, Action<Fnv, T> w) { _g = g; _w = w; }
        public override bool Hash(Fnv h, object o, bool first)
        {
            T? v = _g(o);
            if (v == null) return false;
            Key(h, first); _w(h, v.Value);
            return true;
        }
        public override object Box(object o) => _g(o);     // boxes to null or the plain value
    }

    private sealed class RefF : Field
    {
        private readonly Func<object, object> _g;
        private readonly Action<Fnv, object> _w;
        public RefF(Func<object, object> g, Action<Fnv, object> w) { _g = g; _w = w; }
        public override bool Hash(Fnv h, object o, bool first)
        {
            object v = _g(o);
            if (v == null) return false;
            Key(h, first); _w(h, v);
            return true;
        }
        public override object Box(object o) => _g(o);
    }
}
