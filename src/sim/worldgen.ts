import { Rng } from './rng';
import { HexGrid } from './hex';
import { CLASSES, type ClassId } from '../data/classes';
import { DEPOSITS, WOOD_RESERVE, STONE_RESERVE, type DepositKind, type Terrain } from '../data/goods';
import type { Camp, Civ, Deposit, Inn, IsleInfo, IsleKind, Relation, Settlement, Tile, World } from './types';

export const MAP_W = 110;
export const MAP_H = 75;
/** eski 72×48 haritaya göre kara alanı çarpanı (yatak, kamp sayıları) */
export const AREA_K = 1.6;

function valueNoise(rng: Rng, W: number, H: number, cell: number) {
  const gw = Math.ceil(W / cell) + 2, gh = Math.ceil(H / cell) + 2;
  const grid: number[] = [];
  for (let i = 0; i < gw * gh; i++) grid.push(rng.next());
  const s = (t: number) => t * t * (3 - 2 * t);
  return (x: number, y: number) => {
    const gx = x / cell, gy = y / cell;
    const x0 = Math.floor(gx), y0 = Math.floor(gy);
    const tx = s(gx - x0), ty = s(gy - y0);
    const g = (a: number, b: number) => grid[b * gw + a];
    const a = g(x0, y0) * (1 - tx) + g(x0 + 1, y0) * tx;
    const b = g(x0, y0 + 1) * (1 - tx) + g(x0 + 1, y0 + 1) * tx;
    return a * (1 - ty) + b * ty;
  };
}

export const emptyRel = (): Relation => ({ contact: false, mods: [], war: null, treaty: null, tension: {}, lastTalk: -9999, lastRaid: -9999 });

