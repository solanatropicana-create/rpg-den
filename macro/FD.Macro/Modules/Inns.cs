using System;
using System.Collections.Generic;
using System.Linq;

// Tarafsız hanlar: serbest kahraman havuzu, açık artırma, sözleşme, misafir hakkı, hanın panosu.
// Port of src/sim/inns.ts.

namespace FD.Macro;

public static class Inns
{
    public const double INN_POOL = 4;
    public const double CONTRACT_YEARS = 5;
    public const double MAX_INN_HEROES = 2;

    /// <summary>Yuvası bu han olan, bağımsız ve hayattaki (emekli olmayan) kahramanlar.</summary>
    public static List<Hero> InnPool(Sim s, Inn inn) =>
        J.Filter(s.W.Heroes, h => h.BaseInn && h.Base == inn.Id && h.Civ == -1 && h.State != "dead" && h.State != "gone" && h.State != "retired");

    /// <summary>Medeniyetin handan sözleşmeyle tuttuğu (hayattaki) kahramanlar.</summary>
    public static List<Hero> InnHeroesOf(Sim s, Civ c) =>
        J.Filter(s.W.Heroes, h => h.Contract?.Civ == c.Id && h.Civ == c.Id && h.State != "dead" && h.State != "gone");

    /// <summary>Paktçı mı (pakt etkisi ya da çok kötü hizalama).</summary>
    public static bool IsPact(Sim s, Civ c) => s.E(c, "pact") > 0 || c.Align.Good < -0.5;

    /// <summary>"&lt;ad&gt; Hanı"</summary>
    public static string InnName(Inn inn) => $"{inn.Name} Hanı";

    // ------------------------------------------------------------ doğum
    private static readonly List<string> MIXED_RACES = new() { "halfelf", "halforc" };
    private static readonly List<string> HUMAN_ONLY = new() { "human" };

    private static void SpawnInnHero(Sim s, Inn inn)
    {
        var races = J.Unique(J.Map(J.Filter(s.W.Civs, c => c.Alive), c => c.Race));
        string race = s.Rng.Chance(0.15) ? s.Rng.Pick(MIXED_RACES) : s.Rng.Pick(races.Count > 0 ? races : HUMAN_ONLY);
        string cls = s.Rng.Pick(D.HERO_CLASS_IDS);
        bool teacher = inn.Teacher != null && J.Find(s.W.Heroes, h0 => h0.Id == inn.Teacher)?.State == "retired";
        int level = Heroes.BirthLevel(s, teacher);
        var h = Heroes.MakeHero(s, (race, cls, level, inn.Tile, inn.Id, true));
        s.Metric("innHeroSpawn");
        s.Log("inn", $"{InnName(inn)} kapısından bir yabancı girdi: {s.HeroTitle(h)}.", tile: inn.Tile, cause: $"{Heroes.UpperFirst(h.Bio)}{(teacher ? " Emekli bir kahramanın yanında yetişti." : "")}");
    }

