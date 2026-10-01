using System;
using System.Collections.Generic;
using System.Linq;

// Goblin istilası: büyüyen, yayılan, üst kademeye evrilen kamplar. Port of src/sim/monsters.ts.

namespace FD.Macro;

public static class Monsters
{
    private static readonly JsObj<double> CAP = new JsObj<double> { ["goblin"] = 14, ["hobgoblin"] = 10, ["bugbear"] = 4, ["pirate"] = 14 };
    private static readonly JsObj<double> GROW = new JsObj<double> { ["goblin"] = 0.035, ["hobgoblin"] = 0.028, ["bugbear"] = 0.008, ["pirate"] = 0.022 };

    /// <summary>Kamp türünün savaşçıları: n canavar (+ bugbear dışında önder), hepsi kötü (evil).</summary>
    public static List<Combatant> MonsterSide(string kind, double n, bool boss, string side)
    {
        var cs = new List<Combatant>();
        UnitStats @base = kind == "goblin" ? D.MONSTERS["goblin"] : kind == "hobgoblin" ? D.MONSTERS["hobgoblin"] : kind == "pirate" ? D.MONSTERS["pirate"] : D.MONSTERS["bugbear"];
        for (int i = 0; i < n; i++) { var u = Combat.Unit(@base, side, "monster"); u.Evil = true; cs.Add(u); }
        if (boss && kind != "bugbear") { var u = Combat.Unit(kind == "goblin" ? D.MONSTERS["goblinBoss"] : kind == "pirate" ? D.MONSTERS["pirateCaptain"] : D.MONSTERS["hobCaptain"], side, "boss"); u.Evil = true; cs.Add(u); }
        return cs;
    }

    /// <summary>Canavar türünün adı: çoğul ("Goblinler") ya da tekil ("Goblin").</summary>
    public static string MonsterName(string kind, bool plural = true) =>
        kind == "goblin" ? (plural ? "Goblinler" : "Goblin") : kind == "hobgoblin" ? (plural ? "Hobgoblinler" : "Hobgoblin") : kind == "pirate" ? (plural ? "Korsanlar" : "Korsan") : (plural ? "Bugbearlar" : "Bugbear");

    /// <summary>Kamptan akında olan (ölmemiş raid ajanlarındaki) canavar sayısı.</summary>
    public static double CampAway(Sim s, Camp c)
    {
        double n = 0;
        foreach (var a in J.Filter(s.W.Agents, a => a.Kind == "raid" && a.From == c.Id && !J.T(a.Dead))) n = n + (a.Troops ?? 0);
        return n;
    }