export function generateWorld(seed: number): World {
  const rng = new Rng(seed);
  const W = MAP_W, H = MAP_H;
  const g = new HexGrid(W, H);
  const n1 = valueNoise(rng, W, H, 10), n2 = valueNoise(rng, W, H, 4), m1 = valueNoise(rng, W, H, 8), m2 = valueNoise(rng, W, H, 3), of = valueNoise(rng, W, H, 5);
  // kıta şekli: büyük ölçekli gürültü + merkeze doğru yükselen kubbe; eşik kara oranına göre seçilir
  const L1 = valueNoise(rng, W, H, 26), L2 = valueNoise(rng, W, H, 11), L3 = valueNoise(rng, W, H, 5);
  const shape: number[] = [];
  for (let r = 0; r < H; r++)
    for (let c = 0; c < W; c++) {
      const dx = (c - W / 2) / (W / 2), dy = (r - H / 2) / (H / 2);
      const d = Math.sqrt(dx * dx * 0.9 + dy * dy * 1.1);
      shape.push(L1(c, r) * 0.48 + L2(c, r) * 0.36 + L3(c, r) * 0.16 - Math.pow(d, 2.4) * 0.6);
    }
  const sorted = [...shape].sort((a, b) => a - b);
  const thr = sorted[Math.floor(sorted.length * 0.44)]; // ~%56 kara
  const tiles: Tile[] = [];
  const landE: number[] = [];
  for (let r = 0; r < H; r++)
    for (let c = 0; c < W; c++) {
      const i = r * W + c;
      const edge = Math.min(c, r, W - 1 - c, H - 1 - r);
      const sea = shape[i] < thr || edge < 2;
      const e = n1(c, r) * 0.62 + n2(c, r) * 0.26 + Math.max(0, shape[i] - thr) * 0.45;
      if (!sea) landE.push(e);
      tiles.push({ terrain: sea ? 'water' : 'grass', elev: sea ? Math.max(0, 0.18 - (thr - shape[i]) * 0.8) : e, owner: -1, road: 0, deposit: -1, reserve: 0, wood: 0, sea: sea || undefined });
    }
  // yükseklik dağılımı karaya göre: dağ ve tepe oranı sabit
  landE.sort((a, b) => a - b);
  const q = (f: number) => landE[Math.min(landE.length - 1, Math.floor(landE.length * f))];
  const eMtn = q(0.925), eHill = q(0.8), eLow = q(0.22);
  const tundraRows = Math.round(H * 0.1);
  for (let r = 0; r < H; r++)
    for (let c = 0; c < W; c++) {
      const i = r * W + c, t = tiles[i];
      if (t.sea) continue;
      const e = t.elev, m = m1(c, r) * 0.7 + m2(c, r) * 0.3;
      let terrain: Terrain = 'grass';
      if (e > eMtn) terrain = 'mountain';
      else if (e > eHill) terrain = 'hill';
      else if (e < eLow && m > 0.56) terrain = 'swamp';
      else if (m > 0.54) terrain = of(c, r) > 0.68 && m > 0.6 ? 'oldforest' : 'forest';
      if (r < tundraRows + Math.round(n2(c, 0) * 3) && (terrain === 'grass' || terrain === 'swamp' || terrain === 'forest')) terrain = 'tundra';
      t.terrain = terrain;
      // kıyı: denize göre yükseklik 0.3–0.7 aralığına yayılır (görünüm için)
      t.elev = 0.28 + ((e - landE[0]) / Math.max(0.01, landE[landE.length - 1] - landE[0])) * 0.62;
    }
  // adalar: açık denizde birkaç küçük kara parçası
  const nIsles = rng.int(3, 7);
  for (let k = 0; k < nIsles; k++) {
    for (let tries = 0; tries < 80; tries++) {
      const i = rng.int(0, W * H - 1);
      const c = g.col(i), r = g.row(i);
      if (!tiles[i].sea || c < 4 || r < 4 || c > W - 5 || r > H - 5) continue;
      if (g.within(i, 3).some((n) => !tiles[n].sea)) continue;
      const rad = rng.int(1, 3);
      for (const n of g.within(i, rad)) {
        const cn = g.col(n), rn = g.row(n);
        if (cn < 3 || rn < 3 || cn > W - 4 || rn > H - 4) continue;
        if (g.dist(i, n) === rad && rng.chance(0.45)) continue;
        const e = 0.3 + (1 - g.dist(i, n) / (rad + 1)) * 0.4 + rng.next() * 0.1;
        const m = rng.next();
        tiles[n] = { ...tiles[n], sea: undefined, elev: e, terrain: e > 0.62 ? 'hill' : m > 0.55 ? 'forest' : 'grass' };
      }
      break;
    }
  }
  // kara parçaları: en büyüğü anakara (isle 0), küçük adacıklar denize döner
  const labelLand = () => {
    const comp = new Array<number>(tiles.length).fill(-1);
    const sizes: number[] = [];
    for (let i = 0; i < tiles.length; i++) {
      if (tiles[i].sea || comp[i] >= 0) continue;
      const id = sizes.length; let n = 0;
      const st = [i]; comp[i] = id;
      while (st.length) { const cur = st.pop()!; n++; for (const nb of g.neighbors(cur)) if (!tiles[nb].sea && comp[nb] < 0) { comp[nb] = id; st.push(nb); } }
      sizes.push(n);
    }
    return { comp, sizes };
  };
  {
    const { comp, sizes } = labelLand();
    const main = sizes.indexOf(Math.max(...sizes));
    const order = sizes.map((n, k) => [n, k]).filter(([, k]) => k !== main).sort((a, b) => b[0] - a[0]).map(([, k]) => k);
    const isleId = new Map<number, number>([[main, 0]]);
    order.forEach((k, j) => isleId.set(k, j + 1));
    for (let i = 0; i < tiles.length; i++) {
      if (comp[i] < 0) continue;
      if (sizes[comp[i]] < 4) { tiles[i] = { ...tiles[i], terrain: 'water', sea: true, elev: 0.12 }; continue; }
      const id = isleId.get(comp[i])!;
      if (id) tiles[i].isle = id;
    }
  }
  const mainLand = (i: number) => !tiles[i].sea && !tiles[i].isle;
  // Göller (yalnız iç kesimde)
  for (let k = 0; k < Math.round(4 * AREA_K); k++) {
    let best = -1, be = 9;
    for (let t = 0; t < 60; t++) { const i = rng.int(0, W * H - 1); if (mainLand(i) && tiles[i].elev < be && tiles[i].terrain !== 'tundra' && !g.within(i, 3).some((n) => tiles[n].sea)) { be = tiles[i].elev; best = i; } }
    if (best >= 0) for (const t of g.within(best, rng.int(1, 2))) if (!tiles[t].sea && rng.chance(0.8)) tiles[t].terrain = 'water';
  }
  // Nehirler: dağdan denize ya da göle akar
  for (let k = 0; k < Math.round(3 * AREA_K) + 1; k++) {
    const mts = tiles.map((t, i) => (t.terrain === 'mountain' && mainLand(i) ? i : -1)).filter((i) => i >= 0);
    if (!mts.length) break;
    let cur = rng.pick(mts);
    const seen = new Set<number>();
    for (let step = 0; step < 120; step++) {
      seen.add(cur);
      if (tiles[cur].terrain !== 'mountain') tiles[cur].terrain = 'water';
      const ns = g.neighbors(cur).filter((n) => !seen.has(n));
      if (!ns.length) break;
      if (ns.some((n) => tiles[n].sea)) break;
      if (ns.some((n) => tiles[n].terrain === 'water' && !seen.has(n)) && step > 6) break;
      // karşılaştırıcı içinde rng kullanmak motorlar arası farklı sonuç verir; önce sabit gürültü
      const jit = ns.map((n) => tiles[n].elev + (rng.next() - 0.5) * 0.06);
      cur = ns[jit.indexOf(Math.min(...jit))];
    }
  }

  // Medeniyet seçimi
  const pool = (Object.keys(CLASSES) as ClassId[]).filter((c) => CLASSES[c].implemented);
  rng.shuffle(pool);
  const count = 7 + rng.int(0, 2);
  const chosen = pool.slice(0, count);
  const starts: number[] = [];
  for (const cls of chosen) {
    const like = CLASSES[cls].terrainLike;
    let best = -1, bs = -Infinity;
    for (let i = 0; i < tiles.length; i++) {
      const t = tiles[i];
      if (t.terrain !== 'grass' || !mainLand(i)) continue;
      if (g.within(i, 1).some((n) => tiles[n].sea)) continue;
      if (starts.some((s) => g.dist(s, i) < 15)) continue;
      let sc = rng.next() * 3;
      for (const n of g.within(i, 3)) sc += like[tiles[n].terrain] ?? 0.3;
      if (g.within(i, 2).some((n) => tiles[n].terrain === 'water')) sc += 3;
      if (sc > bs) { bs = sc; best = i; }
    }
    if (best < 0) {
      // yedek: en uzak çayır
      for (let i = 0; i < tiles.length; i++) if (mainLand(i) && tiles[i].terrain !== 'water' && tiles[i].terrain !== 'mountain' && !starts.some((s) => g.dist(s, i) < 10)) { best = i; break; }
    }
    starts.push(best);
    for (const t of g.within(best, 1)) if (tiles[t].terrain !== 'water') tiles[t].terrain = 'grass';
    // yakında orman garantisi
    const ring = g.within(best, 3).filter((t) => g.dist(best, t) >= 2 && tiles[t].terrain === 'grass');
    const forests = g.within(best, 3).filter((t) => tiles[t].terrain === 'forest' || tiles[t].terrain === 'oldforest').length;
    for (const t of rng.shuffle(ring).slice(0, Math.max(0, 4 - forests))) tiles[t].terrain = 'forest';
    // tepe garantisi (taş)
    if (!g.within(best, 2).some((t) => tiles[t].terrain === 'hill')) {
      const cands = g.within(best, 2).filter((t) => g.dist(best, t) === 2 && tiles[t].terrain === 'grass');
      if (cands.length) tiles[rng.pick(cands)].terrain = 'hill';
    }
  }

  // Temel rezervler
  for (const t of tiles) {
    if (t.terrain === 'forest' || t.terrain === 'oldforest') t.wood = WOOD_RESERVE;
    if (t.terrain === 'hill' || t.terrain === 'mountain') t.reserve = STONE_RESERVE;
  }

  // Yataklar
  const deposits: Deposit[] = [];
  let nextId = 1;
  const place = (kind: DepositKind, near?: number, dmin = 3, dmax = 7): Deposit | null => {
    const def = DEPOSITS[kind];
    const ok = (i: number) => tiles[i].deposit < 0 && !starts.includes(i) && starts.every((s) => g.dist(s, i) >= (near !== undefined ? 1 : 3));
    let seedTile = -1;
    for (let tries = 0; tries < 400; tries++) {
      const i = near !== undefined ? rng.pick(g.within(near, dmax).filter((x) => g.dist(near, x) >= dmin)) : rng.int(0, W * H - 1);
      if (i === undefined || !ok(i) || tiles[i].sea) continue;
      if (!def.terrain.includes(tiles[i].terrain)) {
        if (near === undefined || tiles[i].terrain === 'water' || tiles[i].terrain === 'mountain') continue;
        tiles[i].terrain = def.terrain[0];
        if (tiles[i].terrain === 'hill') tiles[i].reserve = STONE_RESERVE;
      }
      seedTile = i; break;
    }
    if (seedTile < 0) return null;
    const size = rng.int(def.size[0], def.size[1]);
    const cl = [seedTile];
    const frontier = [seedTile];
    while (cl.length < size && frontier.length) {
      const cur = frontier.shift()!;
      for (const n of rng.shuffle(g.neighbors(cur))) {
        if (cl.length >= size) break;
        if (cl.includes(n) || !ok(n) || tiles[n].sea) continue;
        if (!def.terrain.includes(tiles[n].terrain)) {
          if (tiles[n].terrain === 'water' || rng.chance(0.6)) continue;
          if (kind === 'fertile' || kind === 'horses') { if (tiles[n].terrain !== 'forest') continue; }
          tiles[n].terrain = def.terrain[0];
          if (tiles[n].terrain === 'hill' || tiles[n].terrain === 'mountain') tiles[n].reserve = STONE_RESERVE;
        }
        cl.push(n); frontier.push(n);
      }
    }
    const richness = rng.pick([0.7, 1, 1, 1.4]);
    const d: Deposit = { id: nextId++, kind, tiles: cl, richness, depleted: false, knownBy: [] };
    for (const t of cl) { tiles[t].deposit = d.id; tiles[t].reserve = def.reserve ? Math.round(def.reserve * richness) : 0; }
    deposits.push(d);
    return d;
  };
  // garantiler
  chosen.forEach((cls, k) => {
    const s = starts[k];
    place('fertile', s, 1, 2);
    place('clay', s, 2, 3);
    if (cls === 'cleric') place('copper', s, 4, 8);
    if (cls === 'barbarian') place('horses', s, 3, 7);
    if (cls === 'rogue') place('salt', s, 4, 8);
  });
  for (const kind of Object.keys(DEPOSITS) as DepositKind[]) {
    const def = DEPOSITS[kind];
    const already = deposits.filter((d) => d.kind === kind).length;
    for (let k = already; k < Math.round(def.count * AREA_K); k++) place(kind);
  }
  // Kadim ormanlar: kümeleri yatak yap
  const seen = new Set<number>();
  for (let i = 0; i < tiles.length; i++) {
    if (tiles[i].terrain !== 'oldforest' || seen.has(i) || tiles[i].deposit >= 0) continue;
    const blob: number[] = [];
    const q = [i];
    seen.add(i);
    while (q.length) {
      const c = q.pop()!;
      blob.push(c);
      for (const n of g.neighbors(c)) if (!seen.has(n) && tiles[n].terrain === 'oldforest' && tiles[n].deposit < 0) { seen.add(n); q.push(n); }
    }
    if (blob.length < 3) { for (const t of blob) tiles[t].terrain = 'forest'; continue; }
    const d: Deposit = { id: nextId++, kind: 'heartwood', tiles: blob.slice(0, 8), richness: 1, depleted: false, knownBy: [] };
    for (const t of d.tiles) { tiles[t].deposit = d.id; tiles[t].reserve = DEPOSITS.heartwood.reserve; }
    deposits.push(d);
  }
  // Druid'e kadim orman garantisi
  const di = chosen.indexOf('druid');
  if (di >= 0 && !deposits.some((d) => d.kind === 'heartwood' && d.tiles.some((t) => g.dist(t, starts[di]) <= 7))) {
    const cands = g.within(starts[di], 6).filter((t) => g.dist(t, starts[di]) >= 3 && tiles[t].deposit < 0 && tiles[t].terrain !== 'water' && tiles[t].terrain !== 'mountain');
    const seedT = rng.pick(cands);
    const blob = [seedT, ...g.neighbors(seedT).filter((n) => tiles[n].deposit < 0 && tiles[n].terrain !== 'water' && !starts.includes(n)).slice(0, 3)];
    const d: Deposit = { id: nextId++, kind: 'heartwood', tiles: blob, richness: 1, depleted: false, knownBy: [] };
    for (const t of blob) { tiles[t].terrain = 'oldforest'; tiles[t].wood = WOOD_RESERVE; tiles[t].deposit = d.id; tiles[t].reserve = DEPOSITS.heartwood.reserve; }
    deposits.push(d);
  }

  const civs: Civ[] = [];
  const settlements: Settlement[] = [];
  chosen.forEach((cls, i) => {
    civs.push(makeCiv(i, cls, 0));
    settlements.push(makeSettlement(nextId++, i, CLASSES[cls].capital, starts[i], { [CLASSES[cls].race]: 6 }, 0));
  });

  const camps: Camp[] = [];
  for (let k = 0; k < 3; k++) {
    const t = pickCampTile(g, tiles, rng, [...starts, ...camps.map((c) => c.tile)], 11);
    if (t >= 0) camps.push(makeCamp(nextId++, 'goblin', t, rng.pick(GOBLIN_CAMP_NAMES), 0, rng));
  }
  for (const c of camps) tiles[c.tile].camp = c.id;

  // tarafsız hanlar dünya kurulurken yoktur: oyun başlayınca hancılar yerleşimlerden öküz arabasıyla çıkıp
  // sınır bölgelerinde kendileri kurar (innlife.ts). Kaç hancının yola çıkacağı burada belirlenir.
  const innTarget = Math.max(2, Math.min(4, starts.length - 2));

  // adaların hazinesi: ayrı rastgele akışla (anakara ve eski dünyalar değişmez); kolonileşmeye değer adalar
  // denizler: seed'e göre kıta (birkaç ada), takımada ya da büyük adalar; ayrı rastgele akışla eklenir
  const sea = addIsles(g, tiles, seed);
  enrichIsles(g, tiles, deposits, seed);
  // dağ kütleleri geçilmez: bir kara parçasını ikiye bölen dağda en ucuz yerden geçit açılır
  const passes = carvePasses(g, tiles);

  const relations = civs.map(() => civs.map(() => emptyRel()));
  return { seed, day: 0, W, H, tiles, deposits, civs, settlements, heroes: [], camps, inns: [], innPlan: { target: innTarget, wave: innTarget, next: 2 }, quests: [], agents: [], routes: [], relations, events: [], battles: [], nextId, rngState: rng.state(), metrics: { mountainPass: passes }, isles: sea.isles, seaProfile: sea.profile };
}

