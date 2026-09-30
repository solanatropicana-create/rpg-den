// Denizcilik: kıyı suları, limanlar, kara + deniz yol bulma, gemi hesapları,
// deniz yolculukları (koloni, ticaret, sefer, keşif), fırtına ve deniz savaşı.
import type { Sim } from './sim';
import { ek } from './tr';
import { MinHeap } from './hex';
import { unit, resolveBattle, type Combatant } from './combat';
import { YEAR } from './sim';
import { civTroops, recordBattle, removeFromPop, tileOf, armyReturn, scoutSight, drawSoldiers, raidReturn } from './agents';
import { monsterSide } from './monsters';
import { campPower } from './heroes';
import { heroCombatant, powerOf } from './combat';
import { makeCamp, ISLE_TR } from './worldgen';
import { UNITS, RACES } from '../data/classes';
import type { Agent, Camp, Civ, Settlement } from './types';

export const EMBARK = 1.5;      // yükleme/indirme: yol maliyetine eklenir
export const SEA_STEP = 0.5;    // deniz karosunun yol bulma maliyeti (kara yolu kadar)
export const GALLEY = { name: 'Kadırga', hp: 26, ac: 13, atk: 5, dmg: [2, 8, 2] as [number, number, number], attacks: 2 };
export const COG = { name: 'Koga', hp: 18, ac: 11, atk: 1, dmg: [1, 4, 0] as [number, number, number] };

// ------------------------------------------------------------ harita
/** kıyı suyu: karaya komşu deniz karosu (tekneler yalnız burada yol alır) */
export function shoreWater(s: Sim): Uint8Array {
  if (s.shoreW) return s.shoreW;
  const w = s.w, out = new Uint8Array(w.tiles.length);
  for (let i = 0; i < w.tiles.length; i++) if (w.tiles[i].sea && s.g.neighbors(i).some((n) => !w.tiles[n].sea)) out[i] = 1;
  s.shoreW = out;
  return out;
}
export const isSea = (s: Sim, i: number) => !!s.w.tiles[i].sea;
export const onSea = (s: Sim, a: Agent) => isSea(s, tileOf(a));

/** yerleşimin tersane kurabileceği kıyı karosu (-1 yok) */
export function pickPort(s: Sim, st: Settlement): number {
  const w = s.w;
  let best = -1, bd = Infinity;
  for (const i of s.g.within(st.tile, s.radiusOf(st))) {
    const t = w.tiles[i];
    if (t.sea || t.terrain === 'water' || t.terrain === 'mountain' || (t.owner !== st.id && i !== st.tile)) continue;
    if (!s.g.neighbors(i).some((n) => w.tiles[n].sea)) continue;
    // liman surların dışında dursun: iki karo uzak kıyı en iyisi, merkez en son seçenek
    const dd = s.g.dist(i, st.tile);
    const d = [9, 2, 0, 1, 1.5, 2][dd] + (t.ext ? 1.5 : 0) + (t.deposit >= 0 ? 1 : 0);
    if (d < bd) { bd = d; best = i; }
  }
  return best;
}
/** limanın denize açılan komşusu */
export function portWater(s: Sim, port: number): number {
  const ns = s.g.neighbors(port).filter((n) => s.w.tiles[n].sea);
  return ns.sort((a, b) => s.g.neighbors(b).filter((q) => s.w.tiles[q].sea).length - s.g.neighbors(a).filter((q) => s.w.tiles[q].sea).length)[0] ?? -1;
}

// ------------------------------------------------------------ filo
export function ports(s: Sim, c: Civ) { return s.civSettlements(c).filter((x) => x.civics.shipyard && x.port !== undefined); }
export const isFleet = (a: Agent) => a.kind === 'ship' && (a.purpose === 'fleet' || a.purpose === 'fleet-back');
export function hullsOut(s: Sim, st: Settlement) { let n = 0; for (const a of s.w.agents) if (!a.dead && a.hull === st.id && !isFleet(a)) n++; return n; }
export function galleysOut(s: Sim, st: Settlement) { let n = 0; for (const a of s.w.agents) if (!a.dead && a.hull === st.id) n += a.galleys ?? 0; return n; }
export function freeHulls(s: Sim, st: Settlement) { return Math.max(0, (st.ships ?? 0) - hullsOut(s, st)); }
export function freeGalleys(s: Sim, st: Settlement) { return Math.max(0, (st.galleys ?? 0) - galleysOut(s, st)); }
export function fleet(s: Sim, c: Civ) { let ships = 0, galleys = 0; for (const x of s.civSettlements(c)) { ships += x.ships ?? 0; galleys += x.galleys ?? 0; } return { ships, galleys }; }
/** gemi adı: Gemicilik'e dek kıyı teknesi, sonra koga */
export function hullName(s: Sim, c: Civ, plural = false) { return s.has(c, 'shipbuilding') ? (plural ? 'kogalar' : 'koga') : (plural ? 'tekneler' : 'tekne'); }

// ------------------------------------------------------------ yol bulma
export interface NavOpts { embark: number[]; open: boolean; landOnly?: number[] }
/**
 * Kara + deniz A*: karada yürünür, yalnız `embark` karolarından (limanlar) gemiye binilir,
 * denizde kıyı suyu (Seyir ile açık deniz) izlenir, istenen yerde (ya da `landOnly`) karaya çıkılır.
 */
