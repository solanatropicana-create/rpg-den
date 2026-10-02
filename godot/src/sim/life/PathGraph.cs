using System;
using System.Collections.Generic;
using System.Numerics;

namespace FD.Sim.Life;

/// <summary>
/// Walkable path network: the road, trails, village lanes and door/field spurs, sampled into nodes.
/// People walk along it (keeping to the right) and cut straight only inside open areas (fields, pasture, plaza,
/// camp) — so they use the paths a real villager would use, and those paths are visibly trodden in the terrain.
/// </summary>
public sealed class PathGraph
{
    public readonly List<Vector2> Nodes = new();
    /// <summary>Lane width class per node (half road width): road 2, lane 1.4, path 0.6.</summary>
    public readonly List<float> HalfWidth = new();
    public readonly List<List<(int to, float len)>> Adj = new();

    public int AddNode(Vector2 p, float halfWidth)
    {
        Nodes.Add(p);
        HalfWidth.Add(halfWidth);
        Adj.Add(new List<(int, float)>());
        return Nodes.Count - 1;
    }

    public void Link(int a, int b)
    {
        if (a == b || a < 0 || b < 0) return;
        foreach (var (to, _) in Adj[a]) if (to == b) return;
        float l = Vector2.Distance(Nodes[a], Nodes[b]);
        Adj[a].Add((b, l));
        Adj[b].Add((a, l));
    }

    /// <summary>Add a polyline resampled every <paramref name="spacing"/> m; returns its node ids in order.
    /// Endpoints snap to existing nodes within <paramref name="snap"/> m (junctions), and are otherwise joined to
    /// the nearest node of the network if within <paramref name="attach"/> m.</summary>
    public List<int> AddPolyline(IReadOnlyList<Vector2> pts, float spacing, float halfWidth, float snap = 2.5f, float attach = 8f)
    {
        var ids = new List<int>();
        if (pts.Count < 2) return ids;
        var samples = Resample(pts, spacing);
        for (int i = 0; i < samples.Count; i++)
        {
            int id = -1;
            if (i == 0 || i == samples.Count - 1)
            {
                int n = Nearest(samples[i], snap);
                if (n >= 0) id = n;
            }
            if (id < 0) id = AddNode(samples[i], halfWidth);
            if (ids.Count > 0) Link(ids[^1], id);
            ids.Add(id);
        }
        // attach dangling endpoints to the rest of the network (e.g. a lane ending on the road)
        foreach (int end in new[] { ids[0], ids[^1] })
        {
            if (Adj[end].Count > 1) continue;
            int n = NearestExcluding(Nodes[end], attach, ids);
            if (n >= 0) Link(end, n);
        }
        return ids;
    }

    static List<Vector2> Resample(IReadOnlyList<Vector2> pts, float spacing)
    {
        var r = new List<Vector2> { pts[0] };
        float carry = 0;
        for (int i = 1; i < pts.Count; i++)
        {
            Vector2 a = pts[i - 1], b = pts[i];
            float L = Vector2.Distance(a, b);
            float t = spacing - carry;
            while (t <= L)
            {
                r.Add(Vector2.Lerp(a, b, t / L));
                t += spacing;
            }
            carry = L - (t - spacing);
        }
        if (Vector2.Distance(r[^1], pts[^1]) > spacing * 0.3f) r.Add(pts[^1]);
        else r[^1] = pts[^1];
        return r;
    }

    public int Nearest(Vector2 p, float maxDist = float.MaxValue)
    {
        int best = -1; float bd = maxDist * maxDist;
        for (int i = 0; i < Nodes.Count; i++)
        {
            float d = Vector2.DistanceSquared(Nodes[i], p);
            if (d < bd) { bd = d; best = i; }
        }
        return best;
    }

    int NearestExcluding(Vector2 p, float maxDist, List<int> exclude)
    {
        var ex = new HashSet<int>(exclude);
        int best = -1; float bd = maxDist * maxDist;
        for (int i = 0; i < Nodes.Count; i++)
        {
            if (ex.Contains(i)) continue;
            float d = Vector2.DistanceSquared(Nodes[i], p);
            if (d < bd) { bd = d; best = i; }
        }
        return best;
    }

    /// <summary>A* between two nodes; returns node ids or null.</summary>
    public List<int> FindPath(int start, int goal)
    {
        if (start < 0 || goal < 0) return null;
        if (start == goal) return new List<int> { start };
        int n = Nodes.Count;
        var g = new float[n];
        var came = new int[n];
        var closed = new bool[n];
        Array.Fill(g, float.MaxValue);
        Array.Fill(came, -1);
        var open = new PriorityQueue<int, float>();
        g[start] = 0;
        open.Enqueue(start, Vector2.Distance(Nodes[start], Nodes[goal]));
        while (open.Count > 0)
        {
            int c = open.Dequeue();
            if (closed[c]) continue;
            if (c == goal) break;
            closed[c] = true;
            foreach (var (to, len) in Adj[c])
            {
                if (closed[to]) continue;
                float ng = g[c] + len;
                if (ng < g[to])
                {
                    g[to] = ng;
                    came[to] = c;
                    open.Enqueue(to, ng + Vector2.Distance(Nodes[to], Nodes[goal]));
                }
            }
        }
        if (came[goal] < 0) return null;
        var path = new List<int>();
        for (int c = goal; c >= 0; c = came[c]) path.Add(c);
        path.Reverse();
        return path;
    }

    /// <summary>Connected-component id per node (for validation: everything should be one network).</summary>
    public int[] Components(out int count)
    {
        var comp = new int[Nodes.Count];
        Array.Fill(comp, -1);
        count = 0;
        var stack = new Stack<int>();
        for (int i = 0; i < Nodes.Count; i++)
        {
            if (comp[i] >= 0) continue;
            stack.Push(i); comp[i] = count;
            while (stack.Count > 0)
            {
                int c = stack.Pop();
                foreach (var (to, _) in Adj[c]) if (comp[to] < 0) { comp[to] = count; stack.Push(to); }
            }
            count++;
        }
        return comp;
    }
}
