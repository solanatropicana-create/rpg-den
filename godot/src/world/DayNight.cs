using System;
using Godot;
using FD.Core;

namespace FD.World;

/// <summary>
/// Global game clock. Default speed (Faz 2): 1 game day = 30 real minutes (<see cref="TimeScale"/> = 48 game seconds per
/// real second). Read <see cref="Hour"/>, <see cref="Day"/>, <see cref="IsNight"/> anywhere; advanced by
/// <see cref="DayNight"/> every frame. Sun position is a pure function of the hour (see <see cref="SunDirection"/>),
/// so simulation code can reason about light without the scene.
/// </summary>
public static class GameClock
{
    /// <summary>Game hours since day 0, 00:00. Starts on day 0 at 09:00.</summary>
    public static double TotalHours { get; private set; } = 9.0;
    /// <summary>Game seconds per real second. Faz 2: 48 → a 30-minute day (yol haritası: 1 gün = 30 dk; makro dünyanın bir günü),
    /// night ≈ 8 real minutes. Settable (dev: --timescale=K).</summary>
    public static float TimeScale = 48f;
    public static bool Paused;

    public static float Hour => (float)(TotalHours % 24.0);
    public static int Day => (int)Math.Floor(TotalHours / 24.0);
    /// <summary>Sun elevation in degrees at the current hour.</summary>
    public static float SunElevation => Mathf.RadToDeg(MathF.Asin(SunDirection(Hour).Y));
    /// <summary>Night = sun more than 6° below the horizon (after civil dusk, before civil dawn).</summary>
    public static bool IsNight => SunElevation < -6f;
    /// <summary>0 at night … 1 in full daylight (sun ≥ 10°).</summary>
    public static float Daylight => FMath.Smoothstep(-6f, 10f, SunElevation);

    /// <summary>Raised whenever the whole hour changes (arg = new hour 0..23).</summary>
    public static event Action<int> HourChanged;

    /// <summary>Faz 2 (kayıt): restore the absolute clock.</summary>
    public static void SetTotalHours(double h) { TotalHours = Math.Max(0, h); HourChanged?.Invoke((int)Hour); }

    public static void SetHour(float hour)
    {
        int prev = (int)Hour;
        TotalHours = Day * 24.0 + ((hour % 24f) + 24f) % 24f;
        if ((int)Hour != prev) HourChanged?.Invoke((int)Hour);
    }

    public static void Advance(double realSeconds)
    {
        if (Paused || TimeScale <= 0) return;
        int prev = (int)Hour;
        TotalHours += realSeconds * TimeScale / 3600.0;
        if ((int)Hour != prev) HourChanged?.Invoke((int)Hour);
    }

    public static string TimeString => $"Gün {Day + 1}, {(int)Hour:00}:{(int)(Hour % 1f * 60f):00}";

    // Sun path (Faz 2: kısa gece): latitude 50°N, midsummer declination 23°, local clock 0.8 h ahead of solar time
    // → sunrise ≈ 04:47, noon ≈ 12:48, sunset ≈ 20:50; night (sun < −6°) 21:41–03:56 ≈ 6,25 h ≈ 8 real minutes at TimeScale 48.
    const float Latitude = 50f, Declination = 23f, ClockOffset = 0.8f;

    /// <summary>Unit vector pointing TO the sun (x east, y up, z south) for a clock hour.</summary>
    public static Vector3 SunDirection(float hour)
    {
        float ha = Mathf.DegToRad((hour - ClockOffset - 12f) * 15f);
        float lat = Mathf.DegToRad(Latitude), dec = Mathf.DegToRad(Declination);
        float east = -MathF.Cos(dec) * MathF.Sin(ha);
        float north = MathF.Sin(dec) * MathF.Cos(lat) - MathF.Cos(dec) * MathF.Cos(ha) * MathF.Sin(lat);
        float up = MathF.Sin(dec) * MathF.Sin(lat) + MathF.Cos(dec) * MathF.Cos(ha) * MathF.Cos(lat);
        return new Vector3(east, up, -north).Normalized();
    }

    /// <summary>Full moon: roughly opposite the sun, a little higher so moonlit nights have long soft shadows.</summary>
    public static Vector3 MoonDirection(float hour)
    {
        Vector3 s = -SunDirection(hour);
        return new Vector3(s.X * 0.9f, s.Y + 0.25f, s.Z * 0.9f + 0.15f).Normalized();
    }
}

