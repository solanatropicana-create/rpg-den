"""
Fantastik Dünya — Blender asset kiti (bpy).

Kurallar (docs/art-pipeline.md ile aynı):
- Blender'da metre ile modellenir. 1 oyun birimi = 15 m (kasabalı ~0,12 birim = 1,8 m).
- Ön yüz Blender'da -Y yönüne bakar (glTF/Three.js'te +Z). Yukarı +Z.
- Renk dokusu yok: her yüz köşe rengi taşır (COLOR_0, RGBA, doğrusal).
    RGB = palet rengi,  A = medeniyet maskesi (1: oyunda örnek rengiyle çarpılır — çatı, sancak vb.).
    Maskeli parçalarda RGB gri bir gölge çarpanıdır (1 = tam medeniyet rengi, 0.8 = biraz koyu).
- Her model iki sürüm: <ad> (uzak/sade) ve <ad>_hi (yakın/ayrıntılı). İkisinin dış ölçüsü aynı.
- Dışa aktarımda her nesne kendi 'norm' ölçüsüne bölünür (ör. ev: x,y / 5 m, z / 4 m) ve orijine alınır;
  oyun örneği (sx, sy, sx) ile ölçekleyip yerleştirir.
"""
import bpy, bmesh, math, random
from mathutils import Vector, Matrix

# ---------------------------------------------------------------- renk
def srgb_to_lin(c: float) -> float:
    return c / 12.92 if c <= 0.04045 else ((c + 0.055) / 1.055) ** 2.4

def hexcol(h: int):
    return tuple(srgb_to_lin(((h >> s) & 255) / 255) for s in (16, 8, 0))

# Palet: oyundaki mevcut renklerle aynı aile (diorama.ts)
PAL = {
    'plaster':   0xe9dcc0,  # badana
    'plaster2':  0xdccfb2,
    'timber':    0x5e3f24,  # yarı ahşap kiriş
    'timber2':   0x4a3120,
    'wood':      0x7a5a3a,
    'wood_lt':   0x9a7650,
    'stone':     0x8d877e,
    'stone2':    0x7a746c,
    'stone3':    0x9e978c,
    'stone_dk':  0x6d6258,
    'glass':     0x23262e,  # ışıksız pencere camı
    'shutter':   0x44604c,
    'shutter2':  0x5a4a7a,
    'soil':      0x5a4030,
    'flower_r':  0xc0392b,
    'flower_y':  0xe8c040,
    'leaf':      0x4f7a3a,
    'iron':      0x3a3a3e,
    'straw':     0xc9a75a,
    'log_end':   0xb08a5a,
}

def col(name_or_hex, mask=0.0, shade=1.0):
    """RGBA (doğrusal). mask=1 → oyunda medeniyet rengiyle boyanır; o zaman RGB = gölge çarpanı."""
    if mask >= 0.5:
        return (shade, shade, shade, 1.0)
    h = PAL[name_or_hex] if isinstance(name_or_hex, str) else name_or_hex
    r, g, b = hexcol(h)
    return (r * shade, g * shade, b * shade, 0.0)

def jit_shade(rng, c, amt=0.06):
    k = 1 + (rng.random() - 0.5) * 2 * amt
    return (c[0] * k, c[1] * k, c[2] * k, c[3])