export function navPath(s: Sim, from: number, to: number, o: NavOpts): number[] | null {
  const key = `${from}:${to}:${o.open ? 1 : 0}:${o.embark.join(',')}:${o.landOnly?.join(',') ?? ''}`;
  const cache = s.navCache;
  if (cache.has(key)) return cache.get(key)!;
  const w = s.w, g = s.g, shore = shoreWater(s);
  const emb = new Set(o.embark), land = o.landOnly ? new Set(o.landOnly) : null;
  const gScore = new Map<number, number>(), came = new Map<number, number>(), closed = new Set<number>();
  const open = new MinHeap();
  gScore.set(from, 0); open.push(from, g.dist(from, to) * 0.5);
  let found = false;
  while (open.size) {
    const cur = open.pop()!;
    if (cur === to) { found = true; break; }
    if (closed.has(cur)) continue;
    closed.add(cur);
    const cs = !!w.tiles[cur].sea;
    for (const n of g.neighbors(cur)) {
      const ns = !!w.tiles[n].sea;
      let step: number;
      if (!cs && !ns) step = n === to ? Math.min(s.moveCost(n), 3) : s.moveCost(n);
      else if (!cs && ns) { if (!emb.has(cur) || (!o.open && !shore[n])) continue; step = SEA_STEP + EMBARK; }
      else if (cs && ns) { if (!o.open && !shore[n]) continue; step = SEA_STEP; }
      else { if (w.tiles[n].terrain === 'mountain' || (land && !land.has(n))) continue; step = Math.min(s.moveCost(n), 3) + EMBARK; }
      if (!isFinite(step)) continue;
      const t = gScore.get(cur)! + step;
      if (t < (gScore.get(n) ?? Infinity)) { gScore.set(n, t); came.set(n, cur); open.push(n, t + g.dist(n, to) * 0.5); }
    }
  }
  let path: number[] | null = null;
  if (found) { path = [to]; let c = to; while (came.has(c)) { c = came.get(c)!; path.push(c); } path.reverse(); }
  if (cache.size > 3000) cache.clear();
  cache.set(key, path);
  return path;
}
export const hasSea = (s: Sim, p: number[]) => p.some((t) => s.w.tiles[t].sea);
/** yolun ilk bindiği liman yerleşimi */
export function embarkPort(s: Sim, c: Civ, p: number[]): Settlement | undefined {
  const k = p.findIndex((t) => s.w.tiles[t].sea);
  if (k <= 0) return undefined;
  const t = p[k - 1];
  return ports(s, c).find((x) => x.port === t);
}
/** medeniyet için kara/deniz yolu: boş gemisi olan limanlardan binilir */
export function civPath(s: Sim, c: Civ, from: number, to: number, o: { landOnly?: number[] } = {}): { path: number[]; hull?: Settlement } | null {
  const emb = ports(s, c).filter((x) => freeHulls(s, x) > 0).map((x) => x.port!);
  if (!s.has(c, 'boatbuilding') || !emb.length) { const p = s.path(from, to); return p ? { path: p } : null; }
  const p = navPath(s, from, to, { embark: emb, open: s.has(c, 'navigation'), landOnly: o.landOnly });
  if (!p) return null;
  if (!hasSea(s, p)) return { path: p };
  const hull = embarkPort(s, c, p);
  return hull ? { path: p, hull } : null;
}

// ------------------------------------------------------------ hareket
/** denizde günlük ilerleme (karo/gün) */
export function shipSpeed(s: Sim, a: Agent) {
  const c = a.civ >= 0 ? s.w.civs[a.civ] : undefined;
  if (a.monster === 'pirate') return 1.25; // hafif korsan kayıkları
  let v = 0.85;
  if (c && s.has(c, 'navigation')) v += 0.2;
  if (c && s.has(c, 'seatrade') && (a.kind === 'caravan' || a.kind === 'ship')) v += 0.1;
  if (a.kind === 'ship' && !a.purpose?.startsWith('explore')) v += 0.05;
  return v;
}
/** denizden karaya adım: gemi limanına döner ya da (ordu) kıyıda bekler */
export function disembark(s: Sim, a: Agent, seaTile: number, landTile: number) {
  a.landing = landTile;
  if (a.kind === 'ship') return;
  const home = a.hull !== undefined ? s.settlement(a.hull) : undefined;
  if (a.kind === 'army' && !a.returning) return; // kogalar kıyıda bekler, dönüşte yeniden binilir
  if (a.kind === 'army' && a.returning) { a.hull = undefined; a.galleys = undefined; return; }
  a.hull = undefined;
  if (!home || !home.alive || home.port === undefined) return;
  const c = s.w.civs[a.civ];
  const p = navPath(s, seaTile, home.port, { embark: [], open: s.has(c, 'navigation'), landOnly: [home.port] });
  if (!p || p.length < 2) return;
  s.w.agents.push({ id: s.id(), kind: 'ship', civ: a.civ, path: p, step: 0, progress: 0, speed: 1, hull: home.id, purpose: 'return' });
}
/** ordu dönüşü: karaya çıktığı yerden yeniden biner, kendi limanında iner */
export function seaReturnPath(s: Sim, a: Agent, to: number): number[] | null {
  if (a.hull === undefined || a.landing === undefined) return null;
  const c = s.w.civs[a.civ];
  const home = s.settlement(a.hull);
  const land = home && home.alive && home.civ === a.civ && home.port !== undefined ? [home.port] : undefined;
  return navPath(s, tileOf(a), to, { embark: [a.landing], open: s.has(c, 'navigation'), landOnly: land }) ?? navPath(s, tileOf(a), to, { embark: [a.landing], open: s.has(c, 'navigation') });
}

// ------------------------------------------------------------ günlük deniz olayları
export function seaTick(s: Sim) {
  const w = s.w;
  if (w.day % 30 === 7) for (const st of w.settlements) {
    if (!st.alive || !st.civics.shipyard) continue;
    const ok = st.port !== undefined && (w.tiles[st.port].owner === st.id || st.port === st.tile) && s.g.neighbors(st.port).some((n) => w.tiles[n].sea);
    if (!ok) { const p = pickPort(s, st); st.port = p >= 0 ? p : undefined; }
  }
  for (const a of w.agents) {
    if (a.dead || !onSea(s, a)) continue;
    if (storm(s, a)) continue;
    if ((a.kind === 'army' && !a.returning && (a.purpose === 'war' || a.purpose === 'plunder')) || (a.kind === 'ship' && a.purpose === 'fleet')) navalEncounter(s, a);
    if (!a.dead && isFleet(a)) intercept(s, a);
    if (!a.dead && a.kind === 'raid' && a.monster === 'pirate' && !a.returning) pirateSea(s, a);
  }
  if (w.day % 10 === 3) for (const c of w.civs) if (c.alive) considerSeaExplore(s, c);
  if (w.day % 10 === 6) for (const c of w.civs) if (c.alive) considerFleet(s, c);
  if (w.day % 10 === 8) for (const c of w.civs) if (c.alive) considerPirateHunt(s, c);
  if (w.day % YEAR === 60) pirateCoveTick(s);
  if (w.day % YEAR === 90) volcanoTick(s);
}

