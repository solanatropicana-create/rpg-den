using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using FD.Macro;

namespace SaveCheck;

// ---- test-only shapes (public: the codec only sees public members)
public sealed class TNode
{
    public int Id;
    public TNode Next;
    public List<TNode> Kids = new();
    public JsObj<double> Vals = new();
}

public enum TEnum { A, B = 5, C = -3 }
public enum TBig : ulong { Zero = 0, Max = ulong.MaxValue }

public sealed class TAll
{
    public double Dbl = 1.5;
    public double NegZero = -0.0;
    public float F;
    public long L;
    public byte By;
    public sbyte SB;
    public short Sh;
    public ushort US;
    public uint U;
    public ulong UL;
    public char Ch;
    public TEnum E = TEnum.B;
    public TBig Big;
    public TEnum? NE;
    public int? NI;
    public double? ND;
    public bool? NB;
    public bool Flag = true;
    public string S = "def";
    public string SN;
    public int[] Arr;
    public byte[] Bytes;
    public HashSet<string> Set;
    public Dictionary<string, TNode> SD;
    public Dictionary<int, double> ID;
    public Dictionary<long, string> LD;
    public JsMap<int, TNode> Map;
    public JsNumObj<List<int>> NO;
    public List<List<double>> LL;
    public List<int?> LNI;
    public TNode[] Nodes;
    public int Prop { get; set; } = 3;
    public int Computed => Prop * 2;           // get-only: not state
}

public sealed class TWithObject { public object X; }
public sealed class TWithPrivate { private int _x = 1; public int Y; public int X => _x; }
public sealed class TWithDelegate { public Action A; }
public sealed class TWithNoSave { public int A; [NoSave] public Action Cb; [NoSave] public int Transient = 5; }
public sealed class TWithStruct { public (int, double) Pair; }
public class TBase { public int B; }
public sealed class TDerived : TBase { public int C; }
public sealed class TPoly { public TBase X; }
public sealed class TWithInit { public List<int> L = new() { 1 }; public string S = "x"; public int I = 7; public double D = double.NaN; }

internal static class SelfTest
{
    private static int _n, _failed;

    private static void Check(bool ok, string what)
    {
        _n++;
        if (ok) return;
        _failed++;
        Console.WriteLine("  FAIL " + what);
    }

    private static void Throws<E>(Action a, string what) where E : Exception
    {
        try { a(); Check(false, what + " (no exception)"); }
        catch (E) { Check(true, what); }
        catch (Exception x) { Check(false, $"{what} (got {x.GetType().Name}: {x.Message})"); }
    }

    private static T RT<T>(T v, bool preserve = true) => SaveCodec.FromJson<T>(SaveCodec.ToJson(v, preserve));

    private static bool Bits(double a, double b) => BitConverter.DoubleToInt64Bits(a) == BitConverter.DoubleToInt64Bits(b);

    public static int Run()
    {
        Console.WriteLine("selftest:");
        Doubles();
        Records();
        Shapes();
        Sharing();
        Strictness();
        SimRoundTrips();
        FingerprintSensitivity();
        Audits();
        Console.WriteLine($"selftest: {_n - _failed}/{_n} checks passed{(_failed > 0 ? $", {_failed} FAILED" : "")}");
        return _failed == 0 ? 0 : 1;
    }

