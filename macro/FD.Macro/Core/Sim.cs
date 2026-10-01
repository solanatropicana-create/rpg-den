using System;
using System.Collections.Generic;
using System.Linq;

namespace FD.Macro;

/// <summary>
/// The world simulation: state (<see cref="W"/>) + daily <see cref="Step"/>. Exact port of
/// <c>src/sim/sim.ts</c>. Module logic lives in static classes named after the TS files
/// (Economy, Research, Diplomacy, ...), all taking the Sim as first argument like in TS.
/// </summary>
public sealed partial class Sim
{
    public const double FOOD_PER_POP = 0.1;
    public const int YEAR = 120;
    public static readonly string[] SEASONS = { "İlkbahar", "Yaz", "Sonbahar", "Kış" };
    public static readonly string[] TIER_TR = { "Kamp", "Köy", "Kasaba", "Şehir" };
    public static readonly double[] TIER_POP = { 0, 12, 40, 100 };
    public static readonly double[] TIER_SLOTS = { 4, 8, 14, 20 };
    public static readonly double[] TIER_CROWD = { 24, 55, 120, 230 };

    public World W;
    public HexGrid G;
    public Rng Rng;

    // Path caches are part of the simulation's behaviour (stale entries are reused on purpose,
    // like in TS), so they are ported exactly, including the size-triggered clears.
    private readonly Dictionary<string, List<int>> _pathCache = new();
    /// <summary>land + sea routes (keyed by ports and tech; not cleared by road building).</summary>
    public readonly Dictionary<string, List<int>> NavCache = new();
    public byte[] ShoreW;
    public Action<GameEvent> OnEvent;

    /// <summary>Golden-test checkpoint hook (null in normal runs).</summary>
    public Action<string> Cp;

    public int PathCacheSize => _pathCache.Count;

    public Sim(double seed)
    {
        W = WorldGen.GenerateWorld(seed);
        G = new HexGrid(W.Width, W.Height);
        Rng = new Rng(JsMath.ToInt32(W.RngState) ^ unchecked((int)0x9e3779b9));
        UpdateTerritory();
        Discover();
        foreach (var c in W.Civs) { RecomputeEff(c); Research.ChooseResearch(this, c); }
        Log("world", $"Dünya uyandı. {W.Civs.Count} topluluk ilk kamplarını kurdu: {string.Join(", ", W.Civs.Select(c => $"{c.Name} ({D.RACES[c.Race].Plural}, {D.CLASSES[c.Cls].Name})"))}.", major: true);
    }

    private Sim() { }

    /// <summary>Rebuilds a Sim around a saved/snapshotted world (RNG from w.RngState; caches empty).</summary>
    public static Sim FromWorld(World w)
    {
        var s = new Sim { W = w, G = new HexGrid(w.Width, w.Height), Rng = new Rng(1) };
        s.Rng.SetState(w.RngState);
        return s;
    }

    // ------------------------------------------------------------ time & log
    public int Day => W.Day;
    public int Season => (W.Day % YEAR) / (YEAR / 4);
    public int Year => W.Day / YEAR + 1;
    public string DateStr() => DateStr(W.Day);
    public string DateStr(double d) => $"Yıl {J.S(Math.Floor(d / YEAR) + 1)}, {SEASONS[(int)Math.Floor((d % YEAR) / (YEAR / 4))]}";
    public int Id() => W.NextId++;

    public GameEvent Log(string kind, string text, string cause = null, int? civ = null, int? tile = null, int? battle = null, bool? major = null)
    {
        var e = new GameEvent { Id = Id(), Day = W.Day, Kind = kind, Text = text, Cause = cause, Civ = civ, Tile = tile, Battle = battle, Major = major };
        W.Events.Add(e);
        if (W.Events.Count > 2500) W.Events.RemoveRange(0, W.Events.Count - 2500);
        // kalıcı kronik: büyük olaylar kırpılmadan saklanır (aynı nesne; Events'ten düşse de burada kalır)
        if (major == true) W.Chronicle.Add(e);
        Metric("ev_" + kind);
        OnEvent?.Invoke(e);
        return e;
    }

    public void Metric(string k, double v = 1) => W.Metrics.Set(k, (W.Metrics.Get(k) ?? 0) + v);