    // ------------------------------------------------------------ tik
    /// <summary>Han yaşamı (her gün); 5 günde bir: kahraman gelişi, açık artırmalar, pano, sözleşme bitişleri, han baskınları, süresi dolan ilanlar.</summary>
    public static void InnsTick(Sim s)
    {
        var w = s.W;
        InnLife.InnLifeTick(s);   // hancılar, inşaat, misafirler, kiler, defter (her gün)
        if (s.Day % 5 != 0) return;
        var inns = w.Inns;
        for (int ii = 0; ii < inns.Count; ii++)
        {
            var inn = inns[ii];
            if (!inn.Alive) continue;
            var pool = InnPool(s, inn);
            // yılda ~0.5–1; ünlü hana kahraman daha çok uğrar (Faz 1: eskisinin yarısı)
            double p = (pool.Count == 0 ? 0.03 : pool.Count < INN_POOL ? 0.012 : 0) * (0.8 + (inn.Fame / 100) * 0.35) * Heroes.Demand(s, inn.Tile, null);
            if (s.Rng.Chance(p)) SpawnInnHero(s, inn);
            foreach (var h in pool)
            {
                if (h.Auction == null && h.State == "tavern" && h.Tavern == inn.Id) h.Auction = new Auction { End = s.Day + Sim.YEAR, Bids = new List<Bid>() };
                else if (h.Auction != null && s.Day >= h.Auction.End) CloseAuction(s, inn, h);
            }
            if (s.Day % 30 == 0) PostGuardQuest(s, inn);
        }
        var heroes = w.Heroes;
        for (int i = 0; i < heroes.Count; i++) { var h = heroes[i]; if (h.Contract != null && s.Day >= h.Contract.Until && h.State == "home") ContractEnd(s, h); }
        if (s.Day % 30 == 0) { var civs = w.Civs; for (int i = 0; i < civs.Count; i++) { var c = civs[i]; if (c.Alive) ConsiderInnRaid(s, c); } }
        // süresi dolan ilanlar ödülü iade eder
        var quests = w.Quests;
        for (int i = 0; i < quests.Count; i++)
        {
            var q = quests[i];
            if (q.Open && q.Expires != null && s.Day >= q.Expires.Value)
            {
                q.Open = false;
                if (q.Civ >= 0) s.Add(w.Civs[q.Civ], "gold", q.Bounty); else { var inn = Will.InnById(s, q.Inn); if (inn != null) InnLife.InnIncome(s, inn, q.Bounty, $"İlanın süresi doldu; {J.S(q.Bounty)} altın ödül kasaya döndü"); }
                s.Metric("questExpired");
            }
        }
    }

    // ------------------------------------------------------------ açık artırma
    /// <summary>medeniyet, hanlardaki açık artırmalara teklif verir</summary>
    public static void ConsiderInnBids(Sim s, Civ c)
    {
        if ((c.InnBanUntil ?? 0) > s.Day || InnHeroesOf(s, c).Count >= MAX_INN_HEROES) return;
        bool war = s.InWar(c); double gold = s.St(c, "gold");
        int mine = s.CivHeroes(c).Count;
        if (!(c.Threat > 0.25 || war || (mine == 0 && gold > 90) || gold > 260)) return;
        var cap = s.Capital(c);
        if (cap == null) return;
        int already = J.Filter(s.W.Heroes, h0 => h0.Civ == -1 && h0.State != "dead" && h0.State != "gone" && h0.Auction != null && J.Some(h0.Auction.Bids, b => b.Civ == c.Id)).Count;
        if (already >= MAX_INN_HEROES - InnHeroesOf(s, c).Count) return;
        string heroClass = D.CLASSES[c.Cls].HeroClass;
        var cands = J.Sort(J.Filter(J.Filter(s.W.Heroes, h0 => h0.Civ == -1 && h0.Auction != null && h0.BaseInn && (h0.State == "tavern" || h0.State == "quest" || h0.State == "traveling") && Will.HeroWillServe(c, h0) && !J.Some(h0.Auction.Bids, b => b.Civ == c.Id)),
                h0 => { var i = Will.InnById(s, h0.Base); return i != null && i.Alive && s.G.Dist(i.Tile, cap.Tile) <= 30; }),
            (a, b) => J.Or(J.Or(b.Level - a.Level, (b.Cls == heroClass ? 1 : 0) - (a.Cls == heroClass ? 1 : 0)), a.Id - b.Id));
        var h = J.At(cands, 0);
        if (h == null) return;
        double K = Heroes.HeroBaseCost(h);
        double need = (c.Threat > 0.35 || war ? 3 : 2) + (D.CLASSES[c.Cls].HeroClass == h.Cls ? 0.3 : 0);
        double max = JsMath.Min(gold * 0.6, K * need);
        // Math.max(0, ...bids.map(effBid))
        var tops = new List<double> { 0 };
        foreach (var b in h.Auction.Bids) tops.Add(EffBid(s, h, b));
        double top = JsMath.Max(tops.ToArray());
        double mult = (h.Rep.Get(c.Id) ?? 0) >= 3 ? 1.25 : 1;
        if (max < K * 1.5 || max * mult <= top) return;
        double bid = JsMath.Round(JsMath.Min(max, JsMath.Max(K * 1.5, top / mult * 1.1 + 5)));
        h.Auction.Bids.Add(new Bid { Civ = c.Id, Gold = bid });
        s.Metric("innBid");
    }

