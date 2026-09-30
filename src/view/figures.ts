// Low-poly figürler: bacak, gövde, kol, kafa, şapka, alet. Hepsi instanced parçalar.
import * as THREE from 'three';
import { mergeGeometries } from 'three/examples/jsm/utils/BufferGeometryUtils.js';

export type AddFn = (key: string, x: number, y: number, z: number, sx?: number, sy?: number, sz?: number, ry?: number, color?: number, rx?: number, rz?: number) => void;
type Reg = (key: string, geo: THREE.BufferGeometry, mat?: THREE.Material, shadow?: boolean, receive?: boolean) => void;

export type Tool = 'axe' | 'pick' | 'hoe' | 'shovel' | 'sword' | 'spear' | 'staff' | 'mace' | 'dagger' | 'club' | 'rod' | 'basket' | 'none';
export type Hat = 'none' | 'hood' | 'wizard' | 'helm' | 'goldhelm' | 'leaf' | 'straw' | 'horns' | 'ears';
export type Pose = 'walk' | 'idle' | 'work' | 'fight' | 'guard' | 'carry' | 'fish' | 'cast' | 'cheer';

export interface Look {
  race: string;
  body: number;
  skin: number;
  legs?: number;
  hat?: Hat;
  hatColor?: number;
  tool?: Tool;
  toolColor?: number;
  orb?: number;
  shield?: number;
  sack?: number;
  cape?: number;
}

/** [boy, en] */
export const RACE_DIM: Record<string, [number, number]> = {
  human: [1, 1], elf: [1.1, 0.88], halfelf: [1.05, 0.94], dwarf: [0.78, 1.32], halfling: [0.7, 0.95], gnome: [0.66, 0.9],
  halforc: [1.12, 1.2], dragonborn: [1.16, 1.16], tiefling: [1.03, 0.95],
  goblin: [0.66, 0.92], hobgoblin: [1.05, 1.1], bugbear: [1.38, 1.32],
};

