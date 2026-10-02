using System;
using System.Collections.Generic;
using Godot;
using FD.Core;

namespace FD.World;

/// <summary>
/// Tur 1 D: what makes the air of the region — rain streaks around the camera, wind-blown dust and leaves on dry windy days,
/// fog banks in the stream valley, over the pond and in the deep forest (volumetric fog volumes, thick at dawn, at dusk and on
/// foggy mornings), a lantern in the hand of the controlled character at night, a faint film grain and vignette over the view,
/// and the sound of the place (<see cref="AmbientSound"/>: wind first, rain, fires; the nights quiet).
/// </summary>
public partial class Atmosphere : Node3D
{
    GpuParticles3D _rain, _dust;
    OmniLight3D _lantern;
    readonly List<(FogVolume v, FogMaterial m, float baseDensity, bool valley)> _fog = new();
    ColorRect _vignette, _grain;
    ShaderMaterial _vigMat, _grainMat;

    public override void _Ready()
    {
        var r = Region.Current;
        bool headless = DisplayServer.GetName() == "headless";
        if (!headless)
        {
            BuildRain();
            BuildDust();
            BuildOverlay();
        }
        if (r?.Heightfield != null) BuildFog(r.Heightfield);
        _lantern = new OmniLight3D
        {
            Name = "Lantern", LightColor = new Color(1f, 0.62f, 0.3f), OmniRange = 7.5f, OmniAttenuation = 1.3f, LightEnergy = 0f,
            ShadowEnabled = false, Visible = false, LightVolumetricFogEnergy = 0.6f,
        };
        AddChild(_lantern);
        AddChild(new AmbientSound { Name = "AmbientSound" });
    }

