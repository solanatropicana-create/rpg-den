# Fantastik Dünya: Sistem ve Veri Raporu (oyuncu gözü, v0.23)

**Veri:** 12 seed × 30 yıl ve 3 seed × 60 yıl. Zaman serisini `probe.ts`, tabloları `analyze.ts` üretti. Ham tablolar `tablo_12seed_30yil.md` ve `tablo_3seed_60yil.md`, seed 4'ün son 2 yılının olay dökümü `son2yil_seed4.txt`. `src/` değişmedi.

## 1. İlk izlenim

İlk 12–15 yıl gerçekten canlı: yılda ~60 büyük olay, çağ yarışı, kamp ağı ve farklılaşan erken araştırma sıraları var. Sonra dünya platoya oturuyor. Ağaç ~19. yılda bitiyor, 5 yerleşim tavanı doluyor, kamplar yarıya iniyor, iş gücünün %43'ü boşta kalıyor ve altın anlamsızlaşıyor. Her dünya iki sondan birine varıyor: "5'er yerleşimli donmuş barış" ya da "başkenti düşmeyen zombi komşuları yiyen tek fatih".

## 2. Neyi iyi yapıyor

- **Erken oyun çeşitli.** 4–10. yıllarda iki medeniyetin ana ağaç düğüm kümeleri ortalama %58–67 örtüşüyor (Jaccard). 98 medeniyetten 79 farklı ilk-4-düğüm dizisi çıktı. Kaynak kapıları erken yolu gerçekten değiştiriyor.
- **Sınıf sonucu etkiliyor.** 30. yılda Barbar ortalama 475 nüfus ve 10,9 yerleşime ulaşıyor, Sihirbaz 160 nüfusta, Kan Büyücüsü 216 nüfusta kalıyor.
- **Nedensellik zinciri güçlü.** Her olayın `cause` alanı var. Baskından intikamcı kahraman, kıtlıktan göç, çekişmeden antlaşma ya da savaş doğuyor.
- **Ölçülebilir.** Simülasyon deterministik ve `metrics` sayaçları zengin; bu rapordaki her ölçüt tek bir probe'la alındı.

## 3. Neyi eksik ya da zayıf yapıyor

