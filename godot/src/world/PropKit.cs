using System;
using System.Collections.Generic;
using Godot;
using FD.Core;

namespace FD.World;

/// <summary>
/// Places Blender props/buildings (village.glb, buildings.glb, nature.glb) with their manifest data:
/// hi/lo LOD MeshInstances for unique buildings, MultiMesh batches for repeated small props (fences, crops,
/// barrels), collision boxes from the manifest on one StaticBody per kit, and manifest points → world space.
/// Facing convention: a model's front is local +Z; <c>yaw = RegionSpec.YawFacing(dir)</c>.
/// </summary>
public sealed class PropKit
{
    public readonly Node3D Root;
    readonly StaticBody3D _body;
    readonly Dictionary<(string, string), List<(Transform3D xf, Color custom)>> _batches = new();

    public PropKit(Node parent, string name)
    {
        Root = new Node3D { Name = name };
        parent.AddChild(Root);
        _body = new StaticBody3D { Name = "Collision", CollisionLayer = App.LayerProps, CollisionMask = 0 };
        Root.AddChild(_body);
    }

    // ------------------------------------------------------------------------------------------ manifest

    static readonly Dictionary<string, Godot.Collections.Dictionary> _objs = new();

    public static Godot.Collections.Dictionary Obj(string model, string obj)
    {
        string key = model + "/" + obj;
        if (_objs.TryGetValue(key, out var d)) return d;
        var man = Models.Manifest(model);
        d = null;
        if (man != null && man.ContainsKey("objects"))
        {
            var o = man["objects"].AsGodotDictionary();
            if (o.ContainsKey(obj)) d = o[obj].AsGodotDictionary();
        }
        _objs[key] = d;
        return d;
    }

    /// <summary>Local position of a manifest point (Godot local coordinates), or null.</summary>
    public static Vector3? Point(string model, string obj, string point)
    {
        var o = Obj(model, obj);
        if (o == null || !o.ContainsKey("points")) return null;
        var pts = o["points"].AsGodotDictionary();
        if (!pts.ContainsKey(point)) return null;
        var p = pts[point].AsGodotDictionary()["pos"].AsFloat32Array();
        return new Vector3(p[0], p[1], p[2]);
    }

    /// <summary>Local facing (x, z) of a manifest point, or +Z.</summary>
    public static Vector2 PointFace(string model, string obj, string point)
    {
        var o = Obj(model, obj);
        if (o == null || !o.ContainsKey("points")) return new Vector2(0, 1);
        var pts = o["points"].AsGodotDictionary();
        if (!pts.ContainsKey(point)) return new Vector2(0, 1);
        var pd = pts[point].AsGodotDictionary();
        if (!pd.ContainsKey("facing")) return new Vector2(0, 1);
        var f = pd["facing"].AsFloat32Array();
        return new Vector2(f[0], f[2]);
    }

    public static IEnumerable<string> PointNames(string model, string obj)
    {
        var o = Obj(model, obj);
        if (o == null || !o.ContainsKey("points")) yield break;
        foreach (var k in o["points"].AsGodotDictionary().Keys) yield return k.AsString();
    }

    /// <summary>Local → world for an object placed at pos with yaw.</summary>
    public static Vector3 ToWorld(Vector3 local, Vector3 pos, float yaw) => pos + local.Rotated(Vector3.Up, yaw);
    public static Vector2 ToWorld2(Vector3 local, Vector3 pos, float yaw) { var w = ToWorld(local, pos, yaw); return new Vector2(w.X, w.Z); }
    public static Vector2 DirToWorld(Vector2 localDir, float yaw)
    {
        var v = new Vector3(localDir.X, 0, localDir.Y).Rotated(Vector3.Up, yaw);
        return new Vector2(v.X, v.Z);
    }

    // ------------------------------------------------------------------------------------------ placement

    static Vector3 Lin(Color srgb) { var l = srgb.SrgbToLinear(); return new Vector3(l.R, l.G, l.B); }

    /// <summary>Unique placement with hi/lo LOD (lo = obj + "_lo" if present). Returns the node (null if missing).</summary>
    public Node3D Place(string model, string obj, Vector3 pos, float yaw, Color? tint = null, float lit = 1f,
                        float hiEnd = 160f, bool collide = true, bool shadows = true, string name = null)
    {
        if (!Models.Exists(model)) return null;
        var meshes = Models.Meshes(model, MatKind.Static);
        if (!meshes.TryGetValue(obj, out var hi)) { GD.PushWarning($"PropKit: {model}/{obj} missing"); return null; }
        var node = new Node3D { Name = name ?? obj, Position = pos, Rotation = new Vector3(0, yaw, 0) };
        Root.AddChild(node);
        bool hasLo = meshes.TryGetValue(obj + "_lo", out var lo);
        var cast = shadows ? GeometryInstance3D.ShadowCastingSetting.On : GeometryInstance3D.ShadowCastingSetting.Off;
        var mi = new MeshInstance3D { Name = "hi", Mesh = hi, VisibilityRangeEnd = hasLo ? hiEnd : hiEnd * 2.5f, CastShadow = cast };
        node.AddChild(mi);
        if (hasLo) node.AddChild(new MeshInstance3D { Name = "lo", Mesh = lo, VisibilityRangeBegin = hiEnd, CastShadow = cast });
        foreach (var c in node.GetChildren())
            if (c is GeometryInstance3D g)
            {
                if (tint.HasValue) g.SetInstanceShaderParameter("tint", Lin(tint.Value));
                g.SetInstanceShaderParameter("lit", lit);
            }
        if (collide) AddCollision(model, obj, pos, yaw);
        return node;
    }

