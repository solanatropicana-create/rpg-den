// Sea port check (TS side). Runs the real sim; at the given days it writes the world as a JSON snapshot line
// ({ kind: 'snap', seed, day, tag, snap }) for a natural ('nat') and two perturbed copies of the world ('pert': ports,
// ships, galleys, wars, sea techs, a pirate cove and staged armies / fleets / merchants / pirates at sea; 'port': the
// same ports / techs / wars without staged vessels, plus pirate-hit memory, big garrisons, a volcano-island colony and
// a smuggler civ, so considerSeaExplore / considerFleet / pirate hunts / volcano / smuggling fire), then:
//  - 'q' lines: queries run IN ORDER on ONE Sim restored from that snapshot (so navCache / path cache / shoreW
//    evolve identically): shoreWater, portWater (every tile), pickPort (real settlements at every tier + fake
//    ones), hulls/fleet/ports, isleOf, shipSpeed, thousands of navPath queries (random from/to, embark sets,
//    open flag, landOnly undefined / [] / tiles, repeats), civPath, embarkPort, seaReturnPath, pirateReturnPath.
//    Each line carries the query's parameters, the result `r` and the cache sizes after it.
//  - 'case' lines: each restores a fresh Sim from the snapshot, runs ONE sea.ts entry point (seaTick on chosen
//    days / RNG states that force storms, considerSeaExplore, considerFleet, launchPirates, exploreSight,
//    exploreTurn, fleetArrive, disembark, pirateSmuggle, seaReturnPath, pirateReturnPath, newCaptain) and
//    writes the post-state.
// C# twin: tests/SeaCheck (replays the parameters of every line). Run from macro/:
//   node golden/ts/build.mjs sea_check && node golden/out/sea_check.js <seed> <days,...> <navQueries> > /tmp/sea_ts.ndjson
//   dotnet build tests/SeaCheck -c Release && dotnet tests/SeaCheck/bin/Release/net8.0/SeaCheck.dll /tmp/sea_ts.ndjson > /tmp/sea_cs.ndjson
//   python3 tests/SeaCheck/sea_compare.py /tmp/sea_ts.ndjson /tmp/sea_cs.ndjson
import { Sim } from '../../../src/sim/sim';
import { Rng } from '../../../src/sim/rng';
import { HexGrid } from '../../../src/sim/hex';
import {
  shoreWater, pickPort, portWater, navPath, civPath, hullsOut, galleysOut, freeHulls, freeGalleys, fleet, hullName, ports,
  shipSpeed, seaReturnPath, pirateReturnPath, isleOf, embarkPort, hasSea, seaTick, considerSeaExplore, considerFleet,
  launchPirates, exploreSight, exploreTurn, fleetArrive, disembark, pirateSmuggle, newCaptain, onSea, isFleet,
} from '../../../src/sim/sea';
import { tileOf } from '../../../src/sim/agents';
import { makeSettlement } from '../../../src/sim/worldgen';
import type { Agent, Camp, Settlement, World } from '../../../src/sim/types';

const seed = Number(process.argv[2] ?? '1');
const days = (process.argv[3] ?? '0,1500,3000').split(',').map(Number);
const nq = Number(process.argv[4] ?? '3500');

const main = new Sim(seed);
const scratch = new Sim(seed);
const replacer = (_k: string, v: unknown) => (typeof v === 'number' && !Number.isFinite(v) ? String(v) : v);
const reviver = (_k: string, v: unknown) => (v === 'Infinity' ? Infinity : v === '-Infinity' ? -Infinity : v === 'NaN' ? NaN : v);
const out = (o: unknown) => process.stdout.write(JSON.stringify(o, replacer) + '\n');

function restore(snap: string): Sim {
  const s = scratch as unknown as { w: World; g: HexGrid; shoreW?: Uint8Array };
  s.w = JSON.parse(snap, reviver);
  s.g = new HexGrid(s.w.W, s.w.H);
  scratch.rng.setState(scratch.w.rngState);
  scratch.clearPaths(); scratch.navCache.clear(); s.shoreW = undefined;
  return scratch;
}
const pathCacheSize = (s: Sim) => (s as unknown as { pathCache: Map<string, unknown> }).pathCache.size;

