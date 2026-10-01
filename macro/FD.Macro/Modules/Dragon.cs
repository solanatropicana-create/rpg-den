using System;
using System.Collections.Generic;
using System.Linq;

// Faz 1 B2: ejderha. Dünyada bir kez, 18–22. yıllar arasında, yerleşimlerden uzak bir dağ eteğindeki inine uyanır.
// Kampı sıradan bir kamp gibi durur (Kind "dragon", Count 1, önder): kahramanlar, ilanlar ve seferler onu mevcut düzenekle
// hedefler (FightCamp, Engage, Will.ChooseGoal). Burada: uyanış, akınlar (tarla ve ev yakar, halkı öldürür, hazineden altın
// kaçırır), yıllık haraç isteği, hanlara ve medeniyetlere ödül ilanı, ejderhaya karşı ittifak (orduları inde aynı anda
// buluşur), ölüm (son darbeyi vuran efsane olur, hazine paylaşılır). Kalıcı yaraları DragonState.Hp'de durur.

namespace FD.Macro;

public static class Dragon
{
    /// <summary>akın ve haraç menzili (fersah; ejderha uçar)</summary>
    public const double RANGE = 26;
    /// <summary>hanların ejderha ilanı asacağı en uzak mesafe</summary>
    public const double INN_RANGE = 30;
    /// <summary>inde ve akında alınan yaralar günde en büyük canının bu kadarı kapanır</summary>
    public const double HEAL = 0.005;
    /// <summary>ejderha hazinesiyle büyür (yaşlanıp semirir): uyandıktan sonra kaçırdığı (haraç ve akın) her GROW_GOLD altın +1 can
    /// (en çok +GROW_HP); her BREATH_GOLD altın nefesine +1 zar (en çok +BREATH_MAX). Faz 1 C3: haraç tavanlandı, hazineler küçüldü
    /// (dünya başına ~150 bin → ~15 bin altın); büyüme eğrisi korunsun diye 20 → 1,5 ve 4000 → 750 (uykudan kalan hazine sayılmaz).</summary>
    public const double GROW_GOLD = 1.5, GROW_HP = 3000, BREATH_GOLD = 750, BREATH_MAX = 6;
    /// <summary>haraç: hazinenin payı; ejderha her yıl biraz daha açgözlü olur (yılda +%2, en çok %30)</summary>
    public const double TRIBUTE = 0.12;
    /// <summary>Faz 1 C3: haracın tavanı: TRIBUTE_CAP + nüfus × TRIBUTE_POP altın</summary>
    public const double TRIBUTE_CAP = 100, TRIBUTE_POP = 0.6;
    /// <summary>Faz 1 C3: haracı reddeden medeniyetin ejderha ilanı: en az BOUNTY_GOLD altını varsa hazinenin BOUNTY_SHARE payı (en az 100,
    /// en çok BOUNTY_MAX). Eskiden 1500 / %6 / 2500: küçülen hazinelere göre ölçeklendi.</summary>
    public const double BOUNTY_GOLD = 500, BOUNTY_SHARE = 0.15, BOUNTY_MAX = 1000;
    /// <summary>ün: son darbeyi vuran kahraman (efsane eşiğinin tamamı: ne olursa olsun efsane olur) ve savaşta sağ kalan diğer kahramanlar</summary>
    public const double R_SLAYER = Heroes.LEGEND_RENOWN, R_PARTY = 6;
    /// <summary>akında ejderha canının bu payını yitirirse yarım işle inine çekilir (püskürtülür). Ejderha kolay altın arar: üç turda
    /// canının %5'ini alan bir savunma onu caydırır (akınların ~%5'i; tipik akında %1'ini bile yitirmez).</summary>
    public const double DRIVEN = 0.05;
    /// <summary>ittifak ordularının inde birbirini bekleyeceği en uzun süre (gün; sıradan kampta 16)</summary>
    public const double MUSTER = 40;
    /// <summary>kahraman grubu ejderhaya karşı bu kat güç ister (ilan ve av)</summary>
    public const double HERO_NEED = 1.4;
    /// <summary>ejderhanın dehşeti: karşısındakilerin moral eşiği bu kadar düşer (daha çabuk bozguna uğrarlar)</summary>
    public const double FEAR = 0.2;
    /// <summary>ittifak, ejderhanın dehşetiyle büyütülmüş gücüne (× ALLY_FEAR: güç tahmini nefesi ve bozgunu hafife alır) karşı en
    /// az bu kazanma şansını görürse yola çıkar</summary>
    public const double ALLY_WIN = 0.8, ALLY_FEAR = 1.6;
    /// <summary>tek bir medeniyetin ejderhaya kendi seferi (Heroes.ConsiderQuest): ordusu ejderhanın gücünün 0,95 × bu katına
    /// ulaşmalı (ittifakla aynı eşik: dehşet payıyla ~%80 kazanma şansı)</summary>
    public const double SOLO_FEAR = 1.83;

    private static readonly List<string> NAMES = new() { "Alazkanat", "Közpençe", "Yalımdiş", "Külsavuran", "Karakor", "Odboğaz", "Kızılyele", "Közgöz" };

    /// <summary>Ejderhanın kampı (öldüyse de döner); null: henüz uyanmadı.</summary>
    public static Camp CampOf(Sim s)
    {
        var d = s.W.Dragon;
        if (d == null || d.Camp < 0) return null;
        return J.Find(s.W.Camps, c => c.Id == d.Camp);
    }

    /// <summary>Ejderha uyanık ve yaşıyor mu.</summary>
    public static bool Alive(Sim s)
    {
        var cp = CampOf(s);
        return cp != null && cp.Alive;
    }

    /// <summary>"ejderha Alazkanat" (cümle içinde; katil adı ve karşı tarafın adı olarak)</summary>
    public static string Label(Sim s) => $"ejderha {s.W.Dragon?.Name ?? "?"}";

    /// <summary>Ejderhanın savaşçısı: adıyla, kalıcı yaralarıyla ve hazinesiyle büyümüş canıyla (DragonState.Hp/MaxHp), hazineyle
    /// güçlenen nefesiyle, önder bayrağıyla.</summary>
    public static List<Combatant> Side(Sim s, Camp cp, string side)
    {
        var u = Combat.Unit(D.MONSTERS["dragon"], side, "monster");
        u.Evil = true; u.Boss = true;
        var d = s.W.Dragon;
        if (d != null && d.Camp == cp.Id)
        {
            u.Name = d.Name; u.MaxHp = d.MaxHp; u.Hp = JsMath.Max(1, JsMath.Min(d.MaxHp, d.Hp));
            u.Breath = (u.Breath ?? 0) + JsMath.Min(BREATH_MAX, Math.Floor(Taken(d, cp) / BREATH_GOLD));
        }
        return new List<Combatant> { u };
    }

    /// <summary>Faz 1 C3: ejderhanın uyandıktan sonra kaçırdığı altın (hazinesi − uykudan kalan hazine).</summary>
    private static double Taken(DragonState d, Camp cp) => JsMath.Max(0, cp.Loot - d.Loot0);

