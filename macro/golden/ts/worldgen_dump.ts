// WorldGen port check (TS side): JSON.stringify(generateWorld(seed)) per seed, one document per line.
// Seeds: "<from> <to>" (inclusive range) or one comma-separated list ("360,1192,4294967296,-7,0.5").
// Run from macro/: node golden/ts/build.mjs worldgen_dump && node golden/out/worldgen_dump.js 1 30 > ts.ndjson
// C# side: dotnet run -c Release --project tests/WorldGenCheck -- 1 30 > cs.ndjson
// Compare:  python3 tests/WorldGenCheck/worldgen_compare.py ts.ndjson cs.ndjson
import { generateWorld } from '../../../src/sim/worldgen';

const a = process.argv[2] ?? '1';
const seeds: number[] = [];
if (a.includes(',')) for (const x of a.split(',')) seeds.push(Number(x));
else for (let s = Number(a), to = Number(process.argv[3] ?? a); s <= to; s++) seeds.push(s);
for (const seed of seeds) process.stdout.write(JSON.stringify(generateWorld(seed)) + '\n');
