// Golden-test harness, TS side (C# twin: FD.Macro.Run/Program.cs; driver: golden/compare.py; docs: golden/README.md).
// Build + run from macro/:  node golden/ts/build.mjs trace && node golden/out/trace.js <mode> ...
//   hash <seed> <days>          one line per day (0 = right after new Sim):  <day> <hash> <rngState> <rngCalls> <pathCache> <navCache>
//   cps <seed> <day>            days 1..day-1 silently, then one line per checkpoint of <day>:  <label> <hash> <rngState> <rngCalls>
//   dump <seed> <day> [label]   flat canonical listing (path=value) at checkpoint <label> of <day> (default: end; day 0 = construction;
//                               'start' = before the step; a comma-separated label list prints each listing after a line '@<label>')
//   rng <seed> <day>            every RNG call of <day>:  <index> <lastCheckpointLabel> <value> <callerFunction>
//   verify <seeds> <days>       proves tracedStep() == sim.step() (seeds "1,2,3" or "1-3")
// The canonical form / hash (FNV-1a 64 over UTF-16 code units) must stay byte-identical to the C# runner.
import * as fs from 'node:fs';
import { Sim, YEAR } from '../../../src/sim/sim';
import { Rng } from '../../../src/sim/rng';
import { economyTick, chooseBuilds, recruitTick, repairTick } from '../../../src/sim/economy';
import { disastersTick } from '../../../src/sim/events';
import { chooseResearch, eraCheck, classYearly } from '../../../src/sim/research';
import { relationsTick, worldTick, considerExpansion, considerTrade, considerWarAction, considerScout, considerRaid } from '../../../src/sim/diplomacy';
import { campsTick } from '../../../src/sim/monsters';
import { tavernsTick, heroesTick, considerHero, considerQuest } from '../../../src/sim/heroes';
import { innsTick, considerInnBids } from '../../../src/sim/inns';
import { agentsTick, routesTick, roadsTick } from '../../../src/sim/agents';
import { seaTick } from '../../../src/sim/sea';
import type { Civ } from '../../../src/sim/types';

export type Checkpoint = (label: string) => void;

// ================================================================ traced step
// Exact mirror of Sim.step() + Sim.civAI() (src/sim/sim.ts) with checkpoints at the same places and with the
// same labels as the C# Sim.Step / Sim.CivAI (FD.Macro/Core/Sim.cs). `verify` proves it equals sim.step().
export function tracedStep(sim: Sim, cp: Checkpoint) {
  const w = sim.w;
  w.day++;
  for (const c of w.civs) if (c.alive) { economyTick(sim, c); cp('econ:' + c.id); }
  if (w.day % 5 === 0) { for (const c of w.civs) if (c.alive) repairTick(sim, c); cp('repair'); }
  for (const c of w.civs) if (c.alive && (w.day + c.id * 3) % 10 === 0) tracedCivAI(sim, c, cp);
  if (w.day % 10 === 0) { sim.updateTerritory(); cp('territory'); relationsTick(sim); cp('relations'); }
  if (w.day % 30 === 0) { sim.discover(); cp('discover'); worldTick(sim); cp('world'); disastersTick(sim); cp('disasters'); }
  if (w.day % YEAR === 0) { for (const c of w.civs) if (c.alive) classYearly(sim, c); cp('yearly'); }
  campsTick(sim); cp('camps');
  tavernsTick(sim); cp('taverns');
  innsTick(sim); cp('inns');
  if (w.day % 5 === 0) { heroesTick(sim); cp('heroes'); }
  agentsTick(sim); cp('agents');
  seaTick(sim); cp('sea');
  routesTick(sim); cp('routes');
  roadsTick(sim); cp('roads');
  for (const c of w.civs) c.threat = Math.min(1.5, Math.max(0, c.threat - 0.0015));
  if (w.day % 60 === 0) for (const c of w.civs) if (c.alive) c.history.push({ day: w.day, pop: sim.civPop(c), gold: Math.round(sim.st(c, 'gold')), techs: c.research.done.length });
  w.rngState = sim.rng.state();
  cp('end');
}

