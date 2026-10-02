using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;
using FD.Rpg;
using V2 = System.Numerics.Vector2;
using M = FD.Macro;

namespace FD.Dev;

/// <summary>
/// Faz 2 (--fighttest): headless checks of the real-time d20 fight. (1) The shared attack rule (Combat.Strike): the d20 is flat,
/// natural 20 crits and natural 1 misses come ~5 % each, hit chance follows d20 + attack vs AC. (2) Class behaviour in many
/// simulated goblin fights: the fighter fights in melee, the wizard keeps its distance and casts, the rogue gets sneak attacks, the
/// cleric raises the fallen, goblins break and run at their morale, fights end, the same seed gives the same fight. PASS/FAIL.
/// </summary>
public static class FightTest
{
    public static void Run(Node host)
    {
        bool ok = true;
        var sb = new StringBuilder();
        void Check(string name, bool pass, string info) { ok &= pass; sb.AppendLine($"[FightTest] {name}: {info} {(pass ? "PASS" : "FAIL")}"); }

        // (1) the rule
        var rng = new M.Rng(12345);
        var counts = new int[21];
        int crit = 0, fumble = 0, hits = 0, N = 40000;
        for (int i = 0; i < N; i++)
        {
            var a = M.Combat.Unit(M.D.MONSTERS["goblin"], "A", "monster");
            var b = M.Combat.Unit(M.D.MONSTERS["goblin"], "B", "monster");
            b.Hp = 1000; b.MaxHp = 1000;
            var r = M.Combat.Strike(rng, a, b, new M.Combat.StrikeCtx());
            counts[(int)r.D20]++;
            if (r.Crit) crit++;
            if (r.Fumble) fumble++;
            if (r.Hit) hits++;
        }
        double exp = N / 20.0, chi = 0;
        for (int k = 1; k <= 20; k++) chi += (counts[k] - exp) * (counts[k] - exp) / exp;
        Check("d20 düz", chi < 43.8, $"ki-kare {chi:F1} (19 sd, %0,1 sınırı 43,8); 1: {counts[1]}, 20: {counts[20]}");
        Check("kritik ve ıska", Math.Abs(crit / (double)N - 0.05) < 0.006 && Math.Abs(fumble / (double)N - 0.05) < 0.006, $"doğal 20 %{100.0 * crit / N:F2}, doğal 1 %{100.0 * fumble / N:F2}");
        // goblin +4 vs AC 15: hit on 11+ → 50 %
        Check("isabet olasılığı", Math.Abs(hits / (double)N - 0.5) < 0.012, $"+4 vs ZS 15: %{100.0 * hits / N:F1} (beklenen %50)");

        // (2) fights
        string[] parties = { "fighter+fighter", "fighter+wizard", "rogue+fighter", "cleric+fighter", "wizard+fighter" };
        foreach (var comp in parties)
        {
            var cls = comp.Split('+');
            int wins = 0, routs = 0, n = 60, ended = 0, sneaks = 0, raises = 0, spells = 0;
            float wizDist = 0; int wizSamples = 0; float meleeShare = 0; int meleeSamples = 0;
            float secs = 0;
            for (int s = 0; s < n; s++)
            {
                var fight = Make(cls, 5, s * 7919 + 13, out var lead);
                float t = 0;
                while (!fight.Over && t < 240f)
                {
                    fight.Update(0.05f); t += 0.05f;
                    foreach (var f in fight.F)
                    {
                        if (f.Side != FSide.Party || !f.Standing || f.Char == null) continue;
                        var near = fight.F.Where(o => o.Side == FSide.Foe && o.Standing && !o.Fleeing).OrderBy(o => V2.Distance(o.Pos, f.Pos)).FirstOrDefault();
                        if (near == null) continue;
                        float d = V2.Distance(near.Pos, f.Pos);
                        if (f.Char.Cls == "wizard") { wizDist += d; wizSamples++; }
                        if (f.Char.Cls == "fighter") { meleeShare += d < 2.2f ? 1 : 0; meleeSamples++; }
                    }
                }
                secs += t;
                if (fight.Over) ended++;
                if (fight.Winner == FSide.Party) wins++;
                if (fight.Log.Any(e => e.Kind == "rout")) routs++;
                sneaks += fight.Log.Count(e => e.Kind == "attack" && e.Detail != null && e.Detail.Contains("sinsi"));
                raises += fight.Log.Count(e => e.Kind == "up" && e.B != null);
                spells += fight.Log.Count(e => e.Kind == "spell");
            }
            string extra = "";
            bool pass = ended == n;
            if (cls.Contains("wizard")) { float wd = wizDist / Math.Max(1, wizSamples); extra += $", büyücü en yakın goblinden ort {wd:F1} m, büyü {spells}"; pass &= wd > 4f && spells > n; }
            if (cls.Contains("rogue")) { extra += $", sinsi saldırı {sneaks}"; pass &= sneaks > n / 2; }
            if (cls.Contains("cleric")) { extra += $", rahibin kaldırdığı {raises}, büyü {spells}"; pass &= spells > n / 2; }
            if (cls[0] == "fighter") { float ms = meleeShare / Math.Max(1, meleeSamples); extra += $", savaşçı yakın dövüşte %{ms * 100:F0}"; pass &= ms > 0.4f; }
            Check($"{comp} (Sv1+Sv2) vs 5 goblin", pass, $"{n} savaş: kazanılan {wins}, bozgun {routs}, biten {ended}, ort {secs / n:F0} sn{extra}");
        }
        // G: a band of the sim's heroes (higher level) against a whole camp — the same rules
        foreach (var comp in new[] { "cleric", "fighter", "rogue" })
        {
            int wins = 0, n = 60; float secs = 0;
            for (int s = 0; s < n; s++)
            {
                var fight = new Fight(s * 7717 + 3) { FoeHome = new V2(0, -40) };
                for (int i = 0; i < 3; i++)
                {
                    var c = Companion(comp, 6);
                    fight.AddCharacter(c, HeroFor(c), FSide.Party, new V2(i * 1.5f, 0), "hero");
                }
                for (int g = 0; g < 7; g++) fight.AddMonster("goblin", $"G{g}", FSide.Foe, new V2(-6 + g * 2f, -14 - (g % 2) * 2), g % 4 == 3 ? "archer" : "goblin", -1, g % 4 == 3 ? 16f : 0f);
                fight.Begin("deneme");
                float t = 0;
                while (!fight.Over && t < 240f) { fight.Update(0.05f); t += 0.05f; }
                secs += t;
                if (fight.Winner == FSide.Party) wins++;
            }
            Check($"3 × {comp} Sv6 vs 7 goblin", wins >= n * 0.8, $"{n} savaş: kazanılan {wins}, ort {secs / n:F0} sn");
        }

        // Tur 1 C: stances and running off (no buttons: the stance and a "go there" are all the player gives)
        {
            int n = 60, esc = 0, caught = 0, fleeEsc = 0, fleeSwings = 0, passiveSwings = 0, holdMoved = 0, bookOk = 0, offOk = 0;
            for (int s = 0; s < n; s++)
            {
                // a right click far away a second in: they run; caught ones turn and fight
                var fight = Make(new[] { "fighter", "cleric" }, 4, s * 9001 + 5, out var lead);
                float t = 0; bool ordered = false;
                while (!fight.Over && t < 120f)
                {
                    fight.Update(0.05f); t += 0.05f;
                    if (!ordered && t > 1f) { ordered = true; foreach (var f in fight.Of(FSide.Party)) { f.Order = "move"; f.OrderPoint = f.Pos + new V2(0, 90f); } }
                }
                if (fight.Escaped) esc++;
                if (s < 2) GD.Print($"[FightTest] kaçış örneği {s}: bitti {fight.Over} t {t:F0} kazanan {fight.Winner} kaçış {fight.Escaped}; ekip {string.Join(", ", fight.Of(FSide.Party).Select(f => $"{f.Name} ({f.Pos.X:F0},{f.Pos.Y:F0}) {f.Order} {(f.Down ? "yerde" : "")}"))}; düşman {string.Join(", ", fight.Of(FSide.Foe).Select(f => $"({f.Pos.X:F0},{f.Pos.Y:F0}){(f.Standing ? "" : "x")}"))}; son: {fight.Log.LastOrDefault()?.Text}");
                caught += fight.Log.Count(e => e.Kind == "caught") > 0 ? 1 : 0;
                // stance Kaç from the start: nobody of the party strikes, they get away
                var fk = Make(new[] { "fighter", "cleric" }, 4, s * 9001 + 6, out _);
                foreach (var f in fk.Of(FSide.Party)) f.Char.Stance = Stances.Flee;
                t = 0; while (!fk.Over && t < 120f) { fk.Update(0.05f); t += 0.05f; }
                if (fk.Escaped) fleeEsc++;
                fleeSwings += fk.Log.Count(e => e.A?.Side == FSide.Party && e.Kind is "attack" or "spell");
                // a passive cleric never strikes; a holding fighter never steps
                var fp = Make(new[] { "fighter", "cleric" }, 3, s * 9001 + 7, out var l3);
                var cl = fp.Of(FSide.Party).First(f => f.Char.Cls == "cleric"); cl.Char.Stance = Stances.Passive;
                l3.Char.Stance = Stances.Hold;
                var p0 = l3.Pos;
                t = 0; while (!fp.Over && t < 120f) { fp.Update(0.05f); t += 0.05f; }
                passiveSwings += fp.Log.Count(e => e.A == cl && e.Kind is "attack" or "spell");
                if (V2.Distance(l3.Pos, p0) > 1.0f) holdMoved++;
                // the book: firebolt first → only firebolts; sleep switched off → never sleep
                var fb = Make(new[] { "wizard", "fighter" }, 5, s * 9001 + 8, out var wz);
                wz.Char.SpellOrder = new List<string> { "firebolt" };
                t = 0; while (!fb.Over && t < 120f) { fb.Update(0.05f); t += 0.05f; }
                if (fb.Log.Where(e => e.A == wz && e.Kind == "spell").All(e => e.Short == null || e.Short.StartsWith("Ateş Oku"))) bookOk++;
                var fo = Make(new[] { "wizard", "fighter" }, 5, s * 9001 + 9, out var wz2);
                wz2.Char.SpellOff = new List<string> { "sleep" };
                t = 0; while (!fo.Over && t < 120f) { fo.Update(0.05f); t += 0.05f; }
                if (!fo.Log.Any(e => e.A == wz2 && e.Kind == "sleep")) offOk++;
            }
            Check("Tur 1: yere sağ tıkla kaçış", esc >= n * 0.6, $"{n} savaşta {esc} kaçış ({caught} savaşta biri yakalanıp dövüştü)");
            Check("Tur 1: Kaç duruşu", fleeEsc >= n * 0.8 && fleeSwings == 0, $"{fleeEsc}/{n} kurtuldu, ekibin vuruşu {fleeSwings}");
            Check("Tur 1: Pasif ve Yerini koru", passiveSwings == 0 && holdMoved <= n / 20, $"pasifin vuruşu {passiveSwings}, yerinden 1 m'den çok kayan {holdMoved}/{n}");
            Check("Tur 1: büyü kitabı sırası", bookOk == n && offOk == n, $"Ateş Oku başta: {bookOk}/{n} yalnız Ateş Oku; Uyku kapalı: {offOk}/{n} hiç uyutmadı");
        }

        // D: permanent wounds after bad death saves / crits while down (one in four), applied to the character
        {
            int fights = 200, wounds = 0, bad = 0; var kinds = new Dictionary<string, int>();
            int cha0 = 0, cha1 = 0, scars = 0; bool epithets = true;
            for (int s = 0; s < fights; s++)
            {
                var fight = Make(new[] { "wizard" }, 4, s * 104729 + 7, out var lead);
                float t = 0;
                while (!fight.Over && t < 200f) { fight.Update(0.05f); t += 0.05f; }
                if (fight.Winner != FSide.Party) fight.SettleFallen();
                bad += fight.Log.Count(e => e.Kind == "wound");
                cha0 = lead.Char.Stats[Rules.CHA];
                fight.WriteBack(100, "Deneme Kampı");
                if (lead.NewWound != null && !lead.Char.Dead)
                {
                    wounds++;
                    kinds[lead.NewWound] = kinds.GetValueOrDefault(lead.NewWound) + 1;
                    epithets &= lead.Char.Epithet != null && lead.Char.Wounds.Count == 1 && lead.Char.Wounds[0].Where == "Deneme Kampı";
                    if (lead.NewWound == "scar") { scars++; cha1 += cha0 - lead.Char.Stats[Rules.CHA]; }
                }
            }
            Check("kalıcı yara", wounds > 0 && epithets && cha1 == scars, $"{fights} yalnız büyücü savaşında (kaybedilenlerde yerdekiler ölüm zarlarını sürdürür) {wounds} kalıcı yara ({bad} yara olayı) ({string.Join(", ", kinds.Select(k => $"{Wound.Name(k.Key)} {k.Value}"))}); lakap ve yer yazıldı, iz Karizmayı 1 düşürdü");
        }

        // determinism
        var f1 = Make(new[] { "fighter", "wizard" }, 5, 777, out _); var f2 = Make(new[] { "fighter", "wizard" }, 5, 777, out _);
        for (int i = 0; i < 2000 && !f1.Over; i++) { f1.Update(0.05f); f2.Update(0.05f); }
        string h1 = string.Join("|", f1.Log.Select(e => e.Text)), h2 = string.Join("|", f2.Log.Select(e => e.Text));
        Check("belirlenim", h1 == h2 && h1.Length > 0, $"aynı tohum aynı savaş ({f1.Log.Count} olay)");
        GD.Print(sb.ToString());
        GD.Print(ok ? "FIGHTTEST PASS" : "FIGHTTEST FAIL");
        host.GetTree().Quit(ok ? 0 : 1);
    }

