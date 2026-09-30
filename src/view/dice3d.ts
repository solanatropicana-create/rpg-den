// Zar tepsisi: three.js ile gerçek çokyüzlü zarlar (d4, d6, d8, d10, d12, d20).
// Zar yuvarlanır, sekip durur ve sonuç yüzü kameraya döner. Sayılar yüz yüz dokuya çizilir.
import * as THREE from 'three';
import { RoomEnvironment } from 'three/examples/jsm/environments/RoomEnvironment.js';

export interface DieSpec {
  sides: number;
  value: number;
  color: string;          // gövde rengi
  ink?: string;           // rakam rengi
  glow?: 'crit' | 'fumble' | 'hit' | 'miss' | 'heal' | 'fire';
}
export type Zone = 'hit' | 'dmg' | 'center';

interface FaceDef { verts: THREE.Vector3[]; c: THREE.Vector3; n: THREE.Vector3; up: THREE.Vector3; R: number }
interface DieShape { faces: FaceDef[]; scale: number }

const SHAPES = new Map<number, DieShape>();

/** üçgenleri normale göre gruplayıp çokgen yüzleri çıkarır */
function facesOf(g: THREE.BufferGeometry): THREE.Vector3[][] {
  const ng = g.index ? g.toNonIndexed() : g;
  const p = ng.attributes.position;
  const groups = new Map<string, THREE.Vector3[]>();
  const a = new THREE.Vector3(), b = new THREE.Vector3(), c = new THREE.Vector3();
  for (let i = 0; i < p.count; i += 3) {
    a.fromBufferAttribute(p, i); b.fromBufferAttribute(p, i + 1); c.fromBufferAttribute(p, i + 2);
    const n = new THREE.Vector3().subVectors(b, a).cross(new THREE.Vector3().subVectors(c, a)).normalize();
    const key = `${n.x.toFixed(2)},${n.y.toFixed(2)},${n.z.toFixed(2)}`;
    let arr = groups.get(key);
    if (!arr) { arr = []; groups.set(key, arr); }
    for (const v of [a, b, c]) if (!arr.some((q) => q.distanceTo(v) < 1e-4)) arr.push(v.clone());
  }
  return [...groups.values()];
}

/** d10: beşgen yamuk yüzlü (trapezohedron); yüzlerin düzlemsel olması için halka yüksekliği sayısal bulunur */
function d10Polys(): THREE.Vector3[][] {
  const T = new THREE.Vector3(0, 1, 0), B = new THREE.Vector3(0, -1, 0);
  const ring = (h: number, off: number) => [0, 1, 2, 3, 4].map((k) => new THREE.Vector3(Math.cos((k + off) * Math.PI * 2 / 5) * 0.95, h, Math.sin((k + off) * Math.PI * 2 / 5) * 0.95));
  const planar = (h: number) => {
    const U = ring(h, 0), L = ring(-h, 0.5);
    const n = new THREE.Vector3().subVectors(U[0], T).cross(new THREE.Vector3().subVectors(U[1], T)).normalize();
    return n.dot(new THREE.Vector3().subVectors(L[0], T));
  };
  let lo = 0.01, hi = 0.9;
  for (let i = 0; i < 50; i++) { const m = (lo + hi) / 2; if (Math.sign(planar(m)) === Math.sign(planar(lo))) lo = m; else hi = m; }
  const h = (lo + hi) / 2;
  const U = ring(h, 0), L = ring(-h, 0.5);
  const polys: THREE.Vector3[][] = [];
  for (let k = 0; k < 5; k++) polys.push([T.clone(), U[k].clone(), L[k].clone(), U[(k + 1) % 5].clone()]);
  for (let k = 0; k < 5; k++) polys.push([B.clone(), L[k].clone(), U[(k + 1) % 5].clone(), L[(k + 1) % 5].clone()]);
  return polys;
}

