using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace FD.World;

/// <summary>
/// Renders and collides the <see cref="Heightfield"/>: 15 × 15 chunks of 80 m. Each chunk has three meshes —
/// LOD0 (2 m grid), LOD1 (4 m) and LOD2 (8 m) — switched by GeometryInstance3D visibility ranges measured from
/// the chunk centre, with skirts hanging below the chunk borders to hide LOD cracks. Collision is one
/// ConcavePolygonShape3D per chunk built from the full-resolution triangles (same split as
/// <see cref="Heightfield.Height"/>), on StaticBody3D layer 1 ("terrain").
/// Ground cover comes from the heightfield's splat maps sampled per pixel in assets/shaders/terrain.gdshader.
/// </summary>
public partial class Terrain : Node3D
{
    public const float ChunkSize = 80f;
    public const int ChunkCells = 40;
    public const int Chunks = 15;
    static readonly int[] Step = { 1, 2, 4 };
    static readonly float[] SkirtDepth = { 1.5f, 3.5f, 7f };
    /// <summary>Distances (camera → chunk centre) at which LOD0→1 and LOD1→2 switch.</summary>
    public static readonly float[] LodSwitch = { 190f, 470f };
    /// <summary>Fade-out band (m) of the finer LOD beyond its switch distance.</summary>
    public const float LodFade = 12f;
    public const uint CollisionLayer = 1;

    /// <summary>Ground palette (sRGB hex) pushed into terrain.gdshader and grass.gdshader (source_color uniforms).</summary>
    public static readonly Dictionary<string, uint> Palette = new()
    {
        ["grass_lush"] = 0x427f33, ["grass_dry"] = 0x7b9346, ["grass_deep"] = 0x2c5f2b, ["meadow_col"] = 0x6a933c,
        ["forest_floor"] = 0x384824, ["forest_litter"] = 0x5b4b2d,
        ["dirt_col"] = 0xa98a57, ["dirt_dark"] = 0x7b5f3f, ["soil_col"] = 0x6f4c2f, ["soil_dark"] = 0x4a3220,
        ["earth_col"] = 0x7b6041, ["rock_col"] = 0x908b83, ["rock_dark"] = 0x615c56,
        ["sand_col"] = 0x9d8d68, ["sand_wet"] = 0x5f5645,
        ["crop_wheat"] = 0xd0b056, ["crop_barley"] = 0xbab868, ["crop_veg"] = 0x4d8a37, ["crop_flax"] = 0x9caf5c,
    };

    public ShaderMaterial Material { get; private set; }
    /// <summary>Per-LOD copies of <see cref="Material"/> (lod_min/lod_max set); runtime parameter changes should
    /// go through <see cref="SetParam"/> so every LOD gets them.</summary>
    public ShaderMaterial[] LodMaterials { get; private set; }

    public void SetParam(string name, Variant value)
    {
        Material.SetShaderParameter(name, value);
        if (LodMaterials != null) foreach (var m in LodMaterials) m.SetShaderParameter(name, value);
    }
    public ImageTexture SplatA { get; private set; }
    public ImageTexture SplatB { get; private set; }
    public ImageTexture SplatC { get; private set; }
    public int TriangleCountLod0 { get; private set; }

    Heightfield _hf;

