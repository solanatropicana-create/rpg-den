using System;
using System.Collections.Generic;
using System.Linq;

// Yaşayan han: hancı kafilesi (Settlers gibi yerleşimden çıkıp yer seçer), inşaat, misafirler, kiler,
// erzak arabası (medeniyetten gerçek alım), personel, ün, genişleme ve 30 günlük kasa defteri.
// Port of src/sim/innlife.ts.

namespace FD.Macro;

/// <summary>TS <c>type Want</c> (innlife.ts): erzak arabasından istenen miktarlar; hepsi isteğe bağlı (undefined = null).</summary>
public sealed class Want
{
    public double? Food;
    public double? Ale;
    public double? Wood;
    public double? Stone;
}

public static class InnLife
{
    private static readonly Dictionary<string, string> HERO_WORK = new()
    {
        ["ranger"] = "avlanıp mutfağa et getiriyor", ["barbarian"] = "avlanıp mutfağa et getiriyor", ["druid"] = "ormandan ot ve mantar topluyor",
        ["fighter"] = "avluda nöbet tutuyor", ["paladin"] = "avluda nöbet tutuyor", ["cleric"] = "yolcuların yaralarını sarıyor",
        ["wizard"] = "ocak başında hikâye anlatıyor", ["rogue"] = "zar masasında şansını deniyor",
    };
    private static readonly List<string> RACE_IDS = new() { "human", "dwarf", "elf", "halfling", "gnome", "halfelf", "halforc", "dragonborn", "tiefling" };

    /// <summary>JS template of a possibly-undefined record read (<c>`${REC[k]}`</c> prints "undefined").</summary>
    private static string U(string x) => x ?? "undefined";
    private static string HeroWork(string cls) => cls != null && HERO_WORK.TryGetValue(cls, out var w) ? w : "undefined";
    private static string ClassTr(string cls) => U(cls != null ? D.HERO_CLASS_TR[cls] : null);

    /// <summary>"{name} Hanı"</summary>
    public static string InnTitle(Inn inn) => $"{inn.Name} Hanı";

    /// <summary>Kademe adı: INN_LEVEL[max(1, level)].name.</summary>
    public static string LevelName(Inn inn) => D.INN_LEVEL[Math.Max(1, inn.Level)].Name;

    /// <summary>fiyatlar gümüş (10 gümüş = 1 altın)</summary>
    public static (double Room, double Meal, double Ale) InnPrices(Inn inn)
    {
        var L = D.INN_LEVEL[Math.Max(1, inn.Level)];
        return (L.Room + (inn.Fame >= 70 ? 1 : 0), L.Meal, L.AleP);
    }

    /// <summary>Doluluk: odada yatan, ahırda yatan ve toplam kişi sayısı.</summary>
    public static (double Rooms, double Stable, double Persons) InnOcc(Inn inn)
    {
        double rooms = 0, stable = 0;
        foreach (var g in inn.Guests) if (J.T(g.Stable)) stable += g.N; else rooms += g.N;
        return (rooms, stable, rooms + stable);
    }

    /// <summary>Servis kapasitesi: 3 + personelin STAFF[role].serve toplamı.</summary>
    public static double ServeCap(Inn inn) => 3 + J.Reduce(inn.Staff, (a, x) => a + D.STAFF[x.Role].Serve, 0.0);

    /// <summary>Personelin ücret toplamı (STAFF[role].wage; her 30 günde ödenir).</summary>
    public static double Wages(Inn inn) => J.Reduce(inn.Staff, (a, x) => a + D.STAFF[x.Role].Wage, 0.0);

    /// <summary>İnşaat / genişletme aşamasının adı ("" inşaat yoksa).</summary>
    public static string BuildStageName(Inn inn)
    {
        var b = inn.Build;
        if (b == null) return "";
        double p = b.Work / b.Need;
        if (b.Level > 1 && !J.T(b.Rebuild) && inn.Stage == "open") return p < 0.3 ? "Yeni kanadın temeli" : p < 0.7 ? "Kanadın duvarları" : "Kanadın çatısı";
        return p < 0.2 ? "Temel kazılıyor" : p < 0.5 ? "İskelet çatılıyor" : p < 0.8 ? "Duvarlar örülüyor" : "Çatı kapatılıyor";
    }

    /// <summary>İnşaat ilerlemesi 0..1 (iş %80 + malzeme %20); inşaat yoksa 0.</summary>
    public static double BuildPct(Inn inn)
    {
        var b = inn.Build;
        if (b == null) return 0;
        double mat = JsMath.Min(J.T(b.WoodNeed) ? b.Wood / b.WoodNeed : 1, J.T(b.StoneNeed) ? b.Stone / b.StoneNeed : 1);
        return JsMath.Max(0, JsMath.Min(1, (b.Work / b.Need) * 0.8 + JsMath.Min(1, mat) * 0.2));
    }

    private static void AddLog(Sim s, Inn inn, string k, string t, double? g = null)
    {
        inn.Log.Add(new InnLog { Day = s.Day, K = k, T = t, G = g });
        if (inn.Log.Count > 90) J.Splice(inn.Log, 0, inn.Log.Count - 90);
    }

    /// <summary>hanın hesap dönemi (gün)</summary>
    public const double BOOK_DAYS = 30;

    private static InnBook Book(Sim s, Inn inn)
    {
        double period = Math.Floor(s.Day / BOOK_DAYS);
        var b = J.Last(inn.Books);
        if (b == null || b.Period != period)
        {
            b = new InnBook { Period = period, Room = 0, Food = 0, Ale = 0, Other = 0, Supply = 0, Wage = 0, Build = 0, Guests = 0, Nights = 0 };
            inn.Books.Add(b);
            if (inn.Books.Count > 8) J.Shift(inn.Books);
        }
        return b;
    }

    /// <summary>hana başka yerden giren para (açık artırma payı, ilan iadesi...)</summary>
    public static void InnIncome(Sim s, Inn inn, double gold, string why)
    {
        inn.Gold += gold;
        Book(s, inn).Other += gold;
        inn.Total.Income += gold;
        AddLog(s, inn, "ev", why, gold);
    }

    /// <summary>Hanın defterine bir olay satırı ('ev') yazar.</summary>
    public static void InnEvent(Sim s, Inn inn, string text) => AddLog(s, inn, "ev", text);

    private static string PersonName(Sim s, string race) => s.Rng.Pick((race != null ? D.HERO_NAMES[race] : null) ?? D.HERO_NAMES["human"]);

    private static string RaceFrom(Sim s, Settlement st)
    {
        if (st == null) return s.Rng.Pick(RACE_IDS);
        var rs = J.Filter(st.Pop.Keys(), r => (st.Pop.Get(r) ?? 0) > 0);
        return s.Rng.Chance(0.85) && rs.Count > 0 ? s.Rng.Weighted(rs, r => st.Pop.Get(r) ?? 0) : s.Rng.Pick(RACE_IDS);
    }

    private static List<(Settlement X, int D)> NearestSettlements(Sim s, int tile, int n = 2)
    {
        var l = new List<(Settlement X, int D)>();
        foreach (var x in s.W.Settlements) if (x.Alive) l.Add((x, s.G.Dist(x.Tile, tile)));
        J.Sort(l, (a, b) => J.Or((double)(a.D - b.D), a.X.Id - b.X.Id));
        return J.Slice(l, 0, n);
    }

    /// <summary>"Tunçkale ile Lirköy arasına" (iki yerleşim arası / "X'in yakını" / "ıssız bir yol ağzı")</summary>
    public static string WhereStr(Sim s, int tile)
    {
        var nn = NearestSettlements(s, tile, 2);
        if (nn.Count >= 2) return $"{nn[0].X.Name} ile {nn[1].X.Name} arası";
        return nn.Count > 0 ? $"{Tr.Ek(nn[0].X.Name, "in")} yakını" : "ıssız bir yol ağzı";
    }

    // ------------------------------------------------------------ yer seçimi
    private static bool ForestNear(Sim s, int tile) => J.Some(s.G.Within(tile, 2), t => (s.W.Tiles[t].Terrain == "forest" || s.W.Tiles[t].Terrain == "oldforest") && s.W.Tiles[t].Wood > 5);
    private static bool StoneNear(Sim s, int tile) => J.Some(s.G.Within(tile, 3), t => s.W.Tiles[t].Terrain == "hill" || s.W.Tiles[t].Terrain == "mountain");

