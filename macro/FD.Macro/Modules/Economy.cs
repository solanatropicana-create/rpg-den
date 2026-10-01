using System;
using System.Collections.Generic;
using System.Linq;

// Aktif ekonomi: değer tabanlı iş gücü, çıkarma/işleme, tükenme, tüketim, büyüme, inşaat kararları.
// Port of src/sim/economy.ts.

namespace FD.Macro;

/// <summary>economy.ts <c>JobOpt</c>: one job option of a settlement's daily labour allocation.</summary>
public sealed class JobOpt
{
    public string Key;
    public double Slots;
    public double V;
    /// <summary>TS <c>run: (n: number) =&gt; void</c> (n = workers assigned).</summary>
    public Action<double> Run;
}

/// <summary>
/// economy.ts <c>Cand</c>: a build candidate. <c>Project</c> is TS
/// <c>Omit&lt;Project, 'left' | 'total'&gt;</c>: only Type/Kind/Tile/Level are set (Tile/Level null when absent);
/// Left/Total are filled when the candidate becomes <c>st.Project</c>.
/// </summary>
public sealed class Cand
{
    public double Score;
    public JsObj<double> Cost;
    public double Work;
    public Project Project;
}

public static class Economy
{
    /// <summary>Faz 1b-3: mevsimler kalktı; tarla ve yaban (av, balık, ot, toplayıcı) verimi eski mevsim çarpanlarının yıllık
    /// ortalaması: tarla (1 + 1,3 + 1,1 + 0,25) / 4, yaban (1 + 1,2 + 1,1 + 0,5) / 4</summary>
    public const double FARM_YIELD = 0.9125, WILD_YIELD = 0.95;
    /// <summary>Faz 1b-3: kademe verimi (eski ana ağacın verim düğümlerinin yerine: Bronz Aletler, Lonca, Taç ve Kanun her biri +0,1):
    /// yapının bağlı olduğu yerleşimin kademesine göre çıkarma verimine eklenir (atölyelere yarısı)</summary>
    public static readonly double[] TIER_PROD = { 0, 0.1, 0.2, 0.3 };
    /// <summary>Faz 1b-3: kademe vergisi (eski Para +0,5 ve Taç ve Kanun +0,3): devletin hazine düzeni medeniyetin kademesiyle
    /// (en büyük yerleşimi) gelir ve bütün nüfusun vergisine eklenir</summary>
    public static readonly double[] TIER_TAX = { 0, 0, 0.5, 0.8 };
    /// <summary>Faz 1b-3: kademe büyümesi (eski İnanç +0,05 baştan, Hekimlik +0,1 Kasaba'dan)</summary>
    public static readonly double[] TIER_GROWTH = { 0.05, 0.05, 0.15, 0.15 };
    /// <summary>Faz 1b-3: Ormancılık (eski düğüm) yerine Kasaba+ yerleşimin kereste verimi</summary>
    public const double FORESTRY_WOOD = 0.25;
    /// <summary>Faz 1b-3: askerin deri bakımı (kayış, çizme, kalkan kaplaması): asker başı günlük deri (deri artık kış giysisi değil;
    /// kentler de deri tüketir: TOWN_NEEDS "leather")</summary>
    public const double SOLDIER_LEATHER = 0.003;
    /// <summary>Faz 1b-3: kuraklıkta (anlatıcı krizi) tarla ve toplayıcı verimi bu kadar düşer</summary>
    public const double DROUGHT_FARM = 0.5;
    private static readonly string[] HOUSING = { "hut", "house", "stonehouse" };

    private static readonly string[] DEPOSIT_EXTRACTS = { "farm", "mine", "claypit", "pasture", "herbalist", "crystal", "mithril", "grove" };

    /// <summary>Hex'teki çıkarma yapısının ürettiği mal (yatak malı; yoksa kereste / et / balık / taş).</summary>
    public static string ExtractGood(Tile t, Sim s)
    {
        string k = t.Ext.Kind;
        if (t.Deposit >= 0 && Array.IndexOf(DEPOSIT_EXTRACTS, k) >= 0)
        {
            var d = J.Find(s.W.Deposits, x => x.Id == t.Deposit);
            return D.DEPOSITS[d.Kind].Good;
        }
        return k == "lumber" ? "wood" : k == "hunt" ? "meat" : k == "dock" ? "fish" : "stone";
    }

    /// <summary>Kuraklık sürüyor mu (Storyteller krizi): tarla ve toplayıcı verimi düşer.</summary>
    public static bool Drought(Sim s) => s.W.Story != null && s.Day < s.W.Story.DroughtUntil;

    /// <summary>Hex'teki yapının işçi başına günlük verimi (level/kind default: t.Ext.Level / t.Ext.Kind; tier: yapının bağlı
    /// olduğu yerleşimin kademesi, verilmezse hex'in sahibinden okunur).</summary>
    public static (string Good, double Y) ExtractYield(Sim s, Civ c, Tile t, int? level = null, string kind = null, int? tier = null)
    {
        int lvl = level ?? t.Ext.Level;
        int tr = tier ?? (t.Owner >= 0 ? s.Settlement(t.Owner)?.Tier ?? 0 : 0);
        string knd = kind ?? t.Ext.Kind;
        double rate = 0.1; string good = "stone";
        double richness = 1;
        if (t.Deposit >= 0 && knd != "lumber" && knd != "hunt" && knd != "quarry" && knd != "dock")
        {
            var d = J.Find(s.W.Deposits, x => x.Id == t.Deposit);
            rate = D.DEPOSITS[d.Kind].Rate; good = D.DEPOSITS[d.Kind].Good; richness = d.Richness;
        }
        else if (knd == "lumber") { rate = 0.25; good = "wood"; if (c.Cls == "druid") rate *= 0.4; }
        else if (knd == "hunt") { rate = 0.17; good = "meat"; }
        else if (knd == "dock") { rate = 0.26; good = "fish"; }
        else if (knd == "quarry") { rate = 0.2; good = "stone"; }
        double m = 1 + s.E(c, "prodAll") + TIER_PROD[tr];
        bool food = good == "grain" || good == "meat" || good == "fish";
        if (food) m += s.E(c, "prodFood");
        if (good == "wood") m += s.E(c, "prodWood") + (tr >= Gate.FORESTRY ? FORESTRY_WOOD : 0);
        if (knd == "mine" || knd == "mithril" || knd == "quarry") m += s.E(c, "prodMine");
        if (good == "herbs") m += s.E(c, "prodHerbs");
        if (good == "mana") m += s.E(c, "prodMana");
        // Faz 1b-3: mevsim yok (yıllık ortalama); kuraklıkta tarlalar yarı verir
        double season = 1;
        if (knd == "farm") season = FARM_YIELD * (Drought(s) ? 1 - DROUGHT_FARM : 1);
        else if (knd == "hunt" || knd == "dock" || knd == "herbalist") season = WILD_YIELD;
        double toolF = 1;
        if (lvl >= 2 && s.St(c, "tools") < 0.5) toolF = 0.75;
        if (lvl >= 2 && c.Broke != null) toolF *= BROKE_EXT;   // C3: hazine boşken L2+ yapıların bakımı aksar
        return (good, rate * D.LEVEL_MULT[lvl] * richness * m * season * toolF);
    }

    // ------------------------------------------------------------ Faz 1 C3: altın ve ambar
    // Altın ve ambar anlam kazansın: boştaki işçi az altın getirir; asker, kahraman ve L2–L3 yapı bakım ister; kasaba ve
    // şehir bira, ekmek ve alet tüketir (yoksa büyüme yavaşlar, huzursuzluk artar); kıtlık tek büyük olaydır (komşular
    // yardım eder ya da yüz çevirir, aç medeniyet yağmaya döner); hazine fazlası kamu işlerine (imar) akar.

    /// <summary>işe yerleşemeyen (boştaki) işçinin günlük altını (eskiden 0,02: hazineler sınırsız büyüyordu)</summary>
    public const double IDLE_GOLD = 0.004;
    /// <summary>asker bakımı: günlük maaş (altın; eskiden 0,006) ve erzak (kişi başı gıdanın üstüne)</summary>
    public const double SOLDIER_PAY = 0.01, SOLDIER_RATION = 0.05;
    /// <summary>L2 ve L3 çıkarma yapısının günlük bakımı: altın ve alet aşınması (Kasaba+ yerleşimde demir aletler yarı hızda aşınır)</summary>
    public static readonly double[] EXT_GOLD = { 0, 0, 0.004, 0.01 }, EXT_TOOLS = { 0, 0, 0.0015, 0.003 };
    /// <summary>medeniyete bağlı kahramanın yıllık maaşı: taban + seviye başına; 30 günde bir dörtte biri ödenir. Maaşı art
    /// arda HERO_UNPAID kez ödenmeyen kahraman (yurttaysa) hizmetten ayrılır.</summary>
    public const double HERO_WAGE = 12, HERO_WAGE_LV = 6, HERO_UNPAID = 2;
    /// <summary>hazine boşken L2+ yapılar bu katla üretir; ödenemeyen her 10 günde yerleşim başına askerlerin bu payı firar eder</summary>
    public const double BROKE_EXT = 0.8, DESERT = 0.1;
    /// <summary>hazine boşluğu, ancak bu kadar günlük bakım biriktirince biter (kıl payı ödemeler bayrağı oynatmasın)</summary>
    public const double BROKE_RESERVE = 30;
    /// <summary>atölye işçi yuvası kademeyle büyür: kasaba ve şehir atölyeleri daha çok usta çalıştırır</summary>
    public static readonly double[] WS_TIER = { 1, 1, 1.5, 2.5 };
    /// <summary>kent tüketiminde yokluk: LACK_DAYS gün süren yokluk büyümeyi yavaşlatır (ekmek %12, bira %8, alet %5); LACK_LOG günde
    /// kroniğe düşer; ekmek ya da bira yokluğu LACK_UNREST günden sonra huzursuzluk getirir (Diplomacy.Unrest; alet yokluğu getirmez:
    /// madenler tükenince alet herkese kıt olur)</summary>
    public const double LACK_DAYS = 10, LACK_LOG = 30, LACK_UNREST = 30;
    private static readonly Dictionary<string, double> LACK_GROWTH = new() { ["bread"] = 0.12, ["beer"] = 0.08, ["tools"] = 0.05 };
    /// <summary>aynı malın yokluğu medeniyet başına en çok bu kadar yılda bir kroniğe düşer</summary>
    public const double LACK_LOG_GAP = 3;
    private static readonly string[] LACK_GOODS = { "bread", "beer", "tools" };
    /// <summary>kıtlık: açık bu kadar gün sürünce büyük olay olur; ambar art arda bu kadar gün (bir yıl) yetince biter: arada
    /// tekrarlayan açık aynı kıtlıktır. Birkaç günlük açık kıtlık sayılmaz: ilan için 10 gün ve ortalama %25 açık gerekir.</summary>
    public const double FAMINE_DECLARE = 10, FAMINE_END = 120;
    /// <summary>kıtlık ilanı için açık gıdanın ortalama bu payı olmalı (birkaç lokmalık açık kıtlık sayılmaz)</summary>
    public const double FAMINE_SHORT = 0.25;
    /// <summary>komşu yardımı: verenin kendine ayırdığı gıda (gün), yardım turları arası (gün), en çok tur, en çok kaç günlük gıda</summary>
    public const double AID_KEEP = 60, AID_GAP = 30, AID_ROUNDS = 3, AID_DAYS = 10;
    /// <summary>kıtlık turunda en çok bu kadar komşu yardım eder (en dost olandan başlayarak sorulur)</summary>
    public const int AID_HELPERS = 3;
    /// <summary>kamu işleri (imar): hazinenin yedeği (PW_RESERVE + PW_RESERVE_DAYS günlük bakım) aşan kısmının yılda PW_SPEND payı imara
    /// harcanır (kıtlıkta ve hazine boşken durur); boştakilerden amele tutulur (yerleşimin nüfus payı kadar, kademeyle: PW_SLOTS), amele
    /// en az AMELE_WAGE altın alır ve günde AMELE_MAT taş, kereste ya da tuğla harcar</summary>
    public const double PW_RESERVE = 200, PW_RESERVE_DAYS = 60, PW_SPEND = 0.8, AMELE_WAGE = 0.012, AMELE_MAT = 0.04;
    public static readonly double[] PW_SLOTS = { 0.05, 0.15, 0.3, 0.45 };
    /// <summary>ambarla beslenen amele (angarya): PW_FOOD_KEEP günlük gıdanın üstündeki ambarın yılda PW_FOOD_SPEND payı, altın yetmeyince
    /// amele tayınına gider (amele başı günde AMELE_FOOD gıda; imara AMELE_WAGE altın değerinde katkı)</summary>
    public const double PW_FOOD_KEEP = 120, PW_FOOD_SPEND = 1, AMELE_FOOD = 0.1;
    /// <summary>imar: kişi başı harcanan her altın IMAR_RATE puan; günde IMAR_DECAY payı söner; 100'de büyüme +IMAR_GROWTH, huzursuzluk
    /// −IMAR_CALM; IMAR_REPAIR üstünde yanan evler iki kat hızlı onarılır</summary>
    public const double IMAR_RATE = 12, IMAR_DECAY = 0.002, IMAR_GROWTH = 0.1, IMAR_CALM = 0.5, IMAR_REPAIR = 50;
    /// <summary>imar bu düzeye ilk kez varınca kroniğe düşer (başkentte büyük olay)</summary>
    public const double IMAR_LOG = 60;

