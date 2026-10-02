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
            Outfit = look.Outfit, Ears = look.Ears,
        };
        if (P.Role == Role.Goblin) { Body.WalkAnimSpeed = 1.1f; Body.RunAnimSpeed = 3.8f; }
        _baseOutfit = look.Outfit;
        Body.Scale = new Vector3(Scale01 * look.Wide, Scale01, Scale01 * look.Wide);
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
        if (p.Dead || p.InFight || p.Down) { SyncFight(dt, hf, space, cam); return; }
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

    /// <summary>Faz 2: in a fight (the fight sets Pos, Dir, FightAnim) or lying dead/unconscious (Die clip, held on its last frame).</summary>
    void SyncFight(float dt, Heightfield hf, PhysicsDirectSpaceState3D space, Vector3 cam)
    {
        var p = P;
        _enterT = -1f;
        if (!Visible) { Visible = true; _yInit = false; }
        _col.CollisionLayer = p.Dead || p.Down ? 0u : App.LayerActors;
        float gy = hf.Height(p.Pos.X, p.Pos.Y);
        _y = _yInit ? Mathf.Lerp(_y, gy, 1f - MathF.Exp(-dt * 14f)) : gy;
        _yInit = true;
        Position = new Vector3(p.Pos.X, _y, p.Pos.Y);
        string clip = p.Dead || p.Down ? "Die" : p.FightAnim ?? "Idle";
        float speed = clip == "Run" ? p.RunSpeed : clip == "Walk" ? p.WalkSpeed : 0f;
        string tool = p.Role == Role.Goblin && !p.Dead ? (p.IsBoss ? "tool_club" : (p.Work % 3 == 0 ? "tool_spear" : "tool_club")) : null;
        string key = tool ?? "";
        if (key != _outfitKey)
        {
            _outfitKey = key;
            if (tool == null) Body.SetOutfit(_baseOutfit);
            else { var o = new string[_baseOutfit.Length + 1]; _baseOutfit.CopyTo(o, 0); o[^1] = tool; Body.SetOutfit(o); }
        }
        if (clip == "Attack" && _lastClip == "Attack" && !Body.IsPlaying()) Body.Restart();
        if (clip != "Die" || _lastClip != "Die") Body.Drive(clip, speed, clip == "Die" ? 0.15f : 0.2f);
        _lastClip = clip;
        if (!(p.Dead || p.Down))
        {
            float want = MathF.Atan2(p.Dir.X, p.Dir.Y);
            _yaw = Mathf.LerpAngle(_yaw, want, 1f - MathF.Exp(-dt * 12f));
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
    /// <summary>Faz 2: width multiplier (stocky dwarves and half-orcs, slender elves) and ear shape (see Humanoid.Ears)</summary>
    public float Wide;
    public int Ears;

    /// <summary>Faz 2: how a people looks on the shared human model: height, build, skin, ears, beard (gerçek modeller sonra).</summary>
    public readonly struct RaceLook
    {
        public readonly float Height, Wide; public readonly int Ears; public readonly int[] Skins; public readonly float BeardMale, BeardFemale; public readonly bool Hair;
        public RaceLook(float h, float w, int ears, int[] skins, float bm, float bf, bool hair = true) { Height = h; Wide = w; Ears = ears; Skins = skins; BeardMale = bm; BeardFemale = bf; Hair = hair; }
    }

    public static RaceLook Look(string race) => race switch
    {
        "dwarf" => new(0.8f, 1.16f, 0, new[] { 0xe8b896, 0xd9a27c, 0xc68a63, 0xb07450 }, 1f, 0.2f),
        "elf" => new(1.05f, 0.9f, 2, new[] { 0xf2d6be, 0xe8c4a4, 0xd4a888, 0x9a7a6a }, 0f, 0f),
        "halfling" => new(0.62f, 1.06f, 1, new[] { 0xf0c8a8, 0xe0b48f, 0xd9a27c, 0xc68a63 }, 0.15f, 0f),
        "gnome" => new(0.58f, 1.02f, 1, new[] { 0xf0c8a8, 0xe0b48f, 0xd9a27c }, 0.5f, 0f),
        "halfelf" => new(1.0f, 0.95f, 1, new[] { 0xf0c8a8, 0xe0b48f, 0xd9a27c, 0xc68a63, 0xa8704f }, 0.25f, 0f),
        "halforc" => new(1.07f, 1.14f, 1, new[] { 0x8a9a74, 0x7a8a68, 0x9aa080, 0x6f7f5c }, 0.3f, 0f),
        "dragonborn" => new(1.09f, 1.14f, 0, new[] { 0xa87a3c, 0xb89448, 0x8a3a2a, 0x4a7a4a, 0x3a5a8a, 0x9a9a9a }, 0f, 0f, false),
        "tiefling" => new(1.0f, 0.98f, 1, new[] { 0xa84a44, 0x8a3a5a, 0x7a3a6a, 0xb05a4a }, 0.3f, 0f),
        _ => new(1f, 1f, 0, null, 0.45f, 0f),
    };

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
            if (p.IsBoss) { o.Clear(); o.Add("helmet"); ap.Cloth1 = FMath.Hex(0x7a2a1au); }
            ap.Outfit = o.ToArray();
            ap.Scale = p.IsBoss ? 1.22f : 0.95f + r.Next01() * 0.12f;
            ap.Wide = p.IsBoss ? 1.15f : 1f;
            return ap;
        }
        var rl = Look(p.Race);
        if (rl.Skins != null) ap.Skin = FMath.Hex(Pick(rl.Skins));
        ap.Wide = rl.Wide;
        ap.Ears = rl.Ears;
        if (!rl.Hair) { }
        else if (p.Female) o.Add(r.Chance(0.5f) ? "hair_long" : "hair_bun");
        else if (p.Age >= 18 && r.Chance(0.3f) && p.Age > 50) { /* bald-ish */ }
        else o.Add("hair_short");
        bool adult = p.Role is not (Role.Child or Role.Apprentice or Role.StableHand);
        if (adult && r.Chance(p.Female ? rl.BeardFemale : rl.BeardMale)) o.Add("beard");
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
        var ages = FD.Game.RegionBind.Ages(p.Race);
        ap.Scale = rl.Height * ((p.Female ? 0.95f : 1.0f) + (r.Next01() - 0.5f) * 0.07f - (p.Age > ages.old + 10 ? 0.03f : 0f));
        return ap;
    }
}
