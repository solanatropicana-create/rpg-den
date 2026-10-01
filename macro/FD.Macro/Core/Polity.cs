using System;
using System.Collections.Generic;

// Faz 1b-6: devlet, inanç ve örgüt modeli (claude/devlet-orgut-spec.md). 12 sınıf-medeniyetin yerine:
//  - Irk bir nüfus özelliğidir (Settlement.Pop zaten ırk dağılımıdır). Devletin kültürü çoğunluk ırkından gelir (Civ.Race):
//    ad kökleri, kent adları, arazi zevki, istekleri, kültür birimi (CultureDef).
//  - Devlet siyasi kabuktur: hükümet tipi (Civ.Gov: kingdom, clans, republic, theocracy), yönetici (Person), meşruiyet, yasa
//    (LawState). Saldırganlık, hizalama ve kahraman yakınlığı tipten, kültürden ve yöneticiden gelir (GovDef).
//  - Üç inanç (sun, old, pact) ve inançsızlar (none): her yerleşimin dağılımı tutulur (Settlement.Faith, States.FaithTick).
//  - Örgütler (sınıflar burada yaşar): OrgDef (bu dosya), Org (PolityTypes.cs), Modules/Orgs.cs.
// Eski sınıf alanlarının eşlemesi spec §8'deki gibi (DURUM.md, Faz 1b-6).

namespace FD.Macro;

/// <summary>Spec §1: ana ırkın kültürü (devletin kültürü çoğunluk ırkından gelir).</summary>
public sealed class CultureDef
{
    public string Race;
    /// <summary>devlet adı kökleri</summary>
    public List<string> Stems;
    /// <summary>ilk yerleşimin (başkent) adları</summary>
    public List<string> Capitals;
    public List<string> Towns;
    public string Color;
    public JsObj<double> TerrainLike;
    /// <summary>yatkınlıklar (deniz: kıyı ve tersane)</summary>
    public double Sea;
    public List<string> Desires;
    /// <summary>kültür birimi (Kasaba kademesinden)</summary>
    public string Unit;
    /// <summary>saldırganlığa katkı</summary>
    public double Aggression;
    /// <summary>yöneticinin iyilik ortalaması</summary>
    public double Good;
    public JsObj<double> Eff;
    /// <summary>bu kültürün kahraman sınıfı yakınlığı (krallıkta)</summary>
    public string Hero;
    /// <summary>hükümet tiplerine yatkınlık (dünya üretiminde tip ataması)</summary>
    public JsObj<double> Govs;
}

/// <summary>Spec §2: hükümet tipi.</summary>
public sealed class GovDef
{
    public string Id, Name;
    /// <summary>devlet adı kalıbı ("{0} Krallığı")</summary>
    public string Pattern;
    /// <summary>yöneticinin unvanı</summary>
    public string Title;
    /// <summary>halef: dynasty | duel | vote | order</summary>
    public string Succession;
    /// <summary>hizalamanın kanun ekseni (yöneticiyle karışır)</summary>
    public double Law;
    public double Aggression;
    public JsObj<double> Eff;
    /// <summary>tipin birimi (boylarda Akıncı); null yok</summary>
    public string Unit;
    /// <summary>başkentin özel yapısı (eski "sınıf yapısı")</summary>
    public string Seat;
    /// <summary>kahraman sınıfı yakınlığı (null: kültürden)</summary>
    public string Hero;
    /// <summary>yasanın varsayılanı</summary>
    public double Harsh, Smuggle, Bribe, Patrol;
    public bool Slavery, PrisonMine;
    public string Faith;
    public JsObj<double> FaithTol;
    /// <summary>çöküş yolları (Faz 1b/7)</summary>
    public List<string> Collapse;
}

public sealed class FaithDef
{
    public string Id, Name;
    /// <summary>mabedi</summary>
    public string Shrine;
}