    private static double Demand(Sim s, Civ c, string g, double P)
    {
        double w = c.Want.Get(g) ?? 0;
        switch (g)
        {
            case "wood": return 30 + P * 1.5 + w;
            case "stone": return 15 + P * 0.8 + w;
            // Faz 1b-3: eskiden düğüme bağlı talepler medeniyetin kademesine bağlı (I. çağ düğümleri baştan açık)
            case "bricks": return 8 + P * 0.15 + w;
            case "leather": return 6 + P * 0.3 + w;        // koşum, ayakkabı, asker teçhizatı ve silah
            case "tools": return (s.CivAt(c, Gate.TOOLS) ? 4 + P * 0.05 : 0) + w;
            case "arms": return (s.CivAt(c, Gate.ENCHANTING) ? 3 : 0) + w;
            case "horses": return 4 + P * 0.03 + w;
            case "potion": return (s.CivAt(c, Gate.MEDICINE) ? 4 + P * 0.06 : 0) + w;
            case "mana": { int ct = s.CivTier(c); return (ct >= Gate.ARCANA ? 5 + P * 0.04 : 0) + (ct >= Gate.ENCHANTING ? 6 : 0) + w; }
            case "mithril": return (s.CivAt(c, Gate.MITHRILWORK) ? 8 : 1) + w;
            case "beer": return P * 0.3 + w;
            case "salt": return 3 + P * 0.05 + w;
            case "gold": return 1e9;
            default: return 3 + w;
        }
    }

    /// <summary>mal, medeniyetin ihtiyacının çok üstünde birikmiş mi (yiyecek ve altın hariç)</summary>
    public static bool IsGlut(Sim s, Civ c, string g, double P)
    {
        return !J.T(D.GOODS[g].Food) && g != "gold" && s.St(c, g) > JsMath.Max(40, Demand(s, c, g, P) * 5);
    }

    /// <summary>Darphane geliri (altın/gün): taban + nüfus başına, yerleşim başına tavanlı.</summary>
    public const double MINT_BASE = 0.01, MINT_PER_POP = 0.0008, MINT_CAP = 0.12;

    /// <summary>
    /// Bir yerleşimin darphane geliri (altın/gün): her darphane kendi yerleşiminin büyüklüğüyle ölçeklenir,
    /// min(0,12; 0,01 + 0,0008 × nüfus): 8 kişilik kamp ≈ 0,016, 40 kişilik kasaba ≈ 0,042, 140+ kişilik şehir 0,12.
    /// (Eskiden medeniyetin herhangi bir yerleşiminde darphane varsa medeniyet başına tek 0,12 sayılırdı.)
    /// </summary>
    public static double MintIncome(Sim s, Settlement st)
    {
        double n = st.Civics.Get("mint") ?? 0;
        if (!st.Alive || !(n > 0)) return 0;
        return n * JsMath.Min(MINT_CAP, MINT_BASE + MINT_PER_POP * s.Pop(st));
    }

    /// <summary>Recomputes the civ's prices: food by food need, other goods by demand vs stock (desires ×1.3).</summary>
    public static void UpdatePrices(Sim s, Civ c)
    {
        double P = s.CivPop(c);
        double foodNeed = P * Sim.FOOD_PER_POP * 70;
        double foodHave = s.FoodTotal(c);
        double fr = JsMath.Min(6, JsMath.Max(0.15, (foodNeed + 5) / (foodHave + 5)));
        var desires = new HashSet<string>(D.CLASSES[c.Cls].Desires);
        foreach (var g in D.GOOD_IDS)
        {
            if (g == "gold") { c.Price.Set("gold", 1); continue; }
            double p;
            if (J.T(D.GOODS[g].Food)) p = D.GOODS[g].Base * fr;
            else
            {
                double dm = Demand(s, c, g, P); // TS local `D`
                p = D.GOODS[g].Base * JsMath.Min(5, JsMath.Max(0.05, (dm + 3) / (s.St(c, g) + 3)));
            }
            if (desires.Contains(g)) p *= 1.3;
            c.Price.Set(g, p);
        }
    }

    /// <summary>yanan yapıların ve evlerin onarımı: süre dolunca odun harcanarak yeniden kurulur</summary>
    public static void RepairTick(Sim s, Civ c)
    {
        foreach (var st in s.CivSettlements(c))
        {
            if ((st.BurnedHouses ?? 0) > 0 && s.Day - (st.BurnedAt ?? 0) > ((st.Imar ?? 0) >= IMAR_REPAIR ? 10 : 20) && s.St(c, "wood") >= 4)   // C3: bayındır kent çabuk onarılır
            {
                s.Add(c, "wood", -4); st.BurnedHouses = (st.BurnedHouses ?? 0) - 1; st.BurnedAt = s.Day - 10;
            }
        }
        var tiles = s.W.Tiles;
        for (int i = 0; i < tiles.Count; i++)
        {
            var t = tiles[i];
            var e = t.Ext;
            if (e == null || !J.T(e.Burned) || s.Day < e.Burned || t.Owner < 0 || s.Settlement(t.Owner)?.Civ != c.Id) continue;
            double need = 6 + e.Level * 4;
            if (s.St(c, "wood") < need) { e.Burned = s.Day + 10; continue; }
            s.Add(c, "wood", -need);
            e.Burned = null; e.BurnedAt = null;
            var st = s.Settlement(t.Owner);
            // tile: s.w.tiles.indexOf(t) — every Tile object occurs once, so that is i
            s.Log("build", $"{Tr.Ek(st.Name, "in")} yanan {D.ExtPoss(e.Kind, e.Level)} yeniden kuruldu.", civ: c.Id, tile: i);
        }
    }

    private static readonly string[] FOOD_ORDER = { "bread", "fish", "meat", "grain" };
    private static readonly string[] SPOIL_ORDER = { "grain", "meat", "fish", "bread" };
    private static readonly double[] MILESTONES = { 25, 50, 100, 200 };

