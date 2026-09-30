"""
İnsan evi — yarı ahşap, beşik çatılı, taş temelli.
Çalıştır:  python human_house.py [çıktı_klasörü]   (bpy modülüyle)  ya da Blender > Scripting > Run.
Üretir: human_house.blend, ../models/human_house.glb, önizleme png'leri.

Ölçü (m): gövde 5 × 6.25, duvar üstü 4.0, mahya 7.4. Norm: x,y / 5, z / 4  (oyunda örnek ölçeği sx, sy, sx).
Oyunla anlaşma (diorama.ts, humanHouse):
  kapı:   ön yüz ortası, taban z=0.35, 1.5 × 2.1 m
  pencere (ışık örnekleri): ön x=±1.45, yan y=0; alt z=1.10, 1.05 × 1.05 m
  baca ağzı (A): x=+1.2, y=+1.3, z=8.42
"""
import sys, os, math, random
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import bpy
from fdkit import Builder, col, jit_shade, reset_scene, export_glb, setup_preview, render, tri_count

HERE = os.path.dirname(os.path.abspath(__file__))
W, D = 2.5, 3.125          # yarı genişlik (x), yarı derinlik (y)
PL = 0.35                  # temel üstü
FL = 2.5                   # üst kat tabanı (kuşak kirişi)
TOP = 4.0                  # duvar üstü
RIDGE = 7.4
EAVE_X, EAVE_Z = 3.05, 3.72
OVER_Y = 3.55
WIN_Z0, WIN_W = 1.10, 1.05
FRONT_WX = 1.45
CHIM = (1.2, 1.3)
NORM = (5.0, 5.0, 4.0)


def plinth(b: Builder, rng):
    """Düzensiz taş sırası (temel)."""
    out = 0.12
    n0 = len(b.bm.faces)
    for side in range(4):
        # kenar boyunca taşlar
        if side < 2:
            y = (-D - out / 2) if side == 0 else (D + out / 2)
            x0, x1 = -W - out, W + out
            t = x0
            while t < x1 - 0.05:
                L = min(x1 - t, 0.55 + rng.random() * 0.5)
                c = jit_shade(rng, col(rng.choice(['stone', 'stone2', 'stone3'])), 0.05)
                hh = PL + (rng.random() - 0.5) * 0.06
                b.box(c, (L - 0.04, 0.34 + rng.random() * 0.06, hh), (t + L / 2, y + (rng.random() - 0.5) * 0.03, hh / 2))
                t += L
        else:
            x = (-W - out / 2) if side == 2 else (W + out / 2)
            y0, y1 = -D + 0.1, D - 0.1
            t = y0
            while t < y1 - 0.05:
                L = min(y1 - t, 0.55 + rng.random() * 0.5)
                c = jit_shade(rng, col(rng.choice(['stone', 'stone2', 'stone3'])), 0.05)
                hh = PL + (rng.random() - 0.5) * 0.06
                b.box(c, (0.34 + rng.random() * 0.06, L - 0.04, hh), (x + (rng.random() - 0.5) * 0.03, t + L / 2, hh / 2))
                t += L
    b.bm.faces.ensure_lookup_table()
    b.jitter([b.bm.faces[i] for i in range(n0, len(b.bm.faces))], 0.06, seed=5)


def walls(b: Builder, rng, plaster):
    pc = col(plaster)
    # zemin kat
    b.box(pc, (2 * W, 2 * D, FL - PL), (0, 0, (PL + FL) / 2))
    # üst kat: yanlarda çıkma (jetty)
    b.box(pc, (2 * W + 0.24, 2 * D, TOP - FL), (0, 0, (FL + TOP) / 2))
    # alınlar (ön ve arka üçgen): duvar renginde
    tri = [(-W - 0.12, TOP), (W + 0.12, TOP), (0, RIDGE - 0.12)]
    b.prism(pc, tri, -D, D)


