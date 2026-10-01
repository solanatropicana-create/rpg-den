// InnLife port check (TS side): a hero-less world driven day by day with the real Sim core and the already
// ported modules (economy, research, gear, events, diplomacy, monsters, combat, agents) plus innLifeTick,
// which Sim.step normally reaches through innsTick. Skipped (not modelled by the C# harness stand-ins):
// tavernsTick / heroesTick / considerHero / considerQuest (no heroes are ever born), innsTick's hero pool,
// auctions, board quests and pact raids (innLifeTick is called directly instead), considerInnBids and seaTick.
// Monster raids on inns still happen (agents -> innMonsterRaid -> ruinInn -> innScatter).
// One JSON line per day: rng/nextId/metrics/innPlan/inns/inn-related agents/new events every day,
// all agents every 10 days, civs/settlements/routes/camps/quests every 30 days, tiles every 90 days.
// scen=1 adds a scripted hero scenario (campsTick is skipped then: hero defenders would need Will/Heroes code):
// 8 heroes (every class) visit the first open inn on a schedule, leave as dead/retired/gone/contracted/with a goal,
// earn gold now and then (tabs, poor-hero work, rogue theft); new keepers' sites are invalidated (target tile only,
// or the whole radius-4 disk) to reach keeperArrive's re-route / give-up branches; innIncome and a non-ruin
// innScatter hit that inn periodically; innDetour is called on every live route every 10 days (result length and
// whether it is the same array instance) and the heroes are dumped every day.
// scen=2 adds targeted perturbations at that inn: unpaid wages with several hired hands (who leaves), a busy
// history (extra waiter), low gold at hiring time, a starving neighbour during innLifeTick (refugees), crafted
// supply carts (bread/beer/fish/meat/stone/grain/wood, empty, unknown origin), a guest without destination, a
// traveler without guest, a keeper whose inn is gone, whereStr with 0/1 live settlements, innDetour on a short path.
// C# twin: tests/InnLifeCheck (`world` mode). Run from macro/:
//   node golden/ts/build.mjs innlife_world && node golden/out/innlife_world.js <seed> <days> [scen] [innTarget] > ts.ndjson
import { Sim, YEAR } from '../../../src/sim/sim';
import { economyTick, repairTick, chooseBuilds, recruitTick } from '../../../src/sim/economy';
import { chooseResearch, eraCheck, classYearly } from '../../../src/sim/research';
import { disastersTick } from '../../../src/sim/events';
import { relationsTick, worldTick, considerExpansion, considerTrade, considerWarAction, considerScout, considerRaid } from '../../../src/sim/diplomacy';
import { campsTick } from '../../../src/sim/monsters';
import { agentsTick, routesTick, roadsTick } from '../../../src/sim/agents';
import { innLifeTick, innIncome, innScatter, innDetour, whereStr } from '../../../src/sim/innlife';
import type { Agent, Civ, Hero, Inn, Settlement } from '../../../src/sim/types';
import type { RaceId } from '../../../src/data/classes';
import type { HeroClass } from '../../../src/data/heroes';

const seed = Number(process.argv[2] ?? '1');
const days = Number(process.argv[3] ?? '600');
const scen = Number(process.argv[4] ?? '0');
const target = process.argv[5] !== undefined ? Number(process.argv[5]) : undefined;   // optional innPlan.target override
const sim = new Sim(seed);
const w = sim.w;
if (target !== undefined) { w.innPlan.target = target; w.innPlan.wave = Math.max(w.innPlan.wave, Math.min(target, 4)); }

function civAI(c: Civ) {
  if (!sim.civSettlements(c).length) { sim.extinct(c); return; }
  if (!c.research.current) chooseResearch(sim, c);
  eraCheck(sim, c);
  chooseBuilds(sim, c);
  recruitTick(sim, c);
  considerExpansion(sim, c);
  considerScout(sim, c);
  considerTrade(sim, c);
  considerWarAction(sim, c);
  considerRaid(sim, c);
}