function tileSets(s: Sim) {
  const w = s.w, g = s.g, N = w.tiles.length;
  const isSea = (i: number) => !!w.tiles[i].sea;
  const land: number[] = [], coast: number[] = [], seaT: number[] = [];
  for (let i = 0; i < N; i++) { if (isSea(i)) seaT.push(i); else { land.push(i); if (g.neighbors(i).some(isSea)) coast.push(i); } }
  return { N, isSea, land, coast, seaT, coastSet: new Set(coast) };
}

/** random walk over distinct sea tiles */
function seaWalk(s: Sim, R: Rng, start: number, len: number): number[] {
  const p = [start];
  let cur = start;
  for (let k = 1; k < len; k++) {
    const ns = s.g.neighbors(cur).filter((n) => s.w.tiles[n].sea && !p.includes(n));
    if (!ns.length) break;
    cur = ns[Math.floor(R.next() * ns.length)];
    p.push(cur);
  }
  return p;
}

// ------------------------------------------------------------------ perturbation ('pert')
function perturb(nat: string, R: Rng, stage: boolean): string {
  const s = restore(nat);
  const w = s.w, g = s.g;
  const { coast } = tileSets(s);
  const pick = <T>(xs: readonly T[]): T => xs[Math.floor(R.next() * xs.length)];
  const alive = w.civs.filter((c) => c.alive);
  for (const c of alive)
    for (const [t, p] of [['boatbuilding', 1], ['shipbuilding', 0.7], ['navigation', 0.6], ['navy', 0.75], ['seatrade', 0.5]] as [string, number][])
      if (!c.research.done.includes(t) && R.chance(p)) c.research.done.push(t);
  for (const st of w.settlements) {
    if (!st.alive) continue;
    const p = pickPort(s, st);
    if (p >= 0 && R.chance(0.85)) {
      st.civics.shipyard = 1; st.port = p; st.ships = R.int(stage ? 0 : 1, stage ? 4 : 6); st.galleys = R.int(0, stage ? 4 : 5); st.soldiers += R.int(0, stage ? 6 : 25);
      if (R.chance(0.25)) st.civics.lighthouse = 1;
    }
  }
  for (const a of alive) for (const b of alive) if (a.id < b.id) {
    w.relations[a.id][b.id].contact = true; w.relations[b.id][a.id].contact = true;
    if (R.chance(0.6)) {
      const bs = w.settlements.filter((x) => x.alive && x.civ === b.id);
      const war = { since: w.day - R.int(0, 300), attacker: a.id, target: bs.length ? pick(bs).id : -1, attacks: 0, lastArmy: -999, goal: 'Deniz sınaması' };
      w.relations[a.id][b.id].war = war; w.relations[b.id][a.id].war = { ...war };
    }
  }
  // a pirate cove to launch from / return to
  let coves = w.camps.filter((c) => c.alive && c.kind === 'pirate');
  if (!coves.length) {
    const free = coast.filter((i) => w.tiles[i].owner < 0 && w.tiles[i].camp === undefined && w.tiles[i].terrain !== 'mountain' && w.tiles[i].terrain !== 'water');
    if (free.length) {
      const tile = pick(free);
      const cp: Camp = { id: w.nextId++, kind: 'pirate', tile, name: 'Sınama Koyu', count: 9, boss: true, hadBoss: true, loot: 20, growthAcc: 0, alive: true, nextRaid: w.day, founded: w.day - 500, captain: 'Kara Nesrin' };
      w.camps.push(cp); w.tiles[tile].camp = cp.id;
      coves = [cp];
    }
  }
  for (const cp of coves) { cp.count = Math.max(cp.count, R.int(5, 12)); cp.boss = R.chance(0.6); }
  const nearSea0 = (x: number, r: number) => { const c = g.within(x, r).filter((i) => w.tiles[i].sea); return c.length ? pick(c) : -1; };
  for (const cp of coves) {
    const t = nearSea0(cp.tile, 5);
    if (t >= 0) w.agents.push({ id: w.nextId++, kind: 'raid', civ: -1, monster: 'pirate', from: cp.id, purpose: 'return', returning: true, loot: R.int(0, 120), troops: R.int(1, 5), path: seaWalk(s, R, t, 3), step: 0, progress: 0, speed: 1, landing: R.chance(0.5) ? cp.tile : undefined } as Agent);
  }
  if (!stage) {
    // pirate hunts: remembered pirate hits; a volcano-island colony with farms; a smuggler (Haydut) civ with ports
    for (const c of alive) if (R.chance(0.6)) c.yearly.pirateHit = Math.floor(w.day / 120) + 1 - R.int(0, 1);
    for (const isl of (w.isles ?? []).filter((x) => x.kind === 'volkan' && x.peak !== undefined)) {
      if (w.settlements.some((x) => x.alive && w.tiles[x.tile].isle === isl.id) || !alive.length) continue;
      const cand: number[] = [];
      w.tiles.forEach((t, i) => { if (t.isle === isl.id && !t.sea && t.terrain !== 'mountain' && t.terrain !== 'water' && t.owner < 0 && t.camp === undefined) cand.push(i); });
      if (!cand.length) continue;
      const civ = pick(alive), tile = pick(cand);
      const st = makeSettlement(w.nextId++, civ.id, 'Kül Obası', tile, { [civ.race]: 14 }, w.day - 200);
      st.tier = 1; st.overseas = true;
      w.settlements.push(st);
      for (const i of g.within(tile, 3)) {
        const t = w.tiles[i];
        if (t.sea || t.isle !== isl.id || t.camp !== undefined) continue;
        t.owner = st.id;
        if (i !== tile && !t.ext && R.chance(0.6)) t.ext = { kind: 'farm', level: 1, settlement: st.id, workers: 2, burned: R.chance(0.2) ? w.day + 5 : undefined };
      }
    }
    const withPorts = alive.filter((c) => w.settlements.some((x) => x.alive && x.civ === c.id && x.civics.shipyard && x.port !== undefined));
    if (!withPorts.some((c) => c.cls === 'rogue') && withPorts.length) pick(withPorts).cls = 'rogue';
    return JSON.stringify(w, replacer);
  }
  // staged vessels
  const nearSea = (x: number, r: number) => { const c = g.within(x, r).filter((i) => w.tiles[i].sea); return c.length ? pick(c) : -1; };
  const push = (a: Partial<Agent>) => { const ag = { id: w.nextId++, step: 0, progress: 0, speed: 1, ...a } as Agent; w.agents.push(ag); return ag; };
  for (const a of alive) for (const b of alive) {
    if (a.id === b.id || !w.relations[a.id][b.id].war) continue;
    const portsA = w.settlements.filter((x) => x.alive && x.civ === a.id && x.civics.shipyard && x.port !== undefined);
    const portsB = w.settlements.filter((x) => x.alive && x.civ === b.id && x.civics.shipyard && x.port !== undefined);
    if (!portsB.length) continue;
    const stB = pick(portsB);
    const capA = w.settlements.find((x) => x.alive && x.civ === a.id);
    const t0 = nearSea(stB.port!, 3);
    if (t0 < 0) continue;
    if (R.chance(0.7)) {
      const troops = R.int(1, 12);
      push({ kind: 'army', civ: a.id, path: seaWalk(s, R, t0, R.int(1, 6)), speed: 0.6, heroes: [], troops, pop: { [a.race]: troops }, from: capA?.id, to: stB.id, purpose: R.chance(0.75) ? 'war' : 'plunder', hull: portsA.length ? pick(portsA).id : undefined, galleys: R.int(0, 3), progress: R.next() });
    }
    const t1 = nearSea(stB.port!, 4);
    if (t1 >= 0 && R.chance(0.7)) push({ kind: 'ship', civ: a.id, purpose: 'fleet', galleys: R.int(0, 4), hull: portsA.length ? pick(portsA).id : undefined, to: stB.id, path: seaWalk(s, R, t1, R.int(1, 5)) });
    // b's civilian vessels near there
    for (let k = R.int(0, 3); k > 0; k--) {
      const t2 = nearSea(t1 >= 0 ? t1 : t0, 3);
      if (t2 < 0) break;
      const kind = pick(['caravan', 'settlers', 'ship', 'ship'] as const);
      const base = { civ: b.id, path: seaWalk(s, R, t2, R.int(2, 9)), hull: R.chance(0.85) ? stB.id : undefined, galleys: R.chance(0.3) ? R.int(1, 2) : undefined };
      if (kind === 'caravan') push({ ...base, kind, cargo: R.chance(0.8) ? { grain: R.int(1, 30), iron: R.int(0, 6) } : undefined, troops: R.chance(0.7) ? R.int(0, 4) : undefined, from: stB.id, to: stB.id, purpose: 'trade' });
      else if (kind === 'settlers') push({ ...base, kind, pop: { [b.race]: R.int(3, 6) }, from: stB.id, purpose: 'settle' });
      else push({ ...base, kind, purpose: pick(['return', 'explore', 'explore-back', 'trade', undefined]) });
    }
  }
  const civVessels = w.agents.filter((x) => !x.dead && x.civ >= 0 && (x.kind === 'caravan' || x.kind === 'settlers' || (x.kind === 'ship' && !isFleet(x))) && w.tiles[tileOf(x)].sea);
  for (const cp of coves) {
    for (const prey of civVessels) {
      if (!R.chance(0.6)) continue;
      const t = nearSea(tileOf(prey), R.chance(0.6) ? 2 : 7);
      if (t < 0) continue;
      push({ kind: 'raid', civ: -1, monster: 'pirate', from: cp.id, purpose: 'prey', to: prey.id, troops: R.int(1, 8), boss: R.chance(0.5), path: seaWalk(s, R, t, R.int(1, 4)), targetTile: tileOf(prey), chase: R.chance(0.3) ? w.day - R.int(0, 40) : undefined });
    }
    const targets = w.settlements.filter((x) => x.alive && x.port !== undefined);
    for (let k = R.int(0, 2); k > 0 && targets.length; k--) {
      const st = pick(targets);
      const t = nearSea(st.port!, 3);
      if (t < 0) continue;
      const settle = R.chance(0.6);
      push({ kind: 'raid', civ: -1, monster: 'pirate', from: cp.id, purpose: settle ? 'settlement' : 'ext', to: settle ? st.id : st.port, targetTile: settle ? st.tile : st.port, troops: R.int(1, 8), boss: R.chance(0.5), path: seaWalk(s, R, t, R.int(1, 4)), fought: R.chance(0.2) ? [st.id] : undefined });
    }
    const t = nearSea(cp.tile, 4);
    if (t >= 0) push({ kind: 'raid', civ: -1, monster: 'pirate', from: cp.id, purpose: 'return', returning: true, loot: R.int(0, 90), troops: R.int(1, 5), path: seaWalk(s, R, t, 3), landing: R.chance(0.5) ? cp.tile : undefined });
  }
  for (const c of alive) {
    const pc = w.settlements.filter((x) => x.alive && x.civ === c.id && x.civics.shipyard && x.port !== undefined);
    if (!pc.length || !R.chance(0.6)) continue;
    const st = pick(pc);
    const t = nearSea(st.port!, 8);
    if (t >= 0) push({ kind: 'ship', civ: c.id, purpose: pick(['explore', 'explore-back']), hull: st.id, path: seaWalk(s, R, t, R.int(1, 8)) });
  }
  return JSON.stringify(w, replacer);
}