    /// <summary>Two heroes (a level-1 player and a level-2 companion with the class kit) against <paramref name="goblins"/> goblins 18 m away.</summary>
    public static Fight Make(string[] cls, int goblins, int seed, out Fighter lead)
    {
        var fight = new Fight(seed) { FoeHome = new V2(0, -40) };
        lead = null;
        for (int i = 0; i < cls.Length; i++)
        {
            var c = i == 0 ? PlayerLike(cls[i]) : Companion(cls[i]);
            var f = fight.AddCharacter(c, HeroFor(c), FSide.Party, new V2(i * 1.5f, 0), i == 0 ? "player" : "companion");
            if (i == 0) lead = f;
        }
        for (int g = 0; g < goblins; g++)
            fight.AddMonster("goblin", $"Goblin {g + 1}", FSide.Foe, new V2(-4 + g * 2f, -18 - (g % 2) * 2), g == goblins - 1 ? "archer" : "goblin", -1, g == goblins - 1 ? 18f : 0f);
        fight.Begin("deneme");
        return fight;
    }

    /// <summary>a fresh level-1 character with a starting kit bought (the test is about behaviour, not poverty)</summary>
    static Character PlayerLike(string cls)
    {
        var c = CharacterFactory.Player("Oyuncu", "human", cls, "good", Rules.SuggestedBase(cls), new Look(), "sleep", 10);
        switch (cls)
        {
            case "fighter": c.Inv.Add("longsword"); c.Weapon = "longsword"; c.Inv.Add("leather"); c.Armor = "leather"; break;
            case "rogue": c.Inv.Add("dagger"); c.Weapon = "dagger"; c.Inv.Add("leather"); c.Armor = "leather"; break;
            case "cleric": c.Inv.Add("mace"); c.Weapon = "mace"; c.Inv.Add("bandage", 2); break;
            case "wizard": c.Inv.Add("staff"); c.Weapon = "staff"; break;
        }
        c.Recalc(true); c.Rest();
        return c;
    }

    static Character Companion(string cls, int level = 2)
    {
        var h = new M.Hero { Id = 9000 + cls.Length, Name = "Yoldaş " + cls, Race = "human", Cls = cls, Level = level, Xp = 300, Stats = new M.JsObj<double>(), Hp = 99, MaxHp = 99, Align = "good", Given = "Yoldaş" };
        var pr = M.D.HERO_CLASSES[cls].Priority; int[] arr = { 15, 14, 13, 12, 10, 8 };
        for (int i = 0; i < 6; i++) h.Stats.Set(pr[i], arr[i]);
        return CharacterFactory.FromHero(h);
    }

    static M.Hero HeroFor(Character c) => new() { Id = 1, Name = c.Name, Cls = c.Cls, Level = c.Level, Race = c.Race, Stats = new M.JsObj<double>() };
}
