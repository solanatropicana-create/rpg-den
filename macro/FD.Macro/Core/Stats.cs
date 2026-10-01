using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

// Headless ölçüm (Faz 1 A1): bir dünyanın yıllık istatistikleri.
//
// Kullanım:
//   var sim = new Sim(seed);
//   var st = new WorldStats(sim);                // Sim.OnEvent'e bağlanır (varsa önceki işleyici zincirlenir)
//   for (...) { sim.Step(); st.AfterStep(); }    // her günden sonra bir kez
//   st.Finish();                                 // yarım kalan yılı kapatır, özetleri çıkarır, OnEvent'ten ayrılır
//   st.Years[y - 1]["majorEvents"], st.ToJson() ...
//
// Toplayıcı simülasyonu yalnızca okur: RNG, yol önbelleği ya da dünya durumu değişmez (Sim.Capital/CivSettlements
// gibi yan etkisiz yardımcılar dışında Sim çağrılmaz). Durum statik değil, örnekte tutulur; dünyalar paralel ölçülebilir.
//
// Yıl tanımı: 1 yıl = Sim.YEAR (120) gün; y. yıl = (y-1)*120+1 ... y*120. günler (60 yıl = 7200 adım).
// Akış ölçüleri (Flow) yıl içindeki toplam, stok ölçüleri (Stock) yıl sonu değeri, ortalamalar (Mean) yıl içi ortalama ya da orandır.

namespace FD.Macro;

public enum StatKind { Stock, Flow, Mean }

/// <summary>Bir ölçünün tanımı: anahtar (JSON), Türkçe etiket, grup, tür; Share = 0–1 arası pay (raporda yüzde).</summary>
public sealed class StatDef
{
    public readonly string Key, Label, Group;
    public readonly StatKind Kind;
    public readonly bool Share;

    public StatDef(string key, string label, string group, StatKind kind, bool share = false)
    {
        Key = key; Label = label; Group = group; Kind = kind; Share = share;
    }
}

/// <summary>Bir dünyanın bir yılı: <see cref="V"/> skaler ölçüler (<see cref="WorldStats.Defs"/> sırasıyla), <see cref="Maps"/> anahtarlı sayımlar.</summary>
public sealed class YearStats
{
    public int Year, FirstDay, LastDay;
    /// <summary>Yıl tamamlanmadan bitti (koşu yıl ortasında durdu).</summary>
    public bool Partial;
    public readonly double[] V = new double[WorldStats.Defs.Length];
    public readonly SortedDictionary<string, SortedDictionary<string, double>> Maps = new(StringComparer.Ordinal);

    public double this[string key] => V[WorldStats.IndexOf(key)];

    /// <summary>Adlı sayım (yoksa null).</summary>
    public SortedDictionary<string, double> Map(string name) => Maps.TryGetValue(name, out var m) ? m : null;

    internal void Inc(string map, string key, double v = 1)
    {
        if (!Maps.TryGetValue(map, out var m)) { m = new SortedDictionary<string, double>(WorldStats.KeyOrder); Maps[map] = m; }
        m[key] = (m.TryGetValue(key, out double x) ? x : 0) + v;
    }
}

/// <summary>Çöküş: medeniyet yok oldu ("extinct") ya da yaşarken başkentini kaybetti ("capital").</summary>
public sealed class CollapseInfo
{
    public int Day, Year, Civ;
    public string CivName, Kind, Settlement, By;
}

/// <summary>Koşu sonunda medeniyet özeti.</summary>
public sealed class CivSummary
{
    public int Id, MaxSettlements, FinalSettlements, FinalEra, Techs, TreeSize;
    public string Name, Cls, Race;
    public double Founded, FinalPop, Gold, BattlesWon, BattlesLost, Traded;
    public double? ExtinctDay;
    /// <summary>Araştıracak düğüm kalmadığı ve son çağa ulaşıldığı ilk gün (-1: hiç).</summary>
    public double TreeDoneDay = -1;
    public List<double> EraDay = new();
    public bool Alive;
}

/// <summary>
/// Bir dünyanın yıllık istatistiklerini toplar (Faz 1 ölçüm aracı). Bkz. dosya başı. Sim'i yalnızca okur.
/// </summary>
public sealed class WorldStats
{
    public const int YearDays = Sim.YEAR;

    private const string G_CIV = "Medeniyet", G_EV = "Olaylar", G_WAR = "Savaş", G_MON = "Canavarlar", G_HERO = "Kahramanlar", G_INN = "Han ve ticaret";