/// <summary>Spec §4: örgüt tanımı (sınıfın eşlemesi: alt sınıflar → rütbe yolları, uç güç → iç çember sırrı, özel birimler →
/// örgüt birlikleri, yıllık sınıf olayı → örgüt olayı, harika → merkezin landmark binası).</summary>
public sealed class OrgDef
{
    public string Id, Name;
    /// <summary>eski sınıfı (yoksa "yeni")</summary>
    public string Source;
    /// <summary>bağlı inanç (null: yok)</summary>
    public string Faith;
    public double Good, Law;
    /// <summary>yasal durum, hükümet tipine göre: patron (himaye) | legal | banned</summary>
    public JsObj<string> Legal;
    /// <summary>yasak olduğu yerde gizli hücrelerle yaşar (Pakt, Hırsızlar, Özgürlük Ağı)</summary>
    public bool Secret;
    /// <summary>merkezin landmark binası (eski harikalar buraya geçer, bonusu yok) ve şubenin adı</summary>
    public string Hq, Branch;
    public List<string> Units;
    public List<string> Enemies;
    /// <summary>gerilim (düşman değil)</summary>
    public List<string> Rivals;
    /// <summary>rütbe yolları (eski alt sınıflar)</summary>
    public List<string> Ranks;
    /// <summary>iç çember sırrı (eski uç güç)</summary>
    public string Inner;
    /// <summary>yıllık olay (eski sınıf olayı)</summary>
    public string Yearly;
    public List<string> Goals;
    /// <summary>şube açabileceği en düşük yerleşim kademesi</summary>
    public int MinTier;
    /// <summary>bu örgütün doğal kahraman yolu / sınıfları (üyelik)</summary>
    public List<string> Paths, Classes;
}

public static class Polity
{
    public static readonly List<string> MAIN_RACES = new() { "human", "dwarf", "elf", "halfling", "halforc", "dragonborn", "tiefling" };
    public static readonly List<string> GOV_IDS = new() { "kingdom", "clans", "republic", "theocracy" };
    public static readonly List<string> FAITH_IDS = new() { "sun", "old", "pact", "none" };

    public static readonly JsObj<FaithDef> FAITHS = new()
    {
        ["sun"] = new FaithDef { Id = "sun", Name = "Güneş Kilisesi", Shrine = "tapınak" },
        ["old"] = new FaithDef { Id = "old", Name = "Eski İnanç", Shrine = "taş sunak" },
        ["pact"] = new FaithDef { Id = "pact", Name = "Pakt", Shrine = "gizli mahzen" },
        ["none"] = new FaithDef { Id = "none", Name = "inançsız", Shrine = "" },
    };

    private static JsObj<double> O(params (string K, double V)[] kv)
    {
        var o = new JsObj<double>();
        foreach (var (k, v) in kv) o.Set(k, v);
        return o;
    }

