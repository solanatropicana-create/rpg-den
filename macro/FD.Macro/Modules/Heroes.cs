using System;
using System.Collections.Generic;
using System.Linq;

// Taverna, kahramanlar, görevler ve seferler. Port of src/sim/heroes.ts; Faz 1 A3a ile kendi yolunda:
// doğuş Sv1–3 (yıl şişirmez), Sv10'a dek D&D eşikleri, meydan okumaya göre XP, ünle gelen efsanelik,
// kalıcı ölüm ve destan (Lore.cs).

namespace FD.Macro;

public static class Heroes
{
    /// <summary>spawnHero: rastgele ırk havuzu (yerli nüfus seçilmezse)</summary>
    private static readonly List<string> ANY_RACE = new() { "human", "dwarf", "elf", "halfling", "gnome", "halfelf", "halforc", "dragonborn", "tiefling" };

    /// <summary>JS <c>s.charAt(0).toUpperCase() + s.slice(1)</c> (locale-independent upper case of the first UTF-16 unit).</summary>
    internal static string UpperFirst(string s) => string.IsNullOrEmpty(s) ? "" : s.Substring(0, 1).ToUpperInvariant() + s.Substring(1);

    /// <summary>tavernada en çok bu kadar serbest (yuvası taverna olan) kahraman bulunur</summary>
    public const int TAVERN_CAP = 2;
    /// <summary>tavernaya günlük yabancı gelme olasılığı (boşken / doluyken), iş talebiyle (Demand) çarpılır (Faz 1b-5: eski günde 1/300 ve 1/600; ×PACE)</summary>
    public const double TAVERN_SPAWN_EMPTY = Sim.PACE / 300, TAVERN_SPAWN = Sim.PACE / 600;
    /// <summary>Faz 1b-5: ilanın ömrü (gün; yol haritası v3: 10–20 gün; eskiden 3 eski yıl). Başarısız sefer ödülü %25 artırır.</summary>
    public const double QUEST_DAYS = 15;

    /// <summary>İş olan yere kahraman gelir: taban 0.2; 15 fersah içindeki her canlı kamp +D_CAMP, 20 fersah içindeki kampa
    /// asılmış her açık ilan +D_QUEST, savaş +0.5, medeniyetin tehdidi (en çok +1); tavan D_MAX.</summary>
    public static double Demand(Sim s, int tile, Civ c)
    {
        var w = s.W;
        double d = 0.2;
        foreach (var cp in w.Camps)
            if (cp.Alive && !J.T(cp.Hidden) && !J.T(w.Tiles[cp.Tile].Isle) && s.G.Dist(cp.Tile, tile) <= 15) d += D_CAMP;   // gizli in (B2) iş getirmez
        foreach (var q in w.Quests)
        {
            if (!q.Open) continue;
            var cp = J.Find(w.Camps, x => x.Id == q.Camp);
            if (cp != null && s.G.Dist(cp.Tile, tile) <= 20) d += D_QUEST;
        }
        if (c != null) { if (s.InWar(c)) d += 0.5; d += JsMath.Min(1, c.Threat); }
        return JsMath.Min(D_MAX, d);
    }
    /// <summary>iş talebi ağırlıkları. Faz 1 C1'de 0.25 / 0.25 / 2.0'a indirmek taverna doğumunu dünya başına ~147'den ~110'a
    /// düşürdü ama kahraman arzı savaş dengesine bağlı çıktı (savaş ilanı −%37, çöküş 153 → 95, ölçüt 2 13/16 dünya): olduğu gibi kaldı.</summary>
    public const double D_CAMP = 0.35, D_QUEST = 0.3, D_MAX = 2.5;
    /// <summary>doğuş seviyesi tavanı</summary>
    public const int BIRTH_CAP = 3;

    /// <summary>Tavernası olan her yerleşime ara sıra yabancı bir kahraman gelir; ayrıca savaş dışında ölüp (ör. suikast)
    /// destanı yazılmamış kahramanların destanı yazılır.</summary>
    public static void TavernsTick(Sim s)
    {
        var sts = s.W.Settlements;
        for (int i = 0; i < sts.Count; i++)
        {
            var st = sts[i];
            if (!st.Alive || !J.T(st.Civics.Get("tavern") ?? 0)) continue;
            int present = J.Filter(s.W.Heroes, h => h.Civ == -1 && !h.BaseInn && h.Base == st.Id && h.State != "dead" && h.State != "gone" && h.State != "retired").Count;
            if (present >= TAVERN_CAP) continue;
            if (s.Rng.Chance((present == 0 ? TAVERN_SPAWN_EMPTY : TAVERN_SPAWN) * Demand(s, st.Tile, s.W.Civs[st.Civ]))) SpawnHero(s, st);
        }
        var hs = s.W.Heroes;
        for (int i = 0; i < hs.Count; i++) if (hs[i].State == "dead" && !J.T(hs[i].Epitaph)) Lore.LateDeath(s, hs[i]);
    }

    /// <summary>Doğuş seviyesi: Sv1, %30 Sv2; +1 (medeniyetin heroLevel etkisi ya da hanın emekli öğretmeni); tavan Sv3.</summary>
    public static int BirthLevel(Sim s, bool bonus) => Math.Min(BIRTH_CAP, (s.Rng.Chance(0.3) ? 2 : 1) + (bonus ? 1 : 0));

