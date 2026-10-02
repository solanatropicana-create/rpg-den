using System;
using Godot;
using FD.Core;
using FD.World;

namespace FD.Actors;

/// <summary>
/// Tur 1 A: the one camera of the game — no modes. The wheel zooms continuously from over the shoulder (2 m behind the head,
/// looking ahead) up to a high angle over the land (75 m, looking down ~64°); far out it simply is the tactical view. It follows
/// the controlled character (or the lead fighter) unless the player pans it: arrow keys, the screen edge, or WASD when nobody is
/// selected. <b>F</b> follows again. Middle mouse or Alt+drag turns it (and tilts a little). The mouse stays free for clicking.
/// Runs while the game is paused and at the real (not the game's) speed.
/// </summary>
public partial class GameCamera : Camera3D
{
    public static GameCamera Instance { get; private set; }
    public const float MinDist = 2.2f, MaxDist = 75f;
    /// <summary>the ground point (or the followed character's feet) the camera turns around</summary>
    public Vector3 Focus;
    /// <summary>radians; 0 looks north (−z), the same convention as the old third-person rig</summary>
    public float Yaw;
    public float Dist = 5.5f;
    /// <summary>the player's tilt on top of the zoom's own pitch</summary>
    public float PitchBias;
    public bool Follow = true;
    /// <summary>what to follow (the controlled body; the lead fighter in a fight)</summary>
    public Func<Vector3?> Target;
    /// <summary>WASD pans the camera (nobody selected)</summary>
    public Func<bool> WasdPans;
    /// <summary>hit shake (seconds left)</summary>
    public float Shake;
    float _distGoal = 5.5f;
    bool _rotating, _snap = true;
    Heightfield _hf;
    Rid _exclude;
    readonly bool _headless = DisplayServer.GetName() == "headless";

    public override void _Ready()
    {
        Instance = this;
        ProcessMode = ProcessModeEnum.Always;
        ProcessPriority = 500;
        Near = 0.08f; Far = 3200f; Fov = 60f;
        _hf = Region.Current?.Heightfield;
    }

    public override void _ExitTree() { if (Instance == this) Instance = null; }

    public void Exclude(Rid body) => _exclude = body;

    /// <summary>0 at the shoulder … 1 at the highest view</summary>
    public float Zoom01 => MathF.Log(Dist / MinDist) / MathF.Log(MaxDist / MinDist);
    public bool IsFar => Dist > 22f;

    /// <summary>Jump to the target at once (after a teleport, a load, a body swap).</summary>
    public void SnapNow() { _snap = true; }

    public void SetZoom(float d, bool instant = false) { _distGoal = Math.Clamp(d, MinDist, MaxDist); if (instant) Dist = _distGoal; }

    public override void _UnhandledInput(InputEvent e)
    {
        if (!Current) return;
        if (e is InputEventMouseButton mb)
        {
            if (mb.Pressed && mb.ButtonIndex == MouseButton.WheelUp) { _distGoal = MathF.Max(MinDist, _distGoal * 0.87f); GetViewport().SetInputAsHandled(); }
            else if (mb.Pressed && mb.ButtonIndex == MouseButton.WheelDown) { _distGoal = MathF.Min(MaxDist, _distGoal * 1.15f); GetViewport().SetInputAsHandled(); }
            else if (mb.ButtonIndex == MouseButton.Middle) { _rotating = mb.Pressed; GetViewport().SetInputAsHandled(); }
            else if (mb.ButtonIndex == MouseButton.Left && mb.AltPressed) { _rotating = mb.Pressed; GetViewport().SetInputAsHandled(); }
            else if (mb.ButtonIndex == MouseButton.Left && !mb.Pressed) _rotating = false;
        }
        else if (e is InputEventMouseMotion mm && _rotating)
        {
            Yaw -= mm.Relative.X * 0.0065f;
            PitchBias = Math.Clamp(PitchBias - mm.Relative.Y * 0.0035f, Mathf.DegToRad(-28f), Mathf.DegToRad(22f));
            GetViewport().SetInputAsHandled();
        }
        else if (e is InputEventKey k && k.Pressed && !k.Echo && k.PhysicalKeycode == Key.F && !k.CtrlPressed)
        {
            Follow = true;
            GetViewport().SetInputAsHandled();
        }
    }

    public bool Rotating => _rotating;

