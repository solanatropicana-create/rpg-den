using System;
using Godot;
using FD.Core;
using FD.World;

namespace FD.Actors;

/// <summary>
/// Third-person hero. CharacterBody3D with a 0.35 × 1.8 m capsule; camera-relative WASD movement.
/// <list type="bullet">
/// <item>Default jog 4.0 m/s · hold Shift to sprint 11 m/s (fast exploring) · hold Ctrl (or Alt) to walk 1.6 m/s.</item>
/// <item>Space jumps (~0.9 m), slopes up to 46° are walkable, floor snapping keeps the feet on descents.</item>
/// <item>Mouse orbits the camera (captured; Esc releases, click recaptures), wheel zooms 2–12 m,
/// the spring arm keeps the camera out of terrain and buildings.</item>
/// </list>
/// The camera rig is top-level and follows the physics-interpolated body every frame.
/// </summary>
public partial class Player : CharacterBody3D
{
    [Export] public float WalkSpeed = 1.6f;
    [Export] public float JogSpeed = 4.0f;
    [Export] public float SprintSpeed = 11f;
    [Export] public float Acceleration = 16f;
    [Export] public float Deceleration = 20f;
    [Export] public float AirControl = 3f;
    [Export] public float Gravity = 17f;
    [Export] public float JumpVelocity = 5.6f;
    [Export] public float TurnRate = 11f;
    [Export] public float MouseSensitivity = 0.0024f;
    [Export] public float MinZoom = 2f, MaxZoom = 12f;
    [Export] public float Zoom = 4.4f;
    [Export] public float EyeHeight = 1.55f;

    public static Player Current { get; private set; }
    public Camera3D Camera { get; private set; }
    public SpringArm3D Arm { get; private set; }
    public Node3D CameraRig { get; private set; }
    public Humanoid Body { get; private set; }
    public float CameraYaw => _yaw;
    /// <summary>When false the player ignores input (dev free camera, UI).</summary>
    public bool InputEnabled = true;

    Node3D _pitchNode;
    float _yaw, _pitch = -0.24f, _zoomTarget, _facing, _rigY;
    bool _rigInit;

    public override void _Ready()
    {
        Current = this;
        GameInput.Ensure();
        FloorMaxAngle = Mathf.DegToRad(46f);
        FloorSnapLength = 0.45f;
        FloorConstantSpeed = true;
        FloorBlockOnWall = true;
        SafeMargin = 0.02f;
        CollisionLayer = App.LayerPlayer;
        CollisionMask = App.LayerTerrain | App.LayerProps | App.LayerActors;

        CameraRig = GetNode<Node3D>("CameraRig");
        CameraRig.TopLevel = true;
        CameraRig.PhysicsInterpolationMode = PhysicsInterpolationModeEnum.Off;
        _pitchNode = GetNode<Node3D>("CameraRig/Pitch");
        Arm = GetNode<SpringArm3D>("CameraRig/Pitch/SpringArm");
        Camera = GetNode<Camera3D>("CameraRig/Pitch/SpringArm/Camera");
        Arm.CollisionMask = App.LayerTerrain | App.LayerProps;
        Arm.AddExcludedObject(GetRid());
        _zoomTarget = Zoom;
        Arm.SpringLength = Zoom;

        Body = new Humanoid { Name = "Body" };
        GetNode<Node3D>("Visual").AddChild(Body);
        Input.MouseMode = Input.MouseModeEnum.Captured;
        SnapCamera();
    }

    public override void _ExitTree() { if (Current == this) Current = null; }

    /// <summary>Place the player on the ground at (x, z), facing yaw (radians, model-front convention).</summary>
    public void Teleport(Vector2 xz, float facingYaw, Heightfield hf)
    {
        float y = hf != null ? hf.Height(xz.X, xz.Y) + 0.05f : GlobalPosition.Y;
        GlobalPosition = new Vector3(xz.X, y, xz.Y);
        Velocity = Vector3.Zero;
        _facing = facingYaw;
        GetNode<Node3D>("Visual").Rotation = new Vector3(0, _facing, 0);
        // camera behind the character: camera forward = model front
        _yaw = facingYaw + MathF.PI;
        ResetPhysicsInterpolation();
        SnapCamera();
    }

    public void SetCameraAngles(float yaw, float pitch, float zoom)
    {
        _yaw = yaw; _pitch = Math.Clamp(pitch, Mathf.DegToRad(-75f), Mathf.DegToRad(35f));
        _zoomTarget = Zoom = Math.Clamp(zoom, MinZoom, MaxZoom);
        Arm.SpringLength = Zoom;
        SnapCamera();
    }