    // ------------------------------------------------------------ population
    public double Pop(Settlement s)
    {
        double t = 0;
        foreach (var kv in s.Pop) t += kv.Value;
        return t;
    }

    public List<Settlement> CivSettlements(Civ c) => CivSettlements(c.Id);

    public List<Settlement> CivSettlements(int id)
    {
        var o = new List<Settlement>();
        foreach (var s in W.Settlements) if (s.Alive && s.Civ == id) o.Add(s);
        return o;
    }

    public double CivPop(Civ c)
    {
        double a = 0;
        foreach (var s in CivSettlements(c)) a += Pop(s);
        return a;
    }

    /// <summary>Most populous settlement (ties: oldest); null (undefined) when none.</summary>
    public Settlement Capital(Civ c) => J.At(J.Sort(CivSettlements(c), (a, b) => J.Or(Pop(b) - Pop(a), a.Founded - b.Founded)), 0);

    public void AddPop(Settlement s, string r, double n) => s.Pop.Set(r, (s.Pop.Get(r) ?? 0) + n);

    public JsObj<double> RemovePop(Settlement s, double n)
    {
        var outp = new JsObj<double>();
        for (int i = 0; i < n; i++)
        {
            var rs = new List<string>();
            foreach (var k in s.Pop.Keys()) if ((s.Pop.Get(k) ?? 0) > 0) rs.Add(k);
            if (rs.Count == 0) break;
            string r = Rng.Weighted(rs, x => s.Pop.Get(x) ?? 0);
            s.Pop.Set(r, (s.Pop.Get(r) ?? 1) - 1);
            if (!J.T(s.Pop.Get(r))) s.Pop.Delete(r);
            outp.Set(r, (outp.Get(r) ?? 0) + 1);
        }
        return outp;
    }

    public void MergePop(Settlement s, JsObj<double> p)
    {
        if (p != null) foreach (var kv in p) AddPop(s, kv.Key, kv.Value);
    }

    public double PopSize(JsObj<double> p)
    {
        double t = 0;
        if (p != null) foreach (var kv in p) t += kv.Value;
        return t;
    }

    public string RaceStr(JsObj<double> p)
    {
        var parts = new List<string>();
        foreach (var k in p.Keys()) if ((p.Get(k) ?? 0) > 0) parts.Add($"{D.RACES[k].Name} {J.S(p.Get(k).Value)}");
        return string.Join(", ", parts);
    }

    /// <summary>those without housing, living in shacks outside the walls</summary>
    public double Homeless(Settlement s) => JsMath.Max(0, Pop(s) - Housing(s));

    private static readonly string[] HousingKinds = { "hut", "house", "stonehouse" };

    public double Housing(Settlement s)
    {
        double h = 4;
        foreach (var k in HousingKinds) h += (s.Civics.Get(k) ?? 0) * (D.CIVICS[k].Housing ?? 0);
        return JsMath.Max(4, h - (s.BurnedHouses ?? 0) * 4);
    }

    /// <summary>is the extraction building working (not burned, not depleted)</summary>
    public bool ExtWorking(Tile t) => t.Ext != null && !J.T(t.Ext.Depleted) && !J.T(t.Ext.Burned);

    private static readonly string[] NoSlotCivics = { "hut", "house", "stonehouse", "shipyard", "lighthouse" };

    public double SlotsUsed(Settlement s)
    {
        double n = 0;
        foreach (var kv in s.Civics) if (Array.IndexOf(NoSlotCivics, kv.Key) < 0) n += kv.Value;
        foreach (var kv in s.Workshops) n += kv.Value;
        return n;
    }

    public int TierOf(Settlement s)
    {
        var c = W.Civs[s.Civ];
        double P = Pop(s);
        int t = 0;
        for (int i = 1; i < 4; i++) if (P >= TIER_POP[i] && c.Era >= i + 1) t = i;
        return t;
    }

    public int RadiusOf(Settlement s) => 2 + s.Tier;

    // ------------------------------------------------------------ stock
    public double St(Civ c, string g) => c.Stock.Get(g) ?? 0;
    public void Add(Civ c, string g, double v) => c.Stock.Set(g, JsMath.Max(0, (c.Stock.Get(g) ?? 0) + v));