    public override void _Process(double delta)
    {
        if (!Current) return;
        _hf ??= Region.Current?.Heightfield;
        float dt = (float)delta / MathF.Max(0.05f, (float)Engine.TimeScale);
        dt = MathF.Min(dt, 0.1f);
        // pan: arrows always, WASD when nobody is selected, the screen edge
        Vector2 pan = Vector2.Zero;
        bool wasd = WasdPans?.Invoke() ?? false;
        if (Input.IsPhysicalKeyPressed(Key.Up) || (wasd && Input.IsPhysicalKeyPressed(Key.W))) pan.Y -= 1;
        if (Input.IsPhysicalKeyPressed(Key.Down) || (wasd && Input.IsPhysicalKeyPressed(Key.S))) pan.Y += 1;
        if (Input.IsPhysicalKeyPressed(Key.Left) || (wasd && Input.IsPhysicalKeyPressed(Key.A))) pan.X -= 1;
        if (Input.IsPhysicalKeyPressed(Key.Right) || (wasd && Input.IsPhysicalKeyPressed(Key.D))) pan.X += 1;
        if (FD.Game.Settings.Current.EdgeScroll && !_rotating && !_headless && DisplayServer.WindowIsFocused() && FD.UI.PanelLayer.OpenPanel == null)
        {
            var vp = GetViewport().GetVisibleRect().Size;
            var m = GetViewport().GetMousePosition();
            const float edge = 3f;
            if (m.X >= 0 && m.Y >= 0 && m.X <= vp.X && m.Y <= vp.Y)
            {
                if (m.X < edge) pan.X -= 1; else if (m.X > vp.X - edge) pan.X += 1;
                if (m.Y < edge) pan.Y -= 1; else if (m.Y > vp.Y - edge) pan.Y += 1;
            }
        }
        if (pan != Vector2.Zero)
        {
            pan = pan.Normalized() * MathF.Max(8f, Dist * 1.1f) * dt;
            var fwd = new Vector3(-MathF.Sin(Yaw), 0, -MathF.Cos(Yaw));
            var right = new Vector3(MathF.Cos(Yaw), 0, -MathF.Sin(Yaw));
            Focus += right * pan.X - fwd * pan.Y;
            Follow = false;
        }
        Dist = Mathf.Lerp(Dist, _distGoal, 1f - MathF.Exp(-dt * 10f));
        var target = Follow ? Target?.Invoke() : null;
        if (target is Vector3 t)
        {
            if (_snap) Focus = t;
            else
            {
                float k = 1f - MathF.Exp(-dt * FMath.Lerp(16f, 6f, Zoom01));
                Focus = Focus.Lerp(t, k);
            }
        }
        else if (_hf != null) Focus.Y = Mathf.Lerp(Focus.Y, _hf.Height(Focus.X, Focus.Z), 1f - MathF.Exp(-dt * 6f));
        _snap = false;
        Apply();
    }

    void Apply()
    {
        float z = Zoom01;
        float curve = MathF.Pow(z, 0.8f);
        float pitch = Math.Clamp(Mathf.DegToRad(FMath.Lerp(-9f, -64f, curve)) + PitchBias, Mathf.DegToRad(-86f), Mathf.DegToRad(6f));
        Fov = FMath.Lerp(62f, 50f, z);
        var right = new Vector3(MathF.Cos(Yaw), 0, -MathF.Sin(Yaw));
        // close: over the right shoulder at head height; far: the ground
        float near = 1f - FMath.Smoothstep(0f, 0.35f, z);
        var look = Focus + Vector3.Up * FMath.Lerp(0.7f, 1.55f, near) + right * 0.42f * near;
        var dir = new Vector3(MathF.Sin(Yaw) * MathF.Cos(pitch), -MathF.Sin(pitch), MathF.Cos(Yaw) * MathF.Cos(pitch));
        var pos = look + dir * Dist;
        // keep out of walls and hills when close
        if (Dist < 14f && IsInsideTree())
        {
            var space = GetWorld3D().DirectSpaceState;
            var q = PhysicsRayQueryParameters3D.Create(look, pos, App.LayerTerrain | App.LayerProps);
            if (_exclude.IsValid) q.Exclude = new Godot.Collections.Array<Rid> { _exclude };
            var hit = space.IntersectRay(q);
            if (hit.Count > 0)
            {
                var hp = (Vector3)hit["position"];
                pos = look + (hp - look) * 0.92f;
            }
        }
        if (_hf != null) pos.Y = MathF.Max(pos.Y, _hf.Height(pos.X, pos.Z) + 0.45f);
        var shake = Vector3.Zero;
        if (Shake > 0) { Shake -= (float)GetProcessDeltaTime(); shake = new Vector3(GD.Randf() - 0.5f, GD.Randf() - 0.5f, 0) * 0.1f; }
        GlobalTransform = new Transform3D(Basis.LookingAt(look - pos, Vector3.Up), pos + shake);
    }

    /// <summary>Ground point under the screen position (ray marched over the heightfield), or null.</summary>
    public Vector3? Ground(Vector2 screen)
    {
        if (_hf == null) return null;
        var o = ProjectRayOrigin(screen);
        var d = ProjectRayNormal(screen);
        float t = 0, step = 0.4f;
        for (int i = 0; i < 700; i++)
        {
            var p = o + d * t;
            float h = _hf.Height(p.X, p.Z);
            if (p.Y <= h)
            {
                float a = t - step, b = t;
                for (int k = 0; k < 10; k++) { float m = (a + b) * 0.5f; var q = o + d * m; if (q.Y <= _hf.Height(q.X, q.Z)) b = m; else a = m; }
                var hit = o + d * b;
                return new Vector3(hit.X, _hf.Height(hit.X, hit.Z), hit.Z);
            }
            t += step;
            if (t > 30) step = 1.0f;
            if (t > 150) step = 2.5f;
        }
        return null;
    }

    /// <summary>screen position of a world point (null when behind the camera)</summary>
    public Vector2? Screen(Vector3 w) => IsPositionBehind(w) ? null : UnprojectPosition(w);
}
