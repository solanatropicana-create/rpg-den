using System;
using Godot;
using FD.World;

namespace FD.Combat;

/// <summary>
/// Faz 2 C: the fight's camera — high and angled (≈58° down), following the middle of the fight unless the player pans it.
/// WASD / arrows pan, wheel zooms (8–42 m), right mouse drag (or Q/E with Shift) turns it. The mouse is free for picking.
/// </summary>
public partial class TacticalCamera : Camera3D
{
    public Vector3 Focus;
    public float Yaw, Pitch = Mathf.DegToRad(-58f), Dist = 19f;
    Vector3 _goal;
    bool _follow = true;
    float _panHold;
    Heightfield _hf;

    public void Begin(Heightfield hf, Vector3 focus, float yaw)
    {
        _hf = hf;
        Focus = _goal = focus;
        Yaw = yaw;
        _follow = true;
        Fov = 52f; Near = 0.2f; Far = 2600f;
        Apply(1f);
    }

    /// <summary>where the fight is; the camera drifts there unless the player panned in the last few seconds</summary>
    public void Track(Vector3 p) { if (_follow) _goal = p; }

    public override void _UnhandledInput(InputEvent e)
    {
        if (!Current) return;
        if (e is InputEventMouseButton mb && mb.Pressed)
        {
            if (mb.ButtonIndex == MouseButton.WheelUp) Dist = MathF.Max(8f, Dist * 0.9f);
            else if (mb.ButtonIndex == MouseButton.WheelDown) Dist = MathF.Min(42f, Dist * 1.1f);
        }
        else if (e is InputEventMouseMotion mm && (mm.ButtonMask & MouseButtonMask.Right) != 0)
        {
            Yaw -= mm.Relative.X * 0.006f;
            Pitch = Math.Clamp(Pitch - mm.Relative.Y * 0.004f, Mathf.DegToRad(-80f), Mathf.DegToRad(-30f));
        }
    }

    public override void _Process(double delta)
    {
        if (!Current) return;
        float dt = (float)delta;
        Vector2 pan = Vector2.Zero;
        if (Input.IsKeyPressed(Key.W) || Input.IsKeyPressed(Key.Up)) pan.Y -= 1;
        if (Input.IsKeyPressed(Key.S) || Input.IsKeyPressed(Key.Down)) pan.Y += 1;
        if (Input.IsKeyPressed(Key.A) || Input.IsKeyPressed(Key.Left)) pan.X -= 1;
        if (Input.IsKeyPressed(Key.D) || Input.IsKeyPressed(Key.Right)) pan.X += 1;
        if (pan != Vector2.Zero)
        {
            pan = pan.Normalized() * Dist * 0.9f * dt;
            var fwd = new Vector3(-MathF.Sin(Yaw), 0, -MathF.Cos(Yaw));
            var right = new Vector3(MathF.Cos(Yaw), 0, -MathF.Sin(Yaw));
            _goal += right * pan.X - fwd * pan.Y;
            _follow = false; _panHold = 5f;
        }
        else if (!_follow) { _panHold -= dt; if (_panHold <= 0) _follow = true; }
        Apply(1f - MathF.Exp(-dt * 4f));
    }

    void Apply(float k)
    {
        Focus = Focus.Lerp(_goal, k);
        if (_hf != null) Focus.Y = Mathf.Lerp(Focus.Y, _hf.Height(Focus.X, Focus.Z), k);
        var dir = new Vector3(MathF.Sin(Yaw) * MathF.Cos(Pitch), -MathF.Sin(Pitch), MathF.Cos(Yaw) * MathF.Cos(Pitch));
        var pos = Focus + dir * Dist;
        if (_hf != null) pos.Y = MathF.Max(pos.Y, _hf.Height(pos.X, pos.Z) + 2f);
        GlobalTransform = new Transform3D(Basis.LookingAt(Focus + Vector3.Up * 0.8f - pos, Vector3.Up), pos);
    }

    /// <summary>Ground point under the screen position (ray marched over the heightfield), or null.</summary>
    public Vector3? Ground(Vector2 screen)
    {
        if (_hf == null) return null;
        var o = ProjectRayOrigin(screen);
        var d = ProjectRayNormal(screen);
        float t = 0, step = 0.5f;
        for (int i = 0; i < 400; i++)
        {
            var p = o + d * t;
            float h = _hf.Height(p.X, p.Z);
            if (p.Y <= h)
            {
                // refine
                float a = t - step, b = t;
                for (int k = 0; k < 8; k++) { float m = (a + b) * 0.5f; var q = o + d * m; if (q.Y <= _hf.Height(q.X, q.Z)) b = m; else a = m; }
                var hit = o + d * b;
                return new Vector3(hit.X, _hf.Height(hit.X, hit.Z), hit.Z);
            }
            t += step;
            if (t > 30) step = 1.5f;
        }
        return null;
    }
}