export const GOBLIN_CAMP_NAMES = ['Kırıkdiş Kampı', 'Çürükpençe Kampı', 'Kanlıkaya Kampı', 'Paslıkılıç Kampı', 'Kemikçatal Kampı', 'Karagöz Kampı', 'Kurtkulak Kampı', 'Sümüklüdere Kampı'];
export const HOB_NAMES = ['Demirtoynak Karakolu', 'Kızılsancak Karakolu', 'Kara Lejyon Karakolu', 'Paslızırh Karakolu', 'Kanlıbayrak Karakolu', 'Demirçene Karakolu', 'Külrengi Karakolu', 'Kırbaç Karakolu'];
export const BUGBEAR_NAMES = ['Sessizpençe İni', 'Kıllıgölge İni'];

export function pickCampTile(g: HexGrid, tiles: Tile[], rng: Rng, avoid: number[], minD: number): number {
  let best = -1, bs = -Infinity;
  for (let k = 0; k < 600; k++) {
    const i = rng.int(0, tiles.length - 1);
    const t = tiles[i];
    if (t.terrain === 'water' || t.terrain === 'mountain' || t.owner >= 0 || t.camp !== undefined || t.ext || t.isle || t.innZone !== undefined) continue;
    const d = avoid.length ? Math.min(...avoid.map((a) => g.dist(a, i))) : 20;
    if (d < minD) continue;
    const sc = -Math.abs(d - minD - 3) + (t.terrain === 'forest' || t.terrain === 'hill' ? 3 : 0) + rng.next() * 2;
    if (sc > bs) { bs = sc; best = i; }
  }
  return best;
}