    /// <summary>Hazine büyüdükçe ejderha da büyür: en büyük canı (uyandığı yaşın canı + her GROW_GOLD altın için 1, en çok
    /// GROW_HP) ve büyüme kadar canı artar; hiç küçülmez.</summary>
    private static void Grow(DragonState d, Camp cp)
    {
        double max = d.BaseHp + JsMath.Min(GROW_HP, Math.Floor(Taken(d, cp) / GROW_GOLD));
        if (max > d.MaxHp) { d.Hp += max - d.MaxHp; d.MaxHp = max; }
    }

    /// <summary>ejderhanın uyandığı yaş (can) çağın krallıklarına göre: menzilindeki en güçlü iki medeniyetin ordusu birlikte onu
    /// (dehşet payıyla) ancak bu olasılıkla yenebilir; en az MONSTERS.dragon.hp, en çok WAKE_MAX</summary>
    public const double WAKE_ODDS = 0.12, WAKE_MAX = 3000;

    /// <summary>
    /// Uyanan ejderhanın canı: büyük krallıkların gürültüsü daha yaşlı bir ejderhayı uyandırır. Menzildeki en güçlü medeniyetin
    /// bütün askerleri ile ikinci en güçlünün seferi (askerlerinin %65'i; ikisinin de yurttaki kahramanları) birlikte, ejderhanın
    /// dehşetiyle büyütülmüş gücüne (×ALLY_FEAR) karşı en çok %12 kazanma şansı görecek kadar can (ikiye bölmeyle; zar atılmaz).
    /// Tek bir dev ordu ya da iki komşu onu uyanır uyanmaz deviremez; ittifak büyümeli ya da ejderha yaralanmalı.
    /// </summary>
    private static double WakeHp(Sim s, int lair)
    {
        double floor = D.MONSTERS["dragon"].Hp;
        var civs = J.Filter(s.W.Civs, c => c.Alive && J.Some(s.CivSettlements(c), x => s.G.Dist(x.Tile, lair) <= RANGE));
        if (civs.Count == 0) return floor;
        J.Sort(civs, (a, b) => J.Or(Combat.PowerOf(Force(s, b, 1), 20) - Combat.PowerOf(Force(s, a, 1), 20), a.Id - b.Id));
        var side = Force(s, civs[0], 1);
        if (civs.Count > 1) side.AddRange(Force(s, civs[1]));
        if (side.Count == 0) return floor;
        double lo = floor, hi = WAKE_MAX;
        var u = Combat.Unit(D.MONSTERS["dragon"], "B", "monster");
        var one = new List<Combatant> { u };
        u.Hp = u.MaxHp = hi;
        { var (pa, pb) = Combat.PowerVs(side, one); if (Combat.WinChance(pa, pb * ALLY_FEAR) > WAKE_ODDS) return hi; }
        u.Hp = u.MaxHp = lo;
        { var (pa, pb) = Combat.PowerVs(side, one); if (Combat.WinChance(pa, pb * ALLY_FEAR) <= WAKE_ODDS) return lo; }
        for (int it = 0; it < 14; it++)
        {
            double mid = Math.Floor((lo + hi) / 2);
            u.Hp = u.MaxHp = mid;
            var (pa, pb) = Combat.PowerVs(side, one);
            if (Combat.WinChance(pa, pb * ALLY_FEAR) > WAKE_ODDS) lo = mid; else hi = mid;
        }
        return Math.Ceiling(hi / 10) * 10;
    }

    /// <summary>İndeki savaşın ayarları: ejderhanın dehşeti saldıranların moralini düşürür; ejderha bozguna uğramaz.</summary>
    public static void LairOpts(BattleOpts o)
    {
        o.MoraleA = (o.MoraleA ?? 0.6) - FEAR;
        o.NoRoutB = true;
    }

    // ------------------------------------------------------------ günlük
    /// <summary>Günlük (Storyteller.Tick'ten): uyanış, yaraların kapanması, yılda bir haraç ve ilanlar, ayda bir ittifak, sefer
    /// takvimi, akınlar (canı %60'ın altındaysa inde bekler).</summary>
    public static void Tick(Sim s)
    {
        var w = s.W;
        w.Dragon ??= new DragonState { WakeDay = s.Rng.Int(17 * Sim.YEAR, 22 * Sim.YEAR - 1) };
        var d = w.Dragon;
        if (d.Camp < 0) { if (s.Day >= d.WakeDay) Wake(s, d); return; }
        var cp = CampOf(s);
        if (cp == null || !cp.Alive) return;
        Grow(d, cp);
        if (d.Hp < d.MaxHp) d.Hp = JsMath.Min(d.MaxHp, d.Hp + d.MaxHp * HEAL);
        // ittifak, yolda ya da inde ordusu kalmayınca dağılır
        if (d.Alliance.Count > 0 && d.Marches.Count == 0
            && !J.Some(w.Agents, b => !J.T(b.Dead) && !J.T(b.Returning) && b.Kind == "army" && b.Purpose == "expedition" && b.To == cp.Id && d.Alliance.Contains(b.Civ)))
            d.Alliance.Clear();
        double since = s.Day - d.WakeDay;
        if (since % Sim.YEAR == 60) { Demand(s, d, cp); Bounties(s, d, cp); }
        if (since % 30 == 15) ConsiderAlliance(s, d, cp);
        Marches(s, d, cp);
        if (s.Day >= d.NextRaid)
        {
            if (d.Hp >= d.MaxHp * 0.6) Raid(s, d, cp, null, null);
            else d.NextRaid = s.Day + 15;   // yaralı ejderha inde yaralarını yalar
        }
    }

    /// <summary>Anlatıcının "ejderha akını" krizi için: ejderha yaşıyor, yaraları akına el veriyor ve menzilinde korunmasız
    /// bir yerleşim var.</summary>
    public static bool CanRaid(Sim s)
    {
        var d = s.W.Dragon;
        var cp = CampOf(s);
        if (d == null || cp == null || !cp.Alive || d.Hp < d.MaxHp * 0.6) return false;
        return J.Some(s.W.Settlements, x => Target(s, d, cp, x));
    }

    // ------------------------------------------------------------ uyanış
    private static void Wake(Sim s, DragonState d)
    {
        var w = s.W;
        int t = LairTile(s);
        if (t < 0) { d.WakeDay = s.Day + 30; return; }   // uygun dağ yoksa bir ay sonra yeniden
        string name = s.Rng.Pick(NAMES);
        var cp = WorldGen.MakeCamp(s.Id(), "dragon", t, $"{name} İni", s.Day, s.Rng);
        cp.Count = 1; cp.Boss = true; cp.HadBoss = true; cp.Captain = name;
        cp.Loot = s.Rng.Int(400, 900);   // yüzyıllık uykusundan kalan hazine
        d.Loot0 = cp.Loot;               // Faz 1 C3: büyüme bundan sonra kaçırılan altınla
        w.Camps.Add(cp); w.Tiles[t].Camp = cp.Id; w.Tiles[t].Owner = -1;
        double hp = WakeHp(s, t);
        d.Camp = cp.Id; d.Name = name; d.BaseHp = hp; d.MaxHp = hp; d.Hp = hp; d.WakeDay = s.Day;
        d.NextRaid = s.Day + s.Rng.Int(25, 60);
        s.Metric("dragonWake");
        var near = Nearest(s, t);
        string where = near != null ? $"{Tr.Ek(near.Name, "in")} {Dir(s, near.Tile, t)} dağlarda" : "uzak dağlarda";
        string age = hp >= 2000 ? "kadim" : hp >= 1300 ? "yaşlı" : "genç";
        s.Log("dragon", $"Ejderha uyandı! {J.TrCap(age)} kızıl ejderha {name}, {where} yüzyıllık uykusundan uyandı ve inine altın yığmaya başladı.", tile: t, major: true,
            cause: "Büyüyen krallıkların gürültüsü ve altın kokusu onu uyandırdı");
    }

