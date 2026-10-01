using System;
using System.Collections.Generic;
using System.Linq;

// Taverna, kahramanlar, görevler ve seferler. Port of src/sim/heroes.ts.

namespace FD.Macro;

public static class Heroes
{
    /// <summary>spawnHero: rastgele ırk havuzu (yerli nüfus seçilmezse)</summary>
    private static readonly List<string> ANY_RACE = new() { "human", "dwarf", "elf", "halfling", "gnome", "halfelf", "halforc", "dragonborn", "tiefling" };

    /// <summary>JS <c>s.charAt(0).toUpperCase() + s.slice(1)</c> (locale-independent upper case of the first UTF-16 unit).</summary>
    internal static string UpperFirst(string s) => string.IsNullOrEmpty(s) ? "" : s.Substring(0, 1).ToUpperInvariant() + s.Substring(1);

    /// <summary>Tavernası olan her yerleşime ara sıra yabancı bir kahraman gelir (tavernada en çok 3 serbest kahraman).</summary>
    public static void TavernsTick(Sim s)
    {
        var sts = s.W.Settlements;
        for (int i = 0; i < sts.Count; i++)
        {
            var st = sts[i];
            if (!st.Alive || !J.T(st.Civics.Get("tavern") ?? 0)) continue;
            int present = J.Filter(s.W.Heroes, h => h.Civ == -1 && !h.BaseInn && h.Base == st.Id && h.State != "dead" && h.State != "gone" && h.State != "retired").Count;
            if (present >= 3) continue;
            if (s.Rng.Chance(present == 0 ? 1.0 / 70 : 1.0 / 130)) SpawnHero(s, st);
        }
    }

    /// <summary>Bir yerleşimin tavernasına yeni bir yabancı kahraman gelir (ırk/sınıf/seviye zarı + kayıt).</summary>
    public static void SpawnHero(Sim s, Settlement st)
    {
        var civ = s.W.Civs[st.Civ];
        var locals = J.Filter(st.Pop.Keys(), r => (st.Pop.Get(r) ?? 0) > 0);
        string race = s.Rng.Chance(0.65) && locals.Count > 0 ? s.Rng.Weighted(locals, r => st.Pop.Get(r) ?? 0) : s.Rng.Pick(ANY_RACE);
        string affinity = D.CLASSES[civ.Cls].HeroClass;
        string cls = s.Rng.Weighted(D.HERO_CLASS_IDS, k => k == affinity ? 3 : 1);
        int level = s.Rng.Chance(0.6) ? 1 : s.Rng.Chance(0.75) ? 2 : 3;
        level = J.I(JsMath.Min(5, level + Math.Floor(s.Year / 6.0) + (cls == affinity ? s.E(civ, "heroLevel") : 0)));
        var h = MakeHero(s, (race, cls, level, st.Tile, st.Id, false));
        s.Metric("heroSpawn");
        s.Log("hero", $"{Tr.Ek(st.Name, "in")} tavernasına bir yabancı geldi: {s.HeroTitle(h)}.", tile: st.Tile, civ: st.Civ, cause: UpperFirst(h.Bio));
    }