// ------------------------------------------------------------ scripted hero scenario (scen=1)
const CLS: HeroClass[] = ['ranger', 'fighter', 'rogue', 'cleric', 'wizard', 'paladin', 'druid', 'barbarian'];
const GOLD = [0, 0.5, 3, 25, 0, 1, 0, 12];
const NAMES = ['Aldric', 'Mira', 'Tomas', 'Elena', 'Borin', 'Helga', 'Pip', 'Gruk'];
const RACES: RaceId[] = ['human', 'human', 'human', 'elf', 'dwarf', 'dwarf', 'halfling', 'halforc'];
function mkHero(k: number, inn: Inn): Hero {
  return {
    id: 900000 + k, name: NAMES[k], race: RACES[k], cls: CLS[k], level: 1 + (k % 4), xp: 0, stats: { str: 10, dex: 10, con: 10, int: 10, wis: 10, cha: 10 },
    maxHp: 20, hp: 20, ac: 12, civ: -1, pos: inn.tile, tavern: inn.id, state: 'tavern', born: k === 0 ? w.day : 0, idleSince: w.day, kills: 0, gold: GOLD[k], bio: 'test',
    align: 'neutral', path: 'wanderer', traits: [], epithet: k % 3 === 1 ? 'Cesur' : undefined, tally: {}, bonus: { atk: 0 }, rep: {},
    journal: k % 2 === 0 ? [{ day: 0, text: `günlük ${k}` }] : [], base: k === 4 ? inn.id : -5, baseInn: k === 4, birth: -5, legend: k === 4 ? true : undefined,
  };
}
const touched = new Set<number>();
let tempCiv: Hero | undefined;
function scriptBefore() {
  const d = w.day;
  for (const a of w.agents) {
    if (a.kind !== 'keeper' || a.dead || touched.has(a.id)) continue;
    touched.add(a.id);
    const tt = a.targetTile!;
    if (a.id % 3 === 0) w.tiles[tt].innZone = 999999;
    else if (a.id % 3 === 1) for (const t of sim.g.within(tt, 4)) w.tiles[t].innZone = 999999;
  }
  const hinn = w.inns.find((i) => i.alive);
  if (!hinn) return;
  if (!w.heroes.length) for (let k = 0; k < 8; k++) w.heroes.push(mkHero(k, hinn));
  for (const h of w.heroes) {
    const k = h.id - 900000;
    const present = (d + k * 7) % 40 < 25;
    h.civ = -1; h.tavern = hinn.id; h.state = present ? 'tavern' : 'traveling';
    h.goal = !present && k === 2 ? { kind: 'ruin', tile: hinn.tile, text: 'eski harabeyi keşfetmek', since: d } : undefined;
    if (k === 5 && d % 97 === 13) h.state = 'dead';
    if (k === 6 && d % 83 === 29) h.state = 'retired';
    if (k === 7 && d % 71 === 41) h.state = 'gone';
    if (k === 3 && d % 59 === 17) { h.civ = 0; h.state = 'home'; tempCiv = h; }
    if (d % 50 === 0) h.gold += k * 3;
  }
  if (d % 97 === 50) innIncome(sim, hinn, 12.5, `Test geliri ${d}`);
  if (d % 131 === 70) innScatter(sim, hinn, 'Test baskını', false);
  if (scen >= 2) script2(d, hinn);
}
function nearestAlive(tile: number): Settlement | undefined {
  return w.settlements.filter((x) => x.alive).sort((a, b) => sim.g.dist(a.tile, tile) - sim.g.dist(b.tile, tile) || a.id - b.id)[0];
}
let starved: { st: Settlement; old: number } | undefined;
function script2(d: number, hinn: Inn) {
  const near = nearestAlive(hinn.tile);
  if ((d + hinn.id) % 30 === 0) {
    const phase = Math.floor(d / 30) % 3;
    if (phase === 0) {
      hinn.staff.push({ name: `Ekstra${d}a`, race: 'human', role: 'asci', since: d - 5, from: 'test' }, { name: `Ekstra${d}b`, race: 'elf', role: 'garson', since: d - 2, from: 'test' }, { name: `Ekstra${d}c`, race: 'gnome', role: 'bekci', since: d - 2, from: 'test' });
      hinn.gold = 1;
    } else if (phase === 1 && hinn.hist.length >= 6) {
      for (const h of hinn.hist.slice(-6)) h.guests = 99;
      hinn.gold = Math.max(hinn.gold, 200);
    }
  }
  if (d % 5 === 0 && near) { starved = { st: near, old: near.starving }; near.starving = 2; }
  if (!near) return;
  const path = sim.path(near.tile, hinn.tile);
  if (!path || path.length < 2) return;
  const push = (a: Partial<Agent>) => w.agents.push({ id: sim.id(), civ: near.civ, path, step: 0, progress: 0, speed: 0.7, ...a } as Agent);
  if (d % 150 === 75) push({ kind: 'supply', cargo: { bread: 3, beer: 2, fish: 1, meat: 1, stone: 2, wood: 3, grain: 5 }, from: near.id, to: hinn.id, inn: hinn.id, purpose: 'supply' });
  if (d % 150 === 76) push({ kind: 'supply', cargo: {}, from: -1, to: hinn.id, inn: hinn.id, purpose: 'supply' });
  if (d % 150 === 77) push({ kind: 'traveler', from: near.id, to: hinn.id, inn: hinn.id, purpose: 'come', guest: { id: sim.id(), kind: 'wanderer', name: 'Yabancı', race: 'elf', n: 1, civ: -1, from: -1, fromName: 'uzaklar', to: -1, toName: 'bilinmez', why: 'test', purse: 5, spent: 0, nights: 2, arrived: d, mood: 0.7 } });
  if (d % 150 === 78) push({ kind: 'traveler', from: near.id, to: hinn.id, inn: hinn.id, purpose: 'come' });
  if (d % 150 === 79) push({ kind: 'keeper', from: near.id, to: 424242, inn: 424242, purpose: 'found', targetTile: hinn.tile });
}
function scriptAfter() {
  if (tempCiv) { tempCiv.civ = -1; tempCiv = undefined; }
  if (starved) { starved.st.starving = starved.old; starved = undefined; }
}
function probes(d: number) {
  if (scen < 2 || d % 100 !== 0) return undefined;
  const hinn = w.inns.find((i) => i.alive);
  const tile = hinn ? hinn.tile : w.settlements[0].tile;
  const alive = w.settlements.map((x) => x.alive);
  for (const x of w.settlements) x.alive = false;
  const r0 = whereStr(sim, tile);
  w.settlements[0].alive = true;
  const r1 = whereStr(sim, tile);
  w.settlements.forEach((x, i) => { x.alive = alive[i]; });
  const short = [tile, tile, tile];
  return { r0, r1, shortSame: innDetour(sim, short) === short };
}

