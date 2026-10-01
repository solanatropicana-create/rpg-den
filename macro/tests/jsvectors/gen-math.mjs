// Generates V8 reference vectors for Math.exp / Math.log / Math.pow / Math.round /
// ToInt32 / ToUint32 / Math.min / Math.max as raw little-endian float64/int32 records.
import path from 'node:path';
import {
  makeRng, fromBits, hiWord, stepUlps, randBits, rand01, uniform, randInt, logUniform, pick,
  SPECIALS, BinWriter,
} from './lib.mjs';

// Adds x plus a few bit-neighbours of x.
function withNeighbours(list, x, span = 3) {
  for (let k = -span; k <= span; k++) list.push(stepUlps(x, k));
}

function genExpInputs(rng) {
  const xs = [...SPECIALS];
  // Boundaries of fdlibm e_exp.c's high-word tests and thresholds.
  for (const hw of [0x40862E42, 0x3FD62E42, 0x3FF0A2B2, 0x3E300000]) {
    for (const s of [0, 0x80000000]) {
      for (let k = -3; k <= 3; k++) {
        xs.push(fromBits((hw + k) | s, 0), fromBits((hw + k) | s, 0xFFFFFFFF), fromBits((hw + k) | s, rng()));
      }
    }
  }
  for (const t of [709.782712893384, -745.1332191019412, 709.7827128933840, -708.3964185322641, -708.05,
    -707.7, 0.34657359027997264, 1.0397207708399179, 3.725290298461914e-9, 1, -1]) withNeighbours(xs, t, 50);
  for (let i = -750; i <= 715; i++) { xs.push(i, i + 0.5, i + 0.25, i * Math.LN2, -i * Math.LN2); }
  for (let i = 0; i < 200000; i++) xs.push(randBits(rng));
  for (let i = 0; i < 300000; i++) xs.push(uniform(rng, -50, 5));           // game range
  for (let i = 0; i < 200000; i++) xs.push(uniform(rng, -750, 750));
  for (let i = 0; i < 150000; i++) xs.push(logUniform(rng, -60, 10, true));
  for (let i = 0; i < 100000; i++) xs.push(uniform(rng, -1, 1));
  for (let i = 0; i < 50000; i++) xs.push(randInt(rng, -1100, 1030) * Math.LN2 + uniform(rng, -1e-9, 1e-9));
  return xs;
}

function genLogInputs(rng) {
  const xs = [...SPECIALS];
  for (const t of [1, 2, 0.5, Math.E, 10, 1e-4, 1e4, Math.SQRT2, Math.SQRT1_2, 2.2250738585072014e-308]) withNeighbours(xs, t, 60);
  for (let i = 1; i <= 50000; i++) { xs.push(i, i / 1000); }
  for (let e = -1074; e <= 1023; e++) xs.push(2 ** e);
  for (let i = 0; i < 200000; i++) xs.push(randBits(rng));
  for (let i = 0; i < 150000; i++) xs.push(Math.abs(randBits(rng)));        // all exponents
  for (let i = 0; i < 300000; i++) xs.push(uniform(rng, 1e-4, 1e4));        // game range
  for (let i = 0; i < 100000; i++) xs.push(Math.pow(10, uniform(rng, -4, 4)));
  for (let i = 0; i < 100000; i++) xs.push(1 + uniform(rng, -1, 1) * 2 ** -18); // near 1 (|f| < 2^-20 path)
  for (let i = 0; i < 20000; i++) xs.push(stepUlps(1, randInt(rng, -5000, 5000)));
  for (let i = 0; i < 50000; i++) xs.push(fromBits(rng() & 0x000FFFFF, rng())); // subnormals
  for (let i = 0; i < 30000; i++) xs.push(uniform(rng, 0, 3));
  return xs;
}