    private static bool SiteOk(Sim s, int i, Inn self = null)
    {
        var w = s.W; var t = w.Tiles[i];
        if (J.T(t.Sea) || J.T(t.Isle) || !(t.Terrain == "grass" || t.Terrain == "tundra" || t.Terrain == "hill")) return false;
        if (t.Owner >= 0 || t.Deposit >= 0 || t.Camp != null || t.Ext != null) return false;
        if (t.InnZone != null && t.InnZone != self?.Id) return false;
        int c = s.G.Col(i), r = s.G.Row(i);
        if (c < 3 || r < 3 || c > w.Width - 4 || r > w.Height - 4) return false;
        if (J.Some(w.Settlements, x => x.Alive && s.G.Dist(x.Tile, i) < 5)) return false;
        if (J.Some(w.Inns, x => x != self && s.G.Dist(x.Tile, i) < 10)) return false;
        if (J.Some(w.Camps, cp => cp.Alive && s.G.Dist(cp.Tile, i) < 4)) return false;
        return true;
    }

    private static double? SiteScore(Sim s, int i)
    {
        if (!SiteOk(s, i)) return null;
        var w = s.W; var t = w.Tiles[i];
        var ds = new List<double>();
        foreach (var x in w.Settlements) if (x.Alive) ds.Add(s.G.Dist(x.Tile, i));
        J.Sort(ds, (a, b) => a - b);
        if (ds.Count < 2 || ds[0] < 6 || ds[0] > 17 || ds[1] - ds[0] > 9) return null;
        double sc = -((ds[0] - 9) * (ds[0] - 9)) * 0.05 - (ds[1] - ds[0]) * 0.22;
        if (ds.Count > 2 && ds[2] <= 18) sc += 0.7;               // üç yolun kavşağı
        if (t.Terrain == "grass") sc += 1; else if (t.Terrain == "hill") sc -= 0.4;
        if (J.T(t.Road)) sc += 1.5;
        if (J.Some(s.G.Neighbors(i), n => w.Tiles[n].Terrain == "water" && !J.T(w.Tiles[n].Sea))) sc += 0.6;   // göl, dere
        if (ForestNear(s, i)) sc += 0.7;
        if (StoneNear(s, i)) sc += 0.3;
        foreach (var cp in w.Camps) if (cp.Alive) { int d = s.G.Dist(cp.Tile, i); if (d <= 6) sc -= 1; else if (d <= 11) sc += 0.4; }   // maceracı gelir ama kapı dibinde goblin istemez
        return sc;
    }

    /// <summary>göreceli rastgele: en iyi yirmi yerden biri, puanla ağırlıklı</summary>
    private static int PickSite(Sim s)
    {
        var cands = new List<(int I, double Sc)>();
        for (int i = 0; i < s.W.Tiles.Count; i++) { var sc = SiteScore(s, i); if (sc != null) cands.Add((i, sc.Value)); }
        if (cands.Count == 0) return -1;
        J.Sort(cands, (a, b) => J.Or(b.Sc - a.Sc, a.I - b.I));
        var top = J.Slice(cands, 0, 20); double best = top[0].Sc;
        return s.Rng.Weighted(top, c => JsMath.Exp((c.Sc - best) * 0.9)).I;
    }

    // ------------------------------------------------------------ hancı kafilesi
    private static Inn NewInn(Sim s, int tile, string keeper, string race, int originId, string originName)
    {
        var used = new HashSet<string>(J.Map(s.W.Inns, x => x.Name));
        var free = J.Filter(D.INN_NAMES, n => !used.Contains(n));
        string name = free.Count > 0 ? s.Rng.Pick(free) : $"{s.Rng.Pick(D.INN_NAMES)} {J.S(s.W.Inns.Count + 1)}";
        return new Inn
        {
            Id = s.Id(), Tile = tile, Name = name, Keeper = keeper, Founded = s.Day, Alive = false, Gold = 80, Raids = 0,
            Stage = "road", Level = 0, KeeperRace = race, Origin = originId, OriginName = originName,
            Stock = new InnStock { Food = 24, Ale = 10, Wood = 16 }, Fame = 12, Staff = new List<InnStaff>(), Guests = new List<Guest>(), Tabs = new JsNumObj<double>(), Log = new List<InnLog>(), Books = new List<InnBook>(), Hist = new List<InnHist>(),
            Total = new InnTotal { Guests = 0, Nights = 0, Income = 0, Turned = 0, Bought = 0 }, Turned = 0, Sat = 0.7, Traffic = 0,
        };
    }

    /// <summary>hancı yola çıkar: yeni han, harabeyi yeniden kurma ya da ustasının yanından ayrılan çırak (TS options object <c>o</c> → optional named params).</summary>
    public static bool DispatchKeeper(Sim s, Inn rebuild = null, Inn fromInn = null, string name = null, string race = null)
    {
        var w = s.W;
        int site = rebuild != null ? rebuild.Tile : PickSite(s);
        if (site < 0) return false;
        int fromTile; int originId; string originName; Settlement homeSt;
        if (fromInn != null) { fromTile = fromInn.Tile; originId = fromInn.Id; originName = InnTitle(fromInn); homeSt = J.At(NearestSettlements(s, fromInn.Tile, 1), 0).X; }
        else
        {
            var near = NearestSettlements(s, site, 3);
            if (near.Count == 0) return false;
            homeSt = s.Rng.Weighted(near, x => 1 / (1 + JsMath.Max(0, x.D - near[0].D))).X;
            fromTile = homeSt.Tile; originId = homeSt.Id; originName = homeSt.Name;
        }
        var path = s.Path(fromTile, site);
        if (path == null || path.Count < 2) return false;
        string krace = race ?? (homeSt != null && s.Rng.Chance(0.75) ? w.Civs[homeSt.Civ].Race : s.Rng.Pick(RACE_IDS));
        string keeper = name ?? (s.Rng.Chance(0.5) ? s.Rng.Pick(D.KEEPER_NAMES) : PersonName(s, krace));
        Inn inn;
        if (rebuild != null)
        {
            inn = rebuild;
            // Object.assign(inn, { stage, keeper, keeperRace, origin, originName, gold, stock, staff, guests, fame, order: undefined })
            double gold = JsMath.Max(inn.Gold, 50), fame = JsMath.Max(8, inn.Fame * 0.5);
            inn.Stage = "road"; inn.Keeper = keeper; inn.KeeperRace = krace; inn.Origin = originId; inn.OriginName = originName; inn.Gold = gold;
            inn.Stock = new InnStock { Food = 24, Ale = 10, Wood = 16 }; inn.Staff = new List<InnStaff>(); inn.Guests = new List<Guest>(); inn.Fame = fame; inn.Order = null;
        }
        else
        {
            inn = NewInn(s, site, keeper, krace, originId, originName);
            w.Inns.Add(inn);
        }
        // yanında bir çırak (ve bazen bir yardımcı) getirir
        string r0 = RaceFrom(s, homeSt);
        inn.Staff.Add(new InnStaff { Name = PersonName(s, r0), Race = r0, Role = "cirak", Since = s.Day, From = originName });
        if (s.Rng.Chance(0.5)) { string r1 = RaceFrom(s, homeSt); inn.Staff.Add(new InnStaff { Name = PersonName(s, r1), Race = r1, Role = "seyis", Since = s.Day, From = originName }); }
        w.Agents.Add(new Agent { Id = s.Id(), Kind = "keeper", Civ = -1, Path = path, Step = 0, Progress = 0, Speed = 0.55, From = originId, To = inn.Id, Inn = inn.Id, TargetTile = site, Purpose = "found" });
        string where = WhereStr(s, site);
        AddLog(s, inn, "build", $"Hancı {keeper} {Tr.Ek(originName, "dan")} öküz arabasıyla yola çıktı");
        s.Metric("innKeeper");
        string cause = rebuild != null ? $"{InnTitle(inn)} yıkılalı {J.S(Math.Floor((s.Day - (inn.RuinedDay ?? s.Day)) / Sim.YEAR))} yıl oldu; çevre temizlendi"
            : fromInn != null ? $"{fromInn.Keeper} ustasının yanında yetişti" : "Yollar kalabalıklaşacak; yorgun yolcuya bir çatı, atına bir ahır gerek";
        s.Log("inn", rebuild != null ? $"Hancı {keeper}, {InnTitle(inn)} harabesini yeniden kurmak için {Tr.Ek(originName, "dan")} yola çıktı."
            : fromInn != null ? $"Çırak {keeper}, {Tr.Ek(InnTitle(fromInn), "dan")} ayrıldı: {where}nda kendi hanını kuracak."
                : $"Hancı {keeper}, {Tr.Ek(originName, "dan")} öküz arabasıyla yola çıktı: {where}nda bir han kuracak.",
            tile: fromTile, major: true, cause: cause);
        return true;
    }

