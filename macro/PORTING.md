# TS → C# sim port: rules (Faz 1)

Goal: `macro/FD.Macro` is a **bit-exact** port of the TypeScript world simulation in `src/sim/*.ts` + `src/data/*.ts`.
The golden test runs both with the same seed and compares the whole world state **every day** (and at
checkpoints inside a day). Any difference — a number, a string, a key order, one extra RNG call — is a bug.
Fidelity beats elegance: port line by line, keep the TS structure, names, order of statements and
evaluation order. Do not "fix", simplify or optimise TS logic, even if it looks wrong. Keep TS comments
(Turkish is fine).

## Layout

| TS | C# |
|---|---|
| `src/sim/types.ts` | `FD.Macro/Core/Types.cs` (done) |
| `src/sim/sim.ts` | `FD.Macro/Core/Sim.cs` (done) |
| `src/sim/rng.ts`, `hex.ts`, `tr.ts` | `Core/Rng.cs`, `Core/Hex.cs`, `Core/Tr.cs` (done) |
| `src/data/*.ts` | `Data/D.cs` (static class `D`, loaded from `Data/data.json`), `Data/Defs.cs` (done) |
| `src/sim/<name>.ts` | `FD.Macro/Modules/<Name>.cs`: `public static class <Name>` |

Module classes: `WorldGen`, `Economy`, `Research`, `Gear`, `Events`, `Diplomacy`, `Monsters`, `Combat`,
`Agents`, `Heroes`, `Will`, `Inns`, `InnLife`, `Sea`. Namespace `FD.Macro` (file-scoped `namespace FD.Macro;`).
Nullable reference types are **disabled**, implicit usings are **disabled** (write `using System;`,
`using System.Collections.Generic;`, `using System.Linq;` yourself).

Build: `cd macro && dotnet build FD.Macro/FD.Macro.csproj -c Release 2>&1 | grep -E "error|Build succeeded"`
(offline; no NuGet packages allowed).

## Names

- Exported TS function `fooBar` → `public static <Ret> FooBar(...)` in the module class, **same parameter
  order**. Non-exported helpers → `private static` (or `internal static` if another module needs it).
- Sim methods: `s.log(...)` → `s.Log(...)`, `s.rng` → `s.Rng`, `s.w` → `s.W`, `s.g` → `s.G`,
  `s.e(c, 'x')` → `s.E(c, "x")`, `s.st(c, g)` → `s.St(c, g)`, `s.day` → `s.Day`, etc. Read `Core/Sim.cs`.
  `s.log(kind, text, { civ, tile, cause, battle, major })` → `s.Log(kind, text, civ: .., tile: .., cause: .., battle: .., major: ..)`.
- World fields: `w.W`/`w.H` (width/height) → `W.Width`/`W.Height`; everything else `w.fooBar` → `W.FooBar`.
- Data: `GOODS[g].base` → `D.GOODS[g].Base`, `TECH[id]` → `D.TECH[id]`, `techCost(t)` → `D.TechCost(t)`,
  `extPoss(k, l)` → `D.ExtPoss(k, l)`. `ek(x, 'da')` → `Tr.Ek(x, "da")`. `mod(score)` → `Rng.Mod(score)`.
  `heroClassTr(c)` → `Sim.HeroClassTr(c)`. Constants keep their TS names (`public const double MUSTER_CAMP = 16;`).
- TS interfaces declared inside a module (e.g. `Combatant`, `BattleOpts`, `UnitStats` in combat.ts,
  `NavOpts` in sea.ts) → top-level `public sealed class` in that module's file with PascalCase fields.
- Inline object return types (`{ good: Good; y: number }`) → named value tuples `(string Good, double Y)`,
  or a small class when `null`/`undefined` is a possible result (e.g. `{ path, hull } | null`).

## Types

- JS numbers are doubles. Use `int` only for ids, tile indices, array indices, `level`, `tier`, `era`,
  counts used as loop bounds/indices; **everything else `double`**. Parameters: tile/id/index → `int`,
  other numbers → `double`. Optional numeric params → `int? x = null` / `double? x = null`.
- **Integer division trap:** C# `int / int` truncates; JS never does. Whenever a TS `/` has int operands in
  C#, force double (`a / (double)b`). Same for `%` only if operands can be negative/non-integer.
- Never cast a double to int to silence the compiler unless TS applies `Math.floor`/`Math.round`/`|0`
  at that exact point (then write the same op: `(int)Math.Floor(x)`), or the value is provably integral
  (use `J.I(x)` which throws if not integral).
- TS optional `x?: T` ↔ nullable C# (`double?`, `int?`, `bool?`, reference null). `undefined` and `null`
  are both C# `null`. Keep "absent" vs "0/false" distinctions exactly as TS writes them (the golden test
  compares JSON-like state: an explicitly written `false`/`0` differs from an absent key).
