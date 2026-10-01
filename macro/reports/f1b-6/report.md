# Ölçüm raporu: f1b-6

16 dünya (seed 1-16) × 60 yıl (2400 gün), 1 yıl = 40 gün · 2026-10-01 23:25 · `FD.Macro.Run stats --seeds 1-16 --years 60 --jobs 2 --verify 1 --saveload 1`

Süre: 9 dk 12 sn duvar saati, 2 iş parçacığı; dünya başına 59,8 sn (en az 33,1, en çok 96,5; yıl sonu hash'leri dâhil).

## Bitiş ölçütleri

DESIGN-FAZ1.md, "Bitiş ölçütleri" (1–5), yol haritası v3 (6–7, Faz 1b-4) ve v3'ün süre tablosu (8, Faz 1b-5). ✓ geçti · ✗ kaldı · — ölçülemedi · ○ bilgi (hedef yok) · † zaman ölçeğine bağlı (Faz 1b-5'ten beri takvim yeni ölçekte: 1 yıl = 40 gün, değerler doğrudan okunur).

| # | Ölçüt | Koşul | Ölçülen | Sonuç |
|---|---|---|---|---|
| 1 | Donma yok | 41–60. yılların yıllık büyük olay medyanı ≥ 0,8 × 6–20. yılların medyanı | 113 / 94,5 = 1,2 kat | ✓ |
| 2 | Çöküş | dünyaların ≥ %75'inde 60 yılda ≥ 1 çöküş (yok olma ya da başkent kaybı) | %100 (16/16 dünya); toplam 65 çöküş: 24 yok olma, 41 başkent kaybı | ✓ |
| 3 | Kamplar | 41–60. yıllarda yaşayan kamp medyanı ≥ 6–20. yılların medyanı | 8,98 < 9,23 (yıl sonu sayımıyla 9 / 9) | ✗ |
| 4a | Kahraman: doğuş seviyesi | her on yılda doğanların ortalama seviyesi ≤ 2 | 1,56 · 1,58 · 1,56 · 1,53 · 1,57 · 1,56 (on yıllar sırasıyla) | ✓ |
| 4b | Kahraman: Sv8+ | dünyaların ≥ yarısında en az bir kahraman Sv8 ve üstüne çıkar | %100 (16/16 dünya); dünyadaki en yüksek seviye: medyan Sv10, en çok Sv10 | ✓ |
| 4c | Kahraman: efsane | dünya başına efsane medyanı 1–6 | medyan 2 (p10–p90: 0–4,5; toplam 35) | ✓ |
| 4d | Kahraman: ölüm payı | doğan kahramanların %30–80'i ölür | %29 (2939/10281); dünya medyanı %31 (%20–%39) | ✗ |
| 5a | Determinizm | aynı seed → aynı tarih (toplayıcılı ve toplayıcısız koşu, her yıl sonu hash'i) | seed 1: 60/60 yıl sonu aynı | ✓ |
| 5b | Kayıt/yükleme | kaydet → yükle → devam = kesintisiz koşu (her yıl sonu hash'i) | seed 1, gün 1237: yüklenen dünya aynı, sonraki 30/30 yıl sonu aynı | ✓ |
| 6a | Yerleşim sayısı sabit, ısınmadan sonra (21–60. yıl) | dünya başına yaşayan yerleşim sayısının (günlük) değişim katsayısı medyanı ≤ %10 | değişim katsayısı %1 (%0,3–%2,3); ortalama 74,7 yerleşim, en az ve en çok ortalamanın %98 ve %101 kadarı | ✓ |
| 6b | Yerleşim sayısı sabit, bütün koşu (1–60. yıl) | dünya başına yaşayan yerleşim sayısının (günlük) değişim katsayısı medyanı ≤ %10 (Faz 1b-6: dünya tarih öncesiyle olgun başlar) | değişim katsayısı %1,4 (%0,9–%2,2); ortalama 74,9 yerleşim, en az ve en çok ortalamanın %97 ve %102 kadarı | ✓ |
| 6c | El değiştirme (her yerleşim) | fetih + bölünme, 100 günde, dünya başına ve yerleşim başına (21–60. yıl); hedef yok | dünyada 1,84 (0,44–3,34) / 100 gün; yerleşim başına 2,65 / 10 bin gün: 1,84 / 100 gün | ○ |
| 6d | Durum dalgalanması (yerleşim başına) | açlık, salgın, kuşatma, yakılma, kademe, el değiştirme, terk, yeniden yerleşim: yerleşim başına 100 günde (21–60. yıl); hedef yok | yerleşim başına 0,49 (0,28–0,62) / 100 gün, yani ~202 günde bir; dünyada 32,4 / 100 gün: yerleşim başına ~202 günde bir | ○ |
| 6e | Orta halka: kademe değişimi † | Köy/Kasaba başına kademe değişimi ya da terk, yeni takvimde 60–150 günde bir (21–60. yıl) | yerleşim başına 776 (588–1310) günde bir | ✗ |
| 6f | Büyük şehir el değiştirmesi † | büyük şehir (Şehir kademesi) bütün dünyada yeni takvimde 100 günde 2–4 kez el değiştirir (21–60. yıl); felaketle düşmez | dünyada 0,13 (0–0,25) / 100 gün; şehir başına 0,02 / 100 gün; dünyada ortalama 4,65 büyük şehir; toplam 30 el değiştirme (0 bölünme). Ayrıştırma: dünyada 2,78 savaş / 100 gün, hedefi büyük şehir olan %19; büyük şehre 168 hücum, düşüşle bitenlerin payı %18 | ✗ |
| 6g | Büyük şehir: uyarı süresi † | büyük şehrin düşüşünden önce yeni takvimde 5–10 gün uyarı (kuşatmanın başı → düşüş, medyan; 21–60. yıl) | kuşatmanın başından 5 gün (5–5; n = 30); savaşın başından 16,5 gün (10,9–36,7) | ✓ |
| 6h | Savaş süresi † | savaş (ilandan barışa) yeni takvimde 10–40 gün (medyan; 21–60. yıl başlayıp biten savaşlar) | 15 gün (10–45; n = 775) | ✓ |
| 6i | Kuşatma süresi † | büyük şehir kuşatması (karargâhtan hücuma) yeni takvimde 2–6 gün (medyan; 21–60. yıl) | büyük şehir 6 gün (6–6; n = 168); diğer yerleşimler 3 gün (3–6; n = 732) | ✓ |
| 6j | Başkent kaybı (medeniyet başına) | hiçbir medeniyet başkentini 3 kereden çok kaybetmez (yol haritası: "en fazla birkaç kez") | en çok 2; 3'ten çok kaybeden 0 medeniyet; toplam 41 başkent kaybı, 34/111 medeniyette (1×: 27, 2×: 7) | ✓ |
| 7 | Felaket büyük şehri düşürmez | Şehir kademesine varmış yerleşim hiç terk edilmez; ejderha akını büyük şehrin nüfusunu Şehir eşiğinin (85) altına indiremez; akından sonraki 60 günde terk yok | terk edilen eski Şehir: 0; ejderha akını 642 (büyük şehre 75, orada 885 ölü): akınla eşiğin altına inen 0, sonraki 60 günde terk 0; bilgi: akın günü başka nedenlerle (ordu, öncü) eşiğin altına inen 1, 60 gün içinde kademe düşüşü 22, el değiştirme 0 | ✓ |
| 8a | Salgın süresi | salgın başladığı günden bittiği güne: 5–10 gün (medyan) | 6 gün (4–7; n = 434) | ✓ |
| 8b | Tepki inşaatı: yanan ev | yanan her ev yandığı günden onarıldığı güne (ilk yanan ilk onarılır): 1–3 gün (medyan) | 3 gün (2–5; n = 14640) | ✓ |
| 8c | Tepki inşaatı: sur | palisat ve taş sur: projenin başından bitişine: 5–10 gün (medyan) | 9 gün (7–10; n = 322); palisat 10, taş sur 8 gün | ✓ |
| 8d | Büyük proje (kale, kule) | kale ve fener kulesi: projenin başından bitişine: 15–30 gün (medyan) | 23 gün (20–23; n = 120); kale 20 (n = 9), kule 23 gün (n = 111) | ✓ |
| 8e | Temizlenen kamp → yeni köy | temizlenen kara kampının 6 fersah yakınına 60 gün içinde ilk yerleşimin kurulması: 10–20 gün (medyan) | 11 gün (5,4–51; n = 125); temizlenen 3750 kampın 125 tanesine (%3,3) köy kuruldu, 20 gün içinde %2,1 | ✓ |
| 8f | Han kurulumu | yeni han: hancının yola çıktığı günden kapıların açıldığı güne: 5–10 gün (medyan) | 6 gün (5,4–7; n = 5); harabeyi yeniden kurma 3 gün (n = 4) | ✓ |
| 8g | Kahraman doğumu (han) | han başına 10–20 günde bir (açık han-günü / handa doğan kahraman) | 19 günde bir (9226 doğum / 174881 han-günü); aynı handa iki doğum arası medyan 13 gün (2–43); bilgi: taverna başına 981 günde bir (1055 doğum) | ✓ |
| 8h | Kahramanın efsaneye yükselişi | doğumundan efsane olduğu güne: 100–300 gün (medyan) | 246 gün (47,2–1105; n = 35) | ✓ |
| 8i | İlan ömrü | alınmayan ilanın asıldığı günden kapandığı güne (süresi doldu ya da kampı başkası temizledi): 10–20 gün (medyan) | 15 gün (15–15; n = 4778); bütün ilanlar 9 gün (n = 10404): biten %33 (asılıştan 5 günde), süresi dolan %45; başarısız sefer ödülü %25 artırır (Heroes.QuestFailed) | ✓ |
| 8j | Yoldaş/kahraman maaşı | haftalık (5 gün) | kural: medeniyetin kahramanları her 5. gün (Economy.PayHeroes), han personeli haftada bir (InnLife.PayWages); maaşı 3 hafta ödenmeyen kahraman ayrılır | ○ |
| 9a | Örgütler yaşar, şubeleri dalgalanır | hiçbir örgüt kalıcı olarak yok olmaz (koşu sonunda ya yaşıyor ya da yeniden kurulmayı bekliyor); örgüt başına yıllık şube sayısının değişim katsayısı medyanı ≥ %10 | kalıcı yok olan 0/208; koşu sonunda yaşayan 201; dağılma 62, yeniden kuruluş 55; şube değişim katsayısı %16 (%8,7–%46); açılan 8163, kapanan 6499 şube (510 / 406 dünya başına) | ✓ |
| 9b | Gölge savaşı düzenli | dünya-yıllarının ≥ %90'ında en az bir gölge savaşı eylemi (suikast, sabotaj, ihbar); hedef sıklık sonra ayarlanacak | en az bir eylemi olan dünya-yılı %100; dünyada 16,3 (11,3–20) / 100 gün; başarısız %56; öldürülen usta/lider 1410; türler: suikast 3322, sabotaj 1702, ihbar 1265 | ✓ |
| 9c | Tiplerin çöküş nedenleri farklı | dört hükümet tipinin en sık çöküş nedeni birbirinden farklı (başkent fethi, bölünme, veraset krizi, düello, darbe, aforoz); tipe özgü çöküş yolları Faz 1b/7 | Krallık: Pakt bağı ve aforoz (başkent fethi 10, bölünme 3, veraset krizi 4, darbe girişimi 3, Pakt bağı ve aforoz 12; 1257 devlet-yılı); Boylar: başkent fethi (başkent fethi 33, bölünme 10, reisin düelloda ölümü 4, darbe girişimi 7; 1443 devlet-yılı); Cumhuriyet: başkent fethi (başkent fethi 13, bölünme 2, darbe girişimi 8; 1167 devlet-yılı); Teokrasi: başkent fethi (başkent fethi 9, bölünme 6, Pakt bağı ve aforoz 6; 1036 devlet-yılı) | ✗ |
| 9d | Esaret sınırlı, Özgürlük Ağı etkin | köle payı her dünya-yılında nüfusun ≤ %5'i; Özgürlük Ağı dünyaların ≥ %75'inde köle kurtarır | köle payı medyan %0,2 (p90 %1,6, en çok %3,3); esarete düşen 4490 (av 32, savaş 1996, borç 1029, baskın 1433); hapis madeni 4319; Özgürlük Ağı 16/16 dünyada 3756 köle kurtardı; kaçan 698, azat 240; esir düşen kahraman 1079 | ✓ |
| 9e | Devriye profili tipe göre ayrışır | kervan başına durdurma, el koyma ve rüşvet: tipler arasında en yüksek / en düşük durdurma oranı ≥ 1,5, el koyma ve rüşvet oranları ≥ 2 | Krallık: 100 kervan-günde 7,23 durdurma, durdurmada el koyma %8,9, rüşvet %4,8, haraç/vergi %25, tutuklama 5, düello 0; Boylar: 100 kervan-günde 5,07 durdurma, durdurmada el koyma %2,4, rüşvet %8,5, haraç/vergi %54, tutuklama 0, düello 1493; Cumhuriyet: 100 kervan-günde 6,43 durdurma, durdurmada el koyma %3, rüşvet %13, haraç/vergi %66, tutuklama 0, düello 0; Teokrasi: 100 kervan-günde 9,36 durdurma, durdurmada el koyma %13, rüşvet %4,4, haraç/vergi %26, tutuklama 630, düello 0 (oranlar: durdurma ×1,85, el koyma ×5,34, rüşvet ×3) | ✓ |
| 9f | Aç haydutlar kıtlıkla ilişkili | dünya-yılı başına aç ya da ekmeksiz yerleşim payı ile haydut olan aç halk arasında korelasyon r ≥ 0,3 (en az 30 haydut) | r = 0,49 (960 dünya-yılı); haydut olan 787, aç haydut kampı 189, yiyecek verilip dağılan 23, açlıktan eriyen 104; aç ya da ekmeksiz yerleşim payı medyanı %1 | ✓ |

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
| Yaşayan yerleşim (günlük ortalama) | 74,9 (52,5–93,9) | 74,7 (52,8–94,1) |
| Yaşayan yerleşim: en az (ortalamaya oranı) | %97 (%94–%97) | %98 (%94–%99) |
| Yaşayan yerleşim: en çok (ortalamaya oranı) | %102 (%100–%103) | %101 (%100–%103) |
| Yaşayan yerleşim: değişim katsayısı | %1,4 (%0,9–%2,2) | %1 (%0,3–%2,3) |
| Büyük şehir (günlük ortalama) | 4,61 (4,07–6,4) | 4,65 (4,18–6,64) |
| El değiştirme (fetih + bölünme), dünyada / 100 gün | 1,98 (0,9–3,67) | 1,84 (0,44–3,34) |
| El değiştirme, yerleşim başına / 10 bin gün | 2,86 (1,36–4,71) | 2,65 (0,78–4,08) |
| Durum değişimi (kuruluş hariç), yerleşim başına / 100 gün | 0,53 (0,37–0,7) | 0,49 (0,28–0,62) |
| Durum değişimi, dünyada / 100 gün | 36,2 (22,4–52,2) | 32,4 (20,5–49,6) |
| Orta halka (Köy/Kasaba) kaydı: yerleşim başına kaç günde bir | 733 (521–959) | 776 (588–1310) |
| Büyük şehir el değiştirdi, dünyada / 100 gün | 0,13 (0,02–0,27) | 0,13 (0–0,25) |
| Büyük şehir el değiştirdi, şehir başına / 100 gün | 0,03 (0–0,04) | 0,02 (0–0,04) |
| Büyük şehre hücum, dünyada / 100 gün | 0,77 (0,23–1,17) | 0,53 (0,19–1,09) |
| Büyük şehir yağmalandı ama tutulmadı, dünyada / 100 gün | 0,4 (0,1–0,67) | 0,38 (0,13–0,81) |

Durum değişimi türleri (yerleşim başına 10 bin günde; bölünme dışında olayın kademesi olaydan önceki):

| Tür | bütün koşu (1–60. yıl) | ısınmadan sonra (21–60. yıl) |
|---|---|---|
| Fetih (`capture`) | 2,75 (1,32–4,54) | 2,58 (0,69–4,03) |
| Bölünme (`secede`) | 0,05 (0–0,2) | 0,04 (0–0,19) |
| Açlık başladı (`famine`) | 0,21 (0–2,32) | 0 (0–3,15) |
| Salgın başladı (`plague`) | 1,62 (1,02–2,15) | 1,66 (1,04–2,15) |
| Kuşatma (hücum) (`siege`) | 5,49 (1,95–7,38) | 5,06 (1,47–7,19) |
| Yakıldı / yandı (`burn`) | 25,3 (14,1–32,6) | 25,5 (12,8–31) |
| Kademe yükseldi (`tierUp`) | 7,24 (5,24–10,7) | 6,43 (4,12–8,96) |
| Kademe düştü (`tierDown`) | 8,26 (5,89–11,9) | 7,55 (4,7–11) |
| Terk (harabe) (`abandon`) | 0,52 (0,26–1,25) | 0,45 (0,18–1) |
| Harabeye yeniden yerleşim (`resettle`) | 0,06 (0–0,6) | 0,07 (0–0,3) |
| Kuruluş (durum değişimi sayılmaz) (`found`) | 0,57 (0,34–0,8) | 0,37 (0,2–0,68) |

Süreler (gün; bütün dünyalar havuzlanmış; savaş: ilandan ilişkiden düştüğü güne, koşu sonunda süren savaşlar hariç; kuşatma: hedefin önündeki ilk karargâhtan hücuma, karargâhsız hücum 1 gün; uyarı: büyük şehrin düşüşünden geriye):

| Süre | Pencere | n | p10 | medyan | p90 | ortalama | en çok |
|---|---|---|---|---|---|---|---|
| Savaş (hepsi) | bütün koşu (1–60. yıl) | 1326 | 10 | 13 | 40 | 21,1 | 78 |
| Savaş: sıradan | bütün koşu (1–60. yıl) | 696 | 10 | 13 | 40 | 21,5 | 78 |
| Savaş: haraç | bütün koşu (1–60. yıl) | 2 | 23,3 | 28,5 | 33,7 | 28,5 | 35 |
| Savaş: tarihî hak | bütün koşu (1–60. yıl) | 146 | 10 | 13 | 39 | 21 | 78 |
| Savaş: Kutsal Sefer | bütün koşu (1–60. yıl) | 141 | 10 | 28 | 60 | 29,9 | 78 |
| Savaş: savunma paktı | bütün koşu (1–60. yıl) | 179 | 10 | 10 | 20,6 | 13,6 | 57 |
| Savaş: müttefik çağrısı | bütün koşu (1–60. yıl) | 162 | 10 | 13 | 38 | 19,9 | 78 |
| Kuşatma: büyük şehir | bütün koşu (1–60. yıl) | 268 | 6 | 6 | 6 | 5,98 | 8 |
| Kuşatma: diğer yerleşimler | bütün koşu (1–60. yıl) | 1168 | 3 | 3 | 6 | 4,06 | 8 |
| Uyarı: kuşatmanın başı → büyük şehrin düşüşü | bütün koşu (1–60. yıl) | 50 | 5 | 5 | 5 | 4,98 | 7 |
| Uyarı: savaşın başı → büyük şehrin düşüşü | bütün koşu (1–60. yıl) | 50 | 10 | 14 | 35,1 | 19,7 | 57 |
| Savaş (hepsi) | ısınmadan sonra (21–60. yıl) | 775 | 10 | 15 | 45 | 22,9 | 78 |
| Savaş: sıradan | ısınmadan sonra (21–60. yıl) | 421 | 10 | 15 | 45 | 23,9 | 78 |
| Savaş: haraç | ısınmadan sonra (21–60. yıl) | 1 | 35 | 35 | 35 | 35 | 35 |
| Savaş: tarihî hak | ısınmadan sonra (21–60. yıl) | 74 | 10 | 15 | 39,4 | 19,9 | 77 |
| Savaş: Kutsal Sefer | ısınmadan sonra (21–60. yıl) | 88 | 10 | 30 | 65,6 | 32,1 | 78 |
| Savaş: savunma paktı | ısınmadan sonra (21–60. yıl) | 110 | 10 | 10 | 23,2 | 13,9 | 57 |
| Savaş: müttefik çağrısı | ısınmadan sonra (21–60. yıl) | 81 | 10 | 15 | 38 | 22,5 | 78 |
| Kuşatma: büyük şehir | ısınmadan sonra (21–60. yıl) | 168 | 6 | 6 | 6 | 5,99 | 8 |
| Kuşatma: diğer yerleşimler | ısınmadan sonra (21–60. yıl) | 732 | 3 | 3 | 6 | 4,22 | 8 |
| Uyarı: kuşatmanın başı → büyük şehrin düşüşü | ısınmadan sonra (21–60. yıl) | 30 | 5 | 5 | 5 | 5,07 | 7 |
| Uyarı: savaşın başı → büyük şehrin düşüşü | ısınmadan sonra (21–60. yıl) | 30 | 10,9 | 16,5 | 36,7 | 21,4 | 57 |

Koşu sonunda süren savaş: 10 (sürelere girmedi).

Kuşatma sonuçları (bütün koşu): büyük şehir: 268 hücum: 50 el değiştirdi, 163 yağmalandı (tutulmadı), 55 püskürtüldü; diğer: 1168 hücum: 798 el değiştirdi, 346 yağmalandı (tutulmadı), 24 püskürtüldü.

Büyük şehrin el değiştirmesi (bütün koşu, 50; ilk 60):

| Seed | Gün (yıl) | Şehir | Nasıl | Önceki → yeni sahip | Kuşatma → düşüş | Savaş → düşüş |
|---|---|---|---|---|---|---|
| 1 | 171 (5) | Yeşilkaya | fetih | Demirkanat Cumhuriyeti → Granitsunak Krallığı | 5 gün | 11 gün |
| 1 | 955 (24) | Ulukoru | fetih | Demirkanat Cumhuriyeti → Granitsunak Krallığı | 5 gün | 10 gün |
| 1 | 1137 (29) | Yıldızçayır | fetih | Meşekale Cumhuriyeti → Granitsunak Krallığı | 5 gün | 57 gün |
| 2 | 737 (19) | Ulukoru | fetih | Kurtoba Boyları → Közburç Krallığı | 5 gün | 12 gün |
| 3 | 670 (17) | Okyayı | fetih | Kanlıdiş Boyları → Bozkale Teokrasisi | 5 gün | 22 gün |
| 4 | 284 (8) | Kuzeybük | fetih | Toynakbaş Boyları → Keseli Cumhuriyeti | 5 gün | 21 gün |
| 4 | 1001 (26) | Kuzeybük | fetih | Keseli Cumhuriyeti → Toynakbaş Boyları | 5 gün | 18 gün |
| 6 | 273 (7) | Kızılçayır | fetih | Kızılyurt Boyları → Demirkanat Boyları | 5 gün | 10 gün |
| 6 | 1113 (28) | Gölköprü | fetih | Gölköprü Cumhuriyeti → Demirkanat Boyları | 5 gün | 35 gün |
| 6 | 1186 (30) | Karlıtepe | fetih | Demirkanat Boyları → Kızılyurt Boyları | 5 gün | 11 gün |
| 6 | 1334 (34) | Neşeliova | fetih | Kızılyurt Boyları → Alevgeçit Boyları | 7 gün | 14 gün |
| 6 | 1781 (45) | Alacayayla | fetih | Alevgeçit Boyları → Kızılyurt Boyları | 5 gün | 11 gün |
| 6 | 2007 (51) | Neşeliova | fetih | Kızılyurt Boyları → Alevgeçit Boyları | 5 gün | 24 gün |
| 7 | 108 (3) | Çamova | fetih | Kurtoba Boyları → Kemikçadır Boyları | 5 gün | 10 gün |
| 7 | 296 (8) | Közburç | fetih | Kemikçadır Boyları → Kurtoba Boyları | 5 gün | 16 gün |
| 7 | 373 (10) | Közburç | fetih | Kurtoba Boyları → Közburç Krallığı | 5 gün | 33 gün |
| 7 | 2014 (51) | Kafatepe | fetih | Kemikçadır Boyları → Közburç Krallığı | 5 gün | 14 gün |
| 8 | 848 (22) | Sarıhisar | fetih | Tatlıçayır Krallığı → Güneştacı Teokrasisi | 5 gün | 28 gün |
| 9 | 280 (7) | Karaköy | fetih | Güneştacı Krallığı → Toynakbaş Boyları | 5 gün | 12 gün |
| 9 | 864 (22) | Sarıhisar | fetih | Demirkanat Teokrasisi → Yeminköprü Boyları | 5 gün | 26 gün |
| 9 | 1449 (37) | Sarıhisar | fetih | Yeminköprü Boyları → Demirkanat Teokrasisi | 5 gün | 26 gün |
| 9 | 2273 (57) | Fıçıköy | fetih | Fıçıköy Cumhuriyeti → Güneştacı Krallığı | 5 gün | 43 gün |
| 10 | 101 (3) | Boynuztepe | fetih | Kızılyurt Boyları → Taşkandil Teokrasisi | 5 gün | 11 gün |
| 10 | 1181 (30) | Kafatepe | fetih | Kafatepe Boyları → Kızılyurt Boyları | 5 gün | 33 gün |
| 10 | 1891 (48) | Sisova | fetih | Sisova Boyları → Karamum Krallığı | 5 gün | 13 gün |
| 11 | 36 (1) | Yosunpınar | fetih | Gümüşdal Cumhuriyeti → Demirkanat Boyları | 5 gün | 16 gün |
| 11 | 180 (5) | Yosunpınar | fetih | Demirkanat Boyları → Karaörs Teokrasisi | 5 gün | 12 gün |
| 11 | 614 (16) | Yabanyurt | fetih | Demirkanat Boyları → Karaörs Teokrasisi | 5 gün | 14 gün |
| 11 | 687 (18) | Okyayı | fetih | Demirkanat Boyları → Karaörs Teokrasisi | 5 gün | 12 gün |
| 11 | 1551 (39) | Kurtkent | fetih | Karaörs Teokrasisi → Demirkanat Boyları | 5 gün | 13 gün |
| 11 | 1916 (48) | Pulkalkan | fetih | Şafaktepe Krallığı → Demirkanat Boyları | 5 gün | 36 gün |
| 11 | 2157 (54) | Pulkalkan | fetih | Şafaktepe Krallığı → Demirkanat Boyları | 5 gün | 24 gün |
| 11 | 2312 (58) | Pulkalkan | fetih | Şafaktepe Krallığı → Demirkanat Boyları | 5 gün | 24 gün |
| 11 | 2384 (60) | Pulkalkan | fetih | Demirkanat Boyları → Şafaktepe Krallığı | 5 gün | 11 gün |
| 12 | 985 (25) | Kurtkale | fetih | Gümüşdal Boyları → Pulzırh Teokrasisi | 5 gün | 12 gün |
| 12 | 1250 (32) | Kurtkale | fetih | Tatlıçayır Cumhuriyeti → Pulzırh Teokrasisi | 5 gün | 12 gün |
| 12 | 1886 (48) | Derinmihrap | fetih | Pulzırh Teokrasisi → Taşkandil Krallığı | 5 gün | 28 gün |
| 14 | 1412 (36) | Közsaray | fetih | Toynakbaş Boyları → Güneştacı Krallığı | 5 gün | 12 gün |
| 14 | 1507 (38) | Alazvadi | fetih | Toynakbaş Boyları → Güneştacı Krallığı | 5 gün | 9 gün |
| 15 | 408 (11) | Gölköprü | fetih | Kızılyurt Boyları → Balköprü Teokrasisi | 5 gün | 15 gün |
| 15 | 1008 (26) | Sarıhisar | fetih | Balköprü Teokrasisi → Kızılyurt Boyları | 5 gün | 15 gün |
| 15 | 1366 (35) | Sarıhisar | fetih | Balköprü Teokrasisi → Kızılyurt Boyları | 5 gün | 43 gün |
| 15 | 2384 (60) | Günyazı | fetih | Kızılyurt Boyları → Demirkanat Krallığı | 5 gün | 19 gün |
| 16 | 17 (1) | Kılıçyurt | fetih | Alazvadi Cumhuriyeti → Alevgeçit Krallığı | 5 gün | 14 gün |
| 16 | 113 (3) | Dinginpınar | fetih | Akburç Cumhuriyeti → Kurtoba Boyları | 5 gün | 55 gün |
| 16 | 285 (8) | Kocapınar | fetih | Kurtoba Boyları → Karaörs Teokrasisi | 2 gün | 10 gün |
| 16 | 500 (13) | Gölpınar | fetih | Alazvadi Cumhuriyeti → Alevgeçit Krallığı | 5 gün | 30 gün |
| 16 | 783 (20) | Gölpınar | fetih | Alevgeçit Krallığı → Tatlıçayır Krallığı | 5 gün | 10 gün |
| 16 | 891 (23) | Kızıltepe | fetih | Kurtoba Boyları → Karaörs Teokrasisi | 5 gün | 11 gün |
| 16 | 1137 (29) | Alazvadi | fetih | Alevgeçit Krallığı → Tatlıçayır Krallığı | 5 gün | 9 gün |

## v3: süre tablosu (Faz 1b-5)

Yol haritası v3'ün hedef süreleri (oyun günü; 1 yıl = 40 gün, 1 ay = 10, 1 hafta = 5). Bütün dünyalar havuzlanmış; 6e–6i ısınmadan sonra, 8a–8i bütün koşu. Ajan hızları `Core/Time.cs` (`Pace`).

| Süreç | Hedef | Ölçülen | Sonuç |
|---|---|---|---|
| Orta halkada kademe kayması (6e) | yerleşim başına 60–150 günde bir | yerleşim başına 776 (588–1310) günde bir | ✗ |
| Büyük şehir el değiştirmesi (6f) | dünyada 100 günde 2–4 | dünyada 0,13 (0–0,25) / 100 gün; şehir başına 0,02 / 100 gün; dünyada ortalama 4,65 büyük şehir; toplam 30 el değiştirme (0 bölünme). Ayrıştırma: dünyada 2,78 savaş / 100 gün, hedefi büyük şehir olan %19; büyük şehre 168 hücum, düşüşle bitenlerin payı %18 | ✗ |
| ↳ uyarı (6g) | 5–10 gün önceden | kuşatmanın başından 5 gün (5–5; n = 30); savaşın başından 16,5 gün (10,9–36,7) | ✓ |
| Savaş (6h) | 10–40 gün | 15 gün (10–45; n = 775) | ✓ |
| ↳ kuşatma (6i) | 2–6 gün | büyük şehir 6 gün (6–6; n = 168); diğer yerleşimler 3 gün (3–6; n = 732) | ✓ |
| Salgın (8a) | 5–10 gün | 6 gün (4–7; n = 434) | ✓ |
| Tepki inşaatı: yanan ev (8b) | 1–3 gün | 3 gün (2–5; n = 14640) | ✓ |
| Tepki inşaatı: sur (8c) | 5–10 gün | 9 gün (7–10; n = 322); palisat 10, taş sur 8 gün | ✓ |
| Büyük proje: kale, kule (8d) | 15–30 gün | 23 gün (20–23; n = 120); kale 20 (n = 9), kule 23 gün (n = 111) | ✓ |
| Temizlenen kamp → yeni köy (8e) | 10–20 gün | 11 gün (5,4–51; n = 125); temizlenen 3750 kampın 125 tanesine (%3,3) köy kuruldu, 20 gün içinde %2,1 | ✓ |
| Han kurulumu (8f) | 5–10 gün | 6 gün (5,4–7; n = 5); harabeyi yeniden kurma 3 gün (n = 4) | ✓ |
| Kahraman doğumu (8g) | han başına 10–20 günde bir | 19 günde bir (9226 doğum / 174881 han-günü); aynı handa iki doğum arası medyan 13 gün (2–43); bilgi: taverna başına 981 günde bir (1055 doğum) | ✓ |
| Efsaneye yükseliş (8h) | 100–300 gün | 246 gün (47,2–1105; n = 35) | ✓ |
| İlan ömrü (8i) | 10–20 gün (başarısız sefer ödülü +%25) | 15 gün (15–15; n = 4778); bütün ilanlar 9 gün (n = 10404): biten %33 (asılıştan 5 günde), süresi dolan %45; başarısız sefer ödülü %25 artırır (Heroes.QuestFailed) | ✓ |
| Yoldaş maaşı (8j) | haftalık (5 gün) | kural: medeniyetin kahramanları her 5. gün (Economy.PayHeroes), han personeli haftada bir (InnLife.PayWages); maaşı 3 hafta ödenmeyen kahraman ayrılır | ○ |

Proje süreleri, yapı türüne göre (gün; bütün koşu):

| Yapı | n | p10 | medyan | p90 |
|---|---|---|---|---|
| castle | 9 | 20 | 20 | 20 |
| extract | 11739 | 1 | 1 | 1 |
| guild | 45 | 1 | 1 | 1 |
| house | 9936 | 1 | 1 | 1 |
| hut | 26 | 1 | 1 | 1 |
| library | 200 | 1 | 1 | 1 |
| lighthouse | 111 | 23 | 23 | 23 |
| market | 344 | 1 | 1 | 1 |
| mint | 343 | 1 | 1 | 1 |
| palisade | 206 | 8 | 10 | 10 |
| ship | 1007 | 1 | 1 | 2 |
| shipyard | 120 | 1 | 1 | 3 |
| stonehouse | 239 | 1 | 1 | 1 |
| stonewall | 116 | 7 | 8 | 9 |
| tavern | 210 | 1 | 1 | 1 |
| temple | 208 | 1 | 2 | 2 |
| unique | 205 | 1 | 1 | 1 |
| upgrade | 1487 | 1 | 1 | 2 |
| workshop | 1940 | 1 | 1 | 1 |

## Devlet, inanç ve örgüt (Faz 1b-6)

claude/devlet-orgut-spec.md: 4–6 devlet (dört hükümet tipi her dünyada), yerleşimlerin ırk ve inanç dağılımı, 13 örgüt (merkezleri çekirdek şehirlerde). Dünya tarih öncesiyle (1600 gün) olgun başlar; örgütler tarih öncesinin sonunda kurulur, ölçüm o günden başlar. Örgüt kararları ~10 günde bir.

Devletler: dünya başına 5 (ilk yıl, medyan); devlet-yılı tipe göre: Krallık 1257, Boylar 1443, Cumhuriyet 1167, Teokrasi 1036. Yönetici değişimi 232 (veraset 35, veraset krizi 4, düello 26, boy meclisi 28, konsey oyu 102, tarikat 37); seçim 383; reise meydan okuma 13. Pakt'ın sızdığı yönetici 100, ortaya çıkan 92; Pakt suikastı 17; Tarikat'ın Kutsal Sefer çağrısı 558, başlayan sefer 57. Lobiyle yasa değişikliği 867 (kölelik yasası 0), darbe girişimi 18.

Örgütler (koşu sonu; dünyalar arası medyan, toplamlar bütün dünyalar):

| Örgüt | yaşıyor (dünya) | şube | üye | gizli şube payı | dağılma / yeniden kuruluş | açılan / kapanan şube | gölge savaşı | üye kahraman işi |
|---|---|---|---|---|---|---|---|---|
| Güneş Kilisesi | 16/16 | 59,5 | 400 | %0 | 0 / 0 | 1405 / 465 | 699 | 1404 |
| Güneş Tarikatı | 16/16 | 15 | 92 | %0 | 0 / 0 | 810 / 604 | 679 | 195 |
| Kara Pakt | 9/16 | 2 | 7 | %100 | 7 / 54 | 724 / 689 | 671 | 50 |
| Druid Çemberi | 16/16 | 30,5 | 123 | %0 | 0 / 0 | 794 / 283 | 0 | 1190 |
| Avcılar Locası | 16/16 | 28,5 | 82,5 | %0 | 0 / 0 | 719 / 214 | 0 | 0 |
| Hırsızlar Loncası | 16/16 | 10 | 43,5 | %65 | 0 / 0 | 1152 / 992 | 1139 | 365 |
| Büyücü Akademisi | 16/16 | 15 | 89 | %0 | 0 / 0 | 915 / 694 | 0 | 2074 |
| Paralı Bölükler | 16/16 | 15 | 74 | %0 | 0 / 0 | 925 / 703 | 0 | 1772 |
| Ozanlar Koleji | 16/16 | 42,5 | 156 | %0 | 0 / 0 | 886 / 216 | 0 | 0 |
| Tüccarlar Loncası | 16/16 | 56 | 186 | %0 | 0 / 0 | 1303 / 408 | 1177 | 0 |
| Harabe Kâşifleri | 16/16 | 31 | 95,5 | %0 | 0 / 0 | 789 / 279 | 0 | 301 |
| Köle Avcıları | 16/16 | 12,5 | 39 | %0 | 0 / 0 | 827 / 539 | 969 | 0 |
| Özgürlük Ağı | 16/16 | 10 | 24,5 | %2,2 | 0 / 1 | 551 / 413 | 955 | 0 |

Örgüt operasyonları (bütün dünyalar): artifact 3840, news 3840, caravan 3776, vigil 3725, pilgrims 3540, rite 3426, map 2560, escort 2157, smuggle 2063, track 1987, bounty 1853, rob 1777, hire 1683, ruin 1280, soul 1200, whisper 1059, sabotage 414, heal 234, infiltrate 100, unmask 92, patrol 89, famineOrder 64, assassinate 17.

Devriye (hükümet tipine göre; bütün dünyalar):

| Tip | maruz ajan-günü | 100 kervan-günde durdurma | durdurmada el koyma | rüşvet | haraç/vergi | düello | kahraman tutuklama | el konan değer | vergi/haraç altını |
|---|---|---|---|---|---|---|---|---|---|
| Krallık | 117169 | 7,23 | %8,9 | %4,8 | %25 | 0 | 5 | 2806 | 1338 |
| Boylar | 75959 | 5,07 | %2,4 | %8,5 | %54 | 1493 | 0 | 417 | 2205 |
| Cumhuriyet | 52780 | 6,43 | %3 | %13 | %66 | 0 | 0 | 99,4 | 549 |
| Teokrasi | 114390 | 9,36 | %13 | %4,4 | %26 | 0 | 630 | 5241 | 971 |

Çöküş nedenleri (hükümet tipine göre; bütün dünyalar; 100 devlet-yılı başına):

| Tip | devlet-yılı | başkent fethi | bölünme | veraset krizi | reisin düelloda ölümü | darbe girişimi | Pakt bağı ve aforoz |
|---|---|---|---|---|---|---|---|
| Krallık | 1257 | 10 (0,8) | 3 (0,24) | 4 (0,32) | 0 (0) | 3 (0,24) | 12 (0,95) |
| Boylar | 1443 | 33 (2,29) | 10 (0,69) | 0 (0) | 4 (0,28) | 7 (0,49) | 0 (0) |
| Cumhuriyet | 1167 | 13 (1,11) | 2 (0,17) | 0 (0) | 0 (0) | 8 (0,69) | 0 (0) |
| Teokrasi | 1036 | 9 (0,87) | 6 (0,58) | 0 (0) | 0 (0) | 0 (0) | 6 (0,58) |

## Eski analizdeki sorunlar

Eski analiz: TS v0.23, 12 seed × 30 yıl ve 3 seed × 60 yıl (Proje: `analiz-5-ajan-oneriler.md`). "Sürüyor mu" kaba bir eşiktir: araştırma ağacı Faz 1b-3'te kaldırıldı; 30. yılda tam 5 kara yerleşimli medeniyet ≥ %50; kamp (30. yıl) < 0,75 × en yüksek yıl; altın (30. yıl) ≥ 10 × altın (1. yıl); boştaki iş gücü (30. yıl) ≥ %30; büyük olay (30. yıl) ≤ 0,6 × en yüksek yıl; 25. yıldan sonra doğanların ≥ %50'si Sv5+; hiç başkent kaybı yok.

| Bulgu | Eski analiz | Bu ölçüm | Sürüyor mu? |
|---|---|---|---|
| Araştırma ağacı erken bitiyor | ~19. yılda bitiyor; 30. yılda medeniyetlerin %98'i bitirmiş | ağaç ve çağlar kaldırıldı (Faz 1b-3); başlangıç medeniyetlerinin ilk kasabası medyan –. yılda (0/0), ilk şehri –. yılda (0/0); 30. yılda başkent kademesi ortalaması 2,78 | hayır |
| Medeniyetler 5 yerleşimde takılıyor | 98 medeniyetin 74'ü (%76) tam 5 yerleşimde | tam 5 kara yerleşimli medeniyet payı 30. yılda %1,2, 60. yılda %0 (denizaşırı koloniler dâhil 30. yılda tam 5: %2,4, 5+: %82); medeniyet başına 14,5 yerleşim (30. yıl) | hayır |
| Kamp sayısı düşüyor | 6,8'den 3,5'e iniyor | 9 (1. yıl) → en yüksek 10 (3. yıl) → 9 (30. yıl) → 9 (60. yıl); yıl sonu, yıllık dünya medyanı | hayır |
| Altın birikiyor | altın medyanı 78'den 6.503'e çıkıyor | 277 (1. yıl) → 453 (30. yıl) → 519 (60. yıl) | hayır |
| İş gücü boşta | iş gücünün %43'ü boşta | 30. yılda %16, 60. yılda %16 (işe yerleşemeyen `zanaatçı` / bütün iş gücü, askerler dâhil; yıl içi ortalama) | hayır |
| Büyük olaylar seyreliyor | yıllık büyük olay 59'dan 28'e düşüyor | en yüksek 144 (54. yıl) → 92,5 (30. yıl) → 124 (60. yıl), yıllık dünya medyanı | hayır |
| Doğuş seviyesi şişiyor | 24. yıldan sonra herkes Sv5 doğuyor; efsane mekaniği ölü | 25–60. yıllarda Sv5+ doğanların payı %0; on yıllık doğuş seviyesi ortalaması 1,56 · 1,58 · 1,56 · 1,53 · 1,57 · 1,56 | hayır |
| Başkent düşmüyor | başkent fethedilemiyor (agents.ts:673) | 16 dünyada 41 başkent kaybı, 24 yok olma; 869 yerleşim fethi | hayır |

## On yıllık özet

Hücre: dünyalar arası medyan (p10–p90). Her dünyada on yılın yıllık değerlerinin ortalaması alınır: akış ölçülerinde yıllık ortalama, stok ölçülerinde yıl sonu değerlerinin ortalaması. Yüzdeler 0–1 paylardır.

| Ölçü | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| **Medeniyet** | | | | | | |
| Yaşayan medeniyet | 5,05 (3,6–6,75) | 5 (3,9–7) | 5 (4–6,5) | 5 (4–6,5) | 5 (4–6,5) | 5 (3,95–6,5) |
| Yeni medeniyet (yeniden doğan) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,05) | 0 (0–0,05) | 0 (0–0,1) | 0 (0–0,1) |
| Yok olan medeniyet | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,15) |
| Başkent kaybı (medeniyet yaşarken) | 0,1 (0–0,1) | 0,05 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 | 0 (0–0,1) |
| Çöküş (yok olma + başkent kaybı) | 0,1 (0–0,2) | 0,1 (0–0,2) | 0,1 (0–0,1) | 0,05 (0–0,15) | 0 (0–0,1) | 0 (0–0,35) |
| Yaşayan yerleşim | 74,7 (52–93,1) | 74,8 (52,2–94,3) | 74,7 (52–94,3) | 74,9 (52,1–94,5) | 74,5 (53,6–93,6) | 74,9 (53,3–94,2) |
| Medeniyet başına yerleşim | 14,6 (13,5–15,2) | 14,7 (12,6–15,5) | 14,4 (13,2–15,8) | 14,6 (13,2–16,5) | 14,9 (13,2–16,5) | 15 (13,1–16,6) |
| 5+ kara yerleşimli medeniyet payı | %97 (%81–%100) | %85 (%66–%100) | %80 (%65–%100) | %77 (%61–%98) | %75 (%59–%98) | %74 (%54–%100) |
| Kurulan yerleşim | 0,4 (0,2–0,65) | 0,2 (0,05–0,5) | 0,1 (0–0,45) | 0,2 (0,05–0,5) | 0,1 (0–0,25) | 0 (0–0,25) |
| Fethedilen yerleşim | 1,05 (0,5–2,05) | 1,2 (0,45–2,05) | 0,75 (0,2–1,5) | 0,75 (0,15–1,65) | 0,75 (0,1–1,25) | 0,65 (0–1,25) |
| Terk edilen yerleşim | 0,2 (0,05–0,55) | 0,2 (0–0,4) | 0,15 (0–0,4) | 0,2 (0–0,35) | 0 (0–0,3) | 0,1 (0–0,3) |
| Toplam nüfus | 2311 (1719–3083) | 2428 (1793–3260) | 2465 (1838–3137) | 2527 (1878–3226) | 2585 (1811–3226) | 2529 (1770–3151) |
| Altın medyanı (medeniyetler) | 334 (140–561) | 430 (180–587) | 449 (309–578) | 489 (277–701) | 489 (327–688) | 471 (354–769) |
| Boştaki iş gücü payı | %15 (%12–%25) | %17 (%12–%24) | %15 (%11–%24) | %18 (%12–%22) | %17 (%11–%23) | %17 (%10–%22) |
| Bölünme (ayrılıp kurulan medeniyet) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,05) | 0 (0–0,05) | 0 (0–0,1) | 0 (0–0,1) |
| En büyük medeniyetin yerleşimi | 19,7 (17,5–22,2) | 21,4 (17,6–25,1) | 23,6 (17,3–27,1) | 23,9 (19,4–30,2) | 23,8 (21,1–29,7) | 24,5 (20,8–30,4) |
| **Olaylar** | | | | | | |
| Olay | 388 (261–771) | 443 (292–824) | 459 (290–834) | 428 (298–842) | 455 (275–934) | 501 (282–855) |
| Büyük olay | 90,4 (56,9–231) | 93,7 (59,1–245) | 93,8 (56,6–256) | 83,6 (50,4–220) | 104 (51,7–254) | 113 (52,9–223) |
| **Savaş** | | | | | | |
| Muharebe | 13,3 (8,65–18,3) | 12,6 (8,65–18,9) | 12 (8,4–17,6) | 11,4 (7,55–16,4) | 11,6 (7,35–17,7) | 11,6 (6,75–16,9) |
| Başlayan savaş | 1,7 (0,95–2,85) | 1,55 (0,8–2,95) | 1,25 (0,55–2,5) | 1,1 (0,5–2,3) | 1,1 (0,35–1,7) | 1 (0,15–1,75) |
| Süren savaş (yıl sonu) | 0,9 (0,3–1,55) | 0,7 (0,5–1,3) | 0,9 (0,4–1,35) | 0,65 (0,25–1,3) | 0,65 (0,2–1,35) | 0,6 (0,1–1,25) |
| Yıl içinde süren savaş | 2,5 (1,35–4,4) | 2,3 (1,55–4,4) | 2,05 (0,95–3,85) | 1,85 (0,85–3,25) | 1,65 (0,55–3,3) | 1,6 (0,45–2,95) |
| Yağma akını (medeniyet) | 1,45 (0,3–3,7) | 1,6 (0,1–3,75) | 1,1 (0,45–3,35) | 1,25 (0,35–2,85) | 1,25 (0–2,45) | 0,7 (0–2,1) |
| Tarihî hak savaşı | 0,2 (0–0,45) | 0,1 (0–0,5) | 0,05 (0–0,3) | 0,1 (0–0,35) | 0,1 (0–0,25) | 0 (0–0,35) |
| Pakt gereği savaş | 0,1 (0–0,45) | 0,2 (0–0,7) | 0 (0–0,55) | 0,15 (0–0,45) | 0 (0–0,4) | 0 (0–0,5) |
| Kutsal Sefer çağrısı | 0,05 (0–0,2) | 0 (0–0,2) | 0,05 (0–0,2) | 0,05 (0–0,15) | 0 (0–0,1) | 0 (0–0,2) |
| İhanet (pakt çiğnendi) | 0 (0–0,1) | 0 (0–0,05) | 0 (0–0,05) | 0 (0–0,15) | 0 (0–0,1) | 0 (0–0,1) |
| Savunma paktı (yıl sonu) | 1 (0–1,65) | 1 (0–2) | 1 (0–2) | 1 (0–2) | 1 (0–2) | 0,95 (0,4–1,6) |
| **Canavarlar** | | | | | | |
| Yaşayan kamp (yıl sonu) | 9,3 (8,5–10,5) | 9,2 (8,6–10,2) | 9,5 (8,5–10,8) | 9,35 (8,2–11,2) | 9,2 (7,95–10,9) | 8,95 (7,9–10,1) |
| Yaşayan kamp (yıl ort.) | 9,34 (8,55–10,5) | 9,24 (8,62–10,1) | 9,44 (8,57–10,7) | 9,22 (8,24–11,2) | 9,11 (8,16–11) | 8,99 (8,05–9,94) |
| Doğan kamp | 3,85 (0,95–9,45) | 3,9 (0,5–9,2) | 2,7 (0,5–9,7) | 2,8 (0,45–9,25) | 3,5 (0,35–9,05) | 4,15 (0,75–8,55) |
| Temizlenen kamp | 3,85 (0,9–9,35) | 4 (0,55–9,3) | 2,65 (0,45–9,7) | 2,9 (0,4–9,1) | 3,4 (0,4–9,25) | 4,2 (0,7–8,55) |
| Canavar baskını | 4,35 (2,85–5,95) | 4,4 (2,2–6,15) | 4,15 (2,45–6,25) | 4,15 (2,25–5,3) | 4,05 (2,6–4,95) | 3,4 (1,5–5) |
| Yaşayan trol ini (yıl sonu) | 2,15 (1,45–2,9) | 2,55 (1,75–2,95) | 2,55 (1,7–3) | 2,75 (1,9–3) | 2,7 (2,25–3) | 2,9 (2,4–3) |
| Yaşayan ejderha (yıl sonu) | 1 (0–1) | 1 (0–1) | 0,9 (0–1) | 0,15 (0–1) | 0 (0–1) | 0 (0–1) |
| Ejderha akını | 0,95 (0–1,6) | 1,05 (0–1,6) | 0,85 (0–1,65) | 0,2 (0–1,7) | 0 (0–1,55) | 0 (0–1,05) |
| Kriz (anlatıcı) | 0,25 (0–0,7) | 0,3 (0,05–0,65) | 0,3 (0,05–0,55) | 0,5 (0,05–0,75) | 0,55 (0,2–0,75) | 0,6 (0,3–0,7) |
| Rahatlama dönemi (anlatıcı) | 0,1 (0–0,2) | 0,2 (0–0,25) | 0,1 (0–0,3) | 0,1 (0–0,2) | 0,1 (0–0,2) | 0,1 (0–0,25) |
| **Kahramanlar** | | | | | | |
| Doğan kahraman | 10 (5,9–15,3) | 10,8 (7,4–13,9) | 11,3 (6,75–14,2) | 10,9 (6,35–15,2) | 9,25 (5,85–16) | 10,5 (6,8–14,6) |
| Ölen kahraman | 4,1 (2,65–6,25) | 4,05 (2,65–6,3) | 4,6 (2,3–7,65) | 4,2 (2,55–5,4) | 3,8 (2,5–6,05) | 3,9 (1,75–6,75) |
| Emekli olan kahraman | 1,2 (0,6–1,7) | 1,1 (0,5–1,85) | 1,25 (0,8–2,2) | 1,1 (0,5–3) | 1,45 (0,75–2,55) | 1,3 (0,95–3,4) |
| Diyarı terk eden kahraman | 4,5 (2,65–7,45) | 3,95 (2,65–7,25) | 4,3 (3,1–8,1) | 5,05 (2,7–7,9) | 4,25 (2,55–7,85) | 3,9 (2,85–8,5) |
| Ölümden dönen kahraman | 0 | 0 | 0 | 0 | 0 | 0 |
| Efsane olan kahraman | 0 (0–0,2) | 0 (0–0,15) | 0 (0–0,15) | 0 (0–0,15) | 0 (0–0,1) | 0,05 (0–0,15) |
| Yaşayan kahraman (yıl sonu) | 94,8 (64,8–163) | 109 (72,1–180) | 116 (80–190) | 119 (86,4–199) | 122 (82,5–196) | 137 (85,1–193) |
| Doğuş seviyesi (ort.) | 1,53 (1,42–1,69) | 1,56 (1,45–1,7) | 1,56 (1,47–1,68) | 1,55 (1,41–1,64) | 1,59 (1,47–1,64) | 1,56 (1,41–1,66) |
| Ölüm seviyesi (ort.) | 3,3 (2,76–4,34) | 3,96 (3,34–5) | 4,19 (3,52–5,11) | 4,11 (3,51–5,27) | 4,3 (3,42–5,09) | 4,1 (3,75–5,51) |
| Yaşayan kahraman seviyesi (ort.) | 3,5 (2,99–4,77) | 3,51 (2,97–4,93) | 3,5 (3,16–4,91) | 3,54 (3,02–4,92) | 3,76 (2,95–4,76) | 3,82 (2,95–4,75) |
| En yüksek seviye (şimdiye dek) | 10 (9,1–10) | 10 | 10 | 10 | 10 | 10 |
| **Han ve ticaret** | | | | | | |
| Ayakta han | 4 (3–6,4) | 4,35 (3–6,45) | 4,95 (3–6,6) | 4,75 (3–6,7) | 4,3 (3–7) | 4,45 (3–6,55) |
| Asılan ilan | 8,1 (4,6–17,9) | 8,05 (3,25–19,2) | 6,4 (3,1–20) | 6,6 (4,15–15,6) | 6,6 (3,55–17,7) | 7,65 (2,2–13,7) |
| Biten ilan | 3,15 (1,15–8,45) | 2,7 (0,55–9,45) | 2,1 (0,25–8,8) | 2,1 (0,35–8,5) | 3,2 (0,4–10,2) | 2,3 (0,45–7) |
| Ticaret seferi (kervan) | 56,2 (22,2–95,6) | 61,6 (23,3–110) | 61,1 (23,3–114) | 69,1 (25,2–113) | 66,3 (23,6–114) | 60,2 (21,4–122) |
| İkmal seferi | 31,4 (19,2–40,9) | 31,6 (18,2–45,3) | 32,6 (16,3–46,9) | 34 (14,3–59) | 35,2 (18,8–66,9) | 34,2 (21,4–66,8) |
| **Altın ve ambar** | | | | | | |
| Altın p90 (medeniyetler) | 801 (523–1066) | 895 (626–1382) | 1097 (784–1427) | 1145 (826–1400) | 1150 (769–1679) | 1226 (824–1865) |
| Bakım gideri (altın; asker, kahraman, L2–L3) | 2282 (1649–2997) | 2275 (1760–2964) | 2339 (1649–3015) | 2224 (1567–3011) | 2243 (1641–2805) | 2275 (1604–2899) |
| Kamu işlerine (imar) harcanan altın | 958 (370–1943) | 1445 (459–2505) | 1634 (739–2682) | 1690 (814–2906) | 1676 (782–3786) | 2211 (878–4480) |
| Ambarla beslenen amele tayını (gıda) | 2332 (997–3745) | 1458 (494–3300) | 1518 (279–2781) | 695 (51,9–1892) | 606 (126–1630) | 448 (86,4–1562) |
| Kamu işlerindeki (amele) iş gücü payı | %15 (%13–%18) | %16 (%11–%18) | %15 (%11–%17) | %15 (%12–%17) | %14 (%12–%17) | %15 (%10–%16) |
| İmar ortalaması (köy+, 0–100) | 11,9 (9,44–16,2) | 12,5 (9,15–19,6) | 12,8 (8,61–20,3) | 13,5 (10,4–19,2) | 13,5 (8,01–21,5) | 15,6 (8,58–21,6) |
| Canavar baskınında yitirilen altın | 4,55 (2,75–12,3) | 3,4 (0,75–14,3) | 7,05 (0,35–12,6) | 5,05 (2,05–15,3) | 6,1 (0,85–18,7) | 6,6 (1,35–22) |
| Ejderhaya giden altın (haraç + akın) | 297 (0–485) | 269 (0–519) | 217 (0–445) | 23,5 (0–628) | 0 (0–545) | 0 (0–530) |
| Hazinesi boş medeniyet payı | %0 | %0 | %0 | %0 | %0 | %0 |
| Kent tüketiminde yokluk payı (köy+; ekmek, bira ya da alet) | %22 (%18–%31) | %26 (%21–%30) | %24 (%21–%31) | %23 (%20–%34) | %24 (%18–%29) | %22 (%12–%28) |
| Ekmek ya da bira yokluğu payı (köy+) | %1,9 (%0–%4,8) | %2,5 (%0,1–%10) | %3,7 (%0,1–%10) | %3,8 (%0,3–%9,4) | %4,2 (%0,8–%8,3) | %3,1 (%0,8–%5,2) |
| Kıtlık (büyük olay) | 0 (0–0,05) | 0 | 0 (0–0,05) | 0 (0–0,2) | 0 (0–0,05) | 0 (0–0,15) |
| Açlıktan ölen | 0 (0–0,7) | 0 | 0 (0–0,45) | 0 (0–1,75) | 0 (0–0,7) | 0 (0–0,15) |
| Kıtlık yardımı (sevkiyat) | 0 (0–0,05) | 0 | 0 (0–0,05) | 0 (0–0,5) | 0 (0–0,15) | 0 (0–0,25) |
| Kıtlıkta yüz çeviren | 0 (0–0,15) | 0 | 0 (0–0,1) | 0 (0–0,4) | 0 | 0 (0–0,25) |
| Kıtlık akını | 0 | 0 | 0 | 0 (0–0,35) | 0 | 0 (0–0,05) |
| Ambarın yettiği gün (medeniyet medyanı) | 49,4 (36,9–64,8) | 48 (31,3–57,5) | 46,2 (30,2–52,3) | 41,6 (28,6–50,8) | 41,5 (29,2–52,1) | 41,5 (21,7–56) |
| **Yerleşim kademesi** | | | | | | |
| Ortalama yerleşim kademesi | 1,05 (0,92–1,13) | 1,1 (0,98–1,18) | 1,09 (0,98–1,23) | 1,13 (0,97–1,19) | 1,1 (0,96–1,18) | 1,09 (0,92–1,19) |
| Köy+ yerleşim | 55,9 (39,4–71,4) | 60,5 (39,3–75,1) | 61,6 (42–74,6) | 59,7 (39,7–74,7) | 60,3 (39,7–73,9) | 59 (40,9–77,4) |
| Kasaba+ yerleşim | 14,7 (10,8–20,4) | 17,1 (12,2–21,6) | 16,1 (11,4–20) | 15,4 (9,45–20,6) | 14,2 (9,5–20,4) | 14,1 (5,85–19,6) |
| Şehir | 4,8 (3,4–5,85) | 5 (3,9–6,4) | 5,5 (3,8–6,45) | 4,95 (3,7–7,25) | 4,8 (3,75–6,55) | 5,25 (3,2–6,3) |
| Ortalama başkent kademesi | 2,73 (2,47–2,82) | 2,72 (2,51–2,9) | 2,73 (2,53–2,9) | 2,75 (2,59–2,94) | 2,77 (2,53–2,95) | 2,68 (2,29–3) |
| Kademe değişimi (yerleşim, yıl içinde) | 7,05 (3,35–9,95) | 6,1 (2,9–7,7) | 5,45 (2,2–7,85) | 4,2 (2,2–6,75) | 3,5 (2,2–5,8) | 3,5 (1,25–4,8) |
| **Deniz** | | | | | | |
| Liman (tersane) | 15,4 (9–20,3) | 16,7 (10,2–21,2) | 17,2 (11,5–21,4) | 18 (12,9–22,6) | 19,3 (13,5–25,1) | 19,8 (13,6–24,9) |
| Gemi (koga/tekne) | 35,8 (23,1–50,6) | 35,9 (25,6–57) | 37,4 (27,3–52,1) | 38,7 (31,2–53,7) | 39,4 (32,1–56) | 43,5 (32,2–56) |
| Kadırga | 18,9 (12,7–22) | 18,7 (13,5–22,1) | 19,3 (13,5–22) | 18,6 (13,7–21,5) | 18,6 (14,1–21,3) | 19 (12,6–21,3) |
| Denizaşırı yerleşim | 7 (4,7–11,4) | 7 (4,95–11,5) | 7 (5–11,5) | 7,1 (5–11,9) | 7,5 (4,9–12) | 7,5 (4,5–12) |
| Deniz ticaret yolu (yıl sonu) | 11,5 (7,55–21,5) | 11,3 (8,4–25,5) | 12 (8,5–26,2) | 12,4 (8,25–26,6) | 12,9 (7,85–28) | 14,4 (7,5–28,5) |
| Deniz seferi (ticaret) | 14 (8,8–29,5) | 15,5 (10,9–31,2) | 16,4 (10,7–32) | 18,8 (12–31,6) | 18 (12,4–35,3) | 19,1 (11,4–36,8) |
| **v3: durum değişimi** | | | | | | |
| Yaşayan yerleşim (yıl ort.) | 74,4 (51,9–93) | 74,8 (52,2–94,3) | 74,7 (51,9–94,3) | 74,9 (52–94,4) | 74,5 (53,6–93,6) | 74,9 (53,4–94,1) |
| Büyük şehir (Şehir kademesi, yıl ort.) | 4,74 (3,5–5,87) | 5,06 (3,87–6,36) | 5,59 (3,88–6,31) | 5,01 (3,58–7,19) | 4,88 (3,98–6,73) | 5,23 (3,28–6,31) |
| El değiştiren yerleşim (fetih + bölünme) | 1,05 (0,5–2,05) | 1,2 (0,45–2,05) | 0,75 (0,2–1,5) | 0,75 (0,15–1,65) | 0,75 (0,1–1,25) | 0,65 (0–1,25) |
| El değiştiren büyük şehir | 0,05 (0–0,25) | 0 (0–0,15) | 0,1 (0–0,2) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) |
| Büyük şehre hücum (kuşatma muharebesi) | 0,3 (0–0,6) | 0,3 (0,1–0,6) | 0,35 (0,05–0,75) | 0,25 (0–0,55) | 0,15 (0–0,35) | 0,15 (0–0,5) |
| Büyük şehir yağmalandı, tutulmadı | 0,15 (0–0,4) | 0,15 (0,05–0,35) | 0,2 (0–0,45) | 0,1 (0–0,4) | 0 (0–0,25) | 0,1 (0–0,4) |
| Yerleşim durum değişimi (kuruluş hariç hepsi) | 19,4 (11,2–28,7) | 16,9 (9,4–24,5) | 16,1 (7,7–26,6) | 14,3 (7,7–20,6) | 11,1 (8,9–18,8) | 12,3 (6,95–19,1) |
| Orta halka kaydı (Köy/Kasaba: kademe değişimi ya da terk) | 4,05 (2,2–6,3) | 3,75 (1,85–4,95) | 3,4 (1,2–4,85) | 2,95 (1,4–4,55) | 2,25 (1,2–3,85) | 2,15 (0,65–3,25) |
| Açlık başlayan yerleşim | 0 (0–0,95) | 0 (0–0,25) | 0 (0–0,35) | 0 (0–1,15) | 0 (0–1,5) | 0 (0–0,75) |
| Salgın başlayan yerleşim | 0,3 (0,1–0,7) | 0,45 (0,2–0,85) | 0,3 (0,1–0,65) | 0,5 (0,2–0,85) | 0,35 (0,2–0,9) | 0,5 (0,2–1,05) |
| Yakılan/yanan yerleşim | 6,6 (5,5–11,6) | 6,9 (4,3–11,5) | 7,05 (3,25–14,1) | 5,6 (3,45–9,8) | 6,35 (4,15–9,55) | 5,55 (3,2–10,1) |
| Harabeye yeniden yerleşim | 0 (0–0,1) | 0 (0–0,15) | 0 (0–0,2) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) |
| **Devlet, inanç ve örgüt** | | | | | | |
| Yaşayan örgüt (yıl sonu) | 12,5 (12,1–13) | 12,6 (12,1–13) | 12,8 (12–13) | 12,7 (12,2–13) | 12,8 (12,2–13) | 12,8 (12,2–13) |
| Örgüt şubesi (yıl sonu) | 278 (213–344) | 316 (242–385) | 330 (260–393) | 328 (250–410) | 324 (249–422) | 318 (251–437) |
| Gizli şube (yıl sonu) | 10,2 (8,25–13,9) | 9,95 (5,75–12,5) | 8,6 (5,1–14,8) | 9,65 (6,9–13,7) | 10,1 (5,7–12,8) | 9,9 (4,95–14) |
| Örgüt üyesi (yıl sonu) | 1102 (831–1399) | 1277 (998–1595) | 1334 (1070–1660) | 1371 (1110–1736) | 1389 (1133–1779) | 1421 (1139–1825) |
| Açılan şube | 13,7 (10,1–16,6) | 8,75 (6,7–13) | 7,6 (4,6–10,2) | 7,45 (3,85–10,6) | 6,15 (3,65–10,3) | 5,2 (3,55–9,55) |
| Kapanan şube | 6,7 (4,65–10,5) | 7,3 (4,35–9,15) | 6,75 (4,2–10,5) | 6,85 (3,55–12,4) | 6,2 (3,25–10,2) | 5,5 (3,25–8,25) |
| Dağılan örgüt | 0,1 (0–0,2) | 0,1 (0–0,2) | 0 (0–0,1) | 0,05 (0–0,15) | 0 (0–0,15) | 0 (0–0,15) |
| Yeniden kurulan örgüt | 0 (0–0,1) | 0,1 (0–0,2) | 0,05 (0–0,2) | 0 (0–0,15) | 0,05 (0–0,15) | 0 (0–0,1) |
| Gölge savaşı eylemi | 7,15 (5,2–8,4) | 6,2 (4,7–8,55) | 6,15 (3,5–8,1) | 6,25 (3,8–8,15) | 6,9 (4,1–9,35) | 6,95 (4,2–8,75) |
| Gölge savaşında öldürülen usta ya da lider | 1,35 (1–2,25) | 1,3 (0,85–2,2) | 1,2 (0,6–1,9) | 1,55 (0,85–2,05) | 1,45 (0,8–2,25) | 1,55 (0,9–2,4) |
| Gizli şubeye baskın | 0,7 (0,55–1,1) | 0,7 (0,5–1,3) | 0,7 (0,4–1,05) | 0,9 (0,7–1,1) | 0,65 (0,5–1,3) | 1 (0,45–1,15) |
| Lobiyle yasa değişikliği | 1,15 (0,65–1,85) | 1 (0,35–2,4) | 0,65 (0,3–2) | 0,7 (0–1,5) | 0,8 (0,05–1,3) | 0,45 (0–1,1) |
| Darbe girişimi | 0 (0–0,05) | 0 (0–0,15) | 0 | 0 (0–0,05) | 0 | 0 (0–0,15) |
| Örgüt ilanı (Avcılar) | 1,25 (0,5–3,35) | 1,4 (0,55–3,4) | 1,95 (0,65–4) | 2,1 (0,4–4) | 1,9 (0,45–4) | 1,4 (0,2–3,95) |
| Örgüt üyesi kahraman payı (yıl sonu) | %100 (%99–%100) | %99 (%99–%100) | %99 (%99–%100) | %100 (%99–%100) | %100 (%99–%100) | %100 (%99–%100) |
| Yönetici değişimi | 0,2 (0,1–0,35) | 0,2 (0,1–0,4) | 0,25 (0,1–0,45) | 0,3 (0,1–0,45) | 0,2 (0,15–0,5) | 0,2 (0,05–0,35) |
| Veraset krizi | 0 | 0 | 0 (0–0,05) | 0 | 0 (0–0,05) | 0 |
| Meşruiyet ortalaması (yıl sonu) | 67,5 (65–68,3) | 67 (63,8–69,5) | 67,4 (63,4–69,3) | 66,5 (64,5–71,1) | 67,2 (64,2–69,1) | 67,4 (64,1–70,1) |
| Pakt'a bağlı yönetici (yıl sonu) | 0,1 (0–0,9) | 0 (0–0,3) | 0,1 (0–0,2) | 0,1 (0–0,45) | 0,05 (0–0,25) | 0 (0–0,05) |
| Köle (yıl sonu) | 11 (1,2–28,5) | 7,95 (0–16,8) | 8 (0,4–19,3) | 5,3 (1,1–21,7) | 5,8 (0,75–26,2) | 6,4 (0,95–37,6) |
| Köle payı (nüfusun, yıl sonu) | %0,4 (%0,1–%1,5) | %0,3 (%0–%0,9) | %0,3 (%0–%0,9) | %0,2 (%0–%1,1) | %0,2 (%0–%1,2) | %0,3 (%0–%1,9) |
| Hapis madeninde mahkûm (yıl sonu) | 0,9 (0,1–1,85) | 1,05 (0,3–2,2) | 0,9 (0,4–1,85) | 1,2 (0,4–1,8) | 0,8 (0,4–2,9) | 1 (0,6–6,6) |
| Esarete düşen | 4,35 (0,9–9,05) | 5,15 (0–9,8) | 3,6 (0,35–9,15) | 4 (0,3–8,4) | 3,7 (0,2–10) | 2,7 (0–10,6) |
| Kurtulan köle (Özgürlük Ağı, kaçış, azat) | 4,8 (1,5–12,6) | 5,55 (0–10,1) | 3,15 (0,4–8,9) | 3,65 (0,2–8,9) | 3,3 (0,2–10,4) | 2,95 (0,05–9,65) |
| Özgürlük Ağı'nın kurtardığı köle | 4,55 (1,35–10,4) | 4,9 (0–8,1) | 2,75 (0,35–7,6) | 3,15 (0,1–7,55) | 2,95 (0,05–7,75) | 2,4 (0–6,65) |
| Esir kahraman (yıl sonu) | 0,6 (0,15–1,35) | 0,75 (0,15–1,3) | 0,4 (0,1–1,7) | 0,7 (0,1–2,3) | 0,4 (0,25–1,7) | 0,55 (0–2,2) |
| Aç haydut kampı (yıl sonu) | 0 (0–0,05) | 0 (0–0,1) | 0 (0–0,25) | 0 (0–0,4) | 0 (0–0,2) | 0 (0–0,55) |
| Haydut olan aç halk | 0 (0–1,05) | 0 (0–2,1) | 0,3 (0–3,7) | 0,6 (0–3,1) | 0,45 (0–1,8) | 0,4 (0–1,9) |
| Aç ya da ekmeksiz yerleşim payı (köy+) | %0,5 (%0–%2) | %1,6 (%0–%5,2) | %1,8 (%0–%4,6) | %2,4 (%0,1–%5) | %1,7 (%0,3–%5) | %1,2 (%0,1–%3,1) |
| Devriye durdurması | 18 (9–43,4) | 20,7 (12,4–51,2) | 23,8 (12,7–49,5) | 25 (11,2–54,3) | 25,4 (12,7–59,3) | 31,7 (12,8–54,2) |

