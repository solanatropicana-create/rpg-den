// Ayrıntılı low-poly modeller: ağaçlar, ekin sıraları, çit kazıkları, çatılar, yaban hayatı.
// Her biri tek bir BufferGeometry (birleştirilmiş parçalar) — instanced çizime uygun.
// Yakın plan (LOD "hi") ve uzak plan ("lo") sürümleri aynı dış ölçüyü korur: yerleşim kodu değişmeden değiştirilebilir.
import * as THREE from 'three';
import { mergeGeometries } from 'three/examples/jsm/utils/BufferGeometryUtils.js';

const h3 = (x: number, y: number, z: number, k: number) => { const v = Math.sin(x * 127.1 + y * 311.7 + z * 74.7 + k * 19.19) * 43758.5453; return v - Math.floor(v); };

/** indekssiz yap, konuma bağlı hafif sarsıntı ver (aynı köşe aynı yere gider: dikiş açılmaz) */
function prep(g: THREE.BufferGeometry, jit = 0, seed = 0): THREE.BufferGeometry {
  const n = g.index ? g.toNonIndexed() : g;
  if (jit > 0) {
    const p = n.getAttribute('position') as THREE.BufferAttribute;
    for (let i = 0; i < p.count; i++) {
      const x = p.getX(i), y = p.getY(i), z = p.getZ(i);
      const qx = Math.round(x * 1000) / 1000, qy = Math.round(y * 1000) / 1000, qz = Math.round(z * 1000) / 1000;
      p.setXYZ(i, x + (h3(qx, qy, qz, seed) - 0.5) * jit, y + (h3(qx, qy, qz, seed + 1) - 0.5) * jit * 0.6, z + (h3(qx, qy, qz, seed + 2) - 0.5) * jit);
    }
  }
  if (n.getAttribute('uv')) n.deleteAttribute('uv');
  n.computeVertexNormals();
  return n;
}
function merge(parts: THREE.BufferGeometry[]): THREE.BufferGeometry {
  const g = mergeGeometries(parts.map((p) => (p.index ? p.toNonIndexed() : p)).map((p) => { if (p.getAttribute('uv')) p.deleteAttribute('uv'); return p; }))!;
  g.computeVertexNormals();
  return g;
}