/// <summary>
/// Sky, sun/moon lights, fog and post-processing driven by <see cref="GameClock"/>. Also owns the global shader
/// parameters: <c>night_light</c> (0 day … 1 night, windows glow), <c>fd_cam_pos</c> (active camera position,
/// used by vegetation LOD fades), <c>fd_wind</c> (xy = direction, z = strength, w = gust).
/// </summary>
public partial class DayNight : Node3D
{
    public DirectionalLight3D Sun { get; private set; }
    public DirectionalLight3D Moon { get; private set; }
    public WorldEnvironment WorldEnv { get; private set; }
    public Godot.Environment Env { get; private set; }
    public ShaderMaterial SkyMaterial { get; private set; }

    /// <summary>Extra exposure multiplier at night so moonlit scenes stay readable.</summary>
    [Export] public float NightExposure = 1.45f;
    /// <summary>Global exposure multiplier (look-dev / settings).</summary>
    [Export] public float ExposureScale = 1f;

    float _skyTimer = 999f, _lastSkyHour = -100f;
    double _time;

    public override void _Ready()
    {
        Name = "DayNight";
        ProcessPriority = 1000;   // after cameras moved this frame (fd_cam_pos drives vegetation/grass LOD)
        SkyMaterial = new ShaderMaterial { Shader = GD.Load<Shader>("res://assets/shaders/sky.gdshader") };
        SkyMaterial.SetShaderParameter("cloud_noise", Terrain.MakeNoise(RegionSpec.Seed + 99, 0.006f, 5, 256));
        Env = new Godot.Environment
        {
            BackgroundMode = Godot.Environment.BGMode.Sky,
            Sky = new Sky { SkyMaterial = SkyMaterial, RadianceSize = Sky.RadianceSizeEnum.Size128, ProcessMode = Sky.ProcessModeEnum.Incremental },
            AmbientLightSource = Godot.Environment.AmbientSource.Sky,
            ReflectedLightSource = Godot.Environment.ReflectionSource.Sky,
            TonemapMode = Godot.Environment.ToneMapper.Filmic,
            TonemapExposure = 1.0f,
            SsaoEnabled = true, SsaoRadius = 1.4f, SsaoIntensity = 1.6f, SsaoPower = 1.4f, SsaoDetail = 0.5f,
            SsaoLightAffect = 0.12f,
            GlowEnabled = true, GlowIntensity = 0.45f, GlowStrength = 1.0f, GlowBloom = 0.03f, GlowHdrThreshold = 1.2f,
            GlowBlendMode = Godot.Environment.GlowBlendModeEnum.Softlight,
            FogEnabled = true, FogMode = Godot.Environment.FogModeEnum.Exponential,
            FogDensity = 0.0009f, FogAerialPerspective = 0.55f, FogSkyAffect = 0.25f,
            FogHeight = -7f, FogHeightDensity = 0.0f, FogSunScatter = 0.1f,
            AdjustmentEnabled = true, AdjustmentSaturation = 1.0f, AdjustmentContrast = 1.03f,
        };
        WorldEnv = new WorldEnvironment { Name = "WorldEnvironment", Environment = Env };
        AddChild(WorldEnv);

        Sun = new DirectionalLight3D
        {
            Name = "Sun", ShadowEnabled = true,
            DirectionalShadowMode = DirectionalLight3D.ShadowMode.Parallel4Splits,
            DirectionalShadowMaxDistance = 300f,
            DirectionalShadowSplit1 = 0.06f, DirectionalShadowSplit2 = 0.18f, DirectionalShadowSplit3 = 0.45f,
            DirectionalShadowBlendSplits = true, DirectionalShadowFadeStart = 0.85f,
            ShadowBias = 0.04f, ShadowNormalBias = 1.2f, ShadowBlur = 1.2f,
            LightAngularDistance = 0f,
            SkyMode = DirectionalLight3D.SkyModeEnum.LightOnly,
        };
        AddChild(Sun);
        Moon = new DirectionalLight3D
        {
            Name = "Moon", ShadowEnabled = true,
            DirectionalShadowMode = DirectionalLight3D.ShadowMode.Parallel2Splits,
            DirectionalShadowMaxDistance = 160f, DirectionalShadowSplit1 = 0.2f,
            ShadowBias = 0.05f, ShadowNormalBias = 1.5f, ShadowBlur = 2.0f,
            LightColor = FMath.Hex(0x8fa6ff), LightEnergy = 0f, Visible = false,
            SkyMode = DirectionalLight3D.SkyModeEnum.LightOnly,
        };
        AddChild(Moon);
        Apply(true);
    }