    /// <summary>Günlük kamp güncellemesi: büyüme, şef, yayılma, hobgoblin evrimi ve işgali, korsan kaptanı, akınlar, bugbear ini, yeni kabile.</summary>
    public static void CampsTick(Sim s)
    {
        var w = s.W;
        var alive = J.Filter(w.Camps, c => c.Alive);
        foreach (var c in alive)
        {
            double away = CampAway(s, c);
            c.GrowthAcc += J.N(GROW.Get(c.Kind)) + JsMath.Min(0.02, c.Loot / 3000);
            if (c.GrowthAcc >= 1 && c.Count + away < J.N(CAP.Get(c.Kind))) { c.GrowthAcc = 0; c.Count++; }
            bool bossAway = J.Some(w.Agents, a => a.Kind == "raid" && a.From == c.Id && J.T(a.Boss));
            if (c.Kind == "goblin" && !c.Boss && !bossAway && !c.HadBoss && c.Count >= 11)
            {
                c.HadBoss = true; c.Boss = true;
                s.Log("lair", $"{Tr.Ek(c.Name, "da")} goblinlerin başına acımasız bir şef geçti.", tile: c.Tile, cause: $"Kamp kalabalıklaştı ({J.S(c.Count)} goblin)", major: true);
            }
            // yayılma
            if (c.Kind == "goblin" && c.Count >= 12 && J.Filter(w.Camps, x => x.Alive).Count < 10 && s.Rng.Chance(1.0 / 200))
            {
                var avoid = J.Map(J.Filter(w.Settlements, x => x.Alive), x => x.Tile);
                avoid.AddRange(J.Map(alive, x => x.Tile));
                var near = J.Filter(s.G.Within(c.Tile, 10), t => s.G.Dist(t, c.Tile) >= 6);
                int t = near.Count > 0 ? PickNear(s, near, avoid) : -1;
                if (t >= 0)
                {
                    var nc = WorldGen.MakeCamp(s.Id(), "goblin", t, FreshName(s, WorldGen.GOBLIN_CAMP_NAMES), s.Day, s.Rng);
                    nc.NextRaid = s.Day + s.Rng.Int(100, 180);
                    c.Count -= 5; nc.Count = 5;
                    w.Camps.Add(nc); w.Tiles[t].Camp = nc.Id; w.Tiles[t].Owner = -1;
                    s.Metric("campSpread");
                    s.Log("lair", $"{c.Name} kalabalıklaştı; goblinler {nc.Name} adıyla yeni bir kamp kurdu.", tile: t, major: true, cause: "Goblin istilası yayılıyor");
                }
            }
            // hobgoblin karakoluna evrilme
            int goblinCamps = J.Filter(alive, x => x.Kind == "goblin").Count;
            int hobs = J.Filter(alive, x => x.Kind == "hobgoblin").Count;
            if (c.Kind == "goblin" && hobs < 3 && s.Day > 4 * Sim.YEAR && (s.Day - c.Founded > 3 * Sim.YEAR || goblinCamps >= 4) && c.Count >= 10 && s.Rng.Chance(1.0 / 500))
            {
                c.Kind = "hobgoblin";
                c.Name = FreshName(s, WorldGen.HOB_NAMES);
                c.Count = JsMath.Max(6, Math.Floor(c.Count * 0.6));
                c.Boss = true; c.HadBoss = true;
                s.Metric("hobgoblin");
                s.Log("lair", $"Hobgoblin lejyonerleri kampı ele geçirdi: {c.Name} kuruldu.", tile: c.Tile, major: true, cause: "Uzun süre temizlenmeyen goblin kampı disiplinli bir orduya dönüştü");
            }
            // hobgoblin işgali: terk edilmiş yerleşim ya da sahipsiz yatak
            if (c.Kind == "hobgoblin" && c.Count >= 10 && hobs < 4 && J.Filter(w.Camps, x => x.Alive).Count < 7 && s.Rng.Chance(1.0 / 300))
            {
                var cand = J.Map(J.Filter(w.Settlements, x => !x.Alive && s.G.Dist(x.Tile, c.Tile) <= 12 && w.Tiles[x.Tile].Owner < 0 && w.Tiles[x.Tile].Camp == null && w.Tiles[x.Tile].InnZone == null), x => x.Tile);
                var depTiles = new List<int>();
                foreach (var d in J.Filter(w.Deposits, d => !d.Depleted)) depTiles.AddRange(d.Tiles);
                cand.AddRange(J.Filter(depTiles, t => s.G.Dist(t, c.Tile) <= 10 && s.G.Dist(t, c.Tile) >= 4 && w.Tiles[t].Owner < 0 && w.Tiles[t].Camp == null && w.Tiles[t].InnZone == null && w.Tiles[t].Terrain != "mountain"));
                if (cand.Count > 0)
                {
                    int t = s.Rng.Pick(cand);
                    var nc = WorldGen.MakeCamp(s.Id(), "hobgoblin", t, FreshName(s, WorldGen.HOB_NAMES), s.Day, s.Rng);
                    c.Count -= 5; nc.Count = 5;
                    w.Camps.Add(nc); w.Tiles[t].Camp = nc.Id;
                    s.Metric("occupation");
                    s.Log("lair", $"Hobgoblinler {(w.Tiles[t].Deposit >= 0 ? "sahipsiz bir yatağı" : "terk edilmiş bir yerleşimi")} işgal etti.", tile: t, major: true, cause: $"{c.Name} genişliyor");
                }
            }
            if (c.Kind == "pirate")
            {
                if (!c.Boss && !bossAway && c.Count >= 9 && s.Rng.Chance(1.0 / 120)) { c.Boss = true; c.Captain = Sea.NewCaptain(s); s.Log("lair", $"{Tr.Ek(c.Name, "da")} korsanlar yeni kaptanlarını seçti: {c.Captain}.", tile: c.Tile, major: true, cause: $"Koy kalabalıklaştı ({J.S(c.Count)} korsan)"); }
                if (s.Day >= c.NextRaid && c.Count >= 5) Sea.LaunchPirates(s, c);
                continue;
            }
            if (s.Day >= c.NextRaid && c.Count >= (c.Kind == "goblin" ? 7 : c.Kind == "hobgoblin" ? 6 : 2)) LaunchRaid(s, c);
        }
        // bugbear ini
        if (s.Year >= 4 && !J.Some(alive, c => c.Kind == "bugbear") && s.Rng.Chance(1.0 / 500))
        {
            var forest = new List<int>();
            for (int i = 0; i < w.Tiles.Count; i++)
            {
                var t = w.Tiles[i];
                int v = (t.Terrain == "forest" || t.Terrain == "oldforest") && t.Owner < 0 && t.Camp == null && t.InnZone == null && !J.T(t.Isle) ? i : -1;
                if (v >= 0) forest.Add(v);
            }
            var avoid = J.Map(J.Filter(w.Settlements, x => x.Alive), x => x.Tile);
            int tt = PickNear(s, forest, avoid, 9);
            if (tt >= 0)
            {
                var c = WorldGen.MakeCamp(s.Id(), "bugbear", tt, FreshName(s, WorldGen.BUGBEAR_NAMES), s.Day, s.Rng);
                w.Camps.Add(c); w.Tiles[tt].Camp = c.Id;
                s.Log("lair", $"Ormanın derinliklerinde bir bugbear ini belirdi: {c.Name}.", tile: tt, major: true, cause: "Yolcuları ve kahramanları pusuya düşürürler");
            }
        }
        // her şey temizlendiyse yeni kabile
        if (alive.Count < 2 && s.Day - LastCleared(w) > Sim.YEAR * (alive.Count != 0 ? 2 : 1) && s.Rng.Chance(1.0 / 150))
        {
            var avoid = J.Map(J.Filter(w.Settlements, x => x.Alive), x => x.Tile);
            int t = WorldGen.PickCampTile(s.G, w.Tiles, s.Rng, avoid, 10);
            if (t < 0) t = WorldGen.PickCampTile(s.G, w.Tiles, s.Rng, avoid, 7); // kalabalık dünyada kıyıda köşede de olsa yer bulurlar
            if (t >= 0)
            {
                var c = WorldGen.MakeCamp(s.Id(), "goblin", t, FreshName(s, WorldGen.GOBLIN_CAMP_NAMES), s.Day, s.Rng);
                w.Camps.Add(c); w.Tiles[t].Camp = c.Id;
                s.Log("lair", $"Dağlardan yeni bir goblin kabilesi indi ve {Tr.Ek(c.Name, "i")} kurdu.", tile: t, major: true, cause: "Boşalan topraklar yeni yağmacıları çekti");
            }
        }
    }