    // ---------------------------------------------------------------- numbers
    private static void Doubles()
    {
        var special = new List<double>
        {
            0.0, -0.0, 1, -1, 0.1, 0.2, 0.1 + 0.2, 1.0 / 3, Math.PI, -Math.E, 1e21, 1e-7, 123456789012345680000.0,
            double.Epsilon, -double.Epsilon, double.MaxValue, double.MinValue, 2.2250738585072014e-308, 2.2250738585072009e-308,
            double.NaN, double.PositiveInfinity, double.NegativeInfinity, BitConverter.Int64BitsToDouble(0x7FF8000000000001), BitConverter.Int64BitsToDouble(0x7FF0000000000001),
            9007199254740992.0, 9007199254740994.0, 9007199254740996.0 * 1024, 18014398509481988.0, 1.8446744073709552e19 + 4096, 4294967296.0 * 4294967296.0 * 3,
        };
        var rnd = new Random(12345);
        var bytes = new byte[8];
        for (int i = 0; i < 300000; i++) { rnd.NextBytes(bytes); special.Add(BitConverter.ToDouble(bytes, 0)); }
        for (int i = 0; i < 100000; i++) special.Add(Math.Round(rnd.NextDouble() * 1000, rnd.Next(0, 6)) * (rnd.Next(2) == 0 ? 1 : -1));
        for (long s = 1; s < 1L << 62; s = s * 3 + 1) special.Add(s);           // big integers (RNG-like states)
        var back = RT(special);
        int bad = 0;
        for (int i = 0; i < special.Count; i++) if (!Bits(special[i], back[i])) bad++;
        Check(back.Count == special.Count && bad == 0, $"{special.Count} doubles round-trip bit-exactly ({bad} differ)");
        string j = Encoding.UTF8.GetString(SaveCodec.ToJson(new List<double> { -0.0, double.NaN, double.PositiveInfinity, double.NegativeInfinity, 0.1, 1e21, BitConverter.Int64BitsToDouble(0x7FF8000000000001) }));
        Check(j == "[\"-0\",\"NaN\",\"Infinity\",\"-Infinity\",0.1,1E+21,\"NaN:7ff8000000000001\"]", "special doubles are spelled out: " + j);
        var floats = new List<float> { 0f, -0f, 1.1f, float.NaN, float.MaxValue, float.Epsilon, -3.4e38f, float.NegativeInfinity };
        var fb = RT(floats);
        Check(fb.Zip(floats, (a, b) => BitConverter.SingleToInt32Bits(a) == BitConverter.SingleToInt32Bits(b)).All(x => x), "floats round-trip bit-exactly");
        var longs = new List<long> { long.MinValue, long.MaxValue, 0, -1, 1L << 53, (1L << 53) + 1 };
        Check(RT(longs).SequenceEqual(longs), "longs round-trip");
    }

    // ---------------------------------------------------------------- JsObj / JsNumObj order
    private static void Records()
    {
        var o = new JsObj<double>();
        foreach (var k in new[] { "b", "a", "10", "2", "$id", "$$x", "-1", "01", "4294967295", "4294967294", "", "$ref", "$values" }) o.Set(k, o.Count);
        o.Delete("b"); o.Set("b", 99);           // deleted then re-added: goes last
        o.Delete("a");
        var c = RT(o);
        Check(c.Keys().SequenceEqual(o.Keys()), "JsObj key order round-trips: " + string.Join(",", c.Keys()));
        Check(string.Join(",", o.Keys()) == "2,10,4294967294,$id,$$x,-1,01,4294967295,,$ref,$values,b", "JsObj order is the JS order: " + string.Join(",", o.Keys()));
        Check(o.Keys().All(k => Bits(o.Get(k).Value, c.Get(k).Value)), "JsObj values round-trip");
        foreach (var x in new[] { o, c }) { x.Set("a", 1); x.Delete("10"); x.Set("10", 2); x.Set("z", 3); x.Delete("$id"); x.Set("$id", 4); x.Set("7", 5); x.Delete("01"); x.Set("01", 6); }
        Check(c.Keys().SequenceEqual(o.Keys()), "JsObj behaves identically after further set/delete: " + string.Join(",", c.Keys()));
        var n = new JsNumObj<double>();
        foreach (int k in new[] { 5, -2, 0, 3, -7, 100 }) n.Set(k, k * 1.5);
        n.Delete(3); n.Set(3, 9);
        var nc = RT(n);
        Check(nc.Keys().SequenceEqual(n.Keys()) && string.Join(",", n.Keys()) == "0,3,5,100,-2,-7", "JsNumObj order round-trips: " + string.Join(",", nc.Keys()));
        var nested = new JsObj<JsObj<List<string>>>();
        nested.Set("x", new JsObj<List<string>> { ["q"] = new List<string> { "a", null, "" } });
        nested.Set("y", null);
        var nb = RT(nested);
        Check(nb.Keys().SequenceEqual(new[] { "x", "y" }) && nb["y"] == null && nb["x"]["q"].SequenceEqual(new[] { "a", null, "" }), "nested JsObj with null values");
        var map = new JsMap<string, int>();
        map.Set("k", 1); map.Set("j", 2); map.Delete("k"); map.Set("k", 3);
        var mb = RT(map);
        Check(mb.Keys().SequenceEqual(new[] { "j", "k" }) && mb.Get("k") == 3, "JsMap order round-trips");
    }