    public static readonly JsObj<CultureDef> CULTURES = new()
    {
        ["human"] = new CultureDef
        {
            Race = "human", Stems = new() { "Güneştacı", "Akburç", "Kalkanova", "Şafaktepe", "Yeminköprü" }, Capitals = new() { "Altınkapı", "Sessiztepe" },
            Towns = new() { "Şafaktepe", "Kalkanova", "Yeminköprü", "Akburç", "Işıkdere", "Çankule", "Sisliyamaç", "Dinginpınar", "Taşbasamak", "Bulutkapı" },
            Color = "#b5871a", TerrainLike = O(("grass", 1.5), ("hill", 1.1), ("forest", 0.6)), Sea = 1, Desires = new() { "horses", "iron" },
            Unit = "pikeman", Aggression = 0, Good = 0.45, Eff = O(("growth", 0.05)), Hero = "fighter",
            Govs = O(("kingdom", 3), ("theocracy", 2), ("republic", 1), ("clans", 0.3)),
        },
        ["dwarf"] = new CultureDef
        {
            Race = "dwarf", Stems = new() { "Örsyürek", "Demirçan", "Taşkandil", "Granitsunak", "Karaörs" }, Capitals = new() { "Kutsalörs", "Kara Mihrap" },
            Towns = new() { "Demirçan", "Taşkandil", "Derinmihrap", "Közkapı", "Granitsunak", "Külçukur", "Sessizocak", "Karagöl", "Zincirkaya", "Gölgeörs" },
            Color = "#3d7ab8", TerrainLike = O(("hill", 2), ("mountain", 1.5), ("grass", 0.6)), Sea = 0.8, Desires = new() { "iron", "gold" },
            Unit = "ironguard", Aggression = 0, Good = 0.3, Eff = O(("prodMine", 0.1)), Hero = "cleric",
            Govs = O(("kingdom", 2.5), ("theocracy", 1.5), ("republic", 1), ("clans", 0.5)),
        },
        ["elf"] = new CultureDef
        {
            Race = "elf", Stems = new() { "Yeşilyaprak", "Ayışığı", "Sessizkoru", "Gümüşdal" }, Capitals = new() { "Sessizkoru" },
            Towns = new() { "Yosunpınar", "Geyikyurt", "Çiyardı", "Ayışığı Korusu", "Kökbağ", "İzsürer", "Okyayı", "Yabanyurt", "Çamgözcü" },
            Color = "#3f9e6b", TerrainLike = O(("forest", 2), ("oldforest", 3), ("grass", 0.8)), Sea = 1, Desires = new() { "herbs", "meat" },
            Unit = "longbow", Aggression = -0.1, Good = 0.6, Eff = O(("prodHerbs", 0.5), ("growth", 0.1)), Hero = "ranger",
            Govs = O(("kingdom", 1.5), ("clans", 1.5), ("republic", 1), ("theocracy", 0.4)),
        },
        ["halfling"] = new CultureDef
        {
            Race = "halfling", Stems = new() { "Tatlıçayır", "Balköprü", "Keseli", "Fıçıköy" }, Capitals = new() { "Kavşakpazar" },
            Towns = new() { "Fıçıköy", "Gölgeçarşı", "Balköprü", "Keseli", "Kırkkapı", "Tamburlu", "Kavalpınar", "Şarkıdere", "Neşeliova", "Telliköprü" },
            Color = "#c9722b", TerrainLike = O(("grass", 1.5), ("water", 1.2)), Sea = 1.4, Desires = new() { "salt", "gold" },
            Unit = "slinger", Aggression = -0.05, Good = 0.25, Eff = O(("tradeGold", 0.15), ("raidEvade", 0.2)), Hero = "rogue",
            Govs = O(("republic", 3), ("kingdom", 1), ("theocracy", 0.5), ("clans", 0.3)),
        },
        ["halforc"] = new CultureDef
        {
            Race = "halforc", Stems = new() { "Kanlıdiş", "Kurtoba", "Kızılyurt", "Toynakbaş" }, Capitals = new() { "Kemikçadır" },
            Towns = new() { "Kurtoba", "Savaşçukur", "Kızılyurt", "Toynakbaş", "Kafatepe", "Kurtgeçit" },
            Color = "#b23b30", TerrainLike = O(("grass", 1.6), ("tundra", 1.2), ("hill", 1)), Sea = 1.4, Desires = new() { "horses", "meat" },
            Unit = "raider", Aggression = 0.25, Good = -0.1, Eff = O(("soldierDmg", 1)), Hero = "barbarian",
            Govs = O(("clans", 3), ("kingdom", 0.7), ("republic", 0.3), ("theocracy", 0.3)),
        },
        ["dragonborn"] = new CultureDef
        {
            Race = "dragonborn", Stems = new() { "Pulzırh", "Közburç", "Alevgeçit", "Demirkanat" }, Capitals = new() { "Ejderkale" },
            Towns = new() { "Pulkalkan", "Közburç", "Kılıçyurt", "Demirkanat", "Alevgeçit" },
            Color = "#b0703a", TerrainLike = O(("hill", 1.6), ("grass", 1.2), ("mountain", 0.8)), Sea = 1.1, Desires = new() { "iron", "arms" },
            Unit = "flamebreath", Aggression = 0.2, Good = 0.1, Eff = O(("soldierAc", 1)), Hero = "fighter",
            Govs = O(("kingdom", 2), ("clans", 1.5), ("theocracy", 1), ("republic", 0.7)),
        },
        ["tiefling"] = new CultureDef
        {
            Race = "tiefling", Stems = new() { "Kızılboynuz", "Közsaray", "Karamum", "Alazvadi" }, Capitals = new() { "Közsaray" },
            Towns = new() { "Kızılkül", "Boynuztepe", "Alazvadi", "Karamum", "Kıvılcımlı" },
            Color = "#a33d6b", TerrainLike = O(("swamp", 1.2), ("hill", 1.3), ("grass", 1)), Sea = 1, Desires = new() { "mana", "gold" },
            Unit = "flameborn", Aggression = 0.1, Good = -0.15, Eff = new JsObj<double>(), Hero = "wizard",
            Govs = O(("republic", 2.5), ("kingdom", 1), ("clans", 1), ("theocracy", 0.1)),
        },
    };