    /// <summary>
    /// İn: dağ eteğinde (en az iki komşusu dağ; bulunamazsa bir, sonra hiç), sahipsiz, adada ve han halkasında olmayan, kampsız ve
    /// yapısız bir kara karosu; en yakın yerleşime 12–20 fersah (medeniyetlerin "sınıra fazla yakın" diye kendiliğinden sefer
    /// açtığı 11 fersahın dışında; yer yoksa 9'dan). Puan: menzildeki yerleşim sayısı (akınlar için), dağ komşuluğu, tepe; en yakın
    /// yerleşimden yürüyerek ulaşılabilmeli (kahramanlar ve ordular gelebilsin). En iyi 6 aday arasından zarla.
    /// </summary>
    private static int LairTile(Sim s)
    {
        var w = s.W;
        var setts = J.Filter(w.Settlements, x => x.Alive);
        if (setts.Count == 0) return -1;
        for (int pass = 0; pass < 6; pass++)
        {
            int need = 2 - pass % 3;
            double lo = pass < 3 ? 12 : 9, hi = pass < 3 ? 20 : 18;
            var cand = new List<int>();
            var score = new List<double>();
            for (int i = 0; i < w.Tiles.Count; i++)
            {
                var t = w.Tiles[i];
                if (t.Terrain == "water" || t.Terrain == "mountain" || J.T(t.Sea) || t.Owner >= 0 || t.Camp != null || t.Ext != null || J.T(t.Isle) || t.InnZone != null || t.Inn != null) continue;
                double m = 0;
                foreach (int nb in s.G.Neighbors(i)) if (w.Tiles[nb].Terrain == "mountain") m++;
                if (m < need) continue;
                double dmin = double.PositiveInfinity, inRange = 0;
                foreach (var x in setts) { double dd = s.G.Dist(x.Tile, i); if (dd < dmin) dmin = dd; if (dd <= RANGE) inRange++; }
                if (dmin < lo || dmin > hi) continue;
                cand.Add(i);
                score.Add(JsMath.Min(inRange, 14) + m * 0.8 + (t.Terrain == "hill" ? 1.5 : 0) - Math.Abs(dmin - lo - 1) * 0.3);
            }
            if (cand.Count == 0) continue;
            var idx = J.Sort(J.From(cand.Count, k => k), (a, b) => J.Or(score[b] - score[a], cand[a] - cand[b]));
            var ok = new List<int>();
            for (int k = 0; k < idx.Count && k < 24 && ok.Count < 6; k++)
            {
                var near = Nearest(s, cand[idx[k]]);
                if (near != null && s.Path(near.Tile, cand[idx[k]]) != null) ok.Add(idx[k]);
            }
            if (ok.Count == 0) continue;
            double low = score[ok[ok.Count - 1]];
            int pick = s.Rng.Weighted(ok, k => score[k] - low + 1);
            return cand[pick];
        }
        return -1;
    }

    private static Settlement Nearest(Sim s, int tile)
    {
        Settlement best = null; double bd = double.PositiveInfinity;
        foreach (var x in s.W.Settlements) if (x.Alive) { double d = s.G.Dist(x.Tile, tile); if (d < bd) { bd = d; best = x; } }
        return best;
    }

    /// <summary>Yön (sıfat-fiil hâli): "kuzeyindeki", "güneydoğusundaki"... (ekrandaki konuma göre; kuzey yukarı)</summary>
    private static string Dir(Sim s, int from, int to)
    {
        var (x0, z0) = s.G.Pixel(from, 1);
        var (x1, z1) = s.G.Pixel(to, 1);
        double dx = x1 - x0, dn = z0 - z1;   // dn > 0: kuzey
        double ax = Math.Abs(dx), an = Math.Abs(dn);
        const double T = 0.4142;   // tan 22,5°
        if (an <= ax * T) return dx >= 0 ? "doğusundaki" : "batısındaki";
        if (ax <= an * T) return dn >= 0 ? "kuzeyindeki" : "güneyindeki";
        return (dn >= 0 ? "kuzey" : "güney") + (dx >= 0 ? "doğusundaki" : "batısındaki");
    }

    // ------------------------------------------------------------ akın
    /// <summary>Akına uygun hedef: yaşayan, ejderhanın menzilinde, haraçla korunmayan ve en az 10 nüfuslu yerleşim.</summary>
    private static bool Target(Sim s, DragonState d, Camp cp, Settlement x) =>
        x.Alive && s.W.Civs[x.Civ].Alive && s.G.Dist(x.Tile, cp.Tile) <= RANGE && (d.Paid.Get(x.Civ) ?? -1) < s.Day && s.Pop(x) >= 10;

    private static Settlement PickTarget(Sim s, DragonState d, Camp cp)
    {
        var w = s.W;
        var cands = J.Filter(w.Settlements, x => Target(s, d, cp, x));
        if (cands.Count == 0) return null;
        // ejderha altını sever, zayıf savunmayı seçer; haracı reddedeni ve ona karşı ittifak kuranı cezalandırır
        return s.Rng.Weighted(cands, x =>
        {
            var c = w.Civs[x.Civ];
            double refused = (d.Refused.Get(c.Id) ?? -1) == s.Year ? 2 : 1;
            double ally = d.Alliance.Contains(c.Id) ? 1.5 : 1;
            return (1 + Math.Sqrt(JsMath.Max(0, s.St(c, "gold"))) / 10) * (1 + s.Pop(x) / 60) * refused * ally / (1 + x.Soldiers * 0.08) / (1 + s.G.Dist(x.Tile, cp.Tile) / 12.0);
        });
    }

    /// <summary>akında ejderhaya karşı duran en düşük kahraman seviyesi: acemiler halkla birlikte kilerlere saklanır</summary>
    public const int DEFEND_LEVEL = 3;

    /// <summary>Ejderhaya karşı savunanlar: askerler (surların ardında), yurtta duran kiralık kahramanlar, yerleşimin tavernasındaki
    /// serbest kahramanlar (en az Sv <see cref="DEFEND_LEVEL"/>: ejderhanın nefesi acemiyi kül eder, onlar halkla saklanır).
    /// Milis savaşmaz: ejderha havadan saldırır, halk kilerlere saklanır.</summary>
    private static List<Combatant> Defenders(Sim s, Settlement st)
    {
        var c = s.W.Civs[st.Civ];
        var cs = Agents.CivTroops(s, c, st.Soldiers, "A");
        double ac = (J.T(st.Civics.Get("palisade")) ? 1 : 0) + (J.T(st.Civics.Get("stonewall")) ? 2 : 0) + (J.T(st.Civics.Get("castle")) ? 3 : 0) + s.E(c, "defAc");
        foreach (var x in cs) x.Ac += ac;
        foreach (var h in s.CivHeroes(c)) if (h.State == "home" && h.Pos == st.Tile && h.Level >= DEFEND_LEVEL) cs.Add(Combat.HeroCombatant(h, "A"));
        foreach (var h in s.W.Heroes) if (h.Civ == -1 && h.State == "tavern" && !h.BaseInn && h.Tavern == st.Id && h.Level >= DEFEND_LEVEL) cs.Add(Will.HeroSide(h, "A"));
        return cs;
    }