// ------------------------------------------------------------------ queries
let qi = 0;
function queries(s: Sim, R: Rng, count: number) {
  const w = s.w, g = s.g;
  const { N, isSea, land, coast, seaT, coastSet } = tileSets(s);
  const pick = <T>(xs: readonly T[]): T => xs[Math.floor(R.next() * xs.length)];
  const near = (x: number, r: number, f: (i: number) => boolean) => { const c = g.within(x, r).filter(f); return c.length ? pick(c) : x; };
  const q = (o: Record<string, unknown>) => out({ kind: 'q', i: qi++, ...o });
  const sizes = () => ({ n: s.navCache.size, pn: pathCacheSize(s) });

  const sh = shoreWater(s);
  const shIdx: number[] = [];
  for (let k = 0; k < sh.length; k++) if (sh[k]) shIdx.push(k);
  q({ op: 'shore', r: shIdx });
  const pw: number[] = [];
  for (let k = 0; k < N; k++) pw.push(portWater(s, k));
  q({ op: 'portWater', r: pw });
  for (const st of w.settlements) {
    if (!st.alive) continue;
    const t0 = st.tier;
    for (let t = 0; t <= 3; t++) { st.tier = t; q({ op: 'pickPort', sid: st.id, tier: t, r: pickPort(s, st) }); }
    st.tier = t0;
  }
  for (let k = 0; k < 200 && coast.length; k++) {
    const tile = pick(coast);
    const id = w.tiles[tile].owner >= 0 && R.chance(0.7) ? w.tiles[tile].owner : 100000 + k;
    const tier = R.int(0, 3);
    q({ op: 'pickPortF', id, tile, tier, r: pickPort(s, { id, tile, tier } as Settlement) });
  }
  for (const st of w.settlements) q({ op: 'hulls', sid: st.id, r: [hullsOut(s, st), galleysOut(s, st), freeHulls(s, st), freeGalleys(s, st)] });
  for (const c of w.civs) { const f = fleet(s, c); q({ op: 'fleet', civ: c.id, r: [f.ships, f.galleys, hullName(s, c), hullName(s, c, true), ports(s, c).map((x) => x.id)] }); }
  for (let k = 0; k < 300; k++) { const t = R.int(0, N - 1); q({ op: 'isleOf', tile: t, r: isleOf(s, t)?.id ?? null }); }
  for (const a of w.agents) q({ op: 'speedA', aid: a.id, r: shipSpeed(s, a) });
  const KINDS = ['caravan', 'ship', 'settlers', 'army', 'raid', 'scout'];
  const PURP = [undefined, 'explore', 'explore-back', 'exploreX', 'fleet', 'trade', 'return'];
  for (let k = 0; k < 150; k++) {
    const a = { civ: R.int(-1, w.civs.length - 1), kind: pick(KINDS), purpose: pick(PURP), monster: R.chance(0.2) ? 'pirate' : undefined };
    q({ op: 'speedF', a, r: shipSpeed(s, a as Agent) });
  }

  const alive = w.civs.filter((c) => c.alive);
  const setl = w.settlements.filter((x) => x.alive);
  type NQ = { from: number; to: number; embark: number[]; open: boolean; landOnly?: number[] };
  const prev: NQ[] = [];
  for (let k = 0; k < count; k++) {
    const t = R.int(0, 99);
    let nqv: NQ;
    if (t < 8 && prev.length) nqv = pick(prev);
    else {
      let from: number, to: number, embark: number[];
      if (t < 38) { from = pick(coast); to = near(from, R.int(3, 30), (j) => !isSea(j)); embark = [from]; for (let e = R.int(0, 3); e > 0; e--) embark.push(near(from, 10, (j) => coastSet.has(j))); }
      else if (t < 53) { from = pick(seaT); to = near(from, R.int(2, 25), (j) => !isSea(j)); embark = R.chance(0.7) ? [] : [pick(coast)]; }
      else if (t < 63) { from = pick(coast); to = near(from, R.int(3, 30), isSea); embark = [from]; }
      else if (t < 73) { from = pick(seaT); to = near(from, R.int(2, 20), isSea); embark = []; }
      else if (t < 88) { from = pick(land); to = near(from, R.int(2, 20), (j) => !isSea(j)); embark = R.chance(0.8) ? [] : [near(from, 6, (j) => coastSet.has(j))]; }
      else { from = R.int(0, N - 1); to = R.int(0, N - 1); embark = []; for (let e = R.int(0, 3); e > 0; e--) embark.push(pick(coast)); }
      const open = R.chance(0.5);
      const lo = R.int(0, 99);
      const landOnly = lo < 35 ? undefined : lo < 50 ? [] : lo < 80 ? [to] : [to, near(to, 5, (j) => coastSet.has(j)), near(to, 5, (j) => coastSet.has(j))];
      nqv = { from, to, embark, open, landOnly };
      prev.push(nqv);
    }
    const r = navPath(s, nqv.from, nqv.to, { embark: nqv.embark, open: nqv.open, landOnly: nqv.landOnly });
    q({ op: 'nav', ...nqv, r, ...sizes() });
    if (r && alive.length && hasSea(s, r) && R.chance(0.2)) { const c = pick(alive); q({ op: 'embarkPort', civ: c.id, p: r, r: embarkPort(s, c, r)?.id ?? null }); }
  }
  for (let k = 0; k < Math.floor(count / 8) && alive.length; k++) {
    const c = pick(alive);
    const from = R.chance(0.7) && setl.length ? pick(setl).tile : pick(land);
    const to = R.chance(0.5) && setl.length ? pick(setl).tile : pick(land);
    const lo = R.int(0, 9);
    const landOnly = lo < 6 ? undefined : lo < 7 ? [] : [near(to, 4, (j) => coastSet.has(j))];
    const r = civPath(s, c, from, to, { landOnly });
    q({ op: 'civPath', civ: c.id, from, to, landOnly, r: r ? { path: r.path, hull: r.hull?.id } : null, ...sizes() });
  }
  for (let k = 0; k < Math.floor(count / 12) && alive.length && setl.length; k++) {
    const c = pick(alive);
    const landing = pick(coast);
    const here = R.chance(0.5) ? landing : near(landing, 4, (j) => !isSea(j));
    const a = { id: -5, kind: 'army', civ: c.id, path: R.chance(0.8) ? [here] : [here, near(here, 2, () => true)], step: R.int(0, 2), progress: 0, speed: 1, hull: R.chance(0.9) ? pick(setl).id : undefined, landing: R.chance(0.9) ? landing : undefined };
    const to = R.chance(0.7) ? (s.capital(c)?.tile ?? pick(land)) : pick(land);
    q({ op: 'seaReturn', a, to, r: seaReturnPath(s, a as Agent, to), ...sizes() });
  }
  for (let k = 0; k < Math.floor(count / 12) && coast.length; k++) {
    const here = R.chance(0.6) ? pick(seaT) : pick(coast);
    const a = { id: -6, kind: 'raid', civ: -1, path: [here], step: 0, progress: 0, speed: 1, landing: R.chance(0.5) ? near(here, 3, (j) => coastSet.has(j)) : undefined };
    const cove = near(here, R.int(3, 30), (j) => coastSet.has(j));
    q({ op: 'pirateReturn', a, cove, r: pirateReturnPath(s, a as Agent, cove), ...sizes() });
  }
}

