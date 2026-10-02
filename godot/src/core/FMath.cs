using System;
using System.Runtime.CompilerServices;
using Godot;

namespace FD.Core;

/// <summary>Small float helpers shared by world systems (shader-style naming).</summary>
public static class FMath
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Saturate(float x) => x < 0 ? 0 : (x > 1 ? 1 : x);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Smoothstep(float e0, float e1, float x)
    {
        float t = Saturate((x - e0) / (e1 - e0));
        return t * t * (3f - 2f * t);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Lerp(float a, float b, float t) => a + (b - a) * t;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float InvLerp(float a, float b, float x) => Saturate((x - a) / (b - a));

    /// <summary>Polynomial smooth maximum (k = blend width).</summary>
    public static float SmoothMax(float a, float b, float k)
    {
        float h = Saturate(0.5f + 0.5f * (a - b) / k);
        return Lerp(b, a, h) + k * h * (1f - h);
    }

    public static float SmoothMin(float a, float b, float k) => -SmoothMax(-a, -b, k);

    /// <summary>Distance from p to segment ab; t = parameter (0..1) of the closest point.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static float SegDist(Vector2 p, Vector2 a, Vector2 b, out float t)
    {
        Vector2 ab = b - a;
        float l2 = ab.LengthSquared();
        t = l2 > 1e-9f ? Saturate((p - a).Dot(ab) / l2) : 0f;
        return (a + ab * t - p).Length();
    }

    /// <summary>sRGB hex (0xRRGGBB) to a Godot Color in sRGB space.</summary>
    public static Color Hex(uint rgb, float a = 1f) =>
        new(((rgb >> 16) & 255) / 255f, ((rgb >> 8) & 255) / 255f, (rgb & 255) / 255f, a);

    /// <summary>sRGB hex to linear-space Color (what shaders without source_color expect).</summary>
    public static Color HexLinear(uint rgb, float a = 1f) => Hex(rgb, a).SrgbToLinear();

    public static Vector3 ToV3(this Vector2 v, float y = 0) => new(v.X, y, v.Y);
    public static Vector2 XZ(this Vector3 v) => new(v.X, v.Z);
}