    /// <summary>Daily economy of a civ: jobs and production, consumption/spoilage, growth or famine, deposit regen.</summary>
    public static void EconomyTick(Sim s, Civ c)
    {
        var ss = s.CivSettlements(c);
        if (ss.Count == 0) { s.Extinct(c); return; }
        if (s.Day % 5 == 0 || !J.T(c.Price.Get("grain"))) UpdatePrices(s, c);
        double totalPop = 0;
        foreach (var x in ss) totalPop = totalPop + s.Pop(x);
        double daysFood = s.FoodTotal(c) / JsMath.Max(0.1, totalPop * Sim.FOOD_PER_POP);
        double foodUrg = daysFood < 20 ? 4 : daysFood < 45 ? 1.8 : daysFood > 180 ? 0.6 : 1;
        // ihtiyacın çok üstünde biriken mal için kimse çalışmaz (yiyecek ve altın hariç)
        bool glut(string g) => IsGlut(s, c, g, totalPop);
        var W = s.W;

        foreach (var st in ss)
        {
            double P = s.Pop(st);
            if (P <= 0) { s.Abandon(st, "Yerleşimde kimse kalmadı"); continue; }
            var jobs = new JsObj<double>();
            bool famine = daysFood < 12;
            // kıtlıkta askerler de toplayıcılığa çıkar
            double avail = famine ? P : JsMath.Max(0, P - st.Soldiers);
            jobs.Set("asker", famine ? 0 : st.Soldiers);

            var opts = new List<JobOpt>();
            // çıkarma yapıları
            var myTiles = new List<int>();
            double forestGrass = 0;
            foreach (int ti in s.G.Within(st.Tile, s.RadiusOf(st)))
            {
                var t = W.Tiles[ti];
                if (t.Owner != st.Id) continue;
                myTiles.Add(ti);
                if (t.Terrain == "forest" || t.Terrain == "grass" || t.Terrain == "tundra" || t.Terrain == "oldforest" || t.Terrain == "hill" || t.Terrain == "swamp") forestGrass++;
                if (t.Ext == null || !s.ExtWorking(t)) { if (t.Ext != null) t.Ext.Workers = 0; continue; }
                t.Ext.Workers = 0;
                var (good, y) = ExtractYield(s, c, t, tier: st.Tier);
                double v = y * s.Price(c, good);
                if (t.Ext.Kind == "hunt") v += y * (t.Terrain == "tundra" ? D.HIDE_TUNDRA : D.HIDE) * s.Price(c, "leather");
                if (t.Ext.Kind == "claypit") v -= y * D.BRICK_FUEL * s.Price(c, "wood");
                if (J.T(D.GOODS[good].Food)) v *= foodUrg;
                if (s.St(c, good) > 150 + totalPop * 12 || glut(good)) v *= 0.08; // depolar dolu ya da ihtiyaçtan fazlası var
                var tile = t;
                int tIdx = ti;
                opts.Add(new JobOpt { Key = good, Slots = D.LEVEL_SLOTS[t.Ext.Level], V = v, Run = n => { tile.Ext.Workers = n; ProduceTile(s, c, tIdx, tile, good, y * n); } });
            }
            // toplayıcılar
            double gy = 0.14 * WILD_YIELD * (Drought(s) ? 1 - DROUGHT_FARM : 1) * (1 + s.E(c, "prodFood") * 0.5);
            opts.Add(new JobOpt { Key = "toplayıcı", Slots = JsMath.Min(10, forestGrass), V = gy * s.Price(c, "grain") * foodUrg * 0.9 + 0.03 * s.Price(c, "wood"), Run = n => { s.Add(c, "grain", gy * n); s.Add(c, "wood", 0.03 * n); } });
            // atölyeler
            foreach (var k in D.WORKSHOP_IDS)
            {
                double cnt = st.Workshops.Get(k) ?? 0;
                if (!J.T(cnt)) continue;
                var def = D.WORKSHOPS[k];
                var inp = PickInputs(s, c, def.Inputs, Sim.WorkshopAltOk(st, def) ? def.Alt : null);
                if (inp == null) continue;
                double margin = (s.Price(c, def.Output) - s.Worth(c, inp)) * (s.St(c, def.Output) > 150 + totalPop * 12 || glut(def.Output) ? 0.08 : 1);
                double perW = def.Rate * (1 + (s.E(c, "prodAll") + TIER_PROD[st.Tier]) * 0.5);
                opts.Add(new JobOpt
                {
                    Key = def.Name, Slots = Math.Floor(def.Slots * cnt * WS_TIER[st.Tier]), V = perW * JsMath.Max(0.05, margin), Run = n =>
                    {
                        double outp = perW * n;
                        foreach (var g in inp.Keys()) outp = JsMath.Min(outp, s.St(c, g) / (inp.Get(g) ?? 1));
                        if (outp <= 0) return;
                        foreach (var g in inp.Keys()) s.Add(c, g, -(inp.Get(g) ?? 0) * outp);
                        s.Add(c, def.Output, outp);
                    },
                });
            }
            // inşaatçılar
            if (st.Project != null) opts.Add(new JobOpt
            {
                Key = "inşaatçı", Slots = JsMath.Min(8, 2 + Math.Floor(P / 6)), V = daysFood < 15 ? 0.3 : daysFood < 30 ? 1.2 : 2.5, Run = n =>
                {
                    st.Project.Left -= n * (s.St(c, "tools") > 1 ? 1.25 : 1);
                    if (st.Project.Left <= 0) FinishProject(s, c, st);
                },
            });
            J.Sort(opts, (a, b) => b.V - a.V);
            foreach (var o in opts)
            {
                if (avail <= 0) break;
                if (o.V < 0.035 && o.Key != "inşaatçı") continue;
                double n = JsMath.Min(avail, o.Slots);
                if (n <= 0) continue;
                avail -= n;
                jobs.Set(o.Key, (jobs.Get(o.Key) ?? 0) + n);
                o.Run(n);
            }
            jobs.Set("zanaatçı", avail);   // C3: boştakiler; altını ve kamu işleri PublicWorks'te
            st.Jobs = jobs;
        }

        // C3: hazine yedeğinin üstündeki altın kamu işlerine (imar) akar: boştakilerden amele tutulur; kalan boştakiler az altın getirir
        double idleGold = PublicWorks(s, c, ss, totalPop);
        // Tüketim (C3: askerin erzakı halkınkinin üstüne)
        double soldiers = 0;
        foreach (var x in ss) soldiers = soldiers + x.Soldiers;
        double need = (totalPop * Sim.FOOD_PER_POP + soldiers * SOLDIER_RATION) * (1 - JsMath.Min(0.5, s.E(c, "frugal")));
        // C3: kent tüketimi (kademe başına, kişi başı): bira ve alet; kasaba ve şehir gıdasının bir payını ekmekten ister
        double beerNeed = 0, toolNeed = 0, breadNeed = 0, leatherNeed = soldiers * SOLDIER_LEATHER;
        foreach (var st in ss)
        {
            var tn = D.TOWN_NEEDS[st.Tier];
            double P = s.Pop(st);
            beerNeed += P * (tn.Get("beer") ?? 0);
            toolNeed += P * (tn.Get("tools") ?? 0);
            breadNeed += P * Sim.FOOD_PER_POP * (tn.Get("bread") ?? 0);
            leatherNeed += P * (tn.Get("leather") ?? 0) * (s.W.Tiles[st.Tile].Terrain == "tundra" ? 1.5 : 1);   // Faz 1b-3: tundrada kürk
        }
        // ekmek önce yenir (FOOD_ORDER); kentlerin ekmek payı karşılanmıyorsa yokluk
        bool lackBread = breadNeed > 0 && s.St(c, "bread") * D.GOODS["bread"].Food.Value < breadNeed * 0.8;
        foreach (var g in FOOD_ORDER)
        {
            double fv = D.GOODS[g].Food.Value;
            double have = s.St(c, g) * fv;
            double use = JsMath.Min(have, need);
            s.Add(c, g, -use / fv);
            need -= use;
            if (need <= 1e-6) break;
        }
        bool salted = s.St(c, "salt") >= 1;
        if (salted) s.Add(c, "salt", -totalPop * 0.0004);
        double spoil = salted ? 0.0012 : 0.0025;
        foreach (var g in SPOIL_ORDER) s.Add(c, g, -s.St(c, g) * spoil);
        // depo sınırı: kapasiteyi aşan mal yavaşça çürür, çalınır ya da dağılır
        double capStore = 150 + totalPop * 12;
        foreach (var g in c.Stock.Keys())
        {
            double v = c.Stock.Get(g) ?? 0;
            if (v > capStore && g != "gold") s.Add(c, g, -(v - capStore) * 0.01);
        }
        // C3: bira kentlerde daha çok içilir (eskiden herkes 0,004); yetmezse meyhaneler kurur
        double happy = 0;
        bool lackBeer = false;
        if (beerNeed > 0)
        {
            double haveBeer = s.St(c, "beer");
            if (haveBeer >= beerNeed) { s.Add(c, "beer", -beerNeed); happy = 0.15; }
            else { s.Add(c, "beer", -haveBeer); lackBeer = haveBeer < beerNeed * 0.8; }
        }
        // L2–L3 yapılar aleti aşındırır (L3 iki kat; Kasaba+ yerleşimin demir aletleri yarı hızda) ve altın bakımı ister
        double l2 = 0, l3 = 0, wear = 0;
        for (int i = 0; i < W.Tiles.Count; i++)
        {
            var t = W.Tiles[i];
            if (t.Ext == null || t.Ext.Level < 2 || t.Owner < 0) continue;
            var ow = J.Find(ss, x => x.Id == t.Owner);
            if (ow == null) continue;
            if (t.Ext.Level >= 3) l3++; else l2++;
            wear += EXT_TOOLS[Math.Min(3, t.Ext.Level)] * (ow.Tier >= Gate.IRON_TOOLS ? 0.5 : 1);
        }
        if (J.T(l2 + l3)) s.Add(c, "tools", -wear);
        // C3: köy, kasaba ve şehir zanaatkârları alet tüketir
        bool lackTools = false;
        if (toolNeed > 0)
        {
            double haveTools = s.St(c, "tools");
            if (haveTools >= toolNeed) s.Add(c, "tools", -toolNeed);
            else { s.Add(c, "tools", -haveTools); lackTools = haveTools < toolNeed * 0.8; }
        }
        // Faz 1b-3: deri: kentlerin koşum ve ayakkabısı, askerin teçhizat bakımı (eski kış giysisinin yerine; yokluğu cezasız)
        if (leatherNeed > 0) s.Add(c, "leather", -JsMath.Min(leatherNeed, s.St(c, "leather")));
        // yokluk ancak malı yapan zanaatı olan medeniyette sayılır (bir yerleşiminde fırın, bira evi, aletçi): zanaatı olmayan halk
        // onu aramaz (Faz 1b-3: eskiden düğüm; köyler artık bira evinden önce kurulabiliyor)
        lackBread = lackBread && HasWorkshop(ss, "bakery");
        lackBeer = lackBeer && HasWorkshop(ss, "brewery");
        lackTools = lackTools && HasWorkshop(ss, "toolmaker");
        foreach (var st in ss)
        {
            var tn = D.TOWN_NEEDS[st.Tier];
            SetLack(st, "bread", lackBread && J.T(tn.Get("bread") ?? 0));
            SetLack(st, "beer", lackBeer && st.Tier >= 1 && J.T(tn.Get("beer") ?? 0));
            SetLack(st, "tools", lackTools && J.T(tn.Get("tools") ?? 0));
        }
        LackLog(s, c, ss);
        Gear.GearTick(s, c);
        double mint = 0;
        foreach (var x in ss) mint += MintIncome(s, x);
        double income = totalPop * 0.004 * (1 + s.E(c, "tax") + TIER_TAX[s.CivTier(c)]) + mint;   // Faz 1b-3: kademe vergisi
        s.Add(c, "gold", income);
        // C3: bakım: asker maaşı ve L2–L3 yapılar (kahraman maaşı 30 günde bir: PayHeroes)
        double extGold = l2 * EXT_GOLD[2] + l3 * EXT_GOLD[3];
        double upkeep = soldiers * SOLDIER_PAY + extGold;
        var bud = c.Budget ??= new CivBudget();
        bud.Income = income + idleGold; bud.Soldiers = soldiers * SOLDIER_PAY; bud.Buildings = extGold;
        bud.Heroes = 0;
        foreach (var h in s.CivHeroes(c)) bud.Heroes += HeroWage(h) * 4 / Sim.YEAR;
        bud.Upkeep = upkeep + bud.Heroes;
        PayUpkeep(s, c, ss, upkeep);
        if (s.Day % 30 == 0) PayHeroes(s, c);

        if (need > 0.01)
        {
            // C3: kıtlık medeniyetin tek büyük olayıdır (yerleşim başına "kıtlık başladı" akışı yerine)
            var fam = FamineDay(s, c, need, totalPop);
            foreach (var st in s.CivSettlements(c))
            {
                st.Starving++;
                // açlık, açığın büyüklüğüyle orantılı birikir
                st.Hunger = (st.Hunger ?? 0) + JsMath.Min(1, need / JsMath.Max(0.05, totalPop * Sim.FOOD_PER_POP));
                // Faz 1b-4: çekirdek şehir (bir kez Şehir olmuş) açlıktan Sim.CORE_MIN'in altına inmez: büyük şehir küçülür ama terk edilmez
                if (st.Hunger >= 10 && !(Sim.IsCore(st) && s.Pop(st) <= Sim.CORE_MIN)) { st.Hunger -= 10; s.RemovePop(st, 1); s.Metric("starved"); fam.Dead++; }
            }
        }
        else
        {
            if (c.Famine != null) FamineOk(s, c);
            foreach (var st in s.CivSettlements(c))
            {
                st.Starving = 0; st.Hunger = 0;
                double P = s.Pop(st);
                double cap = s.Housing(st);
                if (P < cap && daysFood > 8)
                {
                    double rate = 0;
                    foreach (var kv in st.Pop) rate += kv.Value * D.RACES[kv.Key].Growth;
                    double crowd = JsMath.Max(0.05, 1 - P / Sim.TIER_CROWD[st.Tier]) / (1 + JsMath.Max(0, totalPop - 60) / 70);
                    st.GrowthAcc += rate * 0.0058 * (daysFood > 30 ? 1 : 0.5) * crowd * (1 + s.E(c, "growth") + TIER_GROWTH[st.Tier] + happy) * LackGrowth(st) * ImarGrowth(st); // C3: kentte yokluk, imar; Faz 1b-3: kademe
                    while (st.GrowthAcc >= 1)
                    {
                        st.GrowthAcc -= 1;
                        var rs = J.Filter(st.Pop.Keys(), r => (st.Pop.Get(r) ?? 0) > 0);
                        s.AddPop(st, s.Rng.Weighted(rs, x => (st.Pop.Get(x) ?? 0) * D.RACES[x].Growth) ?? c.Race, 1);
                    }
                }
                if (s.Rng.Chance(P * 0.00025) && !(Sim.IsCore(st) && P <= Sim.CORE_MIN)) s.RemovePop(st, 1);   // Faz 1b-4: çekirdek şehir yaşlılıktan da tükenmez
            }
        }

        // Yenilenen yataklar ve orman
        if (s.Day % 10 == 0)
        {
            foreach (var d in W.Deposits)
            {
                double? regen = D.DEPOSITS[d.Kind].Regen;
                if (!J.T(regen)) continue;
                double max = D.DEPOSITS[d.Kind].Reserve;
                foreach (int ti in d.Tiles) W.Tiles[ti].Reserve = JsMath.Min(max, W.Tiles[ti].Reserve + regen.Value * 10);
            }
        }
        double tp = s.CivPop(c);
        foreach (double m in MILESTONES) if (tp >= m && c.Stats.PeakPop < m) s.Log("growth", $"{c.Name} nüfusu {J.S(m)} kişiye ulaştı.", civ: c.Id, major: m >= 50);
        c.Stats.PeakPop = JsMath.Max(c.Stats.PeakPop, tp);
    }

