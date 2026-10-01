# macro — durum (1 Ekim 2026)

`macro/`, Fantastik Dünya'nın Godot'dan bağımsız C# dünya simülasyonudur (`FD.Macro`, .NET 8). TS simülasyonunun birebir portu `port-exact` etiketinde durur; ana hatta bunun üstüne Faz 1 dalga A, dalga B ve C1 değişiklikleri vardır. Godot projesine henüz bağlı değil.

## Altın test (TS ↔ C#)

**Geçerli olduğu yer:** yalnız `port-exact` etiketi.
- TS kaynağı: `src/sim` + `src/data`, 30 Eylül 13:46 sürümü (içerik hash'i `a7767adc…`). 1 Ekim 03:40'ta kullanıcı klasöründe değişmemişti.

**Kapsam: 3 seed, 7200 gün, her gün birebir.**
- Seed 1, 2 ve 3, 7200 gün (60 oyun yılı) koşuldu.
- Her gün karşılaştırılanlar:
  - dünya durumunun kanonik hash'i (FNV-1a 64);
  - RNG durumu ve RNG çağrı sayısı;
  - yol ve deniz önbelleklerinin boyutu.
- İlk fark yok. 60. yılın sonuna kadar her gün eşleşiyor.

**Modüller:** 14 modülün hepsi ve Sim çekirdeği (`Step`/`CivAI`) eşleşiyor.
- Modüller: WorldGen, Economy, Research, Gear, Events, Diplomacy, Monsters, Combat, Agents, Heroes, Will, Inns, InnLife, Sea.
- Gün içi kontrol noktaları yoğun günlerde de eşleşti: `econ:N`, `repair`, `ai:N:*`, `territory`, `relations`, `discover`, `world`, `disasters`, `yearly`, `camps`, `taverns`, `inns`, `heroes`, `agents`, `sea`, `routes`, `roads`, `end`.

**RNG izi ve döküm**
- RNG izi: seed 1'in 275, 575, 922, 1183, 1660 ve 1753. günlerinde çağrı çağrı aynı (günde 1073 çağrıya kadar).
- `dump`: gün 0 (seed 1–3) ve gün 1800 (seed 1) bayt bayt aynı.

**Modül bazında ek denetimler, hepsi 0 fark**

| Denetim | Kapsam |
|---|---|
| WorldGen | 517 seed |
| Combat.ResolveBattle | binlerce rastgele savaş |
| Sea | ~150 bin navPath sorgusu; seed 1 (3600 gün), seed 2, 3, 4, 9 (4000'er gün) tam koşu |
| Heroes / Will / Inns, InnLife | 3000–6000 günlük koşular |
| JS anlamları katmanı | exp/log/pow, Math.round, sayı yazımı, V8 TimSort; milyonlarca vektör |

**Koşturma:** `golden/README.md` → `python3 golden/compare.py --seeds 1,2,3 --days 7200` (yaklaşık 15 dk).

**Ana hat:** dalga A'dan itibaren kurallar bilerek değişti. TS ile karşılaştırma artık anlamlı değil; ilk ayrılma günü ölçülmedi. Doğuş seviyesi kuralı değiştiği için ilk günlerde beklenir.

## Faz 1 durumu (ana hat)

**Bitenler**
- **A1 · Ölçüm aracı:** `FD.Macro.Run stats --seeds 1-16 --years 60 --out reports/<ad>`, 2 çekirdekte 5–8 dk. Bitiş ölçütlerini, determinizmi (5a) ve kayıt/yüklemeyi (5b) denetler.
- **A2 · Kayıt/yükleme:** `Sim.Save` / `Sim.Load` (`Core/Save.cs`).
  - Kaydet → yükle → devam, kesintisiz koşuyla gün gün aynı.
  - Doğrulama: `tests/SaveCheck` matrisi, seed 1–4, K ∈ {0, 1, 37, 500, 1234, 2400, 3599} → 3600.
- **A3a · Kahraman kimliği ve ilerlemesi:**
  - Sv1–2 doğar, Sv10'a kadar çıkar; yaşlanır ve ölür.
  - Efsanelik ünle (renown) gelir.
  - Adlar tekil; kilometre taşları tutulur; ölünce destan yazılır.
  - Kod: `Modules/Lore.cs`.
- **A3b · Temizlik ve kalıcı kronik:**
  - `World.Chronicle`'a eklenenler: çalınan teknoloji düzeltmesi, prodWood/harvest/teleport, darphane geliri.
- **B1 · Yükseliş ve çöküş:**
  - Yerleşim tavanı `2 + çağ + nüfus/200`.
  - Başkent düşebilir.
  - Bölünme ile yeni medeniyet doğar.
  - Kutsal sefer, savunma paktı, ihanet ve tarihî hak var.
- **B2 · Anlatıcı ve geç tehdit:**
  - Hedef kamp sayısı `3 + yıl/6`.
  - Gerilim bütçesi: kriz ve rahatlama dönemleri.
  - Troller 15. yıldan sonra çıkar.
  - Ejderha 18–22. yıllar arasında uyanır: haraç ister, ilan ve ittifak doğurur.
  - Kod: `Modules/Storyteller.cs`, `Modules/Dragon.cs`.
- **C1 · Kahraman ince ayarı:** efsaneler kamp temizlemekten değil, kişisel işlerden gelir.

**Son ölçüm** (`reports/c1-4`, 16 dünya × 60 yıl): bitiş ölçütlerinin hepsi geçiyor. Ana hat bu raporu birebir üretiyor: seed 1, gün 7200 hash'i `20c9085ff9cd062a` (1 Ekim'de yeniden koşuldu).

| # | Ölçüt | Port (başlangıç) | Ana hat |
|---|---|---|---|
| 1 | Donma yok (41–60. yıl / 6–20. yıl büyük olay) | 0,46 ✗ | 0,97 ✓ |
| 2 | Çöküş olan dünya | 1/16 ✗ | 14/16 ✓ |
| 3 | Yaşayan kamp (41–60. yıl / 6–20. yıl) | 1,74 / 6,52 ✗ | 10,2 / 8,31 ✓ |
| 4a | Doğuş seviyesi (on yıllar) | 2,2 → 5,0 ✗ | 1,31–1,45 ✓ |
| 4b | Sv8+ olan dünya | 0/16 ✗ | 15/16 ✓ |
| 4c | Dünya başına efsane (medyan) | 13 ✗ | 3 ✓ |
| 4d | Ölen kahraman payı | %19 ✗ | %44 ✓ |
| 5a / 5b | Determinizm / kayıt-yükleme | ✓ / — | ✓ / ✓ |

**Yarım kalanlar.** Kullanıcı durdurduğunda iki ajan işin ortasındaydı. Değişiklikleri ana hatta değil, ayrı tutuluyor:
- bulut kopyasında `wip/faz1-c3-c4` dalında;
- burada `wip/faz1-c3-c4.patch` olarak: `git apply wip/faz1-c3-c4.patch` ile ana hatta temiz uygulanır.

İçerik:
- **C3 · #8 Altın ve ambar:**
  - boştaki işçi geliri düşer;
  - bakım giderleri, kasaba tüketimi, dünya harikası yarışı;
  - kıtlık tek büyük olay olur, komşu yardımı gelir;
  - baskın yağması kasabanın payıyla sınırlanır.
- **C4 · #6 Sınıflar farklı bitsin:**
  - III. ve IV. çağda birbirini dışlayan düğüm grupları;
  - IV. çağın maliyeti artar;
  - "Kadim Bilgi" düğümleri tekrarlanabilir;
  - paladin yemini, warlock bedeli, druid çemberi, d20 Yabani Büyü tablosu.

WIP'in son tam ölçümü (`reports/c3-3`, yalnız WIP dalında):
- Ölçütlerin hepsi geçiyor, ama ikisi sınırda: çöküş 12/16 dünya, efsane medyanı 6.
- C4'ün son koşusu yarım kaldı. WIP derleniyor ama gözden geçirilmedi ve doğrulanmadı.

**Başlanmayanlar**
- #4 kronik zinciri (olaylara `Parent` bağı).
- Godot bağlantısı (Faz 2: gözlemci harita).

## Açık hatalar ve riskler

### Ekonomi ve kurallar (ana hatta açık)
1. **Yatak yenilenmesi çok hızlı.** Yenilenme döngüsü her yaşayan medeniyetin ekonomi tikinde ayrı çalışıyor. Şifalı ot ve kadim ağaç 7–9 kat hızlı yenileniyor (`Economy`).
2. **Altın kapısı hep açık.** `Sim.Access(c, "gold")` stokta 2 altın olunca geçiyor. Bu yüzden para teknolojisi kapısı, darphane koşulu ve altın anlaşmazlıkları hiçbir şeyi engellemiyor.
3. **`cheapKnown` etkisinin büyüklüğü yok sayılıyor.** Ozanın Bilgi uç gücündeki ek +0,25 etkisiz.
4. **Canavar baskını fazla götürüyor.** Bütün hazinenin %30'unu ve tahılın %25'ini alıyor; geç yıllarda tek baskın ~15 bin altın (`Agents.RaidSettlement`). C3 WIP'te düzeltildi.
5. **Ejderha haracı geç yıllarda çok büyük:** dünya başına medyan ~147 bin altın (`Dragon.cs`).
6. **Altın birikiyor, gider yok.** Medeniyet altın medyanı 60. yılda ~35 bin. Bu #8'in işi.
7. **Araştırma ağacı ~20. yılda bitiyor;** sınıflar aynı sonla bitiyor. Bu #6'nın işi.

### Denge ve tasarım
8. **Bazı dünyalar hep barışçıl kalıyor.** Savaş olmayınca çöküş de olmuyor.
9. **"Kötü" medeniyetler başkentini defalarca kaybedebiliyor:** en kötü durumda 7 kez.
10. **Bölünme sayısı eşiğe çok duyarlı.** Eşik 3,5'ten 3,25'e inince bölünme 6'dan 16'ya çıkıyor.
11. **B1, eski davranışları değiştirdi;** tasarımla teyit edilmeli:
    - savaş sırasında göçmen gönderilmiyor;
    - kurucu yeniden doğuş yalnız kendi kendine yok olan medeniyete;
    - başkente yürüyüş 1,5× güç istiyor.
12. **Ejderhaların çoğunu adsız bir asker deviriyor.** Ün en yüksek seviyeli sağ kahramana yazılıyor; bu geçici bir çözüm.
13. **Efsane sayısı gürültülü:** benzer ayarlarla medyan 2–4 arasında oynuyor.
14. **`harvest` ve `teleport` etkileri pratikte nadiren devreye giriyor.**

### Kod ve araçlar
15. **`Sim.Extinct` sözleşmesiz kiralık kahramanları "gone" yapıyor.**
16. **`Tr.Ek` sayıyla biten adlara yanlış ek veriyor.** `Lore.Ek` bunu sarıyor.
17. **Suikast destanı dolaylı yazılıyor:** doğrudan çağrıyla değil, günlük taramayla (`Lore.LateDeath`).
18. **İstatistikte ölüm nedeni savaş başlığından çıkarılıyor;** bir kısmı "Bilinmiyor" kalıyor.
19. **`World.Chronicle` hiç kırpılmıyor.** Kayıt dosyası zamanla büyür; gün 7200'de gzip ile ~0,3 MB.
20. **Kayıt biçimi katı.** Alan adı değişince eski kayıt açılmaz. Birebir devam yalnız aynı kodla garanti.
21. **Kayıtta sözlükler kayıtlı sırayla yeniden kuruluyor** (Dictionary/HashSet). İleride ara silme ve yineleme yapan bir sözlük eklenirse sıra değişebilir.
22. **TS karşılaştırma testleri yalnız `port-exact` üzerinde geçerli.**
    - Ana hatta derlenmeyenler: `tests/CombatCheck`, `tests/EconCheck`, `tests/InnLifeCheck`.
    - Derlenen ama TS'den fark raporlayanlar: `HeroesCheck`, `SeaCheck`, `WorldGenCheck`, `CombatModCheck`, `CleanupCheck`.
    - Ana hatta derlenip geçenler: `JsCheck`, `SaveCheck`, `HeroStats`, `FD.Macro`, `FD.Macro.Run`.

## Devam için

1. WIP'i (`wip/faz1-c3-c4.patch`) incele, tamamla ve ölç; ya da at.
2. #4 kronik zinciri.
3. Faz 2: Godot gözlemci harita. `FD.Macro`'yu Godot projesine bağla.