export function makeCamp(id: number, kind: Camp['kind'], tile: number, name: string, day: number, rng: Rng): Camp {
  const count = kind === 'goblin' ? 5 : kind === 'hobgoblin' ? 7 : 3;
  return { id, kind, tile, name, count, boss: kind === 'hobgoblin', hadBoss: kind === 'hobgoblin', loot: 10, growthAcc: 0, alive: true, nextRaid: day + rng.int(kind === 'goblin' ? 360 : 150, 460), founded: day };
}

export function makeCiv(id: number, cls: ClassId, day: number): Civ {
  const c = CLASSES[cls];
  return {
    id, cls, name: c.civName, race: c.race, align: { ...c.align }, color: c.color,
    stock: { grain: 25, meat: 10, wood: 20, gold: 5 }, price: {}, want: {},
    research: { current: null, progress: 0, done: [], reason: '' },
    era: 1, eraDay: [0, day], subclass: null, eff: { ...c.base }, alive: true, founded: day, threat: 0, lastRaidedDay: -9999, scoutSent: false,
    stats: { peakPop: 6, battlesWon: 0, battlesLost: 0, traded: 0, mined: {}, depleted: 0 }, lastExpand: -9999, yearly: {}, history: [],
  };
}

export function makeSettlement(id: number, civ: number, name: string, tile: number, pop: Settlement['pop'], day: number): Settlement {
  return { id, civ, name, tile, founded: day, pop: { ...pop }, growthAcc: 0, civics: { hut: 1 }, workshops: {}, project: null, jobs: {}, soldiers: 0, alive: true, starving: 0, tier: 0, mixedSince: {} };
}