    /// <summary>Hancı kafilesi vardı: yer uygunsa temel atar, kapılmışsa yakında yer arar ya da geri döner (true = ajan biter).</summary>
    public static bool KeeperArrive(Sim s, Agent a)
    {
        var inn = Will.InnById(s, a.Inn);
        if (inn == null || inn.Stage != "road") return true;
        int here = a.Path[Math.Min(a.Step, a.Path.Count - 1)];
        bool rebuild = inn.RuinedDay != null;
        if (!rebuild && !SiteOk(s, here, inn))
        {
            // yol boyunca yer kapılmış: çevrede başka düz bir yer ara
            var alts = J.Sort(J.Filter(s.G.Within(here, 4), i => SiteOk(s, i, inn)), (x, y) => J.Or((double)(s.G.Dist(x, here) - s.G.Dist(y, here)), x - y));
            int? alt = alts.Count > 0 ? alts[0] : null;
            var path = alt != null ? s.Path(here, alt.Value) : null;
            if (alt == null || path == null)
            {
                s.W.Inns = J.Filter(s.W.Inns, x => x != inn);
                s.Log("inn", $"Hancı {inn.Keeper} kuracak yer bulamadı; arabasını çevirip {Tr.Ek(inn.OriginName, "a")} döndü.", tile: here, cause: "Seçtiği toprak başkasına geçmiş");
                return true;
            }
            a.Path = path; a.Step = 0; a.Progress = 0; a.TargetTile = alt; inn.Tile = alt.Value;
            return false;
        }
        StartBuild(s, inn, rebuild ? inn.Tile : here);
        return true;
    }

    private static void StartBuild(Sim s, Inn inn, int tile)
    {
        inn.Tile = tile;
        inn.Stage = "build";
        WorldGen.MarkInn(s.G, s.W.Tiles, inn);
        bool re = inn.RuinedDay != null;
        inn.Build = new InnBuild { Level = 1, Work = 0, Need = re ? 36 : 48, Wood = inn.Stock.Wood, WoodNeed = re ? 22 : 30, Stone = 0, StoneNeed = re ? 0 : 8, Started = s.Day, Rebuild = re };
        inn.Stock.Wood = 0;
        AddLog(s, inn, "build", re ? "Harabe temizlendi, eski temel üstüne kuruluyor" : "Temel atıldı");
        s.Log("inn", $"Hancı {inn.Keeper}, {WhereStr(s, tile)}nda {InnTitle(inn)} için temel attı.", tile: tile, cause: $"Yanında {J.S(inn.Staff.Count)} yardımcı; arabada {J.S(JsMath.Round(inn.Build.Wood))} kereste");
    }

    private static void FinishBuild(Sim s, Inn inn)
    {
        var b = inn.Build;
        inn.Build = null;
        if (inn.Stage == "build")
        {
            inn.Stage = "open"; inn.Alive = true; inn.Level = 1; inn.Founded = s.Day; inn.RuinedDay = null;
            inn.Fame = JsMath.Max(inn.Fame, 15);
            s.Metric("innOpened");
            double n = J.N(s.W.Metrics.Get("innOpened"));
            if (n <= 2 && !J.T(s.W.Metrics.Get("innOpenDay" + J.S(n)))) s.W.Metrics.Set("innOpenDay" + J.S(n), s.Day);
            AddLog(s, inn, "build", "Kapılar açıldı; ocak yandı, ilk fıçı açıldı");
            s.Log("inn", $"{InnTitle(inn)} kapılarını açtı! Hancı {inn.Keeper} ocağı yaktı, ilk fıçıyı açtı.", tile: inn.Tile, major: true, cause: $"{J.S(s.Day - b.Started)} günlük inşaat · {J.S(D.INN_LEVEL[1].Rooms)} oda · {WhereStr(s, inn.Tile)}");
        }
        else
        {
            inn.Level = b.Level;
            s.Metric("innUpgrade");
            AddLog(s, inn, "build", $"Genişletme bitti: artık bir {LevelName(inn)} ({J.S(D.INN_LEVEL[inn.Level].Rooms)} oda)");
            s.Log("inn", $"{InnTitle(inn)} genişledi: artık bir {J.TrLower(LevelName(inn))}, {J.S(D.INN_LEVEL[inn.Level].Rooms)} odalı.", tile: inn.Tile, major: true, cause: $"{J.S(s.Day - b.Started)} günlük inşaat · ün {J.S(JsMath.Round(inn.Fame))}");
        }
        EnsureStaff(s, inn);
    }

    /// <summary>Faz 1b-3: mevsim yok; inşaat hızı eski kış ×0,6'nın yıllık ortalaması</summary>
    public const double BUILD_PACE = 0.9;

    private static void BuildTick(Sim s, Inn inn)
    {
        var b = inn.Build;
        double pace = BUILD_PACE;
        // gündelikçi: yakın köylerden, günlüğü 0.2 altın
        int hired = inn.Gold > 30 ? (inn.Gold > 90 ? 2 : 1) : 0;
        if (J.T(hired)) { double pay = hired * 0.2; inn.Gold -= pay; Book(s, inn).Build += pay; }
        int hands = 1 + inn.Staff.Count + hired;
        bool forest = ForestNear(s, inn.Tile), quarry = StoneNear(s, inn.Tile);
        if (b.Wood < b.WoodNeed && forest) { int k = Math.Min(hands - 1, 2); if (k > 0) { b.Wood += 0.6 * k * pace; hands -= k; } }
        if (b.Stone < b.StoneNeed && quarry) { int k = Math.Min(hands - 1, 1); if (k > 0) { b.Stone += 0.4 * k * pace; hands -= k; } }
        if (inn.Order == null && ((b.Wood < b.WoodNeed && !forest) || (b.Stone < b.StoneNeed && !quarry)))
        {
            bool ok = PlaceOrder(s, inn, new Want { Wood = forest ? 0 : Math.Ceiling(b.WoodNeed - b.Wood), Stone = quarry ? 0 : Math.Ceiling(b.StoneNeed - b.Stone) });
            // taş hiçbir yerden gelmiyorsa ahşap temelle yetin
            if (!ok && b.Stone < b.StoneNeed && !quarry && s.Day - b.Started > 60) { b.WoodNeed += Math.Ceiling((b.StoneNeed - b.Stone) * 0.8); b.StoneNeed = Math.Floor(b.Stone); AddLog(s, inn, "build", "Taş bulunamadı; temel ahşapla güçlendirildi"); }
        }
        double mat = JsMath.Min(J.T(b.WoodNeed) ? b.Wood / b.WoodNeed : 1, J.T(b.StoneNeed) ? b.Stone / b.StoneNeed : 1);
        double cap = b.Need * JsMath.Min(1, mat + 0.15);
        if (b.Work < cap) b.Work = JsMath.Min(cap, b.Work + (hands * 0.55 + 0.3) * pace);
        if (b.Work >= b.Need && mat >= 1) FinishBuild(s, inn);
    }

    // ------------------------------------------------------------ erzak arabası: medeniyetten gerçek alım
    private static readonly List<string> QUOTE_FOODS = new() { "grain", "fish", "meat", "bread" };

    /// <summary>bir yerleşimden ne alınabilir: satıcı kendi ambar payını (20 günlük gıda) ve her malın %30'undan fazlasını vermez</summary>
    private static (JsObj<double> Cargo, double Cost, double Fill) Quote(Sim s, Inn inn, Settlement st, Want want)
    {
        var c = s.W.Civs[st.Civ];
        var cargo = new JsObj<double>();
        double reserve = s.CivPop(c) * Sim.FOOD_PER_POP * 40 + 15;   // kendi ambar payına (40 gün) dokunmaz
        double surplus = s.FoodTotal(c) - reserve;
        double fNeed = want.Food ?? 0, aNeed = want.Ale ?? 0, got = 0;
        double Take(string g, double units)
        {
            double q = JsMath.Max(0, JsMath.Min(Math.Ceiling(units), Math.Floor(s.St(c, g) * 0.2) - (cargo.Get(g) ?? 0)));
            if (q > 0) cargo.Set(g, (cargo.Get(g) ?? 0) + q);
            return q;
        }
        if (aNeed > 0 && s.St(c, "beer") >= 4) { double q = Take("beer", aNeed / D.BEER_ALE); aNeed -= q * D.BEER_ALE; got += q * D.BEER_ALE; }
        foreach (var g in QUOTE_FOODS)
        {
            if (fNeed <= 0 || surplus <= 0) break;
            double per = g == "bread" ? 24 : D.GRAIN_FOOD;
            double q = Take(g, JsMath.Min(fNeed / per, surplus / (D.GOODS[g].Food ?? 1)));
            fNeed -= q * per; surplus -= q * (D.GOODS[g].Food ?? 1); got += q * per;
        }
        if (aNeed > 0 && surplus > 0) { double q = Take("grain", JsMath.Min(aNeed / D.GRAIN_ALE, surplus)); got += q * D.GRAIN_ALE; }
        if (J.T(want.Wood)) got += Take("wood", want.Wood.Value) * 6;
        if (J.T(want.Stone)) got += Take("stone", want.Stone.Value) * 6;
        double cost = 0;
        foreach (var kv in cargo) cost += kv.Value * s.Price(c, kv.Key) * 1.15;
        double budget = inn.Gold - 5;
        if (cost > budget && cost > 0)
        {
            double k = JsMath.Max(0, budget / cost);
            got *= k;
            // for-in over cargo: every key present at the start is visited once; only the current key is deleted
            foreach (var g in cargo.Keys()) { cargo.Set(g, Math.Floor((cargo.Get(g) ?? 0) * k)); if (!J.T(cargo.Get(g))) cargo.Delete(g); }
            cost = 0;
            foreach (var kv in cargo) cost += kv.Value * s.Price(c, kv.Key) * 1.15;
        }
        double need = (want.Food ?? 0) + (want.Ale ?? 0) + (want.Wood ?? 0) * 6 + (want.Stone ?? 0) * 6;
        return (cargo, JsMath.Round(cost * 10) / 10, need > 0 ? got / need : 0);
    }

