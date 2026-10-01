using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace FD.Macro;

/// <summary>
/// A plain JavaScript object used as a string-keyed record (<c>Record&lt;string, V&gt;</c>), with
/// exactly JS's property order:
///  - array-index keys ("0", "1", ... up to 2^32-2, canonical form) come first, ascending;
///  - all other keys follow in insertion order;
///  - assigning an existing key keeps its position; <c>delete</c> then re-adding moves it to the end.
/// Iteration order matters for the golden test (floating-point sums, "first match" loops, RNG
/// consumption order), so every TS <c>Record</c> in the simulation state uses this type.
///
/// Missing keys read as "undefined": use <see cref="TryGet"/>, <see cref="Has"/>, or the
/// <c>Get</c> extension for <c>JsObj&lt;double&gt;</c> which returns <c>double?</c> (null = undefined),
/// so TS <c>(o[k] ?? 0)</c> ports to <c>(o.Get(k) ?? 0)</c>.
///
/// Enumeration (foreach, <see cref="Keys"/>) follows V8's for-in semantics closely enough for
/// the simulation: keys added during a foreach are not visited; keys deleted before being
/// visited are skipped; values are read at visit time.
/// </summary>
public sealed class JsObj<V> : IEnumerable<KeyValuePair<string, V>>
{
    private readonly Dictionary<string, int> _index = new(StringComparer.Ordinal);
    private readonly List<string> _keys = new();
    private readonly List<V> _vals = new();
    private readonly List<bool> _live = new();
    private int _count, _dead, _indexKeys, _enumerating;

    public JsObj() { }

    public int Count => _count;

    public bool Has(string key) => _index.ContainsKey(key);

    public bool TryGet(string key, out V value)
    {
        if (_index.TryGetValue(key, out int i)) { value = _vals[i]; return true; }
        value = default;
        return false;
    }

    /// <summary>Value or <paramref name="fallback"/> when the key is absent (TS <c>o[k] ?? fallback</c> for non-nullable V).</summary>
    public V GetOr(string key, V fallback) => _index.TryGetValue(key, out int i) ? _vals[i] : fallback;

    /// <summary>Raw indexer: get returns default(V) when missing (use only for reference-type V where null = undefined).</summary>
    public V this[string key]
    {
        get => _index.TryGetValue(key, out int i) ? _vals[i] : default;
        set => Set(key, value);
    }

