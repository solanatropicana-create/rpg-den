using System;
using System.Collections.Generic;
using Godot;
using FD.Core;

namespace FD.Actors;

/// <summary>
/// Visual body of a person (player now, NPCs later). Uses assets/models/human.glb when present — driving its
/// AnimationPlayer (Idle, Walk, Run, Jump) by ground speed with crossfades and speed-matched playback
/// (Walk authored for 1.4 m/s, Run for 4.5 m/s) — otherwise a procedural low-poly placeholder figure with a
/// simple stride animation. Colours use the character slot convention (fd_char.gdshader). Faces +Z.
/// </summary>
public partial class Humanoid : Node3D
{
    /// <summary>Which character glb to use: "human" (default) or "goblin" (same bones and clip names).</summary>
    public string ModelName = "human";
    /// <summary>Speeds the locomotion clips were authored for (human 1.4 / 4.5, goblin 1.1 / 3.8 m/s).</summary>
    public float WalkAnimSpeed = 1.4f, RunAnimSpeed = 4.5f;
    public const float Height = 1.8f;

    static readonly string[] LoopClips = { "Idle", "Walk", "Run", "Carry", "Chop", "Hoe", "Hammer", "Sit", "Talk", "Gather", "Pray" };
    readonly Dictionary<string, string> _clips = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Slot colours (sRGB); applied as linear instance uniforms.</summary>
    public Color Skin = FMath.Hex(0xd9a27c), Cloth1 = FMath.Hex(0x8c2f22), Cloth2 = FMath.Hex(0x4c3b2b), Hair = FMath.Hex(0x3a2314);

    public bool IsPlaceholder { get; private set; }

    /// <summary>Accessory/tool meshes of human.glb to show (everything else except "body" is hidden).
    /// Accessories: hair_short, hair_long, hair_bun, beard, hat_straw, hood, helmet, apron;
    /// tools: tool_axe, tool_hoe, tool_hammer, tool_pitchfork, tool_spear, tool_lantern, tool_sack, tool_bucket.</summary>
    public string[] Outfit = { "hair_short", "beard" };

    AnimationPlayer _anim;
    string _idle, _walk, _run, _jump, _current;
    readonly List<GeometryInstance3D> _geoms = new();

    // placeholder rig
    Node3D _root, _hips, _legL, _legR, _armL, _armR, _torso;
    float _phase, _runBlend, _moveBlend, _air, _t;

    public override void _Ready()
    {
        var model = Models.Exists(ModelName) ? Models.Instantiate(ModelName, MatKind.Character) : null;
        if (model != null)
        {
            AddChild(model);
            _anim = model.FindChild("AnimationPlayer", true, false) as AnimationPlayer
                    ?? FindFirst<AnimationPlayer>(model);
            foreach (var n in model.FindChildren("*", "GeometryInstance3D", true, false)) _geoms.Add((GeometryInstance3D)n);
            ApplyOutfit();
            if (_anim != null) BindAnimations();
        }
        if (model == null || _anim == null)
        {
            if (model != null) { model.QueueFree(); _geoms.Clear(); }
            BuildPlaceholder();
        }
        ApplyColors();
    }

    /// <summary>Show "body" plus the <see cref="Outfit"/> parts, hide every other accessory/tool mesh.</summary>
    public void ApplyOutfit()
    {
        if (IsPlaceholder) return;
        foreach (var g in _geoms)
        {
            string n = g.Name.ToString();
            bool show = n == "body" || n.StartsWith("body") || Array.IndexOf(Outfit, n) >= 0;
            g.Visible = show;
        }
    }

    public void SetOutfit(params string[] parts) { Outfit = parts; ApplyOutfit(); }

    static T FindFirst<T>(Node n) where T : class
    {
        if (n is T t) return t;
        foreach (var c in n.GetChildren()) { var r = FindFirst<T>(c); if (r != null) return r; }
        return null;
    }

