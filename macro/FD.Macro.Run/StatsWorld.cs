using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Faz 1b-7: dünyanın durumu (yol haritası v3, "Dünya yapısı"): yerleşim durum tablosu (6k), fırsat merkezi döngüsü (6l), tepki inşaatı
// ve büyük projeler (6m), büyük şehrin istikrarı ve iç krizler (6n, bilgi). Orta halkanın basamak kayması (6e), büyük şehrin el
// değiştirmesi (6f) ve uyarı (6g) EvaluateV3'te; çöküş nedenleri (9c) StatsPolity'de.

namespace FD.Macro.Run;

internal static partial class StatsMode
{
    private sealed partial class Report
    {
        private static readonly Dictionary<string, string> PATH_TR = new()
        {
            ["succession"] = "veraset", ["nobles"] = "soylu isyanı", ["duel"] = "düello", ["clansplit"] = "boyların ayrılması", ["coup"] = "darbe",
            ["mutiny"] = "paralı askerler (iflas)", ["schism"] = "mezhep bölünmesi", ["aforoz"] = "aforoz", ["secession"] = "ayrılık",
        };

        private static string PathTr(string k) => k != null && PATH_TR.TryGetValue(k, out var v) ? v : k ?? "?";

        private static readonly Dictionary<string, string> STATUS_TR = new()
        {
            ["prosper"] = "Refah", ["boom"] = "Ticaret patlaması", ["festival"] = "Festival", ["found"] = "Kaynak bulundu", ["migration"] = "Göç dalgası",
            ["shortage"] = "Kıtlık (hasat)", ["monsters"] = "Canavar tehdidi", ["depleted"] = "Kaynak tükendi", ["plague"] = "Salgın", ["siege"] = "Kuşatma",
            ["hunger"] = "Kıtlık (açlık)", ["occupation"] = "İşgal", ["newlord"] = "Yeni lord",
            ["feud"] = "Veraset kavgası", ["revolt"] = "Soylu isyanı", ["separatism"] = "Ayrılık", ["coup"] = "Darbe söylentisi", ["mutiny"] = "Paralı askerler huzursuz",
            ["schism"] = "Mezhep çatışması", ["challenge"] = "Meydan okuma",
        };

        private static string StatusTr(string k) => k != null && STATUS_TR.TryGetValue(k, out var v) ? v : k ?? "?";

        /// <summary>zarla gelen (sıradan) durumlar: süre ölçütü bunlara bakar (zorunlu durumlar koşul sürdükçe sürer)</summary>
        private static readonly string[] ROLLED = { "prosper", "boom", "festival", "found", "migration", "shortage", "monsters", "depleted" };

        private static readonly (string Phase, int Lo, int Hi)[] HUB_PHASES = { ("söylenti", 1, 2), ("hücum", 3, 6), ("zirve", 4, 10), ("tükeniş", 2, 6), ("hayalet", 3, 6) };

        private static double HubPhase(Hub h, int k)
        {
            double[] st = { h.Rumor, h.Rush, h.Peak, h.Bust, h.Ghost, h.End };
            if (st[k] < 0) return double.NaN;
            for (int j = k + 1; j < st.Length; j++) if (st[j] >= 0) return st[j] - st[k];
            return double.NaN;
        }

        private double PermSetlDays(WorldRun r)
        {
            var L = r.Stats.V3;
            double n = 0;
            for (int k = 0; k < 4; k++) foreach (var x in L.TierDaily[k]) n += x;
            return n;
        }