    // ---------------------------------------------------------------- shapes
    private static void Shapes()
    {
        var shared = new TNode { Id = 1 };
        var lst = new List<int> { 4, 5 };
        var a = new TAll
        {
            Dbl = 2.5, NegZero = 0.0, F = 1.25f, L = -5_000_000_000_000, By = 200, SB = -100, Sh = -30000, US = 60000, U = 4_000_000_000, UL = ulong.MaxValue, Ch = 'ğ',
            E = TEnum.C, Big = TBig.Max, NE = TEnum.A, NI = 0, ND = -0.0, NB = false, Flag = false, S = null, SN = "set",
            Arr = new[] { 3, 1, 2 }, Bytes = new byte[] { 0, 255, 7 }, Set = new HashSet<string> { "z", "a", "m" },
            SD = new Dictionary<string, TNode> { ["$id"] = shared, ["k"] = shared, ["n"] = null },
            ID = new Dictionary<int, double> { [-4] = 1.5, [9] = double.NaN }, LD = new Dictionary<long, string> { [1L << 40] = "big", [-1] = null },
            Map = new JsMap<int, TNode>(), NO = new JsNumObj<List<int>>(), LL = new List<List<double>> { new() { 1, 2 }, null, new() },
            LNI = new List<int?> { 1, null, 3 }, Nodes = new[] { shared, null, new TNode { Id = 2 } }, Prop = 11,
        };
        a.Map.Set(7, shared);
        a.NO.Set(2, lst); a.NO.Set(1, lst);
        var b = RT(a);
        var g = GraphCompare.Run(a, b, "a");
        Check(g.Ok, "all supported member shapes round-trip (values + sharing): " + string.Join(" | ", g.Errors));
        Check(ReferenceEquals(b.SD["$id"], b.SD["k"]) && ReferenceEquals(b.SD["k"], b.Map.Get(7)) && ReferenceEquals(b.Map.Get(7), b.Nodes[0]), "one object shared by Dictionary, JsMap and array");
        Check(ReferenceEquals(b.NO[1], b.NO[2]), "list shared inside a JsNumObj");
        Check(b.Prop == 11 && b.S == null && b.SN == "set" && Bits(b.NegZero, 0.0) && Bits(b.ND.Value, -0.0), "defaults vs explicit values (null over a non-null initializer, 0 over -0)");
        Check(b.Set.SequenceEqual(a.Set) && b.SD.Keys.SequenceEqual(a.SD.Keys), "HashSet / Dictionary enumeration order kept");

        // cycles
        var n1 = new TNode { Id = 1 }; var n2 = new TNode { Id = 2, Next = n1 }; n1.Next = n2; n1.Kids.Add(n1); n1.Kids.Add(n2);
        var c1 = RT(n1);
        Check(ReferenceEquals(c1.Next.Next, c1) && ReferenceEquals(c1.Kids[0], c1) && ReferenceEquals(c1.Kids[1], c1.Next), "cycles (self and mutual references) round-trip");

        // members with initializers: null / other values must survive, omitted ones get the initializer back
        var wi = RT(new TWithInit { L = null, S = null, I = 0, D = 0 });
        Check(wi.L == null && wi.S == null && wi.I == 0 && Bits(wi.D, 0), "null/0 written over non-null/non-zero initializers");
        var wd = RT(new TWithInit());
        Check(wd.L.SequenceEqual(new[] { 1 }) && wd.S == "x" && wd.I == 7 && double.IsNaN(wd.D), "omitted members get the constructor default");
        var ag = RT(new Agent { Path = null, Pop = new JsObj<double>() });
        Check(ag.Path == null && ag.Pop != null && ag.Cargo == null, "Agent.Path = null survives (initializer is new())");

        // strings
        var strs = new List<string> { "", null, "Dünya uyandı. İğne ışık Çağı ÖŞÜ", "q\"b\\s/\n\t\u0001\u001f\u007f", "😀 \ud800x", "\udc00", "a\ud83d", "<>&'+", "\u2028\u2029", new string('x', 70000) };
        var sb = RT(strs);
        Check(sb.Count == strs.Count && sb.Zip(strs, (x, y) => string.Equals(x, y, StringComparison.Ordinal)).All(x => x), "strings incl. control chars and lone surrogates round-trip");

        // unsupported shapes fail loudly
        Throws<NotSupportedException>(() => SaveCodec.ToJson(new TWithObject { X = 1 }), "object member is rejected");
        Throws<NotSupportedException>(() => SaveCodec.ToJson(new TWithPrivate()), "non-public instance field (hidden state) is rejected");
        Throws<NotSupportedException>(() => SaveCodec.ToJson(new TWithDelegate()), "delegate member is rejected");
        Throws<NotSupportedException>(() => SaveCodec.ToJson(new TWithStruct()), "struct member is rejected");
        Throws<NotSupportedException>(() => SaveCodec.ToJson(new TPoly { X = new TDerived() }), "polymorphic value is rejected");
        Throws<NotSupportedException>(() => SaveCodec.ToJson(new TPoly { X = new TDerived() }, preserveReferences: false), "polymorphic value is rejected (no-ref mode)");
        Check(RT(new TPoly { X = new TBase { B = 4 } }).X.B == 4, "non-sealed class with an exact-type value works");
        var ns = RT(new TWithNoSave { A = 2, Cb = () => { }, Transient = 9 });
        Check(ns.A == 2 && ns.Cb == null && ns.Transient == 5, "[NoSave] members are skipped (constructor default after load)");
    }