    /// <summary>Skaler ölçüler (sıra JSON ve rapor sırasıdır).</summary>
    public static readonly StatDef[] Defs =
    {
        new("civsAlive", "Yaşayan medeniyet", G_CIV, StatKind.Stock),
        new("civsNew", "Yeni medeniyet (yeniden doğan)", G_CIV, StatKind.Flow),
        new("civsExtinct", "Yok olan medeniyet", G_CIV, StatKind.Flow),
        new("capitalLost", "Başkent kaybı (medeniyet yaşarken)", G_CIV, StatKind.Flow),
        new("collapses", "Çöküş (yok olma + başkent kaybı)", G_CIV, StatKind.Flow),
        new("settlements", "Yaşayan yerleşim", G_CIV, StatKind.Stock),
        new("settlementsPerCiv", "Medeniyet başına yerleşim", G_CIV, StatKind.Stock),
        new("civsLand5", "5+ kara yerleşimli medeniyet payı", G_CIV, StatKind.Stock, true),
        new("founded", "Kurulan yerleşim", G_CIV, StatKind.Flow),
        new("captured", "Fethedilen yerleşim", G_CIV, StatKind.Flow),
        new("abandoned", "Terk edilen yerleşim", G_CIV, StatKind.Flow),
        new("population", "Toplam nüfus", G_CIV, StatKind.Stock),
        new("eraMean", "Ortalama çağ", G_CIV, StatKind.Stock),
        new("researchDone", "Araştırma ağacının biten payı (ort.)", G_CIV, StatKind.Stock, true),
        new("treeDone", "Ağacı bitmiş medeniyet payı", G_CIV, StatKind.Stock, true),
        new("researchIdle", "Araştırması duran medeniyet payı", G_CIV, StatKind.Stock, true),
        new("goldMedian", "Altın medyanı (medeniyetler)", G_CIV, StatKind.Stock),
        new("idleShare", "Boştaki iş gücü payı", G_CIV, StatKind.Mean, true),
        // Faz 1 B1: bölünme ve büyüklük farkı
        new("secessions", "Bölünme (ayrılıp kurulan medeniyet)", G_CIV, StatKind.Flow),
        new("maxCivSettlements", "En büyük medeniyetin yerleşimi", G_CIV, StatKind.Stock),
        new("events", "Olay", G_EV, StatKind.Flow),
        new("majorEvents", "Büyük olay", G_EV, StatKind.Flow),
        new("battles", "Muharebe", G_WAR, StatKind.Flow),
        new("warsStarted", "Başlayan savaş", G_WAR, StatKind.Flow),
        new("warsActive", "Süren savaş (yıl sonu)", G_WAR, StatKind.Stock),
        new("warsDuring", "Yıl içinde süren savaş", G_WAR, StatKind.Flow),
        new("plunders", "Yağma akını (medeniyet)", G_WAR, StatKind.Flow),
        // Faz 1 B1: tarihî hak, savunma paktı, Kutsal Sefer, ihanet
        new("reclaimWars", "Tarihî hak savaşı", G_WAR, StatKind.Flow),
        new("pactWars", "Pakt gereği savaş", G_WAR, StatKind.Flow),
        new("crusades", "Kutsal Sefer çağrısı", G_WAR, StatKind.Flow),
        new("betrayals", "İhanet (pakt çiğnendi)", G_WAR, StatKind.Flow),
        new("pactsActive", "Savunma paktı (yıl sonu)", G_WAR, StatKind.Stock),
        new("campsAlive", "Yaşayan kamp (yıl sonu)", G_MON, StatKind.Stock),
        new("campsAliveAvg", "Yaşayan kamp (yıl ort.)", G_MON, StatKind.Mean),
        new("campsSpawned", "Doğan kamp", G_MON, StatKind.Flow),
        new("campsCleared", "Temizlenen kamp", G_MON, StatKind.Flow),
        new("raids", "Canavar baskını", G_MON, StatKind.Flow),
        // Faz 1 B2: anlatıcı ve geç tehdit
        new("trollCamps", "Yaşayan trol ini (yıl sonu)", G_MON, StatKind.Stock),
        new("dragonAlive", "Yaşayan ejderha (yıl sonu)", G_MON, StatKind.Stock),
        new("dragonRaids", "Ejderha akını", G_MON, StatKind.Flow),
        new("crises", "Kriz (anlatıcı)", G_MON, StatKind.Flow),
        new("reliefs", "Rahatlama dönemi (anlatıcı)", G_MON, StatKind.Flow),
        new("heroesBorn", "Doğan kahraman", G_HERO, StatKind.Flow),
        new("heroesDied", "Ölen kahraman", G_HERO, StatKind.Flow),
        new("heroesRetired", "Emekli olan kahraman", G_HERO, StatKind.Flow),
        new("heroesGone", "Diyarı terk eden kahraman", G_HERO, StatKind.Flow),
        new("heroesRevived", "Ölümden dönen kahraman", G_HERO, StatKind.Flow),
        new("legends", "Efsane olan kahraman", G_HERO, StatKind.Flow),
        new("heroesAlive", "Yaşayan kahraman (yıl sonu)", G_HERO, StatKind.Stock),
        new("birthLevel", "Doğuş seviyesi (ort.)", G_HERO, StatKind.Mean),
        new("deathLevel", "Ölüm seviyesi (ort.)", G_HERO, StatKind.Mean),
        new("aliveLevel", "Yaşayan kahraman seviyesi (ort.)", G_HERO, StatKind.Stock),
        new("maxLevel", "En yüksek seviye (şimdiye dek)", G_HERO, StatKind.Stock),
        new("innsAlive", "Ayakta han", G_INN, StatKind.Stock),
        new("questsPosted", "Asılan ilan", G_INN, StatKind.Flow),
        new("questsDone", "Biten ilan", G_INN, StatKind.Flow),
        new("tradeTrips", "Ticaret seferi (kervan)", G_INN, StatKind.Flow),
        new("supplyTrips", "İkmal seferi", G_INN, StatKind.Flow),
    };

    /// <summary>Anahtarlı yıllık sayımlar: (ad, Türkçe etiket, tür). Flow = yıl içi toplam, Stock = yıl sonu.</summary>
    public static readonly (string Key, string Label, StatKind Kind)[] MapDefs =
    {
        ("eventsByKind", "Olay türleri", StatKind.Flow),
        ("majorByKind", "Büyük olay türleri", StatKind.Flow),
        ("battleTypes", "Muharebe türleri", StatKind.Flow),
        ("deathCauses", "Kahraman ölüm nedenleri", StatKind.Flow),
        ("levelBirth", "Doğuş seviyesi dağılımı", StatKind.Flow),
        ("levelDeath", "Ölüm seviyesi dağılımı", StatKind.Flow),
        ("levelAlive", "Yaşayan kahraman seviye dağılımı (yıl sonu)", StatKind.Stock),
        ("eraDist", "Çağ dağılımı (yıl sonu)", StatKind.Stock),
        ("civSettlements", "Medeniyet başına yerleşim dağılımı (yıl sonu)", StatKind.Stock),
        ("civLandSettlements", "Medeniyet başına kara yerleşimi dağılımı (yıl sonu; denizaşırı koloniler hariç)", StatKind.Stock),
        ("campsByKind", "Kamp türleri (yıl sonu)", StatKind.Stock),
        ("metricDeltas", "Sim sayaçları (W.Metrics yıllık farkı)", StatKind.Flow),
    };

    /// <summary>Muharebe türleri ve kahraman ölüm nedenleri (battleTypes, deathCauses) için Türkçe etiketler.</summary>
    public static readonly Dictionary<string, string> BattleLabels = new()
    {
        ["siege"] = "Kuşatma", ["plunder"] = "Yağma akını", ["camp"] = "Kamp saldırısı", ["raid"] = "Yerleşim baskını (canavar)",
        ["extRaid"] = "Yapı baskını (canavar)", ["ambush"] = "Yol pususu", ["innMonster"] = "Han baskını (canavar)", ["innCiv"] = "Han baskını (medeniyet)",
        ["naval"] = "Deniz savaşı", ["pirate"] = "Korsan savaşı", ["duel"] = "Düello", ["robbery"] = "Kervan soygunu", ["other"] = "Diğer",
        ["assassination"] = "Suikast", ["unknown"] = "Bilinmiyor",
        // Faz 1 B2: ejderha (inde ya da akında) ve troller (in, akın, pusu)
        ["dragon"] = "Ejderha", ["troll"] = "Trol",
    };

