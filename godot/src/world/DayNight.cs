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
/// Sky, sun/moon lights, fog and post-processing driven by <see cref="GameClock"/> and <see cref="Weather"/>. Also owns the global
/// shader parameters: <c>night_light</c> (0 day … 1 night, windows glow), <c>fd_cam_pos</c> (active camera position, used by
/// vegetation LOD fades), <c>fd_wind</c> (xy = direction, z = strength, w = gust), <c>fd_wet</c> (rain-soaked surfaces).
/// <para>Tur 1 D (art direction Valheim, the style frames in docs/stil): a grey-blue overcast sky most days, a low pale sun from the
/// side (its light never climbs above <see cref="SunMaxElevation"/>, so shadows stay long), haze everywhere and volumetric fog
/// thickest in the valleys, at dawn and at dusk; truly dark nights with a weak blue moon where lamps, the forge and fires carry the
/// scene; a cold-earth colour grade (see also fd_grade.gdshaderinc: every albedo ×0.60 saturation, ×0.88 brightness, dirtied).</para>
/// </summary>
public partial class DayNight : Node3D
{
    public DirectionalLight3D Sun { get; private set; }
    public DirectionalLight3D Moon { get; private set; }
    public WorldEnvironment WorldEnv { get; private set; }
    public Godot.Environment Env { get; private set; }
    public ShaderMaterial SkyMaterial { get; private set; }

    /// <summary>Extra exposure multiplier at night (1: the night stays dark; lamps and fires carry it).</summary>
    [Export] public float NightExposure = 1.0f;
    /// <summary>Global exposure multiplier (look-dev / settings).</summary>
    [Export] public float ExposureScale = 1f;
    /// <summary>The sunlight never comes from higher than this (degrees): a low pale sun and long shadows all day.</summary>
    public const float SunMaxElevation = 24f;

    // style frame values (docs/stil, brief D+), as sRGB colours (the frame's linear values converted)
    static readonly Color SunDay = new(1.0f, 0.92f, 0.81f);          // linear (1.0, 0.82, 0.62)
    static readonly Color MoonCol = new(0.77f, 0.83f, 1.0f);         // linear (0.55, 0.65, 1.0)
    static readonly Color FogDay = new(0.70f, 0.72f, 0.75f);         // style frame: linear (0.62, 0.65, 0.70), toned to the frame's look
    static readonly Color FogNight = new(0.54f, 0.57f, 0.63f);       // linear (0.25, 0.28, 0.35)
    static readonly Color SkyGrey = new(0.60f, 0.615f, 0.635f);      // style frame: linear (0.48, 0.50, 0.53) at strength 0.9
    static readonly Color SkyGreyTop = new(0.50f, 0.52f, 0.555f);
    static readonly Color NightTop = new(0.075f, 0.09f, 0.135f);     // linear (0.03, 0.04, 0.07) × 0.6
    static readonly Color NightHor = new(0.11f, 0.13f, 0.18f);

