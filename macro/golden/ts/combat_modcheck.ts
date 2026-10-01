// Diplomacy / Monsters port check (TS side). Runs the real sim; every `every` days it writes the world as a JSON
// snapshot line ({ kind: 'snap', day, snap }) and then, for each check case, restores a scratch Sim from that same
// JSON (so object identity / caches match what the C# side gets from Json.Deserialize), runs ONE function of
// diplomacy.ts / monsters.ts and writes the post-state ({ kind: 'case', day, name, rng, extra, w }).
// C# twin: tests/CombatModCheck (reads this file, replays every case on Sim.FromWorld(snapshot)).
// Run from macro/:
//   node golden/ts/build.mjs combat_modcheck && node golden/out/combat_modcheck.js <seed> <days> <every> > /tmp/cm_ts.ndjson
//   dotnet build tests/CombatModCheck -c Release && dotnet tests/CombatModCheck/bin/Release/net8.0/CombatModCheck.dll /tmp/cm_ts.ndjson > /tmp/cm_cs.ndjson
//   python3 tests/CombatModCheck/combat_modcompare.py /tmp/cm_ts.ndjson /tmp/cm_cs.ndjson
import { Sim } from '../../../src/sim/sim';
import { Rng } from '../../../src/sim/rng';
import { relationsTick, worldTick, considerExpansion, considerTrade, considerWarAction, considerScout, considerRaid, militaryPower } from '../../../src/sim/diplomacy';
import { campsTick, launchRaid, campAway } from '../../../src/sim/monsters';
import type { World } from '../../../src/sim/types';

const seed = Number(process.argv[2] ?? '1');
const days = Number(process.argv[3] ?? '2400');
const every = Number(process.argv[4] ?? '120');
const main = new Sim(seed);
const scratch = new Sim(seed);
const replacer = (_k: string, v: unknown) => (typeof v === 'number' && !Number.isFinite(v) ? String(v) : v);
const out = (o: unknown) => process.stdout.write(JSON.stringify(o, replacer) + '\n');

function restore(snap: string): Sim {
  const s = scratch as unknown as { w: World; shoreW?: Uint8Array };
  s.w = JSON.parse(snap);
  scratch.rng.setState(scratch.w.rngState);
  scratch.clearPaths(); scratch.navCache.clear(); s.shoreW = undefined;
  return scratch;
}

/** check cases: name → [run, include tiles in the dump] (same names in C#) */
function cases(w: World): [string, (s: Sim) => unknown, boolean][] {
  const cs: [string, (s: Sim) => unknown, boolean][] = [
    ['relationsTick', (s) => relationsTick(s), false],
    ['worldTick', (s) => worldTick(s), true],
    ['campsTick', (s) => campsTick(s), true],
  ];
  for (const c of w.civs) {
    if (!c.alive) continue;
    const id = c.id;
    cs.push([`scout:${id}`, (s) => considerScout(s, s.w.civs[id]), false]);
    cs.push([`trade:${id}`, (s) => considerTrade(s, s.w.civs[id]), false]);
    cs.push([`expand:${id}`, (s) => considerExpansion(s, s.w.civs[id]), false]);
    cs.push([`war:${id}`, (s) => considerWarAction(s, s.w.civs[id]), false]);
    cs.push([`raid:${id}`, (s) => considerRaid(s, s.w.civs[id]), false]);
    cs.push([`mil:${id}`, (s) => militaryPower(s, s.w.civs[id]), false]);
  }
  if (w.relations.length && (w as unknown as { _tag?: string })._tag !== 'nat')
    for (let k = 1, n = w.camps.filter((c) => c.alive).length < 2 ? 150 : 40; k <= n; k++) cs.push([`campsR:${k}`, (s) => { s.rng.setState(s.w.rngState + k * 7777777); campsTick(s); }, false]);
  for (const cp of w.camps) {
    if (!cp.alive) continue;
    const id = cp.id;
    cs.push([`away:${id}`, (s) => campAway(s, s.w.camps.find((x) => x.id === id)!), false]);
    cs.push([`launch:${id}`, (s) => launchRaid(s, s.w.camps.find((x) => x.id === id)!), false]);
  }
  return cs;
}

/**
 * 'pert' snapshots: the natural world pushed into states the natural run rarely reaches early (contacts, tensions,
 * relation mods, wars to end, trade/road techs, markets, crowded capitals, food, extinct civs, half-elf households,
 * camps due to raid). Applied to the parsed snapshot with its own RNG; both sides then load the SAME JSON.
 */