    /// <summary>Olay türleri (GameEvent.Kind) için Türkçe etiketler.</summary>
    public static readonly Dictionary<string, string> EventLabels = new()
    {
        ["sea"] = "Deniz", ["hero"] = "Kahraman", ["class"] = "Sınıf", ["war"] = "Savaş", ["lair"] = "Kamp", ["raid"] = "Baskın", ["inn"] = "Han", ["migration"] = "Göç",
        ["world"] = "Dünya", ["build"] = "İnşaat", ["economy"] = "Ekonomi", ["discover"] = "Keşif", ["settle"] = "Yerleşim", ["quest"] = "Sefer/ilan",
        ["death"] = "Ölüm/terk", ["trade"] = "Ticaret", ["diplomacy"] = "Diplomasi", ["wonder"] = "Harika", ["tension"] = "Gerginlik",
        ["research"] = "Araştırma", ["growth"] = "Büyüme", ["era"] = "Çağ", ["contact"] = "Temas",
        // Faz 1 B2
        ["dragon"] = "Ejderha", ["crisis"] = "Kriz (anlatıcı)", ["relief"] = "Rahatlama (anlatıcı)",
    };

    /// <summary>Kamp türleri için Türkçe etiketler.</summary>
    public static readonly Dictionary<string, string> CampLabels = new()
    {
        ["goblin"] = "Goblin", ["hobgoblin"] = "Hobgoblin", ["bugbear"] = "Bugbear", ["pirate"] = "Korsan",
        ["troll"] = "Trol", ["dragon"] = "Ejderha",   // Faz 1 B2
    };

    /// <summary>Anahtarlı sayımdaki bir anahtarın Türkçe etiketi (bilinmiyorsa anahtarın kendisi).</summary>
    public static string LabelOf(string map, string key)
    {
        var d = map == "battleTypes" || map == "deathCauses" ? BattleLabels : map == "eventsByKind" || map == "majorByKind" ? EventLabels : map == "campsByKind" ? CampLabels : null;
        return key != null && d != null && d.TryGetValue(key, out var l) ? l : key;
    }

    private static readonly Dictionary<string, int> _index = BuildIndex();

    private static Dictionary<string, int> BuildIndex()
    {
        var d = new Dictionary<string, int>(StringComparer.Ordinal);
        for (int i = 0; i < Defs.Length; i++) d.Add(Defs[i].Key, i);
        return d;
    }

    /// <summary>Ölçü anahtarının <see cref="YearStats.V"/> içindeki yeri (yoksa -1).</summary>
    public static int IndexOf(string key) => _index.TryGetValue(key, out int i) ? i : -1;

    /// <summary>Sayısal anahtarlar sayı sırasıyla, diğerleri ordinal (sayılar önce).</summary>
    public static readonly IComparer<string> KeyOrder = Comparer<string>.Create((a, b) =>
    {
        bool na = int.TryParse(a, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out int ia);
        bool nb = int.TryParse(b, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out int ib);
        if (na && nb) return ia.CompareTo(ib);
        if (na != nb) return na ? -1 : 1;
        return string.CompareOrdinal(a, b);
    });

    /// <summary>Gün → ölçüm yılı (1 tabanlı; gün 1–120 → 1).</summary>
    public static int YearOf(int day) => day <= 0 ? 1 : (day - 1) / YearDays + 1;

    private static readonly int I_CIVS = IndexOf("civsAlive"), I_CIVSNEW = IndexOf("civsNew"), I_EXTINCT = IndexOf("civsExtinct"), I_CAPLOST = IndexOf("capitalLost"),
        I_COLLAPSE = IndexOf("collapses"), I_SETL = IndexOf("settlements"), I_SPC = IndexOf("settlementsPerCiv"), I_C5 = IndexOf("civsLand5"), I_FOUNDED = IndexOf("founded"),
        I_CAPTURED = IndexOf("captured"), I_ABANDONED = IndexOf("abandoned"), I_POP = IndexOf("population"), I_ERA = IndexOf("eraMean"), I_RES = IndexOf("researchDone"),
        I_TREE = IndexOf("treeDone"), I_RESIDLE = IndexOf("researchIdle"), I_GOLD = IndexOf("goldMedian"), I_IDLE = IndexOf("idleShare"), I_EVENTS = IndexOf("events"),
        I_MAJOR = IndexOf("majorEvents"), I_BATTLES = IndexOf("battles"), I_WARSTART = IndexOf("warsStarted"), I_WARACT = IndexOf("warsActive"), I_WARDUR = IndexOf("warsDuring"),
        I_PLUNDER = IndexOf("plunders"), I_CAMPS = IndexOf("campsAlive"), I_CAMPSAVG = IndexOf("campsAliveAvg"), I_CAMPSPAWN = IndexOf("campsSpawned"),
        I_CAMPCLEAR = IndexOf("campsCleared"), I_RAIDS = IndexOf("raids"), I_BORN = IndexOf("heroesBorn"), I_DIED = IndexOf("heroesDied"), I_RETIRED = IndexOf("heroesRetired"),
        I_GONE = IndexOf("heroesGone"), I_REVIVED = IndexOf("heroesRevived"), I_LEGENDS = IndexOf("legends"), I_HALIVE = IndexOf("heroesAlive"), I_BLV = IndexOf("birthLevel"),
        I_DLV = IndexOf("deathLevel"), I_ALV = IndexOf("aliveLevel"), I_MAXLV = IndexOf("maxLevel"), I_INNS = IndexOf("innsAlive"), I_QPOST = IndexOf("questsPosted"),
        I_QDONE = IndexOf("questsDone"), I_TRADE = IndexOf("tradeTrips"), I_SUPPLY = IndexOf("supplyTrips"),
        I_SECEDE = IndexOf("secessions"), I_MAXSETL = IndexOf("maxCivSettlements"), I_RECLAIM = IndexOf("reclaimWars"), I_PACTWAR = IndexOf("pactWars"),
        I_CRUSADE = IndexOf("crusades"), I_BETRAY = IndexOf("betrayals"), I_PACTS = IndexOf("pactsActive"),
        I_TROLLS = IndexOf("trollCamps"), I_DRAGON = IndexOf("dragonAlive"), I_DRAIDS = IndexOf("dragonRaids"), I_CRISES = IndexOf("crises"), I_RELIEFS = IndexOf("reliefs");   // B2