/** yanardağ adası: arada bir kül püskürür; adadaki koloninin evleri ve tarlaları zarar görür */
function volcanoTick(s: Sim) {
  for (const isl of s.w.isles ?? []) {
    if (isl.kind !== 'volkan' || isl.peak === undefined) continue;
    const st = s.w.settlements.find((x) => x.alive && s.w.tiles[x.tile].isle === isl.id);
    if (!st || !s.rng.chance(0.06)) continue;
    const burnt = s.rng.int(1, 2);
    st.burnedHouses = (st.burnedHouses ?? 0) + burnt; st.burnedAt = s.day;
    let fields = 0;
    for (const i of s.g.within(st.tile, s.radiusOf(st))) { const t = s.w.tiles[i]; if (t.ext && t.owner === st.id && t.ext.kind === 'farm' && s.extWorking(t) && fields < 2) { t.ext.burned = s.day + 60; t.ext.burnedAt = s.day; t.ext.workers = 0; fields++; } }
    s.metric('eruption');
    s.log('world', `${ek(isl.name, 'da')}ki yanardağ kül püskürdü: ${ek(st.name, 'da')} ${burnt} ev yandı${fields ? `, ${fields} tarla küle gömüldü` : ''}.`, { civ: st.civ, tile: isl.peak, major: true, cause: 'Volkanik ada; zengin topraklar, huysuz dağ' });
  }
}

const SEASON_STORM = [1, 0.7, 1.5, 2.6];
function storm(s: Sim, a: Agent): boolean {
  const w = s.w, here = tileOf(a);
  const c = a.civ >= 0 ? w.civs[a.civ] : undefined;
  if (!c) return false; // korsanlar suları bilir
  let p = 0.00012 * (shoreWater(s)[here] ? 0.6 : 1.8) * SEASON_STORM[s.season];
  if (c && s.has(c, 'navigation')) p *= 0.65;
  if (c && s.civSettlements(c).some((x) => x.civics.lighthouse && x.port !== undefined && s.g.dist(x.port, here) <= 8)) p *= 0.4;
  if (!s.rng.chance(p)) return false;
  const who = c ? c.name : '';
  s.metric('storm');
  if (a.kind === 'army') {
    const lost = Math.max(1, Math.floor((a.troops ?? 0) * s.rng.int(10, 30) / 100));
    a.troops = Math.max(0, (a.troops ?? 0) - lost); removeFromPop(s, a.pop, lost);
    let gl = 0;
    if ((a.galleys ?? 0) > 0 && s.rng.chance(0.4)) { gl = 1; a.galleys!--; const h = a.hull !== undefined ? s.settlement(a.hull) : undefined; if (h) h.galleys = Math.max(0, (h.galleys ?? 0) - 1); }
    s.log('sea', `Fırtına ${c ? ek(who, 'in') : 'bir'} donanmasını savurdu: ${lost} asker denize düştü${gl ? ', bir kadırga battı' : ''}.`, { civ: a.civ, tile: here, major: true, cause: ['Kış fırtınası', 'İlkbahar borası', 'Yaz sağanağı', 'Güz fırtınası'][s.season] });
    return false;
  }
  // tekil gemiler: çoğu hasarla kurtulur, beşte biri batar
  if (s.rng.chance(0.8)) {
    a.progress = Math.min(a.progress, 0) - 2;
    if (a.kind !== 'ship' && c) s.log('sea', `${ek(who, 'in')} gemisi fırtınaya yakalandı; direği kırıldı ama batmadı.`, { civ: a.civ, tile: here });
    return false;
  }
  const home = a.hull !== undefined ? s.settlement(a.hull) : undefined;
  if (home) home.ships = Math.max(0, (home.ships ?? 0) - 1);
  a.dead = true;
  s.metric('shipSunk');
  const what = a.kind === 'settlers' ? `${s.popSize(a.pop)} öncüyü taşıyan` : a.kind === 'caravan' ? 'yüklü' : a.purpose === 'explore' || a.purpose === 'explore-back' ? 'keşif' : 'boş dönen';
  s.log('sea', `${c ? ek(who, 'in') : 'Bir'} ${what} gemisi fırtınada battı.`, { civ: a.civ, tile: here, major: a.kind === 'settlers' || a.kind === 'caravan' || a.purpose?.startsWith('explore'), cause: `${['İlkbahar', 'Yaz', 'Güz', 'Kış'][s.season]} fırtınası${shoreWater(s)[here] ? '' : ', açık denizde'}` });
  return true;
}

/** saldıran donanma düşman limanına yaklaşınca kadırgalar çıkar */
function navalEncounter(s: Sim, a: Agent) {
  const w = s.w, here = tileOf(a), att = w.civs[a.civ];
  for (const st of w.settlements) {
    if (!st.alive || st.civ === a.civ || st.port === undefined || !(st.galleys ?? 0)) continue;
    if (a.fought?.includes(st.id)) continue;
    if (!s.atWar(a.civ, st.civ) && !(a.purpose === 'plunder' && st.id === a.to)) continue;
    if (s.g.dist(st.port, here) > 3) continue;
    const free = freeGalleys(s, st);
    if (!free) continue;
    (a.fought ??= []).push(st.id);
    const dfc = w.civs[st.civ];
    const fleetOnly = a.kind === 'ship';
    const A: Combatant[] = [];
    for (let i = 0; i < (a.galleys ?? 0); i++) A.push(unit(GALLEY, 'A', 'galley'));
    if (!fleetOnly) { A.push(unit(COG, 'A', 'cog')); A.push(...civTroops(s, att, Math.min(6, a.troops ?? 0), 'A')); }
    const B: Combatant[] = [];
    for (let i = 0; i < Math.min(4, free); i++) B.push(unit(GALLEY, 'B', 'galley'));
    B.push(...civTroops(s, dfc, Math.min(3, st.soldiers), 'B'));
    const b = resolveBattle(s.rng, A, B, { id: s.id(), day: s.day, tile: here, title: `${st.name} açıklarında deniz savaşı`, sideA: `${att.name} donanması`, sideB: `${st.name} kadırgaları`, moraleA: 0.5, moraleB: 0.55, maxRounds: 12, civA: att.id, civB: dfc.id });
    b.naval = true;
    recordBattle(s, b);
    s.metric('navalBattle');
    const deadG = A.filter((x) => x.kind === 'galley' && x.hp <= 0).length;
    const cogSunk = A.some((x) => x.kind === 'cog' && x.hp <= 0);
    const deadM = A.filter((x) => (x.kind === 'soldier' || x.kind === 'unique') && x.hp <= 0).length;
    const deadBG = B.filter((x) => x.kind === 'galley' && x.hp <= 0).length;
    const deadBM = B.filter((x) => (x.kind === 'soldier' || x.kind === 'unique') && x.hp <= 0).length;
    const home = a.hull !== undefined ? s.settlement(a.hull) : undefined;
    if (home) home.galleys = Math.max(0, (home.galleys ?? 0) - deadG);
    a.galleys = Math.max(0, (a.galleys ?? 0) - deadG);
    let drowned = deadM;
    if (cogSunk) { drowned += Math.floor(((a.troops ?? 0) - deadM) * 0.5); if (home) home.ships = Math.max(0, (home.ships ?? 0) - 1); }
    a.troops = Math.max(0, (a.troops ?? 0) - drowned); removeFromPop(s, a.pop, drowned);
    st.galleys = Math.max(0, (st.galleys ?? 0) - deadBG);
    if (deadBM) { st.soldiers = Math.max(0, st.soldiers - deadBM); s.removePop(st, deadBM); }
    if (b.winner === 'A') {
      att.stats.battlesWon++; dfc.stats.battlesLost++;
      s.log('sea', `${att.name} donanması ${st.name} açıklarında ${dfc.name} kadırgalarını yendi${deadBG ? `: ${deadBG} kadırga battı` : ''}.`, { civ: att.id, tile: here, battle: b.id, major: true, cause: fleetOnly ? `${a.galleys ?? 0} kadırga limana doğru ilerliyor` : `${a.galleys ?? 0} kadırga, ${a.troops} asker karaya çıkmaya devam ediyor` });
    } else {
      att.stats.battlesLost++; dfc.stats.battlesWon++;
      s.log('sea', `${st.name} kadırgaları ${att.name} donanmasını geri püskürttü${cogSunk ? '; bir asker kogası battı' : ''}.`, { civ: dfc.id, tile: here, battle: b.id, major: true, cause: fleetOnly ? `${deadG} kadırga kayıp` : `${drowned} asker ve ${deadG} kadırga kayıp` });
      if (fleetOnly) { if (!a.galleys) a.dead = true; else fleetHome(s, a); } else armyReturn(s, a);
      return;
    }
    if (fleetOnly) { if (!a.galleys) a.dead = true; continue; }
    if ((a.troops ?? 0) <= 0 && !(a.heroes?.length)) { a.dead = true; return; }
  }
}