        private void EvaluateWorld()
        {
            int n = Ok.Count;
            // 6k. Durum tablosu
            {
                double rolls = Ok.Sum(r => MSum(r, "statusRoll")), sdays = Ok.Sum(PermSetlDays);
                double gap = rolls > 0 ? sdays / rolls : double.NaN;
                var sp = new Spans();
                foreach (var r in Ok) foreach (var x in r.Stats.V3.Statuses) if (x.End >= 0 && ROLLED.Contains(x.Kind)) sp.Xs.Add(x.End - x.Start);
                double share = Ok.Sum(r => r.Stats.V3.Statuses.Where(x => x.End >= 0).Sum(x => x.End - x.Start)) / Math.Max(1, sdays);
                Crits.Add(new Crit
                {
                    Id = "6k", Name = "Durum tablosu",
                    Rule = "her yerleşimde 5–15 günde bir zar (yerleşim-günü / zar ortalaması 5–15); zarla gelen durum 3–15 gün sürer (medyan, p10 ≥ 3, p90 ≤ 15)",
                    Pass = !double.IsNaN(gap) && gap >= 5 && gap <= 15 && sp.N > 0 && sp.Q(0.5) >= 3 && sp.Q(0.1) >= 3 && sp.Q(0.9) <= 15,
                    Measured = $"zar ortalama {F(gap)} günde bir ({F(rolls)} zar); zarla gelen durum {F(sp.Q(0.5))} gün ({F(sp.Q(0.1))}–{F(sp.Q(0.9))}; n = {sp.N}); yerleşim-günlerinin {Pct(share)}'inde bir durum var",
                    Data = new JObj { { "rollGap", gap }, { "rolls", rolls }, { "durations", sp.Json() }, { "statusShare", share } },
                });
            }
            // 6l. Fırsat merkezi döngüsü
            {
                var hs = Ok.SelectMany(r => r.Stats.V3.Hubs).ToList();
                var done = hs.Where(h => h.Phase == "done" && h.Outcome != "gone" && h.Outcome != "lost").ToList();
                var tot = new Spans(); foreach (var h in done) tot.Xs.Add(h.End - h.Rumor);
                var ph = HUB_PHASES.Select((p, k) => { var s = new Spans(); foreach (var h in done) { double v = HubPhase(h, k); if (!double.IsNaN(v)) s.Xs.Add(v); } return (p.Phase, p.Lo, p.Hi, S: s); }).ToList();
                bool phasesOk = ph.All(p => p.S.N == 0 || (p.S.Q(0.5) >= p.Lo && p.S.Q(0.5) <= p.Hi));
                var perWorld = Ok.Select(r => (double)r.Stats.V3.Hubs.Count(h => h.Phase == "done" && h.Outcome != "gone")).ToList();
                var rate = Dist(Ok.Select(r => Per100(r.Stats.V3.Hubs.Count, RunDays(r))));
                int started = hs.Count(h => h.Outcome != "gone");
                double completed = started > 0 ? done.Count / (double)hs.Count(h => h.Phase == "done" && h.Outcome != "gone") : double.NaN;
                Crits.Add(new Crit
                {
                    Id = "6l", Name = "Fırsat merkezi döngüsü",
                    Rule = "döngüler kurulur ve çöker: her dünyada ≥ 5 tamamlanan döngü; toplam süre (söylentiden sona) medyanı 10–30 gün; evrelerin medyanı hedef aralıkta (söylenti 1–2, hücum 3–6, zirve 4–10, tükeniş 2–6, hayalet 3–6)",
                    Pass = tot.N > 0 && tot.Q(0.5) >= 10 && tot.Q(0.5) <= 30 && phasesOk && perWorld.Min() >= 5,
                    Measured = $"toplam {F(tot.Q(0.5))} gün ({F(tot.Q(0.1))}–{F(tot.Q(0.9))}; n = {tot.N}); evreler: {string.Join(", ", ph.Select(p => $"{p.Phase} {F(p.S.Q(0.5))}"))}; dünyada {F(rate.Med)} ({F(rate.P10)}–{F(rate.P90)}) / 100 gün, dünya başına en az {F(perWorld.Min())} tamamlanan; türler: {string.Join(", ", hs.GroupBy(h => h.Kind).OrderByDescending(g => g.Count()).Select(g => $"{Hubs.KindName(g.Key)} {g.Count()}"))}; sonuç: {string.Join(", ", hs.Where(h => h.Phase == "done").GroupBy(h => h.Outcome).OrderByDescending(g => g.Count()).Select(g => $"{HubOutcomeTr(g.Key)} {g.Count()}"))}; zirve nüfusu medyanı {F(Median(done.Select(h => h.PeakPop)))}",
                    Data = new JObj { { "total", tot.Json() }, { "perWorldMin", perWorld.Min() }, { "per100", rate.Med }, { "phasesOk", phasesOk }, { "hubs", hs.Count }, { "completed", done.Count } },
                });
            }
            // 6m. Tepki inşaatı
            {
                double walls = Ok.Sum(r => MSum(r, "reactWall")), wa = Ok.Sum(r => MSum(r, "reactWallAlarm")), wd = Ok.Sum(r => MSum(r, "reactWallDays"));
                double alarms = Ok.Sum(r => MSum(r, "alarmOpen")), grand = Ok.Sum(r => MSum(r, "grandProject")), sab = Ok.Sum(r => MSum(r, "grandSabotage"));
                double delay = wa > 0 ? wd / wa : double.NaN;
                var gp = Dist(Ok.Select(r => MSum(r, "grandProject")));
                Crits.Add(new Crit
                {
                    Id = "6m", Name = "Tepki inşaatı",
                    Rule = "sur yalnız tehditten (baskın, kuşatma, yağma, akın) sonra ya da savaşta sınırda kurulur (kural); tehditten sonra sur ortalama ≤ 20 günde başlar; büyük proje (sınır kalesi, fener kulesi) olay olarak gelir, dünya başına 60 yılda 3–60",
                    Pass = walls > 0 && !double.IsNaN(delay) && delay <= 20 && gp.Med >= 3 && gp.Med <= 60,
                    Measured = $"sur projesi {F(walls)} (tehditten sonra {F(wa)}, ortalama {F(delay)} gün sonra; gerisi savaşta sınır boyunda); surusuz yerleşime gelen tehdit {F(alarms)}; büyük proje dünya başına {F(gp.Med)} ({F(gp.P10)}–{F(gp.P90)}; kale {F(Ok.Sum(r => MSum(r, "grandProject_castle")))}, fener {F(Ok.Sum(r => MSum(r, "grandProject_lighthouse")))}), sabotaj {F(sab)}",
                    Data = new JObj { { "walls", walls }, { "wallsAfterAlarm", wa }, { "delay", delay }, { "alarms", alarms }, { "grandPerWorld", gp.Med }, { "sabotage", sab } },
                });
            }
            // 6n. Büyük şehrin istikrarı ve iç krizler (bilgi)
            {
                var cr = Ok.SelectMany(r => r.Stats.V3.Crises).Where(x => x.End >= 0).ToList();
                var stab = Dist(Ok.SelectMany(r => r.Stats.V3.StabBig));
                var rate = Dist(Ok.Select(r => Per100(r.Stats.V3.Crises.Count, RunDays(r))));
                double fall = cr.Count > 0 ? cr.Count(x => x.Outcome == "fall") / (double)cr.Count : double.NaN;
                var lead = new Spans(); foreach (var x in cr) lead.Xs.Add(x.End - x.Start);
                Crits.Add(new Crit
                {
                    Id = "6n", Name = "Büyük şehrin istikrarı ve iç krizler", Info = true,
                    Rule = "istikrar (0–100) garnizon, kıtlık, vergi, meşruiyet, savaş yorgunluğu ve durumdan; düşük istikrarda iç kriz (5–10 gün belirti) ve tipin çöküş yolu; hedef yok",
                    Measured = $"büyük şehir ve taht şehri istikrarı {F(stab.Med)} ({F(stab.P10)}–{F(stab.P90)}); kriz dünyada {F(rate.Med)} / 100 gün (n = {cr.Count}), belirti {F(lead.Q(0.5))} gün ({F(lead.Q(0.1))}–{F(lead.Q(0.9))}), düşüşle biten {Pct(fall)}; büyük şehirde {cr.Count(x => x.Big)} kriz, düşüş {Pct(cr.Count(x => x.Big) > 0 ? cr.Count(x => x.Big && x.Outcome == "fall") / (double)cr.Count(x => x.Big) : double.NaN)}",
                    Data = new JObj { { "stability", stab.Med }, { "stabP10", stab.P10 }, { "stabP90", stab.P90 }, { "per100", rate.Med }, { "crises", cr.Count }, { "fallShare", fall } },
                });
            }
        }