    // ------------------------------------------------------------ çıktı
    public Sim Sim { get; private set; }
    public readonly double Seed;
    public readonly List<YearStats> Years = new();
    public readonly List<CollapseInfo> CollapseLog = new();
    public readonly List<CivSummary> Civs = new();
    /// <summary>Efsane olan kahramanlar (koşu sonunda).</summary>
    public readonly List<JObj> LegendList = new();
    public int DaysSeen { get; private set; }
    public int LastDay { get; private set; }
    /// <summary>Koşu boyunca doğan kahraman sayısı ve bunlardan koşu sonunda ölü olanlar.</summary>
    public int HeroesBornTotal, HeroesBornDead;
    /// <summary>Herhangi bir kahramanın ulaştığı en yüksek seviye.</summary>
    public int MaxHeroLevel;
    public bool Finished { get; private set; }

    // ------------------------------------------------------------ izleyiciler
    private sealed class HeroT { public string State; public bool Legend, Initial; public int Level; }
    private sealed class CivT { public bool Alive; public int Cap = -1, MaxSettlements; public double TreeDoneDay = -1; }
    private sealed class SetlT { public bool Alive; public int Civ; }

    private readonly Dictionary<int, HeroT> _heroes = new();
    private readonly Dictionary<int, CivT> _civs = new();
    private readonly Dictionary<int, SetlT> _setls = new();
    private readonly Dictionary<int, Settlement> _setlById = new();
    private readonly Dictionary<int, bool> _camps = new();
    private readonly HashSet<War> _warsSeen = new(ReferenceEqualityComparer.Instance);
    private readonly HashSet<War> _warsYear = new(ReferenceEqualityComparer.Instance);
    private readonly HashSet<War> _warsNow = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<string, HashSet<string>> _tree = new(StringComparer.Ordinal);
    private readonly HashSet<string> _scratch = new(StringComparer.Ordinal);
    private Dictionary<string, double> _metrics0;
    private int _lastBattleId = int.MinValue, _heroCount, _campsNow;
    private readonly int _maxEra;
    private Action<GameEvent> _prevHandler, _handler;
    private bool _detached;

    // yıl birikimleri
    private YearStats _cur;
    private double _birthSum, _birthN, _deathSum, _deathN, _idle, _work, _campsSum, _campDays;

    public WorldStats(Sim sim)
    {
        Sim = sim ?? throw new ArgumentNullException(nameof(sim));
        Seed = sim.W.Seed;
        _maxEra = D.ERA_TR.Count - 1;
        var w = sim.W;
        // başlangıçta var olan her şey "yeni" sayılmaz
        foreach (var h in w.Heroes) _heroes[h.Id] = new HeroT { State = h.State, Legend = h.Legend == true, Initial = true, Level = h.Level };
        _heroCount = w.Heroes.Count;
        foreach (var st in w.Settlements) { _setls[st.Id] = new SetlT { Alive = st.Alive, Civ = st.Civ }; _setlById[st.Id] = st; }
        foreach (var c in w.Civs)
        {
            var t = new CivT { Alive = c.Alive };
            if (c.Alive) { t.Cap = sim.Capital(c)?.Id ?? -1; t.MaxSettlements = sim.CivSettlements(c).Count; }
            _civs[c.Id] = t;
        }
        foreach (var cp in w.Camps) _camps[cp.Id] = cp.Alive;
        foreach (var row in w.Relations) foreach (var r in row) if (r.War != null) _warsSeen.Add(r.War);
        foreach (var b in w.Battles) if (b.Id > _lastBattleId) _lastBattleId = b.Id;
        foreach (var h in w.Heroes) if (h.Level > MaxHeroLevel) MaxHeroLevel = h.Level;
        _metrics0 = SnapshotMetrics(w);
        LastDay = w.Day;
        _cur = NewYear(w.Day + 1);
        _prevHandler = sim.OnEvent;
        var prev = _prevHandler;
        _handler = e => { prev?.Invoke(e); OnEvent(e); };
        sim.OnEvent = _handler;
    }

    private static YearStats NewYear(int firstDay) => new YearStats { Year = YearOf(firstDay), FirstDay = firstDay, LastDay = firstDay - 1 };

    // ------------------------------------------------------------ olay kancası (adım içinde: yalnız okuma, JsObj gezilmez)
    private void OnEvent(GameEvent e)
    {
        if (_detached) return;
        var y = _cur;
        y.V[I_EVENTS]++;
        y.Inc("eventsByKind", e.Kind ?? "?");
        if (e.Major == true) { y.V[I_MAJOR]++; y.Inc("majorByKind", e.Kind ?? "?"); }
        bool deathish = e.Kind == "death" || e.Kind == "class";
        // doğan kahramanın seviyesi doğduğu anda, ölen kahramanın nedeni öldüğü anda okunur
        if (deathish || Sim.W.Heroes.Count != _heroCount) ScanHeroes(deathish ? CauseOf(e) : "unknown");
    }

    private string CauseOf(GameEvent e)
    {
        if (e.Kind == "class" && e.Text != null && e.Text.Contains("suikast", StringComparison.Ordinal)) return "assassination";
        var bs = Sim.W.Battles;
        var b = bs.Count > 0 ? bs[bs.Count - 1] : null;
        return b != null && b.Day == Sim.W.Day ? BattleType(b) : "unknown";
    }

    /// <summary>Muharebe türü (başlık ve taraflardan; bkz. Agents/Inns/Sea/Will'deki başlıklar).</summary>
    public static string BattleType(Battle b)
    {
        string t = b.Title ?? "";
        // Faz 1 B2: ejderha (inindeki savaş ve akınları) ve troller, yerden ve başlıktan önce karşı taraftan tanınır
        if (b.SideB == "Ejderha" || t.EndsWith("ejderha akını", StringComparison.Ordinal)) return "dragon";
        if (b.SideB == "Troller") return "troll";
        if (t == "Düello") return "duel";
        if (t == "Kervan soygunu") return "robbery";
        if (t.EndsWith("pususu", StringComparison.Ordinal)) return "ambush";
        if (t.EndsWith("kuşatması", StringComparison.Ordinal)) return "siege";
        if (t.EndsWith("yağması", StringComparison.Ordinal)) return "plunder";
        if (t.Contains("deniz savaşı", StringComparison.Ordinal)) return "naval";
        if (t.Contains("korsan savaşı", StringComparison.Ordinal) || t.StartsWith("Korsan baskını", StringComparison.Ordinal)) return "pirate";
        if (t.EndsWith("Hanı baskını", StringComparison.Ordinal)) return b.CivA != null ? "innCiv" : "innMonster";
        if (t.EndsWith("baskını", StringComparison.Ordinal))
        {
            string a = b.SideA ?? "";
            if (a.EndsWith("işçileri", StringComparison.Ordinal)) return "extRaid";
            if (a.EndsWith("savunucuları", StringComparison.Ordinal)) return "raid";
            return "camp";
        }
        if (b.Naval == true) return "naval";
        return "other";
    }

