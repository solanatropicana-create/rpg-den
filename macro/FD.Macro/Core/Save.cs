using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

// Kayıt/yükleme (DESIGN-FAZ1.md A2): kaydet → yükle → devam et, kesintisiz koşuyla gün gün aynı.
//
// File: gzip-compressed UTF-8 JSON
//   {"format":"fd-macro-save","version":5,"day":D,"seed":S,"state":{ SaveState }}
// SaveState = World + the Sim state that lives outside it (RNG state, land path cache, nav cache, ShoreW), all
// under one root so a path list shared by a trade route, an agent and a cache stays one list after loading.
//
// The serializer is generic and reflection based (public instance fields and public get/set properties), so
// fields added to Types.cs are saved without touching this file:
//  - reference identity is preserved: an object referenced more than once gets "$id": n at its first occurrence
//    and every later occurrence is written as {"$ref": n} (lists/arrays/sets then use {"$id": n, "$values": [...]});
//    objects referenced once carry no id. Cycles are fine (an object is registered before its members are read).
//  - doubles round-trip bit-exactly: finite values as shortest round-trip JSON numbers; -0, NaN (with payload when
//    not the default NaN), Infinity and -Infinity as the strings "-0", "NaN" ("NaN:<hex bits>"), "Infinity", "-Infinity".
//  - JsObj / JsNumObj are written as JSON objects in their current JS key order and rebuilt by inserting in that
//    order (keys starting with '$' are written with one extra '$'). Dictionary / HashSet / JsMap: enumeration order.
//  - member values equal to the constructor default are omitted (also nulls); on load every object is created with
//    its parameterless constructor, so omitted members get exactly that default back.
//  - unsupported shapes (object/interface/abstract members, delegates, structs, polymorphic values, non-public
//    instance fields = hidden state) make Save throw instead of silently losing state. A public member that is
//    really transient can be marked [NoSave].
// Loading is strict: an unknown member name is an error; a member missing from the file keeps its constructor
// default (lets saves from before a field was added load).

namespace FD.Macro;

/// <summary>Excludes a public field/property from save files (only for transient values that are rebuilt).</summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class NoSaveAttribute : Attribute { }

public sealed partial class Sim
{
    /// <summary>
    /// Writes the whole simulation state to <paramref name="path"/> (gzip-compressed JSON; written to
    /// <c>path.tmp</c> first and then renamed, so a crash never leaves a half-written save). Call between
    /// <see cref="Step"/>s, not from a <see cref="Cp"/>/<see cref="OnEvent"/> hook. Nothing in the Sim is changed.
    /// </summary>
    public void Save(string path)
    {
        string tmp = path + ".tmp";
        try
        {
            using (var fs = new FileStream(tmp, FileMode.Create, FileAccess.Write, FileShare.None, 1 << 16))
                Save(fs);
            File.Move(tmp, path, overwrite: true);
        }
        catch
        {
            try { File.Delete(tmp); } catch (IOException) { } catch (UnauthorizedAccessException) { }
            throw;
        }
    }

    /// <summary>Writes the save to <paramref name="stream"/> (left open). <paramref name="compress"/>: gzip (default) or plain JSON.</summary>
    public void Save(Stream stream, bool compress = true) => SaveCodec.WriteSave(stream, CaptureSave(), compress);

    /// <summary>Loads a save written by <see cref="Save(string)"/> into a new Sim that continues exactly like the saved one.</summary>
    public static Sim Load(string path)
    {
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 16);
        return Load(fs);
    }

    /// <summary>Loads a save (gzip or plain JSON) from <paramref name="stream"/> (read to the end, left open).</summary>
    public static Sim Load(Stream stream) => FromSave(SaveCodec.ReadSave(stream));

    /// <summary>The save root around the live objects (no copies): world, RNG, caches.</summary>
    internal SaveState CaptureSave() => new SaveState
    {
        World = W, RngState = Rng.State(), RngCalls = Rng.Calls, PathCache = _pathCache, NavCache = NavCache, ShoreW = ShoreW,
    };

    /// <summary>Rebuilds a Sim from a loaded save root (caches refilled in their saved order).</summary>
    internal static Sim FromSave(SaveState st)
    {
        if (st?.World == null) throw new InvalidDataException("save: no world");
        var s = FromWorld(st.World);
        s.Rng.SetState(st.RngState);
        s.Rng.Calls = st.RngCalls;
        if (st.PathCache != null) foreach (var kv in st.PathCache) s._pathCache.Add(kv.Key, kv.Value);
        if (st.NavCache != null) foreach (var kv in st.NavCache) s.NavCache.Add(kv.Key, kv.Value);
        s.ShoreW = st.ShoreW;
        return s;
    }

    /// <summary>Read-only view of the land path cache (tests: state digests).</summary>
    internal IReadOnlyDictionary<string, List<int>> PathCacheView => _pathCache;
}

/// <summary>Root object of a save file: the world plus the Sim state that lives outside it.</summary>
public sealed class SaveState
{
    public World World;
    /// <summary>RNG state (equals World.RngState after every Step; differs right after <c>new Sim(seed)</c>).</summary>
    public double RngState;
    /// <summary>Rng.Calls debug counter (restored so traces/hash lines continue seamlessly).</summary>
    public long RngCalls;
    /// <summary>Sim's land path cache in insertion order; values are shared with agents/routes; null = no path.</summary>
    public Dictionary<string, List<int>> PathCache;
    /// <summary>Sim.NavCache (land + sea paths), same conventions.</summary>
    public Dictionary<string, List<int>> NavCache;
    public byte[] ShoreW;
}

/// <summary>Numbers about one written save.</summary>
public sealed class SaveInfo
{
    /// <summary>uncompressed JSON size</summary>
    public long JsonBytes;
    /// <summary>bytes written to the stream (= JsonBytes when not compressed)</summary>
    public long Bytes;
    /// <summary>distinct reference objects (class instances, lists, arrays, records...)</summary>
    public int Objects;
    /// <summary>objects referenced more than once (written with "$id")</summary>
    public int Shared;
    /// <summary>"$ref" occurrences</summary>
    public int Refs;
}

/// <summary>Save file reader/writer (see the comment at the top of this file).</summary>
public static class SaveCodec
{
    public const string Format = "fd-macro-save";
    /// <summary>2: Faz 1b-3 (araştırma, çağ, alt sınıf, harika ve mevsim alanları kalktı; kademe alanları geldi). Sürüm 1 kayıtlar
    /// açılmaz (alan adları değişti). 3: Faz 1b-5 (40 günlük takvim: gün sayaçlarının, zamanlayıcıların ve ajan ilerlemesinin
    /// anlamı değişti; Agent.Progress artık gün cinsinden). Sürüm 2 kayıtlar açılmaz. 4: Faz 1b-6 (devlet, inanç, örgüt, esaret;
    /// Civ.Cls kalktı). Sürüm 3 kayıtlar açılmaz. 5: Faz 1b-7 (yerleşim durumu, istikrar, iç kriz, fırsat merkezleri; sürüm 4 kayıtlar
    /// açılır ama yeni alanlar boş başlar, birebir devam garantisi yok: MinVersion 5).</summary>
    public const int Version = 5;
    /// <summary>bu yapının açabildiği en eski sürüm</summary>
    public const int MinVersion = 5;