def timber(b: Builder, rng):
    t, t2 = col('timber'), col('timber2')
    w = 0.2
    fo = 0.03   # yüzeyden taşma
    # zemin kat köşe dikmeleri
    for sx in (-1, 1):
        for sy in (-1, 1):
            b.box(t, (w, w, FL - PL), (sx * (W - w / 2 + fo), sy * (D - w / 2 + fo), (PL + FL) / 2))
    # temel üstü eşik kirişi ve kuşak kirişi (çıkma altı)
    for z, h in ((PL + 0.07, 0.14), (FL + 0.05, 0.2)):
        for sy in (-1, 1):
            b.box(t2, (2 * W + 0.1 + (0.24 if z > 1 else 0), 0.2, h), (0, sy * (D + fo), z))
        for sx in (-1, 1):
            xx = W + fo + (0.12 if z > 1 else 0)
            b.box(t2, (0.2, 2 * D + 0.1, h), (sx * xx, 0, z))
    # çıkma dirsekleri (yanlarda, kuşak kirişinin altında)
    for sx in (-1, 1):
        for yy in (-D + 0.35, -1.0, 1.0, D - 0.35):
            b.beam(t2, (sx * (W + 0.0), yy, FL - 0.55), (sx * (W + 0.15), yy, FL - 0.05), 0.12)
    # üst kat: köşe dikmeleri, üst başlık
    for sx in (-1, 1):
        for sy in (-1, 1):
            b.box(t, (w, w, TOP - FL), (sx * (W + 0.12 - w / 2 + fo), sy * (D - w / 2 + fo), (FL + TOP) / 2))
    for sy in (-1, 1):
        b.box(t2, (2 * W + 0.34, 0.2, 0.16), (0, sy * (D + fo), TOP - 0.06))
    for sx in (-1, 1):
        b.box(t2, (0.2, 2 * D + 0.1, 0.16), (sx * (W + 0.12 + fo), 0, TOP - 0.06))
    # üst kat yanları: dikmeler ve çapraz payandalar (yarı ahşap deseni)
    x = W + 0.12 + fo
    posts = [-D + 0.1, -1.55, 0.0, 1.55, D - 0.1]
    for sx in (-1, 1):
        for yy in posts[1:-1]:
            b.box(t, (0.16, 0.16, TOP - FL - 0.2), (sx * x, yy, (FL + TOP) / 2))
        for i in range(len(posts) - 1):
            ya, yb = posts[i] + 0.1, posts[i + 1] - 0.1
            if i % 2 == 0:
                b.beam(t, (sx * x, ya, FL + 0.18), (sx * x, yb, TOP - 0.16), 0.13)
            else:
                b.beam(t, (sx * x, ya, TOP - 0.16), (sx * x, yb, FL + 0.18), 0.13)
    # üst kat ön/arka: dikmeler + St. Andreas çarpısı ortada
    for sy in (-1, 1):
        y = sy * (D + fo)
        for xx in (-1.25, 1.25):
            b.box(t, (0.16, 0.16, TOP - FL - 0.2), (xx, y, (FL + TOP) / 2))
        for xa, xb in ((-W, -1.25), (1.25, W)):
            b.beam(t, (xa + 0.12, y, FL + 0.18), (xb - 0.1, y, TOP - 0.16), 0.13)
    # alın: dikme, maşa kirişi, payandalar
    for sy in (-1, 1):
        y = sy * (D + fo)
        b.box(t, (0.16, 0.16, RIDGE - TOP - 0.4), (0, y, (TOP + RIDGE - 0.4) / 2))
        zc = 5.55
        hw = (W + 0.12) * (RIDGE - 0.12 - zc) / (RIDGE - 0.12 - TOP)
        b.box(t2, (2 * hw - 0.1, 0.16, 0.16), (0, y, zc))
        for sx in (-1, 1):
            b.beam(t, (sx * 2.05, y, TOP + 0.1), (sx * 0.12, y, zc - 0.05), 0.13)


