# Ölçüm raporu: f1b-7: dünyanın durumu (durum tablosu, istikrar ve iç çöküş yolları, fırsat merkezleri, tepki inşaatı)

16 dünya (seed 1-16) × 60 yıl (2400 gün), 1 yıl = 40 gün · 2026-10-02 02:22 · `FD.Macro.Run stats --seeds 1-16 --years 60 --jobs 2 --verify 1 --saveload 1`

Süre: 16 dk 28 sn duvar saati, 2 iş parçacığı; dünya başına 111 sn (en az 64,8, en çok 151; yıl sonu hash'leri dâhil).

## Bitiş ölçütleri

DESIGN-FAZ1.md, "Bitiş ölçütleri" (1–5), yol haritası v3 (6–7, Faz 1b-4) ve v3'ün süre tablosu (8, Faz 1b-5). ✓ geçti · ✗ kaldı · — ölçülemedi · ○ bilgi (hedef yok) · † zaman ölçeğine bağlı (Faz 1b-5'ten beri takvim yeni ölçekte: 1 yıl = 40 gün, değerler doğrudan okunur).

| # | Ölçüt | Koşul | Ölçülen | Sonuç |
|---|---|---|---|---|
| 1 | Donma yok | 41–60. yılların yıllık büyük olay medyanı ≥ 0,8 × 6–20. yılların medyanı | 256 / 230 = 1,11 kat | ✓ |
| 2 | Çöküş | dünyaların ≥ %75'inde 60 yılda ≥ 1 çöküş (yok olma ya da başkent kaybı) | %100 (16/16 dünya); toplam 109 çöküş: 43 yok olma, 66 başkent kaybı | ✓ |
| 3 | Kamplar | 41–60. yıllarda yaşayan kamp medyanı bantta (8–10, ±1) ve 6–20. yılların en az %90'ı (v3: sabit bant) | geç 9,03, erken 8,69 (oran %104; yıl sonu sayımıyla 9 / 9) | ✓ |
| 4a | Kahraman: doğuş seviyesi | her on yılda doğanların ortalama seviyesi ≤ 2 | 1,62 · 1,62 · 1,58 · 1,61 · 1,6 · 1,65 (on yıllar sırasıyla) | ✓ |
| 4b | Kahraman: Sv8+ | dünyaların ≥ yarısında en az bir kahraman Sv8 ve üstüne çıkar | %100 (16/16 dünya); dünyadaki en yüksek seviye: medyan Sv10, en çok Sv10 | ✓ |
| 4c | Kahraman: efsane | dünya başına efsane medyanı 1–6 | medyan 2 (p10–p90: 0–4,5; toplam 39) | ✓ |
| 4d | Kahraman: ölüm payı | doğan kahramanların %30–80'i ölür | %27 (2994/11267); dünya medyanı %25 (%20–%34) | ✗ |
| 5a | Determinizm | aynı seed → aynı tarih (toplayıcılı ve toplayıcısız koşu, her yıl sonu hash'i) | seed 1: 60/60 yıl sonu aynı | ✓ |
| 5b | Kayıt/yükleme | kaydet → yükle → devam = kesintisiz koşu (her yıl sonu hash'i) | seed 1, gün 1237: yüklenen dünya aynı, sonraki 30/30 yıl sonu aynı | ✓ |
| 6a | Yerleşim sayısı sabit, ısınmadan sonra (21–60. yıl) | dünya başına yaşayan yerleşim sayısının (günlük) değişim katsayısı medyanı ≤ %10 | değişim katsayısı %0,7 (%0,1–%1,6); ortalama 78,7 yerleşim, en az ve en çok ortalamanın %97 ve %101 kadarı | ✓ |
| 6b | Yerleşim sayısı sabit, bütün koşu (1–60. yıl) | dünya başına yaşayan yerleşim sayısının (günlük) değişim katsayısı medyanı ≤ %10 (Faz 1b-6: dünya tarih öncesiyle olgun başlar) | değişim katsayısı %0,9 (%0,3–%3); ortalama 78,8 yerleşim, en az ve en çok ortalamanın %95 ve %101 kadarı | ✓ |
| 6c | El değiştirme (her yerleşim) | fetih + bölünme + içeriden düşüş (Faz 1b-7), 100 günde, dünya başına ve yerleşim başına (21–60. yıl); hedef yok | dünyada 4,72 (3,22–7,41) / 100 gün; yerleşim başına 6,56 / 10 bin gün: 4,72 / 100 gün | ○ |
| 6d | Durum dalgalanması (yerleşim başına) | açlık, salgın, kuşatma, yakılma, kademe, el değiştirme, terk, yeniden yerleşim: yerleşim başına 100 günde (21–60. yıl); hedef yok | yerleşim başına 1,22 (1,01–1,42) / 100 gün, yani ~81,9 günde bir; dünyada 93,8 / 100 gün: yerleşim başına ~81,9 günde bir | ○ |
| 6e | Orta halka: kademe değişimi † | Köy/Kasaba başına kademe değişimi ya da terk, yeni takvimde 60–150 günde bir (21–60. yıl) | yerleşim başına 127 (110–152) günde bir | ✓ |
| 6f | Büyük şehir el değiştirmesi † | büyük şehir (Şehir kademesi) bütün dünyada yeni takvimde 100 günde 2–4 kez el değiştirir (21–60. yıl): dışarıdan (fetih, bölünme) ya da içeriden (Faz 1b-7: tipin çöküş yolu; yönetim değişir ya da şehir ayrılır); felaketle düşmez | dünyada 2,56 (1,59–3,25) / 100 gün; şehir başına 0,44 / 100 gün; dünyada ortalama 6 büyük şehir; toplam 664 el değiştirme (40 fetih, 3 bölünme, 621 içeriden: soylu isyanı 218, darbe 158, düello 137, mezhep bölünmesi 42, aforoz 29, veraset 19, ayrılık 11, boyların ayrılması 7). Ayrıştırma: dünyada 3,25 savaş / 100 gün, hedefi büyük şehir olan %22; büyük şehre 212 hücum, düşüşle bitenlerin payı %19 | ✓ |
| 6g | Büyük şehir: uyarı süresi † | büyük şehrin düşüşünden önce yeni takvimde 5–10 gün uyarı (kuşatmanın ya da iç krizin başı → düşüş, medyan; 21–60. yıl) | 7 gün (5–10; n = 661): kuşatmanın başından 5 (n = 40), iç krizin başından 7 (n = 621); savaşın başından 22 gün (11–48,1) | ✓ |
| 6h | Savaş süresi † | savaş (ilandan barışa) yeni takvimde 10–40 gün (medyan; 21–60. yıl başlayıp biten savaşlar) | 15 gün (10–40; n = 759) | ✓ |
| 6i | Kuşatma süresi † | büyük şehir kuşatması (karargâhtan hücuma) yeni takvimde 2–6 gün (medyan; 21–60. yıl) | büyük şehir 6 gün (6–6; n = 212); diğer yerleşimler 3 gün (3–6; n = 621) | ✓ |
| 6j | Başkent kaybı (medeniyet başına) | hiçbir medeniyet başkentini 3 kereden çok kaybetmez (yol haritası: "en fazla birkaç kez") | en çok 3; 3'ten çok kaybeden 0 medeniyet; toplam 66 başkent kaybı, 47/169 medeniyette (1×: 33, 2×: 9, 3×: 5) | ✓ |
| 7 | Felaket büyük şehri düşürmez | Şehir kademesine varmış yerleşim hiç terk edilmez; ejderha akını büyük şehrin nüfusunu Şehir eşiğinin (85) altına indiremez; akından sonraki 60 günde terk yok | terk edilen eski Şehir: 0; ejderha akını 377 (büyük şehre 35, orada 390 ölü): akınla eşiğin altına inen 0, sonraki 60 günde terk 0; bilgi: akın günü başka nedenlerle (ordu, öncü) eşiğin altına inen 0, 60 gün içinde kademe düşüşü 12, el değiştirme 2 | ✓ |
| 8a | Salgın süresi | salgın başladığı günden bittiği güne: 5–10 gün (medyan) | 6 gün (4–7; n = 535) | ✓ |
| 8b | Tepki inşaatı: yanan ev | yanan her ev yandığı günden onarıldığı güne (ilk yanan ilk onarılır): 1–3 gün (medyan) | 3 gün (2–5; n = 13053) | ✓ |
| 8c | Tepki inşaatı: sur | palisat ve taş sur: projenin başından bitişine: 5–10 gün (medyan) | 7 gün (5–10; n = 406); palisat 7, taş sur 8 gün | ✓ |
| 8d | Büyük proje (kale, kule) | kale ve fener kulesi: projenin başından bitişine: 15–30 gün (medyan) | 25 gün (23–30; n = 218); kale 25 (n = 140), kule 23 gün (n = 78) | ✓ |
| 8e | Temizlenen kamp → yeni köy | temizlenen kara kampının 6 fersah yakınına 60 gün içinde ilk yerleşimin kurulması: 10–20 gün (medyan) | 12 gün (4–47; n = 153); temizlenen 5999 kampın 153 tanesine (%2,6) köy kuruldu, 20 gün içinde %1,6 | ✓ |
| 8f | Han kurulumu | yeni han: hancının yola çıktığı günden kapıların açıldığı güne: 5–10 gün (medyan) | 7,5 gün (6–9,7; n = 14); harabeyi yeniden kurma 3,5 gün (n = 16) | ✓ |
| 8g | Kahraman doğumu (han) | han başına 10–20 günde bir (açık han-günü / handa doğan kahraman) | 15,1 günde bir (11118 doğum / 168052 han-günü); aynı handa iki doğum arası medyan 12 gün (2–34); bilgi: taverna başına 6898 günde bir (149 doğum) | ✓ |
| 8h | Kahramanın efsaneye yükselişi | doğumundan efsane olduğu güne: 100–300 gün (medyan) | 209 gün (71,6–745; n = 39) | ✓ |
| 8i | İlan ömrü | alınmayan ilanın asıldığı günden kapandığı güne (süresi doldu ya da kampı başkası temizledi): 10–20 gün (medyan) | 15 gün (0–15; n = 3549); bütün ilanlar 5 gün (n = 17621): biten %44 (asılıştan 4 günde), süresi dolan %17; başarısız sefer ödülü %25 artırır (Heroes.QuestFailed) | ✓ |
| 8j | Yoldaş/kahraman maaşı | haftalık (5 gün) | kural: medeniyetin kahramanları her 5. gün (Economy.PayHeroes), han personeli haftada bir (InnLife.PayWages); maaşı 3 hafta ödenmeyen kahraman ayrılır | ○ |
| 9a | Örgütler yaşar, şubeleri dalgalanır | hiçbir örgüt kalıcı olarak yok olmaz (koşu sonunda ya yaşıyor ya da yeniden kurulmayı bekliyor); örgüt başına yıllık şube sayısının değişim katsayısı medyanı ≥ %10 | kalıcı yok olan 0/208; koşu sonunda yaşayan 200; dağılma 49, yeniden kuruluş 41; şube değişim katsayısı %19 (%10–%64); açılan 15075, kapanan 14459 şube (942 / 904 dünya başına) | ✓ |
| 9b | Gölge savaşı düzenli | dünya-yıllarının ≥ %90'ında en az bir gölge savaşı eylemi (suikast, sabotaj, ihbar); hedef sıklık sonra ayarlanacak | en az bir eylemi olan dünya-yılı %100; dünyada 18,1 (15,4–20,3) / 100 gün; başarısız %54; öldürülen usta/lider 1554; türler: suikast 3489, sabotaj 1788, ihbar 1627 | ✓ |
| 9c | Tiplerin çöküş nedenleri farklı | dört hükümet tipinin en sık çöküş nedeni birbirinden farklı (başkent fethi, bölünme ya da içeriden düşüş: veraset savaşı, soylu isyanı, düello, boyların ayrılması, darbe, paralı askerler, mezhep bölünmesi, aforoz) | Krallık: soylu isyanı (başkent fethi 20, bölünme 14, veraset savaşı 32, soylu isyanı 413, Pakt bağı ve aforoz 30; 2368 devlet-yılı); Boylar: reisin düelloda ölümü (başkent fethi 50, bölünme 8, reisin düelloda ölümü 301, boyların ayrılması 23; 1394 devlet-yılı); Cumhuriyet: darbe (başkent fethi 21, bölünme 22, darbe 249; 1106 devlet-yılı); Teokrasi: mezhep bölünmesi (başkent fethi 10, bölünme 6, mezhep bölünmesi 80, Pakt bağı ve aforoz 13; 1218 devlet-yılı) | ✓ |
| 9d | Esaret sınırlı, Özgürlük Ağı etkin | köle payı her dünya-yılında nüfusun ≤ %5'i; Özgürlük Ağı dünyaların ≥ %75'inde köle kurtarır | köle payı medyan %0,3 (p90 %1,6, en çok %4,1); esarete düşen 6066 (av 1389, savaş 1633, borç 1640, baskın 1404); hapis madeni 3258; Özgürlük Ağı 16/16 dünyada 4944 köle kurtardı; kaçan 761, azat 539; esir düşen kahraman 818 | ✓ |
| 9e | Devriye profili tipe göre ayrışır | kervan başına durdurma, el koyma ve rüşvet: tipler arasında en yüksek / en düşük durdurma oranı ≥ 1,5, el koyma ve rüşvet oranları ≥ 2 | Krallık: 100 kervan-günde 7,25 durdurma, durdurmada el koyma %5, rüşvet %4,1, haraç/vergi %39, tutuklama 1, düello 0; Boylar: 100 kervan-günde 4,88 durdurma, durdurmada el koyma %3,2, rüşvet %5,8, haraç/vergi %57, tutuklama 0, düello 2108; Cumhuriyet: 100 kervan-günde 5,79 durdurma, durdurmada el koyma %2,3, rüşvet %15, haraç/vergi %58, tutuklama 0, düello 0; Teokrasi: 100 kervan-günde 10 durdurma, durdurmada el koyma %7,3, rüşvet %4,9, haraç/vergi %28, tutuklama 622, düello 0 (oranlar: durdurma ×2,06, el koyma ×3,22, rüşvet ×3,72) | ✓ |
| 9f | Aç haydutlar kıtlıkla ilişkili | dünya-yılı başına aç ya da ekmeksiz yerleşim payı ile haydut olan aç halk arasında korelasyon r ≥ 0,3 (en az 30 haydut) | r = 0,86 (960 dünya-yılı); haydut olan 15991, aç haydut kampı 3163, yiyecek verilip dağılan 753, açlıktan eriyen 1032; aç ya da ekmeksiz yerleşim payı medyanı %4,4 | ✓ |
| 6k | Durum tablosu | her yerleşimde 5–15 günde bir zar (yerleşim-günü / zar ortalaması 5–15); zarla gelen durum 3–15 gün sürer (medyan, p10 ≥ 3, p90 ≤ 15) | zar ortalama 10 günde bir (293752 zar); zarla gelen durum 8 gün (4–13; n = 94545); yerleşim-günlerinin %30'inde bir durum var | ✓ |
| 6l | Fırsat merkezi döngüsü | döngüler kurulur ve çöker: her dünyada ≥ 5 tamamlanan döngü; toplam süre (söylentiden sona) medyanı 10–30 gün; evrelerin medyanı hedef aralıkta (söylenti 1–2, hücum 3–6, zirve 4–10, tükeniş 2–6, hayalet 3–6) | toplam 21 gün (17–25; n = 2322); evreler: söylenti 2, hücum 5, zirve 7, tükeniş 4, hayalet 4; dünyada 6,56 (6,21–6,81) / 100 gün, dünya başına en az 121 tamamlanan; türler: maden 676, yol konağı 514, verimli vadi 500, kutsal kalıntı 476, antik harabe 191, ordu pazarı 155; sonuç: hayalet (terk) 2313, söylentide söndü 146, yıkıldı 19, kalıcı köy 9; zirve nüfusu medyanı 23 | ✓ |
| 6m | Tepki inşaatı | sur yalnız tehditten (baskın, kuşatma, yağma, akın) sonra ya da savaşta sınırda kurulur (kural); tehditten sonra sur ortalama ≤ 20 günde başlar; büyük proje (sınır kalesi, fener kulesi) olay olarak gelir, dünya başına 60 yılda 3–60 | sur projesi 408 (tehditten sonra 171, ortalama 4,65 gün sonra; gerisi savaşta sınır boyunda); surusuz yerleşime gelen tehdit 864; büyük proje dünya başına 13 (8–19; kale 141, fener 78), sabotaj 74 | ✓ |
| 6n | Büyük şehrin istikrarı ve iç krizler | istikrar (0–100) garnizon, kıtlık, vergi, meşruiyet, savaş yorgunluğu ve durumdan; düşük istikrarda iç kriz (5–10 gün belirti) ve tipin çöküş yolu; hedef yok | büyük şehir ve taht şehri istikrarı 64,2 (39,5–82,8); kriz dünyada 6,1 / 100 gün (n = 2346), belirti 8 gün (5–10), düşüşle biten %48; büyük şehirde 1841 kriz, düşüş %50 | ○ |

Tanımlar:

- **1, 3:** "41–60. yılların medyanı": 41–60. yıllardaki bütün (dünya, yıl) değerlerinin medyanı (16 dünya × 20 yıl = 320 değer); 6–20. yıllar için de aynı. Pencereler koşunun uzunluğuna göre: erken = koşunun %10'u – üçte biri, geç = son üçte biri (1 yıl = 40 gün). Büyük olay = `GameEvent.Major == true` olan olaylar (`Sim.OnEvent` ile sayılır, `World.Events` kırpılmasından etkilenmez). Kamp ölçütünde yıllık değer, o yıl her gün sayılan yaşayan kamp sayısının ortalamasıdır (korsan koyları dâhil; yıl sonu sayımıyla değer ayrıca verilmiştir).
- **2:** Çöküş: medeniyet yok olur (`Civ.Alive` false olur) ya da medeniyet yaşarken başkentini kaybeder: bir önceki gün sonunda başkenti olan yerleşim (`Sim.Capital`, en kalabalık yerleşim) artık yaşamıyor ya da başka medeniyetin; medeniyetin o gün hâlâ yerleşimi vardır. Son yerleşimin düşmesi yok olma olarak bir kez sayılır.
- **4a:** Her on yılda doğan bütün kahramanların (bütün dünyalar) doğuş seviyelerinin ortalaması; her on yıl ≤ 2 olmalı. Doğuş seviyesi kahraman `World.Heroes`'a girdiği anda okunur.
- **4b:** Dünyada koşu boyunca herhangi bir kahramanın ulaştığı en yüksek seviye ≥ 8 olan dünyaların payı.
- **4c:** Dünya başına koşu boyunca efsane olan (`Hero.Legend`) kahraman sayısının dünyalar arası medyanı.
- **4d:** Koşu boyunca doğan bütün kahramanlardan (bütün dünyalar) koşu sonunda ölü (`State == "dead"`) olanların payı; emekli olanlar ve diyarı terk edenler ölü sayılmaz.
- **5a:** İlk `--verify` seed'i aynı süreçte toplayıcı bağlanmadan yeniden koşulur; her yıl sonundaki kanonik dünya hash'i (golden testteki FNV-1a) toplayıcılı koşuyla karşılaştırılır. Bu, determinizmi ve toplayıcının simülasyonu değiştirmediğini birlikte denetler.
- **5b:** `Sim.Save(string)` ve `static Sim Sim.Load(string)` varsa ilk `--saveload` seed'i yıl ortasında (gün = yıl/2 × 40 + 37) kaydedilip yüklenir ve sonuna dek koşulur; yüklenen dünyanın hash'i ve sonraki yıl sonu hash'leri kesintisiz koşuyla karşılaştırılır.

- **6–7 (v3):** bkz. "v3: durum değişimi" bölümü. Pencere: ısınmadan sonra (koşunun son üçte ikisi); 6b bütün koşu. Büyük şehir = Şehir kademesi (`Sim.BIG_TIER`). El değiştirme = fetih ya da bölünme. Kuşatma = hedefin önünde ilk savaş ordusunun karargâh kurduğu günden (`Agent.Muster`) hücum muharebesine; karargâhsız hücum 1 gün. Uyarı = kuşatmanın (ya da savaşın) başından büyük şehrin el değiştirdiği güne. Savaş = `War.Since`'ten savaşın ilişkiden düştüğü güne.

## v3: durum değişimi

Yol haritası v3: dünya büyüyerek değil, durum değiştirerek yaşar. Bütün değerler dünya başına; hücre: dünyalar arası medyan (p10–p90). "100 günde": pencerede sayılan / pencerenin günü × 100; "yerleşim başına": yaşayan yerleşim-günlerine bölünür. Büyük şehir = Şehir kademesi (kademe 3, nüfus ≥ 100; histerezisle ≥ 85). Isınma: koşunun ilk üçte biri (800 gün). Faz 1b-6'dan beri dünya tarih öncesiyle (1600 gün) olgun başlar, ölçüm tarih öncesinin sonundan başlar; ısınma penceresi önceki ölçümlerle karşılaştırma için tutuldu.

| Ölçü | bütün koşu (1–60. yıl) | ısınmadan sonra (21–60. yıl) |
|---|---|---|
| Yaşayan yerleşim (günlük ortalama) | 78,8 (74,2–80) | 78,7 (75,2–80) |
| Yaşayan yerleşim: en az (ortalamaya oranı) | %95 (%86–%96) | %97 (%93–%99) |
| Yaşayan yerleşim: en çok (ortalamaya oranı) | %101 (%100–%103) | %101 (%100–%102) |
| Yaşayan yerleşim: değişim katsayısı | %0,9 (%0,3–%3) | %0,7 (%0,1–%1,6) |
| Büyük şehir (günlük ortalama) | 6,19 (4,92–7,1) | 6 (4,43–7,21) |
| El değiştirme (fetih + bölünme), dünyada / 100 gün | 5,08 (3,33–7,21) | 4,72 (3,22–7,41) |
| El değiştirme, yerleşim başına / 10 bin gün | 6,73 (4,2–9,45) | 6,56 (4,05–9,61) |
| Durum değişimi (kuruluş hariç), yerleşim başına / 100 gün | 1,28 (1,04–1,43) | 1,22 (1,01–1,42) |
| Durum değişimi, dünyada / 100 gün | 96,8 (80,9–110) | 93,8 (77,9–109) |
| Orta halka (Köy/Kasaba) kaydı: yerleşim başına kaç günde bir | 122 (107–135) | 127 (110–152) |
| Büyük şehir el değiştirdi, dünyada / 100 gün | 2,65 (1,63–3,35) | 2,56 (1,59–3,25) |
| Büyük şehir el değiştirdi, şehir başına / 100 gün | 0,43 (0,27–0,62) | 0,44 (0,25–0,67) |
| Büyük şehre hücum, dünyada / 100 gün | 0,94 (0,27–1,35) | 0,81 (0,19–1,41) |
| Büyük şehir yağmalandı ama tutulmadı, dünyada / 100 gün | 0,33 (0,08–0,79) | 0,31 (0,03–0,78) |

Durum değişimi türleri (yerleşim başına 10 bin günde; bölünme dışında olayın kademesi olaydan önceki):

| Tür | bütün koşu (1–60. yıl) | ısınmadan sonra (21–60. yıl) |
|---|---|---|
| Fetih (`capture`) | 3,63 (1,56–4,61) | 2,95 (1,1–4,35) |
| Bölünme (`secede`) | 0,13 (0,05–0,23) | 0,16 (0,04–0,28) |
| İçeriden düşüş (yönetim değişti) (`regime`) | 3,53 (2,07–4,6) | 3,53 (1,78–4,78) |
| Açlık başladı (`famine`) | 8,8 (0,35–29,6) | 9,57 (0,04–31,9) |
| Salgın başladı (`plague`) | 1,82 (1,15–2,4) | 2,02 (1,06–2,46) |
| Kuşatma (hücum) (`siege`) | 4,93 (2,15–6,84) | 4,45 (1,64–6,55) |
| Yakıldı / yandı (`burn`) | 22,2 (12,9–34,6) | 20,4 (12,1–30,8) |
| Kademe yükseldi (`tierUp`) | 37,6 (32,2–45,4) | 35,7 (28,2–43) |
| Kademe düştü (`tierDown`) | 37,7 (33,3–45,6) | 37,2 (30,8–41,3) |
| Terk (harabe) (`abandon`) | 0,71 (0,16–2) | 0,63 (0,15–1,74) |
| Harabeye yeniden yerleşim (`resettle`) | 0,67 (0,16–1,45) | 0,44 (0,12–1,35) |
| Kuruluş (durum değişimi sayılmaz) (`found`) | 0,32 (0,1–0,64) | 0,24 (0–0,48) |

Süreler (gün; bütün dünyalar havuzlanmış; savaş: ilandan ilişkiden düştüğü güne, koşu sonunda süren savaşlar hariç; kuşatma: hedefin önündeki ilk karargâhtan hücuma, karargâhsız hücum 1 gün; uyarı: büyük şehrin düşüşünden geriye):

| Süre | Pencere | n | p10 | medyan | p90 | ortalama | en çok |
|---|---|---|---|---|---|---|---|
| Savaş (hepsi) | bütün koşu (1–60. yıl) | 1311 | 10 | 13 | 40 | 21,3 | 78 |
| Savaş: sıradan | bütün koşu (1–60. yıl) | 624 | 10 | 15 | 45 | 23,6 | 78 |
| Savaş: tarihî hak | bütün koşu (1–60. yıl) | 195 | 10 | 13 | 35 | 19,7 | 78 |
| Savaş: Kutsal Sefer | bütün koşu (1–60. yıl) | 160 | 8 | 15 | 47,3 | 24,5 | 78 |
| Savaş: savunma paktı | bütün koşu (1–60. yıl) | 165 | 8,8 | 12 | 26,2 | 14,2 | 52 |
| Savaş: müttefik çağrısı | bütün koşu (1–60. yıl) | 167 | 10 | 13 | 35 | 18,9 | 78 |
| Kuşatma: büyük şehir | bütün koşu (1–60. yıl) | 341 | 6 | 6 | 6 | 5,96 | 7 |
| Kuşatma: diğer yerleşimler | bütün koşu (1–60. yıl) | 1028 | 3 | 3 | 6 | 3,97 | 8 |
| Uyarı: kuşatmanın başı → büyük şehrin düşüşü | bütün koşu (1–60. yıl) | 74 | 5 | 5 | 5 | 4,95 | 6 |
| Uyarı: savaşın başı → büyük şehrin düşüşü | bütün koşu (1–60. yıl) | 74 | 12 | 18 | 31,7 | 21,6 | 67 |
| Savaş (hepsi) | ısınmadan sonra (21–60. yıl) | 759 | 10 | 15 | 40 | 22,2 | 78 |
| Savaş: sıradan | ısınmadan sonra (21–60. yıl) | 368 | 10 | 17,5 | 45 | 24,3 | 78 |
| Savaş: tarihî hak | ısınmadan sonra (21–60. yıl) | 110 | 10 | 15 | 35 | 20,9 | 78 |
| Savaş: Kutsal Sefer | ısınmadan sonra (21–60. yıl) | 104 | 10 | 15 | 51,4 | 26,2 | 78 |
| Savaş: savunma paktı | ısınmadan sonra (21–60. yıl) | 89 | 9,6 | 12 | 27 | 14,7 | 45 |
| Savaş: müttefik çağrısı | ısınmadan sonra (21–60. yıl) | 88 | 10 | 12 | 35 | 17,9 | 50 |
| Kuşatma: büyük şehir | ısınmadan sonra (21–60. yıl) | 212 | 6 | 6 | 6 | 5,94 | 6 |
| Kuşatma: diğer yerleşimler | ısınmadan sonra (21–60. yıl) | 621 | 3 | 3 | 6 | 4 | 7 |
| Uyarı: kuşatmanın başı → büyük şehrin düşüşü | ısınmadan sonra (21–60. yıl) | 40 | 5 | 5 | 5 | 4,93 | 5 |
| Uyarı: savaşın başı → büyük şehrin düşüşü | ısınmadan sonra (21–60. yıl) | 40 | 11 | 22 | 48,1 | 24,2 | 67 |

Koşu sonunda süren savaş: 10 (sürelere girmedi).

Kuşatma sonuçları (bütün koşu): büyük şehir: 341 hücum: 74 el değiştirdi, 147 yağmalandı (tutulmadı), 120 püskürtüldü; diğer: 1028 hücum: 741 el değiştirdi, 243 yağmalandı (tutulmadı), 44 püskürtüldü.

Büyük şehrin el değiştirmesi (bütün koşu, 997; ilk 60):

| Seed | Gün (yıl) | Şehir | Nasıl | Önceki → yeni sahip | Kuşatma → düşüş | Savaş → düşüş |
|---|---|---|---|---|---|---|
| 1 | 29 (1) | Kurtgeçit | içeriden: düello | Kemikçadır Boyları → Kemikçadır Boyları | kriz 7 gün | – |
| 1 | 91 (3) | Kurtgeçit | içeriden: düello | Kemikçadır Boyları → Kemikçadır Boyları | kriz 5 gün | – |
| 1 | 111 (3) | Yelköy | içeriden: darbe | Demirkanat Krallığı → Demirkanat Krallığı | kriz 9 gün | – |
| 1 | 162 (5) | Kocapınar | içeriden: düello | Kurtoba Boyları → Kurtoba Boyları | kriz 7 gün | – |
| 1 | 167 (5) | Kurtgeçit | içeriden: düello | Kemikçadır Boyları → Kemikçadır Boyları | kriz 8 gün | – |
| 1 | 208 (6) | Yelköy | içeriden: soylu isyanı | Demirkanat Krallığı → Demirkanat Krallığı | kriz 9 gün | – |
| 1 | 209 (6) | Kurtgeçit | içeriden: düello | Kemikçadır Boyları → Kemikçadır Boyları | kriz 5 gün | – |
| 1 | 218 (6) | Kurtgeçit | içeriden: düello | Kemikçadır Boyları → Kemikçadır Boyları | kriz 5 gün | – |
| 1 | 224 (6) | Kurtgeçit | fetih | Kemikçadır Boyları → Karageçit Teokrasisi | 5 gün | 14 gün |
| 1 | 249 (7) | Kurtgeçit | içeriden: mezhep bölünmesi | Karageçit Teokrasisi → Karageçit Teokrasisi | kriz 5 gün | – |
| 1 | 277 (7) | Kurtgeçit | içeriden: mezhep bölünmesi | Karageçit Teokrasisi → Karageçit Teokrasisi | kriz 8 gün | – |
| 1 | 309 (8) | Yelköy | içeriden: soylu isyanı | Demirkanat Krallığı → Demirkanat Krallığı | kriz 8 gün | – |
| 1 | 325 (9) | Kocapınar | içeriden: düello | Kurtoba Boyları → Kurtoba Boyları | kriz 7 gün | – |
| 1 | 366 (10) | Kurtgeçit | içeriden: soylu isyanı | Karageçit Teokrasisi → Karageçit Teokrasisi | kriz 8 gün | – |
| 1 | 380 (10) | Kocapınar | içeriden: düello | Kurtoba Boyları → Kurtoba Boyları | kriz 6 gün | – |
| 1 | 399 (10) | Kurtgeçit | içeriden: soylu isyanı | Karageçit Teokrasisi → Karageçit Teokrasisi | kriz 6 gün | – |
| 1 | 546 (14) | Kocapınar | içeriden: düello | Kurtoba Boyları → Kurtoba Boyları | kriz 5 gün | – |
| 1 | 561 (15) | Demiroba | içeriden: mezhep bölünmesi | Yeminköprü Teokrasisi → Yeminköprü Teokrasisi | kriz 6 gün | – |
| 1 | 582 (15) | Kocapınar | fetih | Kurtoba Boyları → Yeminköprü Teokrasisi | 5 gün | 24 gün |
| 1 | 587 (15) | Kurtgeçit | içeriden: soylu isyanı | Karageçit Teokrasisi → Karageçit Teokrasisi | kriz 9 gün | – |
| 1 | 625 (16) | Yelköy | içeriden: soylu isyanı | Demirkanat Krallığı → Demirkanat Krallığı | kriz 10 gün | – |
| 1 | 627 (16) | Kurtgeçit | içeriden: soylu isyanı | Karageçit Teokrasisi → Karageçit Teokrasisi | kriz 5 gün | – |
| 1 | 630 (16) | Kocapınar | içeriden: mezhep bölünmesi | Yeminköprü Teokrasisi → Kocapınar Teokrasisi | kriz 10 gün | – |
| 1 | 660 (17) | Demiroba | içeriden: mezhep bölünmesi | Yeminköprü Teokrasisi → Yeminköprü Teokrasisi | kriz 10 gün | – |
| 1 | 688 (18) | Kocapınar | içeriden: mezhep bölünmesi | Kocapınar Teokrasisi → Kocapınar Teokrasisi | kriz 5 gün | – |
| 1 | 760 (19) | Kurtgeçit | içeriden: soylu isyanı | Karageçit Teokrasisi → Karageçit Teokrasisi | kriz 8 gün | – |
| 1 | 787 (20) | Kocapınar | fetih | Kocapınar Teokrasisi → Yeminköprü Teokrasisi | 5 gün | 12 gün |
| 1 | 851 (22) | Kurtgeçit | içeriden: soylu isyanı | Karageçit Teokrasisi → Karageçit Teokrasisi | kriz 6 gün | – |
| 1 | 879 (22) | Yelköy | içeriden: darbe | Demirkanat Krallığı → Demirkanat Krallığı | kriz 8 gün | – |
| 1 | 963 (25) | Kuzeybük | içeriden: düello | Kurtoba Boyları → Kurtoba Boyları | kriz 8 gün | – |
| 1 | 1000 (25) | Kuzeybük | içeriden: düello | Kurtoba Boyları → Kurtoba Boyları | kriz 5 gün | – |
| 1 | 1021 (26) | Yelköy | içeriden: darbe | Demirkanat Krallığı → Demirkanat Krallığı | kriz 8 gün | – |
| 1 | 1093 (28) | Kurtgeçit | içeriden: darbe | Karageçit Teokrasisi → Karageçit Teokrasisi | kriz 6 gün | – |
| 1 | 1122 (29) | Kurtgeçit | içeriden: darbe | Karageçit Teokrasisi → Karageçit Teokrasisi | kriz 10 gün | – |
| 1 | 1124 (29) | Kuzeybük | içeriden: düello | Kurtoba Boyları → Kurtoba Boyları | kriz 6 gün | – |
| 1 | 1163 (30) | Kurtgeçit | içeriden: darbe | Karageçit Teokrasisi → Karageçit Teokrasisi | kriz 9 gün | – |
| 1 | 1171 (30) | Yelköy | içeriden: darbe | Demirkanat Krallığı → Demirkanat Krallığı | kriz 10 gün | – |
| 1 | 1182 (30) | Yelköy | içeriden: darbe | Demirkanat Krallığı → Demirkanat Krallığı | kriz 9 gün | – |
| 1 | 1201 (31) | Sümüklüdere II Vadisi | içeriden: soylu isyanı | Granitsunak Krallığı → Granitsunak Krallığı | kriz 8 gün | – |
| 1 | 1202 (31) | Yelköy | içeriden: darbe | Demirkanat Krallığı → Demirkanat Krallığı | kriz 9 gün | – |
| 1 | 1241 (32) | Çeliksöz Vadisi | içeriden: ayrılık | Granitsunak Krallığı → Kocapınar Teokrasisi | kriz 9 gün | – |
| 1 | 1242 (32) | Yelköy | içeriden: soylu isyanı | Demirkanat Krallığı → Demirkanat Krallığı | kriz 5 gün | – |
| 1 | 1267 (32) | Sümüklüdere II Vadisi | içeriden: darbe | Granitsunak Krallığı → Granitsunak Krallığı | kriz 9 gün | – |
| 1 | 1271 (32) | Kurtgeçit | içeriden: darbe | Karageçit Teokrasisi → Karageçit Teokrasisi | kriz 9 gün | – |
| 1 | 1306 (33) | Kurtgeçit | içeriden: darbe | Karageçit Teokrasisi → Karageçit Teokrasisi | kriz 9 gün | – |
| 1 | 1309 (33) | Yelköy | içeriden: soylu isyanı | Demirkanat Krallığı → Demirkanat Krallığı | kriz 5 gün | – |
| 1 | 1353 (34) | Kurtgeçit | içeriden: darbe | Karageçit Teokrasisi → Karageçit Teokrasisi | kriz 7 gün | – |
| 1 | 1505 (38) | Tuzyayla | fetih | Granitsunak Krallığı → Yeminköprü Teokrasisi | 3 gün | 15 gün |
| 1 | 1654 (42) | Kurtgeçit | içeriden: darbe | Karageçit Teokrasisi → Karageçit Teokrasisi | kriz 10 gün | – |
| 1 | 1675 (42) | Kurtgeçit | içeriden: darbe | Karageçit Teokrasisi → Karageçit Teokrasisi | kriz 6 gün | – |
| 1 | 1735 (44) | Kurtgeçit | içeriden: darbe | Karageçit Teokrasisi → Karageçit Teokrasisi | kriz 10 gün | – |
| 1 | 1758 (44) | Kurtgeçit | içeriden: soylu isyanı | Karageçit Teokrasisi → Karageçit Teokrasisi | kriz 8 gün | – |
| 1 | 1789 (45) | Yeni Yıldızbük | içeriden: düello | Kurtoba Boyları → Kurtoba Boyları | kriz 6 gün | – |
| 1 | 1821 (46) | Kurtgeçit | içeriden: soylu isyanı | Karageçit Teokrasisi → Karageçit Teokrasisi | kriz 9 gün | – |
| 1 | 1865 (47) | Kurtgeçit | içeriden: veraset | Karageçit Teokrasisi → Karageçit Teokrasisi | kriz 10 gün | – |
| 1 | 1895 (48) | Sümüklüdere II Vadisi | içeriden: soylu isyanı | Granitsunak Krallığı → Granitsunak Krallığı | kriz 9 gün | – |
| 1 | 1990 (50) | Kurtgeçit | içeriden: aforoz | Karageçit Teokrasisi → Karageçit Teokrasisi | kriz 0 gün | – |
| 1 | 2007 (51) | Sümüklüdere II Vadisi | içeriden: soylu isyanı | Granitsunak Krallığı → Kocapınar Teokrasisi | kriz 6 gün | – |
| 1 | 2062 (52) | Kurtgeçit | içeriden: mezhep bölünmesi | Karageçit Teokrasisi → Karageçit Teokrasisi | kriz 7 gün | – |
| 1 | 2251 (57) | Kurtgeçit | içeriden: mezhep bölünmesi | Karageçit Teokrasisi → Karageçit Teokrasisi | kriz 8 gün | – |

## v3: süre tablosu (Faz 1b-5)

Yol haritası v3'ün hedef süreleri (oyun günü; 1 yıl = 40 gün, 1 ay = 10, 1 hafta = 5). Bütün dünyalar havuzlanmış; 6e–6i ısınmadan sonra, 8a–8i bütün koşu. Ajan hızları `Core/Time.cs` (`Pace`).

| Süreç | Hedef | Ölçülen | Sonuç |
|---|---|---|---|
| Orta halkada kademe kayması (6e) | yerleşim başına 60–150 günde bir | yerleşim başına 127 (110–152) günde bir | ✓ |
| Büyük şehir el değiştirmesi (6f) | dünyada 100 günde 2–4 | dünyada 2,56 (1,59–3,25) / 100 gün; şehir başına 0,44 / 100 gün; dünyada ortalama 6 büyük şehir; toplam 664 el değiştirme (40 fetih, 3 bölünme, 621 içeriden: soylu isyanı 218, darbe 158, düello 137, mezhep bölünmesi 42, aforoz 29, veraset 19, ayrılık 11, boyların ayrılması 7). Ayrıştırma: dünyada 3,25 savaş / 100 gün, hedefi büyük şehir olan %22; büyük şehre 212 hücum, düşüşle bitenlerin payı %19 | ✓ |
| ↳ uyarı (6g) | 5–10 gün önceden | 7 gün (5–10; n = 661): kuşatmanın başından 5 (n = 40), iç krizin başından 7 (n = 621); savaşın başından 22 gün (11–48,1) | ✓ |
| Savaş (6h) | 10–40 gün | 15 gün (10–40; n = 759) | ✓ |
| ↳ kuşatma (6i) | 2–6 gün | büyük şehir 6 gün (6–6; n = 212); diğer yerleşimler 3 gün (3–6; n = 621) | ✓ |
| Salgın (8a) | 5–10 gün | 6 gün (4–7; n = 535) | ✓ |
| Tepki inşaatı: yanan ev (8b) | 1–3 gün | 3 gün (2–5; n = 13053) | ✓ |
| Tepki inşaatı: sur (8c) | 5–10 gün | 7 gün (5–10; n = 406); palisat 7, taş sur 8 gün | ✓ |
| Büyük proje: kale, kule (8d) | 15–30 gün | 25 gün (23–30; n = 218); kale 25 (n = 140), kule 23 gün (n = 78) | ✓ |
| Temizlenen kamp → yeni köy (8e) | 10–20 gün | 12 gün (4–47; n = 153); temizlenen 5999 kampın 153 tanesine (%2,6) köy kuruldu, 20 gün içinde %1,6 | ✓ |
| Han kurulumu (8f) | 5–10 gün | 7,5 gün (6–9,7; n = 14); harabeyi yeniden kurma 3,5 gün (n = 16) | ✓ |
| Kahraman doğumu (8g) | han başına 10–20 günde bir | 15,1 günde bir (11118 doğum / 168052 han-günü); aynı handa iki doğum arası medyan 12 gün (2–34); bilgi: taverna başına 6898 günde bir (149 doğum) | ✓ |
| Efsaneye yükseliş (8h) | 100–300 gün | 209 gün (71,6–745; n = 39) | ✓ |
| İlan ömrü (8i) | 10–20 gün (başarısız sefer ödülü +%25) | 15 gün (0–15; n = 3549); bütün ilanlar 5 gün (n = 17621): biten %44 (asılıştan 4 günde), süresi dolan %17; başarısız sefer ödülü %25 artırır (Heroes.QuestFailed) | ✓ |
| Yoldaş maaşı (8j) | haftalık (5 gün) | kural: medeniyetin kahramanları her 5. gün (Economy.PayHeroes), han personeli haftada bir (InnLife.PayWages); maaşı 3 hafta ödenmeyen kahraman ayrılır | ○ |

Proje süreleri, yapı türüne göre (gün; bütün koşu):

| Yapı | n | p10 | medyan | p90 |
|---|---|---|---|---|
| castle | 140 | 25 | 25 | 30 |
| extract | 23069 | 1 | 1 | 1 |
| guild | 51 | 1 | 1 | 1 |
| house | 12588 | 1 | 1 | 1 |
| hut | 40 | 1 | 1 | 1 |
| library | 231 | 1 | 1 | 1 |
| lighthouse | 78 | 23 | 23 | 27 |
| market | 298 | 1 | 1 | 1 |
| mint | 299 | 1 | 1 | 1 |
| palisade | 352 | 5 | 7 | 10 |
| ship | 1225 | 1 | 1 | 2 |
| shipyard | 122 | 1 | 1 | 3 |
| stonehouse | 313 | 1 | 1 | 1 |
| stonewall | 54 | 7 | 8 | 9 |
| tavern | 136 | 1 | 1 | 1 |
| temple | 302 | 1 | 2 | 2 |
| unique | 233 | 1 | 1 | 2 |
| upgrade | 1517 | 1 | 1 | 2 |
| workshop | 2077 | 1 | 1 | 2 |

## Devlet, inanç ve örgüt (Faz 1b-6)

claude/devlet-orgut-spec.md: 4–6 devlet (dört hükümet tipi her dünyada), yerleşimlerin ırk ve inanç dağılımı, 13 örgüt (merkezleri çekirdek şehirlerde). Dünya tarih öncesiyle (1600 gün) olgun başlar; örgütler tarih öncesinin sonunda kurulur, ölçüm o günden başlar. Örgüt kararları ~10 günde bir.

Devletler: dünya başına 6 (ilk yıl, medyan); devlet-yılı tipe göre: Krallık 2368, Boylar 1394, Cumhuriyet 1106, Teokrasi 1218. Yönetici değişimi 1076 (veraset 34, veraset krizi 14, düello 6, boy meclisi 3, konsey oyu 45, tarikat 33); seçim 244; reise meydan okuma 584. Pakt'ın sızdığı yönetici 117, ortaya çıkan 93; Pakt suikastı 14; Tarikat'ın Kutsal Sefer çağrısı 560, başlayan sefer 76. Lobiyle yasa değişikliği 1189 (kölelik yasası 0), darbe girişimi 26.

Örgütler (koşu sonu; dünyalar arası medyan, toplamlar bütün dünyalar):

| Örgüt | yaşıyor (dünya) | şube | üye | gizli şube payı | dağılma / yeniden kuruluş | açılan / kapanan şube | gölge savaşı | üye kahraman işi |
|---|---|---|---|---|---|---|---|---|
| Güneş Kilisesi | 16/16 | 52,5 | 436 | %0 | 0 / 0 | 2448 / 1598 | 928 | 1454 |
| Güneş Tarikatı | 16/16 | 12 | 116 | %0 | 0 / 0 | 1788 / 1581 | 868 | 337 |
| Kara Pakt | 9/16 | 1 | 7,5 | %100 | 7 / 35 | 1093 / 1068 | 830 | 30 |
| Druid Çemberi | 16/16 | 46,5 | 145 | %0 | 0 / 0 | 1070 / 353 | 0 | 1222 |
| Avcılar Locası | 16/16 | 25 | 76 | %0 | 0 / 0 | 824 / 399 | 0 | 0 |
| Hırsızlar Loncası | 16/16 | 7 | 48 | %67 | 0 / 0 | 1345 / 1226 | 1129 | 306 |
| Büyücü Akademisi | 16/16 | 15 | 97,5 | %0 | 0 / 0 | 1740 / 1517 | 0 | 1862 |
| Paralı Bölükler | 16/16 | 16 | 79 | %0 | 0 / 0 | 1950 / 1705 | 0 | 1773 |
| Ozanlar Koleji | 16/16 | 42,5 | 164 | %0 | 0 / 0 | 1470 / 780 | 0 | 0 |
| Tüccarlar Loncası | 16/16 | 55 | 220 | %0 | 0 / 0 | 2462 / 1595 | 1187 | 0 |
| Harabe Kâşifleri | 16/16 | 36 | 193 | %0 | 0 / 0 | 1538 / 952 | 0 | 550 |
| Köle Avcıları | 15/16 | 11,5 | 46 | %0 | 1 / 4 | 1238 / 1016 | 978 | 0 |
| Özgürlük Ağı | 16/16 | 6 | 18,5 | %5,3 | 0 / 2 | 802 / 669 | 984 | 0 |

Örgüt operasyonları (bütün dünyalar): artifact 3840, news 3840, caravan 3744, pilgrims 3515, vigil 3488, rite 3381, track 2707, escort 2112, smuggle 2100, ruin 1988, map 1852, rob 1740, hire 1728, soul 1491, whisper 1251, bounty 1133, sabotage 459, patrol 328, heal 256, infiltrate 117, famineOrder 96, unmask 93, assassinate 14.

Devriye (hükümet tipine göre; bütün dünyalar):

| Tip | maruz ajan-günü | 100 kervan-günde durdurma | durdurmada el koyma | rüşvet | haraç/vergi | düello | kahraman tutuklama | el konan değer | vergi/haraç altını |
|---|---|---|---|---|---|---|---|---|---|
| Krallık | 324691 | 7,25 | %5 | %4,1 | %39 | 0 | 1 | 3760 | 4324 |
| Boylar | 116909 | 4,88 | %3,2 | %5,8 | %57 | 2108 | 0 | 455 | 1964 |
| Cumhuriyet | 129116 | 5,79 | %2,3 | %15 | %58 | 0 | 0 | 424 | 687 |
| Teokrasi | 134109 | 10 | %7,3 | %4,9 | %28 | 0 | 622 | 5671 | 2097 |

Çöküş nedenleri (hükümet tipine göre; bütün dünyalar; 100 devlet-yılı başına):

| Tip | devlet-yılı | başkent fethi | bölünme | veraset savaşı | soylu isyanı | reisin düelloda ölümü | boyların ayrılması | darbe | paralı askerler (iflas) | mezhep bölünmesi | Pakt bağı ve aforoz |
|---|---|---|---|---|---|---|---|---|---|---|---|
| Krallık | 2368 | 20 (0,84) | 14 (0,59) | 32 (1,35) | 413 (17,4) | 0 (0) | 0 (0) | 0 (0) | 0 (0) | 0 (0) | 30 (1,27) |
| Boylar | 1394 | 50 (3,59) | 8 (0,57) | 0 (0) | 0 (0) | 301 (21,6) | 23 (1,65) | 0 (0) | 0 (0) | 0 (0) | 0 (0) |
| Cumhuriyet | 1106 | 21 (1,9) | 22 (1,99) | 0 (0) | 0 (0) | 0 (0) | 0 (0) | 249 (22,5) | 0 (0) | 0 (0) | 0 (0) |
| Teokrasi | 1218 | 10 (0,82) | 6 (0,49) | 0 (0) | 0 (0) | 0 (0) | 0 (0) | 0 (0) | 0 (0) | 80 (6,57) | 13 (1,07) |

## Dünyanın durumu (Faz 1b-7)

Yol haritası v3: yerleşim durum tablosu (her yerleşimde 5–15 günde bir zar), büyük şehrin istikrarı ve tipe göre iç çöküş yolları, fırsat merkezi döngüsü, tepki inşaatı. Bütün dünyalar havuzlanmış, bütün koşu.

Durumlar (yerleşim başına 1000 günde kaç kez; süre: gün, medyan (p10–p90)):

| Durum | 1000 yerleşim-gününde | süre | büyük şehirde payı |
|---|---|---|---|
| Refah (`prosper`) | 7,91 | 10 (6–15) | %8,5 |
| Kaynak tükendi (`depleted`) | 5,06 | 10 (6–15) | %5,5 |
| Kaynak bulundu (`found`) | 4,54 | 9 (5–12) | %6,5 |
| Ticaret patlaması (`boom`) | 4,2 | 7 (4–10) | %11 |
| Göç dalgası (`migration`) | 4,13 | 7 (4–10) | %5,9 |
| Festival (`festival`) | 3,76 | 4 (3–5) | %7,9 |
| Kıtlık (hasat) (`shortage`) | 2,5 | 10 (5–14) | %8,1 |
| Kıtlık (açlık) (`hunger`) | 1,3 | 5 (1–39,2) | %11 |
| Kuşatma (`siege`) | 0,48 | 2 (1–5) | %25 |
| Yeni lord (`newlord`) | 0,37 | 7 (5–10) | %77 |
| İşgal (`occupation`) | 0,28 | 11 (8–14) | %43 |
| Salgın (`plague`) | 0,18 | 5 (4–7) | %54 |
| Canavar tehdidi (`monsters`) | 0,16 | 8 (4–12) | %8,3 |

Göç: göçmen kafilesi 106623, kaçan 140845, gelen 247632 kişi; basamak kaymasına yetecek göç denemesi: yukarı 6754, aşağı 2288.

İç krizler (hükümet tipine göre; düşüş: krizin sonunda yerleşim içeriden düştü):

| Tip | Kriz | n | büyük şehirde | düşüş payı | belirti (gün, medyan) |
|---|---|---|---|---|---|
| Boylar | Meydan okuma | 616 | 406 | %50 | 8 |
| Boylar | Ayrılık | 58 | 58 | %43 | 7 |
| Krallık | Soylu isyanı | 836 | 686 | %50 | 7,5 |
| Krallık | Veraset kavgası | 51 | 43 | %65 | 8 |
| Cumhuriyet | Darbe söylentisi | 470 | 386 | %54 | 8 |
| Cumhuriyet | Ayrılık | 52 | 52 | %33 | 8 |
| Teokrasi | Mezhep çatışması | 271 | 216 | %30 | 7 |

İstikrar (büyük şehir ve taht şehri, 5 günde bir örnek, n = 57714): p10 39,5, p25 51,5, medyan 64,2, p75 74,7, p90 82,8; 40'ın altında %10.
İçeriden düşüş: 1158 (büyük şehirde 911); yollar: veraset 32, soylu isyanı 413, düello 301, boyların ayrılması 23, darbe 249, mezhep bölünmesi 80, aforoz 43, ayrılık 17. Tip değişimi 105 (republic → kingdom 40, theocracy → kingdom 9, kingdom → republic 43, kingdom → theocracy 13); birleşme (evlilik ittifakı) 0; komşuya geçen şehir 161; Kutsal Sefer yenilgisi 18.

Fırsat merkezleri (türe göre; süre: söylentiden sona, gün):

| Tür | n | süre | zirve nüfusu | sonuç | altın (toplam) |
|---|---|---|---|---|---|
| maden | 676 | 21 (18–25) | 22 | hayalet (terk) 633, yıkıldı 4, söylentide söndü 29 | 5642 |
| yol konağı | 514 | 21 (18–26) | 21 | söylentide söndü 42, hayalet (terk) 463, yıkıldı 2 | 2333 |
| verimli vadi | 500 | 21 (17–25) | 20 | söylentide söndü 39, hayalet (terk) 446, yıkıldı 2, kalıcı köy 9 | 0 |
| kutsal kalıntı | 476 | 22 (17–25) | 25 | hayalet (terk) 468, yıkıldı 3, söylentide söndü 1 | 1843 |
| antik harabe | 191 | 22 (18–26) | 16 | söylentide söndü 29, hayalet (terk) 156, yıkıldı 6 | 539 |
| ordu pazarı | 155 | 18 (13–23) | 21 | hayalet (terk) 147, söylentide söndü 6, yıkıldı 2 | 707 |

Aynı anda yaşayan merkez (dünya başına, günlük ortalama): 1,23 (1,09–1,35). Zirvede gelen haydut kampı 829, hayalet kasabaya yerleşen goblin 929, kalıcı köy olan vadi 10.

## Eski analizdeki sorunlar

Eski analiz: TS v0.23, 12 seed × 30 yıl ve 3 seed × 60 yıl (Proje: `analiz-5-ajan-oneriler.md`). "Sürüyor mu" kaba bir eşiktir: araştırma ağacı Faz 1b-3'te kaldırıldı; 30. yılda tam 5 kara yerleşimli medeniyet ≥ %50; kamp (30. yıl) < 0,75 × en yüksek yıl; altın (30. yıl) ≥ 10 × altın (1. yıl); boştaki iş gücü (30. yıl) ≥ %30; büyük olay (30. yıl) ≤ 0,6 × en yüksek yıl; 25. yıldan sonra doğanların ≥ %50'si Sv5+; hiç başkent kaybı yok.

| Bulgu | Eski analiz | Bu ölçüm | Sürüyor mu? |
|---|---|---|---|
| Araştırma ağacı erken bitiyor | ~19. yılda bitiyor; 30. yılda medeniyetlerin %98'i bitirmiş | ağaç ve çağlar kaldırıldı (Faz 1b-3); başlangıç medeniyetlerinin ilk kasabası medyan –. yılda (0/0), ilk şehri –. yılda (0/0); 30. yılda başkent kademesi ortalaması 2,69 | hayır |
| Medeniyetler 5 yerleşimde takılıyor | 98 medeniyetin 74'ü (%76) tam 5 yerleşimde | tam 5 kara yerleşimli medeniyet payı 30. yılda %0, 60. yılda %5,6 (denizaşırı koloniler dâhil 30. yılda tam 5: %2, 5+: %75); medeniyet başına 12,2 yerleşim (30. yıl) | hayır |
| Kamp sayısı düşüyor | 6,8'den 3,5'e iniyor | 8 (1. yıl) → en yüksek 10 (18. yıl) → 9 (30. yıl) → 9 (60. yıl); yıl sonu, yıllık dünya medyanı | hayır |
| Altın birikiyor | altın medyanı 78'den 6.503'e çıkıyor | 611 (1. yıl) → 356 (30. yıl) → 376 (60. yıl) | hayır |
| İş gücü boşta | iş gücünün %43'ü boşta | 30. yılda %19, 60. yılda %17 (işe yerleşemeyen `zanaatçı` / bütün iş gücü, askerler dâhil; yıl içi ortalama) | hayır |
| Büyük olaylar seyreliyor | yıllık büyük olay 59'dan 28'e düşüyor | en yüksek 288 (43. yıl) → 253 (30. yıl) → 250 (60. yıl), yıllık dünya medyanı | hayır |
| Doğuş seviyesi şişiyor | 24. yıldan sonra herkes Sv5 doğuyor; efsane mekaniği ölü | 25–60. yıllarda Sv5+ doğanların payı %0; on yıllık doğuş seviyesi ortalaması 1,62 · 1,62 · 1,58 · 1,61 · 1,6 · 1,65 | hayır |
| Başkent düşmüyor | başkent fethedilemiyor (agents.ts:673) | 16 dünyada 66 başkent kaybı, 43 yok olma; 1021 yerleşim fethi | hayır |

## On yıllık özet

Hücre: dünyalar arası medyan (p10–p90). Her dünyada on yılın yıllık değerlerinin ortalaması alınır: akış ölçülerinde yıllık ortalama, stok ölçülerinde yıl sonu değerlerinin ortalaması. Yüzdeler 0–1 paylardır.

| Ölçü | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| **Medeniyet** | | | | | | |
| Yaşayan medeniyet | 6 (5,5–6,85) | 6,1 (5,75–7) | 6 (5,55–7,05) | 6,25 (5,9–7,05) | 6,5 (6–7) | 6,7 (6–7,85) |
| Yeni medeniyet (yeniden doğan) | 0 (0–0,2) | 0 (0–0,1) | 0 (0–0,15) | 0 (0–0,1) | 0,05 (0–0,1) | 0 (0–0,1) |
| Yok olan medeniyet | 0 (0–0,1) | 0 (0–0,2) | 0 (0–0,15) | 0,05 (0–0,1) | 0 (0–0,15) | 0 (0–0,1) |
| Başkent kaybı (medeniyet yaşarken) | 0,1 (0–0,2) | 0,1 (0–0,2) | 0,1 (0–0,2) | 0,05 (0–0,1) | 0 (0–0,05) | 0 (0–0,1) |
| Çöküş (yok olma + başkent kaybı) | 0,1 (0,05–0,2) | 0,1 (0–0,35) | 0,1 (0–0,25) | 0,1 (0–0,25) | 0 (0–0,15) | 0,1 (0–0,2) |
| Yaşayan yerleşim | 80,1 (73,2–81,1) | 80,2 (73,6–81,2) | 80,1 (76,1–81,3) | 79,9 (76,9–81,4) | 79,8 (75,5–81,9) | 79,7 (75,3–81,8) |
| Medeniyet başına yerleşim | 13,1 (11,4–13,6) | 12,9 (11,1–13,5) | 12,8 (10,8–13,7) | 12,8 (11–13,7) | 12 (10,6–13,6) | 11,6 (9,9–13,4) |
| 5+ kara yerleşimli medeniyet payı | %83 (%69–%96) | %80 (%66–%83) | %78 (%65–%83) | %78 (%60–%85) | %70 (%57–%86) | %66 (%54–%88) |
| Kurulan yerleşim | 0,4 (0,3–1,3) | 0,1 (0–0,85) | 0,1 (0–0,55) | 0,1 (0–0,65) | 0,15 (0–1) | 0,15 (0–0,9) |
| Fethedilen yerleşim | 1,65 (0,6–2,05) | 1 (0,65–2,4) | 0,9 (0,15–1,75) | 0,85 (0,45–1,6) | 0,7 (0,25–1,05) | 0,65 (0,1–1,65) |
| Terk edilen yerleşim | 0,05 (0–1,4) | 0,1 (0–0,3) | 0,1 (0–0,35) | 0,1 (0–0,65) | 0,1 (0–1,15) | 0,1 (0–1) |
| Toplam nüfus | 2840 (2402–3129) | 2879 (2219–3137) | 2926 (2343–3213) | 2854 (2269–3269) | 2920 (2228–3364) | 2948 (2432–3371) |
| Altın medyanı (medeniyetler) | 423 (316–713) | 351 (240–499) | 381 (223–612) | 366 (150–687) | 402 (237–612) | 435 (236–553) |
| Boştaki iş gücü payı | %18 (%12–%24) | %20 (%13–%27) | %18 (%13–%25) | %20 (%13–%28) | %20 (%12–%34) | %20 (%16–%32) |
| Bölünme (ayrılıp kurulan medeniyet) | 0 (0–0,2) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) |
| En büyük medeniyetin yerleşimi | 21,1 (18,4–26,1) | 23,4 (18,6–30,3) | 23,4 (19,2–34,3) | 23,6 (18,8–36) | 22,7 (18,8–35,9) | 24,1 (18,7–37,8) |
| **Olaylar** | | | | | | |
| Olay | 1256 (837–1447) | 1366 (873–1526) | 1410 (915–1577) | 1393 (999–1613) | 1444 (1116–1751) | 1470 (1263–1758) |
| Büyük olay | 228 (134–332) | 231 (158–327) | 275 (152–325) | 236 (156–338) | 260 (155–363) | 259 (189–354) |
| **Savaş** | | | | | | |
| Muharebe | 19,4 (14,7–24) | 19,1 (13,7–24,3) | 19,2 (12,1–25,8) | 19,5 (10,9–23,1) | 20,3 (11–26,6) | 19,9 (10,1–25,8) |
| Başlayan savaş | 1,8 (1–2,55) | 1,45 (0,75–2,2) | 1,7 (0,4–2) | 1,3 (0,75–1,8) | 1,1 (0,3–1,45) | 1,3 (0–1,75) |
| Süren savaş (yıl sonu) | 1,05 (0,65–1,3) | 0,75 (0,4–1,4) | 0,9 (0,3–1,1) | 0,9 (0,5–1,25) | 0,55 (0,25–0,85) | 0,7 (0–1,15) |
| Yıl içinde süren savaş | 2,95 (1,65–3,8) | 2,6 (1,4–3,6) | 2,55 (0,85–3,15) | 2,3 (1,35–3,15) | 1,65 (0,9–2,35) | 1,9 (0–2,75) |
| Yağma akını (medeniyet) | 1,75 (0,5–4,05) | 1,4 (0–3,2) | 1,05 (0–2,45) | 1,05 (0–2,1) | 1 (0,1–2,25) | 0,9 (0–2) |
| Tarihî hak savaşı | 0,3 (0,1–0,45) | 0,2 (0–0,55) | 0,25 (0–0,4) | 0,2 (0–0,3) | 0,15 (0–0,4) | 0,1 (0–0,25) |
| Pakt gereği savaş | 0,2 (0–0,65) | 0,05 (0–0,45) | 0,15 (0–0,4) | 0,1 (0–0,45) | 0 (0–0,35) | 0 (0–0,3) |
| Kutsal Sefer çağrısı | 0,1 (0–0,25) | 0,1 (0–0,15) | 0,1 (0–0,3) | 0,1 (0–0,2) | 0 (0–0,1) | 0,1 (0–0,1) |
| İhanet (pakt çiğnendi) | 0 (0–0,1) | 0 (0–0,15) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) |
| Savunma paktı (yıl sonu) | 1,35 (0,2–2,25) | 1,2 (0,5–1,95) | 1,3 (0,85–2) | 1,3 (1–2) | 1 (0,55–2) | 1 (0,3–1,95) |
| **Canavarlar** | | | | | | |
| Yaşayan kamp (yıl sonu) | 8,45 (7,7–9,4) | 9 (8,3–9,7) | 9,1 (8,1–10,3) | 9,2 (8,25–9,85) | 8,95 (7,9–10,4) | 9,1 (8,1–10,1) |
| Yaşayan kamp (yıl ort.) | 8,29 (7,65–9,39) | 8,74 (8,23–9,73) | 8,93 (8,11–10,3) | 9,18 (7,94–9,93) | 9,08 (7,89–10,3) | 8,92 (7,96–10,2) |
| Doğan kamp | 7 (2,8–11) | 6,6 (3,2–10,6) | 5,8 (2,45–10,9) | 5,45 (1,85–11) | 6,35 (1,5–11,5) | 5,5 (1,6–10,9) |
| Temizlenen kamp | 7 (2,8–10,9) | 6,35 (3,25–10,7) | 5,9 (2,45–10,9) | 5,35 (1,8–11) | 6,4 (1,45–11,3) | 5,5 (1,75–10,8) |
| Canavar baskını | 3,45 (2,8–4,45) | 3,85 (3,25–5) | 3,9 (2,6–5,45) | 3,7 (2,2–5,7) | 3,8 (1,7–5,05) | 3,7 (2,05–5,2) |
| Yaşayan trol ini (yıl sonu) | 2 (1,55–2,7) | 2,25 (1,65–2,65) | 2,65 (1,7–2,85) | 2,65 (1,85–3) | 2,8 (2,15–3) | 2,9 (2,25–3) |
| Yaşayan ejderha (yıl sonu) | 0 | 0,5 (0,2–0,7) | 0,8 (0–1) | 0 (0–1) | 0 (0–1) | 0 (0–1) |
| Ejderha akını | 0 | 0,65 (0,1–0,9) | 0,6 (0–1,45) | 0 (0–1,55) | 0 (0–1,6) | 0 (0–1,5) |
| Kriz (anlatıcı) | 2,65 (1,75–3,4) | 3 (2–3,85) | 2,8 (1,8–3,2) | 2,85 (1,95–3,65) | 2,7 (1,95–3,65) | 2,5 (1,7–3,7) |
| Rahatlama dönemi (anlatıcı) | 0,1 (0,05–0,3) | 0,15 (0,1–0,4) | 0,15 (0–0,4) | 0,1 (0–0,3) | 0,1 (0–0,5) | 0,1 (0–0,3) |
| **Kahramanlar** | | | | | | |
| Doğan kahraman | 10,6 (5,75–15,3) | 11 (6,45–16,3) | 10,8 (6,8–17,3) | 11,3 (6,95–16,7) | 12,8 (7,6–16,2) | 13,3 (7,55–17,8) |
| Ölen kahraman | 3,1 (2,3–3,75) | 4,8 (2,85–6,2) | 4,3 (3,15–7,35) | 4,2 (2,95–6,5) | 4,75 (2,35–7,5) | 4,55 (2,55–7,9) |
| Emekli olan kahraman | 1,9 (1–2,85) | 1,75 (1,15–3,15) | 2,25 (1,45–2,9) | 2,45 (1,4–2,95) | 1,9 (1,25–2,6) | 2,2 (1,65–3,1) |
| Diyarı terk eden kahraman | 5,3 (0,95–9,25) | 4,7 (1,05–9,45) | 5,25 (1,65–9,8) | 6,3 (1,95–11,8) | 5,75 (2,05–10,3) | 7,55 (2,15–10,3) |
| Ölümden dönen kahraman | 0 | 0 | 0 | 0 | 0 | 0 |
| Efsane olan kahraman | 0 (0–0,1) | 0,05 (0–0,2) | 0,1 (0–0,2) | 0,1 (0–0,1) | 0 (0–0,1) | 0 (0–0,15) |
| Yaşayan kahraman (yıl sonu) | 145 (94,9–157) | 153 (96,7–162) | 159 (100–168) | 155 (110–175) | 153 (120–179) | 158 (135–183) |
| Doğuş seviyesi (ort.) | 1,61 (1,5–1,72) | 1,62 (1,53–1,72) | 1,59 (1,46–1,69) | 1,61 (1,55–1,67) | 1,62 (1,55–1,73) | 1,68 (1,55–1,75) |
| Ölüm seviyesi (ort.) | 4,17 (3,55–5,33) | 4,65 (4,21–4,93) | 4,82 (4,2–5,53) | 5,05 (4,28–5,56) | 4,84 (4,09–5,22) | 4,72 (3,67–5,01) |
| Yaşayan kahraman seviyesi (ort.) | 4,74 (4,15–5,31) | 4,51 (4,24–5,38) | 4,66 (4,18–5,35) | 4,46 (4,25–5,57) | 4,53 (4,16–5,41) | 4,59 (4,18–5,36) |
| En yüksek seviye (şimdiye dek) | 10 | 10 | 10 | 10 | 10 | 10 |
| **Han ve ticaret** | | | | | | |
| Ayakta han | 4 (2,5–5,85) | 4,3 (2,45–5,8) | 4,25 (2,5–6) | 4,25 (2,9–5,95) | 4,75 (3–5,9) | 4,9 (3–6,35) |
| Asılan ilan | 17 (6,7–25,1) | 16,2 (9,75–24,2) | 18,5 (9,1–24,2) | 15,9 (7,65–24) | 18,4 (8,4–25) | 17,5 (11–27,2) |
| Biten ilan | 8,7 (2,95–13,9) | 7,35 (4,25–13,6) | 6,6 (2,9–12,8) | 6,55 (3,25–12,9) | 7,8 (3,3–14,4) | 7,85 (3,75–13,8) |
| Ticaret seferi (kervan) | 85,5 (61,6–119) | 81,8 (63,6–123) | 104 (69,4–117) | 104 (66,3–137) | 104 (60,6–134) | 116 (72,2–148) |
| İkmal seferi | 34,1 (23,5–49,7) | 32,7 (20,8–56,9) | 40,3 (24,1–55,6) | 41,8 (23,3–58,9) | 37 (27,5–62,3) | 37,5 (31,4–57,1) |
| **Altın ve ambar** | | | | | | |
| Altın p90 (medeniyetler) | 1160 (973–1290) | 1153 (1000–1574) | 1190 (846–1436) | 1344 (1097–1804) | 1416 (1128–1665) | 1500 (1249–1686) |
| Bakım gideri (altın; asker, kahraman, L2–L3) | 2889 (2027–3245) | 2728 (1873–3203) | 2629 (2038–3275) | 2633 (1964–3142) | 2326 (2007–3029) | 2379 (1792–3118) |
| Kamu işlerine (imar) harcanan altın | 2130 (1366–2649) | 2146 (1303–2978) | 2028 (1255–2821) | 2393 (1699–3246) | 2409 (1754–3341) | 2879 (2165–3496) |
| Ambarla beslenen amele tayını (gıda) | 937 (316–2376) | 714 (93,6–2049) | 532 (84–1697) | 329 (76–1409) | 367 (24,9–1357) | 171 (3,94–819) |
| Kamu işlerindeki (amele) iş gücü payı | %15 (%12–%19) | %14 (%10–%16) | %13 (%9,6–%17) | %13 (%10–%15) | %13 (%11–%14) | %13 (%9,9–%18) |
| İmar ortalaması (köy+, 0–100) | 18,1 (14,8–22,1) | 17 (13,8–21,9) | 16,1 (11,7–22,5) | 15,6 (12,9–25,5) | 16,7 (13,1–28,3) | 17,3 (12–21,5) |
| Canavar baskınında yitirilen altın | 11,1 (5,4–19,4) | 12,4 (3,7–19,7) | 10,1 (2,9–17,3) | 8,7 (1,1–19,1) | 8,15 (1,5–18,7) | 10,9 (1,5–24,9) |
| Ejderhaya giden altın (haraç + akın) | 0 | 79 (12,1–243) | 94 (0–646) | 0 (0–575) | 0 (0–404) | 0 (0–426) |
| Hazinesi boş medeniyet payı | %0 | %0 | %0 | %0 | %0 | %0 |
| Kent tüketiminde yokluk payı (köy+; ekmek, bira ya da alet) | %31 (%25–%37) | %34 (%26–%39) | %33 (%26–%43) | %33 (%21–%42) | %34 (%20–%48) | %34 (%21–%42) |
| Ekmek ya da bira yokluğu payı (köy+) | %6,4 (%3–%16) | %6,8 (%2,9–%18) | %6,2 (%1,7–%19) | %8,7 (%1,8–%21) | %10 (%1,9–%30) | %13 (%2,4–%22) |
| Kıtlık (büyük olay) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,2) | 0 (0–0,15) | 0 (0–0,15) |
| Açlıktan ölen | 0 (0–8,55) | 0 (0–2,75) | 0 (0–3) | 0,05 (0–6,35) | 0 (0–9,1) | 0 (0–6,85) |
| Kıtlık yardımı (sevkiyat) | 0 (0–0,4) | 0 (0–0,35) | 0 (0–0,4) | 0 (0–0,55) | 0 (0–0,65) | 0 (0–0,55) |
| Kıtlıkta yüz çeviren | 0 (0–0,3) | 0 (0–0,15) | 0 (0–0,15) | 0 (0–0,6) | 0 (0–0,25) | 0 (0–0,35) |
| Kıtlık akını | 0 (0–0,1) | 0 | 0 | 0 | 0 (0–0,05) | 0 (0–0,15) |
| Ambarın yettiği gün (medeniyet medyanı) | 38,7 (26,5–54,1) | 35,7 (14,9–46,4) | 36,1 (12,3–53,2) | 34,7 (13,3–46,2) | 32,7 (10,1–44,1) | 25,3 (10,3–44,5) |
| **Yerleşim kademesi** | | | | | | |
| Ortalama yerleşim kademesi | 1,06 (0,89–1,21) | 1,06 (0,82–1,24) | 1 (0,82–1,32) | 1,03 (0,84–1,32) | 1,03 (0,83–1,3) | 1,04 (0,81–1,27) |
| Köy+ yerleşim | 60,3 (40,5–69,6) | 59,3 (38,7–68) | 56,7 (41,9–71,4) | 59,5 (44,8–69,8) | 60,6 (44,1–67,4) | 59,1 (43,9–68,9) |
| Kasaba+ yerleşim | 19,5 (11,9–24,3) | 19,1 (12–24,2) | 18,1 (11,5–28,4) | 17,1 (10,1–29,7) | 16,4 (10,7–26,2) | 17 (11,2–26,5) |
| Şehir | 5,85 (4,15–7,9) | 6,35 (4,75–8,85) | 5,95 (4,85–7,9) | 5,75 (4,1–6,9) | 5,85 (3,95–7,6) | 5,9 (4,45–8,35) |
| Ortalama başkent kademesi | 2,78 (2,55–2,93) | 2,77 (2,46–2,87) | 2,72 (2,52–2,89) | 2,63 (2,42–2,86) | 2,8 (2,39–2,89) | 2,69 (2,58–2,88) |
| Kademe değişimi (yerleşim, yıl içinde) | 26,8 (19,8–34,3) | 24,8 (17,1–30,6) | 23,5 (19–30,6) | 23,5 (17,6–28,4) | 22 (15,2–27,7) | 20,7 (12,5–25,5) |
| **Deniz** | | | | | | |
| Liman (tersane) | 21,5 (14,7–25,5) | 21,7 (15,9–26,5) | 23,6 (16,9–27,3) | 23,6 (17,3–26,7) | 23,6 (18–26) | 23,8 (18,2–25,8) |
| Gemi (koga/tekne) | 43,8 (30,5–51,8) | 45 (35,4–57,4) | 49 (39,4–58,2) | 50,9 (42–60,1) | 52,1 (41,3–62,3) | 54 (44,6–66,2) |
| Kadırga | 21 (16,7–26,8) | 20 (17,1–25,2) | 20,3 (16,1–23,7) | 19,6 (15,4–23,2) | 19,5 (14,6–21,7) | 19 (14–21,6) |
| Denizaşırı yerleşim | 7,5 (5,5–10,2) | 7,5 (5,5–9,5) | 7,9 (5,5–9,5) | 7,5 (5,5–9,5) | 7,5 (5,5–9,35) | 7,05 (5,5–9) |
| Deniz ticaret yolu (yıl sonu) | 16,1 (8,5–19,8) | 16,6 (8,8–21,9) | 16,7 (10,2–23,3) | 17,5 (11,4–24,5) | 18,1 (12,1–25,3) | 20,5 (11,2–29,1) |
| Deniz seferi (ticaret) | 25,3 (11,3–36) | 30 (14,5–37,4) | 29,1 (16,8–42,5) | 29,4 (18,4–48,8) | 29,2 (20,1–51,2) | 32,6 (20,8–62) |
| **v3: durum değişimi** | | | | | | |
| Yaşayan yerleşim (yıl ort.) | 78,8 (72,4–79,9) | 78,8 (72–79,9) | 79 (75,1–80) | 78,9 (75,7–80) | 78,5 (74,6–80,1) | 78,7 (74,4–80,5) |
| Büyük şehir (Şehir kademesi, yıl ort.) | 5,92 (4,5–7,9) | 6,19 (4,77–8,87) | 6,04 (4,87–7,96) | 5,7 (4,16–6,94) | 5,8 (4–7,7) | 6,01 (4,44–8,2) |
| El değiştiren yerleşim (fetih + bölünme) | 2,3 (1,15–3,4) | 2,35 (1,3–3,6) | 2,15 (1,2–2,7) | 2,05 (1–3,2) | 1,65 (1,1–3,1) | 1,85 (1–3,05) |
| El değiştiren büyük şehir | 0,8 (0,3–1,6) | 1,25 (0,45–1,8) | 1,1 (0,55–1,6) | 1 (0,35–1,5) | 1,05 (0,45–1,7) | 0,9 (0,65–1,65) |
| Büyük şehre hücum (kuşatma muharebesi) | 0,4 (0,1–0,7) | 0,35 (0,05–0,7) | 0,3 (0,05–0,9) | 0,3 (0,1–0,65) | 0,2 (0–0,7) | 0,2 (0–0,7) |
| Büyük şehir yağmalandı, tutulmadı | 0,1 (0–0,3) | 0,15 (0–0,35) | 0,1 (0–0,4) | 0,1 (0–0,5) | 0,1 (0–0,4) | 0,1 (0–0,2) |
| Yerleşim durum değişimi (kuruluş hariç hepsi) | 42,8 (35,1–50,8) | 39,4 (31,6–49,3) | 36,8 (29,7–49,4) | 36,8 (31,3–48,9) | 33,6 (28,5–50,5) | 33,4 (23,9–42,8) |
| Orta halka kaydı (Köy/Kasaba: kademe değişimi ya da terk) | 19,5 (14–25,9) | 17,6 (10,9–22,8) | 17,3 (12,8–22,5) | 16,6 (12,3–21,8) | 15,3 (10,5–21,3) | 14,5 (8,35–21,1) |
| Açlık başlayan yerleşim | 0,05 (0–5,6) | 2,4 (0–8) | 0,5 (0–4,35) | 0,5 (0–14,3) | 1,1 (0–15,8) | 0,85 (0–11,5) |
| Salgın başlayan yerleşim | 0,6 (0,25–1,05) | 0,3 (0,15–0,85) | 0,6 (0,2–1,15) | 0,6 (0,25–1,1) | 0,5 (0,25–0,7) | 0,65 (0,25–0,95) |
| Yakılan/yanan yerleşim | 8,05 (3,65–12,2) | 7 (5–12,7) | 6,25 (4,35–12,1) | 6,6 (3,7–9,45) | 6,05 (2,9–8,85) | 5,8 (2,75–8,7) |
| Harabeye yeniden yerleşim | 0,2 (0–0,6) | 0,05 (0–0,45) | 0,05 (0–0,35) | 0,1 (0–0,55) | 0,05 (0–0,95) | 0,1 (0–0,65) |
| **Devlet, inanç ve örgüt** | | | | | | |
| Yaşayan örgüt (yıl sonu) | 13 (12,3–13) | 13 (12,2–13) | 12,8 (12,2–13) | 12,9 (12,2–13) | 12,7 (12,2–13) | 12,5 (12,1–13) |
| Örgüt şubesi (yıl sonu) | 294 (220–337) | 302 (217–349) | 301 (239–368) | 317 (247–373) | 313 (267–364) | 324 (283–380) |
| Gizli şube (yıl sonu) | 12,9 (8,85–18) | 8,85 (5,9–13,2) | 9,1 (6,55–11,9) | 7,95 (5,1–10,9) | 7,6 (4,1–10,4) | 7,45 (4,65–11) |
| Örgüt üyesi (yıl sonu) | 1418 (1110–1541) | 1473 (1088–1603) | 1577 (1143–1660) | 1534 (1240–1706) | 1619 (1290–1784) | 1685 (1387–1839) |
| Açılan şube | 17,4 (15,7–19,3) | 16,3 (13,7–18,4) | 15,6 (14–17,2) | 14,9 (13,7–18,4) | 14,9 (12,7–16,9) | 14,4 (12,8–17,2) |
| Kapanan şube | 18,7 (15,5–24) | 14,4 (12,9–16,5) | 14,4 (11,5–16,9) | 14,4 (11,2–18) | 14,2 (11,1–16,8) | 13,8 (11,5–16,7) |
| Dağılan örgüt | 0 (0–0,15) | 0 (0–0,15) | 0 (0–0,1) | 0 (0–0,1) | 0,05 (0–0,1) | 0,1 (0–0,1) |
| Yeniden kurulan örgüt | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0,05 (0–0,1) | 0 (0–0,15) | 0,1 (0–0,1) |
| Gölge savaşı eylemi | 7,9 (6,4–9,6) | 7,75 (5,25–9,15) | 6,6 (5,4–8,8) | 7,8 (5,05–9,5) | 6,7 (5,3–8,85) | 6,6 (4,7–8,35) |
| Gölge savaşında öldürülen usta ya da lider | 1,8 (1,25–2,25) | 1,55 (0,85–2,1) | 1,5 (1–2,25) | 1,7 (1,25–2,3) | 1,6 (1,05–2,2) | 1,45 (0,8–2,1) |
| Gizli şubeye baskın | 0,9 (0,4–1,2) | 0,55 (0,2–1,1) | 0,5 (0,3–1,15) | 0,55 (0,2–1,1) | 0,5 (0,15–1) | 0,6 (0,2–1,15) |
| Lobiyle yasa değişikliği | 1,05 (0,65–2) | 1 (0,35–2,85) | 1,1 (0,6–1,85) | 1,3 (0,75–1,8) | 1,3 (0,2–1,95) | 1,05 (0,25–1,95) |
| Darbe girişimi | 0 (0–0,1) | 0 (0–0,15) | 0 (0–0,05) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,05) |
| Örgüt ilanı (Avcılar) | 1,1 (0,65–1,7) | 0,75 (0,55–2,3) | 1 (0,45–1,95) | 0,95 (0,55–2,75) | 0,7 (0,4–2,3) | 0,75 (0,55–2,3) |
| Örgüt üyesi kahraman payı (yıl sonu) | %100 (%99–%100) | %99 (%98–%100) | %99 (%98–%100) | %100 (%98–%100) | %100 (%98–%100) | %100 (%99–%100) |
| Yönetici değişimi | 0,7 (0,5–1,55) | 1,15 (0,65–1,55) | 1,15 (0,8–1,65) | 1,2 (0,5–1,8) | 1,05 (0,5–1,85) | 1,25 (0,8–2) |
| Veraset krizi | 0 | 0 (0–0,1) | 0 (0–0,05) | 0 (0–0,05) | 0 (0–0,05) | 0 (0–0,1) |
| Meşruiyet ortalaması (yıl sonu) | 66,2 (62,9–70,4) | 65,7 (60,5–69,9) | 67 (60,7–71) | 66,8 (59,2–72,4) | 68,5 (62,6–72,5) | 66,6 (58,4–72,7) |
| Pakt'a bağlı yönetici (yıl sonu) | 0,1 (0–0,25) | 0,1 (0–0,5) | 0,05 (0–0,25) | 0 (0–0,25) | 0,05 (0–0,3) | 0 (0–0,1) |
| Köle (yıl sonu) | 9,6 (2,5–39,4) | 9,65 (1,1–32,9) | 8,2 (0,75–28,4) | 6,25 (1,3–27,7) | 7 (1,4–42,6) | 9,75 (1,05–31,8) |
| Köle payı (nüfusun, yıl sonu) | %0,3 (%0,1–%1,7) | %0,3 (%0–%1,2) | %0,3 (%0–%1) | %0,2 (%0–%1) | %0,3 (%0–%1,6) | %0,4 (%0–%1,4) |
| Hapis madeninde mahkûm (yıl sonu) | 0,65 (0–1,75) | 0,6 (0–2,2) | 0,2 (0–2,45) | 0,45 (0–2,45) | 0,35 (0–4,4) | 0,25 (0–3,4) |
| Esarete düşen | 6,2 (3,3–10,5) | 5,85 (0,9–12) | 5,75 (1,1–10,4) | 5,3 (1,35–11,8) | 5,05 (1,85–10,4) | 5,75 (2,3–11,5) |
| Kurtulan köle (Özgürlük Ağı, kaçış, azat) | 8,65 (3,75–10,5) | 6,6 (1–12,1) | 6,65 (0,7–10,2) | 5,75 (1,35–11,2) | 4,6 (1,9–12,5) | 5,5 (2,3–11,2) |
| Özgürlük Ağı'nın kurtardığı köle | 6,2 (3,1–8,85) | 5,15 (0,55–9,1) | 5,55 (0,6–8,7) | 5,2 (1,25–8,6) | 4,4 (1,8–9,3) | 4,9 (1,65–9,35) |
| Esir kahraman (yıl sonu) | 0,25 (0–0,8) | 0,2 (0–1,35) | 0,25 (0–1) | 0,1 (0–0,8) | 0,15 (0–0,95) | 0,2 (0–1,5) |
| Aç haydut kampı (yıl sonu) | 0,7 (0,15–1,5) | 0,55 (0,2–1,4) | 0,45 (0,1–1,25) | 0,4 (0,1–1,35) | 0,9 (0,15–2,05) | 0,9 (0,35–1,9) |
| Haydut olan aç halk | 11,4 (6,05–21,8) | 12,8 (5,05–24) | 9,3 (5,7–29,4) | 10,3 (4,85–28,2) | 15,7 (4,95–38,4) | 17,2 (7,25–37,6) |
| Aç ya da ekmeksiz yerleşim payı (köy+) | %4,3 (%1,9–%11) | %4,4 (%2–%15) | %4,1 (%2,2–%13) | %4,1 (%1,8–%12) | %5,8 (%1,8–%18) | %8,1 (%2,3–%18) |
| Devriye durdurması | 50 (27–62,8) | 54,9 (29,3–61,8) | 56,4 (28,5–68,4) | 59,9 (34,4–63,6) | 56,4 (42,1–64,2) | 55,8 (39,1–68,3) |

## Kahraman seviyeleri

### Doğuş seviyesi (bütün dünyalar, on yıl içinde doğanlar)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 |
|---|---|---|---|---|---|
| 1–10 | 1685 | 1,62 | %47 | %43 | %9,7 |
| 11–20 | 1781 | 1,62 | %47 | %43 | %9,7 |
| 21–30 | 1877 | 1,58 | %51 | %40 | %9,1 |
| 31–40 | 1890 | 1,61 | %47 | %46 | %7,6 |
| 41–50 | 1965 | 1,6 | %48 | %43 | %8,4 |
| 51–60 | 2069 | 1,65 | %46 | %43 | %11 |

### Ölüm seviyesi (bütün dünyalar, on yıl içinde ölenler)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 | Sv4 | Sv5 | Sv6 | Sv7 | Sv8 | Sv9 | Sv10 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 1–10 | 498 | 4,33 | %7,6 | %14 | %18 | %21 | %14 | %9 | %4,2 | %5,4 | %4,4 | %2,2 |
| 11–20 | 742 | 4,69 | %6,3 | %9,8 | %14 | %21 | %20 | %11 | %6,2 | %5,4 | %3 | %4,2 |
| 21–30 | 789 | 4,91 | %7,4 | %9,3 | %13 | %17 | %19 | %11 | %7 | %6,2 | %3,9 | %6,5 |
| 31–40 | 720 | 4,74 | %6,7 | %8,9 | %18 | %16 | %19 | %11 | %4,9 | %5,4 | %3,3 | %5,8 |
| 41–50 | 820 | 4,64 | %7,6 | %9 | %18 | %16 | %21 | %9,8 | %5,4 | %5,2 | %3,2 | %4,9 |
| 51–60 | 798 | 4,44 | %7,1 | %9,6 | %22 | %18 | %18 | %9,3 | %4,8 | %6 | %2,3 | %3,4 |

### Yaşayan kahramanların seviyesi (bütün dünyalar, on yılın son yılının sonunda)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 | Sv4 | Sv5 | Sv6 | Sv7 | Sv8 | Sv9 | Sv10 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 10 | 2211 | 4,82 | %2,6 | %9,3 | %16 | %26 | %17 | %8 | %6,1 | %4,2 | %3,9 | %6,3 |
| 20 | 2256 | 4,82 | %3,3 | %7,8 | %18 | %25 | %18 | %7,8 | %5,5 | %4,4 | %3,5 | %6,6 |
| 30 | 2353 | 4,77 | %3,3 | %9 | %17 | %23 | %21 | %7,3 | %5,9 | %4,5 | %2,5 | %6,3 |
| 40 | 2376 | 4,77 | %2,6 | %8,1 | %18 | %25 | %21 | %7,2 | %5,1 | %4 | %2,6 | %6,5 |
| 50 | 2468 | 4,74 | %3 | %9 | %17 | %24 | %21 | %8,1 | %4,6 | %3,7 | %2,4 | %6,6 |
| 60 | 2544 | 4,72 | %3,5 | %8,2 | %17 | %23 | %23 | %9 | %4,5 | %3,6 | %2,2 | %5,8 |

### Ölüm nedenleri (bütün dünyalar)

| Neden | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 | Toplam |
|---|---|---|---|---|---|---|---|
| Kamp saldırısı | 165 | 220 | 160 | 156 | 214 | 178 | 1093 (%25) |
| Trol | 152 | 168 | 185 | 116 | 225 | 207 | 1053 (%24) |
| Bilinmiyor | 47 | 142 | 205 | 186 | 181 | 178 | 939 (%22) |
| Kuşatma | 67 | 90 | 99 | 116 | 83 | 60 | 515 (%12) |
| Ejderha | 0 | 57 | 71 | 73 | 52 | 84 | 337 (%7,7) |
| Düello | 39 | 28 | 37 | 34 | 39 | 42 | 219 (%5) |
| Kervan soygunu | 17 | 14 | 21 | 21 | 16 | 35 | 124 (%2,8) |
| Han baskını (canavar) | 3 | 6 | 8 | 4 | 6 | 7 | 34 (%0,8) |
| Yağma akını | 6 | 10 | 2 | 8 | 1 | 6 | 33 (%0,8) |
| Han baskını (medeniyet) | 1 | 7 | 0 | 6 | 2 | 0 | 16 (%0,4) |
| Yol pususu | 1 | 0 | 1 | 0 | 1 | 1 | 4 (%0,1) |

Neden, ölümün kaydedildiği andaki son muharebenin türünden (başlık ve taraflar) ya da suikast olayından çıkarılır.

## Olay türleri

Dünya başına yıllık olay sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Kahraman (`hero`) | 767 (508–841) | 829 (530–949) | 919 (588–1019) | 921 (633–1074) | 899 (745–1136) | 967 (843–1186) |
| Sefer/ilan (`quest`) | 149 (58,6–264) | 142 (83,7–239) | 174 (77,1–241) | 127 (66,7–255) | 179 (63,4–284) | 165 (106–267) |
| state | 99,4 (87,3–109) | 102 (84,9–106) | 104 (83,5–108) | 104 (92,3–111) | 102 (83–115) | 98,4 (91,5–112) |
| org | 60,5 (55,4–64,8) | 53,4 (46,7–59,3) | 51,7 (46,3–57,8) | 53,3 (46,1–60,4) | 51,2 (45,5–56,5) | 51 (44,3–55,8) |
| Göç (`migration`) | 38,5 (30,2–45,6) | 37,5 (28,1–47) | 38,2 (24,6–47,2) | 35,8 (24,7–46,4) | 33,6 (22,7–52,8) | 35,1 (22–47,3) |
| Kamp (`lair`) | 28,1 (11,2–42,7) | 26,1 (14,7–40,6) | 27,3 (13,4–43) | 25,4 (10,9–42,8) | 30,5 (11,4–48,1) | 31,6 (13,1–41,5) |
| Han (`inn`) | 13,1 (6,9–19,3) | 14,3 (7,9–19,5) | 14,2 (8,65–20,5) | 15,5 (9,55–20,3) | 15,5 (8,8–20,2) | 16,3 (10,1–22,9) |
| hub | 12,3 (11,4–13,9) | 13 (11,4–13,8) | 12,6 (10,7–13,4) | 12,6 (10,7–13,9) | 12,8 (11,1–13,9) | 11,9 (10,7–13,7) |
| İnşaat (`build`) | 17,5 (11,8–28,4) | 13,5 (10,6–23,5) | 11,4 (6,95–16,9) | 11 (6,4–15,7) | 8,7 (5,5–17,6) | 8,95 (5–20,4) |
| Savaş (`war`) | 15,1 (8,3–25,8) | 12,1 (5,15–20,7) | 12,2 (2,9–17,5) | 11,7 (4,35–16,9) | 8,5 (2,5–16,5) | 9,9 (0–17) |
| Ölüm/terk (`death`) | 5,8 (5,1–7,3) | 7,5 (5,6–8,95) | 7,15 (5,85–9,9) | 6,75 (5,6–9,65) | 8,35 (5,35–10,3) | 7,4 (5,05–10,6) |
| politics | 6,15 (4,35–7,75) | 6,8 (5,2–8,55) | 6,15 (4,9–7,9) | 6,55 (4,6–8,75) | 6,6 (4,1–8,05) | 5,65 (4,4–9,2) |
| Ekonomi (`economy`) | 6,4 (4,75–8,85) | 5,85 (3,9–8,6) | 5,95 (3,75–7,8) | 5,35 (3,6–7,4) | 4,65 (3,4–6,65) | 4,35 (3,3–5,6) |
| Deniz (`sea`) | 6,1 (1,65–10,4) | 5,85 (2,6–8,7) | 5 (2,05–8,35) | 4,65 (1,7–6,55) | 3 (1,15–6,15) | 3,75 (1,6–7,7) |
| epitaph | 3,1 (2,3–3,75) | 4,8 (2,85–6,2) | 4,3 (3,15–7,35) | 4,2 (2,95–6,5) | 4,75 (2,35–7,5) | 4,55 (2,55–7,9) |
| Baskın (`raid`) | 3,75 (2,95–5,1) | 3,45 (2,7–4,5) | 3,25 (2,65–4,45) | 3,7 (2,3–5,65) | 4,1 (1,8–4,9) | 3,7 (2,05–4,45) |
| Büyüme (`growth`) | 3,75 (2,65–4,65) | 3,05 (2,55–4,3) | 2,95 (2,1–3,45) | 2,7 (2,35–3,55) | 2,4 (2,05–3,6) | 2,65 (1,45–3,3) |
| Dünya (`world`) | 2,75 (1,55–3,2) | 1,75 (1,45–2,65) | 2,3 (1,35–3,75) | 2,5 (1,65–3,4) | 2,15 (1,45–2,6) | 2,4 (1,45–3,45) |
| Sınıf (`class`) | 1 (0,6–1,8) | 1 (0,25–2) | 1 (0,1–2,4) | 1 (0–2,05) | 1,2 (0–2,65) | 1,25 (0–2,85) |
| Diplomasi (`diplomacy`) | 1,05 (0,25–1,65) | 1,05 (0,2–2,05) | 0,85 (0,2–2,3) | 0,75 (0,1–2,35) | 0,55 (0,25–3,3) | 0,75 (0,2–2,5) |
| Gerginlik (`tension`) | 0,95 (0,15–2) | 0,9 (0,15–1,95) | 0,8 (0,2–1,35) | 0,45 (0,15–1,95) | 0,35 (0,05–2,7) | 0,6 (0,15–2,55) |
| Ticaret (`trade`) | 0,65 (0,2–2,05) | 0,85 (0,2–1,9) | 0,6 (0,25–1,85) | 0,6 (0–1,1) | 0,55 (0,1–1,4) | 0,5 (0–1,95) |
| Keşif (`discover`) | 1,05 (0,25–1,65) | 0,45 (0,1–1,4) | 0,5 (0,05–1,25) | 0,5 (0,1–1,45) | 0,35 (0,1–1,55) | 0,3 (0–0,8) |
| Ejderha (`dragon`) | 0 | 1,4 (0,65–1,85) | 1,7 (0–2,75) | 0 (0–2,9) | 0 (0–2,95) | 0 (0–2,8) |
| Yerleşim (`settle`) | 1,15 (0,6–2,9) | 0,35 (0–1,95) | 0,15 (0–1,25) | 0,2 (0–1,6) | 0,3 (0–2,4) | 0,3 (0–2,05) |
| Kriz (anlatıcı) (`crisis`) | 0,25 (0–0,55) | 0,2 (0–0,6) | 0,15 (0–0,55) | 0,25 (0–0,65) | 0,2 (0–0,75) | 0,15 (0–0,8) |
| Rahatlama (anlatıcı) (`relief`) | 0,1 (0,05–0,3) | 0,15 (0,1–0,4) | 0,1 (0–0,4) | 0,1 (0–0,3) | 0,1 (0–0,5) | 0,1 (0–0,3) |
| Temas (`contact`) | 0 (0–0,15) | 0 (0–0,1) | 0 (0–0,05) | 0 | 0 (0–0,15) | 0 (0–0,1) |

## Büyük olay türleri

Dünya başına yıllık büyük olay sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Sefer/ilan (`quest`) | 97,5 (38–176) | 94,8 (55,3–159) | 118 (51,3–160) | 85,7 (43,8–168) | 120 (43,2–187) | 110 (69,2–180) |
| Kahraman (`hero`) | 64,2 (41,6–73,6) | 73,7 (45,8–78,8) | 75,5 (51,7–86,8) | 74,9 (51,9–92,2) | 78,6 (58,6–92,8) | 79,6 (65,1–99,5) |
| Kamp (`lair`) | 20,1 (7,9–29,6) | 18,7 (10,5–29,1) | 17,9 (8,9–30,9) | 17,6 (7,25–29,9) | 21,6 (7,75–34,4) | 21,9 (8,75–29,2) |
| Savaş (`war`) | 8,65 (5–16,1) | 8,05 (3,05–13,3) | 7,9 (1,75–11,4) | 7,6 (3,05–10) | 6,35 (1,85–9) | 6,55 (0–9,55) |
| Ölüm/terk (`death`) | 5,8 (5,1–7,3) | 7,5 (5,6–8,95) | 7,15 (5,85–9,9) | 6,75 (5,6–9,65) | 8,35 (5,35–10,3) | 7,4 (5,05–10,6) |
| epitaph | 3,1 (2,3–3,75) | 4,8 (2,85–6,2) | 4,3 (3,15–7,35) | 4,2 (2,95–6,5) | 4,75 (2,35–7,5) | 4,55 (2,55–7,9) |
| politics | 3,4 (2–4,75) | 4,25 (2,55–5,55) | 3,75 (2,7–4,55) | 3,7 (2,3–5,4) | 3,6 (2,55–4,95) | 3,7 (2,3–5,4) |
| Deniz (`sea`) | 4,05 (0,9–7,45) | 3,25 (1,85–6,4) | 3,55 (1,6–6,25) | 3,45 (1,35–5,1) | 2,2 (0,8–4,2) | 2,7 (0,95–4,75) |
| Baskın (`raid`) | 3,25 (2,4–4,15) | 3 (2,05–4) | 3,05 (2,15–4) | 3,05 (2,1–5,1) | 3,4 (1,6–4,25) | 3,15 (1,7–3,95) |
| org | 4,3 (3,45–5,8) | 3,05 (2,35–3,95) | 2,8 (2,35–4,55) | 3,05 (1,55–4,4) | 2,6 (2,3–3,55) | 2,55 (1,6–3,5) |
| Han (`inn`) | 2,4 (1,7–4,3) | 2,85 (1,8–3,75) | 3,35 (2,4–4,35) | 3,2 (2,2–4,35) | 2,75 (1,95–4,4) | 3,1 (1,8–4,55) |
| hub | 2,4 (2,3–2,8) | 2,6 (2,2–2,8) | 2,5 (2–2,65) | 2,5 (2,1–2,8) | 2,5 (2,15–2,8) | 2,35 (2,05–2,75) |
| Dünya (`world`) | 1,9 (1,1–2,6) | 1,45 (1,2–2,3) | 1,8 (1,1–2,95) | 2,05 (1,4–2,35) | 1,75 (1,2–1,95) | 1,9 (1,15–2,65) |
| İnşaat (`build`) | 1,45 (0,9–2,5) | 1,1 (0,85–2,05) | 0,7 (0,35–1,7) | 0,6 (0,3–1,35) | 0,55 (0,2–1,5) | 0,6 (0,25–1,2) |
| Gerginlik (`tension`) | 0,95 (0,15–2) | 0,9 (0,15–1,95) | 0,8 (0,2–1,35) | 0,45 (0,15–1,95) | 0,35 (0,05–2,7) | 0,6 (0,15–2,55) |
| Diplomasi (`diplomacy`) | 0,6 (0,1–1,3) | 0,7 (0,2–1,3) | 0,6 (0,2–1,6) | 0,4 (0,05–1,75) | 0,35 (0,05–2,4) | 0,55 (0,2–1,85) |
| Ejderha (`dragon`) | 0 | 1,4 (0,65–1,85) | 1,7 (0–2,75) | 0 (0–2,9) | 0 (0–2,95) | 0 (0–2,8) |
| Göç (`migration`) | 0,4 (0,25–0,8) | 0,3 (0,2–0,5) | 0,3 (0,05–0,55) | 0,25 (0,15–0,7) | 0,3 (0,1–0,95) | 0,3 (0,15–0,75) |
| Ticaret (`trade`) | 0,2 (0,1–0,9) | 0,35 (0,05–0,85) | 0,2 (0,05–1) | 0,25 (0–0,5) | 0,25 (0–0,55) | 0,25 (0–0,75) |
| Keşif (`discover`) | 0,4 (0,1–0,95) | 0,25 (0–0,6) | 0,2 (0–0,65) | 0,2 (0–0,6) | 0,15 (0–0,7) | 0,1 (0–0,4) |
| Kriz (anlatıcı) (`crisis`) | 0,25 (0–0,55) | 0,2 (0–0,6) | 0,15 (0–0,55) | 0,25 (0–0,65) | 0,2 (0–0,75) | 0,15 (0–0,8) |
| Ekonomi (`economy`) | 0,2 (0,05–0,75) | 0,25 (0,1–0,55) | 0,2 (0–0,3) | 0,15 (0–0,4) | 0,1 (0–0,35) | 0,15 (0–0,45) |
| Yerleşim (`settle`) | 0,4 (0,3–1,3) | 0,1 (0–0,85) | 0,1 (0–0,55) | 0,1 (0–0,65) | 0,15 (0–1) | 0,15 (0–0,9) |
| Rahatlama (anlatıcı) (`relief`) | 0,1 (0,05–0,3) | 0,15 (0,1–0,4) | 0,1 (0–0,4) | 0,1 (0–0,3) | 0,1 (0–0,5) | 0,1 (0–0,3) |
| Büyüme (`growth`) | 0,1 (0–0,25) | 0,1 (0–0,2) | 0 (0–0,15) | 0 (0–0,15) | 0 (0–0,15) | 0,05 (0–0,4) |
| Sınıf (`class`) | 0 (0–0,35) | 0 | 0 | 0 | 0 | 0 |
| Temas (`contact`) | 0 (0–0,15) | 0 (0–0,1) | 0 (0–0,05) | 0 | 0 (0–0,15) | 0 (0–0,1) |

## Muharebe türleri

Dünya başına yıllık muharebe sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Kamp saldırısı (`camp`) | 9,5 (4,35–12,9) | 8,5 (6,2–12,6) | 9,2 (5,4–13,1) | 9 (4,65–12,3) | 10,7 (5,2–14,4) | 10,2 (6,3–15,5) |
| Trol (`troll`) | 3,65 (1,45–6,1) | 3,65 (1,55–6,15) | 2,7 (1,6–7,65) | 2,95 (0,65–7,15) | 3,6 (0,15–7,9) | 2,85 (0–8) |
| Kuşatma (`siege`) | 1,75 (0,8–2,65) | 1,7 (0,75–2,15) | 1,45 (0,5–2,1) | 1,5 (0,9–2) | 1,15 (0,5–1,75) | 1,45 (0–1,8) |
| Yağma akını (`plunder`) | 1,75 (0,5–4,05) | 1,4 (0–3,2) | 1,05 (0–2,45) | 1,05 (0–2,1) | 1 (0,1–2,25) | 0,9 (0–2) |
| Yapı baskını (canavar) (`extRaid`) | 1 (0,35–2,2) | 0,75 (0,4–2,2) | 0,6 (0,25–1,55) | 0,85 (0,3–2) | 0,85 (0,4–1,35) | 0,7 (0,3–1,5) |
| Yerleşim baskını (canavar) (`raid`) | 0,65 (0,3–0,95) | 0,55 (0,25–0,9) | 0,35 (0,1–1,25) | 0,4 (0,1–1,1) | 0,3 (0,05–0,9) | 0,4 (0,05–1,3) |
| Kervan soygunu (`robbery`) | 0,4 (0,1–0,55) | 0,2 (0–0,35) | 0,25 (0,05–0,5) | 0,3 (0,15–0,55) | 0,3 (0,15–0,6) | 0,4 (0,15–1,4) |
| Ejderha (`dragon`) | 0 | 0,6 (0,2–0,8) | 0,65 (0–1,35) | 0 (0–1,5) | 0 (0–1,5) | 0 (0–1,5) |
| Düello (`duel`) | 0,2 (0,05–0,4) | 0,1 (0–0,4) | 0,2 (0–0,5) | 0,2 (0,05–0,4) | 0,2 (0,05–0,5) | 0,2 (0–0,55) |
| Han baskını (canavar) (`innMonster`) | 0,05 (0–0,15) | 0 (0–0,2) | 0,05 (0–0,15) | 0,1 (0–0,1) | 0,05 (0–0,1) | 0,05 (0–0,2) |
| Deniz savaşı (`naval`) | 0,05 (0–0,3) | 0,05 (0–0,25) | 0 (0–0,25) | 0 (0–0,15) | 0 (0–0,1) | 0 (0–0,2) |
| Korsan savaşı (`pirate`) | 0 (0–0,15) | 0 (0–0,1) | 0 (0–0,1) | 0,1 (0–0,15) | 0 (0–0,1) | 0 (0–0,1) |
| Yol pususu (`ambush`) | 0 (0–0,1) | 0 | 0 (0–0,1) | 0 (0–0,05) | 0 (0–0,1) | 0 (0–0,1) |
| Han baskını (medeniyet) (`innCiv`) | 0 | 0 (0–0,1) | 0 | 0 (0–0,1) | 0 | 0 |

## Kamp türleri

Dünya başına yaşayan kamp (yıl sonu değerlerinin on yıllık ortalaması): dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Hobgoblin (`hobgoblin`) | 3,25 (2,85–3,7) | 3,2 (2,65–4,1) | 3,1 (2,4–3,85) | 3,05 (2,35–4,35) | 3,2 (2,75–3,95) | 3 (2,15–3,95) |
| Trol (`troll`) | 2 (1,55–2,7) | 2,25 (1,65–2,65) | 2,65 (1,7–2,85) | 2,65 (1,85–3) | 2,8 (2,15–3) | 2,9 (2,25–3) |
| Goblin (`goblin`) | 1,8 (0,9–2,55) | 1,6 (0,75–2,5) | 2 (1,15–2,45) | 1,75 (0,5–2,3) | 1,3 (0,3–1,8) | 1,9 (0,4–2,45) |
| Bugbear (`bugbear`) | 0,8 (0,55–1,2) | 0,9 (0,35–1,35) | 0,8 (0,3–1,25) | 1 (0,45–1,35) | 0,75 (0,5–1,25) | 0,95 (0,45–1,7) |
| Aç haydut (`bandit`) | 0,7 (0,15–1,5) | 0,55 (0,2–1,4) | 0,45 (0,1–1,25) | 0,4 (0,1–1,35) | 0,9 (0,15–2,05) | 0,9 (0,35–1,9) |
| Korsan (`pirate`) | 0,35 (0–0,85) | 0,35 (0,1–1) | 0,35 (0,05–1,1) | 0,5 (0,05–1,05) | 0,45 (0,1–1,25) | 0,45 (0,1–0,9) |
| Ejderha (`dragon`) | 0 | 0,5 (0,2–0,7) | 0,8 (0–1) | 0 (0–1) | 0 (0–1) | 0 (0–1) |

## Kademe dağılımı

Yaşayan yerleşimlerin kademelere dağılımı, bütün dünyalar (yıl sonu; parantezde sayı).

| Yıl | 0 Kamp | 1 Köy | 2 Kasaba | 3 Şehir |
|---|---|---|---|---|
| 10 | %30 (363) | %48 (595) | %14 (177) | %7,6 (94) |
| 20 | %27 (339) | %49 (605) | %16 (196) | %8,4 (105) |
| 30 | %26 (329) | %49 (609) | %17 (216) | %7,4 (92) |
| 40 | %25 (317) | %52 (650) | %15 (190) | %7,6 (95) |
| 50 | %27 (333) | %50 (622) | %16 (196) | %7,8 (97) |
| 60 | %26 (319) | %51 (636) | %15 (190) | %8,3 (103) |

Başkentlerin kademelere dağılımı, bütün dünyalar (yıl sonu; parantezde sayı).

| Yıl | 0 Kamp | 1 Köy | 2 Kasaba | 3 Şehir |
|---|---|---|---|---|
| 10 | %1 (1) | %7,1 (7) | %19 (19) | %73 (72) |
| 20 | %1 (1) | %4 (4) | %19 (19) | %76 (75) |
| 30 | %1 (1) | %2,9 (3) | %24 (24) | %73 (74) |
| 40 | %1 (1) | %3,9 (4) | %21 (21) | %75 (76) |
| 50 | %0,9 (1) | %4,7 (5) | %17 (18) | %77 (82) |
| 60 | %1,9 (2) | %4,7 (5) | %17 (18) | %77 (82) |

## Dünyalar

| Seed | Medeniyet | Yerleşim | Nüfus | Çöküş | Efsane | En yüksek Sv | Doğan / ölü kahraman | İlk şehir (yıl, medyan) | Süre (sn) | Son hash |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | 6 | 78 | 2669 | 6 | 0 | 10 | 458 / 117 | – | 91,8 | `a5f15403c1a696b7` |
| 2 | 7 | 82 | 3469 | 6 | 4 | 10 | 883 / 175 | – | 132 | `545eaf8c94697649` |
| 3 | 7 | 79 | 2737 | 7 | 2 | 10 | 314 / 102 | – | 75,9 | `dd46efc12e6dc583` |
| 4 | 7 | 79 | 2847 | 3 | 1 | 10 | 885 / 190 | – | 129 | `8fbc9212985e4deb` |
| 5 | 8 | 81 | 3456 | 6 | 4 | 10 | 1027 / 290 | – | 151 | `017c6ebe6180a0f7` |
| 6 | 6 | 80 | 2128 | 2 | 2 | 10 | 943 / 332 | – | 125 | `5370691d40107a50` |
| 7 | 4 | 82 | 2547 | 12 | 0 | 10 | 523 / 122 | – | 95,2 | `4b02e3ca901c305f` |
| 8 | 8 | 80 | 3296 | 7 | 2 | 10 | 638 / 122 | – | 116 | `9baab778e1f377ba` |
| 9 | 6 | 83 | 3326 | 8 | 5 | 10 | 772 / 215 | – | 123 | `bf8ac643fe6b10d8` |
| 10 | 8 | 77 | 2642 | 8 | 2 | 10 | 514 / 117 | – | 99,8 | `0f9da186aeb72893` |
| 11 | 7 | 79 | 2863 | 5 | 2 | 10 | 585 / 113 | – | 106 | `6937da04626f3c5f` |
| 12 | 7 | 82 | 3659 | 5 | 3 | 10 | 943 / 234 | – | 146 | `63118b56367c9fcb` |
| 13 | 8 | 81 | 3008 | 10 | 5 | 10 | 742 / 180 | – | 104 | `d1d9d3d6048893ab` |
| 14 | 6 | 55 | 1912 | 7 | 3 | 10 | 714 / 329 | – | 64,8 | `b4043ae6dc4d0dd0` |
| 15 | 6 | 70 | 3052 | 15 | 0 | 10 | 346 / 91 | – | 88,7 | `3bda24ea52b47a56` |
| 16 | 6 | 80 | 3340 | 2 | 4 | 10 | 980 / 265 | – | 120 | `7e53461909560e29` |

Çöküşler:

- seed 1, 6. yıl (gün 224): Kemikçadır Boyları başkenti kaybetti: Kurtgeçit (Karageçit Teokrasisi aldı)
- seed 1, 9. yıl (gün 324): Kemikçadır Boyları yok oldu
- seed 1, 15. yıl (gün 582): Kurtoba Boyları başkenti kaybetti: Kocapınar (Yeminköprü Teokrasisi aldı)
- seed 1, 20. yıl (gün 787): Kocapınar Teokrasisi başkenti kaybetti: Kocapınar (Yeminköprü Teokrasisi aldı)
- seed 1, 57. yıl (gün 2259): Yeminköprü Teokrasisi başkenti kaybetti: Demiroba (Kurtoba Boyları aldı)
- seed 1, 60. yıl (gün 2365): Akburç Boyları yok oldu
- seed 2, 5. yıl (gün 170): Közburç Krallığı başkenti kaybetti: Savaşçukur (Taşkandil Krallığı aldı)
- seed 2, 9. yıl (gün 331): Közburç Krallığı başkenti kaybetti: Derinmihrap (Taşkandil Krallığı aldı)
- seed 2, 16. yıl (gün 617): Kurtoba Boyları başkenti kaybetti: Taşçukur Madeni Harabesi Vadisi (Közburç Cumhuriyeti aldı)
- seed 2, 28. yıl (gün 1097): Kurtoba Boyları başkenti kaybetti: Yıldızçayır (Balköprü Krallığı aldı)
- seed 2, 35. yıl (gün 1392): Kutsalörs Boyları yok oldu
- seed 2, 56. yıl (gün 2221): Balköprü Teokrasisi başkenti kaybetti: Savaşçukur (Kurtoba Boyları aldı)
- seed 3, 2. yıl (gün 47): Kanlıdiş Boyları başkenti kaybetti: Kurtkale (Güneştacı Krallığı aldı)
- seed 3, 3. yıl (gün 101): Dumanköprü Krallığı yok oldu
- seed 3, 14. yıl (gün 530): Savaşçukur Krallığı başkenti kaybetti: Savaşçukur (Güneştacı Krallığı aldı)
- seed 3, 17. yıl (gün 660): Yıldıztepe Boyları yok oldu
- seed 3, 23. yıl (gün 883): Alevgeçit Cumhuriyeti başkenti kaybetti: Taşhisar (Güneştacı Krallığı aldı)
- seed 3, 26. yıl (gün 1038): Kanlıdiş Boyları başkenti kaybetti: Meşekent (Güneştacı Krallığı aldı)
- seed 3, 31. yıl (gün 1222): Güneştacı Krallığı başkenti kaybetti: Taşhisar (Alevgeçit Cumhuriyeti aldı)
- seed 4, 27. yıl (gün 1063): Keseli Cumhuriyeti başkenti kaybetti: Gölpınar (Kalkanova Krallığı aldı)
- seed 4, 37. yıl (gün 1454): Pınarhisar Boyları yok oldu
- seed 4, 57. yıl (gün 2268): Kalkanova Teokrasisi başkenti kaybetti: Gölpınar (Keseli Krallığı aldı)
- seed 5, 10. yıl (gün 386): Tatlıçayır Krallığı başkenti kaybetti: Uluova (Kızılboynuz Krallığı aldı)
- seed 5, 20. yıl (gün 780): Tatlıçayır Cumhuriyeti başkenti kaybetti: Tuzyayla (Kızılboynuz Krallığı aldı)
- seed 5, 24. yıl (gün 931): Tuzyayla Teokrasisi başkenti kaybetti: Tuzyayla (Kızılboynuz Krallığı aldı)
- seed 5, 35. yıl (gün 1392): Kızılboynuz Krallığı başkenti kaybetti: Yıldıztepe (Çamova Teokrasisi aldı)
- seed 5, 39. yıl (gün 1544): Çamova Krallığı yok oldu
- seed 5, 54. yıl (gün 2132): Ceylanbük Krallığı yok oldu
- seed 6, 7. yıl (gün 269): Granitsunak Teokrasisi yok oldu
- seed 6, 15. yıl (gün 592): Fıçıköy Krallığı başkenti kaybetti: Tamburlu (Kızılyurt Boyları aldı)
- seed 7, 3. yıl (gün 92): Alacayayla Boyları başkenti kaybetti: Alacayayla (Közburç Krallığı aldı)
- seed 7, 10. yıl (gün 394): Fıçıköy Cumhuriyeti başkenti kaybetti: Neşeliova (Kurtoba Boyları aldı)
- seed 7, 16. yıl (gün 610): Alacayayla Boyları başkenti kaybetti: Taşdere (Karaörs Krallığı aldı)
- seed 7, 18. yıl (gün 688): Fıçıköy Cumhuriyeti yok oldu
- seed 7, 19. yıl (gün 725): Alacayayla Boyları yok oldu
- seed 7, 20. yıl (gün 797): Bozkent Boyları yok oldu
- seed 7, 22. yıl (gün 872): Taşdere Boyları yok oldu
- seed 7, 24. yıl (gün 959): Kuzeybük Cumhuriyeti yok oldu
- seed 7, 25. yıl (gün 1000): Dumanpınar Krallığı yok oldu
- seed 7, 40. yıl (gün 1577): Kuzeybük Boyları yok oldu
- seed 7, 43. yıl (gün 1681): Aksırt Boyları yok oldu
- seed 7, 53. yıl (gün 2113): Neşeliova Boyları yok oldu
- seed 8, 2. yıl (gün 71): Toynakbaş Boyları yok oldu
- seed 8, 12. yıl (gün 476): Demirkanat Krallığı başkenti kaybetti: Demirkanat (Özgür Pulzırh aldı)
- seed 8, 17. yıl (gün 671): Demirkanat Krallığı başkenti kaybetti: Alacayurt (Özgür Pulzırh aldı)
- seed 8, 39. yıl (gün 1558): Demirkanat Krallığı yok oldu
- seed 8, 40. yıl (gün 1577): Ayışığı Cumhuriyeti başkenti kaybetti: Meşekent (Kutsalörs Krallığı aldı)
- seed 8, 42. yıl (gün 1653): Tatlıçayır Cumhuriyeti yok oldu
- seed 8, 42. yıl (gün 1677): Balgeçit Boyları yok oldu
- seed 9, 10. yıl (gün 388): Kurtoba Krallığı başkenti kaybetti: Kurtoba (Demirkanat Teokrasisi aldı)
- seed 9, 18. yıl (gün 710): Toynakbaş Boyları başkenti kaybetti: Toynakbaş (Kurtoba Krallığı aldı)
- seed 9, 21. yıl (gün 820): Dikenli III Vadisi Cumhuriyeti yok oldu
- seed 9, 37. yıl (gün 1466): Demirkanat Teokrasisi başkenti kaybetti: Toynakbaş (Toynakbaş Boyları aldı)
- seed 9, 39. yıl (gün 1523): Taşdere Boyları yok oldu
- seed 9, 47. yıl (gün 1869): Demirkanat Krallığı başkenti kaybetti: Demirbük (Toynakbaş Boyları aldı)
- seed 9, 51. yıl (gün 2003): Taşhisar Boyları yok oldu
- seed 9, 52. yıl (gün 2048): Demirkanat Cumhuriyeti başkenti kaybetti: Kızıltepe (Toynakbaş Boyları aldı)
- seed 10, 5. yıl (gün 169): Yeşilyaprak Cumhuriyeti başkenti kaybetti: Yıldızçayır (Kızılyurt Boyları aldı)
- seed 10, 7. yıl (gün 248): İzsürer Cumhuriyeti başkenti kaybetti: Yıldızçayır (Taşkandil Teokrasisi aldı)
- seed 10, 17. yıl (gün 667): Kızılyurt Boyları başkenti kaybetti: Demiroba (Taşkandil Teokrasisi aldı)
- seed 10, 21. yıl (gün 807): Kızılyurt Boyları başkenti kaybetti: Kızılçayır (Yeşilyaprak Cumhuriyeti aldı)
- seed 10, 27. yıl (gün 1044): Kızılyurt Boyları başkenti kaybetti: Geyikyurt (Yabanyurt Krallığı aldı)
- seed 10, 30. yıl (gün 1169): Kızıltepe Boyları yok oldu
- seed 10, 36. yıl (gün 1416): Karamum Krallığı başkenti kaybetti: Karagöl (Yabanyurt Krallığı aldı)
- seed 10, 55. yıl (gün 2184): Taşkandil Teokrasisi başkenti kaybetti: Demiroba (Kızılyurt Boyları aldı)
- seed 11, 5. yıl (gün 196): Demirkanat Boyları başkenti kaybetti: Çürükpençe Vadisi (Kutsalörs Teokrasisi aldı)
- seed 11, 7. yıl (gün 273): Şafaktepe Krallığı başkenti kaybetti: Gölköprü (Demirkanat Boyları aldı)
- seed 11, 28. yıl (gün 1088): Demirkanat Boyları başkenti kaybetti: Gölköprü (Kutsalörs Teokrasisi aldı)
- seed 11, 32. yıl (gün 1270): Demirkanat Boyları başkenti kaybetti: Yeşilkaya (Karaörs Teokrasisi aldı)
- seed 11, 34. yıl (gün 1343): Gölköprü Boyları yok oldu
- seed 12, 3. yıl (gün 98): Pulzırh Teokrasisi başkenti kaybetti: Kartalkaya (Gümüşdal Boyları aldı)
- seed 12, 3. yıl (gün 105): Demirçene Vadisi Boyları yok oldu
- seed 12, 8. yıl (gün 308): Gümüşdal Boyları başkenti kaybetti: Kartalkaya (Közsaray Krallığı aldı)
- seed 12, 28. yıl (gün 1092): Közsaray Cumhuriyeti başkenti kaybetti: Kartalkaya (Yosunpınar Teokrasisi aldı)
- seed 12, 45. yıl (gün 1793): Gümüşdal Boyları yok oldu
- seed 13, 6. yıl (gün 236): Kemikçadır Boyları başkenti kaybetti: Kuzeybük (Kızılyurt Boyları aldı)
- seed 13, 15. yıl (gün 574): Kızılyurt Boyları başkenti kaybetti: Taşdere (Keseli Cumhuriyeti aldı)
- seed 13, 17. yıl (gün 650): Özgür Ulukoru yok oldu
- seed 13, 30. yıl (gün 1168): Yeşilyaprak Krallığı başkenti kaybetti: Toynakbaş (Kızılyurt Boyları aldı)
- seed 13, 34. yıl (gün 1349): Kızılyurt Boyları başkenti kaybetti: Balköy (Kemikçadır Boyları aldı)
- seed 13, 36. yıl (gün 1426): Kemikçadır Boyları başkenti kaybetti: Balköy (Örsyürek Cumhuriyeti aldı)
- seed 13, 38. yıl (gün 1493): Kızılyurt Boyları başkenti kaybetti: Ayburç (Kemikçadır Boyları aldı)
- seed 13, 42. yıl (gün 1643): Ayburç Boyları yok oldu
- seed 13, 53. yıl (gün 2094): Örsyürek Cumhuriyeti başkenti kaybetti: Balköy (Kızılyurt Boyları aldı)
- seed 13, 56. yıl (gün 2222): Granitsunak Boyları başkenti kaybetti: Granitsunak (Keseli Krallığı aldı)
- seed 14, 10. yıl (gün 400): Boynuztepe Boyları başkenti kaybetti: Boynuztepe (Toynakbaş Boyları aldı)
- seed 14, 14. yıl (gün 548): Toynakbaş Boyları başkenti kaybetti: Boynuztepe (Güneştacı Krallığı aldı)
- seed 14, 17. yıl (gün 643): Kara Lejyon Vadisi Cumhuriyeti yok oldu
- seed 14, 19. yıl (gün 750): Güneştacı Cumhuriyeti başkenti kaybetti: Boynuztepe (Toynakbaş Boyları aldı)
- seed 14, 27. yıl (gün 1073): Boynuztepe Boyları başkenti kaybetti: Boynuztepe (Toynakbaş Boyları aldı)
- seed 14, 29. yıl (gün 1148): Güneştacı Cumhuriyeti başkenti kaybetti: Alacayurt (Toynakbaş Boyları aldı)
- seed 14, 46. yıl (gün 1818): Boynuztepe Boyları başkenti kaybetti: Kara Lejyon Vadisi (Toynakbaş Boyları aldı)
- seed 15, 7. yıl (gün 265): Karlıçayır Boyları başkenti kaybetti: Gökyayla (Akburç Teokrasisi aldı)
- seed 15, 11. yıl (gün 435): Karlıçayır Boyları yok oldu
- seed 15, 11. yıl (gün 439): Akburç Teokrasisi başkenti kaybetti: Akburç (Kızılyurt Boyları aldı)
- seed 15, 13. yıl (gün 504): Ejderkale Cumhuriyeti yok oldu
- seed 15, 18. yıl (gün 711): Kuruçalı Vadisi Krallığı başkenti kaybetti: Günkaya (Demirkanat Krallığı aldı)
- seed 15, 20. yıl (gün 766): Kızılyurt Boyları başkenti kaybetti: Kızılyurt (Balköprü Teokrasisi aldı)
- seed 15, 20. yıl (gün 776): Yeşilyaprak Cumhuriyeti yok oldu
- seed 15, 22. yıl (gün 875): Örsyürek Krallığı yok oldu
- seed 15, 23. yıl (gün 903): Kurtgeçit Cumhuriyeti yok oldu
- seed 15, 32. yıl (gün 1254): Güneştacı Teokrasisi yok oldu
- seed 15, 37. yıl (gün 1466): Demirçan Krallığı yok oldu
- seed 15, 39. yıl (gün 1544): Kızılyurt Boyları başkenti kaybetti: Savaşçukur (Balköprü Teokrasisi aldı)
- seed 15, 41. yıl (gün 1616): Demirbük Cumhuriyeti yok oldu
- seed 15, 47. yıl (gün 1867): Taşkandil Krallığı yok oldu
- seed 15, 53. yıl (gün 2107): Kızılyurt Boyları yok oldu
- seed 16, 51. yıl (gün 2018): Karaörs Krallığı başkenti kaybetti: Kemikçadır (Kurtoba Boyları aldı)
- seed 16, 53. yıl (gün 2112): Karagöl Cumhuriyeti yok oldu

## Yıllık ayrıntı

Hücre: medyan (p10–p90), 16 dünya. Yıl y = (y−1)·40+1 … y·40. günler. Bütün değerler `report.json` içinde (`metrics`), dünya başına değerler `../runs/f1b-7` altında.

### Medeniyet (1/3)

| Yıl | Yaşayan medeniyet | Yeni medeniyet (yeniden doğan) | Yok olan medeniyet | Başkent kaybı (medeniyet yaşarken) | Çöküş (yok olma + başkent kaybı) | Yaşayan yerleşim |
|---|---|---|---|---|---|---|
| 1 | 6 (5,5–7) | 0 (0–0,5) | 0 | 0 | 0 | 81 (73–81) |
| 2 | 6 (5,5–7) | 0 | 0 | 0 | 0 (0–0,5) | 80 (74,5–81,5) |
| 3 | 6 (5–7) | 0 | 0 (0–0,5) | 0 (0–0,5) | 0 (0–1) | 80,5 (76–81,5) |
| 4 | 6 (5,5–7) | 0 | 0 | 0 | 0 | 81 (76–81,5) |
| 5 | 6 (5,5–7) | 0 | 0 | 0 (0–1) | 0 (0–1) | 80 (75,5–81) |
| 6 | 6 (5,5–7) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 80 (73–81,5) |
| 7 | 6 (5,5–7) | 0 | 0 | 0 (0–1) | 0 (0–1) | 80 (69–81,5) |
| 8 | 6 (5,5–7) | 0 | 0 | 0 | 0 | 80 (66–81,5) |
| 9 | 6 (5–7) | 0 | 0 | 0 | 0 (0–0,5) | 79,5 (67,5–81,5) |
| 10 | 6 (5,5–7) | 0 (0–0,5) | 0 | 0 (0–1) | 0 (0–1) | 80 (69–81,5) |
| 11 | 6 (5,5–7) | 0 | 0 | 0 | 0 | 80 (70,5–81,5) |
| 12 | 6 (5,5–7) | 0 (0–0,5) | 0 | 0 | 0 | 80 (72–81) |
| 13 | 6 (5,5–7) | 0 | 0 | 0 | 0 | 80 (73–81) |
| 14 | 6 (5,5–7) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 80 (74–81,5) |
| 15 | 6 (6–7) | 0 (0–1) | 0 | 0 (0–1) | 0 (0–1) | 80 (73–81,5) |
| 16 | 6 (6–7) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 80,5 (73,5–82) |
| 17 | 6 (6–7) | 0 | 0 (0–1) | 0 (0–0,5) | 0 (0–1) | 80 (73,5–81) |
| 18 | 6 (6–7) | 0 | 0 | 0 (0–0,5) | 0 (0–1) | 80 (75–81) |
| 19 | 6 (6–7) | 0 (0–0,5) | 0 | 0 | 0 (0–0,5) | 80 (74,5–81) |
| 20 | 6 (6–7) | 0 | 0 (0–0,5) | 0 (0–1) | 0 (0–1) | 79,5 (75–82) |
| 21 | 6 (6–7) | 0 (0–0,5) | 0 | 0 | 0 (0–0,5) | 80 (75–81,5) |
| 22 | 6 (5,5–7) | 0 | 0 (0–0,5) | 0 | 0 (0–0,5) | 80 (75–81,5) |
| 23 | 6 (5,5–7) | 0 | 0 | 0 | 0 (0–0,5) | 79,5 (76,5–81) |
| 24 | 6 (5,5–7) | 0 | 0 | 0 | 0 (0–0,5) | 80 (74,5–82) |
| 25 | 6 (5,5–7) | 0 | 0 | 0 | 0 | 80 (73,5–82) |
| 26 | 6 (5,5–7) | 0 | 0 | 0 | 0 | 80 (75,5–81) |
| 27 | 6 (5,5–7) | 0 | 0 | 0 (0–1) | 0 (0–1) | 80 (76,5–82) |
| 28 | 6 (5,5–7) | 0 | 0 | 0 (0–1) | 0 (0–1) | 80 (76,5–81,5) |
| 29 | 6 (5,5–7) | 0 | 0 | 0 | 0 | 80 (76,5–81) |
| 30 | 6 (6–7) | 0 | 0 | 0 | 0 (0–0,5) | 79,5 (77–81) |
| 31 | 6 (6–7) | 0 | 0 | 0 | 0 | 80 (74,5–81,5) |
| 32 | 6 (6–7) | 0 | 0 | 0 | 0 (0–0,5) | 80 (76,5–81,5) |
| 33 | 6 (6–7) | 0 | 0 | 0 | 0 | 80 (77–81) |
| 34 | 6 (6–7) | 0 | 0 | 0 | 0 (0–0,5) | 79 (77–81,5) |
| 35 | 6 (6–7) | 0 | 0 | 0 | 0 (0–0,5) | 80 (76,5–81,5) |
| 36 | 6 (6–7) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 79,5 (77–82) |
| 37 | 6 (6–7) | 0 | 0 (0–0,5) | 0 | 0 (0–1) | 79 (77–81,5) |
| 38 | 6,5 (6–7) | 0 (0–1) | 0 | 0 | 0 | 80 (77,5–81) |
| 39 | 6 (5,5–7) | 0 | 0 (0–1) | 0 | 0 (0–1) | 79 (77–82) |
| 40 | 6 (5,5–7) | 0 | 0 | 0 | 0 (0–0,5) | 80 (77,5–81) |
| 41 | 6,5 (6–7) | 0 | 0 | 0 | 0 | 80 (75,5–82) |
| 42 | 6 (5,5–7) | 0 | 0 (0–0,5) | 0 | 0 (0–0,5) | 80 (76–82) |
| 43 | 6 (6–7) | 0 | 0 | 0 | 0 | 79 (76–82) |
| 44 | 6,5 (6–7) | 0 (0–0,5) | 0 | 0 | 0 | 80 (74–82) |
| 45 | 6 (6–7) | 0 | 0 | 0 | 0 | 80 (74–82) |
| 46 | 6,5 (6–7) | 0 | 0 | 0 | 0 | 80 (74–82) |
| 47 | 6 (6–7) | 0 | 0 | 0 | 0 (0–0,5) | 80 (75–82) |
| 48 | 7 (6–7) | 0 (0–0,5) | 0 | 0 | 0 | 80 (76–82) |
| 49 | 7 (6–7) | 0 | 0 | 0 | 0 | 79,5 (75–82) |
| 50 | 7 (6–7) | 0 | 0 | 0 | 0 | 80 (76–82) |
| 51 | 7 (6–8) | 0 (0–1) | 0 | 0 | 0 (0–0,5) | 80 (75–82) |
| 52 | 7 (6–8) | 0 | 0 | 0 | 0 | 79 (75,5–82) |
| 53 | 6,5 (6–8) | 0 | 0 (0–1) | 0 | 0 (0–1) | 80 (74,5–82,5) |
| 54 | 6,5 (6–7,5) | 0 | 0 | 0 | 0 | 80 (75,5–81,5) |
| 55 | 6,5 (6–8) | 0 | 0 | 0 | 0 | 80 (76–82,5) |
| 56 | 6,5 (6–8) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 80 (76–82) |
| 57 | 7 (6–8) | 0 (0–0,5) | 0 | 0 (0–0,5) | 0 (0–0,5) | 80 (76,5–82) |
| 58 | 7 (6–8) | 0 | 0 | 0 | 0 | 80 (77–82) |
| 59 | 7 (6–8) | 0 | 0 | 0 | 0 | 80 (73–81) |
| 60 | 7 (6–8) | 0 | 0 | 0 | 0 | 80 (73,5–82) |

### Medeniyet (2/3)

| Yıl | Medeniyet başına yerleşim | 5+ kara yerleşimli medeniyet payı | Kurulan yerleşim | Fethedilen yerleşim | Terk edilen yerleşim | Toplam nüfus |
|---|---|---|---|---|---|---|
| 1 | 13,3 (11,4–13,6) | %83 (%71–%100) | 3 (0–4) | 1 (0–3,5) | 0 | 2812 (2409–3310) |
| 2 | 13 (11,4–13,8) | %83 (%73–%100) | 0 (0–1) | 2 (0,5–3) | 0 (0–1) | 2751 (2364–3258) |
| 3 | 13,3 (11,4–14,6) | %83 (%69–%100) | 0 (0–1) | 2 (0–4) | 0 | 2867 (2347–3256) |
| 4 | 13,4 (11,4–13,7) | %83 (%69–%100) | 0 (0–1,5) | 1 (0–3) | 0 (0–2) | 2960 (2300–3300) |
| 5 | 13,3 (10,9–13,6) | %83 (%69–%100) | 0 (0–1) | 2 (0,5–3) | 0 | 2834 (2338–3176) |
| 6 | 13,2 (11,2–13,7) | %83 (%67–%100) | 0 (0–1) | 1 (0–2) | 0 (0–2) | 2782 (2355–3106) |
| 7 | 13,3 (11–13,7) | %83 (%67–%100) | 0 (0–1) | 1,5 (0–3) | 0 | 2814 (2387–3212) |
| 8 | 12,9 (10,8–13,7) | %83 (%67–%93) | 0 (0–2) | 1,5 (0–3) | 0 (0–2,5) | 2904 (2314–3112) |
| 9 | 13 (11,1–13,7) | %83 (%71–%93) | 0 (0–2,5) | 1 (0–3) | 0 (0–2) | 2921 (2290–3119) |
| 10 | 13,2 (11–13,6) | %83 (%65–%83) | 0 (0–2) | 1 (0–3) | 0 (0–0,5) | 2826 (2266–3058) |
| 11 | 13,2 (11,2–13,6) | %83 (%69–%83) | 0 (0–1) | 1 (0–2) | 0 (0–0,5) | 2874 (2396–3082) |
| 12 | 13,2 (11–13,6) | %83 (%61–%83) | 0 (0–2) | 1 (0–3) | 0 (0–0,5) | 2874 (2340–3140) |
| 13 | 13,3 (11,1–13,6) | %83 (%66–%83) | 0 (0–2,5) | 0 (0–3) | 0 (0–0,5) | 2825 (2239–3186) |
| 14 | 13,3 (11,2–13,6) | %83 (%66–%83) | 0 (0–2) | 2 (0–3) | 0 (0–1,5) | 2970 (2189–3230) |
| 15 | 12,5 (11,1–13,7) | %76 (%62–%83) | 0 (0–1) | 1 (1–3) | 0 | 2868 (2138–3231) |
| 16 | 11,6 (11,1–13,7) | %71 (%62–%83) | 0 | 0,5 (0–2,5) | 0 | 2946 (2135–3190) |
| 17 | 13,2 (11,2–13,5) | %83 (%63–%85) | 0 (0–0,5) | 1 (0–4) | 0 | 2878 (2186–3194) |
| 18 | 13,3 (11,1–13,5) | %83 (%67–%83) | 0 | 1 (0–1,5) | 0 (0–0,5) | 2946 (2123–3202) |
| 19 | 13 (10,9–13,5) | %83 (%67–%83) | 0 | 1 (0–2,5) | 0 (0–1) | 2867 (2175–3194) |
| 20 | 12,9 (11,2–13,6) | %77 (%67–%83) | 0 | 1 (0–3) | 0 (0–1) | 2912 (2270–3112) |
| 21 | 13,1 (10,8–13,6) | %82 (%65–%83) | 0 (0–1,5) | 1 (0–2,5) | 0 (0–1) | 2814 (2201–3079) |
| 22 | 13 (11–13,7) | %82 (%67–%83) | 0 (0–1) | 0 (0–1,5) | 0 (0–0,5) | 2871 (2190–3290) |
| 23 | 13 (11,1–13,6) | %82 (%67–%83) | 0 (0–0,5) | 1 (0–3) | 0 | 2839 (2276–3240) |
| 24 | 13,1 (10,9–13,7) | %82 (%67–%83) | 0 (0–0,5) | 0,5 (0–3) | 0 (0–1) | 2992 (2336–3246) |
| 25 | 13 (10,7–13,7) | %83 (%67–%85) | 0 | 1,5 (0–3) | 0 | 2971 (2298–3300) |
| 26 | 13,3 (10,9–13,7) | %83 (%67–%85) | 0 (0–1) | 0,5 (0–1,5) | 0 | 2883 (2387–3170) |
| 27 | 13 (11,1–13,8) | %82 (%59–%83) | 0 (0–1,5) | 1 (0–1,5) | 0 (0–0,5) | 2842 (2368–3206) |
| 28 | 13 (10,9–13,8) | %78 (%62–%83) | 0 (0–0,5) | 1 (0–2,5) | 0 | 2864 (2284–3206) |
| 29 | 12,1 (10,9–13,7) | %71 (%60–%83) | 0 (0–0,5) | 1 (0–2) | 0 (0–0,5) | 2938 (2442–3218) |
| 30 | 12,2 (11,1–13,6) | %71 (%57–%83) | 0 (0–1,5) | 1 (0–3) | 0 (0–1) | 2902 (2186–3344) |
| 31 | 12 (10,9–13,8) | %76 (%57–%83) | 0 (0–1,5) | 1 (0–2) | 0 (0–2) | 2810 (2350–3414) |
| 32 | 12,8 (10,4–13,7) | %82 (%60–%83) | 0 (0–0,5) | 1 (0–3) | 0 (0–0,5) | 2821 (2329–3492) |
| 33 | 12,2 (10,5–13,6) | %76 (%60–%83) | 0 (0–1,5) | 0 (0–1) | 0 (0–0,5) | 2870 (2308–3340) |
| 34 | 12,2 (11,1–13,6) | %76 (%57–%83) | 0 | 1 (0–2) | 0 (0–0,5) | 2906 (2202–3268) |
| 35 | 12,3 (11,1–13,6) | %76 (%57–%83) | 0 (0–1,5) | 1 (0–2) | 0 (0–1) | 2813 (2262–3366) |
| 36 | 12,3 (11,1–13,7) | %76 (%57–%83) | 0 (0–1) | 1 (0–2,5) | 0 (0–0,5) | 2848 (2148–3263) |
| 37 | 12,8 (11,2–13,6) | %76 (%57–%83) | 0 (0–0,5) | 1 (0–3) | 0 (0–1) | 2926 (2090–3288) |
| 38 | 11,6 (10,5–13,4) | %71 (%57–%83) | 0 (0–1) | 1 (0–2,5) | 0 (0–0,5) | 2898 (2032–3260) |
| 39 | 12,2 (10,4–14,7) | %69 (%57–%85) | 0 (0–1) | 1 (0–2,5) | 0 (0–0,5) | 2815 (2148–3406) |
| 40 | 12,1 (10,5–14,8) | %71 (%57–%85) | 0 | 1 (0–2) | 0 | 2912 (2393–3297) |
| 41 | 11,5 (10,9–13,8) | %69 (%57–%83) | 0 (0–1) | 1 (0–2,5) | 0 (0–1) | 2898 (2392–3409) |
| 42 | 12,9 (11–14,9) | %76 (%57–%85) | 0 (0–1) | 1 (0–2) | 0 (0–1) | 2987 (2304–3312) |
| 43 | 12,9 (10,9–13,8) | %77 (%57–%85) | 0 (0–0,5) | 0,5 (0–2) | 0 (0–1) | 2926 (2152–3486) |
| 44 | 11,7 (10,6–13,7) | %71 (%54–%86) | 0 (0–1,5) | 0,5 (0–2) | 0 (0–5) | 2848 (2035–3384) |
| 45 | 12,2 (10,6–13,8) | %69 (%57–%93) | 0 (0–1,5) | 1 (0–2) | 0 (0–1) | 2866 (2149–3398) |
| 46 | 11,6 (10,6–13,8) | %69 (%54–%93) | 0 (0–0,5) | 0 (0–2) | 0 | 2958 (2066–3414) |
| 47 | 12,1 (10,3–13,8) | %67 (%54–%93) | 0 (0–1) | 0,5 (0–1,5) | 0 (0–0,5) | 2783 (2061–3446) |
| 48 | 11,6 (10,2–13,6) | %71 (%57–%86) | 0 (0–1,5) | 0,5 (0–1,5) | 0 (0–1) | 2908 (2104–3224) |
| 49 | 11,5 (10,1–13,7) | %69 (%57–%86) | 0 | 1 (0–2) | 0 | 2976 (2194–3258) |
| 50 | 11,5 (10,2–13,7) | %69 (%54–%86) | 0 (0–1,5) | 0 (0–1,5) | 0 (0–1) | 2966 (2533–3408) |
| 51 | 11,4 (9,81–13,7) | %62 (%54–%86) | 0 (0–1) | 1 (0–2,5) | 0 | 2894 (2368–3569) |
| 52 | 11,3 (9,94–13,7) | %69 (%54–%85) | 0 (0–1,5) | 0,5 (0–2,5) | 0 (0–0,5) | 2882 (2402–3471) |
| 53 | 11,6 (9,88–13,8) | %69 (%54–%93) | 0 (0–1) | 1 (0–2) | 0 | 2968 (2448–3374) |
| 54 | 11,5 (9,94–13,6) | %69 (%54–%93) | 0 (0–1,5) | 0 (0–1,5) | 0 (0–1) | 2999 (2494–3405) |
| 55 | 11,6 (9,94–13,8) | %71 (%54–%92) | 0 (0–1) | 0 (0–2) | 0 (0–1) | 2866 (2338–3506) |
| 56 | 11,6 (9,81–13,7) | %67 (%54–%92) | 0 (0–1,5) | 1 (0–2) | 0 (0–0,5) | 2928 (2304–3364) |
| 57 | 11,6 (9,88–13,7) | %62 (%54–%92) | 0 (0–1) | 1 (0–2) | 0 (0–0,5) | 2857 (2384–3446) |
| 58 | 11,4 (9,81–13,7) | %62 (%54–%92) | 0 | 0 (0–1,5) | 0 | 2904 (2215–3468) |
| 59 | 11,4 (9,88–13,5) | %62 (%54–%83) | 0 (0–1) | 1 (0–2) | 0 (0–1) | 2950 (2378–3436) |
| 60 | 11,5 (9,81–13,6) | %67 (%54–%83) | 0 (0–0,5) | 0,5 (0–1,5) | 0 (0–1) | 2936 (2338–3462) |

### Medeniyet (3/3)

| Yıl | Altın medyanı (medeniyetler) | Boştaki iş gücü payı | Bölünme (ayrılıp kurulan medeniyet) | En büyük medeniyetin yerleşimi |
|---|---|---|---|---|
| 1 | 611 (346–862) | %16 (%10–%22) | 0 (0–0,5) | 21 (17–25,5) |
| 2 | 536 (285–757) | %17 (%11–%24) | 0 | 21 (17,5–26) |
| 3 | 546 (300–740) | %18 (%11–%24) | 0 | 21,5 (18–26,5) |
| 4 | 474 (214–674) | %20 (%10–%24) | 0 | 21,5 (18,5–26) |
| 5 | 417 (248–665) | %19 (%12–%26) | 0 | 21,5 (18,5–26) |
| 6 | 387 (249–811) | %17 (%13–%27) | 0 | 22 (18,5–27) |
| 7 | 393 (98–880) | %18 (%11–%26) | 0 | 22 (18–27) |
| 8 | 476 (150–760) | %17 (%14–%26) | 0 | 21 (18–27,5) |
| 9 | 445 (182–797) | %16 (%12–%26) | 0 | 21,5 (18–28) |
| 10 | 441 (108–771) | %19 (%12–%30) | 0 (0–0,5) | 21,5 (18–27,5) |
| 11 | 418 (155–651) | %20 (%9,2–%29) | 0 | 22,5 (18–29) |
| 12 | 426 (118–744) | %16 (%10–%30) | 0 | 22,5 (18–30) |
| 13 | 335 (68,6–776) | %17 (%11–%28) | 0 | 23 (18,5–29,5) |
| 14 | 357 (89,5–716) | %22 (%10–%30) | 0 | 23 (19–31) |
| 15 | 230 (86–602) | %19 (%11–%29) | 0 (0–1) | 22,5 (18–31) |
| 16 | 203 (75,4–397) | %20 (%13–%26) | 0 | 23 (19–31) |
| 17 | 338 (137–546) | %20 (%12–%26) | 0 | 24 (19–30,5) |
| 18 | 349 (205–593) | %19 (%12–%27) | 0 | 23,5 (18,5–30,5) |
| 19 | 419 (196–643) | %20 (%10–%26) | 0 (0–0,5) | 22,5 (18,5–31) |
| 20 | 455 (211–663) | %19 (%11–%26) | 0 | 24 (18,5–30) |
| 21 | 415 (135–627) | %18 (%10–%26) | 0 (0–0,5) | 23 (19–30,5) |
| 22 | 305 (148–554) | %17 (%14–%27) | 0 | 23 (20–32) |
| 23 | 410 (171–577) | %18 (%12–%24) | 0 | 23,5 (19,5–32,5) |
| 24 | 455 (323–547) | %17 (%12–%24) | 0 | 23,5 (19,5–34) |
| 25 | 430 (161–624) | %14 (%13–%26) | 0 | 23,5 (19–34,5) |
| 26 | 332 (146–738) | %18 (%13–%26) | 0 | 23 (19–35) |
| 27 | 326 (112–732) | %17 (%14–%25) | 0 | 24 (19,5–35) |
| 28 | 393 (80–805) | %18 (%13–%29) | 0 | 23,5 (18,5–35,5) |
| 29 | 402 (133–719) | %19 (%12–%29) | 0 | 24 (19–37) |
| 30 | 356 (132–750) | %19 (%12–%29) | 0 | 24 (18,5–35,5) |
| 31 | 339 (112–755) | %20 (%12–%26) | 0 | 24 (19,5–35,5) |
| 32 | 448 (150–792) | %20 (%14–%25) | 0 | 24,5 (19–36,5) |
| 33 | 363 (117–912) | %20 (%12–%25) | 0 | 23,5 (18,5–36) |
| 34 | 361 (115–771) | %20 (%12–%26) | 0 | 23,5 (18,5–35,5) |
| 35 | 229 (65,1–760) | %19 (%14–%28) | 0 | 23,5 (18,5–35,5) |
| 36 | 304 (74,3–689) | %20 (%14–%28) | 0 | 24 (18,5–35) |
| 37 | 289 (72,2–653) | %18 (%12–%29) | 0 | 24 (18,5–36) |
| 38 | 444 (119–727) | %17 (%12–%30) | 0 (0–0,5) | 24 (18,5–36) |
| 39 | 398 (92,2–658) | %19 (%13–%29) | 0 | 24,5 (18,5–36,5) |
| 40 | 312 (94,4–857) | %18 (%14–%28) | 0 | 24,5 (18–37) |
| 41 | 392 (80,7–601) | %21 (%13–%29) | 0 | 24,5 (18,5–37) |
| 42 | 424 (66,6–638) | %21 (%11–%31) | 0 | 24,5 (18,5–36,5) |
| 43 | 398 (82,5–848) | %20 (%11–%34) | 0 | 24,5 (19–37) |
| 44 | 346 (210–477) | %21 (%11–%35) | 0 (0–0,5) | 23 (18,5–35,5) |
| 45 | 465 (85,2–695) | %20 (%11–%38) | 0 | 22,5 (18,5–35,5) |
| 46 | 388 (142–767) | %20 (%14–%39) | 0 | 23 (18,5–35,5) |
| 47 | 406 (208–737) | %19 (%12–%37) | 0 | 23 (19–35,5) |
| 48 | 469 (287–708) | %20 (%12–%36) | 0 | 23 (17,5–36) |
| 49 | 429 (157–646) | %21 (%12–%36) | 0 | 23 (17,5–35,5) |
| 50 | 396 (120–754) | %20 (%14–%38) | 0 | 23,5 (18,5–35) |
| 51 | 431 (134–627) | %20 (%13–%35) | 0 (0–1) | 24 (18,5–37,5) |
| 52 | 336 (148–906) | %19 (%15–%34) | 0 | 23,5 (18–36,5) |
| 53 | 348 (67,8–728) | %20 (%15–%34) | 0 | 24 (18,5–38) |
| 54 | 391 (156–759) | %20 (%14–%32) | 0 | 24 (18–36,5) |
| 55 | 433 (207–736) | %22 (%15–%29) | 0 | 23,5 (18,5–37,5) |
| 56 | 444 (180–808) | %21 (%17–%31) | 0 | 24 (18,5–38,5) |
| 57 | 460 (114–994) | %21 (%15–%32) | 0 (0–0,5) | 24 (19–38,5) |
| 58 | 450 (86,3–839) | %19 (%15–%30) | 0 | 25 (18,5–38,5) |
| 59 | 305 (102–802) | %22 (%14–%30) | 0 | 24,5 (18–38) |
| 60 | 376 (118–739) | %17 (%15–%31) | 0 | 24 (18–38) |

### Olaylar

| Yıl | Olay | Büyük olay |
|---|---|---|
| 1 | 1189 (784–1471) | 224 (130–337) |
| 2 | 1197 (816–1458) | 230 (142–322) |
| 3 | 1153 (783–1518) | 215 (126–331) |
| 4 | 1299 (848–1488) | 229 (118–342) |
| 5 | 1211 (806–1545) | 246 (122–366) |
| 6 | 1222 (827–1474) | 216 (129–363) |
| 7 | 1224 (866–1506) | 222 (145–348) |
| 8 | 1250 (906–1461) | 231 (154–322) |
| 9 | 1312 (876–1443) | 244 (141–340) |
| 10 | 1286 (869–1498) | 230 (120–350) |
| 11 | 1274 (898–1476) | 242 (137–310) |
| 12 | 1346 (875–1526) | 252 (135–327) |
| 13 | 1325 (832–1575) | 225 (147–354) |
| 14 | 1391 (892–1572) | 226 (170–358) |
| 15 | 1312 (895–1537) | 212 (161–351) |
| 16 | 1301 (923–1510) | 204 (166–345) |
| 17 | 1319 (918–1536) | 224 (156–338) |
| 18 | 1402 (913–1587) | 222 (174–338) |
| 19 | 1353 (883–1580) | 232 (140–324) |
| 20 | 1400 (878–1600) | 260 (134–339) |
| 21 | 1389 (899–1582) | 254 (139–329) |
| 22 | 1392 (905–1555) | 268 (160–368) |
| 23 | 1390 (943–1571) | 258 (179–339) |
| 24 | 1370 (928–1650) | 268 (134–332) |
| 25 | 1396 (885–1638) | 253 (150–330) |
| 26 | 1475 (928–1585) | 260 (141–350) |
| 27 | 1443 (898–1598) | 274 (144–368) |
| 28 | 1406 (951–1639) | 250 (146–362) |
| 29 | 1413 (962–1635) | 218 (148–356) |
| 30 | 1426 (911–1632) | 253 (122–332) |
| 31 | 1402 (940–1617) | 266 (149–338) |
| 32 | 1381 (948–1664) | 229 (144–351) |
| 33 | 1372 (964–1580) | 216 (134–289) |
| 34 | 1412 (1020–1576) | 244 (144–346) |
| 35 | 1404 (1035–1629) | 216 (146–378) |
| 36 | 1334 (1014–1621) | 236 (147–328) |
| 37 | 1367 (1023–1690) | 213 (134–344) |
| 38 | 1386 (1072–1666) | 247 (166–358) |
| 39 | 1379 (1010–1609) | 238 (142–321) |
| 40 | 1361 (1043–1694) | 202 (146–340) |
| 41 | 1404 (1056–1728) | 216 (138–350) |
| 42 | 1395 (1066–1728) | 242 (140–380) |
| 43 | 1446 (1137–1739) | 288 (147–390) |
| 44 | 1380 (1074–1735) | 256 (141–382) |
| 45 | 1397 (1112–1786) | 260 (154–342) |
| 46 | 1412 (1128–1752) | 254 (164–366) |
| 47 | 1448 (1140–1735) | 274 (156–398) |
| 48 | 1441 (1128–1704) | 240 (160–358) |
| 49 | 1433 (1151–1684) | 253 (162–390) |
| 50 | 1402 (1172–1726) | 262 (150–374) |
| 51 | 1434 (1190–1716) | 268 (154–350) |
| 52 | 1470 (1171–1776) | 268 (178–409) |
| 53 | 1490 (1221–1765) | 252 (158–400) |
| 54 | 1442 (1226–1690) | 272 (194–354) |
| 55 | 1470 (1242–1682) | 284 (198–375) |
| 56 | 1432 (1224–1770) | 246 (154–344) |
| 57 | 1453 (1275–1820) | 278 (180–346) |
| 58 | 1452 (1292–1856) | 248 (188–368) |
| 59 | 1393 (1343–1816) | 240 (186–367) |
| 60 | 1454 (1290–1755) | 250 (174–351) |

### Savaş (1/2)

| Yıl | Muharebe | Başlayan savaş | Süren savaş (yıl sonu) | Yıl içinde süren savaş | Yağma akını (medeniyet) | Tarihî hak savaşı |
|---|---|---|---|---|---|---|
| 1 | 20,5 (14–28) | 2 (0–3,5) | 0,5 (0–2) | 3 (0–4) | 2,5 (0–4,5) | 0 |
| 2 | 17,5 (13,5–26) | 2 (0–4) | 1 (0–2,5) | 3 (1–4) | 1 (0–4,5) | 0 (0–1) |
| 3 | 17,5 (14–25,5) | 2 (0–3) | 0 (0–2) | 3 (1–4,5) | 3 (0–5,5) | 0 (0–1) |
| 4 | 21,5 (12–26) | 2 (1,5–4) | 1,5 (0–3) | 3 (2–4,5) | 1 (0–5) | 0 (0–1) |
| 5 | 20 (14–28,5) | 1 (0–3,5) | 0,5 (0–2) | 3 (2–4,5) | 1,5 (0–3) | 0 (0–1) |
| 6 | 19,5 (12,5–23) | 2 (1–3,5) | 1 (0–3) | 3 (2–4,5) | 1,5 (0–3) | 1 (0–1) |
| 7 | 19,5 (10,5–27) | 1 (0–3) | 0 (0–2,5) | 2 (0–4,5) | 2 (1–3,5) | 0 (0–1) |
| 8 | 21 (14,5–24,5) | 2 (1–4) | 1 (0–3,5) | 3 (2–5,5) | 1 (0–5) | 0 (0–1) |
| 9 | 19,5 (13–28) | 0,5 (0–2) | 0 (0–2) | 2 (0–6) | 1,5 (0–5) | 0 |
| 10 | 17,5 (10–26) | 2 (0–3) | 1 (0–2) | 2 (1–4) | 1,5 (0–3) | 0 (0–1) |
| 11 | 16 (11,5–23) | 1 (0–4) | 1 (0–2) | 2 (1–5) | 1 (0–3,5) | 0 (0–0,5) |
| 12 | 19,5 (10,5–27) | 1 (0–3) | 0 (0–1) | 2 (1–3,5) | 2 (0–5,5) | 0 (0–1) |
| 13 | 17,5 (10–28,5) | 2 (0,5–3,5) | 1 (0–2) | 2 (1–4) | 1 (0–3) | 0 (0–1) |
| 14 | 18,5 (14–25) | 2 (0–3) | 1 (0–2) | 3 (1–4) | 1 (0–3) | 0 (0–1,5) |
| 15 | 19 (10,5–28) | 2 (0–3) | 1 (0–2,5) | 3 (1–4,5) | 0,5 (0–3,5) | 0 (0–1) |
| 16 | 17,5 (12–30) | 2 (0–3) | 1,5 (0–2) | 2 (1,5–4,5) | 1 (0–2,5) | 0 (0–1) |
| 17 | 21 (14–26) | 1,5 (0,5–3,5) | 0 (0–2) | 2,5 (1–5) | 1 (0–2,5) | 0 (0–1) |
| 18 | 18,5 (13,5–27) | 1 (0–3) | 0 (0–2) | 2 (0–4) | 1 (0–2,5) | 0 (0–0,5) |
| 19 | 18,5 (11,5–28) | 1,5 (0,5–3,5) | 1 (0–2,5) | 3 (1–4,5) | 1 (0–4) | 0 (0–1) |
| 20 | 17,5 (12,5–31) | 0,5 (0–3) | 0 (0–1,5) | 3 (0,5–4,5) | 1 (0–3,5) | 0 (0–1) |
| 21 | 17,5 (10,5–27) | 2 (0–2,5) | 1 (0–1,5) | 2 (0–3,5) | 1 (0–2,5) | 0 (0–1) |
| 22 | 18,5 (12–27) | 0,5 (0–3) | 1 (0–2) | 2 (0,5–3,5) | 1 (0–3) | 0 (0–0,5) |
| 23 | 18 (12–25,5) | 2 (0–3,5) | 1 (0–2,5) | 2 (0,5–5) | 1 (0–3) | 0 (0–1) |
| 24 | 18,5 (11,5–28,5) | 1 (0–3,5) | 1 (0–1,5) | 1 (0–5) | 1 (0–4) | 0 (0–0,5) |
| 25 | 19,5 (12–28,5) | 1,5 (0–2,5) | 0 (0–1) | 2 (0,5–4) | 1 (0–3) | 0 |
| 26 | 16,5 (11–29) | 1,5 (0–3) | 1 (0–2) | 2 (0,5–3) | 1 (0–3) | 0 (0–1) |
| 27 | 18,5 (11–24) | 1 (0–2) | 1 (0–2) | 2 (1–3) | 0,5 (0–3) | 0 |
| 28 | 15,5 (10,5–24,5) | 1,5 (0–3) | 0 (0–2) | 2,5 (1–4) | 1 (0–2) | 0 (0–1) |
| 29 | 19,5 (10,5–30,5) | 1 (0–2,5) | 1 (0–2) | 2 (1–3,5) | 1 (0–3,5) | 0 (0–1) |
| 30 | 18 (9–24) | 1 (0–3) | 1 (0–3) | 2,5 (0,5–4,5) | 1 (0–2,5) | 0 (0–1) |
| 31 | 18 (12,5–23,5) | 1 (0–2,5) | 1 (0–2) | 2 (1–3) | 1 (0–3) | 0 (0–1) |
| 32 | 18 (12–23,5) | 1,5 (0–3,5) | 0 (0–2) | 2 (1–4,5) | 1 (0–2,5) | 0 (0–1) |
| 33 | 16 (11–26,5) | 1 (0–3) | 0 (0–2,5) | 2 (0–4) | 1 (0–2) | 0 (0–1) |
| 34 | 18 (13,5–25,5) | 1,5 (0,5–2,5) | 1 (0–2) | 2 (1–5) | 1 (0–2) | 0 (0–0,5) |
| 35 | 18 (10–24) | 1 (0–2) | 0 (0–2) | 2 (1–3) | 0,5 (0–2) | 0 |
| 36 | 17 (9–22) | 2 (0–3) | 1 (0–2) | 3 (1–3,5) | 1 (0–2,5) | 0 (0–1) |
| 37 | 19 (10–30) | 1 (0–2,5) | 0 (0–2) | 2 (1–3) | 1 (0–2,5) | 0 |
| 38 | 18,5 (9,5–24) | 1,5 (0–3) | 1 (0–2,5) | 2 (1–3,5) | 1 (0–2,5) | 0 (0–0,5) |
| 39 | 18,5 (10,5–23) | 1 (0–3) | 1 (0–1,5) | 2 (1–4,5) | 1 (0–2,5) | 0 |
| 40 | 17,5 (12,5–23,5) | 1 (0–2) | 1 (0–1,5) | 2 (1–2,5) | 1,5 (0–3) | 0 (0–1) |
| 41 | 16 (9,5–25,5) | 2 (0–2) | 0,5 (0–2) | 2 (0,5–3) | 1 (0–2) | 0 (0–1) |
| 42 | 19 (9–24) | 1 (0–2,5) | 0,5 (0–1,5) | 2 (0,5–3,5) | 1 (0–2) | 0 (0–1) |
| 43 | 22 (13–31) | 1 (0–2) | 0,5 (0–2) | 2 (0,5–3) | 1 (0–2) | 0 (0–1) |
| 44 | 19,5 (8,5–27,5) | 1 (0–2,5) | 0,5 (0–2) | 2 (1–3) | 1 (0–3) | 0 (0–1) |
| 45 | 21,5 (9,5–27) | 1 (0–1,5) | 0 (0–1) | 1 (0,5–2,5) | 2 (0–2) | 0 |
| 46 | 17 (14,5–26) | 1 (0–2) | 0 (0–1,5) | 1 (0–2,5) | 1 (0–3) | 0 (0–1) |
| 47 | 20 (8,5–31,5) | 1 (0–2,5) | 0 (0–1) | 1 (0–3) | 1 (0–3,5) | 0 (0–1) |
| 48 | 19,5 (9–28) | 1 (0–2) | 0,5 (0–1,5) | 1 (0–3) | 1 (0–3) | 0 (0–0,5) |
| 49 | 21,5 (7,5–32) | 1 (0–2) | 0,5 (0–1,5) | 2 (0–3) | 1 (0–2) | 0 |
| 50 | 20,5 (6–27,5) | 0,5 (0–1,5) | 0 (0–1,5) | 1 (0–2,5) | 1,5 (0–3,5) | 0 (0–1) |
| 51 | 21 (7–26,5) | 1 (0–3) | 1 (0–2,5) | 2 (0–4) | 0,5 (0–2,5) | 0 (0–0,5) |
| 52 | 21 (13–27,5) | 0,5 (0–2) | 0,5 (0–1) | 1,5 (0–3,5) | 1 (0–2,5) | 0 |
| 53 | 19 (8–25) | 1 (0–2) | 0 (0–1,5) | 2 (0–3) | 0 (0–2,5) | 0 (0–1) |
| 54 | 20,5 (11–24) | 0 (0–2,5) | 0 (0–2) | 1,5 (0–3) | 1 (0–2) | 0 (0–1) |
| 55 | 22 (12,5–27,5) | 1 (0–2) | 0 (0–1,5) | 1,5 (0–3) | 1 (0–3) | 0 |
| 56 | 18,5 (9,5–25) | 1 (0–2) | 0 (0–1,5) | 1,5 (0–3) | 0,5 (0–2) | 0 (0–1) |
| 57 | 20,5 (8,5–27,5) | 1 (0–3) | 0 (0–2) | 2 (0–4) | 0,5 (0–3,5) | 0 (0–1) |
| 58 | 22,5 (11,5–31) | 0,5 (0–2) | 0 (0–1,5) | 1 (0–3,5) | 0 (0–2,5) | 0 |
| 59 | 18 (9,5–28,5) | 1 (0–3) | 0 (0–1,5) | 2 (0–3,5) | 1 (0–1,5) | 0 (0–0,5) |
| 60 | 16,5 (9–25,5) | 1 (0–2,5) | 0 (0–2) | 2 (0–3) | 1 (0–2) | 0 (0–0,5) |

### Savaş (2/2)

| Yıl | Pakt gereği savaş | Kutsal Sefer çağrısı | İhanet (pakt çiğnendi) | Savunma paktı (yıl sonu) |
|---|---|---|---|---|
| 1 | 0 (0–0,5) | 0 | 0 | 1 (0–2,5) |
| 2 | 0 (0–1,5) | 0 | 0 | 1 (0–2,5) |
| 3 | 0 (0–1) | 0 | 0 | 1 (0–2,5) |
| 4 | 0 (0–1) | 0 | 0 | 1 (0–2,5) |
| 5 | 0 (0–1) | 0 | 0 | 1,5 (0–2,5) |
| 6 | 0 (0–1) | 0 | 0 | 1 (0–2,5) |
| 7 | 0 (0–1) | 0 (0–1) | 0 | 1 (0,5–2,5) |
| 8 | 0,5 (0–1) | 0 (0–0,5) | 0 | 1 (0,5–2) |
| 9 | 0 (0–1) | 0 | 0 (0–0,5) | 1 (0,5–2) |
| 10 | 0 (0–1) | 0 (0–1) | 0 | 1 (0,5–2) |
| 11 | 0 (0–1) | 0 | 0 | 1 (0,5–2) |
| 12 | 0 | 0 | 0 | 1 (0,5–2) |
| 13 | 0 | 0 (0–1) | 0 (0–1) | 1 (0–2) |
| 14 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 15 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 16 | 0 (0–1) | 0 | 0 | 1 (1–2) |
| 17 | 0 (0–1) | 0 (0–0,5) | 0 | 1 (1–2) |
| 18 | 0 (0–1) | 0 | 0 | 1,5 (1–2) |
| 19 | 0 (0–0,5) | 0 | 0 (0–1) | 1 (0,5–2) |
| 20 | 0 (0–1) | 0 | 0 | 1 (0,5–2) |
| 21 | 0 (0–0,5) | 0 (0–1) | 0 | 1 (0,5–2) |
| 22 | 0 (0–1) | 0 | 0 | 1 (1–2) |
| 23 | 0 (0–1) | 0 (0–1) | 0 | 1 (1–2) |
| 24 | 0 (0–0,5) | 0 (0–0,5) | 0 | 1 (0,5–2) |
| 25 | 0 (0–1) | 0 | 0 | 1 (0,5–2) |
| 26 | 0 (0–1) | 0 | 0 | 1 (1–2) |
| 27 | 0 | 0 (0–1) | 0 | 1,5 (1–2) |
| 28 | 0 (0–1) | 0 | 0 | 2 (1–2) |
| 29 | 0 (0–1) | 0 | 0 | 2 (1–2) |
| 30 | 0 | 0 (0–1) | 0 | 1,5 (1–2) |
| 31 | 0 (0–1) | 0 (0–0,5) | 0 | 1,5 (1–2) |
| 32 | 0 (0–1) | 0 | 0 | 2 (1–2) |
| 33 | 0 (0–0,5) | 0 (0–0,5) | 0 | 2 (1–2) |
| 34 | 0 (0–0,5) | 0 (0–0,5) | 0 | 1,5 (1–2) |
| 35 | 0 | 0 | 0 | 1,5 (1–2) |
| 36 | 0 (0–1) | 0 | 0 | 1 (1–2) |
| 37 | 0 | 0 | 0 (0–0,5) | 1 (1–2) |
| 38 | 0 (0–1) | 0 (0–0,5) | 0 | 1 (1–2) |
| 39 | 0 (0–0,5) | 0 | 0 | 1 (1–2) |
| 40 | 0 (0–0,5) | 0 | 0 | 1 (0,5–2) |
| 41 | 0 | 0 | 0 (0–0,5) | 1 (0–2) |
| 42 | 0 (0–0,5) | 0 | 0 | 1 (0,5–2) |
| 43 | 0 (0–0,5) | 0 | 0 | 1 (1–2) |
| 44 | 0 | 0 (0–0,5) | 0 | 1 (0,5–2) |
| 45 | 0 | 0 | 0 | 1 (0–2) |
| 46 | 0 | 0 | 0 | 1 (0–2) |
| 47 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 48 | 0 | 0 | 0 | 1 (0–2) |
| 49 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 50 | 0 | 0 | 0 | 1 (0–2) |
| 51 | 0 (0–0,5) | 0 | 0 | 1 (0,5–2) |
| 52 | 0 | 0 (0–1) | 0 | 1 (0,5–2) |
| 53 | 0 | 0 | 0 (0–0,5) | 1 (0–2) |
| 54 | 0 (0–1) | 0 | 0 | 1 (0,5–2) |
| 55 | 0 | 0 | 0 | 1 (0,5–2) |
| 56 | 0 (0–1) | 0 | 0 | 1 (0,5–2) |
| 57 | 0 | 0 | 0 | 1 (0–2) |
| 58 | 0 | 0 | 0 | 1 (0–2) |
| 59 | 0 | 0 | 0 | 1 (0–2) |
| 60 | 0 (0–0,5) | 0 (0–0,5) | 0 | 1 (0–2) |

### Canavarlar (1/2)

| Yıl | Yaşayan kamp (yıl sonu) | Yaşayan kamp (yıl ort.) | Doğan kamp | Temizlenen kamp | Canavar baskını | Yaşayan trol ini (yıl sonu) |
|---|---|---|---|---|---|---|
| 1 | 8 (7,5–10) | 8,1 (7,61–9,25) | 8,5 (1,5–12) | 8 (3–12) | 4 (2–6) | 2 (1–3) |
| 2 | 8 (7–10) | 8,28 (7,55–9,63) | 7 (2–10,5) | 6,5 (2,5–11) | 3,5 (1,5–7,5) | 1,5 (0,5–3) |
| 3 | 8,5 (7,5–10) | 7,99 (7,8–9,74) | 8,5 (2,5–10,5) | 7,5 (3–10) | 3 (1,5–4,5) | 2 (1–3) |
| 4 | 8 (7,5–10) | 8,4 (7,48–9,51) | 7 (2–12) | 7 (1,5–12) | 3 (1–5) | 2 (1–3) |
| 5 | 8,5 (7,5–9,5) | 8,28 (7,56–9,7) | 7 (2–11) | 6,5 (3–11,5) | 4 (2–6,5) | 2 (1–3) |
| 6 | 8 (7,5–9) | 7,95 (7,4–9) | 6 (2–12,5) | 6 (2,5–12) | 4 (1–6) | 2 (1–3) |
| 7 | 8 (7–10) | 8,05 (7,48–9,71) | 7 (2–11,5) | 7,5 (1,5–11,5) | 3,5 (1–5) | 2 (1–3) |
| 8 | 8 (7,5–9,5) | 8,19 (7,61–9,63) | 6,5 (3–10,5) | 6 (3,5–10) | 4,5 (2–7) | 2,5 (1–3) |
| 9 | 8,5 (7,5–10) | 8,4 (7,68–9,54) | 7,5 (2,5–11,5) | 7 (3–11,5) | 3 (2–6) | 3 (1–3) |
| 10 | 9 (8–10) | 8,2 (7,59–10) | 5,5 (3–11) | 6,5 (2–10,5) | 3 (1–5) | 2 (1,5–3) |
| 11 | 8,5 (7,5–10) | 8,73 (7,89–9,45) | 7 (3–10) | 6 (3,5–11,5) | 2 (1–4) | 2 (1–3) |
| 12 | 8 (7–9) | 8,3 (7,73–9,29) | 6 (2,5–10,5) | 6 (3–10,5) | 3 (1,5–5,5) | 2,5 (1,5–3) |
| 13 | 8 (7,5–9,5) | 8,15 (7,53–9,13) | 8 (3,5–11,5) | 7,5 (3–11,5) | 3,5 (2–6) | 2,5 (1,5–3) |
| 14 | 9 (8–9) | 8,55 (7,79–9,33) | 6,5 (3,5–11,5) | 6 (3,5–11,5) | 4 (2–5) | 2,5 (1–3) |
| 15 | 9 (8–10) | 8,81 (8,09–9,6) | 7,5 (3–11) | 7,5 (2,5–11) | 3,5 (1,5–6) | 2 (1–3) |
| 16 | 9 (8–11,5) | 8,78 (8,19–10,7) | 8 (3–12) | 6 (3,5–11,5) | 4 (2–5,5) | 2 (1–3) |
| 17 | 9 (8–11) | 9,29 (8,25–10,5) | 7 (2–11) | 6,5 (2,5–10,5) | 5 (3–6) | 2 (1–3) |
| 18 | 10 (8,5–10,5) | 9,31 (8,65–10,5) | 6,5 (2,5–10) | 5,5 (1,5–10) | 4 (3–5,5) | 3 (2–3) |
| 19 | 10 (8–11) | 9,38 (8,11–10,9) | 5,5 (3–11) | 5,5 (2,5–12) | 4,5 (3–8) | 2 (1–3) |
| 20 | 9 (7–11) | 8,78 (8,03–10,1) | 7 (1–10,5) | 6,5 (2,5–11,5) | 4 (3–6,5) | 2,5 (1–3) |
| 21 | 9 (7–10) | 8,64 (8,25–10,6) | 6,5 (1–11) | 8 (1,5–11) | 4 (1,5–6,5) | 2 (1–3) |
| 22 | 9 (8–11) | 9,06 (8,08–10,6) | 8 (3–12,5) | 7,5 (2,5–11,5) | 4 (2–6) | 3 (1–3) |
| 23 | 9 (8–11) | 9,09 (8,21–10,7) | 6,5 (2–11) | 7 (2,5–12) | 4 (1,5–6) | 3 (1–3) |
| 24 | 10 (8–11) | 9,05 (8,11–10,8) | 7,5 (1–11) | 6,5 (1–11) | 5 (1,5–7) | 2 (0,5–3) |
| 25 | 9 (8–10) | 8,76 (8,04–10,5) | 6 (2,5–11,5) | 6 (3–11,5) | 4,5 (1,5–8) | 3 (2–3) |
| 26 | 9 (8–11) | 9,04 (7,91–10,2) | 6 (3–12) | 7 (3–11) | 4 (2–7) | 3 (1–3) |
| 27 | 9,5 (8–11) | 9,19 (8,41–10,3) | 6,5 (2–11,5) | 5,5 (2–11,5) | 4 (2–6) | 3 (2–3) |
| 28 | 9 (8–11) | 8,86 (8,1–10,3) | 4,5 (2,5–12) | 5 (1,5–12) | 3 (0,5–6) | 3 (1–3) |
| 29 | 9 (7–11) | 8,73 (7,54–10,4) | 5 (2–11,5) | 5,5 (2–12,5) | 3 (1,5–5) | 3 (1,5–3) |
| 30 | 9 (7–10) | 8,86 (7,4–10,3) | 6,5 (1,5–11) | 6 (2–11) | 3,5 (2–5,5) | 3 (2–3) |
| 31 | 9 (8–10) | 9,1 (7,66–9,64) | 7 (1,5–12) | 6,5 (2,5–11) | 4 (3–7) | 3 (1,5–3) |
| 32 | 10 (8–10,5) | 9,28 (7,91–9,91) | 5,5 (2–10,5) | 6 (2–10,5) | 3 (1,5–6) | 3 (2–3) |
| 33 | 10 (8–11) | 9,46 (7,94–10,3) | 6 (2–9,5) | 4,5 (1,5–10) | 4 (2–6,5) | 3 (1,5–3) |
| 34 | 9 (7,5–10) | 9,15 (7,93–9,96) | 5 (0,5–11) | 5 (2–11,5) | 4 (1–6) | 3 (1–3) |
| 35 | 9 (8–10) | 9,18 (7,6–10,1) | 6,5 (1,5–12) | 5,5 (1–10,5) | 4 (2,5–6) | 3 (1–3) |
| 36 | 9 (8–10) | 9,11 (7,84–9,98) | 5 (2,5–10,5) | 5,5 (2–10,5) | 3,5 (1–6,5) | 3 (1,5–3) |
| 37 | 9 (8–10,5) | 9,14 (7,83–10,1) | 5 (2–12) | 5,5 (1–12) | 4,5 (1–7) | 2 (1–3) |
| 38 | 9 (8–10) | 9,34 (7,83–10) | 7 (2,5–12) | 6 (2,5–12,5) | 3,5 (0,5–6,5) | 3 (2–3) |
| 39 | 9 (8–10,5) | 8,74 (7,94–10,5) | 6,5 (0,5–10,5) | 6,5 (1,5–11) | 4 (2–6,5) | 3 (1,5–3) |
| 40 | 9 (7,5–11) | 9,09 (7,7–10,2) | 5,5 (1–10) | 5 (1–10,5) | 4 (2–7,5) | 3 (2–3) |
| 41 | 8,5 (7–10) | 8,83 (7,54–10,1) | 3,5 (1–10) | 5,5 (1–11) | 4 (0,5–5,5) | 3 (1,5–3) |
| 42 | 9 (8–10) | 8,88 (7,53–10,3) | 5,5 (2–12) | 5,5 (2–11) | 3 (1,5–6) | 3 (1,5–3) |
| 43 | 9 (7,5–10,5) | 8,74 (7,96–10,1) | 7 (2–10,5) | 7 (1,5–12) | 4 (2–6,5) | 3 (1,5–3) |
| 44 | 9 (7–10,5) | 8,85 (7,58–10,3) | 7 (1–11) | 7 (2–11) | 3 (1–6) | 3 (1–3) |
| 45 | 9 (7,5–10,5) | 9,04 (7,41–10,1) | 8 (1–12) | 8 (0,5–12) | 3 (1–6,5) | 3 (2–3) |
| 46 | 9 (8,5–10,5) | 9,08 (7,64–10,2) | 7 (1–13,5) | 7 (1–12) | 3 (2–5) | 3 (1,5–3) |
| 47 | 9 (8–10) | 9,21 (7,94–10,3) | 7 (2–11,5) | 7 (2–12) | 4 (0,5–5,5) | 3 (2–3) |
| 48 | 9 (7,5–10) | 9,25 (7,9–10,3) | 6 (2–11) | 6 (1,5–12) | 4 (2–5) | 2,5 (2–3) |
| 49 | 9 (8–11) | 9,31 (7,33–10,7) | 5 (1,5–12) | 5 (1,5–11,5) | 4 (1,5–6) | 3 (2–3) |
| 50 | 9 (8–11) | 9,4 (7,6–10,8) | 6,5 (1,5–12) | 7,5 (1,5–11,5) | 4,5 (1–6,5) | 3 (2–3) |
| 51 | 9 (7–10) | 9,1 (8,03–10,4) | 6 (1,5–10) | 6 (1,5–10,5) | 4 (1–5,5) | 3 (2–3) |
| 52 | 9 (8–10,5) | 8,91 (7,39–10,1) | 6 (2,5–11) | 5 (2,5–11) | 4 (2–5) | 3 (2,5–3) |
| 53 | 8,5 (7,5–10,5) | 9,31 (7,84–10,3) | 3,5 (1,5–9,5) | 4,5 (1,5–10,5) | 3 (1–5) | 3 (1–3) |
| 54 | 9 (8–10,5) | 9,09 (7,55–10,5) | 6 (0,5–12,5) | 5,5 (1–12,5) | 4 (2–6) | 3 (2–3) |
| 55 | 9 (7–11) | 9,01 (7,48–10,7) | 5,5 (1,5–11,5) | 4,5 (2–11,5) | 3 (2,5–6) | 3 (2,5–3) |
| 56 | 9,5 (8–10,5) | 9,13 (7,78–10,1) | 5 (1,5–9) | 6 (1–9) | 4 (1,5–6) | 3 (2–3) |
| 57 | 10 (8,5–11,5) | 8,98 (7,73–10,7) | 6 (1,5–10,5) | 5,5 (0,5–11) | 3,5 (2–6) | 3 (1,5–3) |
| 58 | 9 (7,5–11) | 9,04 (8–11,1) | 7 (1–12,5) | 8 (2,5–12,5) | 4 (1,5–6,5) | 3 (2–3) |
| 59 | 8 (7–10) | 8,99 (7,56–9,91) | 6 (0,5–11) | 5,5 (2–12) | 2 (0,5–6) | 3 |
| 60 | 9 (8–10) | 8,61 (7,66–10,4) | 6 (1–12) | 6 (2–10) | 3 (1–5) | 3 (2–3) |

### Canavarlar (2/2)

| Yıl | Yaşayan ejderha (yıl sonu) | Ejderha akını | Kriz (anlatıcı) | Rahatlama dönemi (anlatıcı) |
|---|---|---|---|---|
| 1 | 0 | 0 | 2 (0–4) | 0 (0–0,5) |
| 2 | 0 | 0 | 3 (1–4,5) | 0 |
| 3 | 0 | 0 | 3 (1,5–4,5) | 0 (0–0,5) |
| 4 | 0 | 0 | 3 (1–5) | 0 (0–1) |
| 5 | 0 | 0 | 2 (1–4,5) | 0 (0–1) |
| 6 | 0 | 0 | 1 (0–3) | 0 (0–1) |
| 7 | 0 | 0 | 2 (1–4) | 0 (0–0,5) |
| 8 | 0 | 0 | 2,5 (0,5–4,5) | 0 (0–0,5) |
| 9 | 0 | 0 | 2,5 (1–4) | 0 (0–1) |
| 10 | 0 | 0 | 3 (1–4,5) | 0 |
| 11 | 0 | 0 | 2,5 (0,5–3,5) | 0 |
| 12 | 0 | 0 | 3,5 (0,5–5) | 0 |
| 13 | 0 | 0 | 2 (1–4,5) | 0 (0–1) |
| 14 | 0 (0–1) | 0 (0–0,5) | 3 (1,5–5) | 0 (0–0,5) |
| 15 | 0,5 (0–1) | 0,5 (0–2) | 3 (2–4,5) | 0 (0–1) |
| 16 | 1 | 1 (0–2) | 3 (1,5–5) | 0 (0–1) |
| 17 | 1 (0–1) | 1 (0–2) | 2 (1–4,5) | 0 (0–1) |
| 18 | 1 (0–1) | 0,5 (0–2) | 3 (1–4) | 0 (0–1) |
| 19 | 1 (0–1) | 1 (0–2) | 2,5 (1–5) | 0 (0–1) |
| 20 | 1 (0–1) | 1 (0–2) | 2,5 (1–6) | 0 (0–1) |
| 21 | 1 (0–1) | 0,5 (0–2) | 1 (0,5–4) | 0 (0–0,5) |
| 22 | 1 (0–1) | 0,5 (0–2) | 3 (2–5) | 0 (0–0,5) |
| 23 | 1 (0–1) | 0 (0–1,5) | 2 (0,5–4) | 0 (0–0,5) |
| 24 | 1 (0–1) | 0 (0–1) | 2,5 (1–3,5) | 0 (0–1) |
| 25 | 1 (0–1) | 0,5 (0–2) | 3 (1,5–4,5) | 0 (0–0,5) |
| 26 | 1 (0–1) | 1 (0–2) | 3 (1,5–4) | 0 (0–1) |
| 27 | 0,5 (0–1) | 0,5 (0–2) | 2,5 (1–4) | 0 (0–1) |
| 28 | 0,5 (0–1) | 0 (0–2) | 3 (2–4) | 0 (0–0,5) |
| 29 | 0,5 (0–1) | 0 (0–1,5) | 3 (1,5–4) | 0 (0–1) |
| 30 | 0,5 (0–1) | 0 (0–1,5) | 2 (2–3,5) | 0 |
| 31 | 0 (0–1) | 0 (0–2) | 3 (1,5–5) | 0 (0–1) |
| 32 | 0 (0–1) | 0 (0–1,5) | 3 (1,5–4,5) | 0 |
| 33 | 0 (0–1) | 0 (0–1,5) | 2 (1–3) | 0 |
| 34 | 0 (0–1) | 0 (0–2) | 2 (1,5–5) | 0 (0–0,5) |
| 35 | 0 (0–1) | 0 (0–1) | 2,5 (0,5–5) | 0 (0–1) |
| 36 | 0 (0–1) | 0 (0–2) | 3 (2–5) | 0 |
| 37 | 0 (0–1) | 0 (0–2) | 3 (1–4) | 0 (0–1) |
| 38 | 0 (0–1) | 0 (0–1) | 3 (1,5–4) | 0 (0–1) |
| 39 | 0 (0–1) | 0 (0–1) | 2,5 (1–4) | 0 (0–1) |
| 40 | 0 (0–1) | 0 (0–2) | 2 (1,5–5) | 0 |
| 41 | 0 (0–1) | 0 (0–1) | 3 (2–4,5) | 0 |
| 42 | 0 (0–1) | 0 (0–1) | 3 (2–5) | 0 (0–0,5) |
| 43 | 0 (0–1) | 0 (0–1,5) | 2 (1–5) | 0 (0–1) |
| 44 | 0 (0–1) | 0 (0–2) | 3 (1–4,5) | 0 |
| 45 | 0 (0–1) | 0 (0–1,5) | 1 (0–5,5) | 0 (0–1) |
| 46 | 0 (0–1) | 0 (0–1,5) | 2 (1–5,5) | 0 (0–0,5) |
| 47 | 0 (0–1) | 0 (0–2) | 3 (1–4,5) | 0 (0–1) |
| 48 | 0 (0–1) | 0 (0–1) | 2 (1–4) | 0 (0–1) |
| 49 | 0 (0–1) | 0 (0–1,5) | 2 (1–4) | 0 (0–1) |
| 50 | 0 (0–1) | 0 (0–2) | 2,5 (1–4) | 0 |
| 51 | 0 (0–1) | 0 (0–2) | 3 (1–5) | 0 (0–1) |
| 52 | 0 (0–1) | 0 (0–1) | 2,5 (0,5–5) | 0 (0–1) |
| 53 | 0 (0–1) | 0 (0–1,5) | 3 (0–5,5) | 0 |
| 54 | 0 (0–1) | 0 (0–1) | 2 (1–5) | 0 |
| 55 | 0 (0–1) | 0 (0–1,5) | 3 (1–4,5) | 0 (0–1) |
| 56 | 0 (0–1) | 0 (0–1,5) | 2 (1–3,5) | 0 |
| 57 | 0 (0–1) | 0 (0–1) | 2 (0,5–5,5) | 0 |
| 58 | 0 (0–1) | 0 (0–2) | 3 (1–5) | 0 (0–0,5) |
| 59 | 0 (0–1) | 0 (0–1) | 3 (1,5–4,5) | 0 (0–1) |
| 60 | 0 (0–1) | 0 (0–2) | 2 (1–6) | 0 |

### Kahramanlar (1/2)

| Yıl | Doğan kahraman | Ölen kahraman | Emekli olan kahraman | Diyarı terk eden kahraman | Ölümden dönen kahraman | Efsane olan kahraman |
|---|---|---|---|---|---|---|
| 1 | 10,5 (4–16,5) | 2 (0,5–4,5) | 1 (0–3,5) | 4,5 (0,5–8,5) | 0 | 0 |
| 2 | 9 (5–15,5) | 1 (1–4) | 1 (0–4) | 5,5 (0–11,5) | 0 | 0 |
| 3 | 10,5 (5–16,5) | 3 (1–7,5) | 1,5 (0–4) | 5,5 (1–12,5) | 0 | 0 |
| 4 | 10 (3,5–16,5) | 3 (1–4,5) | 2 (0–6) | 3,5 (0–11,5) | 0 | 0 |
| 5 | 10 (5,5–15) | 2 (0–6,5) | 1 (0–3) | 4 (1–10) | 0 | 0 |
| 6 | 10 (6–17,5) | 4 (1–6,5) | 2 (0–4) | 5 (1–13) | 0 | 0 |
| 7 | 10 (5,5–16,5) | 2 (0,5–6) | 1,5 (0–3,5) | 2 (0–10) | 0 | 0 |
| 8 | 9,5 (5,5–16) | 3 (1–7) | 2 (0,5–5) | 7 (0–13) | 0 | 0 |
| 9 | 11 (5–17,5) | 2 (0,5–7) | 2 (0,5–4,5) | 6 (1–10,5) | 0 | 0 |
| 10 | 12 (4,5–16) | 3 (1–7,5) | 2 (0–4,5) | 5,5 (1–9) | 0 | 0 |
| 11 | 11 (4,5–16,5) | 3 (0,5–6) | 2,5 (0–4) | 4,5 (2–12) | 0 | 0 |
| 12 | 11 (5–15) | 4 (1–6) | 2 (1,5–3) | 3,5 (0,5–10,5) | 0 | 0 |
| 13 | 11 (4,5–16) | 3 (0–5,5) | 2 (0,5–3) | 5 (0–10,5) | 0 | 0 |
| 14 | 11,5 (3,5–14) | 5 (1–9) | 2 (0–4) | 5,5 (1–10,5) | 0 | 0 |
| 15 | 11 (6–16,5) | 4,5 (1,5–7,5) | 1,5 (0,5–3,5) | 3 (0–11) | 0 | 0 |
| 16 | 11 (7–18) | 5 (1,5–11,5) | 1 (0–3,5) | 5 (0–8) | 0 | 0 (0–0,5) |
| 17 | 11 (5,5–16,5) | 5 (2–7,5) | 2 (0,5–4) | 4,5 (0,5–10,5) | 0 | 0 (0–1,5) |
| 18 | 11,5 (6,5–16) | 4,5 (0,5–11) | 1 (0,5–4) | 5,5 (1,5–16) | 0 | 0 |
| 19 | 12 (6,5–18) | 4,5 (1,5–9,5) | 1,5 (1–3) | 5 (0,5–13,5) | 0 | 0 |
| 20 | 11 (4,5–16,5) | 4 (1,5–11,5) | 2 (0–4) | 4 (1–12) | 0 | 0 |
| 21 | 13 (9–17,5) | 3,5 (0–7,5) | 2 (0–3,5) | 4 (0,5–11) | 0 | 0 (0–0,5) |
| 22 | 10 (6–13,5) | 4 (1,5–11) | 2 (1–4) | 4 (2–12,5) | 0 | 0 |
| 23 | 12,5 (3,5–15,5) | 4,5 (1,5–9) | 2,5 (0–5) | 3,5 (0,5–11,5) | 0 | 0 (0–0,5) |
| 24 | 11,5 (5,5–18) | 5 (2–9) | 1 (0–3) | 3 (1–11,5) | 0 | 0 |
| 25 | 10 (6–16) | 5 (1,5–11) | 2 (0–3,5) | 6,5 (0,5–13) | 0 | 0 (0–0,5) |
| 26 | 11,5 (4,5–18,5) | 4 (0,5–7,5) | 2,5 (0–4) | 4 (1–11) | 0 | 0 |
| 27 | 12,5 (7–17,5) | 5 (2–7,5) | 2 (1–3,5) | 4,5 (1,5–13,5) | 0 | 0 |
| 28 | 12,5 (8–19,5) | 4 (2–7,5) | 2 (1–6) | 4 (1–11) | 0 | 0 (0–0,5) |
| 29 | 9,5 (7–18,5) | 4 (2–10,5) | 2 (0–3,5) | 4,5 (2–11) | 0 | 0 |
| 30 | 11 (5,5–19) | 3,5 (0–8) | 2 (0–3) | 3 (1–13,5) | 0 | 0 |
| 31 | 11 (4,5–19) | 3,5 (0,5–13,5) | 2,5 (1–4,5) | 4 (2–10,5) | 0 | 0 (0–0,5) |
| 32 | 12 (5,5–18) | 3 (1,5–5,5) | 2 (1–4) | 3,5 (0,5–15,5) | 0 | 0 |
| 33 | 10,5 (7–17,5) | 3 (2–7,5) | 1,5 (0–3) | 6,5 (0–11) | 0 | 0 |
| 34 | 13,5 (7–17) | 5 (3–9) | 3 (1–4) | 4,5 (1–10,5) | 0 | 0 |
| 35 | 10,5 (6–20) | 3,5 (2–10) | 2 (0,5–4) | 6,5 (2–18) | 0 | 0 |
| 36 | 10 (6–19) | 4 (1,5–5,5) | 2 (0–3,5) | 5 (1–11) | 0 | 0 (0–0,5) |
| 37 | 12 (7,5–15,5) | 5,5 (2,5–10) | 1,5 (0,5–4) | 5 (1–14,5) | 0 | 0 |
| 38 | 12 (5,5–17,5) | 3,5 (0,5–8,5) | 2 (0–6) | 6 (1–16,5) | 0 | 0 |
| 39 | 12 (6,5–18,5) | 4 (1,5–8,5) | 2,5 (1–3) | 6 (1–13,5) | 0 | 0 (0–1) |
| 40 | 12,5 (6,5–15,5) | 4 (0,5–7) | 2 (1–3) | 6 (1–13,5) | 0 | 0 |
| 41 | 12 (8–16,5) | 3 (1,5–9,5) | 2 (0–3,5) | 3,5 (0–9,5) | 0 | 0 |
| 42 | 12,5 (6–18) | 4 (1–8,5) | 2,5 (0–5) | 7,5 (1–10,5) | 0 | 0 |
| 43 | 12 (8–17) | 7 (2,5–13) | 1 (0–3) | 4 (1–10,5) | 0 | 0 |
| 44 | 11 (6–18) | 5,5 (0,5–8,5) | 2 (0–3,5) | 5 (2,5–10) | 0 | 0 |
| 45 | 10,5 (7,5–15) | 6,5 (1–13) | 1 (0–2) | 5,5 (2,5–10,5) | 0 | 0 (0–0,5) |
| 46 | 12 (6,5–20) | 4 (1,5–7,5) | 2 (0–3,5) | 4,5 (1,5–9) | 0 | 0 |
| 47 | 13,5 (6–18,5) | 4 (0,5–12,5) | 2 (0–3,5) | 5 (1,5–10,5) | 0 | 0 |
| 48 | 11,5 (7–17,5) | 4 (1–7) | 2 (0,5–4) | 4,5 (1,5–13,5) | 0 | 0 |
| 49 | 12 (8,5–17,5) | 4 (0,5–8,5) | 2 (0–3,5) | 8,5 (3–10,5) | 0 | 0 |
| 50 | 13 (7–18) | 5 (1–11,5) | 2,5 (1–4,5) | 5 (1,5–11,5) | 0 | 0 |
| 51 | 12,5 (5,5–18) | 4 (2–9) | 2,5 (0,5–3) | 6 (2,5–13,5) | 0 | 0 |
| 52 | 12 (7,5–17,5) | 4 (2,5–7) | 2 (0–4,5) | 3,5 (1,5–7,5) | 0 | 0 |
| 53 | 14,5 (5,5–19,5) | 5,5 (1,5–11,5) | 1,5 (1–3) | 6,5 (1,5–12,5) | 0 | 0 |
| 54 | 13 (6,5–19,5) | 3 (2–5,5) | 2 (1–4) | 6,5 (2–13,5) | 0 | 0 |
| 55 | 11,5 (8–16) | 4,5 (1–9,5) | 1 (0–4,5) | 5,5 (1,5–14,5) | 0 | 0 |
| 56 | 14,5 (7–21,5) | 4,5 (1–9) | 2,5 (1,5–4) | 7 (1,5–11,5) | 0 | 0 |
| 57 | 12,5 (5,5–18,5) | 4 (0–8) | 2 (1–3,5) | 5,5 (1,5–10) | 0 | 0 (0–0,5) |
| 58 | 13 (7,5–19,5) | 6,5 (2–12) | 3 (0,5–4,5) | 6 (1–14,5) | 0 | 0 |
| 59 | 12 (6,5–21) | 4 (1,5–9,5) | 2 (1–3,5) | 7 (1,5–13) | 0 | 0 |
| 60 | 14,5 (8–20,5) | 4 (0,5–9) | 3 (0,5–5) | 4 (2–18) | 0 | 0 (0–0,5) |

### Kahramanlar (2/2)

| Yıl | Yaşayan kahraman (yıl sonu) | Doğuş seviyesi (ort.) | Ölüm seviyesi (ort.) | Yaşayan kahraman seviyesi (ort.) | En yüksek seviye (şimdiye dek) |
|---|---|---|---|---|---|
| 1 | 143 (94,5–160) | 1,71 (1,34–2) | 3,5 (1,55–6,3) | 4,68 (4,06–5,36) | 10 |
| 2 | 142 (97–160) | 1,59 (1,38–2) | 5 (1–6,13) | 4,68 (4,05–5,43) | 10 |
| 3 | 143 (92–158) | 1,6 (1,33–1,78) | 3,94 (2,19–5,55) | 4,74 (4,09–5,37) | 10 |
| 4 | 146 (93,5–155) | 1,73 (1,34–2) | 4,33 (2,4–5,73) | 4,77 (4,12–5,3) | 10 |
| 5 | 146 (93,5–159) | 1,55 (1,22–1,76) | 4 (2,6–6,8) | 4,73 (4,11–5,28) | 10 |
| 6 | 144 (95,5–159) | 1,52 (1,35–1,89) | 4,5 (2,33–6,53) | 4,78 (4,18–5,32) | 10 |
| 7 | 146 (96,5–160) | 1,62 (1,44–1,92) | 4,5 (2,88–8,47) | 4,74 (4,14–5,32) | 10 |
| 8 | 148 (97,5–160) | 1,56 (1,39–1,92) | 3,67 (2,16–6,23) | 4,71 (4,21–5,35) | 10 |
| 9 | 150 (98–158) | 1,65 (1,37–1,86) | 4,67 (2,58–6,76) | 4,58 (4,2–5,35) | 10 |
| 10 | 150 (98,5–164) | 1,65 (1,45–1,94) | 4,5 (2,7–5,48) | 4,57 (4,22–5,4) | 10 |
| 11 | 152 (99–162) | 1,69 (1,32–2) | 4 (3–5,8) | 4,58 (4,16–5,41) | 10 |
| 12 | 150 (99,5–162) | 1,64 (1,4–1,8) | 3,96 (2,5–6,58) | 4,51 (4,2–5,41) | 10 |
| 13 | 150 (100–162) | 1,67 (1,33–1,88) | 4,9 (3–5,5) | 4,59 (4,09–5,39) | 10 |
| 14 | 149 (96,5–164) | 1,63 (1,46–1,85) | 5,15 (3,77–6,33) | 4,56 (4,21–5,46) | 10 |
| 15 | 150 (95,5–165) | 1,65 (1,32–1,85) | 4,93 (3,42–6) | 4,47 (4,25–5,48) | 10 |
| 16 | 152 (96,5–167) | 1,59 (1,33–1,92) | 4,63 (4,06–6,56) | 4,51 (4,24–5,38) | 10 |
| 17 | 155 (96–166) | 1,58 (1,33–1,97) | 4,45 (3,63–6,42) | 4,47 (4,25–5,3) | 10 |
| 18 | 150 (95,5–165) | 1,62 (1,37–1,8) | 4,5 (2,71–5,41) | 4,56 (4,21–5,43) | 10 |
| 19 | 152 (96–166) | 1,68 (1,34–2,14) | 4,67 (3–6,1) | 4,58 (4,25–5,36) | 10 |
| 20 | 154 (95,5–164) | 1,61 (1,28–1,96) | 4,61 (3,25–5,63) | 4,64 (4,22–5,34) | 10 |
| 21 | 154 (99,5–168) | 1,56 (1,41–1,95) | 4,67 (3,37–5,74) | 4,54 (4,23–5,15) | 10 |
| 22 | 155 (99–166) | 1,57 (1,42–1,89) | 4,94 (3,5–6,73) | 4,62 (4,25–5,28) | 10 |
| 23 | 156 (99,5–168) | 1,62 (1,26–1,82) | 5,27 (3,88–6,26) | 4,61 (4,18–5,24) | 10 |
| 24 | 160 (99–169) | 1,63 (1,21–1,98) | 4,16 (2,75–5,18) | 4,62 (4,19–5,36) | 10 |
| 25 | 159 (100–168) | 1,56 (1,33–1,97) | 5,17 (3,88–7,58) | 4,66 (4,17–5,45) | 10 |
| 26 | 158 (100–169) | 1,67 (1,15–1,72) | 4,88 (3,77–6,36) | 4,71 (4,19–5,46) | 10 |
| 27 | 158 (98–170) | 1,59 (1,29–1,8) | 5 (4,45–6,32) | 4,67 (4,18–5,43) | 10 |
| 28 | 156 (99,5–172) | 1,48 (1,2–1,84) | 5,06 (2,5–6,82) | 4,64 (4,1–5,3) | 10 |
| 29 | 156 (103–174) | 1,56 (1,33–1,78) | 4,67 (2,64–6,6) | 4,61 (4,12–5,44) | 10 |
| 30 | 156 (102–172) | 1,58 (1,39–1,83) | 4,33 (3,27–6,22) | 4,62 (4,12–5,54) | 10 |
| 31 | 148 (100–175) | 1,6 (1,45–1,85) | 4,89 (2,86–9,4) | 4,59 (4,16–5,56) | 10 |
| 32 | 152 (103–174) | 1,57 (1,5–1,82) | 5 (3,4–7,6) | 4,64 (4,14–5,64) | 10 |
| 33 | 154 (109–176) | 1,44 (1,34–1,75) | 5,07 (3,54–6,75) | 4,52 (4,18–5,58) | 10 |
| 34 | 155 (110–176) | 1,67 (1,43–1,74) | 4,45 (3,55–5,88) | 4,49 (4,16–5,55) | 10 |
| 35 | 154 (111–172) | 1,55 (1,39–1,83) | 4,57 (2,7–7) | 4,55 (4,18–5,56) | 10 |
| 36 | 157 (113–174) | 1,58 (1,42–1,9) | 5,3 (4–6,42) | 4,46 (4,19–5,57) | 10 |
| 37 | 152 (112–176) | 1,61 (1,42–1,88) | 4,71 (3,42–5,73) | 4,41 (4,26–5,52) | 10 |
| 38 | 152 (112–176) | 1,62 (1,35–1,89) | 5 (4,04–5,68) | 4,45 (4,22–5,53) | 10 |
| 39 | 154 (112–180) | 1,62 (1,49–1,75) | 5,5 (2,93–6,54) | 4,5 (4,23–5,48) | 10 |
| 40 | 154 (113–178) | 1,57 (1,33–1,86) | 4,54 (2,88–5,75) | 4,55 (4,21–5,5) | 10 |
| 41 | 156 (116–181) | 1,57 (1,27–1,86) | 4,42 (3,75–5,67) | 4,48 (4,15–5,42) | 10 |
| 42 | 154 (116–180) | 1,5 (1,35–1,68) | 4,33 (3,16–6,18) | 4,62 (4,16–5,41) | 10 |
| 43 | 154 (122–177) | 1,63 (1,43–1,83) | 4,67 (3,66–6,56) | 4,57 (4,18–5,45) | 10 |
| 44 | 152 (122–178) | 1,65 (1,44–1,93) | 4,48 (3,38–5,5) | 4,51 (4,2–5,39) | 10 |
| 45 | 149 (121–178) | 1,63 (1,4–1,9) | 4,84 (4,08–6,2) | 4,61 (4,21–5,51) | 10 |
| 46 | 152 (118–180) | 1,64 (1,38–1,86) | 4 (2,98–6,76) | 4,52 (4,16–5,42) | 10 |
| 47 | 154 (123–180) | 1,6 (1,42–1,8) | 4,63 (3,1–5,86) | 4,54 (4,18–5,33) | 10 |
| 48 | 154 (122–178) | 1,65 (1,34–1,87) | 5 (3,47–8,12) | 4,53 (4,11–5,36) | 10 |
| 49 | 155 (124–180) | 1,62 (1,44–1,85) | 4,71 (2,65–6,7) | 4,62 (4,14–5,35) | 10 |
| 50 | 156 (124–178) | 1,66 (1,38–1,87) | 5 (4,18–5,78) | 4,59 (4,09–5,39) | 10 |
| 51 | 158 (126–176) | 1,7 (1,6–1,93) | 4 (2,76–4,56) | 4,59 (4,14–5,43) | 10 |
| 52 | 161 (128–178) | 1,75 (1,43–1,95) | 4,75 (3,85–5,43) | 4,51 (4,16–5,41) | 10 |
| 53 | 161 (128–182) | 1,6 (1,37–1,89) | 4,5 (3,48–5,73) | 4,57 (4,12–5,3) | 10 |
| 54 | 160 (132–180) | 1,71 (1,44–1,86) | 5 (2,92–7,1) | 4,56 (4,16–5,37) | 10 |
| 55 | 156 (134–182) | 1,65 (1,33–1,82) | 4,14 (2–4,75) | 4,61 (4,18–5,41) | 10 |
| 56 | 158 (136–184) | 1,63 (1,39–1,84) | 5 (3,31–6,05) | 4,58 (4,08–5,27) | 10 |
| 57 | 157 (139–190) | 1,65 (1,46–1,95) | 4,55 (3,18–6,16) | 4,61 (4,14–5,28) | 10 |
| 58 | 155 (137–186) | 1,7 (1,4–1,9) | 4,17 (3,46–6,63) | 4,67 (4,18–5,33) | 10 |
| 59 | 155 (138–179) | 1,61 (1,38–1,75) | 4,5 (3,58–6) | 4,66 (4,21–5,24) | 10 |
| 60 | 160 (138–178) | 1,68 (1,52–1,79) | 4,36 (2,1–5,85) | 4,69 (4,15–5,39) | 10 |

### Han ve ticaret

| Yıl | Ayakta han | Asılan ilan | Biten ilan | Ticaret seferi (kervan) | İkmal seferi |
|---|---|---|---|---|---|
| 1 | 4 (2,5–6) | 14 (5,5–23) | 7 (2–13,5) | 84 (63,5–107) | 36,5 (21,5–50,5) |
| 2 | 4 (2,5–5,5) | 15,5 (9–23,5) | 6,5 (3–14,5) | 82 (62–120) | 34,5 (21,5–50,5) |
| 3 | 4 (2,5–6) | 13,5 (4,5–28,5) | 6 (1–15,5) | 83 (62–120) | 34,5 (22–49,5) |
| 4 | 4 (2–6) | 13 (4,5–30,5) | 7,5 (2–17) | 85 (60–127) | 34 (23–50) |
| 5 | 4 (2,5–6) | 18,5 (7–30,5) | 7,5 (3–16) | 79 (59–120) | 35 (23–49) |
| 6 | 4 (2,5–6) | 16 (6–23) | 7,5 (1,5–16,5) | 86,5 (63–124) | 33 (22–48,5) |
| 7 | 4 (2,5–6) | 18 (4,5–25) | 8,5 (1–13) | 90 (59,5–130) | 33 (22–49,5) |
| 8 | 4,5 (2,5–6) | 17 (8,5–20) | 7,5 (3–12) | 82 (58,5–118) | 31,5 (22–52) |
| 9 | 4,5 (2,5–6) | 19 (9–27) | 8,5 (3,5–13,5) | 80,5 (45,5–124) | 32 (21,5–49) |
| 10 | 4,5 (2,5–6) | 16 (5–29) | 7 (1,5–13,5) | 86 (56–123) | 33 (22–48,5) |
| 11 | 4,5 (2,5–5,5) | 14 (7,5–25,5) | 6 (2,5–15,5) | 82,5 (55–119) | 33,5 (18–49) |
| 12 | 4 (2,5–5,5) | 16 (7,5–24,5) | 7,5 (2,5–13) | 87 (52–121) | 35,5 (19–47,5) |
| 13 | 4 (2,5–6) | 16,5 (7,5–27) | 7,5 (1,5–15,5) | 86,5 (57,5–122) | 35 (19,5–53) |
| 14 | 4 (2–6) | 16,5 (9,5–26) | 9,5 (5–13) | 80 (56–130) | 33,5 (20–60,5) |
| 15 | 4 (2–6) | 19 (8,5–28,5) | 8 (4–14) | 80,5 (61–122) | 31 (19–57,5) |
| 16 | 4,5 (2,5–6) | 20 (10–27,5) | 8,5 (3,5–13,5) | 81 (62–125) | 31,5 (20,5–57,5) |
| 17 | 4,5 (2,5–6) | 16 (8,5–26,5) | 6,5 (2–16,5) | 85,5 (65–126) | 32,5 (22,5–58) |
| 18 | 4,5 (2,5–6) | 17 (9,5–25,5) | 8 (3–14,5) | 90,5 (61–128) | 34 (21,5–56,5) |
| 19 | 4,5 (2–6) | 16 (7,5–24,5) | 6,5 (3–15) | 77 (63,5–120) | 33,5 (22,5–57) |
| 20 | 4 (2,5–5,5) | 16,5 (9,5–25) | 6,5 (2–14) | 82,5 (64,5–129) | 34 (20,5–59) |
| 21 | 4 (2,5–6) | 16 (7–28,5) | 7,5 (3–15,5) | 90 (68–128) | 35 (21,5–56,5) |
| 22 | 4 (2–6) | 18,5 (8–29) | 9 (2,5–15) | 86,5 (63,5–116) | 38 (20–57) |
| 23 | 4 (2,5–6) | 18,5 (8,5–28,5) | 6,5 (2,5–15) | 98 (64,5–118) | 39 (21,5–57,5) |
| 24 | 4 (2,5–6) | 20 (7,5–25,5) | 6 (0,5–13) | 104 (68,5–114) | 43,5 (23,5–57) |
| 25 | 4 (2,5–6) | 13 (9–26,5) | 7 (2–13,5) | 104 (74–113) | 42 (23,5–59,5) |
| 26 | 4 (2,5–6) | 17,5 (9,5–25,5) | 7 (3–15) | 103 (71,5–124) | 42 (23,5–55) |
| 27 | 4 (2,5–6) | 18,5 (7–28,5) | 9 (2–16) | 99,5 (61–124) | 40,5 (25,5–55,5) |
| 28 | 4 (2,5–6) | 16 (7–23,5) | 5,5 (2–13) | 91,5 (62,5–124) | 40,5 (25,5–57,5) |
| 29 | 4 (2,5–6) | 15 (9–28) | 7 (2,5–13,5) | 105 (67–142) | 42 (24,5–58,5) |
| 30 | 4 (2,5–6) | 15,5 (6–28,5) | 8 (3–13) | 102 (69,5–133) | 40,5 (26–55,5) |
| 31 | 4 (2,5–6) | 16 (7–24) | 7 (2,5–11) | 106 (64–137) | 39,5 (22,5–56) |
| 32 | 4 (2,5–6) | 17 (8,5–29,5) | 8 (3–13) | 104 (62,5–148) | 37,5 (21,5–56) |
| 33 | 4 (3–6) | 13,5 (6,5–25,5) | 4 (1,5–12,5) | 97 (64,5–154) | 38,5 (24–58) |
| 34 | 4 (3–6) | 15,5 (7–26,5) | 8 (3,5–13,5) | 100 (63,5–148) | 39 (24,5–59,5) |
| 35 | 4 (3–6) | 14 (5–30) | 5,5 (1,5–15) | 98 (70–138) | 40,5 (23–59,5) |
| 36 | 4 (3–6) | 14,5 (6,5–22,5) | 4 (3–12,5) | 99,5 (60,5–136) | 42 (22,5–58) |
| 37 | 4,5 (3–6) | 16 (3,5–23,5) | 5 (0,5–15) | 101 (67,5–128) | 42,5 (22,5–64,5) |
| 38 | 4 (3–6) | 18,5 (9,5–24,5) | 6 (3–14) | 110 (63–138) | 40 (24–63) |
| 39 | 4,5 (2,5–6) | 15,5 (6,5–24) | 7 (1,5–14) | 106 (59,5–134) | 42 (25–62,5) |
| 40 | 4,5 (2,5–6) | 14 (7,5–23,5) | 7,5 (0–13) | 102 (64,5–130) | 42 (25–63) |
| 41 | 4,5 (3–6) | 13,5 (4,5–24,5) | 5 (1,5–14) | 92 (62–126) | 41 (26–64) |
| 42 | 4,5 (2,5–6) | 18 (4–29) | 7,5 (2,5–14,5) | 95 (63–132) | 41,5 (26,5–60,5) |
| 43 | 4,5 (3–6) | 17,5 (6,5–25) | 8 (3,5–15) | 104 (62–140) | 38,5 (25,5–63) |
| 44 | 4,5 (3–5,5) | 19,5 (6,5–25,5) | 7,5 (1–14) | 96,5 (64,5–136) | 39 (27–61) |
| 45 | 4,5 (3–6) | 18,5 (6,5–25,5) | 7 (2–11,5) | 101 (70,5–146) | 38,5 (27,5–60,5) |
| 46 | 5 (3–6) | 20,5 (9,5–30) | 7 (3–17,5) | 109 (52–132) | 39 (27–63) |
| 47 | 5 (3–6) | 19 (8–30) | 7,5 (2–21) | 98,5 (64–134) | 35 (26,5–62,5) |
| 48 | 5 (3–6) | 18,5 (7–28) | 7,5 (2–18) | 104 (56,5–148) | 37,5 (27,5–62,5) |
| 49 | 5 (3–6) | 20,5 (7–32) | 8 (2,5–13,5) | 108 (56–135) | 36,5 (31–61,5) |
| 50 | 5 (3–6) | 15,5 (8–28) | 6 (2–18) | 106 (68–138) | 36,5 (31–59) |
| 51 | 5 (3–6) | 19 (6,5–27) | 10 (1,5–14,5) | 111 (70,5–138) | 36 (27–57,5) |
| 52 | 5 (3–6) | 21,5 (10–33,5) | 8 (4–20) | 114 (76–142) | 38 (28–58) |
| 53 | 5 (3–6,5) | 17 (6–24,5) | 8 (1,5–14) | 111 (62–146) | 38,5 (30,5–57,5) |
| 54 | 5 (3–6,5) | 17,5 (11–26) | 8 (3,5–14) | 114 (75,5–139) | 40 (30,5–61) |
| 55 | 5 (3–6) | 18,5 (13–27,5) | 8,5 (6–15) | 116 (73,5–149) | 37 (30,5–59,5) |
| 56 | 5 (3–6) | 18,5 (7,5–33) | 7,5 (2,5–14,5) | 113 (66,5–158) | 37,5 (30–59,5) |
| 57 | 4,5 (3–6) | 19,5 (11,5–27,5) | 8,5 (2–15) | 116 (72,5–154) | 38,5 (27,5–59,5) |
| 58 | 4,5 (3–6,5) | 18,5 (8–23) | 8,5 (1,5–17,5) | 120 (77,5–156) | 36 (29–59,5) |
| 59 | 4,5 (3–6,5) | 19 (9–25,5) | 6,5 (2–13) | 118 (74,5–158) | 36,5 (29–56) |
| 60 | 5 (3–6) | 17 (9,5–25) | 6 (1–12) | 117 (86–158) | 37 (28–55) |

### Altın ve ambar (1/3)

| Yıl | Altın p90 (medeniyetler) | Bakım gideri (altın; asker, kahraman, L2–L3) | Kamu işlerine (imar) harcanan altın | Ambarla beslenen amele tayını (gıda) | Kamu işlerindeki (amele) iş gücü payı | İmar ortalaması (köy+, 0–100) |
|---|---|---|---|---|---|---|
| 1 | 1069 (866–1448) | 2943 (2130–3299) | 2130 (1557–2930) | 829 (67,4–2445) | %16 (%12–%21) | 19,1 (15,9–23,3) |
| 2 | 1052 (828–1198) | 2904 (2153–3386) | 1897 (1311–2660) | 1092 (189–2340) | %15 (%13–%20) | 18,5 (15,2–21,6) |
| 3 | 1145 (882–1377) | 2866 (2149–3375) | 2077 (1165–2531) | 864 (50–2462) | %15 (%12–%20) | 18 (14,6–20,9) |
| 4 | 1109 (923–1416) | 2925 (2228–3249) | 1914 (1383–2745) | 692 (49,6–2911) | %14 (%9,8–%19) | 18,3 (14,1–21,2) |
| 5 | 1114 (907–1375) | 2868 (2133–3300) | 1761 (1223–2712) | 915 (90,2–2349) | %14 (%11–%18) | 18,1 (14–23,2) |
| 6 | 1104 (915–1439) | 2840 (2142–3311) | 2105 (1217–2779) | 1240 (34–2162) | %14 (%11–%18) | 18,1 (13,8–22,9) |
| 7 | 1192 (992–1524) | 2791 (2051–3291) | 2025 (1248–3009) | 992 (333–2131) | %15 (%11–%19) | 17,4 (13,2–24) |
| 8 | 1079 (974–1503) | 2698 (1988–3303) | 2168 (1547–3140) | 850 (76,6–2163) | %16 (%10–%20) | 17,8 (13,5–23,3) |
| 9 | 1167 (1007–1461) | 2717 (1963–3248) | 2154 (1602–2718) | 788 (107–2097) | %15 (%11–%19) | 17,4 (13,1–21,9) |
| 10 | 1204 (958–1453) | 2748 (1920–3278) | 2045 (1416–2883) | 691 (19–2280) | %14 (%11–%19) | 17,4 (14,3–22,5) |
| 11 | 1335 (954–1927) | 2796 (1792–3152) | 2196 (1515–2943) | 796 (119–2715) | %14 (%11–%19) | 17 (13,6–22,4) |
| 12 | 1198 (916–1693) | 2622 (1816–3266) | 2455 (1360–3256) | 477 (9,4–2367) | %15 (%10–%19) | 16,1 (13,8–23,2) |
| 13 | 1224 (1022–1645) | 2554 (1812–3324) | 2360 (1226–3010) | 391 (0–2533) | %14 (%12–%17) | 15,8 (13,8–23,3) |
| 14 | 1243 (940–1646) | 2549 (1846–3276) | 2102 (1333–2908) | 327 (0–2360) | %13 (%9,1–%16) | 17,2 (14–22,2) |
| 15 | 1162 (926–1601) | 2662 (1767–3313) | 2049 (1396–3049) | 451 (0–2374) | %13 (%10–%16) | 17,8 (14,7–22,9) |
| 16 | 1195 (817–1575) | 2632 (1853–3150) | 2146 (1257–3113) | 546 (0–2801) | %14 (%8,7–%18) | 16,5 (14,5–21,8) |
| 17 | 1112 (735–1550) | 2716 (1890–3195) | 1976 (846–3234) | 846 (34,2–2744) | %13 (%8,2–%17) | 17 (13,7–22,7) |
| 18 | 1107 (870–1463) | 2708 (1984–3173) | 1657 (849–3256) | 819 (5,6–1896) | %14 (%9,8–%16) | 16,9 (12,7–25,5) |
| 19 | 1070 (720–1382) | 2661 (1995–3111) | 1726 (959–3104) | 563 (28,4–1479) | %13 (%9,9–%18) | 15,7 (11,7–27,4) |
| 20 | 1122 (743–1338) | 2573 (2098–3151) | 1756 (830–2815) | 588 (10,2–1839) | %14 (%10–%16) | 15,1 (11,1–25,6) |
| 21 | 1121 (849–1379) | 2758 (2044–3267) | 2052 (1156–2551) | 435 (9,2–1728) | %13 (%9,6–%17) | 15,6 (11,5–23,9) |
| 22 | 1198 (750–1467) | 2688 (2053–3233) | 1832 (896–2859) | 497 (5,6–2304) | %14 (%9,3–%16) | 15,7 (11,4–25,2) |
| 23 | 1207 (765–1420) | 2593 (1991–3230) | 1717 (1017–2954) | 623 (56,6–2938) | %14 (%9,6–%16) | 16,2 (11,5–24,2) |
| 24 | 1010 (721–1401) | 2511 (2000–3240) | 1686 (942–2726) | 682 (47,6–2842) | %15 (%9,4–%18) | 15,2 (11,2–22,1) |
| 25 | 1187 (783–1674) | 2666 (2022–3239) | 1835 (965–3164) | 612 (88,6–1632) | %14 (%11–%17) | 14,7 (11,4–22) |
| 26 | 1143 (887–1748) | 2720 (2099–3205) | 2241 (1139–2931) | 647 (2,2–2017) | %13 (%9,9–%18) | 16 (11–22,6) |
| 27 | 1176 (881–1613) | 2654 (2035–3214) | 2104 (1187–3006) | 465 (0–1773) | %13 (%9,2–%18) | 17,1 (11–22,9) |
| 28 | 1180 (938–1516) | 2603 (2039–3254) | 2034 (1266–2967) | 294 (0,6–1319) | %13 (%7,7–%17) | 17,9 (10–23,3) |
| 29 | 1201 (835–1498) | 2556 (1866–3231) | 2052 (1174–3011) | 308 (33,2–2000) | %13 (%8,6–%17) | 17,3 (11,1–21,9) |
| 30 | 1193 (667–1555) | 2584 (1864–3270) | 2127 (1162–3345) | 515 (0,6–1547) | %13 (%8,6–%17) | 15,3 (12,6–21,7) |
| 31 | 1073 (884–1444) | 2694 (1862–3197) | 2110 (1231–3268) | 529 (0–1315) | %13 (%8,5–%17) | 15 (12,4–22,3) |
| 32 | 1252 (790–1520) | 2531 (1950–3241) | 1868 (1075–3367) | 272 (0–1548) | %13 (%8,7–%15) | 15,5 (12,4–22) |
| 33 | 1315 (872–1578) | 2657 (1994–3262) | 2223 (1127–3115) | 332 (2,8–1356) | %13 (%8,4–%16) | 14,5 (12,8–22,7) |
| 34 | 1322 (962–1532) | 2594 (1958–3183) | 2352 (1415–3481) | 203 (0–1667) | %13 (%8,5–%16) | 14,9 (12,2–22,4) |
| 35 | 1383 (1018–1691) | 2579 (1915–3085) | 2373 (1709–3665) | 152 (0–1482) | %12 (%9,4–%16) | 15 (13–22,1) |
| 36 | 1217 (985–1613) | 2679 (2008–3171) | 2338 (1539–3270) | 338 (0–1188) | %12 (%8–%15) | 16,8 (12,5–23) |
| 37 | 1275 (970–1834) | 2485 (2046–3159) | 2338 (1555–3238) | 337 (1,8–1567) | %11 (%9,1–%15) | 16,4 (12–26,8) |
| 38 | 1329 (833–2595) | 2436 (1960–3113) | 2495 (1270–5577) | 503 (12,2–1744) | %13 (%9,4–%16) | 17,9 (11,7–29,4) |
| 39 | 1435 (1055–1834) | 2455 (1879–3080) | 2615 (1678–4226) | 536 (0–1982) | %14 (%11–%16) | 17,2 (11,8–27,5) |
| 40 | 1407 (1108–1630) | 2431 (1896–3110) | 2719 (1629–3663) | 194 (0–1493) | %14 (%11–%17) | 16,8 (12,1–26,4) |
| 41 | 1332 (1164–1583) | 2386 (1963–3111) | 2560 (1729–3240) | 379 (0–1188) | %14 (%11–%17) | 17,7 (11,9–25,1) |
| 42 | 1368 (886–1923) | 2426 (2044–2948) | 2519 (1622–3350) | 349 (1,2–1118) | %13 (%11–%15) | 17,6 (12,8–27,1) |
| 43 | 1415 (919–1691) | 2433 (2081–2996) | 2415 (1475–3495) | 397 (0–1110) | %13 (%9,4–%15) | 17 (13–27,5) |
| 44 | 1334 (1119–1923) | 2425 (1998–3048) | 2419 (1680–3604) | 284 (0–1213) | %13 (%11–%17) | 16,4 (12,2–27) |
| 45 | 1405 (1144–1718) | 2306 (2129–3084) | 2367 (1863–3826) | 434 (1–1559) | %13 (%8,1–%16) | 16,1 (11,8–29,4) |
| 46 | 1391 (1044–1742) | 2292 (2007–3055) | 2535 (1610–3783) | 244 (0–1026) | %12 (%6,7–%15) | 16,2 (12,2–29,2) |
| 47 | 1368 (1074–1607) | 2343 (1978–3080) | 2520 (1681–3337) | 273 (0–1670) | %12 (%8,2–%16) | 16,5 (12,5–28,1) |
| 48 | 1455 (1163–1599) | 2322 (1958–3071) | 2696 (1706–3319) | 170 (0–1061) | %14 (%6,8–%18) | 16 (12,3–26,9) |
| 49 | 1444 (1153–1711) | 2258 (1974–3005) | 2551 (1868–3390) | 266 (0–1392) | %14 (%10–%17) | 16,3 (12–22,6) |
| 50 | 1496 (1068–1870) | 2188 (1841–3033) | 2700 (1771–3411) | 199 (0–1618) | %13 (%7,8–%16) | 16,8 (12,6–22,2) |
| 51 | 1512 (1006–1929) | 2346 (1890–2992) | 3087 (1740–3837) | 251 (0–857) | %13 (%8,1–%16) | 16,2 (12,1–21,9) |
| 52 | 1444 (1118–1681) | 2191 (1801–3112) | 2922 (1812–3566) | 204 (0–1072) | %13 (%10–%19) | 15,9 (12,7–21,9) |
| 53 | 1472 (1152–1840) | 2397 (1775–3165) | 2810 (1881–3842) | 195 (0–933) | %13 (%10–%17) | 16,5 (12,3–21,1) |
| 54 | 1533 (1190–1904) | 2285 (1780–3182) | 2930 (1869–3748) | 249 (0–1231) | %13 (%11–%18) | 16,6 (12,1–22) |
| 55 | 1478 (1304–1875) | 2357 (1804–3178) | 2894 (2121–3720) | 176 (0–1002) | %13 (%10–%19) | 16,5 (11,9–22,5) |
| 56 | 1440 (1212–2075) | 2425 (1836–3109) | 2634 (2179–3906) | 133 (0–789) | %14 (%9,6–%19) | 17,6 (11,8–23) |
| 57 | 1406 (1171–1881) | 2413 (1881–3135) | 2667 (2060–4342) | 238 (0–765) | %13 (%9,2–%19) | 17,2 (10,4–22,3) |
| 58 | 1382 (1042–2146) | 2447 (1744–3048) | 2864 (2046–4336) | 42,6 (0–533) | %15 (%9,1–%20) | 18,1 (10–21,9) |
| 59 | 1573 (1235–1834) | 2460 (1719–3101) | 2967 (1901–4135) | 72,6 (0–561) | %15 (%9,1–%18) | 17,4 (9,1–23,7) |
| 60 | 1362 (1248–1617) | 2485 (1778–3088) | 2974 (2166–3967) | 12 (0–626) | %14 (%8,1–%17) | 18,4 (9,49–24,1) |

### Altın ve ambar (2/3)

| Yıl | Canavar baskınında yitirilen altın | Ejderhaya giden altın (haraç + akın) | Hazinesi boş medeniyet payı | Kent tüketiminde yokluk payı (köy+; ekmek, bira ya da alet) | Ekmek ya da bira yokluğu payı (köy+) | Kıtlık (büyük olay) |
|---|---|---|---|---|---|---|
| 1 | 5 (0–32) | 0 | %0 | %31 (%22–%39) | %5,5 (%1,5–%11) | 0 |
| 2 | 7 (0–45) | 0 | %0 | %29 (%19–%40) | %5,3 (%0,9–%10) | 0 |
| 3 | 0,5 (0–21) | 0 | %0 | %32 (%18–%40) | %6,1 (%1,6–%12) | 0 |
| 4 | 0 (0–17,5) | 0 | %0 | %32 (%19–%39) | %7,3 (%4,2–%16) | 0 |
| 5 | 14 (0–32) | 0 | %0 | %30 (%24–%41) | %7,6 (%2–%18) | 0 |
| 6 | 1 (0–26,5) | 0 | %0 | %34 (%25–%38) | %7,2 (%2–%16) | 0 |
| 7 | 0 (0–29,5) | 0 | %0 | %30 (%23–%38) | %5,4 (%2,6–%12) | 0 |
| 8 | 7,5 (0–50) | 0 | %0 | %31 (%26–%41) | %7,6 (%1,8–%16) | 0 (0–0,5) |
| 9 | 0 (0–40,5) | 0 | %0 | %31 (%24–%39) | %7,3 (%2,8–%19) | 0 |
| 10 | 1 (0–44) | 0 | %0 | %33 (%26–%40) | %6,1 (%2,3–%18) | 0 |
| 11 | 0 (0–26) | 0 | %0 | %32 (%27–%41) | %6,6 (%2,9–%16) | 0 |
| 12 | 5,5 (0–41,5) | 0 | %0 | %32 (%26–%43) | %8,2 (%2,7–%16) | 0 |
| 13 | 7 (0–39,5) | 0 | %0 | %31 (%23–%43) | %6,2 (%2,9–%18) | 0 |
| 14 | 8,5 (0–30) | 0 (0–94,5) | %0 | %34 (%25–%44) | %6,5 (%2,5–%26) | 0 |
| 15 | 0 (0–46,5) | 9,5 (0–206) | %0 | %37 (%23–%43) | %6,3 (%2,8–%18) | 0 |
| 16 | 0 (0–29) | 70 (0–290) | %0 | %33 (%23–%42) | %7 (%1,6–%18) | 0 |
| 17 | 0,5 (0–26,5) | 136 (0–468) | %0 | %32 (%21–%40) | %7,4 (%1,3–%18) | 0 |
| 18 | 7 (0–36) | 73 (0–367) | %0 | %34 (%26–%43) | %7,2 (%2,1–%19) | 0 |
| 19 | 2,5 (0–31) | 82 (0–598) | %0 | %33 (%24–%41) | %9,2 (%1,4–%20) | 0 |
| 20 | 0 (0–21,5) | 54,5 (0–724) | %0 | %33 (%24–%41) | %8,1 (%2,3–%19) | 0 |
| 21 | 0 (0–31) | 47 (0–715) | %0 | %33 (%23–%43) | %6,2 (%1,4–%23) | 0 |
| 22 | 13 (0–25) | 9,5 (0–535) | %0 | %32 (%23–%44) | %6,5 (%1,9–%24) | 0 |
| 23 | 5 (0–23) | 41 (0–928) | %0 | %35 (%26–%43) | %6,8 (%1,8–%21) | 0 |
| 24 | 6,5 (0–26,5) | 161 (0–698) | %0 | %36 (%27–%46) | %6,1 (%1,6–%18) | 0 |
| 25 | 5 (0–33,5) | 80 (0–834) | %0 | %34 (%26–%43) | %5,1 (%1,5–%22) | 0 |
| 26 | 1,5 (0–32) | 37 (0–734) | %0 | %33 (%23–%45) | %4,3 (%1,4–%20) | 0 |
| 27 | 4 (0–28) | 8 (0–614) | %0 | %39 (%23–%43) | %4,8 (%1,4–%25) | 0 (0–0,5) |
| 28 | 0 (0–10) | 0 (0–775) | %0 | %35 (%24–%45) | %6,4 (%1,9–%21) | 0 |
| 29 | 0 (0–15,5) | 0 (0–728) | %0 | %33 (%24–%45) | %5,8 (%2,2–%19) | 0 |
| 30 | 3 (0–23,5) | 0 (0–689) | %0 | %32 (%25–%45) | %7,1 (%1,4–%18) | 0 (0–0,5) |
| 31 | 1 (0–18,5) | 0 (0–704) | %0 | %34 (%21–%45) | %6,3 (%1,4–%20) | 0 |
| 32 | 1 (0–43,5) | 0 (0–590) | %0 | %32 (%19–%50) | %7,3 (%1,6–%30) | 0 |
| 33 | 3 (0–22,5) | 0 (0–529) | %0 | %32 (%20–%47) | %7,7 (%1,7–%18) | 0 |
| 34 | 7,5 (0–32) | 0 (0–1102) | %0 | %31 (%21–%49) | %8,1 (%2–%18) | 0 (0–0,5) |
| 35 | 10,5 (0–39) | 0 (0–778) | %0 | %33 (%21–%46) | %11 (%1,9–%20) | 0 |
| 36 | 0,5 (0–25) | 0 (0–592) | %0 | %35 (%20–%43) | %10 (%1,6–%17) | 0 |
| 37 | 0,5 (0–33,5) | 0 (0–594) | %0 | %32 (%20–%46) | %7,7 (%1,5–%16) | 0 |
| 38 | 2 (0–27,5) | 0 (0–372) | %0 | %29 (%20–%50) | %6,1 (%1,5–%14) | 0 |
| 39 | 5 (0–29,5) | 0 (0–230) | %0 | %31 (%19–%46) | %6,9 (%1,3–%17) | 0 (0–1) |
| 40 | 1,5 (0–23,5) | 0 (0–272) | %0 | %28 (%23–%42) | %6,3 (%1,5–%16) | 0 |
| 41 | 2,5 (0–29) | 0 (0–488) | %0 | %28 (%20–%41) | %4,7 (%2,1–%24) | 0 |
| 42 | 2 (0–22,5) | 0 (0–438) | %0 | %34 (%16–%41) | %4,8 (%1,9–%25) | 0 |
| 43 | 4,5 (0–24) | 0 (0–259) | %0 | %33 (%19–%43) | %8,9 (%1,6–%22) | 0 |
| 44 | 8,5 (0–39,5) | 0 (0–364) | %0 | %35 (%18–%45) | %8,6 (%1,5–%29) | 0 (0–0,5) |
| 45 | 5 (0–15,5) | 0 (0–314) | %0 | %33 (%19–%46) | %11 (%1,6–%33) | 0 |
| 46 | 0 (0–28) | 0 (0–419) | %0 | %37 (%21–%53) | %13 (%1,5–%45) | 0 |
| 47 | 0 (0–29) | 0 (0–458) | %0 | %33 (%21–%52) | %9,6 (%1,5–%36) | 0 |
| 48 | 2,5 (0–23,5) | 0 (0–277) | %0 | %34 (%20–%56) | %9,7 (%1–%34) | 0 (0–0,5) |
| 49 | 6 (0–21) | 0 (0–233) | %0 | %33 (%18–%57) | %9 (%0,8–%37) | 0 |
| 50 | 1 (0–27,5) | 0 (0–318) | %0 | %39 (%19–%51) | %11 (%1,4–%35) | 0 |
| 51 | 5,5 (0–39) | 0 (0–368) | %0 | %37 (%19–%53) | %12 (%2,3–%34) | 0 |
| 52 | 6,5 (0–48,5) | 0 (0–296) | %0 | %32 (%20–%47) | %11 (%1,9–%27) | 0 |
| 53 | 0,5 (0–18,5) | 0 (0–422) | %0 | %33 (%20–%43) | %8,9 (%1,8–%23) | 0 |
| 54 | 2,5 (0–24,5) | 0 (0–451) | %0 | %33 (%23–%39) | %10 (%2,4–%18) | 0 |
| 55 | 0 (0–27) | 0 (0–426) | %0 | %32 (%20–%41) | %8,8 (%2,9–%18) | 0 (0–1) |
| 56 | 6,5 (0–28,5) | 0 (0–372) | %0 | %35 (%19–%44) | %8,5 (%2,3–%22) | 0 |
| 57 | 5,5 (0–30,5) | 0 (0–346) | %0 | %33 (%19–%45) | %10 (%2,1–%23) | 0 |
| 58 | 1,5 (0–27,5) | 0 (0–384) | %0 | %32 (%18–%45) | %11 (%2,4–%23) | 0 (0–0,5) |
| 59 | 0,5 (0–35) | 0 (0–482) | %0 | %37 (%17–%46) | %9,5 (%1,4–%31) | 0 |
| 60 | 4 (0–25) | 0 (0–608) | %0 | %34 (%11–%52) | %7,2 (%1,2–%27) | 0 |

### Altın ve ambar (3/3)

| Yıl | Açlıktan ölen | Kıtlık yardımı (sevkiyat) | Kıtlıkta yüz çeviren | Kıtlık akını | Ambarın yettiği gün (medeniyet medyanı) |
|---|---|---|---|---|---|
| 1 | 0 | 0 | 0 | 0 | 43,3 (28,2–60,3) |
| 2 | 0 | 0 | 0 | 0 | 41,5 (22,2–58,3) |
| 3 | 0 | 0 | 0 | 0 | 41,5 (13,7–58,5) |
| 4 | 0 (0–3) | 0 | 0 (0–0,5) | 0 | 42,3 (21,4–52,7) |
| 5 | 0 (0–1) | 0 (0–2) | 0 | 0 | 39,2 (19,5–57,3) |
| 6 | 0 (0–25) | 0 | 0 | 0 | 40,8 (25,2–53,1) |
| 7 | 0 (0–20) | 0 | 0 | 0 | 38 (24,7–51,8) |
| 8 | 0 (0–8) | 0 (0–0,5) | 0 (0–1,5) | 0 | 36,4 (19,6–50,8) |
| 9 | 0 (0–5,5) | 0 | 0 | 0 | 34,7 (13,4–49,4) |
| 10 | 0 (0–6,5) | 0 | 0 | 0 | 34,9 (11,1–57,4) |
| 11 | 0 (0–3,5) | 0 | 0 | 0 | 35,2 (17,3–48,2) |
| 12 | 0 (0–7) | 0 | 0 | 0 | 36,7 (13,1–49,5) |
| 13 | 0 (0–4) | 0 | 0 | 0 | 37,8 (11,5–46) |
| 14 | 0 (0–3) | 0 | 0 | 0 | 36,4 (9,57–48,9) |
| 15 | 0 (0–2,5) | 0 | 0 | 0 | 36,1 (12,6–50,7) |
| 16 | 0 (0–1) | 0 | 0 | 0 | 32,8 (15,1–46) |
| 17 | 0 (0–0,5) | 0 | 0 | 0 | 32,5 (16,6–49,9) |
| 18 | 0 | 0 | 0 | 0 | 32,3 (17,9–50,8) |
| 19 | 0 | 0 | 0 | 0 | 35,4 (16,7–49,3) |
| 20 | 0 (0–2) | 0 (0–0,5) | 0 (0–0,5) | 0 | 35,3 (18–48,6) |
| 21 | 0 (0–2,5) | 0 | 0 | 0 | 34,8 (18,3–49,1) |
| 22 | 0 (0–6,5) | 0 | 0 | 0 | 36,5 (16,6–49,9) |
| 23 | 0 (0–2) | 0 | 0 | 0 | 34,6 (15,7–58,6) |
| 24 | 0 (0–6) | 0 | 0 | 0 | 35,8 (5,63–54,4) |
| 25 | 0 (0–2,5) | 0 | 0 | 0 | 36,1 (5,37–51,4) |
| 26 | 0 | 0 | 0 | 0 | 35,8 (7,35–52) |
| 27 | 0 | 0 (0–0,5) | 0 (0–0,5) | 0 | 36,5 (5,51–55,7) |
| 28 | 0 | 0 | 0 | 0 | 37,3 (16,2–54,4) |
| 29 | 0 (0–13) | 0 | 0 | 0 | 33,6 (13,1–58,7) |
| 30 | 0 | 0 | 0 | 0 | 37,6 (12,1–55) |
| 31 | 0 (0–7) | 0 | 0 | 0 | 32,8 (12,6–50,4) |
| 32 | 0 (0–0,5) | 0 | 0 | 0 | 32 (9,87–46,4) |
| 33 | 0 (0–4,5) | 0 | 0 | 0 | 33,4 (11,9–49,7) |
| 34 | 0 (0–4) | 0 (0–1) | 0 (0–0,5) | 0 | 33,6 (12,1–48,9) |
| 35 | 0 (0–4,5) | 0 | 0 | 0 | 34,1 (11,8–39,7) |
| 36 | 0 | 0 | 0 | 0 | 32,7 (14,4–50,3) |
| 37 | 0 | 0 | 0 | 0 | 34,8 (15,6–47,9) |
| 38 | 0 (0–2,5) | 0 | 0 | 0 | 33,9 (15,4–48,1) |
| 39 | 0 (0–2,5) | 0 (0–1) | 0 (0–2) | 0 | 38,2 (13,8–51,8) |
| 40 | 0 (0–2,5) | 0 (0–2) | 0 (0–1) | 0 | 36 (10,8–49,7) |
| 41 | 0 (0–5) | 0 | 0 | 0 | 31,1 (8,6–46,3) |
| 42 | 0 (0–2) | 0 | 0 | 0 | 31,6 (8,74–47,8) |
| 43 | 0 (0–33) | 0 | 0 | 0 | 29 (12,7–46,9) |
| 44 | 0 (0–21) | 0 (0–1) | 0 (0–0,5) | 0 | 30,5 (10,5–48) |
| 45 | 0 (0–7) | 0 | 0 (0–0,5) | 0 | 32,1 (10,1–44,4) |
| 46 | 0 (0–6,5) | 0 | 0 | 0 | 34,7 (8,2–45) |
| 47 | 0 (0–5,5) | 0 | 0 | 0 | 32,8 (10,9–47) |
| 48 | 0 (0–2,5) | 0 (0–1,5) | 0 (0–0,5) | 0 | 35,5 (10,8–44,1) |
| 49 | 0 (0–1) | 0 (0–0,5) | 0 | 0 | 30,5 (10,2–42,1) |
| 50 | 0 (0–9) | 0 | 0 | 0 | 29,4 (10,4–45,8) |
| 51 | 0 (0–13) | 0 (0–1) | 0 (0–0,5) | 0 | 29,7 (8,06–43,7) |
| 52 | 0 (0–14) | 0 | 0 (0–0,5) | 0 | 30,1 (7,59–44,2) |
| 53 | 0 (0–10) | 0 | 0 | 0 | 25,9 (10,5–42,9) |
| 54 | 0 (0–7) | 0 | 0 | 0 | 27,9 (10,6–43) |
| 55 | 0 (0–7,5) | 0 (0–1,5) | 0 (0–1) | 0 | 24,9 (11,8–41,5) |
| 56 | 0 (0–4,5) | 0 | 0 | 0 | 28,2 (11,2–42) |
| 57 | 0 (0–6) | 0 | 0 | 0 | 24,7 (11,7–46,8) |
| 58 | 0 (0–4) | 0 (0–1) | 0 | 0 | 22 (11,6–40,6) |
| 59 | 0 (0–5,5) | 0 | 0 | 0 | 19,1 (10,2–42,9) |
| 60 | 0 (0–4) | 0 (0–0,5) | 0 | 0 | 19,4 (11,1–42,2) |

### Yerleşim kademesi

| Yıl | Ortalama yerleşim kademesi | Köy+ yerleşim | Kasaba+ yerleşim | Şehir | Ortalama başkent kademesi | Kademe değişimi (yerleşim, yıl içinde) |
|---|---|---|---|---|---|---|
| 1 | 1,07 (0,91–1,21) | 60 (43,5–69,5) | 21 (12–24) | 6 (4,5–8) | 2,73 (2,54–2,92) | 27 (20–39,5) |
| 2 | 1,08 (0,92–1,2) | 59,5 (46–68) | 20,5 (11–23,5) | 6 (3,5–7) | 2,73 (2,5–3) | 28 (18,5–39) |
| 3 | 1,08 (0,92–1,22) | 62,5 (42–69) | 19 (13–25,5) | 5,5 (4–7) | 2,82 (2,5–3) | 30 (20–38) |
| 4 | 1,1 (0,93–1,23) | 63,5 (44–69,5) | 20 (13,5–24,5) | 6 (4–8) | 2,83 (2,58–3) | 25,5 (20,5–33,5) |
| 5 | 1,07 (0,88–1,23) | 61,5 (41–72,5) | 19,5 (12–25,5) | 5,5 (3,5–7) | 2,83 (2,5–3) | 25,5 (18,5–34,5) |
| 6 | 1,06 (0,89–1,24) | 61 (37,5–69) | 20 (11,5–26) | 6 (4–8,5) | 2,82 (2,45–3) | 30 (20–36) |
| 7 | 1,06 (0,88–1,23) | 62 (40–69) | 18 (10–24,5) | 6,5 (3,5–8,5) | 2,83 (2,45–3) | 23,5 (17–35,5) |
| 8 | 1,04 (0,86–1,24) | 59,5 (40–71) | 17,5 (11–23) | 6 (4–9) | 2,82 (2,32–3) | 24,5 (18–33,5) |
| 9 | 1,05 (0,81–1,24) | 59 (36,5–71) | 17,5 (12–24) | 6 (4,5–8) | 2,82 (2,46–3) | 27,5 (16–46,5) |
| 10 | 1,01 (0,76–1,19) | 55 (34,5–72) | 18,5 (10,5–21) | 6 (3–8,5) | 2,69 (2,15–3) | 23,5 (12,5–38,5) |
| 11 | 1,07 (0,8–1,18) | 57 (36,5–69) | 19,5 (11,5–23,5) | 6 (4–8,5) | 2,76 (2,33–3) | 25,5 (14–37) |
| 12 | 1,09 (0,78–1,22) | 58,5 (39–70) | 18 (11,5–21,5) | 6 (5–8,5) | 2,76 (2,29–3) | 26 (18–39) |
| 13 | 1,08 (0,81–1,24) | 58 (41,5–68) | 19 (11,5–25,5) | 6 (5–8,5) | 2,71 (2,38–3) | 24 (16–38) |
| 14 | 1,09 (0,83–1,28) | 59,5 (41,5–68) | 19 (11–26,5) | 6 (4,5–9) | 2,71 (2,33–3) | 24 (17,5–33) |
| 15 | 1,05 (0,79–1,26) | 59 (37,5–68) | 19 (9,5–27) | 6 (4,5–8) | 2,63 (2,46–2,92) | 21,5 (14–30,5) |
| 16 | 1,09 (0,77–1,22) | 58 (39–67,5) | 18 (11,5–24) | 7 (4–9) | 2,69 (2,36–3) | 24,5 (16–36) |
| 17 | 1,09 (0,8–1,25) | 58,5 (39,5–67) | 18 (12–25,5) | 7 (5–8,5) | 2,83 (2,46–3) | 26 (18,5–32) |
| 18 | 1,04 (0,8–1,26) | 57,5 (38,5–69) | 18,5 (13,5–23,5) | 6,5 (4,5–9) | 2,83 (2,33–3) | 21 (15–29) |
| 19 | 1,04 (0,81–1,24) | 56,5 (36,5–68,5) | 18,5 (11,5–24,5) | 7 (4–8,5) | 2,83 (2,43–2,85) | 22,5 (16–39) |
| 20 | 1,01 (0,84–1,3) | 58,5 (38–68) | 18 (12,5–26,5) | 6,5 (4–9) | 2,69 (2,38–2,93) | 20,5 (12,5–32,5) |
| 21 | 1,04 (0,86–1,29) | 60 (38,5–69,5) | 19 (12–25,5) | 6,5 (4,5–8) | 2,69 (2,43–2,92) | 22,5 (14–33) |
| 22 | 1,03 (0,87–1,29) | 58 (41,5–69,5) | 20,5 (12–27,5) | 7 (4–8) | 2,71 (2,5–2,93) | 23 (17–31,5) |
| 23 | 1,04 (0,81–1,32) | 58,5 (42–68) | 18,5 (11–30) | 6 (4,5–8,5) | 2,71 (2,45–2,83) | 25 (19–32) |
| 24 | 1,05 (0,83–1,33) | 59 (42,5–71) | 19,5 (11,5–28) | 6 (4,5–8,5) | 2,69 (2,59–2,85) | 24 (19–31,5) |
| 25 | 1,01 (0,82–1,36) | 59,5 (42,5–72,5) | 17,5 (10,5–29,5) | 6 (4–9) | 2,71 (2,41–2,83) | 23,5 (19–36) |
| 26 | 1,02 (0,81–1,37) | 59,5 (42,5–72,5) | 17,5 (10–30,5) | 6,5 (4–9) | 2,71 (2,5–2,93) | 23 (16,5–34) |
| 27 | 0,96 (0,82–1,33) | 56,5 (41–70,5) | 17,5 (10–30) | 6 (4–8,5) | 2,71 (2,54–3) | 24,5 (18–27,5) |
| 28 | 1,02 (0,84–1,31) | 56,5 (42–72) | 19 (11–29) | 5 (5–7,5) | 2,67 (2,36–2,92) | 23 (16,5–34) |
| 29 | 1 (0,76–1,33) | 58 (39–72,5) | 18 (10,5–29,5) | 5,5 (5–6) | 2,65 (2,34–2,85) | 24 (14–34) |
| 30 | 1,02 (0,78–1,33) | 57,5 (39–72) | 18,5 (10–29) | 6 (4,5–7) | 2,69 (2,42–2,86) | 23 (16–35) |
| 31 | 1,08 (0,77–1,29) | 55,5 (43,5–72) | 18 (10–27) | 5 (5–6) | 2,69 (2,42–2,85) | 24,5 (16,5–30,5) |
| 32 | 1 (0,78–1,29) | 58,5 (42,5–69,5) | 17,5 (8–29,5) | 6 (5–7) | 2,69 (2,43–2,86) | 24,5 (15,5–30) |
| 33 | 1 (0,83–1,28) | 58,5 (43,5–68) | 16,5 (10,5–30) | 6 (5–7) | 2,77 (2,4–2,93) | 23 (12,5–32,5) |
| 34 | 1,02 (0,82–1,3) | 59 (46–69) | 17 (11–30) | 5,5 (3,5–7) | 2,62 (2,33–2,93) | 22,5 (12,5–35) |
| 35 | 1,02 (0,78–1,31) | 59 (43–68,5) | 16,5 (9,5–27,5) | 5,5 (3,5–7) | 2,57 (2,46–2,85) | 22,5 (14–30,5) |
| 36 | 1,01 (0,86–1,38) | 59,5 (46,5–69,5) | 18 (8,5–32,5) | 5 (3,5–7) | 2,57 (2,25–2,83) | 22,5 (15–31) |
| 37 | 1,02 (0,88–1,39) | 57,5 (47–69) | 15,5 (11–33,5) | 6 (4–7) | 2,67 (2,5–2,83) | 23,5 (20–29) |
| 38 | 1,01 (0,82–1,39) | 58 (44,5–69,5) | 15,5 (9–32) | 6,5 (5–7,5) | 2,73 (2,36–3) | 25 (16–32) |
| 39 | 1,02 (0,85–1,29) | 58 (45–69) | 17 (11–28) | 5,5 (4–6,5) | 2,69 (2,3–3) | 19,5 (13–33,5) |
| 40 | 0,99 (0,87–1,32) | 58,5 (47,5–69) | 16,5 (9–28,5) | 5,5 (4–8) | 2,71 (2,47–2,92) | 25 (13–31,5) |
| 41 | 1,03 (0,87–1,31) | 59,5 (46–69,5) | 15,5 (11–26) | 5,5 (4,5–8) | 2,76 (2,3–2,85) | 25 (14,5–35) |
| 42 | 1,03 (0,84–1,27) | 59,5 (43,5–68,5) | 15 (10–24) | 6 (4–8,5) | 2,83 (2,38–3) | 21,5 (15–31,5) |
| 43 | 1,03 (0,82–1,3) | 60 (44–66) | 16,5 (8,5–26,5) | 6 (4–9) | 2,83 (2,38–3) | 21,5 (17,5–27,5) |
| 44 | 0,99 (0,78–1,29) | 57,5 (44–68) | 15 (7,5–25,5) | 6 (4–7,5) | 2,77 (2,24–2,93) | 19 (13–30,5) |
| 45 | 1,01 (0,81–1,33) | 59 (44–70) | 17,5 (10–28,5) | 6 (4–7,5) | 2,83 (2,38–3) | 19 (11,5–32) |
| 46 | 1,06 (0,81–1,29) | 61 (44,5–68) | 16,5 (10–23,5) | 6 (3,5–7,5) | 2,83 (2,36–2,93) | 19,5 (10–26,5) |
| 47 | 1,04 (0,79–1,34) | 60,5 (44–70) | 17 (9,5–27,5) | 6 (3,5–7,5) | 2,83 (2,43–2,93) | 19,5 (11–28) |
| 48 | 1,06 (0,79–1,29) | 62 (43–69,5) | 16 (10–27,5) | 5,5 (4–7) | 2,62 (2,36–3) | 19 (13,5–24,5) |
| 49 | 1,02 (0,81–1,27) | 60,5 (44,5–69,5) | 15 (9–26,5) | 6 (3,5–7,5) | 2,62 (2,38–2,86) | 18,5 (11,5–27,5) |
| 50 | 1,02 (0,81–1,28) | 61 (43–67) | 17,5 (10,5–30) | 6 (4–8) | 2,73 (2,46–2,93) | 20,5 (13,5–29,5) |
| 51 | 1,07 (0,83–1,29) | 63 (44,5–68,5) | 16 (12,5–25,5) | 6 (4–8) | 2,71 (2,36–3) | 16 (10–27) |
| 52 | 1,08 (0,82–1,28) | 58,5 (43,5–70) | 15 (11–26,5) | 6 (4,5–8) | 2,71 (2,47–2,87) | 21,5 (13–30) |
| 53 | 1,05 (0,82–1,28) | 59 (42–68) | 16 (12,5–28) | 6 (4,5–7) | 2,71 (2,5–3) | 18,5 (10–28) |
| 54 | 1,04 (0,85–1,28) | 59 (43,5–67,5) | 17 (13–26,5) | 6 (4,5–8,5) | 2,77 (2,54–3) | 18 (14–28) |
| 55 | 1 (0,78–1,28) | 57,5 (43–71) | 16 (11–27) | 5 (4,5–8,5) | 2,73 (2,5–2,86) | 20,5 (11,5–26) |
| 56 | 1,04 (0,8–1,26) | 57 (43,5–70,5) | 17,5 (10,5–28,5) | 6 (4,5–8) | 2,71 (2,46–2,86) | 18,5 (10–31) |
| 57 | 1,09 (0,8–1,27) | 59,5 (45–70) | 16,5 (8,5–30) | 6 (5–8,5) | 2,71 (2,5–2,92) | 20,5 (12,5–29) |
| 58 | 1,06 (0,8–1,3) | 61,5 (42–70,5) | 19 (9–28,5) | 6 (4,5–8,5) | 2,69 (2,5–2,87) | 20,5 (9,5–23) |
| 59 | 1,08 (0,79–1,26) | 60 (43–70) | 17 (8–27) | 6 (4,5–9) | 2,73 (2,5–2,94) | 18 (11–27) |
| 60 | 1,06 (0,8–1,26) | 59,5 (45–70) | 16 (7–27,5) | 6,5 (4,5–8,5) | 2,63 (2,5–2,94) | 21,5 (9,5–28) |

### Deniz

| Yıl | Liman (tersane) | Gemi (koga/tekne) | Kadırga | Denizaşırı yerleşim | Deniz ticaret yolu (yıl sonu) | Deniz seferi (ticaret) |
|---|---|---|---|---|---|---|
| 1 | 21,5 (14,5–24,5) | 44 (31–51,5) | 21 (16–27) | 7,5 (5,5–10,5) | 16 (8,5–18) | 25 (12–30) |
| 2 | 21 (14,5–25) | 45 (31,5–52) | 21 (16–27,5) | 7,5 (5,5–10,5) | 16 (8,5–18,5) | 23 (11,5–36,5) |
| 3 | 21 (14,5–25) | 45 (31,5–52,5) | 21 (16–27,5) | 7,5 (5,5–10,5) | 16 (8,5–19) | 25,5 (12–35,5) |
| 4 | 21,5 (14,5–25) | 43,5 (30,5–52,5) | 21 (16–27,5) | 7,5 (5,5–10,5) | 16,5 (8,5–20) | 27 (10–35) |
| 5 | 22 (15,5–26) | 43,5 (30–52) | 21 (16–27,5) | 7,5 (5,5–10,5) | 16,5 (8,5–20) | 24,5 (12–34) |
| 6 | 22 (14–26,5) | 42 (30,5–52) | 21 (16–27,5) | 7,5 (5,5–10,5) | 16 (8,5–20) | 23 (11,5–35) |
| 7 | 22 (14,5–25,5) | 42,5 (30,5–52) | 21 (17,5–27,5) | 7,5 (5,5–10) | 17 (8,5–20) | 25,5 (11,5–39,5) |
| 8 | 21 (14,5–26) | 42,5 (27,5–52,5) | 20 (17,5–26,5) | 7,5 (5,5–10) | 14,5 (8,5–20,5) | 24,5 (10–40) |
| 9 | 21,5 (14,5–25,5) | 42,5 (29,5–53,5) | 20 (16,5–26,5) | 7,5 (5,5–9,5) | 14,5 (8,5–21) | 23,5 (10–39,5) |
| 10 | 21,5 (14,5–25,5) | 42,5 (31,5–53,5) | 20 (16,5–26,5) | 7,5 (5,5–9,5) | 14,5 (8,5–21) | 23,5 (11,5–39,5) |
| 11 | 21 (14,5–25,5) | 43 (32–54) | 20 (17,5–26,5) | 7,5 (5,5–9,5) | 15 (8,5–21) | 26 (11–38,5) |
| 12 | 21,5 (15–25) | 45,5 (33,5–54) | 20 (17,5–26,5) | 7,5 (5,5–9,5) | 14,5 (8,5–21) | 25,5 (12–37) |
| 13 | 22 (15,5–25,5) | 44,5 (32–56,5) | 20 (16–25,5) | 7,5 (5,5–9,5) | 16 (8,5–21) | 27 (12,5–38,5) |
| 14 | 22 (15,5–25,5) | 45 (34,5–57) | 19,5 (16–25,5) | 7,5 (5,5–9,5) | 16 (8,5–21) | 23,5 (13,5–39) |
| 15 | 21,5 (15,5–26,5) | 44,5 (36–59) | 19,5 (16–25,5) | 7,5 (5,5–9,5) | 16 (9–21,5) | 23 (14,5–32) |
| 16 | 22 (15,5–27) | 47 (36,5–59) | 20 (17–25) | 7,5 (5,5–9,5) | 16,5 (9–22,5) | 26 (13–39) |
| 17 | 21,5 (15,5–26,5) | 47 (36,5–59) | 20 (16,5–25) | 7,5 (5,5–9,5) | 17 (9–22,5) | 28,5 (14,5–43,5) |
| 18 | 22,5 (16–26,5) | 47,5 (37,5–58,5) | 20,5 (16,5–24,5) | 7,5 (5,5–9,5) | 17,5 (9–22,5) | 30 (16,5–41,5) |
| 19 | 22 (16–28) | 46,5 (36,5–58,5) | 20,5 (16,5–24) | 7,5 (5,5–9,5) | 17,5 (9–22,5) | 28,5 (12,5–46,5) |
| 20 | 22,5 (16,5–27,5) | 48 (36,5–58) | 20,5 (16,5–24) | 7,5 (5,5–9,5) | 17,5 (9–23) | 29 (14–45,5) |
| 21 | 23 (17–27,5) | 48 (37–58) | 20,5 (16,5–24) | 7,5 (5,5–9,5) | 17,5 (9–23) | 33 (13–41,5) |
| 22 | 23,5 (16,5–27) | 47,5 (38–58,5) | 20,5 (16,5–24) | 8 (5,5–9,5) | 17,5 (9,5–23,5) | 27 (15–37) |
| 23 | 23,5 (16,5–27) | 48 (37–59,5) | 20,5 (16,5–24) | 8 (5,5–9,5) | 17,5 (9,5–24) | 27,5 (14,5–44) |
| 24 | 23,5 (16,5–27,5) | 49 (37–59) | 20,5 (15,5–23,5) | 8 (5,5–9,5) | 17,5 (9,5–23,5) | 33 (17–44,5) |
| 25 | 24 (16,5–27,5) | 49 (38,5–58,5) | 20,5 (15–23,5) | 8 (5,5–9,5) | 16,5 (10–23,5) | 32,5 (13–39) |
| 26 | 23,5 (17–27,5) | 49 (40–59) | 20,5 (15–23,5) | 8 (5,5–9,5) | 16,5 (10–23,5) | 28 (17–48) |
| 27 | 23,5 (16,5–27,5) | 49 (41,5–57,5) | 20,5 (15–23,5) | 8 (5,5–9,5) | 16,5 (10,5–23,5) | 29,5 (18–43) |
| 28 | 24 (16,5–27,5) | 49 (41–57,5) | 20,5 (15,5–23,5) | 8 (5,5–9,5) | 16,5 (11–23,5) | 26,5 (13–47,5) |
| 29 | 24 (17–27,5) | 49 (40,5–59) | 19,5 (14,5–23,5) | 7,5 (5,5–9,5) | 17 (11–23,5) | 29 (20,5–51,5) |
| 30 | 24 (17–28) | 49 (42,5–59) | 19,5 (14–23,5) | 7,5 (5,5–9,5) | 17 (11–24) | 28,5 (19,5–50) |
| 31 | 23,5 (17–27,5) | 49 (42,5–59) | 19,5 (14–23,5) | 7,5 (5,5–9,5) | 17 (11–24) | 28 (19–55,5) |
| 32 | 23,5 (17–27) | 49 (42–59,5) | 19,5 (15,5–23,5) | 7,5 (5,5–9,5) | 17 (11–24,5) | 29,5 (15,5–53) |
| 33 | 23,5 (17–27) | 49 (42–60) | 19,5 (15,5–23,5) | 7,5 (5,5–9,5) | 17 (11–24,5) | 30 (18,5–58,5) |
| 34 | 24 (17,5–26,5) | 50 (42–60) | 19 (15,5–23,5) | 7,5 (5,5–9,5) | 17 (11–24,5) | 34 (18,5–55,5) |
| 35 | 23,5 (17–27) | 52 (42–60,5) | 19,5 (15,5–23) | 7,5 (5,5–9,5) | 17,5 (11–24,5) | 30 (17,5–50) |
| 36 | 23,5 (17–27,5) | 52 (42–60) | 20 (15,5–23) | 7,5 (5,5–9,5) | 17,5 (11–24,5) | 30 (16–46) |
| 37 | 23,5 (16,5–27) | 51,5 (42–60) | 20 (15,5–23) | 7,5 (5,5–9,5) | 18 (11–24,5) | 26 (16,5–48) |
| 38 | 23 (17,5–27) | 51 (41,5–60,5) | 20 (15,5–23) | 7,5 (5,5–9,5) | 18 (12–24,5) | 28,5 (18,5–49) |
| 39 | 23,5 (18–26) | 51,5 (42–60,5) | 20 (15,5–23) | 7,5 (5,5–9,5) | 18 (12–24,5) | 28 (20–46) |
| 40 | 23,5 (18–26,5) | 51,5 (41,5–62,5) | 20 (15,5–23) | 7,5 (5,5–9,5) | 18 (12,5–25) | 27 (18,5–48) |
| 41 | 23,5 (17,5–26,5) | 50,5 (41–61,5) | 19,5 (15–22,5) | 7,5 (5,5–9,5) | 18 (13–25) | 25 (19,5–47,5) |
| 42 | 24 (18–26,5) | 50,5 (42–62) | 19,5 (15–22,5) | 7,5 (5,5–9,5) | 18 (13–25) | 28,5 (18,5–47,5) |
| 43 | 23 (18–26) | 52 (40,5–62) | 19,5 (15–22) | 7,5 (5,5–9,5) | 18 (13–25) | 27,5 (22–51) |
| 44 | 23,5 (18–26) | 52 (39,5–62) | 19,5 (15–22) | 7,5 (5,5–9,5) | 18 (12,5–25) | 27 (20–48) |
| 45 | 24 (18–26) | 52 (42–62) | 19,5 (14–22) | 7,5 (5,5–9,5) | 18,5 (11–25,5) | 30,5 (19,5–47) |
| 46 | 24 (17,5–25,5) | 51,5 (43–62) | 19,5 (14–21,5) | 7,5 (5,5–9,5) | 18,5 (11–25,5) | 28,5 (18–48,5) |
| 47 | 23,5 (18–25,5) | 51,5 (42–64) | 19,5 (14–21,5) | 7,5 (5,5–10) | 18 (11–25,5) | 32,5 (20–54,5) |
| 48 | 23,5 (18,5–25,5) | 51,5 (43,5–64) | 19 (14–21,5) | 7,5 (5,5–9) | 19 (11–25,5) | 27,5 (17,5–57) |
| 49 | 23 (17,5–26,5) | 53 (42,5–64) | 19,5 (14–21,5) | 7,5 (5,5–9) | 18,5 (11–25,5) | 25 (21–54,5) |
| 50 | 24 (18–27) | 54,5 (42,5–64) | 19 (14–21,5) | 7,5 (5,5–9) | 19 (11,5–25,5) | 32 (19,5–58) |
| 51 | 24 (18,5–27) | 54,5 (42,5–64) | 19 (14–21,5) | 7,5 (5,5–9) | 19,5 (11,5–26,5) | 30 (20–56,5) |
| 52 | 23,5 (18,5–25,5) | 54,5 (44–64,5) | 19 (14,5–20,5) | 7 (5,5–9) | 20 (11,5–27,5) | 37,5 (20,5–56) |
| 53 | 23,5 (18–25,5) | 55 (44,5–64,5) | 19 (14,5–20,5) | 7 (5,5–9) | 20,5 (11,5–26,5) | 32,5 (20–58) |
| 54 | 24 (18,5–25,5) | 55 (44,5–65) | 19 (14–22) | 7 (5,5–9) | 20,5 (11,5–27,5) | 31 (20,5–55,5) |
| 55 | 24,5 (17,5–26) | 55 (45–66) | 19 (13,5–22) | 7 (5,5–9) | 21 (11–29,5) | 30 (21,5–61) |
| 56 | 24 (17,5–26) | 54,5 (45–66,5) | 19 (13,5–22) | 7 (5,5–9) | 20 (11–29,5) | 32,5 (19,5–65,5) |
| 57 | 24 (18–26) | 54 (45–69,5) | 19 (13,5–22) | 7 (5,5–9) | 20 (11–30,5) | 32,5 (20–66) |
| 58 | 24 (18,5–27) | 54 (44,5–69) | 19 (13,5–22,5) | 7 (5,5–9) | 20 (11,5–31) | 33,5 (21–63,5) |
| 59 | 23,5 (18–27) | 54 (46–69) | 19 (13,5–22,5) | 7 (5,5–9) | 20,5 (11,5–31) | 31 (21,5–64) |
| 60 | 24 (18–27) | 53,5 (46,5–69) | 19 (13,5–22,5) | 7 (5,5–9) | 20,5 (12–31,5) | 33,5 (24,5–58) |

### v3: durum değişimi (1/2)

| Yıl | Yaşayan yerleşim (yıl ort.) | Büyük şehir (Şehir kademesi, yıl ort.) | El değiştiren yerleşim (fetih + bölünme) | El değiştiren büyük şehir | Büyük şehre hücum (kuşatma muharebesi) | Büyük şehir yağmalandı, tutulmadı |
|---|---|---|---|---|---|---|
| 1 | 77,9 (72,5–79,2) | 5,76 (4,16–7,85) | 2 (0–4,5) | 0,5 (0–2) | 0 (0–0,5) | 0 |
| 2 | 79 (73,4–80) | 5,94 (4,4–7,58) | 2 (1–4) | 0,5 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 3 | 79 (74,1–80) | 5,71 (4,15–7,28) | 2,5 (1–4,5) | 1 (0–2) | 0 (0–1,5) | 0 (0–1) |
| 4 | 79 (74,5–80) | 5,96 (3,79–7,34) | 2 (0–4,5) | 0,5 (0–2) | 0 (0–1) | 0 (0–1) |
| 5 | 79 (74,7–80) | 5,81 (3,71–7,53) | 3 (1–4,5) | 1 (0–2,5) | 0 (0–0,5) | 0 |
| 6 | 79 (73,7–80) | 5,8 (3,65–8) | 2 (0,5–4) | 0 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 7 | 79 (71,4–80) | 5,96 (3,95–8,48) | 2 (0,5–4) | 1 (0–3) | 0 (0–1) | 0 |
| 8 | 79 (67,7–80) | 5,86 (4,29–8,5) | 2 (0–3,5) | 1 (0–2) | 0 (0–1,5) | 0 (0–1) |
| 9 | 78,8 (65,2–80) | 5,69 (4,4–8,3) | 2 (0–4) | 1 (0–2) | 0 (0–1,5) | 0 (0–0,5) |
| 10 | 78,6 (67,6–80) | 5,79 (4,2–8,49) | 2 (1–4) | 0 (0–2,5) | 0 (0–1,5) | 0 (0–1) |
| 11 | 79 (68,6–79,8) | 5,78 (4,14–8,43) | 2 (0–4) | 0,5 (0–2,5) | 0 (0–1) | 0 (0–1) |
| 12 | 79 (69,8–79,5) | 6 (4,3–8,75) | 2,5 (0–5,5) | 1 (0–3) | 0 (0–1) | 0 |
| 13 | 78,7 (71,5–79,8) | 6 (5–8,5) | 2 (0–4,5) | 1 (0–3) | 0 (0–1) | 0 (0–1) |
| 14 | 78,7 (71,9–80) | 6,34 (4,53–8,98) | 3 (1–5) | 1,5 (0–3) | 0 (0–1,5) | 0 (0–0,5) |
| 15 | 78,7 (72–80,1) | 6,16 (4,73–8,7) | 2 (1–4) | 0,5 (0–3) | 0 (0–1,5) | 0 (0–1) |
| 16 | 79 (72–80) | 6,48 (4,7–8,55) | 2 (0–4) | 1 (0–2) | 0 (0–0,5) | 0 |
| 17 | 79 (72,1–80) | 6,93 (4,63–8,8) | 3 (0,5–5) | 1 (0–3) | 0 (0–1) | 0 (0–1) |
| 18 | 79 (72,9–80) | 6,48 (5,03–8,5) | 2 (1–3) | 1 (0–2) | 0 (0–2) | 0 (0–1) |
| 19 | 79 (73,6–80) | 7 (4,3–8,88) | 2 (1–4) | 1 (0–2,5) | 0 (0–1,5) | 0 (0–0,5) |
| 20 | 79 (73,7–80) | 6,45 (4,23–8,83) | 1,5 (0–4,5) | 1 (0–2,5) | 0 (0–1) | 0 (0–0,5) |
| 21 | 78,9 (74,1–80) | 6,21 (4,46–8,58) | 2,5 (0–3) | 1 (0–2) | 0 (0–1,5) | 0 (0–1) |
| 22 | 79 (73,9–80) | 6,45 (4,71–7,75) | 1 (0–4) | 0 (0–2,5) | 0 (0–0,5) | 0 |
| 23 | 79 (74,4–80) | 6,44 (4,24–8,05) | 2 (1,5–4) | 1 (0–2) | 0 (0–1) | 0 |
| 24 | 79 (74,7–80) | 6,19 (4,48–8,69) | 1 (0–4) | 1 (0–2,5) | 0 (0–1,5) | 0 (0–0,5) |
| 25 | 79 (72,6–80) | 6,24 (4,31–8,7) | 2,5 (1–5) | 1 (0–2) | 0 (0–1,5) | 0 (0–1) |
| 26 | 79 (73,9–80) | 6,04 (4,19–9,21) | 2 (0,5–3,5) | 1 (0–3) | 0 (0–0,5) | 0 (0–0,5) |
| 27 | 79 (75,3–80) | 6,16 (4,08–8,59) | 1 (0–3) | 1 (0–2,5) | 0 (0–2) | 0 (0–0,5) |
| 28 | 79 (75,6–80) | 5,86 (4,1–7,56) | 2 (0,5–4) | 1 (0–2) | 0,5 (0–1,5) | 0 (0–1,5) |
| 29 | 79 (76–80) | 5,41 (4,43–6,58) | 1,5 (1–5) | 1 (0–2) | 0 (0–1) | 0 (0–1) |
| 30 | 78,9 (76–80) | 5,84 (4,5–6,73) | 2 (0,5–4) | 1 (0–2) | 0 (0–1,5) | 0 (0–0,5) |
| 31 | 79 (74,9–80) | 5,95 (4,6–6,74) | 2 (0,5–4,5) | 1 (0–2,5) | 0 (0–0,5) | 0 |
| 32 | 79 (74,5–80) | 5,75 (4,66–7,09) | 2 (0,5–5,5) | 1 (0–2,5) | 0 (0–1,5) | 0 (0–1) |
| 33 | 79 (75,1–80) | 5,98 (4,66–6,88) | 1 (0–3) | 0,5 (0–2) | 0 (0–2) | 0 (0–0,5) |
| 34 | 78,7 (76–80) | 5,71 (4,06–6,8) | 2,5 (0,5–4,5) | 1 (0–2) | 0 (0–2,5) | 0 (0–1) |
| 35 | 78,5 (76–80) | 5,25 (3,96–6,98) | 2 (0,5–3,5) | 1 (0–2,5) | 0 (0–1) | 0 (0–0,5) |
| 36 | 78,5 (76–80) | 5,4 (3,55–6,84) | 2 (0,5–3,5) | 1 (0–2,5) | 0 (0–1) | 0 (0–1) |
| 37 | 78,5 (75,8–80) | 5,56 (3,93–7,1) | 2 (0–4) | 1 (0–2) | 0 (0–1,5) | 0 (0–1) |
| 38 | 78,5 (75,7–80) | 6,04 (4,54–7,74) | 1 (1–3,5) | 1 (0–1) | 0 (0–1) | 0 |
| 39 | 78,9 (76–80) | 5,88 (4,2–7,16) | 1,5 (0–4) | 0,5 (0–2) | 0 (0–1) | 0 (0–1) |
| 40 | 78,9 (76–80) | 5,66 (4,61–7,45) | 2 (0,5–3) | 1 (0–2) | 0 (0–1) | 0 |
| 41 | 79 (75,5–80) | 5,88 (4,4–8,14) | 2 (1–4) | 1 (0–2,5) | 0 (0–1,5) | 0 (0–1) |
| 42 | 79 (75,5–80) | 5,63 (4,06–8,06) | 1,5 (0,5–3,5) | 1 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 43 | 78,7 (75,5–80) | 5,95 (3,89–8,91) | 2,5 (0–4) | 1,5 (0–3) | 0 (0–1) | 0 (0–0,5) |
| 44 | 78 (73,6–80) | 5,68 (3,99–8,48) | 2 (0,5–4,5) | 1 (0–2,5) | 0 (0–1,5) | 0 (0–0,5) |
| 45 | 78,1 (72,9–80) | 5,74 (4,23–7,5) | 1 (1–3) | 1 (0–1,5) | 0 | 0 |
| 46 | 78,5 (73,3–80,1) | 5,76 (3,84–7,51) | 1 (0–4,5) | 1 (0–2,5) | 0 (0–1) | 0 (0–1) |
| 47 | 78 (73,7–80,5) | 5,81 (3,4–7,18) | 2 (0–3) | 1 (0–2) | 0 (0–1) | 0 (0–1) |
| 48 | 78,5 (74,2–80,5) | 5,85 (3,99–7,19) | 2 (0,5–3) | 1 (0–2) | 0 | 0 |
| 49 | 78,2 (74,5–80,5) | 5,88 (3,61–7,55) | 1 (0–2,5) | 0,5 (0–1) | 0 (0–1,5) | 0 (0–1) |
| 50 | 78,1 (74,8–80,5) | 6 (3,48–7,91) | 1 (1–2,5) | 1 (0–2) | 0 (0–0,5) | 0 |
| 51 | 78,9 (75,1–80,5) | 6 (4,03–7,71) | 2 (0–4) | 0 (0–2,5) | 0 (0–1,5) | 0 (0–0,5) |
| 52 | 78,5 (74,2–80,5) | 6,03 (4,11–8,09) | 3 (0–4) | 1 (0–2) | 0 (0–1) | 0 |
| 53 | 78,5 (74,4–80,5) | 6 (4,61–7,39) | 1,5 (0–3) | 0,5 (0–1,5) | 0 (0–2) | 0 (0–1) |
| 54 | 78,7 (73,8–80,5) | 5,93 (4,54–7,68) | 2 (1–3,5) | 1 (0,5–3) | 0 (0–1) | 0 |
| 55 | 79 (74,1–80,5) | 5,5 (4,49–8,04) | 2 (0–4) | 1 (0–2) | 0 (0–3) | 0 (0–0,5) |
| 56 | 78,6 (74,4–80,5) | 5,91 (4,5–8,08) | 2 (0–4,5) | 1 (0–2,5) | 0 | 0 |
| 57 | 78,6 (75,1–80,5) | 6 (4,99–8,19) | 2 (1–4) | 1 (0–2) | 0 (0–1,5) | 0 |
| 58 | 79 (75,5–80,5) | 6 (4,86–8,44) | 1 (1–3) | 1 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 59 | 78,9 (74,9–80,5) | 5,91 (4,5–8,85) | 2 (0,5–4) | 1 (0–3) | 0 (0–1) | 0 (0–0,5) |
| 60 | 78,9 (72,9–80,5) | 6,25 (4,5–8,64) | 2 (0–4) | 0,5 (0–2) | 0 | 0 |

### v3: durum değişimi (2/2)

| Yıl | Yerleşim durum değişimi (kuruluş hariç hepsi) | Orta halka kaydı (Köy/Kasaba: kademe değişimi ya da terk) | Açlık başlayan yerleşim | Salgın başlayan yerleşim | Yakılan/yanan yerleşim | Harabeye yeniden yerleşim |
|---|---|---|---|---|---|---|
| 1 | 38,5 (26,5–57,5) | 20 (13,5–30) | 0 | 0 (0–2) | 5,5 (3–12) | 1 (0–2,5) |
| 2 | 42 (27,5–59) | 18 (12–29) | 0 | 0 (0–1,5) | 8 (3,5–17) | 0 (0–0,5) |
| 3 | 46,5 (28–57) | 20,5 (11,5–27,5) | 0 (0–9,5) | 0 (0–2) | 6,5 (3–14) | 0 (0–1) |
| 4 | 40 (30–55) | 18,5 (13,5–25,5) | 0 (0–0,5) | 0 (0–1,5) | 8,5 (2–14,5) | 0 (0–0,5) |
| 5 | 42,5 (36–62) | 19 (13–25,5) | 0 (0–17) | 0 (0–3) | 8 (4–10,5) | 0 (0–0,5) |
| 6 | 43,5 (37–61,5) | 22 (11,5–30) | 0 (0–19,5) | 0 (0–2,5) | 6 (3,5–10) | 0 |
| 7 | 39 (26–60) | 16 (12–28) | 0 (0–10,5) | 0 (0–1) | 6,5 (3–12,5) | 0 |
| 8 | 43 (31,5–62) | 17,5 (13–27) | 0 (0–6) | 1 (0–2,5) | 7,5 (4–11) | 0 (0–1) |
| 9 | 42,5 (28,5–63) | 20 (9,5–33,5) | 0 (0–3) | 0 (0–1) | 6 (2–11,5) | 0 (0–2,5) |
| 10 | 41,5 (19,5–56) | 17 (7–30) | 0 | 0 (0–1,5) | 8 (2,5–15,5) | 0 (0–1) |
| 11 | 40 (18–59,5) | 17,5 (6,5–28) | 0 (0–6) | 0 (0–1,5) | 5 (2–11) | 0 (0–1) |
| 12 | 41,5 (26–61,5) | 15,5 (10,5–29) | 0 (0–8) | 0 (0–1) | 7,5 (2–11,5) | 0 (0–1) |
| 13 | 41 (26–52,5) | 18,5 (8,5–28,5) | 0 (0–7) | 0 (0–1,5) | 6,5 (2–13) | 0 (0–1,5) |
| 14 | 40 (28–59) | 17,5 (10–23) | 0 (0–15) | 0 (0–1) | 6 (3,5–16) | 0 (0–1) |
| 15 | 39 (26–57) | 15,5 (11–22,5) | 0 (0–16) | 0 (0–1,5) | 8,5 (3,5–14,5) | 0 |
| 16 | 39 (24–55,5) | 18,5 (12–27) | 0 (0–6) | 0 (0–1) | 6 (3–15,5) | 0 |
| 17 | 41 (29,5–63,5) | 18 (14–24) | 0 (0–8) | 0 (0–1) | 9 (4–20,5) | 0 |
| 18 | 37 (24–48,5) | 13,5 (11–21) | 0 (0–12) | 0 (0–1) | 8 (4–12,5) | 0 |
| 19 | 40,5 (25–57,5) | 16 (10,5–27) | 0 (0–11,5) | 0 (0–1) | 7 (1,5–17,5) | 0 |
| 20 | 36,5 (21,5–53,5) | 15 (8–24) | 0 (0–11,5) | 0 (0–1) | 8 (2,5–13,5) | 0 |
| 21 | 35,5 (26–41) | 16,5 (10,5–26) | 0 (0–2) | 0 (0–1) | 5,5 (3–10,5) | 0 (0–1) |
| 22 | 33,5 (25,5–54) | 18,5 (12–23) | 0 (0–7,5) | 1 (0–2) | 6,5 (2–10) | 0 (0–0,5) |
| 23 | 43,5 (26,5–56,5) | 16,5 (11–24,5) | 0 (0–8,5) | 0,5 (0–1) | 6 (3–15,5) | 0 |
| 24 | 34,5 (25,5–52,5) | 17,5 (11–26,5) | 0 (0–2,5) | 0 (0–1) | 5 (2–12) | 0 (0–0,5) |
| 25 | 40 (27,5–57,5) | 16 (12–24,5) | 0 (0–4,5) | 0 (0–2) | 7,5 (3–16) | 0 |
| 26 | 36 (26,5–56,5) | 14,5 (12,5–24) | 0 (0–6,5) | 0 (0–1,5) | 6 (2,5–15,5) | 0 (0–1) |
| 27 | 37 (30–48,5) | 18 (11–21,5) | 0 (0–12,5) | 1 (0–1,5) | 5 (3–12) | 0 (0–0,5) |
| 28 | 33,5 (25–53,5) | 16 (9,5–27) | 0 (0–10,5) | 0 (0–1) | 6,5 (1–12) | 0 (0–0,5) |
| 29 | 38 (25–52) | 17 (9,5–24,5) | 0 (0–1) | 0 (0–1,5) | 6,5 (4–14) | 0 (0–0,5) |
| 30 | 38 (25,5–65) | 16,5 (9,5–26) | 0 (0–5,5) | 0 (0–2) | 7 (2–16,5) | 0 (0–1) |
| 31 | 36 (30–54,5) | 15 (10–23,5) | 0 (0–16) | 0 (0–1) | 7,5 (3–10,5) | 0 (0–1) |
| 32 | 42 (29–79,5) | 16 (11–24,5) | 0 (0–51) | 0 (0–1) | 7,5 (4–14) | 0 (0–0,5) |
| 33 | 35 (22,5–47) | 16 (9–23,5) | 0 (0–11,5) | 0,5 (0–1) | 6,5 (1–9,5) | 0 (0–0,5) |
| 34 | 37 (28,5–51,5) | 15 (8–24,5) | 0 (0–7) | 0,5 (0–1,5) | 7 (2,5–12) | 0 |
| 35 | 36,5 (27,5–46) | 18 (10–22) | 0 (0–5,5) | 1 (0–3) | 6 (4–12,5) | 0 (0–1,5) |
| 36 | 34 (21,5–46,5) | 16 (12–24,5) | 0 (0–6) | 0 (0–1,5) | 5 (2,5–8,5) | 0 (0–0,5) |
| 37 | 37 (30,5–54) | 18 (12–22,5) | 0 (0–15,5) | 0 (0–1) | 6,5 (3–10) | 0 (0–0,5) |
| 38 | 38,5 (27–58) | 18 (13–26,5) | 0 (0–19) | 1 (0–2) | 6,5 (3–10) | 0 (0–1) |
| 39 | 32,5 (24–54,5) | 14 (8–26,5) | 0 (0–6) | 0 (0–2,5) | 6,5 (3,5–11) | 0 (0–1) |
| 40 | 34 (22–50,5) | 18 (10–23,5) | 0 (0–7) | 0 (0–1,5) | 5,5 (1,5–10) | 0 |
| 41 | 34 (28–52,5) | 18,5 (10–27,5) | 0 (0–5,5) | 0 (0–1) | 6,5 (1,5–10) | 0 |
| 42 | 37,5 (23–58,5) | 15,5 (11–25) | 0 (0–24,5) | 0 (0–1,5) | 5,5 (3–9) | 0 (0–1) |
| 43 | 31,5 (27–47) | 16,5 (12–21,5) | 0 (0–2,5) | 1 (0–1,5) | 5 (2,5–9) | 0 |
| 44 | 35,5 (21,5–49,5) | 14 (9–20,5) | 0 (0–7,5) | 0 (0–2) | 7,5 (2,5–13) | 0 (0–1,5) |
| 45 | 28 (21,5–48,5) | 14,5 (8,5–24,5) | 0 (0–6) | 0 (0–1) | 5,5 (3–8) | 0 (0–1) |
| 46 | 33,5 (18–79) | 14,5 (6–21,5) | 0 (0–46,5) | 0 (0–0,5) | 3 (1,5–10,5) | 0 |
| 47 | 31 (22–43) | 15 (7,5–22) | 0 (0–11,5) | 0 (0–1) | 5 (2–10,5) | 0 (0–1) |
| 48 | 28 (19,5–53,5) | 12 (8,5–20,5) | 0 (0–23) | 0 (0–1) | 4,5 (2–7) | 0 (0–1) |
| 49 | 31 (18–45,5) | 12,5 (8–21,5) | 0 (0–13) | 0 (0–1,5) | 5,5 (2–11,5) | 0 |
| 50 | 33 (23,5–48,5) | 17,5 (8,5–23) | 0 (0–14,5) | 0 (0–1) | 6 (2,5–11,5) | 0 (0–0,5) |
| 51 | 32,5 (18,5–50,5) | 9 (6–21,5) | 0 (0–10) | 1 (0–4) | 5,5 (1,5–15) | 0 |
| 52 | 32,5 (25,5–47,5) | 15,5 (9–23,5) | 0 (0–10) | 0 (0–1,5) | 6 (2–10,5) | 0 (0–1,5) |
| 53 | 31,5 (20,5–44,5) | 14,5 (6,5–24,5) | 0 (0–8) | 0 (0–1) | 6 (3–11) | 0 (0–1) |
| 54 | 32 (26–45) | 15 (8,5–23,5) | 0 (0–11) | 0 (0–1,5) | 4,5 (1,5–11) | 0 (0–1) |
| 55 | 34,5 (20,5–47) | 15 (7–20,5) | 0 (0–16,5) | 0 (0–2) | 5 (2,5–8,5) | 0 (0–1) |
| 56 | 29,5 (18–55,5) | 13 (5,5–24) | 0 (0–6,5) | 0 (0–2) | 6 (2–10) | 0 (0–1,5) |
| 57 | 32,5 (22,5–51,5) | 17 (7,5–20) | 0 (0–12,5) | 0 (0–2,5) | 5 (1,5–10) | 0 (0–0,5) |
| 58 | 30 (21,5–50,5) | 14 (5,5–20) | 0 (0–27) | 0 (0–1) | 4 (2–8,5) | 0 |
| 59 | 30 (22–45) | 15 (7–22) | 0 (0–5,5) | 0 (0–2,5) | 7 (4–10) | 0 (0–1) |
| 60 | 34,5 (22–48) | 15,5 (6–20,5) | 0 (0–10) | 0 | 5 (2,5–10,5) | 0 (0–0,5) |

### Devlet, inanç ve örgüt (1/5)

| Yıl | Yaşayan örgüt (yıl sonu) | Örgüt şubesi (yıl sonu) | Gizli şube (yıl sonu) | Örgüt üyesi (yıl sonu) | Açılan şube | Kapanan şube |
|---|---|---|---|---|---|---|
| 1 | 13 (12–13) | 300 (222–352) | 15 (11–22) | 1434 (1108–1575) | 22 (19–27) | 18,5 (9–28) |
| 2 | 13 (12–13) | 300 (216–343) | 16 (10–21,5) | 1378 (1105–1584) | 17,5 (14–21,5) | 24 (13–34,5) |
| 3 | 13 (12–13) | 296 (224–336) | 16 (9,5–20,5) | 1394 (1062–1576) | 18,5 (12,5–25) | 21 (14,5–34,5) |
| 4 | 13 (12–13) | 290 (220–337) | 15 (8,5–19,5) | 1366 (1076–1562) | 16 (12,5–19) | 16,5 (10,5–23) |
| 5 | 13 (12–13) | 292 (223–330) | 13 (8–19) | 1414 (1068–1524) | 17 (12–20) | 14,5 (10–28) |
| 6 | 13 (12–13) | 290 (220–336) | 12 (7,5–18,5) | 1426 (1068–1521) | 17,5 (14,5–20,5) | 19 (10,5–27) |
| 7 | 13 (12–13) | 288 (213–340) | 12 (7,5–18) | 1436 (1120–1566) | 16 (12,5–20,5) | 16 (10,5–26,5) |
| 8 | 13 | 283 (217–342) | 9,5 (7–17) | 1434 (1124–1528) | 15 (12,5–19) | 17 (11,5–27) |
| 9 | 13 | 284 (210–335) | 11 (7,5–16) | 1434 (1138–1536) | 16 (12,5–21) | 18 (13,5–22,5) |
| 10 | 13 (12,5–13) | 289 (210–336) | 8,5 (5,5–14,5) | 1444 (1122–1553) | 16 (13,5–22,5) | 16 (10,5–21,5) |
| 11 | 13 (12–13) | 294 (210–336) | 8 (6–16) | 1466 (1134–1570) | 15 (11,5–19) | 15,5 (8,5–18) |
| 12 | 13 (12–13) | 292 (206–338) | 9 (5,5–14) | 1479 (1110–1560) | 15,5 (11,5–21,5) | 14,5 (10–20,5) |
| 13 | 13 (12–13) | 296 (212–338) | 9 (5,5–15,5) | 1440 (1113–1558) | 15 (10,5–19) | 13 (9,5–22,5) |
| 14 | 13 (12–13) | 298 (216–341) | 9 (6–14) | 1462 (1091–1578) | 18 (13–22) | 15 (9,5–19,5) |
| 15 | 13 (12–13) | 306 (214–350) | 9 (6–15) | 1464 (1064–1589) | 16,5 (10–19,5) | 11 (8–19) |
| 16 | 13 (12–13) | 304 (217–350) | 9 (6–14) | 1460 (1074–1626) | 16 (12,5–19) | 14 (10,5–19) |
| 17 | 13 | 307 (217–354) | 8 (5,5–13) | 1506 (1082–1620) | 16,5 (10,5–24) | 17,5 (8–22,5) |
| 18 | 13 (12–13) | 307 (220–355) | 9 (5,5–12,5) | 1514 (1074–1648) | 15 (12,5–19) | 13 (7,5–18,5) |
| 19 | 13 (12–13) | 308 (222–357) | 8 (5–12) | 1531 (1096–1653) | 16,5 (13,5–20,5) | 14 (8,5–19,5) |
| 20 | 13 (12–13) | 304 (227–361) | 8 (5–12,5) | 1500 (1079–1656) | 16 (12–18) | 15,5 (10–21) |
| 21 | 13 (12–13) | 298 (232–363) | 8 (6–12,5) | 1524 (1100–1668) | 14 (12,5–18) | 15 (8–19) |
| 22 | 13 (12–13) | 305 (236–370) | 8 (5,5–11,5) | 1545 (1086–1662) | 16 (10,5–19,5) | 10 (8–18) |
| 23 | 13 (12–13) | 302 (236–372) | 8 (6,5–12) | 1550 (1089–1690) | 17 (13–19,5) | 13,5 (8–24,5) |
| 24 | 13 (12–13) | 303 (240–366) | 8,5 (6,5–12) | 1552 (1120–1668) | 16 (13–20,5) | 12 (7,5–21,5) |
| 25 | 13 (12–13) | 304 (241–373) | 9,5 (5–12,5) | 1564 (1152–1694) | 17,5 (14–21,5) | 13 (7–21,5) |
| 26 | 13 (12–13) | 310 (243–376) | 9,5 (5,5–13) | 1556 (1174–1696) | 15 (10,5–19) | 12 (5,5–14) |
| 27 | 13 (12–13) | 306 (242–371) | 9 (7–13) | 1570 (1174–1670) | 14,5 (12–20) | 17,5 (10,5–25) |
| 28 | 13 (12–13) | 307 (239–368) | 8 (6–13) | 1546 (1170–1684) | 15 (11–18,5) | 15 (8,5–23,5) |
| 29 | 13 (12–13) | 303 (235–368) | 9 (6–13) | 1568 (1179–1670) | 15 (11–20) | 15,5 (10,5–21,5) |
| 30 | 12,5 (12–13) | 295 (234–372) | 7,5 (6,5–13,5) | 1561 (1189–1672) | 15 (11–17,5) | 12,5 (9,5–18) |
| 31 | 13 (12–13) | 308 (240–373) | 8 (4,5–12,5) | 1556 (1212–1656) | 15,5 (11–20) | 13,5 (8,5–19,5) |
| 32 | 13 (12–13) | 311 (240–371) | 8,5 (4,5–13) | 1528 (1202–1674) | 16 (11–20) | 15,5 (11–24) |
| 33 | 13 (12–13) | 306 (237–373) | 8,5 (5–11,5) | 1522 (1213–1688) | 15 (10–18,5) | 15 (10–23,5) |
| 34 | 13 (12–13) | 309 (238–376) | 8 (4,5–11,5) | 1520 (1206–1710) | 16 (10–19,5) | 13,5 (9–21) |
| 35 | 13 (12–13) | 308 (246–374) | 8 (5–11,5) | 1522 (1230–1724) | 14 (12–18,5) | 14 (7–20) |
| 36 | 13 (12–13) | 312 (255–372) | 8 (5,5–11,5) | 1566 (1241–1711) | 15,5 (12–21,5) | 11,5 (6–22,5) |
| 37 | 13 (12–13) | 310 (258–372) | 8 (5,5–11,5) | 1568 (1250–1697) | 16 (11,5–20) | 14 (11–18) |
| 38 | 13 (12–13) | 316 (251–381) | 8 (4,5–11) | 1541 (1258–1750) | 15,5 (13–21,5) | 14 (8–23,5) |
| 39 | 13 (12–13) | 318 (252–374) | 7 (5,5–10,5) | 1542 (1243–1766) | 16,5 (11,5–20,5) | 12,5 (9–21) |
| 40 | 13 (12–13) | 320 (257–374) | 8 (4,5–10,5) | 1559 (1264–1752) | 15,5 (8–19) | 13,5 (8,5–18) |
| 41 | 13 (12–13) | 312 (256–368) | 8 (3,5–12) | 1564 (1270–1738) | 15 (12–17) | 13 (9,5–24,5) |
| 42 | 13 (12–13) | 312 (262–370) | 7 (4–11,5) | 1585 (1330–1752) | 13,5 (11,5–19) | 11,5 (8,5–21,5) |
| 43 | 13 (12–13) | 308 (264–364) | 8 (3,5–12) | 1583 (1283–1772) | 15,5 (11,5–20) | 15,5 (7–27) |
| 44 | 13 (12–13) | 311 (267–361) | 8 (4–11,5) | 1581 (1264–1766) | 15,5 (11–17,5) | 14 (9,5–20,5) |
| 45 | 13 (12–13) | 313 (264–362) | 7,5 (3,5–11) | 1607 (1300–1777) | 15 (10,5–19) | 14 (7,5–24) |
| 46 | 13 (12–13) | 316 (272–370) | 8 (3,5–11) | 1625 (1295–1796) | 14,5 (12–19) | 11 (4,5–17,5) |
| 47 | 13 (12–13) | 322 (267–378) | 7 (3–12) | 1644 (1286–1826) | 15 (11–18,5) | 13 (6–19,5) |
| 48 | 13 (12–13) | 321 (270–376) | 7,5 (3,5–10) | 1668 (1280–1815) | 15 (12,5–19,5) | 11,5 (6–16,5) |
| 49 | 13 (12–13) | 320 (270–376) | 7 (3,5–10) | 1672 (1272–1802) | 15 (10,5–17,5) | 15,5 (10–17) |
| 50 | 13 (12–13) | 320 (273–376) | 7,5 (4–10,5) | 1682 (1292–1825) | 14,5 (9–19) | 13,5 (7–19,5) |
| 51 | 12,5 (12–13) | 326 (272–372) | 7 (3,5–11,5) | 1684 (1324–1828) | 15,5 (10,5–17,5) | 9,5 (7–20) |
| 52 | 13 (12–13) | 324 (273–378) | 7,5 (4–12,5) | 1683 (1355–1846) | 15 (11–17) | 12 (6,5–19,5) |
| 53 | 13 (12–13) | 328 (280–376) | 9 (4–13) | 1685 (1376–1830) | 14 (11–21) | 14 (10–21,5) |
| 54 | 13 (12–13) | 324 (282–384) | 9 (3,5–14) | 1674 (1382–1848) | 15 (11,5–18) | 14,5 (8,5–21,5) |
| 55 | 13 (12–13) | 325 (285–380) | 8 (4,5–13) | 1662 (1378–1856) | 14,5 (11–19,5) | 14,5 (8,5–21) |
| 56 | 12 (12–13) | 325 (286–386) | 8 (5–14) | 1676 (1404–1832) | 17 (10–20) | 11,5 (8–21) |
| 57 | 12 (12–13) | 326 (284–388) | 8 (4,5–11) | 1674 (1378–1860) | 15 (8,5–20) | 15,5 (10,5–23) |
| 58 | 12 (12–13) | 326 (285–386) | 6 (4,5–9,5) | 1676 (1387–1876) | 13 (10–16,5) | 12,5 (9–19) |
| 59 | 12 (12–13) | 322 (278–389) | 6 (4,5–10,5) | 1681 (1402–1872) | 14 (9–17) | 12,5 (9,5–22) |
| 60 | 12,5 (12–13) | 324 (270–386) | 7 (4–10) | 1696 (1385–1894) | 13 (11,5–18) | 14,5 (7–23) |

### Devlet, inanç ve örgüt (2/5)

| Yıl | Dağılan örgüt | Yeniden kurulan örgüt | Gölge savaşı eylemi | Gölge savaşında öldürülen usta ya da lider | Gizli şubeye baskın | Lobiyle yasa değişikliği |
|---|---|---|---|---|---|---|
| 1 | 0 (0–1) | 0 | 9 (5–11,5) | 2 (1–2,5) | 1 (0–2,5) | 1,5 (0–3,5) |
| 2 | 0 | 0 | 7,5 (4–10,5) | 2 (0,5–3,5) | 0,5 (0–1) | 1 (0–2) |
| 3 | 0 | 0 | 6,5 (2–11) | 1 (0–2,5) | 1 (0–2) | 1 (0–3) |
| 4 | 0 | 0 | 8,5 (4,5–11) | 1,5 (0–3) | 0 (0–1,5) | 0,5 (0–2,5) |
| 5 | 0 | 0 | 8 (4–11) | 2 (1–4) | 1 (0–2) | 1 (0–1,5) |
| 6 | 0 | 0 | 8 (5,5–11,5) | 2 (0–3) | 0 (0–2) | 1 (0–2,5) |
| 7 | 0 | 0 | 7 (3,5–11) | 2 (0–3) | 1 (0–1) | 1 (0–1,5) |
| 8 | 0 | 0 (0–0,5) | 9 (6–11,5) | 2 (0,5–4) | 1 (0–2,5) | 1 (0–2,5) |
| 9 | 0 | 0 | 8 (4–10,5) | 2 (0–4) | 0 (0–1,5) | 1 (0–3) |
| 10 | 0 | 0 | 8,5 (5,5–11) | 2 (1–3,5) | 1 (0–2) | 1 (0–3,5) |
| 11 | 0 (0–0,5) | 0 | 7,5 (3,5–12,5) | 1,5 (0–3,5) | 0 (0–1) | 1 (0–3,5) |
| 12 | 0 | 0 | 8 (4,5–10) | 1 (0–3) | 0 (0–2) | 1 (0–5) |
| 13 | 0 | 0 | 6 (2–9,5) | 1 (0–2,5) | 0 (0–1) | 0 (0–3,5) |
| 14 | 0 | 0 | 7,5 (5–10,5) | 1,5 (0–3) | 0 (0–1,5) | 0,5 (0–2) |
| 15 | 0 | 0 | 7 (4–11,5) | 1 (0–2) | 0 (0–1,5) | 1,5 (0–4) |
| 16 | 0 | 0 | 7 (3,5–9) | 1 (0–2) | 0 (0–1) | 1 (0–3,5) |
| 17 | 0 | 0 (0–0,5) | 7,5 (5–11,5) | 1,5 (0,5–3,5) | 0 (0–2) | 1,5 (0–3) |
| 18 | 0 (0–1) | 0 | 7 (5–14) | 1 (0,5–3,5) | 1 (0–2) | 1 (0–2) |
| 19 | 0 | 0 | 6,5 (5–9) | 1,5 (0–3) | 1 (0–2) | 1,5 (0–3) |
| 20 | 0 (0–0,5) | 0 | 7,5 (5–10) | 1,5 (0,5–4) | 0 (0–1,5) | 1 (0–2) |
| 21 | 0 | 0 | 8 (3,5–9) | 1 (0–3) | 0 (0–1) | 1 (0–1,5) |
| 22 | 0 | 0 | 7 (5–9,5) | 0,5 (0–3) | 0 (0–1) | 1 (0–2,5) |
| 23 | 0 | 0 | 8 (3,5–10,5) | 2 (1–4) | 0 (0–1) | 1 (0–2) |
| 24 | 0 | 0 | 4 (2–9) | 1 (0–2) | 1 (0–2) | 1 (0–2) |
| 25 | 0 | 0 | 6,5 (3,5–11,5) | 1,5 (0–3,5) | 0,5 (0–2) | 2 (1–2,5) |
| 26 | 0 | 0 | 7 (4–11) | 2 (0–3) | 1 (0–2) | 1 (0–3) |
| 27 | 0 | 0 (0–0,5) | 8,5 (3–11) | 2 (0,5–3) | 0 (0–1) | 1 (0–2,5) |
| 28 | 0 | 0 | 8,5 (4,5–11) | 1 (0–3) | 1 (0–1) | 2 (0,5–2,5) |
| 29 | 0 (0–1) | 0 | 7,5 (2,5–9) | 1 (0–4) | 0,5 (0–2) | 1 (0–4) |
| 30 | 0 | 0 | 7,5 (3–9,5) | 1 (0–2) | 0 (0–1) | 0,5 (0–1,5) |
| 31 | 0 | 0 (0–0,5) | 6 (2–11) | 1,5 (0–4) | 0 (0–1) | 1 (0–3) |
| 32 | 0 | 0 (0–0,5) | 7 (4,5–10,5) | 2 (0,5–3,5) | 0 (0–1,5) | 1,5 (0–2) |
| 33 | 0 | 0 (0–0,5) | 6,5 (5–12) | 1 (0–3,5) | 1 (0–2) | 2 (0–3) |
| 34 | 0 | 0 | 7 (4–12,5) | 1,5 (0,5–3,5) | 1 (0–2) | 1 (0–2,5) |
| 35 | 0 | 0 | 7,5 (3–10) | 1 (0–4) | 1 (0–2) | 2 (0–3,5) |
| 36 | 0 | 0 | 7 (4–13) | 1 (0–4) | 0 (0–1) | 1 (0–1,5) |
| 37 | 0 | 0 (0–0,5) | 7,5 (4,5–10) | 1 (0–3) | 0 (0–1,5) | 1,5 (0–2,5) |
| 38 | 0 | 0 | 8 (5–12,5) | 2 (0,5–4) | 0,5 (0–2) | 1 (0–2) |
| 39 | 0 | 0 | 9 (1,5–12,5) | 2 (0–3,5) | 0 (0–2) | 1 (0–2) |
| 40 | 0 | 0 | 7 (4,5–10,5) | 1,5 (0–2,5) | 0 (0–1,5) | 1 (0,5–2,5) |
| 41 | 0 | 0 | 7 (5–10,5) | 2 (0–3,5) | 0 (0–1) | 1 (0–3) |
| 42 | 0 | 0 | 6 (3,5–9,5) | 1 (0–3) | 0,5 (0–1) | 2 (0–3,5) |
| 43 | 0 | 0 | 6,5 (4–8,5) | 2 (0,5–3,5) | 0 (0–2) | 1 (0–2,5) |
| 44 | 0 | 0 | 7,5 (4–9,5) | 1 (0,5–2) | 0 (0–1) | 1 (0–3,5) |
| 45 | 0 (0–0,5) | 0 (0–1) | 7,5 (4–11) | 1,5 (0,5–3,5) | 0 (0–2) | 1,5 (0–3) |
| 46 | 0 | 0 | 9 (4,5–11) | 2 (0–3) | 0 (0–1) | 1 (0–3) |
| 47 | 0 | 0 | 7,5 (4–10) | 2 (0–3) | 1 (0–1,5) | 1 (0–1,5) |
| 48 | 0 | 0 | 6 (3–11) | 1,5 (0,5–4,5) | 0,5 (0–1,5) | 1 (0–2,5) |
| 49 | 0 | 0 | 6,5 (3–9) | 1 (0–2) | 0,5 (0–3) | 1 (0–2,5) |
| 50 | 0 | 0 (0–0,5) | 6,5 (4,5–9,5) | 2 (0–3,5) | 0 (0–1,5) | 0 (0–2,5) |
| 51 | 0 (0–1) | 0 | 6,5 (3,5–11,5) | 1 (0–2,5) | 0 (0–1) | 1 (0–3) |
| 52 | 0 | 0 (0–0,5) | 5 (3–7,5) | 1 (0–2) | 0 (0–1,5) | 1 (0–2) |
| 53 | 0 | 0 | 7,5 (5–9,5) | 1 (0–3) | 1 (0–2) | 0 (0–2,5) |
| 54 | 0 | 0 | 8 (5–12) | 2 (0–3,5) | 0 (0–1) | 1 (0–3,5) |
| 55 | 0 (0–0,5) | 0 | 7,5 (3,5–10,5) | 1,5 (0–2,5) | 0,5 (0–1,5) | 1 (0–2) |
| 56 | 0 (0–1) | 0 | 6,5 (2,5–9,5) | 1 (0–3) | 0 (0–3) | 1 (0–3) |
| 57 | 0 (0–0,5) | 0 | 7 (3,5–9,5) | 1,5 (0–3) | 1 (0–1) | 1 (0–2,5) |
| 58 | 0 | 0 | 5,5 (3,5–9,5) | 1 (0–5,5) | 0 (0–2,5) | 1 (0–3) |
| 59 | 0 | 0 | 5 (2,5–9) | 1 (0–3) | 0 (0–1) | 1 (0–3) |
| 60 | 0 | 0 (0–0,5) | 5,5 (2–8,5) | 1,5 (0,5–3) | 0 (0–2) | 1 (0–3) |

### Devlet, inanç ve örgüt (3/5)

| Yıl | Darbe girişimi | Örgüt ilanı (Avcılar) | Örgüt üyesi kahraman payı (yıl sonu) | Yönetici değişimi | Veraset krizi | Meşruiyet ortalaması (yıl sonu) |
|---|---|---|---|---|---|---|
| 1 | 0 | 2 (1–3) | %100 (%99–%100) | 0 (0–2) | 0 | 65,8 (61,9–69,2) |
| 2 | 0 | 1 (0–3) | %100 (%99–%100) | 1 (0–2) | 0 | 66,7 (59,8–69,1) |
| 3 | 0 | 1 (0–2,5) | %100 (%99–%100) | 1 (0–2) | 0 | 66,6 (62,3–71,2) |
| 4 | 0 | 0,5 (0–3) | %100 (%99–%100) | 1 (0–2) | 0 | 65,1 (60,8–72,5) |
| 5 | 0 | 1 (0–2,5) | %99 (%99–%100) | 1 (0–2,5) | 0 | 66,5 (61,5–69,7) |
| 6 | 0 | 1 (0–2) | %99 (%99–%100) | 1 (0–2) | 0 | 67,2 (61,2–69,7) |
| 7 | 0 | 1 (0–1,5) | %99 (%99–%100) | 1 (0–2,5) | 0 | 66,1 (58,9–72,4) |
| 8 | 0 | 1 (0–3) | %99 (%99–%100) | 0 (0–1,5) | 0 | 67,3 (60,6–71,7) |
| 9 | 0 | 1 (0–3) | %99 (%99–%100) | 1 (0–2) | 0 | 66,5 (61,6–71,2) |
| 10 | 0 | 1 (0–2) | %99 (%98–%100) | 0 (0–2) | 0 | 67 (61,3–72,5) |
| 11 | 0 | 1 (0–2,5) | %99 (%98–%100) | 1 (0–2,5) | 0 | 65,9 (59,2–73,2) |
| 12 | 0 | 1 (0–3) | %99 (%98–%100) | 1 (0–2,5) | 0 | 66,5 (56,5–71,7) |
| 13 | 0 | 1 (0–2,5) | %99 (%98–%100) | 1 (0–2,5) | 0 | 66 (61–70,3) |
| 14 | 0 | 1 (0–2,5) | %99 (%98–%100) | 1 (0–2) | 0 | 63,8 (61,1–70,5) |
| 15 | 0 | 1 (0–2) | %99 (%98–%100) | 1 (0–2) | 0 | 65,7 (56,6–72,3) |
| 16 | 0 | 1 (0–2,5) | %99 (%98–%100) | 1 (0–2,5) | 0 | 64,8 (59,6–73,6) |
| 17 | 0 (0–0,5) | 1 (0–2,5) | %99 (%98–%100) | 1 (0–2,5) | 0 | 64,8 (59,5–72,5) |
| 18 | 0 (0–0,5) | 1 (0–2) | %99 (%98–%100) | 1 (0–2) | 0 | 66,2 (57,7–72,5) |
| 19 | 0 | 0 (0–2,5) | %99 (%98–%100) | 1 (0–2,5) | 0 | 64,8 (58,7–74) |
| 20 | 0 | 1 (0–2,5) | %99 (%98–%100) | 1 (0–3) | 0 | 65,7 (59,4–73,5) |
| 21 | 0 | 1 (0–3) | %99 (%98–%100) | 1 (0–2) | 0 | 66,4 (60,5–72,1) |
| 22 | 0 | 1 (1–3) | %99 (%98–%100) | 0 (0–2,5) | 0 | 67,9 (58,4–74) |
| 23 | 0 | 1 (0–3,5) | %99 (%98–%100) | 1,5 (0–3) | 0 | 66,6 (59,4–72,9) |
| 24 | 0 | 1 (0–3) | %99 (%98–%100) | 1 (0–2,5) | 0 | 67,3 (61,7–70,6) |
| 25 | 0 | 1 (0–3) | %99 (%98–%100) | 1 (0–3) | 0 | 64,2 (62,5–71,7) |
| 26 | 0 | 1 (0–2) | %99 (%98–%100) | 1 (0–3) | 0 | 67 (62,9–72,1) |
| 27 | 0 | 1 (0–2,5) | %99 (%98–%100) | 0,5 (0–2) | 0 | 67,6 (60–72,5) |
| 28 | 0 | 0 (0–1,5) | %99 (%98–%100) | 1 (0–2,5) | 0 | 68,6 (58,2–72,7) |
| 29 | 0 | 1 (0–1,5) | %100 (%98–%100) | 1 (0,5–3) | 0 | 65,6 (61,3–72,4) |
| 30 | 0 | 1 (0–2) | %99 (%98–%100) | 1 (0–2,5) | 0 | 65,3 (61,2–74,7) |
| 31 | 0 (0–0,5) | 1 (0–2,5) | %99 (%98–%100) | 1,5 (0–3) | 0 | 66,5 (60,8–72,9) |
| 32 | 0 | 1 (0–3) | %100 (%98–%100) | 1 (0–2,5) | 0 | 64,4 (57,5–72,3) |
| 33 | 0 | 1 (0–3,5) | %99 (%98–%100) | 1 (0–2) | 0 | 66,5 (59,1–73,5) |
| 34 | 0 | 1 (0–4) | %100 (%98–%100) | 2 (0–3) | 0 | 65,8 (56–73,9) |
| 35 | 0 | 1 (0–3) | %100 (%98–%100) | 1 (0–2,5) | 0 | 64,3 (60,4–72,6) |
| 36 | 0 | 1 (0,5–3,5) | %99 (%98–%100) | 1 (0–2,5) | 0 | 66,5 (56,2–73,5) |
| 37 | 0 | 0,5 (0–4) | %99 (%98–%100) | 1 (0–2) | 0 | 69,3 (58,3–74,2) |
| 38 | 0 | 1 (0–4) | %99 (%98–%100) | 1 (0–2) | 0 | 67,1 (60,1–75) |
| 39 | 0 | 1 (0–4) | %100 (%98–%100) | 0,5 (0–2,5) | 0 | 68,1 (56,5–76,2) |
| 40 | 0 | 1 (0–3,5) | %100 (%98–%100) | 1 (0–2) | 0 | 69,1 (57,9–75,6) |
| 41 | 0 | 1 (0–2,5) | %100 (%98–%100) | 1,5 (0–2,5) | 0 | 67 (61,7–71,4) |
| 42 | 0 | 1 (0–3) | %100 (%98–%100) | 1 (0–2) | 0 | 70 (61,4–72,3) |
| 43 | 0 | 1 (0–3) | %100 (%98–%100) | 1,5 (0–3) | 0 | 70,2 (58,4–73) |
| 44 | 0 | 1 (0–3) | %100 (%98–%100) | 1 (0–2) | 0 | 68,7 (58,6–73,1) |
| 45 | 0 | 1 (0–3) | %100 (%98–%100) | 0 (0–2) | 0 | 68,8 (61,6–74,1) |
| 46 | 0 | 1 (0–3,5) | %100 (%98–%100) | 1 (0–2,5) | 0 | 67,5 (62,2–73,3) |
| 47 | 0 | 1 (0–2,5) | %100 (%98–%100) | 1 (0–2,5) | 0 | 69,4 (62,5–73,5) |
| 48 | 0 | 1 (0–3,5) | %100 (%98–%100) | 1 (0–2,5) | 0 | 70 (64,5–72,9) |
| 49 | 0 | 1 (0–2,5) | %99 (%99–%100) | 0,5 (0–1,5) | 0 | 69,5 (63,9–73,5) |
| 50 | 0 | 1 (0–2,5) | %100 (%99–%100) | 1 (0,5–2) | 0 | 66,8 (62,8–73,3) |
| 51 | 0 | 1 (0–2) | %100 (%99–%100) | 1 (0–2,5) | 0 | 68,4 (57,6–73,9) |
| 52 | 0 (0–0,5) | 1 (0–3) | %100 (%99–%100) | 1 (0–3) | 0 | 68,8 (60,5–74,4) |
| 53 | 0 | 1 (0–3) | %99 (%99–%100) | 1 (0–2) | 0 | 68 (63,3–76) |
| 54 | 0 | 1 (0–2) | %100 (%99–%100) | 1,5 (0,5–4) | 0 | 65,4 (57,7–75) |
| 55 | 0 | 1 (0–3) | %100 (%99–%100) | 1 (0,5–2,5) | 0 | 66,5 (55,1–72,3) |
| 56 | 0 | 1 (0–2,5) | %100 (%99–%100) | 1 (0–2,5) | 0 | 63,4 (54–73,6) |
| 57 | 0 | 1 (0–2) | %99 (%99–%100) | 1 (0–2,5) | 0 | 66,6 (56,8–73,2) |
| 58 | 0 | 1 (0,5–2,5) | %99 (%99–%100) | 1 (0–2,5) | 0 | 66 (58,7–72) |
| 59 | 0 | 0,5 (0–1,5) | %100 (%99–%100) | 1 (0–3) | 0 | 66,3 (59,4–72,4) |
| 60 | 0 | 1 (0–2,5) | %100 (%99–%100) | 1 (0–2,5) | 0 | 67,4 (57,2–75,8) |

### Devlet, inanç ve örgüt (4/5)

| Yıl | Pakt'a bağlı yönetici (yıl sonu) | Köle (yıl sonu) | Köle payı (nüfusun, yıl sonu) | Hapis madeninde mahkûm (yıl sonu) | Esarete düşen | Kurtulan köle (Özgürlük Ağı, kaçış, azat) |
|---|---|---|---|---|---|---|
| 1 | 0 | 19 (1,5–42) | %0,7 (%0,1–%2) | 0 (0–1) | 6 (1–11,5) | 12 (5–19) |
| 2 | 0 | 11 (0,5–46,5) | %0,4 (%0–%1,9) | 0 (0–2,5) | 5,5 (1,5–12,5) | 8,5 (3,5–17,5) |
| 3 | 0 | 10,5 (0–45) | %0,4 (%0–%2) | 0,5 (0–2) | 4 (0–22) | 10,5 (1–17) |
| 4 | 0 (0–0,5) | 8,5 (3–41,5) | %0,3 (%0,1–%1,9) | 0 (0–1) | 5,5 (1–17,5) | 7,5 (2–12) |
| 5 | 0 (0–0,5) | 8 (1,5–42) | %0,3 (%0,1–%1,6) | 0 (0–2) | 3,5 (0,5–12) | 6,5 (2,5–14,5) |
| 6 | 0 (0–1) | 7,5 (0–37) | %0,3 (%0–%1,7) | 0 (0–1) | 5 (0–15) | 7 (2–14,5) |
| 7 | 0 | 8,5 (0–32) | %0,3 (%0–%1,5) | 0 (0–2,5) | 5 (0–13) | 6,5 (0–10,5) |
| 8 | 0 (0–0,5) | 9 (0–30,5) | %0,3 (%0–%1,4) | 0 (0–2) | 5,5 (0–18,5) | 5 (1,5–13) |
| 9 | 0 (0–0,5) | 6,5 (0–35,5) | %0,2 (%0–%1,4) | 0 (0–2) | 4,5 (0–12) | 6 (0–10,5) |
| 10 | 0 (0–1) | 5,5 (0–40) | %0,2 (%0–%1,5) | 0,5 (0–3,5) | 6 (0–14) | 6,5 (0–13,5) |
| 11 | 0 | 7 (0,5–35,5) | %0,3 (%0–%1,3) | 0 (0–1,5) | 4 (0–13,5) | 3,5 (0–15) |
| 12 | 0 (0–1) | 5 (0–33,5) | %0,2 (%0–%1,2) | 0 (0–1) | 5 (0–11,5) | 5,5 (0,5–15) |
| 13 | 0 (0–1) | 4,5 (0,5–31,5) | %0,2 (%0–%1,2) | 0,5 (0–4) | 2 (0,5–13) | 5 (0–11) |
| 14 | 0 (0–0,5) | 8,5 (0–25) | %0,3 (%0–%1,1) | 1 (0–2) | 5 (0–14,5) | 3 (0–16) |
| 15 | 0 (0–1) | 10,5 (0,5–32) | %0,3 (%0–%1,3) | 0 (0–2) | 3,5 (0,5–21) | 4,5 (0–9,5) |
| 16 | 0 (0–1) | 12 (1–28,5) | %0,4 (%0–%1,3) | 0 (0–2,5) | 4 (0,5–11,5) | 5 (0–11) |
| 17 | 0 (0–1) | 6,5 (1–36) | %0,2 (%0–%1,2) | 0 (0–2,5) | 4,5 (0–10,5) | 7 (0,5–10,5) |
| 18 | 0 (0–1) | 9 (0,5–37) | %0,4 (%0–%1,3) | 0,5 (0–3) | 4,5 (0–17,5) | 6 (0–13) |
| 19 | 0 (0–0,5) | 11 (0,5–33) | %0,4 (%0–%1,2) | 0,5 (0–2) | 6 (0–19) | 8 (0–13,5) |
| 20 | 0 (0–0,5) | 7,5 (0–29,5) | %0,3 (%0–%1,1) | 0 (0–4) | 4,5 (0–11,5) | 7 (0–13) |
| 21 | 0 (0–0,5) | 8,5 (0–25) | %0,3 (%0–%1,1) | 0 (0–3) | 5,5 (0–11,5) | 7 (0–11) |
| 22 | 0 (0–0,5) | 7,5 (0–34,5) | %0,2 (%0–%1,3) | 0 (0–1) | 3 (0–18,5) | 4,5 (0–12,5) |
| 23 | 0 | 5,5 (0–31) | %0,2 (%0–%1) | 0 (0–1,5) | 4 (0–13) | 7,5 (0–15) |
| 24 | 0 | 9 (0,5–30,5) | %0,3 (%0–%1) | 0 (0–3) | 2,5 (0–10,5) | 4,5 (0–9,5) |
| 25 | 0 | 7,5 (0,5–34,5) | %0,3 (%0–%1,2) | 0 (0–3) | 5,5 (0–13,5) | 7,5 (0–11) |
| 26 | 0 | 9,5 (0,5–30,5) | %0,3 (%0–%1,1) | 0 (0–2,5) | 4 (0–13) | 4,5 (0,5–11) |
| 27 | 0 (0–0,5) | 12,5 (0,5–33,5) | %0,4 (%0–%1,3) | 0 (0–3,5) | 8,5 (0–13) | 5,5 (0–11) |
| 28 | 0 (0–1) | 7,5 (1–30) | %0,2 (%0–%1,2) | 0 (0–5) | 4 (0–12,5) | 6,5 (1,5–13,5) |
| 29 | 0 (0–1) | 9,5 (1–24) | %0,3 (%0–%0,9) | 0 (0–3,5) | 4 (0–15) | 3,5 (0,5–10,5) |
| 30 | 0 | 10,5 (0,5–28) | %0,3 (%0–%1,1) | 0 (0–3) | 5,5 (1–12,5) | 5 (1–12) |
| 31 | 0 | 9,5 (2–31,5) | %0,3 (%0,1–%1,2) | 0 (0–6,5) | 6 (0–13,5) | 7 (1,5–12) |
| 32 | 0 | 10,5 (1–34) | %0,4 (%0–%1,3) | 0 (0–4) | 4,5 (1–15,5) | 5 (0,5–15) |
| 33 | 0 | 7,5 (0,5–31,5) | %0,3 (%0–%1,3) | 0,5 (0–2,5) | 3 (0–12) | 5,5 (0–13) |
| 34 | 0 | 6,5 (0,5–34) | %0,2 (%0–%1,3) | 0 (0–2,5) | 6 (0–17) | 6,5 (0,5–11) |
| 35 | 0 (0–1) | 7 (0,5–29) | %0,3 (%0–%1,1) | 0 (0–2,5) | 4 (0–9) | 4 (0–12) |
| 36 | 0 | 7,5 (0,5–28) | %0,3 (%0–%1) | 1 (0–2) | 6 (0–10) | 5 (0–9,5) |
| 37 | 0 | 6 (0,5–29,5) | %0,2 (%0–%1,1) | 0 (0–2,5) | 4 (0–12) | 5,5 (0–11,5) |
| 38 | 0 | 7,5 (0,5–30) | %0,3 (%0–%1,1) | 0 (0–1,5) | 7 (0,5–16) | 6 (1,5–11,5) |
| 39 | 0 | 6 (0,5–23,5) | %0,2 (%0–%0,9) | 0,5 (0–2) | 4 (0–10,5) | 4 (0–11,5) |
| 40 | 0 | 5 (0–25,5) | %0,2 (%0–%1) | 0 (0–1,5) | 3 (0,5–13,5) | 4 (0,5–11) |
| 41 | 0 (0–1) | 4,5 (0,5–33,5) | %0,1 (%0–%1,2) | 0 (0–1) | 3,5 (0–13,5) | 2,5 (0–11,5) |
| 42 | 0 (0–0,5) | 5 (0–33,5) | %0,2 (%0–%1,2) | 0 (0–4,5) | 3,5 (0–14,5) | 3,5 (0–12,5) |
| 43 | 0 | 6 (0–40) | %0,2 (%0–%1,6) | 0 (0–3,5) | 2 (0–12) | 3 (0–10) |
| 44 | 0 | 6,5 (0,5–50,5) | %0,2 (%0–%2,1) | 0 (0–3,5) | 5 (0–25) | 4 (0–16) |
| 45 | 0 (0–0,5) | 6,5 (0,5–45) | %0,2 (%0–%1,7) | 0 (0–3) | 4 (0–12,5) | 5 (1–14) |
| 46 | 0 (0–0,5) | 4,5 (0–52) | %0,2 (%0–%1,9) | 0 (0–3,5) | 6 (0–17) | 5,5 (0,5–12,5) |
| 47 | 0 (0–0,5) | 8 (2,5–52,5) | %0,3 (%0,1–%2) | 0 (0–3) | 4,5 (2,5–13) | 6 (2–13,5) |
| 48 | 0 (0–0,5) | 5,5 (1,5–38,5) | %0,3 (%0–%1,4) | 0 (0–4,5) | 4 (1,5–9,5) | 5,5 (2–13) |
| 49 | 0 (0–0,5) | 6 (0,5–41,5) | %0,2 (%0–%1,4) | 0 (0–9) | 6 (0,5–13) | 5,5 (0,5–12,5) |
| 50 | 0 (0–0,5) | 5,5 (0–31) | %0,2 (%0–%1,1) | 0 (0–4,5) | 3 (0–12) | 4 (0–15) |
| 51 | 0 | 11 (0–32,5) | %0,4 (%0–%1,5) | 0 (0–3) | 5,5 (0,5–22) | 7,5 (0,5–13) |
| 52 | 0 | 9 (0–37) | %0,3 (%0–%1,6) | 0 (0–6,5) | 4,5 (0–22) | 6 (0–22) |
| 53 | 0 | 10,5 (0,5–34) | %0,4 (%0–%1,5) | 0 (0–3) | 6 (1–12,5) | 4,5 (1–15,5) |
| 54 | 0 | 12 (0,5–32,5) | %0,4 (%0–%1,2) | 0 (0–3) | 4,5 (0–14) | 5 (0,5–12) |
| 55 | 0 | 9 (0–31,5) | %0,3 (%0–%1,1) | 0 (0–3) | 7 (0–15) | 6,5 (1,5–15) |
| 56 | 0 | 9,5 (1–40) | %0,3 (%0–%1,4) | 0 (0–2) | 8 (0–15,5) | 6 (1–12,5) |
| 57 | 0 | 10,5 (0–33) | %0,4 (%0–%1,3) | 0 (0–2) | 4 (0,5–14,5) | 7 (1,5–13) |
| 58 | 0 | 5 (0,5–40) | %0,2 (%0–%1,4) | 0,5 (0–3) | 3,5 (0–13,5) | 6,5 (0–12,5) |
| 59 | 0 (0–0,5) | 7 (0,5–34) | %0,3 (%0–%1,6) | 0 (0–2) | 4,5 (0–13,5) | 5 (0–11) |
| 60 | 0 | 8 (0,5–30) | %0,3 (%0–%1,3) | 0,5 (0–3) | 2,5 (0–16,5) | 4,5 (0–12) |

### Devlet, inanç ve örgüt (5/5)

| Yıl | Özgürlük Ağı'nın kurtardığı köle | Esir kahraman (yıl sonu) | Aç haydut kampı (yıl sonu) | Haydut olan aç halk | Aç ya da ekmeksiz yerleşim payı (köy+) | Devriye durdurması |
|---|---|---|---|---|---|---|
| 1 | 7 (4–13,5) | 0 (0–0,5) | 0 (0–1) | 11 (0–19,5) | %3,5 (%1,1–%5,4) | 47 (24,5–64,5) |
| 2 | 6 (1–9) | 0 (0–1) | 1 (0–2) | 10,5 (0–39) | %3,4 (%0,9–%6) | 46 (26,5–64,5) |
| 3 | 7 (1–14,5) | 0 (0–1) | 0,5 (0–1) | 6 (0–25,5) | %2,6 (%0,7–%6,4) | 50,5 (23–64,5) |
| 4 | 4,5 (2–9) | 0 (0–1,5) | 0 (0–1,5) | 10,5 (0–33,5) | %3,9 (%1,3–%11) | 49,5 (27–71,5) |
| 5 | 5 (2–11) | 0 (0–1) | 1 (0–2) | 14 (3–32) | %5 (%1,5–%12) | 46,5 (32,5–61) |
| 6 | 6 (2–9,5) | 0 (0–1) | 1 (0–2,5) | 9 (1,5–22,5) | %3,9 (%1–%12) | 51,5 (30–60) |
| 7 | 5 (0–8) | 0 (0–2) | 0 (0–2) | 12,5 (3–24) | %6,2 (%1,1–%12) | 53 (24,5–70,5) |
| 8 | 5 (0,5–10,5) | 0 (0–1) | 1 (0–2) | 13 (7–30) | %6,1 (%2,1–%11) | 55,5 (23,5–67,5) |
| 9 | 5,5 (0–8,5) | 0 (0–1) | 1 (0–2) | 12 (3–27) | %6,3 (%1,9–%9,5) | 48 (21–65,5) |
| 10 | 5 (0–11,5) | 0 (0–1) | 0 (0–1) | 6 (0–18) | %3,7 (%0,6–%9,7) | 49 (29–61,5) |
| 11 | 3,5 (0–11,5) | 0 (0–1) | 1 (0–2) | 12 (0–35) | %4,2 (%1,1–%13) | 51 (27,5–68) |
| 12 | 4 (0–11,5) | 0 (0–1,5) | 0 (0–2) | 15 (0–34) | %5,8 (%0,9–%14) | 52,5 (31–65,5) |
| 13 | 4,5 (0–9) | 0 (0–2) | 0 (0–1) | 10 (1,5–28,5) | %4,4 (%1,2–%17) | 55 (25–65,5) |
| 14 | 3 (0–8) | 0 (0–1) | 0,5 (0–2,5) | 10 (0–34,5) | %4,1 (%1,4–%18) | 58,5 (38,5–70,5) |
| 15 | 3,5 (0–8,5) | 0 (0–1) | 1 (0–1) | 15 (6–34) | %4,7 (%3–%17) | 47 (28,5–68) |
| 16 | 5 (0–9,5) | 0 (0–2) | 0 (0–1,5) | 11,5 (3–19,5) | %3,6 (%1,8–%14) | 48,5 (27–66) |
| 17 | 5 (0–9) | 0 (0–1) | 0 (0–3) | 18,5 (3,5–31,5) | %3,8 (%1,5–%16) | 54,5 (26–65) |
| 18 | 5 (0–10) | 0,5 (0–1) | 0 (0–2) | 10,5 (3,5–23) | %3,8 (%1,5–%17) | 55,5 (27,5–67) |
| 19 | 7 (0–10) | 0 (0–1,5) | 0 (0–1) | 9 (0–18) | %5,3 (%1–%13) | 53 (32,5–70,5) |
| 20 | 4,5 (0–9,5) | 0 (0–1,5) | 0,5 (0–2,5) | 11 (1,5–25) | %4,8 (%1,5–%9,4) | 56,5 (26–70) |
| 21 | 7 (0–10) | 0 (0–1) | 0 (0–2) | 4,5 (0–31) | %4 (%1,1–%15) | 54 (26,5–74,5) |
| 22 | 3,5 (0–11) | 0,5 (0–2) | 0 (0–1) | 12 (0–31) | %5 (%1,7–%13) | 57 (31,5–64) |
| 23 | 5 (0–7,5) | 1 (0–1,5) | 0,5 (0–1,5) | 10,5 (6–35) | %5 (%1,8–%15) | 57 (26,5–69,5) |
| 24 | 3 (0–8,5) | 0 (0–0,5) | 0,5 (0–2) | 11 (0–25,5) | %5,1 (%1,5–%12) | 56,5 (23,5–70) |
| 25 | 7 (0–9,5) | 0 (0–1) | 0 (0–1) | 10,5 (1,5–30) | %3,6 (%1,3–%14) | 61 (34,5–76,5) |
| 26 | 4 (0–10) | 0 (0–1) | 1 (0–1,5) | 12 (3–47) | %4,2 (%2,1–%18) | 51,5 (29,5–70,5) |
| 27 | 5 (0–8,5) | 0 (0–1) | 0 (0–1) | 10,5 (3,5–46) | %3,1 (%1,8–%19) | 56 (22–72) |
| 28 | 4 (0,5–12) | 0 (0–1) | 1 (0–1,5) | 9 (4,5–18) | %3,2 (%1,3–%11) | 53,5 (32–63) |
| 29 | 3,5 (0,5–8,5) | 0 (0–1) | 0,5 (0–2) | 12 (1,5–50) | %5,1 (%2,1–%17) | 55,5 (32–77,5) |
| 30 | 4,5 (0,5–10,5) | 0 (0–1,5) | 0 (0–1,5) | 6 (0–25) | %2,8 (%1–%11) | 53 (24,5–64) |
| 31 | 4,5 (1–9) | 0 (0–2) | 0,5 (0–2) | 6 (0–27) | %3 (%1,4–%15) | 54,5 (32–79,5) |
| 32 | 5 (0,5–10) | 0 (0–0,5) | 0,5 (0–1) | 9,5 (3–39) | %4,2 (%1,3–%19) | 56,5 (25–69,5) |
| 33 | 5 (0–10,5) | 0 (0–1,5) | 0 (0–2) | 13,5 (1,5–35,5) | %3,9 (%1,5–%11) | 52 (30,5–70) |
| 34 | 6,5 (0–9) | 0 (0–1) | 0 (0–1) | 11,5 (2–26,5) | %4,7 (%1,7–%12) | 54 (36–64,5) |
| 35 | 3 (0–9,5) | 0 (0–1,5) | 0,5 (0–2) | 11 (0–37,5) | %4,6 (%1,4–%14) | 51 (24,5–80,5) |
| 36 | 4 (0–8,5) | 0 (0–0,5) | 0,5 (0–2,5) | 10,5 (0–31,5) | %2,6 (%1,5–%12) | 63,5 (30,5–70,5) |
| 37 | 5 (0–9) | 0 (0–1) | 0 (0–2) | 7,5 (0–35) | %5,5 (%0,9–%14) | 57 (31–67,5) |
| 38 | 4,5 (1,5–10) | 0 | 1 (0–2) | 12 (4–27) | %2,9 (%0,9–%13) | 59 (33,5–65) |
| 39 | 3,5 (0–9,5) | 0,5 (0–1,5) | 1 (0–2) | 10,5 (0–27) | %2,6 (%1,2–%14) | 59 (34,5–64,5) |
| 40 | 4 (0–8,5) | 0 (0–1) | 0 (0–1) | 7 (1,5–25,5) | %3,2 (%1,7–%11) | 53 (34,5–70) |
| 41 | 2,5 (0–10) | 0 (0–0,5) | 1 (0–2) | 8 (3–48) | %3,6 (%1,5–%16) | 59 (40–73) |
| 42 | 3,5 (0–8,5) | 0 (0–1,5) | 1 (0–1,5) | 6 (0–42) | %3,5 (%1,5–%20) | 53,5 (40–64) |
| 43 | 2,5 (0–8,5) | 0 (0–1,5) | 0,5 (0–1) | 12 (0–58,5) | %3,9 (%1,5–%22) | 53 (39–75,5) |
| 44 | 4 (0–11,5) | 0 | 1 (0–3) | 12 (3–43,5) | %3,5 (%1,4–%18) | 53,5 (33,5–63) |
| 45 | 5 (1–8,5) | 0 (0–1) | 1 (0–1,5) | 16,5 (1,5–34) | %5,8 (%1,3–%16) | 49 (37,5–66) |
| 46 | 5 (0,5–9,5) | 0 (0–1,5) | 1 (0–2,5) | 12 (4,5–60) | %4,7 (%2,1–%30) | 57 (40–71,5) |
| 47 | 5,5 (1,5–11) | 0 (0–1) | 1 (0–4,5) | 13,5 (1,5–91) | %4 (%1–%35) | 56,5 (36,5–68) |
| 48 | 5 (2–10,5) | 0 (0–1,5) | 1 (0–3) | 13 (3–38,5) | %6,3 (%1–%23) | 52 (42,5–70) |
| 49 | 5 (0,5–11) | 0 (0–2,5) | 0 (0–2) | 22,5 (1,5–42,5) | %6,7 (%1,4–%22) | 56 (41,5–73,5) |
| 50 | 4 (0–10,5) | 0 (0–0,5) | 0 (0–2) | 15 (1,5–43) | %4,4 (%1,2–%18) | 61,5 (37–70) |
| 51 | 6 (0–11) | 0 (0–1,5) | 1 (0–2) | 9 (3–52) | %6,9 (%1,6–%20) | 53,5 (40–68) |
| 52 | 5 (0–14,5) | 0 (0–1,5) | 0 (0–1,5) | 12,5 (4,5–41) | %6,3 (%1–%20) | 51 (27,5–57) |
| 53 | 3 (1–14) | 0 (0–1,5) | 1 (0–2,5) | 23 (6,5–41,5) | %8,1 (%2,1–%20) | 51 (34–64) |
| 54 | 4 (0,5–11) | 0 (0–1) | 1 (0–2,5) | 19 (1,5–37) | %9,7 (%1,5–%20) | 57,5 (38–69,5) |
| 55 | 6,5 (1–13) | 0 (0–2) | 0 (0–1) | 18 (6–39,5) | %8,4 (%2,2–%16) | 54 (33,5–81) |
| 56 | 4,5 (0,5–9,5) | 0 (0–1) | 1,5 (0–3,5) | 18 (3–44) | %5,8 (%1,9–%19) | 56,5 (36–67) |
| 57 | 5,5 (0–11,5) | 1 (0–1,5) | 1 (0–2) | 12 (1,5–41,5) | %4,5 (%2,2–%20) | 55,5 (38–80) |
| 58 | 5,5 (0–8) | 0 (0–1) | 0 (0–1,5) | 15,5 (1,5–37,5) | %4,9 (%2,6–%17) | 56 (41–68) |
| 59 | 3,5 (0–9,5) | 0 (0–1,5) | 1 (0–3,5) | 15 (1,5–63) | %5,4 (%1,4–%21) | 61,5 (39–67) |
| 60 | 3,5 (0–7) | 0 (0–3) | 1 (0–2,5) | 11,5 (1,5–39) | %6 (%1,7–%19) | 55,5 (33,5–68) |

