using System;
using Godot;
using FD.Core;
using FD.World;

namespace FD.Actors;

/// <summary>
/// The body of the character the player controls. CharacterBody3D with a 0.35 × 1.8 m capsule.
/// <list type="bullet">
/// <item>Tur 1 A: WASD walks it relative to the one camera (<see cref="GameCamera"/>) when it is selected; a right click sends it
/// somewhere (<see cref="GoTo"/>: along the village lanes and roads when a house is in the way, then the last stretch straight).</item>
/// <item>Default jog 4.0 m/s · hold Shift to sprint 11 m/s (fast exploring) · hold Ctrl to walk 1.6 m/s.</item>
/// <item>Slopes up to 46° are walkable, floor snapping keeps the feet on descents (no jumping: Space pauses the game).</item>
/// </list>
/// The old camera rig of the scene is kept but never used.
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
    /// <summary>body facing (yaw, radians, model front)</summary>
    public float Facing => _facing;
    /// <summary>Faz 2: the character this body belongs to (look, speed); null before the session gives one</summary>
    public FD.Rpg.Character Character { get; private set; }

    /// <summary>Faz 2: dress the body as the character (race build and ears, colours, hair, weapon in hand).</summary>
    public void SetCharacter(FD.Rpg.Character c)
    {
        Character = c;
        var visual = GetNode<Node3D>("Visual");
        Body?.QueueFree();
        Body = LookKit.Body(c);
        visual.AddChild(Body);
    }
    /// <summary>When false the player ignores input (dev free camera, UI).</summary>
    public bool InputEnabled = true;
    /// <summary>Tur 1 A: WASD walks this body (it is selected); otherwise the keys pan the camera</summary>
    public static Func<bool> WasdDrives;
    /// <summary>Faz 2: the fight moves the body (no physics, no input); <see cref="SetPose"/> each frame</summary>
    public bool Scripted;
    string _scriptAnim = "Idle";
    float _scriptSpeed;

    /// <summary>Faz 2: place and animate the body from the fight (scripted mode).</summary>
    public void SetPose(Vector3 pos, float yaw, string anim, float speed)
    {
        GlobalPosition = pos;
        Velocity = Vector3.Zero;
        _facing = Mathf.LerpAngle(_facing, yaw, 0.35f);
        GetNode<Node3D>("Visual").Rotation = new Vector3(0, _facing, 0);
        _scriptAnim = anim; _scriptSpeed = speed;
    }

    Node3D _pitchNode;
    string _lastScriptAnim;
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
        Camera.Current = false;

        Body = new Humanoid { Name = "Body" };
        GetNode<Node3D>("Visual").AddChild(Body);
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

    // ------------------------------------------------------------------------------------------------ going somewhere (Tur 1 A/B)
    readonly System.Collections.Generic.List<Vector2> _route = new();
    Func<Vector2> _goal;
    float _reach;
    Action _arrive;
    float _stuckT, _sideT;
    int _stuck;
    Vector2 _lastPos;
    Vector2 _side;
    /// <summary>walking to a right-clicked place or thing</summary>
    public bool Going => _goal != null;
    /// <summary>where it is walking to (the end of the route)</summary>
    public Vector2? GoingTo => _goal?.Invoke();

    /// <summary>Walk to a point (or a moving thing: <paramref name="goal"/> is asked every frame), within <paramref name="reach"/>
    /// metres, then <paramref name="arrive"/>. A house in the way: along the lanes (the people's path graph), the last stretch straight.</summary>
    public void GoTo(Func<Vector2> goal, float reach = 0.5f, Action arrive = null)
    {
        _goal = goal; _reach = MathF.Max(0.3f, reach); _arrive = arrive;
        _stuck = 0; _stuckT = 0; _sideT = 0;
        _lastPos = new Vector2(GlobalPosition.X, GlobalPosition.Z);
        PlanRoute();
    }

    public void GoTo(Vector2 p, float reach = 0.5f, Action arrive = null) => GoTo(() => p, reach, arrive);

    public void CancelGoTo() { _goal = null; _arrive = null; _route.Clear(); }

    void PlanRoute()
    {
        _route.Clear();
        var life = FD.Life.LifeWorld.Instance?.Sim;
        var here = new Vector2(GlobalPosition.X, GlobalPosition.Z);
        var goal = _goal();
        if (life == null || !Blocked(life, here, goal)) return;
        var g = life.Graph;
        int a = g.Nearest(new System.Numerics.Vector2(here.X, here.Y), 60f), b = g.Nearest(new System.Numerics.Vector2(goal.X, goal.Y), 60f);
        if (a < 0 || b < 0) return;
        var path = g.FindPath(a, b);
        if (path == null) return;
        foreach (var n in path) _route.Add(new Vector2(g.Nodes[n].X, g.Nodes[n].Y));
        // skip the first nodes we can already pass
        while (_route.Count > 1 && !Blocked(life, here, _route[1])) _route.RemoveAt(0);
    }

    static bool Blocked(FD.Sim.Life.LifeSim life, Vector2 a, Vector2 b)
    {
        float L = a.DistanceTo(b);
        for (float t = 1f; t < L; t += 1f)
        {
            var p = a.Lerp(b, t / L);
            if (life.ObstacleAt(new System.Numerics.Vector2(p.X, p.Y), -0.4f) != null) return true;
        }
        return false;
    }

    Vector3 AutoWish(float dt)
    {
        if (_goal == null) return Vector3.Zero;
        var here = new Vector2(GlobalPosition.X, GlobalPosition.Z);
        var goal = _goal();
        if (here.DistanceTo(goal) <= _reach)
        {
            var act = _arrive;
            CancelGoTo();
            act?.Invoke();
            return Vector3.Zero;
        }
        while (_route.Count > 0 && here.DistanceTo(_route[0]) < 1.6f) _route.RemoveAt(0);
        var next = _route.Count > 0 ? _route[0] : goal;
        // stuck (a fence, a tree): step aside for a moment; give up after a few tries
        _stuckT += dt;
        if (_stuckT > 0.8f)
        {
            if (here.DistanceTo(_lastPos) < 0.35f)
            {
                _stuck++;
                if (_stuck > 5)
                {
                    // close enough to talk or take across a fence: do it; otherwise give up
                    var act = here.DistanceTo(goal) <= _reach + 2.5f ? _arrive : null;
                    CancelGoTo();
                    if (act != null) act(); else FD.World.Region.Current?.Hud?.Toast("Oraya bir yol bulamadım.", 2.5f);
                    return Vector3.Zero;
                }
                var d0 = (next - here).Normalized();
                _side = (_stuck % 2 == 0 ? new Vector2(-d0.Y, d0.X) : new Vector2(d0.Y, -d0.X));
                _sideT = 0.7f;
                if (_stuck == 3) PlanRoute();
            }
            _stuckT = 0; _lastPos = here;
        }
        var d = (next - here).Normalized();
        if (_sideT > 0) { _sideT -= dt; d = (d * 0.3f + _side).Normalized(); }
        return new Vector3(d.X, 0, d.Y);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Scripted) return;
        float dt = (float)delta;
        bool drive = InputEnabled && (WasdDrives?.Invoke() ?? true);
        Vector2 inp = drive ? Input.GetVector("move_left", "move_right", "move_forward", "move_back") : Vector2.Zero;
        float yaw = GameCamera.Instance?.Yaw ?? _yaw;
        Vector3 fwd = new(-MathF.Sin(yaw), 0, -MathF.Cos(yaw));
        Vector3 right = new(MathF.Cos(yaw), 0, -MathF.Sin(yaw));
        Vector3 wish = right * inp.X - fwd * inp.Y;
        if (wish.LengthSquared() > 1f) wish = wish.Normalized();
        if (wish.LengthSquared() > 0.01f) CancelGoTo();
        else if (_goal != null && InputEnabled) wish = AutoWish(dt);

        bool sprint = drive && Input.IsActionPressed("sprint");
        bool walk = drive && Input.IsActionPressed("walk");
        float speed = (sprint ? SprintSpeed : walk ? WalkSpeed : JogSpeed) * (Character?.SpeedFactor ?? 1f);
        Vector3 target = wish * speed;

        Vector3 v = Velocity;
        Vector3 hv = new(v.X, 0, v.Z);
        bool floor = IsOnFloor();
        float a = floor ? (target.LengthSquared() >= hv.LengthSquared() ? Acceleration : Deceleration) : AirControl;
        hv = hv.MoveToward(target, a * dt);
        float vy = v.Y;
        if (floor) vy = MathF.Min(vy, 0f);
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
        if (Scripted)
        {
            if (_scriptAnim == "Attack" && _lastScriptAnim == "Attack" && Body != null && !Body.IsPlaying()) Body.Restart();
            if (!(_scriptAnim == "Die" && _lastScriptAnim == "Die")) Body?.Drive(_scriptAnim, _scriptSpeed, 0.18f);
            _lastScriptAnim = _scriptAnim;
            return;
        }
        _lastScriptAnim = null;
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
