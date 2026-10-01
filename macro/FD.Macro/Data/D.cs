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
    /// <summary>Faz 1 C3: kent tüketimi, kademe başına (0 kamp … 3 şehir): kişi başı günlük bira ve alet; "bread": gıdanın
    /// ekmekten karşılanması gereken payı</summary>
    public static List<JsObj<double>> TOWN_NEEDS;
    // ---- classes.ts
    public static JsObj<RaceDef> RACES;
    public static JsObj<ClassDef> CLASSES;
    public static List<string> CLASS_IDS;
    public static JsObj<UnitDef> UNITS;
    public static JsObj<MonsterDef> MONSTERS;
    // ---- heroes.ts
    public static JsObj<string> HERO_CLASS_TR;
    public static List<string> HERO_CLASS_IDS;
    public static List<string> STATS;
    public static JsObj<HeroClassDef> HERO_CLASSES;
    public static List<double> XP_LEVELS;
    public static JsObj<List<string>> HERO_NAMES;
    /// <summary>ırka göre soyad / lakap havuzu (Faz 1 A3a; tam ad "Ad Soyad")</summary>
    public static JsObj<List<string>> HERO_SURNAMES;
    /// <summary>ırka göre yaş: [yola çıkış en genç, en yaşlı, yaşlılık başlangıcı, en uzun ömür] (yıl)</summary>
    public static JsObj<List<double>> HERO_AGE;
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
            if (!doc.RootElement.TryGetProperty(f.Name, out var el))
                throw new InvalidOperationException($"data.json has no {f.Name}");
            f.SetValue(null, el.Deserialize(f.FieldType, Json.Options));
        }
    }

    /// <summary>Forces loading (call once at startup to fail fast).</summary>
    public static void Init() { }

    // ---- functions from the data modules

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