    public void Build(Heightfield hf, bool collision = true)
    {
        _hf = hf;
        Name = "Terrain";
        CreateMaterial();
        LodMaterials = new ShaderMaterial[3];
        for (int l = 0; l < 3; l++)
        {
            var m = (ShaderMaterial)Material.Duplicate();
            m.SetShaderParameter("lod_min", l == 0 ? 0f : LodSwitch[l - 1]);
            m.SetShaderParameter("lod_max", l == 2 ? 100000f : LodSwitch[l]);
            m.SetShaderParameter("lod_fade", LodFade);
            LodMaterials[l] = m;
        }

        // meshes are computed in parallel (pure C# arrays), then handed to Godot on the main thread
        var jobs = new List<(int cx, int cz)>();
        for (int cz = 0; cz < Chunks; cz++)
            for (int cx = 0; cx < Chunks; cx++) jobs.Add((cx, cz));
        var data = new ChunkArrays[jobs.Count, 3];
        var faces = new Vector3[jobs.Count][];
        Parallel.For(0, jobs.Count, j =>
        {
            var (cx, cz) = jobs[j];
            for (int l = 0; l < 3; l++) data[j, l] = BuildChunkArrays(cx, cz, Step[l], SkirtDepth[l]);
            if (collision) faces[j] = BuildCollisionFaces(cx, cz);
        });

        for (int j = 0; j < jobs.Count; j++)
        {
            var (cx, cz) = jobs[j];
            var chunk = new Node3D { Name = $"Chunk_{cx}_{cz}" };
            AddChild(chunk);
            for (int l = 0; l < 3; l++)
            {
                // LOD switching happens per fragment in the shader (dithered crossfade on the distance to the camera,
                // see terrain.gdshader); instance visibility ranges only cull chunks that lie entirely outside their
                // LOD's band (chunk half-diagonal ≈ 57 m, so the ranges are widened by that much).
                float reach = ChunkSize * 0.72f;
                var mi = new MeshInstance3D
                {
                    Name = $"LOD{l}",
                    Mesh = ToMesh(data[j, l]),
                    MaterialOverride = LodMaterials[l],
                    CastShadow = l == 2 ? GeometryInstance3D.ShadowCastingSetting.Off : GeometryInstance3D.ShadowCastingSetting.On,
                    GIMode = GeometryInstance3D.GIModeEnum.Disabled,
                    VisibilityRangeBegin = l == 0 ? 0f : MathF.Max(0f, LodSwitch[l - 1] - LodFade - reach),
                    VisibilityRangeEnd = l == 2 ? 0f : LodSwitch[l] + reach,
                    VisibilityRangeFadeMode = GeometryInstance3D.VisibilityRangeFadeModeEnum.Disabled,
                };
                chunk.AddChild(mi);
            }
            if (collision)
            {
                var body = new StaticBody3D { Name = "Body", CollisionLayer = CollisionLayer, CollisionMask = 0 };
                var shape = new ConcavePolygonShape3D { BackfaceCollision = false };
                shape.SetFaces(faces[j]);
                body.AddChild(new CollisionShape3D { Shape = shape });
                chunk.AddChild(body);
            }
        }
        TriangleCountLod0 = Chunks * Chunks * ChunkCells * ChunkCells * 2;
    }

    // ------------------------------------------------------------------------------------------ material

    void CreateMaterial()
    {
        SplatA = MakeTexture(_hf.SplatA, true);
        SplatB = MakeTexture(_hf.SplatB, true);
        SplatC = MakeTexture(_hf.SplatC, false);
        Material = new ShaderMaterial { Shader = GD.Load<Shader>("res://assets/shaders/terrain.gdshader") };
        Material.SetShaderParameter("splat_a", SplatA);
        Material.SetShaderParameter("splat_b", SplatB);
        Material.SetShaderParameter("splat_c", SplatC);
        Material.SetShaderParameter("noise_tex", MakeNoise(RegionSpec.Seed, 0.011f, 5));
        Material.SetShaderParameter("detail_normal", new NoiseTexture2D
        {
            Width = 256, Height = 256, Seamless = true, AsNormalMap = true, BumpStrength = 5f, GenerateMipmaps = true,
            Noise = new FastNoiseLite { Seed = RegionSpec.Seed + 3, NoiseType = FastNoiseLite.NoiseTypeEnum.Cellular, Frequency = 0.045f,
                                        FractalType = FastNoiseLite.FractalTypeEnum.Fbm, FractalOctaves = 2,
                                        CellularReturnType = FastNoiseLite.CellularReturnTypeEnum.Distance },
        });
        Material.SetShaderParameter("region", new Vector4(Heightfield.Min, Heightfield.Min, RegionSpec.Size, RegionSpec.Size));
        foreach (var (k, v) in Palette) Material.SetShaderParameter(k, FD.Core.FMath.Hex(v));
    }

    static ImageTexture MakeTexture(byte[] rgba, bool mips)
    {
        var img = Image.CreateFromData(Heightfield.SplatRes, Heightfield.SplatRes, false, Image.Format.Rgba8, rgba);
        if (mips) img.GenerateMipmaps();
        return ImageTexture.CreateFromImage(img);
    }

    /// <summary>Seamless fbm noise texture (grayscale) used for painterly detail variation.</summary>
    public static NoiseTexture2D MakeNoise(int seed, float frequency, int octaves, int size = 512)
    {
        return new NoiseTexture2D
        {
            Width = size, Height = size, Seamless = true, GenerateMipmaps = true, Normalize = true,
            Noise = new FastNoiseLite
            {
                Seed = seed, NoiseType = FastNoiseLite.NoiseTypeEnum.SimplexSmooth, Frequency = frequency,
                FractalType = FastNoiseLite.FractalTypeEnum.Fbm, FractalOctaves = octaves,
            },
        };
    }

