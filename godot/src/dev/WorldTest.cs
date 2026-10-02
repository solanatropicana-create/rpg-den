using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using FD.Game;
using FD.Sim.Life;
using FD.World;
using M = FD.Macro;

namespace FD.Dev;

/// <summary>
/// Faz 2 (--worldtest=N): the macro link. Checks that the region is bound to a macro village/inn/goblin camp, that the 1:1
/// village takes its name, population, races and trades from the sim (no children), that the camp has an open quest, and that
/// N local days step the macro world N days (one per local midnight). Prints PASS/FAIL and exits with 0/1.
/// </summary>
public static class WorldTest
{
    public static void Run(Node host, Region region, int days)
    {
        var s = region.Session;
        var life = region.Life;
        bool ok = true;
        void Check(string name, bool pass, string info)
        {
            ok &= pass;
            GD.Print($"[WorldTest] {name}: {info} {(pass ? "PASS" : "FAIL")}");
        }
        GD.Print($"[WorldTest] {s.Describe()}");
        var link = s.Link;
        var v = s.VillageSettlement; var inn = s.Inn; var cp = s.Camp;
        Check("bağ", link != null && v != null && cp != null, $"köy {v?.Name} (kademe {v?.Tier}), han {inn?.Name ?? "yok"}, kamp {cp?.Name} ({cp?.Kind}, {cp?.Count:F0})");
        var (vn, inName, cn) = s.Names();
        Check("adlar", life.VillageName == vn && life.InnName == inName && life.CampName == cn, $"{life.VillageName} / {life.InnName} / {life.CampName}");
        var vi = s.Village();
        var spec = RegionBind.Spec(s);
        int residents = life.People.Count(p => p.Household >= 0 && p.Home >= 0 && life.Places[p.Home].Kind == PlaceKind.Home);
        Check("nüfus simden", residents == Math.Clamp(spec.Residents, 8, 46), $"sim {vi.Pop:F0} → köylü {residents} (beklenen {spec.Residents})");
        int children = life.People.Count(p => p.Role == Role.Child);
        Check("çocuk yok", children == 0, $"{children} çocuk");
        var races = life.People.Where(p => p.Household >= 0 && p.Role is not (Role.Innkeeper or Role.InnServant or Role.StableHand)).GroupBy(p => p.Race).Select(g => $"{g.Key} {g.Count()}");
        var simRaces = vi.Races.Keys.ToHashSet();
        bool racesOk = life.People.Where(p => p.Household >= 0 && p.Role is not (Role.Innkeeper or Role.InnServant or Role.StableHand)).All(p => simRaces.Contains(p.Race));
        Check("ırklar simden", racesOk, $"köy [{string.Join(", ", Session.RacesSorted(vi))}] → 1:1 [{string.Join(", ", races)}]");
        var roles = life.People.Where(p => p.Household >= 0).GroupBy(p => p.Role).Select(g => $"{g.Key} {g.Count()}");
        GD.Print($"[WorldTest] işler: sim [{string.Join(", ", vi.Jobs.OrderByDescending(kv => kv.Value).Select(kv => $"{kv.Key} {kv.Value:F0}"))}] → 1:1 [{string.Join(", ", roles)}]");
        int gob = life.People.Count(p => p.Role == Role.Goblin);
        Check("goblinler simden", gob == spec.Goblins + (spec.GoblinBoss ? 1 : 0), $"kamp {cp?.Count:F0}{(cp != null && cp.Boss ? " + şef" : "")} → 1:1 {gob}");
        var q = M.Local.CampQuest(s.Macro);
        var taken = M.Local.TakenQuest(s.Macro);
        Check("kamp ilanı", q != null || taken != null,
            q != null ? $"{q.Bounty:F0} altın, {(q.Inn != null ? "han panosunda" : "tavernalarda")}"
            : taken != null ? $"{taken.Value.q.Bounty:F0} altınlık ilanı {string.Join(", ", taken.Value.heroes.Select(h => h.Name))} aldı, yolda" : "yok");
        Check("devlet", vi.CivName != null, $"{vi.CivName} ({vi.GovName}), {vi.RulerTitle}, meşruiyet {vi.Legit:F0}, istikrar {vi.Stability:F0}, durum {vi.StatusName ?? "olağan"}");

        // days: one macro step per local midnight
        int d0 = s.Macro.W.Day, ld0 = GameClock.Day;
        var sw = System.Diagnostics.Stopwatch.StartNew();
        int steps = 0;
        for (int d = 1; d <= days; d++)
        {
            GameClock.Advance(24 * 3600 / Math.Max(1f, GameClock.TimeScale));
            steps += s.SyncDay(GameClock.Day);
        }
        double ms = sw.Elapsed.TotalMilliseconds;
        Check("gün adımı", s.Macro.W.Day - d0 == GameClock.Day - ld0 && steps == days, $"yerel {GameClock.Day - ld0} gün → makro {s.Macro.W.Day - d0} gün, adım başına {ms / Math.Max(1, steps):F0} ms");
        var news = M.Local.News(s.Macro, d0 - 30, 6);
        GD.Print($"[WorldTest] haberler ({news.Count}): " + string.Join(" | ", news.Select(e => $"g{e.Day} {e.Text}")));
        var heroes = M.Local.InnHeroes(s.Macro);
        GD.Print($"[WorldTest] handaki kahramanlar: {string.Join(", ", heroes.Select(h => $"{h.Name} ({h.Race} {h.Cls} Sv{h.Level})"))}");
        GD.Print(ok ? "WORLDTEST PASS" : "WORLDTEST FAIL");
        host.GetTree().Quit(ok ? 0 : 1);
    }
}
