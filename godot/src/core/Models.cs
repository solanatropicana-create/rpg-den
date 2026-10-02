using System;
using System.Collections.Generic;
using Godot;

namespace FD.Core;

/// <summary>Which material a loaded mesh should carry.</summary>
public enum MatKind
{
    /// <summary>fd_vc.gdshader — MeshInstance3D; per-instance "tint"/"lit" instance uniforms.</summary>
    Static,
    /// <summary>fd_vc_mm.gdshader — MultiMesh with use_custom_data (custom.rgb = tint, custom.a = lit).</summary>
    Multi,
    /// <summary>grass.gdshader — near-field MultiMesh grass/flowers (custom.rgb = tint).</summary>
    Grass,
    /// <summary>fd_char.gdshader — characters; slot colours via instance uniforms.</summary>
    Character,
}

/// <summary>
/// Loads Blender-exported .glb files from res://assets/models once and hands out their meshes by node name.
/// <para>In the editor/dev build the raw .glb is parsed at runtime with GLTFDocument (so freshly re-exported
/// files are always current and no import step is needed); exported builds use the imported resource. Either
/// path falls back to the other. Missing files are not an error: <see cref="Exists"/> lets callers use
/// placeholders.</para>
/// Vertex colours arrive as linear RGBA (glTF COLOR_0), exactly what the fd_* shaders expect.
/// </summary>
public static class Models
{
    public const string Dir = "res://assets/models/";

    static readonly Dictionary<string, PackedScene> _scenes = new();
    static readonly Dictionary<string, Dictionary<string, Mesh>> _raw = new();
    static readonly Dictionary<(string, MatKind), Dictionary<string, Mesh>> _byKind = new();
    static readonly Dictionary<MatKind, Material> _materials = new();

    /// <summary>Dev switch (--noassets): behave as if no glb exists, to test the procedural placeholders.</summary>
    public static bool ForceMissing;

    public static bool Exists(string model) =>
        !ForceMissing && (FileAccess.FileExists(Dir + model + ".glb") || ResourceLoader.Exists(Dir + model + ".glb"));

    /// <summary>Shared material for a kind (vegetation code usually duplicates it to set LOD/wind uniforms).</summary>
    public static Material MaterialFor(MatKind kind)
    {
        if (_materials.TryGetValue(kind, out var m)) return m;
        string path = kind switch
        {
            MatKind.Static => "res://assets/shaders/fd_vc.gdshader",
            MatKind.Multi => "res://assets/shaders/fd_vc_mm.gdshader",
            MatKind.Grass => "res://assets/shaders/grass.gdshader",
            _ => "res://assets/shaders/fd_char.gdshader",
        };
        m = new ShaderMaterial { Shader = GD.Load<Shader>(path) };
        _materials[kind] = m;
        return m;
    }

    /// <summary>Packed template scene of a model (null if the file is missing or unreadable).</summary>
    public static PackedScene Scene(string model)
    {
        if (ForceMissing) return null;
        if (_scenes.TryGetValue(model, out var ps)) return ps;
        string path = Dir + model + ".glb";
        bool preferRuntime = OS.HasFeature("editor");
        ps = preferRuntime ? LoadRuntime(path) ?? LoadImported(path) : LoadImported(path) ?? LoadRuntime(path);
        if (ps == null) GD.PushWarning($"Models: '{model}' not available ({path}).");
        _scenes[model] = ps;
        return ps;
    }

    static PackedScene LoadImported(string path)
    {
        if (!ResourceLoader.Exists(path)) return null;
        return ResourceLoader.Load<PackedScene>(path);
    }

    static PackedScene LoadRuntime(string path)
    {
        if (!FileAccess.FileExists(path)) return null;
        var doc = new GltfDocument();
        var state = new GltfState();
        if (doc.AppendFromFile(path, state) != Error.Ok) return null;
        var root = doc.GenerateScene(state);
        if (root == null) return null;
        SetOwners(root, root);
        var ps = new PackedScene();
        var err = ps.Pack(root);
        root.Free();
        return err == Error.Ok ? ps : null;
    }

    static void SetOwners(Node n, Node owner)
    {
        foreach (var c in n.GetChildren())
        {
            c.Owner = owner;
            SetOwners(c, owner);
        }
    }

    /// <summary>All meshes in the model, keyed by their node name, with <paramref name="kind"/>'s material on
    /// every surface (ready for MeshInstance3D.Mesh or MultiMesh.Mesh). Empty if the model is missing.</summary>
    public static IReadOnlyDictionary<string, Mesh> Meshes(string model, MatKind kind = MatKind.Static)
    {
        if (_byKind.TryGetValue((model, kind), out var d)) return d;
        var raw = RawMeshes(model);
        d = new Dictionary<string, Mesh>();
        var mat = MaterialFor(kind);
        foreach (var (name, mesh) in raw)
        {
            var copy = (Mesh)mesh.Duplicate();
            if (copy is ArrayMesh am)
                for (int i = 0; i < am.GetSurfaceCount(); i++) am.SurfaceSetMaterial(i, mat);
            d[name] = copy;
        }
        _byKind[(model, kind)] = d;
        return d;
    }

    /// <summary>One mesh by node name (null if missing).</summary>
    public static Mesh Mesh(string model, string node, MatKind kind = MatKind.Static) =>
        Meshes(model, kind).TryGetValue(node, out var m) ? m : null;

    static Dictionary<string, Mesh> RawMeshes(string model)
    {
        if (_raw.TryGetValue(model, out var d)) return d;
        d = new Dictionary<string, Mesh>();
        var ps = Scene(model);
        if (ps != null)
        {
            var root = ps.Instantiate<Node>();
            Collect(root, d);
            root.Free();
        }
        _raw[model] = d;
        return d;
    }

    static void Collect(Node n, Dictionary<string, Mesh> d)
    {
        if (n is MeshInstance3D mi && mi.Mesh != null && !d.ContainsKey(mi.Name)) d[mi.Name] = mi.Mesh;
        foreach (var c in n.GetChildren()) Collect(c, d);
    }

    /// <summary>Instantiate the whole model (e.g. a skinned character with its AnimationPlayer). Every
    /// MeshInstance3D gets <paramref name="kind"/>'s material as override. Null if missing.</summary>
    public static Node3D Instantiate(string model, MatKind kind = MatKind.Character)
    {
        var ps = Scene(model);
        if (ps == null) return null;
        var root = ps.Instantiate<Node3D>();
        var mat = MaterialFor(kind);
        foreach (var n in root.FindChildren("*", "MeshInstance3D", true, false))
            ((MeshInstance3D)n).MaterialOverride = mat;
        return root;
    }

    /// <summary>Parsed manifest (&lt;model&gt;.json next to the glb) or null.</summary>
    public static Godot.Collections.Dictionary Manifest(string model)
    {
        string path = Dir + model + ".json";
        if (!FileAccess.FileExists(path)) return null;
        var v = Json.ParseString(FileAccess.GetFileAsString(path));
        return v.VariantType == Variant.Type.Dictionary ? v.AsGodotDictionary() : null;
    }

    /// <summary>Axis-aligned bounds of a mesh (convenience for collision proxies).</summary>
    public static Aabb Bounds(Mesh m) => m?.GetAabb() ?? new Aabb();

    /// <summary>Forget cached models (e.g. after re-exporting a glb while running).</summary>
    public static void Clear() { _scenes.Clear(); _raw.Clear(); _byKind.Clear(); }
}