        // ------------------------------------------------------------ Faz 1b-8: başsız ölçütler (yol haritası v3, Faz 1b/8)
        /// <summary>on yıllık pencere (gün)</summary>
        private const int DECADE = 10 * YEAR;

        private Crit Find(string id) => Crits.FirstOrDefault(c => c.Id == id);

        /// <summary>Dünyanın on yıllık pencereleri: kalıcı yerleşim (günlük ortalama), el değiştiren ve durumu olan yerleşim sayısı,
        /// büyük şehrin el değiştirmesi, savaş + iç kriz, fırsat merkezi, durum değişimi.</summary>
        private sealed class DecW { public double Setl, Owned, Statused, Big, WarCrisis, Wars, OppWars, Hubs, Changes; }

        private static List<DecW> Decades(WorldRun r)
        {
            var L = r.Stats.V3;
            var o = new List<DecW>();
            for (int d0 = 1; d0 + DECADE - 1 <= L.Days; d0 += DECADE)
            {
                int d1 = d0 + DECADE - 1;
                double setl = 0;
                for (int d = d0; d <= d1; d++) setl += L.Setl(d);
                var x = new DecW { Setl = setl / DECADE };
                var owned = new HashSet<int>();
                foreach (var e in L.Events)
                {
                    if (e.Day < d0 || e.Day > d1) continue;
                    if (e.Kind == "capture" || e.Kind == "secede" || e.Kind == "regime") owned.Add(e.Settlement);
                    if (e.Kind != "found") x.Changes++;
                }
                x.Owned = owned.Count;
                x.Statused = L.Statuses.Where(z => z.Start >= d0 && z.Start <= d1).Select(z => z.Settlement).Distinct().Count();
                x.Big = L.BigFalls.Count(f => f.Day >= d0 && f.Day <= d1);
                x.Wars = L.Wars.Count(w => w.Start >= d0 && w.Start <= d1);
                x.OppWars = L.Wars.Count(w => w.Start >= d0 && w.Start <= d1 && w.Kind == "opportunity");
                x.WarCrisis = x.Wars + L.Crises.Count(c => c.Start >= d0 && c.Start <= d1);
                x.Hubs = L.Hubs.Count(h => h.Rumor >= d0 && h.Rumor <= d1);
                o.Add(x);
            }
            return o;
        }

