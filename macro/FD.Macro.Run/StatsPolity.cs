using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Faz 1b-6: devlet, inanç ve örgüt modelinin başsız ölçütleri (claude/devlet-orgut-spec.md §9): örgütlerin yaşamı ve şubelerin
// dalgalanması (9a), gölge savaşı (9b), hükümet tipine göre çöküş nedenleri (9c), esaret ve Özgürlük Ağı (9d), devriye profili (9e),
// aç haydutlar ve kıtlık (9f).

namespace FD.Macro.Run;

internal static partial class StatsMode
{
    private sealed partial class Report
    {
        private static readonly string[] GOVS = { "kingdom", "clans", "republic", "theocracy" };
        private static readonly Dictionary<string, string> GOV_TR = new() { ["kingdom"] = "Krallık", ["clans"] = "Boylar", ["republic"] = "Cumhuriyet", ["theocracy"] = "Teokrasi" };
        private static readonly string[] CAUSES = { "conquest", "secession", "succession", "duel", "coup", "aforoz" };
        private static readonly Dictionary<string, string> CAUSE_TR = new()
        {
            ["conquest"] = "başkent fethi", ["secession"] = "bölünme", ["succession"] = "veraset krizi", ["duel"] = "reisin düelloda ölümü",
            ["coup"] = "darbe girişimi", ["aforoz"] = "Pakt bağı ve aforoz",
        };

        private static double MSum(WorldRun r, string key) => r.Stats.Years.Sum(y => y.Map("metricDeltas") is { } m && m.TryGetValue(key, out double v) ? v : 0);
        private static double MapSum(WorldRun r, string map, string key) => r.Stats.Years.Sum(y => y.Map(map) is { } m && m.TryGetValue(key, out double v) ? v : 0);
        private static double RunDays(WorldRun r) => r.Stats.Years.Sum(y => y.LastDay - y.FirstDay + 1);

        private static double Pearson(List<(double X, double Y)> xs)
        {
            int n = xs.Count;
            if (n < 3) return double.NaN;
            double mx = xs.Average(p => p.X), my = xs.Average(p => p.Y);
            double sxy = xs.Sum(p => (p.X - mx) * (p.Y - my)), sxx = xs.Sum(p => (p.X - mx) * (p.X - mx)), syy = xs.Sum(p => (p.Y - my) * (p.Y - my));
            return sxx > 0 && syy > 0 ? sxy / Math.Sqrt(sxx * syy) : double.NaN;
        }

        /// <summary>Hükümet tipine göre devlet-yılı (yıl sonu sayımı).</summary>
        private double GovYears(string g) => Ok.Sum(r => MapSum(r, "govs", g));

        private (double StopRate, double CaravanStop, double SeizeRate, double BribeRate, double TollRate, double Arrests, double Duels) PatrolProfile(string g)
        {
            double exp = Ok.Sum(r => MSum(r, "patrolExposure_" + g)), cdays = Ok.Sum(r => MSum(r, "patrolCaravanDays_" + g));
            double stops = Ok.Sum(r => MSum(r, "patrolStop_" + g)), cstops = Ok.Sum(r => MSum(r, "patrolStopCaravan_" + g));
            double seize = Ok.Sum(r => MSum(r, "patrolSeize_" + g)), bribe = Ok.Sum(r => MSum(r, "patrolBribe_" + g)), toll = Ok.Sum(r => MSum(r, "patrolToll_" + g));
            double arrest = Ok.Sum(r => MSum(r, "patrolArrest_" + g)), duel = Ok.Sum(r => MSum(r, "patrolDuel_" + g) + MSum(r, "patrolDuelHero_" + g));
            return (Per100(stops, exp), Per100(cstops, cdays), cstops > 0 ? seize / cstops : double.NaN, cstops > 0 ? bribe / cstops : double.NaN,
                cstops > 0 ? toll / cstops : double.NaN, arrest, duel);
        }

