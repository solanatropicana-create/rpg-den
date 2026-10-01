using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FD.Macro;

/// <summary>
/// Small JavaScript-semantics helpers for porting the TS simulation line by line.
/// Rule of thumb: when a TS expression relies on a JS quirk (truthiness, out-of-range array
/// reads returning undefined, `||` defaults, string conversion of numbers), use a helper here
/// instead of the "obvious" C# code.
/// </summary>
public static class J
{
    // ------------------------------------------------------------------ truthiness
    /// <summary>JS truthiness of a number: false for 0, -0 and NaN.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool T(double x) => x != 0 && !double.IsNaN(x);
    /// <summary>JS truthiness of an optional number (undefined is falsy).</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool T(double? x) => x.HasValue && x.Value != 0 && !double.IsNaN(x.Value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool T(int x) => x != 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool T(int? x) => x.HasValue && x.Value != 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool T(bool? x) => x == true;
    /// <summary>JS truthiness of a string: false for undefined/null and "".</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool T(string s) => !string.IsNullOrEmpty(s);

    /// <summary>JS <c>x || y</c> for numbers.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Or(double x, double y) => T(x) ? x : y;
    /// <summary>JS <c>x || y</c> for optional numbers.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Or(double? x, double y) => T(x) ? x.Value : y;
    /// <summary>JS <c>s || t</c> for strings.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Or(string s, string t) => T(s) ? s : t;

    /// <summary>Optional number read as a JS number: undefined becomes NaN in arithmetic.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double N(double? x) => x ?? double.NaN;

