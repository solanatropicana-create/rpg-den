// Dumps every exported data table of the TS game (src/data/*.ts) to JSON for the C# port.
// Functions (subclass `pick`) are dropped (ported by hand in C#); Infinity/NaN become strings.
// Run: node golden/ts/build.mjs dumpdata && node golden/out/dumpdata.js > FD.Macro/Data/data.json
import * as goods from '../../../src/data/goods';
import * as classes from '../../../src/data/classes';
import * as techs from '../../../src/data/techs';
import * as heroes from '../../../src/data/heroes';
import * as inn from '../../../src/data/inn';

const out: Record<string, unknown> = {};
for (const mod of [goods, classes, techs, heroes, inn]) {
  for (const [k, v] of Object.entries(mod)) {
    if (typeof v === 'function') continue;
    if (k === 'TECH' || k === 'ALL_TECHS') continue; // derived from MAIN_TECHS + CLASS_TECHS in C#
    out[k] = v;
  }
}
const json = JSON.stringify(out, (_k, v) => {
  if (typeof v === 'function') return undefined;
  if (typeof v === 'number' && !Number.isFinite(v)) return String(v);
  return v;
}, 1);
process.stdout.write(json + '\n');
