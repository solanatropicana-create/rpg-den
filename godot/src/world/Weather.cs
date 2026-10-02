using System;
using Godot;
using FD.Core;

namespace FD.World;

/// <summary>
/// Tur 1 D: the weather of the region — mostly grey (Valheim's gloom): overcast and hazy days most of the time, clear spells rare,
/// rain often and dark, mornings in the valleys often foggy. A pure function of the game clock and the world seed, in 8-hour
/// spells blended over the last 1.5 hours of each (so shots and tests are repeatable and the save needs nothing).
/// <list type="bullet">
/// <item><see cref="Cloud"/> 0 clear … 1 solid overcast (sun dimmed, sky grey, shadows faint)</item>
/// <item><see cref="Rain"/> 0 … 1 (streaks around the camera, darker sky, wet ground: <see cref="Wet"/>)</item>
/// <item><see cref="Fog"/> 0 … 1 extra fog on top of the always-present haze (valleys and forest first)</item>
/// <item><see cref="Wind"/> 0.3 … 1 (trees, grass, dust and leaves, the wind's sound)</item>
/// </list>
/// Dev: <c>--weather=clear|hazy|overcast|rain|storm|fog</c> holds one weather.
/// </summary>
public static class Weather
{
    public enum Kind { Clear, Hazy, Overcast, Rain, Storm }

    /// <summary>dev/tests: hold this weather (null: the schedule)</summary>
    public static string Force;
    public static float Cloud { get; private set; } = 0.8f;
    public static float Rain { get; private set; }
    public static float Fog { get; private set; }
    public static float Wind { get; private set; } = 0.5f;
    public static float Wet { get; private set; }
    public static Kind Now { get; private set; } = Kind.Overcast;

    const double Spell = 8.0, Blend = 1.5;

    static double Seed => FD.Game.Session.Current?.Seed ?? 1;

    static double Hash(long a, int salt)
    {
        unchecked
        {
            ulong x = (ulong)a * 0x9E3779B97F4A7C15UL + (ulong)salt * 0xBF58476D1CE4E5B9UL + (ulong)(long)(Seed * 7919);
            x ^= x >> 30; x *= 0xBF58476D1CE4E5B9UL; x ^= x >> 27; x *= 0x94D049BB133111EBUL; x ^= x >> 31;
            return (x >> 11) * (1.0 / (1UL << 53));
        }
    }

    /// <summary>the weather of an 8-hour spell: overcast 45 %, hazy 17 %, clear 10 %, rain 20 %, storm 8 %</summary>
    public static Kind KindOf(long spell)
    {
        double r = Hash(spell, 11);
        return r < 0.45 ? Kind.Overcast : r < 0.62 ? Kind.Hazy : r < 0.72 ? Kind.Clear : r < 0.92 ? Kind.Rain : Kind.Storm;
    }

    static (float cloud, float rain, float fog, float wind) Params(Kind k) => k switch
    {
        Kind.Clear => (0.22f, 0f, 0.0f, 0.35f),
        Kind.Hazy => (0.62f, 0f, 0.25f, 0.5f),
        Kind.Overcast => (0.92f, 0f, 0.3f, 0.62f),
        Kind.Rain => (1f, 0.62f, 0.5f, 0.75f),
        _ => (1f, 1f, 0.6f, 1f),
    };

    static Kind? Forced => Force switch
    {
        "clear" => Kind.Clear, "hazy" => Kind.Hazy, "overcast" => Kind.Overcast, "rain" => Kind.Rain, "storm" => Kind.Storm,
        "fog" => Kind.Overcast, _ => null,
    };

    /// <summary>(cloud, rain, fog, wind) at an absolute game hour</summary>
    static (float c, float r, float f, float w) At(double hours)
    {
        if (Forced is Kind fk) { var p = Params(fk); return (p.cloud, p.rain, Force == "fog" ? 1f : p.fog, p.wind); }
        long spell = (long)Math.Floor(hours / Spell);
        double into = hours - spell * Spell;
        var a = Params(KindOf(spell));
        if (into > Spell - Blend)
        {
            var b = Params(KindOf(spell + 1));
            float t = (float)((into - (Spell - Blend)) / Blend);
            t = t * t * (3 - 2 * t);
            a = (Mathf.Lerp(a.cloud, b.cloud, t), Mathf.Lerp(a.rain, b.rain, t), Mathf.Lerp(a.fog, b.fog, t), Mathf.Lerp(a.wind, b.wind, t));
        }
        // morning valley fog: two mornings in five are thick, the rest misty
        double day = Math.Floor(hours / 24.0), h = hours - day * 24.0;
        float morning = (float)(1.0 - FMath.Smoothstep(0f, 4.5f, (float)Math.Abs(h - 6.0)));
        float thick = Hash((long)day, 23) < 0.4 ? 1f : 0.35f;
        return (a.cloud, a.rain, Math.Max(a.fog, morning * thick), a.wind);
    }

    /// <summary>Recompute for the current clock (DayNight calls this every frame).</summary>
    public static void Update()
    {
        double t = GameClock.TotalHours;
        var (c, r, f, w) = At(t);
        Cloud = c; Rain = r; Fog = f; Wind = w;
        Now = Forced ?? KindOf((long)Math.Floor(t / Spell));
        // wetness: soaked within an hour of rain, dry three hours after it stops
        float wet = 0;
        for (int i = 0; i <= 6; i++) wet = MathF.Max(wet, At(t - i * 0.5).r * (1f - i / 7f));
        Wet = Math.Clamp(wet * 1.4f, 0f, 1f);
    }

    public static string Describe() => Now switch
    {
        Kind.Clear => "açık", Kind.Hazy => "puslu", Kind.Overcast => "kapalı", Kind.Rain => "yağmurlu", _ => "fırtınalı",
    };
}