    internal static readonly JsonWriterOptions WriterOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, Indented = false, SkipValidation = true,
    };

    internal static readonly JsonReaderOptions ReaderOptions = new() { MaxDepth = 4096, CommentHandling = JsonCommentHandling.Disallow };

    /// <summary>
    /// Writes a save file. <paramref name="preserveReferences"/> = false writes every occurrence of a shared object in
    /// full (loads as separate copies: only for tests showing that sharing matters).
    /// </summary>
    public static SaveInfo WriteSave(Stream stream, SaveState st, bool compress = true, bool preserveReferences = true)
    {
        if (st?.World == null) throw new ArgumentException("save: no world", nameof(st));
        var info = new SaveInfo();
        var outer = new CountingStream(stream);
        Stream z = compress ? new GZipStream(outer, CompressionLevel.Optimal, leaveOpen: true) : outer;
        var inner = new CountingStream(z);
        using (var j = new Utf8JsonWriter(inner, WriterOptions))
        {
            var w = new SvWriter(j, preserveReferences);
            var codec = SvTypes.Codec(typeof(SaveState));
            if (preserveReferences) w.ScanRef(st, codec);
            j.WriteStartObject();
            j.WriteString("format", Format);
            j.WriteNumber("version", Version);
            j.WriteNumber("day", st.World.Day);
            j.WritePropertyName("seed");
            w.WriteDouble(st.World.Seed);
            j.WritePropertyName("state");
            w.WriteRef(st, codec);
            j.WriteEndObject();
            j.Flush();
            w.Fill(info);
        }
        if (compress) z.Dispose();     // gzip trailer (outer and the caller's stream stay open)
        stream.Flush();
        info.JsonBytes = inner.Count;
        info.Bytes = outer.Count;
        return info;
    }

    /// <summary>Reads a save (gzip or plain JSON) from the rest of <paramref name="stream"/>.</summary>
    public static SaveState ReadSave(Stream stream)
    {
        var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ReadSave(new ArraySegment<byte>(ms.GetBuffer(), 0, (int)ms.Length));
    }

    /// <summary>Reads a save from its bytes (gzip or plain JSON).</summary>
    public static SaveState ReadSave(byte[] data) => ReadSave(new ArraySegment<byte>(data));

    private static SaveState ReadSave(ArraySegment<byte> data)
    {
        var json = Unwrap(data);
        var r = new Utf8JsonReader(json, ReaderOptions);
        var rd = new SvReader();
        SvReader.Next(ref r);
        if (r.TokenType != JsonTokenType.StartObject) throw rd.Bad(ref r, "not a save file (JSON object expected)");
        SvReader.Next(ref r);
        if (r.TokenType != JsonTokenType.PropertyName || !r.ValueTextEquals("format"u8)) throw rd.Bad(ref r, "not a save file (no \"format\")");
        SvReader.Next(ref r);
        if (r.TokenType != JsonTokenType.String || r.GetString() != Format) throw rd.Bad(ref r, $"not a save file (format must be \"{Format}\")");
        int version = 0;
        SaveState st = null;
        while (true)
        {
            SvReader.Next(ref r);
            if (r.TokenType == JsonTokenType.EndObject) break;
            if (r.ValueTextEquals("version"u8))
            {
                SvReader.Next(ref r);
                version = r.TokenType == JsonTokenType.Number ? r.GetInt32() : 0;
                if (version < MinVersion || version > Version) throw rd.Bad(ref r, $"unsupported save version {version} (this build reads {MinVersion}..{Version})");
            }
            else if (r.ValueTextEquals("state"u8))
            {
                if (version == 0) throw rd.Bad(ref r, "save version missing before \"state\"");
                SvReader.Next(ref r);
                st = (SaveState)SvTypes.Codec(typeof(SaveState)).Read(ref r, rd);
            }
            else { SvReader.Next(ref r); r.Skip(); }      // informational header fields (day, seed, ...)
        }
        if (st?.World == null) throw new InvalidDataException("save: no state/world");
        return st;
    }

    /// <summary>Any supported object graph → JSON bytes (same encoding as save files; tests and tools).</summary>
    public static byte[] ToJson<T>(T value, bool preserveReferences = true, bool indented = false)
    {
        var ms = new MemoryStream();
        using (var j = new Utf8JsonWriter(ms, new JsonWriterOptions { Encoder = WriterOptions.Encoder, Indented = indented, SkipValidation = true }))
        {
            var w = new SvWriter(j, preserveReferences);
            WriteValue(w, value);
            j.Flush();
        }
        return ms.ToArray();
    }

    /// <summary>Inverse of <see cref="ToJson{T}"/>.</summary>
    public static T FromJson<T>(byte[] json)
    {
        var r = new Utf8JsonReader(json, ReaderOptions);
        var rd = new SvReader();
        SvReader.Next(ref r);
        var v = SvTypes.Elem<T>().Read(ref r, rd);
        if (r.Read()) throw rd.Bad(ref r, "trailing data");
        return v;
    }

    private static void WriteValue<T>(SvWriter w, T value)
    {
        var e = SvTypes.Elem<T>();
        if (w.Preserve && e.HasRefs) e.Scan(w, value);
        e.Write(w, value);
    }

    /// <summary>gzip → JSON bytes (plain JSON passes through, UTF-8 BOM skipped).</summary>
    private static ReadOnlySpan<byte> Unwrap(ArraySegment<byte> data)
    {
        var d = data.AsSpan();
        if (d.Length >= 2 && d[0] == 0x1f && d[1] == 0x8b)
        {
            using var z = new GZipStream(new MemoryStream(data.Array, data.Offset, data.Count, writable: false), CompressionMode.Decompress);
            var ms = new MemoryStream(Math.Max(1024, data.Count * 8));
            z.CopyTo(ms);
            return new ReadOnlySpan<byte>(ms.GetBuffer(), 0, (int)ms.Length);
        }
        if (d.Length >= 3 && d[0] == 0xEF && d[1] == 0xBB && d[2] == 0xBF) return d.Slice(3);
        return d;
    }

    /// <summary>
    /// Write-only pass-through stream that counts bytes. It never closes the inner stream, and Flush() is not passed on:
    /// the JSON writer flushes every 64 KB, which on a GZipStream would emit a deflate sync block each time.
    /// </summary>
    private sealed class CountingStream : Stream
    {
        private readonly Stream _s;
        public long Count;
        public CountingStream(Stream s) { _s = s; }
        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => Count; set => throw new NotSupportedException(); }
        public override void Flush() { }
        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) { _s.Write(buffer, offset, count); Count += count; }
        public override void Write(ReadOnlySpan<byte> buffer) { _s.Write(buffer); Count += buffer.Length; }
    }
}

// ==================================================================== writer / reader state

internal static class SvNames
{
    public static readonly JsonEncodedText Id = JsonEncodedText.Encode("$id");
    public static readonly JsonEncodedText Ref = JsonEncodedText.Encode("$ref");
    public static readonly JsonEncodedText Values = JsonEncodedText.Encode("$values");
    public static readonly JsonEncodedText B64 = JsonEncodedText.Encode("$b64");
    public static readonly JsonEncodedText U16 = JsonEncodedText.Encode("$u16");

    /// <summary>Record key → property name: a leading '$' is doubled ("$id" stays a meta name).</summary>
    public static string Esc(string k) => k.Length > 0 && k[0] == '$' ? "$" + k : k;
}