    private static double EffBid(Sim s, Hero h, Bid b) => b.Gold * ((h.Rep.Get(b.Civ) ?? 0) >= 3 ? 1.25 : 1);

    private static void CloseAuction(Sim s, Inn inn, Hero h)
    {
        var a = h.Auction;
        bool busy = J.Some(s.W.Agents, x => !J.T(x.Dead) && x.Kind == "party" && x.Heroes != null && x.Heroes.Contains(h.Id));
        if (busy) { a.End = s.Day + 20; return; }
        var ok = J.Sort(J.Filter(a.Bids, b0 =>
        {
            var c0 = s.W.Civs[b0.Civ];
            return c0.Alive && s.St(c0, "gold") >= b0.Gold && (c0.InnBanUntil ?? 0) <= s.Day && InnHeroesOf(s, c0).Count < MAX_INN_HEROES && Will.HeroWillServe(c0, h);
        }), (x, y) => J.Or(J.Or(EffBid(s, h, y) - EffBid(s, h, x), (s.W.Civs[y.Civ].Race == h.Race ? 1 : 0) - (s.W.Civs[x.Civ].Race == h.Race ? 1 : 0)), x.Civ - y.Civ));
        h.Auction = null;
        if (ok.Count == 0) return;
        var b = ok[0]; var c = s.W.Civs[b.Civ];
        s.Add(c, "gold", -b.Gold);
        InnLife.InnIncome(s, inn, JsMath.Round(b.Gold * 0.1), $"Açık artırma: {c.Name}, {h.Name} için {J.S(b.Gold)} altın verdi; hanın payı %10");
        foreach (var x in s.W.Agents) if (!J.T(x.Dead) && x.Kind == "hero" && x.Heroes != null && x.Heroes.Contains(h.Id)) x.Dead = true;
        h.Goal = null;
        h.Civ = c.Id;
        h.Contract = new HeroContract { Civ = c.Id, Since = s.Day, Until = s.Day + CONTRACT_YEARS * Sim.YEAR, Paid = b.Gold };
        h.Hired = (h.Hired ?? 0) + 1;
        Will.Note(s, h, $"{c.Name} ile {J.S(CONTRACT_YEARS)} yıllık sözleşme ({J.S(b.Gold)} altın)");
        Lore.Deed(s, h, "contract", $"{c.Name} ile {Lore.Num(CONTRACT_YEARS)} yıllık sözleşme imzaladı", h.Pos, c.Name);
        s.Metric("innHire"); s.Metric($"innHire_{J.S(c.Id)}");
        int rivals = a.Bids.Count - 1;
        s.Log("inn", $"{InnName(inn)}'nda açık artırma: {c.Name}, {s.HeroTitle(h)} için {J.S(b.Gold)} altınla {J.S(CONTRACT_YEARS)} yıllık sözleşme imzaladı.", civ: c.Id, tile: inn.Tile, major: true, cause: rivals > 0 ? $"{J.S(rivals)} rakip teklif geride kaldı" : "Tek teklif");
        var cap = s.Capital(c);
        if (cap != null)
        {
            int t = h.Pos; h.Tavern = -1;
            if (t == cap.Tile) h.State = "home";
            else
            {
                h.State = "traveling";
                var path = s.Path(t, cap.Tile);
                if (path != null) s.W.Agents.Add(new Agent { Id = s.Id(), Kind = "hero", Civ = c.Id, Path = path, Step = 0, Progress = 0, Speed = 0.9, Heroes = new List<int> { h.Id }, Purpose = "home" });
                else { h.Pos = cap.Tile; h.State = "home"; }
            }
        }
    }

