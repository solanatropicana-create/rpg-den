using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using FD.Macro;

namespace SaveCheck;

/// <summary>
/// Fast 128-bit fingerprint of an object graph, written independently of the save code. It feeds
///  - every instance field (public or not, inherited too, ordered by name) of every reachable object,
///  - container contents in iteration order (JS key order for JsObj/JsNumObj; enumeration order otherwise),
///  - doubles as raw bits (-0, NaN payloads count), strings as UTF-16 code units,
///  - the identity structure: an object met again is fed as a back-reference to its first occurrence, so a world
///    whose War/path/guest sharing differs has a different fingerprint even while all values are equal.
/// The per-day digest of the equivalence test is <c>Fingerprint.Of(sim)</c> = this over Sim.CaptureSave()
/// (world + RNG state + both path caches + ShoreW). Mixing is murmur3-fmix64 based: meant to catch accidental
/// differences, not adversarial ones.
/// </summary>
internal sealed class Fingerprint
{
    private ulong _a = 0x243F6A8885A308D3UL, _b = 0x13198A2E03707344UL, _n;
    private readonly Dictionary<object, int> _ids = new(ReferenceEqualityComparer.Instance);

    private const ulong TNull = 0x6E756C6C6E756C6CUL, TNew = 0x4E45574E45574E45UL, TBack = 0x4241434B4241434BUL, THas = 0x4841534841534841UL,
        TLen = 0x4C454E4C454E4C45UL, TStr = 0x5354525354525354UL;

    public static string Of(Sim s) => Of(s.CaptureSave());

    public static string Of(object root)
    {
        var f = new Fingerprint();
        f.Ref(root);
        return f.Hex();
    }

    public int Objects => _ids.Count;

    private string Hex() => $"{Fmix(_a ^ _n):x16}{Fmix(_b + _n * 0x9E3779B97F4A7C15UL):x16}";

    private static ulong Fmix(ulong k)
    {
        k ^= k >> 33; k *= 0xff51afd7ed558ccdUL; k ^= k >> 33; k *= 0xc4ceb9fe1a85ec53UL; k ^= k >> 33;
        return k;
    }

    public void W(ulong w)
    {
        ulong k = Fmix(w ^ (_n * 0x9E3779B97F4A7C15UL));
        _a = BitOperations.RotateLeft(_a ^ k, 27) * 0x9E3779B97F4A7C15UL + 0x52DCE729UL;
        _b = (BitOperations.RotateLeft(_b + k, 31) * 0xC2B2AE3D27D4EB4FUL) ^ _a;
        _n++;
    }

    public void D(double v) => W(BitConverter.DoubleToUInt64Bits(v));
    public void I(long v) => W((ulong)v);
    public void B(bool v) => W(v ? 0xB1UL : 0xB0UL);

    public void S(string s)
    {
        if (s == null) { W(TNull); return; }
        W(TStr ^ (ulong)s.Length);
        var u = MemoryMarshal.Cast<char, ulong>(s.AsSpan());
        foreach (var x in u) W(x);
        for (int i = u.Length * 4; i < s.Length; i++) W(s[i]);
    }

    public void N<U>(U? v) where U : struct
    {
        if (!v.HasValue) { W(TNull); return; }
        W(THas);
        ElemOf<U>.F(this, v.GetValueOrDefault());
    }

    public void Len(int n) => W(TLen ^ (ulong)n);

    public void Ref(object o)
    {
        if (o == null) { W(TNull); return; }
        if (o is string s) { S(s); return; }
        ref int id = ref CollectionsMarshal.GetValueRefOrAddDefault(_ids, o, out bool exists);
        if (exists) { W(TBack); W((ulong)id); return; }
        id = _ids.Count;                                     // set before recursing (the dictionary may grow)
        var (tid, feed) = Info(o.GetType());
        W(TNew);
        W(tid);
        feed(this, o);
    }