function shape(sides: number): DieShape {
  const hit = SHAPES.get(sides);
  if (hit) return hit;
  let polys: THREE.Vector3[][];
  let scale = 1;
  switch (sides) {
    case 4: polys = facesOf(new THREE.TetrahedronGeometry(1)); scale = 1.15; break;
    case 6: polys = facesOf(new THREE.BoxGeometry(1.15, 1.15, 1.15)); scale = 0.9; break;
    case 8: polys = facesOf(new THREE.OctahedronGeometry(1)); scale = 1.02; break;
    case 10: polys = d10Polys(); scale = 1.0; break;
    case 12: polys = facesOf(new THREE.DodecahedronGeometry(1)); scale = 1.0; break;
    default: polys = facesOf(new THREE.IcosahedronGeometry(1)); scale = 1.06; sides = 20; break;
  }
  // yüzleri sırala (kararlı numaralama) ve dışa bakan sarımı, merkezi, "yukarı" yönünü hesapla
  const faces: FaceDef[] = polys.map((vs, fi) => {
    const c = vs.reduce((a, v) => a.add(v), new THREE.Vector3()).multiplyScalar(1 / vs.length);
    let n = c.clone().normalize();
    // köşeleri yüz düzleminde açıya göre sırala (d10 kendi sırasını korur)
    if (sides !== 10) {
      const e1 = new THREE.Vector3().subVectors(vs[0], c).normalize();
      const e2 = new THREE.Vector3().crossVectors(n, e1);
      vs.sort((p, q) => Math.atan2(new THREE.Vector3().subVectors(p, c).dot(e2), new THREE.Vector3().subVectors(p, c).dot(e1)) - Math.atan2(new THREE.Vector3().subVectors(q, c).dot(e2), new THREE.Vector3().subVectors(q, c).dot(e1)));
    }
    const nn = new THREE.Vector3().subVectors(vs[1], vs[0]).cross(new THREE.Vector3().subVectors(vs[2], vs[0])).normalize();
    if (nn.dot(c) < 0) vs.reverse();
    n = new THREE.Vector3().subVectors(vs[1], vs[0]).cross(new THREE.Vector3().subVectors(vs[2], vs[0])).normalize();
    // rakamın "yukarısı": üçgen/beşgende bir köşeye, karede kenar ortasına, d10'da tepeye doğru
    let upPt: THREE.Vector3;
    if (sides === 6) upPt = vs[0].clone().add(vs[1]).multiplyScalar(0.5);
    else if (sides === 10) upPt = (fi < 5 ? vs.find((v) => v.y > 0.99) : vs.find((v) => v.y < -0.99)) ?? vs[0];
    else upPt = vs[0];
    const up = new THREE.Vector3().subVectors(upPt, c);
    up.sub(n.clone().multiplyScalar(up.dot(n))).normalize();
    const R = Math.max(...vs.map((v) => v.distanceTo(c)));
    return { verts: vs, c, n, up, R };
  });
  const out = { faces, scale };
  SHAPES.set(sides, out);
  return out;
}

