// Kesintisiz yükseklik alanı: hex ızgarasıyla hizalı üçgen kafes.
// Hex köşeleri ve merkezleri kenar uzunluğu 1 olan bir üçgen kafes oluşturur; bu kafes N'e bölünür (aralık 1/N).
// Böylece hex kenarları kafes çizgilerinin üstüne düşer: ızgara çizgileri zemine tam oturur.
import * as THREE from 'three';

const SQ3 = Math.sqrt(3);

/** tohumlu 2B simpleks gürültü, [-1, 1] */
export class Noise2 {
  private perm = new Uint8Array(512);
  constructor(seed: number) {
    const p = new Uint8Array(256);
    for (let i = 0; i < 256; i++) p[i] = i;
    let s = (seed * 2654435761) >>> 0 || 1;
    for (let i = 255; i > 0; i--) {
      s ^= s << 13; s >>>= 0; s ^= s >>> 17; s ^= s << 5; s >>>= 0;
      const j = s % (i + 1);
      const t = p[i]; p[i] = p[j]; p[j] = t;
    }
    for (let i = 0; i < 512; i++) this.perm[i] = p[i & 255];
  }
  n(x: number, y: number): number {
    const F2 = 0.5 * (SQ3 - 1), G2 = (3 - SQ3) / 6, P = this.perm;
    const s = (x + y) * F2, i = Math.floor(x + s), j = Math.floor(y + s);
    const t = (i + j) * G2, x0 = x - (i - t), y0 = y - (j - t);
    const i1 = x0 > y0 ? 1 : 0, j1 = 1 - i1;
    const x1 = x0 - i1 + G2, y1 = y0 - j1 + G2, x2 = x0 - 1 + 2 * G2, y2 = y0 - 1 + 2 * G2;
    const ii = i & 255, jj = j & 255;
    const g = (h: number, px: number, py: number) => { const a = (h & 7) * 0.785398; return Math.cos(a) * px + Math.sin(a) * py; };
    let n = 0, q = 0.5 - x0 * x0 - y0 * y0;
    if (q > 0) { q *= q; n += q * q * g(P[ii + P[jj]], x0, y0); }
    q = 0.5 - x1 * x1 - y1 * y1;
    if (q > 0) { q *= q; n += q * q * g(P[ii + i1 + P[jj + j1]], x1, y1); }
    q = 0.5 - x2 * x2 - y2 * y2;
    if (q > 0) { q *= q; n += q * q * g(P[ii + 1 + P[jj + 1]], x2, y2); }
    return n * 99;
  }
  /** katmanlı gürültü, yaklaşık [-1, 1] */
  fbm(x: number, y: number, oct = 4): number {
    let a = 1, f = 1, sum = 0, norm = 0;
    for (let o = 0; o < oct; o++) { sum += this.n(x * f + o * 17.3, y * f - o * 9.1) * a; norm += a; a *= 0.5; f *= 2.03; }
    return sum / norm;
  }
  /** sırtlı gürültü: keskin dağ sırtları, [0, 1] */
  ridged(x: number, y: number, oct = 5): number {
    let a = 1, f = 1, sum = 0, norm = 0, w = 1;
    for (let o = 0; o < oct; o++) {
      let v = 1 - Math.abs(this.n(x * f + o * 31.7, y * f + o * 12.9));
      v *= v; v *= w; w = Math.min(1, v * 1.6);
      sum += v * a; norm += a; a *= 0.52; f *= 2.1;
    }
    return sum / norm;
  }
}

export interface HeightInput {
  /** harita sınırları (dünya birimi) */
  minX: number; maxX: number; minZ: number; maxZ: number;
  /** kafes köşesinin yüksekliği */
  height: (x: number, z: number) => number;
  /** sınır dışı yükseklik (deniz dibi) */
  outside: number;
}

/** kafes üstünde yükseklik alanı + ızgara dokusu */
export class Heightfield {
  readonly N: number;
  readonly dx: number;
  readonly dz: number;
  iMin = 0; iMax = 0; kMin = 0; kMax = 0; nK = 0; nI = 0;
  H = new Float32Array(0);
  outside = -1;
  constructor(N = 3) { this.N = N; this.dx = SQ3 / (2 * N); this.dz = 1 / N; }