internal sealed class SvWriter
{
    public readonly Utf8JsonWriter J;
    public readonly bool Preserve;
    // scan pass: reference count (≥ 1) per object; write pass: -id once a shared object has been written
    private readonly Dictionary<object, int> _seen = new(ReferenceEqualityComparer.Instance);
    private int _nextId, _shared, _refs, _depth;
    private static readonly long DefaultNaNBits = BitConverter.DoubleToInt64Bits(double.NaN);

    public SvWriter(Utf8JsonWriter j, bool preserve) { J = j; Preserve = preserve; }

    public void Fill(SaveInfo info) { info.Objects = _seen.Count; info.Shared = _shared; info.Refs = _refs; }

    /// <summary>Scan pass: counts references; visits the children of an object the first time only.</summary>
    public void ScanRef(object o, SvCodec codec)
    {
        if (o == null) return;
        ref int c = ref CollectionsMarshal.GetValueRefOrAddDefault(_seen, o, out bool exists);
        c++;
        if (exists) return;
        if (o.GetType() != codec.Type) throw Poly(o, codec);
        codec.ScanBody(this, o);
    }

    /// <summary>Write pass: null, {"$ref": n}, or the object (with "$id" when it is shared).</summary>
    public void WriteRef(object o, SvCodec codec)
    {
        if (o == null) { J.WriteNullValue(); return; }
        int id = 0;
        if (Preserve)
        {
            ref int c = ref CollectionsMarshal.GetValueRefOrNullRef(_seen, o);
            if (Unsafe.IsNullRef(ref c)) throw new InvalidOperationException($"save: {o.GetType()} reached in the write pass but not in the scan pass (state changed during Save?)");
            if (c < 0)
            {
                J.WriteStartObject(); J.WriteNumber(SvNames.Ref, -c); J.WriteEndObject();
                _refs++;
                return;
            }
            if (c > 1) { id = ++_nextId; c = -id; _shared++; }      // set before recursing (cycles)
        }
        else
        {
            if (o.GetType() != codec.Type) throw Poly(o, codec);
            if (_depth > 1000) throw new InvalidOperationException("save: object graph too deep (a cycle needs preserveReferences)");
        }
        _depth++;
        codec.WriteBody(this, o, id);
        _depth--;
        if (J.BytesPending > 1 << 16) J.Flush();
    }

    private static Exception Poly(object o, SvCodec codec) =>
        new NotSupportedException($"save: a {codec.Type} slot holds a {o.GetType()} (polymorphic values are not supported)");

    public void WriteDouble(double d)
    {
        if (double.IsFinite(d))
        {
            if (d == 0 && double.IsNegative(d)) J.WriteStringValue("-0");
            else J.WriteNumberValue(d);              // shortest round-trip text
        }
        else if (double.IsNaN(d))
        {
            long bits = BitConverter.DoubleToInt64Bits(d);
            J.WriteStringValue(bits == DefaultNaNBits ? "NaN" : "NaN:" + bits.ToString("x16", CultureInfo.InvariantCulture));
        }
        else J.WriteStringValue(d > 0 ? "Infinity" : "-Infinity");
    }

    public void WriteString(string s)
    {
        if (s == null) { J.WriteNullValue(); return; }
        if (HasLoneSurrogate(s))
        {
            // not representable as UTF-8 JSON text: raw UTF-16 code units, base64
            J.WriteStartObject();
            J.WriteBase64String(SvNames.U16, MemoryMarshal.AsBytes(s.AsSpan()));
            J.WriteEndObject();
            return;
        }
        J.WriteStringValue(s);
    }

    private static bool HasLoneSurrogate(string s)
    {
        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            if (!char.IsSurrogate(c)) continue;
            if (char.IsHighSurrogate(c) && i + 1 < s.Length && char.IsLowSurrogate(s[i + 1])) { i++; continue; }
            return true;
        }
        return false;
    }
}

internal sealed class SvReader
{
    private readonly List<object> _objs = new();      // id - 1 → object (reserved as null until built)

    public static void Next(ref Utf8JsonReader r)
    {
        if (!r.Read()) throw new InvalidDataException("save: unexpected end of data");
    }

    public InvalidDataException Bad(ref Utf8JsonReader r, string msg) =>
        new($"save: {msg} (token {r.TokenType} at byte {r.TokenStartIndex})");

    public void Expect(ref Utf8JsonReader r, JsonTokenType t)
    {
        if (r.TokenType != t) throw Bad(ref r, $"{t} expected");
    }

    /// <summary>
    /// At a StartObject: "$ref" → returns the referenced object (reader at its EndObject); "$id" → reserves the id
    /// (reader moved past the id value); otherwise returns null with slot = -1 and the reader on the first
    /// property name (or the EndObject).
    /// </summary>
    public object ReadMeta(ref Utf8JsonReader r, out int slot)
    {
        slot = -1;
        Next(ref r);
        if (r.TokenType != JsonTokenType.PropertyName) return null;
        if (r.ValueTextEquals("$ref"u8))
        {
            Next(ref r);
            if (r.TokenType != JsonTokenType.Number) throw Bad(ref r, "$ref must be a number");
            int id = r.GetInt32();
            Next(ref r);
            Expect(ref r, JsonTokenType.EndObject);
            if (id < 1 || id > _objs.Count || _objs[id - 1] == null) throw Bad(ref r, $"$ref {id} does not name an earlier object");
            return _objs[id - 1];
        }
        if (r.ValueTextEquals("$id"u8))
        {
            Next(ref r);
            if (r.TokenType != JsonTokenType.Number) throw Bad(ref r, "$id must be a number");
            int id = r.GetInt32();
            if (id != _objs.Count + 1) throw Bad(ref r, $"$id {id} out of sequence (expected {_objs.Count + 1})");
            _objs.Add(null);
            slot = id - 1;
            Next(ref r);
        }
        return null;
    }

    public void Fill(int slot, object o)
    {
        if (slot >= 0) _objs[slot] = o;
    }

    /// <summary>After "$id" of a wrapped list/array/set: expects "$values" and moves to its StartArray.</summary>
    public void Values(ref Utf8JsonReader r)
    {
        if (r.TokenType != JsonTokenType.PropertyName || !r.ValueTextEquals("$values"u8)) throw Bad(ref r, "\"$values\" expected");
        Next(ref r);
        Expect(ref r, JsonTokenType.StartArray);
    }

    /// <summary>End of a wrapped container: moves to and checks the EndObject.</summary>
    public void EndWrapped(ref Utf8JsonReader r, int slot)
    {
        if (slot < 0) return;
        Next(ref r);
        Expect(ref r, JsonTokenType.EndObject);
    }

    public static double ReadDouble(ref Utf8JsonReader r)
    {
        if (r.TokenType == JsonTokenType.Number) return r.GetDouble();
        if (r.TokenType == JsonTokenType.String)
        {
            string s = r.GetString();
            switch (s)
            {
                case "-0": return -0.0;
                case "NaN": return double.NaN;
                case "Infinity": return double.PositiveInfinity;
                case "-Infinity": return double.NegativeInfinity;
            }
            if (s.StartsWith("NaN:", StringComparison.Ordinal) && long.TryParse(s.AsSpan(4), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out long bits))
            {
                double d = BitConverter.Int64BitsToDouble(bits);
                if (double.IsNaN(d)) return d;
            }
            throw new InvalidDataException($"save: bad number \"{s}\" at byte {r.TokenStartIndex}");
        }
        throw new InvalidDataException($"save: number expected, got {r.TokenType} at byte {r.TokenStartIndex}");
    }

