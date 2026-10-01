# Golden test: TS simulation vs C# port

Runs the TypeScript simulation (`src/sim`, the reference) and the C# port (`FD.Macro`) with the same seed, compares
the whole world state after every simulated day and, on the first difference, narrows it down to a checkpoint
inside that day, the state paths that differ and the first diverging RNG call.

```
cd macro
python3 golden/compare.py --seeds 1,2,3 --days 7200 [--jobs N]
```

`compare.py` builds the TS bundle (`golden/out/trace.js`) and the C# runner (`FD.Macro.Run`, Release) once, checks
that both runners print the same `selftest` output (canonical-form test vectors), then for each seed:

1. runs `hash` on both sides in parallel and compares the per-day lines as they arrive (the first difference stops both);
2. `cps` for the first differing day on both sides → the first differing checkpoint;
3. `dump` on both sides at that checkpoint and at the previous one → the first 40 differing paths with the TS value,
   the C# value and the value before the segment, plus counts (changed / TS-only / C#-only, paths each side changed
   in the segment) and a per-area summary;
4. `rng` for that day on both sides → the first divergent RNG call with ±5 calls of context and both callers.

A crash (e.g. `NotImplementedException`) is reported as `C# crashed at day N (last checkpoint X): Type: message`
with the top stack frames. The checkpoints of the crash day are still compared, so the report also says whether
the state was still identical up to the crash.
Exit code: 0 only if every seed matches on every day; 1 on any difference or crash; 2 on build/usage errors.

| option | default | |
|---|---|---|
| `--seeds` | `1,2,3` | comma-separated seeds and ranges (`1-20`) |
| `--days` | `7200` | days per seed (`0` = only the state right after `new Sim(seed)`) |
| `--jobs` | `1` | seeds checked in parallel (each runs two processes) |
| `--no-build` | | skip both builds |
| `--paths` / `--context` | `40` / `5` | differing paths / RNG context lines to print |
| `--progress` | `1000` | progress line every N days (`0`: off) |
| `--fault` | | harness self-test, see below |

Example report (produced with `--fault state@3:econ:2`, see below):

```
seed 2: FIRST DIFFERENCE on day 3 (days 0..2 identical)
  TS  3 d8eb9687c1ebfdb9 75511158219 40 1 0
  C#  3 1b4e40a889980b35 75511158219 40 1 0   [hash]
  checkpoints of day 3: 2 identical (through econ:1), first difference at #3:
    TS  econ:2           29a433200fa9c0ae 68184894967 36
    C#  econ:2           a62c67d09f259bf2 68184894967 36   [hash]
  state at econ:2: 1 paths differ (0 changed, 0 TS-only, 1 C#-only); since econ:1: TS changed 6 paths, C# 7
    metrics.goldenFault  TS (absent)                  C# 1                         (before: (absent))
  RNG (day 3): TS 7 calls, C# 7 calls -- identical sequence, labels and callers
```

## The two runners

Both take the same arguments and print byte-identical output when the port is exact (run from `macro/`):

```
node golden/ts/build.mjs trace                                     # TS  → golden/out/trace.js
node golden/out/trace.js <mode> ...
dotnet build FD.Macro.Run/FD.Macro.Run.csproj -c Release           # C#
dotnet FD.Macro.Run/bin/Release/net8.0/FD.Macro.Run.dll <mode> ...
```

| mode | output |
|---|---|
| `hash <seed> <days>` | one line for day 0 (right after `new Sim(seed)`) and one per simulated day: `<day> <hash> <rngState> <rngCalls> <pathCacheSize> <navCacheSize>` |
| `cps <seed> <day>` | days 1..day−1 silently, then one line per checkpoint of `<day>`: `<label> <hash> <rngState> <rngCalls>` (day 0: one `init` line) |
| `dump <seed> <day> [label]` | flat canonical listing of the world at checkpoint `label` of `<day>` (default `end`; `start` = before the step, i.e. the end of day−1; day 0 = right after construction): one `path=value` line per leaf. A comma-separated label list prints each listing after a line `@<label>` (one run instead of several). |
| `rng <seed> <day>` | every RNG call of `<day>` (day ≥ 1): `<index> <lastCheckpointLabel> <value> <callerFunction>` |
| `selftest` | canonical-form test vectors (strings, numbers, key order, skipping rules); must be identical on both sides |