# ---------------------------------------------------------------- birleştirici
class Builder:
    """Parçaları tek bir bmesh'e yazar; her yüzün köşe rengi ayrı (düz renk)."""
    def __init__(self, seed=1):
        self.bm = bmesh.new()
        self.cl = self.bm.loops.layers.float_color.new('Col')
        self.rng = random.Random(seed)

    def _paint(self, faces, c):
        for f in faces:
            for l in f.loops:
                l[self.cl] = c

    def box(self, c, size, loc, rot=(0, 0, 0), bevel=0.0, taper=None):
        """size=(sx,sy,sz) tam ölçü; loc = taban merkezi değil GÖVDE merkezi. rot derece (x,y,z)."""
        ret = bmesh.ops.create_cube(self.bm, size=1.0)
        verts = ret['verts']
        if taper is not None:  # üst yüzü daralt (tx, ty)
            for v in verts:
                if v.co.z > 0:
                    v.co.x *= taper[0]; v.co.y *= taper[1]
        bmesh.ops.scale(self.bm, vec=Vector(size), verts=verts)
        faces = list({f for v in verts for f in v.link_faces})
        if bevel > 0:
            before = set(self.bm.faces) - set(faces)
            edges = list({e for v in verts for e in v.link_edges})
            bmesh.ops.bevel(self.bm, geom=verts + edges + faces, offset=bevel, segments=1, affect='EDGES', profile=0.5)
            faces = [f for f in self.bm.faces if f not in before]
            verts = list({v for f in faces for v in f.verts})
        R = Matrix.Rotation(math.radians(rot[2]), 4, 'Z') @ Matrix.Rotation(math.radians(rot[1]), 4, 'Y') @ Matrix.Rotation(math.radians(rot[0]), 4, 'X')
        bmesh.ops.transform(self.bm, matrix=Matrix.Translation(Vector(loc)) @ R, verts=verts)
        self._paint(faces, c)
        return faces

    def beam(self, c, a, b, w, h=None, bevel=0.0):
        """a→b arası kare kesitli kiriş (w genişlik, h yükseklik)."""
        a, b = Vector(a), Vector(b)
        d = b - a
        L = d.length
        h = h or w
        ret = bmesh.ops.create_cube(self.bm, size=1.0)
        verts = ret['verts']
        bmesh.ops.scale(self.bm, vec=Vector((w, h, L)), verts=verts)
        faces = list({f for v in verts for f in v.link_faces})
        q = Vector((0, 0, 1)).rotation_difference(d.normalized())
        bmesh.ops.transform(self.bm, matrix=Matrix.Translation((a + b) / 2) @ q.to_matrix().to_4x4(), verts=verts)
        self._paint(faces, c)
        return faces

    def poly(self, c, pts):
        """düz çokgen yüz (pts sırası dışa bakan normal için saat yönü tersine)."""
        vs = [self.bm.verts.new(Vector(p)) for p in pts]
        f = self.bm.faces.new(vs)
        self._paint([f], c)
        return [f]

    def prism(self, c, tri, y0, y1):
        """XZ düzleminde üçgen/çokgen kesit, y0→y1 boyunca uzatılmış kapalı prizma."""
        n = len(tri)
        a = [self.bm.verts.new(Vector((x, y0, z))) for x, z in tri]
        b = [self.bm.verts.new(Vector((x, y1, z))) for x, z in tri]
        faces = [self.bm.faces.new(a[::-1]), self.bm.faces.new(b)]
        for i in range(n):
            j = (i + 1) % n
            faces.append(self.bm.faces.new((a[i], a[j], b[j], b[i])))
        bmesh.ops.recalc_face_normals(self.bm, faces=faces)
        self._paint(faces, c)
        return faces

    def ico(self, c, r, loc, sub=1, squash=1.0):
        ret = bmesh.ops.create_icosphere(self.bm, subdivisions=sub, radius=r)
        verts = ret['verts']
        for v in verts:
            v.co.z *= squash
        bmesh.ops.translate(self.bm, vec=Vector(loc), verts=verts)
        faces = list({f for v in verts for f in v.link_faces})
        self._paint(faces, c)
        return faces

    def cyl(self, c, r, h, loc, seg=6, r2=None, rot=(0, 0, 0)):
        ret = bmesh.ops.create_cone(self.bm, cap_ends=True, segments=seg, radius1=r, radius2=r if r2 is None else r2, depth=h)
        verts = ret['verts']
        R = Matrix.Rotation(math.radians(rot[2]), 4, 'Z') @ Matrix.Rotation(math.radians(rot[1]), 4, 'Y') @ Matrix.Rotation(math.radians(rot[0]), 4, 'X')
        bmesh.ops.transform(self.bm, matrix=Matrix.Translation(Vector(loc)) @ R, verts=verts)
        faces = list({f for v in verts for f in v.link_faces})
        self._paint(faces, c)
        return faces

    def jitter(self, faces, amt, seed=0):
        """seçili yüzlerin köşelerini konuma bağlı sars (aynı köşe aynı yere: dikiş açılmaz)."""
        vs = {v for f in faces for v in f.verts}
        for v in vs:
            k = hash((round(v.co.x, 3), round(v.co.y, 3), round(v.co.z, 3), seed))
            r = random.Random(k)
            v.co += Vector(((r.random() - 0.5) * amt, (r.random() - 0.5) * amt, (r.random() - 0.5) * amt * 0.6))

    def to_object(self, name, collection=None, location=(0, 0, 0)):
        me = bpy.data.meshes.new(name)
        self.bm.normal_update()
        self.bm.to_mesh(me)
        self.bm.free()
        me.color_attributes.active_color_name = 'Col'
        me.color_attributes.render_color_index = me.color_attributes.active_color_index
        ob = bpy.data.objects.new(name, me)
        (collection or bpy.context.scene.collection).objects.link(ob)
        ob.location = location
        ob.data.materials.append(preview_material())
        return ob

# ---------------------------------------------------------------- sahne / materyal
def reset_scene():
    bpy.ops.wm.read_factory_settings(use_empty=True)

