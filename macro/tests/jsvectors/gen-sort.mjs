// Generates Array.prototype.sort reference cases: final order + full comparator call log.
//
// sort.bin layout (little-endian):
//   "JSRT" u32 version=1 u32 caseCount
//   per case: u32 kind, u32 pattern, u32 seed, u32 n,
//             f64[n] k, f64[n] g            (NaN in k = key missing / undefined in JS)
//             u16[n] final order (element ids)
//             u32 callCount, u16[2*callCount] comparator calls (a.id, b.id)
import path from 'node:path';
import { makeRng, uniform, randInt, pick, BinWriter } from './lib.mjs';

// Comparator kinds. The C# checker (Program.cs, MakeComparer) mirrors each one exactly.
export const KINDS = [
  'num',          // 0: a.k - b.k
  'numDesc',      // 1: b.k - a.k
  'nanKey',       // 2: a.k - b.k with ~20% keys undefined -> NaN
  'nanSome',      // 3: (a.k % 5 === 0 || b.k % 5 === 0) ? NaN : a.k - b.k
  'random',       // 4: pseudo-random results (NaN, -0, 0, -1, 1, fractions) from a seeded PRNG
  'mostly',       // 5: a.k - b.k, but 1/16 of calls return a random fraction
  'gtOnly',       // 6: a.k > b.k ? 1 : -1   (never 0: inconsistent for equal keys)
  'boolish',      // 7: a.k > b.k ? 1 : 0    (like returning a boolean)
  'doubleKeys',   // 8: a.k - b.k on random doubles incl. +-0, tiny and huge values
  'twoKey',       // 9: (a.g - b.g) || (b.k - a.k)
];

function makeComparator(kind, seed) {
  const rng = makeRng(seed);
  switch (kind) {
    case 0: return (a, b) => a.k - b.k;
    case 1: return (a, b) => b.k - a.k;
    case 2: return (a, b) => a.k - b.k;
    case 3: return (a, b) => (a.k % 5 === 0 || b.k % 5 === 0) ? NaN : a.k - b.k;
    case 4: return () => {
      const r = rng();
      switch (r & 7) {
        case 0: return NaN;
        case 1: return -0;
        case 2: return 0;
        case 3: return -1;
        case 4: return 1;
        default: return (r >>> 3) / 536870912 - 0.5;
      }
    };
    case 5: return (a, b) => {
      const r = rng();
      return (r & 15) === 0 ? (r >>> 4) / 268435456 - 0.5 : a.k - b.k;
    };
    case 6: return (a, b) => a.k > b.k ? 1 : -1;
    case 7: return (a, b) => a.k > b.k ? 1 : 0;
    case 8: return (a, b) => a.k - b.k;
    case 9: return (a, b) => (a.g - b.g) || (b.k - a.k);
    default: throw new Error('kind ' + kind);
  }
}

const PATTERNS = 14;

function makeKeys(rng, pattern, n) {
  const k = new Array(n);
  switch (pattern) {
    case 0: for (let i = 0; i < n; i++) k[i] = randInt(rng, 0, Math.max(0, n - 1)); break;         // random
    case 1: for (let i = 0; i < n; i++) k[i] = randInt(rng, 0, 3); break;                            // many dups
    case 2: for (let i = 0; i < n; i++) k[i] = 7; break;                                              // all equal
    case 3: for (let i = 0; i < n; i++) k[i] = i; break;                                              // ascending
    case 4: for (let i = 0; i < n; i++) k[i] = n - i; break;                                          // descending
    case 5: for (let i = 0; i < n; i++) k[i] = i % 7; break;                                          // sawtooth
    case 6: for (let i = 0; i < n; i++) k[i] = i < n / 2 ? i : n - i; break;                          // organ pipe
    case 7: {                                                                                         // nearly sorted
      for (let i = 0; i < n; i++) k[i] = i;
      for (let s = 0; s < Math.max(1, n >> 5); s++) {
        const x = randInt(rng, 0, n - 1), y = randInt(rng, 0, n - 1); const t = k[x]; k[x] = k[y]; k[y] = t;
      }
      break;
    }
    case 8: {                                                                                         // sorted runs
      let i = 0;
      while (i < n) {
        const len = randInt(rng, 1, Math.max(1, Math.floor(n / 3)));
        let v = randInt(rng, 0, n);
        const asc = (rng() & 3) !== 0;
        for (let j = 0; j < len && i < n; j++, i++) { k[i] = v; v += asc ? randInt(rng, 0, 3) : -randInt(rng, 0, 3); }
      }
      break;
    }
    case 9: for (let i = 0; i < n; i++) k[i] = Math.floor((n - i) / 3); break;                        // descending w/ dups
    case 10: {                                                                                        // two interleaving sorted halves
      const h = n >> 1;
      for (let i = 0; i < n; i++) k[i] = i < h ? 2 * i : 2 * (i - h) + 1;
      break;
    }
    case 11: for (let i = 0; i < n; i++) k[i] = rng() & 1; break;                                     // 0/1
    case 12:                                                                                          // MergeHigh gallop exits
    case 13: {                                                                                        // (13: mirrored)
      // Run A = [x0, high ascending block], run B = [a few keys < x0, many keys in (x0, high)]:
      // MergeHigh gallops and A runs out right after a GallopLeft (rare exit path).
      const la = Math.max(1, Math.floor(n * uniform(rng, 0.55, 0.8)));
      const lb = n - la;
      const few = randInt(rng, 0, 4);
      const bvals = [];
      for (let i = 0; i < lb; i++) bvals.push(i < few ? randInt(rng, 0, 999) : randInt(rng, 1001, 4999));
      bvals.sort((p, q) => p - q);
      const t = new Array(n);
      t[0] = 1000;
      for (let i = 1; i < la; i++) t[i] = 5000 + i;
      for (let i = 0; i < lb; i++) t[la + i] = bvals[i];
      for (let i = 0; i < n; i++) k[i] = pattern === 12 ? t[i] : -t[n - 1 - i];
      break;
    }
    default: throw new Error('pattern');
  }
  return k;
}