export function markInn(g: HexGrid, tiles: Tile[], inn: Inn) {
  tiles[inn.tile].inn = inn.id;
  tiles[inn.tile].road = Math.max(1, tiles[inn.tile].road);
  for (const t of g.within(inn.tile, 1)) tiles[t].innZone = inn.id;
}

/**
 * Her ada (≥5 karo) bir yatak taşır; büyük adalar bazen iki. Değerli türler ağır basar.
 * Arazi değiştirilmez ve kimlikler ayrı aralıktan verilir: anakaranın erken tarihi eski dünyalarla aynı kalır.
 */
function enrichIsles(g: HexGrid, tiles: Tile[], deposits: Deposit[], seed: number): void {
  const rng = new Rng((seed * 2654435761) ^ 0x51ed2701);
  let id = 900000;
  const isles = new Map<number, number[]>();
  tiles.forEach((t, i) => { if (t.isle && t.terrain !== 'water') { const a = isles.get(t.isle) ?? []; a.push(i); isles.set(t.isle, a); } });
  const KINDS: [DepositKind, number][] = [['gold', 3], ['silver', 2], ['salt', 3], ['herbs', 3], ['horses', 2], ['iron', 2], ['copper', 2], ['fertile', 2], ['coal', 1], ['mana', 1]];
  for (const [, ts] of [...isles.entries()].sort((a, b) => a[0] - b[0])) {
    if (ts.length < 5) continue;
    const have = new Set(ts.map((i) => tiles[i].deposit).filter((d) => d >= 0)).size;
    const want = (ts.length >= 40 ? rng.int(2, 3) : ts.length >= 18 && rng.chance(0.5) ? 2 : 1) - have;
    for (let k = 0; k < want; k++) {
      const fits = KINDS.filter(([kind]) => ts.some((i) => tiles[i].deposit < 0 && DEPOSITS[kind].terrain.includes(tiles[i].terrain)));
      if (!fits.length) break;
      const kind = rng.weighted(fits, ([, w]) => w)![0];
      const def = DEPOSITS[kind];
      const free = ts.filter((i) => tiles[i].deposit < 0 && def.terrain.includes(tiles[i].terrain));
      const seedT = rng.pick(free);
      const size = Math.min(free.length, rng.int(def.size[0], Math.max(def.size[0], def.size[1] - 1)));
      const cl = [seedT];
      for (const n of rng.shuffle(g.within(seedT, 2))) { if (cl.length >= size) break; if (n !== seedT && free.includes(n)) cl.push(n); }
      const richness = rng.pick([1, 1, 1.4]);
      const d: Deposit = { id: id++, kind, tiles: cl, richness, depleted: false, knownBy: [] };
      for (const t of cl) { tiles[t].deposit = d.id; if (def.reserve) tiles[t].reserve = Math.round(def.reserve * richness); }
      deposits.push(d);
    }
  }
}