    // ---------------------------------------------------------------- per-type feeders (cached, lock-free reads)
    private static readonly ConcurrentDictionary<Type, (ulong Id, Action<Fingerprint, object> Feed)> Types = new();

    private static (ulong Id, Action<Fingerprint, object> Feed) Info(Type t) => Types.TryGetValue(t, out var x) ? x : Types.GetOrAdd(t, MakeInfo);

    private static (ulong, Action<Fingerprint, object>) MakeInfo(Type t)
    {
        ulong h = 0xcbf29ce484222325UL;
        foreach (char c in t.FullName ?? t.Name) h = (h ^ c) * 0x100000001b3UL;
        return (h, Make(t));
    }

    private static Action<Fingerprint, object> Gen(Type generic, params Type[] args) =>
        generic.MakeGenericType(args).GetMethod("Feed", BindingFlags.Public | BindingFlags.Static).CreateDelegate<Action<Fingerprint, object>>();

    private static Action<Fingerprint, object> Make(Type t)
    {
        if (t.IsPrimitive || t.IsEnum || t == typeof(decimal))
            return (f, o) => f.S(Convert.ToString(o, System.Globalization.CultureInfo.InvariantCulture));     // boxed scalar (not in sim state)
        if (t == typeof(byte[])) return (f, o) => { var b = (byte[])o; f.Len(b.Length); var u = MemoryMarshal.Cast<byte, ulong>(b); foreach (var x in u) f.W(x); for (int i = u.Length * 8; i < b.Length; i++) f.W(b[i]); };
        if (t.IsArray)
        {
            if (t.GetArrayRank() != 1) throw new NotSupportedException($"fingerprint: {t}");
            return Gen(typeof(ArrayF<>), t.GetElementType());
        }
        if (t.IsGenericType)
        {
            var d = t.GetGenericTypeDefinition();
            var a = t.GetGenericArguments();
            if (d == typeof(List<>)) return Gen(typeof(ListF<>), a);
            if (d == typeof(JsObj<>)) return Gen(typeof(JsObjF<>), a);
            if (d == typeof(JsNumObj<>)) return Gen(typeof(JsNumObjF<>), a);
            if (d == typeof(Dictionary<,>)) return Gen(typeof(DictF<,>), a);
            if (d == typeof(JsMap<,>)) return Gen(typeof(JsMapF<,>), a);
            if (d == typeof(HashSet<>)) return Gen(typeof(SetF<>), a);
        }
        if (typeof(Delegate).IsAssignableFrom(t)) return (f, o) => f.W(0xDE1E6A7E);       // hooks: presence only
        if (t.Namespace != null && t.Namespace.StartsWith("System", StringComparison.Ordinal)) throw new NotSupportedException($"fingerprint: unsupported type {t}");
        return ClassFeeder(t);
    }