function perturb(w: World, day: number, calm: boolean): World {
  const R = new Rng(day * 7919 + (calm ? 29 : 13));
  const ri = (a: number, b: number) => R.int(a, b);
  const ch = (p: number) => R.next() < p;
  const pick = <T>(xs: readonly T[]): T => xs[Math.floor(R.next() * xs.length)];
  const GOODS = ['copper', 'tin', 'iron', 'gold', 'mana', 'mithril', 'horses', 'herbs', 'salt', 'bricks', 'arms', 'beer'];
  const alive = w.civs.filter((c) => c.alive);
  for (const a of w.civs) for (const b of w.civs) {
    if (a.id === b.id) continue;
    const r = w.relations[a.id][b.id];
    if (ch(0.9)) r.contact = true;
    if (a.id < b.id && ch(0.15)) { r.contact = false; w.relations[b.id][a.id].contact = false; }
    if (ch(0.5)) for (let k = ri(1, 3); k > 0; k--) (r.tension as Record<string, number>)[pick(GOODS)] = pick([0, 2, 5.5, 6, 9, 10, 12, 16, 20, 25]);
    if (ch(0.7)) r.lastTalk = -9999;
    if (ch(0.3)) r.land = pick([0, 3, 7.9, 8, 12, 20, 30]);
    if (ch(0.4)) r.mods.push({ key: 'test' + ri(1, 3), text: pick(['Eski kan davası', 'Ticaret dostluğu', 'Işık yemini']), value: ri(-60, 60), decay: pick([0, 0.02, 0.05, 0.1]) });
  }
  for (const c of alive) {
    for (const t of ['roads', 'barter', 'caravans', 'fishing']) if (!c.research.done.includes(t) && ch(0.7)) c.research.done.push(t);
    if (ch(0.5)) c.lastExpand = -9999;
    if (ch(0.5)) c.lastRaidedDay = day - ri(0, 300);
    if (ch(0.3)) c.lastWarEnd = day - ri(0, 400);
    c.stock.grain = (c.stock.grain ?? 0) + ri(0, 150);
    c.stock.wood = (c.stock.wood ?? 0) + ri(0, 30);
    const ss = w.settlements.filter((x) => x.alive && x.civ === c.id);
    for (const st of ss) {
      st.soldiers += ri(0, 8);
      if (ch(0.3)) st.civics.market = 1;
      if (ch(0.2)) { st.pop.human = (st.pop.human ?? 0) + ri(0, 4); st.pop.elf = (st.pop.elf ?? 0) + ri(0, 3); if (ch(0.5)) st.mixedSince['human-elf'] = day - ri(0, 250); }
    }
    if (ss.length && ch(0.5)) { const cap = ss[0]; cap.pop[c.race] = (cap.pop[c.race] ?? 0) + ri(5, 40); }
  }
  // wars to judge (peace branch) or to act on
  for (let k = ri(0, 2); k > 0 && alive.length >= 2; k--) {
    const a = pick(alive), b = pick(alive);
    if (a === b) continue;
    const bs = w.settlements.filter((x) => x.civ === b.id);
    const war = { since: day - ri(0, 400), attacker: a.id, target: bs.length ? pick(bs).id : -1, attacks: ri(0, 4), lastArmy: -999, goal: 'Sınama savaşı' };
    w.relations[a.id][b.id].war = war; w.relations[b.id][a.id].war = { ...war };
  }
  // an extinct civ waiting for new founders
  if (alive.length >= 3 && ch(0.3)) {
    const c = pick(alive);
    c.alive = false; c.extinctDay = day - ri(150, 300); c.respawned = undefined;
    for (const st of w.settlements) if (st.civ === c.id) st.alive = false;
  }
  for (const cp of w.camps) if (cp.alive && ch(0.4)) { cp.nextRaid = day; cp.count += ri(0, 6); if (ch(0.3)) cp.boss = true; }
  // 'calm': everyone met and recently made peace, so relationsTick runs disputes / talks / treaties / peace
  // without reaching makeContact or militaryPower (agents.ts)
  if (calm) for (const a of w.civs) for (const b of w.civs) if (a.id !== b.id) {
    const r = w.relations[a.id][b.id];
    r.contact = true; r.peaceDay = day - ri(0, 230);
    if (ch(0.5)) for (const g of ['copper', 'tin', 'iron', 'gold', 'mana', 'mithril', 'horses', 'herbs', 'salt', 'bricks']) (r.tension as Record<string, number>)[g] = 5.5;
  }
  // most camps cleared long ago: the "new tribe" branch of campsTick
  if (!calm && ch(0.3)) { let keep = ri(0, 1); for (const cp of w.camps) if (cp.alive) { if (keep-- > 0) continue; cp.alive = false; cp.clearedDay = day - ri(250, 400); w.tiles[cp.tile].camp = undefined; } }
  return w;
}

for (let d = 1; d <= days; d++) {
  main.step();
  if (d % every !== 0) continue;
  const nat = JSON.stringify(main.w, replacer);
  for (const tag of ['nat', 'pert', 'calm']) {
    const snap = tag === 'nat' ? nat : JSON.stringify(perturb(JSON.parse(nat), d, tag === 'calm'), replacer);
    out({ kind: 'snap', day: d, tag, snap });
    const preNext = main.w.nextId;
    const cw = JSON.parse(snap); cw._tag = tag;
    for (const [name, run, tiles] of cases(cw)) {
      const s = restore(snap);
      let extra: unknown, err: string | undefined;
      try { extra = run(s); } catch (e) { err = String(e); }
      const w = s.w;
      const dumpW = { ...w, tiles: tiles ? w.tiles : undefined, deposits: undefined, battles: undefined, inns: undefined, isles: undefined, events: w.events.filter((e) => e.id >= preNext) };
      const campTiles = name.startsWith('campsR') ? w.tiles.map((t, i) => (t.camp !== undefined ? [i, t.camp, t.owner] : null)).filter((x) => x) : undefined;
      out({ kind: 'case', day: d, tag, name, rng: s.rng.state(), extra, err, campTiles, w: dumpW });
    }
  }
}