    public static readonly JsObj<GovDef> GOVS = new()
    {
        ["kingdom"] = new GovDef
        {
            Id = "kingdom", Name = "Krallık", Pattern = "{0} Krallığı", Title = "Lord", Succession = "dynasty", Law = 0.6, Aggression = 0.3,
            Eff = new JsObj<double>(), Seat = "Saray", Hero = null,
            Harsh = 0.55, Smuggle = 0.5, Bribe = 0.4, Patrol = 0.6, Slavery = false, Faith = "sun",
            FaithTol = O(("sun", 1), ("old", 0.6), ("pact", 0), ("none", 0.8)), Collapse = new() { "succession", "nobles" },
        },
        ["clans"] = new GovDef
        {
            Id = "clans", Name = "Boy konfederasyonu", Pattern = "{0} Boyları", Title = "Reis", Succession = "duel", Law = -0.6, Aggression = 0.55,
            Eff = O(("lootMult", 0.5)), Unit = "raider", Seat = "Boy Meclisi", Hero = "barbarian",
            Harsh = 0.35, Smuggle = 0.2, Bribe = 0.6, Patrol = 0.4, Slavery = true, Faith = "old",
            FaithTol = O(("sun", 0.5), ("old", 1), ("pact", 0.1), ("none", 0.8)), Collapse = new() { "duel", "split" },
        },
        ["republic"] = new GovDef
        {
            Id = "republic", Name = "Tüccar cumhuriyeti", Pattern = "{0} Cumhuriyeti", Title = "Konsey Başı", Succession = "vote", Law = 0, Aggression = 0.1,
            Eff = O(("tradeGold", 0.3)), Seat = "Lonca Konseyi", Hero = "rogue",
            Harsh = 0.3, Smuggle = 0.2, Bribe = 0.85, Patrol = 0.5, Slavery = true, Faith = null,
            FaithTol = O(("sun", 0.9), ("old", 0.9), ("pact", 0.2), ("none", 1)), Collapse = new() { "coup", "bankrupt" },
        },
        ["theocracy"] = new GovDef
        {
            Id = "theocracy", Name = "Teokrasi", Pattern = "{0} Teokrasisi", Title = "Başrahip", Succession = "order", Law = 0.8, Aggression = 0.3,
            Eff = O(("festival", 1), ("smite", 1)), Seat = "Başkatedral", Hero = "cleric",
            Harsh = 0.85, Smuggle = 0.8, Bribe = 0.1, Patrol = 0.8, Slavery = false, PrisonMine = true, Faith = "sun",
            FaithTol = O(("sun", 1), ("old", 0.2), ("pact", 0), ("none", 0.4)), Collapse = new() { "schism", "excommunication", "crusadeLost" },
        },
    };

    /// <summary>Irkın inanç eğilimi (sun, old, pact, none): yerleşimin inanç hedefinin tabanı (nüfusa ağırlıklı).</summary>
    public static readonly JsObj<JsObj<double>> RACE_FAITH = new()
    {
        ["human"] = O(("sun", 0.6), ("old", 0.25), ("pact", 0.03), ("none", 0.12)),
        ["dwarf"] = O(("sun", 0.6), ("old", 0.25), ("pact", 0.03), ("none", 0.12)),
        ["elf"] = O(("sun", 0.2), ("old", 0.7), ("pact", 0.02), ("none", 0.08)),
        ["halfling"] = O(("sun", 0.45), ("old", 0.35), ("pact", 0.02), ("none", 0.18)),
        ["gnome"] = O(("sun", 0.4), ("old", 0.3), ("pact", 0.05), ("none", 0.25)),
        ["halfelf"] = O(("sun", 0.4), ("old", 0.4), ("pact", 0.03), ("none", 0.17)),
        ["halforc"] = O(("sun", 0.2), ("old", 0.7), ("pact", 0.04), ("none", 0.06)),
        ["dragonborn"] = O(("sun", 0.5), ("old", 0.35), ("pact", 0.03), ("none", 0.12)),
        ["tiefling"] = O(("sun", 0.3), ("old", 0.2), ("pact", 0.25), ("none", 0.25)),
    };

    private static JsObj<string> L(string kingdom, string clans, string republic, string theocracy) =>
        new() { ["kingdom"] = kingdom, ["clans"] = clans, ["republic"] = republic, ["theocracy"] = theocracy };

    public const string PATRON = "patron", LEGAL = "legal", BANNED = "banned";

    public static readonly List<string> ORG_IDS = new() { "church", "order", "pact", "circle", "hunters", "thieves", "academy", "companies", "bards", "merchants", "explorers", "slavers", "freedom" };

