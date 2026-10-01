using System;
using System.Collections.Generic;

// Harness-only stand-ins for Heroes / Will / Inns / Sea (other agents are porting the real ones right now).
// Only what a hero-less world without sea trade can reach is ported (faithfully, from src/sim/*.ts); everything
// else throws, which stops the harness run and names the missing piece. Hero loops keep their TS shape but
// throw if a hero would actually be processed (the harness world never creates heroes).

namespace FD.Macro;

public sealed class NavOpts { public List<int> Embark; public bool Open; public List<int> LandOnly; }
public sealed class CivPathResult { public List<int> Path; public Settlement Hull; }

internal static class Stub
{
    internal static Exception Nope(string what) => new NotImplementedException($"harness stand-in: {what}");
}

public static class Sea
{
    private static readonly double[] PORT_D = { 9, 2, 0, 1, 1.5, 2 };

    // sea.ts pickPort
    public static int PickPort(Sim s, Settlement st)
    {
        var w = s.W;
        int best = -1; double bd = double.PositiveInfinity;
        foreach (int i in s.G.Within(st.Tile, s.RadiusOf(st)))
        {
            var t = w.Tiles[i];
            if (J.T(t.Sea) || t.Terrain == "water" || t.Terrain == "mountain" || (t.Owner != st.Id && i != st.Tile)) continue;
            if (!J.Some(s.G.Neighbors(i), n => J.T(w.Tiles[n].Sea))) continue;
            int dd = s.G.Dist(i, st.Tile);
            double d = (dd < PORT_D.Length ? PORT_D[dd] : double.NaN) + (t.Ext != null ? 1.5 : 0) + (t.Deposit >= 0 ? 1 : 0);
            if (d < bd) { bd = d; best = i; }
        }
        return best;
    }

    public static List<Settlement> Ports(Sim s, Civ c) => J.Filter(s.CivSettlements(c), x => J.T(x.Civics.Get("shipyard")) && x.Port != null);
    public static bool IsFleet(Agent a) => a.Kind == "ship" && (a.Purpose == "fleet" || a.Purpose == "fleet-back");
    private static double HullsOut(Sim s, Settlement st) { double n = 0; foreach (var a in s.W.Agents) if (!J.T(a.Dead) && a.Hull == st.Id && !IsFleet(a)) n++; return n; }
    private static double GalleysOut(Sim s, Settlement st) { double n = 0; foreach (var a in s.W.Agents) if (!J.T(a.Dead) && a.Hull == st.Id) n += a.Galleys ?? 0; return n; }
    public static double FreeHulls(Sim s, Settlement st) => JsMath.Max(0, (st.Ships ?? 0) - HullsOut(s, st));
    public static double FreeGalleys(Sim s, Settlement st) => JsMath.Max(0, (st.Galleys ?? 0) - GalleysOut(s, st));

    public static (double Ships, double Galleys) Fleet(Sim s, Civ c)
    {
        double ships = 0, galleys = 0;
        foreach (var x in s.CivSettlements(c)) { ships += x.Ships ?? 0; galleys += x.Galleys ?? 0; }
        return (ships, galleys);
    }

    public static string HullName(Sim s, Civ c, bool plural = false) => s.Has(c, "shipbuilding") ? (plural ? "kogalar" : "koga") : (plural ? "tekneler" : "tekne");

    public const double EMBARK = 1.5;
    public const double SEA_STEP = 0.5;

    // sea.ts shoreWater
    public static byte[] ShoreWater(Sim s)
    {
        if (s.ShoreW != null) return s.ShoreW;
        var w = s.W; var outp = new byte[w.Tiles.Count];
        for (int i = 0; i < w.Tiles.Count; i++) if (J.T(w.Tiles[i].Sea) && J.Some(s.G.Neighbors(i), n => !J.T(w.Tiles[n].Sea))) outp[i] = 1;
        s.ShoreW = outp;
        return outp;
    }

