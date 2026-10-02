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
    public int DeathOk, DeathFail;

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

    /// <summary>Epithet from the worst wound (eye &gt; limp &gt; scar) unless the world already gave one.</summary>
    public void UpdateEpithet()
    {
        foreach (var k in new[] { "eye", "limp", "scar" }) if (HasWound(k)) { Epithet = Wound.Epithet(k); return; }
        Epithet = null;
    }
}
