using System;
using Godot;
using FD.Rpg;
using FD.World;

namespace FD.Actors;

/// <summary>
/// Faz 2: the body of a party member who is not the player (a hired fighter or rogue, an NPC hero travelling along). Outside fights
/// it keeps its slot in a loose file behind the leader (walks, jogs or runs to keep up; catches up at once if left far behind);
/// in a fight <see cref="Scripted"/> is set and the fight places it every frame (<see cref="SetPose"/>).
/// </summary>
public partial class Companion : Node3D
{
    public Character Char { get; private set; }
    public Humanoid Body { get; private set; }
    /// <summary>place in the file behind the leader (0 = right behind)</summary>
    public int Slot;
    bool _scripted;
    /// <summary>the fight drives the body (no following, no name over the head)</summary>
    public bool Scripted { get => _scripted; set { _scripted = value; if (_name != null) _name.Visible = !value; } }
    Node3D _leader;
    Heightfield _hf;
    float _yaw, _speed, _y;
    bool _yInit;
    string _scriptAnim = "Idle", _lastAnim;
    float _scriptSpeed;
    Label3D _name;

    public static Companion Create(Character c, int slot)
    {
        var n = new Companion { Name = $"Companion_{c.Id}", Slot = slot };
        n.Char = c;
        return n;
    }

    public override void _Ready()
    {
        Body = LookKit.Body(Char);
        AddChild(Body);
        _name = new Label3D
        {
            Text = Char.FullName, Billboard = BaseMaterial3D.BillboardModeEnum.Enabled, FontSize = 30, PixelSize = 0.004f, OutlineSize = 8,
            Modulate = new Color(0.75f, 0.95f, 0.7f), OutlineModulate = new Color(0.05f, 0.05f, 0.04f, 0.85f), NoDepthTest = true,
            Position = new Vector3(0, 2.15f * LookKit.HeightOf(Char), 0), VisibilityRangeEnd = 18f,
        };
        AddChild(_name);
    }

    public void Init(Node3D leader, Heightfield hf) { _leader = leader; _hf = hf; }

    /// <summary>D: this body now shows another person (a fallen leader's corpse after the leadership passed on).</summary>
    public void Become(Character c)
    {
        Char = c;
        Body?.QueueFree();
        Body = LookKit.Body(c);
        AddChild(Body);
        _name.Text = c.FullName;
        _name.Position = new Vector3(0, 2.15f * LookKit.HeightOf(c), 0);
        _lastAnim = null;
    }

    /// <summary>D: wait where they stand (the leader is caged, or told to stay)</summary>
    public bool Hold;
    /// <summary>Tur 1 A: the player told them to wait here (the party bar's "bekle"); a group order including them ends it</summary>
    public bool Waiting;

    /// <summary>Put the companion on the ground at its slot behind the leader (after a teleport or a load).</summary>
    public void SnapToLeader()
    {
        if (_leader == null) return;
        var t = SlotPoint();
        GlobalPosition = new Vector3(t.X, _hf?.Height(t.X, t.Y) ?? _leader.GlobalPosition.Y, t.Y);
        _yInit = false;
    }

    /// <summary>fight: place and animate (scripted mode)</summary>
    public void SetPose(Vector3 pos, float yaw, string anim, float speed)
    {
        GlobalPosition = pos;
        _yaw = Mathf.LerpAngle(_yaw, yaw, 0.35f);
        Rotation = new Vector3(0, _yaw, 0);
        _scriptAnim = anim; _scriptSpeed = speed;
        _y = pos.Y; _yInit = true;
    }

    float LeaderFacing => _leader is Player p ? p.Facing : _leader.Rotation.Y;

    Vector2 SlotPoint()
    {
        var lp = _leader.GlobalPosition;
        float f = LeaderFacing;
        var back = new Vector2(-MathF.Sin(f), -MathF.Cos(f));
        var side = new Vector2(back.Y, -back.X);
        int row = Slot / 2 + 1;
        float s = Slot % 2 == 0 ? 0.9f : -0.9f;
        return new Vector2(lp.X, lp.Z) + back * (1.6f * row) + side * s;
    }

    public override void _Process(double delta)
    {
        float dt = (float)delta;
        if (Char == null || Body == null) return;
        if (Scripted)
        {
            if (_scriptAnim == "Attack" && _lastAnim == "Attack" && !Body.IsPlaying()) Body.Restart();
            if (!(_scriptAnim == "Die" && _lastAnim == "Die")) Body.Drive(_scriptAnim, _scriptSpeed, 0.18f);
            _lastAnim = _scriptAnim;
            return;
        }
        if (Char.Down || Char.Dead)
        {
            if (_lastAnim != "Die") Body.Drive("Die", 0, 0.15f);
            _lastAnim = "Die";
            return;
        }
        if (Char.Captive)
        {
            // in the goblins' cage: sits behind the bars
            var cp = FD.World.CampSite.CagePrisoner + new Vector2(0.15f * (Slot % 2 == 0 ? 1 : -1), 0.1f * Slot);
            GlobalPosition = new Vector3(cp.X, _hf?.Height(cp.X, cp.Y) ?? GlobalPosition.Y, cp.Y);
            Rotation = new Vector3(0, FD.World.CampSite.CageYaw, 0);
            if (_lastAnim != "Sit") Body.Drive(Body.HasClip("Sit") ? "Sit" : "Idle", 0, 0.3f);
            _lastAnim = "Sit";
            return;
        }
        if (_leader == null) return;
        if (Hold || Waiting)
        {
            _speed = 0;
            if (_lastAnim != "Idle") { Body.Drive("Idle", 0, 0.3f); _lastAnim = "Idle"; }
            return;
        }
        var goal = SlotPoint();
        var here = new Vector2(GlobalPosition.X, GlobalPosition.Z);
        var d = goal - here;
        float L = d.Length();
        float leaderSpeed = _leader is Player pl ? new Vector2(pl.Velocity.X, pl.Velocity.Z).Length() : 0f;
        _lastAnim = null;
        if (L > 40f) { SnapToLeader(); return; }
        // speed to keep up: match the leader near the slot, hurry when behind
        float want = L < 0.35f ? 0f
            : L > 10f ? MathF.Max(leaderSpeed * 1.15f, 6.5f)
            : L > 4f ? MathF.Max(leaderSpeed * 1.2f, 4.2f)
            : MathF.Max(leaderSpeed, 1.4f) * (L > 1.5f ? 1.1f : 0.8f);
        want = MathF.Min(want, 11f * Char.SpeedFactor + 0.5f);
        _speed = Mathf.Lerp(_speed, want, 1f - MathF.Exp(-dt * 6f));
        if (_speed > 0.05f && L > 0.05f)
        {
            here += d / L * MathF.Min(L, _speed * dt);
            float yawWant = MathF.Atan2(d.X, d.Y);
            _yaw = Mathf.LerpAngle(_yaw, yawWant, 1f - MathF.Exp(-dt * 9f));
        }
        else if (leaderSpeed < 0.1f) _yaw = Mathf.LerpAngle(_yaw, LeaderFacing, 1f - MathF.Exp(-dt * 2f));
        float gy = _hf?.Height(here.X, here.Y) ?? GlobalPosition.Y;
        _y = _yInit ? Mathf.Lerp(_y, gy, 1f - MathF.Exp(-dt * 14f)) : gy;
        _yInit = true;
        GlobalPosition = new Vector3(here.X, _y, here.Y);
        Rotation = new Vector3(0, _yaw, 0);
        _lastAnim = null;
        Body.UpdateMotion(_speed, true, 0, dt);
    }
}
