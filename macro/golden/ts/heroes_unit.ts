// Heroes / Will / Inns unit check (TS side): calls the three modules' functions (exported and private) directly on a
// fresh world with synthetic inns, heroes, goals, routes and raids, forcing rare branches; one JSON line per step.
// C# twin: tests/HeroesCheck (mode "unit": HeroesCheck.dll unit <seed>). Compare with tests/HeroesCheck/heroes_compare.py.
// Build with heroes_build.mjs (it also exports the private helpers): run from macro/:
//   node golden/ts/heroes_build.mjs heroes_unit && node golden/out/heroes_unit.js <seed> > ts.ndjson
import { Sim } from '../../../src/sim/sim';
import * as H from '../../../src/sim/heroes';
import * as W from '../../../src/sim/will';
import * as I from '../../../src/sim/inns';
import { HERO_CLASS_IDS } from '../../../src/data/heroes';

/* eslint-disable @typescript-eslint/no-explicit-any */
const h_ = H as any, w_ = W as any, i_ = I as any;
const seed = Number(process.argv[2] ?? '1');
const sim = new Sim(seed);
const w = sim.w;
const rep = (_k: string, v: unknown) => (typeof v === 'number' && !Number.isFinite(v) ? String(v) : v);
let lastEv = -1;
function out(label: string, o: Record<string, unknown> = {}) {
  const events = w.events.filter((e) => e.id > lastEv);
  for (const e of events) lastEv = Math.max(lastEv, e.id);
  process.stdout.write(JSON.stringify({ label, day: w.day, rng: sim.rng.state(), nextId: w.nextId, events, ...o }, rep) + '\n');
}
const all = () => ({ heroes: w.heroes, agents: w.agents, quests: w.quests, inns: w.inns, civs: w.civs, battles: w.battles, metrics: w.metrics });
const alive = (h: any) => h.state !== 'dead' && h.state !== 'gone' && h.state !== 'retired';
const sts = w.settlements;
const camps = w.camps;
const civs = w.civs;
const RACES = ['human', 'dwarf', 'elf', 'halfling', 'gnome', 'halfelf', 'halforc', 'dragonborn', 'tiefling'];
const PATHS = ['hunter', 'wanderer', 'healer', 'sage', 'mercenary', 'dark'];