    /// <summary>Yeni kahraman üretir (4d6 statlar, can, irade), doğum notunu düşer ve dünyaya ekler. TS <c>o</c> = <c>{ race, cls, level, tile, base, baseInn }</c>.</summary>
    public static Hero MakeHero(Sim s, (string Race, string Cls, int Level, int Tile, int Base, bool BaseInn) o)
    {
        string race = o.Race, cls = o.Cls;
        int level = o.Level;
        var rolls = new List<double>();
        foreach (var _ in D.STATS)
        {
            var d = new List<double>();
            for (int k = 0; k < 4; k++) d.Add(s.Rng.Dice(1, 6));
            J.Sort(d, (a, b) => a - b);
            rolls.Add(d[1] + d[2] + d[3]);
        }
        J.Sort(rolls, (a, b) => b - a);
        var stats = new JsObj<double>();
        var pr = D.HERO_CLASSES[cls].Priority;
        for (int i = 0; i < pr.Count; i++) stats.Set(pr[i], rolls[i]);
        var rb = D.RACE_STAT[race];
        foreach (var kv in rb) stats.Set(kv.Key, J.N(stats.Get(kv.Key)) + kv.Value);
        var def = D.HERO_CLASSES[cls];
        double con = Rng.Mod(J.N(stats.Get("con"))), dex = Rng.Mod(J.N(stats.Get("dex")));
        double maxHp = def.HitDie + con + (level - 1) * (def.HitDie / 2 + 1 + con);
        // `${pick(HERO_NAMES[race])} ${chance(0.5) ? pick(EPITHETS) : ''}`.trim(): left to right
        string first = s.Rng.Pick(D.HERO_NAMES[race]);
        string epi = s.Rng.Chance(0.5) ? s.Rng.Pick(D.EPITHETS) : "";
        string name = $"{first} {epi}".Trim();
        var will = Will.RollWill(s, cls);
        // object literal: id first, then gold, then bio (origin, drive)
        int id = s.Id();
        double gold = s.Rng.Int(3, 12);
        string origin = s.Rng.Pick(D.HERO_ORIGINS);
        string drive = s.Rng.Pick(D.HERO_DRIVES);
        var h = new Hero
        {
            Id = id, Name = name, Race = race, Cls = cls, Level = level, Xp = D.XP_LEVELS[level - 1], Stats = stats, MaxHp = maxHp, Hp = maxHp, Ac = Combat.HeroAc(cls, dex, level), Civ = -1, Pos = o.Tile, Tavern = o.Base,
            State = "tavern", Born = s.Day, IdleSince = s.Day, Kills = 0, Gold = gold, Bio = $"{origin}, {drive}.",
            Align = will.Align, Path = will.Path, Traits = new List<string>(), Tally = new JsObj<double>(), Bonus = new HeroBonus { Atk = 0 }, Rep = new JsNumObj<double>(), Journal = new List<JournalEntry>(), Base = o.Base, BaseInn = o.BaseInn, Birth = o.Base,
        };
        Will.Note(s, h, $"{(o.BaseInn ? (Will.InnById(s, o.Base)?.Name ?? "") + " Hanı" : (s.Settlement(o.Base)?.Name ?? "") + " tavernası")}nda doğdu");
        s.W.Heroes.Add(h);
        return h;
    }

    /// <summary>hanlardaki fiyat çıpası (K)</summary>
    public static double HeroBaseCost(Hero h) => 35 + 25 * (double)h.Level;

    private static readonly List<string> TWO_ATTACKS = new() { "fighter", "paladin", "barbarian", "ranger" };

    /// <summary>Deneyim ekler; seviye atlamaları (can, zırh, gelişim, efsane) uygular.</summary>
    public static void GainXp(Sim s, Hero h, double xp)
    {
        h.Xp += xp;
        while (h.Level < 5 && h.Xp >= D.XP_LEVELS[h.Level])
        {
            h.Level++;
            double inc = D.HERO_CLASSES[h.Cls].HitDie / 2 + 1 + Rng.Mod(J.N(h.Stats.Get("con")));
            h.MaxHp += inc; h.Hp += inc;
            if (h.Cls == "fighter" && h.Level == 3) h.Ac++;
            Will.GrowFromExperience(s, h);
            s.Log("hero", $"{h.Name} {J.S(h.Level)}. seviyeye yükseldi!{(h.Cls == "wizard" && h.Level == 5 ? " Artık Ateş Topu büyüsünü biliyor." : h.Level == 5 && TWO_ATTACKS.Contains(h.Cls) ? " Artık turda iki kez saldırıyor." : "")}", tile: h.Pos, civ: h.Civ >= 0 ? h.Civ : (int?)null, major: h.Level >= 4);
            if (h.Level == 5) Will.BecomeLegend(s, h);
        }
    }

    /// <summary>Tavernadaki kahramanın bir medeniyete kiralanma bedeli (sınıf yakınlığında %25 indirim).</summary>
    public static (double Gold, double Food) HeroCost(Sim s, Civ c, Hero h)
    {
        double aff = D.CLASSES[c.Cls].HeroClass == h.Cls ? 0.75 : 1;
        return (JsMath.Round((35 + 25 * (double)h.Level) * aff), JsMath.Round((30 + 10 * (double)h.Level) * aff));
    }

