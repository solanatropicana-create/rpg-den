# macro — durum (1 Ekim 2026)

`macro/`, Fantastik Dünya'nın Godot'dan bağımsız C# dünya simülasyonudur (`FD.Macro`, .NET 8). TS simülasyonunun birebir portu `port-exact` etiketinde durur; ana hatta bunun üstüne Faz 1 dalga A, dalga B, C1 ve C3 değişiklikleri vardır. Godot projesine henüz bağlı değil.

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
- **C3 · #8 Altın ve ambar** (Faz 1b; dünya harikası yarışı atıldı). Kod: `Modules/Economy.cs` (C3 bölümleri).
  - Boştaki işçi 0,004 altın/gün getirir (eskiden 0,02).
  - Bakım: asker 0,01/gün, L2/L3 yapı 0,004/0,01 altın + alet aşınması, kahraman maaşı (mevsimde bir, seviyeyle). Hazine bakımı ödeyemezse "hazine boş": firar, yeni asker yok, L2+ verim ×0,8. Maaşı iki mevsim ödenmeyen kahraman ayrılır.
  - Kamu işleri (imar): hazinenin yedeği aşan kısmı (yılda %80) ve ambarın 120 günü aşan fazlası boştakilerden amele tutar (kademe başına nüfusun %5/15/30/45'i). İmar büyüme, huzur ve onarım hızı verir. Altın birikimini asıl bu tutar.
  - Kent tüketimi (`TOWN_NEEDS`): kademeye göre bira, alet, ekmek payı. 10 gün yokluk büyümeyi yavaşlatır, 30 gün kroniğe düşer; ekmek/bira yokluğu huzursuzluk getirir.
  - Kıtlık tek büyük olay: 10 gün ve ort. %25 açıkla ilan edilir, bir yıl tok geçince biter. Komşular yardım eder (+ilişki) ya da yüz çevirir (−ilişki); aç medeniyet saldırgan olmasa da akına çıkabilir, akıncıları erzak da taşır.
  - Canavar baskını kentin nüfus payı kadar hazine alır (eskiden bütün hazinenin %30'u). Ejderha haracı `100 + 0,6 × nüfus` ile tavanlı; akın, ilan ve büyüme buna göre ölçeklendi.

**Son ölçüm** (`reports/f1b-2`, 16 dünya × 60 yıl, C3 dâhil): bitiş ölçütlerinin hepsi geçiyor. Seed 1, gün 7200 hash'i `6922b0fc1f0de1c2`. Önceki ölçüm `reports/c1-4` (C3'süz; hash `20c9085ff9cd062a`).

| # | Ölçüt | Port (başlangıç) | c1-4 | f1b-2 (C3) |
|---|---|---|---|---|
| 1 | Donma yok (41–60. yıl / 6–20. yıl büyük olay) | 0,46 ✗ | 0,97 ✓ | 0,90 ✓ |
| 2 | Çöküş olan dünya | 1/16 ✗ | 14/16 ✓ (146 çöküş) | 14/16 ✓ (98 çöküş) |
| 3 | Yaşayan kamp (41–60. yıl / 6–20. yıl) | 1,74 / 6,52 ✗ | 10,2 / 8,31 ✓ | 9,31 / 8,67 ✓ |
| 4a | Doğuş seviyesi (on yıllar) | 2,2 → 5,0 ✗ | 1,31–1,45 ✓ | 1,32–1,50 ✓ |
| 4b | Sv8+ olan dünya | 0/16 ✗ | 15/16 ✓ | 14/16 ✓ |
| 4c | Dünya başına efsane (medyan) | 13 ✗ | 3 ✓ | 4 ✓ |
| 4d | Ölen kahraman payı | %19 ✗ | %44 ✓ | %40 ✓ |
| 5a / 5b | Determinizm / kayıt-yükleme | ✓ / — | ✓ / ✓ | ✓ / ✓ |

C3 ekonomisi (f1b-2, c1-4'e karşı): medeniyet altın medyanı 20. yılda 325, 60. yılda 781 (2,4 kat; önce 2223 → 5585, p90 37 bin → 1,6 bin). Boştaki iş gücü 20. yıldan sonra %15–20 (önce %47–50). Kıtlık: 16 dünyada 17 ilan, 6 açlık ölümü (önce 244). Canavar baskınında giden altın dünya başına ~0,5 bin (önce ~43 bin); ejderha haracı ~17 bin (önce ~160 bin).

**Yarım kalanlar.** Yok. Eski WIP (`wip/faz1-c3-c4.patch`) Faz 1b'de ayrıldı: C3 ana hatta (yukarıda), dünya harikası yarışı ve C4'ün tamamı (dışlayan teknoloji grupları, IV. çağ maliyeti, Kadim Bilgi, yemin/bedel/çember, d20 Yabani Büyü) atıldı. Patch git geçmişinde duruyor.

**Başlanmayanlar**
- #4 kronik zinciri (olaylara `Parent` bağı).
- Godot bağlantısı (Faz 2: gözlemci harita).

## Açık hatalar ve riskler

### Ekonomi ve kurallar (ana hatta açık)
1. **Yatak yenilenmesi çok hızlı.** Yenilenme döngüsü her yaşayan medeniyetin ekonomi tikinde ayrı çalışıyor. Şifalı ot ve kadim ağaç 7–9 kat hızlı yenileniyor (`Economy`).
2. **Altın kapısı hep açık.** `Sim.Access(c, "gold")` stokta 2 altın olunca geçiyor. Bu yüzden para teknolojisi kapısı, darphane koşulu ve altın anlaşmazlıkları hiçbir şeyi engellemiyor.
3. **`cheapKnown` etkisinin büyüklüğü yok sayılıyor.** Ozanın Bilgi uç gücündeki ek +0,25 etkisiz.
4. ~~Canavar baskını fazla götürüyor.~~ Düzeldi (C3): kentin nüfus payı kadar alıyor.
5. ~~Ejderha haracı geç yıllarda çok büyük.~~ Düzeldi (C3): haraç nüfusla tavanlı; dünya başına ~17 bin.
6. ~~Altın birikiyor, gider yok.~~ Düzeldi (C3): bakım ve kamu işleri; 60. yıl medyanı ~0,8 bin.
7. **Araştırma ağacı ~20. yılda bitiyor;** sınıflar aynı sonla bitiyor. C4 (#6) atıldı; araştırma ve çağlar yol haritası v3'te kalkıyor.

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

### Altın ve ambar (C3'ten kalanlar)
23. **Geç yıllarda alet yokluğu yaygın.** 51–60. yıllarda köy+ yerleşimlerin ~%48'inde kent tüketiminde yokluk var, çoğu alet: bakır, kalay ve demir tükeniyor. Büyüme %5 yavaşlar; dünya başına ~57 "alet yok" kaydı (büyük olay değil).
24. **"Hazine boş" hiç tetiklenmedi** (16 × 60 yıl): altın günlük bakımın altına inmiyor. Firar ve L2+ verim cezası pratikte denenmedi. Bakım gelirin yanında küçük; asıl gider kamu işleri.
25. **Maaşsız kahramanlar ayrılıyor:** dünya başına ~14 (toplam 236). Yoksul medeniyetlerin kahraman maaşı ödenemiyor. Ölçüt 4b/4d biraz düştü (15→14/16, %44→%40).
26. **Kıtlık seyrek:** 16 dünyada 17 ilan (7 dünyada), 6 açlık ölümü. Çoğu ilk yardım turuyla hemen kapanıyor; aynı medeniyet 2–3 yılda bir yeniden aç kalabiliyor.
27. **Medeniyet yağması (`Agents.LootFrom`) hâlâ bütün hazinenin %20'sini alıyor**, kentin payını değil. C3'ün kapsamı dışında kaldı.
28. **Amele iş gücünün ~%22'si** (geç yıllar). Boştaki payı (%15–20) buna bağlı; amele altın ve gıda yakar, mal üretmez.
29. **Çöküş azaldı:** 146 → 98 (başkent kaybı 134 → 86), büyük olay oranı 0,97 → 0,90. Neden incelenmedi: küçük hazineler ya da imarın huzursuzluğu azaltması olabilir.
30. **Rapordaki "Altın birikiyor" satırı hâlâ "evet" diyor.** Kaba eşik: 30. yıl ≥ 10 × 1. yıl. Gerçek artış 20 → 60. yıl 2,4 kat.

## Devam için

1. #4 kronik zinciri.
2. Faz 2: Godot gözlemci harita. `FD.Macro`'yu Godot projesine bağla.
