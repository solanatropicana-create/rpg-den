using System;
using System.Collections.Generic;

namespace FD.Rpg;

/// <summary>Permanent wound (Faz 2 D): from a bad death save or a critical hit while down.</summary>
public sealed class Wound
{
    public string Kind;     // eye | limp | scar
    public int Day;         // macro day it happened
    public string Where;    // e.g. "Kırıkdiş Kampı"
    public static string Name(string k) => k switch { "eye" => "Kör göz", "limp" => "Topallık", _ => "Derin iz" };
    public static string Epithet(string k) => k switch { "eye" => "Tek Göz", "limp" => "Topal", _ => "Yaralı Yüz" };
    public static string Effect(string k) => k switch
    {
        "eye" => "Uzak saldırılarda −2; derinlik algısı bozuk.",
        "limp" => "Yürüme ve koşu %20 yavaş.",
        _ => "Karizma −1; yüzündeki iz herkesin dikkatini çeker.",
    };
    /// <summary>Temple cure price (silver): expensive (D&amp;D lesser restoration ≈ 40 gp).</summary>
    public const int CureSilver = 400;
}

/// <summary>Appearance chosen at creation (hair/skin on the shared human model; race sets build and ears).</summary>
public sealed class Look
{
    public bool Female;
    public int Skin = 0xd9a27c, Hair = 0x3a2314, Cloth1 = 0x7a5a3a, Cloth2 = 0x4c3b2b;
    public float Height = 1f;            // ±6 % around the race height
    public string HairStyle = "hair_short"; // hair_short | hair_long | hair_bun | none
    public bool Beard;
}

/// <summary>
/// A person at player scale: the player, a hired companion, or an NPC hero brought into the region. Linked to a macro
/// <c>Hero</c> record (<see cref="HeroId"/>) so the world knows them (faith, reputation, membership, wounds). Godot-free.
/// </summary>
public sealed class Character
{
    public int Id;
    public int? HeroId;
    public bool IsPlayer;
    public string Name = "", Race = "human", Cls = "fighter", Align = "neutral", Faith = "none";
    /// <summary>final scores (point buy + race bonus)</summary>
    public int[] Stats = { 10, 10, 10, 10, 10, 10 };
    public int Level = 1;
    public double Xp;
    public int MaxHp = 10, Hp = 10;
    public Inventory Inv = new();
    /// <summary>equipped item ids (null: none) — the items stay in the inventory</summary>
    public string Weapon, Armor, Shield;
    public List<string> Spells = new();
    public int Slots, SlotsMax;
    /// <summary>per-rest abilities (secondWind…)</summary>
    public Dictionary<string, int> Uses = new();
    public List<Wound> Wounds = new();
    public Look Look = new();
    public string Epithet;
    // companion contract (E)
    public int WageSilver;               // per week (5 days)
    public int HiredDay = -1, PaidUntil = -1;
    // downed / dead (D)
    public bool Down, Stable, Dead;
    /// <summary>D: locked in the goblin camp's cage</summary>
    public bool Captive;
    public int DeathOk, DeathFail;
    /// <summary>Tur 1 C: how they behave when foes are about (Kenshi): aggressive | defend (default) | hold | flee | passive</summary>
    public string Stance = Stances.Defend;
    /// <summary>Tur 1 C (spellbook v1): the order in which the caster uses their spells — the first one that fits is cast; spells in
    /// <see cref="SpellOff"/> are never cast by themselves. New spells go to the end.</summary>
    public List<string> SpellOrder = new();
    public List<string> SpellOff = new();

    /// <summary>The book as the caster reads it: the player's order, then any spell not yet placed (in the default order: healing,
    /// the area spells, the missiles, the cantrips); switched-off ones left out.</summary>
    public List<string> Book()
    {
        var r = new List<string>();
        foreach (var id in SpellOrder) if (Spells.Contains(id) && !SpellOff.Contains(id) && !r.Contains(id)) r.Add(id);
        var rest = new List<string>();
        foreach (var id in Spells) if (!SpellOrder.Contains(id) && !SpellOff.Contains(id) && !r.Contains(id) && !rest.Contains(id)) rest.Add(id);
        rest.Sort((a, b) => FD.Rpg.Spells.Rank(a).CompareTo(FD.Rpg.Spells.Rank(b)));
        r.AddRange(rest);
        return r;
    }

    /// <summary>the whole book in the order shown to the player (switched-off ones included)</summary>
    public List<string> BookAll()
    {
        var r = new List<string>();
        foreach (var id in SpellOrder) if (Spells.Contains(id) && !r.Contains(id)) r.Add(id);
        var rest = new List<string>();
        foreach (var id in Spells) if (!r.Contains(id) && !rest.Contains(id)) rest.Add(id);
        rest.Sort((a, b) => FD.Rpg.Spells.Rank(a).CompareTo(FD.Rpg.Spells.Rank(b)));
        r.AddRange(rest);
        return r;
    }

    /// <summary>Move a spell up (−1) or down (+1) in the book.</summary>
    public void MoveSpell(string id, int dir)
    {
        var all = BookAll();
        SpellOrder.Clear(); SpellOrder.AddRange(all);
        int i = SpellOrder.IndexOf(id), j = i + dir;
        if (i < 0 || j < 0 || j >= SpellOrder.Count) return;
        (SpellOrder[i], SpellOrder[j]) = (SpellOrder[j], SpellOrder[i]);
    }

    public void ToggleSpell(string id) { if (!SpellOff.Remove(id)) SpellOff.Add(id); }