    private static bool PlaceOrder(Sim s, Inn inn, Want want)
    {
        var w = s.W;
        if (inn.Order != null || inn.Gold < 6) return false;
        var cands = J.Slice(J.Sort(J.Filter(w.Settlements, x => x.Alive && s.Pop(x) >= 8 && x.Starving <= 0 && s.G.Dist(x.Tile, inn.Tile) <= 24 && (w.Civs[x.Civ].InnBanUntil ?? 0) <= s.Day),
            (a, b) => J.Or((double)(s.G.Dist(a.Tile, inn.Tile) - s.G.Dist(b.Tile, inn.Tile)), a.Id - b.Id)), 0, 6);
        Settlement bestSt = null; (JsObj<double> Cargo, double Cost, double Fill) bestQ = default; double bestSc = 0;
        foreach (var st in cands)
        {
            var q = Quote(s, inn, st, want);
            if (q.Cost <= 0) continue;
            double unit = q.Cost / JsMath.Max(1, q.Fill * ((want.Food ?? 0) + (want.Ale ?? 0) + ((want.Wood ?? 0) + (want.Stone ?? 0)) * 6));
            if (unit > 0.35 && inn.Build == null && inn.Stock.Food > D.INN_LEVEL[Math.Max(1, inn.Level)].Food * 0.12) continue;   // karaborsa fiyatına almaz
            double sc = JsMath.Min(1, q.Fill) - s.G.Dist(st.Tile, inn.Tile) * 0.015 - unit * 3;
            if (bestSt == null || sc > bestSc) { bestSt = st; bestQ = q; bestSc = sc; }
        }
        var L = D.INN_LEVEL[Math.Max(1, inn.Level)];
        bool critical = inn.Stock.Food < L.Food * 0.12 || inn.Build != null;
        if (bestSt == null || (bestQ.Fill < 0.25 && !critical)) return false;
        var stB = bestSt; var cargo = bestQ.Cargo; double cost = bestQ.Cost;
        var path = s.Path(stB.Tile, inn.Tile);
        if (path == null) return false;
        var c = w.Civs[stB.Civ];
        foreach (var kv in cargo) s.Add(c, kv.Key, -kv.Value);
        s.Add(c, "gold", cost);
        inn.Gold -= cost;
        var bk = Book(s, inn);
        if (J.T(want.Wood) || J.T(want.Stone)) bk.Build += cost; else bk.Supply += cost;
        inn.Total.Bought += cost;
        string desc = string.Join(", ", J.Map(cargo.Entries(), kv => $"{J.S(kv.Value)} {J.TrLower(D.GOODS[kv.Key].Name)}"));
        int id = s.Id();
        w.Agents.Add(new Agent { Id = id, Kind = "supply", Civ = c.Id, Path = path, Step = 0, Progress = 0, Speed = 0.7, Cargo = cargo, From = stB.Id, To = inn.Id, Inn = inn.Id, Purpose = "supply" });
        inn.Order = new InnOrder { Agent = id, FromName = stB.Name, Goods = desc, Cost = cost, Day = s.Day };
        AddLog(s, inn, "buy", $"{Tr.Ek(stB.Name, "dan")} {desc} ısmarlandı ({c.Name})", -cost);
        s.Metric("innBuy");
        return true;
    }

    /// <summary>kervan başı yolunu hanın önünden geçirir (az sapma ise); sapma yoksa aynı liste örneğini döndürür</summary>
    public static List<int> InnDetour(Sim s, List<int> path)
    {
        if (path.Count < 6) return path;
        int a = path[0], b = path[path.Count - 1];
        List<int> best = null;
        var inns = s.W.Inns;
        for (int ii = 0; ii < inns.Count; ii++)
        {
            var inn = inns[ii];
            if (!inn.Alive) continue;
            bool near = false;
            for (int k = 0; k < path.Count; k++) if (k > 1 && k < path.Count - 2 && s.G.Dist(path[k], inn.Tile) <= 4) { near = true; break; }
            if (!near) continue;
            var p1 = s.Path(a, inn.Tile); var p2 = s.Path(inn.Tile, b);
            if (p1 == null || p2 == null) continue;
            var p = new List<int>(p1); p.AddRange(J.Slice(p2, 1));
            if (p.Count <= path.Count * 1.3 + 2 && (best == null || p.Count < best.Count)) best = p;
        }
        return best ?? path;
    }

    /// <summary>Erzak arabası hana vardı: kiler ya da inşaat malzemesi dolar (true = ajan biter).</summary>
    public static bool SupplyArrive(Sim s, Agent a)
    {
        var inn = Will.InnById(s, a.Inn);
        if (inn == null) return true;
        if (inn.Order != null && inn.Order.Agent == a.Id) inn.Order = null;
        if (inn.Stage == "ruin" || inn.Stage == "road") return true;
        var L = D.INN_LEVEL[Math.Max(1, inn.Level)];
        double food = 0, ale = 0, wood = 0, stone = 0;
        foreach (var kv in (a.Cargo ?? new JsObj<double>()).Entries())
        {
            string g = kv.Key; double q = kv.Value;
            if (g == "grain") { double toFood = JsMath.Min(q, Math.Ceiling(JsMath.Max(0, L.Food - inn.Stock.Food - food) / D.GRAIN_FOOD)); food += toFood * D.GRAIN_FOOD; ale += (q - toFood) * D.GRAIN_ALE; }
            else if (g == "fish" || g == "meat") food += q * D.GRAIN_FOOD;
            else if (g == "bread") food += q * 24;
            else if (g == "beer") ale += q * D.BEER_ALE;
            else if (g == "wood") wood += q;
            else if (g == "stone") stone += q;
        }
        inn.Stock.Food = JsMath.Min(L.Food * 1.2, inn.Stock.Food + food);
        inn.Stock.Ale = JsMath.Min(L.Ale * 1.2, inn.Stock.Ale + ale);
        if (inn.Build != null) { inn.Build.Wood += wood; inn.Build.Stone += stone; } else inn.Stock.Wood = JsMath.Min(L.Wood * 1.5, inn.Stock.Wood + wood);
        string from = s.Settlement(a.From ?? -1)?.Name ?? "pazar";
        var parts = J.Filter(new List<string> { J.T(food) ? $"+{J.S(JsMath.Round(food))} porsiyon" : "", J.T(ale) ? $"+{J.S(JsMath.Round(ale))} bardak" : "", J.T(wood) ? $"+{J.S(wood)} kereste" : "", J.T(stone) ? $"+{J.S(stone)} taş" : "" }, x => J.T(x));
        AddLog(s, inn, "buy", $"{Tr.Ek(from, "dan")} araba geldi: {J.Or(string.Join(", ", parts), "boş")}");
        return true;
    }

    // ------------------------------------------------------------ yolcular
    private static void ComputeTraffic(Sim s, Inn inn)
    {
        double T = 0;
        foreach (var st in s.W.Settlements) { if (!st.Alive) continue; int d = s.G.Dist(st.Tile, inn.Tile); if (d <= 18) T += s.Pop(st) / (1 + d / 5.0); }
        foreach (var r in s.W.Routes)
        {
            if (!r.Alive) continue;
            bool near = false;
            for (int k = 0; k < r.Path.Count; k++) if (k % 2 == 0 && s.G.Dist(r.Path[k], inn.Tile) <= 2) { near = true; break; }
            if (near) T += 12;
        }
        inn.Traffic = T;
    }