    public void Set(string key, V value)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (_index.TryGetValue(key, out int i)) { _vals[i] = value; return; }
        if (_enumerating == 0 && _dead > 8 && _dead > _count) Compact();
        _index[key] = _keys.Count;
        _keys.Add(key);
        _vals.Add(value);
        _live.Add(true);
        _count++;
        if (IsArrayIndex(key)) _indexKeys++;
    }

    /// <summary>JS <c>delete o[k]</c>. Returns true if the key existed.</summary>
    public bool Delete(string key)
    {
        if (!_index.Remove(key, out int i)) return false;
        _live[i] = false;
        _vals[i] = default;
        _count--;
        _dead++;
        if (IsArrayIndex(key)) _indexKeys--;
        return true;
    }

    public void Clear()
    {
        _index.Clear(); _keys.Clear(); _vals.Clear(); _live.Clear();
        _count = _dead = _indexKeys = 0;
    }

    /// <summary>Snapshot of the keys in JS order (<c>Object.keys(o)</c>).</summary>
    public List<string> Keys()
    {
        var outp = new List<string>(_count);
        if (_indexKeys > 0)
        {
            var idx = new List<(uint n, string k)>(_indexKeys);
            for (int i = 0; i < _keys.Count; i++)
                if (_live[i] && IsArrayIndex(_keys[i])) idx.Add((uint.Parse(_keys[i], CultureInfo.InvariantCulture), _keys[i]));
            idx.Sort((a, b) => a.n.CompareTo(b.n));
            foreach (var (_, k) in idx) outp.Add(k);
            for (int i = 0; i < _keys.Count; i++)
                if (_live[i] && !IsArrayIndex(_keys[i])) outp.Add(_keys[i]);
        }
        else
        {
            for (int i = 0; i < _keys.Count; i++) if (_live[i]) outp.Add(_keys[i]);
        }
        return outp;
    }

    /// <summary>Snapshot of values in JS order (<c>Object.values(o)</c>).</summary>
    public List<V> Values()
    {
        var outp = new List<V>(_count);
        foreach (var k in Keys()) outp.Add(_vals[_index[k]]);
        return outp;
    }

    /// <summary>Snapshot of entries in JS order (<c>Object.entries(o)</c>).</summary>
    public List<KeyValuePair<string, V>> Entries()
    {
        var outp = new List<KeyValuePair<string, V>>(_count);
        foreach (var k in Keys()) outp.Add(new KeyValuePair<string, V>(k, _vals[_index[k]]));
        return outp;
    }

    /// <summary>Shallow copy with the same key order (<c>{ ...o }</c>).</summary>
    public JsObj<V> Clone()
    {
        var c = new JsObj<V>();
        foreach (var k in Keys()) c.Set(k, _vals[_index[k]]);
        return c;
    }

    public Enumerator GetEnumerator() => new Enumerator(this);
    IEnumerator<KeyValuePair<string, V>> IEnumerable<KeyValuePair<string, V>>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>for-in style enumerator: fixed key set at start, skips keys deleted meanwhile, reads live values.</summary>
    public struct Enumerator : IEnumerator<KeyValuePair<string, V>>
    {
        private readonly JsObj<V> _o;
        private readonly List<string> _snapshot; // only when array-index keys exist
        private readonly int _end;
        private int _pos;
        private KeyValuePair<string, V> _cur;
        private bool _disposed;

        internal Enumerator(JsObj<V> o)
        {
            _o = o;
            _snapshot = o._indexKeys > 0 ? o.Keys() : null;
            _end = _snapshot != null ? _snapshot.Count : o._keys.Count;
            _pos = -1;
            _cur = default;
            _disposed = false;
            o._enumerating++;
        }

        public KeyValuePair<string, V> Current => _cur;
        object IEnumerator.Current => _cur;

        public bool MoveNext()
        {
            while (++_pos < _end)
            {
                if (_snapshot != null)
                {
                    string k = _snapshot[_pos];
                    if (_o._index.TryGetValue(k, out int i)) { _cur = new KeyValuePair<string, V>(k, _o._vals[i]); return true; }
                }
                else if (_o._live[_pos])
                {
                    _cur = new KeyValuePair<string, V>(_o._keys[_pos], _o._vals[_pos]);
                    return true;
                }
            }
            return false;
        }

        public void Reset() => throw new NotSupportedException();

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _o._enumerating--;
        }
    }

    private void Compact()
    {
        var keys = new List<string>(_count);
        var vals = new List<V>(_count);
        for (int i = 0; i < _keys.Count; i++)
            if (_live[i]) { keys.Add(_keys[i]); vals.Add(_vals[i]); }
        _keys.Clear(); _vals.Clear(); _live.Clear(); _index.Clear();
        for (int i = 0; i < keys.Count; i++) { _index[keys[i]] = i; _keys.Add(keys[i]); _vals.Add(vals[i]); _live.Add(true); }
        _dead = 0;
    }

    /// <summary>ECMAScript array index: canonical decimal string of an integer in [0, 2^32 - 2].</summary>
    public static bool IsArrayIndex(string k)
    {
        int n = k.Length;
        if (n == 0 || n > 10) return false;
        if (k[0] == '0') return n == 1;
        ulong v = 0;
        for (int i = 0; i < n; i++)
        {
            char c = k[i];
            if (c < '0' || c > '9') return false;
            v = v * 10 + (ulong)(c - '0');
        }
        return v <= 4294967294UL;
    }
}

/// <summary>
/// A JS object used as a number-keyed record (<c>Record&lt;number, V&gt;</c>): keys are stored as
/// strings exactly like JS (so non-negative integers iterate ascending, others by insertion).
/// </summary>
public sealed class JsNumObj<V> : IEnumerable<KeyValuePair<int, V>>
{
    internal readonly JsObj<V> O = new();

    private static string K(int k) => k.ToString(CultureInfo.InvariantCulture);

    public int Count => O.Count;
    public bool Has(int key) => O.Has(K(key));
    public bool TryGet(int key, out V value) => O.TryGet(K(key), out value);
    public V GetOr(int key, V fallback) => O.GetOr(K(key), fallback);
    public V this[int key] { get => O[K(key)]; set => O.Set(K(key), value); }
    public void Set(int key, V value) => O.Set(K(key), value);
    public bool Delete(int key) => O.Delete(K(key));
    public void Clear() => O.Clear();

    public List<int> Keys()
    {
        var ks = O.Keys();
        var outp = new List<int>(ks.Count);
        foreach (var k in ks) outp.Add(int.Parse(k, CultureInfo.InvariantCulture));
        return outp;
    }

