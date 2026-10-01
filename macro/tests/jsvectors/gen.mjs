// Regenerates all JS-semantics reference vectors from the running Node (must be Node 22 / V8 12.4).
//   node tests/jsvectors/gen.mjs [outDir]      (default outDir: tests/jsvectors/data)
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { genMath } from './gen-math.mjs';
import { genFormat } from './gen-format.mjs';
import { genSort } from './gen-sort.mjs';

const here = path.dirname(fileURLToPath(import.meta.url));
const outDir = path.resolve(process.argv[2] ?? path.join(here, 'data'));
const major = Number(process.versions.node.split('.')[0]);
if (major !== 22) console.warn(`warning: expected Node 22 (V8 12.4), running Node ${process.versions.node} (V8 ${process.versions.v8})`);
console.log(`Node ${process.versions.node}, V8 ${process.versions.v8} -> ${outDir}`);
genMath(outDir);
genFormat(outDir);
genSort(outDir);