export function tracedCivAI(sim: Sim, c: Civ, cp: Checkpoint) {
  const p = 'ai:' + c.id + ':';
  if (!sim.civSettlements(c).length) { sim.extinct(c); cp(p + 'extinct'); return; }
  if (!c.research.current) chooseResearch(sim, c);
  cp(p + 'research');
  eraCheck(sim, c); cp(p + 'era');
  chooseBuilds(sim, c); cp(p + 'builds');
  recruitTick(sim, c); cp(p + 'recruit');
  considerExpansion(sim, c); cp(p + 'expand');
  considerHero(sim, c); cp(p + 'hero');
  considerInnBids(sim, c); cp(p + 'bids');
  considerQuest(sim, c); cp(p + 'quest');
  considerScout(sim, c); cp(p + 'scout');
  considerTrade(sim, c); cp(p + 'trade');
  considerWarAction(sim, c); cp(p + 'war');
  considerRaid(sim, c); cp(p + 'raid');
}

// ================================================================ RNG instrumentation
// Every Rng instance counts its own next() calls (like C# Rng.Calls, which is per instance); only the calls of
// the `traced` instance (the sim's rng) are reported to `onRng`.
type CountedRng = Rng & { __calls?: number };
const rawNext = Rng.prototype.next;
let traced: Rng | null = null;
let onRng: (v: number, n: number) => void = () => {};
Rng.prototype.next = function next(this: CountedRng): number {
  const v = rawNext.call(this);
  const n = (this.__calls = (this.__calls ?? 0) + 1);
  if (this === traced) onRng(v, n);
  return v;
};
export const rngCalls = (sim: Sim) => (sim.rng as CountedRng).__calls ?? 0;
const pathCacheSize = (sim: Sim) => (sim as unknown as { pathCache: Map<string, unknown> }).pathCache.size;

/** esbuild renames colliding top-level names of different modules (innlife's syncHeroes → syncHeroes2):
 *  map them back by scanning this bundle for `fooN` definitions whose base name `foo` is also defined. */
let renames: Map<string, string> | null = null;
function bundleRenames() {
  const map = new Map<string, string>();
  try {
    const src = fs.readFileSync(__filename, 'utf8');
    const defs = new Set<string>();
    for (const m of src.matchAll(/\bfunction\s+([A-Za-z_$][\w$]*)\s*\(/g)) defs.add(m[1]);
    for (const m of src.matchAll(/\b(?:var|let|const)\s+([A-Za-z_$][\w$]*)\s*=\s*(?:function\b|(?:async\s*)?(?:\([^()]*\)|[A-Za-z_$][\w$]*)\s*=>)/g)) defs.add(m[1]);
    for (const d of defs) { const m = /^(.*\D)(\d+)$/.exec(d); if (m && Number(m[2]) >= 2 && defs.has(m[1])) map.set(d, m[1]); }
  } catch { /* informational only */ }
  return map;
}
/** First named, non-Rng, non-builtin function on the stack above Rng.next (informational). */
function callerName(): string {
  const lines = (new Error().stack ?? '').split('\n');
  let seenRng = false;
  for (let i = 1; i < lines.length; i++) {
    const m = /^\s*at (?:async )?(.*?) \((.*)\)$/.exec(lines[i]);
    if (!m) continue;                                   // anonymous function frame: "at file:line:col"
    const loc = m[2];
    if (loc === '<anonymous>' || loc === 'native' || loc.startsWith('node:')) continue;   // builtins (Array.filter...)
    const fn = m[1].replace(/ \[as [^\]]*\]$/, '').replace(/^new /, '');
    if (fn.startsWith('Rng.')) { seenRng = true; continue; }
    if (!seenRng) continue;                             // the tracer's own frames
    const name = fn.slice(fn.lastIndexOf('.') + 1);
    if (!name || name === '<anonymous>') continue;
    renames ??= bundleRenames();
    return renames.get(name) ?? name;
  }
  return '?';
}