// ------------------------------------------------------------------ cases
type Case = [string, (s: Sim) => unknown, boolean];
function nextDay(d: number, mod: number, res: number) { let x = d + 1; while (x % mod !== res) x++; return x; }

function cases(snap: string, day: number, R: Rng, volcanoRuns: number): Case[] {
  const s = restore(snap);
  const w = s.w;
  const cs: Case[] = [];
  const tickDays = [day + 1 + (day % 10 === 0 ? 1 : 0), nextDay(day, 30, 7), nextDay(day, 10, 3), nextDay(day, 10, 6), nextDay(day, 10, 8), nextDay(day, 120, 60), nextDay(day, 120, 90), nextDay(day + 240, 120, 60)];
  for (const d of tickDays) cs.push([`tick:${d}`, (x) => { x.w.day = d; seaTick(x); }, d % 120 === 60 || d % 120 === 90]);
  // storms: RNG states whose j-th draw is below every storm probability (and earlier draws above them)
  const civSea = w.agents.filter((a) => !a.dead && a.civ >= 0 && w.tiles[tileOf(a)].sea);
  const dq = nextDay(day, 10, 1);
  const probe = new Rng(1);
  for (let j = 0; j < Math.min(civSea.length, 10); j++) {
    for (const sink of [false, true]) {
      for (let k = 1; k < 3_000_000; k++) {
        probe.setState(w.rngState + k * 7777777);
        let ok = true;
        for (let m = 0; m < j && ok; m++) if (probe.next() < 0.001) ok = false;
        if (!ok || probe.next() >= 1e-5) continue;
        const nx = probe.next();
        if (sink ? nx < 0.8 : nx >= 0.8) continue;
        cs.push([`tickR:${dq}:${k}`, (x) => { x.w.day = dq; x.rng.setState(x.w.rngState + k * 7777777); seaTick(x); }, false]);
        break;
      }
    }
  }
  for (let k = 1; k <= 4; k++) cs.push([`tickR:${nextDay(day, 10, 8)}:${k}`, (x) => { x.w.day = nextDay(day, 10, 8); x.rng.setState(x.w.rngState + k * 7777777); seaTick(x); }, false]);
  for (let k = 1; k <= 4; k++) { const d = nextDay(day + 120 * k, 120, 60); cs.push([`tickR:${d}:${k}`, (x) => { x.w.day = d; x.rng.setState(x.w.rngState + k * 7777777); seaTick(x); }, true]); }
  for (let k = 1; k <= volcanoRuns; k++) { const d = nextDay(day + 120 * (k % 4), 120, 90); cs.push([`tickR:${d}:${k}`, (x) => { x.w.day = d; x.rng.setState(x.w.rngState + k * 7777777); seaTick(x); }, true]); }
  for (const c of w.civs) {
    if (!c.alive) continue;
    const id = c.id;
    cs.push([`explore:${id}`, (x) => considerSeaExplore(x, x.w.civs[id]), false]);
    cs.push([`fleet:${id}`, (x) => considerFleet(x, x.w.civs[id]), false]);
  }
  for (const cp of w.camps) {
    if (!cp.alive || cp.kind !== 'pirate') continue;
    const id = cp.id;
    const get = (x: Sim) => x.w.camps.find((c) => c.id === id)!;
    cs.push([`launch:${id}`, (x) => launchPirates(x, get(x)), false]);
    for (let k = 1; k <= 3; k++) cs.push([`launchR:${id}:${k}`, (x) => { x.rng.setState(x.w.rngState + k * 7777777); launchPirates(x, get(x)); }, false]);
  }
  for (const a of w.agents) {
    if (a.dead) continue;
    const id = a.id;
    const get = (x: Sim) => x.w.agents.find((b) => b.id === id)!;
    if (a.kind === 'ship' && a.civ >= 0) cs.push([`sight:${id}`, (x) => exploreSight(x, get(x)), false]);
    if (a.kind === 'ship' && a.civ >= 0) cs.push([`turn:${id}`, (x) => exploreTurn(x, get(x)), false]);
    if (isFleet(a)) cs.push([`arrive:${id}`, (x) => fleetArrive(x, get(x)), false]);
    if (onSea(s, a) && (a.civ >= 0 || a.monster === 'pirate')) {
      const land = s.g.neighbors(tileOf(a)).filter((n) => !w.tiles[n].sea);
      if (land.length) { const lt = land[Math.floor(R.next() * land.length)]; cs.push([`disembark:${id}:${lt}`, (x) => { const b = get(x); disembark(x, b, tileOf(b), lt); }, false]); }
    }
    if (a.monster === 'pirate') {
      cs.push([`smuggle:${id}`, (x) => pirateSmuggle(x, get(x)), false]);
      const cp = w.camps.find((c) => c.id === a.from);
      if (cp) { const cove = cp.tile; cs.push([`pirateReturn:${id}:${cove}`, (x) => pirateReturnPath(x, get(x), cove), false]); }
    }
    if (a.kind === 'army' && a.civ >= 0) {
      const cap = s.capital(w.civs[a.civ]);
      if (cap) { const to = cap.tile; cs.push([`seaReturn:${id}:${to}`, (x) => seaReturnPath(x, get(x), to), false]); }
    }
  }
  cs.push(['captain', (x) => newCaptain(x), false]);
  for (let k = 1; k <= 3; k++) cs.push([`captainR:${k}`, (x) => { x.rng.setState(x.w.rngState + k * 7777777); return newCaptain(x); }, false]);
  return cs;
}