    private static readonly string[] FOOD_TAKE = { "grain", "meat", "fish", "bread" };

    /// <summary>Medeniyet YZ: tehdit, savaş ya da dolu hazine varsa tavernadan kahraman kiralar.</summary>
    public static void ConsiderHero(Sim s, Civ c)
    {
        double pop = s.CivPop(c);
        var mine = s.CivHeroes(c);
        if (mine.Count >= 1 + Math.Floor(pop / 35)) return;
        bool war = s.InWar(c);
        bool want = c.Threat > 0.25 || war || (mine.Count == 0 && s.St(c, "gold") > 80) || s.St(c, "gold") > 200;
        if (!want) return;
        var avail = J.Filter(J.Filter(s.W.Heroes, h0 => h0.Civ == -1 && h0.State == "tavern" && !h0.BaseInn && Will.HeroWillServe(c, h0)), h0 =>
        {
            var t = s.Settlement(h0.Tavern);
            return t != null && (t.Civ == c.Id || (s.Rel(c.Id, t.Civ).Contact && s.RelValue(c.Id, t.Civ) >= 0 && !s.AtWar(c.Id, t.Civ)));
        });
        double reserve = pop * Sim.FOOD_PER_POP * 25;
        var ok = J.Filter(avail, h0 => { var k0 = HeroCost(s, c, h0); return s.St(c, "gold") >= k0.Gold && s.FoodTotal(c) - k0.Food >= reserve; });
        if (ok.Count == 0) return;
        string aff = D.CLASSES[c.Cls].HeroClass;
        var h = J.Sort(ok, (a, b) => b.Level - a.Level + (b.Cls == aff ? 1 : 0) - (a.Cls == aff ? 1 : 0))[0];
        var k = HeroCost(s, c, h);
        s.Add(c, "gold", -k.Gold);
        double f = k.Food;
        foreach (var g in FOOD_TAKE) { double take = JsMath.Min(f, s.St(c, g)); s.Add(c, g, -take); f -= take; }
        h.Civ = c.Id;
        h.Hired = (h.Hired ?? 0) + 1;
        h.Goal = null; h.Auction = null;
        Will.Note(s, h, $"{c.Name} saflarına katıldı");
        var cap = s.Capital(c);
        string why = war ? "savaşta güçlü bir kol gerekiyordu" : c.Threat > 0.25 ? "canavar tehdidine karşı" : "hazine doluydu, şan isteniyordu";
        s.Log("hero", $"{c.Name}, {s.HeroTitle(h)} adlı kahramanı {J.S(k.Gold)} altın ve {J.S(k.Food)} gıda karşılığında saflarına kattı.", civ: c.Id, tile: h.Pos, cause: UpperFirst(why), major: true);
        s.Metric("heroBought");
        SendHero(s, h, cap.Tile, "home");
    }

    /// <summary>Kahramanı bir karoya yollar (kahraman ajanı); zaten oradaysa ya da yol yoksa durumu doğrudan <paramref name="then"/> yapar.</summary>
    public static void SendHero(Sim s, Hero h, int tile, string then)
    {
        if (h.Pos == tile) { h.State = then; return; }
        var path = s.Path(h.Pos, tile);
        if (path == null) { h.Pos = tile; h.State = then; return; }
        h.State = "traveling"; h.Tavern = -1;
        s.W.Agents.Add(new Agent { Id = s.Id(), Kind = "hero", Civ = h.Civ, Path = path, Step = 0, Progress = 0, Speed = 0.9, Heroes = new List<int> { h.Id }, Purpose = then });
    }

    /// <summary>kampın gücü; vsAc: saldıranların ortalama zırhı</summary>
    public static double CampPower(Sim s, Camp cp, double vsAc = 15) => Combat.PowerOf(CampForce(s, cp), vsAc);

    /// <summary>Kampın canavar tarafı ('B'), baskına çıkmış olanlar dâhil.</summary>
    public static List<Combatant> CampForce(Sim s, Camp cp) => Monsters.MonsterSide(cp.Kind, cp.Count + Monsters.CampAway(s, cp), cp.Boss, "B");

