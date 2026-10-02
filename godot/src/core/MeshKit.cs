using System;
using System.Collections.Generic;
using Godot;

namespace FD.Core;

/// <summary>
/// Minimal flat-shaded, vertex-coloured mesh builder (the procedural counterpart of the Blender kit).
/// Colours follow the same RGBA convention as the Blender assets (linear RGB; A = 0 fixed, 1 tint mask,
/// 0.5 glass; for characters 0.25/0.5/0.75/1 = skin/cloth1/cloth2/hair). Triangles are wound clockwise
/// (Godot front faces). Use <see cref="Transform"/> to place parts.
/// </summary>
public sealed class MeshKit
{
    readonly List<Vector3> _v = new();
    readonly List<Vector3> _n = new();
    readonly List<Color> _c = new();
    public Transform3D Transform = Transform3D.Identity;
    public int TriangleCount => _v.Count / 3;

    /// <summary>Add one triangle (clockwise seen from the front) with a flat normal.</summary>
    public void Tri(Vector3 a, Vector3 b, Vector3 c, Color col)
    {
        a = Transform * a; b = Transform * b; c = Transform * c;
        Vector3 n = (c - a).Cross(b - a);
        if (n.LengthSquared() < 1e-12f) return;
        n = n.Normalized();
        _v.Add(a); _v.Add(b); _v.Add(c);
        _n.Add(n); _n.Add(n); _n.Add(n);
        _c.Add(col); _c.Add(col); _c.Add(col);
    }

    /// <summary>Quad p0..p3 in clockwise order seen from the front.</summary>
    public void Quad(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Color col)
    {
        Tri(p0, p1, p2, col); Tri(p0, p2, p3, col);
    }

    /// <summary>Axis-aligned box (before <see cref="Transform"/>), given centre and full size.</summary>
    public void Box(Vector3 center, Vector3 size, Color col, float taperTop = 1f)
    {
        Vector3 h = size * 0.5f;
        Vector3 P(float sx, float sy, float sz)
        {
            float t = sy > 0 ? taperTop : 1f;
            return center + new Vector3(sx * h.X * t, sy * h.Y, sz * h.Z * t);
        }
        // each face: corners ordered (-u,-v), (-u,+v), (+u,+v), (+u,-v) with u × v = n → clockwise front
        Face(P(1, -1, -1), P(1, -1, 1), P(1, 1, 1), P(1, 1, -1), col);       // +X (u=Y, v=Z)
        Face(P(-1, -1, -1), P(-1, 1, -1), P(-1, 1, 1), P(-1, -1, 1), col);   // -X (u=Z, v=Y)
        Face(P(-1, 1, -1), P(1, 1, -1), P(1, 1, 1), P(-1, 1, 1), col);       // +Y (u=Z, v=X)
        Face(P(-1, -1, -1), P(-1, -1, 1), P(1, -1, 1), P(1, -1, -1), col);   // -Y (u=X, v=Z)
        Face(P(-1, -1, 1), P(-1, 1, 1), P(1, 1, 1), P(1, -1, 1), col);       // +Z (u=X, v=Y)
        Face(P(-1, -1, -1), P(1, -1, -1), P(1, 1, -1), P(-1, 1, -1), col);   // -Z (u=Y, v=X)
    }

    void Face(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Color col) { Tri(a, b, c, col); Tri(a, c, d, col); }

    /// <summary>Frustum/cylinder along +Y from base centre, radius r0 at the bottom, r1 at the top.</summary>
    public void Cylinder(Vector3 baseCenter, float r0, float r1, float height, int seg, Color col, bool caps = true,
                         float twist = 0f, Color? capColor = null)
    {
        for (int i = 0; i < seg; i++)
        {
            float a0 = (i / (float)seg) * MathF.Tau + twist, a1 = ((i + 1) / (float)seg) * MathF.Tau + twist;
            Vector3 d0 = new(MathF.Cos(a0), 0, MathF.Sin(a0)), d1 = new(MathF.Cos(a1), 0, MathF.Sin(a1));
            Vector3 b0 = baseCenter + d0 * r0, b1 = baseCenter + d1 * r0;
            Vector3 t0 = baseCenter + d0 * r1 + Vector3.Up * height, t1 = baseCenter + d1 * r1 + Vector3.Up * height;
            // outward face: seen from outside, b0 → b1 → t1 → t0 is clockwise (angle grows clockwise seen from above)
            if (r1 > 1e-4f) Quad(b0, b1, t1, t0, col);
            else Tri(b0, b1, t0, col);
            if (caps)
            {
                Color cc = capColor ?? col;
                if (r1 > 1e-4f) Tri(baseCenter + Vector3.Up * height, t0, t1, cc);
                Tri(baseCenter, b1, b0, cc);
            }
        }
    }

    /// <summary>Cone (apex up) — convenience for conifer tiers.</summary>
    public void Cone(Vector3 baseCenter, float radius, float height, int seg, Color col, float twist = 0f, bool bottom = true)
    {
        Vector3 apex = baseCenter + Vector3.Up * height;
        for (int i = 0; i < seg; i++)
        {
            float a0 = (i / (float)seg) * MathF.Tau + twist, a1 = ((i + 1) / (float)seg) * MathF.Tau + twist;
            Vector3 b0 = baseCenter + new Vector3(MathF.Cos(a0), 0, MathF.Sin(a0)) * radius;
            Vector3 b1 = baseCenter + new Vector3(MathF.Cos(a1), 0, MathF.Sin(a1)) * radius;
            Tri(b0, b1, apex, col);
            if (bottom) Tri(baseCenter, b1, b0, col);
        }
    }

