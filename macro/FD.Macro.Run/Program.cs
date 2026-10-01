using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;

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

    private sealed class UsageException : Exception { public UsageException(string m) : base(m) { } }
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
            default:
                throw new UsageException("usage: FD.Macro.Run hash <seed> <days> | cps <seed> <day> | dump <seed> <day> [label] | rng <seed> <day> | selftest");
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