    /// <summary>
    /// Ejderha akını. <paramref name="forced"/>: hedef (yoksa menzildeki korunmasız yerleşimlerden altını çok, savunması zayıf ve
    /// yakın olan seçilir). Savunucular üç tur çarpışır (ejderha nefesini kullanır). Ejderha ölürse <see cref="Fall"/>; canının
    /// %5'ini (DRIVEN) yitirirse ya da %40'ın altına düşerse yarım işle inine çekilir (az yangın, altın yok); yoksa tarlaları, yapıları
    /// ve evleri yakar, halkı öldürür, hazineden altın ve ambardan tahıl kaçırır. <paramref name="why"/>: kroniğin nedeni.
    /// </summary>
    public static bool Raid(Sim s, DragonState d, Camp cp, Settlement forced, string why)
    {
        var w = s.W;
        d.NextRaid = s.Day + s.Rng.Int(70, 130);
        var st = forced ?? PickTarget(s, d, cp);
        if (st == null) return false;
        var c = w.Civs[st.Civ];
        bool refused = (d.Refused.Get(c.Id) ?? -1) == s.Year;
        var def = Defenders(s, st);
        var dr = Side(s, cp, "B");
        var dragon = dr[0];
        double hp0 = dragon.Hp;
        Battle b = null;
        if (def.Count > 0)
        {
            var o = new BattleOpts { Id = s.Id(), Day = s.Day, Tile = st.Tile, Title = $"{Tr.Ek(st.Name, "a")} ejderha akını", SideA = $"{st.Name} savunucuları", SideB = "Ejderha", MoraleA = 0.8 - FEAR, NoRoutB = true, MaxRounds = 3, TimeoutWinner = "B", CivA = c.Id };
            b = Combat.ResolveBattle(s.Rng, def, dr, Agents.CivOpts(s, c, "A", o));
        }
        bool slain = dragon.Hp <= 0;
        double lost = hp0 - JsMath.Max(0, dragon.Hp);
        bool driven = !slain && (lost >= d.MaxHp * DRIVEN || dragon.Hp < d.MaxHp * 0.4);
        if (b != null)
        {
            // üç tur sonra ejderha göğe yükselir: canının büyük kısmını yitirdiyse savunanlar kazanmış sayılır
            if (b.Replay?.End == "timeout")
            {
                b.Winner = driven ? "A" : "B";
                if (b.Lines.Count > 0) b.Lines[b.Lines.Count - 1] = new BattleLine { T = driven ? $"{dragon.Name} yaralı hâlde göğe yükselip inine çekildi." : $"{dragon.Name} işini bitirip göğe yükseldi." };
            }
            Agents.RecordBattle(s, b);
            Agents.SyncHeroes(s, def, 0, dr, Label(s), "defend");
        }
        double heroDead = J.Filter(def, x => x.Hero != null && x.Hero.State == "dead").Count;
        double deadS = J.Filter(def, x => (x.Kind == "soldier" || x.Kind == "unique") && x.Hp <= 0).Count;
        double back = deadS > 0 ? Gear.HealWounded(s, c, deadS) : 0;
        double fire = slain ? 0 : driven ? s.Rng.Int(0, 2) : s.Rng.Int(1, 3) + st.Tier * 2;   // alevlerde ölen siviller
        fire = JsMath.Max(0, JsMath.Min(fire, s.Pop(st) - st.Soldiers - 3));
        if (deadS - back + fire > 0) s.RemovePop(st, deadS - back + fire);
        st.Soldiers = JsMath.Max(0, st.Soldiers - deadS + back);
        double deadAll = deadS - back + fire + heroDead;
        d.Hits.Set(c.Id, (d.Hits.Get(c.Id) ?? 0) + 1);
        d.Raids++;
        c.LastRaidedDay = s.Day;
        s.Metric("raids"); s.Metric("dragonRaid");
        if (slain) { Fall(s, cp, dragon, def, b, $"{st.Name} savunucuları", "kendi surlarının üstünde", c); return true; }
        d.Hp = JsMath.Max(1, dragon.Hp);
        // yangın: önce tarlalar, sonra öbür çıkarma yapıları; evler
        var exts = new List<int>();
        foreach (int i in s.G.Within(st.Tile, s.RadiusOf(st))) { var t = w.Tiles[i]; if (t.Ext != null && t.Owner == st.Id && s.ExtWorking(t)) exts.Add(i); }
        J.Sort(exts, (x, y) => J.Or((w.Tiles[y].Ext.Kind == "farm" ? 1 : 0) - (w.Tiles[x].Ext.Kind == "farm" ? 1 : 0), x - y));
        double burnN = driven ? s.Rng.Int(0, 1) : s.Rng.Int(2, 3) + st.Tier;
        double fields = 0, other = 0;
        for (int k = 0; k < exts.Count && k < burnN; k++)
        {
            var t = w.Tiles[exts[k]];
            t.Ext.Burned = s.Day + s.Rng.Int(60, 120); t.Ext.BurnedAt = s.Day; t.Ext.Workers = 0;
            s.Metric("extBurned");
            if (t.Ext.Kind == "farm") fields++; else other++;
        }
        double houses = driven ? s.Rng.Int(1, 2) : s.Rng.Int(2, 4) + st.Tier;
        st.BurnedHouses = (st.BurnedHouses ?? 0) + houses; st.BurnedAt = s.Day;
        // Faz 1 C3: ejderha yaktığı kentin hazine payını kaçırır (nüfus payının yarısı, en çok %20; eskiden bütün hazinenin %20'si)
        double gold = driven ? 0 : Math.Floor(s.St(c, "gold") * JsMath.Min(0.2, s.Pop(st) / JsMath.Max(1, s.CivPop(c)) * 0.5));
        if (gold > 0) { s.Add(c, "gold", -gold); cp.Loot += gold; s.Metric("dragonRaidGold", gold); }
        double grain = Math.Floor(s.St(c, "grain") * (driven ? 0.03 : 0.1));
        if (grain > 0) s.Add(c, "grain", -grain);
        c.Threat += driven ? 0.3 : 0.6;
        if (driven) c.Stats.BattlesWon++; else c.Stats.BattlesLost++;
        Will.OnHomeBurned(s, st, cp);
        var burnt = new List<string>();
        if (fields > 0) burnt.Add($"{J.S(fields)} tarla");
        if (other > 0) burnt.Add($"{J.S(other)} yapı");
        burnt.Add($"{J.S(houses)} ev");
        if (driven)
        {
            s.Metric("dragonDriven");
            s.Log("dragon", $"{st.Name}, ejderha {Tr.Ek(d.Name, "i")} püskürttü: yaralı ejderha {Lore.JoinVe(burnt)} yakıp inine çekildi{(deadAll > 0 ? $"; {J.S(deadAll)} kişi öldü" : "")}.", civ: c.Id, tile: st.Tile, battle: b?.Id, major: true,
                cause: $"Savunucular ejderhayı kanattı; kolay av bekleyen ejderha geri çekildi (canı {J.S(JsMath.Round(d.Hp))}/{J.S(d.MaxHp)})");
        }
        else
        {
            s.Log("dragon", $"Ejderha {d.Name}, {Tr.Ek(st.Name, "in")} üzerine alev yağdırdı: {Lore.JoinVe(burnt)} kül oldu{(deadAll > 0 ? $", {J.S(deadAll)} kişi öldü" : "")}{(gold >= 50 ? $"; hazineden {J.S(gold)} altın kaçırdı" : "")}.", civ: c.Id, tile: st.Tile, battle: b?.Id, major: true,
                cause: why ?? (refused ? $"{c.Name} haraç vermeyi reddetmişti" : d.Alliance.Contains(c.Id) ? "Ejderha kendisine karşı kurulan ittifakı cezalandırıyor" : $"Ejderha hazinesini büyütüyor ({J.S(JsMath.Round(cp.Loot))} altın)"));
        }
        return true;
    }

