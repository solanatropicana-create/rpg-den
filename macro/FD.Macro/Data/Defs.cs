using System.Collections.Generic;
using System.Text.Json.Serialization;

// Static game data definitions (mirror of src/data/*.ts interfaces). Loaded from Data/data.json
// (generated from the TS tables by golden/ts/dumpdata.ts) by D.cs.

namespace FD.Macro;

public sealed class GoodDef
{
    public string Id, Name;
    public double Base;
    public string Tier, Group;
    public double? Food;
    public string Src, Use, Lack;
}

public sealed class DepositDef
{
    public string Kind, Name, Good;
    public double Rate;
    public double Reserve;
    public double? Regen;
    public List<string> Terrain;
    public List<double> Size;
    public double Count;
    /// <summary>Faz 1b-3: medeniyet bu kademeye (en büyük yerleşimi) varmadan yatak görünmez (null: hep görünür)</summary>
    public int? HiddenTier;
    public string Building;
    public string Color;
}

public sealed class ExtractDef
{
    public string Kind;
    public List<string> Names;   // L1, L2, L3 ("" = none)
    /// <summary>Faz 1b-3: seviye başına gereken yerleşim kademesi (yapının bağlı olduğu yerleşimin; null = seviye yok)</summary>
    public List<int?> Tier;
    public int? StartLevel;
}

public sealed class WorkshopDef
{
    public string Kind, Name;
    /// <summary>Faz 1b-3: atölyenin kurulacağı yerleşimin en az kademesi (null: 0)</summary>
    public int? Tier;
    public JsObj<double> Inputs;
    public JsObj<double> Alt;
    /// <summary>Faz 1b-3: alternatif girdinin (demir) kullanılacağı yerleşim kademesi</summary>
    public int? AltTier;
    public string Output;
    public double Rate;
    public double Slots;
    public JsObj<double> Cost;
}

public sealed class CivicDef
{
    public string Kind, Name;
    /// <summary>Faz 1b-3: yapının kurulacağı yerleşimin en az kademesi (null: 0)</summary>
    public int? Tier;
    public JsObj<double> Cost;
    public double Work;
    public double? Housing;
    public double? Max;
}

public sealed class ShipDef
{
    public JsObj<double> Cost;
    public double Work;
}

public sealed class RaceDef
{
    public string Id, Name, Plural;
    public double Growth, Hp, Atk;
}

public sealed class ClassDef
{
    public string Id, Name, Dnd, Race;
    public Alignment Align;
    public string Feature, FeatureDesc;
    public JsObj<double> Base;
    public List<string> Desires;
    public string HeroClass, CivName, Capital;
    public List<string> Towns;
    public string Color;
    public JsObj<double> Prefer;
    public JsObj<double> TerrainLike;
    public double Aggression;
    /// <summary>Faz 1b-3: sınıfın kademeye bağlı ayrıcalıkları (eski sınıf ağacından kalanlar: özel birlikler ve sınıf
    /// özelliğini taşıyan birkaç etki). Medeniyetin en büyük yerleşiminin kademesiyle açılır.</summary>
    public List<ClassPerk> Perks;
    public bool Implemented;
}

/// <summary>Faz 1b-3: sınıf ayrıcalığı: medeniyet <see cref="Tier"/> kademesine varınca özel birlik ve/veya etki açılır.</summary>
public sealed class ClassPerk
{
    public int Tier;
    public string Name;
    /// <summary>açılan özel birlik (UNITS anahtarı; null: yok)</summary>
    public string Unit;
    /// <summary>açılan etki (Civ.Eff'e eklenir; null: yok)</summary>
    public JsObj<double> Eff;
}

public sealed class UnitDef
{
    public string Id, Name;
    public double Hp, Ac, Atk;
    public List<double> Dmg;     // [n, sides, bonus]
    public double? Attacks;
    public double? Per;
    public string Needs;
}

/// <summary>Monster stat block (MONSTERS.*); same shape as UnitStats in combat.ts.</summary>
public sealed class MonsterDef
{
    public string Name;
    public double Hp, Ac, Atk;
    public List<double> Dmg;
    public double? Attacks;
    /// <summary>Faz 1 B2: her tur başında kapanan yara (trol yenilenmesi; ateş yarası bir tur durdurur)</summary>
    public double? Regen;
    /// <summary>Faz 1 B2: nefes silahı (ejderha): zar sayısı (d6) ve en çok hedef; 1/3 olasılıkla yeniden dolar</summary>
    public double? Breath, BreathN;
}

public sealed class HeroClassDef
{
    public string Id;
    public double HitDie;
    public string Primary;
    public List<string> Priority;
    public List<double> Dmg;
}

public sealed class TraitDef
{
    public string Name, Desc, Icon;
}

public sealed class InnLevelDef
{
    public string Name;
    public double Rooms, Stable, Food, Ale, Wood;
    public List<string> Staff;
    public double Room, Meal;
    [JsonPropertyName("ale$")] public double AleP;
}

public sealed class InnUpgradeDef
{
    public double Gold, Wood, Stone, Work, Fame, Year;
}

public sealed class StaffDef
{
    public string Name;
    public double Wage, Serve;
}

public sealed class GuestDef
{
    public string Name, Icon;
    public List<double> Purse, Nights, N;
    public double Mugs, Eat;
}