const ATLAS = new Map<string, THREE.CanvasTexture>();
const CELL = 192;
function shade(hex: string, k: number) {
  const c = new THREE.Color(hex);
  const hsl = { h: 0, s: 0, l: 0 };
  c.getHSL(hsl);
  c.setHSL(hsl.h, Math.min(1, hsl.s * 1.05), Math.max(0, Math.min(1, hsl.l * k)));
  return `#${c.getHexString()}`;
}
/** yüz yüz rakamlı doku. Her yüzün çokgeni kendi hücresine izdüşürülür */
function atlas(sides: number, color: string, ink: string): THREE.CanvasTexture {
  const key = `${sides}|${color}|${ink}`;
  const hit = ATLAS.get(key);
  if (hit) return hit;
  const sh = shape(sides);
  const n = sh.faces.length;
  const cols = Math.ceil(Math.sqrt(n)), rows = Math.ceil(n / cols);
  const cv = document.createElement('canvas');
  cv.width = cols * CELL; cv.height = rows * CELL;
  const g = cv.getContext('2d')!;
  g.fillStyle = color; g.fillRect(0, 0, cv.width, cv.height);
  sh.faces.forEach((f, i) => {
    const col = i % cols, row = Math.floor(i / cols);
    const cx = (col + 0.5) * CELL, cy = (row + 0.5) * CELL;
    const r = new THREE.Vector3().crossVectors(f.up, f.n);
    const pts = f.verts.map((v) => { const d = new THREE.Vector3().subVectors(v, f.c); return [cx + (d.dot(r) / f.R) * 0.47 * CELL, cy - (d.dot(f.up) / f.R) * 0.47 * CELL] as [number, number]; });
    // gövde: merkezden kenara hafif koyulaşan reçine
    const grd = g.createRadialGradient(cx, cy - CELL * 0.08, CELL * 0.05, cx, cy, CELL * 0.5);
    grd.addColorStop(0, shade(color, 1.28)); grd.addColorStop(0.6, color); grd.addColorStop(1, shade(color, 0.72));
    g.beginPath(); pts.forEach(([x, y], k) => (k ? g.lineTo(x, y) : g.moveTo(x, y))); g.closePath();
    g.fillStyle = grd; g.fill();
    // pah: kenarda ince açık çizgi (ışık yakalar) ve içte koyu çizgi
    g.lineJoin = 'round';
    g.strokeStyle = shade(color, 1.55); g.lineWidth = 5; g.stroke();
    g.strokeStyle = shade(color, 0.55); g.lineWidth = 1.5; g.stroke();
    const val = i + 1;
    // yazının oturacağı iç yarıçap
    const inr = sides === 6 ? 0.66 : sides === 12 ? 0.74 : sides === 10 ? 0.5 : 0.46;
    if (sides === 6) {
      // d6: nokta (pip)
      const P: Record<number, [number, number][]> = { 1: [[0, 0]], 2: [[-1, -1], [1, 1]], 3: [[-1, -1], [0, 0], [1, 1]], 4: [[-1, -1], [1, -1], [-1, 1], [1, 1]], 5: [[-1, -1], [1, -1], [0, 0], [-1, 1], [1, 1]], 6: [[-1, -1], [1, -1], [-1, 0], [1, 0], [-1, 1], [1, 1]] };
      const s = CELL * 0.17;
      for (const [px, py] of P[val]) {
        g.beginPath(); g.arc(cx + px * s, cy + py * s, CELL * (val === 1 ? 0.085 : 0.058), 0, Math.PI * 2);
        g.fillStyle = 'rgba(0,0,0,.45)'; g.fill();
        g.beginPath(); g.arc(cx + px * s, cy + py * s - 1.5, CELL * (val === 1 ? 0.08 : 0.052), 0, Math.PI * 2);
        g.fillStyle = ink; g.fill();
      }
      return;
    }
    const fs = Math.round(CELL * 0.47 * inr * (val >= 10 ? 1.05 : 1.3));
    const ty = cy + (sides === 10 ? CELL * 0.05 : sides === 4 ? CELL * 0.03 : 0);
    g.font = `700 ${fs}px "Alegreya SC", Georgia, "Times New Roman", serif`;
    g.textAlign = 'center'; g.textBaseline = 'middle';
    // oyma etkisi: altta koyu gölge, üstte rakam
    g.fillStyle = 'rgba(0,0,0,.5)'; g.fillText(String(val), cx + 1.5, ty + 2.5);
    g.fillStyle = ink; g.fillText(String(val), cx, ty);
    if ((val === 6 || val === 9) && sides >= 8) { g.fillRect(cx - fs * 0.28, ty + fs * 0.42, fs * 0.56, Math.max(2, fs * 0.07)); }
  });
  const tex = new THREE.CanvasTexture(cv);
  tex.colorSpace = THREE.SRGBColorSpace;
  tex.anisotropy = 4;
  ATLAS.set(key, tex);
  return tex;
}

const GEOS = new Map<number, THREE.BufferGeometry>();
function dieGeometry(sides: number): THREE.BufferGeometry {
  const hit = GEOS.get(sides);
  if (hit) return hit;
  const sh = shape(sides);
  const n = sh.faces.length;
  const cols = Math.ceil(Math.sqrt(n)), rows = Math.ceil(n / cols);
  const pos: number[] = [], nor: number[] = [], uv: number[] = [];
  sh.faces.forEach((f, i) => {
    const col = i % cols, row = Math.floor(i / cols);
    const r = new THREE.Vector3().crossVectors(f.up, f.n);
    const uvOf = (v: THREE.Vector3) => {
      const d = new THREE.Vector3().subVectors(v, f.c);
      const x = d.dot(r) / f.R, y = d.dot(f.up) / f.R;
      return [(col + 0.5 + 0.47 * x) / cols, 1 - (row + 0.5 - 0.47 * y) / rows];
    };
    for (let k = 1; k < f.verts.length - 1; k++) {
      for (const v of [f.verts[0], f.verts[k], f.verts[k + 1]]) {
        pos.push(v.x * sh.scale, v.y * sh.scale, v.z * sh.scale);
        nor.push(f.n.x, f.n.y, f.n.z);
        uv.push(...uvOf(v));
      }
    }
  });
  const g = new THREE.BufferGeometry();
  g.setAttribute('position', new THREE.Float32BufferAttribute(pos, 3));
  g.setAttribute('normal', new THREE.Float32BufferAttribute(nor, 3));
  g.setAttribute('uv', new THREE.Float32BufferAttribute(uv, 2));
  GEOS.set(sides, g);
  return g;
}