    /// <summary>Medeniyetin herhangi bir yerleşiminde bu atölye var mı.</summary>
    private static bool HasWorkshop(List<Settlement> ss, string k)
    {
        foreach (var x in ss) if ((x.Workshops.Get(k) ?? 0) > 0) return true;
        return false;
    }

    // ------------------------------------------------------------ C3: kent tüketiminde yokluk
    /// <summary>Yerleşimin yokluk sayacı: yoksa bir gün artar, varsa silinir (hiç yokluk kalmayınca Lack null olur).</summary>
    private static void SetLack(Settlement st, string g, bool on)
    {
        if (on) { st.Lack ??= new JsObj<double>(); st.Lack.Set(g, (st.Lack.Get(g) ?? 0) + 1); }
        else if (st.Lack != null && st.Lack.Delete(g) && st.Lack.Count == 0) st.Lack = null;
    }

    /// <summary>Yerleşimde en az <paramref name="days"/> gündür süren yokluk sayısı (food: yalnız ekmek ve bira).</summary>
    public static int LackCount(Settlement st, double days, bool food = false)
    {
        if (st.Lack == null) return 0;
        int n = 0;
        foreach (var kv in st.Lack) if (kv.Value >= days && (!food || kv.Key != "tools")) n++;
        return n;
    }

    /// <summary>Kentte süren her yokluk büyümeyi yavaşlatır (bkz. LACK_GROWTH).</summary>
    private static double LackGrowth(Settlement st)
    {
        if (st.Lack == null) return 1;
        double f = 1;
        foreach (var kv in st.Lack) if (kv.Value >= LACK_DAYS) f -= LACK_GROWTH.TryGetValue(kv.Key, out double g) ? g : 0.1;
        return JsMath.Max(0.5, f);
    }

    private static readonly Dictionary<string, string> LACK_TEXT = new()
    {
        ["bread"] = "{0} kentlerinde fırınlar soğudu: halk ekmek bulamıyor.",
        ["beer"] = "{0} meyhaneleri kurudu: kentlerde bira kalmadı.",
        ["tools"] = "{0} zanaatkârları alet bulamıyor; tezgâhlar boş.",
    };

    /// <summary>Bir mal kentlerde LACK_LOG gündür yoksa akışa düşer: yokluğun başında bir kez (medeniyet başına LACK_LOG_GAP yılda en
    /// çok bir, mal başına); süregelen yokluk her yıl yeniden yazılmaz.</summary>
    private static void LackLog(Sim s, Civ c, List<Settlement> ss)
    {
        foreach (var g in LACK_GOODS)
        {
            int n = 0; bool start = false;
            foreach (var x in ss) { double d = x.Lack?.Get(g) ?? 0; if (d >= LACK_LOG) n++; if (d == LACK_LOG) start = true; }
            if (!start || s.Year - (c.Yearly.Get("lack_" + g) ?? -99) < LACK_LOG_GAP) continue;
            c.Yearly.Set("lack_" + g, s.Year);
            s.Metric("lack_" + g);
            string why = g == "bread" ? "Kasabalılar gıdasının bir payını ekmekten ister; fırınlar ya da tahıl yetmiyor"
                : g == "beer" ? "Bira evleri kentlerin susuzluğuna yetişemiyor"
                : (s.St(c, "copper") < 2 || s.St(c, "tin") < 1) && s.St(c, "iron") < 2 ? "Bakır, kalay ve demir tükendi; aletçiler boş oturuyor" : "Aletçiler kentlerin aşındırdığı aletlere yetişemiyor";
            s.Log("economy", LACK_TEXT[g].Replace("{0}", c.Name), civ: c.Id, cause: $"{why}; {J.S(n)} yerleşimde büyüme yavaşladı{(g == "tools" ? "" : ", halk huzursuz")}");
        }
    }

    // ------------------------------------------------------------ C3: bakım ve hazine
    /// <summary>Kahramanın 30 günlük maaşı (yıllık HERO_WAGE + HERO_WAGE_LV × seviye'nin dörtte biri, yuvarlanmış).</summary>
    public static double HeroWage(Hero h) => JsMath.Round((HERO_WAGE + HERO_WAGE_LV * h.Level) / 4);

    /// <summary>Günlük bakımı öder. Hazine yetmezse ödeyebildiğini öder ve medeniyet "hazinesi boş" olur: ödenemeyen her 10 günde
    /// askerlerin bir payı firar eder, L2+ yapılar aksar (BROKE_EXT), yeni asker yazılmaz, kahramanlar ayrılmaya başlar.
    /// Hazine BROKE_RESERVE günlük bakım biriktirince boşluk biter.</summary>
    private static void PayUpkeep(Sim s, Civ c, List<Settlement> ss, double upkeep)
    {
        double gold = s.St(c, "gold");
        if (gold >= upkeep)
        {
            s.Add(c, "gold", -upkeep);
            s.Metric("upkeepGold", upkeep);
            if (c.Broke != null && gold - upkeep >= JsMath.Max(20, (c.Budget?.Upkeep ?? upkeep) * BROKE_RESERVE))
            {
                double days = s.Day - c.Broke.Value;
                c.Broke = null;
                s.Log("economy", $"{c.Name} hazinesi yeniden doldu; maaşlar ödeniyor.", civ: c.Id, cause: $"{J.S(days)} gün süren darlık bitti");
            }
            return;
        }
        if (gold > 0) { s.Add(c, "gold", -gold); s.Metric("upkeepGold", gold); }
        if (c.Broke == null)
        {
            c.Broke = s.Day;
            s.Metric("broke");
            var b = c.Budget;
            s.Log("economy", $"{c.Name} hazinesi tükendi: askerlerin ve kahramanların maaşı ödenemiyor.", civ: c.Id, major: true,
                cause: b != null ? $"Günlük gelir {J.S(JsMath.Round(b.Income * 10) / 10)}, bakım {J.S(JsMath.Round(b.Upkeep * 10) / 10)} altın; askerler firar ediyor, yapılar bakımsız" : "Askerler firar ediyor, yapılar bakımsız");
        }
        // firar: maaşı ödenmeyen askerlerin bir payı silahı bırakıp işine döner
        if ((s.Day + c.Id) % 10 == 0)
            foreach (var st in ss)
                if (st.Soldiers > 0) { double n = JsMath.Max(1, Math.Floor(st.Soldiers * DESERT)); st.Soldiers -= n; s.Metric("deserted", n); }
    }

    /// <summary>30 günde bir: medeniyete bağlı kahramanların maaşı (kahramanın kesesine). Maaşı art arda HERO_UNPAID kez
    /// ödenmeyen (ya da hazinesi bir aydır boş medeniyetin) kahramanı yurttaysa hizmetten ayrılır (sözleşmeli olan hanına, öteki
    /// tavernasına döner).</summary>
    private static void PayHeroes(Sim s, Civ c)
    {
        foreach (var h in s.CivHeroes(c))
        {
            double w = HeroWage(h);
            if (s.St(c, "gold") >= w)
            {
                s.Add(c, "gold", -w); h.Gold += w; h.Unpaid = null;
                s.Metric("upkeepGold", w); s.Metric("upkeepHero", w);
                continue;
            }
            h.Unpaid = (h.Unpaid ?? 0) + 1;
            s.Metric("heroUnpaid");
            bool broke = c.Broke != null && s.Day - c.Broke.Value >= 30;
            if ((h.Unpaid < HERO_UNPAID && !broke) || h.State != "home") continue;
            double seasons = h.Unpaid.Value;
            h.Civ = -1; h.Contract = null; h.Unpaid = null;
            s.Metric("heroQuit");
            Will.Note(s, h, $"{c.Name} maaşını ödeyemeyince hizmetten ayrıldı");
            s.Log("hero", $"{h.Name}, maaşı ödenmeyince {Tr.Ek(c.Name, "in")} hizmetinden ayrıldı.", civ: c.Id, tile: h.Pos, major: true,
                cause: broke ? $"{c.Name} hazinesi {J.S(s.Day - c.Broke.Value)} gündür boş" : $"{J.S(seasons)} aydır maaş alamadı; hazinede {J.S(Math.Floor(s.St(c, "gold")))} altın var");
            Will.ReturnToBase(s, h);
        }
    }