    float _skyTimer = 999f, _lastSkyHour = -100f, _lastCloud = -1f;
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
            SsaoEnabled = true, SsaoRadius = 1.4f, SsaoIntensity = 1.8f, SsaoPower = 1.5f, SsaoDetail = 0.5f,
            SsaoLightAffect = 0.15f,
            GlowEnabled = true, GlowIntensity = 0.45f, GlowStrength = 1.0f, GlowBloom = 0.02f, GlowHdrThreshold = 1.1f,
            GlowBlendMode = Godot.Environment.GlowBlendModeEnum.Softlight,
            FogEnabled = true, FogMode = Godot.Environment.FogModeEnum.Exponential,
            FogDensity = 0.0024f, FogAerialPerspective = 0.35f, FogSkyAffect = 0.55f,
            FogHeight = -7f, FogHeightDensity = 0.0f, FogSunScatter = 0.08f,
            VolumetricFogEnabled = true, VolumetricFogDensity = 0.0035f, VolumetricFogAnisotropy = 0.35f,
            VolumetricFogLength = 140f, VolumetricFogDetailSpread = 2.0f, VolumetricFogAmbientInject = 0.9f,
            VolumetricFogSkyAffect = 0.15f, VolumetricFogTemporalReprojectionEnabled = true,
            AdjustmentEnabled = true, AdjustmentSaturation = 0.9f, AdjustmentContrast = 1.1f,
            AdjustmentColorCorrection = GradeLut(),
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
            ShadowBias = 0.04f, ShadowNormalBias = 1.2f, ShadowBlur = 1.6f,
            LightAngularDistance = 0f, LightVolumetricFogEnergy = 1.2f,
            SkyMode = DirectionalLight3D.SkyModeEnum.LightOnly,
        };
        AddChild(Sun);
        Moon = new DirectionalLight3D
        {
            Name = "Moon", ShadowEnabled = true,
            DirectionalShadowMode = DirectionalLight3D.ShadowMode.Parallel2Splits,
            DirectionalShadowMaxDistance = 160f, DirectionalShadowSplit1 = 0.2f,
            ShadowBias = 0.05f, ShadowNormalBias = 1.5f, ShadowBlur = 2.0f,
            LightColor = MoonCol, LightEnergy = 0f, Visible = false, LightVolumetricFogEnergy = 0.6f,
            SkyMode = DirectionalLight3D.SkyModeEnum.LightOnly,
        };
        AddChild(Moon);
        AddChild(new Atmosphere { Name = "Atmosphere" });
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

    /// <summary>Cold-earth grade (applied after tonemapping): cool blue-grey in the shadows, warm grey-brown in the highlights,
    /// a slightly lifted, desaturated film look.</summary>
    static ImageTexture3D GradeLut()
    {
        const int N = 17;
        var imgs = new Godot.Collections.Array<Image>();
        for (int b = 0; b < N; b++)
        {
            var img = Image.CreateEmpty(N, N, false, Image.Format.Rgb8);
            for (int g = 0; g < N; g++)
                for (int r = 0; r < N; r++)
                {
                    var c = new Vector3(r, g, b) / (N - 1);
                    float l = c.Dot(new Vector3(0.2126f, 0.7152f, 0.0722f));
                    c = new Vector3(l, l, l).Lerp(c, 0.9f);
                    float sh = (1 - l) * (1 - l), hi = l * l;
                    c += sh * new Vector3(-0.012f, 0.0f, 0.03f) + hi * new Vector3(0.018f, 0.006f, -0.03f);
                    c = c * 0.965f + new Vector3(0.016f, 0.017f, 0.02f);
                    img.SetPixel(r, g, new Color(Math.Clamp(c.X, 0, 1), Math.Clamp(c.Y, 0, 1), Math.Clamp(c.Z, 0, 1)));
                }
            imgs.Add(img);
        }
        var tex = new ImageTexture3D();
        tex.Create(Image.Format.Rgb8, N, N, N, false, imgs);
        return tex;
    }

    // ---------------------------------------------------------------------------------------------- keyframes

    static readonly float[] Keys = { -18f, -10f, -4f, 0f, 5f, 15f, 35f };
    static readonly uint[] Zenith = { 0x060a14, 0x0e1428, 0x2a3350, 0x45516e, 0x55657f, 0x5a6f8c, 0x5c7393 };
    static readonly uint[] Horizon = { 0x0b1020, 0x161c32, 0x4a4a5e, 0x8f8a90, 0xa8a8aa, 0xa9b4c0, 0xa6b5c4 };
    static readonly uint[] HorizonSun = { 0x0b1020, 0x262438, 0x9c6658, 0xd29a70, 0xd8b896, 0xd2cbbd, 0xc0c8d0 };
    static readonly uint[] SunCol = { 0xff6a30, 0xff6a30, 0xff7c40, 0xff9a5a, 0xffc890, 0xffe9cf, 0xffebce };

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
        Weather.Update();
        float hour = GameClock.Hour;
        Vector3 sunDir = GameClock.SunDirection(hour);
        Vector3 moonDir = GameClock.MoonDirection(hour);
        float e = Mathf.RadToDeg(MathF.Asin(sunDir.Y));
        float em = Mathf.RadToDeg(MathF.Asin(moonDir.Y));
        float cloud = Weather.Cloud, rain = Weather.Rain, fogW = Weather.Fog;
        Vector3 lightDir = LowSun(sunDir);

        float day = FMath.Smoothstep(-8f, 8f, e);
        float night = 1f - FMath.Smoothstep(-12f, -3f, e);
        float dusk = 1f - FMath.Smoothstep(4f, 14f, MathF.Abs(e - 1f));
        float grey = Math.Clamp(cloud * 1.1f - 0.1f, 0f, 1f);

        // --- sun: pale and warm, low; clouds swallow most of it (overcast light is the sky's)
        float sunE = FMath.Smoothstep(-1.5f, 3f, e) * FMath.Lerp(0.8f, 1.65f, FMath.Smoothstep(2f, 22f, e)) * FMath.Lerp(1f, 0.2f, grey) * (1f - 0.6f * rain);
        Sun.LightColor = Key(SunCol, e).Lerp(SunDay, FMath.Smoothstep(6f, 18f, e));
        Sun.LightEnergy = sunE;
        Sun.ShadowOpacity = FMath.Lerp(1f, 0.55f, grey);
        Sun.Visible = sunE > 0.002f;
        if (Sun.Visible) Sun.Basis = Basis.LookingAt(-SafeLightDir(lightDir), Vector3.Up);

        // --- moon: weak and blue, hidden by clouds
        float moonE = 0.11f * FMath.Smoothstep(-2f, -9f, e) * FMath.Smoothstep(2f, 14f, em) * FMath.Lerp(1f, 0.35f, grey);
        Moon.LightEnergy = moonE;
        Moon.Visible = moonE > 0.002f;
        if (Moon.Visible) Moon.Basis = Basis.LookingAt(-SafeLightDir(moonDir), Vector3.Up);

        // --- ambient, fog, exposure
        Color hor = Key(Horizon, e), horSun = Key(HorizonSun, e);
        Color dayHor = SkyGrey.Darkened(0.12f * rain);
        hor = hor.Lerp(dayHor, grey * day).Lerp(NightHor, night);
        horSun = horSun.Lerp(dayHor.Lerp(horSun, 0.25f * dusk), grey * day).Lerp(NightHor, night);
        Env.AmbientLightColor = FMath.Hex(0x1c2438).Lerp(FMath.Hex(0x8a929c), day);
        Env.AmbientLightSkyContribution = FMath.Lerp(0.5f, 0.85f, day);
        Env.AmbientLightEnergy = FMath.Lerp(0.32f, FMath.Lerp(0.75f, 0.95f, grey), day) * (1f - 0.2f * dusk);
        Color fogCol = FogNight.Lerp(FogDay, day).Lerp(horSun, 0.35f * dusk * (1f - grey));
        Env.FogLightColor = fogCol;
        Env.FogLightEnergy = FMath.Lerp(0.16f, 0.74f - 0.2f * rain, day);
        Env.FogSunScatter = (0.05f + 0.2f * dusk) * (1f - grey * 0.7f);
        // haze always; fog thickest at dawn and dusk, in rain and on foggy mornings (valleys and forest: Atmosphere's fog volumes)
        float fogK = 1f + 0.5f * dusk + 2.4f * fogW * fogW + 0.8f * rain;
        Env.FogDensity = 0.0017f * fogK * FMath.Lerp(1.15f, 1f, day);
        Env.VolumetricFogDensity = 0.0012f * (1f + 0.6f * dusk + 6f * fogW * fogW + rain);
        Env.VolumetricFogAlbedo = fogCol;
        Env.TonemapExposure = ExposureScale * FMath.Lerp(0.82f, NightExposure, night) * FMath.Lerp(1f, 1.08f, dusk) * FMath.Lerp(1f, 1.06f, grey * day);
        Env.GlowIntensity = FMath.Lerp(0.4f, 1.0f, night);
        Env.GlowHdrThreshold = FMath.Lerp(1.1f, 0.6f, night);
        Env.AdjustmentSaturation = FMath.Lerp(0.92f - 0.06f * grey, 0.58f, night);

        // window lights: on from sunset to dawn (and a little under heavy cloud)
        RenderingServer.GlobalShaderParameterSet("night_light", Math.Max(1f - FMath.Smoothstep(-3f, 7f, e), 0.12f * rain * day));
        float gust = 0.5f + 0.5f * MathF.Sin((float)_time * 0.23f) * MathF.Sin((float)_time * 0.071f + 1.3f);
        float w = Weather.Wind;
        RenderingServer.GlobalShaderParameterSet("fd_wind", new Vector4(0.83f, 0.55f, (0.35f + 0.55f * w) * (0.75f + 0.45f * gust), gust * w));
        RenderingServer.GlobalShaderParameterSet("fd_wet", Weather.Wet);

        // --- sky material: throttle (every radiance update is expensive)
        _skyTimer += dt;
        if (force || MathF.Abs(hour - _lastSkyHour) > 0.02f || _skyTimer > 2f || MathF.Abs(cloud - _lastCloud) > 0.02f)
        {
            _skyTimer = 0; _lastSkyHour = hour; _lastCloud = cloud;
            Color zen = Key(Zenith, e).Lerp(SkyGreyTop.Darkened(0.18f * rain), grey * day).Lerp(NightTop, night);
            SkyMaterial.SetShaderParameter("zenith_color", zen);
            SkyMaterial.SetShaderParameter("horizon_color", hor);
            SkyMaterial.SetShaderParameter("horizon_sun_color", horSun);
            SkyMaterial.SetShaderParameter("ground_color", hor.Darkened(0.35f));
            SkyMaterial.SetShaderParameter("sun_dir", lightDir);
            SkyMaterial.SetShaderParameter("sun_color", Key(SunCol, e).Lerp(new Color(1, 1, 1), 0.15f));
            SkyMaterial.SetShaderParameter("sun_glow", FMath.Smoothstep(-8f, 0f, e) * FMath.Lerp(1.2f, 0.55f, FMath.Smoothstep(3f, 30f, e)) * (1f - 0.8f * grey));
            SkyMaterial.SetShaderParameter("sun_disk", FMath.Smoothstep(-1.5f, 0.5f, e) * (1f - grey));
            SkyMaterial.SetShaderParameter("moon_dir", moonDir);
            SkyMaterial.SetShaderParameter("moon_amount", FMath.Smoothstep(-2f, -8f, e) * FMath.Smoothstep(-2f, 4f, em) * (1f - 0.85f * grey));
            SkyMaterial.SetShaderParameter("star_amount", FMath.Smoothstep(-5f, -13f, e) * 0.7f * (1f - grey) * (1f - grey));
            SkyMaterial.SetShaderParameter("sky_time", (float)(_time % 10000.0));
            SkyMaterial.SetShaderParameter("cloud_cover", FMath.Lerp(0.38f, 0.97f, cloud));
            SkyMaterial.SetShaderParameter("cloud_lit", Key(HorizonSun, e).Lerp(new Color(0.86f, 0.86f, 0.86f), 0.6f * day).Lerp(SkyGrey, grey * day).Lerp(NightHor.Lightened(0.05f), night));
            SkyMaterial.SetShaderParameter("cloud_dark", Key(Horizon, e).Darkened(FMath.Lerp(0.5f, 0.3f, day)).Lerp(SkyGreyTop.Darkened(0.25f + 0.2f * rain), grey * day).Lerp(NightTop, night));
            SkyMaterial.SetShaderParameter("cloud_alpha", FMath.Lerp(0.6f, FMath.Lerp(0.8f, 0.92f, grey), day));
        }
    }

    /// <summary>The direction the sunlight comes from: the sun's own azimuth, never higher than <see cref="SunMaxElevation"/>.</summary>
    public static Vector3 LowSun(Vector3 sunDir)
    {
        float maxY = MathF.Sin(Mathf.DegToRad(SunMaxElevation));
        if (sunDir.Y <= maxY) return sunDir;
        var flat = new Vector3(sunDir.X, 0, sunDir.Z);
        if (flat.LengthSquared() < 1e-6f) flat = new Vector3(0, 0, 1);
        flat = flat.Normalized() * MathF.Cos(Mathf.DegToRad(SunMaxElevation));
        return new Vector3(flat.X, maxY, flat.Z).Normalized();
    }

    /// <summary>Keep light directions a few degrees above the horizon so shadows don't stretch to infinity.</summary>
    static Vector3 SafeLightDir(Vector3 d)
    {
        if (d.Y >= 0.06f) return d;
        return new Vector3(d.X, 0.06f, d.Z).Normalized();
    }
}