    // ------------------------------------------------------------ haraç ve ilanlar
    /// <summary>Medeniyetin ejderhaya karşı çıkarabileceği güç (ejderhanın zırhına karşı): askerlerinin %65'i ve yurttaki sağlıklı
    /// kahramanları.</summary>
    private static List<Combatant> Force(Sim s, Civ c, double share = 0.65)
    {
        double soldiers = Math.Floor(J.Reduce(s.CivSettlements(c), (a, x) => a + x.Soldiers, 0.0) * share);
        var cs = Agents.CivTroops(s, c, soldiers, "A");
        foreach (var h in s.CivHeroes(c)) if (h.State == "home" && h.Hp > h.MaxHp * 0.7) cs.Add(Combat.HeroCombatant(h, "A"));
        return cs;
    }

    /// <summary>
    /// Yılda bir (uyanışından 60 gün sonra ve her yıl): menzildeki medeniyetlerden haraç ister (hazinenin %12'si, her yıl +%2, en
    /// çok %30; en az 40 altın; Faz 1 C3: en çok TRIBUTE_CAP + nüfus × TRIBUTE_POP). Ödeyen bir yıl dokunulmaz kalır. Karar: korku (ejderhanın gücü / çıkarabileceği ordunun gücü),
    /// yediği akınlar ve tabiatı (düzenciler pazarlığa yatkın, iyiler değil); paladinler hiç ödemez, ittifak üyeleri ödemez,
    /// altını yetmeyen ödeyemez.
    /// </summary>
    private static void Demand(Sim s, DragonState d, Camp cp)
    {
        var w = s.W;
        double years = Math.Floor((s.Day - d.WakeDay) / Sim.YEAR);
        double share = JsMath.Min(0.3, TRIBUTE + 0.02 * years);
        var dragon = Side(s, cp, "B");
        var paid = new List<string>();
        var refused = new List<string>();
        foreach (var c in w.Civs)
        {
            if (!c.Alive || !J.Some(s.CivSettlements(c), x => s.G.Dist(x.Tile, cp.Tile) <= RANGE)) continue;
            double gold = s.St(c, "gold");
            // Faz 1 C3: haraç hazinenin payıdır ama nüfusla tavanlı (TRIBUTE_CAP + kişi başı TRIBUTE_POP): koca hazineler haracı saçmalaştırıyordu
            double tribute = JsMath.Round(JsMath.Max(40, JsMath.Min(gold * share, TRIBUTE_CAP + s.CivPop(c) * TRIBUTE_POP)));
            double hits = d.Hits.Get(c.Id) ?? 0;
            var (pa, pb) = Combat.PowerVs(Force(s, c), dragon);
            double fear = pb / JsMath.Max(1, pa);
            double p = 0.1 + 0.12 * JsMath.Min(4, hits) + 0.25 * JsMath.Max(-1, JsMath.Min(2, fear - 1)) + (c.Align.Law > 0.3 ? 0.1 : 0) - (c.Align.Good > 0.3 ? 0.1 : 0)
                + (Evil(c) ? 0.3 : 0);   // kötüler ejderhayla pazarlık eder
            bool pays = gold >= tribute && c.Cls != "paladin" && !d.Alliance.Contains(c.Id) && s.Rng.Chance(JsMath.Max(0.05, JsMath.Min(0.85, p)));
            if (pays)
            {
                s.Add(c, "gold", -tribute);
                cp.Loot += tribute;
                d.Paid.Set(c.Id, s.Day + Sim.YEAR);
                d.Tributes += tribute;
                paid.Add($"{c.Name} {J.S(tribute)} altın");
                s.Metric("dragonTribute"); s.Metric("dragonTributeGold", tribute);
            }
            else
            {
                d.Refused.Set(c.Id, s.Year);
                refused.Add(c.Name);
                s.Metric("dragonRefused");
            }
        }
        if (paid.Count == 0 && refused.Count == 0) return;
        string text = paid.Count > 0 && refused.Count > 0 ? $"Ejderha {d.Name} haraç istedi: {Lore.JoinVe(paid)} ödedi; {Lore.JoinVe(refused)} boyun eğmedi."
            : paid.Count > 0 ? $"Ejderha {d.Name} haraç istedi; {Lore.JoinVe(paid)} ödedi."
            : $"Ejderha {d.Name} haraç istedi; {Lore.JoinVe(refused)} boyun eğmedi.";
        s.Log("dragon", text, tile: cp.Tile, major: true,
            cause: paid.Count > 0 ? "Haraç bir yıllık aman getirir; reddedenler ejderhanın öfkesini üstüne çeker" : "Kılıç haraçtan ucuz sayıldı; ejderhanın öfkesi kapıda");
    }

