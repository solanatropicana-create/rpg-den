# FD.Macro: Fantastik Dünya makro simülasyonu (C#)

Kendi kendine işleyen D&D dünyası: medeniyetler, ekonomi, diplomasi, savaşlar, canavar kampları, kahramanlar, hanlar, deniz.
TypeScript simülasyonunun (`../src/sim`) birebir portudur (git etiketi `port-exact`, golden test: 3 seed × 7200 gün).
Faz 1'den beri C# kendi yolunda ilerler; değişiklikler `DESIGN-FAZ1.md`'de, deterministik yazım kuralları `PORTING.md`'de.

## Klasörler

| Klasör | İçerik |
|---|---|
| `FD.Macro/` | Simülasyon kütüphanesi (.NET 8, NuGet yok). `Core/`: `Sim`, `World` türleri, RNG, hex, kayıt/yükleme, `Stats.cs` (ölçüm toplayıcı). `Modules/`: TS dosyası başına bir statik sınıf. `Data/`: oyun verisi (`data.json`, gömülü). `Js/`: JS anlamları (sayı biçimi, sıralama, matematik). |
| `FD.Macro.Run/` | Komut satırı: golden test koşucusu (`hash`, `cps`, `dump`, `rng`, `selftest`, `bench`) ve ölçüm aracı (`stats`). |
| `golden/` | TS ↔ C# golden test (`compare.py`), bkz. `golden/README.md`. |
| `tests/` | Modül ve JS-anlam denetimleri, bkz. `tests/README.md`. |
| `reports/` | Ölçüm raporları (`report.md`, `report.json`). Dünya başına JSON'lar `reports/runs/` altında (git'e girmez). |

## Derleme

```
cd macro
dotnet build FD.Macro.Run/FD.Macro.Run.csproj -c Release 2>&1 | grep -E "error|Build succeeded"
dotnet FD.Macro.Run/bin/Release/net8.0/FD.Macro.Run.dll <mod> ...
```

## Golden test

`python3 golden/compare.py --seeds 1,2,3 --days 7200`: TS ve C# aynı seed ile koşar, dünya durumu her gün karşılaştırılır.
`port-exact` etiketindeki kod için geçerlidir; Faz 1 değişikliklerinden sonra TS'ye karşı fark beklenir. Ayrıntı: `golden/README.md`.

## Ölçüm aracı (`stats`)

```
dotnet FD.Macro.Run/bin/Release/net8.0/FD.Macro.Run.dll stats --seeds 1-16 --years 60 --out reports/<ad> --runs reports/runs/<ad> [--jobs N] [--label <metin>] [--verify N] [--saveload N]
```

| Seçenek | Varsayılan | |
|---|---|---|
| `--seeds` | `1-16` | seed listesi ve aralıkları (`1,3,5-8`) |
| `--years` | `60` | dünya başına yıl (1 yıl = 120 gün) |
| `--out` | (gerekli) | `report.md` ve `report.json` klasörü |
| `--runs` | `<out>/worlds` | dünya başına `seed-N.json` klasörü; `reports/runs/<ad>` git'e girmez |
| `--jobs` | çekirdek sayısı | paralel dünya (iş parçacığı) |
| `--label` | `--out` klasörünün adı | rapor başlığı |
| `--verify` | `1` | ilk N seed toplayıcısız yeniden koşulur; her yıl sonu hash'i karşılaştırılır (determinizm + toplayıcı salt okunur) |
| `--saveload` | `1` | `Sim.Save`/`Sim.Load` varsa ilk N seed yıl ortasında kaydedilip yüklenir, sonraki hash'ler karşılaştırılır |

Çıktı:
- `report.md`: bitiş ölçütleri tablosu (✓/✗, ölçülen değerler ve tanımlar), eski analizdeki sorunların durumu, on yıllık özet,
  kahraman seviye dağılımları, olay/muharebe/ölüm nedeni türleri, dünya tablosu, her ölçü için yıllık medyan (p10–p90);
- `report.json`: aynı veriler (yıllık medyan/p10/p90 dizileri, on yıllık değerler, ölçütler);
- `seed-N.json`: dünyanın yıllık değerleri (her ölçü bir dizi), anahtarlı sayımlar (olay türleri, seviye dağılımları, `W.Metrics`
  farkları), çöküş listesi, medeniyet özetleri, efsaneler, yıl sonu hash'leri.