    public bool CanPay(Civ c, JsObj<double> cost)
    {
        foreach (var g in cost.Keys()) if (!(St(c, g) >= (cost.Get(g) ?? 0))) return false;
        return true;
    }

    public void Pay(Civ c, JsObj<double> cost)
    {
        foreach (var g in cost.Keys()) Add(c, g, -(cost.Get(g) ?? 0));
    }

    private static readonly string[] FoodGoods = { "bread", "fish", "meat", "grain" };

    public double FoodTotal(Civ c)
    {
        double t = 0;
        foreach (var g in FoodGoods) t += St(c, g) * (D.GOODS[g].Food ?? 0);
        return t;
    }

    public double Price(Civ c, string g) => c.Price.Get(g) ?? D.GOODS[g].Base;

    public double Worth(Civ c, JsObj<double> st)
    {
        double v = 0;
        foreach (var kv in st) v += kv.Value * Price(c, kv.Key);
        return v;
    }

    // ------------------------------------------------------------ research & effects
    public bool Has(Civ c, string t) => c.Research.Done.Contains(t);

    public void RecomputeEff(Civ c)
    {
        var cls = D.CLASSES[c.Cls];
        var e = new JsObj<double>();
        void AddE(JsObj<double> x)
        {
            if (x == null) return;
            foreach (var kv in x) e.Set(kv.Key, (e.Get(kv.Key) ?? 0) + kv.Value);
        }
        AddE(cls.Base);
        foreach (var t in c.Research.Done) AddE(D.TECH[t]?.Eff);
        var sub = J.Find(cls.Subclasses, x => x.Id == c.Subclass);
        if (sub != null) { AddE(sub.Eff); if (c.Research.Done.Contains($"{c.Cls}_cap")) AddE(sub.CapEff); }
        if (J.Some(W.Settlements, x => x.Alive && x.Civ == c.Id && J.T(x.Civics.Get("wonder")))) AddE(D.WONDERS[c.Cls].Eff);
        c.Eff = e;
    }

    /// <summary>TS <c>s.e(c, k)</c>: summed effect value (0 when absent).</summary>
    public double E(Civ c, string k) => c.Eff.Get(k) ?? 0;

    public string SubclassName(Civ c) => J.Find(D.CLASSES[c.Cls].Subclasses, s => s.Id == c.Subclass)?.Name;
    public string CapName(Civ c) => J.Find(D.CLASSES[c.Cls].Subclasses, s => s.Id == c.Subclass)?.CapName ?? "Uç güç";
    public string TechName(Civ c, string id) => id == $"{c.Cls}_cap" && J.T(c.Subclass) ? $"{D.TECH[id].Name}: {CapName(c)}" : D.TECH[id].Name;
    public string EraName(Civ c) => D.ERA_TR[c.Era];

    /// <summary>Resource gate: own deposit or stock.</summary>
    public bool Access(Civ c, string key)
    {
        if (key == "water") return J.Some(CivSettlements(c), s => J.Some(G.Within(s.Tile, RadiusOf(s)), t => W.Tiles[t].Terrain == "water"));
        if (key == "coast") return J.Some(CivSettlements(c), s => J.Some(G.Within(s.Tile, RadiusOf(s)), t => J.T(W.Tiles[t].Sea)));
        var kinds = key == "gold" ? new[] { "gold", "silver" } : new[] { key };
        foreach (var k in kinds) if (OwnsDeposit(c, k)) return true;
        if (key == "fertile") return false;
        return St(c, key) >= 2;
    }

    public bool OwnsDeposit(Civ c, string kind) =>
        J.Some(W.Deposits, d => d.Kind == kind && !d.Depleted && DepositVisible(c, d)
            && J.Some(d.Tiles, t => TileCiv(t) == c.Id && (D.DEPOSITS[d.Kind].Reserve == 0 || W.Tiles[t].Reserve > 0)));

    public bool DepositVisible(Civ c, Deposit d)
    {
        string h = D.DEPOSITS[d.Kind].HiddenUntil;
        return (!J.T(h) || Has(c, h)) && d.KnownBy.Contains(c.Id);
    }