    /// <summary>Faz 1b-3: mevsim yok; yolcu akışı eski mevsim çarpanlarının yıllık ortalaması ((1 + 1,25 + 1,05 + 0,55) / 4)</summary>
    public const double ARRIVAL_AVG = 0.9625;

    private static double ArrivalRate(Sim s, Inn inn)
    {
        double season = ARRIVAL_AVG;
        double danger = 1;
        foreach (var cp in s.W.Camps) if (cp.Alive && s.G.Dist(cp.Tile, inn.Tile) <= 7) danger *= 0.75;
        double pantry = inn.Stock.Food < 5 ? 0.6 : 1;
        return JsMath.Min(0.75, 0.04 + 0.003 * inn.Traffic) * (0.6 + (inn.Fame / 100) * 0.8) * season * JsMath.Max(0.4, danger) * pantry;
    }

    private static List<(string Kind, double W)> KindWeights(Sim s, Inn inn, List<Settlement> near)
    {
        var w = s.W;
        bool trouble = J.Some(near, x => x.Plague != null || x.Starving > 0 || (x.BurnedHouses ?? 0) > 0);
        bool war = J.Some(near, x => s.InWar(w.Civs[x.Civ]));
        bool temple = J.Some(w.Settlements, x => x.Alive && J.T(x.Civics.Get("temple")) && s.G.Dist(x.Tile, inn.Tile) <= 28);
        bool lib = J.Some(w.Settlements, x => x.Alive && J.T(x.Civics.Get("library")) && s.G.Dist(x.Tile, inn.Tile) <= 30);
        bool bardCiv = J.Some(near, x => w.Civs[x.Civ].Cls == "bard");
        int routes = J.Filter(w.Routes, r => r.Alive).Count;
        return new List<(string Kind, double W)>
        {
            ("merchant", 2 + JsMath.Min(3, routes * 0.3)), ("pilgrim", temple ? 1.4 : 0.3), ("bard", bardCiv ? 1.5 : 0.6), ("hunter", ForestNear(s, inn.Tile) ? 1 : 0.4),
            ("scholar", lib ? 0.9 : 0.15), ("soldier", war ? 1.6 : 0.4), ("refugee", trouble ? 2.2 : 0), ("wanderer", 1), ("noble", s.Year >= 6 && inn.Level >= 2 ? 0.35 : 0),
        };
    }

    private static double Range(Sim s, List<double> r) => r[0] + s.Rng.Next() * (r[1] - r[0]);
    private static double Irange(Sim s, List<double> r) => s.Rng.Int(r[0], r[1]);

    private static void SpawnTraveler(Sim s, Inn inn)
    {
        var w = s.W;
        int coming = J.Filter(w.Agents, a => !J.T(a.Dead) && a.Kind == "traveler" && a.Purpose == "come" && a.Inn == inn.Id).Count;
        if (coming >= 6) return;
        var near = J.Filter(w.Settlements, x => x.Alive && s.G.Dist(x.Tile, inn.Tile) <= 20);
        if (near.Count == 0) return;
        string kind = s.Rng.Weighted(KindWeights(s, inn, near), k => k.W).Kind;
        var troubled = J.Filter(near, x => x.Plague != null || x.Starving > 0 || (x.BurnedHouses ?? 0) > 0);
        var atWar = J.Filter(near, x => s.InWar(w.Civs[x.Civ]));
        var pool = kind == "refugee" && troubled.Count > 0 ? troubled : kind == "soldier" && atWar.Count > 0 ? atWar : near;
        var from = s.Rng.Weighted(pool, x => (s.Pop(x) + 3) / (1 + s.G.Dist(x.Tile, inn.Tile) / 4.0));
        // gideceği yer: hacı tapınağa, bilgin kütüphaneye, göçmen dertsiz bir yerleşime, tüccar başka bir medeniyete
        var others = J.Filter(w.Settlements, x => x.Alive && x.Id != from.Id && s.G.Dist(x.Tile, inn.Tile) <= 30);
        List<Settlement> WantOf(Func<Settlement, bool> f) => J.Filter(others, f);   // TS: const want = (f) => others.filter(f)
        var destPool = kind == "pilgrim" ? WantOf(x => J.T(x.Civics.Get("temple"))) : kind == "scholar" ? WantOf(x => J.T(x.Civics.Get("library")))
            : kind == "refugee" ? WantOf(x => x.Plague == null && x.Starving <= 0) : kind == "merchant" ? WantOf(x => x.Civ != from.Civ) : others;
        var dests = destPool.Count > 0 ? destPool : others;
        var to = dests.Count > 0 ? s.Rng.Weighted(dests, x => (s.Pop(x) + 5) / (1 + s.G.Dist(x.Tile, inn.Tile) / 6.0)) : null;
        var def = D.GUEST[kind];
        string race = RaceFrom(s, from);
        double n = Irange(s, def.N);
        if (kind == "refugee") { n = JsMath.Min(n, JsMath.Max(0, s.Pop(from) - (Sim.IsCore(from) ? Sim.CORE_MIN : 4))); if (n <= 0) return; }   // Faz 1b-4: çekirdek şehir mültecilerle tükenmez
        var c = w.Civs[from.Civ];
        string toName = to?.Name ?? "uzak diyarlar";
        string topGood = J.At(J.Sort(J.Filter(c.Stock.Keys(), g => g != "gold" && (c.Stock.Get(g) ?? 0) >= 5), (a, b) => (c.Stock.Get(b) ?? 0) * D.GOODS[b].Base - (c.Stock.Get(a) ?? 0) * D.GOODS[a].Base), 0);
        string why = kind == "merchant" ? $"{Tr.Ek(toName, "a")} {(J.T(topGood) ? J.TrLower(D.GOODS[topGood].Name) : "mal")} götürüyor"
            : kind == "pilgrim" ? $"{(to != null && J.T(to.Civics.Get("temple")) ? Tr.Ek(toName, "in") + " sunağına hacca" : "kutsal bir yere")} gidiyor"
                : kind == "bard" ? "şarkı söyleyip yol parası topluyor"
                    : kind == "hunter" ? "av peşinde; hana et satacak"
                        : kind == "scholar" ? $"{(to != null && J.T(to.Civics.Get("library")) ? Tr.Ek(toName, "in") + " kütüphanesine" : "eski yazıtların peşinde")} gidiyor"
                            : kind == "soldier" ? (s.InWar(c) ? $"{c.Name} ordusundan izinli" : $"{c.Name} sınır bekçisi, nöbet değişimi")
                                : kind == "refugee" ? $"{Tr.Ek(from.Name, "da")}ki {(from.Plague != null ? "salgından" : from.Starving > 0 ? "kıtlıktan" : "yangından")} kaçıyor"
                                    : kind == "noble" ? $"maiyetiyle {Tr.Ek(toName, "a")} gidiyor"
                                        : "kısmetini arıyor";
        var path = s.Path(from.Tile, inn.Tile);
        if (path == null || path.Count < 2) return;
        if (kind == "refugee") s.RemovePop(from, n);
        // object literal order: id, name (rng), ..., purse (rng), spent, nights (rng)
        int gid = s.Id();
        string gname = PersonName(s, race);
        double purse = JsMath.Round(Range(s, def.Purse) * n * 10) / 10;
        double nights = Irange(s, def.Nights);
        var g = new Guest
        {
            Id = gid, Kind = kind, Name = gname, Race = race, N = n, Civ = from.Civ, From = from.Id, FromName = from.Name, To = to?.Id ?? -1, ToName = toName, Why = why,
            Purse = purse, Spent = 0, Nights = nights, Arrived = s.Day, Mood = 0.7,
        };
        w.Agents.Add(new Agent { Id = s.Id(), Kind = "traveler", Civ = from.Civ, Path = path, Step = 0, Progress = 0, Speed = kind == "noble" ? 1 : 0.8, From = from.Id, To = inn.Id, Inn = inn.Id, Guest = g, Purpose = "come" });
        s.Metric("innTraveler");
    }

    /// <summary>handan (ya da kapalı handan) hedefine yürür</summary>
    private static void SendOn(Sim s, int fromTile, Guest g, int innId)
    {
        var to = g.To >= 0 ? s.Settlement(g.To) : null;
        if (to == null || !to.Alive) return;
        var path = s.Path(fromTile, to.Tile);
        if (path == null || path.Count < 2) { if (g.Kind == "refugee") s.AddPop(to, g.Race, g.N); return; }
        s.W.Agents.Add(new Agent { Id = s.Id(), Kind = "traveler", Civ = g.Civ, Path = path, Step = 0, Progress = 0, Speed = g.Kind == "noble" ? 1 : 0.8, From = innId, To = to.Id, Inn = innId, Guest = g, Purpose = "leave" });
    }