// ---------------------------------------------------------------- ağaçlar
/** köknar: katmanlı koni. Dış ölçü eski tek koniyle aynı (taban yarıçapı 0.3, boy 0.72) */
export function firGeo(hi: boolean): THREE.BufferGeometry {
  const seg = hi ? 8 : 6;
  const tiers: [number, number, number][] = hi ? [[0.3, 0.36, 0], [0.24, 0.32, 0.2], [0.17, 0.28, 0.38], [0.1, 0.18, 0.54]] : [[0.3, 0.44, 0], [0.19, 0.4, 0.32]];
  return merge(tiers.map(([r, h, y], k) => { const c = new THREE.ConeGeometry(r, h, seg, 1, !hi); c.rotateY(k * 0.7); c.translate(0, y + h / 2, 0); return prep(c, hi ? 0.035 : 0, k * 7); }));
}
/** çam (tundra, dağ): daha dar, sivri, sarkık katmanlar. Dış ölçü: yarıçap 0.22, boy 0.85 */
export function pineGeo(hi: boolean): THREE.BufferGeometry {
  const seg = hi ? 7 : 5;
  const tiers: [number, number, number][] = hi ? [[0.22, 0.3, 0.04], [0.18, 0.27, 0.24], [0.14, 0.25, 0.42], [0.09, 0.22, 0.6]] : [[0.22, 0.5, 0.04], [0.13, 0.42, 0.43]];
  return merge(tiers.map(([r, h, y], k) => { const c = new THREE.ConeGeometry(r, h, seg, 1, !hi); c.rotateY(k * 1.1); c.translate(0, y + h / 2, 0); return prep(c, hi ? 0.03 : 0, 40 + k * 7); }));
}
/** yapraklı ağaç tacı: yakında birkaç topak, uzakta tek topak. Dış ölçü eski ikosahedronla (r 0.36, merkez y 0.3) uyumlu */
export function crownGeo(hi: boolean): THREE.BufferGeometry {
  if (!hi) { const g = new THREE.IcosahedronGeometry(0.36, 0); g.translate(0, 0.3, 0); return prep(g); }
  const blobs: [number, number, number, number][] = [[0, 0.3, 0, 0.29], [0.17, 0.22, 0.06, 0.2], [-0.14, 0.24, -0.1, 0.21], [0.03, 0.48, -0.03, 0.19], [-0.05, 0.2, 0.17, 0.17]];
  return merge(blobs.map(([x, y, z, r], k) => { const g = new THREE.IcosahedronGeometry(r, 0); g.rotateY(k * 1.3); g.rotateX(k * 0.7); g.translate(x, y, z); return prep(g, 0.04, 80 + k * 5); }));
}
/** ağaç gövdesi: yakında kök çıkıntısı ve bir dal */
export function trunkGeo(hi: boolean): THREE.BufferGeometry {
  const t = new THREE.CylinderGeometry(0.045, 0.07, 0.25, hi ? 6 : 5, 1, true); t.translate(0, 0.125, 0);
  if (!hi) return prep(t);
  const root = new THREE.ConeGeometry(0.1, 0.06, 6, 1, true); root.translate(0, 0.03, 0);
  const br = new THREE.CylinderGeometry(0.012, 0.022, 0.13, 4, 1, true); br.rotateZ(-0.9); br.translate(0.055, 0.19, 0);
  return merge([prep(t), prep(root), prep(br)]);
}
/** çalı: basık topaklar */
export function bushGeo(hi = true): THREE.BufferGeometry {
  if (!hi) { const g = new THREE.IcosahedronGeometry(0.21, 0); g.scale(1.25, 0.8, 1.25); g.translate(0, 0.15, 0); return prep(g); }
  const blobs: [number, number, number, number][] = [[0, 0.16, 0, 0.2], [0.14, 0.12, 0.05, 0.14], [-0.12, 0.12, -0.06, 0.15], [0.02, 0.12, -0.15, 0.13]];
  return merge(blobs.map(([x, y, z, r], k) => { const g = new THREE.IcosahedronGeometry(r, 0); g.scale(1, 0.8, 1); g.rotateY(k); g.translate(x, y, z); return prep(g, 0.03, 120 + k); }));
}
/** kaya: yakında iki parçalı, çatlaklı */
export function rockGeo(hi: boolean): THREE.BufferGeometry {
  if (!hi) return prep(new THREE.DodecahedronGeometry(0.14, 0));
  const a = new THREE.DodecahedronGeometry(0.14, 0); a.scale(1, 0.85, 1);
  const b = new THREE.DodecahedronGeometry(0.075, 0); b.translate(0.1, -0.04, 0.06);
  return merge([prep(a, 0.035, 150), prep(b, 0.02, 160)]);
}

// ---------------------------------------------------------------- ekin, çit, çatı
/**
 * Ekin sırası: birim kutu ölçüsünde (x,z ∈ [-0.5,0.5], y ∈ [0,1]) — eskiden 'box' ile çizilen sıranın yerini alır.
 * Yakında: toprak sırtı + iki sıra başak/sap; uzakta: tek kutu.
 */
