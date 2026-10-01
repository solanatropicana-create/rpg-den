using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using FD.Macro;

namespace CombatCheck;

/// <summary>
/// C# twin of golden/ts/combat_check.ts: reads its ndjson ({ k, spec, out } per line), rebuilds each case from
/// `spec` through the real Combat / Monsters code and prints { k, out } per line (compare with combat_compare.py).
/// </summary>
public static class Program
{
    private sealed class Snap
    {
        public string Name, Side;
        public double Hp, MaxHp, Ac, Atk;
        public List<double> Dmg;
        public double Attacks;
        public int? Hero;
        public bool? Boss;
        public string Kind;
        public bool? Evil;
        public double? Smite;
        public bool? Fled;
        public JsObj<double> Uses;
        public double Kills;
        public bool? Rage;
        public double? Temp;
        public int? Grp;
    }

    private sealed class Pre
    {
        public double PowA, PowBvs;
        public List<double> Vs;
        public double AvgA, AvgB, Wc;
    }

    private sealed class CaseOut
    {
        public Pre Pre;
        public Battle Battle;
        public double Rng;
        public List<Snap> Cs;
    }

    private sealed class Line<T>
    {
        public JsonElement K;
        public T Out;
    }

    private static Snap SnapOf(Combatant c) => new Snap
    {
        Name = c.Name, Side = c.Side, Hp = c.Hp, MaxHp = c.MaxHp, Ac = c.Ac, Atk = c.Atk, Dmg = c.Dmg, Attacks = c.Attacks, Hero = c.Hero?.Id, Boss = c.Boss, Kind = c.Kind,
        Evil = c.Evil, Smite = c.Smite, Fled = c.Fled, Uses = c.Uses, Kills = c.Kills, Rage = c.Rage, Temp = c.Temp, Grp = c.Grp,
    };

    // ------------------------------------------------------------ JSON helpers
    private static bool Has(JsonElement e, string n, out JsonElement v) => e.TryGetProperty(n, out v) && v.ValueKind != JsonValueKind.Null;

    private static double Num(JsonElement v)
    {
        if (v.ValueKind == JsonValueKind.String)
        {
            string s = v.GetString();
            return s == "NaN" ? double.NaN : s == "Infinity" ? double.PositiveInfinity : s == "-Infinity" ? double.NegativeInfinity : throw new FormatException(s);
        }
        return v.GetDouble();
    }

    private static double? NumOpt(JsonElement e, string n) => Has(e, n, out var v) ? Num(v) : null;
    private static int? IntOpt(JsonElement e, string n) => Has(e, n, out var v) ? v.GetInt32() : null;
    private static bool? BoolOpt(JsonElement e, string n) => Has(e, n, out var v) ? v.GetBoolean() : null;
    private static string StrOpt(JsonElement e, string n) => Has(e, n, out var v) ? v.GetString() : null;

    private static List<double> Nums(JsonElement a)
    {
        var l = new List<double>();
        foreach (var x in a.EnumerateArray()) l.Add(Num(x));
        return l;
    }

    // ------------------------------------------------------------ spec → combatants (same steps as the TS harness)
    private static Hero MakeHero(JsonElement h)
    {
        var stats = new JsObj<double>();
        foreach (var p in h.GetProperty("stats").EnumerateObject()) stats.Set(p.Name, Num(p.Value));
        return new Hero
        {
            Id = h.GetProperty("id").GetInt32(), Name = StrOpt(h, "name"), Race = StrOpt(h, "race"), Cls = StrOpt(h, "cls"), Level = h.GetProperty("level").GetInt32(),
            Stats = stats, MaxHp = Num(h.GetProperty("maxHp")), Hp = Num(h.GetProperty("hp")), Ac = Num(h.GetProperty("ac")),
            Bonus = Has(h, "bonus", out var b) ? new HeroBonus { Atk = Num(b.GetProperty("atk")) } : null,
        };
    }

    private static UnitStats SrcUnit(JsonElement op)
    {
        string src = op.GetProperty("src").GetString();
        if (src == "inline")
        {
            var u = op.GetProperty("u");
            return new UnitStats { Name = StrOpt(u, "name"), Hp = Num(u.GetProperty("hp")), Ac = Num(u.GetProperty("ac")), Atk = Num(u.GetProperty("atk")), Dmg = Nums(u.GetProperty("dmg")), Attacks = NumOpt(u, "attacks") };
        }
        if (src.StartsWith("UNITS:", StringComparison.Ordinal)) return D.UNITS[src.Substring(6)];
        return D.MONSTERS[src.Substring(9)];
    }

    private static void ApplyPost(Combatant c, JsonElement p)
    {
        if (Has(p, "ac", out var v)) c.Ac += Num(v);
        if (Has(p, "dmg2", out v)) c.Dmg[2] += Num(v);
        if (Has(p, "atk", out v)) c.Atk += Num(v);
        if (Has(p, "smite", out v)) c.Smite = Num(v);
        if (Has(p, "evil", out v) && v.GetBoolean()) c.Evil = true;
        if (Has(p, "rage", out v) && v.GetBoolean()) c.Rage = true;
        if (Has(p, "fled", out v) && v.GetBoolean()) c.Fled = true;
        if (Has(p, "grp", out v)) c.Grp = v.GetInt32();
    }

