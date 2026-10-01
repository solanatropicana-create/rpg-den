// Like build.mjs, but also exports the private helpers of heroes.ts / will.ts / inns.ts (appended at bundle time;
// src/ is not modified) so the Heroes/Will/Inns unit check can call them directly.
// Run from macro/: node golden/ts/heroes_build.mjs heroes_unit && node golden/out/heroes_unit.js <seed>
import { build } from '../../../node_modules/esbuild/lib/main.js';
import { fileURLToPath } from 'node:url';
import path from 'node:path';
import fs from 'node:fs';
const here = path.dirname(fileURLToPath(import.meta.url));
const name = process.argv[2];
const expose = {
  'heroes.ts': ['potionHeal'],
  'will.ts': ['FIT', 'distPen', 'partyFor', 'campRisk', 'campAllies', 'startGoal', 'progressGoal', 'robCaravan', 'duel'],
  'inns.ts': ['spawnInnHero', 'effBid', 'closeAuction', 'contractEnd', 'postGuardQuest', 'innDefenders', 'considerInnRaid'],
};
const plugin = {
  name: 'expose-privates',
  setup(b) {
    b.onLoad({ filter: /[\\/]src[\\/]sim[\\/](heroes|will|inns)\.ts$/ }, async (args) => {
      const src = await fs.promises.readFile(args.path, 'utf8');
      return { contents: `${src}\nexport { ${expose[path.basename(args.path)].join(', ')} };\n`, loader: 'ts', resolveDir: path.dirname(args.path) };
    });
  },
};
await build({
  entryPoints: [path.join(here, `${name}.ts`)],
  bundle: true, platform: 'node', format: 'cjs', target: 'node22',
  outfile: path.join(here, '..', 'out', `${name}.js`), logLevel: 'warning', plugins: [plugin],
});