    // ------------------------------------------------------------ C3: kıtlık (tek büyük olay)
    private static readonly HashSet<string> GENEROUS = new() { "cleric", "paladin", "druid", "monk", "bard" };

    /// <summary>Gıda açığı olan gün: kıtlık kaydı açılır ya da sürer; açık FAMINE_DECLARE gün sürünce ve ortalama açık ihtiyacın
    /// FAMINE_SHORT payını geçince büyük olay olarak ilan edilir, komşulara yardım çağrısı gider (AID_GAP günde bir, en çok AID_ROUNDS
    /// kez). Ambar FAMINE_END gün art arda yetmeden kıtlık bitmez (arada tekrarlayan açık aynı kıtlıktır).</summary>
    private static FamineState FamineDay(Sim s, Civ c, double need, double totalPop)
    {
        var f = c.Famine ??= new FamineState { Since = s.Day, NextAid = s.Day };
        f.Days++; f.OkDays = 0;
        f.Short += JsMath.Min(1, need / JsMath.Max(0.05, totalPop * Sim.FOOD_PER_POP));
        s.Metric("famineDays");
        if (!f.Declared && f.Days >= FAMINE_DECLARE && f.Short / f.Days >= FAMINE_SHORT)
        {
            f.Declared = true;
            f.Why = FamineWhy(s, c);
            s.Metric("famine");
            int n = s.CivSettlements(c).Count;
            s.Log("economy", $"KITLIK! {c.Name} ambarları boşaldı; {(n > 1 ? $"{J.S(n)} yerleşimde" : "yurtta")} halk aç.", civ: c.Id, tile: s.Capital(c)?.Tile, major: true,
                cause: $"{f.Why}; günde {J.S(JsMath.Round(need * 10) / 10)} gıda eksik");
        }
        if (f.Declared && s.Day >= f.NextAid && f.Rounds < AID_ROUNDS) FamineAid(s, c, f, totalPop);
        return f;
    }

    /// <summary>Kıtlığın nedeni: yanan tarlalar, kuşatma ve savaş, kuraklık, salgın, yoksa ambarın nüfusa yetmemesi.</summary>
    private static string FamineWhy(Sim s, Civ c)
    {
        var why = new List<string>();
        int burned = 0;
        foreach (var t in s.W.Tiles) if (t.Ext != null && t.Ext.Kind == "farm" && J.T(t.Ext.Burned) && t.Owner >= 0 && s.Settlement(t.Owner)?.Civ == c.Id) burned++;
        if (burned > 0) why.Add($"{J.S(burned)} tarla yanmış");
        if (s.InWar(c)) why.Add("savaş tarlaları boş bıraktı");
        if (Drought(s)) why.Add("kuraklık tarlaları kuruttu");
        if (J.Some(s.CivSettlements(c), x => x.Plague != null)) why.Add("salgın çiftçileri yatağa düşürdü");
        if (why.Count == 0) why.Add("ambarlar büyüyen nüfusa yetmedi");
        return J.TrCap(string.Join(", ", why));
    }

    /// <summary>Ambarın yettiği gün: FAMINE_END gün art arda tok geçince kıtlık biter (ilan edildiyse kroniğe düşer).</summary>
    private static void FamineOk(Sim s, Civ c)
    {
        var f = c.Famine;
        f.OkDays++;
        if (f.OkDays < FAMINE_END) return;
        c.Famine = null;
        if (!f.Declared) return;
        s.Metric("famineEnd");
        var helped = J.Map(J.Filter(f.Helped, id => id >= 0 && id < s.W.Civs.Count), id => s.W.Civs[id].Name);
        var refused = J.Map(J.Filter(f.Refused, id => id >= 0 && id < s.W.Civs.Count), id => s.W.Civs[id].Name);
        string aid = helped.Count > 0 ? $"yardım edenler: {Lore.JoinVe(J.Unique(helped))} ({J.S(JsMath.Round(f.AidFood))} gıda)" : "kimse yardıma gelmedi";
        if (refused.Count > 0) aid += $"; yüz çevirenler: {Lore.JoinVe(J.Unique(refused))}";
        double span = JsMath.Max(f.Days, s.Day - FAMINE_END - f.Since + 1);   // ilk açıktan son açığa
        s.Log("economy", $"{c.Name} kıtlığı atlattı: {J.S(span)} gün sürdü ({J.S(f.Days)} gün aç), {(f.Dead > 0 ? $"{J.S(f.Dead)} kişi açlıktan öldü" : "açlıktan ölen olmadı")}.", civ: c.Id, tile: s.Capital(c)?.Tile,
            major: f.Dead > 0 || f.Days >= 20, cause: J.TrCap(aid));
    }

    /// <summary>
    /// Kıtlıkta komşulara çağrı (en dost olandan başlayarak; en çok AID_HELPERS yardım eder): temastaki, savaşta olmayan, kendisi aç
    /// olmayan ve kendine AID_KEEP günlük gıda ayırdıktan sonra fazlası olan her medeniyet karar verir: ilişki, iyilik, cömert sınıf
    /// (rahip, paladin, druid, keşiş, ozan) ve kötülük olasılığı belirler. Yardım eden en çok AID_DAYS günlük gıda yollar (fazlasının
    /// %35'i), aç medeniyet ona minnet duyar (+20); yüz çevirene kin tutar (−12) ve bu kıtlıkta bir daha sormaz.
    /// </summary>
    private static void FamineAid(Sim s, Civ c, FamineState f, double totalPop)
    {
        f.Rounds++;
        f.NextAid = s.Day + AID_GAP;
        double perDay = JsMath.Max(1, totalPop * Sim.FOOD_PER_POP);
        var helped = new List<string>();
        var refused = new List<string>();
        var asked = J.Sort(J.Filter(s.W.Civs, o => o.Alive && o.Id != c.Id && s.Rel(o.Id, c.Id).Contact && !s.AtWar(o.Id, c.Id) && !(o.Famine != null && o.Famine.Declared) && !f.Refused.Contains(o.Id)),
            (x, y) => J.Or(s.RelValue(y.Id, c.Id) - s.RelValue(x.Id, c.Id), x.Id - y.Id));
        foreach (var o in asked)
        {
            if (helped.Count >= AID_HELPERS) break;
            double surplus = s.FoodTotal(o) - s.CivPop(o) * Sim.FOOD_PER_POP * AID_KEEP;
            if (surplus < perDay * 5) continue;   // verecek fazlası yok: kimse onu suçlamaz
            double rel = s.RelValue(o.Id, c.Id);
            double p = 0.3 + rel / 100 + o.Align.Good * 0.3 + (GENEROUS.Contains(o.Cls) ? 0.15 : 0) - (Diplomacy.IsEvil(s, o) ? 0.25 : 0);
            if (!s.Rng.Chance(JsMath.Max(0.05, JsMath.Min(0.95, p))))
            {
                f.Refused.Add(o.Id);
                s.AddMod(c.Id, o.Id, "noaid", "Kıtlıkta yüz çevirdi", -12, -24, 0.01, false);
                s.Metric("famineRefused");
                refused.Add(o.Name);
                continue;
            }
            double amount = Math.Floor(JsMath.Min(surplus * 0.35, perDay * AID_DAYS));
            double left = amount;
            // en bol gıdadan başlayarak yüklenir
            foreach (var g in J.Sorted(FOOD_KINDS, (x, y) => J.Or(s.St(o, y) * D.GOODS[y].Food.Value - s.St(o, x) * D.GOODS[x].Food.Value, FOOD_KINDS.IndexOf(x) - FOOD_KINDS.IndexOf(y))))
            {
                if (left <= 0) break;
                double fv = D.GOODS[g].Food.Value;
                double q = Math.Floor(JsMath.Min(s.St(o, g), left / fv));
                if (q <= 0) continue;
                s.Add(o, g, -q); s.Add(c, g, q);
                left -= q * fv;
            }
            double sent = amount - JsMath.Max(0, left);
            if (sent <= 0) continue;
            f.AidFood += sent;
            if (!f.Helped.Contains(o.Id)) f.Helped.Add(o.Id);
            s.AddMod(c.Id, o.Id, "aid", "Kıtlıkta uzanan el", 20, 30, 0.005, false);
            s.AddMod(o.Id, c.Id, "aidgiven", "Kıtlıkta yardım ettiğimiz halk", 5, 10, 0.01, false);
            s.Metric("famineAid"); s.Metric("famineAidFood", sent);
            helped.Add($"{o.Name} ({J.S(JsMath.Round(sent))} gıda)");
        }
        if (helped.Count == 0 && refused.Count == 0) return;
        string text = helped.Count > 0 && refused.Count > 0 ? $"{Lore.JoinVe(helped)} aç {c.Name} halkına erzak kervanı yolladı; {Lore.JoinVe(refused)} yüz çevirdi."
            : helped.Count > 0 ? $"{Lore.JoinVe(helped)} aç {c.Name} halkına erzak kervanı yolladı."
            : $"{c.Name} kıtlıkta komşularından yardım istedi; {Lore.JoinVe(refused)} yüz çevirdi.";
        s.Log("diplomacy", text, civ: c.Id, tile: s.Capital(c)?.Tile, major: true,
            cause: helped.Count > 0 ? "Minnet unutulmaz; kapısını kapatanlar da unutulmaz" : "Ambarları dolu komşular kapılarını kapattı");
    }

    private static readonly List<string> FOOD_KINDS = new() { "grain", "fish", "meat", "bread" };

