using System;
using System.Collections.Generic;
using System.Linq;

// Savaş çözümü: birimler, güç tahmini, tur tur çarpışma ve savaş tekrarı. Port of src/sim/combat.ts.

namespace FD.Macro;

/// <summary>TS <c>UnitStats</c>: stat block of a unit / monster (UNITS.*, MONSTERS.*, GALLEY, COG, inline literals).</summary>
public sealed class UnitStats
{
    public string Name;
    public double Hp;
    public double Ac;
    public double Atk;
    public List<double> Dmg;     // [n, sides, bonus]
    public double? Attacks;

    /// <summary>TS passes <c>UNITS.x</c> (UnitDef) where a UnitStats is expected (structural typing): same fields, Dmg list shared.</summary>
    public static implicit operator UnitStats(UnitDef d) =>
        d == null ? null : new UnitStats { Name = d.Name, Hp = d.Hp, Ac = d.Ac, Atk = d.Atk, Dmg = d.Dmg, Attacks = d.Attacks };

    /// <summary>TS passes <c>MONSTERS.x</c> (MonsterDef) where a UnitStats is expected: same fields, Dmg list shared.</summary>
    public static implicit operator UnitStats(MonsterDef d) =>
        d == null ? null : new UnitStats { Name = d.Name, Hp = d.Hp, Ac = d.Ac, Atk = d.Atk, Dmg = d.Dmg, Attacks = d.Attacks };
}

/// <summary>TS <c>Combatant</c>: one fighter inside a battle (transient; not part of World).</summary>
public sealed class Combatant
{
    public string Name;
    public string Side;          // 'A' | 'B'
    public double Hp;
    public double MaxHp;
    public double Ac;
    public double Atk;
    public List<double> Dmg;     // [n, sides, bonus]
    public double Attacks;
    public Hero Hero;
    public bool? Boss;
    public string Kind;          // soldier | militia | monster | hero | ballista | unique
    public bool? Evil;           // kötü ya da canavar (Paladin çarpışı için)
    public double? Smite;        // kötülere karşı kritik çarpanı
    public bool? Fled;
    public JsObj<double> Uses = new();
    public double Kills;
    public bool? Rage;
    public double? Temp;
    public int? Grp;             // ortak saldırıda hangi gruba ait (Replay.groups dizini)

    /// <summary>TS <c>{ ...c }</c> (shallow copy: Dmg, Uses and Hero are shared like in JS).</summary>
    public Combatant Clone() => (Combatant)MemberwiseClone();
}

/// <summary>TS <c>BattleOpts</c>: options of <see cref="Combat.ResolveBattle"/>.</summary>
public sealed class BattleOpts
{
    public double Day;
    public int Tile;
    public string Title;
    public string SideA;
    public string SideB;
    public int Id;
    public double? MaxRounds;
    public double? MoraleA;
    public double? MoraleB;
    public bool? NoRoutA;
    public bool? NoRoutB;
    public double? FirstStrikeA;
    public double? FirstStrikeB;
    public bool? MeteorA;
    public bool? MeteorB;
    /// <summary>süre dolarsa kazanan (kuşatmada ve kamp baskınında savunan). Verilmezse kalan can toplamı.</summary>
    public string TimeoutWinner; // 'A' | 'B'
    public List<ReplayGroup> Groups;
    public int? CivA;
    public int? CivB;
}

public static class Combat
{
    /// <summary>kahraman yaşarken moral eşiği bu kadar yükselir (artık bozgunu tamamen engellemez)</summary>
    public const double HERO_MORALE = 0.15;

    /// <summary>ayrıntılı tekrarı tutulan son savaş sayısı</summary>
    public const int REPLAY_KEEP = 40;

    /// <summary>Stat bloğundan savaşan: hp = max(1, hp + bonusHp), atk + bonusAtk, dmg kopyalanır, boss = (kind == "boss").</summary>
    public static Combatant Unit(UnitStats u, string side, string kind, double bonusHp = 0, double bonusAtk = 0)
    {
        double hp = JsMath.Max(1, u.Hp + bonusHp);
        return new Combatant { Name = u.Name, Side = side, Hp = hp, MaxHp = hp, Ac = u.Ac, Atk = u.Atk + bonusAtk, Dmg = new List<double>(u.Dmg), Attacks = u.Attacks ?? 1, Kind = kind, Uses = new JsObj<double>(), Kills = 0, Boss = kind == "boss" };
    }