    private static string LvKey(int level) => level.ToString(CultureInfo.InvariantCulture);

    private static bool Living(string state) => state != "dead" && state != "gone" && state != "retired";

    private void ScanHeroes(string deathCause)
    {
        var w = Sim.W;
        var y = _cur;
        var hs = w.Heroes;
        for (int i = 0; i < hs.Count; i++)
        {
            var h = hs[i];
            if (!_heroes.TryGetValue(h.Id, out var t))
            {
                t = new HeroT { State = null, Legend = false, Level = h.Level };
                _heroes[h.Id] = t;
                y.V[I_BORN]++;
                y.Inc("levelBirth", LvKey(h.Level));
                _birthSum += h.Level; _birthN++;
            }
            if (h.State != t.State)
            {
                if (h.State == "dead")
                {
                    y.V[I_DIED]++;
                    y.Inc("deathCauses", deathCause);
                    y.Inc("levelDeath", LvKey(h.Level));
                    _deathSum += h.Level; _deathN++;
                }
                else if (t.State == "dead") y.V[I_REVIVED]++;
                if (h.State == "retired") y.V[I_RETIRED]++;
                else if (h.State == "gone") y.V[I_GONE]++;
                t.State = h.State;
            }
            if (h.Legend == true && !t.Legend) { t.Legend = true; y.V[I_LEGENDS]++; }
            if (h.Level > t.Level) t.Level = h.Level;
            if (h.Level > MaxHeroLevel) MaxHeroLevel = h.Level;
        }
        _heroCount = hs.Count;
    }

    // ------------------------------------------------------------ günlük
    /// <summary>Her <see cref="Sim.Step"/> sonrasında bir kez çağrılır. Yıl sonunda (gün % 120 == 0) yılı kapatır.</summary>
    public void AfterStep()
    {
        if (_detached) throw new InvalidOperationException("WorldStats: Finish() sonrası AfterStep çağrıldı");
        var w = Sim.W;
        int day = w.Day;
        if (day <= LastDay) return;      // aynı gün iki kez
        if (YearOf(day) != _cur.Year) CloseYear(LastDay, true);   // gün atlandıysa yarım yılı kapat
        LastDay = day;
        DaysSeen++;
        _cur.LastDay = day;
        ScanHeroes("unknown");
        ScanSettlements();
        ScanCivs(day);
        ScanCamps();
        ScanWars();
        ScanBattles();
        AccumulateIdle();
        if (day % YearDays == 0) CloseYear(day, false);
    }

    private void ScanSettlements()
    {
        var y = _cur;
        foreach (var st in Sim.W.Settlements)
        {
            if (!_setls.TryGetValue(st.Id, out var t))
            {
                _setls[st.Id] = new SetlT { Alive = st.Alive, Civ = st.Civ };
                _setlById[st.Id] = st;
                if (st.Alive) y.V[I_FOUNDED]++;
                continue;
            }
            if (t.Alive && !st.Alive) y.V[I_ABANDONED]++;
            else if (!t.Alive && st.Alive) y.V[I_FOUNDED]++;
            else if (t.Alive && st.Alive && t.Civ != st.Civ) y.V[I_CAPTURED]++;
            t.Alive = st.Alive; t.Civ = st.Civ;
        }
    }

    private void ScanCivs(int day)
    {
        var s = Sim; var w = s.W; var y = _cur;
        foreach (var c in w.Civs)
        {
            if (!_civs.TryGetValue(c.Id, out var t)) { t = new CivT { Alive = false }; _civs[c.Id] = t; }
            if (!t.Alive && c.Alive) y.V[I_CIVSNEW]++;
            else if (t.Alive && !c.Alive)
            {
                y.V[I_EXTINCT]++; y.V[I_COLLAPSE]++;
                CollapseLog.Add(new CollapseInfo { Day = day, Year = YearOf(day), Civ = c.Id, CivName = c.Name, Kind = "extinct" });
                t.Cap = -1;
            }
            t.Alive = c.Alive;
            if (!c.Alive) continue;
            var ss = s.CivSettlements(c);
            if (ss.Count > 0)
            {
                // dün akşamki başkent artık yaşamıyor ya da başkasının: medeniyet yaşarken başkentini kaybetti
                if (t.Cap >= 0 && _setlById.TryGetValue(t.Cap, out var old) && (!old.Alive || old.Civ != c.Id))
                {
                    y.V[I_CAPLOST]++; y.V[I_COLLAPSE]++;
                    CollapseLog.Add(new CollapseInfo
                    {
                        Day = day, Year = YearOf(day), Civ = c.Id, CivName = c.Name, Kind = "capital", Settlement = old.Name,
                        By = old.Alive ? (old.Civ >= 0 && old.Civ < w.Civs.Count ? w.Civs[old.Civ].Name : null) : "terk",
                    });
                }
                t.Cap = s.Capital(c)?.Id ?? -1;
                if (ss.Count > t.MaxSettlements) t.MaxSettlements = ss.Count;
            }
            if (t.TreeDoneDay < 0 && c.Research != null && c.Research.Current == null && c.Era >= _maxEra) t.TreeDoneDay = day;
        }
    }

    private void ScanCamps()
    {
        var y = _cur;
        int alive = 0;
        foreach (var cp in Sim.W.Camps)
        {
            if (!_camps.TryGetValue(cp.Id, out bool was)) { if (cp.Alive) y.V[I_CAMPSPAWN]++; }
            else if (was && !cp.Alive) y.V[I_CAMPCLEAR]++;
            else if (!was && cp.Alive) y.V[I_CAMPSPAWN]++;
            _camps[cp.Id] = cp.Alive;
            if (cp.Alive) alive++;
        }
        _campsNow = alive;
        _campsSum += alive; _campDays++;
    }

    private void ScanWars()
    {
        var y = _cur;
        _warsNow.Clear();
        foreach (var row in Sim.W.Relations)
            foreach (var r in row)
            {
                var war = r.War;
                if (war == null) continue;
                _warsNow.Add(war);
                if (_warsSeen.Add(war)) y.V[I_WARSTART]++;
                _warsYear.Add(war);
            }
    }

