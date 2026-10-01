// Heroes / Will / Inns port check (TS side): runs the real Sim.step() and writes one JSON line per day:
// rng state, nextId, the day's new events and battles (+ every <every> days a snapshot of heroes, inns, quests,
// agents, camps, civs, settlements, routes, metrics). C# twin: tests/HeroesCheck (commands in its .csproj).
// Run from macro/: node golden/ts/build.mjs heroes_sim && node golden/out/heroes_sim.js <seed> <days> [every] [scen] > ts.ndjson
// scen 1: day-0 injection (all taverns, contacts, gold/food, threat) so heroes/inns/quests get busy early.
// scen 2: scen 1 + civ 0 made a pact civ + daily injections forcing rare branches (revive effect on every civ,
//         periodic XP boosts → legends/retirement, onHomeBurned with camp and civ culprits).
import { Sim } from '../../../src/sim/sim';
import { gainXp } from '../../../src/sim/heroes';
import { onHomeBurned } from '../../../src/sim/will';

const seed = Number(process.argv[2] ?? '1');
const days = Number(process.argv[3] ?? '1000');
const every = Number(process.argv[4] ?? '25');
const scen = Number(process.argv[5] ?? '0');
const sim = new Sim(seed);
const w = sim.w;

function inject() {
  if (scen !== 1 && scen !== 2) return;
  if (scen === 2) w.civs[0].align.good = -0.8;
  for (const a of w.civs) for (const b of w.civs) if (a.id !== b.id) w.relations[a.id][b.id].contact = true;
  w.settlements.forEach((st, i) => {
    st.civics.tavern = 1;
    if (i % 3 === 0) st.civics.temple = 1;
    if (i % 4 === 1) st.civics.library = 1;
  });
  w.civs.forEach((c, i) => {
    sim.add(c, 'gold', 400); sim.add(c, 'grain', 300); sim.add(c, 'potion', 6);
    if (i % 2 === 0) c.threat = 0.5;
  });
  const st = w.settlements[1];
  if (st) st.plague = { since: 0, until: 400, severity: 0.6, dead: 0 };
}

function injectDaily(d: number) {
  if (scen !== 2) return;
  for (const c of w.civs) (c.eff as Record<string, number>).revive = 1;
  if (d % 100 === 50) for (const h of w.heroes) if (h.state !== 'dead' && h.state !== 'gone' && h.state !== 'retired' && h.level < 5) gainXp(sim, h, 1200);
  if (d % 150 === 75) {
    const alive = w.settlements.filter((x) => x.alive);
    if (alive.length) {
      const st = alive[(d / 75) % alive.length];
      const cp = w.camps.find((c) => c.alive);
      onHomeBurned(sim, st, cp, w.civs[(st.civ + 1) % w.civs.length]);
    }
  }
}

const rep = (_k: string, v: unknown) => (typeof v === 'number' && !Number.isFinite(v) ? String(v) : v);
let lastEv = -1, lastBattle = -1;
function dump(full: boolean) {
  const events = w.events.filter((e) => e.id > lastEv);
  for (const e of events) lastEv = Math.max(lastEv, e.id);
  const battles = w.battles.filter((b) => b.id > lastBattle);
  for (const b of battles) lastBattle = Math.max(lastBattle, b.id);
  const o: Record<string, unknown> = { day: w.day, rng: sim.rng.state(), nextId: w.nextId, events, battles };
  if (full) Object.assign(o, { heroes: w.heroes, inns: w.inns, quests: w.quests, agents: w.agents, camps: w.camps, civs: w.civs, settlements: w.settlements, routes: w.routes, metrics: w.metrics, innPlan: w.innPlan });
  process.stdout.write(JSON.stringify(o, rep) + '\n');
}
inject();
dump(true);
for (let d = 1; d <= days; d++) {
  injectDaily(d);
  sim.step();
  dump(d % every === 0);
}