    /// <summary>Bir medeniyeti en çok tehdit eden kamp (null = yok)</summary>
    public static Camp ThreatCamp(Sim s, Civ c)
    {
        var ss = s.CivSettlements(c);
        Camp best = null; double bd = 1e9;
        foreach (var cp in s.W.Camps)
        {
            if (!cp.Alive || (cp.Kind == "pirate" && J.T(s.W.Tiles[cp.Tile].Isle))) continue; // ada korsanı: korsan avı (sea.ts)
            double d = J.MinOf(ss, x => s.G.Dist(x.Tile, cp.Tile));
            if (d < bd && d <= 18) { bd = d; best = cp; }
        }
        return best;
    }

    /// <summary>Medeniyet YZ: tehdit eden / yatak işgal eden kampa sefer düzenler ya da ilan asar.</summary>
    public static void ConsiderQuest(Sim s, Civ c)
    {
        // hedef: tehdit eden kamp ya da topraklarındaki bir yatağı işgal eden kamp
        var ss = s.CivSettlements(c);
        bool hunt = s.E(c, "favoredHunt") > 0 || c.Cls == "ranger";
        var occ = J.At(J.Sort(J.Filter(s.W.Camps, x => x.Alive && !J.T(s.W.Tiles[x.Tile].Isle) && s.W.Tiles[x.Tile].Deposit >= 0 && J.Some(ss, st => s.G.Dist(st.Tile, x.Tile) <= 9)),
            (x, y) => J.MinOf(ss, st => s.G.Dist(st.Tile, x.Tile)) - J.MinOf(ss, st => s.G.Dist(st.Tile, y.Tile))), 0);
        var tc = ThreatCamp(s, c);
        var near = tc != null && J.Some(ss, st => s.G.Dist(st.Tile, tc.Tile) <= 11) ? tc : null;
        var cp = c.Threat >= 0.35 ? tc : occ ?? (hunt ? tc : near);
        if (cp == null) return;
        string motive = c.Threat >= 0.35 ? "Canavar baskınları dayanılmaz hâle geldi" : ReferenceEquals(cp, occ) ? $"{cp.Name} topraklarındaki bir yatağı işgal ediyor" : ReferenceEquals(cp, near) && !hunt ? $"{cp.Name} sınıra fazla yakın" : "Korucular avlanacak canavar arıyor";
        var home = J.Filter(s.CivHeroes(c), h => h.State == "home" && h.Hp > h.MaxHp * 0.7);
        var cap = s.Capital(c);
        if (!J.Some(s.W.Agents, a => a.Civ == c.Id && a.Purpose == "expedition"))
        {
            double soldiers = Math.Floor(J.Reduce(s.CivSettlements(c), (a, x) => a + x.Soldiers, 0.0) * 0.6);
            var side = J.Map(home, h => Combat.HeroCombatant(h, "A"));
            side.AddRange(Agents.CivTroops(s, c, soldiers, "A"));
            var path0 = s.Path(cap.Tile, cp.Tile);
            // aynı kampa yaklaşık aynı anda varacak dost gruplar (ilanı alan parti, av partisi) hesaba katılır
            var probe = new Agent { Id = -1, Kind = "army", Civ = c.Id, Path = path0 ?? new List<int> { cap.Tile }, Step = 0, Progress = 0, Speed = 0.6, Heroes = J.Map(home, h => h.Id), To = cp.Id, Purpose = "expedition" };
            var allies = path0 != null ? Agents.ProbeAllies(s, probe, Agents.EtaDays(s, probe), Agents.MUSTER_CAMP) : new List<Agent>();
            var allyCs = new List<Combatant>();
            foreach (var b in allies) allyCs.AddRange(Agents.AgentCombatants(s, b, cp.Kind));
            var sideAll = new List<Combatant>(side);
            sideAll.AddRange(allyCs);
            var (pa, pb) = Combat.PowerVs(sideAll, CampForce(s, cp));
            if ((home.Count > 0 || soldiers >= 6 || (allies.Count > 0 && soldiers >= 3)) && pa >= pb * (hunt ? 0.8 : 0.95))
            {
                var pop = Agents.DrawSoldiers(s, c, soldiers);
                var path = path0;
                if (path != null)
                {
                    foreach (var h in home) h.State = "army";
                    s.W.Agents.Add(new Agent { Id = s.Id(), Kind = "army", Civ = c.Id, Path = path, Step = 0, Progress = 0, Speed = 0.6, Heroes = J.Map(home, h => h.Id), Troops = soldiers, Pop = pop, From = cap.Id, To = cp.Id, Purpose = "expedition" });
                    s.Log("quest", $"{c.Name}{(home.Count > 0 ? $", {string.Join(" ve ", J.Map(home, h => h.Name))} önderliğinde" : "")} {J.S(soldiers)} askerle {Tr.Ek(cp.Name, "a")} sefer başlattı.", civ: c.Id, tile: cap.Tile, cause: allies.Count > 0 ? $"{motive}; yoldaki {J.S(allies.Count)} dost grupla birlikte saldıracak" : motive, major: true);
                    if (allies.Count > 0) s.Metric("jointPlanned");
                    return;
                }
                s.MergePop(cap, pop);
            }
        }
        var open = J.Find(s.W.Quests, q => q.Open && q.Civ == c.Id && q.Camp == cp.Id);
        if (open != null || J.Some(s.W.Quests, q => !q.Open && q.Camp == cp.Id && q.Civ == c.Id && q.TakenBy.Count > 0 && J.Some(s.W.Agents, a => a.Quest == q.Id && !J.T(a.Dead)))) return;
        if (s.St(c, "gold") < 25) return;
        if (c.Threat < 0.35 && !ReferenceEquals(cp, occ) && (!ReferenceEquals(cp, near) || s.St(c, "gold") < 60)) return;
        double kindF = cp.Kind == "hobgoblin" ? 1.5 : cp.Kind == "bugbear" || cp.Kind == "pirate" ? 2 : 1;
        double bounty = JsMath.Round(JsMath.Min(s.St(c, "gold") * 0.6, (30 + c.Threat * 50) * kindF + (ReferenceEquals(cp, occ) ? 15 : 0)));
        s.Add(c, "gold", -bounty);
        var inn = J.At(J.Sort(J.Filter(s.W.Inns, i => i.Alive), (a, b) => s.G.Dist(a.Tile, cp.Tile) - s.G.Dist(b.Tile, cp.Tile)), 0);
        var q = new Quest { Id = s.Id(), Civ = c.Id, Camp = cp.Id, Bounty = bounty, Posted = s.Day, TakenBy = new List<int>(), Open = true, Inn = inn != null && s.G.Dist(inn.Tile, cp.Tile) <= 22 ? inn.Id : null, Expires = s.Day + 3 * Sim.YEAR };
        s.W.Quests.Add(q);
        s.Metric("questPosted");
        string where = q.Inn != null ? $"{Will.InnById(s, q.Inn).Name} Hanı'nın panosuna" : "tavernalara";
        s.Log("quest", $"{c.Name} {where} ilan astı: \"{cp.Name} temizlensin, ödül {J.S(bounty)} altın.\"", civ: c.Id, tile: cap.Tile, cause: motive, major: true);
    }