function genPowInputs(rng) {
  const pairs = [];
  const specialY = [...SPECIALS, 0.3, 2.4, -0.5, -2, 5, -5, 7, -7, 1 / 3, 2 ** 31 + 1, -(2 ** 31) - 1, 2 ** 52 + 1,
    2 ** 53 + 1, 2 ** 64, -(2 ** 64), 2 ** 65, 1e10, -1e10, 1023, 1024, 1075, -1074, -1075, -1076, 0.999999, 1.000001];
  for (const x of SPECIALS) for (const y of specialY) pairs.push([x, y]);
  for (const y of specialY) for (let i = 0; i < 40; i++) pairs.push([randBits(rng), y]);
  // Game ranges: base in [0.01, 1000], exponent in {0.3, 2.4, 2, 0.5} and uniform in [-5, 5].
  for (const y of [0.3, 2.4, 2, 0.5]) {
    for (let i = 0; i < 60000; i++) pairs.push([uniform(rng, 0.01, 1000), y]);
    for (let i = 0; i < 40000; i++) pairs.push([Math.pow(10, uniform(rng, -2, 3)), y]);
  }
  for (let i = 0; i < 150000; i++) pairs.push([uniform(rng, 0.01, 1000), uniform(rng, -5, 5)]);
  for (let i = 0; i < 50000; i++) pairs.push([Math.pow(10, uniform(rng, -2, 3)), uniform(rng, -5, 5)]);
  // Random bit patterns (all special paths, NaN, huge/tiny).
  for (let i = 0; i < 150000; i++) pairs.push([randBits(rng), randBits(rng)]);
  // Wide-range bases (both signs, incl. subnormal) with integer and real exponents.
  for (let i = 0; i < 50000; i++) pairs.push([logUniform(rng, -1074, 1023, true), randInt(rng, -40, 40)]);
  for (let i = 0; i < 50000; i++) pairs.push([logUniform(rng, -1074, 1023, true), uniform(rng, -100, 100)]);
  for (let i = 0; i < 30000; i++) pairs.push([logUniform(rng, -1074, 1023, false), uniform(rng, -2, 2)]);
  // Negative bases with integer exponents (odd/even, incl. yisint detection for large y).
  for (let i = 0; i < 40000; i++) {
    const x = -uniform(rng, 0, 10);
    const r = rng() % 4;
    const y = r === 0 ? randInt(rng, -60, 60) : r === 1 ? randInt(rng, -1e9, 1e9)
      : r === 2 ? (2 ** randInt(rng, 21, 60)) + randInt(rng, -3, 3) : randInt(rng, -1e6, 1e6) + 0.5;
    pairs.push([x, y]);
  }
  // |y| > 2^31 with x close to 1 (the log(x) ~ x - x^2/2 + ... branch) and around it.
  for (let i = 0; i < 50000; i++) {
    const x = 1 + uniform(rng, -1, 1) * 2 ** -randInt(rng, 18, 52);
    const y = (rng() & 1 ? 1 : -1) * 2 ** uniform(rng, 31, 70);
    pairs.push([rng() & 7 ? x : -x, y]);
  }
  for (let i = 0; i < 20000; i++) pairs.push([stepUlps(1, randInt(rng, -2000, 2000)), (rng() & 1 ? 1 : -1) * 2 ** uniform(rng, 20, 64)]);
  // Results near overflow / underflow / subnormal range (z ~ 1024, z ~ -1075, scalbn path).
  for (let i = 0; i < 60000; i++) {
    const y = (rng() & 1 ? 1 : -1) * uniform(rng, 0.5, 60);
    const T = pick(rng, [[1015, 1030], [-1080, -1070], [-1076, -1020], [-1030, -1015]]);
    pairs.push([Math.pow(2, uniform(rng, T[0], T[1]) / y), y]);
  }
  for (let i = 0; i < 20000; i++) {
    const y = randInt(rng, 2, 80);
    const t = rng() & 1 ? 1024 : -1075;
    pairs.push([stepUlps(Math.pow(2, t / y), randInt(rng, -20, 20)), y]);
  }
  // Subnormal bases.
  for (let i = 0; i < 20000; i++) pairs.push([fromBits(rng() & 0x000FFFFF, rng()), uniform(rng, -1.2, 1.2)]);
  // Near-integer exponents, exponents with few bits.
  for (let i = 0; i < 30000; i++) pairs.push([uniform(rng, 0, 20), randInt(rng, -30, 30) / 8]);
  return pairs;
}

