// Economy / Research / Gear / Events port check (TS side). Drives only those modules' ticks day by day
// (plus Sim core: updateTerritory, discover) and writes one JSON line per day (full tiles every 10 days,
// otherwise only tiles with ext/cutDay). C# twin: tests/EconCheck (run/compare commands in its .csproj).
// Run from macro/: node golden/ts/build.mjs econ_diff && node golden/out/econ_diff.js <seed> <days> > ts.ndjson
import { Sim, YEAR } from '../../../src/sim/sim';
import { economyTick, repairTick, chooseBuilds, recruitTick, regrowForests } from '../../../src/sim/economy';
import { chooseResearch, eraCheck, classYearly } from '../../../src/sim/research';
import { disastersTick } from '../../../src/sim/events';

const seed = Number(process.argv[2] ?? '1');
const days = Number(process.argv[3] ?? '300');
const scen = Number(process.argv[4] ?? '0');   // 1: contacts, war 0-1, threat, extra stock/pop, periodic famine
const tilesEvery = Number(process.argv[5] ?? '10');
const sim = new Sim(seed);
const w = sim.w;
const BONUS: [string, number][] = [['gold', 300], ['iron', 60], ['copper', 40], ['tin', 25], ['leather', 40], ['wood', 150], ['stone', 150], ['bricks', 40], ['tools', 12], ['mana', 12], ['mithril', 6], ['herbs', 25], ['potion', 12], ['salt', 6], ['horses', 6], ['arms', 12]];
function inject(day: number) {
  if (!scen) return;
  if (day === 0) {
    for (const a of w.civs) for (const b of w.civs) if (a.id !== b.id) w.relations[a.id][b.id].contact = true;
    const war = { since: 0, attacker: 0, target: 1, attacks: 0, lastArmy: 0, goal: 'land' };
    w.relations[0][1].war = war; w.relations[1][0].war = war;
    for (const c of w.civs) {
      if (c.id % 2 === 0) c.threat = 0.8;
      for (const [g, v] of BONUS) sim.add(c, g as never, v);
      const cap = sim.capital(c); if (cap) sim.addPop(cap, c.race, 30);
    }
  }
  if (day % 400 === 200) { const c = w.civs[Math.floor(day / 400) % w.civs.length]; for (const g of ['bread', 'fish', 'meat', 'grain']) c.stock[g as never] = 0; }
}
let lastEv = -1;
function dump(full: boolean) {
  const events = w.events.filter((e) => e.id > lastEv);
  for (const e of events) lastEv = Math.max(lastEv, e.id);
  let ext: Record<number, unknown> | undefined;
  if (!full) { ext = {}; w.tiles.forEach((t, i) => { if (t.ext || t.cutDay !== undefined) ext![i] = t; }); }
  process.stdout.write(JSON.stringify({
    day: w.day, rng: sim.rng.state(), nextId: w.nextId, metrics: w.metrics,
    civs: w.civs, settlements: w.settlements, agents: w.agents, events,
    deposits: full ? w.deposits : undefined, tiles: full ? w.tiles : undefined, ext,
    relations: full ? w.relations : undefined,
  }) + '\n');
}
inject(0);
dump(true);
for (let d = 1; d <= days; d++) {
  w.day++;
  inject(w.day);
  for (const c of w.civs) if (c.alive) economyTick(sim, c);
  if (w.day % 5 === 0) for (const c of w.civs) if (c.alive) repairTick(sim, c);
  for (const c of w.civs) {
    if (!(c.alive && (w.day + c.id * 3) % 10 === 0)) continue;
    if (!sim.civSettlements(c).length) { sim.extinct(c); continue; }
    if (!c.research.current) chooseResearch(sim, c);
    eraCheck(sim, c);
    chooseBuilds(sim, c);
    recruitTick(sim, c);
  }
  if (w.day % 10 === 0) sim.updateTerritory();
  if (w.day % 30 === 0) { sim.discover(); regrowForests(sim); disastersTick(sim); }
  if (w.day % YEAR === 0) for (const c of w.civs) if (c.alive) classYearly(sim, c);
  w.rngState = sim.rng.state();
  dump(w.day % tilesEvery === 0);
}