    // ------------------------------------------------------------ C3: kamu işleri (imar)
    /// <summary>
    /// Kamu işleri (imar). Hazine yedeğini (PW_RESERVE + PW_RESERVE_DAYS günlük bakım) aşan altının yılda PW_SPEND payı kamu işlerine
    /// akar; altın yetmezse ambarın fazlası (PW_FOOD_KEEP günlük gıdanın üstü, yılda PW_FOOD_SPEND payı) amele tayını olur (angarya).
    /// Yerleşimlerdeki boştakilerden (zanaatçı) nüfusun PW_SLOTS payına dek amele tutulur: önce altınla (amele başı en az AMELE_WAGE),
    /// sonra ambarla (amele başı AMELE_FOOD gıda). Her amele günde AMELE_MAT yapı malı (taş, kereste ya da tuğla; en bol olandan) harcar;
    /// malzeme yoksa iş durur. Kıtlıkta ve hazine boşken durur. Harcanan altın (ve ambarla beslenen amele başı AMELE_WAGE değeri)
    /// yerleşimin imarını yükseltir (kişi başı × IMAR_RATE; günde IMAR_DECAY payı söner). Kalan boştakiler IDLE_GOLD getirir; dönen
    /// değer boştakilerin altınıdır (bütçe için).
    /// </summary>
    private static double PublicWorks(Sim s, Civ c, List<Settlement> ss, double totalPop)
    {
        double upk = c.Budget?.Upkeep ?? 0;
        double reserve = PW_RESERVE + upk * PW_RESERVE_DAYS;
        bool stop = (c.Famine != null && c.Famine.Declared) || c.Broke != null;
        double budget = stop ? 0 : JsMath.Max(0, s.St(c, "gold") - reserve) * PW_SPEND / Sim.YEAR;
        double foodBudget = stop ? 0 : JsMath.Max(0, s.FoodTotal(c) - totalPop * Sim.FOOD_PER_POP * PW_FOOD_KEEP) * PW_FOOD_SPEND / Sim.YEAR;
        double hireGold = Math.Floor(budget / AMELE_WAGE);
        // yapı malı: amele başı AMELE_MAT taş, kereste ya da tuğla (ambarda ne kadar varsa)
        double mats = s.St(c, "stone") + s.St(c, "wood") + s.St(c, "bricks");
        double cap = 0;
        foreach (var st in ss) if (st.Alive) cap += JsMath.Min(st.Jobs.Get("zanaatçı") ?? 0, Math.Floor(s.Pop(st) * PW_SLOTS[st.Tier]));
        double hire = JsMath.Min(cap, JsMath.Min(hireGold + Math.Floor(foodBudget / AMELE_FOOD), Math.Floor(mats / AMELE_MAT)));
        double ratio = cap > 0 ? hire / cap : 0;
        // amele yerleşimlere boştakilerinin (yuva payına dek) oranıyla dağıtılır
        var hired = new List<double>();
        double used = 0;
        foreach (var st in ss)
        {
            double a = 0;
            if (st.Alive && hire > 0)
            {
                double z = st.Jobs.Get("zanaatçı") ?? 0;
                a = Math.Floor(JsMath.Min(z, Math.Floor(s.Pop(st) * PW_SLOTS[st.Tier])) * ratio + 1e-9);
            }
            hired.Add(a);
            used += a;
        }
        // önce altınla tutulur (bütçenin tamamı harcanır), kalanı ambar besler
        double fed = JsMath.Max(0, used - hireGold);
        double spend = used > fed ? budget : 0;
        double worth = spend + fed * AMELE_WAGE;   // imara giden değer (altın cinsinden)
        double idleGold = 0;
        for (int i = 0; i < ss.Count; i++)
        {
            var st = ss[i];
            if (!st.Alive) continue;
            double a = hired[i];
            double z = st.Jobs.Get("zanaatçı") ?? 0;
            if (a > 0)
            {
                st.Jobs.Set("zanaatçı", z - a);
                st.Jobs.Set("amele", a);
                z -= a;
            }
            idleGold += z * IDLE_GOLD;
            double P = s.Pop(st);
            double im = (st.Imar ?? 0) * (1 - IMAR_DECAY) + (a > 0 ? IMAR_RATE * worth * (a / used) / JsMath.Max(10, P) : 0);
            st.Imar = im >= 0.05 ? JsMath.Min(100, im) : null;
            if ((st.Imar ?? 0) >= IMAR_LOG && !J.T(st.ImarLog))
            {
                st.ImarLog = true;
                s.Metric("imarTown");
                bool isCap = s.Capital(c)?.Id == st.Id;
                s.Log("build", $"{st.Name} bayındır bir {(st.Tier >= 3 ? "şehre" : st.Tier == 2 ? "kasabaya" : "köye")} dönüştü: taş döşeli yollar, çeşmeler, su kanalları.", civ: c.Id, tile: st.Tile, major: isCap,
                    cause: $"{c.Name} {(fed > 0 && spend <= 0 ? "ambarlarının fazlası yıllardır angaryaya" : "hazinesinin fazlası yıllardır imara")} akıyor; halk daha hızlı çoğalıyor, yangın yerleri çabuk onarılıyor");
            }
        }
        if (used > 0)
        {
            // yapı malı: önce en bol olandan
            double need = used * AMELE_MAT;
            foreach (var g in J.Sorted(PW_MATS, (x, y) => J.Or(s.St(c, y) - s.St(c, x), PW_MATS.IndexOf(x) - PW_MATS.IndexOf(y))))
            {
                if (need <= 0) break;
                double take = JsMath.Min(need, s.St(c, g));
                s.Add(c, g, -take); need -= take;
            }
            if (spend > 0) { s.Add(c, "gold", -spend); s.Metric("publicWorks", spend); }
            if (fed > 0)
            {
                // amele tayını: önce çabuk bozulan, ucuz gıdadan
                double ration = fed * AMELE_FOOD;
                foreach (var g in FOOD_KINDS)
                {
                    if (ration <= 1e-9) break;
                    double fv = D.GOODS[g].Food.Value;
                    double take = JsMath.Min(ration, s.St(c, g) * fv);
                    s.Add(c, g, -take / fv); ration -= take;
                }
                s.Metric("publicWorksFood", fed * AMELE_FOOD);
            }
            s.Metric("amele", used);
        }
        s.Add(c, "gold", idleGold);
        return idleGold;
    }

    private static readonly List<string> PW_MATS = new() { "stone", "wood", "bricks" };

    /// <summary>İmarın büyüme katsayısı (100'de 1 + IMAR_GROWTH).</summary>
    private static double ImarGrowth(Settlement st) => 1 + IMAR_GROWTH * (st.Imar ?? 0) / 100;

    /// <summary>İmarın huzursuzluğu azaltması (Diplomacy.Unrest): 100'de IMAR_CALM.</summary>
    public static double ImarCalm(Settlement st) => IMAR_CALM * (st.Imar ?? 0) / 100;

    /// <summary>Orman yeniden büyümesi (30 günde bir, dünya düzeyinde)</summary>
    public static void RegrowForests(Sim s)
    {
        for (int i = 0; i < s.W.Tiles.Count; i++)
        {
            var t = s.W.Tiles[i];
            if (t.CutDay == null || t.Terrain != "grass" || t.Ext != null || t.Deposit >= 0) continue;
            bool fast = s.TileTier(i) >= Gate.FORESTRY;   // Faz 1b-3: Kasaba+ yerleşimin ormancıları
            if (s.Day - t.CutDay.Value > D.FOREST_REGROW_DAYS * (fast ? 0.5 : 1)) { t.Terrain = "forest"; t.Wood = 160; t.CutDay = null; }
        }
    }

    private static void ProduceTile(Sim s, Civ c, int ti, Tile t, string good, double amount)
    {
        if (amount <= 0) return;
        string kind = t.Ext.Kind;
        if (kind == "lumber")
        {
            if (c.Cls != "druid")
            {
                t.Wood -= amount;
                if (t.Wood <= 0)
                {
                    t.Terrain = "grass"; t.CutDay = s.Day; t.Ext = null; t.Wood = 0;
                    s.ClearPaths();
                }
            }
            s.Add(c, "wood", amount);
            return;
        }
        if (kind == "hunt")
        {
            s.Add(c, "meat", amount); s.Add(c, "leather", amount * (t.Terrain == "tundra" ? D.HIDE_TUNDRA : D.HIDE)); // tundrada kürklü av
            return;
        }
        if (kind == "claypit") amount = JsMath.Min(amount, s.St(c, "wood") / D.BRICK_FUEL); // tuğla ocağı kereste yakar
        if (amount <= 0) return;
        if (kind == "dock" || kind == "pasture" || kind == "farm") { s.Add(c, good, amount); return; }
        // tükenen rezervler
        double take = JsMath.Min(amount, t.Reserve);
        s.Add(c, good, take);
        if (kind == "claypit") s.Add(c, "wood", -take * D.BRICK_FUEL);
        c.Stats.Mined.Set(good, (c.Stats.Mined.Get(good) ?? 0) + take);
        t.Reserve -= take;
        if (t.Reserve <= 0.01)
        {
            t.Reserve = 0;
            t.Ext.Depleted = true;
            if (t.Deposit >= 0)
            {
                var d = J.Find(s.W.Deposits, x => x.Id == t.Deposit);
                if (!J.T(D.DEPOSITS[d.Kind].Regen) && J.Every(d.Tiles, x => s.W.Tiles[x].Reserve <= 0))
                {
                    d.Depleted = true;
                    c.Stats.Depleted++;
                    s.Metric("depleted");
                    s.Log("economy", $"{c.Name}, {J.TrLower(D.DEPOSITS[d.Kind].Name)} yatağını tüketti.", civ: c.Id, tile: ti, major: true, cause: $"{d.Tiles.Count} hex'lik yatak boşaldı; yeni kaynak aranacak");
                }
                else if (J.T(D.DEPOSITS[d.Kind].Regen))
                {
                    t.Ext.Depleted = false; // yenilenir, beklemede
                }
            }
        }
    }

    private static JsObj<double> PickInputs(Sim s, Civ c, JsObj<double> a, JsObj<double> b = null)
    {
        bool ok(JsObj<double> x) => J.Every(x.Keys(), g => s.St(c, g) >= (x.Get(g) ?? 0));
        if (b != null && ok(b)) return b;
        if (ok(a)) return a;
        return null;
    }

    // ------------------------------------------------------------ projeler
    private static readonly Dictionary<string, string> SHIP_ACC = new() { ["kadırga"] = "kadırgasını", ["koga"] = "kogasını", ["tekne"] = "teknesini" };

