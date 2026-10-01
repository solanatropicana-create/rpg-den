# JS-semantics golden checks

From `macro/` (needs Node 22 on x64 and the .NET 8 SDK):

    node tests/jsvectors/gen.mjs && dotnet run -c Release --project tests/JsCheck

`gen.mjs` writes about 200 MB of V8 reference vectors to `tests/jsvectors/data/` (git-ignored, regenerated on each run). `JsCheck` compares `FD.Macro.JsMath` / `JsSort` against them bit for bit and exits non-zero on any mismatch.

Optional: `node --allow-natives-syntax tests/jsvectors/tiers.mjs` checks that V8's execution tiers (Ignition, Sparkplug, TurboFan with constant folding) all return the same values.