def window(b: Builder, rng, center, normal, shutters=None, glass=True, w=WIN_W, h=WIN_W, z0=WIN_Z0):
    """center=(x,y) duvar yüzeyinde; normal=(nx,ny) dışa. Pencere boşluğu ışık örneğiyle doldurulur."""
    cx, cy = center
    nx, ny = normal
    tx, ty = -ny, nx          # duvar boyunca
    fr = col('timber2')
    depth = 0.1
    ox, oy = cx + nx * depth / 2, cy + ny * depth / 2
    def at(u, dz=0.0, out=0.0):
        return (ox + tx * u + nx * out, oy + ty * u + ny * out, dz)
    rot = 0 if abs(ny) > 0.5 else 90
    fw = 0.1
    # çerçeve
    b.box(fr, (w + 2 * fw, depth, fw), at(0, z0 + h + fw / 2), rot=(0, 0, rot))
    for s in (-1, 1):
        b.box(fr, (fw, depth, h), at(s * (w / 2 + fw / 2), z0 + h / 2), rot=(0, 0, rot))
    # orta kayıt (haç)
    b.box(fr, (0.05, depth * 0.6, h), at(0, z0 + h / 2, 0.02), rot=(0, 0, rot))
    b.box(fr, (w, depth * 0.6, 0.05), at(0, z0 + h * 0.55, 0.02), rot=(0, 0, rot))
    # denizlik
    b.box(col('stone3'), (w + 0.35, 0.22, 0.09), at(0, z0 - 0.05, 0.07), rot=(0, 0, rot))
    if glass:
        b.box(col('glass'), (w, 0.02, h), (cx + nx * 0.005, cy + ny * 0.005, z0 + h / 2), rot=(0, 0, rot))
    if shutters:
        sw = 0.46
        for s in (-1, 1):
            u = s * (w / 2 + fw + sw / 2 + 0.02)
            b.box(col(shutters), (sw, 0.05, h + 0.06), at(u, z0 + h / 2, 0.0), rot=(0, 0, rot))
            for k in (0.25, 0.75):   # tahta çıtaları
                b.box(col(shutters, shade=0.8), (sw * 0.9, 0.03, 0.06), at(u, z0 + h * k, 0.035), rot=(0, 0, rot))


def flower_box(b: Builder, rng, x, y):
    b.box(col('wood'), (1.2, 0.26, 0.22), (x, y - 0.13, WIN_Z0 - 0.2))
    b.box(col('soil'), (1.1, 0.2, 0.04), (x, y - 0.13, WIN_Z0 - 0.08))
    for k in range(5):
        c = col(rng.choice(['flower_r', 'flower_y', 'leaf', 'flower_r']))
        b.ico(c, 0.11 + rng.random() * 0.04, (x - 0.46 + k * 0.23, y - 0.13 + (rng.random() - 0.5) * 0.08, WIN_Z0 - 0.02 + rng.random() * 0.05), sub=0, squash=0.8)


def door(b: Builder, rng):
    y = -D - 0.03
    fr = col('timber')
    b.box(fr, (0.16, 0.16, 2.2), (-0.83, y, PL + 1.1))
    b.box(fr, (0.16, 0.16, 2.2), (0.83, y, PL + 1.1))
    b.box(col('timber2'), (1.95, 0.2, 0.2), (0, y - 0.02, PL + 2.2))
    # basamaklar
    b.box(col('stone3'), (1.8, 0.7, 0.2), (0, -D - 0.45, 0.1), bevel=0.03)
    b.box(col('stone'), (1.6, 0.35, 0.16), (0, -D - 0.25, 0.28), bevel=0.03)
    # kapı üstü küçük saçak
    b.box(col('wood'), (2.1, 0.55, 0.07), (0, -D - 0.28, PL + 2.42), rot=(-14, 0, 0))
    for s in (-1, 1):
        b.beam(col('timber2'), (s * 0.95, -D, PL + 1.95), (s * 0.95, -D - 0.45, PL + 2.36), 0.08)


def roof(b: Builder, rng, rows=5, segs=4):
    ang = math.atan2(RIDGE - EAVE_Z, EAVE_X)
    L = math.hypot(EAVE_X, RIDGE - EAVE_Z) + 0.12
    th = 0.14
    seglen = 2 * OVER_Y / segs
    for s in (-1, 1):
        nx, nz = s * math.sin(ang), math.cos(ang)       # çatı yüzü normali (XZ)
        for i in range(rows):
            t0, t1 = i / rows, (i + 1) / rows + 0.06     # 0 mahya → 1 saçak
            tm = (t0 + t1) / 2
            px = s * (L * tm) * math.cos(ang)
            pz = RIDGE + 0.1 - (L * tm) * math.sin(ang)
            lift = (rows - 1 - i) * 0.035 + th / 2
            off = (i % 2) * seglen / 2
            for k in range(segs + 1):
                y0 = -OVER_Y + k * seglen - off
                y1 = y0 + seglen
                y0, y1 = max(y0, -OVER_Y), min(y1, OVER_Y)
                if y1 - y0 < 0.05:
                    continue
                shade = 0.8 + rng.random() * 0.2 - (0.06 if i == rows - 1 else 0)
                tilt = (rng.random() - 0.5) * 1.6
                b.box(col(None, mask=1, shade=shade), (L * (t1 - t0), y1 - y0 - 0.03, th),
                      (px + nx * lift, (y0 + y1) / 2, pz + nz * lift), rot=(tilt, s * math.degrees(ang), 0))
    # mahya başlığı
    b.box(col(None, mask=1, shade=0.62), (0.34, 2 * OVER_Y + 0.1, 0.2), (0, 0, RIDGE + 0.3), rot=(0, 45, 0))
    # sakak tahtaları (alın kenarı)
    for sy in (-1, 1):
        for s in (-1, 1):
            b.beam(col('timber2'), (0, sy * (OVER_Y - 0.05), RIDGE + 0.28), (s * (EAVE_X + 0.1), sy * (OVER_Y - 0.05), EAVE_Z - 0.12), 0.1, 0.26)