// ---- adalar ----
export const ISLE_TR: Record<IsleKind, string> = { volkan: 'volkanik ada', orman: 'ormanlık ada', cayir: 'çayırlık ada', kayalik: 'kayalık ada', bataklik: 'sisli bataklık adası', kumsal: 'kumsal adacık' };
export const SEA_PROFILE_TR = { kita: 'kıta ve birkaç ada', takimada: 'takımadalar', buyuk: 'büyük adalar' } as const;
const ISLE_NAMES: Record<IsleKind, string[]> = {
  volkan: ['Dumanlı Ada', 'Ateş Adası', 'Kızıl Ada', 'Kül Adası', 'Kara Ada', 'Ejder Adası', 'Kükürt Adası', 'Kor Adası', 'Obsidyen Adası'],
  orman: ['Yeşil Ada', 'Çam Adası', 'Meşe Adası', 'Gölgeli Ada', 'Kuş Adası', 'Sarmaşık Adası', 'Porsuk Adası', 'Yosunlu Ada', 'Geyik Adası'],
  cayir: ['Keçi Adası', 'Rüzgârlı Ada', 'Çiçekli Ada', 'At Adası', 'Uzun Ada', 'Bereket Adası', 'Arı Adası', 'Kuzu Adası', 'Gelincik Adası', 'Sarı Ada'],
  kayalik: ['Taşlı Ada', 'Kartal Adası', 'Martı Adası', 'Tuzlu Ada', 'Yalnız Ada', 'Fırtına Adası', 'Kayalı Ada', 'Karabatak Adası', 'Dişli Ada'],
  bataklik: ['Sisli Ada', 'Sazlı Ada', 'Hayalet Adası', 'Yılan Adası', 'Kurbağa Adası', 'Çürük Ada', 'Bataklı Ada', 'Sivrisinek Adası'],
  kumsal: ['Kaplumbağa Adası', 'İnci Adası', 'Mercan Adası', 'Balina Adası', 'Ay Adası', 'Kemik Adası', 'Midye Adası', 'Yengeç Adası', 'Deniz Kızı Adası'],
};

/** kıyıdan uzaklık (karada: en yakın denize), BFS */
function coastDepth(g: HexGrid, tiles: Tile[], ts: number[]): Map<number, number> {
  const set = new Set(ts), d = new Map<number, number>(), q: number[] = [];
  for (const i of ts) if (g.neighbors(i).some((n) => !set.has(n))) { d.set(i, 0); q.push(i); }
  for (let h = 0; h < q.length; h++) { const i = q[h]; for (const n of g.neighbors(i)) if (set.has(n) && !d.has(n)) { d.set(n, d.get(i)! + 1); q.push(n); } }
  return d;
}

/**
 * Anakara dışına ada ekler ve bütün adalara ad/tür verir. Profil seed'e göre:
 * kıta (birkaç orta ada), takımada (kümelenmiş çok ada), büyük adalar (2–3 geniş ada).
 * Yeni adalar mevcut karaya en az 2 deniz karosu uzak durur; eski adaların arazisi değişmez.
 */