    public string ReadString(ref Utf8JsonReader r)
    {
        switch (r.TokenType)
        {
            case JsonTokenType.Null: return null;
            case JsonTokenType.String: return r.GetString();
            case JsonTokenType.StartObject:
                Next(ref r);
                if (r.TokenType != JsonTokenType.PropertyName || !r.ValueTextEquals("$u16"u8)) throw Bad(ref r, "\"$u16\" expected");
                Next(ref r);
                byte[] b = r.GetBytesFromBase64();
                Next(ref r);
                Expect(ref r, JsonTokenType.EndObject);
                if (b.Length % 2 != 0) throw Bad(ref r, "bad $u16 string");
                return new string(MemoryMarshal.Cast<byte, char>(b));
            default: throw Bad(ref r, "string expected");
        }
    }
}

// ==================================================================== type metadata

/// <summary>Codec of a reference type whose identity is tracked (everything but string).</summary>
internal abstract class SvCodec
{
    public readonly Type Type;
    protected SvCodec(Type t) { Type = t; }
    /// <summary>Resolves child codecs (called after registration, so recursive types work).</summary>
    public virtual void Init() { }
    /// <summary>Scan pass over the children (the object itself is already counted).</summary>
    public abstract void ScanBody(SvWriter w, object o);
    /// <summary>Writes the object; <paramref name="id"/> &gt; 0 → include "$id".</summary>
    public abstract void WriteBody(SvWriter w, object o, int id);
    /// <summary>Reads a value positioned on its first token (Null, StartObject, StartArray or String).</summary>
    public abstract object Read(ref Utf8JsonReader r, SvReader rd);
}

/// <summary>Writer/reader of a value of static type T (no boxing for numbers).</summary>
internal abstract class SvElem<T>
{
    public virtual bool HasRefs => false;
    public virtual void Scan(SvWriter w, T v) { }
    public abstract void Write(SvWriter w, T v);
    public abstract T Read(ref Utf8JsonReader r, SvReader rd);
    /// <summary>v equals the member default d (the member is then omitted). Bitwise for floats; both null for tracked references.</summary>
    public abstract bool IsDefault(T v, T d);
}

internal static class SvTypes
{
    private static readonly object Lock = new();
    private static readonly Dictionary<Type, SvCodec> Codecs = new();
    private static readonly Dictionary<Type, object> Elems = new();

    public static SvCodec Codec(Type t)
    {
        lock (Lock)
        {
            try { return CodecLocked(t); }
            catch { Codecs.Clear(); Elems.Clear(); throw; }     // never keep half-built metadata
        }
    }

    public static SvElem<T> Elem<T>()
    {
        lock (Lock)
        {
            try { return ElemLocked<T>(); }
            catch { Codecs.Clear(); Elems.Clear(); throw; }
        }
    }

    internal static SvCodec CodecLocked(Type t)
    {
        if (Codecs.TryGetValue(t, out var c)) return c;
        c = Create(t);
        Codecs[t] = c;                 // before Init: recursive types
        c.Init();
        return c;
    }

    internal static SvElem<T> ElemLocked<T>()
    {
        var t = typeof(T);
        if (Elems.TryGetValue(t, out var e)) return (SvElem<T>)e;
        object x;
        if (t == typeof(double)) x = new DoubleElem();
        else if (t == typeof(float)) x = new SingleElem();
        else if (t == typeof(int)) x = new Int32Elem();
        else if (t == typeof(long)) x = new Int64Elem();
        else if (t == typeof(bool)) x = new BoolElem();
        else if (t == typeof(string)) x = new StringElem();
        else if (t == typeof(sbyte) || t == typeof(short) || t == typeof(byte) || t == typeof(ushort) || t == typeof(uint) || t == typeof(char))
            x = Activator.CreateInstance(typeof(SmallIntElem<>).MakeGenericType(t));
        else if (t == typeof(ulong)) x = new UInt64Elem();
        else if (t.IsEnum) x = Activator.CreateInstance(typeof(EnumElem<>).MakeGenericType(t));
        else if (Nullable.GetUnderlyingType(t) is Type u) x = Activator.CreateInstance(typeof(NullableElem<>).MakeGenericType(u));
        else if (!t.IsValueType) x = Activator.CreateInstance(typeof(RefElem<>).MakeGenericType(t), CodecLocked(t));
        else throw new NotSupportedException($"save: unsupported value type {t} (only numbers, bool, char, enums and their nullables)");
        Elems[t] = x;
        return (SvElem<T>)x;
    }

    private static SvCodec Create(Type t)
    {
        if (t == typeof(string)) throw new InvalidOperationException("save: string has no codec");
        if (t == typeof(object)) throw new NotSupportedException("save: members of type object are not supported (no type information)");
        if (t.IsArray)
        {
            if (t.GetArrayRank() != 1) throw new NotSupportedException($"save: multi-dimensional array {t} is not supported");
            if (t == typeof(byte[])) return new BytesCodec();
            return Make(typeof(ArrayCodec<>), t.GetElementType());
        }
        if (t.IsGenericType)
        {
            var d = t.GetGenericTypeDefinition();
            var a = t.GetGenericArguments();
            if (d == typeof(List<>)) return Make(typeof(ListCodec<>), a);
            if (d == typeof(JsObj<>)) return Make(typeof(JsObjCodec<>), a);
            if (d == typeof(JsNumObj<>)) return Make(typeof(JsNumObjCodec<>), a);
            if (d == typeof(JsMap<,>)) return Make(typeof(JsMapCodec<,>), a);
            if (d == typeof(HashSet<>)) return Make(typeof(HashSetCodec<>), a);
            if (d == typeof(Dictionary<,>))
            {
                if (a[0] == typeof(string)) return Make(typeof(StringDictCodec<>), a[1]);
                if (a[0] == typeof(int)) return Make(typeof(IntDictCodec<>), a[1]);
                return Make(typeof(PairDictCodec<,>), a);
            }
        }
        if (t.IsInterface || t.IsAbstract) throw new NotSupportedException($"save: interface/abstract member type {t} is not supported");
        if (typeof(Delegate).IsAssignableFrom(t)) throw new NotSupportedException($"save: delegate type {t} cannot be saved (mark the member [NoSave] if it is transient)");
        if (typeof(System.Collections.IEnumerable).IsAssignableFrom(t)) throw new NotSupportedException($"save: collection type {t} is not supported (List, T[], JsObj, JsNumObj, JsMap, Dictionary, HashSet are)");
        if (t.IsValueType) throw new NotSupportedException($"save: struct {t} is not supported");
        return new ClassCodec(t);
    }

    private static SvCodec Make(Type generic, params Type[] args) => (SvCodec)Activator.CreateInstance(generic.MakeGenericType(args));
}

// ---------------------------------------------------------------- scalar elements

internal sealed class DoubleElem : SvElem<double>
{
    public override void Write(SvWriter w, double v) => w.WriteDouble(v);
    public override double Read(ref Utf8JsonReader r, SvReader rd) => SvReader.ReadDouble(ref r);
    public override bool IsDefault(double v, double d) => BitConverter.DoubleToInt64Bits(v) == BitConverter.DoubleToInt64Bits(d);
}