Çıkış kodu 0; bir dünya çökerse 3 (raporlar yine yazılır). Dünyalar aynı süreçte paralel koşar; simülasyonda statik değişken
durum olmamalıdır (`--verify` bunu da yakalar).

Toplayıcıyı kendi kodundan kullanmak için (`FD.Macro/Core/Stats.cs`):

```csharp
var sim = new Sim(seed);
var st = new WorldStats(sim);      // Sim.OnEvent'e bağlanır, önceki işleyiciyi zincirler
for (int d = 1; d <= 7200; d++) { sim.Step(); st.AfterStep(); }
st.Finish();                       // st.Years[y - 1]["majorEvents"], st.CollapseLog, st.ToJson()
```

Yeni bir ölçü: `WorldStats.Defs`'e tanım ekle, `AfterStep`/`CloseYear` içinde doldur; rapor ve JSON kendiliğinden içerir.
Bitiş ölçütleri `FD.Macro.Run/Program.cs` → `StatsMode.Report.Evaluate`.

Temel ölçüm (değişmemiş port, `port-exact`): `reports/baseline-port-exact/`. Yeniden üretmek için:

```
git worktree add ../macro-base port-exact
# Stats.cs ve Program.cs'i worktree'ye kopyala, orada derle, sonra:
dotnet FD.Macro.Run/bin/Release/net8.0/FD.Macro.Run.dll stats --seeds 1-16 --years 60 --jobs 2 \
  --out ../macro/reports/baseline-port-exact --runs ../macro/reports/runs/baseline-port-exact --label "port-exact (temel ölçüm)"
```

## Kayıt/yükleme

Kaydet → yükle → devam et, kesintisiz koşuyla gün gün aynıdır (`DESIGN-FAZ1.md` A2). Kod: `FD.Macro/Core/Save.cs`.

```csharp
sim.Save("dunya.sav");                  // gzip'li JSON; önce dunya.sav.tmp yazılır, sonra yeniden adlandırılır
var sim2 = Sim.Load("dunya.sav");       // kaldığı yerden birebir devam eder
sim.Save(stream);                       // akış sürümleri: Save(Stream, compress = true), Sim.Load(Stream) (gzip ya da düz JSON)
```

- `Save`'i iki `Step` arasında çağırın, bir `Cp`/`OnEvent` kancasının içinden değil. `Save` simülasyonda hiçbir şeyi değiştirmez.
- Kancalar kaydedilmez: `OnEvent`, `Cp` ve `Rng.Trace` yüklemeden sonra yeniden bağlanır (ör. `new WorldStats(sim2)`).

**Dosyada ne var.** `{"format":"fd-macro-save","version":1,"day":…,"seed":…,"state":{…}}`. `state` (`SaveState`) şunları tutar:
`World`, RNG durumu ve `Rng.Calls`, kara yol önbelleği, deniz yol önbelleği (`NavCache`) ve `ShoreW`. Önbellekler sonucu etkiler
(bayat girdiler bilerek yeniden kullanılır, boyut sınırında temizlenir), o yüzden onlar da kaydedilir. RNG durumu ayrı saklanır:
`new Sim(seed)`'ten hemen sonra `World.RngState` henüz dünya üretiminin durumunu tutar. İlk `Step`'ten sonra ikisi hep eşittir.

**Serileştirici** geneldir ve yansıma kullanır: public alanlar ve public get/set özellikleri, C# adlarıyla. `Types.cs`'e eklenen
alan kendiliğinden kaydedilir.
- **Nesne paylaşımı korunur.** Birden çok yerden gösterilen nesne ilk geçtiği yerde `"$id": n` alır; sonraki her geçişi
  `{"$ref": n}` olur (liste ve dizi: `{"$id": n, "$values": […]}`). Örnekler: iki yöndeki `Relation`'ın aynı `War`'ı; rota, ajan ve
  önbellekteki aynı yol listesi; `Events` ile `Chronicle`'daki aynı `GameEvent`; handaki misafir ile yolcu ajanı. Döngüler de çalışır.