function addIsles(g: HexGrid, tiles: Tile[], seed: number): { profile: 'kita' | 'takimada' | 'buyuk'; isles: IsleInfo[] } {
  const rng = new Rng((seed * 40503 + 17) ^ 0x2c1b3c6d);
  const W = g.W, H = g.H;
  const p0 = rng.next();
  const profile = p0 < 0.4 ? 'kita' : p0 < 0.75 ? 'takimada' : 'buyuk';
  const edgeOk = (i: number) => Math.min(g.col(i), g.row(i), W - 1 - g.col(i), H - 1 - g.row(i)) >= 3;
  const landDist = () => {
    const d = new Int16Array(tiles.length).fill(99), q: number[] = [];
    for (let i = 0; i < tiles.length; i++) if (!tiles[i].sea) { d[i] = 0; q.push(i); }
    for (let h = 0; h < q.length; h++) { const i = q[h]; if (d[i] >= 12) continue; for (const n of g.neighbors(i)) if (d[n] > d[i] + 1) { d[n] = d[i] + 1; q.push(n); } }
    return d;
  };
  let nextIsle = Math.max(0, ...tiles.map((t) => t.isle ?? 0)) + 1;
  const tundraRows = Math.round(H * 0.1);
  const place = (seedTile: number, size: number, gap: number): number[] | null => {
    const ld = landDist();
    if (!tiles[seedTile].sea || !edgeOk(seedTile) || ld[seedTile] < gap + 1) return null;
    const blob = [seedTile], inB = new Set(blob);
    for (let tries = 0; blob.length < size && tries < size * 30; tries++) {
      const from = blob[Math.floor(rng.next() * blob.length)];
      const n = rng.pick(g.neighbors(from));
      if (inB.has(n) || !tiles[n].sea || !edgeOk(n) || ld[n] < gap) continue;
      // yuvarlak değil, girintili: kalabalık tarafa ek zorlaşır
      if (g.neighbors(n).filter((q) => inB.has(q)).length >= 4 && rng.chance(0.5)) continue;
      blob.push(n); inB.add(n);
    }
    if (blob.length < 5) return null;
    const id = nextIsle++;
    for (const i of blob) tiles[i] = { ...tiles[i], sea: undefined, terrain: 'grass', elev: 0.34, isle: id, owner: -1, road: 0, deposit: -1, reserve: 0, wood: 0 };
    return blob;
  };
  const kindFor = (size: number): IsleKind => {
    if (size <= 8 && rng.chance(0.6)) return 'kumsal';
    const opts: [IsleKind, number][] = size >= 30 ? [['volkan', 3], ['orman', 2.5], ['cayir', 2.5], ['kayalik', 1], ['bataklik', 1]] : [['volkan', 2], ['orman', 2.5], ['cayir', 2], ['kayalik', 2], ['bataklik', 1.5], ['kumsal', 1]];
    return rng.weighted(opts, ([, w]) => w)![0];
  };
  const shape = (blob: number[], kind: IsleKind): number | undefined => {
    const depth = coastDepth(g, tiles, blob);
    const maxD = Math.max(...blob.map((i) => depth.get(i) ?? 0));
    let peak: number | undefined;
    for (const i of blob) {
      const d = depth.get(i) ?? 0, r = rng.next(), t = tiles[i];
      let tr: Terrain = 'grass';
      switch (kind) {
        case 'volkan': tr = d === maxD && maxD >= 2 ? 'mountain' : d >= 1 ? (r < 0.6 ? 'hill' : r < 0.8 ? 'forest' : 'grass') : r < 0.25 ? 'hill' : 'grass'; break;
        case 'orman': tr = r < 0.75 || d >= 1 ? 'forest' : 'grass'; break;
        case 'cayir': tr = r < 0.8 ? 'grass' : 'forest'; break;
        case 'kayalik': tr = r < 0.6 || d >= 2 ? 'hill' : 'grass'; break;
        case 'bataklik': tr = r < 0.55 ? 'swamp' : r < 0.85 ? 'forest' : 'grass'; break;
        case 'kumsal': tr = d >= 1 && r < 0.3 ? 'forest' : 'grass'; break;
      }
      if (g.row(i) < tundraRows && (tr === 'grass' || tr === 'forest' || tr === 'swamp')) tr = 'tundra';
      t.terrain = tr;
      t.elev = Math.min(0.92, 0.3 + d * 0.1 + (tr === 'mountain' ? 0.3 : tr === 'hill' ? 0.12 : 0) + rng.next() * 0.04);
      if (tr === 'forest') t.wood = WOOD_RESERVE;
      if (tr === 'hill' || tr === 'mountain') t.reserve = STONE_RESERVE;
      if (tr === 'mountain' && (peak === undefined || t.elev > tiles[peak].elev)) peak = i;
    }
    return peak;
  };
  const made: { blob: number[]; kind: IsleKind; peak?: number }[] = [];
  const add = (seedTile: number, size: number, gap: number) => {
    const blob = place(seedTile, size, gap);
    if (!blob) return false;
    const kind = kindFor(blob.length);
    made.push({ blob, kind, peak: shape(blob, kind) });
    return true;
  };
  const randSea = (minLd: number) => {
    const ld = landDist();
    let best = -1, bs = -1;
    for (let k = 0; k < 300; k++) { const i = rng.int(0, tiles.length - 1); if (!tiles[i].sea || !edgeOk(i) || ld[i] < minLd) continue; const sc = Math.min(ld[i], 9) + rng.next() * 3; if (sc > bs) { bs = sc; best = i; } }
    return best;
  };
  if (profile === 'kita') {
    for (let k = rng.int(1, 3); k > 0; k--) { const t = randSea(5); if (t >= 0) add(t, rng.int(8, 22), 3); }
  } else if (profile === 'takimada') {
    for (let cl = rng.int(2, 3); cl > 0; cl--) {
      const c0 = randSea(5);
      if (c0 < 0) continue;
      for (let k = rng.int(3, 5), tries = 0; k > 0 && tries < 40; tries++) {
        const near = g.within(c0, 7).filter((i) => tiles[i].sea);
        if (!near.length) break;
        if (add(rng.pick(near), rng.int(5, 26), 2)) k--;
      }
    }
  } else {
    for (let k = rng.int(2, 3); k > 0; k--) { const t = randSea(7); if (t >= 0) add(t, rng.int(40, 95), 3); }
  }
  // bütün adalara ad ve tür (eski adaların arazisi olduğu gibi kalır; tür araziden okunur)
  const members = new Map<number, number[]>();
  tiles.forEach((t, i) => { if (t.isle && !t.sea) { const a = members.get(t.isle) ?? []; a.push(i); members.set(t.isle, a); } });
  const used = new Set<string>();
  const isles: IsleInfo[] = [];
  for (const [id, ts] of [...members.entries()].sort((a, b) => a[0] - b[0])) {
    const m = made.find((x) => tiles[x.blob[0]].isle === id);
    let kind: IsleKind;
    if (m) kind = m.kind;
    else {
      const cnt = (tr: Terrain[]) => ts.filter((i) => tr.includes(tiles[i].terrain)).length / ts.length;
      kind = ts.length <= 6 ? 'kumsal' : cnt(['mountain']) > 0 ? 'volkan' : cnt(['hill']) > 0.4 ? 'kayalik' : cnt(['swamp']) > 0.3 ? 'bataklik' : cnt(['forest', 'oldforest']) > 0.45 ? 'orman' : 'cayir';
    }
    const list = rng.shuffle(ISLE_NAMES[kind].slice());
    const name = list.find((n) => !used.has(n)) ?? `Küçük ${list[0]}`;
    used.add(name);
    let cx = 0, cz = 0;
    for (const i of ts) { const [x, z] = g.pixel(i, 1); cx += x; cz += z; }
    cx /= ts.length; cz /= ts.length;
    const center = ts.slice().sort((a, b) => { const [ax, az] = g.pixel(a, 1), [bx, bz] = g.pixel(b, 1); return (ax - cx) ** 2 + (az - cz) ** 2 - ((bx - cx) ** 2 + (bz - cz) ** 2); })[0];
    const peak = m?.peak ?? (kind === 'volkan' ? ts.slice().sort((a, b) => tiles[b].elev - tiles[a].elev)[0] : undefined);
    isles.push({ id, name, kind, size: ts.length, center, peak });
  }
  return { profile, isles };
}