    // ------------------------------------------------------------------ numbers
    /// <summary>JS <c>String(x)</c> / template literal <c>${x}</c> for a number.</summary>
    public static string S(double x) => JsMath.Str(x);
    public static string S(int x) => x.ToString(System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>JS <c>Math.round</c>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Round(double x) => JsMath.Round(x);

    /// <summary>JS <c>Number.isFinite</c> / global <c>isFinite</c> for numbers.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFinite(double x) => double.IsFinite(x);

    /// <summary>JS <c>Math.sign</c>.</summary>
    public static double Sign(double x) => double.IsNaN(x) ? double.NaN : x > 0 ? 1 : x < 0 ? -1 : x;

    /// <summary>Use a double that is known to be integral as an index (throws if it is not).</summary>
    public static int I(double x)
    {
        int i = (int)x;
        if (i != x) throw new InvalidOperationException($"non-integer index {x}");
        return i;
    }

    // ------------------------------------------------------------------ arrays
    /// <summary>JS <c>a[i]</c>: default (undefined) when out of range.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T At<T>(List<T> a, int i) => a != null && i >= 0 && i < a.Count ? a[i] : default;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T At<T>(T[] a, int i) => a != null && i >= 0 && i < a.Length ? a[i] : default;
    /// <summary>JS <c>a[i]</c> for a number array: null (undefined) when out of range or i not an integer.</summary>
    public static double? AtN(double[] a, double i) => i >= 0 && i < a.Length && i == Math.Floor(i) ? a[(int)i] : null;
    public static double? AtN(List<double> a, double i) => i >= 0 && i < a.Count && i == Math.Floor(i) ? a[(int)i] : null;

    /// <summary>JS <c>a[a.length - 1]</c>.</summary>
    public static T Last<T>(List<T> a) => a.Count > 0 ? a[a.Count - 1] : default;

    /// <summary>JS <c>a.find(f)</c>: default (undefined) when not found.</summary>
    public static T Find<T>(List<T> a, Func<T, bool> f)
    {
        for (int i = 0; i < a.Count; i++) if (f(a[i])) return a[i];
        return default;
    }

    /// <summary>JS <c>a.findIndex(f)</c>.</summary>
    public static int FindIndex<T>(List<T> a, Func<T, bool> f)
    {
        for (int i = 0; i < a.Count; i++) if (f(a[i])) return i;
        return -1;
    }

    /// <summary>JS <c>a.filter(f)</c>.</summary>
    public static List<T> Filter<T>(List<T> a, Func<T, bool> f)
    {
        var o = new List<T>();
        for (int i = 0; i < a.Count; i++) if (f(a[i])) o.Add(a[i]);
        return o;
    }

    /// <summary>JS <c>a.map(f)</c>.</summary>
    public static List<R> Map<T, R>(List<T> a, Func<T, R> f)
    {
        var o = new List<R>(a.Count);
        for (int i = 0; i < a.Count; i++) o.Add(f(a[i]));
        return o;
    }

    /// <summary>JS <c>a.some(f)</c>.</summary>
    public static bool Some<T>(List<T> a, Func<T, bool> f)
    {
        for (int i = 0; i < a.Count; i++) if (f(a[i])) return true;
        return false;
    }

    /// <summary>JS <c>a.every(f)</c>.</summary>
    public static bool Every<T>(List<T> a, Func<T, bool> f)
    {
        for (int i = 0; i < a.Count; i++) if (!f(a[i])) return false;
        return true;
    }

    /// <summary>JS <c>a.reduce((acc, x) => ..., init)</c> (left to right, same float order).</summary>
    public static R Reduce<T, R>(List<T> a, Func<R, T, R> f, R init)
    {
        R acc = init;
        for (int i = 0; i < a.Count; i++) acc = f(acc, a[i]);
        return acc;
    }

    /// <summary>JS <c>a.reduce((s, x) => s + f(x), 0)</c>: sum in array order.</summary>
    public static double Sum<T>(List<T> a, Func<T, double> f)
    {
        double s = 0;
        for (int i = 0; i < a.Count; i++) s += f(a[i]);
        return s;
    }

    /// <summary>JS <c>a.slice(start, end)</c> with negative-index semantics.</summary>
    public static List<T> Slice<T>(List<T> a, int start = 0, int? end = null)
    {
        int n = a.Count;
        int s = start < 0 ? Math.Max(n + start, 0) : Math.Min(start, n);
        int e = end == null ? n : end.Value < 0 ? Math.Max(n + end.Value, 0) : Math.Min(end.Value, n);
        var o = new List<T>(Math.Max(0, e - s));
        for (int i = s; i < e; i++) o.Add(a[i]);
        return o;
    }

    /// <summary>JS <c>a.splice(start, deleteCount, ...items)</c>; returns removed items.</summary>
    public static List<T> Splice<T>(List<T> a, int start, int? deleteCount = null, params T[] items)
    {
        int n = a.Count;
        int s = start < 0 ? Math.Max(n + start, 0) : Math.Min(start, n);
        int dc = deleteCount == null ? n - s : Math.Min(Math.Max(deleteCount.Value, 0), n - s);
        var removed = a.GetRange(s, dc);
        a.RemoveRange(s, dc);
        if (items != null && items.Length > 0) a.InsertRange(s, items);
        return removed;
    }

    /// <summary>JS <c>a.shift()</c>: removes and returns the first item (default when empty).</summary>
    public static T Shift<T>(List<T> a)
    {
        if (a.Count == 0) return default;
        T x = a[0];
        a.RemoveAt(0);
        return x;
    }

    /// <summary>JS <c>a.pop()</c>: removes and returns the last item (default when empty).</summary>
    public static T Pop<T>(List<T> a)
    {
        if (a.Count == 0) return default;
        T x = a[a.Count - 1];
        a.RemoveAt(a.Count - 1);
        return x;
    }

    /// <summary>JS <c>a.unshift(x)</c>.</summary>
    public static void Unshift<T>(List<T> a, T x) => a.Insert(0, x);

    /// <summary>JS <c>a.join(sep)</c> for strings.</summary>
    public static string Join(List<string> a, string sep = ",") => string.Join(sep, a);

    /// <summary>JS <c>[...new Set(a)]</c>: first occurrences, original order.</summary>
    public static List<T> Unique<T>(List<T> a)
    {
        var seen = new HashSet<T>();
        var o = new List<T>();
        foreach (var x in a) if (seen.Add(x)) o.Add(x);
        return o;
    }

    /// <summary>JS <c>Math.max(...a.map(f))</c>: -Infinity when empty.</summary>
    public static double MaxOf<T>(List<T> a, Func<T, double> f)
    {
        double m = double.NegativeInfinity;
        for (int i = 0; i < a.Count; i++) m = JsMath.Max(m, f(a[i]));
        return m;
    }

    /// <summary>JS <c>Math.min(...a.map(f))</c>: +Infinity when empty.</summary>
    public static double MinOf<T>(List<T> a, Func<T, double> f)
    {
        double m = double.PositiveInfinity;
        for (int i = 0; i < a.Count; i++) m = JsMath.Min(m, f(a[i]));
        return m;
    }

    /// <summary>Sort in place exactly like V8 <c>a.sort(cmp)</c>. Returns the list (like JS).</summary>
    public static List<T> Sort<T>(List<T> a, Func<T, T, double> cmp) { JsSort.Sort(a, cmp); return a; }

    /// <summary>JS <c>a.slice().sort(cmp)</c>.</summary>
    public static List<T> Sorted<T>(List<T> a, Func<T, T, double> cmp) { var c = new List<T>(a); JsSort.Sort(c, cmp); return c; }

    /// <summary>JS <c>Array.from({ length: n }, (_, i) => f(i))</c>.</summary>
    public static List<T> From<T>(int n, Func<int, T> f)
    {
        var o = new List<T>(n);
        for (int i = 0; i < n; i++) o.Add(f(i));
        return o;
    }

    public static List<T> L<T>(params T[] items) => new List<T>(items);

    // ------------------------------------------------------------------ strings
    /// <summary>JS <c>s.toLocaleLowerCase('tr')</c>: Turkish dotted/dotless i rules, otherwise invariant.</summary>
    public static string TrLower(string s)
    {
        if (s == null) return null;
        var sb = new StringBuilder(s.Length);
        foreach (char c in s)
        {
            if (c == 'I') sb.Append('ı');
            else if (c == 'İ') sb.Append('i');
            else sb.Append(char.ToLowerInvariant(c));
        }
        return sb.ToString();
    }

    /// <summary>JS <c>s.toLocaleUpperCase('tr')</c>.</summary>
    public static string TrUpper(string s)
    {
        if (s == null) return null;
        var sb = new StringBuilder(s.Length);
        foreach (char c in s)
        {
            if (c == 'i') sb.Append('İ');
            else if (c == 'ı') sb.Append('I');
            else sb.Append(char.ToUpperInvariant(c));
        }
        return sb.ToString();
    }

    /// <summary>Upper-case the first UTF-16 unit (TS <c>s[0].toLocaleUpperCase('tr') + s.slice(1)</c>).</summary>
    public static string TrCap(string s) => string.IsNullOrEmpty(s) ? s : TrUpper(s.Substring(0, 1)) + s.Substring(1);

    /// <summary>JS <c>s.padEnd(n)</c>.</summary>
    public static string PadEnd(string s, int n) => s.Length >= n ? s : s + new string(' ', n - s.Length);
}