    /// <summary>JS <c>Math.max(-9999, ...w.camps.map((c) => c.clearedDay ?? -9999))</c>.</summary>
    private static double LastCleared(World w)
    {
        var xs = new double[w.Camps.Count + 1];
        xs[0] = -9999;
        for (int i = 0; i < w.Camps.Count; i++) xs[i + 1] = w.Camps[i].ClearedDay ?? -9999;
        return JsMath.Max(xs);
    }

    private static int PickNear(Sim s, List<int> cands, List<int> avoid, double minD = 7)
    {
        int best = -1; double bs = double.NegativeInfinity;
        foreach (int i in cands)
        {
            var t = s.W.Tiles[i];
            if (t.Terrain == "water" || t.Terrain == "mountain" || t.Owner >= 0 || t.Camp != null || J.T(t.Isle) || t.InnZone != null) continue;
            double d = avoid.Count > 0 ? J.MinOf(avoid, a => s.G.Dist(a, i)) : 20;
            if (d < minD) continue;
            double sc = JsMath.Min(d, 12) + s.Rng.Next() * 3;
            if (sc > bs) { bs = sc; best = i; }
        }
        return best;
    }

    private static readonly string[] ROMAN = { "II", "III", "IV", "V", "VI", "VII", "VIII" };

    private static string FreshName(Sim s, List<string> list)
    {
        var used = new HashSet<string>();
        foreach (var c in s.W.Camps) used.Add(c.Name);
        var free = J.Find(list, n => !used.Contains(n));
        if (J.T(free)) return free;
        foreach (var r in ROMAN) foreach (var n in list) if (!used.Contains($"{n} {r}")) return $"{n} {r}";
        return $"{(list.Count > 0 ? list[0] : "undefined")} {s.W.Camps.Count}";
    }