    /// <summary>Yılda bir: menzildeki hanlar panolarına büyük ejderha ilanı asar (en az 150 altını olan han, altınının yarısı, en çok
    /// 500); haracı reddeden zengin medeniyetler (en az BOUNTY_GOLD altın) de en yakın hana kendi ödüllerini koyar (hazinenin
    /// BOUNTY_SHARE payı, en az 100, en çok BOUNTY_MAX): ejderha başına birkaç düzine altınlık ilan asılmaz. İlanlar 3 yıl açık kalır.</summary>
    private static void Bounties(Sim s, DragonState d, Camp cp)
    {
        var w = s.W;
        var inns = w.Inns;
        for (int i = 0; i < inns.Count; i++)
        {
            var inn = inns[i];
            if (!inn.Alive || s.G.Dist(inn.Tile, cp.Tile) > INN_RANGE || inn.Gold < 150) continue;
            if (J.Some(w.Quests, q => q.Open && q.Camp == cp.Id && q.Civ == -1 && q.Inn == inn.Id)) continue;
            double bounty = JsMath.Round(JsMath.Min(inn.Gold * 0.5, 500));
            inn.Gold -= bounty;
            w.Quests.Add(new Quest { Id = s.Id(), Civ = -1, Camp = cp.Id, Bounty = bounty, Posted = s.Day, TakenBy = new List<int>(), Open = true, Inn = inn.Id, Expires = s.Day + 3 * Sim.YEAR });
            s.Metric("innQuest"); s.Metric("dragonBounty");
            InnLife.InnEvent(s, inn, $"Panoya ejderha ilanı asıldı: {d.Name} ({J.S(bounty)} altın)");
            s.Log("quest", $"Hancı {inn.Keeper}, {Inns.InnName(inn)} panosuna ejderha ilanı astı: \"{d.Name} öldürülsün, ödül {J.S(bounty)} altın.\"", tile: inn.Tile, major: true,
                cause: $"Ejderhanın ini hana {J.S(s.G.Dist(cp.Tile, inn.Tile))} fersah; yollar yanıyor");
        }
        var civs = w.Civs;
        for (int i = 0; i < civs.Count; i++)
        {
            var c = civs[i];
            if (!c.Alive || (d.Refused.Get(c.Id) ?? -1) != s.Year || s.St(c, "gold") < BOUNTY_GOLD) continue;
            if (J.Some(w.Quests, q => q.Open && q.Camp == cp.Id && q.Civ == c.Id)) continue;
            var cap = s.Capital(c);
            if (cap == null) continue;
            var inn = J.At(J.Sort(J.Filter(w.Inns, x => x.Alive), (a, b) => J.Or(s.G.Dist(a.Tile, cp.Tile) - s.G.Dist(b.Tile, cp.Tile), a.Id - b.Id)), 0);
            double bounty = JsMath.Round(JsMath.Min(JsMath.Max(100, s.St(c, "gold") * BOUNTY_SHARE), BOUNTY_MAX));
            s.Add(c, "gold", -bounty);
            w.Quests.Add(new Quest { Id = s.Id(), Civ = c.Id, Camp = cp.Id, Bounty = bounty, Posted = s.Day, TakenBy = new List<int>(), Open = true, Inn = inn != null && s.G.Dist(inn.Tile, cp.Tile) <= INN_RANGE ? inn.Id : null, Expires = s.Day + 3 * Sim.YEAR });
            s.Metric("questPosted"); s.Metric("dragonBounty");
            s.Log("quest", $"{c.Name}, ejderha {Tr.Ek(d.Name, "in")} başına {J.S(bounty)} altın ödül koydu.", civ: c.Id, tile: cap.Tile, major: true, cause: "Haraç yerine kılıç: ejderhayı öldürene servet");
        }
    }

    // ------------------------------------------------------------ ittifak
    /// <summary>
    /// Ayda bir: ejderhaya karşı ittifak. Ejderha en az iki akın yapmış olmalı. Aday: menzilde, haraçla korunmayan, ejderhanın en
    /// az bir akınını yemiş, kötü hizalı olmayan medeniyetler; en çok yara alandan başlayarak birbirleriyle savaşta olmayanlar (ve
    /// araları çok kötü olmayanlar) toplanır. Ortak güç (her birinin askerlerinin %65'i ve yurttaki sağlıklı kahramanları)
    /// ejderhanın dehşetiyle büyütülmüş gücüne (× <see cref="ALLY_FEAR"/>) karşı en az <see cref="ALLY_WIN"/> kazanma şansı
    /// veriyorsa ittifak kurulur: ordular inde aynı anda buluşacak biçimde sırayla yola çıkar (<see cref="Marches"/>; inde
    /// birbirlerini <see cref="MUSTER"/> gün bekler). Kurulan ittifak bir sonraki denemeyi iki yıl, inde püskürtülen ittifak
    /// dört yıl erteler.
    /// </summary>
    private static void ConsiderAlliance(Sim s, DragonState d, Camp cp)
    {
        var w = s.W;
        if (s.Day < d.NextAlliance || d.Raids < 2 || d.Marches.Count > 0) return;
        if (J.Some(w.Agents, a => !J.T(a.Dead) && a.Kind == "army" && a.Purpose == "expedition" && a.To == cp.Id && !J.T(a.Returning))) return;
        var cands = J.Filter(w.Civs, c => c.Alive && !Evil(c) && (d.Hits.Get(c.Id) ?? 0) >= 1 && (d.Paid.Get(c.Id) ?? -1) < s.Day && s.Capital(c) != null
            && J.Some(s.CivSettlements(c), x => s.G.Dist(x.Tile, cp.Tile) <= RANGE));
        J.Sort(cands, (a, b) => J.Or((d.Hits.Get(b.Id) ?? 0) - (d.Hits.Get(a.Id) ?? 0), a.Id - b.Id));
        var members = new List<Civ>();
        foreach (var c in cands) if (!J.Some(members, m => s.AtWar(m.Id, c.Id) || s.RelValue(m.Id, c.Id) <= -30)) members.Add(c);
        if (members.Count == 0) return;
        var side = new List<Combatant>();
        foreach (var c in members) side.AddRange(Force(s, c));
        if (side.Count == 0) return;
        var (pa, pb) = Combat.PowerVs(side, Side(s, cp, "B"));
        double win = Combat.WinChance(pa, pb * ALLY_FEAR);
        if (win < ALLY_WIN) return;
        // sefer takvimi: herkes inde aynı gün buluşsun
        var eta = new List<double>();
        var go = new List<Civ>();
        foreach (var c in members)
        {
            var cap = s.Capital(c);
            var path = s.Path(cap.Tile, cp.Tile);
            if (path == null) continue;
            var probe = new Agent { Id = -1, Kind = "army", Civ = c.Id, Path = path, Step = 0, Progress = 0, Speed = 0.6, To = cp.Id, Purpose = "expedition" };
            go.Add(c); eta.Add(Agents.EtaDays(s, probe));
        }
        if (go.Count == 0) return;
        double gather = s.Day + J.MaxOf(eta, x => x) + 5;
        for (int i = 0; i < go.Count; i++) { double day = JsMath.Max(s.Day, Math.Floor(gather - eta[i])); d.Marches.Add(new DragonMarch { Civ = go[i].Id, Day = day, Until = day + 30 }); }
        d.Alliance = J.Map(go, c => c.Id);
        d.AllianceTries++;
        d.NextAlliance = s.Day + 2 * Sim.YEAR;
        if (go.Count > 1) foreach (var x in go) foreach (var y in go) if (!ReferenceEquals(x, y)) s.SetMod(x.Id, y.Id, "dragonpact", "Ejderhaya karşı ittifak", 15, 0.01, false);
        s.Metric("dragonAlliance");
        string names = Lore.JoinVe(J.Map(go, c => c.Name));
        s.Log("dragon", go.Count > 1
                ? $"Ejderhaya karşı ittifak kuruldu: {names} ordularını {Tr.Ek(cp.Name, "in")} önünde birleştirecek."
                : $"{go[0].Name} ejderha {Tr.Ek(d.Name, "a")} karşı tek başına sefer hazırlıyor.",
            civ: go[0].Id, tile: cp.Tile, major: true,
            cause: $"Ejderha {J.S(d.Raids)} kez akın etti; {(d.AllianceTries > 1 ? $"{J.S(d.AllianceTries)}. deneme" : "kılıç haraçtan ucuz sayıldı")}");
    }