    private void ScanBattles()
    {
        var y = _cur;
        var bs = Sim.W.Battles;
        int i = bs.Count - 1;
        while (i >= 0 && bs[i].Id > _lastBattleId) i--;
        for (int k = i + 1; k < bs.Count; k++)
        {
            var b = bs[k];
            y.V[I_BATTLES]++;
            y.Inc("battleTypes", BattleType(b));
            if (b.Id > _lastBattleId) _lastBattleId = b.Id;
        }
    }

    private void AccumulateIdle()
    {
        foreach (var st in Sim.W.Settlements)
        {
            if (!st.Alive || st.Jobs == null) continue;
            // economy.ts: "zanaatçı" = hiçbir işe yerleşemeyen iş gücü (gün başına 0,02 altın); payda bütün iş gücü (askerler de bir iştir)
            double tot = 0, idle = 0;
            foreach (var kv in st.Jobs)
            {
                tot += kv.Value;
                if (kv.Key == "zanaatçı") idle = kv.Value;
            }
            _idle += idle; _work += tot;
        }
    }

    private static Dictionary<string, double> SnapshotMetrics(World w)
    {
        var d = new Dictionary<string, double>(StringComparer.Ordinal);
        foreach (var kv in w.Metrics) d[kv.Key] = kv.Value;
        return d;
    }

    /// <summary>Medeniyete özgü ya da sayaç olmayan W.Metrics anahtarları (yıllık farka girmez).</summary>
    private static bool PerCivMetric(string k)
    {
        if (k.StartsWith("ev_", StringComparison.Ordinal) || k.StartsWith("innOpenDay", StringComparison.Ordinal)) return true;
        int u = k.LastIndexOf('_');
        if (u <= 0 || u == k.Length - 1) return false;
        for (int i = u + 1; i < k.Length; i++) if (k[i] < '0' || k[i] > '9') return false;
        string p = k.Substring(0, u);
        return p == "research" || p == "settle" || p == "innHire";
    }

    private HashSet<string> TreeOf(string cls)
    {
        if (_tree.TryGetValue(cls, out var t)) return t;
        t = new HashSet<string>(StringComparer.Ordinal);
        foreach (var x in D.ALL_TECHS) if (x.Tree == "main" || x.Tree == cls) t.Add(x.Id);
        _tree[cls] = t;
        return t;
    }

    /// <summary>Medeniyetin ağacından (ana ağaç + kendi sınıf ağacı) biten düğüm payı (çift kayıtlar bir sayılır).</summary>
    private double TreeFraction(Civ c, out int done, out int size)
    {
        var tree = TreeOf(c.Cls);
        _scratch.Clear();
        if (c.Research?.Done != null) foreach (var id in c.Research.Done) if (tree.Contains(id)) _scratch.Add(id);
        done = _scratch.Count; size = tree.Count;
        return size > 0 ? done / (double)size : double.NaN;
    }

    private static double Median(List<double> xs)
    {
        if (xs.Count == 0) return double.NaN;
        xs.Sort();
        int n = xs.Count;
        return n % 2 == 1 ? xs[n / 2] : (xs[n / 2 - 1] + xs[n / 2]) / 2;
    }

    private double Delta(string key) => (Sim.W.Metrics.Get(key) ?? 0) - (_metrics0.TryGetValue(key, out double v) ? v : 0);

    private void CloseYear(int day, bool partial)
    {
        var s = Sim; var w = s.W; var y = _cur;
        y.LastDay = day; y.Partial = partial || day - y.FirstDay + 1 < YearDays;
        // medeniyetler
        int civs = 0, civs5 = 0, maxSetl = 0; double eraSum = 0, resSum = 0, tree = 0, resIdle = 0;
        var golds = new List<double>();
        foreach (var c in w.Civs)
        {
            if (!c.Alive) continue;
            civs++;
            var ss = s.CivSettlements(c);
            int n = ss.Count, land = 0;
            if (n > maxSetl) maxSetl = n;
            foreach (var st in ss) if (st.Overseas != true) land++;
            if (land >= 5) civs5++;   // diplomacy.ts:223: anakarada en çok 5 yerleşim (denizaşırı koloniler ayrı)
            y.Inc("civSettlements", n.ToString(CultureInfo.InvariantCulture));
            y.Inc("civLandSettlements", land.ToString(CultureInfo.InvariantCulture));
            eraSum += c.Era;
            y.Inc("eraDist", c.Era.ToString(CultureInfo.InvariantCulture));
            resSum += TreeFraction(c, out _, out _);
            if (c.Research != null && c.Research.Current == null) { resIdle++; if (c.Era >= _maxEra) tree++; }
            golds.Add(s.St(c, "gold"));
        }
        int setl = 0; double pop = 0;
        foreach (var st in w.Settlements) if (st.Alive) { setl++; pop += s.Pop(st); }
        y.V[I_CIVS] = civs;
        y.V[I_SETL] = setl;
        y.V[I_SPC] = civs > 0 ? setl / (double)civs : double.NaN;
        y.V[I_C5] = civs > 0 ? civs5 / (double)civs : double.NaN;
        y.V[I_POP] = pop;
        y.V[I_ERA] = civs > 0 ? eraSum / civs : double.NaN;
        y.V[I_RES] = civs > 0 ? resSum / civs : double.NaN;
        y.V[I_TREE] = civs > 0 ? tree / civs : double.NaN;
        y.V[I_RESIDLE] = civs > 0 ? resIdle / civs : double.NaN;
        y.V[I_GOLD] = Median(golds);
        y.V[I_IDLE] = _work > 0 ? _idle / _work : double.NaN;
        // savaş
        y.V[I_WARACT] = _warsNow.Count;
        y.V[I_WARDUR] = _warsYear.Count;
        // canavarlar
        y.V[I_CAMPS] = _campsNow;
        y.V[I_CAMPSAVG] = _campDays > 0 ? _campsSum / _campDays : double.NaN;
        foreach (var cp in w.Camps) if (cp.Alive) y.Inc("campsByKind", cp.Kind ?? "?");
        // kahramanlar
        int alive = 0; double lvSum = 0;
        foreach (var h in w.Heroes)
        {
            if (!Living(h.State)) continue;
            alive++; lvSum += h.Level;
            y.Inc("levelAlive", LvKey(h.Level));
        }
        y.V[I_HALIVE] = alive;
        y.V[I_ALV] = alive > 0 ? lvSum / alive : double.NaN;
        y.V[I_BLV] = _birthN > 0 ? _birthSum / _birthN : double.NaN;
        y.V[I_DLV] = _deathN > 0 ? _deathSum / _deathN : double.NaN;
        y.V[I_MAXLV] = MaxHeroLevel;
        // han, ilan, ticaret (sim sayaçlarından)
        int inns = 0;
        foreach (var inn in w.Inns) if (inn.Alive) inns++;
        y.V[I_INNS] = inns;
        y.V[I_QPOST] = Delta("questPosted") + Delta("innQuest");
        y.V[I_QDONE] = Delta("questDone");
        y.V[I_RAIDS] = Delta("raids");
        y.V[I_PLUNDER] = Delta("plunder");
        y.V[I_TRADE] = Delta("caravanTrips");
        y.V[I_SUPPLY] = Delta("supplyTrips");
        // Faz 1 B1
        y.V[I_SECEDE] = Delta("secession");
        y.V[I_MAXSETL] = maxSetl;
        y.V[I_RECLAIM] = Delta("reclaimWar");
        y.V[I_PACTWAR] = Delta("pactCall");
        y.V[I_CRUSADE] = Delta("crusade");
        y.V[I_BETRAY] = Delta("betrayal");
        int pacts = 0;
        foreach (var a in w.Civs)
            if (a.Alive)
                foreach (var b in w.Civs)
                    if (b.Alive && a.Id < b.Id && s.Rel(a.Id, b.Id).Pact != null) pacts++;
        y.V[I_PACTS] = pacts;
        // Faz 1 B2
        int trolls = 0, dragons = 0;
        foreach (var cp in w.Camps) if (cp.Alive) { if (cp.Kind == "troll") trolls++; else if (cp.Kind == "dragon") dragons++; }
        y.V[I_TROLLS] = trolls; y.V[I_DRAGON] = dragons;
        y.V[I_DRAIDS] = Delta("dragonRaid"); y.V[I_CRISES] = Delta("crisis"); y.V[I_RELIEFS] = Delta("relief");
        foreach (var kv in w.Metrics)
        {
            if (PerCivMetric(kv.Key)) continue;
            double d = kv.Value - (_metrics0.TryGetValue(kv.Key, out double v0) ? v0 : 0);
            if (d != 0) y.Inc("metricDeltas", kv.Key, d);
        }
        Years.Add(y);
        // yeni yıl
        _metrics0 = SnapshotMetrics(w);
        _birthSum = _birthN = _deathSum = _deathN = _idle = _work = _campsSum = _campDays = 0;
        _warsYear.Clear();
        foreach (var war in _warsNow) _warsYear.Add(war);
        _cur = NewYear(day + 1);
    }