    private static readonly string[] MajorFinds = { "mana", "mithril", "gold", "tin" };

    public void Discover()
    {
        foreach (var c in W.Civs)
        {
            if (!c.Alive) continue;
            var ss = CivSettlements(c);
            foreach (var d in W.Deposits)
            {
                if (d.KnownBy.Contains(c.Id)) continue;
                if (J.Some(d.Tiles, t => J.Some(ss, s => G.Dist(s.Tile, t) <= RadiusOf(s) + 4)))
                {
                    d.KnownBy.Add(c.Id);
                    var def = D.DEPOSITS[d.Kind];
                    if (def.Count <= 5 && DepositVisible(c, d) && W.Day > 0)
                        Log("discover", $"{c.Name} bir {J.TrLower(def.Name)} keşfetti.", civ: c.Id, tile: d.Tiles[0], major: Array.IndexOf(MajorFinds, d.Kind) >= 0);
                }
            }
        }
    }

    // ------------------------------------------------------------ relations
    public Relation Rel(int a, int b) => W.Relations[a][b];

    public double RelValue(int a, int b)
    {
        double sum = 0;
        foreach (var m in Rel(a, b).Mods) sum += m.Value;
        return JsMath.Max(-100, JsMath.Min(100, JsMath.Round(sum)));
    }

    private static IEnumerable<(int x, int y)> Pairs(int a, int b, bool both)
    {
        yield return (a, b);
        if (both) yield return (b, a);
    }

    public void SetMod(int a, int b, string key, string text, double value, double decay = 0, bool both = true)
    {
        foreach (var (x, y) in Pairs(a, b, both))
        {
            var r = Rel(x, y);
            var m = J.Find(r.Mods, mm => mm.Key == key);
            if (m != null) { m.Value = value; m.Text = text; m.Decay = decay; }
            else r.Mods.Add(new RelMod { Key = key, Text = text, Value = value, Decay = decay });
        }
    }

    public void AddMod(int a, int b, string key, string text, double delta, double cap, double decay, bool both = true)
    {
        foreach (var (x, y) in Pairs(a, b, both))
        {
            var r = Rel(x, y);
            var m = J.Find(r.Mods, mm => mm.Key == key);
            if (m == null) { m = new RelMod { Key = key, Text = text, Value = 0, Decay = decay }; r.Mods.Add(m); }
            m.Value = cap >= 0 ? JsMath.Min(cap, m.Value + delta) : JsMath.Max(cap, m.Value + delta);
            m.Text = text; m.Decay = decay;
        }
    }

    public void RemoveMod(int a, int b, string key, bool both = true)
    {
        foreach (var (x, y) in Pairs(a, b, both)) { var r = Rel(x, y); r.Mods = J.Filter(r.Mods, m => m.Key != key); }
    }

    public bool AtWar(int a, int b) => Rel(a, b).War != null;
    public bool InWar(Civ c) => J.Some(W.Civs, o => o.Id != c.Id && Rel(c.Id, o.Id).War != null);

    // ------------------------------------------------------------ map
    public double MoveCost(int i)
    {
        var t = W.Tiles[i];
        return J.T(t.Sea) ? double.PositiveInfinity : J.T(t.Road) ? 0.5 : D.MOVE_COST.Get(t.Terrain) ?? double.NaN;
    }

    /// <summary>Cached land path (the same list instance is returned on cache hits, like TS). Null when unreachable.</summary>
    public List<int> Path(int from, int to)
    {
        string k = from + ":" + to;
        if (_pathCache.TryGetValue(k, out var cached)) return cached;
        // separate land masses (isle ↔ mainland): no land path, answer without searching the whole map
        var A = W.Tiles[from]; var B = W.Tiles[to];
        if (!J.T(A.Sea) && !J.T(B.Sea) && (A.Isle ?? 0) != (B.Isle ?? 0)) return null;
        var p = HexGrid.FindPath(G, from, to, MoveCost);
        if (_pathCache.Count > 6000) _pathCache.Clear();
        _pathCache[k] = p;
        return p;
    }

    public void ClearPaths() => _pathCache.Clear();