// ================================================================ canonical form + FNV-1a 64 hash
// object: keys sorted (UTF-16 code unit order), null/undefined values and '_'-prefixed keys skipped;
// number: NaN/Infinity/-Infinity, -0 → 0, else String(x); string: "…" with \\ \" and \u00xx (< 0x20) escapes;
// array: [a,b], null/undefined elements → null. Hash: FNV-1a 64 over UTF-16 code units, as two uint32 halves
// (h * 0x100000001b3 = h * 0x1b3 + (lo << 40) mod 2^64).
// HI/LO hold the two uint32 halves as int32 bit patterns so they stay small integers (no heap numbers):
// lo * 0x1b3 = (lo >>> 16) * 0x1b3 * 2^16 + (lo & 0xffff) * 0x1b3, split to get the carry into the high half.
let HI = 0, LO = 0;
function hReset() { HI = 0xcbf29ce4 | 0; LO = 0x84222325 | 0; }
function hUnit(c: number) {
  const l = LO ^ c;
  const carry = (((l >>> 16) * 435) + (((l & 0xffff) * 435) >>> 16)) >>> 16;
  HI = (Math.imul(HI, 435) + carry + (l << 8)) | 0;
  LO = Math.imul(l, 435);
}
function hRaw(s: string) {
  let hi = HI, lo = LO;
  for (let i = 0; i < s.length; i++) {
    const l = lo ^ s.charCodeAt(i);
    const carry = (((l >>> 16) * 435) + (((l & 0xffff) * 435) >>> 16)) >>> 16;
    hi = (Math.imul(hi, 435) + carry + (l << 8)) | 0;
    lo = Math.imul(l, 435);
  }
  HI = hi; LO = lo;
}
const HEX = '0123456789abcdef';
/** '"' + escaped s + '"' in one pass (escapes are rare: they take the slow path through hUnit). */
function hQuoted(s: string) {
  let hi = HI, lo = LO;
  for (let i = -1; i <= s.length; i++) {
    const c = i < 0 || i === s.length ? 34 : s.charCodeAt(i);
    if ((c === 34 || c === 92 || c < 32) && i >= 0 && i < s.length) {
      HI = hi; LO = lo;
      hUnit(92);
      if (c < 32) { hUnit(117); hUnit(48); hUnit(48); hUnit(HEX.charCodeAt(c >> 4)); hUnit(HEX.charCodeAt(c & 15)); }
      else hUnit(c);
      hi = HI; lo = LO;
      continue;
    }
    const l = lo ^ c;
    const carry = (((l >>> 16) * 435) + (((l & 0xffff) * 435) >>> 16)) >>> 16;
    hi = (Math.imul(hi, 435) + carry + (l << 8)) | 0;
    lo = Math.imul(l, 435);
  }
  HI = hi; LO = lo;
}
const INTS: string[] = [];                      // String(i) for 0 <= i < 65536 (ids, tile indices, days...)
for (let i = 0; i < 65536; i++) INTS.push(String(i));
const numCache = new Map<number, string>();   // String(x) is slow; most non-integers (elevations...) never change
function hNum(x: number) {
  if ((x | 0) === x) {                  // int32 (also -0 → "0")
    if (x >= 0) { if (x < 65536) { hRaw(INTS[x]); return; } }
    else if (x > -65536) { hUnit(45); hRaw(INTS[-x]); return; }
  }
  let t = numCache.get(x);               // NaN, Infinity, -Infinity, fractions, big integers: JS String()
  if (t === undefined) { if (numCache.size >= 1 << 16) numCache.clear(); numCache.set(x, (t = String(x))); }
  hRaw(t);
}
export function numStr(x: number) { return Object.is(x, -0) ? '0' : String(x); }
export function jsQuote(s: string) {
  let o = '"';
  for (let i = 0; i < s.length; i++) {
    const c = s.charCodeAt(i);
    if (c === 34 || c === 92) o += '\\' + s[i];
    else if (c < 32) o += '\\u00' + HEX[c >> 4] + HEX[c & 15];
    else o += s[i];
  }
  return o + '"';
}

