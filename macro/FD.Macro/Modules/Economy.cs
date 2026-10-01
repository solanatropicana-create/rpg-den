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
    private static readonly double[] SEASON_FARM = { 1.0, 1.3, 1.1, 0.25 };
    private static readonly double[] SEASON_WILD = { 1.0, 1.2, 1.1, 0.5 };
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

    /// <summary>Hex'teki yapının işçi başına günlük verimi (level/kind default: t.Ext.Level / t.Ext.Kind).</summary>
    public static (string Good, double Y) ExtractYield(Sim s, Civ c, Tile t, int? level = null, string kind = null)
    {
        int lvl = level ?? t.Ext.Level;
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
        double m = 1 + s.E(c, "prodAll");
        bool food = good == "grain" || good == "meat" || good == "fish";
        if (food) m += s.E(c, "prodFood");
        if (good == "wood") m += s.E(c, "prodWood");
        if (knd == "mine" || knd == "mithril" || knd == "quarry") m += s.E(c, "prodMine");
        if (good == "herbs") m += s.E(c, "prodHerbs");
        if (good == "mana") m += s.E(c, "prodMana");
        double season = 1;
        bool winterImmune = s.E(c, "winterImmune") > 0;
        // harvest (Kadim Şampiyon: kayıpsız hasat): tarlalar hiçbir mevsimde verim kaybetmez (yaz/güz artısı kalır)
        if (knd == "farm") season = winterImmune || s.E(c, "harvest") > 0 ? JsMath.Max(1, SEASON_FARM[s.Season]) : SEASON_FARM[s.Season];
        else if (knd == "hunt" || knd == "dock" || knd == "herbalist") season = winterImmune ? 1 : SEASON_WILD[s.Season];
        double toolF = 1;
        if (lvl >= 2 && s.St(c, "tools") < 0.5) toolF = 0.75;
        return (good, rate * D.LEVEL_MULT[lvl] * richness * m * season * toolF);
    }

    private static double Demand(Sim s, Civ c, string g, double P)
    {
        double w = c.Want.Get(g) ?? 0;
        switch (g)
        {
            case "wood": return 30 + P * 1.5 + w;
            case "stone": return 15 + P * 0.8 + w;
            case "bricks": return (s.Has(c, "pottery") ? 8 + P * 0.15 : 0) + w;
            case "leather": return 6 + P * 0.3 + w;        // bir kışlık giysi + silah
            case "tools": return (s.Has(c, "bronzetools") ? 4 + P * 0.05 : 0) + w;
            case "arms": return (s.Has(c, "enchanting") ? 3 : 0) + w;
            case "horses": return (s.Has(c, "husbandry") ? 4 + P * 0.03 : 1) + w;
            case "potion": return (s.Has(c, "medicine") ? 4 + P * 0.06 : 0) + w;
            case "mana": return (s.Has(c, "arcana1") ? 5 + P * 0.04 : 0) + (s.Has(c, "enchanting") ? 6 : 0) + w;
            case "mithril": return (s.Has(c, "mithrilwork") ? 8 : 1) + w;
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
            if ((st.BurnedHouses ?? 0) > 0 && s.Day - (st.BurnedAt ?? 0) > 20 && s.St(c, "wood") >= 4)
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

    /// <summary>Daily economy of a civ: jobs and production, consumption/spoilage, growth or famine, research progress, deposit regen.</summary>
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
        double researchPts = 0;
        var cls = D.CLASSES[c.Cls];
        double resShare = JsMath.Max(0.08, 0.17 + (c.Cls == "wizard" ? 0.07 : 0) + (c.Cls == "barbarian" ? -0.05 : 0));

        foreach (var st in ss)
        {
            double P = s.Pop(st);
            if (P <= 0) { s.Abandon(st, "Yerleşimde kimse kalmadı"); continue; }
            var jobs = new JsObj<double>();
            bool famine = daysFood < 12;
            // kıtlıkta askerler de toplayıcılığa çıkar, araştırma durur
            double avail = famine ? P : JsMath.Max(0, P - st.Soldiers);
            jobs.Set("asker", famine ? 0 : st.Soldiers);
            // araştırmacılar önce ayrılır
            double nr = J.T(c.Research.Current) && !famine ? JsMath.Min(avail, JsMath.Max(avail >= 3 ? 1 : 0, JsMath.Round(avail * resShare))) : 0;
            avail -= nr; jobs.Set("araştırmacı", nr);

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
                var (good, y) = ExtractYield(s, c, t);
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
            double gy = 0.14 * SEASON_WILD[s.Season] * (1 + s.E(c, "prodFood") * 0.5);
            opts.Add(new JobOpt { Key = "toplayıcı", Slots = JsMath.Min(10, forestGrass), V = gy * s.Price(c, "grain") * foodUrg * 0.9 + 0.03 * s.Price(c, "wood"), Run = n => { s.Add(c, "grain", gy * n); s.Add(c, "wood", 0.03 * n); } });
            // atölyeler
            foreach (var k in D.WORKSHOP_IDS)
            {
                double cnt = st.Workshops.Get(k) ?? 0;
                if (!J.T(cnt)) continue;
                var def = D.WORKSHOPS[k];
                var inp = PickInputs(s, c, def.Inputs, def.Alt != null && (!J.T(def.AltTech) || s.Has(c, def.AltTech)) ? def.Alt : null);
                if (inp == null) continue;
                double margin = (s.Price(c, def.Output) - s.Worth(c, inp)) * (s.St(c, def.Output) > 150 + totalPop * 12 || glut(def.Output) ? 0.08 : 1);
                double perW = def.Rate * (1 + s.E(c, "prodAll") * 0.5);
                opts.Add(new JobOpt
                {
                    Key = def.Name, Slots = def.Slots * cnt, V = perW * JsMath.Max(0.05, margin), Run = n =>
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
            jobs.Set("zanaatçı", avail);
            s.Add(c, "gold", avail * 0.02);
            st.Jobs = jobs;
            // araştırma puanı (ırk yatkınlığı yok; sınıf etkileri); bilginler mana kristali yakarsa +%25
            if (J.T(nr) && J.T(c.Research.Current))
            {
                double lib = (st.Civics.Get("library") ?? 0) > 0 ? 0.2 : 0;
                double manaUse = nr * 0.01;
                double mana = s.Has(c, "arcana1") && s.St(c, "mana") >= manaUse ? 0.25 : 0;
                if (J.T(mana)) s.Add(c, "mana", -manaUse);
                researchPts += nr * 0.3 * (1 + s.E(c, "research") + lib + mana);
            }
        }

        // Tüketim
        double need = totalPop * Sim.FOOD_PER_POP * (1 - JsMath.Min(0.5, s.E(c, "frugal")));
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
        double happy = 0;
        if (s.St(c, "beer") > totalPop * 0.01) { s.Add(c, "beer", -totalPop * 0.004); happy = 0.15; }
        double l2 = 0;
        for (int i = 0; i < W.Tiles.Count; i++) { var t = W.Tiles[i]; if (t.Ext != null && t.Ext.Level >= 2 && t.Owner >= 0 && J.Some(ss, x => x.Id == t.Owner)) l2++; }
        if (J.T(l2)) s.Add(c, "tools", -l2 * 0.0015 * (s.Has(c, "smithing") ? 0.5 : 1)); // demir aletler geç aşınır
        Gear.WearTick(s, c, ss);
        Gear.GearTick(s, c);
        double soldiers = 0;
        foreach (var x in ss) soldiers = soldiers + x.Soldiers;
        double mint = 0;
        foreach (var x in ss) mint += MintIncome(s, x);
        s.Add(c, "gold", totalPop * 0.004 * (1 + s.E(c, "tax")) + mint - soldiers * 0.006);

        if (need > 0.01)
        {
            if (s.E(c, "noFamine") > 0) { s.Add(c, "grain", need); }
            else foreach (var st in s.CivSettlements(c))
            {
                st.Starving++;
                if (st.Starving == 1) s.Log("economy", $"{Tr.Ek(st.Name, "da")} kıtlık başladı.", civ: c.Id, tile: st.Tile, cause: $"Gıda stoğu tükendi ({s.DateStr()})");
                // açlık, açığın büyüklüğüyle orantılı birikir
                st.Hunger = (st.Hunger ?? 0) + JsMath.Min(1, need / JsMath.Max(0.05, totalPop * Sim.FOOD_PER_POP));
                if (st.Hunger >= 10) { st.Hunger -= 10; s.RemovePop(st, 1); s.Metric("starved"); }
            }
        }
        else
        {
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
                    st.GrowthAcc += rate * 0.0058 * (daysFood > 30 ? 1 : 0.5) * crowd * (1 + s.E(c, "growth") + happy) * (Gear.IsCold(s, c) ? 0.5 : 1); // üşüyen halk yavaş büyür
                    while (st.GrowthAcc >= 1)
                    {
                        st.GrowthAcc -= 1;
                        var rs = J.Filter(st.Pop.Keys(), r => (st.Pop.Get(r) ?? 0) > 0);
                        s.AddPop(st, s.Rng.Weighted(rs, x => (st.Pop.Get(x) ?? 0) * D.RACES[x].Growth) ?? c.Race, 1);
                    }
                }
                if (s.Rng.Chance(P * 0.00025)) s.RemovePop(st, 1);
            }
        }

        // Araştırma
        if (J.T(c.Research.Current))
        {
            var t = D.TECH[c.Research.Current];
            c.Research.Progress += researchPts;
            double cost = ResearchCost(s, c, t);
            if (c.Research.Progress >= cost)
            {
                c.Research.Done.Add(t.Id);
                c.Research.Current = null;
                c.Research.Progress = 0;
                Research.OnTechDone(s, c, t.Id);
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

    /// <summary>Orman yeniden büyümesi (30 günde bir, dünya düzeyinde)</summary>
    public static void RegrowForests(Sim s)
    {
        for (int i = 0; i < s.W.Tiles.Count; i++)
        {
            var t = s.W.Tiles[i];
            if (t.CutDay == null || t.Terrain != "grass" || t.Ext != null || t.Deposit >= 0) continue;
            int owner = s.TileCiv(i);
            bool fast = owner >= 0 && s.Has(s.W.Civs[owner], "forestry");
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
            if (k == "wonder")
            {
                s.RecomputeEff(c);
                bool first = !J.T(s.W.Metrics.Get("wonder") ?? 0);
                s.Metric("wonder");
                var wd = D.WONDERS[c.Cls];
                foreach (var o in s.W.Civs) if (o.Alive && o.Id != c.Id && s.Rel(o.Id, c.Id).Contact) s.AddMod(o.Id, c.Id, "wonder", $"{wd.Name} hayranlığı", first ? 12 : 6, 12, 0.01, false);
                s.Log("wonder", first ? $"DÜNYANIN İLK HARİKASI: {c.Name}, {Tr.Ek(st.Name, "da")} {wd.Name} inşasını tamamladı!" : $"{c.Name}, {Tr.Ek(st.Name, "da")} {wd.Name} inşasını tamamladı.", civ: c.Id, tile: st.Tile, major: true, cause: $"{wd.Desc}{(first ? "; bütün diyar hayranlıkla izliyor" : "")}");
                return;
            }
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

    /// <summary>Effective research cost of a tech for this civ (hard path ×2.2, cheapKnown ×0.5), Math.round'ed.</summary>
    public static double ResearchCost(Sim s, Civ c, TechDef t)
    {
        double cost = D.TechCost(t) * (J.T(c.Research.Hard) && c.Research.Current == t.Id ? 2.2 : 1);
        if (J.T(s.E(c, "cheapKnown")) && J.Some(s.W.Civs, o => o.Id != c.Id && o.Alive && s.Rel(c.Id, o.Id).Contact && o.Research.Done.Contains(t.Id))) cost *= 0.5;
        return JsMath.Round(cost);
    }

    /// <summary>Display name of a civic ('unique' and 'wonder' resolve per class).</summary>
    public static string CivicName(Civ c, string k) => k == "unique" ? UNIQUE_BUILDING[c.Cls] ?? "Sınıf yapısı" : k == "wonder" ? D.WONDERS[c.Cls].Name : D.CIVICS[k].Name;

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
        bool civicAvail(string k) { var d = D.CIVICS[k]; return !J.T(d.Tech) || s.Has(c, d.Tech); }
        // Barınma
        if (P >= cap - 2)
        {
            foreach (var (k, sc) in HOUSING_CANDS)
            {
                if (!civicAvail(k)) continue;
                if (k == "hut" && (st.Civics.Get("hut") ?? 0) >= 6 && s.Has(c, "woodwork")) continue;
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
                foreach (var kind in PossibleKinds(s, c, t, ti))
                {
                    int lvl = D.EXTRACTS[kind].StartLevel ?? 1;
                    var fake = t.Clone();
                    fake.Ext = new ExtractBuilding { Kind = kind, Level = lvl, Settlement = st.Id, Workers = 0 };
                    var (good, y) = ExtractYield(s, c, fake, lvl, kind);
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
            if (!J.T(def.Names[nl - 1]) || !J.T(def.Tech[nl - 1]) || !s.Has(c, def.Tech[nl - 1])) continue;
            if (t.Ext.Kind == "dock" && nl == 3 && !J.Some(s.G.Neighbors(ti), n => J.T(W.Tiles[n].Sea))) continue; // balıkçı filosu açık denize açılır
            if (t.Ext.Workers < D.LEVEL_SLOTS[t.Ext.Level] - 1) continue;
            var cur = ExtractYield(s, c, t);
            var nxt = ExtractYield(s, c, t, nl);
            double gain = (nxt.Y * D.LEVEL_SLOTS[nl] - cur.Y * t.Ext.Workers) * s.Price(c, cur.Good);
            outp.Add(new Cand { Score = gain * 10 + 8, Cost = D.LEVEL_COST[nl], Work = 10 + nl * 8, Project = new Project { Type = "upgrade", Kind = t.Ext.Kind, Tile = ti, Level = nl } });
        }
        // Denizcilik: tersane, fener, gemiler (yerleşim yuvası kullanmaz); filo medeniyet çapında hedeflenir
        var yards = J.Filter(s.CivSettlements(c), x => J.T(x.Civics.Get("shipyard")) && x.Port != null);
        if (s.Has(c, "boatbuilding") && !J.T(st.Civics.Get("shipyard")) && Sea.PickPort(s, st) >= 0)
        {
            bool near = J.Some(yards, x => s.G.Dist(x.Tile, st.Tile) <= 10);
            double sc = yards.Count == 0 ? 30 + (D.CLASSES[c.Cls].Prefer.Get("deniz") ?? 1) * 6 : J.T(st.Overseas) || (st.Tier >= 2 && !near) ? 12 : 0;
            if (sc > 0) outp.Add(new Cand { Score = sc, Cost = D.CIVICS["shipyard"].Cost, Work = D.CIVICS["shipyard"].Work, Project = new Project { Type = "civic", Kind = "shipyard" } });
        }
        if (J.T(st.Civics.Get("shipyard")) && st.Port != null)
        {
            if (s.Has(c, "seatrade") && !J.T(st.Civics.Get("lighthouse")) && (st.Tier >= 2 || !J.Some(yards, x => J.T(x.Civics.Get("lighthouse"))))) outp.Add(new Cand { Score = 14, Cost = D.CIVICS["lighthouse"].Cost, Work = D.CIVICS["lighthouse"].Work, Project = new Project { Type = "civic", Kind = "lighthouse" } });
            double routes = J.Filter(W.Routes, r => r.Alive && J.T(r.Sea) && (s.Settlement(r.A)?.Civ == c.Id || s.Settlement(r.B)?.Civ == c.Id)).Count;
            double want = JsMath.Min(7, 1 + (s.Has(c, "shipbuilding") ? 1 : 0) + (s.Has(c, "navigation") ? 1 : 0) + (s.Has(c, "seatrade") ? 1 : 0) + routes);
            var fl = Sea.Fleet(s, c); double n = JsMath.Max(1, yards.Count);
            double have = st.Ships ?? 0;
            if (fl.Ships < want && have < Math.Ceiling(want / n)) outp.Add(new Cand { Score = 22 + (fl.Ships == 0 ? 14 : 0) + routes * 3, Cost = D.SHIPS["hull"].Cost, Work = D.SHIPS["hull"].Work, Project = new Project { Type = "ship", Kind = "hull" } });
            if (s.Has(c, "navy"))
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
                if (!s.Has(c, def.Tech)) continue;
                double have = st.Workshops.Get(k) ?? 0;
                if (have >= (st.Tier >= 2 ? 2 : 1) || IsGlut(s, c, def.Output, s.CivPop(c))) continue;
                var alt = def.Alt != null && (!J.T(def.AltTech) || s.Has(c, def.AltTech)) ? def.Alt : null;
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
            if (st.Tier >= 1) AddCivic("stonewall", 10 + threat * 30 + st.Tier * 6);
            if (isCap && st.Tier >= 2) AddCivic("castle", 20 + threat * 25);
            if (isCap && c.Era >= 4 && st.Tier >= 2 && !J.T(st.Civics.Get("wonder")) && !J.Some(s.CivSettlements(c), x => J.T(x.Civics.Get("wonder"))))
            {
                var d = D.CIVICS["wonder"];
                double rivals = J.Filter(s.W.Settlements, x => x.Alive && x.Civ != c.Id && (J.T(x.Civics.Get("wonder")) || x.Project?.Kind == "wonder")).Count;
                outp.Add(new Cand { Score = 44 + rivals * 6, Cost = d.Cost, Work = d.Work, Project = new Project { Type = "civic", Kind = "wonder" } });
            }
            if (isCap && J.T(c.Subclass) && !J.T(st.Civics.Get("unique")))
            {
                var d = D.CIVICS["unique"];
                outp.Add(new Cand { Score = 40, Cost = d.Cost, Work = d.Work, Project = new Project { Type = "civic", Kind = "unique" } });
            }
        }
        return outp;
    }

    private static bool Producing(Sim s, Civ c, string g)
    {
        foreach (var t in s.W.Tiles) if (t.Ext != null && !J.T(t.Ext.Depleted) && t.Ext.Workers > 0 && t.Owner >= 0 && s.Settlement(t.Owner)?.Civ == c.Id && ExtractGood(t, s) == g) return true;
        return false;
    }

    /// <summary>Extraction building kinds (ExtractKind) that can be built on tile ti.</summary>
    public static List<string> PossibleKinds(Sim s, Civ c, Tile t, int ti)
    {
        var outp = new List<string>();
        bool lv1(string k) { var def = D.EXTRACTS[k]; int sl = def.StartLevel ?? 1; string tech = def.Tech[sl - 1]; return !J.T(tech) || s.Has(c, tech); }
        if (t.Deposit >= 0)
        {
            var d = J.Find(s.W.Deposits, x => x.Id == t.Deposit);
            if (!s.DepositVisible(c, d) || d.Depleted) return outp;
            string kind = D.DEPOSITS[d.Kind].Building;
            if (kind == "grove") { if (c.Cls == "druid" && s.Has(c, "druid_grove")) outp.Add("grove"); return outp; }
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
        bool tribal = c.Cls == "barbarian" && (s.Has(c, "barbarian_totem") || s.Has(c, "barbarian_raiders"));
        bool unarmed = s.E(c, "unarmed") > 0;
        if (s.Has(c, "training") || tribal || unarmed) ratio = (0.07 + cls.Aggression * 0.07 + s.E(c, "warband")) * (war ? 1.8 : 1) * (c.Threat > 0.5 ? 1.3 : 1);
        double deficit = 0;
        foreach (var st in s.CivSettlements(c))
        {
            double target = Math.Floor(s.Pop(st) * ratio);
            if (st.Soldiers < target)
            {
                double n = JsMath.Min(2, target - st.Soldiers);
                for (int i = 0; i < n; i++)
                {
                    if (s.St(c, "arms") >= 1) { s.Add(c, "arms", -1); st.Soldiers++; }
                    else if (tribal && s.St(c, "leather") >= 2) { s.Add(c, "leather", -2); st.Soldiers++; }
                    else if (unarmed && s.St(c, "grain") >= 4) { s.Add(c, "grain", -4); st.Soldiers++; }
                    // silah yoksa: mızrak ve deri zırhla hafif piyade
                    else if (s.Has(c, "training") && s.St(c, "leather") >= 2 && s.St(c, "wood") >= 3) { s.Add(c, "leather", -2); s.Add(c, "wood", -3); st.Soldiers++; }
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
