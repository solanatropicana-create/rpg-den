using System;
using System.Collections.Generic;
using Godot;

namespace FD.Core;

/// <summary>
/// 2D polyline in the ground plane (Vector2 = (x, z)) with arc-length parameterisation.
/// Use <see cref="CatmullRom"/> to turn sparse control points (as in the region spec) into a smooth,
/// evenly sampled path that still passes through every control point.
/// </summary>
public sealed class Polyline
{
    public readonly Vector2[] Points;
    /// <summary>Cumulative arc length at each point (S[0] = 0).</summary>
    public readonly float[] S;
    public float Length => S[^1];
    public readonly Rect2 Bounds;

    public Polyline(IReadOnlyList<Vector2> pts)
    {
        if (pts.Count < 2) throw new ArgumentException("polyline needs >= 2 points");
        Points = new Vector2[pts.Count];
        S = new float[pts.Count];
        var min = pts[0]; var max = pts[0];
        for (int i = 0; i < pts.Count; i++)
        {
            Points[i] = pts[i];
            if (i > 0) S[i] = S[i - 1] + (pts[i] - pts[i - 1]).Length();
            min = min.Min(pts[i]); max = max.Max(pts[i]);
        }
        Bounds = new Rect2(min, max - min);
    }

    /// <summary>Centripetal Catmull-Rom spline through the control points, resampled every ~step metres.</summary>
    public static Polyline CatmullRom(IReadOnlyList<Vector2> ctrl, float step = 2f)
    {
        var dense = new List<Vector2>();
        int n = ctrl.Count;
        for (int i = 0; i < n - 1; i++)
        {
            Vector2 p0 = ctrl[Math.Max(i - 1, 0)], p1 = ctrl[i], p2 = ctrl[i + 1], p3 = ctrl[Math.Min(i + 2, n - 1)];
            if (i == 0) p0 = p1 * 2 - p2;           // mirrored end tangents
            if (i + 2 >= n) p3 = p2 * 2 - p1;
            float seg = (p2 - p1).Length();
            int k = Math.Max(1, (int)MathF.Ceiling(seg / step));
            for (int j = 0; j < k; j++)
                dense.Add(CentripetalPoint(p0, p1, p2, p3, j / (float)k));
        }
        dense.Add(ctrl[n - 1]);
        return new Polyline(dense);
    }

    static Vector2 CentripetalPoint(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float u)
    {
        static float Tj(float ti, Vector2 a, Vector2 b) => ti + MathF.Sqrt(Math.Max((b - a).Length(), 1e-4f));
        float t0 = 0, t1 = Tj(t0, p0, p1), t2 = Tj(t1, p1, p2), t3 = Tj(t2, p2, p3);
        float t = t1 + (t2 - t1) * u;
        Vector2 a1 = (t1 - t) / (t1 - t0) * p0 + (t - t0) / (t1 - t0) * p1;
        Vector2 a2 = (t2 - t) / (t2 - t1) * p1 + (t - t1) / (t2 - t1) * p2;
        Vector2 a3 = (t3 - t) / (t3 - t2) * p2 + (t - t2) / (t3 - t2) * p3;
        Vector2 b1 = (t2 - t) / (t2 - t0) * a1 + (t - t0) / (t2 - t0) * a2;
        Vector2 b2 = (t3 - t) / (t3 - t1) * a2 + (t - t1) / (t3 - t1) * a3;
        return (t2 - t) / (t2 - t1) * b1 + (t - t1) / (t2 - t1) * b2;
    }

    int SegmentAt(float s)
    {
        if (s <= 0) return 0;
        if (s >= Length) return Points.Length - 2;
        int lo = 0, hi = S.Length - 1;
        while (hi - lo > 1) { int mid = (lo + hi) >> 1; if (S[mid] <= s) lo = mid; else hi = mid; }
        return lo;
    }

    public Vector2 PointAt(float s)
    {
        int i = SegmentAt(s);
        float len = S[i + 1] - S[i];
        float t = len > 1e-6f ? Math.Clamp((s - S[i]) / len, 0f, 1f) : 0f;
        return Points[i].Lerp(Points[i + 1], t);
    }

    /// <summary>Unit tangent (direction of increasing s).</summary>
    public Vector2 TangentAt(float s)
    {
        int i = SegmentAt(s);
        return (Points[i + 1] - Points[i]).Normalized();
    }

    /// <summary>Brute-force closest point: distance, arc length s and side (+1 = right-hand side of the travel
    /// direction, see <see cref="Side"/>). Fine for occasional queries; bulk queries should use a
    /// <see cref="DistanceField"/>.</summary>
    public float Distance(Vector2 p, out float s, out float side)
    {
        float best = float.MaxValue; s = 0; side = 1;
        for (int i = 0; i < Points.Length - 1; i++)
        {
            float d = FMath.SegDist(p, Points[i], Points[i + 1], out float t);
            if (d < best)
            {
                best = d; s = S[i] + t * (S[i + 1] - S[i]);
                side = Side(Points[i], Points[i + 1], p);
            }
        }
        return best;
    }

