using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace FD.Macro;

/// <summary>
/// All static game data (TS <c>src/data/*.ts</c> exports), loaded once from the embedded
/// <c>Data/data.json</c>. Field names are the TS export names, so TS <c>GOODS[g].base</c> ports to
/// <c>D.GOODS[g].Base</c>. Record key order is the TS object order (it matters for iteration).
/// </summary>
public static class D
{
    // ---- goods.ts
    public static JsObj<GoodDef> GOODS;
    public static List<string> GOOD_IDS;
    public static List<string> FOODS;
    public static JsObj<string> TERRAIN_TR;
    public static JsObj<double> MOVE_COST;
    public static JsObj<DepositDef> DEPOSITS;
    public static JsObj<ExtractDef> EXTRACTS;
    public static List<double> LEVEL_SLOTS;
    public static List<double> LEVEL_MULT;
    public static List<JsObj<double>> LEVEL_COST;
    public static double BRICK_FUEL, HIDE, HIDE_TUNDRA, WOOD_RESERVE, STONE_RESERVE, FOREST_REGROW_DAYS;
    public static JsObj<WorkshopDef> WORKSHOPS;
    public static List<string> WORKSHOP_IDS;
    public static JsObj<CivicDef> CIVICS;
    public static JsObj<ShipDef> SHIPS;
    // ---- classes.ts
    public static JsObj<RaceDef> RACES;
    public static JsObj<ClassDef> CLASSES;
    public static List<string> CLASS_IDS;
    public static JsObj<WonderDef> WONDERS;
    public static JsObj<UnitDef> UNITS;
    public static JsObj<MonsterDef> MONSTERS;
    // ---- techs.ts
    public static List<string> ERA_TR;
    public static List<string> ERA_ROMAN;
    public static JsNumObj<EraRule> ERA_RULE;
    public static JsNumObj<List<double>> ERA_COST;
    public static List<TechDef> MAIN_TECHS;
    public static List<TechDef> CLASS_TECHS;
    /// <summary>[...MAIN_TECHS, ...CLASS_TECHS]</summary>
    public static List<TechDef> ALL_TECHS;
    /// <summary>Object.fromEntries(ALL_TECHS.map(t => [t.id, t]))</summary>
    public static JsObj<TechDef> TECH;
    // ---- heroes.ts
    public static JsObj<string> HERO_CLASS_TR;
    public static List<string> HERO_CLASS_IDS;
    public static List<string> STATS;
    public static JsObj<HeroClassDef> HERO_CLASSES;
    public static List<double> XP_LEVELS;
    public static JsObj<List<string>> HERO_NAMES;
    public static List<string> EPITHETS, HERO_ORIGINS, HERO_DRIVES;
    public static JsObj<JsObj<double>> RACE_STAT;
    public static JsObj<string> ALIGN_TR, PATH_TR, PATH_DESC, CLASS_PATH;
    public static JsObj<TraitDef> TRAITS;
    public static List<string> INN_NAMES, KEEPER_NAMES;
    // ---- inn.ts
    public static List<InnLevelDef> INN_LEVEL;
    public static JsNumObj<InnUpgradeDef> INN_UPGRADE;
    public static JsObj<StaffDef> STAFF;
    public static JsObj<GuestDef> GUEST;
    public static double GRAIN_FOOD, GRAIN_ALE, BEER_ALE;

    static D()
    {
        using var s = typeof(D).Assembly.GetManifestResourceStream("FD.Macro.Data.data.json")
            ?? throw new InvalidOperationException("embedded Data/data.json missing");
        using var doc = JsonDocument.Parse(s);
        foreach (var f in typeof(D).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (f.Name == nameof(ALL_TECHS) || f.Name == nameof(TECH)) continue;
            if (!doc.RootElement.TryGetProperty(f.Name, out var el))
                throw new InvalidOperationException($"data.json has no {f.Name}");
            f.SetValue(null, el.Deserialize(f.FieldType, Json.Options));
        }
        ALL_TECHS = new List<TechDef>(MAIN_TECHS);
        ALL_TECHS.AddRange(CLASS_TECHS);
        TECH = new JsObj<TechDef>();
        foreach (var t in ALL_TECHS) TECH.Set(t.Id, t);
        Picks.Attach();
    }

    /// <summary>Forces loading (call once at startup to fail fast).</summary>
    public static void Init() { }

    // ---- functions from the data modules