  build(inp: HeightInput) {
    this.outside = inp.outside;
    // sütun indeksleri çift sayıdan başlasın ki (i & 1) eşliği dünya konumuyla tutarlı kalsın
    this.iMin = Math.floor(inp.minX / this.dx / 2) * 2;
    this.iMax = Math.ceil(inp.maxX / this.dx);
    this.kMin = Math.floor(inp.minZ / this.dz) - 1;
    this.kMax = Math.ceil(inp.maxZ / this.dz) + 1;
    this.nI = this.iMax - this.iMin + 1; this.nK = this.kMax - this.kMin + 1;
    this.H = new Float32Array(this.nI * this.nK);
    for (let a = 0; a < this.nI; a++) {
      const i = a + this.iMin, x = i * this.dx, off = (i & 1) * 0.5;
      for (let b = 0; b < this.nK; b++) {
        const z = (b + this.kMin + off) * this.dz;
        this.H[a * this.nK + b] = inp.height(x, z);
      }
    }
  }
  /** kafes köşesi (i, k) dünya konumu */
  vx(i: number) { return i * this.dx; }
  vz(i: number, k: number) { return (k + (i & 1) * 0.5) * this.dz; }
  /** kafes koordinatı (i, j) → yükseklik; j: eğik eksen (k = j + floor(i/2)) */
  private hij(i: number, j: number): number {
    const a = i - this.iMin, b = j + Math.floor(i / 2) - this.kMin;
    if (a < 0 || a >= this.nI || b < 0 || b >= this.nK) return this.outside;
    return this.H[a * this.nK + b];
  }
  hik(i: number, k: number): number {
    const a = i - this.iMin, b = k - this.kMin;
    if (a < 0 || a >= this.nI || b < 0 || b >= this.nK) return this.outside;
    return this.H[a * this.nK + b];
  }
  /** (x, z) noktasındaki yükseklik: çizilen üçgenlerle birebir aynı (barisentrik) */
  sample(x: number, z: number): number {
    const fi = x / this.dx, fj = z / this.dz - fi / 2;
    const i0 = Math.floor(fi), j0 = Math.floor(fj);
    const a = fi - i0, b = fj - j0;
    if (a + b <= 1) return this.hij(i0, j0) * (1 - a - b) + this.hij(i0 + 1, j0) * a + this.hij(i0, j0 + 1) * b;
    return this.hij(i0 + 1, j0 + 1) * (a + b - 1) + this.hij(i0 + 1, j0) * (1 - b) + this.hij(i0, j0 + 1) * (1 - a);
  }
  /** eğim (yükseklik değişimi / yatay mesafe), kafes komşularından */
  slopeAt(x: number, z: number): number {
    const e = this.dz;
    const gx = (this.sample(x + e, z) - this.sample(x - e, z)) / (2 * e);
    const gz = (this.sample(x, z + e) - this.sample(x, z - e)) / (2 * e);
    return Math.hypot(gx, gz);
  }
  /** ışın–zemin kesişimi (adım + ikiye bölme); yoksa null */
  raycast(o: THREE.Vector3, d: THREE.Vector3, maxT = 600, floorY = -Infinity): THREE.Vector3 | null {
    let t = 0, prev = 0;
    const f = (tt: number) => o.y + d.y * tt - Math.max(this.sample(o.x + d.x * tt, o.z + d.z * tt), floorY);
    let fp = f(0);
    if (fp < 0) return o.clone();
    while (t < maxT) {
      const step = Math.max(0.1, Math.min(3, fp * 0.3)); // dik sırtları atlamasın (eğim ~3'e kadar)
      prev = t; t += step;
      const fv = f(t);
      if (fv <= 0) {
        let lo = prev, hi = t;
        for (let q = 0; q < 18; q++) { const m = (lo + hi) / 2; if (f(m) > 0) lo = m; else hi = m; }
        return new THREE.Vector3(o.x + d.x * hi, o.y + d.y * hi, o.z + d.z * hi);
      }
      fp = fv;
      if (d.y > 0 && o.y + d.y * t > 200) return null;
    }
    return null;
  }