    // ---------------------------------------------------------------- sharing in sim types
    private static void Sharing()
    {
        var w = new World { Seed = 1, Width = 2, Height = 1 };
        var war = new War { Since = 3, Attacker = 0, Target = 1, Goal = "land" };
        w.Relations = new List<List<Relation>> { new() { new Relation(), new Relation { War = war } }, new() { new Relation { War = war }, new Relation() } };
        var path = new List<int> { 0, 1 };
        var g = new Guest { Id = 9, Kind = "merchant", Name = "Ali" };
        var inn = new Inn { Id = 4, Name = "Han" };
        inn.Guests.Add(g);
        w.Inns.Add(inn);
        w.Routes.Add(new TradeRoute { Id = 1, Path = path });
        w.Agents.Add(new Agent { Id = 2, Kind = "traveler", Path = path, Guest = g });
        var ev = new GameEvent { Id = 5, Text = "x", Major = true };
        w.Events.Add(ev); w.Events.Add(ev);
        var st = new SaveState
        {
            World = w, RngState = 4.5035996273705e15 * 7, RngCalls = 123,
            PathCache = new Dictionary<string, List<int>> { ["0:1"] = path, ["5:6"] = null, ["1:0"] = new List<int> { 1, 0 } },
            NavCache = new Dictionary<string, List<int>> { ["0:1:0::"] = path }, ShoreW = new byte[] { 0, 1 },
        };
        var ms = new MemoryStream();
        var info = SaveCodec.WriteSave(ms, st);
        var b = SaveCodec.ReadSave(ms.ToArray());
        var gc = GraphCompare.Run(st, b);
        Check(gc.Ok, "SaveState round trip is isomorphic: " + string.Join(" | ", gc.Errors));
        var bw = b.World;
        Check(ReferenceEquals(bw.Relations[0][1].War, bw.Relations[1][0].War), "War shared by both relations");
        Check(ReferenceEquals(bw.Routes[0].Path, bw.Agents[0].Path) && ReferenceEquals(bw.Agents[0].Path, b.PathCache["0:1"]) && ReferenceEquals(b.PathCache["0:1"], b.NavCache["0:1:0::"]), "path list shared by route, agent, path cache and nav cache");
        Check(ReferenceEquals(bw.Agents[0].Guest, bw.Inns[0].Guests[0]), "guest shared by traveller agent and inn");
        Check(ReferenceEquals(bw.Events[0], bw.Events[1]), "event listed twice is one object");
        Check(b.PathCache.Keys.SequenceEqual(st.PathCache.Keys) && b.PathCache.ContainsKey("5:6") && b.PathCache["5:6"] == null, "cache keys in order, null (no path) entries kept");
        Check(Bits(b.RngState, st.RngState) && b.RngCalls == 123, "RNG state above 2^53 round-trips");
        Check(info.Shared == 4 && info.Refs == 6, $"shared/ref counts ({info.Shared} shared, {info.Refs} refs)");
        bw.Relations[0][1].War.Attacks = 7;
        Check(bw.Relations[1][0].War.Attacks == 7, "mutation through one alias is visible through the other");
        var noref = SaveCodec.ReadSave(Save(st, preserve: false));
        var gn = GraphCompare.Run(st, noref);
        Check(!gn.Ok && gn.LostByType.Count > 0, "preserveReferences=false loses sharing and the graph check notices (" + string.Join(", ", gn.LostByType.Select(kv => kv.Key + "×" + kv.Value)) + ")");
    }