TS only: `verify <seeds> <days>` (proves `tracedStep()` ≡ `sim.step()`), `bench <seed> <days>`, `fingerprint`.
C# only: `bench <seed> <days>`.
Errors: `CRASH <day> <lastCheckpoint> <Type>: <message>` + stack on stderr, exit code 3; usage errors exit 2.

Column details:
- `hash`: 16 lowercase hex digits, FNV-1a 64 of the canonical text of the world (`sim.w` / `Sim.W`) only.
- `rngState`: the RNG state printed with JS `String()` / `JsMath.Str` (it is a growing double, see `Core/Rng.cs`).
- `rngCalls`: calls of the sim's RNG so far (C# `Rng.Calls` is per instance; the TS side wraps `Rng.prototype.next`
  in the same bundle and counts per instance, so worldgen's own RNGs are not included on either side).
- `<index>` in `rng`: the call counter after the call (1-based, same scale as `rngCalls`).
- `<lastCheckpointLabel>`: the last checkpoint passed before the call (`start` before the first one of the day).
- `<callerFunction>`: informational, compare case-insensitively. TS: first named, non-builtin frame above
  `Rng.*` in `new Error().stack` (anonymous arrows → the enclosing named function; esbuild renames like
  `syncHeroes2` are mapped back). C#: first non-`Rng`, non-`System.*`, non-`J`/`Js*` frame of a `StackTrace`
  (lambdas → the containing method, local functions → their own name). The C# JIT inlines small methods in
  Release, so a C# caller can be the caller's caller; a TS `const f = () => …` ported as a C# lambda shows
  up under the containing method.

## Checkpoints

The C# `Sim.Step` / `Sim.CivAI` call `Sim.Cp(label)` (null in normal runs). `golden/ts/trace.ts` has `tracedStep()`, an
exact mirror of `sim.step()` + `civAI()` calling the same exported module functions, with the same checkpoints.
In day order:

```
econ:<civ> (each alive civ, after economyTick)   repair (day % 5)
ai:<civ>:research era builds recruit expand hero bids quest scout trade war raid   (civs whose AI runs that day)
ai:<civ>:extinct (instead, when the civ has no settlement left)
territory relations (day % 10)   discover world disasters (day % 30)   yearly (day % 120)
camps taverns inns   heroes (day % 5)   agents sea routes roads   end (after threat decay, history, w.rngState)
```

`hash` mode uses the real `sim.step()` on the TS side; `cps`/`dump`/`rng` use `tracedStep()`.
`node golden/out/trace.js verify 1-3 1800` checks that both give identical `JSON.stringify(sim.w)`, canonical hash,
RNG state/calls every day and cache sizes. `trace.js` also fingerprints the source of `Sim.step()`/`civAI()` and
warns on stderr when it changes (then update `tracedStep()`, the C# checkpoints and the fingerprint constant;
`GOLDEN_QUIET=1` silences it).

## Canonical form

