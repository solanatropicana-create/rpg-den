using System;
using System.Collections.Generic;
using Godot;
using FD.Core;
using FD.Life;
using FD.Rpg;

namespace FD.Actors;

/// <summary>Faz 2: a Character's body on the shared human model — race build and ears (Appearance.Look), chosen colours, hair and
/// beard, the equipped weapon in the right hand and armour (helmet for chain). Used by the creation preview, the player and companions.</summary>
public static class LookKit
{
    public static readonly int[] HairColors = { 0x2b1a10, 0x4a2e1a, 0x6b4424, 0xa0683a, 0xc9a060, 0x1a1a1a, 0x7a2e1a, 0xd8d0c0 };
    public static readonly int[] HumanSkins = { 0xf0c8a8, 0xe0b48f, 0xd9a27c, 0xc68a63, 0xa8704f, 0x8a5a3c };
    public static readonly int[] Tunics = { 0x7a5a3a, 0x5a7a3a, 0x3a4a6a, 0x6a2a2a, 0x5a3a7a, 0x2a3a2a, 0x8a7a50, 0xd8c890 };

    public static int[] SkinsFor(string race) => Appearance.Look(race).Skins ?? HumanSkins;

    public static int SkinOf(Character c)
    {
        if (c.Look.Skin != 0) return c.Look.Skin;
        var s = SkinsFor(c.Race);
        return s[(int)(Math.Abs((long)c.Name.GetHashCode()) % s.Length)];
    }

    public static int HairOf(Character c) => c.Look.Hair != 0 ? c.Look.Hair : HairColors[(int)(Math.Abs((long)c.Name.GetHashCode() / 7) % (HairColors.Length - 1))];

    public static string[] Outfit(Character c)
    {
        var o = new List<string>();
        var rl = Appearance.Look(c.Race);
        if (rl.Hair && c.Look.HairStyle is "hair_short" or "hair_long" or "hair_bun" && !(c.Armor == "chainmail")) o.Add(c.Look.HairStyle);
        if (c.Look.Beard && rl.BeardMale > 0) o.Add("beard");
        if (c.Armor is "chainshirt" or "chainmail") o.Add("helmet");
        var w = c.WeaponDef;
        if (w?.Prop != null && w.Prop.StartsWith("tool_")) o.Add(w.Prop);
        return o.ToArray();
    }

    public static float HeightOf(Character c) => Appearance.Look(c.Race).Height * c.Look.Height * (c.Look.Female ? 0.95f : 1f);

    /// <summary>New body for the character (not yet in the tree; procedural weapon props are added on ready).</summary>
    public static Humanoid Body(Character c)
    {
        var rl = Appearance.Look(c.Race);
        var h = new Humanoid
        {
            Name = "Body", ModelName = "human",
            Skin = FMath.Hex((uint)SkinOf(c)), Hair = FMath.Hex((uint)HairOf(c)), Cloth1 = FMath.Hex((uint)c.Look.Cloth1), Cloth2 = FMath.Hex((uint)c.Look.Cloth2),
            Outfit = Outfit(c), Ears = rl.Ears, HandProp = c.WeaponDef?.Prop is string p && p.StartsWith("proc_") ? p : null,
        };
        float s = HeightOf(c);
        h.Scale = new Vector3(s * rl.Wide, s, s * rl.Wide);
        return h;
    }
}