    /// <summary>Completes st.Project (ship, civic, workshop, extract, upgrade) and logs it.</summary>
    public static void FinishProject(Sim s, Civ c, Settlement st)
    {
        var p = st.Project;
        st.Project = null;
        if (p.Type == "ship")
        {
            string k = p.Kind;
            bool first = k == "galley" ? !J.T(Sea.Fleet(s, c).Galleys) : !J.T(Sea.Fleet(s, c).Ships);
            if (k == "galley") st.Galleys = (st.Galleys ?? 0) + 1; else st.Ships = (st.Ships ?? 0) + 1;
            s.Metric(k == "galley" ? "galleysBuilt" : "shipsBuilt");
            string nm = k == "galley" ? "kadırga" : Sea.HullName(s, c);
            string accNm = SHIP_ACC.TryGetValue(nm, out var a) ? a : "undefined";
            s.Log("sea", first ? $"{c.Name} ilk {accNm} {Tr.Ek(st.Name, "da")} denize indirdi." : $"{Tr.Ek(st.Name, "da")} yeni bir {nm} denize indirildi.", civ: c.Id, tile: st.Port ?? st.Tile, major: first);
            return;
        }
        if (p.Type == "civic")
        {
            string k = p.Kind;
            st.Civics.Set(k, (st.Civics.Get(k) ?? 0) + 1);
            if (k == "shipyard")
            {
                int pt = Sea.PickPort(s, st);
                st.Port = pt >= 0 ? pt : null;
                bool first = !J.Some(s.CivSettlements(c), x => x.Id != st.Id && J.T(x.Civics.Get("shipyard")));
                s.Log("sea", $"{Tr.Ek(st.Name, "da")} Tersane kuruldu{(first ? $": {c.Name} denize açılıyor" : "")}.", civ: c.Id, tile: st.Port ?? st.Tile, major: first);
                return;
            }
            if (k == "lighthouse") { s.Log("sea", $"{Tr.Ek(st.Name, "da")} Fener Kulesi yükseldi; gece gemilere yol gösterecek.", civ: c.Id, tile: st.Port ?? st.Tile, major: true); return; }
            if (Array.IndexOf(HOUSING, k) < 0 || (k == "house" && st.Civics.Get("house") == 1) || (k == "stonehouse" && st.Civics.Get("stonehouse") == 1))
                s.Log("build", $"{Tr.Ek(st.Name, "da")} {CivicName(c, k)} yükseldi.", civ: c.Id, tile: st.Tile, major: k == "tavern" || k == "unique" || k == "castle");
        }
        else if (p.Type == "workshop")
        {
            string k = p.Kind;
            st.Workshops.Set(k, (st.Workshops.Get(k) ?? 0) + 1);
            if (st.Workshops.Get(k) == 1) s.Log("build", $"{Tr.Ek(st.Name, "da")} {D.WORKSHOPS[k].Name} açıldı.", civ: c.Id, tile: st.Tile);
        }
        else if (p.Type == "extract")
        {
            var t = s.W.Tiles[p.Tile.Value];
            if (t.Owner == st.Id && t.Ext == null)
            {
                string kind = p.Kind;
                t.Ext = new ExtractBuilding { Kind = kind, Level = p.Level ?? 1, Settlement = st.Id, Workers = 0 };
                if (kind == "mine" || kind == "crystal" || kind == "mithril" || kind == "grove")
                {
                    var d = J.Find(s.W.Deposits, x => x.Id == t.Deposit);
                    bool firstOfKind = d != null && !AnyOtherExtOfKind(s, c, p.Tile, d.Kind);
                    if (d != null && firstOfKind) s.Log("build", $"{c.Name} {J.TrLower(D.DEPOSITS[d.Kind].Name)} üzerine {D.EXTRACTS[kind].Names[(p.Level ?? 1) - 1]} kurdu.", civ: c.Id, tile: p.Tile, major: D.DEPOSITS[d.Kind].Count <= 5);
                }
            }
        }
        else if (p.Type == "upgrade")
        {
            var t = s.W.Tiles[p.Tile.Value];
            if (t.Ext != null && t.Owner == st.Id)
            {
                t.Ext.Level = p.Level.Value;
                s.Metric("upgrade");
                string nm = D.EXTRACTS[t.Ext.Kind].Names[t.Ext.Level - 1];
                s.Log("build", $"{st.Name} yakınındaki {J.TrLower(D.EXTRACTS[t.Ext.Kind].Names[t.Ext.Level - 2])} {nm} seviyesine yükseltildi (L{t.Ext.Level}).", civ: c.Id, tile: p.Tile, major: t.Ext.Level == 3);
            }
        }
    }

    /// <summary>TS <c>s.w.tiles.some((x, i) =&gt; i !== p.tile &amp;&amp; x.ext &amp;&amp; x.deposit &gt;= 0 &amp;&amp; s.tileCiv(i) === c.id &amp;&amp; s.w.deposits.find((y) =&gt; y.id === x.deposit)?.kind === kind)</c>.</summary>
    private static bool AnyOtherExtOfKind(Sim s, Civ c, int? pTile, string kind)
    {
        var tiles = s.W.Tiles;
        for (int i = 0; i < tiles.Count; i++)
        {
            var x = tiles[i];
            if (i != pTile && x.Ext != null && x.Deposit >= 0 && s.TileCiv(i) == c.Id && J.Find(s.W.Deposits, y => y.Id == x.Deposit)?.Kind == kind) return true;
        }
        return false;
    }

    /// <summary>Sınıfa özgü yapı adları (civic 'unique'); bilinmeyen sınıf → 'Sınıf yapısı'.</summary>
    public static readonly JsObj<string> UNIQUE_BUILDING = new JsObj<string>
    {
        ["paladin"] = "Yemin Tapınağı", ["cleric"] = "Tapınak Ocağı", ["druid"] = "Kutsal Koru", ["rogue"] = "Hırsızlar Loncası", ["wizard"] = "Akademi", ["barbarian"] = "Totem Direği",
        ["bard"] = "Ozanlar Salonu", ["fighter"] = "Lejyon Kışlası", ["monk"] = "Manastır", ["ranger"] = "Korucu Locası", ["sorcerer"] = "Kan Soyu Mabedi", ["warlock"] = "Pakt Mihrabı",
    };

    /// <summary>Display name of a civic ('unique' resolves per class).</summary>
    public static string CivicName(Civ c, string k) => k == "unique" ? UNIQUE_BUILDING[c.Cls] ?? "Sınıf yapısı" : D.CIVICS[k].Name;

    /// <summary>Picks a project for each idle settlement; an unaffordable best candidate adds its cost to c.Want.</summary>
    public static void ChooseBuilds(Sim s, Civ c)
    {
        c.Want = new JsObj<double>();
        bool war = s.InWar(c);
        var cap = s.Capital(c);
        foreach (var st in s.CivSettlements(c))
        {
            if (st.Project != null) continue;
            var cands = BuildCandidates(s, c, st, st.Id == cap?.Id, war);
            if (cands.Count == 0) continue;
            J.Sort(cands, (a, b) => b.Score - a.Score);
            var best = cands[0];
            Cand chosen = null;
            foreach (var cd in cands)
            {
                if (cd.Score < best.Score * 0.2) break;
                if (s.CanPay(c, cd.Cost)) { chosen = cd; break; }
            }
            // karşılanamayan en iyi hedef talebe yansır
            if (chosen == null || chosen != best) foreach (var g in best.Cost.Keys()) c.Want.Set(g, (c.Want.Get(g) ?? 0) + (best.Cost.Get(g) ?? 0));
            if (chosen == null) continue;
            s.Pay(c, chosen.Cost);
            // { ...chosen.project, left: chosen.work, total: chosen.work }
            st.Project = new Project { Type = chosen.Project.Type, Kind = chosen.Project.Kind, Tile = chosen.Project.Tile, Level = chosen.Project.Level, Left = chosen.Work, Total = chosen.Work };
        }
    }

    private static readonly (string K, double Sc)[] HOUSING_CANDS = { ("stonehouse", 62), ("house", 56), ("hut", 50) };