// Objects are written by a serializer generated once per own-key list ("shape"): the keys in canonical order
// ('_'-prefixed ones dropped) as constant property loads with pre-escaped '"key":' / ',"key":' prefixes. Each
// shape gets its own monomorphic property-load ICs, and every (shape, key) / array remembers the shape of the
// object it saw last, so the lookup is usually one key-list comparison (optional keys are last: compare from the end).
interface Shape { keys: string[]; fn: (o: Record<string, unknown>) => void }
const shapesBySig = new Map<string, Shape>();
function sameKeys(a: string[], b: string[]) {
  if (a.length !== b.length) return false;
  for (let i = a.length - 1; i >= 0; i--) if (a[i] !== b[i]) return false;
  return true;
}
function shapeFor(ks: string[]): Shape {
  const sig = JSON.stringify(ks);
  let sh = shapesBySig.get(sig);
  if (sh === undefined) {
    const order = ks.filter((k) => k.charCodeAt(0) !== 95).sort();       // '_'-prefixed keys are skipped
    const lit = (x: string) => JSON.stringify(x);
    const body = order.map((k, i) => `
    v = o[${lit(k)}];
    if (v !== undefined && v !== null) {
      if (f) { f = false; R(${lit(jsQuote(k) + ':')}); } else R(${lit(',' + jsQuote(k) + ':')});
      if (typeof v === 'number') N(v);
      else if (typeof v === 'string') Q(v);
      else if (typeof v === 'object') { if (Array.isArray(v)) A(v); else h${i} = O(v, h${i}); }
      else V(v);
    }`).join('');
    const hints = order.length ? `let ${order.map((_, i) => `h${i}`).join(', ')};` : '';
    const make = new Function('R', 'N', 'Q', 'A', 'O', 'V', `${hints}\nreturn function (o) {\n  let v, f = true;${body}\n};`);
    sh = { keys: ks, fn: make(hRaw, hNum, hQuoted, hArr, hObj, hVal) };
    shapesBySig.set(sig, sh);
  }
  return sh;
}
function checkPlain(o: object, where: string) {
  const p = Object.getPrototypeOf(o);
  if (p !== Object.prototype && p !== null) throw new Error(`canonical form: unsupported ${o.constructor?.name ?? 'object'}${where}`);
}
/** Any value; undefined/null → null (array element context). */
function hVal(v: unknown) {
  switch (typeof v) {
    case 'number': hNum(v); return;
    case 'string': hQuoted(v); return;
    case 'boolean': hRaw(v ? 'true' : 'false'); return;
    case 'undefined': hRaw('null'); return;
    case 'object':
      if (v === null) hRaw('null');
      else if (Array.isArray(v)) hArr(v);
      else hObj(v as Record<string, unknown>, undefined);
      return;
    default: throw new Error(`canonical form: unsupported ${typeof v}`);
  }
}
function hArr(a: unknown[]) {
  hUnit(91);
  let ring: Shape[] | undefined;                    // up to 8 shapes seen in this array, last one first
  for (let i = 0; i < a.length; i++) {
    if (i) hUnit(44);
    const v = a[i];
    if (typeof v === 'number') hNum(v);
    else if (typeof v === 'object' && v !== null && !Array.isArray(v)) {
      const o = v as Record<string, unknown>;
      checkPlain(o, '');
      const ks = Object.keys(o);
      let sh: Shape | undefined;
      if (ring === undefined) ring = [];
      for (let j = 0; j < ring.length; j++) if (sameKeys(ring[j].keys, ks)) { sh = ring[j]; break; }
      if (sh === undefined) { sh = shapeFor(ks); if (ring.length < 8) ring.push(sh); else ring[i & 7] = sh; }
      if (sh !== ring[0]) { const k = ring.indexOf(sh); ring[k] = ring[0]; ring[0] = sh; }
      hUnit(123); sh.fn(o); hUnit(125);
    } else hVal(v);
  }
  hUnit(93);
}
/** Writes a plain object; returns its shape (callers keep it as the hint for the next object in the same place). */
function hObj(o: Record<string, unknown>, hint: Shape | undefined): Shape {
  checkPlain(o, '');
  const ks = Object.keys(o);
  const sh = hint !== undefined && sameKeys(hint.keys, ks) ? hint : shapeFor(ks);
  hUnit(123);
  sh.fn(o);
  hUnit(125);
  return sh;
}
function hexOf(hi: number, lo: number) { return (hi >>> 0).toString(16).padStart(8, '0') + (lo >>> 0).toString(16).padStart(8, '0'); }
/** FNV-1a 64 of the canonical text of v (16 lowercase hex digits). */
export function canonHash(v: unknown): string { hReset(); hVal(v); return hexOf(HI, LO); }
/** FNV-1a 64 of a plain string (test vectors / fingerprints). */
export function strHash(s: string): string { hReset(); hRaw(s); return hexOf(HI, LO); }
/** The canonical text itself (slow; for tests). */
export function canonText(v: unknown): string {
  switch (typeof v) {
    case 'number': return numStr(v);
    case 'string': return jsQuote(v);
    case 'boolean': return v ? 'true' : 'false';
    case 'undefined': return 'null';
    case 'object': {
      if (v === null) return 'null';
      if (Array.isArray(v)) return '[' + v.map((x) => canonText(x)).join(',') + ']';
      checkPlain(v, '');
      const o = v as Record<string, unknown>;
      const parts: string[] = [];
      for (const k of Object.keys(o).sort()) {
        if (k.charCodeAt(0) === 95 || o[k] === undefined || o[k] === null) continue;
        parts.push(jsQuote(k) + ':' + canonText(o[k]));
      }
      return '{' + parts.join(',') + '}';
    }
    default: throw new Error(`canonical form: unsupported ${typeof v}`);
  }
}

