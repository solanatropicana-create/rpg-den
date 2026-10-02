using System;
using System.Collections.Generic;
using Godot;
using FD.Core;

namespace FD.World;

/// <summary>
/// Procedural low-poly stand-ins for assets/models/nature.glb, with the same object names, sizes and
/// vertex-colour conventions (leaves A = 1 tint mask). Used only when the glb is missing so the world is always
/// complete.
/// </summary>
public static class PlaceholderVeg
{
    static readonly Color Bark = MeshKit.C(0x5a4030), BarkDk = MeshKit.C(0x46321f), BirchBark = MeshKit.C(0xe6e2d6),
        BirchMark = MeshKit.C(0x2e2a26), Stone = MeshKit.C(0x8d877e), StoneDk = MeshKit.C(0x6d6258), Moss = MeshKit.C(0x5d7a3a),
        Wood = MeshKit.C(0x7a5a3a), WoodEnd = MeshKit.C(0xb08a5a), PetalY = MeshKit.C(0xe8c040), PetalW = MeshKit.C(0xf2efe6),
        PetalR = MeshKit.C(0xc0392b), PetalP = MeshKit.C(0x8a5ab0), Cattail = MeshKit.C(0x5a3d22);

    public static Dictionary<string, Mesh> Build()
    {
        var d = new Dictionary<string, Mesh>();
        for (int i = 1; i <= 3; i++)
        {
            d[$"tree_oak_{i}"] = Oak(i, true); d[$"tree_oak_{i}_lo"] = Oak(i, false);
            d[$"tree_pine_{i}"] = Pine(i, true); d[$"tree_pine_{i}_lo"] = Pine(i, false);
        }
        for (int i = 1; i <= 2; i++) { d[$"tree_birch_{i}"] = Birch(i, true); d[$"tree_birch_{i}_lo"] = Birch(i, false); }
        for (int i = 1; i <= 3; i++) { d[$"bush_{i}"] = Bush(i, true); d[$"bush_{i}_lo"] = Bush(i, false); }
        d["rock_s"] = Rock(0.28f, 0.22f, 1, 1); d["rock_m"] = Rock(0.75f, 0.6f, 1, 2);
        d["rock_l"] = Rock(2.3f, 1.5f, 1, 3); d["rock_l_lo"] = Rock(2.3f, 1.5f, 0, 3);
        d["rock_cliff"] = Cliff(true); d["rock_cliff_lo"] = Cliff(false);
        d["stump"] = Stump(); d["log_fallen"] = Log();
        d["grass_clump"] = Grass(0.55f, 7, 1); d["grass_clump_2"] = Grass(0.4f, 6, 2);
        d["flower_clump"] = Flowers(PetalY, PetalW, 3); d["flower_clump_2"] = Flowers(PetalR, PetalP, 4);
        d["reeds"] = Reeds(); d["fern"] = Fern();
        return d;
    }

    static Mesh Oak(int v, bool hi)
    {
        var k = new MeshKit();
        float h = 9.6f + 1.6f * (v - 1), crownBase = h * 0.5f, trunkR = 0.45f + 0.02f * v;
        k.Cylinder(Vector3.Zero, trunkR * 1.25f, trunkR * 0.7f, crownBase + 0.8f, hi ? 7 : 5, Bark);
        var rng = new Rng((uint)(v * 7919));
        int blobs = hi ? 7 : 4;
        float cr = h * 0.38f;
        for (int i = 0; i < blobs; i++)
        {
            float a = i * MathF.Tau / blobs + rng.F() * 0.6f;
            float rr = i == 0 ? 0 : cr * (0.45f + 0.25f * rng.F());
            var c = new Vector3(MathF.Cos(a) * rr, crownBase + cr * (0.55f + 0.5f * rng.F()) + (i == 0 ? cr * 0.3f : 0), MathF.Sin(a) * rr);
            float br = cr * (i == 0 ? 0.75f : 0.55f + 0.15f * rng.F());
            k.Blob(c, new Vector3(br, br * 0.8f, br), MeshKit.Mask(1f), hi ? 1 : 0, 0.15f, (uint)(v * 31 + i),
                   off => MeshKit.Mask(0.72f + 0.28f * Mathf.Clamp(off.Y / br * 0.5f + 0.5f, 0f, 1f)));
            if (hi && i > 0) k.Cylinder(new Vector3(0, crownBase * 0.8f, 0), 0.16f, 0.08f, 0.1f, 4, BarkDk);
        }
        return k.ToMesh();
    }

