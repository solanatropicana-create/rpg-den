// Bundles a golden-harness entry (golden/ts/<name>.ts) into golden/out/<name>.js with esbuild.
import { build } from '../../../node_modules/esbuild/lib/main.js';
import { fileURLToPath } from 'node:url';
import path from 'node:path';
const here = path.dirname(fileURLToPath(import.meta.url));
const name = process.argv[2];
await build({
  entryPoints: [path.join(here, `${name}.ts`)],
  bundle: true, platform: 'node', format: 'cjs', target: 'node22',
  outfile: path.join(here, '..', 'out', `${name}.js`), logLevel: 'warning',
});