    /// <summary>Koşuyu bitirir: yarım kalan yılı kapatır, özetleri çıkarır ve <see cref="Sim.OnEvent"/>'ten ayrılır. Sim referansı bırakılır.</summary>
    public void Finish()
    {
        if (Finished) return;
        var s = Sim; var w = s.W;
        if (_cur.LastDay >= _cur.FirstDay) CloseYear(_cur.LastDay, true);
        foreach (var h in w.Heroes)
        {
            if (_heroes.TryGetValue(h.Id, out var t) && t.Initial) continue;
            HeroesBornTotal++;
            if (h.State == "dead") HeroesBornDead++;
            if (h.Legend == true)
                LegendList.Add(new JObj
                {
                    { "id", h.Id }, { "name", h.Name }, { "race", h.Race }, { "cls", h.Cls }, { "level", h.Level }, { "born", h.Born },
                    { "state", h.State }, { "deathDay", h.DeathDay }, { "kills", h.Kills }, { "civ", h.Civ },
                });
        }
        foreach (var c in w.Civs)
        {
            _civs.TryGetValue(c.Id, out var t);
            var ss = s.CivSettlements(c);
            double pop = 0;
            foreach (var st in ss) pop += s.Pop(st);
            TreeFraction(c, out int done, out int size);
            Civs.Add(new CivSummary
            {
                Id = c.Id, Name = c.Name, Cls = c.Cls, Race = c.Race, Founded = c.Founded, ExtinctDay = c.ExtinctDay, Alive = c.Alive,
                MaxSettlements = t?.MaxSettlements ?? ss.Count, FinalSettlements = ss.Count, FinalEra = c.Era, FinalPop = pop,
                Techs = done, TreeSize = size, TreeDoneDay = t?.TreeDoneDay ?? -1, EraDay = new List<double>(c.EraDay ?? new List<double>()),
                Gold = s.St(c, "gold"), BattlesWon = c.Stats?.BattlesWon ?? 0, BattlesLost = c.Stats?.BattlesLost ?? 0, Traded = c.Stats?.Traded ?? 0,
            });
        }
        Detach();
        Finished = true;
    }

    /// <summary>OnEvent kancasını söker (zincirdeki önceki işleyici geri konur) ve Sim referansını bırakır.</summary>
    public void Detach()
    {
        if (_detached) return;
        _detached = true;
        if (Sim != null && Sim.OnEvent == _handler) Sim.OnEvent = _prevHandler;
        Sim = null;
    }

    // ------------------------------------------------------------ JSON
    /// <summary>Dünyanın yıllık değerleri, sütun düzeninde (her ölçü bir dizi: yıl 1..n).</summary>
    public JObj ToJson()
    {
        var o = new JObj
        {
            { "seed", Seed },
            { "yearDays", YearDays },
            { "days", LastDay },
            { "years", Years.Count },
        };
        var yearly = new JObj();
        for (int k = 0; k < Defs.Length; k++)
        {
            var col = new double[Years.Count];
            for (int i = 0; i < Years.Count; i++) col[i] = Years[i].V[k];
            yearly.Add(Defs[k].Key, col);
        }
        o.Add("yearly", yearly);
        var maps = new JObj();
        foreach (var (key, _, _) in MapDefs)
        {
            var list = new List<object>();
            foreach (var y in Years) list.Add(MapJson(y.Map(key)));
            maps.Add(key, list);
        }
        o.Add("yearlyMaps", maps);
        var partial = new List<object>();
        foreach (var y in Years) if (y.Partial) partial.Add(y.Year);
        if (partial.Count > 0) o.Add("partialYears", partial);
        o.Add("totals", new JObj
        {
            { "collapses", CollapseLog.Count },
            { "heroesBorn", HeroesBornTotal },
            { "heroesBornDead", HeroesBornDead },
            { "heroesBornDeadShare", HeroesBornTotal > 0 ? HeroesBornDead / (double)HeroesBornTotal : double.NaN },
            { "legends", LegendList.Count },
            { "maxHeroLevel", MaxHeroLevel },
        });
        var col2 = new List<object>();
        foreach (var c in CollapseLog)
            col2.Add(new JObj { { "day", c.Day }, { "year", c.Year }, { "civ", c.Civ }, { "civName", c.CivName }, { "kind", c.Kind }, { "settlement", c.Settlement }, { "by", c.By } });
        o.Add("collapseLog", col2);
        var civs = new List<object>();
        foreach (var c in Civs)
            civs.Add(new JObj
            {
                { "id", c.Id }, { "name", c.Name }, { "cls", c.Cls }, { "race", c.Race }, { "alive", c.Alive }, { "founded", c.Founded }, { "extinctDay", c.ExtinctDay },
                { "maxSettlements", c.MaxSettlements }, { "settlements", c.FinalSettlements }, { "pop", c.FinalPop }, { "era", c.FinalEra }, { "eraDay", c.EraDay },
                { "techs", c.Techs }, { "treeSize", c.TreeSize }, { "treeDoneDay", c.TreeDoneDay < 0 ? (object)null : c.TreeDoneDay }, { "gold", c.Gold },
                { "battlesWon", c.BattlesWon }, { "battlesLost", c.BattlesLost }, { "traded", c.Traded },
            });
        o.Add("civs", civs);
        o.Add("legendList", LegendList);
        return o;
    }

