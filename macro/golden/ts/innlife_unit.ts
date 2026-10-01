// InnLife port check, pure helpers (TS side): random inns (with edge cases: need 0, NaN-producing builds, fame 70,
// stable undefined/false/true, empty staff) and the results of innTitle, levelName, innPrices, innOcc, serveCap,
// wages, buildStageName, buildPct. One JSON line per inn: { inn, out }. C# twin: InnLifeCheck `unit <file>` reads
// the same file and prints { out } per line. Run from macro/:
//   node golden/ts/build.mjs innlife_unit && node golden/out/innlife_unit.js 3000 > unit_in.ndjson
import { innTitle, levelName, innPrices, innOcc, serveCap, wages, buildStageName, buildPct } from '../../../src/sim/innlife';
import { Rng } from '../../../src/sim/rng';
import type { Guest, Inn, InnStaff, StaffRole } from '../../../src/sim/types';

const n = Number(process.argv[2] ?? '2000');
const r = new Rng(12345);
const pick = <T>(a: T[]): T => a[Math.floor(r.next() * a.length)];
const num = (): number => pick([0, 0, 1, 2.5, 3, 7, 10, 29.9, 30, 48, 0.1 + 0.2, r.next() * 60, r.next() * 5, -1]);
const ROLES: StaffRole[] = ['cirak', 'asci', 'seyis', 'garson', 'bekci'];

for (let k = 0; k < n; k++) {
  const guests: Guest[] = [];
  const ng = Math.floor(r.next() * 6);
  for (let i = 0; i < ng; i++) {
    const g = { id: i, kind: 'merchant', name: 'G' + i, race: 'human', n: pick([1, 2, 3, 5, 0.5]), civ: 0, from: 0, fromName: 'a', to: 1, toName: 'b', why: '', purse: 1, spent: 0, nights: 1, arrived: 0, mood: 0.7 } as Guest;
    const st = pick([0, 1, 2]);
    if (st === 1) g.stable = true; else if (st === 2) g.stable = false;
    guests.push(g);
  }
  const staff: InnStaff[] = [];
  const ns = Math.floor(r.next() * 7);
  for (let i = 0; i < ns; i++) staff.push({ name: 'S' + i, race: 'dwarf', role: pick(ROLES), since: 0, from: 'x' });
  const inn = {
    id: k, tile: 0, name: pick(['Kırık Kupa', 'Üç Yol', 'Yaşlı Ejder', 'Son Durak 19']), keeper: 'K', founded: 0, alive: true, gold: 10, raids: 0,
    stage: pick(['road', 'build', 'open', 'ruin']), level: pick([0, 1, 2, 3]), keeperRace: 'human', origin: 0, originName: 'o',
    stock: { food: 1, ale: 1, wood: 1 }, fame: pick([0, 12, 69.99, 70, 70.5, 100, r.next() * 100]), staff, guests, tabs: {}, log: [], books: [], hist: [],
    total: { guests: 0, nights: 0, income: 0, turned: 0, bought: 0 }, turned: 0, sat: 0.7, traffic: 0,
  } as Inn;
  if (r.next() < 0.8) {
    inn.build = { level: pick([1, 2, 3]), work: num(), need: pick([0, 36, 48, 70, 120, num()]), wood: num(), woodNeed: pick([0, 22, 30, 34, num()]), stone: num(), stoneNeed: pick([0, 8, 16, num()]), started: 0 };
    const rb = pick([0, 1, 2]);
    if (rb === 1) inn.build.rebuild = true; else if (rb === 2) inn.build.rebuild = false;
  }
  const out = {
    title: innTitle(inn), levelName: levelName(inn), prices: innPrices(inn), occ: innOcc(inn),
    serveCap: serveCap(inn), wages: wages(inn), stage: buildStageName(inn), pct: buildPct(inn), pctS: String(buildPct(inn)),
  };
  process.stdout.write(JSON.stringify({ inn, out }) + '\n');
}
