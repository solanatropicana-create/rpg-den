using System;
using System.Runtime.CompilerServices;

namespace FD.Core;

/// <summary>
/// Deterministic 2D gradient noise (Perlin style) implemented in plain C#.
/// Independent of Godot's FastNoiseLite so terrain/vegetation are bit-for-bit reproducible
/// across engine versions and usable from headless tests. Output of <see cref="Get"/> is ~[-1, 1].
/// </summary>
public sealed class Noise2
{
    readonly byte[] _perm = new byte[512];
    static readonly float[] GX, GY;

    static Noise2()
    {
        const int n = 16;
        GX = new float[n]; GY = new float[n];
        for (int i = 0; i < n; i++)
        {
            float a = (i + 0.5f) * MathF.Tau / n;
            GX[i] = MathF.Cos(a); GY[i] = MathF.Sin(a);
        }
    }

    public Noise2(int seed)
    {
        var p = new byte[256];
        for (int i = 0; i < 256; i++) p[i] = (byte)i;
        uint s = (uint)seed * 747796405u + 2891336453u;
        for (int i = 255; i > 0; i--)
        {
            s = s * 1664525u + 1013904223u;
            int j = (int)((s >> 8) % (uint)(i + 1));
            (p[i], p[j]) = (p[j], p[i]);
        }
        for (int i = 0; i < 512; i++) _perm[i] = p[i & 255];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static float Fade(float t) => t * t * t * (t * (t * 6f - 15f) + 10f);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    float Grad(int ix, int iy, float dx, float dy)
    {
        int h = _perm[_perm[ix & 255] + (iy & 255)] & 15;
        return GX[h] * dx + GY[h] * dy;
    }

    /// <summary>Single octave, roughly in [-1, 1].</summary>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public float Get(float x, float y)
    {
        int x0 = (int)MathF.Floor(x), y0 = (int)MathF.Floor(y);
        float fx = x - x0, fy = y - y0;
        float u = Fade(fx), v = Fade(fy);
        float n00 = Grad(x0, y0, fx, fy);
        float n10 = Grad(x0 + 1, y0, fx - 1, fy);
        float n01 = Grad(x0, y0 + 1, fx, fy - 1);
        float n11 = Grad(x0 + 1, y0 + 1, fx - 1, fy - 1);
        float a = n00 + u * (n10 - n00);
        float b = n01 + u * (n11 - n01);
        return (a + v * (b - a)) * 1.41f;
    }

    /// <summary>Fractal sum normalised to ~[-1, 1].</summary>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public float Fbm(float x, float y, int octaves, float lacunarity = 2f, float gain = 0.5f)
    {
        float sum = 0, amp = 1, norm = 0;
        for (int i = 0; i < octaves; i++)
        {
            // small per-octave offset/rotation-free shift avoids lattice alignment artefacts
            sum += Get(x + i * 17.31f, y - i * 9.73f) * amp;
            norm += amp;
            amp *= gain; x *= lacunarity; y *= lacunarity;
        }
        return sum / norm;
    }

    /// <summary>Ridged multifractal in [0, 1] (sharp crests), good for rocky hills.</summary>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public float Ridged(float x, float y, int octaves, float lacunarity = 2f, float gain = 0.5f)
    {
        float sum = 0, amp = 1, norm = 0, weight = 1;
        for (int i = 0; i < octaves; i++)
        {
            float n = 1f - MathF.Abs(Get(x + i * 31.7f, y + i * 11.1f));
            n *= n * weight;
            weight = Math.Clamp(n * 1.6f, 0f, 1f);
            sum += n * amp; norm += amp;
            amp *= gain; x *= lacunarity; y *= lacunarity;
        }
        return sum / norm;
    }
}

/// <summary>Stateless integer hashing for deterministic placement.</summary>
public static class Hash
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint U32(int a, int b = 0, int c = 0, int d = 0)
    {
        uint h = 0x9E3779B9u ^ (uint)a * 0x85EBCA6Bu;
        h = (h ^ (h >> 15)) * 0x2C1B3C6Du;
        h ^= (uint)b * 0xC2B2AE35u; h = (h ^ (h >> 13)) * 0x27D4EB2Fu;
        h ^= (uint)c * 0x165667B1u; h = (h ^ (h >> 16)) * 0x85EBCA6Bu;
        h ^= (uint)d * 0xD3A2646Cu; h = (h ^ (h >> 15)) * 0x2C1B3C6Du;
        return h ^ (h >> 16);
    }

    /// <summary>Uniform float in [0, 1).</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float F01(int a, int b = 0, int c = 0, int d = 0) => (U32(a, b, c, d) >> 8) * (1f / 16777216f);
}

/// <summary>Tiny deterministic PRNG (xorshift32) for sequential draws inside one placement cell.</summary>
public struct Rng
{
    uint _s;
    public Rng(uint seed) { _s = seed == 0 ? 0x6D2B79F5u : seed; }
    public uint Next() { _s ^= _s << 13; _s ^= _s >> 17; _s ^= _s << 5; return _s; }
    public float F() => (Next() >> 8) * (1f / 16777216f);
    public float Range(float a, float b) => a + (b - a) * F();
    public int Int(int n) => (int)(Next() % (uint)n);
}
