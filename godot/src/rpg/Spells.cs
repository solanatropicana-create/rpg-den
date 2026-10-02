using System.Collections.Generic;

namespace FD.Rpg;

public enum SpellKind { Attack, Save, Auto, Heal, Sleep }

/// <summary>Faz 2 spells (PHB, level 0–1). Wizards keep them in a book (2 cantrips + 1 first-level spell at the start, more are found
/// in the world); clerics of the Sun Church have two prayers.</summary>
public sealed class SpellDef
{
    public string Id, Name, Desc;
    public int Level;               // 0 = cantrip (at will)
    public SpellKind Kind;
    public float Range;             // m
    public int N, S;                // dice
    public bool AddMod;             // + casting stat modifier (heals)
    public int Save = -1;           // stat index the target saves with
    public float Radius;            // area (m)
    public int Missiles;            // magic missile darts
    public bool Slows;              // ray of frost
    public string Class;            // wizard | cleric
}

public static class Spells
{
    public static readonly Dictionary<string, SpellDef> All = new();
    static SpellDef Add(SpellDef d) { All[d.Id] = d; return d; }

    static Spells()
    {
        // büyücü: basit büyüler
        Add(new SpellDef { Id = "firebolt", Name = "Ateş Oku", Level = 0, Kind = SpellKind.Attack, Range = 30f, N = 1, S = 10, Class = "wizard",
            Desc = "Basit büyü. Uzaktaki bir düşmana ateş fırlatır: büyü saldırısı, isabette 1d10 ateş." });
        Add(new SpellDef { Id = "rayoffrost", Name = "Buz Işını", Level = 0, Kind = SpellKind.Attack, Range = 18f, N = 1, S = 8, Slows = true, Class = "wizard",
            Desc = "Basit büyü. Soğuk bir ışın: büyü saldırısı, isabette 1d8 soğuk ve hedef bir süre yavaşlar." });
        // büyücü: 1. seviye (başta biri seçilir)
        Add(new SpellDef { Id = "magicmissile", Name = "Sihirli Füze", Level = 1, Kind = SpellKind.Auto, Range = 30f, N = 1, S = 4, Missiles = 3, Class = "wizard",
            Desc = "1. seviye. Üç parlak ok şaşmadan vurur: her biri 1d4+1." });
        Add(new SpellDef { Id = "burninghands", Name = "Yanan Eller", Level = 1, Kind = SpellKind.Save, Range = 4.5f, Radius = 4.5f, N = 3, S = 6, Save = Rules.DEX, Class = "wizard",
            Desc = "1. seviye. Önündeki koniye alev: 3d6 ateş, Çeviklik kurtarışını tutan yarısını alır." });
        Add(new SpellDef { Id = "sleep", Name = "Uyku", Level = 1, Kind = SpellKind.Sleep, Range = 27f, Radius = 6f, N = 5, S = 8, Class = "wizard",
            Desc = "1. seviye. 5d8 can değerince yaratık (en zayıftan başlayarak) bir süre uyur." });
        // rahip (Güneş Kilisesi)
        Add(new SpellDef { Id = "sacredflame", Name = "Kutsal Alev", Level = 0, Kind = SpellKind.Save, Range = 18f, N = 1, S = 8, Save = Rules.DEX, Class = "cleric",
            Desc = "Dua. Güneşin ışığı hedefin üstüne iner: Çeviklik kurtarışı, tutmazsa 1d8 ışıltı." });
        Add(new SpellDef { Id = "curewounds", Name = "Yara Sarma", Level = 1, Kind = SpellKind.Heal, Range = 1.8f, N = 1, S = 8, AddMod = true, Class = "cleric",
            Desc = "Dua (1. seviye). Dokunduğu yaralıya 1d8 + Bilgelik can; baygını ayağa kaldırır." });
    }

    public static SpellDef Get(string id) => id != null && All.TryGetValue(id, out var d) ? d : null;
    public static readonly string[] WizardCantrips = { "firebolt", "rayoffrost" };
    public static readonly string[] WizardFirst = { "magicmissile", "burninghands", "sleep" };
    public static readonly string[] ClericPrayers = { "sacredflame", "curewounds" };
}