// Flat listing: one "path=value" line per leaf, in canonical order (object keys sorted, array indices ascending);
// empty objects/arrays are leaves ("{}" / "[]"). Keys that are not plain words are written as ["key"].
function keyPath(path: string, k: string) {
  for (let i = 0; i < k.length; i++) {
    const c = k.charCodeAt(i);
    if (c <= 32 || c === 34 || c === 46 || c === 61 || c === 91 || c === 92 || c === 93) return path + '[' + jsQuote(k) + ']';
  }
  return path ? path + '.' + k : k;
}
function flat(v: unknown, path: string, out: string[]) {
  if (v !== null && typeof v === 'object') {
    if (Array.isArray(v)) {
      if (!v.length) { out.push(path + '=[]'); return; }
      for (let i = 0; i < v.length; i++) flat(v[i], path + '[' + i + ']', out);
      return;
    }
    checkPlain(v, ` at ${path}`);
    const o = v as Record<string, unknown>;
    let n = 0;
    for (const k of Object.keys(o).sort()) {
      if (k.charCodeAt(0) === 95 || o[k] === undefined || o[k] === null) continue;
      flat(o[k], keyPath(path, k), out); n++;
    }
    if (!n) out.push(path + '={}');
    return;
  }
  out.push(path + '=' + canonText(v));
}
export function flatDump(v: unknown): string[] { const out: string[] = []; flat(v, '', out); return out; }

// ================================================================ CLI
class Stop { }
const STOP = new Stop();
let curDay = 0, curLabel = 'init';
const track: Checkpoint = (l) => { curLabel = l; };
const write = (s: string) => { process.stdout.write(s); };
process.stdout.on('error', (e: NodeJS.ErrnoException) => { if (e.code === 'EPIPE') process.exit(0); throw e; });   // reader went away
function fail(msg: string): never { process.stderr.write(msg + '\n'); process.exit(2); }
function num(s: string | undefined, what: string) {
  const x = Number(s);
  if (s === undefined || s.trim() === '' || Number.isNaN(x)) fail(`bad ${what}: ${s}`);
  return x;
}
function day(s: string | undefined) {
  const d = num(s, 'day');
  if (!Number.isInteger(d) || d < 0) fail(`bad day: ${s}`);
  return d;
}
function seedList(s: string) {
  const out: number[] = [];
  for (const part of s.split(',')) {
    const m = /^(-?\d+)-(-?\d+)$/.exec(part);
    if (m) for (let x = Number(m[1]); x <= Number(m[2]); x++) out.push(x);
    else out.push(num(part, 'seed'));
  }
  return out;
}
function newSim(seed: number) { curDay = 0; curLabel = 'init'; return new Sim(seed); }
function runTo(sim: Sim, d: number) { for (let x = 1; x < d; x++) { curDay = x; tracedStep(sim, track); } curDay = d; }
const state = (sim: Sim) => `${canonHash(sim.w)} ${String(sim.rng.state())} ${rngCalls(sim)}`;

