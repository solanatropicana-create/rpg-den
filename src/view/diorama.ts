// 3D low-poly hex diorama (Three.js). Simülasyonu yalnızca okur.
import * as THREE from 'three';
import { MapControls } from 'three/examples/jsm/controls/MapControls.js';
import { EffectComposer } from 'three/examples/jsm/postprocessing/EffectComposer.js';
import { RenderPass } from 'three/examples/jsm/postprocessing/RenderPass.js';
import { ShaderPass } from 'three/examples/jsm/postprocessing/ShaderPass.js';
import { OutputPass } from 'three/examples/jsm/postprocessing/OutputPass.js';
import { UnrealBloomPass } from 'three/examples/jsm/postprocessing/UnrealBloomPass.js';
import { HorizontalTiltShiftShader } from 'three/examples/jsm/shaders/HorizontalTiltShiftShader.js';
import { VerticalTiltShiftShader } from 'three/examples/jsm/shaders/VerticalTiltShiftShader.js';
import type { Sim } from '../sim/sim';
import { musterKey, atTarget } from '../sim/agents';
import { YEAR } from '../sim/sim';
import { DEPOSITS, GOODS, WORKSHOPS, type Terrain } from '../data/goods';
import { HERO_NAMES } from '../data/heroes';
import type { Agent, GameEvent, CampKind, GuestKind, Hero, Inn, Settlement } from '../sim/types';
import { GUEST } from '../data/inn';
import { ek } from '../sim/tr';
import { Atmosphere } from './atmosphere';
import { freeHulls, freeGalleys, portWater } from '../sim/sea';
import { SmokeSystem } from './smoke';
import { Heightfield, Noise2 } from './terrain';
import { FreeCam } from './freecam';
import * as M from './models';
import { parseGlb, maskedVertexColors } from './glb';
import humanHouseGlb from '../../assets/models/human_house.glb';
import type { SoundInfo } from './audio';
import { registerFigures, drawFigure, drawHorse, type Look, type Pose, type Tool } from './figures';

// ---------------------------------------------------------------- yardımcılar
const s_g = (sim: Sim) => sim.g;
const SQ3 = Math.sqrt(3);
const DIO_FAMILY = ['Taşkalp', 'Demirel', 'Kızılsakal', 'Akbulut', 'Yıldızgöz', 'Karaorman', 'Gümüşkol', 'Çelikyürek', 'Kuyucu', 'Değirmenci', 'Ormanlı', 'Kayalı', 'Sarıbaş', 'Tunçer', 'Balkaya', 'Dereli', 'Yelkenci', 'Ocakçı', 'Bakırcı', 'Kurtbey', 'Ayvaz', 'Tepeli', 'Göltaş', 'Akkuş', 'Sessizdere', 'Kartalgöz', 'Ilgaz', 'Çınaraltı'];
const PROF: Record<string, string> = { grain: 'çiftçi', meat: 'avcı', fish: 'balıkçı', wood: 'oduncu', stone: 'taşçı', clay: 'kilci', leather: 'avcı', copper: 'madenci', tin: 'madenci', iron: 'madenci', coal: 'kömürcü', gold: 'altın madencisi', salt: 'tuzcu', horses: 'seyis', herbs: 'otçu', fur: 'kürkçü', mana: 'kristalci', mithril: 'mithril madencisi', heartwood: 'ağaç bekçisi', asker: 'asker' };
const hash = (i: number, k: number) => { const v = Math.sin(i * 12.9898 + k * 78.233) * 43758.5453; return v - Math.floor(v); };
/** duman topağı boyu: u (0→1 yükseliş) boyunca büyür, son üçte birde küçülüp erir */
const puffS = (u: number, s0: number, grow: number) => (s0 + u * grow) * Math.min(1, (1 - u) * 3.2);
const _jc = new THREE.Color();
/** renge küçük ton/parlaklık oynaması: aynı nesnelerin tekdüze görünmemesi için */
const jitter = (hex: number, r: number, l = 0.06, hue = 0.012) => _jc.setHex(hex).offsetHSL((r - 0.5) * hue, 0, (hash(r * 977, 3) - 0.5) * l).getHex();
const TERRAIN_COL: Record<Terrain, number> = {
  grass: 0x86ad5a, forest: 0x5f944f, oldforest: 0x3f7a4b, hill: 0xb3a36b, mountain: 0x78726b, water: 0x4d8bbd, swamp: 0x6d7f55, tundra: 0xe4ecef,
};
const WATER_Y = 0.32;
const SEA_H = -0.9;
const RIN = 0.6;
const EARTH_COL: Record<Terrain, number> = {
  grass: 0x7a5b3b, forest: 0x6e5236, oldforest: 0x5e4630, hill: 0x8a6e4c, mountain: 0x77716b, water: 0xb3a174, swamp: 0x5c4d3a, tundra: 0x8e8b88,
};
/** zemin boyası renkleri (THREE.Color: doğrusal uzayda; köşe renkleri de doğrusal) */
const GC = {
  bed: new THREE.Color(0x264a5e), alp: new THREE.Color(0x6f7d4a), snow: new THREE.Color(0xf1f4f6), sand: new THREE.Color(0xd6c592),
  rockW: new THREE.Color(0x6e665d), rockC: new THREE.Color(0x5c6066),
};
const TERRAIN_BONUS: Partial<Record<Terrain, number>> = { mountain: 1.9, hill: 0.22, oldforest: 0.03, swamp: -0.08, forest: 0.02 };
/** ova yükseklik ölçeği: dünya düz ve okunur olsun (eskiden 3.4: her yer dalgalı tepeydi) */
const ELEV_K = 1.5;
const CAP = 0.14;
const SKIN: Record<string, number> = { human: 0xe8b894, dwarf: 0xd9a47e, elf: 0xf0d3b5, halfling: 0xeec39c, gnome: 0xf2c8a8, halfelf: 0xe9c09c, halforc: 0x8fa66b, dragonborn: 0xb85f3e, tiefling: 0xc0566d };
const WS_BY_NAME: Record<string, string> = Object.fromEntries(Object.entries(WORKSHOPS).map(([k, d]) => [d.name, k]));
const WS_TOOL: Record<string, [Tool, Pose]> = {
  smithy: ['mace', 'work'], foundry: ['mace', 'work'], armory: ['mace', 'work'], toolmaker: ['mace', 'work'], mithrilforge: ['mace', 'work'],
  sawmill: ['axe', 'work'], bakery: ['basket', 'work'], brickkiln: ['shovel', 'work'], brewery: ['basket', 'work'], apothecary: ['basket', 'work'],
  scriptorium: ['staff', 'cast'], enchanter: ['staff', 'cast'],
};
const w_isTown = (sim: Sim, i: number) => sim.w.settlements.some((q) => q.alive && q.tier >= 2 && q.tile === i);
const MONSTER_COL = { goblin: 0x6f8f2f, hobgoblin: 0xc0502e, bugbear: 0x7a5533 };
const TUNIC_S = [0x9a8a6a, 0x7a6a4a, 0x8a5a3a, 0x6a7a5a, 0xa89a7a, 0x5a6a7a];

interface Geo { geo: THREE.BufferGeometry; mat: THREE.Material; shadow?: boolean; receive?: boolean }

/** Aynı geometri/malzemeyi paylaşan örnekleri toplayıp InstancedMesh'e döken katman */
class Layer {
  group = new THREE.Group();
  private meshes = new Map<string, THREE.InstancedMesh>();
  private buf = new Map<string, { m: Float32Array; c: Float32Array; n: number }>();
  private m4 = new THREE.Matrix4();
  private q = new THREE.Quaternion();
  private e = new THREE.Euler(0, 0, 0, 'YXZ');
  private p = new THREE.Vector3();
  private s = new THREE.Vector3();
  private col = new THREE.Color();
  /** chunk > 0: örnekler dünya parçalarına bölünür, ekran dışındaki parçalar çizilmez */
  constructor(private reg: Map<string, Geo>, private chunk = 0) {}
  /** uzakta çizilmeyecek küçük ayrıntılar (çimen, çiçek): anahtar → görünür uzaklık */
  near: Record<string, number> = {};
  private centers = new Map<string, [number, number]>();
  mesh(key: string) { return this.meshes.get(key); }
  begin() { for (const b of this.buf.values()) b.n = 0; }
  /** yakın planda ayrıntılı model (LOD): uzak anahtar → [yakın anahtar, uzaklık]; iki sürüm aynı parçaya yazılır, biri görünür */
  lod: Record<string, [string, number]> = {};
  private lodHi: Record<string, number> | null = null;
  /** kameraya uzak ayrıntı parçalarını gizle; LOD parçalarında yakın/uzak sürümü seç */
  cull(cx: number, cz: number, camD = 0) {
    if (!this.chunk) return;
    if (!this.lodHi) { this.lodHi = {}; for (const [hi, d] of Object.values(this.lod)) this.lodHi[hi] = d; }
    for (const [k, m] of this.meshes) {
      const base = k.slice(0, k.indexOf('#'));
      const lim = this.near[base];
      const c = this.centers.get(k)!;
      if (lim) { m.visible = m.count > 0 && camD < lim * 1.3 && (c[0] - cx) ** 2 + (c[1] - cz) ** 2 < lim * lim; continue; }
      const lo = this.lod[base], hiD = this.lodHi[base];
      const d = lo ? lo[1] : hiD;
      if (!d) continue;
      // parça köşesi hedefe yakınsa ayrıntılı sürüm (parça merkezine göre ölçmek yakındaki parçanın yarısını kaba bırakıyordu)
      const h = this.chunk / 2, qx = Math.max(Math.abs(c[0] - cx) - h, 0), qz = Math.max(Math.abs(c[1] - cz) - h, 0);
      const near = camD < d * 1.2 && qx * qx + qz * qz < d * d;
      m.visible = m.count > 0 && (lo ? !near : near);
    }
  }
  /** anahtar → [hedef renk, oran]: ör. kışın çatılara kar */
  tint: Record<string, [number, number]> = {};
  private tc = new THREE.Color();
  add(key: string, x: number, y: number, z: number, sx = 1, sy = 1, sz = 1, ry = 0, color = 0xffffff, rx = 0, rz = 0) {
    const tn = this.tint[key];
    if (tn) color = this.tc.setHex(color).lerp(this.col.setHex(tn[0]), tn[1]).getHex();
    this.put(key, x, y, z, sx, sy, sz, ry, color, rx, rz);
  }
  private put(key: string, x: number, y: number, z: number, sx: number, sy: number, sz: number, ry: number, color: number, rx: number, rz: number) {
    if (this.chunk) { const cx = Math.floor(x / this.chunk), cz = Math.floor(z / this.chunk); key = `${key}#${cx},${cz}`; if (!this.centers.has(key)) this.centers.set(key, [(cx + 0.5) * this.chunk, (cz + 0.5) * this.chunk]); }
    let b = this.buf.get(key);
    if (!b) { b = { m: new Float32Array(16 * 256), c: new Float32Array(3 * 256), n: 0 }; this.buf.set(key, b); }
    if (b.n * 16 >= b.m.length) {
      const m = new Float32Array(b.m.length * 2); m.set(b.m); b.m = m;
      const c = new Float32Array(b.c.length * 2); c.set(b.c); b.c = c;
    }
    this.e.set(rx, ry, rz);
    this.q.setFromEuler(this.e);
    this.m4.compose(this.p.set(x, y, z), this.q, this.s.set(sx, sy, sz));
    this.m4.toArray(b.m, b.n * 16);
    this.col.setHex(color);
    b.c[b.n * 3] = this.col.r; b.c[b.n * 3 + 1] = this.col.g; b.c[b.n * 3 + 2] = this.col.b;
    b.n++;
  }
  end() {
    for (const [key, b] of this.buf) {
      let mesh = this.meshes.get(key);
      const need = b.n;
      if (!mesh || mesh.instanceMatrix.count < need) {
        if (mesh) { this.group.remove(mesh); mesh.dispose(); }
        const g = this.reg.get(this.chunk ? key.slice(0, key.indexOf('#')) : key)!;
        const cap = Math.max(16, Math.ceil(need * 1.5));
        mesh = new THREE.InstancedMesh(g.geo, g.mat, cap);
        mesh.instanceColor = new THREE.InstancedBufferAttribute(new Float32Array(cap * 3), 3);
        mesh.castShadow = g.shadow ?? true;
        mesh.receiveShadow = g.receive ?? true;
        mesh.frustumCulled = !!this.chunk;
        this.meshes.set(key, mesh);
        this.group.add(mesh);
      }
      (mesh.instanceMatrix.array as Float32Array).set(b.m.subarray(0, need * 16));
      (mesh.instanceColor!.array as Float32Array).set(b.c.subarray(0, need * 3));
      mesh.count = need;
      // boş örnek kümesi çizim çağrısı üretmesin
      mesh.visible = need > 0;
      mesh.instanceMatrix.needsUpdate = true;
      mesh.instanceColor!.needsUpdate = true;
      if (this.chunk) { if (need) mesh.computeBoundingSphere(); }
      else if ((key === 'hex' || key === 'col') && !mesh.boundingSphere) mesh.computeBoundingSphere();
      // LOD: ayrıntılı sürüm aynı örnek tamponlarını paylaşır (ikinci kez yazılmaz, ikinci kez yüklenmez)
      const hash_ = this.chunk ? key.indexOf('#') : -1;
      const ld = hash_ > 0 ? this.lod[key.slice(0, hash_)] : undefined;
      if (ld) {
        const hk = ld[0] + key.slice(hash_);
        let hm = this.meshes.get(hk);
        if (!hm) {
          const g = this.reg.get(ld[0])!;
          hm = new THREE.InstancedMesh(g.geo, g.mat, 1);
          hm.castShadow = g.shadow ?? true; hm.receiveShadow = g.receive ?? true; hm.frustumCulled = true;
          this.meshes.set(hk, hm); this.group.add(hm);
          this.centers.set(hk, this.centers.get(key)!);
        }
        hm.instanceMatrix = mesh.instanceMatrix; hm.instanceColor = mesh.instanceColor;
        hm.count = need; hm.visible = false;
        if (need) hm.computeBoundingSphere();
      }
    }
  }
}

interface House { x: number; y: number; z: number; kind: string; cap: number; occ: number; race: string; seed: number; burnt: boolean }
interface TownLayout { x: number; z: number; homes: [number, number][]; work: Record<string, [number, number][]>; R: number; wallR: number; houses: House[]; homeless: number }
interface BattleFx { tile: number; t0: number; a: number; b: number; winner: 'A' | 'B'; dur: number; n: number; raceA: string; mk?: CampKind; raceB: string; naval?: boolean; pirate?: boolean }
interface Berth { x: number; z: number; face: number; st: number }
interface Pop { el: HTMLDivElement; tile: number; t0: number; dur: number }

export class Diorama {
  renderer: THREE.WebGLRenderer;
  scene = new THREE.Scene();
  camera: THREE.PerspectiveCamera;
  controls: MapControls;
  composer: EffectComposer;
  private bloom!: UnrealBloomPass;
  private hPass: ShaderPass;
  private vPass: ShaderPass;
  private sun: THREE.DirectionalLight;
  private hemi: THREE.HemisphereLight;
  private reg = new Map<string, Geo>();
  private terrain: Layer;
  private dynamic: Layer;
  private actors: Layer;
  private terr: THREE.Group = new THREE.Group();
  private sim!: Sim;
  private lastTerrain = -999;
  private lastSeason = -1;
  private lastDyn = -999;
  private lastDynReal = 0;
  private overlay: HTMLDivElement;
  private labels = new Map<number, HTMLDivElement>();
  /** son karede çizilen kahramanların konumu (baş üstü rozet için) */
  private heroSpots = new Map<number, [number, number, number]>();
  private pops: Pop[] = [];
  private battles: BattleFx[] = [];
  /** limanlarda gemi bağlama yerleri (buildDynamic'te hesaplanır, gemiler her karede çizilir) */
  private berths = new Map<number, Berth[]>();
  private windowMat!: THREE.MeshStandardMaterial;
  /** Blender asset'leri: köşe rengi + medeniyet maskesi (bkz. glb.ts, assets/blender) */
  private assetMat!: THREE.MeshStandardMaterial;
  private foamMat!: THREE.MeshStandardMaterial;
  private waterMesh: THREE.Group = new THREE.Group();
  private atmo = new Atmosphere(WATER_Y);
  private night = 0;
  /** kesintisiz zemin: yükseklik alanı + parça ağları */
  private hf = new Heightfield(3);
  private ground = new THREE.Group();
  private groundMeshes: THREE.Mesh[] = [];
  private groundMat = new THREE.MeshStandardMaterial({ vertexColors: true, flatShading: true, roughness: 0.95 });
  /** karo merkezlerinin gerçek zemin yüksekliği */
  private hc = new Float32Array(0);
  private noise = new Noise2(1);
  fly!: FreeCam;
  private grid: THREE.LineSegments = new THREE.LineSegments(new THREE.BufferGeometry(), new THREE.LineBasicMaterial({ color: 0x000000, transparent: true, opacity: 0.13, depthWrite: false }));
  private borders: THREE.Mesh = new THREE.Mesh(new THREE.BufferGeometry(), new THREE.MeshBasicMaterial({ vertexColors: true, transparent: true, opacity: 1, depthWrite: false, side: THREE.DoubleSide, polygonOffset: true, polygonOffsetFactor: -2 }));
  private th = new Float32Array(0);
  private ch = new Float32Array(0);
  private mh = new Float32Array(0);
  private groundSeason = -1;
  showGrid = true;
  private tween: { pos: THREE.Vector3; target: THREE.Vector3; k?: number } | null = null;
  autoOrbit = false;
  private layouts = new Map<number, TownLayout>();
  private cyc = 0.3;
  /** günün saati adı (üst şerit için); gece/gündüz kapalıysa boş */
  timeOfDay(): string {
    if (!this.dayNight && this.debugCyc === undefined) return '';
    const c = this.cyc;
    return c < 0.05 ? 'şafak' : c < 0.19 ? 'sabah' : c < 0.31 ? 'öğle' : c < 0.44 ? 'ikindi' : c < 0.54 ? 'akşam' : c < 0.96 ? 'gece' : 'şafak';
  }
  private burning: [number, number, number, number][] = [];
  /** yol trafiği: yerleşimden yola çıkan yolcu hatları */
  walkPos: [number, number][] = [];
  /** üstüne gelinebilen halk: konum, isim, iş, hâl */
  cits: { x: number; y: number; z: number; name: string; role: string; st: number; mood: string; race: string; idx: number }[] = [];
  private bubbles: { x: number; y: number; z: number; icon: string; id: number; fixed?: boolean }[] = [];
  private bubEls: HTMLDivElement[] = [];
  private walks: { pts: [number, number][]; cum: number[]; len: number; race: string; civ: number; kind: number; id: number }[] = [];
  private smokes: [number, number, number][] = [];
  private smoke = new SmokeSystem();
  /** 2 yüksek, 1 orta, 0 düşük */
  quality = 2;
  autoQuality = true;
  fps = 60;
  onQuality?: (q: number, auto: boolean) => void;
  private fcount = 0;
  private ftime = 0;
  private lowFor = 0;
  private highFor = 0;
  private drops = [0, 0, 0];
  setQuality(q: number, auto = false) {
    q = Math.max(0, Math.min(2, q));
    this.quality = q;
    const dpr = window.devicePixelRatio || 1;
    // 1'in altına inmiyoruz: alt çözünürlük kenarları tırtıklı/bulanık gösteriyordu
    const pr = q === 2 ? Math.min(2, dpr) : q === 1 ? Math.min(1.25, dpr) : 1;
    this.renderer.setPixelRatio(pr);
    this.composer.setPixelRatio(pr);
    this.setMsaa(q === 0 ? 2 : 4);
    const size = q === 2 ? 2048 : 1024;
    if (this.sun.shadow.mapSize.x !== size) { this.sun.shadow.map?.dispose(); this.sun.shadow.map = null as unknown as THREE.WebGLRenderTarget; this.sun.shadow.mapSize.set(size, size); }
    this.sun.castShadow = q > 0;
    if (this.bloom) this.bloom.enabled = q === 2;
    this.w = 0; // yeniden boyutlandırmayı zorla
    this.onQuality?.(q, auto);
  }
  private setMsaa(n: number) {
    for (const rt of [this.composer.renderTarget1, this.composer.renderTarget2]) {
      if (rt.samples !== n) { rt.samples = n; rt.dispose(); }
    }
  }
  private lastFrame = 0;
  showDeposits = true;
  tiltShift = false;
  dayNight = true;
  private w = 1; private h = 1;
  private center = new THREE.Vector3();

  constructor(private host: HTMLElement) {
    this.renderer = new THREE.WebGLRenderer({ antialias: true, powerPreference: 'high-performance' });
    this.renderer.setPixelRatio(Math.min(2, window.devicePixelRatio || 1));
    this.renderer.shadowMap.enabled = true;
    this.renderer.shadowMap.type = THREE.PCFSoftShadowMap;
    this.renderer.toneMapping = THREE.ACESFilmicToneMapping;
    this.renderer.toneMappingExposure = 1.05;
    this.renderer.domElement.className = 'dio';
    host.appendChild(this.renderer.domElement);
    // ekran kartı sürücüsü sıfırlanırsa (uyku, sürücü güncellemesi) boş siyah ekran yerine açıklama göster
    const lost = document.createElement('div');
    lost.className = 'dio-lost'; lost.hidden = true;
    lost.textContent = 'Grafik bağlamı kayboldu, yeniden kuruluyor…';
    host.appendChild(lost);
    this.renderer.domElement.addEventListener('webglcontextlost', (e) => { e.preventDefault(); lost.hidden = false; this.ctxLost = true; }, false);
    this.renderer.domElement.addEventListener('webglcontextrestored', () => { lost.hidden = true; this.ctxLost = false; this.forceRebuild(); this.w = 0; }, false);
    // hafif köşe kararması: diorama/masa üstü hissi (CSS, kareye maliyeti yok)
    this.vignette = document.createElement('div');
    this.vignette.className = 'dio-vignette';
    host.appendChild(this.vignette);
    this.overlay = document.createElement('div');
    this.overlay.className = 'dio-overlay';
    host.appendChild(this.overlay);
    this.overlay.addEventListener('click', (e) => { const el = (e.target as HTMLElement).closest<HTMLElement>('[data-inn]'); if (el) this.onInnClick?.(Number(el.dataset.inn)); });

    this.camera = new THREE.PerspectiveCamera(38, 1, 0.5, 1000);
    this.controls = new MapControls(this.camera, this.renderer.domElement);
    this.controls.enableDamping = true;
    this.controls.dampingFactor = 0.12;
    this.controls.screenSpacePanning = false;
    this.controls.minDistance = 4;
    this.controls.maxDistance = 320;
    this.controls.maxPolarAngle = 1.53; // ufka bakabilsin
    this.controls.minPolarAngle = 0;
    this.controls.zoomToCursor = true;
    this.controls.rotateSpeed = 0.6;
    this.controls.keyPanSpeed = 22;
    this.controls.listenToKeyEvents(window);
    this.controls.addEventListener('start', () => { this.tween = null; });
    this.fly = new FreeCam(this.camera, this.renderer.domElement, {
      ground: (x, z) => this.hf.sample(x, z),
      water: WATER_Y,
      look: () => this.hf.raycast(this.camera.position, this.fly.forward(), 120, WATER_Y),
      collide: (x, z, px, pz) => this.collide(x, z, px, pz),
      bounds: () => this.sim ? { minX: -20, maxX: SQ3 * (this.sim.w.W + 0.5) + 20, minZ: -20, maxZ: 1.5 * this.sim.w.H + 20 } : { minX: -1e3, maxX: 1e3, minZ: -1e3, maxZ: 1e3 },
    });
    this.fly.onModeChange = () => this.onRoam?.();
    window.addEventListener('keydown', (e) => {
      if (e.target instanceof HTMLInputElement || this.renderer.domElement.hidden || this.fly.on) return;
      if (e.code === 'KeyQ') this.orbit(0.25, 0);
      else if (e.code === 'KeyE') this.orbit(-0.25, 0);
      else if (e.code === 'KeyR') this.orbit(0, -0.15);
      else if (e.code === 'KeyF') this.orbit(0, 0.15);
      else if (e.code === 'KeyW' || e.code === 'KeyS' || e.code === 'KeyA' || e.code === 'KeyD') this.panKey(e.code);
    });

    this.scene.background = new THREE.Color(0xbfd9e6);
    this.scene.fog = new THREE.Fog(0xbfd9e6, 120, 260);
    this.hemi = new THREE.HemisphereLight(0xdfefff, 0x5a6b3c, 0.9);
    this.scene.add(this.hemi);
    this.sun = new THREE.DirectionalLight(0xfff1d6, 2.1);
    this.sun.castShadow = true;
    this.sun.shadow.mapSize.set(2048, 2048);
    const sc = this.sun.shadow.camera;
    sc.left = -75; sc.right = 75; sc.top = 50; sc.bottom = -50; sc.near = 1; sc.far = 300;
    this.sun.shadow.bias = -0.0008;
    this.sun.shadow.normalBias = 0.03;
    this.scene.add(this.sun, this.sun.target);
    // masa: dioramanın oturduğu zemin
    const bed = new THREE.Mesh(new THREE.PlaneGeometry(4000, 4000), new THREE.MeshStandardMaterial({ color: 0x264a5e, roughness: 1 }));
    bed.rotation.x = -Math.PI / 2; bed.position.y = SEA_H - 0.3;
    this.waterMesh.add(this.atmo.group);
    this.scene.add(this.atmo.sky);
    this.setupGroundMat();
    this.grid.renderOrder = 2; this.borders.renderOrder = 3;
    this.scene.add(bed, this.ground, this.grid, this.borders);

    this.buildRegistry();
    this.terrain = new Layer(this.reg, 24);
    this.terrain.near = { tuft: 42, flower: 36, foam: 90, reed: 60, dot: 50 };
    // uzaklık parçanın en yakın noktasına: ayrıntılı bölge ≈ uzaklık + parça köşegeni
    this.terrain.lod = { tree: ['tree_hi', 24], pine: ['pine_hi', 24], blob: ['blob_hi', 24], bush: ['bush_hi', 16], trunk: ['trunk_hi', 14], rock: ['rock_hi', 16] };
    this.dynamic = new Layer(this.reg, 32);
    // uzaktan seçilemeyen küçük yapı ayrıntıları (pencere, kapı) parça parça çizilmez: çizim çağrısı tasarrufu
    this.dynamic.near = { win: 80, win2: 80, winoff: 70, door: 80, door0: 70 };
    this.dynamic.lod = { human_house_a: ['human_house_a_hi', 16], human_house_b: ['human_house_b_hi', 16], crop: ['crop_hi', 20], blob: ['blob_hi', 24], tree: ['tree_hi', 24], trunk: ['trunk_hi', 14], stake: ['stake_hi', 18] };
    this.actors = new Layer(this.reg);
    this.scene.add(this.terrain.group, this.dynamic.group, this.actors.group, this.terr, this.waterMesh, this.smoke.points);

    // EffectComposer kendi hedefine çizer; tarayıcının antialias'ı orada çalışmaz → MSAA'lı hedef
    this.composer = new EffectComposer(this.renderer, new THREE.WebGLRenderTarget(1, 1, { type: THREE.HalfFloatType, samples: 4 }));
    this.composer.addPass(new RenderPass(this.scene, this.camera));
    this.hPass = new ShaderPass(HorizontalTiltShiftShader);
    this.vPass = new ShaderPass(VerticalTiltShiftShader);
    this.composer.addPass(this.hPass);
    this.composer.addPass(this.vPass);
    this.bloom = new UnrealBloomPass(new THREE.Vector2(256, 256), 0.4, 0.45, 0.92);
    this.composer.addPass(this.bloom);
    this.composer.addPass(new OutputPass());
  }

  // ---------------------------------------------------------------- geometri kayıtları
  private buildRegistry() {
    const std = (flat = true, extra: THREE.MeshStandardMaterialParameters = {}) => new THREE.MeshStandardMaterial({ color: 0xffffff, flatShading: flat, roughness: 0.92, metalness: 0, ...extra });
    const base = std();
    const r = (key: string, geo: THREE.BufferGeometry, mat: THREE.Material = base, shadow = true, receive = true) => this.reg.set(key, { geo, mat, shadow, receive });
    // hex prizma: tabanı y=0, yüksekliği 1
    const hex = new THREE.CylinderGeometry(0.94, 0.985, CAP, 6); hex.translate(0, -CAP / 2, 0);
    r('hex', hex, base, false, true);
    const col = new THREE.CylinderGeometry(0.985, 0.985, 1, 6); col.translate(0, 0.5, 0);
    r('col', col, std(true, { roughness: 1 }), true, true);
    const water = new THREE.CylinderGeometry(1.0, 1.0, 1, 6); water.translate(0, 0.5, 0);
    r('water', water, std(false, { roughness: 0.25, metalness: 0.05, transparent: true, opacity: 0.88 }), false, true);
    // yeşillik rüzgârda hafifçe salınır (yakından bakınca dünya kıpırdasın)
    const leaf = std();
    leaf.onBeforeCompile = (sh) => {
      sh.uniforms.uWT = this.windU.t; sh.uniforms.uWA = this.windU.a;
      sh.vertexShader = 'uniform float uWT; uniform float uWA;\n' + sh.vertexShader.replace('#include <begin_vertex>', `#include <begin_vertex>
        {
          vec3 ip = vec3(0.0);
          #ifdef USE_INSTANCING
            ip = vec3(instanceMatrix[3][0], 0.0, instanceMatrix[3][2]);
          #endif
          float hh = max(0.0, position.y);
          float ph = ip.x * 0.73 + ip.z * 0.91;
          float sw = sin(uWT * 1.7 + ph) * 0.6 + sin(uWT * 2.9 + ph * 1.9) * 0.3;
          float k = hh * (0.035 + hh * 0.06) * uWA;
          transformed.x += sw * k;
          transformed.z += cos(uWT * 1.3 + ph * 1.3) * k * 0.7;
        }`);
    };
    // ağaç tacı ve gövdesi: kamera ile bakılan nokta arasına giren ağaçlar kafes desenle saydamlaşır
    // (ormanın içindeki kamp, köy ya da kahraman ağaçların arkasında kaybolmasın)
    const seeThrough = (sh: THREE.WebGLProgramParametersWithUniforms) => {
      sh.uniforms.uCam = this.occU.cam; sh.uniforms.uTgt = this.occU.tgt; sh.uniforms.uOn = this.occU.on;
      sh.vertexShader = 'varying vec3 vCanW;\n' + sh.vertexShader.replace('#include <project_vertex>', `#include <project_vertex>
        vec4 canW = vec4(transformed, 1.0);
        #ifdef USE_INSTANCING
          canW = instanceMatrix * canW;
        #endif
        vCanW = (modelMatrix * canW).xyz;`);
      sh.fragmentShader = 'uniform vec3 uCam; uniform vec3 uTgt; uniform float uOn; varying vec3 vCanW;\n' + sh.fragmentShader.replace('#include <clipping_planes_fragment>', `#include <clipping_planes_fragment>
        if (uOn > 0.0) {
          vec3 ab = uTgt - uCam; float L = length(ab); vec3 dd = ab / L;
          float tt = dot(vCanW - uCam, dd);
          if (tt > 0.0 && tt < L - 0.35) {
            vec3 lp = uCam + dd * tt;
            if (vCanW.y > lp.y - 0.6) {
              float R = mix(0.5, 1.9, tt / L);
              float fade = smoothstep(R, R * 0.45, length(vCanW - lp)) * uOn;
              const float BAYER[16] = float[16](0., 8., 2., 10., 12., 4., 14., 6., 3., 11., 1., 9., 15., 7., 13., 5.);
              int bi = int(mod(gl_FragCoord.x, 4.0)) + int(mod(gl_FragCoord.y, 4.0)) * 4;
              if (fade * 0.92 > (BAYER[bi] + 0.5) / 16.0) discard;
            }
          }
        }`);
    };
    const canopy = std();
    canopy.onBeforeCompile = (sh, rr) => { leaf.onBeforeCompile(sh, rr); seeThrough(sh); };
    const bark = std();
    bark.onBeforeCompile = (sh) => seeThrough(sh);
    // ağaçlar: uzakta sade, yakında katmanlı/topaklı (LOD, bkz. Layer.lod)
    r('tree', M.firGeo(false), canopy); r('tree_hi', M.firGeo(true), canopy);
    r('pine', M.pineGeo(false), canopy); r('pine_hi', M.pineGeo(true), canopy);
    r('bush', M.bushGeo(false), leaf); r('bush_hi', M.bushGeo(true), leaf);
    // gövde: uçları yaprak ve zeminle örtülü, kapaksız; yakında kök ve dal
    r('trunk', M.trunkGeo(false), bark); r('trunk_hi', M.trunkGeo(true), bark);
    r('blob', M.crownGeo(false), canopy); r('blob_hi', M.crownGeo(true), canopy);
    const peak = new THREE.ConeGeometry(0.78, 1.25, 5); peak.translate(0, 0.62, 0);
    // dağlar kamera ile bakılan nokta arasına girerse ekran kafesiyle (dither) saydamlaşır
    const occ = std();
    occ.onBeforeCompile = (sh) => {
      sh.uniforms.uCam = this.occU.cam; sh.uniforms.uTgt = this.occU.tgt; sh.uniforms.uOn = this.occU.on;
      sh.vertexShader = 'varying vec3 vOccW;\n' + sh.vertexShader.replace('#include <project_vertex>', `#include <project_vertex>
        vec4 occW = vec4(transformed, 1.0);
        #ifdef USE_INSTANCING
          occW = instanceMatrix * occW;
        #endif
        vOccW = (modelMatrix * occW).xyz;`);
      sh.fragmentShader = 'uniform vec3 uCam; uniform vec3 uTgt; uniform float uOn; varying vec3 vOccW;\n' + sh.fragmentShader.replace('#include <clipping_planes_fragment>', `#include <clipping_planes_fragment>
        if (uOn > 0.0) {
          vec3 ab = uTgt - uCam; float L = length(ab); vec3 dd = ab / L;
          float tt = dot(vOccW - uCam, dd);
          if (tt > 0.0 && tt < L - 1.2) {
            float R = mix(0.6, 3.2, tt / L);
            float rr = length(vOccW - (uCam + dd * tt));
            float fade = smoothstep(R, R * 0.55, rr) * uOn;
            const float BAYER[16] = float[16](0., 8., 2., 10., 12., 4., 14., 6., 3., 11., 1., 9., 15., 7., 13., 5.);
            int bi = int(mod(gl_FragCoord.x, 4.0)) + int(mod(gl_FragCoord.y, 4.0)) * 4;
            if (fade * 0.9 > (BAYER[bi] + 0.5) / 16.0) discard;
          }
        }`);
    };
    r('peak', peak, occ);
    const cap = new THREE.ConeGeometry(0.33, 0.42, 5); cap.translate(0, 0.21, 0);
    r('cap', cap, occ);
    const bump = new THREE.SphereGeometry(0.55, 7, 4, 0, Math.PI * 2, 0, Math.PI / 2);
    r('bump', bump);
    const reed = new THREE.CylinderGeometry(0.015, 0.02, 0.3, 3); reed.translate(0, 0.15, 0);
    r('reed', reed, leaf);
    const plate = new THREE.CylinderGeometry(0.9, 0.9, 0.03, 6); plate.translate(0, 0.015, 0);
    r('plate', plate, std(true, { transparent: true, opacity: 0.32, depthWrite: false }), false, false);
    // yapılar
    const box = new THREE.BoxGeometry(1, 1, 1); box.translate(0, 0.5, 0);
    r('box', box);
    this.windowMat = std(true, { emissive: new THREE.Color(0xffb85a), emissiveIntensity: 0 });
    r('house', box, this.windowMat);
    // Blender asset'leri (assets/models/*.glb): ad → geometri; '<ad>_hi' yakın plan sürümü
    this.assetMat = maskedVertexColors(std(true, { emissive: new THREE.Color(0xffb85a), emissiveIntensity: 0 })) as THREE.MeshStandardMaterial;
    for (const [name, g] of parseGlb(humanHouseGlb)) r(name, g, this.assetMat);
    const roof = new THREE.ConeGeometry(0.72, 0.5, 4); roof.rotateY(Math.PI / 4); roof.translate(0, 0.25, 0);
    r('roof', roof);
    const tower = new THREE.CylinderGeometry(0.5, 0.55, 1, 7); tower.translate(0, 0.5, 0);
    r('tower', tower);
    const spire = new THREE.ConeGeometry(0.6, 0.7, 7); spire.translate(0, 0.35, 0);
    r('spire', spire);
    const ring = new THREE.TorusGeometry(1, 0.06, 4, 24); ring.rotateX(Math.PI / 2);
    r('ring', ring);
    const post = new THREE.CylinderGeometry(0.03, 0.03, 1, 4); post.translate(0, 0.5, 0);
    r('post', post);
    const flag = new THREE.BoxGeometry(0.28, 0.16, 0.015); flag.translate(0.14, 0, 0);
    r('flag', flag, std(true, { side: THREE.DoubleSide }));
    const log = new THREE.CylinderGeometry(0.06, 0.06, 0.4, 6); log.rotateZ(Math.PI / 2);
    r('log', log);
    r('rock', M.rockGeo(false)); r('rock_hi', M.rockGeo(true));
    // ekin: sallanmaz (rüzgâr kayması örnek ölçeğiyle birleşince sıraları yana yatırıyordu)
    r('crop', M.cropGeo(false)); r('crop_hi', M.cropGeo(true));
    r('stake', post); r('stake_hi', M.stakeGeo());
    r('roofslab', M.roofSlabGeo());
    // yaban hayatı
    r('deer', M.deerGeo(false)); r('deer_m', M.deerGeo(true));
    r('rabbit', M.rabbitGeo()); r('wool', M.sheepGeo(), base, true, true);
    r('fish', M.fishGeo(), std(true, { metalness: 0.4, roughness: 0.35 }), false, false);
    r('bfly', M.butterflyGeo(), std(true, { side: THREE.DoubleSide }), false, false);
    r('ffly', new THREE.IcosahedronGeometry(0.012, 0), std(true, { color: 0x000000, emissive: new THREE.Color(0xd8ff7a), emissiveIntensity: 2.2 }), false, false);
    const crystal = new THREE.OctahedronGeometry(0.16, 0); crystal.scale(0.7, 1.6, 0.7); crystal.translate(0, 0.22, 0);
    r('crystal', crystal, std(true, { emissive: new THREE.Color(0x6a3dff), emissiveIntensity: 0.6, roughness: 0.3 }));
    r('glow', crystal, std(true, { emissive: new THREE.Color(0x8fe6f0), emissiveIntensity: 0.7, roughness: 0.3 }));
    const tent = new THREE.ConeGeometry(0.28, 0.4, 4); tent.translate(0, 0.2, 0);
    r('tent', tent);
    const disc = new THREE.CylinderGeometry(0.7, 0.7, 0.05, 6); disc.translate(0, 0.025, 0);
    r('disc', disc);
    const rdisc = new THREE.CylinderGeometry(0.7, 0.74, 0.05, 12, 1, false, 0, Math.PI * 2); rdisc.deleteAttribute('uv'); rdisc.translate(0, 0.025, 0);
    r('rdisc', rdisc, base, false, true);
    const road = new THREE.BoxGeometry(0.28, 0.04, 1); road.translate(0, 0.02, 0.5);
    r('road', road, base, false, true);
    const lantern = new THREE.BoxGeometry(0.1, 0.1, 0.1);
    r('lantern', lantern, std(true, { emissive: new THREE.Color(0xffc45a), emissiveIntensity: 1.2 }), false, false);
    const cave = new THREE.SphereGeometry(0.5, 6, 4, 0, Math.PI * 2, 0, Math.PI / 2);
    r('cave', cave);
    // figürler
    const body = new THREE.CylinderGeometry(0.09, 0.13, 0.26, 7); body.translate(0, 0.13, 0);
    r('body', body);
    const head = new THREE.IcosahedronGeometry(0.1, 1); head.translate(0, 0.34, 0);
    r('head', head);
    const cart = new THREE.BoxGeometry(0.34, 0.18, 0.22); cart.translate(0, 0.16, 0);
    r('cart', cart);
    const wheel = new THREE.CylinderGeometry(0.08, 0.08, 0.26, 8); wheel.rotateX(Math.PI / 2); wheel.translate(0, 0.08, 0);
    r('wheel', wheel);
    const puff = new THREE.IcosahedronGeometry(0.14, 0);
    r('puff', puff, std(true, { transparent: true, opacity: 0.8 }), false, false);
    registerFigures(r, std);
    const hull = new THREE.CylinderGeometry(0.5, 0.32, 1, 4, 1); hull.rotateY(Math.PI / 4); hull.scale(1, 1, 2.4); hull.translate(0, 0.5, 0);
    r('hull', hull);
    // gemi gövdesi: sivri pruva, dar omurga, pruva ve kıçta yükselen küpeşte (ileri = +z, taban y=0)
    const keel = new THREE.BoxGeometry(1, 1, 1, 2, 2, 8); keel.translate(0, 0.5, 0);
    { const P = keel.attributes.position as THREE.BufferAttribute;
      for (let i = 0; i < P.count; i++) {
        const x = P.getX(i), y = P.getY(i), z = P.getZ(i), zn = z * 2;
        const wz = zn > 0.15 ? Math.max(0, 1 - Math.pow((zn - 0.15) / 0.85, 1.7)) : zn < -0.75 ? 1 - ((-zn - 0.75) / 0.25) * 0.3 : 1;
        const wy = 0.4 + 0.6 * y;
        P.setX(i, x * wz * wy);
        P.setY(i, y + y * 0.3 * zn * zn + (1 - y) * 0.3 * Math.pow(zn, 4));
      }
      keel.computeVertexNormals(); }
    r('keel', keel);
    // yelken: rüzgârla pruvaya doğru şişen kare yelken (taban y=0)
    const sail = new THREE.PlaneGeometry(1, 1, 3, 3); sail.translate(0, 0.5, 0);
    { const P = sail.attributes.position as THREE.BufferAttribute;
      for (let i = 0; i < P.count; i++) { const x = P.getX(i), y = P.getY(i); P.setZ(i, 0.16 * (1 - 4 * x * x) * Math.sin(Math.PI * y)); }
      sail.computeVertexNormals(); }
    r('sail', sail, std(true, { side: THREE.DoubleSide }));
    // üçgen (Latin) yelken: keşif teknesi
    const lateen = new THREE.BufferGeometry();
    lateen.setAttribute('position', new THREE.Float32BufferAttribute([0, 0, -0.5, 0, 1, 0.1, 0, 0, 0.5, 0.06, 0.45, 0.02, 0, 0, -0.5, 0, 0, 0.5], 3));
    lateen.setIndex([0, 3, 1, 1, 3, 2, 0, 2, 3]);
    lateen.computeVertexNormals();
    r('lateen', lateen, std(true, { side: THREE.DoubleSide }));
    // palmiye yaprağı: gövdenin tepesinden dışa uzanıp sarkan yaprak
    const frond = new THREE.BufferGeometry();
    frond.setAttribute('position', new THREE.Float32BufferAttribute([0, 0, 0, 0.2, 0.06, -0.075, 0.2, 0.06, 0.075, 0.46, -0.14, 0], 3));
    frond.setIndex([0, 1, 3, 0, 3, 2, 1, 2, 3]);
    frond.computeVertexNormals();
    r('frond', frond, std(true, { side: THREE.DoubleSide }), true, false);
    const fire = new THREE.ConeGeometry(0.1, 0.32, 5); fire.translate(0, 0.16, 0);
    r('fire', fire, std(true, { color: 0xff7a1a, emissive: new THREE.Color(0xff5a0a), emissiveIntensity: 1.6 }), false, false);
    const bird = new THREE.BufferGeometry();
    bird.setAttribute('position', new THREE.Float32BufferAttribute([0, 0, 0.06, -0.22, 0.05, -0.02, 0, 0, -0.06, 0, 0, 0.06, 0, 0, -0.06, 0.22, 0.05, -0.02], 3));
    bird.computeVertexNormals();
    r('bird', bird, std(true, { side: THREE.DoubleSide }), false, false);
    const gable = new THREE.CylinderGeometry(0.5, 0.5, 1, 3); gable.rotateX(-Math.PI / 2); gable.translate(0, 0.25, 0);
    r('gable', gable);
    r('gablew', gable); // çatı kaplamasının altındaki duvar renginde alın: kışın beyazlamaz
    const dome = new THREE.SphereGeometry(0.5, 9, 5, 0, Math.PI * 2, 0, Math.PI / 2);
    r('dome', dome);
    const door = new THREE.BoxGeometry(0.1, 0.14, 0.02); door.translate(0, 0.07, 0);
    r('door', door, std(true, { emissive: new THREE.Color(0xffb85a), emissiveIntensity: 0 }));
    r('door0', door); // boş evin kapısı: ışıksız (eskiden 'box' kullanılıyordu → 1×1×1 dev kara kutu)
    const win = new THREE.BoxGeometry(0.07, 0.07, 0.014); win.translate(0, 0.035, 0);
    r('win', win, std(true, { color: 0x2a2a30, emissive: new THREE.Color(0xffb04a), emissiveIntensity: 0 }), false, false);
    r('winoff', win, std(true, { color: 0x1e1c1a }), false, false);
    r('win2', win, std(true, { color: 0x2a2a30, emissive: new THREE.Color(0xffa040), emissiveIntensity: 0 }), false, false);
    // kıyı köpüğü: yumuşak kenarlı, kalınlıksız şerit
    this.foamMat = std(true, { color: 0xffffff, transparent: true, opacity: 0.55, depthWrite: false, emissive: new THREE.Color(0x8fb0c0), emissiveIntensity: 0.3, map: M.foamTexture() ?? undefined, side: THREE.DoubleSide });
    r('foam', M.foamGeo(), this.foamMat, false, false);
    const tuft = new THREE.ConeGeometry(0.03, 0.11, 3); tuft.translate(0, 0.05, 0);
    r('tuft', tuft, leaf, false, true);
    const bloomG = new THREE.IcosahedronGeometry(0.022, 0);
    r('flower', bloomG, std(true), false, false);
    const dots = new THREE.IcosahedronGeometry(0.05, 0);
    r('dot', dots);
    // duman: yumuşak gölgeli, yarı saydam; yükselirken büyür, sonda erir (bkz. puffS)
    const smoke = new THREE.IcosahedronGeometry(0.08, 1);
    r('smoke', smoke, std(false, { transparent: true, opacity: 0.32, depthWrite: false, roughness: 1, emissive: new THREE.Color(0x303030) }), false, false);
    const halo = new THREE.TorusGeometry(0.2, 0.025, 4, 16); halo.rotateX(Math.PI / 2);
    r('halo', halo, std(true, { emissive: new THREE.Color(0xf2cf5b), emissiveIntensity: 1 }), false, false);
  }

  // ---------------------------------------------------------------- dünya
  setSim(sim: Sim) {
    this.sim = sim;
    this.lastTerrain = -999; this.lastDyn = -999; this.lastSeason = -1;
    for (const el of this.labels.values()) el.remove();
    this.labels.clear();
    for (const p of this.pops) p.el.remove();
    this.pops = []; this.battles = [];
    const W = sim.w.W, H = sim.w.H;
    this.center.set((SQ3 * (W + 0.5)) / 2, 0, (1.5 * (H - 1)) / 2);
    this.sun.target.position.copy(this.center);
    this.computeHeights();
    this.atmo.setBounds(this.center, SQ3 * (W + 0.5), 1.5 * H);
    this.groundSeason = -1;
    this.borderSig = '';
    this.tween = null;
    if (this.fly.on) this.fly.exit();
    this.controls.enabled = true;
    this.openingShot();
    this.controls.update();
  }
  /** açılış: en kalabalık kasabaya alçak eğik açı, arkada kıtanın içi ve dağlar (haritaya değil dünyaya bakış) */
  private openingShot() {
    const w = this.sim.w;
    const st = w.settlements.filter((q) => q.alive).sort((a, b) => this.sim.pop(b) - this.sim.pop(a))[0];
    if (!st) { this.controls.target.copy(this.center); this.camera.position.copy(this.viewPos('diorama')); return; }
    const [x, z] = this.pos(st.tile);
    const tg = new THREE.Vector3(x, this.height(st.tile), z);
    const away = new THREE.Vector3(x - this.center.x, 0, z - this.center.z);
    if (away.lengthSq() < 1) away.set(0, 0, 1);
    away.normalize();
    const polar = 1.2, dist = 15;
    const pos = tg.clone().addScaledVector(away, Math.sin(polar) * dist);
    pos.y += Math.cos(polar) * dist;
    this.controls.target.copy(tg);
    this.camera.position.copy(pos);
  }
  pos(i: number): [number, number] {
    const g = this.sim.g;
    const c = g.col(i), r = g.row(i);
    return [SQ3 * (c + 0.5 * (r & 1)), 1.5 * r];
  }
  height(i: number) {
    return this.sim.w.tiles[i].terrain === 'water' ? WATER_Y : this.hc[i] ?? this.th[i];
  }

  /** karo, köşe ve kenar ortası yükseklikleri (dünya başına bir kez) */
  private computeHeights() {
    const w = this.sim.w, n = w.tiles.length;
    this.th = new Float32Array(n);
    // kıyıya uzaklık: karada sahile doğru alçalma, denizde açığa doğru derinleşme
    const dist = new Int16Array(n).fill(-1);
    const q: number[] = [];
    for (let i = 0; i < n; i++) if (w.tiles[i].sea && this.sim.g.neighbors(i).some((j) => !w.tiles[j].sea)) { dist[i] = 0; q.push(i); }
    for (let i = 0; i < n; i++) if (!w.tiles[i].sea && this.sim.g.neighbors(i).some((j) => w.tiles[j].sea)) { dist[i] = 1; q.push(i); }
    for (let h = 0; h < q.length; h++) { const i = q[h]; for (const j of this.sim.g.neighbors(i)) if (dist[j] < 0 && !!w.tiles[j].sea === !!w.tiles[i].sea) { dist[j] = dist[i] + 1; q.push(j); } }
    this.coastDist = dist;
    for (let i = 0; i < n; i++) {
      const t = w.tiles[i];
      if (t.sea) { this.th[i] = -0.3 - Math.min(1.6, Math.max(0, dist[i]) * 0.32) + (hash(i, 13) - 0.5) * 0.08; continue; }
      this.th[i] = t.terrain === 'water' ? -0.25 - Math.max(0, 0.45 - t.elev) * 1.2 : 0.55 + Math.max(0, t.elev - 0.2) * ELEV_K + (TERRAIN_BONUS[t.terrain] ?? 0) + (hash(i, 13) - 0.5) * 0.05;
      const d = dist[i];
      if (t.terrain !== 'water' && d >= 1 && d <= 3) this.th[i] = Math.min(this.th[i], 0.42 + (d - 1) * 0.32 + (hash(i, 14) - 0.5) * 0.1 + (t.terrain === 'hill' || t.terrain === 'mountain' ? 0.35 : 0));
    }
    // göl ve nehir kıyısı: kara suya doğru vadi gibi alçalır (eskiden yüksek platodan suya dik taş duvar iniyordu)
    const wd = new Int16Array(n).fill(-1), wq: number[] = [];
    for (let i = 0; i < n; i++) if (!w.tiles[i].sea && w.tiles[i].terrain === 'water') { wd[i] = 0; wq.push(i); }
    for (let h = 0; h < wq.length; h++) { const i = wq[h]; if (wd[i] >= 3) continue; for (const j of this.sim.g.neighbors(i)) if (wd[j] < 0 && !w.tiles[j].sea && w.tiles[j].terrain !== 'water') { wd[j] = wd[i] + 1; wq.push(j); } }
    const VAL = [0, 0.3, 0.6, 0.85], LIFT = [0, 0.2, 0.36, 0.52];
    for (let i = 0; i < n; i++) { const d = wd[i]; if (d >= 1 && d <= 3) this.th[i] = Math.min(this.th[i], WATER_Y + LIFT[d] + (this.th[i] - WATER_Y) * VAL[d]); }
    const corner = new Map<string, number[]>(), mid = new Map<string, number[]>();
    const key = (x: number, z: number) => `${Math.round(x * 100)},${Math.round(z * 100)}`;
    for (let i = 0; i < n; i++) {
      const [cx, cz] = this.pos(i);
      for (let k = 0; k < 6; k++) {
        const [ax, az] = this.cornerOff(k), [mx, mz] = this.midOff(k);
        const kc = key(cx + ax, cz + az), km = key(cx + mx, cz + mz);
        (corner.get(kc) ?? corner.set(kc, []).get(kc)!).push(i);
        (mid.get(km) ?? mid.set(km, []).get(km)!).push(i);
      }
    }
    const avg = (ids: number[], expect: number) => (ids.reduce((a, j) => a + this.th[j], 0) + (expect - ids.length) * SEA_H) / expect;
    this.ch = new Float32Array(n * 6); this.mh = new Float32Array(n * 6);
    this.cornerIds = new Array(n * 6); this.midIds = new Array(n * 6);
    for (let i = 0; i < n; i++) {
      const [cx, cz] = this.pos(i);
      for (let k = 0; k < 6; k++) {
        const [ax, az] = this.cornerOff(k), [mx, mz] = this.midOff(k);
        const c = corner.get(key(cx + ax, cz + az))!, m = mid.get(key(cx + mx, cz + mz))!;
        this.ch[i * 6 + k] = avg(c, 3); this.mh[i * 6 + k] = avg(m, 2);
        this.cornerIds[i * 6 + k] = c; this.midIds[i * 6 + k] = m;
      }
    }
    this.buildHeightfield();
  }
  private coastDist: Int16Array = new Int16Array(0);
  private cornerIds: number[][] = [];
  private midIds: number[][] = [];
  private cornerOff(k: number): [number, number] { const a = (Math.PI / 180) * (30 + 60 * k); return [Math.cos(a), Math.sin(a)]; }
  private midOff(k: number): [number, number] { const a = (Math.PI / 180) * (60 * (k + 1)); return [Math.cos(a) * SQ3 / 2, Math.sin(a) * SQ3 / 2]; }
  /** kesintisiz zeminin (x,z) noktasındaki yüksekliği (çizilen ağla birebir) */
  groundY(x: number, z: number): number { return this.hf.sample(x, z); }
  /** eski hex-plato yüzeyi: karo içi düz alan + komşulara kaynaşan halka (yükseklik alanının tabanı) */
  private plateauY(x: number, z: number): number {
    const i = this.nearestTile(x, z);
    if (i < 0) return SEA_H;
    const [cx, cz] = this.pos(i);
    const dx = x - cx, dz = z - cz, r = Math.hypot(dx, dz), H = this.th[i];
    if (r < 1e-4) return H;
    const deg = ((Math.atan2(dz, dx) * 180) / Math.PI - 30 + 720) % 360;
    const k = Math.min(5, Math.floor(deg / 60)), t = (deg - k * 60) / 60;
    const rb = (SQ3 / 2) / Math.cos((t - 0.5) * (Math.PI / 3));
    const c0 = this.ch[i * 6 + k], c1 = this.ch[i * 6 + ((k + 1) % 6)], m = this.mh[i * 6 + k];
    const hb = t < 0.5 ? c0 + (m - c0) * t * 2 : m + (c1 - m) * (t - 0.5) * 2;
    const q = Math.min(1, Math.max(0, (r / rb - RIN) / (1 - RIN)));
    return H + (hb - H) * q;
  }
  private tileColor(i: number, season: number, fertile: Set<number>, out: THREE.Color) {
    const t = this.sim.w.tiles[i];
    const winter = season === 3, autumn = season === 2;
    if (t.terrain === 'water') {
      const depth = Math.min(1, Math.max(0, -this.th[i] / 0.8));
      return out.setHex(0xc9b98a).lerp(new THREE.Color(0x3d6a70), depth);
    }
    out.setHex(TERRAIN_COL[t.terrain]);
    if (fertile.has(i) && t.terrain === 'grass') out.lerp(new THREE.Color(0xc9c26a), 0.3);
    if (autumn && (t.terrain === 'grass' || t.terrain === 'forest')) out.lerp(new THREE.Color(0xc2a04a), 0.25);
    if (season === 0 && t.terrain === 'grass') out.lerp(new THREE.Color(0x9fd06a), 0.15);
    if (winter && t.terrain !== 'mountain') out.lerp(new THREE.Color(0xf1f5f6), t.terrain === 'tundra' ? 0 : 0.6);
    if (t.terrain === 'mountain' || t.terrain === 'hill') {
      const rock = Math.min(1, Math.max(0, (this.th[i] - 2.0) / 1.0));
      out.lerp(new THREE.Color(winter ? 0xf1f5f6 : 0x74706a), rock * 0.6);
    }
    return out.offsetHSL(0, 0, (hash(i, 1) - 0.5) * 0.045);
  }
  // ---------------------------------------------------------------- yükseklik alanı
  /**
   * Gerçek rölyef: hex-plato tabanı (düzlükte korunur) + yumuşak çekirdekli komşu ortalaması
   * + araziye göre gürültü: dağlarda sırtlı zirveler, tepelerde yuvarlak yamaçlar, ovada hafif dalga.
   * Yerleşimlerin, tarlaların ve madenlerin oturduğu karo ortası düzleşir (tezgâh).
   */
  private buildHeightfield() {
    const w = this.sim.w, g = this.sim.g, n = w.tiles.length;
    this.noise = new Noise2(((w.seed ?? 1) * 7919 + 17) >>> 0);
    const nz = this.noise;
    const nb = new Int32Array(n * 6).fill(-1);
    for (let i = 0; i < n; i++) g.neighbors(i).forEach((j, q) => { if (q < 6) nb[i * 6 + q] = j; });
    const keep = new Float32Array(n), amp = new Float32Array(n), mtn = new Float32Array(n), bench = new Float32Array(n);
    const wet = (j: number) => !!w.tiles[j].sea || w.tiles[j].terrain === 'water';
    for (let i = 0; i < n; i++) {
      const t = w.tiles[i];
      if (wet(i)) { keep[i] = 1; continue; }
      let nbW = false, inner = true;
      for (let q = 0; q < 6; q++) { const j = nb[i * 6 + q]; if (j < 0) continue; if (wet(j)) nbW = true; if (w.tiles[j].terrain !== 'mountain') inner = false; }
      switch (t.terrain) {
        // dağ: geçilmez kaya kütlesi; iç kısım biraz daha yüksek, sırtlı ama iğne gibi değil
        case 'mountain': keep[i] = 0; amp[i] = 1.25 + (inner ? 1.0 : 0) + Math.max(0, t.elev - 0.8) * 4; mtn[i] = 1; bench[i] = t.deposit >= 0 ? 0.92 : 0; break;
        case 'hill': keep[i] = 0.35; amp[i] = 0.5; mtn[i] = 0.25; bench[i] = t.deposit >= 0 ? 0.92 : 0.7; break;
        case 'oldforest': keep[i] = 0.5; amp[i] = 0.2; mtn[i] = 0.05; bench[i] = 0.6; break;
        case 'forest': keep[i] = 0.6; amp[i] = 0.15; bench[i] = 0.7; break;
        case 'tundra': keep[i] = 0.55; amp[i] = 0.18; mtn[i] = 0.08; bench[i] = 0.7; break;
        case 'swamp': keep[i] = 0.9; amp[i] = 0.04; bench[i] = 0.9; break;
        default: keep[i] = 0.7; amp[i] = 0.1; bench[i] = 0.85;
      }
      if (nbW) { keep[i] = Math.max(keep[i], 0.85); amp[i] *= 0.3; }
    }
    this.hfTile = { keep, amp, mtn, nb };
    const R2 = 1.8 * 1.8;
    const th = this.th;
    const minX = -3, maxX = SQ3 * (w.W + 0.5) + 3, minZ = -3, maxZ = 1.5 * (w.H - 1) + 3;
    /** tezgâh maskesi dışındaki ham yükseklik; i: en yakın karo */
    const raw = (x: number, z: number, i: number) => {
        const [cx, cz] = this.pos(i);
        let sw = 0, sh = 0, sk = 0, sa = 0, sm = 0;
        for (let q = -1; q < 6; q++) {
          const j = q < 0 ? i : nb[i * 6 + q];
          if (j < 0) continue;
          const [jx, jz] = q < 0 ? [cx, cz] : this.pos(j);
          const d2 = (jx - x) ** 2 + (jz - z) ** 2;
          if (d2 >= R2) continue;
          const wt = (1 - d2 / R2) ** 2;
          sw += wt; sh += wt * th[j]; sk += wt * keep[j]; sa += wt * amp[j]; sm += wt * mtn[j];
        }
        const smooth = sh / sw, kp = sk / sw, a = sa / sw, m = sm / sw;
        const pl = this.plateauY(x, z);
        let h = smooth + (pl - smooth) * kp;
        if (a > 0.002) {
          const roll = 0.5 + 0.5 * nz.fbm(x * 0.3, z * 0.3, 3);
          let rel = a * (1 - m) * roll;
          if (m > 0.001) {
            // dağ: geniş gövde (yumuşak) + sırtlar; tek köşelik iğne zirveler olmasın diye sırt keskinliği sınırlı
            const mass = 0.5 + 0.5 * nz.fbm(x * 0.085 + 40, z * 0.085 - 20, 3);
            const rid = nz.ridged(x * 0.15, z * 0.15, 4);
            rel += a * m * (0.25 + 0.55 * mass + 0.35 * rid * rid + 0.08 * roll);
          }
          // karo ortası tezgâh: yerleşim, tarla ve madenler düz zemine otursun
          const dx = x - cx, dz = z - cz, r = Math.hypot(dx, dz);
          let damp = 1;
          if (bench[i] > 0) {
            const deg = ((Math.atan2(dz, dx) * 180) / Math.PI - 30 + 720) % 360;
            const tt = (deg % 60) / 60;
            const rb = (SQ3 / 2) / Math.cos((tt - 0.5) * (Math.PI / 3));
            const u = Math.min(1, Math.max(0, (r / rb - 0.25) / 0.6));
            damp = 1 - bench[i] * (1 - u * u * (3 - 2 * u));
          }
          h += rel * damp + Math.min(0.045, a * 0.18) * nz.n(x * 1.15, z * 1.15);
        }
        return h;
    };
    // karo ortası tezgâhı: yumuşatılmış taban eğimini de düzle (yalnız gürültüyü sönümlemek yetmiyordu:
    // tepedeki kasabada evler ve tarlalar yamaçta asılı kalıyordu). Kenara doğru söner, komşuyla dikiş olmaz.
    const core = new Float32Array(n);
    for (let i = 0; i < n; i++) if (bench[i] > 0) { const [cx, cz] = this.pos(i); core[i] = raw(cx, cz, i); }
    this.hf.build({
      minX, maxX, minZ, maxZ, outside: SEA_H - 0.9,
      height: (x, z) => {
        const i = this.nearestTile(x, z);
        if (i < 0) return SEA_H - 0.9;
        const h = raw(x, z, i);
        if (bench[i] <= 0) return h;
        const [cx, cz] = this.pos(i);
        const dx = x - cx, dz = z - cz;
        const deg = ((Math.atan2(dz, dx) * 180) / Math.PI - 30 + 720) % 360;
        const rb = (SQ3 / 2) / Math.cos(((deg % 60) / 60 - 0.5) * (Math.PI / 3));
        const u = Math.min(1, Math.max(0, (Math.hypot(dx, dz) / rb - 0.5) / 0.42));
        return h + (core[i] - h) * bench[i] * (1 - u * u * (3 - 2 * u));
      },
    });
    this.hc = new Float32Array(n);
    for (let i = 0; i < n; i++) { const [x, z] = this.pos(i); this.hc[i] = this.hf.sample(x, z); }
  }
  private hfTile: { keep: Float32Array; amp: Float32Array; mtn: Float32Array; nb: Int32Array } | null = null;
  /** zemin malzemesi: kamera ile hedef arasına giren yüksek zemin (dağ) kafes deseniyle saydamlaşır */
  private setupGroundMat() {
    const m = this.groundMat;
    m.onBeforeCompile = (sh) => {
      sh.uniforms.uCam = this.occU.cam; sh.uniforms.uTgt = this.occU.tgt; sh.uniforms.uOn = this.occU.on;
      sh.vertexShader = 'varying vec3 vOccW;\n' + sh.vertexShader.replace('#include <project_vertex>', `#include <project_vertex>
        vOccW = (modelMatrix * vec4(transformed, 1.0)).xyz;`);
      sh.fragmentShader = 'uniform vec3 uCam; uniform vec3 uTgt; uniform float uOn; varying vec3 vOccW;\n' + sh.fragmentShader.replace('#include <clipping_planes_fragment>', `#include <clipping_planes_fragment>
        if (uOn > 0.0) {
          vec3 ab = uTgt - uCam; float L = length(ab); vec3 dd = ab / L;
          float tt = dot(vOccW - uCam, dd);
          if (tt > 0.0 && tt < L - 1.5) {
            vec3 lp = uCam + dd * tt;
            // yalnız bakış çizgisinin üstüne taşan zemin (öndeki dağ); ayak altındaki zemin delinmez
            if (vOccW.y > lp.y - 0.25) {
              float R = mix(0.8, 3.6, tt / L);
              float rr = length(vOccW - lp);
              float fade = smoothstep(R, R * 0.5, rr) * uOn;
              const float BAYER[16] = float[16](0., 8., 2., 10., 12., 4., 14., 6., 3., 11., 1., 9., 15., 7., 13., 5.);
              int bi = int(mod(gl_FragCoord.x, 4.0)) + int(mod(gl_FragCoord.y, 4.0)) * 4;
              if (fade * 0.9 > (BAYER[bi] + 0.5) / 16.0) discard;
            }
          }
        }`);
    };
  }
  /** zemin köşe rengi: karo renkleri yumuşak karışır; dik yamaç kaya, yüksek zirve kar, kıyı kum */
  private groundColor(x: number, z: number, h: number, season: number, tc: Float32Array, out: number[]) {
    const w = this.sim.w;
    const i = this.nearestTile(x, z);
    if (i < 0) { out[0] = 0.18; out[1] = 0.33; out[2] = 0.4; return; }
    const T = this.hfTile!;
    const [cx, cz] = this.pos(i);
    const R2 = 1.45 * 1.45;
    let sw = 0, r = 0, g = 0, b = 0, sm = 0;
    for (let q = -1; q < 6; q++) {
      const j = q < 0 ? i : T.nb[i * 6 + q];
      if (j < 0) continue;
      const [jx, jz] = q < 0 ? [cx, cz] : this.pos(j);
      const d2 = (jx - x) ** 2 + (jz - z) ** 2;
      if (d2 >= R2) continue;
      const wt = (1 - d2 / R2) ** 3;
      sw += wt; r += wt * tc[j * 3]; g += wt * tc[j * 3 + 1]; b += wt * tc[j * 3 + 2]; sm += wt * T.mtn[j];
    }
    r /= sw; g /= sw; b /= sw; sm /= sw;
    const t = w.tiles[i];
    const under = h < WATER_Y - 0.02;
    const nz = this.noise;
    // deniz dibi derinleştikçe yatak rengine döner: çizilen dip ile düz yatak arasında dikiş görünmesin
    const G = GC;
    if (under && h < -0.35) { const k = Math.min(1, (-0.35 - h) / 0.8); r += (G.bed.r - r) * k; g += (G.bed.g - g) * k; b += (G.bed.b - b) * k; }
    if (!under) {
      const winter = season === 3;
      const slope = this.hf.slopeAt(x, z);
      // dik yamaç kaya: dağda erken, ovada yalnız gerçekten dik yerde (göl kıyısı gri duvar olmasın)
      // dağ kütlesi baştan sona kaya (düz sırtı da çayır görünmesin); ovada yalnız gerçekten dik yer
      const rock = Math.max(Math.min(1, Math.max(0, (slope - (0.7 + (1 - sm) * 0.55)) / 0.7)), Math.min(0.85, Math.max(0, (sm - 0.35) * 1.4)));
      // dağ etekleri: yeşil-kahve çayır; yukarı çıktıkça kaya
      if (sm > 0.05 && !winter) { const alp = Math.min(1, Math.max(0, (4.1 - h) / 1.5)) * sm * (1 - rock) * 0.65; r += (G.alp.r - r) * alp; g += (G.alp.g - g) * alp; b += (G.alp.b - b) * alp; }
      if (rock > 0) {
        const warm = 0.5 + 0.5 * nz.n(x * 0.21, z * 0.21);
        const rr = G.rockC.r + (G.rockW.r - G.rockC.r) * warm, rg = G.rockC.g + (G.rockW.g - G.rockC.g) * warm, rb2 = G.rockC.b + (G.rockW.b - G.rockC.b) * warm;
        const k = rock * 0.85; r += (rr - r) * k; g += (rg - g) * k; b += (rb2 - b) * k;
      }
      // kar sınırı: kışın aşağı iner; dik kaya yüzünde kar tutmaz
      const line = winter ? 2.7 : 4.4;
      const sk = Math.min(1, Math.max(0, (h + nz.n(x * 0.5, z * 0.5) * 0.7 - line) / 0.9)) * (1 - rock * 0.55);
      if (sk > 0) { r += (G.snow.r - r) * sk; g += (G.snow.g - g) * sk; b += (G.snow.b - b) * sk; }
      // kıyı kumu: su seviyesine yakın düz kara
      const beach = t.terrain !== 'swamp' && t.terrain !== 'tundra' && !winter ? Math.max(0, 1 - Math.abs(h - (WATER_Y + 0.05)) / 0.13) * (1 - rock) : 0;
      if (beach > 0) { const k = beach * 0.7; r += (G.sand.r - r) * k; g += (G.sand.g - g) * k; b += (G.sand.b - b) * k; }
    }
    const v = 1 + nz.n(x * 0.8, z * 0.8) * 0.045;
    out[0] = r * v; out[1] = g * v; out[2] = b * v;
  }
  /** kesintisiz zemin ağı: yükseklik alanından parça parça (ekran dışı parçalar çizilmez) */
  private tileCols(season: number): Float32Array {
    const w = this.sim.w, n = w.tiles.length;
    const fertile = new Set<number>();
    for (const d of w.deposits) if (d.kind === 'fertile') for (const t of d.tiles) fertile.add(t);
    const tc = new Float32Array(n * 3);
    const c = new THREE.Color();
    for (let i = 0; i < n; i++) { this.tileColor(i, season, fertile, c); tc[i * 3] = c.r; tc[i * 3 + 1] = c.g; tc[i * 3 + 2] = c.b; }
    return tc;
  }
  /** mevsim başına köşe renkleri: bir kez hesaplanır, sonra mevsim değişimi yalnız kopyalamadır (hızlı akışta takılma olmasın) */
  private seasonCols = new Map<number, (Float32Array | null)[]>();
  /** renklerin hesaplandığı andaki karo arazisi: kesim/yeniden ağaçlanma olunca ilgili parçalar yeniden boyanır */
  private colSig = new Uint8Array(0);
  private static TCODE: Record<string, number> = { grass: 1, forest: 2, oldforest: 3, hill: 4, mountain: 5, water: 6, swamp: 7, tundra: 8 };
  /** arazisi değişen karoların üstündeki zemin parçalarını yeniden boya (öbür mevsimlerin önbelleği o parçalar için düşer) */
  private checkTerrainColors() {
    const w = this.sim.w;
    if (this.colSig.length !== w.tiles.length || this.groundFor !== this.sim) return;
    const changed: number[] = [];
    for (let i = 0; i < w.tiles.length; i++) { const c = Diorama.TCODE[w.tiles[i].terrain] ?? 0; if (c !== this.colSig[i]) { this.colSig[i] = c; changed.push(i); } }
    if (!changed.length) return;
    const dirty = new Set<number>();
    const bb = new THREE.Box3();
    this.groundMeshes.forEach((m, k) => {
      bb.copy(m.geometry.boundingBox!).expandByScalar(2.4);
      for (const i of changed) { const [x, z] = this.pos(i); if (x >= bb.min.x && x <= bb.max.x && z >= bb.min.z && z <= bb.max.z) { dirty.add(k); break; } }
    });
    this.warm = null; // yarım kalan ısınma eski renklerle olurdu
    for (const arr of this.seasonCols.values()) for (const k of dirty) arr[k] = null;
    const season = this.sim.season, tc = this.tileCols(season);
    let arr = this.seasonCols.get(season);
    if (!arr) { arr = this.groundMeshes.map(() => null); this.seasonCols.set(season, arr); }
    for (const k of dirty) {
      const m = this.groundMeshes[k], c = this.colorChunk(m, season, tc);
      arr[k] = c;
      const at = m.geometry.getAttribute('color') as THREE.BufferAttribute; (at.array as Float32Array).set(c); at.needsUpdate = true;
    }
    this.warmSeasons();
  }
  private warm: { season: number; k: number; tc: Float32Array; out: (Float32Array | null)[]; sim: Sim } | null = null;
  private colorChunk(m: THREE.Mesh, season: number, tc: Float32Array): Float32Array {
    const pa = (m.geometry.getAttribute('position') as THREE.BufferAttribute).array as Float32Array;
    const out = new Float32Array(pa.length), cc = [0, 0, 0];
    for (let v = 0; v < pa.length; v += 3) { this.groundColor(pa[v], pa[v + 2], pa[v + 1], season, tc, cc); out[v] = cc[0]; out[v + 1] = cc[1]; out[v + 2] = cc[2]; }
    return out;
  }
  /** boşta kalan zamanda diğer mevsimlerin renklerini parça parça hazırla */
  private warmSeasons() {
    if (this.warm || typeof window === 'undefined') return;
    // önce sıradaki mevsim; eksik parçası olan mevsim de tamamlanır
    const s0 = this.sim.season;
    const next = [1, 2, 3, 0].map((q) => (s0 + q) % 4).find((q) => { const a = this.seasonCols.get(q); return !a || a.some((c) => !c); });
    if (next === undefined) return;
    const prev = this.seasonCols.get(next);
    const wm0 = { season: next, k: 0, tc: this.tileCols(next), out: prev ? prev.slice() : this.groundMeshes.map(() => null) as (Float32Array | null)[], sim: this.sim };
    this.warm = wm0;
    const tick = () => {
      const wm = this.warm;
      if (wm !== wm0 || wm.sim !== this.sim) return;
      const t0 = performance.now();
      while (wm.k < this.groundMeshes.length && performance.now() - t0 < 6) { if (!wm.out[wm.k]) wm.out[wm.k] = this.colorChunk(this.groundMeshes[wm.k], wm.season, wm.tc); wm.k++; }
      if (wm.k >= this.groundMeshes.length) { this.seasonCols.set(wm.season, wm.out); this.warm = null; this.warmSeasons(); return; }
      setTimeout(tick, 30);
    };
    setTimeout(tick, 400);
  }
  /** kesintisiz zemin ağı: yükseklik alanından parça parça (ekran dışı parçalar çizilmez) */
  private buildGround() {
    const season = this.sim.season;
    if (this.groundFor === this.sim && this.groundMeshes.length) {
      let arr = this.seasonCols.get(season);
      if (!arr) { arr = this.groundMeshes.map(() => null); this.seasonCols.set(season, arr); }
      let tc: Float32Array | null = null;
      for (let k = 0; k < this.groundMeshes.length; k++) if (!arr[k]) arr[k] = this.colorChunk(this.groundMeshes[k], season, tc ??= this.tileCols(season));
      this.groundMeshes.forEach((m, k) => { const c = m.geometry.getAttribute('color') as THREE.BufferAttribute; (c.array as Float32Array).set(arr![k]!); c.needsUpdate = true; });
    } else {
      for (const m of this.groundMeshes) { m.geometry.dispose(); this.ground.remove(m); }
      this.seasonCols.clear(); this.warm = null;
      const tc = this.tileCols(season);
      // açık denizin dibi çizilmez (su ve deniz yatağı düzlemi örter)
      const deep = SEA_H - 0.3;
      this.groundMeshes = this.hf.buildChunks(this.groundMat, (x, z, h, out) => this.groundColor(x, z, h, season, tc, out), (a, b, d) => a < deep && b < deep && d < deep);
      for (const m of this.groundMeshes) this.ground.add(m);
      this.seasonCols.set(season, this.groundMeshes.map((m) => ((m.geometry.getAttribute('color') as THREE.BufferAttribute).array as Float32Array).slice()));
      this.colSig = Uint8Array.from(this.sim.w.tiles, (t) => Diorama.TCODE[t.terrain] ?? 0);
      this.groundFor = this.sim;
    }
    this.groundSeason = season;
    this.warmSeasons();
    if (this.gridFor !== this.sim) this.buildGrid();
  }
  private gridFor: Sim | null = null;
  private groundFor: Sim | null = null;
  private buildGrid() {
    const w = this.sim.w, pts: number[] = [];
    for (let i = 0; i < w.tiles.length; i++) {
      // su (deniz ve göl) üstüne ızgara çizilmez: göllerde karo deseni gibi duruyordu
      const wet = (j: number) => w.tiles[j].sea || w.tiles[j].terrain === 'water';
      if (wet(i)) continue;
      const [cx, cz] = this.pos(i);
      for (let k = 0; k < 6; k++) {
        const nb = this.midIds[i * 6 + k];
        if (nb.length > 1 && Math.min(...nb.filter((j) => !wet(j))) !== i) continue;
        // hex kenarı kafes çizgisinin üstünde: üçte birlik noktalar zemin köşeleriyle çakışır, çizgi zemine tam oturur
        const [ax, az] = this.cornerOff(k), [bx2, bz2] = this.cornerOff((k + 1) % 6);
        const N = this.hf.N;
        for (let q = 0; q < N; q++) {
          const x0 = cx + ax + (bx2 - ax) * (q / N), z0 = cz + az + (bz2 - az) * (q / N);
          const x1 = cx + ax + (bx2 - ax) * ((q + 1) / N), z1 = cz + az + (bz2 - az) * ((q + 1) / N);
          pts.push(x0, Math.max(this.groundY(x0, z0), WATER_Y) + 0.025, z0, x1, Math.max(this.groundY(x1, z1), WATER_Y) + 0.025, z1);
        }
      }
    }
    const geo = new THREE.BufferGeometry();
    geo.setAttribute('position', new THREE.Float32BufferAttribute(pts, 3));
    this.grid.geometry.dispose();
    this.grid.geometry = geo;
    this.gridFor = this.sim;
  }
  /** medeniyet sınırları: sahip karonun iç tarafına yatan renkli şerit */
  private borderSig = '';
  private buildBorders() {
    const s = this.sim, w = s.w;
    let sig = 0;
    for (let i = 0; i < w.tiles.length; i++) { const o = w.tiles[i].owner; if (o >= 0) sig = (sig * 31 + i * 7 + (s.settlement(o)?.civ ?? 9)) | 0; }
    const key = `${w.seed ?? ''}:${sig}`;
    if (key === this.borderSig) return;
    this.borderSig = key;
    const civOf = (i: number) => { const o = w.tiles[i].owner; if (o < 0) return -1; const st = s.settlement(o); return st ? st.civ : -1; };
    const pos: number[] = [], col: number[] = [];
    const c = new THREE.Color(), cd = new THREE.Color();
    // iki katman: kenarda ince, koyu-doygun çizgi + içeri doğru sönen renk tülü
    const LINE = 0.07, FADE = 0.34;
    const band = (cx: number, cz: number, ax: number, az: number, bx: number, bz: number, w0: number, w1: number, a0: number, a1: number, cc: THREE.Color) => {
      const P = [[cx + ax * (1 - w0), cz + az * (1 - w0)], [cx + bx * (1 - w0), cz + bz * (1 - w0)], [cx + bx * (1 - w1), cz + bz * (1 - w1)], [cx + ax * (1 - w1), cz + az * (1 - w1)]];
      const A = [a0, a0, a1, a1];
      const Y = P.map(([x, z]) => Math.max(this.groundY(x, z), WATER_Y) + 0.05);
      for (const t of [0, 1, 2, 0, 2, 3]) { pos.push(P[t][0], Y[t], P[t][1]); col.push(cc.r, cc.g, cc.b, A[t]); }
    };
    for (let i = 0; i < w.tiles.length; i++) {
      const cv = civOf(i);
      if (cv < 0) continue;
      c.set(w.civs[cv].color);
      cd.copy(c).multiplyScalar(0.78);
      const [cx, cz] = this.pos(i);
      for (let k = 0; k < 6; k++) {
        const nb = this.midIds[i * 6 + k].find((q) => q !== i);
        if (nb !== undefined && civOf(nb) === cv) continue;
        const [c0x, c0z] = this.cornerOff(k), [c1x, c1z] = this.cornerOff((k + 1) % 6);
        const NS = 6; // engebeli zeminde şerit havada kalmasın/gömülmesin: kenar altıya bölünür
        const seg: [number, number][] = [];
        for (let q = 0; q <= NS; q++) seg.push([c0x + (c1x - c0x) * (q / NS), c0z + (c1z - c0z) * (q / NS)]);
        for (let q = 0; q < NS; q++) {
          const [ax, az] = seg[q], [bx, bz] = seg[q + 1];
          band(cx, cz, ax, az, bx, bz, 0, LINE, 0.95, 0.95, cd);
          band(cx, cz, ax, az, bx, bz, LINE, FADE, 0.42, 0, c);
        }
      }
    }
    const geo = new THREE.BufferGeometry();
    geo.setAttribute('position', new THREE.Float32BufferAttribute(pos, 3));
    geo.setAttribute('color', new THREE.Float32BufferAttribute(col, 4));
    this.borders.geometry.dispose();
    this.borders.geometry = geo;
  }

  // ---------------------------------------------------------------- yürüyüş çarpışması (gezgin)
  /** evler, atölyeler, han: daire (x, z, r); surlar: halka + kapı boşlukları */
  private colC: number[] = [];
  private colGrid = new Map<number, number[]>();
  private colWalls: { x: number; z: number; R: number; gates: number[]; t: number }[] = [];
  private buildColliders() {
    this.colC = []; this.colGrid.clear();
    const add = (x: number, z: number, r: number) => {
      const id = this.colC.length / 3;
      this.colC.push(x, z, r);
      for (let gx = Math.floor((x - r) / 2); gx <= Math.floor((x + r) / 2); gx++) for (let gz = Math.floor((z - r) / 2); gz <= Math.floor((z + r) / 2); gz++) {
        const k = gx * 4096 + gz; let a = this.colGrid.get(k); if (!a) this.colGrid.set(k, a = []); a.push(id);
      }
    };
    for (const lay of this.layouts.values()) {
      for (const h of lay.houses) add(h.x, h.z, h.kind === 'stonehouse' ? 0.2 : h.kind === 'hut' ? 0.12 : 0.16);
      for (const [k, spots] of Object.entries(lay.work)) if (k !== 'plaza' && k !== 'site') for (const [px, pz] of spots) add(px, pz, 0.2);
    }
    for (const inn of this.sim.w.inns) if (inn.alive) { const [x, z] = this.pos(inn.tile); add(x, z, 0.3); }
  }
  /** yürüyen gezgini yapıların dışına it (kapıdan girilir) */
  private collide(x: number, z: number, px: number, pz: number): [number, number] {
    const B = 0.09;
    for (let it = 0; it < 2; it++) {
      const cell = this.colGrid.get(Math.floor(x / 2) * 4096 + Math.floor(z / 2));
      if (cell) for (const id of cell) {
        const cx = this.colC[id * 3], cz = this.colC[id * 3 + 1], r = this.colC[id * 3 + 2] + B;
        const dx = x - cx, dz = z - cz, d2 = dx * dx + dz * dz;
        if (d2 < r * r) { const d = Math.sqrt(d2) || 1e-4; x = cx + (dx / d) * r; z = cz + (dz / d) * r; }
      }
    }
    for (const wl of this.colWalls) {
      const dx = x - wl.x, dz = z - wl.z, d = Math.hypot(dx, dz);
      const band = wl.t + B;
      if (Math.abs(d - wl.R) > band || d < 1e-4) continue;
      const a = Math.atan2(dz, dx);
      if (wl.gates.some((g) => Math.abs(Math.atan2(Math.sin(a - g), Math.cos(a - g))) < 0.2 - B / wl.R)) continue;
      const inside = Math.hypot(px - wl.x, pz - wl.z) < wl.R;
      const nd = inside ? wl.R - band : wl.R + band;
      x = wl.x + (dx / d) * nd; z = wl.z + (dz / d) * nd;
    }
    return [x, z];
  }
  private tmpC = new THREE.Color();
  private flash = 0;
  private flash2 = 0;
  private nextBolt = 0;
  /** gök gürültüsü: (gecikme sn, şiddet 0–1) */
  onThunder?: (delay: number, power: number) => void;
  /** rüzgâr: zaman ve şiddet (yağmurda artar) */
  private windU = { t: { value: 0 }, a: { value: 1 } };
  private occU = { cam: { value: new THREE.Vector3() }, tgt: { value: new THREE.Vector3() }, on: { value: 1 } };
  /**
   * Ormandaki açıklıklar: karo → kalan ağaç oranı. Yerleşim ve kamp karosu tamamen açık, çevresi seyrek;
   * han ve avlusu, çıkarma yapıları ve yollar da açılır (ağaçlar içindekini yutmasın).
   */
  private clearings(): Map<number, number> {
    const w = this.sim.w, g = this.sim.g, m = new Map<number, number>();
    const set = (i: number, v: number) => { if (i >= 0 && v < (m.get(i) ?? 1)) m.set(i, v); };
    for (const st of w.settlements) if (st.alive) { set(st.tile, 0); for (const q of g.neighbors(st.tile)) set(q, st.tier >= 1 ? 0 : 0.35); if (st.tier >= 2) for (const q of g.within(st.tile, 2)) set(q, 0.45); }
    for (const cp of w.camps) if (cp.alive) { set(cp.tile, 0); for (const q of g.neighbors(cp.tile)) set(q, 0.45); }
    for (const inn of w.inns) if (inn.stage !== 'road') { set(inn.tile, 0); for (const q of g.neighbors(inn.tile)) set(q, 0.3); }
    for (let i = 0; i < w.tiles.length; i++) { const t = w.tiles[i]; if (t.ext) set(i, t.ext.kind === 'lumber' ? 0.4 : 0.15); else if (t.road) set(i, 0.55); }
    return m;
  }
  /** açıklıklar değişince (yeni kamp, han, köy büyüdü) orman yeniden dizilir */
  private clearKey(): string {
    const w = this.sim.w;
    let k = '';
    for (const cp of w.camps) if (cp.alive) k += cp.tile + ',';
    k += '|';
    for (const inn of w.inns) if (inn.stage !== 'road') k += inn.tile + ',';
    k += '|';
    for (const st of w.settlements) if (st.alive) k += st.tile + ':' + Math.min(2, st.tier) + ',';
    return k;
  }
  private lastClearKey = '';
  private buildTerrain() {
    this.lastClearKey = this.clearKey();
    // zemin yüksekliği artık yükseklik alanından O(1) okunuyor: eski önbellek gereksiz (ve daha yavaştı)
    this.buildTerrain0();
    this.checkTerrainColors();
  }
  private buildTerrain0() {
    const w = this.sim.w;
    const L = this.terrain;
    const season = this.sim.season;
    const winter = season === 3, autumn = season === 2;
    if (this.groundSeason !== season) this.buildGround();
    const town = new Set<number>();
    for (const st of w.settlements) if (st.alive) { town.add(st.tile); if (st.tier >= 1) for (const q of this.sim.g.neighbors(st.tile)) town.add(q); }
    const keepT = this.clearings();
    const isleKind = new Map((w.isles ?? []).map((x) => [x.id, x.kind]));
    L.begin();
    for (let i = 0; i < w.tiles.length; i++) {
      const t = w.tiles[i];
      const [x, z] = this.pos(i);
      const h = this.hc[i];
      const r1 = hash(i, 2), r2 = hash(i, 3);
      switch (t.terrain) {
        case 'forest': case 'oldforest': {
          if (town.has(i)) break;
          const old = t.terrain === 'oldforest';
          const dense = t.wood > 0 || old ? 1 : 0.5;
          // orman: yerleşim, kamp, han, iş yeri ve yol çevresi açıklık (ağaçlar kampı ve köyü yutmasın)
          const keep = keepT.get(i) ?? 1;
          const n = Math.round((old ? 7 : 5.5) * dense) + (r1 > 0.6 ? 1 : 0);
          for (let k = 0; k < n; k++) {
            const a = hash(i, 10 + k) * Math.PI * 2, d = 0.12 + hash(i, 20 + k) * 0.78;
            const tx = x + Math.cos(a) * d, tz = z + Math.sin(a) * d;
            const ty = this.groundY(tx, tz);
            if (ty < WATER_Y + 0.05) continue;
            if (hash(i, 200 + k) >= keep) {
              // kesilmiş ağacın kütüğü (kamp ve kasabanın tam ortasında kütük de yok)
              if (keep > 0.05 && hash(i, 210 + k) < 0.5) L.add('trunk', tx, ty - 0.02, tz, 1.3, 0.3, 1.3, 0, 0x7a5a3a);
              continue;
            }
            const young = hash(i, 35 + k) < 0.22;
            // boylar eskisinin ~%75'i: orman okunur kalsın, içindeki kamp ve köy görünsün
            const s = young ? 0.38 + hash(i, 30 + k) * 0.2 : (old ? 0.9 : 0.62) + hash(i, 30 + k) * 0.32;
            const lift = young ? 0.14 : 0.32;
            L.add('trunk', tx, ty - 0.02, tz, s, s * (young ? 1 : 1.45), s, 0, jitter(0x6b4a2b, hash(i, 37 + k), 0.08));
            const leaf = jitter(old ? 0x2f6b3d : autumn && hash(i, 40 + k) > 0.5 ? (hash(i, 45 + k) > 0.5 ? 0xd08a2e : 0xc2622a) : 0x3f8a3f, hash(i, 60 + k), 0.09, 0.03);
            if (hash(i, 50 + k) > 0.45 || old) L.add('tree', tx, ty + lift * s, tz, s, s * (young ? 1 : 1.1), s, a, winter ? 0xdde8e2 : leaf);
            else L.add('blob', tx, ty + (lift - 0.06) * s, tz, s * 0.95, s, s * 0.95, a, winter ? 0xdde8e2 : leaf);
          }
          // orman tabanı: çalı ve eğrelti (açıklıkta seyrek)
          for (let k = 0; k < (keep < 0.5 ? 1 : old ? 3 : 2); k++) { const tx = x + (hash(i, 140 + k) - 0.5) * 1.4, tz = z + (hash(i, 150 + k) - 0.5) * 1.4, ty = this.groundY(tx, tz); if (ty > WATER_Y + 0.05) L.add('bush', tx, ty - 0.02, tz, 0.8, 0.7, 0.8, k, winter ? 0xd5ddd6 : jitter(0x4a7a3a, hash(i, 160 + k), 0.1)); }
          break;
        }
        case 'grass': {
          // adalarda palmiye: kumsal adacıklarda bol, diğer adaların kıyısında seyrek
          if (t.isle && !town.has(i) && season !== 3) {
            const ik = isleKind.get(t.isle);
            const coastal = this.sim.g.neighbors(i).some((q) => w.tiles[q].sea);
            const np = ik === 'kumsal' ? 2 + (r1 > 0.5 ? 1 : 0) : coastal && r2 < 0.45 && ik !== 'kayalik' ? 1 : 0;
            for (let k = 0; k < np; k++) {
              const a0 = hash(i, 170 + k) * 6.28, d = 0.35 + hash(i, 175 + k) * 0.5;
              const tx = x + Math.cos(a0) * d, tz = z + Math.sin(a0) * d, ty = this.groundY(tx, tz);
              if (ty < WATER_Y + 0.03) continue;
              const hgt = 2.6 + hash(i, 180 + k) * 1.4, lean = 0.18 + hash(i, 185 + k) * 0.22, yaw = hash(i, 190 + k) * 6.28, H0 = 0.25 * 0.8 * hgt;
              L.add('trunk', tx, ty - 0.02, tz, 0.8, 0.8 * hgt, 0.8, yaw, 0x8a6a45, lean);
              const topx = tx + H0 * Math.sin(lean) * Math.sin(yaw), topz = tz + H0 * Math.sin(lean) * Math.cos(yaw), topy = ty + H0 * Math.cos(lean);
              for (let f = 0; f < 6; f++) L.add('frond', topx, topy, topz, 1.1, 1, 1.1, f * 1.05 + yaw, jitter(0x3f8a3f, hash(i, 200 + f), 0.1), 0, 0.12);
              L.add('dot', topx, topy - 0.03, topz, 0.9, 0.9, 0.9, 0, 0x6b4a2b);
            }
          }
          if (!town.has(i)) {
            // çimen öbekleri ve mevsim çiçekleri
            const tc = winter ? 0xc9d2c4 : autumn ? 0x9a9a4a : 0x5f9a45;
            for (let k = 0; k < 7; k++) {
              const tx = x + (hash(i, 90 + k) - 0.5) * 1.4, tz = z + (hash(i, 100 + k) - 0.5) * 1.4, ty = this.groundY(tx, tz);
              if (ty < WATER_Y + 0.04) continue;
              const cl = 0.7 + hash(i, 110 + k) * 0.6;
              for (let j = 0; j < 3; j++) L.add('tuft', tx + (j - 1) * 0.03, ty, tz + (j % 2) * 0.025, cl, cl * (0.8 + j * 0.2), cl, j, tc, (j - 1) * 0.35);
            }
            if (season <= 1) {
              const fc = [0xf2e25a, 0xffffff, 0xd96a8a, 0x9a7ad9][Math.floor(r2 * 4)];
              const nf = r1 > 0.5 ? 6 : 2;
              for (let k = 0; k < nf; k++) { const tx = x + (hash(i, 120 + k) - 0.5) * 1.3, tz = z + (hash(i, 130 + k) - 0.5) * 1.3, ty = this.groundY(tx, tz); if (ty > WATER_Y + 0.04) L.add('flower', tx, ty + 0.06, tz, 1, 1, 1, 0, fc); }
            }
          }
        }
          if (r1 > 0.9 && (keepT.get(i) ?? 1) >= 0.5) { const tx = x + (r2 - 0.5) * 1.1, tz = z + (hash(i, 4) - 0.5) * 1.1; L.add('blob', tx, this.groundY(tx, tz), tz, 0.7, 0.8, 0.7, r1 * 6, winter ? 0xdde8e2 : 0x4f8f45); }
          break;
        case 'tundra':
          if ((keepT.get(i) ?? 1) < 0.5) break;
          for (let k = 0; k < (r1 > 0.6 ? 2 : r1 > 0.35 ? 1 : 0); k++) { const tx = x + (hash(i, 60 + k) - 0.5) * 1.2, tz = z + (hash(i, 70 + k) - 0.5) * 1.2; L.add('pine', tx, this.groundY(tx, tz), tz, 0.9, 0.9 + r2 * 0.4, 0.9, 0, 0x3d6b4f); }
          break;
        case 'mountain': {
          // dağın kendisi artık zemin (sırtlar, zirveler, kar); üstüne yalnız çam, kaya ve moloz
          const rockCol = jitter(hash(i, 12) > 0.5 ? 0x7f7a75 : 0x767b80, hash(i, 15), 0.08, 0.02);
          // geçilmez kaya kütlesi: çam yalnız eteklerde (dağ olmayan komşusu olan karoda), içeride kaya ve moloz
          const edge = this.sim.g.neighbors(i).some((q) => w.tiles[q].terrain !== 'mountain');
          for (let k = 0; k < 7; k++) {
            const tx = x + (hash(i, 60 + k) - 0.5) * 1.6, tz = z + (hash(i, 70 + k) - 0.5) * 1.6;
            const ty = this.groundY(tx, tz), sl = this.hf.slopeAt(tx, tz);
            if (ty < WATER_Y + 0.05) continue;
            if (edge && k < 3 && ty < 3.3 - hash(i, 80 + k) * 0.6 && sl < 1.1) { const ps = 0.75 + hash(i, 90 + k) * 0.5; L.add('pine', tx, ty - 0.03, tz, ps, ps * (0.9 + r2 * 0.4), ps, k, winter ? 0xd5e2dc : jitter(0x35604a, hash(i, 95 + k), 0.08)); }
            else if (hash(i, 100 + k) > 0.65 && sl < 1.6) { const rs = 0.8 + hash(i, 110 + k) * 0.9; L.add('rock', tx, ty - 0.04 - sl * 0.05 * rs, tz, rs, rs * 0.7, rs, hash(i, 120 + k) * 6, jitter(rockCol, hash(i, 130 + k), 0.08)); }
          }
          break;
        }
        case 'hill':
          if (r1 > 0.55) {
            // kaya öbeği: bir iri + bir-iki küçük, ton ve biçim çeşitli
            const tx = x + (r2 - 0.5) * 0.8, tz = z + (hash(i, 4) - 0.5) * 0.8, rc = jitter(0x9a948a, hash(i, 5), 0.1, 0.03);
            const big = 1.6 + hash(i, 6) * 0.9;
            L.add('rock', tx, this.groundY(tx, tz) + 0.03, tz, big, big * (0.55 + hash(i, 7) * 0.35), big * (0.8 + hash(i, 8) * 0.3), r1 * 6, rc, (hash(i, 9) - 0.5) * 0.4);
            for (let k = 0; k < (r1 > 0.8 ? 2 : 1); k++) { const a = hash(i, 20 + k) * 6.28, px = tx + Math.cos(a) * 0.32, pz = tz + Math.sin(a) * 0.32, sm = 0.6 + hash(i, 22 + k) * 0.5; L.add('rock', px, this.groundY(px, pz) + 0.01, pz, sm, sm * 0.7, sm, a, jitter(rc, hash(i, 24 + k), 0.08)); }
          }
          if (r2 > 0.6) { const tx = x - 0.3, tz = z + 0.35; L.add('blob', tx, this.groundY(tx, tz), tz, 0.6, 0.7, 0.6, r1 * 6, winter ? 0xdde8e2 : 0x6a8a40); }
          break;
        case 'swamp':
          // bataklık gölcükleri: yuvarlak, düzensiz boyutlu (eskiden altıgen levhalar)
          L.add('rdisc', x + (r1 - 0.5) * 0.4, h + 0.01, z + (r2 - 0.5) * 0.4, 0.5 + r2 * 0.2, 0.3, 0.36 + r1 * 0.14, r1 * 6, winter ? 0xcfe0ea : 0x4f6f68);
          if (r2 > 0.45) { const px = x - (r1 - 0.5) * 0.9 - 0.25, pz = z + (r2 - 0.5) * 0.7 + 0.3; L.add('rdisc', px, this.groundY(px, pz) + 0.01, pz, 0.24, 0.3, 0.18, r2 * 6, winter ? 0xcfe0ea : 0x4a6a64); }
          // saz öbekleri: üçlü, hafif eğik; bazılarının ucunda kahverengi püskül
          for (let k = 0; k < 4; k++) {
            const tx = x + (hash(i, 60 + k) - 0.5) * 1.3, tz = z + (hash(i, 70 + k) - 0.5) * 1.3, ty = this.groundY(tx, tz);
            const rc = winter ? 0xb8b08a : jitter(0x7f9a52, hash(i, 85 + k), 0.1);
            for (let j = 0; j < 3; j++) {
              const hh = 0.8 + hash(i * 3 + j, 80 + k) * 0.6, lx = (j - 1) * 0.05, lz = (j % 2) * 0.04, tl = (j - 1) * 0.22;
              L.add('reed', tx + lx, ty, tz + lz, 1, hh, 1, j, rc, tl * 0.6, tl);
              if (j === 1 && hash(i, 90 + k) > 0.5) L.add('dot', tx + lx + Math.sin(tl) * 0.3 * hh * -0.5, ty + 0.29 * hh, tz + lz, 0.35, 0.9, 0.35, 0, 0x6b4a2b);
            }
          }
          break;
      }
      // kıyı köpüğü: kara-su kenarında, zeminin su seviyesini kestiği yerde
      if (t.terrain !== 'water') for (const n of this.sim.g.neighbors(i)) {
        if (w.tiles[n].terrain !== 'water') continue;
        const [nx, nz] = this.pos(n);
        const dx = nx - x, dz = nz - z;
        let f = 0.5;
        for (let k = 2; k <= 18; k++) { const q = k / 20; if (this.groundY(x + dx * q, z + dz * q) < WATER_Y) { f = q; break; } }
        L.add('foam', x + dx * f, WATER_Y + 0.012, z + dz * f, 1.12, 1, 1, Math.atan2(dx, dz), 0xffffff);
        L.add('foam', x + dx * (f + 0.06), WATER_Y + 0.01, z + dz * (f + 0.06), 1.15, 1, 0.6, Math.atan2(dx, dz), 0xdfeef5);
      }
    }
    L.end();
    this.lastTerrain = this.sim.day;
    this.lastSeason = season;
  }

  private buildDynamic() {
    const s = this.sim, w = s.w, L = this.dynamic;
    // kışın çatılar karla örtülür
    const snowRoof: [number, number] = [0xf2f5f7, 0.62];
    L.tint = s.season === 3 ? { human_house_a: snowRoof, human_house_b: snowRoof, roof: snowRoof, roofslab: snowRoof, gable: snowRoof, stake: [0xf2f5f7, 0.15], spire: [0xf2f5f7, 0.45], dome: [0xf2f5f7, 0.5], tent: [0xf2f5f7, 0.4], blob: [0xdde8e2, 0.75], tree: [0xdde8e2, 0.7] } : {};
    this.burning = [];
    this.berths.clear();
    this.smokes = [];
    this.colWalls = [];
    // av hayvanı yerleşimden (sahipli olsun olmasın) 3 karo uzak durur
    this.nearTown.clear();
    for (const st of w.settlements) if (st.alive) for (const q of s.g.within(st.tile, 3)) this.nearTown.add(q);
    L.begin();
    this.buildBorders();
    this.buildWalks();
    // yollar: merkez → kenar ortası → komşu merkez, zemini izler
    const seg0 = (x1: number, z1: number, x2: number, z2: number) => {
      const y1 = Math.max(this.groundY(x1, z1), WATER_Y + 0.02), y2 = Math.max(this.groundY(x2, z2), WATER_Y + 0.02);
      const hor = Math.hypot(x2 - x1, z2 - z1);
      L.add('road', x1, y1 + 0.02, z1, 1, 1, Math.hypot(hor, y2 - y1), Math.atan2(x2 - x1, z2 - z1), 0xcdb68a, -Math.atan2(y2 - y1, hor));
    };
    // engebeli zeminde yol üçe bölünür: zemini izler, havada kalmaz
    const seg = (x1: number, z1: number, x2: number, z2: number) => { for (let q = 0; q < 3; q++) seg0(x1 + (x2 - x1) * q / 3, z1 + (z2 - z1) * q / 3, x1 + (x2 - x1) * (q + 1) / 3, z1 + (z2 - z1) * (q + 1) / 3); };
    for (let i = 0; i < w.tiles.length; i++) {
      if (!w.tiles[i].road) continue;
      const [x, z] = this.pos(i);
      for (const n of s.g.neighbors(i)) {
        if (n < i || !(w.tiles[n].road || w.settlements.some((q) => q.alive && q.tile === n))) continue;
        const [nx, nz] = this.pos(n);
        const mx = (x + nx) / 2, mz = (z + nz) / 2;
        seg(x, z, mx, mz); seg(mx, mz, nx, nz);
      }
    }
    // yataklar
    if (this.showDeposits) for (const d of w.deposits) {
      if (d.kind === 'fertile' || d.kind === 'heartwood') continue;
      const def = DEPOSITS[d.kind];
      const col = new THREE.Color(def.color).getHex();
      for (const ti of d.tiles) {
        const t = w.tiles[ti];
        if (t.ext || (def.reserve && t.reserve <= 0)) continue;
        const [x, z] = this.pos(ti);
        if (d.kind === 'mana') { L.add('crystal', x - 0.35, this.gy(x - 0.35, z + 0.2), z + 0.2, 1, 1, 1, ti, col); L.add('crystal', x - 0.1, this.gy(x - 0.1, z + 0.4), z + 0.4, 0.7, 0.7, 0.7, ti * 2, col); continue; }
        if (d.kind === 'mithril') { L.add('glow', x - 0.35, this.gy(x - 0.35, z + 0.2), z + 0.2, 0.9, 0.9, 0.9, ti, col); continue; }
        for (let k = 0; k < 3; k++) { const rx = x - 0.45 + hash(ti, 80 + k) * 0.5, rz = z + 0.1 + hash(ti, 90 + k) * 0.5; L.add('rock', rx, this.groundY(rx, rz) + 0.06, rz, 1, 0.8, 1, hash(ti, k) * 6, col); }
      }
    }
    // çıkarma yapıları
    for (let i = 0; i < w.tiles.length; i++) {
      const t = w.tiles[i];
      if (!t.ext) continue;
      const st = s.settlement(t.ext.settlement);
      const civCol = st ? new THREE.Color(w.civs[st.civ].color).getHex() : 0x888888;
      const [x, z] = this.pos(i);
      const h = this.height(i);
      const lv = t.ext.level;
      if (t.ext.burned) {
        if (t.ext.kind === 'farm') L.add('disc', x, h, z, 1, 1, 1, 0, 0x2e2a24);
        this.charred(x + 0.2, this.gy(x + 0.2, z + 0.25), z + 0.25, 0.3, 0.2, i);
        for (let k = 0; k < 3; k++) L.add('log', x - 0.2 + k * 0.15, this.gy(x - 0.2 + k * 0.15, z - 0.1 + hash(i, k) * 0.2) + 0.04, z - 0.1 + hash(i, k) * 0.2, 1, 1, 1, hash(i, k + 4) * 3, 0x1e1a18);
        if (s.day - (t.ext.burnedAt ?? -999) < 25) this.burning.push([x, h + 0.1, z, 1]);
        continue;
      }
      const dep = t.ext.depleted;
      const k = 0.85 + lv * 0.15;
      switch (t.ext.kind) {
        case 'farm':
          // tarla toprağı zemine gömülü: yan yüzü ince bir set gibi görünsün, levha gibi değil
          L.add('disc', x, h - 0.03, z, 1, 1, 1, 0, s.season === 3 ? 0xd9d4c2 : 0x7a5a3a);
          {
            // ekin sıraları: ilkbaharda filiz, yazın boy, sonbaharda altın başak ve hasat, kışın çıplak
            const sd = s.day % 30;
            const hgt = s.season === 0 ? 0.03 + sd * 0.002 : s.season === 1 ? 0.09 + sd * 0.002 : s.season === 2 ? (sd < 15 ? 0.15 : 0.02) : 0;
            const col = s.season === 0 ? 0x8fcf5a : s.season === 1 ? 0xb9c24a : 0xe0b84a;
            if (hgt > 0) for (let r = -2; r <= 2; r++) L.add('crop', x + r * 0.2, this.gy(x + r * 0.2, z) + 0.01, z, 0.14, hgt + 0.02, 1.05 - Math.abs(r) * 0.12, 0.3, jitter(col, hash(i, 200 + r), 0.07, 0.02));
            if (s.season === 2 && sd >= 12) for (let r = 0; r < 2 + lv; r++) L.add('dome', x - 0.4 + r * 0.26, this.gy(x - 0.4 + r * 0.26, z + 0.4) + 0.03, z + 0.4, 0.18, 0.3, 0.18, 0, 0xd8b64a);
            else for (let r = 0; r < lv; r++) L.add('box', x - 0.4 + r * 0.3, this.gy(x - 0.4 + r * 0.3, z + 0.45) + 0.05, z + 0.45, 0.14, 0.14, 0.14, 0.3, 0xe8c85a);
          }
          if (lv >= 3) { L.add('house', x + 0.35, this.gy(x + 0.35, z - 0.3), z - 0.3, 0.32, 0.26, 0.32, 0.4, 0xe9dcc0); L.add('roof', x + 0.35, this.gy(x + 0.35, z - 0.3) + 0.26, z - 0.3, 0.32, 0.4, 0.32, 0.4, 0xa0442e); }
          break;
        case 'lumber':
          for (let r = 0; r < 2 + lv; r++) L.add('log', x + 0.3, this.gy(x + 0.3, z + 0.25 + (r >> 1) * 0.12 - 0.1) + 0.07 + (r % 2) * 0.1, z + 0.25 + (r >> 1) * 0.12 - 0.1, 1, 1, 1, 0.3, 0x8a5a33);
          if (lv >= 2) { L.add('house', x - 0.25, this.gy(x - 0.25, z + 0.35), z + 0.35, 0.34, 0.24, 0.3, 0, 0x9b7650); L.add('roof', x - 0.25, this.gy(x - 0.25, z + 0.35) + 0.24, z + 0.35, 0.32, 0.35, 0.3, 0, civCol); }
          break;
        case 'hunt':
          L.add('tent', x + 0.3, this.gy(x + 0.3, z + 0.3), z + 0.3, k, k, k, 0.5, 0x8b6a45);
          break;
        case 'dock': {
          const wn = s.g.neighbors(i).find((n) => w.tiles[n].terrain === 'water');
          const [wx, wz] = wn !== undefined ? this.pos(wn) : [x, z + 1];
          const ang = Math.atan2(wx - x, wz - z);
          const ux = Math.sin(ang), uz = Math.cos(ang), px = Math.cos(ang), pz = -Math.sin(ang);
          // iskele: karadan suya uzanan tahta, kazıklar üstünde
          const d0 = 0.25, d1 = 1.05 + lv * 0.18;
          const x1 = x + ux * d0, z1 = z + uz * d0, y1 = this.groundY(x1, z1) + 0.02;
          const x2 = x + ux * d1, z2 = z + uz * d1, y2 = WATER_Y + 0.09;
          const hor = d1 - d0;
          L.add('road', x1, y1, z1, 1.25, 1.2, Math.hypot(hor, y2 - y1), ang, 0x8a6a45, -Math.atan2(y2 - y1, hor));
          for (let q = 0.55; q <= d1 + 0.01; q += 0.22) {
            const qx = x + ux * q, qz = z + uz * q, gy = Math.min(this.groundY(qx, qz), WATER_Y) - 0.12;
            const dy = y1 + (y2 - y1) * Math.min(1, (q - d0) / hor);
            for (const e of [-1, 1]) L.add('post', qx + px * e * 0.17, gy, qz + pz * e * 0.17, 1.3, dy - gy + 0.07, 1.3, 0, 0x5a4330);
          }
          if (lv >= 2) { // uçta iskele başı
            L.add('box', x2 - ux * 0.08, y2 - 0.01, z2 - uz * 0.08, 0.55, 0.035, 0.3, ang, 0x8a6a45);
            L.add('lantern', x2 + px * 0.24, y2 + 0.2, z2 + pz * 0.24, 0.7, 0.7, 0.7, 0, 0xffc45a);
            L.add('post', x2 + px * 0.24, y2, z2 + pz * 0.24, 1, 0.2, 1, 0, 0x5a4330);
          }
          // balıkçı kulübesi ve ağ kurutma
          const hx = x - ux * 0.25 + px * 0.35, hz = z - uz * 0.25 + pz * 0.35, hy = this.groundY(hx, hz);
          L.add('house', hx, hy, hz, 0.3, 0.2, 0.26, ang, 0x9b7650);
          L.add('gable', hx, hy + 0.2, hz, 0.36, 0.2, 0.3, ang, civCol);
          const rx0 = x - px * 0.35 - ux * 0.05, rz0 = z - pz * 0.35 - uz * 0.05, ry0 = this.groundY(rx0, rz0);
          for (const e of [-1, 1]) L.add('post', rx0 + ux * e * 0.15, ry0, rz0 + uz * e * 0.15, 1, 0.24, 1, 0, 0x5a4330);
          L.add('box', rx0, ry0 + 0.08, rz0, 0.01, 0.15, 0.3, ang, 0x6a7a6a);
          for (let k = 0; k < lv; k++) L.add('box', rx0 + ux * (k - 1) * 0.1, ry0 + 0.14, rz0 + uz * (k - 1) * 0.1, 0.02, 0.07, 0.035, ang, 0x9fb4c0);
          L.add('box', x + px * 0.05, this.gy(x + px * 0.05, z + pz * 0.05), z + pz * 0.05, 0.12, 0.1, 0.12, ang + 0.4, 0x7a5a3a);
          break;
        }
        case 'quarry': {
          // kademeli kesilmiş taş yüzü, önünde kesme bloklar ve moloz
          const qa = hash(i, 33) * Math.PI * 2, fx = Math.sin(qa), fz = Math.cos(qa);
          const bx0 = x + 0.1 - fx * 0.2, bz0 = z + 0.15 - fz * 0.2;
          for (let q = 0; q < 3; q++) {
            const px = bx0 - fx * q * 0.13, pz = bz0 - fz * q * 0.13;
            L.add('box', px, this.gy(px, pz) - 0.03, pz, 0.62 - q * 0.08, 0.1 + q * 0.1, 0.16, qa, jitter(0xb9b3a7, hash(i, q + 50), 0.07));
          }
          for (let r = 0; r < 1 + lv; r++) {
            const px = bx0 + fx * (0.3 + (r % 2) * 0.12) + Math.cos(qa) * (r - lv / 2) * 0.16, pz = bz0 + fz * (0.3 + (r % 2) * 0.12) - Math.sin(qa) * (r - lv / 2) * 0.16;
            L.add('box', px, this.gy(px, pz), pz, 0.13, 0.1, 0.13, qa + r * 0.3, jitter(0xd6d0c4, hash(i, r + 60), 0.05));
          }
          for (let r = 0; r < 3; r++) { const px = bx0 + fx * 0.18 + (hash(i, r + 70) - 0.5) * 0.4, pz = bz0 + fz * 0.18 + (hash(i, r + 74) - 0.5) * 0.4; L.add('rock', px, this.gy(px, pz) + 0.02, pz, 0.45, 0.35, 0.45, r, 0xa39d92); }
          if (lv >= 2) { const px = bx0 + Math.cos(qa) * 0.42, pz = bz0 - Math.sin(qa) * 0.42; L.add('post', px, this.gy(px, pz), pz, 1.3, 0.55, 1.3, 0, 0x7a5030); L.add('log', px + fx * 0.1, this.gy(px, pz) + 0.53, pz + fz * 0.1, 0.8, 0.5, 0.5, qa + Math.PI / 2, 0x7a5030); }
          break;
        }
        case 'claypit': {
          // ıslak kil çukuru, kenarında kuruyan kerpiç sıraları
          L.add('rdisc', x + 0.2, this.gy(x + 0.2, z + 0.2) + 0.004, z + 0.2, 0.6, 1, 0.52, 0.2, 0x8a5a3c);
          L.add('rdisc', x + 0.2, this.gy(x + 0.2, z + 0.2) + 0.01, z + 0.2, 0.42, 1, 0.36, 0.2, 0xa8683f);
          for (let r = 0; r < 2 + lv; r++) for (let c2 = 0; c2 < 3; c2++) { const px = x - 0.35 + c2 * 0.1, pz = z - 0.2 + r * 0.12; L.add('box', px, this.gy(px, pz), pz, 0.07, 0.035, 0.1, 0.1, jitter(0xc27a4e, hash(i, r * 3 + c2), 0.08)); }
          break;
        }
        case 'mine': {
          // yamaca oyulmuş galeri: taş höyük, ahşap çatkılı karanlık ağız, önünde cevher yığını
          const dk = t.deposit >= 0 ? w.deposits[t.deposit]?.kind : undefined;
          const ore = new THREE.Color(dk ? DEPOSITS[dk].color : '#8a8f99').getHex();
          const ma = hash(i, 31) * Math.PI * 2, fx = Math.sin(ma), fz = Math.cos(ma), rx = Math.cos(ma), rz = -Math.sin(ma);
          const mx = x + 0.1 - fx * 0.12, mz = z + 0.2 - fz * 0.12;
          L.add('bump', mx, this.gy(mx, mz) - 0.06, mz, 0.62 * k, 0.55 * k, 0.58 * k, ma, dep ? 0x7c7770 : 0x8f8272);
          const ex = mx + fx * 0.3 * k, ez = mz + fz * 0.3 * k, ey = this.gy(ex, ez);
          L.add('box', ex - fx * 0.02, ey, ez - fz * 0.02, 0.17, 0.19, 0.05, ma, 0x14110e);
          for (const e of [-1, 1]) L.add('post', ex + rx * e * 0.11, ey, ez + rz * e * 0.11, 1.5, 0.23, 1.5, 0, 0x7a5030);
          L.add('box', ex, ey + 0.2, ez, 0.3, 0.045, 0.06, ma, 0x7a5030);
          if (dep) for (const e of [-1, 1]) L.add('box', ex + fx * 0.03, ey + 0.1, ez + fz * 0.03, 0.26, 0.03, 0.015, ma, 0x6b4a2b, 0, e * 0.55);
          else {
            const ox = ex + fx * 0.2 + rx * 0.2, oz = ez + fz * 0.2 + rz * 0.2, oy = this.gy(ox, oz);
            for (let q = 0; q < 2 + lv; q++) L.add('rock', ox + (hash(i, q + 40) - 0.5) * 0.14, oy + 0.03 + (q > 2 ? 0.06 : 0), oz + (hash(i, q + 44) - 0.5) * 0.14, 0.75, 0.6, 0.75, q, ore);
          }
          if (lv >= 2) {
            const cx2 = ex + fx * 0.22 - rx * 0.2, cz2 = ez + fz * 0.22 - rz * 0.2;
            L.add('cart', cx2, this.gy(cx2, cz2), cz2, 0.5, 0.55, 0.5, ma, 0x6b4a2b);
            if (!dep) L.add('rock', cx2, this.gy(cx2, cz2) + 0.15, cz2, 0.7, 0.45, 0.7, 0, ore);
          }
          if (lv >= 3) { L.add('tower', x - 0.35, this.gy(x - 0.35, z - 0.2), z - 0.2, 0.25, 0.6, 0.25, 0, 0x8d8a84); L.add('spire', x - 0.35, this.gy(x - 0.35, z - 0.2) + 0.6, z - 0.2, 0.3, 0.4, 0.3, 0, civCol); }
          break;
        }
        case 'pasture':
          // ağıl: direk ve iki sıra kiriş (eskiden havada asılı bir halka)
          for (let k = 0; k < 14; k++) {
            const a0 = (k / 14) * Math.PI * 2, a1 = ((k + 1) / 14) * Math.PI * 2, R = 0.62;
            const px = x + 0.1 + Math.cos(a0) * R, pz = z + 0.1 + Math.sin(a0) * R, py = this.gy(px, pz);
            L.add('post', px, py - 0.02, pz, 1.1, 0.14, 1.1, 0, 0x6b4a2b);
            if (k === 3) continue; // kapı boşluğu
            const mx = x + 0.1 + Math.cos((a0 + a1) / 2) * R, mz = z + 0.1 + Math.sin((a0 + a1) / 2) * R, my = this.gy(mx, mz);
            for (const hh of [0.05, 0.1]) L.add('box', mx, my + hh, mz, 2 * R * Math.sin(Math.PI / 14) + 0.01, 0.012, 0.014, -(a0 + a1) / 2 - Math.PI / 2, 0x8a6a45);
          }
          for (let r = 0; r < lv + 1; r++) L.add('box', x + 0.1 + (hash(i, r) - 0.5) * 0.5, this.gy(x + 0.1 + (hash(i, r) - 0.5) * 0.5, z + 0.1 + (hash(i, r + 5) - 0.5) * 0.5), z + 0.1 + (hash(i, r + 5) - 0.5) * 0.5, 0.2, 0.14, 0.09, hash(i, r) * 6, 0x7a4f2d);
          break;
        case 'herbalist':
          L.add('house', x + 0.3, this.gy(x + 0.3, z + 0.25), z + 0.25, 0.26, 0.2, 0.26, 0.2, 0xd8cfae);
          L.add('roof', x + 0.3, this.gy(x + 0.3, z + 0.25) + 0.2, z + 0.25, 0.26, 0.3, 0.26, 0.2, 0x5f9a4f);
          break;
        case 'crystal':
          for (let r = 0; r < lv + 1; r++) L.add('crystal', x - 0.2 + r * 0.2, this.gy(x - 0.2 + r * 0.2, z + 0.25), z + 0.25, 1 + r * 0.1, 1 + lv * 0.25, 1, r, 0x9b6bff);
          if (lv >= 2) { L.add('tower', x + 0.3, this.gy(x + 0.3, z - 0.2), z - 0.2, 0.2, 0.8, 0.2, 0, 0x6d6280); L.add('spire', x + 0.3, this.gy(x + 0.3, z - 0.2) + 0.8, z - 0.2, 0.24, 0.5, 0.24, 0, 0x9b6bff); }
          break;
        case 'mithril':
          L.add('box', x, this.gy(x, z + 0.25), z + 0.25, 0.4, 0.3, 0.3, 0.4, 0x2f3a40);
          L.add('glow', x + 0.25, this.gy(x + 0.25, z - 0.1), z - 0.1, 1.2, 1.2, 1.2, 0, 0x8fe6f0);
          break;
        case 'grove':
          L.add('ring', x, h + 0.04, z, 0.5, 1, 0.5, 0, 0xcfe89a);
          L.add('glow', x, h, z, 0.8, 0.9, 0.8, 0, 0x9fe6a0);
          break;
      }
      // sahiplik bayrağı
      if (t.ext.kind === 'mine' || t.ext.kind === 'crystal' || t.ext.kind === 'mithril' || t.ext.kind === 'quarry') {
        // sahiplik flaması: küçük ve toprağa oturan (eskiden büyük bayraklar kalabalık görüntü yapıyordu)
        const fy = this.gy(x + 0.42, z - 0.28);
        L.add('post', x + 0.42, fy - 0.02, z - 0.28, 0.8, 0.4, 0.8, 0, 0x5a4330);
        L.add('flag', x + 0.42, fy + 0.33, z - 0.28, 0.5, 0.45, 1, hash(i, 77) * 6, dep ? 0x666666 : civCol);
      }
    }
    // yerleşimler: evler gerçek konut sayısından, atölye ve kamu yapıları iç halkada
    this.layouts.clear();
    for (const st of w.settlements) {
      if (!st.alive) continue;
      const c = w.civs[st.civ];
      const civCol = new THREE.Color(c.color).getHex();
      const [x, z] = this.pos(st.tile);
      const h = this.height(st.tile);
      const R = 0.35 + st.tier * 0.35;
      const building = st.project?.type === 'civic' && st.project.kind === 'wonder';
      const wSpot = st.civics.wonder || building ? this.wonderSpot(st.tile) : null;
      if (wSpot) this.wonder(c.cls, wSpot[0], wSpot[1], building ? 1 - st.project!.left / st.project!.total : 1, civCol);
      // kamu yapıları ve atölyeler
      const pub: string[] = [];
      for (const k of ['market', 'tavern', 'temple', 'library', 'guild', 'mint', 'unique'] as const) if (st.civics[k]) pub.push(k);
      for (const [k, v] of Object.entries(st.workshops)) for (let q = 0; q < Math.min(2, v ?? 0); q++) pub.push(k);
      if (st.project && (st.project.type === 'civic' || st.project.type === 'workshop') && !['hut', 'house', 'stonehouse', 'wonder', 'palisade', 'stonewall', 'castle'].includes(st.project.kind)) pub.push('site:' + st.project.kind);
      const lay: TownLayout = { x, z, homes: [], work: {}, R, wallR: R * 1.9 + 0.55, houses: [], homeless: s.homeless(st) };
      const spots: [number, number][] = [];
      pub.forEach((k, j) => {
        const a = j * 2.2 + st.id * 0.7 + 0.5, r = (st.tier >= 1 ? 0.78 : 0.62) + (j % 3) * 0.3 + st.tier * 0.08;
        let px = x + Math.cos(a) * r, pz = z + Math.sin(a) * r;
        if (wSpot && (px - wSpot[0]) ** 2 + (pz - wSpot[1]) ** 2 < 0.8) { px = x + Math.cos(a + 1.2) * r; pz = z + Math.sin(a + 1.2) * r; }
        const py = this.groundY(px, pz);
        if (py < WATER_Y + 0.05) return;
        spots.push([px, pz]);
        (lay.work[k.startsWith('site:') ? 'site' : k] ??= []).push([px, pz]);
        if (k.startsWith('site:')) this.site(px, py, pz, a, 1 - (st.project!.left / st.project!.total));
        else this.workshopModel(k, px, py, pz, a + Math.PI, civCol, c.race);
      });
      const plz = this.plazaSpot(st, x, z, spots, wSpot);
      lay.work.plaza = [plz];
      // evler: kulübe, ev, taş konak sayısı kadar
      const counts: [string, number][] = [['stonehouse', st.civics.stonehouse ?? 0], ['house', st.civics.house ?? 0], ['hut', st.civics.hut ?? 0]];
      const kinds: string[] = [];
      for (const [k, v] of counts) for (let q = 0; q < v; q++) kinds.push(k);
      if (kinds.length < 2) kinds.push('hut', 'hut');
      const n = Math.min(48, kinds.length);
      const popE = Object.entries(st.pop).filter(([, v]) => (v ?? 0) > 0) as [string, number][];
      const popT = popE.reduce((q, [, v]) => q + v, 0) || 1;
      const pickRace = (k: number) => { let r0 = hash(st.id * 7 + k, 21) * popT; for (const [rc, v] of popE) { r0 -= v; if (r0 <= 0) return rc; } return c.race; };
      const burnt = Math.min(n, st.burnedHouses ?? 0);
      // doluluk: konutlu nüfus merkezden dışa doğru evlere dağılır, kenardaki evler boş kalabilir
      const HCAP: Record<string, number> = { hut: 3, house: 6, stonehouse: 11 };
      const realCap = kinds.reduce((q, k) => q + HCAP[k], 0);
      const livingDrawn = kinds.slice(burnt, n).reduce((q, k) => q + HCAP[k], 0);
      const housedIn = Math.max(0, s.pop(st) - s.homeless(st) - 4);
      let toHouse = Math.round(Math.min(1, housedIn / Math.max(1, realCap - burnt * 4)) * livingDrawn);
      const fresh = s.day - (st.burnedAt ?? -999) < 25;
      let placed = 0;
      for (let k = 0; k < n * 3 && placed < n; k++) {
        const a = k * 2.399 + st.id, d = R * Math.sqrt((k + 0.5) / (n * 1.4)) * 1.9 + 0.3;
        const hx = x + Math.cos(a) * d, hz = z + Math.sin(a) * d;
        const ti = this.nearestTile(hx, hz);
        if (ti < 0 || w.tiles[ti].terrain === 'water' || w.tiles[ti].terrain === 'mountain') continue;
        if (st.tier >= 1 && d < 0.55) continue;
        if (wSpot && (hx - wSpot[0]) ** 2 + (hz - wSpot[1]) ** 2 < 1.0) continue;
        if (spots.some(([sx, sz]) => (hx - sx) ** 2 + (hz - sz) ** 2 < 0.12)) continue;
        if ((hx - plz[0]) ** 2 + (hz - plz[1]) ** 2 < (st.tier >= 1 ? 0.2 : 0.08)) continue;
        const hh = this.groundY(hx, hz);
        if (hh < WATER_Y + 0.05) continue;
        const kind = kinds[placed];
        const big = kind === 'stonehouse' ? 1.3 : kind === 'hut' ? 0.8 : 1;
        const hr0 = pickRace(placed);
        const hr = c.cls === 'warlock' && hr0 === c.race ? 'warlock' : hr0;
        const sx = (0.28 + hash(st.id, k) * 0.1) * big, sy = (0.22 + hash(st.id, k + 9) * 0.1) * big;
        if (placed < burnt) {
          this.charred(hx, hh, hz, sx, sy, a);
          (lay.work.ruin ??= []).push([hx, hz]);
          if (fresh) this.burning.push([hx, hh + sy * 0.6, hz, 0.7]);
          lay.houses.push({ x: hx, y: hh, z: hz, kind, cap: HCAP[kind], occ: 0, race: hr0, seed: st.id * 1000 + k, burnt: true });
        } else {
          const occ = Math.min(HCAP[kind], toHouse); toHouse -= occ;
          this.hLit = occ <= 0 ? 0 : Math.max(1, Math.round((occ / HCAP[kind]) * 4));
          this.house(hr, hx, hh, hz, sx, sy, a, civCol, kind === 'hut' ? 0 : st.tier, hash(st.id, k + 3));
          this.hLit = 9;
          lay.houses.push({ x: hx, y: hh, z: hz, kind, cap: HCAP[kind], occ, race: hr0, seed: st.id * 1000 + k, burnt: false });
          // dolu evin bacası tüter (kışın daha çok)
          if (occ > 0 && hash(st.id, k + 50) < (s.season === 3 ? 0.55 : 0.2)) this.smokes.push(this.chimTop ?? [hx, hh + sy * 1.7, hz]);
        }
        lay.homes.push([hx + Math.sin(a) * sx * 0.7, hz + Math.cos(a) * sx * 0.7]);
        placed++;
      }
      if (!lay.homes.length) lay.homes.push([x + 0.3, z + 0.3]);
      this.layouts.set(st.id, lay);
      this.townProps(st, lay, civCol);
      // mezarlık: salgın ölüleri kasabanın dışında
      const gv = Math.min(16, Math.round((st.graves ?? 0) / 3));
      if (gv > 0) {
        const ga = hash(st.id, 44) * 6.28, gr = lay.wallR + 0.45;
        const gx = x + Math.cos(ga) * gr, gz = z + Math.sin(ga) * gr;
        for (let k = 0; k < gv; k++) {
          const px = gx + ((k % 4) - 1.5) * 0.14, pz = gz + (Math.floor(k / 4) - 1.5) * 0.16, py = this.gy(px, pz);
          L.add('box', px, py - 0.02, pz, 0.07, 0.14, 0.03, ga, 0xa8a296); L.add('box', px, py + 0.07, pz, 0.11, 0.025, 0.035, ga, 0xa8a296);
        }
        L.add('post', gx - 0.35, this.gy(gx - 0.35, gz), gz, 1, 0.5, 1, 0, 0x3a3a3a);
        lay.work.cemetery = [[gx, gz]];
      }
      this.hall(c.cls === 'warlock' ? 'warlock' : c.race, x, h, z, civCol, st.tier, !!st.civics.castle);
      const wall = st.civics.castle ? 3 : st.civics.stonewall ? 2 : st.civics.palisade ? 1 : 0;
      if (wall) this.walls(st.tile, x, z, lay.wallR, wall, c.cls === 'warlock' ? 'warlock' : c.race, civCol);
      L.add('post', x, h, z, 1.2, 1.3, 1.2, 0, 0x5a4330);
      L.add('flag', x, h + 1.2, z, 1.2, 1.2, 1, 0, civCol);
    }
    // harabeler: terk edilen ya da yıkılan yerleşimler
    for (const st of w.settlements) {
      if (st.alive || w.settlements.some((o) => o.alive && o.tile === st.tile)) continue;
      const [x, z] = this.pos(st.tile);
      for (let k = 0; k < 6; k++) {
        const a = k * 1.9 + st.id, d = 0.25 + hash(st.id, k) * 0.5;
        const px = x + Math.cos(a) * d, pz = z + Math.sin(a) * d, py = this.gy(px, pz);
        const hgt = 0.06 + hash(st.id, k + 3) * 0.16;
        L.add('box', px, py - 0.02, pz, 0.22 + hash(st.id, k + 5) * 0.12, hgt, 0.06, a, 0x8d877e);
        if (k % 2 === 0) L.add('rock', px + 0.1, py + 0.03, pz + 0.05, 0.8, 0.6, 0.8, a, 0x7a746c);
      }
      if (st.tier >= 1) { const y = this.gy(x, z); L.add('tower', x + 0.15, y, z - 0.1, 0.18, 0.35 + hash(st.id, 9) * 0.25, 0.18, 0, 0x7d776f); }
    }
    // kahraman mezarları (son 12)
    const dead = w.heroes.filter((h) => h.state === 'dead' && h.deathDay !== undefined).sort((a, b) => b.deathDay! - a.deathDay!).slice(0, 12);
    for (const h of dead) {
      const [x0, z0] = this.pos(h.pos);
      const x = x0 + (hash(h.id, 1) - 0.5) * 0.6, z = z0 + (hash(h.id, 2) - 0.5) * 0.6, y = this.gy(x, z);
      L.add('box', x, y - 0.02, z, 0.11, 0.2, 0.04, hash(h.id, 3) * 6, 0xb8b2a6);
      L.add('box', x, y + 0.1, z, 0.16, 0.035, 0.045, hash(h.id, 3) * 6, 0xb8b2a6);
      L.add('disc', x, y, z + 0.02, 0.18, 0.4, 0.26, hash(h.id, 3) * 6, 0x5f7a45);
    }
    // tarafsız hanlar
    for (const inn of w.inns) this.innModel(inn);
    // tersaneler ve limanlar
    for (const st of w.settlements) if (st.alive && st.civics.shipyard && st.port !== undefined) this.harbor(st);
    // canavar kampları
    for (const cp of w.camps) {
      if (!cp.alive) continue;
      const [x, z] = this.pos(cp.tile);
      const h = this.height(cp.tile);
      const gy = (px: number, pz: number) => this.gy(px, pz);
      // çiğnenmiş toprak: kamp ormanda da yukarıdan seçilsin
      if (cp.kind !== 'bugbear') { L.add('rdisc', x, h - 0.015, z, 1.25, 0.5, 1.25, cp.id, 0x6a5238); L.add('rdisc', x + 0.25, this.gy(x + 0.25, z - 0.2) - 0.012, z - 0.2, 0.7, 0.5, 0.6, cp.id + 1, 0x5a4530); }
      if (cp.kind === 'goblin') {
        const n = Math.min(6, 1 + Math.floor(cp.count / 2.5));
        for (let k = 0; k < n; k++) { const a = k * 2.1 + cp.id, tx = x + Math.cos(a) * 0.5, tz = z + Math.sin(a) * 0.5; L.add('tent', tx, gy(tx, tz), tz, 1.1, 1.2, 1.1, a, k % 2 ? 0x8a3a2a : 0x6b5a3a); }
        L.add('disc', x, h, z, 0.3, 1, 0.3, 0, 0x2a2622);
        // kafatası totemi
        L.add('post', x - 0.3, this.gy(x - 0.3, z + 0.25), z + 0.25, 1.4, 1.1, 1.4, 0, 0x4a3522); L.add('dot', x - 0.3, this.gy(x - 0.3, z + 0.25) + 1.05, z + 0.25, 1.4, 1.3, 1.4, 0, 0xe8e0c8);
        if (cp.boss || cp.count >= 10) for (let k = 0; k < 10; k++) { const a = (k / 10) * Math.PI * 1.4 + cp.id, px = x + Math.cos(a) * 0.9, pz = z + Math.sin(a) * 0.9; L.add('post', px, gy(px, pz) - 0.05, pz, 1.8, 0.45, 1.8, 0, 0x5a3a22); }
      } else if (cp.kind === 'hobgoblin') {
        for (let k = 0; k < 22; k++) { const a = (k / 22) * Math.PI * 2; if (Math.abs(Math.sin(a - 0.6)) < 0.12 && Math.cos(a - 0.6) > 0) continue; const px = x + Math.cos(a) * 0.85, pz = z + Math.sin(a) * 0.85; L.add('post', px, gy(px, pz) - 0.05, pz, 2.2, 0.6, 2.2, 0, 0x4a2e22); }
        L.add('tower', x + 0.2, this.gy(x + 0.2, z - 0.15), z - 0.15, 0.25, 0.95, 0.25, 0, 0x3a2a24); L.add('spire', x + 0.2, this.gy(x + 0.2, z - 0.15) + 0.95, z - 0.15, 0.3, 0.35, 0.3, 0, 0x6a1a14);
        L.add('house', x - 0.25, this.gy(x - 0.25, z + 0.2), z + 0.2, 0.35, 0.26, 0.5, 0.4, 0x4a3226); L.add('gable', x - 0.25, this.gy(x - 0.25, z + 0.2) + 0.26, z + 0.2, 0.42, 0.3, 0.55, 0.4, 0x2a1a14);
        L.add('post', x + 0.45, this.gy(x + 0.45, z + 0.3), z + 0.3, 1, 1.4, 1, 0, 0x2a1a14);
        L.add('flag', x + 0.45, this.gy(x + 0.45, z + 0.3) + 1.3, z + 0.3, 1.1, 1.1, 1, 0, 0x8a1a14);
      } else if (cp.kind === 'pirate') {
        // korsan koyu: kıyıya çekilmiş gemi, derme çatma kulübeler, fıçılar, kara bayrak
        const wn = this.sim.g.neighbors(cp.tile).find((q) => w.tiles[q].sea);
        const [wx, wz] = wn !== undefined ? this.pos(wn) : [x + 1, z];
        const ang = Math.atan2(wx - x, wz - z), ux = Math.sin(ang), uz = Math.cos(ang), px = Math.cos(ang), pz = -Math.sin(ang);
        const bx = x + ux * 0.75 + px * 0.2, bz = z + uz * 0.75 + pz * 0.2, by = Math.max(this.groundY(bx, bz), WATER_Y - 0.1);
        L.add('keel', bx, by - 0.04, bz, 0.3, 0.16, 0.95, ang + 1.2, 0x2e2420, 0.08, 0.35); // karaya çekilmiş, yan yatmış gemi
        L.add('post', bx, by + 0.1, bz, 1.2, 0.6, 1.2, ang, 0x3a2e24, 0.1, 0.5);
        for (let k = 0; k < Math.min(4, 1 + Math.floor(cp.count / 4)); k++) {
          const a = ang + Math.PI + (k - 1.5) * 0.7, r = 0.45 + (k % 2) * 0.15, hx = x + Math.sin(a) * r, hz = z + Math.cos(a) * r, hy = gy(hx, hz);
          L.add('house', hx, hy, hz, 0.24, 0.16, 0.2, a, k % 2 ? 0x5a4632 : 0x6a5238); L.add('gable', hx, hy + 0.16, hz, 0.28, 0.14, 0.22, a + Math.PI / 2, 0x3a2e24);
        }
        for (let k = 0; k < 4; k++) { const a = hash(cp.id, k) * 6.28, r = 0.3 + hash(cp.id, k + 9) * 0.3, qx = x + Math.cos(a) * r, qz = z + Math.sin(a) * r; L.add('box', qx, gy(qx, qz), qz, 0.08, 0.1, 0.08, a, k % 2 ? 0x6b4a2b : 0x8a6a45); }
        const fx0 = x - ux * 0.2 + px * 0.45, fz0 = z - uz * 0.2 + pz * 0.45, fy0 = gy(fx0, fz0);
        L.add('post', fx0, fy0, fz0, 1.3, 1.25, 1.3, 0, 0x2a2622);
        L.add('flag', fx0, fy0 + 1.15, fz0, 1.3, 1.3, 1, 0, 0x141418);
        L.add('dot', fx0 + 0.18, fy0 + 1.15, fz0, 0.8, 0.8, 0.4, 0, 0xe8e0c8);
        if (cp.boss) { L.add('post', fx0 - 0.25, gy(fx0 - 0.25, fz0 + 0.1), fz0 + 0.1, 1.1, 0.35, 1.1, 0, 0x5a4330); L.add('box', fx0 - 0.25, gy(fx0 - 0.25, fz0 + 0.1) + 0.3, fz0 + 0.1, 0.16, 0.1, 0.1, 0, 0x8a6a2a); } // hazine sandığı
      } else {
        L.add('cave', x, h - 0.05, z, 1, 0.9, 1, 0, 0x4a3a2a);
        L.add('disc', x, this.gy(x, z + 0.4) + 0.05, z + 0.4, 0.2, 1, 0.12, 0, 0x0a0a0a);
      }
    }
    L.end();
    this.buildColliders();
    this.lastDyn = s.day;
  }

  /**
   * Tarafsız han. İnşaatta: kazıklar, kereste ve taş yığını, hancının çadırı ve arabası; temel → iskelet → duvar → çatı.
   * Açıkken: iki katlı taş-ahşap bina (Han: yan kanat; Kervansaray: üçüncü kat, avlu duvarı, kapı, gözcü kulesi),
   * tabela, ilan panosu, kuyu, masa-sıralar, ahır ve binekler, arkada mezarlık, efsane heykelleri.
   */
  private innModel(inn: Inn) {
    const L = this.dynamic, w = this.sim.w;
    if (inn.stage === 'road') return;
    const [x0, z0] = this.pos(inn.tile);
    const a = hash(inn.id, 7) * Math.PI * 2;
    const fx = Math.sin(a), fz = Math.cos(a), rx = Math.cos(a), rz = -Math.sin(a);
    const at = (f: number, r: number): [number, number, number] => { const px = x0 + fx * f + rx * r, pz = z0 + fz * f + rz * r; return [px, this.gy(px, pz), pz]; };
    const [x, y, z] = at(0, 0);
    const k01 = (v: number) => Math.max(0, Math.min(1, v));
    if (inn.stage === 'ruin') {
      for (let k = 0; k < 7; k++) {
        const [px, py, pz] = at((hash(inn.id, k) - 0.5) * 0.7, (hash(inn.id, k + 9) - 0.5) * 0.8);
        L.add('box', px, py - 0.02, pz, 0.18 + hash(inn.id, k + 3) * 0.15, 0.05 + hash(inn.id, k + 5) * 0.18, 0.06, a + k, 0x5a534c);
        if (k % 2) L.add('log', px + 0.08, py + 0.03, pz, 1, 1, 1, a + k * 0.7, 0x2a2420);
      }
      L.add('post', ...at(0.45, 0.3), 0.8, 0.4, 0.8, 0, 0x3a2e22);
      return;
    }
    const b = inn.build;
    const stone = 0x9a9185, plaster = 0xd9c49a, beam = 0x4a3322, roofC = 0x6a3a2a, timber = 0x7a5533;
    /** yığınlar: henüz kullanılmamış kereste ve taş */
    const piles = (f: number, r: number) => {
      if (!b) return;
      const p = Math.min(1, b.work / b.need);
      const logs = Math.min(9, Math.ceil(Math.max(0, b.wood - p * b.woodNeed) / 3));
      for (let k = 0; k < logs; k++) { const [px, py, pz] = at(f + (k % 3) * 0.02, r + (k % 3 - 1) * 0.06); L.add('log', px, py + 0.04 + Math.floor(k / 3) * 0.1, pz, 1, 1, 1, a, 0x9a6a3a); }
      const rocks = Math.min(6, Math.ceil(Math.max(0, b.stone - p * b.stoneNeed) / 1.5));
      for (let k = 0; k < rocks; k++) { const [px, py, pz] = at(f - 0.28 + (k % 3) * 0.08, r + Math.floor(k / 3) * 0.08); L.add('rock', px, py + 0.04, pz, 0.8, 0.6, 0.8, k, 0x8d877e); }
    };
    if (inn.stage === 'build' && b) {
      const p = Math.min(1, b.work / b.need);
      // kazılmış toprak, köşe kazıkları
      L.add('disc', x, y - 0.01, z, 0.85, 0.4, 0.85, a, 0x7a6448);
      for (const [f, r] of [[0.3, 0.4], [0.3, -0.4], [-0.3, 0.4], [-0.3, -0.4]]) L.add('post', ...at(f, r), 1, 0.2, 1, 0, 0x8a6a45);
      // hancının çadırı, arabası ve "yakında" tabelası
      L.add('tent', ...at(-0.55, 0.5), 1.3, 1.2, 1.3, a, 0xd8c8a0);
      const [cx, cy, cz] = at(0.45, 0.62);
      L.add('cart', cx, cy, cz, 1, 1, 1, a + Math.PI / 2, 0x8a6a45); L.add('wheel', cx, cy, cz, 1, 1, 1, a, 0x3a2a1a);
      const [sx, sy, sz] = at(0.62, -0.25);
      L.add('post', sx, sy, sz, 1.1, 0.5, 1.1, 0, beam); L.add('box', sx, sy + 0.38, sz, 0.16, 0.09, 0.02, a, 0xe8c24a);
      piles(0.05, 0.62);
      const found = b.stoneNeed > 0 ? stone : timber;
      if (p >= 0.06) L.add('box', x, y - 0.02, z, 0.66, 0.08 * k01((p - 0.06) / 0.12) + 0.01, 0.48, a, found);
      const y1 = y + 0.06;
      if (p >= 0.2) { const h1 = 0.24 * k01((p - 0.2) / 0.2); for (const f of [-0.21, 0.21]) for (const r of [-0.3, 0, 0.3]) L.add('post', ...[at(f, r)[0], y1, at(f, r)[2]] as [number, number, number], 1.5, h1, 1.5, 0, beam); }
      if (p >= 0.36) for (const f of [-0.21, 0.21]) { const [bx, , bz] = at(f, 0); L.add('box', bx, y1 + 0.23, bz, 0.66, 0.03, 0.03, a, beam); }
      if (p >= 0.42) L.add('box', x, y1, z, 0.6, 0.23 * k01((p - 0.42) / 0.18), 0.42, a, stone);
      const y2 = y1 + 0.24;
      if (p >= 0.6) { const h2 = 0.22 * k01((p - 0.6) / 0.1); for (const f of [-0.23, 0.23]) for (const r of [-0.32, 0, 0.32]) L.add('post', ...[at(f, r)[0], y2, at(f, r)[2]] as [number, number, number], 1.5, h2, 1.5, 0, beam); }
      if (p >= 0.68) L.add('box', x, y2, z, 0.64, 0.21 * k01((p - 0.68) / 0.14), 0.46, a, plaster);
      if (p >= 0.82) L.add('gable', x, y2 + 0.22, z, 0.76, 0.42 * k01((p - 0.82) / 0.18), 0.56, a, roofC);
      // iskele: dış direkler ve kalaslar
      if (p > 0.25 && p < 0.97) {
        const top = p < 0.6 ? 0.34 : 0.62;
        for (const [f, r] of [[0.32, 0.42], [0.32, -0.42], [-0.32, 0.42], [-0.32, -0.42]]) L.add('post', ...at(f, r), 1.1, top, 1.1, 0, 0xa8845a);
        for (const hh of p < 0.6 ? [0.2] : [0.2, 0.46]) for (const f of [0.32, -0.32]) { const [bx, by, bz] = at(f, 0); L.add('box', bx, by + hh, bz, 0.86, 0.015, 0.06, a, 0xa8845a); }
      }
      return;
    }
    // ---- açık han
    const lv = inn.level;
    const rooms = [0, 6, 10, 16][lv] ?? 6;
    let persons = 0; for (const g of inn.guests) persons += g.n;
    const lit = persons > 0 ? Math.min(9, 1 + Math.ceil((persons / rooms) * 8)) : 1;
    L.add('disc', x, y - 0.01, z, lv >= 3 ? 1.12 : 0.95, 0.4, lv >= 3 ? 1.12 : 0.95, a, 0x8f7a5a);
    // alt kat taş, üst kat ahşap (kervansarayda bir kat daha), dik çatı
    L.add('house', x, y, z, 0.62, 0.24, 0.44, a, 0x9a9185);
    L.add('house', x, y + 0.24, z, 0.66, 0.22, 0.48, a, plaster);
    for (const r of [-0.3, 0, 0.3]) L.add('box', x + rx * r + fx * 0.245, y + 0.24, z + rz * r + fz * 0.245, 0.03, 0.22, 0.012, a, beam);
    const roofY = lv >= 3 ? y + 0.66 : y + 0.46;
    if (lv >= 3) { L.add('house', x, y + 0.46, z, 0.64, 0.2, 0.46, a, 0xe4d2a8); for (const r of [-0.3, 0.3]) L.add('box', x + rx * r + fx * 0.235, y + 0.46, z + rz * r + fz * 0.235, 0.03, 0.2, 0.012, a, beam); }
    L.add('gable', x, roofY, z, 0.76, 0.42, 0.56, a, roofC);
    // pencereler: dolu oda kadar yanar
    let wk = 0;
    const wins: [number, number, number][] = [[0.225, -0.2, 0.09], [0.225, 0.2, 0.09], [0.245, -0.2, 0.31], [0.245, 0, 0.31], [0.245, 0.2, 0.31], [-0.245, 0.15, 0.31]];
    if (lv >= 3) wins.push([0.235, -0.18, 0.52], [0.235, 0.18, 0.52], [-0.235, -0.15, 0.52]);
    for (const [f, r, h] of wins) { const [px, , pz] = at(f, r); L.add(wk++ >= lit ? 'winoff' : 'win', px, y + h, pz, 1.2, 1.2, 1, a, 0x2a2a30); }
    L.add('door', ...at(0.225, 0), 1.2, 1.2, 1, a, 0x4a3322);
    // baca ve duman
    const [cx, , cz] = at(-0.1, -0.24);
    L.add('box', cx, y + 0.46, cz, 0.08, roofY - y + 0.3 - 0.46, 0.08, a, 0x6d6760);
    this.smokes.push([cx, roofY + 0.3, cz]);
    // tabela ve fener
    const [sx, sy, sz] = at(0.42, 0.34);
    L.add('post', sx, sy, sz, 1.1, 0.62, 1.1, 0, beam);
    L.add('box', sx + rx * 0.05, sy + 0.5, sz + rz * 0.05, 0.16, 0.1, 0.02, a, 0xe8c24a);
    L.add('lantern', ...at(0.26, 0.12), 1, 1, 1, 0, 0xffc45a);
    // avluda masa ve sıralar
    const [tx, ty, tz] = at(0.36, -0.08);
    L.add('box', tx, ty + 0.07, tz, 0.2, 0.02, 0.1, a, 0x7a5533); L.add('post', tx, ty, tz, 1.4, 0.07, 1.4, 0, 0x5a4330);
    for (const s0 of [-1, 1]) { const [bx, by, bz] = at(0.36 + s0 * 0.1, -0.08); L.add('box', bx, by + 0.035, bz, 0.2, 0.02, 0.04, a, 0x6a4a2a); }
    // ilan panosu: açık ilan kadar kâğıt
    const [bx, by, bz] = at(0.5, -0.3);
    L.add('post', bx - rx * 0.1, by, bz - rz * 0.1, 1, 0.3, 1, 0, 0x5a4330); L.add('post', bx + rx * 0.1, by, bz + rz * 0.1, 1, 0.3, 1, 0, 0x5a4330);
    L.add('box', bx, by + 0.14, bz, 0.26, 0.15, 0.025, a, 0x7a5533);
    const notes = w.quests.filter((q) => q.open && (q.inn === inn.id)).length;
    for (let k = 0; k < Math.min(4, notes); k++) L.add('box', bx + rx * (-0.08 + k * 0.055) + fx * 0.015, by + 0.18 - (k % 2) * 0.05, bz + rz * (-0.08 + k * 0.055) + fz * 0.015, 0.04, 0.045, 0.01, a, 0xf2ecdc);
    // kuyu
    const [wx, wy, wz] = at(0.55, 0.05);
    L.add('tower', wx, wy, wz, 0.1, 0.08, 0.1, 0, 0x8d877e); L.add('disc', wx, wy + 0.08, wz, 0.12, 0.3, 0.12, 0, 0x2a4a5a);
    // ahır ve binekler (han büyüdükçe ahır uzar)
    const [hx, hy, hz] = at(-0.05, 0.58);
    const stL = lv >= 2 ? 0.46 : 0.34;
    L.add('house', hx, hy, hz, 0.3, 0.16, stL, a + Math.PI / 2, 0x7a5a3a); L.add('gable', hx, hy + 0.16, hz, 0.36, 0.2, stL + 0.06, a + Math.PI / 2, 0x5a3a22);
    const mounts = Math.min(lv >= 2 ? 5 : 3, w.heroes.filter((q) => q.civ === -1 && q.state === 'tavern' && q.tavern === inn.id).length + 1 + (persons > 3 ? 1 : 0));
    for (let k = 0; k < mounts; k++) {
      const [mx, my, mz] = at(0.25 + k * 0.14, 0.62);
      const col = [0x6a4a2a, 0xd8d0c0, 0x2a2420][(inn.id + k) % 3];
      L.add('box', mx, my + 0.08, mz, 0.07, 0.07, 0.2, a, col); L.add('box', mx + fx * 0.1, my + 0.13, mz + fz * 0.1, 0.05, 0.08, 0.07, a, col);
      for (const [lf, lr] of [[0.07, 0.025], [0.07, -0.025], [-0.07, 0.025], [-0.07, -0.025]]) L.add('post', mx + fx * lf + rx * lr, my, mz + fz * lf + rz * lr, 0.5, 0.09, 0.5, 0, col);
    }
    // han: yan kanat ve bira fıçıları
    const wing = (p: number) => {
      const [gx, gy2, gz] = at(-0.12, -0.52);
      L.add('box', gx, gy2 - 0.02, gz, 0.34, 0.04, 0.46, a + Math.PI / 2, stone);
      if (p > 0.2) L.add('house', gx, gy2, gz, 0.28, 0.22 * k01((p - 0.2) / 0.4), 0.42, a + Math.PI / 2, plaster);
      if (p > 0.7) L.add('gable', gx, gy2 + 0.22, gz, 0.34, 0.24 * k01((p - 0.7) / 0.3), 0.48, a + Math.PI / 2, roofC);
      if (p >= 1) for (const r of [-0.1, 0.1]) { const [px, , pz] = at(-0.12 + 0.145, -0.52 + r); L.add(wk++ >= lit ? 'winoff' : 'win', px, gy2 + 0.1, pz, 1.1, 1.1, 1, a, 0x2a2a30); }
    };
    if (lv >= 2) {
      wing(1);
      for (let k = 0; k < 3; k++) { const [px, py, pz] = at(0.12 + k * 0.08, -0.36); L.add('tower', px, py, pz, 0.045, 0.09, 0.045, 0, 0x6a4a2a); }
    }
    // kervansaray: avlu duvarı, kemerli kapı, gözcü kulesi, sancak
    if (lv >= 3) {
      const R = 1.02;
      for (let k = 0; k < 20; k++) {
        const q = (k / 20) * Math.PI * 2, f = Math.cos(q) * R, r = Math.sin(q) * R;
        if (f > 0.8 && Math.abs(r) < 0.36) continue;
        const [px, py, pz] = at(f, r);
        L.add('box', px, py - 0.02, pz, 0.34, 0.16, 0.07, a + q, 0xb8a888);
      }
      for (const s0 of [-1, 1]) L.add('tower', ...at(R, s0 * 0.26), 0.07, 0.3, 0.07, 0, 0xb8a888);
      const [gx, gy2, gz] = at(R, 0); L.add('box', gx, gy2 + 0.26, gz, 0.6, 0.06, 0.08, a + Math.PI / 2, 0xb8a888);
      const [tx2, ty2, tz2] = at(-0.72, 0.66); L.add('tower', tx2, ty2, tz2, 0.12, 0.62, 0.12, 0, 0xa89a80); L.add('spire', tx2, ty2 + 0.62, tz2, 0.18, 0.22, 0.18, 0, roofC);
      L.add('post', tx2, ty2 + 0.8, tz2, 1, 0.3, 1, 0, beam); L.add('flag', tx2, ty2 + 1.05, tz2, 1.1, 1.1, 1, a, 0xe8c24a);
    }
    // genişletme inşaatı: yeni kanat ya da üçüncü kat iskelesi
    if (b && b.level >= 2) {
      const p = Math.min(1, b.work / b.need);
      if (b.level === 2) { wing(p); piles(-0.42, -0.62); }
      else {
        piles(0.62, 0.62);
        for (const [f, r] of [[0.3, 0.4], [0.3, -0.4], [-0.3, 0.4], [-0.3, -0.4]]) L.add('post', ...at(f, r), 1.1, 0.72, 1.1, 0, 0xa8845a);
        for (const f of [0.3, -0.3]) { const [px, py, pz] = at(f, 0); L.add('box', px, py + 0.5, pz, 0.86, 0.015, 0.06, a, 0xa8845a); }
        if (p > 0.3) L.add('house', x, y + 0.46, z, 0.64, 0.2 * k01((p - 0.3) / 0.5), 0.46, a, 0xe4d2a8);
      }
    }
    // arkada mezarlık: burada yaşamış ölü kahramanlar
    const dead = w.heroes.filter((h) => h.state === 'dead' && h.baseInn && h.base === inn.id).slice(-10);
    if (dead.length) for (let k = -2; k <= 2; k++) { const [px, py, pz] = at(-0.62, k * 0.16); L.add('post', px, py, pz, 0.8, 0.12, 0.8, 0, 0x5a4330); }
    dead.forEach((h, k) => {
      const [gx, gy2, gz] = at(-0.5 - Math.floor(k / 5) * 0.14, -0.3 + (k % 5) * 0.15);
      L.add('box', gx, gy2 - 0.02, gz, 0.07, 0.13, 0.03, a + Math.PI / 2, 0xb8b2a6);
      L.add('disc', gx, gy2, gz, 0.1, 0.4, 0.14, a, 0x5f7a45);
      void h;
    });
    // efsane heykelleri
    const legends = w.heroes.filter((h) => h.legend && h.baseInn && h.base === inn.id).slice(0, 3);
    legends.forEach((h, k) => {
      const [tx3, ty3, tz3] = at(0.62, -0.05 - k * 0.2 - 0.3);
      L.add('box', tx3, ty3, tz3, 0.1, 0.08, 0.1, 0, 0x8d877e);
      L.add('body', tx3, ty3 + 0.08, tz3, 0.9, 0.9, 0.9, a, 0xb8b2a6); L.add('head', tx3, ty3 + 0.08, tz3, 0.9, 0.9, 0.9, a, 0xb8b2a6);
      void h;
    });
    // emekli öğretmen: avluda talim kazığı
    if (inn.teacher !== undefined) { const [px, py, pz] = at(0.35, -0.55); L.add('post', px, py, pz, 2, 0.35, 2, 0, 0x6a4a2a); L.add('box', px, py + 0.25, pz, 0.14, 0.03, 0.03, a, 0x6a4a2a); }
  }

  /** görsel bayram takvimi: hasat (sonbahar), kış ortası; ozanlarda bahar şenliği */
  festival(st: Settlement): string | null {
    if (st.tier < 1 || st.plague || st.starving > 0) return null;
    const sd = this.sim.day % 30, se = this.sim.season;
    if (se === 2 && sd >= 18 && sd < 25) return 'Hasat bayramı';
    if (se === 3 && sd >= 8 && sd < 12) return 'Kış şenliği';
    if (se === 0 && sd >= 10 && sd < 15 && this.sim.w.civs[st.civ].cls === 'bard') return 'Bahar şenliği';
    return null;
  }

  /** meydan, kuyu, pazar tezgâhları, çamaşır ipleri, bayram süsleri */
  private plazaSpot(st: Settlement, x: number, z: number, spots: [number, number][], wSpot: [number, number] | null): [number, number] {
    let pa = st.id * 1.7;
    const r = st.tier >= 2 ? 0.62 : st.tier >= 1 ? 0.5 : 0.55;
    for (let k = 0; k < 8; k++, pa += 0.8) {
      const px = x + Math.cos(pa) * r, pz = z + Math.sin(pa) * r;
      if (this.groundY(px, pz) > WATER_Y + 0.05 && !spots.some(([a, b]) => (a - px) ** 2 + (b - pz) ** 2 < 0.2) && !(wSpot && (wSpot[0] - px) ** 2 + (wSpot[1] - pz) ** 2 < 0.9)) return [px, pz];
    }
    return [x + 0.3, z + 0.3];
  }

  private townProps(st: Settlement, lay: TownLayout, civCol: number) {
    const L = this.dynamic;
    const { x, z } = lay;
    // barakalar: konutu yetmeyenler sur dışında çadır ve derme çatma kulübelerde
    const occHomes: [number, number][] = [];
    lay.houses.forEach((h, k) => { if (h.occ > 0) for (let q = 0; q < h.occ; q++) occHomes.push(lay.homes[k] ?? [h.x, h.z]); });
    if (lay.homeless > 0) {
      const n = Math.min(18, Math.ceil(lay.homeless / 3));
      const a0 = hash(st.id, 61) * 6.28, R0 = lay.wallR + 0.45;
      const spots: [number, number][] = [];
      for (let k = 0; k < n * 3 && spots.length < n; k++) {
        const a = a0 + (k - n / 2) * 0.32 / Math.max(1, R0 / 1.5), r = R0 + (k % 3) * 0.28 + hash(st.id, k + 70) * 0.12;
        const sx = x + Math.cos(a) * r, sz = z + Math.sin(a) * r;
        if (this.groundY(sx, sz) < WATER_Y + 0.05) continue;
        const ti = this.nearestTile(sx, sz);
        if (ti < 0 || this.sim.w.tiles[ti].terrain === 'mountain') continue;
        const sy = this.gy(sx, sz), ry = a + Math.PI / 2 + (hash(st.id, k + 80) - 0.5);
        const kind = hash(st.id, k + 90);
        if (kind < 0.5) L.add('tent', sx, sy, sz, 1.1, 0.9, 1.3, ry, [0x8a7a64, 0x6a5e50, 0x9a8a70, 0x7a6048][k % 4]);
        else {
          // derme çatma kulübe: yamalı duvar, eğik çatı
          L.add('box', sx, sy, sz, 0.2, 0.13, 0.17, ry, [0x6a5a48, 0x7a6a55, 0x5a4a3a][k % 3]);
          L.add('box', sx, sy + 0.13, sz, 0.24, 0.02, 0.21, ry, 0x4a4038, 0.25);
          L.add('box', sx + Math.cos(ry) * 0.05, sy + 0.06, sz - Math.sin(ry) * 0.05, 0.06, 0.07, 0.175, ry, 0x8a7a64);
        }
        spots.push([sx, sz]);
        for (let q = 0; q < 3; q++) occHomes.push([sx, sz]);
      }
      if (spots.length) {
        const [fx, fz] = spots[Math.floor(spots.length / 2)];
        const fx2 = fx + Math.cos(a0) * 0.25, fz2 = fz + Math.sin(a0) * 0.25;
        L.add('log', fx2, this.gy(fx2, fz2) + 0.03, fz2, 0.7, 1, 0.7, 0.4, 0x4a3a2a); L.add('log', fx2, this.gy(fx2, fz2) + 0.03, fz2, 0.7, 1, 0.7, 2, 0x4a3a2a);
        lay.work.shanty = spots; lay.work.shantyFire = [[fx2, fz2]];
      }
    }
    if (occHomes.length) lay.work.occHomes = occHomes;
    // meydan ve kuyu
    const [px, pz] = lay.work.plaza[0];
    const py = this.gy(px, pz);
    L.add('tower', px, py, pz, 0.2, 0.12, 0.2, 0, 0x9a948a);
    L.add('disc', px, py + 0.115, pz, 0.12, 0.3, 0.12, 0, 0x2a4a6a);
    for (const e of [-1, 1]) L.add('post', px + e * 0.09, py, pz, 1, 0.28, 1, 0, 0x5a4330);
    L.add('roof', px, py + 0.27, pz, 0.28, 0.2, 0.2, 0, 0x7a4a2a);
    L.add('box', px + 0.09, py + 0.11, pz + 0.02, 0.05, 0.05, 0.05, 0, 0x6a4a2a);
    // pazar tezgâhları: renkli tente, mal sepetleri
    const mk = lay.work.market?.[0];
    if (mk) {
      const stalls: [number, number][] = [];
      const GOODS_C = [0xd8a03a, 0xb8442e, 0x6aa04a, 0xe8d8a0, 0x8a5a9a, 0x9fb4c0];
      for (let k = 0; k < 3 + Math.min(2, st.tier); k++) {
        const a = k * 1.35 + st.id, sx = mk[0] + Math.cos(a) * 0.36, sz = mk[1] + Math.sin(a) * 0.36;
        if (this.groundY(sx, sz) < WATER_Y + 0.05) continue;
        const sy = this.gy(sx, sz), ry = -a + Math.PI / 2;
        L.add('box', sx, sy, sz, 0.22, 0.07, 0.1, ry, 0x7a5a3a);
        for (const e of [-1, 1]) { const ox = Math.cos(ry) * e * 0.1, oz = -Math.sin(ry) * e * 0.1; L.add('post', sx + ox, sy, sz + oz, 0.7, 0.2, 0.7, 0, 0x5a4330); }
        L.add('box', sx, sy + 0.19, sz, 0.26, 0.015, 0.16, ry, k % 2 ? civCol : 0xf2ecd8, 0.25);
        for (let g = 0; g < 3; g++) { const ox = Math.cos(ry) * (g - 1) * 0.06, oz = -Math.sin(ry) * (g - 1) * 0.06; L.add('dot', sx + ox, sy + 0.08, sz + oz, 0.55, 0.45, 0.55, 0, GOODS_C[(k * 3 + g + st.id) % GOODS_C.length]); }
        stalls.push([sx + Math.cos(a) * 0.12, sz + Math.sin(a) * 0.12]);
      }
      lay.work.stall = stalls;
    }
    // çamaşır ipleri: yakın ev çiftleri arasında
    if (st.tier >= 1) {
      let lines = 0;
      for (let i = 0; i < lay.homes.length && lines < 1 + st.tier; i++) for (let j = i + 1; j < lay.homes.length && lines < 1 + st.tier; j++) {
        const [ax, az] = lay.homes[i], [bx, bz] = lay.homes[j];
        const d = Math.hypot(bx - ax, bz - az);
        if (d < 0.3 || d > 0.55 || hash(st.id * 13 + i, j) > 0.35) continue;
        const ay = this.gy(ax, az), by = this.gy(bx, bz), top = Math.max(ay, by) + 0.2;
        L.add('post', ax, ay, az, 0.6, top - ay, 0.6, 0, 0x6a5a4a); L.add('post', bx, by, bz, 0.6, top - by, 0.6, 0, 0x6a5a4a);
        const ry = Math.atan2(bx - ax, bz - az);
        L.add('box', (ax + bx) / 2, top - 0.005, (az + bz) / 2, 0.006, 0.006, d, ry, 0xd8d0c0);
        for (let c = 0; c < 3; c++) { const q = 0.25 + c * 0.25; L.add('box', ax + (bx - ax) * q, top - 0.06, az + (bz - az) * q, 0.004, 0.055, 0.07, ry, [0xf2ecd8, 0x6a8ab0, 0xb04a3a, 0xd8b84a][(c + i) % 4]); }
        lines++;
      }
    }
    // bayram: meydanın çevresinde direkler ve flama dizileri; gece şenlik ateşi
    if (this.festival(st)) {
      const R = 0.42, N = 6;
      const cols = [0xd84a3a, 0xf2cf5b, 0x4a8ad8, 0x6ab04a, civCol];
      for (let k = 0; k < N; k++) {
        const a1 = (k / N) * Math.PI * 2, a2 = ((k + 1) / N) * Math.PI * 2;
        const x1 = px + Math.cos(a1) * R, z1 = pz + Math.sin(a1) * R, x2 = px + Math.cos(a2) * R, z2 = pz + Math.sin(a2) * R;
        const y1 = this.gy(x1, z1);
        L.add('post', x1, y1, z1, 0.8, 0.42, 0.8, 0, 0x6a4a2a);
        const ry = Math.atan2(x2 - x1, z2 - z1), top = y1 + 0.4;
        for (let f = 0; f < 4; f++) { const q = (f + 0.5) / 4; L.add('flag', x1 + (x2 - x1) * q, top - 0.03 - Math.sin(q * Math.PI) * 0.05, z1 + (z2 - z1) * q, 0.28, 0.35, 1, ry + Math.PI / 2, cols[(f + k) % cols.length], 0, 0); }
      }
      L.add('log', px + 0.2, py + 0.03, pz - 0.15, 0.8, 1, 0.8, 0.6, 0x4a3a2a); L.add('log', px + 0.2, py + 0.03, pz - 0.15, 0.8, 1, 0.8, 2.2, 0x4a3a2a);
      lay.work.bonfire = [[px + 0.2, pz - 0.15]];
    }
    // salgın: meydanda sarı karantina bayrağı
    if (st.plague) { L.add('post', px - 0.2, py, pz + 0.15, 1, 0.6, 1, 0, 0x3a3a3a); L.add('flag', px - 0.2, py + 0.55, pz + 0.15, 0.9, 0.9, 1, 0, 0xe0c030); }
  }

  /** ırka göre ev */
  /** son çizilen evin baca ağzı (duman buradan çıkar); bacasız evlerde çatı tepesi */
  private chimTop: [number, number, number] | null = null;
  private house(race: string, x: number, y: number, z: number, sx: number, sy: number, a: number, civ: number, tier: number, r: number) {
    const L = this.dynamic;
    this.chimTop = null;
    const face = (d: number): [number, number] => [x + Math.sin(a) * d, z + Math.cos(a) * d];
    // pencere: fd öne, ld yana, h yükseklik; side=yan duvar
    let wk = 0;
    const ws = Math.max(0.7, Math.min(1.3, sx / 0.32));
    const win = (fd: number, ld: number, h: number, side = false) => {
      const px = x + Math.sin(a) * fd + Math.cos(a) * ld, pz = z + Math.cos(a) * fd - Math.sin(a) * ld;
      const k = wk++;
      const lit = k >= this.hLit ? 'winoff' : hash(Math.floor(r * 1e4), k) > 0.45 ? 'win' : 'win2';
      L.add(lit, px, y + h, pz, ws, ws, 1, side ? a + Math.PI / 2 : a, 0x2a2a30);
    };
    switch (race) {
      case 'dwarf': {
        // taş, basık, yuvarlak çatılı; bacalı
        L.add('house', x, y, z, sx * 1.15, sy * 0.8, sx * 1.1, a, 0xa9a39a);
        L.add('dome', x, y + sy * 0.8, z, sx * 1.1, sy * 0.9, sx * 1.05, a, 0x7d776f);
        const [dx, dz] = face(sx * 0.56); L.add(this.hLit > 0 ? 'door' : 'door0', dx, y, dz, 1.1, 1, 1, a, 0x4a3322);
        win(sx * 0.57, sx * 0.33, sy * 0.3); if (r > 0.4) win(sx * 0.57, -sx * 0.33, sy * 0.3);
        L.add('box', x + Math.cos(a) * sx * 0.35, y + sy * 0.8, z - Math.sin(a) * sx * 0.35, 0.07, sy * 0.9, 0.07, a, 0x6d6760);
        this.chimTop = [x + Math.cos(a) * sx * 0.35, y + sy * 1.7, z - Math.sin(a) * sx * 0.35];
        break;
      }
      case 'halfling': {
        // çimenli höyük, yuvarlak kapı
        L.add('dome', x, y - 0.02, z, sx * 1.5, sy * 1.6, sx * 1.5, a, tier >= 2 ? 0x6fa04a : 0x7cab52);
        const [dx, dz] = face(sx * 0.68);
        L.add(this.hLit > 0 ? 'door' : 'door0', dx, y, dz, 1.2, 1, 1, a, civ);
        // pencereler kubbenin yüzeyinde, yüzeye dik (eskiden havada asılı kara kareler)
        for (const e of [-1, 1]) {
          const q = a + e * 0.62, R = sx * 0.69;
          const k = wk++;
          L.add(k >= this.hLit ? 'winoff' : 'win', x + Math.sin(q) * R, y + sy * 0.3, z + Math.cos(q) * R, ws * 1.1, ws * 1.1, 1, q, 0x2a2a30);
        }
        // baca kubbenin sırtına oturur (eskiden kubbenin üstünde havada asılıydı)
        { const cx = x - Math.cos(a) * sx * 0.35, cz = z + Math.sin(a) * sx * 0.35, cy = y + sy * 0.6; L.add('box', cx, cy, cz, 0.045, 0.1, 0.045, a, 0x8a7a6a); L.add('box', cx, cy + 0.1, cz, 0.06, 0.018, 0.06, a, 0x6d6258); this.chimTop = [cx, cy + 0.12, cz]; }
        break;
      }
      case 'gnome': {
        // mantar ev
        L.add('tower', x, y, z, sx * 0.55, sy * 1.5, sx * 0.55, a, 0xefe6d2);
        L.add('dome', x, y + sy * 1.4, z, sx * 1.4, sy * 1.1, sx * 1.4, a, r > 0.5 ? 0xc0392b : civ);
        for (let k = 0; k < 3; k++) { const q = a + k * 2.1; L.add('dot', x + Math.cos(q) * sx * 0.4, y + sy * 1.4 + sy * 0.75, z + Math.sin(q) * sx * 0.4, 1, 0.6, 1, 0, 0xffffff); }
        const [dx, dz] = face(sx * 0.3); L.add(this.hLit > 0 ? 'door' : 'door0', dx, y, dz, 0.9, 1, 1, a, 0x6a4a2a);
        win(sx * 0.3, 0, sy * 0.85);
        break;
      }
      case 'elf': case 'halfelf': {
        // ince yuvarlak ahşap ev, yüksek sivri çatı
        L.add('tower', x, y, z, sx * 0.7, sy * 1.3, sx * 0.7, a, 0xd9c9a0);
        L.add('spire', x, y + sy * 1.3, z, sx * 0.9, sy * 1.6, sx * 0.9, a, r > 0.45 ? 0x4f8a4a : civ);
        const [dx, dz] = face(sx * 0.36); L.add(this.hLit > 0 ? 'door' : 'door0', dx, y, dz, 0.9, 1.1, 1, a, 0x7a5a3a);
        win(sx * 0.37, 0, sy * 0.8); win(0, sx * 0.37, sy * 0.6, true);
        break;
      }
      case 'halforc': {
        if (tier === 0 || r > 0.7) { L.add('tent', x, y, z, sx * 3.6, sy * 4.2, sx * 3.6, a, r > 0.5 ? 0x8a6a45 : 0x6b4f33); L.add('post', x, y + sy * 1.4, z, 1, 0.25, 1, 0, 0x3a2a1a); break; }
        // uzun ev: koyu ahşap, sivri kazıklı
        L.add('house', x, y, z, sx * 0.9, sy * 0.9, sx * 1.4, a, 0x6b4f33);
        L.add('gablew', x, y + sy * 0.9, z, sx * 1.04, sy * 0.9, sx * 1.4, a, 0x6b4f33);
        L.add('roofslab', x, y + sy * 0.9, z, sx * 1.04, sy * 0.9, sx * 1.4, a, 0x4a3522);
        for (const e of [-1, 1]) { const [px, pz] = face(e * sx * 0.75); L.add('post', px, y + sy * 0.9, pz, 1.2, 0.25, 1.2, 0, 0xe8dcc0); }
        win(sx * 0.25, sx * 0.46, sy * 0.4, true); win(-sx * 0.25, -sx * 0.46, sy * 0.4, true);
        break;
      }
      case 'warlock': {
        // bazalt, basık; mor ışıklı kapı, sivri kara çatı
        L.add('house', x, y, z, sx * 1.1, sy * 0.85, sx * 1.1, a, 0x3a3440);
        L.add('spire', x, y + sy * 0.85, z, sx * 1.0, sy * 1.2, sx * 1.0, a, 0x251f2a);
        const [dx, dz] = face(sx * 0.57); L.add(this.hLit > 0 ? 'door' : 'door0', dx, y, dz, 1.1, 1.1, 1, a, 0x6a3a9a);
        win(sx * 0.56, sx * 0.3, sy * 0.4);
        if (r > 0.55) L.add('crystal', x - Math.cos(a) * sx * 0.45, y, z + Math.sin(a) * sx * 0.45, 0.6, 0.6, 0.6, a, 0x9a4aff);
        break;
      }
      case 'dragonborn': {
        // kalın taş, kiremit rengi çatı, kapıda mangal
        L.add('house', x, y, z, sx * 1.1, sy * 0.95, sx * 1.2, a, 0x8a6f5a);
        L.add('gablew', x, y + sy * 0.95, z, sx * 1.27, sy * 0.8, sx * 1.2, a, 0x8a6f5a);
        L.add('roofslab', x, y + sy * 0.95, z, sx * 1.27, sy * 0.8, sx * 1.2, a, 0xb4532a);
        const [dx, dz] = face(sx * 0.65); L.add(this.hLit > 0 ? 'door' : 'door0', dx, y, dz, 1.1, 1.1, 1, a, 0x3a2a1a);
        win(sx * 0.61, sx * 0.32, sy * 0.45); win(sx * 0.61, -sx * 0.32, sy * 0.45);
        const [bx, bz] = face(sx * 0.9); L.add('lantern', bx, y + 0.08, bz, 0.9, 0.9, 0.9, 0, 0xff8a3a);
        break;
      }
      case 'tiefling': {
        // koyu taş kule-ev, sivri kızıl çatı, mor pencere ışığı
        L.add('tower', x, y, z, sx * 0.75, sy * 1.5, sx * 0.75, a, 0x4a3a4f);
        L.add('spire', x, y + sy * 1.5, z, sx * 0.95, sy * 1.9, sx * 0.95, a, r > 0.5 ? 0x8a1f3a : civ);
        const [dx, dz] = face(sx * 0.38); L.add(this.hLit > 0 ? 'door' : 'door0', dx, y, dz, 0.9, 1.2, 1, a, 0x2a1a2a);
        win(sx * 0.4, 0, sy * 0.95); win(0, sx * 0.4, sy * 0.6, true);
        if (r > 0.6) L.add('crystal', x + Math.cos(a) * sx * 0.5, y + sy * 1.2, z - Math.sin(a) * sx * 0.5, 0.5, 0.5, 0.5, 0, 0xb04aff);
        break;
      }
      default: {
        // insan: Blender modeli (assets/blender/human_house.py) — yarı ahşap, taş temel, kiremit çatı (medeniyet rengi)
        // model normu: genişlik sx (5 m), derinlik 1.25·sx, duvar üstü sy (4 m); kapı ve ışıklı pencereler modeldeki yuvalara oturur
        const v = r > 0.35 ? 'human_house_a' : 'human_house_b';
        L.add(v, x, y, z, sx, sy, sx, a, jitter(civ, r * 3.1, 0.08, 0.02));
        const [dx, dz] = face(sx * 0.632);
        L.add(this.hLit > 0 ? 'door' : 'door0', dx, y + sy * 0.0875, dz, sx / 0.333, sy / 0.267, 1, a, 0x5a3a22);
        // ışık örnekleri: ön iki pencere, yanlarda birer (model: alt 1.10 m, 1.05 × 1.05 m)
        let k = 0;
        const pane = (fd: number, ld: number, side: boolean) => {
          const px = x + Math.sin(a) * fd + Math.cos(a) * ld, pz = z + Math.cos(a) * fd - Math.sin(a) * ld;
          const kk = k++;
          const lit = kk >= this.hLit ? 'winoff' : hash(Math.floor(r * 1e4), kk) > 0.45 ? 'win' : 'win2';
          L.add(lit, px, y + sy * 0.275, pz, sx * 3, sy * 3.75, 0.3, side ? a + Math.PI / 2 : a, 0x2a2a30);
        };
        pane(sx * 0.625 + 0.0035, sx * 0.29, false); pane(sx * 0.625 + 0.0035, -sx * 0.29, false);
        pane(0, sx * 0.5 + 0.0035, true); pane(0, -(sx * 0.5 + 0.0035), true);
        if (v === 'human_house_a') this.chimTop = [x + Math.sin(a) * -0.26 * sx + Math.cos(a) * 0.24 * sx, y + sy * 2.16, z + Math.cos(a) * -0.26 * sx - Math.sin(a) * 0.24 * sx];
      }
    }
  }
  /** merkez yapı: ırk ve kademeye göre */
  private hall(race: string, x: number, y: number, z: number, civ: number, tier: number, castle: boolean) {
    const L = this.dynamic;
    if (tier >= 1) switch (race) {
      case 'dwarf':
        L.add('house', x, y, z, 0.6, 0.38, 0.55, 0.3, 0x9a948a); L.add('dome', x, y + 0.38, z, 0.62, 0.45, 0.58, 0.3, 0x6d6760);
        L.add('box', x + 0.2, y + 0.3, z - 0.1, 0.1, 0.55, 0.1, 0.3, 0x5a554f);
        break;
      case 'halfling':
        L.add('dome', x, y - 0.03, z, 1.0, 0.75, 1.0, 0, 0x6fa04a); L.add('door', x + 0.48, y, z, 1.8, 1.6, 1.4, Math.PI / 2, civ);
        break;
      case 'gnome':
        L.add('tower', x, y, z, 0.26, 0.9, 0.26, 0, 0xefe6d2); L.add('dome', x, y + 0.88, z, 0.9, 0.5, 0.9, 0, civ);
        for (let k = 0; k < 5; k++) L.add('dot', x + Math.cos(k * 1.3) * 0.28, y + 1.18, z + Math.sin(k * 1.3) * 0.28, 1.3, 0.7, 1.3, 0, 0xffffff);
        break;
      case 'elf': case 'halfelf':
        L.add('tower', x, y, z, 0.32, 0.8, 0.32, 0, 0xd9c9a0); L.add('spire', x, y + 0.8, z, 0.45, 1.2, 0.45, 0, 0x4f8a4a);
        L.add('blob', x + 0.35, y, z + 0.2, 1.3, 1.6, 1.3, 0, 0x3f8a3f);
        break;
      case 'halforc':
        L.add('house', x, y, z, 0.5, 0.36, 0.85, 0.3, 0x5a4230); L.add('gable', x, y + 0.36, z, 0.65, 0.45, 0.95, 0.3, 0x3a2a1a);
        for (let k = 0; k < 4; k++) L.add('post', x - 0.3 + k * 0.2, y + 0.36, z + 0.45, 1.3, 0.35, 1.3, 0, 0xe8dcc0);
        break;
      case 'dragonborn':
        L.add('house', x, y, z, 0.62, 0.45, 0.62, 0.3, 0x7a5f4a); L.add('box', x, y + 0.45, z, 0.66, 0.08, 0.66, 0.3, 0x5a4535);
        for (let k = 0; k < 4; k++) L.add('lantern', x + Math.cos(0.3 + k * Math.PI / 2 + Math.PI / 4) * 0.42, y + 0.55, z - Math.sin(0.3 + k * Math.PI / 2 + Math.PI / 4) * 0.42, 1, 1, 1, 0, 0xff8a3a);
        break;
      case 'warlock':
        for (let k = 0; k < 3; k++) L.add('box', x, y + k * 0.28, z, 0.85 - k * 0.22, 0.28, 0.85 - k * 0.22, 0.3, k % 2 ? 0x2e2833 : 0x3a3440);
        L.add('glow', x, y + 0.95, z, 1.1, 1.1, 1.1, 0, 0xa05aff);
        for (let k = 0; k < 4; k++) { const q = 0.3 + k * Math.PI / 2 + Math.PI / 4; L.add('post', x + Math.cos(q) * 0.55, y, z - Math.sin(q) * 0.55, 1.6, 0.9, 1.6, 0, 0x251f2a); L.add('lantern', x + Math.cos(q) * 0.55, y + 0.95, z - Math.sin(q) * 0.55, 0.8, 0.8, 0.8, 0, 0x9a4aff); }
        break;
      case 'tiefling':
        L.add('tower', x, y, z, 0.3, 1.1, 0.3, 0, 0x3e3044); L.add('spire', x, y + 1.1, z, 0.4, 1.0, 0.4, 0, 0x8a1f3a);
        L.add('glow', x, y + 1.0, z, 0.9, 0.9, 0.9, 0, 0xd06aff);
        break;
      default:
        L.add('house', x, y, z, 0.48, 0.38, 0.62, 0.3, 0xe9dcc0); L.add('gable', x, y + 0.38, z, 0.58, 0.45, 0.7, 0.3, civ);
    }
    if (tier >= 2 || castle) {
      const stone = race === 'warlock' ? 0x2e2833 : race === 'elf' || race === 'halfelf' ? 0xd9c9a0 : race === 'halforc' ? 0x5a4230 : race === 'tiefling' ? 0x3e3044 : race === 'dragonborn' ? 0x7a5f4a : 0xa9a39a;
      L.add('tower', x + 0.25, y, z - 0.3, 0.22, 1.1, 0.22, 0, stone); L.add('spire', x + 0.25, y + 1.1, z - 0.3, 0.28, 0.6, 0.28, 0, civ);
      if (tier >= 3 || castle) { L.add('tower', x - 0.3, y, z - 0.2, 0.2, 0.85, 0.2, 0, stone); L.add('spire', x - 0.3, y + 0.85, z - 0.2, 0.26, 0.5, 0.26, 0, civ); }
    }
  }
  /** kömürleşmiş bina kalıntısı */
  private charred(x: number, y: number, z: number, sx: number, sy: number, a: number) {
    const L = this.dynamic;
    L.add('house', x, y, z, sx, sy * 0.55, sx * 1.1, a, 0x2a2522);
    L.add('log', x, y + sy * 0.6, z, sx * 2.2, 1, 1, a + 0.4, 0x1a1614, 0, 0.3);
    L.add('log', x + 0.05, y + sy * 0.5, z - 0.04, sx * 1.8, 1, 1, a - 0.5, 0x1a1614, 0, -0.25);
  }
  /** şantiye: iskele ve yükselen çatkı */
  private site(x: number, y: number, z: number, a: number, p: number) {
    const L = this.dynamic;
    const H = 0.08 + p * 0.3;
    L.add('box', x, y, z, 0.34, H, 0.3, a, 0xc9b48a);
    for (const [dx, dz] of [[-0.19, -0.17], [0.19, -0.17], [-0.19, 0.17], [0.19, 0.17]]) L.add('post', x + dx, y, z + dz, 0.8, 0.45, 0.8, 0, 0x8a6a45);
    L.add('box', x, y + 0.42, z, 0.44, 0.02, 0.4, a, 0x8a6a45);
    for (let k = 0; k < 3; k++) L.add('log', x + 0.35, y + 0.04 + k * 0.05, z + 0.1, 0.8, 1, 1, a, 0xb89a6a);
  }
  /** atölye ve kamu yapıları: her birinin kendine özgü bir işareti var */
  private workshopModel(kind: string, x: number, y: number, z: number, a: number, civ: number, race: string) {
    const L = this.dynamic;
    const wall = race === 'dwarf' || race === 'gnome' ? 0xb0a99e : race === 'tiefling' ? 0x4a3a4f : race === 'dragonborn' ? 0x8a6f5a : race === 'halforc' ? 0x6b4f33 : 0xe2d4b4;
    const fx = Math.sin(a), fz = Math.cos(a), rx = Math.cos(a), rz = -Math.sin(a);
    // atölye gövdesi: taş temel, duvar, duvar renginde üçgen alın ve saçaklı çatı kaplaması
    const body = (w: number, hgt: number, d: number, roof: number, col = wall) => {
      L.add('box', x, y - 0.03, z, w * 1.07, 0.06, d * 1.07, a, 0x8d877e);
      L.add('house', x, y, z, w, hgt, d, a, col);
      L.add('gablew', x, y + hgt, z, w * 1.155, hgt * 0.9, d, a, col);
      L.add('roofslab', x, y + hgt, z, w * 1.155, hgt * 0.9, d, a, roof);
    };
    const chimney = (h: number) => { const cx = x - rx * 0.1, cz = z - rz * 0.1; L.add('box', cx, y + h, cz, 0.07, 0.22, 0.07, a, 0x6d6760); this.smokes.push([cx, y + h + 0.22, cz]); };
    switch (kind) {
      case 'bakery': case 'brickkiln':
        body(0.32, 0.24, 0.3, 0x9a5a3a);
        L.add('dome', x + rx * 0.3, y, z + rz * 0.3, 0.32, 0.4, 0.32, 0, kind === 'bakery' ? 0xc07a4a : 0xa0522d);
        L.add('lantern', x + rx * 0.3 + fx * 0.13, y + 0.05, z + rz * 0.3 + fz * 0.13, 0.6, 0.5, 0.6, 0, 0xff8a3a);
        chimney(0.3); break;
      case 'brewery':
        body(0.36, 0.26, 0.3, 0x6a4a2a);
        for (let k = 0; k < 3; k++) L.add('tower', x + rx * (0.28 + k * 0.11) + fx * 0.15, y, z + rz * (0.28 + k * 0.11) + fz * 0.15, 0.07, 0.13, 0.07, 0, 0x8a5a33);
        break;
      case 'sawmill':
        body(0.34, 0.22, 0.28, 0x7a5a3a, 0xa08060);
        for (let k = 0; k < 4; k++) L.add('log', x + rx * 0.32, y + 0.04 + (k % 2) * 0.07, z + rz * 0.32 + (k >> 1) * 0.1 - 0.05, 1, 1, 1, a + Math.PI / 2, 0x9a6a3a);
        L.add('box', x + fx * 0.22, y + 0.06, z + fz * 0.22, 0.3, 0.02, 0.06, a, 0xc9b48a);
        break;
      case 'foundry': case 'smithy': case 'toolmaker': case 'armory': case 'mithrilforge':
        body(0.34, 0.24, 0.3, 0x3e3a38, 0x8d877e);
        L.add('box', x + fx * 0.24, y, z + fz * 0.24, 0.08, 0.08, 0.05, a, 0x3a3a3a);
        L.add('lantern', x + fx * 0.16 + rx * 0.1, y + 0.08, z + fz * 0.16 + rz * 0.1, 0.8, 0.6, 0.8, 0, kind === 'mithrilforge' ? 0x8fe6f0 : 0xff6a1a);
        if (kind === 'armory') for (let k = 0; k < 3; k++) L.add('post', x + rx * (0.25 + k * 0.06), y, z + rz * (0.25 + k * 0.06), 0.8, 0.35, 0.8, 0, 0x9aa0a8);
        chimney(0.32); break;
      case 'apothecary':
        body(0.3, 0.22, 0.28, 0x4f8a4a);
        L.add('blob', x + rx * 0.26, y, z + rz * 0.26, 0.5, 0.4, 0.5, 0, 0x6fae4a);
        break;
      case 'scriptorium': case 'library':
        body(0.36, 0.34, 0.32, 0x3d5a8a);
        L.add('tower', x - rx * 0.2, y, z - rz * 0.2, 0.1, 0.6, 0.1, 0, wall); L.add('spire', x - rx * 0.2, y + 0.6, z - rz * 0.2, 0.13, 0.25, 0.13, 0, 0x3d5a8a);
        break;
      case 'enchanter':
        body(0.3, 0.26, 0.3, 0x5a3d8a);
        L.add('crystal', x + rx * 0.25, y, z + rz * 0.25, 1, 1.2, 1, 0, 0xb08aff);
        break;
      case 'market':
        for (let k = 0; k < 3; k++) {
          const px = x + rx * (k - 1) * 0.3, pz = z + rz * (k - 1) * 0.3;
          for (const [dx, dz] of [[-0.1, -0.08], [0.1, -0.08], [-0.1, 0.08], [0.1, 0.08]]) L.add('post', px + dx, y, pz + dz, 0.6, 0.24, 0.6, 0, 0x7a5533);
          L.add('box', px, y + 0.24, pz, 0.26, 0.02, 0.22, a, [0xc0392b, 0xe8c24a, 0x3d7ab8][(k + Math.round(x * 3)) % 3], 0.15);
          L.add('box', px, y + 0.08, pz, 0.18, 0.06, 0.1, a, 0x9b7650);
        }
        break;
      case 'tavern':
        body(0.44, 0.3, 0.36, 0x7a3a2a);
        L.add('post', x + fx * 0.3 + rx * 0.2, y, z + fz * 0.3 + rz * 0.2, 0.8, 0.45, 0.8, 0, 0x5a4330);
        L.add('flag', x + fx * 0.3 + rx * 0.2, y + 0.4, z + fz * 0.3 + rz * 0.2, 0.6, 0.6, 1, a, 0xe8c24a);
        L.add('lantern', x + fx * 0.25, y + 0.26, z + fz * 0.25, 1, 1, 1, 0, 0xffc45a);
        chimney(0.34); break;
      case 'temple':
        body(0.34, 0.3, 0.4, 0xe8e0c8, 0xf2ecdc);
        L.add('spire', x - fz * 0.12, y + 0.55, z + fx * 0.12, 0.12, 0.35, 0.12, 0, 0xe8c24a);
        break;
      case 'guild':
        body(0.42, 0.32, 0.32, civ);
        L.add('post', x + fx * 0.25, y, z + fz * 0.25, 1, 0.8, 1, 0, 0x5a4330); L.add('flag', x + fx * 0.25, y + 0.75, z + fz * 0.25, 0.9, 0.9, 1, a, civ);
        break;
      case 'mint':
        body(0.3, 0.26, 0.3, 0x8d877e, 0xb0aa9f);
        L.add('dot', x + fx * 0.22, y + 0.05, z + fz * 0.22, 1, 0.5, 1, 0, 0xe8c24a);
        break;
      case 'unique':
        L.add('tower', x, y, z, 0.16, 0.75, 0.16, 0, civ); L.add('glow', x, y + 0.75, z, 0.8, 0.8, 0.8, 0, 0xffffff);
        break;
      default: body(0.3, 0.24, 0.28, civ);
    }
  }
  private wonderSpot(tile: number): [number, number] {
    const [x, z] = this.pos(tile);
    const w = this.sim.w;
    const ns = this.sim.g.neighbors(tile).filter((n) => w.tiles[n].terrain !== 'water' && w.tiles[n].terrain !== 'mountain');
    const n = ns.length ? ns.slice().sort((a, b) => hash(a, 5) - hash(b, 5))[0] : tile;
    const [nx, nz] = this.pos(n);
    return [x + (nx - x) * 0.75, z + (nz - z) * 0.75];
  }
  /** sınıf harikası; p<1 iken iskele içinde yükselir */
  private wonder(cls: string, x: number, z: number, p: number, civ: number) {
    const L = this.dynamic;
    const y = this.gy(x, z);
    L.add('box', x, y - 0.1, z, 1.35, 0.25, 1.35, 0.3, 0xcfc6b2);
    if (p < 1) {
      const H = 0.3 + p * 1.8;
      L.add('box', x, y + 0.1, z, 0.9, H * 0.8, 0.9, 0.3, 0xb8ad96);
      for (let k = 0; k < 8; k++) { const a = (k / 8) * Math.PI * 2 + 0.3; L.add('post', x + Math.cos(a) * 0.62, y + 0.1, z + Math.sin(a) * 0.62, 1.2, H + 0.2, 1.2, 0, 0x8a6a45); }
      for (let q = 1; q <= Math.floor(H / 0.35); q++) L.add('box', x, y + 0.1 + q * 0.35, z, 1.3, 0.025, 1.3, 0.3, 0x8a6a45);
      return;
    }
    const Y = y + 0.15;
    switch (cls) {
      case 'paladin':
        L.add('house', x, Y, z, 0.95, 0.75, 0.95, 0.3, 0xf2ecdc); L.add('dome', x, Y + 0.75, z, 1.0, 0.75, 1.0, 0, 0xe8c24a); L.add('spire', x, Y + 1.25, z, 0.22, 0.8, 0.22, 0, 0xe8c24a);
        for (let k = 0; k < 4; k++) { const a = k * Math.PI / 2 + 0.3 + Math.PI / 4; L.add('tower', x + Math.cos(a) * 0.62, Y, z + Math.sin(a) * 0.62, 0.14, 1.1, 0.14, 0, 0xf2ecdc); L.add('spire', x + Math.cos(a) * 0.62, Y + 1.1, z + Math.sin(a) * 0.62, 0.18, 0.4, 0.18, 0, civ); }
        break;
      case 'cleric': case 'warlock':
        for (let k = 0; k < 4; k++) L.add('box', x, Y + k * 0.32, z, 1.2 - k * 0.25, 0.32, 1.2 - k * 0.25, 0.3, k % 2 ? 0x8d877e : 0xa39c90);
        L.add('glow', x, Y + 1.3, z, 1.6, 1.6, 1.6, 0, cls === 'warlock' ? 0x9a4aff : 0x8fd8ff);
        break;
      case 'druid': case 'ranger':
        L.add('trunk', x, Y, z, 5.5, 9, 5.5, 0, 0x6b4a2b);
        for (let k = 0; k < 6; k++) { const a = k * 1.05; L.add('blob', x + Math.cos(a) * 0.55, Y + 1.7 + (k % 2) * 0.35, z + Math.sin(a) * 0.55, 2.6, 2.4, 2.6, a, k % 2 ? 0x3f8a3f : 0x4f9a4a); }
        L.add('blob', x, Y + 2.3, z, 3, 2.6, 3, 0, 0x5aa54f);
        if (cls === 'ranger') { L.add('disc', x, Y + 1.1, z, 1.1, 1, 1.1, 0, 0x8a6a45); L.add('post', x + 0.5, Y + 1.1, z, 1, 0.9, 1, 0, 0x5a4330); L.add('flag', x + 0.5, Y + 1.9, z, 1.2, 1.2, 1, 0, civ); }
        else L.add('glow', x, Y + 1.5, z + 0.3, 1.2, 1.2, 1.2, 0, 0x9fe6a0);
        break;
      case 'rogue':
        L.add('house', x, Y, z, 1.2, 0.5, 0.9, 0.3, 0xe0c68a); L.add('dome', x, Y + 0.5, z, 0.7, 0.6, 0.7, 0, 0xe8c24a);
        for (let k = 0; k < 6; k++) L.add('post', x - 0.55 + k * 0.22, Y, z + 0.52, 2.2, 0.5, 2.2, 0, 0xf2ecdc);
        L.add('box', x, Y + 0.5, z + 0.52, 1.25, 0.06, 0.12, 0.3, civ);
        break;
      case 'wizard':
        L.add('tower', x, Y, z, 0.42, 2.6, 0.42, 0, 0x7a7090); L.add('spire', x, Y + 2.6, z, 0.55, 0.9, 0.55, 0, 0x6a3dbd);
        L.add('glow', x, Y + 3.8 + Math.sin(this.sim.day * 0.3) * 0.05, z, 1.5, 1.5, 1.5, 0, 0xb08aff);
        for (let k = 0; k < 3; k++) L.add('ring', x, Y + 1 + k * 0.6, z, 0.5, 2, 0.5, 0, 0xe8c24a);
        break;
      case 'barbarian':
        L.add('box', x, Y, z, 0.8, 0.9, 0.5, 0.3, 0x4a3522); L.add('box', x, Y + 0.9, z - 0.15, 0.8, 0.8, 0.14, 0.3, 0x3a2a1a);
        for (let k = 0; k < 10; k++) { const a = (k / 10) * Math.PI * 2; L.add('post', x + Math.cos(a) * 0.62, Y, z + Math.sin(a) * 0.62, 1.4, 1.2 + (k % 3) * 0.2, 1.4, 0, 0xe8dcc0); L.add('dot', x + Math.cos(a) * 0.62, Y + 1.25 + (k % 3) * 0.2, z + Math.sin(a) * 0.62, 1.2, 1.2, 1.2, 0, 0xf2ecdc); }
        L.add('flag', x, Y + 1.7, z, 1.5, 1.5, 1, 0, civ);
        break;
      case 'bard':
        for (let k = 0; k < 3; k++) L.add('ring', x, Y + k * 0.18, z, 0.95 - k * 0.12, 3.5, 0.95 - k * 0.12, 0, k % 2 ? 0xe9dcc0 : 0xd8c9a8);
        L.add('house', x, Y, z - 0.45, 0.8, 0.45, 0.3, 0, civ); L.add('gable', x, Y + 0.45, z - 0.45, 0.9, 0.35, 0.35, 0, 0xb04a6a);
        break;
      case 'fighter':
        L.add('house', x, Y, z, 1.0, 0.8, 1.0, 0.3, 0x7a5f4a);
        for (let k = 0; k < 4; k++) { const a = k * Math.PI / 2 + 0.3 + Math.PI / 4; L.add('tower', x + Math.cos(a) * 0.62, Y, z + Math.sin(a) * 0.62, 0.22, 1.35, 0.22, 0, 0x6a5040); L.add('spire', x + Math.cos(a) * 0.62, Y + 1.35, z + Math.sin(a) * 0.62, 0.28, 0.5, 0.28, 0, 0xb4532a); }
        L.add('tower', x, Y + 0.8, z, 0.3, 0.9, 0.3, 0, 0x6a5040); L.add('spire', x, Y + 1.7, z, 0.4, 0.8, 0.4, 0, civ);
        break;
      case 'monk':
        for (let k = 0; k < 4; k++) { const s = 1 - k * 0.2; L.add('house', x, Y + k * 0.45, z, 0.7 * s, 0.3, 0.7 * s, 0.3, 0xd8c9a8); L.add('roof', x, Y + k * 0.45 + 0.3, z, 1.25 * s, 0.25, 1.25 * s, 0.3, 0x4f7a4a); }
        L.add('spire', x, Y + 1.85, z, 0.15, 0.6, 0.15, 0, 0xe8c24a);
        break;
      case 'sorcerer':
        L.add('box', x, Y, z, 0.36, 2.8, 0.36, 0.3, 0x5a1a2a); L.add('spire', x, Y + 2.8, z, 0.3, 0.5, 0.3, 0.3, 0x8a1f3a);
        L.add('glow', x, Y + 2.55, z, 1.3, 1.3, 1.3, 0, 0xff4a8a);
        for (let k = 0; k < 5; k++) { const a = k * 1.26; L.add('crystal', x + Math.cos(a) * 0.55, Y, z + Math.sin(a) * 0.55, 1.4, 1.6, 1.4, a, 0xb04aff); }
        break;
    }
  }
  /** zemini izleyen sur: 1 çit, 2 taş sur, 3 kale; yolların geçtiği yerde kapı boşluğu */
  private walls(tile: number, x: number, z: number, R: number, kind: number, race: string, civ = 0x8a3a2a) {
    const L = this.dynamic, w = this.sim.w;
    const gates: number[] = [];
    for (const n of this.sim.g.neighbors(tile)) if (w.tiles[n].road) { const [nx, nz] = this.pos(n); gates.push(Math.atan2(nz - z, nx - x)); }
    const nearGate = (a: number) => gates.some((g) => Math.abs(Math.atan2(Math.sin(a - g), Math.cos(a - g))) < 0.2);
    this.colWalls.push({ x, z, R, gates, t: kind === 1 ? 0.05 : kind === 3 ? 0.08 : 0.06 });
    const stone = race === 'warlock' ? 0x2e2833 : race === 'tiefling' ? 0x4a3a4f : race === 'dragonborn' ? 0x7a5f4a : race === 'elf' || race === 'halfelf' ? 0xcfc4a2 : race === 'halforc' ? 0x6b4f33 : 0xb0aa9f;
    if (kind === 1) {
      // sivri kazıklı çit: sık dizili, hafif eğri; iç yüzde iki kuşak kirişi
      const n = Math.round((Math.PI * 2 * R) / 0.115);
      for (let k = 0; k < n; k++) {
        const a = (k / n) * Math.PI * 2;
        if (nearGate(a)) continue;
        const px = x + Math.cos(a) * R, pz = z + Math.sin(a) * R, py = this.groundY(px, pz);
        if (py < WATER_Y) continue;
        L.add('stake', px, py - 0.05, pz, 1.8, 0.4 + hash(k, tile) * 0.1, 1.8, hash(k, tile + 1) * 6, jitter(0x7a5533, hash(k, tile + 2), 0.1, 0.02), (hash(k, tile + 3) - 0.5) * 0.08, (hash(k, tile + 4) - 0.5) * 0.08);
        if (k % 3 === 0 && !nearGate(a + (Math.PI * 2 * 3) / n)) {
          const a2 = a + (Math.PI * 2 * 1.5) / n, rr = R - 0.075, bx = x + Math.cos(a2) * rr, bz = z + Math.sin(a2) * rr, by = this.groundY(bx, bz);
          for (const hh of [0.12, 0.27]) L.add('box', bx, by + hh, bz, (Math.PI * 2 * R * 3) / n + 0.02, 0.022, 0.025, -a2 - Math.PI / 2, 0x5e4128);
        }
      }
      return;
    }
    const segL = 0.34, n = Math.round((Math.PI * 2 * R) / segL);
    const hgt = kind === 3 ? 0.42 : 0.3, thick = kind === 3 ? 0.12 : 0.09;
    const dark = this.tmpC.setHex(stone).multiplyScalar(0.78).getHex(), light = this.tmpC.setHex(stone).offsetHSL(0, -0.02, 0.05).getHex();
    const step = (Math.PI * 2) / n, sl = (Math.PI * 2 * R) / n + 0.02;
    for (let k = 0; k < n; k++) {
      const a = ((k + 0.5) / n) * Math.PI * 2;
      if (nearGate(a)) {
        // kapı: iki yanda konik çatılı kule, üstte kemer kirişi, açık kanatlar
        const edgeL = !nearGate(a - step), edgeR = !nearGate(a + step);
        if (edgeL || edgeR) {
          const px = x + Math.cos(a) * R, pz = z + Math.sin(a) * R, py = this.gy(px, pz);
          L.add('tower', px, py - 0.05, pz, 0.13, hgt + 0.25, 0.13, 0, jitter(stone, hash(k, tile + 5), 0.05));
          L.add('spire', px, py + hgt + 0.2, pz, 0.16, 0.26, 0.16, 0, civ);
          if (edgeL) {
            // kemer: bu kuleden kapı boşluğunun öbür ucuna
            let m = 1; while (m < n && nearGate(a + m * step)) m++;
            const ga = a + ((m - 1) * step) / 2, gx = x + Math.cos(ga) * R, gz = z + Math.sin(ga) * R, gyy = Math.max(py, this.gy(gx, gz));
            const span = 2 * R * Math.sin(((m - 1) * step) / 2) + 0.06;
            L.add('box', gx, gyy + hgt + 0.02, gz, span, 0.07, thick * 1.1, -ga - Math.PI / 2, dark);
            for (const e of [-1, 1]) { const la = ga + e * ((m - 1) * step) * 0.32, lx = x + Math.cos(la) * (R - 0.08), lz = z + Math.sin(la) * (R - 0.08); L.add('box', lx, this.gy(lx, lz), lz, span * 0.3, hgt * 0.85, 0.018, -la - Math.PI / 2 + e * 1.1, 0x6b4a2b); }
          }
        }
        continue;
      }
      const px = x + Math.cos(a) * R, pz = z + Math.sin(a) * R, py = this.groundY(px, pz);
      if (py < WATER_Y) continue;
      const rot = -a - Math.PI / 2;
      // taş sıraları: koyu temel, açık-koyu oynayan gövde, üstte yürüme yolu, dış kenarda mazgallar
      L.add('box', px, py - 0.1, pz, sl + 0.01, 0.14, thick * 1.3, rot, dark);
      L.add('box', px, py - 0.02, pz, sl, hgt + 0.02, thick, rot, jitter(stone, hash(k, tile + 7), 0.08, 0.01));
      L.add('box', px, py + hgt - 0.01, pz, sl + 0.01, 0.025, thick * 1.35, rot, light);
      if (k % 2 === 0) { const ox = Math.cos(a) * thick * 0.4, oz = Math.sin(a) * thick * 0.4; L.add('box', px + ox, py + hgt + 0.01, pz + oz, 0.09, 0.07, thick * 0.35, rot, stone); }
    }
    // taş surda dört basık gözcü kulesi
    if (kind === 2) for (let k = 0; k < 4; k++) {
      const a = (k / 4) * Math.PI * 2 + 0.78;
      if (nearGate(a)) continue;
      const px = x + Math.cos(a) * R, pz = z + Math.sin(a) * R, py = this.gy(px, pz);
      if (py < WATER_Y) continue;
      L.add('tower', px, py - 0.05, pz, 0.14, hgt + 0.28, 0.14, 0, stone);
      for (let q = 0; q < 4; q++) { const b = q * Math.PI / 2 + 0.4; L.add('box', px + Math.cos(b) * 0.055, py + hgt + 0.2, pz + Math.sin(b) * 0.055, 0.04, 0.05, 0.04, b, stone); }
      if (hash(k, tile + 9) > 0.4) { L.add('spire', px, py + hgt + 0.25, pz, 0.17, 0.24, 0.17, 0, civ); L.add('flag', px, py + hgt + 0.58, pz, 0.8, 0.8, 1, a, civ); L.add('post', px, py + hgt + 0.45, pz, 0.5, 0.2, 0.5, 0, 0x3a2a1a); }
    }
    if (kind === 3) for (let k = 0; k < 6; k++) {
      const a = (k / 6) * Math.PI * 2 + 0.3;
      if (nearGate(a)) continue;
      const px = x + Math.cos(a) * R, pz = z + Math.sin(a) * R, py = this.gy(px, pz);
      L.add('tower', px, py - 0.05, pz, 0.17, 0.75, 0.17, 0, stone);
      L.add('spire', px, py + 0.7, pz, 0.22, 0.32, 0.22, 0, civ);
      // kale kulesinde mazgal pencereler
      for (let q = 0; q < 3; q++) { const b = a + (q - 1) * 0.9, wx = px + Math.cos(b) * 0.085, wz = pz + Math.sin(b) * 0.085; L.add('winoff', wx, py + 0.45, wz, 0.5, 1.4, 1, -b + Math.PI / 2, 0x1e1c1a); }
    }
  }
  nearestTile(x: number, z: number): number {
    const w = this.sim.w;
    const r0 = Math.round(z / 1.5);
    let best = -1, bd = Infinity;
    for (let r = r0 - 1; r <= r0 + 1; r++) {
      if (r < 0 || r >= w.H) continue;
      const c0 = Math.round(x / SQ3 - 0.5 * (r & 1));
      for (let c = c0 - 1; c <= c0 + 1; c++) {
        if (c < 0 || c >= w.W) continue;
        const i = r * w.W + c;
        const [px, pz] = this.pos(i);
        const d = (px - x) ** 2 + (pz - z) ** 2;
        if (d < bd) { bd = d; best = i; }
      }
    }
    return bd < 1.2 ? best : -1;
  }

  // ---------------------------------------------------------------- figürler
  private detail = true;
  private add: (key: string, x: number, y: number, z: number, sx?: number, sy?: number, sz?: number, ry?: number, color?: number, rx?: number, rz?: number) => void = (...a) => this.actors.add(...a);
  private gy(x: number, z: number) { return Math.max(this.groundY(x, z), WATER_Y); }
  private figK = 1;
  /** çizilen evin yanabilecek pencere sayısı (0 = boş ev) */
  private hLit = 9;
  private fig(x: number, z: number, face: number, t: number, id: number, look: Look, pose: Pose, scale = 1) {
    drawFigure(this.add, x, this.gy(x, z), z, face, t, id, look, pose, scale * this.figK, this.detail);
  }
  private figAt(x: number, y: number, z: number, face: number, t: number, id: number, look: Look, pose: Pose, scale = 1) {
    drawFigure(this.add, x, y, z, face, t, id, look, pose, scale * this.figK, this.detail);
  }
  /** deniz yüzeyi (atmosferdeki dalga ile aynı formül) */
  private waveY(x: number, z: number, t: number) {
    return WATER_Y + Math.sin(x * 0.55 + t * 1.1) * 0.045 + Math.cos(z * 0.63 - t * 0.9) * 0.045 + Math.sin((x + z) * 1.3 + t * 1.7) * 0.018;
  }
  /**
   * Gemi: kind = boat (tekne), cog (koga), galley (kadırga), scout (keşif teknesi).
   * load: yük/yolcu (crates | people | troops | none); furled: yelken sarılı (limanda, demirde).
   */
  private ship(x: number, z: number, face: number, t: number, id: number, kind: 'boat' | 'cog' | 'galley' | 'scout' | 'pirate', civCol: number, race: string, o: { load?: 'crates' | 'people' | 'troops' | 'none'; furled?: boolean; moving?: boolean; sink?: number; cargo?: number; wreck?: boolean } = {}) {
    const L = this.actors;
    const K = kind === 'pirate' ? { len: 1.0, beam: 0.3, h: 0.16, mast: 0.9, sw: 0.5, sh: 0.5 } : kind === 'galley' ? { len: 1.35, beam: 0.3, h: 0.15, mast: 0.78, sw: 0.5, sh: 0.46 } : kind === 'cog' ? { len: 1.1, beam: 0.36, h: 0.2, mast: 0.92, sw: 0.56, sh: 0.5 } : kind === 'scout' ? { len: 0.8, beam: 0.24, h: 0.13, mast: 0.8, sw: 0, sh: 0.62 } : { len: 0.62, beam: 0.22, h: 0.11, mast: 0.5, sw: 0.32, sh: 0.3 };
    const fx = Math.sin(face), fz = Math.cos(face), sx = Math.cos(face), sz = -Math.sin(face);
    const sink = o.sink ?? 0;
    const wy = this.waveY(x, z, t);
    const pitch = (this.waveY(x - fx * 0.4, z - fz * 0.4, t) - this.waveY(x + fx * 0.4, z + fz * 0.4, t)) * 1.1 + sink * 0.5;
    const roll = Math.sin(t * 1.15 + id * 1.7) * (o.moving ? 0.05 : 0.03) + sink * 0.35;
    const y = wy - K.h * 0.45 - sink * 0.5;
    const wood = kind === 'galley' ? 0x4a2e1c : kind === 'pirate' ? 0x2e2420 : 0x6b4a2b;
    L.add('keel', x, y, z, K.beam, K.h, K.len, face, wood, pitch, roll);
    // küpeşte şeridi: medeniyet rengi
    L.add('box', x, y + K.h * 0.82, z, K.beam * 0.94, K.h * 0.12, K.len * 0.62, face, civCol, pitch, roll);
    const deck = y + K.h * 0.95;
    const at = (along: number, side = 0, up = 0): [number, number, number] => [x + fx * along + sx * side, deck + up - along * pitch, z + fz * along + sz * side];
    // kıç kasarası (koga) ya da dümen kulübesi
    if (kind === 'cog' || kind === 'pirate') { const [px, py, pz] = at(-K.len * 0.34); L.add('box', px, py, pz, K.beam * 0.8, 0.1, K.len * 0.2, face, 0x7a5636, pitch, roll); L.add('box', px, py + 0.1, pz, K.beam * 0.86, 0.02, K.len * 0.22, face, 0x5a3a22, pitch, roll); }
    if (kind === 'galley') {
      // kürekler: iki yanda sıra, çekişle salınır
      const n = 6, sweep = o.moving ? Math.sin(t * 3.2 + id) * 0.35 : 0.1;
      for (let k = 0; k < n; k++) for (const e of [-1, 1]) {
        const along = (k / (n - 1) - 0.5) * K.len * 0.62;
        const [px, py, pz] = at(along, e * K.beam * 0.5, -0.02);
        L.add('box', px + sx * e * 0.14, py - 0.03, pz + sz * e * 0.14, 0.34, 0.012, 0.018, face + e * sweep, 0x8a6a45, 0, -e * 0.3);
      }
      const [bx, by, bz] = at(K.len * 0.47, 0, -0.02); L.add('box', bx, by, bz, 0.03, 0.03, 0.18, face, 0x8d877e, pitch - 0.2, roll); // mahmuz
    }
    if (sink > 0.9) return;
    // direk, seren, yelken
    const [mx, my, mz] = at(kind === 'scout' ? -0.05 : 0.04);
    L.add('post', mx, my - 0.02, mz, 1.3, K.mast, 1.3, face, 0x5a4330, pitch, roll);
    const sailCol = kind === 'pirate' ? 0x1e1e22 : kind === 'galley' ? civCol : _jc.setHex(0xefe6d2).lerp(new THREE.Color(civCol), 0.22).getHex();
    if (kind === 'scout') {
      if (!o.furled) L.add('lateen', mx, my + 0.02, mz, 1, K.sh, K.len * 0.7, face + 0.25 * Math.sin(t * 0.3 + id), sailCol, pitch, roll);
    } else if (o.furled) {
      L.add('log', mx, my + K.mast * 0.78, mz, K.sw / 0.4, 0.5, 0.5, face, 0xe2d6bc, pitch, roll);
    } else {
      L.add('log', mx, my + K.mast * 0.86, mz, K.sw / 0.4, 0.35, 0.35, face, 0x5a4330, pitch, roll);
      L.add('sail', mx + fx * 0.03, my + K.mast * 0.86 - K.sh, mz + fz * 0.03, K.sw, K.sh, 1, face, sailCol, pitch, roll);
    }
    L.add('flag', mx, my + K.mast - 0.04, mz, 0.8, 0.8, 1, face + Math.PI / 2 + Math.sin(t * 4 + id) * 0.25, civCol);
    if (kind === 'pirate') {
      L.add('dot', mx, my + K.mast + 0.02, mz, 0.6, 0.6, 0.6, 0, 0xe8e0c8); // kafatası
      if (this.detail || o.load === 'troops') for (let k = 0; k < 3; k++) { const [px, py, pz] = at(K.len * (0.25 - k * 0.2), ((k % 2) - 0.5) * K.beam * 0.5); this.figAt(px, py, pz, face + (k - 1) * 0.6, t, id * 7 + k, this.monsterLook('pirate', k), o.moving ? 'idle' : 'cheer', 0.62); }
      return;
    }
    // yük
    const load = o.load ?? 'none';
    if (load === 'crates') for (let k = 0; k < 3; k++) { const [px, py, pz] = at(K.len * (0.2 - k * 0.12), ((k % 2) - 0.5) * 0.08); L.add('box', px, py, pz, 0.1, 0.08, 0.1, face + k, o.cargo ?? 0x9a6a3a, pitch, roll); }
    if (load === 'people' || load === 'troops') {
      const n = kind === 'boat' ? 2 : 4;
      for (let k = 0; k < n; k++) {
        const [px, py, pz] = at(K.len * (0.28 - k * 0.14), ((k % 2) - 0.5) * K.beam * 0.45);
        const look = load === 'troops' ? this.raceLook(race, 0x5a5f66, { hat: 'helm', tool: 'spear', shield: civCol }) : this.raceLook(race, TUNIC_S[k % TUNIC_S.length], k % 2 ? { sack: 0xc8b48a } : {});
        this.figAt(px, py, pz, face + (k % 2 ? 0.4 : -0.3), t, id * 7 + k, look, 'idle', 0.62);
      }
    } else if (this.detail && kind !== 'galley') {
      // dümenci
      const [px, py, pz] = at(-K.len * 0.4);
      this.figAt(px, py + (kind === 'cog' ? 0.1 : 0), pz, face, t, id * 7 + 9, this.raceLook(race, 0x4a5a6a, { hat: 'hood', hatColor: civCol }), 'idle', 0.6);
    }
    // dümen suyu
    if (o.moving) for (const e of [-1, 1]) {
      const bx = x - fx * K.len * 0.55, bz = z - fz * K.len * 0.55;
      L.add('foam', bx - fx * 0.3 + sx * e * 0.1, WATER_Y + 0.05, bz - fz * 0.3 + sz * e * 0.1, 0.62, 1, 1.4, face + Math.PI / 2 + e * 0.22, 0xffffff);
    }
  }
  private raceLook(race: string, body: number, extra: Partial<Look> = {}): Look {
    const hat: Look['hat'] = race === 'elf' || race === 'halfelf' ? 'ears' : race === 'tiefling' || race === 'dragonborn' ? 'horns' : 'none';
    return { race, body, skin: SKIN[race] ?? 0xe8b894, hat, ...extra };
  }
  private heroLook(h: Hero, civCol: number): Look {
    const L = (body: number, x: Partial<Look>): Look => ({ ...this.raceLook(h.race, body), ...x });
    switch (h.cls) {
      case 'fighter': return L(0x7a7f88, { hat: 'helm', tool: 'sword', shield: civCol, cape: civCol });
      case 'wizard': return L(0x4b3a8a, { hat: 'wizard', hatColor: 0x3b2f7a, tool: 'staff', orb: 0x8fd8ff });
      case 'cleric': return L(0xe8e0c8, { hat: 'hood', hatColor: 0xf2ecd8, tool: 'mace', shield: 0xf2e6b0 });
      case 'rogue': return L(0x2e2a2a, { hat: 'hood', hatColor: 0x1e1c1c, tool: 'dagger', cape: 0x1e1c1c });
      case 'ranger': return L(0x3f6b3a, { hat: 'hood', hatColor: 0x345a30, tool: 'spear', cape: 0x4a6a3a });
      case 'paladin': return L(0xd9d2b8, { hat: 'goldhelm', tool: 'sword', shield: 0xf0e2a8, cape: 0xb5871a });
      case 'druid': return L(0x5f8a3a, { hat: 'leaf', tool: 'staff', orb: 0x9fe6a0 });
      case 'barbarian': return L(0x8a4a2a, { hat: 'horns', tool: 'axe', legs: 0x5a3a22 });
    }
    return L(0xcccccc, {});
  }
  private monsterLook(kind: CampKind, k: number): Look {
    if (kind === 'goblin') return { race: 'goblin', body: k % 2 ? 0x6b5a3a : 0x8a3a2a, skin: 0x8fb04f, hat: 'ears', tool: k % 3 === 0 ? 'spear' : 'dagger', legs: 0x3a2e22 };
    if (kind === 'hobgoblin') return { race: 'hobgoblin', body: 0x6a2418, skin: 0xc0673e, hat: 'helm', tool: 'sword', shield: 0x3a1a14, legs: 0x2a1a14 };
    if (kind === 'pirate') return { race: k % 3 === 2 ? 'halforc' : 'human', body: [0x2a2a30, 0x6a1a1a, 0xd8d0c0][k % 3], skin: k % 3 === 2 ? 0x8fa66b : 0xd9a47e, hat: 'hood', hatColor: [0xa0281e, 0x1e1e22, 0x2a4a7a][k % 3], tool: k % 2 ? 'sword' : 'dagger', legs: 0x3a2e22 };
    return { race: 'bugbear', body: 0x6a4a2a, skin: 0x9a6a3e, hat: 'ears', tool: 'club', legs: 0x4a3322 };
  }
  /** han yolcusu: tüccar mor cüppe ve heybe, hacı ak başlık, ozan renkli pelerin, asker miğfer... */
  private guestLook(kind: GuestKind, race: string, cc: number, k = 0): Look {
    switch (kind) {
      case 'merchant': return this.raceLook(race, 0x8a5a9a, { hat: 'hood', hatColor: 0x6a3a7a, sack: 0xc8a060 });
      case 'pilgrim': return this.raceLook(race, 0xece6d6, { hat: 'hood', hatColor: 0xf2ecdc, tool: 'staff', orb: 0x6b4a2b });
      case 'bard': return this.raceLook(race, 0xc04a6a, { hat: 'hood', hatColor: 0xe8c24a, cape: 0x3a6ab0, tool: 'staff', orb: 0xe8c24a });
      case 'hunter': return this.raceLook(race, 0x4a6a3a, { hat: 'hood', hatColor: 0x3a5a2a, tool: 'spear' });
      case 'scholar': return this.raceLook(race, 0x3a4a7a, { hat: 'hood', hatColor: 0x2a3a6a, tool: 'staff', orb: 0x8fd8ff, sack: 0x7a5533 });
      case 'soldier': return this.raceLook(race, 0x5a5f66, { hat: 'helm', tool: 'spear', shield: cc, legs: 0x3a3a3a });
      case 'refugee': return this.raceLook(race, [0x8a7a6a, 0x7a6a5a, 0x9a8a6a][k % 3], { sack: 0xb8a078 });
      case 'noble': return k === 0 ? this.raceLook(race, cc, { cape: 0x7a1a2a, legs: 0x2a2a3a }) : this.raceLook(race, 0x5a5f66, { hat: 'helm', tool: 'spear', shield: cc, legs: 0x3a3a3a });
      case 'wanderer': return this.raceLook(race, 0x7a6a5a, { hat: 'hood', hatColor: 0x5a4a3a, tool: 'staff', orb: 0x6b4a2b });
      default: return this.raceLook(race, 0x8a8a7a);
    }
  }
  /** seçili han (sağ panelde açık): etrafında altın halka */
  selInn: number | null = null;
  /** ekrandaki hana tıklandı mı: yapının kapladığı alan kadar yarıçap */
  pickInn(clientX: number, clientY: number): number | null {
    const r = this.renderer.domElement.getBoundingClientRect();
    const mx = clientX - r.left, my = clientY - r.top;
    let best: number | null = null, bd = Infinity;
    for (const inn of this.sim.w.inns) {
      if (inn.stage === 'road') continue;
      const [x, z] = this.pos(inn.tile), y = this.height(inn.tile);
      const [sx, sy, ok] = this.project(x, y + 0.3, z);
      if (!ok) continue;
      const [ex, ey] = this.project(x + 0.75, y + 0.3, z);
      const R = Math.max(16, Math.min(160, Math.hypot(ex - sx, ey - sy)));
      const d = Math.hypot(sx - mx, sy - my);
      if (d < R && d < bd) { bd = d; best = inn.id; }
    }
    return best;
  }
  private heroFig(x: number, z: number, t: number, hid: number, face: number, pose: Pose = 'walk') {
    const h = this.sim.w.heroes.find((q) => q.id === hid);
    if (!h || h.state === 'dead') return;
    const cc = h.civ >= 0 && this.sim.w.civs[h.civ] ? new THREE.Color(this.sim.w.civs[h.civ].color).getHex() : 0x8a8f99;
    this.fig(x, z, face, t, hid, this.heroLook(h, cc), h.cls === 'wizard' && pose === 'fight' ? 'cast' : pose, 1.2);
    this.heroSpots.set(hid, [x, this.gy(x, z), z]);
    this.actors.add('halo', x, this.gy(x, z) + 0.02, z, 1.1, 1, 1.1, 0, 0xf2cf5b);
  }

  private agentWorld(a: Agent): [number, number, number, number] {
    if (a.restUntil !== undefined && a.inn !== undefined) {
      // handa konaklayan kervan: arabası avlunun kenarında
      const inn = this.sim.w.inns.find((q) => q.id === a.inn);
      if (inn) {
        const [ix, iz] = this.pos(inn.tile), ia = hash(inn.id, 7) * Math.PI * 2;
        const f = 0.15 - (a.id % 3) * 0.28, r = -0.78;
        const x = ix + Math.sin(ia) * f + Math.cos(ia) * r, z = iz + Math.cos(ia) * f - Math.sin(ia) * r;
        return [x, Math.max(this.groundY(x, z), WATER_Y), z, ia + Math.PI / 2];
      }
    }
    const i = Math.min(a.step, a.path.length - 1), j = Math.min(i + 1, a.path.length - 1);
    const [x1, z1] = this.pos(a.path[i]), [x2, z2] = this.pos(a.path[j]);
    const p = Math.min(1, a.progress);
    const x = x1 + (x2 - x1) * p, z = z1 + (z2 - z1) * p;
    return [x, Math.max(this.groundY(x, z), WATER_Y), z, Math.atan2(x2 - x1, z2 - z1)];
  }

  private buildWalks() {
    const s = this.sim, w = s.w;
    this.walks = [];
    const setAt = new Map<number, number>();
    for (const st of w.settlements) if (st.alive) setAt.set(st.tile, st.id);
    const isRoad = (q: number) => w.tiles[q].road || setAt.has(q);
    for (const st of w.settlements) {
      if (!st.alive) continue;
      const P = s.pop(st);
      const n = Math.min(5, Math.floor(P / 20) + (st.tier >= 1 ? 1 : 0));
      for (let k = 0; k < n; k++) {
        const path = [st.tile], seen = new Set(path);
        let cur = st.tile;
        for (let step = 0; step < 12; step++) {
          const nb = s.g.neighbors(cur).filter((q) => isRoad(q) && !seen.has(q));
          if (!nb.length) break;
          cur = nb[Math.floor(hash(st.id * 31 + k, step + 200) * nb.length)];
          path.push(cur); seen.add(cur);
          if (setAt.has(cur) && step >= 1) break;
        }
        if (path.length < 3) continue;
        const pts = path.map((q) => this.pos(q));
        const cum = [0];
        for (let j = 1; j < pts.length; j++) cum.push(cum[j - 1] + Math.hypot(pts[j][0] - pts[j - 1][0], pts[j][1] - pts[j - 1][1]));
        const race = Object.entries(st.pop).filter(([, v]) => (v ?? 0) > 0).map(([r]) => r)[k % 2] ?? w.civs[st.civ].race;
        this.walks.push({ pts, cum, len: cum[cum.length - 1], race, civ: st.civ, kind: Math.floor(hash(st.id, k + 300) * 4), id: st.id * 50 + k });
      }
    }
  }

  /** kasabadaki işler: sim'deki iş dağılımı (tarla/maden işçileri kendi karolarında ayrıca çizilir) */
  rolesOf(st: Settlement): string[] {
    const roles: string[] = [];
    for (const [k, v] of Object.entries(st.jobs ?? {})) {
      if (!v || k === 'asker' || (GOODS as Record<string, unknown>)[k]) continue;
      for (let q = 0; q < v; q++) roles.push(k);
    }
    return roles;
  }

  /** bir kasabalının şu anki hâli ve günlük planı: ev → iş → (öğle molası) → iş → ev/meyhane → uyku */
  citState(st: Settlement, lay: TownLayout, roles: string[], i: number, t: number) {
    const c = this.sim.w.civs[st.civ];
    const cc = new THREE.Color(c.color).getHex();
    const TUNIC = [0x9a8a6a, 0x7a6a4a, 0x8a5a3a, 0x6a7a5a, 0xa89a7a, 0x5a6a7a];
    const { x, z } = lay;
    const center: [number, number] = [x, z];
    const at = (key: string, k: number): [number, number] => { const l = lay.work[key]; return l?.length ? l[k % l.length] : center; };
    const role = roles.length ? roles[(i * 7919 + st.id * 13) % roles.length] : 'halk';
    const id = st.id * 97 + i;
    const races = Object.entries(st.pop).filter(([, v]) => (v ?? 0) > 0).map(([k]) => k);
    const race = races[i % Math.max(1, races.length)] ?? c.race;
    const names = HERO_NAMES[race as keyof typeof HERO_NAMES] ?? HERO_NAMES.human;
    const name = names[Math.floor(hash(id, 31) * names.length)];
    const oh = lay.work.occHomes;
    const home = oh?.length ? oh[(i * 13 + 5) % oh.length] : lay.homes[i % lay.homes.length];
    const inShanty = !!lay.work.shanty?.some((q) => q[0] === home[0] && q[1] === home[1]);
    const wk = WS_BY_NAME[role];
    let work: [number, number] = center, tool: Tool = 'none', pose: Pose = 'idle', hat: Look['hat'] | undefined, errand = false;
    let place = 'kasaba', job = role, verb = 'çalışır';
    if (wk) { work = at(wk, i); [tool, pose] = WS_TOOL[wk] ?? ['none', 'idle']; place = role; job = `${role.toLocaleLowerCase('tr')} işçisi`; verb = 'çalışır'; }
    else if (role === 'inşaatçı') {
      const p = st.project;
      if (p && (p.type === 'extract' || p.type === 'upgrade') && p.tile !== undefined) { work = this.pos(p.tile); place = 'kasaba dışındaki şantiye'; }
      else { work = at('site', i); place = 'şantiye'; }
      tool = i % 3 === 0 ? 'none' : 'mace'; pose = i % 3 === 0 ? 'carry' : 'work'; verb = i % 3 === 0 ? 'malzeme taşır' : 'duvar örer';
    } else if (role === 'araştırmacı') { work = lay.work.library?.[0] ?? lay.work.scriptorium?.[0] ?? center; pose = i % 4 === 0 ? 'cast' : 'idle'; hat = 'hood'; tool = i % 4 === 0 ? 'staff' : 'none'; place = lay.work.library ? 'kütüphane' : 'meclis'; verb = 'kitap okur, deney yapar'; }
    else if (role === 'toplayıcı') { const a = hash(id, 3) * 6.28, r = lay.R * 1.9 + 0.8 + hash(id, 4) * 1.2; work = [x + Math.cos(a) * r, z + Math.sin(a) * r]; tool = 'basket'; pose = 'work'; hat = 'straw'; place = 'kasaba dışı'; verb = 'meyve, ot toplar'; }
    else { errand = true; work = lay.work.market?.[0] ?? lay.homes[(i * 7 + 3) % lay.homes.length]; place = lay.work.market ? 'pazar' : 'sokaklar'; verb = 'alışveriş ve ayak işi'; }
    const ox = (hash(id, 5) - 0.5) * 0.36, oz = (hash(id, 6) - 0.5) * 0.36;
    const wx = work[0] + ox, wz = work[1] + oz;
    const plaza = lay.work.plaza?.[0] ?? center;
    const lx = plaza[0] + Math.cos(i * 2.4) * 0.2, lz = plaza[1] + Math.sin(i * 2.4) * 0.2;
    const luncher = i % 3 === 0 && role !== 'toplayıcı' && Math.hypot(wx - x, wz - z) < lay.R * 2 + 1;
    const tavernGoer = !!st.civics.tavern && i % 5 === 0;
    const tv = at('tavern', 0);
    const tx = tv[0] + ox * 0.8, tz = tv[1] + oz * 0.8;
    const ph = ((this.cyc - hash(id, 7) * 0.035) % 1 + 1) % 1;
    let px = home[0], pz = home[1], face = 0, p2: Pose = pose, visible = true, step = 0;
    const walk = (ax: number, az: number, bx: number, bz: number, q: number) => { q = Math.max(0, Math.min(1, q)); px = ax + (bx - ax) * q; pz = az + (bz - az) * q; face = Math.atan2(bx - ax, bz - az); p2 = 'walk'; };
    const stay = (sx: number, sz: number, f: number, pp: Pose) => { px = sx; pz = sz; face = f; p2 = pp; };
    const atWork = () => {
      if (errand) {
        const loop = ((t * 0.05 + hash(id, 8)) % 1);
        const other = lay.homes[(i * 5 + 1) % lay.homes.length];
        if (loop < 0.35) walk(wx, wz, other[0], other[1], loop / 0.35);
        else if (loop < 0.5) stay(other[0], other[1], hash(id, 9) * 6.28, 'idle');
        else if (loop < 0.85) walk(other[0], other[1], wx, wz, (loop - 0.5) / 0.35);
        else stay(wx, wz, Math.atan2(work[0] - wx, work[1] - wz), Math.sin(t + i) > 0.5 ? 'cheer' : 'idle');
        if (p2 === 'walk' && i % 2 === 0) p2 = 'carry';
      } else stay(wx, wz, Math.atan2(work[0] - wx, work[1] - wz) + (role === 'toplayıcı' ? hash(id, 10) * 3 : 0), pose);
    };
    if (ph < 0.03) { visible = false; step = 5; }
    else if (ph < 0.09) { walk(home[0], home[1], wx, wz, (ph - 0.03) / 0.06); step = 0; }
    else if (ph < 0.24 || (!luncher && ph < 0.42)) { atWork(); step = ph < 0.24 ? 1 : 3; }
    else if (luncher && ph < 0.27) { walk(wx, wz, lx, lz, (ph - 0.24) / 0.03); step = 2; }
    else if (luncher && ph < 0.31) { stay(lx, lz, Math.atan2(plaza[0] - lx, plaza[1] - lz), Math.sin(t * 1.5 + i) > 0.4 ? 'cheer' : 'idle'); step = 2; }
    else if (luncher && ph < 0.34) { walk(lx, lz, wx, wz, (ph - 0.31) / 0.03); step = 2; }
    else if (ph < 0.42) { atWork(); step = 3; }
    else if (ph < 0.48) { if (tavernGoer) walk(wx, wz, tx, tz, (ph - 0.42) / 0.06); else walk(wx, wz, home[0], home[1], (ph - 0.42) / 0.06); step = 4; }
    else if (tavernGoer && ph < 0.72) { stay(tx, tz, Math.atan2(tv[0] - tx, tv[1] - tz), Math.sin(t * 2 + i) > 0.3 ? 'cheer' : 'idle'); tool = 'none'; step = 4; }
    else if (tavernGoer && ph < 0.77) { walk(tx, tz, home[0], home[1], (ph - 0.72) / 0.05); step = 5; }
    else { visible = false; step = 5; }
    const body = i % 4 === 0 ? cc : TUNIC[(i + st.id) % TUNIC.length];
    const look = this.raceLook(race, body, { tool: p2 === 'walk' || p2 === 'carry' ? (tool === 'basket' ? 'basket' : 'none') : tool, sack: p2 === 'carry' ? 0xc8b48a : undefined });
    if (hat) look.hat = hat;
    const fest = this.festival(st);
    const night = ph > 0.48 || ph < 0.03;
    const mood = !visible ? 'evde uyuyor' : st.plague && hash(id, 12) < 0.3 ? 'hasta' : st.starving > 0 ? 'aç' : step === 4 && tavernGoer && p2 !== 'walk' ? 'meyhanede' : step === 2 && p2 !== 'walk' ? 'öğle molasında' : fest ? 'bayramda' : p2 === 'walk' ? (step === 0 ? 'işe gidiyor' : step >= 4 ? (tavernGoer && step === 4 ? 'meyhaneye gidiyor' : 'eve dönüyor') : 'yolda') : p2 === 'carry' ? 'yük taşıyor' : 'çalışıyor';
    const icon = !visible ? '💤' : mood === 'hasta' ? '🤒' : mood === 'aç' ? '🍞' : mood === 'meyhanede' ? '🍺' : mood === 'öğle molasında' ? '💬' : fest ? '🎵' : role === 'inşaatçı' && p2 === 'work' ? '🔨' : role === 'araştırmacı' && p2 !== 'walk' ? '📜' : '';
    const plan: [string, string][] = [
      ['Şafak', 'evden çıkar, işe yürür'],
      ['Sabah', `${place}: ${verb}`],
      ['Öğle', luncher ? 'meydanda mola, sohbet' : 'işinin başında atıştırır'],
      ['Öğleden sonra', `${place}: ${verb}`],
      ['Akşam', tavernGoer ? 'meyhaneye uğrar' : 'eve döner'],
      ['Gece', inShanty ? 'sur dışındaki barakada uyur' : tavernGoer ? 'geç saatte eve döner, uyur' : 'evde uyur'],
    ];
    if (inShanty) plan[4] = ['Akşam', tavernGoer ? 'meyhaneye uğrar, sonra barakaya' : 'sur dışındaki barakaya döner'];
    const mood2 = inShanty && !visible ? 'barakada uyuyor' : inShanty && step >= 4 && p2 === 'walk' && !(tavernGoer && step === 4) ? 'barakaya dönüyor' : mood;
    const hk = lay.homes.indexOf(home);
    const family = hk >= 0 && lay.houses[hk] ? DIO_FAMILY[Math.floor(hash(lay.houses[hk].seed, 3) * DIO_FAMILY.length)] : null;
    return { family, x: px, z: pz, face, pose: p2, look, visible, id, role, job, race, name, mood: mood2, icon, step, plan, home, age: 16 + Math.floor(hash(id, 33) * 48), night, inShanty };
  }

  private citizen(x: number, z: number, id: number, race: string, role: string, stId: number, mood: string, icon: string, closeUp: boolean, idx = -1) {
    const names = HERO_NAMES[race as keyof typeof HERO_NAMES] ?? HERO_NAMES.human;
    const y = this.gy(x, z);
    this.cits.push({ x, y, z, name: names[Math.floor(hash(id, 31) * names.length)], role, st: stId, mood, race, idx });
    if (icon && closeUp && hash(id, 32) < 0.35 && this.bubbles.filter((b) => b.icon === icon).length < 3) this.bubbles.push({ x, y: y + 0.55, z, icon, id });
  }

  /** kasaba hayatı: çocuklar, kuyu başı, pazar, hayvanlar ve duruma göre sahneler */
  private townLife(st: Settlement, lay: TownLayout, race: string, cc: number, dCam: number, t: number, atWar: boolean, closeUp: boolean) {
    if (dCam > 34) return;
    const L = this.actors;
    const day = this.night < 0.5;
    const P = this.sim.pop(st);
    const plaza = lay.work.plaza?.[0] ?? [lay.x + 0.3, lay.z + 0.3];
    const [px, pz] = plaza;
    const TUN = [0xb04a3a, 0x4a7ab0, 0xd8b84a, 0x6aa04a, 0xa86ab0, 0xe8dcc0];
    const fest = this.festival(st);
    const sid = st.id * 1000;
    const near = dCam < 20;
    const look = (k: number, extra: Partial<Look> = {}) => this.raceLook(race, TUN[k % TUN.length], extra);
    // bayram: meydanda halka dansı, ozan çalar; gece ateş başında
    if (fest) {
      const n = Math.min(near ? 14 : 7, 4 + Math.floor(P / 10));
      for (let k = 0; k < n; k++) {
        const a = (k / n) * Math.PI * 2 + t * 0.6;
        const r = 0.24 + (k % 2) * 0.05;
        const x = px + Math.cos(a) * r, z = pz + Math.sin(a) * r;
        this.fig(x, z, -a, t, sid + k, look(k), Math.sin(t * 4 + k) > 0 ? 'cheer' : 'walk', 0.8);
        if (k % 3 === 0) this.citizen(x, z, sid + k, race, 'halk', st.id, 'bayramda', '🎵', closeUp);
      }
      this.fig(px, pz, t * 0.3, t, sid + 99, look(2, { hat: 'hood', hatColor: 0x8a2a4a, tool: 'staff', orb: 0xd8b84a }), 'cast', 0.85);
      this.citizen(px, pz, sid + 99, race, 'ozan', st.id, 'çalgı çalıyor', '🎶', closeUp);
      const bf = lay.work.bonfire?.[0];
      if (bf && !day) {
        const by = this.gy(bf[0], bf[1]), fl = 0.8 + Math.sin(t * 11) * 0.2;
        for (let q = 0; q < 3; q++) L.add('fire', bf[0] + Math.cos(q * 2.1) * 0.05, by, bf[1] + Math.sin(q * 2.1) * 0.05, 0.9 * fl, 1.6 * fl, 0.9 * fl, t * (q % 2 ? 1 : -1), 0xffffff);
        L.add('lantern', bf[0], by + 0.1, bf[1], 1.4 * fl, 1.4 * fl, 1.4 * fl, t, 0xffb03a);
      }
    }
    // baraka halkı: ateş başında, yoksul giysiler
    const sh = lay.work.shanty, sf = lay.work.shantyFire?.[0];
    if (sh && sf) {
      if (!day) {
        const by = this.gy(sf[0], sf[1]), fl = 0.8 + Math.sin(t * 10 + st.id) * 0.2;
        for (let q = 0; q < 2; q++) L.add('fire', sf[0] + (q - 0.5) * 0.04, by, sf[1], 0.8 * fl, 1.3 * fl, 0.8 * fl, t * (q ? 1 : -1), 0xffffff);
        L.add('lantern', sf[0], by + 0.08, sf[1], 1.1 * fl, 1.1 * fl, 1.1 * fl, t, 0xffb03a);
      }
      const n = Math.min(near ? 8 : 3, lay.homeless);
      for (let k = 0; k < n; k++) {
        const atFire = !day || k % 2 === 0;
        const a = k * 1.9 + st.id;
        const x = atFire ? sf[0] + Math.cos(a) * 0.22 : sh[k % sh.length][0] + Math.cos(a) * 0.15, z = atFire ? sf[1] + Math.sin(a) * 0.22 : sh[k % sh.length][1] + Math.sin(a) * 0.15;
        const f = atFire ? Math.atan2(sf[0] - x, sf[1] - z) : a;
        this.fig(x, z, f, t, sid + 800 + k, this.raceLook(race, [0x6a6258, 0x5a5048, 0x7a6a58][k % 3], { hat: k % 3 === 0 ? 'hood' : 'none', hatColor: 0x4a4238 }), atFire ? 'idle' : k % 3 === 1 ? 'carry' : 'work', 0.8);
        if (k === 0) this.citizen(x, z, sid + 800, race, 'evsiz', st.id, 'sur dışındaki barakada yaşıyor', '🏚', closeUp);
      }
    }
    if (!day) return;
    // kuyu başı: biri su çeker, biri kovasıyla bekler
    if (!fest) {
      const q = (t * 0.07 + st.id * 0.3) % 1;
      this.fig(px + 0.14, pz + 0.02, Math.PI / 2 + Math.PI, t, sid + 50, look(3, { hat: 'hood', hatColor: 0xe8dcc0 }), 'work', 0.8);
      const wx = px + 0.1 + q * 0.5, wz = pz - 0.2;
      this.fig(q < 0.5 ? px - 0.05 : wx, q < 0.5 ? pz - 0.18 : wz, q < 0.5 ? 0 : Math.PI / 2, t, sid + 51, look(5, { tool: 'basket' }), q < 0.5 ? 'idle' : 'carry', 0.8);
      this.citizen(px + 0.14, pz + 0.02, sid + 50, race, 'halk', st.id, 'kuyudan su çekiyor', '💧', closeUp);
    }
    // çocuklar: meydanda kovalamaca
    const kids = Math.min(near ? 8 : 4, Math.floor(P / 14)) * (st.plague ? 0 : 1);
    for (let k = 0; k < kids; k++) {
      const chase = k < 4;
      const a = chase ? t * 1.3 + k * 0.5 : t * 0.9 + k * 2.1;
      const r = chase ? 0.3 + Math.sin(t * 0.7 + k) * 0.06 : 0.55 + (k % 3) * 0.12;
      const cx = chase ? px : lay.x, cz = chase ? pz : lay.z;
      const x = cx + Math.cos(a) * r, z = cz + Math.sin(a) * r;
      if (this.groundY(x, z) < WATER_Y) continue;
      this.fig(x, z, -a, t * 1.6, sid + 200 + k, look(k + 1), 'walk', 0.5);
      if (k === 0) this.citizen(x, z, sid + 200 + k, race, 'çocuk', st.id, 'oyun oynuyor', '😄', closeUp);
    }
    // köpek: çocukların peşinde
    if (kids > 0 && P > 20) {
      const a = t * 1.3 - 0.35, x = px + Math.cos(a) * 0.32, z = pz + Math.sin(a) * 0.32, y = this.gy(x, z);
      const f = -a, b = Math.abs(Math.sin(t * 12)) * 0.015;
      L.add('box', x, y + 0.05 + b, z, 0.05, 0.05, 0.13, f, 0x8a6a45);
      L.add('box', x + Math.sin(f) * 0.08, y + 0.08 + b, z + Math.cos(f) * 0.08, 0.05, 0.05, 0.06, f, 0x7a5a35);
      L.add('box', x - Math.sin(f) * 0.07, y + 0.1, z - Math.cos(f) * 0.07, 0.012, 0.05, 0.012, f, 0x7a5a35, 0.5 + Math.sin(t * 20) * 0.4);
    }
    // tavuklar: evlerin önünde eşinir
    if (near) for (let k = 0; k < Math.min(8, Math.floor(lay.homes.length / 2)); k++) {
      const hm = lay.homes[(k * 3 + 1) % lay.homes.length];
      const a = hash(sid + k, 5) * 6.28 + Math.sin(t * 0.4 + k) * 0.8;
      const x = hm[0] + Math.cos(a) * 0.1 + Math.sin(t * 0.3 + k) * 0.05, z = hm[1] + Math.sin(a) * 0.1, y = this.gy(x, z);
      const peck = Math.max(0, Math.sin(t * 5 + k * 1.7)) * 0.4;
      L.add('dot', x, y + 0.035, z, 0.7, 0.6, 0.9, a, k % 3 ? 0xf2efe6 : 0xb07a3a);
      L.add('dot', x + Math.sin(a) * 0.04, y + 0.06 - peck * 0.05, z + Math.cos(a) * 0.04, 0.4, 0.4, 0.4, 0, 0xc0392b);
    }
    // pazar: satıcılar tezgâhta, müşteriler dolaşır
    const stalls = lay.work.stall ?? [];
    stalls.forEach(([sx, sz], k) => {
      const mk = lay.work.market![0];
      const f = Math.atan2(sx - mk[0], sz - mk[1]);
      this.fig(sx - Math.sin(f) * 0.2, sz - Math.cos(f) * 0.2, f, t, sid + 300 + k, look(k + 2, { hat: k % 2 ? 'straw' : 'hood', hatColor: cc }), Math.sin(t * 1.5 + k) > 0.6 ? 'cheer' : 'idle', 0.8);
      if (k === 0) this.citizen(sx - Math.sin(f) * 0.2, sz - Math.cos(f) * 0.2, sid + 300 + k, race, 'tüccar', st.id, 'mal satıyor', '💰', closeUp);
      const q = (t * 0.05 + k * 0.37) % 1, nk = stalls[(k + 1) % stalls.length];
      const walking = q > 0.6;
      const cx = walking ? sx + (nk[0] - sx) * ((q - 0.6) / 0.4) : sx + Math.sin(f) * 0.08, cz = walking ? sz + (nk[1] - sz) * ((q - 0.6) / 0.4) : sz + Math.cos(f) * 0.08;
      this.fig(cx, cz, walking ? Math.atan2(nk[0] - sx, nk[1] - sz) : f + Math.PI, t, sid + 320 + k, look(k + 4, { sack: 0xc8b48a }), walking ? 'walk' : 'idle', 0.8);
    });
    // kıtlık: ambar önünde ekmek kuyruğu
    if (st.starving > 0) {
      const n = Math.min(near ? 9 : 5, 3 + Math.floor(P / 12));
      const a0 = Math.atan2(pz - lay.z, px - lay.x) + 0.8;
      for (let k = 0; k < n; k++) {
        const d = 0.3 + k * 0.12, x = lay.x + Math.cos(a0) * d, z = lay.z + Math.sin(a0) * d;
        if (this.groundY(x, z) < WATER_Y) break;
        this.fig(x, z, Math.atan2(lay.x - x, lay.z - z), t + k, sid + 400 + k, this.raceLook(race, 0x6a6258, { hat: 'hood', hatColor: 0x5a5248 }), k === 0 ? 'carry' : 'idle', 0.8);
        if (k % 3 === 1) this.citizen(x, z, sid + 400 + k, race, 'halk', st.id, 'ekmek kuyruğunda', '🍞', closeUp);
      }
    }
    // salgın: ceset arabası mezarlığa gider gelir
    const cem = lay.work.cemetery?.[0];
    if (st.plague && cem) {
      const q = (t * 0.03 + st.id * 0.2) % 2, u = q > 1 ? 2 - q : q;
      const x = px + (cem[0] - px) * u, z = pz + (cem[1] - pz) * u, y = this.gy(x, z);
      let f = Math.atan2(cem[0] - px, cem[1] - pz); if (q > 1) f += Math.PI;
      L.add('cart', x, y, z, 0.8, 0.8, 0.8, f + Math.PI / 2, 0x4a3a2a);
      L.add('wheel', x, y, z, 0.8, 0.8, 0.8, f + Math.PI / 2, 0x2a2a2a, 0, t * 4);
      if (q < 1) L.add('box', x, y + 0.2, z, 0.12, 0.05, 0.22, f, 0xd8d0c0);
      const hx = x + Math.sin(f) * 0.25, hz = z + Math.cos(f) * 0.25;
      this.fig(hx, hz, f, t, sid + 500, this.raceLook(race, 0x1e1c1c, { hat: 'hood', hatColor: 0x121010 }), 'carry', 0.85);
      this.citizen(hx, hz, sid + 500, race, 'mezarcı', st.id, 'ölüleri taşıyor', '⚰', closeUp);
    }
    // yeniden inşa: yanık evlerde ustalar
    (lay.work.ruin ?? []).slice(0, 3).forEach(([rx, rz], k) => {
      for (let j = 0; j < 2; j++) {
        const a = k + j * 2.6, x = rx + Math.cos(a) * 0.2, z = rz + Math.sin(a) * 0.2;
        this.fig(x, z, Math.atan2(rx - x, rz - z), t, sid + 600 + k * 2 + j, this.raceLook(race, 0x8a6a45, { tool: j ? 'mace' : 'none', sack: j ? undefined : 0x8a5a33 }), j ? 'work' : 'carry', 0.82);
        if (j) this.citizen(x, z, sid + 600 + k * 2 + j, race, 'usta', st.id, 'yanan evi onarıyor', '🔨', closeUp);
      }
    });
    // savaş: meydan dışında talim eden askerler
    if (atWar && st.soldiers > 0) {
      const n = Math.min(8, st.soldiers), a0 = Math.atan2(pz - lay.z, px - lay.x) + Math.PI;
      const bx = lay.x + Math.cos(a0) * (lay.R + 0.55), bz = lay.z + Math.sin(a0) * (lay.R + 0.55);
      if (this.groundY(bx, bz) > WATER_Y + 0.05) for (let k = 0; k < n; k++) {
        const row = Math.floor(k / 4), col = (k % 4) - 1.5;
        const x = bx + Math.cos(a0 + Math.PI / 2) * col * 0.2 + Math.cos(a0) * row * 0.22, z = bz + Math.sin(a0 + Math.PI / 2) * col * 0.2 + Math.sin(a0) * row * 0.22;
        this.fig(x, z, Math.atan2(lay.x - x, lay.z - z) + Math.PI, t, sid + 700 + k, this.raceLook(race, 0x5a5f66, { hat: 'helm', tool: 'spear', shield: cc }), 'fight', 0.9);
        if (k === 0) this.citizen(x, z, sid + 700, race, 'asker', st.id, 'savaşa hazırlanıyor', '⚔', closeUp);
      }
    }
  }

  /**
   * Yaban hayatı: yalnız kameranın çevresinde. Geyik sürüleri ormanda ve orman kıyısında otlar, tavşanlar çayırda sekip durur,
   * göllerde ve kıyıda balık sıçrar; yaz gecesi ormanda ateş böcekleri, ilkbahar-yaz gündüzü çiçekli çayırda kelebekler.
   * İnsan yerleşimine yakın yerlerde (kasabadan 3 karo içinde) av hayvanı yok.
   */
  private nearTown = new Set<number>();
  /**
   * Gezginde ayak altı çimen: kameranın çevresindeki dünyaya sabit bir ızgarada (kamerayla kaymaz) sık tutamlar.
   * Yalnız yürürken / alçak uçarken; tarla, yol, kasaba ve su dışında. Kenara doğru küçülür (belirip kaybolmasın).
   */
  private nearGrass(L: Layer) {
    if (!this.fly.on) return;
    const alt = this.camAlt();
    if (alt > 3.5) return;
    const s = this.sim, w = s.w, cp = this.camera.position;
    const R = 6.5 - alt, C = 0.2, season = s.season;
    const winter = season === 3, autumn = season === 2;
    const col = winter ? 0xc9d2c4 : autumn ? 0x9a9a4a : 0x5f9a45;
    const i0 = Math.floor((cp.x - R) / C), i1 = Math.ceil((cp.x + R) / C), k0 = Math.floor((cp.z - R) / C), k1 = Math.ceil((cp.z + R) / C);
    const ok = new Map<number, boolean>();
    const f = this.fly.forward();
    for (let a = i0; a <= i1; a++) for (let b = k0; b <= k1; b++) {
      const h1 = hash(a * 7919 + b, 501), h2 = hash(a * 104729 + b, 502);
      const x = (a + h1) * C, z = (b + h2) * C;
      const d2 = (x - cp.x) ** 2 + (z - cp.z) ** 2;
      if (d2 > R * R || d2 < 0.3) continue; // ayak dibindekiler göz önünde dev diken gibi görünüyordu
      // arkada kalanları atla (görünmez, boşuna)
      if ((x - cp.x) * f.x + (z - cp.z) * f.z < -1.2) continue;
      const ti = this.nearestTile(x, z);
      if (ti < 0) continue;
      let good = ok.get(ti);
      if (good === undefined) {
        const t = w.tiles[ti];
        good = (t.terrain === 'grass' || t.terrain === 'forest' || t.terrain === 'oldforest' || t.terrain === 'hill' || t.terrain === 'swamp') && !t.road && !(t.ext && (t.ext.kind === 'farm' || t.ext.kind === 'quarry' || t.ext.kind === 'mine' || t.ext.kind === 'claypit')) && !w.settlements.some((q) => q.alive && q.tile === ti);
        ok.set(ti, good);
      }
      if (!good) continue;
      const y = this.hf.sample(x, z);
      if (y < WATER_Y + 0.04) continue;
      const edge = Math.min(1, (R - Math.sqrt(d2)) / 1.5);
      const sc = (0.5 + hash(a + b * 31, 503) * 0.45) * edge * Math.min(1, Math.sqrt(d2) / 1.2);
      L.add('tuft', x, y - 0.01, z, sc, sc * (0.8 + h1 * 0.6), sc, h2 * 6, jitter(col, h1, 0.08, 0.02), (h2 - 0.5) * 0.5);
      if (!winter && season <= 1 && h1 > 0.93) L.add('flower', x + 0.03, y + 0.06 * sc, z, 1, 1, 1, 0, [0xf2e25a, 0xffffff, 0xd96a8a, 0x9a7ad9][Math.floor(h2 * 4)]);
    }
  }
  private wildlife(L: Layer, t: number) {
    const s = this.sim, w = s.w, tg = this.controls.target;
    const camD = this.camera.position.distanceTo(tg);
    if (camD > 45) return;
    const R = Math.min(26, 10 + camD * 0.6);
    const r0 = Math.max(0, Math.floor((tg.z - R) / 1.5)), r1 = Math.min(w.H - 1, Math.ceil((tg.z + R) / 1.5));
    const c0 = Math.max(0, Math.floor((tg.x - R) / SQ3) - 1), c1 = Math.min(w.W - 1, Math.ceil((tg.x + R) / SQ3) + 1);
    const day = this.night < 0.55, season = s.season;
    const townNear = (i: number) => this.nearTown.has(i);
    let deer = 0, hare = 0, fish = 0, ffly = 0, bfly = 0;
    for (let r = r0; r <= r1; r++) for (let c = c0; c <= c1; c++) {
      const i = r * w.W + c, tl = w.tiles[i];
      const [x, z] = this.pos(i);
      if ((x - tg.x) ** 2 + (z - tg.z) ** 2 > R * R) continue;
      const tr = tl.terrain;
      const forest = tr === 'forest' || tr === 'oldforest';
      // geyik: gündüz ve alacakaranlıkta; gece yatar (görünmez)
      if (day && deer < 24 && (forest || tr === 'grass' || tr === 'tundra') && hash(i, 301) < (forest ? 0.14 : 0.05) && !townNear(i)) {
        const n = 1 + Math.floor(hash(i, 302) * 3);
        for (let k = 0; k < n; k++) {
          const id = i * 7 + k;
          // sürekli bir yol üstünde dolaşır: yürü (%35) → dur, otla; yol parametresi yalnız yürürken ilerler (sıçrama yok)
          const sc = t * 0.06 + hash(id, 303) * 10, fr = sc - Math.floor(sc);
          const walking = fr < 0.35;
          const pth = Math.floor(sc) * 0.35 + Math.min(fr, 0.35);
          const ph = hash(id, 304) * 6.28, u = pth * 1.6 + ph;
          const R1 = 0.35 + hash(id, 305) * 0.3, R2 = 0.3 + hash(id, 306) * 0.3;
          const ox = (k - 1) * 0.16, oz = hash(id, 307) * 0.16;
          const px = x + Math.cos(u) * R1 + ox, pz = z + Math.sin(u * 1.37) * R2 + oz;
          const py = this.gy(px, pz);
          if (py <= WATER_Y + 0.02) continue;
          // yön: yolun türevi (model +z'ye bakar → ry = atan2(dx, dz))
          const dx = -Math.sin(u) * R1, dz = Math.cos(u * 1.37) * 1.37 * R2;
          const head = Math.atan2(dx, dz) + (walking ? 0 : Math.sin(t * 0.3 + k) * 0.5);
          const graze = !walking && Math.sin(t * 0.9 + id) > -0.2;
          const bob = walking ? Math.abs(Math.sin(t * 7 + id)) * 0.012 : 0;
          const stag = k === 0 && hash(i, 308) > 0.45;
          L.add(stag ? 'deer_m' : 'deer', px, py + bob + (graze ? 0.006 : 0), pz, 0.9, 0.9, 0.9, head, stag ? 0x8a5a36 : 0x9a6a42, graze ? 0.12 : 0);
          deer++;
        }
      }
      // tavşan: çayırda, zıplayıp durur
      if (day && hare < 20 && tr === 'grass' && hash(i, 311) < 0.07 && !townNear(i)) {
        const id = i * 3;
        // sek-sek: her saniyenin ilk %40'ında zıplar, yol yalnız zıplarken ilerler
        const sc = t * 1.1 + hash(id, 312) * 5, fr = sc - Math.floor(sc);
        const hopping = fr < 0.4 && Math.floor(sc) % 3 !== 0;
        const pth = Math.floor(sc) * 0.4 + Math.min(fr, 0.4) * (hopping ? 1 : 0) + (Math.floor(sc) % 3 === 0 ? 0 : 0);
        const u = pth * 0.35 + hash(id, 313) * 6.28;
        const px = x + Math.cos(u) * 0.45, pz = z + Math.sin(u * 1.3) * 0.4, py = this.gy(px, pz);
        const hop = hopping ? Math.sin((fr / 0.4) * Math.PI) * 0.035 : 0;
        const head = Math.atan2(-Math.sin(u) * 0.45, Math.cos(u * 1.3) * 1.3 * 0.4);
        L.add('rabbit', px, py + hop, pz, 1, 1, 1, head, season === 3 ? 0xeeeeea : 0x9a8468);
        hare++;
      }
      // balık sıçraması: göl, nehir ve kıyı suyu
      if (fish < 8 && tr === 'water' && hash(i, 321) < 0.18) {
        const period = 5 + hash(i, 322) * 6, ph = (t + hash(i, 323) * period) % period;
        if (ph < 0.7) {
          const u = ph / 0.7, a = hash(i, 324) * 6.28 + Math.floor((t + hash(i, 323) * period) / period);
          const bx = x + Math.cos(a) * (0.2 + hash(i, 325) * 0.4) + Math.cos(a + 1.57) * (u - 0.5) * 0.25, bz = z + Math.sin(a) * (0.2 + hash(i, 325) * 0.4) + Math.sin(a + 1.57) * (u - 0.5) * 0.25;
          L.add('fish', bx, WATER_Y + Math.sin(u * Math.PI) * 0.16, bz, 1, 1, 1, -a, 0xb8c4c8, (u - 0.5) * 2.4);
          fish++;
        } else if (ph < 1.6) {
          const q = (ph - 0.7) / 0.9, a = hash(i, 324) * 6.28 + Math.floor((t + hash(i, 323) * period) / period);
          const bx = x + Math.cos(a) * (0.2 + hash(i, 325) * 0.4) + Math.cos(a + 1.57) * 0.125, bz = z + Math.sin(a) * (0.2 + hash(i, 325) * 0.4) + Math.sin(a + 1.57) * 0.125;
          L.add('ring', bx, WATER_Y + 0.02, bz, 0.05 + q * 0.14, 1, 0.05 + q * 0.14, 0, 0xe8f2f6);
        }
      }
      // ateş böcekleri: yaz gecesi, orman ve bataklık
      if (!day && season === 1 && ffly < 90 && (forest || tr === 'swamp') && hash(i, 331) < 0.5) {
        for (let k = 0; k < 6; k++) {
          const id = i * 11 + k;
          const blink = Math.sin(t * (1.3 + hash(id, 332)) + id) * 0.5 + 0.5;
          if (blink < 0.35) continue;
          const px = x + (hash(id, 333) - 0.5) * 1.4 + Math.sin(t * 0.4 + id) * 0.15, pz = z + (hash(id, 334) - 0.5) * 1.4 + Math.cos(t * 0.35 + id) * 0.15;
          L.add('ffly', px, this.gy(px, pz) + 0.08 + hash(id, 335) * 0.3 + Math.sin(t * 0.9 + id) * 0.04, pz, 0.6 + blink, 0.6 + blink, 0.6 + blink, 0, 0xffffff);
          ffly++;
        }
      }
      // kelebek: ilkbahar-yaz gündüzü, çayır
      if (day && season <= 1 && bfly < 24 && tr === 'grass' && hash(i, 341) < 0.12) {
        for (let k = 0; k < 2; k++) {
          const id = i * 5 + k;
          const a = t * (0.5 + hash(id, 342) * 0.4) + id, rr = 0.25 + hash(id, 343) * 0.35;
          const px = x + Math.cos(a) * rr + Math.sin(t * 1.7 + id) * 0.05, pz = z + Math.sin(a * 1.3) * rr;
          const flap = Math.abs(Math.sin(t * 16 + id));
          L.add('bfly', px, this.gy(px, pz) + 0.12 + Math.sin(t * 2.3 + id) * 0.05, pz, 0.25 + flap * 0.9, 1, 1, a + 1.57, [0xf2e25a, 0xffffff, 0xe0784a, 0x9fc4ff][Math.floor(hash(id, 344) * 4)], 0, (flap - 0.5) * 1.2);
          bfly++;
        }
      }
    }
  }
  /** sabah sisi: şafakta (ve yağmurda) göl, nehir ve alçak vadilerin üstünde yerde süzülen pus */
  private mist(t: number) {
    const s = this.sim, w = s.w, tg = this.controls.target, c = this.cyc;
    const dawn = !this.dayNight && this.debugCyc === undefined ? 0 : c > 0.9 ? Math.min(1, (c - 0.9) / 0.06) : c < 0.13 ? 1 - c / 0.13 : 0;
    const sea = [0.8, 0.4, 1, 0.65][s.season];
    const m = Math.min(1, dawn * sea + this.atmo.rainAmount * 0.45);
    if (m < 0.03 || this.camera.position.distanceTo(tg) > 70) return;
    const R = 34;
    const r0 = Math.max(0, Math.floor((tg.z - R) / 1.5)), r1 = Math.min(w.H - 1, Math.ceil((tg.z + R) / 1.5));
    const c0 = Math.max(0, Math.floor((tg.x - R) / SQ3) - 1), c1 = Math.min(w.W - 1, Math.ceil((tg.x + R) / SQ3) + 1);
    let n = 0;
    for (let r = r0; r <= r1 && n < 280; r++) for (let q = c0; q <= c1 && n < 280; q++) {
      const i = r * w.W + q, tl = w.tiles[i];
      const [x, z] = this.pos(i);
      if ((x - tg.x) ** 2 + (z - tg.z) ** 2 > R * R) continue;
      const wet = tl.terrain === 'water';
      const low = !wet && this.hc[i] < WATER_Y + 0.9 && (tl.terrain === 'swamp' || this.sim.g.neighbors(i).some((j) => w.tiles[j].terrain === 'water'));
      if (!(wet && (!tl.sea || (this.coastDist[i] ?? 9) <= 1)) && !low) continue;
      if (hash(i, 351) > 0.6) continue;
      for (let k = 0; k < 2; k++) {
        const id = i * 3 + k, drift = ((t * 0.035 + hash(id, 352)) % 1) - 0.5;
        const px = x + (hash(id, 353) - 0.5) * 1.4 + drift * 1.2, pz = z + (hash(id, 354) - 0.5) * 1.3;
        const py = Math.max(this.gy(px, pz), WATER_Y) + 0.1 + hash(id, 355) * 0.22;
        const fade = Math.sin((drift + 0.5) * Math.PI);
        this.smoke.add(px, py, pz, 2.6 + hash(id, 356) * 2.2, 0xe4eaee, 0.24 * m * fade * (0.7 + 0.3 * Math.sin(t * 0.2 + i)));
        n++;
      }
    }
  }
  /** tersane: kıyıdan suya inen kızak, vinç, ambar, gemilerin bağlandığı iskele; Fener Kulesi */
  private harbor(st: Settlement) {
    const s = this.sim, w = s.w, L = this.dynamic;
    const port = st.port!;
    const wn = portWater(s, port);
    if (wn < 0) return;
    const c = w.civs[st.civ], civCol = new THREE.Color(c.color).getHex();
    const [x, z] = this.pos(port), [wx, wz] = this.pos(wn);
    const ang = Math.atan2(wx - x, wz - z);
    const ux = Math.sin(ang), uz = Math.cos(ang), px = Math.cos(ang), pz = -Math.sin(ang);
    const plank = (ox: number, d0: number, d1: number, wid: number, col: number) => {
      const x1 = x + ux * d0 + px * ox, z1 = z + uz * d0 + pz * ox, y1 = Math.max(this.groundY(x1, z1), WATER_Y) + 0.03;
      const hor = d1 - d0, y2 = WATER_Y + 0.08;
      L.add('road', x1, y1, z1, wid / 0.28, 1.2, Math.hypot(hor, y2 - y1), ang, col, -Math.atan2(y2 - y1, hor));
      for (let q = d0 + 0.3; q <= d1 + 0.01; q += 0.26) {
        const qx = x + ux * q + px * ox, qz = z + uz * q + pz * ox, gy = Math.min(this.groundY(qx, qz), WATER_Y) - 0.14;
        const dy = y1 + (y2 - y1) * Math.min(1, (q - d0) / hor);
        for (const e of [-1, 1]) L.add('post', qx + px * e * wid * 0.45, gy, qz + pz * e * wid * 0.45, 1.4, dy - gy + 0.06, 1.4, 0, 0x5a4330);
      }
    };
    // kızak: iki kalas ray, suya iner
    const kx = -0.42;
    for (const e of [-1, 1]) {
      const x1 = x + ux * 0.05 + px * (kx + e * 0.13), z1 = z + uz * 0.05 + pz * (kx + e * 0.13), y1 = this.groundY(x1, z1) + 0.02;
      const x2 = x + ux * 1.25 + px * (kx + e * 0.13), z2 = z + uz * 1.25 + pz * (kx + e * 0.13), y2 = WATER_Y - 0.12;
      const hor = 1.2;
      L.add('road', x1, y1, z1, 0.3, 1.4, Math.hypot(hor, y2 - y1), ang, 0x6a4a2c, -Math.atan2(y2 - y1, hor));
    }
    // tezgâhtaki gemi: ilerledikçe kaburgalar, sonra kaplama
    if (st.project?.type === 'ship') {
      const prog = 1 - st.project.left / st.project.total;
      const galley = st.project.kind === 'galley';
      const cx = x + ux * 0.5 + px * kx, cz = z + uz * 0.5 + pz * kx, cy = Math.max(this.groundY(cx, cz), WATER_Y - 0.05) + 0.05;
      L.add('box', cx, cy, cz, 0.05, 0.05, galley ? 1.2 : 1.0, ang, 0x6b4a2b); // omurga
      const ribs = 2 + Math.floor(Math.min(1, prog * 1.6) * 7);
      for (let k = 0; k < ribs; k++) {
        const along = (k / 8 - 0.5) * (galley ? 1.1 : 0.9), w0 = 0.16 * (1 - Math.pow(Math.abs(along) / 0.6, 2));
        const rx = cx + ux * along, rz = cz + uz * along;
        for (const e of [-1, 1]) L.add('box', rx + px * e * w0, cy, rz + pz * e * w0, 0.025, 0.2, 0.025, ang, 0x8a6a45, 0, -e * 0.35);
      }
      if (prog > 0.6) L.add('keel', cx, cy - 0.02, cz, 0.34 * Math.min(1, (prog - 0.6) * 2.5 + 0.4), 0.16, galley ? 1.25 : 1.0, ang, 0x6b4a2b);
      // iskele iskelesi: yan destek kazıkları
      for (const e of [-1, 1]) for (const q of [-0.3, 0.1, 0.4]) L.add('post', cx + ux * q + px * e * 0.3, this.groundY(cx + ux * q + px * e * 0.3, cz + uz * q + pz * e * 0.3) - 0.05, cz + uz * q + pz * e * 0.3, 1.2, 0.34, 1.2, 0, 0x5a4330);
    }
    // vinç
    const vx = x + ux * 0.2 + px * (kx - 0.38), vz = z + uz * 0.2 + pz * (kx - 0.38), vy = this.groundY(vx, vz);
    L.add('post', vx, vy, vz, 2.2, 1.25, 2.2, 0, 0x5a4330);
    L.add('log', vx + ux * 0.28, vy + 1.18, vz + uz * 0.28, 1.6, 0.5, 0.5, ang + Math.PI / 2, 0x5a4330);
    L.add('post', vx + ux * 0.52, vy + 0.72, vz + uz * 0.52, 0.4, 0.46, 0.4, 0, 0x2a2622);
    // ambar
    const ax = x - ux * 0.35 + px * 0.2, az = z - uz * 0.35 + pz * 0.2, ay = this.groundY(ax, az);
    L.add('house', ax, ay, az, 0.42, 0.22, 0.3, ang, 0x9b7650);
    L.add('gable', ax, ay + 0.22, az, 0.44, 0.24, 0.32, ang + Math.PI / 2, civCol);
    L.add('box', ax + ux * 0.28, this.groundY(ax + ux * 0.28, az + uz * 0.28), az + uz * 0.28, 0.1, 0.08, 0.1, 0.3, 0x9a6a3a);
    L.add('box', ax + ux * 0.3 + px * 0.12, this.groundY(ax + ux * 0.3, az + uz * 0.3), az + uz * 0.3 + pz * 0.12, 0.08, 0.1, 0.08, 0.8, 0x7a5a3a);
    // iskele: gemiler iki yanına bağlanır
    plank(0.38, 0.1, 1.75, 0.26, 0x8a6a45);
    // iki gemi iskelenin yanına bağlanır, fazlası açıkta demirler
    const b: Berth[] = [];
    for (const e of [1, -1]) b.push({ x: x + ux * 1.25 + px * (0.38 + e * 0.36), z: z + uz * 1.25 + pz * (0.38 + e * 0.36), face: ang + (e > 0 ? 0 : Math.PI), st: st.id });
    for (const [d, o] of [[2.9, 0.3], [2.7, -1.2], [3.1, 1.6], [3.6, -0.3]] as [number, number][]) {
      const bx = x + ux * d + px * o, bz = z + uz * d + pz * o;
      if (this.groundY(bx, bz) < WATER_Y - 0.15) b.push({ x: bx, z: bz, face: ang + 1.2 + hash(st.id, d * 10) * 1.5, st: st.id });
    }
    this.berths.set(st.id, b);
    // fener kulesi: kıyının burnunda
    if (st.civics.lighthouse) {
      const lx = x + ux * 0.55 + px * 1.05, lz = z + uz * 0.55 + pz * 1.05, ly = Math.max(this.groundY(lx, lz), WATER_Y - 0.1);
      for (let k = 0; k < 5; k++) L.add('rock', lx + Math.cos(k * 1.3) * 0.2, ly, lz + Math.sin(k * 1.3) * 0.2, 1.2, 0.8, 1.2, k, 0x7a746c);
      L.add('tower', lx, ly, lz, 0.2, 1.05, 0.2, 0, 0xd9d2c4);
      L.add('tower', lx, ly + 0.32, lz, 0.205, 0.1, 0.205, 0, 0xa0442e);
      L.add('tower', lx, ly + 0.7, lz, 0.19, 0.1, 0.19, 0, 0xa0442e);
      L.add('lantern', lx, ly + 1.12, lz, 1.3, 1.3, 1.3, 0);
      L.add('spire', lx, ly + 1.2, lz, 0.2, 0.18, 0.2, 0, 0x5a3a2a);
    }
  }

  /** denizdeki birim: kervan → yüklü koga, öncüler → yolcu kogası, ordu → kadırgalar ve asker kogaları */
  private agentShip(a: Agent, x: number, z: number, face: number, t: number, cc: number, race: string) {
    const s = this.sim, c = a.civ >= 0 ? s.w.civs[a.civ] : undefined;
    const big = c && s.has(c, 'shipbuilding') ? 'cog' : 'boat';
    const sx = Math.cos(face), sz = -Math.sin(face), fx = Math.sin(face), fz = Math.cos(face);
    const moving = a.step < a.path.length - 1;
    switch (a.kind) {
      case 'caravan': {
        const g = Object.keys(a.cargo ?? {})[0];
        this.ship(x, z, face, t, a.id, big, cc, race, { load: 'crates', moving, cargo: g === 'grain' || g === 'bread' ? 0xd8c48a : g === 'fish' ? 0x9fb4c0 : g === 'stone' || g === 'iron' ? 0x8d877e : 0x9a6a3a });
        break;
      }
      case 'settlers': this.ship(x, z, face, t, a.id, big, cc, race, { load: 'people', moving }); break;
      case 'army': {
        const gal = a.galleys ?? 0;
        const cogs = Math.max(1, Math.min(3, Math.ceil((a.troops ?? 1) / 8)));
        for (let k = 0; k < cogs; k++) this.ship(x - fx * k * 1.25 + sx * (k % 2 ? 0.55 : -0.2), z - fz * k * 1.25 + sz * (k % 2 ? 0.55 : -0.2), face, t, a.id * 3 + k, big, cc, race, { load: 'troops', moving });
        for (let k = 0; k < gal; k++) { const e = k % 2 ? 1 : -1, row = Math.floor(k / 2); this.ship(x + fx * (0.9 - row * 1.3) + sx * e * 0.95, z + fz * (0.9 - row * 1.3) + sz * e * 0.95, face, t, a.id * 5 + k, 'galley', cc, race, { moving }); }
        break;
      }
      case 'ship':
        if (a.purpose === 'fleet' || a.purpose === 'fleet-back') {
          const n = Math.max(1, a.galleys ?? 1);
          for (let k = 0; k < n; k++) { const e = k === 0 ? 0 : k % 2 ? 1 : -1, row = Math.ceil(k / 2); this.ship(x - fx * row * 1.1 + sx * e * 0.8 * row, z - fz * row * 1.1 + sz * e * 0.8 * row, face, t, a.id * 5 + k, 'galley', cc, race, { moving }); }
        } else this.ship(x, z, face, t, a.id, a.purpose?.startsWith('explore') ? 'scout' : big, cc, race, { moving });
        break;
      default: this.ship(x, z, face, t, a.id, 'boat', cc, race, { load: 'people', moving });
    }
  }

  private buildActors(now: number) {
    const s = this.sim, w = s.w, L = this.actors;
    const t = now / 1000;
    const cd = this.camera.position.distanceTo(this.controls.target);
    this.detail = cd < (this.quality === 0 ? 32 : this.quality === 1 ? 45 : 60);
    // yakın planda figürler gerçekçi ölçüye iner, uzakta okunur kalsın diye büyür (gezginde yüksekliğe göre)
    this.figK = 0.7 + 0.3 * Math.min(1, Math.max(0, ((this.fly.on ? this.camAlt() * 1.4 : cd) - 6) / 18));
    L.begin();
    this.smoke.begin();
    this.heroSpots.clear();
    const TUNIC = [0x9a8a6a, 0x7a6a4a, 0x8a5a3a, 0x6a7a5a, 0xa89a7a, 0x5a6a7a];
    this.cits = []; this.bubbles = [];
    const closeUp = this.camera.position.distanceTo(this.controls.target) < 16;
    const warCiv = new Set<number>();
    for (const a of w.civs) for (const b of w.civs) if (a.id < b.id && this.sim.rel(a.id, b.id)?.war) { warCiv.add(a.id); warCiv.add(b.id); }
    // halk: nüfusa orantılı; evinden işine gider, çalışır, akşam döner
    const cyc = this.cyc;
    const camP = this.camera.position;
    for (const st of w.settlements) {
      if (!st.alive) continue;
      const lay = this.layouts.get(st.id);
      if (!lay) continue;
      const c = w.civs[st.civ];
      const cc = new THREE.Color(c.color).getHex();
      const [x, z] = [lay.x, lay.z];
      const dCam = Math.hypot(camP.x - x, camP.y - this.height(st.tile), camP.z - z);
      const [scale, cap] = dCam < 18 ? [0.6, 60] : dCam < 30 ? [0.35, 34] : dCam < 50 ? [0.15, 14] : [0.05, 4];
      const roles = this.rolesOf(st);
      const n = Math.min(cap, Math.round(roles.length * scale * (st.plague ? 0.35 : 1)));
      const fol = this.follow?.kind === 'cit' && this.follow.id === st.id ? this.follow.idx ?? -1 : -1;
      for (let i = 0; i < n || i === fol; i++) {
        if (i >= n && i !== fol) continue;
        const cs = this.citState(st, lay, roles, i, t);
        if (!cs.visible) { if (i === fol) this.bubbles.unshift({ x: cs.home[0], y: this.gy(cs.home[0], cs.home[1]) + 0.5, z: cs.home[1], icon: '💤', id: cs.id, fixed: true }); continue; }
        if (i === fol && cs.icon) this.bubbles.unshift({ x: cs.x, y: this.gy(cs.x, cs.z) + 0.55, z: cs.z, icon: cs.icon, id: cs.id, fixed: true });
        this.fig(cs.x, cs.z, cs.face, t, cs.id, cs.look, cs.pose, 0.85);
        if (i === fol) { this.actors.add('halo', cs.x, this.gy(cs.x, cs.z) + 0.02, cs.z, 0.9, 1, 0.9, 0, 0xffffff); if (this.night > 0.5) this.actors.add('lantern', cs.x + Math.cos(cs.face) * 0.08, this.gy(cs.x, cs.z) + 0.2, cs.z - Math.sin(cs.face) * 0.08, 0.6, 0.6, 0.6, 0, 0xffc45a); }
        if (dCam < 30) this.citizen(cs.x, cs.z, cs.id, cs.race, cs.job, st.id, cs.mood, cs.icon, closeUp, i);
        if (i > 400) break;
      }
      this.townLife(st, lay, c.race, cc, dCam, t, warCiv.has(st.civ), closeUp);
      // nöbetçiler: sur boyunca devriye, gece fener taşır
      const sol = Math.min(dCam < 30 ? 12 : 4, st.soldiers);
      const Rw = (st.civics.palisade || st.civics.stonewall || st.civics.castle) ? lay.wallR - 0.15 : lay.R * 1.9 + 0.5;
      for (let k = 0; k < sol; k++) {
        const patrol = k % 3 !== 0;
        const dir = k % 2 ? 1 : -1;
        const a = patrol ? (k / Math.max(1, sol)) * Math.PI * 2 + dir * t * 0.04 : (k / Math.max(1, sol)) * Math.PI * 2 + 0.4;
        const px = x + Math.cos(a) * Rw, pz = z + Math.sin(a) * Rw;
        const face = patrol ? Math.atan2(-Math.sin(a) * dir, Math.cos(a) * dir) : Math.atan2(Math.cos(a), Math.sin(a));
        this.fig(px, pz, face, t, st.id * 7 + k, this.raceLook(c.race, c.cls === 'warlock' ? 0x2e2833 : 0x5a5f66, { hat: 'helm', tool: 'spear', shield: cc, legs: 0x3a3a3a }), patrol ? 'walk' : 'guard', 0.95);
        if (this.night > 0.5 && k % 2 === 0) L.add('lantern', px + Math.cos(face) * 0.1, this.gy(px, pz) + 0.25, pz - Math.sin(face) * 0.1, 0.7, 0.7, 0.7, 0, 0xffc45a);
      }
    }
    // yangınlar ve bacalar
    for (const [bx, by, bz, k] of this.burning) {
      const fl = 0.8 + Math.sin(t * 11 + bx) * 0.2 + Math.sin(t * 17 + bz) * 0.1;
      for (let q = 0; q < 3; q++) {
        const ox = Math.cos(q * 2.1 + bx) * 0.12, oz = Math.sin(q * 2.1 + bx) * 0.12, f = fl * (1 + Math.sin(t * 9 + q * 2) * 0.2);
        L.add('fire', bx + ox, by - 0.05, bz + oz, 0.9 * k * f, 1.5 * k * f, 0.9 * k * f, t * (q % 2 ? 1 : -1), 0xffffff);
      }
      L.add('lantern', bx, by + 0.05, bz, 2 * k * fl, 2 * k * fl, 2 * k * fl, t, 0xffb03a);
      // kalın, rüzgârla yatan kara duman sütunu
      for (let q = 0; q < 7; q++) { const u = (t * 0.4 + q / 7 + bx * 0.1) % 1; const ps = puffS(u, 1.0, 3.2) * k; this.smoke.add(bx + u * u * 0.6, by + 0.15 + u * 1.9, bz + Math.sin(u * 6 + q) * 0.12, ps * 0.3, u < 0.45 ? 0x24201e : 0x4a4644, 0.92 * (1 - u * 0.55)); }
    }
    for (const [sx, sy, sz] of this.smokes) {
      if ((camP.x - sx) ** 2 + (camP.z - sz) ** 2 > 3600) continue;
      for (let q = 0; q < 5; q++) { const u = (t * 0.22 + q / 5 + sx * 0.37) % 1; const ps = puffS(u, 0.45, 1.5); this.smoke.add(sx + u * u * 0.35, sy + u * 1.0, sz + Math.sin(u * 5 + q) * 0.05, ps * 0.2, 0xe8e8e6, 0.42 * (1 - u * 0.75)); }
    }
    // evdeki / tavernadaki kahramanlar
    const moving = new Set<number>();
    for (const a of w.agents) if (!a.dead) for (const id of a.heroes ?? []) moving.add(id);
    for (const h of w.heroes) {
      if (h.state !== 'home' && h.state !== 'tavern' && !(h.state === 'quest' && !moving.has(h.id))) continue;
      const [x, z] = this.pos(h.pos);
      const atInn = h.state === 'tavern' && w.tiles[h.pos].inn !== undefined;
      // handa: gündüz avluda talim ya da panoya bakar, gece içeride
      if (atInn && this.night > 0.6 && hash(h.id, 4) > 0.2) continue;
      const a = hash(h.id, 1) * 6.28, r = atInn ? 0.55 + hash(h.id, 2) * 0.25 : 0.45, px = x + Math.cos(a) * r, pz = z + Math.sin(a) * r;
      const drill = atInn && hash(h.id, 3) > 0.5 && Math.sin(t * 0.2 + h.id) > 0;
      this.heroFig(px, pz, t, h.id, a + Math.PI + Math.sin(t * 0.3 + h.id) * 0.6, drill ? 'fight' : Math.sin(t * 0.5 + h.id) > 0.85 ? 'cheer' : 'idle');
    }
    // hanlar: inşaatta hancı ve yardımcıları çalışır (gece ateş başında); açık handa hancı, personel ve avluda misafirler
    for (const inn of w.inns) {
      if (inn.stage === 'road' || inn.stage === 'ruin') continue;
      const [ix, iz] = this.pos(inn.tile);
      if ((camP.x - ix) ** 2 + (camP.z - iz) ** 2 > 3600) continue;
      const ia = hash(inn.id, 7) * Math.PI * 2;
      const ifx = Math.sin(ia), ifz = Math.cos(ia), irx = Math.cos(ia), irz = -Math.sin(ia);
      const at2 = (f: number, r: number): [number, number] => [ix + ifx * f + irx * r, iz + ifz * f + irz * r];
      const day = this.night < 0.6;
      const reg = (px: number, pz: number, name: string, race: string, role: string, mood: string) => { if (closeUp) this.cits.push({ x: px, y: this.gy(px, pz), z: pz, name, role, st: -1, mood, race, idx: -1 }); };
      const keeperLook = this.raceLook(inn.keeperRace, 0xe8e0c8, { legs: 0x4a3a2a, hat: inn.keeperRace === 'elf' || inn.keeperRace === 'halfelf' ? 'ears' : 'none' });
      if (this.selInn === inn.id) {
        const pulse = 1 + Math.sin(t * 3) * 0.06;
        L.add('halo', ix, this.gy(ix, iz) + 0.03, iz, 4.9 * pulse, 0.6, 4.9 * pulse, 0, 0xf2cf5b);
      }
      const b = inn.build;
      if (b) {
        // inşaat: yeni han ya da kanat; kereste yığınından iskeleye taşıyan çırak, çekiç sallayan hancı
        const up = inn.stage === 'open';
        const [sx0, sz0] = up ? (b.level === 2 ? at2(-0.12, -0.52) : at2(0, 0)) : at2(0, 0);
        const [px0, pz0] = up ? (b.level === 2 ? at2(-0.42, -0.62) : at2(0.62, 0.62)) : at2(0.05, 0.62);
        if (day) {
          const kx = sx0 + ifx * 0.34, kz = sz0 + ifz * 0.34 + Math.sin(t * 0.3 + inn.id) * 0.05;
          this.fig(kx, kz, ia + Math.PI, t, inn.id * 7, { ...keeperLook, tool: 'pick' }, 'work', 0.95);
          reg(kx, kz, inn.keeper, inn.keeperRace, 'hancı', up ? 'hanını büyütüyor' : `${inn.name} Hanı'nı kuruyor`);
          inn.staff.forEach((x, k) => {
            if (k === 0) {
              const loop = (t / 9 + hash(inn.id, 50)) % 1, go = loop < 0.5, q = go ? loop / 0.5 : (loop - 0.5) / 0.5;
              const ax = go ? px0 : sx0, az = go ? pz0 : sz0, bx = go ? sx0 : px0, bz = go ? sz0 : pz0;
              const cx2 = ax + (bx - ax) * q, cz2 = az + (bz - az) * q;
              this.fig(cx2, cz2, Math.atan2(bx - ax, bz - az), t, inn.id * 7 + 1, this.raceLook(x.race, TUNIC[k], go ? { sack: 0x9a6a3a } : {}), go ? 'carry' : 'walk', 0.85);
              reg(cx2, cz2, x.name, x.race, 'çırak', 'kereste taşıyor');
            } else if (k < 3) {
              const [wx2, wz2] = [sx0 - ifx * 0.34 + irx * (k - 1.5) * 0.3, sz0 - ifz * 0.34 + irz * (k - 1.5) * 0.3];
              this.fig(wx2, wz2, ia, t, inn.id * 7 + 1 + k, this.raceLook(x.race, TUNIC[k], { tool: k % 2 ? 'axe' : 'shovel' }), 'work', 0.85);
              reg(wx2, wz2, x.name, x.race, 'yardımcı', 'inşaatta çalışıyor');
            }
          });
          if (inn.gold > 30) for (let k = 0; k < (inn.gold > 90 ? 2 : 1); k++) {
            const [lx, lz] = [sx0 + irx * (k ? -0.42 : 0.42), sz0 + irz * (k ? -0.42 : 0.42)];
            this.fig(lx, lz, ia + Math.PI / 2 * (k ? 1 : -1), t, inn.id * 7 + 9 + k, this.raceLook('human', TUNIC[(inn.id + k) % TUNIC.length], { tool: 'shovel', hat: 'straw' }), 'work', 0.85);
            reg(lx, lz, 'Gündelikçi', 'human', 'gündelikçi', 'günlüğü 2 gümüşe çalışıyor');
          }
        } else if (!up) {
          // gece: çadırın önünde ateş
          const [fx0, fz0] = at2(-0.35, 0.35), fy = this.gy(fx0, fz0);
          const fl = 0.8 + Math.sin(t * 11 + inn.id) * 0.2;
          L.add('fire', fx0, fy - 0.04, fz0, 0.7 * fl, 1.1 * fl, 0.7 * fl, t, 0xffffff);
          L.add('lantern', fx0, fy + 0.05, fz0, 1.6 * fl, 1.6 * fl, 1.6 * fl, t, 0xffb03a);
          const sitters = [{ name: inn.keeper, race: inn.keeperRace, look: keeperLook }, ...inn.staff.slice(0, 2).map((x, k) => ({ name: x.name, race: x.race, look: this.raceLook(x.race, TUNIC[k]) }))];
          sitters.forEach((q, k) => { const aa = k * 2.1 + inn.id; const px = fx0 + Math.cos(aa) * 0.22, pz = fz0 + Math.sin(aa) * 0.22; this.fig(px, pz, Math.atan2(fx0 - px, fz0 - pz), t, inn.id * 7 + 20 + k, q.look, 'idle', 0.85); reg(px, pz, q.name, q.race, k ? 'yardımcı' : 'hancı', 'ateş başında dinleniyor'); });
        }
      }
      if (inn.stage !== 'open') continue;
      if (day && !b) {
        // hancı avluda dolaşır
        const aa = t * 0.12 + inn.id, px = ix + Math.cos(aa) * 0.5, pz = iz + Math.sin(aa) * 0.5;
        this.fig(px, pz, aa + Math.PI / 2, t, inn.id * 7, keeperLook, 'walk', 1);
        reg(px, pz, inn.keeper, inn.keeperRace, 'hancı', 'avluda misafirlerle ilgileniyor');
      }
      // personel: garson masaya taşır, seyis ahırda, aşçı kapıda, bekçi kapıda nöbette
      if (day) inn.staff.forEach((x, k) => {
        if (b && k < 3) return;
        let px: number, pz: number, face: number, pose: Pose = 'idle', look = this.raceLook(x.race, 0xe8dcc0, { legs: 0x4a3a2a });
        let mood = '';
        if (x.role === 'garson' || x.role === 'cirak') {
          const loop = (t / 7 + hash(inn.id, k + 60)) % 1, go = loop < 0.5, q = go ? loop / 0.5 : (loop - 0.5) / 0.5;
          const [ax, az] = at2(0.24, (k % 2 ? 0.1 : -0.1)), [bx, bz] = at2(0.36, -0.08);
          px = go ? ax + (bx - ax) * q : bx + (ax - bx) * q; pz = go ? az + (bz - az) * q : bz + (az - bz) * q;
          face = go ? Math.atan2(bx - ax, bz - az) : Math.atan2(ax - bx, az - bz); pose = go ? 'carry' : 'walk';
          look = this.raceLook(x.race, 0xe8dcc0, { legs: 0x4a3a2a, sack: go ? 0xc8a060 : undefined });
          mood = go ? 'masaya yemek ve bira taşıyor' : 'mutfağa dönüyor';
        } else if (x.role === 'seyis') { [px, pz] = at2(0.25 + (k % 3) * 0.14, 0.5); face = ia + Math.PI / 2; pose = 'work'; look = this.raceLook(x.race, 0x7a6a4a, { tool: 'basket' }); mood = 'binekleri tımar ediyor'; }
        else if (x.role === 'asci') { [px, pz] = at2(-0.02, -0.34); face = ia + Math.PI; pose = 'work'; look = this.raceLook(x.race, 0xf2ecdc, { legs: 0x4a3a2a, tool: 'basket' }); mood = 'arka kapıda sebze ayıklıyor'; }
        else { [px, pz] = at2(inn.level >= 3 ? 1.08 : 0.62, 0.36); face = ia; pose = 'guard'; look = this.raceLook(x.race, 0x5a5f66, { hat: 'helm', tool: 'spear', legs: 0x3a3a3a }); mood = 'kapıda nöbet tutuyor'; }
        this.fig(px, pz, face, t, inn.id * 7 + 30 + k, look, pose, 0.88);
        reg(px, pz, x.name, x.race, x.role === 'cirak' ? 'çırak' : x.role === 'asci' ? 'aşçı' : x.role, mood);
      });
      // misafirler: gündüz masada ve avluda, gece yalnız ozan ve bir iki ayyaş fenerin dibinde
      const gs = inn.guests.filter((g) => g.kind !== 'hero' && g.kind !== 'caravan');
      const shown = day ? gs.slice(0, 6) : gs.filter((g) => g.kind === 'bard' || g.kind === 'soldier').slice(0, 2);
      shown.forEach((g, k) => {
        const seat = k < 4;
        const [px, pz] = seat ? at2(0.36 + (k % 2 ? 0.1 : -0.1), -0.08 + (k < 2 ? -0.07 : 0.07)) : at2(0.5 + (k - 4) * 0.1, 0.12 + Math.sin(t * 0.2 + k) * 0.05);
        const face = seat ? Math.atan2(...(at2(0.36, -0.08).map((v, i) => v - (i ? pz : px)) as [number, number])) : ia;
        const look = this.guestLook(g.kind, g.race, g.civ >= 0 && w.civs[g.civ] ? new THREE.Color(w.civs[g.civ].color).getHex() : 0x8a8f99, k);
        const pose: Pose = g.kind === 'bard' ? 'cheer' : seat ? (Math.sin(t * 0.7 + k * 2 + g.id) > 0.8 ? 'cheer' : 'idle') : 'idle';
        this.fig(px, pz, face, t, g.id, look, pose, 0.85);
        reg(px, pz, g.name, g.race, `${GUEST[g.kind].name.toLocaleLowerCase('tr')} · ${inn.name} Hanı misafiri`, `${ek(g.fromName, 'dan')} geldi, ${g.toName} yolunda · ${g.why}`);
      });
    }
    // kuş sürüleri
    if (this.night < 0.6) for (let f = 0; f < 4; f++) {
      const cx = this.center.x + (hash(f, 31) - 0.5) * 90, cz = this.center.z + (hash(f, 32) - 0.5) * 50;
      const R0 = 6 + hash(f, 33) * 10, sp = (0.06 + hash(f, 34) * 0.05) * (f % 2 ? 1 : -1);
      for (let k = 0; k < 7; k++) {
        const a = t * sp + k * 0.09 * Math.sign(sp) + f;
        const r0 = R0 + Math.sin(k * 1.7) * 0.8;
        const bx = cx + Math.cos(a) * r0, bz = cz + Math.sin(a) * r0;
        // dağların üstünden geçsinler: zemine göre yükseklik
        const by = Math.max(6.5 + hash(f, 35) * 3, this.gy(bx, bz) + 2.6) + Math.sin(t * 0.8 + k) * 0.3;
        const flap = Math.sin(t * 11 + k * 1.3);
        L.add('bird', bx, by, bz, 1.2, 1 + flap * 1.6, 1.2, a + (sp > 0 ? 0 : Math.PI), 0x2a2a2a);
      }
    }
    this.wildlife(L, t);
    this.nearGrass(L);
    this.mist(t);
    // kamp ateşleri ve kamptaki canavarlar
    for (const cp of w.camps) {
      if (!cp.alive) continue;
      const [x, z] = this.pos(cp.tile);
      const y = this.gy(x, z);
      if (cp.kind !== 'bugbear') {
        const fl = 0.8 + Math.sin(t * 13 + cp.id) * 0.15 + Math.sin(t * 7.3 + cp.id * 2) * 0.1;
        L.add('lantern', x, y + 0.06, z, 1.4 * fl, 1.8 * fl, 1.4 * fl, t * 2, 0xff7a2a);
        for (let k = 0; k < 3; k++) { const q = (t * 0.4 + k / 3 + cp.id * 0.17) % 1; const ps = puffS(q, 0.6, 1.8); this.smoke.add(x + q * 0.2, y + 0.2 + q * 1.1, z + Math.sin(q * 6 + k) * 0.06, ps * 0.24, 0xb0aeaa, 0.55 * (1 - q * 0.6)); }
      }
      const n = Math.min(this.detail ? 6 : 2, Math.ceil(cp.count / 2));
      for (let k = 0; k < n; k++) {
        const sit = k % 3 === 0;
        const a = sit ? hash(cp.id, k) * 6.28 : t * 0.3 * (k % 2 ? 1 : -1) + hash(cp.id, k) * 6.28;
        const r = sit ? 0.32 : 0.55 + hash(cp.id, k + 5) * 0.3;
        const px = x + Math.cos(a) * r, pz = z + Math.sin(a) * r;
        const face = sit ? Math.atan2(x - px, z - pz) : Math.atan2(-Math.sin(a) * (k % 2 ? 1 : -1), Math.cos(a) * (k % 2 ? 1 : -1));
        this.fig(px, pz, face, t, cp.id * 11 + k, this.monsterLook(cp.kind, k), sit ? (Math.sin(t * 0.8 + k) > 0.7 ? 'cheer' : 'idle') : 'walk', cp.kind === 'bugbear' ? 1 : 0.9);
      }
      if (cp.boss && cp.kind !== 'bugbear') this.fig(x + 0.35, z - 0.35, 2.4, t, cp.id, cp.kind === 'pirate' ? { ...this.monsterLook('pirate', 1), body: 0x1e1e22, cape: 0xa0281e, hatColor: 0x141418, tool: 'sword' } : { ...this.monsterLook(cp.kind, 0), body: 0x2a1a14, cape: 0x6a1a14, tool: cp.kind === 'goblin' ? 'axe' : 'sword' }, 'idle', cp.kind === 'pirate' ? 1.05 : 1.3);
    }
    // işçiler
    const JOB: Record<string, { tool: Tool; pose: Pose; hat?: Look['hat']; orb?: number }> = {
      farm: { tool: 'hoe', pose: 'work', hat: 'straw' }, lumber: { tool: 'axe', pose: 'work' }, hunt: { tool: 'spear', pose: 'walk', hat: 'hood' },
      dock: { tool: 'rod', pose: 'fish', hat: 'straw' }, quarry: { tool: 'pick', pose: 'work' }, claypit: { tool: 'shovel', pose: 'work' },
      mine: { tool: 'pick', pose: 'work', hat: 'hood' }, pasture: { tool: 'staff', pose: 'walk', orb: 0x7a5533 }, herbalist: { tool: 'basket', pose: 'work', hat: 'hood' },
      crystal: { tool: 'staff', pose: 'cast', orb: 0xb08aff }, mithril: { tool: 'pick', pose: 'work', hat: 'helm' }, grove: { tool: 'staff', pose: 'cast', hat: 'leaf', orb: 0x9fe6a0 },
    };
    for (let i = 0; i < w.tiles.length; i++) {
      const tl = w.tiles[i];
      if (!tl.ext) continue;
      const [x, z] = this.pos(i);
      const st = s.settlement(tl.ext.settlement);
      const race = st ? w.civs[st.civ].race : 'human';
      if (tl.ext.kind === 'pasture') for (let k = 0; k < 1 + tl.ext.level; k++) {
        const a = hash(i, k + 40) * 6.28 + Math.sin(t * 0.2 + k) * 0.4;
        const hx = x + 0.1 + Math.cos(a) * 0.35, hz = z + 0.1 + Math.sin(a) * 0.35;
        drawHorse(this.add, hx, this.gy(hx, hz), hz, a + Math.PI / 2 + Math.sin(t * 0.5 + k), t, i + k, [0x7a4f2d, 0x3a2a1e, 0xd8cfb8][k % 3], false);
      }
      if (tl.ext.burned || tl.ext.depleted) continue;
      const near = (camP.x - x) ** 2 + (camP.z - z) ** 2 < 1800;
      const day = !(this.cyc < 0.02 || this.cyc > 0.5);
      // koyunlar: meralar ve büyük çiftlikler
      if (near && (tl.ext.kind === 'pasture' || (tl.ext.kind === 'farm' && tl.ext.level >= 2))) for (let k = 0; k < 2 + (tl.ext.level > 2 ? 1 : 0); k++) {
        const a = hash(i, k + 60) * 6.28 + Math.sin(t * 0.08 + k * 2) * 0.5;
        // merada ağılın içinde (ağıl merkezi x+0.1, z+0.1, yarıçap 0.62), çiftlikte tarla kenarında
        const pas = tl.ext.kind === 'pasture';
        const sx0 = pas ? x + 0.1 + Math.cos(a + 1.2) * 0.22 : x - 0.25 + Math.cos(a) * 0.3, sz0 = pas ? z + 0.1 + Math.sin(a + 1.2) * 0.2 : z + 0.3 + Math.sin(a) * 0.25, sy = this.gy(sx0, sz0);
        const graze = Math.sin(t * 0.7 + k * 3 + i) > 0;
        // koyun: yün topakları, kara baş ve bacaklar
        L.add('wool', sx0, sy + 0.01, sz0, 1, 1, 1, a, jitter(0xf2efe6, hash(i, k + 70), 0.06));
        L.add('box', sx0 + Math.sin(a) * 0.1, sy + (graze ? 0.04 : 0.11), sz0 + Math.cos(a) * 0.1, 0.045, 0.05, 0.065, a, 0x2a2622);
        for (const [lx, lz] of [[-0.03, 0.04], [0.03, 0.04], [-0.03, -0.04], [0.03, -0.04]]) L.add('box', sx0 + Math.cos(a) * lx + Math.sin(a) * lz, sy - 0.01, sz0 - Math.sin(a) * lx + Math.cos(a) * lz, 0.016, 0.06, 0.016, a, 0x2a2622);
      }
      // balıkçı tekneleri: gündüz açıkta, gece iskelede
      if (near && tl.ext.kind === 'dock') {
        const wn = s.g.within(i, 2).filter((q) => w.tiles[q].terrain === 'water');
        for (let k = 0; k < Math.min(wn.length, tl.ext.level); k++) {
          const q = wn[Math.floor(hash(i, k + 70) * wn.length)];
          const [wx, wz] = this.pos(q);
          const a = t * 0.08 * (k % 2 ? 1 : -1) + hash(i, k) * 6.28;
          const wn1 = s.g.neighbors(i).find((n) => w.tiles[n].terrain === 'water');
          const [ax, az] = wn1 !== undefined ? this.pos(wn1) : [wx, wz];
          const pa = Math.atan2(ax - x, az - z), side = k % 2 ? 1 : -1;
          // gece: iskelenin yanında bağlı
          const bx = day ? wx + Math.cos(a) * 0.45 : x + Math.sin(pa) * (0.8 + (k >> 1) * 0.35) + Math.cos(pa) * 0.36 * side;
          const bz = day ? wz + Math.sin(a) * 0.45 : z + Math.cos(pa) * (0.8 + (k >> 1) * 0.35) - Math.sin(pa) * 0.36 * side;
          const face = day ? a + (k % 2 ? Math.PI : 0) : pa;
          const by = WATER_Y + 0.02 + Math.sin(t * 1.6 + k + i) * 0.015;
          L.add('hull', bx, by - 0.05, bz, 0.2, 0.12, 0.2, face, 0x6a4526, Math.sin(t * 1.3 + k) * 0.05);
          if (day) { this.fig(bx, bz, face, t, i * 11 + k, this.raceLook(race, TUNIC[k % TUNIC.length], { tool: 'rod', hat: 'straw' }), 'fish', 0.75); L.add('post', bx, by + 0.06, bz, 0.8, 0.4, 0.8, 0, 0x5a4330); L.add('flag', bx, by + 0.38, bz, 0.9, 1.2, 1, face + Math.PI / 2, 0xe9dcc0); }
        }
      }
      if (!tl.ext.workers || !day) continue; // gece işçiler evde
      // taşıyıcı: ürünü kasabaya götürür, boş döner
      if (near && st && tl.ext.workers >= 2 && s.g.dist(i, st.tile) >= 1) {
        const [cx, cz] = this.pos(st.tile);
        const loop = (t / 26 + hash(i, 90)) % 1;
        const go = loop < 0.5, q = go ? loop / 0.5 : (loop - 0.5) / 0.5;
        const ax = go ? x : cx, az = go ? z : cz, bx = go ? cx : x, bz = go ? cz : z;
        const px = ax + (bx - ax) * q, pz = az + (bz - az) * q;
        const cargo = tl.ext.kind === 'lumber' ? 0x9a6a3a : tl.ext.kind === 'quarry' || tl.ext.kind === 'mine' ? 0x8d877e : tl.ext.kind === 'dock' ? 0x9fb4c0 : 0xd8c48a;
        this.fig(px, pz, Math.atan2(bx - ax, bz - az), t, i * 13, this.raceLook(race, TUNIC[(i + 3) % TUNIC.length], go ? { sack: cargo } : {}), go ? 'carry' : 'walk', 0.85);
      }
      const job = JOB[tl.ext.kind];
      const n = Math.min(this.detail ? 3 : 1, tl.ext.workers);
      let waterFace = 0;
      if (tl.ext.kind === 'dock') { const wn = s.g.neighbors(i).find((q) => w.tiles[q].terrain === 'water'); if (wn !== undefined) { const [wx, wz] = this.pos(wn); waterFace = Math.atan2(wx - x, wz - z); } }
      for (let k = 0; k < n; k++) {
        const look = this.raceLook(race, TUNIC[(i + k) % TUNIC.length], { tool: job.tool, orb: job.orb });
        if (job.hat) look.hat = job.hat;
        if (job.pose === 'walk') {
          const a = t * 0.35 * (k % 2 ? 1 : -1) + hash(i, k) * 6.28;
          const px = x + Math.cos(a) * 0.5, pz = z + Math.sin(a) * 0.5;
          this.fig(px, pz, a + (k % 2 ? 0 : Math.PI), t, i * 3 + k, look, 'walk', 0.85);
        } else {
          const ox = -0.3 + k * 0.3, oz = -0.25 + hash(i, k) * 0.3;
          const dock = tl.ext.kind === 'dock';
          const face = dock ? waterFace : hash(i, k + 7) * 6.28;
          const px = dock ? x + Math.sin(waterFace) * 0.45 + ox * 0.5 : x + ox, pz = dock ? z + Math.cos(waterFace) * 0.45 + oz * 0.3 : z + oz;
          this.fig(px, pz, face, t, i * 3 + k, look, job.pose, 0.85);
        }
      }
    }
    // yol trafiği: köylüler, yük arabaları, yolcular (gündüz)
    this.walkPos = [];
    if (this.night < 0.6) for (const wk of this.walks) {
      const [sx0, sz0] = wk.pts[0];
      if ((camP.x - sx0) ** 2 + (camP.z - sz0) ** 2 > 1600) continue;
      const speed = wk.kind === 1 ? 0.28 : 0.36;
      const u = ((t * speed / wk.len + hash(wk.id, 1)) % 2 + 2) % 2;
      const back = u > 1;
      const d = (back ? 2 - u : u) * wk.len;
      let j = 1; while (j < wk.cum.length - 1 && wk.cum[j] < d) j++;
      const [ax, az] = wk.pts[j - 1], [bx, bz] = wk.pts[j];
      const q = (d - wk.cum[j - 1]) / Math.max(0.001, wk.cum[j] - wk.cum[j - 1]);
      let face = Math.atan2(bx - ax, bz - az); if (back) face += Math.PI;
      // yolun sağından yürü
      const rx = Math.cos(face) * 0.1, rz = -Math.sin(face) * 0.1;
      const x = ax + (bx - ax) * q - rx, z = az + (bz - az) * q - rz;
      if (this.groundY(x, z) < WATER_Y) continue;
      this.walkPos.push([x, z]);
      const fx = Math.sin(face), fz = Math.cos(face);
      const civ = w.civs[wk.civ], cc = civ ? new THREE.Color(civ.color).getHex() : 0x8a6a45;
      const tunic = TUNIC[wk.id % TUNIC.length];
      if (wk.kind === 1) {
        // öküz arabası
        const y = this.gy(x, z), bob = Math.abs(Math.sin(t * 6 + wk.id)) * 0.015;
        L.add('cart', x, y + bob, z, 0.9, 0.9, 0.9, face + Math.PI / 2, 0x8a6a45);
        L.add('wheel', x, y, z, 0.9, 0.9, 0.9, face + Math.PI / 2, 0x3a2a1a, 0, t * 5);
        L.add('box', x, y + 0.22 + bob, z, 0.22, 0.08, 0.14, face + Math.PI / 2, back ? 0xd8c48a : 0x9a6a3a);
        const hx = x + fx * 0.38, hz = z + fz * 0.38;
        drawHorse(this.add, hx, this.gy(hx, hz), hz, face, t, wk.id, [0x6a5a4a, 0x8a6a45, 0x3a2e24][wk.id % 3], true);
        this.fig(x + fx * 0.15 - rx * 2, z + fz * 0.15 - rz * 2, face, t, wk.id, this.raceLook(wk.race, tunic, { hat: 'straw' }), 'walk', 0.85);
      } else if (wk.kind === 2) {
        // iki yolcu
        this.fig(x, z, face, t, wk.id, this.raceLook(wk.race, tunic, { tool: 'staff', orb: 0x6b4a2b, hat: 'hood', hatColor: 0x6a5a4a }), 'walk', 0.85);
        this.fig(x - fx * 0.22 + rx, z - fz * 0.22 + rz, face, t, wk.id + 7, this.raceLook(wk.race, TUNIC[(wk.id + 2) % TUNIC.length], { sack: 0xc8b48a }), 'carry', 0.8);
      } else if (wk.kind === 3) {
        // haberci/asker
        this.fig(x, z, face, t, wk.id, this.raceLook(wk.race, cc, { hat: 'helm', tool: 'spear' }), 'walk', 0.9);
      } else {
        // sırtında çuvalla köylü
        this.fig(x, z, face, t, wk.id, this.raceLook(wk.race, tunic, { sack: back ? 0xc8b48a : 0x9a7a4a, hat: 'straw' }), 'carry', 0.85);
      }
    }
    // yanardağ adaları: zirveden kül dumanı, gece kızıl ışıltı
    for (const isl of w.isles ?? []) {
      if (isl.kind !== 'volkan' || isl.peak === undefined) continue;
      const [vx, vz] = this.pos(isl.peak);
      if ((camP.x - vx) ** 2 + (camP.z - vz) ** 2 > 110 * 110) continue;
      const vy = this.groundY(vx, vz);
      for (let q = 0; q < 7; q++) { const u = (t * 0.07 + q / 7 + isl.id * 0.13) % 1; this.smoke.add(vx + u * u * 1.6, vy + 0.2 + u * 3.2, vz + Math.sin(u * 5 + q) * 0.3, puffS(u, 1.2, 4.2) * 0.3, u < 0.3 ? 0x3a3634 : 0x8a8682, 0.6 * (1 - u * 0.7)); }
      if (this.night > 0.3) L.add('fire', vx, vy - 0.05, vz, 2.2, 0.8 + Math.sin(t * 2 + isl.id) * 0.15, 2.2, t * 0.3, 0xff5a1a);
    }
    // limanda bağlı gemiler: boştaki kogalar ve kadırgalar
    for (const [sid, bs] of this.berths) {
      const st = s.settlement(sid);
      if (!st || !st.alive) continue;
      const [bx0, bz0] = [bs[0].x, bs[0].z];
      if ((camP.x - bx0) ** 2 + (camP.z - bz0) ** 2 > 3600) continue;
      const c = w.civs[st.civ], cc = new THREE.Color(c.color).getHex();
      const big = s.has(c, 'shipbuilding') ? 'cog' : 'boat';
      const nh = Math.min(bs.length, freeHulls(s, st)), ng = Math.min(bs.length - nh, freeGalleys(s, st));
      for (let k = 0; k < nh + ng; k++) { const b = bs[k]; if (b) this.ship(b.x, b.z, b.face, t, sid * 11 + k, k < nh ? big : 'galley', cc, c.race, { furled: true }); }
    }
    // hareketli birimler
    for (const a of w.agents) {
      if (a.dead) continue;
      const [x, , z, face] = this.agentWorld(a);
      const civ = a.civ >= 0 ? w.civs[a.civ] : undefined;
      const cc = civ ? new THREE.Color(civ.color).getHex() : 0xaaaaaa;
      const race = civ?.race ?? 'human';
      const sx = Math.cos(face), sz = -Math.sin(face), fx = Math.sin(face), fz = Math.cos(face);
      // denizde: gemi
      const i0 = Math.min(a.step, a.path.length - 1), j0 = Math.min(i0 + 1, a.path.length - 1);
      if (civ && (w.tiles[a.path[i0]].sea || w.tiles[a.path[j0]].sea) && this.groundY(x, z) < WATER_Y - 0.01) { this.agentShip(a, x, z, face, t, cc, race); continue; }
      if (a.monster === 'pirate' && (w.tiles[a.path[i0]].sea || w.tiles[a.path[j0]].sea) && this.groundY(x, z) < WATER_Y - 0.01) { this.ship(x, z, face, t, a.id, 'pirate', 0x141418, 'human', { moving: a.step < a.path.length - 1, load: 'troops' }); continue; }
      // karaya çıkmış seferin kogaları kıyıda demirli bekler
      if (a.kind === 'army' && a.hull !== undefined && a.landing !== undefined && !a.returning) {
        const k = a.path.indexOf(a.landing);
        if (k > 0 && w.tiles[a.path[k - 1]].sea) {
          const [ax, az] = this.pos(a.path[k - 1]), [lx, lz] = this.pos(a.landing);
          const f2 = Math.atan2(lx - ax, lz - az) + 0.6;
          this.ship(ax, az, f2, t, a.id * 3, civ && s.has(civ, 'shipbuilding') ? 'cog' : 'boat', cc, race, { furled: true });
          for (let q = 0; q < (a.galleys ?? 0); q++) this.ship(ax + Math.cos(q * 2.1) * 0.9, az + Math.sin(q * 2.1) * 0.9, f2 + q, t, a.id * 5 + q, 'galley', cc, race, { furled: true });
        }
      }
      switch (a.kind) {
        case 'caravan': {
          const y = this.gy(x, z);
          const bob = Math.abs(Math.sin(t * 8 + a.id)) * 0.02;
          L.add('cart', x, y + bob, z, 1, 1, 1, face, 0x9b7650);
          L.add('wheel', x, y, z, 1, 1, 1, face + Math.PI / 2, 0x3a2a1a, 0, t * 6);
          L.add('box', x, y + 0.26 + bob, z, 0.26, 0.1, 0.18, face, cc);
          const hx = x + fx * 0.42, hz = z + fz * 0.42;
          drawHorse(this.add, hx, this.gy(hx, hz), hz, face, t, a.id, 0x7a4f2d, true);
          this.fig(x + sx * 0.25 + fx * 0.3, z + sz * 0.25 + fz * 0.3, face, t, a.id, this.raceLook(race, 0x8a6a45, { hat: 'hood', hatColor: cc, tool: 'staff', orb: 0x6b4a2b }), 'walk', 0.85);
          break;
        }
        case 'army': {
          const n = Math.min(9, 2 + Math.ceil((a.troops ?? 0) / 3));
          const plunder = a.purpose === 'plunder';
          for (let k = 0; k < n; k++) {
            const row = Math.floor(k / 3), colI = (k % 3) - 1;
            const px = x + sx * colI * 0.26 - fx * row * 0.28, pz = z + sz * colI * 0.26 - fz * row * 0.28;
            const look = plunder ? this.raceLook(race, 0x7a2a22, { hat: 'horns', tool: k % 2 ? 'axe' : 'club' }) : this.raceLook(race, civ?.cls === 'warlock' ? 0x2e2833 : 0x5a5f66, { hat: civ?.cls === 'warlock' ? 'hood' : 'helm', hatColor: 0x1e1a22, tool: civ?.cls === 'warlock' ? 'sword' : 'spear', shield: cc, legs: 0x2a2a2a });
            this.fig(px, pz, face, t, a.id * 13 + k, look, 'walk', 0.95);
          }
          (a.heroes ?? []).forEach((hid, k) => this.heroFig(x + fx * 0.4 + sx * (k - 0.5) * 0.3, z + fz * 0.4 + sz * (k - 0.5) * 0.3, t, hid, face));
          const y = this.gy(x, z);
          L.add('post', x + fx * 0.1, y, z + fz * 0.1, 1, 1.1, 1, 0, 0x5a4330);
          L.add('flag', x + fx * 0.1, y + 1.02, z + fz * 0.1, 1, 1, 1, face + Math.PI / 2 + Math.sin(t * 3) * 0.2, cc);
          break;
        }
        case 'raid': {
          const kind = a.monster ?? 'goblin';
          const n = Math.min(8, Math.max(1, Math.ceil((a.troops ?? 1) / 2)));
          const sc = kind === 'bugbear' ? 1 : kind === 'hobgoblin' ? 0.95 : 0.9;
          for (let k = 0; k < n; k++) {
            const ox = (hash(a.id, k) - 0.5) * 0.8, oz = (hash(a.id, k + 20) - 0.5) * 0.8;
            this.fig(x + ox, z + oz, face, t * 1.25, a.id * 17 + k, this.monsterLook(kind, k), 'walk', sc);
          }
          if (a.boss) this.fig(x + fx * 0.3, z + fz * 0.3, face, t * 1.25, a.id, { ...this.monsterLook(kind, 0), body: 0x2a1a14, cape: 0x6a1a14, tool: kind === 'goblin' ? 'axe' : 'sword' }, 'walk', 1.35);
          break;
        }
        case 'hero': case 'party': {
          const hs = a.heroes ?? [];
          hs.forEach((hid, k) => this.heroFig(x + (k - (hs.length - 1) / 2) * 0.3 * sx, z + (k - (hs.length - 1) / 2) * 0.3 * sz, t, hid, face));
          break;
        }
        case 'settlers': {
          for (let k = 0; k < 4; k++) this.fig(x + (hash(a.id, k) - 0.5) * 0.6 - fx * 0.2, z + (hash(a.id, k + 5) - 0.5) * 0.6 - fz * 0.2, face, t, a.id * 5 + k, this.raceLook(race, TUNIC[k], k % 2 ? { sack: 0xc8b48a } : { tool: 'staff', orb: 0x6b4a2b }), k % 2 ? 'carry' : 'walk', 0.85);
          const cx = x + fx * 0.25, cz = z + fz * 0.25, y = this.gy(cx, cz);
          L.add('cart', cx, y, cz, 0.85, 0.85, 0.85, face, 0x9b7650);
          L.add('wheel', cx, y, cz, 0.85, 0.85, 0.85, face + Math.PI / 2, 0x3a2a1a, 0, t * 6);
          const hx = cx + fx * 0.4, hz = cz + fz * 0.4;
          drawHorse(this.add, hx, this.gy(hx, hz), hz, face, t, a.id, 0x5a3a22, true);
          break;
        }
        case 'scout':
          this.fig(x, z, face, t * 1.4, a.id, this.raceLook(race, 0x4a5a3a, { hat: 'hood', hatColor: cc, tool: 'dagger', cape: cc }), 'walk', 0.9);
          break;
        case 'keeper': case 'supply': {
          // hancı kafilesi: kereste ve fıçı yüklü öküz arabası, yanında hancı, arkada yardımcılar; erzak arabası: çuval ve fıçı
          const inn = w.inns.find((q) => q.id === a.inn);
          const y = this.gy(x, z), bob = Math.abs(Math.sin(t * 7 + a.id)) * 0.015;
          L.add('cart', x, y + bob, z, 1.05, 1.05, 1.05, face, 0x8a6a45);
          L.add('wheel', x, y, z, 1.05, 1.05, 1.05, face + Math.PI / 2, 0x3a2a1a, 0, t * 5);
          if (a.kind === 'keeper') {
            for (let k = 0; k < 3; k++) L.add('log', x + sx * (k - 1) * 0.07, y + 0.29 + bob, z + sz * (k - 1) * 0.07, 1.05, 1, 1, face + Math.PI / 2, 0x9a6a3a);
            L.add('tower', x - fx * 0.12, y + 0.27 + bob, z - fz * 0.12, 0.06, 0.09, 0.06, 0, 0x6a4a2a);
          } else {
            for (let k = 0; k < 3; k++) L.add('box', x + sx * (k - 1) * 0.08 - fx * 0.03, y + 0.28 + bob, z + sz * (k - 1) * 0.08 - fz * 0.03, 0.08, 0.07, 0.11, face, k === 1 ? 0x6a4a2a : 0xd8c48a);
          }
          const hx = x + fx * 0.44, hz = z + fz * 0.44;
          drawHorse(this.add, hx, this.gy(hx, hz), hz, face, t, a.id, a.kind === 'keeper' ? 0x8a7a66 : 0x6a5a4a, true);
          if (a.kind === 'keeper' && inn) {
            this.fig(x + sx * 0.24 + fx * 0.28, z + sz * 0.24 + fz * 0.28, face, t, a.id, this.raceLook(inn.keeperRace, 0xe8e0c8, { legs: 0x4a3a2a, tool: 'staff', orb: 0x6b4a2b }), 'walk', 0.92);
            inn.staff.slice(0, 2).forEach((q, k) => this.fig(x - fx * (0.36 + k * 0.2) + sx * (k ? 0.12 : -0.12), z - fz * (0.36 + k * 0.2) + sz * (k ? 0.12 : -0.12), face, t, a.id + 3 + k, this.raceLook(q.race, TUNIC[k + 2], { sack: 0xc8b48a }), 'carry', 0.85));
          } else this.fig(x + sx * 0.24 + fx * 0.28, z + sz * 0.24 + fz * 0.28, face, t, a.id, this.raceLook(race, 0x8a6a45, { hat: 'hood', hatColor: cc, tool: 'staff', orb: 0x6b4a2b }), 'walk', 0.85);
          break;
        }
        case 'traveler': {
          const g = a.guest;
          if (!g) break;
          const n = Math.min(g.kind === 'noble' ? 3 : 4, g.n);
          for (let k = 0; k < n; k++) {
            const kid = g.kind === 'refugee' && k >= 2;
            const px = x - fx * k * 0.2 + sx * (k % 2 ? 0.1 : -0.1) * (k ? 1 : 0), pz = z - fz * k * 0.2 + sz * (k % 2 ? 0.1 : -0.1) * (k ? 1 : 0);
            this.fig(px, pz, face, t, a.id * 7 + k, this.guestLook(g.kind, g.race, cc, k), g.kind === 'refugee' || g.kind === 'merchant' ? 'carry' : 'walk', kid ? 0.62 : 0.85);
          }
          if (g.kind === 'merchant' || g.kind === 'noble') { const hx = x - fx * (n * 0.2 + 0.12), hz = z - fz * (n * 0.2 + 0.12); drawHorse(this.add, hx, this.gy(hx, hz), hz, face, t, a.id, g.kind === 'noble' ? 0xe8e0d0 : 0x7a5a3a, true); }
          break;
        }
      }
    }
    // savaş anları: iki hat karşılıklı dövüşür, kaybedenler savrulur
    for (let k = this.battles.length - 1; k >= 0; k--) {
      const b = this.battles[k];
      const p = (now - b.t0) / b.dur;
      if (p > 1) { this.battles.splice(k, 1); continue; }
      const [x, z] = this.pos(b.tile);
      const ax = hash(b.t0, 1) * 6.28;
      const dx = Math.cos(ax), dz = Math.sin(ax);
      if (b.naval) {
        // deniz savaşı: iki kadırga hattı yaklaşır, oklar ve ateş; kaybedenler yan yatıp batar
        const close = 1.6 - Math.min(1, p * 2.2) * 0.95;
        for (let i = 0; i < b.n * 2; i++) {
          const sideA = i < b.n, idx = sideA ? i : i - b.n, sgn = sideA ? -1 : 1;
          const lose = (sideA ? b.winner === 'B' : b.winner === 'A') ? 0.65 : 0.2;
          const dies = hash(b.t0 + i, 3) < lose, dieAt = 0.35 + hash(b.t0 + i, 4) * 0.4;
          const lane = (idx - (b.n - 1) / 2) * 1.1;
          const sx0 = x + dx * sgn * close - dz * lane, sz0 = z + dz * sgn * close + dx * lane;
          const face = Math.atan2(-dx * sgn, -dz * sgn) + Math.sin(t * 0.7 + i) * 0.15 + (sideA ? 0.35 : -0.35);
          const sink = dies && p > dieAt ? Math.min(1.4, (p - dieAt) * 2.4) : 0;
          this.ship(sx0, sz0, face, t, b.t0 + i, !sideA && b.pirate ? 'pirate' : 'galley', sideA ? b.a : b.b, sideA ? b.raceA : b.raceB, { moving: p < 0.5, sink });
          if (sink > 0 && sink < 1.2) for (let m = 0; m < 2; m++) L.add('fire', sx0 + (hash(i, m) - 0.5) * 0.4, WATER_Y + 0.12, sz0 + (hash(i, m + 4) - 0.5) * 0.4, 1.2, 1 + Math.sin(t * 9 + m) * 0.3, 1.2, 0, 0xff7a1a);
          if (sink > 0 && sink < 1.4) for (let q = 0; q < 5; q++) { const u = (t * 0.4 + q / 5 + i * 0.13) % 1; this.smoke.add(sx0 + u * u * 0.4, WATER_Y + 0.2 + u * 1.4, sz0 + Math.sin(u * 6 + q) * 0.1, puffS(u, 0.8, 2.4) * 0.26, u < 0.4 ? 0x24201e : 0x4a4644, 0.85 * (1 - u * 0.6)); }
          // ok yağmuru: iki hat arasında uçuşan kıvılcımlar
          if (p > 0.15 && p < 0.8 && !dies) for (let m = 0; m < 2; m++) { const q = ((t * 1.3 + hash(i, m)) % 1); L.add('dot', sx0 - dx * sgn * close * 2 * q, WATER_Y + 0.4 + Math.sin(q * Math.PI) * 0.5, sz0 - dz * sgn * close * 2 * q, 0.5, 0.5, 0.5, 0, 0x3a2a1a); }
        }
        if (p < 0.12) L.add('puff', x, WATER_Y + 0.3 + p, z, 1 + p * 8, 0.6 + p * 4, 1 + p * 8, 0, 0xffe0a0);
        continue;
      }
      for (let i = 0; i < b.n * 2; i++) {
        const sideA = i < b.n;
        const idx = sideA ? i : i - b.n;
        const lose = (sideA ? b.winner === 'B' : b.winner === 'A') ? 0.7 : 0.25;
        const dies = hash(b.t0 + i, 3) < lose;
        const dieAt = 0.25 + hash(b.t0 + i, 4) * 0.55;
        const lane = (idx - (b.n - 1) / 2) * 0.26;
        const clash = 0.22 + Math.max(0, Math.sin(p * 26 + i * 1.3)) * 0.12;
        const sgn = sideA ? -1 : 1;
        const fx = x + dx * sgn * clash - dz * lane, fz = z + dz * sgn * clash + dx * lane;
        const face = Math.atan2(-dx * sgn, -dz * sgn);
        const col = sideA ? b.a : b.b;
        const look: Look = !sideA && b.mk ? this.monsterLook(b.mk, idx) : this.raceLook(sideA ? b.raceA : b.raceB, col, { hat: 'helm', tool: idx % 2 ? 'sword' : 'spear', shield: col });
        if (dies && p > dieAt) {
          const q = Math.min(1, (p - dieAt) * 5);
          const y = this.gy(fx, fz);
          if (q < 1) {
            const ox = fx + dx * sgn * q * 0.45, oz = fz + dz * sgn * q * 0.45;
            L.add('f_torso', ox, y + Math.sin(q * Math.PI) * 0.35, oz, 0.9, 0.9, 0.9, face, look.body, -q * 4, 0);
            L.add('f_head', ox, y + Math.sin(q * Math.PI) * 0.35 + 0.1, oz, 0.9, 0.9, 0.9, face, look.skin);
          } else if (p - dieAt < 0.4) {
            const g = (p - dieAt - 0.2) / 0.2;
            for (let m = 0; m < 3; m++) L.add('puff', fx + dx * sgn * 0.45 + (hash(i, m) - 0.5) * 0.3, y + 0.15 + g * 0.3, fz + dz * sgn * 0.45 + (hash(i, m + 3) - 0.5) * 0.3, 1 + g * 1.6, 1 + g * 1.6, 1 + g * 1.6, 0, 0xf2f2f2);
          }
        } else this.fig(fx, fz, face, t * 1.3, i + b.t0, look, p > 0.85 && !dies ? 'cheer' : 'fight', !sideA && b.mk === 'bugbear' ? 1.1 : 0.95);
      }
      if (p < 0.2) L.add('puff', x, this.gy(x, z) + 0.2 + p, z, 1 + p * 8, 0.6 + p * 4, 1 + p * 8, 0, 0xffe0a0);
    }
    L.end();
    this.smoke.end(this.camera, this.h * this.renderer.getPixelRatio(), 1 - this.night * 0.72);
  }

  // ---------------------------------------------------------------- olaylar ve etiketler
  onEvent(e: GameEvent, now: number) {
    if (e.tile === undefined || !this.sim) return;
    if (e.battle) {
      const b = this.sim.w.battles.find((x) => x.id === e.battle);
      if (!b) return;
      const civA = e.civ !== undefined ? this.sim.w.civs[e.civ] : undefined;
      const colA = civA ? new THREE.Color(civA.color).getHex() : 0x8a8f99;
      const colB = /goblin|Goblin/i.test(b.sideB) ? MONSTER_COL.goblin : /Hobgoblin/i.test(b.sideB) ? MONSTER_COL.hobgoblin : /Bugbear/i.test(b.sideB) ? MONSTER_COL.bugbear : 0x7a2a22;
      const mk: CampKind | undefined = /Hobgoblin/i.test(b.sideB) ? 'hobgoblin' : /Bugbear/i.test(b.sideB) ? 'bugbear' : /goblin/i.test(b.sideB) ? 'goblin' : undefined;
      const civB = this.sim.w.civs.find((c) => b.sideB.includes(c.name));
      if (b.naval) {
        const W = this.sim.w;
        const pirate = /orsan/.test(b.title) || W.camps.some((c) => c.kind === 'pirate' && b.sideB === c.name);
        const portA = W.settlements.find((x) => b.sideA.startsWith(x.name + ' '));
        const att = W.civs.find((c) => b.sideA.startsWith(c.name)) ?? (portA ? W.civs[portA.civ] : undefined);
        const defSt = W.settlements.find((x) => b.title.startsWith(x.name + ' '));
        const dfc = pirate ? undefined : defSt ? W.civs[defSt.civ] : undefined;
        this.battles.push({ tile: e.tile, t0: now, a: att ? new THREE.Color(att.color).getHex() : 0x8a8f99, b: pirate ? 0x1c1c20 : dfc ? new THREE.Color(dfc.color).getHex() : 0x7a2a22, winner: b.winner, dur: 5600, n: Math.min(pirate ? 2 : 4, 2 + Math.floor((b.lossesA + b.lossesB) / 4)), raceA: att?.race ?? 'human', raceB: dfc?.race ?? 'human', naval: true, pirate });
      }
      else this.battles.push({ tile: e.tile, t0: now, a: colA, b: civB ? new THREE.Color(civB.color).getHex() : colB, winner: b.winner, dur: 3200, n: Math.min(6, 3 + Math.floor((b.lossesA + b.lossesB) / 3)), raceA: civA?.race ?? 'human', mk, raceB: mk ?? civB?.race ?? 'human' });
      if (this.battles.length > 8) this.battles.shift();
      const pick = b.rolls.filter((r) => r.d20 === 20 || r.d20 === 1).slice(0, 2);
      if (!pick.length && b.rolls.length) pick.push(b.rolls[0]);
      pick.forEach((r, k) => this.pop(e.tile!, r.d20 === 20 ? 'nat 20!' : r.d20 === 1 ? 'nat 1' : `d20: ${r.d20}`, r.d20 === 20 ? 'gold' : r.d20 === 1 ? 'red' : '', now + k * 380));
    } else if (e.kind === 'era') this.pop(e.tile, 'Yeni çağ!', 'era', now);
    else if (e.major && (e.kind === 'settle' || e.kind === 'war' || e.kind === 'lair')) this.pop(e.tile, e.kind === 'settle' ? 'Yeni yerleşim' : e.kind === 'war' ? 'Savaş!' : '☠', e.kind === 'war' ? 'red' : '', now);
  }
  private pop(tile: number, text: string, cls: string, t0: number) {
    const el = document.createElement('div');
    el.className = `dio-pop ${cls}`;
    el.textContent = text;
    this.overlay.appendChild(el);
    this.pops.push({ el, tile, t0, dur: 1900 });
    if (this.pops.length > 14) this.pops.shift()!.el.remove();
  }
  private v = new THREE.Vector3();
  private project(x: number, y: number, z: number): [number, number, boolean] {
    this.v.set(x, y, z).project(this.camera);
    return [(this.v.x * 0.5 + 0.5) * this.w, (-this.v.y * 0.5 + 0.5) * this.h, this.v.z < 1 && Math.abs(this.v.x) < 1.1 && Math.abs(this.v.y) < 1.1];
  }
  /** ortak saldırı hazırlığı: hedefte bekleyen gruplar */
  private musterLabels() {
    const w = this.sim.w;
    const out: { id: number; tile: number; text: string; cls: string; color: string; show: boolean; wy?: number; wx?: number; wz?: number }[] = [];
    const seen = new Set<string>();
    for (const a of w.agents) {
      if (a.dead || !a.muster) continue;
      const key = musterKey(a);
      if (!key || seen.has(key)) continue;
      seen.add(key);
      const same = w.agents.filter((b) => !b.dead && musterKey(b) === key);
      const ready = same.filter((b) => b.muster && atTarget(b)).length;
      const coming = same.length - ready;
      const tile = a.path[a.path.length - 1];
      const [x, z] = this.pos(tile);
      out.push({ id: 3e7 + (a.to ?? 0), tile, wx: x, wz: z, wy: this.height(tile) + 2.3, text: `⛺ Ortak saldırı · ${ready} hazır${coming ? ` · ${coming} yolda` : ''} · ≤${Math.max(0, a.muster.until - w.day)} gün`, cls: 'muster', color: '#e0bb55', show: true });
    }
    return out;
  }
  private updateOverlay(now: number) {
    const w = this.sim.w;
    const dist = this.camera.position.distanceTo(this.controls.target);
    const seen = new Set<number>();
    const boxes: [number, number, number, number][] = [];
    const warTargets = new Set<number>();
    for (const a of w.civs) for (const b of w.civs) { const r = a.id < b.id ? this.sim.rel(a.id, b.id) : null; if (r?.war) warTargets.add(r.war.target); }
    const items = [
      ...w.settlements.filter((s) => s.alive).sort((a, b) => this.sim.pop(b) - this.sim.pop(a)).map((s) => {
        const cap = this.sim.capital(w.civs[s.civ])?.id === s.id;
        const war = warTargets.has(s.id);
        const hungry = s.starving > 0;
        const sick = !!s.plague;
        const shanty = this.sim.homeless(s) >= 5;
        return { id: s.id, tile: s.tile, text: `${war ? '⚔ ' : ''}${cap ? '♛ ' : ''}${s.name}${s.civics.shipyard ? ' ⚓' : ''} · ${this.sim.pop(s)}${hungry ? ' · kıtlık' : ''}${sick ? ' · salgın' : ''}${shanty ? ' · 🏚' : ''}`, cls: `set${war ? ' war' : ''}${cap ? ' cap' : ''}${hungry ? ' hungry' : ''}${sick ? ' sick' : ''}`, color: w.civs[s.civ].color, show: true };
      }),
      ...w.camps.filter((c) => c.alive).map((c) => ({ id: -1e6 - c.id, tile: c.tile, text: c.kind === 'pirate' ? `☠ ${dist < 45 ? c.name + ' · ' : ''}${c.count}${c.boss ? '+' : ''}` : `${c.count}${c.boss ? '+' : ''}`, cls: `camp ${c.kind}`, color: '', show: dist < 110 })),
      ...(w.isles ?? []).filter((i) => i.size >= 5).map((i) => ({ id: -3e6 - i.id, tile: i.center, text: i.name, cls: 'isle', color: '', show: dist < 120 && (i.size >= 12 || dist < 60) })),
      ...w.inns.filter((inn) => inn.stage !== 'road').map((inn) => {
        const q = w.quests.filter((x) => x.open && x.inn === inn.id).length;
        const n = w.heroes.filter((h) => h.civ === -1 && h.state === 'tavern' && h.tavern === inn.id).length;
        let persons = 0; for (const g of inn.guests) persons += g.n;
        const rooms = [0, 6, 10, 16][inn.level] ?? 6;
        const pct = inn.build ? Math.round((inn.build.work / inn.build.need) * 100) : 0;
        const text = inn.stage === 'build' ? `🔨 ${inn.name} · %${pct}` : inn.alive ? `🍺 ${inn.name} · ${persons}/${rooms}${n ? ` · ★${n}` : ''}${q ? ` · 📜${q}` : ''}${inn.build ? ' · 🔨' : ''}` : `${inn.name} (harabe)`;
        return { id: -2e6 - inn.id, tile: inn.tile, text, cls: `inn${inn.alive ? '' : inn.stage === 'build' ? ' build' : ' ruin'}${this.selInn === inn.id ? ' sel' : ''}`, color: '#e8c24a', show: dist < 130, inn: inn.id };
      }),
      ...w.agents.filter((a) => !a.dead && a.kind === 'keeper').map((a) => {
        const inn = w.inns.find((q) => q.id === a.inn);
        const [ax, ay, az] = this.agentWorld(a);
        return { id: 1e7 + a.id, tile: a.path[Math.min(a.step, a.path.length - 1)], wx: ax, wy: ay + 0.85, wz: az, text: `🔨 Hancı ${inn?.keeper ?? ''} → ${inn?.name ?? ''}`, cls: 'inn keeper', color: '#e8c24a', show: dist < 95, inn: inn?.id };
      }),
      ...[...this.heroSpots].filter(([id]) => { const h = w.heroes.find((q) => q.id === id); return h && h.state !== 'dead'; }).map(([id, [hx, hy, hz]]) => {
        const h = w.heroes.find((q) => q.id === id)!;
        return { id: 2e7 + id, tile: h.pos, wx: hx, wy: hy + 0.72, wz: hz, text: dist < 15 ? `${h.name.split(' ')[0]} · ${h.level}${h.legend ? ' ♪' : ''}` : `${h.level}${h.legend ? ' ♪' : ''}`, cls: `hero${h.civ === -1 ? ' free' : ''}${h.legend ? ' legend' : ''}`, color: h.civ >= 0 ? w.civs[h.civ].color : '#e8c24a', show: dist < 34 };
      }),
      ...this.musterLabels(),
      ...w.agents.filter((a) => !a.dead && (a.kind === 'army' || a.kind === 'raid') && (a.troops ?? 0) > 0).map((a) => {
        const [ax, ay, az] = this.agentWorld(a);
        const civ = a.civ >= 0 ? w.civs[a.civ] : undefined;
        return { id: 1e7 + a.id, tile: a.path[Math.min(a.step, a.path.length - 1)], wx: ax, wy: ay + 1.1, wz: az, text: `⚔ ${a.troops}${a.heroes?.length ? ' ★' + a.heroes.length : ''}`, cls: a.kind === 'raid' ? `army camp ${a.monster ?? 'goblin'}` : 'army', color: civ?.color ?? '#999', show: dist < 70 };
      }),
    ];
    // gezginde yalnız yakındaki yerlerin adı: dünya bir harita gibi yazılarla dolmasın
    const camP = this.camera.position, labR = this.fly.on ? 26 + Math.max(0, this.camAlt()) * 0.8 : Math.max(55, dist * 2.4);
    for (const it of items) {
      if (!it.show) continue;
      const pw = it as { wx?: number; wy?: number; wz?: number };
      const [x, z] = pw.wx !== undefined ? [pw.wx, pw.wz!] : this.pos(it.tile);
      // alçak açıda ufuktaki uzak etiketler kalabalık yapmasın: kameradan uzak olanları gizle (başkent ve savaş hedefi hariç)
      const cdx = x - camP.x, cdz = z - camP.z;
      const lr = /war|cap/.test(it.cls) ? labR * 1.7 : labR;
      if (cdx * cdx + cdz * cdz > lr * lr) continue;
      const [sx, sy, ok] = this.project(x, pw.wy ?? this.height(it.tile) + (it.cls.startsWith('set') ? 1.6 : it.cls.startsWith('inn') ? 1.35 : 1.1), z);
      if (!ok) continue;
      let el = this.labels.get(it.id);
      if (!el) { el = document.createElement('div'); this.overlay.appendChild(el); this.labels.set(it.id, el); }
      el.className = `dio-label ${it.cls}`;
      if (el.textContent !== it.text) el.textContent = it.text;
      const innId = (it as { inn?: number }).inn;
      if (innId !== undefined && el.dataset.inn !== String(innId)) el.dataset.inn = String(innId);
      if (it.color) el.style.setProperty('--cc', it.color);
      const wdt = it.text.length * (it.cls.startsWith('hero') ? 5.5 : 6.5) + 14;
      const box: [number, number, number, number] = [sx - wdt / 2, sy - 10, sx + wdt / 2, sy + 10];
      if (boxes.some((b) => b[0] < box[2] && box[0] < b[2] && b[1] < box[3] && box[1] < b[3])) continue;
      boxes.push(box);
      el.style.transform = `translate(${sx.toFixed(1)}px, ${sy.toFixed(1)}px) translate(-50%, -50%)`;
      seen.add(it.id);
    }
    for (const [id, el] of this.labels) {
      const vis = seen.has(id);
      if (!vis && id >= 1e7) { el.remove(); this.labels.delete(id); continue; }
      if (el.hidden === vis) el.hidden = !vis;
    }
    for (let k = this.pops.length - 1; k >= 0; k--) {
      const p = this.pops[k];
      const q = (now - p.t0) / p.dur;
      if (q > 1) { p.el.remove(); this.pops.splice(k, 1); continue; }
      if (q < 0) { p.el.style.opacity = '0'; continue; }
      const [x, z] = this.pos(p.tile);
      const [sx, sy, ok] = this.project(x, this.height(p.tile) + 1.2, z);
      p.el.style.opacity = ok ? String(Math.min(1, (1 - q) * 2.5)) : '0';
      p.el.style.transform = `translate(${sx}px, ${sy - q * 46}px) translate(-50%, -50%) scale(${0.8 + Math.min(1, q * 5) * 0.3})`;
    }
  }

  // ---------------------------------------------------------------- ışık, çerçeve
  private light(now: number) {
    const cyc = this.debugCyc ?? (this.dayNight ? ((now / 1000) % 150) / 150 : 0.3);
    this.cyc = cyc;
    const sunA = cyc * Math.PI * 2;
    const elev = Math.sin(sunA); // >0 gündüz
    const day = Math.max(0, Math.min(1, elev * 2.2 + 0.35));
    const dist = 90;
    const tg = this.controls.target;
    // gezginde gölge kutusu bakılan yerin çevresini kaplar; yükseldikçe genişler (dağ gölgeleri vadiye düşsün)
    const camD = this.fly.on ? Math.max(30, this.camAlt() * 1.8, this.camera.position.distanceTo(tg)) : this.camera.position.distanceTo(tg);
    const ext = Math.min(80, Math.max(14, camD * 0.85));
    const sc = this.sun.shadow.camera;
    if (Math.abs(sc.right - ext) > 0.5) { sc.left = -ext; sc.right = ext; sc.top = ext * 0.8; sc.bottom = -ext * 0.8; sc.updateProjectionMatrix(); }
    const fx = ext > 70 ? this.center.x : tg.x, fz = ext > 70 ? this.center.z : tg.z;
    this.sun.target.position.set(fx, 0, fz);
    const wet = 1 - this.atmo.rainAmount * 0.45;
    if (elev >= -0.05) {
      this.sun.position.set(fx + Math.cos(sunA) * dist * 0.8, 25 + Math.max(0.15, elev) * 70, fz + 40 + Math.sin(sunA * 0.5) * 20);
      this.sun.intensity = (0.25 + day * 2.0) * wet;
      this.sun.color.setHSL(0.09, 0.6, 0.62 + day * 0.3);
    } else {
      // ay ışığı: karşı yönden, soğuk mavi
      const mA = sunA + Math.PI;
      this.sun.position.set(fx + Math.cos(mA) * dist * 0.8, 25 + Math.max(0.2, Math.sin(mA)) * 60, fz + 30);
      this.sun.intensity = 0.28 + Math.min(0.3, -elev * 0.5);
      this.sun.color.setHex(0x9fb4ff);
    }
    this.night = 1 - day;
    // bulut gölgesi yalnız güneş yüksekken: alçak güneş ve ay ışığında denizde kapkara lekeler bırakıyordu
    this.atmo.cloudShadows(elev > 0.3 && this.atmo.rainAmount < 0.25 && this.camera.position.y < 24); // yüksekten bakınca bulutlar zaten saydam: gölgeleri haritada kara leke bırakmasın
    this.atmo.seaTone(this.night);
    // sınır şeridi ışıksız malzeme: gece neon gibi parlamasın
    (this.borders.material as THREE.MeshBasicMaterial).color.setScalar(0.42 + day * 0.58);
    this.hemi.intensity = 0.35 + day * 0.65;
    const sky = new THREE.Color(0x141d33).lerp(new THREE.Color(0xbfd9e6), day);
    sky.lerp(new THREE.Color(0x8a939c), this.atmo.rainAmount * 0.5 * day);
    const dusk = this.dayNight && elev < 0.25 && elev > -0.12 ? Math.max(0, 1 - Math.abs(elev - 0.06) / 0.19) : 0;
    if (dusk > 0) sky.lerp(new THREE.Color(0xe8906a), 0.45 * dusk);
    // ortam ışığı tonu: gece ay mavisi, alacakaranlıkta sıcak, gündüz açık gök
    this.hemi.color.setHex(0x4a5a8e).lerp(this.tmpC.setHex(0xdfefff), day);
    if (dusk > 0) this.hemi.color.lerp(this.tmpC.setHex(0xffc79a), 0.35 * dusk);
    this.hemi.groundColor.setHex(0x1e2128).lerp(this.tmpC.setHex(0x5a6b3c), day);
    // şimşek: gök ve ortam ışığı bir an beyazlar
    if (this.flash > 0.01) { sky.lerp(this.tmpC.setHex(0xdfe8ff), Math.min(0.85, this.flash * 0.8)); this.hemi.intensity += this.flash * 1.6; }
    (this.scene.background as THREE.Color).copy(sky);
    this.atmo.cloudTint(sky);
    (this.scene.fog as THREE.Fog).color.copy(sky);
    // gök kubbesi: tepe koyu mavi, ufuk sis rengi; gün batımında güneş tarafı kızıl
    const U = this.atmo.skyU;
    U.hor.value.copy(sky);
    U.top.value.setHex(0x0a1024).lerp(new THREE.Color(0x4f8fd0), day).lerp(new THREE.Color(0x6f7a86), this.atmo.rainAmount * 0.6 * day);
    if (dusk > 0) U.top.value.lerp(new THREE.Color(0x5a5a8a), 0.3 * dusk);
    U.sunDir.value.set(Math.cos(sunA), Math.max(-0.05, elev) * 0.9 + 0.02, 0.35).normalize();
    U.sunCol.value.setHex(0xffc890).lerp(new THREE.Color(0xff7a3a), dusk);
    U.glow.value = elev > -0.08 ? (0.35 + dusk * 1.4) * (1 - this.atmo.rainAmount * 0.8) : 0;
    U.moon.value = this.dayNight ? Math.max(0, Math.min(1, (-elev - 0.05) * 4)) * (1 - this.atmo.rainAmount) : 0;
    this.bloom.strength = 0.15 + (1 - day) * 0.55;
    this.bloom.threshold = 0.78 + day * 0.5;
    this.windowMat.emissiveIntensity = (1 - day) * 0.04;
    this.assetMat.emissiveIntensity = (1 - day) * 0.04;
    this.foamMat.opacity = (0.4 + Math.sin(now / 900) * 0.15) * (0.5 + day * 0.5);
    // pencereler: akşam yanar, gece yarısından sonra çoğu söner
    const late = cyc > 0.62 && cyc < 0.95 ? Math.max(0, Math.min(1, (cyc - 0.62) / 0.08)) * Math.max(0, Math.min(1, (0.95 - cyc) / 0.05)) : 0;
    (this.reg.get('win')!.mat as THREE.MeshStandardMaterial).emissiveIntensity = (1 - day) * 2.2;
    (this.reg.get('win2')!.mat as THREE.MeshStandardMaterial).emissiveIntensity = (1 - day) * 2.2 * (1 - late * 0.9);
    (this.reg.get('door')!.mat as THREE.MeshStandardMaterial).emissiveIntensity = (1 - day) * 1.6;
  }

  resize() {
    const r = this.host.getBoundingClientRect();
    const w = Math.max(1, Math.round(r.width)), h = Math.max(1, Math.round(r.height));
    if (w === this.w && h === this.h) return;
    this.w = w; this.h = h;
    this.renderer.setSize(w, h, false);
    this.composer.setSize(w, h);
    this.camera.aspect = w / h;
    this.camera.updateProjectionMatrix();
  }

  private ctxLost = false;
  render(now: number) {
    if (!this.sim || this.ctxLost) return;
    this.resize();
    if (this.sim.day - this.lastTerrain >= 60 || this.sim.season !== this.lastSeason || (this.sim.day % 5 === 0 && this.sim.day !== this.lastTerrain && this.clearKey() !== this.lastClearKey)) this.buildTerrain();
    if (Math.floor(this.sim.day / 5) !== Math.floor(this.lastDyn / 5) && now - this.lastDynReal > 350) { this.buildDynamic(); this.lastDynReal = now; }
    this.buildActors(now);
    this.light(now);
    const dt = Math.min(0.1, Math.max(0.001, (now - (this.lastFrame || now)) / 1000));
    // kare hızı ölçümü ve otomatik kalite
    this.fcount++;
    if (now - this.ftime >= 1000) {
      this.fps = Math.round((this.fcount * 1000) / Math.max(1, now - this.ftime));
      this.fcount = 0; this.ftime = now;
      if (this.autoQuality && document.visibilityState === 'visible' && now > 8000) {
        this.lowFor = this.fps < 28 ? this.lowFor + 1 : 0;
        this.highFor = this.fps >= 55 ? this.highFor + 1 : 0;
        if (this.lowFor >= 3 && this.quality > 0) { this.lowFor = 0; this.highFor = 0; this.drops[this.quality]++; this.setQuality(this.quality - 1, true); }
        // geçici bir takılma kaliteyi kalıcı düşürmesin: uzun süre rahat akıyorsa bir kademe geri çık
        // (aynı kademeden iki kez düşülmüşse o kademeye bir daha çıkılmaz)
        else if (this.highFor >= 15 && this.quality < 2 && this.drops[this.quality + 1] < 2) { this.highFor = 0; this.setQuality(this.quality + 1, true); }
      }
    }
    this.lastFrame = now;
    const sd = this.sim.day % 30, season = this.sim.season;
    const rain = (season === 0 || season === 2) && hash(this.sim.year * 4 + season, 77) > 0.45 && sd >= 5 && sd < 21;
    // fırtına: yağmurlu dönemlerin bir kısmında şimşek çakar, gök gürler (ses gecikmeli gelir)
    const storm = rain && hash(this.sim.year * 4 + season, 91) > 0.5 && this.atmo.rainAmount > 0.7;
    if (storm && now > this.nextBolt) {
      if (this.nextBolt > 0) { this.flash = 1; this.flash2 = now + 90 + Math.random() * 120; this.onThunder?.(0.4 + Math.random() * 1.8, 0.5 + Math.random() * 0.5); }
      this.nextBolt = now + 5000 + Math.random() * 14000;
    }
    if (!storm) this.nextBolt = 0;
    if (this.flash2 && now > this.flash2) { this.flash = Math.max(this.flash, 0.8); this.flash2 = 0; }
    this.flash *= Math.exp(-dt * 7);
    this.atmo.update(now, dt, this.night, this.controls.target, this.camera, season === 3 && sd >= 2, rain);
    this.windU.t.value = now / 1000;
    this.windU.a.value = 1 + this.atmo.rainAmount * 1.6 + (season === 2 ? 0.4 : 0);
    if (this.fly.on) {
      // gezgin: kamerayı FreeCam sürer; "hedef" bakılan zemin noktasıdır (ayrıntı, gölge, ses, kar kutusu onu izler)
      this.fly.update(dt);
      this.controls.target.lerp(this.roamLook(), Math.min(1, dt * 6));
    } else {
      if (this.tween) {
        const k = 1 - Math.pow(1 - (this.tween.k ?? 0.14), dt * 60);
        this.controls.target.lerp(this.tween.target, k);
        this.camera.position.lerp(this.tween.pos, k);
        if (this.camera.position.distanceTo(this.tween.pos) < 0.05 && this.controls.target.distanceTo(this.tween.target) < 0.05) this.tween = null;
      }
      this.updateFollow();
      if (this.autoOrbit && !this.tween) {
        const o = this.camera.position.clone().sub(this.controls.target);
        o.applyAxisAngle(new THREE.Vector3(0, 1, 0), dt * 0.06);
        this.camera.position.copy(this.controls.target).add(o);
      }
      this.controls.update();
      const floor = this.gy(this.camera.position.x, this.camera.position.z) + 0.6;
      if (this.camera.position.y < floor) this.camera.position.y = floor;
    }
    this.terrain.cull(this.controls.target.x, this.controls.target.z, this.camera.position.distanceTo(this.controls.target));
    this.dynamic.cull(this.controls.target.x, this.controls.target.z, this.camera.position.distanceTo(this.controls.target));
    this.occU.cam.value.copy(this.camera.position); this.occU.tgt.value.copy(this.controls.target);
    this.occU.on.value = !this.fly.on && this.camera.position.distanceTo(this.controls.target) < 60 ? 1 : 0;
    const dist = this.camera.position.distanceTo(this.controls.target);
    const alt = this.camAlt();
    const fog = this.scene.fog as THREE.Fog;
    if (this.fly.on) {
      // yere yakınken pus: uzak dağlar göğe karışır (derinlik hissi)
      fog.near = 16 + alt * 1.25; fog.far = Math.min(650, 150 + alt * 3.4);
    } else { fog.near = dist * 1.3 + 20; fog.far = dist * 3 + 120; }
    this.updateLens(dt, dist, alt);
    // tilt-shift: odak ekranın ortası; tepeden bakınca azalır
    const polar = this.fly.on ? 1.3 : this.controls.getPolarAngle();
    const amt = this.tiltShift && !this.fly.on ? 2.4 * Math.min(1, Math.max(0.15, (polar - 0.1) / 0.7)) * Math.min(1, 30 / Math.max(10, dist) + 0.5) : 0;
    (this.hPass.uniforms.h.value as number) = amt / this.w;
    (this.vPass.uniforms.v.value as number) = amt / this.h;
    this.hPass.uniforms.r.value = 0.55; this.vPass.uniforms.r.value = 0.55;
    this.hPass.enabled = this.vPass.enabled = amt > 0;
    // son işlem yoksa doğrudan ekrana çiz: tarayıcının yerel antialias'ı + daha ucuz
    if (this.hPass.enabled || this.bloom.enabled) this.composer.render();
    else { this.renderer.setRenderTarget(null); this.renderer.render(this.scene, this.camera); }
    this.updateOverlay(now);
    this.drawBubbles(now);
    void YEAR;
  }

  setGrid(v: boolean) { this.showGrid = v; this.grid.visible = v; }
  /** kameranın zeminden (ya da su yüzünden) yüksekliği */
  camAlt() { const p = this.camera.position; return p.y - Math.max(this.hf.sample(p.x, p.z), WATER_Y); }
  private lensNear = 0.5;
  /**
   * Mercek: yere yaklaştıkça görüş açısı genişler (derinlik hissi), yükseldikçe daralır (harita okunur).
   * Yakın düzlem de yüksekliğe göre: yürürken ayak ucundaki çimen kırpılmasın.
   */
  private updateLens(dt: number, dist: number, alt: number) {
    let fov: number, near: number;
    if (this.fly.on) { fov = this.fly.walk ? 66 : 62; near = this.fly.walk ? 0.04 : Math.min(0.4, Math.max(0.05, alt * 0.01)); }
    else { const u = Math.min(1, Math.max(0, (alt - 5) / 50)); fov = 55 - 17 * u * u * (3 - 2 * u); near = Math.min(0.5, Math.max(0.08, dist * 0.008)); }
    const k = Math.min(1, dt * 3.5);
    const nf = this.camera.fov + (fov - this.camera.fov) * k;
    if (Math.abs(nf - this.camera.fov) > 0.005 || Math.abs(near - this.lensNear) > 0.005) {
      this.camera.fov = nf; this.camera.near = this.lensNear = near;
      this.camera.updateProjectionMatrix();
    }
    // ızgara ve sınırlar: yukarıdan bakınca belirgin, yere inince silikleşir; gezginde yok
    const gA = this.fly.on ? 0 : 0.13 * Math.min(1, Math.max(0, (alt - 3) / 20));
    (this.grid.material as THREE.LineBasicMaterial).opacity = gA;
    this.grid.visible = this.showGrid && gA > 0.004;
    const bA = this.fly.on ? 0 : 0.3 + 0.7 * Math.min(1, Math.max(0, (alt - 2) / 16));
    (this.borders.material as THREE.MeshBasicMaterial).opacity = bA;
    this.borders.visible = bA > 0.01;
    this.vignette.hidden = this.fly.on || this.renderer.domElement.hidden;
  }
  /**
   * Gezginde "hedef": kameranın biraz önündeki zemin noktası. Ayrıntı, çimen, gölge kutusu, ses ve kar kutusu
   * bunu izler; bu yüzden uzağa bakılsa da kameranın yanında kalır (en çok birkaç on birim).
   */
  private roamLook(): THREE.Vector3 {
    const cam = this.camera.position, f = this.fly.forward();
    const alt = Math.max(0, this.camAlt());
    const D = Math.min(22, Math.max(3.5, 3 + alt * 1.2));
    const hit = this.hf.raycast(cam, f, D * 1.6, WATER_Y);
    if (hit && Math.hypot(hit.x - cam.x, hit.z - cam.z) <= D) return hit;
    const h = new THREE.Vector3(f.x, 0, f.z);
    if (h.lengthSq() < 1e-6) h.set(0, 0, -1);
    h.normalize().multiplyScalar(D);
    const p = cam.clone().add(h);
    p.y = this.gy(p.x, p.z);
    return p;
  }
  /** gezgin kipindeyken true */
  get roam() { return this.fly.on; }
  onRoam?: () => void;
  enterRoam() {
    if (this.fly.on || !this.sim) return;
    this.follow = null; this.tween = null; this.autoOrbit = false;
    this.controls.enabled = false;
    this.fly.enter(this.controls.target.clone());
  }
  exitRoam() {
    if (!this.fly.on) return;
    const lp = this.hf.raycast(this.camera.position, this.fly.forward(), 60, WATER_Y) ?? this.roamLook();
    this.fly.exit();
    this.controls.enabled = true;
    this.controls.target.copy(lp);
    // yürürken çıkıldıysa kamerayı rahat bir alçak açıya kaldır
    const off = this.camera.position.clone().sub(lp);
    if (off.length() < 7 || off.y < 2.5) { off.y = Math.max(off.y, 3); off.setLength(Math.max(8, off.length())); this.tween = { target: lp.clone(), pos: lp.clone().add(off) }; }
    this.controls.update();
  }
  forceRebuild() { this.lastDyn = -999; this.lastTerrain = -999; }

  /** ekran koordinatından tile */
  pick(clientX: number, clientY: number): number {
    const r = this.renderer.domElement.getBoundingClientRect();
    const ndc = new THREE.Vector2(((clientX - r.left) / r.width) * 2 - 1, -((clientY - r.top) / r.height) * 2 + 1);
    this.ray.setFromCamera(ndc, this.camera);
    // yükseklik alanında ışın yürütme (üçgen üçgen sınamaktan çok ucuz); su yüzü de zemin sayılır
    const p = this.hf.raycast(this.ray.ray.origin, this.ray.ray.direction, 700, WATER_Y);
    return p ? this.nearestTile(p.x, p.z) : -1;
  }
  private ray = new THREE.Raycaster();
  private viewPos(kind: 'top' | 'diorama' | 'low', target = this.controls.target): THREE.Vector3 {
    const az = kind === 'top' ? 0 : this.azimuth();
    const span = this.sim ? Math.max(1, this.sim.w.W / 72) : 1;
    const tv = Math.tan((38 * Math.PI) / 360); // tepe görünümünde mercek 38°'ye daralır
    const fit = this.sim ? Math.max((SQ3 * this.sim.w.W) / 2 / (tv * Math.max(0.5, this.camera.aspect)), (1.5 * this.sim.w.H) / 2 / tv) * 0.92 : 118;
    const [polar, dist] = kind === 'top' ? [0.001, Math.min(315, fit)] : kind === 'diorama' ? [0.72, 95 * span] : [1.22, 20];
    const d = kind === 'top' ? this.center : target;
    return new THREE.Vector3(d.x + Math.sin(polar) * Math.sin(az) * dist, d.y + Math.cos(polar) * dist, d.z + Math.sin(polar) * Math.cos(az) * dist);
  }
  private azimuth() { const o = this.camera.position.clone().sub(this.controls.target); return Math.atan2(o.x, o.z); }
  /** kamera ön ayarları: kuş bakışı, diorama, alçak (yere yakın) */
  view(kind: 'top' | 'diorama' | 'low') {
    this.exitRoam();
    const target = kind === 'top' ? this.center.clone() : kind === 'diorama' ? (this.controls.target.distanceTo(this.center) > 40 ? this.center.clone() : this.controls.target.clone()) : this.controls.target.clone();
    if (kind === 'low') target.y = Math.max(0.5, this.height(Math.max(0, this.nearestTile(target.x, target.z))));
    this.tween = { target, pos: this.viewPos(kind, target) };
  }
  orbit(dAz: number, dPolar: number) {
    if (this.fly.on) return;
    const o = this.camera.position.clone().sub(this.controls.target);
    const sph = new THREE.Spherical().setFromVector3(o);
    sph.theta += dAz;
    sph.phi = Math.min(this.controls.maxPolarAngle, Math.max(0.01, sph.phi + dPolar));
    this.tween = { target: this.controls.target.clone(), pos: this.controls.target.clone().add(new THREE.Vector3().setFromSpherical(sph)) };
  }
  private panKey(code: string) {
    const az = this.azimuth(), dist = this.camera.position.distanceTo(this.controls.target);
    const step = Math.max(2, dist * 0.12);
    const fwd = new THREE.Vector3(-Math.sin(az), 0, -Math.cos(az)), right = new THREE.Vector3(Math.cos(az), 0, -Math.sin(az));
    const mv = code === 'KeyW' ? fwd : code === 'KeyS' ? fwd.negate() : code === 'KeyD' ? right : right.negate();
    mv.multiplyScalar(step);
    this.tween = { target: this.controls.target.clone().add(mv), pos: this.camera.position.clone().add(mv) };
  }
  follow: { kind: 'agent' | 'hero' | 'cit'; id: number; idx?: number } | null = null;
  onInnClick?: (id: number) => void;
  /** izlenen kasabalının hâli */
  citInfo(stId: number, idx: number) {
    const st = this.sim.w.settlements.find((q) => q.id === stId && q.alive);
    const lay = st && this.layouts.get(st.id);
    if (!st || !lay) return null;
    return { st, ...this.citState(st, lay, this.rolesOf(st), idx, performance.now() / 1000) };
  }
  private followPos(): [number, number] | null {
    const f = this.follow, w = this.sim.w;
    if (!f) return null;
    if (f.kind === 'cit') { const c = this.citInfo(f.id, f.idx ?? 0); return c ? [c.x, c.z] : null; }
    if (f.kind === 'agent') {
      const a = w.agents.find((q) => q.id === f.id && !q.dead);
      if (!a) return null;
      const [x, , z] = this.agentWorld(a);
      return [x, z];
    }
    const h = w.heroes.find((q) => q.id === f.id);
    if (!h || h.state === 'dead' || h.state === 'gone') return null;
    const a = w.agents.find((q) => !q.dead && q.heroes?.includes(h.id));
    if (a) { const [x, , z] = this.agentWorld(a); return [x, z]; }
    return this.pos(h.pos);
  }
  /** ekrana en yakın hareketli birim (kahraman, kervan, ordu, akıncı, öncü, göçmen) */
  pickAgent(clientX: number, clientY: number): { kind: 'agent' | 'hero'; id: number } | null {
    const r = this.renderer.domElement.getBoundingClientRect();
    const mx = clientX - r.left, my = clientY - r.top;
    let best: { kind: 'agent' | 'hero'; id: number } | null = null, bd = 30 * 30;
    for (const a of this.sim.w.agents) {
      if (a.dead) continue;
      const [x, y, z] = this.agentWorld(a);
      const [sx, sy, ok] = this.project(x, y + 0.3, z);
      if (!ok) continue;
      const d = (sx - mx) ** 2 + (sy - my) ** 2;
      if (d < bd) { bd = d; best = (a.kind === 'hero' || a.kind === 'party') && a.heroes?.length ? { kind: 'hero', id: a.heroes[0] } : { kind: 'agent', id: a.id }; }
    }
    return best;
  }
  /** ses için çevre özeti: kameranın baktığı yer */
  soundInfo(): SoundInfo {
    const w = this.sim.w, tg = this.controls.target;
    const dist = this.camera.position.distanceTo(tg);
    const ti = this.nearestTile(tg.x, tg.z);
    let water = 0, forest = 0, n = 0;
    if (ti >= 0) for (const q of this.sim.g.within(ti, 3)) { const tr = w.tiles[q].terrain; n++; if (tr === 'water') water++; else if (tr === 'forest' || tr === 'oldforest' || tr === 'grass' || tr === 'swamp') forest += tr === 'grass' ? 0.5 : 1; }
    // harita kenarı açık deniz
    const [cx, cz] = [this.center.x, this.center.z];
    const edge = Math.max(Math.abs(tg.x - cx) / (SQ3 * 36), Math.abs(tg.z - cz) / 36);
    water = Math.min(1, water / Math.max(1, n) * 2 + Math.max(0, edge - 0.85) * 4);
    let town = 0, work = 0, fest = 0, tavern = 0;
    for (const st of w.settlements) {
      if (!st.alive) continue;
      const lay = this.layouts.get(st.id); if (!lay) continue;
      const d = Math.hypot(lay.x - tg.x, lay.z - tg.z);
      const k = Math.max(0, 1 - d / (3 + st.tier * 1.2));
      if (k <= 0) continue;
      const P = this.sim.pop(st);
      town = Math.max(town, k * Math.min(1, 0.3 + P / 120));
      work = Math.max(work, k * Math.min(1, Object.values(st.workshops).reduce((a, v) => a + (v ?? 0), 0) / 4 + (st.project ? 0.3 : 0)));
      if (this.festival(st)) fest = Math.max(fest, k);
      if (st.civics.tavern && this.night > 0.5) tavern = Math.max(tavern, k);
    }
    if (this.night > 0.5) for (const inn of w.inns) { if (!inn.alive) continue; const [ix, iz] = this.pos(inn.tile); tavern = Math.max(tavern, 1 - Math.hypot(ix - tg.x, iz - tg.z) / 3); }
    let battle = 0, fire = 0;
    for (const b of this.battles) { const [bx, bz] = this.pos(b.tile); battle = Math.max(battle, 1 - Math.hypot(bx - tg.x, bz - tg.z) / 6); }
    for (const [bx, , bz] of this.burning) fire = Math.max(fire, 1 - Math.hypot(bx - tg.x, bz - tg.z) / 5);
    return { dist, night: this.night, rain: this.atmo.rainAmount, water, forest: Math.min(1, forest / Math.max(1, n) * 1.4), town, work, fest, battle: Math.max(0, battle), fire: Math.max(0, fire), tavern, alt: this.fly.on ? this.camAlt() : undefined };
  }
  /** gezginin ayağının bastığı zemin (ayak sesi için) */
  stepSurface(): 'grass' | 'stone' | 'snow' | 'water' | 'wood' {
    if (this.fly.swimming) return 'water';
    const p = this.camera.position, i = this.nearestTile(p.x, p.z);
    if (i < 0) return 'grass';
    const t = this.sim.w.tiles[i];
    if (this.sim.season === 3 || t.terrain === 'tundra' || this.hf.sample(p.x, p.z) > 4.4) return 'snow';
    if (t.ext?.kind === 'dock' && this.hf.sample(p.x, p.z) < WATER_Y + 0.2) return 'wood';
    if (t.terrain === 'mountain' || this.hf.slopeAt(p.x, p.z) > 0.9) return 'stone';
    // kasaba ve üstü: taş döşeli sokaklar
    if (w_isTown(this.sim, i)) return 'stone';
    return 'grass';
  }

  /** evin hanesi: soyadı, kaç kişi, kim ne iş yapar */
  houseInfo(st: Settlement, h: House) {
    const family = DIO_FAMILY[Math.floor(hash(h.seed, 3) * DIO_FAMILY.length)];
    const pool: string[] = [];
    for (const [k, v] of Object.entries(st.jobs ?? {})) for (let q = 0; q < (v ?? 0); q++) pool.push(PROF[k] ?? (WS_BY_NAME[k] ? `${k.toLocaleLowerCase('tr')} işçisi` : k));
    const occ = h.occ;
    const elders = occ >= 4 && hash(h.seed, 4) < 0.55 ? 1 : 0;
    const adults = occ <= 1 ? occ : Math.max(1, Math.min(occ - elders, Math.round(occ * 0.55)));
    const kids = Math.max(0, occ - adults - elders);
    const jobs: Record<string, number> = {};
    for (let j = 0; j < adults; j++) { const p = pool.length ? pool[Math.floor(hash(h.seed, 10 + j) * pool.length)] : 'toplayıcı'; jobs[p] = (jobs[p] ?? 0) + 1; }
    const parts = Object.entries(jobs).map(([k, v]) => (v > 1 ? `${v} ${k}` : k));
    if (kids) parts.push(`${kids} çocuk`);
    if (elders) parts.push(`${elders} yaşlı`);
    return { family, kind: h.kind === 'stonehouse' ? 'Taş konak' : h.kind === 'house' ? 'Ev' : 'Kulübe', occ, cap: h.cap, burnt: h.burnt, members: parts, race: h.race };
  }
  pickHouse(clientX: number, clientY: number) {
    const r = this.renderer.domElement.getBoundingClientRect();
    const mx = clientX - r.left, my = clientY - r.top;
    if (this.camera.position.distanceTo(this.controls.target) > 30) return null;
    let best: { st: Settlement; h: House } | null = null, bd = 18 * 18;
    for (const st of this.sim.w.settlements) {
      if (!st.alive) continue;
      const lay = this.layouts.get(st.id);
      if (!lay) continue;
      for (const h of lay.houses) {
        const [sx, sy, ok] = this.project(h.x, h.y + 0.2, h.z);
        if (!ok) continue;
        const d = (sx - mx) ** 2 + (sy - my) ** 2;
        if (d < bd) { bd = d; best = { st, h }; }
      }
    }
    return best ? { st: best.st, ...this.houseInfo(best.st, best.h) } : null;
  }

  /** ekrandaki en yakın kasabalı (yakın plan) */
  pickCitizen(clientX: number, clientY: number) {
    const r = this.renderer.domElement.getBoundingClientRect();
    const mx = clientX - r.left, my = clientY - r.top;
    let best: Diorama['cits'][number] | null = null, bd = 14 * 14;
    for (const c of this.cits) {
      const [sx, sy, ok] = this.project(c.x, c.y + 0.2, c.z);
      if (!ok) continue;
      const d = (sx - mx) ** 2 + (sy - my) ** 2;
      if (d < bd) { bd = d; best = c; }
    }
    return best;
  }
  private drawBubbles(now: number) {
    let k = 0;
    for (const b of this.bubbles) {
      if (k >= 24) break;
      const [sx, sy, ok] = this.project(b.x, b.y, b.z);
      if (!ok) continue;
      let el = this.bubEls[k];
      if (!el) { el = document.createElement('div'); el.className = 'dio-bub'; this.overlay.appendChild(el); this.bubEls[k] = el; }
      if (el.textContent !== b.icon) el.textContent = b.icon;
      const ph = (now / 1000 + hash(b.id, 40) * 6) % 6;
      const vis = b.fixed ? 1 : ph < 3.2 ? Math.min(1, ph * 3, (3.2 - ph) * 3) : 0;
      el.style.opacity = String(vis);
      el.style.transform = `translate(${sx}px, ${sy - 8 - Math.min(1, ph) * 6}px) translate(-50%, -100%)`;
      el.hidden = vis <= 0;
      k++;
    }
    for (; k < this.bubEls.length; k++) this.bubEls[k].hidden = true;
  }
  startFollow(f: { kind: 'agent' | 'hero' | 'cit'; id: number; idx?: number } | null) {
    if (f) this.exitRoam();
    this.follow = f;
    if (!f) return;
    const p = this.followPos();
    if (!p) { this.follow = null; return; }
    const off = this.camera.position.clone().sub(this.controls.target);
    if (f.kind === 'cit') { off.y = Math.max(off.y, off.length() * 0.55); off.setLength(5.5); }
    else if (off.length() > 22 || off.length() < 5) off.setLength(12);
    const target = new THREE.Vector3(p[0], this.gy(p[0], p[1]), p[1]);
    this.tween = { target, pos: target.clone().add(off) };
  }
  private updateFollow() {
    if (!this.follow || this.tween) return;
    const p = this.followPos();
    if (!p) { this.follow = null; this.onFollowEnd?.(); return; }
    const target = new THREE.Vector3(p[0], this.gy(p[0], p[1]), p[1]);
    const d = target.sub(this.controls.target).multiplyScalar(0.12);
    this.controls.target.add(d);
    this.camera.position.add(d);
  }
  onFollowEnd?: () => void;
  /** sinematik çekim: hedefe yavaşça süzül */
  shot(i: number, dist = 14, polar = 0.95) {
    const [x, z] = this.pos(i);
    if (this.fly.on) { this.fly.flyTo(new THREE.Vector3(x, this.gy(x, z), z)); return; }
    const target = new THREE.Vector3(x, this.gy(x, z), z);
    const az = this.azimuth() + (hash(i, Math.floor(this.lastFrame)) - 0.5) * 1.4;
    const pos = new THREE.Vector3(x + Math.sin(polar) * Math.sin(az) * dist, target.y + Math.cos(polar) * dist, z + Math.sin(polar) * Math.cos(az) * dist);
    this.follow = null;
    this.tween = { target, pos, k: 0.035 };
  }
  focusTile(i: number) {
    const [x, z] = this.pos(i);
    if (this.fly.on) { this.fly.flyTo(new THREE.Vector3(x, this.height(i), z)); return; }
    const off = this.camera.position.clone().sub(this.controls.target);
    if (off.length() > 45) off.setLength(30);
    const target = new THREE.Vector3(x, this.height(i), z);
    this.tween = { target, pos: target.clone().add(off) };
  }
  dolly(k: number, instant = false) {
    if (this.fly.on) return;
    const off = this.camera.position.clone().sub(this.controls.target).multiplyScalar(k);
    const len = Math.min(this.controls.maxDistance, Math.max(this.controls.minDistance, off.length()));
    off.setLength(len);
    if (instant) { this.tween = null; this.camera.position.copy(this.controls.target).add(off); }
    else this.tween = { target: this.controls.target.clone(), pos: this.controls.target.clone().add(off) };
  }
  /** ?debug: günün saatini sabitle (0–1) */
  debugCyc?: number;
  private vignette: HTMLDivElement;
  setVisible(v: boolean) { this.renderer.domElement.hidden = !v; this.overlay.hidden = !v; this.vignette.hidden = !v; }
}