interface Rolling {
  mesh: THREE.Mesh; spec: DieSpec; t0: number; dur: number;
  from: THREE.Vector3; to: THREE.Vector3; q0: THREE.Quaternion; qT: THREE.Quaternion; axis: THREE.Vector3; spin: number;
  size: number; bounced: number; done: boolean; ring?: THREE.Mesh; ringT0?: number; out?: number;
}

const GLOW: Record<NonNullable<DieSpec['glow']>, number> = { crit: 0xf2cf5b, fumble: 0xff5a43, hit: 0xf4f1e6, miss: 0x6b7280, heal: 0x7ee08f, fire: 0xff8a3a };

export class DiceTray {
  private renderer: THREE.WebGLRenderer;
  private scene = new THREE.Scene();
  private camera = new THREE.PerspectiveCamera(30, 1, 0.1, 100);
  private dice: Rolling[] = [];
  private sparks: { pts: THREE.Points; v: Float32Array; t0: number }[] = [];
  private raf = 0;
  private alive = true;
  private waiters: { until: number; res: () => void }[] = [];
  private reduce = false;
  speed = 1;
  onBounce: ((strength: number) => void) | null = null;

  constructor(private canvas: HTMLCanvasElement) {
    this.renderer = new THREE.WebGLRenderer({ canvas, antialias: true, alpha: true, powerPreference: 'low-power' });
    this.renderer.setPixelRatio(Math.min(2, window.devicePixelRatio || 1));
    this.renderer.outputColorSpace = THREE.SRGBColorSpace;
    this.renderer.toneMapping = THREE.ACESFilmicToneMapping;
    this.renderer.toneMappingExposure = 1.05;
    this.renderer.shadowMap.enabled = true;
    this.renderer.shadowMap.type = THREE.PCFSoftShadowMap;
    try { this.reduce = matchMedia('(prefers-reduced-motion: reduce)').matches; } catch { /* yok */ }
    const pm = new THREE.PMREMGenerator(this.renderer);
    this.scene.environment = pm.fromScene(new RoomEnvironment(), 0.04).texture;
    (this.scene as unknown as { environmentIntensity: number }).environmentIntensity = 0.55;
    pm.dispose();
    this.camera.position.set(0, 10.5, 6.2);
    this.camera.lookAt(0, 0, 0.25);
    // keçe tepsi
    const fc = document.createElement('canvas'); fc.width = fc.height = 512;
    const g = fc.getContext('2d')!;
    const grd = g.createRadialGradient(256, 236, 30, 256, 256, 300);
    grd.addColorStop(0, '#2f4a3c'); grd.addColorStop(0.65, '#1b2b23'); grd.addColorStop(1, '#0c1310');
    g.fillStyle = grd; g.fillRect(0, 0, 512, 512);
    const img = g.getImageData(0, 0, 512, 512);
    for (let i = 0; i < img.data.length; i += 4) { const k = (Math.random() - 0.5) * 14; img.data[i] += k; img.data[i + 1] += k; img.data[i + 2] += k; }
    g.putImageData(img, 0, 0);
    const felt = new THREE.CanvasTexture(fc); felt.colorSpace = THREE.SRGBColorSpace;
    const floor = new THREE.Mesh(new THREE.PlaneGeometry(16, 11), new THREE.MeshStandardMaterial({ map: felt, roughness: 0.95, metalness: 0 }));
    floor.rotation.x = -Math.PI / 2; floor.receiveShadow = true;
    this.scene.add(floor);
    this.scene.add(new THREE.HemisphereLight(0xfff1dc, 0x10160f, 0.55));
    const key = new THREE.DirectionalLight(0xffe2b8, 2.1);
    key.position.set(-4, 9, 3); key.castShadow = true;
    key.shadow.mapSize.set(1024, 1024);
    const sc = key.shadow.camera as THREE.OrthographicCamera; sc.left = -7; sc.right = 7; sc.top = 6; sc.bottom = -6; sc.near = 1; sc.far = 25;
    key.shadow.radius = 5; key.shadow.bias = -0.0015;
    this.scene.add(key);
    const rim = new THREE.DirectionalLight(0x9fc6ff, 0.9); rim.position.set(5, 4, -6); this.scene.add(rim);
    this.resize();
    const loop = () => { if (!this.alive) return; this.raf = requestAnimationFrame(loop); this.frame(performance.now()); };
    this.raf = requestAnimationFrame(loop);
  }

