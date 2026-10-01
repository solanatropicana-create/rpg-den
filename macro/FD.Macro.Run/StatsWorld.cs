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