    public void ApplyColors()
    {
        foreach (var g in _geoms)
        {
            g.SetInstanceShaderParameter("skin_color", ToVec(Skin));
            g.SetInstanceShaderParameter("cloth1_color", ToVec(Cloth1));
            g.SetInstanceShaderParameter("cloth2_color", ToVec(Cloth2));
            g.SetInstanceShaderParameter("hair_color", ToVec(Hair));
        }
    }

    static Vector3 ToVec(Color srgb) { var l = srgb.SrgbToLinear(); return new Vector3(l.R, l.G, l.B); }

    void BindAnimations()
    {
        string Find(string key)
        {
            foreach (var n in _anim.GetAnimationList())
                if (n.Equals(key, StringComparison.OrdinalIgnoreCase) || n.EndsWith("|" + key, StringComparison.OrdinalIgnoreCase)
                    || n.EndsWith("/" + key, StringComparison.OrdinalIgnoreCase)) return n;
            foreach (var n in _anim.GetAnimationList())
                if (n.Contains(key, StringComparison.OrdinalIgnoreCase)) return n;
            return null;
        }
        _idle = Find("Idle"); _walk = Find("Walk"); _run = Find("Run"); _jump = Find("Jump");
        foreach (var key in new[] { "Idle", "Walk", "Run", "Carry", "Jump", "Chop", "Hoe", "Hammer", "Sit", "Talk", "Wave", "Attack", "Hit", "Gather", "Pray", "Die" })
        {
            var n = Find(key);
            if (n == null) continue;
            _clips[key] = n;
            if (Array.IndexOf(LoopClips, key) >= 0) _anim.GetAnimation(n).LoopMode = Animation.LoopModeEnum.Linear;
        }
        if (_idle != null) { _anim.Play(_idle); _current = _idle; }
    }

    /// <summary>Play a named clip (Idle, Walk, Run, Carry, Chop, Hoe, Hammer, Sit, Talk, Gather, Pray, Wave, Attack…).
    /// Locomotion clips are speed-matched to <paramref name="groundSpeed"/>. Returns false if the clip is missing.</summary>
    public bool Drive(string clip, float groundSpeed, float blend = 0.25f)
    {
        if (_anim == null) { UpdatePlaceholder(clip is "Walk" or "Run" or "Carry" ? groundSpeed : 0f, true, 0f, (float)GetProcessDeltaTime()); return false; }
        if (!_clips.TryGetValue(clip, out var name) && !_clips.TryGetValue("Idle", out name)) return false;
        float speed = 1f;
        if (clip is "Walk" or "Carry") speed = Math.Clamp(groundSpeed / WalkAnimSpeed, 0.4f, 1.8f);
        else if (clip == "Run") speed = Math.Clamp(groundSpeed / RunAnimSpeed, 0.6f, 1.6f);
        if (name != _current)
        {
            _anim.Play(name, customBlend: blend);
            _current = name;
        }
        _anim.SpeedScale = speed * _speedMul;
        return true;
    }

    float _speedMul = 1f;
    /// <summary>Global playback multiplier (e.g. slightly varied per NPC so crowds don't move in lockstep).</summary>
    public float SpeedMul { get => _speedMul; set => _speedMul = value; }

    /// <summary>Restart the current clip from the start (one-shots like Attack).</summary>
    public void Restart() { if (_anim != null && _current != null) { _anim.Stop(); _anim.Play(_current); } }

    public bool HasClip(string clip) => _clips.ContainsKey(clip);
    public bool IsPlaying() => _anim?.IsPlaying() ?? false;

    /// <summary>Drive the animation. groundSpeed in m/s, onFloor, vertical velocity.</summary>
    public void UpdateMotion(float groundSpeed, bool onFloor, float vy, float dt)
    {
        _t += dt;
        if (_anim != null) { UpdateGlb(groundSpeed, onFloor, vy); return; }
        UpdatePlaceholder(groundSpeed, onFloor, vy, dt);
    }

