using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using FD.Macro;

namespace SaveCheck;

/// <summary>
/// Generic object-graph walking, independent of the save code (plain reflection over ALL instance fields, public or
/// not, so hidden state would show up too). Containers are walked through their public enumeration (JS key order for
/// JsObj/JsNumObj, enumeration order for Dictionary/HashSet/JsMap).
/// </summary>
internal static class Graph
{
    public static bool IsLeaf(object o) => o is string || o is decimal || o.GetType().IsPrimitive || o.GetType().IsEnum;

    private static readonly Dictionary<Type, FieldInfo[]> FieldCache = new();
    private static readonly Dictionary<Type, (PropertyInfo K, PropertyInfo V)> PairCache = new();

    public static FieldInfo[] Fields(Type t)
    {
        lock (FieldCache)
        {
            if (FieldCache.TryGetValue(t, out var fs)) return fs;
            var list = new List<FieldInfo>();
            for (var x = t; x != null && x != typeof(object); x = x.BaseType)
                list.AddRange(x.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly));
            fs = list.OrderBy(f => f.DeclaringType.Name, StringComparer.Ordinal).ThenBy(f => f.Name, StringComparer.Ordinal).ToArray();
            FieldCache[t] = fs;
            return fs;
        }
    }

    private static (PropertyInfo K, PropertyInfo V) Pair(Type t)
    {
        lock (PairCache)
        {
            if (!PairCache.TryGetValue(t, out var p)) PairCache[t] = p = (t.GetProperty("Key"), t.GetProperty("Value"));
            return p;
        }
    }

    public static bool IsKeyed(object o)
    {
        var t = o.GetType();
        if (!t.IsGenericType) return false;
        var d = t.GetGenericTypeDefinition();
        return d == typeof(JsObj<>) || d == typeof(JsNumObj<>) || d == typeof(JsMap<,>) || d == typeof(Dictionary<,>);
    }

    /// <summary>Entries (key, value) of a keyed container in its iteration order.</summary>
    public static List<(object K, object V)> Entries(object o)
    {
        var outp = new List<(object, object)>();
        foreach (var kv in (IEnumerable)o)
        {
            var (pk, pv) = Pair(kv.GetType());
            outp.Add((pk.GetValue(kv), pv.GetValue(kv)));
        }
        return outp;
    }

    /// <summary>Children (label, value) of a non-leaf object.</summary>
    public static List<(string L, object V)> Children(object o, bool tolerant = false)
    {
        var outp = new List<(string, object)>();
        var t = o.GetType();
        if (o is Delegate) return outp;
        if (o is Array a)
        {
            int i = 0;
            foreach (var x in a) outp.Add(("[" + i++ + "]", x));      // any rank, row-major
            return outp;
        }
        if (o is IList l)
        {
            for (int i = 0; i < l.Count; i++) outp.Add(("[" + i + "]", l[i]));
            return outp;
        }
        if (IsKeyed(o))
        {
            foreach (var (k, v) in Entries(o)) outp.Add(("[" + Key(k) + "]", v));
            return outp;
        }
        if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(HashSet<>))
        {
            int i = 0;
            foreach (var x in (IEnumerable)o) outp.Add(("{" + i++ + "}", x));
            return outp;
        }
        if (t.Namespace != null && t.Namespace.StartsWith("System", StringComparison.Ordinal) && !t.IsValueType)
        {
            if (tolerant) return outp;                  // opaque (e.g. JSON options held by a static)
            throw new NotSupportedException($"graph walk: unsupported BCL type {t}");
        }
        foreach (var f in Fields(t)) outp.Add(("." + f.Name, f.GetValue(o)));
        return outp;
    }

    public static string Key(object k) => k is string s ? "\"" + s + "\"" : Convert.ToString(k, System.Globalization.CultureInfo.InvariantCulture);

    public static bool LeafEquals(object a, object b)
    {
        if (a is double da && b is double db) return BitConverter.DoubleToInt64Bits(da) == BitConverter.DoubleToInt64Bits(db);
        if (a is float fa && b is float fb) return BitConverter.SingleToInt32Bits(fa) == BitConverter.SingleToInt32Bits(fb);
        return a.GetType() == b.GetType() && a.Equals(b);
    }
}