export function cropGeo(hi: boolean): THREE.BufferGeometry {
  if (!hi) { const b = new THREE.BoxGeometry(1, 1, 1); b.translate(0, 0.5, 0); return prep(b); }
  const parts: THREE.BufferGeometry[] = [];
  const ridge = new THREE.BoxGeometry(1.1, 0.16, 0.98); ridge.translate(0, 0.08, 0); parts.push(prep(ridge));
  // saplar sıra boyunca basık (z'de dar): uzak sürümle aynı boyda kalır (±0.5)
  for (let row = 0; row < 2; row++) for (let k = 0; k < 7; k++) {
    const c = new THREE.ConeGeometry(0.28, 1, 4, 1, true);
    c.scale(1, 1, 0.45);
    c.translate((row - 0.5) * 0.5, 0.5, -0.37 + k * 0.118 + row * 0.03);
    parts.push(prep(c, 0.04, row * 20 + k));
  }
  return merge(parts);
}
/** sivri çit kazığı (eski 'post' ölçüsü: yarıçap 0.03, boy 1) */
export function stakeGeo(): THREE.BufferGeometry {
  const c = new THREE.CylinderGeometry(0.03, 0.033, 0.84, 6); c.translate(0, 0.42, 0);
  const t = new THREE.ConeGeometry(0.03, 0.16, 6); t.translate(0, 0.92, 0);
  return merge([prep(c), prep(t)]);
}
/**
 * Beşik çatı kaplaması: 'gable' prizmasıyla aynı birimde (taban genişliği 0.866, mahya 0.75, boy 1),
 * iki eğimli levha saçak payıyla taşar, üstte mahya kirişi. Prizma duvar rengine boyanır, bu çatı rengine.
 */
export function roofSlabGeo(): THREE.BufferGeometry {
  const parts: THREE.BufferGeometry[] = [];
  const ax = 0, ay = 0.8, ex = 0.54, ey = -0.08; // mahya → saçak
  const len = Math.hypot(ex - ax, ey - ay), ang = Math.atan2(ay - ey, ex - ax);
  for (const s of [-1, 1]) {
    // aynalama ölçekle değil dönmeyle: eksi ölçek yüzleri ters çevirip görünmez yapardı
    const b = new THREE.BoxGeometry(len, 0.07, 1.14);
    b.translate((s * len) / 2, 0, 0);
    b.rotateZ(-s * ang);
    b.translate(ax, ay, 0);
    parts.push(prep(b));
  }
  const ridge = new THREE.BoxGeometry(0.1, 0.08, 1.2); ridge.translate(0, 0.82, 0); parts.push(prep(ridge));
  return merge(parts);
}