    public float Distance(Vector2 p) => Distance(p, out _, out _);

    /// <summary>+1 if p lies to the right-hand side of a→b when looking down from +y with x east, z south
    /// (i.e. cross(ab, ap) &gt; 0 in x/z), else -1.</summary>
    public static float Side(Vector2 a, Vector2 b, Vector2 p)
    {
        Vector2 ab = b - a, ap = p - a;
        return ab.X * ap.Y - ab.Y * ap.X >= 0 ? 1f : -1f;
    }

    /// <summary>First intersection with another polyline (null if none).</summary>
    public Vector2? Intersect(Polyline o, out float sThis, out float sOther)
    {
        sThis = sOther = 0;
        for (int i = 0; i < Points.Length - 1; i++)
        {
            Vector2 a = Points[i], b = Points[i + 1];
            for (int j = 0; j < o.Points.Length - 1; j++)
            {
                Vector2 c = o.Points[j], d = o.Points[j + 1];
                Vector2 r = b - a, q = d - c;
                float den = r.X * q.Y - r.Y * q.X;
                if (MathF.Abs(den) < 1e-9f) continue;
                float t = ((c.X - a.X) * q.Y - (c.Y - a.Y) * q.X) / den;
                float u = ((c.X - a.X) * r.Y - (c.Y - a.Y) * r.X) / den;
                if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
                {
                    sThis = S[i] + t * (S[i + 1] - S[i]);
                    sOther = o.S[j] + u * (o.S[j + 1] - o.S[j]);
                    return a + r * t;
                }
            }
        }
        return null;
    }
}

/// <summary>
/// Distance-to-polyline field sampled on a regular grid (same layout as the heightfield: N×N samples,
/// spacing <c>cell</c>, origin <c>min</c>). Stores unsigned distance, arc length of the closest point and side.
/// Only samples within <c>maxDist</c> are exact; everything farther reads as maxDist.
/// </summary>
public sealed class DistanceField
{
    public readonly int N;
    public readonly float Cell, Min, MaxDist;
    public readonly float[] D, S;
    public readonly sbyte[] Side;
    public readonly Polyline Line;

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveOptimization)]
    public DistanceField(Polyline line, int n, float cell, float min, float maxDist)
    {
        Line = line; N = n; Cell = cell; Min = min; MaxDist = maxDist;
        D = new float[n * n]; S = new float[n * n]; Side = new sbyte[n * n];
        Array.Fill(D, maxDist);
        var pts = line.Points;
        for (int i = 0; i < pts.Length - 1; i++)
        {
            Vector2 a = pts[i], b = pts[i + 1];
            int x0 = Math.Max(0, (int)MathF.Floor((MathF.Min(a.X, b.X) - maxDist - min) / cell));
            int x1 = Math.Min(n - 1, (int)MathF.Ceiling((MathF.Max(a.X, b.X) + maxDist - min) / cell));
            int z0 = Math.Max(0, (int)MathF.Floor((MathF.Min(a.Y, b.Y) - maxDist - min) / cell));
            int z1 = Math.Min(n - 1, (int)MathF.Ceiling((MathF.Max(a.Y, b.Y) + maxDist - min) / cell));
            for (int gz = z0; gz <= z1; gz++)
            {
                float pz = min + gz * cell;
                for (int gx = x0; gx <= x1; gx++)
                {
                    var p = new Vector2(min + gx * cell, pz);
                    float d = FMath.SegDist(p, a, b, out float t);
                    int k = gz * n + gx;
                    if (d < D[k])
                    {
                        D[k] = d;
                        S[k] = line.S[i] + t * (line.S[i + 1] - line.S[i]);
                        Side[k] = (sbyte)Polyline.Side(a, b, p);
                    }
                }
            }
        }
    }

    /// <summary>Bilinearly interpolated distance at world (x, z).</summary>
    public float Dist(float x, float z)
    {
        float fx = (x - Min) / Cell, fz = (z - Min) / Cell;
        int ix = Math.Clamp((int)fx, 0, N - 2), iz = Math.Clamp((int)fz, 0, N - 2);
        float tx = Math.Clamp(fx - ix, 0, 1), tz = Math.Clamp(fz - iz, 0, 1);
        int k = iz * N + ix;
        float a = D[k] + (D[k + 1] - D[k]) * tx;
        float b = D[k + N] + (D[k + N + 1] - D[k + N]) * tx;
        return a + (b - a) * tz;
    }

    /// <summary>Arc length of the closest polyline point (nearest-sample, refined by a local exact query).</summary>
    public float ArcAt(float x, float z)
    {
        int ix = Math.Clamp((int)MathF.Round((x - Min) / Cell), 0, N - 1);
        int iz = Math.Clamp((int)MathF.Round((z - Min) / Cell), 0, N - 1);
        return S[iz * N + ix];
    }
}