        private void EvaluatePolity()
        {
            int n = Ok.Count;
            // 9a. Örgütler yaşar ya da yok olup yeniden doğar; şube sayısı dalgalanır
            {
                var all = Ok.SelectMany(r => r.Stats.Orgs).ToList();
                int gone = all.Count(o => !o.Alive && !o.Pending);
                int dissolved = (int)Ok.Sum(r => r.Stats.Years.Sum(y => y["orgDissolved"])), reborn = (int)Ok.Sum(r => r.Stats.Years.Sum(y => y["orgReborn"]));
                var cvs = new List<double>();
                foreach (var r in Ok)
                    foreach (var kind in Polity.ORG_IDS)
                    {
                        var xs = r.Stats.Years.Where(y => !y.Partial).Select(y => y.Map("orgBranchesByKind") is { } m && m.TryGetValue(kind, out double v) ? v : 0).ToList();
                        if (xs.Count < 2) continue;
                        double mean = xs.Average();
                        if (mean <= 0) continue;
                        double sd = Math.Sqrt(xs.Sum(x => (x - mean) * (x - mean)) / xs.Count);
                        cvs.Add(sd / mean);
                    }
                var cv = Dist(cvs);
                double opened = Ok.Sum(r => r.Stats.Years.Sum(y => y["orgOpened"])), closed = Ok.Sum(r => r.Stats.Years.Sum(y => y["orgClosed"]));
                Crits.Add(new Crit
                {
                    Id = "9a", Name = "Örgütler yaşar, şubeleri dalgalanır",
                    Rule = "hiçbir örgüt kalıcı olarak yok olmaz (koşu sonunda ya yaşıyor ya da yeniden kurulmayı bekliyor); örgüt başına yıllık şube sayısının değişim katsayısı medyanı ≥ %10",
                    Pass = gone == 0 && cv.Med >= 0.1,
                    Measured = $"kalıcı yok olan {gone}/{all.Count}; koşu sonunda yaşayan {all.Count(o => o.Alive)}; dağılma {dissolved}, yeniden kuruluş {reborn}; şube değişim katsayısı {Pct(cv.Med)} ({Pct(cv.P10)}–{Pct(cv.P90)}); açılan {F(opened)}, kapanan {F(closed)} şube ({F(opened / n)} / {F(closed / n)} dünya başına)",
                    Data = new JObj { { "gone", gone }, { "orgs", all.Count }, { "dissolved", dissolved }, { "reborn", reborn }, { "branchCv", cv.Med }, { "opened", opened }, { "closed", closed } },
                });
            }
            // 9b. Gölge savaşı düzenli
            {
                var wy = Ok.SelectMany(r => r.Stats.Years.Where(y => !y.Partial).Select(y => y["shadowWar"])).ToList();
                double share = wy.Count > 0 ? wy.Count(v => v >= 1) / (double)wy.Count : double.NaN;
                var rate = Dist(Ok.Select(r => Per100(r.Stats.Years.Sum(y => y["shadowWar"]), RunDays(r))));
                double kills = Ok.Sum(r => r.Stats.Years.Sum(y => y["shadowKills"]));
                double fail = Ok.Sum(r => MSum(r, "shadowFail")), tot = Ok.Sum(r => MSum(r, "shadowWar"));
                Crits.Add(new Crit
                {
                    Id = "9b", Name = "Gölge savaşı düzenli",
                    Rule = "dünya-yıllarının ≥ %90'ında en az bir gölge savaşı eylemi (suikast, sabotaj, ihbar); hedef sıklık sonra ayarlanacak",
                    Pass = share >= 0.9,
                    Measured = $"en az bir eylemi olan dünya-yılı {Pct(share)}; dünyada {F(rate.Med)} ({F(rate.P10)}–{F(rate.P90)}) / 100 gün; başarısız {Pct(tot > 0 ? fail / tot : double.NaN)}; öldürülen usta/lider {F(kills)}; türler: suikast {F(Ok.Sum(r => MSum(r, "shadow_assassinate")))}, sabotaj {F(Ok.Sum(r => MSum(r, "shadow_sabotage")))}, ihbar {F(Ok.Sum(r => MSum(r, "shadow_denounce")))}",
                    Data = new JObj { { "yearShare", share }, { "per100", rate.Med }, { "kills", kills }, { "failShare", tot > 0 ? fail / tot : double.NaN } },
                });
            }
            // 9c. Hükümet tiplerinin en sık çöküş nedenleri farklı
            {
                var tops = new List<string>();
                var parts = new List<string>();
                var data = new JObj();
                foreach (var g in GOVS)
                {
                    var counts = CAUSES.Select(cs => (Cause: cs, N: Ok.Sum(r => MSum(r, $"collapse_{cs}_{g}")))).ToList();
                    var top = counts.OrderByDescending(x => x.N).ThenBy(x => Array.IndexOf(CAUSES, x.Cause)).First();
                    tops.Add(top.N > 0 ? top.Cause : "-");
                    double gy = GovYears(g);
                    parts.Add($"{GOV_TR[g]}: {(top.N > 0 ? CAUSE_TR[top.Cause] : "yok")} ({string.Join(", ", counts.Where(x => x.N > 0).Select(x => $"{CAUSE_TR[x.Cause]} {F(x.N)}"))}; {F(gy)} devlet-yılı)");
                    var gj = new JObj { { "stateYears", gy } };
                    foreach (var (cs, cnt) in counts) gj.Add(cs, cnt);
                    data.Add(g, gj);
                }
                bool distinct = tops.All(t => t != "-") && tops.Distinct().Count() == tops.Count;
                Crits.Add(new Crit
                {
                    Id = "9c", Name = "Tiplerin çöküş nedenleri farklı",
                    Rule = "dört hükümet tipinin en sık çöküş nedeni birbirinden farklı (başkent fethi, bölünme, veraset krizi, düello, darbe, aforoz); tipe özgü çöküş yolları Faz 1b/7",
                    Pass = distinct,
                    Measured = string.Join("; ", parts),
                    Data = data,
                });
            }
            // 9d. Köle payı küçük, Özgürlük Ağı etkin
            {
                var shares = Ok.SelectMany(r => r.Stats.Years.Where(y => !y.Partial).Select(y => y["slaveShare"])).Where(v => !double.IsNaN(v)).ToList();
                double mx = shares.Count > 0 ? shares.Max() : double.NaN;
                var med = Dist(shares);
                var freed = Ok.Select(r => r.Stats.Years.Sum(y => y["freedByNetwork"])).ToList();
                int active = freed.Count(v => v >= 1);
                double ens = Ok.Sum(r => r.Stats.Years.Sum(y => y["enslaved"]));
                Crits.Add(new Crit
                {
                    Id = "9d", Name = "Esaret sınırlı, Özgürlük Ağı etkin",
                    Rule = "köle payı her dünya-yılında nüfusun ≤ %5'i; Özgürlük Ağı dünyaların ≥ %75'inde köle kurtarır",
                    Pass = mx <= 0.05 && active >= 0.75 * n,
                    Measured = $"köle payı medyan {Pct(med.Med)} (p90 {Pct(med.P90)}, en çok {Pct(mx)}); esarete düşen {F(ens)} (av {F(Ok.Sum(r => MSum(r, "enslaved_hunt")))}, savaş {F(Ok.Sum(r => MSum(r, "enslaved_war")))}, borç {F(Ok.Sum(r => MSum(r, "enslaved_debt")))}, baskın {F(Ok.Sum(r => MSum(r, "enslaved_raid")))}); hapis madeni {F(Ok.Sum(r => MSum(r, "prisonMine")))}; Özgürlük Ağı {active}/{n} dünyada {F(freed.Sum())} köle kurtardı; kaçan {F(Ok.Sum(r => MSum(r, "escaped")))}, azat {F(Ok.Sum(r => MSum(r, "manumitted")))}; esir düşen kahraman {F(Ok.Sum(r => MSum(r, "heroCaptive")))}",
                    Data = new JObj { { "maxShare", mx }, { "medianShare", med.Med }, { "freedWorlds", active }, { "freed", freed.Sum() }, { "enslaved", ens } },
                });
            }
            // 9e. Devriye profili tipe göre ayrışır
            {
                var prof = GOVS.Select(g => (G: g, P: PatrolProfile(g))).ToList();
                double Ratio(Func<(double StopRate, double CaravanStop, double SeizeRate, double BribeRate, double TollRate, double Arrests, double Duels), double> f)
                {
                    var xs = prof.Select(p => f(p.P)).Where(v => !double.IsNaN(v)).ToList();
                    if (xs.Count < 2) return double.NaN;
                    return xs.Min() > 0 ? xs.Max() / xs.Min() : double.PositiveInfinity;
                }
                double rStop = Ratio(p => p.CaravanStop), rSeize = Ratio(p => p.SeizeRate), rBribe = Ratio(p => p.BribeRate);
                Crits.Add(new Crit
                {
                    Id = "9e", Name = "Devriye profili tipe göre ayrışır",
                    Rule = "kervan başına durdurma, el koyma ve rüşvet: tipler arasında en yüksek / en düşük durdurma oranı ≥ 1,5, el koyma ve rüşvet oranları ≥ 2",
                    Pass = rStop >= 1.5 && rSeize >= 2 && rBribe >= 2,
                    Measured = string.Join("; ", prof.Select(p => $"{GOV_TR[p.G]}: 100 kervan-günde {F(p.P.CaravanStop)} durdurma, durdurmada el koyma {Pct(p.P.SeizeRate)}, rüşvet {Pct(p.P.BribeRate)}, haraç/vergi {Pct(p.P.TollRate)}, tutuklama {F(p.P.Arrests)}, düello {F(p.P.Duels)}")) + $" (oranlar: durdurma ×{F(rStop)}, el koyma ×{F(rSeize)}, rüşvet ×{F(rBribe)})",
                    Data = prof.Aggregate(new JObj(), (j, p) => { j.Add(p.G, new JObj { { "caravanStopPer100", p.P.CaravanStop }, { "stopPer100", p.P.StopRate }, { "seize", p.P.SeizeRate }, { "bribe", p.P.BribeRate }, { "toll", p.P.TollRate }, { "arrests", p.P.Arrests }, { "duels", p.P.Duels } }); return j; }),
                });
            }
            // 9f. Aç haydutlar kıtlıkla ilişkili
            {
                var pairs = Ok.SelectMany(r => r.Stats.Years.Where(y => !y.Partial).Select(y => (X: y["hungryShare"], Y: y["banditsBorn"]))).Where(p => !double.IsNaN(p.X)).ToList();
                double rr = Pearson(pairs);
                double born = pairs.Sum(p => p.Y);
                double fed = Ok.Sum(r => MSum(r, "banditFed"));
                Crits.Add(new Crit
                {
                    Id = "9f", Name = "Aç haydutlar kıtlıkla ilişkili",
                    Rule = "dünya-yılı başına aç ya da ekmeksiz yerleşim payı ile haydut olan aç halk arasında korelasyon r ≥ 0,3 (en az 30 haydut)",
                    Pass = born < 30 ? null : rr >= 0.3,
                    Measured = $"r = {F(rr)} ({pairs.Count} dünya-yılı); haydut olan {F(born)}, aç haydut kampı {F(Ok.Sum(r => MSum(r, "banditCamp")))}, yiyecek verilip dağılan {F(fed)}, açlıktan eriyen {F(Ok.Sum(r => MSum(r, "banditStarved")))}; aç ya da ekmeksiz yerleşim payı medyanı {Pct(Dist(pairs.Select(p => p.X)).Med)}",
                    Data = new JObj { { "r", rr }, { "n", pairs.Count }, { "born", born }, { "fed", fed } },
                });
            }
        }