// ------------------------------------------------------------ keşif gemisi
export function considerSeaExplore(s: Sim, c: Civ) {
  if (c.seaScout || !s.has(c, 'navigation')) return;
  const port = ports(s, c).find((x) => freeHulls(s, x) > 0);
  if (!port) return;
  const w = s.w, g = s.g;
  const start = portWater(s, port.port!);
  if (start < 0) return;
  // açık denizde en uzak, adalara ve bilinmeyen kıyılara yakın bir noktaya
  const dist = new Map<number, number>([[start, 0]]), par = new Map<number, number>();
  const q = [start];
  for (let h = 0; h < q.length; h++) {
    const i = q[h], d = dist.get(i)!;
    if (d >= 42) continue;
    for (const n of g.neighbors(i)) if (w.tiles[n].sea && !dist.has(n)) { dist.set(n, d + 1); par.set(n, i); q.push(n); }
  }
  const own = s.civSettlements(c);
  let goal = -1, bs = -Infinity;
  for (const [i, d] of dist) {
    if (d < 14) continue;
    let sc = Math.min(d, 34) + s.rng.next() * 4;
    for (const n of g.within(i, 3)) { const t = w.tiles[n]; if (t.isle && !own.some((x) => x.tile === n)) sc += 1.2; }
    if (own.some((x) => g.dist(x.tile, i) < 8)) sc -= 10;
    if (sc > bs) { bs = sc; goal = i; }
  }
  if (goal < 0) return;
  const path: number[] = [goal];
  let cur = goal; while (par.has(cur)) { cur = par.get(cur)!; path.push(cur); }
  path.push(port.port!);
  path.reverse();
  c.seaScout = true;
  w.agents.push({ id: s.id(), kind: 'ship', civ: c.id, path, step: 0, progress: 0, speed: 1, hull: port.id, purpose: 'explore' });
  s.metric('seaExplore');
  s.log('sea', `${c.name} denizcileri ${ek(port.name, 'dan')} ufkun ötesine yelken açtı.`, { civ: c.id, tile: port.port, major: true, cause: 'Seyir: yıldızlarla yön bulmak' });
}
/** keşif gemisinin gözü: kıyılar, adalar, yataklar, uzak halklar */
export function exploreSight(s: Sim, a: Agent) {
  scoutSight(s, a);
  const c = s.w.civs[a.civ], here = tileOf(a);
  for (const n of s.g.within(here, 4)) {
    const id = s.w.tiles[n].isle;
    if (!id || c.yearly['isle' + id]) continue;
    c.yearly['isle' + id] = 1;
    const isl = s.w.isles?.find((x) => x.id === id);
    const size = isl?.size ?? s.w.tiles.filter((t) => t.isle === id).length;
    s.log('sea', isl ? `${ek(c.name, 'in')} keşif gemisi ${ek(isl.name, 'i')} gördü: ${ISLE_TR[isl.kind]}, ${size} karo.` : `${c.name} keşif gemisi uzak bir ada gördü (${size} karo).`, { civ: c.id, tile: n, major: size >= 10 });
    s.metric('isleSeen');
  }
}
/** keşif gemisi hedefe varınca limana döner */
export function exploreTurn(s: Sim, a: Agent): boolean {
  const home = a.hull !== undefined ? s.settlement(a.hull) : undefined;
  if (!home || !home.alive || home.port === undefined) return true;
  const c = s.w.civs[a.civ];
  const p = navPath(s, tileOf(a), home.port, { embark: [], open: s.has(c, 'navigation'), landOnly: [home.port] });
  if (!p) return true;
  a.path = p; a.step = 0; a.progress = 0; a.purpose = 'explore-back';
  return false;
}