    /// <summary>Kamptan akın: han, korumasız çıkarma yapısı, kervan ya da yerleşim hedefi seçip raid ajanı yollar.</summary>
    public static void LaunchRaid(Sim s, Camp c)
    {
        var w = s.W;
        c.NextRaid = s.Day + s.Rng.Int(c.Kind == "hobgoblin" ? 110 : 90, 170);
        int range = c.Kind == "bugbear" ? 9 : 16;
        var caravans = J.Filter(w.Agents, a => (a.Kind == "caravan" || (c.Kind == "bugbear" && (a.Kind == "party" || a.Kind == "scout" || a.Kind == "hero"))) && !J.T(a.Dead) && a.Hull == null
            && !J.Some(J.Slice(a.Path, a.Step, a.Step + 4), t => J.T(w.Tiles[t].Sea)) && PathDist(s, a, c.Tile) <= range - 3);
        var setts = c.Kind == "bugbear" ? new List<Settlement>() : J.Filter(w.Settlements, x => x.Alive && s.G.Dist(x.Tile, c.Tile) <= range && s.Pop(x) >= 10);
        int targetTile = -1, to = -1;
        string purpose = "";
        // korumasız çıkarma yapıları: iskele, tarla, maden, av kampı...
        var exts = new List<int>();
        if (c.Kind != "bugbear")
        {
            for (int i = 0; i < w.Tiles.Count; i++)
            {
                var t = w.Tiles[i];
                if (t.Ext != null && s.ExtWorking(t) && t.Owner >= 0 && s.G.Dist(i, c.Tile) <= range - 2) exts.Add(i);
            }
            exts = J.Filter(exts, i => { var st = s.Settlement(w.Tiles[i].Owner); return st != null && st.Alive && s.G.Dist(i, st.Tile) >= 1 && s.E(w.Civs[st.Civ], "pact") <= 0; });
        }
        bool pickExt = exts.Count > 0 && (setts.Count == 0 || s.Rng.Chance(0.55));
        var inns = c.Kind == "bugbear" ? new List<Inn>() : J.Filter(w.Inns, i => i.Alive && s.G.Dist(i.Tile, c.Tile) <= range - 2);
        if (inns.Count > 0 && s.Rng.Chance(0.2))
        {
            var inn = J.Sort(inns, (a, b) => s.G.Dist(a.Tile, c.Tile) - s.G.Dist(b.Tile, c.Tile))[0];
            targetTile = inn.Tile; to = inn.Id; purpose = "inn";
        }
        else if (pickExt && !(caravans.Count > 0 && s.Rng.Chance(0.4)))
        {
            int t = s.Rng.Weighted(exts, i => { var st = s.Settlement(w.Tiles[i].Owner); return (1 + s.G.Dist(i, st.Tile)) / (1 + st.Soldiers * 0.5) / (1 + s.G.Dist(i, c.Tile) * 0.15); });
            targetTile = t; to = t; purpose = "ext";
        }
        else if (caravans.Count > 0 && (s.Rng.Chance(0.65) || setts.Count == 0))
        {
            var cv = s.Rng.Pick(caravans);
            targetTile = cv.Path[Math.Min(cv.Path.Count - 1, cv.Step + 3)]; to = cv.Id; purpose = "caravan";
        }
        else if (setts.Count > 0)
        {
            var st = s.Rng.Weighted(setts, x => (s.E(s.W.Civs[x.Civ], "pact") > 0 ? 0.25 : 1) / (1 + x.Soldiers * 3 + s.Pop(x) * 0.4 + (J.T(x.Civics.Get("palisade")) ? 6 : 0) + (J.T(x.Civics.Get("stonewall")) ? 12 : 0)) / (1 + s.G.Dist(x.Tile, c.Tile) * 0.1));
            targetTile = st.Tile; to = st.Id; purpose = "settlement";
        }
        if (targetTile < 0) return;
        double n = c.Kind == "bugbear" ? c.Count : JsMath.Min(Math.Ceiling(c.Count * 0.5), 3 + Math.Floor(s.Year / 2.0));
        bool boss = c.Boss && c.Count >= 10 && s.Rng.Chance(0.5);
        var path = s.Path(c.Tile, targetTile);
        if (path == null) return;
        c.Count -= n;
        if (boss) c.Boss = false;
        s.W.Agents.Add(new Agent { Id = s.Id(), Kind = "raid", Civ = -1, Path = path, Step = 0, Progress = 0, Speed = c.Kind == "bugbear" ? 1.1 : 0.9, Troops = n, From = c.Id, To = to, Purpose = purpose, Boss = boss, Monster = c.Kind, TargetTile = targetTile });
    }

    /// <summary>JS <c>s.g.dist(a.path[Math.min(a.step, a.path.length - 1)], tile)</c>: NaN when the path is empty (undefined tile).</summary>
    private static double PathDist(Sim s, Agent a, int tile)
    {
        int k = Math.Min(a.Step, a.Path.Count - 1);
        return k >= 0 && k < a.Path.Count ? s.G.Dist(a.Path[k], tile) : double.NaN;
    }
}
