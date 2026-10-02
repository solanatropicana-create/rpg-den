using System;
using System.Collections.Generic;
using Godot;
using FD.World;

namespace FD.Dev;

/// <summary>
/// Headless world checks: <c>godot --headless --path . -- --selftest</c>. Prints one line per check and
/// "SELFTEST PASS"/"SELFTEST FAIL", exits with 0/1.
/// </summary>
public static class SelfTest
{
    public static async void Run(Node host, Region region)
    {
        var results = new List<(string name, bool ok, string info)>();
        void Check(string name, bool ok, string info = "") => results.Add((name, ok, info));
        var hf = region.Heightfield;

        // --- heights finite and within a sane range
        {
            int bad = 0; float mn = float.MaxValue, mx = float.MinValue;
            foreach (var h in hf.H) { if (!float.IsFinite(h)) bad++; mn = MathF.Min(mn, h); mx = MathF.Max(mx, h); }
            Check("heights finite", bad == 0 && mn > -40 && mx < 120, $"range {mn:F1}..{mx:F1} m, non-finite {bad}");
        }

        // --- stream bed below banks, water between bed and bank
        {
            int bad = 0, n = 0; float worstDepth = 99, minDepth = 99, maxDepth = 0;
            var line = RegionSpec.Stream;
            for (float s = 10; s < line.Length - 10; s += 10)
            {
                Vector2 p = line.PointAt(s);
                float bed = hf.Height(p.X, p.Y), bank = hf.StreamBankAt(s), water = hf.StreamWaterAt(s);
                Vector2 t = line.TangentAt(s), side = new(-t.Y, t.X);
                float hw = hf.StreamHalfWidthAt(s);
                Vector2 b1 = p + side * (hw + 2.5f), b2 = p - side * (hw + 2.5f);
                float bankL = hf.Height(b1.X, b1.Y), bankR = hf.Height(b2.X, b2.Y);
                float depth = MathF.Min(bankL, bankR) - bed;
                n++;
                minDepth = MathF.Min(minDepth, bank - bed); maxDepth = MathF.Max(maxDepth, bank - bed);
                if (!(bed < water - 0.3f && water < MathF.Min(bankL, bankR) - 0.2f && bank - bed >= 1.3f && bank - bed <= 2.8f)) { bad++; worstDepth = MathF.Min(worstDepth, depth); }
            }
            Check("stream bed below banks", bad <= n / 50, $"{n - bad}/{n} samples ok, bed depth {minDepth:F2}..{maxDepth:F2} m");
        }

        // --- roads walkable (slope < 20°), trail < 25°, away from the stream crossing
        {
            float worst = 0; Vector2 at = default;
            var road = RegionSpec.Road;
            for (float s = 0; s < road.Length; s += 2)
            {
                if (MathF.Abs(s - RegionSpec.BridgeRoadS) < 10f) continue;
                Vector2 p = road.PointAt(s);
                float sl = hf.Slope(p.X, p.Y);
                if (sl > worst) { worst = sl; at = p; }
            }
            Check("road walkable (<20°)", worst < 20f, $"max slope {worst:F1}° at {at}");
            float worstT = 0;
            foreach (var line in new[] { RegionSpec.Trail, RegionSpec.CampSpur })
                for (float s = 0; s < line.Length; s += 2)
                {
                    Vector2 p = line.PointAt(s);
                    worstT = MathF.Max(worstT, hf.Slope(p.X, p.Y));
                }
            Check("trail walkable (<25°)", worstT < 25f, $"max slope {worstT:F1}°");
        }

        // --- village gently flat (±2 m around a plane), inn pad flat, ruin top flat & highest
        {
            var c = RegionSpec.VillageCenter;
            int n = 0;
            var pts = new List<(Vector2 p, float h)>();
            for (float x = -80; x <= 80; x += 8)
                for (float z = -80; z <= 80; z += 8)
                {
                    if (x * x + z * z > 80 * 80) continue;
                    var p = c + new Vector2(x, z);
                    if (hf.RoadDistance(p.X, p.Y) < 4) continue;
                    float h = hf.Height(p.X, p.Y);
                    pts.Add((p, h)); n++;
                }
            // least-squares plane h = a + b·dx + c·dz, then the largest residual (spec: ±2 m undulation)
            double sx = 0, sz = 0, sxx = 0, szz = 0, sxz = 0, sh = 0, sxh = 0, szh = 0;
            foreach (var (p, h) in pts)
            {
                double dx = p.X - c.X, dz = p.Y - c.Y;
                sx += dx; sz += dz; sxx += dx * dx; szz += dz * dz; sxz += dx * dz; sh += h; sxh += dx * h; szh += dz * h;
            }
            // solve the 3×3 normal equations with Cramer's rule
            double[,] m = { { n, sx, sz }, { sx, sxx, sxz }, { sz, sxz, szz } };
            double[] rhs = { sh, sxh, szh };
            double Det(double[,] a) => a[0, 0] * (a[1, 1] * a[2, 2] - a[1, 2] * a[2, 1]) - a[0, 1] * (a[1, 0] * a[2, 2] - a[1, 2] * a[2, 0]) + a[0, 2] * (a[1, 0] * a[2, 1] - a[1, 1] * a[2, 0]);
            double d0 = Det(m);
            double[] coef = new double[3];
            for (int col = 0; col < 3; col++)
            {
                var mm = (double[,])m.Clone();
                for (int row = 0; row < 3; row++) mm[row, col] = rhs[row];
                coef[col] = Det(mm) / d0;
            }
            float dev = 0;
            foreach (var (p, h) in pts)
                dev = MathF.Max(dev, MathF.Abs(h - (float)(coef[0] + coef[1] * (p.X - c.X) + coef[2] * (p.Y - c.Y))));
            float grade = (float)Math.Sqrt(coef[1] * coef[1] + coef[2] * coef[2]) * 100f;
            Check("village gentle (±2 m around a plane)", dev < 2.2f, $"max residual {dev:F2} m, plane grade {grade:F1}%");

            float mn = float.MaxValue, mx = float.MinValue;
            float a = Mathf.DegToRad(RegionSpec.InnAngleDeg);
            Vector2 u = new(MathF.Cos(a), MathF.Sin(a)), v = new(-u.Y, u.X);
            for (float i = -19; i <= 19; i += 2)
                for (float j = -14; j <= 14; j += 2)
                {
                    var p = RegionSpec.Inn + u * i + v * j;
                    float h = hf.Height(p.X, p.Y);
                    mn = MathF.Min(mn, h); mx = MathF.Max(mx, h);
                }
            Check("inn pad flat", mx - mn < 0.6f, $"range {mx - mn:F2} m");

            float top = hf.Height(RegionSpec.Ruin.X, RegionSpec.Ruin.Y), topRange = 0, higher = 0;
            for (float x = -5; x <= 5; x += 1)
                for (float z = -5; z <= 5; z += 1)
                    topRange = MathF.Max(topRange, MathF.Abs(hf.Height(RegionSpec.Ruin.X + x, RegionSpec.Ruin.Y + z) - top));
            for (float x = -150; x <= 150; x += 6)
                for (float z = -150; z <= 150; z += 6)
                    higher = MathF.Max(higher, hf.Height(RegionSpec.Ruin.X + x, RegionSpec.Ruin.Y + z) - top);
            Check("ruin plateau flat & highest", topRange < 0.35f && higher < 1.0f, $"top {top:F1} m, flatness {topRange:F2} m, max higher {higher:F2} m");
        }

        // --- pond holds water
        {
            float centre = hf.Height(RegionSpec.Pond.X, RegionSpec.Pond.Y);
            float minRim = float.MaxValue;
            for (int i = 0; i < 48; i++)
            {
                float ang = i * MathF.Tau / 48;
                var d = new Vector2(MathF.Cos(ang), MathF.Sin(ang));
                var p = RegionSpec.Pond + d * (hf.PondRadiusAt(RegionSpec.Pond + d) + 6f);
                minRim = MathF.Min(minRim, hf.Height(p.X, p.Y));
            }
            Check("pond basin", hf.PondLevel - centre > 1.5f && minRim > hf.PondLevel, $"level {hf.PondLevel:F2}, depth {hf.PondLevel - centre:F2}, rim min {minRim:F2}");
        }

        // --- forest where the spec says, none on the road/village
        {
            int inF = 0, nF = 0, bad = 0;
            for (float x = -200; x < 560; x += 20)
                for (float z = -560; z < -170; z += 20)
                {
                    nF++;
                    if (hf.ForestDensity(x, z) > 0.4f) inF++;
                }
            var road = RegionSpec.Road;
            for (float s = 0; s < road.Length; s += 5)
            {
                var p = road.PointAt(s);
                if (hf.ForestDensity(p.X, p.Y) > 0.01f) bad++;
            }
            if (hf.ForestDensity(RegionSpec.VillageCenter.X, RegionSpec.VillageCenter.Y) > 0) bad++;
            Check("forest mask", inF > nF * 0.55f && bad == 0, $"{100f * inF / nF:F0}% of the NE block is forest, {bad} forest samples on road/village");
        }

        // --- vegetation respects exclusions
        if (region.Vegetation != null)
        {
            var (checkedN, violations, info) = region.Vegetation.VerifyExclusions(hf);
            Check("vegetation exclusions", violations == 0, $"{checkedN} trees checked, {violations} violations {info}");
        }

        // --- terrain chunks and collision
        {
            int chunks = region.Terrain.GetChildCount();
            Check("terrain chunks", chunks == Terrain.Chunks * Terrain.Chunks, $"{chunks} chunks");
        }

        // --- player spawns on the ground and can walk
        {
            var player = region.Player;
            for (int i = 0; i < 45; i++) await host.ToSignal(host.GetTree(), SceneTree.SignalName.PhysicsFrame);
            float ground = hf.Height(player.GlobalPosition.X, player.GlobalPosition.Z);
            bool onFloor = player.IsOnFloor();
            float dy = player.GlobalPosition.Y - ground;
            Check("player on ground at start", onFloor && MathF.Abs(dy) < 0.25f,
                  $"pos {player.GlobalPosition}, ground {ground:F2}, dy {dy:F2}, on_floor {onFloor}");
            // walk east along the road for ~2 s
            Vector3 start = player.GlobalPosition;
            Input.ActionPress("move_forward");
            for (int i = 0; i < 120; i++) await host.ToSignal(host.GetTree(), SceneTree.SignalName.PhysicsFrame);
            Input.ActionRelease("move_forward");
            float moved = (player.GlobalPosition - start).Length();
            Check("player moves on terrain", moved > 1.0f && player.IsOnFloor(), $"moved {moved:F1} m, on_floor {player.IsOnFloor()}");
        }

        bool all = true;
        foreach (var (name, ok, info) in results)
        {
            GD.Print($"  [{(ok ? "PASS" : "FAIL")}] {name} — {info}");
            all &= ok;
        }
        GD.Print(all ? "SELFTEST PASS" : "SELFTEST FAIL");
        host.GetTree().Quit(all ? 0 : 1);
    }
}