  resize() {
    const r = this.canvas.getBoundingClientRect();
    const w = Math.max(10, Math.round(r.width)), h = Math.max(10, Math.round(r.height));
    this.renderer.setSize(w, h, false);
    this.camera.aspect = w / h;
    // dar ekranda tepsiyi sığdırmak için geri çekil
    const fit = Math.max(1, 1.55 / this.camera.aspect);
    this.camera.position.set(0, 10.5 * fit, 6.2 * fit);
    this.camera.lookAt(0, 0, 0.25);
    this.camera.updateProjectionMatrix();
  }

  /** zarları kaldır (hafifçe küçülüp kaybolur) */
  clear(instant = false) {
    const now = performance.now();
    for (const d of this.dice) { if (instant) this.drop(d); else d.out = now; }
    if (instant) this.dice = [];
  }
  private drop(d: Rolling) {
    this.scene.remove(d.mesh);
    (d.mesh.material as THREE.Material).dispose();
    if (d.ring) { this.scene.remove(d.ring); (d.ring.material as THREE.Material).dispose(); d.ring.geometry.dispose(); }
  }

  /** zar at: hepsi durunca çözülür */
  roll(specs: DieSpec[], zone: Zone, fromSide: 'left' | 'right' = 'left'): Promise<void> {
    const now = performance.now();
    const n = specs.length;
    const small = n > 4 ? (n > 8 ? 0.55 : 0.72) : 1;
    // hedef konumlar
    const spots: THREE.Vector3[] = [];
    const baseX = zone === 'hit' ? -2.7 : zone === 'center' ? 0 : 1.35;
    const perRow = n <= 4 ? n : Math.ceil(n / 2);
    const gap = 1.75 * small;
    for (let i = 0; i < n; i++) {
      const row = Math.floor(i / perRow), k = i % perRow;
      const cnt = Math.min(perRow, n - row * perRow);
      spots.push(new THREE.Vector3(baseX + (k - (cnt - 1) / 2) * gap, 0, (n > perRow ? (row === 0 ? -0.85 : 0.95) : 0.1)));
    }
    let longest = 0;
    specs.forEach((spec, i) => {
      const sh = shape(spec.sides);
      const mat = new THREE.MeshPhysicalMaterial({ map: atlas(spec.sides, spec.color, spec.ink ?? '#f6ecd0'), roughness: 0.34, metalness: 0.02, clearcoat: 1, clearcoatRoughness: 0.14, sheen: 0.2, sheenColor: new THREE.Color(0xffffff) });
      const mesh = new THREE.Mesh(dieGeometry(spec.sides), mat);
      mesh.castShadow = true;
      const size = 0.72 * small;
      mesh.scale.setScalar(size);
      const to = spots[i].clone();
      to.y = size * 1.02;
      to.x += (Math.random() - 0.5) * 0.25 * small; to.z += (Math.random() - 0.5) * 0.25 * small;
      const sx = fromSide === 'left' ? -8.5 : 8.5;
      const from = new THREE.Vector3(sx, 3.2 + Math.random(), -2.5 + Math.random() * 5);
      const axis = new THREE.Vector3(Math.random() - 0.5, Math.random() - 0.5, Math.random() - 0.5).normalize();
      const q0 = new THREE.Quaternion().setFromEuler(new THREE.Euler(Math.random() * 6, Math.random() * 6, Math.random() * 6));
      const qT = this.faceToCamera(spec.sides, Math.min(spec.value, sh.faces.length), to);
      const dur = (this.reduce ? 60 : 1050 + Math.random() * 260 + i * 70) / this.speed;
      longest = Math.max(longest, dur);
      const d: Rolling = { mesh, spec, t0: now + (this.reduce ? 0 : i * 45 / this.speed), dur, from, to, q0, qT, axis, spin: 16 + Math.random() * 8, size, bounced: 0, done: false };
      mesh.position.copy(from); mesh.quaternion.copy(q0);
      this.scene.add(mesh);
      this.dice.push(d);
    });
    return new Promise((res) => this.waiters.push({ until: now + longest + (n - 1) * 45 / this.speed + 40, res }));
  }