    void UpdateGlb(float v, bool onFloor, float vy)
    {
        string want; float speed = 1f;
        if (!onFloor && _jump != null && (vy > 0.5f || vy < -3f)) want = _jump;
        else if (v < 0.15f || _walk == null) want = _idle ?? _walk;
        else if (v < 2.6f || _run == null) { want = _walk; speed = Math.Clamp(v / WalkAnimSpeed, 0.4f, 1.8f); }
        else { want = _run; speed = Math.Clamp(v / RunAnimSpeed, 0.6f, 2.4f); }
        if (want == null) return;
        if (want != _current)
        {
            _anim.Play(want, customBlend: want == _jump ? 0.12 : 0.25);
            _current = want;
        }
        _anim.SpeedScale = speed;
    }

    // ------------------------------------------------------------------------------------ placeholder figure

    void BuildPlaceholder()
    {
        IsPlaceholder = true;
        var mat = Models.MaterialFor(MatKind.Character);
        Color skin = new(1, 1, 1, 0.25f), cloth1 = new(1, 1, 1, 0.5f), cloth2 = new(1, 1, 1, 0.75f), hair = new(1, 1, 1, 1f);
        Color cloth1dk = new(0.8f, 0.8f, 0.8f, 0.5f), cloth2dk = new(0.8f, 0.8f, 0.8f, 0.75f);
        Color boot = MeshKit.C(0x3b2a1e), belt = MeshKit.C(0x5a3d22), buckle = MeshKit.C(0xc9a24a), eye = MeshKit.C(0x1e1a18);

        _root = new Node3D { Name = "Figure" };
        AddChild(_root);
        MeshInstance3D Part(Node3D parent, string name, Vector3 pos, Action<MeshKit> build)
        {
            var k = new MeshKit();
            build(k);
            var mi = new MeshInstance3D { Name = name, Mesh = k.ToMesh(), MaterialOverride = mat, Position = pos };
            parent.AddChild(mi);
            _geoms.Add(mi);
            return mi;
        }
        Node3D Joint(Node3D parent, string name, Vector3 pos) { var n = new Node3D { Name = name, Position = pos }; parent.AddChild(n); return n; }

        _hips = Joint(_root, "Hips", new Vector3(0, 0.93f, 0));
        Part(_hips, "Pelvis", Vector3.Zero, k =>
        {
            k.Box(new Vector3(0, 0.03f, 0), new Vector3(0.36f, 0.2f, 0.22f), cloth2);
            k.Box(new Vector3(0, 0.12f, 0), new Vector3(0.39f, 0.07f, 0.245f), belt);
            k.Box(new Vector3(0, 0.12f, 0.125f), new Vector3(0.07f, 0.06f, 0.02f), buckle);
        });
        _torso = Joint(_hips, "Torso", new Vector3(0, 0.14f, 0));
        Part(_torso, "Chest", Vector3.Zero, k =>
        {
            k.Box(new Vector3(0, 0.22f, 0), new Vector3(0.40f, 0.44f, 0.23f), cloth1, 1.06f);
            k.Box(new Vector3(0, -0.02f, 0), new Vector3(0.38f, 0.1f, 0.235f), cloth1dk);   // tunic hem
            k.Box(new Vector3(0, 0.47f, 0), new Vector3(0.12f, 0.08f, 0.12f), skin);       // neck
            // head
            k.Box(new Vector3(0, 0.62f, 0.01f), new Vector3(0.22f, 0.25f, 0.23f), skin);
            k.Box(new Vector3(0, 0.6f, 0.13f), new Vector3(0.05f, 0.07f, 0.04f), skin);    // nose
            k.Box(new Vector3(-0.055f, 0.645f, 0.123f), new Vector3(0.035f, 0.03f, 0.01f), eye);
            k.Box(new Vector3(0.055f, 0.645f, 0.123f), new Vector3(0.035f, 0.03f, 0.01f), eye);
            k.Box(new Vector3(0, 0.76f, -0.02f), new Vector3(0.245f, 0.07f, 0.25f), hair);   // hair cap
            k.Box(new Vector3(0, 0.66f, -0.105f), new Vector3(0.24f, 0.2f, 0.05f), hair);    // back of head
        });
        Node3D Leg(float x)
        {
            var hip = Joint(_hips, x < 0 ? "LegL" : "LegR", new Vector3(x, 0, 0));
            Part(hip, "Leg", Vector3.Zero, k =>
            {
                k.Box(new Vector3(0, -0.4f, 0), new Vector3(0.15f, 0.78f, 0.16f), cloth2);
                k.Box(new Vector3(0, -0.84f, 0.03f), new Vector3(0.155f, 0.18f, 0.25f), boot);
                k.Box(new Vector3(0, -0.72f, 0), new Vector3(0.165f, 0.06f, 0.175f), cloth2dk);
            });
            return hip;
        }
        _legL = Leg(-0.1f); _legR = Leg(0.1f);
        Node3D Arm(float x)
        {
            var sh = Joint(_torso, x < 0 ? "ArmL" : "ArmR", new Vector3(x, 0.4f, 0));
            Part(sh, "Arm", Vector3.Zero, k =>
            {
                k.Box(new Vector3(0, -0.2f, 0), new Vector3(0.12f, 0.42f, 0.13f), cloth1);
                k.Box(new Vector3(0, -0.47f, 0), new Vector3(0.1f, 0.14f, 0.11f), skin);
            });
            sh.Rotation = new Vector3(0, 0, x < 0 ? -0.08f : 0.08f);
            return sh;
        }
        _armL = Arm(-0.26f); _armR = Arm(0.26f);
    }