    /// <summary>Yolcu vardı: hana giriş yapar; ayrılan göçmen hedef yerleşime katılır (true = ajan biter).</summary>
    public static bool TravelerArrive(Sim s, Agent a)
    {
        var g = a.Guest;
        if (g == null) return true;
        if (a.Purpose == "come")
        {
            var inn = Will.InnById(s, a.Inn);
            if (inn == null || !inn.Alive) { SendOn(s, a.Path[a.Path.Count - 1], g, a.Inn ?? -1); return true; }
            CheckIn(s, inn, g);
            return true;
        }
        if (g.Kind == "refugee")
        {
            var st = s.Settlement(g.To);
            if (st != null && st.Alive) { s.AddPop(st, g.Race, g.N); s.Log("migration", $"{J.S(g.N)} göçmen {Tr.Ek(g.FromName, "dan")} {Tr.Ek(st.Name, "a")} ulaştı; yolda handa soluklandılar.", civ: st.Civ, tile: st.Tile); }
        }
        return true;
    }

    private static string GuestTag(Guest g) => $"{D.GUEST[g.Kind].Icon} {g.Name}{(g.N > 1 ? $" +{J.S(g.N - 1)}" : "")}";

    private static bool CheckIn(Sim s, Inn inn, Guest g, bool force = false)
    {
        var L = D.INN_LEVEL[inn.Level];
        var occ = InnOcc(inn);
        if (occ.Rooms + g.N <= L.Rooms) g.Stable = false;
        else if (force || occ.Stable + g.N <= L.Stable) { g.Stable = true; inn.Turned++; }
        else
        {
            inn.Turned++; inn.Total.Turned += g.N; inn.Fame = JsMath.Max(0, inn.Fame - 0.4);
            AddLog(s, inn, "no", $"{GuestTag(g)} ({J.TrLower(D.GUEST[g.Kind].Name)}) yer bulamadı, {g.ToName} yoluna devam etti");
            if (g.Kind != "caravan") SendOn(s, inn.Tile, g, inn.Id);
            return false;
        }
        g.Arrived = s.Day;
        inn.Guests.Add(g);
        inn.Total.Guests += g.N;
        Book(s, inn).Guests += g.N;
        string extra = "";
        if (g.Kind == "hunter" && inn.Gold > 3) { inn.Stock.Food += 15; inn.Gold -= 1.5; Book(s, inn).Supply += 1.5; g.Purse += 1.5; extra = " · hana av eti sattı"; }
        if (g.Kind == "bard") { inn.Fame = JsMath.Min(100, inn.Fame + 0.8); extra = " · akşam ocak başında çalacak"; }
        if (g.Kind == "refugee") inn.Fame = JsMath.Min(100, inn.Fame + 0.4);
        AddLog(s, inn, "in", $"{GuestTag(g)} — {J.TrLower(D.GUEST[g.Kind].Name)}, {Tr.Ek(g.FromName, "dan")} geldi{(J.T(g.Stable) ? " (ahırda yatacak)" : "")}{extra}");
        return true;
    }

    private static void CheckOut(Sim s, Inn inn, Guest g, string why = "")
    {
        inn.Guests = J.Filter(inn.Guests, x => x != g);
        if (g.Kind == "caravan")
        {
            var a = J.Find(s.W.Agents, x => x.Id == g.Agent && !J.T(x.Dead));
            if (a != null) a.RestUntil = s.Day;
            AddLog(s, inn, "out", $"{GuestTag(g)} yola koyuldu → {g.ToName}{(J.T(why) ? $" ({why})" : "")}", g.Spent);
            return;
        }
        AddLog(s, inn, "out", $"{GuestTag(g)} ayrıldı → {g.ToName}{(J.T(why) ? $" ({why})" : "")}", g.Spent);
        SendOn(s, inn.Tile, g, inn.Id);
    }

    /// <summary>kervan hanın önünden geçerken bir gece konaklar (tayfanın yemeğini medeniyet hazinesi öder)</summary>
    public static bool CaravanPassInn(Sim s, Agent a)
    {
        int tile = a.Path[a.Step];
        int? z = s.W.Tiles[tile].InnZone;
        if (z == null || (a.RestedAt != null && a.RestedAt.Contains(z.Value)) || a.Path.Count - 1 - a.Step < 3) return false;
        var inn = J.Find(s.W.Inns, x => x.Id == z && x.Alive);
        if (inn == null) return false;
        var restedAt = new List<int>(a.RestedAt ?? new List<int>()); restedAt.Add(z.Value);
        a.RestedAt = restedAt;
        var c = s.W.Civs[a.Civ];
        Settlement from = s.Settlement(a.From ?? -1), to = s.Settlement(a.To ?? -1);
        string cargo = string.Join(", ", J.Slice(J.Map(J.Filter((a.Cargo ?? new JsObj<double>()).Entries(), kv => kv.Value >= 1), kv => $"{J.S(JsMath.Round(kv.Value))} {J.TrLower(D.GOODS[kv.Key].Name)}"), 0, 2));
        var g = new Guest
        {
            Id = s.Id(), Kind = "caravan", Name = $"{c.Name} kervanı", Race = c.Race, N = 2 + JsMath.Min(4, a.Troops ?? 0), Civ = c.Id, From = from?.Id ?? -1, FromName = from?.Name ?? "?", To = to?.Id ?? -1, ToName = to?.Name ?? "?",
            Why = J.T(cargo) ? $"{cargo} taşıyor" : "boş dönüyor", Purse = 0, Spent = 0, Nights = 1, Arrived = s.Day, Agent = a.Id, Mood = 0.7,
        };
        if (!CheckIn(s, inn, g)) return false;
        a.RestUntil = s.Day + 1;
        a.Inn = inn.Id;
        s.Metric("innCaravan");
        return true;
    }

    // ------------------------------------------------------------ kahramanlar: hanın müdavimleri
    private static void SyncHeroes(Sim s, Inn inn)
    {
        var present = J.Filter(s.W.Heroes, h => h.Civ == -1 && h.State == "tavern" && h.Tavern == inn.Id);
        var ids = new HashSet<int>(J.Map(present, h => h.Id));
        foreach (var g in J.Filter(inn.Guests, x => x.Kind == "hero" && !(x.Hero != null && ids.Contains(x.Hero.Value))))
        {
            var h = s.Hero(g.Hero.Value);
            string why = h.State == "dead" ? "öldü" : h.State == "retired" ? "kılıcını astı" : h.State == "gone" ? "uzak diyarlara gitti"
                : h.Civ >= 0 ? $"{s.W.Civs[h.Civ].Name} ile sözleşme imzaladı" : h.Goal != null ? $"yola çıktı: {h.Goal.Text}" : "yola çıktı";
            inn.Guests = J.Filter(inn.Guests, x => x != g);
            AddLog(s, inn, "out", $"★ {Will.HeroLabel(h)} {why}", g.Spent);
        }
        foreach (var h in present)
        {
            if (J.Some(inn.Guests, x => x.Hero == h.Id)) continue;
            var L = D.INN_LEVEL[inn.Level];
            var occ = InnOcc(inn);
            bool fresh = s.Day - h.Born <= 5;
            var g = new Guest
            {
                Id = s.Id(), Kind = "hero", Name = Will.HeroLabel(h), Race = h.Race, N = 1, Civ = -1, From = -1, FromName = fresh ? "yollardan" : (J.Last(h.Journal)?.Text ?? "yollardan"),
                To = -1, ToName = "", Why = $"Sv {J.S(h.Level)} {ClassTr(h.Cls)}", Purse = 0, Spent = 0, Nights = 0, Arrived = s.Day, Hero = h.Id, Mood = 0.75, Stable = occ.Rooms + 1 > L.Rooms,
            };
            inn.Guests.Add(g);
            inn.Total.Guests++;
            Book(s, inn).Guests++;
            string extra = "";
            double tab = inn.Tabs.Get(h.Id) ?? 0;
            if (tab >= 10 && h.Gold > 0)
            {
                double pay = JsMath.Min(h.Gold, Math.Floor(tab / 10));
                h.Gold -= pay; inn.Tabs.Set(h.Id, tab - pay * 10); inn.Gold += pay; inn.Total.Income += pay; Book(s, inn).Room += pay;
                extra = $" · veresiye borcunu ödedi ({J.S(pay)} altın)";
            }
            AddLog(s, inn, "in", $"★ {Will.HeroLabel(h)} {(fresh ? "kapıdan girdi: yeni bir yüz" : "döndü")} (Sv {J.S(h.Level)} {ClassTr(h.Cls)}){extra}");
        }
    }

