using System;
using System.Collections.Generic;
using System.Linq;

// Dünya olayları: salgın, kasaba yangını, kıtlık göçü.
// Port of src/sim/events.ts.

namespace FD.Macro;

public static class Events
{
    // hekimlik bilgisi, sunak ve sınıf şifası kalıcı korur; iksirli şifa evi salgını önler, salgın sırasında iksir harcanır (plaguePotions)
    private static double Medicine(Sim s, Civ c, Settlement st) =>
        (s.Has(c, "medicine") ? 0.45 : 0) + ((st.Workshops.Get("apothecary") ?? 0) > 0 && s.St(c, "potion") >= 2 ? 0.2 : 0) + (J.T(st.Civics.Get("temple")) ? 0.1 : 0) + JsMath.Min(0.3, s.E(c, "healBack") * 0.3);

    /// <summary>TS literal <c>[24, 55, 120, 230]</c> (crowding thresholds by tier).</summary>
    private static readonly double[] CROWD = { 24, 55, 120, 230 };

    /// <summary>ayda bir çağrılır: salgın (başlama/yayılma/bitiş), kasaba ve baraka yangını, iç göç, kıtlık göçü</summary>
    public static void DisastersTick(Sim s)
    {
        var w = s.W;
        var alive = J.Filter(w.Settlements, x => x.Alive);
        // --- salgın: başlama, yayılma, ölüm, bitiş
        foreach (var st in alive)
        {
            var c = w.Civs[st.Civ];
            double P = s.Pop(st);
            var pl = st.Plague;
            if (pl != null)
            {
                double @base = JsMath.Min(0.85, Medicine(s, c, st));
                double sick = JsMath.Max(1, JsMath.Round(P * 0.05 * pl.Severity * (1 - @base)));
                double prot = JsMath.Min(0.92, @base + Gear.PlaguePotions(s, c, sick) * (1 - @base));
                if (prot > @base + 0.05) pl.Potions = (pl.Potions ?? 0) + 1;
                double deaths = JsMath.Min(P - 2, JsMath.Max(1, JsMath.Round(P * 0.05 * pl.Severity * (1 - prot))));
                if (deaths > 0) { s.RemovePop(st, deaths); pl.Dead += deaths; st.Graves = (st.Graves ?? 0) + deaths; s.Metric("plagueDead", deaths); }
                if (s.Day >= pl.Until || s.Pop(st) <= 3)
                {
                    s.Log("world", $"{Tr.Ek(st.Name, "da")} salgın sona erdi; {J.S(pl.Dead)} kişi hayatını kaybetti.", civ: c.Id, tile: st.Tile, major: pl.Dead >= 5, cause: (pl.Potions ?? 0) > 0 ? "Şifa evinin iksirleri hastaları ayağa kaldırdı" : prot > 0.3 ? "Hekimler hastalığı dizginledi" : "Hastalık kendi yolunu tamamladı");
                    st.Plague = null; st.PlagueImmune = s.Day + 3 * Sim.YEAR;
                }
                continue;
            }
            double hl = s.Homeless(st);
            if ((P < 28 && hl < 8) || P < 14 || (st.PlagueImmune ?? 0) > s.Day) continue;
            double crowded = P > CROWD[st.Tier] * 0.85 ? 1.6 : 1;
            double shanty = 1 + JsMath.Min(2.5, (hl / P) * 4);
            double chance = 0.0035 * (P / 40) * crowded * shanty * (1 - JsMath.Min(0.85, Medicine(s, c, st))) * (st.Starving > 0 ? 2 : 1) * (Gear.IsCold(s, c) ? 1.3 : 1);
            if (s.Rng.Chance(chance)) StartPlague(s, st, hl >= 6 ? $"Sur dışındaki barakalarda {J.S(hl)} kişi üst üste yaşıyordu" : "Kalabalık sokaklar ve kirli kuyular");
        }
        // yayılma: ticaret yolları ve yakın komşular
        foreach (var st in J.Filter(alive, x => x.Plague != null && x.Plague.Since < s.Day - 20))
        {
            // Set<number>: insertion order, no duplicates
            var links = new List<int>();
            void Link(int id) { if (!links.Contains(id)) links.Add(id); }
            foreach (var r in w.Routes) if (r.Alive) { if (r.A == st.Id) Link(r.B); if (r.B == st.Id) Link(r.A); }
            foreach (var o in alive) if (o.Id != st.Id && s.G.Dist(o.Tile, st.Tile) <= 6) Link(o.Id);
            foreach (int id in links)
            {
                var o = s.Settlement(id);
                if (o == null || !o.Alive || o.Plague != null || (o.PlagueImmune ?? 0) > s.Day || s.Pop(o) < 8) continue;
                double prot = JsMath.Min(0.85, Medicine(s, w.Civs[o.Civ], o));
                if (s.Rng.Chance(0.12 * (1 - prot))) StartPlague(s, o, $"{Tr.Ek(st.Name, "dan")} gelen yolcular hastalığı taşıdı");
            }
        }
        // --- kasaba yangını: ahşap evler, kuru yaz
        foreach (var st in alive)
        {
            double wood = (st.Civics.Get("hut") ?? 0) + (st.Civics.Get("house") ?? 0), stone = st.Civics.Get("stonehouse") ?? 0;
            if (wood < 5) continue;
            double summer = s.Season == 1 ? 2 : 1;
            double chance = 0.0028 * summer * (wood / (wood + stone * 2)) * (J.T(st.Civics.Get("stonewall")) ? 0.8 : 1);
            if (!s.Rng.Chance(chance)) continue;
            double burnt = JsMath.Min(JsMath.Max(2, JsMath.Round(wood / 4)), 2 + s.Rng.Int(0, 4));
            st.BurnedHouses = (st.BurnedHouses ?? 0) + burnt; st.BurnedAt = s.Day;
            double dead = s.Rng.Chance(0.5) ? s.Rng.Int(1, 2) : 0;
            if (J.T(dead)) s.RemovePop(st, dead);
            s.Metric("townFire");
            s.Log("world", $"{Tr.Ek(st.Name, "da")} yangın çıktı: {J.S(burnt)} ev kül oldu{(J.T(dead) ? $", {J.S(dead)} kişi öldü" : "")}.", civ: st.Civ, tile: st.Tile, major: true, cause: s.Season == 1 ? "Kurak yaz, ahşap çatılar" : "Devrilen bir kandil, sık ahşap evler");
        }
        // --- barakalar: konutu yetmeyen halk sur dışında yaşar; yangın riski, iç göç
        foreach (var st in alive)
        {
            double hl = s.Homeless(st); var c = w.Civs[st.Civ];
            if (hl >= 5 && !J.T(st.ShantyLog))
            {
                st.ShantyLog = true;
                s.Log("economy", $"{Tr.Ek(st.Name, "da")} konut yetmiyor: {J.S(hl)} kişi sur dışında barakalarda yaşıyor.", civ: c.Id, tile: st.Tile, cause: (st.BurnedHouses ?? 0) > 0 ? "Yanan evler henüz onarılmadı" : "Gelen göçmenler evlerden hızlı çoğaldı");
            }
            else if (hl == 0 && J.T(st.ShantyLog))
            {
                st.ShantyLog = false;
                s.Log("economy", $"{Tr.Ek(st.Name, "da")} barakalar boşaldı; herkesin bir evi var.", civ: c.Id, tile: st.Tile);
            }
            if (hl < 4) continue;
            // baraka yangını
            if (s.Rng.Chance(0.004 * JsMath.Min(3, hl / 8) * (s.Season == 1 ? 2 : 1)))
            {
                double dead = s.Rng.Chance(0.4) ? 1 : 0;
                if (J.T(dead)) s.RemovePop(st, dead);
                s.Metric("shantyFire");
                s.Log("world", $"{Tr.Ek(st.Name, "in")} barakalarında yangın çıktı{(J.T(dead) ? "; bir kişi öldü" : "")}.", civ: c.Id, tile: st.Tile, cause: "Sık çadırlar, açık ocaklar");
            }
            // iç göç: aynı medeniyette boş evi olan yerleşime yürürler
            if (J.Some(w.Agents, a => a.Kind == "settlers" && a.Purpose == "homeless" && a.From == st.Id)) continue;
            var to = J.At(J.Sort(J.Filter(s.CivSettlements(c), o => o.Id != st.Id && s.Housing(o) - s.Pop(o) >= 3 && o.Starving == 0),
                (a, b) => s.G.Dist(a.Tile, st.Tile) - s.G.Dist(b.Tile, st.Tile)), 0);
            if (to == null || !s.Rng.Chance(0.6)) continue;
            var path = s.Path(st.Tile, to.Tile);
            if (path == null) continue;
            double n = JsMath.Min(hl, s.Housing(to) - s.Pop(to), 8);
            var pop = s.RemovePop(st, n);
            w.Agents.Add(new Agent { Id = s.Id(), Kind = "settlers", Civ = c.Id, Path = path, Step = 0, Progress = 0, Speed = 0.45, Pop = pop, From = st.Id, To = to.Id, Purpose = "homeless" });
            s.Metric("homelessMove");
            s.Log("migration", $"{Tr.Ek(st.Name, "in")} barakalarından {J.S(n)} kişi, boş evleri olan {Tr.Ek(to.Name, "a")} yola çıktı.", civ: c.Id, tile: st.Tile);
        }
        // --- kıtlık göçü: aç kalan halk, yiyeceği olan komşuya yürür
        foreach (var c in J.Filter(w.Civs, x => x.Alive))
        {
            var ss = J.Filter(s.CivSettlements(c), x => x.Starving > 20 && s.Pop(x) > 8);
            if (ss.Count == 0 || J.Some(w.Agents, a => a.Kind == "settlers" && a.Purpose == "refugee" && a.Civ == c.Id)) continue;
            var from = J.Sort(ss, (a, b) => s.Pop(b) - s.Pop(a))[0];
            var targets = J.Sort(J.Filter(J.Filter(alive, o => o.Civ != c.Id && w.Civs[o.Civ].Alive),
                    o => o.Starving == 0 && !s.AtWar(c.Id, o.Civ) && s.G.Dist(o.Tile, from.Tile) <= 22 && s.Pop(o) < s.Housing(o) + 6),
                (a, b) => s.G.Dist(a.Tile, from.Tile) - s.G.Dist(b.Tile, from.Tile));
            var to = J.At(targets, 0);
            if (to == null || !s.Rng.Chance(0.5)) continue;
            var path = s.Path(from.Tile, to.Tile);
            if (path == null) continue;
            double n = JsMath.Min(6, JsMath.Max(2, JsMath.Round(s.Pop(from) * 0.12)));
            var pop = s.RemovePop(from, n);
            w.Agents.Add(new Agent { Id = s.Id(), Kind = "settlers", Civ = c.Id, Path = path, Step = 0, Progress = 0, Speed = 0.45, Pop = pop, From = from.Id, To = to.Id, Purpose = "refugee" });
            s.Metric("famineMigration");
            s.Log("migration", $"Açlıktan kaçan {J.S(n)} kişi {Tr.Ek(from.Name, "dan")} {Tr.Ek(to.Name, "a")} doğru yola düştü.", civ: c.Id, tile: from.Tile, major: true, cause: $"{c.Name} ambarları boş");
        }
    }

    private static void StartPlague(Sim s, Settlement st, string why)
    {
        // object literal order: since, until (rng.int), severity (rng.next)
        double since = s.Day;
        double until = s.Day + s.Rng.Int(60, 150);
        double severity = 0.6 + s.Rng.Next() * 0.8;
        st.Plague = new PlagueInfo { Since = since, Until = until, Severity = severity, Dead = 0 };
        s.Metric("plague");
        s.Log("world", $"{Tr.Ek(st.Name, "da")} salgın başladı!", civ: st.Civ, tile: st.Tile, major: true, cause: why);
    }
}