## Kahraman seviyeleri

### Doğuş seviyesi (bütün dünyalar, on yıl içinde doğanlar)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 |
|---|---|---|---|---|---|
| 1–10 | 1711 | 1,56 | %52 | %40 | %7,9 |
| 11–20 | 1766 | 1,58 | %51 | %41 | %8,5 |
| 21–30 | 1746 | 1,56 | %52 | %40 | %8 |
| 31–40 | 1723 | 1,53 | %54 | %40 | %6,6 |
| 41–50 | 1666 | 1,57 | %51 | %40 | %8,5 |
| 51–60 | 1669 | 1,56 | %52 | %40 | %7,6 |

### Ölüm seviyesi (bütün dünyalar, on yıl içinde ölenler)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 | Sv4 | Sv5 | Sv6 | Sv7 | Sv8 | Sv9 | Sv10 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 1–10 | 630 | 3,64 | %9 | %20 | %31 | %17 | %7,8 | %5,9 | %3 | %3,3 | %2,4 | %1,3 |
| 11–20 | 701 | 3,96 | %6,8 | %17 | %29 | %18 | %9,6 | %6,8 | %5 | %2,6 | %2,7 | %2,9 |
| 21–30 | 779 | 4,08 | %7,3 | %14 | %29 | %21 | %9,2 | %4,5 | %3,9 | %4,2 | %3,5 | %3,7 |
| 31–40 | 656 | 4,29 | %8,2 | %11 | %25 | %20 | %12 | %6,7 | %5,2 | %4,3 | %2,9 | %4,6 |
| 41–50 | 691 | 4,25 | %9,4 | %10 | %24 | %22 | %12 | %6,4 | %3,8 | %4,5 | %3,2 | %4,5 |
| 51–60 | 673 | 4,3 | %8,9 | %12 | %22 | %22 | %10 | %7 | %5,5 | %4,8 | %3,9 | %3,9 |

### Yaşayan kahramanların seviyesi (bütün dünyalar, on yılın son yılının sonunda)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 | Sv4 | Sv5 | Sv6 | Sv7 | Sv8 | Sv9 | Sv10 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 10 | 1782 | 3,94 | %8,4 | %20 | %29 | %15 | %7,2 | %5,1 | %4,6 | %3,9 | %3,2 | %4 |
| 20 | 1989 | 3,88 | %9 | %18 | %29 | %17 | %8,4 | %4,1 | %3,8 | %2,9 | %2 | %5,2 |
| 30 | 2017 | 3,89 | %8,1 | %20 | %27 | %19 | %7,7 | %4,1 | %3,6 | %3,4 | %2,2 | %5 |
| 40 | 2159 | 3,84 | %8 | %18 | %31 | %18 | %8,8 | %5,7 | %3,1 | %2,7 | %2,3 | %3,7 |
| 50 | 2217 | 3,94 | %6,9 | %17 | %29 | %20 | %9,1 | %5 | %4 | %3,5 | %2 | %3,7 |
| 60 | 2336 | 3,91 | %7,5 | %18 | %27 | %20 | %11 | %6,1 | %3,8 | %2,4 | %1,9 | %3,8 |

### Ölüm nedenleri (bütün dünyalar)

| Neden | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 | Toplam |
|---|---|---|---|---|---|---|---|
| Kamp saldırısı | 207 | 204 | 212 | 184 | 194 | 184 | 1185 (%29) |
| Kuşatma | 134 | 143 | 198 | 114 | 100 | 111 | 800 (%19) |
| Trol | 128 | 120 | 122 | 112 | 131 | 105 | 718 (%17) |
| Bilinmiyor | 32 | 88 | 114 | 131 | 147 | 157 | 669 (%16) |
| Ejderha | 79 | 109 | 82 | 65 | 67 | 49 | 451 (%11) |
| Düello | 26 | 20 | 25 | 26 | 18 | 31 | 146 (%3,5) |
| Kervan soygunu | 21 | 12 | 15 | 15 | 19 | 23 | 105 (%2,5) |
| Yağma akını | 1 | 3 | 9 | 3 | 7 | 4 | 27 (%0,7) |
| Han baskını (medeniyet) | 0 | 2 | 1 | 3 | 3 | 1 | 10 (%0,2) |
| Han baskını (canavar) | 1 | 0 | 0 | 1 | 4 | 4 | 10 (%0,2) |
| Yol pususu | 1 | 0 | 1 | 2 | 1 | 4 | 9 (%0,2) |

Neden, ölümün kaydedildiği andaki son muharebenin türünden (başlık ve taraflar) ya da suikast olayından çıkarılır.

## Olay türleri