function step() {
  w.day++;
  for (const c of w.civs) if (c.alive) economyTick(sim, c);
  if (w.day % 5 === 0) for (const c of w.civs) if (c.alive) repairTick(sim, c);
  for (const c of w.civs) if (c.alive && (w.day + c.id * 3) % 10 === 0) civAI(c);
  if (w.day % 10 === 0) { sim.updateTerritory(); relationsTick(sim); }
  if (w.day % 30 === 0) { sim.discover(); worldTick(sim); disastersTick(sim); }
  if (w.day % YEAR === 0) for (const c of w.civs) if (c.alive) classYearly(sim, c);
  if (!scen) campsTick(sim);
  if (scen) scriptBefore();
  innLifeTick(sim);
  if (scen) scriptAfter();
  agentsTick(sim);
  routesTick(sim);
  roadsTick(sim);
  for (const c of w.civs) c.threat = Math.min(1.5, Math.max(0, c.threat - 0.0015));
  if (w.day % 60 === 0) for (const c of w.civs) if (c.alive) c.history.push({ day: w.day, pop: sim.civPop(c), gold: Math.round(sim.st(c, 'gold')), techs: c.research.done.length });
  w.rngState = sim.rng.state();
}

const INN_AGENTS = new Set(['keeper', 'traveler', 'supply', 'caravan']);
let lastEv = -1;
function dump() {
  const d = w.day;
  const events = w.events.filter((e) => e.id > lastEv);
  for (const e of events) lastEv = Math.max(lastEv, e.id);
  process.stdout.write(JSON.stringify({
    day: d, rng: sim.rng.state(), nextId: w.nextId, metrics: w.metrics, innPlan: w.innPlan, inns: w.inns,
    agents: d % 10 === 0 ? w.agents : w.agents.filter((a) => INN_AGENTS.has(a.kind)), events,
    civs: d % 30 === 0 ? w.civs : undefined, settlements: d % 30 === 0 ? w.settlements : undefined,
    routes: d % 30 === 0 ? w.routes : undefined, camps: d % 30 === 0 ? w.camps : undefined, quests: d % 30 === 0 ? w.quests : undefined,
    tiles: d % 90 === 0 ? w.tiles : undefined,
    heroes: scen ? w.heroes : undefined,
    probes: probes(d),
    detour: scen && d % 10 === 0 ? w.routes.filter((r) => r.alive).map((r) => { const p = innDetour(sim, r.path); return { id: r.id, same: p === r.path, len: p.length, head: p.slice(0, 3) }; }) : undefined,
  }) + '\n');
}

dump();
for (let k = 1; k <= days; k++) {
  try { step(); } catch (e) { process.stdout.write(JSON.stringify({ day: w.day, error: String(e) }) + '\n'); break; }
  dump();
}
