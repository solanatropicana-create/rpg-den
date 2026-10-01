# macro — durum (1 Ekim 2026, Faz 1b-4)

`macro/`, Fantastik Dünya'nın Godot'dan bağımsız C# dünya simülasyonudur (`FD.Macro`, .NET 8). TS simülasyonunun birebir portu `port-exact` etiketinde durur; ana hatta bunun üstüne Faz 1 dalga A, dalga B, C1, C3, Faz 1b-3 (4X budaması, yol haritası v3) ve Faz 1b-4 (dünyanın ayarı: kamp bandı, büyük şehir, ejderha; v3 ölçüleri) değişiklikleri vardır. Godot projesine henüz bağlı değil.

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
- Modüller: WorldGen, Economy, Research, Gear, Events, Diplomacy, Monsters, Combat, Agents, Heroes, Will, Inns, InnLife, Sea (Research ana hatta Faz 1b-3'te silindi).
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
  - Yerleşim tavanı `3 + başkentin kademesi + nüfus/200` (Faz 1b-3; eskiden `2 + çağ`, aynı değerler).
  - Başkent düşebilir.
  - Bölünme ile yeni medeniyet doğar.
  - Kutsal sefer, savunma paktı, ihanet ve tarihî hak var.
- **B2 · Anlatıcı ve geç tehdit:**
  - Hedef kamp sayısı `3 + yıl/6` (Faz 1b-4: sabit bant, aşağıda).
  - Gerilim bütçesi: kriz ve rahatlama dönemleri.
  - Troller 15. yıldan sonra çıkar.
  - Ejderha 18–22. yıllar arasında uyanır: haraç ister, ilan ve ittifak doğurur.
  - Kod: `Modules/Storyteller.cs`, `Modules/Dragon.cs`.
- **C1 · Kahraman ince ayarı:** efsaneler kamp temizlemekten değil, kişisel işlerden gelir.
- **C3 · #8 Altın ve ambar** (Faz 1b; dünya harikası yarışı atıldı). Kod: `Modules/Economy.cs` (C3 bölümleri).
  - Boştaki işçi 0,004 altın/gün getirir (eskiden 0,02).
  - Bakım: asker 0,01/gün, L2/L3 yapı 0,004/0,01 altın + alet aşınması, kahraman maaşı (30 günde bir, seviyeyle). Hazine bakımı ödeyemezse "hazine boş": firar, yeni asker yok, L2+ verim ×0,8. Maaşı iki kez ödenmeyen kahraman ayrılır.
  - Kamu işleri (imar): hazinenin yedeği aşan kısmı (yılda %80) ve ambarın 120 günü aşan fazlası boştakilerden amele tutar (kademe başına nüfusun %5/15/30/45'i). İmar büyüme, huzur ve onarım hızı verir. Altın birikimini asıl bu tutar.
  - Kent tüketimi (`TOWN_NEEDS`): kademeye göre bira, alet, ekmek payı. 10 gün yokluk büyümeyi yavaşlatır, 30 gün kroniğe düşer; ekmek/bira yokluğu huzursuzluk getirir.
  - Kıtlık tek büyük olay: 10 gün ve ort. %25 açıkla ilan edilir, bir yıl tok geçince biter. Komşular yardım eder (+ilişki) ya da yüz çevirir (−ilişki); aç medeniyet saldırgan olmasa da akına çıkabilir, akıncıları erzak da taşır.
  - Canavar baskını kentin nüfus payı kadar hazine alır (eskiden bütün hazinenin %30'u). Ejderha haracı `100 + 0,6 × nüfus` ile tavanlı; akın, ilan ve büyüme buna göre ölçeklendi.

- **Faz 1b-3 · 4X budaması ve kademe** (yol haritası v3: dünya büyüyerek değil, durum değiştirerek yaşar). Kod: `Core/Tiers.cs` (kapılar, `Sim.TierOf`, `CivTier`, `RecomputeEff`), `Modules/Classes.cs` (yıllık sınıf yetenekleri).
  - **Silinenler:**
    - Araştırma: teknoloji ağacı (`MAIN_TECHS`, `CLASS_TECHS`, `TECH`, `D.TechCost`), `Civ.Research`, `Research.cs` (seçim, düğüm bitişi, kaynak kapısı, bilgi çalma), araştırmacı işi, `research`/`cheapKnown`/`lootTech`/`spy` etkileri, araştırma olay ve ölçüleri.
    - Çağlar: `Civ.Era`/`EraDay`, `ERA_*`, çağ olayları ve bütün çağ koşulları.
    - Alt sınıflar ve uç güçler (`Civ.Subclass`, seçim ağırlıkları, olayları; avatar, kalkan, meteor, talih, diriliş, ilahi müdahale, usta hırsız, kukla, suikast, kayıpsız hasat, kıtlıksızlık).
    - Harikalar (`WONDERS`, `wonder` yapısı, etkileri, olayları).
    - Mevsimler: üretim çarpanları yıllık ortalamayla (tarla 0,9125, yaban 0,95, fırtına 1,45, yangın 1,25, han yolcu akışı 0,9625), kış/soğuk (deri kış giysisi, soğuk yıl, `winterImmune`), atların yaşlanması, sert kış krizi. Tarih geçici olarak "Yıl N, Gün D" ("N. yılın D. gününde").
  - **Kademe** yalnız nüfusla (12 / 40 / 100), histerezisle: nüfus eşiğin %85'ine inmeden kademe düşmez (titreme 9 → 5 değişim/dünya-yıl). Kademenin ilk yükselişi kroniğe yazılır; medeniyetin ilk kasabası ve şehri büyük olay (eski çağ atlamanın yerine), mana ve mithril yatakları o kademede görünür olur.
  - **Kural:** E. çağın düğümü → kademe ≥ E−1. Yere bağlı olanlar yerleşimin, medeniyet çapındakiler medeniyetin kademesine (en büyük yerleşimi) bakar:

    | Kapı | Kademe | Bakılan | Eski düğüm |
    |---|---|---|---|
    | Ev, palisat, sunak, fırın, tarla/iskele/ocak/tuğla/maden/ağıl/ot L1, bıçkıhane, av köşkü, tuz | 0 Kamp | — | I. çağ (baştan açık) |
    | Tersane (liman) | yok: kıyısı olan her yerleşim | yerleşim | Tekne Yapımı |
    | Taverna, pazar, kütüphane, sınıf yapısı (başkent), bira evi, aletçi, silahhane, tarla L2, iskele L2, maden L2, haras, kristal kazısı, Kutsal Koru | 1 Köy | yerleşim | II. çağ |
    | Taş konak, taş sur, darphane, lonca salonu, şifa evi, demir girdisi, tarla L3, orman L3, iskele L3, kesme taş, bitki bahçesi, kristal kulesi; demir aletler yarı aşınır, orman 2 kat hızlı yenilenir (+%25 kereste) | 2 Kasaba | yerleşim | III. çağ |
    | Kale (başkent), fener kulesi, derin maden, ley taşı, mithril | 3 Şehir | yerleşim | IV. çağ |
    | Asker yazımı, kervan muhafızı, yol, öncü, kara ticareti, alet talebi, mana görünür | 1 Köy | medeniyet | Talim, Yol, Takas, Bronz Aletler, Arcana I |
    | Uzak kervan (40), demir silah/zırh, hekimlik, koga, denizaşırı koloni (2), açık deniz, keşif gemisi, ikmal, korsan avı | 2 Kasaba | medeniyet | Kervancılık, Demircilik, Hekimlik, Gemicilik, Seyir |
    | Kadırga ve donanma, deniz ticareti (80, geniş ambar), kuşatma makineleri, efsun, mithril zırh ve görünürlük | 3 Şehir | medeniyet | Donanma, Deniz Ticareti, Kuşatma, Efsunlama, Mithril, Derin Kazı |
    | Özel birlikler (paladin muhafız/şövalye, lejyoner, kurtlar, treant, golem…), Kutsal Sefer (paladin, Şehir), Druid ormanı (Köy), Keşiş silahsız askeri (Kasaba) | sınıfa göre | medeniyet | sınıf ağacı (`ClassDef.Perks`) |

  - **Yerine konan denge etkileri** (ölçülerek): kademe verimi +0,1/0,2/0,3 (yerleşimin; Bronz Aletler, Lonca, Taç), kademe vergisi +0,5/+0,8 (medeniyetin Kasaba/Şehir kademesi; Para, Taç), büyüme +0,05 (+0,1 Kasaba'dan; İnanç, Hekimlik), ticaret altını +0,3 (Kasaba) / +1,0 (Şehir), savunma AC +2 (Şehir; Kale Yapımı). Sınıf ağacının savaş etkileri (saldırı/zırh/hasar artıları, bozgunsuzluk, ilk vuruş…) yerine bir şey konmadı.
  - **Deri:** kış giysisi yerine kent tüketimi (kademeyle 0,001–0,0025/kişi/gün, tundrada ×1,5) ve askerin teçhizat bakımı (0,003/gün); yokluğu cezasız.
  - **Kuraklık** (sert kışın yerine, mevsimsiz kriz): 60–90 gün tarla ve toplayıcı verimi yarıya iner, kasaba yangını iki katı, ambardaki tahılın %20–40'ı çürür.
  - **Komşu kaynak hırsı:** kaynak kapısına takılan araştırma yerine, medeniyet bir kademeye yeni vardığında (5 yıl) o kademenin işlediği ama erişemediği kaynağı (kil; bakır, kalay, at; demir, altın, ot, mana; mithril) arar.
  - **Kent tüketiminde yokluk** ancak medeniyetin bir yerleşiminde o atölye (fırın, bira evi, aletçi) varsa sayılır.
  - Kayıt biçimi sürüm 2 (sürüm 1 açılmaz). Ölçüme kademe dağılımı (yerleşim ve başkent), kademe değişimi, liman/gemi/kadırga, ambar günü eklendi; çağ ve araştırma ölçüleri çıktı.

- **Faz 1b-4 · Dünyanın ayarı** (yol haritası Faz 1b/4). Kod: `Modules/Monsters.cs` (kamp bandı), `Modules/Diplomacy.cs` (büyük şehrin istikrarı, hedef seçimi, taht şehri), `Modules/Agents.cs` (`Engage`/`Siege`: kuşatma ve düşüş), `Modules/Dragon.cs` (akın tavanı), `Core/Tiers.cs` (`Sim.BIG_TIER`, `IsBig`, `IsCore`), `Core/Sim.cs` (`RemovePop` tabanı, `Civ.Seat`).
  - **Kamp bandı:** büyüyen hedef (`3 + yıl/6`) yerine yaşayan kara kampı (korsan koyu ve ejderha hariç) **8–10**. 8'in altında yeni inler birer birer gelir (günlük olasılık eksik/15, ardışık iki in arası 20–40 gün; eskiden eksik/40, 40–80 gün); goblin yayılması ve hobgoblin işgali de yalnız 8'in altında. 8–10 arasında yalnız bugbear ini ve anlatıcının krizleri (istila, trol çetesi) in açar; 10 hiçbir yoldan aşılmaz. Ölçüm: f1b-3'te kara kampı 1–10. yıllarda 7–9,5 (erken goblin yayılması), 11–40. yıllarda ~6, 51–60. yıllarda ~9; bantla bütün yıllarda ~8 (geç yıllarda ejderha ve korsanla ~9). Bant dar: üst sınırı erken goblin yayılması doldurursa erken yıllar geç yıllardan kalabalık olur (ölçüt 3). Geç inlerin büyüklüğü ve troller yerinde.
  - **Büyük şehir** = Şehir kademesi (kademe 3, nüfus ≥ 100, histerezisle ≥ 85; `Sim.IsBig`). Dünyada 21–60. yıllarda ortalama ~7, 60. yılda ~10; v3'ün 4–6'sından çok, çünkü dünya hâlâ 8 kamptan büyüyor. "Şehir kademesindeki başkent" de ~7 veriyor (60. yılda başkentlerin %87'si Şehir), ayrı bir nüfus eşiği kademe eşiğinin kopyası olurdu; 4–6 çekirdek şehir, az sayıda büyük şehirle başlayan dünya üretimiyle gelmeli.
  - **Korunan yerleşim** (`Diplomacy.Guarded`): büyük şehir ve Kasaba+ başkent (başkentini bir kez kaybetmiş medeniyette her başkent). Başkent = taht şehri: bugünkü en kalabalık yerleşim ya da dünkü gün sonunun başkenti (`Civ.Seat`; asker yazımı başkentin nüfusunu bir günlüğüne düşürse de taht korunur).
  - **Kuşatma:** savaş ordusu korunan yerleşimin önünde en az `BIG_SIEGE_DAYS` = 20 gün karargâh kurar; kuşatma büyük olaydır ve şehrin o günkü zayıflığını yazar (uyarı). Sonra hücum.
  - **Zayıflık** (`Diplomacy.CityWeakness`, v3 istikrarının en küçük hâli, zar yok): garnizon (asker/nüfus < %4: +1, < %8: +0,5), açlık (+1; kıtlık ilanı +0,5), pazarda ekmek/bira yok (+0,5), hazine boş (+1), son 10 yılda zorla alınmış (+1), saldıranın tarihî hakkı (+0,5), halkın çoğu başka ırktan (+0,5), 200 günü aşan savaş (+0,5), art arda hücumlar (son 2 yılda her hücum +1, en çok +2), salgın (+1); kanun −0,5×kanun, imar −0,5×imar/100; taht şehrinde başkent kaybından sonra 3 yıl −1 ve her eski kayıp için −1.
  - **Düşüş:** hücumu kazanan ordu korunan yerleşimi ancak zayıflık ≥ `BIG_FALL` = 1 ve şehri tutacak güç kaldıysa (`CanHoldCapital`) alır; yoksa yağmalar (başkentte hazinenin %35'i, taşradaki büyük şehirde nüfus payı kadarı) ve çekilir. Sağlam bir büyük şehir ilk hücuma düşmez; iki yıl içinde ikinci/üçüncü hücuma ya da içi çürüyünce düşer.
  - **Hedef:** büyük şehre yürümek üstünlük ister (`BIG_ODDS` = 1, başkentse `CAPITAL_ODDS` = 1,5). Üstün saldırgan sıradan savaşta düşmanın 40 fersah içindeki en zayıf büyük şehrine yürür ("Büyük şehir X'i almak"); güç yetmezse hedef büyük şehirse en yakın küçük yerleşime döner.
  - **Çekirdek şehir** (bir kez Şehir olmuş; `Sim.IsCore`) terk edilmez: `RemovePop` onu 12 kişinin altına indiremez; açlık, yaşlılık, salgın, kıtlık göçü ve han mültecileri de o sınırda durur. f1b-3'te 16 dünyada 4 eski Şehir terk edilmişti (mülteci akını, salgın, akınlar).
  - **Ejderha** büyük şehri düşüremez: akındaki ölüler şehri 85'in altına indiremez (önce alevde ölenler azalır, sonra düşen askerler yaralı sayılır), yanan evler 4 evsizi (baraka göçü eşiği) aşmaz. Anlatıcının ejderha krizi de aynı `Raid`'i kullanır.
  - **Ölçüm aracı (v3):** `Core/Stats.cs` `V3Log` (günlük kademe sayıları, yerleşim durum değişimleri, savaş, kuşatma, büyük şehrin el değiştirmesi, ejderha akınları) ve rapordaki "v3: durum değişimi" bölümü; yeni ölçütler 6a–6j ve 7 († = yeni takvime bağlı: oranlar ×4, süreler ÷4; `--proj`). Pencereler koşunun uzunluğuna göre (60 yılda eskisiyle aynı), yazılar `Sim.YEAR`'ı izler; `--days N` eklendi. "Yakıldı/yandı" sayımı onarımları saymaz (onarım `BurnedAt`'i gün − 10 yapar).

**Son ölçüm** (`reports/f1b-4`, 16 dünya × 60 yıl, Faz 1b-4 dâhil): 1–5 ve 5a/5b geçiyor. Seed 1, gün 7200 hash'i `ea518c4edcbf6601` (iki ayrı süreçte aynı). Önceki ölçüm `reports/f1b-3` (hash `bc3a0162ce7eebdc`); v3 ölçüleri f1b-3 kodunda aynı araçla yeniden koşuldu (`reports/f1b-3-v3`, hash'ler aynı); aşağıdaki "f1b-3" v3 sütunu odur.

| # | Ölçüt | Port (başlangıç) | c1-4 | f1b-2 (C3) | f1b-3 (kademe) | f1b-4 (ayar) |
|---|---|---|---|---|---|---|
| 1 | Donma yok (41–60. yıl / 6–20. yıl büyük olay) | 0,46 ✗ | 0,97 ✓ | 0,90 ✓ | 1,2 ✓ | 1,03 ✓ |
| 2 | Çöküş olan dünya | 1/16 ✗ | 14/16 ✓ (146 çöküş) | 14/16 ✓ (98 çöküş) | 15/16 ✓ (130 çöküş) | 14/16 ✓ (40 çöküş) |
| 3 | Yaşayan kamp (41–60. yıl / 6–20. yıl) | 1,74 / 6,52 ✗ | 10,2 / 8,31 ✓ | 9,31 / 8,67 ✓ | 10,2 / 8,16 ✓ | 9 / 8,75 ✓ |
| 4a | Doğuş seviyesi (on yıllar) | 2,2 → 5,0 ✗ | 1,31–1,45 ✓ | 1,32–1,50 ✓ | 1,28–1,43 ✓ | 1,29–1,38 ✓ |
| 4b | Sv8+ olan dünya | 0/16 ✗ | 15/16 ✓ | 14/16 ✓ | 15/16 ✓ | 13/16 ✓ |
| 4c | Dünya başına efsane (medyan) | 13 ✗ | 3 ✓ | 4 ✓ | 5,5 ✓ | 6 ✓ |
| 4d | Ölen kahraman payı | %19 ✗ | %44 ✓ | %40 ✓ | %39 ✓ | %42 ✓ |
| 5a / 5b | Determinizm / kayıt-yükleme | ✓ / — | ✓ / ✓ | ✓ / ✓ | ✓ / ✓ | ✓ / ✓ |

v3 ölçütleri (Faz 1b-4; pencere 21–60. yıl, 6b bütün koşu; † yeni takvime yansıtılmış: oran ×4, süre ÷4):

| # | Ölçüt (hedef) | f1b-3 | f1b-4 |
|---|---|---|---|
| 6a | Yerleşim sayısı sabit, ısınmadan sonra (değişim katsayısı ≤ %10) | %9,4 ✓ | %9,5 ✓ |
| 6b | Yerleşim sayısı sabit, bütün koşu (≤ %10) | %35 ✗ | %36 ✗ |
| 6c | El değiştirme, dünyada / 100 gün (bilgi) | 0,86 (yeni takvimde 3,5) | 0,77 (3,1) |
| 6d | Durum değişimi, yerleşim başına (bilgi) | ~670 günde bir | ~675 günde bir |
| 6e † | Orta halka kademe değişimi (60–150 günde bir) | 1741 → 435 ✗ | 1783 → 446 ✗ |
| 6f † | Büyük şehir el değiştirmesi (dünyada 2–4 / 100 gün) | 0,04 → 0,17 ✗ (toplam 49) | 0,02 → 0,08 ✗ (toplam 19) |
| 6g † | Uyarı: kuşatmanın başı → düşüş (5–10 gün) | 0 → 0 ✗ | 20 → 5 ✓ |
| 6h † | Savaş süresi (10–40 gün) | 30 → 7,5 ✗ | 40 → 10 ✓ |
| 6i † | Büyük şehir kuşatması (2–6 gün) | 1 → 0,25 ✗ | 21 → 5,25 ✓ |
| 6j | Başkent kaybı, medeniyet başına en çok 3 | 7 ✗ (12 medeniyet > 3) | 2 ✓ |
| 7 | Felaket büyük şehri düşürmez (terk yok, ejderha eşiğin altına indiremez) | ✗ (4 eski Şehir terk) | ✓ |

Faz 1b-4, f1b-3'e karşı (16 dünya): büyük şehre hücum 55 → 114, el değiştirerek biten %89 → %18 (66'sı yağmayla, 28'i püskürtülerek bitti); büyük şehrin el değiştirmesi 49 → 19 (21–60. yıl); başkent kaybı 113 → 29, yok olma 17 → 11; bütün fetihler 820 → ~680; savaş medyanı 30 → 40 gün (Kutsal Sefer 39 → 110). Yaşayan kamp on yıllara göre 7,1 → 11 yerine 8,1 → 9,1 (bant). Ayrıştırma: dünyada ~1 savaş / 100 gün (yeni takvimde ~4), hedefi büyük şehir olan %15 (f1b-3: %8): her savaş bir büyük şehir alsa bile yeni takvimde ~4 / 100 gün.

Faz 1b-3, f1b-2'ye karşı (önceki adım):

On yıllık dünya medyanı; aynı f1b-2 kodu kademe ölçümüyle yeniden koşuldu, hash'ler aynı:

| Ölçü | 1–10 | 11–20 | 21–30 | 51–60 |
|---|---|---|---|---|
| Nüfus | 272 → 391 | 1390 → 1629 | 2510 → 2578 | 3810 → 3661 |
| Yerleşim | 15 → 22 | 49 → 51 | 65 → 66 | 85 → 78 |
| Köy+ / Kasaba+ / Şehir | 8 / 1 / 0 → 13 / 2 / 0 | 33 / 10 / 0,7 → 35 / 15 / 1,3 | 47 / 21 / 3 → 48 / 24 / 5 | 63 / 33 / 8 → 63 / 33 / 10 |
| Başkent kademesi (ort.) | 0,76 → 0,96 | 1,87 → 2,03 | 2,38 → 2,55 | 2,77 → 2,87 |
| Altın medyanı | 53 → 44 | 209 → 201 | 360 → 398 | 769 → 731 |
| Ambar (gün) | 81 → 80 | 156 → 221 | 253 → 257 | 183 → 177 |
| Liman / gemi / kadırga | 0,5 / 0,6 / 0 → 2 / 2,7 / 0 | 6 / 13 / 2,5 → 8,5 / 18 / 1,2 | 11 / 28 / 12 → 13 / 29 / 9 | 18 / 52 / 20 → 17 / 47 / 16 |
| Savaş başlangıcı / çöküş (yıl) | 0,1 / 0 → 0,2 / 0 | 0,95 / 0,05 → 0,8 / 0,15 | 1,15 / 0,05 → 1,2 / 0,1 | 1,4 / 0,1 → 1,6 / 0,1 |

Toplamlar (16 dünya × 60 yıl, f1b-2 → f1b-3): kıtlık ilanı 17 → 10, açlık ölümü 6 → 29 (soğuktan ölen 217 → 0), gerginlik 399 → 616, antlaşma 175 → 258, kasaba yangını 202 → 458, maaşsız ayrılan kahraman 236 → 411, ejderha ittifakı 8 → 3 ve ejderha ölümü 8 → 2, kuraklık 46 (sert kış 31).

C3 ekonomisi (f1b-2, c1-4'e karşı): medeniyet altın medyanı 20. yılda 325, 60. yılda 781 (2,4 kat; önce 2223 → 5585, p90 37 bin → 1,6 bin). Boştaki iş gücü 20. yıldan sonra %15–20 (önce %47–50). Kıtlık: 16 dünyada 17 ilan, 6 açlık ölümü (önce 244). Canavar baskınında giden altın dünya başına ~0,5 bin (önce ~43 bin); ejderha haracı ~17 bin (önce ~160 bin).

**Yarım kalanlar.** Yok. Eski WIP (`wip/faz1-c3-c4.patch`) Faz 1b'de ayrıldı: C3 ana hatta (yukarıda), dünya harikası yarışı ve C4'ün tamamı (dışlayan teknoloji grupları, IV. çağ maliyeti, Kadim Bilgi, yemin/bedel/çember, d20 Yabani Büyü) atıldı. Patch git geçmişinde duruyor.

**Başlanmayanlar**
- #4 kronik zinciri (olaylara `Parent` bağı).
- Godot bağlantısı (Faz 2: gözlemci harita).

## Açık hatalar ve riskler

### Ekonomi ve kurallar (ana hatta açık)
1. **Yatak yenilenmesi çok hızlı.** Yenilenme döngüsü her yaşayan medeniyetin ekonomi tikinde ayrı çalışıyor. Şifalı ot ve kadim ağaç 7–9 kat hızlı yenileniyor (`Economy`).
2. **Altın kapısı hep açık.** `Sim.Access(c, "gold")` stokta 2 altın olunca geçiyor. Bu yüzden darphane koşulu ve altın anlaşmazlıkları hiçbir şeyi engellemiyor.
3. ~~`cheapKnown` etkisinin büyüklüğü yok sayılıyor.~~ Kalktı (Faz 1b-3: araştırma yok).
4. ~~Canavar baskını fazla götürüyor.~~ Düzeldi (C3): kentin nüfus payı kadar alıyor.
5. ~~Ejderha haracı geç yıllarda çok büyük.~~ Düzeldi (C3): haraç nüfusla tavanlı; dünya başına ~17 bin.
6. ~~Altın birikiyor, gider yok.~~ Düzeldi (C3): bakım ve kamu işleri; 60. yıl medyanı ~0,8 bin.
7. ~~Araştırma ağacı ~20. yılda bitiyor.~~ Kalktı (Faz 1b-3): araştırma ve çağlar yok, ilerleme yerleşim kademesinde.

### Denge ve tasarım
8. **Bazı dünyalar hep barışçıl kalıyor.** Savaş olmayınca çöküş de olmuyor.
9. ~~**"Kötü" medeniyetler başkentini defalarca kaybedebiliyor:** en kötü durumda 7 kez.~~ Düzeldi (Faz 1b-4): taht şehri korunuyor; en çok 2 kez (16 dünya). Bkz. #47.
10. **Bölünme sayısı eşiğe çok duyarlı.** Eşik 3,5'ten 3,25'e inince bölünme 6'dan 16'ya çıkıyor.
11. **B1, eski davranışları değiştirdi;** tasarımla teyit edilmeli:
    - savaş sırasında göçmen gönderilmiyor;
    - kurucu yeniden doğuş yalnız kendi kendine yok olan medeniyete;
    - başkente yürüyüş 1,5× güç istiyor.
12. **Ejderhaların çoğunu adsız bir asker deviriyor.** Ün en yüksek seviyeli sağ kahramana yazılıyor; bu geçici bir çözüm.
13. **Efsane sayısı gürültülü:** benzer ayarlarla medyan 2–4 arasında oynuyor.
14. ~~`harvest` ve `teleport` etkileri pratikte nadiren devreye giriyor.~~ `harvest` kalktı (uç güç); `teleport` artık hiçbir yerden gelmiyor (bkz. #31).

### Kod ve araçlar
15. **`Sim.Extinct` sözleşmesiz kiralık kahramanları "gone" yapıyor.**
16. **`Tr.Ek` sayıyla biten adlara yanlış ek veriyor.** `Lore.Ek` bunu sarıyor.
17. **Suikast destanı dolaylı yazılıyor:** doğrudan çağrıyla değil, günlük taramayla (`Lore.LateDeath`). Faz 1b-3'ten beri suikast yok (alt sınıf etkisiydi); tarama başka geç ölümler için duruyor.
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
25. **Maaşsız kahramanlar ayrılıyor:** dünya başına ~26 (toplam 411; f1b-2'de 236). Yoksul medeniyetlerin kahraman maaşı ödenemiyor; Faz 1b-3'te taverna Köy'le erken açıldığından erken yıllarda daha çok.
26. **Kıtlık seyrek:** 16 dünyada 10 ilan (f1b-2'de 17), 29 açlık ölümü (6). Mevsimsiz ambarda açık küçük ve sürekli olabiliyor: fethedilip köylerini yitiren büyük şehir (tarlası yok, toplayıcı 10 kişiyle sınırlı) ilan eşiğine (ortalama %25 açık) varmadan yavaş açlık çekiyor.
27. **Medeniyet yağması (`Agents.LootFrom`) hâlâ bütün hazinenin %20'sini alıyor**, kentin payını değil. C3'ün kapsamı dışında kaldı.
28. **Amele iş gücünün ~%22'si** (geç yıllar). Boştaki payı (%15–20) buna bağlı; amele altın ve gıda yakar, mal üretmez.
29. **Çöküş:** c1-4 146 → f1b-2 98 → f1b-3 130 (başkent kaybı 113). Faz 1b-3'teki artış için bkz. #35.
30. **Rapordaki "Altın birikiyor" satırı hâlâ "evet" diyor.** Kaba eşik: 30. yıl ≥ 10 × 1. yıl. Gerçek artış 20 → 60. yıl 2,4 kat.

### Faz 1b-3'ten kalanlar
31. **Sınıf kimliği zayıfladı.** Sınıf ağacının savaş etkileri (saldırı/zırh/hasar artıları, bozgunsuzluk, ilk vuruş, şifa) gitti; yerine bir şey konmadı. Sihirbaz'ın mekanik özelliği kalmadı (araştırmaydı), Haydut casusluğu, Keşiş inzivası ve Rahip/Paktçı'nın alt sınıfa göre hediyeleri yok. Artık hiçbir kaynaktan gelmeyen genel etki anahtarları: `soldierAtk`, `soldierHp`, `noRout`, `firstStrike`, `enemyMorale`, `healBack`, `teleport`, `forestMove`, `blackMarket`, `warband`, `heroLevel`, `prodFood`, `prodMana`, `prodAll`, `favoredHunt` (`defAc` yalnız Şehir kademesinden). Sınıfların devlet/kurum spec'ine.
32. **Ejderha seyrek ölüyor:** ittifak 8 → 3, ölüm 8 → 2 (16 dünya); haraç +%22. Uyanış canı en güçlü iki ordunun o günkü gücüne göre ölçülüyor; eskiden ağaç uyanıştan sonra orduları büyütüyordu, şimdi yalnız kademe (Şehir: efsun, mithril, AC +2). v3'e uygun olabilir (oyuncuya yer kalır), ama ejderha ünü 4b/4c'yi besliyordu; izlenmeli.
33. **Kasaba yangını 2,3 kat** (202 → 458; büyük olay): taş konak ve taş sur Kasaba kademesine bağlandı, köyler ve kamplar ahşap kalıyor.
34. **Erken yıllar hızlandı:** araştırmacı işi yok (iş gücünün ~%17'si), I. çağ yapıları baştan açık, kademe yalnız nüfusla. İlk on yılda nüfus +%44, yerleşim +%43, yaşayan kahraman 2,6 kat; 30. yıldan sonra f1b-2 düzeyinde (60. yıl nüfus −%4, yerleşim −%8).
35. **Gerginlik +%54** (399 → 616), antlaşma +%47: kademe ihtiyaçlarından (yeni kademede 5 yıl). Başlayan savaş benzer; çöküş 98 → 130, bölünme 13 → 18, Kutsal Sefer 23 → 32: savunma zayıfladı (sınıf etkileri yok, kale yalnız Şehir başkentte, taş sur Kasaba'da).
36. **Kütüphane ve lonca salonunun ekonomik etkisi kalmadı** (araştırma ve verim düğümleriydi). Kütüphaneyi yalnız kahramanlar kullanıyor (`Will` "library" hedefi); lonca salonu boş bir yapı.
37. **Göç %64 azaldı** (3103 → 1128): mevsimsiz ambarlar medeniyetler arasında refah farkı yaratmıyor.
38. **Fener kulesi yalnız Şehir kademesindeki limanlarda** (eskiden Deniz Ticareti bilen her liman); kadırga Şehir başkentiyle (60. yılda filo −%18).
39. **Takvim geçici:** yıl hâlâ 120 gün, tarih "Yıl N, Gün D"; 40 günlük takvim sonraki adım. Han defteri 30 günlük dönem (`InnBook.Period`); kahraman maaşı ve hedef seçimi 30 günde bir. Faz 1b-4'ün süreleri (kuşatma 20 gün, yıpranma 2 yıl, kamp arası 20–40 gün) de yeniden zamanlanmalı.
40. **Kademe eşikleri sabit** (12 / 40 / 100; histerezis %85). Başkentlerin %33'ü 20. yılda, %87'si 60. yılda Şehir; Şehir kapıları (kadırga, efsun, kale) geç yıllarda neredeyse herkeste.

### Faz 1b-4'ten kalanlar
41. **Büyük şehir çok seyrek el değiştiriyor:** yeni takvimde ~0,08 / 100 gün (hedef 2–4; f1b-3'te 0,17). Ayrıştırma (21–60. yıl): dünyada ~1 savaş / 100 gün (yeni takvimde ~4), hedefi büyük şehir olan %15, büyük şehre hücumların %17'si düşüşle bitiyor. Her savaş bir büyük şehir alsa bile ~4 / 100 gün: hedef ya savaşların neredeyse hepsinin çekirdek şehirler için olmasını ya da iç yoldan düşüşü (veraset krizi, darbe, mezhep bölünmesi; yol haritası Faz 1b/7) ister. Kollar: `BIG_FALL`, `BIG_WEAR`, `BIG_ODDS`, `BIG_SIEGE_DAYS`, savaş temposu.
42. **Orta halka durgun:** Köy/Kasaba başına ~1780 günde bir kademe değişimi ya da terk (yeni takvimde ~450; hedef 60–150). Yerleşim durum tablosu (Faz 1b/7) ve yeniden zamanlama gerekir.
43. **Küçük yerleşim kuşatması 1 gün** (yeni takvimde 0,25; v3: 2–6): karargâh süresi yalnız korunan yerleşimlerde.
44. **Savaşların çoğu kısa:** medyan 40 gün (yeni takvimde 10, hedefin alt ucu); küçük hedefli savaş ilk fetihte biter. Kutsal Sefer uzadı (medyan 110 gün): başkente yürür, taht şehri yağmalanıp düşmez.
45. **Bütün koşuda yerleşim sayısı sabit değil** (değişim katsayısı %36): dünya 8 kamptan başlıyor (dünya üretimi). Isınmadan sonra %9,5 (sınırda).
46. **Çöküş azaldı** (130 → 40; 16 dünyanın 14'ünde ≥ 1; ölçüt 2'ye 12 dünya yeter): başkent kaybı 113 → 29.
47. **Taht şehri geçici bir tanım:** başkent hâlâ "en kalabalık yerleşim" (B1); `Civ.Seat` yalnız bir gün geriye bakıyor (asker yazımı başkenti bir günlüğüne başka yere taşımasın). Eski kayıp başına −1 zayıflık yol haritasının "en fazla birkaç kez"i için konan yapay bir ayar: iki kez düşmüş taht neredeyse alınamaz. Devlet modelinde meşruiyetle değişmeli.
48. **Çekirdek şehir tabanı** (`RemovePop`, 12 kişi): tabandaki şehirde ölen asker nüfustan düşmez, yalnız asker sayısı düşer; ordu daha az nüfus taşır.
49. **Ejderha akınından sonraki 60 günde** 20 büyük şehir kademe düşürdü (bilgi; asker yazımı, salgın, göç). Akının kendisi düşüremiyor (ölçüt 7).
50. **Kamp ölçütü (3) dar:** 9 ≥ 8,75. Bant 8–10, yayılma yalnız 8'in altında; erken yıllar üst sınıra dayanırsa ölçüt kalır.
51. **Yeni takvime yansıtma ×4 varsayımı** (`--proj`; eski 120 günlük yıl ≈ 30 gün). Takvim değişince 1 olur, hedefler doğrudan okunur.

## Devam için

1. 40 günlük takvim (sonraki adım; yıl uzunluğu ve "Gün D" biçimi): v3 ölçütlerinin † satırları (6e–6i) yeni ölçekte doğrudan okunur (`--proj 1`, `--days`); #41–#44.
2. Sınıfların devlet/kurum spec'i (#31).
3. #4 kronik zinciri.
4. Faz 2: Godot gözlemci harita. `FD.Macro`'yu Godot projesine bağla.