    /// <summary>D&amp;D yeterlilik bonusu: seviye 5'ten itibaren 3, yoksa 2.</summary>
    public static double ProfBonus(int level) => level >= 5 ? 3 : 2;

    /// <summary>Kahramanın savaş hâli: sınıf zarı, birincil stat modu, ek saldırı, özel yetenek hakları (uses).</summary>
    public static Combatant HeroCombatant(Hero h, string side)
    {
        var def = D.HERO_CLASSES[h.Cls];
        double pm = Rng.Mod(J.N(h.Stats.Get(def.Primary)));
        double atk = ProfBonus(h.Level) + pm + (h.Bonus?.Atk ?? 0);
        var dmg = new List<double> { def.Dmg[0], def.Dmg[1], pm };
        if (h.Cls == "wizard") dmg = new List<double> { h.Level >= 5 ? 2 : 1, 10, 0 };
        return new Combatant
        {
            Name = h.Name, Side = side, Hp = h.Hp, MaxHp = h.MaxHp, Ac = h.Ac, Atk = atk, Dmg = dmg,
            Attacks = (h.Cls == "fighter" || h.Cls == "paladin" || h.Cls == "barbarian" || h.Cls == "ranger") && h.Level >= 5 ? 2 : 1,
            Hero = h, Kind = "hero",
            Uses = new JsObj<double> { ["secondWind"] = 1, ["burning"] = 1, ["fireball"] = h.Level >= 5 ? 1 : 0, ["cure"] = 2, ["smite"] = 2, ["wild"] = 1, ["rage"] = 1 },
            Kills = 0,
        };
    }

    /// <summary>Kahraman zırh sınıfı (sınıf, dex modu, seviye).</summary>
    public static double HeroAc(string cls, double dex, int level)
    {
        switch (cls)
        {
            case "fighter": return 17 + (level >= 3 ? 1 : 0);
            case "paladin": return 18;
            case "cleric": return 16;
            case "rogue": return 11 + dex;
            case "ranger": return 13 + JsMath.Min(2, dex);
            case "druid": return 13 + JsMath.Min(2, dex);
            case "barbarian": return 12 + dex;
            default: return 13 + dex; // wizard: mage armor
        }
    }

    /// <summary>ortalama zırh sınıfı (güç tahmininde karşı tarafın isabet şansı için)</summary>
    public static double AvgAc(List<Combatant> cs)
    {
        if (cs.Count == 0) return 15;
        double a = 0;
        for (int i = 0; i < cs.Count; i++) a += cs[i].Ac;
        return a / cs.Count;
    }

    /// <summary>Lanchester tarzı güç tahmini. vsAc: karşı tarafın ortalama AC'si (bilinmiyorsa 15)</summary>
    public static double PowerOf(List<Combatant> cs, double vsAc = 15)
    {
        double dpr = 0, ehp = 0;
        foreach (var c in cs)
        {
            double avg = c.Dmg[0] * (c.Dmg[1] + 1) / 2 + c.Dmg[2];
            double hit = JsMath.Min(0.95, JsMath.Max(0.1, (21 - (vsAc - c.Atk)) / 20));
            double extra = 1;
            string hc = c.Hero?.Cls;
            if (hc == "wizard") extra = 1.8;
            if (hc == "cleric" || hc == "druid") extra = 1.3;
            if (hc == "rogue" || hc == "paladin" || hc == "barbarian") extra = 1.4;
            dpr += avg * hit * c.Attacks * extra;
            ehp += c.Hp * (1 + (c.Ac - 12) * 0.08) * (hc == "barbarian" ? 1.5 : 1);
        }
        return Math.Sqrt(dpr * ehp);
    }

    /// <summary>iki tarafın birbirine karşı gücü: [A, B]</summary>
    public static (double PowA, double PowB) PowerVs(List<Combatant> A, List<Combatant> B)
    {
        double pa = PowerOf(A, AvgAc(B));
        double pb = PowerOf(B, AvgAc(A));
        return (pa, pb);
    }

    /// <summary>savaş öncesi tahmini zafer şansı (A için). Simülasyondan ölçülmüş lojistik eğri: güç oranının logaritması</summary>
    public static double WinChance(double powA, double powB)
    {
        double x = JsMath.Log(JsMath.Max(0.01, powA) / JsMath.Max(0.01, powB));
        return 1 / (1 + JsMath.Exp(-(5.75 * x + 0.9)));
    }

