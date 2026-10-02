using System;
using Godot;
using FD.Actors;
using FD.Core;
using FD.Sim.Life;
using Rng = FD.Sim.Life.Rng;
using FD.World;

namespace FD.Life;

/// <summary>
/// The visible body of one simulated person: a Humanoid (human.glb / goblin.glb) placed where the sim says,
/// playing the clip of what they are doing, holding the right tool, facing their work, their talk partner or
/// the player. Kinematic capsule so the hero bumps into people instead of walking through them.
/// </summary>
public partial class PersonActor : Node3D
{
    public Person P;
    public Humanoid Body;
    AnimatableBody3D _col;
    Label3D _bubble;
    float _bubbleT;
    float _yaw, _y;
    bool _yInit;
    float _enterT = -1f;
    Vector3 _enterFrom;
    string _outfitKey;
    string[] _baseOutfit = Array.Empty<string>();
    string _lastClip;
    public float Scale01 = 1f;
    public double LastGreet = -999;
    public Vector3 HeadPos => GlobalPosition + Vector3.Up * (Scale01 * (P.Role == Role.Goblin ? 1.35f : 1.95f));

    public static PersonActor Create(Person p)
    {
        var a = new PersonActor { P = p, Name = $"P{p.Id}_{p.Name.Replace(' ', '_')}" };
        return a;
    }

    public override void _Ready()
    {
        var look = Appearance.For(P);
        Scale01 = look.Scale;
        Body = new Humanoid
        {
            Name = "Body", ModelName = P.Role == Role.Goblin ? "goblin" : "human",
            Skin = look.Skin, Cloth1 = look.Cloth1, Cloth2 = look.Cloth2, Hair = look.Hair,
            Outfit = look.Outfit,
        };
        if (P.Role == Role.Goblin) { Body.WalkAnimSpeed = 1.1f; Body.RunAnimSpeed = 3.8f; }
        _baseOutfit = look.Outfit;
        Body.Scale = Vector3.One * Scale01;
        AddChild(Body);
        Body.SpeedMul = 0.93f + H.Hash(P.Id, 5) * 0.14f;

        _col = new AnimatableBody3D { Name = "Col", CollisionLayer = App.LayerActors, CollisionMask = 0, SyncToPhysics = false };
        _col.AddChild(new CollisionShape3D
        {
            Shape = new CapsuleShape3D { Radius = 0.3f * Scale01, Height = 1.7f * Scale01 },
            Position = new Vector3(0, 0.85f * Scale01, 0),
        });
        AddChild(_col);

        _bubble = new Label3D
        {
            Name = "Bubble", Billboard = BaseMaterial3D.BillboardModeEnum.Enabled, FontSize = 40, PixelSize = 0.0042f,
            OutlineSize = 10, Modulate = new Color(1, 0.97f, 0.9f), OutlineModulate = new Color(0.08f, 0.06f, 0.05f, 0.85f),
            Position = new Vector3(0, (P.Role == Role.Goblin ? 1.55f : 2.25f) * Scale01, 0), Visible = false, NoDepthTest = true,
            Width = 520, AutowrapMode = TextServer.AutowrapMode.WordSmart, FixedSize = false,
        };
        AddChild(_bubble);
    }

    public void Say(string text, float seconds = 3.5f)
    {
        _bubble.Text = text;
        _bubble.Visible = true;
        _bubbleT = seconds;
    }