    static readonly Vector3[] IcoV;
    static readonly int[] IcoF;
    static MeshKit()
    {
        float t = (1f + MathF.Sqrt(5f)) / 2f;
        IcoV = new[]
        {
            new Vector3(-1, t, 0), new Vector3(1, t, 0), new Vector3(-1, -t, 0), new Vector3(1, -t, 0),
            new Vector3(0, -1, t), new Vector3(0, 1, t), new Vector3(0, -1, -t), new Vector3(0, 1, -t),
            new Vector3(t, 0, -1), new Vector3(t, 0, 1), new Vector3(-t, 0, -1), new Vector3(-t, 0, 1),
        };
        for (int i = 0; i < IcoV.Length; i++) IcoV[i] = IcoV[i].Normalized();
        IcoF = new[]
        {
            0, 11, 5, 0, 5, 1, 0, 1, 7, 0, 7, 10, 0, 10, 11, 1, 5, 9, 5, 11, 4, 11, 10, 2, 10, 7, 6, 7, 1, 8,
            3, 9, 4, 3, 4, 2, 3, 2, 6, 3, 6, 8, 3, 8, 9, 4, 9, 5, 2, 4, 11, 6, 2, 10, 8, 6, 7, 9, 8, 1,
        };
    }

    /// <summary>Low-poly blob: icosahedron (sub=0, 20 tris) or once-subdivided (sub=1, 80 tris), scaled and
    /// jittered deterministically. Good for foliage clumps and rocks.</summary>
    public void Blob(Vector3 center, Vector3 radius, Color col, int sub, float jitter, uint seed, Func<Vector3, Color> shade = null)
    {
        var verts = new List<Vector3>(IcoV);
        var faces = new List<int>(IcoF);
        if (sub > 0)
        {
            var cache = new Dictionary<long, int>();
            int Mid(int a, int b)
            {
                long key = a < b ? ((long)a << 32) | (uint)b : ((long)b << 32) | (uint)a;
                if (cache.TryGetValue(key, out int m)) return m;
                verts.Add(((verts[a] + verts[b]) * 0.5f).Normalized());
                cache[key] = verts.Count - 1;
                return verts.Count - 1;
            }
            var nf = new List<int>();
            for (int i = 0; i < faces.Count; i += 3)
            {
                int a = faces[i], b = faces[i + 1], c = faces[i + 2];
                int ab = Mid(a, b), bc = Mid(b, c), ca = Mid(c, a);
                nf.AddRange(new[] { a, ab, ca, b, bc, ab, c, ca, bc, ab, bc, ca });
            }
            faces = nf;
        }
        var rng = new Rng(seed * 2654435761u + 17u);
        var pos = new Vector3[verts.Count];
        for (int i = 0; i < verts.Count; i++)
        {
            float k = 1f + (rng.F() - 0.5f) * 2f * jitter;
            pos[i] = center + verts[i] * radius * k;
        }
        // icosahedron faces above are counter-clockwise from outside → emit reversed for Godot
        for (int i = 0; i < faces.Count; i += 3)
        {
            Vector3 a = pos[faces[i]], b = pos[faces[i + 1]], c = pos[faces[i + 2]];
            Color fc = shade != null ? shade((a + b + c) / 3f - center) : col;
            Tri(a, c, b, fc);
        }
    }

    /// <summary>A tapered blade/leaf: base centre, direction (up-ish), width at the base, bend toward `lean`.</summary>
    public void Blade(Vector3 root, Vector3 tip, float width, Color baseCol, Color tipCol, Vector3 side)
    {
        Vector3 s = side.Normalized() * width * 0.5f;
        Vector3 mid = root.Lerp(tip, 0.5f) + (tip - root).Length() * 0.05f * Vector3.Up;
        Color mc = baseCol.Lerp(tipCol, 0.5f);
        // two-sided via cull_disabled material; emit once
        Tri(root - s, mid - s * 0.6f, root + s, baseCol);
        Tri(root + s, mid - s * 0.6f, mid + s * 0.6f, mc);
        Tri(mid - s * 0.6f, tip, mid + s * 0.6f, tipCol);
    }

    public ArrayMesh ToMesh(Material material = null)
    {
        var arrays = new Godot.Collections.Array();
        arrays.Resize((int)Mesh.ArrayType.Max);
        arrays[(int)Mesh.ArrayType.Vertex] = _v.ToArray();
        arrays[(int)Mesh.ArrayType.Normal] = _n.ToArray();
        arrays[(int)Mesh.ArrayType.Color] = _c.ToArray();
        var m = new ArrayMesh();
        if (_v.Count == 0) return m;
        m.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
        if (material != null) m.SurfaceSetMaterial(0, material);
        return m;
    }

    public void Clear() { _v.Clear(); _n.Clear(); _c.Clear(); Transform = Transform3D.Identity; }

    /// <summary>Linear-space colour from sRGB hex with the given A (mask/slot) value.</summary>
    public static Color C(uint srgb, float a = 0f)
    {
        var c = FMath.Hex(srgb).SrgbToLinear();
        return new Color(c.R, c.G, c.B, a);
    }

    /// <summary>Grey shade for tint-masked parts (A = 1).</summary>
    public static Color Mask(float shade) => new(shade, shade, shade, 1f);
}