    public override void _Process(double delta)
    {
        GameClock.Advance(delta);
        _time += delta;
        Apply(false, (float)delta);
        var cam = GetViewport().GetCamera3D();
        if (cam != null) RenderingServer.GlobalShaderParameterSet("fd_cam_pos", cam.GlobalPosition);
    }

    // ---------------------------------------------------------------------------------------------- keyframes

    static readonly float[] Keys = { -18f, -10f, -4f, 0f, 5f, 15f, 35f };
    static readonly uint[] Zenith = { 0x040914, 0x0b1430, 0x26335e, 0x3d5688, 0x4670ac, 0x3d7cc8, 0x3778c9 };
    static readonly uint[] Horizon = { 0x0a1122, 0x182040, 0x4b4a70, 0x9d8f9e, 0xc6c2bd, 0xb5cde4, 0xa9c9e7 };
    static readonly uint[] HorizonSun = { 0x0a1122, 0x2a2442, 0xb2625a, 0xf49256, 0xf7c38c, 0xe9dcc2, 0xc9ddef };
    static readonly uint[] SunCol = { 0xff6a30, 0xff6a30, 0xff7438, 0xff8a46, 0xffb472, 0xffe2bc, 0xfff4e6 };

    static Color Key(uint[] table, float e)
    {
        if (e <= Keys[0]) return FMath.Hex(table[0]);
        for (int i = 0; i < Keys.Length - 1; i++)
            if (e <= Keys[i + 1])
            {
                float t = (e - Keys[i]) / (Keys[i + 1] - Keys[i]);
                t = t * t * (3 - 2 * t);
                return FMath.Hex(table[i]).Lerp(FMath.Hex(table[i + 1]), t);
            }
        return FMath.Hex(table[^1]);
    }