export function registerFigures(r: Reg, std: (flat?: boolean, extra?: THREE.MeshStandardMaterialParameters) => THREE.MeshStandardMaterial) {
  const base = std();
  const leg = new THREE.BoxGeometry(0.045, 0.13, 0.05); leg.translate(0, -0.065, 0);
  r('f_leg', leg, base);
  const torso = new THREE.CylinderGeometry(0.07, 0.095, 0.2, 7); torso.translate(0, 0.1, 0);
  r('f_torso', torso, base);
  const arm = new THREE.BoxGeometry(0.036, 0.13, 0.036); arm.translate(0, -0.065, 0);
  r('f_arm', arm, base);
  const head = new THREE.IcosahedronGeometry(0.082, 1);
  r('f_head', head, base);
  // gözler: kafanın önünde iki küçük koyu nokta (yakın planda figürlere yüz verir)
  const e1 = new THREE.BoxGeometry(0.018, 0.024, 0.01); e1.translate(-0.03, 0.012, 0.074);
  const e2 = new THREE.BoxGeometry(0.018, 0.024, 0.01); e2.translate(0.03, 0.012, 0.074);
  r('f_eyes', mergeGeometries([e1, e2])!, std(true, { roughness: 0.4 }), false, false);
  // şapkalar (kafa merkezine göre)
  const hood = new THREE.ConeGeometry(0.1, 0.16, 7); hood.translate(0, 0.07, -0.01);
  r('h_hood', hood, base);
  const wiz = new THREE.ConeGeometry(0.075, 0.3, 7); wiz.translate(0, 0.19, 0);
  r('h_wiz', wiz, base);
  const brim = new THREE.CylinderGeometry(0.15, 0.15, 0.014, 10); brim.translate(0, 0.05, 0);
  r('h_brim', brim, base);
  const helm = new THREE.SphereGeometry(0.094, 8, 4, 0, Math.PI * 2, 0, Math.PI / 2); helm.translate(0, 0.005, 0);
  r('h_helm', helm, std(true, { metalness: 0.5, roughness: 0.45 }));
  const leaf = new THREE.TorusGeometry(0.085, 0.02, 4, 10); leaf.rotateX(Math.PI / 2); leaf.translate(0, 0.045, 0);
  r('h_leaf', leaf, base);
  const straw = new THREE.ConeGeometry(0.16, 0.08, 10); straw.translate(0, 0.08, 0);
  r('h_straw', straw, base);
  const horn = new THREE.ConeGeometry(0.02, 0.1, 5); horn.rotateZ(-0.7); horn.translate(0.08, 0.07, 0);
  r('h_horn', horn, base);
  const ear = new THREE.ConeGeometry(0.022, 0.09, 4); ear.rotateZ(-1.25); ear.translate(0.1, 0.02, 0);
  r('h_ear', ear, base);
  // aletler: el noktasından -y yönüne uzanır
  const handle = new THREE.CylinderGeometry(0.011, 0.011, 1, 5); handle.translate(0, -0.5, 0);
  r('t_handle', handle, base);
  const axe = new THREE.BoxGeometry(0.018, 0.065, 0.08); axe.translate(0, -0.27, 0.035);
  r('t_axe', axe, std(true, { metalness: 0.5, roughness: 0.4 }));
  const pick = new THREE.BoxGeometry(0.02, 0.024, 0.18); pick.translate(0, -0.29, 0);
  r('t_pick', pick, std(true, { metalness: 0.5, roughness: 0.4 }));
  const hoe = new THREE.BoxGeometry(0.06, 0.012, 0.06); hoe.translate(0, -0.35, 0.03);
  r('t_hoe', hoe, std(true, { metalness: 0.4, roughness: 0.5 }));
  const shovel = new THREE.BoxGeometry(0.06, 0.08, 0.01); shovel.translate(0, -0.35, 0);
  r('t_shovel', shovel, std(true, { metalness: 0.4, roughness: 0.5 }));
  const blade = new THREE.BoxGeometry(0.022, 0.27, 0.008); blade.translate(0, -0.18, 0);
  r('t_blade', blade, std(true, { metalness: 0.7, roughness: 0.3 }));
  const guard = new THREE.BoxGeometry(0.07, 0.014, 0.018); guard.translate(0, -0.045, 0);
  r('t_guard', guard, base);
  const dagger = new THREE.BoxGeometry(0.016, 0.12, 0.006); dagger.translate(0, -0.1, 0);
  r('t_dagger', dagger, std(true, { metalness: 0.7, roughness: 0.3 }));
  const tip = new THREE.ConeGeometry(0.022, 0.07, 5); tip.rotateX(Math.PI); tip.translate(0, -0.53, 0);
  r('t_tip', tip, std(true, { metalness: 0.6, roughness: 0.35 }));
  const orb = new THREE.IcosahedronGeometry(0.035, 0); orb.translate(0, -0.47, 0);
  r('t_orb', orb, std(true, { emissive: new THREE.Color(0xffffff), emissiveIntensity: 0.9, roughness: 0.3 }), false, false);
  const mace = new THREE.IcosahedronGeometry(0.035, 0); mace.translate(0, -0.21, 0);
  r('t_mace', mace, std(true, { metalness: 0.5, roughness: 0.4 }));
  const club = new THREE.CylinderGeometry(0.04, 0.016, 0.3, 6); club.translate(0, -0.15, 0);
  r('t_club', club, base);
  const basket = new THREE.CylinderGeometry(0.06, 0.045, 0.07, 7); basket.translate(0, -0.06, 0.04);
  r('t_basket', basket, base);
  const shield = new THREE.CylinderGeometry(0.085, 0.085, 0.02, 8); shield.rotateX(Math.PI / 2); shield.translate(0, -0.08, 0.04);
  r('t_shield', shield, std(true, { metalness: 0.3, roughness: 0.5 }));
  const sack = new THREE.IcosahedronGeometry(0.075, 0); sack.scale(1, 1.2, 0.8);
  r('t_sack', sack, base);
  const cape = new THREE.BoxGeometry(0.15, 0.2, 0.012); cape.translate(0, -0.1, 0);
  r('t_cape', cape, std(true, { side: THREE.DoubleSide }));
  // saç ve sakal (şapkasız kafalarda)
  const hair = new THREE.SphereGeometry(0.089, 8, 5, 0, Math.PI * 2, 0, Math.PI * 0.55); hair.rotateX(-0.35); hair.translate(0, 0.008, -0.008);
  r('f_hair', hair, base);
  const beard = new THREE.ConeGeometry(0.055, 0.09, 6); beard.rotateX(Math.PI); beard.translate(0, -0.06, 0.05);
  r('f_beard', beard, base);
  // at: gövde (göğüs ve sağrı yuvarlatılmış), boyun ve baş tek parça; yele ve kuyruk koyu renkte ayrı
  const nonIdx = (g: THREE.BufferGeometry) => { const n = g.index ? g.toNonIndexed() : g; if (n.getAttribute('uv')) n.deleteAttribute('uv'); return n; };
  const hparts: THREE.BufferGeometry[] = [];
  { const b = new THREE.BoxGeometry(0.1, 0.1, 0.24); b.translate(0, 0.2, 0); hparts.push(nonIdx(b)); }
  { const c = new THREE.IcosahedronGeometry(0.062, 0); c.scale(0.9, 1, 1); c.translate(0, 0.205, 0.105); hparts.push(nonIdx(c)); }
  { const c = new THREE.IcosahedronGeometry(0.066, 0); c.scale(0.95, 0.95, 1); c.translate(0, 0.215, -0.1); hparts.push(nonIdx(c)); }
  { const n = new THREE.BoxGeometry(0.055, 0.15, 0.07); n.rotateX(0.55); n.translate(0, 0.29, 0.15); hparts.push(nonIdx(n)); }
  { const h = new THREE.BoxGeometry(0.05, 0.06, 0.12); h.rotateX(0.35); h.translate(0, 0.35, 0.215); hparts.push(nonIdx(h)); }
  for (const e of [-1, 1]) { const ear = new THREE.ConeGeometry(0.01, 0.035, 3); ear.translate(e * 0.018, 0.39, 0.18); hparts.push(nonIdx(ear)); }
  const hbody = mergeGeometries(hparts)!; hbody.computeVertexNormals();
  r('horse_body', hbody, base);
  const mparts: THREE.BufferGeometry[] = [];
  { const m = new THREE.BoxGeometry(0.02, 0.05, 0.15); m.rotateX(0.55); m.translate(0, 0.33, 0.125); mparts.push(nonIdx(m)); }
  { const tl = new THREE.BoxGeometry(0.025, 0.14, 0.03); tl.rotateX(-0.35); tl.translate(0, 0.17, -0.18); mparts.push(nonIdx(tl)); }
  const mane = mergeGeometries(mparts)!; mane.computeVertexNormals();
  r('horse_head', mane, base);
  const hl = new THREE.BoxGeometry(0.03, 0.16, 0.032); hl.translate(0, -0.08, 0);
  const hoof = new THREE.BoxGeometry(0.036, 0.022, 0.04); hoof.translate(0, -0.155, 0.003);
  const leg2 = mergeGeometries([nonIdx(hl), nonIdx(hoof)])!; leg2.computeVertexNormals();
  r('horse_leg', leg2, base);
}