        private void EvaluateHeadless()
        {
            int n = Ok.Count;
            var decs = Ok.Select(r => (R: r, D: Decades(r))).ToList();
            var allDec = decs.SelectMany(x => x.D).ToList();
            // H1. Dünya donmuyor
            {
                int frozen = allDec.Count(x => x.Owned < 1 || x.WarCrisis < 1 || x.Hubs < 1);
                var ratio = Dist(decs.Where(x => x.D.Count >= 2).Select(x =>
                {
                    double early = x.D[0].Changes / Math.Max(1, x.D[0].Setl), late = x.D[x.D.Count - 1].Changes / Math.Max(1, x.D[x.D.Count - 1].Setl);
                    return early > 0 ? late / early : double.NaN;
                }));
                var c1 = Find("1");
                Crits.Add(new Crit
                {
                    Id = "H1", Name = "Dünya donmuyor",
                    Rule = "her dünyanın her on yılında en az bir yerleşim el değiştirir (fetih, bölünme, komşuya geçiş, içeriden düşüş), bir savaş ya da iç kriz başlar ve bir fırsat merkezi kurulur; yerleşim başına durum değişimi son on yılda ilk on yılın en az %80'i (dünya medyanı); ölçüt 1 (büyük olay) geçer",
                    Pass = allDec.Count > 0 && frozen == 0 && ratio.Med >= 0.8 && c1?.Pass == true,
                    Measured = $"donmuş on yıl {frozen}/{allDec.Count}; durum değişimi son / ilk on yıl {Pct(ratio.Med)} ({Pct(ratio.P10)}–{Pct(ratio.P90)}); ölçüt 1: {c1?.Measured}",
                    Data = new JObj { { "frozenDecades", frozen }, { "decades", allDec.Count }, { "changeRatio", ratio.Med } },
                });
            }
            // H2. Yerleşim sayısı aşağı yukarı sabit, sahiplik ve durum dalgalı
            {
                var c6b = Find("6b") ?? Find("6a");
                var own = Dist(allDec.Select(x => x.Owned / Math.Max(1, x.Setl)));
                double ownMin = allDec.Count > 0 ? allDec.Min(x => x.Owned / Math.Max(1, x.Setl)) : double.NaN;
                var st = Dist(allDec.Select(x => x.Statused / Math.Max(1, x.Setl)));
                Crits.Add(new Crit
                {
                    Id = "H2", Name = "Yerleşim sayısı sabit, sahiplik ve durum dalgalı",
                    Rule = "yerleşim sayısının değişim katsayısı ≤ %10 (6b); on yılda el değiştiren (fetih, bölünme, komşuya geçiş ya da içeriden düşüş) yerleşim payı medyanı ≥ %10, hiçbir dünya-on yılında %2'nin altında değil; on yılda en az bir durum yaşayan yerleşim / ortalama yerleşim sayısı medyanı ≥ 0,9 (on yılda kurulup terk edilenler yüzünden 1'i aşabilir)",
                    Pass = c6b?.Pass == true && own.Med >= 0.10 && ownMin >= 0.02 && st.Med >= 0.9,
                    Measured = $"yerleşim sayısı: {c6b?.Measured}; on yılda el değiştiren payı {Pct(own.Med)} ({Pct(own.P10)}–{Pct(own.P90)}; en az {Pct(ownMin)}); durum yaşayan yerleşim / ortalama {F(st.Med)} ({F(st.P10)}–{F(st.P90)})",
                    Data = new JObj { { "ownedShare", own.Med }, { "ownedMin", ownMin }, { "statusShare", st.Med } },
                });
            }
            // H3. Döngüler kuruluyor ve çöküyor
            {
                var parts = new List<string>();
                int ok = 0;
                var fails = new List<string>();
                foreach (var r in Ok)
                {
                    var L = r.Stats.V3;
                    int hubs = L.Hubs.Count(h => h.Phase == "done" && h.Outcome != "gone");
                    int up = L.Events.Count(e => e.Kind == "tierUp"), down = L.Events.Count(e => e.Kind == "tierDown");
                    int ruin = L.Events.Count(e => e.Kind == "abandon"), back = L.Events.Count(e => e.Kind == "resettle" || e.Kind == "found");
                    int falls = L.Events.Count(e => e.Kind == "regime" || e.Kind == "capture" || e.Kind == "secede");
                    int born = r.Stats.Civs.Count(c => c.Founded > 0), died = r.Stats.Civs.Count(c => c.ExtinctDay != null && c.ExtinctDay > 0);
                    bool good = hubs >= 5 && up >= 20 && down >= 20 && ruin >= 1 && back >= 1 && falls >= 1;
                    if (good) ok++; else fails.Add($"seed {S(r.Seed)} (merkez {hubs}, kademe {up}/{down}, harabe {ruin}, kuruluş {back}, el değiştirme {falls})");
                }
                double hubsMed = Median(Ok.Select(r => (double)r.Stats.V3.Hubs.Count(h => h.Phase == "done" && h.Outcome != "gone")));
                double ruinMed = Median(Ok.Select(r => (double)r.Stats.V3.Events.Count(e => e.Kind == "abandon")));
                double backMed = Median(Ok.Select(r => (double)r.Stats.V3.Events.Count(e => e.Kind == "resettle" || e.Kind == "found")));
                double resMed = Median(Ok.Select(r => (double)r.Stats.V3.Events.Count(e => e.Kind == "resettle")));
                double bornMed = Median(Ok.Select(r => (double)r.Stats.Civs.Count(c => c.Founded > 0)));
                double diedMed = Median(Ok.Select(r => (double)r.Stats.Civs.Count(c => c.ExtinctDay != null && c.ExtinctDay > 0)));
                var a9 = Find("9a");
                Crits.Add(new Crit
                {
                    Id = "H3", Name = "Döngüler kuruluyor ve çöküyor",
                    Rule = "her dünyada: ≥ 5 fırsat merkezi döngüsü tamamlanır; kademe hem yükselir hem düşer (≥ 20 / ≥ 20); en az bir yerleşim harabe olur ve en az bir yerleşim kurulur ya da harabe yeniden iskân edilir; en az bir el değiştirme; örgütler dağılıp yeniden kurulur (9a)",
                    Pass = ok == n && a9?.Pass == true,
                    Measured = $"{ok}/{n} dünya{(fails.Count > 0 ? $" (kalan: {string.Join("; ", fails)})" : "")}; dünya medyanı: tamamlanan merkez {F(hubsMed)}, harabe {F(ruinMed)}, kuruluş {F(backMed)} (harabeye yeniden iskân {F(resMed)}), doğan devlet {F(bornMed)}, yok olan {F(diedMed)}; örgüt: {a9?.Measured}",
                    Data = new JObj { { "worldsOk", ok }, { "hubs", hubsMed }, { "ruins", ruinMed }, { "founded", backMed }, { "resettled", resMed }, { "statesBorn", bornMed }, { "statesDied", diedMed } },
                });
            }
            // H4. Büyük şehir 100 günde 2–4 kez el değiştirir
            {
                var c6f = Find("6f");
                // çekirdek halka: dünya-on yılı başına büyük şehir (günlük ortalama)
                var bd = new List<double>();
                foreach (var r in Ok)
                {
                    var L = r.Stats.V3;
                    for (int d0 = 1; d0 + DECADE - 1 <= L.Days; d0 += DECADE) { double b = 0; for (int d = d0; d < d0 + DECADE; d++) b += L.TierDaily[Sim.BIG_TIER][d - 1]; bd.Add(b / DECADE); }
                }
                var bm = Dist(bd);
                double bmin = bd.Count > 0 ? bd.Min() : double.NaN, bmax = bd.Count > 0 ? bd.Max() : double.NaN;
                Crits.Add(new Crit
                {
                    Id = "H4", Name = "Büyük şehir: el değiştirme ve çekirdek halka",
                    Rule = "büyük şehir dünyada 100 günde 2–4 kez el değiştirir (6f; içeriden düşüş dâhil, Faz 1b-7'de onaylandı); çekirdek halka 4–6 büyük şehir: dünya-on yılı ortalamalarının medyanı 4–6, hiçbiri 3'ün altında ya da 8'in üstünde değil",
                    Pass = c6f?.Pass == true && bm.Med >= 4 && bm.Med <= 6 && bmin >= 3 && bmax <= 8,
                    Measured = $"{c6f?.Measured}; büyük şehir (dünya-on yılı ortalaması) medyan {F(bm.Med)} ({F(bm.P10)}–{F(bm.P90)}; en az {F(bmin)}, en çok {F(bmax)})",
                    Data = new JObj { { "bigCities", bm.Med }, { "bigMin", bmin }, { "bigMax", bmax } },
                });
            }
            // H5. Spec §9
            {
                var ids = new[] { "9a", "9b", "9c", "9d", "9e", "9f" };
                var cs = ids.Select(Find).ToList();
                Crits.Add(new Crit
                {
                    Id = "H5", Name = "Örgüt, devriye ve esaret (spec §9)",
                    Rule = "9a–9f'nin hepsi geçer",
                    Pass = cs.All(c => c?.Pass == true),
                    Measured = string.Join(", ", ids.Zip(cs, (i, c) => $"{i} {(c == null ? "—" : Mark(c))}")),
                });
            }
        }