_MAT = None
def preview_material(civ=0xb03a2e):
    """Blender'da görmek için: köşe rengi, maskeli yerler örnek medeniyet rengiyle (varsayılan kırmızı)."""
    global _MAT
    if _MAT and _MAT.name in bpy.data.materials:
        return _MAT
    m = bpy.data.materials.new('FD_VertexCol')
    m.use_nodes = True
    nt = m.node_tree
    bsdf = nt.nodes.get('Principled BSDF')
    bsdf.inputs['Roughness'].default_value = 0.9
    attr = nt.nodes.new('ShaderNodeVertexColor'); attr.layer_name = 'Col'
    civc = nt.nodes.new('ShaderNodeRGB'); civc.outputs[0].default_value = (*hexcol(civ), 1); civc.label = 'Medeniyet rengi (önizleme)'
    mul = nt.nodes.new('ShaderNodeMix'); mul.data_type = 'RGBA'; mul.blend_type = 'MULTIPLY'; mul.inputs['Factor'].default_value = 1
    nt.links.new(attr.outputs['Color'], mul.inputs[6]); nt.links.new(civc.outputs[0], mul.inputs[7])
    mix = nt.nodes.new('ShaderNodeMix'); mix.data_type = 'RGBA'
    nt.links.new(attr.outputs['Alpha'], mix.inputs['Factor'])
    nt.links.new(attr.outputs['Color'], mix.inputs[6]); nt.links.new(mul.outputs[2], mix.inputs[7])
    nt.links.new(mix.outputs[2], bsdf.inputs['Base Color'])
    _MAT = m
    return m

def tri_count(ob):
    return sum(len(p.vertices) - 2 for p in ob.data.polygons)

# ---------------------------------------------------------------- dışa aktarım
def export_glb(objs, path, norm):
    """objs: {ad: obje}. norm=(nx, ny, nz) Blender eksenlerinde bölen (m). Kopyalar orijinde, ölçekli."""
    tmp = []
    coll = bpy.data.collections.new('__export'); bpy.context.scene.collection.children.link(coll)
    for name, ob in objs.items():
        ob.name = name + '__src'
    for name, ob in objs.items():
        me = ob.data.copy()
        me.transform(Matrix.Diagonal((1 / norm[0], 1 / norm[1], 1 / norm[2], 1)))
        c = bpy.data.objects.new(name, me)
        coll.objects.link(c)
        tmp.append(c)
    bpy.ops.object.select_all(action='DESELECT')
    for c in tmp:
        c.select_set(True)
    bpy.context.view_layer.objects.active = tmp[0]
    bpy.ops.export_scene.gltf(filepath=path, export_format='GLB', use_selection=True, export_apply=True,
                              export_yup=True, export_normals=False, export_texcoords=False, export_materials='NONE',
                              export_vertex_color='ACTIVE', export_all_vertex_colors=False,
                              export_active_vertex_color_when_no_material=True)
    for c in tmp:
        bpy.data.objects.remove(c)
    bpy.data.collections.remove(coll)
    for name, ob in objs.items():
        ob.name = name

# ---------------------------------------------------------------- önizleme render'ı
def setup_preview(target=(0, 0, 3), dist=18, elev=24, azim=-35, res=(1100, 800), samples=24, lens=50):
    sc = bpy.context.scene
    sc.render.engine = 'CYCLES'
    sc.cycles.samples = samples
    sc.cycles.use_denoising = True
    sc.cycles.device = 'CPU'
    sc.render.resolution_x, sc.render.resolution_y = res
    sc.view_settings.view_transform = 'AgX'
    world = bpy.data.worlds.new('W'); sc.world = world; world.use_nodes = True
    world.node_tree.nodes['Background'].inputs['Color'].default_value = (0.62, 0.72, 0.85, 1)
    world.node_tree.nodes['Background'].inputs['Strength'].default_value = 0.9
    sun = bpy.data.objects.new('Sun', bpy.data.lights.new('Sun', 'SUN')); sc.collection.objects.link(sun)
    sun.data.energy = 3.2; sun.data.angle = math.radians(3)
    sun.rotation_euler = (math.radians(50), math.radians(8), math.radians(-40))
    # zemin
    bpy.ops.mesh.primitive_plane_add(size=200, location=(0, 0, 0))
    g = bpy.context.active_object; gm = bpy.data.materials.new('Ground'); gm.use_nodes = True
    gm.node_tree.nodes['Principled BSDF'].inputs['Base Color'].default_value = (*hexcol(0x7fa650), 1)
    gm.node_tree.nodes['Principled BSDF'].inputs['Roughness'].default_value = 1
    g.data.materials.append(gm)
    cam = bpy.data.objects.new('Cam', bpy.data.cameras.new('Cam')); sc.collection.objects.link(cam); sc.camera = cam
    cam.data.lens = lens
    t = Vector(target)
    e, a = math.radians(elev), math.radians(azim)
    cam.location = t + Vector((math.sin(a) * math.cos(e), -math.cos(a) * math.cos(e), math.sin(e))) * dist
    cam.rotation_euler = (t - cam.location).to_track_quat('-Z', 'Y').to_euler()
    return cam

def render(path):
    bpy.context.scene.render.filepath = path
    bpy.ops.render.render(write_still=True)