// ------------------------------------------------------------ savaş filoları
/** savaşta kadırgalar düşman limanına akın eder: limandaki gemileri yakar, denizdeki gemilerini ele geçirir */
export function considerFleet(s: Sim, c: Civ) {
  if (!s.has(c, 'navy') || !s.inWar(c)) return;
  if (s.w.agents.some((a) => a.civ === c.id && isFleet(a))) return;
  const base = ports(s, c).filter((x) => freeGalleys(s, x) >= 2).sort((a, b) => freeGalleys(s, b) - freeGalleys(s, a))[0];
  if (!base) return;
  const w = s.w;
  let best: Settlement | undefined, bd = Infinity;
  for (const o of w.civs) {
    if (o.id === c.id || !o.alive || !s.atWar(c.id, o.id)) continue;
    if (s.day - (c.yearly['fleet' + o.id] ?? -9999) < 200) continue;
    for (const st of ports(s, o)) { const d = s.g.dist(st.port!, base.port!); if (d < bd && d <= (s.has(c, 'navigation') ? 50 : 30)) { bd = d; best = st; } }
  }
  if (!best) return;
  const goal = portWater(s, best.port!);
  if (goal < 0) return;
  const path = navPath(s, base.port!, goal, { embark: [base.port!], open: s.has(c, 'navigation') });
  if (!path || path.length < 3) return;
  const o = w.civs[best.civ];
  c.yearly['fleet' + o.id] = s.day;
  const n = Math.min(4, freeGalleys(s, base));
  w.agents.push({ id: s.id(), kind: 'ship', civ: c.id, path, step: 0, progress: 0, speed: 1, hull: base.id, galleys: n, purpose: 'fleet', to: best.id });
  s.metric('fleetSortie');
  s.log('sea', `${c.name} ${n} kadırgayla ${ek(base.name, 'dan')} ${ek(best.name, 'in')} limanına akına çıktı.`, { civ: c.id, tile: base.port, major: true, cause: `${o.name} ile savaş` });
}
/** filo hedef limana vardı: savunan kadırga kalmadıysa limandaki gemileri yakar */
export function fleetArrive(s: Sim, a: Agent): boolean {
  if (a.purpose === 'fleet-back') return true;
  const st = a.to !== undefined ? s.settlement(a.to) : undefined;
  const att = s.w.civs[a.civ];
  if (st && st.alive && st.civ !== a.civ && s.atWar(a.civ, st.civ)) {
    const dfc = s.w.civs[st.civ];
    const burn = Math.min(freeHulls(s, st), 1 + Math.floor((a.galleys ?? 1) / 2));
    const gold = Math.min(s.st(dfc, 'gold'), 8 + (a.galleys ?? 1) * 6);
    if (burn) st.ships = Math.max(0, (st.ships ?? 0) - burn);
    s.add(dfc, 'gold', -gold); s.add(att, 'gold', gold);
    s.metric('portRaid');
    s.log('sea', `${att.name} kadırgaları ${ek(st.name, 'in')} limanını ateşe verdi${burn ? `: ${burn} gemi yandı` : ''}${gold >= 1 ? `, ${Math.round(gold)} altınlık ganimet` : ''}.`, { civ: att.id, tile: st.port ?? st.tile, major: true, cause: 'Deniz akını' });
    if (st.port !== undefined) { st.burnedHouses = (st.burnedHouses ?? 0) + 1; st.burnedAt = s.day; }
  }
  return fleetHome(s, a);
}
function fleetHome(s: Sim, a: Agent): boolean {
  const home = a.hull !== undefined ? s.settlement(a.hull) : undefined;
  if (!home || !home.alive || home.civ !== a.civ || home.port === undefined) { a.dead = true; return true; }
  const p = navPath(s, tileOf(a), home.port, { embark: [], open: s.has(s.w.civs[a.civ], 'navigation'), landOnly: [home.port] });
  if (!p) { a.dead = true; return true; }
  a.path = p; a.step = 0; a.progress = 0; a.purpose = 'fleet-back';
  return false;
}
/** denizdeki düşman gemilerini ele geçirme */
function intercept(s: Sim, a: Agent) {
  const w = s.w, here = tileOf(a), att = w.civs[a.civ];
  for (const b of w.agents) {
    if (b.dead || b === a || b.civ < 0 || b.civ === a.civ || !s.atWar(a.civ, b.civ)) continue;
    if (!(b.kind === 'caravan' || b.kind === 'settlers' || (b.kind === 'ship' && !isFleet(b)))) continue;
    const bt = tileOf(b);
    if (!w.tiles[bt].sea || s.g.dist(bt, here) > 2) continue;
    const owner = w.civs[b.civ];
    const home = b.hull !== undefined ? s.settlement(b.hull) : undefined;
    if (home) home.ships = Math.max(0, (home.ships ?? 0) - 1);
    const hb = a.hull !== undefined ? s.settlement(a.hull) : undefined;
    if (hb && hb.alive) hb.ships = (hb.ships ?? 0) + 1; // ganimet gemi
    let loot = '';
    for (const g in b.cargo ?? {}) { s.add(att, g as never, b.cargo![g as never] ?? 0); loot = ' ve yükü'; }
    b.dead = true;
    s.metric('shipCaptured');
    s.log('sea', `${att.name} kadırgaları ${ek(owner.name, 'in')} ${b.kind === 'settlers' ? 'öncü gemisini' : 'gemisini'}${loot} ele geçirdi.`, { civ: att.id, tile: bt, major: true, cause: b.kind === 'settlers' ? `${s.popSize(b.pop)} öncü esir düştü` : 'Savaşta deniz yolları kesildi' });
    if (b.kind === 'settlers' && hb) s.mergePop(hb, b.pop);
  }
}

// ------------------------------------------------------------ adalar
export function isleOf(s: Sim, tile: number) { const id = s.w.tiles[tile].isle; return id ? s.w.isles?.find((x) => x.id === id) : undefined; }

// ------------------------------------------------------------ korsanlar
const COVE_NAMES = ['Kara Bayrak Koyu', 'Kanlı Çapa Koyu', 'Tuzlu Kurt Koyu', 'Kemik Sandık Koyu', 'Martı Mezarı Koyu', 'Paslı Kanca Koyu', 'Ölü Rüzgâr Koyu', 'Kör Korsan Koyu', 'Yırtık Yelken Koyu', 'Rom Fıçısı Koyu'];
const CAPTAINS = ['Tuzlusakal', 'Tek Göz Rıza', 'Kanca Elli Mira', 'Kızıl Bayrak Dursun', 'Kara Nesrin', 'Martı Hasan', 'Çapa Kerim', 'Fırtına Leyla', 'Paslı Tunç', 'Yarım Ay Selim'];
export const isPirate = (a: Agent) => a.kind === 'raid' && a.monster === 'pirate';
export function newCaptain(s: Sim) { return s.rng.pick(CAPTAINS.filter((x) => !s.w.camps.some((c) => c.alive && c.captain === x))) ?? CAPTAINS[0]; }
/** korsanlar Haydut'un gemilerine dokunmaz: ganimeti onların limanlarında satarlar */
const smugglers = (c: Civ) => c.cls === 'rogue';