    public string FullName => string.IsNullOrEmpty(Epithet) ? Name : $"{Name} «{Epithet}»";
    public int Mod(int stat) => Rules.Mod(Stats[stat]);
    public int Prof => Rules.Prof(Level);
    public bool Alive => !Dead;
    public bool Up => !Dead && !Down;

    public ItemDef WeaponDef => Items.Get(Weapon);
    public ItemDef ArmorDef => Items.Get(Armor);
    public ItemDef ShieldDef => Items.Get(Shield);

    /// <summary>Armour class: armour (or 10) + DEX (capped by armour) + shield; scar/eye do not change it.</summary>
    public int Ac
    {
        get
        {
            var a = ArmorDef;
            int dex = Mod(Rules.DEX);
            int ac = a != null ? a.ArmorBase + Math.Min(dex, a.DexMax) : 10 + dex;
            if (ShieldDef != null) ac += ShieldDef.ArmorBase;
            return ac;
        }
    }

    /// <summary>Melee/ranged attack with the equipped weapon (or unarmed: 1 + STR, D&amp;D).</summary>
    public (int atk, int n, int sides, int bonus, bool finesse, float range, string name) Attack()
    {
        var w = WeaponDef;
        if (w == null) return (Prof + Mod(Rules.STR), 1, 1, Math.Max(0, Mod(Rules.STR)), false, 0, "Yumruk");
        bool useDex = w.Finesse && Mod(Rules.DEX) > Mod(Rules.STR) || w.Range > 0;
        int m = Mod(useDex ? Rules.DEX : Rules.STR);
        int atk = Prof + m - (w.Range > 0 && HasWound("eye") ? 2 : 0);
        return (atk, w.DiceN, w.DiceS, m, w.Finesse, w.Range, w.Name);
    }

    public bool HasWound(string k) { foreach (var w in Wounds) if (w.Kind == k) return true; return false; }
    public float SpeedFactor => Rules.SpeedFactor(Race) * (HasWound("limp") ? 0.8f : 1f) * (Inv.Weight > Capacity ? 0.6f : Inv.Weight > Capacity * 0.5f ? 0.85f : 1f);
    public float Capacity => Rules.Capacity(Stats[Rules.STR], Race);
    public bool Overloaded => Inv.Weight > Capacity;

    /// <summary>Long rest: hit points, spell slots and per-rest abilities back (not the dead).</summary>
    public void Rest()
    {
        if (Dead) return;
        Hp = MaxHp; Down = false; Stable = false; DeathOk = DeathFail = 0;
        Slots = SlotsMax;
        Uses["secondWind"] = Cls == "fighter" ? 1 : 0;
    }

    /// <summary>Recompute derived numbers after a level or stat change.</summary>
    public void Recalc(bool heal = false)
    {
        int old = MaxHp;
        MaxHp = Rules.MaxHp(Cls, Level, Stats[Rules.CON]);
        if (heal) Hp = MaxHp; else Hp = Math.Min(MaxHp, Hp + Math.Max(0, MaxHp - old));
        SlotsMax = Rules.Slots1(Cls, Level);
        if (heal) Slots = SlotsMax;
        if (!Uses.ContainsKey("secondWind")) Uses["secondWind"] = Cls == "fighter" ? 1 : 0;
    }

    /// <summary>D: a permanent wound (a scar costs a point of Charisma; the limp and the eye act through <see cref="SpeedFactor"/>
    /// and <see cref="Attack"/>); the epithet follows the worst wound.</summary>
    public void AddWound(string kind, int day, string where)
    {
        if (HasWound(kind)) return;
        Wounds.Add(new Wound { Kind = kind, Day = day, Where = where });
        if (kind == "scar") Stats[Rules.CHA] = Math.Max(3, Stats[Rules.CHA] - 1);
        UpdateEpithet();
    }

    /// <summary>D: the temple heals a wound (lesser restoration); the scar's Charisma comes back.</summary>
    public void CureWound(string kind)
    {
        int i = Wounds.FindIndex(w => w.Kind == kind);
        if (i < 0) return;
        Wounds.RemoveAt(i);
        if (kind == "scar") Stats[Rules.CHA] += 1;
        UpdateEpithet();
    }

    /// <summary>Epithet from the worst wound (eye &gt; limp &gt; scar) unless the world already gave one.</summary>
    public void UpdateEpithet()
    {
        foreach (var k in new[] { "eye", "limp", "scar" }) if (HasWound(k)) { Epithet = Wound.Epithet(k); return; }
        Epithet = null;
    }
}

/// <summary>Tur 1 C: the stances (Kenshi). The fight reads them; the party bar sets them (Shift+1…5 for the selected).</summary>
public static class Stances
{
    public const string Aggressive = "aggressive", Defend = "defend", Hold = "hold", Flee = "flee", Passive = "passive";
    public static readonly string[] All = { Aggressive, Defend, Hold, Flee, Passive };
    public static string Name(string s) => s switch
    {
        Aggressive => "Saldırgan", Hold => "Yerini koru", Flee => "Kaç", Passive => "Pasif", _ => "Savunmada",
    };
    public static string Short(string s) => s switch { Aggressive => "Sal", Hold => "Yer", Flee => "Kaç", Passive => "Pas", _ => "Sav" };
    public static string Desc(string s) => s switch
    {
        Aggressive => "Gözüne kestirdiği düşmana kendiliğinden saldırır (yolda da).",
        Hold => "Yerinden kıpırdamaz; menzildekine vurur.",
        Flee => "Dövüşmez; düşmandan uzaklaşır.",
        Passive => "Hiç karşılık vermez.",
        _ => "Saldırıya uğrayınca ya da bir yoldaşı sıkışınca karşılık verir.",
    };
}