// A) rollWill
{
  const wills: unknown[] = [];
  for (const cls of HERO_CLASS_IDS) for (let i = 0; i < 40; i++) { const r = W.rollWill(sim, cls); wills.push([r.align, r.path]); }
  out('rollWill', { wills });
}
// B) makeHero for every race × class × level
{
  let k = 0;
  for (const race of RACES) for (const cls of HERO_CLASS_IDS) for (let level = 1; level <= 5; level++) {
    const st = sts[k % sts.length];
    H.makeHero(sim, { race: race as any, cls, level, tile: st.tile, base: st.id, baseInn: k % 7 === 3 });
    k++;
  }
  out('makeHero', { heroes: w.heroes });
}
// C) gainXp with every growFromExperience branch
{
  w.heroes.forEach((h, i) => {
    const m = i % 5;
    // ties (1.5 vs 1.5) on low-level heroes: the earlier option must win
    if (m === 0) { if (i % 10 === 5) { h.tally.goblin = 5; h.tally.plague = 1; } else { h.tally.goblin = 3; h.tally.kills = 4; } }
    else if (m === 1) { if (i % 10 === 6) { h.tally.plague = 1; h.tally.lib = 1; } else { h.tally.plague = 1; h.tally.temple = 1; } }
    else if (m === 2) { h.tally.ruins = 1; h.tally.rob = 1; }
    else if (m === 3) { h.tally.lib = 1; }
    if (i % 7 === 0) h.civ = i % civs.length;
  });
  w.heroes.forEach((h, i) => H.gainXp(sim, h, 250 * (1 + (i % 60))));
  out('gainXp', { heroes: w.heroes });
}
// D) labels, costs, willServe, isPact
{
  w.heroes.forEach((h, i) => { if (i % 3 === 0) h.epithet = 'Kırık Belası'; if (i % 4 === 1) h.grudge = i % civs.length; });
  civs.forEach((c, i) => { if (i % 3 === 1) c.align.good = -0.4; if (i % 3 === 2) c.align.good = -0.6; });
  const costs: unknown[] = [];
  for (const c of civs) for (const h of w.heroes.slice(0, 60)) { const k = H.heroCost(sim, c, h); costs.push([k.gold, k.food, W.heroWillServe(c, h), W.heroLabel(h), H.heroBaseCost(h), W.hasTrait(h, 'legend'), I.isPact(sim, c)]); }
  out('costs', { costs });
}
// E) afterCampFight
{
  for (let j = 0; j < 40; j++) {
    const cp = camps[j % camps.length];
    if (j % 8 === 5) cp.kind = 'hobgoblin';
    const hs = w.heroes.slice(j * 3, j * 3 + 3);
    if (j % 5 === 0) for (const h of hs) { h.civ = -1; h.align = 'evil'; h.path = 'hunter'; h.tally.fails = 1; }
    if (j % 6 === 0) hs[1].vendetta = cp.id;
    const side = hs.map((h) => W.heroSide(h, 'A', cp.kind, j % 2 === 0));
    side.forEach((x, q) => { x.kills = ((j + q) % 4) * 4; });
    if (j % 3 === 0) hs[0].state = 'dead';
    if (j % 9 === 3) hs[2].state = 'dead';
    const rolls = [{ d20: 20, side: 'A' as const, who: hs[1].name }, { d20: 13, side: 'B' as const, who: 'x' }];
    W.afterCampFight(sim, side, cp, j % 2 === 0, j % 4 < 2, rolls);
    for (const h of hs) if (h.state === 'dead') h.state = 'tavern';
  }
  out('afterCampFight', { heroes: w.heroes });
}
// F) onHomeBurned
{
  sts.slice(0, 6).forEach((st, j) => W.onHomeBurned(sim, st, j % 2 === 0 ? camps[j % camps.length] : undefined, j % 3 !== 1 ? civs[(st.civ + 1) % civs.length] : undefined));
  out('onHomeBurned', { heroes: w.heroes });
}
// G) threatCamp / campPower / campForce
{
  const tc = civs.map((c) => H.threatCamp(sim, c)?.id ?? -1);
  const cpw = camps.map((cp) => [H.campPower(sim, cp), H.campPower(sim, cp, 12.5), H.campForce(sim, cp).length]);
  out('camps', { tc, cpw });
}
// H) synthetic inns, taverns, temples, libraries
function mkInn(k: number) {
  const st = sts[k % sts.length];
  const cand = sim.g.within(st.tile, 3).filter((t) => !w.tiles[t].sea && w.tiles[t].terrain !== 'water' && w.tiles[t].terrain !== 'mountain' && sim.g.dist(t, st.tile) === 3 && w.tiles[t].inn === undefined);
  const tile = cand.length ? cand[0] : st.tile;
  const inn: any = {
    id: sim.id(), tile, name: `Deneme${k}`, keeper: `Hancı${k}`, founded: 0, alive: true, gold: 60 + 90 * k, raids: 0, stage: 'open', level: 1 + (k % 3),
    keeperRace: 'human', origin: st.id, originName: st.name, stock: { food: 40, ale: 40, wood: 5 }, fame: 25 * k, staff: [], guests: [], tabs: {}, log: [],
    books: [], hist: [], total: { guests: 0, nights: 0, income: 0, turned: 0, bought: 0 }, turned: 0, sat: 0.5, traffic: 0,
  };
  w.inns.push(inn); w.tiles[tile].inn = inn.id;
  return inn;
}
const inns = [mkInn(0), mkInn(1), mkInn(2), mkInn(3)];
sts.forEach((st, i) => { if (i % 2 === 0) st.civics.tavern = 1; if (i % 3 === 0) st.civics.temple = 1; if (i % 4 === 1) st.civics.library = 1; });
// I) heroes into inns; basics
{
  w.heroes.forEach((h, i) => {
    if (h.state === 'dead') return;
    if (i % 4 === 0) { const inn = inns[i % inns.length]; h.base = inn.id; h.baseInn = true; h.pos = inn.tile; h.tavern = inn.id; h.state = 'tavern'; h.civ = -1; h.contract = undefined; }
  });
  const nr: unknown[] = [];
  for (let t = 0; t < w.tiles.length; t += 97) { const r = W.nearestRest(sim, t); nr.push(r ? [r.id, r.tile, r.inn] : null); }
  const bt = w.heroes.map((h) => W.baseTile(sim, h) ?? -1);
  const ia = inns.map((inn) => W.innAt(sim, inn.tile)?.id ?? -1).concat([W.innAt(sim, 0)?.id ?? -1]);
  const pools = inns.map((inn) => I.innPool(sim, inn).map((h) => h.id));
  const iho = civs.map((c) => I.innHeroesOf(sim, c).map((h) => h.id));
  const names = inns.map((inn) => I.innName(inn));
  const byId = [W.innById(sim, inns[2].id)?.name ?? '-', W.innById(sim, undefined)?.name ?? '-', W.innById(sim, -5)?.name ?? '-'];
  const dp = [w_.distPen(sim, 0, 500), w_.distPen(sim, sts[0].tile, sts[1 % sts.length].tile)];
  out('innsBasics', { nr, bt, ia, pools, iho, names, byId, dp });
}
// J) spawns (inn heroes with and without a retired teacher, tavern heroes)
{
  const rh = w.heroes[5]; rh.state = 'retired'; inns[1].teacher = rh.id;
  inns[3].teacher = w.heroes[6].id;
  for (let i = 0; i < 12; i++) i_.spawnInnHero(sim, inns[i % inns.length]);
  for (let i = 0; i < 6; i++) H.spawnHero(sim, sts[i % sts.length]);
  out('spawn', { heroes: w.heroes.slice(-18) });
}
// K) auctions and bids
{
  w.heroes.forEach((h, i) => { if (i % 5 === 0) h.rep[i % civs.length] = 3; });
  for (const h of w.heroes) if (h.baseInn && h.civ === -1 && h.state === 'tavern' && !h.auction) h.auction = { end: sim.day + 120, bids: [] };
  civs.forEach((c, i) => { sim.add(c, 'gold', 300 + 50 * i); c.threat = i % 2 ? 0.5 : 0.1; });
  for (let r = 0; r < 3; r++) for (const c of civs) I.considerInnBids(sim, c);
  const eff = w.heroes.filter((h) => h.auction && h.auction.bids.length).map((h) => h.auction!.bids.map((b) => i_.effBid(sim, h, b)));
  out('bids', { auctions: w.heroes.map((h) => h.auction ?? null), eff });
}
// L) closeAuction (one bidder busy in a party)
{
  const busy = w.heroes.find((h) => h.auction && h.auction.bids.length);
  if (busy) w.agents.push({ id: sim.id(), kind: 'party', civ: -1, path: [busy.pos], step: 0, progress: 0, speed: 0.85, heroes: [busy.id], purpose: 'quest' } as any);
  for (const h of w.heroes.slice()) if (h.auction) { const inn = W.innById(sim, h.base); if (inn) i_.closeAuction(sim, inn, h); }
  out('closeAuction', { ...all(), gold: civs.map((c) => sim.st(c, 'gold')) });
}
// M) contractEnd (renew, leave, retire)
{
  w.heroes.forEach((h, i) => { if (h.contract) { h.state = 'home'; if (i % 2 === 0) { h.level = Math.max(h.level, 4); h.born = -2000; } h.rep[h.contract.civ] = i % 4; } });
  for (const h of w.heroes.slice()) if (h.contract) i_.contractEnd(sim, h);
  out('contractEnd', all());
}
// N) postGuardQuest (topping a civ quest, posting inn quests)
{
  w.quests.push({ id: sim.id(), civ: 0, camp: camps[0].id, bounty: 50, posted: 0, takenBy: [], open: true, inn: inns[2].id, expires: 360 });
  inns[2].gold = 400; inns[0].gold = 30;
  const near1 = sim.g.within(inns[1].tile, 4).filter((t) => !w.tiles[t].sea && w.tiles[t].terrain !== 'water' && sim.g.dist(t, inns[1].tile) === 4 && w.tiles[t].camp === undefined && w.tiles[t].inn === undefined);
  const mv = camps.find((c) => c.alive && sim.g.dist(c.tile, inns[1].tile) > 12);
  if (near1.length && mv) { delete w.tiles[mv.tile].camp; mv.tile = near1[0]; w.tiles[near1[0]].camp = mv.id; }
  for (const inn of inns) i_.postGuardQuest(sim, inn);
  out('postGuardQuest', { quests: w.quests, inns: w.inns });
}
// O) considerHero / considerQuest
{
  civs.forEach((c, i) => { c.threat = [0.1, 0.3, 0.5][i % 3]; sim.add(c, 'grain', 200); });
  w.heroes.forEach((h, i) => { if (h.civ >= 0 && alive(h) && i % 2 === 0) h.state = 'home'; });
  for (const c of civs) if (c.alive && sim.civSettlements(c).length) { H.considerHero(sim, c); H.considerQuest(sim, c); }
  out('consider', all());
}
// P) questFailed
{
  for (const q of w.quests.slice()) H.questFailed(sim, q);
  out('questFailed', { quests: w.quests, inns: w.inns, gold: civs.map((c) => sim.st(c, 'gold')) });
}
// Q) chooseGoal over free heroes (all paths; plague, ruin, temple, library, rob route, dark heroes)
const ruinT = sim.g.within(sts[0].tile, 5).filter((t) => !w.tiles[t].sea && w.tiles[t].terrain !== 'water' && sim.g.dist(t, sts[0].tile) === 5 && w.tiles[t].camp === undefined)[0] ?? sts[0].tile;
const ruin: any = { id: sim.id(), civ: 0, name: 'Yıkıkköy', tile: ruinT, founded: 0, pop: {}, growthAcc: 0, civics: {}, workshops: {}, project: null, jobs: {}, soldiers: 0, alive: false, starving: 0, tier: 0, mixedSince: {} };
sts.push(ruin);
const routePath = sim.path(sts[0].tile, sts[1].tile) ?? [];
const route: any = { id: sim.id(), a: sts[0].id, b: sts[1].id, kind: 'trade', path: routePath, nextDepart: 0, trips: 0, alive: true, since: 0 };
w.routes.push(route);
{
  sts[1].plague = { since: 0, until: 300, severity: 0.7, dead: 0 };
  w.heroes.forEach((h, i) => {
    if (h.civ !== -1 || !alive(h)) return;
    h.path = PATHS[i % 6] as any;
    if (i % 11 === 0) h.align = 'good';
    if (h.path === 'dark') h.tally.rob = 1;
    if (i % 13 === 0) h.soloUntil = 500;
    h.hp = h.maxHp;
    h.state = 'tavern';
    if (i % 3 === 0) { h.pos = sts[i % sts.length].tile; h.tavern = sts[i % sts.length].id; }
  });
  for (const h of w.heroes.slice()) if (h.civ === -1 && h.state === 'tavern') W.chooseGoal(sim, h);
  out('chooseGoal', all());
}
// R) arriveGoal for every kind, progressGoal (library, rob with a caravan), duel
{
  const free = w.heroes.filter((h) => h.civ === -1 && alive(h));
  const kinds = ['ruin', 'plague', 'temple', 'library', 'rob', 'quest'];
  const mid = routePath.length ? routePath[Math.floor(routePath.length / 2)] : sts[0].tile;
  free.slice(0, 24).forEach((h, i) => {
    const kind = kinds[i % kinds.length];
    const target = kind === 'ruin' ? ruin.id : kind === 'plague' ? sts[1].id : kind === 'temple' || kind === 'library' ? sts[i % sts.length].id : kind === 'rob' ? route.id : camps[0].id;
    h.goal = { kind: kind as any, tile: h.pos, target, text: `test ${kind}`, since: 0 };
    if (i % 12 === 6) h.tally.ruins = 2;
    W.arriveGoal(sim, h);
  });
  out('arriveGoal', all());
  // duel: a good hunter meets a dark hero on the same tile
  const good = free.filter((h) => alive(h) && h.align === 'good').slice(0, 3);
  const dark = free.filter((h) => alive(h) && h.align !== 'good').slice(0, 3);
  good.forEach((g, i) => {
    const o = dark[i]; if (!o) return;
    o.pos = g.pos; o.state = i === 1 ? 'traveling' : 'tavern';
    g.goal = { kind: 'duel', tile: g.pos, target: o.id, text: 'düello', since: 0 };
    W.arriveGoal(sim, g);
  });
  // a duel whose target is gone
  if (good[0] && alive(good[0])) { good[0].goal = { kind: 'duel', tile: good[0].pos, target: -77, text: 'yok', since: 0 }; W.arriveGoal(sim, good[0]); }
  out('duel', all());
  // progressGoal: library (stay over) and rob with a caravan nearby
  w.day = 30;
  free.forEach((h, i) => {
    if (!alive(h) || !h.goal) return;
    if (h.goal.kind === 'library') { h.goal.stay = 10; w_.progressGoal(sim, h); }
    else if (h.goal.kind === 'rob' && i % 2 === 0) {
      const cvPath = routePath.length ? routePath : [h.pos];
      const step = Math.floor(cvPath.length / 2);
      h.pos = cvPath[step];
      w.agents.push({ id: sim.id(), kind: 'caravan', civ: sts[0].civ, path: cvPath, step, progress: 0, speed: 0.65, cargo: { grain: 10, iron: 3 }, troops: 1 + (i % 3), from: sts[0].id, to: sts[1].id, route: route.id } as any);
      w_.progressGoal(sim, h);
    } else w_.progressGoal(sim, h);
  });
  out('progressGoal', all());
}
// S) maybeRetire, tryRevive, sendHero / returnToBase / heroAgent
{
  w.day = 1500;
  const olds = w.heroes.filter((h) => alive(h)).slice(0, 40);
  olds.forEach((h, i) => { h.level = 4 + (i % 2); h.born = 0; if (i % 3 === 0) { h.baseInn = true; h.base = inns[i % inns.length].id; } });
  inns[0].teacher = undefined;
  const ret = olds.map((h) => W.maybeRetire(sim, h));
  out('maybeRetire', { ret, ...all() });
  (civs[0].eff as any).revive = 1; (civs[1 % civs.length].eff as any).revive = 1;
  const cand = w.heroes.filter((h) => alive(h)).slice(0, 12);
  cand.forEach((h, i) => { h.civ = i % 3 === 2 ? -1 : i % 2; if (i === 4) h.revived = true; });
  const rev = cand.map((h) => H.tryRevive(sim, h));
  const ha = w.heroes.slice(0, 50).map((h) => W.heroAgent(sim, h)?.id ?? -1);
  cand.forEach((h, i) => { if (i % 2 === 0) H.sendHero(sim, h, sts[i % sts.length].tile, 'home'); else W.returnToBase(sim, h); });
  out('revive', { rev, ha, ...all() });
}
// T) monster raid on an inn (won, lost → ruinInn), pact inn raid (considerInnRaid → innRaidArrive), breakGuestRight
{
  w.day = 720;
  for (const h of w.heroes) if (h.civ === -1 && alive(h) && h.baseInn) { const inn = W.innById(sim, h.base); if (inn) { h.state = 'tavern'; h.pos = inn.tile; h.tavern = h.base; } }
  const mk = (inn: any, troops: number, boss: boolean, monster: string) => ({ id: sim.id(), kind: 'raid', civ: -1, path: [inn.tile], step: 0, progress: 0, speed: 0.9, troops, from: camps[0].id, to: inn.id, purpose: 'inn', boss, monster, targetTile: inn.tile } as any);
  const r1 = mk(inns[1], 2, false, 'goblin'); w.agents.push(r1); I.innMonsterRaid(sim, r1);
  const r2 = mk(inns[2], 16, true, 'hobgoblin'); w.agents.push(r2); I.innMonsterRaid(sim, r2);
  const r3 = mk(inns[2], 3, false, 'bugbear'); w.agents.push(r3); I.innMonsterRaid(sim, r3);
  out('monsterRaid', { ...all(), tiles: inns.map((inn) => w.tiles[inn.tile]) });
  const pc = civs[0];
  pc.align.good = -0.8;
  for (const st of sim.civSettlements(pc)) st.soldiers = 14;
  for (const o of civs) if (o.id !== pc.id) { w.relations[pc.id][o.id].war = null; w.relations[o.id][pc.id].war = null; }
  pc.innBanUntil = undefined;
  inns[0].alive = true; inns[0].gold = 500;
  let kept = 0;
  for (const h of w.heroes) if (h.tavern === inns[0].id && h.state === 'tavern') { if (kept++ >= 1) h.state = 'traveling'; }
  let tries = 0;
  while (tries < 300 && !w.agents.some((a) => a.civ === pc.id && a.purpose === 'innraid')) { i_.considerInnRaid(sim, pc); tries++; }
  const raid = w.agents.find((a) => a.civ === pc.id && a.purpose === 'innraid');
  if (raid) I.innRaidArrive(sim, raid);
  (civs[1 % civs.length].eff as any).crusade = 1;
  I.breakGuestRight(sim, civs[1 % civs.length], inns[3]);
  const defs = inns.map((inn) => i_.innDefenders(sim, inn, 'B').length);
  out('innRaid', { tries, defs, ...all(), relations: w.relations });
}
// U) the ticks on chosen days (taverns, inns incl. expiring quests and auctions, heroes incl. migration)
{
  for (const d of [735, 750, 840, 960, 1080, 1200, 1500, 1800]) {
    w.day = d;
    H.tavernsTick(sim);
    I.innsTick(sim);
    H.heroesTick(sim);
    out(`ticks${d}`, all());
  }
}
// V) nowhere to rest: every inn closed, no taverns
{
  for (const inn of w.inns) inn.alive = false;
  for (const st of sts) delete st.civics.tavern;
  const lost = w.heroes.filter((h) => alive(h)).slice(0, 3);
  for (const h of lost) W.returnToBase(sim, h);
  const nr2 = W.nearestRest(sim, 0);
  out('noRest', { heroes: lost, nr2: nr2 ? [nr2.id] : null });
}