/** Deniz ticareti canlanınca sahipsiz adalarda (yoksa ıssız kıyıda) korsan koyu kurulur */
function pirateCoveTick(s: Sim) {
  const w = s.w;
  if (s.year < 5) return;
  let ships = 0; for (const c of w.civs) if (c.alive) ships += fleet(s, c).ships;
  if (ships < 3) return;
  const coves = w.camps.filter((c) => c.alive && c.kind === 'pirate');
  const lastClear = Math.max(-9999, ...w.camps.filter((c) => c.kind === 'pirate' && !c.alive).map((c) => c.clearedDay ?? c.founded));
  if (s.day - lastClear < 2 * YEAR) return;
  const seaRoutes = w.routes.filter((r) => r.alive && r.sea).length;
  const max = (w.seaProfile === 'kita' ? 1 : 2) + (seaRoutes >= 6 ? 1 : 0);
  if (coves.length >= max || !s.rng.chance(0.4)) return;
  const setl = w.settlements.filter((x) => x.alive);
  const allPorts = setl.filter((x) => x.civics.shipyard && x.port !== undefined);
  if (!allPorts.length) return;
  let best = -1, bs = -Infinity;
  for (let i = 0; i < w.tiles.length; i++) {
    const t = w.tiles[i];
    if (t.sea || t.terrain === 'water' || t.terrain === 'mountain' || t.owner >= 0 || t.camp !== undefined || t.innZone !== undefined) continue;
    if (!s.g.neighbors(i).some((n) => w.tiles[n].sea)) continue;
    const dS = setl.length ? Math.min(...setl.map((x) => s.g.dist(x.tile, i))) : 99;
    if (dS < (t.isle ? 7 : 11)) continue;
    const dP = Math.min(...allPorts.map((x) => s.g.dist(x.port!, i)));
    if (dP > 34) continue;
    if (w.inns.some((inn) => s.g.dist(inn.tile, i) < 6)) continue;
    const isle = t.isle ? w.isles?.find((x) => x.id === t.isle) : undefined;
    let sc = (t.isle ? 6 : 0) + (isle?.kind === 'kumsal' ? 3 : 0) - Math.abs(dP - 16) * 0.3 + s.rng.next() * 3 + (t.deposit >= 0 ? -1 : 0);
    if (w.camps.some((c) => c.alive && s.g.dist(c.tile, i) < (c.kind === 'pirate' ? 12 : 6))) continue;
    if (sc > bs) { bs = sc; best = i; }
  }
  if (best < 0) return;
  const used = new Set(w.camps.map((c) => c.name));
  const name = COVE_NAMES.find((n) => !used.has(n)) ?? `${COVE_NAMES[0]} ${coves.length + 2}`;
  const cp = makeCamp(s.id(), 'pirate', best, name, s.day, s.rng);
  cp.count = 7; cp.boss = true; cp.hadBoss = true; cp.loot = 20;
  cp.captain = newCaptain(s);
  cp.nextRaid = s.day + s.rng.int(60, 120);
  w.camps.push(cp);
  w.tiles[best].camp = cp.id; w.tiles[best].owner = -1;
  const isle = isleOf(s, best);
  s.metric('pirateCove');
  s.log('lair', `${isle ? `${ek(isle.name, 'da')} ` : 'Issız bir kıyıda '}korsanlar ${ek(name, 'i')} kurdu; başlarında Kaptan ${cp.captain}.`, { tile: best, major: true, cause: `Deniz ticareti yağmacıları çekti (${ships} gemi, ${seaRoutes} deniz yolu)` });
}

/** korsan kayığı denize açılır: denizdeki sivil gemiyi avlar ya da iskele/kıyı kasabasına çıkar */
export function launchPirates(s: Sim, cp: Camp) {
  const w = s.w;
  cp.nextRaid = s.day + s.rng.int(80, 150);
  const n = Math.min(Math.ceil(cp.count * 0.6), 3 + Math.floor(s.year / 3), 9);
  const prey = w.agents.filter((a) => !a.dead && a.civ >= 0 && (a.kind === 'caravan' || a.kind === 'settlers' || (a.kind === 'ship' && !isFleet(a)))
    && onSea(s, a) && !smugglers(w.civs[a.civ]) && s.g.dist(tileOf(a), cp.tile) <= 30 && a.path.length - a.step > 4);
  const coast: { tile: number; st: Settlement; ext: boolean }[] = [];
  for (const st of w.settlements) {
    if (!st.alive || smugglers(w.civs[st.civ]) || s.g.dist(st.tile, cp.tile) > 26) continue;
    for (const i of s.g.within(st.tile, s.radiusOf(st))) {
      const t = w.tiles[i];
      if (t.owner !== st.id || !s.g.neighbors(i).some((q) => w.tiles[q].sea)) continue;
      if (t.ext?.kind === 'dock' && s.extWorking(t)) coast.push({ tile: i, st, ext: true });
    }
    if (s.g.within(st.tile, 1).some((q) => w.tiles[q].sea) && s.pop(st) >= 10) coast.push({ tile: st.tile, st, ext: false });
  }
  let path: number[] | null = null, purpose = '', to = -1, targetTile = -1;
  if (prey.length && (s.rng.chance(0.6) || !coast.length)) {
    const p = s.rng.weighted(prey, (a) => (a.kind === 'caravan' ? 3 : a.kind === 'settlers' ? 2 : 1) / (1 + (a.galleys ?? 0) * 3))!;
    const goal = aheadOf(s, p, Math.ceil(s.g.dist(cp.tile, tileOf(p)) * 0.9) + 2);
    if (goal >= 0) { path = navPath(s, cp.tile, goal, { embark: [cp.tile], open: true }); purpose = 'prey'; to = p.id; targetTile = goal; }
  } else if (coast.length) {
    const c = s.rng.weighted(coast, (x) => (x.ext ? 2 : 1) / (1 + x.st.soldiers * 0.6 + (x.st.galleys ?? 0) * 2) / (1 + s.g.dist(x.tile, cp.tile) * 0.08))!;
    path = navPath(s, cp.tile, c.tile, { embark: [cp.tile], open: true });
    purpose = c.ext ? 'ext' : 'settlement'; to = c.ext ? c.tile : c.st.id; targetTile = c.tile;
  }
  if (!path || !hasSea(s, path)) return;
  cp.count -= n;
  const boss = cp.boss && s.rng.chance(0.35);
  if (boss) cp.boss = false;
  w.agents.push({ id: s.id(), kind: 'raid', civ: -1, path, step: 0, progress: 0, speed: 1, troops: n, from: cp.id, to, purpose, boss, monster: 'pirate', targetTile });
  s.metric('pirateSortie');
}