    // ------------------------------------------------------------------------------------------------ rain, dust, leaves
    void BuildRain()
    {
        var mat = new StandardMaterial3D
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded, Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
            BillboardMode = BaseMaterial3D.BillboardModeEnum.FixedY, AlbedoColor = new Color(0.72f, 0.75f, 0.8f, 0.32f),
            CullMode = BaseMaterial3D.CullModeEnum.Disabled,
        };
        var pm = new ParticleProcessMaterial
        {
            EmissionShape = ParticleProcessMaterial.EmissionShapeEnum.Box, EmissionBoxExtents = new Vector3(24f, 1f, 24f),
            Direction = new Vector3(0.12f, -1f, 0.08f), Spread = 4f, InitialVelocityMin = 17f, InitialVelocityMax = 21f,
            Gravity = new Vector3(0, -9.8f, 0),
        };
        _rain = new GpuParticles3D
        {
            Name = "Rain", Amount = 3200, Lifetime = 1.15f, Preprocess = 1.2f, ProcessMaterial = pm, Emitting = false,
            DrawPass1 = new QuadMesh { Size = new Vector2(0.022f, 0.62f), Material = mat },
            VisibilityAabb = new Aabb(new Vector3(-30, -30, -30), new Vector3(60, 45, 60)), LocalCoords = false,
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
        };
        AddChild(_rain);
    }

    void BuildDust()
    {
        var mat = new StandardMaterial3D
        {
            Transparency = BaseMaterial3D.TransparencyEnum.Alpha, BillboardMode = BaseMaterial3D.BillboardModeEnum.Particles,
            VertexColorUseAsAlbedo = true, CullMode = BaseMaterial3D.CullModeEnum.Disabled, Roughness = 1f,
        };
        var g = new Gradient();
        g.SetColor(0, new Color(0.42f, 0.36f, 0.24f, 0f));
        g.AddPoint(0.15f, new Color(0.40f, 0.35f, 0.24f, 0.8f));
        g.AddPoint(0.85f, new Color(0.33f, 0.30f, 0.20f, 0.8f));
        g.SetColor(g.GetPointCount() - 1, new Color(0.3f, 0.28f, 0.2f, 0f));
        var pm = new ParticleProcessMaterial
        {
            EmissionShape = ParticleProcessMaterial.EmissionShapeEnum.Box, EmissionBoxExtents = new Vector3(26f, 4f, 26f),
            Direction = new Vector3(0.83f, 0.05f, 0.55f), Spread = 25f, InitialVelocityMin = 2.5f, InitialVelocityMax = 6f,
            Gravity = new Vector3(0, -0.35f, 0), AngularVelocityMin = -220f, AngularVelocityMax = 220f,
            ScaleMin = 0.6f, ScaleMax = 1.6f, ColorRamp = new GradientTexture1D { Gradient = g },
            TurbulenceEnabled = true, TurbulenceNoiseStrength = 1.4f, TurbulenceNoiseScale = 6f, TurbulenceInfluenceMin = 0.05f, TurbulenceInfluenceMax = 0.15f,
        };
        _dust = new GpuParticles3D
        {
            Name = "DustLeaves", Amount = 140, Lifetime = 6f, Preprocess = 4f, ProcessMaterial = pm, Emitting = false,
            DrawPass1 = new QuadMesh { Size = new Vector2(0.09f, 0.06f), Material = mat },
            VisibilityAabb = new Aabb(new Vector3(-40, -10, -40), new Vector3(80, 25, 80)), LocalCoords = false,
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
        };
        AddChild(_dust);
    }

    // ------------------------------------------------------------------------------------------------ fog banks
    void BuildFog(Heightfield hf)
    {
        // the stream valley: a bank every 38 m along the carved stream, lying in the bed
        var st = RegionSpec.Stream;
        for (float s = 10f; s < st.Length; s += 38f)
        {
            var p = st.PointAt(s);
            if (MathF.Abs(p.X) > 560 || MathF.Abs(p.Y) > 560) continue;
            var t = st.TangentAt(s).Normalized();
            float y = hf.Height(p.X, p.Y);
            AddFog(new Vector3(p.X, y + 2.5f, p.Y), new Vector3(54f, 9f, 30f), MathF.Atan2(t.X, t.Y) + MathF.PI / 2, 0.045f, true, RenderingServer.FogVolumeShape.Box);
        }
        // the pond
        var pd = RegionSpec.Pond;
        AddFog(new Vector3(pd.X, hf.Height(pd.X, pd.Y) + 2f, pd.Y), new Vector3(90f, 10f, 90f), 0f, 0.04f, true, RenderingServer.FogVolumeShape.Ellipsoid);
        // the deep forest: banks where the trees are thickest (a coarse grid, the densest cells first, at most 18)
        var cells = new List<(float d, Vector2 p)>();
        for (float x = -520; x <= 520; x += 80)
            for (float z = -520; z <= 520; z += 80)
            {
                float d = hf.ForestDensity(x, z);
                if (d > 0.62f) cells.Add((d, new Vector2(x, z)));
            }
        cells.Sort((a, b) => b.d.CompareTo(a.d));
        for (int i = 0; i < cells.Count && i < 18; i++)
        {
            var p = cells[i].p;
            AddFog(new Vector3(p.X, hf.Height(p.X, p.Y) + 4f, p.Y), new Vector3(110f, 16f, 110f), 0f, 0.03f, false, RenderingServer.FogVolumeShape.Ellipsoid);
        }
    }

    void AddFog(Vector3 pos, Vector3 size, float yaw, float density, bool valley, RenderingServer.FogVolumeShape shape)
    {
        var m = new FogMaterial { Density = density, Albedo = new Color(0.8f, 0.82f, 0.85f), HeightFalloff = 0.12f, EdgeFade = 0.6f };
        var v = new FogVolume { Size = size, Shape = shape, Material = m, Position = pos, Rotation = new Vector3(0, yaw, 0) };
        AddChild(v);
        _fog.Add((v, m, density, valley));
    }

    // ------------------------------------------------------------------------------------------------ grain and vignette
    void BuildOverlay()
    {
        var layer = new CanvasLayer { Name = "FilmLayer", Layer = 2 };
        AddChild(layer);
        _vigMat = new ShaderMaterial { Shader = new Shader { Code = @"shader_type canvas_item;
uniform float strength = 0.35;
void fragment() {
	vec2 d = UV - 0.5; d.x *= 1.35;
	float v = smoothstep(0.32, 0.86, length(d));
	COLOR = vec4(0.02, 0.025, 0.035, v * strength);
}" } };
        _vignette = new ColorRect { MouseFilter = Control.MouseFilterEnum.Ignore, Material = _vigMat };
        _vignette.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        layer.AddChild(_vignette);
        _grainMat = new ShaderMaterial { Shader = new Shader { Code = @"shader_type canvas_item;
uniform float strength = 0.035;
uniform float seed = 0.0;
void fragment() {
	vec2 p = floor(FRAGCOORD.xy / 1.5);
	float n = fract(sin(dot(p + seed * vec2(37.1, 91.7), vec2(12.9898, 78.233))) * 43758.5453);
	COLOR = vec4(vec3(n), strength);
}" } };
        _grain = new ColorRect { MouseFilter = Control.MouseFilterEnum.Ignore, Material = _grainMat };
        _grain.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        layer.AddChild(_grain);
    }

    public override void _Process(double delta)
    {
        var cam = GetViewport().GetCamera3D();
        float night = 1f - GameClock.Daylight;
        float e = GameClock.SunElevation;
        float dusk = 1f - FMath.Smoothstep(4f, 14f, MathF.Abs(e - 1f));
        if (_rain != null && cam != null)
        {
            var cp = cam.GlobalPosition;
            _rain.GlobalPosition = cp + new Vector3(0, 13f, 0) - cam.GlobalBasis.Z * 8f;
            _rain.AmountRatio = Math.Clamp(Weather.Rain, 0f, 1f);
            _rain.Emitting = Weather.Rain > 0.03f;
            bool windy = Weather.Wind > 0.55f && Weather.Wet < 0.2f && cp.Y < (Region.Current?.Heightfield.Height(cp.X, cp.Z) ?? 0) + 45f;
            _dust.Emitting = windy;
            _dust.AmountRatio = Math.Clamp((Weather.Wind - 0.5f) * 2f, 0.2f, 1f);
            _dust.GlobalPosition = cp - cam.GlobalBasis.Z * 10f + new Vector3(0, -2f, 0);
        }
        // fog banks: always a little in the valley; thick at dawn, at dusk and on foggy mornings
        float valleyK = 0.25f + 1.6f * Weather.Fog + 0.7f * dusk + 0.4f * Weather.Rain + 0.3f * night;
        float forestK = 0.35f + 1.1f * Weather.Fog + 0.5f * dusk + 0.4f * Weather.Rain + 0.3f * night;
        foreach (var (v, m, d, valley) in _fog) m.Density = d * (valley ? valleyK : forestK);
        // a lantern for whoever the player controls, at night
        var pl = Region.Current?.Player;
        if (pl != null)
        {
            float on = FMath.Smoothstep(0.55f, 0.9f, night) * (pl.Visible ? 1f : 0f);
            _lantern.Visible = on > 0.01f;
            _lantern.LightEnergy = 1.25f * on;
            float f = pl.Facing;
            _lantern.GlobalPosition = pl.GlobalPosition + new Vector3(MathF.Sin(f) * 0.35f + MathF.Cos(f) * 0.25f, 1.05f, MathF.Cos(f) * 0.35f - MathF.Sin(f) * 0.25f);
        }
        if (_vigMat != null)
        {
            _vigMat.SetShaderParameter("strength", FMath.Lerp(0.32f, 0.55f, night));
            _grainMat.SetShaderParameter("strength", FMath.Lerp(0.03f, 0.05f, night));
            _grainMat.SetShaderParameter("seed", (float)(Time.GetTicksMsec() % 100000) * 0.013f);
        }
    }
}

/// <summary>
/// Tur 1 D: the sound of the place, made on the fly (no sound files): the wind first — a rushing band of noise that swells with the
/// gusts and the weather — the rain's hiss and patter, the crackle of a fire when one is near. No tunes; the nights are quiet but
/// for the wind. Runs while the game is paused.
/// </summary>
public partial class AmbientSound : Node
{
    const int Rate = 22050;
    AudioStreamGeneratorPlayback _wind, _rain;
    AudioStreamPlayer _windP, _rainP;
    readonly Random _rng = new(1234);
    double _t;
    float _b1, _b2, _lp, _hp, _rlp, _crk;
    float _gust = 0.5f, _gustGoal = 0.5f, _gustT;
    readonly List<Vector3> _fires = new();
    float _fireNear;
    bool _ok;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        if (DisplayServer.GetName() == "headless" || AudioServer.GetDriverName() == "Dummy") return;
        _windP = new AudioStreamPlayer { Stream = new AudioStreamGenerator { MixRate = Rate, BufferLength = 0.3f }, VolumeDb = -6f };
        _rainP = new AudioStreamPlayer { Stream = new AudioStreamGenerator { MixRate = Rate, BufferLength = 0.3f }, VolumeDb = -8f };
        AddChild(_windP); AddChild(_rainP);
        _windP.Play(); _rainP.Play();
        _wind = _windP.GetStreamPlayback() as AudioStreamGeneratorPlayback;
        _rain = _rainP.GetStreamPlayback() as AudioStreamGeneratorPlayback;
        _ok = _wind != null && _rain != null;
        CallDeferred(nameof(FindFires));
    }

    void FindFires()
    {
        var root = Region.Current;
        if (root == null) return;
        void Walk(Node n) { if (n is NightLight l && l.AlwaysOn && l.Flicker) _fires.Add(l.GlobalPosition); foreach (var c in n.GetChildren()) Walk(c); }
        Walk(root);
    }

    float N() => (float)_rng.NextDouble() * 2f - 1f;

    public override void _Process(double delta)
    {
        if (!_ok) return;
        float dt = (float)delta;
        _t += dt;
        // gusts wander toward a new goal every few seconds
        _gustT -= dt;
        if (_gustT <= 0) { _gustT = 2f + (float)_rng.NextDouble() * 5f; _gustGoal = 0.25f + (float)_rng.NextDouble() * 0.75f; }
        _gust += (_gustGoal - _gust) * MathF.Min(1f, dt * 0.6f);
        var cam = GetViewport().GetCamera3D();
        _fireNear = 0;
        if (cam != null) foreach (var f in _fires) _fireNear = MathF.Max(_fireNear, 1f - Math.Clamp((cam.GlobalPosition.DistanceTo(f) - 3f) / 14f, 0f, 1f));
        float night = 1f - GameClock.Daylight;
        float windAmp = (0.18f + 0.5f * Weather.Wind) * (0.55f + 0.6f * _gust) * (1f + 0.25f * night) * (GetTree().Paused ? 0.6f : 1f);
        float rainAmp = Weather.Rain * 0.55f;
        Fill(_wind, windAmp, true);
        Fill(_rain, rainAmp, false);
    }

    void Fill(AudioStreamGeneratorPlayback pb, float amp, bool wind)
    {
        int n = pb.GetFramesAvailable();
        if (n <= 0) return;
        var buf = new Vector2[n];
        for (int i = 0; i < n; i++)
        {
            float w = N();
            float s;
            if (wind)
            {
                // brown-ish noise, band-limited; the cutoff breathes with the gust
                _b1 = (_b1 + 0.02f * w) / 1.02f;
                float cut = 0.035f + 0.05f * _gust;
                _lp += cut * (w * 0.6f + _b1 * 3f - _lp);
                _b2 += 0.02f * (_lp - _b2);
                s = (_lp - _b2) * 2.2f * amp;
                // fire crackle when near a fire
                if (_fireNear > 0.01f)
                {
                    if (_rng.NextDouble() < 0.0009 * _fireNear) _crk = 0.5f + (float)_rng.NextDouble() * 0.5f;
                    s += _crk * N() * 0.35f * _fireNear;
                    _crk *= 0.94f;
                }
            }
            else
            {
                // hiss (high-passed noise) with patter
                _rlp += 0.25f * (w - _rlp);
                _hp = w - _rlp;
                s = _hp * 0.35f * amp;
                if (_rng.NextDouble() < 0.004 * amp) s += N() * 0.6f * amp;
            }
            buf[i] = new Vector2(s, s);
        }
        pb.PushBuffer(buf);
    }
}
