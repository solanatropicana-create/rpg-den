using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using FD.Macro;

namespace JsCheck;

/// <summary>
/// Compares FD.Macro's JsMath / JsSort against reference vectors produced by Node 22
/// (tests/jsvectors/gen.mjs). Doubles are compared by IEEE-754 bit pattern (so -0 != +0);
/// any NaN matches any NaN (exact NaN payload agreement is reported for information).
/// Exit code: 0 = everything matches, 1 = at least one mismatch, 2 = vectors missing.
/// </summary>
public static class Program
{
    private static int failedChecks;
    private const int MaxReported = 8;

    public static int Main(string[] args)
    {
        string dir = FindDataDir(args);
        if (dir == null)
        {
            Console.Error.WriteLine("Reference vectors not found. Generate them first:  node tests/jsvectors/gen.mjs");
            Console.Error.WriteLine("(or pass the data directory as the first argument)");
            return 2;
        }
        Console.WriteLine($"vectors: {dir}");
        var total = Stopwatch.StartNew();

        CheckUnary("Exp", Path.Combine(dir, "exp.bin"), JsMath.Exp, Math.Exp);
        CheckUnary("Log", Path.Combine(dir, "log.bin"), JsMath.Log, Math.Log);
        CheckPow(Path.Combine(dir, "pow.bin"));
        CheckUnary("Round", Path.Combine(dir, "round.bin"), JsMath.Round, null);
        CheckToInt(Path.Combine(dir, "toint.bin"));
        CheckMinMax2(Path.Combine(dir, "minmax2.bin"));
        CheckMinMaxN(Path.Combine(dir, "minmaxN.bin"));
        CheckFormat(Path.Combine(dir, "format.txt"));
        CheckSort(Path.Combine(dir, "sort.bin"));
        Benchmark(dir);

        Console.WriteLine();
        Console.WriteLine(failedChecks == 0
            ? $"ALL CHECKS PASSED ({total.ElapsedMilliseconds} ms)"
            : $"FAILED: {failedChecks} check(s) had mismatches");
        return failedChecks == 0 ? 0 : 1;
    }

    // ---------------------------------------------------------------------------------------
    // helpers
    // ---------------------------------------------------------------------------------------

    private static string FindDataDir(string[] args)
    {
        if (args.Length > 0)
            return File.Exists(Path.Combine(args[0], "exp.bin")) ? Path.GetFullPath(args[0]) : null;
        var candidates = new List<string>();
        string cwd = Directory.GetCurrentDirectory();
        candidates.Add(Path.Combine(cwd, "tests", "jsvectors", "data"));
        candidates.Add(Path.Combine(cwd, "jsvectors", "data"));
        candidates.Add(Path.Combine(cwd, "..", "jsvectors", "data"));
        candidates.Add(Path.Combine(cwd, "data"));
        for (var d = new DirectoryInfo(AppContext.BaseDirectory); d != null; d = d.Parent)
            candidates.Add(Path.Combine(d.FullName, "jsvectors", "data"));
        foreach (string c in candidates)
            if (File.Exists(Path.Combine(c, "exp.bin"))) return Path.GetFullPath(c);
        return null;
    }

    private static double F64(byte[] b, int offset) =>
        BitConverter.Int64BitsToDouble(BinaryPrimitives.ReadInt64LittleEndian(b.AsSpan(offset, 8)));

    private static string Hex(double d) => BitConverter.DoubleToInt64Bits(d).ToString("x16");

    private static string Show(double d) => $"{d.ToString("R", CultureInfo.InvariantCulture)} [{Hex(d)}]";

    /// <summary>Bit-exact equality, except that all NaNs are equal.</summary>
    private static bool Same(double expected, double actual) =>
        double.IsNaN(expected) ? double.IsNaN(actual)
                               : BitConverter.DoubleToInt64Bits(expected) == BitConverter.DoubleToInt64Bits(actual);

    private static void Report(string name, long count, long mismatches, string extra = "")
    {
        Console.WriteLine($"{name,-22} {count,10:N0} cases  {mismatches,8:N0} mismatches{extra}");
        if (mismatches != 0) failedChecks++;
    }

    // ---------------------------------------------------------------------------------------
    // Exp / Log / Round
    // ---------------------------------------------------------------------------------------