    void SnapCamera()
    {
        if (CameraRig == null) return;
        _rigInit = false;
        UpdateRig(0f);
    }

    public override void _UnhandledInput(InputEvent e)
    {
        if (!InputEnabled) return;
        if (e is InputEventMouseMotion mm && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            _yaw -= mm.Relative.X * MouseSensitivity;
            _pitch = Math.Clamp(_pitch - mm.Relative.Y * MouseSensitivity, Mathf.DegToRad(-75f), Mathf.DegToRad(35f));
        }
        else if (e is InputEventMouseButton mb && mb.Pressed)
        {
            if (mb.ButtonIndex == MouseButton.WheelUp) _zoomTarget = Math.Max(MinZoom, _zoomTarget * 0.88f);
            else if (mb.ButtonIndex == MouseButton.WheelDown) _zoomTarget = Math.Min(MaxZoom, _zoomTarget * 1.13f);
            else if (mb.ButtonIndex == MouseButton.Left && Input.MouseMode != Input.MouseModeEnum.Captured)
                Input.MouseMode = Input.MouseModeEnum.Captured;
        }
        else if (e.IsActionPressed("release_mouse"))
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;
        Vector2 inp = InputEnabled ? Input.GetVector("move_left", "move_right", "move_forward", "move_back") : Vector2.Zero;
        Vector3 fwd = new(-MathF.Sin(_yaw), 0, -MathF.Cos(_yaw));
        Vector3 right = new(MathF.Cos(_yaw), 0, -MathF.Sin(_yaw));
        Vector3 wish = right * inp.X - fwd * inp.Y;
        if (wish.LengthSquared() > 1f) wish = wish.Normalized();

        bool sprint = InputEnabled && Input.IsActionPressed("sprint");
        bool walk = InputEnabled && Input.IsActionPressed("walk");
        float speed = sprint ? SprintSpeed : walk ? WalkSpeed : JogSpeed;
        Vector3 target = wish * speed;

        Vector3 v = Velocity;
        Vector3 hv = new(v.X, 0, v.Z);
        bool floor = IsOnFloor();
        float a = floor ? (target.LengthSquared() >= hv.LengthSquared() ? Acceleration : Deceleration) : AirControl;
        hv = hv.MoveToward(target, a * dt);
        float vy = v.Y;
        if (floor)
        {
            if (InputEnabled && Input.IsActionJustPressed("jump")) vy = JumpVelocity;
            else vy = MathF.Min(vy, 0f);
        }
        else vy -= Gravity * dt;
        Velocity = new Vector3(hv.X, vy, hv.Z);
        MoveAndSlide();

        // keep inside the region
        var p = GlobalPosition;
        float lim = RegionSpec.WalkLimit;
        if (MathF.Abs(p.X) > lim || MathF.Abs(p.Z) > lim)
            GlobalPosition = new Vector3(Math.Clamp(p.X, -lim, lim), p.Y, Math.Clamp(p.Z, -lim, lim));

        // turn the body toward the movement direction
        Vector3 flat = new(Velocity.X, 0, Velocity.Z);
        if (flat.LengthSquared() > 0.04f)
        {
            float want = MathF.Atan2(flat.X, flat.Z);
            _facing = Mathf.LerpAngle(_facing, want, 1f - MathF.Exp(-TurnRate * dt));
            GetNode<Node3D>("Visual").Rotation = new Vector3(0, _facing, 0);
        }
    }

    public override void _Process(double delta)
    {
        float dt = (float)delta;
        Zoom = Mathf.Lerp(Zoom, _zoomTarget, 1f - MathF.Exp(-dt * 12f));
        Arm.SpringLength = Zoom;
        UpdateRig(dt);
        var hv = new Vector2(Velocity.X, Velocity.Z).Length();
        Body?.UpdateMotion(IsOnFloor() ? hv : hv, IsOnFloor(), Velocity.Y, dt);
    }

    void UpdateRig(float dt)
    {
        Vector3 pos = IsInsideTree() ? GetGlobalTransformInterpolated().Origin : Position;
        // smooth only vertical motion (steps, landing) so the view never jitters
        float targetY = pos.Y + EyeHeight;
        _rigY = _rigInit && dt > 0 ? Mathf.Lerp(_rigY, targetY, 1f - MathF.Exp(-dt * 18f)) : targetY;
        _rigInit = true;
        CameraRig.GlobalPosition = new Vector3(pos.X, _rigY, pos.Z);
        CameraRig.Rotation = new Vector3(0, _yaw, 0);
        _pitchNode.Rotation = new Vector3(_pitch, 0, 0);
    }
}