internal sealed class SingleElem : SvElem<float>
{
    public override void Write(SvWriter w, float v) => w.WriteDouble(v);          // exact widening
    public override float Read(ref Utf8JsonReader r, SvReader rd) => (float)SvReader.ReadDouble(ref r);
    public override bool IsDefault(float v, float d) => BitConverter.SingleToInt32Bits(v) == BitConverter.SingleToInt32Bits(d);
}

internal sealed class Int32Elem : SvElem<int>
{
    public override void Write(SvWriter w, int v) => w.J.WriteNumberValue(v);
    public override int Read(ref Utf8JsonReader r, SvReader rd)
    {
        if (r.TokenType != JsonTokenType.Number) throw rd.Bad(ref r, "integer expected");
        return r.GetInt32();
    }
    public override bool IsDefault(int v, int d) => v == d;
}

internal sealed class Int64Elem : SvElem<long>
{
    public override void Write(SvWriter w, long v) => w.J.WriteNumberValue(v);
    public override long Read(ref Utf8JsonReader r, SvReader rd)
    {
        if (r.TokenType != JsonTokenType.Number) throw rd.Bad(ref r, "integer expected");
        return r.GetInt64();
    }
    public override bool IsDefault(long v, long d) => v == d;
}

internal sealed class UInt64Elem : SvElem<ulong>
{
    public override void Write(SvWriter w, ulong v) => w.J.WriteNumberValue(v);
    public override ulong Read(ref Utf8JsonReader r, SvReader rd)
    {
        if (r.TokenType != JsonTokenType.Number) throw rd.Bad(ref r, "integer expected");
        return r.GetUInt64();
    }
    public override bool IsDefault(ulong v, ulong d) => v == d;
}

/// <summary>sbyte, short, byte, ushort, uint, char (as their integer value).</summary>
internal sealed class SmallIntElem<T> : SvElem<T> where T : struct
{
    public override void Write(SvWriter w, T v) => w.J.WriteNumberValue(Convert.ToInt64(v, CultureInfo.InvariantCulture));
    public override T Read(ref Utf8JsonReader r, SvReader rd)
    {
        if (r.TokenType != JsonTokenType.Number) throw rd.Bad(ref r, "integer expected");
        return (T)Convert.ChangeType(r.GetInt64(), typeof(T), CultureInfo.InvariantCulture);
    }
    public override bool IsDefault(T v, T d) => EqualityComparer<T>.Default.Equals(v, d);
}

internal sealed class EnumElem<T> : SvElem<T> where T : struct, Enum
{
    private static readonly bool Unsigned = Type.GetTypeCode(Enum.GetUnderlyingType(typeof(T))) is TypeCode.Byte or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64;
    public override void Write(SvWriter w, T v)
    {
        if (Unsigned) w.J.WriteNumberValue(Convert.ToUInt64(v, CultureInfo.InvariantCulture));
        else w.J.WriteNumberValue(Convert.ToInt64(v, CultureInfo.InvariantCulture));
    }
    public override T Read(ref Utf8JsonReader r, SvReader rd)
    {
        if (r.TokenType != JsonTokenType.Number) throw rd.Bad(ref r, "integer expected");
        return Unsigned ? (T)Enum.ToObject(typeof(T), r.GetUInt64()) : (T)Enum.ToObject(typeof(T), r.GetInt64());
    }
    public override bool IsDefault(T v, T d) => EqualityComparer<T>.Default.Equals(v, d);
}

internal sealed class BoolElem : SvElem<bool>
{
    public override void Write(SvWriter w, bool v) => w.J.WriteBooleanValue(v);
    public override bool Read(ref Utf8JsonReader r, SvReader rd) => r.TokenType switch
    {
        JsonTokenType.True => true,
        JsonTokenType.False => false,
        _ => throw rd.Bad(ref r, "boolean expected"),
    };
    public override bool IsDefault(bool v, bool d) => v == d;
}

internal sealed class StringElem : SvElem<string>
{
    public override void Write(SvWriter w, string v) => w.WriteString(v);
    public override string Read(ref Utf8JsonReader r, SvReader rd) => rd.ReadString(ref r);
    public override bool IsDefault(string v, string d) => string.Equals(v, d, StringComparison.Ordinal);
}

internal sealed class NullableElem<U> : SvElem<U?> where U : struct
{
    private SvElem<U> _e = SvTypes.ElemLocked<U>();
    public override void Write(SvWriter w, U? v)
    {
        if (v.HasValue) _e.Write(w, v.GetValueOrDefault()); else w.J.WriteNullValue();
    }
    public override U? Read(ref Utf8JsonReader r, SvReader rd) => r.TokenType == JsonTokenType.Null ? null : _e.Read(ref r, rd);
    public override bool IsDefault(U? v, U? d) => v.HasValue ? d.HasValue && _e.IsDefault(v.GetValueOrDefault(), d.GetValueOrDefault()) : !d.HasValue;
}

/// <summary>Tracked reference: null, {"$ref"}, or the codec's body.</summary>
internal sealed class RefElem<T> : SvElem<T> where T : class
{
    private readonly SvCodec _c;
    public RefElem(SvCodec c) { _c = c; }
    public override bool HasRefs => true;
    public override void Scan(SvWriter w, T v) => w.ScanRef(v, _c);
    public override void Write(SvWriter w, T v) => w.WriteRef(v, _c);
    public override T Read(ref Utf8JsonReader r, SvReader rd) => (T)_c.Read(ref r, rd);
    public override bool IsDefault(T v, T d) => v == null && d == null;
}

// ---------------------------------------------------------------- containers

internal sealed class ListCodec<T> : SvCodec
{
    private SvElem<T> _e;
    public ListCodec() : base(typeof(List<T>)) { }
    public override void Init() => _e = SvTypes.ElemLocked<T>();

    public override void ScanBody(SvWriter w, object o)
    {
        if (!_e.HasRefs) return;
        var l = (List<T>)o;
        for (int i = 0; i < l.Count; i++) _e.Scan(w, l[i]);
    }

    public override void WriteBody(SvWriter w, object o, int id)
    {
        var l = (List<T>)o;
        var j = w.J;
        if (id > 0) { j.WriteStartObject(); j.WriteNumber(SvNames.Id, id); j.WritePropertyName(SvNames.Values); }
        j.WriteStartArray();
        for (int i = 0; i < l.Count; i++) _e.Write(w, l[i]);
        j.WriteEndArray();
        if (id > 0) j.WriteEndObject();
    }

    public override object Read(ref Utf8JsonReader r, SvReader rd)
    {
        int slot = -1;
        if (r.TokenType == JsonTokenType.Null) return null;
        if (r.TokenType == JsonTokenType.StartObject)
        {
            var x = rd.ReadMeta(ref r, out slot);
            if (x != null) return x is List<T> ? x : throw rd.Bad(ref r, $"$ref to a {x.GetType()} where a {Type} is expected");
            if (slot < 0) throw rd.Bad(ref r, "list in object form without $id");
            rd.Values(ref r);
        }
        else if (r.TokenType != JsonTokenType.StartArray) throw rd.Bad(ref r, $"array expected for {Type}");
        var l = new List<T>();
        rd.Fill(slot, l);
        while (true)
        {
            SvReader.Next(ref r);
            if (r.TokenType == JsonTokenType.EndArray) break;
            l.Add(_e.Read(ref r, rd));
        }
        rd.EndWrapped(ref r, slot);
        return l;
    }
}