Dünya başına yıllık olay sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Kahraman (`hero`) | 229 (130–450) | 271 (162–487) | 295 (171–475) | 292 (187–525) | 283 (167–566) | 342 (187–562) |
| Sefer/ilan (`quest`) | 43,3 (17,6–196) | 46,3 (15,7–216) | 39,1 (10,6–229) | 41,4 (16,8–190) | 62,5 (13,5–221) | 74,6 (12,3–173) |
| org | 41,9 (33,5–48,9) | 38 (28,1–45) | 35,3 (27,1–42,5) | 35,9 (25–44,1) | 33,3 (24,4–43,2) | 33,1 (23,6–41,3) |
| İnşaat (`build`) | 16,6 (9,55–26,1) | 14,3 (10,4–22,8) | 14,5 (7,8–19,2) | 10,2 (5,95–14,5) | 10,3 (4,3–13,2) | 6,2 (3,5–13,7) |
| Kamp (`lair`) | 12,6 (2,5–30,5) | 12,7 (2,05–30,7) | 9,95 (1,75–31,3) | 9,15 (2–28,9) | 11,6 (1,2–30) | 13,8 (2,55–27,6) |
| Han (`inn`) | 10,8 (6,7–16,4) | 11,4 (7,3–17,6) | 11,8 (6,65–17,8) | 11,6 (6–18,7) | 10,6 (5,7–19,4) | 10,6 (6,5–18,6) |
| Savaş (`war`) | 12,4 (5,95–22,1) | 11,8 (5,75–24,9) | 11,2 (4,35–24,6) | 9,15 (4,25–19,3) | 10,9 (2,3–15,7) | 9,4 (1,35–14,8) |
| Göç (`migration`) | 4,2 (2,65–6,5) | 5,15 (2,8–10,1) | 4,9 (1,95–9,95) | 5,15 (2,35–8,25) | 4,25 (2–8,4) | 5,55 (2,3–8,1) |
| Ölüm/terk (`death`) | 4,25 (2,75–6,6) | 4,4 (3,1–6,45) | 4,9 (2,65–7,75) | 4,45 (2,7–5,8) | 4,05 (2,65–6,1) | 3,9 (2,15–7) |
| Deniz (`sea`) | 4,75 (2,7–10,1) | 5,15 (2,6–8,15) | 4,25 (3,15–7,1) | 4,45 (1,85–6,7) | 3,9 (0,7–6,7) | 2,6 (0,65–5,85) |
| epitaph | 4,1 (2,65–6,25) | 4,05 (2,65–6,3) | 4,6 (2,3–7,65) | 4,2 (2,55–5,4) | 3,8 (2,5–6,05) | 3,9 (1,75–6,75) |
| Baskın (`raid`) | 3,85 (2,5–5) | 3,75 (2,05–4,85) | 3,15 (1,95–5,15) | 3,35 (2,2–5,05) | 3,9 (2,05–4,8) | 3,7 (1,8–4,75) |
| Ekonomi (`economy`) | 4,2 (2,65–5,8) | 4,2 (2,85–6) | 3,35 (1,15–5,65) | 2,9 (1,55–4,55) | 2,55 (1,35–4,35) | 2,8 (0,65–4,55) |
| Dünya (`world`) | 1,55 (1,15–2,95) | 1,8 (1–2,75) | 1,45 (1–2,6) | 1,8 (1,25–3,15) | 1,85 (1,25–3,1) | 1,9 (1,55–3,1) |
| politics | 1,6 (1,05–2,2) | 1,4 (0,8–2,7) | 1,15 (0,6–2,1) | 1,5 (0,15–1,9) | 1,25 (0,4–1,7) | 1 (0,3–1,65) |
| Ejderha (`dragon`) | 2,35 (0–2,95) | 2,4 (0–3) | 2,1 (0–2,95) | 0,5 (0–3) | 0 (0–2,9) | 0 (0–2,3) |
| Sınıf (`class`) | 1 (1–1,25) | 1 | 1 (1–1,05) | 1 | 1 (1–1,1) | 1 (1–1,65) |
| Büyüme (`growth`) | 1,25 (0,75–1,7) | 1,1 (0,65–1,6) | 0,8 (0,3–1,3) | 0,65 (0,35–1,05) | 0,45 (0,2–0,65) | 0,3 (0–0,8) |
| Kriz (anlatıcı) (`crisis`) | 0,25 (0–0,7) | 0,3 (0,05–0,65) | 0,3 (0,05–0,55) | 0,5 (0,05–0,75) | 0,55 (0,2–0,75) | 0,6 (0,3–0,7) |
| Keşif (`discover`) | 0,55 (0,25–1,45) | 0,65 (0,2–1,4) | 0,55 (0,15–1) | 0,25 (0,05–1) | 0,25 (0–0,55) | 0,1 (0–0,7) |
| Yerleşim (`settle`) | 0,85 (0,45–1,4) | 0,4 (0,1–1,25) | 0,2 (0–0,9) | 0,4 (0,05–1) | 0,2 (0–0,6) | 0 (0–0,6) |
| Diplomasi (`diplomacy`) | 0,2 (0–1,4) | 0,25 (0–1,8) | 0,2 (0–1,15) | 0,2 (0–1) | 0,3 (0–0,75) | 0,2 (0–1,2) |
| Ticaret (`trade`) | 0,35 (0–1,15) | 0,3 (0–1,4) | 0,3 (0–0,95) | 0,1 (0–0,5) | 0,15 (0–0,3) | 0,1 (0–0,35) |
| Gerginlik (`tension`) | 0,2 (0–1,15) | 0,25 (0–1,15) | 0,15 (0–1,05) | 0,3 (0–0,6) | 0,05 (0–0,7) | 0 (0–0,8) |
| Rahatlama (anlatıcı) (`relief`) | 0,1 (0–0,2) | 0,2 (0–0,2) | 0,1 (0–0,3) | 0,1 (0–0,2) | 0,1 (0–0,25) | 0,1 (0–0,2) |
| Temas (`contact`) | 0 (0–0,1) | 0 (0–0,1) | 0 | 0 | 0 | 0 |

## Büyük olay türleri

Dünya başına yıllık büyük olay sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Sefer/ilan (`quest`) | 28,2 (11,8–132) | 29,7 (10–145) | 26,5 (7,25–160) | 26,4 (10–128) | 40 (8,8–153) | 48,6 (7,6–121) |
| Kahraman (`hero`) | 19,1 (13–43,2) | 22,6 (14,5–44,9) | 25,3 (13,9–43,5) | 21 (13–46) | 23,5 (12–48,6) | 27,2 (14,1–46,9) |
| Kamp (`lair`) | 9,1 (2–21,7) | 9,4 (1,55–21,6) | 7,35 (1,35–22,2) | 7 (1,5–20,8) | 8,55 (1–21,7) | 10,5 (2–19,7) |
| Savaş (`war`) | 7,9 (4,65–15,1) | 7,25 (4,1–16,2) | 7 (3–15,9) | 6,2 (3,45–13,1) | 6,95 (1,45–10,4) | 6 (0,9–10,1) |
| Ölüm/terk (`death`) | 4,25 (2,75–6,6) | 4,4 (3,1–6,45) | 4,9 (2,65–7,75) | 4,45 (2,7–5,8) | 4,05 (2,65–6,1) | 3,9 (2,15–7) |
| epitaph | 4,1 (2,65–6,25) | 4,05 (2,65–6,3) | 4,6 (2,3–7,65) | 4,2 (2,55–5,4) | 3,8 (2,5–6,05) | 3,9 (1,75–6,75) |
| Baskın (`raid`) | 3,55 (2,5–4,75) | 3,6 (1,7–4,7) | 3,1 (1,85–4,85) | 3,2 (2,05–4,35) | 3,55 (1,95–4,45) | 3,6 (1,5–4,55) |
| Deniz (`sea`) | 2,95 (1,8–7,1) | 3,25 (1,75–6,75) | 3,05 (1,7–5,2) | 3 (0,95–4,5) | 2,85 (0,4–5,15) | 1,8 (0,3–4,7) |
| Han (`inn`) | 1,8 (1,2–3,6) | 1,85 (1,25–3,45) | 1,9 (1,15–3,8) | 1,5 (0,75–4,25) | 1,7 (1,1–3,55) | 1,7 (1,1–5,05) |
| org | 1,6 (0,95–2,75) | 1,6 (0,85–2,6) | 1,75 (0,8–2,5) | 1,9 (1,1–3,15) | 1,8 (0,55–2,85) | 1,7 (0,5–2,75) |
| Dünya (`world`) | 1,3 (0,85–2,05) | 1,4 (0,8–2,05) | 1,25 (0,8–2,15) | 1,45 (0,9–2,6) | 1,55 (1,05–2,45) | 1,6 (1,25–2,25) |
| Ejderha (`dragon`) | 2,35 (0–2,95) | 2,4 (0–3) | 2,1 (0–2,95) | 0,5 (0–3) | 0 (0–2,9) | 0 (0–2,3) |
| İnşaat (`build`) | 0,65 (0,15–2,05) | 0,9 (0,25–1,4) | 0,65 (0,4–1,25) | 0,25 (0,1–0,7) | 0,45 (0,1–1) | 0,2 (0–0,5) |
| Kriz (anlatıcı) (`crisis`) | 0,25 (0–0,7) | 0,3 (0,05–0,65) | 0,3 (0,05–0,55) | 0,5 (0,05–0,75) | 0,55 (0,2–0,75) | 0,6 (0,3–0,7) |
| Göç (`migration`) | 0,3 (0,2–0,55) | 0,2 (0,1–0,4) | 0,2 (0–0,4) | 0,25 (0,1–0,3) | 0,3 (0,1–0,5) | 0,2 (0,1–0,4) |
| Yerleşim (`settle`) | 0,4 (0,2–0,65) | 0,2 (0,05–0,5) | 0,1 (0–0,45) | 0,2 (0,05–0,5) | 0,1 (0–0,25) | 0 (0–0,25) |
| Keşif (`discover`) | 0,3 (0,15–0,6) | 0,35 (0,1–0,5) | 0,2 (0–0,5) | 0,1 (0–0,5) | 0,05 (0–0,35) | 0 (0–0,35) |
| Gerginlik (`tension`) | 0,2 (0–1,15) | 0,25 (0–1,15) | 0,15 (0–1,05) | 0,3 (0–0,6) | 0,05 (0–0,7) | 0 (0–0,8) |
| Ekonomi (`economy`) | 0,3 (0,2–0,6) | 0,2 (0,05–0,3) | 0,15 (0–0,35) | 0,15 (0–0,45) | 0 (0–0,2) | 0,1 (0–0,5) |
| politics | 0,2 (0,05–0,35) | 0,15 (0–0,4) | 0,2 (0–0,35) | 0,1 (0–0,4) | 0,1 (0–0,3) | 0,1 (0–0,45) |
| Diplomasi (`diplomacy`) | 0,15 (0–0,55) | 0,1 (0–0,6) | 0,1 (0–0,65) | 0,1 (0–0,9) | 0,15 (0–0,35) | 0,1 (0–0,9) |
| Rahatlama (anlatıcı) (`relief`) | 0,1 (0–0,2) | 0,2 (0–0,2) | 0,1 (0–0,3) | 0,1 (0–0,2) | 0,1 (0–0,25) | 0,1 (0–0,2) |
| Ticaret (`trade`) | 0,15 (0–0,55) | 0,15 (0–0,45) | 0,15 (0–0,4) | 0 (0–0,2) | 0,1 (0–0,2) | 0 (0–0,15) |
| Sınıf (`class`) | 0 | 0 | 0 | 0 | 0 | 0 |
| Temas (`contact`) | 0 (0–0,1) | 0 (0–0,1) | 0 | 0 | 0 | 0 |
| Büyüme (`growth`) | 0 (0–0,1) | 0 (0–0,25) | 0 (0–0,15) | 0 (0–0,15) | 0 | 0 (0–0,05) |

## Muharebe türleri

Dünya başına yıllık muharebe sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Kamp saldırısı (`camp`) | 3,55 (1,1–7,3) | 3,55 (1–7,35) | 3,7 (0,6–7,1) | 3 (1,15–7,05) | 4,2 (0,65–7,25) | 4,3 (1,1–6,8) |
| Trol (`troll`) | 2,65 (1–5,05) | 2,7 (0,75–5,75) | 2,15 (0,7–6,25) | 2,7 (0,6–5,5) | 2,65 (1,25–6,35) | 2,35 (0,5–5,8) |
| Kuşatma (`siege`) | 1,7 (0,75–2,55) | 1,5 (0,75–2,8) | 1,35 (0,7–2,65) | 1,25 (0,55–2,4) | 1,3 (0,25–2,3) | 1,5 (0,15–2,55) |
| Yağma akını (`plunder`) | 1,45 (0,3–3,7) | 1,6 (0,1–3,75) | 1,1 (0,45–3,35) | 1,25 (0,35–2,85) | 1,25 (0–2,45) | 0,7 (0–2,1) |
| Yapı baskını (canavar) (`extRaid`) | 1,5 (0,45–2,6) | 1,3 (0,35–2,3) | 1,2 (0,2–2,35) | 1,2 (0,25–1,85) | 0,75 (0,1–1,5) | 0,8 (0,2–1,55) |
| Ejderha (`dragon`) | 0,95 (0–1,5) | 1,05 (0–1,6) | 0,9 (0–1,55) | 0,25 (0–1,65) | 0 (0–1,5) | 0 (0–1,1) |
| Yerleşim baskını (canavar) (`raid`) | 0,4 (0,05–1,05) | 0,25 (0–1,2) | 0,3 (0–1,2) | 0,35 (0–1,05) | 0,5 (0,05–1,15) | 0,4 (0,25–0,6) |
| Kervan soygunu (`robbery`) | 0,3 (0,05–0,75) | 0,2 (0–0,35) | 0,25 (0–0,4) | 0,2 (0–0,4) | 0,2 (0,05–0,55) | 0,25 (0–0,8) |
| Düello (`duel`) | 0,15 (0–0,45) | 0,1 (0–0,35) | 0,1 (0–0,45) | 0,2 (0–0,35) | 0,1 (0–0,25) | 0,15 (0–0,5) |
| Korsan savaşı (`pirate`) | 0 (0–0,1) | 0 (0–0,15) | 0 (0–0,1) | 0 (0–0,15) | 0 (0–0,1) | 0,05 (0–0,1) |
| Yol pususu (`ambush`) | 0 | 0 | 0 | 0 (0–0,1) | 0 | 0 (0–0,05) |
| Han baskını (medeniyet) (`innCiv`) | 0 | 0 (0–0,05) | 0 (0–0,05) | 0 | 0 (0–0,05) | 0 |
| Han baskını (canavar) (`innMonster`) | 0 | 0 | 0 | 0 (0–0,05) | 0 (0–0,05) | 0 (0–0,1) |
| Deniz savaşı (`naval`) | 0 (0–0,2) | 0 (0–0,2) | 0 (0–0,2) | 0 (0–0,2) | 0 (0–0,2) | 0 (0–0,15) |

## Kamp türleri

Dünya başına yaşayan kamp (yıl sonu değerlerinin on yıllık ortalaması): dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Hobgoblin (`hobgoblin`) | 3,65 (2,75–4,9) | 3,55 (3–5,1) | 3,35 (2,75–4,75) | 3,45 (2,8–5,1) | 3,3 (2,65–4,7) | 3,1 (2,55–4,1) |
| Trol (`troll`) | 2,15 (1,45–2,9) | 2,55 (1,75–2,95) | 2,55 (1,7–3) | 2,75 (1,9–3) | 2,7 (2,25–3) | 2,9 (2,4–3) |
| Goblin (`goblin`) | 1,9 (0,8–2,4) | 1,5 (0,5–1,9) | 1,55 (0,55–2,35) | 1,85 (0,2–2,2) | 1,7 (0,5–2,2) | 1,55 (0,85–2,4) |
| Bugbear (`bugbear`) | 0,6 (0,15–1) | 0,65 (0,3–1,1) | 0,8 (0,35–1) | 0,95 (0,5–1,05) | 0,75 (0,2–1,25) | 0,8 (0,3–1,1) |
| Ejderha (`dragon`) | 1 (0–1) | 1 (0–1) | 0,9 (0–1) | 0,15 (0–1) | 0 (0–1) | 0 (0–1) |
| Korsan (`pirate`) | 0,45 (0,15–1,15) | 0,35 (0,05–0,65) | 0,45 (0–1,1) | 0,5 (0–0,7) | 0,35 (0,05–0,75) | 0,35 (0–0,65) |
| Aç haydut (`bandit`) | 0 (0–0,05) | 0 (0–0,1) | 0 (0–0,25) | 0 (0–0,4) | 0 (0–0,2) | 0 (0–0,55) |

## Kademe dağılımı

Yaşayan yerleşimlerin kademelere dağılımı, bütün dünyalar (yıl sonu; parantezde sayı).

| Yıl | 0 Kamp | 1 Köy | 2 Kasaba | 3 Şehir |
|---|---|---|---|---|
| 10 | %24 (282) | %54 (634) | %15 (179) | %6,7 (79) |
| 20 | %20 (240) | %57 (676) | %16 (184) | %6,8 (80) |
| 30 | %19 (229) | %59 (688) | %14 (168) | %7,7 (90) |
| 40 | %20 (236) | %60 (703) | %13 (151) | %7,4 (87) |
| 50 | %19 (230) | %63 (742) | %11 (130) | %6,8 (81) |
| 60 | %18 (218) | %64 (754) | %11 (129) | %6,6 (78) |

Başkentlerin kademelere dağılımı, bütün dünyalar (yıl sonu; parantezde sayı).

| Yıl | 0 Kamp | 1 Köy | 2 Kasaba | 3 Şehir |
|---|---|---|---|---|
| 10 | %0 (0) | %6 (5) | %20 (17) | %73 (61) |
| 20 | %0 (0) | %4,8 (4) | %20 (17) | %75 (62) |
| 30 | %2,4 (2) | %1,2 (1) | %17 (14) | %79 (65) |
| 40 | %1,2 (1) | %2,5 (2) | %14 (11) | %83 (67) |
| 50 | %2,5 (2) | %4,9 (4) | %12 (10) | %80 (65) |
| 60 | %0 (0) | %7,6 (6) | %11 (9) | %81 (64) |

## Dünyalar

| Seed | Medeniyet | Yerleşim | Nüfus | Çöküş | Efsane | En yüksek Sv | Doğan / ölü kahraman | İlk şehir (yıl, medyan) | Süre (sn) | Son hash |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | 5 | 67 | 2010 | 1 | 0 | 10 | 481 / 160 | – | 56,9 | `9d69b8332edbe6f6` |
| 2 | 6 | 79 | 2835 | 3 | 2 | 10 | 683 / 220 | – | 67,4 | `f88ffb96321e9577` |
| 3 | 5 | 87 | 3204 | 2 | 3 | 10 | 758 / 152 | – | 67,1 | `45bc674d7de19694` |
| 4 | 6 | 89 | 3657 | 2 | 5 | 10 | 1135 / 242 | – | 87 | `ab3580824e6f92ed` |
| 5 | 7 | 95 | 2931 | 1 | 5 | 10 | 884 / 307 | – | 96,5 | `44dc46d684505f34` |
| 6 | 6 | 93 | 3278 | 8 | 2 | 10 | 831 / 173 | – | 95 | `bf9642da791d97fc` |
| 7 | 4 | 72 | 2148 | 6 | 4 | 10 | 513 / 142 | – | 51 | `863b7339c7af3b5c` |
| 8 | 5 | 78 | 3002 | 2 | 4 | 10 | 723 / 230 | – | 62,8 | `002f06c398bf22b7` |
| 9 | 5 | 75 | 2509 | 9 | 2 | 10 | 620 / 112 | – | 56,3 | `ce48752b5349a843` |
| 10 | 4 | 46 | 1626 | 6 | 2 | 9 | 390 / 133 | – | 38,7 | `56a2826288a5e8d6` |
| 11 | 4 | 61 | 1606 | 8 | 0 | 10 | 389 / 121 | – | 41,2 | `83dc5d7f9e64ea7e` |
| 12 | 5 | 75 | 2521 | 4 | 2 | 10 | 653 / 265 | – | 64,4 | `64dceeda66a3d1f9` |
| 13 | 4 | 59 | 2511 | 1 | 3 | 10 | 581 / 235 | – | 46,4 | `06b44c3e3a955ff6` |
| 14 | 3 | 47 | 1703 | 3 | 0 | 10 | 404 / 150 | – | 33,1 | `5d18923f1b4ce8c6` |
| 15 | 4 | 60 | 2252 | 4 | 0 | 10 | 414 / 104 | – | 39,6 | `04edca848b3719b4` |
| 16 | 6 | 96 | 2710 | 5 | 1 | 10 | 822 / 193 | – | 83,1 | `fab52bb2b093b0f1` |

Çöküşler:

- seed 1, 5. yıl (gün 171): Demirkanat Cumhuriyeti başkenti kaybetti: Yeşilkaya (Granitsunak Krallığı aldı)
- seed 2, 3. yıl (gün 113): Alacayurt Boyları başkenti kaybetti: Karlıtepe (Közburç Krallığı aldı)
- seed 2, 20. yıl (gün 776): Taşkandil Krallığı yok oldu
- seed 2, 36. yıl (gün 1402): Kurtoba Boyları başkenti kaybetti: Dumanköprü (Şafaktepe Teokrasisi aldı)
- seed 3, 17. yıl (gün 670): Kanlıdiş Boyları başkenti kaybetti: Okyayı (Bozkale Teokrasisi aldı)
- seed 3, 20. yıl (gün 764): Kanlıdiş Boyları yok oldu
- seed 4, 8. yıl (gün 284): Toynakbaş Boyları başkenti kaybetti: Kuzeybük (Keseli Cumhuriyeti aldı)
- seed 4, 26. yıl (gün 1001): Keseli Cumhuriyeti başkenti kaybetti: Kuzeybük (Toynakbaş Boyları aldı)
- seed 5, 14. yıl (gün 551): Savaşçukur Boyları yok oldu
- seed 6, 2. yıl (gün 65): Kuzeybük Krallığı yok oldu
- seed 6, 9. yıl (gün 348): Kalkanova Teokrasisi başkenti kaybetti: Kavşakpazar (Demirkanat Boyları aldı)
- seed 6, 9. yıl (gün 349): Közsaray Cumhuriyeti başkenti kaybetti: Günyazı (Alevgeçit Boyları aldı)
- seed 6, 21. yıl (gün 837): Meşekale Krallığı yok oldu
- seed 6, 51. yıl (gün 2007): Kızılyurt Boyları başkenti kaybetti: Neşeliova (Alevgeçit Boyları aldı)
- seed 6, 56. yıl (gün 2231): Pınardere Boyları yok oldu
- seed 6, 57. yıl (gün 2259): Fıçıköy Krallığı yok oldu
- seed 6, 57. yıl (gün 2273): Yeşilyazı Boyları yok oldu
- seed 7, 10. yıl (gün 373): Kurtoba Boyları başkenti kaybetti: Közburç (Közburç Krallığı aldı)
- seed 7, 13. yıl (gün 515): Kavalpınar Boyları yok oldu
- seed 7, 20. yıl (gün 784): Fıçıköy Cumhuriyeti başkenti kaybetti: Kocapınar (Közburç Krallığı aldı)
- seed 7, 34. yıl (gün 1324): Fıçıköy Cumhuriyeti yok oldu
- seed 7, 39. yıl (gün 1530): Kurtoba Boyları başkenti kaybetti: Gölköprü (Közburç Krallığı aldı)
- seed 7, 48. yıl (gün 1901): Kafatepe Teokrasisi yok oldu
- seed 8, 22. yıl (gün 848): Tatlıçayır Krallığı başkenti kaybetti: Sarıhisar (Güneştacı Teokrasisi aldı)
- seed 8, 37. yıl (gün 1472): Toynakbaş Boyları yok oldu
- seed 9, 7. yıl (gün 280): Güneştacı Krallığı başkenti kaybetti: Karaköy (Toynakbaş Boyları aldı)
- seed 9, 10. yıl (gün 366): Toynakbaş Krallığı yok oldu
- seed 9, 16. yıl (gün 602): Toynakbaş Boyları başkenti kaybetti: Kocapınar (Güneştacı Krallığı aldı)
- seed 9, 22. yıl (gün 864): Demirkanat Teokrasisi başkenti kaybetti: Sarıhisar (Yeminköprü Boyları aldı)
- seed 9, 25. yıl (gün 989): Pınardere Boyları yok oldu
- seed 9, 37. yıl (gün 1449): Yeminköprü Boyları başkenti kaybetti: Sarıhisar (Demirkanat Teokrasisi aldı)
- seed 9, 57. yıl (gün 2273): Fıçıköy Cumhuriyeti başkenti kaybetti: Fıçıköy (Güneştacı Krallığı aldı)
- seed 9, 57. yıl (gün 2280): Toynakbaş Boyları yok oldu
- seed 9, 60. yıl (gün 2381): Kurtkale Teokrasisi yok oldu
- seed 10, 3. yıl (gün 101): Kızılyurt Boyları başkenti kaybetti: Boynuztepe (Taşkandil Teokrasisi aldı)
- seed 10, 6. yıl (gün 218): Sisova Teokrasisi yok oldu
- seed 10, 13. yıl (gün 483): Kızılyurt Boyları başkenti kaybetti: Sisova (Taşkandil Teokrasisi aldı)
- seed 10, 30. yıl (gün 1181): Kafatepe Boyları başkenti kaybetti: Kafatepe (Kızılyurt Boyları aldı)
- seed 10, 42. yıl (gün 1671): Kafatepe Boyları yok oldu
- seed 10, 48. yıl (gün 1891): Sisova Boyları başkenti kaybetti: Sisova (Karamum Krallığı aldı)
- seed 11, 1. yıl (gün 36): Gümüşdal Cumhuriyeti başkenti kaybetti: Yosunpınar (Demirkanat Boyları aldı)
- seed 11, 18. yıl (gün 687): Demirkanat Boyları başkenti kaybetti: Okyayı (Karaörs Teokrasisi aldı)
- seed 11, 21. yıl (gün 810): Çamgözcü Boyları yok oldu
- seed 11, 39. yıl (gün 1551): Karaörs Teokrasisi başkenti kaybetti: Kurtkent (Demirkanat Boyları aldı)
- seed 11, 52. yıl (gün 2071): Karaörs Teokrasisi başkenti kaybetti: Söğütsırt (Demirkanat Boyları aldı)
- seed 11, 54. yıl (gün 2130): Çamkoru Boyları yok oldu
- seed 11, 54. yıl (gün 2157): Şafaktepe Krallığı başkenti kaybetti: Pulkalkan (Demirkanat Boyları aldı)
- seed 11, 60. yıl (gün 2384): Demirkanat Boyları başkenti kaybetti: Pulkalkan (Şafaktepe Krallığı aldı)
- seed 12, 10. yıl (gün 368): Gümüşdal Boyları başkenti kaybetti: Bozkent (Pulzırh Teokrasisi aldı)
- seed 12, 11. yıl (gün 415): Közsaray Cumhuriyeti başkenti kaybetti: Kızılkül (Taşkandil Krallığı aldı)
- seed 12, 25. yıl (gün 985): Gümüşdal Boyları başkenti kaybetti: Kurtkale (Pulzırh Teokrasisi aldı)
- seed 12, 32. yıl (gün 1250): Tatlıçayır Cumhuriyeti başkenti kaybetti: Kurtkale (Pulzırh Teokrasisi aldı)
- seed 13, 59. yıl (gün 2351): Kızılyurt Boyları yok oldu
- seed 14, 36. yıl (gün 1412): Toynakbaş Boyları başkenti kaybetti: Közsaray (Güneştacı Krallığı aldı)
- seed 14, 38. yıl (gün 1505): Günyazı Cumhuriyeti yok oldu
- seed 14, 50. yıl (gün 1975): Ayburç Boyları yok oldu
- seed 15, 7. yıl (gün 251): Közsaray Cumhuriyeti başkenti kaybetti: Gölköprü (Kızılyurt Boyları aldı)
- seed 15, 11. yıl (gün 408): Kızılyurt Boyları başkenti kaybetti: Gölköprü (Balköprü Teokrasisi aldı)
- seed 15, 26. yıl (gün 1008): Balköprü Teokrasisi başkenti kaybetti: Sarıhisar (Kızılyurt Boyları aldı)
- seed 15, 35. yıl (gün 1366): Balköprü Teokrasisi başkenti kaybetti: Sarıhisar (Kızılyurt Boyları aldı)
- seed 16, 3. yıl (gün 113): Akburç Cumhuriyeti başkenti kaybetti: Dinginpınar (Kurtoba Boyları aldı)
- seed 16, 10. yıl (gün 399): Meşekale Boyları yok oldu
- seed 16, 13. yıl (gün 500): Alazvadi Cumhuriyeti başkenti kaybetti: Gölpınar (Alevgeçit Krallığı aldı)
- seed 16, 20. yıl (gün 783): Alevgeçit Krallığı başkenti kaybetti: Gölpınar (Tatlıçayır Krallığı aldı)
- seed 16, 29. yıl (gün 1137): Alevgeçit Krallığı başkenti kaybetti: Alazvadi (Tatlıçayır Krallığı aldı)

## Yıllık ayrıntı

Hücre: medyan (p10–p90), 16 dünya. Yıl y = (y−1)·40+1 … y·40. günler. Bütün değerler `report.json` içinde (`metrics`), dünya başına değerler `../runs/f1b-6` altında.

### Medeniyet (1/3)