    // ------------------------------------------------------------ gün
    private static void Nightly(Sim s, Inn inn)
    {
        var pr = InnPrices(inn); var bk = Book(s, inn);
        var occ = InnOcc(inn); double cap = ServeCap(inn);
        bool bard = J.Some(inn.Guests, g => g.Kind == "bard");
        double satSum = 0, satN = 0, helpers = 0;
        foreach (var g in J.Slice(inn.Guests))
        {
            var def = D.GUEST[g.Kind];
            bool poorHero = g.Kind == "hero" && s.Hero(g.Hero.Value).Gold < 1;   // kesesi boş: ekmek, çorba, bir yudum bira
            double eat = poorHero ? 1 : g.N * def.Eat, mugs = poorHero ? 0.4 : g.N * def.Mugs * (bard && g.Kind != "bard" ? 1.3 : 1);
            bool gotFood = inn.Stock.Food >= eat;
            inn.Stock.Food = JsMath.Max(0, inn.Stock.Food - eat);
            double aleGot = JsMath.Min(inn.Stock.Ale, mugs);
            inn.Stock.Ale -= aleGot;
            double roomS = (J.T(g.Stable) ? Math.Ceiling(pr.Room / 2) : pr.Room) * g.N, foodS = gotFood ? pr.Meal * g.N : 0, aleS = aleGot * pr.Ale;
            double billS = roomS + foodS + aleS;
            double paid = 0;
            if (g.Kind == "hero")
            {
                var h = s.Hero(g.Hero.Value);
                double tab = (inn.Tabs.Get(h.Id) ?? 0) + billS;
                if (h.Gold >= 1 && tab >= 10) { double pay = JsMath.Min(h.Gold, Math.Floor(tab / 10)); h.Gold -= pay; tab -= pay * 10; paid = pay; }
                if (h.Gold < 1 && tab >= 30)
                {
                    // kesesi boş kahraman borcunu emeğiyle öder: avlanır, nöbet tutar, yaraları sarar, hikâye anlatır
                    tab = JsMath.Max(0, tab - billS * 1.1);
                    g.Why = $"Sv {J.S(h.Level)} {ClassTr(h.Cls)} · kesesi boş, {HeroWork(h.Cls)}";
                    if (h.Cls == "ranger" || h.Cls == "barbarian" || h.Cls == "druid") inn.Stock.Food += 1.6;
                    else if (h.Cls == "fighter" || h.Cls == "paladin") inn.Fame = JsMath.Min(100, inn.Fame + 0.004);
                    else if (h.Cls == "rogue" && s.Rng.Chance(0.04)) { var mark = J.Find(inn.Guests, x => x.Purse >= 1 && x.Kind != "hero"); if (mark != null) { mark.Purse -= 1; h.Gold += 1; } }
                    helpers += h.Cls == "cleric" || h.Cls == "wizard" ? 1 : 0;
                }
                else g.Why = $"Sv {J.S(h.Level)} {ClassTr(h.Cls)}{(J.T(h.Gold) ? $" · kesesinde {J.S(h.Gold)} altın" : "")}";
                inn.Tabs.Set(h.Id, JsMath.Min(300, tab));
            }
            else if (g.Kind == "caravan")
            {
                var c = s.W.Civs[g.Civ];
                paid = JsMath.Min(billS / 10, JsMath.Max(0, s.St(c, "gold")));
                s.Add(c, "gold", -paid);
            }
            else if (g.Kind == "refugee" || g.Kind == "bard")
            {
                // göçmen elinden geleni öder; ozan şarkılarıyla öder
                paid = JsMath.Min(g.Purse, (billS / 10) * 0.3); g.Purse -= paid;
            }
            else
            {
                paid = JsMath.Min(g.Purse, billS / 10); g.Purse -= paid;
            }
            paid = JsMath.Round(paid * 100) / 100;
            g.Spent += paid; inn.Gold += paid; inn.Total.Income += paid; inn.Total.Nights += g.N; bk.Nights += g.N;
            if (billS > 0 && paid > 0) { double k = paid / (billS / 10); bk.Room += (roomS / 10) * k; bk.Food += (foodS / 10) * k; bk.Ale += (aleS / 10) * k; }
            // memnuniyet
            double m = 0.72 + 0.04 * (inn.Level - 1);
            if (!gotFood) m -= 0.35;
            if (def.Mugs > 0.5 && aleGot < mugs * 0.6) m -= 0.15;
            if (J.T(g.Stable)) m -= 0.18;
            if (occ.Persons > cap) m -= 0.15 * JsMath.Min(1, (occ.Persons - cap) / cap);
            if (inn.Stock.Wood <= 0) m -= 0.06;   // Faz 1b-3: sönük ocak (eski kışın −0,25'inin yıllık ortalaması)
            if (bard && g.Kind != "bard") m += 0.08;
            m += JsMath.Min(0.06, helpers * 0.03);
            g.Mood = g.Mood * 0.5 + JsMath.Max(0, JsMath.Min(1, m)) * 0.5;
            satSum += g.Mood * g.N; satN += g.N;
            // ayrılış
            if (g.Kind == "hero") continue;
            double stayed = s.Day - g.Arrived;
            bool broke = g.Kind != "refugee" && g.Kind != "caravan" && g.Kind != "bard" && g.Purse < 0.3;
            if (stayed >= g.Nights) CheckOut(s, inn, g);
            else if (broke && stayed >= 1) CheckOut(s, inn, g, "kesesi boşaldı");
            else if (g.Mood < 0.35 && stayed >= 1) { CheckOut(s, inn, g, "memnun kalmadı"); inn.Fame = JsMath.Max(0, inn.Fame - 0.6); }
        }
        // hancı ve personel de yer; ocak yanar; bahçe ve kümes
        var L = D.INN_LEVEL[inn.Level];
        inn.Stock.Food = JsMath.Max(0, inn.Stock.Food - (1 + inn.Staff.Count));
        inn.Stock.Food = JsMath.Min(L.Food * 1.2, inn.Stock.Food + 1.35 * inn.Level);   // bahçe, kümes, küçük tarla (Faz 1b-3: kışsız yıllık ortalama)
        // bira biterse hancı kilerdeki tahıldan kendi birasını mayalar
        if (inn.Stock.Ale < L.Ale * 0.2 && inn.Stock.Food > L.Food * 0.5) { inn.Stock.Food -= 5; inn.Stock.Ale += (5 / D.GRAIN_FOOD) * D.GRAIN_ALE; }
        // geçenlerden yol parası: at sulama, yem, nal, su
        double toll = 0.04 + JsMath.Min(200, inn.Traffic) * 0.0014;
        inn.Gold += toll; bk.Other += toll; inn.Total.Income += toll;
        double woodUse = 0.2875 + occ.Persons * 0.02;   // Faz 1b-3: ocak ve mutfak, eski mevsimlerin yıllık ortalaması
        if (ForestNear(s, inn.Tile) && inn.Stock.Wood < L.Wood) inn.Stock.Wood += 0.45;   // çırak ya da seyis odun keser
        inn.Stock.Wood = JsMath.Max(0, inn.Stock.Wood - woodUse);
        if (J.T(satN)) inn.Sat = inn.Sat * 0.8 + (satSum / satN) * 0.2;
        int legends = J.Filter(s.W.Heroes, h => J.T(h.Legend) && h.BaseInn && h.Base == inn.Id).Count;
        double target = 10 + inn.Sat * 60 + inn.Level * 4 + JsMath.Min(12, legends * 4) + JsMath.Min(6, inn.Raids);
        inn.Fame = JsMath.Max(0, JsMath.Min(100, inn.Fame + (target - inn.Fame) * (J.T(satN) ? 0.012 : 0.004)));
    }

    private static void Restock(Sim s, Inn inn)
    {
        if (inn.Order != null) return;
        var L = D.INN_LEVEL[inn.Level];
        bool lowF = inn.Stock.Food < L.Food * 0.35, lowA = inn.Stock.Ale < L.Ale * 0.3;
        bool lowW = !ForestNear(s, inn.Tile) && inn.Stock.Wood < L.Wood * 0.3;
        if (!lowF && !lowA && !lowW) return;
        PlaceOrder(s, inn, new Want { Food = JsMath.Max(0, L.Food - inn.Stock.Food), Ale = JsMath.Max(0, L.Ale - inn.Stock.Ale), Wood = lowW ? Math.Ceiling(L.Wood - inn.Stock.Wood) : 0 });
    }