- **Sayılar bit bit aynı döner.** Sonlu double'lar en kısa geri dönüşlü sayı olarak yazılır. `-0`, `NaN`, `Infinity` ve
  `-Infinity` dizgi olur (varsayılan dışı NaN yükü `"NaN:<hex>"`). 2^53'ü aşan RNG durumu da aynen döner.
- **Anahtar sırası korunur.** `JsObj`/`JsNumObj` JS anahtar sırasıyla yazılır ve aynı sırayla eklenerek kurulur; silinip yeniden
  eklenen anahtar sonda kalır. `$` ile başlayan anahtar bir `$` daha alır. `Dictionary`, `HashSet` ve `JsMap` numaralama sırasıyla.
- Kurucu varsayılanına eşit üyeler (null'lar da) yazılmaz. Yüklemede her nesne parametresiz kurucusuyla yaratılır.
- **Desteklenmeyen biçim `Save`'de hata verir**, durum sessizce kaybolmaz: `object`, arayüz ya da soyut tipte üye, delegate,
  struct, çok biçimli değer, public olmayan örnek alanı (gizli durum). Gerçekten geçici bir public üye `[NoSave]` ile işaretlenir.
- **Yükleme katıdır.** Bilinmeyen üye adı hatadır. Dosyada olmayan üye kurucu varsayılanını alır, böylece sonradan eklenen alan
  eski kayıtları bozmaz. Birebir devam yalnız aynı kodla güvencededir.

**Test:** `tests/SaveCheck` (FD.Macro kaynaklarını doğrudan derler).

```
dotnet build tests/SaveCheck -c Release
dotnet tests/SaveCheck/bin/Release/net8.0/SaveCheck.dll selftest            # birim denetimleri
dotnet tests/SaveCheck/bin/Release/net8.0/SaveCheck.dll matrix              # seed 1–4 × 3600 gün, K ∈ {0,1,37,500,1234,2400,3599}
dotnet tests/SaveCheck/bin/Release/net8.0/SaveCheck.dll negative            # paylaşımı bozan kayıt yakalanıyor mu
dotnet tests/SaveCheck/bin/Release/net8.0/SaveCheck.dll bench --days 7200   # boyut ve süre
```

- `matrix`: her seed için bir süreç kesintisiz koşar ve her günün özetini yazar. Her K'da kaydeder; aynı süreçte yükleyip iki
  nesne grafını karşılaştırır (değerler ve paylaşım). Sonra her (seed, K) için **ayrı bir süreç** dosyayı yükler, N'ye kadar koşar
  ve her günü karşılaştırır. K = 2400'de kayıt bellek akışından aynı süreçte de yüklenir.
- Günlük özet iki türlüdür. Her gün 128 bitlik bir parmak izi alınır: tüm alanları, anahtar sırasını ve nesne kimliği yapısını
  kapsar. Ayrıca her 100 günde, her K'da ve N'de `Json.Serialize(world)`'ün SHA-256'sı alınır.
- Ek denetimler: `Sim`'de bilinenlerin dışında alan yok, modüllerde değişken statik alan yok, statik veri koşu boyunca değişmiyor,
  dünya nesneleri statik veriyle paylaşılmıyor.
- `negative`: aynı gün iki bozuk yolla yüklenir: paylaşımsız yazım (`preserveReferences: false`) ve düz `Json.Serialize`/
  `Deserialize`. İkisi de yüklemede yakalanır. Savaş sürerken değerler de birkaç gün içinde ayrışır.

Sonuçlar (2026-10-01, paylaşımlı 2 çekirdekli makine):
- `selftest` 71/71. `matrix` geçti: 28 süreçler arası ve 4 süreç içi devam, K'dan 3600'e her gün aynı (859 sn).
- `negative`: seed 1, gün 2070. Bozuk yüklemelerde 745 `GameEvent`, 44 yol listesi ve 1 `War` paylaşımı kayboldu; değerler
  5 gün sonra ayrıştı. Doğru yükleme 1531 gün boyunca aynı kaldı.
- Gün 7200 (seed 1–3): 0,28–0,35 MB gzip (2,0–2,6 MB JSON, 26–36 bin nesne). `Save` ≈ 90–200 ms, `Load` ≈ 80–125 ms.