/// <summary>
/// Compares two object graphs for equal values AND equal sharing: there must be a bijection between their reference
/// objects such that every path reaches corresponding objects (so "the same War in two relations" must again be one
/// object, and two distinct lists must stay distinct).
/// </summary>
internal sealed class GraphCompare
{
    private readonly Dictionary<object, object> _ab = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<object, object> _ba = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<object, string> _firstPath = new(ReferenceEqualityComparer.Instance);
    private readonly HashSet<object> _countedShared = new(ReferenceEqualityComparer.Instance);
    private readonly HashSet<object> _countedLost = new(ReferenceEqualityComparer.Instance);
    private readonly List<string> _path = new();
    /// <summary>first MaxErrors messages (the walk goes on to count everything)</summary>
    public readonly List<string> Errors = new();
    /// <summary>objects of graph a reached through more than one path (sharing that b reproduced)</summary>
    public readonly SortedDictionary<string, int> SharedByType = new(StringComparer.Ordinal);
    /// <summary>objects of graph a reached through more than one path whose copies in b are different objects</summary>
    public readonly SortedDictionary<string, int> LostByType = new(StringComparer.Ordinal);
    public int Objects, ErrorCount, MaxErrors = 12;

    public bool Ok => ErrorCount == 0;
    private string P => string.Concat(_path);

    private void Err(string msg)
    {
        ErrorCount++;
        if (Errors.Count < MaxErrors) Errors.Add($"{P}: {msg}");
    }

    public static GraphCompare Run(object a, object b, string rootName = "state")
    {
        var g = new GraphCompare();
        g._path.Add(rootName);
        g.Cmp(a, b);
        return g;
    }

    private void Cmp(object a, object b)
    {
        if (a == null || b == null) { if (a != b) Err($"{Show(a)} vs {Show(b)}"); return; }
        if (Graph.IsLeaf(a) || Graph.IsLeaf(b))
        {
            if (!Graph.LeafEquals(a, b)) Err($"{Show(a)} vs {Show(b)}");
            return;
        }
        if (_ab.TryGetValue(a, out var mb))
        {
            if (!ReferenceEquals(mb, b))
            {
                Err($"sharing lost: the original object here is the one first reached at {_firstPath[a]}, but the copies are two different objects");
                if (_countedLost.Add(a)) LostByType[Name(a.GetType())] = LostByType.GetValueOrDefault(Name(a.GetType())) + 1;
                if (!_ba.ContainsKey(b)) { _ba[b] = a; WalkChildren(a, b); }      // still compare values below
            }
            else if (_countedShared.Add(a)) SharedByType[Name(a.GetType())] = SharedByType.GetValueOrDefault(Name(a.GetType())) + 1;
            return;
        }
        if (_ba.TryGetValue(b, out var ma))
        {
            Err($"sharing added: the copy here is the object that stands for {_firstPath.GetValueOrDefault(ma, "?")}, but the originals differ");
            return;
        }
        _ab[a] = b; _ba[b] = a; _firstPath[a] = P;
        Objects++;
        WalkChildren(a, b);
    }

    private void WalkChildren(object a, object b)
    {
        if (a.GetType() != b.GetType()) { Err($"type {a.GetType()} vs {b.GetType()}"); return; }
        var ca = Graph.Children(a);
        var cb = Graph.Children(b);
        if (ca.Count != cb.Count) { Err($"{Name(a.GetType())}: {ca.Count} vs {cb.Count} children"); return; }
        for (int i = 0; i < ca.Count; i++)
        {
            if (ca[i].L != cb[i].L) { Err($"key/order differs at #{i}: {ca[i].L} vs {cb[i].L}"); return; }
            _path.Add(ca[i].L);
            Cmp(ca[i].V, cb[i].V);
            _path.RemoveAt(_path.Count - 1);
        }
    }

    private static string Show(object o) => o == null ? "null" : o is string s ? "\"" + (s.Length > 40 ? s.Substring(0, 40) + "…" : s) + "\"" : o is double d ? d.ToString("R", System.Globalization.CultureInfo.InvariantCulture) : Graph.IsLeaf(o) ? o.ToString() : o.GetType().Name;

    public static string Name(Type t) => t.IsGenericType ? t.Name.Substring(0, t.Name.IndexOf('`')) + "<" + string.Join(",", t.GetGenericArguments().Select(Name)) + ">" : t.Name;

    public string SharedSummary() => SharedByType.Count == 0 ? "none" : string.Join(", ", SharedByType.Select(kv => $"{kv.Key}×{kv.Value}"));
}

/// <summary>Audits for state that lives outside World/Sim (and so could not be saved).</summary>
internal static class Audit
{
    /// <summary>Sim's instance fields that are not covered by the save (anything but the known set is reported).</summary>
    public static readonly string[] KnownSimFields = { "W", "G", "Rng", "_pathCache", "NavCache", "ShoreW", "OnEvent", "Cp" };

    public static List<string> UnknownSimFields() =>
        typeof(Sim).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Select(f => f.Name).Where(n => Array.IndexOf(KnownSimFields, n) < 0).ToList();

    /// <summary>Rng's instance fields (only _s, Calls, Trace are known; _s and Calls are saved).</summary>
    public static List<string> UnknownRngFields() =>
        typeof(Rng).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Select(f => f.Name).Where(n => n != "_s" && n != "Calls" && n != "Trace").ToList();