1. **Araştırma bitiyor ve dallanmıyor.** 46 düğümün tamamını bitirme yılının medyanı 19. 30. yılda medeniyetlerin %98'i, 60. yılda hepsi bitirmiş. Tech CV'si 0,21'den 0,04'e iniyor. İki medeniyetin tamamlama sırası arasındaki benzerlik ρ=0,86; sıraların yalnız çağa göre dizilmiş hâlle benzerliği ise ρ=0,95. Yani sırayı çağ yapısı belirliyor, sonunda da herkes her şeyi alıyor.
2. **Yerleşim tavanı dünyayı donduruyor.** 30. yılda 98 medeniyetin 74'ü tam 5 yerleşimde (`diplomacy.ts:223`). Barış dünyalarında (s1, s4) yerleşim CV'si 0,00. s1 60. yılda da aynı: 8 medeniyetin hepsi 5 yerleşimde, nüfuslar 400–580.
3. **Gerilim sönüyor.** Yıllık büyük olay 11–15. yıllarda 59, 26–30'da 28, 51–60'ta 20. Canavar baskını yılda 3,0'dan 0,3'e iniyor, tehdit altındaki (`threat` > 0,3) medeniyet oranı %24'ten %0'a. Toplamı ~105 olayda tutan şey kahraman olayları: 16–30. yıllarda olayların %40'ı `hero` türünde, en sık şablon kütüphane ziyareti.
4. **Canavarlar geriliyor, kahramanlar birikiyor.** Kamp sayısı 10. yılda 6,8, 30. yılda 3,5. Dünyaların yarısında en fazla 1 kamp kalıyor, açık ilan 0'a iniyor. Buna karşılık dünya başına 131 kahraman var (ortalama seviye 4,26). 24. yıldan sonra tavernalar yalnız Sv5 kahraman üretiyor (`heroes.ts:31`). 12 dünyanın 8'inde tek efsane yok.
5. **Ekonomi doyuyor.** Boşta iş gücü 10. yılda %1, 25–30. yıllarda %43. Altın medyanı 78'den 6503'e çıkıyor (s1'de 60. yılda ~39.800). En pahalı alım ~530 altın. 26–30. yıllardaki net altın artışının %59'u boştaki işçilerin günlük 0,02 altınından geliyor (`economy.ts:190`).
6. **Savaş ya hiç yok ya kartopu, başkent de düşmüyor.** 2/12 dünyada 30 yılda tek savaş çıkmadı. Diğerlerinde bir fatih 12–16 yerleşime (60. yılda 24–30) çıkıyor, kurbanlar başkentlerine sıkışıp yaşıyor (`agents.ts:673`). 98 medeniyetten yalnız 2'si öldü, ikisi de savaşta değil. s10 Druid 20 yıl boyunca 17 nüfustan 0'a eridi; yerine kurulan Paladin 6 kişiyle 2 yılda silindi.

### Tablo A: Yakınsama (12 seed; 60. yıl 3 seed)

| Yıl | Nüfus ort. | Nüfus CV | Tech CV | Yerleşim CV | Çağ IV % | Ağacı bitiren % |
|---|---|---|---|---|---|---|
| 5 | 26 | 0,20 | 0,18 | 0,34 | 0 | 0 |
| 10 | 92 | 0,30 | 0,21 | 0,22 | 12 | 3 |
| 15 | 179 | 0,27 | 0,17 | 0,14 | 55 | 45 |
| 20 | 253 | 0,27 | 0,11 | 0,21 | 82 | 76 |
| 30 | 337 | 0,35 | 0,04 | 0,36 | 94 | 98 |
| 60 | 487 | 0,35 | 0,00 | 0,60 | 100 | 100 |

### Tablo B: Gerilim (dünya başına yıllık ortalama)

| Yıllar | Olay | Büyük olay | Tech | Savaş ilanı | Fetih | Canavar baskını | Kamp temizlenen | Kahraman ölümü |
|---|---|---|---|---|---|---|---|---|
| 1–5 | 55 | 16 | 12,9 | 0 | 0 | 0,9 | 0,4 | 0,6 |
| 6–10 | 132 | 49 | 23,4 | 0,1 | 0,1 | 3,1 | 0,6 | 1,1 |
| 11–15 | 166 | 59 | 23,6 | 0,6 | 0,5 | 3,0 | 1,3 | 2,0 |
| 16–20 | 117 | 40 | 8,4 | 0,7 | 0,7 | 1,0 | 0,7 | 1,3 |
| 21–25 | 104 | 30 | 3,1 | 0,7 | 0,8 | 0,6 | 0,6 | 1,7 |
| 26–30 | 106 | 28 | 0,6 | 0,7 | 0,6 | 0,3 | 0,7 | 3,6 |
| 51–60 (3 seed) | 132 | 20 | 0,4 | 0,5 | 0,5 | 0,5 | 0,5 | 1,8 |

Salgın 6. yıldan sonra yılda 0,2–0,3, yangın 0,1–0,3. Savaşta olan medeniyet oranı hiçbir yıl sonunda %11'i geçmiyor.

### Tablo C: Canavar ve kahraman (dünya başına)

| Yıl | Canlı kamp | ≤1 kamplı dünya % | Açık ilan | Serbest kahraman | Kiralı kahraman | Ort. seviye |
|---|---|---|---|---|---|---|
| 10 | 6,8 | 0 | 3,4 | 22 | 13 | 2,44 |
| 15 | 4,3 | 33 | 0,8 | 29 | 41 | 3,02 |
| 20 | 4,2 | 25 | 0,3 | 37 | 63 | 3,58 |
| 30 | 3,5 | 50 | 0 | 49 | 83 | 4,26 |
| 60 (3 seed) | 4,7 | 0 | 0 | 74 | 119 | 4,65 |

### Tablo D: Ekonomi (medeniyet başına)

| Yıl | Altın medyan | Altın max | Altın/nüfus | Boşta iş gücü % | Araştırmacı % | Gıda (gün) |
|---|---|---|---|---|---|---|
| 10 | 78 | 586 | 1,0 | 1 | 15 | 68 |
| 15 | 225 | 3.159 | 1,3 | 11 | 9 | 144 |
| 20 | 1.118 | 8.660 | 4,3 | 33 | 4 | 192 |
| 25 | 3.278 | 17.128 | 11,2 | 43 | 1 | 232 |
| 30 | 6.503 | 31.324 | 16,9 | 43 | 0 | 224 |

### Tablo E: Seedler arası fark (30. yıl)

| Rejim | Seedler | Savaş ilanı | Nüfus CV | Yerleşim CV | Yerleşim min–max |
|---|---|---|---|---|---|
| Donmuş barış | 1, 4 | 0 | 0,10–0,12 | 0,00 | 5–5 |
| Ilımlı | 2, 6, 8 | 8–9 | 0,14–0,38 | 0,06–0,36 | 5–11 |
| Kartopu | 3, 5, 7, 9–12 | 16–34 | 0,28–0,54 | 0,34–0,62 | 2–16 |

İki donmuş dünyada da Barbar var; rejimi yalnız sınıf listesi belirlemiyor, büyük olasılıkla başlangıç mesafeleri de etkili (doğrulanmadı).

### Tablo F: Hatalar ve ölü mekanikler

| # | Bulgu | Yer | Kanıt |
|---|---|---|---|
| 1 | Çalınan düğüm o an araştırılan düğümse iki kez kaydediliyor. Etkisi `recomputeEff` içinde iki kez toplanıyor, çağ sayımına da giriyor. | `research.ts:170-174`, `research.ts:250-251`, `agents.ts:709-710` → `economy.ts:262`; `sim.ts:116`, `research.ts:122` | 98 medeniyetin 11'inde (s12 Paktçı: 49 kayıt, 46 tekil düğüm) |
| 2 | Darphane her yerleşimde kuruluyor ama gelir medeniyet başına tek +0,12. Stokta 2 altın olması "altın erişimi" sayılıyor, bu yüzden `currency` kapısı fiilen hep açık. | `economy.ts:501`, `sim.ts:135`, `economy.ts:225` | Yaşayan 586 yerleşimin 546'sında darphane var |
| 3 | Hiçbir yerde okunmayan etkiler: `teleport` (Işınlanma Çemberi, Gölge Adımı) ve `harvest`. `prodWood` okunuyor ama hiçbir yerde verilmiyor. | `techs.ts:169`, `classes.ts:157`, `classes.ts:61`, `economy.ts:41` | grep: okuma sayısı 0 |
| 4 | Başkent fethedilemiyor. Kutsal Sefer başkenti hedef aldığı için hiçbir zaman "hedef ele geçirildi" diye bitemiyor. | `agents.ts:673`, `diplomacy.ts:151`, `diplomacy.ts:162` | Savaşla ölen medeniyet 0/98 |
| 5 | Olay listesi 2500 kayıtta kesiliyor. | `sim.ts:53` | 30. yılda ilk 6–15 yılın kaydı silinmiş |
| 6 | Harabe hedefi yalnız terk edilmiş yerleşimden doğuyor, bu yüzden Gezgin yolunun içeriği yok. | `will.ts:239` | `goal_ruin` 12 dünyanın 8'inde 0 |
| 7 | Kahraman seviyesi yılla şişiyor: tavernada +yıl/6, handa +yıl/8. Tavan Sv5 olduğu için gelişim ve efsane mekaniği ölüyor. | `heroes.ts:31`, `inns.ts:33` | Seed 4, 29. yıl: yeni gelenler Sv5 |
| 8 | Geç oyunda yeniden doğan medeniyet yaşayamıyor: 6 kişi ve I. çağla başlıyor. | `diplomacy.ts:377-404` | s10'daki Paladin 2 yılda silindi |

## 4. Kalite artırma önerileri

**1. Yükseliş ve çöküş**
- **Ne yapılır:** `siege()` içindeki `if (!isCap)` koşulu "başka yerleşimi varsa başkent de düşer" olur; `capital()` zaten sıradaki en kalabalık yerleşimi seçiyor. Son yerleşim düşünce `extinct` ve `spawnFounders` devreye girer. `worldTick`'e yıllık bölünme kontrolü eklenir: başkente 12 hex'ten uzak, tier≥1 ve son 3 yılda fethedilmiş ya da kıtlık/salgın görmüş bir yerleşim p≈0,15 ile yeni medeniyet olur (`makeCiv`, ana medeniyetin tech'lerinin %80'iyle).
- **Neden kaliteyi artırır:** Kartopuyu frenler, zombi medeniyetleri sahneden çıkarır. Harabe, hobgoblin işgali (`monsters.ts:64`), yeni kurucu ve Kutsal Sefer gibi ölü duran sistemleri kendiliğinden besler.
- **Efor:** M
- **Dosyalar:** `agents.ts`, `diplomacy.ts`, `sim.ts`, `worldgen.ts`
- **Headless ölçütü:** Dünyaların ≥%75'inde 30 yılda ≥1 çöküş ya da bölünme; 21–30. yıllarda büyük olay ≥40/yıl; en büyük medeniyetin yerleşim payı ≤0,30.

**2. Yerleşim tavanını çağa bağla**
- **Ne yapılır:** `ss.length >= 5` koşulu `>= 2 + c.era + floor(nüfus/200)` olur. `pickSettleTarget` null dönünce `rr.land` (toprak açlığı) her sınıfta artar; saldırganlığı 0,2'nin altındaki sınıflarda yarı hızla (`diplomacy.ts:55`).
- **Neden kaliteyi artırır:** 74/98 medeniyet tam 5 yerleşimde duruyor. Barış dünyası da sınır baskısı yaşar ama kavganın zamanlaması seed'e göre değişir.
- **Efor:** S
- **Dosyalar:** `diplomacy.ts`
- **Headless ölçütü:** 30. yılda tam 5 yerleşimli medeniyet oranı <%35; barış dünyaları dahil yerleşim CV'si ≥0,25; savaşsız dünya 0/12.

**3. Canavar tabanı ve geç tehdit**
- **Ne yapılır:** `monsters.ts:90`'daki `alive.length < 2` koşulu yerine hedef kamp sayısı `3 + floor(yıl/6)` olur; sahipsiz karayla ölçeklenir, eksik kamp başına yılda ~1 kamp doğar. Baskın büyüklüğü (`monsters.ts:148`) hedefin savunma gücüne bağlanır. 15. yıldan sonra yeni bir kademe gelir: `Camp.kind`'a trol çetesi ya da ejderha ini eklenir.
- **Neden kaliteyi artırır:** Çoğu evde bekleyen 131 kahramana iş çıkar, tehdit eğrisi sıfırdan kalkar.
- **Efor:** M
- **Dosyalar:** `monsters.ts`, `data/classes.ts`, `worldgen.ts`, `heroes.ts`
- **Headless ölçütü:** 20–30. yıllarda canlı kamp ≥4 ve en fazla 1 kamplı dünya ≤%10; açık ilan ≥1,5; canavar baskını ≥1,5/yıl; tamamlanan ilan dünya başına ≥6 (bugün ortalama ~4).

**4. Ekonomiye çıkış kanalları**
- **Ne yapılır:** Boştaki işçinin günlük altını 0,02'den 0,004'e iner (`economy.ts:190`). Bakım gelir: kiralık kahraman 0,25/gün, L2–L3 yapı 0,01–0,03/gün, asker 0,006'dan 0,02'ye (`economy.ts:225`). Kasaba ve şehirde kişi başı bira/ekmek/alet/iksir tüketilir; karşılanmazsa büyüme −%30 (`happy` genişletilir). Hazine 50×nüfusu aşarsa aşan kısım günde %1 erir.
- **Neden kaliteyi artırır:** Kıtlık ve seçim geri gelir, zengin ile yoksul medeniyet ayrışır.
- **Efor:** M
- **Dosyalar:** `economy.ts`, `heroes.ts`, `inns.ts`
- **Headless ölçütü:** 20–30. yıllarda boşta iş gücü ≤%15; 30. yılda altın/nüfus medyanı 2–8; 15. yıldan sonra medeniyetlerin ≥%30'unda en az bir yıl negatif altın değişimi.

**5. Ağaçta gerçek seçim, bitmeyen araştırma**
- **Ne yapılır:** `TechDef`'e bir `excl` grubu eklenir. III. çağda {Kervancılık, Taş Surlar, Gemicilik} içinden 2'si, IV. çağda {Taç ve Kanun, Ticaret Ağları, Kuşatma Makineleri, Yüksek Büyü, Kale Yapımı} içinden 3'ü seçilebilir; `availableTechs` (`research.ts:13`) kotası dolan grubu eler. Ağaç bitince araştırma puanı tekrarlanabilir "Kadim Bilgi" düğümlerine gider (her biri prodAll +%3, maliyet ×1,3). Mana, mithril ve altın kapısı stokla aşılamaz olur (`sim.ts:135`).
- **Neden kaliteyi artırır:** Medeniyetler farklı biter, araştırmacı katmanı kaybolmaz.
- **Efor:** M
- **Dosyalar:** `data/techs.ts`, `research.ts`, `economy.ts`, `sim.ts`
- **Headless ölçütü:** 30. yılda aynı düğüm kümesine sahip medeniyet çifti <%10; 20. yılda ana ağaç Jaccard ≤0,80; 21–30. yıllarda araştırma olayı ≥3/yıl.

Ayrıca Tablo F'deki 1–3 numaralı hatalar S eforla hemen düzelir. Çift tech için steal yollarına `t !== c.research.current` koşulu yeter; darphaneye adet sınırı konabilir.

## 5. Tek cümle

Eren yalnızca bir şey yapacaksa, o şey şu olmalı: başkentin de düşebildiği bir çöküş ve bölünme mekaniği eklemek, çünkü dünyaları "donmuş barış" ile "zombi komşulu tek fatih" sonlarından yalnız bu kurtarır ve zaten yazılmış ama ölü duran harabe, işgal, yeni kurucu ve Kutsal Sefer sistemlerini kendiliğinden besler.