        private void PolityMd(StringBuilder sb)
        {
            void L(string s = "") => sb.Append(s).Append('\n');
            L("## Devlet, inanç ve örgüt (Faz 1b-6)");
            L();
            L($"claude/devlet-orgut-spec.md: 4–6 devlet (dört hükümet tipi her dünyada), yerleşimlerin ırk ve inanç dağılımı, 13 örgüt (merkezleri çekirdek şehirlerde). Dünya tarih öncesiyle ({Sim.PREHISTORY_DAYS} gün) olgun başlar; örgütler tarih öncesinin sonunda kurulur, ölçüm o günden başlar. Örgüt kararları ~{F(Orgs.ORG_DAYS)} günde bir.");
            L();
            // devletler
            var gy = GOVS.Select(g => (G: g, Y: GovYears(g))).ToList();
            double startStates = Median(Ok.Select(r => r.Stats.Years.Count > 0 ? r.Stats.Years[0]["civsAlive"] : double.NaN));
            L($"Devletler: dünya başına {F(startStates)} (ilk yıl, medyan); devlet-yılı tipe göre: {string.Join(", ", gy.Select(x => $"{GOV_TR[x.G]} {F(x.Y)}"))}. Yönetici değişimi {F(Ok.Sum(r => r.Stats.Years.Sum(y => y["successions"])))} (veraset {F(Ok.Sum(r => MSum(r, "succession_heir")))}, veraset krizi {F(Ok.Sum(r => MSum(r, "succession_crisis")))}, düello {F(Ok.Sum(r => MSum(r, "succession_duel")))}, boy meclisi {F(Ok.Sum(r => MSum(r, "succession_council")))}, konsey oyu {F(Ok.Sum(r => MSum(r, "succession_vote")))}, tarikat {F(Ok.Sum(r => MSum(r, "succession_order")))}); seçim {F(Ok.Sum(r => MSum(r, "election")))}; reise meydan okuma {F(Ok.Sum(r => MSum(r, "chiefDuel")))}. Pakt'ın sızdığı yönetici {F(Ok.Sum(r => MSum(r, "pactInfiltrate")))}, ortaya çıkan {F(Ok.Sum(r => MSum(r, "pactExposed")))}; Pakt suikastı {F(Ok.Sum(r => MSum(r, "pactAssassination")))}; Tarikat'ın Kutsal Sefer çağrısı {F(Ok.Sum(r => MSum(r, "orgCrusadeCall")))}, başlayan sefer {F(Ok.Sum(r => MSum(r, "crusade")))}. Lobiyle yasa değişikliği {F(Ok.Sum(r => MSum(r, "orgLobby")))} (kölelik yasası {F(Ok.Sum(r => MSum(r, "slaveryLaw")))}), darbe girişimi {F(Ok.Sum(r => MSum(r, "orgCoup")))}.");
            L();
            L("Örgütler (koşu sonu; dünyalar arası medyan, toplamlar bütün dünyalar):");
            L();
            L("| Örgüt | yaşıyor (dünya) | şube | üye | gizli şube payı | dağılma / yeniden kuruluş | açılan / kapanan şube | gölge savaşı | üye kahraman işi |");
            L("|---|---|---|---|---|---|---|---|---|");
            foreach (var kind in Polity.ORG_IDS)
            {
                var os = Ok.Select(r => r.Stats.Orgs.FirstOrDefault(o => o.Kind == kind)).Where(o => o != null).ToList();
                if (os.Count == 0) continue;
                double T(string k) => os.Sum(o => o.Tally.TryGetValue(k, out double v) ? v : 0);
                L($"| {Polity.ORGS[kind].Name} | {os.Count(o => o.Alive)}/{os.Count} | {F(Median(os.Select(o => (double)o.Branches)))} | {F(Median(os.Select(o => o.Members)))} | {Pct(os.Sum(o => o.Branches) > 0 ? os.Sum(o => o.Hidden) / (double)os.Sum(o => o.Branches) : double.NaN)} | {F(Ok.Sum(r => r.Stats.Orgs.Count(o => o.Kind == kind && !o.Alive)))} / {F(os.Sum(o => o.Rebirths))} | {F(T("opened"))} / {F(T("closed"))} | {F(T("shadow"))} | {F(T("heroTask"))} |");
            }
            L();
            L("Örgüt operasyonları (bütün dünyalar): " + string.Join(", ", Ok.SelectMany(r => r.Stats.Years).SelectMany(y => y.Map("metricDeltas") ?? new SortedDictionary<string, double>()).Where(kv => kv.Key.StartsWith("orgOp_", StringComparison.Ordinal)).GroupBy(kv => kv.Key.Substring(6)).OrderByDescending(g => g.Sum(x => x.Value)).Select(g => $"{g.Key} {F(g.Sum(x => x.Value))}")) + ".");
            L();
            L("Devriye (hükümet tipine göre; bütün dünyalar):");
            L();
            L("| Tip | maruz ajan-günü | 100 kervan-günde durdurma | durdurmada el koyma | rüşvet | haraç/vergi | düello | kahraman tutuklama | el konan değer | vergi/haraç altını |");
            L("|---|---|---|---|---|---|---|---|---|---|");
            foreach (var g in GOVS)
            {
                var p = PatrolProfile(g);
                L($"| {GOV_TR[g]} | {F(Ok.Sum(r => MSum(r, "patrolExposure_" + g)))} | {F(p.CaravanStop)} | {Pct(p.SeizeRate)} | {Pct(p.BribeRate)} | {Pct(p.TollRate)} | {F(p.Duels)} | {F(p.Arrests)} | {F(Ok.Sum(r => MSum(r, "patrolSeizeValue_" + g)))} | {F(Ok.Sum(r => MSum(r, "patrolTollGold_" + g)))} |");
            }
            L();
            L("Çöküş nedenleri (hükümet tipine göre; bütün dünyalar; 100 devlet-yılı başına):");
            L();
            L("| Tip | devlet-yılı | " + string.Join(" | ", CAUSES.Select(c => CAUSE_TR[c])) + " |");
            L("|---|---|" + string.Concat(CAUSES.Select(_ => "---|")));
            foreach (var g in GOVS)
            {
                double y = GovYears(g);
                L($"| {GOV_TR[g]} | {F(y)} | " + string.Join(" | ", CAUSES.Select(c => { double k = Ok.Sum(r => MSum(r, $"collapse_{c}_{g}")); return $"{F(k)} ({F(y > 0 ? k / y * 100 : double.NaN)})"; })) + " |");
            }
            L();
        }
    }
}