    public static readonly JsObj<OrgDef> ORGS = new()
    {
        ["church"] = new OrgDef
        {
            Id = "church", Name = "Güneş Kilisesi", Source = "cleric", Faith = "sun", Good = 0.6, Law = 0.7, Legal = L(PATRON, LEGAL, LEGAL, PATRON),
            Hq = "Güneş Katedrali", Branch = "tapınak", Units = new() { "blessed" }, Enemies = new() { "pact" }, Rivals = new() { "circle" },
            Ranks = new() { "Yaşam Yolu", "Ocak Yolu", "Savaş Yolu" }, Inner = "İlahi Müdahale", Yearly = "Güneş Bayramı",
            Goals = new() { "hacıları korumak", "salgında şifa taşımak", "Pakt'ı ihbar etmek" }, MinTier = 1, Paths = new() { "healer" }, Classes = new() { "cleric" },
        },
        ["order"] = new OrgDef
        {
            Id = "order", Name = "Güneş Tarikatı", Source = "paladin", Faith = "sun", Good = 0.7, Law = 0.9, Legal = L(PATRON, LEGAL, LEGAL, PATRON),
            Hq = "Kale-Manastır", Branch = "tarikat evi", Units = new() { "holyguard", "knight" }, Enemies = new() { "pact" }, Rivals = new(),
            Ranks = new() { "Yemin Yolu", "Kadimler Yolu", "İntikam Yolu" }, Inner = "Avatar", Yearly = "Kutsal Sefer çağrısı",
            Goals = new() { "Kutsal Sefer", "Pakt hücresi avı" }, MinTier = 2, Paths = new(), Classes = new() { "paladin" },
        },
        ["pact"] = new OrgDef
        {
            Id = "pact", Name = "Kara Pakt", Source = "warlock", Faith = "pact", Good = -0.8, Law = 0.3, Legal = L(BANNED, BANNED, BANNED, BANNED), Secret = true,
            Hq = "Gizli Mahzen", Branch = "hücre", Units = new() { "imp", "fiend" }, Enemies = new() { "order", "church" }, Rivals = new(),
            Ranks = new() { "İblis Paktı", "Peri Paktı", "Kadim Pakt" }, Inner = "Kara Kapı", Yearly = "Kara Ayin",
            Goals = new() { "suikast", "ruh sözleşmesi", "yetkililere sızmak" }, MinTier = 2, Paths = new() { "dark" }, Classes = new(),
        },
        ["circle"] = new OrgDef
        {
            Id = "circle", Name = "Druid Çemberi", Source = "druid", Faith = "old", Good = 0.4, Law = 0, Legal = L(LEGAL, PATRON, LEGAL, BANNED),
            Hq = "Taş Halka", Branch = "koru", Units = new() { "treant", "wolves" }, Enemies = new(), Rivals = new() { "church", "merchants" },
            Ranks = new() { "Toprak Çemberi", "Ay Çemberi", "Yıldız Çemberi" }, Inner = "Doğanın Uyanışı", Yearly = "Koru Ayini",
            Goals = new() { "ağaç kesimini sabote etmek", "kirleten madeni kapatmak", "canavar dengesi" }, MinTier = 0, Paths = new(), Classes = new() { "druid" },
        },
        ["hunters"] = new OrgDef
        {
            Id = "hunters", Name = "Avcılar Locası", Source = "ranger", Faith = null, Good = 0.3, Law = 0.2, Legal = L(LEGAL, LEGAL, LEGAL, LEGAL),
            Hq = "Sınır Karakolu", Branch = "av köşkü", Units = new() { "warden" }, Enemies = new(), Rivals = new(),
            Ranks = new() { "Avcı Yolu", "Hayvan Ustası", "Gölge Avcısı" }, Inner = "Gözde Av", Yearly = "Büyük Av",
            Goals = new() { "kamp ve canavar ödülleri", "kaçak suçlu avı" }, MinTier = 1, Paths = new() { "hunter" }, Classes = new() { "ranger" },
        },
        ["thieves"] = new OrgDef
        {
            Id = "thieves", Name = "Hırsızlar Loncası", Source = "rogue", Faith = null, Good = -0.3, Law = -0.3, Legal = L(BANNED, BANNED, LEGAL, BANNED), Secret = true,
            Hq = "Şehrin Altı", Branch = "meyhane arkası", Units = new() { "shadowguard" }, Enemies = new() { "merchants" }, Rivals = new(),
            Ranks = new() { "Hırsız", "Suikastçı", "Akıl Hocası" }, Inner = "Usta Hırsız", Yearly = "Gölge Müzayedesi",
            Goals = new() { "soygun", "kaçakçılık", "borç tahsili", "suikast" }, MinTier = 2, Paths = new() { "dark" }, Classes = new() { "rogue" },
        },
        ["academy"] = new OrgDef
        {
            Id = "academy", Name = "Büyücü Akademisi", Source = "wizard", Faith = null, Good = 0.4, Law = 0.3, Legal = L(PATRON, LEGAL, PATRON, LEGAL),
            Hq = "Yıldız Kulesi", Branch = "parşömen dükkânı", Units = new() { "golem" }, Enemies = new(), Rivals = new(),
            Ranks = new() { "Yıkım Okulu", "Koruma Okulu", "Kehanet Okulu" }, Inner = "Kadim Bilgi", Yearly = "Yıldız Gecesi",
            Goals = new() { "antik eser getirmek", "büyü kazası temizliği" }, MinTier = 2, Paths = new() { "sage" }, Classes = new() { "wizard" },
        },
        ["companies"] = new OrgDef
        {
            Id = "companies", Name = "Paralı Bölükler", Source = "fighter", Faith = null, Good = 0, Law = 0.4, Legal = L(LEGAL, LEGAL, LEGAL, LEGAL),
            Hq = "Bölük Kışlası", Branch = "kışla", Units = new() { "legionary", "dragonguard" }, Enemies = new(), Rivals = new(),
            Ranks = new() { "Şampiyon", "Savaş Ustası", "Büyülü Kılıç" }, Inner = "Aksiyon Dalgası", Yearly = "Bölük Turnuvası",
            Goals = new() { "kervan koruma", "kuşatma", "karakol tutmak" }, MinTier = 2, Paths = new() { "mercenary" }, Classes = new() { "fighter" },
        },
        ["bards"] = new OrgDef
        {
            Id = "bards", Name = "Ozanlar Koleji", Source = "bard", Faith = null, Good = 0.4, Law = -0.2, Legal = L(LEGAL, LEGAL, LEGAL, LEGAL),
            Hq = "Ozanlar Salonu", Branch = "meyhane sahnesi", Units = new() { "skald" }, Enemies = new(), Rivals = new(),
            Ranks = new() { "Bilgi Okulu", "Yiğitlik Okulu", "Belagat Okulu" }, Inner = "Sihirli Ezgi", Yearly = "Ozan Şenliği",
            Goals = new() { "haber taşımak", "kayıp bir şarkıyı bulmak" }, MinTier = 1, Paths = new(), Classes = new(),
        },
        ["merchants"] = new OrgDef
        {
            Id = "merchants", Name = "Tüccarlar Loncası", Source = "yeni", Faith = null, Good = 0.1, Law = 0.5, Legal = L(LEGAL, LEGAL, PATRON, LEGAL),
            Hq = "Lonca Evi", Branch = "kontuar", Units = new(), Enemies = new() { "thieves" }, Rivals = new() { "circle" },
            Ranks = new() { "Çırak", "Kalfa", "Usta" }, Inner = "Konsey Sırrı", Yearly = "Büyük Panayır",
            Goals = new() { "kervan işleri", "kıtlık siparişi", "hamallık" }, MinTier = 1, Paths = new(), Classes = new(),
        },
        ["explorers"] = new OrgDef
        {
            Id = "explorers", Name = "Harabe Kâşifleri", Source = "yeni", Faith = null, Good = 0.1, Law = 0, Legal = L(LEGAL, LEGAL, LEGAL, LEGAL),
            Hq = "Kâşifler Locası", Branch = "harita odası", Units = new(), Enemies = new(), Rivals = new(),
            Ranks = new() { "Haritacı", "Mezar Avcısı", "Eser Ustası" }, Inner = "Kayıp Harita", Yearly = "Büyük Keşif",
            Goals = new() { "harabe keşfi", "eser kurtarma" }, MinTier = 1, Paths = new() { "wanderer" }, Classes = new(),
        },
        ["slavers"] = new OrgDef
        {
            Id = "slavers", Name = "Köle Avcıları", Source = "yeni", Faith = null, Good = -0.7, Law = 0, Legal = L(BANNED, LEGAL, LEGAL, BANNED),
            Hq = "Köle Pazarı", Branch = "kamp", Units = new(), Enemies = new() { "freedom" }, Rivals = new(),
            Ranks = new() { "Avcı", "Kafile Başı", "Pazar Ağası" }, Inner = "Zincir Defteri", Yearly = "Büyük Pazar",
            Goals = new() { "avlanmak", "köle taşımak" }, MinTier = 1, Paths = new(), Classes = new(),
        },
        ["freedom"] = new OrgDef
        {
            Id = "freedom", Name = "Özgürlük Ağı", Source = "yeni", Faith = null, Good = 0.7, Law = -0.4, Legal = L(LEGAL, BANNED, BANNED, LEGAL), Secret = true,
            Hq = "Gizli Sığınak", Branch = "sığınak", Units = new(), Enemies = new() { "slavers" }, Rivals = new(),
            Ranks = new() { "Yardımcı", "Kılavuz", "Kurtarıcı" }, Inner = "Kırık Zincir", Yearly = "Kaçış Gecesi",
            Goals = new() { "köle kurtarmak", "avcıları öldürmek" }, MinTier = 1, Paths = new(), Classes = new(),
        },
    };