    // sea.ts navPath
    public static List<int> NavPath(Sim s, int from, int to, NavOpts o)
    {
        string key = $"{from}:{to}:{(o.Open ? 1 : 0)}:{string.Join(",", o.Embark)}:{(o.LandOnly != null ? string.Join(",", o.LandOnly) : "")}";
        var cache = s.NavCache;
        if (cache.TryGetValue(key, out var hit)) return hit;
        var w = s.W; var g = s.G; var shore = ShoreWater(s);
        var emb = new HashSet<int>(o.Embark); HashSet<int> land = o.LandOnly != null ? new HashSet<int>(o.LandOnly) : null;
        var gScore = new Dictionary<int, double>(); var came = new Dictionary<int, int>(); var closed = new HashSet<int>();
        var open = new MinHeap();
        gScore[from] = 0; open.Push(from, g.Dist(from, to) * 0.5);
        bool found = false;
        while (open.Size > 0)
        {
            int cur = open.Pop();
            if (cur == to) { found = true; break; }
            if (closed.Contains(cur)) continue;
            closed.Add(cur);
            bool cs = J.T(w.Tiles[cur].Sea);
            foreach (int n in g.Neighbors(cur))
            {
                bool ns = J.T(w.Tiles[n].Sea);
                double step;
                if (!cs && !ns) step = n == to ? JsMath.Min(s.MoveCost(n), 3) : s.MoveCost(n);
                else if (!cs && ns) { if (!emb.Contains(cur) || (!o.Open && shore[n] == 0)) continue; step = SEA_STEP + EMBARK; }
                else if (cs && ns) { if (!o.Open && shore[n] == 0) continue; step = SEA_STEP; }
                else { if (w.Tiles[n].Terrain == "mountain" || (land != null && !land.Contains(n))) continue; step = JsMath.Min(s.MoveCost(n), 3) + EMBARK; }
                if (!double.IsFinite(step)) continue;
                double t = gScore[cur] + step;
                if (t < (gScore.TryGetValue(n, out var gn) ? gn : double.PositiveInfinity)) { gScore[n] = t; came[n] = cur; open.Push(n, t + g.Dist(n, to) * 0.5); }
            }
        }
        List<int> path = null;
        if (found) { path = new List<int> { to }; int c = to; while (came.TryGetValue(c, out int p)) { c = p; path.Add(c); } path.Reverse(); }
        if (cache.Count > 3000) cache.Clear();
        cache[key] = path;
        return path;
    }

    public static bool HasSea(Sim s, List<int> p) => J.Some(p, t => J.T(s.W.Tiles[t].Sea));

    // sea.ts embarkPort
    public static Settlement EmbarkPort(Sim s, Civ c, List<int> p)
    {
        int k = J.FindIndex(p, t => J.T(s.W.Tiles[t].Sea));
        if (k <= 0) return null;
        int t0 = p[k - 1];
        return J.Find(Ports(s, c), x => x.Port == t0);
    }

    // sea.ts civPath
    public static CivPathResult CivPath(Sim s, Civ c, int from, int to, List<int> landOnly = null)
    {
        var emb = J.Map(J.Filter(Ports(s, c), x => FreeHulls(s, x) > 0), x => x.Port.Value);
        if (!s.Has(c, "boatbuilding") || emb.Count == 0) { var p0 = s.Path(from, to); return p0 != null ? new CivPathResult { Path = p0 } : null; }
        var p = NavPath(s, from, to, new NavOpts { Embark = emb, Open = s.Has(c, "navigation"), LandOnly = landOnly });
        if (p == null) return null;
        if (!HasSea(s, p)) return new CivPathResult { Path = p };
        var hull = EmbarkPort(s, c, p);
        return hull != null ? new CivPathResult { Path = p, Hull = hull } : null;
    }

    // sea.ts shipSpeed
    public static double ShipSpeed(Sim s, Agent a)
    {
        var c = a.Civ >= 0 ? s.W.Civs[a.Civ] : null;
        if (a.Monster == "pirate") return 1.25;
        double v = 0.85;
        if (c != null && s.Has(c, "navigation")) v += 0.2;
        if (c != null && s.Has(c, "seatrade") && (a.Kind == "caravan" || a.Kind == "ship")) v += 0.1;
        if (a.Kind == "ship" && !(a.Purpose != null && a.Purpose.StartsWith("explore", StringComparison.Ordinal))) v += 0.05;
        return v;
    }

    // sea.ts disembark
    public static void Disembark(Sim s, Agent a, int seaTile, int landTile)
    {
        a.Landing = landTile;
        if (a.Kind == "ship") return;
        var home = a.Hull != null ? s.Settlement(a.Hull.Value) : null;
        if (a.Kind == "army" && !J.T(a.Returning)) return;
        if (a.Kind == "army" && J.T(a.Returning)) { a.Hull = null; a.Galleys = null; return; }
        a.Hull = null;
        if (home == null || !home.Alive || home.Port == null) return;
        var c = s.W.Civs[a.Civ];
        var p = NavPath(s, seaTile, home.Port.Value, new NavOpts { Embark = new List<int>(), Open = s.Has(c, "navigation"), LandOnly = new List<int> { home.Port.Value } });
        if (p == null || p.Count < 2) return;
        s.W.Agents.Add(new Agent { Id = s.Id(), Kind = "ship", Civ = a.Civ, Path = p, Step = 0, Progress = 0, Speed = 1, Hull = home.Id, Purpose = "return" });
    }