let day = 0;
for (const target of days) {
  while (main.w.day < target) main.step();
  day = main.w.day;
  const nat = JSON.stringify(main.w, replacer);
  for (const tag of ['nat', 'pert', 'port']) {
    const snap = tag === 'nat' ? nat : perturb(nat, new Rng(seed * 7919 + day * 13 + (tag === 'pert' ? 5 : 6)), tag === 'pert');
    out({ kind: 'snap', seed, day, tag, snap });
    const R = new Rng(seed * 104729 + day * 31 + (tag === 'nat' ? 1 : tag === 'pert' ? 2 : 3));
    queries(restore(snap), R, tag === 'port' ? Math.floor(nq / 3) : nq);
    const preNext = (JSON.parse(snap) as World).nextId;
    for (const [name, run, tiles] of cases(snap, day, R, tag === 'port' ? 40 : 3)) {
      const s = restore(snap);
      let extra: unknown, err: string | undefined;
      try { extra = run(s); } catch (e) { err = String(e); }
      const w = s.w;
      const dumpW = { ...w, tiles: undefined, relations: undefined, inns: undefined, isles: undefined, events: w.events.filter((e) => e.id >= preNext), battles: w.battles.filter((b) => b.id >= preNext) };
      const tl = tiles && !err ? w.tiles.map((t, i) => [i, t] as const).filter(([, t]) => t.ext || t.camp !== undefined) : undefined;
      out({ kind: 'case', day, tag, name, rng: s.rng.state(), extra, err, n: s.navCache.size, tiles: tl, w: dumpW });
    }
  }
}
