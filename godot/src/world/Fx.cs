using System;
using Godot;

namespace FD.World;

/// <summary>Cheap particle effects: flames (forge, campfire, torches) and chimney/fire smoke.</summary>
public static class Fx
{
    static StandardMaterial3D _flameMat, _smokeMat;
    static QuadMesh _flameQuad, _smokeQuad;

    static GradientTexture2D RadialTex(Color inner, Color outer)
    {
        var g = new Gradient();
        g.SetColor(0, inner);
        g.SetColor(1, outer);
        return new GradientTexture2D { Gradient = g, Fill = GradientTexture2D.FillEnum.Radial, FillFrom = new Vector2(0.5f, 0.5f), FillTo = new Vector2(0.5f, 0f), Width = 64, Height = 64 };
    }

    static void Init()
    {
        if (_flameMat != null) return;
        _flameMat = new StandardMaterial3D
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            BillboardMode = BaseMaterial3D.BillboardModeEnum.Particles,
            Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
            BlendMode = BaseMaterial3D.BlendModeEnum.Add,
            VertexColorUseAsAlbedo = true,
            AlbedoTexture = RadialTex(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0)),
            NoDepthTest = false,
        };
        _smokeMat = new StandardMaterial3D
        {
            // Tur 1 D: lit by the sky (dark at night, grey under clouds) instead of glowing
            ShadingMode = BaseMaterial3D.ShadingModeEnum.PerVertex, Roughness = 1f, DisableReceiveShadows = true,
            BillboardMode = BaseMaterial3D.BillboardModeEnum.Particles,
            Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
            VertexColorUseAsAlbedo = true,
            AlbedoTexture = RadialTex(new Color(1, 1, 1, 0.85f), new Color(1, 1, 1, 0)),
        };
        _flameQuad = new QuadMesh { Size = new Vector2(0.4f, 0.4f), Material = _flameMat };
        _smokeQuad = new QuadMesh { Size = new Vector2(1f, 1f), Material = _smokeMat };
    }

    public static CpuParticles3D Fire(Node parent, Vector3 pos, float size)
    {
        Init();
        var g = new Gradient();
        g.SetColor(0, new Color(1.0f, 0.85f, 0.4f, 0.9f));
        g.AddPoint(0.4f, new Color(1.0f, 0.45f, 0.08f, 0.7f));
        g.SetColor(g.GetPointCount() - 1, new Color(0.6f, 0.1f, 0.02f, 0f));
        var p = new CpuParticles3D
        {
            Name = "Fire", Position = pos, Amount = Math.Clamp((int)(size * 60), 8, 40), Lifetime = 0.7f,
            Mesh = _flameQuad, EmissionShape = CpuParticles3D.EmissionShapeEnum.Sphere, EmissionSphereRadius = size * 0.45f,
            Direction = Vector3.Up, Spread = 12f, Gravity = new Vector3(0, 1.6f, 0),
            InitialVelocityMin = 0.3f, InitialVelocityMax = 0.8f,
            ScaleAmountMin = size * 1.6f, ScaleAmountMax = size * 3.2f, ColorRamp = g,
            LocalCoords = false,
        };
        var curve = new Curve();
        curve.AddPoint(new Vector2(0, 0.6f)); curve.AddPoint(new Vector2(0.3f, 1f)); curve.AddPoint(new Vector2(1, 0.1f));
        p.ScaleAmountCurve = curve;
        p.VisibilityAabb = new Aabb(new Vector3(-2, -1, -2), new Vector3(4, 5, 4));
        parent.AddChild(p);
        return p;
    }

    public static CpuParticles3D Smoke(Node parent, Vector3 pos, float size)
    {
        Init();
        var g = new Gradient();
        g.SetColor(0, new Color(0.55f, 0.55f, 0.55f, 0f));
        g.AddPoint(0.15f, new Color(0.62f, 0.62f, 0.63f, 0.35f));
        g.SetColor(g.GetPointCount() - 1, new Color(0.75f, 0.75f, 0.78f, 0f));
        var p = new CpuParticles3D
        {
            Name = "Smoke", Position = pos, Amount = 16, Lifetime = 7f,
            Mesh = _smokeQuad, EmissionShape = CpuParticles3D.EmissionShapeEnum.Sphere, EmissionSphereRadius = size * 0.3f,
            Direction = Vector3.Up, Spread = 8f, Gravity = new Vector3(0.25f, 0.12f, 0.1f),
            InitialVelocityMin = 0.5f, InitialVelocityMax = 0.9f,
            ScaleAmountMin = size * 0.8f, ScaleAmountMax = size * 1.4f, ColorRamp = g,
            LocalCoords = false,
        };
        var curve = new Curve();
        curve.AddPoint(new Vector2(0, 0.4f)); curve.AddPoint(new Vector2(1, 2.6f));
        p.ScaleAmountCurve = curve;
        p.VisibilityAabb = new Aabb(new Vector3(-6, -1, -6), new Vector3(12, 16, 12));
        parent.AddChild(p);
        return p;
    }
}