    private static List<Cand> BuildCandidates(Sim s, Civ c, Settlement st, bool isCap, bool war)
    {
        var W = s.W;
        var outp = new List<Cand>();
        double P = s.Pop(st);
        double cap = s.Housing(st);
        bool civicAvail(string k) => Sim.CivicOk(st, k);   // Faz 1b-3: yerleşimin kademesi
        // Barınma
        if (P >= cap - 2)
        {
            foreach (var (k, sc) in HOUSING_CANDS)
            {
                if (!civicAvail(k)) continue;
                if (k == "hut" && (st.Civics.Get("hut") ?? 0) >= 6) continue;
                outp.Add(new Cand { Score = sc + (P >= cap ? 15 : 0) + JsMath.Min(30, (P - cap) * 2.5 * (P > cap ? 1 : 0)), Cost = D.CIVICS[k].Cost, Work = D.CIVICS[k].Work, Project = new Project { Type = "civic", Kind = k } });
            }
        }
        double slotsFree = Sim.TIER_SLOTS[st.Tier] - s.SlotsUsed(st);
        // Çıkarma yapıları
        double extSlots = 0;
        var tiles = J.Filter(s.G.Within(st.Tile, s.RadiusOf(st)), t => W.Tiles[t].Owner == st.Id);
        foreach (int ti in tiles) { var t = W.Tiles[ti]; if (t.Ext != null && !J.T(t.Ext.Depleted)) extSlots += D.LEVEL_SLOTS[t.Ext.Level]; }
        var bestByKind = new JsMap<string, Cand>();
        if (extSlots < P * 1.0 + 3)
        {
            foreach (int ti in tiles)
            {
                var t = W.Tiles[ti];
                if (t.Ext != null || ti == st.Tile || t.Camp != null) continue;
                foreach (var kind in PossibleKinds(s, c, t, ti, st))
                {
                    int lvl = D.EXTRACTS[kind].StartLevel ?? 1;
                    var fake = t.Clone();
                    fake.Ext = new ExtractBuilding { Kind = kind, Level = lvl, Settlement = st.Id, Workers = 0 };
                    var (good, y) = ExtractYield(s, c, fake, lvl, kind, st.Tier);
                    double v = y * D.LEVEL_SLOTS[lvl] * s.Price(c, good);
                    if (kind == "hunt") v += y * D.LEVEL_SLOTS[lvl] * (t.Terrain == "tundra" ? D.HIDE_TUNDRA : D.HIDE) * s.Price(c, "leather");
                    if (kind == "claypit") v -= y * D.LEVEL_SLOTS[lvl] * D.BRICK_FUEL * s.Price(c, "wood");
                    double score = v * 9 + (D.CLASSES[c.Cls].Desires.Contains(good) ? 6 : 0);
                    var cost = kind == "lumber" || kind == "hunt" ? new JsObj<double>() : D.LEVEL_COST[lvl];
                    var cand = new Cand { Score = score, Cost = cost, Work = 6 + lvl * 4, Project = new Project { Type = "extract", Kind = kind, Tile = ti, Level = lvl } };
                    var prev = bestByKind.Get(kind);
                    if (prev == null || prev.Score < score) bestByKind.Set(kind, cand);
                }
            }
            outp.AddRange(bestByKind.Values());
        }
        // Yükseltmeler
        foreach (int ti in tiles)
        {
            var t = W.Tiles[ti];
            if (t.Ext == null || J.T(t.Ext.Depleted) || t.Ext.Level >= 3) continue;
            var def = D.EXTRACTS[t.Ext.Kind];
            int nl = t.Ext.Level + 1;
            if (!J.T(def.Names[nl - 1]) || !Sim.ExtractLevelOk(st, t.Ext.Kind, nl)) continue;   // Faz 1b-3: yerleşimin kademesi
            if (t.Ext.Kind == "dock" && nl == 3 && !J.Some(s.G.Neighbors(ti), n => J.T(W.Tiles[n].Sea))) continue; // balıkçı filosu açık denize açılır
            if (t.Ext.Workers < D.LEVEL_SLOTS[t.Ext.Level] - 1) continue;
            var cur = ExtractYield(s, c, t, tier: st.Tier);
            var nxt = ExtractYield(s, c, t, nl, tier: st.Tier);
            double gain = (nxt.Y * D.LEVEL_SLOTS[nl] - cur.Y * t.Ext.Workers) * s.Price(c, cur.Good);
            outp.Add(new Cand { Score = gain * 10 + 8, Cost = D.LEVEL_COST[nl], Work = 10 + nl * 8, Project = new Project { Type = "upgrade", Kind = t.Ext.Kind, Tile = ti, Level = nl } });
        }
        // Denizcilik: tersane, fener, gemiler (yerleşim yuvası kullanmaz); filo medeniyet çapında hedeflenir.
        // Faz 1b-3: kıyısı olan her yerleşim liman kurabilir (eskiden Tekne Yapımı düğümü)
        var yards = J.Filter(s.CivSettlements(c), x => J.T(x.Civics.Get("shipyard")) && x.Port != null);
        if (civicAvail("shipyard") && !J.T(st.Civics.Get("shipyard")) && Sea.PickPort(s, st) >= 0)
        {
            bool near = J.Some(yards, x => s.G.Dist(x.Tile, st.Tile) <= 10);
            double sc = yards.Count == 0 ? 30 + (D.CLASSES[c.Cls].Prefer.Get("deniz") ?? 1) * 6 : J.T(st.Overseas) || (st.Tier >= 2 && !near) ? 12 : 0;
            if (sc > 0) outp.Add(new Cand { Score = sc, Cost = D.CIVICS["shipyard"].Cost, Work = D.CIVICS["shipyard"].Work, Project = new Project { Type = "civic", Kind = "shipyard" } });
        }
        if (J.T(st.Civics.Get("shipyard")) && st.Port != null)
        {
            if (civicAvail("lighthouse") && !J.T(st.Civics.Get("lighthouse"))) outp.Add(new Cand { Score = 14, Cost = D.CIVICS["lighthouse"].Cost, Work = D.CIVICS["lighthouse"].Work, Project = new Project { Type = "civic", Kind = "lighthouse" } });
            double routes = J.Filter(W.Routes, r => r.Alive && J.T(r.Sea) && (s.Settlement(r.A)?.Civ == c.Id || s.Settlement(r.B)?.Civ == c.Id)).Count;
            int ct = s.CivTier(c);
            double want = JsMath.Min(7, 1 + (ct >= Gate.SHIPBUILDING ? 1 : 0) + (ct >= Gate.NAVIGATION ? 1 : 0) + (ct >= Gate.SEATRADE ? 1 : 0) + routes);
            var fl = Sea.Fleet(s, c); double n = JsMath.Max(1, yards.Count);
            double have = st.Ships ?? 0;
            if (fl.Ships < want && have < Math.Ceiling(want / n)) outp.Add(new Cand { Score = 22 + (fl.Ships == 0 ? 14 : 0) + routes * 3, Cost = D.SHIPS["hull"].Cost, Work = D.SHIPS["hull"].Work, Project = new Project { Type = "ship", Kind = "hull" } });
            if (ct >= Gate.NAVY)
            {
                double gw = 2 + (war ? 2 : 0) + (D.CLASSES[c.Cls].Aggression >= 0.5 ? 1 : 0);
                if (fl.Galleys < gw && (st.Galleys ?? 0) < Math.Ceiling(gw / n)) outp.Add(new Cand { Score = 20 + (war ? 16 : 0) + D.CLASSES[c.Cls].Aggression * 10, Cost = D.SHIPS["galley"].Cost, Work = D.SHIPS["galley"].Work, Project = new Project { Type = "ship", Kind = "galley" } });
            }
        }
        if (slotsFree > 0)
        {
            // Atölyeler
            foreach (var k in D.WORKSHOP_IDS)
            {
                var def = D.WORKSHOPS[k];
                if (!Sim.WorkshopOk(st, k)) continue;   // Faz 1b-3: yerleşimin kademesi
                double have = st.Workshops.Get(k) ?? 0;
                if (have >= (st.Tier >= 2 ? 2 : 1) || IsGlut(s, c, def.Output, s.CivPop(c))) continue;
                var alt = Sim.WorkshopAltOk(st, def) ? def.Alt : null;
                bool inputsOk = J.Every(def.Inputs.Keys(), g => s.St(c, g) >= (def.Inputs.Get(g) ?? 0) * 4 || Producing(s, c, g))
                    || (alt != null && J.Every(alt.Keys(), g => s.St(c, g) >= (alt.Get(g) ?? 0) * 4 || Producing(s, c, g)));
                if (!inputsOk) continue;
                var inp = alt != null && J.Every(alt.Keys(), g => Producing(s, c, g) || s.St(c, g) > 0) ? alt : def.Inputs;
                double margin = s.Price(c, def.Output) - s.Worth(c, inp);
                if (margin <= 0) continue;
                outp.Add(new Cand { Score = margin * def.Rate * def.Slots * 9 + 5, Cost = def.Cost, Work = 16, Project = new Project { Type = "workshop", Kind = k } });
            }
            // Yerleşim yapıları
            void AddCivic(string k, double score)
            {
                if (!civicAvail(k)) return;
                var d = D.CIVICS[k];
                if ((st.Civics.Get(k) ?? 0) >= (d.Max ?? 99)) return;
                outp.Add(new Cand { Score = score, Cost = d.Cost, Work = d.Work, Project = new Project { Type = "civic", Kind = k } });
            }
            if (isCap) AddCivic("tavern", 45);
            if (isCap || P > 25) AddCivic("market", 30);
            AddCivic("temple", 18 + (c.Cls == "cleric" || c.Cls == "paladin" ? 10 : 0));
            if (isCap) AddCivic("library", 30 + (c.Cls == "wizard" ? 20 : 0));
            if (s.Access(c, "gold")) AddCivic("mint", 28);
            if (isCap) AddCivic("guild", 26);
            double threat = c.Threat + (war ? 1 : 0);
            AddCivic("palisade", 8 + threat * 35 + st.Tier * 6);
            AddCivic("stonewall", 10 + threat * 30 + st.Tier * 6);   // Faz 1b-3: Kasaba+ (CivicDef.Tier)
            if (isCap) AddCivic("castle", 20 + threat * 25);         // Faz 1b-3: Şehir
            if (isCap) AddCivic("unique", 40);                        // sınıf yapısı: başkent Köy olunca (eskiden alt sınıf seçimi)
        }
        return outp;
    }

    private static bool Producing(Sim s, Civ c, string g)
    {
        foreach (var t in s.W.Tiles) if (t.Ext != null && !J.T(t.Ext.Depleted) && t.Ext.Workers > 0 && t.Owner >= 0 && s.Settlement(t.Owner)?.Civ == c.Id && ExtractGood(t, s) == g) return true;
        return false;
    }

    /// <summary>Extraction building kinds (ExtractKind) that can be built on tile ti (st: the settlement it would belong to;
    /// Faz 1b-3: its tier gates the start level).</summary>
    public static List<string> PossibleKinds(Sim s, Civ c, Tile t, int ti, Settlement st)
    {
        var outp = new List<string>();
        bool lv1(string k) => Sim.ExtractLevelOk(st, k, D.EXTRACTS[k].StartLevel ?? 1);
        if (t.Deposit >= 0)
        {
            var d = J.Find(s.W.Deposits, x => x.Id == t.Deposit);
            if (!s.DepositVisible(c, d) || d.Depleted) return outp;
            string kind = D.DEPOSITS[d.Kind].Building;
            if (kind == "grove") { if (c.Cls == "druid" && lv1("grove")) outp.Add("grove"); return outp; }
            if (t.Reserve <= 0 && D.DEPOSITS[d.Kind].Reserve > 0) return outp;
            if (lv1(kind)) outp.Add(kind);
            return outp;
        }
        if ((t.Terrain == "forest") && t.Wood > 0) { outp.Add("lumber"); outp.Add("hunt"); }
        if (t.Terrain == "tundra") outp.Add("hunt");
        if (t.Terrain == "hill" && t.Reserve > 0 && lv1("quarry")) outp.Add("quarry");
        if ((t.Terrain == "grass" || t.Terrain == "swamp") && lv1("dock") && J.Some(s.G.Neighbors(ti), n => s.W.Tiles[n].Terrain == "water")) outp.Add("dock");
        return outp;
    }

    // ------------------------------------------------------------ askerler
    /// <summary>Soldier recruitment/disbanding toward the target ratio, then equipTick and first-troops militia.</summary>
    public static void RecruitTick(Sim s, Civ c)
    {
        bool war = s.InWar(c);
        var cls = D.CLASSES[c.Cls];
        double ratio = 0;
        bool training = s.CivAt(c, Gate.TRAINING);   // Faz 1b-3: Talim yerine Köy kademesi
        bool tribal = c.Cls == "barbarian" && training;
        bool unarmed = s.E(c, "unarmed") > 0;          // Keşiş: Yumruk Keşişleri ayrıcalığı (Kasaba)
        if (training || tribal || unarmed) ratio = (0.07 + cls.Aggression * 0.07 + s.E(c, "warband")) * (war ? 1.8 : 1) * (c.Threat > 0.5 ? 1.3 : 1);
        double deficit = 0;
        bool broke = c.Broke != null;   // C3: hazine boşken yeni asker yazılmaz
        foreach (var st in s.CivSettlements(c))
        {
            double target = Math.Floor(s.Pop(st) * ratio);
            if (st.Soldiers < target && !broke)
            {
                double n = JsMath.Min(2, target - st.Soldiers);
                for (int i = 0; i < n; i++)
                {
                    if (s.St(c, "arms") >= 1) { s.Add(c, "arms", -1); st.Soldiers++; }
                    else if (tribal && s.St(c, "leather") >= 2) { s.Add(c, "leather", -2); st.Soldiers++; }
                    else if (unarmed && s.St(c, "grain") >= 4) { s.Add(c, "grain", -4); st.Soldiers++; }
                    // silah yoksa: mızrak ve deri zırhla hafif piyade
                    else if (training && s.St(c, "leather") >= 2 && s.St(c, "wood") >= 3) { s.Add(c, "leather", -2); s.Add(c, "wood", -3); st.Soldiers++; }
                    else deficit++;
                }
            }
            else if (st.Soldiers > target + 2) st.Soldiers--;
        }
        if (J.T(deficit)) c.Want.Set("arms", (c.Want.Get("arms") ?? 0) + deficit * 2);
        Gear.EquipTick(s, c);
        // Talim ilk kez açıldığında birkaç milis silahsız asker olur
        if (ratio > 0 && !J.T(c.Yearly.Get("firstTroops")))
        {
            c.Yearly.Set("firstTroops", 1);
            var cap = s.Capital(c);
            if (cap != null) cap.Soldiers = JsMath.Max(cap.Soldiers, JsMath.Min(3, Math.Floor(s.Pop(cap) * 0.1)));
        }
    }
}
