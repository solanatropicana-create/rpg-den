using System;
using System.Collections.Generic;
using M = FD.Macro;

namespace FD.Rpg;

/// <summary>Faz 2: building characters — the player's (creation screen) and NPC heroes brought into the region from the macro
/// world (companions, rivals), so both are the same kind of sheet and fight by the same rules.</summary>
public static class CharacterFactory
{
    /// <summary>
    /// The player's character from the creation screen: point-buy base scores + race bonus, level 1, full HP, the class's spells
    /// (wizard: two cantrips and the chosen first-level spell; cleric: the Sun Church's two prayers), no weapon, the clothes they
    /// stand in and <paramref name="silver"/> (5–15).
    /// </summary>
    public static Character Player(string name, string race, string cls, string align, int[] baseStats, Look look, string wizardSpell, int silver)
    {
        var c = new Character { Name = name, Race = race, Cls = cls, Align = align, Look = look, IsPlayer = true };
        for (int i = 0; i < 6; i++) c.Stats[i] = baseStats[i] + Rules.RaceBonus(race, i);
        c.Faith = cls == "cleric" ? "sun" : "none";
        AddClassSpells(c, wizardSpell);
        c.Inv.Silver = silver;
        c.Recalc(heal: true);
        c.Rest();
        return c;
    }

    static void AddClassSpells(Character c, string wizardSpell)
    {
        if (c.Cls == "wizard")
        {
            c.Spells.AddRange(Spells.WizardCantrips);
            c.Spells.Add(Array.IndexOf(Spells.WizardFirst, wizardSpell) >= 0 ? wizardSpell : Spells.WizardFirst[0]);
        }
        else if (c.Cls == "cleric") c.Spells.AddRange(Spells.ClericPrayers);
    }

    /// <summary>
    /// An NPC hero of the macro world at player scale: the hero's own scores, level, HP and alignment, and the kit the macro rules
    /// assume for the class (Combat.HeroAc / HeroCombatant): fighter longsword, chain shirt and shield; rogue shortsword and
    /// leather; cleric mace, chain shirt and shield; wizard staff; others a spear.
    /// </summary>
    public static Character FromHero(M.Hero h)
    {
        string cls = Array.IndexOf(Rules.Classes, h.Cls) >= 0 ? h.Cls : h.Cls switch { "paladin" => "fighter", "barbarian" => "fighter", "ranger" => "fighter", "druid" => "cleric", _ => "fighter" };
        var c = new Character
        {
            HeroId = h.Id, Name = h.Name, Race = h.Race, Cls = cls, Align = h.Align ?? "neutral", Faith = h.Faith ?? "none", Level = h.Level, Xp = h.Xp,
            Epithet = h.Epithet,
        };
        for (int i = 0; i < 6; i++) c.Stats[i] = (int)(M.JsObjExt.Get(h.Stats, Rules.StatIds[i]) ?? 10);
        switch (cls)
        {
            case "fighter": Give(c, "longsword", true); Give(c, "chainshirt", true); Give(c, "shield", true); break;
            case "rogue": Give(c, "shortsword", true); Give(c, "dagger"); Give(c, "leather", true); break;
            case "cleric": Give(c, "mace", true); Give(c, "chainshirt", true); Give(c, "shield", true); break;
            case "wizard": Give(c, "staff", true); break;
        }
        if (cls == "wizard") { c.Spells.AddRange(Spells.WizardCantrips); c.Spells.Add(h.Level >= 3 ? "sleep" : "magicmissile"); if (h.Level >= 2) c.Spells.Add("burninghands"); }
        else if (cls == "cleric") c.Spells.AddRange(Spells.ClericPrayers);
        c.Inv.Add("ration", 2);
        if (h.Level >= 2) c.Inv.Add("bandage", 1);
        c.Inv.Silver = (int)Math.Round(h.Gold * Rules.SilverPerGold * 0.3);
        c.Recalc(heal: true);
        c.Hp = Math.Clamp((int)Math.Round(h.Hp), 1, c.MaxHp);
        c.Rest();
        c.Hp = Math.Clamp((int)Math.Round(h.Hp), 1, c.MaxHp);
        var r = new M.Rng(h.Id * 7 + 3);
        c.Look = new Look
        {
            Female = IsFemaleName(h),
            Skin = 0, Hair = 0, Cloth1 = ClassColor(cls, r), Cloth2 = 0x4a3a2a,
            HairStyle = r.Chance(0.5) ? "hair_short" : "hair_long", Beard = r.Chance(0.4), Height = 0.97f + (float)r.Next() * 0.06f,
        };
        return c;
    }

    static void Give(Character c, string id, bool equip = false)
    {
        c.Inv.Add(id);
        if (!equip) return;
        var d = Items.Get(id);
        if (d == null) return;
        if (d.Kind == ItemKind.Weapon) c.Weapon = id;
        else if (d.Kind == ItemKind.Armor) c.Armor = id;
        else if (d.Kind == ItemKind.Shield) c.Shield = id;
    }

    static int ClassColor(string cls, M.Rng r) => cls switch
    {
        "fighter" => r.Chance(0.5) ? 0x6a2a2a : 0x3a4a6a,
        "rogue" => r.Chance(0.5) ? 0x2a3a2a : 0x3a2f2a,
        "wizard" => r.Chance(0.5) ? 0x3a2f6a : 0x5a2a5a,
        "cleric" => 0xd8c890,
        _ => 0x5a4a3a,
    };

    /// <summary>Macro name pools alternate male/female; a hero's given name tells which.</summary>
    public static bool IsFemaleName(M.Hero h)
    {
        var pool = M.D.HERO_NAMES.GetOr(h.Race, null);
        if (pool == null || string.IsNullOrEmpty(h.Given)) return false;
        int i = pool.IndexOf(h.Given);
        return i >= 0 && i % 2 == 1;
    }

    /// <summary>Random name from the race's macro pools (male even, female odd entries).</summary>
    public static string RandomName(string race, bool female, Random rnd)
    {
        var given = M.D.HERO_NAMES.GetOr(race, null) ?? M.D.HERO_NAMES["human"];
        var fam = M.D.HERO_SURNAMES.GetOr(race, null) ?? M.D.HERO_SURNAMES["human"];
        int half = Math.Max(1, given.Count / 2);
        int k = Math.Min(given.Count - 1, rnd.Next(half) * 2 + (female ? 1 : 0));
        return $"{given[k]} {fam[rnd.Next(fam.Count)]}";
    }
}