    private static void CheckUnary(string name, string file, Func<double, double> f, Func<double, double> platform)
    {
        byte[] b = File.ReadAllBytes(file);
        int n = b.Length / 16;
        long mism = 0, nanTotal = 0, nanBitsSame = 0, platformMism = 0;
        for (int i = 0; i < n; i++)
        {
            double x = F64(b, i * 16), expected = F64(b, i * 16 + 8);
            double actual = f(x);
            if (!Same(expected, actual))
            {
                if (mism < MaxReported) Console.WriteLine($"  {name}({Show(x)}): node {Show(expected)}  c# {Show(actual)}");
                mism++;
            }
            if (double.IsNaN(expected))
            {
                nanTotal++;
                if (BitConverter.DoubleToInt64Bits(expected) == BitConverter.DoubleToInt64Bits(actual)) nanBitsSame++;
            }
            if (platform != null && !Same(expected, platform(x))) platformMism++;
        }
        string extra = $"   (NaN results: {nanTotal:N0}, identical NaN bits: {nanBitsSame:N0}"
                     + (platform != null ? $"; System.Math.{name} would mismatch {platformMism:N0})" : ")");
        Report(name, n, mism, extra);
    }

    // ---------------------------------------------------------------------------------------
    // Pow
    // ---------------------------------------------------------------------------------------

    private static void CheckPow(string file)
    {
        byte[] b = File.ReadAllBytes(file);
        int n = b.Length / 24;
        long mism = 0, platformMism = 0, nanTotal = 0, nanBitsSame = 0;
        // Per-class counters for the game's exponent set.
        var classCount = new Dictionary<string, long>();
        var classMism = new Dictionary<string, long>();
        for (int i = 0; i < n; i++)
        {
            double x = F64(b, i * 24), y = F64(b, i * 24 + 8), expected = F64(b, i * 24 + 16);
            double actual = JsMath.Pow(x, y);
            bool ok = Same(expected, actual);
            string cls = y == 0.3 ? "y=0.3" : y == 2.4 ? "y=2.4" : y == 2 ? "y=2" : y == 0.5 ? "y=0.5"
                       : (x >= 0.01 && x <= 1000 && y >= -5 && y <= 5) ? "game x,y" : "other";
            classCount[cls] = classCount.GetValueOrDefault(cls) + 1;
            if (!ok)
            {
                classMism[cls] = classMism.GetValueOrDefault(cls) + 1;
                if (mism < MaxReported) Console.WriteLine($"  Pow({Show(x)}, {Show(y)}): node {Show(expected)}  c# {Show(actual)}");
                mism++;
            }
            if (double.IsNaN(expected))
            {
                nanTotal++;
                if (BitConverter.DoubleToInt64Bits(expected) == BitConverter.DoubleToInt64Bits(actual)) nanBitsSame++;
            }
            if (!Same(expected, Math.Pow(x, y))) platformMism++;
        }
        Report("Pow", n, mism, $"   (NaN results: {nanTotal:N0}, identical NaN bits: {nanBitsSame:N0}; System.Math.Pow would mismatch {platformMism:N0})");
        foreach (var kv in classCount)
            Console.WriteLine($"    {kv.Key,-10} {kv.Value,10:N0} cases  {classMism.GetValueOrDefault(kv.Key),8:N0} mismatches");
    }

    // ---------------------------------------------------------------------------------------
    // ToInt32 / ToUint32
    // ---------------------------------------------------------------------------------------