        /// <summary>Faz 1b-8: başsız ölçütlerin on yıllık tablosu (dünya medyanı, p10–p90).</summary>
        private void HeadlessMd(StringBuilder sb)
        {
            void L(string s = "") => sb.Append(s).Append('\n');
            L("## Başsız ölçütler: on yıllar (Faz 1b-8)");
            L();
            L("Yol haritası Faz 1b/8 (H1–H5, ölçüt tablosunda). Dünya-on yılı pencereleri (400 gün; tarih öncesinden sonra); hücre: dünya medyanı (p10–p90). El değiştiren: fetih, bölünme, komşuya geçiş ya da içeriden düşüş yaşayan yerleşim payı; durum değişimi: kuruluş dışındaki bütün olaylar (kademe, yakılma, kıtlık, kuşatma, salgın, el değiştirme, terk, yeniden iskân), yerleşim başına.");
            L();
            var decs = Ok.Select(r => Decades(r)).ToList();
            int nd = decs.Count > 0 ? decs.Min(d => d.Count) : 0;
            if (nd == 0) { L("(on yıl yok)"); L(); return; }
            L("| Ölçü | " + string.Join(" | ", Enumerable.Range(0, nd).Select(i => $"{i * 10 + 1}–{i * 10 + 10}. yıl")) + " |");
            L("|---|" + string.Concat(Enumerable.Range(0, nd).Select(_ => "---|")));
            void Row(string name, Func<DecW, double> f, Func<double, string> fmt)
            {
                L($"| {name} | " + string.Join(" | ", Enumerable.Range(0, nd).Select(i => { var d = Dist(decs.Select(x => f(x[i]))); return $"{fmt(d.Med)} ({fmt(d.P10)}–{fmt(d.P90)})"; })) + " |");
            }
            Row("Kalıcı yerleşim", x => x.Setl, F);
            Row("El değiştiren payı", x => x.Owned / Math.Max(1, x.Setl), Pct);
            Row("Durum yaşayan / yerleşim", x => x.Statused / Math.Max(1, x.Setl), F);
            Row("Durum değişimi / yerleşim", x => x.Changes / Math.Max(1, x.Setl), F);
            Row("Savaş + iç kriz", x => x.WarCrisis, F);
            Row("Savaş", x => x.Wars, F);
            Row("Fırsat savaşı", x => x.OppWars, F);
            Row("Fırsat merkezi", x => x.Hubs, F);
            Row("Büyük şehir el değiştirmesi", x => x.Big, F);
            L();
        }

