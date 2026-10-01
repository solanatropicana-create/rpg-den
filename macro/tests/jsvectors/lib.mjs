// Shared helpers for the JS-semantics vector generators (run with Node 22).
import fs from 'node:fs';
import path from 'node:path';

// 32-bit PRNG (mulberry32 variant). The C# checker ports this exactly (JsRng in Program.cs);
// it is only needed on both sides for the random sort comparators.
export function makeRng(seed) {
  let a = seed >>> 0;
  return function next() {
    a = (a + 0x6D2B79F5) >>> 0;
    let t = a;
    t = Math.imul(t ^ (t >>> 15), t | 1) >>> 0;
    t = (t ^ (t + Math.imul(t ^ (t >>> 7), t | 61))) >>> 0;
    return (t ^ (t >>> 14)) >>> 0;
  };
}

const scratch = new DataView(new ArrayBuffer(8));

export function fromBits(hi, lo) {
  scratch.setUint32(0, lo >>> 0, true);
  scratch.setUint32(4, hi >>> 0, true);
  return scratch.getFloat64(0, true);
}

export function toBitsHex(x) {
  scratch.setFloat64(0, x, true);
  return scratch.getUint32(4, true).toString(16).padStart(8, '0') + scratch.getUint32(0, true).toString(16).padStart(8, '0');
}

export function hiWord(x) { scratch.setFloat64(0, x, true); return scratch.getUint32(4, true); }
export function loWord(x) { scratch.setFloat64(0, x, true); return scratch.getUint32(0, true); }

// x stepped by k ulps along the integer bit pattern (k may be negative; stays within the sign).
export function stepUlps(x, k) {
  scratch.setFloat64(0, x, true);
  let v = scratch.getBigUint64(0, true);
  const sign = v & 0x8000000000000000n;
  let mag = v & 0x7FFFFFFFFFFFFFFFn;
  mag = mag + BigInt(k);
  if (mag < 0n) mag = 0n;
  if (mag > 0x7FF0000000000000n) mag = 0x7FF0000000000000n;
  scratch.setBigUint64(0, sign | mag, true);
  return scratch.getFloat64(0, true);
}

export function randBits(rng) { return fromBits(rng(), rng()); }
// uniform in [0, 1) with 53 random bits
export function rand01(rng) { return ((rng() >>> 11) * 4294967296 + rng()) / 9007199254740992; }
export function uniform(rng, lo, hi) { return lo + (hi - lo) * rand01(rng); }
export function randInt(rng, lo, hi) { return lo + Math.floor(rand01(rng) * (hi - lo + 1)); } // inclusive
// |x| = 2^e with e uniform in [e0, e1], random sign if signed
export function logUniform(rng, e0, e1, signed = false) {
  const v = Math.pow(2, uniform(rng, e0, e1));
  return signed && (rng() & 1) ? -v : v;
}
export function pick(rng, arr) { return arr[rng() % arr.length]; }

export const SPECIALS = [
  0, -0, Infinity, -Infinity, NaN,
  fromBits(0x7FF80000, 1), fromBits(0xFFF80000, 0), fromBits(0x7FF00000, 1), fromBits(0x7FF40000, 0),
  1, -1, 0.5, -0.5, 2, -2, 3, -3, 1.5, -1.5, 10, -10, 0.1, -0.1, 0.25, 4, 1 / 3, 2 / 3,
  Number.MIN_VALUE, -Number.MIN_VALUE, 2.2250738585072014e-308, -2.2250738585072014e-308,
  2.225073858507201e-308, -2.225073858507201e-308, Number.MAX_VALUE, -Number.MAX_VALUE,
  Number.EPSILON, 1 + Number.EPSILON, 1 - Number.EPSILON / 2, -1 - Number.EPSILON, -1 + Number.EPSILON / 2,
  2 ** 31, -(2 ** 31), 2 ** 32, 2 ** 52, 2 ** 53, 2 ** 63, 2 ** 64, 2 ** 1023, 2 ** -1022, 2 ** -1074,
  2 ** -28, 2 ** -20, 2 ** 31 + 1, 2 ** 53 + 2, -(2 ** 53) - 2,
  Math.E, Math.PI, Math.LN2, Math.LN10, Math.SQRT2, Math.SQRT1_2,
  709.782712893384, -745.1332191019412, 709.78, -745.13, 1e300, 1e-300, -1e300, -1e-300,
  1e21, 1e-7, 1e-6, 123456789, -123456789, 0.49999999999999994, -0.49999999999999994,
];

// Buffered binary writer for fixed-layout little-endian records.
export class BinWriter {
  constructor(capacityBytes = 1 << 20) { this.buf = Buffer.alloc(capacityBytes); this.pos = 0; }
  ensure(n) {
    if (this.pos + n <= this.buf.length) return;
    let cap = this.buf.length * 2;
    while (cap < this.pos + n) cap *= 2;
    const nb = Buffer.alloc(cap); this.buf.copy(nb, 0, 0, this.pos); this.buf = nb;
  }
  f64(x) { this.ensure(8); this.buf.writeDoubleLE(x, this.pos); this.pos += 8; }
  i32(x) { this.ensure(4); this.buf.writeInt32LE(x | 0, this.pos); this.pos += 4; }
  u32(x) { this.ensure(4); this.buf.writeUInt32LE(x >>> 0, this.pos); this.pos += 4; }
  save(file) { fs.mkdirSync(path.dirname(file), { recursive: true }); fs.writeFileSync(file, this.buf.subarray(0, this.pos)); }
}