    static Mesh Pine(int v, bool hi)
    {
        var k = new MeshKit();
        float h = 12.5f + 2.6f * (v - 1), r0 = 0.35f + 0.03f * v;
        k.Cylinder(Vector3.Zero, r0 * 1.2f, r0 * 0.5f, h * 0.35f, hi ? 6 : 4, Bark);
        int tiers = hi ? 6 : 3;
        float baseY = h * 0.16f, span = h - baseY;
        for (int i = 0; i < tiers; i++)
        {
            float t = i / (float)tiers;
            float y = baseY + span * t * 0.82f;
            float rad = (h * 0.24f) * (1f - t * 0.8f);
            float th = span / tiers * 1.8f;
            float shade = 0.72f + 0.28f * t;
            k.Cone(new Vector3(0, y, 0), rad, th, hi ? 9 : 6, MeshKit.Mask(shade), i * 0.37f);
        }
        return k.ToMesh();
    }

    static Mesh Birch(int v, bool hi)
    {
        var k = new MeshKit();
        float h = 10.4f + 1.8f * (v - 1);
        k.Cylinder(Vector3.Zero, 0.22f, 0.12f, h * 0.7f, hi ? 6 : 4, BirchBark);
        if (hi)
            for (int i = 0; i < 6; i++)
                k.Box(new Vector3(0, 0.8f + i * 1.05f, 0.18f - 0.03f * i), new Vector3(0.16f, 0.07f, 0.06f), BirchMark);
        var rng = new Rng((uint)(v * 104729));
        int blobs = hi ? 6 : 3;
        for (int i = 0; i < blobs; i++)
        {
            float a = i * 2.3f;
            float y = h * (0.45f + 0.5f * i / blobs);
            float rr = 0.9f + 0.6f * rng.F();
            var c = new Vector3(MathF.Cos(a) * rr, y, MathF.Sin(a) * rr);
            float br = 1.3f + 0.5f * rng.F();
            k.Blob(c, new Vector3(br, br * 1.1f, br), MeshKit.Mask(1f), hi ? 1 : 0, 0.18f, (uint)(v * 97 + i),
                   off => MeshKit.Mask(0.75f + 0.25f * Mathf.Clamp(off.Y / br * 0.5f + 0.5f, 0f, 1f)));
        }
        return k.ToMesh();
    }

    static Mesh Bush(int v, bool hi)
    {
        var k = new MeshKit();
        var rng = new Rng((uint)(v * 1543));
        int blobs = hi ? 4 : 2;
        for (int i = 0; i < blobs; i++)
        {
            var c = new Vector3((rng.F() - 0.5f) * 1.1f, 0.45f + rng.F() * 0.3f, (rng.F() - 0.5f) * 0.8f);
            float br = 0.5f + 0.25f * rng.F();
            k.Blob(c, new Vector3(br, br * 0.85f, br), MeshKit.Mask(0.9f), hi ? 1 : 0, 0.2f, (uint)(v * 13 + i),
                   off => MeshKit.Mask(0.7f + 0.3f * Mathf.Clamp(off.Y / br * 0.5f + 0.5f, 0f, 1f)));
        }
        return k.ToMesh();
    }

    static Mesh Rock(float r, float h, int sub, uint seed)
    {
        var k = new MeshKit();
        k.Blob(new Vector3(0, h * 0.35f, 0), new Vector3(r, h * 0.6f, r * 0.8f), Stone, sub, 0.22f, seed,
               off => off.Y > h * 0.2f ? Moss.Lerp(Stone, 0.5f) : (off.X > 0 ? Stone : StoneDk));
        return k.ToMesh();
    }

