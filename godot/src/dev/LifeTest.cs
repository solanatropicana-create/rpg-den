using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;
using FD.Sim.Life;
using FD.World;
using V2 = System.Numerics.Vector2;

namespace FD.Dev;

/// <summary>
/// Headless life-simulation check (--lifetest=N): runs N game days in fixed real-time steps and reports
/// how many villagers are outside in the village by day and by night, anyone stuck, anyone walking through
/// a building, route fallbacks and what people are doing at a few times of day. PASS/FAIL + exit code.
/// </summary>
public static class LifeTest
{
    public static void Run(Node host, Region region, int days)
    {
        var sim = region.Life;
        var sw = System.Diagnostics.Stopwatch.StartNew();
        float ts = GameClock.TimeScale;
        sim.TimeScale = ts;
        double start = Math.Floor(GameClock.TotalHours / 24.0) * 1440.0 + 5 * 60;   // day start 05:00
        const float dt = 0.2f;
        double now = start;
        var center = new V2(RegionSpec.VillageCenter.X, RegionSpec.VillageCenter.Y);
        float maxDay = 0, maxNight = 0, sumDay = 0; int nDay = 0;
        var lastPos = new Dictionary<int, (V2 p, double since)>();
        int maxCluster = 0, aimless = 0; string clusterAt = "", nightWho = "";
        var stuck = new Dictionary<int, string>();
        int wallHits = 0, samples = 0;
        var wallWho = new Dictionary<string, int>();
        var snapshots = new StringBuilder();
        double nextSample = now;
        var dist = new Dictionary<int, float>();
        var prev = new Dictionary<int, V2>();
        sim.Update(dt, now);
        while (now < start + days * 1440.0)
        {
            now += dt * ts / 60.0;
            sim.Update(dt, now);
            foreach (var p in sim.People)
            {
                if (!p.Present) continue;
                if (prev.TryGetValue(p.Id, out var q) && p.Motion == Motion.Walking) dist[p.Id] = dist.GetValueOrDefault(p.Id) + V2.Distance(q, p.Pos);
                prev[p.Id] = p.Pos;
            }
            if (now < nextSample) continue;
            nextSample = now + 10;   // every 10 game minutes
            float h = (float)((now % 1440) / 60.0);
            var (o, t) = sim.CountOutside(center, 95f);
            float ratio = t > 0 ? (float)o / t : 0;
            if (h >= 9 && h < 17) { maxDay = MathF.Max(maxDay, ratio); sumDay += ratio; nDay++; }
            if ((h >= 23 || h < 4) && ratio > maxNight)
            {
                maxNight = ratio;
                nightWho = string.Join(", ", sim.People.Where(p => p.Visible && !p.IsVisitor && p.Role != Role.Goblin && V2.Distance(p.Pos, center) < 95f).Select(p => $"{p.Name}:{p.Role}/{p.Act?.Label}/{p.Motion}/ev {(p.Home >= 0 ? V2.Distance(p.Pos, sim.Places[p.Home].Door) : -1):F0} m [{string.Join("; ", p.Log)}]").Take(6));
            }
            foreach (var p in sim.People)
            {
                if (!p.Visible) { lastPos.Remove(p.Id); continue; }
                samples++;
                var hit = sim.ObstacleAt(p.Pos, 0.35f);
                if (hit != null)
                {
                    wallHits++;
                    string k = $"{p.FullName} ({p.Act?.Label}) in {hit}";
                    wallWho[k] = wallWho.GetValueOrDefault(k) + 1;
                }
                if (p.Motion == Motion.Walking && p.Wait <= 0)
                {
                    if (lastPos.TryGetValue(p.Id, out var lp) && V2.Distance(lp.p, p.Pos) < 0.3f)
                    {
                        if (now - lp.since > 30) stuck[p.Id] = $"{p.FullName}: {p.Act?.GoLabel} @({p.Pos.X:F0},{p.Pos.Y:F0})";
                    }
                    else lastPos[p.Id] = (p.Pos, now);
                }
                else lastPos.Remove(p.Id);
            }
            // crowding: most people within 5 m of one visible person (seated groups at tables/benches count too)
            foreach (var p in sim.People)
            {
                if (!p.Visible) continue;
                int n = 0;
                // Faz 2: the goblin band (up to 10) crowds its own fire; the crowding rule is about people of the village and the road
                if (p.Role != Role.Goblin) foreach (var q2 in sim.People) if (q2.Visible && q2.Role != Role.Goblin && V2.DistanceSquared(q2.Pos, p.Pos) < 25f) n++;
                if (n > maxCluster)
                {
                    maxCluster = n;
                    var who = sim.People.Where(q2 => q2.Visible && q2.Role != Role.Goblin && V2.DistanceSquared(q2.Pos, p.Pos) < 25f)
                        .GroupBy(q2 => q2.Motion == Motion.Walking ? "yürüyor" : q2.Act?.Label ?? "?").Select(g => $"{g.Key}×{g.Count()}");
                    clusterAt = $"{H.Clock(now)} ({p.Pos.X:F0},{p.Pos.Y:F0}): {string.Join(", ", who)}";
                }
                if (p.Act == null || string.IsNullOrEmpty(p.Act.Reason)) aimless++;
            }
            int hh = (int)h, mm = (int)((h - hh) * 60 + 0.5f);
            if ((hh == 10 || hh == 15 || hh == 20 || hh == 1) && mm < 10 && now < start + 1440)
            {
                var groups = sim.People.Where(p => p.Present).GroupBy(p => p.Act?.Label ?? "?").OrderByDescending(g => g.Count());
                snapshots.AppendLine($"  [{hh:00}:{mm:00}] köyde dışarıda {o}/{t}: " + string.Join("; ", groups.Take(9).Select(g => $"{g.Key}×{g.Count()}")));
            }
        }
        float avgDay = nDay > 0 ? sumDay / nDay : 0;
        var comps = sim.Graph.Components(out int nComp);
        if (nComp > 1)
        {
            var sizes = new int[nComp];
            var sample = new V2[nComp];
            for (int i = 0; i < comps.Length; i++) { sizes[comps[i]]++; sample[comps[i]] = sim.Graph.Nodes[i]; }
            for (int c = 0; c < nComp; c++) GD.Print($"[LifeTest] ağ parçası {c}: {sizes[c]} düğüm, örnek ({sample[c].X:F0},{sample[c].Y:F0})");
        }
        bool okDay = avgDay <= 0.45f, okNight = maxNight <= 0.10f, okStuck = stuck.Count == 0, okWall = samples == 0 || wallHits < samples * 0.002f;
        bool okCluster = maxCluster <= 8, okAim = aimless == 0;
        // Faz 2: çocuk ve köpek yok; köyün nüfusu simden (RegionBind.Spec)
        int children = sim.People.Count(p => p.Role == Role.Child);
        int residents = sim.People.Count(p => p.Household >= 0 && p.Home >= 0 && sim.Places[p.Home].Kind == PlaceKind.Home);
        int want = region.Session != null ? Math.Clamp(FD.Game.RegionBind.Spec(region.Session).Residents, 8, 46) : residents;
        bool okKids = children == 0, okPop = residents == want;
        var r = new StringBuilder();
        r.AppendLine($"[LifeTest] {days} gün, {sim.People.Count} kişi, {sim.Places.Count} yer, ağ {sim.Graph.Nodes.Count} düğüm / {nComp} parça, süre {sw.ElapsedMilliseconds} ms");
        r.AppendLine($"[LifeTest] köyde gündüz (09–17) dışarıda: ort %{avgDay * 100:F0} (≤45), en çok %{maxDay * 100:F0} {(okDay ? "PASS" : "FAIL")}");
        r.AppendLine($"[LifeTest] en kalabalık 5 m'lik öbek: {maxCluster} kişi (≤8) {(okCluster ? "PASS" : "FAIL")} — {clusterAt}");
        r.AppendLine($"[LifeTest] nedensiz (amaçsız) dışarıdaki örnek: {aimless} {(okAim ? "PASS" : "FAIL")}");
        r.AppendLine($"[LifeTest] köyde gece (23–04) dışarıda: en çok %{maxNight * 100:F0} (≤10) {(okNight ? "PASS" : "FAIL")} {nightWho}");
        r.AppendLine($"[LifeTest] takılan: {stuck.Count} {(okStuck ? "PASS" : "FAIL")} {string.Join(" | ", stuck.Values.Take(6))}");
        r.AppendLine($"[LifeTest] bina içinden geçen örnek: {wallHits}/{samples} {(okWall ? "PASS" : "FAIL")} {string.Join(" | ", wallWho.OrderByDescending(k => k.Value).Take(6).Select(k => k.Key + "×" + k.Value))}");
        r.AppendLine($"[LifeTest] düz çizgi yedek rota: {sim.DirectFallbacks} {string.Join(" | ", sim.FallbackLog)}");
        var walked = dist.Where(k => !sim.People[k.Key].IsVisitor && sim.People[k.Key].Role != Role.Goblin).Select(k => k.Value).ToList();
        if (walked.Count > 0) r.AppendLine($"[LifeTest] köylü günlük yürüme: ort {walked.Average() / days:F0} m, en çok {walked.Max() / days:F0} m");
        r.AppendLine($"[LifeTest] çocuk: {children} {(okKids ? "PASS" : "FAIL")} · köylü {residents} (simden {want}{(region.Session != null ? $"; köyün sim nüfusu {region.Session.Village()?.Pop:F0}" : "")}) {(okPop ? "PASS" : "FAIL")}");
        r.Append(snapshots);
        bool pass = okDay && okNight && okStuck && okWall && okCluster && okAim && okKids && okPop && sim.DirectFallbacks == 0 && nComp == 1;
        r.AppendLine($"[LifeTest] {(pass ? "PASS" : "FAIL")}");
        GD.Print(r.ToString());
        host.GetTree().Quit(pass ? 0 : 1);
    }
}
