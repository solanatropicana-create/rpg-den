using System;
using System.Collections.Generic;

// Faz 1b-6 (3): devriye (spec §2 "Devriye") ve aç haydutlar (spec §4) — makro istatistik. Balonun dışında devriye istatistiksel
// çözülür: bir devletin topraklarından geçen kervan ve yalnız kahraman her gün devletin devriye sıklığıyla durdurulabilir. Sorgu
// devletin yasasına göre: krallıkta vergi memuru (yabancı kervandan pay), boylarda geçiş haracı ya da düello, cumhuriyette paralı
// muhafız (ucuz rüşvet, hafif kaçakçılık cezası), teokraside sorgu (inanç, ırk, örgüt, çanta; tiefling ve Pakt'a bağlı tutuklanır).
// Kaçak mal: cumhuriyetten ya da Hırsızlar Loncası şubesi olan yerden çıkan kervanlar daha çok taşır; arama sertlikle, rüşvet
// yasanın rüşvet kolaylığıyla. Sayaçlar hükümet tipine göre (patrolStop_kingdom …) ve maruz kalınan ajan-günü (patrolExposure_*).
//
// Aç haydutlar bir örgüt değil, tehdit grubudur: kıtlık ya da ekmek yokluğu çeken yerleşimden halk kaçıp yakına haydut kampı kurar
// (Camp.Kind = "bandit"; sayıları gerçek ekonomiden). Aç haydutlar yiyecek ister: komşu yerleşim tahıl verirse dağılır ve halka
// döner; vermezse akına çıkar. Ganimetle dönen haydut tok olur ve yağma için saldırır (Monsters).

namespace FD.Macro;

public static class Patrol
{
    /// <summary>devriyenin günlük durdurma olasılığı = yasanın devriye sıklığı × STOP_K</summary>
    public const double STOP_K = 0.12;

    private static Civ CivAt(Sim s, int tile)
    {
        int c = s.TileCiv(tile);
        return c >= 0 ? s.W.Civs[c] : null;
    }

    /// <summary>Her gün: devlet topraklarındaki kervanlar ve yalnız serbest kahramanlar devriyeye takılabilir.</summary>
    public static void Tick(Sim s)
    {
        var w = s.W;
        for (int i = 0; i < w.Agents.Count; i++)
        {
            var a = w.Agents[i];
            if (J.T(a.Dead) || a.Hull != null || a.RestUntil != null) continue;
            bool caravan = a.Kind == "caravan";
            bool hero = a.Kind == "hero" && a.Heroes?.Count == 1 && s.Hero(a.Heroes[0])?.Civ == -1;
            if (!caravan && !hero) continue;
            int t = a.Path[Math.Min(a.Step, a.Path.Count - 1)];
            if (J.T(w.Tiles[t].Sea)) continue;
            var c = CivAt(s, t);
            if (c?.Law == null) continue;
            s.Metric("patrolExposure_" + c.Gov);
            if (caravan) s.Metric("patrolCaravanDays_" + c.Gov);
            if (!s.Rng.Chance(c.Law.Patrol * STOP_K)) continue;
            s.Metric("patrolStop");
            s.Metric("patrolStop_" + c.Gov);
            if (caravan) { s.Metric("patrolStopCaravan_" + c.Gov); StopCaravan(s, a, c, t); }
            else StopHero(s, a, s.Hero(a.Heroes[0]), c, t);
        }
    }

    private static double CargoValue(Sim s, Agent a, Civ c)
    {
        double v = 0;
        if (a.Cargo != null) foreach (var kv in a.Cargo) v += kv.Value * s.Price(c, kv.Key);
        return v;
    }