    private static byte[] Save(SaveState st, bool preserve = true, bool compress = true)
    {
        var ms = new MemoryStream();
        SaveCodec.WriteSave(ms, st, compress, preserve);
        return ms.ToArray();
    }

    // ---------------------------------------------------------------- strict loading
    private static void Strictness()
    {
        Throws<InvalidDataException>(() => SaveCodec.FromJson<TNode>(Encoding.UTF8.GetBytes("{\"Id\":1,\"Zzz\":2}")), "unknown member is an error");
        Check(SaveCodec.FromJson<TWithNoSave>(Encoding.UTF8.GetBytes("{}")).Transient == 5, "missing member keeps the constructor default");
        Throws<InvalidDataException>(() => SaveCodec.FromJson<List<TNode>>(Encoding.UTF8.GetBytes("[{\"$ref\":1}]")), "dangling $ref is an error");
        Throws<InvalidDataException>(() => SaveCodec.FromJson<List<TNode>>(Encoding.UTF8.GetBytes("[{\"$id\":2,\"Id\":1}]")), "$id out of sequence is an error");
        Throws<InvalidDataException>(() => SaveCodec.FromJson<JsObj<double>>(Encoding.UTF8.GetBytes("{\"a\":1,\"a\":2}")), "duplicate record key is an error");
        Throws<InvalidDataException>(() => SaveCodec.FromJson<JsObj<double>>(Encoding.UTF8.GetBytes("{\"a\":1,\"$x\":2}")), "unescaped $-key is an error");
        Throws<InvalidDataException>(() => SaveCodec.FromJson<List<double>>(Encoding.UTF8.GetBytes("[\"nan\"]")), "bad number string is an error");
        var st = new SaveState { World = new World { Day = 3 } };
        var plain = Save(st, compress: false);
        Check(SaveCodec.ReadSave(plain).World.Day == 3, "plain (uncompressed) JSON save loads");
        string txt = Encoding.UTF8.GetString(plain);
        string ver = $"\"version\":{SaveCodec.Version}";
        Check(txt.StartsWith("{\"format\":\"fd-macro-save\"," + ver + ",\"day\":3,", StringComparison.Ordinal), "header: " + txt.Substring(0, Math.Min(60, txt.Length)));
        Throws<InvalidDataException>(() => SaveCodec.ReadSave(Encoding.UTF8.GetBytes(txt.Replace(ver, "\"version\":99"))), "newer version is rejected");
        // Faz 1b-3: sürüm 1 (araştırma/çağ alanlı) kayıtlar açılmaz
        Throws<InvalidDataException>(() => SaveCodec.ReadSave(Encoding.UTF8.GetBytes(txt.Replace(ver, "\"version\":1"))), "older version is rejected");
        Throws<InvalidDataException>(() => SaveCodec.ReadSave(Encoding.UTF8.GetBytes(txt.Replace("fd-macro-save", "something-else"))), "foreign format is rejected");
        Throws<InvalidDataException>(() => SaveCodec.ReadSave(Encoding.UTF8.GetBytes("[1,2]")), "non-save JSON is rejected");
        var gz = Save(st);
        Throws<Exception>(() => SaveCodec.ReadSave(gz.AsSpan(0, gz.Length / 2).ToArray()), "truncated gzip fails");
        Throws<ArgumentException>(() => SaveCodec.WriteSave(new MemoryStream(), new SaveState()), "saving without a world is rejected");
    }