    /// <summary>Sync with the sim (called by LifeWorld every frame for nearby people).</summary>
    public void Sync(float dt, Heightfield hf, PhysicsDirectSpaceState3D space, Vector3 cam, LifeSim sim, Vector3 playerPos)
    {
        var p = P;
        if (_bubbleT > 0) { _bubbleT -= dt; if (_bubbleT <= 0) _bubble.Visible = false; }
        bool inside = p.Motion == Motion.Inside || !p.Present;
        Vector3 target = new(p.Pos.X, 0, p.Pos.Y);

        // entering a building: keep walking into the doorway for a moment, then vanish
        if (inside)
        {
            if (Visible && _enterT < 0 && p.Present && p.Act != null && p.Act.Place >= 0)
            {
                _enterT = 0.75f;
                _enterFrom = Position;
            }
            if (_enterT >= 0)
            {
                _enterT -= dt;
                var place = sim.Places[p.Act.Place];
                Vector3 inward = new(-place.DoorFace.X, 0, -place.DoorFace.Y);
                Position += inward * dt * 1.4f;
                Body.Drive("Walk", 1.4f);
                if (_enterT < 0) { Visible = false; _col.CollisionLayer = 0; }
                return;
            }
            Visible = false;
            _col.CollisionLayer = 0;
            return;
        }
        _enterT = -1f;
        if (!Visible) { Visible = true; _col.CollisionLayer = App.LayerActors; _yInit = false; }

        // ground height: ray on props (porches, bridge) + terrain near the camera, heightfield otherwise
        float gy = hf.Height(target.X, target.Z);
        float distCam = new Vector2(cam.X - target.X, cam.Z - target.Z).Length();
        if (distCam < 90f && space != null)
        {
            float from = (_yInit ? _y : gy) + 1.1f;
            var q = PhysicsRayQueryParameters3D.Create(new Vector3(target.X, from, target.Z), new Vector3(target.X, from - 4f, target.Z), App.LayerTerrain | App.LayerProps);
            var hit = space.IntersectRay(q);
            if (hit.Count > 0) gy = MathF.Max(gy, ((Vector3)hit["position"]).Y);
        }
        _y = _yInit ? Mathf.Lerp(_y, gy, 1f - MathF.Exp(-dt * 14f)) : gy;
        _yInit = true;
        Position = new Vector3(target.X, _y, target.Z);

        // animation + tools
        var a = p.Act;
        string clip; string tool = null;
        float speed = 0f;
        bool walking = p.Motion == Motion.Walking;
        if (walking)
        {
            speed = p.Running ? p.RunSpeed : p.WalkSpeed;
            if (p.HasCart) speed = MathF.Min(speed, 1.15f);
            clip = a?.Kind == ActKind.Chase ? "Run" : p.Running ? "Run" : a?.WalkAnim ?? "Walk";
            if (clip == "Run" && speed < 2.2f) clip = "Walk";
            tool = p.SubTargetTimer > 0 ? a?.Tool : a?.WalkTool;
            if (p.SubTargetTimer > 0 && (tool is "tool_hoe" or "tool_axe" or "tool_pitchfork") ) tool = null;
        }
        else
        {
            clip = a?.Anim ?? "Idle";
            tool = a?.Tool;
        }
        if (P.Role == Role.Goblin && GameClock.IsNight && walking && tool == null) tool = "tool_torch";
        string key = tool ?? "";
        if (key != _outfitKey)
        {
            _outfitKey = key;
            if (tool == null) Body.SetOutfit(_baseOutfit);
            else { var o = new string[_baseOutfit.Length + 1]; _baseOutfit.CopyTo(o, 0); o[^1] = tool; Body.SetOutfit(o); }
        }
        if (clip == "Attack" && _lastClip == "Attack" && !Body.IsPlaying()) Body.Restart();
        Body.Drive(clip, speed);
        _lastClip = clip;

        // facing
        Vector2 face = walking ? new Vector2(p.Dir.X, p.Dir.Y) : (a != null && a.Face != System.Numerics.Vector2.Zero ? new Vector2(a.Face.X, a.Face.Y) : new Vector2(p.Dir.X, p.Dir.Y));
        if (!walking && a != null)
        {
            if (clip == "Talk" && a.Spot?.Tag != "vendor")
            {
                // turn to the nearest other talker
                Person best = null; float bd = 3.8f * 3.8f;
                foreach (var o in sim.People)
                {
                    if (o == p || !o.Visible || o.Motion != Motion.Doing || o.Act == null) continue;
                    if (o.Act.Anim != "Talk" && o.Act.Anim != "Sit" && o.Act.Kind != ActKind.Work) continue;
                    float d = System.Numerics.Vector2.DistanceSquared(o.Pos, p.Pos);
                    if (d < bd) { bd = d; best = o; }
                }
                if (best != null) face = new Vector2(best.Pos.X - p.Pos.X, best.Pos.Y - p.Pos.Y).Normalized();
            }
            // idle/talking people glance at a hero standing right next to them
            float dp = new Vector2(playerPos.X - target.X, playerPos.Z - target.Z).Length();
            if (dp < 2.6f && clip is "Idle" or "Talk")
                face = new Vector2(playerPos.X - target.X, playerPos.Z - target.Z).Normalized();
        }
        if (face.LengthSquared() > 1e-4f)
        {
            float want = MathF.Atan2(face.X, face.Y);
            _yaw = Mathf.LerpAngle(_yaw, want, 1f - MathF.Exp(-dt * (walking ? 9f : 5f)));
            Rotation = new Vector3(0, _yaw, 0);
        }
    }

    public void SnapYaw()
    {
        var f = P.Act != null && P.Act.Face != System.Numerics.Vector2.Zero ? P.Act.Face : P.Dir;
        _yaw = MathF.Atan2(f.X, f.Y);
        Rotation = new Vector3(0, _yaw, 0);
    }
}

/// <summary>Deterministic look of a person (colours, hair, hats, scale) from their role, sex, age and seed.</summary>
public struct Appearance
{
    public Color Skin, Cloth1, Cloth2, Hair;
    public string[] Outfit;
    public float Scale;

