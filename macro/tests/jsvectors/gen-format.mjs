// Generates V8 reference strings for String(x) and x.toFixed(0..6).
// Output: format.txt, one line per value:  <hex bits>|String(x)|toFixed(0)|...|toFixed(6)
import fs from 'node:fs';
import path from 'node:path';
import {
  makeRng, fromBits, toBitsHex, stepUlps, randBits, rand01, uniform, randInt, logUniform, pick, SPECIALS,
} from './lib.mjs';

function genValues(rng) {
  const xs = [...SPECIALS];
  const push = (x) => { xs.push(x); };
  // Powers of two (asymmetric rounding interval), powers of ten, and their bit neighbours.
  for (let e = -1074; e <= 1023; e++) {
    const p = 2 ** e;
    push(p); push(-p); push(stepUlps(p, 1)); push(stepUlps(p, -1)); push(stepUlps(p, 2));
  }
  for (let e = -323; e <= 308; e++) {
    const p = Number('1e' + e);
    for (let k = -2; k <= 2; k++) push(stepUlps(p, k));
    push(-p);
    push(Number('5e' + e)); push(Number('2.5e' + e)); push(Number('1.5e' + e)); push(Number('9.999999999999999e' + e));
  }
  // Around the notation switches: 1e21, 1e-6, 1e-7.
  for (const t of [1e21, 1e-6, 1e-7, 999999999999999900000, 0.000001, 0.0000009999999999999999]) {
    for (let k = -200; k <= 200; k++) { push(stepUlps(t, k)); push(-stepUlps(t, k)); }
  }
  // Random bit patterns (every exponent, subnormals, NaN/Inf now and then).
  for (let i = 0; i < 70000; i++) push(randBits(rng));
  // "Nice" values the game produces.
  for (let i = 0; i < 20000; i++) push(randInt(rng, -1e6, 1e6) / 100);
  for (let i = 0; i < 10000; i++) push(randInt(rng, -1e6, 1e6) / 1000);
  for (let i = 0; i < 10000; i++) push(randInt(rng, -1e5, 1e5) / 8);
  for (let i = 0; i < 10000; i++) push(randInt(rng, -1e5, 1e5) / 16);
  for (let i = 0; i < 10000; i++) push(randInt(rng, -1e9, 1e9));
  for (let i = 0; i < 10000; i++) push(randInt(rng, -1e6, 1e6) / 10 ** randInt(rng, 0, 9));
  for (let i = 0; i < 10000; i++) push(randInt(rng, 1, 999) * Number('1e' + randInt(rng, -30, 30)));
  for (let i = 0; i < 10000; i++) push(Math.floor(uniform(rng, 0, 2 ** 53)));
  // Large integers >= 2^53: rounding-interval bounds land exactly on shorter decimals (even/odd significand).
  for (let i = 0; i < 20000; i++) push(Math.floor(uniform(rng, 2 ** 53, 2 ** 70)));
  for (let i = 0; i < 10000; i++) push(Math.floor(uniform(rng, 2 ** 53, 2 ** 57)));
  // Exact ties in the shortest-digit search (e.g. 2^49 + 0.25 -> "562949953421312.2"): N + k/16.
  for (let i = 0; i < 20000; i++) push(Math.floor(uniform(rng, 2 ** 40, 2 ** 53)) + randInt(rng, 0, 15) / 16);
  // Values with few significant bits: m * 2^e (exact decimal expansions ending in 5s).
  for (let i = 0; i < 20000; i++) push((rng() & 1 ? -1 : 1) * randInt(rng, 1, 2 ** 20) * 2 ** randInt(rng, -90, 90));
  // Uniform / log-uniform magnitudes.
  for (let i = 0; i < 20000; i++) push(rand01(rng));
  for (let i = 0; i < 20000; i++) push(uniform(rng, -1000, 1000));
  for (let i = 0; i < 20000; i++) push(logUniform(rng, -40, 80, true));
  // toFixed-specific: exact ties k/2^j, (k + 0.5)/10^d (usually not exact), classic cases.
  for (const v of [0.125, 0.375, 2.5, -2.5, 1.005, 1.045, 1.0005, 0.0005, 0.00005, 8.345, 1.45, 10.235, 1234.5678,
    0.5, -0.5, 1.5, 0.05, 0.005, 5e-7, 4.9999999999999996e-7, 9.9999995, 999999.9999995, 0.0000005, -0.0000004]) push(v);
  for (let i = 0; i < 20000; i++) push((rng() & 1 ? -1 : 1) * randInt(rng, 0, 1 << 20) / 2 ** randInt(rng, 1, 24));
  for (let i = 0; i < 10000; i++) push((randInt(rng, -1e7, 1e7) + 0.5) / 10 ** randInt(rng, 0, 6));
  for (let i = 0; i < 10000; i++) push(uniform(rng, -1e21, 1e21));
  for (let i = 0; i < 10000; i++) push(uniform(rng, -1e-5, 1e-5));
  return xs;
}

export function genFormat(outDir, seed = 777) {
  const rng = makeRng(seed);
  const t0 = Date.now();
  const xs = genValues(rng);
  const lines = new Array(xs.length);
  for (let i = 0; i < xs.length; i++) {
    const x = xs[i];
    let line = toBitsHex(x) + '|' + String(x);
    for (let d = 0; d <= 6; d++) line += '|' + x.toFixed(d);
    lines[i] = line;
  }
  fs.mkdirSync(outDir, { recursive: true });
  fs.writeFileSync(path.join(outDir, 'format.txt'), lines.join('\n') + '\n');

  // Extra: toFixed with 7..100 digits (JsMath.ToFixed supports the full JS range 0..100).
  // Lines: <hex bits>|<digits>|x.toFixed(digits)
  const ext = [];
  const extDigits = [7, 8, 9, 10, 12, 15, 16, 17, 18, 19, 20, 21, 22, 25, 30, 40, 50, 64, 80, 99, 100];
  for (let i = 0; i < xs.length; i += 17) {
    const x = xs[i];
    for (const d of extDigits) ext.push(toBitsHex(x) + '|' + d + '|' + x.toFixed(d));
  }
  for (const x of [0.1, 0.125, 1.005, 2 ** -20, 2 ** -60, 5e-324, 1e-300, 123.456, 999999999999999900000, 2 ** 69, -(2 ** 60) - 2048]) {
    for (let d = 7; d <= 100; d++) ext.push(toBitsHex(x) + '|' + d + '|' + x.toFixed(d));
  }
  fs.writeFileSync(path.join(outDir, 'tofixed-ext.txt'), ext.join('\n') + '\n');
  console.log(`format vectors: ${xs.length} values x (String + toFixed(0..6)), ${ext.length} toFixed(7..100) (${Date.now() - t0} ms)`);
}