    // sea.ts seaReturnPath
    public static List<int> SeaReturnPath(Sim s, Agent a, int to)
    {
        if (a.Hull == null || a.Landing == null) return null;
        var c = s.W.Civs[a.Civ];
        var home = s.Settlement(a.Hull.Value);
        List<int> land = home != null && home.Alive && home.Civ == a.Civ && home.Port != null ? new List<int> { home.Port.Value } : null;
        return NavPath(s, Agents.TileOf(a), to, new NavOpts { Embark = new List<int> { a.Landing.Value }, Open = s.Has(c, "navigation"), LandOnly = land })
            ?? NavPath(s, Agents.TileOf(a), to, new NavOpts { Embark = new List<int> { a.Landing.Value }, Open = s.Has(c, "navigation") });
    }

    public static void SeaTick(Sim s) => throw Stub.Nope("Sea.SeaTick");
    public static void ExploreSight(Sim s, Agent a) => throw Stub.Nope("Sea.ExploreSight");
    public static bool ExploreTurn(Sim s, Agent a) => throw Stub.Nope("Sea.ExploreTurn");
    public static bool FleetArrive(Sim s, Agent a) => throw Stub.Nope("Sea.FleetArrive");
    public static string NewCaptain(Sim s) => throw Stub.Nope("Sea.NewCaptain");
    public static void LaunchPirates(Sim s, Camp cp) => throw Stub.Nope("Sea.LaunchPirates");
    public static List<int> PirateReturnPath(Sim s, Agent a, int cove) => throw Stub.Nope("Sea.PirateReturnPath");
    public static void PirateSmuggle(Sim s, Agent a) => throw Stub.Nope("Sea.PirateSmuggle");
}

public static class Heroes
{
    public static void TavernsTick(Sim s) => throw Stub.Nope("Heroes.TavernsTick");
    public static void GainXp(Sim s, Hero h, double xp) => throw Stub.Nope("Heroes.GainXp");
    public static void ConsiderHero(Sim s, Civ c) => throw Stub.Nope("Heroes.ConsiderHero");
    public static void SendHero(Sim s, Hero h, int tile, string then) => throw Stub.Nope("Heroes.SendHero");
    public static void ConsiderQuest(Sim s, Civ c) => throw Stub.Nope("Heroes.ConsiderQuest");
    public static void QuestFailed(Sim s, Quest q) => throw Stub.Nope("Heroes.QuestFailed");
    public static void HeroesTick(Sim s) => throw Stub.Nope("Heroes.HeroesTick");
    public static bool TryRevive(Sim s, Hero h) => throw Stub.Nope("Heroes.TryRevive");
    // Faz 1 A3a: Agents.SyncHeroes → Heroes.AfterBattle; FightCamp → Heroes.QuestDone (no heroes in the harness world)
    public static void AfterBattle(Sim s, List<Combatant> cs, double xpBonus, List<Combatant> foes, string foe, string ctx)
    {
        foreach (var x in cs) if (x.Hero != null) throw Stub.Nope("Heroes.AfterBattle with heroes");
    }
    public static void QuestDone(Sim s, Hero h, Quest q, Camp cp) => throw Stub.Nope("Heroes.QuestDone");
}

public static class Will
{
    public static void Note(Sim s, Hero h, string text) => throw Stub.Nope("Will.Note");
    // will.ts heroLabel / innById / innAt
    public static string HeroLabel(Hero h) => J.T(h.Epithet) ? $"{h.Name} «{h.Epithet}»" : h.Name;
    public static Combatant HeroSide(Hero h, string side, string vs = null, bool alone = false) => throw Stub.Nope("Will.HeroSide");
    public static Inn InnById(Sim s, int? id) => id == null ? null : J.Find(s.W.Inns, i => i.Id == id);
    public static Inn InnAt(Sim s, int tile) { int? id = s.W.Tiles[tile].Inn; return id == null ? null : J.Find(s.W.Inns, i => i.Id == id && i.Alive); }
    public static void ReturnToBase(Sim s, Hero h) => throw Stub.Nope("Will.ReturnToBase");

    // will.ts onHomeBurned (no heroes in the harness world: the loop body never runs)
    public static void OnHomeBurned(Sim s, Settlement st, Camp byCamp = null, Civ byCiv = null)
    {
        foreach (var h in s.W.Heroes)
        {
            if (h.Birth != st.Id || h.State == "dead" || h.State == "gone" || h.State == "retired") continue;
            throw Stub.Nope("Will.OnHomeBurned with heroes");
        }
    }