/** avın rotasında n adım ötedeki deniz karosu (karşısına çıkmak için) */
function aheadOf(s: Sim, t: Agent, n: number): number {
  for (let k = Math.min(t.path.length - 1, t.step + n); k >= t.step; k--) if (s.w.tiles[t.path[k]].sea) return t.path[k];
  return -1;
}
/** denizdeki korsan: avını kovalar, yakalayınca saldırır; kıyıya giderken kadırgalara takılabilir */
function pirateSea(s: Sim, a: Agent) {
  const w = s.w, here = tileOf(a);
  const cp = w.camps.find((x) => x.id === a.from);
  if (a.purpose === 'prey') {
    const t = w.agents.find((x) => x.id === a.to && !x.dead);
    if (!t || !onSea(s, t)) { raidReturn(s, a); return; }
    a.chase ??= s.day;
    if (s.day - a.chase > 35) { raidReturn(s, a); return; }
    if (s.g.dist(here, tileOf(t)) <= 2) { pirateAttack(s, a, t, cp); return; }
    if (s.day % 3 === 0) {
      const goal = aheadOf(s, t, Math.ceil(s.g.dist(here, tileOf(t)) * 0.8) + 1);
      const p = goal >= 0 ? navPath(s, here, goal, { embark: [], open: true }) : null;
      if (p && p.length > 1) { a.path = p; a.step = 0; a.progress = 0; }
    }
    return;
  }
  // kıyı baskını: hedefin limanında boş kadırga varsa önce denizde karşılanır
  const st = a.purpose === 'settlement' ? s.settlement(a.to!) : a.targetTile !== undefined && w.tiles[a.targetTile].owner >= 0 ? s.settlement(w.tiles[a.targetTile].owner) : undefined;
  if (!st || !st.alive) return;
  for (const port of s.civSettlements(st.civ)) {
    if (port.port === undefined || a.fought?.includes(port.id) || s.g.dist(port.port, here) > 3) continue;
    const free = freeGalleys(s, port);
    if (!free) continue;
    (a.fought ??= []).push(port.id);
    const c = w.civs[port.civ];
    const A: Combatant[] = [];
    for (let i = 0; i < Math.min(3, free); i++) A.push(unit(GALLEY, 'A', 'galley'));
    A.push(...civTroops(s, c, Math.min(2, port.soldiers), 'A'));
    const B = monsterSide('pirate', a.troops ?? 0, !!a.boss, 'B');
    const b = resolveBattle(s.rng, A, B, { id: s.id(), day: s.day, tile: here, title: `${port.name} açıklarında korsan savaşı`, sideA: `${port.name} kadırgaları`, sideB: `${cp?.name ?? 'Korsanlar'}`, moraleA: 0.55, moraleB: 0.45, maxRounds: 10 });
    b.naval = true; recordBattle(s, b); s.metric('pirateNaval');
    const lostG = A.filter((x) => x.kind === 'galley' && x.hp <= 0).length;
    port.galleys = Math.max(0, (port.galleys ?? 0) - lostG);
    a.troops = B.filter((x) => x.kind === 'monster' && x.hp > 0).length; a.boss = B.some((x) => x.boss && x.hp > 0);
    if (b.winner === 'A') {
      s.log('sea', `${port.name} kadırgaları korsanları açıkta karşıladı ve kaçırdı.`, { civ: c.id, tile: here, battle: b.id, major: true, cause: `${cp?.name ?? 'Korsan koyu'}; ${b.lossesB} korsan öldü` });
      if ((a.troops ?? 0) <= 0 && !a.boss) a.dead = true; else raidReturn(s, a);
      return;
    }
    s.log('sea', `Korsanlar ${ek(port.name, 'in')} kadırgalarını yarıp kıyıya yöneldi${lostG ? `: ${lostG} kadırga battı` : ''}.`, { civ: c.id, tile: here, battle: b.id, major: true });
  }
}

