// Sanity check that V8's execution tiers agree with each other (the vectors from gen.mjs are
// produced by generic loops; the game code will run with literal constants in optimized code).
//   node --allow-natives-syntax tests/jsvectors/tiers.mjs
// (Node 22 release builds have Maglev compiled out, v8_enable_maglev=0, so the tiers are
// Ignition -> Sparkplug -> TurboFan; on a build with Maglev pass --maglev to include it.)
// Compares Ignition (never optimized) vs Sparkplug vs Maglev vs TurboFan, incl. TurboFan's
// literal-exponent reductions (x ** 2 -> x*x, x ** 0.5 -> sqrt) and compile-time constant folding.
import { makeRng, randBits, uniform, logUniform, SPECIALS, toBitsHex } from './lib.mjs';

const rng = makeRng(123);
const xs = [...SPECIALS];
for (let i = 0; i < 3000; i++) xs.push(randBits(rng), uniform(rng, -50, 5), uniform(rng, 0.01, 1000),
  logUniform(rng, -1074, 1023, true), uniform(rng, -10, 10), Math.round(uniform(rng, -5, 5)) / 2);

const unary = [
  'Math.exp(x)', 'Math.log(x)', 'Math.round(x)', 'x | 0', 'x >>> 0', 'String(x)', "'' + x", 'x.toFixed(2)', 'x.toFixed(0)',
  'Math.pow(x, 0.5)', 'x ** 0.5', 'Math.pow(x, 2)', 'x ** 2', 'Math.pow(x, 0.3)', 'x ** 2.4', 'Math.pow(x, -0.5)',
  'Math.pow(x, 3)', 'x ** -1', 'x ** 0', 'Math.pow(2, x)', 'Math.pow(10, x)', 'Math.E ** x', 'Math.pow(-x, 2)',
  'Math.min(x, 0)', 'Math.max(x, -0)', 'Math.min(x, 1, -x)', 'Math.max(x, 0.5 * x)',
];

const maglevBuilt = !!process.config.variables.v8_enable_maglev;
const maglevEnabled = maglevBuilt && process.execArgv.includes('--maglev');
const same = (a, b) => (typeof a === 'number' && typeof b === 'number') ? (Object.is(a, b) || (a !== a && b !== b)) : a === b;
let failures = 0, checks = 0, maglevCompiled = 0, turbofanCompiled = 0;

for (const expr of unary) {
  // Distinct source text per copy, so each gets its own SharedFunctionInfo (tier state).
  const make = (tag) => new Function('x', `/* ${tag} */ return ${expr};`);
  const fi = make('ignition'), fb = make('sparkplug'), fm = make('maglev'), ft = make('turbofan');
  %NeverOptimizeFunction(fi);
  for (const f of [fb, fm, ft]) { %PrepareFunctionForOptimization(f); for (let i = 0; i < 50; i++) f(xs[i]); }
  %NeverOptimizeFunction(fb); %CompileBaseline(fb);
  if (maglevEnabled) { %OptimizeMaglevOnNextCall(fm); fm(1.5); }
  %OptimizeFunctionOnNextCall(ft); ft(1.5);
  for (const x of xs) {
    const r = fi(x), rb = fb(x), rm = fm(x), rt = ft(x);
    checks++;
    if (!same(r, rb) || !same(r, rm) || !same(r, rt)) {
      if (failures++ < 10) console.log(`MISMATCH ${expr} x=${toBitsHex(x)}: ignition=${r} sparkplug=${rb} maglev=${rm} turbofan=${rt}`);
    }
  }
  // %GetOptimizationStatus bits (V8 12.x): 1<<5 = Maglev code, 1<<6 = TurboFan code.
  if (%GetOptimizationStatus(fm) & 32) maglevCompiled++;
  if (%GetOptimizationStatus(ft) & 64) turbofanCompiled++;
}

// Compile-time constant folding: Math.pow / Math.exp / Math.log on literal arguments.
for (let i = 0; i < 1500; i++) {
  const x = i % 3 === 0 ? randBits(rng) : uniform(rng, 0.01, 1000);
  const y = i % 2 === 0 ? uniform(rng, -5, 5) : [0.3, 2.4, 2, 0.5][i % 4];
  const lit = (v) => (v !== v ? 'NaN' : Object.is(v, -0) ? '-0' : v === Infinity ? 'Infinity' : v === -Infinity ? '-Infinity' : String(v));
  const f = new Function(`/* fold ${i} */ return [Math.pow(${lit(x)}, ${lit(y)}), Math.exp(${lit(y)}), Math.log(${lit(x)})];`);
  %PrepareFunctionForOptimization(f); f(); %OptimizeFunctionOnNextCall(f);
  const folded = f();
  const ref = [Math.pow(x, y), Math.exp(y), Math.log(x)];
  for (let j = 0; j < 3; j++) {
    checks++;
    if (!same(folded[j], ref[j])) { if (failures++ < 10) console.log(`MISMATCH folded #${j} x=${x} y=${y}: ${folded[j]} vs ${ref[j]}`); }
  }
}

console.log(`tiers: ${checks} comparisons, ${failures} mismatches; still optimized at the end: ` +
  `TurboFan ${turbofanCompiled}/${unary.length}, Maglev ${maglevEnabled ? maglevCompiled + '/' + unary.length
    : maglevBuilt ? 'not enabled (pass --maglev)' : 'not built into this Node'}`);
process.exitCode = failures ? 1 : 0;