    /// <summary>başarısız sefer: ilan açık kalır, ödül %25 artar</summary>
    public static void QuestFailed(Sim s, Quest q)
    {
        q.Open = true; q.TakenBy = new List<int>();
        q.Failures = (q.Failures ?? 0) + 1;
        double add = JsMath.Round(q.Bounty * 0.25);
        if (q.Civ >= 0) { var c = s.W.Civs[q.Civ]; double pay = JsMath.Min(add, Math.Floor(s.St(c, "gold"))); s.Add(c, "gold", -pay); q.Bounty += pay; }
        else { var i = Will.InnById(s, q.Inn); if (i != null) { double pay = JsMath.Min(add, Math.Floor(i.Gold)); i.Gold -= pay; q.Bounty += pay; } }
    }

    /// <summary>ağır yaralı kahraman bulunduğu kasabadan iksir alır: kendi medeniyeti verir, serbest kahraman altınla satın alır</summary>
    private static void PotionHeal(Sim s, Hero h)
    {
        if (h.Hp >= h.MaxHp * 0.6) return;
        var t = J.At(s.W.Tiles, h.Pos);
        var st = t != null && t.Owner >= 0 ? s.Settlement(t.Owner) : null;
        var c = st != null ? s.W.Civs[st.Civ] : null;
        if (c == null || !c.Alive || s.St(c, "potion") < 1) return;
        if (h.Civ != c.Id)
        {
            double price = Math.Ceiling(s.Price(c, "potion"));
            if (h.Gold < price) return;
            h.Gold -= price; s.Add(c, "gold", price);
        }
        s.Add(c, "potion", -1);
        h.Hp = h.MaxHp;
        s.Metric("potionHero");
    }