const HAIR = [0x3a2a1e, 0x2a1e16, 0x5a3a22, 0x8a5a2a, 0xc89a5a, 0x1c1814, 0x6a4a30, 0xa0602a, 0x8a8480];
const DWARF_BEARD = [0x8a3a1a, 0x5a3a22, 0xa0602a, 0x9a948a, 0x3a2a1e];
const TOOL_LEN: Record<Tool, number> = { axe: 0.3, pick: 0.3, hoe: 0.36, shovel: 0.33, sword: 0.06, spear: 0.52, staff: 0.46, mace: 0.2, dagger: 0.04, club: 0, rod: 0.62, basket: 0, none: 0 };
const UPRIGHT: Partial<Record<Tool, true>> = { spear: true, staff: true };

/** yerel (lx,ly,lz) → dünya; yön ry */
const tw = (x: number, z: number, ry: number, lx: number, lz: number): [number, number] => [x + lx * Math.cos(ry) + lz * Math.sin(ry), z - lx * Math.sin(ry) + lz * Math.cos(ry)];

/**
 * Bir figür çizer. detail=false iken yalnızca gövde+kafa (uzak kamera).
 */
export function drawFigure(add: AddFn, x: number, y: number, z: number, face: number, t: number, id: number, look: Look, pose: Pose, scale = 1, detail = true) {
  const [H0, W0] = RACE_DIM[look.race] ?? [1, 1];
  const H = H0 * scale, W = W0 * scale;
  const ph = t * (pose === 'walk' || pose === 'carry' ? 9 : 1) + id * 1.7;
  const walking = pose === 'walk' || pose === 'carry';
  const bob = walking ? Math.abs(Math.sin(ph)) * 0.025 * H : 0;
  const hip = 0.13 * H;
  // iş döngüsü: yavaş kaldır, hızlı vur
  const rate = pose === 'fight' ? 1.6 : pose === 'work' ? 0.9 : 0.5;
  const cyc = ((t * rate + id * 0.37) % 1 + 1) % 1;
  const strike = cyc < 0.62 ? cyc / 0.62 : 1 - (cyc - 0.62) / 0.38;
  let lean = 0, liftR = 0.1, liftL = 0.1, legA = 0;
  switch (pose) {
    case 'walk': legA = Math.sin(ph) * 0.65; liftR = 0.1 - Math.sin(ph) * 0.45; liftL = 0.1 + Math.sin(ph) * 0.45; break;
    case 'carry': legA = Math.sin(ph) * 0.55; liftR = 2.7; liftL = 0.1 + Math.sin(ph) * 0.35; lean = 0.12; break;
    case 'idle': liftR = 0.12 + Math.sin(t * 1.3 + id) * 0.05; liftL = 0.12; break;
    case 'guard': liftR = 0.35; liftL = 0.9; break;
    case 'work': liftR = 0.7 + strike * 2.5; liftL = 0.5 + strike * 1.8; lean = (1 - strike) * 0.35; break;
    case 'fight': liftR = 0.6 + strike * 2.6; liftL = 1.0; lean = (1 - strike) * 0.25; legA = 0.35; break;
    case 'fish': liftR = 1.9 + Math.sin(t * 0.8 + id) * 0.08; liftL = 1.4; break;
    case 'cast': liftR = 2.2 + Math.sin(t * 3 + id) * 0.4; liftL = 1.6 + Math.sin(t * 3 + id + 1) * 0.3; break;
    case 'cheer': liftR = 2.8 + Math.sin(t * 7 + id) * 0.3; liftL = 2.8 - Math.sin(t * 7 + id) * 0.3; legA = 0; break;
  }
  const yy = y + bob;
  if (!detail) {
    add('f_torso', x, yy + hip * 0.4, z, W, H * 1.2, W, face, look.body);
    add('f_head', x, yy + 0.4 * H + hip * 0.1, z, W * 0.95, H * 0.95, W * 0.95, face, look.skin);
    return;
  }
  // bacaklar
  const legC = look.legs ?? 0x4a3a2a;
  for (const s of [-1, 1]) {
    const [lx, lz] = tw(x, z, face, s * 0.042 * W, 0);
    add('f_leg', lx, yy + hip, lz, W, H, W, face, legC, s * legA);
  }
  // gövde
  const [bx, bz] = tw(x, z, face, 0, 0);
  add('f_torso', bx, yy + hip - 0.01 * H, bz, W, H, W, face, look.body, lean);
  const shY = yy + hip + 0.18 * H;
  const fwd = lean * 0.18 * H;
  // kafa + şapka
  const [hx, hz] = tw(x, z, face, 0, fwd + 0.01);
  const headY = shY + 0.1 * H;
  add('f_head', hx, headY, hz, W * 0.95, H * 0.95, W * 0.95, face, look.skin);
  if (scale < 0.9 || look.race !== 'bugbear') add('f_eyes', hx, headY, hz, W * 0.95, H * 0.95, W * 0.95, face, 0x1c1612);
  const hc = look.hatColor ?? look.body;
  // şapkası saçı örtmeyenlerde saç; cücelerde (ve bazı insanlarda) sakal
  const bald = look.race === 'goblin' || look.race === 'hobgoblin' || look.race === 'bugbear' || look.race === 'dragonborn' || look.race === 'halforc' && (id % 3 === 0);
  if (!bald && (!look.hat || look.hat === 'none' || look.hat === 'ears' || look.hat === 'horns' || look.hat === 'leaf')) {
    const hcol = HAIR[(id * 7 + (look.race === 'elf' ? 3 : 0)) % HAIR.length];
    add('f_hair', hx, headY, hz, W * 0.95, H * 0.95, W * 0.95, face, look.race === 'elf' && id % 2 ? 0xe8d9a0 : hcol);
  }
  if (look.race === 'dwarf' || (look.race === 'human' && id % 5 === 0 && scale > 0.8)) add('f_beard', hx, headY, hz, W * 0.95, H * (look.race === 'dwarf' ? 1.3 : 0.8), W * 0.95, face, look.race === 'dwarf' ? DWARF_BEARD[id % DWARF_BEARD.length] : HAIR[(id * 7) % HAIR.length]);
  switch (look.hat) {
    case 'hood': add('h_hood', hx, headY, hz, W, H, W, face, hc); break;
    case 'wizard': add('h_wiz', hx, headY, hz, W, H, W, face, hc, -0.15); add('h_brim', hx, headY, hz, W, H, W, face, hc); break;
    case 'helm': add('h_helm', hx, headY, hz, W, H, W, face, 0x9aa0a8); break;
    case 'goldhelm': add('h_helm', hx, headY, hz, W, H, W, face, 0xd8b24a); break;
    case 'leaf': add('h_leaf', hx, headY, hz, W, H, W, face, 0x5f9a3f); break;
    case 'straw': add('h_straw', hx, headY, hz, W, H, W, face, 0xd8bf6a); break;
    case 'horns': add('h_horn', hx, headY, hz, W, H, W, face, 0xe8dcc0); add('h_horn', hx, headY, hz, W, H, W, face + Math.PI, 0xe8dcc0); break;
    case 'ears': add('h_ear', hx, headY, hz, W, H, W, face, look.skin); add('h_ear', hx, headY, hz, W, H, W, face + Math.PI, look.skin); break;
  }
  if (look.cape !== undefined) { const [cx, cz] = tw(x, z, face, 0, -0.07 * W); add('t_cape', cx, shY + 0.02 * H, cz, W, H, W, face, look.cape, walking ? -0.25 - Math.abs(Math.sin(ph)) * 0.15 : -0.08); }
  if (look.sack !== undefined) { const [sx, sz] = tw(x, z, face, 0.02, -0.09 * W); add('t_sack', sx, shY - 0.02 * H, sz, W, H, W, face, look.sack); }
  // kollar: rx = -lift (0 aşağı, π/2 ileri, π yukarı)
  const armL = 0.13 * H;
  const hand = (side: number, lift: number): [number, number, number] => {
    const [sx, sz] = tw(x, z, face, side * 0.1 * W, fwd);
    const [dx, dz] = tw(0, 0, face, 0, Math.sin(lift) * armL);
    return [sx + dx, shY - Math.cos(lift) * armL, sz + dz];
  };
  for (const [side, lift] of [[1, liftR], [-1, liftL]] as const) {
    const [sx, sz] = tw(x, z, face, side * 0.1 * W, fwd);
    add('f_arm', sx, shY, sz, W, H, W, face, look.body, -lift);
  }
  // sağ el aleti
  const tool = look.tool ?? 'none';
  if (tool !== 'none') {
    const [px, py, pz] = hand(1, liftR);
    const up = UPRIGHT[tool] && pose !== 'fight' && pose !== 'cast';
    const rx = up ? -(liftR + Math.PI) + 0.15 : tool === 'rod' ? -(liftR - 0.35) : -liftR;
    const tc = look.toolColor ?? 0x7a5533;
    const L = TOOL_LEN[tool];
    if (L > 0) add('t_handle', px, py, pz, W, L * H * (1 / scale) * scale, W, face, tool === 'staff' ? 0x6b4a2b : tc, rx);
    const k = H;
    switch (tool) {
      case 'axe': add('t_axe', px, py, pz, W, k, W, face, 0xb8bcc2, rx); break;
      case 'pick': add('t_pick', px, py, pz, W, k, W, face, 0x9aa0a8, rx); break;
      case 'hoe': add('t_hoe', px, py, pz, W, k, W, face, 0x9aa0a8, rx); break;
      case 'shovel': add('t_shovel', px, py, pz, W, k, W, face, 0x9aa0a8, rx); break;
      case 'sword': add('t_blade', px, py, pz, W, k, W, face, 0xd8dde2, rx); add('t_guard', px, py, pz, W, k, W, face, 0x8a6a2a, rx); break;
      case 'dagger': add('t_dagger', px, py, pz, W, k, W, face, 0xd8dde2, rx); break;
      case 'spear': add('t_tip', px, py, pz, W, k, W, face, 0xc8ccd2, rx); break;
      case 'staff': add('t_orb', px, py, pz, W, k, W, face, look.orb ?? 0x9fe0ff, rx); break;
      case 'mace': add('t_mace', px, py, pz, W, k, W, face, 0x9aa0a8, rx); break;
      case 'club': add('t_club', px, py, pz, W, k, W, face, 0x6b4a2b, rx); break;
      case 'basket': add('t_basket', px, py, pz, W, k, W, face, 0xb89a5a, 0); break;
      case 'rod': break;
    }
  }
  if (look.shield !== undefined) {
    const [px, py, pz] = hand(-1, liftL);
    add('t_shield', px, py, pz, W, H, W, face, look.shield, -liftL + 0.2);
  }
}

const _dc = new THREE.Color();
const darker = (c: number) => _dc.setHex(c).multiplyScalar(0.45).getHex();
/** koşan/duran at */
export function drawHorse(add: AddFn, x: number, y: number, z: number, face: number, t: number, id: number, color: number, moving = true) {
  const ph = t * 10 + id;
  const bob = moving ? Math.abs(Math.sin(ph)) * 0.02 : 0;
  add('horse_body', x, y + bob, z, 1, 1, 1, face, color);
  add('horse_head', x, y + bob, z, 1, 1, 1, face, darker(color));
  for (const [lx, lz, p] of [[-0.04, 0.11, 0], [0.04, 0.11, Math.PI], [-0.04, -0.11, Math.PI], [0.04, -0.11, 0]] as const) {
    const [px, pz] = tw(x, z, face, lx, lz);
    add('horse_leg', px, y + 0.16 + bob, pz, 1, 1, 1, face, color, moving ? Math.sin(ph + p) * 0.5 : 0);
  }
}