    static Mesh Cliff(bool hi)
    {
        var k = new MeshKit();
        var rng = new Rng(77);
        int n = hi ? 5 : 3;
        for (int i = 0; i < n; i++)
        {
            var c = new Vector3((i - n / 2f) * 1.6f, 1.6f + rng.F() * 1.5f, (rng.F() - 0.5f) * 1.2f);
            k.Blob(c, new Vector3(1.6f, 2.4f + rng.F(), 1.4f), Stone, hi ? 1 : 0, 0.18f, (uint)(200 + i),
                   off => off.Y > 1.2f ? Moss.Lerp(Stone, 0.6f) : (off.X > 0 ? Stone : StoneDk));
        }
        return k.ToMesh();
    }

    static Mesh Stump()
    {
        var k = new MeshKit();
        k.Cylinder(Vector3.Zero, 0.45f, 0.38f, 0.52f, 7, Bark, true, 0f, WoodEnd);
        return k.ToMesh();
    }

    static Mesh Log()
    {
        var k = new MeshKit();
        k.Transform = new Transform3D(Basis.FromEuler(new Vector3(0, 0, MathF.PI / 2)), new Vector3(2.6f, 0.3f, 0));
        k.Cylinder(Vector3.Zero, 0.3f, 0.26f, 5.2f, 7, Bark, true, 0f, WoodEnd);
        return k.ToMesh();
    }

    static Mesh Grass(float h, int blades, uint seed)
    {
        var k = new MeshKit();
        var rng = new Rng(seed * 3571);
        for (int i = 0; i < blades; i++)
        {
            float a = rng.F() * MathF.Tau;
            var root = new Vector3(MathF.Cos(a) * 0.08f * rng.F(), 0, MathF.Sin(a) * 0.08f * rng.F());
            var lean = new Vector3(MathF.Cos(a) * 0.18f, 0, MathF.Sin(a) * 0.18f) * rng.F();
            var tip = root + lean + Vector3.Up * h * (0.6f + 0.4f * rng.F());
            k.Blade(root, tip, 0.07f, MeshKit.Mask(0.45f), MeshKit.Mask(1f), new Vector3(-MathF.Sin(a), 0, MathF.Cos(a)));
        }
        return k.ToMesh();
    }

    static Mesh Flowers(Color a, Color b, uint seed)
    {
        var k = new MeshKit();
        var rng = new Rng(seed * 7919);
        for (int i = 0; i < 5; i++)
        {
            float ang = rng.F() * MathF.Tau;
            var root = new Vector3(MathF.Cos(ang) * 0.12f, 0, MathF.Sin(ang) * 0.12f);
            var tip = root + Vector3.Up * (0.3f + 0.15f * rng.F());
            k.Blade(root, tip, 0.03f, MeshKit.Mask(0.5f), MeshKit.Mask(0.9f), new Vector3(1, 0, 0));
            k.Blob(tip, new Vector3(0.05f, 0.035f, 0.05f), i % 2 == 0 ? a : b, 0, 0.1f, (uint)i);
        }
        return k.ToMesh();
    }

    static Mesh Reeds()
    {
        var k = new MeshKit();
        var rng = new Rng(4242);
        for (int i = 0; i < 9; i++)
        {
            float ang = rng.F() * MathF.Tau;
            var root = new Vector3(MathF.Cos(ang) * 0.25f * rng.F(), 0, MathF.Sin(ang) * 0.25f * rng.F());
            var tip = root + new Vector3((rng.F() - 0.5f) * 0.2f, 1.2f + 0.5f * rng.F(), (rng.F() - 0.5f) * 0.2f);
            k.Blade(root, tip, 0.05f, MeshKit.Mask(0.5f), MeshKit.Mask(1f), new Vector3(MathF.Sin(ang), 0, MathF.Cos(ang)));
            if (i % 3 == 0) k.Cylinder(tip - Vector3.Up * 0.3f, 0.035f, 0.035f, 0.2f, 4, Cattail);
        }
        return k.ToMesh();
    }

    static Mesh Fern()
    {
        var k = new MeshKit();
        for (int i = 0; i < 7; i++)
        {
            float a = i * MathF.Tau / 7f;
            var dir = new Vector3(MathF.Cos(a), 0, MathF.Sin(a));
            k.Blade(Vector3.Zero, dir * 0.6f + Vector3.Up * 0.4f, 0.22f, MeshKit.Mask(0.6f), MeshKit.Mask(1f), new Vector3(-dir.Z, 0, dir.X));
        }
        return k.ToMesh();
    }
}