    private static List<Combatant> Build(IEnumerable<JsonElement> ops)
    {
        var res = new List<Combatant>();
        foreach (var op in ops)
        {
            List<Combatant> cs;
            string kind = op.GetProperty("op").GetString();
            if (kind == "unit")
            {
                var u = SrcUnit(op);
                string side = op.GetProperty("side").GetString(), k = op.GetProperty("kind").GetString();
                double? bh = NumOpt(op, "bonusHp"), ba = NumOpt(op, "bonusAtk");
                cs = new List<Combatant> { bh == null ? Combat.Unit(u, side, k) : ba == null ? Combat.Unit(u, side, k, bh.Value) : Combat.Unit(u, side, k, bh.Value, ba.Value) };
            }
            else if (kind == "monsterSide") cs = Monsters.MonsterSide(op.GetProperty("kind").GetString(), Num(op.GetProperty("n")), op.GetProperty("boss").GetBoolean(), op.GetProperty("side").GetString());
            else cs = new List<Combatant> { Combat.HeroCombatant(MakeHero(op.GetProperty("hero")), op.GetProperty("side").GetString()) };
            if (Has(op, "post", out var p)) foreach (var c in cs) ApplyPost(c, p);
            res.AddRange(cs);
        }
        return res;
    }

    private static BattleOpts Opts(JsonElement o)
    {
        var r = new BattleOpts
        {
            Day = Num(o.GetProperty("day")), Tile = o.GetProperty("tile").GetInt32(), Title = StrOpt(o, "title"), SideA = StrOpt(o, "sideA"), SideB = StrOpt(o, "sideB"), Id = o.GetProperty("id").GetInt32(),
            MaxRounds = NumOpt(o, "maxRounds"), MoraleA = NumOpt(o, "moraleA"), MoraleB = NumOpt(o, "moraleB"), NoRoutA = BoolOpt(o, "noRoutA"), NoRoutB = BoolOpt(o, "noRoutB"),
            FirstStrikeA = NumOpt(o, "firstStrikeA"), FirstStrikeB = NumOpt(o, "firstStrikeB"), MeteorA = BoolOpt(o, "meteorA"), MeteorB = BoolOpt(o, "meteorB"),
            TimeoutWinner = StrOpt(o, "timeoutWinner"), CivA = IntOpt(o, "civA"), CivB = IntOpt(o, "civB"),
        };
        if (Has(o, "groups", out var gs))
        {
            r.Groups = new List<ReplayGroup>();
            foreach (var g in gs.EnumerateArray()) r.Groups.Add(new ReplayGroup { Name = StrOpt(g, "name"), Side = StrOpt(g, "side"), Civ = IntOpt(g, "civ"), Kind = StrOpt(g, "kind") });
        }
        return r;
    }

    // ------------------------------------------------------------ cases
    private static object RunCase(JsonElement spec)
    {
        var cA = Build(spec.GetProperty("A").EnumerateArray());
        var cB = Build(spec.GetProperty("B").EnumerateArray());
        var (pa, pb) = Combat.PowerVs(cA, cB);
        var pre = new Pre { PowA = Combat.PowerOf(cA), PowBvs = Combat.PowerOf(cB, Num(spec.GetProperty("vsAc"))), Vs = new List<double> { pa, pb }, AvgA = Combat.AvgAc(cA), AvgB = Combat.AvgAc(cB), Wc = Combat.WinChance(pa, pb) };
        var rng = new Rng(Num(spec.GetProperty("seed")));
        var b = Combat.ResolveBattle(rng, cA, cB, Opts(spec.GetProperty("opts")));
        var all = new List<Combatant>(cA); all.AddRange(cB);
        return new CaseOut { Pre = pre, Battle = b, Rng = rng.State(), Cs = J.Map(all, SnapOf) };
    }

    private static object RunGrid(string k, JsonElement spec)
    {
        switch (k)
        {
            case "profBonus":
                { var o = new List<double>(); foreach (var l in spec.GetProperty("levels").EnumerateArray()) o.Add(Combat.ProfBonus(l.GetInt32())); return o; }
            case "heroAc":
                { var o = new List<double>(); foreach (var x in spec.GetProperty("in").EnumerateArray()) o.Add(Combat.HeroAc(x[0].GetString(), Num(x[1]), x[2].GetInt32())); return o; }
            case "monsterSide":
                { var o = new List<List<Snap>>(); foreach (var x in spec.GetProperty("in").EnumerateArray()) o.Add(J.Map(Monsters.MonsterSide(x[0].GetString(), Num(x[1]), x[2].GetBoolean(), x[3].GetString()), SnapOf)); return o; }
            case "unit":
                { var o = new List<Snap>(); foreach (var x in spec.GetProperty("in").EnumerateArray()) o.Add(SnapOf(Build(new[] { x })[0])); return o; }
            case "heroCombatant":
                { var o = new List<Snap>(); foreach (var x in spec.GetProperty("in").EnumerateArray()) o.Add(SnapOf(Combat.HeroCombatant(MakeHero(x), "B"))); return o; }
            case "winChance":
                { var o = new List<double>(); foreach (var x in spec.GetProperty("in").EnumerateArray()) o.Add(Combat.WinChance(Num(x[0]), Num(x[1]))); return o; }
            default: throw new InvalidOperationException("unknown grid " + k);
        }
    }

    public static int Main(string[] args)
    {
        D.Init();
        string path = args.Length > 0 ? args[0] : "/tmp/combat_ts.ndjson";
        using var w = new StreamWriter(Console.OpenStandardOutput(), new UTF8Encoding(false), 1 << 16);
        foreach (string line in File.ReadLines(path))
        {
            if (line.Length == 0) continue;
            using var doc = JsonDocument.Parse(line);
            var root = doc.RootElement;
            var k = root.GetProperty("k");
            var spec = root.GetProperty("spec");
            object o = k.ValueKind == JsonValueKind.Number ? RunCase(spec) : RunGrid(k.GetString(), spec);
            w.Write(Json.Serialize(new Line<object> { K = k.Clone(), Out = o }));
            w.Write('\n');
        }
        return 0;
    }
}