    /// <summary>Spec: generic birimler (milis, asker, okçu, şövalye) + kültür birimleri; örgüt birlikleri (Paralı Asker…).
    /// data.json'daki birimlere eklenir (D.Init sonrası, bir kez).</summary>
    private static readonly List<UnitDef> NEW_UNITS = new()
    {
        new UnitDef { Id = "archer", Name = "Okçu", Hp = 9, Ac = 13, Atk = 4, Dmg = new() { 1, 8, 0 }, Per = 0.25 },
        new UnitDef { Id = "pikeman", Name = "Mızraklı", Hp = 12, Ac = 15, Atk = 3, Dmg = new() { 1, 8, 1 }, Per = 0.3 },
        new UnitDef { Id = "ironguard", Name = "Demir Muhafız", Hp = 14, Ac = 17, Atk = 3, Dmg = new() { 1, 8, 2 }, Per = 0.3 },
        new UnitDef { Id = "longbow", Name = "Uzun Yaycı", Hp = 9, Ac = 14, Atk = 6, Dmg = new() { 1, 8, 2 }, Per = 0.3 },
        new UnitDef { Id = "slinger", Name = "Sapancı", Hp = 8, Ac = 14, Atk = 4, Dmg = new() { 1, 4, 2 }, Per = 0.35 },
        new UnitDef { Id = "flamebreath", Name = "Alevnefes Muhafız", Hp = 14, Ac = 16, Atk = 4, Dmg = new() { 2, 6, 1 }, Per = 0.3 },
    };