internal sealed class ArrayCodec<T> : SvCodec
{
    private SvElem<T> _e;
    public ArrayCodec() : base(typeof(T[])) { }
    public override void Init() => _e = SvTypes.ElemLocked<T>();

    public override void ScanBody(SvWriter w, object o)
    {
        if (!_e.HasRefs) return;
        foreach (var x in (T[])o) _e.Scan(w, x);
    }

    public override void WriteBody(SvWriter w, object o, int id)
    {
        var a = (T[])o;
        var j = w.J;
        if (id > 0) { j.WriteStartObject(); j.WriteNumber(SvNames.Id, id); j.WritePropertyName(SvNames.Values); }
        j.WriteStartArray();
        for (int i = 0; i < a.Length; i++) _e.Write(w, a[i]);
        j.WriteEndArray();
        if (id > 0) j.WriteEndObject();
    }

    public override object Read(ref Utf8JsonReader r, SvReader rd)
    {
        int slot = -1;
        if (r.TokenType == JsonTokenType.Null) return null;
        if (r.TokenType == JsonTokenType.StartObject)
        {
            var x = rd.ReadMeta(ref r, out slot);
            if (x != null) return x is T[] ? x : throw rd.Bad(ref r, $"$ref to a {x.GetType()} where a {Type} is expected");
            if (slot < 0) throw rd.Bad(ref r, "array in object form without $id");
            rd.Values(ref r);
        }
        else if (r.TokenType != JsonTokenType.StartArray) throw rd.Bad(ref r, $"array expected for {Type}");
        var l = new List<T>();
        while (true)
        {
            SvReader.Next(ref r);
            if (r.TokenType == JsonTokenType.EndArray) break;
            l.Add(_e.Read(ref r, rd));
        }
        var a = l.ToArray();
        rd.Fill(slot, a);              // after the elements: an array cannot contain a reference to itself
        rd.EndWrapped(ref r, slot);
        return a;
    }
}

/// <summary>byte[] as base64 ({"$id": n, "$b64": "..."} when shared).</summary>
internal sealed class BytesCodec : SvCodec
{
    public BytesCodec() : base(typeof(byte[])) { }
    public override void ScanBody(SvWriter w, object o) { }

    public override void WriteBody(SvWriter w, object o, int id)
    {
        var j = w.J;
        if (id > 0) { j.WriteStartObject(); j.WriteNumber(SvNames.Id, id); j.WriteBase64String(SvNames.B64, (byte[])o); j.WriteEndObject(); }
        else j.WriteBase64StringValue((byte[])o);
    }

    public override object Read(ref Utf8JsonReader r, SvReader rd)
    {
        if (r.TokenType == JsonTokenType.Null) return null;
        if (r.TokenType == JsonTokenType.String) return r.GetBytesFromBase64();
        if (r.TokenType != JsonTokenType.StartObject) throw rd.Bad(ref r, "base64 string expected");
        var x = rd.ReadMeta(ref r, out int slot);
        if (x != null) return x is byte[] ? x : throw rd.Bad(ref r, $"$ref to a {x.GetType()} where a byte[] is expected");
        if (slot < 0 || r.TokenType != JsonTokenType.PropertyName || !r.ValueTextEquals("$b64"u8)) throw rd.Bad(ref r, "\"$b64\" expected");
        SvReader.Next(ref r);
        var b = r.GetBytesFromBase64();
        rd.Fill(slot, b);
        SvReader.Next(ref r);
        rd.Expect(ref r, JsonTokenType.EndObject);
        return b;
    }
}

/// <summary>Base of the record-like containers written as JSON objects with string keys.</summary>
internal abstract class KeyedCodec<C, V> : SvCodec where C : class
{
    protected SvElem<V> E;
    protected KeyedCodec() : base(typeof(C)) { }
    public override void Init() => E = SvTypes.ElemLocked<V>();
    protected abstract C New();
    protected abstract void Add(C c, string key, V v, ref Utf8JsonReader r, SvReader rd);

    protected void Start(SvWriter w, int id)
    {
        w.J.WriteStartObject();
        if (id > 0) w.J.WriteNumber(SvNames.Id, id);
    }

    protected void Entry(SvWriter w, string key, V v)
    {
        w.J.WritePropertyName(SvNames.Esc(key));
        E.Write(w, v);
    }

    public override object Read(ref Utf8JsonReader r, SvReader rd)
    {
        if (r.TokenType == JsonTokenType.Null) return null;
        if (r.TokenType != JsonTokenType.StartObject) throw rd.Bad(ref r, $"object expected for {Type}");
        var x = rd.ReadMeta(ref r, out int slot);
        if (x != null) return x is C ? x : throw rd.Bad(ref r, $"$ref to a {x.GetType()} where a {Type} is expected");
        var c = New();
        rd.Fill(slot, c);
        while (r.TokenType != JsonTokenType.EndObject)
        {
            string k = r.GetString();
            if (k.Length > 0 && k[0] == '$')
            {
                if (k.Length < 2 || k[1] != '$') throw rd.Bad(ref r, $"unexpected meta property \"{k}\"");
                k = k.Substring(1);
            }
            SvReader.Next(ref r);
            Add(c, k, E.Read(ref r, rd), ref r, rd);
            SvReader.Next(ref r);
        }
        return c;
    }
}

internal sealed class JsObjCodec<V> : KeyedCodec<JsObj<V>, V>
{
    protected override JsObj<V> New() => new();
    protected override void Add(JsObj<V> c, string key, V v, ref Utf8JsonReader r, SvReader rd)
    {
        if (c.Has(key)) throw rd.Bad(ref r, $"duplicate key \"{key}\"");
        c.Set(key, v);
    }

    public override void ScanBody(SvWriter w, object o)
    {
        if (!E.HasRefs) return;
        foreach (var kv in ((JsObj<V>)o).Entries()) E.Scan(w, kv.Value);
    }

    public override void WriteBody(SvWriter w, object o, int id)
    {
        Start(w, id);
        foreach (var kv in ((JsObj<V>)o).Entries()) Entry(w, kv.Key, kv.Value);     // JS key order
        w.J.WriteEndObject();
    }
}

internal sealed class JsNumObjCodec<V> : KeyedCodec<JsNumObj<V>, V>
{
    protected override JsNumObj<V> New() => new();
    protected override void Add(JsNumObj<V> c, string key, V v, ref Utf8JsonReader r, SvReader rd)
    {
        if (c.O.Has(key)) throw rd.Bad(ref r, $"duplicate key \"{key}\"");
        c.O.Set(key, v);
    }

    public override void ScanBody(SvWriter w, object o)
    {
        if (!E.HasRefs) return;
        foreach (var kv in ((JsNumObj<V>)o).O.Entries()) E.Scan(w, kv.Value);
    }

    public override void WriteBody(SvWriter w, object o, int id)
    {
        Start(w, id);
        foreach (var kv in ((JsNumObj<V>)o).O.Entries()) Entry(w, kv.Key, kv.Value);
        w.J.WriteEndObject();
    }
}

internal sealed class StringDictCodec<V> : KeyedCodec<Dictionary<string, V>, V>
{
    protected override Dictionary<string, V> New() => new();
    protected override void Add(Dictionary<string, V> c, string key, V v, ref Utf8JsonReader r, SvReader rd)
    {
        if (!c.TryAdd(key, v)) throw rd.Bad(ref r, $"duplicate key \"{key}\"");
    }