def chimney(b: Builder, rng):
    x, y = CHIM
    z = 3.4
    k = 0
    while z < 8.2:
        hh = 0.42 + rng.random() * 0.12
        c = jit_shade(rng, col(['stone', 'stone2', 'stone3', 'stone_dk'][k % 4]), 0.04)
        b.box(c, (0.78 + rng.random() * 0.05, 0.78 + rng.random() * 0.05, hh), (x, y, z + hh / 2), rot=(0, 0, (rng.random() - 0.5) * 6))
        z += hh; k += 1
    b.box(col('stone_dk'), (0.98, 0.98, 0.12), (x, y, z + 0.06))
    b.cyl(col('stone_dk', shade=0.7), 0.2, 0.18, (x, y, z + 0.21), seg=6)
    return (x, y, z + 0.3)


def woodshed(b: Builder, rng):
    """arka duvara yaslı odunluk"""
    x0, x1, y0, y1 = -2.2, 0.9, D, D + 1.15
    for xx in (x0, x1):
        b.box(col('wood'), (0.14, 0.14, 1.9), (xx, y1 - 0.07, 0.95))
    b.box(col('wood_lt'), (x1 - x0 + 0.5, 1.5, 0.08), ((x0 + x1) / 2, (y0 + y1) / 2 + 0.05, 2.12), rot=(-22, 0, 0))
    # istif
    for row in range(3):
        for k in range(7):
            xx = x0 + 0.3 + k * 0.42 + (row % 2) * 0.2
            if xx > x1 - 0.2:
                continue
            zz = 0.19 + row * 0.34
            b.cyl(col('log_end'), 0.18, 0.95, (xx, (y0 + y1) / 2 - 0.05, zz), seg=5, rot=(90, 0, rng.random() * 40))
    # kütük ve balta sapı
    b.cyl(col('wood'), 0.28, 0.45, (1.8, D + 0.8, 0.225), seg=7)
    b.beam(col('wood_lt'), (1.8, D + 0.8, 0.45), (1.95, D + 0.95, 0.95), 0.05)


def barrel(b: Builder, rng, x, y):
    b.cyl(col('wood'), 0.3, 0.8, (x, y, 0.4), seg=8, r2=0.3)
    b.cyl(col('wood_lt'), 0.33, 0.35, (x, y, 0.4), seg=8)
    for z in (0.12, 0.68):
        b.cyl(col('iron'), 0.315, 0.06, (x, y, z), seg=8)


def build_hi(variant: str, seed: int):
    rng = random.Random(seed)
    b = Builder(seed)
    plaster = 'plaster' if variant == 'a' else 'plaster2'
    plinth(b, rng)
    walls(b, rng, plaster)
    timber(b, rng)
    # ön pencereler (ışık örneği yuvası), çiçeklik
    for s in (-1, 1):
        window(b, rng, (s * FRONT_WX, -D), (0, -1))
        flower_box(b, rng, s * FRONT_WX, -D - 0.02)
    # yan pencereler (kepenkli)
    sh = 'shutter' if variant == 'a' else 'shutter2'
    window(b, rng, (W, 0.0), (1, 0), shutters=sh)
    window(b, rng, (-W, 0.0), (-1, 0), shutters=sh)
    # üst kat ön: küçük ışıksız pencere; arka: bir tane
    window(b, rng, (0, -D - 0.03), (0, -1), w=0.7, h=0.75, z0=2.95)
    window(b, rng, (0, D + 0.03), (0, 1), w=0.7, h=0.75, z0=2.95)
    # alın havalandırması
    window(b, rng, (0, -D - 0.03), (0, -1), w=0.45, h=0.45, z0=6.1)
    door(b, rng)
    roof(b, rng)
    top = None
    if variant == 'a':
        top = chimney(b, rng)
        barrel(b, rng, W + 0.55, D - 0.5)
    else:
        woodshed(b, rng)
        barrel(b, rng, -W - 0.5, -D + 0.6)
    return b, top