    public Settlement Settlement(int id) => J.Find(W.Settlements, s => s.Id == id);
    public Settlement Settlement(int? id) => id == null ? null : Settlement(id.Value);
    public Hero Hero(int id) => J.Find(W.Heroes, h => h.Id == id);
    public List<Hero> CivHeroes(Civ c) => J.Filter(W.Heroes, h => h.Civ == c.Id && h.State != "dead" && h.State != "gone");

    public int TileCiv(int i)
    {
        int o = W.Tiles[i].Owner;
        if (o < 0) return -1;
        return Settlement(o)?.Civ ?? -1;
    }

    public void UpdateTerritory()
    {
        var w = W;
        foreach (var s in w.Settlements) if (s.Alive) s.Tier = TierOf(s);
        var prev = new int[w.Tiles.Count];
        for (int i = 0; i < prev.Length; i++) prev[i] = w.Tiles[i].Owner;
        foreach (var t in w.Tiles) t.Owner = -1;
        var alive = J.Sort(J.Filter(w.Settlements, s => s.Alive), (a, b) => J.Or(a.Founded - b.Founded, a.Id - b.Id));
        var byId = new Dictionary<int, Settlement>();
        foreach (var s in alive) byId[s.Id] = s;
        for (int i = 0; i < w.Tiles.Count; i++)
        {
            if (!byId.TryGetValue(prev[i], out var o)) continue;
            var ti = w.Tiles[i];
            if (!J.T(ti.Sea) && G.Dist(o.Tile, i) <= RadiusOf(o) && ti.Camp == null && ti.InnZone == null) ti.Owner = o.Id;
        }
        foreach (var s in alive)
        {
            w.Tiles[s.Tile].Owner = s.Id;
            var tiles = J.Sort(G.Within(s.Tile, RadiusOf(s)), (a, b) => G.Dist(s.Tile, a) - G.Dist(s.Tile, b));
            foreach (int t in tiles)
            {
                var tt = w.Tiles[t];
                if (tt.Owner < 0 && !J.T(tt.Sea) && tt.Camp == null && tt.InnZone == null) tt.Owner = s.Id;
            }
        }
        foreach (var t in w.Tiles)
        {
            if (t.Ext == null) continue;
            if (t.Owner < 0) t.Ext = null;
            else if (t.Ext.Settlement != t.Owner)
            {
                var ns = byId[t.Owner];
                int? oldCiv = byId.TryGetValue(t.Ext.Settlement, out var os) ? os.Civ : null;
                if (ns.Civ == oldCiv) t.Ext.Settlement = ns.Id; else t.Ext = null;
            }
        }
    }

    // ------------------------------------------------------------ lifecycle
    public void Abandon(Settlement s, string why)
    {
        if (!s.Alive) return;
        s.Alive = false;
        foreach (var t in W.Tiles) if (t.Owner == s.Id) { t.Owner = -1; t.Ext = null; }
        Log("death", $"{s.Name} terk edildi.", civ: s.Civ, tile: s.Tile, cause: why, major: true);
    }

    public void Extinct(Civ c)
    {
        if (!c.Alive) return;
        c.Alive = false;
        c.ExtinctDay = W.Day;
        foreach (var h in CivHeroes(c))
        {
            if (h.Contract != null) { h.Civ = -1; h.Contract = null; Will.ReturnToBase(this, h); }
            else { h.Civ = -1; h.State = "gone"; }
        }
        foreach (var o in W.Civs) if (o.Id != c.Id) { Rel(o.Id, c.Id).War = null; Rel(c.Id, o.Id).War = null; Rel(o.Id, c.Id).Pact = null; Rel(c.Id, o.Id).Pact = null; }
        // B1: son kalesi fethedilen medeniyetin yok oluşu nedeniyle kroniğe düşer (Diplomacy.CapitalFell)
        Log("death", $"{c.Name} tarihten silindi.", civ: c.Id, cause: c.FallCause, major: true);
    }

    public string HeroTitle(Hero h) => $"{Will.HeroLabel(h)} ({D.RACES[h.Race].Name} {HeroClassTr(h.Cls)}, Sv {h.Level})";