    /// <summary>techs.ts techCost: explicit cost or a deterministic spread inside the era range.</summary>
    public static double TechCost(TechDef t)
    {
        if (J.T(t.Cost)) return t.Cost.Value;
        var lohi = ERA_COST[t.Era];
        double lo = lohi[0], hi = lohi[1];
        uint h = 0;
        foreach (Rune ch in t.Id.EnumerateRunes())
        {
            // `for (const ch of id)` iterates code points; charCodeAt(0) is the first UTF-16 unit
            Span<char> buf = stackalloc char[2];
            ch.EncodeToUtf16(buf);
            h = unchecked(h * 31 + buf[0]);
        }
        return JsMath.Round(lo + (h % 1000) / 1000.0 * (hi - lo));
    }

    private static readonly Dictionary<string, string> EXT_POSS = new()
    {
        ["Tarla"] = "tarlası", ["Balıkçı Limanı"] = "balıkçı limanı", ["Balıkçı Filosu"] = "balıkçı filosu",
        ["Sulamalı Tarla"] = "sulamalı tarlası", ["Çiftlik"] = "çiftliği", ["Bıçkıhane"] = "bıçkıhanesi",
        ["İskele"] = "iskelesi", ["Açık Ocak"] = "açık ocağı", ["Derin Maden"] = "derin madeni", ["Ağıl"] = "ağılı",
        ["Haras"] = "harası", ["Öz Toplama"] = "öz toplama yeri", ["Kutsal Koru"] = "kutsal korusu",
    };

    /// <summary>goods.ts extPoss: possessive form of an extraction building name ("X'in ___").</summary>
    public static string ExtPoss(string kind, int level)
    {
        var names = EXTRACTS[kind].Names;
        string n = J.At(names, level - 1);
        if (!J.T(n)) n = J.Find(names, x => J.T(x));
        return EXT_POSS.TryGetValue(n, out var p) ? p : J.TrLower(n);
    }
}

/// <summary>Subclass pick weights, hand-ported from src/data/classes.ts (functions are not in data.json).</summary>
internal static class Picks
{
    private static readonly Dictionary<string, Func<PickCtx, double>> P = new()
    {
        ["devotion"] = c => 1 + c.Law,
        ["ancients"] = c => 0.6 + c.Forest * 2,
        ["vengeance"] = c => 0.4 + (c.War ? 1.5 : 0) + c.Threat,
        ["forge"] = _ => 1.6,
        ["war"] = c => 0.5 + (c.War ? 1.5 : 0) + c.Threat,
        ["life"] = _ => 0.9,
        ["land"] = _ => 1.2,
        ["moon"] = c => 0.5 + c.Threat + (c.War ? 1 : 0),
        ["stars"] = _ => 0.9,
        ["thief"] = _ => 1,
        ["assassin"] = c => 0.5 + (c.War ? 1.2 : 0),
        ["mastermind"] = _ => 1,
        ["evocation"] = c => 0.6 + c.Threat + (c.War ? 1 : 0),
        ["abjuration"] = _ => 1,
        ["divination"] = _ => 1.1,
        ["berserker"] = c => 0.8 + (c.War ? 1 : 0),
        ["totem"] = _ => 1,
        ["ancestral"] = c => 0.6 + c.Threat,
        ["lore"] = _ => 1.1,
        ["valor"] = c => 0.5 + c.Threat + (c.War ? 1 : 0),
        ["eloquence"] = _ => 1,
        ["champion"] = _ => 1,
        ["battlemaster"] = c => 0.6 + c.Law,
        ["eldritch"] = _ => 0.8,
        ["openhand"] = c => 0.6 + c.Threat,
        ["shadow"] = _ => 0.9,
        ["elements"] = c => 0.5 + (c.War ? 1 : 0) + c.Threat,
        ["hunter"] = c => 0.8 + c.Threat * 1.5,
        ["beastmaster"] = c => 0.6 + c.Forest * 2,
        ["gloom"] = _ => 0.8,
        ["draconic"] = _ => 1,
        ["wildmagic"] = _ => 1,
        ["shadowborn"] = c => 0.6 + (c.War ? 1 : 0) + (0.5 - c.Good),
        ["fiend"] = c => 0.8 + (c.War ? 1 : 0),
        ["archfey"] = _ => 0.9,
        ["oldone"] = _ => 1,
    };

    internal static void Attach()
    {
        foreach (var cls in D.CLASSES.Values())
            foreach (var sc in cls.Subclasses)
                sc.Pick = P.TryGetValue(sc.Id, out var f) ? f : throw new InvalidOperationException($"no pick for subclass {sc.Id}");
    }
}
