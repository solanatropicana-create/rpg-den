using System;
using System.Collections.Generic;
using Godot;

namespace FD.World;

/// <summary>
/// Stream ribbon and pond surfaces. The stream mesh follows <see cref="RegionSpec.Stream"/> at the heightfield's
/// water profile (<see cref="Heightfield.StreamWaterAt"/>), 1 m wider than the water line on each side so its
/// edges tuck under the banks; UV.x = arc length (metres, flow runs toward s = 0), UV.y = −1..1 across.
/// The pond is a disc at <see cref="Heightfield.PondLevel"/>. Both use assets/shaders/water.gdshader.
/// No collision (the player wades on the carved bed).
/// </summary>
public partial class Water : Node3D
{
    public ShaderMaterial StreamMaterial { get; private set; }
    public ShaderMaterial PondMaterial { get; private set; }

    const int Across = 6;            // vertices across the ribbon
    const int SamplesPerPiece = 60;  // 120 m pieces for culling

    public void Build(Heightfield hf)
    {
        Name = "Water";
        var shader = GD.Load<Shader>("res://assets/shaders/water.gdshader");
        var normals = MakeNormalNoise(RegionSpec.Seed + 5, 0.035f);
        var normals2 = MakeNormalNoise(RegionSpec.Seed + 6, 0.06f);
        StreamMaterial = new ShaderMaterial { Shader = shader };
        StreamMaterial.SetShaderParameter("normal_a", normals);
        StreamMaterial.SetShaderParameter("normal_b", normals2);
        StreamMaterial.SetShaderParameter("flowing", true);
        PondMaterial = (ShaderMaterial)StreamMaterial.Duplicate();
        PondMaterial.SetShaderParameter("flowing", false);
        PondMaterial.SetShaderParameter("deep_depth", 2.2f);

        BuildStream(hf);
        BuildPond(hf);
    }

    static NoiseTexture2D MakeNormalNoise(int seed, float freq) => new()
    {
        Width = 256, Height = 256, Seamless = true, AsNormalMap = true, BumpStrength = 6f, GenerateMipmaps = true,
        Noise = new FastNoiseLite
        {
            Seed = seed, NoiseType = FastNoiseLite.NoiseTypeEnum.SimplexSmooth, Frequency = freq,
            FractalType = FastNoiseLite.FractalTypeEnum.Fbm, FractalOctaves = 3,
        },
    };

    void BuildStream(Heightfield hf)
    {
        var line = RegionSpec.Stream;
        int n = hf.StreamWater.Length;
        for (int start = 0; start < n - 1; start += SamplesPerPiece)
        {
            int end = Math.Min(start + SamplesPerPiece, n - 1);
            var v = new List<Vector3>(); var nr = new List<Vector3>(); var uv = new List<Vector2>(); var idx = new List<int>();
            for (int i = start; i <= end; i++)
            {
                float s = MathF.Min(i * Heightfield.StreamDs, line.Length);
                Vector2 p = line.PointAt(s);
                // smooth tangent over ±4 m
                Vector2 t = (line.PointAt(MathF.Min(s + 4f, line.Length)) - line.PointAt(MathF.Max(s - 4f, 0f))).Normalized();
                Vector2 side = new(-t.Y, t.X);
                float w = hf.StreamWaterAt(s);
                float half = hf.StreamWaterHalfWidthAt(s) + 1.0f;
                for (int j = 0; j < Across; j++)
                {
                    float u = j / (float)(Across - 1) * 2f - 1f;
                    Vector2 q = p + side * (u * half);
                    v.Add(new Vector3(q.X, w, q.Y));
                    nr.Add(Vector3.Up);
                    uv.Add(new Vector2(s, u));
                }
            }
            int rows = end - start + 1;
            for (int r = 0; r < rows - 1; r++)
                for (int j = 0; j < Across - 1; j++)
                {
                    int a = r * Across + j, b = a + 1, c = a + Across, d = c + 1;
                    // winding: pick the order that faces +Y (clockwise from above)
                    Vector3 pa = v[a], pb = v[b], pc = v[c];
                    if ((pc - pa).Cross(pb - pa).Y > 0) { idx.Add(a); idx.Add(b); idx.Add(c); idx.Add(b); idx.Add(d); idx.Add(c); }
                    else { idx.Add(a); idx.Add(c); idx.Add(b); idx.Add(b); idx.Add(c); idx.Add(d); }
                }
            AddChild(new MeshInstance3D
            {
                Name = $"Stream{start / SamplesPerPiece}",
                Mesh = MakeMesh(v, nr, uv, idx),
                MaterialOverride = StreamMaterial,
                CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
            });
        }
    }

    void BuildPond(Heightfield hf)
    {
        Vector2 c = RegionSpec.Pond;
        const int seg = 72, rings = 5;
        var v = new List<Vector3>(); var nr = new List<Vector3>(); var uv = new List<Vector2>(); var idx = new List<int>();
        float y = hf.PondLevel;
        v.Add(new Vector3(c.X, y, c.Y)); nr.Add(Vector3.Up); uv.Add(Vector2.Zero);
        for (int r = 1; r <= rings; r++)
            for (int i = 0; i < seg; i++)
            {
                float a = i * MathF.Tau / seg;
                Vector2 dir = new(MathF.Cos(a), MathF.Sin(a));
                float rad = (hf.PondRadiusAt(c + dir) + 2.5f) * r / rings;
                Vector2 q = c + dir * rad;
                v.Add(new Vector3(q.X, y, q.Y)); nr.Add(Vector3.Up); uv.Add(q - c);
            }
        for (int i = 0; i < seg; i++)
        {
            int a = 1 + i, b = 1 + (i + 1) % seg;
            // angle grows clockwise seen from above: centre → a → b is clockwise
            idx.Add(0); idx.Add(a); idx.Add(b);
        }
        for (int r = 1; r < rings; r++)
            for (int i = 0; i < seg; i++)
            {
                int a = 1 + (r - 1) * seg + i, b = 1 + (r - 1) * seg + (i + 1) % seg;
                int c2 = a + seg, d = b + seg;
                idx.Add(a); idx.Add(c2); idx.Add(b);
                idx.Add(b); idx.Add(c2); idx.Add(d);
            }
        AddChild(new MeshInstance3D
        {
            Name = "Pond", Mesh = MakeMesh(v, nr, uv, idx), MaterialOverride = PondMaterial,
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
        });
    }

    static ArrayMesh MakeMesh(List<Vector3> v, List<Vector3> n, List<Vector2> uv, List<int> idx)
    {
        var arrays = new Godot.Collections.Array();
        arrays.Resize((int)Mesh.ArrayType.Max);
        arrays[(int)Mesh.ArrayType.Vertex] = v.ToArray();
        arrays[(int)Mesh.ArrayType.Normal] = n.ToArray();
        arrays[(int)Mesh.ArrayType.TexUV] = uv.ToArray();
        arrays[(int)Mesh.ArrayType.Index] = idx.ToArray();
        var m = new ArrayMesh();
        m.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
        return m;
    }
}