    private static void ContractEnd(Sim s, Hero h)
    {
        var k = h.Contract; var c = s.W.Civs[k.Civ];
        double years = Math.Floor((s.Day - k.Since) / Sim.YEAR);
        if (Will.MaybeRetire(s, h)) return;
        double pay = JsMath.Max(0, JsMath.Min(3, Math.Floor(k.Paid / Heroes.HeroBaseCost(h) - 1)));
        double roll = s.Rng.Dice(1, 20);
        double total = roll + years + pay + JsMath.Min(2, JsMath.Max(0, h.Rep.Get(c.Id) ?? 0));
        if (total >= 12)
        {
            k.Until = s.Day + CONTRACT_YEARS * Sim.YEAR;
            Will.Note(s, h, $"{c.Name} ile sözleşmesini yeniledi");
            s.Log("hero", $"{h.Name}, {c.Name} ile sözleşmesini {J.S(CONTRACT_YEARS)} yıl uzattı.", civ: c.Id, tile: h.Pos, cause: $"Sadakat zarı {J.S(roll)} + {J.S(years + pay)} = {J.S(total)} (DC 12)");
            return;
        }
        h.Civ = -1; h.Contract = null;
        s.Metric("contractLeave");
        Will.Note(s, h, $"{c.Name} ile sözleşmesi bitti; hana döndü");
        s.Log("hero", $"{h.Name}, {c.Name} ile sözleşmesi bitince eşyasını toplayıp hana döndü.", civ: c.Id, tile: h.Pos, major: true, cause: $"Sadakat zarı {J.S(roll)} + {J.S(years + pay)} = {J.S(total)} (DC 12)");
        Will.ReturnToBase(s, h);
    }

    // ------------------------------------------------------------ hanın panosu
    private static void PostGuardQuest(Sim s, Inn inn)
    {
        var w = s.W;
        // zengin han, panosundaki medeniyet ilanlarının ödülüne katkı koyar (yolları güvenli olsun, yolcu gelsin)
        if (inn.Gold > 220)
        {
            var quests = w.Quests;
            for (int i = 0; i < quests.Count; i++)
            {
                var q = quests[i];
                if (!q.Open || q.Inn != inn.Id || q.Civ < 0 || J.T(q.Topped)) continue;
                double add = JsMath.Round(JsMath.Min(inn.Gold * 0.15, q.Bounty * 0.4));
                if (add < 5) continue;
                q.Bounty += add; q.Topped = add; inn.Gold -= add;
                InnLife.InnEvent(s, inn, $"Hancı {w.Civs[q.Civ].Name} ilanının ödülüne {J.S(add)} altın ekledi");
            }
        }
        if (inn.Gold < 40) return;
        var mine = J.Filter(w.Quests, q => q.Open && q.Civ == -1 && q.Inn == inn.Id);
        if (mine.Count >= Math.Max(1, inn.Level)) return;
        // ejderhanın ilanı ayrı (Dragon.Bounties: büyük ödül)
        var cp = J.At(J.Sort(J.Filter(w.Camps, c => c.Alive && c.Kind != "dragon" && !J.T(c.Hidden) && !J.T(w.Tiles[c.Tile].Isle) && s.G.Dist(c.Tile, inn.Tile) <= 12 && !J.Some(w.Quests, q => q.Open && q.Camp == c.Id && q.Civ == -1)),
            (a, b) => s.G.Dist(a.Tile, inn.Tile) - s.G.Dist(b.Tile, inn.Tile)), 0);
        if (cp == null) return;
        double @base = 30 + (cp.Kind == "hobgoblin" ? 25 : cp.Kind == "bugbear" || cp.Kind == "pirate" ? 35 : cp.Kind == "troll" ? 50 : 0);
        double bounty = JsMath.Round(JsMath.Min(inn.Gold * 0.35, @base * (1 + (Math.Max(1, inn.Level) - 1) * 0.4)));
        inn.Gold -= bounty;
        w.Quests.Add(new Quest { Id = s.Id(), Civ = -1, Camp = cp.Id, Bounty = bounty, Posted = s.Day, TakenBy = new List<int>(), Open = true, Inn = inn.Id, Expires = s.Day + 3 * Sim.YEAR });
        s.Metric("innQuest");
        InnLife.InnEvent(s, inn, $"Panoya ilan asıldı: {cp.Name} temizlensin ({J.S(bounty)} altın)");
        s.Log("quest", $"Hancı {inn.Keeper}, {InnName(inn)} panosuna ilan astı: \"{cp.Name} temizlensin, ödül {J.S(bounty)} altın.\"", tile: inn.Tile, major: true, cause: $"{cp.Name} hana {J.S(s.G.Dist(cp.Tile, inn.Tile))} fersah");
    }