    /// <summary>Kervan sorgusu: yabancı kervandan tipin payı (vergi, haraç, muhafız ücreti, aşar); çantada kaçak mal varsa arama
    /// (sertlik), rüşvet (rüşvet kolaylığı) ya da el koyma.</summary>
    private static void StopCaravan(Sim s, Agent a, Civ c, int t)
    {
        var law = c.Law;
        string g = c.Gov;
        bool foreign = a.Civ != c.Id;
        double value = CargoValue(s, a, c);
        // kaçak mal: cumhuriyetten ya da Hırsızlar Loncası şubesi olan yerden çıkan kervan
        var from = s.Settlement(a.From);
        var oc = a.Civ >= 0 && a.Civ < s.W.Civs.Count ? s.W.Civs[a.Civ] : null;
        double pc = 0.08 + (oc != null && Polity.Smugglers(oc) ? 0.2 : 0) + (from != null && Orgs.HasBranch(s, from, "thieves") ? 0.15 : 0);
        bool contraband = s.Rng.Chance(pc);
        // kaçakçı aramadan önce rüşvet dener: rüşvetin kolay olduğu yerde çoğu kez kabul edilir (cumhuriyetin paralı muhafızı ucuz)
        if (contraband && s.Rng.Chance(law.Bribe * 0.8))
        {
            double bribe = JsMath.Round(JsMath.Max(1, value * 0.05));
            if (oc != null) s.Add(oc, "gold", -JsMath.Min(bribe, s.St(oc, "gold")));
            s.Metric("patrolBribe_" + g);
            s.Metric("patrolBribeGold_" + g, bribe);
            return;
        }
        if (contraband && s.Rng.Chance(law.Harsh))
        {
            // el koyma: yükün bir kısmı devletin ambarına
            double share = 0.3 + 0.5 * law.Smuggle;
            if (a.Cargo != null) foreach (var k in a.Cargo.Keys()) { double q = Math.Floor((a.Cargo.Get(k) ?? 0) * share); if (q <= 0) continue; a.Cargo.Set(k, (a.Cargo.Get(k) ?? 0) - q); s.Add(c, k, q); }
            s.Metric("patrolSeize_" + g);
            s.Metric("patrolSeizeValue_" + g, value * share);
            if (oc != null && foreign && oc.Alive && s.Rel(oc.Id, c.Id).Contact) s.AddMod(oc.Id, c.Id, "seized", "Kervanımıza el koydular", -3, -12, 0.03, false);
            return;
        }
        if (!foreign) return;
        double toll = g switch
        {
            "kingdom" => value * 0.05,              // vergi memuru
            "clans" => 3 + value * 0.02,            // geçiş haracı
            "republic" => 1,                        // paralı muhafız ücreti
            _ => value * 0.03,                      // aşar
        };
        if (g == "clans" && (a.Troops ?? 0) >= 2 && s.Rng.Chance(0.3))
        {
            // haracı vermek istemeyen kervan muhafızı düelloya çıkar: kazanan geçer
            s.Metric("patrolDuel_" + g);
            if (s.Rng.Chance(0.5)) return;
            toll *= 2;
        }
        toll = JsMath.Round(toll);
        if (toll <= 0 || oc == null) return;
        double paid = JsMath.Min(toll, s.St(oc, "gold"));
        s.Add(oc, "gold", -paid); s.Add(c, "gold", paid);
        s.Metric("patrolToll_" + g);
        s.Metric("patrolTollGold_" + g, paid);
    }

    /// <summary>Yalnız kahramanın sorgusu: teokraside yasanın hoş görmediği ırk ya da inanç (Pakt) ve Pakt üyeliği tutuklanır (rüşvet
    /// zor, kaçmak çeviklikle); krallıkta soyguncu (son işleri soygun) tutuklanır; boylarda haraç ya da düello (kazanırsa ün).</summary>
    private static void StopHero(Sim s, Agent a, Hero h, Civ c, int t)
    {
        if (h == null) return;
        var law = c.Law;
        string g = c.Gov;
        bool suspect;
        if (g == "theocracy") suspect = (law.RaceTol.Get(h.Race) ?? 1) < 0.3 || h.Faith == "pact" || (h.Orgs != null && h.Orgs.Exists(m => m.Kind == "pact"));
        else if (g == "kingdom") suspect = h.Path == "dark" && (h.Tally.Get("rob") ?? 0) > 0;
        else suspect = false;
        if (g == "clans")
        {
            if (h.Gold >= 3 && s.Rng.Chance(0.6)) { h.Gold -= 3; s.Add(c, "gold", 3); s.Metric("patrolTollHero_" + g); return; }
            s.Metric("patrolDuelHero_" + g);
            double me = h.Level + Rng.Mod(J.N(h.Stats.Get("str"))) + s.Rng.D20(), them = 3 + s.Rng.D20();
            if (me >= them) { Heroes.AddRenown(s, h, 0.3, "duel"); s.Metric("patrolDuelWon"); }
            else { h.Hp = JsMath.Max(1, h.Hp - Math.Ceiling(h.MaxHp * 0.3)); double take = Math.Floor(h.Gold * 0.5); h.Gold -= take; s.Add(c, "gold", take); }
            return;
        }
        if (!suspect) return;
        if (h.Gold >= 10 && s.Rng.Chance(law.Bribe)) { h.Gold -= 10; s.Metric("patrolBribe_" + g); s.Metric("patrolBribeGold_" + g, 10); return; }
        if (s.Rng.Chance(0.15 + Rng.Mod(J.N(h.Stats.Get("dex"))) * 0.05)) { s.Metric("patrolFled_" + g); return; }
        // tutuklama: en yakın yerleşimin zindanına (teokraside hapis madeni)
        var st = s.Settlement(s.W.Tiles[t].Owner);
        if (st == null || !st.Alive) return;
        s.Metric("patrolArrest_" + g);
        Bondage.Capture(s, h, st, "prison", s.Rng.Int(Bondage.HERO_PRISON_MIN, Bondage.HERO_PRISON_MAX));
    }

    // ------------------------------------------------------------ aç haydutlar
    /// <summary>aç haydut kampının adı ve büyüklüğü</summary>
    public const double BANDIT_MIN = 3, BANDIT_SHARE = 0.06;
    /// <summary>aç yerleşimden WORLD_DAYS başına haydut çıkma olasılığı (Faz 1b-6: 0,12)</summary>
    public const double BANDIT_P = 0.2;
    /// <summary>bir seferde haydut olan en çok (Faz 1b-7: aç büyük şehirden de çıkar)</summary>
    public const double BANDIT_MAX = 6;