    public static void AfterCampFight(Sim s, List<Combatant> side, Camp cp, bool won, bool hadBoss, List<BattleRoll> rolls) => throw Stub.Nope("Will.AfterCampFight");
    public static void ArriveGoal(Sim s, Hero h) => throw Stub.Nope("Will.ArriveGoal");
}

public static class Inns
{
    public static string InnName(Inn inn) => $"{inn.Name} Hanı";
    public static void InnsTick(Sim s) => throw Stub.Nope("Inns.InnsTick");
    public static void ConsiderInnBids(Sim s, Civ c) => throw Stub.Nope("Inns.ConsiderInnBids");
    public static void InnRaidArrive(Sim s, Agent a) => throw Stub.Nope("Inns.InnRaidArrive");

    // inns.ts innDefenders (hero-less)
    private static List<Combatant> InnDefenders(Sim s, Inn inn, string side)
    {
        var d = new List<Combatant>();
        foreach (var h in s.W.Heroes) if (h.Civ == -1 && h.State == "tavern" && h.Tavern == inn.Id) throw Stub.Nope("Inns.InnDefenders with heroes");
        for (int i = 0; i < 2; i++) d.Add(Combat.Unit(D.UNITS["militia"], side, "militia"));
        return d;
    }

    // inns.ts innMonsterRaid (hero-less: `heroes` is always empty)
    public static void InnMonsterRaid(Sim s, Agent a)
    {
        var inn = Will.InnById(s, a.To);
        if (inn == null || !inn.Alive) { Agents.RaidReturn(s, a); return; }
        string kind = a.Monster ?? "goblin";
        var cp = J.Find(s.W.Camps, x => x.Id == a.From);
        var def = InnDefenders(s, inn, "A");
        var mons = Monsters.MonsterSide(kind, a.Troops ?? 0, J.T(a.Boss), "B");
        var b = Combat.ResolveBattle(s.Rng, def, mons, new BattleOpts { Id = s.Id(), Day = s.Day, Tile = inn.Tile, Title = $"{InnName(inn)} baskını", SideA = $"{InnName(inn)} misafirleri", SideB = Monsters.MonsterName(kind), MoraleA = 0.6, MoraleB = 0.5, TimeoutWinner = "A" });
        Agents.RecordBattle(s, b);
        Agents.SyncHeroes(s, def, b.Winner == "A" ? 80 : 10);
        a.Troops = J.Filter(mons, x => x.Kind == "monster" && x.Hp > 0).Count;
        a.Boss = J.Some(mons, x => J.T(x.Boss) && x.Hp > 0);
        inn.Raids++;
        s.Metric("innMonsterRaid");
        var heroes = J.Map(J.Filter(def, x => x.Hero != null), x => x.Hero);
        if (b.Winner == "B")
        {
            a.Loot = (a.Loot ?? 0) + Math.Floor(inn.Gold * 0.6);
            RuinInn(s, inn, $"{cp?.Name ?? Monsters.MonsterName(kind)} baskını", b.Id);
        }
        else
        {
            foreach (var h in heroes) if (h.State != "dead") throw Stub.Nope("Inns.InnMonsterRaid with heroes");
            inn.Fame = JsMath.Min(100, inn.Fame + 3);
            InnLife.InnEvent(s, inn, $"{Monsters.MonsterName(kind)} baskını püskürtüldü{(heroes.Count > 0 ? " (…)" : "")}");
            s.Log("raid", $"{InnName(inn)} misafirleri {J.TrLower(Monsters.MonsterName(kind))} baskınını püskürttü{(heroes.Count > 0 ? ": …" : "")}.", tile: inn.Tile, battle: b.Id, major: heroes.Count > 0, cause: $"{cp?.Name ?? "Kamp"} hana yakın");
        }
        Agents.RaidReturn(s, a);
    }

    // inns.ts ruinInn (hero-less)
    public static void RuinInn(Sim s, Inn inn, string why, int? battle = null)
    {
        InnLife.InnEvent(s, inn, $"Han yandı: {why}");
        InnLife.InnScatter(s, inn, "han yanarken kaçtı", true);
        inn.Alive = false; inn.RuinedDay = s.Day; inn.Teacher = null;
        s.Metric("innRuined");
        s.Log("inn", $"{InnName(inn)} yandı ve harabeye döndü.", tile: inn.Tile, battle: battle, major: true, cause: why);
        foreach (var q in s.W.Quests) if (q.Open && q.Civ == -1 && q.Inn == inn.Id) q.Open = false;
        foreach (var h in s.W.Heroes)
        {
            if (h.State == "dead" || h.State == "gone") continue;
            throw Stub.Nope("Inns.RuinInn with heroes");
        }
    }
}