Identical on both sides (TS: `canonHash`/`flatDump` in `trace.ts`; C#: `Canon` in `FD.Macro.Run/Program.cs`):

- object: keys sorted by ordinal UTF-16 order; properties whose value is null/undefined are skipped; keys starting
  with `_` are skipped (TS attaches `_def`, `_m`, … to some Battle objects); `{"k":v,…}` with keys quoted like strings;
- number: `NaN`, `Infinity`, `-Infinity`, −0 → `0`, otherwise JS `String(x)` (C#: `JsMath.Str`);
- string: `"` + escaped + `"` (`\` → `\\`, `"` → `\"`, chars < 0x20 → `\u00xx` lowercase hex, everything else literal,
  including lone surrogates); booleans `true`/`false`; arrays `[a,b,…]`, null/undefined elements → `null`;
- C# object graph: public instance fields, name = C# name with the first letter lowered unless `[JsonPropertyName]`,
  `[JsonIgnore]` skipped; `JsObj<T>`/`JsNumObj<T>` are objects, `List<T>`/`T[]` arrays, nullables null when empty,
  other numeric primitives (`long`, `float`, …) as the JS number they convert to. Any other type (Dictionary,
  HashSet, JsMap, enums, …) reachable from `World` fails at day 0 with a `NotSupportedException` naming the field;
  mark C#-only helper fields `[JsonIgnore]`;
- hash: FNV-1a 64 over the UTF-16 code units of the canonical text (offset `cbf29ce484222325`, prime
  `100000001b3`), computed while walking — the text itself is never built. TS keeps the two 32-bit halves as int32
  bit patterns (`Math.imul` + a 16-bit split for the carry) and writes objects with a small serializer generated
  once per own-key list (`new Function`: sorted constant property loads, pre-escaped `"key":` prefixes), which
  keeps V8's property access monomorphic; C# uses compiled field getters (expression trees) per type.
- flat listing (`dump`): the same scalars, one `path=value` per leaf in canonical order (object keys sorted, array
  indices ascending), e.g. `civs[3].stock.grain=12.5`, `tiles[1234].owner=-1`; empty objects/arrays are leaves
  (`path={}`, `path=[]`); a key containing a space, control char or one of `"`, `.`, `=`, `[`, `\`, `]` is written
  `["key"]`.

## Harness self-tests

```
node golden/out/trace.js selftest > /tmp/a; dotnet FD.Macro.Run/bin/Release/net8.0/FD.Macro.Run.dll selftest > /tmp/b; cmp /tmp/a /tmp/b
node golden/out/trace.js verify 1-3 1800
python3 golden/compare.py --seeds 1-20 --days 0          # day-0 state, both languages
python3 golden/compare.py --seeds 1 --days 20 --fault state@3:econ:2
```

`--fault <kind>@<day>[:<label>]` sets `GOLDEN_FAULT` for the C# runs only, which perturbs the C# side at that
checkpoint (day 0: right after `new Sim`) so every analysis path can be exercised while the port is exact:
`crash` (throws `NotImplementedException`), `rng` (one extra `Rng.Next()`), `state` (adds `metrics.goldenFault`),
`num` (`seed += 0.5`).

## Caveats — what the comparison cannot see

- Key order of JS records (`stock`, `pop`, `metrics`, …): the canonical form sorts keys, so a `JsObj` insertion-order
  bug is only caught once it changes behaviour (iteration order → RNG use, float sums, "first match" loops).
- `null`, `undefined` and an absent property are the same (all skipped); `-0` equals `0`; all NaNs are `NaN`;
  array holes, `undefined` and `null` elements are all `null`.
- Object identity/aliasing (e.g. the shared `war` object of `relations[a][b]` and `[b][a]`, cached path arrays) is
  flattened into a tree; a port that clones where TS shares shows up only when a later mutation makes it visible.
- `_`-prefixed properties are skipped (the `_def`/`_m` Combatant arrays left on raid battles are in
  `JSON.stringify(sim.w)` but not in the hash).
- State outside `sim.w`: the path/nav caches are compared by size only (per day, not per checkpoint); `shoreW` and
  other derived caches are not compared (a wrong cache shows up once it changes the world).
- The RNG calls made inside `new Sim(seed)` cannot be traced on the C# side (the hook is per instance); day 0 is
  compared by hash, rng state/calls and `dump`.
- The TS walker rejects values it cannot represent (Map, Set, typed arrays, class instances, functions, BigInt) with
  an error (`dump` names the path); none occur in `sim.w` today.

## Performance

Measured on the 2-core dev VM (Node 22, .NET 8), seed 1:

| | 1800 days | notes |
|---|---|---|
| TS `sim.step()` only | ≈ 12 s | `trace.js bench 1 1800` |
| TS `hash 1 1800` | ≈ 40 s | world hash ≈ 13 ms/day on average, 25–34 ms for the late-game 1.7 M-char world (≈ 2× `JSON.stringify`) |
| C# `hash 1 1800` | ≈ 15 s | of which hashing ≈ 9 s (≈ 5 ms/day) |

`compare.py --seeds 1,2,3 --days 7200` (TS and C# in parallel per seed, so TS sets the pace): ≈ 15 min in total,
TS ≈ 290–320 s and C# ≈ 100–115 s per seed. `--jobs` only pays off with more than two cores.