const DOUBLE_POOL = [0, -0, 1e-300, -1e-300, 5e-324, 1, -1, 0.1, 0.2, 0.30000000000000004, 1e300, -1e300, 2 ** 53, 123.456];

export function genSort(outDir, seed = 4242) {
  const rng = makeRng(seed);
  const t0 = Date.now();
  const sizes = [0, 1, 2, 3, 4, 5, 6, 7, 8, 10, 15, 16, 17, 31, 32, 33, 50, 63, 64, 65, 100, 127, 128, 129,
    200, 255, 256, 257, 400, 500, 777, 1000, 1024, 1500, 2000];
  const w = new BinWriter(1 << 25);
  w.ensure(12); w.buf.write('JSRT', 0, 'ascii'); w.pos = 4; w.u32(1);
  const countPos = w.pos; w.u32(0);
  let cases = 0, totalCalls = 0;
  for (const n of sizes) {
    const patterns = n >= 400 ? [0, 1, 3, 4, 8, 10, 12, 13] : [...Array(PATTERNS).keys()];
    for (const pattern of patterns) {
      for (let kind = 0; kind < KINDS.length; kind++) {
        const keys = makeKeys(rng, pattern, n);
        const gs = new Array(n);
        for (let i = 0; i < n; i++) gs[i] = randInt(rng, 0, 3);
        if (kind === 2) for (let i = 0; i < n; i++) if (rng() % 5 === 0) keys[i] = undefined;
        if (kind === 8) for (let i = 0; i < n; i++) keys[i] = (rng() & 1) ? pick(rng, DOUBLE_POOL) : uniform(rng, -3, 3);
        const arr = [];
        for (let i = 0; i < n; i++) arr.push({ id: i, k: keys[i], g: gs[i] });
        const cmpSeed = (cases * 2654435761 + seed * 40503 + 12345) >>> 0;
        const inner = makeComparator(kind, cmpSeed);
        const log = [];
        arr.sort((a, b) => { log.push(a.id, b.id); return inner(a, b); });

        w.u32(kind); w.u32(pattern); w.u32(cmpSeed); w.u32(n);
        for (let i = 0; i < n; i++) w.f64(keys[i] === undefined ? NaN : keys[i]);
        for (let i = 0; i < n; i++) w.f64(gs[i]);
        w.ensure(2 * n);
        for (let i = 0; i < n; i++) { w.buf.writeUInt16LE(arr[i].id, w.pos); w.pos += 2; }
        w.u32(log.length / 2);
        w.ensure(2 * log.length);
        for (let i = 0; i < log.length; i++) { w.buf.writeUInt16LE(log[i], w.pos); w.pos += 2; }
        cases++; totalCalls += log.length / 2;
      }
    }
  }
  w.buf.writeUInt32LE(cases, countPos);
  w.save(path.join(outDir, 'sort.bin'));
  console.log(`sort vectors: ${cases} cases, ${totalCalls} comparator calls (${Date.now() - t0} ms)`);
}
