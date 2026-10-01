using System;
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
    public string HiddenUntil;
    public string Building;
    public string Color;
}

public sealed class ExtractDef
{
    public string Kind;
    public List<string> Names;   // L1, L2, L3 ("" = none)
    public List<string> Tech;    // per level; null = none/start
    public int? StartLevel;
}

public sealed class WorkshopDef
{
    public string Kind, Name, Tech;
    public JsObj<double> Inputs;
    public JsObj<double> Alt;
    public string AltTech;
    public string Output;
    public double Rate;
    public double Slots;
    public JsObj<double> Cost;
}

public sealed class CivicDef
{
    public string Kind, Name, Tech;
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

/// <summary>Context for a subclass pick weight (TS PickCtx).</summary>
public sealed class PickCtx
{
    public bool War;
    public double Threat, Forest, Law, Good, Pop;
}

public sealed class SubclassDef
{
    public string Id, Name, Desc;
    public JsObj<double> Eff;
    public string CapName, CapDesc;
    public JsObj<double> CapEff;
    /// <summary>Hand-ported from classes.ts (see Picks.cs).</summary>
    [JsonIgnore] public Func<PickCtx, double> Pick;
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
    public List<SubclassDef> Subclasses;
    public bool Implemented;
}

public sealed class WonderDef
{
    public string Name, Desc;
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
}

public sealed class TechDef
{
    public string Id, Name;
    public int Era;
    public string Tree;          // 'main' | ClassId
    public List<string> Req;
    public List<string> Gate;
    public List<string> GateAny;
    public string Unlock;
    public JsObj<double> Eff;
    public string Unit;
    public bool? Subclass;
    public bool? Capstone;
    public string Chain;
    public double? Cost;
}

public sealed class EraRule
{
    public double Nodes, Pop;
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