    /// <summary>Repeated prop: drawn from a MultiMesh flushed in <see cref="Flush"/>.</summary>
    public void Batch(string model, string obj, Vector3 pos, float yaw, Color? tint = null, float scale = 1f, bool collide = true)
    {
        if (!Models.Exists(model)) return;
        var key = (model, obj);
        if (!_batches.TryGetValue(key, out var list)) _batches[key] = list = new List<(Transform3D, Color)>();
        var t = tint.HasValue ? tint.Value.SrgbToLinear() : new Color(1, 1, 1);
        var xf = new Transform3D(Basis.FromEuler(new Vector3(0, yaw, 0)).Scaled(Vector3.One * scale), pos);
        list.Add((xf, new Color(t.R, t.G, t.B, 1f)));
        if (collide) AddCollision(model, obj, pos, yaw, scale);
    }

    /// <summary>Create the MultiMeshes (one per model/object), visible up to <paramref name="range"/> m.</summary>
    public void Flush(float range = 150f, bool shadows = true)
    {
        foreach (var ((model, obj), list) in _batches)
        {
            var mesh = Models.Mesh(model, obj, MatKind.Multi);
            if (mesh == null || list.Count == 0) continue;
            var mm = new MultiMesh { TransformFormat = MultiMesh.TransformFormatEnum.Transform3D, UseCustomData = true, Mesh = mesh, InstanceCount = list.Count };
            for (int i = 0; i < list.Count; i++) { mm.SetInstanceTransform(i, list[i].xf); mm.SetInstanceCustomData(i, list[i].custom); }
            var mmi = new MultiMeshInstance3D
            {
                Name = "mm_" + obj, Multimesh = mm, VisibilityRangeEnd = range,
                CastShadow = shadows ? GeometryInstance3D.ShadowCastingSetting.On : GeometryInstance3D.ShadowCastingSetting.Off,
            };
            Root.AddChild(mmi);
        }
        _batches.Clear();
    }

    public void AddCollision(string model, string obj, Vector3 pos, float yaw, float scale = 1f)
    {
        var o = Obj(model, obj);
        if (o == null) return;
        foreach (var key in new[] { "collision_boxes", "walkable_boxes" })
        {
            if (!o.ContainsKey(key)) continue;
            foreach (var bx in o[key].AsGodotArray())
            {
                var d = bx.AsGodotDictionary();
                var c = d["center"].AsFloat32Array(); var sz = d["size"].AsFloat32Array();
                float byaw = d.ContainsKey("yaw_deg") ? Mathf.DegToRad((float)d["yaw_deg"].AsDouble()) : 0f;
                var local = new Transform3D(Basis.FromEuler(new Vector3(0, byaw, 0)), new Vector3(c[0], c[1], c[2]) * scale);
                var world = new Transform3D(Basis.FromEuler(new Vector3(0, yaw, 0)), pos) * local;
                AddBox(world, new Vector3(sz[0], MathF.Max(sz[1], 0.12f), sz[2]) * scale);
            }
        }
    }

    public void AddBox(Transform3D world, Vector3 size)
    {
        _body.AddChild(new CollisionShape3D { Shape = new BoxShape3D { Size = size }, Transform = world });
    }

    /// <summary>A warm point light (lamp, torch, forge, fire) that only shines at night; flickers if asked.</summary>
    public OmniLight3D Light(Vector3 worldPos, Color color, float energy = 1.6f, float range = 9f, bool flicker = false, bool alwaysOn = false)
    {
        var l = new NightLight { Position = worldPos, LightColor = color, OmniRange = range, BaseEnergy = energy, Flicker = flicker, AlwaysOn = alwaysOn, ShadowEnabled = false };
        Root.AddChild(l);
        return l;
    }
}

/// <summary>Point light that fades in at dusk (GameClock.Daylight) and optionally flickers like a flame.</summary>
public partial class NightLight : OmniLight3D
{
    public float BaseEnergy = 1.5f;
    public bool Flicker, AlwaysOn;
    float _t, _seed;

    public override void _Ready()
    {
        _seed = Position.X * 0.37f + Position.Z * 0.11f;
        DistanceFadeEnabled = true;
        DistanceFadeBegin = 90f;
        DistanceFadeLength = 40f;
        OmniAttenuation = 1.4f;
        // Tur 1 D: the nights are dark now; lamps, torches and the forge carry them (brighter and wider pools of warm light)
        OmniRange *= 1.25f;
        LightVolumetricFogEnergy = 1.5f;
    }

    /// <summary>Tur 1 D: night light multiplier (dark nights: the warm lights stand out)</summary>
    public const float NightBoost = 1.8f;

    public override void _Process(double delta)
    {
        _t += (float)delta;
        float night = AlwaysOn ? 1f : 1f - GameClock.Daylight;
        float f = Flicker ? 0.82f + 0.1f * MathF.Sin(_t * 11f + _seed) + 0.08f * MathF.Sin(_t * 23.7f + _seed * 3f) : 1f;
        float e = BaseEnergy * (AlwaysOn ? MathF.Max(0.35f, night) : night) * f * FMath.Lerp(1f, NightBoost, night);
        LightEnergy = e;
        Visible = e > 0.02f;
    }
}