    /// <summary>Bir yerleşimin tavernasına yeni bir yabancı kahraman gelir (ırk/sınıf/seviye zarı + kayıt).</summary>
    public static void SpawnHero(Sim s, Settlement st)
    {
        var civ = s.W.Civs[st.Civ];
        var locals = J.Filter(st.Pop.Keys(), r => (st.Pop.Get(r) ?? 0) > 0);
        string race = s.Rng.Chance(0.65) && locals.Count > 0 ? s.Rng.Weighted(locals, r => st.Pop.Get(r) ?? 0) : s.Rng.Pick(ANY_RACE);
        string affinity = Polity.HeroClass(civ);
        string cls = s.Rng.Weighted(D.HERO_CLASS_IDS, k => k == affinity ? 3 : 1);
        int level = BirthLevel(s, cls == affinity && s.E(civ, "heroLevel") > 0);
        var h = MakeHero(s, (race, cls, level, st.Tile, st.Id, false));
        s.Metric("heroSpawn");
        s.Log("hero", $"{Lore.Ek(st.Name, "in")} tavernasına bir yabancı geldi: {s.HeroTitle(h)}.", tile: st.Tile, civ: st.Civ, cause: UpperFirst(h.Bio));
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
        // ad: ırkın ad ve soyad havuzundan (ya da yuvasında doğmuş ünlü bir atadan); tam ad tekrarlanmaz
        var nm = Lore.RollName(s, race, o.Base);
        string name = $"{nm.Given} {nm.Surname}";
        var ages = D.HERO_AGE[race];
        double birthAge = s.Rng.Int(ages[0], ages[1]);
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
            Given = nm.Given, Surname = nm.Surname, Lineage = nm.Ancestor?.Id, BirthLevel = level, BirthAge = birthAge, Deeds = new List<HeroDeed>(),
        };
        Will.Note(s, h, $"{(o.BaseInn ? (Will.InnById(s, o.Base)?.Name ?? "") + " Hanı" : (s.Settlement(o.Base)?.Name ?? "") + " tavernası")}nda doğdu");
        if (nm.Ancestor != null) Will.Note(s, h, $"{(J.T(nm.Ancestor.Legend) ? "efsanevi " : "")}{Lore.Ek(nm.Ancestor.Name, "in")} soyundan");
        string placeLoc = o.BaseInn ? Lore.Ek($"{Will.InnById(s, o.Base)?.Name ?? "Yol"} Hanı", "da") : $"{Lore.Ek(s.Settlement(o.Base)?.Name ?? "Yurt", "in")} tavernasında";
        Lore.Born(s, h, placeLoc, nm.Ancestor);
        s.W.Heroes.Add(h);
        // Faz 1b-6: inanç (doğduğu yerin dağılımından; rahip, paladin, druid sınıfına göre) ve örgüt üyeliği (tarih öncesinden sonra)
        h.Faith = Orgs.HeroFaith(s, h, o.BaseInn ? s.Settlement(s.W.Tiles[o.Tile].Owner) : s.Settlement(o.Base));
        if (s.W.Orgs.Count > 0) Orgs.JoinFor(s, h, quiet: true);
        return h;
    }

    /// <summary>hanlardaki fiyat çıpası (K)</summary>
    public static double HeroBaseCost(Hero h) => 35 + 25 * (double)h.Level;

    private static readonly List<string> TWO_ATTACKS = new() { "fighter", "paladin", "barbarian", "ranger" };

    /// <summary>seviye tavanı (XP_LEVELS'teki eşik sayısı: Sv10)</summary>
    public static int MaxLevel => D.XP_LEVELS.Count;

    /// <summary>Deneyim ekler (tamsayıya yuvarlanır; <paramref name="src"/> verilirse xp_&lt;src&gt; sayacına yazılır) ve
    /// seviye atlamalarını uygular: can, Sv3 savaşçı / Sv7 herkes +1 zırh, gelişim notu; Sv5 ateş topu / iki saldırı,
    /// Sv9 sınıf gücü (Combat.HeroCombatant). Efsanelik seviyeden değil, ünden gelir (AddRenown).</summary>
    public static void GainXp(Sim s, Hero h, double xp, string src = null)
    {
        if (!(xp > 0) || h.State == "dead") return;
        xp = JsMath.Round(xp);
        h.Xp += xp;
        if (src != null) { s.Metric("xp_" + src, xp); s.Metric("xpn_" + src); }
        while (h.Level < MaxLevel && h.Xp >= D.XP_LEVELS[h.Level])
        {
            h.Level++;
            double inc = D.HERO_CLASSES[h.Cls].HitDie / 2 + 1 + Rng.Mod(J.N(h.Stats.Get("con")));
            h.MaxHp += inc; h.Hp += inc;
            if (h.Cls == "fighter" && h.Level == 3) h.Ac++;
            if (h.Level == 7) h.Ac++;
            Will.GrowFromExperience(s, h);
            string perk = h.Level == 5 ? (h.Cls == "wizard" ? " Artık Ateş Topu büyüsünü biliyor." : TWO_ATTACKS.Contains(h.Cls) ? " Artık turda iki kez saldırıyor." : "")
                : h.Level == 7 ? " Savaş tecrübesi zırhını sağlamlaştırdı (+1 zırh)."
                : h.Level == 9 ? " " + Combat.Sv9Power(h.Cls) : "";
            s.Metric("levelUp_" + J.S(h.Level));
            s.Log("hero", $"{h.Name} {J.S(h.Level)}. seviyeye yükseldi!{perk}", tile: h.Pos, civ: h.Civ >= 0 ? h.Civ : (int?)null, major: h.Level >= 5);
            if (h.Level >= 5) Lore.Deed(s, h, "level", $"{Lore.Ord(h.Level)} seviyeye yükseldi", h.Pos);
        }
    }

    // ------------------------------------------------------------ ün ve efsanelik
    /// <summary>bu kadar üne ulaşan (yaşayan) kahraman efsane olur (Faz 1 C1: 30 → 27, aşağıdaki kaynaklar küçültüldükten sonra;
    /// dünya başına efsane medyanı 13,5 → 3; Faz 1b-5: ün sönüp genç kahramanın ünü çabuk yayılınca yeniden 30). Ejderhayı deviren bu
    /// kadar ün alır (Dragon.R_SLAYER), ata olmak yarısını ister.</summary>
    public const double LEGEND_RENOWN = 30;
    /// <summary>Faz 1b-5 (v3: efsaneye yükseliş 100–300 gün): genç kahramanın ünü çabuk yayılır. Doğumundan sonraki işlerinin ünü
    /// 1 + RENOWN_YOUNG kattan başlar, RENOWN_YOUNG_DAYS günde doğrusal olarak 1 kata iner (yeni yüz dillere düşer; tanınmış kahramanın
    /// yeni işi eski ününe eklenir ama ün söner, bkz. RENOWN_HALF).</summary>
    public const double RENOWN_YOUNG = 1.5, RENOWN_YOUNG_DAYS = 400;

    /// <summary>genç kahramanın ün katsayısı (bkz. RENOWN_YOUNG)</summary>
    public static double YoungFame(Sim s, Hero h) => 1 + RENOWN_YOUNG * JsMath.Max(0, 1 - (s.Day - h.Born) / RENOWN_YOUNG_DAYS);
    /// <summary>ün kaynakları: sağ çıkılan zafer, kamp temizleme, kamp önderi öldürme, ilan bitirme, düello, doğal 20, yurt/han savunma.
    /// Faz 1 C1: dünya kanlandıkça (dünya başına ~100 kamp) kamp ve ilan ünü tek başına efsane yaratıyordu; artık kişisel
    /// işler (önder devirmek, yurt savunmak, düello, ejderha) ağır basar, tekrarlanan işin ünü azalır (Repeat).</summary>
    public const double R_WIN = 0.15, R_CAMP = 1.25, R_BOSS = 7, R_QUEST = 2, R_DUEL = 7, R_NAT20 = 1, R_DEFEND = 5;
    /// <summary>tekrarlanan işin ünü azalır: n'inci kamp, ilan ya da savunma 1 / (1 + R_REPEAT × (n − 1)) kat ün getirir</summary>
    public const double R_REPEAT = 0.25;

    /// <summary>n'inci tekrarın ün katsayısı (n = 1: tam ün).</summary>
    public static double Repeat(double n) => 1 / (1 + R_REPEAT * JsMath.Max(0, n - 1));

    /// <summary>Ünü yazar (rn_&lt;src&gt; kişisel sayaç, renown_&lt;src&gt; dünya sayacı); efsanelik denetimi yok.</summary>
    private static void Fame(Sim s, Hero h, double v, string src)
    {
        v *= YoungFame(s, h);
        h.Renown += v;
        h.Tally.Set("rn_" + src, (h.Tally.Get("rn_" + src) ?? 0) + v);
        s.Metric("renown_" + src, v);
    }

    /// <summary>Ün ekler (renown_&lt;src&gt; sayacı); yaşayan kahraman eşiği geçince efsane olur.</summary>
    public static void AddRenown(Sim s, Hero h, double v, string src)
    {
        if (!(v > 0)) return;
        Fame(s, h, v, src);
        if (!J.T(h.Legend) && h.Renown >= LEGEND_RENOWN && h.State != "dead") Will.BecomeLegend(s, h);
    }

    /// <summary>Sayacı bir artırır ve yeni değeri döndürür (kamp, ilan, savunma tekrarları için).</summary>
    public static double Count(Hero h, string key)
    {
        double n = (h.Tally.Get(key) ?? 0) + 1;
        h.Tally.Set(key, n);
        return n;
    }

    /// <summary>İlanı bitiren kahraman (FightCamp'ten): kilometre taşı ve ün (her yeni ilan biraz daha az).</summary>
    public static void QuestDone(Sim s, Hero h, Quest q, Camp cp)
    {
        if (h == null || h.State == "dead") return;
        string poster = q.Org != null ? Orgs.ById(s, q.Org.Value)?.Name ?? "Avcılar Locası" : q.Civ >= 0 ? s.W.Civs[q.Civ].Name : $"{Will.InnById(s, q.Inn)?.Name ?? "Yol"} Hanı";
        Lore.Deed(s, h, "quest", $"{Lore.Ek(poster, "in")} ilanını bitirip {J.S(q.Bounty)} altın ödül aldı", cp?.Tile, poster);
        AddRenown(s, h, R_QUEST * Repeat(Count(h, "quests")), "quest");
    }

    // ------------------------------------------------------------ savaş sonrası
    /// <summary>savaş XP'si = XP_MUL × yenilen düşmanların değeri × kahramanların kendi taraflarındaki güç payı, kahramanlar
    /// arasında eşit bölünür (D&amp;D gibi; sınıfın güç tahmini kimseyi kayırmaz). Kalabalık bir ordunun içindeki kahramanlar az,
    /// kendi partisiyle savaşanlar çok öğrenir. Faz 1 C1: 14 → 9 (kamp savaşları ~2,5 katına çıkınca Sv9–10 sıradanlaşıyordu).</summary>
    public const double XP_MUL = 9.0;
    /// <summary>payı bölüştürürken askerlerin, milislerin ağırlığı (XP almazlar; destanı kahramanlar taşır)</summary>
    public const double XP_TROOP_WEIGHT = 0.25;
    /// <summary>tek savaştan en çok o seviyenin XP aralığının bu kadarı (bir savaşla seviye atlanmaz); Sv5'ten sonra her seviyede
    /// XP_CAP_STEP azalır (Faz 1 C1: kanlı dünyada büyük inler hep tavana dayanıyor, Sv9–10 sıradanlaşıyordu)</summary>
    public const double XP_BATTLE_CAP = 0.6, XP_CAP_STEP = 0.1;
    /// <summary>kendi eliyle devirdiği düşmanın değerinden ek pay</summary>
    public const double XP_KILL = 0.5;
    /// <summary>yenilip sağ kalan kahramanın aldığı XP oranı</summary>
    public const double XP_LOSS = 0.5;
    /// <summary>yenilmeyen (ayakta kalan ya da kaçmayan) düşmanın değerinden sayılan pay</summary>
    public const double XP_FACED = 0.2;

    /// <summary>Tek savaştan alınabilecek en çok XP: bulunduğu seviyenin XP aralığının XP_BATTLE_CAP katı (Sv10'da son aralık),
    /// Sv5'in üstünde seviye başına XP_CAP_STEP eksik (Sv9: 0.2). Alt seviyelerde hemen her savaş bu tavana dayanır (seviye
    /// başına ~2 anlamlı savaş); üst seviyelerde XP'yi düşmanın büyüklüğü belirler ve en büyük savaş bile seviyenin ancak
    /// %20'sini getirir (büyük seviyeler çok savaş ister).</summary>
    public static double XpCap(Hero h)
    {
        var xl = D.XP_LEVELS;
        int L = Math.Max(1, Math.Min(h.Level, xl.Count - 1));
        return (XP_BATTLE_CAP - XP_CAP_STEP * Math.Max(0, L - 5)) * (xl[L] - xl[L - 1]);
    }

    /// <summary>Savaşanın XP değeri (D&amp;D'deki CR XP'sine yakın): 2 × tur başı hasar × etkin can; AC 15'e karşı, tam canla.
    /// Goblin ≈ 50, hobgoblin ≈ 80, bugbear ≈ 390, hobgoblin yüzbaşısı ≈ 980, Sv1 savaşçı ≈ 160, Sv5 savaşçı ≈ 1300.</summary>
    public static double XpValue(Combatant c)
    {
        double avg = c.Dmg[0] * (c.Dmg[1] + 1) / 2 + c.Dmg[2];
        double hit = JsMath.Min(0.95, JsMath.Max(0.1, (21 - (15 - c.Atk)) / 20));
        double extra = 1;
        string hc = c.Hero?.Cls;
        if (hc == "wizard") extra = 1.8;
        if (hc == "cleric" || hc == "druid") extra = 1.3;
        if (hc == "rogue" || hc == "paladin" || hc == "barbarian") extra = 1.4;
        double dpr = avg * hit * c.Attacks * extra;
        double ehp = JsMath.Max(1, c.MaxHp) * (1 + (c.Ac - 12) * 0.08) * (hc == "barbarian" ? 1.5 : 1);
        return JsMath.Max(1, 2 * dpr * ehp);
    }

    /// <summary>
    /// Savaş sonrası kahramanlar (Agents.SyncHeroes): can ve öldürmeler; ilk kan, ilk doğal 20, kamp önderi; ölüm (diriliş
    /// denenir; katil, kilometre taşı, destan); sağ kalanlara meydan okumaya göre XP ve ün.
    /// <paramref name="cs"/>: kahramanın tarafı (askerler dâhil); <paramref name="foes"/>: karşı taraf (null: eski düz ödül,
    /// öldürme × 50 + <paramref name="xpBonus"/>); <paramref name="foe"/>: karşı tarafın adı (kamp, yerleşim, medeniyet,
    /// kahraman); <paramref name="ctx"/>: camp | siege | defend | ambush | inn | rob | duel.
    /// </summary>
    public static void AfterBattle(Sim s, List<Combatant> cs, double xpBonus, List<Combatant> foes, string foe, string ctx)
    {
        var bs = s.W.Battles;
        var b = bs.Count > 0 && bs[bs.Count - 1].Day == s.Day ? bs[bs.Count - 1] : null;
        double sideVal = 0, heroVal = 0, weighted = 0; int nHero = 0;
        foreach (var x in cs) { double v = XpValue(x); sideVal += v; if (x.Hero != null) { heroVal += v; nHero++; weighted += v; } else weighted += v * XP_TROOP_WEIGHT; }
        double beaten = 0;
        if (foes != null) foreach (var f in foes) beaten += XpValue(f) * (f.Hp <= 0 || J.T(f.Fled) ? 1 : XP_FACED);
        double perHero = weighted > 0 && nHero > 0 ? XP_MUL * beaten * (heroVal / weighted) / nHero : 0;
        foreach (var x in cs)
        {
            if (x.Hero == null) continue;
            var h = x.Hero;
            bool won = b != null ? b.Winner == x.Side : xpBonus >= 50;
            bool firstBlood = h.Kills == 0 && x.Kills > 0;
            h.Hp = JsMath.Max(0, JsMath.Min(h.MaxHp, x.Hp));
            h.Kills += x.Kills;
            h.Tally.Set("battles", (h.Tally.Get("battles") ?? 0) + 1); h.Tally.Set("lastFight", s.Day);
            s.Metric("hb_" + (ctx ?? "other"));
            if (won) h.Tally.Set("wins", (h.Tally.Get("wins") ?? 0) + 1);
            // kilometre taşları ve kişisel ün (ölse de sayılır; efsanelik ancak sağ kalana)
            bool fame = false;
            if (firstBlood && b != null) Lore.Deed(s, h, "firstblood", $"ilk kanını {Lore.TitleLoc(b.Title)} döktü", b.Tile);
            if (b != null && J.Some(b.Rolls, r => r.D20 == 20 && r.Who == h.Name))
            {
                if (!Lore.HasDeed(h, "nat20")) Lore.Deed(s, h, "nat20", $"ilk doğal 20'sini {Lore.TitleLoc(b.Title)} attı", b.Tile);
                Fame(s, h, R_NAT20, "nat20"); fame = true;
            }
            double killVal = 0;
            if (foes != null)
                foreach (var f in foes)
                {
                    if (f.KilledBy != x) continue;
                    killVal += XpValue(f);
                    if (!J.T(f.Boss)) continue;
                    var cp = J.T(foe) ? J.Find(s.W.Camps, c => c.Name == foe) : null;
                    bool dragon = f.Breath != null;   // Faz 1 B2: ejderhayı devirmek ayrı bir kilometre taşı (destanda öne çıkar)
                    string who = dragon ? $"ejderha {Lore.Ek(f.Name, "i")}"
                        : cp != null && cp.Kind == "pirate" && J.T(cp.Captain) ? $"{Lore.Ek(foe, "in")} kaptanı {Lore.Ek(cp.Captain, "i")}"
                        : J.T(foe) ? $"{Lore.Ek(foe, "in")} {Lore.Ek0(J.TrLower(f.Name), "i")}" : $"bir {Lore.Ek0(J.TrLower(f.Name), "i")}";
                    Lore.Deed(s, h, dragon ? "dragon" : "boss", $"{who} kendi eliyle devirdi", b?.Tile, dragon ? f.Name : foe);
                    Fame(s, h, R_BOSS, "boss"); fame = true;
                    s.Metric("bossSlain");
                }
            if (x.Hp <= 0)
            {
                Die(s, h, x.KilledBy, foe, ctx, b);
                continue;
            }
            if (fame && !J.T(h.Legend) && h.Renown >= LEGEND_RENOWN) Will.BecomeLegend(s, h);
            double xp = foes != null ? JsMath.Min(XpCap(h), perHero + XP_KILL * killVal) * (won ? 1 : XP_LOSS) : x.Kills * 50 + xpBonus;
            GainXp(s, h, xp, ctx ?? "battle");
            if (!won) continue;
            AddRenown(s, h, R_WIN * JsMath.Min(1, 2 * heroVal / JsMath.Max(1, sideVal)), "victory");   // kalabalık ordunun içindeki kahramana az
            if ((ctx == "defend" || ctx == "inn") && b != null)
            {
                var st = J.Find(s.W.Settlements, z => z.Alive && z.Tile == b.Tile);
                var inn = st == null ? Will.InnAt(s, b.Tile) : null;
                string place = st?.Name ?? (inn != null ? Inns.InnName(inn) : null);
                if (place != null) Lore.Deed(s, h, "defend", $"{Lore.Ek(place, "i")} {(J.T(foe) ? Lore.Ek(foe, "a") : "düşmana")} karşı savundu", b.Tile, place);
                AddRenown(s, h, R_DEFEND * Repeat(Count(h, "defends")), "defend");   // her yeni savunma biraz daha az
            }
        }
    }

    /// <summary>Kahraman ölür (kalıcı): durum, yer, katil, "death" olayı, kilometre taşı ve destan.</summary>
    public static void Die(Sim s, Hero h, Combatant killer, string foe, string ctx, Battle b)
    {
        h.State = "dead"; h.DeathDay = s.Day; h.Auction = null; h.Goal = null;
        if (h.Contract != null) h.Contract = null;
        if (b != null) h.Pos = b.Tile;
        h.DeathTile = h.Pos;
        h.Killer = Lore.KillerOf(s, killer, foe);
        s.Metric("heroDeath");
        s.Metric("heroDeath_" + (ctx ?? "other"));
        s.Log("death", $"{s.HeroTitle(h)} öldü.", tile: h.Pos, civ: h.Civ >= 0 ? h.Civ : (int?)null, major: true, cause: $"Katili: {h.Killer}; {J.S(h.Kills)} düşman devirmişti");
        Lore.Deed(s, h, "death", Lore.DeathClause(s, h, b, foe), h.Pos, h.Killer);
        Lore.WriteEpitaph(s, h);
    }

    /// <summary>Tavernadaki kahramanın bir medeniyete kiralanma bedeli (sınıf yakınlığında %25 indirim).</summary>
    public static (double Gold, double Food) HeroCost(Sim s, Civ c, Hero h)
    {
        double aff = Polity.HeroClass(c) == h.Cls ? 0.75 : 1;
        return (JsMath.Round((35 + 25 * (double)h.Level) * aff), JsMath.Round((30 + 10 * (double)h.Level) * aff));
    }

    private static readonly string[] FOOD_TAKE = { "grain", "meat", "fish", "bread" };

    /// <summary>medeniyetin kiralık kahraman tavanı: 1 + nüfus / HIRE_POP, en çok HIRE_MAX</summary>
    public const double HIRE_POP = 60, HIRE_MAX = 5;

    /// <summary>Medeniyet YZ: tehdit, savaş ya da dolu hazine varsa tavernadan kahraman kiralar.</summary>
    public static void ConsiderHero(Sim s, Civ c)
    {
        double pop = s.CivPop(c);
        var mine = s.CivHeroes(c);
        // Faz 1: medeniyet kahraman ordusu değil, birkaç şampiyon tutar (eskiden 1 + nüfus/35, sınırsız)
        if (mine.Count >= JsMath.Min(HIRE_MAX, 1 + Math.Floor(pop / HIRE_POP))) return;
        bool war = s.InWar(c);
        bool want = c.Threat > 0.25 || war || (mine.Count == 0 && s.St(c, "gold") > 80) || (mine.Count < 2 && s.St(c, "gold") > 200);
        if (!want) return;
        var avail = J.Filter(J.Filter(s.W.Heroes, h0 => h0.Civ == -1 && h0.State == "tavern" && !h0.BaseInn && Will.HeroWillServe(c, h0)), h0 =>
        {
            var t = s.Settlement(h0.Tavern);
            return t != null && (t.Civ == c.Id || (s.Rel(c.Id, t.Civ).Contact && s.RelValue(c.Id, t.Civ) >= 0 && !s.AtWar(c.Id, t.Civ)));
        });
        double reserve = pop * Sim.FOOD_PER_POP * 25 / Sim.PACE;
        var ok = J.Filter(avail, h0 => { var k0 = HeroCost(s, c, h0); return s.St(c, "gold") >= k0.Gold && s.FoodTotal(c) - k0.Food >= reserve; });
        if (ok.Count == 0) return;
        string aff = Polity.HeroClass(c);
        var h = J.Sort(ok, (a, b) => b.Level - a.Level + (b.Cls == aff ? 1 : 0) - (a.Cls == aff ? 1 : 0))[0];
        var k = HeroCost(s, c, h);
        s.Add(c, "gold", -k.Gold);
        double f = k.Food;
        foreach (var g in FOOD_TAKE) { double take = JsMath.Min(f, s.St(c, g)); s.Add(c, g, -take); f -= take; }
        h.Civ = c.Id;
        h.Hired = (h.Hired ?? 0) + 1;
        h.Goal = null; h.Auction = null;
        Will.Note(s, h, $"{c.Name} saflarına katıldı");
        Lore.Deed(s, h, "contract", $"{c.Name} saflarına katıldı", h.Pos, c.Name);
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
        s.W.Agents.Add(new Agent { Id = s.Id(), Kind = "hero", Civ = h.Civ, Path = path, Step = 0, Progress = 0, Speed = Pace.HERO, Heroes = new List<int> { h.Id }, Purpose = then });
    }

    /// <summary>kampın gücü; vsAc: saldıranların ortalama zırhı</summary>
    public static double CampPower(Sim s, Camp cp, double vsAc = 15) => Combat.PowerOf(CampForce(s, cp), vsAc);

    /// <summary>Kampın canavar tarafı ('B'), baskına çıkmış olanlar dâhil (ejderha adıyla ve kalıcı yaralarıyla: Faz 1 B2).</summary>
    public static List<Combatant> CampForce(Sim s, Camp cp) => Monsters.ForceSide(s, cp);

    /// <summary>Bir medeniyeti en çok tehdit eden kamp (null = yok)</summary>
    public static Camp ThreatCamp(Sim s, Civ c)
    {
        var ss = s.CivSettlements(c);
        Camp best = null; double bd = 1e9;
        foreach (var cp in s.W.Camps)
        {
            if (!cp.Alive || J.T(cp.Hidden) || (cp.Kind == "pirate" && J.T(s.W.Tiles[cp.Tile].Isle))) continue; // ada korsanı: korsan avı (sea.ts); gizli in (B2) bilinmez
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
        bool hunt = s.E(c, "favoredHunt") > 0 || Polity.HeroClass(c) == "ranger";
        var occ = J.At(J.Sort(J.Filter(s.W.Camps, x => x.Alive && !J.T(x.Hidden) && !J.T(s.W.Tiles[x.Tile].Isle) && s.W.Tiles[x.Tile].Deposit >= 0 && J.Some(ss, st => s.G.Dist(st.Tile, x.Tile) <= 9)),
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
            var probe = new Agent { Id = -1, Kind = "army", Civ = c.Id, Path = path0 ?? new List<int> { cap.Tile }, Step = 0, Progress = 0, Speed = Pace.EXPEDITION, Heroes = J.Map(home, h => h.Id), To = cp.Id, Purpose = "expedition" };
            var allies = path0 != null ? Agents.ProbeAllies(s, probe, Agents.EtaDays(s, probe), Agents.MUSTER_CAMP) : new List<Agent>();
            var allyCs = new List<Combatant>();
            foreach (var b in allies) allyCs.AddRange(Agents.AgentCombatants(s, b, cp.Kind));
            var sideAll = new List<Combatant>(side);
            sideAll.AddRange(allyCs);
            var (pa, pb) = Combat.PowerVs(sideAll, CampForce(s, cp));
            // Faz 1 B2: güç tahmini ejderhanın nefesini ve dehşetini hafife alır; ona karşı ordu çok daha güçlü olmalı
            if ((home.Count > 0 || soldiers >= 6 || (allies.Count > 0 && soldiers >= 3)) && pa >= pb * (hunt ? 0.8 : 0.95) * (cp.Kind == "dragon" ? Dragon.SOLO_FEAR : 1))
            {
                var pop = Agents.DrawSoldiers(s, c, soldiers);
                var path = path0;
                if (path != null)
                {
                    foreach (var h in home) h.State = "army";
                    s.W.Agents.Add(new Agent { Id = s.Id(), Kind = "army", Civ = c.Id, Path = path, Step = 0, Progress = 0, Speed = Pace.EXPEDITION, Heroes = J.Map(home, h => h.Id), Troops = soldiers, Pop = pop, From = cap.Id, To = cp.Id, Purpose = "expedition" });
                    s.Log("quest", $"{c.Name}{(home.Count > 0 ? $", {string.Join(" ve ", J.Map(home, h => h.Name))} önderliğinde" : "")} {J.S(soldiers)} askerle {Lore.Ek(cp.Name, "a")} sefer başlattı.", civ: c.Id, tile: cap.Tile, cause: allies.Count > 0 ? $"{motive}; yoldaki {J.S(allies.Count)} dost grupla birlikte saldıracak" : motive, major: true);
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
        double kindF = cp.Kind == "hobgoblin" ? 1.5 : cp.Kind == "bugbear" || cp.Kind == "pirate" ? 2 : cp.Kind == "troll" ? 2.5 : cp.Kind == "dragon" ? 5 : 1;
        double bounty = JsMath.Round(JsMath.Min(s.St(c, "gold") * 0.6, (30 + c.Threat * 50) * kindF + (ReferenceEquals(cp, occ) ? 15 : 0)));
        s.Add(c, "gold", -bounty);
        var inn = J.At(J.Sort(J.Filter(s.W.Inns, i => i.Alive), (a, b) => s.G.Dist(a.Tile, cp.Tile) - s.G.Dist(b.Tile, cp.Tile)), 0);
        var q = new Quest { Id = s.Id(), Civ = c.Id, Camp = cp.Id, Bounty = bounty, Posted = s.Day, TakenBy = new List<int>(), Open = true, Inn = inn != null && s.G.Dist(inn.Tile, cp.Tile) <= 22 ? inn.Id : null, Expires = s.Day + QUEST_DAYS };
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
        if (q.Org != null) { var og = Orgs.ById(s, q.Org.Value); if (og != null) { double pay = JsMath.Min(add, Math.Floor(og.Gold)); og.Gold -= pay; q.Bounty += pay; } }
        else if (q.Civ >= 0) { var c = s.W.Civs[q.Civ]; double pay = JsMath.Min(add, Math.Floor(s.St(c, "gold"))); s.Add(c, "gold", -pay); q.Bounty += pay; }
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

    // ------------------------------------------------------------ yaş
    /// <summary>Kahramanın bugünkü yaşı (yıl).</summary>
    public static double Age(Sim s, Hero h) => h.BirthAge + (s.Day - h.Born) / Sim.YEAR;

    /// <summary>Yaşlılıkta yıllık ölüm olasılığı: ırkın yaşlılık eşiğinden sonra %3 + %30 × ((yaş − eşik) / (ömür − eşik))²,
    /// en uzun ömrü aşınca %50.</summary>
    public static double OldAgeDeathChance(Sim s, Hero h)
    {
        var a = D.HERO_AGE[h.Race];
        double age = Age(s, h), old = a[2], max = a[3];
        if (age < old) return 0;
        if (age >= max) return 0.5;
        double x = (age - old) / (max - old);
        return 0.03 + 0.3 * x * x;
    }

    /// <summary>Kahraman yaşlılıktan ölür (dinlenirken): kalıcı ölüm, kilometre taşı ve destan.</summary>
    public static void DieOfAge(Sim s, Hero h)
    {
        double age = Math.Floor(Age(s, h));
        h.State = "dead"; h.DeathDay = s.Day; h.Auction = null; h.Goal = null;
        if (h.Contract != null) h.Contract = null;
        foreach (var a in s.W.Agents) if (a.Heroes != null && a.Heroes.Contains(h.Id) && a.Kind == "hero") a.Dead = true;
        h.DeathTile = h.Pos;
        h.Killer = "yaşlılık";
        s.Metric("heroDeath");
        s.Metric("heroDeath_age");
        s.Log("death", $"{s.HeroTitle(h)} ileri yaşında öldü.", tile: h.Pos, civ: h.Civ >= 0 ? h.Civ : (int?)null, major: true, cause: $"Yaşlılık: {J.S(age)} yaşındaydı; {J.S(h.Kills)} düşman devirmişti");
        Lore.Deed(s, h, "death", $"{Lore.PlaceLoc(s, h.Pos) ?? "yurdunda"}, {J.S(age)} yaşında huzur içinde gözlerini yumdu", h.Pos, "yaşlılık");
        Lore.WriteEpitaph(s, h);
    }

    /// <summary>eski iki yıldır iş bulamayan serbest kahramanın diyarı terk etmek yerine başka bir yuvaya göçme olasılığı</summary>
    public const double MOVE_NOT_LEAVE = 0.75;
    /// <summary>Faz 1b-5: dinlenen kahraman günde en büyük canının bu payını kapatır (eskiden 5 eski günde %10: tam iyileşme ~12 gün)</summary>
    public const double REST_HEAL = 0.1 / (5 / Sim.PACE);
    /// <summary>işsiz serbest kahramanın göç zarı: eski iki yıl (60 gün) işsizlikten sonra, günde bu olasılıkla (eskiden 5 eski günde 0,1)</summary>
    public const double IDLE_DAYS = 2 * Sim.OLD_YEAR, IDLE_MOVE = 0.1 / (5 / Sim.PACE);

    /// <summary>Faz 1b-5 (yol haritası v3: kahraman doğumu han başına 10–20 günde bir): handa doğan yabancı yolcudur. Hanına INN_STAY
    /// günden uzun süredir yerleşik, teklifsiz serbest kahraman günde INN_MOVE olasılıkla yoluna devam eder: tecrübeli (Sv3+) yeri olan en
    /// yakın tavernalardan birine göçer, acemi INN_LEAVE olasılıkla diyardan çıkar, yoksa o da bir tavernaya göçer. Han havuzu dönüşür.</summary>
    public const double INN_STAY = 30, INN_MOVE = 0.1, INN_LEAVE = 0.3;
    /// <summary>göçen yabancıyı kabul eden tavernada en çok bu kadar serbest kahraman olabilir (yeni doğuşun tavanı TAVERN_CAP)</summary>
    public const int TAVERN_ROOM = 4;
    /// <summary>Faz 1b-5 (v3: efsaneye yükseliş 100–300 gün): ün söner; efsane olmamış yaşayan kahramanın ünü RENOWN_HALF günde yarıya
    /// iner. Efsane, ünü kısa sürede biriktiren (önder deviren, yurt savunan, düello kazanan) kahramandan çıkar, uzun ömürden değil.</summary>
    public const double RENOWN_HALF = 90;
    public static readonly double RENOWN_KEEP = JsMath.Pow(0.5, 1 / RENOWN_HALF);

    /// <summary>Her gün (Faz 1b-5; eskiden 5 eski günde bir): dinlenenler iyileşir, ün söner, irade tiki, handaki yabancılar yoluna devam eder,
    /// işsiz serbest kahramanlar emekli olur, göçer ya da diyarı terk eder. Yaşlılık ve emeklilik zarı takvim yılında bir.</summary>
    public static void HeroesTick(Sim s)
    {
        var w = s.W;
        var heroes = w.Heroes;
        for (int i = 0; i < heroes.Count; i++)
        {
            var h = heroes[i];
            if (h.State == "dead" || h.State == "gone" || h.State == "retired") continue;
            if (h.State == "tavern" || h.State == "home") { PotionHeal(s, h); h.Hp = JsMath.Min(h.MaxHp, h.Hp + Math.Ceiling(h.MaxHp * REST_HEAL)); }
            if (h.Renown > 0 && !J.T(h.Legend)) h.Renown *= RENOWN_KEEP;
        }
        Will.WillTick(s);
        heroes = w.Heroes;
        for (int i = 0; i < heroes.Count; i++)
        {
            var h = heroes[i];
            if ((s.Day + h.Id * 5) % Sim.YEAR != 0) continue;   // her kahramanın yılda bir günü
            // yaşlılık: dinlenen (tavernada, yurtta, emeklilikte) kahraman yaşlanıp ölebilir
            if ((h.State == "tavern" || h.State == "home" || h.State == "retired") && s.Rng.Chance(OldAgeDeathChance(s, h))) { DieOfAge(s, h); continue; }
            // yurtta oturan sözleşmesiz kiralık kahraman da yılları bulunca kılıcını asabilir
            if (h.Civ >= 0 && h.State == "home" && h.Contract == null) Will.MaybeRetire(s, h);
        }
        for (int i = 0; i < heroes.Count; i++)
        {
            var h = heroes[i];
            if (h.Civ != -1 || h.State != "tavern") continue;
            // yaşlı ya da yılları bulmuş serbest kahraman emekli olabilir
            if ((s.Day + h.Id * 5) % Sim.YEAR == 0 && Will.MaybeRetire(s, h)) continue;
            bool bids = h.Auction != null && h.Auction.Bids.Count > 0;
            if (h.BaseInn && h.Tavern == h.Base && !bids && s.Day - (h.BaseDay ?? h.Born) > INN_STAY && s.Rng.Chance(INN_MOVE)) { MoveOn(s, h); continue; }
            // uzun süre iş bulamayan başka yere göçer ya da diyarı terk eder
            double idle = s.Day - JsMath.Max(h.IdleSince, h.LastGoal ?? 0);
            if (idle > IDLE_DAYS && !bids && s.Rng.Chance(IDLE_MOVE))
            {
                var others = J.Filter(w.Settlements, x => x.Alive && J.T(x.Civics.Get("tavern") ?? 0) && x.Id != h.Base);
                var inns = J.Filter(w.Inns, x => x.Alive && x.Id != h.Base);
                // tecrübeli (Sv3+) kahraman diyarı terk etmez, iş aramak için göçer; acemiler çoğunlukla göçer, bazen gider
                if ((others.Count > 0 || inns.Count > 0) && (h.Level >= 3 || s.Rng.Chance(MOVE_NOT_LEAVE)))
                {
                    bool pickInn = inns.Count > 0 && (others.Count == 0 || s.Rng.Chance(0.5));
                    int tId; string tName;
                    if (pickInn) { var ti = s.Rng.Pick(inns); tId = ti.Id; tName = ti.Name; }
                    else { var ts = s.Rng.Pick(others); tId = ts.Id; tName = ts.Name; }
                    h.IdleSince = s.Day; h.Base = tId; h.BaseInn = pickInn; h.BaseDay = s.Day; h.Auction = null;
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
    /// <summary>Handaki yabancı yoluna devam eder (bkz. INN_STAY): tecrübeli (Sv3+) ya da INN_LEAVE zarını geçen acemi, yeri olan en yakın
    /// üç tavernadan birine göçer; öteki (ya da yer bulamayan) diyardan çıkar.</summary>
    private static void MoveOn(Sim s, Hero h)
    {
        var inn = Will.InnById(s, h.Base);
        string innName = inn != null ? Inns.InnName(inn) : "han";
        // yalnız yeri olan (TAVERN_ROOM'dan az serbest kahramanı olan) tavernalar: dolu diyarda yabancı yoluna devam eder
        var towns = J.Filter(s.W.Settlements, x => x.Alive && J.T(x.Civics.Get("tavern") ?? 0)
            && J.Filter(s.W.Heroes, o => o.Civ == -1 && !o.BaseInn && o.Base == x.Id && o.State != "dead" && o.State != "gone" && o.State != "retired").Count < TAVERN_ROOM);
        h.Auction = null;
        if (towns.Count > 0 && (h.Level >= 3 || !s.Rng.Chance(INN_LEAVE)))
        {
            J.Sort(towns, (a, b) => J.Or(s.G.Dist(a.Tile, h.Pos) - s.G.Dist(b.Tile, h.Pos), a.Id - b.Id));
            var ts = s.Rng.Pick(towns.GetRange(0, Math.Min(3, towns.Count)));
            h.IdleSince = s.Day; h.Base = ts.Id; h.BaseInn = false; h.BaseDay = s.Day;
            s.Metric("innMoveOn");
            s.Log("hero", $"{h.Name}, {Lore.Ek(innName, "da")} iş bulamayınca {Lore.Ek(ts.Name, "in")} tavernasına doğru yola çıktı.", tile: h.Pos);
            Will.ReturnToBase(s, h);
            return;
        }
        h.State = "gone";
        s.Metric("innPassOn");
        s.Log("hero", $"{h.Name} {Lore.Ek(innName, "dan")} ayrılıp yoluna devam etti; bu diyara bir daha uğramadı.", tile: h.Pos);
    }
}
