using System;
using System.Collections.Generic;

namespace FD.Macro;

/// <summary>Cube coordinates of a hex.</summary>
public readonly struct Cube
{
    public readonly int X, Y, Z;
    public Cube(int x, int y, int z) { X = x; Y = y; Z = z; }
}

/// <summary>Pointy-top hex grid, odd-r offset storage. Index = row * W + col. Port of <c>src/sim/hex.ts</c>.</summary>
public sealed class HexGrid
{
    public readonly int W, H;

    public HexGrid(int w, int h) { W = w; H = h; }

    public int Idx(int col, int row) => row * W + col;
    public int Col(int i) => i % W;
    public int Row(int i) => i / W;              // i ≥ 0: same as Math.floor(i / W)
    public bool Inside(int col, int row) => col >= 0 && row >= 0 && col < W && row < H;
    public int Size => W * H;

    public Cube ToCube(int i)
    {
        int c = Col(i), r = Row(i);
        int x = c - (r - (r & 1)) / 2;
        int z = r;
        return new Cube(x, -x - z, z);
    }

    public int Dist(int a, int b)
    {
        Cube A = ToCube(a), B = ToCube(b);
        return Math.Max(Math.Abs(A.X - B.X), Math.Max(Math.Abs(A.Y - B.Y), Math.Abs(A.Z - B.Z)));
    }

    private static readonly int[,] OddD = { { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, 0 }, { 0, 1 }, { 1, 1 } };
    private static readonly int[,] EvenD = { { 1, 0 }, { 0, -1 }, { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, 1 } };

    /// <summary>Neighbours in the TS order (E, NE, NW, W, SW, SE for the row parity tables).</summary>
    public List<int> Neighbors(int i)
    {
        int c = Col(i), r = Row(i);
        var d = (r & 1) != 0 ? OddD : EvenD;
        var outp = new List<int>(6);
        for (int k = 0; k < 6; k++)
        {
            int nc = c + d[k, 0], nr = r + d[k, 1];
            if (Inside(nc, nr)) outp.Add(Idx(nc, nr));
        }
        return outp;
    }

    /// <summary>All tiles within <paramref name="radius"/> (row-major order, like TS).</summary>
    public List<int> Within(int i, int radius)
    {
        var outp = new List<int>();
        int c = Col(i), r = Row(i);
        for (int rr = r - radius; rr <= r + radius; rr++)
            for (int cc = c - radius - 1; cc <= c + radius + 1; cc++)
            {
                if (!Inside(cc, rr)) continue;
                int j = Idx(cc, rr);
                if (Dist(i, j) <= radius) outp.Add(j);
            }
        return outp;
    }

    /// <summary>Pixel centre for rendering (pointy-top, size = hex radius).</summary>
    public (double x, double z) Pixel(int i, double size)
    {
        int c = Col(i), r = Row(i);
        return (size * Math.Sqrt(3) * (c + 0.5 * (r & 1)), size * 1.5 * r);
    }

    /// <summary>A* over the hex grid. cost(i) returns Infinity for impassable. Exact port of TS findPath.</summary>
    public static List<int> FindPath(HexGrid g, int from, int to, Func<int, double> cost)
    {
        if (from == to) return new List<int> { from };
        var open = new MinHeap();
        var gScore = new Dictionary<int, double>();
        var came = new Dictionary<int, int>();
        gScore[from] = 0;
        open.Push(from, g.Dist(from, to));
        var closed = new HashSet<int>();
        while (open.Size > 0)
        {
            int cur = open.Pop();
            if (cur == to)
            {
                var path = new List<int> { cur };
                int c = cur;
                while (came.TryGetValue(c, out int p)) { c = p; path.Add(c); }
                path.Reverse();
                return path;
            }
            if (closed.Contains(cur)) continue;
            closed.Add(cur);
            foreach (int n in g.Neighbors(cur))
            {
                double step = n == to ? JsMath.Min(cost(n), 3) : cost(n);
                if (!double.IsFinite(step)) continue;
                double t = gScore[cur] + step;
                if (t < (gScore.TryGetValue(n, out double gn) ? gn : double.PositiveInfinity))
                {
                    gScore[n] = t;
                    came[n] = cur;
                    open.Push(n, t + g.Dist(n, to) * 0.5);
                }
            }
        }
        return null;
    }
}

/// <summary>Binary min-heap on (value, priority); exact port of the TS MinHeap (same tie behaviour).</summary>
public sealed class MinHeap
{
    private readonly List<int> _a = new();
    private readonly List<double> _p = new();

    public int Size => _a.Count;

    public void Push(int v, double pr)
    {
        _a.Add(v); _p.Add(pr);
        int i = _a.Count - 1;
        while (i > 0)
        {
            int par = (i - 1) >> 1;
            if (_p[par] <= _p[i]) break;
            Swap(i, par); i = par;
        }
    }

    /// <summary>Removes and returns the top value; -1 when empty (TS returns undefined; callers check Size first).</summary>
    public int Pop()
    {
        if (_a.Count == 0) return -1;
        int top = _a[0];
        int lv = _a[_a.Count - 1]; double lp = _p[_p.Count - 1];
        _a.RemoveAt(_a.Count - 1); _p.RemoveAt(_p.Count - 1);
        if (_a.Count > 0)
        {
            _a[0] = lv; _p[0] = lp;
            int i = 0;
            for (; ; )
            {
                int l = 2 * i + 1, r = l + 1;
                int m = i;
                if (l < _a.Count && _p[l] < _p[m]) m = l;
                if (r < _a.Count && _p[r] < _p[m]) m = r;
                if (m == i) break;
                Swap(i, m); i = m;
            }
        }
        return top;
    }

    private void Swap(int i, int j)
    {
        (_a[i], _a[j]) = (_a[j], _a[i]);
        (_p[i], _p[j]) = (_p[j], _p[i]);
    }
}