    /// <summary>İnde saldırmak üzere olan grup (Agents.Engage): kendisi ya da inde bekleyenlerden biri ittifak ordusuysa ve ittifakın
    /// yola çıkmamış ya da yoldaki başka bir ordusu varsa, ilk ittifak ordusunun varışından en çok GATHER gün sonrasına dek
    /// saldırmadan beklenir; ittifak orduları ejderhaya tek tek değil, birlikte girer.</summary>
    public static bool Gathering(Sim s, Agent a)
    {
        var d = s.W.Dragon;
        if (d == null || d.Alliance.Count == 0) return false;
        var mine = J.Filter(s.W.Agents, b => !J.T(b.Dead) && !J.T(b.Returning) && b.Kind == "army" && b.Purpose == "expedition" && b.To == a.To && d.Alliance.Contains(b.Civ));
        var waiting = J.Filter(mine, b => b.Muster != null && Agents.AtTarget(b));
        if (waiting.Count == 0) return false;
        if (s.Day - J.MinOf(waiting, b => b.Muster.Since) >= GATHER) return false;
        return d.Marches.Count > 0 || J.Some(mine, b => !Agents.AtTarget(b));
    }

    /// <summary>ittifak ordusunun inde öbür üyeleri bekleyeceği en uzun süre (gün)</summary>
    public const double GATHER = 90;

    /// <summary>İki grup aynı ejderha ittifakının ejderhanın inine giden ordularıysa dosttur (Agents.Friendly): ortak düşmana karşı
    /// aralarındaki soğukluk ya da kahramanlarının hizası onları ayırmaz.</summary>
    public static bool Allied(Sim s, Agent a, Agent b)
    {
        var d = s.W.Dragon;
        return d != null && d.Alliance.Count > 1 && a.Kind == "army" && b.Kind == "army" && a.Purpose == "expedition" && b.Purpose == "expedition"
            && a.To == d.Camp && b.To == d.Camp && d.Alliance.Contains(a.Civ) && d.Alliance.Contains(b.Civ);
    }

    /// <summary>Kötü hizalı medeniyet (iyi/kötü ekseni −0,3'ün altında): ejderhaya karşı ittifaka girmez, haraca yatkındır.</summary>
    private static bool Evil(Civ c) => c.Align.Good < -0.3;

    /// <summary>Takvimi gelen ittifak ordusu yola çıkar: askerlerin %65'i ve yurttaki sağlıklı kahramanlar (sefer: inde buluşma).</summary>
    private static void Marches(Sim s, DragonState d, Camp cp)
    {
        if (d.Marches.Count == 0) return;
        var w = s.W;
        for (int i = 0; i < d.Marches.Count; i++)
        {
            var m = d.Marches[i];
            if (s.Day < m.Day) continue;
            var c = w.Civs[m.Civ];
            // başka bir seferdeki ordu dönene dek (en çok 30 gün) bekler
            if (c.Alive && s.Day < m.Until && J.Some(w.Agents, a => !J.T(a.Dead) && a.Civ == c.Id && a.Purpose == "expedition")) { m.Day = s.Day + 5; continue; }
            d.Marches.RemoveAt(i); i--;
            if (!c.Alive) continue;
            // ittifaka ordu gönderemeyen üye kronikte anılır (müttefikleri inde boşuna bekler ya da yalnız girer)
            void Fail(string why) => s.Log("dragon", $"{c.Name}, ejderhaya karşı ittifaka söz verdiği orduyu gönderemedi.", civ: c.Id, tile: cp.Tile, cause: why);
            var busy = J.Find(w.Agents, a => !J.T(a.Dead) && a.Civ == c.Id && a.Purpose == "expedition");
            if (busy != null) { if (busy.To != cp.Id) Fail("Ordusu başka bir seferden dönemedi"); continue; }
            var cap = s.Capital(c);
            if (cap == null) continue;
            var path = s.Path(cap.Tile, cp.Tile);
            if (path == null) continue;
            double soldiers = Math.Floor(J.Reduce(s.CivSettlements(c), (a, x) => a + x.Soldiers, 0.0) * 0.65);
            var home = J.Filter(s.CivHeroes(c), h => h.State == "home" && h.Hp > h.MaxHp * 0.7);
            if (soldiers < 3 && home.Count == 0) { Fail("Savaşlar ve akınlar ordusunu eritmişti"); continue; }
            var pop = Agents.DrawSoldiers(s, c, soldiers);
            foreach (var h in home) h.State = "army";
            w.Agents.Add(new Agent { Id = s.Id(), Kind = "army", Civ = c.Id, Path = path, Step = 0, Progress = 0, Speed = 0.6, Heroes = J.Map(home, h => h.Id), Troops = soldiers, Pop = pop, From = cap.Id, To = cp.Id, Purpose = "expedition" });
            s.Metric("dragonMarch");
            s.Log("quest", $"{c.Name}{(home.Count > 0 ? $", {Lore.JoinVe(J.Map(home, h => h.Name))} önderliğinde" : "")} {J.S(soldiers)} askerle ejderha {Tr.Ek(d.Name, "a")} karşı yola çıktı.", civ: c.Id, tile: cap.Tile, cause: "Ejderhaya karşı ittifak");
        }
    }

    // ------------------------------------------------------------ ölüm ve yenilgi
    /// <summary>İnindeki savaşta öldü (Agents.FightCamp; hazine orada güç payına göre paylaşıldı). Kronikte ordular adlarıyla,
    /// kahraman grupları kahramanlarıyla anılır; orduların başındaki kahramanlar "önderliğinde" diye eklenir.</summary>
    public static void SlainAtLair(Sim s, Camp cp, List<Agent> band, List<Combatant> side, List<Combatant> mons, Battle b, double loot)
    {
        var dragon = J.Find(mons, m => m.Breath != null) ?? J.At(mons, 0);
        s.W.Dragon.Hoard = loot;
        var armies = new List<string>();
        var party = new List<string>();
        var leaders = new List<string>();
        foreach (var a in band)
        {
            var hs = J.Filter(J.Map(a.Heroes ?? new List<int>(), id => s.Hero(id)), h => h != null && h.State != "dead");
            if (a.Kind == "army" && a.Civ >= 0) { string cn = s.W.Civs[a.Civ].Name; if (!armies.Contains(cn)) armies.Add(cn); foreach (var h in hs) leaders.Add(h.Name); }
            else foreach (var h in hs) party.Add(h.Name);
        }
        static List<string> Cap(List<string> xs) { if (xs.Count <= 3) return xs; var o = J.Slice(xs, 0, 2); o.Add($"{J.S(xs.Count - 2)} kahraman daha"); return o; }
        // "A ve B orduları, yanlarında C ile" / "C ve D"; ordulara önderlik eden kahramanlar ayrı yan cümlede
        string who = armies.Count > 0 ? $"{Lore.JoinVe(armies)} {(armies.Count > 1 ? "orduları" : "ordusu")}{(party.Count > 0 ? $", yanlarında {Lore.JoinVe(Cap(party))} ile," : "")}"
            : party.Count > 0 ? Lore.JoinVe(Cap(party)) : "Saldıranlar";
        string lead = leaders.Count > 0 ? $"; {(armies.Count > 1 ? "ordulara" : "orduya")} {Lore.JoinVe(Cap(leaders))} önderlik etti" : "";
        Fall(s, cp, dragon, side, b, who, "inine girip", null, lead);
    }