function modeHash(seed: number, days: number) {
  const sim = newSim(seed);
  const line = (d: number) => write(`${d} ${state(sim)} ${pathCacheSize(sim)} ${sim.navCache.size}\n`);
  line(0);
  for (let d = 1; d <= days; d++) { curDay = d; curLabel = '-'; sim.step(); line(d); }
}

function modeCps(seed: number, d: number) {
  const sim = newSim(seed);
  if (d === 0) { write(`init ${state(sim)}\n`); return; }
  runTo(sim, d);
  tracedStep(sim, (l) => { curLabel = l; write(`${l} ${state(sim)}\n`); });
}

/** labels: one checkpoint label of day d (default "end"), or a comma-separated list (then each listing is preceded by a
 *  line "@label"). "start" = before the step of day d (the end of day d-1); day 0 only has init (= start = end). */
function modeDump(seed: number, d: number, labels?: string) {
  const want = (labels ?? 'end').split(',');
  const got = new Map<string, string[]>();
  const sim = newSim(seed);
  if (d === 0) {
    for (const l of want) if (l !== 'init' && l !== 'start' && l !== 'end') fail(`day 0 has no checkpoint '${l}' (only init)`);
    const flat = flatDump(sim.w);
    for (const l of want) got.set(l, flat);
  } else {
    runTo(sim, d);
    if (want.includes('start')) got.set('start', flatDump(sim.w));
    const n = new Set(want).size;
    if (got.size < n) {
      try {
        tracedStep(sim, (l) => {
          curLabel = l;
          if (!want.includes(l) || got.has(l)) return;
          got.set(l, flatDump(sim.w));
          if (got.size === n) throw STOP;
        });
      } catch (e) { if (e !== STOP) throw e; }
    }
    for (const l of want) if (!got.has(l)) fail(`checkpoint '${l}' not reached on day ${d}`);
  }
  for (const l of want) {
    if (want.length > 1) write(`@${l}\n`);
    write(got.get(l)!.join('\n') + '\n');
  }
}

function modeRng(seed: number, d: number) {
  if (d < 1) fail('rng: day must be >= 1 (the RNG calls of new Sim() cannot be traced on the C# side)');
  const sim = newSim(seed);
  runTo(sim, d);
  let label = 'start';
  Error.stackTraceLimit = 64;
  traced = sim.rng;
  onRng = (v, n) => write(`${n} ${label} ${String(v)} ${callerName()}\n`);
  tracedStep(sim, (l) => { label = l; curLabel = l; });
  traced = null;
}

function modeVerify(seeds: number[], days: number) {
  let bad = 0;
  for (const seed of seeds) {
    const t0 = Date.now();
    const a = newSim(seed), b = newSim(seed);
    let firstBad = -1;
    for (let d = 1; d <= days; d++) {
      curDay = d;
      a.step();
      tracedStep(b, track);
      if (firstBad < 0 && (a.rng.state() !== b.rng.state() || rngCalls(a) !== rngCalls(b))) firstBad = d;
    }
    const ja = JSON.stringify(a.w), jb = JSON.stringify(b.w);
    const ha = canonHash(a.w), hb = canonHash(b.w);
    const same = ja === jb && ha === hb && firstBad < 0 && pathCacheSize(a) === pathCacheSize(b) && a.navCache.size === b.navCache.size;
    if (!same) bad++;
    write(`seed ${seed}: ${days} days sim.step() vs tracedStep(): ${same ? 'IDENTICAL' : 'DIFFERENT'} (JSON ${ja.length} chars ${ja === jb ? '==' : '!='}, hash ${ha}${ha === hb ? '' : ' != ' + hb}, rng calls ${rngCalls(a)}${firstBad < 0 ? '' : ', rng differs from day ' + firstBad}, ${Date.now() - t0} ms)\n`);
  }
  if (bad) process.exitCode = 1;
}