/**
 * Dağlar geçilmez (MOVE_COST = ∞). Bir kara parçasında dağların kapattığı her cep, en az dağ karosu
 * aşan yoldan ana bölgeye bağlanır; aşılan dağ karoları tepeye (geçit) döner. Yatak karoları son çare.
 * Rastgelelik kullanmaz: aynı seed aynı geçitleri verir.
 */
export function carvePasses(g: HexGrid, tiles: Tile[]): number {
  const n = tiles.length;
  const open = (i: number) => !tiles[i].sea && tiles[i].terrain !== 'mountain';
  const land = (i: number) => (tiles[i].sea ? -1 : tiles[i].isle ?? 0);
  let carved = 0;
  for (let guard = 0; guard < 200; guard++) {
    const comp = new Int32Array(n).fill(-1);
    const info: { land: number; size: number; seed: number }[] = [];
    for (let i = 0; i < n; i++) {
      if (!open(i) || comp[i] >= 0) continue;
      const id = info.length, st = [i];
      comp[i] = id; let size = 0;
      while (st.length) { const c = st.pop()!; size++; for (const q of g.neighbors(c)) if (comp[q] < 0 && open(q) && land(q) === land(i)) { comp[q] = id; st.push(q); } }
      info.push({ land: land(i), size, seed: i });
    }
    // her kara parçasının en büyük bölgesi ana bölgedir
    const main = new Map<number, number>();
    info.forEach((c, id) => { const m = main.get(c.land); if (m === undefined || info[m].size < c.size) main.set(c.land, id); });
    const orphan = info.map((c, id) => ({ c, id })).filter(({ c, id }) => main.get(c.land) !== id).sort((a, b) => b.c.size - a.c.size)[0];
    if (!orphan) break;
    // kova kuyruklu en kısa yol (Dial): açık karo 0, dağ 1, yataklı dağ 4
    const target = main.get(orphan.c.land)!;
    const dist = new Int32Array(n).fill(1 << 30), prev = new Int32Array(n).fill(-1);
    const buckets: number[][] = [[]];
    for (let i = 0; i < n; i++) if (comp[i] === orphan.id) { dist[i] = 0; buckets[0].push(i); }
    let hit = -1;
    for (let d = 0; d < buckets.length && hit < 0; d++) {
      const b = buckets[d];
      if (!b) continue;
      for (let k = 0; k < b.length && hit < 0; k++) {
        const c = b[k];
        if (dist[c] !== d) continue;
        if (comp[c] === target) { hit = c; break; }
        for (const nb of g.neighbors(c)) {
          if (land(nb) !== orphan.c.land) continue;
          const nd = d + (open(nb) ? 0 : tiles[nb].deposit >= 0 ? 4 : 1);
          if (nd < dist[nb]) { dist[nb] = nd; prev[nb] = c; (buckets[nd] ??= []).push(nb); }
        }
      }
    }
    if (hit < 0) break;
    for (let c = hit; c >= 0 && comp[c] !== orphan.id; c = prev[c]) if (tiles[c].terrain === 'mountain') { tiles[c].terrain = 'hill'; tiles[c].pass = true; carved++; }
  }
  return carved;
}