    // ------------------------------------------------------------ step
    public void Step()
    {
        var w = W;
        w.Day++;
        foreach (var c in w.Civs) if (c.Alive) { Economy.EconomyTick(this, c); Cp?.Invoke("econ:" + c.Id); }
        if (w.Day % 5 == 0) { foreach (var c in w.Civs) if (c.Alive) Economy.RepairTick(this, c); Cp?.Invoke("repair"); }
        foreach (var c in w.Civs) if (c.Alive && (w.Day + c.Id * 3) % 10 == 0) CivAI(c);
        if (w.Day % 10 == 0) { UpdateTerritory(); Cp?.Invoke("territory"); Diplomacy.RelationsTick(this); Cp?.Invoke("relations"); }
        if (w.Day % 30 == 0) { Discover(); Cp?.Invoke("discover"); Diplomacy.WorldTick(this); Cp?.Invoke("world"); Events.DisastersTick(this); Cp?.Invoke("disasters"); }
        if (w.Day % YEAR == 0) { foreach (var c in w.Civs) if (c.Alive) Research.ClassYearly(this, c); Cp?.Invoke("yearly"); }
        Monsters.CampsTick(this); Cp?.Invoke("camps");
        Storyteller.Tick(this); Cp?.Invoke("story");   // Faz 1 B2: anlatıcı (gerilim, kriz, rahatlama) ve ejderha
        Heroes.TavernsTick(this); Cp?.Invoke("taverns");
        Inns.InnsTick(this); Cp?.Invoke("inns");
        if (w.Day % 5 == 0) { Heroes.HeroesTick(this); Cp?.Invoke("heroes"); }
        Agents.AgentsTick(this); Cp?.Invoke("agents");
        Sea.SeaTick(this); Cp?.Invoke("sea");
        Agents.RoutesTick(this); Cp?.Invoke("routes");
        Agents.RoadsTick(this); Cp?.Invoke("roads");
        foreach (var c in w.Civs) c.Threat = JsMath.Min(1.5, JsMath.Max(0, c.Threat - 0.0015));
        if (w.Day % 60 == 0)
            foreach (var c in w.Civs)
                if (c.Alive) c.History.Add(new HistPoint { Day = w.Day, Pop = CivPop(c), Gold = JsMath.Round(St(c, "gold")), Techs = c.Research.Done.Count });
        w.RngState = Rng.State();
        Cp?.Invoke("end");
    }

    public void CivAI(Civ c)
    {
        string p = Cp != null ? "ai:" + c.Id + ":" : null;
        if (CivSettlements(c).Count == 0) { Extinct(c); Cp?.Invoke(p + "extinct"); return; }
        if (!J.T(c.Research.Current)) Research.ChooseResearch(this, c);
        Cp?.Invoke(p + "research");
        Research.EraCheck(this, c); Cp?.Invoke(p + "era");
        Economy.ChooseBuilds(this, c); Cp?.Invoke(p + "builds");
        Economy.RecruitTick(this, c); Cp?.Invoke(p + "recruit");
        Diplomacy.ConsiderExpansion(this, c); Cp?.Invoke(p + "expand");
        Heroes.ConsiderHero(this, c); Cp?.Invoke(p + "hero");
        Inns.ConsiderInnBids(this, c); Cp?.Invoke(p + "bids");
        Heroes.ConsiderQuest(this, c); Cp?.Invoke(p + "quest");
        Diplomacy.ConsiderScout(this, c); Cp?.Invoke(p + "scout");
        Diplomacy.ConsiderTrade(this, c); Cp?.Invoke(p + "trade");
        Diplomacy.ConsiderWarAction(this, c); Cp?.Invoke(p + "war");
        Diplomacy.ConsiderRaid(this, c); Cp?.Invoke(p + "raid");
    }

    private static readonly Dictionary<string, string> ClassTr = new()
    {
        ["fighter"] = "Savaşçı", ["wizard"] = "Büyücü", ["cleric"] = "Rahip", ["rogue"] = "Hırsız",
        ["ranger"] = "Korucu", ["paladin"] = "Paladin", ["druid"] = "Druid", ["barbarian"] = "Barbar",
    };

    public static string HeroClassTr(string c) => ClassTr.TryGetValue(c, out var t) ? t : c;
}
