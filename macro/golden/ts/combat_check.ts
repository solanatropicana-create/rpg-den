// Combat port check (TS side): random battles through the real unit() / monsterSide() / heroCombatant() +
// resolveBattle(), plus grids for profBonus / heroAc / unit / heroCombatant / monsterSide / powerOf / powerVs /
// avgAc / winChance. One JSON line per case: { k, spec, out }. The C# twin (tests/CombatCheck) reads the same
// file, rebuilds every case from `spec` and writes { k, out }; combat_compare.py diffs the two `out`s.
// Run from macro/:
//   node golden/ts/build.mjs combat_check && node golden/out/combat_check.js 5000 20260930 > /tmp/combat_ts.ndjson
//   dotnet build tests/CombatCheck -c Release && dotnet tests/CombatCheck/bin/Release/net8.0/CombatCheck.dll /tmp/combat_ts.ndjson > /tmp/combat_cs.ndjson
//   python3 tests/CombatCheck/combat_compare.py /tmp/combat_ts.ndjson /tmp/combat_cs.ndjson
import { Rng, mod } from '../../../src/sim/rng';
import { unit, heroCombatant, heroAc, avgAc, powerOf, powerVs, winChance, profBonus, resolveBattle, type Combatant, type UnitStats } from '../../../src/sim/combat';
import { monsterSide } from '../../../src/sim/monsters';
import { UNITS, MONSTERS } from '../../../src/data/classes';
import type { Hero } from '../../../src/sim/types';

/* eslint-disable @typescript-eslint/no-explicit-any */
const N = Number(process.argv[2] ?? '5000');
const G = new Rng(Number(process.argv[3] ?? '20260930'));   // generator (separate from the battle RNGs)

const ri = (a: number, b: number) => G.int(a, b);
const pick = <T>(xs: readonly T[]): T => xs[Math.floor(G.next() * xs.length)];
const chance = (p: number) => G.next() < p;
const other = (s: string) => (s === 'A' ? 'B' : 'A');

const UNIT_IDS = Object.keys(UNITS);
const MON_IDS = Object.keys(MONSTERS);
const KINDS = ['soldier', 'militia', 'unique', 'monster', 'boss', 'galley', 'cog', 'ballista'];
const HCLS = ['fighter', 'wizard', 'cleric', 'rogue', 'ranger', 'paladin', 'druid', 'barbarian'];
const STATS = ['str', 'dex', 'con', 'int', 'wis', 'cha'];
const NAMES = ['Aras', 'Bora', 'Ceren', 'Deniz', 'Ece', 'Ilgaz', 'Işık', 'Oğuz', 'Ümit', 'Şule', 'Kılıç', 'Gökçe', 'İpek'];
const RACES = ['human', 'elf', 'dwarf', 'halfling', 'gnome', 'orc', 'tiefling', 'dragonborn', 'halfelf'];
const SIDES = ['Aslanburç Krallığı', 'Kırıkdiş Kampı', 'Demirtoynak Karakolu', 'Gümüşkule savunucuları', 'Kuzey Ordusu', 'Goblinler', 'Ayşe', 'İzmir', 'Üsküdar', 'Korsanlar'];
const TITLES = ['Kervan soygunu', 'Düello', 'Gümüşkule kuşatması', 'Yol pususu', 'Kırıkdiş Kampı baskını'];
const UNAMES = ['Milis', 'Asker', 'Kadırga', 'Savaş Avatarı', 'İskelet', 'Ork', 'Işık Muhafızı'];

const replacer = (_k: string, v: unknown) => (typeof v === 'number' && !Number.isFinite(v) ? String(v) : v);
const out = (o: unknown) => process.stdout.write(JSON.stringify(o, replacer) + '\n');

// ------------------------------------------------------------ build combatants from a spec (same in C#)
function makeHero(h: any): Hero { return { ...h, stats: { ...h.stats }, bonus: h.bonus ? { ...h.bonus } : undefined } as Hero; }
function srcUnit(op: any): UnitStats {
  if (op.src === 'inline') return op.u;
  if (op.src.startsWith('UNITS:')) return (UNITS as any)[op.src.slice(6)];
  return (MONSTERS as any)[op.src.slice(9)];
}
function applyPost(c: Combatant, p: any) {
  if (!p) return;
  if (p.ac !== undefined) c.ac += p.ac;
  if (p.dmg2 !== undefined) c.dmg[2] += p.dmg2;
  if (p.atk !== undefined) c.atk += p.atk;
  if (p.smite !== undefined) c.smite = p.smite;
  if (p.evil) c.evil = true;
  if (p.rage) c.rage = true;
  if (p.fled) c.fled = true;
  if (p.grp !== undefined) c.grp = p.grp;
}
function build(ops: any[]): Combatant[] {
  const res: Combatant[] = [];
  for (const op of ops) {
    let cs: Combatant[];
    if (op.op === 'unit') {
      const u = srcUnit(op);
      cs = [op.bonusHp === undefined ? unit(u, op.side, op.kind) : op.bonusAtk === undefined ? unit(u, op.side, op.kind, op.bonusHp) : unit(u, op.side, op.kind, op.bonusHp, op.bonusAtk)];
    } else if (op.op === 'monsterSide') cs = monsterSide(op.kind, op.n, op.boss, op.side);
    else cs = [heroCombatant(makeHero(op.hero), op.side)];
    for (const c of cs) applyPost(c, op.post);
    res.push(...cs);
  }
  return res;
}
const snap = (c: Combatant) => ({
  name: c.name, side: c.side, hp: c.hp, maxHp: c.maxHp, ac: c.ac, atk: c.atk, dmg: c.dmg, attacks: c.attacks, hero: c.hero?.id, boss: c.boss, kind: c.kind,
  evil: c.evil, smite: c.smite, fled: c.fled, uses: c.uses, kills: c.kills, rage: c.rage, temp: c.temp, grp: c.grp,
});