  /** sonuç yüzünü kameraya çeviren dönüş */
  private faceToCamera(sides: number, value: number, at: THREE.Vector3): THREE.Quaternion {
    const f = shape(sides).faces[value - 1];
    const D = new THREE.Vector3().subVectors(this.camera.position, at).normalize();
    const camUp = new THREE.Vector3(0, 1, 0).applyQuaternion(this.camera.quaternion);
    const U = camUp.sub(D.clone().multiplyScalar(camUp.dot(D))).normalize();
    const R = new THREE.Vector3().crossVectors(U, D);
    const r = new THREE.Vector3().crossVectors(f.up, f.n);
    const Bw = new THREE.Matrix4().makeBasis(R, U, D);
    const Bl = new THREE.Matrix4().makeBasis(r, f.up, f.n).transpose();
    const q = new THREE.Quaternion().setFromRotationMatrix(Bw.multiply(Bl));
    const twist = new THREE.Quaternion().setFromAxisAngle(D, (Math.random() - 0.5) * 0.35);
    return twist.multiply(q);
  }

  private frame(now: number) {
    for (const d of this.dice) {
      const p = Math.min(1, Math.max(0, (now - d.t0) / d.dur));
      const e = 1 - Math.pow(1 - p, 3);
      d.mesh.position.lerpVectors(d.from, d.to, e);
      // sekme: üç sıçrama, gittikçe alçalan
      const bounce = Math.abs(Math.cos(p * Math.PI * 3.0)) * Math.pow(1 - p, 2) * 2.6;
      d.mesh.position.y = d.to.y + (p < 1 ? bounce : 0);
      const nb = Math.floor(p * 3 + 0.5);
      if (p > 0 && nb > d.bounced && p < 0.95) { d.bounced = nb; this.onBounce?.(Math.pow(1 - p, 1.5)); }
      const ang = d.spin * (1 - Math.pow(1 - p, 2)) * 0.9;
      const qFree = new THREE.Quaternion().setFromAxisAngle(d.axis, ang).multiply(d.q0);
      const w = THREE.MathUtils.smoothstep(p, 0.45, 0.93);
      d.mesh.quaternion.slerpQuaternions(qFree, d.qT, w);
      if (p >= 1 && !d.done) {
        d.done = true;
        d.mesh.position.copy(d.to); d.mesh.quaternion.copy(d.qT);
        this.onBounce?.(0.25);
        if (d.spec.glow) this.pulse(d, now);
      }
      if (d.ring && d.ringT0 !== undefined) {
        const q = (now - d.ringT0) / 900;
        const m = d.ring.material as THREE.MeshBasicMaterial;
        d.ring.scale.setScalar(1 + Math.min(1, q) * 0.9);
        m.opacity = q < 1 ? (1 - q) * 0.85 : (d.spec.glow === 'crit' || d.spec.glow === 'fumble' ? 0.35 + Math.sin(now / 180) * 0.12 : 0);
      }
      if (d.out !== undefined) {
        const q = Math.min(1, (now - d.out) / 260);
        d.mesh.scale.setScalar(d.size * (1 - q * 0.9));
        d.mesh.position.y = d.to.y - q * d.size;
      }
    }
    // kaybolanları at
    this.dice = this.dice.filter((d) => { if (d.out !== undefined && now - d.out > 280) { this.drop(d); return false; } return true; });
    for (let k = this.sparks.length - 1; k >= 0; k--) {
      const s = this.sparks[k];
      const q = (now - s.t0) / 1100;
      if (q > 1) { this.scene.remove(s.pts); s.pts.geometry.dispose(); (s.pts.material as THREE.Material).dispose(); this.sparks.splice(k, 1); continue; }
      const pa = s.pts.geometry.attributes.position as THREE.BufferAttribute;
      for (let i = 0; i < pa.count; i++) {
        pa.setX(i, pa.getX(i) + s.v[i * 3] * 0.016); pa.setY(i, pa.getY(i) + s.v[i * 3 + 1] * 0.016); pa.setZ(i, pa.getZ(i) + s.v[i * 3 + 2] * 0.016);
        s.v[i * 3 + 1] -= 0.09;
      }
      pa.needsUpdate = true;
      (s.pts.material as THREE.PointsMaterial).opacity = 1 - q;
    }
    for (let k = this.waiters.length - 1; k >= 0; k--) if (now >= this.waiters[k].until) { const w = this.waiters[k]; this.waiters.splice(k, 1); w.res(); }
    this.renderer.render(this.scene, this.camera);
  }