    public override void ScanBody(SvWriter w, object o)
    {
        if (!E.HasRefs) return;
        foreach (var kv in (Dictionary<string, V>)o) E.Scan(w, kv.Value);
    }

    public override void WriteBody(SvWriter w, object o, int id)
    {
        Start(w, id);
        foreach (var kv in (Dictionary<string, V>)o) Entry(w, kv.Key, kv.Value);    // enumeration order
        w.J.WriteEndObject();
    }
}

internal sealed class IntDictCodec<V> : KeyedCodec<Dictionary<int, V>, V>
{
    protected override Dictionary<int, V> New() => new();
    protected override void Add(Dictionary<int, V> c, string key, V v, ref Utf8JsonReader r, SvReader rd)
    {
        if (!int.TryParse(key, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out int k)) throw rd.Bad(ref r, $"bad integer key \"{key}\"");
        if (!c.TryAdd(k, v)) throw rd.Bad(ref r, $"duplicate key {k}");
    }

    public override void ScanBody(SvWriter w, object o)
    {
        if (!E.HasRefs) return;
        foreach (var kv in (Dictionary<int, V>)o) E.Scan(w, kv.Value);
    }

    public override void WriteBody(SvWriter w, object o, int id)
    {
        Start(w, id);
        foreach (var kv in (Dictionary<int, V>)o) Entry(w, kv.Key.ToString(CultureInfo.InvariantCulture), kv.Value);
        w.J.WriteEndObject();
    }
}

/// <summary>Base of containers written as JSON arrays (of elements or of [key, value] pairs).</summary>
internal abstract class SeqCodec<C> : SvCodec where C : class
{
    protected SeqCodec() : base(typeof(C)) { }
    protected abstract C New();
    protected abstract void WriteItems(SvWriter w, C c);
    protected abstract void ReadItem(C c, ref Utf8JsonReader r, SvReader rd);

    public override void WriteBody(SvWriter w, object o, int id)
    {
        var j = w.J;
        if (id > 0) { j.WriteStartObject(); j.WriteNumber(SvNames.Id, id); j.WritePropertyName(SvNames.Values); }
        j.WriteStartArray();
        WriteItems(w, (C)o);
        j.WriteEndArray();
        if (id > 0) j.WriteEndObject();
    }

    public override object Read(ref Utf8JsonReader r, SvReader rd)
    {
        int slot = -1;
        if (r.TokenType == JsonTokenType.Null) return null;
        if (r.TokenType == JsonTokenType.StartObject)
        {
            var x = rd.ReadMeta(ref r, out slot);
            if (x != null) return x is C ? x : throw rd.Bad(ref r, $"$ref to a {x.GetType()} where a {Type} is expected");
            if (slot < 0) throw rd.Bad(ref r, "collection in object form without $id");
            rd.Values(ref r);
        }
        else if (r.TokenType != JsonTokenType.StartArray) throw rd.Bad(ref r, $"array expected for {Type}");
        var c = New();
        rd.Fill(slot, c);
        while (true)
        {
            SvReader.Next(ref r);
            if (r.TokenType == JsonTokenType.EndArray) break;
            ReadItem(c, ref r, rd);
        }
        rd.EndWrapped(ref r, slot);
        return c;
    }
}

internal sealed class HashSetCodec<T> : SeqCodec<HashSet<T>>
{
    private SvElem<T> _e;
    public override void Init() => _e = SvTypes.ElemLocked<T>();
    protected override HashSet<T> New() => new();

    public override void ScanBody(SvWriter w, object o)
    {
        if (!_e.HasRefs) return;
        foreach (var x in (HashSet<T>)o) _e.Scan(w, x);
    }

    protected override void WriteItems(SvWriter w, HashSet<T> c) { foreach (var x in c) _e.Write(w, x); }

    protected override void ReadItem(HashSet<T> c, ref Utf8JsonReader r, SvReader rd)
    {
        if (!c.Add(_e.Read(ref r, rd))) throw rd.Bad(ref r, "duplicate set element");
    }
}

/// <summary>[[key, value], ...] for maps whose keys are not strings/ints.</summary>
internal abstract class PairCodec<C, K, V> : SeqCodec<C> where C : class
{
    protected SvElem<K> EK;
    protected SvElem<V> EV;
    public override void Init() { EK = SvTypes.ElemLocked<K>(); EV = SvTypes.ElemLocked<V>(); }
    protected abstract IEnumerable<KeyValuePair<K, V>> Pairs(C c);
    protected abstract void Add(C c, K k, V v, ref Utf8JsonReader r, SvReader rd);

    public override void ScanBody(SvWriter w, object o)
    {
        if (!EK.HasRefs && !EV.HasRefs) return;
        foreach (var kv in Pairs((C)o)) { EK.Scan(w, kv.Key); EV.Scan(w, kv.Value); }
    }

    protected override void WriteItems(SvWriter w, C c)
    {
        foreach (var kv in Pairs(c))
        {
            w.J.WriteStartArray();
            EK.Write(w, kv.Key);
            EV.Write(w, kv.Value);
            w.J.WriteEndArray();
        }
    }

    protected override void ReadItem(C c, ref Utf8JsonReader r, SvReader rd)
    {
        rd.Expect(ref r, JsonTokenType.StartArray);
        SvReader.Next(ref r);
        K k = EK.Read(ref r, rd);
        SvReader.Next(ref r);
        V v = EV.Read(ref r, rd);
        SvReader.Next(ref r);
        rd.Expect(ref r, JsonTokenType.EndArray);
        Add(c, k, v, ref r, rd);
    }
}

internal sealed class PairDictCodec<K, V> : PairCodec<Dictionary<K, V>, K, V>
{
    protected override Dictionary<K, V> New() => new();
    protected override IEnumerable<KeyValuePair<K, V>> Pairs(Dictionary<K, V> c) => c;
    protected override void Add(Dictionary<K, V> c, K k, V v, ref Utf8JsonReader r, SvReader rd)
    {
        if (!c.TryAdd(k, v)) throw rd.Bad(ref r, "duplicate key");
    }
}

internal sealed class JsMapCodec<K, V> : PairCodec<JsMap<K, V>, K, V>
{
    protected override JsMap<K, V> New() => new();
    protected override IEnumerable<KeyValuePair<K, V>> Pairs(JsMap<K, V> c) => c;     // snapshot, insertion order
    protected override void Add(JsMap<K, V> c, K k, V v, ref Utf8JsonReader r, SvReader rd)
    {
        if (c.Has(k)) throw rd.Bad(ref r, "duplicate key");
        c.Set(k, v);
    }
}

// ---------------------------------------------------------------- classes

/// <summary>
/// Member accessors and constructors as tiny DynamicMethods (cheap to create: a save touches ~40 types and ~800
/// members, and Expression.Compile made the first save/load of a process take over a second). Falls back to
/// reflection where dynamic code is not supported (AOT).
/// </summary>
internal static class SvAccess
{
    private static readonly bool Emit = RuntimeFeature.IsDynamicCodeSupported;