// ------------------------------------------------------------ random specs
let heroId = 0;
function genPost(): any {
  if (!chance(0.4)) return undefined;
  const p: any = {};
  if (chance(0.4)) p.ac = pick([1, 2, -2, 3, 0.5]);
  if (chance(0.3)) p.dmg2 = pick([1, 2, 0.5, -1]);
  if (chance(0.3)) p.atk = pick([1, 2, -1]);
  if (chance(0.3)) p.smite = pick([1.5, 2, 2.5, 3, 1.2, 4.6]);
  if (chance(0.4)) p.evil = true;
  if (chance(0.08)) p.rage = true;
  if (chance(0.04)) p.fled = true;
  return p;
}
function genHero(): any {
  const cls = pick(HCLS);
  const level = pick([1, 1, 2, 3, 3, 4, 5, 5, 6, 7, 8, 10]);
  const stats: Record<string, number> = {};
  for (const st of STATS) stats[st] = ri(6, 20);
  if (chance(0.01)) delete stats[pick(STATS)];            // missing stat → NaN path
  const maxHp = ri(6, 90);
  let hp = chance(0.45) ? maxHp : Math.max(1, Math.round(maxHp * G.next()));
  if (chance(0.03)) hp += 0.5;
  const ac = chance(0.6) ? heroAc(cls, mod(stats.dex ?? 10), level) : ri(10, 20);
  const h: any = { id: ++heroId, name: pick(NAMES), race: pick(RACES), cls, level, stats, maxHp, hp, ac };
  if (chance(0.9)) h.bonus = { atk: pick([0, 0, 1, 2, 3]) };
  return h;
}
function genOp(side: string): any {
  const r = G.next();
  let op: any;
  if (r < 0.3) op = { op: 'hero', hero: genHero(), side: chance(0.02) ? other(side) : side };
  else if (r < 0.6) op = { op: 'monsterSide', kind: pick(['goblin', 'hobgoblin', 'bugbear', 'pirate', 'goblin']), n: pick([0, 1, 2, 3, 4, 5, 6, 8, 10, 12, 2.5]), boss: chance(0.4), side };
  else {
    const q = G.next();
    op = { op: 'unit', src: q < 0.35 ? 'UNITS:' + pick(UNIT_IDS) : q < 0.6 ? 'MONSTERS:' + pick(MON_IDS) : 'inline', side: chance(0.02) ? other(side) : side, kind: pick(KINDS) };
    if (op.src === 'inline') {
      op.u = { name: pick(UNAMES), hp: ri(1, 40), ac: ri(8, 20), atk: ri(0, 8), dmg: [ri(1, 3), pick([4, 6, 8, 10, 12]), ri(0, 4)] };
      if (chance(0.4)) op.u.attacks = pick([1, 2, 3, 1.5]);
    }
    if (chance(0.5)) { op.bonusHp = pick([0, 1, 2, 3, -3, 5, 0.5, -20]); if (chance(0.7)) op.bonusAtk = pick([0, 1, 2, -1, 1.5, 3]); }
  }
  const post = genPost();
  if (post) op.post = post;
  return op;
}
function genSide(side: string): any[] {
  const n = pick([0, 1, 1, 2, 2, 3, 3, 4, 5, 6, 8, 12]);
  const ops: any[] = [];
  for (let i = 0; i < n; i++) ops.push(genOp(side));
  return ops;
}
function genOpts(A: any[], B: any[]): any {
  const o: any = { day: ri(0, 5000), tile: ri(0, 8000), title: pick(TITLES), sideA: pick(SIDES), sideB: pick(SIDES), id: ri(1, 100000) };
  const MOR = [0.2, 0.25, 0.35, 0.4, 0.45, 0.5, 0.55, 0.6, 0.65, 0.7, 0.9, 1];
  if (chance(0.5)) o.maxRounds = pick([0, 1, 2, 3, 5, 10, 12, 15, 20, 25]);
  if (chance(0.6)) o.moraleA = pick(MOR);
  if (chance(0.6)) o.moraleB = pick(MOR);
  if (chance(0.15)) o.noRoutA = chance(0.8);
  if (chance(0.15)) o.noRoutB = chance(0.8);
  if (chance(0.2)) o.firstStrikeA = pick([1, 1.25, 1.5, 2, 0.5, 0]);
  if (chance(0.2)) o.firstStrikeB = pick([1, 1.25, 1.5, 2, 0.5, 0]);
  if (chance(0.1)) o.meteorA = chance(0.85);
  if (chance(0.1)) o.meteorB = chance(0.85);
  if (chance(0.4)) o.timeoutWinner = pick(['A', 'B']);
  if (chance(0.3)) o.civA = ri(0, 7);
  if (chance(0.3)) o.civB = ri(0, 7);
  if (chance(0.2)) {
    const nA = ri(1, 3);
    o.groups = [];
    for (let i = 0; i < nA; i++) { const g: any = { name: pick(SIDES), side: 'A' }; if (chance(0.7)) g.civ = ri(0, 7); if (chance(0.3)) g.kind = pick(['civ', 'heroes', 'party']); o.groups.push(g); }
    o.groups.push({ name: pick(SIDES), side: 'B', civ: chance(0.5) ? ri(0, 7) : undefined });
    for (const op of A) if (chance(0.9)) op.post = { ...(op.post ?? {}), grp: ri(0, nA - 1) };
    for (const op of B) if (chance(0.9)) op.post = { ...(op.post ?? {}), grp: nA };
  }
  return o;
}