  private pulse(d: Rolling, now: number) {
    const col = GLOW[d.spec.glow!];
    const ring = new THREE.Mesh(new THREE.RingGeometry(0.95, 1.12, 48), new THREE.MeshBasicMaterial({ color: col, transparent: true, opacity: 0.85, depthWrite: false, blending: THREE.AdditiveBlending }));
    ring.rotation.x = -Math.PI / 2;
    ring.position.set(d.to.x, 0.02, d.to.z);
    ring.scale.setScalar(1);
    ring.geometry.scale(d.size * 1.1, d.size * 1.1, 1);
    this.scene.add(ring);
    d.ring = ring; d.ringT0 = now;
    if (d.spec.glow === 'crit' || d.spec.glow === 'fire') {
      const N = 60;
      const pos = new Float32Array(N * 3), v = new Float32Array(N * 3);
      for (let i = 0; i < N; i++) {
        pos[i * 3] = d.to.x; pos[i * 3 + 1] = d.to.y; pos[i * 3 + 2] = d.to.z;
        const a = Math.random() * Math.PI * 2, s = 1.5 + Math.random() * 3.5;
        v[i * 3] = Math.cos(a) * s; v[i * 3 + 1] = 3 + Math.random() * 4; v[i * 3 + 2] = Math.sin(a) * s;
      }
      const g = new THREE.BufferGeometry(); g.setAttribute('position', new THREE.BufferAttribute(pos, 3));
      const pts = new THREE.Points(g, new THREE.PointsMaterial({ color: col, size: 0.12, transparent: true, depthWrite: false, blending: THREE.AdditiveBlending }));
      this.scene.add(pts);
      this.sparks.push({ pts, v, t0: now });
    }
  }

  dispose() {
    this.alive = false;
    cancelAnimationFrame(this.raf);
    for (const d of this.dice) this.drop(d);
    this.dice = [];
    for (const w of this.waiters) w.res();
    this.waiters = [];
    this.scene.environment?.dispose();
    this.renderer.dispose();
    this.renderer.forceContextLoss();
  }
}

// ---------------------------------------------------------------- zar sesi (WebAudio, küçük tık)
let actx: AudioContext | null = null;
export function diceClack(strength: number) {
  try {
    actx ??= new AudioContext();
    const ctx = actx;
    if (ctx.state === 'suspended') void ctx.resume();
    const t = ctx.currentTime;
    const len = Math.floor(ctx.sampleRate * 0.05);
    const buf = ctx.createBuffer(1, len, ctx.sampleRate);
    const d = buf.getChannelData(0);
    for (let i = 0; i < len; i++) d[i] = (Math.random() * 2 - 1) * Math.pow(1 - i / len, 6);
    const src = ctx.createBufferSource(); src.buffer = buf;
    const f = ctx.createBiquadFilter(); f.type = 'bandpass'; f.frequency.value = 2200 + Math.random() * 1600; f.Q.value = 3.5;
    const g = ctx.createGain(); g.gain.setValueAtTime(0.28 * Math.min(1, 0.25 + strength), t);
    src.connect(f); f.connect(g); g.connect(ctx.destination);
    src.start(t);
  } catch { /* ses yok */ }
}