        private static string HubOutcomeTr(string k) => k switch { "ghost" => "hayalet (terk)", "village" => "kalıcı köy", "gone" => "söylentide söndü", "lost" => "yıkıldı", _ => k ?? "sürüyor" };

        private void WorldMd(StringBuilder sb)
        {
            void L(string s = "") => sb.Append(s).Append('\n');
            L("## Dünyanın durumu (Faz 1b-7)");
            L();
            L($"Yol haritası v3: yerleşim durum tablosu (her yerleşimde {Status.ROLL_MIN}–{Status.ROLL_MAX} günde bir zar), büyük şehrin istikrarı ve tipe göre iç çöküş yolları, fırsat merkezi döngüsü, tepki inşaatı. Bütün dünyalar havuzlanmış, bütün koşu.");
            L();
            // durumlar
            double sdays = Ok.Sum(PermSetlDays);
            L("Durumlar (yerleşim başına 1000 günde kaç kez; süre: gün, medyan (p10–p90)):");
            L();
            L("| Durum | 1000 yerleşim-gününde | süre | büyük şehirde payı |");
            L("|---|---|---|---|");
            foreach (var g in Ok.SelectMany(r => r.Stats.V3.Statuses).GroupBy(x => x.Kind).OrderByDescending(g => g.Count()))
            {
                var sp = new Spans(); foreach (var x in g) if (x.End >= 0) sp.Xs.Add(x.End - x.Start);
                L($"| {StatusTr(g.Key)} (`{g.Key}`) | {F(g.Count() / Math.Max(1, sdays) * 1000)} | {F(sp.Q(0.5))} ({F(sp.Q(0.1))}–{F(sp.Q(0.9))}) | {Pct(g.Count(x => x.Big) / (double)g.Count())} |");
            }
            L();
            L($"Göç: göçmen kafilesi {F(Ok.Sum(r => MSum(r, "migrantGroups")))}, kaçan {F(Ok.Sum(r => MSum(r, "emigrants")))}, gelen {F(Ok.Sum(r => MSum(r, "immigrants")))} kişi; basamak kaymasına yetecek göç denemesi: yukarı {F(Ok.Sum(r => MSum(r, "shiftTry_up")))}, aşağı {F(Ok.Sum(r => MSum(r, "shiftTry_down")))}.");
            L();
            // krizler
            var cr = Ok.SelectMany(r => r.Stats.V3.Crises).ToList();
            L("İç krizler (hükümet tipine göre; düşüş: krizin sonunda yerleşim içeriden düştü):");
            L();
            L("| Tip | Kriz | n | büyük şehirde | düşüş payı | belirti (gün, medyan) |");
            L("|---|---|---|---|---|---|");
            foreach (var g in cr.GroupBy(x => (x.Gov ?? "?", x.Kind)).OrderBy(g => g.Key.Item1).ThenByDescending(g => g.Count()))
            {
                var done = g.Where(x => x.End >= 0).ToList();
                L($"| {(GOV_TR.TryGetValue(g.Key.Item1, out var gt) ? gt : g.Key.Item1)} | {StatusTr(g.Key.Kind)} | {g.Count()} | {g.Count(x => x.Big)} | {Pct(done.Count > 0 ? done.Count(x => x.Outcome == "fall") / (double)done.Count : double.NaN)} | {F(Median(done.Select(x => (double)(x.End - x.Start))))} |");
            }
            L();
            var stab = Ok.SelectMany(r => r.Stats.V3.StabBig).OrderBy(x => x).ToList();
            if (stab.Count > 0) L($"İstikrar (büyük şehir ve taht şehri, 5 günde bir örnek, n = {stab.Count}): p10 {F(Q(stab, 0.1))}, p25 {F(Q(stab, 0.25))}, medyan {F(Q(stab, 0.5))}, p75 {F(Q(stab, 0.75))}, p90 {F(Q(stab, 0.9))}; 40'ın altında {Pct(stab.Count(x => x < 40) / (double)stab.Count)}.");
            L($"İçeriden düşüş: {F(Ok.Sum(r => MSum(r, "regime")))} (büyük şehirde {F(Ok.Sum(r => MSum(r, "regimeBig")))}); yollar: {string.Join(", ", PATH_TR.Keys.Select(k => (k, v: Ok.Sum(r => MSum(r, "regime_" + k)))).Where(x => x.v > 0).Select(x => $"{PathTr(x.k)} {F(x.v)}"))}. Tip değişimi {F(Ok.Sum(r => MSum(r, "govChange")))} ({string.Join(", ", Ok.SelectMany(r => r.Stats.Years).SelectMany(y => y.Map("metricDeltas") ?? new SortedDictionary<string, double>()).Where(kv => kv.Key.StartsWith("govChange_", StringComparison.Ordinal)).GroupBy(kv => kv.Key.Substring(10)).Select(g => $"{g.Key.Replace("_", " → ")} {F(g.Sum(x => x.Value))}"))}); birleşme (evlilik ittifakı) {F(Ok.Sum(r => MSum(r, "union")))}; komşuya geçen şehir {F(Ok.Sum(r => MSum(r, "defect")))}; Kutsal Sefer yenilgisi {F(Ok.Sum(r => MSum(r, "crusadeLost")))}.");
            L();
            // fırsat merkezleri
            var hs = Ok.SelectMany(r => r.Stats.V3.Hubs).ToList();
            L("Fırsat merkezleri (türe göre; süre: söylentiden sona, gün):");
            L();
            L("| Tür | n | süre | zirve nüfusu | sonuç | altın (toplam) |");
            L("|---|---|---|---|---|---|");
            foreach (var g in hs.GroupBy(h => h.Kind).OrderByDescending(g => g.Count()))
            {
                var d = g.Where(h => h.Phase == "done" && h.Outcome != "gone").Select(h => h.End - h.Rumor).OrderBy(x => x).ToList();
                L($"| {Hubs.KindName(g.Key)} | {g.Count()} | {(d.Count > 0 ? $"{F(Q(d, 0.5))} ({F(Q(d, 0.1))}–{F(Q(d, 0.9))})" : "–")} | {F(Median(g.Select(h => h.PeakPop)))} | {string.Join(", ", g.Where(h => h.Phase == "done").GroupBy(h => h.Outcome).Select(o => $"{HubOutcomeTr(o.Key)} {o.Count()}"))} | {F(g.Sum(h => h.Gold))} |");
            }
            L();
            var hd = Dist(Ok.Select(r => r.Stats.V3.HubDaily.Count > 0 ? r.Stats.V3.HubDaily.Average() : double.NaN));
            L($"Aynı anda yaşayan merkez (dünya başına, günlük ortalama): {F(hd.Med)} ({F(hd.P10)}–{F(hd.P90)}). Zirvede gelen haydut kampı {F(Ok.Sum(r => MSum(r, "hubBandits")))}, hayalet kasabaya yerleşen goblin {F(Ok.Sum(r => MSum(r, "hubGoblins")))}, kalıcı köy olan vadi {F(Ok.Sum(r => MSum(r, "hubVillage")))}.");
            L();
        }
    }
}