    /// <summary>(f, o) => { var x = (T)o; f.D(x.A); f.I(x.B); f.Ref(x.C); ... } over all instance fields by name.</summary>
    private static Action<Fingerprint, object> ClassFeeder(Type t)
    {
        var f = Expression.Parameter(typeof(Fingerprint), "f");
        var o = Expression.Parameter(typeof(object), "o");
        var x = Expression.Variable(t, "x");
        var body = new List<Expression> { Expression.Assign(x, Expression.Convert(o, t)) };
        var fields = new List<FieldInfo>();
        for (var b = t; b != null && b != typeof(object); b = b.BaseType)
            fields.AddRange(b.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly));
        foreach (var fi in fields.OrderBy(q => q.Name, StringComparer.Ordinal).ThenBy(q => q.DeclaringType.FullName, StringComparer.Ordinal))
            body.Add(Feed(f, Expression.Field(x, fi), fi.FieldType));
        if (body.Count == 1) body.Add(Expression.Empty());
        return Expression.Lambda<Action<Fingerprint, object>>(Expression.Block(new[] { x }, body), f, o).Compile();
    }

    private static readonly MethodInfo MD = typeof(Fingerprint).GetMethod(nameof(D)), MI = typeof(Fingerprint).GetMethod(nameof(I)), MB = typeof(Fingerprint).GetMethod(nameof(B)),
        MS = typeof(Fingerprint).GetMethod(nameof(S)), MW = typeof(Fingerprint).GetMethod(nameof(W)), MRef = typeof(Fingerprint).GetMethod(nameof(Ref)), MN = typeof(Fingerprint).GetMethod(nameof(N));

    private static Expression Feed(Expression f, Expression v, Type t)
    {
        if (t == typeof(double)) return Expression.Call(f, MD, v);
        if (t == typeof(float)) return Expression.Call(f, MD, Expression.Convert(v, typeof(double)));
        if (t == typeof(bool)) return Expression.Call(f, MB, v);
        if (t == typeof(ulong)) return Expression.Call(f, MW, v);
        if (t == typeof(string)) return Expression.Call(f, MS, v);
        if (t.IsEnum) return Expression.Call(f, MI, Expression.Convert(Expression.Convert(v, Enum.GetUnderlyingType(t)), typeof(long)));
        if (t.IsPrimitive) return Expression.Call(f, MI, Expression.Convert(v, typeof(long)));
        if (Nullable.GetUnderlyingType(t) is Type u) return Expression.Call(f, MN.MakeGenericMethod(u), v);
        if (t.IsValueType) throw new NotSupportedException($"fingerprint: struct {t}");
        return Expression.Call(f, MRef, Expression.Convert(v, typeof(object)));
    }

    /// <summary>Typed element feeder (no boxing for numbers).</summary>
    private static class ElemOf<T>
    {
        public static readonly Action<Fingerprint, T> F = MakeElem();

        private static Action<Fingerprint, T> MakeElem()
        {
            var f = Expression.Parameter(typeof(Fingerprint), "f");
            var v = Expression.Parameter(typeof(T), "v");
            return Expression.Lambda<Action<Fingerprint, T>>(Feed(f, v, typeof(T)), f, v).Compile();
        }
    }

    private static class ArrayF<T>
    {
        public static void Feed(Fingerprint f, object o)
        {
            var a = (T[])o;
            var e = ElemOf<T>.F;
            f.Len(a.Length);
            for (int i = 0; i < a.Length; i++) e(f, a[i]);
        }
    }

    private static class ListF<T>
    {
        public static void Feed(Fingerprint f, object o)
        {
            var l = (List<T>)o;
            var e = ElemOf<T>.F;
            f.Len(l.Count);
            for (int i = 0; i < l.Count; i++) e(f, l[i]);
        }
    }

    private static class JsObjF<V>
    {
        public static void Feed(Fingerprint f, object o) => Rec(f, (JsObj<V>)o);

        public static void Rec(Fingerprint f, JsObj<V> j)
        {
            var e = ElemOf<V>.F;
            f.Len(j.Count);
            foreach (var kv in j) { f.S(kv.Key); e(f, kv.Value); }     // JS order
        }
    }

    private static class JsNumObjF<V>
    {
        public static void Feed(Fingerprint f, object o) => JsObjF<V>.Rec(f, ((JsNumObj<V>)o).O);
    }

    private static class DictF<K, V>
    {
        public static void Feed(Fingerprint f, object o)
        {
            var d = (Dictionary<K, V>)o;
            f.Len(d.Count);
            foreach (var kv in d) { ElemOf<K>.F(f, kv.Key); ElemOf<V>.F(f, kv.Value); }
        }
    }

    private static class JsMapF<K, V>
    {
        public static void Feed(Fingerprint f, object o)
        {
            var d = (JsMap<K, V>)o;
            f.Len(d.Size);
            foreach (var kv in d) { ElemOf<K>.F(f, kv.Key); ElemOf<V>.F(f, kv.Value); }
        }
    }

    private static class SetF<T>
    {
        public static void Feed(Fingerprint f, object o)
        {
            var d = (HashSet<T>)o;
            f.Len(d.Count);
            foreach (var x in d) ElemOf<T>.F(f, x);
        }
    }
}