    private static JObj MapJson(SortedDictionary<string, double> m)
    {
        var o = new JObj();
        if (m != null) foreach (var kv in m) o.Add(kv.Key, kv.Value);
        return o;
    }
}

/// <summary>Sıralı JSON nesnesi (ölçüm çıktıları için küçük DOM). Koleksiyon başlatıcıyla: <c>new JObj { { "a", 1 } }</c>.</summary>
public sealed class JObj : List<KeyValuePair<string, object>>
{
    public void Add(string key, object value) => Add(new KeyValuePair<string, object>(key, value));

    public object this[string key]
    {
        get { foreach (var kv in this) if (kv.Key == key) return kv.Value; return null; }
    }
}

/// <summary>
/// <see cref="JObj"/> / liste / sayı ağacını JSON'a yazar: nesneler girintili, yalnız skaler içeren diziler ve nesneler tek satırda.
/// NaN/sonsuz → null; sayılar en çok 4 (|x| ≥ 100 ise 2) ondalıkla.
/// </summary>
public static class StatsJson
{
    public static string Write(object v)
    {
        var sb = new StringBuilder();
        Val(sb, v, 0);
        sb.Append('\n');
        return sb.ToString();
    }

    public static string Num(double d)
    {
        if (double.IsNaN(d) || double.IsInfinity(d)) return "null";
        double r = Math.Abs(d) >= 100 ? Math.Round(d, 2) : Math.Round(d, 4);
        if (r == Math.Floor(r) && Math.Abs(r) < 1e15) return ((long)r).ToString(CultureInfo.InvariantCulture);
        return r.ToString("R", CultureInfo.InvariantCulture);
    }

    public static string Quote(string s)
    {
        var sb = new StringBuilder(s.Length + 2);
        sb.Append('"');
        foreach (char c in s)
        {
            switch (c)
            {
                case '"': sb.Append("\\\""); break;
                case '\\': sb.Append("\\\\"); break;
                case '\n': sb.Append("\\n"); break;
                case '\r': sb.Append("\\r"); break;
                case '\t': sb.Append("\\t"); break;
                default:
                    if (c < ' ') sb.Append("\\u").Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
                    else sb.Append(c);
                    break;
            }
        }
        return sb.Append('"').ToString();
    }

    private static bool Scalar(object v) => v == null || v is string || v is bool || v is double || v is float || v is int || v is long || v is decimal;

    private static void Val(StringBuilder sb, object v, int ind)
    {
        switch (v)
        {
            case null: sb.Append("null"); return;
            case string s: sb.Append(Quote(s)); return;
            case bool b: sb.Append(b ? "true" : "false"); return;
            case double d: sb.Append(Num(d)); return;
            case float f: sb.Append(Num(f)); return;
            case int i: sb.Append(i.ToString(CultureInfo.InvariantCulture)); return;
            case long l: sb.Append(l.ToString(CultureInfo.InvariantCulture)); return;
            case decimal m: sb.Append(Num((double)m)); return;
            case JObj o: Obj(sb, o, ind); return;
            case IDictionary<string, double> dm:
                {
                    var o = new JObj();
                    foreach (var kv in dm) o.Add(kv.Key, kv.Value);
                    Obj(sb, o, ind);
                    return;
                }
            case IEnumerable e: Arr(sb, e, ind); return;
            default: sb.Append(Quote(v.ToString())); return;
        }
    }

    private static void Obj(StringBuilder sb, JObj o, int ind)
    {
        if (o.Count == 0) { sb.Append("{}"); return; }
        bool flat = true;
        foreach (var kv in o) if (!Scalar(kv.Value)) { flat = false; break; }
        if (flat)
        {
            sb.Append('{');
            for (int i = 0; i < o.Count; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(Quote(o[i].Key)).Append(": ");
                Val(sb, o[i].Value, ind);
            }
            sb.Append('}');
            return;
        }
        sb.Append("{\n");
        for (int i = 0; i < o.Count; i++)
        {
            sb.Append(' ', (ind + 1) * 2).Append(Quote(o[i].Key)).Append(": ");
            Val(sb, o[i].Value, ind + 1);
            sb.Append(i < o.Count - 1 ? ",\n" : "\n");
        }
        sb.Append(' ', ind * 2).Append('}');
    }

    private static void Arr(StringBuilder sb, IEnumerable e, int ind)
    {
        var items = new List<object>();
        foreach (var x in e) items.Add(x);
        if (items.Count == 0) { sb.Append("[]"); return; }
        bool flat = true;
        foreach (var x in items) if (!Scalar(x)) { flat = false; break; }
        if (flat)
        {
            sb.Append('[');
            for (int i = 0; i < items.Count; i++) { if (i > 0) sb.Append(", "); Val(sb, items[i], ind); }
            sb.Append(']');
            return;
        }
        sb.Append("[\n");
        for (int i = 0; i < items.Count; i++)
        {
            sb.Append(' ', (ind + 1) * 2);
            Val(sb, items[i], ind + 1);
            sb.Append(i < items.Count - 1 ? ",\n" : "\n");
        }
        sb.Append(' ', ind * 2).Append(']');
    }
}