    private static void CheckToInt(string file)
    {
        byte[] b = File.ReadAllBytes(file);
        int n = b.Length / 16;
        long m32 = 0, mu32 = 0;
        for (int i = 0; i < n; i++)
        {
            double x = F64(b, i * 16);
            int ei = BinaryPrimitives.ReadInt32LittleEndian(b.AsSpan(i * 16 + 8, 4));
            uint eu = BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(i * 16 + 12, 4));
            int ai = JsMath.ToInt32(x);
            uint au = JsMath.ToUint32(x);
            if (ai != ei)
            {
                if (m32 < MaxReported) Console.WriteLine($"  ToInt32({Show(x)}): node {ei}  c# {ai}");
                m32++;
            }
            if (au != eu)
            {
                if (mu32 < MaxReported) Console.WriteLine($"  ToUint32({Show(x)}): node {eu}  c# {au}");
                mu32++;
            }
        }
        Report("ToInt32 (x|0)", n, m32);
        Report("ToUint32 (x>>>0)", n, mu32);
    }

    // ---------------------------------------------------------------------------------------
    // Min / Max
    // ---------------------------------------------------------------------------------------

    private static void CheckMinMax2(string file)
    {
        byte[] b = File.ReadAllBytes(file);
        int n = b.Length / 32;
        long mMin = 0, mMax = 0;
        for (int i = 0; i < n; i++)
        {
            double a = F64(b, i * 32), c = F64(b, i * 32 + 8), eMin = F64(b, i * 32 + 16), eMax = F64(b, i * 32 + 24);
            double aMin = JsMath.Min(a, c), aMax = JsMath.Max(a, c);
            if (!Same(eMin, aMin)) { if (mMin < MaxReported) Console.WriteLine($"  Min({Show(a)}, {Show(c)}): node {Show(eMin)} c# {Show(aMin)}"); mMin++; }
            if (!Same(eMax, aMax)) { if (mMax < MaxReported) Console.WriteLine($"  Max({Show(a)}, {Show(c)}): node {Show(eMax)} c# {Show(aMax)}"); mMax++; }
        }
        Report("Min(a,b)", n, mMin);
        Report("Max(a,b)", n, mMax);
    }

    private static void CheckMinMaxN(string file)
    {
        byte[] b = File.ReadAllBytes(file);
        const int rec = 8 + 6 * 8 + 16;
        int n = b.Length / rec;
        long mMin = 0, mMax = 0;
        for (int i = 0; i < n; i++)
        {
            int o = i * rec;
            int len = BinaryPrimitives.ReadInt32LittleEndian(b.AsSpan(o, 4));
            var vals = new double[len];
            for (int j = 0; j < len; j++) vals[j] = F64(b, o + 8 + 8 * j);
            double eMin = F64(b, o + 56), eMax = F64(b, o + 64);
            double aMin = JsMath.Min(vals), aMax = JsMath.Max(vals);
            if (!Same(eMin, aMin)) { if (mMin < MaxReported) Console.WriteLine($"  Min(params len {len}): node {Show(eMin)} c# {Show(aMin)}"); mMin++; }
            if (!Same(eMax, aMax)) { if (mMax < MaxReported) Console.WriteLine($"  Max(params len {len}): node {Show(eMax)} c# {Show(aMax)}"); mMax++; }
        }
        Report("Min(params)", n, mMin);
        Report("Max(params)", n, mMax);
    }

    // ---------------------------------------------------------------------------------------
    // Str / ToFixed
    // ---------------------------------------------------------------------------------------

    private static void CheckFormat(string file)
    {
        long n = 0, mStr = 0, mFixed = 0, nFixed = 0;
        var fixedByDigits = new long[7];
        foreach (string line in File.ReadLines(file))
        {
            if (line.Length == 0) continue;
            string[] parts = line.Split('|');
            double x = BitConverter.Int64BitsToDouble((long)ulong.Parse(parts[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture));
            n++;
            string s = JsMath.Str(x);
            if (s != parts[1])
            {
                if (mStr < MaxReported) Console.WriteLine($"  Str({Show(x)}): node \"{parts[1]}\"  c# \"{s}\"");
                mStr++;
            }
            for (int d = 0; d <= 6; d++)
            {
                nFixed++;
                string f = JsMath.ToFixed(x, d);
                if (f != parts[2 + d])
                {
                    if (mFixed < MaxReported) Console.WriteLine($"  ToFixed({Show(x)}, {d}): node \"{parts[2 + d]}\"  c# \"{f}\"");
                    mFixed++;
                    fixedByDigits[d]++;
                }
            }
        }
        Report("Str (String(x))", n, mStr);
        Report("ToFixed(x, 0..6)", nFixed, mFixed, mFixed == 0 ? $"   ({n:N0} values x 7 digit counts)" : $"   by digits: {string.Join(",", fixedByDigits)}");

        // Extended range: toFixed(7..100).
        string extFile = Path.Combine(Path.GetDirectoryName(file), "tofixed-ext.txt");
        if (!File.Exists(extFile)) return;
        long nExt = 0, mExt = 0;
        foreach (string line in File.ReadLines(extFile))
        {
            if (line.Length == 0) continue;
            string[] parts = line.Split('|');
            double x = BitConverter.Int64BitsToDouble((long)ulong.Parse(parts[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture));
            int d = int.Parse(parts[1], CultureInfo.InvariantCulture);
            nExt++;
            string f = JsMath.ToFixed(x, d);
            if (f != parts[2])
            {
                if (mExt < MaxReported) Console.WriteLine($"  ToFixed({Show(x)}, {d}): node \"{parts[2]}\"  c# \"{f}\"");
                mExt++;
            }
        }
        Report("ToFixed(x, 7..100)", nExt, mExt);
    }

    // ---------------------------------------------------------------------------------------
    // JsSort
    // ---------------------------------------------------------------------------------------

    private sealed class Elem
    {
        public int Id;
        public double K;
        public double G;
    }

    /// <summary>Exact port of lib.mjs makeRng (mulberry32 variant).</summary>
    private sealed class JsRng
    {
        private uint a;
        public JsRng(uint seed) { a = seed; }
        public uint Next()
        {
            unchecked
            {
                a += 0x6D2B79F5;
                uint t = a;
                t = (t ^ (t >> 15)) * (t | 1);
                t ^= t + (t ^ (t >> 7)) * (t | 61);
                return t ^ (t >> 14);
            }
        }
    }

    private static readonly string[] KindNames =
        { "num", "numDesc", "nanKey", "nanSome", "random", "mostly", "gtOnly", "boolish", "doubleKeys", "twoKey" };

    /// <summary>Mirrors gen-sort.mjs makeComparator.</summary>
    private static Func<Elem, Elem, double> MakeComparer(int kind, uint seed)
    {
        var rng = new JsRng(seed);
        switch (kind)
        {
            case 0: return (a, b) => a.K - b.K;
            case 1: return (a, b) => b.K - a.K;
            case 2: return (a, b) => a.K - b.K; // NaN keys -> NaN
            case 3: return (a, b) => (a.K % 5 == 0 || b.K % 5 == 0) ? double.NaN : a.K - b.K;
            case 4:
                return (a, b) =>
                {
                    uint r = rng.Next();
                    switch (r & 7)
                    {
                        case 0: return double.NaN;
                        case 1: return -0.0;
                        case 2: return 0.0;
                        case 3: return -1.0;
                        case 4: return 1.0;
                        default: return (r >> 3) / 536870912.0 - 0.5;
                    }
                };
            case 5:
                return (a, b) =>
                {
                    uint r = rng.Next();
                    return (r & 15) == 0 ? (r >> 4) / 268435456.0 - 0.5 : a.K - b.K;
                };
            case 6: return (a, b) => a.K > b.K ? 1.0 : -1.0;
            case 7: return (a, b) => a.K > b.K ? 1.0 : 0.0;
            case 8: return (a, b) => a.K - b.K;
            case 9:
                return (a, b) =>
                {
                    double d = a.G - b.G; // JS: (a.g - b.g) || (b.k - a.k)   (0, -0, NaN are falsy)
                    return (d != 0 && !double.IsNaN(d)) ? d : b.K - a.K;
                };
            default: throw new InvalidDataException("unknown comparator kind " + kind);
        }
    }

    private static void CheckSort(string file)
    {
        using var br = new BinaryReader(File.OpenRead(file));
        if (new string(br.ReadChars(4)) != "JSRT") throw new InvalidDataException("bad sort.bin");
        br.ReadUInt32(); // version
        uint caseCount = br.ReadUInt32();

        var casesPerKind = new long[KindNames.Length];
        var orderMism = new long[KindNames.Length];
        var logMism = new long[KindNames.Length];
        var callsPerKind = new long[KindNames.Length];
        long reported = 0;

        for (uint c = 0; c < caseCount; c++)
        {
            int kind = (int)br.ReadUInt32();
            int pattern = (int)br.ReadUInt32();
            uint seed = br.ReadUInt32();
            int n = (int)br.ReadUInt32();
            var elems = new Elem[n];
            for (int i = 0; i < n; i++) elems[i] = new Elem { Id = i, K = br.ReadDouble() };
            for (int i = 0; i < n; i++) elems[i].G = br.ReadDouble();
            var expectedOrder = new int[n];
            for (int i = 0; i < n; i++) expectedOrder[i] = br.ReadUInt16();
            int callCount = (int)br.ReadUInt32();
            var expectedLog = new int[2 * callCount];
            for (int i = 0; i < expectedLog.Length; i++) expectedLog[i] = br.ReadUInt16();

            var inner = MakeComparer(kind, seed);
            var log = new List<int>(expectedLog.Length + 16);
            Func<Elem, Elem, double> cmp = (a, b) => { log.Add(a.Id); log.Add(b.Id); return inner(a, b); };

            // Exercise all three entry points.
            IList<Elem> result;
            switch (c % 3)
            {
                case 0:
                {
                    var list = new List<Elem>(elems);
                    JsSort.Sort(list, cmp);
                    result = list;
                    break;
                }
                case 1:
                    result = JsSort.Sorted(elems, cmp);
                    break;
                default:
                {
                    var arr = (Elem[])elems.Clone();
                    JsSort.Sort(arr, cmp);
                    result = arr;
                    break;
                }
            }

            casesPerKind[kind]++;
            callsPerKind[kind] += callCount;

            bool orderOk = result.Count == n;
            for (int i = 0; orderOk && i < n; i++) orderOk = result[i].Id == expectedOrder[i];
            bool logOk = log.Count == expectedLog.Length;
            int firstDiff = -1;
            for (int i = 0; i < Math.Min(log.Count, expectedLog.Length); i++)
                if (log[i] != expectedLog[i]) { firstDiff = i / 2; logOk = false; break; }

            if (!orderOk) orderMism[kind]++;
            if (!logOk) logMism[kind]++;
            if ((!orderOk || !logOk) && reported++ < MaxReported)
                Console.WriteLine($"  sort case {c}: kind {KindNames[kind]} pattern {pattern} n {n}: order {(orderOk ? "ok" : "DIFF")}, " +
                                  $"calls node {callCount} c# {log.Count / 2}, first differing call #{firstDiff}");
        }

        long totalCases = 0, totalOrder = 0, totalLog = 0, totalCalls = 0;
        Console.WriteLine($"JsSort: {caseCount:N0} cases (lengths 0..2000; 14 key patterns; List / Sorted / array entry points)");
        for (int k = 0; k < KindNames.Length; k++)
        {
            Console.WriteLine($"    cmp {KindNames[k],-11} {casesPerKind[k],6:N0} cases {callsPerKind[k],10:N0} calls   " +
                              $"order mismatches {orderMism[k],4:N0}   call-log mismatches {logMism[k],4:N0}");
            totalCases += casesPerKind[k]; totalOrder += orderMism[k]; totalLog += logMism[k]; totalCalls += callsPerKind[k];
        }
        Report("Sort final order", totalCases, totalOrder);
        Report("Sort comparator log", totalCases, totalLog, $"   ({totalCalls:N0} comparator calls compared: arguments + order)");
    }

    // ---------------------------------------------------------------------------------------
    // Informational timing
    // ---------------------------------------------------------------------------------------

    private static void Benchmark(string dir)
    {
        const int N = 2_000_000;
        var xs = new double[N];
        var ys = new double[N];
        var r = new JsRng(99);
        for (int i = 0; i < N; i++)
        {
            xs[i] = (r.Next() / 4294967296.0) * 55 - 50;   // exp game range
            ys[i] = (r.Next() / 4294967296.0) * 999.99 + 0.01;
        }
        // warm-up
        double sink = 0;
        for (int i = 0; i < 200_000; i++) sink += JsMath.Exp(xs[i]) + JsMath.Log(ys[i]) + JsMath.Pow(ys[i], 2.4) + JsMath.Round(xs[i]);

        double Time(Func<int, double> body)
        {
            var sw = Stopwatch.StartNew();
            double s = 0;
            for (int i = 0; i < N; i++) s += body(i);
            sw.Stop();
            sink += s;
            return sw.Elapsed.TotalMilliseconds * 1e6 / N;
        }
        double tExp = Time(i => JsMath.Exp(xs[i]));
        double tLog = Time(i => JsMath.Log(ys[i]));
        double tPow = Time(i => JsMath.Pow(ys[i], 2.4));
        double tRound = Time(i => JsMath.Round(xs[i] * 10));
        double tMathExp = Time(i => Math.Exp(xs[i]));
        double tMathPow = Time(i => Math.Pow(ys[i], 2.4));
        double tStr = Time(i => JsMath.Str(ys[i]).Length);
        double tFixed = Time(i => JsMath.ToFixed(ys[i], 2).Length);
        Console.WriteLine($"timing (ns/call, incl. delegate): Exp {tExp:F1}  Log {tLog:F1}  Pow {tPow:F1}  Round {tRound:F1}" +
                          $"  Str {tStr:F1}  ToFixed(,2) {tFixed:F1}   | System.Math.Exp {tMathExp:F1}  System.Math.Pow {tMathPow:F1}");
        if (sink == 42) Console.WriteLine(); // keep results alive
    }
}
