using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FD.Macro;

namespace HeroStats;

/// <summary>
/// Kahraman ölçüm konsolu (Faz 1 A3a). Her seed için bir dünyayı N yıl koşar ve kahramanları ölçer:
/// doğuş seviyesi (on yıllık ortalama), yaşayanların / ölenlerin / emeklilerin seviye dağılımı, efsaneler,
/// ölüm ve emeklilik sayıları, dünya başına en yüksek seviye, ad çakışmaları; isteğe bağlı örnek destanlar
/// ve determinizm denetimi (aynı seed iki kez → aynı dünya). Faz 1 C1: efsanelerin ününün kaynağı (Tally rn_*),
/// önder/savunma/ilan/kamp dağılımı, taverna doluluğu, dünya başına savaş ilanı ve fetih.
/// </summary>
public static class Program
{
    private const int YEAR = 120;

    public static int Main(string[] args)
    {
        var inv = CultureInfo.InvariantCulture;
        string seedsArg = "1-8"; int years = 60; int jobs = 2; int epitaphs = 0; bool det = false; int names = 0;
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--seeds": seedsArg = args[++i]; break;
                case "--years": years = int.Parse(args[++i], inv); break;
                case "--jobs": jobs = int.Parse(args[++i], inv); break;
                case "--epitaphs": epitaphs = int.Parse(args[++i], inv); break;
                case "--names": names = int.Parse(args[++i], inv); break;
                case "--det": det = true; break;
                case "--tr": break;
                default: Console.Error.WriteLine($"unknown arg {args[i]}"); return 2;
            }
        }
        var seeds = ParseSeeds(seedsArg);
        D.Init();
        if (args.Length > 0 && args[0] == "--tr") { TrCheck(); return 0; }
        var sw = Stopwatch.StartNew();
        if (det) return Determinism(seeds, years, jobs);
        var res = new WorldStats[seeds.Count];
        Parallel.For(0, seeds.Count, new ParallelOptions { MaxDegreeOfParallelism = jobs }, i => res[i] = RunWorld(seeds[i], years));
        Report(res, years);
        LegendMix(res);
        Top(res, 5);
        Console.WriteLine("\n== Dünya ayrıntısı");
        foreach (var r in res)
        {
            var m = r.Metrics; var w = r.W;
            string Mx(string k) => F(m.Get(k) ?? 0, "0");
            int freeAlive = w.Heroes.Count(h => h.Civ < 0 && h.State != "dead" && h.State != "gone" && h.State != "retired");
            int hiredAlive = w.Heroes.Count(h => h.Civ >= 0 && h.State != "dead" && h.State != "gone" && h.State != "retired");
            int idle2 = w.Heroes.Count(h => h.Civ < 0 && h.State == "tavern" && (h.Tally.Get("battles") ?? 0) == 0 && w.Day - h.Born > 2 * YEAR);
            var taverns = w.Settlements.Where(x => x.Alive && (x.Civics.Get("tavern") ?? 0) > 0).ToList();
            int[] fill = new int[3];
            foreach (var t in taverns) fill[Math.Min(2, w.Heroes.Count(h => h.Civ == -1 && !h.BaseInn && h.Base == t.Id && h.State != "dead" && h.State != "gone" && h.State != "retired"))]++;
            Console.WriteLine($"[{F(r.Seed, "0")}] taverna {taverns.Count} (0/1/2+ yuvalı: {fill[0]}/{fill[1]}/{fill[2]}); serbestler: taverna {w.Heroes.Count(h => h.Civ < 0 && h.State == "tavern")}, görevde {w.Heroes.Count(h => h.Civ < 0 && h.State == "quest")}, yolda {w.Heroes.Count(h => h.Civ < 0 && h.State == "traveling")}, han yuvalı {w.Heroes.Count(h => h.Civ < 0 && h.BaseInn && h.State != "dead" && h.State != "gone" && h.State != "retired")}; kiralıklar: yurtta {w.Heroes.Count(h => h.Civ >= 0 && h.State == "home")}, orduda {w.Heroes.Count(h => h.Civ >= 0 && h.State == "army")}");
            Console.WriteLine($"[{F(r.Seed, "0")}] medeniyet {w.Civs.Count(c => c.Alive)}/{w.Civs.Count} yerleşim {w.Settlements.Count(x => x.Alive)} kamp(canlı) {w.Camps.Count(c => c.Alive)} han {w.Inns.Count(i => i.Alive)} | savaş: kamp {Mx("hb_camp")} kuşatma {Mx("hb_siege")} savunma {Mx("hb_defend")} pusu {Mx("hb_ambush")} han {Mx("hb_inn")} | ölüm: kamp {Mx("heroDeath_camp")} kuşatma {Mx("heroDeath_siege")} savunma {Mx("heroDeath_defend")} pusu {Mx("heroDeath_ambush")} yaşlılık {Mx("heroDeath_age")} | sonda serbest {freeAlive} kiralık {hiredAlive}; hiç savaşmamış 2+ yıllık serbest {idle2} | kiralama {Mx("heroBought")}+{Mx("innHire")} ilan {Mx("questTaken")} av {Mx("goalHunt")} harabe {Mx("goal_ruin")} | savaş ilanı {Mx("war")} fetih {Mx("conquest")} başkent {Mx("capitalFall")} yağma {Mx("plunder")}");
        }
        if (epitaphs > 0) Epitaphs(res, epitaphs);
        if (names > 0) Names(res, names);
        Console.WriteLine($"\n({seeds.Count} dünya × {years} yıl, {sw.Elapsed.TotalSeconds:0.0} sn)");
        return 0;
    }

    private static List<double> ParseSeeds(string a)
    {
        var o = new List<double>();
        foreach (var part in a.Split(','))
        {
            int dash = part.IndexOf('-', 1);
            if (dash > 0) { int lo = int.Parse(part.Substring(0, dash)), hi = int.Parse(part.Substring(dash + 1)); for (int x = lo; x <= hi; x++) o.Add(x); }
            else o.Add(double.Parse(part, CultureInfo.InvariantCulture));
        }
        return o;
    }

    // ------------------------------------------------------------------ one world
    public sealed class WorldStats
    {
        public double Seed;
        public World W;
        public int Born, Dead, Retired, Gone, Alive, Legends, MaxLevel, Sv8Plus, Revived;
        public int[] DeathLv = new int[12], AliveLv = new int[12], RetiredLv = new int[12], GoneLv = new int[12], EndLv = new int[12];
        public double[] BirthSum = new double[8]; public int[] BirthN = new int[8];
        public int[] BirthLvHist = new int[12];
        public int NameClash, Births;
        public int MaxJournal; public double JournalSum;
        public List<string> DeathEvents = new();
        public JsObj<double> Metrics;
        public double FirstLegendYear = -1;
        public int MaxDeeds, Epitaphs; public double DeedSum, MaxRenown, MaxBattles, BattleSum, DeadBattleSum, DeadYears;
        public int[] LegendLv = new int[12];
    }

    private static string StateOf(Hero h) => h.State;

    private static WorldStats RunWorld(double seed, int years)
    {
        var st = new WorldStats { Seed = seed };
        var sim = new Sim(seed);
        var w = sim.W;
        int days = years * YEAR;
        int seen = w.Heroes.Count;
        for (int d = 1; d <= days; d++)
        {
            sim.Step();
            for (; seen < w.Heroes.Count; seen++)
            {
                var h = w.Heroes[seen];
                int dec = Math.Min(7, (int)(h.Born / (10 * YEAR)));
                int bl = h.BirthLevel > 0 ? h.BirthLevel : h.Level;
                st.BirthSum[dec] += bl; st.BirthN[dec]++;
                st.BirthLvHist[Math.Min(11, bl)]++;
                st.Births++;
                // ad çakışması: yaşayan (ölmemiş) ya da son 10 yılda ölmüş biriyle aynı tam ad
                foreach (var o in w.Heroes)
                {
                    if (ReferenceEquals(o, h) || o.Name != h.Name) continue;
                    if (o.State != "dead" || (o.DeathDay ?? -1e9) >= w.Day - 10 * YEAR) { st.NameClash++; break; }
                }
            }
            if (st.FirstLegendYear < 0 && J.Some(w.Heroes, x => J.T(x.Legend))) st.FirstLegendYear = d / (double)YEAR;
        }
        st.W = w;
        st.Born = w.Heroes.Count;
        foreach (var h in w.Heroes)
        {
            int lv = Math.Min(11, h.Level);
            st.MaxLevel = Math.Max(st.MaxLevel, h.Level);
            if (h.Level >= 8) st.Sv8Plus++;
            if (J.T(h.Legend)) { st.Legends++; st.LegendLv[lv]++; }
            if (J.T(h.Revived)) st.Revived++;
            st.EndLv[lv]++;
            switch (StateOf(h))
            {
                case "dead": st.Dead++; st.DeathLv[lv]++; break;
                case "retired": st.Retired++; st.RetiredLv[lv]++; break;
                case "gone": st.Gone++; st.GoneLv[lv]++; break;
                default: st.Alive++; st.AliveLv[lv]++; break;
            }
            st.MaxJournal = Math.Max(st.MaxJournal, h.Journal.Count);
            st.JournalSum += h.Journal.Count;
            st.MaxDeeds = Math.Max(st.MaxDeeds, h.Deeds?.Count ?? 0);
            st.DeedSum += h.Deeds?.Count ?? 0;
            st.MaxRenown = Math.Max(st.MaxRenown, h.Renown);
            double bt = h.Tally.Get("battles") ?? 0;
            st.MaxBattles = Math.Max(st.MaxBattles, bt);
            st.BattleSum += bt;
            if (h.State == "dead") { st.DeadBattleSum += bt; if (!string.IsNullOrEmpty(h.Epitaph)) st.Epitaphs++; }
            double life = ((h.DeathDay ?? w.Day) - h.Born) / YEAR;
            if (h.State == "dead") st.DeadYears += life;
        }
        st.Metrics = w.Metrics;
        return st;
    }

    // ------------------------------------------------------------------ report
    private static string Hist(int[] a, int lo = 1, int hi = 10)
    {
        var sb = new StringBuilder();
        for (int i = lo; i <= hi; i++) sb.Append(a[i].ToString().PadLeft(5));
        if (a[11] > 0) sb.Append($"  (>10: {a[11]})");
        return sb.ToString();
    }

    private static double Median(IEnumerable<double> xs)
    {
        var l = xs.OrderBy(x => x).ToList();
        if (l.Count == 0) return double.NaN;
        return l.Count % 2 == 1 ? l[l.Count / 2] : (l[l.Count / 2 - 1] + l[l.Count / 2]) / 2;
    }

    private static string F(double x, string f = "0.00") => x.ToString(f, CultureInfo.InvariantCulture);

    private static void Report(WorldStats[] res, int years)
    {
        int decs = (years + 9) / 10;
        Console.WriteLine("== Dünya başına");
        Console.Write("seed  doğan  ölen  emekli  gitti  yaşıyor  ölüm%  efsane  maxSv  Sv8+  dirilen  çakışma  günlükMax  ");
        for (int d = 0; d < decs; d++) Console.Write($"dSv{d * 10 + 1}-{d * 10 + 10}".PadLeft(10));
        Console.WriteLine();
        foreach (var r in res)
        {
            Console.Write($"{F(r.Seed, "0"),4}  {r.Born,5}  {r.Dead,4}  {r.Retired,6}  {r.Gone,5}  {r.Alive,7}  {F(100.0 * r.Dead / Math.Max(1, r.Born), "0"),4}%  {r.Legends,6}  {r.MaxLevel,5}  {r.Sv8Plus,4}  {r.Revived,7}  {r.NameClash,7}  {r.MaxJournal,9}  ");
            for (int d = 0; d < decs; d++) Console.Write((r.BirthN[d] > 0 ? F(r.BirthSum[d] / r.BirthN[d]) + $"/{r.BirthN[d]}" : "-").PadLeft(10));
            Console.WriteLine();
        }
        var all = new WorldStats();
        foreach (var r in res)
        {
            for (int i = 0; i < 12; i++) { all.DeathLv[i] += r.DeathLv[i]; all.AliveLv[i] += r.AliveLv[i]; all.RetiredLv[i] += r.RetiredLv[i]; all.GoneLv[i] += r.GoneLv[i]; all.EndLv[i] += r.EndLv[i]; all.BirthLvHist[i] += r.BirthLvHist[i]; all.LegendLv[i] += r.LegendLv[i]; }
            for (int d = 0; d < 8; d++) { all.BirthSum[d] += r.BirthSum[d]; all.BirthN[d] += r.BirthN[d]; }
            all.Born += r.Born; all.Dead += r.Dead; all.Retired += r.Retired; all.Gone += r.Gone; all.Alive += r.Alive; all.Legends += r.Legends; all.NameClash += r.NameClash;
        }
        Console.WriteLine("\n== Seviye dağılımı (tüm dünyalar)        Sv1  Sv2  Sv3  Sv4  Sv5  Sv6  Sv7  Sv8  Sv9 Sv10");
        Console.WriteLine($"doğuşta                              {Hist(all.BirthLvHist)}");
        Console.WriteLine($"ölümde                               {Hist(all.DeathLv)}");
        Console.WriteLine($"emeklilikte                          {Hist(all.RetiredLv)}");
        Console.WriteLine($"diyarı terk edenler                  {Hist(all.GoneLv)}");
        Console.WriteLine($"sonda yaşayanlar                     {Hist(all.AliveLv)}");
        Console.WriteLine($"efsaneler                            {Hist(all.LegendLv)}");
        Console.WriteLine($"herkes (son seviye)                  {Hist(all.EndLv)}");
        int endAll = all.EndLv.Sum();
        int lo = all.EndLv[1] + all.EndLv[2] + all.EndLv[3] + all.EndLv[4] + all.EndLv[5], mid = all.EndLv[6] + all.EndLv[7] + all.EndLv[8], hi = all.EndLv[9] + all.EndLv[10] + all.EndLv[11];
        Console.WriteLine($"son seviye payı: Sv1–5 %{F(100.0 * lo / Math.Max(1, endAll), "0.0")}, Sv6–8 %{F(100.0 * mid / Math.Max(1, endAll), "0.0")}, Sv9–10 %{F(100.0 * hi / Math.Max(1, endAll), "0.0")}");
        int ended = all.Dead + all.Retired;
        int endLo = 0, endMid = 0, endHi = 0;
        for (int i = 1; i <= 11; i++) { int n = all.DeathLv[i] + all.RetiredLv[i]; if (i <= 5) endLo += n; else if (i <= 8) endMid += n; else endHi += n; }
        Console.WriteLine($"ölen+emekli payı: Sv1–5 %{F(100.0 * endLo / Math.Max(1, ended), "0.0")}, Sv6–8 %{F(100.0 * endMid / Math.Max(1, ended), "0.0")}, Sv9–10 %{F(100.0 * endHi / Math.Max(1, ended), "0.0")}");
        Console.Write("doğuş seviyesi ortalaması (on yıllık): ");
        for (int d = 0; d < decs; d++) Console.Write($"{d * 10 + 1}–{d * 10 + 10}: {(all.BirthN[d] > 0 ? F(all.BirthSum[d] / all.BirthN[d]) : "-")}  ");
        Console.WriteLine();

        // bitiş ölçütleri (DESIGN-FAZ1 §4)
        Console.WriteLine("\n== Bitiş ölçütleri (kahramanlar)");
        double worstDec = 0;
        foreach (var r in res) for (int d = 0; d < decs; d++) if (r.BirthN[d] > 0) worstDec = Math.Max(worstDec, r.BirthSum[d] / r.BirthN[d]);
        double allWorstDec = 0;
        for (int d = 0; d < decs; d++) if (all.BirthN[d] > 0) allWorstDec = Math.Max(allWorstDec, all.BirthSum[d] / all.BirthN[d]);
        int sv8Worlds = res.Count(r => r.MaxLevel >= 8);
        double legMed = Median(res.Select(r => (double)r.Legends));
        double deadMin = res.Min(r => 100.0 * r.Dead / Math.Max(1, r.Born)), deadMax = res.Max(r => 100.0 * r.Dead / Math.Max(1, r.Born));
        double deadMed = Median(res.Select(r => 100.0 * r.Dead / Math.Max(1, r.Born)));
        Console.WriteLine($"{Ok(allWorstDec <= 2)} doğuş seviyesi on yıllık ortalama ≤ 2: en kötü on yıl (tüm dünyalar) {F(allWorstDec)}; tek dünyada en kötü {F(worstDec)}");
        Console.WriteLine($"{Ok(sv8Worlds * 2 >= res.Length)} Sv8+ kahramanı olan dünya ≥ %50: {sv8Worlds}/{res.Length}");
        Console.WriteLine($"{Ok(legMed >= 1 && legMed <= 6)} efsane medyanı 1–6: {F(legMed, "0.#")} (dünyalar: {string.Join(",", res.Select(r => r.Legends))})");
        Console.WriteLine($"{Ok(deadMed >= 30 && deadMed <= 80)} doğanların %30–80'i ölür: medyan %{F(deadMed, "0")} (en az %{F(deadMin, "0")}, en çok %{F(deadMax, "0")}; toplam %{F(100.0 * all.Dead / Math.Max(1, all.Born), "0")})");
        Console.WriteLine($"{Ok(all.NameClash == 0)} ad çakışması yok: {all.NameClash}");
        Console.WriteLine($"   en yüksek seviye (dünya başına): {string.Join(",", res.Select(r => r.MaxLevel))}; medyan {F(Median(res.Select(r => (double)r.MaxLevel)), "0.#")}");
        Console.WriteLine($"   günlük: en uzun {res.Max(r => r.MaxJournal)} satır, ortalama {F(res.Sum(r => r.JournalSum) / Math.Max(1, all.Born), "0.0")}");
        Console.WriteLine($"   ilk efsane yılı: {string.Join(",", res.Select(r => r.FirstLegendYear < 0 ? "-" : F(r.FirstLegendYear, "0.0")))}");
        Console.WriteLine($"   soyadını bir atadan alan: {string.Join(",", res.Select(r => r.W.Heroes.Count(h => h.Lineage != null)))}; yaşlılıktan ölen: {string.Join(",", res.Select(r => F(r.Metrics.Get("heroDeath_age") ?? 0, "0")))}");
        Console.WriteLine($"   en yüksek ün: {string.Join(",", res.Select(r => F(r.MaxRenown, "0")))}");
        foreach (int th in new[] { 20, 25, 30, 35, 40, 50, 60 })
            Console.WriteLine($"   ün ≥ {th}: {string.Join(",", res.Select(r => r.W.Heroes.Count(h => h.Renown >= th)))}");
        foreach (int th in new[] { 14000, 23000, 34000, 48000 })
            Console.WriteLine($"   xp ≥ {th}: {string.Join(",", res.Select(r => r.W.Heroes.Count(h => h.Xp >= th)))}");
        Console.WriteLine($"   kilometre taşı: en çok {res.Max(r => r.MaxDeeds)}, ortalama {F(res.Sum(r => r.DeedSum) / Math.Max(1, all.Born), "0.0")}; destan yazılan ölü: {res.Sum(r => r.Epitaphs)}/{all.Dead}");
        Console.WriteLine($"   savaş/kahraman: ortalama {F(res.Sum(r => r.BattleSum) / Math.Max(1, all.Born), "0.0")}, ölenlerde {F(res.Sum(r => r.DeadBattleSum) / Math.Max(1, all.Dead), "0.0")}, en çok {F(res.Max(r => r.MaxBattles), "0")}; ölenlerin ortalama ömrü {F(res.Sum(r => r.DeadYears) / Math.Max(1, all.Dead), "0.0")} yıl");
        // XP kaynakları (metrics xp_*), varsa
        var xpKeys = new SortedSet<string>(StringComparer.Ordinal);
        foreach (var r in res) foreach (var k in r.Metrics.Keys()) if (k.StartsWith("xp_", StringComparison.Ordinal) || k.StartsWith("xpn_", StringComparison.Ordinal) || k.StartsWith("hb_", StringComparison.Ordinal) || k.StartsWith("renown_", StringComparison.Ordinal)) xpKeys.Add(k);
        foreach (var r in res) foreach (var k in r.Metrics.Keys()) if (k.StartsWith("heroDeath_", StringComparison.Ordinal) || k.StartsWith("levelUp_", StringComparison.Ordinal)) xpKeys.Add(k);
        if (xpKeys.Count > 0)
        {
            Console.WriteLine("\n== XP / ün kaynakları (dünya medyanı)");
            foreach (var k in xpKeys) Console.WriteLine($"   {k,-22} {F(Median(res.Select(r => r.Metrics.Get(k) ?? 0)), "0")}");
        }
        var mk = new[] { "heroSpawn", "innHeroSpawn", "heroDeath", "retire", "legend", "battles", "campCleared", "questDone", "questTaken", "goalHunt", "goal_ruin", "goal_plague", "goal_temple", "goal_library", "goal_rob", "duel", "assassination", "innRuined", "innHire", "heroBought", "contractLeave" };
        Console.WriteLine($"\n   ölü yerleşim (sonda): {string.Join(",", res.Select(r => r.W.Settlements.Count(x => !x.Alive)))}; yıkık han: {string.Join(",", res.Select(r => r.W.Inns.Count(x => !x.Alive)))}; temizlenmiş kamp: {string.Join(",", res.Select(r => r.W.Camps.Count(x => !x.Alive)))}");
        Console.WriteLine("\n== Sayaçlar (dünya medyanı)");
        foreach (var k in mk) Console.WriteLine($"   {k,-22} {F(Median(res.Select(r => r.Metrics.Get(k) ?? 0)), "0")}");
    }

    private static string Ok(bool b) => b ? "✓" : "✗";

    private static readonly string[] RN = { "camp", "victory", "boss", "quest", "defend", "nat20", "dragon", "duel" };

    /// <summary>Efsanelerin ünü nereden geldi (Tally rn_*), kişisel işlerin dağılımı (önder, savunma, ilan, kamp).</summary>
    private static void LegendMix(WorldStats[] res)
    {
        Console.WriteLine("\n== Ünün kaynağı (Tally rn_*, tüm dünyalar): efsaneler / herkes");
        var legends = res.SelectMany(r => r.W.Heroes.Where(h => J.T(h.Legend))).ToList();
        var everyone = res.SelectMany(r => r.W.Heroes).ToList();
        double lt = legends.Sum(h => h.Renown), at = everyone.Sum(h => h.Renown);
        foreach (var k in RN)
        {
            double l = legends.Sum(h => h.Tally.Get("rn_" + k) ?? 0), a = everyone.Sum(h => h.Tally.Get("rn_" + k) ?? 0);
            Console.WriteLine($"   {k,-8} efsane %{F(100 * l / Math.Max(1, lt), "0.0"),5}   herkes %{F(100 * a / Math.Max(1, at), "0.0"),5}");
        }
        int Of(Hero h, string k) => h.Deeds == null ? 0 : h.Deeds.Count(d => d.Kind == k);
        bool Notable(Hero h) => Of(h, "boss") + Of(h, "duel") + Of(h, "dragon") > 0 || (h.Tally.Get("defends") ?? 0) > 0;
        Console.WriteLine($"   efsane: {legends.Count}; önder/savunma/düello/ejderha işi olan %{F(100.0 * legends.Count(Notable) / Math.Max(1, legends.Count), "0")}; ortalama önder {F(legends.Average(h => (double)Of(h, "boss")), "0.0")}, savunma {F(legends.Average(h => h.Tally.Get("defends") ?? 0), "0.0")}, ilan {F(legends.Average(h => h.Tally.Get("quests") ?? 0), "0.0")}, kamp {F(legends.Average(h => h.Tally.Get("camps") ?? 0), "0.0")}, savaş {F(legends.Average(h => h.Tally.Get("battles") ?? 0), "0.0")}, Sv {F(legends.Average(h => (double)h.Level), "0.0")}");
        string Dist(Func<Hero, double> f) { var c = new int[6]; foreach (var h in everyone) c[(int)Math.Min(5, f(h))]++; return string.Join(" ", c.Select((x, i) => $"{(i == 5 ? "5+" : i.ToString())}:{x}")); }
        Console.WriteLine($"   önder devirme (kahraman sayısı): {Dist(h => Of(h, "boss"))}");
        Console.WriteLine($"   savunma: {Dist(h => h.Tally.Get("defends") ?? 0)}");
        Console.WriteLine($"   ilan: {Dist(h => h.Tally.Get("quests") ?? 0)}");
        Console.WriteLine($"   kamp: {Dist(h => Math.Floor((h.Tally.Get("camps") ?? 0) / 5))}  (5'li kümeler)");
    }

    private static void Top(WorldStats[] res, int n)
    {
        Console.WriteLine("\n== En deneyimli kahramanlar (dünya başına)");
        foreach (var r in res)
            foreach (var h in r.W.Heroes.OrderByDescending(h => h.Xp).ThenBy(h => h.Id).Take(n))
                Console.WriteLine($"[{F(r.Seed, "0")}] {h.Name,-26} {h.Race,-10} {h.Cls,-9} Sv{h.Level,-2} xp {F(h.Xp, "0"),6} savaş {F(h.Tally.Get("battles") ?? 0, "0"),3} zafer {F(h.Tally.Get("wins") ?? 0, "0"),3} öldürme {F(h.Kills, "0"),3} ün {F(h.Renown, "0"),3} {h.State,-9} {(h.Civ >= 0 ? "kiralık" : "serbest")} {F(((h.DeathDay ?? r.W.Day) - h.Born) / YEAR, "0.0")} yıl");
    }

    private static void Epitaphs(WorldStats[] res, int n)
    {
        Console.WriteLine("\n== Örnek destanlar");
        foreach (var r in res.Take(3))
        {
            var dead = r.W.Heroes.Where(h => h.State == "dead").ToList();
            var pick = dead.OrderByDescending(h => h.Renown).ThenBy(h => h.Id).Take(n).ToList();
            // bir de sıradan, erken ölmüş biri
            var young = dead.Where(h => !pick.Contains(h)).OrderBy(h => h.Id).Skip(dead.Count / 3).Take(1);
            foreach (var h in pick.Concat(young))
                Console.WriteLine($"\n[{F(r.Seed, "0")}] {h.Name} — Sv{h.Level}, ün {F(h.Renown, "0")}{(h.Legend == true ? ", efsane" : "")}, {h.Deeds?.Count ?? 0} taş, katil: {h.Killer}\n{h.Epitaph}");
        }
        Console.WriteLine("\n== Soy (atasının soyadını taşıyanlar)");
        foreach (var r in res)
            foreach (var h in r.W.Heroes.Where(h => h.Lineage != null).Take(2))
            {
                var anc = r.W.Heroes.FirstOrDefault(a => a.Id == h.Lineage);
                Console.WriteLine($"[{F(r.Seed, "0")}] {h.Name} ← {anc?.Name} (Sv{anc?.Level}, ün {F(anc?.Renown ?? 0, "0")}{(anc?.Legend == true ? ", efsane" : "")}) — {string.Join("; ", (h.Deeds ?? new List<HeroDeed>()).Take(2).Select(d => d.Text))}");
                if (!string.IsNullOrEmpty(h.Epitaph)) Console.WriteLine("   " + h.Epitaph);
            }
        Console.WriteLine("\n== Yaşayan efsaneler");
        foreach (var r in res.Take(3))
            foreach (var h in r.W.Heroes.Where(h => h.Legend == true && h.State != "dead").Take(3))
                Console.WriteLine($"[{F(r.Seed, "0")}] {Will.HeroLabel(h)} Sv{h.Level} ün {F(h.Renown, "0")} durum {h.State} — {string.Join("; ", (h.Deeds ?? new List<HeroDeed>()).Where(d => d.Kind != "level").TakeLast(5).Select(d => d.Text))}");
    }

    private static void Names(WorldStats[] res, int n)
    {
        Console.WriteLine("\n== Örnek adlar");
        foreach (var r in res.Take(2))
            Console.WriteLine($"[{F(r.Seed, "0")}] {string.Join(" · ", r.W.Heroes.Take(n).Select(h => $"{h.Name} ({h.Race})"))}");
    }

    // ------------------------------------------------------------------ Türkçe ek denetimi (göz kontrolü)
    private static void TrCheck()
    {
        var names = new List<string> { "Kara Bayrak Koyu 3", "Paslıkılıç Kampı II", "Kırıkdiş Kampı", "Karaköy", "Aelar Ayyaprağı", "Thokk Kurtdişi", "Sora Tunçpul", "Akta Kor", "Pip Tepealtı", "Kızıl Fener Hanı", "Aslanburç Krallığı", "Demir Birliği 10", "Kampı IV" };
        foreach (var n in names)
            Console.WriteLine($"{n}: i={Lore.Ek(n, "i")} a={Lore.Ek(n, "a")} da={Lore.Ek(n, "da")} dan={Lore.Ek(n, "dan")} in={Lore.Ek(n, "in")}");
        foreach (var t in new[] { "Yol pususu", "Bugbear pususu", "Düello", "Kervan soygunu", "Korsan baskını (Sisli Ada açıkları)", "Kırıkdiş Kampı baskını", "Demirhisar kuşatması", "Karaköy'ün tarla baskını", "Kızıl Fener Hanı baskını", "Karaköy açıklarında deniz savaşı" })
            Console.WriteLine($"{t} → {Lore.TitleLoc(t)}");
        foreach (var k in new[] { "bir goblin", "bir şeytancık", "bir kurt sürüsü", "bir pulzırh lejyoneri", "kampın goblin şefi", "bir alev soylu", "Varis Gölgeyaprak", "düşmanları", "Kaptan Tek Göz Rıza", "bir milis", "bir treant" })
            Console.WriteLine($"{k} → {Lore.Gen(k)} elinde");
        foreach (var d in new[] { "şan peşinde", "kayıp kardeşini arıyor", "eski bir borcu ödemek istiyor", "altına düşkün", "goblinlerden intikam almak istiyor", "efsanelerdeki ejderhayı arıyor", "kendini kanıtlamak istiyor" })
            Console.WriteLine($"{d} → {Lore.PastCopula(d)}");
        for (int d = 0; d < 480; d += 37) Console.Write(Lore.DateTr(d) + " | ");
        Console.WriteLine();
    }

    // ------------------------------------------------------------------ determinism
    private static string WorldHash(World w)
    {
        var json = Json.Serialize(w);
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(json))).Substring(0, 16);
    }

    private static int Determinism(List<double> seeds, int years, int jobs)
    {
        bool ok = true;
        foreach (var seed in seeds)
        {
            var hashes = new List<string>[2];
            Parallel.For(0, 2, new ParallelOptions { MaxDegreeOfParallelism = jobs }, k =>
            {
                var sim = new Sim(seed);
                var hs = new List<string>();
                for (int d = 1; d <= years * YEAR; d++)
                {
                    sim.Step();
                    if (d % (10 * YEAR) == 0) hs.Add(WorldHash(sim.W));
                }
                hashes[k] = hs;
            });
            bool same = hashes[0].SequenceEqual(hashes[1]);
            ok &= same;
            Console.WriteLine($"seed {seed}: {(same ? "aynı" : "FARKLI")} ({string.Join(" ", hashes[0])})");
        }
        return ok ? 0 : 1;
    }
}