/** Timing: plain sim.step() vs step + per-day world hash, and the canonical size of the final world. */
function modeBench(seed: number, days: number) {
  let t = process.hrtime.bigint();
  const ms = () => { const n = process.hrtime.bigint(); const d = Number(n - t) / 1e6; t = n; return d.toFixed(0); };
  const a = newSim(seed);
  write(`new Sim: ${ms()} ms\n`);
  for (let d = 1; d <= days; d++) { curDay = d; a.step(); }
  write(`${days} x sim.step(): ${ms()} ms\n`);
  const b = newSim(seed);
  ms();
  for (let d = 1; d <= days; d++) { curDay = d; b.step(); canonHash(b.w); }
  write(`${days} x (sim.step() + world hash): ${ms()} ms\n`);
  const n = 20;
  for (let i = 0; i < n; i++) canonHash(b.w);
  write(`final world: ${canonText(b.w).length} canonical chars, hash ${(Number(ms()) / n).toFixed(1)} ms each; JSON.stringify ${JSON.stringify(b.w).length} chars\n`);
}

// Test vectors shared with the C# runner (`selftest` mode on both sides must print the same lines).
function modeSelfTest() {
  const strs = ['', 'a', 'foobar', 'Dünya uyandı. İğne ışık Çağı ÖŞÜ', 'q"b\\s/\n\t\u0001\u001f\u007f', '😀 \ud800x', '0', '{"a":1}'];
  for (const s of strs) write(`str ${strHash(s)} ${canonHash(s)} ${jsQuote(s)}\n`);
  const nums = [0, -0, 1, -1, 7, 10, 99, 100, 2147483647, -2147483648, 2147483648, 4294967296, 1e21, 1e-7, 123456789012345680000,
    0.1, 0.30000000000000004, -1.5, 5e-324, 1.7976931348623157e308, NaN, Infinity, -Infinity, 1 / 3, 2 ** 53, -(2 ** 53) - 2, 0.000001, 1e-6 * 3];
  for (const x of nums) write(`num ${canonHash(x)} ${numStr(x)}\n`);
  const obj = { b: 1, a: [1, null, undefined, 'x', { z: true, y: false, _skip: 5 }], c: null, d: undefined, 'é': {}, 'B': [], '10': 1, '9': 2, 'k"q': -0 };
  write(`obj ${canonHash(obj)} ${canonText(obj)}\n`);
  for (const l of flatDump(obj)) write(`flat ${l}\n`);
}

const fingerprint = strHash(Sim.prototype.step.toString() + '\n' + Sim.prototype.civAI.toString());
const EXPECTED_FINGERPRINT = '4b91dcf93c31f32a';
if (fingerprint !== EXPECTED_FINGERPRINT && process.env.GOLDEN_QUIET !== '1')
  process.stderr.write(`warning: Sim.step()/civAI() source fingerprint ${fingerprint} != ${EXPECTED_FINGERPRINT}: sim.ts changed? update tracedStep() and the C# checkpoints, re-run verify\n`);

function main(args: string[]) {
  const [mode, a, b, c] = args;
  switch (mode) {
    case 'hash': return modeHash(num(a, 'seed'), day(b));
    case 'cps': return modeCps(num(a, 'seed'), day(b));
    case 'dump': return modeDump(num(a, 'seed'), day(b), c);
    case 'rng': return modeRng(num(a, 'seed'), day(b));
    case 'verify': return modeVerify(seedList(a ?? '1-3'), day(b ?? '1800'));
    case 'selftest': return modeSelfTest();
    case 'fingerprint': return write(fingerprint + '\n');
    case 'bench': return modeBench(num(a, 'seed'), day(b ?? '1800'));
    default: fail('usage: trace.js hash <seed> <days> | cps <seed> <day> | dump <seed> <day> [label] | rng <seed> <day> | verify <seeds> <days> | selftest');
  }
}

try {
  main(process.argv.slice(2));
} catch (e) {
  const err = e as Error;
  process.stderr.write(`CRASH ${curDay} ${curLabel} ${err?.name ?? 'Error'}: ${String(err?.message ?? e).split('\n')[0]}\n${err?.stack ?? ''}\n`);
  process.exitCode = 3;
}
export { Sim };