// ---------------------------------------------------------------- yaban hayatı
/** geyik: gövde, boyun, baş, dört bacak, kuyruk; boy ~0.3 (kasabalıyla orantılı) */
export function deerGeo(antlers: boolean): THREE.BufferGeometry {
  const parts: THREE.BufferGeometry[] = [];
  const body = new THREE.BoxGeometry(0.1, 0.1, 0.24); body.translate(0, 0.2, 0); parts.push(prep(body, 0.02, 1));
  const neck = new THREE.BoxGeometry(0.05, 0.12, 0.05); neck.rotateX(-0.45); neck.translate(0, 0.28, 0.11); parts.push(prep(neck));
  const head = new THREE.BoxGeometry(0.055, 0.055, 0.1); head.translate(0, 0.33, 0.16); parts.push(prep(head, 0.01, 3));
  for (const [x, z] of [[-0.035, 0.09], [0.035, 0.09], [-0.035, -0.09], [0.035, -0.09]]) { const l = new THREE.BoxGeometry(0.022, 0.16, 0.022); l.translate(x, 0.08, z); parts.push(prep(l)); }
  const tail = new THREE.BoxGeometry(0.03, 0.04, 0.03); tail.translate(0, 0.23, -0.125); parts.push(prep(tail));
  for (const e of [-1, 1]) { const ear = new THREE.ConeGeometry(0.012, 0.04, 3); ear.rotateZ(e * 0.9); ear.translate(e * 0.03, 0.365, 0.13); parts.push(prep(ear)); }
  if (antlers) for (const e of [-1, 1]) {
    const a = new THREE.CylinderGeometry(0.005, 0.008, 0.09, 3); a.rotateZ(e * 0.5); a.translate(e * 0.03, 0.4, 0.15); parts.push(prep(a));
    const b = new THREE.CylinderGeometry(0.004, 0.006, 0.05, 3); b.rotateX(0.7); b.translate(e * 0.045, 0.42, 0.17); parts.push(prep(b));
  }
  return merge(parts);
}
/** tavşan: top gövde, kafa, uzun kulaklar */
export function rabbitGeo(): THREE.BufferGeometry {
  const parts: THREE.BufferGeometry[] = [];
  const b = new THREE.IcosahedronGeometry(0.04, 0); b.scale(1, 0.85, 1.25); b.translate(0, 0.04, 0); parts.push(prep(b));
  const h = new THREE.IcosahedronGeometry(0.026, 0); h.translate(0, 0.07, 0.04); parts.push(prep(h));
  for (const e of [-1, 1]) { const ear = new THREE.BoxGeometry(0.01, 0.045, 0.006); ear.rotateZ(e * 0.18); ear.translate(e * 0.012, 0.105, 0.035); parts.push(prep(ear)); }
  return merge(parts);
}
/** koyun: yün gövde (topak), kara baş ve bacaklar ayrı anahtarlarda */
export function sheepGeo(): THREE.BufferGeometry {
  const parts: THREE.BufferGeometry[] = [];
  for (const [x, y, z, r] of [[0, 0.1, 0, 0.075], [0, 0.11, 0.05, 0.06], [0, 0.11, -0.05, 0.062], [0.03, 0.12, 0, 0.05], [-0.03, 0.12, 0, 0.05]] as [number, number, number, number][]) { const g = new THREE.IcosahedronGeometry(r, 0); g.translate(x, y, z); parts.push(prep(g, 0.012, x * 100 + z * 10)); }
  return merge(parts);
}
/** balık (sıçrayan) */
export function fishGeo(): THREE.BufferGeometry {
  const b = new THREE.ConeGeometry(0.018, 0.09, 4); b.rotateX(Math.PI / 2);
  const t = new THREE.ConeGeometry(0.02, 0.03, 3); t.rotateX(-Math.PI / 2); t.translate(0, 0, -0.055);
  return merge([prep(b), prep(t)]);
}
/** kelebek: iki kanat (çift yüzlü düzlemler) */
export function butterflyGeo(): THREE.BufferGeometry {
  const w = new THREE.BufferGeometry();
  w.setAttribute('position', new THREE.Float32BufferAttribute([0, 0, 0.012, 0.04, 0.005, 0.02, 0.035, 0, -0.02, 0, 0, 0.012, 0.035, 0, -0.02, 0, 0, -0.012, 0, 0, 0.012, -0.035, 0, -0.02, -0.04, 0.005, 0.02, 0, 0, 0.012, 0, 0, -0.012, -0.035, 0, -0.02], 3));
  w.computeVertexNormals();
  return w;
}

/** kıyı köpüğü: kenarları yumuşak, kalınlıksız şerit (eskiden göz hizasında beyaz tahta gibi duruyordu) */
export function foamTexture(): THREE.Texture | null {
  if (typeof document === 'undefined') return null;
  const c = document.createElement('canvas'); c.width = 64; c.height = 16;
  const g = c.getContext('2d'); if (!g) return null;
  const gr = g.createLinearGradient(0, 0, 0, 16);
  gr.addColorStop(0, 'rgba(255,255,255,0)'); gr.addColorStop(0.45, 'rgba(255,255,255,0.95)'); gr.addColorStop(0.6, 'rgba(255,255,255,0.8)'); gr.addColorStop(1, 'rgba(255,255,255,0)');
  g.fillStyle = gr; g.fillRect(0, 0, 64, 16);
  // uçlar da sönsün
  const gx = g.createLinearGradient(0, 0, 64, 0);
  gx.addColorStop(0, 'rgba(0,0,0,1)'); gx.addColorStop(0.15, 'rgba(0,0,0,0)'); gx.addColorStop(0.85, 'rgba(0,0,0,0)'); gx.addColorStop(1, 'rgba(0,0,0,1)');
  g.globalCompositeOperation = 'destination-out'; g.fillStyle = gx; g.fillRect(0, 0, 64, 16);
  const t = new THREE.CanvasTexture(c); t.colorSpace = THREE.SRGBColorSpace;
  return t;
}
export function foamGeo(): THREE.BufferGeometry {
  const p = new THREE.PlaneGeometry(1, 0.12); p.rotateX(-Math.PI / 2);
  return p;
}
