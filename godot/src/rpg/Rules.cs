using System;
using System.Collections.Generic;
using M = FD.Macro;

namespace FD.Rpg;

/// <summary>
/// Faz 2: D&amp;D 5e rules the player-scale game uses, on top of the macro world's tables (races, classes, XP, names) so a
/// character made here and a hero born in the sim are the same kind of person (simetri). Godot-free.
/// </summary>
public static class Rules
{
    public static readonly string[] StatIds = { "str", "dex", "con", "int", "wis", "cha" };
    public static readonly string[] StatTr = { "Güç", "Çeviklik", "Dayanıklılık", "Zekâ", "Bilgelik", "Karizma" };
    public static readonly string[] StatShort = { "GÜÇ", "ÇEV", "DAY", "ZEK", "BİL", "KAR" };
    public const int STR = 0, DEX = 1, CON = 2, INT = 3, WIS = 4, CHA = 5;

    /// <summary>9 races of the macro world (D.RACES order).</summary>
    public static readonly string[] Races = { "human", "dwarf", "elf", "halfling", "gnome", "halfelf", "halforc", "dragonborn", "tiefling" };
    /// <summary>Faz 2 classes: savaşçı, haydut, büyücü, rahip.</summary>
    public static readonly string[] Classes = { "fighter", "rogue", "wizard", "cleric" };
    public static readonly string[] Aligns = { "good", "neutral", "evil" };

    public static int Mod(int score) => (int)Math.Floor((score - 10) / 2.0);
    public static string Signed(int v) => v >= 0 ? $"+{v}" : v.ToString();

    // ------------------------------------------------------------ point buy (27)
    public const int PointBuy = 27, BuyMin = 8, BuyMax = 15;
    public static int Cost(int score) => score switch { 8 => 0, 9 => 1, 10 => 2, 11 => 3, 12 => 4, 13 => 5, 14 => 7, 15 => 9, _ => 99 };
    public static int Spent(int[] baseStats) { int t = 0; foreach (int s in baseStats) t += Cost(s); return t; }

    /// <summary>Race bonus to a stat (macro RACE_STAT: human +1 all, dwarf STR/CON +2, …).</summary>
    public static int RaceBonus(string race, int stat)
    {
        var rb = M.D.RACE_STAT.GetOr(race, null);
        if (rb == null) return 0;
        return (int)(M.JsObjExt.Get(rb, StatIds[stat]) ?? 0);
    }

    /// <summary>Suggested point-buy spread for a class (D&amp;D standard array 15,14,13,12,10,8 by the macro priority).</summary>
    public static int[] SuggestedBase(string cls)
    {
        int[] arr = { 15, 14, 13, 12, 10, 8 };
        var o = new int[6];
        var pr = M.D.HERO_CLASSES[cls].Priority;
        for (int i = 0; i < pr.Count && i < 6; i++) o[Array.IndexOf(StatIds, pr[i])] = arr[i];
        return o;
    }

    public static string RaceName(string race) => M.D.RACES.GetOr(race, null)?.Name ?? race;
    public static string ClassName(string cls) => cls switch
    {
        "fighter" => "Savaşçı", "rogue" => "Haydut", "wizard" => "Büyücü", "cleric" => "Rahip",
        _ => M.D.HERO_CLASS_TR.TryGet(cls, out var t) ? t : cls,
    };
    public static string AlignName(string a) => a switch { "good" => "İyi", "evil" => "Kötü", _ => "Tarafsız" };
    public static string FaithName(string f) => f switch { "sun" => "Güneş Kilisesi", "old" => "Eski İnanç", "pact" => "Kara Pakt", _ => "inançsız" };

    public static int HitDie(string cls) => (int)M.D.HERO_CLASSES[cls].HitDie;
    public static string PrimaryStat(string cls) => M.D.HERO_CLASSES[cls].Primary;
    public static int Prof(int level) => level >= 9 ? 4 : level >= 5 ? 3 : 2;
    /// <summary>D&amp;D max HP: full hit die at level 1, then average (half + 1) per level, + CON each level.</summary>
    public static int MaxHp(string cls, int level, int con) => Math.Max(1, HitDie(cls) + Mod(con) + (level - 1) * (HitDie(cls) / 2 + 1 + Mod(con)));
    /// <summary>Level from XP (macro XP_LEVELS, D&amp;D table).</summary>
    public static int LevelFor(double xp)
    {
        var t = M.D.XP_LEVELS;
        int l = 1;
        for (int i = 1; i < t.Count; i++) if (xp >= t[i]) l = i + 1;
        return Math.Min(l, t.Count);
    }
    public static double XpFor(int level) => M.D.XP_LEVELS[Math.Clamp(level - 1, 0, M.D.XP_LEVELS.Count - 1)];

    /// <summary>Walking speed factor (D&amp;D 25 ft small and dwarf vs 30 ft): applied to the 1:1 movement speeds.</summary>
    public static float SpeedFactor(string race) => race is "dwarf" or "halfling" or "gnome" ? 0.85f : race == "elf" ? 1.05f : 1f;
    /// <summary>Carrying capacity (kg): STR × 7 (≈ D&amp;D 15 lb per point); over half → slowed, over full → cannot run.</summary>
    public static float Capacity(int str, string race) => str * 7f * (race is "halfling" or "gnome" ? 0.75f : 1f);

    /// <summary>Spellcasting stat and save DC / attack bonus.</summary>
    public static int CastStat(string cls) => cls == "wizard" ? INT : cls == "cleric" ? WIS : CHA;
    public static int SpellDc(Character c) => 8 + Prof(c.Level) + Mod(c.Stats[CastStat(c.Cls)]);
    public static int SpellAtk(Character c) => Prof(c.Level) + Mod(c.Stats[CastStat(c.Cls)]);
    /// <summary>1st-level spell slots by level (wizard, cleric): 2 at level 1, 3 at 2, 4 from 3.</summary>
    public static int Slots1(string cls, int level) => cls is "wizard" or "cleric" ? (level >= 3 ? 4 : level == 2 ? 3 : 2) : 0;
    /// <summary>Rogue sneak attack dice (d6).</summary>
    public static int SneakDice(int level) => (level + 1) / 2;

    // ------------------------------------------------------------ money
    /// <summary>1 altın = 10 gümüş. The macro world counts gold; the player's purse is silver.</summary>
    public const int SilverPerGold = 10;
    public static string Money(int silver)
    {
        if (silver >= 100 && silver % 10 == 0) return $"{silver / 10} altın";
        if (silver >= 100) return $"{silver / 10} altın {silver % 10} gümüş";
        return $"{silver} gümüş";
    }
}