def build_lo(variant: str):
    b = Builder(3)
    pc = col('plaster' if variant == 'a' else 'plaster2')
    b.box(col('stone'), (2 * W + 0.24, 2 * D + 0.24, PL), (0, 0, PL / 2))
    b.box(pc, (2 * W + 0.12, 2 * D, TOP - PL), (0, 0, (PL + TOP) / 2))
    b.prism(pc, [(-W - 0.12, TOP), (W + 0.12, TOP), (0, RIDGE - 0.12)], -D, D)
    b.box(col('timber2'), (2 * W + 0.36, 2 * D + 0.06, 0.2), (0, 0, FL + 0.05))
    ang = math.atan2(RIDGE - EAVE_Z, EAVE_X)
    L = math.hypot(EAVE_X, RIDGE - EAVE_Z) + 0.12
    for s in (-1, 1):
        px = s * (L / 2) * math.cos(ang)
        pz = RIDGE + 0.1 - (L / 2) * math.sin(ang)
        b.box(col(None, mask=1, shade=0.9), (L, 2 * OVER_Y, 0.2), (px + s * math.sin(ang) * 0.1, 0, pz + math.cos(ang) * 0.1), rot=(0, s * math.degrees(ang), 0))
    if variant == 'a':
        b.box(col('stone2'), (0.8, 0.8, 5.0), (CHIM[0], CHIM[1], 3.4 + 2.5))
    return b


def main(outdir=None):
    reset_scene()
    outdir = outdir or HERE
    objs = {}
    tops = {}
    for i, v in enumerate(('a', 'b')):
        b, top = build_hi(v, 11 + i)
        ob = b.to_object(f'human_house_{v}_hi', location=(i * 9, 0, 0))
        objs[ob.name] = ob; tops[v] = top
        lo = build_lo(v).to_object(f'human_house_{v}', location=(i * 9, 10, 0))
        objs[lo.name] = lo
    for n, o in objs.items():
        print(f'{n}: {tri_count(o)} üçgen')
    print('baca ağzı (m):', tops['a'], '→ norm', tuple(round(c / n, 3) for c, n in zip(tops['a'], NORM)))
    glb = os.path.normpath(os.path.join(HERE, '..', 'models', 'human_house.glb'))
    os.makedirs(os.path.dirname(glb), exist_ok=True)
    export_glb(objs, glb, NORM)
    print('glb →', glb, os.path.getsize(glb), 'bayt')
    bpy.ops.wm.save_as_mainfile(filepath=os.path.join(HERE, 'human_house.blend'))
    if '--render' in sys.argv:
        setup_preview(target=(4.5, 3, 3.2), dist=26, elev=22, azim=-32)
        render(os.path.join(outdir, 'human_house_preview.png'))
        # yakın: A evi önden
        cam = bpy.context.scene.camera
        from mathutils import Vector
        t = Vector((0, 0, 3.4)); cam.location = Vector((7.5, -11.5, 5.2))
        cam.rotation_euler = (t - cam.location).to_track_quat('-Z', 'Y').to_euler()
        render(os.path.join(outdir, 'human_house_a_close.png'))
        for o in list(bpy.data.objects):
            if o.name in ('human_house_a', 'human_house_b'): o.hide_render = True
        t = Vector((9, 0, 3.4)); cam.location = Vector((17.5, 12.5, 6.0))
        cam.rotation_euler = (t - cam.location).to_track_quat('-Z', 'Y').to_euler()
        render(os.path.join(outdir, 'human_house_b_back.png'))


if __name__ == '__main__':
    args = [a for a in sys.argv[1:] if not a.startswith('--')]
    main(args[-1] if args and os.path.isdir(args[-1]) else None)