    // ------------------------------------------------------------ misafir hakkı
    /// <summary>hana ya da handaki kahramana saldıran medeniyet herkesle bozuşur ve 5 yıl kahraman kiralayamaz</summary>
    public static void BreakGuestRight(Sim s, Civ c, Inn inn)
    {
        bool pact = IsPact(s, c);
        double pen = pact ? -15 : -30;
        foreach (var o in s.W.Civs)
        {
            if (!o.Alive || o.Id == c.Id) continue;
            s.SetMod(o.Id, c.Id, "innbreak", $"{c.Name} {Lore.Ek(InnName(inn), "i")} bastı: misafir hakkı çiğnendi", pen, 0.05);
            if (s.E(o, "crusade") > 0) s.SetMod(o.Id, c.Id, "innbreak_holy", "Han baskını paladinlerin öfkesini alevlendirdi", -15, 0.03, false);
        }
        c.InnBanUntil = s.Day + 5 * Sim.YEAR;
        s.Metric("innBreak");
        s.Log("inn", $"{c.Name} {Lore.Ek(InnName(inn), "i")} bastı; bütün medeniyetler onlara sırt çevirdi. Artık \"Han Bozan\" olarak anılıyorlar.", civ: c.Id, tile: inn.Tile, major: true, cause: $"Misafir hakkı çiğnendi: ilişkiler {J.S(pen)}, 5 yıl hanlardan kahraman kiralayamazlar");
    }

    private static List<Combatant> InnDefenders(Sim s, Inn inn, string side)
    {
        var d = J.Map(J.Filter(s.W.Heroes, h => h.Civ == -1 && h.State == "tavern" && h.Tavern == inn.Id), h => Will.HeroSide(h, side));
        for (int i = 0; i < 2; i++) d.Add(Combat.Unit(D.UNITS["militia"], side, "militia"));
        return d;
    }

    /// <summary>Paktçı hanı basar: altın ve kurban için</summary>
    private static void ConsiderInnRaid(Sim s, Civ c)
    {
        if (!IsPact(s, c) || s.Year < 4 || (c.InnBanUntil ?? 0) > s.Day || s.InWar(c)) return;
        if (J.Some(s.W.Agents, a => a.Civ == c.Id && a.Purpose == "innraid")) return;
        double sol = J.Reduce(s.CivSettlements(c), (a, x) => a + x.Soldiers, 0.0);
        if (sol < 6) return;
        var ss = s.CivSettlements(c);
        var inn = J.At(J.Sort(J.Filter(s.W.Inns, i => i.Alive && J.Some(ss, x => s.G.Dist(x.Tile, i.Tile) <= 14) && (i.Gold >= 40 || InnPool(s, i).Count > 0)),
            (a, b) => b.Gold - a.Gold), 0);
        if (inn == null || !s.Rng.Chance(1.0 / 40)) return;
        var cap = s.Capital(c);
        double n = JsMath.Min(10, Math.Floor(sol * 0.6));
        if (Combat.PowerOf(Agents.CivTroops(s, c, n, "A")) < Combat.PowerOf(InnDefenders(s, inn, "B")) * 1.2) return;
        var path = s.Path(cap.Tile, inn.Tile);
        if (path == null) return;
        var pop = Agents.DrawSoldiers(s, c, n);
        s.W.Agents.Add(new Agent { Id = s.Id(), Kind = "army", Civ = c.Id, Path = path, Step = 0, Progress = 0, Speed = 0.7, Troops = n, Pop = pop, From = cap.Id, To = inn.Id, Purpose = "innraid" });
        s.Log("war", $"{c.Name} karanlık bir sefer düzenledi: {J.S(n)} asker {Lore.Ek(InnName(inn), "a")} yürüyor.", civ: c.Id, tile: cap.Tile, major: true, cause: "Patronları altın ve kurban istiyor");
    }

