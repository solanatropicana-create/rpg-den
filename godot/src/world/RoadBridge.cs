using System;
using Godot;
using FD.Core;

namespace FD.World;

/// <summary>
/// The wooden bridge where the road crosses the stream (<see cref="RegionSpec.BridgePoint"/>). Uses village.glb
/// "bridge"/"bridge_lo" (origin at the bed centre, deck 1.8 m above it, span along local Z) with its manifest
/// collision; falls back to a simple plank deck if the model is missing. The heightfield already shapes a
/// 1.8 m deep channel and pins the road to bank level here.
/// </summary>
public sealed class RoadBridge : IRegionFeature
{
    public Node3D Node { get; private set; }

    public void Shape(Heightfield hf)
    {
        hf.ExcludeVegetation(RegionSpec.BridgePoint, 9f);
    }

    public void Build(Region region)
    {
        var hf = region.Heightfield;
        Vector2 p = RegionSpec.BridgePoint;
        Vector2 dir = RegionSpec.Road.TangentAt(RegionSpec.BridgeRoadS);
        float deck = hf.StreamBankAt(RegionSpec.BridgeStreamS);
        float y = deck - RegionSpec.BridgeDeckAboveBed;
        Node = new Node3D { Name = "RoadBridge", Position = new Vector3(p.X, y, p.Y), Rotation = new Vector3(0, RegionSpec.YawFacing(dir), 0) };
        region.AddChild(Node);

        var meshes = Models.Exists("village") ? Models.Meshes("village", MatKind.Static) : null;
        var body = new StaticBody3D { Name = "Body", CollisionLayer = App.LayerProps, CollisionMask = 0 };
        Node.AddChild(body);
        if (meshes != null && meshes.TryGetValue("bridge", out var hi))
        {
            Node.AddChild(new MeshInstance3D { Name = "bridge", Mesh = hi, VisibilityRangeEnd = meshes.ContainsKey("bridge_lo") ? 150f : 0f });
            if (meshes.TryGetValue("bridge_lo", out var lo))
                Node.AddChild(new MeshInstance3D { Name = "bridge_lo", Mesh = lo, VisibilityRangeBegin = 150f });
            var man = Models.Manifest("village");
            bool any = false;
            if (man != null && man.ContainsKey("objects"))
            {
                var o = man["objects"].AsGodotDictionary();
                if (o.ContainsKey("bridge"))
                {
                    var b = o["bridge"].AsGodotDictionary();
                    foreach (var key in new[] { "walkable_boxes", "collision_boxes" })
                    {
                        if (!b.ContainsKey(key)) continue;
                        foreach (var bx in b[key].AsGodotArray())
                        {
                            var d = bx.AsGodotDictionary();
                            var c = d["center"].AsFloat32Array(); var sz = d["size"].AsFloat32Array();
                            float yaw = d.ContainsKey("yaw_deg") ? (float)d["yaw_deg"].AsDouble() : 0f;
                            AddBox(body, new Vector3(c[0], c[1], c[2]), new Vector3(sz[0], MathF.Max(sz[1], 0.2f), sz[2]), yaw);
                            any = true;
                        }
                    }
                }
            }
            if (!any) AddBox(body, new Vector3(0, 1.7f, 0), new Vector3(3.5f, 0.2f, 13f), 0);
        }
        else
        {
            // placeholder: plank deck on two beams with rails
            var k = new MeshKit();
            Color plank = MeshKit.C(0x8a6a44), beam = MeshKit.C(0x5a4030);
            for (int i = 0; i < 26; i++)
                k.Box(new Vector3(0, 1.72f, -6.25f + i * 0.5f), new Vector3(3.4f, 0.1f, 0.46f), i % 3 == 0 ? plank.Darkened(0.12f) : plank);
            foreach (float x in new[] { -1.3f, 1.3f }) k.Box(new Vector3(x, 1.5f, 0), new Vector3(0.3f, 0.3f, 13f), beam);
            foreach (float x in new[] { -1.65f, 1.65f })
            {
                k.Box(new Vector3(x, 2.5f, 0), new Vector3(0.12f, 0.12f, 12.6f), beam);
                for (int i = 0; i < 7; i++) k.Box(new Vector3(x, 2.1f, -6f + i * 2f), new Vector3(0.14f, 0.9f, 0.14f), beam);
            }
            Node.AddChild(new MeshInstance3D { Name = "bridge_placeholder", Mesh = k.ToMesh(Models.MaterialFor(MatKind.Static)) });
            AddBox(body, new Vector3(0, 1.72f, 0), new Vector3(3.5f, 0.2f, 13f), 0);
            foreach (float x in new[] { -1.65f, 1.65f }) AddBox(body, new Vector3(x, 2.3f, 0), new Vector3(0.16f, 1.1f, 12.6f), 0);
        }
    }

    static void AddBox(StaticBody3D body, Vector3 center, Vector3 size, float yawDeg)
    {
        body.AddChild(new CollisionShape3D
        {
            Shape = new BoxShape3D { Size = size },
            Transform = new Transform3D(Basis.FromEuler(new Vector3(0, Mathf.DegToRad(yawDeg), 0)), center),
        });
    }
}