| Yıl | Yaşayan medeniyet | Yeni medeniyet (yeniden doğan) | Yok olan medeniyet | Başkent kaybı (medeniyet yaşarken) | Çöküş (yok olma + başkent kaybı) | Yaşayan yerleşim |
|---|---|---|---|---|---|---|
| 1 | 5 (3,5–7) | 0 | 0 | 0 | 0 | 73,5 (52–92) |
| 2 | 5 (3,5–6,5) | 0 | 0 | 0 | 0 | 74 (52–92,5) |
| 3 | 5 (3,5–6,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 74 (52–93) |
| 4 | 5 (4–6,5) | 0 | 0 | 0 | 0 | 74,5 (52,5–93,5) |
| 5 | 5 (4–7) | 0 | 0 | 0 | 0 | 74,5 (52–93,5) |
| 6 | 5 (3,5–7) | 0 | 0 | 0 | 0 | 74,5 (52–93,5) |
| 7 | 5 (3,5–7) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 75 (52–93,5) |
| 8 | 5 (3,5–7) | 0 | 0 | 0 | 0 | 75,5 (51–93,5) |
| 9 | 5 (3,5–7) | 0 | 0 | 0 | 0 | 75,5 (52–94) |
| 10 | 5 (3,5–7) | 0 (0–0,5) | 0 (0–0,5) | 0 (0–0,5) | 0 (0–1) | 75 (51–94) |
| 11 | 5 (3,5–7) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 75 (51,5–94,5) |
| 12 | 5 (3,5–7) | 0 | 0 | 0 | 0 | 75,5 (51,5–94,5) |
| 13 | 5 (4–7) | 0 | 0 | 0 (0–0,5) | 0 (0–1) | 75 (52–94,5) |
| 14 | 5 (4–7) | 0 | 0 | 0 | 0 | 74,5 (52–94) |
| 15 | 5 (4–7) | 0 | 0 | 0 | 0 | 74,5 (52–94,5) |
| 16 | 5 (4–7) | 0 | 0 | 0 | 0 | 74,5 (52–94) |
| 17 | 5 (4–7) | 0 | 0 | 0 | 0 | 74,5 (53–94) |
| 18 | 5 (4–7) | 0 | 0 | 0 | 0 | 74,5 (52,5–94) |
| 19 | 5 (4–7) | 0 | 0 | 0 | 0 | 75 (52–94) |
| 20 | 5 (4–6,5) | 0 | 0 (0–0,5) | 0 (0–0,5) | 0 (0–1) | 75 (53,5–94,5) |
| 21 | 5 (4–6,5) | 0 | 0 (0–0,5) | 0 | 0 (0–0,5) | 75 (53–94) |
| 22 | 5 (4–6,5) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 75 (53–94) |
| 23 | 5 (4–6,5) | 0 (0–0,5) | 0 | 0 | 0 | 75 (52,5–94) |
| 24 | 5 (4–6,5) | 0 | 0 | 0 | 0 | 74,5 (52,5–94) |
| 25 | 5 (4–6,5) | 0 | 0 | 0 | 0 (0–0,5) | 74,5 (52–94,5) |
| 26 | 5 (4–6,5) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 74,5 (52–94,5) |
| 27 | 5 (4–6,5) | 0 | 0 | 0 | 0 | 74,5 (52–94,5) |
| 28 | 5 (4–6,5) | 0 | 0 | 0 | 0 | 74 (51,5–94,5) |
| 29 | 5 (4–6,5) | 0 | 0 | 0 | 0 | 74,5 (51,5–94,5) |
| 30 | 5 (4–6,5) | 0 | 0 | 0 | 0 | 74,5 (51,5–94,5) |
| 31 | 5 (4–6,5) | 0 | 0 | 0 | 0 | 75,5 (51,5–94,5) |
| 32 | 5 (4–6,5) | 0 | 0 | 0 | 0 | 75,5 (52–94,5) |
| 33 | 5 (4–6,5) | 0 | 0 | 0 | 0 | 75 (52–94,5) |
| 34 | 5 (4–6,5) | 0 | 0 | 0 | 0 | 75 (52,5–95) |
| 35 | 5 (4–6,5) | 0 | 0 | 0 | 0 | 75 (52–94,5) |
| 36 | 5 (4–6,5) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 75 (52–94) |
| 37 | 5 (4–6,5) | 0 | 0 | 0 | 0 (0–0,5) | 74,5 (52–94,5) |
| 38 | 5 (4–6,5) | 0 | 0 | 0 | 0 | 74,5 (52,5–94,5) |
| 39 | 5 (4–6,5) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 74,5 (52,5–94,5) |
| 40 | 5 (4–6,5) | 0 | 0 | 0 | 0 | 74,5 (53–94) |
| 41 | 5 (4–6,5) | 0 | 0 | 0 | 0 | 74,5 (53,5–93,5) |
| 42 | 5 (4–6,5) | 0 | 0 | 0 | 0 | 74,5 (53,5–93,5) |
| 43 | 5 (4–6,5) | 0 | 0 | 0 | 0 | 74,5 (53,5–93,5) |
| 44 | 5 (4–6,5) | 0 | 0 | 0 | 0 | 74,5 (53,5–93,5) |
| 45 | 5 (4–6,5) | 0 | 0 | 0 | 0 | 74,5 (53,5–93,5) |
| 46 | 5 (4–6,5) | 0 | 0 | 0 | 0 | 74,5 (53,5–93,5) |
| 47 | 5 (4–6,5) | 0 | 0 | 0 | 0 | 74,5 (52,5–93,5) |
| 48 | 5 (4–6,5) | 0 | 0 | 0 | 0 (0–0,5) | 74,5 (54,5–93,5) |
| 49 | 5 (4–6,5) | 0 | 0 | 0 | 0 | 74,5 (54–93,5) |
| 50 | 5 (4–6,5) | 0 | 0 | 0 | 0 | 74,5 (54–94) |
| 51 | 5 (4–6,5) | 0 | 0 | 0 | 0 | 74 (54–94) |
| 52 | 5 (4–6,5) | 0 | 0 | 0 | 0 | 75 (53,5–94) |
| 53 | 5 (4–6,5) | 0 | 0 | 0 | 0 | 75 (53–94) |
| 54 | 5 (4–6,5) | 0 | 0 | 0 | 0 | 75 (53,5–94) |
| 55 | 5 (4–6,5) | 0 | 0 | 0 | 0 | 75 (53,5–94) |
| 56 | 5 (4–6,5) | 0 | 0 | 0 | 0 | 75 (53,5–94) |
| 57 | 5 (4–6) | 0 | 0 (0–0,5) | 0 | 0 (0–1) | 75 (53,5–94) |
| 58 | 5 (4–6) | 0 | 0 | 0 | 0 | 75 (52,5–94) |
| 59 | 5 (3,5–6) | 0 | 0 | 0 | 0 | 75 (53–94) |
| 60 | 5 (4–6) | 0 | 0 | 0 | 0 (0–0,5) | 75 (53–94) |

### Medeniyet (2/3)

| Yıl | Medeniyet başına yerleşim | 5+ kara yerleşimli medeniyet payı | Kurulan yerleşim | Fethedilen yerleşim | Terk edilen yerleşim | Toplam nüfus |
|---|---|---|---|---|---|---|
| 1 | 14,6 (13–15,3) | %100 (%86–%100) | 0 (0–1) | 1,5 (0–2) | 0 (0–0,5) | 2158 (1628–2972) |
| 2 | 14,8 (13,4–15,3) | %100 (%86–%100) | 0 (0–1,5) | 1 (0–1,5) | 0 (0–1) | 2311 (1657–2964) |
| 3 | 14,9 (13,4–15,3) | %100 (%83–%100) | 0 (0–1) | 1 (0,5–2) | 0 (0–0,5) | 2243 (1624–2961) |
| 4 | 14,8 (12,3–15,6) | %100 (%80–%100) | 0 (0–2) | 1 (0–1,5) | 0 (0–1) | 2282 (1750–3112) |
| 5 | 14,5 (12,3–15,3) | %100 (%78–%100) | 0 (0–1) | 1 (0–2,5) | 0 (0–1) | 2398 (1594–3033) |
| 6 | 14,7 (13,5–15,4) | %100 (%79–%100) | 0 (0–1) | 1 (0–2) | 0 (0–1) | 2347 (1758–3022) |
| 7 | 14,8 (13,5–15,3) | %100 (%79–%100) | 0 (0–1) | 1 (0–3) | 0 (0–0,5) | 2398 (1779–3016) |
| 8 | 14,5 (13,1–15,3) | %100 (%76–%100) | 0 (0–1) | 1 (0–2,5) | 0 (0–1) | 2370 (1766–3034) |
| 9 | 14,5 (13,1–15,4) | %100 (%71–%100) | 0 (0–1) | 1 (0–2) | 0 (0–1) | 2365 (1822–3148) |
| 10 | 14,7 (12,5–15,6) | %100 (%69–%100) | 0 (0–1,5) | 1 (0,5–2,5) | 0 (0–1) | 2404 (1834–3167) |
| 11 | 14,7 (12,6–15,7) | %100 (%69–%100) | 0 (0–1,5) | 1 (0–3) | 0 (0–1,5) | 2360 (1804–3191) |
| 12 | 14,8 (12–15,7) | %85 (%65–%100) | 0 (0–1) | 1 (0–2,5) | 0 | 2410 (1814–3222) |
| 13 | 14,5 (11,6–15,7) | %82 (%67–%100) | 0 (0–1) | 1,5 (0–2) | 0 (0–1) | 2354 (1874–3232) |
| 14 | 14,5 (12,1–15,7) | %82 (%66–%100) | 0 | 1 (0–2) | 0 (0–1) | 2396 (1812–3290) |
| 15 | 14,5 (12,2–15,7) | %82 (%66–%100) | 0 (0–1) | 1 (0–2) | 0 (0–1) | 2412 (1914–3157) |
| 16 | 14,5 (12,2–15,7) | %82 (%66–%100) | 0 (0–1) | 1 (0–2) | 0 (0–1) | 2567 (1801–3237) |
| 17 | 14,8 (12,2–15,7) | %85 (%66–%100) | 0 (0–1) | 1 (0–2,5) | 0 | 2486 (1752–3350) |
| 18 | 14,6 (11,9–15,4) | %83 (%60–%100) | 0 (0–1) | 1 (0–3) | 0 (0–1) | 2438 (1685–3275) |
| 19 | 14,5 (11,7–15,4) | %80 (%60–%100) | 0 (0–0,5) | 1 (0–2,5) | 0 (0–1) | 2445 (1707–3316) |
| 20 | 14,9 (11,9–16) | %85 (%60–%100) | 0 (0–1) | 1 (0–2,5) | 0 | 2474 (1836–3238) |
| 21 | 15 (13,2–16) | %92 (%66–%100) | 0 | 0,5 (0–2) | 0 (0–0,5) | 2576 (1840–3107) |
| 22 | 15 (13,2–16) | %83 (%66–%100) | 0 (0–1) | 1 (0–2,5) | 0 (0–1) | 2448 (1876–3094) |
| 23 | 14,4 (12,6–15,8) | %82 (%63–%100) | 0 (0–1) | 1 (0–2) | 0 (0–1) | 2384 (1779–3131) |
| 24 | 14,4 (12,7–15,8) | %82 (%63–%100) | 0 | 1 (0–2) | 0 (0–0,5) | 2470 (1789–3180) |
| 25 | 14,5 (13,1–15,8) | %82 (%63–%100) | 0 (0–1) | 0,5 (0–2) | 0 (0–1) | 2398 (1858–3101) |
| 26 | 14,5 (13,1–15,8) | %78 (%63–%100) | 0 (0–1) | 1 (0–2) | 0 | 2546 (1786–3156) |
| 27 | 14,5 (13,1–15,8) | %78 (%63–%100) | 0 (0–0,5) | 1 (0–2,5) | 0 | 2506 (1879–3236) |
| 28 | 14,4 (13–15,8) | %78 (%63–%100) | 0 | 0,5 (0–1,5) | 0 (0–1) | 2494 (1840–3124) |
| 29 | 14,5 (13–15,8) | %78 (%63–%100) | 0 (0–0,5) | 0,5 (0–3,5) | 0 (0–0,5) | 2555 (1884–3230) |
| 30 | 14,5 (13–15,7) | %78 (%63–%100) | 0 (0–0,5) | 0,5 (0–2) | 0 (0–0,5) | 2522 (1882–3224) |
| 31 | 14,6 (13,1–15,8) | %78 (%63–%100) | 0 (0–1) | 0 (0–2) | 0 | 2600 (1862–3214) |
| 32 | 14,4 (13,1–15,8) | %78 (%63–%100) | 0 (0–1,5) | 0,5 (0–2,5) | 0 (0–1,5) | 2542 (1877–3262) |
| 33 | 14,4 (13–15,8) | %78 (%63–%100) | 0 (0–1,5) | 0 (0–1,5) | 0 (0–1,5) | 2497 (1896–3221) |
| 34 | 14,6 (13–16,7) | %75 (%63–%100) | 0 (0–1) | 0 (0–2) | 0 (0–0,5) | 2418 (1930–3291) |
| 35 | 14,5 (13–16,6) | %78 (%59–%100) | 0 | 0,5 (0–2,5) | 0 (0–1) | 2548 (1892–3250) |
| 36 | 14,4 (12,4–16,6) | %75 (%59–%100) | 0 (0–0,5) | 1 (0–2) | 0 (0–0,5) | 2534 (1901–3298) |
| 37 | 14,6 (12,5–16,7) | %78 (%59–%100) | 0 | 1 (0–2) | 0 (0–1) | 2430 (1902–3198) |
| 38 | 14,8 (13,2–16,7) | %80 (%59–%100) | 0 (0–1) | 1 (0–1,5) | 0 (0–0,5) | 2569 (1888–3346) |
| 39 | 14,8 (13,2–16,7) | %78 (%59–%100) | 0 (0–1) | 1 (0–2) | 0 | 2555 (1906–3278) |
| 40 | 14,8 (13,1–16,7) | %75 (%59–%100) | 0 | 0 (0–1) | 0 (0–0,5) | 2636 (1846–3254) |
| 41 | 14,8 (13,1–16,7) | %75 (%59–%100) | 0 | 0 (0–2) | 0 (0–0,5) | 2577 (1894–3370) |
| 42 | 14,9 (13,2–16,7) | %75 (%59–%100) | 0 (0–1) | 1 (0–1) | 0 | 2560 (1788–3342) |
| 43 | 14,9 (13,2–16,7) | %75 (%59–%100) | 0 | 0,5 (0–1) | 0 | 2515 (1724–3257) |
| 44 | 14,9 (13,2–16,7) | %75 (%59–%100) | 0 | 1 (0–2) | 0 | 2572 (1848–3364) |
| 45 | 14,9 (13,2–16,7) | %75 (%59–%100) | 0 (0–0,5) | 0 (0–2) | 0 | 2676 (1884–3172) |
| 46 | 14,9 (13,1–16,7) | %75 (%59–%100) | 0 | 0 (0–1,5) | 0 | 2686 (1758–3150) |
| 47 | 14,9 (13,3–15,7) | %75 (%59–%100) | 0 (0–1,5) | 1 (0–3) | 0 (0–1) | 2592 (1861–3152) |
| 48 | 15 (12,7–16,5) | %75 (%59–%92) | 0 (0–1,5) | 1 (0–1,5) | 0 | 2604 (1806–3226) |
| 49 | 15 (12,6–16,5) | %75 (%54–%92) | 0 | 1 (0–2) | 0 (0–0,5) | 2559 (1791–3294) |
| 50 | 15 (12,8–16,7) | %75 (%50–%100) | 0 (0–1) | 0,5 (0–1,5) | 0 (0–0,5) | 2511 (1728–3304) |
| 51 | 15 (12,7–16,7) | %75 (%50–%100) | 0 | 0 (0–2) | 0 (0–1) | 2540 (1806–3156) |
| 52 | 15 (12,1–16,6) | %75 (%50–%100) | 0 (0–0,5) | 0,5 (0–1,5) | 0 (0–1) | 2574 (1730–3248) |
| 53 | 14,9 (11,9–16,6) | %71 (%50–%100) | 0 | 0,5 (0–2) | 0 (0–0,5) | 2554 (1822–3246) |
| 54 | 15 (12,1–16,7) | %75 (%50–%100) | 0 (0–0,5) | 0 (0–1,5) | 0 | 2524 (1658–3125) |
| 55 | 15 (12,1–16,7) | %75 (%50–%100) | 0 | 0 (0–1) | 0 | 2637 (1614–3201) |
| 56 | 15 (12,1–16,7) | %75 (%50–%100) | 0 | 0 (0–1,5) | 0 (0–0,5) | 2526 (1792–3186) |
| 57 | 15 (13,3–16,7) | %75 (%59–%100) | 0 | 0 (0–1,5) | 0 | 2528 (1740–3190) |
| 58 | 15 (12,8–16,7) | %75 (%54–%100) | 0 | 1 (0–2,5) | 0 | 2566 (1768–3270) |
| 59 | 15,1 (12,8–17,7) | %75 (%54–%100) | 0 | 0 (0–1) | 0 | 2536 (1672–3180) |
| 60 | 15 (13,3–16,7) | %75 (%59–%100) | 0 (0–0,5) | 1 (0–2) | 0 (0–1) | 2516 (1664–3241) |

### Medeniyet (3/3)

| Yıl | Altın medyanı (medeniyetler) | Boştaki iş gücü payı | Bölünme (ayrılıp kurulan medeniyet) | En büyük medeniyetin yerleşimi |
|---|---|---|---|---|
| 1 | 277 (202–449) | %18 (%12–%27) | 0 | 19 (17,5–22) |
| 2 | 274 (156–660) | %18 (%12–%30) | 0 | 19 (17,5–21,5) |
| 3 | 363 (143–491) | %17 (%12–%28) | 0 | 19,5 (17,5–22) |
| 4 | 304 (147–577) | %16 (%11–%26) | 0 | 19,5 (16,5–22) |
| 5 | 311 (84,8–560) | %13 (%10–%24) | 0 | 19 (17–22) |
| 6 | 292 (126–554) | %14 (%9,3–%24) | 0 | 19,5 (17–22) |
| 7 | 345 (49,5–551) | %15 (%11–%24) | 0 | 20 (17–22) |
| 8 | 298 (72,4–617) | %15 (%12–%22) | 0 | 20 (17,5–22) |
| 9 | 280 (106–684) | %16 (%11–%24) | 0 | 20 (18–22,5) |
| 10 | 447 (83,8–649) | %16 (%11–%24) | 0 (0–0,5) | 20,5 (18–23,5) |
| 11 | 452 (98,7–643) | %15 (%13–%24) | 0 | 20,5 (17,5–23,5) |
| 12 | 321 (143–659) | %17 (%12–%25) | 0 | 21 (17,5–24) |
| 13 | 481 (113–769) | %16 (%11–%26) | 0 | 21 (18–25) |
| 14 | 395 (103–656) | %17 (%12–%26) | 0 | 21 (17,5–25) |
| 15 | 384 (161–692) | %17 (%12–%26) | 0 | 21 (18–24,5) |
| 16 | 519 (143–686) | %16 (%11–%25) | 0 | 21,5 (18–25) |
| 17 | 346 (120–652) | %16 (%11–%24) | 0 | 22 (17,5–25) |
| 18 | 386 (154–665) | %17 (%12–%25) | 0 | 22 (17,5–26) |
| 19 | 402 (243–781) | %16 (%11–%25) | 0 | 22,5 (17,5–26) |
| 20 | 364 (265–647) | %16 (%11–%25) | 0 | 22,5 (17–26) |
| 21 | 448 (221–685) | %16 (%9,9–%24) | 0 | 22,5 (17–26) |
| 22 | 416 (220–664) | %15 (%12–%24) | 0 | 22,5 (17–26) |
| 23 | 497 (174–643) | %15 (%11–%23) | 0 (0–0,5) | 23 (18–26,5) |
| 24 | 386 (230–623) | %15 (%11–%24) | 0 | 23 (18–26,5) |
| 25 | 411 (267–672) | %16 (%9,9–%25) | 0 | 23 (17,5–26,5) |
| 26 | 469 (304–746) | %15 (%11–%24) | 0 | 23 (17–27,5) |
| 27 | 410 (240–683) | %15 (%10–%23) | 0 | 23,5 (17–28,5) |
| 28 | 408 (199–761) | %15 (%11–%24) | 0 | 23,5 (17–28,5) |
| 29 | 421 (162–631) | %17 (%10–%22) | 0 | 24 (17,5–29) |
| 30 | 453 (205–697) | %16 (%11–%24) | 0 | 24 (18–29) |
| 31 | 481 (249–712) | %15 (%12–%23) | 0 | 24 (18,5–30) |
| 32 | 515 (289–619) | %15 (%11–%23) | 0 | 24 (18,5–30,5) |
| 33 | 477 (277–681) | %17 (%12–%24) | 0 | 24,5 (18,5–29,5) |
| 34 | 444 (285–649) | %17 (%11–%22) | 0 | 25 (18,5–30) |
| 35 | 496 (255–802) | %18 (%11–%23) | 0 | 25 (19–30) |
| 36 | 462 (185–778) | %17 (%11–%24) | 0 | 24,5 (19–30) |
| 37 | 443 (275–765) | %19 (%10–%23) | 0 | 24 (19,5–30) |
| 38 | 471 (215–722) | %15 (%11–%23) | 0 | 23,5 (20–30,5) |
| 39 | 633 (95,8–811) | %17 (%9,8–%23) | 0 | 23 (20–30,5) |
| 40 | 521 (130–703) | %15 (%10–%23) | 0 | 23,5 (20,5–30,5) |
| 41 | 444 (261–640) | %16 (%11–%23) | 0 | 23,5 (20,5–30,5) |
| 42 | 471 (252–656) | %16 (%10–%23) | 0 | 23,5 (20,5–30) |
| 43 | 503 (253–838) | %15 (%11–%24) | 0 | 23,5 (20,5–30) |
| 44 | 536 (199–862) | %16 (%9,5–%23) | 0 | 23,5 (20,5–30) |
| 45 | 492 (240–904) | %16 (%5,9–%23) | 0 | 23,5 (21–30) |
| 46 | 528 (288–1008) | %17 (%11–%23) | 0 | 23,5 (21–30) |
| 47 | 483 (198–745) | %17 (%11–%23) | 0 | 24 (21–29,5) |
| 48 | 504 (209–736) | %16 (%8,8–%23) | 0 | 24 (21–29,5) |
| 49 | 449 (141–775) | %17 (%12–%22) | 0 | 24,5 (21–30) |
| 50 | 450 (213–719) | %17 (%13–%24) | 0 | 24,5 (21–30) |
| 51 | 439 (287–663) | %18 (%9–%21) | 0 | 24,5 (21–30) |
| 52 | 454 (216–683) | %17 (%9,1–%23) | 0 | 24,5 (21,5–30) |
| 53 | 478 (173–763) | %16 (%11–%22) | 0 | 25 (21–30) |
| 54 | 430 (251–845) | %17 (%9,3–%22) | 0 | 25 (21–30) |
| 55 | 453 (310–757) | %17 (%10–%22) | 0 | 25 (20,5–30) |
| 56 | 461 (394–784) | %17 (%9,6–%23) | 0 | 24,5 (21–30) |
| 57 | 539 (368–825) | %17 (%11–%23) | 0 | 24,5 (21–30,5) |
| 58 | 535 (294–918) | %17 (%11–%21) | 0 | 24,5 (20,5–31) |
| 59 | 453 (232–986) | %17 (%11–%22) | 0 | 24,5 (20,5–31) |
| 60 | 519 (157–1046) | %16 (%12–%21) | 0 | 24,5 (21–31,5) |

### Olaylar

| Yıl | Olay | Büyük olay |
|---|---|---|
| 1 | 378 (236–862) | 83,5 (51,5–222) |
| 2 | 419 (250–853) | 96,5 (49–256) |
| 3 | 400 (234–800) | 97,5 (56–226) |
| 4 | 356 (228–788) | 86,5 (46–216) |
| 5 | 388 (252–731) | 99,5 (52–209) |
| 6 | 382 (256–738) | 90 (58,5–217) |
| 7 | 398 (258–700) | 89 (55,5–224) |
| 8 | 406 (278–728) | 113 (59–213) |
| 9 | 432 (252–705) | 108 (52–216) |
| 10 | 394 (272–795) | 92,5 (53–235) |
| 11 | 454 (280–856) | 104 (59,5–223) |
| 12 | 392 (266–810) | 97,5 (52–225) |
| 13 | 412 (279–908) | 91 (66,5–266) |
| 14 | 385 (284–800) | 88 (72–224) |
| 15 | 427 (286–822) | 100 (53,5–259) |
| 16 | 411 (294–802) | 95 (51,5–210) |
| 17 | 433 (310–734) | 92 (65–234) |
| 18 | 466 (278–796) | 95 (62–244) |
| 19 | 422 (304–876) | 88,5 (55,5–248) |
| 20 | 442 (262–906) | 88 (52–266) |
| 21 | 441 (266–790) | 102 (45–252) |
| 22 | 469 (260–856) | 89,5 (51–258) |
| 23 | 495 (290–835) | 97,5 (50,5–247) |
| 24 | 490 (276–787) | 100 (49–225) |
| 25 | 437 (294–854) | 108 (53–254) |
| 26 | 483 (278–874) | 88,5 (44–282) |
| 27 | 436 (296–844) | 92,5 (58,5–249) |
| 28 | 519 (289–785) | 108 (45–252) |
| 29 | 465 (270–881) | 88 (56,5–264) |
| 30 | 475 (273–832) | 92,5 (56,5–248) |
| 31 | 402 (297–814) | 76 (48–238) |
| 32 | 420 (264–829) | 88 (46–237) |
| 33 | 429 (307–837) | 87 (42–216) |
| 34 | 426 (330–872) | 82,5 (53–233) |
| 35 | 458 (320–838) | 102 (54–192) |
| 36 | 454 (274–861) | 96 (38,5–242) |
| 37 | 465 (268–952) | 97 (39,5–232) |
| 38 | 428 (268–962) | 82 (49,5–278) |
| 39 | 410 (287–932) | 86,5 (57,5–248) |
| 40 | 444 (272–906) | 90 (41,5–238) |
| 41 | 410 (262–917) | 81 (47–236) |
| 42 | 504 (296–935) | 104 (44,5–265) |
| 43 | 434 (263–890) | 84 (46–240) |
| 44 | 450 (246–924) | 95,5 (40–270) |
| 45 | 492 (250–989) | 113 (36,5–270) |
| 46 | 481 (250–830) | 116 (46,5–238) |
| 47 | 495 (276–964) | 103 (42–246) |
| 48 | 574 (290–893) | 132 (46–230) |
| 49 | 494 (265–856) | 99,5 (44,5–236) |
| 50 | 435 (298–888) | 90 (50–250) |
| 51 | 500 (302–955) | 110 (47,5–222) |
| 52 | 537 (293–896) | 117 (57–248) |
| 53 | 498 (260–836) | 115 (50–201) |
| 54 | 552 (288–788) | 144 (62–242) |
| 55 | 524 (232–850) | 120 (41–244) |
| 56 | 514 (290–808) | 108 (47–220) |
| 57 | 484 (286–872) | 101 (48,5–230) |
| 58 | 556 (263–784) | 119 (48,5–208) |
| 59 | 543 (245–838) | 108 (41,5–196) |
| 60 | 538 (278–802) | 124 (54–203) |

### Savaş (1/2)

| Yıl | Muharebe | Başlayan savaş | Süren savaş (yıl sonu) | Yıl içinde süren savaş | Yağma akını (medeniyet) | Tarihî hak savaşı |
|---|---|---|---|---|---|---|
| 1 | 11,5 (9–22,5) | 2 (0,5–3) | 1,5 (0–2,5) | 2 (0,5–4,5) | 1 (0–4) | 0 (0–1) |
| 2 | 15 (8,5–24) | 1 (0–3) | 0,5 (0–1,5) | 2,5 (0–4,5) | 2 (0–3,5) | 0 |
| 3 | 14,5 (7,5–17) | 2 (0–3) | 0 (0–1) | 2,5 (0,5–4) | 1,5 (0–3) | 0,5 (0–1) |
| 4 | 14 (9–18) | 2 (0,5–4) | 1 (0–2,5) | 2 (0,5–5) | 1 (0–5) | 0 (0–1) |
| 5 | 13,5 (8–20) | 2 (0–2,5) | 0 (0–2) | 2,5 (0–4,5) | 2 (0–5) | 0 |
| 6 | 12 (6,5–18,5) | 1,5 (0–4) | 1 (0–2) | 2 (0,5–5) | 1,5 (0–4) | 0 (0–0,5) |
| 7 | 12 (7,5–19) | 2 (0–3) | 0 (0–1,5) | 2,5 (1–4) | 1 (0–4) | 0 (0–1,5) |
| 8 | 13,5 (9–19) | 2 (0,5–4) | 1 (0–3) | 2 (2–4) | 1,5 (0–4) | 0 (0–1) |
| 9 | 14,5 (8–19,5) | 1 (0–2,5) | 0 (0–2) | 2 (1–4) | 1 (0–4) | 0 (0–1) |
| 10 | 12,5 (8–19) | 2 (0,5–4) | 1 (0–1,5) | 2 (1–4,5) | 1 (0–3) | 0 (0–1) |
| 11 | 14,5 (8–19,5) | 2 (0–3,5) | 0 (0–3) | 3 (0,5–4) | 2 (0–4) | 0 |
| 12 | 13,5 (8,5–20) | 2 (0–3) | 1 (0–2) | 2 (1–5,5) | 1,5 (0–5) | 0 (0–0,5) |
| 13 | 12 (8–24) | 2 (0–4) | 0,5 (0–2,5) | 3 (0,5–4) | 1,5 (0–4) | 0 (0–1) |
| 14 | 13 (8,5–19) | 1 (0–3) | 1 (0–2,5) | 2 (0–3,5) | 2 (0–4) | 0 (0–1) |
| 15 | 13,5 (7–21) | 2 (0–4) | 1 (0–2) | 2 (1–5,5) | 1 (0–3,5) | 0 (0–0,5) |
| 16 | 12,5 (7–18,5) | 1,5 (0–3) | 0 (0–2) | 2 (1–4) | 1 (0–3,5) | 0 |
| 17 | 13 (8,5–17) | 2 (0–3) | 1 (0–2) | 2 (1–3,5) | 1 (0–4,5) | 0 (0–0,5) |
| 18 | 12,5 (9–22) | 1,5 (0–3,5) | 1 (0–2) | 2,5 (1–4,5) | 1,5 (0–4,5) | 0 (0–1) |
| 19 | 12,5 (7,5–18) | 1,5 (0–2) | 0 (0–1,5) | 2,5 (0,5–3,5) | 1,5 (0–4,5) | 0 (0–0,5) |
| 20 | 11 (8–18,5) | 1 (0–5) | 0 (0–2) | 2 (0–5) | 1 (0–3,5) | 0 (0–1) |
| 21 | 14 (7,5–21) | 1 (0–2,5) | 1 (0–1) | 2 (0–3,5) | 1 (0–5) | 0 (0–1) |
| 22 | 12 (8–20) | 1,5 (0–4,5) | 0,5 (0–2,5) | 2 (1–4,5) | 1,5 (0–4) | 0 (0–1) |
| 23 | 13 (6–16,5) | 1,5 (0–3) | 1 (0–2) | 2 (0,5–4) | 1 (0–4) | 0 |
| 24 | 14 (8–20) | 2 (0–3,5) | 1 (0–2) | 2 (0,5–4,5) | 2 (0–5) | 0 (0–1) |
| 25 | 12,5 (9–17,5) | 1 (0–3) | 1 (0–2) | 2 (1–4,5) | 1 (0–3,5) | 0 (0–0,5) |
| 26 | 13 (6,5–20,5) | 0,5 (0–3,5) | 0,5 (0–2) | 2 (1–4,5) | 1 (0–3,5) | 0 (0–0,5) |
| 27 | 12 (8,5–17) | 2 (0–4) | 1 (0–2) | 2,5 (1–5) | 1 (0–4,5) | 0 |
| 28 | 12 (8–16) | 1 (0–2) | 1 (0–2) | 2 (0–4) | 1 (0–3,5) | 0 |
| 29 | 11 (6–19,5) | 1 (0–3,5) | 0 (0–1,5) | 2 (0,5–4,5) | 1 (0–3,5) | 0 |
| 30 | 11,5 (8–18,5) | 1 (0–3) | 0 (0–2) | 1,5 (0–3,5) | 1 (0–3,5) | 0 |
| 31 | 13 (8,5–19,5) | 1 (0–2,5) | 1 (0–1) | 2 (0,5–4) | 1 (0–3,5) | 0 (0–0,5) |
| 32 | 12 (6–16,5) | 1 (0–3,5) | 0,5 (0–2) | 2 (0,5–5) | 1 (0–3) | 0 |
| 33 | 11,5 (4,5–18) | 1 (0–2) | 1 (0–2) | 2 (0–3,5) | 1,5 (0–3) | 0 (0–1) |
| 34 | 11,5 (7,5–17,5) | 1 (0–3) | 1 (0–2) | 2 (0,5–4,5) | 1 (0–2) | 0 (0–1) |
| 35 | 12 (8–18) | 0 (0–3) | 0 (0–1,5) | 2 (0,5–4) | 1 (0–3) | 0 (0–0,5) |
| 36 | 11,5 (6–19,5) | 2 (0–2,5) | 0,5 (0–2) | 2 (0,5–4) | 1,5 (0–3,5) | 0 (0–1) |
| 37 | 13,5 (6–17) | 1 (0–2) | 0 (0–1) | 2 (0–4) | 2 (0–3,5) | 0 (0–0,5) |
| 38 | 10,5 (7,5–20) | 1 (0–2) | 0 (0–2) | 2 (0–3) | 1 (0–3) | 0 (0–1) |
| 39 | 13 (6,5–17,5) | 1 (0–4) | 0,5 (0–2,5) | 2 (0–4) | 2 (0–3) | 0 |
| 40 | 11,5 (6,5–19,5) | 1 (0–2) | 0 (0–2) | 2 (0–4,5) | 1,5 (0–4) | 0 |
| 41 | 12,5 (6,5–18) | 1 (0–2) | 1 (0–2,5) | 2 (0–3,5) | 1 (0–4) | 0 |
| 42 | 14,5 (8–21) | 1 (0–3) | 0 (0–2) | 2 (0,5–4) | 1,5 (0–3) | 0 (0–1) |
| 43 | 11 (5–18,5) | 1 (0–2,5) | 0,5 (0–2) | 2 (0–3) | 1 (0–3,5) | 0 |
| 44 | 11 (7,5–19) | 1 (0–2,5) | 0 (0–1,5) | 2 (0–3,5) | 1,5 (0–2,5) | 0 |
| 45 | 12 (6–21,5) | 1,5 (0–3) | 0 (0–1,5) | 2 (0–4) | 1 (0–3) | 0 (0–0,5) |
| 46 | 12,5 (7–16) | 0,5 (0–3) | 0 (0–2) | 1 (0–3) | 1 (0–2,5) | 0 (0–1) |
| 47 | 11,5 (7,5–20) | 1,5 (0–3) | 0 (0–2) | 2,5 (0–4) | 1 (0–2) | 0 (0–1) |
| 48 | 14 (7–18) | 0,5 (0–2,5) | 0,5 (0–2) | 2 (0–3,5) | 1 (0–3,5) | 0 |
| 49 | 12 (7–17) | 1 (0–2) | 0,5 (0–2) | 2 (0–3,5) | 1 (0–3) | 0 |
| 50 | 11,5 (7–17) | 1 (0–2) | 0,5 (0–1,5) | 2 (0,5–3) | 1 (0–3) | 0 (0–0,5) |
| 51 | 11,5 (6–18) | 1 (0–3) | 1 (0–2) | 2 (0–3,5) | 1 (0–2,5) | 0 (0–0,5) |
| 52 | 11,5 (7–19) | 1 (0–2,5) | 0 (0–2) | 1,5 (0,5–4) | 1 (0–2,5) | 0 |
| 53 | 11,5 (7–16) | 1 (0–2,5) | 0 (0–1) | 1 (0–3) | 1 (0–2,5) | 0 (0–1) |
| 54 | 13,5 (7–19) | 0,5 (0–2,5) | 0 (0–2) | 1,5 (0–3) | 0,5 (0–1,5) | 0 (0–0,5) |
| 55 | 11 (5,5–16,5) | 0 (0–2) | 0 (0–1,5) | 1,5 (0–3) | 1 (0–3) | 0 |
| 56 | 10,5 (7–17,5) | 1 (0–2) | 0,5 (0–2) | 2 (0–2) | 1 (0–2,5) | 0 (0–1) |
| 57 | 11,5 (6,5–19,5) | 1 (0–2) | 0,5 (0–2) | 2 (0–3) | 1 (0–2,5) | 0 (0–1) |
| 58 | 12,5 (7–15,5) | 1 (0–3) | 1 (0–2) | 2 (0,5–3,5) | 1 (0–2,5) | 0 |
| 59 | 10 (5–15,5) | 0,5 (0–2) | 0 (0–1,5) | 1 (0–3) | 1 (0–2,5) | 0 |
| 60 | 11 (6,5–16,5) | 1 (0–3) | 1 (0–1) | 2 (0,5–3,5) | 1 (0–2,5) | 0 (0–0,5) |

### Savaş (2/2)

| Yıl | Pakt gereği savaş | Kutsal Sefer çağrısı | İhanet (pakt çiğnendi) | Savunma paktı (yıl sonu) |
|---|---|---|---|---|
| 1 | 0 (0–1) | 0 | 0 | 1 (0–1) |
| 2 | 0 (0–1) | 0 | 0 | 1 (0–1) |
| 3 | 0 | 0 | 0 | 1 (0–1) |
| 4 | 0 (0–1) | 0 (0–1) | 0 | 1 (0–2) |
| 5 | 0 | 0 | 0 | 1 (0–2) |
| 6 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 7 | 0 (0–1) | 0 | 0 | 1 (0–1,5) |
| 8 | 0 (0–0,5) | 0 | 0 | 1 (0–1,5) |
| 9 | 0 (0–0,5) | 0 | 0 | 1 (0–1,5) |
| 10 | 0 (0–0,5) | 0 (0–1) | 0 | 1 (0–2) |
| 11 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 12 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 13 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 14 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 15 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 16 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 17 | 0 (0–1) | 0 (0–0,5) | 0 | 1 (0–2) |
| 18 | 0 (0–1) | 0 (0–0,5) | 0 | 1 (0–2) |
| 19 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 20 | 0 (0–1,5) | 0 | 0 | 1 (0–2) |
| 21 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 22 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 23 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 24 | 0 (0–1) | 0 (0–0,5) | 0 | 1 (0–2) |
| 25 | 0 (0–1) | 0 (0–0,5) | 0 | 1 (0–2) |
| 26 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 27 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 28 | 0 (0–0,5) | 0 (0–0,5) | 0 | 1 (0–2) |
| 29 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 30 | 0 (0–1) | 0 (0–0,5) | 0 | 1 (0–2) |
| 31 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 32 | 0 (0–1) | 0 | 0 (0–0,5) | 1 (0–2) |
| 33 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 34 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 35 | 0 (0–1) | 0 (0–0,5) | 0 (0–0,5) | 1 (0–2) |
| 36 | 0 (0–0,5) | 0 (0–0,5) | 0 | 1 (0–2) |
| 37 | 0 (0–1) | 0 (0–0,5) | 0 | 1 (0–2) |
| 38 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 39 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 40 | 0 (0–0,5) | 0 (0–0,5) | 0 | 1 (0–2) |
| 41 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 42 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 43 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 44 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 45 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 46 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 47 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 48 | 0 | 0 | 0 | 1 (0–2) |
| 49 | 0 | 0 (0–0,5) | 0 | 1 (0–2) |
| 50 | 0 | 0 | 0 | 1 (0–2) |
| 51 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 52 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 53 | 0 (0–0,5) | 0 | 0 | 1 (0–1,5) |
| 54 | 0 | 0 | 0 | 1 (0–1,5) |
| 55 | 0 (0–1) | 0 | 0 | 1 (1–1,5) |
| 56 | 0 (0–0,5) | 0 | 0 | 1 (1–1,5) |
| 57 | 0 | 0 | 0 | 1 (0–1,5) |
| 58 | 0 (0–1) | 0 (0–1) | 0 | 1 (0–1,5) |
| 59 | 0 (0–0,5) | 0 | 0 | 1 (0–1,5) |
| 60 | 0 (0–1) | 0 | 0 | 1 (0–1,5) |

### Canavarlar (1/2)

| Yıl | Yaşayan kamp (yıl sonu) | Yaşayan kamp (yıl ort.) | Doğan kamp | Temizlenen kamp | Canavar baskını | Yaşayan trol ini (yıl sonu) |
|---|---|---|---|---|---|---|
| 1 | 9 (8,5–10) | 9,26 (8,28–10,3) | 3 (1–10,5) | 3,5 (0,5–10) | 4 (2,5–7) | 2 (1–3) |
| 2 | 9 (8–10) | 9,15 (8,28–9,95) | 3,5 (1–9) | 4 (1,5–9) | 4 (2–6,5) | 2 (1–3) |
| 3 | 10 (8,5–10,5) | 9,46 (8,43–10,4) | 4,5 (1–9,5) | 3,5 (0–9) | 4 (2–6) | 2 (1–3) |
| 4 | 10 (8–11) | 9,41 (8,38–10,4) | 3,5 (0,5–9) | 4 (0–10) | 4 (3–6) | 2 (1–3) |
| 5 | 9,5 (8–10,5) | 9,21 (8,18–11) | 3,5 (0,5–8,5) | 4 (0,5–8,5) | 4,5 (2–6) | 2 (1–3) |
| 6 | 9,5 (8–11) | 9,09 (8,06–10,9) | 3,5 (0–9) | 3,5 (0,5–8,5) | 3 (2–7) | 2 (1–3) |
| 7 | 10 (9–11) | 9,43 (8,44–11) | 4 (0–11) | 3,5 (0–9) | 4 (2,5–7,5) | 2 (2–3) |
| 8 | 9,5 (9–10,5) | 9,31 (8,93–10,8) | 4,5 (0,5–9) | 4,5 (1–9) | 5 (3–7,5) | 2,5 (1–3) |
| 9 | 10 (8–11) | 9,3 (8,5–10,9) | 3,5 (1–8,5) | 4 (0–9) | 4 (2–7,5) | 2 (1–3) |
| 10 | 10 (8,5–11) | 9,21 (8,3–11,2) | 3,5 (0–10,5) | 3 (0–9) | 4 (1,5–7,5) | 2 (1–3) |
| 11 | 9 (8,5–10,5) | 9,11 (8,35–11) | 2,5 (0–9) | 4 (0,5–10,5) | 4 (2–9,5) | 2,5 (1,5–3) |
| 12 | 9 (8–10) | 8,99 (8,24–9,96) | 3 (0,5–8) | 3 (0–8,5) | 5 (1,5–7) | 2,5 (2–3) |
| 13 | 10 (8,5–10,5) | 9,81 (8,78–10) | 4 (0–9,5) | 3,5 (0–9,5) | 5 (1,5–7,5) | 3 (1–3) |
| 14 | 9,5 (8–11) | 9,25 (8,46–10,5) | 3,5 (0–8) | 4 (0–8) | 4 (2–7) | 3 (2–3) |
| 15 | 9 (8–10) | 9,2 (8,23–10,7) | 3,5 (0–9) | 4 (0–9) | 5 (2,5–7) | 3 (2–3) |
| 16 | 9 (8–10) | 9,14 (8,24–10) | 3,5 (0–9,5) | 4 (0–9) | 4 (1,5–6,5) | 3 (2–3) |
| 17 | 9 (8–10,5) | 9,33 (8,36–10,4) | 3,5 (1–9) | 3,5 (1–9) | 5 (2,5–5,5) | 3 (1,5–3) |
| 18 | 9 (8–10) | 9,4 (8,3–10,3) | 3 (0–11) | 3 (0,5–11) | 5 (2,5–6,5) | 3 (1,5–3) |
| 19 | 9 (8–10,5) | 9,2 (8,5–9,85) | 4 (0,5–9,5) | 3,5 (0,5–9,5) | 3 (2,5–6) | 2,5 (0,5–3) |
| 20 | 9 (8,5–10) | 9,33 (8,71–10,2) | 2 (0–9) | 2,5 (1–8) | 4 (1,5–6) | 2 (2–3) |
| 21 | 9,5 (8–10) | 9,29 (8,28–10,1) | 3,5 (0,5–9,5) | 3 (0–10) | 6 (2,5–7) | 3 (2–3) |
| 22 | 9,5 (8–10) | 9,29 (8,1–9,96) | 2 (0,5–10) | 2 (0–10) | 4,5 (2–6) | 3 (2–3) |
| 23 | 9,5 (8,5–11) | 9,58 (8,3–10,7) | 2 (0–10) | 1,5 (0–10,5) | 4 (2–6) | 2 (2–3) |
| 24 | 10 (8,5–10,5) | 9,45 (8,66–11) | 2,5 (0–10) | 2,5 (0–9,5) | 5 (2–6,5) | 3 (1–3) |
| 25 | 10 (8,5–11) | 9,54 (8,6–10,8) | 3,5 (0–10) | 2,5 (0–9,5) | 3,5 (2–6) | 3 (1,5–3) |
| 26 | 10 (8,5–10,5) | 9,76 (8,33–10,9) | 2,5 (0–9) | 3 (0–10) | 5 (1–7) | 3 (1,5–3) |
| 27 | 10 (8–11) | 9,78 (8,25–10,6) | 3 (0,5–8,5) | 2,5 (0–9) | 5 (1,5–7) | 3 (1–3) |
| 28 | 9,5 (8–11) | 9,49 (7,93–11) | 3 (0–9) | 2,5 (0,5–9) | 4 (1,5–7) | 3 (2–3) |
| 29 | 9,5 (8–11,5) | 9,33 (8,05–11,2) | 2 (0–9,5) | 2 (0–10) | 4 (2–7) | 2 (1,5–3) |
| 30 | 9 (8–11) | 9,45 (8,21–10,8) | 2 (0,5–9) | 3 (0–8,5) | 4 (2–5,5) | 3 (2–3) |
| 31 | 9 (8–10,5) | 8,99 (8,05–10,5) | 1,5 (0–10) | 2 (0–10) | 4,5 (1–7) | 2 (1–3) |
| 32 | 9 (8,5–11) | 9,09 (8,09–10,8) | 2,5 (0–9,5) | 1,5 (0–10) | 4 (2–6) | 3 (1,5–3) |
| 33 | 9 (8–11) | 9,08 (8,11–11) | 2 (0–9) | 2,5 (0–9,5) | 4 (2–7) | 3 (2–3) |
| 34 | 9,5 (8–11) | 9,16 (7,84–10,8) | 2,5 (0–10) | 2,5 (0–9,5) | 4 (1–8,5) | 3 (2–3) |
| 35 | 9 (8–11) | 9,56 (7,56–11) | 3 (0–7,5) | 3 (0–8) | 4 (2–6) | 3 (1,5–3) |
| 36 | 9 (7,5–12) | 9,2 (8,04–11,6) | 2,5 (0,5–9,5) | 3 (0–9) | 3 (2–4,5) | 3 (2–3) |
| 37 | 9 (7,5–11) | 9,04 (8,05–12,1) | 3 (0,5–8,5) | 2,5 (0–9,5) | 4 (2–5,5) | 3 (1–3) |
| 38 | 9 (8,5–11) | 9,05 (8,09–11) | 2,5 (0–9) | 3 (0–8) | 4 (1,5–5,5) | 3 (1,5–3) |
| 39 | 9 (8–11) | 9,21 (8,08–11,2) | 2 (0–9,5) | 3 (0–10) | 3 (2–6,5) | 3 (2–3) |
| 40 | 10 (8–12) | 9,3 (8,01–11,3) | 3,5 (1–10,5) | 3,5 (0–9) | 4 (1,5–6) | 3 (2–3) |
| 41 | 9 (8–11) | 9,45 (7,81–11,2) | 3 (0–9) | 3 (0–10,5) | 4 (2,5–7) | 3 (2–3) |
| 42 | 9 (8–11) | 9,06 (7,93–11,2) | 2,5 (0–10,5) | 3,5 (0–11) | 4 (3–7,5) | 3 (1,5–3) |
| 43 | 9 (8–12) | 8,84 (7,78–11,5) | 4 (0,5–9) | 3,5 (0–9,5) | 4 (2–6,5) | 3 (2–3) |
| 44 | 8,5 (7,5–11) | 8,79 (7,93–11,1) | 2,5 (0–9) | 3 (1–9,5) | 3,5 (2–5,5) | 3 (1,5–3) |
| 45 | 9 (8–11) | 8,75 (7,88–11) | 4,5 (0–9) | 4 (0–8,5) | 3,5 (1,5–5,5) | 3 (2–3) |
| 46 | 9 (8–11) | 8,99 (7,9–11) | 3,5 (0,5–8,5) | 4 (0–8) | 4 (1–6,5) | 3 |
| 47 | 9 (6,5–10,5) | 8,9 (7,81–10,8) | 3,5 (0–9,5) | 3 (1–10) | 4 (2–5,5) | 3 (1–3) |
| 48 | 9 (7,5–11) | 9,11 (7,64–10,5) | 4,5 (0–10) | 5 (0–9,5) | 4 (2–5) | 3 (2–3) |
| 49 | 9,5 (8–10,5) | 9,19 (7,91–10,9) | 3,5 (0,5–10) | 3 (0,5–9) | 3 (1–5) | 3 (1,5–3) |
| 50 | 9 (8–10,5) | 9,29 (8,03–10,9) | 3,5 (0,5–9) | 3,5 (1–10) | 4 (2–6) | 3 (2–3) |
| 51 | 9 (7,5–10,5) | 8,88 (8,25–10,5) | 4 (0,5–9) | 4,5 (1–9) | 3 (1–5,5) | 3 (2–3) |
| 52 | 9 (8–11) | 8,41 (7,73–10,5) | 4 (0,5–10,5) | 4 (0–10) | 4 (1–7) | 3 (2–3) |
| 53 | 9 (8–10,5) | 9,04 (7,75–10,5) | 3 (0,5–8,5) | 4 (0,5–8) | 3 (1,5–5) | 3 |
| 54 | 9 (8–10,5) | 8,84 (7,8–10,7) | 3,5 (0,5–9) | 3,5 (1–10) | 3 (2–5) | 3 (1,5–3) |
| 55 | 9,5 (7,5–10,5) | 8,76 (7,78–10,2) | 4 (0,5–8,5) | 3,5 (0,5–8,5) | 3 (0,5–5) | 3 (2–3) |
| 56 | 9 (8–11) | 8,88 (7,84–10,3) | 4 (2–9,5) | 5 (1–8,5) | 3 (0,5–5) | 3 (2–3) |
| 57 | 9 (7–9,5) | 9 (8,09–9,95) | 4,5 (0–7,5) | 4 (0,5–8,5) | 4 (1–6) | 3 (2–3) |
| 58 | 9 (8–10) | 9 (7,84–9,68) | 4 (0–9) | 4 (0–7,5) | 3 (1–6,5) | 3 (2,5–3) |
| 59 | 9 (7–10) | 8,71 (7,89–9,74) | 3 (1–7,5) | 3,5 (0–8,5) | 3 (1–4,5) | 3 (2–3) |
| 60 | 9 (8–11) | 9,09 (7,9–10,3) | 4,5 (0,5–8,5) | 4,5 (0–7,5) | 4 (1–5) | 3 (2,5–3) |

### Canavarlar (2/2)

| Yıl | Yaşayan ejderha (yıl sonu) | Ejderha akını | Kriz (anlatıcı) | Rahatlama dönemi (anlatıcı) |
|---|---|---|---|---|
| 1 | 1 (0–1) | 1 (0–2) | 0 (0–1) | 0 |
| 2 | 1 (0–1) | 1 (0–2) | 0 (0–1) | 0 (0–1) |
| 3 | 1 (0–1) | 1 (0–2) | 0 (0–1) | 0 (0–1) |
| 4 | 1 (0–1) | 0,5 (0–2) | 0 (0–1) | 0 |
| 5 | 1 (0–1) | 1 (0–2) | 0 (0–1) | 0 |
| 6 | 1 (0–1) | 1 (0–2) | 0 (0–1) | 0 |
| 7 | 1 (0–1) | 0 (0–2) | 0 (0–1) | 0 |
| 8 | 1 (0–1) | 1 (0–1,5) | 1 (0–1) | 0 |
| 9 | 1 (0–1) | 1 (0–2) | 0 (0–1) | 0 (0–1) |
| 10 | 1 (0–1) | 1 (0–2) | 0 (0–1) | 0 |
| 11 | 1 (0–1) | 1,5 (0–2) | 0,5 (0–1) | 0 (0–1) |
| 12 | 1 (0–1) | 1 (0–2) | 0 (0–1) | 0 (0–1) |
| 13 | 1 (0–1) | 0 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 14 | 1 (0–1) | 1 (0–2) | 0 (0–1) | 0 (0–1) |
| 15 | 1 (0–1) | 0,5 (0–2) | 0 (0–1) | 0 |
| 16 | 1 (0–1) | 0,5 (0–2) | 0 (0–1) | 0 (0–1) |
| 17 | 1 (0–1) | 1 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 18 | 1 (0–1) | 1 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 19 | 1 (0–1) | 1 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 20 | 1 (0–1) | 1 (0–2) | 0 (0–1) | 0 |
| 21 | 1 (0–1) | 0,5 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 22 | 1 (0–1) | 0,5 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 23 | 1 (0–1) | 1 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 24 | 1 (0–1) | 1 (0–2) | 0 (0–1) | 0 (0–1) |
| 25 | 1 (0–1) | 0,5 (0–1,5) | 0 (0–1) | 0 (0–0,5) |
| 26 | 1 (0–1) | 1 (0–2) | 0 (0–1) | 0 (0–1) |
| 27 | 1 (0–1) | 0,5 (0–2) | 0 (0–1) | 0 |
| 28 | 1 (0–1) | 0,5 (0–2) | 1 (0–1) | 0 |
| 29 | 0,5 (0–1) | 1 (0–1,5) | 0 (0–1) | 0 (0–1) |
| 30 | 0,5 (0–1) | 0 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 31 | 0,5 (0–1) | 0,5 (0–2) | 0,5 (0–1) | 0 |
| 32 | 0,5 (0–1) | 0 (0–1,5) | 0 (0–1) | 0 (0–0,5) |
| 33 | 0,5 (0–1) | 0,5 (0–2) | 1 (0–1) | 0 |
| 34 | 0 (0–1) | 0 (0–2) | 1 (0–1) | 0 |
| 35 | 0 (0–1) | 0 (0–2) | 0 (0–1) | 0 (0–1) |
| 36 | 0 (0–1) | 0 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 37 | 0 (0–1) | 0 (0–1,5) | 0 (0–1) | 0 (0–1) |
| 38 | 0 (0–1) | 0 (0–2) | 0,5 (0–1) | 0 |
| 39 | 0 (0–1) | 0 (0–2) | 0,5 (0–1) | 0 (0–0,5) |
| 40 | 0 (0–1) | 0 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 41 | 0 (0–1) | 0 (0–2) | 0 (0–1) | 0 |
| 42 | 0 (0–1) | 0 (0–2) | 0,5 (0–1) | 0 (0–1) |
| 43 | 0 (0–1) | 0 (0–1) | 1 (0–1) | 0 |
| 44 | 0 (0–1) | 0 (0–1,5) | 0 (0–1) | 0 (0–0,5) |
| 45 | 0 (0–1) | 0 (0–1) | 1 (0–1) | 0 |
| 46 | 0 (0–1) | 0 (0–2) | 1 (0–1) | 0 |
| 47 | 0 (0–1) | 0 (0–1) | 0 (0–1) | 0 (0–0,5) |
| 48 | 0 (0–1) | 0 (0–1,5) | 0,5 (0–1) | 0 (0–0,5) |
| 49 | 0 (0–1) | 0 (0–2) | 0 (0–1) | 0 (0–1) |
| 50 | 0 (0–1) | 0 (0–1,5) | 0,5 (0–1) | 0 |
| 51 | 0 (0–1) | 0 (0–1) | 0,5 (0–1) | 0 |
| 52 | 0 (0–1) | 0 (0–2) | 1 (0–1) | 0 |
| 53 | 0 (0–1) | 0 (0–2) | 1 (0–1) | 0 |
| 54 | 0 (0–1) | 0 (0–2) | 0 (0–1) | 0 (0–1) |
| 55 | 0 (0–1) | 0 (0–1) | 0 (0–1) | 0 (0–1) |
| 56 | 0 (0–1) | 0 (0–0,5) | 1 (0–1) | 0 |
| 57 | 0 (0–1) | 0 (0–1) | 1 (0–1) | 0 (0–0,5) |
| 58 | 0 (0–1) | 0 (0–1,5) | 1 (0–1) | 0 (0–1) |
| 59 | 0 (0–1) | 0 (0–1) | 1 (0–1) | 0 |
| 60 | 0 (0–1) | 0 | 1 (0–1) | 0 (0–0,5) |

### Kahramanlar (1/2)

| Yıl | Doğan kahraman | Ölen kahraman | Emekli olan kahraman | Diyarı terk eden kahraman | Ölümden dönen kahraman | Efsane olan kahraman |
|---|---|---|---|---|---|---|
| 1 | 11 (6,5–16) | 3 (0–8,5) | 1 (0–3) | 4 (2,5–10) | 0 | 0 |
| 2 | 9 (6–18,5) | 4,5 (3–11,5) | 1 (0–2,5) | 5 (1,5–8,5) | 0 | 0 (0–1) |
| 3 | 10 (5,5–18) | 3,5 (0–11,5) | 0 (0–3) | 4 (2–7,5) | 0 | 0 |
| 4 | 11 (4,5–14) | 2,5 (0–8) | 0,5 (0–2) | 3 (0,5–12) | 0 | 0 |
| 5 | 10,5 (6–17) | 3,5 (0–6) | 1 (0–2) | 4,5 (1–9) | 0 | 0 (0–0,5) |
| 6 | 10 (5–16,5) | 2,5 (0–7,5) | 1 (0–3) | 4 (2–9,5) | 0 | 0 |
| 7 | 10,5 (5,5–15) | 2 (0–6) | 1 (0–3) | 4 (1,5–7) | 0 | 0 |
| 8 | 10,5 (5–18) | 1,5 (0–7,5) | 1 (0–3,5) | 4,5 (3–8,5) | 0 | 0 |
| 9 | 9,5 (5–15) | 5,5 (0–7,5) | 1 (0–3) | 4 (2–8) | 0 | 0 |
| 10 | 9 (5,5–17,5) | 3 (0,5–8,5) | 1 (0–2,5) | 4 (1,5–7) | 0 | 0 |
| 11 | 10 (6,5–17) | 4 (0,5–8) | 1 (0–3,5) | 3 (2–8,5) | 0 | 0 |
| 12 | 11,5 (6,5–17,5) | 4 (1–12) | 1 (0–2,5) | 3,5 (1,5–9) | 0 | 0 |
| 13 | 9,5 (6,5–13) | 4 (0,5–8,5) | 1 (0–2) | 4 (2–7) | 0 | 0 |
| 14 | 10 (6–18,5) | 5,5 (3–9) | 1 (0–2,5) | 4 (2–12) | 0 | 0 |
| 15 | 10,5 (9–15) | 5 (0,5–9) | 0,5 (0–1,5) | 3,5 (1,5–10) | 0 | 0 |
| 16 | 10 (4,5–16,5) | 3 (0–8,5) | 1 (0–3) | 4 (1–8,5) | 0 | 0 |
| 17 | 11 (4,5–16) | 2,5 (0–7,5) | 1,5 (0–3) | 3 (1–5,5) | 0 | 0 |
| 18 | 9,5 (7–13,5) | 2 (1–7,5) | 1 (0–2,5) | 4,5 (1–10) | 0 | 0 (0–0,5) |
| 19 | 11 (6,5–15,5) | 3,5 (1,5–7) | 1 (0–2) | 4 (2–7,5) | 0 | 0 |
| 20 | 12 (6,5–16,5) | 3 (0,5–8) | 1 (0–2) | 4,5 (2–9) | 0 | 0 |
| 21 | 10,5 (6–15) | 2 (0,5–7) | 1 (0–3) | 5,5 (2–9,5) | 0 | 0 |
| 22 | 8 (5–15) | 5 (0–12) | 1 (0–3,5) | 4,5 (2,5–8) | 0 | 0 |
| 23 | 10,5 (5–16,5) | 3,5 (1–8,5) | 1 (0–1,5) | 5 (2–8,5) | 0 | 0 |
| 24 | 10,5 (6–20,5) | 5 (0,5–9) | 1 (0–2,5) | 5 (2–9,5) | 0 | 0 (0–0,5) |
| 25 | 11,5 (6,5–14) | 3,5 (1,5–11) | 1,5 (0–2) | 4 (1–8) | 0 | 0 |
| 26 | 10,5 (7,5–16,5) | 5 (2–8) | 1 (0–3) | 4 (2–8) | 0 | 0 (0–0,5) |
| 27 | 10 (4,5–16) | 4,5 (0,5–7,5) | 1,5 (0,5–4) | 3,5 (1–10) | 0 | 0 |
| 28 | 13 (4,5–18) | 4,5 (1–11,5) | 1,5 (0–2,5) | 4,5 (2,5–15) | 0 | 0 |
| 29 | 10 (5,5–15) | 4 (1–7,5) | 2 (1–3) | 4,5 (2–9) | 0 | 0 |
| 30 | 10,5 (5,5–19) | 5 (0,5–10) | 2 (0–5) | 4 (2,5–10,5) | 0 | 0 |
| 31 | 10,5 (7–16,5) | 4 (0–8,5) | 1 (0–4) | 4,5 (2–11,5) | 0 | 0 |
| 32 | 10,5 (5,5–16) | 2 (0–9) | 1 (0–3,5) | 4 (2–8) | 0 | 0 |
| 33 | 12,5 (8–16) | 3 (1–5) | 1 (0–2) | 4 (1,5–8,5) | 0 | 0 (0–0,5) |
| 34 | 9,5 (6,5–18,5) | 4 (2–7,5) | 1 (0–2,5) | 4 (1,5–9) | 0 | 0 |
| 35 | 9 (5–17,5) | 5 (1,5–7,5) | 1 (0–4) | 6 (1–9) | 0 | 0 |
| 36 | 10,5 (5–15) | 4 (1–11) | 1 (0–2) | 4 (1,5–8,5) | 0 | 0 (0–1) |
| 37 | 10,5 (5,5–19) | 2 (0,5–8) | 1 (0–3) | 5 (2–11) | 0 | 0 |
| 38 | 10,5 (3–15) | 2,5 (1–10,5) | 1 (0–5) | 4 (1,5–8,5) | 0 | 0 |
| 39 | 10,5 (6–17) | 4 (1–9) | 1 (0–3) | 4,5 (2–8,5) | 0 | 0 (0–0,5) |
| 40 | 9,5 (6,5–16,5) | 3,5 (1–7) | 2 (0–3,5) | 5,5 (1,5–9,5) | 0 | 0 |
| 41 | 8,5 (4–19) | 3 (0,5–5,5) | 1 (0–4) | 5 (2–14,5) | 0 | 0 |
| 42 | 11 (5–18,5) | 4,5 (1,5–8,5) | 1 (0–3) | 3,5 (2–9,5) | 0 | 0 |
| 43 | 10 (5–14,5) | 5,5 (0,5–8) | 2 (1–3) | 3,5 (2–9) | 0 | 0 |
| 44 | 9,5 (4,5–15,5) | 2,5 (1–6) | 0,5 (0–4) | 4 (2–9,5) | 0 | 0 (0–0,5) |
| 45 | 11 (3,5–15) | 4 (0–7) | 2 (0,5–3,5) | 4,5 (2–7,5) | 0 | 0 |
| 46 | 10 (6,5–17,5) | 4,5 (2–10) | 1,5 (0–3,5) | 4 (1,5–12,5) | 0 | 0 |
| 47 | 11 (3,5–17) | 2,5 (0–7,5) | 1 (0–2,5) | 4 (1–10,5) | 0 | 0 |
| 48 | 10 (6–16,5) | 5 (1,5–10) | 2 (0–4) | 5,5 (1,5–8,5) | 0 | 0 |
| 49 | 9,5 (6–16,5) | 4 (2–7) | 1 (0–3) | 4 (1–8) | 0 | 0 |
| 50 | 8,5 (5,5–16) | 3,5 (1–11) | 1 (0–3) | 4 (1,5–9) | 0 | 0 |
| 51 | 10,5 (6–16,5) | 4 (0–6) | 1,5 (0–5) | 3 (1,5–11) | 0 | 0 |
| 52 | 10,5 (5,5–15) | 3,5 (1–11) | 1 (0–5) | 5,5 (1,5–8,5) | 0 | 0 |
| 53 | 10 (5–15,5) | 1,5 (0–7) | 3 (1–4,5) | 4 (0,5–7) | 0 | 0 |
| 54 | 9,5 (6–16) | 8 (2–15,5) | 2 (0–5,5) | 4,5 (3–9) | 0 | 0 (0–0,5) |
| 55 | 8 (6–15,5) | 4 (0,5–7,5) | 1,5 (0–4) | 3,5 (1,5–9) | 0 | 0 |
| 56 | 10 (7–16,5) | 3,5 (1,5–6) | 1 (0–3) | 3 (2–7,5) | 0 | 0 (0–1) |
| 57 | 10,5 (5–15,5) | 3 (0,5–9,5) | 1 (0–3) | 5 (1–9) | 0 | 0 |
| 58 | 12 (7–14,5) | 4 (1,5–6,5) | 1 (0–2,5) | 4 (1,5–7,5) | 0 | 0 |
| 59 | 11,5 (5–19) | 2 (0,5–6) | 1 (0–4,5) | 4 (2–10,5) | 0 | 0 |
| 60 | 9,5 (6,5–15,5) | 4 (0,5–7,5) | 1,5 (0,5–5) | 3,5 (1–10,5) | 0 | 0 |

### Kahramanlar (2/2)

| Yıl | Yaşayan kahraman (yıl sonu) | Doğuş seviyesi (ort.) | Ölüm seviyesi (ort.) | Yaşayan kahraman seviyesi (ort.) | En yüksek seviye (şimdiye dek) |
|---|---|---|---|---|---|
| 1 | 97,5 (63,5–158) | 1,53 (1,31–1,8) | 3,14 (2,26–4,27) | 3,65 (2,87–4,6) | 10 (9–10) |
| 2 | 93,5 (63–154) | 1,51 (1,29–1,84) | 3,52 (2–5) | 3,62 (2,97–4,67) | 10 (9–10) |
| 3 | 91 (61–156) | 1,69 (1,37–1,89) | 3,43 (1,87–4,65) | 3,55 (3,01–4,71) | 10 (9–10) |
| 4 | 91,5 (64–164) | 1,48 (1,33–1,78) | 3 (2–4,13) | 3,58 (2,93–4,8) | 10 (9–10) |
| 5 | 93,5 (64,5–165) | 1,53 (1,27–1,79) | 2,83 (2,25–4,83) | 3,55 (3,02–4,79) | 10 (9–10) |
| 6 | 95 (62,5–164) | 1,44 (1,32–1,73) | 3 (1–4,78) | 3,53 (3,03–4,81) | 10 (9–10) |
| 7 | 99 (64–164) | 1,57 (1,32–1,75) | 4 (2,73–7) | 3,45 (3,04–4,86) | 10 (9–10) |
| 8 | 102 (63,5–171) | 1,54 (1,25–1,81) | 2,73 (1,7–4,75) | 3,49 (3,09–4,75) | 10 (9–10) |
| 9 | 101 (63,5–171) | 1,5 (1,25–1,81) | 3,83 (3–5,3) | 3,44 (3,05–4,86) | 10 (9,5–10) |
| 10 | 102 (63–178) | 1,65 (1,17–1,82) | 4,38 (1,83–5,35) | 3,53 (3,04–4,89) | 10 (9,5–10) |
| 11 | 105 (63,5–176) | 1,58 (1,29–1,87) | 3,19 (3–4,9) | 3,58 (2,97–4,78) | 10 |
| 12 | 109 (65–174) | 1,65 (1,36–1,83) | 3,21 (2,42–5,2) | 3,5 (3–4,79) | 10 |
| 13 | 108 (65,5–170) | 1,51 (1,35–1,88) | 3,9 (2,25–5,32) | 3,44 (2,99–4,88) | 10 |
| 14 | 105 (65–168) | 1,58 (1,3–1,95) | 4,18 (2,42–4,89) | 3,52 (2,99–4,82) | 10 |
| 15 | 107 (69,5–170) | 1,56 (1,27–1,75) | 4,1 (2,53–4,58) | 3,55 (2,89–4,9) | 10 |
| 16 | 110 (73,5–180) | 1,6 (1,13–1,96) | 4,15 (3,03–5,76) | 3,55 (2,91–4,98) | 10 |
| 17 | 115 (75–185) | 1,61 (1,42–1,88) | 4,67 (2,5–7,2) | 3,52 (2,96–4,99) | 10 |
| 18 | 104 (77–188) | 1,58 (1,4–1,81) | 4,25 (2,13–7,6) | 3,59 (3–5,07) | 10 |
| 19 | 112 (78,5–191) | 1,59 (1,3–1,8) | 4,14 (2,75–6,83) | 3,52 (3,03–5,01) | 10 |
| 20 | 114 (79,5–194) | 1,43 (1,33–1,7) | 4,25 (2,28–5,54) | 3,54 (3,03–4,93) | 10 |
| 21 | 114 (79,5–196) | 1,54 (1,31–1,83) | 3,95 (2,83–5,35) | 3,57 (3,05–5,01) | 10 |
| 22 | 112 (77,5–192) | 1,52 (1,17–1,75) | 4,33 (3,6–5,96) | 3,52 (3,13–5,05) | 10 |
| 23 | 116 (79–192) | 1,43 (1,33–1,71) | 4,13 (2,5–6,83) | 3,52 (3,17–4,92) | 10 |
| 24 | 113 (79–194) | 1,6 (1,37–1,86) | 4 (3,04–4,75) | 3,42 (3,17–4,9) | 10 |
| 25 | 115 (81–186) | 1,48 (1,33–1,63) | 3,56 (2,57–4,4) | 3,43 (3,17–4,9) | 10 |
| 26 | 116 (82–188) | 1,52 (1,43–1,9) | 4,33 (3,08–6) | 3,46 (3,14–4,85) | 10 |
| 27 | 117 (83–184) | 1,7 (1,39–1,84) | 3,71 (2,21–5,34) | 3,6 (3,05–4,78) | 10 |
| 28 | 118 (80–187) | 1,5 (1,36–1,67) | 4,29 (2,93–6,53) | 3,49 (2,99–4,83) | 10 |
| 29 | 115 (78–190) | 1,6 (1,31–1,79) | 4,04 (2,9–7,25) | 3,48 (3,03–4,9) | 10 |
| 30 | 116 (76,5–190) | 1,53 (1,38–1,89) | 3,67 (2,79–5,73) | 3,45 (3,13–4,83) | 10 |
| 31 | 114 (82–191) | 1,5 (1,13–1,8) | 3,75 (2,81–5,17) | 3,48 (3,08–5) | 10 |
| 32 | 115 (83,5–191) | 1,55 (1,29–1,72) | 4,13 (2,6–5,32) | 3,44 (3,06–4,98) | 10 |
| 33 | 120 (86–194) | 1,55 (1,24–1,82) | 4 (2,6–4,9) | 3,45 (3,01–4,89) | 10 |
| 34 | 120 (88–199) | 1,62 (1,15–1,94) | 4,63 (3,25–5,52) | 3,37 (3–5,01) | 10 |
| 35 | 121 (87–198) | 1,5 (1,21–1,72) | 4,81 (2,98–6,5) | 3,53 (2,99–4,9) | 10 |
| 36 | 120 (87,5–202) | 1,59 (1,23–1,83) | 4,07 (2,74–6,6) | 3,49 (3,01–4,93) | 10 |
| 37 | 122 (89,5–200) | 1,55 (1,41–1,73) | 3,63 (2,45–7,95) | 3,54 (2,9–4,94) | 10 |
| 38 | 124 (88,5–194) | 1,52 (1,26–1,9) | 4,5 (2,4–5,38) | 3,62 (2,99–4,88) | 10 |
| 39 | 124 (85,5–192) | 1,46 (1,31–1,71) | 4,06 (2–5,93) | 3,63 (3,02–4,86) | 10 |
| 40 | 124 (86–196) | 1,48 (1,31–1,72) | 3,75 (1,8–5,43) | 3,68 (3,01–4,82) | 10 |
| 41 | 121 (82,5–194) | 1,5 (1,33–1,82) | 3,88 (2,3–7,09) | 3,67 (2,99–4,77) | 10 |
| 42 | 125 (82–197) | 1,5 (1,31–1,72) | 4,25 (3,01–5,6) | 3,69 (2,93–4,72) | 10 |
| 43 | 123 (83,5–196) | 1,5 (1,14–1,94) | 4,35 (3,04–4,93) | 3,66 (2,94–4,69) | 10 |
| 44 | 124 (82,5–199) | 1,61 (1,48–1,73) | 3,6 (3–5,5) | 3,71 (2,99–4,78) | 10 |
| 45 | 126 (84–195) | 1,65 (1,24–1,88) | 4,13 (3,03–7,3) | 3,75 (2,99–4,74) | 10 |
| 46 | 125 (82–196) | 1,59 (1,45–1,96) | 3,58 (2,42–6,25) | 3,75 (3,01–4,77) | 10 |
| 47 | 126 (82–198) | 1,61 (1,27–2) | 4,67 (3,63–5,9) | 3,76 (2,96–4,82) | 10 |
| 48 | 126 (82–196) | 1,45 (1,17–1,83) | 4,18 (3–5,75) | 3,78 (2,9–4,83) | 10 |
| 49 | 128 (83,5–205) | 1,63 (1,31–1,78) | 4,17 (3,13–5,75) | 3,72 (2,94–4,86) | 10 |
| 50 | 128 (86–199) | 1,58 (1,34–1,95) | 4 (3,42–6,02) | 3,76 (2,98–4,86) | 10 |
| 51 | 130 (88,5–198) | 1,63 (1,37–1,82) | 3,6 (2,75–6,65) | 3,79 (2,98–4,8) | 10 |
| 52 | 131 (88,5–198) | 1,5 (1,4–1,91) | 4,08 (1,84–6,76) | 3,9 (2,99–4,8) | 10 |
| 53 | 138 (89,5–196) | 1,55 (1,25–1,7) | 3,4 (2,22–6) | 3,85 (2,95–4,71) | 10 |
| 54 | 132 (84,5–188) | 1,48 (1,35–1,86) | 4,22 (2,92–6) | 3,78 (2,92–4,76) | 10 |
| 55 | 132 (80,5–186) | 1,61 (1,36–1,89) | 3,7 (2,71–7,6) | 3,78 (2,97–4,76) | 10 |
| 56 | 132 (81–188) | 1,53 (1,38–1,66) | 3,67 (2,92–5,88) | 3,82 (2,98–4,8) | 10 |
| 57 | 140 (81,5–188) | 1,55 (1,25–1,79) | 4,33 (3–6,35) | 3,8 (2,97–4,8) | 10 |
| 58 | 140 (78,5–192) | 1,52 (1,22–1,78) | 4,9 (2,75–6,38) | 3,81 (2,94–4,73) | 10 |
| 59 | 146 (85,5–202) | 1,58 (1,31–1,92) | 4,5 (3–6,23) | 3,82 (2,89–4,73) | 10 |
| 60 | 146 (82–200) | 1,56 (1,38–1,74) | 5 (3,27–7,7) | 3,87 (2,84–4,69) | 10 |

### Han ve ticaret

| Yıl | Ayakta han | Asılan ilan | Biten ilan | Ticaret seferi (kervan) | İkmal seferi |
|---|---|---|---|---|---|
| 1 | 4 (3–6) | 10 (4,5–17,5) | 3 (0–8,5) | 55 (16–90) | 30,5 (18–40) |
| 2 | 4 (3–6,5) | 9 (3,5–20) | 5 (0,5–11) | 51,5 (17,5–96) | 31,5 (19,5–39) |
| 3 | 4 (3–6,5) | 8 (3–15,5) | 3 (0,5–9) | 53,5 (21–97,5) | 31,5 (16–40,5) |
| 4 | 4 (3–6) | 7,5 (4,5–20) | 2 (0–9) | 59 (23–99) | 32 (17–41,5) |
| 5 | 4 (3–6) | 8 (2,5–20) | 3 (0–7) | 53,5 (21,5–99,5) | 31 (17,5–40,5) |
| 6 | 4 (3–6,5) | 7,5 (4–16,5) | 2,5 (0–8) | 56,5 (23–96,5) | 32 (19,5–41) |
| 7 | 4 (3–6,5) | 8,5 (3,5–20) | 4 (0–9) | 56 (20–90) | 32,5 (22,5–42) |
| 8 | 4 (3–6,5) | 8 (6–19) | 5 (0,5–7,5) | 60 (24,5–91,5) | 32 (22–41) |
| 9 | 4 (3–6,5) | 8,5 (3,5–18,5) | 2 (0–6,5) | 56 (23–91,5) | 30,5 (21,5–42) |
| 10 | 4 (3–6,5) | 8 (3,5–18) | 2,5 (0–8,5) | 60,5 (24–98,5) | 30 (22,5–41,5) |
| 11 | 4 (3–6,5) | 9 (3–19) | 2 (0–10) | 57,5 (21,5–104) | 31 (18–42,5) |
| 12 | 4 (3–6,5) | 10 (2,5–15) | 3 (0–7,5) | 53,5 (20–100) | 31 (18–40,5) |
| 13 | 4 (3–6,5) | 7,5 (3,5–22) | 2,5 (0–7,5) | 56,5 (23–118) | 32 (17–40,5) |
| 14 | 4,5 (2,5–6,5) | 9,5 (2,5–18) | 3 (0–8,5) | 64 (25–121) | 33 (18–42) |
| 15 | 4,5 (2,5–6) | 9 (4–16) | 3 (0,5–9) | 58,5 (23,5–104) | 31 (18–45) |
| 16 | 4 (3–6) | 6 (2,5–16,5) | 4 (0–8,5) | 59,5 (24,5–116) | 30,5 (16,5–46,5) |
| 17 | 4 (3–6) | 9 (3,5–21) | 3,5 (0–9) | 62,5 (20–118) | 29,5 (17–48,5) |
| 18 | 4,5 (3–6,5) | 8 (2–19,5) | 2,5 (0,5–12,5) | 65 (23,5–108) | 31 (18–46) |
| 19 | 5 (3–6,5) | 7 (3,5–19) | 2,5 (0–9) | 61 (28–115) | 31,5 (16,5–46) |
| 20 | 4,5 (3–6,5) | 7,5 (2–23,5) | 1 (0–10,5) | 66 (26,5–106) | 32 (17–48) |
| 21 | 4,5 (3–6,5) | 7 (3–20,5) | 2,5 (0–7) | 60,5 (24–121) | 31 (17,5–47,5) |
| 22 | 5 (3–6,5) | 6 (2,5–18,5) | 2 (0–10,5) | 64 (25,5–116) | 31,5 (18,5–45,5) |
| 23 | 5 (3–7) | 6,5 (2–18) | 1 (0–7,5) | 51 (23,5–114) | 30,5 (18,5–47) |
| 24 | 5 (3–7) | 7 (1–19) | 2 (0–8,5) | 65,5 (23–103) | 30,5 (17,5–46,5) |
| 25 | 5 (3–7) | 5,5 (3–20,5) | 2 (0–9) | 67,5 (23,5–108) | 29,5 (17,5–46) |
| 26 | 5 (3–7) | 6 (3–21) | 2 (0–12,5) | 65,5 (20,5–115) | 30 (16–45,5) |
| 27 | 5 (3–7) | 6 (4–17,5) | 2,5 (0–9) | 68 (20–120) | 31,5 (16,5–47,5) |
| 28 | 5 (3–6,5) | 6 (2,5–21) | 2,5 (0–8,5) | 64,5 (21,5–122) | 32 (16–46) |
| 29 | 5 (3–6,5) | 7 (2–19) | 1,5 (0–9,5) | 64,5 (27–111) | 34 (13–47,5) |
| 30 | 5 (3–6,5) | 6,5 (2,5–19) | 1,5 (0–7,5) | 65 (25–116) | 33 (14,5–49,5) |
| 31 | 4,5 (3–6,5) | 7 (3–17) | 2 (0–8,5) | 68,5 (24–106) | 32 (17,5–54,5) |
| 32 | 5 (3–6,5) | 7 (2–15) | 1,5 (0–8,5) | 69,5 (22–99) | 31,5 (17,5–56) |
| 33 | 5 (3–6,5) | 6,5 (3,5–15,5) | 1,5 (0–8,5) | 67,5 (27,5–107) | 32 (18,5–61) |
| 34 | 4,5 (3–6,5) | 6 (3,5–18,5) | 2 (0–9,5) | 67 (21,5–110) | 34 (19–58) |
| 35 | 4,5 (3–7) | 7 (3,5–14) | 1,5 (0–7,5) | 64 (27,5–118) | 34 (18–57) |
| 36 | 4,5 (3–7) | 6 (2,5–14,5) | 1,5 (0–7,5) | 67 (23,5–116) | 35 (16–57) |
| 37 | 4,5 (3–7) | 5,5 (3–15) | 1 (0–6,5) | 62 (29–128) | 35,5 (10–57,5) |
| 38 | 4,5 (3–7) | 7 (3–16) | 1,5 (0–10) | 67,5 (29–119) | 32 (10–62,5) |
| 39 | 4,5 (3–6,5) | 6,5 (2,5–15) | 1,5 (0–8,5) | 68 (29,5–115) | 32 (14,5–62) |
| 40 | 4,5 (3–6,5) | 8,5 (4,5–17,5) | 2 (0–7,5) | 63,5 (28,5–116) | 32 (14,5–63,5) |
| 41 | 5 (3–7) | 7 (2,5–18,5) | 2 (0–9) | 71 (29,5–110) | 33,5 (17,5–65) |
| 42 | 5 (3–7) | 9 (4–20,5) | 2,5 (0–9) | 69,5 (27–114) | 33 (17–64) |
| 43 | 4,5 (3–7) | 7,5 (3–19) | 2,5 (0–12) | 58 (23–122) | 32 (17,5–62,5) |
| 44 | 4,5 (3–7) | 6,5 (2–18,5) | 4 (0–10) | 70 (23–110) | 33,5 (17,5–62,5) |
| 45 | 4,5 (3–7) | 7,5 (4–15,5) | 3,5 (0–11,5) | 65,5 (23,5–130) | 35,5 (17,5–61) |
| 46 | 4 (3–7) | 7,5 (4–17) | 3,5 (0–9) | 67 (22–106) | 37 (17,5–67) |
| 47 | 4 (3–7) | 6 (3–19,5) | 3,5 (0–10,5) | 61,5 (20–101) | 34 (18,5–69,5) |
| 48 | 4 (3–7) | 8 (3–15,5) | 3 (0–8,5) | 67,5 (22–125) | 35,5 (21,5–67) |
| 49 | 4 (3–7) | 7 (2,5–17,5) | 2 (0,5–9,5) | 63 (20–112) | 35,5 (22–66) |
| 50 | 4 (3–7) | 5 (2,5–14,5) | 2 (0–11,5) | 61,5 (23,5–111) | 36,5 (21,5–67,5) |
| 51 | 4,5 (3–6,5) | 6 (2–14) | 1,5 (0–8) | 62,5 (18–124) | 37 (21,5–68,5) |
| 52 | 4,5 (3–6,5) | 6,5 (3–16) | 3 (0–7) | 57,5 (22,5–111) | 35,5 (21,5–66,5) |
| 53 | 4,5 (3–6,5) | 6 (2,5–16) | 2,5 (0–7,5) | 61,5 (17,5–116) | 33,5 (21,5–65) |
| 54 | 4 (3–6,5) | 8 (2–14,5) | 3 (0–9,5) | 62,5 (22–121) | 31,5 (21,5–63,5) |
| 55 | 4 (3–6) | 7 (0,5–15) | 2,5 (0–7) | 60,5 (20–136) | 33 (21,5–65,5) |
| 56 | 4,5 (3–6) | 8 (1,5–14) | 2,5 (0–9) | 60 (23–122) | 32 (21–65) |
| 57 | 4,5 (3–7) | 7 (2,5–13) | 2,5 (0–6,5) | 59 (24,5–122) | 33 (21,5–66) |
| 58 | 4,5 (3–7) | 7 (2–13) | 1,5 (0–7) | 60 (21,5–136) | 34 (21,5–67,5) |
| 59 | 4,5 (3–7) | 6,5 (2,5–11,5) | 2 (0–7,5) | 63,5 (23,5–122) | 34,5 (21,5–66,5) |
| 60 | 4,5 (3–7) | 7 (1,5–10,5) | 2 (0–5,5) | 54,5 (24–114) | 34 (21–67,5) |

### Altın ve ambar (1/3)

| Yıl | Altın p90 (medeniyetler) | Bakım gideri (altın; asker, kahraman, L2–L3) | Kamu işlerine (imar) harcanan altın | Ambarla beslenen amele tayını (gıda) | Kamu işlerindeki (amele) iş gücü payı | İmar ortalaması (köy+, 0–100) |
|---|---|---|---|---|---|---|
| 1 | 722 (427–1000) | 2296 (1638–3134) | 972 (373–1421) | 2086 (1102–3722) | %16 (%12–%21) | 12,8 (10,2–15,5) |
| 2 | 683 (434–1347) | 2227 (1609–2977) | 772 (205–2168) | 2133 (655–3883) | %15 (%12–%19) | 12,3 (9,6–16,1) |
| 3 | 820 (440–1211) | 2337 (1638–3003) | 803 (286–2071) | 2206 (331–3333) | %14 (%11–%19) | 11,4 (9,48–15,8) |
| 4 | 758 (478–1018) | 2230 (1611–3132) | 928 (277–1853) | 2525 (1088–4057) | %16 (%12–%19) | 11,4 (9,3–16) |
| 5 | 747 (376–1057) | 2225 (1651–2992) | 1011 (268–1794) | 2476 (1283–3604) | %15 (%12–%19) | 11,3 (8,73–16,1) |
| 6 | 758 (545–1082) | 2280 (1626–3004) | 964 (319–1681) | 2371 (1138–3892) | %14 (%11–%20) | 11,8 (8,81–16,1) |
| 7 | 720 (520–1032) | 2299 (1617–3030) | 1032 (271–1637) | 2295 (788–4315) | %15 (%13–%20) | 11,7 (8,79–15,9) |
| 8 | 749 (457–998) | 2325 (1673–3054) | 1035 (333–1848) | 2269 (851–4394) | %14 (%13–%19) | 11,9 (9,21–15,5) |
| 9 | 972 (606–1239) | 2256 (1634–2959) | 1095 (450–1869) | 2377 (910–3721) | %15 (%12–%18) | 12,6 (9,41–15,6) |
| 10 | 888 (559–1123) | 2164 (1682–3011) | 1309 (552–1982) | 1968 (610–4218) | %16 (%9,9–%19) | 12,8 (9,31–17,4) |
| 11 | 862 (647–1172) | 2255 (1602–2964) | 1431 (568–2000) | 1497 (428–3344) | %15 (%11–%19) | 12,5 (9,13–18) |
| 12 | 895 (592–1688) | 2237 (1640–2958) | 1459 (536–2833) | 1716 (635–3006) | %16 (%9,9–%19) | 12,2 (9,05–19,8) |
| 13 | 908 (624–1471) | 2427 (1670–3013) | 1450 (503–2728) | 1754 (289–3103) | %14 (%11–%19) | 12 (8,63–21,1) |
| 14 | 795 (502–1334) | 2252 (1706–3057) | 1331 (401–2435) | 1313 (149–3738) | %14 (%11–%18) | 12,1 (8,72–20,1) |
| 15 | 906 (703–1316) | 2274 (1709–3010) | 1328 (386–2330) | 1568 (270–3239) | %14 (%9,8–%20) | 12,5 (8,88–20,9) |
| 16 | 979 (732–1244) | 2331 (1715–3056) | 1259 (638–2303) | 1695 (93,4–3515) | %15 (%9–%19) | 12,9 (9,33–21,1) |
| 17 | 874 (491–1323) | 2306 (1683–2974) | 1387 (380–2264) | 1557 (176–3750) | %16 (%9,4–%19) | 12,6 (9,15–20,6) |
| 18 | 839 (482–1258) | 2315 (1711–2930) | 1152 (220–2217) | 1783 (394–3981) | %16 (%10–%20) | 12 (8,32–19,3) |
| 19 | 895 (537–1397) | 2266 (1769–2893) | 1266 (281–2392) | 1900 (241–3398) | %15 (%11–%19) | 12,3 (7,63–19,4) |
| 20 | 969 (559–1178) | 2236 (1843–2934) | 1365 (325–2440) | 1620 (243–4182) | %15 (%11–%18) | 13 (7,46–19,5) |
| 21 | 927 (633–1292) | 2244 (1846–3053) | 1384 (409–2497) | 1600 (127–3692) | %14 (%10–%17) | 12,7 (7,26–20,4) |
| 22 | 935 (697–1445) | 2310 (1712–3080) | 1481 (532–2415) | 1577 (392–2754) | %16 (%10–%17) | 12 (7,46–20,7) |
| 23 | 885 (637–1289) | 2154 (1798–3196) | 1476 (500–2416) | 1453 (199–3096) | %15 (%11–%18) | 12,3 (7,93–19,8) |
| 24 | 1102 (601–1593) | 2274 (1644–3137) | 1586 (588–2785) | 1466 (180–3205) | %15 (%11–%18) | 12,6 (7,88–21,7) |
| 25 | 1134 (715–1647) | 2301 (1723–2981) | 1715 (586–3021) | 1504 (182–3350) | %15 (%11–%18) | 12,8 (8,49–21,2) |
| 26 | 922 (722–1254) | 2254 (1650–3013) | 1649 (689–2396) | 1206 (58–3442) | %15 (%9,6–%18) | 13,6 (8,07–20,6) |
| 27 | 960 (829–1348) | 2376 (1550–3005) | 1439 (829–2802) | 1054 (108–2827) | %15 (%9,8–%18) | 14 (7,91–20,5) |
| 28 | 1010 (718–1272) | 2364 (1635–2963) | 1332 (819–2467) | 1200 (230–3113) | %14 (%11–%19) | 14 (8,9–19,7) |
| 29 | 1034 (732–1433) | 2260 (1638–2955) | 1530 (839–2649) | 1350 (350–2975) | %15 (%9,9–%19) | 13,9 (9,66–22,3) |
| 30 | 1118 (864–1571) | 2290 (1531–3007) | 1491 (792–3276) | 1159 (243–3141) | %14 (%11–%18) | 14,2 (10,7–22,6) |
| 31 | 1076 (805–1653) | 2137 (1598–3043) | 1595 (974–3400) | 1366 (86–2792) | %15 (%11–%19) | 15,1 (10,6–22,2) |
| 32 | 1060 (769–1306) | 2245 (1543–3011) | 1642 (850–2785) | 909 (4,8–2220) | %14 (%11–%18) | 15,6 (10,1–21,5) |
| 33 | 1069 (871–1412) | 2213 (1618–3080) | 1631 (902–2165) | 1085 (47,4–1749) | %14 (%12–%17) | 16,2 (10,5–20,3) |
| 34 | 1054 (841–1552) | 2235 (1624–3049) | 1501 (887–2648) | 676 (0–2083) | %14 (%12–%17) | 15,5 (11,1–18,4) |
| 35 | 1031 (729–1319) | 2411 (1542–3032) | 1549 (731–2843) | 783 (6,4–2186) | %15 (%11–%17) | 14 (10,6–19) |
| 36 | 1092 (834–1344) | 2275 (1533–3052) | 1619 (829–3058) | 497 (0–2381) | %15 (%11–%17) | 13,2 (10,1–19,3) |
| 37 | 1140 (817–1451) | 2274 (1522–3028) | 1518 (801–2918) | 776 (4,2–2646) | %15 (%12–%17) | 13,2 (9,92–19,7) |
| 38 | 1144 (763–1635) | 2309 (1479–2938) | 1493 (701–3184) | 583 (7,6–2624) | %15 (%11–%18) | 12,4 (8,91–20,5) |
| 39 | 1201 (896–1488) | 2224 (1576–2912) | 1646 (701–3093) | 575 (0–2031) | %16 (%11–%17) | 12,2 (8,5–21,3) |
| 40 | 1152 (780–1386) | 2297 (1624–2846) | 1695 (813–2921) | 498 (53,4–1596) | %16 (%11–%18) | 11,9 (8,24–21,4) |
| 41 | 1076 (743–1447) | 2193 (1659–2889) | 1506 (652–2981) | 429 (30,6–1638) | %15 (%10–%17) | 12,4 (7,66–21,3) |
| 42 | 1158 (849–1447) | 2325 (1720–2799) | 1478 (655–3107) | 781 (4,2–1964) | %14 (%10–%17) | 12,8 (7,4–21,2) |
| 43 | 1136 (666–1373) | 2288 (1641–2770) | 1521 (762–3265) | 610 (0,4–2015) | %15 (%8,7–%19) | 12,7 (8,18–21,2) |
| 44 | 1172 (745–1579) | 2151 (1619–2744) | 1804 (580–4024) | 1040 (0–2290) | %15 (%13–%18) | 13,5 (7,95–21) |
| 45 | 1195 (797–1545) | 2195 (1644–2829) | 1847 (796–3729) | 458 (13–2447) | %14 (%12–%20) | 13,3 (8,34–22,8) |
| 46 | 1192 (738–1521) | 2182 (1570–2806) | 1886 (874–3786) | 654 (0–2008) | %15 (%13–%18) | 13,9 (8,25–23,8) |
| 47 | 1132 (728–1687) | 2261 (1630–2810) | 1703 (766–3571) | 246 (0,2–1659) | %13 (%12–%19) | 13,6 (8,16–25,2) |
| 48 | 1025 (796–1743) | 2303 (1517–2875) | 1423 (765–3341) | 645 (44,6–1524) | %13 (%10–%18) | 13,4 (8,27–24,4) |
| 49 | 1119 (798–1746) | 2311 (1570–2755) | 1722 (856–3216) | 509 (2,6–1887) | %14 (%11–%18) | 12,8 (8,25–23,2) |
| 50 | 1129 (732–1608) | 2255 (1639–2868) | 1609 (646–3092) | 417 (0,6–1609) | %14 (%9,2–%18) | 12,8 (7,79–22,8) |
| 51 | 1124 (994–1759) | 2239 (1638–2812) | 1512 (760–3152) | 696 (20,2–1982) | %13 (%11–%17) | 12,8 (7,49–22,8) |
| 52 | 1164 (711–1516) | 2275 (1561–2862) | 1551 (912–2904) | 270 (51,8–3164) | %14 (%11–%17) | 12,8 (7,65–22,6) |
| 53 | 1119 (723–1562) | 2217 (1554–2882) | 1582 (699–2940) | 438 (0,2–1400) | %14 (%11–%17) | 13,4 (7,21–22,3) |
| 54 | 1163 (719–1626) | 2195 (1531–2874) | 1703 (714–3521) | 451 (26–1267) | %15 (%10–%17) | 12,9 (6,81–21,8) |
| 55 | 1159 (779–2278) | 2281 (1581–2997) | 2075 (822–5398) | 437 (43,2–1219) | %14 (%9,3–%17) | 13,6 (8,1–22,6) |
| 56 | 1191 (756–1705) | 2352 (1606–2969) | 2072 (768–4250) | 338 (3,4–1547) | %15 (%8,5–%17) | 14 (7,69–22,1) |
| 57 | 1176 (916–1665) | 2251 (1637–2986) | 2126 (864–3765) | 537 (0–1559) | %15 (%8,7–%18) | 15,3 (7,88–21,7) |
| 58 | 1213 (791–1675) | 2340 (1549–2836) | 2021 (870–3725) | 508 (0–1873) | %15 (%9,2–%18) | 16,8 (8,47–21,7) |
| 59 | 1262 (798–1798) | 2302 (1651–2935) | 1813 (798–3684) | 442 (0–1432) | %15 (%9,1–%18) | 16,4 (8,94–21,5) |
| 60 | 1227 (762–1692) | 2195 (1584–2841) | 1768 (694–3709) | 207 (1,8–1749) | %16 (%10–%17) | 15,3 (8,89–21,1) |

### Altın ve ambar (2/3)

| Yıl | Canavar baskınında yitirilen altın | Ejderhaya giden altın (haraç + akın) | Hazinesi boş medeniyet payı | Kent tüketiminde yokluk payı (köy+; ekmek, bira ya da alet) | Ekmek ya da bira yokluğu payı (köy+) | Kıtlık (büyük olay) |
|---|---|---|---|---|---|---|
| 1 | 0 (0–17,5) | 308 (0–854) | %0 | %25 (%18–%33) | %0,5 (%0–%5,7) | 0 |
| 2 | 6 (0–20) | 232 (0–420) | %0 | %24 (%18–%33) | %1,6 (%0–%5) | 0 |
| 3 | 1,5 (0–12,5) | 102 (0–434) | %0 | %24 (%17–%35) | %0,6 (%0–%6,7) | 0 |
| 4 | 3 (0–20) | 237 (0–588) | %0 | %21 (%15–%33) | %1 (%0–%5,4) | 0 |
| 5 | 0 (0–20,5) | 286 (0–527) | %0 | %21 (%16–%32) | %1,4 (%0–%3,9) | 0 |
| 6 | 0 (0–19,5) | 204 (0–418) | %0 | %22 (%15–%29) | %1,9 (%0–%4) | 0 |
| 7 | 2 (0–20) | 170 (0–560) | %0 | %21 (%15–%30) | %2 (%0–%5,9) | 0 |
| 8 | 6 (0–16) | 244 (0–530) | %0 | %21 (%11–%30) | %1,1 (%0–%7,8) | 0 |
| 9 | 1,5 (0–16) | 158 (0–406) | %0 | %22 (%15–%32) | %1,6 (%0–%7,6) | 0 |
| 10 | 0 (0–16,5) | 224 (0–778) | %0 | %25 (%18–%29) | %1,5 (%0–%7,5) | 0 |
| 11 | 0 (0–21,5) | 205 (0–625) | %0 | %27 (%14–%31) | %1,2 (%0–%9,6) | 0 |
| 12 | 0 (0–8) | 186 (0–562) | %0 | %27 (%15–%31) | %1 (%0–%7,9) | 0 |
| 13 | 0,5 (0–10) | 112 (0–534) | %0 | %28 (%15–%34) | %2,5 (%0–%12) | 0 |
| 14 | 1 (0–19) | 217 (0–826) | %0 | %28 (%15–%35) | %1 (%0–%13) | 0 |
| 15 | 0 (0–44,5) | 126 (0–502) | %0 | %28 (%18–%35) | %2,9 (%0–%12) | 0 |
| 16 | 0 (0–17,5) | 272 (0–626) | %0 | %29 (%19–%33) | %2,9 (%0–%10) | 0 |
| 17 | 1 (0–14) | 194 (0–560) | %0 | %28 (%20–%32) | %2,9 (%0–%10) | 0 |
| 18 | 0 (0–19,5) | 146 (0–525) | %0 | %26 (%20–%30) | %2,3 (%0–%8,9) | 0 |
| 19 | 0,5 (0–10) | 158 (0–520) | %0 | %25 (%19–%31) | %3,5 (%0–%9,2) | 0 |
| 20 | 0 (0–11) | 242 (0–533) | %0 | %27 (%21–%32) | %2,5 (%0–%10) | 0 |
| 21 | 5,5 (0–23) | 158 (0–533) | %0 | %27 (%21–%31) | %2,6 (%0–%12) | 0 |
| 22 | 0 (0–13,5) | 204 (0–776) | %0 | %25 (%20–%32) | %3 (%0–%8,8) | 0 |
| 23 | 0 (0–11) | 218 (0–625) | %0 | %25 (%19–%32) | %3 (%0–%11) | 0 |
| 24 | 0,5 (0–12) | 31 (0–467) | %0 | %24 (%20–%32) | %3,4 (%0–%11) | 0 |
| 25 | 2,5 (0–22) | 22,5 (0–580) | %0 | %25 (%20–%33) | %3,7 (%0–%11) | 0 |
| 26 | 0 (0–25,5) | 54,5 (0–590) | %0 | %25 (%19–%31) | %2,8 (%0–%11) | 0 |
| 27 | 0 (0–26) | 44 (0–506) | %0 | %24 (%16–%32) | %2,2 (%0–%10) | 0 |
| 28 | 0 (0–19,5) | 142 (0–480) | %0 | %23 (%17–%34) | %3,7 (%0–%9,9) | 0 |
| 29 | 0,5 (0–22) | 128 (0–598) | %0 | %23 (%15–%34) | %3,8 (%0–%10) | 0 |
| 30 | 2,5 (0–11,5) | 26,5 (0–510) | %0 | %24 (%17–%34) | %4,1 (%0–%11) | 0 |
| 31 | 0 (0–29,5) | 4 (0–804) | %0 | %25 (%16–%35) | %5,1 (%0–%13) | 0 (0–0,5) |
| 32 | 1,5 (0–35) | 28,5 (0–512) | %0 | %24 (%19–%35) | %4,6 (%0–%10) | 0 |
| 33 | 0 (0–25) | 40 (0–349) | %0 | %25 (%19–%36) | %4,5 (%0,1–%12) | 0 |
| 34 | 2,5 (0–21) | 0 (0–900) | %0 | %26 (%18–%35) | %3,2 (%0,2–%11) | 0 |
| 35 | 3 (0–23,5) | 0 (0–586) | %0 | %25 (%17–%35) | %3,6 (%0,3–%13) | 0 |
| 36 | 0 (0–8) | 0 (0–626) | %0 | %25 (%17–%34) | %4,3 (%0,3–%10) | 0 (0–0,5) |
| 37 | 0 (0–8,5) | 0 (0–825) | %0 | %24 (%16–%33) | %4 (%0,4–%7,9) | 0 (0–0,5) |
| 38 | 0,5 (0–18,5) | 0 (0–586) | %0 | %25 (%17–%33) | %4,3 (%0,4–%9,2) | 0 |
| 39 | 1 (0–27) | 0 (0–531) | %0 | %25 (%18–%34) | %4,2 (%0,2–%8,8) | 0 |
| 40 | 6,5 (0–20) | 0 (0–674) | %0 | %24 (%18–%32) | %4,2 (%0,4–%10) | 0 |
| 41 | 1,5 (0–19) | 0 (0–666) | %0 | %25 (%19–%31) | %4,4 (%0,3–%10) | 0 |
| 42 | 0 (0–20,5) | 0 (0–460) | %0 | %25 (%19–%31) | %3,9 (%0,8–%9,9) | 0 |
| 43 | 5 (0–18,5) | 0 (0–966) | %0 | %25 (%17–%30) | %4 (%0–%11) | 0 |
| 44 | 3 (0–27,5) | 0 (0–599) | %0 | %25 (%20–%31) | %4,1 (%0–%9,8) | 0 |
| 45 | 3 (0–22,5) | 0 (0–502) | %0 | %25 (%19–%32) | %4,2 (%0,1–%9,6) | 0 |
| 46 | 5,5 (0–30) | 0 (0–714) | %0 | %25 (%17–%30) | %3,4 (%0–%8,4) | 0 |
| 47 | 3,5 (0–27,5) | 0 (0–512) | %0 | %23 (%15–%31) | %2,6 (%0,3–%8,5) | 0 |
| 48 | 2 (0–19,5) | 0 (0–476) | %0 | %23 (%15–%29) | %3,5 (%0,5–%7) | 0 |
| 49 | 4 (0–16,5) | 0 (0–755) | %0 | %23 (%17–%28) | %3,3 (%0,3–%6) | 0 |
| 50 | 5 (0–46,5) | 0 (0–594) | %0 | %21 (%14–%27) | %2,9 (%0,4–%7,2) | 0 |
| 51 | 3 (0–19) | 0 (0–581) | %0 | %21 (%12–%27) | %2,6 (%0,4–%6,3) | 0 |
| 52 | 3,5 (0–27) | 0 (0–838) | %0 | %22 (%13–%28) | %2,4 (%0,4–%5,8) | 0 (0–0,5) |
| 53 | 0 (0–15,5) | 0 (0–544) | %0 | %22 (%13–%30) | %3,1 (%1,1–%6,8) | 0 |
| 54 | 3 (0–19,5) | 0 (0–414) | %0 | %21 (%12–%30) | %3,5 (%0,9–%7) | 0 (0–0,5) |
| 55 | 0 (0–12,5) | 0 (0–593) | %0 | %22 (%11–%29) | %2,6 (%0,6–%6) | 0 |
| 56 | 11 (0–45) | 0 (0–598) | %0 | %22 (%12–%30) | %2,6 (%0,5–%5) | 0 |
| 57 | 0 (0–38) | 0 (0–600) | %0 | %22 (%11–%30) | %3,1 (%0–%6,3) | 0 |
| 58 | 6,5 (0–18) | 0 (0–515) | %0 | %21 (%10–%29) | %3 (%0,6–%5,8) | 0 |
| 59 | 2 (0–18,5) | 0 (0–557) | %0 | %22 (%8,8–%29) | %3,7 (%0,2–%5,9) | 0 |
| 60 | 7,5 (0–35) | 0 (0–385) | %0 | %22 (%11–%28) | %3,4 (%0,4–%6,4) | 0 |

### Altın ve ambar (3/3)

| Yıl | Açlıktan ölen | Kıtlık yardımı (sevkiyat) | Kıtlıkta yüz çeviren | Kıtlık akını | Ambarın yettiği gün (medeniyet medyanı) |
|---|---|---|---|---|---|
| 1 | 0 | 0 | 0 | 0 | 53,4 (43,6–68,2) |
| 2 | 0 | 0 | 0 | 0 | 53,4 (37,8–73,8) |
| 3 | 0 | 0 | 0 | 0 | 49,2 (40,3–64,6) |
| 4 | 0 | 0 | 0 | 0 | 50,8 (37,4–65,2) |
| 5 | 0 | 0 | 0 | 0 | 49 (37,5–69) |
| 6 | 0 | 0 | 0 | 0 | 47,4 (34,3–62,3) |
| 7 | 0 | 0 | 0 | 0 | 46 (31,9–60,4) |
| 8 | 0 | 0 | 0 | 0 | 48,9 (30–61,1) |
| 9 | 0 | 0 | 0 | 0 | 45,9 (29,6–64,2) |
| 10 | 0 | 0 | 0 | 0 | 41,5 (29–67,9) |
| 11 | 0 | 0 | 0 | 0 | 40,8 (31,3–61,3) |
| 12 | 0 | 0 | 0 | 0 | 48,5 (33,2–64,1) |
| 13 | 0 | 0 | 0 | 0 | 49,5 (30–59,6) |
| 14 | 0 | 0 | 0 | 0 | 49,6 (29,4–60,8) |
| 15 | 0 | 0 | 0 | 0 | 49,6 (32,1–57,5) |
| 16 | 0 | 0 | 0 | 0 | 49,3 (30–57,6) |
| 17 | 0 | 0 | 0 | 0 | 49,9 (33,7–60,8) |
| 18 | 0 | 0 | 0 | 0 | 49,1 (31,2–60,5) |
| 19 | 0 | 0 | 0 | 0 | 48,7 (31,2–57,8) |
| 20 | 0 | 0 | 0 | 0 | 45,4 (29,3–58,5) |
| 21 | 0 | 0 | 0 | 0 | 45,4 (30,5–60,3) |
| 22 | 0 | 0 | 0 | 0 | 42,2 (31–59,6) |
| 23 | 0 | 0 | 0 | 0 | 46,5 (29,9–54,9) |
| 24 | 0 | 0 | 0 | 0 | 46,7 (27,9–57,2) |
| 25 | 0 | 0 | 0 | 0 | 44,6 (30,7–56,1) |
| 26 | 0 | 0 | 0 | 0 | 45,1 (29,1–54,5) |
| 27 | 0 | 0 | 0 | 0 | 44,7 (27,3–56,3) |
| 28 | 0 | 0 | 0 | 0 | 45,5 (29,8–58,2) |
| 29 | 0 | 0 | 0 | 0 | 42,3 (32,7–49,9) |
| 30 | 0 (0–3) | 0 | 0 | 0 | 41,6 (33,6–51,1) |
| 31 | 0 (0–0,5) | 0 (0–0,5) | 0 | 0 | 42,4 (29,6–58,8) |
| 32 | 0 (0–4,5) | 0 | 0 | 0 (0–0,5) | 40,8 (32,8–51,4) |
| 33 | 0 (0–4,5) | 0 | 0 | 0 | 40 (29,8–56) |
| 34 | 0 | 0 | 0 | 0 | 39,7 (28,2–54,1) |
| 35 | 0 | 0 | 0 | 0 | 41 (28–48,3) |
| 36 | 0 | 0 | 0 (0–1,5) | 0 (0–0,5) | 43,9 (28,3–53,8) |
| 37 | 0 | 0 (0–1) | 0 (0–0,5) | 0 | 40,8 (25,7–53,3) |
| 38 | 0 | 0 | 0 | 0 | 42,1 (24,2–49,8) |
| 39 | 0 | 0 | 0 | 0 | 39,5 (26–51,2) |
| 40 | 0 | 0 | 0 | 0 | 47,2 (21,6–56,9) |
| 41 | 0 | 0 | 0 | 0 | 45,4 (25–54,2) |
| 42 | 0 | 0 | 0 | 0 | 44,1 (27,7–51,7) |
| 43 | 0 | 0 | 0 | 0 | 45,6 (27,1–50) |
| 44 | 0 | 0 | 0 | 0 | 42,1 (24,5–52,1) |
| 45 | 0 | 0 | 0 | 0 | 44,3 (22,8–50,7) |
| 46 | 0 | 0 | 0 | 0 | 43,8 (23,9–51,9) |
| 47 | 0 | 0 | 0 | 0 | 41 (23,5–55,1) |
| 48 | 0 (0–4) | 0 | 0 | 0 | 41,2 (30,6–53,2) |
| 49 | 0 | 0 | 0 | 0 | 41,6 (25,2–50,5) |
| 50 | 0 | 0 | 0 | 0 | 38 (20,1–53,3) |
| 51 | 0 | 0 | 0 | 0 | 38,2 (23,7–51,6) |
| 52 | 0 | 0 | 0 (0–1) | 0 | 41,4 (23,2–56,3) |
| 53 | 0 | 0 | 0 | 0 | 42,6 (23,5–59,5) |
| 54 | 0 | 0 (0–0,5) | 0 | 0 | 41,3 (19,6–54,5) |
| 55 | 0 | 0 (0–0,5) | 0 (0–1) | 0 (0–0,5) | 39,2 (20,3–54) |
| 56 | 0 | 0 | 0 | 0 | 37,3 (18,6–58,5) |
| 57 | 0 (0–0,5) | 0 | 0 | 0 | 38,7 (16,5–57,9) |
| 58 | 0 | 0 | 0 | 0 | 42,1 (24,4–54,8) |
| 59 | 0 | 0 | 0 | 0 | 39,8 (20–53) |
| 60 | 0 | 0 | 0 | 0 | 44,2 (21,9–49,8) |

### Yerleşim kademesi

| Yıl | Ortalama yerleşim kademesi | Köy+ yerleşim | Kasaba+ yerleşim | Şehir | Ortalama başkent kademesi | Kademe değişimi (yerleşim, yıl içinde) |
|---|---|---|---|---|---|---|
| 1 | 1,03 (0,91–1,14) | 53 (38,5–74) | 14 (11–19) | 4 (3–6,5) | 2,75 (2,39–2,92) | 6,5 (3–11) |
| 2 | 1,04 (0,89–1,12) | 54 (38,5–73,5) | 14 (11,5–19) | 4 (3–6,5) | 2,73 (2,5–2,92) | 7,5 (0,5–13) |
| 3 | 1,02 (0,9–1,13) | 54,5 (38,5–71) | 15 (9,5–20,5) | 4 (3–6) | 2,75 (2,29–3) | 7 (2,5–13) |
| 4 | 0,99 (0,9–1,14) | 55,5 (39,5–72,5) | 15 (10–21) | 4 (3–6) | 2,63 (2,31–2,93) | 6 (2,5–10,5) |
| 5 | 1,04 (0,91–1,13) | 56,5 (39,5–71,5) | 15,5 (10,5–19,5) | 5 (3–5,5) | 2,73 (2,31–2,82) | 7 (2–10,5) |
| 6 | 1,07 (0,94–1,13) | 56,5 (38,5–71,5) | 15 (10,5–18) | 5 (3–6,5) | 2,78 (2,36–3) | 7,5 (2–10,5) |
| 7 | 1,05 (0,93–1,14) | 56 (38,5–73) | 15,5 (10,5–18,5) | 5 (3–6) | 2,75 (2,46–2,92) | 6,5 (2,5–11,5) |
| 8 | 1,04 (0,94–1,15) | 57 (39–69,5) | 15,5 (11–19,5) | 5 (3–6) | 2,75 (2,51–3) | 5 (1,5–10) |
| 9 | 1,05 (0,95–1,13) | 57 (38,5–70,5) | 15,5 (11,5–20,5) | 5 (3,5–6) | 2,71 (2,5–3) | 4,5 (1–11) |
| 10 | 1,04 (0,96–1,16) | 57,5 (38,5–72,5) | 15,5 (12,5–22) | 5 (3,5–6,5) | 2,75 (2,38–3) | 5 (2,5–11) |
| 11 | 1,07 (0,96–1,15) | 59 (38–73) | 16 (11,5–21,5) | 5 (3,5–6) | 2,78 (2,38–3) | 6 (2–11) |
| 12 | 1,09 (0,94–1,19) | 58 (39–73) | 16,5 (13–21,5) | 5 (3–7) | 2,78 (2,32–3) | 6,5 (2–9,5) |
| 13 | 1,09 (0,97–1,17) | 59,5 (39–74,5) | 17 (12,5–22,5) | 4,5 (4–7,5) | 2,75 (2,29–2,82) | 5,5 (2–12) |
| 14 | 1,1 (0,96–1,2) | 60,5 (39–75,5) | 16,5 (12,5–22,5) | 5 (4–6,5) | 2,75 (2,46–3) | 5 (2–6) |
| 15 | 1,09 (0,96–1,21) | 60 (39,5–74,5) | 17 (12,5–22) | 5 (3–6) | 2,78 (2,45–3) | 7 (2–9) |
| 16 | 1,08 (0,98–1,19) | 60,5 (39–74,5) | 17,5 (11,5–20,5) | 5 (3,5–7) | 2,75 (2,41–2,92) | 5 (2,5–8,5) |
| 17 | 1,1 (0,97–1,2) | 61,5 (39–76,5) | 16,5 (12–21) | 5 (3–7,5) | 2,75 (2,49–3) | 4,5 (1–11) |
| 18 | 1,11 (0,95–1,18) | 62 (39,5–76) | 16 (12–20,5) | 5 (3–6) | 2,69 (2,41–3) | 7 (2–10) |
| 19 | 1,13 (0,98–1,2) | 61 (40–75,5) | 16,5 (12–20,5) | 5 (3,5–7) | 2,75 (2,46–3) | 4 (0,5–8) |
| 20 | 1,13 (0,97–1,19) | 62,5 (40–75) | 16 (11,5–21,5) | 5 (3,5–6,5) | 2,71 (2,49–3) | 6 (1–11) |
| 21 | 1,08 (0,98–1,19) | 61 (41–73) | 16,5 (11,5–20) | 5 (4–6) | 2,71 (2,46–3) | 6 (1–12) |
| 22 | 1,11 (0,98–1,2) | 62,5 (40,5–74) | 15,5 (10,5–20,5) | 5 (3,5–6) | 2,68 (2,46–3) | 4 (1,5–8) |
| 23 | 1,12 (0,99–1,23) | 62 (41–73,5) | 16,5 (11–21) | 5,5 (3–7) | 2,71 (2,5–3) | 5 (2,5–10) |
| 24 | 1,1 (0,99–1,23) | 62 (42–74) | 16 (11,5–20,5) | 6 (4–7) | 2,71 (2,5–3) | 4 (2–10,5) |
| 25 | 1,09 (0,98–1,25) | 61 (42–75) | 16 (11,5–19) | 5 (3,5–7) | 2,73 (2,46–3) | 6,5 (2–10) |
| 26 | 1,08 (0,97–1,25) | 61 (42,5–74,5) | 15,5 (11–20) | 5 (3,5–7) | 2,73 (2,46–3) | 4 (1–9) |
| 27 | 1,1 (0,93–1,24) | 60,5 (42–74,5) | 16 (11–20) | 5 (3,5–6,5) | 2,67 (2,46–3) | 4 (1–8) |
| 28 | 1,08 (0,96–1,26) | 61,5 (42–75,5) | 15 (11–21,5) | 5 (3,5–6,5) | 2,71 (2,46–2,92) | 5 (1,5–9) |
| 29 | 1,1 (0,98–1,27) | 62 (42–74) | 15 (12–21) | 5,5 (4–7) | 2,75 (2,54–3) | 4,5 (1–10,5) |
| 30 | 1,1 (0,97–1,27) | 60,5 (43–73,5) | 15 (12–20,5) | 5,5 (4–7) | 2,78 (2,5–3) | 4 (1,5–10) |
| 31 | 1,09 (0,97–1,26) | 62 (42–73) | 15,5 (11,5–21) | 5 (4–7) | 2,75 (2,54–2,92) | 3 (1–10,5) |
| 32 | 1,08 (0,98–1,21) | 61 (42–74) | 15,5 (10,5–21) | 5 (3–7) | 2,73 (2,42–2,92) | 4 (1–9,5) |
| 33 | 1,09 (0,98–1,23) | 60 (42,5–75) | 15 (10,5–20,5) | 4,5 (3,5–7,5) | 2,73 (2,4–3) | 4 (1–8) |
| 34 | 1,11 (0,98–1,19) | 59 (43–76,5) | 15,5 (8–20,5) | 5 (3–7,5) | 2,78 (2,58–3) | 4 (2–10,5) |
| 35 | 1,12 (0,96–1,23) | 58,5 (37,5–77) | 15,5 (8,5–20,5) | 5 (3–7) | 2,78 (2,58–3) | 6 (1–11) |
| 36 | 1,12 (0,94–1,2) | 59 (37,5–76) | 14,5 (8,5–21) | 5 (3–7) | 2,78 (2,41–3) | 4 (1–11) |
| 37 | 1,11 (0,96–1,2) | 59 (38–75,5) | 15 (9–22) | 5 (4–8) | 2,75 (2,41–3) | 4 (1–9,5) |
| 38 | 1,11 (0,97–1,2) | 59 (38–74,5) | 14 (10–21,5) | 5 (4–8) | 2,82 (2,62–3) | 3 (1–7,5) |
| 39 | 1,11 (0,96–1,23) | 59 (37,5–73,5) | 14 (10–22,5) | 5 (4–7,5) | 2,78 (2,54–3) | 2,5 (0–6,5) |
| 40 | 1,1 (0,96–1,22) | 59,5 (39–72) | 14 (9,5–22) | 5 (4–7,5) | 2,8 (2,46–3) | 4 (0–8) |
| 41 | 1,11 (0,97–1,19) | 60,5 (38,5–73,5) | 14,5 (8,5–23) | 5 (3,5–7,5) | 2,8 (2,61–3) | 2 (0,5–6) |
| 42 | 1,1 (0,96–1,19) | 60 (39–73,5) | 15 (9,5–22,5) | 5 (3,5–7) | 2,75 (2,5–3) | 2,5 (1–6,5) |
| 43 | 1,1 (0,97–1,19) | 62 (39–72) | 15 (9–22) | 5 (4–6,5) | 2,75 (2,51–3) | 4 (1,5–5,5) |
| 44 | 1,11 (0,98–1,21) | 62 (39,5–72) | 15,5 (9,5–21,5) | 5 (4–7) | 2,78 (2,46–3) | 4 (1,5–8,5) |
| 45 | 1,1 (0,96–1,19) | 60,5 (39,5–73,5) | 15 (9,5–20) | 5 (4–6,5) | 2,75 (2,55–3) | 2,5 (0–7) |
| 46 | 1,08 (0,96–1,18) | 59,5 (39,5–74) | 13,5 (9–20) | 5 (3,5–6,5) | 2,78 (2,54–3) | 4 (2,5–6,5) |
| 47 | 1,1 (0,96–1,19) | 59 (40,5–74,5) | 14 (10–18,5) | 5 (4–6,5) | 2,78 (2,41–3) | 3,5 (1–6,5) |
| 48 | 1,1 (0,95–1,14) | 57,5 (40,5–75,5) | 14 (10–19,5) | 5 (3–6,5) | 2,73 (2,45–3) | 3 (0,5–5,5) |
| 49 | 1,08 (0,95–1,17) | 58 (40,5–76) | 14 (8–18) | 5,5 (3–7) | 2,68 (2,27–3) | 4 (1–7,5) |
| 50 | 1,07 (0,93–1,18) | 58,5 (40,5–77) | 13,5 (7,5–18,5) | 5 (3–7) | 2,73 (2,34–3) | 3 (1–6,5) |
| 51 | 1,08 (0,94–1,21) | 58,5 (41,5–76,5) | 14 (6–20) | 5,5 (3,5–7) | 2,78 (2,34–3) | 3,5 (1–6,5) |
| 52 | 1,07 (0,91–1,23) | 57,5 (40,5–75,5) | 14,5 (5,5–20,5) | 5,5 (2,5–7) | 2,71 (2,23–3) | 3 (0,5–7,5) |
| 53 | 1,09 (0,9–1,2) | 58 (40,5–76) | 15 (5–20) | 5 (3,5–7) | 2,83 (2,31–3) | 2 (0–9,5) |
| 54 | 1,06 (0,91–1,19) | 58 (41,5–77,5) | 14,5 (4,5–19,5) | 4,5 (3–6,5) | 2,71 (2,23–3) | 4 (1,5–6) |
| 55 | 1,08 (0,91–1,18) | 59 (42–77) | 14 (5–20,5) | 5 (3–6,5) | 2,71 (2,23–3) | 3,5 (0–6,5) |
| 56 | 1,09 (0,9–1,18) | 58,5 (41,5–77) | 15 (5,5–20,5) | 5 (3–6) | 2,71 (2,31–3) | 3 (0–7) |
| 57 | 1,08 (0,89–1,18) | 60 (40,5–78) | 15 (5,5–20) | 5 (3–6) | 2,79 (2,34–3) | 3 (0,5–5,5) |
| 58 | 1,11 (0,88–1,17) | 60,5 (40,5–79) | 14,5 (5–19,5) | 4,5 (3–6,5) | 2,73 (2,29–3) | 2,5 (1,5–6) |
| 59 | 1,1 (0,89–1,18) | 60,5 (40–78,5) | 14,5 (5–20) | 4,5 (3,5–6,5) | 2,82 (2,34–3) | 2 (0,5–4) |
| 60 | 1,09 (0,91–1,18) | 60,5 (40,5–79) | 14,5 (5–19,5) | 5 (3–6,5) | 2,79 (2,25–3) | 3 (1–6,5) |

### Deniz

| Yıl | Liman (tersane) | Gemi (koga/tekne) | Kadırga | Denizaşırı yerleşim | Deniz ticaret yolu (yıl sonu) | Deniz seferi (ticaret) |
|---|---|---|---|---|---|---|
| 1 | 15 (8,5–19,5) | 35,5 (22–46,5) | 18 (13–23) | 7 (4,5–11) | 11 (7–19,5) | 14 (6,5–30) |
| 2 | 14,5 (8–20) | 35 (22,5–47,5) | 18 (12–22) | 7 (4,5–11) | 11,5 (7–20) | 15 (7–30) |
| 3 | 15 (8–20) | 36 (23–50) | 18 (12,5–22) | 7 (4,5–11,5) | 11,5 (7,5–21) | 12,5 (7–28,5) |
| 4 | 15,5 (8,5–20) | 36 (23–49) | 18 (12,5–22) | 7 (5–11,5) | 11,5 (7,5–21,5) | 15,5 (7–28,5) |
| 5 | 15,5 (9–20,5) | 36,5 (23–50,5) | 20 (12,5–22) | 7 (4,5–11,5) | 11,5 (7–21,5) | 14,5 (7,5–31) |
| 6 | 15,5 (9,5–20,5) | 37 (23–50) | 20 (12,5–22) | 7 (4,5–11,5) | 11,5 (7,5–21,5) | 15 (9–31) |
| 7 | 15,5 (9,5–21) | 37 (23,5–50,5) | 19,5 (12,5–22) | 7 (4,5–11,5) | 11,5 (7,5–21,5) | 15 (9–29,5) |
| 8 | 16 (9,5–20,5) | 36 (23,5–50) | 19,5 (12,5–22) | 7 (4,5–11,5) | 11,5 (7,5–22,5) | 15 (11–30) |
| 9 | 16 (9,5–20,5) | 36 (23,5–51,5) | 19 (12,5–22) | 7 (4,5–11,5) | 11,5 (8–23) | 14,5 (8–31,5) |
| 10 | 16 (9,5–20,5) | 35 (23,5–53) | 19 (12,5–22) | 7 (4,5–11,5) | 11,5 (8–23,5) | 14,5 (9,5–36,5) |
| 11 | 17 (9,5–21,5) | 36 (22,5–54) | 19 (12,5–22) | 7 (4,5–11,5) | 11,5 (8–24) | 16 (10–31,5) |
| 12 | 17,5 (9,5–21,5) | 36,5 (22–55,5) | 19 (12,5–22) | 7 (5–11,5) | 11,5 (8–24,5) | 18,5 (9–33) |
| 13 | 17 (10–21,5) | 36 (25–57) | 18,5 (13–22) | 7 (5–11,5) | 11 (8–26) | 16,5 (9,5–30,5) |
| 14 | 17 (10–21,5) | 35,5 (26,5–57) | 18,5 (13,5–22) | 7 (5–11,5) | 11 (8,5–26) | 16 (12–34) |
| 15 | 17 (10–21) | 35 (27–57) | 18,5 (13,5–22) | 7 (5–11,5) | 11 (8,5–26) | 16,5 (11–34,5) |
| 16 | 17 (10,5–21) | 35 (27,5–59) | 19 (13,5–22) | 7 (5–11,5) | 11 (8,5–25,5) | 16 (9–31,5) |
| 17 | 16 (10,5–21) | 34,5 (27,5–60) | 18,5 (13,5–22) | 7 (5–11,5) | 11 (8,5–26) | 15,5 (8,5–34,5) |
| 18 | 16 (10,5–21,5) | 35,5 (27–56,5) | 18,5 (13,5–22,5) | 7 (5–11,5) | 11 (8,5–26) | 14,5 (9–29,5) |
| 19 | 16,5 (11,5–21,5) | 36 (27–56,5) | 18,5 (13,5–22,5) | 7 (5–11,5) | 11 (8,5–26) | 16,5 (11,5–35) |
| 20 | 16,5 (11,5–22) | 36 (27–54,5) | 19,5 (13,5–22,5) | 7 (5–11,5) | 11 (8,5–26,5) | 16,5 (10–33,5) |
| 21 | 16,5 (11–22) | 37,5 (27–54,5) | 20 (13,5–22,5) | 7 (5–11,5) | 11 (8,5–26,5) | 17 (10–36) |
| 22 | 16,5 (11–21,5) | 37,5 (27,5–53,5) | 20 (13,5–22,5) | 7 (5–11,5) | 11 (8,5–26) | 16,5 (11,5–33) |
| 23 | 16,5 (11–21,5) | 36,5 (27,5–53) | 20 (13,5–22) | 7 (5–11,5) | 11 (8,5–26) | 15 (10,5–33,5) |
| 24 | 17 (11,5–21,5) | 37 (27–53) | 19 (13,5–22) | 7 (5–11,5) | 12 (8,5–26) | 15,5 (10,5–30) |
| 25 | 17 (11,5–21,5) | 37,5 (26,5–52,5) | 19 (13,5–22) | 7 (5–11,5) | 12 (8,5–26) | 19 (9–35) |
| 26 | 17,5 (11,5–21,5) | 37,5 (26,5–52) | 19 (13,5–22) | 7 (5–11,5) | 12 (8,5–26) | 16,5 (9,5–37,5) |
| 27 | 17,5 (12,5–21,5) | 37,5 (25,5–51,5) | 18,5 (13,5–22) | 7 (5–11,5) | 12 (8,5–26) | 19 (10–35,5) |
| 28 | 17,5 (12–21,5) | 37,5 (27,5–51,5) | 19 (13,5–22) | 7 (5–11,5) | 12 (8,5–26) | 18,5 (9–39) |
| 29 | 17,5 (12,5–21,5) | 38,5 (28–52) | 19 (13,5–22) | 7 (5–11,5) | 12 (8,5–26,5) | 19 (11–30,5) |
| 30 | 18 (12,5–21,5) | 39 (30–52) | 19 (13,5–22) | 7 (5–11,5) | 12 (8,5–26,5) | 22,5 (11–32) |
| 31 | 18 (12,5–21,5) | 38 (30–51,5) | 19 (13,5–22) | 7 (5–11,5) | 12 (8,5–26,5) | 19,5 (9,5–28) |
| 32 | 17,5 (12,5–22) | 38,5 (29,5–53,5) | 18,5 (14–22) | 7 (5–11,5) | 12 (8,5–26,5) | 20,5 (10–29,5) |
| 33 | 18 (12,5–22) | 38 (29,5–53,5) | 18,5 (14–21,5) | 7 (5–12) | 12 (8,5–26,5) | 19,5 (11,5–31,5) |
| 34 | 18 (12,5–22) | 39 (29,5–55) | 18,5 (13,5–21,5) | 7 (5–12) | 12,5 (8,5–26,5) | 19 (9–31) |
| 35 | 18 (12,5–23) | 38,5 (29,5–53) | 18,5 (13,5–21,5) | 7 (5–12) | 12,5 (8,5–26,5) | 22 (11–32) |
| 36 | 18 (13,5–22,5) | 39 (32–55) | 18,5 (13,5–21,5) | 7 (5–12) | 12,5 (8–26,5) | 18 (10–34,5) |
| 37 | 18 (12,5–22,5) | 40,5 (32–54) | 17,5 (13,5–21,5) | 7 (5–12) | 12,5 (8–26,5) | 20,5 (12–35,5) |
| 38 | 18,5 (12,5–23) | 40 (33,5–53) | 17,5 (13,5–21,5) | 7 (5–12) | 12,5 (8–26,5) | 21 (11,5–32) |
| 39 | 18,5 (12,5–23) | 38,5 (33,5–54) | 18,5 (13,5–21,5) | 7,5 (5–12) | 12,5 (8–27) | 19 (11,5–31,5) |
| 40 | 19 (13,5–23,5) | 39 (33–54) | 18,5 (13–21,5) | 7,5 (5–12) | 12,5 (8–27) | 19 (10,5–34,5) |
| 41 | 19 (13,5–24) | 39 (33–54,5) | 18,5 (13,5–21,5) | 7,5 (5–12) | 12,5 (8–27,5) | 19,5 (9,5–33,5) |
| 42 | 19 (13,5–24,5) | 38,5 (32–54,5) | 18,5 (13,5–21,5) | 7,5 (5–12) | 12,5 (8–28) | 21 (12,5–34,5) |
| 43 | 19,5 (13–24,5) | 40 (33–54,5) | 18,5 (14–21,5) | 7,5 (5–12) | 12,5 (8–28) | 18 (10,5–38) |
| 44 | 19,5 (13,5–24,5) | 40,5 (33–55,5) | 18,5 (14–21,5) | 7,5 (5–12) | 13 (8–28) | 18,5 (11,5–34) |
| 45 | 19,5 (13,5–25) | 40,5 (32–57) | 18,5 (14–21,5) | 7,5 (5–12) | 13 (8–28) | 19,5 (12,5–40,5) |
| 46 | 19 (13,5–25) | 40 (31,5–57) | 18,5 (14–21) | 7,5 (5–12) | 13 (8–28) | 18 (13–33) |
| 47 | 19 (13,5–25,5) | 41 (32–57) | 18,5 (14–21) | 7,5 (5–12) | 13 (8–28) | 19,5 (9,5–30,5) |
| 48 | 19 (14–25) | 41 (32–57) | 18,5 (14–21) | 7,5 (5–12) | 13 (8–28) | 20,5 (12–41) |
| 49 | 19,5 (13,5–25) | 41 (32,5–56,5) | 18,5 (14–21) | 7,5 (4,5–12) | 13 (7,5–28) | 19 (11–35,5) |
| 50 | 20 (13,5–25) | 41 (31,5–56,5) | 19 (14–21) | 7,5 (4,5–12) | 13,5 (7,5–28) | 19,5 (11,5–40) |
| 51 | 20 (13,5–25) | 42,5 (31,5–56,5) | 19,5 (13–21) | 7,5 (4,5–12) | 14,5 (7,5–28) | 19,5 (10–42) |
| 52 | 19,5 (13,5–25) | 43,5 (31,5–56,5) | 19 (12,5–21) | 7,5 (4,5–12) | 14 (7,5–28) | 19,5 (10–39) |
| 53 | 20 (13,5–25) | 43,5 (32–56,5) | 19 (12,5–21) | 7,5 (4,5–12) | 14,5 (7,5–28) | 19 (8–37) |
| 54 | 19,5 (14–25) | 43,5 (32–55,5) | 19 (12,5–21) | 7,5 (4,5–12) | 14,5 (7,5–28) | 19 (11,5–40) |
| 55 | 19,5 (13,5–25) | 43,5 (32–55,5) | 19 (12,5–21) | 7,5 (4,5–12) | 14,5 (7,5–28) | 22 (11–40,5) |
| 56 | 19,5 (13,5–25) | 44,5 (32–55,5) | 19 (12,5–21) | 7,5 (4,5–12) | 14,5 (7,5–28) | 20,5 (10–34) |
| 57 | 19,5 (13,5–25) | 44,5 (32–55,5) | 19 (12,5–21) | 7,5 (4,5–12) | 14,5 (7,5–28) | 20 (12–35) |
| 58 | 20 (13,5–25) | 44 (32–55,5) | 19 (12,5–21) | 7,5 (4,5–12) | 14,5 (7,5–29) | 17,5 (11,5–38) |
| 59 | 19,5 (13,5–24,5) | 42,5 (32–56) | 19 (12,5–21) | 7,5 (4,5–12) | 14,5 (7,5–30) | 19,5 (10,5–35,5) |
| 60 | 20 (13,5–24,5) | 44 (32–57) | 18,5 (12,5–21) | 7,5 (4,5–12) | 14 (7,5–30) | 18,5 (9–36) |

### v3: durum değişimi (1/2)

| Yıl | Yaşayan yerleşim (yıl ort.) | Büyük şehir (Şehir kademesi, yıl ort.) | El değiştiren yerleşim (fetih + bölünme) | El değiştiren büyük şehir | Büyük şehre hücum (kuşatma muharebesi) | Büyük şehir yağmalandı, tutulmadı |
|---|---|---|---|---|---|---|
| 1 | 73 (51,8–92) | 4,6 (3,4–5,68) | 1,5 (0–2) | 0 (0–0,5) | 0 (0–1,5) | 0 (0–1,5) |
| 2 | 73,2 (52–92,2) | 4 (2,98–6,71) | 1 (0–1,5) | 0 | 0 (0–0,5) | 0 (0–0,5) |
| 3 | 74 (52–92,6) | 4,09 (3,15–6,05) | 1 (0,5–2) | 0 (0–1) | 0,5 (0–1,5) | 0 (0–1) |
| 4 | 74,3 (51,5–93,2) | 4,15 (3,06–5,88) | 1 (0–1,5) | 0 | 0 | 0 |
| 5 | 74,5 (52,5–93,7) | 4,38 (3,11–5,63) | 1 (0–2,5) | 0 (0–0,5) | 0 (0–1) | 0 (0–0,5) |
| 6 | 74,3 (52–93,2) | 4,95 (2,74–6,19) | 1 (0–2) | 0 | 0 (0–1) | 0 (0–0,5) |
| 7 | 75 (51,7–93,5) | 5 (2,96–6,43) | 1 (0–3) | 0 (0–0,5) | 0 (0–1) | 0 (0–1) |
| 8 | 75,1 (51,6–93,5) | 4,66 (3,05–5,98) | 1 (0–2,5) | 0 (0–1) | 0 (0–1) | 0 (0–0,5) |
| 9 | 75,5 (51,3–93,6) | 4,86 (3,4–5,74) | 1 (0–2) | 0 | 0 (0–1) | 0 (0–1) |
| 10 | 75,5 (51–94) | 5 (3,28–6,16) | 1 (0,5–2,5) | 0 | 0 (0–0,5) | 0 |
| 11 | 75 (51,3–94,1) | 5 (3,5–6) | 1 (0–3) | 0 | 0 (0–0,5) | 0 |
| 12 | 75,4 (51,8–94,5) | 5,38 (3,68–6,11) | 1 (0–2,5) | 0 | 0 (0–2) | 0 (0–1,5) |
| 13 | 75 (51,8–94,5) | 5,14 (3,68–6,9) | 1,5 (0–2) | 0 | 0 (0–2) | 0 (0–1,5) |
| 14 | 75 (52–94,4) | 5,1 (3,7–6,64) | 1 (0–2) | 0 | 0 (0–1) | 0 (0–0,5) |
| 15 | 74,2 (52,2–94,5) | 4,91 (3,79–6,1) | 1 (0–2) | 0 | 0 (0–1) | 0 (0–0,5) |
| 16 | 74,5 (52–94,3) | 5 (3,54–6,2) | 1 (0–2) | 0 | 0 (0–1) | 0 (0–0,5) |
| 17 | 74,5 (52,7–94) | 4,96 (3,25–6,88) | 1 (0–2,5) | 0 | 0 (0–2) | 0 (0–1) |
| 18 | 74,5 (52,7–94) | 4,94 (2,98–6,36) | 1 (0–3) | 0 | 0 (0–1) | 0 (0–0,5) |
| 19 | 74,7 (52,3–94) | 5,29 (3,14–6,45) | 1 (0–2,5) | 0 | 0 (0–1) | 0 (0–0,5) |
| 20 | 75 (52,7–94,5) | 5,06 (3,5–6,74) | 1 (0–2,5) | 0 | 0 (0–1) | 0 (0–1) |
| 21 | 75 (53–94,5) | 5,09 (3,61–6,2) | 0,5 (0–2) | 0 | 0 (0–1) | 0 (0–1) |
| 22 | 75,2 (52,6–94) | 5,25 (3,7–6,1) | 1 (0–2,5) | 0 (0–0,5) | 0 (0–1) | 0 (0–1) |
| 23 | 75 (52,3–94) | 5,41 (3,65–6,98) | 1 (0–2) | 0 | 0 (0–1,5) | 0 (0–1) |
| 24 | 74,7 (52,5–94) | 5,85 (3,71–7,09) | 1 (0–2) | 0 | 0 (0–1,5) | 0 (0–1) |
| 25 | 74,5 (52,3–94,2) | 5,69 (3,91–6,81) | 0,5 (0–2) | 0 | 0 (0–1) | 0 (0–1) |
| 26 | 74,5 (51,9–94,5) | 5,14 (3,88–6,61) | 1 (0–2) | 0 (0–0,5) | 0 (0–1,5) | 0 (0–0,5) |
| 27 | 74,5 (51,9–94,5) | 5 (3,49–6,85) | 1 (0–2,5) | 0 | 0 (0–1) | 0 (0–0,5) |
| 28 | 74,1 (51,9–94,5) | 5 (3,4–6,25) | 0,5 (0–1,5) | 0 | 0 (0–1) | 0 (0–1) |
| 29 | 74,1 (51,5–94,5) | 5,23 (3,53–6,81) | 0,5 (0–3,5) | 0 (0–0,5) | 0 (0–1) | 0 (0–0,5) |
| 30 | 74,5 (51,5–94,5) | 5,5 (3,98–7) | 0,5 (0–2) | 0 (0–0,5) | 0 (0–1) | 0 (0–1) |
| 31 | 75,3 (51,5–94,5) | 5,48 (4,23–7) | 0 (0–2) | 0 | 0 (0–1) | 0 (0–0,5) |
| 32 | 75,5 (51,8–94) | 5,01 (3,25–6,81) | 0,5 (0–2,5) | 0 | 0 (0–1) | 0 |
| 33 | 75,4 (52–94,5) | 4,36 (2,93–7,48) | 0 (0–1,5) | 0 | 0 (0–0,5) | 0 (0–0,5) |
| 34 | 75 (52,5–94,4) | 4,78 (3,33–7,23) | 0 (0–2) | 0 | 0 (0–1) | 0 (0–0,5) |
| 35 | 75 (52,3–94,6) | 4,68 (3–7,36) | 0,5 (0–2,5) | 0 | 0 (0–1) | 0 (0–0,5) |
| 36 | 75 (52–94,1) | 5 (3,26–7) | 1 (0–2) | 0 | 0 (0–1) | 0 (0–1) |
| 37 | 74,5 (52–94,5) | 5 (3,5–7,5) | 1 (0–2) | 0 | 0 (0–0,5) | 0 |
| 38 | 74,5 (52,2–94,5) | 5 (3,98–7,91) | 1 (0–1,5) | 0 | 0 | 0 |
| 39 | 74,5 (52,5–94,5) | 5,11 (4–7,71) | 1 (0–2) | 0 | 0 (0–1) | 0 (0–0,5) |
| 40 | 74,5 (52,5–94,1) | 5 (4,04–7,9) | 0 (0–1) | 0 | 0 (0–1,5) | 0 (0–1,5) |
| 41 | 74,5 (53,1–94) | 5 (3,99–7,26) | 0 (0–2) | 0 | 0 (0–0,5) | 0 |
| 42 | 74,5 (53,5–93,5) | 5 (3,43–6,98) | 1 (0–1) | 0 | 0 (0–0,5) | 0 (0–0,5) |
| 43 | 74,5 (53,5–93,5) | 4,98 (4,08–6,8) | 0,5 (0–1) | 0 | 0 | 0 |
| 44 | 74,5 (53,5–93,5) | 4,94 (3,84–6,41) | 1 (0–2) | 0 | 0 (0–0,5) | 0 (0–0,5) |
| 45 | 74,5 (53,5–93,5) | 5 (4–6,68) | 0 (0–2) | 0 | 0 | 0 |
| 46 | 74,5 (53,5–93,5) | 5 (4,06–6,9) | 0 (0–1,5) | 0 | 0 | 0 |
| 47 | 74,5 (52,7–93,5) | 4,96 (3,9–6,48) | 1 (0–3) | 0 | 0 (0–0,5) | 0 |
| 48 | 74,5 (53,9–93,5) | 4,99 (3,49–6,48) | 1 (0–1,5) | 0 (0–1) | 0 (0–1) | 0 |
| 49 | 74,5 (54,4–93,5) | 5,3 (2,98–6,55) | 1 (0–2) | 0 | 0 (0–0,5) | 0 (0–0,5) |
| 50 | 74,5 (53,9–94) | 5,46 (3,06–6,84) | 0,5 (0–1,5) | 0 | 0 (0–1) | 0 (0–0,5) |
| 51 | 74,5 (54–94) | 5,3 (3,45–6,94) | 0 (0–2) | 0 (0–0,5) | 0 (0–1) | 0 (0–0,5) |
| 52 | 74,6 (54–94) | 5,6 (2,85–7) | 0,5 (0–1,5) | 0 | 0 (0–1) | 0 (0–0,5) |
| 53 | 75 (53,7–94) | 5,1 (2,7–7,23) | 0,5 (0–2) | 0 | 0 (0–1) | 0 |
| 54 | 75 (53,4–94) | 5,06 (3,49–6,78) | 0 (0–1,5) | 0 | 0 (0–2) | 0 (0–1) |
| 55 | 75 (53,5–94) | 4,58 (3,11–6,5) | 0 (0–1) | 0 | 0 (0–0,5) | 0 (0–0,5) |
| 56 | 75 (53,4–94) | 4,84 (2,88–6,5) | 0 (0–1,5) | 0 | 0 (0–0,5) | 0 (0–0,5) |
| 57 | 75 (53,5–94) | 4,85 (2,9–6,11) | 0 (0–1,5) | 0 | 0 (0–0,5) | 0 (0–0,5) |
| 58 | 75 (53,3–94) | 4,68 (3–6,51) | 1 (0–2,5) | 0 | 0 (0–0,5) | 0 (0–0,5) |
| 59 | 75 (52,6–94) | 4,5 (3,13–6,43) | 0 (0–1) | 0 | 0 (0–1) | 0 (0–0,5) |
| 60 | 75 (52,9–94,1) | 4,98 (3,31–6,63) | 1 (0–2) | 0 (0–0,5) | 0 (0–1) | 0 (0–0,5) |

### v3: durum değişimi (2/2)

| Yıl | Yerleşim durum değişimi (kuruluş hariç hepsi) | Orta halka kaydı (Köy/Kasaba: kademe değişimi ya da terk) | Açlık başlayan yerleşim | Salgın başlayan yerleşim | Yakılan/yanan yerleşim | Harabeye yeniden yerleşim |
|---|---|---|---|---|---|---|
| 1 | 17 (6,5–30) | 4 (0,5–7,5) | 0 | 0 (0–1) | 7 (4–12) | 0 |
| 2 | 20 (4,5–35) | 4 (0,5–8,5) | 0 (0–0,5) | 0 (0–1) | 8,5 (3–11) | 0 |
| 3 | 21 (7,5–34) | 4 (1,5–8,5) | 0 (0–9) | 0 (0–1,5) | 6 (4–13,5) | 0 |
| 4 | 15 (9,5–24,5) | 3 (1,5–6,5) | 0 | 0 (0–0,5) | 8 (4–11,5) | 0 (0–0,5) |
| 5 | 20 (9–31,5) | 4,5 (0,5–7,5) | 0 | 0 (0–1) | 7,5 (3–12,5) | 0 |
| 6 | 17,5 (4,5–33) | 5 (1–6,5) | 0 | 0 (0–1,5) | 6,5 (2–12) | 0 |
| 7 | 20,5 (6,5–33) | 4 (1–6,5) | 0 | 0 (0–1) | 9 (3–12,5) | 0 |
| 8 | 14 (7,5–26,5) | 3,5 (1–6,5) | 0 | 0 (0–1) | 6 (3,5–12) | 0 |
| 9 | 15,5 (7,5–24) | 3 (1–8) | 0 | 0 (0–2) | 7 (3–13) | 0 (0–0,5) |
| 10 | 15 (11,5–28) | 3 (1–6,5) | 0 | 0 (0–1) | 6,5 (5–12) | 0 (0–0,5) |
| 11 | 16,5 (8,5–28) | 5 (1,5–6) | 0 | 0,5 (0–1) | 7 (4–13,5) | 0 |
| 12 | 19 (10–27,5) | 3 (1–7) | 0 | 0 (0–1,5) | 9,5 (4–11,5) | 0 |
| 13 | 17 (6,5–28) | 3 (1–6,5) | 0 | 0,5 (0–1,5) | 6 (3–10) | 0 |
| 14 | 15 (7,5–21,5) | 3 (1–4) | 0 | 0,5 (0–1) | 7 (3–13) | 0 |
| 15 | 20 (4,5–27) | 3,5 (1–6,5) | 0 | 0 (0–1) | 7 (2–12) | 0 (0–0,5) |
| 16 | 16 (6–28) | 3 (1–6,5) | 0 | 0 (0–2) | 5,5 (3,5–10) | 0 |
| 17 | 15 (6–28) | 3 (1–7) | 0 | 0 (0–1) | 7,5 (3–12,5) | 0 |
| 18 | 16,5 (9,5–28,5) | 4 (0,5–6) | 0 | 0 (0–1,5) | 8 (3–12) | 0 |
| 19 | 14 (2,5–22,5) | 3 (0–6) | 0 | 0 (0–1,5) | 7 (1–11) | 0 |
| 20 | 19 (4–35,5) | 3 (0,5–6) | 0 | 0 (0–1) | 9 (2–16) | 0 |
| 21 | 20 (3,5–30) | 3 (0–9,5) | 0 | 0 (0–2) | 10 (2–14,5) | 0 |
| 22 | 15,5 (3,5–29,5) | 3 (1–4,5) | 0 | 0 (0–0,5) | 6,5 (1,5–16) | 0 |
| 23 | 18,5 (9–27) | 3 (1–7) | 0 | 0 (0–1,5) | 8 (3–12) | 0 (0–0,5) |
| 24 | 15 (6–28,5) | 3 (0,5–7) | 0 | 0 (0–1,5) | 7 (3–14) | 0 |
| 25 | 14 (8,5–28,5) | 4,5 (0,5–6,5) | 0 | 0 | 6,5 (3,5–14) | 0 (0–0,5) |
| 26 | 14,5 (6–29,5) | 3 (0,5–6,5) | 0 | 0 (0–1,5) | 7 (3,5–17) | 0 |
| 27 | 14,5 (7–19,5) | 2 (1–6,5) | 0 | 0 (0–1) | 6,5 (2–10,5) | 0 |
| 28 | 14,5 (7,5–23,5) | 3 (0–6,5) | 0 | 0 (0–1) | 6,5 (4,5–13,5) | 0 |
| 29 | 14,5 (5–33,5) | 3,5 (0,5–6) | 0 | 0 (0–1,5) | 8 (2–15,5) | 0 |
| 30 | 15 (6–26,5) | 2 (0–6,5) | 0 (0–2) | 0 (0–1) | 7,5 (2–13) | 0 |
| 31 | 14,5 (4,5–32) | 2 (0–7,5) | 0 (0–1) | 0 (0–1,5) | 5,5 (3–11,5) | 0 |
| 32 | 14 (5,5–29,5) | 2,5 (0,5–6) | 0 (0–1) | 0 (0–1,5) | 6,5 (2,5–13) | 0 |
| 33 | 12,5 (4–28) | 2,5 (0,5–4,5) | 0 (0–1) | 0 (0–1) | 5,5 (2–11) | 0 |
| 34 | 13 (4,5–28) | 3 (1,5–5,5) | 0 (0–2,5) | 0 (0–2,5) | 6 (2–9,5) | 0 |
| 35 | 14 (7–30) | 3,5 (1–7,5) | 0 (0–1) | 0 (0–2) | 6 (3–13,5) | 0 |
| 36 | 11,5 (7–25,5) | 2,5 (0,5–8,5) | 0 (0–0,5) | 0 (0–1,5) | 5,5 (1,5–10,5) | 0 |
| 37 | 14 (6–22,5) | 3 (0–6) | 0 (0–3) | 0 (0–1) | 6,5 (3–8) | 0 |
| 38 | 10 (5,5–20,5) | 2 (0,5–4,5) | 0 | 0,5 (0–1) | 6 (2–11) | 0 (0–0,5) |
| 39 | 12,5 (5,5–21) | 1 (0–4) | 0 (0–0,5) | 0 (0–1) | 6 (4–10) | 0 |
| 40 | 12 (3,5–22,5) | 2 (0–5,5) | 0 | 0 (0–0,5) | 6 (2–12) | 0 |
| 41 | 10,5 (5–22,5) | 1 (0,5–3,5) | 0 | 0 (0–3,5) | 6,5 (2,5–11,5) | 0 |
| 42 | 13 (7–22) | 2 (0–4,5) | 0 | 0 (0–1) | 6,5 (3,5–11) | 0 |
| 43 | 10 (6,5–21,5) | 2 (1–4) | 0 | 0 (0–2) | 5,5 (3–12) | 0 |
| 44 | 14 (8–21) | 3 (1–5,5) | 0 | 0 (0–1) | 6,5 (3,5–10,5) | 0 |
| 45 | 11,5 (4–21,5) | 2 (0–4) | 0 (0–0,5) | 0,5 (0–2) | 5 (2,5–9) | 0 |
| 46 | 14,5 (7,5–23,5) | 3 (1–5) | 0 (0–1) | 0,5 (0–2) | 6,5 (3–13) | 0 |
| 47 | 13 (8–21,5) | 2 (1–4,5) | 0 (0–3) | 0 (0–1) | 7 (3–9) | 0 |
| 48 | 12 (5,5–20,5) | 2 (0–3,5) | 0 (0–1,5) | 0 (0–1) | 5 (2–11,5) | 0 |
| 49 | 12 (6–21) | 2 (0–6) | 0 | 0 | 6 (3–8,5) | 0 |
| 50 | 12,5 (6–20) | 2 (0–4,5) | 0 (0–1) | 0 (0–1) | 6,5 (3–13) | 0 |
| 51 | 12,5 (5–23) | 2 (0,5–5) | 0 | 0,5 (0–1) | 5,5 (2,5–12) | 0 |
| 52 | 12,5 (5,5–23) | 2 (0,5–4,5) | 0 (0–0,5) | 0 (0–2,5) | 5,5 (2,5–12,5) | 0 |
| 53 | 10 (6,5–21,5) | 1,5 (0–7) | 0 (0–1) | 0 (0–1,5) | 5,5 (1,5–10,5) | 0 |
| 54 | 11,5 (6–23) | 2 (0–4) | 0 (0–0,5) | 0 (0–1) | 4 (2,5–12,5) | 0 |
| 55 | 8 (3–19) | 1 (0–4,5) | 0 (0–0,5) | 0 (0–1) | 5 (1–11) | 0 |
| 56 | 10 (6–26,5) | 1,5 (0–4) | 0 | 0 (0–2,5) | 5 (2,5–15) | 0 |
| 57 | 10,5 (3,5–26,5) | 2 (0–3) | 0 (0–0,5) | 0 (0–1,5) | 5 (1,5–16) | 0 |
| 58 | 12 (6–24) | 2 (1–3,5) | 0 | 0 (0–1,5) | 6,5 (3–12,5) | 0 |
| 59 | 9 (5–20) | 1 (0–3) | 0 | 0,5 (0–1) | 6 (3–11,5) | 0 |
| 60 | 11,5 (2,5–23) | 1,5 (0–5,5) | 0 | 0 (0–1,5) | 5 (2–10) | 0 |

### Devlet, inanç ve örgüt (1/5)

| Yıl | Yaşayan örgüt (yıl sonu) | Örgüt şubesi (yıl sonu) | Gizli şube (yıl sonu) | Örgüt üyesi (yıl sonu) | Açılan şube | Kapanan şube |
|---|---|---|---|---|---|---|
| 1 | 13 (12–13) | 247 (192–324) | 12 (7,5–17) | 962 (782–1232) | 24,5 (20,5–27) | 4,5 (1,5–7) |
| 2 | 13 (12–13) | 256 (200–328) | 11 (8–16,5) | 1014 (786–1258) | 18 (13,5–24) | 5,5 (2,5–12,5) |
| 3 | 13 (12–13) | 266 (205–329) | 11,5 (8–16,5) | 1050 (800–1302) | 13,5 (9,5–18) | 6,5 (2,5–11) |
| 4 | 13 (12–13) | 276 (204–337) | 11 (7–16) | 1070 (802–1370) | 13,5 (10–16,5) | 6 (1–12,5) |
| 5 | 12 (12–13) | 280 (210–344) | 11,5 (8,5–15) | 1098 (840–1410) | 13 (9,5–17,5) | 6,5 (1,5–14) |
| 6 | 12,5 (12–13) | 284 (215–345) | 10 (8–16) | 1138 (852–1425) | 10,5 (6–14) | 6 (2,5–16,5) |
| 7 | 13 (12–13) | 288 (220–348) | 10 (6–15,5) | 1122 (854–1440) | 10 (7,5–15,5) | 8,5 (3–13,5) |
| 8 | 12,5 (12–13) | 286 (224–352) | 10 (6–13,5) | 1149 (865–1475) | 11 (5,5–16,5) | 8 (3,5–14) |
| 9 | 13 (12–13) | 292 (226–358) | 10 (7–14) | 1170 (884–1499) | 11 (4,5–17,5) | 7,5 (2–12) |
| 10 | 12 (12–13) | 300 (227–362) | 10 (6,5–13,5) | 1190 (936–1524) | 9 (6,5–15) | 6,5 (3,5–10,5) |
| 11 | 12 (12–13) | 304 (228–365) | 9 (6,5–15) | 1196 (928–1562) | 9,5 (6–16) | 8 (3,5–14,5) |
| 12 | 12 (12–13) | 306 (232–366) | 8,5 (5,5–13) | 1224 (952–1568) | 11 (5,5–14) | 7 (2,5–11,5) |
| 13 | 13 (12–13) | 308 (234–375) | 10 (6–14) | 1241 (973–1572) | 10,5 (5–14,5) | 5,5 (2–11) |
| 14 | 13 (12–13) | 310 (238–386) | 9,5 (5,5–13,5) | 1271 (995–1595) | 9 (6–14) | 4 (1,5–11,5) |
| 15 | 13 (12–13) | 311 (244–391) | 10,5 (5–14,5) | 1259 (1000–1573) | 9 (5–13) | 6 (1–11,5) |
| 16 | 13 (12–13) | 314 (248–396) | 10 (4–13) | 1296 (1029–1604) | 10,5 (6–14) | 8 (3,5–11,5) |
| 17 | 13 (12–13) | 316 (248–394) | 10 (3,5–13,5) | 1280 (994–1615) | 8,5 (4–15) | 7,5 (2,5–13,5) |
| 18 | 12,5 (12–13) | 318 (247–394) | 8,5 (5,5–14) | 1306 (988–1610) | 8,5 (4,5–13,5) | 8 (2–12) |
| 19 | 13 (12–13) | 321 (248–394) | 9,5 (6,5–15,5) | 1314 (1014–1625) | 10 (3,5–14) | 5 (2–14,5) |
| 20 | 12,5 (12–13) | 328 (250–390) | 8 (6–15) | 1300 (1054–1611) | 8,5 (4–14) | 5,5 (3–14) |
| 21 | 13 (12–13) | 334 (253–384) | 8,5 (5,5–14,5) | 1336 (1068–1620) | 8,5 (3–14) | 5,5 (0,5–16) |
| 22 | 13 (12–13) | 336 (251–386) | 8,5 (4–16) | 1334 (1063–1616) | 5,5 (2–10,5) | 8,5 (1,5–12,5) |
| 23 | 13 (12–13) | 333 (248–389) | 8 (4–16,5) | 1347 (1026–1636) | 7 (4,5–12,5) | 6 (3–14) |
| 24 | 13 (12–13) | 331 (251–388) | 9 (4–16) | 1338 (1046–1637) | 9,5 (3–13) | 5,5 (2–12,5) |
| 25 | 13 (12–13) | 327 (258–388) | 7,5 (4–15) | 1344 (1062–1630) | 7,5 (4,5–12) | 6 (3–10,5) |
| 26 | 13 (12–13) | 324 (262–388) | 7,5 (5,5–14) | 1326 (1068–1678) | 8,5 (3,5–12) | 8 (3–18) |
| 27 | 13 (12–13) | 322 (263–390) | 9 (5–14,5) | 1355 (1070–1687) | 6 (4–10,5) | 5,5 (2,5–11) |
| 28 | 13 (12–13) | 321 (266–398) | 11 (5,5–14) | 1366 (1091–1664) | 7,5 (3,5–13,5) | 5 (2–13,5) |
| 29 | 13 (12–13) | 327 (263–402) | 9 (5–15) | 1377 (1086–1680) | 8 (4–13,5) | 8 (2–12) |
| 30 | 13 (12–13) | 330 (266–402) | 9 (6,5–15) | 1388 (1110–1689) | 7 (4–11,5) | 4 (1,5–10) |
| 31 | 13 (12–13) | 328 (264–398) | 10 (6–17) | 1394 (1118–1715) | 7 (3–10,5) | 5 (3–11) |
| 32 | 13 (12–13) | 328 (260–404) | 10 (7–16) | 1380 (1096–1730) | 6,5 (2–14) | 6,5 (3–16) |
| 33 | 13 (12–13) | 330 (256–406) | 10 (6,5–15) | 1386 (1112–1728) | 7 (2–12) | 7,5 (3–11) |
| 34 | 13 (12–13) | 333 (256–408) | 10 (6,5–12) | 1378 (1084–1752) | 4,5 (3–13) | 7 (2–13,5) |
| 35 | 13 (12–13) | 331 (250–410) | 10 (6–14,5) | 1374 (1098–1754) | 7 (3,5–14,5) | 6 (4–12) |
| 36 | 13 (12–13) | 328 (242–411) | 10 (6–13,5) | 1348 (1102–1716) | 6,5 (4–13,5) | 8 (4–16) |
| 37 | 12,5 (12–13) | 322 (237–404) | 10,5 (6,5–12) | 1376 (1104–1727) | 7,5 (2,5–12) | 6,5 (1–19) |
| 38 | 13 (12–13) | 315 (238–414) | 10 (6,5–12) | 1400 (1108–1744) | 8 (2–16) | 5,5 (2–13,5) |
| 39 | 13 (12–13) | 312 (240–419) | 10 (4,5–13,5) | 1384 (1124–1749) | 7 (1,5–13,5) | 5 (1,5–11) |
| 40 | 13 (12–13) | 310 (243–418) | 10 (5–13,5) | 1354 (1109–1758) | 5,5 (2–13,5) | 6 (1,5–10,5) |
| 41 | 13 (12–13) | 316 (244–417) | 11 (4,5–13) | 1360 (1120–1776) | 7,5 (1,5–12,5) | 3,5 (1–11,5) |
| 42 | 13 (12–13) | 323 (244–424) | 11 (4,5–14) | 1379 (1144–1759) | 5 (2–14) | 3 (0,5–7,5) |
| 43 | 13 (12–13) | 324 (246–423) | 10,5 (6–13) | 1348 (1150–1784) | 6,5 (2,5–12) | 5 (1–11) |
| 44 | 13 (12–13) | 328 (251–423) | 10,5 (6–13) | 1382 (1143–1760) | 7 (3,5–10) | 5 (1–10) |
| 45 | 13 (12–13) | 328 (250–423) | 10 (6,5–13,5) | 1399 (1162–1782) | 6 (2–12) | 4,5 (1,5–11) |
| 46 | 13 (12–13) | 326 (248–425) | 10,5 (6–14) | 1413 (1137–1782) | 7,5 (1–11) | 6 (1,5–15) |
| 47 | 13 (12–13) | 320 (248–424) | 11 (7–13,5) | 1391 (1136–1776) | 7,5 (3–12,5) | 7,5 (2,5–13) |
| 48 | 13 (12–13) | 321 (252–420) | 10,5 (5,5–14) | 1410 (1141–1800) | 6 (1,5–10,5) | 6 (2,5–10) |
| 49 | 13 (12–13) | 322 (254–420) | 10 (6,5–14) | 1432 (1136–1809) | 6,5 (1,5–10) | 6 (1–12) |
| 50 | 13 (12–13) | 324 (252–420) | 10,5 (5,5–15) | 1428 (1140–1816) | 4,5 (2,5–11,5) | 6,5 (2,5–13) |
| 51 | 13 (12–13) | 323 (250–421) | 10,5 (5–14) | 1408 (1148–1796) | 5 (3–9) | 4,5 (1,5–10,5) |
| 52 | 13 (12–13) | 321 (252–426) | 10 (5–14,5) | 1403 (1166–1814) | 6 (1–10) | 5,5 (2,5–10) |
| 53 | 13 (12–13) | 316 (248–432) | 10 (5,5–16,5) | 1425 (1137–1840) | 5,5 (3–9,5) | 5 (1,5–12) |
| 54 | 13 (12–13) | 316 (250–434) | 9 (5–16,5) | 1396 (1144–1804) | 6 (2–9,5) | 5 (3,5–13,5) |
| 55 | 13 (12–13) | 318 (252–437) | 9 (4,5–14) | 1425 (1106–1818) | 6,5 (1–13) | 6 (2,5–11,5) |
| 56 | 13 (12–13) | 320 (255–438) | 10 (5–14) | 1425 (1134–1818) | 5,5 (2–9,5) | 4 (0,5–8) |
| 57 | 13 (12–13) | 322 (253–443) | 11 (3,5–13) | 1414 (1144–1843) | 6 (1,5–11,5) | 5 (2–7) |
| 58 | 13 (12–13) | 322 (250–444) | 9 (3,5–14) | 1389 (1129–1815) | 4 (2–8,5) | 6 (2–11) |
| 59 | 13 (12–13) | 324 (249–442) | 9 (2–13,5) | 1401 (1144–1826) | 4,5 (0,5–9) | 5 (1–8,5) |
| 60 | 13 (12–13) | 321 (249–446) | 9 (2,5–14,5) | 1414 (1133–1872) | 7 (2,5–12,5) | 5 (0,5–10) |

### Devlet, inanç ve örgüt (2/5)

| Yıl | Dağılan örgüt | Yeniden kurulan örgüt | Gölge savaşı eylemi | Gölge savaşında öldürülen usta ya da lider | Gizli şubeye baskın | Lobiyle yasa değişikliği |
|---|---|---|---|---|---|---|
| 1 | 0 (0–1) | 0 | 8 (5–10) | 2 (0–3,5) | 1 (0–2,5) | 2 (0–2) |
| 2 | 0 (0–1) | 0 | 8,5 (4,5–10) | 2 (0,5–4) | 0 (0–1,5) | 1 (0–3) |
| 3 | 0 | 0 | 6,5 (4–10) | 1 (0–2,5) | 0,5 (0–2) | 1 (0–3) |
| 4 | 0 | 0 | 7,5 (4,5–9,5) | 1 (0,5–3) | 1 (0–2) | 1 (0–3) |
| 5 | 0 (0–1) | 0 | 5,5 (3,5–9,5) | 1,5 (0–2,5) | 0 (0–1) | 1 (0–2,5) |
| 6 | 0 | 0 (0–0,5) | 7 (3–11) | 2 (0,5–3,5) | 1 (0–1,5) | 0 (0–2,5) |
| 7 | 0 | 0 (0–0,5) | 7 (5–10,5) | 2 (0–3,5) | 1 (0–2) | 1 (0,5–2,5) |
| 8 | 0 | 0 | 6,5 (4–8,5) | 1 (0–2,5) | 1 (0–2) | 1 (0–3,5) |
| 9 | 0 | 0 | 7 (5–10) | 2 (0,5–3,5) | 1 (0–2) | 1 (0–2) |
| 10 | 0 (0–0,5) | 0 | 4 (3,5–7,5) | 0 (0–1,5) | 0 (0–2) | 1 (0–2) |
| 11 | 0 (0–0,5) | 0 (0–0,5) | 6 (3–9,5) | 1 (0–3,5) | 1 (0–2,5) | 1 (0–3,5) |
| 12 | 0 | 0 | 5,5 (3–10) | 1 (0–2) | 1 (0–1,5) | 1 (0–3) |
| 13 | 0 | 0 (0–1) | 7 (4–9,5) | 1 (0–2) | 0 (0–1,5) | 1 (0–3) |
| 14 | 0 (0–0,5) | 0 | 6,5 (2,5–10) | 1 (0–3) | 1 (0–3) | 1 (0–3) |
| 15 | 0 | 0 | 6 (3–11) | 1 (0–3) | 0,5 (0–1) | 1,5 (0–2) |
| 16 | 0 | 0 | 6 (2,5–11) | 1 (0–3) | 1 (0–2) | 1 (0–2,5) |
| 17 | 0 | 0 | 6 (4–8,5) | 2 (0–2) | 1 (0–2) | 0,5 (0–3,5) |
| 18 | 0 (0–0,5) | 0 | 7 (3,5–11,5) | 1,5 (0,5–3) | 0 (0–2,5) | 0,5 (0–4) |
| 19 | 0 | 0 (0–0,5) | 7 (3,5–9,5) | 1 (0–3) | 1 (0–1) | 0 (0–2,5) |
| 20 | 0 (0–1) | 0 | 6 (2,5–11) | 1 (0–3) | 0,5 (0–2) | 1 (0–2) |
| 21 | 0 | 0 | 5 (2,5–10) | 1 (0–2) | 1 (0–2) | 0,5 (0–3) |
| 22 | 0 | 0 | 7 (3–10) | 1 (1–2) | 0,5 (0–1,5) | 0 (0–2,5) |
| 23 | 0 | 0 | 7,5 (2,5–9,5) | 1 (0–3) | 1 (0–2,5) | 0 (0–2) |
| 24 | 0 | 0 | 6 (2,5–8,5) | 1 (0–2) | 1 (0–2,5) | 1 (0–2) |
| 25 | 0 (0–0,5) | 0 | 6 (2–7,5) | 1 (0–3,5) | 0 (0–2,5) | 0 (0–3) |
| 26 | 0 | 0 | 5,5 (3–9,5) | 1 (0–3) | 0,5 (0–1,5) | 1 (0–3,5) |
| 27 | 0 | 0 | 6 (3–9,5) | 1 (0–2,5) | 1 (0–1,5) | 0 (0–2,5) |
| 28 | 0 (0–0,5) | 0 (0–0,5) | 6,5 (3–9) | 1 (0–2) | 0 (0–1) | 1 (0–3,5) |
| 29 | 0 | 0 | 6,5 (2,5–10,5) | 1 (0–3) | 0 (0–2) | 0,5 (0–3) |
| 30 | 0 | 0 (0–0,5) | 5,5 (3,5–10,5) | 1 (0–5) | 0 (0–1) | 0 (0–2) |
| 31 | 0 | 0 | 7 (3–10) | 1,5 (0–4) | 1 (0–2) | 0 (0–1) |
| 32 | 0 (0–0,5) | 0 | 6 (2–9,5) | 2 (0,5–3) | 1 (0–2) | 0 (0–1,5) |
| 33 | 0 | 0 | 6,5 (2,5–9) | 2 (0–2) | 1 (0–2) | 1 (0–2) |
| 34 | 0 | 0 | 6,5 (2–9) | 1 (0–2) | 0 (0–2) | 0 (0–2) |
| 35 | 0 | 0 | 6 (3–7,5) | 2 (1–3) | 1 (0,5–1) | 0 (0–4) |
| 36 | 0 (0–1) | 0 (0–0,5) | 7,5 (3,5–10) | 1 (0,5–3) | 1 (0–2) | 1 (0–3) |
| 37 | 0 | 0 | 7 (2,5–9) | 1 (1–4) | 0 (0–2) | 0,5 (0–2) |
| 38 | 0 | 0 | 7 (2,5–9,5) | 2 (0–2,5) | 1 (0–2) | 0 (0–1) |
| 39 | 0 | 0 (0–0,5) | 6,5 (2–10,5) | 1 (0–3,5) | 1 (0–2) | 0 (0–2) |
| 40 | 0 (0–0,5) | 0 | 6 (2,5–9) | 1 (0–2,5) | 0 (0–1,5) | 0,5 (0–2) |
| 41 | 0 | 0 | 5,5 (4–8,5) | 2 (0–3) | 0,5 (0–2) | 1 (0–2,5) |
| 42 | 0 | 0 | 6,5 (3–11,5) | 2 (0–3) | 0 (0–2) | 0,5 (0–1,5) |
| 43 | 0 (0–1) | 0 | 7,5 (2–10,5) | 1 (0–2) | 0 (0–2,5) | 0 (0–1,5) |
| 44 | 0 | 0 (0–0,5) | 6 (3–10) | 1,5 (0–3) | 0 (0–1) | 0 (0–2) |
| 45 | 0 | 0 | 6 (2–11) | 1 (0–3) | 1 (0–2) | 0 (0–2) |
| 46 | 0 | 0 (0–0,5) | 6,5 (3,5–10) | 1 (0–2,5) | 1 (0–2) | 1 (0–2) |
| 47 | 0 | 0 | 6,5 (2,5–11,5) | 1 (0–3) | 1 (0–2) | 0 (0–3,5) |
| 48 | 0 | 0 | 7 (3–12) | 2 (0,5–4,5) | 1 (0–2) | 0 (0–1,5) |
| 49 | 0 | 0 | 6,5 (3–12) | 1 (0–2,5) | 0 (0–2) | 0 (0–1,5) |
| 50 | 0 | 0 | 8,5 (5–10) | 1 (0–4) | 0 (0–2,5) | 1 (0–2,5) |
| 51 | 0 | 0 | 6 (3,5–9) | 1,5 (0–3,5) | 1 (0–2) | 0 (0–2,5) |
| 52 | 0 | 0 | 6,5 (3,5–10) | 2 (0–3,5) | 1 (0–2) | 0 (0–1,5) |
| 53 | 0 | 0 (0–0,5) | 6 (4–8) | 1 (0–2) | 1 (0–1,5) | 0 (0–1) |
| 54 | 0 | 0 | 7 (3–11) | 1 (1–2) | 0 (0–2,5) | 0 (0–1,5) |
| 55 | 0 | 0 | 7,5 (4–11) | 1 (0–3) | 1 (0–2) | 0 (0–2) |
| 56 | 0 | 0 | 6,5 (4–9) | 2 (1–3,5) | 1 (0–2,5) | 0 (0–1) |
| 57 | 0 (0–0,5) | 0 (0–0,5) | 7 (3,5–11,5) | 1 (0–2,5) | 1 (0–2) | 0 (0–1,5) |
| 58 | 0 | 0 | 8 (3–10) | 2 (0–5) | 1 (0–2,5) | 0 (0–1,5) |
| 59 | 0 | 0 | 7,5 (3,5–10) | 2 (0–3) | 1 (0–2) | 0 (0–1,5) |
| 60 | 0 | 0 | 5,5 (3–8) | 1 (0–2,5) | 1 (0–1,5) | 0 (0–1) |

### Devlet, inanç ve örgüt (3/5)

| Yıl | Darbe girişimi | Örgüt ilanı (Avcılar) | Örgüt üyesi kahraman payı (yıl sonu) | Yönetici değişimi | Veraset krizi | Meşruiyet ortalaması (yıl sonu) |
|---|---|---|---|---|---|---|
| 1 | 0 | 2,5 (1–4) | %100 (%99–%100) | 0 (0–1) | 0 | 67,2 (65,1–69) |
| 2 | 0 | 1,5 (0–4) | %100 (%99–%100) | 0 (0–1) | 0 | 67,1 (63,4–68,8) |
| 3 | 0 | 1,5 (0–3,5) | %100 (%99–%100) | 0 (0–0,5) | 0 | 67,5 (65,1–68,9) |
| 4 | 0 | 1,5 (0–3,5) | %100 (%99–%100) | 0 (0–0,5) | 0 | 67,8 (62,7–69,6) |
| 5 | 0 | 1 (0–3,5) | %100 (%99–%100) | 0 (0–0,5) | 0 | 68,7 (62,1–70) |
| 6 | 0 | 1 (0–4) | %99 (%99–%100) | 0 (0–0,5) | 0 | 68,4 (63,2–70,5) |
| 7 | 0 | 1 (0–4) | %99 (%99–%100) | 0 (0–1) | 0 | 67,9 (62,6–70,1) |
| 8 | 0 | 1 (0–3,5) | %100 (%99–%100) | 0 (0–1,5) | 0 | 67,1 (63,1–69,7) |
| 9 | 0 | 0,5 (0–4) | %100 (%99–%100) | 0 | 0 | 67,9 (64,4–70,1) |
| 10 | 0 | 1 (0–4) | %100 (%99–%100) | 0 (0–1) | 0 | 68,2 (61,4–70,5) |
| 11 | 0 | 1 (0–3) | %99 (%99–%100) | 0 (0–1) | 0 | 66,7 (62,7–71,1) |
| 12 | 0 | 1 (0–3,5) | %99 (%99–%100) | 0 | 0 | 67,2 (63,4–71,4) |
| 13 | 0 | 1 (0–3,5) | %99 (%99–%100) | 0 (0–1) | 0 | 67,6 (65,1–69,7) |
| 14 | 0 | 2 (0,5–4) | %99 (%99–%100) | 0,5 (0–1) | 0 | 67,2 (63,3–70,1) |
| 15 | 0 | 1,5 (0–4) | %99 (%99–%100) | 0 (0–1) | 0 | 66,8 (63,1–70,2) |
| 16 | 0 | 1 (0–4) | %99 (%99–%100) | 0 (0–1) | 0 | 67 (60,2–70,5) |
| 17 | 0 | 2 (1–4) | %99 (%99–%100) | 0 (0–1) | 0 | 68,2 (60,5–69,9) |
| 18 | 0 | 2 (0–4) | %99 (%99–%100) | 0 | 0 | 68 (63,6–69,6) |
| 19 | 0 | 1,5 (0,5–4) | %99 (%99–%100) | 0 (0–1) | 0 | 66,7 (60,9–69,9) |
| 20 | 0 | 1,5 (0–4) | %99 (%99–%100) | 0 | 0 | 67,8 (63,1–71,3) |
| 21 | 0 | 2 (0,5–4) | %99 (%99–%100) | 0 (0–1) | 0 | 67,5 (62,1–71,1) |
| 22 | 0 | 1,5 (1–4) | %99 (%99–%100) | 0 (0–0,5) | 0 | 67,9 (60,2–70,5) |
| 23 | 0 | 3 (0–4) | %99 (%99–%100) | 0 (0–1,5) | 0 | 66,9 (64–71,2) |
| 24 | 0 | 2 (0–4) | %99 (%99–%100) | 0 | 0 | 66,9 (63,9–71,1) |
| 25 | 0 | 2 (0,5–4) | %99 (%98–%100) | 0 (0–1) | 0 | 67,2 (64,9–70,4) |
| 26 | 0 | 2 (0,5–4) | %100 (%98–%100) | 0 (0–1) | 0 | 67,6 (64,6–69,7) |
| 27 | 0 | 1,5 (0–4) | %100 (%98–%100) | 0 (0–1) | 0 | 67,8 (61,8–69,1) |
| 28 | 0 | 2 (0–4) | %100 (%98–%100) | 0 (0–1) | 0 | 66,4 (62,8–69) |
| 29 | 0 | 2 (0–4) | %100 (%98–%100) | 0 (0–1,5) | 0 | 67,5 (62,4–69,9) |
| 30 | 0 | 1 (0,5–4) | %100 (%99–%100) | 0 (0–0,5) | 0 | 66,8 (64,2–70,2) |
| 31 | 0 | 1,5 (0–4) | %100 (%99–%100) | 0 (0–0,5) | 0 | 67 (65,9–69,9) |
| 32 | 0 | 2,5 (0–4) | %100 (%99–%100) | 0 (0–1) | 0 | 67,6 (64,2–71) |
| 33 | 0 | 2 (0–4) | %100 (%99–%100) | 0 | 0 | 67 (63,6–71,1) |
| 34 | 0 | 1,5 (0–4) | %100 (%99–%100) | 0 (0–1) | 0 | 67,6 (63,5–70,9) |
| 35 | 0 | 2 (1–4) | %100 (%99–%100) | 0 (0–1) | 0 | 66,4 (64,6–71,2) |
| 36 | 0 | 2 (0–4) | %100 (%98–%100) | 0 (0–1) | 0 | 67,1 (64,8–71,2) |
| 37 | 0 | 2,5 (0–4) | %100 (%98–%100) | 0 (0–0,5) | 0 | 67,7 (62,2–71,8) |
| 38 | 0 | 3 (0–4) | %100 (%99–%100) | 0 (0–1) | 0 | 67,5 (62,6–72,6) |
| 39 | 0 | 2,5 (1–4) | %100 (%99–%100) | 0 (0–1) | 0 | 66,4 (61,5–72) |
| 40 | 0 | 1,5 (0–4) | %100 (%99–%100) | 0 (0–1) | 0 | 66,6 (61–71,9) |
| 41 | 0 | 2 (1–4) | %100 (%99–%100) | 0 (0–1,5) | 0 | 65,7 (60,5–70,8) |
| 42 | 0 | 2,5 (0–4) | %100 (%99–%100) | 0 (0–1) | 0 | 66,3 (62–68,8) |
| 43 | 0 | 2,5 (0–4) | %100 (%99–%100) | 0 | 0 | 66,9 (64,6–69,5) |
| 44 | 0 | 3 (0,5–4) | %100 (%99–%100) | 0 (0–1) | 0 | 68,5 (64,3–70) |
| 45 | 0 | 1,5 (1–4) | %100 (%99–%100) | 0 (0–0,5) | 0 | 68,8 (64,2–69,9) |
| 46 | 0 | 1 (0–4) | %100 (%99–%100) | 0 (0–1) | 0 | 68,8 (62,2–70,3) |
| 47 | 0 | 1 (0–4) | %100 (%99–%100) | 0 (0–1,5) | 0 | 67,2 (62,4–70,1) |
| 48 | 0 | 1,5 (0–4) | %100 (%99–%100) | 0 (0–0,5) | 0 | 67,5 (63,2–70,1) |
| 49 | 0 | 1,5 (0–4) | %100 (%99–%100) | 0 (0–1) | 0 | 67,8 (63,4–70,3) |
| 50 | 0 | 1 (0–4) | %100 (%99–%100) | 0 (0–1) | 0 | 67,6 (62,9–70,4) |
| 51 | 0 | 1 (0–4) | %100 (%99–%100) | 0 | 0 | 68,2 (65–70,8) |
| 52 | 0 | 1 (0–4) | %100 (%99–%100) | 0 | 0 | 68,4 (63,3–70,9) |
| 53 | 0 | 1 (0–4) | %100 (%99–%100) | 0 (0–1) | 0 | 68,9 (61,8–70,9) |
| 54 | 0 | 1,5 (0–4) | %100 (%99–%100) | 0 (0–0,5) | 0 | 68,8 (62,8–69,7) |
| 55 | 0 | 1 (0–4) | %100 (%99–%100) | 0 (0–0,5) | 0 | 68,3 (61,6–70,5) |
| 56 | 0 | 1 (0–4) | %100 (%99–%100) | 0 (0–1) | 0 | 67,5 (64,8–70,7) |
| 57 | 0 | 2 (0–4) | %100 (%99–%100) | 0 | 0 | 67,4 (63,8–71) |
| 58 | 0 | 1 (0–4) | %100 (%99–%100) | 0 (0–1) | 0 | 67,9 (64,2–71) |
| 59 | 0 | 1 (0–4) | %100 (%99–%100) | 1 (0–1) | 0 | 68,2 (62,2–70,8) |
| 60 | 0 | 1,5 (0–4) | %100 (%99–%100) | 0 (0–1) | 0 | 68,1 (62,7–70,1) |

### Devlet, inanç ve örgüt (4/5)

| Yıl | Pakt'a bağlı yönetici (yıl sonu) | Köle (yıl sonu) | Köle payı (nüfusun, yıl sonu) | Hapis madeninde mahkûm (yıl sonu) | Esarete düşen | Kurtulan köle (Özgürlük Ağı, kaçış, azat) |
|---|---|---|---|---|---|---|
| 1 | 0 (0–1) | 19 (2–47,5) | %0,8 (%0,1–%2,5) | 0,5 (0–3,5) | 5 (0–13) | 11 (2–18) |
| 2 | 0 (0–1) | 14,5 (0,5–38,5) | %0,6 (%0–%2) | 0 (0–1,5) | 4 (0–10,5) | 8,5 (2–19,5) |
| 3 | 0 (0–1) | 13 (0–37,5) | %0,5 (%0–%1,9) | 0 (0–1) | 7,5 (0–13,5) | 6,5 (1–12) |
| 4 | 0 (0–1) | 9 (0–32) | %0,3 (%0–%1,7) | 1 (0–3) | 2,5 (0–11) | 5 (0–16) |
| 5 | 0 (0–1) | 9 (0–24,5) | %0,3 (%0–%1,3) | 1 (0–2,5) | 1 (0–10,5) | 5 (0–14,5) |
| 6 | 0 (0–1) | 8,5 (0,5–21,5) | %0,3 (%0–%1,1) | 0 (0–3,5) | 2,5 (0–10,5) | 5,5 (0–11,5) |
| 7 | 0 (0–1) | 6 (0,5–20) | %0,2 (%0–%1) | 1 (0–2,5) | 3 (0–16,5) | 4 (0–12) |
| 8 | 0 (0–0,5) | 5,5 (0–23) | %0,2 (%0–%1,1) | 1 (0–2) | 2,5 (0–9) | 5 (0–9) |
| 9 | 0 (0–1) | 6 (0–21) | %0,2 (%0–%1) | 1 (0–2) | 3 (0–9) | 3,5 (0–12) |
| 10 | 0 | 7,5 (0–20) | %0,3 (%0–%1) | 1 (0–2) | 6,5 (0–10,5) | 2 (0–10,5) |
| 11 | 0 (0–0,5) | 6,5 (0–17) | %0,3 (%0–%0,9) | 1 (0–2,5) | 2 (0–11) | 4,5 (0–9,5) |
| 12 | 0 (0–1) | 5,5 (0–18,5) | %0,2 (%0–%0,9) | 1 (0–3) | 3 (0–10) | 6,5 (0–12) |
| 13 | 0 (0–1) | 5,5 (0–15,5) | %0,2 (%0–%0,8) | 1 (0–3,5) | 3,5 (0–11) | 4,5 (0–10,5) |
| 14 | 0 (0–1) | 5 (0–19) | %0,2 (%0–%0,9) | 1 (0–3) | 1,5 (0–15) | 2,5 (0–11) |
| 15 | 0 (0–1) | 9,5 (0–18,5) | %0,4 (%0–%0,8) | 1 (0–2) | 4 (0–14,5) | 4 (0–13) |
| 16 | 0 | 8 (0–16,5) | %0,4 (%0–%0,8) | 0,5 (0–2) | 3 (0–12) | 3,5 (0–13) |
| 17 | 0 | 7 (0–14,5) | %0,3 (%0–%0,8) | 1 (0–2) | 3 (0–7) | 4,5 (0–8) |
| 18 | 0 (0–0,5) | 8 (0–22) | %0,3 (%0–%1,2) | 1 (0–2) | 3,5 (0–15) | 3,5 (0–8,5) |
| 19 | 0 | 8 (0–27) | %0,3 (%0–%1,3) | 1 (0–3) | 5 (0–18,5) | 5 (0–10) |
| 20 | 0 | 6,5 (0–21,5) | %0,2 (%0–%0,9) | 1 (0–3) | 3 (0–9,5) | 5,5 (0–12) |
| 21 | 0 | 8 (0–20,5) | %0,3 (%0–%0,8) | 1 (0–3,5) | 3,5 (0–10) | 4 (0–11,5) |
| 22 | 0 | 5,5 (0–26) | %0,2 (%0–%0,9) | 1 (0–3,5) | 2 (0–14,5) | 4 (0–10,5) |
| 23 | 0 (0–0,5) | 5 (0,5–22,5) | %0,2 (%0–%0,8) | 1 (0–2,5) | 4,5 (0,5–7,5) | 3,5 (0–10,5) |
| 24 | 0 (0–1) | 6 (0–23) | %0,3 (%0–%0,9) | 0 (0–1,5) | 2,5 (0–8) | 4 (0–7) |
| 25 | 0 (0–0,5) | 4 (0,5–22,5) | %0,2 (%0–%1) | 1 (0–4) | 3 (0–10,5) | 4 (0–9,5) |
| 26 | 0 | 7,5 (0–23,5) | %0,3 (%0–%1,1) | 1 (0–2) | 2 (0–14) | 3,5 (0–9,5) |
| 27 | 0 | 8 (0,5–22) | %0,3 (%0–%1,1) | 1 (0–2) | 2 (0–14,5) | 3 (0–12) |
| 28 | 0 | 6 (0,5–19) | %0,2 (%0–%1,1) | 1 (0–2) | 3 (0–9,5) | 3 (0–9) |
| 29 | 0 (0–1) | 5 (0,5–20) | %0,2 (%0–%1,1) | 0 (0–2) | 2 (0–7) | 3 (0–9,5) |
| 30 | 0 (0–0,5) | 6,5 (0–24,5) | %0,2 (%0–%1,3) | 1 (0–2) | 3,5 (0–15) | 2 (0–11) |
| 31 | 0 (0–1) | 5 (0,5–21) | %0,2 (%0–%1,2) | 0,5 (0–2,5) | 1,5 (0–5) | 3,5 (0–7,5) |
| 32 | 0 | 6 (0–21) | %0,2 (%0–%1,2) | 1 (0–2) | 3 (0–11,5) | 3 (0–8,5) |
| 33 | 0 (0–0,5) | 7 (0,5–16,5) | %0,3 (%0–%0,9) | 0 (0–2,5) | 1,5 (0–6,5) | 2 (0–9) |
| 34 | 0 | 6 (0–24,5) | %0,2 (%0–%1,3) | 0 (0–2) | 1 (0–19) | 3 (0–10) |
| 35 | 0 (0–0,5) | 4 (0–22) | %0,1 (%0–%1,2) | 1 (0–4,5) | 2 (0–13,5) | 3 (0–11,5) |
| 36 | 0 (0–1) | 6,5 (1–25) | %0,2 (%0–%1,2) | 1 (0–3) | 3,5 (0–7,5) | 3 (0–10,5) |
| 37 | 0 (0–0,5) | 7 (1–23) | %0,2 (%0–%1,1) | 1 (0–1,5) | 5 (0–9,5) | 3 (0–11,5) |
| 38 | 0 | 5,5 (0,5–28,5) | %0,3 (%0–%1,3) | 1 (0–4) | 2,5 (0–13,5) | 3 (0–12) |
| 39 | 0 (0–1) | 4 (0–32,5) | %0,2 (%0–%1,6) | 1 (0–3) | 1,5 (0–12,5) | 3 (0–12) |
| 40 | 0 (0–1) | 4,5 (0,5–29) | %0,2 (%0–%1,4) | 1 (0–4) | 2,5 (0–12) | 3 (0–10) |
| 41 | 0 (0–1) | 5 (0,5–28) | %0,2 (%0–%1,3) | 1 (0–2,5) | 2,5 (0–11) | 2,5 (0–11) |
| 42 | 0 | 6 (0,5–22,5) | %0,2 (%0–%1,2) | 1 (0–3) | 2,5 (0–13,5) | 3 (0–9,5) |
| 43 | 0 | 4,5 (1–27) | %0,2 (%0–%1,2) | 0 (0–1) | 4,5 (0–9,5) | 3,5 (0–9,5) |
| 44 | 0 (0–0,5) | 4,5 (1–32) | %0,2 (%0–%1,5) | 1 (0–2,5) | 0,5 (0–16,5) | 3 (0–14,5) |
| 45 | 0 (0–1) | 5,5 (0,5–31,5) | %0,2 (%0–%1,5) | 1 (0–4,5) | 3 (0–12,5) | 2,5 (0–9,5) |
| 46 | 0 | 6 (0,5–26,5) | %0,2 (%0–%1,3) | 1 (0–3) | 1,5 (0–11) | 2 (0–10,5) |
| 47 | 0 | 9 (0,5–26,5) | %0,4 (%0–%1,3) | 1 (0–2) | 2 (0–11) | 3,5 (0–10,5) |
| 48 | 0 | 7 (0,5–29,5) | %0,3 (%0–%1,4) | 0,5 (0–5) | 3 (0–13,5) | 4,5 (0–12) |
| 49 | 0 (0–1) | 6 (0,5–26,5) | %0,2 (%0–%1,2) | 1 (0–4,5) | 2 (0–12) | 5 (0–12) |
| 50 | 0 | 4,5 (0,5–30) | %0,1 (%0–%1,3) | 1 (0–4,5) | 2,5 (0–8) | 4 (0–8) |
| 51 | 0 | 5 (1–32,5) | %0,2 (%0–%1,6) | 1 (0–5) | 2 (0–17) | 3 (0–10,5) |
| 52 | 0 (0–0,5) | 4,5 (1–38) | %0,2 (%0–%1,8) | 0,5 (0–6,5) | 3 (0–12) | 4 (0–12,5) |
| 53 | 0 | 4,5 (1–38,5) | %0,2 (%0–%1,9) | 2,5 (0–7) | 4 (0–10,5) | 3,5 (0–9) |
| 54 | 0 | 4 (1–43) | %0,2 (%0–%1,9) | 2 (0–3) | 2 (0–14,5) | 3 (0–11) |
| 55 | 0 | 4 (0,5–39) | %0,1 (%0–%1,7) | 1 (0–7,5) | 1 (0–5) | 2,5 (0–7,5) |
| 56 | 0 | 5 (1–36,5) | %0,2 (%0–%1,9) | 1 (0–7,5) | 3 (0–13) | 2 (0–10,5) |
| 57 | 0 | 5 (1–41) | %0,2 (%0–%2) | 1 (0–9,5) | 2 (0–15,5) | 2 (0–7) |
| 58 | 0 | 3,5 (1–39) | %0,1 (%0–%2) | 1 (0–9) | 1 (0–15) | 3 (0–10,5) |
| 59 | 0 | 9,5 (1–41) | %0,3 (%0–%2,1) | 1 (0–9,5) | 4 (0–15,5) | 3 (0–8,5) |
| 60 | 0 | 7,5 (0,5–27,5) | %0,3 (%0–%1,5) | 1 (0–9) | 3 (0–7) | 4,5 (0–14,5) |

### Devlet, inanç ve örgüt (5/5)

| Yıl | Özgürlük Ağı'nın kurtardığı köle | Esir kahraman (yıl sonu) | Aç haydut kampı (yıl sonu) | Haydut olan aç halk | Aç ya da ekmeksiz yerleşim payı (köy+) | Devriye durdurması |
|---|---|---|---|---|---|---|
| 1 | 10 (2–15,5) | 0 (0–1,5) | 0 | 0 (0–3) | %0 (%0–%3,1) | 15,5 (7–50) |
| 2 | 7 (1,5–15,5) | 1 (0–2,5) | 0 | 0 (0–1,5) | %0,1 (%0–%2,4) | 19,5 (5,5–53) |
| 3 | 5,5 (1–10,5) | 0 (0–1,5) | 0 | 0 | %0,1 (%0–%2,8) | 20,5 (6,5–45,5) |
| 4 | 4,5 (0–10,5) | 0 (0–1) | 0 | 0 | %0 (%0–%3,3) | 15 (9,5–46) |
| 5 | 5 (0–11) | 0 (0–2) | 0 | 0 | %0 (%0–%1,4) | 16 (10–38) |
| 6 | 4 (0–9,5) | 0,5 (0–2,5) | 0 | 0 | %0 (%0–%1,7) | 18,5 (9,5–37,5) |
| 7 | 4 (0–10,5) | 1 (0–2) | 0 | 0 | %0,1 (%0–%2,7) | 18 (8–34,5) |
| 8 | 3 (0–9) | 0,5 (0–2,5) | 0 | 0 | %0 (%0–%2,2) | 26,5 (13,5–36,5) |
| 9 | 3,5 (0–10,5) | 0 (0–2) | 0 | 0 | %0 (%0–%3,2) | 20,5 (9,5–35) |
| 10 | 2 (0–8,5) | 0 (0–1) | 0 | 0 (0–1,5) | %0 (%0–%4,2) | 16,5 (9,5–38,5) |
| 11 | 4 (0–7,5) | 0 (0–1) | 0 | 0 | %0 (%0–%5,1) | 22,5 (11,5–46) |
| 12 | 5,5 (0–10,5) | 1 (0–2) | 0 | 0 (0–1,5) | %0 (%0–%6,7) | 19 (8–51) |
| 13 | 4 (0–8,5) | 1 (0–1,5) | 0 | 0 (0–3,5) | %0,4 (%0–%6,8) | 21,5 (11,5–51) |
| 14 | 2,5 (0–8) | 0 (0–1) | 0 (0–0,5) | 0 (0–4,5) | %0 (%0–%8,5) | 20 (9,5–38) |
| 15 | 4 (0–10) | 0,5 (0–2) | 0 | 0 (0–7) | %1,7 (%0–%5,5) | 20,5 (10–51) |
| 16 | 3 (0–8,5) | 1 (0–2) | 0 | 0 (0–1,5) | %2 (%0–%6,8) | 18 (10,5–47) |
| 17 | 3,5 (0–6,5) | 1 (0–2) | 0 | 0 (0–3) | %1,1 (%0–%5,8) | 21,5 (11–46,5) |
| 18 | 3 (0–7,5) | 1 (0–2,5) | 0 | 0 (0–3) | %0,9 (%0–%4,6) | 22 (11–48) |
| 19 | 4,5 (0–9,5) | 1 (0–2) | 0 | 0 (0–3) | %1,2 (%0–%5,6) | 24,5 (13,5–48) |
| 20 | 4,5 (0–10) | 0 (0–2,5) | 0 | 0 (0–4,5) | %1,7 (%0–%6,3) | 20,5 (10–60) |
| 21 | 3 (0–9) | 0,5 (0–2,5) | 0 | 0 (0–3,5) | %1,1 (%0–%5,7) | 22,5 (12–51) |
| 22 | 3 (0–8,5) | 0 (0–2) | 0 | 0 (0–1,5) | %0,6 (%0–%3,5) | 21 (8–42,5) |
| 23 | 3,5 (0–9) | 0 (0–1,5) | 0 (0–0,5) | 0 (0–6) | %1 (%0–%5,5) | 26 (9,5–47,5) |
| 24 | 3,5 (0–6) | 0 (0–1) | 0 | 0 (0–3,5) | %0,8 (%0–%4) | 23 (8–45,5) |
| 25 | 2,5 (0–8,5) | 0 (0–2,5) | 0 | 0 (0–1,5) | %1,7 (%0–%4,6) | 20 (14,5–53) |
| 26 | 2 (0–8) | 0 (0–2) | 0 | 0 (0–4,5) | %0,6 (%0–%4,2) | 28 (11–56,5) |
| 27 | 3 (0–9,5) | 0 (0–1,5) | 0 (0–1) | 0 (0–5,5) | %0,2 (%0–%5) | 25 (8,5–52,5) |
| 28 | 3 (0–8) | 0,5 (0–2) | 0 | 0 (0–4) | %1 (%0–%5,7) | 28 (13,5–47,5) |
| 29 | 2,5 (0–7) | 0 (0–1) | 0 | 0 (0–4) | %1,3 (%0–%5,7) | 19 (13–59) |
| 30 | 2 (0–10) | 0,5 (0–2) | 0 (0–1) | 0 (0–7) | %2,1 (%0–%8,3) | 25,5 (14,5–48) |
| 31 | 3 (0–7) | 1 (0–2) | 0 (0–0,5) | 0 (0–4,5) | %1,9 (%0–%8,7) | 25,5 (12,5–48) |
| 32 | 2 (0–6,5) | 0,5 (0–2,5) | 0 (0–1) | 0 (0–3) | %2 (%0–%6) | 26,5 (11,5–47) |
| 33 | 2 (0–8,5) | 1 (0–2,5) | 0 (0–0,5) | 0 (0–3) | %1,4 (%0–%6,1) | 24 (9–52) |
| 34 | 2,5 (0–8,5) | 0,5 (0–3,5) | 0 | 0 (0–2) | %1,9 (%0–%3,8) | 23,5 (8–55,5) |
| 35 | 2 (0–9,5) | 1 (0–3) | 0 (0–1) | 0 (0–7) | %1,9 (%0–%5,9) | 21,5 (8,5–58) |
| 36 | 2 (0–8,5) | 0 (0–2,5) | 0 (0–1) | 0 (0–3) | %1,6 (%0–%5,7) | 24 (11–48) |
| 37 | 3 (0–8) | 0,5 (0–3) | 0 (0–0,5) | 0 (0–1,5) | %2,5 (%0,1–%4,8) | 30 (12–52) |
| 38 | 2,5 (0–8,5) | 1 (0–2) | 0 (0–0,5) | 0 (0–7) | %2,8 (%0,1–%5,5) | 24,5 (10,5–57) |
| 39 | 3 (0–10) | 1 (0–2) | 0 (0–0,5) | 0 | %2,1 (%0,1–%5,4) | 23 (10–60) |
| 40 | 3 (0–8,5) | 0 (0–2,5) | 0 | 0 (0–3) | %1,8 (%0–%4,7) | 22,5 (11,5–60) |
| 41 | 2,5 (0–9) | 0,5 (0–1) | 0 (0–0,5) | 0 (0–3,5) | %1,7 (%0–%6) | 20,5 (12–57) |
| 42 | 2 (0–8,5) | 0 (0–1) | 0 | 0 (0–5) | %1,4 (%0,1–%8,1) | 27 (14,5–62,5) |
| 43 | 3 (0–8,5) | 1 (0–2,5) | 0 | 0 (0–3) | %1,9 (%0–%7,6) | 22,5 (9–56) |
| 44 | 2,5 (0–10) | 0 (0–2,5) | 0 (0–1) | 0 (0–6) | %2,2 (%0–%8,9) | 25 (9–55,5) |
| 45 | 2 (0–8,5) | 0 (0–2) | 0 (0–0,5) | 0 (0–1,5) | %1,2 (%0–%7,9) | 30 (10,5–62,5) |
| 46 | 2 (0–8) | 0 (0–2,5) | 0 | 0 | %1 (%0–%3,3) | 28 (8,5–46,5) |
| 47 | 3 (0–8) | 0 (0–2) | 0 (0–0,5) | 0 (0–3) | %1,1 (%0–%3,8) | 23 (10,5–60,5) |
| 48 | 2,5 (0–10,5) | 1 (0–1,5) | 0 | 0 (0–4,5) | %1,6 (%0,1–%4,1) | 29,5 (9–59) |
| 49 | 4 (0–9) | 0,5 (0–2) | 0 | 0 | %1,4 (%0,1–%3) | 27,5 (10,5–55,5) |
| 50 | 3,5 (0–7) | 1 (0–2) | 0 | 0 | %1,9 (%0–%3,5) | 26 (14–55) |
| 51 | 2 (0–9,5) | 0,5 (0–2) | 0 | 0 | %1,4 (%0–%2,8) | 28,5 (9,5–64) |
| 52 | 4 (0–6) | 0,5 (0–2,5) | 0 (0–0,5) | 0 (0–2) | %0,9 (%0–%3,2) | 31,5 (15–52) |
| 53 | 2 (0–7,5) | 1 (0–2) | 0 (0–1) | 0 (0–4,5) | %1,4 (%0–%4,4) | 24 (14–53) |
| 54 | 2,5 (0–8,5) | 0,5 (0–3) | 0 (0–0,5) | 0 (0–3) | %2 (%0–%3,5) | 31,5 (15,5–56,5) |
| 55 | 2 (0–5,5) | 1 (0–2,5) | 0 (0–1) | 0 (0–3,5) | %1,3 (%0–%3,1) | 27 (15,5–52) |
| 56 | 2 (0–5) | 0,5 (0–2,5) | 0 (0–0,5) | 0 | %0,4 (%0–%2,8) | 29,5 (11–58,5) |
| 57 | 2 (0–5,5) | 0,5 (0–2,5) | 0 (0–0,5) | 0 | %0,9 (%0–%3,4) | 31 (11–59,5) |
| 58 | 1,5 (0–7,5) | 0,5 (0–2) | 0 (0–0,5) | 0 (0–1,5) | %0,7 (%0–%3,1) | 33 (12,5–46) |
| 59 | 2,5 (0–6) | 0 (0–1,5) | 0 (0–0,5) | 0 (0–2) | %0,7 (%0–%3,7) | 26,5 (9–49,5) |
| 60 | 3,5 (0–9,5) | 0,5 (0–1) | 0 (0–0,5) | 0 | %1,2 (%0–%3,4) | 28,5 (11,5–50,5) |