for (let k = 0; k < N; k++) {
  const A = genSide('A'), B = genSide('B');
  const opts = genOpts(A, B);
  const spec = { seed: ri(1, 2 ** 31) * (chance(0.1) ? 3 : 1), A, B, opts, vsAc: pick([15, 12, 18.5, 10, 20, 0]) };
  const specJson = JSON.parse(JSON.stringify(spec, replacer));   // freeze what C# will read
  const cA = build(spec.A), cB = build(spec.B);
  const vs = powerVs(cA, cB);
  const pre = { powA: powerOf(cA), powBvs: powerOf(cB, spec.vsAc), vs, avgA: avgAc(cA), avgB: avgAc(cB), wc: winChance(vs[0], vs[1]) };
  const rng = new Rng(spec.seed);
  const b = resolveBattle(rng, cA, cB, spec.opts);
  out({ k, spec: specJson, out: { pre, battle: b, rng: rng.state(), cs: [...cA, ...cB].map(snap) } });
}

// ------------------------------------------------------------ grids
const range = (a: number, b: number) => { const r: number[] = []; for (let i = a; i <= b; i++) r.push(i); return r; };
{
  const levels = range(-1, 25);
  out({ k: 'profBonus', spec: { levels }, out: levels.map((l) => profBonus(l)) });
}
{
  const inp: [string, number, number][] = [];
  for (const cls of [...HCLS, 'monk', 'x']) for (const dex of [-5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 0.5, -0.5, 2.5]) for (const lvl of [0, 1, 2, 3, 4, 5, 10, 20]) inp.push([cls, dex, lvl]);
  out({ k: 'heroAc', spec: { in: inp }, out: inp.map(([c, d, l]) => heroAc(c, d, l)) });
}
{
  const inp: [string, number, boolean, string][] = [];
  for (const kind of ['goblin', 'hobgoblin', 'bugbear', 'pirate', 'other']) for (const n of [0, 1, 2, 3, 5, 8, 13, 2.5, 0.3]) for (const boss of [true, false]) for (const side of ['A', 'B']) inp.push([kind, n, boss, side]);
  out({ k: 'monsterSide', spec: { in: inp }, out: inp.map(([k, n, b, s]) => monsterSide(k as never, n, b, s as never).map(snap)) });
}
{
  const inp: any[] = [];
  for (const src of [...UNIT_IDS.map((x) => 'UNITS:' + x), ...MON_IDS.map((x) => 'MONSTERS:' + x)])
    for (const [bh, ba] of [[undefined, undefined], [2, 1], [-50, 0], [0.5, 1.5], [3, undefined]] as [number | undefined, number | undefined][])
      inp.push({ op: 'unit', src, side: 'A', kind: pick(KINDS), bonusHp: bh, bonusAtk: ba });
  out({ k: 'unit', spec: { in: JSON.parse(JSON.stringify(inp)) }, out: inp.map((op) => snap(build([op])[0])) });
}
{
  const inp: any[] = [];
  for (const cls of HCLS) for (const level of range(1, 12)) { const h = genHero(); h.cls = cls; h.level = level; inp.push(h); }
  out({ k: 'heroCombatant', spec: { in: inp }, out: inp.map((h) => snap(heroCombatant(makeHero(h), 'B'))) });
}
{
  const vals = [0, 0.005, 0.01, 0.02, 1, 2.5, 10, 100, 1e6, -5, 1e-9, 37.3, 1234.5678];
  const inp: [number, number][] = [];
  for (const a of vals) for (const b of vals) inp.push([a, b]);
  out({ k: 'winChance', spec: { in: inp }, out: inp.map(([a, b]) => winChance(a, b)) });
}