    private static void EnsureStaff(Sim s, Inn inn)
    {
        if (inn.Stage != "open") return;
        var want = J.Slice(D.INN_LEVEL[inn.Level].Staff);
        var have = J.Map(inn.Staff, x => x.Role);
        foreach (var r in have) { int k = want.IndexOf(r); if (k >= 0) want.RemoveAt(k); }
        // kalabalık hanın fazladan garsonu
        var busy = J.Slice(inn.Hist, -6);
        if (busy.Count >= 6 && J.Reduce(busy, (a, h) => a + h.Guests, 0.0) / busy.Count > ServeCap(inn) && J.Filter(inn.Staff, x => x.Role == "garson").Count < 3 && inn.Gold > 70) want.Add("garson");
        var home = J.At(NearestSettlements(s, inn.Tile, 1), 0).X;
        foreach (var role in want)
        {
            if (inn.Gold < D.STAFF[role].Wage * 3) break;
            string race = RaceFrom(s, home);
            var x = new InnStaff { Name = PersonName(s, race), Race = race, Role = role, Since = s.Day, From = home?.Name ?? "yollardan" };
            inn.Staff.Add(x);
            AddLog(s, inn, "staff", $"{x.Name} {J.TrLower(D.STAFF[role].Name)} olarak işe başladı ({x.From})");
        }
    }

    private static void PayWages(Sim s, Inn inn)
    {
        double due = Wages(inn);
        if (!J.T(due)) return;
        if (inn.Gold >= due) { inn.Gold -= due; Book(s, inn).Wage += due; return; }
        double paid = JsMath.Max(0, inn.Gold);
        inn.Gold -= paid; Book(s, inn).Wage += paid;
        // kasa boş: en son işe giren (çırak değilse) ayrılır
        var idx = new List<(InnStaff X, int I)>();
        for (int i = 0; i < inn.Staff.Count; i++) idx.Add((inn.Staff[i], i));
        var k = J.At(J.Sort(J.Filter(idx, q => q.X.Role != "cirak"), (a, b) => b.X.Since - a.X.Since), 0);
        if (k.X != null) { J.Splice(inn.Staff, k.I, 1); AddLog(s, inn, "staff", $"{k.X.Name} ({J.TrLower(D.STAFF[k.X.Role].Name)}) ücretini alamayınca ayrıldı"); }
    }

    private static void ConsiderUpgrade(Sim s, Inn inn)
    {
        if (inn.Stage != "open" || inn.Build != null || inn.Level >= 3) return;
        var u = D.INN_UPGRADE[inn.Level + 1];
        if (s.Year < u.Year || inn.Fame < u.Fame || inn.Gold < u.Gold + 30) return;
        var h = J.Slice(inn.Hist, -12);
        double busy = h.Count > 0 ? J.Reduce(h, (a, x) => a + x.Guests, 0.0) / h.Count : 0;
        if (inn.Turned < 5 && busy < D.INN_LEVEL[inn.Level].Rooms * 0.65) return;
        inn.Gold -= u.Gold;
        Book(s, inn).Build += u.Gold;
        inn.Build = new InnBuild { Level = inn.Level + 1, Work = 0, Need = u.Work, Wood = 0, WoodNeed = u.Wood, Stone = 0, StoneNeed = u.Stone, Started = s.Day };
        AddLog(s, inn, "build", $"Genişletme başladı: {D.INN_LEVEL[inn.Level + 1].Name} olacak ({J.S(u.Gold)} altın)", -u.Gold);
        s.Log("inn", $"Hancı {inn.Keeper}, {Tr.Ek(InnTitle(inn), "i")} büyütüyor: yeni kanat, daha büyük ahır. {D.INN_LEVEL[inn.Level + 1].Name} olacak.", tile: inn.Tile, major: true, cause: inn.Turned >= 5 ? $"Son yıl {J.S(inn.Turned)} yolcu yer bulamadı" : $"Odalar hep dolu · ün {J.S(JsMath.Round(inn.Fame))}");
    }

    /// <summary>kasa taşınca hancı kazancın bir kısmını memleketine yollar (akrabalar, vergi, borç)</summary>
    private static void Remit(Sim s, Inn inn)
    {
        if (inn.Gold <= 450) return;
        var home = s.Settlement(inn.Origin) ?? J.At(NearestSettlements(s, inn.Tile, 1), 0).X;
        if (home == null || !home.Alive) return;
        double g = JsMath.Round((inn.Gold - 450) * 0.4);
        var c = s.W.Civs[home.Civ];
        inn.Gold -= g; s.Add(c, "gold", g); Book(s, inn).Other -= g;
        AddLog(s, inn, "ev", $"Hancı {inn.Keeper} kazancından {J.S(g)} altını memleketi {Tr.Ek(home.Name, "a")} yolladı", -g);
    }

    /// <summary>usta hanın çırağı yetişince kendi hanını kurmaya gider</summary>
    private static void ConsiderApprentice(Sim s, Inn inn)
    {
        if (inn.Level < 2 || s.Year < 5) return;
        int active = J.Filter(s.W.Inns, x => x.Stage != "ruin").Count;
        if (active >= s.W.InnPlan.Target + (s.Year >= 12 ? 1 : 0)) return;
        var ap = J.Find(inn.Staff, x => x.Role == "cirak" && s.Day - x.Since >= 2 * Sim.YEAR);
        if (ap == null || !s.Rng.Chance(0.4)) return;
        if (!DispatchKeeper(s, fromInn: inn, name: ap.Name, race: ap.Race)) return;
        inn.Staff = J.Filter(inn.Staff, x => x != ap);
        AddLog(s, inn, "staff", $"Çırak {ap.Name} ustasının hayır duasıyla kendi hanını kurmaya gitti");
    }

    /// <summary>Günlük han tiki: açılış dalgası, harabe yenileme, inşaat, misafirler, kiler, personel, defter.</summary>
    public static void InnLifeTick(Sim s)
    {
        var w = s.W;
        // açılış dalgası: oyun başında birkaç hancı yerleşimlerden çıkar
        var plan = w.InnPlan;
        if (plan.Wave > 0 && s.Day >= plan.Next)
        {
            if (DispatchKeeper(s)) plan.Wave--; else if (s.Day > 90) plan.Wave = 0;
            plan.Next = s.Day + s.Rng.Int(2, 5);
        }
        if (s.Day % 90 == 45 && plan.Wave == 0 && w.Inns.Count < plan.Target && s.Rng.Chance(0.5)) DispatchKeeper(s);
        // JS for...of: keeps the array it started with and re-reads its length (inns pushed meanwhile are visited)
        var inns = w.Inns;
        for (int ii = 0; ii < inns.Count; ii++)
        {
            var inn = inns[ii];
            if (inn.Stage == "road") continue;
            if (inn.Stage == "ruin")
            {
                if (s.Day % 30 == 0 && s.Day - (inn.RuinedDay ?? 0) >= 3 * Sim.YEAR && !J.Some(w.Camps, c => c.Alive && s.G.Dist(c.Tile, inn.Tile) <= 10)) DispatchKeeper(s, rebuild: inn);
                continue;
            }
            if (inn.Build != null) BuildTick(s, inn);
            if (inn.Stage != "open") continue;
            if ((s.Day + inn.Id) % 10 == 0)
            {
                ComputeTraffic(s, inn);
                inn.Hist.Add(new InnHist { Day = s.Day, Guests = InnOcc(inn).Persons, Gold = JsMath.Round(inn.Gold), Fame = JsMath.Round(inn.Fame) });
                if (inn.Hist.Count > 72) J.Shift(inn.Hist);
            }
            SyncHeroes(s, inn);
            if (s.Rng.Chance(ArrivalRate(s, inn))) SpawnTraveler(s, inn);
            Nightly(s, inn);
            Restock(s, inn);
            if ((s.Day + inn.Id) % 30 == 0) { PayWages(s, inn); EnsureStaff(s, inn); ConsiderUpgrade(s, inn); }
            if ((s.Day + inn.Id) % Sim.YEAR == 0) { inn.Turned = Math.Floor(inn.Turned / 2); ConsiderApprentice(s, inn); Remit(s, inn); }
        }
    }

    /// <summary>baskında han boşalır: misafirler kaçar, kiler yağmalanır</summary>
    public static void InnScatter(Sim s, Inn inn, string why, bool ruin)
    {
        foreach (var g in J.Slice(inn.Guests)) if (g.Kind != "hero") CheckOut(s, inn, g, why);
        inn.Stock.Food *= ruin ? 0 : 0.5; inn.Stock.Ale *= ruin ? 0 : 0.5;
        if (ruin)
        {
            inn.Guests = new List<Guest>();
            inn.Stage = "ruin"; inn.Build = null;
            foreach (var x in inn.Staff) AddLog(s, inn, "staff", $"{x.Name} kaçtı");
            inn.Staff = new List<InnStaff>(); inn.Fame *= 0.5; inn.Stock.Wood = 0; inn.Order = null;
        }
        else inn.Fame = JsMath.Max(0, inn.Fame - 8);
    }
}