    /// <summary>Not simulation state: other namespaces, compiler-generated and open generic types (their statics are
    /// per instantiation), the save codec (Save.cs: metadata caches) and the JSON options.</summary>
    private static bool Infra(Type t) =>
        t.Namespace != "FD.Macro" || t.Name.Contains('<') || t.ContainsGenericParameters
        || t.Name.StartsWith("Sv", StringComparison.Ordinal) || t == typeof(SaveCodec) || typeof(SvCodec).IsAssignableFrom(t)
        || t == typeof(Json) || t == typeof(JsObjConverterFactory) || t.DeclaringType == typeof(JsObjConverterFactory);

    /// <summary>Static fields of simulation types (FD.Macro namespace, minus save/JSON infrastructure and compiler-generated types).</summary>
    public static List<FieldInfo> StaticFields() =>
        typeof(Sim).Assembly.GetTypes().Where(t => !Infra(t))
            .SelectMany(t => t.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
            .Where(f => !f.IsLiteral && !f.Name.Contains('<'))
            .OrderBy(f => f.DeclaringType.FullName, StringComparer.Ordinal).ThenBy(f => f.Name, StringComparer.Ordinal).ToList();

    /// <summary>Non-readonly static fields outside the static data class D (should be none: "Statik değişken durum yok").</summary>
    public static List<string> MutableStatics() =>
        StaticFields().Where(f => !f.IsInitOnly && f.DeclaringType != typeof(D)).Select(f => $"{f.DeclaringType.Name}.{f.Name}").ToList();

    /// <summary>Content digest of everything reachable from the static fields (delegates skipped); must not change while sims run.</summary>
    public static string StaticContentDigest()
    {
        using var h = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        var ids = new Dictionary<object, int>(ReferenceEqualityComparer.Instance);
        foreach (var f in StaticFields())
        {
            h.AppendData(Encoding.UTF8.GetBytes($"\n{f.DeclaringType.FullName}.{f.Name}="));
            Feed(h, f.GetValue(null), ids);
        }
        return Convert.ToHexString(h.GetHashAndReset(), 0, 16).ToLowerInvariant();
    }

    private static void Feed(IncrementalHash h, object o, Dictionary<object, int> ids)
    {
        if (o == null) { h.AppendData("n;"u8); return; }
        if (o is Delegate) { h.AppendData("fn;"u8); return; }
        if (Graph.IsLeaf(o))
        {
            string s = o is double d ? BitConverter.DoubleToInt64Bits(d).ToString() : o is float x ? BitConverter.SingleToInt32Bits(x).ToString() : Convert.ToString(o, System.Globalization.CultureInfo.InvariantCulture);
            h.AppendData(Encoding.UTF8.GetBytes(o.GetType().Name + ":" + s.Length + ":" + s + ";"));
            return;
        }
        if (ids.TryGetValue(o, out int id)) { h.AppendData(Encoding.UTF8.GetBytes("#" + id + ";")); return; }
        ids[o] = ids.Count;
        h.AppendData(Encoding.UTF8.GetBytes(o.GetType().Name + "{"));
        foreach (var (l, v) in Graph.Children(o, tolerant: true))
        {
            h.AppendData(Encoding.UTF8.GetBytes(l + "="));
            Feed(h, v, ids);
        }
        h.AppendData("}"u8);
    }

    /// <summary>Reference objects reachable from the static fields.</summary>
    public static HashSet<object> StaticObjects()
    {
        var seen = new HashSet<object>(ReferenceEqualityComparer.Instance);
        var stack = new Stack<object>();
        foreach (var f in StaticFields()) { var v = f.GetValue(null); if (v != null) stack.Push(v); }
        while (stack.Count > 0)
        {
            var o = stack.Pop();
            if (o == null || o is Delegate || Graph.IsLeaf(o) || !seen.Add(o)) continue;
            foreach (var (_, v) in Graph.Children(o, tolerant: true)) if (v != null && !Graph.IsLeaf(v)) stack.Push(v);
        }
        return seen;
    }

    /// <summary>Objects of the save graph that are also reachable from static fields (their identity would be lost by a save).</summary>
    public static List<string> SharedWithStatics(object root, HashSet<object> statics, int max = 10)
    {
        var outp = new List<string>();
        var seen = new HashSet<object>(ReferenceEqualityComparer.Instance);
        var stack = new Stack<(object, string)>();
        stack.Push((root, "state"));
        while (stack.Count > 0 && outp.Count < max)
        {
            var (o, p) = stack.Pop();
            if (o == null || Graph.IsLeaf(o) || !seen.Add(o)) continue;
            if (statics.Contains(o)) { outp.Add($"{p} ({GraphCompare.Name(o.GetType())})"); continue; }
            foreach (var (l, v) in Graph.Children(o)) if (v != null && !Graph.IsLeaf(v)) stack.Push((v, p + l));
        }
        return outp;
    }
}