    private static readonly object UNITS_LOCK = new();

    /// <summary>Yeni birimleri D.UNITS'e (aç haydutları D.MONSTERS'a) ekler; Lejyoner artık Paralı Bölüklerin birimi (adı genelleşir,
    /// oranı var). Bir kez çalışır (eklenmişse bir şey yapmaz; paralel dünyalar için kilitli).</summary>
    public static void InitUnits()
    {
        lock (UNITS_LOCK)
        {
            if (D.UNITS.Has("archer")) return;
            foreach (var u in NEW_UNITS) if (!D.UNITS.Has(u.Id)) D.UNITS.Set(u.Id, u);
            if (!D.MONSTERS.Has("bandit"))
            {
                D.MONSTERS.Set("bandit", new MonsterDef { Name = "Haydut", Hp = 9, Ac = 12, Atk = 3, Dmg = new() { 1, 6, 1 } });
                D.MONSTERS.Set("banditBoss", new MonsterDef { Name = "Haydut Başı", Hp = 16, Ac = 14, Atk = 4, Dmg = new() { 1, 8, 2 } });
            }
            var leg = D.UNITS["legionary"];
            leg.Name = "Lejyoner";
            leg.Per ??= 0.4;
        }
    }

    // ------------------------------------------------------------ devlet
    public static CultureDef Culture(Civ c) => CULTURES.GetOr(c.Race, null) ?? CULTURES["human"];
    public static CultureDef CultureOf(string race) => CULTURES.GetOr(race, null) ?? CULTURES["human"];
    public static GovDef Gov(Civ c) => GOVS.GetOr(c.Gov ?? "kingdom", null) ?? GOVS["kingdom"];

    /// <summary>Saldırganlık (eski ClassDef.Aggression; 0–1): tip + kültür + kötü yönetici.</summary>
    public static double Aggression(Civ c) => JsMath.Max(0, JsMath.Min(1, Gov(c).Aggression + Culture(c).Aggression + (c.Align.Good <= -0.3 ? 0.15 : 0)));