    public static Func<object> Factory(Type t)
    {
        var ctor = t.GetConstructor(Type.EmptyTypes);
        if (ctor == null) return () => RuntimeHelpers.GetUninitializedObject(t);
        if (!Emit) return () => ctor.Invoke(null);
        var dm = new DynamicMethod("new_" + t.Name, typeof(object), Type.EmptyTypes, typeof(SvAccess).Module, skipVisibility: true);
        var il = dm.GetILGenerator();
        il.Emit(OpCodes.Newobj, ctor);
        il.Emit(OpCodes.Ret);
        return dm.CreateDelegate<Func<object>>();
    }

    public static Func<object, T> Getter<T>(Type owner, FieldInfo f, PropertyInfo p)
    {
        if (!Emit) return f != null ? o => (T)f.GetValue(o) : o => (T)p.GetValue(o);
        var dm = new DynamicMethod("get_" + (f?.Name ?? p.Name), typeof(T), new[] { typeof(object) }, typeof(SvAccess).Module, skipVisibility: true);
        var il = dm.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Castclass, owner);
        if (f != null) il.Emit(OpCodes.Ldfld, f); else il.Emit(OpCodes.Callvirt, p.GetMethod);
        il.Emit(OpCodes.Ret);
        return dm.CreateDelegate<Func<object, T>>();
    }

    /// <summary>Also sets readonly fields (the object is fresh from its constructor).</summary>
    public static Action<object, T> Setter<T>(Type owner, FieldInfo f, PropertyInfo p)
    {
        if (!Emit) return f != null ? (o, v) => f.SetValue(o, v) : (o, v) => p.SetValue(o, v);
        var dm = new DynamicMethod("set_" + (f?.Name ?? p.Name), null, new[] { typeof(object), typeof(T) }, typeof(SvAccess).Module, skipVisibility: true);
        var il = dm.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Castclass, owner);
        il.Emit(OpCodes.Ldarg_1);
        if (f != null) il.Emit(OpCodes.Stfld, f); else il.Emit(OpCodes.Callvirt, p.SetMethod);
        il.Emit(OpCodes.Ret);
        return dm.CreateDelegate<Action<object, T>>();
    }
}

/// <summary>A class: public instance fields + public get/set properties, by C# name.</summary>
internal sealed class ClassCodec : SvCodec
{
    private Func<object> _new;
    private Member[] _members;

    public ClassCodec(Type t) : base(t) { }

    public override void Init()
    {
        var t = Type;
        _new = SvAccess.Factory(t);
        var list = new List<Member>();
        var props = new HashSet<string>();
        foreach (var p in t.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (p.GetIndexParameters().Length > 0 || p.IsDefined(typeof(NoSaveAttribute), true)) continue;
            if (p.GetMethod is not { IsPublic: true } || p.SetMethod is not { IsPublic: true }) continue;   // computed (get-only) properties are not state
            list.Add(Member.Make(t, p.Name, p.PropertyType, p, null));
            props.Add(p.Name);
        }
        foreach (var f in t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (f.IsDefined(typeof(NoSaveAttribute), true)) continue;
            if (!f.IsPublic)
            {
                // auto-property backing field: saved through its public property
                if (f.Name.StartsWith("<", StringComparison.Ordinal) && f.Name.EndsWith(">k__BackingField", StringComparison.Ordinal)
                    && props.Contains(f.Name.Substring(1, f.Name.IndexOf('>') - 1))) continue;
                throw new NotSupportedException($"save: {t.Name}.{f.Name} is a non-public instance field (state the save cannot see). Make it public, or mark it [NoSave] if it is transient.");
            }
            list.Add(Member.Make(t, f.Name, f.FieldType, null, f));
        }
        _members = list.ToArray();
        // defaults for omission: two fresh instances must agree, or the member is always written
        object p1 = _new(), p2 = _new();
        foreach (var m in _members) m.InitDefault(p1, p2);
    }

    public override void ScanBody(SvWriter w, object o)
    {
        foreach (var m in _members) if (m.HasRefs) m.Scan(w, o);
    }

    public override void WriteBody(SvWriter w, object o, int id)
    {
        w.J.WriteStartObject();
        if (id > 0) w.J.WriteNumber(SvNames.Id, id);
        foreach (var m in _members) m.Write(w, o);
        w.J.WriteEndObject();
    }

    public override object Read(ref Utf8JsonReader r, SvReader rd)
    {
        if (r.TokenType == JsonTokenType.Null) return null;
        if (r.TokenType != JsonTokenType.StartObject) throw rd.Bad(ref r, $"object expected for {Type.Name}");
        var x = rd.ReadMeta(ref r, out int slot);
        if (x != null) return x.GetType() == Type ? x : throw rd.Bad(ref r, $"$ref to a {x.GetType()} where a {Type} is expected");
        var o = _new();
        rd.Fill(slot, o);
        int hint = 0;
        while (r.TokenType != JsonTokenType.EndObject)
        {
            // members come in declaration order: try the expected one first
            Member m = null;
            for (int k = 0; k < _members.Length; k++)
            {
                var c = _members[(hint + k) % _members.Length];
                if (r.ValueTextEquals(c.Utf8Name)) { m = c; hint = (hint + k + 1) % _members.Length; break; }
            }
            if (m == null) throw rd.Bad(ref r, $"unknown member {Type.Name}.{r.GetString()}");
            SvReader.Next(ref r);
            m.Read(ref r, rd, o);
            SvReader.Next(ref r);
        }
        return o;
    }

    private abstract class Member
    {
        public string Name;
        public byte[] Utf8Name;
        public JsonEncodedText Encoded;
        public bool HasRefs;
        public abstract void InitDefault(object p1, object p2);
        public abstract void Scan(SvWriter w, object o);
        public abstract void Write(SvWriter w, object o);
        public abstract void Read(ref Utf8JsonReader r, SvReader rd, object o);

        public static Member Make(Type owner, string name, Type mt, PropertyInfo p, FieldInfo f)
        {
            Member m;
            try { m = (Member)Activator.CreateInstance(typeof(Member<>).MakeGenericType(mt), owner, p, f); }
            catch (TargetInvocationException e) when (e.InnerException != null)
            {
                if (e.InnerException is NotSupportedException ns) throw new NotSupportedException($"{ns.Message} [member {owner.Name}.{name}]", ns);
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                throw;
            }
            m.Name = name;
            m.Utf8Name = Encoding.UTF8.GetBytes(name);
            m.Encoded = JsonEncodedText.Encode(name);
            return m;
        }
    }

    private sealed class Member<T> : Member
    {
        private readonly Func<object, T> _get;
        private readonly Action<object, T> _set;
        private readonly SvElem<T> _e;
        private T _default;
        private bool _omit;

        public Member(Type owner, PropertyInfo p, FieldInfo f)
        {
            _e = SvTypes.ElemLocked<T>();
            HasRefs = _e.HasRefs;
            _get = SvAccess.Getter<T>(owner, f, p);
            _set = SvAccess.Setter<T>(owner, f, p);
        }

        public override void InitDefault(object p1, object p2)
        {
            _default = _get(p1);
            _omit = _e.IsDefault(_default, _get(p2));
        }

        public override void Scan(SvWriter w, object o) => _e.Scan(w, _get(o));

        public override void Write(SvWriter w, object o)
        {
            T v = _get(o);
            if (_omit && _e.IsDefault(v, _default)) return;
            w.J.WritePropertyName(Encoded);
            _e.Write(w, v);
        }

        public override void Read(ref Utf8JsonReader r, SvReader rd, object o) => _set(o, _e.Read(ref r, rd));
    }
}