    /// <summary>Paktçı ordusu hana vardı: misafirlerle savaş, yağma ya da püskürtülme, misafir hakkı çiğnenir.</summary>
    public static void InnRaidArrive(Sim s, Agent a)
    {
        var inn = Will.InnById(s, a.To);
        var c = s.W.Civs[a.Civ];
        if (inn == null || !inn.Alive) { Agents.ArmyReturn(s, a); return; }
        var side = Agents.CivTroops(s, c, a.Troops ?? 0, "A");
        var def = InnDefenders(s, inn, "B");
        var b = Combat.ResolveBattle(s.Rng, side, def, new BattleOpts { Id = s.Id(), Day = s.Day, Tile = inn.Tile, Title = $"{InnName(inn)} baskını", SideA = $"{c.Name} askerleri", SideB = $"{InnName(inn)} misafirleri", MoraleA = 0.55, MoraleB = 0.6, TimeoutWinner = "B", CivA = c.Id });
        Agents.RecordBattle(s, b);
        Agents.SyncHeroes(s, def, b.Winner == "B" ? 90 : 10, side, c.Name, "inn");
        double dead = J.Filter(side, x => x.Hp <= 0).Count;
        a.Troops = JsMath.Max(0, (a.Troops ?? 0) - dead);
        inn.Raids++;
        s.Metric("innRaid");
        if (b.Winner == "A")
        {
            double gold = Math.Floor(JsMath.Max(0, inn.Gold)); inn.Gold -= gold;
            s.Add(c, "gold", gold);
            InnLife.InnScatter(s, inn, $"{c.Name} askerleri hanı bastı", false);
            InnLife.InnEvent(s, inn, $"{c.Name} askerleri hanı yağmaladı: {J.S(gold)} altın gitti");
            s.Log("war", $"{c.Name} askerleri {Lore.Ek(InnName(inn), "i")} yağmaladı: {J.S(gold)} altın ve misafirlerin kanı.", civ: c.Id, tile: inn.Tile, battle: b.Id, major: true);
            var heroes = s.W.Heroes;
            for (int i = 0; i < heroes.Count; i++) { var h = heroes[i]; if (h.Civ == -1 && h.State == "tavern" && h.Tavern == inn.Id) { Will.Note(s, h, $"{c.Name} hanı bastı"); h.Grudge = c.Id; } }
        }
        else
        {
            s.Log("war", $"{InnName(inn)} misafirleri {c.Name} askerlerini kapıdan geri püskürttü.", civ: c.Id, tile: inn.Tile, battle: b.Id, major: true);
            inn.Fame = JsMath.Min(100, inn.Fame + 4);
            InnLife.InnEvent(s, inn, $"{c.Name} baskını kapıdan geri püskürtüldü");
        }
        BreakGuestRight(s, c, inn);
        Agents.ArmyReturn(s, a);
    }