function genRoundInputs(rng) {
  const xs = [...SPECIALS, 0.5, -0.5, 1.5, -1.5, 2.5, -2.5, -0.5000000000000001, 0.5000000000000001,
    4503599627370495.5, -4503599627370495.5, 4503599627370496, 4503599627370497, 9007199254740991, -9007199254740991,
    2251799813685247.5, -2251799813685247.5, 2251799813685248.5, -2251799813685248.5];
  for (const t of [0.5, -0.5, 0.49999999999999994, 1.5, -1.5, 2 ** 52, -(2 ** 52), 2 ** 51, -(2 ** 51), 2 ** 53]) withNeighbours(xs, t, 40);
  for (let i = 0; i < 200000; i++) xs.push(randBits(rng));
  for (let i = 0; i < 300000; i++) xs.push(uniform(rng, -10, 10));
  for (let i = 0; i < 100000; i++) xs.push(randInt(rng, -1e6, 1e6) / 2);
  for (let i = 0; i < 50000; i++) xs.push(randInt(rng, -1e6, 1e6) / 4);
  for (let i = 0; i < 100000; i++) xs.push((rng() & 1 ? 1 : -1) * uniform(rng, 2 ** 50, 2 ** 53));
  for (let i = 0; i < 50000; i++) xs.push((rng() & 1 ? 1 : -1) * (randInt(rng, 2 ** 20, 2 ** 30) * 2 ** 21 + 0.5));
  for (let i = 0; i < 100000; i++) xs.push(uniform(rng, -1, 1));
  for (let i = 0; i < 50000; i++) xs.push(logUniform(rng, -1074, 1023, true));
  for (let i = 0; i < 50000; i++) xs.push(uniform(rng, -1e6, 1e6));
  return xs;
}

function genToIntInputs(rng) {
  const xs = [...SPECIALS];
  for (const t of [2 ** 31, -(2 ** 31), 2 ** 32, -(2 ** 32), 2 ** 31 - 0.5, -(2 ** 31) - 0.5, 2 ** 53, 2 ** 63, 2 ** 64,
    2 ** 84, 2 ** 85, 2 ** 1023, -(2 ** 1023), 1, -1, 0.5, -0.5]) withNeighbours(xs, t, 30);
  for (let k = -70; k <= 70; k++) for (const d of [-1.5, -1, -0.5, 0, 0.5, 1, 1.5]) xs.push(k * 2 ** 31 + d, k * 2 ** 32 + d);
  for (let i = 0; i < 300000; i++) xs.push(randBits(rng));
  for (let i = 0; i < 250000; i++) xs.push(uniform(rng, -(2 ** 34), 2 ** 34));
  for (let i = 0; i < 150000; i++) xs.push(randInt(rng, -1000, 1000) * 4294967296 + randInt(rng, -3000000000, 3000000000) + pick(rng, [0, 0.25, 0.5, 0.75, -0.5]));
  for (let i = 0; i < 150000; i++) xs.push(logUniform(rng, 0, 1024, true));
  for (let i = 0; i < 100000; i++) xs.push(uniform(rng, -1e3, 1e3));
  for (let i = 0; i < 50000; i++) xs.push((rng() & 1 ? 1 : -1) * Math.floor(uniform(rng, 2 ** 53, 2 ** 90)));
  return xs;
}