    /// <summary>5 günde bir: dinlenenler iyileşir, irade tiki, işsiz serbest kahramanlar emekli olur, göçer ya da diyarı terk eder.</summary>
    public static void HeroesTick(Sim s)
    {
        var w = s.W;
        var heroes = w.Heroes;
        for (int i = 0; i < heroes.Count; i++)
        {
            var h = heroes[i];
            if (h.State == "dead" || h.State == "gone" || h.State == "retired") continue;
            if (h.State == "tavern" || h.State == "home") { PotionHeal(s, h); h.Hp = JsMath.Min(h.MaxHp, h.Hp + Math.Ceiling(h.MaxHp * 0.1)); }
        }
        Will.WillTick(s);
        heroes = w.Heroes;
        for (int i = 0; i < heroes.Count; i++)
        {
            var h = heroes[i];
            if (h.Civ != -1 || h.State != "tavern") continue;
            // yaşlı ve ünlü serbest kahraman emekli olabilir
            if ((s.Day + h.Id * 5) % Sim.YEAR == 0 && Will.MaybeRetire(s, h)) continue;
            // uzun süre iş bulamayan başka yere göçer ya da diyarı terk eder
            double idle = s.Day - JsMath.Max(h.IdleSince, h.LastGoal ?? 0);
            if (idle > 2 * Sim.YEAR && !(h.Auction != null && h.Auction.Bids.Count > 0) && s.Rng.Chance(0.1))
            {
                var others = J.Filter(w.Settlements, x => x.Alive && J.T(x.Civics.Get("tavern") ?? 0) && x.Id != h.Base);
                var inns = J.Filter(w.Inns, x => x.Alive && x.Id != h.Base);
                if ((others.Count > 0 || inns.Count > 0) && s.Rng.Chance(0.5))
                {
                    bool pickInn = inns.Count > 0 && (others.Count == 0 || s.Rng.Chance(0.5));
                    int tId; string tName;
                    if (pickInn) { var ti = s.Rng.Pick(inns); tId = ti.Id; tName = ti.Name; }
                    else { var ts = s.Rng.Pick(others); tId = ts.Id; tName = ts.Name; }
                    h.IdleSince = s.Day; h.Base = tId; h.BaseInn = pickInn; h.Auction = null;
                    s.Log("hero", $"{h.Name}, iş bulamayınca {(pickInn ? $"{tName} Hanı'na" : $"{tName} tavernasına")} doğru yola çıktı.", tile: h.Pos);
                    Will.ReturnToBase(s, h);
                }
                else
                {
                    h.State = "gone";
                    s.Log("hero", $"{h.Name} kimse onu tutmayınca uzak diyarlara gitti.", tile: h.Pos);
                }
            }
        }
    }

    /// <summary>Ölen bağlı kahraman için Diriliş</summary>
    public static bool TryRevive(Sim s, Hero h)
    {
        if (h.Civ < 0 || J.T(h.Revived)) return false;
        var c = s.W.Civs[h.Civ];
        if (!J.T(s.E(c, "revive")) || c.Yearly.Get("revive") == s.Year) return false;
        c.Yearly.Set("revive", s.Year);
        h.Revived = true;
        h.Hp = Math.Ceiling(h.MaxHp / 2);
        h.State = "home";
        var cap = s.Capital(c);
        if (cap != null) h.Pos = cap.Tile;
        s.Log("class", $"{h.Name} Yaşam Alanı rahiplerinin duasıyla dirildi!", civ: c.Id, major: true);
        return true;
    }
}