    static readonly int[] SkinTones = { 0xf0c8a8, 0xe0b48f, 0xd9a27c, 0xc68a63, 0xa8704f, 0x8a5a3c };
    static readonly int[] HairCols = { 0x2b1a10, 0x4a2e1a, 0x6b4424, 0xa0683a, 0xc9a060, 0x1a1a1a, 0x7a2e1a };
    static readonly int[] GreyHair = { 0xc8c8c8, 0xe0e0e0, 0x9a9a9a, 0xb0aaa0 };
    static readonly int[] Trousers = { 0x4c3b2b, 0x5a4a3a, 0x3a3a40, 0x6a5a45, 0x4a4030 };

    public static Appearance For(Person p)
    {
        var r = new Rng((ulong)p.Look + 17);
        uint Pick(int[] a) => (uint)a[r.Int(a.Length)];
        var ap = new Appearance
        {
            Skin = FMath.Hex(Pick(SkinTones)),
            Hair = FMath.Hex(p.Age >= 58 ? Pick(GreyHair) : Pick(HairCols)),
            Cloth2 = FMath.Hex(Pick(Trousers)),
        };
        int[] shirts = p.Role switch
        {
            Role.Farmer => new[] { 0x5a7a3a, 0x7a8a4a, 0x8a6a3a, 0x6a7a5a, 0x9a8050 },
            Role.Homemaker => new[] { 0x8a4a5a, 0x5a6a9a, 0xa0704a, 0x7a5a8a, 0x4a7a7a },
            Role.Smith or Role.Apprentice => new[] { 0x6a4a3a, 0x5a5a5a },
            Role.Woodcutter => new[] { 0x3f5a2f, 0x7a3a2a, 0x5a4a2a },
            Role.Shepherd => new[] { 0x8a8a6a, 0x9a7a5a },
            Role.Headman => new[] { 0x6a2a3a, 0x2f4a6a },
            Role.Priest => new[] { 0xe8e0d0 },
            Role.Innkeeper => new[] { 0x7a5a3a },
            Role.InnServant => new[] { 0x9a4a4a, 0x6a5a8a },
            Role.StableHand => new[] { 0x8a6a3a },
            Role.Child => new[] { 0xc0603a, 0x4a7aa0, 0xa0a040, 0x7aa05a, 0xb05a7a },
            Role.Elder => new[] { 0x6a6a7a, 0x7a6a5a, 0x5a5a4a },
            Role.Merchant => new[] { 0x5a3a7a },
            Role.Pilgrim => new[] { 0x8a7a60 },
            Role.Adventurer => new[] { 0x3a5a3a },
            _ => new[] { 0x5a4a3a },
        };
        ap.Cloth1 = FMath.Hex(Pick(shirts));
        var o = new System.Collections.Generic.List<string>();
        if (p.Role == Role.Goblin)
        {
            int[] gskin = { 0x6a8a3a, 0x7a9a4a, 0x5a7a3a, 0x7a8a40 };
            ap.Skin = FMath.Hex(Pick(gskin));
            ap.Cloth1 = FMath.Hex(0x5a4030u); ap.Cloth2 = FMath.Hex(0x4a3a2au); ap.Hair = FMath.Hex(0x2a2a1au);
            if (p.Work is 0 or 1) o.Add("helmet"); else if (r.Chance(0.5f)) o.Add("hood");
            ap.Outfit = o.ToArray();
            ap.Scale = 0.95f + r.Next01() * 0.12f;
            return ap;
        }
        if (p.Female) o.Add(r.Chance(0.5f) ? "hair_long" : "hair_bun");
        else if (p.Age >= 18 && r.Chance(0.3f) && p.Age > 50) { /* bald-ish */ }
        else o.Add("hair_short");
        if (!p.Female && p.Age >= 20 && r.Chance(0.45f)) o.Add("beard");
        switch (p.Role)
        {
            case Role.Farmer: if (r.Chance(0.45f)) o.Add("hat_straw"); break;
            case Role.Shepherd: if (p.Age >= 16) o.Add("hat_straw"); break;
            case Role.Smith: case Role.Apprentice: case Role.Innkeeper: o.Add("apron"); break;
            case Role.Homemaker: if (r.Chance(0.5f)) o.Add("apron"); break;
            case Role.Priest: o.Clear(); o.Add("hood"); ap.Cloth2 = FMath.Hex(0xd8d0c0); break;
            case Role.Pilgrim: o.Add("hood"); ap.Cloth2 = FMath.Hex(0x6a5a40); break;
            case Role.Adventurer: o.Add("hood"); ap.Cloth2 = FMath.Hex(0x2a3a2a); o.Add("tool_spear"); break;
            case Role.StableHand: if (r.Chance(0.5f)) o.Add("hat_straw"); break;
        }
        ap.Outfit = o.ToArray();
        ap.Scale = p.Age switch
        {
            < 6 => 0.56f,
            < 10 => 0.65f,
            < 13 => 0.75f,
            < 16 => 0.87f,
            _ => (p.Female ? 0.95f : 1.0f) + (r.Next01() - 0.5f) * 0.07f - (p.Age > 70 ? 0.03f : 0f),
        };
        return ap;
    }
}
