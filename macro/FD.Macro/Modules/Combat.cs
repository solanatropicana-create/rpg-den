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
    /// <summary>Faz 1 B2: tur başı yenilenme (trol), nefes silahı zarı ve hedef sayısı (ejderha)</summary>
    public double? Regen, Breath, BreathN;

    /// <summary>TS passes <c>UNITS.x</c> (UnitDef) where a UnitStats is expected (structural typing): same fields, Dmg list shared.</summary>
    public static implicit operator UnitStats(UnitDef d) =>
        d == null ? null : new UnitStats { Name = d.Name, Hp = d.Hp, Ac = d.Ac, Atk = d.Atk, Dmg = d.Dmg, Attacks = d.Attacks };

    /// <summary>TS passes <c>MONSTERS.x</c> (MonsterDef) where a UnitStats is expected: same fields, Dmg list shared.</summary>
    public static implicit operator UnitStats(MonsterDef d) =>
        d == null ? null : new UnitStats { Name = d.Name, Hp = d.Hp, Ac = d.Ac, Atk = d.Atk, Dmg = d.Dmg, Attacks = d.Attacks, Regen = d.Regen, Breath = d.Breath, BreathN = d.BreathN };
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
    /// <summary>bu savaşanı yere seren (Faz 1 A3a: katil ve önder öldürme kaydı); meteorla düşende null</summary>
    public Combatant KilledBy;
    /// <summary>Faz 1 B2: tur başında kapanan yara (trol); ateş yarası (Burned) bir sonraki tur başında yenilenmeyi durdurur</summary>
    public double? Regen;
    public bool? Burned;
    /// <summary>Faz 1 B2: nefes silahı (ejderha): zar sayısı (d6), en çok hedef, hazır mı (her tur 1/3 olasılıkla dolar)</summary>
    public double? Breath, BreathN;
    public bool? BreathReady;

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
        var c = new Combatant { Name = u.Name, Side = side, Hp = hp, MaxHp = hp, Ac = u.Ac, Atk = u.Atk + bonusAtk, Dmg = new List<double>(u.Dmg), Attacks = u.Attacks ?? 1, Kind = kind, Uses = new JsObj<double>(), Kills = 0, Boss = kind == "boss" };
        // Faz 1 B2: trol yenilenmesi, ejderha nefesi (savaşa nefesi dolu girer)
        if ((u.Regen ?? 0) > 0) c.Regen = u.Regen;
        if ((u.Breath ?? 0) > 0) { c.Breath = u.Breath; c.BreathN = u.BreathN ?? 6; c.BreathReady = true; }
        return c;
    }

    /// <summary>D&amp;D yeterlilik bonusu: Sv1–4 2, Sv5–8 3, Sv9–10 4.</summary>
    public static double ProfBonus(int level) => level >= 9 ? 4 : level >= 5 ? 3 : 2;

    /// <summary>Kahramanın savaş hâli: sınıf zarı, birincil stat modu, ek saldırı, özel yetenek hakları (uses).
    /// Sv5: ateş topu / iki saldırı. Sv9: sınıfa göre ölçülü bir güç (<see cref="Sv9Power"/>).</summary>
    public static Combatant HeroCombatant(Hero h, string side)
    {
        var def = D.HERO_CLASSES[h.Cls];
        double pm = Rng.Mod(J.N(h.Stats.Get(def.Primary)));
        bool sv9 = h.Level >= 9;
        double atk = ProfBonus(h.Level) + pm + (h.Bonus?.Atk ?? 0) + (sv9 && (h.Cls == "rogue" || h.Cls == "ranger" || h.Cls == "barbarian") ? 1 : 0);
        var dmg = new List<double> { def.Dmg[0], def.Dmg[1], pm };
        if (h.Cls == "wizard") dmg = new List<double> { h.Level >= 5 ? 2 : 1, 10, 0 };
        return new Combatant
        {
            Name = h.Name, Side = side, Hp = h.Hp, MaxHp = h.MaxHp, Ac = h.Ac, Atk = atk, Dmg = dmg,
            Attacks = (h.Cls == "fighter" || h.Cls == "paladin" || h.Cls == "barbarian" || h.Cls == "ranger") && h.Level >= 5 ? 2 : 1,
            Hero = h, Kind = "hero",
            Uses = new JsObj<double>
            {
                ["secondWind"] = sv9 && h.Cls == "fighter" ? 2 : 1, ["burning"] = 1, ["fireball"] = h.Level >= 5 ? (sv9 ? 2 : 1) : 0,
                ["cure"] = sv9 && (h.Cls == "cleric" || h.Cls == "druid") ? 3 : 2, ["smite"] = sv9 && h.Cls == "paladin" ? 3 : 2, ["wild"] = 1, ["rage"] = 1,
            },
            Kills = 0,
        };
    }

    /// <summary>Sv9'da kazanılan sınıf gücünün adı (seviye atlama satırı için).</summary>
    public static string Sv9Power(string cls) => cls switch
    {
        "fighter" => "Artık savaşta iki kez derin nefes alabiliyor (Second Wind ×2).",
        "wizard" => "Artık bir savaşta iki ateş topu patlatabiliyor.",
        "cleric" => "Artık bir savaşta üç kez yara sarabiliyor.",
        "druid" => "Artık bir savaşta üç kez yara sarabiliyor.",
        "paladin" => "Artık bir savaşta üç kez İlahi Çarpış indirebiliyor.",
        _ => "Artık rakiplerini daha kolay vuruyor (+1 saldırı).",
    };

    /// <summary>Kahraman zırh sınıfı (sınıf, dex modu, seviye).</summary>
    public static double HeroAc(string cls, double dex, int level)
    {
        switch (cls)
        {
            case "fighter": return 17 + (level >= 3 ? 1 : 0) + (level >= 7 ? 1 : 0);
            case "paladin": return 18 + (level >= 7 ? 1 : 0);
            case "cleric": return 16 + (level >= 7 ? 1 : 0);
            case "rogue": return 11 + dex + (level >= 7 ? 1 : 0);
            case "ranger": return 13 + JsMath.Min(2, dex) + (level >= 7 ? 1 : 0);
            case "druid": return 13 + JsMath.Min(2, dex) + (level >= 7 ? 1 : 0);
            case "barbarian": return 12 + dex + (level >= 7 ? 1 : 0);
            default: return 13 + dex + (level >= 7 ? 1 : 0); // wizard: mage armor
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
            // Faz 1 B2: nefes silahı (ortalama üç turda bir, en çok 6 hedef; kurtarma zarı ve boşa giden hasar payı)
            // ve yenilenme (savaş boyunca kapanan yaraların karşılığı etkin can)
            if ((c.Breath ?? 0) > 0) dpr += c.Breath.Value * 3.5 * 0.82 * JsMath.Min(c.BreathN ?? 6, 6) / 3 * 0.5;
            if ((c.Regen ?? 0) > 0) ehp += c.Regen.Value * 4 * (1 + (c.Ac - 12) * 0.08);
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

    /// <summary>Faz 1 B2: nefes silahına karşı kurtarma zarı (Dex) olasılığı: askerler %30; kahramanlar %35 + Dex modu × %5,
    /// hırsız ve korucu (kaçınma) +%15; en çok %75.</summary>
    public static double BreathSave(Combatant t)
    {
        if (t.Hero == null) return 0.3;
        double p = 0.35 + Rng.Mod(J.N(t.Hero.Stats.Get("dex"))) * 0.05 + (t.Hero.Cls == "rogue" || t.Hero.Cls == "ranger" ? 0.15 : 0);
        return JsMath.Max(0.1, JsMath.Min(0.75, p));
    }

    /// <summary>Faz 2: tek saldırının bağlamı. <see cref="Sneak"/>: haydudun sinsi saldırısına izin (tur tabanlı savaşta yanında bir dost
    /// var; bölgedeki gerçek zamanlı savaşta hedefin yanında bir dost ya da arkadan saldırı ve uygun silah); <see cref="Mul"/>: ilk tur
    /// baskın çarpanı; <see cref="Adv"/>: avantaj (iki d20'nin büyüğü; bölge: uyuyan, baygın hedef) ve <see cref="Dis"/> dezavantaj.</summary>
    public sealed class StrikeCtx
    {
        public bool Sneak;
        /// <summary>ilk tur çarpanı: verilirse hasar ×Mul yuvarlanır (1 de olsa yuvarlanır, eski kuralla aynı); null: çarpan yok</summary>
        public double? Mul;
        public bool Adv, Dis;
        /// <summary>bölge: yere düşmüş hedefe yakından vuruş kritik sayılır (D&amp;D: baygına 1,5 m içinden isabet kritiktir)</summary>
        public bool AutoCritOnHit;
    }

    /// <summary>Faz 2: tek saldırının sonucu (zar, isabet, kritik, hasar zarları, ekler).</summary>
    public sealed class StrikeResult
    {
        public double D20, D20b;
        public bool Hit, Crit, Fumble, Half;
        public List<double> Dice;
        public double Dmg, Smite, Bonus;
        public double? Mul;
        public List<ReplayPart> Parts = new();
        public bool Killed;
    }

    /// <summary>
    /// Faz 2: tek saldırının ortak kuralı (simetri): d20 (avantaj/dezavantajla iki zar), doğal 1 ıska (fumble), kritik (doğal 20;
    /// Sv3+ savaşçı 19–20), isabet = d20 + saldırı ≥ zırh sınıfı, hasar zarları (kritikte iki kat; kötülere İlahi Çarpış çarpanı),
    /// öfke +2, haydudun sinsi saldırısı (bağlam izin verirse ⌈Sv/2⌉d6, kritikte iki kat), korucunun av işareti 1d6, paladinin İlahi
    /// Çarpışı 2d8, ilk tur çarpanı, öfkeli hedefe yarı hasar, en az 1. Hasarı uygular; hedef düşerse vuranın öldürme sayısı ve
    /// katil kaydı. Zar sırası eski tur tabanlı savaşla birebir (ölçüm hash'leri değişmez).
    /// </summary>
    public static StrikeResult Strike(Rng rng, Combatant c, Combatant target, StrikeCtx ctx)
    {
        var r = new StrikeResult();
        double d20 = rng.D20();
        if (ctx.Adv || ctx.Dis)
        {
            r.D20b = rng.D20();
            if (ctx.Adv && !ctx.Dis) d20 = JsMath.Max(d20, r.D20b);
            else if (ctx.Dis && !ctx.Adv) d20 = JsMath.Min(d20, r.D20b);
        }
        r.D20 = d20;
        if (d20 == 1) { r.Fumble = true; return r; }
        double critRange = c.Hero?.Cls == "fighter" && c.Hero.Level >= 3 ? 19 : 20;
        bool crit = d20 >= critRange;
        if (!crit && d20 + c.Atk < target.Ac) return r;
        if (!crit && ctx.AutoCritOnHit) crit = true;
        r.Hit = true; r.Crit = crit;
        double n = c.Dmg[0];
        if (crit) n *= (J.T(target.Evil) && J.T(c.Smite) ? JsMath.Max(2, JsMath.Round(c.Smite.Value)) : 2);
        var dd = rng.Roll(n, c.Dmg[1]);
        double dmg = SumOf(dd) + c.Dmg[2];
        r.Dice = dd;
        if (J.T(c.Rage)) { dmg += 2; r.Parts.Add(new ReplayPart { L = "Öfke", V = 2 }); }
        if (c.Hero?.Cls == "rogue" && ctx.Sneak) { var rr = rng.Roll(Math.Ceiling(c.Hero.Level / 2.0) * (crit ? 2 : 1), 6); double v = SumOf(rr); dmg += v; r.Parts.Add(new ReplayPart { L = "Sinsi saldırı", V = v, Dd = rr, Ds = 6 }); }
        if (c.Hero?.Cls == "ranger" && c.Hero.Level >= 2) { var rr = rng.Roll(1, 6); dmg += rr[0]; r.Parts.Add(new ReplayPart { L = "Av işareti", V = rr[0], Dd = rr, Ds = 6 }); }
        if (c.Hero?.Cls == "paladin" && J.N(c.Uses.Get("smite")) > 0 && (J.T(target.Evil) || J.T(target.Boss) || crit))
        {
            c.Uses.Set("smite", J.N(c.Uses.Get("smite")) - 1); var rr = rng.Roll(2, 8); double sm = rr[0] + rr[1]; dmg += sm;
            r.Parts.Add(new ReplayPart { L = "İlahi çarpış", V = sm, Dd = rr, Ds = 8 });
            r.Smite = sm;
        }
        if (ctx.Mul is double fs1) { if (fs1 != 1) r.Mul = fs1; dmg = JsMath.Round(dmg * fs1); }
        bool half = J.T(target.Rage);
        if (half) dmg = Math.Ceiling(dmg / 2);
        r.Half = half;
        dmg = JsMath.Max(1, dmg);
        target.Hp -= dmg;
        r.Dmg = dmg;
        if (target.Hp <= 0)
        {
            c.Kills++;
            target.KilledBy = c;
            r.Killed = true;
        }
        return r;
    }

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
            foreach (var f in foes) { var r = rng.Roll(4, 6); double d = SumOf(r); f.Hp -= d; if (f.Regen != null) f.Burned = true; ts.Add(idx[f]); vs.Add(d); dds.Add(r); if (f.Hp <= 0) killed++; }
            ev.Add(new ReplayEv { R = 0, Sp = "meteor", A = -1, Ts = ts, Vs = vs, Dds = dds, Ds = 6, D = side == "A" ? 0 : 1 });
            lines.Add(new BattleLine { T = $"Gökten meteorlar yağdı! {killed} düşman ilk anda düştü.", Crit = true });
        }

        bool regenLogged = false;
        for (int round = 1; round <= maxRounds && routed == null; round++)
        {
            rounds = round;
            ev.Add(new ReplayEv { R = round, Sp = "round", AA = Alive("A").Count, AB = Alive("B").Count });
            // Faz 1 B2: tur başında troller yaralarını kapatır (bir önceki turda ateşle yananlar kapatamaz),
            // ejderhanın nefesi 1/3 olasılıkla yeniden dolar (D&D'deki 5–6 yenilenmesi)
            for (int ri = 0; ri < all.Count; ri++)
            {
                var u = all[ri];
                if (u.Hp <= 0 || J.T(u.Fled)) continue;
                if ((u.Regen ?? 0) > 0)
                {
                    if (J.T(u.Burned)) u.Burned = null;
                    else if (u.Hp < u.MaxHp)
                    {
                        double v = JsMath.Min(u.Regen.Value, u.MaxHp - u.Hp);
                        u.Hp += v;
                        ev.Add(new ReplayEv { R = round, Sp = "regen", A = ri, T = ri, V = v, Hp = u.Hp });
                        if (!regenLogged) { regenLogged = true; lines.Add(new BattleLine { T = $"{u.Name} yaralarını kapatıyor; ateş değmeyen yara iz bırakmıyor." }); }
                    }
                }
                if ((u.Breath ?? 0) > 0 && !J.T(u.BreathReady) && round > 1 && rng.Chance(1.0 / 3)) u.BreathReady = true;
            }
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

                // Faz 1 B2: ejderha nefesi: hazırsa saldırı yerine rastgele en çok BreathN düşmana alev
                // (kurtarma zarını tutan yarı hasar alır; ateş trolün yenilenmesini durdurur)
                if (J.T(c.BreathReady) && c.Hero == null)
                {
                    c.BreathReady = false;
                    var targets = J.Slice(rng.Shuffle(J.Slice(foes)), 0, (int)(c.BreathN ?? 6));
                    int killed = 0;
                    var ts = new List<int>(); var vs = new List<double>(); var sv = new List<bool>(); var dds = new List<List<double>>();
                    foreach (var t in targets)
                    {
                        var r = rng.Roll(c.Breath ?? 0, 6);
                        dds.Add(r);
                        double d = SumOf(r);
                        bool save = rng.Chance(BreathSave(t));
                        if (save) d = Math.Floor(d / 2);
                        t.Hp -= d;
                        if (t.Regen != null) t.Burned = true;
                        ts.Add(idx[t]); vs.Add(d); sv.Add(save);
                        if (t.Hp <= 0) { killed++; c.Kills++; t.KilledBy = c; }
                    }
                    ev.Add(new ReplayEv { R = round, Sp = "breath", A = ci, Ts = ts, Vs = vs, Sv = sv, Dds = dds, Ds = 6 });
                    lines.Add(new BattleLine { T = $"{c.Name} ateş soludu! {targets.Count} hedef alevlere boğuldu, {killed} ölü.", Crit = true });
                    continue;
                }

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
                            if (t.Regen != null) t.Burned = true;   // Faz 1 B2: ateş trolün yenilenmesini durdurur
                            ts.Add(idx[t]); vs.Add(d); sv.Add(save);
                            if (t.Hp <= 0) { killed++; c.Kills++; t.KilledBy = c; }
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
                    // Faz 2: tek saldırının kuralı ortak (Strike): bölgedeki gerçek zamanlı savaş da aynısını kullanır
                    var r = Strike(rng, c, target, new StrikeCtx { Sneak = friends.Count > 1, Mul = round == 1 ? (c.Side == "A" ? o.FirstStrikeA : o.FirstStrikeB) ?? 1 : (double?)null });
                    double d20 = r.D20;
                    bool important = c.Hero != null || target.Hero != null || J.T(c.Boss);
                    if (rolls.Count < 40 || (important && rolls.Count < 70) || ((d20 == 20 || d20 == 1) && rolls.Count < 80)) rolls.Add(new BattleRoll { D20 = d20, Side = c.Side, Who = c.Name });
                    if (r.Fumble)
                    {
                        ev.Add(new ReplayEv { R = round, Sp = "attack", A = ci, T = ti, D = 1, M = c.Atk, Ac = target.Ac, H = 0 });
                        if (c.Hero != null && rng.Chance(0.5)) lines.Add(new BattleLine { T = $"{c.Name} nat 1! Silahı elinden kaydı.", Fumble = true });
                        continue;
                    }
                    if (!r.Hit) { ev.Add(new ReplayEv { R = round, Sp = "attack", A = ci, T = ti, D = d20, M = c.Atk, Ac = target.Ac, H = 0 }); continue; }
                    bool crit = r.Crit;
                    double dmg = r.Dmg;
                    if (r.Smite > 0) lines.Add(new BattleLine { T = $"{c.Name} İlahi Çarpış indirdi (+{J.S(r.Smite)})!", Crit = true });
                    var e = new ReplayEv { R = round, Sp = "attack", A = ci, T = ti, D = d20, M = c.Atk, Ac = target.Ac, H = crit ? 2 : 1, Dd = r.Dice, Ds = c.Dmg[1], B = c.Dmg[2], V = dmg, Hp = target.Hp };
                    if (r.Parts.Count > 0) e.X = r.Parts;
                    if (J.T(r.Mul)) e.Mul = r.Mul;
                    if (r.Half) e.Half = true;
                    ev.Add(e);
                    if (crit && (c.Hero != null || target.Hero != null || J.T(target.Boss))) lines.Add(new BattleLine { T = $"{c.Name} nat 20! {target.Name} {J.S(dmg)} hasar yedi.", Crit = true });
                    if (target.Hp <= 0)
                    {
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