export function genMath(outDir, seed = 20260930) {
  const rng = makeRng(seed);
  const t0 = Date.now();
  const counts = {};

  { // exp
    const w = new BinWriter(1 << 24);
    const xs = genExpInputs(rng);
    for (const x of xs) { w.f64(x); w.f64(Math.exp(x)); }
    w.save(path.join(outDir, 'exp.bin')); counts.exp = xs.length;
  }
  { // log
    const w = new BinWriter(1 << 24);
    const xs = genLogInputs(rng);
    for (const x of xs) { w.f64(x); w.f64(Math.log(x)); }
    w.save(path.join(outDir, 'log.bin')); counts.log = xs.length;
  }
  { // pow (also cross-checks that ** and Math.pow agree in Node)
    const w = new BinWriter(1 << 25);
    const pairs = genPowInputs(rng);
    let opMismatch = 0;
    for (const [x, y] of pairs) {
      const r = Math.pow(x, y);
      const r2 = x ** y;
      if (!(Object.is(r, r2))) opMismatch++;
      w.f64(x); w.f64(y); w.f64(r);
    }
    if (opMismatch) console.log(`  note: Math.pow vs ** differed on ${opMismatch} inputs`);
    w.save(path.join(outDir, 'pow.bin')); counts.pow = pairs.length;
  }
  { // round
    const w = new BinWriter(1 << 24);
    const xs = genRoundInputs(rng);
    for (const x of xs) { w.f64(x); w.f64(Math.round(x)); }
    w.save(path.join(outDir, 'round.bin')); counts.round = xs.length;
  }
  { // ToInt32 / ToUint32
    const w = new BinWriter(1 << 24);
    const xs = genToIntInputs(rng);
    for (const x of xs) { w.f64(x); w.i32(x | 0); w.u32(x >>> 0); }
    w.save(path.join(outDir, 'toint.bin')); counts.toint = xs.length;
  }
  { // Math.min / Math.max, two arguments
    const w = new BinWriter(1 << 23);
    const pool = [...SPECIALS];
    for (let i = 0; i < 200; i++) pool.push(randBits(rng), uniform(rng, -10, 10), randInt(rng, -3, 3));
    let n = 0;
    for (let i = 0; i < 300000; i++) {
      const a = (rng() & 3) ? pick(rng, pool) : uniform(rng, -1e3, 1e3);
      const b = (rng() & 7) === 0 ? a : (rng() & 3) ? pick(rng, pool) : uniform(rng, -1e3, 1e3);
      w.f64(a); w.f64(b); w.f64(Math.min(a, b)); w.f64(Math.max(a, b)); n++;
    }
    for (const a of [0, -0]) for (const b of [0, -0]) { w.f64(a); w.f64(b); w.f64(Math.min(a, b)); w.f64(Math.max(a, b)); n++; }
    w.save(path.join(outDir, 'minmax2.bin')); counts.minmax2 = n;
  }
  { // Math.min / Math.max, 0..6 arguments: record = i32 len, i32 pad, 6 x f64, min, max
    const w = new BinWriter(1 << 23);
    const pool = [0, -0, NaN, Infinity, -Infinity, 1, -1, 2, 0.5, -0.5, 1e300, -1e300, Number.MIN_VALUE, -Number.MIN_VALUE];
    let n = 0;
    for (let i = 0; i < 60000; i++) {
      const len = i < 10 ? 0 : rng() % 7;
      const vals = [];
      for (let j = 0; j < len; j++) vals.push((rng() & 1) ? pick(rng, pool) : uniform(rng, -5, 5));
      w.i32(len); w.i32(0);
      for (let j = 0; j < 6; j++) w.f64(j < len ? vals[j] : 0);
      w.f64(Math.min(...vals)); w.f64(Math.max(...vals)); n++;
    }
    w.save(path.join(outDir, 'minmaxN.bin')); counts.minmaxN = n;
  }
  console.log(`math vectors: ${JSON.stringify(counts)} (${Date.now() - t0} ms)`);
}