function pirateAttack(s: Sim, p: Agent, t: Agent, cp: Camp | undefined) {
  const w = s.w, here = tileOf(t), c = w.civs[t.civ];
  const A: Combatant[] = [];
  if (t.kind === 'caravan') A.push(...civTroops(s, c, t.troops ?? 2, 'A'));
  else { const n = t.kind === 'settlers' ? Math.min(5, s.popSize(t.pop)) : 2; for (let i = 0; i < n; i++) A.push(unit(UNITS.militia, 'A', 'militia', RACES[c.race].hp)); }
  for (let i = 0; i < (t.galleys ?? 0); i++) A.push(unit(GALLEY, 'A', 'galley'));
  if (s.civSettlements(c).some((x) => x.civics.lighthouse && x.port !== undefined && s.g.dist(x.port, here) <= 8)) for (const x of A) x.ac += 2;
  const B = monsterSide('pirate', p.troops ?? 0, !!p.boss, 'B');
  const isle = isleOf(s, cp?.tile ?? here);
  const b = resolveBattle(s.rng, A, B, { id: s.id(), day: s.day, tile: here, title: `Korsan baskını${isle ? ` (${isle.name} açıkları)` : ''}`, sideA: `${c.name} gemisi`, sideB: `${cp?.name ?? 'Korsanlar'}`, moraleA: 0.45, moraleB: 0.5, maxRounds: 10 });
  b.naval = true; recordBattle(s, b);
  const hadCaptain = !!p.boss;
  p.troops = B.filter((x) => x.kind === 'monster' && x.hp > 0).length; p.boss = B.some((x) => x.boss && x.hp > 0);
  if (hadCaptain && !p.boss && cp) { cp.hadBoss = false; s.log('sea', `Kaptan ${cp.captain ?? ''} ${ek(c.name, 'in')} gemisinin güvertesinde öldü!`, { civ: c.id, tile: here, major: true, cause: cp.name }); cp.captain = undefined; }
  const deadG = A.filter((x) => x.kind === 'galley' && x.hp <= 0).length;
  const home = t.hull !== undefined ? s.settlement(t.hull) : undefined;
  if (deadG && home) home.galleys = Math.max(0, (home.galleys ?? 0) - deadG);
  if (t.kind === 'caravan') t.troops = A.filter((x) => (x.kind === 'soldier' || x.kind === 'unique') && x.hp > 0).length;
  const what = t.kind === 'settlers' ? 'öncü gemisini' : t.kind === 'caravan' ? 'yük gemisini' : t.purpose?.startsWith('explore') ? 'keşif gemisini' : 'gemisini';
  c.yearly.pirateHit = s.year;
  if (b.winner === 'B') {
    let value = 0, loot = '';
    for (const g in t.cargo ?? {}) { const q = t.cargo![g as never] ?? 0; value += q * s.price(c, g as never); loot = `, yükü (${Math.round(q)} ${g === 'grain' ? 'tahıl' : 'mal'}) yağmalandı`; }
    p.loot = (p.loot ?? 0) + Math.round(value);
    if (home) home.ships = Math.max(0, (home.ships ?? 0) - 1);
    t.dead = true;
    c.threat += 0.25;
    s.metric('pirateCapture');
    s.log('sea', `Korsanlar ${ek(c.name, 'in')} ${what} ele geçirdi${loot}${t.kind === 'settlers' ? `; ${s.popSize(t.pop)} öncü esir düştü` : ''}.`, { civ: c.id, tile: here, battle: b.id, major: true, cause: `${cp?.name ?? 'Korsan koyu'}${cp?.captain ? `, Kaptan ${cp.captain}` : ''}` });
    raidReturn(s, p);
  } else {
    s.metric('pirateRepelled');
    const subj = t.kind === 'settlers' ? 'öncü gemisi' : t.kind === 'caravan' ? 'yük gemisi' : t.purpose?.startsWith('explore') ? 'keşif gemisi' : 'gemisi';
    s.log('sea', `${ek(c.name, 'in')} ${subj} korsanları püskürttü.`, { civ: c.id, tile: here, battle: b.id, major: !!t.galleys, cause: t.galleys ? 'Eşlik eden kadırgalar' : `${b.lossesB} korsan öldü` });
    if ((p.troops ?? 0) <= 0 && !p.boss) p.dead = true; else raidReturn(s, p);
  }
}

/** korsan dönüşü: denizden koyuna (kıyı baskınından sonra karaya çıktığı yerden biner) */
export function pirateReturnPath(s: Sim, a: Agent, cove: number): number[] | null {
  const here = tileOf(a);
  const emb = [here]; if (a.landing !== undefined) emb.push(a.landing);
  return navPath(s, here, cove, { embark: emb, open: true });
}
/** korsan ganimeti Haydut limanlarında satılır */
export function pirateSmuggle(s: Sim, a: Agent) {
  const loot = a.loot ?? 0;
  if (loot < 10) return;
  const w = s.w, cp = w.camps.find((x) => x.id === a.from);
  if (!cp) return;
  const rogue = w.civs.find((c) => c.alive && smugglers(c) && ports(s, c).some((x) => s.g.dist(x.port!, cp.tile) <= 45));
  if (!rogue) return;
  const cut = Math.round(loot * 0.3);
  s.add(rogue, 'gold', cut);
  a.loot = loot - cut;
  if (!rogue.yearly['smuggle' + cp.id]) {
    rogue.yearly['smuggle' + cp.id] = s.year;
    s.log('sea', `${ek(rogue.name, 'in')} kaçakçıları ${ek(cp.name, 'in')} ganimetini limanlarında satmaya başladı.`, { civ: rogue.id, tile: cp.tile, major: true, cause: 'Korsanlar Haydut gemilerine dokunmuyor' });
  }
  s.metric('smuggled', cut);
}

/** korsan avı: gemisi olan medeniyet, kıyılarını ya da gemilerini vuran koya asker ve kadırga gönderir */
function considerPirateHunt(s: Sim, c: Civ) {
  if (!s.has(c, 'shipbuilding') || s.w.agents.some((a) => a.civ === c.id && a.purpose === 'expedition')) return;
  const ss = s.civSettlements(c);
  const my = ports(s, c);
  if (!my.length) return;
  const hit = (c.yearly.pirateHit ?? -9) >= s.year - 1;
  const cove = s.w.camps.filter((x) => x.alive && x.kind === 'pirate')
    .map((x) => ({ x, d: Math.min(...ss.map((st) => s.g.dist(st.tile, x.tile))) }))
    .filter((o) => o.d <= 40 && (hit || (o.d <= 14 && s.day - o.x.founded > 3 * YEAR))).sort((a, b) => a.d - b.d)[0]?.x;
  if (!cove) return;
  const soldiers = Math.floor(ss.reduce((a, x) => a + x.soldiers, 0) * 0.6);
  const heroes = s.civHeroes(c).filter((h) => h.state === 'home' && h.hp > h.maxHp * 0.7);
  if (soldiers < 6 && !heroes.length) return;
  const cap = s.capital(c)!;
  const route = civPath(s, c, cap.tile, cove.tile);
  if (!route || !route.hull || !hasSea(s, route.path)) return;
  const gal = Math.min(3, freeGalleys(s, route.hull));
  const side: Combatant[] = [...heroes.map((h) => heroCombatant(h, 'A')), ...civTroops(s, c, soldiers, 'A')];
  for (let i = 0; i < gal; i++) side.push(unit(GALLEY, 'A', 'galley'));
  if (powerOf(side) < campPower(s, cove) * 1.15) return;
  const pop = drawSoldiers(s, c, soldiers);
  for (const h of heroes) h.state = 'army';
  s.w.agents.push({ id: s.id(), kind: 'army', civ: c.id, path: route.path, step: 0, progress: 0, speed: 0.6, heroes: heroes.map((h) => h.id), troops: soldiers, pop, from: cap.id, to: cove.id, purpose: 'expedition', hull: route.hull.id, galleys: gal || undefined });
  s.metric('pirateHunt');
  s.log('sea', `${c.name} korsan avına çıktı: ${soldiers} asker${gal ? `, ${gal} kadırga` : ''}${heroes.length ? ` ve ${heroes.map((h) => h.name).join(', ')}` : ''} ile ${ek(cove.name, 'a')} yelken açtı.`, { civ: c.id, tile: route.hull.port, major: true, cause: hit ? 'Korsanlar gemilerimizi ve kıyılarımızı vuruyor' : `${cove.name} kıyılarımıza fazla yakın` });
}
