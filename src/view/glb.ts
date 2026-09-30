// Blender'dan gelen .glb dosyalarını eşzamanlı okur (GLTFLoader'a gerek yok, ağ yok).
// Beklenen biçim (assets/blender/fdkit.py üretir): her düğüm tek bir mesh, dönüşümsüz;
// POSITION (float3) + COLOR_0 (RGBA: RGB renk, A medeniyet maskesi), üçgen indeksli.
// Çıktı: düğüm adı → indekssiz BufferGeometry (position, color[4], normal).
import * as THREE from 'three';

interface Accessor { bufferView: number; byteOffset?: number; componentType: number; count: number; type: string; normalized?: boolean }
interface BufferView { buffer: number; byteOffset?: number; byteLength: number; byteStride?: number }
interface GltfJson {
  nodes: { name?: string; mesh?: number }[];
  meshes: { name?: string; primitives: { attributes: Record<string, number>; indices?: number; mode?: number }[] }[];
  accessors: Accessor[];
  bufferViews: BufferView[];
}

const COMP: Record<string, number> = { SCALAR: 1, VEC2: 2, VEC3: 3, VEC4: 4 };

function read(json: GltfJson, bin: DataView, ai: number): Float32Array | Uint32Array {
  const a = json.accessors[ai];
  const bv = json.bufferViews[a.bufferView];
  const n = COMP[a.type] * a.count;
  const base = (bv.byteOffset ?? 0) + (a.byteOffset ?? 0);
  const size = a.componentType === 5126 || a.componentType === 5125 ? 4 : a.componentType === 5123 || a.componentType === 5122 ? 2 : 1;
  const stride = bv.byteStride ?? COMP[a.type] * size;
  const isIndex = a.type === 'SCALAR' && !a.normalized && a.componentType !== 5126;
  const out = isIndex ? new Uint32Array(n) : new Float32Array(n);
  const k = COMP[a.type];
  for (let i = 0; i < a.count; i++) {
    const o = base + i * stride;
    for (let c = 0; c < k; c++) {
      const p = o + c * size;
      let v: number;
      switch (a.componentType) {
        case 5126: v = bin.getFloat32(p, true); break;
        case 5125: v = bin.getUint32(p, true); break;
        case 5123: v = bin.getUint16(p, true); if (a.normalized) v /= 65535; break;
        case 5121: v = bin.getUint8(p); if (a.normalized) v /= 255; break;
        default: throw new Error('glb: desteklenmeyen bileşen ' + a.componentType);
      }
      out[i * k + c] = v;
    }
  }
  return out;
}

export function parseGlb(data: Uint8Array): Map<string, THREE.BufferGeometry> {
  const dv = new DataView(data.buffer, data.byteOffset, data.byteLength);
  if (dv.getUint32(0, true) !== 0x46546c67) throw new Error('glb: imza yok');
  let off = 12;
  let json: GltfJson | null = null;
  let bin: DataView | null = null;
  while (off < data.byteLength) {
    const len = dv.getUint32(off, true), type = dv.getUint32(off + 4, true);
    const body = data.subarray(off + 8, off + 8 + len);
    if (type === 0x4e4f534a) json = JSON.parse(new TextDecoder().decode(body));
    else if (type === 0x004e4942) bin = new DataView(body.buffer, body.byteOffset, body.byteLength);
    off += 8 + len;
  }
  if (!json || !bin) throw new Error('glb: parça eksik');
  const out = new Map<string, THREE.BufferGeometry>();
  for (const node of json.nodes) {
    if (node.mesh === undefined) continue;
    const prims = json.meshes[node.mesh].primitives;
    const pos: number[] = [], col: number[] = [];
    for (const pr of prims) {
      const P = read(json, bin, pr.attributes.POSITION) as Float32Array;
      const C = pr.attributes.COLOR_0 !== undefined ? read(json, bin, pr.attributes.COLOR_0) as Float32Array : null;
      const cN = C ? C.length / (P.length / 3) : 0;
      const I = pr.indices !== undefined ? read(json, bin, pr.indices) : null;
      const cnt = I ? I.length : P.length / 3;
      for (let i = 0; i < cnt; i++) {
        const v = I ? I[i] : i;
        pos.push(P[v * 3], P[v * 3 + 1], P[v * 3 + 2]);
        if (C) col.push(C[v * cN], C[v * cN + 1], C[v * cN + 2], cN === 4 ? C[v * cN + 3] : 0);
        else col.push(1, 1, 1, 0);
      }
    }
    const g = new THREE.BufferGeometry();
    g.setAttribute('position', new THREE.Float32BufferAttribute(pos, 3));
    g.setAttribute('color', new THREE.Float32BufferAttribute(col, 4));
    g.computeVertexNormals();
    g.computeBoundingSphere();
    out.set(node.name ?? json.meshes[node.mesh].name ?? String(node.mesh), g);
  }
  return out;
}

/**
 * Köşe rengi + medeniyet maskesi: A=0 → köşe rengi, A=1 → köşe rengi (gölge) × örnek rengi.
 * Örnek rengi böylece yalnız maskeli parçayı (çatı, sancak) boyar; kışın kar tonu da yalnız onu beyazlatır.
 */
export function maskedVertexColors(m: THREE.Material): THREE.Material {
  (m as THREE.MeshStandardMaterial).vertexColors = true;
  m.onBeforeCompile = (sh) => {
    sh.vertexShader = sh.vertexShader.replace('#include <color_vertex>', `
#if defined( USE_COLOR_ALPHA )
  vColor = vec4( color.rgb, 1.0 );
  #ifdef USE_INSTANCING_COLOR
    vColor.rgb *= mix( vec3( 1.0 ), instanceColor.rgb, color.a );
  #endif
#elif defined( USE_COLOR ) || defined( USE_INSTANCING_COLOR )
  vColor = vec3( 1.0 );
  #ifdef USE_COLOR
    vColor *= color;
  #endif
  #ifdef USE_INSTANCING_COLOR
    vColor.xyz *= instanceColor.xyz;
  #endif
#endif`);
    // gece sıcak ışıması albedoyla orantılı: badana hafif parlar, koyu çatı ve kiriş parlamaz
    sh.fragmentShader = sh.fragmentShader.replace('#include <emissivemap_fragment>', `#include <emissivemap_fragment>
#if defined( USE_COLOR_ALPHA ) || defined( USE_COLOR )
  totalEmissiveRadiance *= vColor.rgb;
#endif`);
  };
  m.customProgramCacheKey = () => 'fd-masked-vc';
  return m;
}