    // ------------------------------------------------------------------------------------------ meshes

    sealed class ChunkArrays
    {
        public Vector3[] V, Nrm;
        public int[] I;
    }

    ChunkArrays BuildChunkArrays(int cx, int cz, int step, float skirt)
    {
        var hf = _hf;
        int n = ChunkCells / step + 1;               // vertices per side
        int ix0 = cx * ChunkCells, iz0 = cz * ChunkCells;
        int perim = 4 * (n - 1);
        var v = new Vector3[n * n + perim];
        var nr = new Vector3[v.Length];
        for (int j = 0; j < n; j++)
            for (int i = 0; i < n; i++)
            {
                int gx = ix0 + i * step, gz = iz0 + j * step;
                v[j * n + i] = new Vector3(Heightfield.GX(gx), hf.H[Heightfield.Idx(gx, gz)], Heightfield.GX(gz));
                nr[j * n + i] = hf.GridNormal(gx, gz);
            }
        var idx = new List<int>((n - 1) * (n - 1) * 6 + perim * 6);
        for (int j = 0; j < n - 1; j++)
            for (int i = 0; i < n - 1; i++)
            {
                int a = j * n + i, b = a + 1, c = a + n, d = c + 1;
                // split along b–c, matching Heightfield.Height; clockwise from above = front face
                idx.Add(a); idx.Add(b); idx.Add(c);
                idx.Add(b); idx.Add(d); idx.Add(c);
            }
        // skirts: walk the border clockwise (seen from above) and hang a strip below each edge
        var ring = new List<int>(perim);
        for (int i = 0; i < n - 1; i++) ring.Add(i);                          // north edge, west → east
        for (int j = 0; j < n - 1; j++) ring.Add(j * n + (n - 1));            // east edge, north → south
        for (int i = n - 1; i > 0; i--) ring.Add((n - 1) * n + i);            // south edge, east → west
        for (int j = n - 1; j > 0; j--) ring.Add(j * n);                      // west edge, south → north
        int baseIdx = n * n;
        for (int r = 0; r < perim; r++)
        {
            int top = ring[r];
            v[baseIdx + r] = v[top] - new Vector3(0, skirt, 0);
            nr[baseIdx + r] = nr[top];
        }
        for (int r = 0; r < perim; r++)
        {
            int p0 = ring[r], p1 = ring[(r + 1) % perim];
            int q0 = baseIdx + r, q1 = baseIdx + (r + 1) % perim;
            idx.Add(p0); idx.Add(q0); idx.Add(p1);
            idx.Add(p1); idx.Add(q0); idx.Add(q1);
        }
        return new ChunkArrays { V = v, Nrm = nr, I = idx.ToArray() };
    }

    static ArrayMesh ToMesh(ChunkArrays c)
    {
        var arrays = new Godot.Collections.Array();
        arrays.Resize((int)Mesh.ArrayType.Max);
        arrays[(int)Mesh.ArrayType.Vertex] = c.V;
        arrays[(int)Mesh.ArrayType.Normal] = c.Nrm;
        arrays[(int)Mesh.ArrayType.Index] = c.I;
        var mesh = new ArrayMesh();
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
        return mesh;
    }

    Vector3[] BuildCollisionFaces(int cx, int cz)
    {
        var hf = _hf;
        int n = ChunkCells;
        int ix0 = cx * ChunkCells, iz0 = cz * ChunkCells;
        var f = new Vector3[n * n * 6];
        int o = 0;
        Vector3 P(int gx, int gz) => new(Heightfield.GX(gx), hf.H[Heightfield.Idx(gx, gz)], Heightfield.GX(gz));
        for (int j = 0; j < n; j++)
            for (int i = 0; i < n; i++)
            {
                int gx = ix0 + i, gz = iz0 + j;
                Vector3 a = P(gx, gz), b = P(gx + 1, gz), c = P(gx, gz + 1), d = P(gx + 1, gz + 1);
                f[o++] = a; f[o++] = b; f[o++] = c;
                f[o++] = b; f[o++] = d; f[o++] = c;
            }
        return f;
    }
}
