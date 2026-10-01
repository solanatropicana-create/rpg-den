# JS-semantics golden checks

From `macro/` (needs Node 22 on x64 and the .NET 8 SDK):

    node tests/jsvectors/gen.mjs && dotnet run -c Release --project tests/JsCheck

`gen.mjs` writes about 200 MB of V8 reference vectors to `tests/jsvectors/data/` (git-ignored, regenerated on each run). `JsCheck` compares `FD.Macro.JsMath` / `JsSort` against them bit for bit and exits non-zero on any mismatch.

Optional: `node --allow-natives-syntax tests/jsvectors/tiers.mjs` checks that V8's execution tiers (Ignition, Sparkplug, TurboFan with constant folding) all return the same values.

## TS karşılaştırma testleri (yalnız `port-exact`)

`CombatCheck`, `CombatModCheck`, `EconCheck`, `InnLifeCheck`, `HeroesCheck`, `SeaCheck`, `WorldGenCheck`, `CleanupCheck` ve `golden/compare.py`, portun TS ile birebir olduğunu kanıtlamak için yazıldı. Faz 1 dalga A'dan sonra C# kuralları bilerek değişti; bu testler ana hatta derlenmeyebilir ya da fark raporlar. Çalıştırmak için:

    git worktree add ../macro-port port-exact

Ana hatta geçerli denetimler: `JsCheck` (JS anlamları), `SaveCheck` (kayıt/yükleme), `HeroStats` ve `FD.Macro.Run stats` (16 dünya × 60 yıl ölçüm panosu; determinizm ve kayıt/yüklemeyi de denetler).