    public List<V> Values() => O.Values();

    public List<KeyValuePair<int, V>> Entries()
    {
        var outp = new List<KeyValuePair<int, V>>(O.Count);
        foreach (var kv in O.Entries()) outp.Add(new KeyValuePair<int, V>(int.Parse(kv.Key, CultureInfo.InvariantCulture), kv.Value));
        return outp;
    }

    public JsNumObj<V> Clone()
    {
        var c = new JsNumObj<V>();
        foreach (var kv in O.Entries()) c.O.Set(kv.Key, kv.Value);
        return c;
    }

    public IEnumerator<KeyValuePair<int, V>> GetEnumerator()
    {
        foreach (var kv in O) yield return new KeyValuePair<int, V>(int.Parse(kv.Key, CultureInfo.InvariantCulture), kv.Value);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>Helpers for the very common number records (stock, pop, metrics, effects...).</summary>
public static class JsObjExt
{
    /// <summary>TS <c>o[k]</c> for a number record: null when the key is absent (undefined).</summary>
    public static double? Get(this JsObj<double> o, string key) => o.TryGet(key, out double v) ? v : null;

    /// <summary>TS <c>o[k]</c> for a number-keyed number record.</summary>
    public static double? Get(this JsNumObj<double> o, int key) => o.TryGet(key, out double v) ? v : null;

    /// <summary>TS <c>o[k] = (o[k] ?? 0) + v</c>.</summary>
    public static void Add(this JsObj<double> o, string key, double v) => o.Set(key, (o.Get(key) ?? 0) + v);

    /// <summary>TS <c>o[k] = (o[k] ?? 0) + v</c>.</summary>
    public static void Add(this JsNumObj<double> o, int key, double v) => o.Set(key, (o.Get(key) ?? 0) + v);
}

/// <summary>
/// JS <c>Map</c> with insertion-order iteration (deleting then re-adding moves a key to the end).
/// Use for TS Maps that are iterated; plain lookups may use Dictionary.
/// </summary>
public sealed class JsMap<K, V> : IEnumerable<KeyValuePair<K, V>>
{
    private readonly Dictionary<K, int> _index = new();
    private readonly List<K> _keys = new();
    private readonly List<V> _vals = new();
    private readonly List<bool> _live = new();
    private int _count, _dead;

    public int Size => _count;
    public bool Has(K k) => _index.ContainsKey(k);
    public V Get(K k) => _index.TryGetValue(k, out int i) ? _vals[i] : default;
    public bool TryGet(K k, out V v) { if (_index.TryGetValue(k, out int i)) { v = _vals[i]; return true; } v = default; return false; }

    public JsMap<K, V> Set(K k, V v)
    {
        if (_index.TryGetValue(k, out int i)) { _vals[i] = v; return this; }
        if (_dead > 8 && _dead > _count) Compact();
        _index[k] = _keys.Count; _keys.Add(k); _vals.Add(v); _live.Add(true); _count++;
        return this;
    }

    public bool Delete(K k)
    {
        if (!_index.Remove(k, out int i)) return false;
        _live[i] = false; _vals[i] = default; _keys[i] = default; _count--; _dead++;
        return true;
    }

    public void Clear() { _index.Clear(); _keys.Clear(); _vals.Clear(); _live.Clear(); _count = _dead = 0; }

    public List<K> Keys() { var o = new List<K>(_count); for (int i = 0; i < _keys.Count; i++) if (_live[i]) o.Add(_keys[i]); return o; }
    public List<V> Values() { var o = new List<V>(_count); for (int i = 0; i < _keys.Count; i++) if (_live[i]) o.Add(_vals[i]); return o; }

    /// <summary>Snapshot iteration (safe against mutation; JS would also visit keys added meanwhile — avoid relying on that).</summary>
    public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
    {
        var snap = new List<KeyValuePair<K, V>>(_count);
        for (int i = 0; i < _keys.Count; i++) if (_live[i]) snap.Add(new KeyValuePair<K, V>(_keys[i], _vals[i]));
        return snap.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void Compact()
    {
        var ks = Keys(); var vs = Values();
        _index.Clear(); _keys.Clear(); _vals.Clear(); _live.Clear(); _dead = 0;
        for (int i = 0; i < ks.Count; i++) { _index[ks[i]] = i; _keys.Add(ks[i]); _vals.Add(vs[i]); _live.Add(true); }
    }
}