    /// <summary>İnindeki savaşta saldıranları püskürttü (Agents.FightCamp): yaraları kalıcı, ittifakın bir sonraki denemesi 4 yıl sonra,
    /// ejderha intikam akınına çıkar.</summary>
    public static void Repelled(Sim s, Camp cp, List<Combatant> mons, Battle b, string who, List<Quest> qs, int? civ)
    {
        var d = s.W.Dragon;
        var dragon = J.Find(mons, m => m.Breath != null);
        if (dragon != null) d.Hp = JsMath.Max(1, dragon.Hp);
        if (d.Alliance.Count > 0) d.NextAlliance = JsMath.Max(d.NextAlliance, s.Day + 4 * Sim.YEAR);   // yenilgi ittifakın belini kırdı
        d.NextRaid = JsMath.Min(d.NextRaid, s.Day + s.Rng.Int(15, 40));
        s.Metric("dragonRepelled");
        s.Log("dragon", $"{who}, ejderha {Tr.Ek(d.Name, "in")} ininde ağır bir yenilgiye uğradı.", tile: cp.Tile, civ: civ, battle: b.Id, major: true,
            cause: $"{(b.Replay?.End == "rout" ? "Ejderhanın dehşeti safları dağıttı" : "Ejderhanın alevleri safları biçti")}; canı {J.S(JsMath.Round(d.Hp))}/{J.S(d.MaxHp)}{(qs.Count > 0 ? $"; ilan {string.Join("/", J.Map(qs, q => J.S(q.Bounty)))} altına çıktı" : "")}");
    }

    /// <summary>
    /// Ejderhanın ölümü. Son darbeyi vuran kahraman büyük ün alır (<see cref="R_SLAYER"/>: efsane olur), «Ejderhakıran» lakabını ve
    /// hazineden ejder ateşinde dövülmüş bir kılıç (+1 saldırı) alır. Son darbe adsız bir askerden geldiyse ozanlar zaferi savaşta
    /// sağ kalan en yüksek seviyeli kahramana (savaşın kahramanı) yazar ve ödülü o alır: büyük orduların içindeki birkaç kahraman
    /// yüzlerce askerin arasında son darbeyi nadiren kendisi vurur. Savaşta sağ kalan diğer kahramanlar da ün ve kilometre taşı
    /// alır. <paramref name="town"/> verilirse ejderha akında öldü: kampı burada kapanır, hazine o medeniyete geçer ve ejderhaya
    /// asılmış ilanların ödülü sahiplerine döner. Haraç ve ittifak düzeni kapanır, menzildeki halklar rahatlar, anlatıcı bir
    /// rahatlama dönemi başlatır.
    /// </summary>
    private static void Fall(Sim s, Camp cp, Combatant dragon, List<Combatant> side, Battle b, string who, string where, Civ town, string lead = "")
    {
        var w = s.W;
        var d = w.Dragon;
        var killer = dragon?.KilledBy;
        var slayer = killer?.Hero != null && killer.Hero.State != "dead" ? killer.Hero : null;
        Hero champion = null;
        if (killer?.Hero == null)
            foreach (var x in side)
            {
                var h = x.Hero;
                if (h == null || h.State == "dead") continue;
                if (champion == null || h.Level > champion.Level || (h.Level == champion.Level && h.Renown > champion.Renown)) champion = h;
            }
        var hero = slayer ?? champion;
        foreach (var x in side)
        {
            var h = x.Hero;
            if (h == null || h.State == "dead" || ReferenceEquals(h, hero)) continue;
            Lore.Deed(s, h, "dragon", $"ejderha {Tr.Ek(d.Name, "in")} düştüğü savaşta kılıç salladı", cp.Tile, d.Name);
            Heroes.AddRenown(s, h, R_PARTY, "dragon");
        }
        if (hero != null)
        {
            if (champion != null) Lore.Deed(s, champion, "dragon", $"ejderha {Tr.Ek(d.Name, "in")} düştüğü savaşın kahramanı oldu; ozanlar zaferi ona yazdı", cp.Tile, d.Name);
            if (!J.T(hero.Epithet))
            {
                hero.Epithet = "Ejderhakıran";
                s.Metric("epithet");
                Lore.Deed(s, hero, "epithet", "«Ejderhakıran» diye anılır oldu", cp.Tile);
            }
            hero.Bonus.Atk++;
            Will.Note(s, hero, "ejderhanın hazinesinden ejder ateşinde dövülmüş bir kılıç aldı");
        }
        if (town != null)
        {
            // akında düştü: in boşalır, hazineyi şehrin askerleri taşır
            cp.Alive = false; cp.Count = 0; cp.ClearedDay = s.Day;
            w.Tiles[cp.Tile].Camp = null;
            d.Hoard = JsMath.Round(cp.Loot);
            s.Add(town, "gold", d.Hoard);
            s.Metric("campCleared");
            foreach (var q in w.Quests)
                if (q.Camp == cp.Id && q.Open)
                {
                    q.Open = false;
                    if (q.Civ >= 0) s.Add(w.Civs[q.Civ], "gold", q.Bounty); else { var inn = Will.InnById(s, q.Inn); if (inn != null) inn.Gold += q.Bounty; }
                }
        }
        d.SlainDay = s.Day;
        d.SlainBy = hero != null ? hero.Name : who;
        d.Alliance.Clear(); d.Marches.Clear();
        s.Metric("dragonSlain");
        if (champion != null) s.Metric("dragonChampion");
        string credit = champion != null ? $", ozanlar zaferi {Lore.Ek(champion.Name, "a")} yazdı" : "";
        string blow = slayer != null ? $"; son darbeyi {Will.HeroLabel(slayer)} indirdi"
            : killer?.Hero != null ? $"; son darbeyi indiren {killer.Hero.Name} {(Tr.Ek(killer.Hero.Name, "da").EndsWith("a") ? "da" : "de")} ejderhayla birlikte can verdi"   // bağlaç "da/de" ünlü uyumuyla
            : killer != null && (killer.Kind == "soldier" || killer.Kind == "unique") ? $"; son darbeyi adı bilinmeyen bir asker indirdi{credit}"
            : champion != null ? $"; ozanlar zaferi {Lore.Ek(champion.Name, "a")} yazdı" : "";
        double years = Math.Floor((s.Day - d.WakeDay) / Sim.YEAR);
        s.Log("dragon", $"Ejderha {d.Name} öldü! {who} {where} ejderhayı devirdi{lead}{blow}.", tile: cp.Tile, civ: town?.Id ?? (b?.CivA), battle: b?.Id, major: true,
            cause: $"{(d.Raids == 0 ? "Daha tek bir akın yapamadan devrildi" : $"{(years >= 1 ? $"{J.S(years)} yıl" : "Bir yıl dolmadan")} korku saldı, {J.S(d.Raids)} kez akın etti")}; {J.S(d.Hoard)} altınlık hazinesi {(town != null ? $"{Tr.Ek(town.Name, "a")} kaldı" : "paylaşıldı")}");
        if (hero != null) Heroes.AddRenown(s, hero, R_SLAYER, "dragon");   // efsane olur (ozanların şarkısı ölüm haberinin ardından)
        foreach (var c in w.Civs) if (c.Alive && J.Some(s.CivSettlements(c), x => s.G.Dist(x.Tile, cp.Tile) <= RANGE)) c.Threat *= 0.3;
        Storyteller.StartRelief(s, Storyteller.State(s));
    }
}
