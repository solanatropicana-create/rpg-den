using System;
using System.Collections.Generic;

namespace FD.Macro;

/// <summary>
/// Seeded deterministic RNG (mulberry32) + D&D dice helpers; exact port of <c>src/sim/rng.ts</c>.
///
/// Fidelity note: the TS state <c>s</c> is a JS number that is never wrapped: <c>s += 0x6d2b79f5</c>
/// keeps growing as a double and, after ~4.9M calls, passes 2^53 where the addition starts to round.
/// We keep the state as a double and do the same arithmetic so the sequence matches V8 forever.
/// </summary>
public sealed class Rng
{
    private double _s;

    /// <summary>Number of <see cref="Next"/> calls so far (debug/golden tracing only; not part of the state).</summary>
    public long Calls;

    /// <summary>Optional tracer for golden debugging: called with every value produced.</summary>
    public Action<double> Trace;

    public Rng(double seed)
    {
        uint u = JsMath.ToUint32(seed);
        _s = u != 0 ? u : 1;
    }

    public double Next()
    {
        _s += 0x6d2b79f5;                       // JS: this.s += 0x6d2b79f5 (double addition)
        int t = JsMath.ToInt32(_s);             // Math.imul / ^ / >>> all apply ToInt32/ToUint32
        t = unchecked((int)((uint)(t ^ (int)((uint)t >> 15)) * (uint)(t | 1)));
        t ^= unchecked(t + (int)((uint)(t ^ (int)((uint)t >> 7)) * (uint)(t | 61)));
        double r = (uint)(t ^ (int)((uint)t >> 14)) / 4294967296.0;
        Calls++;
        Trace?.Invoke(r);
        return r;
    }

    public double Int(double min, double max) => min + Math.Floor(Next() * (max - min + 1));

    public bool Chance(double p) => Next() < p;

    /// <summary>JS <c>arr[Math.floor(next() * arr.length)]</c>: consumes one value even when empty (returns default).</summary>
    public T Pick<T>(IReadOnlyList<T> arr)
    {
        double i = Math.Floor(Next() * arr.Count);
        return i < arr.Count ? arr[(int)i] : default;
    }

    public T Pick<T>(List<T> arr)
    {
        double i = Math.Floor(Next() * arr.Count);
        return i < arr.Count ? arr[(int)i] : default;
    }

    /// <summary>Weighted pick; returns default (undefined) without consuming when all weights ≤ 0.</summary>
    public T Weighted<T>(IReadOnlyList<T> items, Func<T, double> w)
    {
        double total = 0;
        foreach (var it in items) total += JsMath.Max(0, w(it));
        if (total <= 0) return default;          // NaN total falls through like JS (returns the last item)
        double r = Next() * total;
        foreach (var it in items)
        {
            r -= JsMath.Max(0, w(it));
            if (r <= 0) return it;
        }
        return items.Count > 0 ? items[items.Count - 1] : default;
    }

    public T Weighted<T>(List<T> items, Func<T, double> w) => Weighted((IReadOnlyList<T>)items, w);

    /// <summary>In-place Fisher–Yates exactly like the TS version; returns the same list.</summary>
    public List<T> Shuffle<T>(List<T> arr)
    {
        for (int i = arr.Count - 1; i > 0; i--)
        {
            int j = (int)Math.Floor(Next() * (i + 1));
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
        return arr;
    }

    /// <summary>roll NdS</summary>
    public double Dice(double n, double sides)
    {
        double t = 0;
        for (int i = 0; i < n; i++) t += 1 + Math.Floor(Next() * sides);
        return t;
    }

    /// <summary>NdS, dice one by one (same consumption order as Dice).</summary>
    public List<double> Roll(double n, double sides)
    {
        var o = new List<double>();
        for (int i = 0; i < n; i++) o.Add(1 + Math.Floor(Next() * sides));
        return o;
    }

    public double D20() => Dice(1, 20);

    public double State() => _s;

    public void SetState(double s) => _s = s;

    /// <summary>D&D ability modifier: <c>Math.floor((score - 10) / 2)</c>.</summary>
    public static double Mod(double score) => Math.Floor((score - 10) / 2);
}