    // ---------------------------------------------------------------- whole sims
    private static void SimRoundTrips()
    {
        // day 0: the Sim's RNG has already moved (constructor) while World.RngState still holds the worldgen state
        var s0 = new Sim(7);
        Check(!Bits(s0.Rng.State(), s0.W.RngState), "day 0: Rng state differs from World.RngState (so the save stores it separately)");
        var ms = new MemoryStream();
        s0.Save(ms);
        var l0 = Sim.Load(new MemoryStream(ms.ToArray()));
        Check(Bits(l0.Rng.State(), s0.Rng.State()) && l0.Rng.Calls == s0.Rng.Calls, "day-0 save restores the live RNG state");
        Check(GraphCompare.Run(s0.CaptureSave(), l0.CaptureSave()).Ok && Digest.Of(l0) == Digest.Of(s0), "day-0 load is isomorphic, same digest");
        bool same = true;
        for (int d = 1; d <= 40 && same; d++) { s0.Step(); l0.Step(); same = Digest.Of(s0) == Digest.Of(l0); }
        Check(same, "day 0 → 40 identical after load");

        // RNG state past 2^53 keeps its exact sequence across save/load
        var s1 = new Sim(3);
        for (int d = 0; d < 5; d++) s1.Step();
        s1.Rng.SetState(9007199254740993.0 * 1000 + 0.5e3);
        var path = Path.Combine(Path.GetTempPath(), $"fd-savecheck-selftest-{Environment.ProcessId}.sav");
        s1.Save(path);
        var l1 = Sim.Load(path);
        File.Delete(path);
        bool seq = true;
        for (int i = 0; i < 10000; i++) if (!Bits(s1.Rng.Next(), l1.Rng.Next())) { seq = false; break; }
        Check(seq && Bits(s1.Rng.State(), l1.Rng.State()), "RNG sequence beyond 2^53 continues identically after Save(path)/Load(path)");
        Check(!File.Exists(path + ".tmp"), "no temp file left behind");
    }

    // ---------------------------------------------------------------- the test's own digest
    private static void FingerprintSensitivity()
    {
        static string F(object o) => Fingerprint.Of(o);
        var l1 = new List<int> { 1, 2 };
        var sharedLists = new TAll { LL = null, NO = new JsNumObj<List<int>>() };
        sharedLists.NO.Set(1, l1); sharedLists.NO.Set(2, l1);
        var separateLists = new TAll { LL = null, NO = new JsNumObj<List<int>>() };
        separateLists.NO.Set(1, new List<int> { 1, 2 }); separateLists.NO.Set(2, new List<int> { 1, 2 });
        Check(F(sharedLists) != F(separateLists), "fingerprint: one shared list ≠ two equal lists");
        Check(F(new TAll { Dbl = 0.0 }) != F(new TAll { Dbl = -0.0 }), "fingerprint: 0 ≠ -0");
        var o1 = new JsObj<double>(); o1.Set("a", 1); o1.Set("b", 2);
        var o2 = new JsObj<double>(); o2.Set("b", 2); o2.Set("a", 1);
        Check(F(o1) != F(o2), "fingerprint: JsObj key order counts");
        Check(F(new TAll { S = null }) != F(new TAll { S = "" }), "fingerprint: null ≠ \"\"");
        Check(F(new TAll { NI = null }) != F(new TAll { NI = 0 }), "fingerprint: null ≠ 0 (int?)");
        Check(F(new TAll()) == F(new TAll()), "fingerprint: deterministic");
        var c1 = new Dictionary<string, List<int>> { ["a"] = null }; var c2 = new Dictionary<string, List<int>> { ["a"] = new List<int>() };
        Check(F(c1) != F(c2), "fingerprint: null cache entry ≠ empty path");
        var s = new Sim(2);
        string f0 = Fingerprint.Of(s);
        s.Rng.Next();
        Check(Fingerprint.Of(s) != f0, "fingerprint covers the RNG state");
    }

    // ---------------------------------------------------------------- hidden state
    private static void Audits()
    {
        var us = Audit.UnknownSimFields();
        Check(us.Count == 0, "Sim has no instance fields beyond W, G, Rng, _pathCache, NavCache, ShoreW, OnEvent, Cp, LocalCamp (Faz 2 hook, not saved)" + (us.Count > 0 ? ": " + string.Join(", ", us) : ""));
        var ur = Audit.UnknownRngFields();
        Check(ur.Count == 0, "Rng has no state beyond _s and Calls (+ Trace hook)" + (ur.Count > 0 ? ": " + string.Join(", ", ur) : ""));
        var mstat = Audit.MutableStatics();
        Check(mstat.Count == 0, "no non-readonly static fields outside D" + (mstat.Count > 0 ? ": " + string.Join(", ", mstat) : ""));
        try
        {
            var codecOk = SaveCodec.ToJson(new SaveState { World = new World() }).Length > 0;
            Check(codecOk, "every World member type is supported by the codec");
        }
        catch (Exception e) { Check(false, "every World member type is supported by the codec: " + e.Message); }
        Console.WriteLine($"  static fields audited: {Audit.StaticFields().Count} (content digest {Audit.StaticContentDigest()})");
    }
}