  /**
   * Parça parça zemin ağları. col(x, z, h, slope, out) köşe rengini verir; skip(h0,h1,h2) üçgeni atlar.
   * Ağlar indeksli; düz gölgeleme malzemeden gelir.
   */
  buildChunks(mat: THREE.Material, col: (x: number, z: number, h: number, out: number[]) => void, skip: (a: number, b: number, c: number) => boolean, CI = 96, CK = 64): THREE.Mesh[] {
    const out: THREE.Mesh[] = [];
    const cc = [0, 0, 0];
    for (let ci = 0; ci < this.nI - 1; ci += CI) {
      for (let ck = 0; ck < this.nK - 1; ck += CK) {
        // hücre sayısı; tek sütunlardaki ikinci üçgen bir üst satıra uzandığı için bir satır fazla köşe alınır
        const cI = Math.min(CI, this.nI - 1 - ci), cK = Math.min(CK, this.nK - 1 - ck);
        const ni = cI + 1, nk = Math.min(cK + 2, this.nK - ck);
        const pos = new Float32Array(ni * nk * 3), colA = new Float32Array(ni * nk * 3);
        for (let a = 0; a < ni; a++) {
          const i = ci + a + this.iMin;
          for (let b = 0; b < nk; b++) {
            const k = ck + b + this.kMin, v = (a * nk + b) * 3;
            const x = this.vx(i), z = this.vz(i, k), h = this.H[(ci + a) * this.nK + ck + b];
            pos[v] = x; pos[v + 1] = h; pos[v + 2] = z;
            col(x, z, h, cc); colA[v] = cc[0]; colA[v + 1] = cc[1]; colA[v + 2] = cc[2];
          }
        }
        // (i,j) hücresi: köşeler (a,b) — j ekseni sütun eşliğine göre k'ye çevrilir
        const idx: number[] = [];
        const V = (a: number, b: number) => a * nk + b;
        for (let a = 0; a < cI; a++) {
          const i = ci + a + this.iMin, odd = i & 1;
          for (let b = 0; b < cK; b++) {
            // sütun a'daki (b) ile sütun a+1'deki komşular: tek sütunda sağ komşu b+1'den, çiftte b'den başlar
            const v00 = V(a, b), v01 = V(a, b + 1);
            const top = odd ? b + 2 : b + 1;
            const r0 = odd ? V(a + 1, b + 1) : V(a + 1, b);
            const h00 = pos[v00 * 3 + 1], h01 = pos[v01 * 3 + 1], hr0 = pos[r0 * 3 + 1];
            // üçgen 1: (a,b) (a,b+1) (a+1, sağ-alt)
            if (!skip(h00, h01, hr0)) idx.push(v00, v01, r0);
            // üçgen 2: (a+1, sağ-alt) (a,b+1) (a+1, sağ-üst)
            if (top < nk) { const r1 = V(a + 1, top), hr1 = pos[r1 * 3 + 1]; if (!skip(hr0, h01, hr1)) idx.push(r0, v01, r1); }
          }
        }
        if (!idx.length) continue;
        const geo = new THREE.BufferGeometry();
        geo.setAttribute('position', new THREE.BufferAttribute(pos, 3));
        geo.setAttribute('color', new THREE.BufferAttribute(colA, 3));
        geo.setIndex(pos.length / 3 > 65535 ? new THREE.Uint32BufferAttribute(idx, 1) : new THREE.Uint16BufferAttribute(idx, 1));
        geo.computeVertexNormals();
        geo.computeBoundingSphere();
        geo.computeBoundingBox();
        const m = new THREE.Mesh(geo, mat);
        m.receiveShadow = true; m.castShadow = true;
        m.userData.chunk = [ci, ck, ni, nk];
        out.push(m);
      }
    }
    return out;
  }
  /** mevsim değişince renkleri yeniden boya */
  recolor(meshes: THREE.Mesh[], col: (x: number, z: number, h: number, out: number[]) => void) {
    const cc = [0, 0, 0];
    for (const m of meshes) {
      const p = m.geometry.getAttribute('position') as THREE.BufferAttribute, c = m.geometry.getAttribute('color') as THREE.BufferAttribute;
      const pa = p.array as Float32Array, ca = c.array as Float32Array;
      for (let v = 0; v < pa.length; v += 3) { col(pa[v], pa[v + 2], pa[v + 1], cc); ca[v] = cc[0]; ca[v + 1] = cc[1]; ca[v + 2] = cc[2]; }
      c.needsUpdate = true;
    }
  }
}