    /// <summary>Update lights/fog/exposure every frame, the sky material only when it visibly changes.</summary>
    void Apply(bool force, float dt = 0f)
    {
        float hour = GameClock.Hour;
        Vector3 sunDir = GameClock.SunDirection(hour);
        Vector3 moonDir = GameClock.MoonDirection(hour);
        float e = Mathf.RadToDeg(MathF.Asin(sunDir.Y));
        float em = Mathf.RadToDeg(MathF.Asin(moonDir.Y));

        // --- sun
        float sunE = FMath.Smoothstep(-1.5f, 3f, e) * FMath.Lerp(0.7f, 1.5f, FMath.Smoothstep(2f, 30f, e));
        Sun.LightColor = Key(SunCol, e);
        Sun.LightEnergy = sunE;
        Sun.Visible = sunE > 0.002f;
        if (Sun.Visible) Sun.Basis = Basis.LookingAt(-SafeLightDir(sunDir), Vector3.Up);

        // --- moon (fades in after dusk)
        float moonE = 0.26f * FMath.Smoothstep(-2f, -9f, e) * FMath.Smoothstep(2f, 14f, em);
        Moon.LightEnergy = moonE;
        Moon.Visible = moonE > 0.002f;
        if (Moon.Visible) Moon.Basis = Basis.LookingAt(-SafeLightDir(moonDir), Vector3.Up);

        // --- ambient, fog, exposure
        float day = FMath.Smoothstep(-8f, 8f, e);
        float night = 1f - FMath.Smoothstep(-12f, -3f, e);
        Color hor = Key(Horizon, e), horSun = Key(HorizonSun, e);
        Env.AmbientLightColor = FMath.Hex(0x2a3a64).Lerp(FMath.Hex(0x8a9bb0), day);
        Env.AmbientLightSkyContribution = FMath.Lerp(0.35f, 0.9f, day);
        float dusk = 1f - FMath.Smoothstep(4f, 14f, MathF.Abs(e - 1f));
        Env.AmbientLightEnergy = FMath.Lerp(0.55f, 0.8f, day) * (1f - 0.25f * dusk);
        Env.FogLightColor = hor.Lerp(horSun, 0.25f + 0.3f * dusk).Darkened(0.05f);
        Env.FogLightEnergy = FMath.Lerp(0.5f, 1.0f, day);
        Env.FogSunScatter = 0.08f + 0.22f * dusk;
        Env.FogDensity = FMath.Lerp(0.0012f, 0.0009f, day);
        // a little ground mist around sunrise
        float morning = hour > 3f && hour < 10f ? 1f - FMath.Smoothstep(2f, 12f, MathF.Abs(e - 2f)) : 0f;
        // Godot's height fog is a constant amount below FogHeight (not integrated along the view ray), so keep it
        // to a thin dawn mist in the lowest hollows (stream valley) only.
        Env.FogHeightDensity = 0.035f * morning;
        Env.TonemapExposure = ExposureScale * FMath.Lerp(0.8f, NightExposure, night) * FMath.Lerp(1f, 1.12f, dusk);
        Env.GlowIntensity = FMath.Lerp(0.45f, 0.9f, night);
        Env.GlowHdrThreshold = FMath.Lerp(1.2f, 0.75f, night);
        // full midday sun pushes the greens toward lime under Filmic: ease saturation a little when the sun is high
        Env.AdjustmentSaturation = FMath.Lerp(1.0f - 0.08f * FMath.Smoothstep(20f, 50f, e), 0.72f, night);

        // window lights: on from sunset to dawn
        RenderingServer.GlobalShaderParameterSet("night_light", 1f - FMath.Smoothstep(-3f, 7f, e));
        float gust = 0.5f + 0.5f * MathF.Sin((float)_time * 0.23f) * MathF.Sin((float)_time * 0.071f + 1.3f);
        RenderingServer.GlobalShaderParameterSet("fd_wind", new Vector4(0.83f, 0.55f, 0.55f + 0.35f * gust, gust));

        // --- sky material: throttle (every radiance update is expensive)
        _skyTimer += dt;
        if (force || MathF.Abs(hour - _lastSkyHour) > 0.02f || _skyTimer > 2f)
        {
            _skyTimer = 0; _lastSkyHour = hour;
            SkyMaterial.SetShaderParameter("zenith_color", Key(Zenith, e));
            SkyMaterial.SetShaderParameter("horizon_color", hor);
            SkyMaterial.SetShaderParameter("horizon_sun_color", horSun);
            SkyMaterial.SetShaderParameter("ground_color", hor.Darkened(0.35f));
            SkyMaterial.SetShaderParameter("sun_dir", sunDir);
            SkyMaterial.SetShaderParameter("sun_color", Key(SunCol, e).Lerp(new Color(1, 1, 1), 0.15f));
            SkyMaterial.SetShaderParameter("sun_glow", FMath.Smoothstep(-8f, 0f, e) * FMath.Lerp(1.4f, 0.7f, FMath.Smoothstep(3f, 30f, e)));
            SkyMaterial.SetShaderParameter("sun_disk", FMath.Smoothstep(-1.5f, 0.5f, e));
            SkyMaterial.SetShaderParameter("moon_dir", moonDir);
            SkyMaterial.SetShaderParameter("moon_amount", FMath.Smoothstep(-2f, -8f, e) * FMath.Smoothstep(-2f, 4f, em));
            SkyMaterial.SetShaderParameter("star_amount", FMath.Smoothstep(-5f, -13f, e) * 1.4f);
            SkyMaterial.SetShaderParameter("sky_time", (float)(_time % 10000.0));
            SkyMaterial.SetShaderParameter("cloud_lit", Key(HorizonSun, e).Lerp(new Color(1, 1, 1), 0.55f * day));
            SkyMaterial.SetShaderParameter("cloud_dark", Key(Horizon, e).Darkened(FMath.Lerp(0.55f, 0.25f, day)));
            SkyMaterial.SetShaderParameter("cloud_alpha", FMath.Lerp(0.55f, 0.85f, day));
        }
    }

    /// <summary>Keep light directions a few degrees above the horizon so shadows don't stretch to infinity.</summary>
    static Vector3 SafeLightDir(Vector3 d)
    {
        if (d.Y >= 0.06f) return d;
        return new Vector3(d.X, 0.06f, d.Z).Normalized();
    }
}