    /// <summary>Her WORLD_DAYS günde: açlık ya da ekmek yokluğu çeken yerleşimden halk haydut olur (yakında aç haydut kampı); aç haydutlar
    /// en yakın yerleşimden yiyecek ister (verilirse dağılır, halk döner); aç kalan kamp erir.</summary>
    public static void BanditTick(Sim s)
    {
        var w = s.W;
        foreach (var st in w.Settlements)
        {
            if (!st.Alive || st.Tier < 1 || st.Hub != null) continue;   // Faz 1b-7: aç büyük şehirden de haydut çıkar (RemovePop çekirdek tabanını korur)
            if (!Hungry(st) || !s.Rng.Chance(BANDIT_P)) continue;
            double P = s.Pop(st);
            double n = Math.Floor(JsMath.Max(BANDIT_MIN, JsMath.Min(BANDIT_MAX, P * BANDIT_SHARE)));
            if (P < 8 + n) continue;
            var near = J.Find(w.Camps, cp => cp.Alive && cp.Kind == "bandit" && J.T(cp.Hungry) && s.G.Dist(cp.Tile, st.Tile) <= 10);
            s.RemovePop(st, n);
            s.Metric("banditsBorn", n);
            if (near != null) { near.Count += n; continue; }
            int t = BanditTile(s, st);
            if (t < 0) continue;
            var c = WorldGen.MakeCamp(s.Id(), "bandit", t, $"{st.Name} Kaçakları", s.Day, s.Rng);
            c.Count = n; c.Hungry = true; c.Loot = 0; c.Home = st.Id;
            w.Camps.Add(c); w.Tiles[t].Camp = c.Id;
            s.Metric("banditCamp");
            s.Log("lair", $"{Tr.Ek(st.Name, "dan")} aç kalan {J.S(n)} kişi ormana kaçıp haydut oldu.", tile: t, civ: st.Civ, cause: "Ambar boş, ekmek yok", major: n >= 6);
        }
        foreach (var cp in w.Camps)
        {
            if (!cp.Alive || cp.Kind != "bandit") continue;
            if (!J.T(cp.Hungry)) continue;
            // yiyecek ister: en yakın yerleşim verirse dağılırlar
            Settlement best = null; double bd = double.PositiveInfinity;
            foreach (var x in w.Settlements) { if (!x.Alive) continue; double d = s.G.Dist(x.Tile, cp.Tile); if (d < bd) { bd = d; best = x; } }
            var c = best != null && best.Civ >= 0 ? w.Civs[best.Civ] : null;
            double need = cp.Count * 6;
            if (c != null && bd <= 12 && s.St(c, "grain") >= need * 2 && s.Rng.Chance(0.2 + (Polity.Generous(c) ? 0.3 : 0) + c.Align.Good * 0.2))
            {
                s.Add(c, "grain", -need);
                var home = s.Settlement(cp.Home) is { Alive: true } hs ? hs : best;
                s.AddPop(home, home.Pop.Count > 0 ? home.Pop.Keys()[0] : c.Race, cp.Count);
                s.Metric("banditFed", cp.Count);
                cp.Alive = false; cp.Count = 0; cp.ClearedDay = s.Day; w.Tiles[cp.Tile].Camp = null;
                s.Log("lair", $"{c.Name}, {Tr.Ek(cp.Name, "a")} {J.S(need)} tahıl verdi; aç haydutlar dağılıp evlerine döndü.", tile: cp.Tile, civ: c.Id, cause: "Yiyecek verirsen giderler");
                continue;
            }
            // aç kalan haydut kampı erir
            if (s.Rng.Chance(0.25)) { cp.Count--; s.Metric("banditStarved"); }
            if (cp.Count <= 0) { cp.Alive = false; cp.ClearedDay = s.Day; w.Tiles[cp.Tile].Camp = null; }
        }
    }

    /// <summary>Yerleşim aç mı: açlık, ekmek yokluğu ya da (Faz 1b-7) yerel kıtlık durumu.</summary>
    public static bool Hungry(Settlement st) => st.Starving > 0 || (st.Hunger ?? 0) > 0 || (st.Lack != null && (st.Lack.Get("bread") ?? 0) >= 10) || st.Status == "shortage" || st.Status == "hunger";

    private static int BanditTile(Sim s, Settlement st)
    {
        int best = -1; double bs = double.NegativeInfinity;
        foreach (int i in s.G.Within(st.Tile, 7))
        {
            var t = s.W.Tiles[i];
            double d = s.G.Dist(i, st.Tile);
            if (d < 3 || t.Terrain == "water" || t.Terrain == "mountain" || J.T(t.Sea) || t.Owner >= 0 || t.Camp != null || t.Inn != null || t.InnZone != null) continue;
            double sc = (t.Terrain == "forest" || t.Terrain == "hill" ? 2 : 0) - Math.Abs(d - 5) + s.Rng.Next();
            if (sc > bs) { bs = sc; best = i; }
        }
        return best;
    }
}
