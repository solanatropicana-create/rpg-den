using System;
using Godot;
using FD.Actors;
using FD.Rpg;

namespace FD.UI;

/// <summary>Faz 2: turning 3D preview of a character (creation screen, character sheet): own world, light and camera.</summary>
public partial class CharacterPreview : SubViewportContainer
{
    SubViewport _vp;
    Node3D _pivot;
    Humanoid _body;
    float _yaw = 0.4f;
    public bool AutoTurn = true;
    string _key;

    public CharacterPreview()
    {
        Stretch = true;
        MouseFilter = MouseFilterEnum.Pass;
        _vp = new SubViewport { OwnWorld3D = true, TransparentBg = true, Msaa3D = Viewport.Msaa.Msaa4X, Size = new Vector2I(360, 480) };
        AddChild(_vp);
        var env = new Godot.Environment
        {
            BackgroundMode = Godot.Environment.BGMode.ClearColor, BackgroundColor = new Color(0, 0, 0, 0),
            AmbientLightSource = Godot.Environment.AmbientSource.Color, AmbientLightColor = new Color(0.62f, 0.58f, 0.52f), AmbientLightEnergy = 0.9f,
            TonemapMode = Godot.Environment.ToneMapper.Filmic,
        };
        _vp.AddChild(new WorldEnvironment { Environment = env });
        var sun = new DirectionalLight3D { LightEnergy = 1.25f, LightColor = new Color(1f, 0.93f, 0.82f), ShadowEnabled = false };
        sun.RotationDegrees = new Vector3(-38, 35, 0);
        _vp.AddChild(sun);
        var rim = new DirectionalLight3D { LightEnergy = 0.5f, LightColor = new Color(0.6f, 0.7f, 1f) };
        rim.RotationDegrees = new Vector3(-20, 200, 0);
        _vp.AddChild(rim);
        var camPos = new Vector3(0, 1.05f, 4.4f);
        var cam = new Camera3D { Fov = 30f, Transform = new Transform3D(Basis.LookingAt(new Vector3(0, 0.88f, 0) - camPos, Vector3.Up), camPos) };
        _vp.AddChild(cam);
        _pivot = new Node3D { Name = "Pivot" };
        _vp.AddChild(_pivot);
        // ground disc
        var disc = new MeshInstance3D { Mesh = new CylinderMesh { TopRadius = 0.7f, BottomRadius = 0.7f, Height = 0.02f }, Position = new Vector3(0, -0.01f, 0) };
        disc.MaterialOverride = new StandardMaterial3D { AlbedoColor = new Color(0.25f, 0.2f, 0.15f, 0.85f), Transparency = BaseMaterial3D.TransparencyEnum.Alpha, ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded };
        _pivot.AddChild(disc);
    }

    /// <summary>Show this character (rebuilds the body when race, colours, hair or kit changed).</summary>
    public void Show(Character c)
    {
        string key = $"{c.Race}|{c.Look.Skin}|{c.Look.Hair}|{c.Look.Cloth1}|{c.Look.Cloth2}|{c.Look.HairStyle}|{c.Look.Beard}|{c.Look.Female}|{c.Look.Height}|{c.Weapon}|{c.Armor}|{c.Shield}";
        if (key == _key) return;
        _key = key;
        _body?.QueueFree();
        _body = LookKit.Body(c);
        _pivot.AddChild(_body);
    }

    public override void _GuiInput(InputEvent e)
    {
        if (e is InputEventMouseMotion mm && (mm.ButtonMask & MouseButtonMask.Left) != 0) { _yaw += mm.Relative.X * 0.012f; AutoTurn = false; }
    }

    public override void _Process(double delta)
    {
        float dt = (float)delta;
        if (AutoTurn) _yaw += dt * 0.45f;
        _pivot.Rotation = new Vector3(0, _yaw, 0);
        _body?.UpdateMotion(0, true, 0, dt);
    }
}