- String unions (`'goblin' | 'hobgoblin'`) stay `string` with the same literals.
- `Record<string, number>` → `JsObj<double>`; `Record<number, number>` → `JsNumObj<double>`;
  other records → `JsObj<T>`. `Map` → `Dictionary` for pure lookups, `JsMap<K,V>` if iterated.
  `Set` → `HashSet` for membership only; if a Set is iterated, keep a `List` in insertion order.
- Arrays → `List<T>`; tuples like `[number, number, number]` → `List<double>` (state) or value tuples (locals).

## JS semantics cheat sheet (use `J`, `JsMath`, `JsSort` — `FD.Macro/Js/`)

| TS | C# |
|---|---|
| `if (x)` number / optional / string | `J.T(x)` (0, NaN, undefined, "" are falsy) |
| `if (obj)` | `obj != null` |
| `a \|\| b` (numbers/strings) | `J.Or(a, b)` |
| `a ?? b` | `a ?? b` (nullable) |
| `o[k] ?? 0` on a number record | `o.Get(k) ?? 0` |
| `o[k] = v`, `delete o[k]`, `k in o` | `o.Set(k, v)`, `o.Delete(k)`, `o.Has(k)` |
| `for (const k in o)` / `Object.keys(o)` / `Object.entries(o)` | `foreach (var kv in o)` / `o.Keys()` / `o.Entries()` |
| `Math.round` | `JsMath.Round` (**not** `Math.Round`: banker's rounding) |
| `Math.floor/ceil/abs/sqrt` | `Math.Floor/Ceiling/Abs/Sqrt` (on doubles) |
| `Math.min/max` (2 args) | `JsMath.Min/Max` (NaN rules) — `Math.Min/Max` is fine for ints |
| `Math.max(...xs)` | `JsMath.Max(params)`, `J.MaxOf(list, f)` (empty → -Infinity) |
| `Math.exp/log/pow`, `x ** y` | `JsMath.Exp/Log/Pow` (never `System.Math`); `x ** 2` → `x * x` |
| `` `${n}` `` for a number | `J.S(n)` (JS number formatting); ints may use `{i}` |
| `n.toFixed(d)` | `JsMath.ToFixed(n, d)` |
| `s.toLocaleLowerCase('tr')` / upper | `J.TrLower(s)` / `J.TrUpper(s)` |
| `arr.sort(cmp)` | `J.Sort(list, cmp)` / `JsSort.Sort` — **never** `List.Sort`/`OrderBy` (V8 TimSort is replicated exactly, incl. NaN comparator results) |
| `arr.slice().sort(cmp)` | `J.Sorted(list, cmp)` |
| `arr[i]` possibly out of range | `J.At(list, i)` (default when out of range) |
| `arr.find(f)` | `J.Find(list, f)` (default when missing — for `List<int>`/`List<double>` that is 0, so use `J.FindIndex` when "not found" matters) |
| `filter/map/some/every/reduce/findIndex/slice/splice/shift/pop` | `J.Filter/Map/Some/Every/Reduce/FindIndex/Slice/Splice/Shift/Pop` |
| `arr.includes(x)` / `indexOf` | `list.Contains(x)` / `list.IndexOf(x)` |
| `rng.pick(arr)` | `s.Rng.Pick(list)` (consumes one number even for an empty list, returns default) |
| `x!` | just `x` |

**RNG order is sacred.** The same RNG calls must happen in the same order with the same arguments.
Watch short-circuits (`a && s.rng.chance(p)` → `a && s.Rng.Chance(p)`, same operand order), conditional
expressions, `Math.max(rng(), rng())` argument order, and loops. Never put RNG calls inside lazy LINQ.

**Loops over mutated arrays.** JS `for (const x of arr)` re-reads `arr.length` every step (items pushed
during the loop are visited; removals shift). Port such loops as `for (int i = 0; i < list.Count; i++)`.
`foreach` is fine only when the body never mutates the list (C# throws otherwise).
`arr.forEach` / `for...of` over a `filter(...)` result iterates a snapshot.

**Object identity.** Keep sharing exactly like TS: if TS stores the same object/array in two places, so
do you (e.g. `Sim.Path` returns the cached list instance; a mutated path mutates the cache — replicate).
Clone only where TS clones (`{ ...x }`, `.slice()`, `[...arr]`).

**Strings.** Event/log/journal texts are compared too: port every template literal exactly (spaces,
punctuation, Turkish letters, `ek()` suffixes, number formatting via `J.S`).

**Hidden state.** No static mutable fields in modules. All state lives in `World` (and Sim's path caches).

## Golden test

`macro/golden/` runs TS and C# side by side (per-day state hashes, then per-checkpoint diffs and RNG
traces for the first differing day). See `macro/golden/README.md`.