    void UpdatePlaceholder(float v, bool onFloor, float vy, float dt)
    {
        float k = 1f - MathF.Exp(-dt * 10f);
        _moveBlend = Mathf.Lerp(_moveBlend, Math.Clamp(v / 1.2f, 0f, 1f), k);
        _runBlend = Mathf.Lerp(_runBlend, FMath.Smoothstep(2.0f, 4.5f, v), k);
        _air = Mathf.Lerp(_air, onFloor ? 0f : 1f, 1f - MathF.Exp(-dt * 8f));
        float stride = Mathf.Lerp(1.3f, 2.3f, _runBlend);
        _phase += v / stride * MathF.Tau * dt;
        float amp = _moveBlend * Mathf.Lerp(0.42f, 0.75f, _runBlend) * (1f - _air);
        float s = MathF.Sin(_phase);
        _legL.Rotation = new Vector3(s * amp - _air * 0.5f, 0, 0);
        _legR.Rotation = new Vector3(-s * amp - _air * 0.2f, 0, 0);
        float armAmp = amp * 0.75f + _air * 0.2f;
        _armL.Rotation = new Vector3(-s * armAmp - _runBlend * 0.25f - _air * 0.9f, 0, -0.08f - _air * 0.3f);
        _armR.Rotation = new Vector3(s * armAmp - _runBlend * 0.25f - _air * 0.9f, 0, 0.08f + _air * 0.3f);
        float bob = MathF.Abs(MathF.Cos(_phase)) * Mathf.Lerp(0.03f, 0.07f, _runBlend) * _moveBlend * (1f - _air);
        float breathe = (1f - _moveBlend) * MathF.Sin(_t * 1.8f) * 0.006f;
        _hips.Position = new Vector3(0, 0.93f - 0.03f * _moveBlend + bob, 0);
        _torso.Rotation = new Vector3(0.05f * _moveBlend + 0.14f * _runBlend, MathF.Sin(_phase) * 0.06f * _moveBlend, 0);
        _torso.Scale = new Vector3(1, 1 + breathe, 1);
    }
}
