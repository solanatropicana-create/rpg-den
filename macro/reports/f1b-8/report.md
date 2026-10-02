# Ölçüm raporu: Faz 1b-8: başsız ölçütler

16 dünya (seed 1-16) × 60 yıl (2400 gün), 1 yıl = 40 gün · 2026-10-02 05:20 · `FD.Macro.Run stats --seeds 1-16 --years 60 --jobs 2 --verify 1 --saveload 1`

Süre: 18 dk 49 sn duvar saati, 2 iş parçacığı; dünya başına 121 sn (en az 106, en çok 186; yıl sonu hash'leri dâhil).

## Bitiş ölçütleri

DESIGN-FAZ1.md, "Bitiş ölçütleri" (1–5), yol haritası v3 (6–7, Faz 1b-4) ve v3'ün süre tablosu (8, Faz 1b-5). ✓ geçti · ✗ kaldı · — ölçülemedi · ○ bilgi (hedef yok) · † zaman ölçeğine bağlı (Faz 1b-5'ten beri takvim yeni ölçekte: 1 yıl = 40 gün, değerler doğrudan okunur).

| # | Ölçüt | Koşul | Ölçülen | Sonuç |
|---|---|---|---|---|
| 1 | Donma yok | 41–60. yılların yıllık büyük olay medyanı ≥ 0,8 × 6–20. yılların medyanı | 262 / 223 = 1,17 kat | ✓ |
| 2 | Çöküş | dünyaların ≥ %75'inde 60 yılda ≥ 1 çöküş (yok olma ya da başkent kaybı) | %100 (16/16 dünya); toplam 154 çöküş: 48 yok olma, 106 başkent kaybı | ✓ |
| 3 | Kamplar | 41–60. yıllarda yaşayan kamp medyanı bantta (8–10, ±1) ve 6–20. yılların en az %90'ı (v3: sabit bant) | geç 8,93, erken 8,73 (oran %102; yıl sonu sayımıyla 9 / 9) | ✓ |
| 4a | Kahraman: doğuş seviyesi | her on yılda doğanların ortalama seviyesi ≤ 2 | 1,63 · 1,63 · 1,61 · 1,6 · 1,66 · 1,64 (on yıllar sırasıyla) | ✓ |
| 4b | Kahraman: Sv8+ | dünyaların ≥ yarısında en az bir kahraman Sv8 ve üstüne çıkar | %100 (16/16 dünya); dünyadaki en yüksek seviye: medyan Sv10, en çok Sv10 | ✓ |
| 4c | Kahraman: efsane | dünya başına efsane medyanı 1–6 | medyan 2 (p10–p90: 0–6; toplam 42) | ✓ |
| 4d | Kahraman: ölüm payı | doğan kahramanların %30–80'i ölür | %25 (2790/11207); dünya medyanı %29 (%16–%36) | ✗ |
| 5a | Determinizm | aynı seed → aynı tarih (toplayıcılı ve toplayıcısız koşu, her yıl sonu hash'i) | seed 1: 60/60 yıl sonu aynı | ✓ |
| 5b | Kayıt/yükleme | kaydet → yükle → devam = kesintisiz koşu (her yıl sonu hash'i) | seed 1, gün 1237: yüklenen dünya aynı, sonraki 30/30 yıl sonu aynı | ✓ |
| 6a | Yerleşim sayısı sabit, ısınmadan sonra (21–60. yıl) | dünya başına yaşayan yerleşim sayısının (günlük) değişim katsayısı medyanı ≤ %10 | değişim katsayısı %1,9 (%0,7–%5,7); ortalama 77 yerleşim, en az ve en çok ortalamanın %92 ve %102 kadarı | ✓ |
| 6b | Yerleşim sayısı sabit, bütün koşu (1–60. yıl) | dünya başına yaşayan yerleşim sayısının (günlük) değişim katsayısı medyanı ≤ %10 (Faz 1b-6: dünya tarih öncesiyle olgun başlar) | değişim katsayısı %2,3 (%0,7–%6,3); ortalama 77,1 yerleşim, en az ve en çok ortalamanın %91 ve %102 kadarı | ✓ |
| 6c | El değiştirme (her yerleşim) | fetih + bölünme + içeriden düşüş (Faz 1b-7), 100 günde, dünya başına ve yerleşim başına (21–60. yıl); hedef yok | dünyada 7,44 (4,91–10,6) / 100 gün; yerleşim başına 9,8 / 10 bin gün: 7,44 / 100 gün | ○ |
| 6d | Durum dalgalanması (yerleşim başına) | açlık, salgın, kuşatma, yakılma, kademe, el değiştirme, terk, yeniden yerleşim: yerleşim başına 100 günde (21–60. yıl); hedef yok | yerleşim başına 1,54 (1,09–1,93) / 100 gün, yani ~65,1 günde bir; dünyada 118 / 100 gün: yerleşim başına ~65,1 günde bir | ○ |
| 6e | Orta halka: kademe değişimi † | Köy/Kasaba başına kademe değişimi ya da terk, yeni takvimde 60–150 günde bir (21–60. yıl) | yerleşim başına 126 (111–149) günde bir | ✓ |
| 6f | Büyük şehir el değiştirmesi † | büyük şehir (Şehir kademesi) bütün dünyada yeni takvimde 100 günde 2–4 kez el değiştirir (21–60. yıl): dışarıdan (fetih, bölünme) ya da içeriden (Faz 1b-7: tipin çöküş yolu; yönetim değişir ya da şehir ayrılır); felaketle düşmez | dünyada 2,63 (2,22–3,38) / 100 gün; şehir başına 0,53 / 100 gün; dünyada ortalama 5,38 büyük şehir; toplam 694 el değiştirme (59 fetih, 2 bölünme, 633 içeriden: soylu isyanı 240, düello 160, darbe 130, mezhep bölünmesi 43, veraset 18, aforoz 18, ayrılık 13, boyların ayrılması 11). Ayrıştırma: dünyada 5,78 savaş / 100 gün, hedefi büyük şehir olan %15; büyük şehre 290 hücum, düşüşle bitenlerin payı %20 | ✓ |
| 6g | Büyük şehir: uyarı süresi † | büyük şehrin düşüşünden önce yeni takvimde 5–10 gün uyarı (kuşatmanın ya da iç krizin başı → düşüş, medyan; 21–60. yıl) | 7 gün (5–10; n = 692): kuşatmanın başından 5 (n = 59), iç krizin başından 8 (n = 633); savaşın başından 14 gün (11–37) | ✓ |
| 6h | Savaş süresi † | savaş (ilandan barışa) yeni takvimde 10–40 gün (medyan; 21–60. yıl başlayıp biten savaşlar) | 13 gün (10–38; n = 1423) | ✓ |
| 6i | Kuşatma süresi † | büyük şehir kuşatması (karargâhtan hücuma) yeni takvimde 2–6 gün (medyan; 21–60. yıl) | büyük şehir 6 gün (6–6; n = 290); diğer yerleşimler 3 gün (3–6; n = 1174) | ✓ |
| 6j | Başkent kaybı (medeniyet başına) | hiçbir medeniyet başkentini 3 kereden çok kaybetmez (yol haritası: "en fazla birkaç kez") | en çok 3; 3'ten çok kaybeden 0 medeniyet; toplam 106 başkent kaybı, 75/188 medeniyette (1×: 51, 2×: 17, 3×: 7) | ✓ |
| 7 | Felaket büyük şehri düşürmez | Şehir kademesine varmış yerleşim hiç terk edilmez; ejderha akını büyük şehrin nüfusunu Şehir eşiğinin (85) altına indiremez; akından sonraki 60 günde terk yok | terk edilen eski Şehir: 0; ejderha akını 429 (büyük şehre 31, orada 355 ölü): akınla eşiğin altına inen 0, sonraki 60 günde terk 0; bilgi: akın günü başka nedenlerle (ordu, öncü) eşiğin altına inen 0, 60 gün içinde kademe düşüşü 6, el değiştirme 4 | ✓ |
| 8a | Salgın süresi | salgın başladığı günden bittiği güne: 5–10 gün (medyan) | 5 gün (4–7; n = 573) | ✓ |
| 8b | Tepki inşaatı: yanan ev | yanan her ev yandığı günden onarıldığı güne (ilk yanan ilk onarılır): 1–3 gün (medyan) | 3 gün (2–5; n = 17141) | ✓ |
| 8c | Tepki inşaatı: sur | palisat ve taş sur: projenin başından bitişine: 5–10 gün (medyan) | 7 gün (5–10; n = 874); palisat 7, taş sur 8 gün | ✓ |
| 8d | Büyük proje (kale, kule) | kale ve fener kulesi: projenin başından bitişine: 15–30 gün (medyan) | 25 gün (23–30; n = 307); kale 25 (n = 242), kule 23 gün (n = 65) | ✓ |
| 8e | Temizlenen kamp → yeni köy | temizlenen kara kampının vadisine 60 gün içinde köy kurulması (Faz 1b-8: kampın vadisine giden öncüler ya da kalıcı köye dönen verimli vadi merkezi; yakındaki ilgisiz kuruluşlar sayılmaz): 10–20 gün (medyan) | 13 gün (9–19; n = 269); temizlenen 5148 kampın 269 tanesine (%5,2) köy kuruldu, 20 gün içinde %4,9 | ✓ |
| 8f | Han kurulumu | yeni han: hancının yola çıktığı günden kapıların açıldığı güne: 5–10 gün (medyan) | 8 gün (6–10,8; n = 13); harabeyi yeniden kurma 3 gün (n = 14) | ✓ |
| 8g | Kahraman doğumu (han) | han başına 10–20 günde bir (açık han-günü / handa doğan kahraman) | 15,3 günde bir (11006 doğum / 168857 han-günü); aynı handa iki doğum arası medyan 12 gün (2–34); bilgi: taverna başına 5398 günde bir (201 doğum) | ✓ |
| 8h | Kahramanın efsaneye yükselişi | doğumundan efsane olduğu güne: 100–300 gün (medyan) | 236 gün (86,2–891; n = 42) | ✓ |
| 8i | İlan ömrü | alınmayan ilanın asıldığı günden kapandığı güne (süresi doldu ya da kampı başkası temizledi): 10–20 gün (medyan) | 15 gün (1–15; n = 4214); bütün ilanlar 5 gün (n = 15800): biten %39 (asılıştan 4 günde), süresi dolan %24; başarısız sefer ödülü %25 artırır (Heroes.QuestFailed) | ✓ |
| 8j | Yoldaş/kahraman maaşı | haftalık (5 gün) | kural: medeniyetin kahramanları her 5. gün (Economy.PayHeroes), han personeli haftada bir (InnLife.PayWages); maaşı 3 hafta ödenmeyen kahraman ayrılır | ○ |
| 9a | Örgütler yaşar, şubeleri dalgalanır | hiçbir örgüt kalıcı olarak yok olmaz (koşu sonunda ya yaşıyor ya da yeniden kurulmayı bekliyor); örgüt başına yıllık şube sayısının değişim katsayısı medyanı ≥ %10 | kalıcı yok olan 0/208; koşu sonunda yaşayan 200; dağılma 73, yeniden kuruluş 65; şube değişim katsayısı %20 (%9,6–%60); açılan 15461, kapanan 15072 şube (966 / 942 dünya başına) | ✓ |
| 9b | Gölge savaşı düzenli | dünya-yıllarının ≥ %90'ında en az bir gölge savaşı eylemi (suikast, sabotaj, ihbar); hedef sıklık sonra ayarlanacak | en az bir eylemi olan dünya-yılı %99; dünyada 17,5 (12,8–20,7) / 100 gün; başarısız %57; öldürülen usta/lider 1394; türler: suikast 3381, sabotaj 1718, ihbar 1455 | ✓ |
| 9c | Tiplerin çöküş nedenleri farklı | dört hükümet tipinin en sık çöküş nedeni birbirinden farklı (başkent fethi, bölünme ya da içeriden düşüş: veraset savaşı, soylu isyanı, düello, boyların ayrılması, darbe, paralı askerler, mezhep bölünmesi, aforoz) | Krallık: soylu isyanı (başkent fethi 51, bölünme 19, veraset savaşı 42, soylu isyanı 482, darbe 1, Pakt bağı ve aforoz 22; 2544 devlet-yılı); Boylar: reisin düelloda ölümü (başkent fethi 42, bölünme 12, reisin düelloda ölümü 327, boyların ayrılması 25; 1392 devlet-yılı); Cumhuriyet: darbe (başkent fethi 36, bölünme 33, darbe 217; 1245 devlet-yılı); Teokrasi: mezhep bölünmesi (başkent fethi 20, bölünme 4, soylu isyanı 1, mezhep bölünmesi 89, Pakt bağı ve aforoz 11; 1041 devlet-yılı) | ✓ |
| 9d | Esaret sınırlı, Özgürlük Ağı etkin | köle payı her dünya-yılında nüfusun ≤ %5'i; Özgürlük Ağı dünyaların ≥ %75'inde köle kurtarır | köle payı medyan %0,5 (p90 %1,3, en çok %3,5); esarete düşen 7196 (av 1488, savaş 2397, borç 1847, baskın 1464); hapis madeni 3762; Özgürlük Ağı 16/16 dünyada 5378 köle kurtardı; kaçan 957, azat 765; esir düşen kahraman 899 | ✓ |
| 9e | Devriye profili tipe göre ayrışır | kervan başına durdurma, el koyma ve rüşvet: tipler arasında en yüksek / en düşük durdurma oranı ≥ 1,5, el koyma ve rüşvet oranları ≥ 2 | Krallık: 100 kervan-günde 7,3 durdurma, durdurmada el koyma %5, rüşvet %4,4, haraç/vergi %42, tutuklama 2, düello 0; Boylar: 100 kervan-günde 4,9 durdurma, durdurmada el koyma %3,2, rüşvet %7,3, haraç/vergi %57, tutuklama 0, düello 2015; Cumhuriyet: 100 kervan-günde 6,24 durdurma, durdurmada el koyma %2,1, rüşvet %11, haraç/vergi %73, tutuklama 0, düello 0; Teokrasi: 100 kervan-günde 9,45 durdurma, durdurmada el koyma %7,5, rüşvet %4,8, haraç/vergi %29, tutuklama 720, düello 0 (oranlar: durdurma ×1,93, el koyma ×3,64, rüşvet ×2,56) | ✓ |
| 9f | Aç haydutlar kıtlıkla ilişkili | dünya-yılı başına aç ya da ekmeksiz yerleşim payı ile haydut olan aç halk arasında korelasyon r ≥ 0,3 (en az 30 haydut) | r = 0,82 (960 dünya-yılı); haydut olan 11653, aç haydut kampı 2652, yiyecek verilip dağılan 435, açlıktan eriyen 933; aç ya da ekmeksiz yerleşim payı medyanı %6,2 | ✓ |
| 6k | Durum tablosu | her yerleşimde 5–15 günde bir zar (yerleşim-günü / zar ortalaması 5–15); zarla gelen durum 3–15 gün sürer (medyan, p10 ≥ 3, p90 ≤ 15) | zar ortalama 10 günde bir (292233 zar); zarla gelen durum 8 gün (4–13; n = 89469); yerleşim-günlerinin %31'inde bir durum var | ✓ |
| 6l | Fırsat merkezi döngüsü | döngüler kurulur ve çöker: her dünyada ≥ 5 tamamlanan döngü; toplam süre (söylentiden sona) medyanı 10–30 gün; evrelerin medyanı hedef aralıkta (söylenti 1–2, hücum 3–6, zirve 4–10, tükeniş 2–6, hayalet 3–6) | toplam 21 gün (17–25; n = 2325); evreler: söylenti 1, hücum 4, zirve 7, tükeniş 4, hayalet 5; dünyada 6,48 (6,31–6,75) / 100 gün, dünya başına en az 120 tamamlanan; türler: maden 661, verimli vadi 459, yol konağı 425, kutsal kalıntı 387, antik harabe 345, ordu pazarı 217; sonuç: hayalet (terk) 2298, söylentide söndü 128, kalıcı köy 27, yıkıldı 23; zirve nüfusu medyanı 23 | ✓ |
| 6m | Tepki inşaatı | sur yalnız tehditten (baskın, kuşatma, yağma, akın) sonra ya da savaşta sınırda kurulur (kural); tehditten sonra sur ortalama ≤ 20 günde başlar; büyük proje (sınır kalesi, fener kulesi) olay olarak gelir, dünya başına 60 yılda 3–60 | sur projesi 895 (tehditten sonra 251, ortalama 7,98 gün sonra; gerisi savaşta sınır boyunda); surusuz yerleşime gelen tehdit 1035; büyük proje dünya başına 19 (13–26,5; kale 244, fener 65), sabotaj 86 | ✓ |
| 6n | Büyük şehrin istikrarı ve iç krizler | istikrar (0–100) garnizon, kıtlık, vergi, meşruiyet, savaş yorgunluğu ve durumdan; düşük istikrarda iç kriz (5–10 gün belirti) ve tipin çöküş yolu; hedef yok | büyük şehir ve taht şehri istikrarı 61,1 (32,3–82,2); kriz dünyada 6,6 / 100 gün (n = 2568), belirti 8 gün (5–10), düşüşle biten %47; büyük şehirde 1741 kriz, düşüş %50 | ○ |
| H1 | Dünya donmuyor | her dünyanın her on yılında en az bir yerleşim el değiştirir (fetih, bölünme, komşuya geçiş, içeriden düşüş), bir savaş ya da iç kriz başlar ve bir fırsat merkezi kurulur; yerleşim başına durum değişimi son on yılda ilk on yılın en az %80'i (dünya medyanı); ölçüt 1 (büyük olay) geçer | donmuş on yıl 0/96; durum değişimi son / ilk on yıl %91 (%65–%130); ölçüt 1: 262 / 223 = 1,17 kat | ✓ |
| H2 | Yerleşim sayısı sabit, sahiplik ve durum dalgalı | yerleşim sayısının değişim katsayısı ≤ %10 (6b); on yılda el değiştiren (fetih, bölünme, komşuya geçiş ya da içeriden düşüş) yerleşim payı medyanı ≥ %10, hiçbir dünya-on yılında %2'nin altında değil; on yılda en az bir durum yaşayan yerleşim / ortalama yerleşim sayısı medyanı ≥ 0,9 (on yılda kurulup terk edilenler yüzünden 1'i aşabilir) | yerleşim sayısı: değişim katsayısı %2,3 (%0,7–%6,3); ortalama 77,1 yerleşim, en az ve en çok ortalamanın %91 ve %102 kadarı; on yılda el değiştiren payı %21 (%14–%31; en az %5); durum yaşayan yerleşim / ortalama 1,06 (1,01–1,23) | ✓ |
| H3 | Döngüler kuruluyor ve çöküyor | her dünyada: ≥ 5 fırsat merkezi döngüsü tamamlanır; kademe hem yükselir hem düşer (≥ 20 / ≥ 20); en az bir yerleşim harabe olur ve en az bir yerleşim kurulur ya da harabe yeniden iskân edilir; en az bir el değiştirme; örgütler dağılıp yeniden kurulur (9a) | 16/16 dünya; dünya medyanı: tamamlanan merkez 148, harabe 49, kuruluş 49,5 (harabeye yeniden iskân 31,5), doğan devlet 4, yok olan 3; örgüt: kalıcı yok olan 0/208; koşu sonunda yaşayan 200; dağılma 73, yeniden kuruluş 65; şube değişim katsayısı %20 (%9,6–%60); açılan 15461, kapanan 15072 şube (966 / 942 dünya başına) | ✓ |
| H4 | Büyük şehir: el değiştirme ve çekirdek halka | büyük şehir dünyada 100 günde 2–4 kez el değiştirir (6f; içeriden düşüş dâhil, Faz 1b-7'de onaylandı); çekirdek halka 4–6 büyük şehir: dünya-on yılı ortalamalarının medyanı 4–6, hiçbiri 3'ün altında ya da 8'in üstünde değil | dünyada 2,63 (2,22–3,38) / 100 gün; şehir başına 0,53 / 100 gün; dünyada ortalama 5,38 büyük şehir; toplam 694 el değiştirme (59 fetih, 2 bölünme, 633 içeriden: soylu isyanı 240, düello 160, darbe 130, mezhep bölünmesi 43, veraset 18, aforoz 18, ayrılık 13, boyların ayrılması 11). Ayrıştırma: dünyada 5,78 savaş / 100 gün, hedefi büyük şehir olan %15; büyük şehre 290 hücum, düşüşle bitenlerin payı %20; büyük şehir (dünya-on yılı ortalaması) medyan 5,38 (4,28–5,97; en az 3,71, en çok 6) | ✓ |
| H5 | Örgüt, devriye ve esaret (spec §9) | 9a–9f'nin hepsi geçer | 9a ✓, 9b ✓, 9c ✓, 9d ✓, 9e ✓, 9f ✓ | ✓ |

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
| Yaşayan yerleşim (günlük ortalama) | 77,1 (72,7–79,4) | 77 (73,3–79,5) |
| Yaşayan yerleşim: en az (ortalamaya oranı) | %91 (%85–%96) | %92 (%86–%98) |
| Yaşayan yerleşim: en çok (ortalamaya oranı) | %102 (%101–%108) | %102 (%101–%107) |
| Yaşayan yerleşim: değişim katsayısı | %2,3 (%0,7–%6,3) | %1,9 (%0,7–%5,7) |
| Büyük şehir (günlük ortalama) | 5,33 (4,7–5,77) | 5,38 (4,65–5,91) |
| El değiştirme (fetih + bölünme), dünyada / 100 gün | 7,58 (5,46–9,96) | 7,44 (4,91–10,6) |
| El değiştirme, yerleşim başına / 10 bin gün | 10,1 (6,94–13,1) | 9,8 (6,24–13,8) |
| Durum değişimi (kuruluş hariç), yerleşim başına / 100 gün | 1,54 (1,22–1,86) | 1,54 (1,09–1,93) |
| Durum değişimi, dünyada / 100 gün | 117 (93,7–139) | 118 (86,8–146) |
| Orta halka (Köy/Kasaba) kaydı: yerleşim başına kaç günde bir | 118 (103–130) | 126 (111–149) |
| Büyük şehir el değiştirdi, dünyada / 100 gün | 2,6 (2,1–3,27) | 2,63 (2,22–3,38) |
| Büyük şehir el değiştirdi, şehir başına / 100 gün | 0,49 (0,39–0,63) | 0,53 (0,4–0,68) |
| Büyük şehre hücum, dünyada / 100 gün | 1,17 (0,63–1,79) | 1,22 (0,53–1,63) |
| Büyük şehir yağmalandı ama tutulmadı, dünyada / 100 gün | 0,33 (0,19–0,71) | 0,31 (0,19–0,69) |

Durum değişimi türleri (yerleşim başına 10 bin günde; bölünme dışında olayın kademesi olaydan önceki):

| Tür | bütün koşu (1–60. yıl) | ısınmadan sonra (21–60. yıl) |
|---|---|---|
| Fetih (`capture`) | 5,88 (3,58–8,61) | 5,53 (2,98–8,92) |
| Bölünme (`secede`) | 0,19 (0,11–0,28) | 0,17 (0,04–0,33) |
| İçeriden düşüş (yönetim değişti) (`regime`) | 3,52 (2,16–4,53) | 3,67 (1,98–5,07) |
| Açlık başladı (`famine`) | 17,1 (1,65–32,5) | 21,1 (1,47–39,3) |
| Salgın başladı (`plague`) | 2,04 (1,41–2,38) | 2,07 (1,45–2,49) |
| Kuşatma (hücum) (`siege`) | 7,81 (4,74–9,89) | 7,77 (3,85–10,1) |
| Yakıldı / yandı (`burn`) | 27,7 (18,7–45,3) | 27,7 (18–47,2) |
| Kademe yükseldi (`tierUp`) | 40 (37,3–43,7) | 37,7 (32–42,8) |
| Kademe düştü (`tierDown`) | 41,4 (37,7–45,5) | 38,4 (33,6–44,1) |
| Terk (harabe) (`abandon`) | 2,66 (0,81–4,94) | 2,96 (0,79–4,39) |
| Harabeye yeniden yerleşim (`resettle`) | 1,68 (0,68–3,07) | 2,13 (0,67–3,46) |
| Kuruluş (durum değişimi sayılmaz) (`found`) | 0,95 (0,26–1,47) | 0,73 (0,16–1,24) |

Süreler (gün; bütün dünyalar havuzlanmış; savaş: ilandan ilişkiden düştüğü güne, koşu sonunda süren savaşlar hariç; kuşatma: hedefin önündeki ilk karargâhtan hücuma, karargâhsız hücum 1 gün; uyarı: büyük şehrin düşüşünden geriye):

| Süre | Pencere | n | p10 | medyan | p90 | ortalama | en çok |
|---|---|---|---|---|---|---|---|
| Savaş (hepsi) | bütün koşu (1–60. yıl) | 2216 | 10 | 13 | 38 | 19,2 | 78 |
| Savaş: sıradan | bütün koşu (1–60. yıl) | 1181 | 10 | 13 | 38 | 19,1 | 78 |
| Savaş: haraç | bütün koşu (1–60. yıl) | 12 | 15 | 42,5 | 73,3 | 41,3 | 78 |
| Savaş: tarihî hak | bütün koşu (1–60. yıl) | 346 | 10 | 13 | 32,5 | 16,8 | 78 |
| Savaş: fırsat | bütün koşu (1–60. yıl) | 189 | 10 | 18 | 55 | 27 | 78 |
| Savaş: Kutsal Sefer | bütün koşu (1–60. yıl) | 188 | 10 | 13,5 | 50 | 22,9 | 78 |
| Savaş: savunma paktı | bütün koşu (1–60. yıl) | 135 | 8 | 10 | 17,6 | 12,7 | 45 |
| Savaş: müttefik çağrısı | bütün koşu (1–60. yıl) | 165 | 10 | 12 | 33 | 15,2 | 65 |
| Kuşatma: büyük şehir | bütün koşu (1–60. yıl) | 454 | 6 | 6 | 6 | 6,02 | 8 |
| Kuşatma: diğer yerleşimler | bütün koşu (1–60. yıl) | 1826 | 3 | 3 | 6 | 3,66 | 8 |
| Uyarı: kuşatmanın başı → büyük şehrin düşüşü | bütün koşu (1–60. yıl) | 111 | 5 | 5 | 5 | 5,03 | 7 |
| Uyarı: savaşın başı → büyük şehrin düşüşü | bütün koşu (1–60. yıl) | 111 | 11 | 15 | 37 | 21,4 | 75 |
| Savaş (hepsi) | ısınmadan sonra (21–60. yıl) | 1423 | 10 | 13 | 38 | 19,2 | 78 |
| Savaş: sıradan | ısınmadan sonra (21–60. yıl) | 781 | 10 | 13 | 38 | 19,3 | 78 |
| Savaş: haraç | ısınmadan sonra (21–60. yıl) | 8 | 13,5 | 33,5 | 75,9 | 39,4 | 78 |
| Savaş: tarihî hak | ısınmadan sonra (21–60. yıl) | 204 | 10 | 13 | 29,4 | 16,1 | 78 |
| Savaş: fırsat | ısınmadan sonra (21–60. yıl) | 127 | 10 | 17 | 55 | 26,2 | 78 |
| Savaş: Kutsal Sefer | ısınmadan sonra (21–60. yıl) | 116 | 10 | 15 | 50 | 23,4 | 77 |
| Savaş: savunma paktı | ısınmadan sonra (21–60. yıl) | 79 | 9,6 | 10 | 17 | 12,4 | 45 |
| Savaş: müttefik çağrısı | ısınmadan sonra (21–60. yıl) | 108 | 10 | 12 | 32 | 14,9 | 48 |
| Kuşatma: büyük şehir | ısınmadan sonra (21–60. yıl) | 290 | 6 | 6 | 6 | 6,02 | 8 |
| Kuşatma: diğer yerleşimler | ısınmadan sonra (21–60. yıl) | 1174 | 3 | 3 | 6 | 3,59 | 8 |
| Uyarı: kuşatmanın başı → büyük şehrin düşüşü | ısınmadan sonra (21–60. yıl) | 59 | 5 | 5 | 5 | 5,05 | 7 |
| Uyarı: savaşın başı → büyük şehrin düşüşü | ısınmadan sonra (21–60. yıl) | 59 | 11 | 14 | 37 | 19,4 | 66 |

Koşu sonunda süren savaş: 18 (sürelere girmedi).

Kuşatma sonuçları (bütün koşu): büyük şehir: 454 hücum: 111 el değiştirdi, 160 yağmalandı (tutulmadı), 183 püskürtüldü; diğer: 1826 hücum: 1498 el değiştirdi, 241 yağmalandı (tutulmadı), 87 püskürtüldü.

Büyük şehrin el değiştirmesi (bütün koşu, 989; ilk 60):

| Seed | Gün (yıl) | Şehir | Nasıl | Önceki → yeni sahip | Kuşatma → düşüş | Savaş → düşüş |
|---|---|---|---|---|---|---|
| 1 | 15 (1) | Alacayayla | içeriden: soylu isyanı | Alacayayla Krallığı → Alacayayla Krallığı | kriz 7 gün | – |
| 1 | 32 (1) | Kıvılcımlı | içeriden: darbe | Kızılboynuz Krallığı → Kızılboynuz Krallığı | kriz 10 gün | – |
| 1 | 42 (2) | Alacayayla | içeriden: soylu isyanı | Alacayayla Krallığı → Alacayayla Krallığı | kriz 9 gün | – |
| 1 | 56 (2) | Közkapı | fetih | Kızılboynuz Krallığı → Yeminköprü Krallığı | 5 gün | 13 gün |
| 1 | 77 (2) | Alacayayla | içeriden: soylu isyanı | Alacayayla Krallığı → Alacayayla Krallığı | kriz 8 gün | – |
| 1 | 116 (3) | Közsaray | içeriden: ayrılık | Kızılboynuz Krallığı → Yeminköprü Krallığı | kriz 6 gün | – |
| 1 | 116 (3) | Alacayayla | içeriden: soylu isyanı | Alacayayla Krallığı → Alacayayla Krallığı | kriz 9 gün | – |
| 1 | 208 (6) | Közsaray | içeriden: ayrılık | Kızılboynuz Krallığı → Yeminköprü Krallığı | kriz 10 gün | – |
| 1 | 236 (6) | Kutsalörs | içeriden: soylu isyanı | Granitsunak Cumhuriyeti → Granitsunak Cumhuriyeti | kriz 7 gün | – |
| 1 | 301 (8) | Taşkandil | içeriden: soylu isyanı | Granitsunak Cumhuriyeti → Granitsunak Cumhuriyeti | kriz 6 gün | – |
| 1 | 308 (8) | Alacayayla | içeriden: soylu isyanı | Alacayayla Krallığı → Alacayayla Krallığı | kriz 10 gün | – |
| 1 | 319 (8) | Alacayayla | içeriden: veraset | Alacayayla Krallığı → Demirkanat Cumhuriyeti | kriz 6 gün | – |
| 1 | 369 (10) | Alacayayla | içeriden: soylu isyanı | Demirkanat Cumhuriyeti → Demirkanat Cumhuriyeti | kriz 10 gün | – |
| 1 | 402 (11) | Alacayayla | fetih | Demirkanat Cumhuriyeti → Yeminköprü Krallığı | 5 gün | 54 gün |
| 1 | 405 (11) | Kıvılcımlı | içeriden: darbe | Kızılboynuz Krallığı → Kızılboynuz Krallığı | kriz 9 gün | – |
| 1 | 494 (13) | Karlıtepe | fetih | Granitsunak Cumhuriyeti → Kızılboynuz Krallığı | 5 gün | 21 gün |
| 1 | 535 (14) | Karlıtepe | fetih | Kızılboynuz Krallığı → Demirçan Cumhuriyeti | 5 gün | 10 gün |
| 1 | 544 (14) | Kutsalörs | içeriden: soylu isyanı | Demirçan Cumhuriyeti → Yeminköprü Krallığı | kriz 8 gün | – |
| 1 | 554 (14) | Karlıtepe | içeriden: soylu isyanı | Demirçan Cumhuriyeti → Demirçan Cumhuriyeti | kriz 8 gün | – |
| 1 | 570 (15) | Alacayayla | içeriden: aforoz | Yeminköprü Krallığı → Yeminköprü Krallığı | kriz 0 gün | – |
| 1 | 589 (15) | Karlıtepe | içeriden: soylu isyanı | Demirçan Cumhuriyeti → Demirçan Cumhuriyeti | kriz 6 gün | – |
| 1 | 626 (16) | Tuzyayla | içeriden: soylu isyanı | Kızılboynuz Krallığı → Kızılboynuz Krallığı | kriz 9 gün | – |
| 1 | 632 (16) | Taşkandil | içeriden: soylu isyanı | Granitsunak Cumhuriyeti → Granitsunak Cumhuriyeti | kriz 10 gün | – |
| 1 | 635 (16) | Alacayayla | içeriden: mezhep bölünmesi | Yeminköprü Krallığı → Yeminköprü Krallığı | kriz 9 gün | – |
| 1 | 703 (18) | Karlıtepe | içeriden: soylu isyanı | Demirçan Cumhuriyeti → Demirçan Cumhuriyeti | kriz 10 gün | – |
| 1 | 719 (18) | Tuzyayla | içeriden: darbe | Kızılboynuz Krallığı → Kızılboynuz Krallığı | kriz 6 gün | – |
| 1 | 758 (19) | Karlıtepe | içeriden: soylu isyanı | Demirçan Cumhuriyeti → Demirçan Cumhuriyeti | kriz 5 gün | – |
| 1 | 778 (20) | Karlıtepe | fetih | Demirçan Cumhuriyeti → Granitsunak Cumhuriyeti | 5 gün | 13 gün |
| 1 | 793 (20) | Tuzyayla | içeriden: darbe | Kızılboynuz Krallığı → Kızılboynuz Krallığı | kriz 8 gün | – |
| 1 | 796 (20) | Bozkale | içeriden: soylu isyanı | Demirkanat Cumhuriyeti → Demirkanat Cumhuriyeti | kriz 6 gün | – |
| 1 | 872 (22) | Taşkandil | içeriden: soylu isyanı | Granitsunak Cumhuriyeti → Taşkandil Krallığı | kriz 7 gün | – |
| 1 | 929 (24) | Karlıtepe | fetih | Granitsunak Cumhuriyeti → Kızılboynuz Krallığı | 5 gün | 11 gün |
| 1 | 939 (24) | Taşkandil | içeriden: soylu isyanı | Taşkandil Krallığı → Taşkandil Krallığı | kriz 5 gün | – |
| 1 | 954 (24) | Tuzyayla | içeriden: soylu isyanı | Kızılboynuz Krallığı → Yeminköprü Krallığı | kriz 6 gün | – |
| 1 | 961 (25) | Karlıtepe | içeriden: soylu isyanı | Kızılboynuz Krallığı → Kızılboynuz Krallığı | kriz 8 gün | – |
| 1 | 993 (25) | Karlıtepe | içeriden: soylu isyanı | Kızılboynuz Krallığı → Kızılboynuz Krallığı | kriz 9 gün | – |
| 1 | 999 (25) | Tuzyayla | fetih | Yeminköprü Krallığı → Demirkanat Cumhuriyeti | 5 gün | 26 gün |
| 1 | 1006 (26) | Tuzyayla | içeriden: veraset | Demirkanat Cumhuriyeti → Demirkanat Cumhuriyeti | kriz 7 gün | – |
| 1 | 1035 (26) | Taşkandil | fetih | Taşkandil Krallığı → Granitsunak Cumhuriyeti | 5 gün | 25 gün |
| 1 | 1065 (27) | Karlıtepe | içeriden: soylu isyanı | Kızılboynuz Krallığı → Kızılboynuz Krallığı | kriz 8 gün | – |
| 1 | 1067 (27) | Alacayayla | içeriden: mezhep bölünmesi | Yeminköprü Krallığı → Yeminköprü Krallığı | kriz 5 gün | – |
| 1 | 1111 (28) | Tuzyayla | içeriden: soylu isyanı | Demirkanat Cumhuriyeti → Demirkanat Cumhuriyeti | kriz 8 gün | – |
| 1 | 1227 (31) | Yeni Gökköy | içeriden: soylu isyanı | Taşkandil Krallığı → Taşkandil Krallığı | kriz 7 gün | – |
| 1 | 1229 (31) | Karlıtepe | içeriden: soylu isyanı | Kızılboynuz Krallığı → Kızılboynuz Krallığı | kriz 5 gün | – |
| 1 | 1251 (32) | Alacayayla | içeriden: mezhep bölünmesi | Yeminköprü Krallığı → Yeminköprü Krallığı | kriz 9 gün | – |
| 1 | 1308 (33) | Karlıtepe | içeriden: soylu isyanı | Kızılboynuz Krallığı → Kızılboynuz Krallığı | kriz 9 gün | – |
| 1 | 1331 (34) | Tuzyayla | içeriden: soylu isyanı | Demirkanat Cumhuriyeti → Demirkanat Cumhuriyeti | kriz 8 gün | – |
| 1 | 1350 (34) | Karlıtepe | içeriden: aforoz | Kızılboynuz Krallığı → Kızılboynuz Krallığı | kriz 0 gün | – |
| 1 | 1381 (35) | Karlıtepe | içeriden: veraset | Kızılboynuz Krallığı → Demirçan Cumhuriyeti | kriz 5 gün | – |
| 1 | 1434 (36) | Yeni Gökköy | içeriden: soylu isyanı | Taşkandil Krallığı → Taşkandil Krallığı | kriz 5 gün | – |
| 1 | 1482 (38) | Karlıtepe | içeriden: soylu isyanı | Demirçan Cumhuriyeti → Demirçan Cumhuriyeti | kriz 8 gün | – |
| 1 | 1489 (38) | Yeni Gökköy | içeriden: soylu isyanı | Taşkandil Krallığı → Taşkandil Krallığı | kriz 7 gün | – |
| 1 | 1527 (39) | Yeni Gökköy | içeriden: soylu isyanı | Taşkandil Krallığı → Taşkandil Krallığı | kriz 9 gün | – |
| 1 | 1561 (40) | Yeni Gökköy | içeriden: soylu isyanı | Taşkandil Krallığı → Taşkandil Krallığı | kriz 9 gün | – |
| 1 | 1572 (40) | Yeni Gökköy | fetih | Taşkandil Krallığı → Granitsunak Cumhuriyeti | 5 gün | 14 gün |
| 1 | 1642 (42) | Karlıtepe | içeriden: soylu isyanı | Demirçan Cumhuriyeti → Demirçan Cumhuriyeti | kriz 6 gün | – |
| 1 | 1643 (42) | Alacayayla | içeriden: mezhep bölünmesi | Yeminköprü Krallığı → Yeminköprü Krallığı | kriz 7 gün | – |
| 1 | 1650 (42) | Tuzyayla | içeriden: soylu isyanı | Demirkanat Cumhuriyeti → Demirkanat Cumhuriyeti | kriz 10 gün | – |
| 1 | 1719 (43) | Alacayayla | içeriden: mezhep bölünmesi | Yeminköprü Krallığı → Yeminköprü Krallığı | kriz 8 gün | – |
| 1 | 1742 (44) | Tuzyayla | içeriden: soylu isyanı | Demirkanat Cumhuriyeti → Demirkanat Cumhuriyeti | kriz 8 gün | – |

## v3: süre tablosu (Faz 1b-5)

Yol haritası v3'ün hedef süreleri (oyun günü; 1 yıl = 40 gün, 1 ay = 10, 1 hafta = 5). Bütün dünyalar havuzlanmış; 6e–6i ısınmadan sonra, 8a–8i bütün koşu. Ajan hızları `Core/Time.cs` (`Pace`).

| Süreç | Hedef | Ölçülen | Sonuç |
|---|---|---|---|
| Orta halkada kademe kayması (6e) | yerleşim başına 60–150 günde bir | yerleşim başına 126 (111–149) günde bir | ✓ |
| Büyük şehir el değiştirmesi (6f) | dünyada 100 günde 2–4 | dünyada 2,63 (2,22–3,38) / 100 gün; şehir başına 0,53 / 100 gün; dünyada ortalama 5,38 büyük şehir; toplam 694 el değiştirme (59 fetih, 2 bölünme, 633 içeriden: soylu isyanı 240, düello 160, darbe 130, mezhep bölünmesi 43, veraset 18, aforoz 18, ayrılık 13, boyların ayrılması 11). Ayrıştırma: dünyada 5,78 savaş / 100 gün, hedefi büyük şehir olan %15; büyük şehre 290 hücum, düşüşle bitenlerin payı %20 | ✓ |
| ↳ uyarı (6g) | 5–10 gün önceden | 7 gün (5–10; n = 692): kuşatmanın başından 5 (n = 59), iç krizin başından 8 (n = 633); savaşın başından 14 gün (11–37) | ✓ |
| Savaş (6h) | 10–40 gün | 13 gün (10–38; n = 1423) | ✓ |
| ↳ kuşatma (6i) | 2–6 gün | büyük şehir 6 gün (6–6; n = 290); diğer yerleşimler 3 gün (3–6; n = 1174) | ✓ |
| Salgın (8a) | 5–10 gün | 5 gün (4–7; n = 573) | ✓ |
| Tepki inşaatı: yanan ev (8b) | 1–3 gün | 3 gün (2–5; n = 17141) | ✓ |
| Tepki inşaatı: sur (8c) | 5–10 gün | 7 gün (5–10; n = 874); palisat 7, taş sur 8 gün | ✓ |
| Büyük proje: kale, kule (8d) | 15–30 gün | 25 gün (23–30; n = 307); kale 25 (n = 242), kule 23 gün (n = 65) | ✓ |
| Temizlenen kamp → yeni köy (8e) | 10–20 gün | 13 gün (9–19; n = 269); temizlenen 5148 kampın 269 tanesine (%5,2) köy kuruldu, 20 gün içinde %4,9 | ✓ |
| Han kurulumu (8f) | 5–10 gün | 8 gün (6–10,8; n = 13); harabeyi yeniden kurma 3 gün (n = 14) | ✓ |
| Kahraman doğumu (8g) | han başına 10–20 günde bir | 15,3 günde bir (11006 doğum / 168857 han-günü); aynı handa iki doğum arası medyan 12 gün (2–34); bilgi: taverna başına 5398 günde bir (201 doğum) | ✓ |
| Efsaneye yükseliş (8h) | 100–300 gün | 236 gün (86,2–891; n = 42) | ✓ |
| İlan ömrü (8i) | 10–20 gün (başarısız sefer ödülü +%25) | 15 gün (1–15; n = 4214); bütün ilanlar 5 gün (n = 15800): biten %39 (asılıştan 4 günde), süresi dolan %24; başarısız sefer ödülü %25 artırır (Heroes.QuestFailed) | ✓ |
| Yoldaş maaşı (8j) | haftalık (5 gün) | kural: medeniyetin kahramanları her 5. gün (Economy.PayHeroes), han personeli haftada bir (InnLife.PayWages); maaşı 3 hafta ödenmeyen kahraman ayrılır | ○ |

Proje süreleri, yapı türüne göre (gün; bütün koşu):

| Yapı | n | p10 | medyan | p90 |
|---|---|---|---|---|
| castle | 242 | 25 | 25 | 30 |
| extract | 29612 | 1 | 1 | 1 |
| guild | 97 | 1 | 1 | 1 |
| house | 16647 | 1 | 1 | 1 |
| hut | 90 | 1 | 1 | 1 |
| library | 372 | 1 | 1 | 1 |
| lighthouse | 65 | 23 | 23 | 27 |
| market | 559 | 1 | 1 | 1 |
| mint | 488 | 1 | 1 | 1 |
| palisade | 732 | 5 | 7 | 10 |
| ship | 1984 | 1 | 1 | 2 |
| shipyard | 240 | 1 | 1 | 3 |
| stonehouse | 518 | 1 | 1 | 1 |
| stonewall | 142 | 7 | 8 | 9 |
| tavern | 232 | 1 | 1 | 1 |
| temple | 760 | 1 | 2 | 2 |
| unique | 391 | 1 | 1 | 2 |
| upgrade | 2474 | 1 | 1 | 2 |
| workshop | 3609 | 1 | 1 | 2 |

## Devlet, inanç ve örgüt (Faz 1b-6)

claude/devlet-orgut-spec.md: 4–6 devlet (dört hükümet tipi her dünyada), yerleşimlerin ırk ve inanç dağılımı, 13 örgüt (merkezleri çekirdek şehirlerde). Dünya tarih öncesiyle (1600 gün) olgun başlar; örgütler tarih öncesinin sonunda kurulur, ölçüm o günden başlar. Örgüt kararları ~10 günde bir.

Devletler: dünya başına 6 (ilk yıl, medyan); devlet-yılı tipe göre: Krallık 2544, Boylar 1392, Cumhuriyet 1245, Teokrasi 1041. Yönetici değişimi 1122 (veraset 30, veraset krizi 9, düello 1, boy meclisi 3, konsey oyu 59, tarikat 34); seçim 302; reise meydan okuma 635. Pakt'ın sızdığı yönetici 93, ortaya çıkan 67; Pakt suikastı 13; Tarikat'ın Kutsal Sefer çağrısı 469, başlayan sefer 90. Lobiyle yasa değişikliği 962 (kölelik yasası 0), darbe girişimi 32.

Örgütler (koşu sonu; dünyalar arası medyan, toplamlar bütün dünyalar):

| Örgüt | yaşıyor (dünya) | şube | üye | gizli şube payı | dağılma / yeniden kuruluş | açılan / kapanan şube | gölge savaşı | üye kahraman işi |
|---|---|---|---|---|---|---|---|---|
| Güneş Kilisesi | 16/16 | 56,5 | 444 | %0 | 0 / 0 | 2420 / 1539 | 726 | 1533 |
| Güneş Tarikatı | 16/16 | 12,5 | 112 | %0 | 0 / 0 | 1940 / 1745 | 737 | 295 |
| Kara Pakt | 8/16 | 0,5 | 3 | %100 | 8 / 61 | 1035 / 1000 | 761 | 18 |
| Druid Çemberi | 16/16 | 39,5 | 120 | %0 | 0 / 0 | 1321 / 634 | 0 | 1182 |
| Avcılar Locası | 16/16 | 22 | 69 | %0 | 0 / 0 | 856 / 454 | 0 | 0 |
| Hırsızlar Loncası | 16/16 | 7 | 45,5 | %67 | 0 / 0 | 1393 / 1275 | 1178 | 278 |
| Büyücü Akademisi | 16/16 | 13 | 84,5 | %0 | 0 / 0 | 1800 / 1610 | 0 | 1862 |
| Paralı Bölükler | 16/16 | 13 | 79 | %0 | 0 / 0 | 2063 / 1856 | 0 | 1827 |
| Ozanlar Koleji | 16/16 | 46 | 151 | %0 | 0 / 0 | 1428 / 725 | 0 | 0 |
| Tüccarlar Loncası | 16/16 | 57,5 | 222 | %0 | 0 / 0 | 2493 / 1609 | 1166 | 0 |
| Harabe Kâşifleri | 16/16 | 32,5 | 184 | %0 | 0 / 0 | 1563 / 990 | 0 | 596 |
| Köle Avcıları | 16/16 | 16 | 51 | %0 | 0 / 4 | 1287 / 1007 | 973 | 0 |
| Özgürlük Ağı | 16/16 | 9,5 | 30 | %4,5 | 0 / 0 | 826 / 628 | 1013 | 0 |

Örgüt operasyonları (bütün dünyalar): artifact 3840, news 3840, vigil 3629, pilgrims 3521, caravan 3470, rite 3439, track 2626, hire 2526, smuggle 2031, ruin 1929, map 1911, rob 1809, escort 1314, soul 1267, bounty 1214, whisper 1086, sabotage 401, famineOrder 370, heal 272, patrol 191, infiltrate 93, unmask 67, assassinate 13.

Devriye (hükümet tipine göre; bütün dünyalar):

| Tip | maruz ajan-günü | 100 kervan-günde durdurma | durdurmada el koyma | rüşvet | haraç/vergi | düello | kahraman tutuklama | el konan değer | vergi/haraç altını |
|---|---|---|---|---|---|---|---|---|---|
| Krallık | 303810 | 7,3 | %5 | %4,4 | %42 | 0 | 2 | 4627 | 5364 |
| Boylar | 115143 | 4,9 | %3,2 | %7,3 | %57 | 2015 | 0 | 311 | 2286 |
| Cumhuriyet | 132285 | 6,24 | %2,1 | %11 | %73 | 0 | 0 | 211 | 989 |
| Teokrasi | 164954 | 9,45 | %7,5 | %4,8 | %29 | 0 | 720 | 3779 | 1870 |

Çöküş nedenleri (hükümet tipine göre; bütün dünyalar; 100 devlet-yılı başına):

| Tip | devlet-yılı | başkent fethi | bölünme | veraset savaşı | soylu isyanı | reisin düelloda ölümü | boyların ayrılması | darbe | paralı askerler (iflas) | mezhep bölünmesi | Pakt bağı ve aforoz |
|---|---|---|---|---|---|---|---|---|---|---|---|
| Krallık | 2544 | 51 (2) | 19 (0,75) | 42 (1,65) | 482 (18,9) | 0 (0) | 0 (0) | 1 (0,04) | 0 (0) | 0 (0) | 22 (0,86) |
| Boylar | 1392 | 42 (3,02) | 12 (0,86) | 0 (0) | 0 (0) | 327 (23,5) | 25 (1,8) | 0 (0) | 0 (0) | 0 (0) | 0 (0) |
| Cumhuriyet | 1245 | 36 (2,89) | 33 (2,65) | 0 (0) | 0 (0) | 0 (0) | 0 (0) | 217 (17,4) | 0 (0) | 0 (0) | 0 (0) |
| Teokrasi | 1041 | 20 (1,92) | 4 (0,38) | 0 (0) | 1 (0,1) | 0 (0) | 0 (0) | 0 (0) | 0 (0) | 89 (8,55) | 11 (1,06) |

## Dünyanın durumu (Faz 1b-7)

Yol haritası v3: yerleşim durum tablosu (her yerleşimde 5–15 günde bir zar), büyük şehrin istikrarı ve tipe göre iç çöküş yolları, fırsat merkezi döngüsü, tepki inşaatı. Bütün dünyalar havuzlanmış, bütün koşu.

Durumlar (yerleşim başına 1000 günde kaç kez; süre: gün, medyan (p10–p90)):

| Durum | 1000 yerleşim-gününde | süre | büyük şehirde payı |
|---|---|---|---|
| Refah (`prosper`) | 7,52 | 11 (6–15) | %7 |
| Kaynak bulundu (`found`) | 4,44 | 8 (5–12) | %5 |
| Ticaret patlaması (`boom`) | 4,4 | 7 (4–10) | %9,2 |
| Göç dalgası (`migration`) | 4,37 | 7 (4–10) | %4,8 |
| Festival (`festival`) | 3,9 | 4 (3–5) | %6 |
| Kaynak tükendi (`depleted`) | 3,82 | 10 (6–14) | %6,3 |
| Kıtlık (hasat) (`shortage`) | 2,09 | 9 (5–14) | %6,8 |
| Kıtlık (açlık) (`hunger`) | 1,7 | 11 (1–65,3) | %13 |
| Kuşatma (`siege`) | 0,79 | 1 (1–5) | %20 |
| İşgal (`occupation`) | 0,55 | 11 (8–14) | %22 |
| Yeni lord (`newlord`) | 0,39 | 7 (5–10) | %67 |
| Salgın (`plague`) | 0,19 | 5 (4–7) | %49 |
| Canavar tehdidi (`monsters`) | 0,14 | 8 (4–12) | %11 |

Göç: göçmen kafilesi 105189, kaçan 145849, gelen 254407 kişi; basamak kaymasına yetecek göç denemesi: yukarı 6946, aşağı 2057.

İç krizler (hükümet tipine göre; düşüş: krizin sonunda yerleşim içeriden düştü):

| Tip | Kriz | n | büyük şehirde | düşüş payı | belirti (gün, medyan) |
|---|---|---|---|---|---|
| Boylar | Meydan okuma | 674 | 383 | %49 | 8 |
| Boylar | Ayrılık | 45 | 45 | %59 | 7,5 |
| Krallık | Soylu isyanı | 1026 | 746 | %47 | 8 |
| Krallık | Veraset kavgası | 71 | 46 | %61 | 8 |
| Cumhuriyet | Darbe söylentisi | 476 | 311 | %46 | 8 |
| Cumhuriyet | Ayrılık | 45 | 45 | %49 | 7 |
| Teokrasi | Mezhep çatışması | 239 | 172 | %37 | 8 |

İstikrar (büyük şehir ve taht şehri, 5 günde bir örnek, n = 57351): p10 32,3, p25 47,8, medyan 61,1, p75 72,7, p90 82,2; 40'ın altında %16.
İçeriden düşüş: 1239 (büyük şehirde 871); yollar: veraset 42, soylu isyanı 483, düello 327, boyların ayrılması 25, darbe 218, mezhep bölünmesi 89, aforoz 33, ayrılık 22. Tip değişimi 90 (republic → kingdom 30, kingdom → republic 42, theocracy → kingdom 7, kingdom → theocracy 11); birleşme (evlilik ittifakı) 2; komşuya geçen şehir 207; Kutsal Sefer yenilgisi 7.

Fırsat merkezleri (türe göre; süre: söylentiden sona, gün):

| Tür | n | süre | zirve nüfusu | sonuç | altın (toplam) |
|---|---|---|---|---|---|
| maden | 661 | 22 (18–25) | 23 | hayalet (terk) 628, söylentide söndü 22, yıkıldı 4 | 6154 |
| verimli vadi | 459 | 21 (17–25) | 17 | hayalet (terk) 378, kalıcı köy 27, söylentide söndü 47, yıkıldı 4 | 0 |
| yol konağı | 425 | 22 (18–26) | 23 | hayalet (terk) 395, söylentide söndü 24, yıkıldı 3 | 2083 |
| kutsal kalıntı | 387 | 21 (18–25) | 24 | hayalet (terk) 379, söylentide söndü 1, yıkıldı 5 | 1439 |
| antik harabe | 345 | 21 (17,8–25) | 22 | hayalet (terk) 316, söylentide söndü 24, yıkıldı 3 | 1239 |
| ordu pazarı | 217 | 19 (12–23) | 21 | hayalet (terk) 202, söylentide söndü 10, yıkıldı 4 | 912 |

Aynı anda yaşayan merkez (dünya başına, günlük ortalama): 1,21 (1,11–1,31). Zirvede gelen haydut kampı 848, hayalet kasabaya yerleşen goblin 895, kalıcı köy olan vadi 27.

## Başsız ölçütler: on yıllar (Faz 1b-8)

Yol haritası Faz 1b/8 (H1–H5, ölçüt tablosunda). Dünya-on yılı pencereleri (400 gün; tarih öncesinden sonra); hücre: dünya medyanı (p10–p90). El değiştiren: fetih, bölünme, komşuya geçiş ya da içeriden düşüş yaşayan yerleşim payı; durum değişimi: kuruluş dışındaki bütün olaylar (kademe, yakılma, kıtlık, kuşatma, salgın, el değiştirme, terk, yeniden iskân), yerleşim başına.

| Ölçü | 1–10. yıl | 11–20. yıl | 21–30. yıl | 31–40. yıl | 41–50. yıl | 51–60. yıl |
|---|---|---|---|---|---|---|
| Kalıcı yerleşim | 77,8 (72,2–79,4) | 78 (70,1–80) | 77,8 (69,4–79,7) | 77,7 (73,7–79,5) | 76,8 (71,9–79,5) | 78,1 (71,8–79,4) |
| El değiştiren payı | %22 (%18–%29) | %20 (%14–%30) | %23 (%14–%29) | %22 (%13–%33) | %22 (%15–%28) | %21 (%11–%29) |
| Durum yaşayan / yerleşim | 1,05 (1,02–1,23) | 1,03 (1,02–1,23) | 1,08 (1,02–1,25) | 1,07 (1,02–1,15) | 1,06 (1,01–1,25) | 1,06 (1,02–1,16) |
| Durum değişimi / yerleşim | 6,39 (5,35–7,83) | 6,58 (5,04–7,32) | 6,17 (4,45–8,26) | 6,09 (4,3–7,58) | 5,73 (4,59–7,75) | 5,55 (4,43–8,51) |
| Savaş + iç kriz | 56,5 (46,5–61,5) | 49 (40–61,5) | 51 (29–62,5) | 47,5 (33,5–71,5) | 46,5 (30,5–68,5) | 49 (30–72) |
| Savaş | 27 (20,5–31,5) | 22,5 (14,5–30,5) | 23,5 (8–30) | 23 (10,5–38) | 20,5 (11–36) | 22,5 (11–36,5) |
| Fırsat savaşı | 1,5 (0–4) | 1 (0–4) | 1 (0–3) | 2 (0–6,5) | 1,5 (0–4,5) | 2 (0–4,5) |
| Fırsat merkezi | 26,5 (24–29) | 25,5 (23,5–28,5) | 26 (24–29,5) | 26,5 (23,5–28) | 26,5 (23,5–28) | 24,5 (22,5–27,5) |
| Büyük şehir el değiştirmesi | 9 (6,5–13,5) | 9,5 (4,5–14,5) | 10,5 (4–13,5) | 11,5 (6,5–16) | 11,5 (7–17) | 10,5 (5–18) |

## Eski analizdeki sorunlar

Eski analiz: TS v0.23, 12 seed × 30 yıl ve 3 seed × 60 yıl (Proje: `analiz-5-ajan-oneriler.md`). "Sürüyor mu" kaba bir eşiktir: araştırma ağacı Faz 1b-3'te kaldırıldı; 30. yılda tam 5 kara yerleşimli medeniyet ≥ %50; kamp (30. yıl) < 0,75 × en yüksek yıl; altın (30. yıl) ≥ 10 × altın (1. yıl); boştaki iş gücü (30. yıl) ≥ %30; büyük olay (30. yıl) ≤ 0,6 × en yüksek yıl; 25. yıldan sonra doğanların ≥ %50'si Sv5+; hiç başkent kaybı yok.

| Bulgu | Eski analiz | Bu ölçüm | Sürüyor mu? |
|---|---|---|---|
| Araştırma ağacı erken bitiyor | ~19. yılda bitiyor; 30. yılda medeniyetlerin %98'i bitirmiş | ağaç ve çağlar kaldırıldı (Faz 1b-3); başlangıç medeniyetlerinin ilk kasabası medyan –. yılda (0/0), ilk şehri –. yılda (0/0); 30. yılda başkent kademesi ortalaması 2,67 | hayır |
| Medeniyetler 5 yerleşimde takılıyor | 98 medeniyetin 74'ü (%76) tam 5 yerleşimde | tam 5 kara yerleşimli medeniyet payı 30. yılda %0, 60. yılda %0,9 (denizaşırı koloniler dâhil 30. yılda tam 5: %1,9, 5+: %71); medeniyet başına 12,3 yerleşim (30. yıl) | hayır |
| Kamp sayısı düşüyor | 6,8'den 3,5'e iniyor | 9 (1. yıl) → en yüksek 10 (46. yıl) → 9 (30. yıl) → 9 (60. yıl); yıl sonu, yıllık dünya medyanı | hayır |
| Altın birikiyor | altın medyanı 78'den 6.503'e çıkıyor | 373 (1. yıl) → 430 (30. yıl) → 389 (60. yıl) | hayır |
| İş gücü boşta | iş gücünün %43'ü boşta | 30. yılda %22, 60. yılda %26 (işe yerleşemeyen `zanaatçı` / bütün iş gücü, askerler dâhil; yıl içi ortalama) | hayır |
| Büyük olaylar seyreliyor | yıllık büyük olay 59'dan 28'e düşüyor | en yüksek 292 (59. yıl) → 244 (30. yıl) → 270 (60. yıl), yıllık dünya medyanı | hayır |
| Doğuş seviyesi şişiyor | 24. yıldan sonra herkes Sv5 doğuyor; efsane mekaniği ölü | 25–60. yıllarda Sv5+ doğanların payı %0; on yıllık doğuş seviyesi ortalaması 1,63 · 1,63 · 1,61 · 1,6 · 1,66 · 1,64 | hayır |
| Başkent düşmüyor | başkent fethedilemiyor (agents.ts:673) | 16 dünyada 106 başkent kaybı, 48 yok olma; 1873 yerleşim fethi | hayır |

## On yıllık özet

Hücre: dünyalar arası medyan (p10–p90). Her dünyada on yılın yıllık değerlerinin ortalaması alınır: akış ölçülerinde yıllık ortalama, stok ölçülerinde yıl sonu değerlerinin ortalaması. Yüzdeler 0–1 paylardır.

| Ölçü | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| **Medeniyet** | | | | | | |
| Yaşayan medeniyet | 6,3 (5,85–6,85) | 6,2 (5,95–7) | 6,1 (5,95–7,25) | 6 (5,7–7,3) | 6,7 (6–7,05) | 7 (6–7,65) |
| Yeni medeniyet (yeniden doğan) | 0,1 (0–0,2) | 0 (0–0,1) | 0 (0–0,2) | 0 (0–0,15) | 0 (0–0,1) | 0 (0–0,15) |
| Yok olan medeniyet | 0,05 (0–0,15) | 0,05 (0–0,15) | 0 (0–0,1) | 0,1 (0–0,15) | 0 (0–0,1) | 0 (0–0,1) |
| Başkent kaybı (medeniyet yaşarken) | 0,1 (0–0,3) | 0,15 (0–0,3) | 0,05 (0–0,25) | 0,1 (0–0,1) | 0,1 (0–0,2) | 0,1 (0–0,2) |
| Çöküş (yok olma + başkent kaybı) | 0,2 (0,05–0,4) | 0,2 (0,05–0,45) | 0,1 (0–0,35) | 0,1 (0–0,25) | 0,1 (0–0,3) | 0,1 (0–0,2) |
| Yaşayan yerleşim | 79 (73,4–80,9) | 79,3 (70,6–81,4) | 79,1 (70,3–81) | 78,7 (75–80,8) | 77,9 (73–80,9) | 79,2 (72,8–80,6) |
| Medeniyet başına yerleşim | 12,3 (11,1–13,4) | 12,4 (10,8–13,6) | 11,6 (10,6–13,4) | 12,5 (11,1–13,7) | 11,7 (11–13,1) | 11,6 (9,92–13,2) |
| 5+ kara yerleşimli medeniyet payı | %78 (%66–%93) | %70 (%59–%90) | %67 (%57–%84) | %73 (%57–%87) | %72 (%58–%83) | %73 (%59–%83) |
| Kurulan yerleşim | 0,8 (0,45–2,1) | 0,5 (0,2–1,5) | 0,9 (0,2–2,25) | 0,75 (0,25–1,55) | 0,6 (0,1–1,7) | 0,55 (0,25–1,3) |
| Fethedilen yerleşim | 2,15 (1,7–2,9) | 1,85 (1,35–3) | 1,9 (0,7–2,8) | 1,7 (0,85–3,4) | 1,8 (0,85–3,15) | 1,85 (0,75–2,9) |
| Terk edilen yerleşim | 0,45 (0,15–1,95) | 0,45 (0,25–2,3) | 0,75 (0,15–2,15) | 0,65 (0,25–1,35) | 0,6 (0,1–1,95) | 0,75 (0,25–1,4) |
| Toplam nüfus | 2778 (2652–3044) | 2901 (2690–3190) | 2957 (2571–3359) | 2929 (2542–3359) | 2890 (2395–3343) | 2996 (2503–3368) |
| Altın medyanı (medeniyetler) | 357 (201–549) | 333 (146–598) | 403 (114–578) | 383 (168–645) | 354 (228–573) | 386 (134–583) |
| Boştaki iş gücü payı | %16 (%12–%25) | %17 (%12–%31) | %20 (%11–%40) | %23 (%14–%42) | %24 (%11–%39) | %24 (%16–%41) |
| Bölünme (ayrılıp kurulan medeniyet) | 0,1 (0–0,2) | 0 (0–0,1) | 0 (0–0,2) | 0 (0–0,15) | 0 (0–0,1) | 0 (0–0,15) |
| En büyük medeniyetin yerleşimi | 21,8 (18–24,1) | 23,3 (19,2–28,1) | 25 (20–30,8) | 25,7 (18,9–31,2) | 24,8 (19–32,9) | 27 (18,6–32,5) |
| **Olaylar** | | | | | | |
| Olay | 1249 (1037–1478) | 1326 (1090–1476) | 1365 (1152–1567) | 1445 (1127–1643) | 1536 (1214–1693) | 1561 (1174–1714) |
| Büyük olay | 220 (173–269) | 226 (188–269) | 229 (196–311) | 253 (196–300) | 267 (183–307) | 256 (174–330) |
| **Savaş** | | | | | | |
| Muharebe | 19,8 (12,2–23,3) | 19,4 (13,5–23) | 18,7 (14,7–25,8) | 19,6 (16,5–26,5) | 20 (14,9–24,5) | 18,9 (12–29,4) |
| Başlayan savaş | 2,7 (2,05–3,15) | 2,25 (1,45–3,05) | 2,35 (0,8–3) | 2,3 (1,05–3,8) | 2,05 (1,1–3,6) | 2,25 (1,1–3,65) |
| Süren savaş (yıl sonu) | 1,3 (1–1,65) | 1,3 (0,8–1,7) | 1,1 (0,7–1,6) | 1,1 (0,55–1,65) | 1,3 (0,55–1,8) | 1,5 (0,6–2,05) |
| Yıl içinde süren savaş | 4 (2,8–4,7) | 3,75 (2,6–4,7) | 3,55 (1,45–4,8) | 3,45 (1,85–5,05) | 3,25 (1,85–5,2) | 3,8 (1,95–5,55) |
| Yağma akını (medeniyet) | 1,15 (0–3,5) | 1,05 (0–4,05) | 1,6 (0–4,1) | 1,5 (0–4,9) | 1,7 (0–5,3) | 1,6 (0–6,2) |
| Tarihî hak savaşı | 0,5 (0,2–0,75) | 0,35 (0,1–0,8) | 0,25 (0–0,65) | 0,35 (0,05–0,85) | 0,15 (0–0,8) | 0,2 (0–0,4) |
| Pakt gereği savaş | 0,1 (0–0,55) | 0,1 (0–0,35) | 0 (0–0,6) | 0,05 (0–0,35) | 0 (0–0,25) | 0 (0–0,4) |
| Kutsal Sefer çağrısı | 0,1 (0–0,25) | 0,1 (0–0,15) | 0 (0–0,2) | 0,1 (0–0,2) | 0,1 (0–0,25) | 0,1 (0–0,25) |
| İhanet (pakt çiğnendi) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,05) | 0 (0–0,1) | 0 (0–0,1) |
| Savunma paktı (yıl sonu) | 0,4 (0–1,55) | 0,45 (0–1,1) | 0,25 (0–1,3) | 0,3 (0–0,95) | 0,25 (0–0,95) | 0,35 (0–0,95) |
| **Canavarlar** | | | | | | |
| Yaşayan kamp (yıl sonu) | 8,55 (8,1–9) | 9,15 (8–9,5) | 9 (8,1–9,85) | 9,05 (8–9,95) | 9 (8,3–10,4) | 8,7 (8,2–10,4) |
| Yaşayan kamp (yıl ort.) | 8,42 (8,04–9,22) | 9,08 (8,18–9,52) | 8,97 (8,17–9,81) | 9,01 (8,16–10) | 8,8 (8,05–10,2) | 8,89 (7,89–10,4) |
| Doğan kamp | 6 (2,85–10,1) | 6,15 (2,55–9,1) | 5,7 (2,1–9,6) | 5,45 (3,1–9,3) | 4,85 (1,95–8,15) | 3,8 (1,65–10) |
| Temizlenen kamp | 5,9 (2,7–10) | 6,15 (2,45–9) | 5,5 (2,25–9,6) | 5,6 (3,2–9,25) | 4,8 (2–8,15) | 3,85 (1,6–10) |
| Canavar baskını | 3,7 (2,4–5,3) | 4,15 (2,85–4,95) | 4,05 (2,95–5,45) | 4,65 (2,3–6,05) | 4,3 (1,75–5,45) | 4,45 (2,05–5,5) |
| Yaşayan trol ini (yıl sonu) | 2,25 (1,85–2,8) | 2,15 (1,5–2,65) | 2,55 (2–2,95) | 2,55 (2,2–3) | 2,85 (2,35–3) | 2,8 (2,25–3) |
| Yaşayan ejderha (yıl sonu) | 0 | 0,5 (0,2–0,65) | 0,15 (0–1) | 0 (0–1) | 0 (0–1) | 0 (0–1) |
| Ejderha akını | 0 | 0,65 (0,3–0,9) | 0,15 (0–1,55) | 0 (0–1,4) | 0 (0–1,55) | 0 (0–1,4) |
| Kriz (anlatıcı) | 3,05 (2,65–3,5) | 3 (2–3,4) | 2,9 (2,05–3,5) | 2,8 (2–3,65) | 2,95 (1,8–3,6) | 2,75 (1,85–3,8) |
| Rahatlama dönemi (anlatıcı) | 0,2 (0,05–0,45) | 0,3 (0,15–0,5) | 0,15 (0–0,5) | 0,3 (0,1–0,65) | 0,25 (0,05–0,55) | 0,2 (0,1–0,6) |
| **Kahramanlar** | | | | | | |
| Doğan kahraman | 10,3 (6,7–15) | 10,6 (6,85–15,2) | 10,1 (6,85–16,1) | 10,7 (8,95–17) | 12,5 (8,9–17,2) | 12,2 (8,9–17,8) |
| Ölen kahraman | 3,4 (2,05–5,05) | 4,35 (2,3–5,6) | 4,7 (3,3–7,1) | 4,35 (3,2–5,95) | 4,75 (3,55–6,55) | 4 (3,15–6,2) |
| Emekli olan kahraman | 1,55 (0,95–2,5) | 2 (1,1–2,75) | 2 (1,65–2,75) | 2,2 (1,75–2,95) | 2,45 (1,8–3,2) | 2,55 (1,45–3,3) |
| Diyarı terk eden kahraman | 3,2 (1,8–9,6) | 5,5 (1,25–9,55) | 4,6 (1,6–10,4) | 5,8 (2,4–11,4) | 6,15 (2,3–12,6) | 6,8 (4,1–10,5) |
| Ölümden dönen kahraman | 0 | 0 | 0 | 0 | 0 | 0 |
| Efsane olan kahraman | 0 (0–0,1) | 0,1 (0–0,3) | 0 (0–0,2) | 0 (0–0,2) | 0 (0–0,2) | 0 (0–0,1) |
| Yaşayan kahraman (yıl sonu) | 141 (112–170) | 148 (122–176) | 148 (127–180) | 158 (127–177) | 168 (134–178) | 169 (139–184) |
| Doğuş seviyesi (ort.) | 1,61 (1,54–1,79) | 1,67 (1,52–1,73) | 1,59 (1,51–1,76) | 1,62 (1,47–1,68) | 1,65 (1,56–1,74) | 1,65 (1,52–1,71) |
| Ölüm seviyesi (ort.) | 4,61 (4,2–5,21) | 4,69 (4,16–5,18) | 4,96 (4,39–5,54) | 4,78 (4,18–5,44) | 4,95 (3,91–5,47) | 4,87 (4,38–5,18) |
| Yaşayan kahraman seviyesi (ort.) | 4,48 (4,14–4,92) | 4,66 (4,27–5,05) | 4,56 (4,06–5,42) | 4,55 (4,24–5,17) | 4,44 (4,15–5,08) | 4,36 (3,98–5,23) |
| En yüksek seviye (şimdiye dek) | 10 | 10 | 10 | 10 | 10 | 10 |
| **Han ve ticaret** | | | | | | |
| Ayakta han | 4,15 (2,85–6) | 4,3 (2,95–6) | 4,15 (2,9–6) | 4 (3–6) | 4,55 (3,5–6,35) | 4,4 (3,15–6,5) |
| Asılan ilan | 12 (7,45–17) | 13,8 (10–17) | 16,2 (10,3–21) | 15,9 (10,7–21,7) | 16,3 (10,5–20,8) | 16,6 (10,1–22,9) |
| Biten ilan | 6,35 (3,05–9,9) | 6,05 (2,7–8,35) | 6,9 (2,9–7,9) | 7,25 (3,15–9,45) | 5,95 (2,95–9,15) | 6,7 (2,95–9,85) |
| Ticaret seferi (kervan) | 76 (60,5–105) | 82,9 (68–124) | 92,1 (72,3–130) | 92,4 (71,8–159) | 100 (69,4–128) | 104 (80,1–139) |
| İkmal seferi | 39 (25,9–47,1) | 37,5 (32–58,3) | 38,4 (29,4–70,5) | 40,2 (23,3–72,3) | 41,5 (27,9–69,8) | 42,3 (28,4–71,5) |
| **Altın ve ambar** | | | | | | |
| Altın p90 (medeniyetler) | 1146 (894–1327) | 1166 (861–1565) | 1273 (894–1780) | 1520 (974–1993) | 1359 (940–2007) | 1499 (1216–1952) |
| Bakım gideri (altın; asker, kahraman, L2–L3) | 2768 (2308–3112) | 2691 (2290–3216) | 2576 (2152–3164) | 2437 (1819–3083) | 2360 (1878–3180) | 2377 (1995–3087) |
| Kamu işlerine (imar) harcanan altın | 1866 (1199–2495) | 2102 (1189–3078) | 2103 (1275–3265) | 2869 (1492–3900) | 2397 (1785–4156) | 2825 (2180–3928) |
| Ambarla beslenen amele tayını (gıda) | 1410 (214–2438) | 851 (91,6–2328) | 437 (120–1944) | 443 (13,6–1617) | 186 (16,5–1557) | 146 (1,78–830) |
| Kamu işlerindeki (amele) iş gücü payı | %14 (%12–%15) | %14 (%10–%15) | %13 (%9,5–%16) | %12 (%6,1–%15) | %11 (%7,1–%15) | %11 (%6,8–%15) |
| İmar ortalaması (köy+, 0–100) | 18 (13,9–22,6) | 16,6 (11,5–22,9) | 15,5 (12,3–23,3) | 16,2 (10,5–23,4) | 17,7 (11,5–22) | 16,4 (13,2–21,2) |
| Canavar baskınında yitirilen altın | 8 (2,25–20,7) | 6,35 (2,15–12) | 9,2 (3,15–16,8) | 12,3 (1,4–28) | 7,65 (3,45–19,4) | 10,7 (1,4–25,7) |
| Ejderhaya giden altın (haraç + akın) | 0 | 56,4 (23,9–217) | 104 (0–617) | 0 (0–577) | 0 (0–828) | 0 (0–767) |
| Hazinesi boş medeniyet payı | %0 | %0 | %0 | %0 | %0 | %0 |
| Kent tüketiminde yokluk payı (köy+; ekmek, bira ya da alet) | %33 (%26–%42) | %33 (%27–%41) | %31 (%29–%41) | %33 (%26–%51) | %35 (%23–%43) | %32 (%23–%42) |
| Ekmek ya da bira yokluğu payı (köy+) | %6,2 (%1,9–%13) | %7,3 (%2,8–%15) | %9,2 (%4,3–%19) | %13 (%4,7–%19) | %13 (%4,3–%28) | %12 (%6,2–%24) |
| Kıtlık (büyük olay) | 0 (0–0,2) | 0 (0–0,2) | 0,05 (0–0,2) | 0,05 (0–0,25) | 0,1 (0–0,45) | 0,05 (0–0,35) |
| Açlıktan ölen | 0 (0–12) | 0 (0–18) | 1,65 (0–16,8) | 0,95 (0–15,2) | 2,2 (0–19,6) | 7,95 (0–12,6) |
| Kıtlık yardımı (sevkiyat) | 0 (0–0,35) | 0 (0–0,3) | 0 (0–0,7) | 0,05 (0–0,9) | 0,1 (0–2,2) | 0 (0–1,25) |
| Kıtlıkta yüz çeviren | 0 (0–0,5) | 0 (0–0,55) | 0,1 (0–0,55) | 0,05 (0–0,4) | 0,3 (0–1,15) | 0,15 (0–0,95) |
| Kıtlık akını | 0 (0–0,15) | 0 (0–0,55) | 0 (0–0,35) | 0 (0–0,3) | 0 (0–0,3) | 0 (0–0,7) |
| Ambarın yettiği gün (medeniyet medyanı) | 42,9 (28,1–52,8) | 40,1 (21,1–49,7) | 31,6 (22,4–45) | 27,5 (16,6–44,4) | 27,5 (13–38,2) | 22,4 (9,97–37,2) |
| **Yerleşim kademesi** | | | | | | |
| Ortalama yerleşim kademesi | 1,16 (0,96–1,28) | 1,13 (0,94–1,29) | 1 (0,82–1,31) | 1,04 (0,88–1,35) | 1,06 (0,94–1,34) | 1,07 (0,89–1,26) |
| Köy+ yerleşim | 64,3 (51,3–68,4) | 64,4 (50,1–69,8) | 59,5 (44,1–71,1) | 61,5 (49–72,3) | 62,1 (52,3–73,6) | 63,5 (48,1–72,7) |
| Kasaba+ yerleşim | 22,1 (14,7–29,4) | 19,6 (14,1–30,2) | 17,4 (9,15–28,8) | 16,5 (10,5–30,7) | 16,1 (10,3–29,7) | 16,1 (9,65–24,2) |
| Şehir | 5,3 (4,25–5,7) | 5,35 (4,3–5,85) | 5,35 (4,15–5,95) | 5,5 (4,45–6) | 5,25 (4,3–5,9) | 5,45 (4,85–5,95) |
| Ortalama başkent kademesi | 2,55 (2,3–2,68) | 2,58 (2,37–2,69) | 2,57 (2,46–2,79) | 2,61 (2,5–2,82) | 2,6 (2,41–2,7) | 2,65 (2,46–2,78) |
| Kademe değişimi (yerleşim, yıl içinde) | 30,9 (24,3–33,5) | 28,3 (22,5–31,7) | 25,7 (19,7–30,8) | 22,6 (16,4–28,6) | 23,8 (18,6–29) | 22,6 (15,5–27,3) |
| **Deniz** | | | | | | |
| Liman (tersane) | 21,3 (18,2–23,4) | 22,5 (18,9–25,3) | 22,8 (18,7–25,6) | 22,4 (18,9–26,2) | 22,4 (20,1–26,9) | 22,3 (19,4–26,6) |
| Gemi (koga/tekne) | 44,2 (39–49,4) | 46,2 (38,7–53,1) | 48,8 (41,3–53,8) | 48 (41,3–53,5) | 48,5 (41,3–56,3) | 51,5 (42,4–58,4) |
| Kadırga | 19,7 (16,5–25,9) | 22,7 (15,3–25,5) | 22,2 (17,5–25,1) | 20,1 (15,6–24,6) | 20,5 (14,1–25,5) | 20,1 (12,2–24,1) |
| Denizaşırı yerleşim | 8 (4,35–10) | 8,35 (4–10,3) | 7,5 (4–10,4) | 7,3 (3,65–10) | 7,15 (4–9,7) | 6,55 (4–10,5) |
| Deniz ticaret yolu (yıl sonu) | 14,2 (10,2–16,6) | 15,9 (11,1–19,5) | 16,9 (9,75–21,3) | 15,1 (9,5–21,1) | 14,9 (9,25–24,3) | 16,8 (9,4–25,4) |
| Deniz seferi (ticaret) | 19,4 (14,8–33,2) | 21,7 (15,9–36,8) | 25,5 (13,5–36,9) | 26,2 (14,5–33,3) | 24,7 (10,5–43,2) | 30,1 (13–45,4) |
| **v3: durum değişimi** | | | | | | |
| Yaşayan yerleşim (yıl ort.) | 77,8 (72,2–79,4) | 78 (70,1–80) | 77,8 (69,4–79,7) | 77,7 (73,7–79,5) | 76,8 (71,9–79,5) | 78,1 (71,8–79,4) |
| Büyük şehir (Şehir kademesi, yıl ort.) | 5,35 (4,25–5,68) | 5,34 (4,28–5,85) | 5,41 (4,18–5,94) | 5,64 (4,35–5,99) | 5,35 (4,33–5,96) | 5,5 (4,87–5,98) |
| El değiştiren yerleşim (fetih + bölünme) | 3,4 (2,65–3,8) | 2,85 (2,4–3,85) | 2,95 (1,5–4,1) | 2,9 (1,5–4,55) | 2,9 (1,95–4,5) | 2,65 (1,7–4,35) |
| El değiştiren büyük şehir | 0,9 (0,65–1,35) | 0,95 (0,45–1,45) | 1,05 (0,4–1,35) | 1,15 (0,65–1,6) | 1,15 (0,7–1,7) | 1,05 (0,5–1,8) |
| Büyük şehre hücum (kuşatma muharebesi) | 0,45 (0,2–0,95) | 0,5 (0,3–0,95) | 0,4 (0,1–0,7) | 0,45 (0,15–0,85) | 0,3 (0,05–0,85) | 0,55 (0,2–0,9) |
| Büyük şehir yağmalandı, tutulmadı | 0,15 (0–0,3) | 0,2 (0–0,5) | 0,1 (0–0,3) | 0,1 (0–0,3) | 0,15 (0–0,5) | 0,1 (0–0,3) |
| Yerleşim durum değişimi (kuruluş hariç hepsi) | 49,9 (40,9–58,1) | 48,2 (39,9–57,3) | 47,3 (34,9–62,6) | 44,1 (33,6–59) | 43,5 (34,4–60) | 42,2 (34,9–63,6) |
| Orta halka kaydı (Köy/Kasaba: kademe değişimi ya da terk) | 23,2 (16,9–25,2) | 21 (18–25,1) | 19,2 (13,2–24,1) | 16,3 (11,2–22,9) | 17,3 (12,4–23,9) | 17,5 (10,5–21,9) |
| Açlık başlayan yerleşim | 0,1 (0–8,3) | 0,05 (0–11,1) | 2,3 (0–15,2) | 5,5 (0,2–14) | 3,4 (0,75–14,8) | 6,4 (0,05–11,8) |
| Salgın başlayan yerleşim | 0,6 (0,4–0,85) | 0,45 (0,3–1,2) | 0,55 (0,25–1) | 0,55 (0,2–0,9) | 0,7 (0,3–0,85) | 0,55 (0,4–1,05) |
| Yakılan/yanan yerleşim | 8,1 (6–11,7) | 8,8 (5,75–14,6) | 9,25 (5,15–13,6) | 7,5 (4,85–14,4) | 9,1 (5–12,9) | 9 (5,25–15,6) |
| Harabeye yeniden yerleşim | 0,6 (0,15–1,15) | 0,35 (0,1–1,05) | 0,6 (0,15–1,8) | 0,45 (0,1–1,15) | 0,4 (0,1–1,05) | 0,55 (0,15–0,9) |
| **Devlet, inanç ve örgüt** | | | | | | |
| Yaşayan örgüt (yıl sonu) | 13 (12,2–13) | 12,9 (12,2–13) | 12,6 (12,1–13) | 12,7 (12–13) | 12,4 (12–13) | 12,5 (12,2–13) |
| Örgüt şubesi (yıl sonu) | 303 (235–358) | 298 (220–379) | 315 (224–392) | 318 (231–382) | 323 (249–397) | 338 (264–402) |
| Gizli şube (yıl sonu) | 11,5 (8,1–15,2) | 7,7 (5,65–12,2) | 7,1 (4–11,8) | 7,15 (4,65–11,8) | 6,75 (3,7–10,4) | 6,75 (4,85–9,65) |
| Örgüt üyesi (yıl sonu) | 1365 (1259–1520) | 1385 (1263–1636) | 1430 (1237–1753) | 1508 (1277–1855) | 1505 (1322–1895) | 1603 (1382–1951) |
| Açılan şube | 17,4 (16,3–19,2) | 16,2 (15–17,5) | 15,9 (14,1–17,3) | 15,8 (13,9–17,8) | 16,5 (13,9–18,2) | 15,8 (13–18) |
| Kapanan şube | 19,1 (15,5–22,2) | 15,6 (11,4–18) | 15 (12,5–17,4) | 14,1 (11,4–17,8) | 15,5 (12,9–16,5) | 15,2 (12,2–18) |
| Dağılan örgüt | 0 (0–0,2) | 0 (0–0,2) | 0,1 (0–0,2) | 0,1 (0–0,1) | 0,1 (0–0,2) | 0,1 (0–0,2) |
| Yeniden kurulan örgüt | 0 (0–0,1) | 0 (0–0,15) | 0,05 (0–0,1) | 0,1 (0–0,15) | 0,05 (0–0,25) | 0,1 (0–0,2) |
| Gölge savaşı eylemi | 7,85 (5,3–9,1) | 7 (5–8,75) | 6,8 (4,8–8,7) | 7,1 (3,55–9) | 6,25 (3,9–8,85) | 6,2 (4,85–9,35) |
| Gölge savaşında öldürülen usta ya da lider | 1,55 (1,15–2,4) | 1,35 (0,85–2,15) | 1,45 (0,95–1,95) | 1,3 (0,8–2) | 1,35 (0,95–1,95) | 1,4 (0,95–1,85) |
| Gizli şubeye baskın | 1 (0,5–1,65) | 0,4 (0,15–1,35) | 0,45 (0,25–0,6) | 0,55 (0,3–0,95) | 0,45 (0,25–0,8) | 0,4 (0,25–0,8) |
| Lobiyle yasa değişikliği | 0,85 (0,3–1,4) | 0,95 (0,25–1,9) | 0,75 (0,25–2,1) | 0,65 (0,3–1,5) | 1,2 (0,3–2,3) | 1,25 (0,1–2,35) |
| Darbe girişimi | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,05) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) |
| Örgüt ilanı (Avcılar) | 0,85 (0,6–1,8) | 0,75 (0,45–2,85) | 0,7 (0,4–3,05) | 0,8 (0,5–3,25) | 0,75 (0,4–3,1) | 0,7 (0,45–2,45) |
| Örgüt üyesi kahraman payı (yıl sonu) | %100 (%99–%100) | %100 (%99–%100) | %100 (%99–%100) | %100 (%99–%100) | %100 (%99–%100) | %100 (%99–%100) |
| Yönetici değişimi | 1,2 (0,85–1,55) | 1 (0,5–1,4) | 1,1 (0,4–1,35) | 1,15 (0,75–1,6) | 1,25 (0,95–1,85) | 1,3 (0,55–2,1) |
| Veraset krizi | 0 (0–0,1) | 0 (0–0,05) | 0 (0–0,05) | 0 | 0 | 0 |
| Meşruiyet ortalaması (yıl sonu) | 64,2 (61,5–67,5) | 65,4 (61–70,2) | 64,8 (59,1–70,4) | 65,5 (61,3–71,4) | 63,1 (59,2–70,4) | 63,6 (58–70,1) |
| Pakt'a bağlı yönetici (yıl sonu) | 0,1 (0–0,55) | 0,05 (0–0,3) | 0 (0–0,45) | 0 (0–0,15) | 0 (0–0,25) | 0,05 (0–0,25) |
| Köle (yıl sonu) | 7,6 (1,65–32,4) | 7,1 (2,05–36,6) | 6,8 (3,65–26,2) | 18,9 (4,4–30,7) | 19,2 (7,6–46) | 19 (6,95–49,4) |
| Köle payı (nüfusun, yıl sonu) | %0,3 (%0,1–%1,1) | %0,2 (%0,1–%1,3) | %0,2 (%0,1–%0,9) | %0,6 (%0,1–%1) | %0,6 (%0,3–%1,4) | %0,6 (%0,2–%1,8) |
| Hapis madeninde mahkûm (yıl sonu) | 0,8 (0–2,45) | 0,75 (0–3,5) | 0,3 (0–2,65) | 0,55 (0–2,1) | 0,15 (0–1,7) | 0,2 (0–2,35) |
| Esarete düşen | 5,9 (1,4–9,55) | 5,7 (1,4–11,2) | 6,15 (3,5–9,25) | 9,25 (2,25–13,1) | 8,6 (3,8–13,4) | 8,3 (5,55–15,1) |
| Kurtulan köle (Özgürlük Ağı, kaçış, azat) | 7,25 (2,4–11,7) | 5 (1,15–10,4) | 5,6 (3,35–10,2) | 8 (2,45–12,1) | 7,8 (4,1–12,4) | 9,8 (5,4–13,1) |
| Özgürlük Ağı'nın kurtardığı köle | 5,2 (1,7–8,85) | 4,45 (0,95–7,85) | 4,8 (2,85–7,5) | 6,45 (1,9–8,65) | 6,1 (2,9–9,4) | 7,55 (4,05–9,85) |
| Esir kahraman (yıl sonu) | 0,3 (0,05–1,9) | 0,2 (0,05–1,55) | 0,2 (0–1,05) | 0,1 (0–0,75) | 0,05 (0–0,75) | 0,1 (0–0,55) |
| Aç haydut kampı (yıl sonu) | 0,4 (0,15–1,3) | 0,45 (0,05–1,4) | 0,8 (0,2–1,55) | 0,6 (0,2–2,45) | 0,85 (0,35–2,55) | 0,85 (0,35–2,4) |
| Haydut olan aç halk | 6,65 (2,3–16,8) | 7,65 (2,8–12,1) | 8,2 (4,05–22,2) | 10,7 (6,35–22,9) | 14,1 (3,9–28,8) | 13,8 (5,2–26,8) |
| Aç ya da ekmeksiz yerleşim payı (köy+) | %4,5 (%2,3–%10) | %4,4 (%2,5–%11) | %5,9 (%3,1–%16) | %7,1 (%3,8–%20) | %9,9 (%2,7–%22) | %9 (%3–%23) |
| Devriye durdurması | 49,9 (37,7–69,3) | 51,2 (32,8–76,3) | 50,4 (34,7–75,2) | 53,5 (36,9–70,5) | 52,5 (41,9–67,1) | 53,8 (34,3–74) |

## Kahraman seviyeleri

### Doğuş seviyesi (bütün dünyalar, on yıl içinde doğanlar)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 |
|---|---|---|---|---|---|
| 1–10 | 1704 | 1,63 | %47 | %44 | %9,5 |
| 11–20 | 1726 | 1,63 | %48 | %42 | %11 |
| 21–30 | 1792 | 1,61 | %49 | %41 | %9,9 |
| 31–40 | 1919 | 1,6 | %48 | %43 | %8,8 |
| 41–50 | 2029 | 1,66 | %45 | %44 | %11 |
| 51–60 | 2037 | 1,64 | %46 | %43 | %11 |

### Ölüm seviyesi (bütün dünyalar, on yıl içinde ölenler)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 | Sv4 | Sv5 | Sv6 | Sv7 | Sv8 | Sv9 | Sv10 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 1–10 | 560 | 4,61 | %4,5 | %10 | %17 | %23 | %18 | %10 | %5,7 | %7 | %2,5 | %2,5 |
| 11–20 | 681 | 4,69 | %4,3 | %7,5 | %19 | %21 | %22 | %7,9 | %5,6 | %5,7 | %2,8 | %4,1 |
| 21–30 | 769 | 4,87 | %4,9 | %9,6 | %14 | %19 | %19 | %12 | %6,8 | %5,9 | %5,3 | %3,6 |
| 31–40 | 704 | 4,81 | %6,8 | %10 | %13 | %17 | %20 | %11 | %8,5 | %6,4 | %3,3 | %4,3 |
| 41–50 | 773 | 4,81 | %5 | %10 | %14 | %17 | %23 | %10 | %7,5 | %6,2 | %2,8 | %4,1 |
| 51–60 | 722 | 4,78 | %5,3 | %7,8 | %16 | %16 | %23 | %13 | %7,8 | %5,4 | %2,2 | %3,5 |

### Yaşayan kahramanların seviyesi (bütün dünyalar, on yılın son yılının sonunda)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 | Sv4 | Sv5 | Sv6 | Sv7 | Sv8 | Sv9 | Sv10 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 10 | 2343 | 4,59 | %3 | %7,5 | %19 | %29 | %19 | %7,4 | %4,2 | %4,5 | %2,9 | %3,9 |
| 20 | 2375 | 4,69 | %2,9 | %7,9 | %16 | %28 | %19 | %8,1 | %5,7 | %3,5 | %3,8 | %4 |
| 30 | 2434 | 4,66 | %3,5 | %7,1 | %17 | %28 | %20 | %7,8 | %5,4 | %3,9 | %2,5 | %4,6 |
| 40 | 2532 | 4,61 | %3 | %8,4 | %17 | %25 | %23 | %8,1 | %5,1 | %3,4 | %2,3 | %4 |
| 50 | 2593 | 4,51 | %2,8 | %8,7 | %19 | %26 | %22 | %8,8 | %4,9 | %3 | %2,3 | %2,9 |
| 60 | 2680 | 4,54 | %3 | %8,1 | %16 | %28 | %23 | %8,3 | %5 | %3 | %1,9 | %3,2 |

### Ölüm nedenleri (bütün dünyalar)

| Neden | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 | Toplam |
|---|---|---|---|---|---|---|---|
| Kamp saldırısı | 198 | 202 | 198 | 160 | 182 | 171 | 1111 (%26) |
| Trol | 164 | 132 | 159 | 176 | 127 | 112 | 870 (%21) |
| Bilinmiyor | 51 | 115 | 158 | 151 | 196 | 199 | 870 (%21) |
| Kuşatma | 97 | 98 | 96 | 71 | 104 | 102 | 568 (%13) |
| Ejderha | 0 | 61 | 98 | 73 | 82 | 60 | 374 (%8,9) |
| Düello | 31 | 40 | 32 | 40 | 28 | 28 | 199 (%4,7) |
| Kervan soygunu | 13 | 16 | 19 | 26 | 25 | 27 | 126 (%3) |
| Han baskını (canavar) | 5 | 3 | 1 | 4 | 12 | 8 | 33 (%0,8) |
| Yağma akını | 0 | 8 | 4 | 0 | 9 | 4 | 25 (%0,6) |
| Han baskını (medeniyet) | 0 | 2 | 1 | 3 | 7 | 9 | 22 (%0,5) |
| Yol pususu | 0 | 4 | 3 | 0 | 1 | 2 | 10 (%0,2) |
| Yerleşim baskını (canavar) | 1 | 0 | 0 | 0 | 0 | 0 | 1 (%0) |

Neden, ölümün kaydedildiği andaki son muharebenin türünden (başlık ve taraflar) ya da suikast olayından çıkarılır.

## Olay türleri

Dünya başına yıllık olay sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Kahraman (`hero`) | 786 (603–962) | 853 (646–978) | 866 (697–1059) | 945 (715–1194) | 1008 (812–1157) | 1051 (830–1210) |
| Sefer/ilan (`quest`) | 118 (73,7–187) | 125 (76,7–178) | 133 (80,2–209) | 138 (99–239) | 139 (82,5–224) | 159 (57,5–229) |
| state | 100 (84,7–105) | 101 (80,8–111) | 100 (79,9–107) | 95,4 (77,3–109) | 90,5 (79,4–108) | 97 (74,8–105) |
| org | 59,7 (57,3–65,4) | 55 (47,3–59,2) | 53 (47,1–58,7) | 52,7 (46–59,8) | 54,6 (47,7–60,3) | 53,4 (48,2–61,7) |
| Göç (`migration`) | 42,8 (30–51,2) | 43,8 (30,2–50,4) | 37,3 (22,5–51,8) | 33,7 (23,8–52,2) | 32,3 (23,4–52,5) | 33,3 (19,8–50) |
| Kamp (`lair`) | 26,2 (11,8–35,8) | 26,7 (12,7–31,2) | 23,3 (13,5–36,8) | 27,3 (17,7–35,5) | 26,8 (12,4–32,9) | 19,3 (12,7–37,4) |
| İnşaat (`build`) | 22,3 (13,1–36,9) | 20,9 (13,2–30,3) | 15,8 (13,9–33,1) | 17,6 (12,8–29,5) | 16,7 (11,9–24,1) | 15,1 (10,8–25,1) |
| Savaş (`war`) | 17 (10,2–25,9) | 15,3 (9,35–28,2) | 16,4 (4,55–25,2) | 16,1 (6,3–31,6) | 15 (5,95–31) | 17,6 (6,7–34,4) |
| Han (`inn`) | 13,2 (7,9–18,2) | 14 (9,15–17,5) | 13,8 (9,45–18,9) | 14,3 (11,4–19,2) | 16,2 (12–20,6) | 15,9 (11,7–21,8) |
| hub | 12,9 (11,7–14) | 12,4 (10,8–13,7) | 12,9 (11,5–13,9) | 12,5 (11,1–13,7) | 12,7 (11–13,8) | 11,8 (10,4–13,2) |
| Ölüm/terk (`death`) | 6,45 (5,45–8,75) | 7,75 (5,75–9,8) | 8,1 (6,4–10,6) | 7,6 (6,15–9,2) | 8,05 (6,35–9,95) | 7,1 (6,15–9,65) |
| Deniz (`sea`) | 8,5 (4,55–10,7) | 7,75 (4,75–12,2) | 7,55 (5–13,8) | 7,3 (4,1–11) | 7,3 (3,6–11,3) | 6,55 (3,35–12,2) |
| politics | 6,95 (6,05–8,15) | 6,9 (5,15–8,05) | 6,65 (5,2–8,25) | 6,45 (4,5–8,6) | 6,9 (4,65–9,05) | 6,55 (4,2–9,7) |
| Ekonomi (`economy`) | 7,75 (5,6–8,55) | 7,2 (5–9,15) | 6,05 (4,3–8,3) | 6,65 (4,15–8,4) | 6,55 (4,3–8,6) | 5,8 (3,65–7,6) |
| epitaph | 3,4 (2,05–5,05) | 4,35 (2,3–5,6) | 4,7 (3,3–7,1) | 4,35 (3,2–5,95) | 4,75 (3,55–6,55) | 4 (3,15–6,2) |
| Baskın (`raid`) | 4,1 (2,55–5,5) | 3,75 (2,4–4,7) | 3,6 (2,7–5,05) | 3,9 (2,55–5,5) | 3,45 (1,9–5,75) | 3,9 (2,4–5,05) |
| Büyüme (`growth`) | 4,5 (3,3–4,7) | 3,8 (3,25–4,45) | 3,45 (3,2–4,5) | 3,35 (2,3–4,6) | 3,3 (2,8–4,05) | 3,15 (2,2–4,1) |
| Dünya (`world`) | 2,5 (1,95–2,9) | 2,1 (1,4–3,3) | 2,3 (1,55–3,5) | 2,4 (1,5–2,8) | 2,3 (1,7–3) | 2,55 (1,65–3,15) |
| Diplomasi (`diplomacy`) | 1,85 (0,95–4,05) | 1,95 (0,75–3,5) | 2,5 (0,6–4,4) | 2,2 (0,25–4,8) | 2,05 (0,7–4,45) | 1,8 (0,6–4,55) |
| Yerleşim (`settle`) | 1,9 (0,75–4,3) | 0,95 (0,4–2,85) | 1,95 (0,4–4,2) | 1,95 (0,7–3,55) | 1,3 (0,2–3,25) | 1,4 (0,7–3,15) |
| Gerginlik (`tension`) | 1,55 (0,8–3,15) | 1,5 (0,5–2,4) | 1,4 (0,35–3,25) | 1,35 (0,15–3,05) | 1,3 (0,3–3,6) | 1,2 (0,35–3,35) |
| Ticaret (`trade`) | 1,35 (0,5–3) | 1,1 (0,2–2,75) | 1,6 (0,45–2,85) | 0,75 (0,4–2,55) | 0,95 (0,25–1,85) | 1,2 (0,05–2,5) |
| Sınıf (`class`) | 1 (0,05–2) | 1 (0–2,05) | 1 (0–2,5) | 1 (0–2,7) | 0,7 (0–2,5) | 1,05 (0–2,55) |
| Keşif (`discover`) | 1,5 (0,8–2,25) | 1,15 (0,4–1,65) | 0,75 (0,25–1,55) | 0,95 (0,5–1,5) | 0,6 (0,3–0,85) | 0,55 (0,25–1,05) |
| Ejderha (`dragon`) | 0 | 1,4 (0,8–1,9) | 0,5 (0–2,85) | 0 (0–2,7) | 0 (0–2,9) | 0 (0–2,8) |
| Rahatlama (anlatıcı) (`relief`) | 0,2 (0,05–0,35) | 0,3 (0,15–0,5) | 0,15 (0–0,5) | 0,3 (0,1–0,6) | 0,2 (0,05–0,55) | 0,2 (0,1–0,7) |
| Kriz (anlatıcı) (`crisis`) | 0,2 (0–0,5) | 0,1 (0–0,4) | 0,05 (0–0,45) | 0,05 (0–0,45) | 0 (0–0,55) | 0 (0–0,6) |
| Temas (`contact`) | 0 (0–0,25) | 0 | 0 (0–0,05) | 0 (0–0,05) | 0 | 0 |

## Büyük olay türleri

Dünya başına yıllık büyük olay sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Sefer/ilan (`quest`) | 77,9 (48,8–124) | 83,2 (50,2–119) | 88,9 (52,6–142) | 90,3 (66,2–162) | 92,9 (56,3–152) | 105 (39,5–151) |
| Kahraman (`hero`) | 66,4 (55,5–80,8) | 71,2 (58,4–80,9) | 75,8 (59,9–87,1) | 80,2 (58,7–95,8) | 83,1 (68,6–91,8) | 88,9 (67,4–99,6) |
| Kamp (`lair`) | 17,9 (8,25–25,4) | 18,3 (8,65–22,5) | 16,4 (8,95–26,3) | 17,7 (11,9–24,5) | 17,1 (8,15–22,7) | 13,4 (8,35–25,8) |
| Savaş (`war`) | 11,6 (7,25–16,7) | 10,4 (5,5–18,1) | 11 (2,9–17,1) | 10,5 (3,8–20) | 10,1 (4–21,4) | 10,7 (3,95–21,7) |
| Ölüm/terk (`death`) | 6,45 (5,45–8,75) | 7,75 (5,75–9,8) | 8,1 (6,4–10,6) | 7,6 (6,15–9,2) | 8,05 (6,35–9,95) | 7,1 (6,15–9,65) |
| Deniz (`sea`) | 5,45 (2,5–7,05) | 4,75 (2,4–8,25) | 4,9 (2,8–8,75) | 5,3 (2,9–8,25) | 5,45 (2,35–7,85) | 5 (2,3–9,25) |
| epitaph | 3,4 (2,05–5,05) | 4,35 (2,3–5,6) | 4,7 (3,3–7,1) | 4,35 (3,2–5,95) | 4,75 (3,55–6,55) | 4 (3,15–6,2) |
| politics | 4,05 (3,6–5) | 4,25 (2,7–4,9) | 4,05 (2,6–4,9) | 4,2 (2,95–5,25) | 4,55 (2,4–5,45) | 4,2 (2,55–5,6) |
| Baskın (`raid`) | 3,6 (1,95–4,75) | 3,15 (2,05–3,95) | 3,25 (2,15–4,35) | 3,6 (2,35–4,9) | 3,1 (1,5–5) | 3,7 (2,05–4,7) |
| org | 4,4 (3,55–5,4) | 3,5 (2,3–4,95) | 3,2 (2,35–5,1) | 3 (2,35–3,7) | 3,05 (2,55–3,75) | 2,9 (2,35–3,8) |
| Han (`inn`) | 2,65 (1,35–3,5) | 2,8 (2,35–4,05) | 2,85 (2,15–3,85) | 3,5 (2,15–4,9) | 3,3 (2,6–4,4) | 3,65 (2,35–4,85) |
| hub | 2,55 (2,35–2,85) | 2,4 (2,1–2,8) | 2,55 (2,2–2,8) | 2,5 (2,15–2,75) | 2,5 (2,2–2,7) | 2,35 (2,1–2,65) |
| Dünya (`world`) | 1,85 (1,55–2,2) | 1,8 (1,05–2,45) | 1,85 (1,15–2,7) | 1,85 (1,3–2,5) | 1,7 (1,35–2,2) | 1,9 (1,35–2,65) |
| İnşaat (`build`) | 2,25 (1,65–2,8) | 1,75 (0,95–2,5) | 1,7 (0,85–2,45) | 1,35 (0,75–2,1) | 1,4 (0,55–2,05) | 1 (0,7–1,75) |
| Gerginlik (`tension`) | 1,55 (0,8–3,15) | 1,5 (0,5–2,4) | 1,4 (0,35–3,25) | 1,35 (0,15–3,05) | 1,3 (0,3–3,6) | 1,2 (0,35–3,35) |
| Diplomasi (`diplomacy`) | 0,85 (0,35–2,3) | 0,7 (0,3–1,9) | 1 (0,15–1,9) | 0,95 (0,05–3,25) | 0,9 (0,35–2,5) | 0,75 (0,35–2,1) |
| Yerleşim (`settle`) | 0,75 (0,4–2,1) | 0,5 (0,2–1,5) | 0,9 (0,2–2,25) | 0,75 (0,25–1,55) | 0,6 (0,1–1,7) | 0,55 (0,25–1,3) |
| Ticaret (`trade`) | 0,5 (0,25–1,15) | 0,45 (0,05–1,15) | 0,5 (0,15–1,4) | 0,3 (0,15–1) | 0,4 (0–0,7) | 0,5 (0–1,05) |
| Keşif (`discover`) | 0,75 (0,4–1,3) | 0,45 (0,15–0,9) | 0,35 (0,1–1) | 0,45 (0,15–0,65) | 0,3 (0–0,5) | 0,2 (0,05–0,55) |
| Ejderha (`dragon`) | 0 | 1,4 (0,8–1,8) | 0,5 (0–2,85) | 0 (0–2,7) | 0 (0–2,9) | 0 (0–2,8) |
| Göç (`migration`) | 0,25 (0,1–0,75) | 0,3 (0–0,6) | 0,3 (0,1–0,75) | 0,3 (0,15–1,25) | 0,3 (0,15–1,45) | 0,3 (0,1–1,7) |
| Ekonomi (`economy`) | 0,3 (0,05–0,75) | 0,4 (0,2–0,65) | 0,3 (0,1–0,7) | 0,15 (0,05–0,65) | 0,35 (0–0,85) | 0,2 (0–0,65) |
| Rahatlama (anlatıcı) (`relief`) | 0,2 (0,05–0,35) | 0,3 (0,15–0,5) | 0,15 (0–0,5) | 0,3 (0,1–0,6) | 0,2 (0,05–0,55) | 0,2 (0,1–0,7) |
| Büyüme (`growth`) | 0,1 (0–0,3) | 0,1 (0–0,35) | 0,1 (0–0,45) | 0 (0–0,35) | 0,1 (0–0,4) | 0,05 (0–0,25) |
| Kriz (anlatıcı) (`crisis`) | 0,2 (0–0,5) | 0,1 (0–0,4) | 0,05 (0–0,45) | 0,05 (0–0,45) | 0 (0–0,55) | 0 (0–0,6) |
| Sınıf (`class`) | 0 (0–0,35) | 0 | 0 (0–0,05) | 0 | 0 (0–0,05) | 0 (0–0,05) |
| Temas (`contact`) | 0 (0–0,25) | 0 | 0 (0–0,05) | 0 (0–0,05) | 0 | 0 |

## Muharebe türleri

Dünya başına yıllık muharebe sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Kamp saldırısı (`camp`) | 9 (4,65–10,6) | 8,15 (5,3–10) | 8,55 (6,25–11,5) | 9,55 (6,15–11,4) | 9,3 (5,25–11,6) | 8 (5,35–13,5) |
| Trol (`troll`) | 3,7 (1,1–6) | 3,15 (1,35–5,15) | 3,1 (0,9–6,35) | 3,4 (1,05–6,5) | 2,4 (0,9–5,65) | 2,55 (1,15–6,85) |
| Kuşatma (`siege`) | 2,6 (2,1–3,15) | 2,65 (1,5–3,45) | 2,4 (0,85–3,25) | 2,3 (1,25–3,65) | 2,35 (1,25–3,25) | 2,55 (1–3,65) |
| Yağma akını (`plunder`) | 1,15 (0–3,5) | 1,05 (0–4,05) | 1,6 (0–4,1) | 1,5 (0–4,9) | 1,7 (0–5,3) | 1,6 (0–6,2) |
| Yapı baskını (canavar) (`extRaid`) | 1,2 (0,75–1,65) | 0,95 (0,4–1,7) | 1 (0,35–1,95) | 1,15 (0,5–2,05) | 0,9 (0,4–2,2) | 1,05 (0,65–1,9) |
| Yerleşim baskını (canavar) (`raid`) | 0,6 (0,35–1,3) | 0,55 (0,15–1,25) | 0,5 (0,2–0,65) | 0,35 (0,1–0,9) | 0,4 (0,2–1,1) | 0,4 (0,1–1,1) |
| Kervan soygunu (`robbery`) | 0,2 (0,1–0,6) | 0,3 (0,2–0,5) | 0,25 (0,15–0,55) | 0,4 (0,2–0,7) | 0,4 (0,15–0,6) | 0,3 (0,1–0,85) |
| Düello (`duel`) | 0,15 (0–0,4) | 0,2 (0,1–0,5) | 0,2 (0–0,4) | 0,25 (0,05–0,45) | 0,2 (0,05–0,3) | 0,15 (0–0,35) |
| Ejderha (`dragon`) | 0 | 0,7 (0,4–0,85) | 0,2 (0–1,55) | 0 (0–1,4) | 0 (0–1,45) | 0 (0–1,25) |
| Deniz savaşı (`naval`) | 0,1 (0–0,4) | 0,15 (0–0,65) | 0,15 (0–0,3) | 0 (0–0,2) | 0 (0–0,2) | 0,1 (0–0,25) |
| Han baskını (canavar) (`innMonster`) | 0 (0–0,2) | 0 (0–0,15) | 0,05 (0–0,2) | 0,05 (0–0,2) | 0,1 (0–0,3) | 0,1 (0–0,15) |
| Korsan savaşı (`pirate`) | 0 (0–0,2) | 0,05 (0–0,1) | 0 (0–0,1) | 0,05 (0–0,2) | 0 (0–0,1) | 0 (0–0,1) |
| Yol pususu (`ambush`) | 0 | 0 (0–0,1) | 0 (0–0,1) | 0 | 0 (0–0,1) | 0 (0–0,05) |
| Han baskını (medeniyet) (`innCiv`) | 0 | 0 | 0 | 0 | 0 (0–0,05) | 0 (0–0,1) |

## Kamp türleri

Dünya başına yaşayan kamp (yıl sonu değerlerinin on yıllık ortalaması): dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Hobgoblin (`hobgoblin`) | 3,15 (2,5–4,2) | 3,6 (3,1–4,15) | 3,05 (2,5–3,85) | 3,05 (2,8–3,95) | 3,15 (2,55–3,45) | 3,15 (2,7–3,6) |
| Trol (`troll`) | 2,25 (1,85–2,8) | 2,15 (1,5–2,65) | 2,55 (2–2,95) | 2,55 (2,2–3) | 2,85 (2,35–3) | 2,8 (2,25–3) |
| Goblin (`goblin`) | 1,75 (1,1–2,4) | 1,65 (1,2–2,05) | 1,75 (1,25–2,15) | 1,6 (1–2) | 1,6 (1,25–1,85) | 1,4 (1,05–2,35) |
| Bugbear (`bugbear`) | 0,8 (0,6–1,2) | 0,65 (0,3–1,15) | 0,75 (0,5–1,4) | 0,7 (0,45–1,25) | 0,85 (0,35–1,5) | 1 (0,25–1,45) |
| Aç haydut (`bandit`) | 0,4 (0,15–1,3) | 0,45 (0,05–1,4) | 0,8 (0,2–1,55) | 0,6 (0,2–2,45) | 0,85 (0,35–2,55) | 0,85 (0,35–2,4) |
| Korsan (`pirate`) | 0,35 (0,1–0,85) | 0,3 (0,1–0,7) | 0,3 (0,15–0,9) | 0,5 (0,05–0,85) | 0,3 (0,1–0,85) | 0,35 (0,05–0,5) |
| Ejderha (`dragon`) | 0 | 0,5 (0,2–0,65) | 0,15 (0–1) | 0 (0–1) | 0 (0–1) | 0 (0–1) |

## Kademe dağılımı

Yaşayan yerleşimlerin kademelere dağılımı, bütün dünyalar (yıl sonu; parantezde sayı).

| Yıl | 0 Kamp | 1 Köy | 2 Kasaba | 3 Şehir |
|---|---|---|---|---|
| 10 | %22 (273) | %48 (603) | %24 (305) | %6,2 (78) |
| 20 | %22 (264) | %51 (623) | %21 (255) | %6,8 (83) |
| 30 | %22 (276) | %54 (671) | %17 (208) | %6,6 (81) |
| 40 | %22 (280) | %53 (656) | %18 (225) | %6,8 (85) |
| 50 | %22 (277) | %54 (675) | %17 (205) | %6,6 (82) |
| 60 | %20 (249) | %57 (702) | %15 (189) | %7,1 (87) |

Başkentlerin kademelere dağılımı, bütün dünyalar (yıl sonu; parantezde sayı).

| Yıl | 0 Kamp | 1 Köy | 2 Kasaba | 3 Şehir |
|---|---|---|---|---|
| 10 | %1,9 (2) | %4,9 (5) | %37 (38) | %56 (58) |
| 20 | %0 (0) | %8,9 (9) | %28 (28) | %63 (64) |
| 30 | %1 (1) | %4,8 (5) | %32 (33) | %62 (65) |
| 40 | %2,9 (3) | %2,9 (3) | %29 (30) | %65 (66) |
| 50 | %1 (1) | %5,7 (6) | %30 (31) | %64 (67) |
| 60 | %0 (0) | %6,4 (7) | %26 (29) | %67 (74) |

## Dünyalar

| Seed | Medeniyet | Yerleşim | Nüfus | Çöküş | Efsane | En yüksek Sv | Doğan / ölü kahraman | İlk şehir (yıl, medyan) | Süre (sn) | Son hash |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | 5 | 56 | 2055 | 12 | 3 | 10 | 535 / 176 | – | 109 | `f1ea090694ab0230` |
| 2 | 6 | 81 | 2962 | 10 | 8 | 10 | 746 / 208 | – | 143 | `5f61d0efa6498895` |
| 3 | 8 | 78 | 3283 | 10 | 0 | 10 | 500 / 149 | – | 120 | `54da56ada6f33cc4` |
| 4 | 7 | 76 | 2962 | 11 | 1 | 10 | 953 / 96 | – | 132 | `10dd185377d57c04` |
| 5 | 7 | 83 | 4075 | 7 | 0 | 10 | 1082 / 152 | – | 186 | `559a658fcff9a809` |
| 6 | 7 | 79 | 2995 | 14 | 4 | 10 | 989 / 213 | – | 137 | `d3ea66195b2e7cb5` |
| 7 | 7 | 77 | 2852 | 10 | 1 | 10 | 723 / 140 | – | 117 | `0fc8b60f5a94e3ed` |
| 8 | 6 | 79 | 3040 | 8 | 6 | 10 | 543 / 161 | – | 122 | `4c02548ee4287e44` |
| 9 | 6 | 74 | 2619 | 11 | 1 | 10 | 730 / 192 | – | 108 | `da1cbef2199f1de5` |
| 10 | 7 | 75 | 2315 | 10 | 2 | 10 | 610 / 180 | – | 114 | `3dc4251412efe1ac` |
| 11 | 8 | 73 | 2719 | 7 | 0 | 10 | 465 / 158 | – | 106 | `533256e2c6b6118b` |
| 12 | 8 | 79 | 3722 | 5 | 6 | 10 | 779 / 200 | – | 138 | `27b94d2df084371f` |
| 13 | 8 | 77 | 3039 | 9 | 3 | 10 | 566 / 194 | – | 123 | `21ae8de5df9b8ed8` |
| 14 | 6 | 80 | 3318 | 9 | 2 | 10 | 486 / 180 | – | 117 | `ea0ff026e85324ab` |
| 15 | 7 | 79 | 2958 | 10 | 1 | 10 | 570 / 227 | – | 118 | `3c7be506cbd88270` |
| 16 | 7 | 81 | 2878 | 11 | 4 | 10 | 930 / 164 | – | 139 | `d1069bd52b3e204c` |

Çöküşler:

- seed 1, 7. yıl (gün 245): Demirkanat Krallığı başkenti kaybetti: Taşkandil (Granitsunak Krallığı aldı)
- seed 1, 7. yıl (gün 279): Demirçan Krallığı başkenti kaybetti: Siskoru (Demirkanat Krallığı aldı)
- seed 1, 8. yıl (gün 319): Alacayayla Krallığı yok oldu
- seed 1, 11. yıl (gün 402): Demirkanat Krallığı başkenti kaybetti: Alacayayla (Yeminköprü Teokrasisi aldı)
- seed 1, 13. yıl (gün 494): Granitsunak Krallığı başkenti kaybetti: Karlıtepe (Kızılboynuz Krallığı aldı)
- seed 1, 14. yıl (gün 535): Kızılboynuz Krallığı başkenti kaybetti: Karlıtepe (Demirçan Krallığı aldı)
- seed 1, 16. yıl (gün 607): Tuzyayla Cumhuriyeti yok oldu
- seed 1, 20. yıl (gün 778): Demirçan Krallığı başkenti kaybetti: Karlıtepe (Granitsunak Krallığı aldı)
- seed 1, 24. yıl (gün 929): Granitsunak Krallığı başkenti kaybetti: Karlıtepe (Kızılboynuz Krallığı aldı)
- seed 1, 26. yıl (gün 1035): Taşkandil Krallığı başkenti kaybetti: Taşkandil (Granitsunak Krallığı aldı)
- seed 1, 35. yıl (gün 1381): Kızılboynuz Krallığı yok oldu
- seed 1, 40. yıl (gün 1572): Taşkandil Cumhuriyeti başkenti kaybetti: Yeni Gökköy (Granitsunak Krallığı aldı)
- seed 2, 2. yıl (gün 72): Pulzırh Krallığı başkenti kaybetti: Şarkıdere (Şafaktepe Teokrasisi aldı)
- seed 2, 5. yıl (gün 195): Sessizkoru Boyları başkenti kaybetti: Telliköprü (Pulzırh Krallığı aldı)
- seed 2, 6. yıl (gün 210): Tamburlu Boyları başkenti kaybetti: Tamburlu (Şafaktepe Teokrasisi aldı)
- seed 2, 7. yıl (gün 246): Bozkent Boyları yok oldu
- seed 2, 8. yıl (gün 304): Tamburlu Boyları yok oldu
- seed 2, 20. yıl (gün 797): Şafaktepe Teokrasisi başkenti kaybetti: Meşekale (Sessizkoru Boyları aldı)
- seed 2, 23. yıl (gün 903): Şafaktepe Teokrasisi başkenti kaybetti: Pınardere (Pulzırh Cumhuriyeti aldı)
- seed 2, 28. yıl (gün 1102): Taşkandil Krallığı başkenti kaybetti: Pınardere (Pulzırh Krallığı aldı)
- seed 2, 30. yıl (gün 1183): Kemikçadır Teokrasisi yok oldu
- seed 2, 56. yıl (gün 2213): Sessizkoru Boyları başkenti kaybetti: Çiyardı (Taşkandil Krallığı aldı)
- seed 3, 2. yıl (gün 63): Kanlıdiş Boyları başkenti kaybetti: İzsürer (Karaörs Teokrasisi aldı)
- seed 3, 5. yıl (gün 185): Kanlıdiş Boyları başkenti kaybetti: Yeşilyazı (Karaörs Teokrasisi aldı)
- seed 3, 10. yıl (gün 365): Yeşilyazı Krallığı başkenti kaybetti: Yeşilyazı (Karaörs Teokrasisi aldı)
- seed 3, 27. yıl (gün 1052): Kanlıdiş Boyları yok oldu
- seed 3, 28. yıl (gün 1103): Karaörs Krallığı başkenti kaybetti: İzsürer (Özgür Yeşilyazı aldı)
- seed 3, 30. yıl (gün 1184): Karaörs Krallığı başkenti kaybetti: Yelgeçit (Sisliyamaç Krallığı aldı)
- seed 3, 33. yıl (gün 1293): Karaörs Krallığı yok oldu
- seed 3, 43. yıl (gün 1717): Güneştacı Krallığı başkenti kaybetti: Yabanyurt (Ayışığı Cumhuriyeti aldı)
- seed 3, 49. yıl (gün 1934): Yeni Karayazı Boyları yok oldu
- seed 3, 55. yıl (gün 2191): Tatlıçayır Boyları başkenti kaybetti: Kavşakpazar Kutsal Korusu II (terk edildi)
- seed 4, 2. yıl (gün 75): Toynakbaş Boyları başkenti kaybetti: Çamgözcü (Kalkanova Teokrasisi aldı)
- seed 4, 5. yıl (gün 174): Yeşilyaprak Boyları başkenti kaybetti: Alacayurt (Keseli Cumhuriyeti aldı)
- seed 4, 9. yıl (gün 345): Keseli Cumhuriyeti başkenti kaybetti: Alacayurt (Kalkanova Teokrasisi aldı)
- seed 4, 11. yıl (gün 412): Yeşilyaprak Boyları başkenti kaybetti: Külçukur (Pulzırh Krallığı aldı)
- seed 4, 13. yıl (gün 492): Toynakbaş Teokrasisi yok oldu
- seed 4, 33. yıl (gün 1319): Külçukur Cumhuriyeti yok oldu
- seed 4, 50. yıl (gün 1993): Kalkanova Teokrasisi başkenti kaybetti: Çamgözcü (Toynakbaş Boyları aldı)
- seed 4, 56. yıl (gün 2202): Sessizocak Cumhuriyeti başkenti kaybetti: Sessizocak (Toynakbaş Boyları aldı)
- seed 4, 56. yıl (gün 2232): Pulzırh Krallığı başkenti kaybetti: Alacayurt (Yeşilyaprak Boyları aldı)
- seed 4, 57. yıl (gün 2273): Çamgözcü Cumhuriyeti yok oldu
- seed 4, 58. yıl (gün 2298): Uluova Cumhuriyeti yok oldu
- seed 5, 1. yıl (gün 13): Yelgeçit Teokrasisi başkenti kaybetti: Yelgeçit (Kızılboynuz Krallığı aldı)
- seed 5, 3. yıl (gün 99): Kemikçadır Teokrasisi yok oldu
- seed 5, 9. yıl (gün 330): Kızılyurt Boyları yok oldu
- seed 5, 18. yıl (gün 706): Kızılboynuz Krallığı yok oldu
- seed 5, 38. yıl (gün 1501): Sarıhisar Teokrasisi yok oldu
- seed 5, 58. yıl (gün 2300): Közsaray Cumhuriyeti başkenti kaybetti: Gölköprü (Akburç Teokrasisi aldı)
- seed 5, 59. yıl (gün 2339): Yelköy Cumhuriyeti yok oldu
- seed 6, 7. yıl (gün 263): Demirkanat Boyları başkenti kaybetti: Sarıhisar (Kalkanova Teokrasisi aldı)
- seed 6, 12. yıl (gün 461): Bulutkapı Boyları başkenti kaybetti: Bulutkapı (Kalkanova Krallığı aldı)
- seed 6, 13. yıl (gün 491): Kızılyurt Boyları yok oldu
- seed 6, 14. yıl (gün 531): Közsaray Krallığı başkenti kaybetti: Şarkıdere (Fıçıköy Krallığı aldı)
- seed 6, 15. yıl (gün 595): Kurtgeçit Boyları yok oldu
- seed 6, 19. yıl (gün 741): Közsaray Krallığı başkenti kaybetti: Kurtgeçit (Fıçıköy Krallığı aldı)
- seed 6, 26. yıl (gün 1010): Közsaray Krallığı başkenti kaybetti: Yeni Söğütkoru (Fıçıköy Krallığı aldı)
- seed 6, 26. yıl (gün 1031): Demirkanat Boyları başkenti kaybetti: Yeni Yeldere (Taşkandil Krallığı aldı)
- seed 6, 31. yıl (gün 1227): Kalkanova Krallığı yok oldu
- seed 6, 40. yıl (gün 1585): Taşkandil Cumhuriyeti başkenti kaybetti: Balköy (Demirkanat Boyları aldı)
- seed 6, 42. yıl (gün 1680): Taşkandil Cumhuriyeti yok oldu
- seed 6, 44. yıl (gün 1735): Demirkanat Boyları başkenti kaybetti: Balköy (Dumanpınar Krallığı aldı)
- seed 6, 45. yıl (gün 1795): Taşdere Boyları başkenti kaybetti: Yeni Ceylanova (Dumanpınar Krallığı aldı)
- seed 6, 48. yıl (gün 1912): Fıçıköy Krallığı başkenti kaybetti: Yeni Söğütkoru (Közsaray Krallığı aldı)
- seed 7, 16. yıl (gün 639): Fıçıköy Krallığı başkenti kaybetti: Demiroba (Kutsalörs Krallığı aldı)
- seed 7, 17. yıl (gün 667): Karaörs Krallığı başkenti kaybetti: Karlıçayır (Kurtoba Boyları aldı)
- seed 7, 19. yıl (gün 748): Fıçıköy Krallığı yok oldu
- seed 7, 20. yıl (gün 768): Karaörs Krallığı yok oldu
- seed 7, 22. yıl (gün 849): Kızılçayır Krallığı başkenti kaybetti: Kızılçayır (Kutsalörs Krallığı aldı)
- seed 7, 24. yıl (gün 923): Kurtoba Boyları başkenti kaybetti: Karlıçayır (Pulkalkan Krallığı aldı)
- seed 7, 36. yıl (gün 1415): Kızılçayır Krallığı başkenti kaybetti: Balköprü (Közburç Krallığı aldı)
- seed 7, 36. yıl (gün 1439): Pulkalkan Krallığı yok oldu
- seed 7, 40. yıl (gün 1579): Yeni Bozbük Boyları yok oldu
- seed 7, 47. yıl (gün 1854): Kutsalörs Krallığı başkenti kaybetti: Demiroba (Közburç Krallığı aldı)
- seed 8, 3. yıl (gün 114): Zincirkaya Krallığı yok oldu
- seed 8, 10. yıl (gün 368): Sessizkoru Cumhuriyeti başkenti kaybetti: Yeşilkaya (Pulzırh Krallığı aldı)
- seed 8, 10. yıl (gün 383): Pulzırh Krallığı başkenti kaybetti: Demirbük (Söğütsırt Teokrasisi aldı)
- seed 8, 18. yıl (gün 710): Sessizkoru Cumhuriyeti yok oldu
- seed 8, 20. yıl (gün 771): Söğütsırt Teokrasisi başkenti kaybetti: Demirbük (Güneştacı Teokrasisi aldı)
- seed 8, 24. yıl (gün 947): Taşkandil Cumhuriyeti başkenti kaybetti: Çamkoru (Ayışığı Krallığı aldı)
- seed 8, 36. yıl (gün 1438): Tatlıçayır Cumhuriyeti başkenti kaybetti: Balköprü (Ayışığı Krallığı aldı)
- seed 8, 60. yıl (gün 2388): Pulzırh Krallığı başkenti kaybetti: Yeşilkaya (Güneştacı Teokrasisi aldı)
- seed 9, 1. yıl (gün 30): Demirçan Krallığı başkenti kaybetti: Kurtoba (Şafaktepe Krallığı aldı)
- seed 9, 4. yıl (gün 152): Kurtoba Krallığı yok oldu
- seed 9, 5. yıl (gün 174): Demirkanat Teokrasisi başkenti kaybetti: Gökyayla (Güneştacı Krallığı aldı)
- seed 9, 8. yıl (gün 282): Kurtoba Teokrasisi başkenti kaybetti: Kurtoba (Şafaktepe Krallığı aldı)
- seed 9, 8. yıl (gün 314): Demirkanat Teokrasisi başkenti kaybetti: Ceylanbük (Güneştacı Krallığı aldı)
- seed 9, 11. yıl (gün 414): Demirkanat Teokrasisi başkenti kaybetti: Gökyayla (Güneştacı Krallığı aldı)
- seed 9, 13. yıl (gün 515): Güneştacı Krallığı başkenti kaybetti: Altınkapı (Şafaktepe Krallığı aldı)
- seed 9, 32. yıl (gün 1253): Güneştacı Teokrasisi başkenti kaybetti: Işıkdere (Demirçan Cumhuriyeti aldı)
- seed 9, 43. yıl (gün 1714): Demirçan Cumhuriyeti başkenti kaybetti: Zincirkaya (Demirkanat Teokrasisi aldı)
- seed 9, 46. yıl (gün 1813): Güneştacı Cumhuriyeti başkenti kaybetti: Çamkoru (Şafaktepe Krallığı aldı)
- seed 9, 59. yıl (gün 2354): Fıçıköy Cumhuriyeti başkenti kaybetti: Gölgeçarşı (Demirkanat Teokrasisi aldı)
- seed 10, 1. yıl (gün 29): Savaşçukur Boyları yok oldu
- seed 10, 4. yıl (gün 146): Kızılyurt Boyları başkenti kaybetti: Sarıdere (Yeşilyaprak Cumhuriyeti aldı)
- seed 10, 13. yıl (gün 492): Yeşilyaprak Cumhuriyeti başkenti kaybetti: Sarıdere (Kızılyurt Boyları aldı)
- seed 10, 19. yıl (gün 758): Dumanköprü Boyları yok oldu
- seed 10, 37. yıl (gün 1450): Közsaray Cumhuriyeti yok oldu
- seed 10, 43. yıl (gün 1681): Çamkoru Boyları yok oldu
- seed 10, 48. yıl (gün 1883): Gölgeörs Boyları yok oldu
- seed 10, 48. yıl (gün 1904): Ceylanbük Boyları başkenti kaybetti: Ceylanbük (Karamum Krallığı aldı)
- seed 10, 51. yıl (gün 2005): Yeşilyaprak Cumhuriyeti başkenti kaybetti: Meşekent (Yıldızçayır Boyları aldı)
- seed 10, 59. yıl (gün 2330): Taşkandil Krallığı başkenti kaybetti: Demirbük (Ceylanbük Boyları aldı)
- seed 11, 4. yıl (gün 133): Karaörs Teokrasisi başkenti kaybetti: Demiroba (Kutsalörs Boyları aldı)
- seed 11, 7. yıl (gün 257): Özgür Kutsalörs başkenti kaybetti: Alacayayla (Şafaktepe Krallığı aldı)
- seed 11, 13. yıl (gün 482): Gökyayla Cumhuriyeti başkenti kaybetti: Gökyayla (Demirkanat Boyları aldı)
- seed 11, 19. yıl (gün 740): Şafaktepe Teokrasisi başkenti kaybetti: Alacayayla (Kutsalörs Boyları aldı)
- seed 11, 23. yıl (gün 889): Gökyayla Cumhuriyeti yok oldu
- seed 11, 41. yıl (gün 1639): Gümüşdal Cumhuriyeti başkenti kaybetti: Yosunpınar (Karaörs Teokrasisi aldı)
- seed 11, 57. yıl (gün 2241): Karaörs Teokrasisi başkenti kaybetti: Alacayayla (Demirkanat Boyları aldı)
- seed 12, 13. yıl (gün 497): Gümüşdal Boyları başkenti kaybetti: Gökyurt (Fıçıköy Krallığı aldı)
- seed 12, 14. yıl (gün 558): Taşkandil Krallığı başkenti kaybetti: Dumanköprü (Fıçıköy Krallığı aldı)
- seed 12, 18. yıl (gün 681): Tamburlu Krallığı başkenti kaybetti: Tamburlu (Taşkandil Krallığı aldı)
- seed 12, 51. yıl (gün 2017): Tamburlu Krallığı yok oldu
- seed 12, 57. yıl (gün 2247): Gümüşdal Boyları başkenti kaybetti: Sarıdere (Közsaray Cumhuriyeti aldı)
- seed 13, 6. yıl (gün 240): Örsyürek Krallığı başkenti kaybetti: Kemikçadır (Kızılyurt Boyları aldı)
- seed 13, 11. yıl (gün 408): Kuzeyoba Krallığı yok oldu
- seed 13, 11. yıl (gün 414): Közburç Cumhuriyeti başkenti kaybetti: Kemikçadır (Kızılyurt Boyları aldı)
- seed 13, 14. yıl (gün 534): Şarkıdere Boyları başkenti kaybetti: Şarkıdere (Keseli Cumhuriyeti aldı)
- seed 13, 37. yıl (gün 1448): Örsyürek Cumhuriyeti başkenti kaybetti: Kurtoba (Kızılyurt Boyları aldı)
- seed 13, 42. yıl (gün 1653): Keseli Krallığı başkenti kaybetti: Kırkkapı (Közburç Krallığı aldı)
- seed 13, 42. yıl (gün 1655): Kartalkaya Boyları yok oldu
- seed 13, 49. yıl (gün 1946): Örsyürek Cumhuriyeti başkenti kaybetti: Kartalkaya (Şarkıdere Boyları aldı)
- seed 13, 56. yıl (gün 2236): Ejderkale Krallığı başkenti kaybetti: Balköy (Granitsunak Krallığı aldı)
- seed 14, 3. yıl (gün 103): İzsürer Boyları başkenti kaybetti: İzsürer (Okyayı Krallığı aldı)
- seed 14, 14. yıl (gün 551): Toynakbaş Boyları başkenti kaybetti: Meşekent (Pulzırh Krallığı aldı)
- seed 14, 30. yıl (gün 1176): Toynakbaş Boyları yok oldu
- seed 14, 34. yıl (gün 1334): Yeni Meşeyayla Cumhuriyeti yok oldu
- seed 14, 37. yıl (gün 1468): Özgür Gökyayla yok oldu
- seed 14, 38. yıl (gün 1502): Pulzırh Cumhuriyeti başkenti kaybetti: Pınardere (Okyayı Krallığı aldı)
- seed 14, 40. yıl (gün 1600): Tuzyurt Boyları yok oldu
- seed 14, 41. yıl (gün 1605): Yeminköprü Krallığı yok oldu
- seed 14, 59. yıl (gün 2334): Okyayı Teokrasisi başkenti kaybetti: Pınardere (Gökyayla Krallığı aldı)
- seed 15, 5. yıl (gün 183): Meşekent Krallığı yok oldu
- seed 15, 11. yıl (gün 419): Yeşilyaprak Krallığı başkenti kaybetti: Çiyardı (Kızılyurt Boyları aldı)
- seed 15, 12. yıl (gün 442): Közsaray Krallığı başkenti kaybetti: Taşyumruk Vadisi Harabesi Vadisi (terk edildi)
- seed 15, 18. yıl (gün 718): Közsaray Cumhuriyeti başkenti kaybetti: Dumanköprü (Kızılyurt Boyları aldı)
- seed 15, 21. yıl (gün 812): Kızılkül Cumhuriyeti başkenti kaybetti: Kızılkül (Balköprü Teokrasisi aldı)
- seed 15, 27. yıl (gün 1044): Balköprü Teokrasisi başkenti kaybetti: Neşeliova (Kızılyurt Boyları aldı)
- seed 15, 28. yıl (gün 1092): Közsaray Cumhuriyeti başkenti kaybetti: Ulukoru (Yeşilyaprak Krallığı aldı)
- seed 15, 30. yıl (gün 1173): Yeşilyaprak Krallığı başkenti kaybetti: Ulukoru (Közsaray Cumhuriyeti aldı)
- seed 15, 31. yıl (gün 1230): Kızılkül Cumhuriyeti yok oldu
- seed 15, 35. yıl (gün 1387): Kızılyurt Boyları başkenti kaybetti: Dumanköprü (Demirkanat Krallığı aldı)
- seed 16, 2. yıl (gün 51): Kurtoba Boyları başkenti kaybetti: Kartalkaya (Alevgeçit Krallığı aldı)
- seed 16, 7. yıl (gün 268): Altınkapı Cumhuriyeti yok oldu
- seed 16, 21. yıl (gün 825): Kurtoba Boyları başkenti kaybetti: Bozkale (Alevgeçit Krallığı aldı)
- seed 16, 23. yıl (gün 887): Akburç Cumhuriyeti başkenti kaybetti: Meşekale (Kurtoba Boyları aldı)
- seed 16, 23. yıl (gün 900): Alazvadi Cumhuriyeti başkenti kaybetti: Yelköy (Tatlıçayır Krallığı aldı)
- seed 16, 27. yıl (gün 1042): Demiroba Boyları yok oldu
- seed 16, 28. yıl (gün 1106): Tuzyayla Teokrasisi yok oldu
- seed 16, 35. yıl (gün 1385): Kurtoba Boyları başkenti kaybetti: Yeşilkaya (Akburç Cumhuriyeti aldı)
- seed 16, 48. yıl (gün 1909): Tatlıçayır Krallığı başkenti kaybetti: Yelköy (Alazvadi Krallığı aldı)
- seed 16, 54. yıl (gün 2124): Tatlıçayır Krallığı başkenti kaybetti: Yelköy (Alazvadi Krallığı aldı)
- seed 16, 56. yıl (gün 2224): Karageçit Krallığı başkenti kaybetti: Karageçit (Akburç Krallığı aldı)

## Yıllık ayrıntı

Hücre: medyan (p10–p90), 16 dünya. Yıl y = (y−1)·40+1 … y·40. günler. Bütün değerler `report.json` içinde (`metrics`), dünya başına değerler `../runs/f1b-8` altında.

### Medeniyet (1/3)

| Yıl | Yaşayan medeniyet | Yeni medeniyet (yeniden doğan) | Yok olan medeniyet | Başkent kaybı (medeniyet yaşarken) | Çöküş (yok olma + başkent kaybı) | Yaşayan yerleşim |
|---|---|---|---|---|---|---|
| 1 | 6 (5,5–7) | 0 | 0 | 0 (0–0,5) | 0 (0–1) | 79 (75–80) |
| 2 | 6 (6–7) | 0 (0–1) | 0 | 0 (0–1) | 0 (0–1) | 79 (74–81) |
| 3 | 6 (6–7) | 0 | 0 (0–0,5) | 0 | 0 (0–1) | 79 (74–81) |
| 4 | 6 (6–7) | 0 (0–0,5) | 0 | 0 (0–0,5) | 0 (0–1) | 79 (71–81) |
| 5 | 6 (6–7) | 0 | 0 | 0 (0–1) | 0 (0–1) | 78,5 (70,5–81) |
| 6 | 6 (6–7,5) | 0 (0–1) | 0 | 0 (0–0,5) | 0 (0–0,5) | 79,5 (69,5–81,5) |
| 7 | 6 (6–7) | 0 | 0 (0–0,5) | 0 (0–1) | 0 (0–1) | 79,5 (72,5–81) |
| 8 | 6 (6–7) | 0 | 0 (0–0,5) | 0 | 0 (0–1) | 79 (73,5–81,5) |
| 9 | 6 (6–7) | 0 (0–0,5) | 0 | 0 | 0 (0–0,5) | 79,5 (72,5–81) |
| 10 | 6 (6–7) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 79,5 (74–81,5) |
| 11 | 6 (6–7) | 0 | 0 | 0 (0–1) | 0 (0–1) | 79,5 (73–81) |
| 12 | 6,5 (6–7) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 79,5 (73–81,5) |
| 13 | 6 (6–7) | 0 | 0 (0–0,5) | 0 (0–1) | 0 (0–1) | 79 (72–81) |
| 14 | 6 (6–7) | 0 | 0 | 0 (0–1) | 0 (0–1) | 79 (72,5–81,5) |
| 15 | 6 (6–7) | 0 (0–0,5) | 0 | 0 | 0 | 79 (72–81,5) |
| 16 | 6 (6–7) | 0 | 0 | 0 | 0 (0–0,5) | 79 (70–81,5) |
| 17 | 6 (6–7) | 0 | 0 | 0 | 0 | 79 (70–81) |
| 18 | 6 (6–7) | 0 | 0 (0–0,5) | 0 (0–0,5) | 0 (0–1) | 79,5 (69–82) |
| 19 | 6 (5,5–7) | 0 | 0 (0–0,5) | 0 (0–0,5) | 0 (0–1) | 80 (68–81,5) |
| 20 | 6 (6–7) | 0 (0–0,5) | 0 | 0 (0–1) | 0 (0–1) | 79 (68,5–81) |
| 21 | 6 (6–7) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 79 (68–81,5) |
| 22 | 6 (6–7) | 0 (0–0,5) | 0 | 0 | 0 | 79 (69–81) |
| 23 | 6 (6–7) | 0 | 0 | 0 (0–0,5) | 0 (0–1) | 79,5 (68–81) |
| 24 | 6 (6–7) | 0 | 0 | 0 (0–1) | 0 (0–1) | 79 (69–82) |
| 25 | 6 (6–7) | 0 | 0 | 0 | 0 | 80 (70–81) |
| 26 | 6 (6–7) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 78,5 (69–81) |
| 27 | 6 (6–7,5) | 0 (0–0,5) | 0 (0–0,5) | 0 | 0 (0–1) | 79 (71–81) |
| 28 | 6 (6–7,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 78,5 (72,5–81,5) |
| 29 | 6 (6–7,5) | 0 | 0 | 0 | 0 | 78,5 (71,5–81,5) |
| 30 | 6 (6–8) | 0 (0–0,5) | 0 (0–0,5) | 0 (0–0,5) | 0 (0–1) | 79 (73,5–81) |
| 31 | 6 (6–7) | 0 | 0 (0–0,5) | 0 | 0 (0–0,5) | 78,5 (74,5–81) |
| 32 | 6 (6–7) | 0 | 0 | 0 | 0 | 78,5 (73–80,5) |
| 33 | 6 (6–7) | 0 | 0 (0–0,5) | 0 | 0 (0–0,5) | 77,5 (72–81) |
| 34 | 6 (6–7) | 0 | 0 | 0 | 0 | 78 (72,5–80) |
| 35 | 6 (6–7) | 0 | 0 | 0 (0–0,5) | 0 (0–1) | 79 (75–81) |
| 36 | 6 (5,5–7,5) | 0 (0–0,5) | 0 | 0 (0–0,5) | 0 (0–0,5) | 79 (74,5–81) |
| 37 | 6 (5–8) | 0 (0–0,5) | 0 (0–0,5) | 0 | 0 (0–1) | 79 (75–81) |
| 38 | 6 (5,5–7,5) | 0 | 0 | 0 | 0 (0–0,5) | 80 (75,5–81,5) |
| 39 | 6 (5,5–7,5) | 0 | 0 | 0 | 0 | 79 (75–81,5) |
| 40 | 6 (5,5–7,5) | 0 (0–0,5) | 0 (0–0,5) | 0 (0–0,5) | 0 (0–1) | 80 (75–81,5) |
| 41 | 6 (5,5–7,5) | 0 | 0 | 0 | 0 (0–0,5) | 78,5 (76–81) |
| 42 | 6 (6–7) | 0 (0–1) | 0 (0–0,5) | 0 | 0 (0–0,5) | 79 (73,5–80,5) |
| 43 | 6 (6–7) | 0 | 0 | 0 (0–0,5) | 0 (0–1) | 78,5 (72–81,5) |
| 44 | 6,5 (6–7) | 0 (0–0,5) | 0 | 0 | 0 | 78,5 (74–81,5) |
| 45 | 7 (6–7) | 0 (0–0,5) | 0 | 0 | 0 | 78,5 (70,5–81) |
| 46 | 7 (6–7) | 0 | 0 | 0 | 0 | 79 (71,5–81) |
| 47 | 7 (6–7) | 0 | 0 | 0 | 0 | 78,5 (71–81) |
| 48 | 7 (6–7) | 0 | 0 | 0 (0–1) | 0 (0–1) | 78,5 (70,5–81) |
| 49 | 7 (6–7) | 0 | 0 | 0 | 0 (0–0,5) | 78,5 (71–81) |
| 50 | 7 (6–7) | 0 | 0 | 0 | 0 | 80 (71,5–81,5) |
| 51 | 6,5 (6–7) | 0 | 0 | 0 | 0 (0–0,5) | 80 (72–81) |
| 52 | 7 (6–7,5) | 0 (0–0,5) | 0 | 0 | 0 | 80 (73–81,5) |
| 53 | 7 (6–7,5) | 0 | 0 | 0 | 0 | 79 (74–81) |
| 54 | 7 (6–8) | 0 | 0 | 0 | 0 | 79,5 (72–81,5) |
| 55 | 7 (6–8) | 0 (0–0,5) | 0 | 0 | 0 | 79 (71,5–81) |
| 56 | 7 (6–8) | 0 | 0 | 0 (0–1) | 0 (0–1) | 79 (70–80,5) |
| 57 | 7 (6–8) | 0 | 0 | 0 (0–0,5) | 0 (0–1) | 79 (71–81,5) |
| 58 | 7 (6–8) | 0 | 0 | 0 | 0 (0–0,5) | 79 (74,5–81) |
| 59 | 7 (6–8) | 0 (0–0,5) | 0 | 0 (0–1) | 0 (0–1) | 78,5 (75–81) |
| 60 | 7 (6–8) | 0 | 0 | 0 | 0 | 78,5 (73,5–81) |

### Medeniyet (2/3)

| Yıl | Medeniyet başına yerleşim | 5+ kara yerleşimli medeniyet payı | Kurulan yerleşim | Fethedilen yerleşim | Terk edilen yerleşim | Toplam nüfus |
|---|---|---|---|---|---|---|
| 1 | 13,2 (10,9–14,2) | %83 (%64–%100) | 2,5 (1–3) | 2 (2–3,5) | 0 (0–1,5) | 2783 (2560–3014) |
| 2 | 13 (11,3–13,3) | %83 (%67–%93) | 1 (0–2) | 2 (1–3) | 0 (0–2) | 2706 (2510–2944) |
| 3 | 12,9 (11,1–13,4) | %83 (%67–%92) | 1 (0–1,5) | 2 (0–3,5) | 0,5 (0–3) | 2774 (2498–3092) |
| 4 | 12,4 (10,6–13,5) | %83 (%57–%100) | 0,5 (0–3) | 2,5 (1,5–3,5) | 0 (0–5,5) | 2764 (2592–3056) |
| 5 | 13 (10,4–13,4) | %83 (%62–%100) | 0,5 (0–3,5) | 2 (1–4) | 1 (0–4,5) | 2809 (2518–3156) |
| 6 | 12 (10,1–13,4) | %77 (%54–%92) | 1 (0–1,5) | 2 (2–3,5) | 0 (0–1,5) | 2749 (2556–3116) |
| 7 | 12,5 (10,4–13,3) | %77 (%57–%100) | 0 (0–3,5) | 2 (1–3) | 0 (0–1) | 2802 (2593–3113) |
| 8 | 12,8 (10,4–13,6) | %83 (%57–%100) | 0,5 (0–3) | 2 (1–4) | 0 (0–1,5) | 2837 (2634–3204) |
| 9 | 12,5 (10,4–13,5) | %77 (%57–%100) | 0 (0–2,5) | 2 (0,5–4,5) | 0,5 (0–1,5) | 2874 (2530–3294) |
| 10 | 12,6 (10,5–13,6) | %71 (%57–%100) | 0,5 (0–2) | 1 (1–2,5) | 0 (0–1,5) | 2832 (2523–3298) |
| 11 | 12,5 (10,3–13,5) | %71 (%57–%100) | 0 (0–1) | 2 (1–4) | 1 (0–1,5) | 2884 (2601–3260) |
| 12 | 12 (10,2–13,7) | %71 (%57–%100) | 1 (0–2) | 1 (0–4) | 0 (0–4) | 2882 (2566–3208) |
| 13 | 12,8 (10,7–13,7) | %67 (%57–%100) | 0 (0–2,5) | 2 (0–3) | 0 (0–2,5) | 2884 (2573–3256) |
| 14 | 12,8 (10,8–13,4) | %67 (%57–%85) | 0 (0–1,5) | 2 (1–4) | 0 (0–1,5) | 2828 (2759–3391) |
| 15 | 12,1 (10,5–13,4) | %69 (%57–%85) | 0 (0–2) | 2 (1–3) | 0 (0–1,5) | 2908 (2742–3103) |
| 16 | 12 (10,6–13,6) | %67 (%57–%93) | 0,5 (0–2,5) | 2 (0–3,5) | 0,5 (0–1,5) | 2878 (2618–3300) |
| 17 | 12,1 (10,5–13,3) | %67 (%57–%93) | 0 (0–1,5) | 1,5 (0–3,5) | 0 (0–1) | 2920 (2644–3262) |
| 18 | 12,6 (11,3–13,6) | %71 (%57–%93) | 0 (0–2,5) | 2 (1–3) | 0 (0–2,5) | 2848 (2600–3290) |
| 19 | 13,1 (11,1–13,7) | %76 (%62–%83) | 0 (0–1) | 1,5 (0–4) | 0 (0–3) | 2860 (2524–3218) |
| 20 | 12,3 (11,1–13,3) | %67 (%57–%85) | 0,5 (0–3) | 1,5 (0–4) | 1 (0–2,5) | 2908 (2561–3266) |
| 21 | 12,3 (11,2–13,4) | %67 (%57–%83) | 1 (0–2) | 2 (0–4) | 1 (0–2) | 2832 (2560–3426) |
| 22 | 11,6 (10,3–13,4) | %67 (%57–%83) | 0 (0–2) | 2 (0,5–3) | 0 (0–1,5) | 2844 (2530–3440) |
| 23 | 11,6 (10,9–13,5) | %67 (%57–%83) | 0 (0–2) | 2 (0–3,5) | 0 (0–1,5) | 2908 (2576–3402) |
| 24 | 11,7 (10,3–13,5) | %67 (%57–%83) | 0,5 (0–3,5) | 2 (0–3,5) | 0,5 (0–2) | 2909 (2529–3540) |
| 25 | 11,7 (10,6–13,4) | %67 (%57–%83) | 0,5 (0–2) | 1 (0–3) | 0 (0–2) | 2980 (2452–3274) |
| 26 | 11,6 (10,5–13,5) | %67 (%54–%83) | 0 (0–2,5) | 2 (0,5–2,5) | 0 (0–2,5) | 2954 (2442–3262) |
| 27 | 11,9 (10,5–13,4) | %67 (%54–%83) | 0,5 (0–2,5) | 2 (1–4,5) | 1 (0–1) | 3036 (2510–3408) |
| 28 | 12,1 (10,5–13,5) | %69 (%57–%83) | 0 (0–2) | 2 (0–4) | 0 (0–2) | 2979 (2568–3399) |
| 29 | 11,9 (10,5–13,3) | %67 (%57–%83) | 1 (0–2,5) | 1 (0–3,5) | 0,5 (0–5) | 2975 (2514–3318) |
| 30 | 12,3 (10,1–13,3) | %67 (%50–%85) | 1,5 (0–3,5) | 2,5 (0–5) | 1 (0–5,5) | 2986 (2604–3401) |
| 31 | 12,4 (10,7–13,5) | %71 (%54–%93) | 0,5 (0–3,5) | 1 (0–4) | 1 (0–3) | 2954 (2545–3344) |
| 32 | 12,2 (10,7–13,3) | %71 (%54–%85) | 0 (0–1,5) | 2 (0,5–4) | 1 (0–1) | 3008 (2682–3298) |
| 33 | 12,3 (10,6–13,3) | %69 (%57–%85) | 1 (0–3) | 2 (0–2,5) | 1 (0–3) | 2983 (2650–3358) |
| 34 | 12,3 (10,6–13,3) | %69 (%57–%93) | 1 (0–3) | 2 (0–4,5) | 0 (0–3,5) | 2874 (2548–3300) |
| 35 | 12,7 (11,3–13,3) | %69 (%59–%93) | 1,5 (0–3,5) | 2,5 (0,5–4) | 0 (0–1,5) | 3006 (2595–3408) |
| 36 | 12,7 (10,7–13,5) | %69 (%59–%100) | 0,5 (0–2) | 2 (0,5–3,5) | 0 (0–2) | 2968 (2519–3508) |
| 37 | 12,6 (10,1–14,7) | %83 (%59–%94) | 1 (0–1) | 2 (0,5–4) | 0 (0–1,5) | 3076 (2488–3440) |
| 38 | 12,6 (10,6–13,6) | %82 (%59–%87) | 0 (0–1) | 2 (1–3) | 0 (0–1) | 2970 (2412–3454) |
| 39 | 12,5 (10,6–13,6) | %78 (%59–%86) | 0 (0–1,5) | 2 (0–4,5) | 0 (0–1,5) | 2941 (2474–3386) |
| 40 | 12,6 (10,7–13,5) | %78 (%63–%86) | 0 (0–1) | 2 (0–5,5) | 0 (0–1) | 2927 (2436–3386) |
| 41 | 12,7 (10,7–13,5) | %73 (%63–%86) | 0 (0–1,5) | 1 (0–3) | 0 (0–1,5) | 2968 (2428–3298) |
| 42 | 11,9 (10,9–13,3) | %67 (%59–%93) | 0 (0–1,5) | 2 (0,5–4,5) | 0,5 (0–3,5) | 2999 (2310–3367) |
| 43 | 11,9 (10,9–13,3) | %69 (%60–%93) | 0,5 (0–3) | 2 (0–3) | 0 (0–3,5) | 2824 (2354–3369) |
| 44 | 11,8 (10,6–13,2) | %71 (%57–%92) | 0 (0–3) | 2 (0–4) | 0 (0–1,5) | 2968 (2356–3436) |
| 45 | 11,6 (10,1–13,3) | %71 (%56–%85) | 0 (0–2,5) | 1,5 (0–4) | 0 (0–2) | 2920 (2376–3422) |
| 46 | 11,4 (10,2–13,3) | %71 (%51–%83) | 0 (0–3) | 1 (0–4) | 0 (0–1,5) | 2808 (2436–3380) |
| 47 | 11,4 (10,1–13,1) | %71 (%51–%85) | 0,5 (0–2) | 1,5 (0,5–3,5) | 0,5 (0–2,5) | 3020 (2550–3478) |
| 48 | 11,5 (10,9–13,2) | %71 (%54–%85) | 0 (0–2,5) | 1 (0–4) | 0 (0–3) | 2899 (2428–3522) |
| 49 | 11,5 (10,9–13,2) | %71 (%57–%86) | 0,5 (0–3) | 2 (0–3,5) | 0,5 (0–1) | 3024 (2520–3454) |
| 50 | 11,6 (10,7–13,3) | %71 (%57–%85) | 0 (0–2) | 1 (0–2) | 0,5 (0–1) | 3054 (2427–3564) |
| 51 | 11,6 (10,6–13,5) | %76 (%57–%85) | 1 (0–2) | 1 (0–3,5) | 0 (0–2) | 3015 (2530–3428) |
| 52 | 11,6 (10,2–13,4) | %71 (%62–%85) | 0,5 (0–2) | 2 (0,5–4) | 0 (0–2) | 3020 (2493–3458) |
| 53 | 11,6 (10,2–13,4) | %73 (%57–%86) | 0 (0–3,5) | 1 (0–3) | 0 (0–1,5) | 3006 (2438–3311) |
| 54 | 11,7 (9,56–13,3) | %71 (%60–%85) | 0 (0–3) | 1 (0,5–3) | 0,5 (0–3) | 3044 (2514–3316) |
| 55 | 11,5 (9,69–13,3) | %69 (%57–%85) | 0 (0–1,5) | 1,5 (0,5–2,5) | 0 (0–1,5) | 3000 (2444–3540) |
| 56 | 11,4 (9,63–13,1) | %73 (%54–%86) | 0 (0–1) | 1,5 (0–4,5) | 0 (0–2) | 3004 (2514–3376) |
| 57 | 11,5 (9,63–13,1) | %71 (%57–%85) | 0,5 (0–1,5) | 2 (0,5–4,5) | 0 (0–1,5) | 2930 (2387–3343) |
| 58 | 11,5 (10–13,1) | %71 (%57–%86) | 0 (0–1) | 2 (1–4) | 0 (0–1) | 3032 (2596–3562) |
| 59 | 11,4 (9,75–13,3) | %76 (%54–%86) | 0 (0–2) | 2 (1–3) | 0,5 (0–3,5) | 2998 (2466–3518) |
| 60 | 11,2 (9,69–13,3) | %69 (%50–%85) | 0 (0–1,5) | 2 (0–4) | 0 (0–2) | 2962 (2467–3520) |

### Medeniyet (3/3)

| Yıl | Altın medyanı (medeniyetler) | Boştaki iş gücü payı | Bölünme (ayrılıp kurulan medeniyet) | En büyük medeniyetin yerleşimi |
|---|---|---|---|---|
| 1 | 373 (138–614) | %14 (%10–%30) | 0 | 21 (17,5–24) |
| 2 | 316 (55,3–585) | %16 (%11–%31) | 0 (0–0,5) | 21,5 (18,5–24,5) |
| 3 | 434 (92–614) | %17 (%12–%27) | 0 | 21 (18,5–25) |
| 4 | 343 (157–487) | %17 (%12–%26) | 0 (0–0,5) | 21,5 (18,5–26) |
| 5 | 318 (117–510) | %16 (%12–%27) | 0 | 21 (17,5–26) |
| 6 | 332 (161–526) | %16 (%11–%27) | 0 (0–1) | 22 (18,5–27) |
| 7 | 331 (179–567) | %15 (%12–%26) | 0 | 22 (18–24,5) |
| 8 | 309 (228–761) | %15 (%11–%28) | 0 | 22 (18,5–24,5) |
| 9 | 375 (150–602) | %16 (%13–%29) | 0 (0–0,5) | 22 (17,5–25) |
| 10 | 338 (160–698) | %15 (%12–%29) | 0 | 22 (18,5–25) |
| 11 | 386 (79,2–657) | %16 (%10–%30) | 0 | 22 (17,5–26,5) |
| 12 | 212 (54,5–630) | %15 (%9,9–%33) | 0 | 22,5 (17,5–27) |
| 13 | 368 (81,8–544) | %18 (%9,5–%33) | 0 | 23 (18–27) |
| 14 | 315 (109–702) | %15 (%8,6–%37) | 0 | 23,5 (19–28) |
| 15 | 347 (98,5–643) | %17 (%11–%35) | 0 (0–0,5) | 23 (19,5–28) |
| 16 | 258 (66,3–632) | %19 (%13–%34) | 0 | 23 (19–28) |
| 17 | 439 (64,2–684) | %20 (%12–%30) | 0 | 23,5 (19,5–28) |
| 18 | 454 (97,6–588) | %18 (%11–%27) | 0 | 23,5 (19–28,5) |
| 19 | 399 (202–668) | %19 (%10–%29) | 0 | 24,5 (19–31) |
| 20 | 372 (160–598) | %16 (%10–%30) | 0 (0–0,5) | 24 (19,5–29) |
| 21 | 236 (73,8–737) | %21 (%11–%35) | 0 | 23 (19–30,5) |
| 22 | 305 (106–573) | %21 (%12–%33) | 0 (0–0,5) | 24,5 (20–29,5) |
| 23 | 413 (81,4–542) | %19 (%10–%32) | 0 | 24 (20–29,5) |
| 24 | 317 (60,6–683) | %20 (%11–%34) | 0 | 25 (20,5–30,5) |
| 25 | 327 (93,2–710) | %18 (%12–%40) | 0 | 25,5 (20–31) |
| 26 | 365 (43,2–683) | %18 (%11–%43) | 0 | 25,5 (20–31) |
| 27 | 359 (109–637) | %21 (%11–%46) | 0 (0–0,5) | 26 (20–32) |
| 28 | 431 (64,6–629) | %21 (%10–%48) | 0 | 26 (19,5–31,5) |
| 29 | 337 (107–740) | %21 (%10–%48) | 0 | 25,5 (20–31) |
| 30 | 430 (149–701) | %22 (%10–%47) | 0 (0–0,5) | 25,5 (19,5–31,5) |
| 31 | 323 (112–605) | %23 (%13–%46) | 0 | 25 (20–32) |
| 32 | 344 (137–541) | %21 (%12–%44) | 0 | 24,5 (19–32) |
| 33 | 395 (175–727) | %22 (%14–%43) | 0 | 24,5 (18,5–32) |
| 34 | 360 (161–749) | %25 (%14–%43) | 0 | 26 (18–30,5) |
| 35 | 363 (84,3–626) | %24 (%14–%45) | 0 | 25,5 (17–29,5) |
| 36 | 361 (31–692) | %24 (%11–%44) | 0 (0–0,5) | 26,5 (17–32) |
| 37 | 315 (91,6–595) | %23 (%13–%40) | 0 (0–0,5) | 25 (18–31,5) |
| 38 | 327 (69,4–741) | %22 (%13–%39) | 0 | 25,5 (17–31) |
| 39 | 458 (136–804) | %22 (%11–%38) | 0 | 25 (18–31) |
| 40 | 485 (112–786) | %24 (%11–%41) | 0 (0–0,5) | 24 (18–32) |
| 41 | 445 (140–728) | %24 (%11–%42) | 0 | 24,5 (18–32) |
| 42 | 368 (154–451) | %24 (%12–%42) | 0 (0–1) | 24 (18,5–32) |
| 43 | 340 (111–699) | %24 (%9,8–%42) | 0 | 24 (18,5–33) |
| 44 | 414 (126–662) | %22 (%9,1–%42) | 0 (0–0,5) | 24,5 (19–33) |
| 45 | 347 (199–550) | %19 (%9,7–%41) | 0 (0–0,5) | 24,5 (18,5–33,5) |
| 46 | 403 (147–611) | %22 (%8,3–%40) | 0 | 24,5 (19,5–34) |
| 47 | 396 (126–666) | %23 (%8,5–%38) | 0 | 25 (19–34) |
| 48 | 304 (109–638) | %25 (%9,7–%41) | 0 | 24,5 (20–33,5) |
| 49 | 365 (139–575) | %22 (%10–%42) | 0 | 26 (19–33,5) |
| 50 | 374 (157–559) | %24 (%13–%45) | 0 | 25,5 (19–33) |
| 51 | 321 (125–484) | %24 (%12–%45) | 0 | 26 (18,5–33,5) |
| 52 | 392 (98,8–672) | %25 (%16–%45) | 0 (0–0,5) | 26,5 (18–33,5) |
| 53 | 381 (76,5–640) | %28 (%15–%47) | 0 | 26,5 (18–33,5) |
| 54 | 379 (72,1–699) | %25 (%15–%44) | 0 | 27 (18–32) |
| 55 | 302 (95,1–762) | %27 (%16–%43) | 0 (0–0,5) | 26,5 (18,5–32,5) |
| 56 | 234 (125–800) | %24 (%15–%39) | 0 | 25,5 (18,5–32) |
| 57 | 301 (125–533) | %23 (%17–%39) | 0 | 26 (19–32) |
| 58 | 292 (129–671) | %27 (%18–%37) | 0 | 25,5 (18–33) |
| 59 | 366 (133–601) | %25 (%13–%35) | 0 (0–0,5) | 26,5 (18,5–33,5) |
| 60 | 389 (65,3–718) | %26 (%13–%38) | 0 | 26,5 (18,5–32) |

### Olaylar

| Yıl | Olay | Büyük olay |
|---|---|---|
| 1 | 1150 (927–1470) | 212 (149–274) |
| 2 | 1209 (1042–1450) | 219 (154–336) |
| 3 | 1218 (1013–1415) | 200 (155–282) |
| 4 | 1190 (1010–1454) | 220 (169–292) |
| 5 | 1222 (1070–1514) | 216 (178–282) |
| 6 | 1296 (1126–1458) | 221 (172–299) |
| 7 | 1323 (1012–1506) | 227 (177–302) |
| 8 | 1279 (1026–1541) | 232 (148–286) |
| 9 | 1234 (1034–1620) | 214 (152–300) |
| 10 | 1315 (1058–1541) | 216 (142–285) |
| 11 | 1342 (1023–1516) | 224 (188–272) |
| 12 | 1290 (1054–1542) | 220 (179–296) |
| 13 | 1314 (1014–1420) | 210 (175–268) |
| 14 | 1280 (1044–1552) | 210 (162–271) |
| 15 | 1258 (1048–1444) | 224 (172–268) |
| 16 | 1314 (1048–1486) | 248 (186–275) |
| 17 | 1286 (1036–1465) | 230 (156–290) |
| 18 | 1289 (1092–1512) | 224 (166–278) |
| 19 | 1317 (1064–1520) | 226 (154–304) |
| 20 | 1323 (1054–1506) | 232 (184–310) |
| 21 | 1357 (1136–1572) | 236 (158–339) |
| 22 | 1342 (1182–1554) | 234 (171–298) |
| 23 | 1308 (1134–1525) | 229 (182–256) |
| 24 | 1331 (1166–1547) | 242 (190–328) |
| 25 | 1341 (1078–1538) | 248 (193–317) |
| 26 | 1379 (1140–1622) | 236 (168–328) |
| 27 | 1420 (1051–1644) | 217 (162–294) |
| 28 | 1367 (1186–1728) | 236 (168–358) |
| 29 | 1389 (1127–1695) | 234 (182–342) |
| 30 | 1400 (1138–1704) | 244 (177–335) |
| 31 | 1402 (1080–1664) | 249 (174–330) |
| 32 | 1394 (1054–1682) | 252 (176–304) |
| 33 | 1414 (1022–1718) | 250 (179–320) |
| 34 | 1438 (1070–1714) | 268 (190–316) |
| 35 | 1436 (1117–1671) | 256 (178–318) |
| 36 | 1484 (1186–1610) | 260 (196–320) |
| 37 | 1512 (1181–1672) | 267 (182–338) |
| 38 | 1502 (1172–1654) | 276 (187–325) |
| 39 | 1469 (1056–1674) | 242 (160–290) |
| 40 | 1516 (1066–1645) | 276 (172–326) |
| 41 | 1474 (1102–1565) | 239 (165–320) |
| 42 | 1490 (1149–1639) | 250 (158–323) |
| 43 | 1508 (1273–1720) | 257 (176–322) |
| 44 | 1528 (1268–1702) | 256 (187–328) |
| 45 | 1499 (1221–1767) | 278 (174–344) |
| 46 | 1532 (1158–1804) | 254 (188–320) |
| 47 | 1535 (1254–1743) | 254 (207–331) |
| 48 | 1522 (1189–1743) | 254 (188–322) |
| 49 | 1553 (1170–1696) | 262 (164–336) |
| 50 | 1479 (1106–1684) | 247 (190–335) |
| 51 | 1470 (1094–1712) | 234 (146–356) |
| 52 | 1513 (1156–1755) | 267 (158–359) |
| 53 | 1521 (1202–1699) | 272 (191–342) |
| 54 | 1554 (1128–1739) | 254 (168–331) |
| 55 | 1544 (1218–1749) | 288 (174–355) |
| 56 | 1598 (1218–1722) | 264 (150–364) |
| 57 | 1607 (1222–1784) | 288 (218–323) |
| 58 | 1610 (1180–1714) | 246 (139–326) |
| 59 | 1590 (1228–1783) | 292 (170–366) |
| 60 | 1604 (1200–1763) | 270 (152–339) |

### Savaş (1/2)

| Yıl | Muharebe | Başlayan savaş | Süren savaş (yıl sonu) | Yıl içinde süren savaş | Yağma akını (medeniyet) | Tarihî hak savaşı |
|---|---|---|---|---|---|---|
| 1 | 18 (10,5–25,5) | 3 (2–4) | 1 (0–2) | 4 (2,5–5,5) | 1 (0–4,5) | 0,5 (0–1,5) |
| 2 | 19 (12,5–25,5) | 3 (2–4,5) | 2 (0–3) | 4 (2,5–5) | 0 (0–2) | 0 (0–1) |
| 3 | 17 (14–25) | 2 (1–4) | 1 (0–2) | 4 (1,5–5,5) | 0,5 (0–3) | 0 (0–1,5) |
| 4 | 20,5 (11,5–24) | 2,5 (2–5) | 1 (0–3) | 4 (2,5–6) | 1 (0–3,5) | 0 (0–1) |
| 5 | 19,5 (12,5–24,5) | 3 (1–5) | 1 (0,5–2) | 4 (1,5–6) | 1 (0–3) | 1 (0–1,5) |
| 6 | 18,5 (12,5–24,5) | 3 (1–4) | 1 (0,5–2,5) | 4 (2,5–5) | 1 (0–2,5) | 0,5 (0–1) |
| 7 | 20 (13–26) | 2,5 (1,5–3,5) | 1 (0–2) | 4 (2,5–5,5) | 1 (0–4,5) | 1 (0–1) |
| 8 | 20,5 (10,5–26) | 2 (1,5–4,5) | 1 (0–2) | 3,5 (2–5,5) | 1,5 (0–5) | 0,5 (0–1) |
| 9 | 17,5 (12,5–24) | 2,5 (0,5–4,5) | 1 (0,5–2) | 4 (2–5,5) | 0,5 (0–3,5) | 0 (0–1) |
| 10 | 20 (13–24) | 2 (0,5–4) | 2 (0–3,5) | 3,5 (1,5–5) | 0 (0–5) | 0 (0–1) |
| 11 | 18,5 (13–25) | 3 (0–4) | 1 (0–2) | 4 (2–6,5) | 0,5 (0–4) | 0 (0–1) |
| 12 | 18 (11–27,5) | 2 (0,5–4) | 2 (0–3) | 3 (2,5–5) | 1,5 (0–3,5) | 0 (0–1) |
| 13 | 20 (13,5–23,5) | 2 (0,5–3,5) | 1 (0–3) | 4 (1,5–5,5) | 2 (0–5,5) | 0 (0–1) |
| 14 | 18 (9–21,5) | 2,5 (0,5–4) | 1 (0–3) | 4 (2–5) | 1 (0–4,5) | 0 (0–1,5) |
| 15 | 18,5 (14–23,5) | 3 (0,5–4) | 1 (0–3) | 4 (2,5–5,5) | 0,5 (0–5,5) | 0,5 (0–1,5) |
| 16 | 20 (14–27) | 2,5 (1,5–4) | 1 (0–3) | 4 (2–5) | 1 (0–5,5) | 0 (0–1) |
| 17 | 19,5 (15,5–25) | 2 (1–3,5) | 1 (0–2,5) | 4 (2–5) | 0 (0–3) | 0 (0–1) |
| 18 | 16,5 (10,5–25) | 2,5 (1–3,5) | 1 (0–3) | 3,5 (2,5–5) | 0,5 (0–4) | 1 (0–1) |
| 19 | 19 (13,5–24,5) | 3 (0,5–4) | 1 (0–2,5) | 3,5 (1,5–6,5) | 0,5 (0–3) | 0 (0–1) |
| 20 | 17,5 (13–23,5) | 1,5 (0–3,5) | 1 (0–2,5) | 3 (1–5) | 1 (0–4) | 0 (0–1,5) |
| 21 | 19,5 (10,5–28,5) | 2 (0–4) | 0,5 (0–2,5) | 3 (1–5) | 0 (0–3,5) | 0 (0–1) |
| 22 | 18 (13,5–24,5) | 2,5 (0–5) | 1 (0–3) | 3,5 (2–5) | 1,5 (0–4,5) | 0 (0–1) |
| 23 | 19,5 (11,5–24,5) | 2 (0–3,5) | 1,5 (0–3) | 3 (1–6) | 1 (0–5) | 0 (0–1) |
| 24 | 19,5 (13–29) | 2 (0–2) | 1 (0–2) | 3 (1,5–5) | 2 (0–5) | 0 (0–1) |
| 25 | 18,5 (12–26,5) | 2 (0,5–4) | 2 (0–2,5) | 4 (1–5) | 0,5 (0–4) | 0 (0–1) |
| 26 | 18 (15–25) | 2 (0,5–3) | 1 (0,5–2,5) | 4 (2–5) | 1,5 (0–4) | 0 (0–1,5) |
| 27 | 19 (12,5–26,5) | 2,5 (0,5–3,5) | 1 (0–2) | 4 (1,5–5,5) | 1 (0–6) | 0 (0–1,5) |
| 28 | 18,5 (11–26,5) | 2 (0,5–3,5) | 1 (0,5–2) | 3,5 (2–5) | 1,5 (0–4,5) | 0 (0–1) |
| 29 | 19,5 (12–30,5) | 1,5 (0–3,5) | 1 (0–2) | 3 (1–5) | 1 (0–5,5) | 0 |
| 30 | 20,5 (14–30) | 3 (1–4,5) | 1 (0–2) | 4 (1–5) | 1,5 (0–5,5) | 0 (0–1) |
| 31 | 21 (13–28,5) | 2 (1–4) | 1 (0–2) | 2 (1,5–6,5) | 1 (0–6,5) | 0 (0–1) |
| 32 | 19,5 (15,5–29,5) | 2,5 (0,5–4,5) | 1 (0–3) | 3,5 (2–6) | 1,5 (0–7) | 0 (0–1) |
| 33 | 20,5 (12–30) | 2 (1–4,5) | 2 (1–3,5) | 4,5 (1,5–5,5) | 1,5 (0–7) | 0,5 (0–1) |
| 34 | 22 (14–31) | 2,5 (0,5–3,5) | 1 (0–2) | 4 (2–6) | 0 (0–5,5) | 0 (0–1) |
| 35 | 19 (16–27,5) | 2 (0–4,5) | 1 (0–2) | 3 (1–5,5) | 1 (0–8,5) | 0 (0–1) |
| 36 | 18,5 (15–29) | 2,5 (1–5) | 1 (0–2,5) | 3,5 (1–6) | 2 (0–5) | 0 (0–1) |
| 37 | 21 (15–27,5) | 2 (0–3,5) | 0,5 (0–2) | 4 (0,5–6) | 1 (0–5,5) | 0 (0–0,5) |
| 38 | 19 (16,5–27,5) | 2,5 (1–4) | 1 (0–2,5) | 3 (1,5–4,5) | 1 (0–5,5) | 0 (0–1) |
| 39 | 17 (13–26,5) | 2 (1–4) | 1 (0–3) | 3 (2–6) | 1 (0–4) | 0 (0–1) |
| 40 | 18,5 (13–25) | 2 (0–3,5) | 0,5 (0–2) | 3 (1,5–5,5) | 1 (0–3,5) | 0 (0–1,5) |
| 41 | 18,5 (13,5–26,5) | 2 (0–4) | 1 (0–2,5) | 3 (1–5) | 1 (0–3,5) | 0 (0–1) |
| 42 | 16,5 (12,5–26,5) | 1,5 (0–4,5) | 1 (0–2) | 2,5 (1–6) | 1 (0–4,5) | 0 (0–1) |
| 43 | 20,5 (13–27,5) | 2,5 (0,5–5) | 1 (0–3) | 3,5 (1–5,5) | 1,5 (0–3,5) | 0 (0–2) |
| 44 | 19,5 (12,5–28,5) | 2 (1–3) | 1 (0–2) | 4 (1,5–5) | 1,5 (0–5,5) | 0 (0–1) |
| 45 | 19,5 (11–25,5) | 1,5 (0,5–4) | 1 (0–3) | 3 (2–5,5) | 1,5 (0–7) | 0 (0–1) |
| 46 | 19,5 (11,5–26) | 2 (0,5–3) | 1,5 (0,5–2,5) | 3 (2–5) | 1,5 (0–5,5) | 0 (0–0,5) |
| 47 | 17,5 (13–27,5) | 2 (0–3,5) | 1 (0–2) | 3 (2–5) | 0,5 (0–7,5) | 0 (0–1) |
| 48 | 18 (14,5–22) | 3 (1–4,5) | 1,5 (0–2,5) | 3,5 (1,5–6) | 1,5 (0–5,5) | 0 (0–1) |
| 49 | 20 (14–24) | 1 (0–4,5) | 1 (0–2) | 3 (1,5–5,5) | 1 (0–7) | 0 (0–1) |
| 50 | 19,5 (16,5–25,5) | 2 (0,5–4) | 1,5 (0–3) | 3 (1–6) | 1,5 (0–5,5) | 0 (0–1,5) |
| 51 | 20 (10,5–29,5) | 2 (0,5–3) | 1 (0–2) | 3 (1,5–6) | 2 (0–6) | 0 (0–1) |
| 52 | 21 (13,5–30) | 2 (0–4) | 1 (0–2,5) | 3 (1,5–5,5) | 1 (0–7,5) | 0 |
| 53 | 18,5 (14–30,5) | 2 (0–3,5) | 1 (0–2) | 3 (1–5,5) | 0,5 (0–6) | 0 (0–1) |
| 54 | 19,5 (12,5–27) | 2 (1–3,5) | 1 (0–2,5) | 4 (2–5) | 0 (0–6) | 0 (0–0,5) |
| 55 | 21 (11–30,5) | 2,5 (1–4) | 1 (0–3) | 4 (1,5–5,5) | 0,5 (0–5) | 0 (0–0,5) |
| 56 | 19,5 (8,5–30) | 2,5 (1–4,5) | 1 (1–3) | 4,5 (1–6) | 1,5 (0–6) | 0 (0–1) |
| 57 | 19 (14–30,5) | 1,5 (1–4,5) | 1 (0–2) | 3 (2–6) | 1 (0–6) | 0 (0–1) |
| 58 | 20,5 (7,5–28) | 2,5 (1–5) | 2 (0–2) | 3,5 (2–6,5) | 1,5 (0–9) | 0 (0–1) |
| 59 | 18,5 (12,5–27,5) | 3 (0–5) | 1 (0–3) | 4 (2–6,5) | 2 (0–6,5) | 0 (0–1,5) |
| 60 | 18 (9,5–26) | 2 (1–4,5) | 1 (0–2) | 3,5 (1–6) | 1 (0–5,5) | 0 (0–1) |

### Savaş (2/2)

| Yıl | Pakt gereği savaş | Kutsal Sefer çağrısı | İhanet (pakt çiğnendi) | Savunma paktı (yıl sonu) |
|---|---|---|---|---|
| 1 | 0 | 0 (0–0,5) | 0 | 0 (0–1) |
| 2 | 0 (0–1) | 0 (0–1) | 0 | 0 (0–1,5) |
| 3 | 0 | 0 (0–0,5) | 0 (0–0,5) | 0 (0–2) |
| 4 | 0 (0–1,5) | 0 | 0 | 0 (0–2) |
| 5 | 0 | 0 | 0 | 0 (0–1,5) |
| 6 | 0 (0–1) | 0 | 0 | 0 (0–1,5) |
| 7 | 0 (0–1) | 0 (0–0,5) | 0 | 1 (0–2) |
| 8 | 0 (0–0,5) | 0 (0–1) | 0 | 0 (0–2) |
| 9 | 0 (0–1) | 0 (0–0,5) | 0 | 0,5 (0–1,5) |
| 10 | 0 (0–1,5) | 0 | 0 | 1 (0–1,5) |
| 11 | 0 (0–1) | 0 | 0 (0–0,5) | 0 (0–1,5) |
| 12 | 0 | 0 (0–0,5) | 0 | 0 (0–1) |
| 13 | 0 | 0 | 0 | 0 (0–1) |
| 14 | 0 (0–0,5) | 0 (0–1) | 0 | 0,5 (0–1) |
| 15 | 0 (0–0,5) | 0 (0–1) | 0 (0–0,5) | 0 (0–1,5) |
| 16 | 0 (0–0,5) | 0 | 0 | 0 (0–1,5) |
| 17 | 0 (0–1) | 0 (0–0,5) | 0 | 0,5 (0–1,5) |
| 18 | 0 (0–1,5) | 0 | 0 | 1 (0–1,5) |
| 19 | 0 (0–1) | 0 | 0 | 0 (0–1,5) |
| 20 | 0 (0–0,5) | 0 | 0 | 0 (0–1,5) |
| 21 | 0 (0–1) | 0 | 0 | 0 (0–1) |
| 22 | 0 (0–0,5) | 0 | 0 | 0 (0–1) |
| 23 | 0 (0–1,5) | 0 | 0 | 0 (0–1) |
| 24 | 0 | 0 | 0 | 0 (0–1,5) |
| 25 | 0 (0–0,5) | 0 (0–0,5) | 0 | 0 (0–1,5) |
| 26 | 0 (0–0,5) | 0 | 0 | 0 (0–1) |
| 27 | 0 (0–1) | 0 (0–0,5) | 0 | 0 (0–1,5) |
| 28 | 0 (0–1) | 0 | 0 | 0 (0–2) |
| 29 | 0 (0–0,5) | 0 | 0 | 0 (0–1,5) |
| 30 | 0 (0–1) | 0 | 0 | 0 (0–1) |
| 31 | 0 (0–0,5) | 0 | 0 | 0 (0–1) |
| 32 | 0 | 0 (0–0,5) | 0 | 0 (0–1) |
| 33 | 0 (0–0,5) | 0 (0–1) | 0 | 0 (0–1) |
| 34 | 0 | 0 | 0 | 0 (0–1) |
| 35 | 0 | 0 (0–1) | 0 | 0 (0–1) |
| 36 | 0 (0–0,5) | 0 (0–0,5) | 0 | 0 (0–1) |
| 37 | 0 (0–0,5) | 0 | 0 | 0 (0–1) |
| 38 | 0 (0–1) | 0 | 0 | 0 (0–1) |
| 39 | 0 | 0 (0–0,5) | 0 | 0 (0–1) |
| 40 | 0 | 0 | 0 | 0,5 (0–1) |
| 41 | 0 (0–0,5) | 0 | 0 | 0 (0–1) |
| 42 | 0 (0–0,5) | 0 (0–0,5) | 0 | 0 (0–1) |
| 43 | 0 (0–0,5) | 0 (0–0,5) | 0 | 0 (0–1) |
| 44 | 0 | 0 | 0 | 0 (0–1) |
| 45 | 0 | 0 (0–0,5) | 0 | 0 (0–1) |
| 46 | 0 | 0 | 0 | 0 (0–1) |
| 47 | 0 | 0 | 0 | 0 (0–1) |
| 48 | 0 (0–0,5) | 0 (0–1) | 0 | 0 (0–1) |
| 49 | 0 (0–0,5) | 0 | 0 | 0 (0–1) |
| 50 | 0 | 0 (0–0,5) | 0 | 0 (0–1) |
| 51 | 0 | 0 (0–0,5) | 0 | 0 (0–1) |
| 52 | 0 | 0 (0–0,5) | 0 | 0 (0–1) |
| 53 | 0 (0–1) | 0 | 0 | 0 (0–1) |
| 54 | 0 | 0 (0–0,5) | 0 | 0 (0–1) |
| 55 | 0 | 0 (0–0,5) | 0 | 0 (0–1) |
| 56 | 0 (0–1) | 0 | 0 | 0 (0–1) |
| 57 | 0 (0–0,5) | 0 | 0 | 0,5 (0–1) |
| 58 | 0 | 0 (0–1) | 0 | 0,5 (0–1) |
| 59 | 0 (0–0,5) | 0 (0–1) | 0 (0–0,5) | 0 (0–1) |
| 60 | 0 (0–1) | 0 (0–1) | 0 | 0 (0–1) |

### Canavarlar (1/2)

| Yıl | Yaşayan kamp (yıl sonu) | Yaşayan kamp (yıl ort.) | Doğan kamp | Temizlenen kamp | Canavar baskını | Yaşayan trol ini (yıl sonu) |
|---|---|---|---|---|---|---|
| 1 | 9 (8–10) | 8,71 (7,94–9,45) | 5 (1,5–9) | 5 (1–8,5) | 4 (1,5–6) | 2,5 (1,5–3) |
| 2 | 9 (7,5–10) | 8,49 (8,13–9,68) | 6 (3–11,5) | 5,5 (3,5–10) | 4 (1,5–6) | 2 (1–3) |
| 3 | 8 (8–10) | 8,36 (7,79–9,24) | 5,5 (3–9,5) | 6,5 (3–9,5) | 3 (2–7) | 2 (1–3) |
| 4 | 8 (8–10) | 8,3 (7,71–9,6) | 6 (3–11) | 6 (3–10,5) | 3,5 (0,5–7) | 3 (1,5–3) |
| 5 | 8 (7–9) | 8,16 (7,71–9,35) | 5 (2,5–10,5) | 6,5 (3–12) | 4 (1–6) | 2 (1–3) |
| 6 | 8 (7–9,5) | 8,13 (7,76–9,38) | 5,5 (1,5–11) | 5 (1,5–10) | 3 (2–5,5) | 2,5 (2–3) |
| 7 | 9 (8–10) | 8,63 (7,74–9,24) | 5,5 (2,5–11,5) | 5,5 (2–10,5) | 3,5 (2–5) | 2,5 (1–3) |
| 8 | 8,5 (7,5–10) | 8,18 (7,78–9,7) | 6 (2–11) | 7 (1–11) | 3 (1–6) | 2 (1–3) |
| 9 | 9 (7,5–9,5) | 8,58 (8,06–9,6) | 6,5 (1,5–10,5) | 5,5 (2–10) | 4 (2–5) | 2 (2–3) |
| 10 | 9 (8–10) | 8,78 (7,76–9,41) | 6,5 (2–10,5) | 7,5 (2–10) | 4 (1,5–6) | 2 (1–3) |
| 11 | 8 (7,5–10) | 8,5 (7,69–9,76) | 6,5 (1,5–10) | 6,5 (1,5–10) | 4 (1–6) | 2 (0,5–3) |
| 12 | 8 (7–10) | 8,3 (7,74–9,65) | 7 (2,5–9,5) | 6 (2–10) | 3,5 (0,5–6) | 2 (1–3) |
| 13 | 8 (7–10) | 8,6 (7,5–9,55) | 6 (2–9,5) | 5,5 (2–10) | 3,5 (2–5) | 2,5 (1–3) |
| 14 | 9 (8–9,5) | 8,43 (7,93–9,4) | 6 (2,5–11,5) | 6,5 (2,5–10,5) | 3,5 (1–7) | 2 (1–3) |
| 15 | 9,5 (8–10,5) | 9,09 (7,86–10,1) | 5 (3–10) | 4,5 (2–10) | 4 (2–7) | 2 (0,5–3) |
| 16 | 9 (8–10,5) | 9,1 (8,48–10,6) | 5 (3–10,5) | 5 (3–10,5) | 5 (2,5–6) | 2 (1–3) |
| 17 | 9 (7–10,5) | 9,28 (7,83–10,4) | 5,5 (3,5–10) | 6 (3–11) | 4 (2–7,5) | 2 (1–3) |
| 18 | 9 (8–10) | 9,41 (7,75–10,1) | 5 (1–10) | 5,5 (1,5–9,5) | 4 (2–5) | 2,5 (1,5–3) |
| 19 | 8,5 (8–10,5) | 9,08 (7,96–9,58) | 4 (2,5–9) | 5 (2–11) | 4 (1,5–6) | 2 (1–3) |
| 20 | 9 (8–10,5) | 8,95 (7,79–10,4) | 6 (2–10,5) | 6,5 (1,5–9,5) | 4 (2–6) | 2 (1,5–3) |
| 21 | 9 (8–11) | 9,16 (7,93–9,96) | 5 (1,5–9,5) | 5,5 (1,5–9,5) | 4,5 (2–7) | 3 (1,5–3) |
| 22 | 9 (8–10) | 9,18 (7,85–10,2) | 7 (0,5–9) | 7 (1–11) | 4 (0,5–7) | 2 (1,5–3) |
| 23 | 9 (7,5–10) | 8,89 (7,75–9,96) | 5 (1–8,5) | 5 (1–10) | 4 (2–6,5) | 3 (2–3) |
| 24 | 9 (8–10) | 9,21 (7,88–10,1) | 5 (1,5–10) | 5,5 (2–9,5) | 5 (2–7) | 3 (1,5–3) |
| 25 | 9 (8–11) | 9,16 (8,28–10,1) | 6 (2–11) | 4,5 (2–10,5) | 3 (2,5–5,5) | 3 (2–3) |
| 26 | 9 (7,5–11) | 9,19 (8,28–10,6) | 6 (1,5–9,5) | 5 (2–10,5) | 4,5 (2–6) | 3 (2–3) |
| 27 | 9 (8–10) | 9,36 (8,16–9,83) | 5 (1,5–10,5) | 5,5 (2–9,5) | 4 (1,5–7) | 3 (2–3) |
| 28 | 8 (7–10,5) | 8,81 (8,21–9,85) | 5 (1,5–10,5) | 4,5 (2,5–11) | 4 (1–6) | 3 (1–3) |
| 29 | 9 (8–10,5) | 8,59 (8–10,3) | 7 (2–10,5) | 6,5 (1,5–9) | 4,5 (2–6) | 2,5 (1,5–3) |
| 30 | 9 (8–10) | 8,9 (8,05–9,91) | 7 (2,5–10) | 6 (3–10) | 3,5 (1–6) | 3 (1,5–3) |
| 31 | 9,5 (8–11) | 9,35 (7,99–10,5) | 4 (3–8,5) | 5,5 (2,5–8) | 4 (2,5–5) | 3 (2–3) |
| 32 | 9 (8–10,5) | 8,86 (7,85–10,4) | 5,5 (1,5–9,5) | 5,5 (2–10) | 5 (2,5–7) | 3 (1–3) |
| 33 | 8,5 (8–10) | 8,95 (7,93–9,86) | 5,5 (1,5–10,5) | 5,5 (2,5–10,5) | 3,5 (2–5,5) | 3 (1,5–3) |
| 34 | 9 (8–10,5) | 8,81 (7,69–9,95) | 6 (3,5–10,5) | 5,5 (2,5–10,5) | 5,5 (2–8,5) | 3 (2–3) |
| 35 | 8,5 (8–10) | 8,85 (7,64–10,3) | 6,5 (2,5–8,5) | 6,5 (3–9) | 4 (1,5–6) | 3 (1–3) |
| 36 | 9 (8–10,5) | 9,1 (7,99–10,1) | 5 (3–8,5) | 5 (3–7) | 5 (1,5–6,5) | 3 (2–3) |
| 37 | 9 (8–10,5) | 8,88 (8,01–10,1) | 6 (3–11) | 6 (2,5–11) | 4,5 (2–7) | 3 (2–3) |
| 38 | 9 (7,5–10,5) | 9,09 (7,91–10,3) | 4,5 (2,5–8,5) | 4,5 (2–9) | 3 (2–7,5) | 3 (2–3) |
| 39 | 9 (8–10) | 8,55 (7,94–9,91) | 6 (2–9,5) | 6 (2–9,5) | 5 (2–6,5) | 3 (2–3) |
| 40 | 9 (8–11) | 9,3 (7,95–10,1) | 5 (2–9,5) | 5 (2–10) | 4 (1,5–6,5) | 3 (2–3) |
| 41 | 9 (8–10) | 9,03 (8,01–10,1) | 5 (2–8) | 5 (2–9) | 4,5 (2–7) | 3 |
| 42 | 9 (8–10,5) | 8,7 (7,73–10) | 3,5 (2–8,5) | 3 (2–9) | 4 (1–6,5) | 3 |
| 43 | 9 (8–10,5) | 8,99 (7,68–9,95) | 5 (1,5–9,5) | 5 (2–9,5) | 5 (2–7) | 3 (2–3) |
| 44 | 8,5 (7,5–10) | 9,03 (7,8–10,1) | 5 (3–8,5) | 5 (2–9,5) | 3 (1–5,5) | 3 (1,5–3) |
| 45 | 9 (8–10,5) | 8,7 (7,76–10,4) | 4,5 (2,5–10) | 4 (1,5–9) | 3 (0–8) | 3 (2,5–3) |
| 46 | 10 (8–10,5) | 9,21 (7,93–10,1) | 5,5 (1,5–9,5) | 5 (1–9) | 4 (1,5–6) | 3 (2,5–3) |
| 47 | 9 (8–10,5) | 8,89 (7,99–10,6) | 5 (2–6,5) | 5 (1,5–8) | 3,5 (1–7) | 3 (1,5–3) |
| 48 | 9 (8–11) | 8,91 (7,85–10,7) | 4,5 (2–8,5) | 5 (2–9) | 3,5 (1–6) | 3 (2–3) |
| 49 | 9 (7,5–11) | 8,7 (7,75–10,4) | 5 (1–7,5) | 4,5 (2–7,5) | 5 (1,5–6) | 3 (2–3) |
| 50 | 9 (8–11) | 9 (7,93–10,7) | 4 (1,5–9) | 4,5 (1,5–8,5) | 5 (2–7) | 3 (2–3) |
| 51 | 9 (8–11) | 8,84 (7,73–10,2) | 5 (1,5–10) | 5 (2–10,5) | 4,5 (2–5,5) | 3 (2,5–3) |
| 52 | 9 (8–10) | 9,1 (8,2–10,7) | 4 (1,5–9,5) | 3,5 (0,5–10) | 4,5 (2,5–8) | 3 (1–3) |
| 53 | 9 (8–11) | 8,96 (7,86–10,4) | 4,5 (1–9,5) | 4,5 (1–10) | 5 (3–6,5) | 3 (2–3) |
| 54 | 9 (8–10,5) | 8,78 (7,95–10,3) | 4,5 (0,5–10) | 4,5 (1–9,5) | 4 (2–7,5) | 3 (2,5–3) |
| 55 | 9 (8–10,5) | 8,86 (7,65–10,9) | 4,5 (1–11) | 3,5 (1–12,5) | 4 (1–7,5) | 3 (1,5–3) |
| 56 | 9,5 (8–10,5) | 8,64 (7,74–10,7) | 3,5 (1,5–9,5) | 3 (1–9) | 3,5 (1,5–5,5) | 3 (1,5–3) |
| 57 | 9 (7,5–11) | 9 (7,93–10,5) | 4,5 (0,5–10,5) | 4 (1,5–10) | 4,5 (1,5–7) | 3 (2–3) |
| 58 | 9 (8–10,5) | 8,91 (7,59–10,3) | 4 (1,5–10,5) | 4,5 (2–9,5) | 4,5 (1,5–6) | 3 (2–3) |
| 59 | 9 (8–10) | 8,58 (7,74–10,4) | 3,5 (1–10,5) | 3 (1–12) | 3,5 (1–6) | 3 (2–3) |
| 60 | 9 (8–11) | 8,9 (7,6–10,5) | 3 (1–10) | 3,5 (0,5–9,5) | 3 (0,5–6,5) | 3 (2–3) |

### Canavarlar (2/2)

| Yıl | Yaşayan ejderha (yıl sonu) | Ejderha akını | Kriz (anlatıcı) | Rahatlama dönemi (anlatıcı) |
|---|---|---|---|---|
| 1 | 0 | 0 | 3,5 (2–6) | 0 (0–1) |
| 2 | 0 | 0 | 3,5 (3–5) | 0 (0–1) |
| 3 | 0 | 0 | 2 (1–3) | 0 |
| 4 | 0 | 0 | 3 (2–4,5) | 0 (0–1) |
| 5 | 0 | 0 | 3 (1–4,5) | 0 (0–0,5) |
| 6 | 0 | 0 | 3 (1,5–5) | 0 (0–1) |
| 7 | 0 | 0 | 3 (2–5,5) | 0 (0–1) |
| 8 | 0 | 0 | 3 (2–4) | 0 (0–1) |
| 9 | 0 | 0 | 3 (1,5–5) | 0 (0–1) |
| 10 | 0 | 0 | 3 (1–4) | 0 (0–1) |
| 11 | 0 | 0 | 2,5 (1,5–4) | 0 (0–1) |
| 12 | 0 | 0 | 3 (1–4,5) | 0 (0–1) |
| 13 | 0 | 0 | 3 (1–4) | 0 (0–1) |
| 14 | 0 (0–1) | 0 (0–1) | 3 (2–4,5) | 0 (0–1) |
| 15 | 1 (0–1) | 1 (0–2) | 2,5 (1,5–4,5) | 0 (0–1) |
| 16 | 1 (0,5–1) | 1 (1–2) | 3 (1–5) | 0,5 (0–1) |
| 17 | 1 (0–1) | 1 (0–2) | 2 (0,5–3,5) | 0 (0–1) |
| 18 | 1 (0–1) | 1 (0–2) | 2,5 (1–4) | 0,5 (0–1) |
| 19 | 0,5 (0–1) | 1 (0–2) | 3 (2–5,5) | 0 (0–1) |
| 20 | 0,5 (0–1) | 0,5 (0–2) | 3 (1,5–4) | 0 (0–1) |
| 21 | 0,5 (0–1) | 0,5 (0–2) | 3 (2–5,5) | 0 (0–1) |
| 22 | 0,5 (0–1) | 0 (0–2) | 2,5 (1,5–4) | 0 |
| 23 | 0,5 (0–1) | 0,5 (0–2) | 3 (1–4) | 0 (0–1) |
| 24 | 0 (0–1) | 0 (0–2) | 3 (1,5–5) | 0 (0–1) |
| 25 | 0 (0–1) | 0 (0–1) | 3 (1,5–4) | 0 (0–1) |
| 26 | 0 (0–1) | 0 (0–2) | 3,5 (0–4,5) | 0 (0–1) |
| 27 | 0 (0–1) | 0 (0–2) | 4 (2–5) | 0 (0–1) |
| 28 | 0 (0–1) | 0 (0–1,5) | 2 (0,5–5) | 0 (0–1) |
| 29 | 0 (0–1) | 0 (0–2) | 2 (1–4) | 0 (0–1) |
| 30 | 0 (0–1) | 0 (0–1) | 3 (0–5) | 0 (0–1) |
| 31 | 0 (0–1) | 0 (0–2) | 3,5 (1,5–4) | 0 (0–1) |
| 32 | 0 (0–1) | 0 (0–1,5) | 2 (1–3,5) | 0 (0–1) |
| 33 | 0 (0–1) | 0 (0–2) | 2 (1–3,5) | 0 (0–1) |
| 34 | 0 (0–1) | 0 (0–2) | 2,5 (1–4,5) | 1 (0–1) |
| 35 | 0 (0–1) | 0 (0–1) | 2 (1–4) | 0 (0–1) |
| 36 | 0 (0–1) | 0 (0–1,5) | 4 (1–6) | 0 (0–1) |
| 37 | 0 (0–1) | 0 (0–2) | 3 (0,5–4,5) | 0 (0–1) |
| 38 | 0 (0–1) | 0 (0–1,5) | 3 (1,5–4,5) | 0 (0–1) |
| 39 | 0 (0–1) | 0 (0–2) | 3 (1,5–6) | 0 (0–1) |
| 40 | 0 (0–1) | 0 (0–1,5) | 2,5 (1–5) | 0 (0–1) |
| 41 | 0 (0–1) | 0 (0–1,5) | 2,5 (1–4) | 0 (0–1) |
| 42 | 0 (0–1) | 0 (0–2) | 2,5 (1–5,5) | 0 (0–1) |
| 43 | 0 (0–1) | 0 (0–2) | 3 (1,5–5) | 0 (0–1) |
| 44 | 0 (0–1) | 0 (0–2) | 2 (1,5–6) | 0 (0–1) |
| 45 | 0 (0–1) | 0 (0–2) | 3 (1–4) | 0 (0–1) |
| 46 | 0 (0–1) | 0 (0–2) | 3 (0,5–4,5) | 0 (0–1) |
| 47 | 0 (0–1) | 0 (0–1,5) | 2 (0,5–5) | 0 (0–1) |
| 48 | 0 (0–1) | 0 (0–2) | 3 (1,5–5) | 0 (0–0,5) |
| 49 | 0 (0–1) | 0 (0–1,5) | 3 (1–4) | 0 (0–1) |
| 50 | 0 (0–1) | 0 (0–2) | 3 (1–5) | 0 (0–1) |
| 51 | 0 (0–1) | 0 (0–1) | 3 (1–5) | 0 (0–1) |
| 52 | 0 (0–1) | 0 (0–2) | 3 (2–4,5) | 0 (0–1) |
| 53 | 0 (0–1) | 0 (0–2) | 2 (1–4) | 0 (0–1) |
| 54 | 0 (0–1) | 0 (0–1) | 2 (1–3,5) | 0 (0–1) |
| 55 | 0 (0–1) | 0 (0–1,5) | 3,5 (0,5–6) | 0 (0–1) |
| 56 | 0 (0–1) | 0 (0–1,5) | 3 (0,5–5,5) | 0 (0–1) |
| 57 | 0 (0–1) | 0 (0–2) | 3 (2–4,5) | 0 (0–1) |
| 58 | 0 (0–1) | 0 (0–2) | 2 (1–4) | 0 (0–1) |
| 59 | 0 (0–1) | 0 (0–2) | 3 (1,5–5,5) | 0 (0–1) |
| 60 | 0 (0–1) | 0 (0–1,5) | 2,5 (1,5–6) | 0 (0–1) |

### Kahramanlar (1/2)

| Yıl | Doğan kahraman | Ölen kahraman | Emekli olan kahraman | Diyarı terk eden kahraman | Ölümden dönen kahraman | Efsane olan kahraman |
|---|---|---|---|---|---|---|
| 1 | 10,5 (5,5–15,5) | 2,5 (0–5,5) | 1 (0–2,5) | 3,5 (0,5–9,5) | 0 | 0 |
| 2 | 11 (5–17) | 2 (0–5,5) | 2 (0–3) | 2,5 (0,5–10) | 0 | 0 (0–0,5) |
| 3 | 10,5 (6,5–13,5) | 3 (0–7) | 2 (0–3) | 3 (1–11) | 0 | 0 |
| 4 | 10,5 (7–15,5) | 4 (0,5–9,5) | 1 (0–3) | 3 (1–11) | 0 | 0 (0–0,5) |
| 5 | 10,5 (6,5–15,5) | 2 (1–5) | 1 (0–4,5) | 2,5 (1–10) | 0 | 0 |
| 6 | 7,5 (4–17,5) | 4,5 (0–9) | 1 (0–2) | 4 (1–8) | 0 | 0 |
| 7 | 9,5 (6–15,5) | 3,5 (1,5–8) | 1 (0–3,5) | 3,5 (0,5–11) | 0 | 0 |
| 8 | 11,5 (5,5–16) | 2,5 (0–5,5) | 1 (0–3) | 2,5 (1,5–12) | 0 | 0 |
| 9 | 10,5 (6,5–12,5) | 2 (1–5) | 2 (0–4,5) | 4 (1–10,5) | 0 | 0 |
| 10 | 10 (5–20) | 3 (0–6,5) | 1,5 (0–3) | 3 (1–13,5) | 0 | 0 |
| 11 | 9,5 (5,5–13,5) | 3 (1–7) | 2 (1–3,5) | 3 (0–9) | 0 | 0 |
| 12 | 10,5 (7,5–19) | 3 (1,5–8) | 1 (0–5) | 3,5 (0,5–11) | 0 | 0 |
| 13 | 10 (6–16) | 3 (0,5–6) | 1 (0–4) | 3,5 (1–12) | 0 | 0 |
| 14 | 11 (6–16) | 3 (0–8) | 2 (0–3,5) | 2,5 (1–11,5) | 0 | 0 (0–0,5) |
| 15 | 9,5 (6–16,5) | 3 (1,5–6,5) | 2 (0,5–5) | 6 (1–11) | 0 | 0 |
| 16 | 10 (6–17) | 5 (2,5–9) | 2 (0,5–4) | 4,5 (1–9) | 0 | 0 (0–0,5) |
| 17 | 11 (3–16) | 4 (2–11,5) | 2 (0,5–3) | 3,5 (1–12,5) | 0 | 0 |
| 18 | 11,5 (6–15,5) | 3 (1–5,5) | 2 (1–3) | 3 (1–11,5) | 0 | 0 (0–1) |
| 19 | 9,5 (6,5–17) | 6,5 (0,5–9) | 1,5 (0–3) | 3,5 (0,5–10) | 0 | 0 (0–1) |
| 20 | 11 (5–20,5) | 4 (1–7) | 2 (0–3,5) | 5 (1–17) | 0 | 0 (0–0,5) |
| 21 | 11 (3,5–16) | 3 (0,5–10) | 2,5 (0,5–3) | 2 (0,5–11) | 0 | 0 (0–0,5) |
| 22 | 12 (8,5–16) | 4,5 (0,5–7) | 2 (0,5–4,5) | 4 (0–12) | 0 | 0 |
| 23 | 9 (5–16) | 4,5 (1–6) | 1,5 (0–3) | 4 (0,5–8,5) | 0 | 0 |
| 24 | 11 (6–17) | 5 (2,5–10) | 2 (1–4) | 4 (1,5–10) | 0 | 0 |
| 25 | 10,5 (7,5–17) | 5 (3,5–9,5) | 2 (0–4) | 3 (1–9) | 0 | 0 |
| 26 | 10 (4,5–17) | 4 (1–10) | 2 (1–5) | 4 (0,5–9,5) | 0 | 0 (0–1) |
| 27 | 11 (7–16) | 3,5 (1,5–8) | 2 (0–3) | 4,5 (1–12) | 0 | 0 |
| 28 | 9,5 (8–16,5) | 3 (1–8) | 2 (0,5–3) | 6 (1,5–11,5) | 0 | 0 (0–1) |
| 29 | 11 (5,5–18) | 5,5 (1–10) | 1,5 (0–3) | 7,5 (1–11) | 0 | 0 |
| 30 | 11 (6–20) | 4,5 (0–9,5) | 2 (0,5–4) | 2,5 (1–16,5) | 0 | 0 |
| 31 | 11 (7,5–18) | 5 (1,5–13,5) | 2 (1–4) | 3 (1–16) | 0 | 0 |
| 32 | 11,5 (6,5–14) | 3 (2–7) | 2,5 (0–4) | 6 (1,5–12,5) | 0 | 0 (0–0,5) |
| 33 | 10 (7,5–17) | 3 (1,5–10) | 2 (0,5–4) | 6 (1–16) | 0 | 0 |
| 34 | 11 (6,5–16) | 4 (1,5–12,5) | 1 (0–3,5) | 3 (0–14,5) | 0 | 0 (0–1) |
| 35 | 12 (7,5–17,5) | 3 (2,5–7) | 2 (1–4,5) | 5 (1–13) | 0 | 0 |
| 36 | 11 (7–19) | 3 (1,5–6) | 2 (0–4) | 4 (1–9) | 0 | 0 |
| 37 | 11 (8,5–15) | 3,5 (1–9,5) | 1,5 (1–3,5) | 3 (1–13) | 0 | 0 |
| 38 | 11 (8–16,5) | 4 (1,5–7,5) | 2,5 (0,5–5) | 7 (1,5–11,5) | 0 | 0 |
| 39 | 13 (8,5–18) | 3,5 (1–5,5) | 2 (1–4) | 3 (0,5–12,5) | 0 | 0 |
| 40 | 11,5 (7–18,5) | 3 (0,5–6,5) | 2 (0,5–5) | 9,5 (0,5–13) | 0 | 0 |
| 41 | 13,5 (7–18) | 2,5 (1,5–11) | 3 (0,5–4) | 4 (2–17) | 0 | 0 |
| 42 | 12 (9,5–17,5) | 3,5 (1,5–6) | 1 (0,5–5) | 5 (2–14) | 0 | 0 |
| 43 | 11,5 (6–17) | 4,5 (1,5–7,5) | 2 (0,5–5) | 7 (1–14) | 0 | 0 (0–1) |
| 44 | 11,5 (8,5–16,5) | 5 (1–8,5) | 2 (1–4) | 3,5 (1–10,5) | 0 | 0 (0–0,5) |
| 45 | 12,5 (8–17,5) | 4,5 (1–6,5) | 2 (1–4) | 3,5 (1–11) | 0 | 0 (0–0,5) |
| 46 | 13,5 (8,5–15,5) | 7 (1,5–11) | 3 (0,5–4,5) | 4,5 (2,5–13) | 0 | 0 |
| 47 | 13 (9–21,5) | 4 (1,5–8) | 2 (1–4) | 5 (2,5–12,5) | 0 | 0 |
| 48 | 13 (6–17,5) | 3,5 (1,5–7,5) | 2,5 (1–4,5) | 4,5 (0,5–14,5) | 0 | 0 |
| 49 | 14 (10,5–18) | 4 (3–10) | 3 (1–4,5) | 6,5 (1,5–11) | 0 | 0 |
| 50 | 12 (8,5–16,5) | 5 (3–10) | 2 (1–5) | 6 (1–17) | 0 | 0 |
| 51 | 12,5 (8,5–19) | 4,5 (1,5–8) | 3 (1,5–4) | 8 (1–14,5) | 0 | 0 (0–0,5) |
| 52 | 10 (7,5–17) | 4 (0–7) | 2,5 (0–4) | 6 (0,5–14) | 0 | 0 |
| 53 | 11 (8–18) | 5 (2–8) | 2 (0,5–4) | 3,5 (1–12,5) | 0 | 0 (0–0,5) |
| 54 | 13 (8,5–17,5) | 4 (1–8,5) | 3 (1–5) | 8 (1–13,5) | 0 | 0 |
| 55 | 11 (9,5–17,5) | 5 (0,5–9) | 1 (0–3,5) | 4 (1–14) | 0 | 0 |
| 56 | 13 (9,5–16) | 3,5 (0–6,5) | 2,5 (1–5,5) | 9 (2–16,5) | 0 | 0 |
| 57 | 14 (6,5–21,5) | 4,5 (2–12) | 2 (0–5) | 5 (2–10,5) | 0 | 0 |
| 58 | 13 (8,5–14,5) | 2 (0,5–6,5) | 2 (0,5–5) | 7 (3–12) | 0 | 0 |
| 59 | 13 (8,5–19,5) | 4 (1–9) | 3 (0,5–4) | 6 (1,5–14) | 0 | 0 |
| 60 | 12 (7,5–16) | 4 (2–7,5) | 2 (0–3) | 8,5 (3–16) | 0 | 0 |

### Kahramanlar (2/2)

| Yıl | Yaşayan kahraman (yıl sonu) | Doğuş seviyesi (ort.) | Ölüm seviyesi (ort.) | Yaşayan kahraman seviyesi (ort.) | En yüksek seviye (şimdiye dek) |
|---|---|---|---|---|---|
| 1 | 132 (108–168) | 1,53 (1,29–1,75) | 4 (3,41–5,25) | 4,42 (4,1–4,97) | 10 |
| 2 | 134 (108–170) | 1,62 (1,25–1,83) | 5,9 (3,72–6,42) | 4,36 (4,07–4,91) | 10 |
| 3 | 140 (111–169) | 1,69 (1,36–1,95) | 4,6 (3,27–5,4) | 4,38 (4,14–4,84) | 10 |
| 4 | 144 (111–166) | 1,58 (1,3–1,94) | 4,88 (3,3–5,5) | 4,43 (4,13–4,91) | 10 |
| 5 | 146 (113–164) | 1,47 (1,29–1,74) | 4,33 (2–5,15) | 4,46 (4,1–4,91) | 10 |
| 6 | 144 (110–171) | 1,63 (1,43–2) | 4,25 (2,29–5) | 4,6 (4,01–5,09) | 10 |
| 7 | 141 (113–172) | 1,76 (1,5–1,96) | 5 (3,4–6,12) | 4,61 (4,1–5,03) | 10 |
| 8 | 142 (118–174) | 1,63 (1,51–1,83) | 4,78 (4,02–7,9) | 4,56 (4,15–4,95) | 10 |
| 9 | 145 (116–172) | 1,72 (1,48–2,15) | 4,73 (2,65–5,85) | 4,49 (4,18–4,99) | 10 |
| 10 | 146 (114–175) | 1,6 (1,45–1,88) | 4,67 (3,09–5,85) | 4,53 (4,14–4,99) | 10 |
| 11 | 147 (113–174) | 1,55 (1,29–1,92) | 4,63 (3,74–7,1) | 4,55 (4,1–5,02) | 10 |
| 12 | 148 (116–177) | 1,69 (1,47–1,95) | 4,75 (2,2–6,2) | 4,57 (4,17–4,95) | 10 |
| 13 | 150 (118–177) | 1,48 (1,38–1,8) | 4,25 (2,77–5,23) | 4,55 (4,17–4,98) | 10 |
| 14 | 148 (120–180) | 1,67 (1,51–2) | 4,13 (2,53–4,93) | 4,64 (4,19–4,98) | 10 |
| 15 | 148 (123–178) | 1,66 (1,33–1,89) | 4 (2,83–5,58) | 4,61 (4,26–5,05) | 10 |
| 16 | 150 (122–176) | 1,65 (1,44–1,85) | 4,47 (3,53–5,92) | 4,56 (4,27–5,03) | 10 |
| 17 | 146 (121–175) | 1,74 (1,32–2,14) | 4,67 (3,4–6,6) | 4,62 (4,34–5,15) | 10 |
| 18 | 146 (124–174) | 1,52 (1,35–1,72) | 4,83 (3,08–6) | 4,59 (4,3–5,12) | 10 |
| 19 | 144 (126–178) | 1,7 (1,33–1,95) | 5,3 (4,27–5,89) | 4,55 (4,29–5,23) | 10 |
| 20 | 144 (129–178) | 1,63 (1,4–1,83) | 5,67 (4–7,6) | 4,55 (4,21–5,24) | 10 |
| 21 | 144 (126–176) | 1,5 (1,37–1,94) | 4,88 (3,4–6,47) | 4,62 (4,05–5,36) | 10 |
| 22 | 146 (126–178) | 1,59 (1,34–1,84) | 5,46 (3,48–6,43) | 4,57 (4,07–5,37) | 10 |
| 23 | 147 (125–182) | 1,53 (1,33–1,85) | 4 (3,08–5,1) | 4,63 (4,06–5,47) | 10 |
| 24 | 142 (127–177) | 1,55 (1,37–1,81) | 4,55 (2,98–6,56) | 4,61 (4,04–5,46) | 10 |
| 25 | 146 (127–178) | 1,6 (1,43–1,9) | 5 (4,19–6,03) | 4,58 (4,05–5,43) | 10 |
| 26 | 146 (127–180) | 1,56 (1,35–1,77) | 5,13 (4,16–7) | 4,54 (4,01–5,38) | 10 |
| 27 | 150 (128–180) | 1,73 (1,37–1,9) | 4,88 (2,75–6,17) | 4,53 (4,01–5,43) | 10 |
| 28 | 151 (128–182) | 1,58 (1,33–1,78) | 5 (2,67–7,29) | 4,47 (4,06–5,42) | 10 |
| 29 | 151 (124–182) | 1,65 (1,38–1,94) | 5 (2,86–6,96) | 4,55 (4,02–5,48) | 10 |
| 30 | 152 (128–182) | 1,6 (1,32–1,88) | 4,33 (4–5,92) | 4,56 (4,04–5,41) | 10 |
| 31 | 152 (130–181) | 1,54 (1,36–1,86) | 4,8 (4,11–6,6) | 4,58 (4,12–5,32) | 10 |
| 32 | 151 (130–177) | 1,67 (1,41–1,92) | 5 (3,92–6,46) | 4,58 (4,14–5,3) | 10 |
| 33 | 150 (127–176) | 1,53 (1,41–1,85) | 4,61 (3,17–6,17) | 4,59 (4,18–5,2) | 10 |
| 34 | 155 (127–170) | 1,61 (1,21–1,82) | 4,29 (2,33–6,52) | 4,63 (4,28–5,24) | 10 |
| 35 | 156 (126–174) | 1,6 (1,4–1,83) | 5 (2,7–7,07) | 4,56 (4,24–5,24) | 10 |
| 36 | 157 (126–178) | 1,57 (1,35–1,87) | 5,33 (3,8–6,31) | 4,47 (4,24–5,14) | 10 |
| 37 | 160 (123–178) | 1,55 (1,44–1,72) | 5 (3–6,8) | 4,51 (4,29–5,19) | 10 |
| 38 | 160 (122–178) | 1,58 (1,37–1,82) | 4,6 (2,8–6,67) | 4,52 (4,31–5,08) | 10 |
| 39 | 166 (124–182) | 1,61 (1,44–1,8) | 4 (3–6,12) | 4,5 (4,22–5,16) | 10 |
| 40 | 166 (122–176) | 1,58 (1,37–1,79) | 4,55 (3,3–7,35) | 4,56 (4,19–5,15) | 10 |
| 41 | 165 (128–174) | 1,67 (1,44–1,73) | 5,31 (3,27–7,1) | 4,48 (4,14–5,07) | 10 |
| 42 | 166 (130–180) | 1,65 (1,56–1,81) | 5 (2,77–6,36) | 4,4 (4,19–5,02) | 10 |
| 43 | 165 (134–174) | 1,65 (1,36–1,92) | 5,5 (3,8–7,09) | 4,43 (4,19–5,12) | 10 |
| 44 | 166 (136–177) | 1,65 (1,38–1,89) | 4,61 (2,17–5,63) | 4,44 (4,23–5,11) | 10 |
| 45 | 170 (135–180) | 1,5 (1,35–1,84) | 5 (2,58–5,72) | 4,45 (4,13–5,07) | 10 |
| 46 | 167 (134–182) | 1,7 (1,42–1,86) | 4,95 (3,94–6,63) | 4,46 (4,14–5,02) | 10 |
| 47 | 166 (132–184) | 1,62 (1,46–1,76) | 4,75 (3–6,55) | 4,36 (4,1–4,96) | 10 |
| 48 | 170 (132–182) | 1,68 (1,55–1,94) | 4,68 (2,42–5,76) | 4,39 (4,1–5,03) | 10 |
| 49 | 170 (132–181) | 1,66 (1,48–2,02) | 4,83 (3,3–6,75) | 4,38 (4,05–5) | 10 |
| 50 | 170 (132–180) | 1,75 (1,55–1,89) | 4,58 (3,81–5,45) | 4,47 (4,01–5,03) | 10 |
| 51 | 166 (133–180) | 1,59 (1,45–1,91) | 5 (4–6,75) | 4,36 (3,96–5,16) | 10 |
| 52 | 169 (130–182) | 1,69 (1,32–1,91) | 4,83 (3,31–6,4) | 4,36 (4,01–5,13) | 10 |
| 53 | 171 (132–185) | 1,6 (1,46–1,88) | 5,06 (2,96–6,13) | 4,29 (4,04–5,05) | 10 |
| 54 | 170 (135–182) | 1,71 (1,43–1,91) | 4,67 (2,85–7,6) | 4,33 (3,98–5,14) | 10 |
| 55 | 168 (137–184) | 1,62 (1,48–1,76) | 4,37 (3,2–4,67) | 4,36 (3,96–5,16) | 10 |
| 56 | 170 (142–182) | 1,67 (1,3–1,87) | 4,57 (2,53–5,93) | 4,33 (3,97–5,24) | 10 |
| 57 | 170 (142–184) | 1,59 (1,44–1,78) | 4,29 (2,85–6,56) | 4,37 (3,97–5,26) | 10 |
| 58 | 167 (146–185) | 1,58 (1,33–1,78) | 5 (3,1–6,7) | 4,37 (3,99–5,38) | 10 |
| 59 | 172 (149–186) | 1,68 (1,46–1,86) | 5,27 (3,33–6,46) | 4,36 (3,96–5,36) | 10 |
| 60 | 168 (149–188) | 1,67 (1,41–1,97) | 4,7 (3,17–8) | 4,44 (3,95–5,34) | 10 |

### Han ve ticaret

| Yıl | Ayakta han | Asılan ilan | Biten ilan | Ticaret seferi (kervan) | İkmal seferi |
|---|---|---|---|---|---|
| 1 | 4 (3–6) | 8 (4,5–15) | 5 (1–8,5) | 76,5 (51,5–110) | 33 (18–49) |
| 2 | 4 (3–6) | 13 (3,5–20,5) | 5 (2–12,5) | 74,5 (51,5–112) | 37 (20,5–50,5) |
| 3 | 4 (2,5–6) | 11,5 (6–18,5) | 4,5 (2,5–10,5) | 79,5 (59–114) | 35 (25–48,5) |
| 4 | 4 (3–6) | 11,5 (6–18,5) | 4 (1–12,5) | 77 (57–107) | 37,5 (24–49,5) |
| 5 | 4 (3–6) | 11 (9–17,5) | 5,5 (2–9,5) | 76,5 (59–108) | 37 (22,5–50) |
| 6 | 4 (2,5–6) | 11 (5,5–16,5) | 5,5 (1,5–11) | 73,5 (56,5–116) | 42 (27–50,5) |
| 7 | 4 (2,5–6) | 11 (8–23) | 5 (3–16,5) | 73,5 (63–110) | 41,5 (25–48,5) |
| 8 | 4 (3–6) | 14,5 (5–21,5) | 7 (3–10) | 76 (63–112) | 39,5 (26,5–51) |
| 9 | 4 (3–6) | 12,5 (3–18,5) | 5,5 (1,5–8,5) | 77,5 (60–116) | 39,5 (25,5–53) |
| 10 | 4 (3–6) | 11,5 (5–18,5) | 6 (3–10) | 85 (58–124) | 40,5 (24–50,5) |
| 11 | 4 (3–6) | 13 (8,5–17,5) | 6 (2,5–12,5) | 84 (64–129) | 39 (26,5–50,5) |
| 12 | 4 (3–6) | 14 (6,5–21,5) | 5 (2–11,5) | 86 (66,5–121) | 42 (28,5–52) |
| 13 | 4 (3–6) | 10 (6,5–14,5) | 5 (2,5–8) | 80 (62,5–126) | 41 (31–53,5) |
| 14 | 4 (3–6) | 11 (7,5–16,5) | 5,5 (3–8) | 85,5 (71,5–128) | 43 (32,5–57,5) |
| 15 | 4 (3–6) | 15 (7–19) | 4 (2–11) | 85 (67–128) | 41,5 (32,5–59,5) |
| 16 | 4 (3–6) | 18 (11–23,5) | 9 (0,5–12) | 77 (59,5–124) | 40,5 (30–64) |
| 17 | 4,5 (3–6) | 14 (9,5–19,5) | 5,5 (2,5–11) | 83,5 (58–131) | 42 (27–66,5) |
| 18 | 4,5 (3–6) | 14,5 (8,5–22,5) | 5,5 (1–10,5) | 84 (66,5–127) | 37,5 (30,5–66,5) |
| 19 | 4,5 (2,5–6) | 14,5 (5–21,5) | 6,5 (1,5–11) | 79,5 (67–131) | 41 (28,5–69,5) |
| 20 | 4 (3–6) | 14 (10–21) | 5 (3–9,5) | 86,5 (70,5–122) | 38,5 (28–64,5) |
| 21 | 4 (3–6) | 14,5 (5–25,5) | 7 (1–11) | 83 (69–136) | 41 (28,5–59) |
| 22 | 4 (3–6) | 16 (9–20) | 6,5 (1,5–11,5) | 85,5 (64–132) | 38 (30–64,5) |
| 23 | 4 (3–6) | 13,5 (9–17,5) | 4 (2,5–11,5) | 90 (69–126) | 39,5 (30–68,5) |
| 24 | 4 (3–6) | 16 (9,5–23,5) | 6 (2–10) | 84 (68–131) | 42 (28–72,5) |
| 25 | 4 (3–6) | 15 (8–27) | 4,5 (2,5–12) | 91 (66,5–138) | 40 (27,5–76) |
| 26 | 4 (3–6) | 16,5 (8,5–25) | 6 (1,5–11) | 91 (74,5–142) | 38 (26–74) |
| 27 | 4 (3–6) | 13,5 (10–22) | 6 (2–9,5) | 91,5 (74–136) | 37,5 (27,5–73,5) |
| 28 | 4 (2,5–6) | 16,5 (8–28) | 7,5 (1,5–12) | 95,5 (71–133) | 37,5 (25,5–73,5) |
| 29 | 4 (2,5–6) | 17,5 (7,5–24) | 6 (2–10,5) | 97 (75,5–140) | 38 (28–76) |
| 30 | 4 (3–6) | 18 (8–25,5) | 6,5 (3–13) | 100 (77,5–137) | 36 (24–72,5) |
| 31 | 4 (3–6) | 18 (8,5–27) | 5,5 (1,5–11,5) | 98,5 (80–155) | 39 (23–67) |
| 32 | 4 (3–6) | 15,5 (8–24) | 6 (2,5–10,5) | 99,5 (71–152) | 37,5 (27–68) |
| 33 | 4 (3–6) | 16 (8,5–24) | 6 (2,5–11,5) | 97,5 (73,5–146) | 39 (22–71,5) |
| 34 | 4 (3–6) | 17,5 (8,5–23,5) | 6,5 (2,5–14) | 90 (69–152) | 38,5 (22–70,5) |
| 35 | 4 (3–6) | 14 (10–25) | 6 (3–9) | 97 (74,5–150) | 39,5 (20,5–70) |
| 36 | 4 (3–6) | 15 (10,5–25) | 5,5 (2–13) | 90,5 (71–166) | 41 (21–71,5) |
| 37 | 4 (3–6) | 17 (10–23,5) | 7 (4–14,5) | 89,5 (68,5–162) | 44 (21,5–75) |
| 38 | 4 (3–6) | 15,5 (10,5–23) | 6,5 (2–14) | 90 (62–162) | 43,5 (23,5–76) |
| 39 | 4 (3–6,5) | 14 (5,5–22) | 6,5 (0,5–11,5) | 96,5 (65–156) | 43 (23–76,5) |
| 40 | 4 (3–6) | 16,5 (12–22,5) | 7 (1,5–12,5) | 99,5 (57,5–145) | 43 (23,5–78) |
| 41 | 4,5 (3,5–6) | 16,5 (8–24) | 5 (2–12) | 102 (64–134) | 43 (24–76,5) |
| 42 | 4,5 (3,5–6) | 16 (11,5–23,5) | 5,5 (2–11,5) | 99 (64–136) | 43 (24,5–78) |
| 43 | 4,5 (3–6) | 15 (9,5–25,5) | 6 (2,5–11) | 97 (74,5–132) | 40,5 (24–74) |
| 44 | 4,5 (3–6) | 17 (10–23) | 6,5 (2,5–12) | 93,5 (74–138) | 39 (27,5–70) |
| 45 | 4 (3,5–6) | 16 (8,5–21) | 6 (2–9,5) | 96 (71–126) | 41 (28–68,5) |
| 46 | 4 (3,5–6,5) | 17,5 (8–27) | 7,5 (1–13,5) | 102 (78,5–130) | 44,5 (25,5–65,5) |
| 47 | 5 (3,5–6,5) | 18 (11–24) | 5,5 (2,5–13,5) | 104 (72–126) | 45 (28–65,5) |
| 48 | 4,5 (3,5–6,5) | 17 (7,5–21) | 4 (2–10) | 99 (62,5–132) | 42 (26,5–66,5) |
| 49 | 4,5 (3,5–6,5) | 14,5 (8,5–20,5) | 6 (1–13) | 100 (58,5–130) | 43,5 (24,5–65,5) |
| 50 | 4 (3–6,5) | 16,5 (9–25) | 6,5 (2–11,5) | 98,5 (70–136) | 46,5 (25–67) |
| 51 | 4 (3–6,5) | 17 (6,5–25,5) | 6,5 (1–11) | 98,5 (74–129) | 45,5 (23–68) |
| 52 | 4 (3–6,5) | 16 (8,5–24,5) | 6,5 (2–9,5) | 102 (72–129) | 41,5 (24,5–66,5) |
| 53 | 4,5 (3–6,5) | 17,5 (10–28,5) | 7,5 (3,5–12,5) | 102 (85,5–124) | 40,5 (27–66,5) |
| 54 | 4,5 (3–6) | 17,5 (7,5–23) | 7 (2–10,5) | 98,5 (76–128) | 43 (28,5–66,5) |
| 55 | 5 (3–6) | 19 (9–25) | 7 (3,5–15) | 98 (81,5–131) | 44,5 (28–71,5) |
| 56 | 4,5 (3–6,5) | 15,5 (7,5–24) | 6 (0,5–9,5) | 101 (74,5–146) | 44 (28–72) |
| 57 | 5 (3–6) | 16,5 (12,5–22,5) | 6 (3,5–9) | 108 (79–154) | 41 (31,5–73,5) |
| 58 | 5 (3–6,5) | 14,5 (7,5–20,5) | 6 (0,5–12) | 107 (78,5–154) | 43 (31,5–75) |
| 59 | 5 (3–6,5) | 17,5 (6–27) | 4,5 (1–15) | 111 (83–162) | 43,5 (30,5–76) |
| 60 | 4,5 (3–6,5) | 16,5 (7–20) | 4,5 (2–8,5) | 114 (79,5–151) | 44,5 (30–76,5) |

### Altın ve ambar (1/3)

| Yıl | Altın p90 (medeniyetler) | Bakım gideri (altın; asker, kahraman, L2–L3) | Kamu işlerine (imar) harcanan altın | Ambarla beslenen amele tayını (gıda) | Kamu işlerindeki (amele) iş gücü payı | İmar ortalaması (köy+, 0–100) |
|---|---|---|---|---|---|---|
| 1 | 1181 (767–1466) | 2677 (2231–3196) | 1937 (970–2587) | 1412 (87,4–2287) | %12 (%10–%19) | 19,5 (13,2–23,1) |
| 2 | 1110 (788–1517) | 2619 (2329–3196) | 1881 (1034–2568) | 1136 (25,8–2327) | %12 (%9,9–%17) | 18,3 (14–24,2) |
| 3 | 1191 (891–1502) | 2586 (2320–3125) | 1844 (1290–2788) | 1018 (22–2422) | %13 (%8,8–%18) | 18,7 (12,9–25,5) |
| 4 | 1157 (811–1458) | 2705 (2200–3223) | 1782 (1408–2692) | 1111 (113–2991) | %14 (%8,4–%17) | 17,8 (14,3–25,5) |
| 5 | 1161 (774–1395) | 2662 (2283–3161) | 1800 (1056–2621) | 1042 (55,2–2929) | %13 (%8,8–%17) | 16,9 (14,3–23,9) |
| 6 | 1066 (781–1468) | 2675 (2268–3143) | 1737 (1209–2611) | 758 (205–2783) | %13 (%7,9–%16) | 17,3 (15–23) |
| 7 | 1121 (799–1261) | 2770 (2247–3084) | 1793 (996–2730) | 1334 (147–2589) | %13 (%9,8–%17) | 17,5 (13,8–21,3) |
| 8 | 1108 (922–1538) | 2851 (2342–3171) | 1895 (991–2563) | 1620 (139–2754) | %13 (%9,9–%18) | 17,7 (13,1–21,2) |
| 9 | 1069 (814–1517) | 2763 (2322–3195) | 1976 (1230–2862) | 1441 (168–2707) | %14 (%10–%19) | 17,6 (13,8–22) |
| 10 | 1060 (853–1537) | 2709 (2279–3276) | 2037 (1170–2923) | 1099 (130–2716) | %13 (%11–%18) | 17,2 (13,7–23,3) |
| 11 | 1095 (846–1989) | 2643 (2334–3215) | 1896 (1097–3069) | 1006 (196–2333) | %14 (%8,8–%17) | 17 (12,9–23,5) |
| 12 | 1082 (752–1931) | 2645 (2290–3219) | 1788 (1125–3712) | 1183 (136–2802) | %15 (%9,3–%16) | 16,5 (12,2–25,6) |
| 13 | 1106 (822–1864) | 2648 (2246–3268) | 2115 (897–3581) | 1184 (214–2490) | %14 (%8,7–%16) | 15,9 (11,4–25,9) |
| 14 | 1175 (794–1812) | 2744 (2198–3299) | 2260 (1076–3370) | 949 (44,6–2870) | %12 (%8,4–%16) | 16,5 (11,8–25,4) |
| 15 | 1092 (819–1560) | 2787 (2253–3139) | 2154 (1162–2968) | 1250 (43,2–2680) | %13 (%9,5–%18) | 16,4 (10,9–24,6) |
| 16 | 1164 (754–1476) | 2712 (2148–3112) | 2148 (859–2878) | 1107 (0–2466) | %13 (%9,9–%18) | 16,7 (10,3–23,7) |
| 17 | 1162 (911–1575) | 2790 (2158–3203) | 2070 (966–2697) | 648 (0–2116) | %13 (%8,4–%17) | 16,7 (10,1–22,4) |
| 18 | 1240 (748–1529) | 2667 (2194–3146) | 2202 (941–2991) | 559 (18–1937) | %13 (%6,9–%16) | 17,2 (10–21,1) |
| 19 | 1138 (732–1735) | 2665 (2181–3224) | 1916 (1039–3107) | 721 (20,6–2037) | %12 (%7,9–%17) | 17,2 (11,1–21,4) |
| 20 | 1203 (819–1758) | 2860 (2125–3292) | 1891 (1060–3332) | 372 (19,4–2530) | %12 (%7,4–%17) | 18,4 (12,4–21,2) |
| 21 | 1192 (824–1725) | 2793 (2175–3241) | 2081 (1265–3300) | 379 (76–2661) | %13 (%7,6–%17) | 17,3 (13,5–21,4) |
| 22 | 1113 (829–1555) | 2682 (2241–3082) | 1958 (996–3284) | 651 (173–2659) | %12 (%7,9–%16) | 17 (13,5–20,9) |
| 23 | 1246 (832–1637) | 2571 (2206–3177) | 1841 (1025–3151) | 725 (82–1777) | %12 (%9–%17) | 16,7 (12,4–21) |
| 24 | 1212 (910–1860) | 2600 (2046–3271) | 1845 (1259–3254) | 391 (29,8–2159) | %14 (%10–%17) | 15,2 (12,2–21,4) |
| 25 | 1095 (777–1913) | 2711 (2033–3059) | 1886 (1343–4140) | 595 (54–1877) | %13 (%10–%18) | 14,6 (11,3–24) |
| 26 | 1145 (740–1653) | 2497 (2057–3180) | 1832 (1228–3302) | 413 (52–1634) | %14 (%9,1–%17) | 14,6 (11,9–23,6) |
| 27 | 1226 (754–1835) | 2480 (2105–3238) | 2137 (1031–3282) | 444 (42,2–1717) | %13 (%7,9–%16) | 15,6 (11–23,3) |
| 28 | 1262 (777–2088) | 2482 (2138–3105) | 2292 (1141–3837) | 273 (0,6–1997) | %13 (%7,5–%17) | 15,3 (11–23,3) |
| 29 | 1396 (913–1919) | 2535 (1978–3183) | 2399 (1430–3712) | 271 (0–1291) | %12 (%6,9–%17) | 15,8 (10,3–24,2) |
| 30 | 1503 (974–2045) | 2497 (1892–3188) | 2651 (1417–3728) | 213 (0–1533) | %13 (%6,7–%16) | 15,1 (10,4–24,6) |
| 31 | 1601 (1018–2040) | 2500 (1815–3055) | 2934 (1414–4058) | 38,4 (0–1275) | %11 (%4–%16) | 14,5 (10,7–24,2) |
| 32 | 1381 (849–2154) | 2579 (1838–3054) | 2642 (1063–3971) | 89,8 (0–1346) | %11 (%4,9–%17) | 15 (10,9–25,2) |
| 33 | 1547 (789–2006) | 2521 (1864–3033) | 2624 (884–4373) | 83,2 (0–1549) | %11 (%5,4–%15) | 16 (10,5–25) |
| 34 | 1416 (678–2142) | 2415 (1921–3012) | 2844 (680–4569) | 213 (0–1567) | %9,6 (%4,8–%17) | 16,2 (9,92–25,1) |
| 35 | 1637 (703–2239) | 2470 (1709–3126) | 2812 (982–3957) | 193 (0,6–1726) | %11 (%4,7–%16) | 16,2 (8,93–24,7) |
| 36 | 1575 (916–2122) | 2425 (1816–3098) | 2855 (1083–4325) | 375 (0–2245) | %11 (%5,6–%15) | 16,3 (9,47–24,1) |
| 37 | 1642 (873–2127) | 2376 (1777–3119) | 2808 (1148–3666) | 258 (0–1872) | %12 (%5,4–%14) | 17,6 (9,16–23,9) |
| 38 | 1401 (739–2085) | 2387 (1841–3283) | 3099 (1164–3811) | 397 (0–1806) | %12 (%7–%16) | 16,9 (9,63–24,1) |
| 39 | 1427 (824–2223) | 2406 (1908–3312) | 2697 (1364–4050) | 493 (0–1622) | %12 (%7,9–%15) | 16,1 (10,1–23,4) |
| 40 | 1295 (912–1894) | 2381 (1779–3203) | 2749 (1273–4692) | 344 (0–1926) | %12 (%8,1–%17) | 15,9 (10,3–23,5) |
| 41 | 1356 (871–1996) | 2457 (1804–3152) | 2297 (1724–3973) | 114 (0–1514) | %12 (%6,2–%17) | 16,8 (10,3–23,5) |
| 42 | 1442 (1114–1982) | 2392 (1884–3020) | 2734 (1955–3741) | 106 (0–1488) | %12 (%7,4–%16) | 16,7 (11,7–24) |
| 43 | 1190 (977–2011) | 2267 (1926–3267) | 2549 (1522–3849) | 103 (0–1743) | %10 (%5,8–%16) | 16,8 (12,5–22,4) |
| 44 | 1261 (823–2125) | 2351 (1871–3211) | 2334 (1458–3920) | 10,2 (0–1502) | %11 (%6,9–%15) | 17,7 (11,9–21,6) |
| 45 | 1289 (743–2005) | 2343 (1819–3241) | 2299 (1221–4105) | 51,8 (0–1762) | %11 (%5,9–%15) | 18,3 (11–21,8) |
| 46 | 1377 (821–1952) | 2401 (1818–3145) | 2103 (1351–4350) | 209 (0–1665) | %11 (%5,1–%16) | 17,1 (10,3–21,2) |
| 47 | 1538 (846–2306) | 2441 (1732–3173) | 2524 (1483–4943) | 103 (0–1450) | %12 (%6,8–%16) | 17,2 (10,5–26,1) |
| 48 | 1356 (845–2160) | 2306 (1775–3208) | 2573 (1562–4644) | 78,4 (0–1246) | %12 (%7,6–%16) | 17 (10,1–27,3) |
| 49 | 1388 (998–1938) | 2359 (1786–3118) | 2691 (1624–4119) | 286 (15–1306) | %11 (%6,2–%15) | 16,5 (9,2–26,5) |
| 50 | 1585 (941–2094) | 2358 (1869–3179) | 2585 (1699–4155) | 208 (0–1232) | %13 (%5,4–%16) | 16,8 (11–26,8) |
| 51 | 1350 (937–2182) | 2284 (1834–3151) | 2620 (1845–4307) | 63,6 (0–1380) | %14 (%6,4–%16) | 16 (11,6–25,1) |
| 52 | 1486 (1081–1878) | 2307 (1815–3207) | 2766 (1732–4600) | 136 (0–1021) | %12 (%6,1–%14) | 16,6 (12,4–22,8) |
| 53 | 1580 (1206–2087) | 2286 (1781–3203) | 2834 (2041–4248) | 29,2 (0–718) | %10 (%5–%14) | 17,6 (13,2–21,3) |
| 54 | 1539 (1024–1997) | 2370 (1788–3150) | 3057 (1872–4167) | 26,8 (0–842) | %10 (%4,6–%16) | 16,9 (13,8–22,6) |
| 55 | 1479 (1043–1922) | 2416 (1972–3058) | 3019 (1731–3956) | 53,8 (0–1208) | %9,9 (%4,7–%18) | 17,3 (13,5–23) |
| 56 | 1479 (1030–1809) | 2554 (1999–3094) | 2810 (1863–3859) | 50,8 (0–906) | %9,6 (%5–%18) | 16,9 (13,3–21,3) |
| 57 | 1586 (955–1910) | 2341 (1981–3068) | 2833 (1749–3587) | 60,6 (0–986) | %13 (%5–%18) | 16,9 (12,9–21,3) |
| 58 | 1564 (1052–1887) | 2343 (2072–2898) | 2876 (1682–3988) | 131 (0,4–560) | %12 (%7,2–%15) | 16,6 (12,9–21,9) |
| 59 | 1397 (1124–2418) | 2305 (2063–2989) | 3007 (1986–3904) | 99,2 (0–668) | %13 (%6,6–%17) | 16,4 (12,3–21,3) |
| 60 | 1620 (1230–2158) | 2404 (2212–2994) | 2979 (1922–4498) | 112 (0–672) | %10 (%6,4–%15) | 15,9 (12,9–20,1) |

### Altın ve ambar (2/3)

| Yıl | Canavar baskınında yitirilen altın | Ejderhaya giden altın (haraç + akın) | Hazinesi boş medeniyet payı | Kent tüketiminde yokluk payı (köy+; ekmek, bira ya da alet) | Ekmek ya da bira yokluğu payı (köy+) | Kıtlık (büyük olay) |
|---|---|---|---|---|---|---|
| 1 | 0 (0–44) | 0 | %0 | %34 (%25–%44) | %5,2 (%1–%16) | 0 |
| 2 | 5 (0–37) | 0 | %0 | %31 (%25–%45) | %4,1 (%1,5–%18) | 0 (0–0,5) |
| 3 | 0 (0–19,5) | 0 | %0 | %33 (%23–%45) | %3,9 (%1,5–%16) | 0 |
| 4 | 2,5 (0–41,5) | 0 | %0 | %36 (%23–%49) | %5,9 (%0,8–%18) | 0 |
| 5 | 2 (0–17) | 0 | %0 | %37 (%29–%46) | %5,9 (%0,8–%11) | 0 |
| 6 | 1 (0–18) | 0 | %0 | %37 (%27–%45) | %8,1 (%1,3–%14) | 0 (0–1) |
| 7 | 1,5 (0–40) | 0 | %0 | %29 (%24–%37) | %6 (%1,6–%15) | 0 |
| 8 | 4,5 (0–37) | 0 | %0 | %33 (%23–%35) | %5,1 (%1,7–%15) | 0 (0–0,5) |
| 9 | 3 (0–23) | 0 | %0 | %35 (%22–%43) | %6,3 (%3,2–%18) | 0 |
| 10 | 8,5 (0–30) | 0 | %0 | %39 (%25–%48) | %7,6 (%1,1–%13) | 0 |
| 11 | 4,5 (0–24,5) | 0 | %0 | %38 (%27–%46) | %7,4 (%2,5–%16) | 0 |
| 12 | 0 (0–8) | 0 | %0 | %35 (%19–%42) | %6,3 (%1,2–%12) | 0 |
| 13 | 5,5 (0–20,5) | 0 | %0 | %31 (%20–%37) | %6,4 (%1,6–%11) | 0 |
| 14 | 1 (0–19) | 0 (0–180) | %0 | %33 (%24–%38) | %6,6 (%1,7–%18) | 0 (0–0,5) |
| 15 | 1 (0–38,5) | 6 (0–191) | %0 | %38 (%27–%43) | %6,6 (%3,5–%17) | 0 |
| 16 | 4 (0–22,5) | 106 (13,5–440) | %0 | %32 (%23–%40) | %7,1 (%3,9–%16) | 0 (0–0,5) |
| 17 | 4 (0–20,5) | 78,5 (0,5–306) | %0 | %35 (%19–%45) | %6,1 (%2,6–%17) | 0 |
| 18 | 0 (0–6,5) | 95 (0–576) | %0 | %31 (%20–%46) | %9,1 (%1,9–%14) | 0 |
| 19 | 0 (0–20,5) | 3 (0–822) | %0 | %30 (%22–%48) | %7 (%1,5–%16) | 0 (0–0,5) |
| 20 | 1 (0–11,5) | 47 (0–426) | %0 | %32 (%24–%47) | %7,5 (%1,8–%16) | 0 |
| 21 | 6,5 (0–19) | 47 (0–502) | %0 | %32 (%22–%47) | %8,4 (%2,7–%16) | 0 (0–0,5) |
| 22 | 0 (0–21) | 27 (0–818) | %0 | %34 (%30–%42) | %8,3 (%3,2–%17) | 0 |
| 23 | 0,5 (0–21) | 2 (0–608) | %0 | %33 (%24–%41) | %9,1 (%2,5–%17) | 0 (0–0,5) |
| 24 | 6,5 (0–26) | 24,5 (0–773) | %0 | %32 (%21–%42) | %6,5 (%2,4–%17) | 0 |
| 25 | 0 (0–31,5) | 0 (0–874) | %0 | %30 (%22–%44) | %5,3 (%2,1–%17) | 0 |
| 26 | 5,5 (0–48) | 0 (0–654) | %0 | %32 (%28–%45) | %8,5 (%3,3–%21) | 0 (0–0,5) |
| 27 | 2,5 (0–12) | 0 (0–521) | %0 | %35 (%25–%50) | %7,1 (%3,3–%21) | 0 |
| 28 | 6 (0–40) | 0 (0–732) | %0 | %33 (%25–%51) | %8 (%3,4–%21) | 0 |
| 29 | 0 (0–17,5) | 0 (0–782) | %0 | %31 (%25–%50) | %10 (%3,7–%21) | 0 |
| 30 | 3 (0–36) | 0 (0–650) | %0 | %31 (%23–%53) | %12 (%3,3–%21) | 0 (0–1) |
| 31 | 5 (0–49) | 0 (0–834) | %0 | %29 (%22–%53) | %8,9 (%4,3–%23) | 0 (0–0,5) |
| 32 | 5,5 (0–74,5) | 0 (0–420) | %0 | %33 (%25–%52) | %8,5 (%3,5–%26) | 0 (0–0,5) |
| 33 | 0 (0–23,5) | 0 (0–747) | %0 | %35 (%24–%53) | %7,6 (%3,8–%24) | 0 |
| 34 | 2,5 (0–38,5) | 0 (0–654) | %0 | %35 (%25–%50) | %11 (%2,8–%29) | 0 |
| 35 | 7,5 (0–26) | 0 (0–492) | %0 | %34 (%27–%51) | %12 (%2,7–%31) | 0 (0–0,5) |
| 36 | 6 (0–25,5) | 0 (0–550) | %0 | %32 (%27–%54) | %14 (%3,4–%39) | 0 (0–1) |
| 37 | 7,5 (0–30) | 0 (0–294) | %0 | %32 (%20–%57) | %10 (%2–%25) | 0 |
| 38 | 10 (0–36,5) | 0 (0–721) | %0 | %32 (%19–%55) | %9,5 (%2,3–%18) | 0 (0–0,5) |
| 39 | 11,5 (0–30) | 0 (0–850) | %0 | %31 (%23–%52) | %11 (%1,7–%20) | 0 |
| 40 | 3,5 (0–34,5) | 0 (0–520) | %0 | %36 (%26–%49) | %15 (%4,3–%24) | 0 |
| 41 | 0 (0–26,5) | 0 (0–612) | %0 | %38 (%23–%51) | %15 (%4,5–%30) | 0 (0–1) |
| 42 | 3 (0–29,5) | 0 (0–670) | %0 | %38 (%24–%50) | %12 (%3,7–%32) | 0 (0–0,5) |
| 43 | 5,5 (0–39) | 0 (0–706) | %0 | %39 (%22–%51) | %11 (%3,9–%35) | 0 (0–0,5) |
| 44 | 0,5 (0–27,5) | 0 (0–858) | %0 | %35 (%19–%41) | %13 (%2,8–%33) | 0 (0–1) |
| 45 | 4,5 (0–25,5) | 0 (0–920) | %0 | %35 (%20–%40) | %11 (%4,5–%31) | 0 (0–1) |
| 46 | 5 (0–23,5) | 0 (0–611) | %0 | %36 (%24–%39) | %12 (%2,7–%27) | 0 (0–1) |
| 47 | 6,5 (0–33) | 0 (0–566) | %0 | %34 (%20–%42) | %12 (%3,7–%27) | 0 |
| 48 | 0 (0–23,5) | 0 (0–1012) | %0 | %36 (%20–%45) | %12 (%5,2–%29) | 0 (0–0,5) |
| 49 | 8,5 (0–35) | 0 (0–508) | %0 | %36 (%20–%48) | %10 (%2,2–%34) | 0 (0–1) |
| 50 | 3 (0–28) | 0 (0–758) | %0 | %38 (%19–%41) | %11 (%4–%34) | 0 (0–1) |
| 51 | 0 (0–40,5) | 0 (0–830) | %0 | %35 (%21–%45) | %12 (%5,1–%35) | 0 |
| 52 | 1 (0–33) | 0 (0–742) | %0 | %36 (%20–%45) | %11 (%5,9–%35) | 0 |
| 53 | 1 (0–56,5) | 0 (0–673) | %0 | %38 (%24–%44) | %12 (%6,2–%33) | 0 |
| 54 | 0 (0–17,5) | 0 (0–777) | %0 | %35 (%19–%49) | %11 (%4,2–%35) | 0 |
| 55 | 1 (0–42,5) | 0 (0–818) | %0 | %32 (%21–%49) | %11 (%4,5–%41) | 0 (0–1) |
| 56 | 2 (0–31,5) | 0 (0–627) | %0 | %34 (%23–%51) | %10 (%5,6–%36) | 0 (0–0,5) |
| 57 | 10 (0–48,5) | 0 (0–926) | %0 | %33 (%21–%43) | %9,5 (%4,6–%23) | 0 |
| 58 | 11 (0–46) | 0 (0–746) | %0 | %31 (%21–%41) | %11 (%4,3–%22) | 0 (0–1) |
| 59 | 7 (0–31,5) | 0 (0–932) | %0 | %33 (%19–%42) | %12 (%3,6–%25) | 0 (0–1) |
| 60 | 0 (0–40,5) | 0 (0–996) | %0 | %29 (%20–%42) | %11 (%4–%21) | 0 |

### Altın ve ambar (3/3)

| Yıl | Açlıktan ölen | Kıtlık yardımı (sevkiyat) | Kıtlıkta yüz çeviren | Kıtlık akını | Ambarın yettiği gün (medeniyet medyanı) |
|---|---|---|---|---|---|
| 1 | 0 | 0 | 0 | 0 | 45,8 (26,3–53) |
| 2 | 0 (0–21) | 0 (0–1,5) | 0 (0–0,5) | 0 | 46,3 (26,8–53,1) |
| 3 | 0 (0–22) | 0 | 0 | 0 | 44 (27–56) |
| 4 | 0 (0–37,5) | 0 | 0 | 0 | 45,3 (22,4–56,1) |
| 5 | 0 (0–17,5) | 0 | 0 | 0 | 42 (28,2–53,5) |
| 6 | 0 (0–11,5) | 0 (0–1,5) | 0 (0–2) | 0 (0–0,5) | 45,2 (28,5–54,6) |
| 7 | 0 (0–9) | 0 | 0 | 0 (0–0,5) | 45,6 (30,9–58,6) |
| 8 | 0 (0–5,5) | 0 | 0 (0–1,5) | 0 | 44,8 (31,2–58,1) |
| 9 | 0 (0–4,5) | 0 | 0 | 0 | 39,8 (30,2–53) |
| 10 | 0 (0–14) | 0 | 0 | 0 | 40,5 (29,7–53) |
| 11 | 0 (0–20) | 0 | 0 | 0 | 42,9 (25,9–53,4) |
| 12 | 0 (0–21) | 0 | 0 | 0 | 41,8 (22–54,3) |
| 13 | 0 (0–16) | 0 | 0 | 0 (0–1) | 43,4 (20,9–51,7) |
| 14 | 0 (0–11,5) | 0 (0–0,5) | 0 (0–1) | 0 (0–0,5) | 42,6 (18,3–56,1) |
| 15 | 0 (0–26) | 0 | 0 | 0 | 42,4 (18,5–52) |
| 16 | 0 (0–18) | 0 | 0 (0–0,5) | 0 (0–0,5) | 38,5 (16,6–54,7) |
| 17 | 0 (0–20,5) | 0 | 0 | 0 | 34,8 (19,6–46,3) |
| 18 | 0 (0–19) | 0 | 0 | 0 (0–0,5) | 34,9 (24,5–50,2) |
| 19 | 0 (0–18) | 0 | 0 (0–1,5) | 0 | 33,3 (27,3–46,2) |
| 20 | 0 (0–10,5) | 0 | 0 | 0 (0–0,5) | 39,6 (25–51,8) |
| 21 | 0 (0–11) | 0 (0–1,5) | 0 (0–2,5) | 0 | 40 (21,3–49,6) |
| 22 | 0 (0–18,5) | 0 (0–0,5) | 0 (0–0,5) | 0 (0–1) | 35,3 (24,4–49,8) |
| 23 | 0 (0–15,5) | 0 | 0 (0–1) | 0 (0–1) | 35,3 (25,2–49) |
| 24 | 0 (0–11,5) | 0 | 0 | 0 (0–0,5) | 36,9 (18,9–49,1) |
| 25 | 0 (0–19,5) | 0 | 0 | 0 | 36,5 (24,6–47,3) |
| 26 | 0 (0–15,5) | 0 (0–1) | 0 | 0 | 33 (25,7–45,3) |
| 27 | 0 (0–11,5) | 0 | 0 | 0 (0–0,5) | 33 (23,3–41,4) |
| 28 | 0 (0–14,5) | 0 | 0 | 0 | 28,2 (19,2–41) |
| 29 | 0 (0–35) | 0 | 0 | 0 | 25,7 (16,1–42,6) |
| 30 | 0 (0–22,5) | 0 (0–2) | 0 (0–0,5) | 0 (0–0,5) | 29,3 (12,2–42) |
| 31 | 0 (0–13) | 0 (0–2) | 0 (0–0,5) | 0 (0–0,5) | 28,6 (15,8–46,6) |
| 32 | 0 (0–9,5) | 0 (0–1) | 0 (0–1) | 0 | 28,5 (16,4–37,7) |
| 33 | 0 (0–36) | 0 (0–0,5) | 0 (0–0,5) | 0 (0–1,5) | 27 (17,3–41) |
| 34 | 0 (0–16,5) | 0 | 0 | 0 | 28,6 (15,8–43,1) |
| 35 | 1,5 (0–17) | 0 | 0 | 0 | 31,1 (14,8–51,3) |
| 36 | 0 (0–15) | 0 (0–1) | 0 (0–1,5) | 0 (0–0,5) | 24,6 (11,9–49,2) |
| 37 | 0 (0–9,5) | 0 (0–3) | 0 (0–1) | 0 (0–0,5) | 24,3 (12,7–47) |
| 38 | 0 (0–13,5) | 0 (0–1,5) | 0 (0–0,5) | 0 | 27,8 (12,8–46,2) |
| 39 | 0 (0–12) | 0 | 0 | 0 | 28,2 (12,5–46) |
| 40 | 0 (0–15) | 0 | 0 | 0 | 26,9 (10,8–46) |
| 41 | 0 (0–14,5) | 0 (0–2) | 0 (0–2,5) | 0 | 26,6 (9,34–40,1) |
| 42 | 0 (0–15) | 0 (0–1,5) | 0 (0–1) | 0 | 28,3 (6,55–46,1) |
| 43 | 0 (0–23,5) | 0 (0–4,5) | 0 | 0 (0–0,5) | 24,7 (10,1–43) |
| 44 | 1 (0–19) | 0 (0–2,5) | 0 (0–3,5) | 0 (0–0,5) | 25 (9,38–38,9) |
| 45 | 0,5 (0–25,5) | 0 (0–2) | 0 (0–3) | 0 (0–0,5) | 29,6 (9,06–41,6) |
| 46 | 4 (0–16) | 0 (0–4,5) | 0 (0–2) | 0 | 29,3 (11,6–40,8) |
| 47 | 2 (0–33) | 0 | 0 | 0 (0–0,5) | 28,4 (9,56–37,7) |
| 48 | 1 (0–15,5) | 0 (0–1) | 0 | 0 (0–1) | 28,3 (16,9–39,2) |
| 49 | 1 (0–17,5) | 0 (0–3,5) | 0 (0–0,5) | 0 | 26 (10,2–39,3) |
| 50 | 1 (0–18,5) | 0 (0–3) | 0 (0–3) | 0 | 28,1 (9,6–35,5) |
| 51 | 5 (0–23) | 0 (0–0,5) | 0 (0–0,5) | 0 (0–0,5) | 21,9 (6,88–37,2) |
| 52 | 5 (0–16) | 0 | 0 | 0 (0–0,5) | 21,2 (5,96–35,9) |
| 53 | 7 (0–21,5) | 0 | 0 | 0 (0–1) | 20,1 (5,44–38,9) |
| 54 | 4,5 (0–17) | 0 | 0 (0–0,5) | 0 (0–2) | 20,4 (6,41–38,3) |
| 55 | 4 (0–14) | 0 | 0 (0–2) | 0 (0–1) | 21 (7,51–39,3) |
| 56 | 6,5 (0–16,5) | 0 (0–1,5) | 0 (0–0,5) | 0 (0–1,5) | 26,3 (6,94–37,4) |
| 57 | 2 (0–12) | 0 (0–2) | 0 (0–0,5) | 0 | 24,3 (7,5–38,5) |
| 58 | 2,5 (0–11) | 0 (0–3) | 0 (0–4) | 0 (0–1) | 25,8 (8,83–34) |
| 59 | 2 (0–23,5) | 0 (0–2) | 0 (0–2) | 0 (0–1) | 24,3 (11,1–37,3) |
| 60 | 1 (0–13,5) | 0 | 0 | 0 (0–1) | 24,7 (12,8–36,2) |

### Yerleşim kademesi

| Yıl | Ortalama yerleşim kademesi | Köy+ yerleşim | Kasaba+ yerleşim | Şehir | Ortalama başkent kademesi | Kademe değişimi (yerleşim, yıl içinde) |
|---|---|---|---|---|---|---|
| 1 | 1,14 (0,9–1,29) | 63 (50–67,5) | 20,5 (13,5–29) | 5 (4–6) | 2,63 (2,39–2,83) | 31,5 (19,5–43) |
| 2 | 1,16 (0,93–1,22) | 63,5 (50,5–70) | 19,5 (14–29) | 5 (4–6) | 2,62 (2,31–2,77) | 28 (22,5–38,5) |
| 3 | 1,16 (0,9–1,26) | 62,5 (47–67,5) | 21,5 (15–28,5) | 5 (4–6) | 2,62 (2,31–2,77) | 31,5 (21,5–37,5) |
| 4 | 1,13 (0,98–1,3) | 62,5 (49–68,5) | 21 (15,5–30) | 5,5 (4–6) | 2,5 (2,29–2,82) | 29 (19,5–34) |
| 5 | 1,15 (0,97–1,27) | 63 (50,5–68) | 20,5 (15,5–30) | 5 (4–6) | 2,54 (2,27–2,77) | 30,5 (23,5–37,5) |
| 6 | 1,19 (0,96–1,28) | 64 (50,5–70) | 23 (15–28) | 5 (4,5–6) | 2,5 (2,19–2,69) | 31 (21,5–37) |
| 7 | 1,18 (0,97–1,3) | 66 (52–71) | 22 (15–30,5) | 5 (4–6) | 2,5 (2,21–2,77) | 28,5 (18,5–36) |
| 8 | 1,2 (1,01–1,29) | 65 (53,5–70,5) | 22,5 (14–31) | 6 (5–6) | 2,54 (2,31–2,82) | 28 (23–43) |
| 9 | 1,13 (0,97–1,33) | 61,5 (51,5–69,5) | 22,5 (13,5–32) | 5,5 (4–6) | 2,57 (2,29–2,76) | 26,5 (18–39) |
| 10 | 1,14 (0,96–1,35) | 62 (51,5–69) | 24 (14,5–33,5) | 5 (4–6) | 2,5 (2,07–2,77) | 29,5 (20,5–37,5) |
| 11 | 1,18 (1,02–1,33) | 64 (53–69) | 21 (14,5–31,5) | 5 (4–6) | 2,5 (2,15–2,85) | 31 (18,5–40) |
| 12 | 1,15 (0,99–1,29) | 66 (48,5–69) | 21,5 (14–32) | 5 (3,5–6) | 2,54 (2,17–2,83) | 26,5 (20–38,5) |
| 13 | 1,13 (0,95–1,33) | 65 (46,5–71,5) | 20,5 (13–32,5) | 5 (3,5–5,5) | 2,54 (2,21–2,77) | 27,5 (20–38) |
| 14 | 1,17 (0,9–1,35) | 65 (49–70) | 22 (11,5–34) | 5,5 (4,5–6) | 2,5 (2,21–2,77) | 27 (22,5–38) |
| 15 | 1,19 (0,88–1,3) | 65,5 (49,5–71) | 22 (12,5–31) | 6 (4–6) | 2,57 (2,31–2,77) | 26 (22,5–36) |
| 16 | 1,12 (0,87–1,31) | 66,5 (44,5–72,5) | 20,5 (11,5–27,5) | 6 (4–6) | 2,57 (2,4–2,82) | 31 (18–35,5) |
| 17 | 1,17 (0,87–1,32) | 63,5 (45,5–74,5) | 21,5 (12,5–29,5) | 6 (4–6) | 2,57 (2,4–2,82) | 24,5 (17,5–35) |
| 18 | 1,14 (0,88–1,33) | 61,5 (48,5–73) | 19,5 (12,5–33) | 6 (4–6) | 2,54 (2,29–2,82) | 27,5 (15–35) |
| 19 | 1,15 (0,93–1,33) | 64,5 (46,5–73,5) | 19,5 (12,5–34) | 5 (5–6) | 2,67 (2,36–2,83) | 26,5 (12,5–32) |
| 20 | 1,13 (0,83–1,37) | 61,5 (43–71,5) | 18,5 (10,5–33) | 5 (4–6) | 2,57 (2,29–2,77) | 26,5 (15,5–36,5) |
| 21 | 1,14 (0,83–1,35) | 62,5 (42,5–71,5) | 19,5 (11–30) | 5 (4–6) | 2,63 (2,38–2,83) | 26 (18–32) |
| 22 | 1,15 (0,82–1,32) | 64 (44–72,5) | 20 (10,5–31) | 5,5 (4–6) | 2,65 (2,29–2,83) | 26 (20–30,5) |
| 23 | 1,11 (0,84–1,32) | 65 (45,5–71,5) | 17,5 (10–29) | 6 (4–6) | 2,67 (2,43–2,83) | 24,5 (20,5–38) |
| 24 | 1,08 (0,79–1,35) | 62,5 (45–73,5) | 18 (10,5–30) | 5,5 (4–6) | 2,57 (2,31–2,83) | 27 (17–36,5) |
| 25 | 1,03 (0,76–1,34) | 59 (41,5–73) | 17 (8–30) | 5 (3,5–6) | 2,57 (2,24–2,83) | 25 (18–33) |
| 26 | 1,01 (0,81–1,32) | 58 (42–70,5) | 16 (8–30,5) | 5 (4–6) | 2,57 (2,31–2,77) | 24,5 (17–39,5) |
| 27 | 1,01 (0,84–1,33) | 56,5 (46–72,5) | 17 (9–29,5) | 6 (3,5–6) | 2,67 (2,29–2,77) | 23,5 (18,5–30) |
| 28 | 1,05 (0,82–1,27) | 57,5 (45–69,5) | 18,5 (8,5–28,5) | 5,5 (4–6) | 2,5 (2,29–2,83) | 29 (16–34) |
| 29 | 1,07 (0,78–1,28) | 59,5 (43,5–73) | 17 (8–28) | 5 (4–6) | 2,67 (2,46–2,83) | 23,5 (10–31) |
| 30 | 1,04 (0,84–1,28) | 61 (47–71,5) | 16,5 (9–28,5) | 5 (3,5–6) | 2,67 (2,26–2,82) | 25,5 (16–36,5) |
| 31 | 1,04 (0,86–1,3) | 58 (46,5–71,5) | 18 (10–28,5) | 6 (4–6) | 2,67 (2,36–2,83) | 23,5 (12–31,5) |
| 32 | 1,04 (0,84–1,29) | 59,5 (45,5–68,5) | 16,5 (9,5–29) | 6 (5–6) | 2,67 (2,31–2,92) | 21,5 (16–30,5) |
| 33 | 1,06 (0,91–1,29) | 60 (48,5–72) | 16,5 (10–26) | 6 (4–6) | 2,57 (2,46–2,83) | 26 (17–33,5) |
| 34 | 1,07 (0,91–1,33) | 60,5 (50,5–73) | 16,5 (12–28,5) | 5,5 (4,5–6) | 2,59 (2,46–2,85) | 21 (11,5–29,5) |
| 35 | 1,08 (0,84–1,39) | 63 (47,5–72,5) | 16,5 (10,5–33,5) | 6 (4,5–6) | 2,67 (2,5–2,79) | 22 (11–32,5) |
| 36 | 1,02 (0,84–1,36) | 61 (49–73) | 16 (9,5–31,5) | 5 (3–6) | 2,59 (2,36–2,85) | 24,5 (15,5–32) |
| 37 | 1,04 (0,85–1,4) | 61,5 (48–72) | 17,5 (8–35,5) | 6 (5–6) | 2,67 (2,5–2,85) | 20 (13–32) |
| 38 | 1,06 (0,85–1,36) | 62,5 (51–73) | 16 (7,5–31,5) | 6 (4–6) | 2,54 (2,31–2,93) | 22,5 (16,5–34,5) |
| 39 | 1,08 (0,83–1,36) | 64 (49,5–72,5) | 16 (10–31) | 6 (5–6) | 2,67 (2,3–2,92) | 23,5 (13,5–30) |
| 40 | 1,08 (0,88–1,38) | 63 (47–74,5) | 17,5 (8,5–32) | 6 (4–6) | 2,5 (2,34–2,83) | 22 (12,5–33) |
| 41 | 1,05 (0,89–1,37) | 61,5 (50,5–74,5) | 15 (11–31,5) | 6 (4–6) | 2,65 (2,46–2,77) | 21,5 (13–30,5) |
| 42 | 1,04 (0,93–1,38) | 62 (52–75,5) | 16 (10,5–30) | 6 (4–6) | 2,65 (2,31–2,71) | 19 (15–36) |
| 43 | 1,06 (0,88–1,34) | 58,5 (51,5–75) | 17,5 (11–29,5) | 5 (4–6) | 2,57 (2,31–2,83) | 24 (18,5–33) |
| 44 | 1,06 (0,88–1,32) | 61 (51,5–72,5) | 17 (10–28,5) | 5 (4–6) | 2,57 (2,31–2,82) | 25,5 (14–34) |
| 45 | 1,07 (0,91–1,3) | 63 (51–74,5) | 16 (10–27,5) | 5 (4–6) | 2,54 (2,43–2,76) | 22,5 (15,5–35) |
| 46 | 1,05 (0,91–1,3) | 61 (52–73) | 17 (8,5–26) | 6 (4,5–6) | 2,67 (2,43–2,82) | 25,5 (11–33,5) |
| 47 | 1,08 (0,93–1,33) | 60,5 (50,5–72,5) | 17 (9–29) | 6 (4–6) | 2,56 (2,36–2,76) | 19,5 (14–28,5) |
| 48 | 1,06 (0,98–1,38) | 61,5 (52–73) | 15 (10–33,5) | 6 (4–6) | 2,57 (2,43–2,71) | 26,5 (17,5–32,5) |
| 49 | 1,07 (0,92–1,32) | 63 (50,5–71,5) | 17 (9,5–30) | 5 (3–6) | 2,6 (2,29–2,77) | 25,5 (16,5–31) |
| 50 | 1,05 (0,9–1,36) | 61 (50,5–73) | 15 (9–30) | 5 (4–6) | 2,57 (2,36–2,71) | 23 (16–28,5) |
| 51 | 1,07 (0,88–1,3) | 63,5 (52–73) | 12,5 (8–27) | 6 (4–6) | 2,57 (2,43–2,83) | 21,5 (9,5–30) |
| 52 | 1,11 (0,86–1,29) | 63,5 (48,5–73,5) | 16 (9,5–27,5) | 6 (4–6) | 2,69 (2,36–2,83) | 23 (16–31) |
| 53 | 1,1 (0,83–1,26) | 64 (47,5–74) | 16,5 (8,5–23,5) | 6 (5–6) | 2,69 (2,46–2,83) | 21,5 (12,5–26) |
| 54 | 1,08 (0,86–1,27) | 63 (47–73,5) | 16 (9,5–26,5) | 5,5 (4–6) | 2,6 (2,46–2,83) | 22 (10,5–37,5) |
| 55 | 1,08 (0,91–1,26) | 64 (49–74) | 15 (8–23,5) | 6 (5–6) | 2,67 (2,39–2,86) | 22,5 (11,5–35,5) |
| 56 | 1,08 (0,87–1,31) | 64,5 (48–72) | 16,5 (9,5–25,5) | 5,5 (5–6) | 2,69 (2,27–2,86) | 23 (9–32,5) |
| 57 | 1,1 (0,83–1,29) | 63,5 (46,5–71) | 15 (8,5–26) | 6 (5–6) | 2,67 (2,27–2,85) | 25 (16–32) |
| 58 | 1,06 (0,87–1,27) | 62 (47,5–73) | 15 (9,5–24,5) | 6 (5–6) | 2,69 (2,39–2,86) | 21,5 (10,5–26,5) |
| 59 | 1,09 (0,84–1,27) | 64 (45,5–72) | 16,5 (9–25) | 6 (4,5–6) | 2,67 (2,29–2,86) | 24 (12,5–32,5) |
| 60 | 1,08 (0,88–1,31) | 63,5 (47,5–72,5) | 14 (10,5–28) | 5 (5–6) | 2,65 (2,38–2,8) | 18,5 (13–31,5) |

### Deniz

| Yıl | Liman (tersane) | Gemi (koga/tekne) | Kadırga | Denizaşırı yerleşim | Deniz ticaret yolu (yıl sonu) | Deniz seferi (ticaret) |
|---|---|---|---|---|---|---|
| 1 | 20 (18–22,5) | 44 (39,5–50,5) | 19,5 (15,5–24,5) | 8 (4,5–9,5) | 13 (11–15,5) | 20 (14,5–30,5) |
| 2 | 20 (18–23,5) | 44 (38,5–48,5) | 19 (16–24,5) | 7,5 (4,5–9,5) | 13 (11–15,5) | 19,5 (14,5–32,5) |
| 3 | 21 (18–24) | 44 (39–49,5) | 20 (15,5–25) | 7,5 (4,5–9) | 13,5 (11–15,5) | 20,5 (15–32) |
| 4 | 20,5 (17,5–23,5) | 44 (35,5–49,5) | 20 (15,5–25) | 7,5 (4,5–10) | 14,5 (10–16) | 20 (11,5–31) |
| 5 | 21 (17,5–23,5) | 42,5 (37,5–51,5) | 21 (16–26,5) | 7,5 (4,5–10) | 14 (10–17) | 19,5 (13,5–35) |
| 6 | 22 (16–24,5) | 44,5 (34,5–50) | 20 (16,5–27) | 8 (4,5–10) | 13,5 (10,5–16,5) | 18,5 (14,5–31,5) |
| 7 | 22 (18–24) | 43,5 (40–50) | 20 (16,5–28,5) | 8 (4,5–11) | 14,5 (10,5–18,5) | 22 (11–30) |
| 8 | 20,5 (18–24) | 43 (39–51,5) | 21 (16–28,5) | 8,5 (4–10,5) | 14 (10,5–19,5) | 20 (14–33,5) |
| 9 | 20,5 (18–24) | 45,5 (39–53) | 21,5 (15,5–28) | 9 (4–10) | 14 (10,5–19) | 19 (12,5–34) |
| 10 | 21,5 (18–25) | 44,5 (38,5–52,5) | 21,5 (15,5–27,5) | 8 (4–10,5) | 14 (10–19,5) | 20,5 (11–35,5) |
| 11 | 21 (18,5–25,5) | 44 (38–52,5) | 21,5 (15,5–27) | 8 (4–10,5) | 14,5 (10–19,5) | 23 (12–36,5) |
| 12 | 22,5 (18,5–26) | 46,5 (39–52,5) | 21 (15,5–26,5) | 8,5 (4–10,5) | 15 (10,5–19,5) | 20,5 (14–38,5) |
| 13 | 22,5 (18,5–25,5) | 46 (39–51) | 23 (15,5–25,5) | 8,5 (4–10,5) | 15 (10,5–20) | 21,5 (13,5–38,5) |
| 14 | 22,5 (19–26) | 46 (39,5–52) | 22,5 (16,5–26) | 8,5 (4–10,5) | 14,5 (11–20,5) | 23 (15–39,5) |
| 15 | 21,5 (18,5–26) | 46,5 (39,5–54) | 22 (15–26) | 8,5 (4–10) | 15,5 (11,5–19,5) | 22,5 (13,5–36) |
| 16 | 21,5 (17,5–25,5) | 47 (40–54,5) | 21,5 (15–25,5) | 8 (4–10,5) | 14,5 (11,5–19,5) | 21 (12–37) |
| 17 | 22,5 (18,5–25) | 47 (39–55) | 23,5 (14–25,5) | 7,5 (4–10) | 15 (11,5–19,5) | 20 (13–36,5) |
| 18 | 23 (19,5–26) | 48,5 (39–54,5) | 23 (14–25) | 8,5 (4–10,5) | 16,5 (11,5–20) | 22 (15–35,5) |
| 19 | 23 (18,5–26) | 46,5 (38,5–54,5) | 22,5 (14,5–25) | 8,5 (4–10) | 16,5 (10,5–20) | 25 (11,5–34) |
| 20 | 22,5 (19–26) | 47,5 (39–54,5) | 21,5 (15–25) | 8,5 (4–10) | 17 (10,5–20) | 25,5 (13,5–38) |
| 21 | 22 (18–26) | 47 (39,5–55) | 21,5 (16,5–25) | 8 (4–10) | 17 (10–20) | 27 (13,5–36) |
| 22 | 23 (19–26) | 47,5 (40,5–56,5) | 21,5 (18,5–25) | 7,5 (4–10,5) | 17 (10–20,5) | 24,5 (13,5–36,5) |
| 23 | 23 (18,5–26) | 49 (41–55) | 21,5 (18–25) | 7,5 (4–10) | 17 (10–21,5) | 25 (11,5–38,5) |
| 24 | 22,5 (18–24,5) | 49,5 (41,5–54,5) | 21,5 (18–25) | 7,5 (4–10) | 17 (10,5–20) | 22 (14,5–36) |
| 25 | 22 (18–24,5) | 47 (39,5–55) | 21,5 (17–25,5) | 7,5 (4–10) | 17 (10,5–20) | 23,5 (12–35,5) |
| 26 | 23 (18,5–26) | 48,5 (40,5–54,5) | 23 (17–25,5) | 7,5 (4–11) | 17 (10–21,5) | 24 (13–34) |
| 27 | 22,5 (18–26) | 48,5 (40,5–54,5) | 22,5 (17–25,5) | 7,5 (4–11) | 17 (10–21,5) | 25,5 (13–42,5) |
| 28 | 22,5 (18,5–26) | 48 (40–56) | 22,5 (16,5–25,5) | 7,5 (4–10,5) | 17 (10–21,5) | 25,5 (14,5–40,5) |
| 29 | 23 (18,5–25,5) | 48 (41–56) | 22,5 (15,5–25,5) | 7,5 (4–10,5) | 17,5 (9,5–23) | 27 (13–36) |
| 30 | 23 (18–25,5) | 49 (41,5–54,5) | 22 (16–25,5) | 7,5 (4–10,5) | 17,5 (9–21) | 26 (12,5–41) |
| 31 | 23 (18–26) | 49 (42,5–55,5) | 21,5 (15,5–25,5) | 7,5 (4–10) | 16 (9,5–21) | 26,5 (17–38) |
| 32 | 23 (18–26,5) | 46 (40,5–55) | 21 (15,5–25) | 7,5 (4–10,5) | 16 (9,5–22) | 24 (13–37) |
| 33 | 22,5 (18–26) | 49 (40,5–53) | 21 (15,5–25) | 7,5 (4–11) | 16 (9,5–22,5) | 23 (18–40,5) |
| 34 | 22 (19–26) | 49 (42,5–53,5) | 20 (14,5–25) | 7 (3,5–10) | 15 (9,5–20,5) | 26 (13,5–32,5) |
| 35 | 21,5 (19–26) | 48 (41,5–53,5) | 20 (15–24,5) | 7 (3,5–10) | 15,5 (9,5–20,5) | 26 (13–33) |
| 36 | 22 (19,5–26) | 47,5 (42–53) | 19,5 (15,5–24) | 7 (3,5–10) | 15 (9,5–20) | 25 (11–34,5) |
| 37 | 22 (19–26,5) | 48 (42–54,5) | 19,5 (15,5–24) | 7 (3,5–10) | 15 (9,5–20) | 24 (13,5–34) |
| 38 | 22,5 (19–27) | 48 (40,5–55) | 20,5 (14,5–24) | 7 (3,5–10) | 15 (9,5–20) | 24 (12,5–34,5) |
| 39 | 22,5 (19–26,5) | 48,5 (41,5–56) | 20,5 (14,5–24) | 7 (3,5–10) | 16 (9,5–20) | 26 (13,5–33) |
| 40 | 22,5 (19–25,5) | 48 (40,5–57) | 20,5 (14,5–25) | 7 (3,5–10) | 15 (9,5–21,5) | 24,5 (10,5–38) |
| 41 | 23 (18,5–26) | 47,5 (40–55,5) | 20,5 (14,5–24) | 7,5 (3,5–10) | 15 (10–22) | 25,5 (10–42) |
| 42 | 23 (19,5–26,5) | 49 (40–55,5) | 21 (14,5–25,5) | 7,5 (3,5–9,5) | 14,5 (9,5–22,5) | 24,5 (11,5–42,5) |
| 43 | 23,5 (18,5–26) | 49 (41–56) | 21 (14,5–25,5) | 7,5 (3,5–10) | 14,5 (8,5–24) | 25 (11,5–45,5) |
| 44 | 23 (19–27) | 49 (40,5–56) | 21 (14,5–26) | 7 (4–10) | 14,5 (8,5–24) | 24,5 (12–46) |
| 45 | 22 (20–26,5) | 47 (40–56,5) | 21 (14,5–26) | 7 (4–9,5) | 15 (9–24,5) | 21 (8–41) |
| 46 | 22 (20–27) | 46,5 (39–57,5) | 21 (14–25) | 7 (4–10) | 15,5 (9–24) | 25 (8,5–41) |
| 47 | 22 (20,5–27) | 49,5 (40–58,5) | 20,5 (14–25,5) | 7 (4–10) | 15,5 (9,5–24,5) | 26,5 (11–45,5) |
| 48 | 22 (20–27) | 47 (40,5–58) | 20,5 (13,5–25,5) | 7 (4–10) | 15,5 (9,5–25) | 22 (11,5–42,5) |
| 49 | 22 (19,5–27,5) | 48,5 (40,5–57) | 20,5 (13,5–25) | 7 (4–10) | 15,5 (9,5–25) | 24,5 (10–47,5) |
| 50 | 22 (19,5–28) | 49 (39,5–57) | 20,5 (13,5–25) | 7 (4–10) | 16 (9,5–25) | 27 (10–35,5) |
| 51 | 21,5 (19,5–27,5) | 50 (39,5–54,5) | 19 (13,5–24,5) | 7 (4–10) | 16 (9,5–24) | 25,5 (11,5–41) |
| 52 | 21,5 (19,5–26,5) | 52 (41–55,5) | 20 (13–24,5) | 6,5 (4–10) | 16 (9,5–25) | 30,5 (11,5–41,5) |
| 53 | 21,5 (19,5–26,5) | 51 (42–56) | 20,5 (13–24,5) | 6,5 (4–10,5) | 16 (9,5–25,5) | 29,5 (11–44,5) |
| 54 | 22 (19–26,5) | 50,5 (40,5–56) | 21 (13–24) | 6,5 (4–10) | 16 (9–25) | 27,5 (12,5–46,5) |
| 55 | 23 (19,5–26,5) | 51 (42,5–58) | 20 (13–24) | 6,5 (4–11) | 17 (9–25,5) | 29 (11–47,5) |
| 56 | 23 (19–26) | 51,5 (43–59,5) | 19 (12,5–23,5) | 6,5 (4–11) | 17 (9,5–25,5) | 29,5 (9–45) |
| 57 | 23 (19–26,5) | 51 (42,5–60,5) | 19 (10,5–23,5) | 6,5 (4–10,5) | 17 (9,5–26,5) | 32 (9–52) |
| 58 | 22 (19–27) | 53,5 (41,5–60,5) | 18,5 (9,5–23,5) | 6,5 (4–11) | 17 (9,5–26) | 34 (12,5–47,5) |
| 59 | 22 (19–27) | 53,5 (42–62) | 18,5 (9,5–23) | 6,5 (4–11) | 18 (9,5–26) | 33,5 (11,5–48) |
| 60 | 22 (19–27) | 52,5 (43–62) | 18,5 (9,5–23) | 6,5 (4–10,5) | 17,5 (9,5–23,5) | 35 (16–44,5) |

### v3: durum değişimi (1/2)

| Yıl | Yaşayan yerleşim (yıl ort.) | Büyük şehir (Şehir kademesi, yıl ort.) | El değiştiren yerleşim (fetih + bölünme) | El değiştiren büyük şehir | Büyük şehre hücum (kuşatma muharebesi) | Büyük şehir yağmalandı, tutulmadı |
|---|---|---|---|---|---|---|
| 1 | 76,5 (74–78,2) | 4,86 (3,96–5,84) | 4 (2,5–5) | 1 (0,5–2,5) | 0 (0–1) | 0 |
| 2 | 77,7 (72,8–80) | 5,19 (3,79–6) | 3 (1,5–6) | 1 (0–2,5) | 0,5 (0–1) | 0 (0–1) |
| 3 | 78 (72,8–79,9) | 5,06 (3,9–6) | 3 (1–5) | 0,5 (0–2,5) | 0 (0–1) | 0 (0–1) |
| 4 | 78 (71–79,9) | 5,35 (3,85–6) | 4 (2–4) | 1 (0–2) | 0 (0–2) | 0 |
| 5 | 78 (70,1–79,5) | 5,69 (3,99–6) | 3,5 (2–5,5) | 1 (0–2) | 0 (0–1) | 0 (0–1) |
| 6 | 78 (69,7–79,8) | 5,39 (3,91–5,9) | 3,5 (2–5,5) | 1 (0–2) | 0 (0–1) | 0 (0–1) |
| 7 | 78,5 (69,6–79,9) | 5,34 (4,19–5,94) | 3 (1,5–4) | 1 (0–2) | 0 (0–3) | 0 (0–0,5) |
| 8 | 78,4 (71,2–80) | 5,75 (4,44–6) | 3 (1,5–4,5) | 0,5 (0–2) | 0 (0–1,5) | 0 |
| 9 | 78,5 (71,2–80) | 5,78 (4,43–5,94) | 3 (1–5,5) | 1 (0–1,5) | 0 (0–1,5) | 0 (0–0,5) |
| 10 | 78,4 (72–79,9) | 5,26 (4,19–5,93) | 2 (1–3) | 0 (0–1) | 0 (0–1) | 0 (0–0,5) |
| 11 | 78,2 (72,8–79,5) | 5,06 (4,15–5,84) | 3 (1–5) | 0,5 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 12 | 78,5 (71,1–80,3) | 4,89 (3,91–5,98) | 2 (1–5) | 1 (0–2) | 0 (0–2) | 0 (0–1) |
| 13 | 78,1 (71,1–80,5) | 4,78 (3,54–5,94) | 3 (1–4,5) | 1 (0–1,5) | 1 (0–3) | 0 (0–2) |
| 14 | 78 (71–80,3) | 5,09 (4,06–6) | 3 (1–5,5) | 1 (0–3) | 0 (0–1) | 0 |
| 15 | 77,5 (71,2–80) | 5,49 (4,35–6) | 3 (2–4) | 1 (0–2) | 0 (0–2) | 0 (0–1,5) |
| 16 | 77,8 (71,1–80,3) | 5,43 (4,36–6) | 3,5 (0,5–4,5) | 1 (0–2,5) | 0 (0–1,5) | 0 |
| 17 | 78,3 (68,7–80) | 5,76 (4,21–6) | 2 (1–4,5) | 0,5 (0–1,5) | 0 (0–2) | 0 (0–1,5) |
| 18 | 78,4 (68,4–80) | 5,59 (4,23–6) | 3,5 (2–4,5) | 1 (0–2,5) | 0 (0–1) | 0 (0–0,5) |
| 19 | 78,2 (67,7–80) | 5,41 (4,26–6) | 3 (0,5–5) | 1 (0–2) | 0 (0–1) | 0 (0–1) |
| 20 | 77,9 (67,1–79,5) | 5 (4,31–6) | 2 (0,5–5,5) | 1 (0–2) | 0 (0–1,5) | 0 |
| 21 | 77,9 (67,5–79,8) | 5,15 (4,01–5,99) | 3 (0–6) | 0,5 (0–2) | 0 (0–1) | 0 |
| 22 | 78,3 (67,8–79,9) | 5,43 (4,06–6) | 3 (2–4) | 1 (0–2) | 0 (0–1) | 0 |
| 23 | 78,2 (67,5–79,6) | 5,84 (4,05–6) | 2,5 (0,5–4,5) | 1 (0–1,5) | 0 (0–1,5) | 0 (0–0,5) |
| 24 | 77,7 (66,9–80) | 5,58 (3,9–6) | 2,5 (1–5,5) | 1 (0–2,5) | 0 (0–1) | 0 (0–1) |
| 25 | 78 (69,1–80) | 5,69 (4,06–6) | 2 (0–4,5) | 0,5 (0–2,5) | 0 (0–1,5) | 0 (0–0,5) |
| 26 | 78 (68,4–80) | 5,1 (3,79–5,99) | 2,5 (0,5–4,5) | 1 (0–2,5) | 1 (0–2) | 0 (0–1) |
| 27 | 77,9 (68,8–80) | 5,56 (3,93–6) | 3 (1,5–5,5) | 1 (0–2) | 0 | 0 |
| 28 | 77,9 (70,6–79,9) | 5,46 (3,74–6) | 2 (0,5–4,5) | 0,5 (0–2) | 0 (0–1,5) | 0 (0–0,5) |
| 29 | 77,5 (70,5–79,5) | 5,46 (4,15–6) | 1 (1–4,5) | 0,5 (0–2) | 0 (0–1,5) | 0 (0–0,5) |
| 30 | 76,3 (71,8–79,5) | 5,24 (4,15–5,98) | 3 (1–6) | 0 (0–2) | 0 | 0 |
| 31 | 77,8 (73,1–79,8) | 5,7 (3,98–6) | 2 (0–6) | 1 (0–2) | 0 (0–1,5) | 0 (0–1) |
| 32 | 77,8 (73,2–79,5) | 5,75 (4,48–6) | 2,5 (1–5) | 1 (0–2) | 0 (0–2,5) | 0 (0–1) |
| 33 | 78,1 (72,6–79,4) | 5,68 (4,45–6) | 3 (0,5–3,5) | 1 (0–2) | 0 (0–2) | 0 (0–1) |
| 34 | 76,9 (71,3–79) | 5,76 (4,38–6) | 3 (1–6,5) | 1 (0–3) | 0,5 (0–2) | 0 (0–1) |
| 35 | 77,3 (72,6–79) | 5,88 (4,19–6) | 3 (1–5) | 1 (0–2) | 0 (0–1) | 0 |
| 36 | 77,7 (73,2–79) | 5,64 (4,21–6) | 3 (1–5) | 1 (0–2) | 0 (0–1,5) | 0 |
| 37 | 78 (73,9–79,5) | 5,7 (4,14–6) | 3 (1–5) | 1 (0–2) | 0 (0–1,5) | 0 |
| 38 | 78 (73,9–79,5) | 5,81 (4,41–6) | 2,5 (1–5) | 1 (0–2) | 0 (0–1) | 0 |
| 39 | 78,1 (74–79,5) | 5,78 (4,66–6) | 3 (1–5,5) | 1 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 40 | 78,2 (74,2–79,5) | 5,86 (4,63–6) | 4 (1,5–7) | 1 (0–3) | 0 (0–2) | 0 (0–1) |
| 41 | 78,2 (74,4–79,5) | 5,76 (4,36–6) | 2,5 (1–4,5) | 1 (0–2) | 0 (0–1) | 0 (0–1) |
| 42 | 77,5 (73,4–79,5) | 5,81 (4,1–6) | 3,5 (1,5–5) | 1 (0–2,5) | 0 (0–1,5) | 0 (0–1) |
| 43 | 77,7 (70,2–79,5) | 5,74 (4,15–6) | 3 (1,5–5) | 1 (0,5–2,5) | 0 (0–1) | 0 (0–1) |
| 44 | 77,4 (71,1–79,5) | 5,3 (3,94–6) | 3 (1–6) | 1 (0,5–2) | 0 (0–1) | 0 (0–0,5) |
| 45 | 77,3 (71,4–79,5) | 5,19 (3,68–6) | 2 (1–5,5) | 1 (0–2,5) | 0 (0–1,5) | 0 |
| 46 | 77,3 (69,7–80) | 5,6 (4,04–6) | 3 (1–6) | 1 (0–2,5) | 0 (0–1) | 0 (0–0,5) |
| 47 | 77,7 (69,7–79,5) | 5,65 (4,03–6) | 3 (1–5,5) | 1 (0–2) | 0 (0–0,5) | 0 (0–0,5) |
| 48 | 77,1 (70–79,5) | 5,58 (3,99–6) | 3 (1–5) | 1 (0–2,5) | 0 (0–1) | 0 (0–0,5) |
| 49 | 77,5 (70–79,4) | 5,68 (4,39–6) | 3,5 (1–5) | 1 (0–2,5) | 0 (0–1) | 0 (0–0,5) |
| 50 | 77,9 (69,6–79,7) | 5,29 (4,08–6) | 3 (1–4) | 1 (0–2) | 0 (0–2,5) | 0 (0–0,5) |
| 51 | 78,1 (70,2–80) | 5,68 (4–6) | 2 (0–5,5) | 1 (0–2) | 0 (0–1) | 0 |
| 52 | 78,2 (70,9–79,7) | 5,88 (4–6) | 4 (2–6) | 1 (0–2) | 0 (0–1,5) | 0 |
| 53 | 79 (72,8–80) | 5,83 (4,3–6) | 2,5 (0–4) | 0,5 (0–1,5) | 0 (0–1) | 0 (0–1) |
| 54 | 78 (72,1–80) | 5,49 (4,44–5,98) | 2 (1–4) | 0,5 (0–1) | 1 (0–2) | 0 (0–1) |
| 55 | 78 (70,6–79,9) | 5,66 (4,49–6) | 2,5 (1–6,5) | 2 (0–2,5) | 0 (0–1,5) | 0 (0–1) |
| 56 | 77,8 (70,6–79,5) | 5,69 (4,86–6) | 2 (0,5–5) | 1 (0–3) | 0 (0–1,5) | 0 |
| 57 | 78,1 (71–79,7) | 5,6 (4,66–6) | 3 (2–5) | 1 (0–2) | 0,5 (0–2) | 0 (0–0,5) |
| 58 | 78 (72,2–80) | 5,59 (4,68–6) | 3 (2–6,5) | 1 (0–2) | 0 (0–0,5) | 0 (0–0,5) |
| 59 | 77,8 (73,4–79,8) | 5,98 (4,91–6) | 3 (1–5,5) | 1 (0–2) | 0 (0–2) | 0 (0–0,5) |
| 60 | 77,6 (73,1–79,5) | 5,9 (5,03–6) | 3,5 (0,5–6) | 1 (0–3,5) | 0 (0–1,5) | 0 (0–0,5) |

### v3: durum değişimi (2/2)

| Yıl | Yerleşim durum değişimi (kuruluş hariç hepsi) | Orta halka kaydı (Köy/Kasaba: kademe değişimi ya da terk) | Açlık başlayan yerleşim | Salgın başlayan yerleşim | Yakılan/yanan yerleşim | Harabeye yeniden yerleşim |
|---|---|---|---|---|---|---|
| 1 | 49,5 (34–63) | 24,5 (13–34) | 0 (0–10) | 0 (0–1) | 8,5 (3,5–15) | 1 (0–2) |
| 2 | 51 (41–73,5) | 21 (14–29) | 0 (0–26,5) | 0 (0–1) | 10 (5,5–13,5) | 1 (0–2) |
| 3 | 50 (35,5–61) | 25,5 (14,5–28) | 0 (0–8) | 0 (0–1,5) | 8 (3–12) | 0 (0–1) |
| 4 | 46,5 (33,5–62,5) | 22 (13–26) | 0 (0–1,5) | 0 (0–2,5) | 9 (5–13) | 0,5 (0–2) |
| 5 | 47 (37–60) | 21,5 (16–28) | 0 (0–4) | 1 (0–1) | 7 (3,5–14) | 0 (0–2) |
| 6 | 51 (39,5–63) | 22 (15–29) | 0 (0–9,5) | 0,5 (0–3) | 7,5 (4,5–17,5) | 0 (0–1) |
| 7 | 45,5 (40,5–64) | 21 (14–27) | 0 (0–17) | 0 (0–1,5) | 9 (4–12) | 0 (0–2) |
| 8 | 46,5 (36–69) | 23,5 (16–33) | 0 (0–10,5) | 0 (0–2) | 9 (6,5–15) | 0 (0–1,5) |
| 9 | 45 (28,5–62) | 21,5 (13,5–28) | 0 (0–10,5) | 1 (0–2,5) | 5,5 (3,5–10,5) | 0 (0–1,5) |
| 10 | 43,5 (35,5–59,5) | 22 (14,5–27,5) | 0 (0–0,5) | 0 (0–2,5) | 8,5 (4–12,5) | 0 (0–1) |
| 11 | 44 (35–68) | 23 (13–31) | 0 (0–7,5) | 0,5 (0–2,5) | 9,5 (4–17) | 0 (0–0,5) |
| 12 | 42,5 (30,5–66,5) | 20 (14–28) | 0 (0–0,5) | 0 (0–1) | 9 (3–15) | 1 (0–2) |
| 13 | 51 (32,5–64,5) | 21 (14,5–30,5) | 0 (0–11,5) | 0,5 (0–1) | 11 (4–17,5) | 0 (0–2) |
| 14 | 49 (36–61) | 22 (17–30,5) | 0 (0–13,5) | 0 (0–1,5) | 9 (4–15) | 0 (0–1) |
| 15 | 46 (35,5–68) | 20 (15,5–31,5) | 0 (0–7,5) | 0 (0–1,5) | 9 (5–16) | 0 (0–1,5) |
| 16 | 49 (38,5–63) | 24 (15–27) | 0 (0–19) | 0 (0–2,5) | 9,5 (6–14) | 0 (0–1) |
| 17 | 43,5 (33–54) | 18,5 (13,5–27) | 0 (0–4,5) | 0,5 (0–1) | 9 (4,5–15) | 0 (0–0,5) |
| 18 | 46 (31,5–62) | 22 (12–28,5) | 0 (0–13) | 0 (0–1) | 9 (4,5–15,5) | 0 (0–1,5) |
| 19 | 39,5 (25,5–65) | 21 (8,5–27) | 0 (0–10) | 0 (0–1) | 6,5 (3–18,5) | 0 (0–0,5) |
| 20 | 45 (29–58) | 20 (13,5–28,5) | 0 (0–6) | 0 (0–1,5) | 7 (3,5–13,5) | 0 (0–2) |
| 21 | 48,5 (23,5–75) | 19 (11–26,5) | 0 (0–17) | 0 (0–2) | 6,5 (2,5–18,5) | 1 (0–2) |
| 22 | 44,5 (33,5–75,5) | 18,5 (14,5–25) | 0 (0–33) | 0,5 (0–2) | 7 (5–12,5) | 0 (0–1,5) |
| 23 | 45,5 (27,5–63) | 19,5 (10,5–32,5) | 0 (0–5) | 0 (0–2) | 9 (2–16,5) | 0 (0–2) |
| 24 | 47 (30–78,5) | 19 (11,5–29) | 0 (0–9,5) | 0 (0–1) | 9 (4,5–20) | 0 (0–2,5) |
| 25 | 39 (26,5–57) | 17,5 (14–26,5) | 0 (0–2,5) | 0 (0–3) | 6,5 (3,5–14,5) | 0 (0–1) |
| 26 | 48,5 (32,5–77) | 17,5 (12–31) | 0,5 (0–20,5) | 0 (0–4) | 11 (3,5–15,5) | 0 (0–2) |
| 27 | 40 (32,5–66,5) | 18 (11–24,5) | 0,5 (0–13,5) | 0 (0–1) | 8 (3–21) | 0 (0–2,5) |
| 28 | 49,5 (39–80) | 21 (12,5–27) | 0 (0–36) | 0 (0–1,5) | 10,5 (5,5–19) | 0 (0–2) |
| 29 | 39 (31,5–59,5) | 18 (6–22) | 0 (0–17) | 0 (0–1) | 7 (3,5–16) | 1 (0–2) |
| 30 | 46,5 (31,5–64,5) | 17 (9,5–29) | 0 (0–9) | 0 (0–1) | 9,5 (3,5–15,5) | 0,5 (0–2) |
| 31 | 41 (28,5–60) | 18,5 (8–24,5) | 0 (0–11,5) | 0 (0–1) | 7 (5–15,5) | 0 (0–3) |
| 32 | 50 (31–64,5) | 15,5 (11,5–26) | 0,5 (0–34,5) | 0,5 (0–3) | 10 (5–16) | 0 (0–0,5) |
| 33 | 43 (36–68,5) | 19 (11–26) | 0 (0–16) | 0 (0–1) | 9 (3,5–14,5) | 0 (0–2) |
| 34 | 38 (24,5–81) | 12 (8–24) | 0 (0–27) | 0 (0–1) | 7,5 (3,5–17,5) | 0 (0–2) |
| 35 | 39 (19,5–58) | 14,5 (8,5–27,5) | 0 (0–2,5) | 0 (0–1,5) | 7,5 (2,5–16,5) | 1 (0–3) |
| 36 | 40,5 (29–55) | 18 (10,5–26) | 0,5 (0–5) | 0 (0–1,5) | 8,5 (4,5–12,5) | 0,5 (0–2) |
| 37 | 47 (28,5–67) | 15,5 (9–28) | 1 (0–25,5) | 1 (0–1) | 9 (2–17) | 0 (0–1) |
| 38 | 39,5 (28,5–83) | 17 (9–28) | 1 (0–32) | 0 (0–1,5) | 10 (3–18) | 0 (0–1) |
| 39 | 46,5 (27,5–64,5) | 17,5 (9–24,5) | 1 (0–31) | 0,5 (0–1) | 9,5 (4–16,5) | 0 (0–0,5) |
| 40 | 44 (27–62) | 15 (9,5–26,5) | 0 (0–21,5) | 0,5 (0–2) | 8,5 (3–14,5) | 0 (0–1) |
| 41 | 42,5 (25–69,5) | 15 (9,5–24,5) | 1,5 (0–31,5) | 0 (0–2) | 10,5 (4,5–14,5) | 0 (0–1,5) |
| 42 | 43 (26–58) | 16 (8–30) | 0,5 (0–12,5) | 0 (0–2) | 8 (4–16,5) | 0 (0–1,5) |
| 43 | 43 (35,5–61) | 19,5 (13–27) | 0,5 (0–7,5) | 0 (0–1) | 10 (4–20) | 0 (0–2,5) |
| 44 | 48 (33,5–72) | 17,5 (9,5–27,5) | 4 (0–22,5) | 1 (0–2) | 7,5 (3,5–17,5) | 0 (0–2,5) |
| 45 | 42,5 (30,5–65,5) | 18 (10–25) | 0 (0–20) | 0 (0–2) | 8 (3–15) | 0 (0–2,5) |
| 46 | 47,5 (33,5–83) | 19 (6–28) | 2,5 (0–34,5) | 0 (0–1) | 7,5 (4–12,5) | 0 (0–2) |
| 47 | 42 (27–57,5) | 15,5 (10–23,5) | 0,5 (0–14) | 0,5 (0–1,5) | 9 (3–15) | 0 (0–1) |
| 48 | 44,5 (25–66,5) | 19,5 (10,5–27) | 1 (0–9,5) | 0 (0–1) | 9 (1,5–16) | 0 (0–2) |
| 49 | 42,5 (34–58) | 18 (11,5–24,5) | 0,5 (0–12) | 1 (0–2) | 8,5 (5–16,5) | 0,5 (0–1,5) |
| 50 | 42 (33–61) | 17,5 (10,5–23) | 2 (0–22,5) | 0 (0–1,5) | 7,5 (2,5–14,5) | 0 (0–1) |
| 51 | 36 (23,5–65) | 15 (7–23) | 1 (0–18,5) | 0 (0–1) | 8 (4–15) | 0 (0–1,5) |
| 52 | 42,5 (33–72,5) | 18,5 (12–25) | 0 (0–24) | 0 (0–2) | 9,5 (5–12) | 0 (0–2) |
| 53 | 34 (23,5–63) | 16,5 (7–19) | 0 (0–15) | 0,5 (0–1,5) | 9 (3,5–15) | 0 (0–2) |
| 54 | 49 (25–80,5) | 14,5 (7,5–29,5) | 0,5 (0–22) | 1 (0–2,5) | 8 (4,5–17) | 0 (0–3) |
| 55 | 42,5 (26,5–73) | 15 (8–27,5) | 2,5 (0–12,5) | 0 (0–1,5) | 8 (4–16) | 0 (0–1,5) |
| 56 | 45,5 (25,5–64) | 16 (6,5–27) | 0 (0–24) | 0 (0–1) | 7 (3–18) | 0 (0–1) |
| 57 | 46 (30–70,5) | 18 (9,5–28,5) | 2 (0–14,5) | 0 (0–2,5) | 9 (5–18) | 0 (0–1) |
| 58 | 39 (29–74,5) | 17,5 (7–20,5) | 1 (0–20,5) | 0 (0–2) | 9,5 (4,5–19) | 0 (0–1) |
| 59 | 44 (32,5–71) | 18,5 (8,5–23,5) | 0,5 (0–19,5) | 0 (0–1) | 9,5 (4,5–20,5) | 0 (0–2) |
| 60 | 39,5 (20–64,5) | 12 (8–25,5) | 0 (0–11) | 0 (0–1,5) | 10 (3–20,5) | 0 (0–1) |

### Devlet, inanç ve örgüt (1/5)

| Yıl | Yaşayan örgüt (yıl sonu) | Örgüt şubesi (yıl sonu) | Gizli şube (yıl sonu) | Örgüt üyesi (yıl sonu) | Açılan şube | Kapanan şube |
|---|---|---|---|---|---|---|
| 1 | 13 | 326 (237–370) | 15,5 (10,5–24) | 1370 (1291–1539) | 23 (18,5–28) | 21 (11–32) |
| 2 | 13 (12–13) | 328 (237–360) | 14,5 (8,5–19,5) | 1366 (1239–1521) | 19,5 (14,5–24,5) | 22,5 (16,5–35,5) |
| 3 | 13 (12–13) | 317 (234–350) | 12 (9–16,5) | 1350 (1244–1510) | 17 (15–21,5) | 23,5 (13–35,5) |
| 4 | 13 (12–13) | 308 (229–353) | 11 (8–16,5) | 1356 (1224–1558) | 17 (14–21) | 19 (11,5–30) |
| 5 | 13 (12–13) | 306 (234–357) | 11 (8–16) | 1348 (1196–1524) | 16,5 (12–20) | 18,5 (8,5–23,5) |
| 6 | 13 (12–13) | 296 (234–355) | 11 (7–14) | 1356 (1224–1527) | 16 (12–20) | 18,5 (10,5–27,5) |
| 7 | 13 (12–13) | 296 (230–351) | 10,5 (7–14) | 1337 (1232–1506) | 16,5 (10,5–20,5) | 18,5 (13,5–26) |
| 8 | 13 (12–13) | 294 (240–354) | 9 (5–14) | 1348 (1234–1486) | 15,5 (10,5–21) | 14 (9,5–22,5) |
| 9 | 13 (12–13) | 290 (233–357) | 8,5 (5–13) | 1328 (1221–1500) | 16,5 (13–20) | 19 (12–21,5) |
| 10 | 13 (12–13) | 290 (229–358) | 9 (4,5–12,5) | 1352 (1212–1556) | 16 (13–21,5) | 15 (8,5–20) |
| 11 | 13 (12–13) | 286 (228–362) | 8,5 (4,5–11,5) | 1352 (1250–1548) | 15 (12,5–19,5) | 15 (8–23,5) |
| 12 | 13 (12–13) | 290 (235–366) | 9 (5–13) | 1340 (1270–1586) | 17 (13,5–19,5) | 13,5 (7,5–23) |
| 13 | 13 (12–13) | 286 (238–372) | 7,5 (4–13,5) | 1369 (1256–1618) | 14,5 (11–19) | 13 (9–28,5) |
| 14 | 13 (12–13) | 286 (224–380) | 8 (4,5–12,5) | 1366 (1248–1630) | 16,5 (14–21) | 16 (5,5–26) |
| 15 | 13 (12–13) | 290 (218–378) | 7,5 (6–12,5) | 1390 (1244–1626) | 17 (12,5–20,5) | 16 (7,5–22) |
| 16 | 13 (12–13) | 298 (212–374) | 8 (6–12,5) | 1392 (1252–1636) | 15 (11,5–18) | 15 (7,5–22,5) |
| 17 | 13 (12–13) | 308 (214–378) | 7 (5–14) | 1402 (1230–1648) | 17 (15–22) | 13,5 (9–25,5) |
| 18 | 13 (12–13) | 308 (215–382) | 7 (5–14) | 1402 (1239–1694) | 18,5 (14–21,5) | 15 (9,5–22) |
| 19 | 13 (12–13) | 302 (220–388) | 8 (5–13) | 1386 (1254–1682) | 16 (10,5–18) | 12 (7,5–25) |
| 20 | 13 (12–13) | 306 (212–388) | 6 (4,5–12,5) | 1406 (1240–1696) | 16 (11,5–20,5) | 15 (8–24) |
| 21 | 13 (12–13) | 304 (218–390) | 8,5 (3–13,5) | 1410 (1242–1712) | 17 (13,5–19,5) | 16 (10,5–25,5) |
| 22 | 13 (12–13) | 306 (220–392) | 8 (3–14,5) | 1436 (1235–1740) | 16,5 (11–19,5) | 14,5 (9,5–20,5) |
| 23 | 13 (12–13) | 312 (228–395) | 7,5 (3–13) | 1403 (1256–1732) | 16 (12,5–18,5) | 14,5 (9–18) |
| 24 | 12,5 (12–13) | 310 (228–395) | 8 (4–11,5) | 1376 (1256–1778) | 14,5 (12–20) | 15 (9,5–20) |
| 25 | 13 (12–13) | 314 (224–394) | 7 (3,5–11,5) | 1399 (1248–1760) | 16,5 (11,5–19,5) | 16 (9,5–22,5) |
| 26 | 13 (12–13) | 306 (224–394) | 6 (3,5–12) | 1427 (1206–1730) | 15 (13,5–18) | 15 (9,5–31) |
| 27 | 13 (12–13) | 308 (220–396) | 7,5 (4–11,5) | 1445 (1232–1751) | 16,5 (13,5–20) | 14,5 (8,5–21) |
| 28 | 13 (12–13) | 305 (226–397) | 7,5 (4–11) | 1449 (1230–1764) | 15,5 (12–20,5) | 15 (9,5–21,5) |
| 29 | 13 (12–13) | 306 (224–389) | 6,5 (4–12,5) | 1436 (1224–1772) | 13,5 (12–18,5) | 14 (7,5–22,5) |
| 30 | 12,5 (12–13) | 314 (228–385) | 6 (4–12,5) | 1462 (1232–1800) | 15 (12–20) | 12 (6,5–19) |
| 31 | 12,5 (12–13) | 317 (225–384) | 8 (3,5–10) | 1490 (1236–1843) | 16,5 (12–21) | 13,5 (7,5–25) |
| 32 | 13 (12–13) | 320 (227–380) | 7 (4–11,5) | 1487 (1247–1850) | 15,5 (11–20,5) | 13,5 (9,5–17,5) |
| 33 | 12 (12–13) | 316 (222–372) | 7,5 (4,5–11,5) | 1494 (1224–1838) | 16 (10,5–19) | 15,5 (10–23,5) |
| 34 | 12,5 (12–13) | 320 (231–382) | 7,5 (4–12,5) | 1508 (1248–1850) | 16 (12–21) | 10 (6,5–16) |
| 35 | 13 (12–13) | 322 (231–387) | 8 (5–12,5) | 1510 (1286–1904) | 15,5 (12–20) | 11,5 (5–17,5) |
| 36 | 13 (12–13) | 322 (233–380) | 7 (4–11) | 1536 (1296–1869) | 13,5 (10–19) | 16,5 (9–24,5) |
| 37 | 13 (12–13) | 323 (230–388) | 7,5 (4–11) | 1546 (1320–1866) | 16,5 (12–20,5) | 13,5 (9–20,5) |
| 38 | 13 (11,5–13) | 322 (235–390) | 8,5 (4,5–12) | 1522 (1351–1856) | 17,5 (13–19) | 15 (8–22) |
| 39 | 12,5 (11,5–13) | 318 (234–390) | 7 (4–12,5) | 1548 (1364–1890) | 16,5 (11,5–20) | 13 (9,5–19) |
| 40 | 13 (11,5–13) | 319 (240–390) | 7,5 (4,5–12,5) | 1526 (1335–1876) | 14,5 (11–18) | 15,5 (9,5–21,5) |
| 41 | 13 (11,5–13) | 316 (244–387) | 7,5 (3,5–11) | 1548 (1316–1846) | 18 (12–21,5) | 16 (7,5–22,5) |
| 42 | 12,5 (12–13) | 324 (242–391) | 7,5 (4–10,5) | 1530 (1307–1832) | 18 (12,5–21) | 13,5 (9–21,5) |
| 43 | 12,5 (12–13) | 320 (242–396) | 7 (3,5–11) | 1503 (1288–1846) | 15,5 (11–20,5) | 14,5 (9,5–20) |
| 44 | 12 (12–13) | 314 (246–400) | 6,5 (2,5–11) | 1498 (1284–1868) | 16 (11,5–18) | 16,5 (12–26,5) |
| 45 | 12,5 (12–13) | 319 (251–401) | 6 (3,5–11) | 1499 (1310–1868) | 16,5 (12–19) | 12 (9–21) |
| 46 | 12 (12–13) | 322 (256–404) | 6 (3–11,5) | 1514 (1343–1898) | 17 (14,5–21) | 12,5 (8,5–21) |
| 47 | 13 (12–13) | 322 (252–405) | 6 (3,5–13) | 1549 (1319–1929) | 15 (13–19,5) | 14,5 (8,5–20) |
| 48 | 13 (12–13) | 329 (250–402) | 7,5 (4–10,5) | 1542 (1330–1978) | 16,5 (11–22,5) | 14,5 (10,5–24,5) |
| 49 | 12,5 (12–13) | 330 (252–400) | 7 (3,5–10,5) | 1547 (1324–1934) | 14 (10–20) | 17 (10–22,5) |
| 50 | 12 (12–13) | 336 (257–400) | 5 (3,5–9,5) | 1580 (1338–1952) | 17,5 (13–20) | 13 (9–19) |
| 51 | 12,5 (12–13) | 342 (260–394) | 5 (4–9,5) | 1608 (1354–1919) | 15 (11–19) | 13 (8–23) |
| 52 | 12 (12–13) | 344 (263–396) | 7 (4–11) | 1658 (1374–1938) | 16 (9,5–20) | 15 (9,5–17,5) |
| 53 | 13 (12–13) | 346 (259–400) | 7 (4–11) | 1651 (1372–1944) | 16 (13,5–21,5) | 14,5 (5,5–21) |
| 54 | 12,5 (12–13) | 338 (260–404) | 7 (4–11) | 1614 (1360–1957) | 14 (8–19,5) | 13 (9,5–24,5) |
| 55 | 12,5 (12–13) | 338 (261–415) | 7 (4,5–10,5) | 1591 (1381–1952) | 15 (12,5–20,5) | 15 (8,5–21,5) |
| 56 | 12,5 (12–13) | 331 (266–416) | 6 (4–10) | 1609 (1398–1963) | 13 (11–20,5) | 14 (5–18,5) |
| 57 | 13 (12–13) | 336 (270–413) | 7 (3,5–10,5) | 1610 (1386–1935) | 17 (13–19,5) | 15,5 (8–23) |
| 58 | 13 (12–13) | 338 (271–400) | 7 (4,5–10,5) | 1618 (1410–1937) | 15 (9–19) | 12 (7,5–21,5) |
| 59 | 13 (12–13) | 344 (266–394) | 7,5 (4,5–11) | 1598 (1408–1934) | 14 (9,5–19) | 17 (11–25) |
| 60 | 12,5 (12–13) | 342 (265–394) | 8 (4,5–10,5) | 1614 (1384–1913) | 16,5 (14–19,5) | 15,5 (7–27,5) |

### Devlet, inanç ve örgüt (2/5)

| Yıl | Dağılan örgüt | Yeniden kurulan örgüt | Gölge savaşı eylemi | Gölge savaşında öldürülen usta ya da lider | Gizli şubeye baskın | Lobiyle yasa değişikliği |
|---|---|---|---|---|---|---|
| 1 | 0 | 0 | 10 (6,5–13,5) | 2 (0,5–4,5) | 1 (0–2) | 1,5 (0,5–3) |
| 2 | 0 (0–0,5) | 0 | 6,5 (4–11) | 1,5 (0–4,5) | 1,5 (0–3) | 0 (0–2) |
| 3 | 0 | 0 | 7 (3–10) | 1 (0–3) | 1 (0–2) | 1 (0–2) |
| 4 | 0 | 0 | 7 (3,5–10,5) | 2 (0–4,5) | 1 (0–3) | 0 (0–2) |
| 5 | 0 | 0 | 6,5 (3–10,5) | 1,5 (0,5–3) | 1 (0–2) | 1 (0–1,5) |
| 6 | 0 | 0 | 7 (4–12) | 1 (0,5–4) | 1 (0–2) | 0 (0–2) |
| 7 | 0 | 0 | 7,5 (4–9,5) | 1,5 (0–3) | 1 (0–2) | 0 (0–2) |
| 8 | 0 | 0 | 7 (4,5–11) | 1 (0–3) | 1 (0–2) | 1 (0–1) |
| 9 | 0 | 0 | 8,5 (4–10,5) | 1,5 (0–3) | 0,5 (0–1) | 1 (0–2) |
| 10 | 0 | 0 | 8,5 (4,5–12) | 0,5 (0–3) | 0 (0–1,5) | 0,5 (0–2,5) |
| 11 | 0 (0–1) | 0 (0–0,5) | 6 (2–11) | 1 (0–3) | 0 (0–1) | 1 (0–3) |
| 12 | 0 | 0 | 7 (5,5–9) | 1 (0–3) | 0 (0–1) | 1 (0–2) |
| 13 | 0 | 0 | 4,5 (3–8,5) | 1 (0–2,5) | 0 (0–2) | 0 (0–1) |
| 14 | 0 | 0 | 6 (2,5–10,5) | 1 (0–3) | 0 (0–2) | 0,5 (0–1,5) |
| 15 | 0 | 0 (0–1) | 6,5 (3,5–10) | 1 (0–2) | 0,5 (0–1,5) | 0 (0–2,5) |
| 16 | 0 | 0 | 7 (4–10) | 2 (0,5–3,5) | 0 (0–1) | 1 (0–3) |
| 17 | 0 (0–0,5) | 0 (0–1) | 8,5 (4,5–10,5) | 1 (0–2,5) | 0,5 (0–1) | 1 (0–2) |
| 18 | 0 | 0 | 8 (4–11,5) | 1,5 (0–3) | 0 (0–1) | 1 (0–3) |
| 19 | 0 (0–0,5) | 0 | 7,5 (4–9) | 1 (1–2,5) | 0 (0–1,5) | 1 (0–2) |
| 20 | 0 | 0 | 6,5 (4,5–12) | 1 (0,5–3,5) | 0 (0–2) | 1 (0–3) |
| 21 | 0 (0–0,5) | 0 | 7,5 (2,5–11,5) | 1 (0–3,5) | 0 (0–1) | 1 (0–2) |
| 22 | 0 (0–0,5) | 0 (0–0,5) | 7 (5,5–8,5) | 1 (0–3) | 0 (0–1) | 1 (0–2) |
| 23 | 0 | 0 | 8 (6–10,5) | 1,5 (0–4) | 0 (0–1) | 0,5 (0–2,5) |
| 24 | 0 (0–0,5) | 0 | 6 (2,5–9) | 1 (0,5–3,5) | 0 (0–1) | 0,5 (0–2,5) |
| 25 | 0 | 0 | 6 (2,5–11,5) | 1 (0–2) | 1 (0–2) | 1 (0–2,5) |
| 26 | 0 | 0 | 6,5 (3,5–9,5) | 2 (0–3) | 0 (0–1) | 1 (0–2,5) |
| 27 | 0 | 0 | 6 (4–10,5) | 1 (0–2,5) | 0 (0–1) | 0,5 (0–2) |
| 28 | 0 (0–0,5) | 0 | 7 (5–9,5) | 1 (0–3) | 0 (0–1) | 1 (0–3,5) |
| 29 | 0 | 0 | 6 (3,5–9,5) | 1 (0,5–2,5) | 0,5 (0–1,5) | 0,5 (0–1) |
| 30 | 0 (0–1) | 0 (0–0,5) | 6 (3–11,5) | 1 (0–3) | 0,5 (0–1,5) | 1 (0–3) |
| 31 | 0 | 0 | 6 (2,5–12) | 1 (0–3) | 0 (0–1,5) | 1 (0–1) |
| 32 | 0 | 0 (0–0,5) | 7,5 (2–11) | 1,5 (0–3) | 0 (0–2) | 0 (0–3) |
| 33 | 0 (0–1) | 0 | 7 (2–11,5) | 1 (0–2) | 0 (0–1) | 1 (0–1,5) |
| 34 | 0 | 0 | 6 (3–10,5) | 1 (0–2,5) | 1 (0–1,5) | 0 (0–1,5) |
| 35 | 0 (0–0,5) | 0 (0–0,5) | 6 (2–9,5) | 1 (0–3) | 0 (0–1,5) | 1 (0–2) |
| 36 | 0 (0–0,5) | 0 (0–1) | 7,5 (3,5–10,5) | 1 (0–3,5) | 1 (0–2,5) | 1 (0–2) |
| 37 | 0 | 0 | 8 (4–11,5) | 1 (0–2,5) | 0 (0–1) | 0,5 (0–2) |
| 38 | 0 (0–0,5) | 0 | 7,5 (3–10,5) | 1,5 (0–2,5) | 0 (0–1) | 1 (0–2) |
| 39 | 0 | 0 | 6,5 (2–10) | 1,5 (0–2,5) | 0 (0–1) | 1 (0–1,5) |
| 40 | 0 | 0 (0–0,5) | 5,5 (2,5–7) | 1 (0–2,5) | 1 (0–3) | 0,5 (0–2) |
| 41 | 0 | 0 | 7 (4–10,5) | 1 (0–2,5) | 0 (0–1,5) | 1 (0–2) |
| 42 | 0 (0–1) | 0 (0–1) | 8,5 (3–10,5) | 1 (0–2) | 0 (0–1,5) | 0,5 (0–2,5) |
| 43 | 0 (0–0,5) | 0 (0–0,5) | 7 (2–12) | 1 (0–3) | 0 (0–1) | 1 (0–4,5) |
| 44 | 0 | 0 | 5,5 (4–8) | 1 (1–3) | 0 (0–1) | 1 (0–3) |
| 45 | 0 | 0 | 5 (2–8) | 1 (0–3) | 0 (0–1) | 1,5 (0–3) |
| 46 | 0 (0–0,5) | 0 | 4,5 (2,5–11) | 1,5 (0–3,5) | 0 (0–1) | 1 (0–2,5) |
| 47 | 0 | 0 (0–0,5) | 7,5 (3–11) | 1 (0–3) | 1 (0–1) | 1 (0–2) |
| 48 | 0 (0–0,5) | 0 (0–1) | 7,5 (3–10,5) | 1,5 (0–3) | 1 (0–1,5) | 1 (0–2) |
| 49 | 0 (0–0,5) | 0 | 7 (2,5–10,5) | 1 (0–3) | 0 (0–1) | 1 (0–2,5) |
| 50 | 0 | 0 | 5,5 (2,5–10) | 1 (0,5–2,5) | 1 (0–1) | 1 (0–3) |
| 51 | 0 | 0 (0–0,5) | 6,5 (4,5–9,5) | 1 (0,5–2,5) | 0 (0–1,5) | 0,5 (0–1,5) |
| 52 | 0 (0–0,5) | 0 | 5,5 (2,5–10) | 1 (0–2) | 0 (0–2) | 1 (0–3,5) |
| 53 | 0 | 0 (0–1) | 6 (4–8) | 1 (0–2) | 0 (0–2) | 0,5 (0–1,5) |
| 54 | 0 (0–0,5) | 0 | 5,5 (4–9) | 1 (0–2) | 0 (0–1) | 0,5 (0–2) |
| 55 | 0 | 0 | 5,5 (3–13) | 1 (0–3) | 0 (0–1) | 1 (0–3) |
| 56 | 0 | 0 | 6 (4–11) | 1 (0–3) | 0,5 (0–2) | 1 (0–2,5) |
| 57 | 0 | 0 (0–0,5) | 7 (4–11) | 1,5 (0,5–3) | 0 (0–1) | 2 (0–3) |
| 58 | 0 | 0 (0–1) | 7 (4–10) | 2 (0–3) | 0 (0–1) | 1 (0–4) |
| 59 | 0 (0–1) | 0 (0–0,5) | 6 (3–11,5) | 1 (0–3) | 1 (0–2) | 1 (0–2) |
| 60 | 0 | 0 | 6 (3–10,5) | 1 (0,5–2,5) | 0 (0–1) | 1,5 (0–4,5) |

### Devlet, inanç ve örgüt (3/5)

| Yıl | Darbe girişimi | Örgüt ilanı (Avcılar) | Örgüt üyesi kahraman payı (yıl sonu) | Yönetici değişimi | Veraset krizi | Meşruiyet ortalaması (yıl sonu) |
|---|---|---|---|---|---|---|
| 1 | 0 | 2 (1–2,5) | %100 (%99–%100) | 1,5 (0,5–3) | 0 | 63,1 (57,5–67,9) |
| 2 | 0 | 0,5 (0–1) | %100 (%99–%100) | 1 (0–2,5) | 0 | 63,5 (57,2–67,2) |
| 3 | 0 | 1 (0–1) | %100 (%99–%100) | 1 (0–2) | 0 | 64,7 (58,5–69) |
| 4 | 0 | 1 (0–1) | %100 (%99–%100) | 1 (0–2) | 0 (0–0,5) | 64,8 (58,4–69,3) |
| 5 | 0 | 1 (0–2) | %100 (%99–%100) | 1 (0,5–2,5) | 0 | 63,9 (58,1–68,8) |
| 6 | 0 | 0,5 (0–2) | %100 (%99–%100) | 1 (0–2) | 0 | 64 (59,1–66,9) |
| 7 | 0 | 1 (0–3) | %100 (%98–%100) | 1 (0–2) | 0 | 64,4 (59,1–67,9) |
| 8 | 0 (0–1) | 1 (0,5–3) | %100 (%99–%100) | 1 (0,5–2) | 0 | 64,9 (61,2–70,5) |
| 9 | 0 | 1 (0–3,5) | %100 (%99–%100) | 1 (0–1,5) | 0 | 66,3 (60,9–71,2) |
| 10 | 0 | 1 (0–3,5) | %100 (%99–%100) | 1 (0–2) | 0 | 67,8 (61,4–72,1) |
| 11 | 0 | 1 (0–4) | %99 (%99–%100) | 0 (0–2) | 0 | 67,6 (61,2–72,8) |
| 12 | 0 | 1 (0–3,5) | %100 (%99–%100) | 1 (0–2) | 0 | 67,7 (63,8–72,1) |
| 13 | 0 | 1 (0–3,5) | %100 (%99–%100) | 1 (0–2) | 0 | 67,4 (61,6–71,3) |
| 14 | 0 | 1 (0–3) | %100 (%99–%100) | 1 (0–2,5) | 0 | 65,3 (60–72,7) |
| 15 | 0 | 1 (0–3) | %100 (%99–%100) | 0 (0–2) | 0 | 65,3 (58,6–76,3) |
| 16 | 0 (0–0,5) | 1 (0–2) | %100 (%99–%100) | 1 (0–2,5) | 0 | 65,3 (60,5–72,2) |
| 17 | 0 (0–0,5) | 1 (0–2) | %100 (%99–%100) | 1 (0–2) | 0 | 64,6 (61–72,7) |
| 18 | 0 | 1 (0–2) | %100 (%99–%100) | 1 (1–2,5) | 0 | 63,2 (57,9–70,9) |
| 19 | 0 | 1 (0–3,5) | %100 (%99–%100) | 1 (0–2) | 0 | 64,2 (58–72,3) |
| 20 | 0 (0–0,5) | 1 (0–3,5) | %100 (%99–%100) | 1 (0–2) | 0 (0–0,5) | 64,5 (55,3–72) |
| 21 | 0 | 1 (0–2,5) | %100 (%99–%100) | 1 (0–2,5) | 0 | 63,3 (55,5–74,2) |
| 22 | 0 | 1 (0–4) | %100 (%98–%100) | 1 (0–2) | 0 | 63,7 (58,1–69,9) |
| 23 | 0 | 1 (0–4) | %100 (%99–%100) | 1 (0–1,5) | 0 | 64,3 (58,1–70,6) |
| 24 | 0 | 1 (0–3,5) | %100 (%99–%100) | 1 (0–2) | 0 | 66 (57–71,7) |
| 25 | 0 | 1 (0–3,5) | %100 (%99–%100) | 1 (0–3) | 0 | 64 (58,5–73,1) |
| 26 | 0 | 1 (0–4) | %100 (%99–%100) | 1 (0–2,5) | 0 | 64,4 (58,3–72,1) |
| 27 | 0 | 1 (0–2) | %100 (%99–%100) | 1,5 (0–2) | 0 | 66,1 (58,2–71) |
| 28 | 0 | 1 (0–2) | %100 (%99–%100) | 0 (0–2) | 0 | 66,2 (58,1–73,1) |
| 29 | 0 | 1 (0–3) | %100 (%99–%100) | 1 (0–2) | 0 | 65,4 (58–71,7) |
| 30 | 0 | 1 (0–3) | %100 (%99–%100) | 1 (0–2) | 0 | 67,2 (59,1–72,3) |
| 31 | 0 | 1 (0,5–2,5) | %100 (%99–%100) | 1 (0–2,5) | 0 | 65,1 (62,4–73,2) |
| 32 | 0 | 0,5 (0–4) | %100 (%99–%100) | 1 (0–2) | 0 | 65,5 (61,4–73) |
| 33 | 0 | 1 (0–3,5) | %100 (%99–%100) | 1 (0–2) | 0 | 66,1 (61,3–73,2) |
| 34 | 0 | 1 (0,5–4) | %100 (%99–%100) | 1 (0–3,5) | 0 | 62,8 (58,2–72,1) |
| 35 | 0 | 1 (0–3,5) | %99 (%99–%100) | 1 (0–2) | 0 | 64 (60,6–71,4) |
| 36 | 0 (0–0,5) | 1 (0–3,5) | %100 (%99–%100) | 1 (0–3) | 0 | 64,3 (59,7–73) |
| 37 | 0 | 1 (0–4) | %100 (%99–%100) | 1 (0,5–2) | 0 | 66,7 (62,5–73,1) |
| 38 | 0 | 1 (0–3,5) | %100 (%99–%100) | 1 (0–2) | 0 | 66,8 (57,9–72,9) |
| 39 | 0 | 1 (0–4) | %100 (%99–%100) | 1 (0–2) | 0 | 64,2 (58,2–73,5) |
| 40 | 0 | 1 (0–3) | %100 (%99–%100) | 1,5 (0–3) | 0 | 63,4 (58,2–72,6) |
| 41 | 0 | 0,5 (0–4) | %100 (%99–%100) | 1 (0–2) | 0 | 64,6 (60–71,5) |
| 42 | 0 | 1 (0–2) | %100 (%99–%100) | 1 (0–2,5) | 0 | 65,6 (58,1–70,2) |
| 43 | 0 | 1 (0–2,5) | %100 (%99–%100) | 1 (1–2,5) | 0 | 64,3 (59,1–71,1) |
| 44 | 0 | 1 (0–3) | %100 (%99–%100) | 1 (0–3) | 0 | 63 (59,2–69,4) |
| 45 | 0 | 1 (0–3,5) | %100 (%99–%100) | 1 (0–2) | 0 | 63,7 (56,9–69,5) |
| 46 | 0 | 1 (0–3,5) | %100 (%99–%100) | 1 (0–2) | 0 | 63,7 (56,6–70,8) |
| 47 | 0 | 1 (0–4) | %100 (%99–%100) | 1 (0–3) | 0 | 64,2 (55,8–70,5) |
| 48 | 0 | 1 (0–4) | %100 (%99–%100) | 1 (0–2,5) | 0 | 63,1 (55,6–73,8) |
| 49 | 0 | 1,5 (0–4) | %100 (%99–%100) | 1 (0–2) | 0 | 63,1 (55,3–72,8) |
| 50 | 0 | 1 (0–3,5) | %100 (%99–%100) | 1 (0,5–2,5) | 0 | 63,7 (56,9–73,2) |
| 51 | 0 | 1 (0–4) | %100 (%99–%100) | 1 (0–2) | 0 | 63,6 (56,1–75) |
| 52 | 0 | 1 (0–3,5) | %100 (%99–%100) | 2 (0–3) | 0 | 62 (55–73,9) |
| 53 | 0 | 1 (0,5–3,5) | %100 (%99–%100) | 1 (0–2) | 0 | 63,9 (59,1–73,4) |
| 54 | 0 | 0,5 (0–2) | %100 (%99–%100) | 1 (0–2) | 0 | 65 (59,5–71,7) |
| 55 | 0 | 1 (0–3) | %100 (%99–%100) | 1,5 (0–3,5) | 0 | 63,9 (56,5–70,5) |
| 56 | 0 | 1 (0–3,5) | %100 (%99–%100) | 1 (0–3) | 0 | 64,6 (57–70,9) |
| 57 | 0 | 0 (0–3,5) | %100 (%99–%100) | 1 (0–2,5) | 0 | 66,4 (58,3–71,6) |
| 58 | 0 | 1 (0–3,5) | %100 (%99–%100) | 1,5 (0–3) | 0 | 66,3 (56,3–69,9) |
| 59 | 0 | 1 (0–3,5) | %100 (%99–%100) | 1,5 (0–3) | 0 | 63,3 (54,3–70,1) |
| 60 | 0 | 1 (0–4) | %100 (%99–%100) | 1 (0–3) | 0 | 62,9 (53,7–71,7) |

### Devlet, inanç ve örgüt (4/5)

| Yıl | Pakt'a bağlı yönetici (yıl sonu) | Köle (yıl sonu) | Köle payı (nüfusun, yıl sonu) | Hapis madeninde mahkûm (yıl sonu) | Esarete düşen | Kurtulan köle (Özgürlük Ağı, kaçış, azat) |
|---|---|---|---|---|---|---|
| 1 | 0 (0–1) | 20,5 (3–37) | %0,7 (%0,1–%1,3) | 0 (0–3) | 6 (0,5–12,5) | 11,5 (2–21) |
| 2 | 0 (0–1) | 12 (1,5–35,5) | %0,4 (%0,1–%1,4) | 0 (0–2) | 5,5 (0–12,5) | 6,5 (3–19,5) |
| 3 | 0 (0–1) | 7,5 (0,5–29) | %0,2 (%0–%1,1) | 1 (0–3) | 4 (0–7,5) | 9,5 (2–12) |
| 4 | 0,5 (0–1) | 5,5 (1–30,5) | %0,2 (%0–%1,1) | 0,5 (0–5,5) | 4,5 (1–14) | 6 (1,5–13) |
| 5 | 0 (0–1) | 3,5 (0–30,5) | %0,1 (%0–%1) | 0 (0–2,5) | 2,5 (0–11) | 5 (1–12) |
| 6 | 0 (0–1) | 8,5 (1–31) | %0,3 (%0–%1,1) | 0 (0–3) | 4 (0–13) | 3 (0–6,5) |
| 7 | 0 (0–0,5) | 10,5 (1–31,5) | %0,4 (%0–%1,1) | 1 (0–3,5) | 5 (1–14,5) | 7 (1–12) |
| 8 | 0 | 11 (0,5–30) | %0,4 (%0–%1,1) | 0,5 (0–3,5) | 4,5 (0,5–10,5) | 5 (0–12,5) |
| 9 | 0 | 5 (1–31,5) | %0,2 (%0–%1) | 0,5 (0–2,5) | 4,5 (0,5–12,5) | 6 (1,5–12,5) |
| 10 | 0 | 3,5 (1,5–31,5) | %0,1 (%0–%1) | 0 (0–2,5) | 4,5 (1–8) | 3 (0,5–9,5) |
| 11 | 0 | 6 (0,5–34,5) | %0,2 (%0–%1,2) | 0 (0–5) | 5 (0–17) | 4 (0–12) |
| 12 | 0 | 4 (0,5–34) | %0,1 (%0–%1,2) | 0 (0–5) | 3,5 (0,5–13) | 6 (1–12) |
| 13 | 0 | 5 (0–34,5) | %0,2 (%0–%1,2) | 0,5 (0–4,5) | 4 (0–7,5) | 4,5 (0–11,5) |
| 14 | 0 (0–1) | 4 (0–30) | %0,1 (%0–%1) | 0,5 (0–4,5) | 3 (0–15,5) | 4,5 (0–10) |
| 15 | 0 (0–0,5) | 6,5 (2–31,5) | %0,2 (%0,1–%1,1) | 0 (0–5,5) | 8,5 (1,5–14) | 6,5 (1–15) |
| 16 | 0 | 4 (1–31,5) | %0,2 (%0–%1,1) | 1 (0–5) | 4 (0–11) | 6 (1,5–12,5) |
| 17 | 0 (0–0,5) | 5 (0,5–34) | %0,2 (%0–%1,2) | 1 (0–2) | 5 (0–11) | 5 (0,5–12,5) |
| 18 | 0 (0–1) | 8,5 (0–47) | %0,3 (%0–%1,7) | 0,5 (0–3,5) | 5,5 (0–20,5) | 5 (0–13) |
| 19 | 0 (0–1) | 9 (1,5–42,5) | %0,3 (%0,1–%1,5) | 0 (0–3,5) | 6,5 (0–11) | 4,5 (1–12,5) |
| 20 | 0 (0–0,5) | 11 (2–38,5) | %0,4 (%0,1–%1,3) | 1 (0–3) | 3 (1–13) | 7 (1–11) |
| 21 | 0 | 7 (0–37,5) | %0,2 (%0–%1,2) | 0,5 (0–5,5) | 5 (0,5–12,5) | 7,5 (2,5–13) |
| 22 | 0 (0–1) | 8 (1–33) | %0,3 (%0–%1,1) | 0 (0–5,5) | 5,5 (1–10) | 5,5 (2–11,5) |
| 23 | 0 (0–1) | 5 (2–23,5) | %0,2 (%0,1–%0,8) | 0 (0–3,5) | 2 (0,5–9) | 5,5 (1–12,5) |
| 24 | 0 (0–1) | 6,5 (1,5–25) | %0,2 (%0–%0,9) | 0 (0–2) | 5 (1,5–10,5) | 6 (1–10) |
| 25 | 0 (0–1) | 7,5 (2–23,5) | %0,3 (%0,1–%0,8) | 0 (0–5) | 7 (2–10,5) | 5 (3–13,5) |
| 26 | 0 (0–1) | 9 (2–27) | %0,3 (%0,1–%1) | 0 (0–4) | 5 (1,5–16,5) | 6,5 (2–13) |
| 27 | 0 | 8 (2–30,5) | %0,3 (%0,1–%1) | 0 (0–1,5) | 6 (2–11,5) | 6 (3–9) |
| 28 | 0 | 8,5 (1–26) | %0,3 (%0–%0,9) | 0 (0–2) | 5,5 (0,5–11) | 6 (2–11,5) |
| 29 | 0 (0–0,5) | 10 (4–24) | %0,4 (%0,1–%0,8) | 0 (0–2,5) | 8 (4–11) | 6 (1,5–12,5) |
| 30 | 0 | 7 (1–29) | %0,2 (%0–%1) | 0 (0–2) | 5 (0,5–15,5) | 6 (1,5–10) |
| 31 | 0 | 16,5 (1,5–29) | %0,5 (%0,1–%1) | 0 (0–2) | 8,5 (1,5–16,5) | 7 (1,5–10,5) |
| 32 | 0 (0–0,5) | 12 (2–28) | %0,4 (%0,1–%0,9) | 0 (0–2) | 7 (1,5–13) | 6 (2–13,5) |
| 33 | 0 (0–0,5) | 20 (5,5–27,5) | %0,7 (%0,2–%0,8) | 0,5 (0–2) | 9 (0,5–17) | 6 (1–10) |
| 34 | 0 | 21 (1–28,5) | %0,7 (%0–%0,9) | 0 (0–7,5) | 6,5 (0,5–12,5) | 7,5 (1,5–11) |
| 35 | 0 | 17,5 (1,5–33) | %0,6 (%0–%1,1) | 0 (0–3) | 8 (2–21) | 9 (2,5–15,5) |
| 36 | 0 | 22 (3–34,5) | %0,7 (%0,1–%1,2) | 0 (0–5) | 9 (2–23) | 9 (1,5–16) |
| 37 | 0 (0–0,5) | 18,5 (6–39,5) | %0,7 (%0,2–%1,3) | 0 (0–2) | 7 (0–20) | 6 (1,5–12,5) |
| 38 | 0 | 15,5 (3–35) | %0,5 (%0,1–%1) | 0 (0–1,5) | 8 (0,5–15,5) | 8,5 (0,5–27,5) |
| 39 | 0 | 15,5 (2–36,5) | %0,6 (%0,1–%1,1) | 0 (0–2) | 9 (0,5–16) | 6 (1,5–11) |
| 40 | 0 | 15 (2–38,5) | %0,5 (%0,1–%1,2) | 0 (0–5,5) | 7,5 (2–17,5) | 6 (1,5–13) |
| 41 | 0 | 20,5 (4,5–45) | %0,7 (%0,2–%1,2) | 0,5 (0–2) | 6,5 (0,5–19) | 5 (1–10,5) |
| 42 | 0 (0–1) | 20 (6,5–48) | %0,7 (%0,2–%1,4) | 0 (0–1,5) | 8,5 (1,5–13) | 8,5 (1,5–13) |
| 43 | 0 (0–1) | 17 (7–47) | %0,6 (%0,2–%1,4) | 0 (0–3) | 6,5 (3–12) | 7 (2,5–17) |
| 44 | 0 (0–0,5) | 18 (6,5–49) | %0,7 (%0,2–%1,5) | 0 (0–3,5) | 7,5 (1–18) | 6,5 (3–15) |
| 45 | 0 | 22 (7–48,5) | %0,7 (%0,2–%1,5) | 0 (0–1,5) | 6 (3,5–17) | 7 (2–11) |
| 46 | 0 | 20 (5–50,5) | %0,6 (%0,2–%1,5) | 0 (0–2) | 6,5 (2–18,5) | 7 (2–15,5) |
| 47 | 0 | 18,5 (7–46) | %0,6 (%0,3–%1,4) | 0 (0–2) | 7,5 (1–14,5) | 8 (4–14) |
| 48 | 0 | 18,5 (9–47) | %0,6 (%0,4–%1,4) | 0 (0–2,5) | 9 (3,5–19,5) | 7,5 (2–15,5) |
| 49 | 0 | 18,5 (5,5–39,5) | %0,6 (%0,2–%1,3) | 0 (0–2,5) | 6,5 (1,5–18) | 8 (6–13) |
| 50 | 0 | 17,5 (4–46) | %0,6 (%0,1–%1,4) | 0 (0–2,5) | 7,5 (0–18) | 8,5 (2,5–12,5) |
| 51 | 0 | 21,5 (5,5–45) | %0,7 (%0,2–%1,6) | 0 (0–2) | 8 (2,5–21) | 8,5 (3,5–13) |
| 52 | 0 (0–0,5) | 20,5 (5–47,5) | %0,7 (%0,2–%1,6) | 0 (0–4) | 8 (2–13) | 9,5 (3,5–13,5) |
| 53 | 0 (0–0,5) | 18,5 (4–46) | %0,7 (%0,1–%1,5) | 0 (0–2) | 6,5 (2,5–13) | 6 (4,5–15,5) |
| 54 | 0 (0–0,5) | 18,5 (7–49) | %0,6 (%0,3–%1,6) | 0 (0–4,5) | 7,5 (2,5–19) | 7,5 (3,5–14) |
| 55 | 0 (0–0,5) | 15,5 (5–43) | %0,5 (%0,2–%1,3) | 0 (0–1) | 3,5 (1–13) | 9 (3–13) |
| 56 | 0 (0–0,5) | 16 (7–44) | %0,5 (%0,2–%1,5) | 0 (0–2) | 8 (3,5–29,5) | 8,5 (4–14,5) |
| 57 | 0 (0–0,5) | 19,5 (9–49) | %0,7 (%0,3–%1,8) | 0 (0–1,5) | 11,5 (5,5–29,5) | 9,5 (4–17) |
| 58 | 0 (0–1) | 22 (10–53) | %0,7 (%0,3–%1,8) | 0 (0–3) | 13 (3,5–22,5) | 8 (4,5–18) |
| 59 | 0 | 15 (8–56,5) | %0,5 (%0,3–%2) | 0 (0–6) | 6 (2–20) | 10,5 (5,5–15,5) |
| 60 | 0 | 19 (4,5–51) | %0,6 (%0,2–%2) | 0 (0–4) | 5 (2,5–24) | 9 (4–15,5) |

### Devlet, inanç ve örgüt (5/5)

| Yıl | Özgürlük Ağı'nın kurtardığı köle | Esir kahraman (yıl sonu) | Aç haydut kampı (yıl sonu) | Haydut olan aç halk | Aç ya da ekmeksiz yerleşim payı (köy+) | Devriye durdurması |
|---|---|---|---|---|---|---|
| 1 | 7,5 (2–12) | 0 (0–1,5) | 0 (0–1) | 5,5 (0–14,5) | %3,8 (%0,1–%11) | 49 (33–66,5) |
| 2 | 5,5 (2–13,5) | 0 (0–1) | 0 (0–1,5) | 4,5 (0–28,5) | %2,8 (%0,8–%13) | 47 (35–58,5) |
| 3 | 8 (1,5–10) | 0 (0–2) | 0 (0–1,5) | 3,5 (0–21) | %3,3 (%0,9–%12) | 44 (35,5–66,5) |
| 4 | 4,5 (1,5–9) | 1 (0–3) | 0 (0–1,5) | 4,5 (0–20,5) | %3,6 (%0,5–%14) | 48 (35,5–77) |
| 5 | 3,5 (1–9) | 0 (0–2) | 1 (0–2) | 3 (1,5–17,5) | %4 (%2,2–%8,6) | 48,5 (30,5–66) |
| 6 | 2 (0–5,5) | 1 (0–2) | 1 (0–1,5) | 8 (0–23) | %5,2 (%1,4–%11) | 52 (35,5–73,5) |
| 7 | 5 (0–9,5) | 0,5 (0–3) | 0 (0–2) | 6 (0–17,5) | %5 (%1,3–%12) | 51 (29,5–73,5) |
| 8 | 5 (0–9) | 0 (0–2,5) | 0 (0–1) | 9,5 (0–26,5) | %3,7 (%0,8–%13) | 52 (29–73) |
| 9 | 3,5 (0,5–9,5) | 0 (0–1) | 0 (0–1) | 4 (0–21,5) | %4 (%1,3–%13) | 57 (28,5–74,5) |
| 10 | 3 (0–7) | 0,5 (0–3) | 0 (0–1) | 4 (0–18,5) | %4,4 (%0,9–%13) | 48 (36–82) |
| 11 | 3,5 (0–8,5) | 0 (0–2) | 1 (0–1) | 9,5 (0–18) | %4,4 (%0,7–%13) | 54 (35,5–82,5) |
| 12 | 5,5 (0–9) | 0 (0–3,5) | 0,5 (0–1,5) | 6 (0–17) | %4 (%1,3–%12) | 48,5 (31–70,5) |
| 13 | 4 (0–8) | 0 (0–1) | 0 (0–1) | 3,5 (0–11) | %4,3 (%1,3–%12) | 54 (28,5–82) |
| 14 | 3 (0–7,5) | 0 (0–2) | 0,5 (0–2) | 3 (0–16,5) | %4,7 (%1–%10) | 52,5 (32–77,5) |
| 15 | 6 (1–9) | 0 (0–2) | 0 (0–1) | 5 (0–14,5) | %5,5 (%0,1–%16) | 50,5 (32,5–80,5) |
| 16 | 5 (0–10) | 0 (0–2,5) | 0 (0–1) | 6,5 (0–18,5) | %4,3 (%1,2–%16) | 53,5 (30,5–88) |
| 17 | 4 (0–11) | 1 (0–1) | 0 (0–1) | 4,5 (0–22,5) | %3,5 (%0,6–%18) | 52 (25,5–78) |
| 18 | 3,5 (0–10) | 0 (0–1) | 1 (0–2) | 4,5 (0–20,5) | %4,5 (%1,2–%14) | 47 (32,5–69,5) |
| 19 | 4,5 (1–10,5) | 0 (0–1) | 0 (0–2) | 7,5 (1,5–12,5) | %4,1 (%1,8–%13) | 52,5 (32,5–75,5) |
| 20 | 4,5 (1–8,5) | 0 (0–0,5) | 0 (0–2) | 8 (0–17,5) | %5,4 (%0,9–%14) | 48 (33,5–70) |
| 21 | 5,5 (2–11) | 0 (0–1) | 0,5 (0–1,5) | 10,5 (0–19,5) | %6,5 (%1,2–%13) | 50 (33–69,5) |
| 22 | 5 (2–8,5) | 0 (0–2) | 0 (0–2) | 8 (0–18,5) | %6,2 (%2,5–%13) | 51 (35–73,5) |
| 23 | 5 (0,5–8,5) | 0 (0–1) | 1 (0–2,5) | 7 (0–22,5) | %4,1 (%2,3–%15) | 48,5 (31–68,5) |
| 24 | 4,5 (1–7) | 0 (0–1,5) | 0,5 (0–2,5) | 5 (0–20,5) | %5,6 (%1,5–%12) | 52,5 (30,5–78) |
| 25 | 4 (1,5–7) | 0 (0–1) | 1 (0–2) | 6,5 (0–16,5) | %3,9 (%1,2–%14) | 49 (38,5–70,5) |
| 26 | 4 (2–10,5) | 0 (0–1) | 1 (0–2) | 11,5 (3–25) | %6,2 (%2,8–%18) | 52 (32–74,5) |
| 27 | 5 (2–7,5) | 0 (0–1,5) | 1 (0–2) | 3 (0–23,5) | %4,4 (%1,7–%16) | 52,5 (29,5–79) |
| 28 | 4,5 (2–8) | 0 (0–2) | 1 (0–2) | 8 (0–26) | %6,4 (%2,2–%19) | 51 (33,5–88) |
| 29 | 4 (1–7,5) | 0 (0–1) | 1 (0–2,5) | 8,5 (0–30,5) | %6,4 (%1,9–%23) | 51 (34,5–75) |
| 30 | 5,5 (1–8) | 0 (0–1) | 1,5 (0–3,5) | 12 (3–34,5) | %8,1 (%2,6–%22) | 46 (30–66,5) |
| 31 | 5,5 (1,5–10) | 0 (0–1) | 1 (0–2) | 11,5 (0–34) | %7,6 (%3,4–%23) | 47,5 (29,5–75) |
| 32 | 4,5 (1–10) | 0 (0–1) | 1 (0–2,5) | 8,5 (2,5–26) | %7,1 (%2,3–%26) | 50 (34–79,5) |
| 33 | 5 (1–7) | 0 (0–1,5) | 0,5 (0–2) | 8,5 (2–27) | %5,8 (%3,3–%23) | 51,5 (33–77,5) |
| 34 | 6 (1–10) | 0 (0–1) | 0 (0–2) | 8,5 (0–24) | %6,5 (%3–%23) | 49,5 (33,5–79,5) |
| 35 | 6 (2–11,5) | 0 (0–1) | 0,5 (0–3) | 11 (2,5–23,5) | %5,8 (%2,5–%30) | 51 (40–72,5) |
| 36 | 7 (1,5–11,5) | 0 (0–2,5) | 1 (0–3,5) | 9 (4–27,5) | %10 (%3,2–%21) | 52,5 (40–74,5) |
| 37 | 5 (1,5–11) | 0 (0–1) | 1 (0–2) | 7,5 (0–25) | %7,7 (%0,7–%19) | 54 (37,5–74) |
| 38 | 6,5 (0–11) | 0 (0–0,5) | 1 (0–2) | 10 (3–39,5) | %5,6 (%1,4–%20) | 52 (39,5–79,5) |
| 39 | 5 (1–10) | 0 (0–1) | 0,5 (0–3,5) | 7 (1,5–24) | %5,6 (%2,1–%16) | 50 (31–70) |
| 40 | 5,5 (1,5–9) | 0 (0–0,5) | 1 (0–3) | 12,5 (0–29,5) | %7,8 (%3,6–%26) | 55 (41,5–72,5) |
| 41 | 4 (0,5–8) | 0 | 1,5 (0–3) | 17,5 (1,5–36,5) | %8,9 (%4,7–%30) | 49,5 (30,5–70) |
| 42 | 5 (1,5–11,5) | 0 (0–1) | 1,5 (0–3) | 9,5 (1,5–32,5) | %9 (%2,9–%26) | 55,5 (41–68) |
| 43 | 6 (2–11) | 0 (0–1) | 1 (0–2) | 9 (0–37,5) | %11 (%2–%28) | 58,5 (37,5–69,5) |
| 44 | 5 (1,5–12,5) | 0 (0–1) | 1 (0–2) | 18,5 (1,5–40,5) | %10 (%1,9–%24) | 55,5 (40,5–70) |
| 45 | 5 (2–8) | 0 (0–1) | 1 (0–2) | 9,5 (3–35) | %9,9 (%1,7–%24) | 57,5 (40–71) |
| 46 | 6 (1,5–10) | 0 (0–0,5) | 1 (0–3) | 15,5 (2–30) | %9,8 (%1,8–%24) | 50,5 (38–70) |
| 47 | 6 (2,5–11) | 0 (0–1) | 1 (0–2,5) | 12 (3–30,5) | %9,1 (%2,4–%23) | 50 (43,5–68,5) |
| 48 | 6 (2–10) | 0 | 1 (0–3) | 12 (0–25) | %8,7 (%1,8–%24) | 46 (29,5–75) |
| 49 | 7 (3,5–10) | 0 (0–1) | 0 (0–2,5) | 3 (0–29) | %6,3 (%1,5–%22) | 54,5 (33–76) |
| 50 | 5,5 (2–10,5) | 0 (0–1) | 1 (0–4) | 12 (7–42) | %8,1 (%2,3–%32) | 53 (37–78,5) |
| 51 | 7 (3–10,5) | 0 (0–0,5) | 1 (0–3) | 8 (1,5–25) | %9,3 (%4,2–%33) | 53,5 (28,5–69) |
| 52 | 6 (3,5–10,5) | 0 | 0 (0–2) | 16,5 (5,5–24,5) | %9 (%3,5–%28) | 54 (33,5–66,5) |
| 53 | 5,5 (2,5–9,5) | 0 | 1,5 (0–4) | 17 (1,5–56,5) | %9,5 (%2,4–%30) | 47,5 (34–75,5) |
| 54 | 6,5 (2,5–10) | 0 (0–1) | 1,5 (0–2,5) | 15 (5,5–39) | %9,1 (%3,9–%28) | 50 (34,5–71) |
| 55 | 8 (2,5–11) | 0 (0–1) | 1 (0–2) | 13,5 (0–34,5) | %9,3 (%3–%24) | 51,5 (34,5–83) |
| 56 | 7 (2,5–12) | 0 (0–1) | 1 (0–3) | 19,5 (3–35) | %9,4 (%2,7–%26) | 52 (36,5–70,5) |
| 57 | 8,5 (4–14,5) | 0 (0–1) | 1,5 (0–3) | 17,5 (2,5–28,5) | %11 (%4,3–%19) | 51 (37–73) |
| 58 | 6,5 (3,5–12) | 0 (0–1) | 0 (0–2,5) | 13 (0–24,5) | %8 (%2,2–%17) | 55,5 (32–76,5) |
| 59 | 8 (2,5–11,5) | 0 (0–0,5) | 1 (0–2,5) | 19,5 (1,5–34) | %8,4 (%1,6–%23) | 59,5 (37,5–88) |
| 60 | 7,5 (3–13) | 0 (0–1) | 1 (0–3) | 13,5 (3–23,5) | %9,3 (%2–%17) | 59,5 (36–85) |