    /// <summary>canavar baskını: hanı handaki serbest kahramanlar savunur</summary>
    public static void InnMonsterRaid(Sim s, Agent a)
    {
        var inn = Will.InnById(s, a.To);
        if (inn == null || !inn.Alive) { Agents.RaidReturn(s, a); return; }
        string kind = a.Monster ?? "goblin";
        var cp = J.Find(s.W.Camps, x => x.Id == a.From);
        var def = InnDefenders(s, inn, "A");
        var mons = Monsters.MonsterSide(kind, a.Troops ?? 0, J.T(a.Boss), "B");
        var b = Combat.ResolveBattle(s.Rng, def, mons, new BattleOpts { Id = s.Id(), Day = s.Day, Tile = inn.Tile, Title = $"{InnName(inn)} baskını", SideA = $"{InnName(inn)} misafirleri", SideB = Monsters.MonsterName(kind), MoraleA = 0.6, MoraleB = 0.5, TimeoutWinner = "A" });
        Agents.RecordBattle(s, b);
        Agents.SyncHeroes(s, def, b.Winner == "A" ? 80 : 10, mons, cp?.Name, "inn");
        a.Troops = J.Filter(mons, x => x.Kind == "monster" && x.Hp > 0).Count;
        a.Boss = J.Some(mons, x => J.T(x.Boss) && x.Hp > 0);
        inn.Raids++;
        s.Metric("innMonsterRaid");
        var heroes = J.Map(J.Filter(def, x => x.Hero != null), x => x.Hero);
        if (b.Winner == "B")
        {
            a.Loot = (a.Loot ?? 0) + Math.Floor(inn.Gold * 0.6);
            RuinInn(s, inn, $"{cp?.Name ?? Monsters.MonsterName(kind)} baskını", b.Id);
        }
        else
        {
            foreach (var h in heroes) if (h.State != "dead") Will.Note(s, h, $"{Lore.Ek(InnName(inn), "i")} {J.TrLower(Monsters.MonsterName(kind))} baskınına karşı savundu");
            inn.Fame = JsMath.Min(100, inn.Fame + 3);
            InnLife.InnEvent(s, inn, $"{Monsters.MonsterName(kind)} baskını püskürtüldü{(heroes.Count > 0 ? $" ({string.Join(", ", J.Map(heroes, h => h.Name))})" : "")}");
            s.Log("raid", $"{InnName(inn)} misafirleri {J.TrLower(Monsters.MonsterName(kind))} baskınını püskürttü{(heroes.Count > 0 ? $": {string.Join(", ", J.Map(heroes, h => h.Name))}" : "")}.", tile: inn.Tile, battle: b.Id, major: heroes.Count > 0, cause: $"{cp?.Name ?? "Kamp"} hana yakın");
        }
        Agents.RaidReturn(s, a);
    }

    /// <summary>Han yanar: misafirler dağılır, ilanlar kapanır, yuvası burası olan kahramanlar yeni yuvaya yönelir.</summary>
    public static void RuinInn(Sim s, Inn inn, string why, int? battle = null)
    {
        InnLife.InnEvent(s, inn, $"Han yandı: {why}");
        InnLife.InnScatter(s, inn, "han yanarken kaçtı", true);
        inn.Alive = false; inn.RuinedDay = s.Day; inn.Teacher = null;
        s.Metric("innRuined");
        s.Log("inn", $"{InnName(inn)} yandı ve harabeye döndü.", tile: inn.Tile, battle: battle, major: true, cause: why);
        var quests = s.W.Quests;
        for (int i = 0; i < quests.Count; i++) { var q = quests[i]; if (q.Open && q.Civ == -1 && q.Inn == inn.Id) q.Open = false; }
        var heroes = s.W.Heroes;
        for (int i = 0; i < heroes.Count; i++)
        {
            var h = heroes[i];
            if (h.State == "dead" || h.State == "gone") continue;
            if (h.State == "retired" && h.Base == inn.Id) { h.State = "gone"; continue; }
            if (!h.BaseInn || h.Base != inn.Id) continue;
            h.Auction = null;
            var r = Will.NearestRest(s, inn.Tile);
            if (r == null) { if (h.Civ == -1) h.State = "gone"; continue; }
            h.Base = r.Id; h.BaseInn = r.Inn;
            if (h.Civ == -1 && h.State == "tavern") Will.ReturnToBase(s, h);
        }
    }
}