    /// <summary>Devletin kahraman sınıfı yakınlığı (eski ClassDef.HeroClass): tipten (teokraside rahip, boylarda barbar,
    /// cumhuriyette hırsız), krallıkta kültürden.</summary>
    public static string HeroClass(Civ c) => Gov(c).Hero ?? Culture(c).Hero;

    /// <summary>Ant bozmayan devlet (eski paladin yemini): kanunlu ve iyi yönetici, krallık ya da teokrasi.</summary>
    public static bool Oathbound(Civ c) => (c.Gov == "kingdom" || c.Gov == "theocracy") && c.Align.Law >= 0.5 && c.Align.Good >= 0.4;

    /// <summary>Kutsal devlet (eski paladin/rahip yakınlığı): resmî inancı Güneş, teokrasi ya da iyi yönetici; kötülüğe tahammül etmez.</summary>
    public static bool Holy(Civ c) => c.Law?.Faith == "sun" && (c.Gov == "theocracy" || c.Align.Good >= 0.4);

    /// <summary>Cömert (kıtlıkta yardım eder; eski rahip, paladin, druid, keşiş, ozan): teokrasi ya da iyi yönetici.</summary>
    public static bool Generous(Civ c) => c.Gov == "theocracy" || c.Align.Good >= 0.5;

    /// <summary>Kaçakçılara göz yuman devlet (eski Haydut): cumhuriyet (korsanlar ganimeti onların limanlarında satar).</summary>
    public static bool Smugglers(Civ c) => c.Gov == "republic";

    /// <summary>Eski İnanç'ın yerleşimi (eski Druid): Eski İnanç payı en az %50. Orman kesilmez (koru), kutsal koru kurulur, kadim
    /// ormana yerleşilir, ormanın içindeki yerleşim savunmada +2 AC.</summary>
    public static bool OldWays(Settlement st) => st?.Faith != null && (st.Faith.Get("old") ?? 0) >= 0.5;

    /// <summary>Medeniyetin yerleşimlerinin çoğu Eski İnanç'ta mı (devlet çapındaki kararlar: kadim orman, kutsal koru).</summary>
    public static bool OldWaysCiv(Sim s, Civ c)
    {
        double old = 0, all = 0;
        foreach (var st in s.CivSettlements(c)) { double p = s.Pop(st); all += p; old += p * (st.Faith?.Get("old") ?? 0); }
        return all > 0 && old / all >= 0.5;
    }

    public static string StateName(string stem, string gov) => GOVS[gov].Pattern.Replace("{0}", stem);

    /// <summary>Başkentin özel yapısının adı (eski sınıf yapısı).</summary>
    public static string SeatName(Civ c) => Gov(c).Seat;

    /// <summary>Hükümet tipinin varsayılan yasası (resmî inanç, hoşgörü; teokraside tiefling şüpheli).</summary>
    public static LawState DefaultLaw(string gov)
    {
        var g = GOVS[gov];
        var l = new LawState
        {
            Harsh = g.Harsh, Slavery = g.Slavery, Faith = g.Faith, FaithTol = g.FaithTol.Clone(), Smuggle = g.Smuggle, Bribe = g.Bribe, Patrol = g.Patrol, PrisonMine = g.PrisonMine,
        };
        if (gov == "theocracy") l.RaceTol.Set("tiefling", 0.2);
        else if (gov == "kingdom") l.RaceTol.Set("tiefling", 0.6);
        return l;
    }

    /// <summary>Örgütün devletteki yasal durumu: patron / legal / banned (tanım + devletin yasası: resmî inancı sert olan devlet
    /// başka inancın örgütünü yasaklar, kölelik yasağı köle avcılarını, köleliğin serbestliği Özgürlük Ağı'nı).</summary>
    public static string Legal(Civ c, string org)
    {
        var d = ORGS[org];
        string v = d.Legal.GetOr(c.Gov ?? "kingdom", null) ?? LEGAL;
        var law = c.Law;
        if (law == null) return v;
        if (d.Faith != null && v != PATRON && (law.FaithTol.Get(d.Faith) ?? 1) <= 0.25) v = BANNED;
        if (org == "slavers") v = law.Slavery ? (v == BANNED ? LEGAL : v) : BANNED;
        if (org == "freedom") v = law.Slavery ? BANNED : (v == BANNED ? LEGAL : v);
        return v;
    }
}