    private static ReplayUnit ReplayUnitOf(Combatant c)
    {
        var u = new ReplayUnit { N = c.Name, S = c.Side, K = c.Kind, Hp = c.Hp, Max = c.MaxHp, Ac = c.Ac, Atk = c.Atk, Dmg = new List<double>(c.Dmg), Att = c.Attacks, G = c.Grp ?? (c.Side == "A" ? 0 : 1) };
        if (c.Hero != null) { u.Cls = c.Hero.Cls; u.Lvl = c.Hero.Level; u.Race = c.Hero.Race; }
        if (J.T(c.Boss)) u.Boss = true;
        return u;
    }

    /// <summary>JS <c>r.reduce((x, y) => x + y, 0)</c>.</summary>
    private static double SumOf(List<double> r)
    {
        double t = 0;
        for (int i = 0; i < r.Count; i++) t += r[i];
        return t;
    }

    /// <summary>JS <c>`${HERO_CLASS_TR[cls]}`</c> ("undefined" when missing).</summary>
    private static string ClassTr(string cls) => D.HERO_CLASS_TR.TryGet(cls, out var t) ? t : "undefined";

    /// <summary>Savaşı tur tur çözer (meteor, kahraman yetenekleri, moral/bozgun, süre dolması) ve tekrarı içeren Battle kaydını döndürür.</summary>
    public static Battle ResolveBattle(Rng rng, List<Combatant> A, List<Combatant> B, BattleOpts o)
    {
        var all = new List<Combatant>(A.Count + B.Count);
        all.AddRange(A);
        all.AddRange(B);
        var idx = new Dictionary<Combatant, int>(ReferenceEqualityComparer.Instance);
        for (int i = 0; i < all.Count; i++) idx[all[i]] = i;
        var lines = new List<BattleLine>();
        var rolls = new List<BattleRoll>();
        var ev = new List<ReplayEv>();
        var units = J.Map(all, ReplayUnitOf);
        var (powA, powB) = PowerVs(A, B);
        int initA = A.Count, initB = B.Count;
        List<Combatant> Alive(string sd) => J.Filter(all, c => c.Side == sd && c.Hp > 0 && !J.T(c.Fled));
        var heroLogged = new HashSet<string>();
        double maxRounds = o.MaxRounds ?? 15;
        string routed = null;
        double rounds = 0;

        foreach (var (side, on) in new (string, bool?)[] { ("A", o.MeteorA), ("B", o.MeteorB) })
        {
            if (!J.T(on)) continue;
            var foes = Alive(side == "A" ? "B" : "A");
            int killed = 0;
            var ts = new List<int>(); var vs = new List<double>(); var dds = new List<List<double>>();
            foreach (var f in foes) { var r = rng.Roll(4, 6); double d = SumOf(r); f.Hp -= d; ts.Add(idx[f]); vs.Add(d); dds.Add(r); if (f.Hp <= 0) killed++; }
            ev.Add(new ReplayEv { R = 0, Sp = "meteor", A = -1, Ts = ts, Vs = vs, Dds = dds, Ds = 6, D = side == "A" ? 0 : 1 });
            lines.Add(new BattleLine { T = $"Gökten meteorlar yağdı! {killed} düşman ilk anda düştü.", Crit = true });
        }

        for (int round = 1; round <= maxRounds && routed == null; round++)
        {
            rounds = round;
            ev.Add(new ReplayEv { R = round, Sp = "round", AA = Alive("A").Count, AB = Alive("B").Count });
            var order = rng.Shuffle(J.Filter(all, c => c.Hp > 0 && !J.T(c.Fled)));
            for (int oi = 0; oi < order.Count; oi++)
            {
                var c = order[oi];
                if (c.Hp <= 0 || J.T(c.Fled)) continue;
                int ci = idx[c];
                string foeSide = c.Side == "A" ? "B" : "A";
                var foes = Alive(foeSide);
                if (foes.Count == 0) break;
                var friends = Alive(c.Side);

                if (c.Hero != null)
                {
                    var h = c.Hero;
                    if (c.Hp < c.MaxHp * 0.25 && foes.Count > friends.Count * 1.5 && rng.Chance(0.5))
                    {
                        c.Fled = true;
                        ev.Add(new ReplayEv { R = round, Sp = "flee", A = ci });
                        lines.Add(new BattleLine { T = $"{h.Name} ağır yaralı halde savaş alanından kaçtı." });
                        continue;
                    }
                    if (h.Cls == "barbarian" && J.T(c.Uses.Get("rage"))) { c.Uses.Set("rage", 0); c.Rage = true; ev.Add(new ReplayEv { R = round, Sp = "rage", A = ci }); lines.Add(new BattleLine { T = $"{h.Name} öfkeye kapıldı!" }); }
                    if (h.Cls == "druid" && J.T(c.Uses.Get("wild")) && c.Hp < c.MaxHp * 0.5)
                    {
                        c.Uses.Set("wild", 0); double t = 10 + h.Level * 3; c.Hp += t;
                        ev.Add(new ReplayEv { R = round, Sp = "wild", A = ci, T = ci, V = t, Hp = c.Hp });
                        lines.Add(new BattleLine { T = $"{h.Name} bir ayıya dönüştü (Wild Shape, +{J.S(t)} can)." });
                    }
                    if (h.Cls == "fighter" && J.T(c.Uses.Get("secondWind")) && c.Hp < c.MaxHp * 0.5)
                    {
                        var dd = rng.Roll(1, 10);
                        double heal = dd[0] + h.Level;
                        c.Hp = JsMath.Min(c.MaxHp, c.Hp + heal); c.Uses.Set("secondWind", J.N(c.Uses.Get("secondWind")) - 1);
                        ev.Add(new ReplayEv { R = round, Sp = "second", A = ci, T = ci, Dd = dd, Ds = 10, B = h.Level, V = heal, Hp = c.Hp });
                        lines.Add(new BattleLine { T = $"{h.Name} derin bir nefes aldı (Second Wind) ve {J.S(heal)} can topladı." });
                    }
                    if ((h.Cls == "cleric" || h.Cls == "druid" || h.Cls == "paladin") && J.N(c.Uses.Get("cure")) > 0)
                    {
                        var hurt = J.At(J.Sort(J.Filter(friends, f => f.Hero != null && f.Hp < f.MaxHp * 0.45), (a, b) => a.Hp - b.Hp), 0);
                        if (hurt != null)
                        {
                            var dd = rng.Roll(1, 8);
                            double bonus = Rng.Mod(J.N(h.Stats.Get("wis"))) + (h.Cls == "paladin" ? h.Level * 2 : 0);
                            double heal = dd[0] + bonus;
                            hurt.Hp = JsMath.Min(hurt.MaxHp, hurt.Hp + heal); c.Uses.Set("cure", J.N(c.Uses.Get("cure")) - 1);
                            ev.Add(new ReplayEv { R = round, Sp = "heal", A = ci, T = idx[hurt], Dd = dd, Ds = 8, B = bonus, V = heal, Hp = hurt.Hp });
                            lines.Add(new BattleLine { T = $"{h.Name}, {(hurt == c ? "kendi" : Tr.Ek(hurt.Name, "in"))} yaralarını iyileştirdi (+{J.S(heal)})." });
                            continue;
                        }
                    }
                    if (h.Cls == "wizard" && foes.Count >= 3 && (J.N(c.Uses.Get("fireball")) > 0 || J.N(c.Uses.Get("burning")) > 0))
                    {
                        bool fire = J.N(c.Uses.Get("fireball")) > 0;
                        var targets = J.Slice(rng.Shuffle(J.Slice(foes)), 0, fire ? 6 : 3);
                        int killed = 0;
                        var ts = new List<int>(); var vs = new List<double>(); var sv = new List<bool>(); var dds = new List<List<double>>();
                        foreach (var t in targets)
                        {
                            var r = rng.Roll(fire ? 8 : 3, 6);
                            dds.Add(r);
                            double d = SumOf(r);
                            bool save = rng.Chance(0.35);
                            if (save) d = Math.Floor(d / 2);
                            t.Hp -= d;
                            ts.Add(idx[t]); vs.Add(d); sv.Add(save);
                            if (t.Hp <= 0) { killed++; c.Kills++; }
                        }
                        if (fire) c.Uses.Set("fireball", J.N(c.Uses.Get("fireball")) - 1); else c.Uses.Set("burning", J.N(c.Uses.Get("burning")) - 1);
                        ev.Add(new ReplayEv { R = round, Sp = fire ? "fireball" : "burning", A = ci, Ts = ts, Vs = vs, Sv = sv, Dds = dds, Ds = 6 });
                        lines.Add(new BattleLine { T = $"{h.Name} {(fire ? "bir ATEŞ TOPU patlattı" : "alevler püskürttü (Burning Hands)")}: {targets.Count} hedef, {killed} ölü!", Crit = fire });
                        continue;
                    }
                }

                for (int a = 0; a < c.Attacks; a++)
                {
                    var fs = Alive(foeSide);
                    if (fs.Count == 0) break;
                    Combatant target;
                    if (c.Hero != null) { var boss = J.Find(fs, f => J.T(f.Boss)); target = boss != null && rng.Chance(0.6) ? boss : rng.Pick(fs); }
                    else target = rng.Pick(fs);
                    int ti = idx[target];
                    double d20 = rng.D20();
                    bool important = c.Hero != null || target.Hero != null || J.T(c.Boss);
                    if (rolls.Count < 40 || (important && rolls.Count < 70) || ((d20 == 20 || d20 == 1) && rolls.Count < 80)) rolls.Add(new BattleRoll { D20 = d20, Side = c.Side, Who = c.Name });
                    if (d20 == 1)
                    {
                        ev.Add(new ReplayEv { R = round, Sp = "attack", A = ci, T = ti, D = 1, M = c.Atk, Ac = target.Ac, H = 0 });
                        if (c.Hero != null && rng.Chance(0.5)) lines.Add(new BattleLine { T = $"{c.Name} nat 1! Silahı elinden kaydı.", Fumble = true });
                        continue;
                    }
                    double critRange = c.Hero?.Cls == "fighter" && c.Hero.Level >= 3 ? 19 : 20;
                    bool crit = d20 >= critRange;
                    if (!crit && d20 + c.Atk < target.Ac) { ev.Add(new ReplayEv { R = round, Sp = "attack", A = ci, T = ti, D = d20, M = c.Atk, Ac = target.Ac, H = 0 }); continue; }
                    double n = c.Dmg[0];
                    if (crit) n *= (J.T(target.Evil) && J.T(c.Smite) ? JsMath.Max(2, JsMath.Round(c.Smite.Value)) : 2);
                    var dd = rng.Roll(n, c.Dmg[1]);
                    double dmg = SumOf(dd) + c.Dmg[2];
                    var x = new List<ReplayPart>();
                    if (J.T(c.Rage)) { dmg += 2; x.Add(new ReplayPart { L = "Öfke", V = 2 }); }
                    if (c.Hero?.Cls == "rogue" && friends.Count > 1) { var r = rng.Roll(Math.Ceiling(c.Hero.Level / 2.0) * (crit ? 2 : 1), 6); double v = SumOf(r); dmg += v; x.Add(new ReplayPart { L = "Sinsi saldırı", V = v, Dd = r, Ds = 6 }); }
                    if (c.Hero?.Cls == "ranger" && c.Hero.Level >= 2) { var r = rng.Roll(1, 6); dmg += r[0]; x.Add(new ReplayPart { L = "Av işareti", V = r[0], Dd = r, Ds = 6 }); }
                    if (c.Hero?.Cls == "paladin" && J.N(c.Uses.Get("smite")) > 0 && (J.T(target.Evil) || J.T(target.Boss) || crit))
                    {
                        c.Uses.Set("smite", J.N(c.Uses.Get("smite")) - 1); var r = rng.Roll(2, 8); double sm = r[0] + r[1]; dmg += sm;
                        x.Add(new ReplayPart { L = "İlahi çarpış", V = sm, Dd = r, Ds = 8 });
                        lines.Add(new BattleLine { T = $"{c.Name} İlahi Çarpış indirdi (+{J.S(sm)})!", Crit = true });
                    }
                    double? mul = null;
                    if (round == 1) { double fs1 = (c.Side == "A" ? o.FirstStrikeA : o.FirstStrikeB) ?? 1; if (fs1 != 1) mul = fs1; dmg = JsMath.Round(dmg * fs1); }
                    bool half = J.T(target.Rage);
                    if (half) dmg = Math.Ceiling(dmg / 2);
                    dmg = JsMath.Max(1, dmg);
                    target.Hp -= dmg;
                    var e = new ReplayEv { R = round, Sp = "attack", A = ci, T = ti, D = d20, M = c.Atk, Ac = target.Ac, H = crit ? 2 : 1, Dd = dd, Ds = c.Dmg[1], B = c.Dmg[2], V = dmg, Hp = target.Hp };
                    if (x.Count > 0) e.X = x;
                    if (J.T(mul)) e.Mul = mul;
                    if (half) e.Half = true;
                    ev.Add(e);
                    if (crit && (c.Hero != null || target.Hero != null || J.T(target.Boss))) lines.Add(new BattleLine { T = $"{c.Name} nat 20! {target.Name} {J.S(dmg)} hasar yedi.", Crit = true });
                    if (target.Hp <= 0)
                    {
                        c.Kills++;
                        if (target.Hero != null) lines.Add(new BattleLine { T = $"{target.Name} ({ClassTr(target.Hero.Cls)}) {c.Name} tarafından yere serildi!", Crit = true });
                        else if (J.T(target.Boss)) lines.Add(new BattleLine { T = $"{c.Name}, {Tr.Ek(target.Name, "i")} devirdi!", Crit = true });
                        else if (c.Hero != null && !heroLogged.Contains(c.Name)) { heroLogged.Add(c.Name); lines.Add(new BattleLine { T = $"{c.Name} ilk {J.TrLower(target.Name)} kurbanını aldı." }); }
                    }
                }
            }
            int aAn = Alive("A").Count, aBn = Alive("B").Count;
            if (aAn == 0 || aBn == 0) break;
            double lossA = 1 - aAn / (double)initA, lossB = 1 - aBn / (double)initB;
            bool heroA = J.Some(Alive("A"), c => c.Hero != null), heroB = J.Some(Alive("B"), c => c.Hero != null);
            // kahraman bozgunu engellemez, eşiği yükseltir
            if (!J.T(o.NoRoutA) && lossA >= JsMath.Min(0.95, (o.MoraleA ?? 0.6) + (heroA ? HERO_MORALE : 0))) routed = "A";
            else if (!J.T(o.NoRoutB) && lossB >= JsMath.Min(0.95, (o.MoraleB ?? 0.6) + (heroB ? HERO_MORALE : 0))) routed = "B";
            if (routed != null)
            {
                ev.Add(new ReplayEv { R = round, Sp = "rout", D = routed == "A" ? 0 : 1 });
                lines.Add(new BattleLine { T = $"{(routed == "A" ? o.SideA : o.SideB)} bozguna uğrayıp kaçtı." });
            }
        }

        var aA = Alive("A"); var aB = Alive("B");
        string winner;
        string end;
        if (routed != null) { winner = routed == "A" ? "B" : "A"; end = "rout"; }
        else if (aA.Count == 0) { winner = "B"; end = "wipe"; }
        else if (aB.Count == 0) { winner = "A"; end = "wipe"; }
        else
        {
            end = "timeout";
            winner = o.TimeoutWinner ?? (J.Sum(aA, c => c.Hp) >= J.Sum(aB, c => c.Hp) ? "A" : "B");
            lines.Add(new BattleLine { T = J.T(o.TimeoutWinner) ? $"Gün battı; {(winner == "A" ? o.SideA : o.SideB)} mevzisini korudu." : $"Gün battı; savaş alanı {(winner == "A" ? o.SideA : o.SideB)} tarafında kaldı." });
        }
        var groups = o.Groups ?? new List<ReplayGroup> { new ReplayGroup { Name = o.SideA, Side = "A", Civ = o.CivA }, new ReplayGroup { Name = o.SideB, Side = "B", Civ = o.CivB } };
        var replay = new Replay
        {
            Units = units, Ev = ev, Groups = groups, MoraleA = o.MoraleA ?? 0.6, MoraleB = o.MoraleB ?? 0.6, HeroMorale = HERO_MORALE, NoRoutA = o.NoRoutA, NoRoutB = o.NoRoutB,
            PowA = JsMath.Round(powA * 10) / 10, PowB = JsMath.Round(powB * 10) / 10, Rounds = rounds, MaxRounds = maxRounds, End = end, Routed = routed, TimeoutWinner = o.TimeoutWinner,
        };
        var bt = new Battle
        {
            Id = o.Id, Day = o.Day, Tile = o.Tile, Title = o.Title, SideA = o.SideA, SideB = o.SideB, Winner = winner,
            LossesA = J.Filter(A, c => c.Hp <= 0).Count, LossesB = J.Filter(B, c => c.Hp <= 0).Count, Lines = lines, Rolls = rolls, Replay = replay,
        };
        if (o.CivA != null) bt.CivA = o.CivA;
        if (o.CivB != null) bt.CivB = o.CivB;
        if (o.Groups != null && J.Filter(o.Groups, g => g.Side == "A").Count > 1) bt.Joint = true;
        return bt;
    }
}
