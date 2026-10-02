# Ölçüm raporu: Faz 2a: bölge bağı (kamp adları)

16 dünya (seed 1-16) × 60 yıl (2400 gün), 1 yıl = 40 gün · 2026-10-02 12:49 · `FD.Macro.Run stats --seeds 1-16 --years 60 --jobs 2 --verify 1 --saveload 1`

Süre: 16 dk 17 sn duvar saati, 2 iş parçacığı; dünya başına 95 sn (en az 71,6, en çok 214; yıl sonu hash'leri dâhil).

## Bitiş ölçütleri

DESIGN-FAZ1.md, "Bitiş ölçütleri" (1–5), yol haritası v3 (6–7, Faz 1b-4) ve v3'ün süre tablosu (8, Faz 1b-5). ✓ geçti · ✗ kaldı · — ölçülemedi · ○ bilgi (hedef yok) · † zaman ölçeğine bağlı (Faz 1b-5'ten beri takvim yeni ölçekte: 1 yıl = 40 gün, değerler doğrudan okunur).

| # | Ölçüt | Koşul | Ölçülen | Sonuç |
|---|---|---|---|---|
| 1 | Donma yok | 41–60. yılların yıllık büyük olay medyanı ≥ 0,8 × 6–20. yılların medyanı | 262 / 252 = 1,04 kat | ✓ |
| 2 | Çöküş | dünyaların ≥ %75'inde 60 yılda ≥ 1 çöküş (yok olma ya da başkent kaybı) | %100 (16/16 dünya); toplam 174 çöküş: 68 yok olma, 106 başkent kaybı | ✓ |
| 3 | Kamplar | 41–60. yıllarda yaşayan kamp medyanı bantta (8–10, ±1) ve 6–20. yılların en az %90'ı (v3: sabit bant) | geç 8,49, erken 8,55 (oran %99; yıl sonu sayımıyla 9 / 9) | ✓ |
| 4a | Kahraman: doğuş seviyesi | her on yılda doğanların ortalama seviyesi ≤ 2 | 1,62 · 1,62 · 1,63 · 1,67 · 1,63 · 1,65 (on yıllar sırasıyla) | ✓ |
| 4b | Kahraman: Sv8+ | dünyaların ≥ yarısında en az bir kahraman Sv8 ve üstüne çıkar | %100 (16/16 dünya); dünyadaki en yüksek seviye: medyan Sv10, en çok Sv10 | ✓ |
| 4c | Kahraman: efsane | dünya başına efsane medyanı 1–6 | medyan 3 (p10–p90: 1–4,5; toplam 46) | ✓ |
| 4d | Kahraman: ölüm payı | doğan kahramanların %30–80'i ölür | %29 (3109/10760); dünya medyanı %29 (%21–%38) | ✗ |
| 5a | Determinizm | aynı seed → aynı tarih (toplayıcılı ve toplayıcısız koşu, her yıl sonu hash'i) | seed 1: 60/60 yıl sonu aynı | ✓ |
| 5b | Kayıt/yükleme | kaydet → yükle → devam = kesintisiz koşu (her yıl sonu hash'i) | seed 1, gün 1237: yüklenen dünya aynı, sonraki 30/30 yıl sonu aynı | ✓ |
| 6a | Yerleşim sayısı sabit, ısınmadan sonra (21–60. yıl) | dünya başına yaşayan yerleşim sayısının (günlük) değişim katsayısı medyanı ≤ %10 | değişim katsayısı %1,6 (%0,8–%4,9); ortalama 78,2 yerleşim, en az ve en çok ortalamanın %90 ve %102 kadarı | ✓ |
| 6b | Yerleşim sayısı sabit, bütün koşu (1–60. yıl) | dünya başına yaşayan yerleşim sayısının (günlük) değişim katsayısı medyanı ≤ %10 (Faz 1b-6: dünya tarih öncesiyle olgun başlar) | değişim katsayısı %1,6 (%0,9–%8,7); ortalama 78,2 yerleşim, en az ve en çok ortalamanın %90 ve %102 kadarı | ✓ |
| 6c | El değiştirme (her yerleşim) | fetih + bölünme + içeriden düşüş (Faz 1b-7), 100 günde, dünya başına ve yerleşim başına (21–60. yıl); hedef yok | dünyada 7,16 (5,59–9,53) / 100 gün; yerleşim başına 9,4 / 10 bin gün: 7,16 / 100 gün | ○ |
| 6d | Durum dalgalanması (yerleşim başına) | açlık, salgın, kuşatma, yakılma, kademe, el değiştirme, terk, yeniden yerleşim: yerleşim başına 100 günde (21–60. yıl); hedef yok | yerleşim başına 1,51 (1,28–1,85) / 100 gün, yani ~66,2 günde bir; dünyada 117 / 100 gün: yerleşim başına ~66,2 günde bir | ○ |
| 6e | Orta halka: kademe değişimi † | Köy/Kasaba başına kademe değişimi ya da terk, yeni takvimde 60–150 günde bir (21–60. yıl) | yerleşim başına 121 (111–151) günde bir | ✓ |
| 6f | Büyük şehir el değiştirmesi † | büyük şehir (Şehir kademesi) bütün dünyada yeni takvimde 100 günde 2–4 kez el değiştirir (21–60. yıl): dışarıdan (fetih, bölünme) ya da içeriden (Faz 1b-7: tipin çöküş yolu; yönetim değişir ya da şehir ayrılır); felaketle düşmez | dünyada 2,63 (2,22–3,31) / 100 gün; şehir başına 0,52 / 100 gün; dünyada ortalama 4,99 büyük şehir; toplam 685 el değiştirme (57 fetih, 2 bölünme, 626 içeriden: soylu isyanı 197, düello 145, darbe 128, mezhep bölünmesi 93, veraset 22, aforoz 17, boyların ayrılması 13, ayrılık 11). Ayrıştırma: dünyada 5,78 savaş / 100 gün, hedefi büyük şehir olan %15; büyük şehre 288 hücum, düşüşle bitenlerin payı %20 | ✓ |
| 6g | Büyük şehir: uyarı süresi † | büyük şehrin düşüşünden önce yeni takvimde 5–10 gün uyarı (kuşatmanın ya da iç krizin başı → düşüş, medyan; 21–60. yıl) | 7 gün (5–10; n = 683): kuşatmanın başından 5 (n = 57), iç krizin başından 8 (n = 626); savaşın başından 15 gün (10,6–33,2) | ✓ |
| 6h | Savaş süresi † | savaş (ilandan barışa) yeni takvimde 10–40 gün (medyan; 21–60. yıl başlayıp biten savaşlar) | 13 gün (10–40; n = 1537) | ✓ |
| 6i | Kuşatma süresi † | büyük şehir kuşatması (karargâhtan hücuma) yeni takvimde 2–6 gün (medyan; 21–60. yıl) | büyük şehir 6 gün (6–6; n = 288); diğer yerleşimler 3 gün (3–6; n = 1301) | ✓ |
| 6j | Başkent kaybı (medeniyet başına) | hiçbir medeniyet başkentini 3 kereden çok kaybetmez (yol haritası: "en fazla birkaç kez") | en çok 3; 3'ten çok kaybeden 0 medeniyet; toplam 106 başkent kaybı, 75/208 medeniyette (1×: 50, 2×: 19, 3×: 6) | ✓ |
| 7 | Felaket büyük şehri düşürmez | Şehir kademesine varmış yerleşim hiç terk edilmez; ejderha akını büyük şehrin nüfusunu Şehir eşiğinin (85) altına indiremez; akından sonraki 60 günde terk yok | terk edilen eski Şehir: 0; ejderha akını 296 (büyük şehre 26, orada 256 ölü): akınla eşiğin altına inen 0, sonraki 60 günde terk 0; bilgi: akın günü başka nedenlerle (ordu, öncü) eşiğin altına inen 2, 60 gün içinde kademe düşüşü 8, el değiştirme 3 | ✓ |
| 8a | Salgın süresi | salgın başladığı günden bittiği güne: 5–10 gün (medyan) | 6 gün (4–7; n = 576) | ✓ |
| 8b | Tepki inşaatı: yanan ev | yanan her ev yandığı günden onarıldığı güne (ilk yanan ilk onarılır): 1–3 gün (medyan) | 3 gün (2–5; n = 16495) | ✓ |
| 8c | Tepki inşaatı: sur | palisat ve taş sur: projenin başından bitişine: 5–10 gün (medyan) | 7 gün (5–10; n = 852); palisat 7, taş sur 8 gün | ✓ |
| 8d | Büyük proje (kale, kule) | kale ve fener kulesi: projenin başından bitişine: 15–30 gün (medyan) | 25 gün (23–30; n = 352); kale 25 (n = 293), kule 23 gün (n = 59) | ✓ |
| 8e | Temizlenen kamp → yeni köy | temizlenen kara kampının vadisine 60 gün içinde köy kurulması (Faz 1b-8: kampın vadisine giden öncüler ya da kalıcı köye dönen verimli vadi merkezi; yakındaki ilgisiz kuruluşlar sayılmaz): 10–20 gün (medyan) | 13 gün (8–18; n = 308); temizlenen 6028 kampın 308 tanesine (%5,1) köy kuruldu, 20 gün içinde %4,9 | ✓ |
| 8f | Han kurulumu | yeni han: hancının yola çıktığı günden kapıların açıldığı güne: 5–10 gün (medyan) | 6 gün (4,6–9,4; n = 7); harabeyi yeniden kurma 3 gün (n = 7) | ✓ |
| 8g | Kahraman doğumu (han) | han başına 10–20 günde bir (açık han-günü / handa doğan kahraman) | 15,2 günde bir (10561 doğum / 160630 han-günü); aynı handa iki doğum arası medyan 12 gün (2–33); bilgi: taverna başına 5591 günde bir (199 doğum) | ✓ |
| 8h | Kahramanın efsaneye yükselişi | doğumundan efsane olduğu güne: 100–300 gün (medyan) | 294 gün (142–908; n = 46) | ✓ |
| 8i | İlan ömrü | alınmayan ilanın asıldığı günden kapandığı güne (süresi doldu ya da kampı başkası temizledi): 10–20 gün (medyan) | 15 gün (1–15; n = 3606); bütün ilanlar 5 gün (n = 15573): biten %42 (asılıştan 4 günde), süresi dolan %20; başarısız sefer ödülü %25 artırır (Heroes.QuestFailed) | ✓ |
| 8j | Yoldaş/kahraman maaşı | haftalık (5 gün) | kural: medeniyetin kahramanları her 5. gün (Economy.PayHeroes), han personeli haftada bir (InnLife.PayWages); maaşı 3 hafta ödenmeyen kahraman ayrılır | ○ |
| 9a | Örgütler yaşar, şubeleri dalgalanır | hiçbir örgüt kalıcı olarak yok olmaz (koşu sonunda ya yaşıyor ya da yeniden kurulmayı bekliyor); örgüt başına yıllık şube sayısının değişim katsayısı medyanı ≥ %10 | kalıcı yok olan 0/208; koşu sonunda yaşayan 201; dağılma 56, yeniden kuruluş 49; şube değişim katsayısı %19 (%9,3–%65); açılan 15279, kapanan 14810 şube (955 / 926 dünya başına) | ✓ |
| 9b | Gölge savaşı düzenli | dünya-yıllarının ≥ %90'ında en az bir gölge savaşı eylemi (suikast, sabotaj, ihbar); hedef sıklık sonra ayarlanacak | en az bir eylemi olan dünya-yılı %99; dünyada 16,4 (12,8–20,8) / 100 gün; başarısız %55; öldürülen usta/lider 1369; türler: suikast 3191, sabotaj 1683, ihbar 1491 | ✓ |
| 9c | Tiplerin çöküş nedenleri farklı | dört hükümet tipinin en sık çöküş nedeni birbirinden farklı (başkent fethi, bölünme ya da içeriden düşüş: veraset savaşı, soylu isyanı, düello, boyların ayrılması, darbe, paralı askerler, mezhep bölünmesi, aforoz) | Krallık: soylu isyanı (başkent fethi 50, bölünme 20, veraset savaşı 44, soylu isyanı 448, Pakt bağı ve aforoz 21; 2479 devlet-yılı); Boylar: reisin düelloda ölümü (başkent fethi 57, bölünme 11, reisin düelloda ölümü 341, boyların ayrılması 30; 1447 devlet-yılı); Cumhuriyet: darbe (başkent fethi 34, bölünme 30, soylu isyanı 1, darbe 287; 1348 devlet-yılı); Teokrasi: mezhep bölünmesi (başkent fethi 23, bölünme 8, veraset savaşı 1, mezhep bölünmesi 152, Pakt bağı ve aforoz 10; 1237 devlet-yılı) | ✓ |
| 9d | Esaret sınırlı, Özgürlük Ağı etkin | köle payı her dünya-yılında nüfusun ≤ %5'i; Özgürlük Ağı dünyaların ≥ %75'inde köle kurtarır | köle payı medyan %0,5 (p90 %1,5, en çok %2,6); esarete düşen 6999 (av 1409, savaş 2116, borç 2095, baskın 1379); hapis madeni 3984; Özgürlük Ağı 16/16 dünyada 4883 köle kurtardı; kaçan 1020, azat 1016; esir düşen kahraman 1366 | ✓ |
| 9e | Devriye profili tipe göre ayrışır | kervan başına durdurma, el koyma ve rüşvet: tipler arasında en yüksek / en düşük durdurma oranı ≥ 1,5, el koyma ve rüşvet oranları ≥ 2 | Krallık: 100 kervan-günde 7,33 durdurma, durdurmada el koyma %5,4, rüşvet %4,4, haraç/vergi %43, tutuklama 2, düello 0; Boylar: 100 kervan-günde 4,73 durdurma, durdurmada el koyma %2,3, rüşvet %6,2, haraç/vergi %57, tutuklama 0, düello 1985; Cumhuriyet: 100 kervan-günde 6,07 durdurma, durdurmada el koyma %2,1, rüşvet %12, haraç/vergi %69, tutuklama 0, düello 0; Teokrasi: 100 kervan-günde 9,53 durdurma, durdurmada el koyma %8, rüşvet %5,8, haraç/vergi %32, tutuklama 1209, düello 0 (oranlar: durdurma ×2,02, el koyma ×3,76, rüşvet ×2,8) | ✓ |
| 9f | Aç haydutlar kıtlıkla ilişkili | dünya-yılı başına aç ya da ekmeksiz yerleşim payı ile haydut olan aç halk arasında korelasyon r ≥ 0,3 (en az 30 haydut) | r = 0,79 (960 dünya-yılı); haydut olan 11722, aç haydut kampı 2636, yiyecek verilip dağılan 674, açlıktan eriyen 928; aç ya da ekmeksiz yerleşim payı medyanı %6,6 | ✓ |
| 6k | Durum tablosu | her yerleşimde 5–15 günde bir zar (yerleşim-günü / zar ortalaması 5–15); zarla gelen durum 3–15 gün sürer (medyan, p10 ≥ 3, p90 ≤ 15) | zar ortalama 9,99 günde bir (288950 zar); zarla gelen durum 8 gün (4–13; n = 89332); yerleşim-günlerinin %31'inde bir durum var | ✓ |
| 6l | Fırsat merkezi döngüsü | döngüler kurulur ve çöker: her dünyada ≥ 5 tamamlanan döngü; toplam süre (söylentiden sona) medyanı 10–30 gün; evrelerin medyanı hedef aralıkta (söylenti 1–2, hücum 3–6, zirve 4–10, tükeniş 2–6, hayalet 3–6) | toplam 21 gün (17–25; n = 2360); evreler: söylenti 1, hücum 4, zirve 7, tükeniş 4, hayalet 4; dünyada 6,58 (6,29–6,96) / 100 gün, dünya başına en az 127 tamamlanan; türler: maden 683, verimli vadi 551, yol konağı 427, antik harabe 351, kutsal kalıntı 335, ordu pazarı 187; sonuç: hayalet (terk) 2328, söylentide söndü 130, kalıcı köy 32, yıkıldı 20; zirve nüfusu medyanı 23 | ✓ |
| 6m | Tepki inşaatı | sur yalnız tehditten (baskın, kuşatma, yağma, akın) sonra ya da savaşta sınırda kurulur (kural); tehditten sonra sur ortalama ≤ 20 günde başlar; büyük proje (sınır kalesi, fener kulesi) olay olarak gelir, dünya başına 60 yılda 3–60 | sur projesi 869 (tehditten sonra 232, ortalama 7,17 gün sonra; gerisi savaşta sınır boyunda); surusuz yerleşime gelen tehdit 1051; büyük proje dünya başına 23,5 (12,5–30; kale 294, fener 61), sabotaj 96 | ✓ |
| 6n | Büyük şehrin istikrarı ve iç krizler | istikrar (0–100) garnizon, kıtlık, vergi, meşruiyet, savaş yorgunluğu ve durumdan; düşük istikrarda iç kriz (5–10 gün belirti) ve tipin çöküş yolu; hedef yok | büyük şehir ve taht şehri istikrarı 60,2 (33,8–78,8); kriz dünyada 6,77 / 100 gün (n = 2701), belirti 8 gün (5–10), düşüşle biten %49; büyük şehirde 1798 kriz, düşüş %53 | ○ |
| H1 | Dünya donmuyor | her dünyanın her on yılında en az bir yerleşim el değiştirir (fetih, bölünme, komşuya geçiş, içeriden düşüş), bir savaş ya da iç kriz başlar ve bir fırsat merkezi kurulur; yerleşim başına durum değişimi son on yılda ilk on yılın en az %80'i (dünya medyanı); ölçüt 1 (büyük olay) geçer | donmuş on yıl 0/96; durum değişimi son / ilk on yıl %90 (%58–%125); ölçüt 1: 262 / 252 = 1,04 kat | ✓ |
| H2 | Yerleşim sayısı sabit, sahiplik ve durum dalgalı | yerleşim sayısının değişim katsayısı ≤ %10 (6b); on yılda el değiştiren (fetih, bölünme, komşuya geçiş ya da içeriden düşüş) yerleşim payı medyanı ≥ %10, hiçbir dünya-on yılında %2'nin altında değil; on yılda en az bir durum yaşayan yerleşim / ortalama yerleşim sayısı medyanı ≥ 0,9 (on yılda kurulup terk edilenler yüzünden 1'i aşabilir) | yerleşim sayısı: değişim katsayısı %1,6 (%0,9–%8,7); ortalama 78,2 yerleşim, en az ve en çok ortalamanın %90 ve %102 kadarı; on yılda el değiştiren payı %22 (%15–%31; en az %8,7); durum yaşayan yerleşim / ortalama 1,07 (1,01–1,22) | ✓ |
| H3 | Döngüler kuruluyor ve çöküyor | her dünyada: ≥ 5 fırsat merkezi döngüsü tamamlanır; kademe hem yükselir hem düşer (≥ 20 / ≥ 20); en az bir yerleşim harabe olur ve en az bir yerleşim kurulur ya da harabe yeniden iskân edilir; en az bir el değiştirme; örgütler dağılıp yeniden kurulur (9a) | 16/16 dünya; dünya medyanı: tamamlanan merkez 152, harabe 49,5, kuruluş 52 (harabeye yeniden iskân 33,5), doğan devlet 4, yok olan 4; örgüt: kalıcı yok olan 0/208; koşu sonunda yaşayan 201; dağılma 56, yeniden kuruluş 49; şube değişim katsayısı %19 (%9,3–%65); açılan 15279, kapanan 14810 şube (955 / 926 dünya başına) | ✓ |
| H4 | Büyük şehir: el değiştirme ve çekirdek halka | büyük şehir dünyada 100 günde 2–4 kez el değiştirir (6f; içeriden düşüş dâhil, Faz 1b-7'de onaylandı); çekirdek halka 4–6 büyük şehir: dünya-on yılı ortalamalarının medyanı 4–6, hiçbiri 3'ün altında ya da 8'in üstünde değil | dünyada 2,63 (2,22–3,31) / 100 gün; şehir başına 0,52 / 100 gün; dünyada ortalama 4,99 büyük şehir; toplam 685 el değiştirme (57 fetih, 2 bölünme, 626 içeriden: soylu isyanı 197, düello 145, darbe 128, mezhep bölünmesi 93, veraset 22, aforoz 17, boyların ayrılması 13, ayrılık 11). Ayrıştırma: dünyada 5,78 savaş / 100 gün, hedefi büyük şehir olan %15; büyük şehre 288 hücum, düşüşle bitenlerin payı %20; büyük şehir (dünya-on yılı ortalaması) medyan 5,16 (4,26–5,92; en az 3,68, en çok 6) | ✓ |
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
| Yaşayan yerleşim (günlük ortalama) | 78,2 (63,8–79,9) | 78,2 (65,4–79,9) |
| Yaşayan yerleşim: en az (ortalamaya oranı) | %90 (%79–%96) | %90 (%86–%97) |
| Yaşayan yerleşim: en çok (ortalamaya oranı) | %102 (%101–%110) | %102 (%101–%106) |
| Yaşayan yerleşim: değişim katsayısı | %1,6 (%0,9–%8,7) | %1,6 (%0,8–%4,9) |
| Büyük şehir (günlük ortalama) | 5,12 (4,62–5,69) | 4,99 (4,73–5,68) |
| El değiştirme (fetih + bölünme), dünyada / 100 gün | 7,65 (5,71–9,77) | 7,16 (5,59–9,53) |
| El değiştirme, yerleşim başına / 10 bin gün | 10,1 (8,25–13,3) | 9,4 (7,56–12,8) |
| Durum değişimi (kuruluş hariç), yerleşim başına / 100 gün | 1,54 (1,4–1,81) | 1,51 (1,28–1,85) |
| Durum değişimi, dünyada / 100 gün | 118 (86–141) | 117 (84,8–145) |
| Orta halka (Köy/Kasaba) kaydı: yerleşim başına kaç günde bir | 115 (108–129) | 121 (111–151) |
| Büyük şehir el değiştirdi, dünyada / 100 gün | 2,48 (1,98–3,27) | 2,63 (2,22–3,31) |
| Büyük şehir el değiştirdi, şehir başına / 100 gün | 0,51 (0,38–0,63) | 0,52 (0,41–0,64) |
| Büyük şehre hücum, dünyada / 100 gün | 1,17 (0,85–1,67) | 1,13 (0,72–1,59) |
| Büyük şehir yağmalandı ama tutulmadı, dünyada / 100 gün | 0,35 (0,19–0,88) | 0,38 (0,13–0,91) |

Durum değişimi türleri (yerleşim başına 10 bin günde; bölünme dışında olayın kademesi olaydan önceki):

| Tür | bütün koşu (1–60. yıl) | ısınmadan sonra (21–60. yıl) |
|---|---|---|
| Fetih (`capture`) | 6,18 (4,48–8,62) | 5,77 (3,89–8,23) |
| Bölünme (`secede`) | 0,21 (0,08–0,32) | 0,17 (0,04–0,32) |
| İçeriden düşüş (yönetim değişti) (`regime`) | 3,89 (2,74–4,94) | 3,92 (2,52–4,93) |
| Açlık başladı (`famine`) | 16,3 (3,12–41,7) | 17,6 (2,85–53,4) |
| Salgın başladı (`plague`) | 2,08 (1,32–2,74) | 1,95 (1,53–2,71) |
| Kuşatma (hücum) (`siege`) | 8,29 (6,59–10,5) | 7,78 (6,22–10,4) |
| Yakıldı / yandı (`burn`) | 30,3 (19,8–40,1) | 29,5 (18,6–43,4) |
| Kademe yükseldi (`tierUp`) | 39,6 (35,9–46,1) | 36,9 (31–44,1) |
| Kademe düştü (`tierDown`) | 40,3 (37,7–46,2) | 39,4 (33,1–44,6) |
| Terk (harabe) (`abandon`) | 2,96 (1,21–4,56) | 2,7 (1,17–4,33) |
| Harabeye yeniden yerleşim (`resettle`) | 1,79 (0,83–3,48) | 1,67 (0,94–3,4) |
| Kuruluş (durum değişimi sayılmaz) (`found`) | 0,9 (0,47–1,47) | 0,7 (0,28–1,32) |

Süreler (gün; bütün dünyalar havuzlanmış; savaş: ilandan ilişkiden düştüğü güne, koşu sonunda süren savaşlar hariç; kuşatma: hedefin önündeki ilk karargâhtan hücuma, karargâhsız hücum 1 gün; uyarı: büyük şehrin düşüşünden geriye):

| Süre | Pencere | n | p10 | medyan | p90 | ortalama | en çok |
|---|---|---|---|---|---|---|---|
| Savaş (hepsi) | bütün koşu (1–60. yıl) | 2408 | 10 | 13 | 40 | 20,2 | 78 |
| Savaş: sıradan | bütün koşu (1–60. yıl) | 1278 | 10 | 13 | 40 | 20,6 | 78 |
| Savaş: haraç | bütün koşu (1–60. yıl) | 13 | 11 | 37 | 75,2 | 39 | 77 |
| Savaş: tarihî hak | bütün koşu (1–60. yıl) | 306 | 10 | 13 | 35 | 18,1 | 78 |
| Savaş: fırsat | bütün koşu (1–60. yıl) | 140 | 10 | 15 | 45 | 23,4 | 78 |
| Savaş: Kutsal Sefer | bütün koşu (1–60. yıl) | 262 | 10 | 16 | 55 | 26,9 | 78 |
| Savaş: savunma paktı | bütün koşu (1–60. yıl) | 185 | 8,8 | 12 | 19,2 | 13,2 | 53 |
| Savaş: müttefik çağrısı | bütün koşu (1–60. yıl) | 224 | 10 | 12 | 35 | 16,4 | 65 |
| Kuşatma: büyük şehir | bütün koşu (1–60. yıl) | 472 | 6 | 6 | 6 | 5,98 | 8 |
| Kuşatma: diğer yerleşimler | bütün koşu (1–60. yıl) | 2012 | 3 | 3 | 6 | 3,83 | 9 |
| Uyarı: kuşatmanın başı → büyük şehrin düşüşü | bütün koşu (1–60. yıl) | 102 | 5 | 5 | 5 | 4,9 | 5 |
| Uyarı: savaşın başı → büyük şehrin düşüşü | bütün koşu (1–60. yıl) | 102 | 11 | 18 | 40,8 | 22,9 | 76 |
| Savaş (hepsi) | ısınmadan sonra (21–60. yıl) | 1537 | 10 | 13 | 40 | 20,4 | 78 |
| Savaş: sıradan | ısınmadan sonra (21–60. yıl) | 835 | 10 | 13 | 40 | 21,1 | 78 |
| Savaş: haraç | ısınmadan sonra (21–60. yıl) | 8 | 13,5 | 39 | 70,7 | 40,4 | 77 |
| Savaş: tarihî hak | ısınmadan sonra (21–60. yıl) | 186 | 10 | 13 | 35 | 17,8 | 78 |
| Savaş: fırsat | ısınmadan sonra (21–60. yıl) | 88 | 10 | 13 | 45,9 | 23,8 | 78 |
| Savaş: Kutsal Sefer | ısınmadan sonra (21–60. yıl) | 167 | 10 | 15 | 53,8 | 26,1 | 78 |
| Savaş: savunma paktı | ısınmadan sonra (21–60. yıl) | 110 | 10 | 12 | 17,1 | 12,9 | 35 |
| Savaş: müttefik çağrısı | ısınmadan sonra (21–60. yıl) | 143 | 10 | 12 | 35 | 15,8 | 45 |
| Kuşatma: büyük şehir | ısınmadan sonra (21–60. yıl) | 288 | 6 | 6 | 6 | 5,97 | 8 |
| Kuşatma: diğer yerleşimler | ısınmadan sonra (21–60. yıl) | 1301 | 3 | 3 | 6 | 3,85 | 7 |
| Uyarı: kuşatmanın başı → büyük şehrin düşüşü | ısınmadan sonra (21–60. yıl) | 57 | 5 | 5 | 5 | 4,91 | 5 |
| Uyarı: savaşın başı → büyük şehrin düşüşü | ısınmadan sonra (21–60. yıl) | 57 | 10,6 | 15 | 33,2 | 20,3 | 76 |

Koşu sonunda süren savaş: 22 (sürelere girmedi).

Kuşatma sonuçları (bütün koşu): büyük şehir: 472 hücum: 103 el değiştirdi, 172 yağmalandı (tutulmadı), 197 püskürtüldü; diğer: 2012 hücum: 1547 el değiştirdi, 367 yağmalandı (tutulmadı), 98 püskürtüldü.

Büyük şehrin el değiştirmesi (bütün koşu, 1043; ilk 60):

| Seed | Gün (yıl) | Şehir | Nasıl | Önceki → yeni sahip | Kuşatma → düşüş | Savaş → düşüş |
|---|---|---|---|---|---|---|
| 1 | 35 (1) | Sarıhisar | içeriden: darbe | Demirkanat Cumhuriyeti → Demirkanat Cumhuriyeti | kriz 9 gün | – |
| 1 | 43 (2) | Karlıtepe | içeriden: soylu isyanı | Kızılboynuz Cumhuriyeti → Kızılboynuz Cumhuriyeti | kriz 5 gün | – |
| 1 | 51 (2) | Karlıtepe | fetih | Kızılboynuz Cumhuriyeti → Yeminköprü Teokrasisi | 5 gün | 31 gün |
| 1 | 71 (2) | Aksırt | içeriden: soylu isyanı | Granitsunak Cumhuriyeti → Granitsunak Cumhuriyeti | kriz 8 gün | – |
| 1 | 73 (2) | Sarıhisar | içeriden: darbe | Demirkanat Cumhuriyeti → Demirkanat Cumhuriyeti | kriz 10 gün | – |
| 1 | 97 (3) | Aksırt | içeriden: soylu isyanı | Granitsunak Cumhuriyeti → Granitsunak Cumhuriyeti | kriz 9 gün | – |
| 1 | 98 (3) | Aksırt | fetih | Granitsunak Cumhuriyeti → Demirçan Cumhuriyeti | 5 gün | 28 gün |
| 1 | 113 (3) | Karlıtepe | içeriden: mezhep bölünmesi | Yeminköprü Teokrasisi → Yeminköprü Teokrasisi | kriz 6 gün | – |
| 1 | 178 (5) | Karlıtepe | içeriden: mezhep bölünmesi | Yeminköprü Teokrasisi → Yeminköprü Teokrasisi | kriz 9 gün | – |
| 1 | 209 (6) | Karlıtepe | içeriden: mezhep bölünmesi | Yeminköprü Teokrasisi → Yeminköprü Teokrasisi | kriz 5 gün | – |
| 1 | 241 (7) | Sarıhisar | içeriden: ayrılık | Demirkanat Cumhuriyeti → Yeminköprü Teokrasisi | kriz 10 gün | – |
| 1 | 324 (9) | Kırbaç Vadisi | içeriden: darbe | Sisliyamaç Krallığı → Demirkanat Cumhuriyeti | kriz 8 gün | – |
| 1 | 359 (9) | Sarıhisar | içeriden: darbe | Demirkanat Cumhuriyeti → Demirkanat Cumhuriyeti | kriz 6 gün | – |
| 1 | 383 (10) | Kızıltepe | içeriden: ayrılık | Demirkanat Cumhuriyeti → Kızılboynuz Cumhuriyeti | kriz 8 gün | – |
| 1 | 387 (10) | Dinginpınar | içeriden: ayrılık | Demirkanat Cumhuriyeti → Yeminköprü Teokrasisi | kriz 6 gün | – |
| 1 | 479 (12) | Karlıtepe | içeriden: mezhep bölünmesi | Yeminköprü Teokrasisi → Yeminköprü Teokrasisi | kriz 8 gün | – |
| 1 | 670 (17) | Demirbük | fetih | Granitsunak Cumhuriyeti → Demirçan Cumhuriyeti | 5 gün | 25 gün |
| 1 | 681 (18) | Karlıtepe | içeriden: mezhep bölünmesi | Yeminköprü Teokrasisi → Yeminköprü Teokrasisi | kriz 10 gün | – |
| 1 | 721 (19) | Karlıtepe | içeriden: mezhep bölünmesi | Yeminköprü Teokrasisi → Yeminköprü Teokrasisi | kriz 8 gün | – |
| 1 | 726 (19) | Aksırt | içeriden: ayrılık | Demirkanat Cumhuriyeti → Yeminköprü Teokrasisi | kriz 10 gün | – |
| 1 | 765 (20) | Kızıltepe | içeriden: darbe | Demirkanat Cumhuriyeti → Demirkanat Cumhuriyeti | kriz 6 gün | – |
| 1 | 772 (20) | Karaköy | içeriden: soylu isyanı | Demirçan Cumhuriyeti → Demirçan Cumhuriyeti | kriz 9 gün | – |
| 1 | 1014 (26) | Karaköy | içeriden: soylu isyanı | Demirçan Cumhuriyeti → Demirçan Cumhuriyeti | kriz 5 gün | – |
| 1 | 1049 (27) | Karaköy | içeriden: soylu isyanı | Demirçan Cumhuriyeti → Demirçan Cumhuriyeti | kriz 8 gün | – |
| 1 | 1075 (27) | Karaköy | içeriden: soylu isyanı | Demirçan Cumhuriyeti → Demirçan Cumhuriyeti | kriz 9 gün | – |
| 1 | 1108 (28) | Karaköy | içeriden: soylu isyanı | Demirçan Cumhuriyeti → Demirçan Cumhuriyeti | kriz 8 gün | – |
| 1 | 1293 (33) | Yeni Sisova | içeriden: ayrılık | Demirkanat Cumhuriyeti → Kızılboynuz Cumhuriyeti | kriz 6 gün | – |
| 1 | 1310 (33) | Karlıtepe | fetih | Yeminköprü Teokrasisi → Granitsunak Cumhuriyeti | 5 gün | 35 gün |
| 1 | 1330 (34) | Karaköy | fetih | Demirçan Cumhuriyeti → Demirkanat Cumhuriyeti | 5 gün | 25 gün |
| 1 | 1336 (34) | Karlıtepe | içeriden: soylu isyanı | Granitsunak Cumhuriyeti → Granitsunak Cumhuriyeti | kriz 8 gün | – |
| 1 | 1342 (34) | Taşkandil | içeriden: ayrılık | Demirkanat Cumhuriyeti → Granitsunak Cumhuriyeti | kriz 10 gün | – |
| 1 | 1378 (35) | Söğütburç | içeriden: soylu isyanı | Granitsunak Cumhuriyeti → Balköy Cumhuriyeti | kriz 8 gün | – |
| 1 | 1379 (35) | Karlıtepe | içeriden: soylu isyanı | Granitsunak Cumhuriyeti → Granitsunak Cumhuriyeti | kriz 7 gün | – |
| 1 | 1399 (35) | Karaköy | içeriden: darbe | Demirkanat Cumhuriyeti → Demirkanat Cumhuriyeti | kriz 7 gün | – |
| 1 | 1408 (36) | Taşkandil | içeriden: soylu isyanı | Granitsunak Cumhuriyeti → Demirkanat Cumhuriyeti | kriz 8 gün | – |
| 1 | 1413 (36) | Karlıtepe | içeriden: soylu isyanı | Granitsunak Cumhuriyeti → Granitsunak Cumhuriyeti | kriz 9 gün | – |
| 1 | 1439 (36) | Söğütburç | içeriden: darbe | Balköy Cumhuriyeti → Balköy Cumhuriyeti | kriz 7 gün | – |
| 1 | 1440 (36) | Karlıtepe | içeriden: darbe | Granitsunak Cumhuriyeti → Granitsunak Cumhuriyeti | kriz 6 gün | – |
| 1 | 1449 (37) | Çamova | içeriden: soylu isyanı | Demirçan Cumhuriyeti → Demirçan Cumhuriyeti | kriz 5 gün | – |
| 1 | 1459 (37) | Karlıtepe | fetih | Granitsunak Cumhuriyeti → Demirkanat Cumhuriyeti | 5 gün | 21 gün |
| 1 | 1477 (37) | Çamova | içeriden: soylu isyanı | Demirçan Cumhuriyeti → Demirçan Cumhuriyeti | kriz 6 gün | – |
| 1 | 1485 (38) | Karlıtepe | içeriden: darbe | Demirkanat Cumhuriyeti → Demirkanat Cumhuriyeti | kriz 5 gün | – |
| 1 | 1504 (38) | Çamova | içeriden: soylu isyanı | Demirçan Cumhuriyeti → Demirçan Cumhuriyeti | kriz 6 gün | – |
| 1 | 1515 (38) | Karaköy | içeriden: ayrılık | Demirkanat Cumhuriyeti → Demirçan Cumhuriyeti | kriz 6 gün | – |
| 1 | 1523 (39) | Karlıtepe | içeriden: darbe | Demirkanat Cumhuriyeti → Demirkanat Cumhuriyeti | kriz 7 gün | – |
| 1 | 1611 (41) | Yeni Kurtyurt | içeriden: soylu isyanı | Kızılboynuz Cumhuriyeti → Kızılboynuz Cumhuriyeti | kriz 9 gün | – |
| 1 | 1617 (41) | Karlıtepe | içeriden: darbe | Demirkanat Cumhuriyeti → Demirkanat Cumhuriyeti | kriz 7 gün | – |
| 1 | 1895 (48) | Karlıtepe | içeriden: darbe | Demirkanat Cumhuriyeti → Demirkanat Cumhuriyeti | kriz 9 gün | – |
| 1 | 1933 (49) | Karlıtepe | içeriden: darbe | Demirkanat Cumhuriyeti → Demirkanat Cumhuriyeti | kriz 9 gün | – |
| 1 | 1962 (50) | Yeşilkaya | içeriden: darbe | Sisliyamaç Krallığı → Sisliyamaç Krallığı | kriz 6 gün | – |
| 1 | 1998 (50) | Karlıtepe | içeriden: soylu isyanı | Demirkanat Cumhuriyeti → Demirkanat Cumhuriyeti | kriz 6 gün | – |
| 1 | 2041 (52) | Yeşilkaya | içeriden: soylu isyanı | Sisliyamaç Krallığı → Sisliyamaç Krallığı | kriz 8 gün | – |
| 1 | 2053 (52) | Karlıtepe | fetih | Demirkanat Cumhuriyeti → Demirçan Cumhuriyeti | 5 gün | 15 gün |
| 1 | 2241 (57) | Yeni Pınaroba | içeriden: darbe | Granitsunak Cumhuriyeti → Granitsunak Cumhuriyeti | kriz 8 gün | – |
| 1 | 2247 (57) | Karlıçayır | içeriden: darbe | Kızılboynuz Cumhuriyeti → Kızılboynuz Cumhuriyeti | kriz 8 gün | – |
| 1 | 2266 (57) | Karlıtepe | içeriden: soylu isyanı | Demirçan Cumhuriyeti → Demirçan Cumhuriyeti | kriz 9 gün | – |
| 1 | 2317 (58) | Söğütburç | içeriden: mezhep bölünmesi | Yeminköprü Teokrasisi → Yeminköprü Teokrasisi | kriz 10 gün | – |
| 1 | 2360 (59) | Demirkanat | fetih | Demirkanat Krallığı → Demirkanat Cumhuriyeti | 5 gün | 27 gün |
| 1 | 2377 (60) | Demirkanat | içeriden: darbe | Demirkanat Cumhuriyeti → Demirkanat Cumhuriyeti | kriz 5 gün | – |
| 2 | 5 (1) | Pınarhisar | içeriden: soylu isyanı | Pulzırh Cumhuriyeti → Kurtoba Boyları | kriz 10 gün | – |

## v3: süre tablosu (Faz 1b-5)

Yol haritası v3'ün hedef süreleri (oyun günü; 1 yıl = 40 gün, 1 ay = 10, 1 hafta = 5). Bütün dünyalar havuzlanmış; 6e–6i ısınmadan sonra, 8a–8i bütün koşu. Ajan hızları `Core/Time.cs` (`Pace`).

| Süreç | Hedef | Ölçülen | Sonuç |
|---|---|---|---|
| Orta halkada kademe kayması (6e) | yerleşim başına 60–150 günde bir | yerleşim başına 121 (111–151) günde bir | ✓ |
| Büyük şehir el değiştirmesi (6f) | dünyada 100 günde 2–4 | dünyada 2,63 (2,22–3,31) / 100 gün; şehir başına 0,52 / 100 gün; dünyada ortalama 4,99 büyük şehir; toplam 685 el değiştirme (57 fetih, 2 bölünme, 626 içeriden: soylu isyanı 197, düello 145, darbe 128, mezhep bölünmesi 93, veraset 22, aforoz 17, boyların ayrılması 13, ayrılık 11). Ayrıştırma: dünyada 5,78 savaş / 100 gün, hedefi büyük şehir olan %15; büyük şehre 288 hücum, düşüşle bitenlerin payı %20 | ✓ |
| ↳ uyarı (6g) | 5–10 gün önceden | 7 gün (5–10; n = 683): kuşatmanın başından 5 (n = 57), iç krizin başından 8 (n = 626); savaşın başından 15 gün (10,6–33,2) | ✓ |
| Savaş (6h) | 10–40 gün | 13 gün (10–40; n = 1537) | ✓ |
| ↳ kuşatma (6i) | 2–6 gün | büyük şehir 6 gün (6–6; n = 288); diğer yerleşimler 3 gün (3–6; n = 1301) | ✓ |
| Salgın (8a) | 5–10 gün | 6 gün (4–7; n = 576) | ✓ |
| Tepki inşaatı: yanan ev (8b) | 1–3 gün | 3 gün (2–5; n = 16495) | ✓ |
| Tepki inşaatı: sur (8c) | 5–10 gün | 7 gün (5–10; n = 852); palisat 7, taş sur 8 gün | ✓ |
| Büyük proje: kale, kule (8d) | 15–30 gün | 25 gün (23–30; n = 352); kale 25 (n = 293), kule 23 gün (n = 59) | ✓ |
| Temizlenen kamp → yeni köy (8e) | 10–20 gün | 13 gün (8–18; n = 308); temizlenen 6028 kampın 308 tanesine (%5,1) köy kuruldu, 20 gün içinde %4,9 | ✓ |
| Han kurulumu (8f) | 5–10 gün | 6 gün (4,6–9,4; n = 7); harabeyi yeniden kurma 3 gün (n = 7) | ✓ |
| Kahraman doğumu (8g) | han başına 10–20 günde bir | 15,2 günde bir (10561 doğum / 160630 han-günü); aynı handa iki doğum arası medyan 12 gün (2–33); bilgi: taverna başına 5591 günde bir (199 doğum) | ✓ |
| Efsaneye yükseliş (8h) | 100–300 gün | 294 gün (142–908; n = 46) | ✓ |
| İlan ömrü (8i) | 10–20 gün (başarısız sefer ödülü +%25) | 15 gün (1–15; n = 3606); bütün ilanlar 5 gün (n = 15573): biten %42 (asılıştan 4 günde), süresi dolan %20; başarısız sefer ödülü %25 artırır (Heroes.QuestFailed) | ✓ |
| Yoldaş maaşı (8j) | haftalık (5 gün) | kural: medeniyetin kahramanları her 5. gün (Economy.PayHeroes), han personeli haftada bir (InnLife.PayWages); maaşı 3 hafta ödenmeyen kahraman ayrılır | ○ |

Proje süreleri, yapı türüne göre (gün; bütün koşu):

| Yapı | n | p10 | medyan | p90 |
|---|---|---|---|---|
| castle | 293 | 25 | 25 | 30 |
| extract | 29887 | 1 | 1 | 1 |
| guild | 79 | 1 | 1 | 1 |
| house | 17706 | 1 | 1 | 1 |
| hut | 84 | 1 | 1 | 1 |
| library | 366 | 1 | 1 | 1 |
| lighthouse | 59 | 23 | 23 | 27 |
| market | 592 | 1 | 1 | 1 |
| mint | 526 | 1 | 1 | 1 |
| palisade | 714 | 5 | 7 | 10 |
| ship | 2024 | 1 | 1 | 2 |
| shipyard | 247 | 1 | 1 | 3 |
| stonehouse | 466 | 1 | 1 | 1 |
| stonewall | 138 | 7 | 8 | 9 |
| tavern | 233 | 1 | 1 | 1,8 |
| temple | 749 | 1 | 2 | 2 |
| unique | 396 | 1 | 1 | 2 |
| upgrade | 2305 | 1 | 1 | 2 |
| workshop | 3577 | 1 | 1 | 2 |

## Devlet, inanç ve örgüt (Faz 1b-6)

claude/devlet-orgut-spec.md: 4–6 devlet (dört hükümet tipi her dünyada), yerleşimlerin ırk ve inanç dağılımı, 13 örgüt (merkezleri çekirdek şehirlerde). Dünya tarih öncesiyle (1600 gün) olgun başlar; örgütler tarih öncesinin sonunda kurulur, ölçüm o günden başlar. Örgüt kararları ~10 günde bir.

Devletler: dünya başına 6 (ilk yıl, medyan); devlet-yılı tipe göre: Krallık 2479, Boylar 1447, Cumhuriyet 1348, Teokrasi 1237. Yönetici değişimi 1251 (veraset 29, veraset krizi 12, düello 1, boy meclisi 5, konsey oyu 61, tarikat 37); seçim 310; reise meydan okuma 641. Pakt'ın sızdığı yönetici 112, ortaya çıkan 65; Pakt suikastı 27; Tarikat'ın Kutsal Sefer çağrısı 520, başlayan sefer 101. Lobiyle yasa değişikliği 1157 (kölelik yasası 0), darbe girişimi 38.

Örgütler (koşu sonu; dünyalar arası medyan, toplamlar bütün dünyalar):

| Örgüt | yaşıyor (dünya) | şube | üye | gizli şube payı | dağılma / yeniden kuruluş | açılan / kapanan şube | gölge savaşı | üye kahraman işi |
|---|---|---|---|---|---|---|---|---|
| Güneş Kilisesi | 16/16 | 56,5 | 448 | %0 | 0 / 0 | 2385 / 1473 | 794 | 1443 |
| Güneş Tarikatı | 16/16 | 10,5 | 94,5 | %0 | 0 / 0 | 1898 / 1722 | 769 | 323 |
| Kara Pakt | 10/16 | 3 | 8 | %100 | 6 / 41 | 1064 / 1023 | 762 | 23 |
| Druid Çemberi | 16/16 | 39 | 133 | %0 | 0 / 0 | 1347 / 703 | 0 | 1428 |
| Avcılar Locası | 16/16 | 17,5 | 54 | %0 | 0 / 0 | 656 / 355 | 0 | 0 |
| Hırsızlar Loncası | 16/16 | 7 | 44 | %55 | 0 / 0 | 1367 / 1242 | 1150 | 293 |
| Büyücü Akademisi | 16/16 | 13 | 90 | %0 | 0 / 0 | 1763 / 1561 | 0 | 1875 |
| Paralı Bölükler | 16/16 | 13,5 | 76 | %0 | 0 / 0 | 2081 / 1863 | 0 | 1822 |
| Ozanlar Koleji | 16/16 | 46 | 160 | %0 | 0 / 0 | 1412 / 700 | 0 | 0 |
| Tüccarlar Loncası | 16/16 | 57,5 | 234 | %0 | 0 / 0 | 2462 / 1563 | 1171 | 0 |
| Harabe Kâşifleri | 16/16 | 41 | 205 | %0 | 0 / 0 | 1532 / 935 | 0 | 598 |
| Köle Avcıları | 15/16 | 17 | 56 | %0 | 1 / 5 | 1340 / 1060 | 872 | 0 |
| Özgürlük Ağı | 16/16 | 7,5 | 21,5 | %5,4 | 0 / 3 | 758 / 610 | 847 | 0 |

Örgüt operasyonları (bütün dünyalar): artifact 3840, news 3840, vigil 3618, pilgrims 3500, rite 3469, caravan 3371, track 3062, hire 2461, smuggle 2115, map 1970, ruin 1870, rob 1725, escort 1379, soul 1327, whisper 1258, bounty 778, famineOrder 469, sabotage 371, heal 294, patrol 203, infiltrate 112, unmask 65, assassinate 27.

Devriye (hükümet tipine göre; bütün dünyalar):

| Tip | maruz ajan-günü | 100 kervan-günde durdurma | durdurmada el koyma | rüşvet | haraç/vergi | düello | kahraman tutuklama | el konan değer | vergi/haraç altını |
|---|---|---|---|---|---|---|---|---|---|
| Krallık | 271687 | 7,33 | %5,4 | %4,4 | %43 | 0 | 2 | 4391 | 5338 |
| Boylar | 110429 | 4,73 | %2,3 | %6,2 | %57 | 1985 | 0 | 390 | 2781 |
| Cumhuriyet | 153548 | 6,07 | %2,1 | %12 | %69 | 0 | 0 | 909 | 1185 |
| Teokrasi | 203616 | 9,53 | %8 | %5,8 | %32 | 0 | 1209 | 6441 | 3101 |

Çöküş nedenleri (hükümet tipine göre; bütün dünyalar; 100 devlet-yılı başına):

| Tip | devlet-yılı | başkent fethi | bölünme | veraset savaşı | soylu isyanı | reisin düelloda ölümü | boyların ayrılması | darbe | paralı askerler (iflas) | mezhep bölünmesi | Pakt bağı ve aforoz |
|---|---|---|---|---|---|---|---|---|---|---|---|
| Krallık | 2479 | 50 (2,02) | 20 (0,81) | 44 (1,77) | 448 (18,1) | 0 (0) | 0 (0) | 0 (0) | 0 (0) | 0 (0) | 21 (0,85) |
| Boylar | 1447 | 57 (3,94) | 11 (0,76) | 0 (0) | 0 (0) | 341 (23,6) | 30 (2,07) | 0 (0) | 0 (0) | 0 (0) | 0 (0) |
| Cumhuriyet | 1348 | 34 (2,52) | 30 (2,23) | 0 (0) | 1 (0,07) | 0 (0) | 0 (0) | 287 (21,3) | 0 (0) | 0 (0) | 0 (0) |
| Teokrasi | 1237 | 23 (1,86) | 8 (0,65) | 1 (0,08) | 0 (0) | 0 (0) | 0 (0) | 0 (0) | 0 (0) | 152 (12,3) | 10 (0,81) |

## Dünyanın durumu (Faz 1b-7)

Yol haritası v3: yerleşim durum tablosu (her yerleşimde 5–15 günde bir zar), büyük şehrin istikrarı ve tipe göre iç çöküş yolları, fırsat merkezi döngüsü, tepki inşaatı. Bütün dünyalar havuzlanmış, bütün koşu.

Durumlar (yerleşim başına 1000 günde kaç kez; süre: gün, medyan (p10–p90)):

| Durum | 1000 yerleşim-gününde | süre | büyük şehirde payı |
|---|---|---|---|
| Refah (`prosper`) | 7,4 | 11 (6–15) | %6,5 |
| Ticaret patlaması (`boom`) | 4,73 | 7 (4–10) | %9,9 |
| Kaynak bulundu (`found`) | 4,55 | 8 (5–12) | %5,2 |
| Göç dalgası (`migration`) | 4,37 | 7 (4–10) | %4,6 |
| Festival (`festival`) | 3,88 | 4 (3–5) | %6,5 |
| Kaynak tükendi (`depleted`) | 3,71 | 10 (5–14) | %5,7 |
| Kıtlık (hasat) (`shortage`) | 2,28 | 10 (5–14) | %7 |
| Kıtlık (açlık) (`hunger`) | 2,06 | 9 (1–51) | %11 |
| Kuşatma (`siege`) | 0,86 | 1 (1–5) | %19 |
| İşgal (`occupation`) | 0,57 | 11 (8–14) | %22 |
| Yeni lord (`newlord`) | 0,44 | 7 (5–10) | %67 |
| Salgın (`plague`) | 0,2 | 5 (4–7) | %48 |
| Canavar tehdidi (`monsters`) | 0,12 | 8 (4–11) | %6,8 |

Göç: göçmen kafilesi 105167, kaçan 141540, gelen 253839 kişi; basamak kaymasına yetecek göç denemesi: yukarı 6884, aşağı 1820.

İç krizler (hükümet tipine göre; düşüş: krizin sonunda yerleşim içeriden düştü):

| Tip | Kriz | n | büyük şehirde | düşüş payı | belirti (gün, medyan) |
|---|---|---|---|---|---|
| Boylar | Meydan okuma | 680 | 395 | %51 | 8 |
| Boylar | Ayrılık | 58 | 58 | %52 | 7 |
| Krallık | Soylu isyanı | 912 | 607 | %49 | 8 |
| Krallık | Veraset kavgası | 59 | 42 | %71 | 8 |
| Krallık | Mezhep çatışması | 1 | 1 | %0 | 6 |
| Cumhuriyet | Darbe söylentisi | 584 | 336 | %49 | 7 |
| Cumhuriyet | Ayrılık | 48 | 48 | %45 | 7 |
| Teokrasi | Mezhep çatışması | 371 | 319 | %42 | 8 |

İstikrar (büyük şehir ve taht şehri, 5 günde bir örnek, n = 58511): p10 33,8, p25 47,7, medyan 60,2, p75 70,8, p90 78,8; 40'ın altında %15.
İçeriden düşüş: 1353 (büyük şehirde 936); yollar: veraset 42, soylu isyanı 449, düello 341, boyların ayrılması 30, darbe 287, mezhep bölünmesi 152, aforoz 31, ayrılık 21. Tip değişimi 122 (kingdom → republic 56, republic → kingdom 42, kingdom → theocracy 13, theocracy → kingdom 11); birleşme (evlilik ittifakı) 1; komşuya geçen şehir 204; Kutsal Sefer yenilgisi 32.

Fırsat merkezleri (türe göre; süre: söylentiden sona, gün):

| Tür | n | süre | zirve nüfusu | sonuç | altın (toplam) |
|---|---|---|---|---|---|
| maden | 683 | 22 (18–26) | 23 | hayalet (terk) 644, söylentide söndü 25, yıkıldı 5 | 6295 |
| verimli vadi | 551 | 21 (17–25) | 18 | hayalet (terk) 471, kalıcı köy 32, söylentide söndü 45, yıkıldı 1 | 0 |
| yol konağı | 427 | 22 (18–26) | 22 | hayalet (terk) 389, söylentide söndü 30, yıkıldı 3 | 2022 |
| antik harabe | 351 | 21 (17–25) | 21 | hayalet (terk) 322, yıkıldı 4, söylentide söndü 22 | 1191 |
| kutsal kalıntı | 335 | 22 (17–25) | 25 | hayalet (terk) 326, yıkıldı 6 | 1333 |
| ordu pazarı | 187 | 18 (12–23) | 22 | hayalet (terk) 176, söylentide söndü 8, yıkıldı 1 | 867 |

Aynı anda yaşayan merkez (dünya başına, günlük ortalama): 1,25 (1,16–1,3). Zirvede gelen haydut kampı 839, hayalet kasabaya yerleşen goblin 952, kalıcı köy olan vadi 32.

## Başsız ölçütler: on yıllar (Faz 1b-8)

Yol haritası Faz 1b/8 (H1–H5, ölçüt tablosunda). Dünya-on yılı pencereleri (400 gün; tarih öncesinden sonra); hücre: dünya medyanı (p10–p90). El değiştiren: fetih, bölünme, komşuya geçiş ya da içeriden düşüş yaşayan yerleşim payı; durum değişimi: kuruluş dışındaki bütün olaylar (kademe, yakılma, kıtlık, kuşatma, salgın, el değiştirme, terk, yeniden iskân), yerleşim başına.

| Ölçü | 1–10. yıl | 11–20. yıl | 21–30. yıl | 31–40. yıl | 41–50. yıl | 51–60. yıl |
|---|---|---|---|---|---|---|
| Kalıcı yerleşim | 77,8 (62,3–79,9) | 78,3 (61,7–79,9) | 78,2 (65,8–79,5) | 78,2 (65,5–80) | 78,2 (65,9–80) | 78 (67–80,1) |
| El değiştiren payı | %24 (%19–%30) | %21 (%16–%32) | %21 (%15–%28) | %22 (%18–%30) | %20 (%14–%29) | %22 (%14–%30) |
| Durum yaşayan / yerleşim | 1,06 (1,02–1,23) | 1,06 (1,02–1,23) | 1,07 (1,02–1,25) | 1,04 (1,01–1,19) | 1,07 (1,01–1,15) | 1,09 (1,03–1,18) |
| Durum değişimi / yerleşim | 6,81 (5,85–7,99) | 6,24 (5,45–8,28) | 5,98 (4,66–7,08) | 6,08 (5,29–7,65) | 5,89 (4,74–7,05) | 5,69 (4,09–9,56) |
| Savaş + iç kriz | 56 (41–76,5) | 57,5 (38–74) | 49 (33,5–69) | 48 (41–70) | 49 (38,5–67) | 48,5 (37,5–76) |
| Savaş | 26,5 (19–40,5) | 29 (12–35,5) | 23 (14–36) | 26 (17,5–29,5) | 22,5 (16–32,5) | 23 (14,5–39) |
| Fırsat savaşı | 2 (0–4) | 1 (0–3,5) | 0,5 (0–3) | 2 (0–3,5) | 1 (0–2) | 1 (0–2,5) |
| Fırsat merkezi | 26,5 (24–29) | 27 (26–28,5) | 25 (24–28) | 26 (24–28) | 26,5 (25–28) | 27 (24–29) |
| Büyük şehir el değiştirmesi | 11 (4,5–15) | 9,5 (5,5–18) | 10 (3,5–14,5) | 10,5 (6,5–17,5) | 11,5 (7,5–16) | 10,5 (5–15) |

## Eski analizdeki sorunlar

Eski analiz: TS v0.23, 12 seed × 30 yıl ve 3 seed × 60 yıl (Proje: `analiz-5-ajan-oneriler.md`). "Sürüyor mu" kaba bir eşiktir: araştırma ağacı Faz 1b-3'te kaldırıldı; 30. yılda tam 5 kara yerleşimli medeniyet ≥ %50; kamp (30. yıl) < 0,75 × en yüksek yıl; altın (30. yıl) ≥ 10 × altın (1. yıl); boştaki iş gücü (30. yıl) ≥ %30; büyük olay (30. yıl) ≤ 0,6 × en yüksek yıl; 25. yıldan sonra doğanların ≥ %50'si Sv5+; hiç başkent kaybı yok.

| Bulgu | Eski analiz | Bu ölçüm | Sürüyor mu? |
|---|---|---|---|
| Araştırma ağacı erken bitiyor | ~19. yılda bitiyor; 30. yılda medeniyetlerin %98'i bitirmiş | ağaç ve çağlar kaldırıldı (Faz 1b-3); başlangıç medeniyetlerinin ilk kasabası medyan –. yılda (0/0), ilk şehri –. yılda (0/0); 30. yılda başkent kademesi ortalaması 2,55 | hayır |
| Medeniyetler 5 yerleşimde takılıyor | 98 medeniyetin 74'ü (%76) tam 5 yerleşimde | tam 5 kara yerleşimli medeniyet payı 30. yılda %3,6, 60. yılda %4,7 (denizaşırı koloniler dâhil 30. yılda tam 5: %1,8, 5+: %68); medeniyet başına 11 yerleşim (30. yıl) | hayır |
| Kamp sayısı düşüyor | 6,8'den 3,5'e iniyor | 8 (1. yıl) → en yüksek 9,5 (19. yıl) → 8 (30. yıl) → 9 (60. yıl); yıl sonu, yıllık dünya medyanı | hayır |
| Altın birikiyor | altın medyanı 78'den 6.503'e çıkıyor | 326 (1. yıl) → 369 (30. yıl) → 253 (60. yıl) | hayır |
| İş gücü boşta | iş gücünün %43'ü boşta | 30. yılda %19, 60. yılda %29 (işe yerleşemeyen `zanaatçı` / bütün iş gücü, askerler dâhil; yıl içi ortalama) | hayır |
| Büyük olaylar seyreliyor | yıllık büyük olay 59'dan 28'e düşüyor | en yüksek 300 (45. yıl) → 226 (30. yıl) → 260 (60. yıl), yıllık dünya medyanı | hayır |
| Doğuş seviyesi şişiyor | 24. yıldan sonra herkes Sv5 doğuyor; efsane mekaniği ölü | 25–60. yıllarda Sv5+ doğanların payı %0; on yıllık doğuş seviyesi ortalaması 1,62 · 1,62 · 1,63 · 1,67 · 1,63 · 1,65 | hayır |
| Başkent düşmüyor | başkent fethedilemiyor (agents.ts:673) | 16 dünyada 106 başkent kaybı, 68 yok olma; 1910 yerleşim fethi | hayır |

## On yıllık özet

Hücre: dünyalar arası medyan (p10–p90). Her dünyada on yılın yıllık değerlerinin ortalaması alınır: akış ölçülerinde yıllık ortalama, stok ölçülerinde yıl sonu değerlerinin ortalaması. Yüzdeler 0–1 paylardır.

| Ölçü | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| **Medeniyet** | | | | | | |
| Yaşayan medeniyet | 6,5 (5,6–8) | 7 (5,65–8,25) | 6,6 (5,65–8) | 6,1 (5,55–8,1) | 6,25 (6–7,95) | 6,05 (5,65–8,7) |
| Yeni medeniyet (yeniden doğan) | 0,1 (0–0,3) | 0,1 (0–0,2) | 0 (0–0,1) | 0,1 (0–0,2) | 0 (0–0,1) | 0,1 (0–0,15) |
| Yok olan medeniyet | 0,1 (0–0,25) | 0,1 (0–0,1) | 0 (0–0,1) | 0,1 (0–0,2) | 0 (0–0,15) | 0,05 (0–0,2) |
| Başkent kaybı (medeniyet yaşarken) | 0,2 (0,1–0,5) | 0,1 (0–0,2) | 0,05 (0–0,2) | 0,1 (0–0,3) | 0,05 (0–0,1) | 0 (0–0,2) |
| Çöküş (yok olma + başkent kaybı) | 0,35 (0,15–0,6) | 0,1 (0–0,3) | 0,1 (0–0,3) | 0,25 (0–0,4) | 0,1 (0–0,3) | 0,15 (0–0,2) |
| Yaşayan yerleşim | 79 (63,3–81,3) | 79,4 (62,9–81,1) | 79,5 (66,9–80,8) | 79,5 (66,6–81,3) | 79,4 (67,2–81,3) | 79,1 (68,4–81,5) |
| Medeniyet başına yerleşim | 11,4 (9,75–13,6) | 11,1 (9,17–13,5) | 11,6 (9,61–13,1) | 11,6 (9,84–14,1) | 11,7 (10–13,5) | 12,3 (9,21–13,7) |
| 5+ kara yerleşimli medeniyet payı | %77 (%56–%91) | %71 (%62–%87) | %70 (%58–%78) | %69 (%58–%78) | %70 (%61–%82) | %68 (%55–%82) |
| Kurulan yerleşim | 0,65 (0,35–1,55) | 0,55 (0,2–1,85) | 0,65 (0,2–1,85) | 0,5 (0,15–1,65) | 0,65 (0,1–1,4) | 0,65 (0,25–1,35) |
| Fethedilen yerleşim | 2,35 (1,65–3,25) | 2,15 (0,75–3,25) | 1,9 (1,15–2,35) | 2 (1,3–2,6) | 1,65 (1,15–2,5) | 1,75 (1,05–2,95) |
| Terk edilen yerleşim | 0,65 (0,15–2,35) | 0,45 (0,3–1,8) | 0,55 (0,15–2,15) | 0,4 (0,1–1,8) | 0,6 (0,15–1,1) | 1 (0,3–1,55) |
| Toplam nüfus | 2807 (2382–3273) | 3039 (2372–3346) | 3185 (2389–3407) | 3115 (2544–3555) | 3004 (2387–3430) | 2905 (2301–3447) |
| Altın medyanı (medeniyetler) | 282 (211–480) | 327 (153–575) | 351 (164–531) | 371 (124–718) | 289 (161–701) | 270 (181–608) |
| Boştaki iş gücü payı | %21 (%11–%34) | %22 (%12–%33) | %18 (%14–%36) | %21 (%13–%35) | %24 (%14–%37) | %28 (%11–%37) |
| Bölünme (ayrılıp kurulan medeniyet) | 0,1 (0–0,25) | 0,1 (0–0,15) | 0 (0–0,1) | 0,1 (0–0,15) | 0 (0–0,1) | 0,1 (0–0,15) |
| En büyük medeniyetin yerleşimi | 19,9 (18,2–26,2) | 20,8 (18,3–29,1) | 23,6 (18,7–29,9) | 23,4 (20,6–31,5) | 24,5 (19,7–32) | 26,1 (20,7–30,6) |
| **Olaylar** | | | | | | |
| Olay | 1194 (1062–1613) | 1294 (1112–1659) | 1308 (1088–1712) | 1456 (1215–1718) | 1562 (1202–1782) | 1604 (1149–1869) |
| Büyük olay | 243 (201–312) | 248 (189–326) | 237 (178–322) | 242 (199–304) | 259 (214–325) | 268 (180–344) |
| **Savaş** | | | | | | |
| Muharebe | 21,2 (16,6–24,8) | 20,5 (16,6–25) | 20,4 (16,6–23,5) | 19,9 (15,5–25,7) | 20 (15,3–25,8) | 21 (14,1–27,7) |
| Başlayan savaş | 2,65 (1,9–4,05) | 2,9 (1,2–3,55) | 2,3 (1,4–3,6) | 2,6 (1,75–2,95) | 2,25 (1,6–3,25) | 2,3 (1,45–3,9) |
| Süren savaş (yıl sonu) | 1,2 (0,95–2,35) | 1,4 (0,75–2,35) | 1,35 (0,55–2,1) | 1,25 (0,7–2,05) | 1,45 (0,85–2,05) | 1,2 (0,75–1,85) |
| Yıl içinde süren savaş | 3,9 (2,95–5,65) | 4,45 (1,9–5,6) | 3,9 (2,05–5,6) | 3,6 (2,85–5,3) | 3,55 (2,6–5,25) | 3,45 (2,55–5,6) |
| Yağma akını (medeniyet) | 1,75 (0–3,4) | 1,65 (0,4–2,85) | 1,3 (0–3,5) | 1,55 (0,1–3,05) | 1,45 (0–3) | 0,95 (0,1–3,75) |
| Tarihî hak savaşı | 0,3 (0,15–0,65) | 0,4 (0,1–0,65) | 0,3 (0,1–0,6) | 0,25 (0,05–0,65) | 0,2 (0,05–0,6) | 0,3 (0,1–0,4) |
| Pakt gereği savaş | 0,05 (0–0,45) | 0,3 (0–0,75) | 0 (0–0,65) | 0,1 (0–0,4) | 0,1 (0–0,45) | 0,15 (0–0,45) |
| Kutsal Sefer çağrısı | 0,1 (0–0,25) | 0,1 (0–0,2) | 0,1 (0–0,2) | 0,1 (0–0,2) | 0,1 (0–0,2) | 0,1 (0–0,2) |
| İhanet (pakt çiğnendi) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0,05 (0–0,1) | 0 | 0 (0–0,1) |
| Savunma paktı (yıl sonu) | 0,85 (0–1,25) | 0,8 (0–1,4) | 0,45 (0–1,2) | 0,45 (0–1,25) | 0,65 (0–1,05) | 0,75 (0–1,2) |
| **Canavarlar** | | | | | | |
| Yaşayan kamp (yıl sonu) | 8,3 (7,6–8,95) | 8,8 (8,15–9,35) | 8,3 (7,85–9,6) | 8,75 (8–9,2) | 8,6 (8,1–9,55) | 8,6 (8–9,75) |
| Yaşayan kamp (yıl ort.) | 8,27 (7,87–8,89) | 8,77 (7,88–9,35) | 8,47 (7,87–9,76) | 8,64 (8,14–9,4) | 8,45 (8,08–9,4) | 8,53 (8,02–9,81) |
| Doğan kamp | 8,45 (3,55–10,1) | 8,05 (3,5–10,4) | 6,55 (3,05–8,75) | 5,5 (3,15–9,9) | 6,3 (3–9,05) | 5,8 (1,9–10,1) |
| Temizlenen kamp | 8,3 (3,6–10,2) | 8,15 (3,5–10,3) | 6,55 (3,25–8,65) | 5,45 (3,1–9,95) | 6,25 (2,8–9) | 5,85 (1,9–9,95) |
| Canavar baskını | 3,75 (2,7–5,3) | 3,9 (2,95–5,2) | 3,9 (3,3–6,05) | 4,15 (2,85–5,6) | 4,05 (2,6–5,45) | 4,15 (2,8–6,15) |
| Yaşayan trol ini (yıl sonu) | 1,75 (1,4–2,55) | 2,1 (1,85–2,6) | 2,5 (2,1–2,8) | 2,7 (2,1–2,9) | 2,65 (2,1–3) | 2,7 (2,4–3) |
| Yaşayan ejderha (yıl sonu) | 0 | 0,4 (0,15–0,55) | 0 (0–1) | 0 (0–1) | 0 (0–1) | 0 (0–0,9) |
| Ejderha akını | 0 | 0,5 (0,25–0,7) | 0 (0–1,5) | 0 (0–1,55) | 0 (0–1,25) | 0 (0–1,1) |
| Kriz (anlatıcı) | 3,1 (2,3–3,75) | 2,8 (2,2–3,9) | 2,75 (2,1–3,7) | 2,8 (1,85–4,2) | 2,85 (2,1–3,5) | 2,8 (2,4–4) |
| Rahatlama dönemi (anlatıcı) | 0,15 (0,1–0,45) | 0,3 (0,15–0,4) | 0,2 (0–0,45) | 0,2 (0–0,55) | 0,25 (0,1–0,45) | 0,3 (0–0,55) |
| **Kahramanlar** | | | | | | |
| Doğan kahraman | 9,2 (6,5–14,6) | 10,4 (7,7–15,9) | 10,5 (7,9–15,7) | 10,9 (7,8–14,8) | 12,1 (7,4–15,3) | 10,6 (7,3–16,9) |
| Ölen kahraman | 4,5 (2,15–6,8) | 4,95 (2,85–6,55) | 5,1 (3,65–6,35) | 5,05 (2,6–7,05) | 4,75 (3–6,95) | 4,3 (3,05–7,05) |
| Emekli olan kahraman | 1,55 (0,85–2,4) | 1,7 (1,15–2,5) | 2,3 (1,75–3) | 2,1 (1,55–2,7) | 2,2 (1,7–3,1) | 2,65 (1,9–3,2) |
| Diyarı terk eden kahraman | 3,15 (1,55–9,55) | 3,2 (1,45–9,05) | 4,3 (1,3–8,6) | 5,6 (1,35–8,6) | 4,85 (1,9–10) | 6,15 (2,25–9,3) |
| Ölümden dönen kahraman | 0 | 0 | 0 | 0 | 0 | 0 |
| Efsane olan kahraman | 0 (0–0,1) | 0,1 (0–0,3) | 0 (0–0,1) | 0,05 (0–0,15) | 0 (0–0,1) | 0 (0–0,2) |
| Yaşayan kahraman (yıl sonu) | 133 (114–180) | 144 (122–182) | 151 (116–184) | 161 (132–183) | 169 (128–184) | 174 (127–190) |
| Doğuş seviyesi (ort.) | 1,64 (1,52–1,71) | 1,6 (1,54–1,75) | 1,64 (1,54–1,71) | 1,67 (1,59–1,75) | 1,65 (1,55–1,72) | 1,62 (1,54–1,74) |
| Ölüm seviyesi (ort.) | 4,3 (3,97–4,89) | 4,82 (4,05–5,7) | 4,93 (4,39–5,97) | 4,81 (4,2–5,45) | 5,09 (4,22–5,61) | 4,85 (4,17–5,74) |
| Yaşayan kahraman seviyesi (ort.) | 4,65 (4,32–5,11) | 4,67 (4,33–5,44) | 4,67 (4,27–5,44) | 4,66 (4,29–5,09) | 4,7 (4,33–5,13) | 4,64 (4,36–5,24) |
| En yüksek seviye (şimdiye dek) | 10 | 10 | 10 | 10 | 10 | 10 |
| **Han ve ticaret** | | | | | | |
| Ayakta han | 3,6 (3–5,65) | 3,95 (2,85–5,9) | 3,95 (3–6) | 4,05 (3–5,6) | 4,1 (3–6) | 4 (3–6,05) |
| Asılan ilan | 14,4 (10,7–16,2) | 15,9 (10–21,3) | 14 (10,1–21,8) | 13,8 (9,45–23,3) | 16 (10,8–24,1) | 15,6 (9,7–21,1) |
| Biten ilan | 7,5 (4,85–9,8) | 7,7 (4,05–9,75) | 6,15 (3,95–8,25) | 6,65 (3,65–8,8) | 6,75 (4,3–9,8) | 6,1 (4,3–10,2) |
| Ticaret seferi (kervan) | 96,4 (60,8–147) | 98,3 (57,2–159) | 110 (68,3–173) | 115 (62,8–175) | 115 (59,8–167) | 119 (65,3–182) |
| İkmal seferi | 34,1 (16,3–49,1) | 32,7 (21,5–50,1) | 39,4 (25,5–51,2) | 41,6 (19–55,4) | 42,1 (29,4–61,2) | 44,9 (33,9–64,5) |
| **Altın ve ambar** | | | | | | |
| Altın p90 (medeniyetler) | 1163 (807–1535) | 1111 (877–1370) | 1145 (921–1757) | 1299 (855–2106) | 1375 (991–2271) | 1299 (1043–2152) |
| Bakım gideri (altın; asker, kahraman, L2–L3) | 2801 (2103–3359) | 2915 (2078–3436) | 2928 (1989–3505) | 2626 (2049–3191) | 2561 (1856–3122) | 2353 (1827–3215) |
| Kamu işlerine (imar) harcanan altın | 1825 (1083–3138) | 1855 (1141–3214) | 2204 (1203–3565) | 2688 (1286–4385) | 3015 (1445–4307) | 2898 (1463–3856) |
| Ambarla beslenen amele tayını (gıda) | 991 (175–2862) | 823 (358–2488) | 720 (227–2638) | 610 (88,4–1898) | 412 (58,6–1013) | 266 (12,6–864) |
| Kamu işlerindeki (amele) iş gücü payı | %13 (%9,7–%15) | %12 (%10–%14) | %12 (%8,6–%15) | %13 (%8,3–%16) | %10 (%8,4–%15) | %9,7 (%6,9–%16) |
| İmar ortalaması (köy+, 0–100) | 18,9 (12,1–23,3) | 18,7 (13–24) | 17,5 (13–26,6) | 18,9 (11,6–27) | 18,4 (9,24–25,8) | 17,8 (8,56–24,9) |
| Canavar baskınında yitirilen altın | 11,3 (6,05–23,3) | 8,5 (4,55–15,3) | 11,2 (4,85–28,9) | 16,3 (4,85–28,8) | 14,8 (4,95–27,2) | 17,2 (7,95–29,6) |
| Ejderhaya giden altın (haraç + akın) | 0 | 60,5 (3,2–178) | 0 (0–711) | 0 (0–663) | 0 (0–528) | 0 (0–646) |
| Hazinesi boş medeniyet payı | %0 | %0 | %0 | %0 | %0 | %0 |
| Kent tüketiminde yokluk payı (köy+; ekmek, bira ya da alet) | %30 (%27–%40) | %33 (%25–%41) | %33 (%24–%40) | %37 (%25–%43) | %37 (%23–%46) | %34 (%21–%50) |
| Ekmek ya da bira yokluğu payı (köy+) | %7,3 (%1,7–%18) | %7,2 (%3,9–%17) | %7,9 (%4,5–%14) | %8,6 (%5,3–%26) | %14 (%4,4–%25) | %13 (%5,1–%33) |
| Kıtlık (büyük olay) | 0 (0–0,25) | 0 (0–0,3) | 0,1 (0–0,2) | 0,1 (0–0,35) | 0,1 (0–0,3) | 0,1 (0–0,3) |
| Açlıktan ölen | 0,1 (0–19,2) | 2,45 (0–10,1) | 1,8 (0–14) | 2,65 (0,05–10,6) | 2,2 (0–12,1) | 6,45 (0–10,6) |
| Kıtlık yardımı (sevkiyat) | 0 (0–0,7) | 0 (0–0,75) | 0,2 (0–0,35) | 0,15 (0–0,85) | 0,1 (0–0,45) | 0,15 (0–0,75) |
| Kıtlıkta yüz çeviren | 0 (0–0,4) | 0 (0–0,85) | 0 (0–0,6) | 0,35 (0–0,9) | 0,3 (0–0,65) | 0,3 (0–0,85) |
| Kıtlık akını | 0 (0–0,1) | 0 (0–0,45) | 0 (0–0,5) | 0,05 (0–0,5) | 0,05 (0–0,6) | 0 (0–0,3) |
| Ambarın yettiği gün (medeniyet medyanı) | 42 (18,2–50,1) | 32,9 (23,6–49,9) | 36,3 (24,4–49,1) | 32,3 (20,1–45,9) | 26,2 (15–41,1) | 22,3 (7,38–38,8) |
| **Yerleşim kademesi** | | | | | | |
| Ortalama yerleşim kademesi | 1,14 (0,91–1,3) | 1,16 (0,97–1,3) | 1,15 (0,93–1,33) | 1,09 (0,98–1,29) | 1,09 (0,89–1,34) | 1,04 (0,9–1,27) |
| Köy+ yerleşim | 62,8 (41,8–70,4) | 64,5 (45,3–71,9) | 64,9 (46–72,8) | 63,4 (47,8–70,2) | 63,8 (46,6–70,4) | 62,5 (47,5–69,7) |
| Kasaba+ yerleşim | 20,1 (11,3–28,1) | 20,2 (14,4–27,1) | 19,2 (13–29,2) | 19,9 (10,9–27,1) | 18,5 (10,1–31,1) | 13,9 (10–28,3) |
| Şehir | 4,8 (4,3–5,7) | 5,3 (4–5,85) | 5,5 (4,65–5,95) | 5,3 (4,4–6) | 5,4 (4,35–5,95) | 4,65 (4,2–5,6) |
| Ortalama başkent kademesi | 2,54 (2,33–2,64) | 2,54 (2,34–2,72) | 2,57 (2,38–2,7) | 2,6 (2,38–2,71) | 2,61 (2,38–2,78) | 2,59 (2,42–2,76) |
| Kademe değişimi (yerleşim, yıl içinde) | 30 (18,4–35) | 26,8 (20,9–34,4) | 25,9 (18,9–31,9) | 24,2 (20,8–30) | 22,5 (13,9–26,6) | 20,2 (12,9–28,4) |
| **Deniz** | | | | | | |
| Liman (tersane) | 18,7 (15,8–21,8) | 20,1 (17,3–23,3) | 21 (17,4–25,5) | 20,1 (17,8–23,5) | 20,4 (18,3–23,8) | 21,4 (18,8–23,8) |
| Gemi (koga/tekne) | 46,1 (37,7–56,5) | 50,1 (37,6–61,3) | 49,4 (40,2–68,6) | 49,9 (40,3–68,1) | 50,9 (37,7–70,4) | 51,1 (40,2–72) |
| Kadırga | 21,4 (14,1–26,8) | 18,5 (14,2–26,7) | 18,9 (13,3–23,1) | 18,4 (11–24,4) | 18,2 (10,5–23,5) | 18,2 (11,4–21,6) |
| Denizaşırı yerleşim | 6,9 (4,2–9,35) | 6,7 (4–9,1) | 6 (3,65–9,85) | 6,1 (3,45–9,5) | 6,5 (3,2–9,55) | 6 (3–9,5) |
| Deniz ticaret yolu (yıl sonu) | 13,2 (8,3–21,9) | 14,8 (7,4–27,7) | 14,1 (9,8–29,9) | 15 (9,5–26) | 16,8 (8,05–27,1) | 18,8 (7,3–27,2) |
| Deniz seferi (ticaret) | 23,3 (14,2–39) | 27,2 (11,6–58,8) | 25,6 (15,8–70,6) | 26,8 (12,7–59,9) | 28,9 (11,4–65,2) | 30,2 (11,9–57,8) |
| **v3: durum değişimi** | | | | | | |
| Yaşayan yerleşim (yıl ort.) | 77,8 (62,3–79,9) | 78,3 (61,7–79,9) | 78,2 (65,8–79,5) | 78,2 (65,5–80) | 78,2 (65,9–80) | 78 (67–80,1) |
| Büyük şehir (Şehir kademesi, yıl ort.) | 4,93 (4,33–5,66) | 5,18 (4,24–5,89) | 5,44 (4,65–5,94) | 5,44 (4,44–5,97) | 5,43 (4,5–5,94) | 4,65 (4,2–5,6) |
| El değiştiren yerleşim (fetih + bölünme) | 3,6 (2,35–4,7) | 3,15 (1,95–4,35) | 2,85 (1,9–3,65) | 3 (2,3–3,95) | 2,9 (2,15–3,8) | 3,15 (1,9–4,45) |
| El değiştiren büyük şehir | 1,1 (0,45–1,5) | 0,95 (0,55–1,8) | 1 (0,35–1,45) | 1,05 (0,65–1,75) | 1,15 (0,75–1,6) | 1,05 (0,5–1,5) |
| Büyük şehre hücum (kuşatma muharebesi) | 0,5 (0,3–1) | 0,5 (0,25–0,9) | 0,2 (0,1–0,65) | 0,45 (0,05–0,9) | 0,6 (0,2–1,1) | 0,35 (0,2–0,75) |
| Büyük şehir yağmalandı, tutulmadı | 0,1 (0–0,4) | 0,1 (0,05–0,45) | 0,1 (0–0,4) | 0,1 (0–0,4) | 0,1 (0–0,4) | 0,1 (0–0,45) |
| Yerleşim durum değişimi (kuruluş hariç hepsi) | 50,6 (43,8–59,5) | 49,2 (36,9–62,5) | 45,6 (35,8–53,5) | 44,5 (39,1–58,8) | 46,3 (33–55,4) | 45,2 (28,6–74,5) |
| Orta halka kaydı (Köy/Kasaba: kademe değişimi ya da terk) | 21,9 (12,4–26,9) | 20,3 (14–27,9) | 20,4 (12,2–25) | 18,9 (14,9–24,4) | 17,1 (9,65–21,8) | 14,8 (8,4–23,7) |
| Açlık başlayan yerleşim | 0,55 (0–14,9) | 0,55 (0–16,1) | 1,2 (0–9,25) | 2,95 (0,65–12,2) | 7,65 (0,25–12,8) | 4,55 (0,2–31,1) |
| Salgın başlayan yerleşim | 0,45 (0,25–1,05) | 0,55 (0,2–1,05) | 0,7 (0,25–1,1) | 0,6 (0,3–1,1) | 0,5 (0,15–0,95) | 0,5 (0,2–1,1) |
| Yakılan/yanan yerleşim | 9,65 (5,65–11,4) | 9,5 (5,65–11,9) | 9,05 (4,85–12,7) | 8,85 (5,75–13,3) | 9,1 (3,85–14) | 8 (4,85–16) |
| Harabeye yeniden yerleşim | 0,45 (0,05–0,9) | 0,25 (0,1–1,25) | 0,3 (0,15–1,45) | 0,3 (0,1–1,25) | 0,55 (0,1–1) | 0,5 (0,2–1) |
| **Devlet, inanç ve örgüt** | | | | | | |
| Yaşayan örgüt (yıl sonu) | 13 (12,4–13) | 13 (12,1–13) | 13 (12–13) | 12,6 (12,2–13) | 12,7 (12,1–13) | 12,6 (12,1–13) |
| Örgüt şubesi (yıl sonu) | 297 (197–334) | 292 (232–336) | 292 (250–372) | 318 (248–379) | 333 (246–378) | 329 (248–378) |
| Gizli şube (yıl sonu) | 10,6 (8,05–18,3) | 7,85 (5,8–10,2) | 7,95 (5,05–11,8) | 6,9 (5,05–11,5) | 6,5 (4,8–10,7) | 5,75 (3,85–10,1) |
| Örgüt üyesi (yıl sonu) | 1286 (1095–1555) | 1380 (1140–1653) | 1478 (1195–1721) | 1550 (1238–1853) | 1546 (1271–1848) | 1543 (1279–1874) |
| Açılan şube | 16,7 (15,3–18,9) | 16,1 (13,8–17,9) | 16,7 (13–18,8) | 16 (13,4–18,8) | 15,2 (13,2–18,2) | 15,4 (11,4–17) |
| Kapanan şube | 19,1 (14,5–21,9) | 14,5 (12,2–17,4) | 14,4 (11,1–16,9) | 16 (12,4–18,4) | 15,4 (10,7–17,9) | 15,1 (11,3–17,4) |
| Dağılan örgüt | 0 (0–0,1) | 0 (0–0,2) | 0 (0–0,1) | 0,1 (0–0,2) | 0,1 (0–0,1) | 0 (0–0,2) |
| Yeniden kurulan örgüt | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,15) | 0,05 (0–0,2) | 0,05 (0–0,1) | 0,1 (0–0,1) |
| Gölge savaşı eylemi | 7,1 (6–8,85) | 7,4 (4,65–8,7) | 7,55 (3,75–9,3) | 5,9 (4,15–8,25) | 5,45 (3,5–8,55) | 6,45 (4,4–8,25) |
| Gölge savaşında öldürülen usta ya da lider | 1,6 (1,1–2,05) | 1,55 (0,9–2,4) | 1,45 (0,65–2) | 1,3 (0,75–1,85) | 1,25 (0,75–2) | 1,45 (1–1,8) |
| Gizli şubeye baskın | 0,85 (0,3–1,25) | 0,6 (0,35–0,95) | 0,5 (0,25–0,85) | 0,55 (0,2–1,05) | 0,4 (0,15–0,8) | 0,4 (0,25–0,8) |
| Lobiyle yasa değişikliği | 0,85 (0,45–1,95) | 1 (0,7–1,7) | 1 (0,4–2,1) | 1,15 (0,25–2) | 1,3 (0,25–2,35) | 1,7 (0,7–2,75) |
| Darbe girişimi | 0 (0–0,05) | 0,1 (0–0,2) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) |
| Örgüt ilanı (Avcılar) | 0,9 (0,45–1,3) | 0,65 (0,4–1,25) | 0,65 (0,45–1) | 0,8 (0,45–1,3) | 0,7 (0,55–1,4) | 0,6 (0,4–1,2) |
| Örgüt üyesi kahraman payı (yıl sonu) | %100 (%99–%100) | %100 (%99–%100) | %100 (%99–%100) | %100 (%99–%100) | %100 (%99–%100) | %99 (%99–%100) |
| Yönetici değişimi | 1,2 (0,65–1,75) | 1,4 (0,6–1,85) | 1,05 (0,6–1,65) | 1,05 (0,7–1,9) | 1,4 (1–1,75) | 1,4 (0,65–2,05) |
| Veraset krizi | 0 (0–0,05) | 0 | 0 | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,05) |
| Meşruiyet ortalaması (yıl sonu) | 61,6 (59,4–68,8) | 63 (61–68) | 63,9 (60,7–69,9) | 64 (59,7–68,9) | 63,8 (59,7–68,7) | 62,4 (56,8–68,6) |
| Pakt'a bağlı yönetici (yıl sonu) | 0,2 (0–0,5) | 0,1 (0–0,4) | 0 (0–0,45) | 0 (0–0,3) | 0,1 (0–0,4) | 0,05 (0–0,25) |
| Köle (yıl sonu) | 6,05 (1,6–35,4) | 9,4 (2,45–42,6) | 15,4 (1,2–45,5) | 21 (1,55–44,6) | 19,9 (1,05–36,6) | 20 (5,3–27,6) |
| Köle payı (nüfusun, yıl sonu) | %0,3 (%0,1–%1,3) | %0,3 (%0,1–%1,5) | %0,6 (%0–%1,6) | %0,7 (%0,1–%1,4) | %0,7 (%0–%1,2) | %0,7 (%0,2–%1) |
| Hapis madeninde mahkûm (yıl sonu) | 0,85 (0–6,9) | 1,15 (0,15–15,1) | 0,7 (0,05–10,5) | 1,2 (0–3,95) | 0,8 (0–3,45) | 0,7 (0–4,45) |
| Esarete düşen | 5,15 (1,7–14,6) | 5,8 (2,8–11,7) | 7 (1,65–14,8) | 6,25 (1,5–15,1) | 6,15 (0,8–12,5) | 8,35 (1,65–11,8) |
| Kurtulan köle (Özgürlük Ağı, kaçış, azat) | 5,9 (2,1–14,3) | 5,35 (2,4–11,6) | 8,35 (1,7–13,9) | 7,55 (1,25–14,5) | 5,2 (0,9–13,1) | 7,35 (1,45–10,9) |
| Özgürlük Ağı'nın kurtardığı köle | 5,15 (1,7–10,8) | 4 (1,55–8,45) | 5,55 (1,05–9) | 5 (0,8–9,75) | 4,35 (0,8–9,25) | 5,6 (1,25–7,6) |
| Esir kahraman (yıl sonu) | 0,5 (0–1,65) | 0,4 (0–2,05) | 0,35 (0–2) | 0,5 (0,05–1,65) | 0,4 (0,1–1,45) | 0,3 (0,1–1,05) |
| Aç haydut kampı (yıl sonu) | 0,55 (0,05–1,3) | 0,5 (0,1–1,25) | 0,7 (0,2–1,4) | 0,6 (0,25–1,4) | 0,7 (0,15–1,65) | 0,75 (0,2–1,7) |
| Haydut olan aç halk | 8,55 (2,05–19,3) | 7,2 (3,55–19,4) | 9,9 (5,15–16) | 9,6 (4,2–24,7) | 17,3 (3,95–22,9) | 16 (6,65–32,3) |
| Aç ya da ekmeksiz yerleşim payı (köy+) | %6,4 (%1,3–%14) | %6,1 (%2,6–%12) | %6,2 (%3,8–%12) | %5,8 (%3,3–%21) | %12 (%3,4–%18) | %10 (%3,2–%25) |
| Devriye durdurması | 48,9 (37,8–72,7) | 51,6 (34,5–70,9) | 49,1 (31,9–70,7) | 60,5 (30,3–69,9) | 63,9 (30,8–75,9) | 64,3 (31,2–74,2) |

## Kahraman seviyeleri

### Doğuş seviyesi (bütün dünyalar, on yıl içinde doğanlar)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 |
|---|---|---|---|---|---|
| 1–10 | 1663 | 1,62 | %47 | %44 | %8,8 |
| 11–20 | 1790 | 1,62 | %46 | %45 | %8,7 |
| 21–30 | 1826 | 1,63 | %47 | %44 | %9,4 |
| 31–40 | 1750 | 1,67 | %45 | %44 | %11 |
| 41–50 | 1867 | 1,63 | %45 | %46 | %8,8 |
| 51–60 | 1864 | 1,65 | %46 | %43 | %11 |

### Ölüm seviyesi (bütün dünyalar, on yıl içinde ölenler)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 | Sv4 | Sv5 | Sv6 | Sv7 | Sv8 | Sv9 | Sv10 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 1–10 | 683 | 4,25 | %7,2 | %12 | %17 | %24 | %17 | %8,8 | %5,4 | %3,7 | %3,1 | %1,3 |
| 11–20 | 808 | 4,75 | %7,1 | %9 | %14 | %20 | %19 | %11 | %7,7 | %6,3 | %2,7 | %4,3 |
| 21–30 | 839 | 4,8 | %6 | %9,2 | %19 | %18 | %17 | %8,5 | %6,6 | %5,7 | %4,6 | %6 |
| 31–40 | 782 | 4,75 | %5 | %10 | %16 | %18 | %20 | %10 | %6,3 | %5,1 | %3,6 | %4,9 |
| 41–50 | 755 | 4,84 | %5,8 | %10 | %13 | %17 | %22 | %9,7 | %8,1 | %5,2 | %3,3 | %5,4 |
| 51–60 | 784 | 4,95 | %5,1 | %7 | %16 | %17 | %21 | %11 | %7 | %8 | %3,7 | %4,2 |

### Yaşayan kahramanların seviyesi (bütün dünyalar, on yılın son yılının sonunda)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 | Sv4 | Sv5 | Sv6 | Sv7 | Sv8 | Sv9 | Sv10 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 10 | 2280 | 4,84 | %2,8 | %7 | %17 | %27 | %19 | %8,9 | %5,4 | %5,1 | %4 | %4,9 |
| 20 | 2401 | 4,83 | %2,7 | %8,5 | %18 | %23 | %19 | %8,2 | %6 | %4,7 | %3,6 | %5,9 |
| 30 | 2436 | 4,71 | %2,6 | %8,4 | %17 | %27 | %19 | %7,7 | %6,5 | %3,3 | %3,1 | %5,1 |
| 40 | 2559 | 4,73 | %2,1 | %7,9 | %18 | %26 | %20 | %9,5 | %4,5 | %4,1 | %2,6 | %5,2 |
| 50 | 2595 | 4,79 | %2,4 | %7,6 | %16 | %26 | %22 | %9,4 | %5,5 | %4 | %2,4 | %5,4 |
| 60 | 2636 | 4,72 | %3,3 | %7,6 | %14 | %26 | %25 | %9,3 | %4,3 | %3,5 | %2,3 | %5 |

### Ölüm nedenleri (bütün dünyalar)

| Neden | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 | Toplam |
|---|---|---|---|---|---|---|---|
| Trol | 260 | 262 | 217 | 152 | 186 | 185 | 1262 (%27) |
| Kamp saldırısı | 207 | 204 | 210 | 227 | 200 | 175 | 1223 (%26) |
| Bilinmiyor | 38 | 107 | 163 | 175 | 168 | 219 | 870 (%19) |
| Kuşatma | 94 | 77 | 103 | 82 | 94 | 89 | 539 (%12) |
| Düello | 38 | 45 | 38 | 62 | 36 | 51 | 270 (%5,8) |
| Ejderha | 0 | 77 | 82 | 43 | 38 | 25 | 265 (%5,7) |
| Kervan soygunu | 27 | 24 | 18 | 34 | 22 | 29 | 154 (%3,3) |
| Han baskını (medeniyet) | 10 | 3 | 5 | 1 | 4 | 4 | 27 (%0,6) |
| Yağma akını | 8 | 1 | 3 | 1 | 3 | 5 | 21 (%0,5) |
| Han baskını (canavar) | 0 | 8 | 0 | 3 | 3 | 0 | 14 (%0,3) |
| Yol pususu | 1 | 0 | 0 | 2 | 1 | 2 | 6 (%0,1) |

Neden, ölümün kaydedildiği andaki son muharebenin türünden (başlık ve taraflar) ya da suikast olayından çıkarılır.

## Olay türleri

Dünya başına yıllık olay sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Kahraman (`hero`) | 733 (591–1044) | 788 (638–1038) | 855 (696–1108) | 972 (808–1115) | 1046 (811–1167) | 1112 (791–1279) |
| Sefer/ilan (`quest`) | 145 (106–218) | 142 (83,6–245) | 135 (89,6–216) | 129 (87,3–219) | 167 (102–224) | 135 (87,7–254) |
| state | 94,1 (71,6–110) | 100 (75,2–108) | 98,9 (78,4–107) | 100 (73,2–108) | 97,2 (76,5–105) | 95,8 (76,3–108) |
| org | 60 (54,5–64,6) | 54,5 (47,3–60,1) | 52,3 (43,6–61) | 54 (45,3–62,1) | 52,1 (42,4–61) | 52,7 (44–57,9) |
| Göç (`migration`) | 37,9 (28,8–51) | 41,5 (29,7–49,3) | 39,2 (30,6–52,5) | 39,9 (23,4–55,7) | 31,6 (23,3–50,9) | 28,8 (20,6–49,3) |
| Kamp (`lair`) | 30,8 (20,6–39,1) | 29,2 (17,8–38,5) | 28,4 (16,5–34,8) | 23,5 (18,1–38) | 27,4 (19,8–38,2) | 25,7 (17,4–40,5) |
| İnşaat (`build`) | 23,8 (17,1–35,5) | 21,7 (14,9–30,7) | 19,9 (14,6–34,7) | 16,7 (13,2–23,1) | 13,8 (9,4–22,9) | 14,1 (6,95–20,8) |
| Savaş (`war`) | 19,8 (11,6–33,3) | 18,8 (10,7–28,5) | 15,7 (7,95–27,2) | 15,7 (9,8–26,3) | 15,4 (9,4–24,9) | 14 (8,55–32,9) |
| Han (`inn`) | 11,2 (8,3–18,3) | 12,6 (9,25–19,4) | 13,3 (10,6–19,8) | 13,8 (10,1–18,8) | 15,1 (9,6–20,8) | 14,1 (10,8–20,8) |
| hub | 13 (11,5–14,4) | 13 (12–14,2) | 12,3 (11,4–13,7) | 12,8 (11,4–13,6) | 12,6 (11,4–13,6) | 12,6 (11,1–13,5) |
| Ölüm/terk (`death`) | 7,85 (5,9–9,95) | 8,25 (6,6–9,9) | 8,55 (6,4–9,7) | 8,4 (5,8–10,4) | 7,85 (5,6–10,2) | 7,55 (6,5–10,5) |
| politics | 7,65 (5,55–8,9) | 6,7 (5,65–9,75) | 7,1 (4,65–9,25) | 6,75 (5,1–9,8) | 7,15 (5,65–8,25) | 7,8 (5,5–10) |
| Ekonomi (`economy`) | 7,55 (5,6–9,8) | 7 (5,25–8,9) | 7,3 (5,15–9) | 6,85 (4,9–8,65) | 6,35 (4,25–7,6) | 5,55 (3,75–8,35) |
| Deniz (`sea`) | 7,15 (5,25–12,6) | 6,45 (4,4–14,9) | 6,55 (4,1–14,1) | 7 (4,6–10,6) | 6,35 (4,65–10,8) | 5,55 (3,35–14,4) |
| epitaph | 4,5 (2,15–6,8) | 4,95 (2,85–6,55) | 5,1 (3,65–6,35) | 5,05 (2,6–7,05) | 4,75 (3–6,95) | 4,3 (3,05–7,05) |
| Baskın (`raid`) | 4 (2,95–5,4) | 4,15 (2,55–4,9) | 4,1 (2,45–5,5) | 4,35 (3,05–5,45) | 4 (2,75–4,9) | 4,2 (2,95–6,55) |
| Büyüme (`growth`) | 4,6 (3,35–5,2) | 3,75 (3,3–5,35) | 3,7 (3,25–4,4) | 3,4 (2,65–4,35) | 3,3 (2,15–4,55) | 3,05 (1,95–4) |
| Dünya (`world`) | 1,95 (1,65–3,25) | 2,2 (1,45–3,3) | 2,2 (1,7–3,5) | 2,3 (1,4–3,35) | 2,25 (1,4–2,85) | 2,1 (1,6–3,4) |
| Diplomasi (`diplomacy`) | 2,4 (1,15–6,6) | 1,8 (0,65–5) | 2,35 (0,75–3,9) | 2,3 (0,9–3,9) | 1,95 (1,05–5) | 1,9 (0,55–5,3) |
| Yerleşim (`settle`) | 1,4 (0,9–3,55) | 1,4 (0,45–4,05) | 1,65 (0,4–4,1) | 1 (0,4–3,75) | 1,85 (0,2–3,2) | 1,4 (0,5–3,6) |
| Gerginlik (`tension`) | 2,1 (1,35–4,85) | 1,2 (0,4–4,1) | 1,3 (0,55–2,9) | 1,25 (0,45–2,4) | 1,25 (0,45–3,8) | 1,15 (0,1–4) |
| Sınıf (`class`) | 1,2 (0,05–2,35) | 1,15 (0,2–2) | 1,15 (0,1–2) | 1,15 (0,05–2,45) | 1,4 (0–2,55) | 1,05 (0,35–2,05) |
| Ticaret (`trade`) | 1,8 (0,9–3,85) | 1,3 (0,6–2,65) | 1,1 (0,5–2,7) | 1 (0,3–3,05) | 0,7 (0,15–2,9) | 0,75 (0,1–3,05) |
| Keşif (`discover`) | 1,3 (0,4–2,15) | 1,05 (0,5–2,1) | 0,95 (0,35–1,55) | 0,8 (0,45–1,1) | 0,45 (0,2–0,85) | 0,55 (0,05–1,3) |
| Rahatlama (anlatıcı) (`relief`) | 0,2 (0,1–0,4) | 0,3 (0,1–0,45) | 0,2 (0–0,45) | 0,2 (0–0,55) | 0,25 (0,1–0,45) | 0,3 (0–0,6) |
| Ejderha (`dragon`) | 0 | 1,25 (0,75–1,65) | 0 (0–2,85) | 0 (0–2,9) | 0 (0–2,6) | 0 (0–2,4) |
| Kriz (anlatıcı) (`crisis`) | 0 (0–0,2) | 0 (0–0,15) | 0 (0–0,25) | 0,05 (0–0,2) | 0,1 (0–0,25) | 0,15 (0–0,35) |
| Temas (`contact`) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,05) | 0 | 0 | 0 |

## Büyük olay türleri

Dünya başına yıllık büyük olay sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Sefer/ilan (`quest`) | 95,3 (71,1–142) | 93,7 (55,9–166) | 91 (59,4–147) | 86,4 (57,8–143) | 111 (67,8–150) | 92,3 (57–164) |
| Kahraman (`hero`) | 66,1 (52,1–86,4) | 70,4 (55,8–94,1) | 70,2 (59,8–95,7) | 79,8 (67,4–99) | 84 (70–102) | 91,9 (66,5–102) |
| Kamp (`lair`) | 21,6 (13,1–27,5) | 20,8 (12,8–26,8) | 19,6 (11,3–23,7) | 16,2 (11,9–26,1) | 18,7 (12,4–25,7) | 17,3 (10,3–27,8) |
| Savaş (`war`) | 13,9 (7,95–20,1) | 11,6 (6,8–17,7) | 10,2 (5,4–17,6) | 10,5 (7–16,5) | 10,5 (6,1–15,3) | 9,55 (6,1–21,8) |
| Ölüm/terk (`death`) | 7,85 (5,9–9,95) | 8,25 (6,6–9,9) | 8,55 (6,4–9,7) | 8,4 (5,8–10,4) | 7,85 (5,6–10,2) | 7,55 (6,5–10,5) |
| epitaph | 4,5 (2,15–6,8) | 4,95 (2,85–6,55) | 5,1 (3,65–6,35) | 5,05 (2,6–7,05) | 4,75 (3–6,95) | 4,3 (3,05–7,05) |
| Deniz (`sea`) | 4,45 (2,65–8) | 4,8 (2,4–9,85) | 4,05 (2,4–9,25) | 4,85 (3,2–7,45) | 4,45 (3,4–8,1) | 3,95 (2,75–9,25) |
| politics | 4,95 (2,95–5,6) | 4,3 (3,2–6,1) | 4 (2,6–5,7) | 3,8 (2,65–6,35) | 4,3 (3,2–5,35) | 4,15 (3–6,2) |
| Baskın (`raid`) | 3,4 (2,15–4,8) | 3,75 (2,2–4,5) | 3,5 (2,05–4,75) | 3,7 (2,6–5) | 3,7 (2,45–4,3) | 3,75 (2,7–5,95) |
| org | 4,25 (3,3–6,25) | 3,6 (2,5–4,3) | 2,9 (2,25–3,5) | 3,65 (2,7–4,5) | 3,15 (2,05–4) | 2,85 (1,6–4,2) |
| Han (`inn`) | 2,5 (1,6–3,65) | 2,95 (1,8–3,55) | 3,5 (2,45–4,05) | 2,85 (2–4,25) | 2,8 (2,25–4,15) | 3,65 (2,75–4,4) |
| hub | 2,6 (2,3–2,85) | 2,6 (2,4–2,8) | 2,5 (2,2–2,75) | 2,55 (2,25–2,7) | 2,5 (2,15–2,75) | 2,45 (2,1–2,7) |
| Dünya (`world`) | 1,5 (1,3–2,3) | 1,75 (1,2–2,6) | 1,8 (1,35–2,7) | 1,9 (1,15–2,8) | 1,85 (1,2–2,3) | 1,7 (1,45–2,65) |
| İnşaat (`build`) | 2,55 (1,55–3,5) | 1,5 (1,05–2,3) | 1,8 (0,8–2,3) | 1,55 (0,7–2,05) | 1,4 (0,6–2,35) | 1,3 (0,4–2,05) |
| Gerginlik (`tension`) | 2,1 (1,35–4,85) | 1,2 (0,4–4,1) | 1,3 (0,55–2,9) | 1,25 (0,45–2,4) | 1,25 (0,45–3,8) | 1,15 (0,1–4) |
| Diplomasi (`diplomacy`) | 1,1 (0,35–3,1) | 1,25 (0,25–2,15) | 1 (0,3–2,2) | 0,95 (0,3–2,2) | 1 (0,4–2,15) | 1 (0,4–2,9) |
| Yerleşim (`settle`) | 0,65 (0,35–1,55) | 0,55 (0,2–1,85) | 0,65 (0,2–1,85) | 0,5 (0,15–1,65) | 0,65 (0,1–1,4) | 0,65 (0,25–1,35) |
| Ticaret (`trade`) | 0,9 (0,5–1,7) | 0,6 (0,3–1,3) | 0,65 (0,2–1,2) | 0,5 (0,15–1,35) | 0,3 (0,05–1,15) | 0,35 (0–1,55) |
| Keşif (`discover`) | 0,7 (0,25–1,2) | 0,6 (0,3–0,85) | 0,4 (0–0,95) | 0,4 (0–0,6) | 0,2 (0–0,5) | 0,35 (0,05–0,8) |
| Ekonomi (`economy`) | 0,5 (0,2–0,7) | 0,3 (0,15–0,7) | 0,25 (0,1–0,6) | 0,3 (0,05–0,7) | 0,3 (0–0,6) | 0,25 (0,1–0,7) |
| Göç (`migration`) | 0,35 (0,05–0,55) | 0,3 (0,1–0,55) | 0,3 (0,1–0,65) | 0,3 (0,1–0,8) | 0,25 (0,05–0,4) | 0,3 (0,2–0,75) |
| Rahatlama (anlatıcı) (`relief`) | 0,2 (0,1–0,4) | 0,3 (0,1–0,45) | 0,2 (0–0,45) | 0,2 (0–0,55) | 0,25 (0,1–0,45) | 0,3 (0–0,6) |
| Ejderha (`dragon`) | 0 | 1,25 (0,75–1,55) | 0 (0–2,85) | 0 (0–2,9) | 0 (0–2,6) | 0 (0–2,4) |
| Büyüme (`growth`) | 0,2 (0–0,7) | 0,2 (0–0,45) | 0,1 (0–0,2) | 0,1 (0–0,2) | 0 (0–0,25) | 0,05 (0–0,25) |
| Kriz (anlatıcı) (`crisis`) | 0 (0–0,2) | 0 (0–0,15) | 0 (0–0,25) | 0,05 (0–0,2) | 0,1 (0–0,25) | 0,15 (0–0,35) |
| Sınıf (`class`) | 0 (0–0,05) | 0 | 0 (0–0,05) | 0 (0–0,05) | 0 | 0 |
| Temas (`contact`) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,05) | 0 | 0 | 0 |

## Muharebe türleri

Dünya başına yıllık muharebe sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Kamp saldırısı (`camp`) | 9,45 (7,25–11,4) | 8,95 (6,35–12,2) | 9,2 (6,25–11) | 9,25 (6,75–12,2) | 9,1 (8–11,9) | 9,3 (6,9–13,1) |
| Trol (`troll`) | 4,45 (1,9–6,5) | 4,8 (2,3–6,3) | 4,3 (1,9–6,5) | 3,8 (1,3–6) | 4,6 (1,7–5,85) | 3,6 (1,7–7,75) |
| Kuşatma (`siege`) | 2,8 (2,4–3,9) | 2,75 (1,65–3,4) | 2,45 (1,6–3,55) | 2,25 (1,9–3,2) | 2,4 (1,45–3,5) | 2,25 (1,65–3,95) |
| Yağma akını (`plunder`) | 1,75 (0–3,4) | 1,65 (0,4–2,85) | 1,3 (0–3,5) | 1,55 (0,1–3,05) | 1,45 (0–3) | 0,95 (0,1–3,75) |
| Yapı baskını (canavar) (`extRaid`) | 0,9 (0,4–2,3) | 0,85 (0,5–1,9) | 0,85 (0,35–2) | 0,9 (0,65–1,65) | 0,65 (0,35–1,55) | 0,95 (0,3–1,65) |
| Yerleşim baskını (canavar) (`raid`) | 0,6 (0,3–1) | 0,3 (0,2–0,8) | 0,55 (0,2–0,95) | 0,55 (0,25–0,85) | 0,4 (0,1–0,8) | 0,35 (0,2–1,1) |
| Kervan soygunu (`robbery`) | 0,3 (0,1–0,75) | 0,35 (0,2–0,65) | 0,3 (0,1–0,5) | 0,6 (0,2–0,9) | 0,35 (0,15–0,65) | 0,4 (0,3–0,95) |
| Düello (`duel`) | 0,15 (0,05–0,5) | 0,3 (0,1–0,5) | 0,2 (0,05–0,45) | 0,4 (0,15–0,75) | 0,2 (0,05–0,45) | 0,25 (0,05–0,5) |
| Ejderha (`dragon`) | 0 | 0,55 (0,35–0,75) | 0 (0–1,4) | 0 (0–1,55) | 0 (0–1,2) | 0 (0–1,15) |
| Deniz savaşı (`naval`) | 0,1 (0–0,35) | 0,1 (0–0,25) | 0,1 (0–0,3) | 0,1 (0–0,15) | 0 (0–0,1) | 0 (0–0,2) |
| Yol pususu (`ambush`) | 0 | 0 | 0 | 0 (0–0,1) | 0 (0–0,05) | 0 (0–0,1) |
| Han baskını (medeniyet) (`innCiv`) | 0 (0–0,1) | 0 (0–0,05) | 0 (0–0,1) | 0 | 0 (0–0,05) | 0 (0–0,1) |
| Han baskını (canavar) (`innMonster`) | 0 (0–0,1) | 0 (0–0,25) | 0 (0–0,15) | 0 (0–0,15) | 0 (0–0,1) | 0 (0–0,15) |
| Korsan savaşı (`pirate`) | 0 (0–0,15) | 0 (0–0,15) | 0 (0–0,2) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,2) |

## Kamp türleri

Dünya başına yaşayan kamp (yıl sonu değerlerinin on yıllık ortalaması): dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Hobgoblin (`hobgoblin`) | 3,2 (2,4–3,85) | 3,2 (2,5–4,05) | 3,15 (2,3–3,9) | 3,1 (2,4–3,6) | 2,8 (2,25–3,5) | 2,95 (2,4–3,3) |
| Trol (`troll`) | 1,75 (1,4–2,55) | 2,1 (1,85–2,6) | 2,5 (2,1–2,8) | 2,7 (2,1–2,9) | 2,65 (2,1–3) | 2,7 (2,4–3) |
| Goblin (`goblin`) | 2,1 (1,15–2,35) | 1,9 (1,2–2,3) | 1,5 (0,95–2,55) | 1,5 (0,95–2,1) | 1,8 (1,2–2,3) | 1,55 (0,55–1,8) |
| Bugbear (`bugbear`) | 0,95 (0,5–1,3) | 0,7 (0,35–1,05) | 0,8 (0,4–0,95) | 0,95 (0,5–1,45) | 1,05 (0,4–1,55) | 0,95 (0,5–1,9) |
| Aç haydut (`bandit`) | 0,55 (0,05–1,3) | 0,5 (0,1–1,25) | 0,7 (0,2–1,4) | 0,6 (0,25–1,4) | 0,7 (0,15–1,65) | 0,75 (0,2–1,7) |
| Korsan (`pirate`) | 0,3 (0,05–0,65) | 0,55 (0,1–0,8) | 0,25 (0,1–0,45) | 0,35 (0–0,65) | 0,2 (0,05–0,6) | 0,4 (0–0,9) |
| Ejderha (`dragon`) | 0 | 0,4 (0,15–0,55) | 0 (0–1) | 0 (0–1) | 0 (0–1) | 0 (0–0,9) |

## Kademe dağılımı

Yaşayan yerleşimlerin kademelere dağılımı, bütün dünyalar (yıl sonu; parantezde sayı).

| Yıl | 0 Kamp | 1 Köy | 2 Kasaba | 3 Şehir |
|---|---|---|---|---|
| 10 | %20 (238) | %53 (638) | %21 (255) | %6,1 (74) |
| 20 | %21 (251) | %53 (644) | %20 (248) | %6,5 (79) |
| 30 | %19 (239) | %53 (653) | %20 (245) | %7,3 (89) |
| 40 | %21 (256) | %56 (688) | %16 (200) | %6,8 (84) |
| 50 | %22 (280) | %54 (668) | %17 (217) | %6,5 (81) |
| 60 | %20 (240) | %58 (703) | %16 (192) | %6,8 (83) |

Başkentlerin kademelere dağılımı, bütün dünyalar (yıl sonu; parantezde sayı).

| Yıl | 0 Kamp | 1 Köy | 2 Kasaba | 3 Şehir |
|---|---|---|---|---|
| 10 | %0,9 (1) | %8,3 (9) | %36 (39) | %55 (60) |
| 20 | %0 (0) | %7,1 (8) | %32 (36) | %61 (68) |
| 30 | %0,9 (1) | %4,5 (5) | %29 (32) | %65 (72) |
| 40 | %1,9 (2) | %8,3 (9) | %24 (26) | %66 (71) |
| 50 | %0,9 (1) | %5,6 (6) | %29 (31) | %64 (69) |
| 60 | %0,9 (1) | %3,7 (4) | %29 (31) | %66 (71) |

## Dünyalar

| Seed | Medeniyet | Yerleşim | Nüfus | Çöküş | Efsane | En yüksek Sv | Doğan / ölü kahraman | İlk şehir (yıl, medyan) | Süre (sn) | Son hash |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | 8 | 80 | 3519 | 11 | 4 | 10 | 508 / 182 | – | 98,2 | `59308e383f41fe8f` |
| 2 | 5 | 82 | 3317 | 10 | 4 | 10 | 803 / 220 | – | 108 | `6569e0d7b1b93ed8` |
| 3 | 6 | 79 | 3214 | 12 | 6 | 10 | 335 / 139 | – | 73,4 | `5999b976678584d4` |
| 4 | 7 | 79 | 3479 | 15 | 1 | 10 | 969 / 143 | – | 121 | `336cb8994314c98c` |
| 5 | 6 | 81 | 3416 | 5 | 2 | 10 | 907 / 204 | – | 111 | `d9019c58cdcd11f8` |
| 6 | 10 | 80 | 2979 | 15 | 4 | 10 | 953 / 281 | – | 120 | `a04e5b8d9af1c746` |
| 7 | 6 | 73 | 2070 | 6 | 1 | 10 | 449 / 120 | – | 82,1 | `605d54d93b4be501` |
| 8 | 6 | 58 | 2025 | 13 | 1 | 10 | 578 / 159 | – | 77,5 | `c3c557d0746ea554` |
| 9 | 6 | 69 | 2368 | 13 | 5 | 10 | 792 / 311 | – | 71,6 | `338aab347963268c` |
| 10 | 8 | 78 | 2876 | 11 | 3 | 10 | 637 / 228 | – | 89,7 | `9a69c78270618a8e` |
| 11 | 5 | 69 | 2798 | 11 | 0 | 10 | 532 / 105 | – | 81,2 | `97eb92a0adc6dee5` |
| 12 | 6 | 81 | 2725 | 4 | 3 | 10 | 774 / 221 | – | 91,8 | `e92e9b8836dee05c` |
| 13 | 5 | 80 | 2747 | 14 | 2 | 10 | 462 / 113 | – | 88,9 | `8efc804962ff38a5` |
| 14 | 7 | 77 | 2732 | 12 | 3 | 10 | 603 / 185 | – | 112 | `0698ee42c74455ab` |
| 15 | 6 | 70 | 2828 | 15 | 4 | 10 | 485 / 171 | – | 118 | `87c3dcbaa15727d5` |
| 16 | 10 | 82 | 3457 | 7 | 3 | 10 | 973 / 327 | – | 214 | `441d1ab0e3aecaa8` |

Çöküşler:

- seed 1, 2. yıl (gün 51): Kızılboynuz Krallığı başkenti kaybetti: Karlıtepe (Yeminköprü Teokrasisi aldı)
- seed 1, 3. yıl (gün 98): Granitsunak Krallığı başkenti kaybetti: Aksırt (Demirçan Krallığı aldı)
- seed 1, 17. yıl (gün 670): Granitsunak Krallığı başkenti kaybetti: Demirbük (Demirçan Krallığı aldı)
- seed 1, 27. yıl (gün 1043): Yeni Sarısırt Teokrasisi yok oldu
- seed 1, 33. yıl (gün 1310): Yeminköprü Teokrasisi başkenti kaybetti: Karlıtepe (Granitsunak Krallığı aldı)
- seed 1, 34. yıl (gün 1330): Demirçan Krallığı başkenti kaybetti: Karaköy (Demirkanat Cumhuriyeti aldı)
- seed 1, 37. yıl (gün 1459): Granitsunak Cumhuriyeti başkenti kaybetti: Karlıtepe (Demirkanat Cumhuriyeti aldı)
- seed 1, 38. yıl (gün 1495): Balköy Cumhuriyeti yok oldu
- seed 1, 45. yıl (gün 1775): Kocaköprü Cumhuriyeti başkenti kaybetti: Kocaköprü (Yeminköprü Teokrasisi aldı)
- seed 1, 52. yıl (gün 2053): Demirkanat Cumhuriyeti başkenti kaybetti: Karlıtepe (Demirçan Krallığı aldı)
- seed 1, 59. yıl (gün 2360): Demirkanat Krallığı başkenti kaybetti: Demirkanat (Demirkanat Cumhuriyeti aldı)
- seed 2, 2. yıl (gün 62): Bulutkapı Boyları yok oldu
- seed 2, 3. yıl (gün 81): Pulzırh Krallığı başkenti kaybetti: Gölgeçarşı (Sessizkoru Boyları aldı)
- seed 2, 7. yıl (gün 272): Sessizkoru Boyları başkenti kaybetti: Gölgeçarşı (Kurtoba Boyları aldı)
- seed 2, 7. yıl (gün 276): Pulzırh Cumhuriyeti başkenti kaybetti: Tuzyurt (Sessizkoru Boyları aldı)
- seed 2, 8. yıl (gün 303): Telliköprü Teokrasisi yok oldu
- seed 2, 21. yıl (gün 815): Kuzeybük Boyları başkenti kaybetti: Kuzeybük (Taşkandil Krallığı aldı)
- seed 2, 21. yıl (gün 823): Tatlıçayır Teokrasisi başkenti kaybetti: Gölköprü (Sessizkoru Boyları aldı)
- seed 2, 27. yıl (gün 1070): Tatlıçayır Teokrasisi yok oldu
- seed 2, 53. yıl (gün 2087): Kuzeybük Boyları yok oldu
- seed 2, 59. yıl (gün 2346): Sarıdere Cumhuriyeti yok oldu
- seed 3, 1. yıl (gün 19): Ayışığı Cumhuriyeti başkenti kaybetti: Ayışığı Korusu (Kanlıdiş Boyları aldı)
- seed 3, 1. yıl (gün 31): Karaörs Krallığı başkenti kaybetti: Yelgeçit (Güneştacı Teokrasisi aldı)
- seed 3, 4. yıl (gün 123): Tuzyayla Krallığı başkenti kaybetti: Tuzyayla (Ayışığı Cumhuriyeti aldı)
- seed 3, 4. yıl (gün 154): Okyayı Boyları yok oldu
- seed 3, 5. yıl (gün 184): Kanlıdiş Boyları başkenti kaybetti: Yelgeçit (Karaörs Krallığı aldı)
- seed 3, 5. yıl (gün 198): Güneştacı Teokrasisi başkenti kaybetti: Günyazı (Tatlıçayır Boyları aldı)
- seed 3, 8. yıl (gün 315): Güneştacı Teokrasisi başkenti kaybetti: Günyazı (Tatlıçayır Boyları aldı)
- seed 3, 10. yıl (gün 391): Kanlıdiş Boyları başkenti kaybetti: Işıkdere (Tatlıçayır Boyları aldı)
- seed 3, 11. yıl (gün 410): Dumanpınar Teokrasisi yok oldu
- seed 3, 18. yıl (gün 683): Kuzeybük Boyları başkenti kaybetti: Kuzeybük (Güneştacı Teokrasisi aldı)
- seed 3, 24. yıl (gün 934): Kuzeybük Boyları yok oldu
- seed 3, 51. yıl (gün 2028): Kanlıdiş Ordugâhı II Harabesi Vadisi Harabesi Vadisi Krallığı yok oldu
- seed 4, 1. yıl (gün 38): Yeşilyaprak Boyları başkenti kaybetti: Şarkıdere (Pulzırh Krallığı aldı)
- seed 4, 3. yıl (gün 104): Demirbük Boyları başkenti kaybetti: Demirbük (Zincirkaya Krallığı aldı)
- seed 4, 4. yıl (gün 124): Karaörs Krallığı başkenti kaybetti: Kızılçayır (Yeşilyaprak Boyları aldı)
- seed 4, 16. yıl (gün 621): Sarıhisar Krallığı yok oldu
- seed 4, 21. yıl (gün 826): Kanlıtırnak Vadisi Krallığı başkenti kaybetti: Kanlıtırnak Vadisi (Yeşilyaprak Boyları aldı)
- seed 4, 28. yıl (gün 1094): Yeşilyaprak Boyları başkenti kaybetti: Gölgeörs Hac Yeri (terk edildi)
- seed 4, 30. yıl (gün 1165): Zincirkaya Cumhuriyeti başkenti kaybetti: Siskoru (Yeşilyaprak Boyları aldı)
- seed 4, 32. yıl (gün 1254): Pulzırh Teokrasisi başkenti kaybetti: Şarkıdere (Demirbük Boyları aldı)
- seed 4, 35. yıl (gün 1373): Demirbük Boyları başkenti kaybetti: Şarkıdere (Kalkanova Teokrasisi aldı)
- seed 4, 36. yıl (gün 1431): Toynakbaş Boyları yok oldu
- seed 4, 40. yıl (gün 1579): Kalkanova Teokrasisi başkenti kaybetti: Şarkıdere (Yeşilyaprak Boyları aldı)
- seed 4, 41. yıl (gün 1636): Ayburç Krallığı yok oldu
- seed 4, 42. yıl (gün 1662): Siskoru Boyları yok oldu
- seed 4, 48. yıl (gün 1885): Yeşilyaprak Boyları başkenti kaybetti: Şarkıdere (Kalkanova Teokrasisi aldı)
- seed 4, 55. yıl (gün 2175): Demirbük Boyları yok oldu
- seed 5, 1. yıl (gün 37): Karagöl Krallığı yok oldu
- seed 5, 8. yıl (gün 293): Kızılboynuz Krallığı başkenti kaybetti: Ceylanoba (Akburç Teokrasisi aldı)
- seed 5, 35. yıl (gün 1369): Alazvadi Cumhuriyeti yok oldu
- seed 5, 41. yıl (gün 1623): Telliköprü Krallığı yok oldu
- seed 5, 56. yıl (gün 2234): Meşekent Boyları yok oldu
- seed 6, 1. yıl (gün 37): Kızılyurt Boyları başkenti kaybetti: Şafaktepe (Kalkanova Teokrasisi aldı)
- seed 6, 3. yıl (gün 94): Demirkanat Boyları başkenti kaybetti: Işıkdere (Kalkanova Teokrasisi aldı)
- seed 6, 4. yıl (gün 142): Közsaray Krallığı başkenti kaybetti: Alacayayla (Kalkanova Teokrasisi aldı)
- seed 6, 6. yıl (gün 218): Közsaray Cumhuriyeti yok oldu
- seed 6, 9. yıl (gün 352): Taşkandil Krallığı başkenti kaybetti: Bulutkapı (Demirkanat Boyları aldı)
- seed 6, 10. yıl (gün 395): Kalkanova Teokrasisi başkenti kaybetti: Alacayayla (Kızılyurt Boyları aldı)
- seed 6, 16. yıl (gün 635): Kızılyurt Boyları başkenti kaybetti: Alacayayla (Fıçıköy Krallığı aldı)
- seed 6, 33. yıl (gün 1305): Fıçıköy Krallığı başkenti kaybetti: Alacayayla (Kalkanova Teokrasisi aldı)
- seed 6, 35. yıl (gün 1400): Gölgeçarşı Boyları başkenti kaybetti: Gölgeçarşı (Fıçıköy Krallığı aldı)
- seed 6, 36. yıl (gün 1440): Gölpınar Krallığı başkenti kaybetti: Sarıhisar (Demirkanat Boyları aldı)
- seed 6, 38. yıl (gün 1511): Demirkanat Boyları başkenti kaybetti: Sarıhisar (Taşkandil Cumhuriyeti aldı)
- seed 6, 40. yıl (gün 1573): Kılıçyurt Teokrasisi yok oldu
- seed 6, 48. yıl (gün 1891): Taşkandil Cumhuriyeti başkenti kaybetti: Sarıhisar (Demirkanat Boyları aldı)
- seed 6, 51. yıl (gün 2038): Demirkanat Boyları başkenti kaybetti: Sarıhisar (Gölpınar Teokrasisi aldı)
- seed 6, 55. yıl (gün 2199): Gölgeçarşı Boyları başkenti kaybetti: Zincirkaya (Fıçıköy Krallığı aldı)
- seed 7, 1. yıl (gün 6): Kurtoba Boyları yok oldu
- seed 7, 15. yıl (gün 583): Karaörs Teokrasisi yok oldu
- seed 7, 17. yıl (gün 645): Ulukoru Krallığı başkenti kaybetti: Ulukoru (Fıçıköy Krallığı aldı)
- seed 7, 19. yıl (gün 737): Kutsalörs Cumhuriyeti başkenti kaybetti: Karlıçayır (Fıçıköy Krallığı aldı)
- seed 7, 24. yıl (gün 942): Fıçıköy Krallığı başkenti kaybetti: Karlıçayır (Közburç Krallığı aldı)
- seed 7, 60. yıl (gün 2371): Kutsalörs Cumhuriyeti başkenti kaybetti: Yeni Yıldızkent (Taşhisar Boyları aldı)
- seed 8, 1. yıl (gün 8): Tatlıçayır Krallığı yok oldu
- seed 8, 4. yıl (gün 143): Sessizkoru Cumhuriyeti yok oldu
- seed 8, 6. yıl (gün 211): Gölgeçarşı Boyları başkenti kaybetti: Gölgeçarşı (Ayışığı Krallığı aldı)
- seed 8, 6. yıl (gün 211): Gölgeörs Boyları yok oldu
- seed 8, 10. yıl (gün 381): Çamkoru Krallığı yok oldu
- seed 8, 10. yıl (gün 385): Gölgeçarşı Boyları başkenti kaybetti: Gölgeörs (Taşkandil Teokrasisi aldı)
- seed 8, 14. yıl (gün 545): Aysırt Krallığı başkenti kaybetti: Aysırt (Pulzırh Krallığı aldı)
- seed 8, 17. yıl (gün 663): Gölgeçarşı Boyları yok oldu
- seed 8, 31. yıl (gün 1213): Ayışığı Krallığı başkenti kaybetti: Fıçıköy (Taşkandil Teokrasisi aldı)
- seed 8, 35. yıl (gün 1366): Ayışığı Cumhuriyeti başkenti kaybetti: Çamkoru (Taşkandil Teokrasisi aldı)
- seed 8, 36. yıl (gün 1406): Kızılçayır Boyları yok oldu
- seed 8, 46. yıl (gün 1811): Pulzırh Krallığı başkenti kaybetti: Meşekent (Aysırt Krallığı aldı)
- seed 8, 47. yıl (gün 1864): Yeni Balköy Boyları yok oldu
- seed 9, 2. yıl (gün 44): Ayışığı Krallığı yok oldu
- seed 9, 6. yıl (gün 216): Kızılboynuz Cumhuriyeti yok oldu
- seed 9, 7. yıl (gün 265): Demirkanat Krallığı başkenti kaybetti: Kurtkent (Alevgeçit Boyları aldı)
- seed 9, 9. yıl (gün 328): Sessizkoru Krallığı yok oldu
- seed 9, 11. yıl (gün 407): Ayışığı Cumhuriyeti yok oldu
- seed 9, 11. yıl (gün 433): Demirçan Cumhuriyeti başkenti kaybetti: Derinmihrap (Alevgeçit Boyları aldı)
- seed 9, 13. yıl (gün 502): Kızılboynuz Krallığı yok oldu
- seed 9, 21. yıl (gün 813): Gümüşdal Krallığı başkenti kaybetti: Kökbağ (Alevgeçit Boyları aldı)
- seed 9, 27. yıl (gün 1077): Alevgeçit Boyları başkenti kaybetti: Külçukur (Fıçıköy Krallığı aldı)
- seed 9, 29. yıl (gün 1158): Yeni Gökçayır Teokrasisi yok oldu
- seed 9, 31. yıl (gün 1224): Alevgeçit Boyları başkenti kaybetti: Günkaya (Gümüşdal Krallığı aldı)
- seed 9, 37. yıl (gün 1454): Güneştacı Krallığı başkenti kaybetti: Gökyayla (Demirçan Krallığı aldı)
- seed 9, 52. yıl (gün 2069): Güneştacı Krallığı yok oldu
- seed 10, 5. yıl (gün 193): Yeşilyaprak Cumhuriyeti başkenti kaybetti: Çamgözcü (Taşkandil Teokrasisi aldı)
- seed 10, 10. yıl (gün 370): Karamum Krallığı başkenti kaybetti: Meşekent (Taşkandil Teokrasisi aldı)
- seed 10, 17. yıl (gün 662): Taşkandil Teokrasisi başkenti kaybetti: Meşekent (Kızılyurt Boyları aldı)
- seed 10, 20. yıl (gün 762): Ayburç Boyları yok oldu
- seed 10, 26. yıl (gün 1036): Alacayayla Boyları yok oldu
- seed 10, 29. yıl (gün 1149): Taşkandil Teokrasisi başkenti kaybetti: Zincirkaya (Kızılyurt Boyları aldı)
- seed 10, 39. yıl (gün 1545): Karamum Cumhuriyeti başkenti kaybetti: Yeni Göloba (Alazvadi Teokrasisi aldı)
- seed 10, 45. yıl (gün 1770): Yeşilyaprak Krallığı başkenti kaybetti: Kızılçayır (Taşkandil Teokrasisi aldı)
- seed 10, 50. yıl (gün 1982): Gökyurt Cumhuriyeti yok oldu
- seed 10, 55. yıl (gün 2175): Közsaray Krallığı başkenti kaybetti: Yeni Bozburç (Yeşilyaprak Krallığı aldı)
- seed 10, 55. yıl (gün 2179): Gölpınar Boyları başkenti kaybetti: Gölpınar (Kızılyurt Boyları aldı)
- seed 11, 1. yıl (gün 35): Özgür Kutsalörs başkenti kaybetti: Yeşilyazı (Şafaktepe Krallığı aldı)
- seed 11, 3. yıl (gün 99): Közburç Krallığı başkenti kaybetti: Közburç (Şafaktepe Krallığı aldı)
- seed 11, 4. yıl (gün 133): Özgür Kutsalörs başkenti kaybetti: Okyayı (Karaörs Teokrasisi aldı)
- seed 11, 7. yıl (gün 273): Özgür Kutsalörs başkenti kaybetti: Okyayı (Karaörs Teokrasisi aldı)
- seed 11, 10. yıl (gün 390): Şafaktepe Krallığı başkenti kaybetti: Yeşilyazı (Karaörs Teokrasisi aldı)
- seed 11, 11. yıl (gün 430): Pulkalkan Krallığı yok oldu
- seed 11, 23. yıl (gün 884): Kocaköprü Krallığı yok oldu
- seed 11, 24. yıl (gün 945): Kutsalörs Boyları başkenti kaybetti: Yeni Gölkoru (Kuzeybük Krallığı aldı)
- seed 11, 32. yıl (gün 1256): Karaörs Teokrasisi yok oldu
- seed 11, 33. yıl (gün 1304): Şafaktepe Krallığı yok oldu
- seed 11, 35. yıl (gün 1381): Kuzeybük Cumhuriyeti başkenti kaybetti: Yeni Gölkoru (Kutsalörs Boyları aldı)
- seed 12, 9. yıl (gün 346): Közsaray Cumhuriyeti başkenti kaybetti: Okyayı (Taşkandil Krallığı aldı)
- seed 12, 31. yıl (gün 1232): Közsaray Cumhuriyeti yok oldu
- seed 12, 34. yıl (gün 1324): Fıçıköy Krallığı başkenti kaybetti: Gölgeçarşı (Taşkandil Krallığı aldı)
- seed 12, 37. yıl (gün 1446): Taşdere Teokrasisi yok oldu
- seed 13, 4. yıl (gün 122): Kızılyurt Boyları başkenti kaybetti: Kafatepe (Közburç Krallığı aldı)
- seed 13, 6. yıl (gün 240): Karlıçayır Boyları yok oldu
- seed 13, 8. yıl (gün 305): Kızılyurt Boyları başkenti kaybetti: Günyazı (Örsyürek Teokrasisi aldı)
- seed 13, 9. yıl (gün 349): Tamburlu Cumhuriyeti yok oldu
- seed 13, 12. yıl (gün 448): Özgür Tamburlu başkenti kaybetti: Tamburlu (Keseli Cumhuriyeti aldı)
- seed 13, 14. yıl (gün 538): Kızılyurt Boyları başkenti kaybetti: Granitsunak (Örsyürek Teokrasisi aldı)
- seed 13, 23. yıl (gün 881): Tamburlu Krallığı başkenti kaybetti: Kuzeybük (Ejderkale Teokrasisi aldı)
- seed 13, 27. yıl (gün 1042): Tamburlu Krallığı yok oldu
- seed 13, 34. yıl (gün 1326): Söğütsırt Boyları başkenti kaybetti: Söğütsırt (Ejderkale Teokrasisi aldı)
- seed 13, 35. yıl (gün 1386): Söğütsırt Boyları yok oldu
- seed 13, 37. yıl (gün 1455): Yeni Bozyurt Boyları yok oldu
- seed 13, 47. yıl (gün 1857): Közburç Krallığı başkenti kaybetti: Pınarhisar (Ejderkale Teokrasisi aldı)
- seed 13, 51. yıl (gün 2002): Yeni Ulukaya Boyları yok oldu
- seed 13, 60. yıl (gün 2379): Kızılyurt Boyları yok oldu
- seed 14, 5. yıl (gün 161): Güneştacı Krallığı başkenti kaybetti: Kurtgeçit (Pulzırh Krallığı aldı)
- seed 14, 5. yıl (gün 176): Taşhisar Boyları yok oldu
- seed 14, 8. yıl (gün 288): Yıldıztepe Boyları yok oldu
- seed 14, 9. yıl (gün 336): Toynakbaş Boyları başkenti kaybetti: Pınardere (Pulzırh Krallığı aldı)
- seed 14, 12. yıl (gün 459): Sarıdere Krallığı yok oldu
- seed 14, 21. yıl (gün 803): Pulzırh Cumhuriyeti başkenti kaybetti: Pınardere (Gümüşdal Krallığı aldı)
- seed 14, 43. yıl (gün 1690): Gümüşdal Krallığı başkenti kaybetti: Pınardere (Toynakbaş Boyları aldı)
- seed 14, 47. yıl (gün 1843): Işıkdere Cumhuriyeti başkenti kaybetti: Yeni Dumansırt (Okyayı Krallığı aldı)
- seed 14, 48. yıl (gün 1903): Gümüşdal Krallığı başkenti kaybetti: Gölpınar (Toynakbaş Boyları aldı)
- seed 14, 52. yıl (gün 2045): Gümüşdal Krallığı başkenti kaybetti: Demirçene Vadisi III Kazısı (terk edildi)
- seed 14, 52. yıl (gün 2080): Toynakbaş Boyları başkenti kaybetti: Pınardere (Güneştacı Krallığı aldı)
- seed 14, 58. yıl (gün 2312): Uluova Teokrasisi başkenti kaybetti: Uluova (Okyayı Krallığı aldı)
- seed 15, 6. yıl (gün 219): Sarıhisar Boyları başkenti kaybetti: Sarıhisar (Balköprü Teokrasisi aldı)
- seed 15, 7. yıl (gün 246): Meşekent Cumhuriyeti başkenti kaybetti: Kafatepe (Kızılkül Cumhuriyeti aldı)
- seed 15, 9. yıl (gün 352): Kızılkül Cumhuriyeti başkenti kaybetti: Kırkkapı (Balköprü Teokrasisi aldı)
- seed 15, 12. yıl (gün 444): Sarıhisar Boyları yok oldu
- seed 15, 18. yıl (gün 702): Közsaray Cumhuriyeti başkenti kaybetti: Kırkkapı (Kızılkül Krallığı aldı)
- seed 15, 19. yıl (gün 742): Kızılkül Krallığı başkenti kaybetti: Kırkkapı (Balköprü Teokrasisi aldı)
- seed 15, 31. yıl (gün 1230): Yelköy Krallığı yok oldu
- seed 15, 35. yıl (gün 1382): Yelköy Cumhuriyeti yok oldu
- seed 15, 36. yıl (gün 1409): Közsaray Krallığı başkenti kaybetti: Ceylanoba (Meşekent Cumhuriyeti aldı)
- seed 15, 36. yıl (gün 1428): Közsaray Krallığı yok oldu
- seed 15, 45. yıl (gün 1773): Bozkent Boyları yok oldu
- seed 15, 47. yıl (gün 1850): Kızılyurt Boyları yok oldu
- seed 15, 49. yıl (gün 1945): Demirkanat Teokrasisi başkenti kaybetti: Alacayurt (Kızılkül Krallığı aldı)
- seed 15, 52. yıl (gün 2077): Pınarhisar Boyları yok oldu
- seed 15, 58. yıl (gün 2294): Şarkıdere Teokrasisi yok oldu
- seed 16, 1. yıl (gün 22): Söğütburç Cumhuriyeti yok oldu
- seed 16, 2. yıl (gün 67): Alazvadi Cumhuriyeti başkenti kaybetti: Meşekent (Alevgeçit Krallığı aldı)
- seed 16, 34. yıl (gün 1330): Alevgeçit Cumhuriyeti başkenti kaybetti: Söğütburç (Şafaktepe Teokrasisi aldı)
- seed 16, 36. yıl (gün 1435): Kıvılcımlı Teokrasisi başkenti kaybetti: Yeni Kızılkale (Kurtoba Boyları aldı)
- seed 16, 49. yıl (gün 1953): Akburç Cumhuriyeti yok oldu
- seed 16, 54. yıl (gün 2155): Alazvadi Krallığı başkenti kaybetti: Kalkanova (Kıvılcımlı Teokrasisi aldı)
- seed 16, 57. yıl (gün 2244): Yeni Yeşiloba Teokrasisi yok oldu

## Yıllık ayrıntı

Hücre: medyan (p10–p90), 16 dünya. Yıl y = (y−1)·40+1 … y·40. günler. Bütün değerler `report.json` içinde (`metrics`), dünya başına değerler `../runs/f2-a` altında.

### Medeniyet (1/3)

| Yıl | Yaşayan medeniyet | Yeni medeniyet (yeniden doğan) | Yok olan medeniyet | Başkent kaybı (medeniyet yaşarken) | Çöküş (yok olma + başkent kaybı) | Yaşayan yerleşim |
|---|---|---|---|---|---|---|
| 1 | 6 (6–8) | 0 (0–0,5) | 0 (0–1) | 0 (0–1) | 0,5 (0–1) | 78,5 (71–81,5) |
| 2 | 6 (6–8) | 0 | 0 (0–0,5) | 0 (0–0,5) | 0 (0–1) | 80 (69,5–81,5) |
| 3 | 6 (6–8) | 0 (0–1) | 0 | 0 (0–1) | 0 (0–1) | 80 (69,5–82) |
| 4 | 6 (6–8) | 0 (0–0,5) | 0 (0–0,5) | 0 (0–1) | 0 (0–1) | 80 (66,5–81) |
| 5 | 7 (6–8) | 0 (0–1) | 0 | 0 (0–1) | 0 (0–1,5) | 79 (61–81) |
| 6 | 6,5 (5–8) | 0 | 0 (0–1) | 0 (0–0,5) | 0 (0–1) | 79 (62,5–81,5) |
| 7 | 6,5 (6–8) | 0 (0–1) | 0 | 0 (0–1) | 0 (0–1) | 79 (59–81) |
| 8 | 6,5 (5,5–8) | 0 (0–0,5) | 0 (0–0,5) | 0 (0–1) | 0 (0–1) | 79 (60–81) |
| 9 | 6,5 (5–8) | 0 | 0 (0–0,5) | 0 (0–1) | 0 (0–1) | 79 (61–82) |
| 10 | 7 (5,5–8) | 0 (0–1) | 0 | 0 (0–1) | 0 (0–1) | 79 (61,5–81,5) |
| 11 | 7 (5,5–8,5) | 0 (0–1) | 0 (0–1) | 0 | 0 (0–1) | 79 (59,5–82) |
| 12 | 7 (6–8,5) | 0 (0–1) | 0 (0–0,5) | 0 | 0 (0–1) | 79 (62–81) |
| 13 | 7 (5,5–8,5) | 0 | 0 | 0 | 0 | 79,5 (61–81,5) |
| 14 | 7 (6–8,5) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 79,5 (62–81) |
| 15 | 7 (5,5–8,5) | 0 | 0 | 0 | 0 | 79 (63–81,5) |
| 16 | 7 (6–8) | 0 | 0 | 0 | 0 (0–0,5) | 80 (63,5–81) |
| 17 | 7 (5,5–8) | 0 | 0 | 0 (0–1) | 0 (0–1) | 79,5 (64–81,5) |
| 18 | 7 (5,5–8) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 79 (65–81) |
| 19 | 7 (5,5–8,5) | 0 (0–1) | 0 | 0 (0–0,5) | 0 (0–0,5) | 80 (64–81) |
| 20 | 7 (5,5–8) | 0 | 0 | 0 | 0 | 79,5 (67,5–82) |
| 21 | 7 (5,5–8) | 0 | 0 | 0 (0–1) | 0 (0–1) | 79,5 (67–82) |
| 22 | 7 (5,5–8) | 0 | 0 | 0 | 0 | 78 (68–80) |
| 23 | 7 (5,5–8) | 0 | 0 | 0 | 0 (0–0,5) | 79 (67,5–81) |
| 24 | 7 (6–8) | 0 | 0 | 0 (0–0,5) | 0 (0–1) | 79,5 (63,5–81) |
| 25 | 7 (6–8) | 0 | 0 | 0 | 0 | 80 (66–81) |
| 26 | 7 (6–8) | 0 | 0 | 0 | 0 | 80 (67,5–81) |
| 27 | 6 (5,5–8) | 0 | 0 (0–1) | 0 | 0 (0–1) | 80 (65,5–82) |
| 28 | 6 (6–8) | 0 | 0 | 0 | 0 | 80 (65,5–81,5) |
| 29 | 6 (6–8) | 0 | 0 | 0 | 0 (0–0,5) | 79,5 (66–82) |
| 30 | 6 (6–8) | 0 | 0 | 0 | 0 | 79,5 (66,5–81) |
| 31 | 6 (5,5–8) | 0 | 0 (0–0,5) | 0 (0–0,5) | 0 (0–1) | 79 (67,5–81) |
| 32 | 6 (6–8) | 0 | 0 | 0 | 0 (0–0,5) | 79,5 (68–81) |
| 33 | 6 (5,5–8) | 0 | 0 | 0 (0–0,5) | 0 (0–1) | 79 (68,5–80,5) |
| 34 | 6 (6–8) | 0 (0–0,5) | 0 | 0 (0–1) | 0 (0–1) | 80 (69–81,5) |
| 35 | 6,5 (6–8) | 0 (0–0,5) | 0 (0–1) | 0 (0–1) | 0 (0–1) | 79 (65,5–81) |
| 36 | 6 (6–8) | 0 | 0 (0–1) | 0 (0–1) | 0 (0–1) | 79,5 (66–81,5) |
| 37 | 6 (5–8) | 0 (0–0,5) | 0 (0–0,5) | 0 (0–0,5) | 0 (0–1) | 79,5 (67,5–82) |
| 38 | 6 (5,5–8,5) | 0 (0–0,5) | 0 | 0 | 0 (0–0,5) | 80 (66–81) |
| 39 | 6 (5,5–8,5) | 0 | 0 | 0 | 0 | 79 (63,5–82,5) |
| 40 | 6 (5,5–8) | 0 | 0 | 0 | 0 (0–0,5) | 79 (65,5–81) |
| 41 | 6 (5,5–8) | 0 (0–0,5) | 0 (0–0,5) | 0 | 0 (0–0,5) | 79,5 (65,5–81) |
| 42 | 6 (5,5–8) | 0 | 0 | 0 | 0 | 79 (66–81,5) |
| 43 | 6 (6–8) | 0 | 0 | 0 | 0 | 79 (66,5–81) |
| 44 | 6 (6–8) | 0 | 0 | 0 | 0 | 80 (67,5–81,5) |
| 45 | 6 (6–8) | 0 | 0 | 0 (0–0,5) | 0 (0–1) | 80 (67,5–81) |
| 46 | 6 (6–8) | 0 | 0 | 0 | 0 | 79,5 (69–81) |
| 47 | 6 (6–8) | 0 (0–0,5) | 0 (0–0,5) | 0 (0–0,5) | 0 (0–1) | 80 (68,5–81) |
| 48 | 6,5 (6–8) | 0 | 0 | 0 (0–1) | 0 (0–1) | 80 (68–81) |
| 49 | 6,5 (6–8) | 0 | 0 | 0 | 0 (0–0,5) | 79,5 (68–82,5) |
| 50 | 6,5 (6–7,5) | 0 | 0 | 0 | 0 | 80 (67,5–82) |
| 51 | 6 (6–8) | 0 (0–0,5) | 0 (0–0,5) | 0 | 0 (0–1) | 79 (68–82) |
| 52 | 6 (6–8,5) | 0 | 0 (0–0,5) | 0 (0–0,5) | 0 (0–1) | 79 (69,5–81,5) |
| 53 | 6 (5,5–8,5) | 0 | 0 | 0 | 0 | 79 (68,5–81) |
| 54 | 6 (5,5–8,5) | 0 | 0 | 0 | 0 | 79,5 (67–81,5) |
| 55 | 6 (5,5–8,5) | 0 | 0 | 0 (0–0,5) | 0 (0–1) | 78,5 (68–82) |
| 56 | 6 (5,5–9) | 0 (0–1) | 0 | 0 | 0 | 80 (68–81) |
| 57 | 6 (6–9) | 0 (0–0,5) | 0 | 0 | 0 | 79 (69–81,5) |
| 58 | 6 (6–9) | 0 | 0 | 0 | 0 (0–0,5) | 79 (68,5–83) |
| 59 | 6 (5,5–9) | 0 | 0 | 0 | 0 (0–0,5) | 78,5 (68,5–81) |
| 60 | 6 (5–9) | 0 | 0 | 0 | 0 (0–0,5) | 79 (69–81,5) |

### Medeniyet (2/3)

| Yıl | Medeniyet başına yerleşim | 5+ kara yerleşimli medeniyet payı | Kurulan yerleşim | Fethedilen yerleşim | Terk edilen yerleşim | Toplam nüfus |
|---|---|---|---|---|---|---|
| 1 | 12,6 (9,81–13,5) | %83 (%56–%94) | 3 (1–4) | 2,5 (1–5) | 0 (0–2) | 2777 (2323–3141) |
| 2 | 11,9 (10,1–13,4) | %77 (%61–%94) | 0 (0–2) | 2 (1–3,5) | 0 (0–1,5) | 2680 (2270–3193) |
| 3 | 11,6 (9,94–13,5) | %83 (%56–%94) | 0 (0–1,5) | 3 (1–5) | 0 (0–1) | 2812 (2192–3204) |
| 4 | 12 (9,58–13,4) | %83 (%54–%87) | 0 (0–1) | 2 (1–4,5) | 0 (0–1,5) | 2715 (2260–3206) |
| 5 | 11,3 (9–13,3) | %71 (%53–%87) | 0 (0–1,5) | 3,5 (0,5–4) | 0,5 (0–3,5) | 2728 (2346–3234) |
| 6 | 11,5 (9,27–14,5) | %82 (%58–%94) | 0 (0–1,5) | 2 (1–4) | 0 (0–3) | 2770 (2158–3236) |
| 7 | 11,4 (9,07–13,3) | %77 (%53–%86) | 0 (0–2) | 3 (1–4,5) | 0 (0–2,5) | 2846 (2274–3323) |
| 8 | 10,6 (9,23–13,5) | %79 (%53–%93) | 1 (0–2,5) | 2 (1–3,5) | 0 (0–2,5) | 2856 (2230–3392) |
| 9 | 11,1 (9,66–14,5) | %79 (%53–%100) | 0 (0–3) | 1 (0,5–3,5) | 0,5 (0–4,5) | 2919 (2281–3364) |
| 10 | 10,7 (9,38–13,5) | %77 (%47–%94) | 1 (0–4) | 3 (1–4,5) | 0 (0–2,5) | 2942 (2374–3323) |
| 11 | 11,2 (9,43–13,4) | %73 (%53–%86) | 0 (0–2) | 2 (0–4) | 0 (0–1) | 3010 (2403–3338) |
| 12 | 11,3 (8,78–13,4) | %71 (%53–%93) | 0,5 (0–2,5) | 1 (0–4) | 1 (0–1,5) | 2954 (2334–3307) |
| 13 | 11,1 (8,92–13,6) | %71 (%56–%100) | 0 (0–2,5) | 2 (0,5–3,5) | 0 (0–3,5) | 2932 (2364–3353) |
| 14 | 11,4 (9–13,4) | %75 (%58–%93) | 0 (0–3) | 2 (0–4,5) | 0 (0–4) | 2922 (2288–3414) |
| 15 | 11,4 (8,89–13,9) | %75 (%54–%85) | 0 (0–2) | 2 (0–4,5) | 0 (0–1) | 3099 (2306–3246) |
| 16 | 11,4 (8,86–13,4) | %69 (%60–%86) | 0 (0–2) | 2 (0,5–3,5) | 0 (0–2) | 3156 (2367–3510) |
| 17 | 11,5 (9,06–13,5) | %75 (%65–%92) | 0 (0–3) | 2 (1–4,5) | 0 (0–3) | 3040 (2316–3433) |
| 18 | 11,3 (8,94–13,4) | %73 (%67–%92) | 0 (0–2,5) | 2 (0,5–3) | 0,5 (0–2) | 3145 (2389–3359) |
| 19 | 11,4 (8,57–13,4) | %71 (%58–%82) | 0 (0–2) | 2 (0,5–4) | 0 (0–2) | 3162 (2368–3370) |
| 20 | 11,4 (9,06–13,5) | %67 (%57–%78) | 0,5 (0–3,5) | 1,5 (0–4) | 1 (0–2) | 2977 (2296–3430) |
| 21 | 11,4 (8,94–13,6) | %71 (%54–%78) | 1 (0–2) | 2 (1–4) | 0 (0–1,5) | 3116 (2334–3484) |
| 22 | 11,2 (9–13,4) | %67 (%54–%78) | 0 (0–2) | 2 (0–4) | 1 (0–3,5) | 3154 (2368–3420) |
| 23 | 11,3 (9,41–13,6) | %71 (%54–%85) | 1 (0–3,5) | 1,5 (0,5–3,5) | 0,5 (0–3,5) | 3039 (2268–3367) |
| 24 | 10,8 (9,48–13,3) | %68 (%54–%83) | 1 (0–3) | 2 (1–3,5) | 0 (0–3,5) | 3112 (2340–3538) |
| 25 | 11 (9,73–13,4) | %67 (%54–%83) | 1 (0–2) | 2 (0–4) | 0,5 (0–1) | 3211 (2416–3470) |
| 26 | 11,3 (9,87–13,5) | %67 (%54–%82) | 0 (0–2) | 2 (0–2,5) | 0 (0–2,5) | 3065 (2352–3443) |
| 27 | 11,2 (9,73–13,7) | %68 (%56–%83) | 0 (0–2) | 1 (0,5–4,5) | 0 (0–1) | 3128 (2380–3456) |
| 28 | 10,9 (9,73–13,6) | %67 (%60–%83) | 0 (0–2,5) | 2 (1–3) | 0 (0–1) | 3022 (2352–3481) |
| 29 | 11 (9,87–13,7) | %67 (%60–%82) | 0,5 (0–2,5) | 1,5 (0–2,5) | 0,5 (0–2) | 3078 (2404–3626) |
| 30 | 11 (9,87–13,6) | %67 (%54–%82) | 0 (0–2,5) | 1 (0–3,5) | 0 (0–2) | 3084 (2428–3615) |
| 31 | 10,8 (9,87–14,5) | %67 (%59–%82) | 0 (0–2) | 2 (2–3) | 0 (0–2) | 3114 (2485–3602) |
| 32 | 11,3 (9,94–13,5) | %67 (%56–%83) | 0 (0–2) | 1 (0–4) | 0 (0–1) | 3214 (2564–3704) |
| 33 | 11,4 (9,81–13,7) | %67 (%56–%83) | 0 (0–1,5) | 2 (1–3,5) | 0 (0–3) | 3182 (2548–3426) |
| 34 | 11,5 (9,81–13,5) | %67 (%56–%83) | 1 (0–2) | 1 (0–3) | 0 (0–1) | 3164 (2592–3490) |
| 35 | 11,4 (9,44–13,3) | %67 (%54–%83) | 0 (0–1) | 2 (0–4) | 0 (0–1) | 3020 (2556–3526) |
| 36 | 11,5 (10–13,6) | %69 (%54–%83) | 1 (0–2,5) | 2 (1–3,5) | 0 (0–3,5) | 3024 (2458–3642) |
| 37 | 11,7 (9,81–14,8) | %67 (%59–%83) | 0 (0–2,5) | 2 (1–3) | 0 (0–1,5) | 3126 (2552–3579) |
| 38 | 11,7 (9,38–13,6) | %67 (%56–%83) | 0 (0–2,5) | 2 (0–3) | 0,5 (0–2) | 3112 (2490–3650) |
| 39 | 11,8 (9,44–13,8) | %67 (%56–%83) | 0 (0–1) | 2 (0–3,5) | 0 (0–3) | 3088 (2426–3496) |
| 40 | 11,5 (9,81–13,7) | %67 (%59–%82) | 1 (0–3) | 2 (0–3) | 0,5 (0–1) | 3112 (2406–3562) |
| 41 | 11,5 (9,88–13,5) | %67 (%61–%82) | 0 (0–1,5) | 2 (0–3) | 0 (0–1,5) | 3180 (2500–3544) |
| 42 | 11,5 (10–13,6) | %67 (%63–%78) | 0 (0–3,5) | 2 (0,5–4) | 0,5 (0–4) | 3084 (2499–3457) |
| 43 | 11,5 (10–13,3) | %69 (%63–%82) | 0 (0–4,5) | 1,5 (0–2,5) | 0,5 (0–4) | 3045 (2476–3556) |
| 44 | 11,5 (9,88–13,5) | %67 (%56–%82) | 0,5 (0–3,5) | 1,5 (0–3,5) | 0 (0–1) | 3020 (2464–3448) |
| 45 | 11,6 (9,94–13,5) | %69 (%63–%85) | 0 (0–1,5) | 1 (0,5–2) | 0 (0–1) | 3014 (2327–3480) |
| 46 | 11,5 (9,69–13,5) | %71 (%56–%85) | 0 (0–1,5) | 1 (1–2,5) | 1 (0–1,5) | 2878 (2269–3544) |
| 47 | 12,1 (9,86–13,5) | %71 (%56–%85) | 0 (0–3) | 2 (1–3) | 0 (0–2) | 2986 (2360–3506) |
| 48 | 11,5 (9,61–13,6) | %69 (%52–%85) | 0 (0–1) | 2 (0,5–3,5) | 0 (0–1,5) | 2952 (2338–3438) |
| 49 | 11,5 (9,79–13,7) | %71 (%59–%83) | 0 (0–2) | 1,5 (0–3) | 0 (0–1) | 2914 (2292–3352) |
| 50 | 11,5 (9,92–13,6) | %71 (%54–%85) | 0 (0–1) | 1 (0,5–2,5) | 0 (0–1) | 2920 (2343–3380) |
| 51 | 12,2 (9,79–13,7) | %71 (%54–%83) | 0 (0–2) | 2 (0,5–4) | 1 (0–2,5) | 2960 (2314–3422) |
| 52 | 12,8 (9,26–13,6) | %69 (%50–%85) | 0 (0–1) | 2 (1–3) | 1 (0–2) | 2996 (2268–3536) |
| 53 | 12,3 (9,38–13,7) | %69 (%50–%83) | 1 (0–1,5) | 2 (0,5–3,5) | 0,5 (0–3,5) | 2928 (2276–3432) |
| 54 | 11,6 (9,31–13,5) | %71 (%53–%83) | 0 (0–2,5) | 2 (1–3) | 0 (0–3) | 2913 (2346–3547) |
| 55 | 12,7 (9,26–13,7) | %67 (%50–%83) | 0 (0–3,5) | 2 (1–3,5) | 0,5 (0–3,5) | 2888 (2212–3528) |
| 56 | 12,9 (9,05–13,7) | %73 (%50–%83) | 1 (0–3) | 1,5 (0–4) | 0 (0–2) | 2997 (2337–3525) |
| 57 | 12,1 (8,98–13,7) | %63 (%50–%83) | 0 (0–1) | 2 (0–4) | 0 (0–1) | 2870 (2320–3536) |
| 58 | 12,8 (8,96–13,8) | %67 (%50–%83) | 0 (0–2) | 2 (0–4) | 0 (0–2) | 2828 (2250–3466) |
| 59 | 12,5 (9,02–13,7) | %67 (%50–%85) | 0,5 (0–4,5) | 1 (0–3,5) | 1 (0–4) | 2952 (2270–3355) |
| 60 | 11,6 (8,93–14,9) | %73 (%50–%87) | 0 (0–1) | 2 (0,5–3,5) | 0,5 (0–2,5) | 2852 (2219–3468) |

### Medeniyet (3/3)

| Yıl | Altın medyanı (medeniyetler) | Boştaki iş gücü payı | Bölünme (ayrılıp kurulan medeniyet) | En büyük medeniyetin yerleşimi |
|---|---|---|---|---|
| 1 | 326 (237–650) | %20 (%11–%30) | 0 (0–0,5) | 20 (17–25) |
| 2 | 389 (168–563) | %19 (%12–%30) | 0 | 20 (18–25) |
| 3 | 336 (151–558) | %20 (%11–%31) | 0 (0–0,5) | 20 (17,5–25,5) |
| 4 | 329 (105–669) | %17 (%9,8–%37) | 0 (0–0,5) | 20 (17–26) |
| 5 | 368 (73,5–587) | %19 (%12–%37) | 0 (0–1) | 20,5 (18,5–27) |
| 6 | 356 (89,2–562) | %21 (%9,6–%38) | 0 | 20,5 (18–26) |
| 7 | 293 (84,9–481) | %20 (%9,2–%38) | 0 (0–0,5) | 20 (18,5–25,5) |
| 8 | 210 (102–425) | %23 (%9,4–%36) | 0 (0–0,5) | 20,5 (18–26,5) |
| 9 | 260 (123–427) | %20 (%11–%37) | 0 | 20,5 (18–26,5) |
| 10 | 262 (67,7–519) | %24 (%10–%34) | 0 (0–1) | 20 (18–27) |
| 11 | 235 (93,1–450) | %21 (%11–%33) | 0 (0–1) | 20,5 (17,5–27) |
| 12 | 319 (72,7–626) | %23 (%9,5–%33) | 0 (0–0,5) | 20 (17,5–27) |
| 13 | 324 (78,1–649) | %24 (%9–%33) | 0 | 21 (18,5–27,5) |
| 14 | 281 (145–541) | %24 (%11–%32) | 0 | 20 (18–27,5) |
| 15 | 320 (87,4–640) | %23 (%11–%33) | 0 | 21 (17,5–28) |
| 16 | 343 (112–740) | %23 (%13–%32) | 0 | 21,5 (18–29) |
| 17 | 337 (62,5–539) | %22 (%12–%32) | 0 | 22,5 (17,5–29,5) |
| 18 | 393 (169–516) | %20 (%12–%35) | 0 | 21,5 (17–30) |
| 19 | 351 (113–682) | %20 (%11–%38) | 0 (0–1) | 22 (17–31) |
| 20 | 240 (159–581) | %19 (%12–%34) | 0 | 22,5 (17,5–30,5) |
| 21 | 325 (117–592) | %19 (%11–%34) | 0 | 22,5 (18,5–31) |
| 22 | 367 (150–675) | %18 (%12–%37) | 0 | 23 (18,5–30) |
| 23 | 243 (116–486) | %20 (%11–%38) | 0 | 23 (19–30,5) |
| 24 | 208 (69,9–565) | %20 (%14–%41) | 0 | 24 (18–30) |
| 25 | 345 (89,5–506) | %19 (%16–%39) | 0 | 24 (19–30,5) |
| 26 | 302 (76,9–525) | %20 (%15–%36) | 0 | 24 (19,5–29) |
| 27 | 257 (144–666) | %19 (%14–%37) | 0 | 24 (18,5–29,5) |
| 28 | 371 (161–712) | %19 (%12–%36) | 0 | 24 (18,5–30) |
| 29 | 358 (169–565) | %17 (%11–%38) | 0 | 24 (18,5–29,5) |
| 30 | 369 (223–629) | %19 (%12–%36) | 0 | 24,5 (19–30,5) |
| 31 | 374 (233–788) | %19 (%13–%34) | 0 | 24,5 (19–30,5) |
| 32 | 339 (105–816) | %19 (%10–%31) | 0 | 24,5 (19–32) |
| 33 | 317 (50,1–845) | %21 (%15–%33) | 0 | 24,5 (20–31,5) |
| 34 | 278 (31,7–683) | %22 (%9,7–%36) | 0 (0–0,5) | 23,5 (20,5–31) |
| 35 | 233 (35,5–765) | %21 (%11–%38) | 0 (0–0,5) | 23,5 (21,5–31,5) |
| 36 | 388 (64,7–699) | %20 (%13–%40) | 0 | 24 (20–32) |
| 37 | 344 (87,3–678) | %22 (%13–%40) | 0 | 23 (20,5–32,5) |
| 38 | 294 (54,3–518) | %22 (%13–%39) | 0 (0–0,5) | 23 (19–32) |
| 39 | 300 (125–796) | %21 (%13–%41) | 0 | 24 (19,5–32,5) |
| 40 | 357 (71,7–799) | %21 (%13–%38) | 0 | 23 (18,5–32,5) |
| 41 | 399 (159–775) | %22 (%15–%38) | 0 (0–0,5) | 23,5 (18–31,5) |
| 42 | 281 (103–874) | %24 (%12–%44) | 0 | 24,5 (19,5–32,5) |
| 43 | 247 (81,2–674) | %22 (%13–%45) | 0 | 23,5 (18,5–31,5) |
| 44 | 262 (98,4–675) | %24 (%13–%44) | 0 | 23,5 (19–32) |
| 45 | 238 (52,2–672) | %24 (%12–%41) | 0 | 24,5 (19,5–33) |
| 46 | 297 (98,7–889) | %23 (%13–%39) | 0 | 24 (19,5–32) |
| 47 | 400 (125–679) | %21 (%13–%37) | 0 (0–0,5) | 24 (18,5–32) |
| 48 | 396 (43,4–763) | %21 (%14–%35) | 0 | 24,5 (19–33) |
| 49 | 339 (162–614) | %21 (%13–%38) | 0 | 24,5 (19–32,5) |
| 50 | 271 (85,3–497) | %22 (%15–%36) | 0 | 24,5 (18,5–32,5) |
| 51 | 322 (64,8–752) | %21 (%12–%39) | 0 (0–0,5) | 25 (19,5–33) |
| 52 | 307 (114–605) | %21 (%12–%41) | 0 | 24,5 (19,5–33) |
| 53 | 317 (127–659) | %24 (%13–%41) | 0 | 25,5 (20,5–31,5) |
| 54 | 325 (178–644) | %23 (%12–%41) | 0 | 27 (20,5–30) |
| 55 | 335 (77,5–641) | %28 (%11–%40) | 0 | 25,5 (20–32) |
| 56 | 340 (127–760) | %30 (%10–%42) | 0 (0–1) | 25,5 (20–31,5) |
| 57 | 278 (155–574) | %28 (%8,8–%41) | 0 (0–0,5) | 26,5 (21,5–31,5) |
| 58 | 248 (64,5–763) | %30 (%11–%42) | 0 | 26 (21,5–31) |
| 59 | 161 (77–682) | %30 (%9,6–%41) | 0 | 24,5 (21,5–31,5) |
| 60 | 253 (71,3–656) | %29 (%12–%42) | 0 | 25,5 (21,5–32) |

### Olaylar

| Yıl | Olay | Büyük olay |
|---|---|---|
| 1 | 1207 (1049–1621) | 244 (177–336) |
| 2 | 1238 (1016–1626) | 235 (181–288) |
| 3 | 1245 (1044–1620) | 228 (175–319) |
| 4 | 1198 (960–1650) | 250 (173–329) |
| 5 | 1269 (1031–1576) | 255 (180–326) |
| 6 | 1226 (1018–1566) | 248 (174–306) |
| 7 | 1246 (975–1650) | 254 (187–313) |
| 8 | 1222 (1012–1762) | 266 (202–348) |
| 9 | 1262 (1013–1693) | 245 (172–352) |
| 10 | 1302 (960–1633) | 250 (174–351) |
| 11 | 1314 (1122–1604) | 248 (174–316) |
| 12 | 1342 (1075–1650) | 242 (154–323) |
| 13 | 1298 (1068–1702) | 264 (176–373) |
| 14 | 1333 (1082–1608) | 246 (176–346) |
| 15 | 1362 (1074–1613) | 264 (156–360) |
| 16 | 1284 (1106–1658) | 258 (186–324) |
| 17 | 1328 (1084–1746) | 248 (174–356) |
| 18 | 1290 (1123–1706) | 266 (164–364) |
| 19 | 1281 (1090–1746) | 268 (186–316) |
| 20 | 1360 (1054–1672) | 232 (168–340) |
| 21 | 1356 (1100–1712) | 258 (178–317) |
| 22 | 1307 (1088–1724) | 264 (198–322) |
| 23 | 1372 (1038–1770) | 247 (179–363) |
| 24 | 1383 (1076–1730) | 247 (179–322) |
| 25 | 1374 (1048–1733) | 249 (166–328) |
| 26 | 1339 (1068–1732) | 226 (138–316) |
| 27 | 1306 (1091–1794) | 248 (151–326) |
| 28 | 1377 (1067–1714) | 239 (156–326) |
| 29 | 1369 (1114–1708) | 242 (180–326) |
| 30 | 1419 (1158–1634) | 226 (193–322) |
| 31 | 1386 (1147–1644) | 248 (190–344) |
| 32 | 1470 (1132–1692) | 231 (172–326) |
| 33 | 1469 (1164–1714) | 248 (170–309) |
| 34 | 1424 (1156–1744) | 242 (164–327) |
| 35 | 1466 (1210–1754) | 260 (208–354) |
| 36 | 1479 (1281–1764) | 238 (200–342) |
| 37 | 1456 (1184–1783) | 240 (188–319) |
| 38 | 1424 (1203–1726) | 236 (182–316) |
| 39 | 1444 (1202–1600) | 226 (195–312) |
| 40 | 1462 (1232–1666) | 242 (187–350) |
| 41 | 1526 (1269–1680) | 248 (208–357) |
| 42 | 1518 (1259–1830) | 266 (226–380) |
| 43 | 1527 (1278–1798) | 254 (208–338) |
| 44 | 1512 (1258–1806) | 265 (194–374) |
| 45 | 1602 (1245–1784) | 300 (214–346) |
| 46 | 1526 (1186–1788) | 264 (204–300) |
| 47 | 1526 (1174–1756) | 271 (179–332) |
| 48 | 1580 (1179–1778) | 258 (192–332) |
| 49 | 1501 (1206–1799) | 254 (183–346) |
| 50 | 1501 (1157–1722) | 224 (184–336) |
| 51 | 1582 (1204–1735) | 258 (197–328) |
| 52 | 1568 (1172–1798) | 275 (178–334) |
| 53 | 1610 (1218–1868) | 271 (169–306) |
| 54 | 1591 (1155–1901) | 268 (182–350) |
| 55 | 1568 (1160–1890) | 248 (168–369) |
| 56 | 1589 (1191–1890) | 272 (168–364) |
| 57 | 1598 (1146–1867) | 264 (178–345) |
| 58 | 1672 (1160–1874) | 294 (174–402) |
| 59 | 1608 (1111–1915) | 273 (174–346) |
| 60 | 1644 (1164–1841) | 260 (146–336) |

### Savaş (1/2)

| Yıl | Muharebe | Başlayan savaş | Süren savaş (yıl sonu) | Yıl içinde süren savaş | Yağma akını (medeniyet) | Tarihî hak savaşı |
|---|---|---|---|---|---|---|
| 1 | 22 (16,5–28,5) | 3 (1–5,5) | 1,5 (0–2,5) | 4,5 (2,5–7) | 1 (0–5,5) | 0 (0–1,5) |
| 2 | 19 (14,5–25) | 2 (1–3,5) | 1 (0–2) | 3,5 (2–5,5) | 1 (0–4,5) | 0 (0–1) |
| 3 | 20,5 (15,5–26,5) | 3 (1,5–5,5) | 2 (0,5–3) | 4 (2,5–6) | 2 (0–3) | 0 (0–1) |
| 4 | 21 (15–27,5) | 2 (1–5,5) | 2 (0,5–3,5) | 4,5 (2,5–7) | 1 (0–5) | 0 (0–1) |
| 5 | 22,5 (16,5–28,5) | 2 (1–5) | 1 (1–2,5) | 4 (3–7) | 2,5 (0–4,5) | 0 (0–1) |
| 6 | 20,5 (12–26) | 2 (1–4,5) | 1 (0–3) | 4 (2–6) | 0 (0–3) | 0 (0–1) |
| 7 | 21,5 (14,5–26,5) | 3 (2–4) | 1 (0–2,5) | 4 (3–6) | 2 (0–5) | 0 (0–1) |
| 8 | 23,5 (12,5–29) | 2 (1–5) | 1 (0–2) | 3,5 (2–5,5) | 1,5 (0–4,5) | 0 (0–1) |
| 9 | 19 (15,5–25,5) | 2,5 (1–4) | 1,5 (0–3,5) | 3,5 (1–5,5) | 1 (0–2,5) | 0 (0–1) |
| 10 | 20,5 (14,5–26) | 3 (1–4,5) | 1 (1–2) | 4 (3–6,5) | 1 (0–2) | 0 (0–1) |
| 11 | 19,5 (15–25,5) | 3 (0,5–4,5) | 1,5 (0–3) | 4,5 (2–6) | 1 (0–3) | 0 (0–1) |
| 12 | 19 (12,5–26) | 3 (0–4,5) | 2 (0–3) | 4 (2–5,5) | 2 (0–3) | 0 (0–1) |
| 13 | 22 (15–28) | 3 (0,5–5) | 1 (0–2,5) | 5 (2,5–6,5) | 2 (0–3,5) | 0 (0–1) |
| 14 | 20 (16–26,5) | 3 (1–4) | 1 (0–2,5) | 3,5 (2,5–5) | 1,5 (0–3,5) | 0 (0–1) |
| 15 | 23 (17–26) | 3 (1,5–5) | 1 (0–2,5) | 4,5 (1,5–7) | 1,5 (0–4) | 0 (0–1) |
| 16 | 23 (17–29) | 2,5 (0,5–5) | 1 (0–4) | 4 (2–6) | 2 (0–3,5) | 0 (0–1) |
| 17 | 19 (15–29) | 2,5 (1–4) | 1 (0–4) | 4 (2–6,5) | 1 (0–4) | 1 (0–2) |
| 18 | 22,5 (14–26,5) | 2,5 (1–5) | 2 (0–3,5) | 5 (1,5–6) | 1 (0–3,5) | 0 (0–1) |
| 19 | 20 (17–28,5) | 2 (1–4,5) | 2 (0,5–2,5) | 4 (2–7) | 1,5 (0–4) | 0 (0–1) |
| 20 | 20 (13,5–25) | 3 (1–5) | 1 (1–3,5) | 4 (1,5–7) | 1 (0–3) | 0 (0–1) |
| 21 | 22 (15,5–27,5) | 2 (0–3,5) | 1 (0–2) | 3 (2–5,5) | 1,5 (0–4) | 0 (0–1) |
| 22 | 20 (16–25,5) | 3 (1–5) | 1 (0–3,5) | 4 (2–6) | 1 (0–2,5) | 0 (0–2) |
| 23 | 18,5 (14,5–24,5) | 3 (0,5–4,5) | 2 (0–3) | 4 (2–7,5) | 1,5 (0–3,5) | 0 (0–0,5) |
| 24 | 20,5 (14,5–24,5) | 2 (1–3,5) | 1 (0–2) | 4 (2–5) | 1 (0–4,5) | 0 (0–1) |
| 25 | 20,5 (12–23) | 2,5 (1–4) | 1 (0–2,5) | 4 (2–6) | 1 (0–3) | 0 (0–1) |
| 26 | 19 (11–25) | 2,5 (0,5–4) | 1,5 (0–3) | 3,5 (1,5–6) | 0 (0–3) | 0 (0–1) |
| 27 | 19,5 (13–24,5) | 2 (1–5) | 1 (0–3) | 4,5 (1,5–6) | 0 (0–4,5) | 0 (0–1) |
| 28 | 20,5 (13,5–27,5) | 2 (0,5–4) | 1 (0–2,5) | 3 (2–6,5) | 2 (0–4) | 0 (0–1) |
| 29 | 20 (16–24) | 2 (0,5–4) | 1,5 (0–2,5) | 3 (2–6) | 1 (0–4) | 0 (0–1) |
| 30 | 18,5 (14–26,5) | 1 (0–5,5) | 1 (0–3) | 3,5 (1–6,5) | 1 (0–4) | 0 (0–1) |
| 31 | 21,5 (11,5–25,5) | 3 (2–4) | 1 (0–3,5) | 4 (2,5–7) | 1 (0–3) | 0 (0–1) |
| 32 | 17,5 (11–27,5) | 2 (1–5) | 1,5 (0,5–3,5) | 3 (1,5–7) | 0,5 (0–3) | 0 (0–1) |
| 33 | 18,5 (11–23) | 2 (1–4) | 1,5 (0–2,5) | 4 (2,5–6) | 1 (0–3,5) | 0 (0–1) |
| 34 | 19,5 (12,5–28) | 2 (1–3) | 1 (0,5–3) | 3 (2–5,5) | 2 (0–4) | 0 (0–0,5) |
| 35 | 19,5 (17,5–31,5) | 2 (1–4,5) | 1 (0–3) | 4,5 (2,5–6) | 0,5 (0–4,5) | 0 (0–1) |
| 36 | 19,5 (16,5–31) | 2 (0,5–5) | 1 (0–3) | 3 (2–5,5) | 1,5 (0–4) | 0 (0–1) |
| 37 | 20 (17–30,5) | 2,5 (1–4,5) | 1 (0–2) | 3,5 (2–5,5) | 1,5 (0–4,5) | 0 (0–1) |
| 38 | 19 (11,5–28) | 2 (1–4,5) | 1 (0–2) | 3 (1–5) | 1,5 (0–4) | 0 (0–1) |
| 39 | 20 (14–31) | 2 (0,5–3,5) | 1 (0–2) | 3 (1,5–5,5) | 1 (0–3) | 0 (0–1,5) |
| 40 | 21 (13,5–28,5) | 3 (0,5–5) | 1 (0–2,5) | 4 (1,5–5,5) | 2 (0–3) | 0 (0–1) |
| 41 | 21 (15,5–26) | 2 (0,5–3,5) | 1 (0–2) | 3 (2–5,5) | 2 (0–3,5) | 0 (0–1) |
| 42 | 21 (13,5–28,5) | 3 (1–5,5) | 1 (0–2,5) | 4 (2,5–6,5) | 1 (0–2,5) | 0 (0–1) |
| 43 | 19 (15–26,5) | 2 (0–3) | 1 (0–2,5) | 3,5 (1–5) | 1 (0–4) | 0 (0–0,5) |
| 44 | 22 (9,5–29) | 2 (1–4) | 1 (1–2,5) | 3 (1,5–5) | 2 (0–3) | 0 (0–1) |
| 45 | 17 (15–24,5) | 2 (1–4,5) | 1,5 (0–3) | 4 (2–6) | 0,5 (0–2) | 0 (0–0,5) |
| 46 | 18 (13,5–24,5) | 2 (1–4,5) | 2 (1–3) | 3,5 (2–6) | 1 (0–3,5) | 0 (0–1) |
| 47 | 20 (13–25) | 2 (1–4) | 1 (0–2) | 4 (3–5,5) | 1,5 (0–4,5) | 0 (0–1) |
| 48 | 20,5 (12,5–27,5) | 2 (1–4,5) | 1 (0–3,5) | 3,5 (2–5,5) | 2 (0–4,5) | 0 (0–1) |
| 49 | 21,5 (13–27) | 2 (1–4) | 2 (0–3) | 3,5 (2–5) | 0,5 (0–4) | 0 (0–1) |
| 50 | 19 (10–24) | 2,5 (1–5) | 2 (0,5–4) | 4 (2,5–6) | 1 (0–4) | 0,5 (0–1,5) |
| 51 | 20 (14–29) | 2 (0,5–3) | 1 (0–2) | 4 (2–6) | 1,5 (0–4) | 0 (0–1) |
| 52 | 20 (15–29) | 2 (1–4) | 1 (0–3) | 3 (2–5) | 1 (0–3) | 0 (0–0,5) |
| 53 | 21,5 (11,5–29,5) | 2 (1,5–3,5) | 1 (0–2) | 3 (2–5,5) | 1 (0–3) | 0 (0–1) |
| 54 | 22,5 (13,5–29) | 1,5 (1–3,5) | 1 (0–2,5) | 3 (2–5) | 1 (0–5) | 0 (0–0,5) |
| 55 | 20 (13–31) | 2,5 (1–4) | 1 (0–2,5) | 3,5 (1,5–6) | 1 (0–4,5) | 0 (0–1) |
| 56 | 20,5 (12–29,5) | 2,5 (1–5) | 1 (0–3,5) | 3,5 (1–6) | 1 (0–5) | 0 (0–0,5) |
| 57 | 22 (12–27,5) | 3 (1–4,5) | 2 (0–3) | 4 (3–6,5) | 1 (0–4,5) | 0 (0–1) |
| 58 | 25 (12–31,5) | 2 (0–4) | 1 (0–2) | 3,5 (1–6) | 0,5 (0–4) | 0 (0–1) |
| 59 | 18 (14,5–27,5) | 2 (1–5) | 2 (0–3) | 3,5 (2–6) | 0,5 (0–3,5) | 0 (0–1) |
| 60 | 21,5 (13,5–27) | 2 (1–4) | 1 (0–3) | 4 (3–6) | 0,5 (0–3,5) | 0 (0–1) |

### Savaş (2/2)

| Yıl | Pakt gereği savaş | Kutsal Sefer çağrısı | İhanet (pakt çiğnendi) | Savunma paktı (yıl sonu) |
|---|---|---|---|---|
| 1 | 0 (0–1) | 0 | 0 | 0 (0–1) |
| 2 | 0 | 0 | 0 | 0,5 (0–1) |
| 3 | 0 (0–0,5) | 0 (0–1) | 0 | 0 (0–1) |
| 4 | 0 (0–1) | 0 (0–0,5) | 0 | 1 (0–1) |
| 5 | 0 (0–0,5) | 0 (0–0,5) | 0 | 1 (0–1) |
| 6 | 0 | 0 | 0 | 1 (0–1,5) |
| 7 | 0 (0–1) | 0 (0–1) | 0 | 1 (0–2) |
| 8 | 0 (0–0,5) | 0 | 0 | 1 (0–1) |
| 9 | 0 | 0 (0–1) | 0 | 1 (0–2) |
| 10 | 0 (0–1) | 0 | 0 | 1 (0–1,5) |
| 11 | 0 (0–1) | 0 (0–1) | 0 | 1 (0–1,5) |
| 12 | 0 (0–0,5) | 0 (0–0,5) | 0 (0–0,5) | 1 (0–2) |
| 13 | 0 (0–1,5) | 0 | 0 | 0,5 (0–2) |
| 14 | 0 (0–1) | 0 | 0 | 1 (0–1,5) |
| 15 | 0 (0–1) | 0 (0–0,5) | 0 | 1 (0–1,5) |
| 16 | 0 (0–1,5) | 0 | 0 | 1 (0–1,5) |
| 17 | 0 (0–1) | 0 (0–1) | 0 | 1 (0–2) |
| 18 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 19 | 0 (0–1) | 0 | 0 | 0,5 (0–2) |
| 20 | 0 (0–1) | 0 (0–0,5) | 0 | 0,5 (0–2) |
| 21 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 22 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 23 | 0 (0–0,5) | 0 (0–1) | 0 | 0,5 (0–2) |
| 24 | 0 (0–1) | 0 | 0 | 0 (0–1) |
| 25 | 0 | 0 (0–1) | 0 | 0 (0–1,5) |
| 26 | 0 (0–1) | 0 | 0 | 0 (0–1,5) |
| 27 | 0 (0–1) | 0 | 0 | 0 (0–1) |
| 28 | 0 (0–0,5) | 0 | 0 | 0,5 (0–1) |
| 29 | 0 (0–0,5) | 0 (0–1) | 0 (0–0,5) | 0 (0–1) |
| 30 | 0 | 0 | 0 | 0 (0–1) |
| 31 | 0 (0–0,5) | 0 (0–0,5) | 0 | 0 (0–1,5) |
| 32 | 0 (0–0,5) | 0 (0–1) | 0 | 0 (0–1,5) |
| 33 | 0 (0–1) | 0 | 0 (0–0,5) | 0 (0–1) |
| 34 | 0 | 0 (0–1) | 0 | 1 (0–2) |
| 35 | 0 (0–0,5) | 0 | 0 (0–0,5) | 0,5 (0–1,5) |
| 36 | 0 (0–1) | 0 | 0 | 0,5 (0–1,5) |
| 37 | 0 (0–1) | 0 (0–1) | 0 | 0,5 (0–1,5) |
| 38 | 0 (0–0,5) | 0 (0–1) | 0 | 1 (0–1,5) |
| 39 | 0 (0–0,5) | 0 | 0 (0–0,5) | 0,5 (0–1) |
| 40 | 0 | 0 | 0 | 0,5 (0–1) |
| 41 | 0 | 0 (0–1) | 0 | 1 (0–1) |
| 42 | 0 (0–0,5) | 0 | 0 | 1 (0–1) |
| 43 | 0 | 0 | 0 | 1 (0–1,5) |
| 44 | 0 (0–1) | 0 (0–1) | 0 | 1 (0–1) |
| 45 | 0 (0–1) | 0 (0–0,5) | 0 | 1 (0–1) |
| 46 | 0 (0–1) | 0 | 0 | 1 (0–1) |
| 47 | 0 (0–0,5) | 0 | 0 | 1 (0–1) |
| 48 | 0 | 0 (0–1) | 0 | 1 (0–1) |
| 49 | 0 (0–0,5) | 0 | 0 | 1 (0–1) |
| 50 | 0 (0–1) | 0 (0–1) | 0 | 1 (0–1) |
| 51 | 0 | 0 (0–1) | 0 | 1 (0–1) |
| 52 | 0 | 0 (0–1) | 0 | 0,5 (0–1) |
| 53 | 0 | 0 | 0 | 1 (0–1) |
| 54 | 0 (0–1) | 0 | 0 | 1 (0–1) |
| 55 | 0 (0–0,5) | 0 | 0 | 0,5 (0–1) |
| 56 | 0 (0–0,5) | 0 (0–0,5) | 0 | 1 (0–2) |
| 57 | 0 (0–1) | 0 (0–1) | 0 | 1 (0–1,5) |
| 58 | 0 (0–1) | 0 | 0 | 1 (0–1,5) |
| 59 | 0 (0–1,5) | 0 | 0 | 1 (0–1,5) |
| 60 | 0 (0–1) | 0 | 0 | 1 (0–1,5) |

### Canavarlar (1/2)

| Yıl | Yaşayan kamp (yıl sonu) | Yaşayan kamp (yıl ort.) | Doğan kamp | Temizlenen kamp | Canavar baskını | Yaşayan trol ini (yıl sonu) |
|---|---|---|---|---|---|---|
| 1 | 8 (7,5–9,5) | 8,29 (7,59–9,54) | 7,5 (5–10) | 8,5 (4,5–11) | 4 (2–6) | 2 (0,5–3) |
| 2 | 8 (7–9) | 8,19 (7,89–8,88) | 7 (3,5–11) | 8 (4–10,5) | 3 (2–5) | 2 (1–3) |
| 3 | 8 (7–9) | 8,15 (7,56–9,09) | 7,5 (3–10,5) | 7,5 (4–10,5) | 4 (2–6) | 2 (0,5–3) |
| 4 | 9 (7,5–9) | 8,49 (7,51–9,36) | 8 (4–11) | 8 (2,5–10) | 4 (1,5–6) | 2 (1–3) |
| 5 | 8 (7,5–8,5) | 8,56 (7,76–8,96) | 7,5 (3–11) | 8 (4–10,5) | 4 (1,5–6,5) | 1,5 (0,5–2,5) |
| 6 | 8 (7,5–9) | 8,05 (7,65–8,69) | 8 (3–10) | 8 (2–10) | 3 (2–5) | 2 (0,5–3) |
| 7 | 8 (7–9) | 8,29 (7,8–9,05) | 8 (2–9) | 7,5 (2,5–11) | 4 (2,5–6) | 2 (1–3) |
| 8 | 8 (7,5–9,5) | 8,25 (7,83–9) | 8,5 (3–10,5) | 8 (3–11) | 4 (2–6,5) | 2 (1–3) |
| 9 | 8 (7,5–9,5) | 8,05 (7,6–9,46) | 8,5 (3,5–11) | 9 (3,5–11) | 4 (3–6,5) | 2 (1–3) |
| 10 | 9 (7,5–10) | 8,11 (7,66–9,46) | 8 (3,5–11,5) | 8 (4–10,5) | 3 (1–5,5) | 2 (1–3) |
| 11 | 8 (7,5–9,5) | 8,11 (7,59–9,6) | 6,5 (2,5–10) | 7 (3–11) | 3 (2–6) | 3 (1–3) |
| 12 | 9 (7,5–10) | 8,13 (7,33–9,68) | 8 (2,5–10,5) | 8 (2–9) | 3,5 (1,5–5) | 2 (1–3) |
| 13 | 8,5 (7–10) | 8,69 (7,68–9,89) | 7,5 (3–10,5) | 7,5 (3,5–11) | 3,5 (2,5–5) | 2 (1–3) |
| 14 | 8,5 (8–10,5) | 8,26 (7,7–9,56) | 7,5 (3–11,5) | 6 (3–11,5) | 3,5 (1,5–6,5) | 2 (1–3) |
| 15 | 9 (7–10) | 8,45 (7,75–9,66) | 7,5 (3,5–10,5) | 7,5 (3,5–12) | 4 (1,5–6,5) | 2 (0,5–3) |
| 16 | 9 (8–9,5) | 8,8 (8,11–9,54) | 8,5 (4–11,5) | 8 (3–11) | 4,5 (2–7) | 2 (1–3) |
| 17 | 9 (8–11) | 9,29 (8,15–10,2) | 7 (4–10,5) | 7 (3,5–10) | 4 (2,5–6) | 2 (2–3) |
| 18 | 9 (7,5–10,5) | 9,13 (7,88–10,4) | 8,5 (2,5–11) | 8,5 (2,5–11) | 4 (2–6,5) | 2 (1–3) |
| 19 | 9,5 (8–10,5) | 9,09 (7,88–10,5) | 8 (2,5–10,5) | 8 (2,5–10) | 5 (3–6) | 2 (1–3) |
| 20 | 8 (7,5–10) | 8,79 (7,79–10) | 7 (3,5–10,5) | 8 (4,5–11,5) | 3,5 (2–6) | 3 (2–3) |
| 21 | 8,5 (8–10) | 8,64 (7,7–9,94) | 8,5 (3–10,5) | 9 (2,5–10) | 4 (2–6,5) | 3 (2–3) |
| 22 | 8 (7–10) | 8,28 (7,73–10,9) | 7 (3–9,5) | 8 (3–9,5) | 3,5 (2,5–5) | 2 (1,5–3) |
| 23 | 8 (7,5–10) | 8,26 (7,46–10,3) | 7 (2–11) | 6 (3–11) | 4 (2,5–6) | 3 (1–3) |
| 24 | 9 (8–9,5) | 8,58 (7,66–9,61) | 6,5 (2–9,5) | 6 (2,5–9) | 4 (2–7,5) | 3 (2–3) |
| 25 | 9 (8–10) | 8,63 (7,88–9,69) | 7 (3–9) | 7 (2,5–9) | 4,5 (2–6,5) | 2,5 (2–3) |
| 26 | 8 (8–10) | 8,36 (7,85–9,35) | 6,5 (1,5–9) | 7 (2,5–9) | 3 (1,5–5,5) | 3 (2–3) |
| 27 | 8,5 (8–11) | 8,64 (7,83–10,2) | 6 (3–9) | 6,5 (1,5–8,5) | 4,5 (2–5,5) | 3 (2–3) |
| 28 | 9 (7–10,5) | 8,58 (7,85–10,6) | 7,5 (2,5–9,5) | 6,5 (2–10) | 4 (2,5–7) | 2 (2–3) |
| 29 | 8,5 (7–9,5) | 8,69 (7,75–9,93) | 6 (4,5–9) | 7 (5–9,5) | 5 (3,5–6) | 3 (2–3) |
| 30 | 8 (8–9,5) | 8,26 (7,69–9,16) | 6 (3,5–9) | 6,5 (3–9) | 3 (2–6) | 2 (1–3) |
| 31 | 8 (7,5–9) | 8,48 (7,56–9,6) | 7 (3–11) | 7,5 (3,5–10) | 4 (2–6,5) | 3 (1,5–3) |
| 32 | 8 (7,5–9,5) | 8,25 (7,73–9,25) | 7,5 (3–11) | 7 (3–11) | 3,5 (2–6,5) | 2 (2–3) |
| 33 | 9 (7–10) | 8,86 (7,85–9,79) | 5,5 (3–8,5) | 5 (2–8,5) | 3,5 (1–6) | 2 (1,5–3) |
| 34 | 9 (8–10) | 8,69 (7,68–9,86) | 5,5 (4–10,5) | 6 (3,5–9,5) | 3 (3–6,5) | 3 (2,5–3) |
| 35 | 8,5 (7,5–9) | 8,85 (7,76–9,49) | 5,5 (2–11) | 5,5 (3,5–10,5) | 4,5 (2,5–6,5) | 2,5 (2–3) |
| 36 | 9 (8–10) | 8,75 (8,03–9,99) | 6 (2–9,5) | 5,5 (2–8) | 5,5 (2,5–7) | 3 (2–3) |
| 37 | 8,5 (8–10) | 8,86 (8,09–9,66) | 6,5 (1,5–11) | 7 (2,5–11) | 4 (2,5–6) | 3 (2–3) |
| 38 | 8 (8–10) | 9,05 (7,9–9,9) | 5,5 (1,5–9,5) | 6 (2–10,5) | 3,5 (2–6) | 3 (1,5–3) |
| 39 | 8 (8–9,5) | 8,8 (7,96–9,6) | 5,5 (2,5–9) | 4,5 (3–10,5) | 4,5 (2–7) | 3 (2–3) |
| 40 | 8,5 (8–9) | 8,88 (7,85–9,54) | 6 (1,5–10,5) | 6 (2,5–10,5) | 5 (2–6,5) | 3 (2,5–3) |
| 41 | 9 (8–9,5) | 8,54 (7,85–9,6) | 6 (3–10,5) | 6 (2,5–10) | 4 (2–5,5) | 3 (1,5–3) |
| 42 | 8,5 (7–10) | 8,56 (7,9–9,29) | 5,5 (2–9,5) | 6 (2–10,5) | 4 (3–6) | 3 (1–3) |
| 43 | 9 (7,5–10) | 8,48 (7,63–9,63) | 7 (2,5–10,5) | 6 (2,5–10) | 3,5 (2–5) | 3 (1,5–3) |
| 44 | 9 (8–10) | 8,73 (7,83–9,71) | 5,5 (3,5–10,5) | 5,5 (3–10,5) | 4,5 (2–7,5) | 3 (2–3) |
| 45 | 9 (8–10) | 8,78 (7,74–9,55) | 6 (2,5–9) | 6 (2–9,5) | 4 (2–5,5) | 3 (2–3) |
| 46 | 8 (8–10,5) | 8,33 (7,81–9,5) | 6,5 (3–8,5) | 6 (3–9) | 3 (1,5–6) | 3 |
| 47 | 8 (8–10) | 8,24 (7,78–8,9) | 5,5 (3–9,5) | 7 (3,5–9) | 4 (2–6) | 3 (1,5–3) |
| 48 | 8 (7,5–9) | 8,33 (7,83–9,28) | 6 (3–9,5) | 6,5 (2–9,5) | 4 (2–7) | 3 (2–3) |
| 49 | 9 (8–9,5) | 8,55 (7,78–8,95) | 8 (1,5–10,5) | 6,5 (2,5–10) | 4 (2–6,5) | 3 (1,5–3) |
| 50 | 8,5 (7–10,5) | 8,34 (7,81–9,93) | 6 (2–9,5) | 6,5 (1,5–10,5) | 4 (1,5–6) | 3 (1,5–3) |
| 51 | 8 (8–9,5) | 8,2 (7,66–9,9) | 6 (2–11) | 6 (2–10) | 4 (2,5–6,5) | 3 (2–3) |
| 52 | 9 (8–10) | 8,43 (7,66–9,4) | 5 (2–11,5) | 5,5 (2–11) | 4 (2,5–7) | 3 |
| 53 | 8,5 (8–9) | 8,59 (7,75–9,7) | 5,5 (1,5–10,5) | 5 (1,5–11) | 5 (2–8,5) | 3 (2–3) |
| 54 | 8,5 (8–10) | 8,8 (7,91–9,98) | 5,5 (2,5–12) | 5,5 (1,5–12,5) | 3,5 (2–6,5) | 3 (2–3) |
| 55 | 9 (7–10) | 8,61 (7,61–10,1) | 5,5 (0,5–10,5) | 5 (1,5–11) | 4 (2–6) | 3 (2–3) |
| 56 | 8,5 (8–10) | 8,33 (7,81–9,61) | 7 (2–10,5) | 7,5 (2–9,5) | 5 (2–7) | 3 (2–3) |
| 57 | 8 (7,5–9,5) | 8,44 (7,64–9,39) | 6,5 (2–8) | 6,5 (2–9) | 3,5 (1,5–6) | 3 (2–3) |
| 58 | 9 (7–10,5) | 8,4 (7,73–9,78) | 6,5 (1,5–10,5) | 5 (2–10) | 4 (2–7) | 3 (2–3) |
| 59 | 9 (8–11) | 8,93 (7,76–10,4) | 6,5 (2,5–9) | 6 (2–8,5) | 4 (2–5,5) | 3 (2–3) |
| 60 | 9 (8–10) | 8,75 (7,8–10,3) | 6 (1–10,5) | 6 (1–10) | 4 (2,5–6,5) | 3 (2–3) |

### Canavarlar (2/2)

| Yıl | Yaşayan ejderha (yıl sonu) | Ejderha akını | Kriz (anlatıcı) | Rahatlama dönemi (anlatıcı) |
|---|---|---|---|---|
| 1 | 0 | 0 | 2,5 (1–4) | 0 (0–0,5) |
| 2 | 0 | 0 | 3 (0,5–6,5) | 0 (0–1) |
| 3 | 0 | 0 | 5 (1,5–7) | 0 |
| 4 | 0 | 0 | 3 (0–5) | 0 (0–1) |
| 5 | 0 | 0 | 3 (1,5–5,5) | 0 (0–1) |
| 6 | 0 | 0 | 2 (1–5) | 0 (0–1) |
| 7 | 0 | 0 | 2 (1–5) | 0 (0–1) |
| 8 | 0 | 0 | 3 (0,5–5) | 0 (0–1) |
| 9 | 0 | 0 | 3 (1,5–4) | 0 (0–1) |
| 10 | 0 | 0 | 3 (2–6,5) | 0 (0–0,5) |
| 11 | 0 | 0 | 3 (1,5–5) | 0 (0–1) |
| 12 | 0 | 0 | 3 (1–3,5) | 0 (0–1) |
| 13 | 0 | 0 | 2 (1–5,5) | 0 (0–1) |
| 14 | 0 (0–1) | 0 (0–1,5) | 3 (2–5,5) | 0 (0–0,5) |
| 15 | 0 (0–1) | 0 (0–1,5) | 2 (1–4,5) | 0 (0–1) |
| 16 | 1 (0–1) | 1 (0–2) | 3 (1,5–5,5) | 0 (0–1) |
| 17 | 1 (0–1) | 1 (0–2) | 2 (1–5) | 0 (0–1) |
| 18 | 1 (0–1) | 1 (0–1) | 2 (0,5–4) | 1 (0–1) |
| 19 | 0 (0–1) | 0,5 (0–2) | 3 (1–5) | 0 (0–1) |
| 20 | 0 (0–1) | 0 (0–1) | 2,5 (1,5–4,5) | 0 (0–1) |
| 21 | 0 (0–1) | 0 (0–2) | 4 (2–5) | 0 (0–1) |
| 22 | 0 (0–1) | 0 (0–1,5) | 3 (2–4,5) | 0 (0–1) |
| 23 | 0 (0–1) | 0 (0–2) | 3 (1,5–4,5) | 0 (0–1) |
| 24 | 0 (0–1) | 0 (0–2) | 3 (1,5–5,5) | 0 (0–1) |
| 25 | 0 (0–1) | 0 (0–1) | 3 (1,5–4,5) | 0 (0–1) |
| 26 | 0 (0–1) | 0 (0–2) | 3 (1,5–5) | 0 |
| 27 | 0 (0–1) | 0 (0–1,5) | 2,5 (1–4) | 0 (0–1) |
| 28 | 0 (0–1) | 0 (0–1,5) | 2 (0,5–3,5) | 0 (0–1) |
| 29 | 0 (0–1) | 0 (0–2) | 2 (0,5–5,5) | 0 (0–1) |
| 30 | 0 (0–1) | 0 (0–1) | 3 (1–5) | 0 (0–1) |
| 31 | 0 (0–1) | 0 (0–1) | 2 (1–4,5) | 0 (0–0,5) |
| 32 | 0 (0–1) | 0 (0–2) | 3,5 (2–5) | 0 (0–1) |
| 33 | 0 (0–1) | 0 (0–1,5) | 2,5 (1–4,5) | 0 (0–1) |
| 34 | 0 (0–1) | 0 (0–2) | 3 (1–4) | 0 (0–1) |
| 35 | 0 (0–1) | 0 (0–1) | 3 (1–4) | 0 (0–1) |
| 36 | 0 (0–1) | 0 (0–2) | 3 (1–6) | 0 (0–1) |
| 37 | 0 (0–1) | 0 (0–1) | 3 (1–4) | 0 (0–1) |
| 38 | 0 (0–1) | 0 (0–1,5) | 2,5 (0,5–6) | 0 (0–1) |
| 39 | 0 (0–1) | 0 (0–1,5) | 3 (1–4,5) | 0 (0–1) |
| 40 | 0 (0–1) | 0 (0–2) | 4 (1–6,5) | 0 (0–1) |
| 41 | 0 (0–1) | 0 (0–1) | 3 (0,5–5,5) | 0 (0–1) |
| 42 | 0 (0–1) | 0 (0–1,5) | 2 (1–4) | 0 (0–1) |
| 43 | 0 (0–1) | 0 (0–1) | 3,5 (1,5–5,5) | 0 (0–1) |
| 44 | 0 (0–1) | 0 (0–1,5) | 2 (1–4) | 0 (0–1) |
| 45 | 0 (0–1) | 0 (0–1,5) | 2 (1–4,5) | 0 (0–1) |
| 46 | 0 (0–1) | 0 (0–1) | 3 (1–4) | 0 (0–1) |
| 47 | 0 (0–1) | 0 (0–1) | 2,5 (1–5,5) | 0 (0–1) |
| 48 | 0 (0–1) | 0 (0–1) | 3 (1–4) | 0 (0–1) |
| 49 | 0 (0–1) | 0 (0–1,5) | 3 (1,5–4,5) | 0 (0–0,5) |
| 50 | 0 (0–1) | 0 (0–0,5) | 3 (2–4,5) | 0 (0–1) |
| 51 | 0 (0–1) | 0 (0–1) | 3,5 (2–5) | 0 (0–1) |
| 52 | 0 (0–1) | 0 (0–1,5) | 4 (1–5,5) | 0 (0–1) |
| 53 | 0 (0–1) | 0 (0–1) | 2 (1–4) | 0 (0–1) |
| 54 | 0 (0–1) | 0 (0–1,5) | 3 (1–6) | 0 (0–1) |
| 55 | 0 (0–1) | 0 (0–0,5) | 2,5 (1–5) | 0 (0–1) |
| 56 | 0 (0–1) | 0 (0–2) | 2 (1–6) | 0 (0–1) |
| 57 | 0 (0–1) | 0 (0–1) | 3 (1,5–5,5) | 0 (0–0,5) |
| 58 | 0 (0–1) | 0 (0–0,5) | 3 (1–4) | 0 (0–1) |
| 59 | 0 (0–0,5) | 0 (0–1) | 2 (0,5–5) | 0 (0–1) |
| 60 | 0 (0–0,5) | 0 (0–0,5) | 2,5 (1–5) | 0 (0–1) |

### Kahramanlar (1/2)

| Yıl | Doğan kahraman | Ölen kahraman | Emekli olan kahraman | Diyarı terk eden kahraman | Ölümden dönen kahraman | Efsane olan kahraman |
|---|---|---|---|---|---|---|
| 1 | 11 (6–14) | 4,5 (1,5–11,5) | 2 (0–4) | 4 (0,5–8) | 0 | 0 |
| 2 | 9,5 (6–14) | 2,5 (1–5,5) | 1,5 (0–3) | 2,5 (0,5–12) | 0 | 0 |
| 3 | 10 (5–16) | 3,5 (1,5–8) | 1,5 (0–2,5) | 2,5 (1–9,5) | 0 | 0 (0–1) |
| 4 | 10 (7,5–16) | 4 (0,5–9) | 1 (0–2,5) | 3 (1–12,5) | 0 | 0 |
| 5 | 10,5 (6–16) | 4,5 (0–8,5) | 2 (0–4) | 3,5 (2–9) | 0 | 0 |
| 6 | 10 (6–15,5) | 2,5 (0,5–9,5) | 1 (0,5–2,5) | 3 (0,5–9,5) | 0 | 0 |
| 7 | 10,5 (4,5–15) | 2,5 (0,5–9,5) | 1,5 (0–3) | 2,5 (0–8) | 0 | 0 (0–0,5) |
| 8 | 9 (5–16) | 4 (1–8) | 1,5 (0–4,5) | 2,5 (1–11,5) | 0 | 0 |
| 9 | 10 (6,5–14,5) | 3 (0–6) | 1,5 (1–3) | 4 (1–13) | 0 | 0 (0–0,5) |
| 10 | 8,5 (5–14,5) | 3 (0,5–11) | 1,5 (0,5–4) | 3 (2–11) | 0 | 0 |
| 11 | 10 (6,5–15) | 4 (1–9) | 1 (0–3) | 1,5 (0–10,5) | 0 | 0 |
| 12 | 10 (7,5–19,5) | 2,5 (1–9) | 2 (0–4,5) | 3 (1–10) | 0 | 0 |
| 13 | 10,5 (6–15) | 4 (2–8,5) | 2 (0,5–4,5) | 2,5 (0–9,5) | 0 | 0 |
| 14 | 10 (6,5–17,5) | 5,5 (3–9) | 2 (0–4) | 2 (0,5–10) | 0 | 0 (0–1) |
| 15 | 10 (6–16,5) | 4 (2–7,5) | 2 (0,5–2,5) | 3 (1,5–7,5) | 0 | 0 (0–0,5) |
| 16 | 11 (6,5–16) | 7 (1–9,5) | 1 (0–3) | 3,5 (1–8) | 0 | 0 |
| 17 | 9,5 (6–16) | 5 (2–9,5) | 1 (0–4) | 2,5 (0–9,5) | 0 | 0 (0–1) |
| 18 | 11 (6–17,5) | 5 (1–14) | 2 (0–3) | 5,5 (1–11) | 0 | 0 (0–1) |
| 19 | 13 (8–17) | 5 (2,5–7,5) | 1,5 (0,5–3,5) | 3 (1–9) | 0 | 0 (0–1) |
| 20 | 10,5 (5,5–16,5) | 2,5 (0–10) | 1,5 (0–3) | 5 (1–10) | 0 | 0 |
| 21 | 8 (5,5–15,5) | 5 (1,5–12) | 3 (1–4,5) | 4 (1–9) | 0 | 0 |
| 22 | 11 (5,5–18) | 4,5 (2–7,5) | 2 (0,5–5,5) | 3 (0,5–6,5) | 0 | 0 |
| 23 | 11,5 (7–16) | 5 (2–12,5) | 2 (0,5–5) | 2,5 (1–13) | 0 | 0 |
| 24 | 11,5 (7–19) | 5 (2–13) | 1 (1–3,5) | 3,5 (1–12) | 0 | 0 |
| 25 | 11 (7–16,5) | 3,5 (1,5–12,5) | 1 (0,5–4,5) | 3 (0,5–12,5) | 0 | 0 |
| 26 | 11 (6–19) | 5 (0,5–11) | 2 (0–4) | 5 (0,5–10,5) | 0 | 0 |
| 27 | 11 (6–17,5) | 4,5 (3–9,5) | 2 (1–3) | 4 (0–12) | 0 | 0 |
| 28 | 12 (8–17,5) | 4,5 (1–9) | 2 (1–4) | 4,5 (1–10,5) | 0 | 0 |
| 29 | 10,5 (4–15) | 4 (0–10) | 1,5 (0,5–5,5) | 3,5 (1–10) | 0 | 0 |
| 30 | 11 (7,5–15) | 5 (1,5–8,5) | 3 (1–5) | 3 (1–7) | 0 | 0 |
| 31 | 11,5 (7,5–17) | 4 (1,5–6,5) | 1 (0–3) | 3,5 (0,5–8) | 0 | 0 |
| 32 | 10,5 (8–16) | 4 (0,5–8,5) | 3 (1–5,5) | 4 (1,5–9) | 0 | 0 |
| 33 | 10 (4,5–14) | 4 (1,5–8,5) | 2 (0,5–4) | 3 (1,5–10,5) | 0 | 0 |
| 34 | 12 (6–16,5) | 4 (1–10,5) | 1 (0–3,5) | 3,5 (0–19,5) | 0 | 0 |
| 35 | 10 (6,5–14) | 5,5 (2–11,5) | 2 (0,5–4,5) | 3,5 (1,5–7,5) | 0 | 0 (0–1) |
| 36 | 12 (5–15,5) | 4 (2,5–10) | 2 (0–3) | 2,5 (0,5–7) | 0 | 0 |
| 37 | 11,5 (7,5–15) | 4 (1,5–11) | 2 (0,5–4) | 4,5 (0,5–11,5) | 0 | 0 |
| 38 | 11 (5,5–17,5) | 3,5 (0–9) | 2 (1–4,5) | 3,5 (0,5–13) | 0 | 0 |
| 39 | 10 (5,5–15) | 4 (1,5–9,5) | 2 (1–3,5) | 3,5 (0,5–11,5) | 0 | 0 |
| 40 | 10,5 (6–13,5) | 5 (1,5–9) | 2 (0–4,5) | 4 (1,5–6,5) | 0 | 0 |
| 41 | 11,5 (7–16,5) | 2 (0,5–5,5) | 2 (1–4) | 4,5 (1–10) | 0 | 0 |
| 42 | 11 (6–19) | 3,5 (2–10,5) | 2 (0–3,5) | 5,5 (2–11,5) | 0 | 0 (0–0,5) |
| 43 | 10,5 (6–16,5) | 5 (1,5–10) | 2 (0,5–4,5) | 5 (2–13) | 0 | 0 |
| 44 | 11 (6,5–17) | 4,5 (2–8) | 2 (0–4) | 3 (1–8,5) | 0 | 0 |
| 45 | 12 (6,5–17) | 4 (1–10,5) | 2 (1–5) | 5,5 (2–12) | 0 | 0 |
| 46 | 12 (8–16,5) | 4 (1–7) | 2 (0–4) | 3,5 (1,5–12,5) | 0 | 0 |
| 47 | 9,5 (7–15) | 5 (1,5–7,5) | 2 (0–5) | 5,5 (1,5–13) | 0 | 0 |
| 48 | 13 (8,5–18) | 7 (2–12,5) | 2 (0,5–5,5) | 3 (0,5–8) | 0 | 0 |
| 49 | 12,5 (5,5–17) | 3,5 (1–8) | 2,5 (0–3) | 7 (1–13) | 0 | 0 |
| 50 | 11 (6–16) | 3 (0–9) | 2 (0,5–4,5) | 6 (1–11,5) | 0 | 0 |
| 51 | 10 (7–17,5) | 4 (2–10) | 3 (1–4,5) | 4,5 (0,5–8,5) | 0 | 0 |
| 52 | 9,5 (7,5–17,5) | 5,5 (1–8) | 3 (0,5–4,5) | 3 (1–9) | 0 | 0 (0–0,5) |
| 53 | 9,5 (5,5–18,5) | 3,5 (1–8) | 2,5 (1–5,5) | 4 (2–10) | 0 | 0 |
| 54 | 11,5 (6–17,5) | 4 (1,5–8,5) | 3 (0–4) | 4,5 (0–10,5) | 0 | 0 |
| 55 | 11 (6–14) | 3 (1–10,5) | 1,5 (0,5–4,5) | 5,5 (1–11,5) | 0 | 0 |
| 56 | 10,5 (4–19) | 4,5 (1–10,5) | 2 (0–4) | 4,5 (1,5–13) | 0 | 0 (0–0,5) |
| 57 | 12,5 (6,5–17) | 4,5 (2,5–8,5) | 3 (1–4) | 4 (0–9) | 0 | 0 |
| 58 | 12,5 (7,5–17) | 4 (2–8,5) | 2 (1–4,5) | 5,5 (2,5–9,5) | 0 | 0 |
| 59 | 11 (8,5–19) | 4,5 (2,5–8,5) | 2 (0,5–4,5) | 8 (2–12) | 0 | 0 |
| 60 | 12 (7–17,5) | 5,5 (2–9,5) | 2,5 (0,5–4,5) | 7 (1,5–10) | 0 | 0 (0–0,5) |

### Kahramanlar (2/2)

| Yıl | Yaşayan kahraman (yıl sonu) | Doğuş seviyesi (ort.) | Ölüm seviyesi (ort.) | Yaşayan kahraman seviyesi (ort.) | En yüksek seviye (şimdiye dek) |
|---|---|---|---|---|---|
| 1 | 128 (114–180) | 1,55 (1,33–2,08) | 4 (3,54–5) | 4,5 (4,14–5,11) | 10 |
| 2 | 132 (111–180) | 1,64 (1,27–1,94) | 3,83 (2,2–5,2) | 4,59 (4,24–5,16) | 10 |
| 3 | 132 (108–184) | 1,55 (1,33–1,81) | 4,5 (3,67–7) | 4,59 (4,2–5,11) | 10 |
| 4 | 132 (112–180) | 1,68 (1,4–1,87) | 4,21 (3–5,7) | 4,58 (4,26–5,18) | 10 |
| 5 | 134 (113–176) | 1,61 (1,5–1,88) | 3,4 (2,87–4,84) | 4,59 (4,32–5,09) | 10 |
| 6 | 134 (112–181) | 1,58 (1,38–1,88) | 4,2 (2,3–6) | 4,66 (4,33–5,09) | 10 |
| 7 | 136 (113–184) | 1,69 (1,34–2) | 4,36 (3,13–5) | 4,66 (4,3–5,09) | 10 |
| 8 | 136 (112–180) | 1,65 (1,36–2) | 3,75 (3–6,25) | 4,67 (4,35–5,18) | 10 |
| 9 | 138 (117–179) | 1,57 (1,32–1,78) | 4,33 (3,3–6,4) | 4,74 (4,41–5,21) | 10 |
| 10 | 139 (114–176) | 1,55 (1,24–1,87) | 4,86 (3,2–7,7) | 4,78 (4,37–5,27) | 10 |
| 11 | 142 (120–176) | 1,68 (1,51–1,88) | 5 (3,33–6,73) | 4,78 (4,37–5,25) | 10 |
| 12 | 144 (122–179) | 1,6 (1,47–1,74) | 5 (4–6) | 4,7 (4,37–5,22) | 10 |
| 13 | 144 (122–182) | 1,58 (1,41–1,73) | 4,25 (2,6–5,61) | 4,66 (4,27–5,37) | 10 |
| 14 | 144 (122–184) | 1,58 (1,4–1,87) | 4,63 (3,33–6,67) | 4,68 (4,22–5,41) | 10 |
| 15 | 144 (122–184) | 1,6 (1,29–2) | 5 (4,13–6,73) | 4,64 (4,32–5,43) | 10 |
| 16 | 142 (120–183) | 1,5 (1,38–1,82) | 4,78 (3,92–5,76) | 4,65 (4,32–5,49) | 10 |
| 17 | 140 (122–184) | 1,65 (1,43–1,97) | 4,57 (2,75–6,24) | 4,67 (4,36–5,51) | 10 |
| 18 | 144 (121–180) | 1,55 (1,29–2,04) | 4,71 (3,76–6) | 4,76 (4,31–5,54) | 10 |
| 19 | 150 (118–184) | 1,62 (1,46–1,94) | 4,58 (3,55–6) | 4,66 (4,3–5,54) | 10 |
| 20 | 150 (118–182) | 1,67 (1,45–1,87) | 4,7 (3,68–8,2) | 4,7 (4,36–5,56) | 10 |
| 21 | 148 (112–184) | 1,61 (1,41–1,77) | 5 (3,18–7,2) | 4,74 (4,32–5,56) | 10 |
| 22 | 148 (114–186) | 1,65 (1,48–1,9) | 4,88 (3,75–6,68) | 4,7 (4,31–5,54) | 10 |
| 23 | 150 (113–182) | 1,68 (1,44–1,85) | 5,41 (4,51–7,75) | 4,62 (4,29–5,47) | 10 |
| 24 | 150 (116–190) | 1,68 (1,46–1,81) | 4,86 (4,04–8) | 4,68 (4,28–5,47) | 10 |
| 25 | 150 (118–188) | 1,79 (1,43–1,95) | 5 (3–6,6) | 4,64 (4,32–5,46) | 10 |
| 26 | 149 (118–185) | 1,6 (1,44–1,79) | 4,31 (3,56–5,6) | 4,61 (4,21–5,43) | 10 |
| 27 | 151 (116–184) | 1,62 (1,39–1,86) | 4 (3,29–6,6) | 4,7 (4,24–5,41) | 10 |
| 28 | 153 (114–185) | 1,55 (1,35–1,82) | 4,57 (3,06–7,17) | 4,69 (4,14–5,31) | 10 |
| 29 | 148 (116–181) | 1,54 (1,33–1,73) | 4,38 (2,69–6,17) | 4,58 (4,17–5,43) | 10 |
| 30 | 150 (122–183) | 1,65 (1,29–2,08) | 5 (3,85–6,12) | 4,56 (4,16–5,28) | 10 |
| 31 | 154 (124–184) | 1,6 (1,35–1,83) | 4,67 (3,2–5,46) | 4,6 (4,18–5,31) | 10 |
| 32 | 156 (126–185) | 1,65 (1,38–1,89) | 5,18 (3–6,76) | 4,58 (4,27–5,17) | 10 |
| 33 | 158 (128–185) | 1,56 (1,18–1,85) | 4,66 (2–6,75) | 4,63 (4,27–5,18) | 10 |
| 34 | 160 (130–182) | 1,65 (1,49–2,02) | 5,4 (4,07–8,3) | 4,69 (4,27–5,15) | 10 |
| 35 | 162 (130–182) | 1,72 (1,35–2,04) | 5,13 (3,17–6,5) | 4,66 (4,25–5,09) | 10 |
| 36 | 162 (132–182) | 1,62 (1,5–1,87) | 4,67 (3,55–5,75) | 4,65 (4,3–5,06) | 10 |
| 37 | 161 (134–184) | 1,63 (1,39–1,85) | 4,9 (4,29–6) | 4,72 (4,24–5,02) | 10 |
| 38 | 162 (135–185) | 1,81 (1,63–2) | 4,81 (3,17–6,21) | 4,69 (4,26–5,08) | 10 |
| 39 | 162 (136–182) | 1,68 (1,42–1,88) | 4,38 (2,67–6) | 4,69 (4,26–5,08) | 10 |
| 40 | 162 (134–184) | 1,7 (1,38–1,96) | 4,54 (3,33–5,69) | 4,73 (4,28–5,09) | 10 |
| 41 | 165 (136–180) | 1,57 (1,31–2) | 5,33 (4–7,35) | 4,68 (4,31–5,16) | 10 |
| 42 | 166 (136–186) | 1,66 (1,47–1,92) | 5,17 (4,23–7,63) | 4,69 (4,3–5,14) | 10 |
| 43 | 162 (130–188) | 1,7 (1,35–1,9) | 4,86 (3,3–7,2) | 4,7 (4,33–5,11) | 10 |
| 44 | 166 (128–190) | 1,64 (1,36–1,72) | 5 (4,23–6,8) | 4,64 (4,35–5,1) | 10 |
| 45 | 166 (127–191) | 1,68 (1,39–2) | 4,63 (3–6,24) | 4,71 (4,33–5,09) | 10 |
| 46 | 167 (126–188) | 1,62 (1,41–1,77) | 4,83 (2,3–6,78) | 4,7 (4,29–5,16) | 10 |
| 47 | 168 (124–182) | 1,67 (1,54–1,9) | 5 (3,35–5,88) | 4,75 (4,31–5,23) | 10 |
| 48 | 169 (124–182) | 1,61 (1,43–1,84) | 4,89 (3,14–6,35) | 4,69 (4,28–5,14) | 10 |
| 49 | 170 (124–186) | 1,64 (1,38–1,85) | 4,75 (2,16–6) | 4,73 (4,33–5,13) | 10 |
| 50 | 172 (126–179) | 1,72 (1,39–1,87) | 4,73 (3,04–7,27) | 4,77 (4,33–5,19) | 10 |
| 51 | 172 (126–180) | 1,64 (1,39–2,01) | 4,63 (2,75–6,33) | 4,72 (4,29–5,23) | 10 |
| 52 | 173 (124–188) | 1,68 (1,42–2) | 5 (3,23–6,9) | 4,7 (4,32–5,24) | 10 |
| 53 | 172 (124–191) | 1,62 (1,38–1,92) | 4,58 (3–6,5) | 4,64 (4,42–5,24) | 10 |
| 54 | 175 (128–186) | 1,63 (1,36–1,94) | 4,71 (3,52–6,6) | 4,57 (4,33–5,3) | 10 |
| 55 | 174 (127–185) | 1,61 (1,39–1,88) | 5 (3,4–6,28) | 4,62 (4,37–5,3) | 10 |
| 56 | 170 (126–191) | 1,58 (1,28–1,84) | 5 (3,36–6,54) | 4,59 (4,37–5,32) | 10 |
| 57 | 172 (128–192) | 1,71 (1,52–1,91) | 5 (3,01–6,4) | 4,69 (4,3–5,29) | 10 |
| 58 | 173 (130–194) | 1,62 (1,35–1,75) | 5,25 (3,33–6,51) | 4,62 (4,27–5,28) | 10 |
| 59 | 170 (130–196) | 1,63 (1,47–1,9) | 5,06 (4,48–6,17) | 4,62 (4,33–5,28) | 10 |
| 60 | 170 (129–196) | 1,63 (1,28–1,84) | 5 (3,58–6,83) | 4,63 (4,37–5,24) | 10 |

### Han ve ticaret

| Yıl | Ayakta han | Asılan ilan | Biten ilan | Ticaret seferi (kervan) | İkmal seferi |
|---|---|---|---|---|---|
| 1 | 4 (3–6) | 14 (11,5–17) | 7,5 (5–11,5) | 85 (62,5–153) | 39 (16,5–49,5) |
| 2 | 4 (3–6) | 13 (10–16,5) | 6 (4–12) | 86 (58,5–164) | 37 (15–50,5) |
| 3 | 4 (3–6) | 12 (8–22) | 7 (3,5–12,5) | 80 (62,5–160) | 39,5 (15,5–48) |
| 4 | 4 (3–5,5) | 15 (7,5–21,5) | 5,5 (3–12) | 90 (70–158) | 39,5 (12,5–48,5) |
| 5 | 3,5 (3–6) | 13 (8,5–22,5) | 6,5 (3–9,5) | 99 (60–149) | 35 (13,5–51,5) |
| 6 | 3,5 (2,5–6) | 14,5 (9,5–19,5) | 8 (3–11) | 102 (55,5–154) | 34,5 (13,5–56,5) |
| 7 | 3,5 (2,5–6) | 13 (8,5–18) | 8 (3–13) | 96 (57–140) | 34,5 (18–51,5) |
| 8 | 4 (3–6) | 13,5 (10–21,5) | 7 (4–13,5) | 97 (51–146) | 35,5 (18–48) |
| 9 | 4 (2,5–6) | 14,5 (7–19,5) | 8 (2,5–11,5) | 102 (51–152) | 33,5 (18–46,5) |
| 10 | 4 (2,5–6) | 15 (7,5–19) | 7,5 (4,5–10) | 108 (55–152) | 33 (20–45,5) |
| 11 | 4 (3–6) | 13 (5,5–20,5) | 6,5 (2,5–9) | 107 (49,5–149) | 33 (24,5–43) |
| 12 | 4 (3–6) | 14 (6–20) | 5,5 (2–11) | 109 (56,5–152) | 35,5 (24,5–45,5) |
| 13 | 4 (3–6) | 13,5 (7–24) | 7 (4–11) | 99 (54–157) | 33,5 (21–46) |
| 14 | 4 (3–6) | 14 (6–23) | 5,5 (3,5–10) | 106 (58,5–148) | 33 (21–47,5) |
| 15 | 4 (2,5–6) | 16,5 (6,5–27,5) | 6 (4–14,5) | 92,5 (57,5–162) | 34 (21–46) |
| 16 | 4 (2,5–6) | 15 (12,5–24) | 7 (4–10,5) | 96,5 (54,5–166) | 35 (21,5–45,5) |
| 17 | 4 (2,5–6) | 16,5 (10,5–23) | 6 (3–11) | 97,5 (61–168) | 36,5 (21,5–54) |
| 18 | 4 (3–6) | 16 (10–28,5) | 7 (4–12,5) | 95,5 (54,5–168) | 36 (20,5–59,5) |
| 19 | 4 (3–6) | 16,5 (10–29) | 9 (5,5–13,5) | 94,5 (59–161) | 36 (19–58,5) |
| 20 | 3,5 (3–6) | 13,5 (10–23,5) | 6 (3–14) | 110 (55–175) | 35,5 (20–62) |
| 21 | 3,5 (3–6) | 14 (9–22,5) | 5,5 (2,5–11) | 122 (63–178) | 37 (24–61) |
| 22 | 3,5 (3–6) | 15 (9–23,5) | 7 (3,5–11,5) | 120 (72–170) | 38,5 (27,5–63) |
| 23 | 4 (3–6) | 14 (9,5–27,5) | 6,5 (3,5–10,5) | 124 (65,5–160) | 40,5 (26,5–57,5) |
| 24 | 4 (3–6) | 15,5 (11–22) | 7,5 (4,5–8,5) | 111 (58,5–172) | 40 (26–52) |
| 25 | 4 (3–6) | 14 (8–23) | 6,5 (3–11) | 106 (59–170) | 37 (23–52) |
| 26 | 4 (3–6,5) | 13,5 (7,5–21,5) | 4,5 (1–8) | 104 (65–168) | 37 (21,5–50,5) |
| 27 | 4 (3–6) | 16,5 (7–22) | 5,5 (2–8,5) | 104 (59,5–176) | 35,5 (19,5–49) |
| 28 | 4 (3–6) | 14,5 (7,5–23) | 5 (2,5–12) | 103 (69–172) | 36,5 (21,5–49,5) |
| 29 | 4 (3–5,5) | 14 (9,5–22) | 5 (3–8,5) | 97,5 (68–172) | 37,5 (23,5–48) |
| 30 | 4 (3–5,5) | 12,5 (9,5–25) | 5,5 (3,5–8) | 99,5 (70,5–179) | 37 (22,5–51) |
| 31 | 4,5 (3–6) | 15 (7,5–26,5) | 6 (4–11) | 108 (63,5–170) | 35 (19,5–51) |
| 32 | 4 (3–6) | 12 (6,5–24) | 7,5 (2,5–10,5) | 112 (73–164) | 38,5 (17,5–50) |
| 33 | 4 (3–6) | 13 (6–20,5) | 4,5 (1–9) | 106 (67–162) | 42 (18–53) |
| 34 | 4 (3–5,5) | 12,5 (7–28) | 5,5 (3–12,5) | 114 (70,5–165) | 41 (20,5–57,5) |
| 35 | 4 (3–5) | 15 (10,5–23) | 6 (2,5–11,5) | 110 (56,5–187) | 40,5 (17–58) |
| 36 | 4 (3–5,5) | 15 (6,5–23,5) | 7,5 (3–11) | 112 (68–170) | 41,5 (17,5–56,5) |
| 37 | 4 (2,5–5,5) | 12 (8,5–26) | 4,5 (1,5–14,5) | 110 (57,5–178) | 44,5 (17–55,5) |
| 38 | 4 (3–5,5) | 13,5 (7–22,5) | 5 (2,5–9) | 118 (61,5–184) | 44 (18,5–60) |
| 39 | 4 (3–5,5) | 13,5 (7,5–23,5) | 6 (1,5–10,5) | 110 (57–176) | 42 (21–59) |
| 40 | 4 (3–5,5) | 13,5 (9,5–27,5) | 7 (2,5–11,5) | 113 (58,5–164) | 41 (21,5–61) |
| 41 | 4 (3–6) | 17 (13–30,5) | 7,5 (4–12) | 106 (50–176) | 43 (23,5–59,5) |
| 42 | 4,5 (3–6) | 16,5 (11,5–26,5) | 7 (4–13) | 108 (49–156) | 41,5 (22,5–61) |
| 43 | 4 (3–6) | 16,5 (9,5–24,5) | 7 (3,5–11,5) | 114 (54–160) | 43,5 (23–61) |
| 44 | 4 (3–6) | 15 (8–32) | 6,5 (3,5–15) | 116 (63,5–166) | 41 (24,5–62,5) |
| 45 | 4 (3–6) | 18,5 (8,5–23,5) | 6,5 (2–10,5) | 116 (61–162) | 40,5 (25,5–62) |
| 46 | 4 (3–6) | 17 (7,5–21,5) | 6,5 (3,5–10,5) | 125 (58,5–166) | 41,5 (27–64) |
| 47 | 4 (3–6) | 16 (6–19,5) | 6 (1,5–9) | 112 (60,5–170) | 42,5 (28,5–64,5) |
| 48 | 4,5 (3–6) | 14 (9–24) | 7 (3–10) | 122 (59,5–186) | 42 (31–66) |
| 49 | 4 (3–6) | 15,5 (7–25,5) | 6,5 (2–12,5) | 114 (59,5–180) | 44,5 (33–67) |
| 50 | 4 (3–6) | 13 (6,5–26) | 7 (2,5–11) | 104 (62–178) | 46,5 (33,5–69) |
| 51 | 4 (3–6) | 16 (8,5–24,5) | 7,5 (4,5–11) | 112 (59,5–172) | 49 (35,5–66) |
| 52 | 4 (3–6) | 14 (11,5–24) | 6 (3–9) | 120 (57–183) | 50,5 (33–67,5) |
| 53 | 4 (3–6) | 14,5 (8–22) | 7 (3–11) | 124 (67,5–194) | 50,5 (33–74) |
| 54 | 4 (3–6) | 13 (8,5–22,5) | 7,5 (3,5–11) | 108 (65,5–191) | 43 (34,5–59) |
| 55 | 4 (2,5–6) | 13,5 (7–27) | 5,5 (2,5–13) | 115 (60–172) | 40,5 (31,5–60) |
| 56 | 4 (2,5–6) | 14,5 (6,5–23,5) | 6 (3–10,5) | 114 (68–182) | 43 (32,5–73) |
| 57 | 4 (3–6) | 13 (8–24,5) | 6,5 (2,5–12,5) | 108 (58–188) | 43 (25,5–65,5) |
| 58 | 4 (3–6) | 15 (6,5–27) | 4,5 (2–13) | 106 (68–176) | 46 (28,5–67) |
| 59 | 4 (3–6) | 16 (10–23,5) | 6 (4–12,5) | 132 (67–172) | 53 (28–74,5) |
| 60 | 4 (3–6) | 13,5 (7–23) | 5 (2–13,5) | 114 (64–190) | 48,5 (23,5–68,5) |

### Altın ve ambar (1/3)

| Yıl | Altın p90 (medeniyetler) | Bakım gideri (altın; asker, kahraman, L2–L3) | Kamu işlerine (imar) harcanan altın | Ambarla beslenen amele tayını (gıda) | Kamu işlerindeki (amele) iş gücü payı | İmar ortalaması (köy+, 0–100) |
|---|---|---|---|---|---|---|
| 1 | 1035 (720–1516) | 2894 (2131–3218) | 1800 (987–2747) | 1212 (21,2–2943) | %15 (%10–%16) | 18,9 (12,8–25,6) |
| 2 | 1171 (605–1742) | 2807 (1957–3205) | 1777 (947–2937) | 852 (57,6–3248) | %14 (%8,8–%19) | 17,9 (12,1–26) |
| 3 | 1090 (752–1456) | 2751 (2197–3322) | 1777 (785–3372) | 863 (1,6–3712) | %14 (%10–%18) | 19,1 (12,2–26,4) |
| 4 | 1027 (641–1531) | 2908 (2207–3280) | 1608 (727–3234) | 942 (16,8–3214) | %14 (%7,4–%17) | 18,9 (12,3–26) |
| 5 | 908 (690–1629) | 2818 (2095–3536) | 1618 (989–2934) | 821 (32,2–3019) | %13 (%7,9–%16) | 17,8 (12,4–24,3) |
| 6 | 1057 (647–1705) | 2814 (2068–3532) | 1639 (822–3176) | 1063 (0–1959) | %11 (%8,2–%17) | 19,1 (12,4–24,1) |
| 7 | 1087 (853–1827) | 2881 (2026–3491) | 1821 (1091–3393) | 602 (2,4–2750) | %12 (%7,2–%16) | 18,6 (12,1–23,1) |
| 8 | 1267 (824–1786) | 2810 (2043–3464) | 1959 (1084–3335) | 987 (25,4–2559) | %11 (%8–%15) | 19 (11,8–23,3) |
| 9 | 1193 (914–1597) | 2906 (1901–3464) | 2001 (880–3446) | 1149 (12,6–3205) | %12 (%8,6–%15) | 19,3 (11,8–22,6) |
| 10 | 1126 (701–1669) | 2800 (2060–3478) | 1938 (1009–3737) | 863 (74,2–3182) | %12 (%8,8–%15) | 19,6 (12,3–23,3) |
| 11 | 1089 (683–1549) | 2696 (2143–3459) | 1916 (797–3368) | 1104 (111–2314) | %12 (%8,6–%16) | 18,3 (11,4–22,7) |
| 12 | 1006 (752–1502) | 2844 (2180–3339) | 1954 (855–2868) | 581 (109–2566) | %11 (%7,4–%14) | 17,4 (11,6–23,2) |
| 13 | 1151 (799–1526) | 2876 (2116–3450) | 2046 (885–3179) | 1043 (223–2968) | %12 (%7,9–%15) | 19,4 (12,3–25,2) |
| 14 | 1107 (762–1543) | 2972 (2130–3514) | 1975 (934–3085) | 1271 (25,4–2959) | %12 (%7,7–%15) | 17,8 (12,3–25) |
| 15 | 1052 (767–1366) | 2932 (2094–3569) | 1978 (969–3073) | 823 (49,2–3153) | %12 (%7,5–%16) | 18,4 (12–27,3) |
| 16 | 1135 (900–1517) | 2931 (2056–3492) | 1896 (1094–2853) | 774 (177–2016) | %11 (%8,9–%16) | 18,5 (12,3–25,7) |
| 17 | 1157 (758–1411) | 2989 (2032–3388) | 1718 (1289–3665) | 807 (26–2567) | %12 (%9,6–%15) | 17,5 (13,3–24,5) |
| 18 | 1103 (834–1470) | 2964 (1975–3377) | 1780 (1259–3644) | 772 (98,4–2554) | %12 (%9,5–%15) | 18,1 (13,6–24,2) |
| 19 | 1174 (844–1511) | 2964 (1962–3467) | 1685 (1184–3823) | 850 (111–2355) | %12 (%8,5–%16) | 17,3 (13,5–24,4) |
| 20 | 1271 (658–1529) | 3029 (1920–3339) | 1804 (1030–3657) | 983 (58–2684) | %11 (%8,7–%16) | 16,5 (13,1–26,2) |
| 21 | 1089 (750–1671) | 3038 (1955–3385) | 2072 (801–3525) | 790 (65,6–2503) | %11 (%9–%16) | 16,4 (12,6–26) |
| 22 | 1175 (595–1716) | 2957 (1960–3480) | 1940 (822–3965) | 633 (32,2–2640) | %11 (%8,3–%16) | 15,2 (12,1–25) |
| 23 | 1097 (856–1895) | 2935 (1977–3474) | 1834 (1034–3708) | 700 (7–2214) | %11 (%7,8–%16) | 15,1 (12–24,7) |
| 24 | 1237 (808–1848) | 2948 (1898–3556) | 1564 (1083–4073) | 1129 (128–2546) | %12 (%7,2–%15) | 16,6 (11,5–25,4) |
| 25 | 1382 (947–1825) | 2955 (1879–3575) | 2057 (1305–3866) | 1094 (129–2651) | %11 (%6,9–%16) | 18,1 (11,6–27,3) |
| 26 | 1174 (954–1773) | 2929 (1980–3618) | 1984 (1433–3794) | 484 (47,2–2437) | %11 (%6,7–%16) | 18,4 (13,2–26,3) |
| 27 | 1206 (834–1772) | 2861 (1950–3525) | 2067 (1114–3423) | 825 (4–2136) | %11 (%6,1–%15) | 18,4 (13,3–26,1) |
| 28 | 1239 (904–2020) | 2815 (1984–3471) | 2412 (1223–4219) | 524 (112–2164) | %10 (%6,8–%16) | 18,3 (13,7–26,5) |
| 29 | 1167 (829–1901) | 2791 (2051–3277) | 2386 (1213–4376) | 624 (123–3155) | %13 (%8–%16) | 17,8 (14,3–26,9) |
| 30 | 1222 (940–1880) | 2852 (2043–3360) | 2302 (1293–3287) | 987 (32,4–2216) | %13 (%8,3–%18) | 18,9 (13,5–25,6) |
| 31 | 1215 (876–2275) | 2739 (2002–3362) | 2245 (1238–4243) | 794 (42,2–2110) | %13 (%8,5–%17) | 19,3 (12–26,8) |
| 32 | 1206 (793–2197) | 2710 (2082–3465) | 2403 (1326–4574) | 648 (30,6–1906) | %14 (%7,8–%18) | 19,2 (10,8–26,8) |
| 33 | 1261 (860–2113) | 2587 (2075–3377) | 2709 (1145–4785) | 738 (0–1598) | %14 (%8,9–%19) | 18,4 (9,74–26,7) |
| 34 | 1206 (858–1977) | 2674 (2115–3382) | 2640 (1250–4224) | 347 (67,6–1675) | %12 (%7,1–%18) | 17,5 (10,4–28,8) |
| 35 | 1220 (766–2108) | 2655 (2074–3237) | 2305 (1315–4129) | 466 (23,2–2083) | %12 (%6,4–%18) | 17,4 (11,1–27,4) |
| 36 | 1395 (773–2171) | 2657 (2051–3051) | 2533 (1179–3943) | 315 (0–1712) | %12 (%5,7–%16) | 17,4 (11,7–28,8) |
| 37 | 1512 (854–2158) | 2616 (1891–3111) | 2705 (1322–4185) | 218 (0–2283) | %13 (%5,8–%17) | 18,4 (11,8–29,2) |
| 38 | 1445 (814–1971) | 2637 (1837–3053) | 2850 (1297–4509) | 328 (0–2166) | %11 (%6,1–%21) | 18,5 (11,7–29,2) |
| 39 | 1379 (828–2476) | 2556 (1890–3027) | 2912 (1133–4114) | 625 (0–1941) | %12 (%6,1–%22) | 19 (10,1–27,8) |
| 40 | 1367 (960–2543) | 2584 (1993–3101) | 2768 (1396–5807) | 565 (0–1955) | %13 (%6,7–%21) | 20,4 (11,3–27,9) |
| 41 | 1440 (770–2242) | 2531 (1921–3142) | 2749 (1311–4714) | 360 (0–1848) | %13 (%7,2–%16) | 19,8 (10,3–26,7) |
| 42 | 1438 (1082–2109) | 2440 (1966–3209) | 2975 (1383–4944) | 414 (0–1529) | %9,8 (%6,3–%16) | 21,2 (9,8–26,9) |
| 43 | 1414 (717–2266) | 2457 (1897–3103) | 2927 (1346–4651) | 470 (0–802) | %9,9 (%6,1–%16) | 19,1 (9,66–25,5) |
| 44 | 1645 (814–2401) | 2599 (1827–3086) | 2897 (1083–4572) | 311 (0–1561) | %11 (%6,4–%14) | 18,9 (9,38–25,2) |
| 45 | 1457 (928–2248) | 2551 (1810–3039) | 3194 (1515–4610) | 348 (0–1387) | %10 (%6,3–%15) | 17,5 (9,6–25,3) |
| 46 | 1351 (1006–2214) | 2502 (1854–3262) | 2975 (2001–4229) | 226 (0–1700) | %10 (%5,9–%14) | 17,6 (9,57–26) |
| 47 | 1394 (882–2303) | 2491 (1799–3283) | 2528 (1366–4285) | 326 (0–931) | %9,9 (%5,8–%16) | 17,9 (9,06–26,5) |
| 48 | 1243 (856–2335) | 2502 (1853–3156) | 2737 (1243–4259) | 189 (0–1090) | %11 (%7,8–%17) | 17,6 (8,88–27,2) |
| 49 | 1262 (885–2320) | 2538 (1832–3143) | 2724 (1200–4622) | 384 (0–961) | %12 (%8,1–%19) | 17,1 (8,33–27,4) |
| 50 | 1544 (782–2259) | 2554 (1882–3127) | 2595 (1154–4360) | 265 (5,6–746) | %11 (%7,9–%18) | 17,2 (7,88–27) |
| 51 | 1246 (890–2237) | 2520 (1985–3106) | 2573 (1259–4201) | 359 (0–1175) | %10 (%8,3–%16) | 18,2 (8,21–27,5) |
| 52 | 1068 (849–2150) | 2368 (1972–3360) | 2255 (1305–3956) | 178 (0–975) | %11 (%5,7–%16) | 19 (8,34–25,7) |
| 53 | 1220 (893–2219) | 2281 (1928–3214) | 2164 (1105–4025) | 195 (0,4–816) | %11 (%7,1–%18) | 17,7 (7,92–25,5) |
| 54 | 1211 (820–2132) | 2342 (1850–3363) | 2420 (1185–3875) | 294 (0–1171) | %11 (%6,6–%17) | 16,9 (7,68–24,3) |
| 55 | 1243 (887–2183) | 2396 (1797–3249) | 2429 (1161–3945) | 150 (0–1477) | %11 (%4,8–%17) | 17,4 (8,47–24,2) |
| 56 | 1277 (962–2121) | 2343 (1780–3217) | 2383 (1398–3783) | 80,8 (0–1040) | %11 (%4,9–%17) | 17,7 (8,61–25,6) |
| 57 | 1274 (971–2217) | 2397 (1771–3213) | 2424 (1389–3734) | 12,4 (0–972) | %9,3 (%6,1–%19) | 17,6 (8,56–24,1) |
| 58 | 1397 (932–2291) | 2350 (1709–3219) | 2733 (1290–4121) | 159 (0–1005) | %8,9 (%5,3–%18) | 18,2 (9,4–24,4) |
| 59 | 1440 (865–2505) | 2438 (1721–3315) | 2856 (1244–5089) | 314 (0–1089) | %8,4 (%4,8–%15) | 19,2 (8,9–24,3) |
| 60 | 1565 (1117–2173) | 2346 (1659–3142) | 2879 (1630–4710) | 124 (0–831) | %10 (%5,9–%17) | 18,9 (8,81–24,2) |

### Altın ve ambar (2/3)

| Yıl | Canavar baskınında yitirilen altın | Ejderhaya giden altın (haraç + akın) | Hazinesi boş medeniyet payı | Kent tüketiminde yokluk payı (köy+; ekmek, bira ya da alet) | Ekmek ya da bira yokluğu payı (köy+) | Kıtlık (büyük olay) |
|---|---|---|---|---|---|---|
| 1 | 18 (0–45) | 0 | %0 | %30 (%21–%39) | %5,7 (%1,7–%11) | 0 (0–0,5) |
| 2 | 0 (0–12,5) | 0 | %0 | %32 (%19–%40) | %5,6 (%1,2–%12) | 0 |
| 3 | 10,5 (0–33,5) | 0 | %0 | %30 (%21–%44) | %5,6 (%2,2–%14) | 0 (0–0,5) |
| 4 | 10,5 (0–31,5) | 0 | %0 | %32 (%17–%41) | %5,6 (%1,8–%22) | 0 |
| 5 | 0,5 (0–93) | 0 | %0 | %34 (%21–%41) | %5,2 (%1–%21) | 0 (0–0,5) |
| 6 | 1 (0–26) | 0 | %0 | %33 (%23–%46) | %7,6 (%0,4–%24) | 0 |
| 7 | 0 (0–22,5) | 0 | %0 | %34 (%22–%42) | %5,9 (%0,8–%22) | 0 |
| 8 | 7 (0–27) | 0 | %0 | %36 (%26–%45) | %7,4 (%2,5–%23) | 0 (0–0,5) |
| 9 | 6,5 (0–34,5) | 0 | %0 | %35 (%26–%47) | %8,6 (%1,6–%19) | 0 |
| 10 | 3 (0–69) | 0 | %0 | %34 (%25–%45) | %8,9 (%1,5–%20) | 0 |
| 11 | 5 (0–22,5) | 0 | %0 | %31 (%22–%46) | %5,5 (%0,7–%18) | 0 |
| 12 | 0 (0–36,5) | 0 | %0 | %29 (%21–%44) | %7,3 (%1,9–%16) | 0 (0–1) |
| 13 | 4,5 (0–26,5) | 0 | %0 | %30 (%20–%40) | %6,9 (%2–%14) | 0 |
| 14 | 6,5 (0–24,5) | 0 (0–9,5) | %0 | %34 (%26–%40) | %8 (%1,9–%18) | 0 |
| 15 | 4 (0–24) | 0 (0–471) | %0 | %34 (%24–%42) | %6,8 (%1,6–%17) | 0 (0–1) |
| 16 | 1,5 (0–25,5) | 98 (0–278) | %0 | %33 (%24–%41) | %8 (%1,2–%16) | 0 |
| 17 | 3,5 (0–18) | 122 (0–356) | %0 | %37 (%28–%45) | %8,2 (%2,5–%22) | 0 |
| 18 | 6,5 (0–23,5) | 132 (0–316) | %0 | %36 (%27–%44) | %9,1 (%3,2–%20) | 0 |
| 19 | 5 (0–28) | 5 (0–320) | %0 | %36 (%26–%45) | %9,8 (%2,5–%17) | 0 |
| 20 | 5,5 (0–17) | 0 (0–434) | %0 | %36 (%26–%43) | %9,8 (%3,3–%19) | 0 (0–0,5) |
| 21 | 1 (0–24) | 0 (0–518) | %0 | %35 (%23–%42) | %10 (%3–%18) | 0 (0–0,5) |
| 22 | 3,5 (0–39) | 0 (0–336) | %0 | %34 (%24–%47) | %10 (%3,4–%18) | 0 (0–0,5) |
| 23 | 9,5 (0–28) | 0 (0–864) | %0 | %35 (%24–%44) | %9,6 (%2,9–%17) | 0 (0–1) |
| 24 | 4 (0–54) | 0 (0–458) | %0 | %33 (%23–%41) | %7,6 (%3,4–%15) | 0 |
| 25 | 4,5 (0–66) | 0 (0–493) | %0 | %35 (%24–%40) | %9,2 (%2,7–%17) | 0 (0–0,5) |
| 26 | 5 (0–25) | 0 (0–911) | %0 | %33 (%22–%41) | %7,9 (%3,1–%15) | 0 (0–0,5) |
| 27 | 1,5 (0–27,5) | 0 (0–743) | %0 | %33 (%23–%43) | %7 (%3,6–%15) | 0 |
| 28 | 9,5 (0–45) | 0 (0–769) | %0 | %32 (%20–%40) | %7,6 (%3,6–%14) | 0 |
| 29 | 0,5 (0–43) | 0 (0–1006) | %0 | %36 (%17–%43) | %7,6 (%3,3–%18) | 0 |
| 30 | 7,5 (0–22) | 0 (0–732) | %0 | %34 (%19–%43) | %9,5 (%3,3–%22) | 0 |
| 31 | 6,5 (0–27,5) | 0 (0–674) | %0 | %34 (%23–%45) | %7,7 (%3,6–%22) | 0 (0–0,5) |
| 32 | 6 (0–29,5) | 0 (0–968) | %0 | %35 (%21–%48) | %6,9 (%3,7–%16) | 0 |
| 33 | 8 (0–19) | 0 (0–452) | %0 | %31 (%23–%48) | %9,4 (%3,1–%24) | 0 (0–0,5) |
| 34 | 0 (0–54,5) | 0 (0–686) | %0 | %35 (%25–%48) | %11 (%5,6–%23) | 0 (0–0,5) |
| 35 | 0,5 (0–32,5) | 0 (0–1154) | %0 | %37 (%23–%45) | %8,9 (%3,7–%32) | 0 (0–1) |
| 36 | 16,5 (0–40) | 0 (0–634) | %0 | %36 (%24–%45) | %7,4 (%4,3–%31) | 0 |
| 37 | 10 (0–45,5) | 0 (0–690) | %0 | %38 (%27–%49) | %10 (%4,5–%33) | 0 (0–1) |
| 38 | 7 (0–58,5) | 0 (0–1199) | %0 | %39 (%20–%49) | %11 (%3,4–%36) | 0 (0–0,5) |
| 39 | 8,5 (0–72) | 0 (0–551) | %0 | %38 (%23–%48) | %12 (%4,8–%37) | 0 (0–1) |
| 40 | 9 (0–58) | 0 (0–659) | %0 | %37 (%20–%45) | %12 (%4–%32) | 0 (0–1) |
| 41 | 12,5 (0–35) | 0 (0–482) | %0 | %37 (%22–%53) | %17 (%2,9–%35) | 0 (0–0,5) |
| 42 | 10 (0–24,5) | 0 (0–410) | %0 | %40 (%20–%54) | %16 (%3,4–%32) | 0 |
| 43 | 0 (0–27,5) | 0 (0–738) | %0 | %43 (%22–%53) | %13 (%3,8–%35) | 0 (0–1) |
| 44 | 9,5 (0–42,5) | 0 (0–266) | %0 | %42 (%22–%55) | %14 (%4,3–%35) | 0 (0–0,5) |
| 45 | 17,5 (0–37,5) | 0 (0–392) | %0 | %40 (%23–%51) | %14 (%3,9–%30) | 0 |
| 46 | 2,5 (0–24,5) | 0 (0–660) | %0 | %35 (%22–%49) | %12 (%3,3–%23) | 0 (0–1) |
| 47 | 3 (0–51,5) | 0 (0–422) | %0 | %34 (%23–%50) | %11 (%5,8–%24) | 0 |
| 48 | 8 (0,5–41,5) | 0 (0–357) | %0 | %31 (%23–%47) | %11 (%4,9–%19) | 0 (0–0,5) |
| 49 | 21 (0–80) | 0 (0–514) | %0 | %38 (%21–%54) | %13 (%3,3–%23) | 0 |
| 50 | 4 (0–26) | 0 (0–468) | %0 | %33 (%22–%53) | %12 (%3,8–%25) | 0 (0–0,5) |
| 51 | 2 (0–32,5) | 0 (0–524) | %0 | %32 (%20–%51) | %11 (%3,6–%27) | 0 |
| 52 | 10 (0–63) | 0 (0–833) | %0 | %34 (%22–%55) | %11 (%3,6–%34) | 0 |
| 53 | 12 (0–34,5) | 0 (0–258) | %0 | %30 (%19–%53) | %10 (%2,5–%35) | 0 (0–1) |
| 54 | 4,5 (0–16) | 0 (0–664) | %0 | %28 (%19–%55) | %12 (%3,9–%35) | 0 (0–1) |
| 55 | 7,5 (0–23) | 0 (0–870) | %0 | %30 (%21–%53) | %11 (%3,4–%36) | 0 (0–0,5) |
| 56 | 10 (0–29) | 0 (0–444) | %0 | %36 (%21–%55) | %15 (%4,1–%36) | 0 (0–1) |
| 57 | 5,5 (0–23,5) | 0 (0–718) | %0 | %35 (%18–%58) | %13 (%3,8–%34) | 0 (0–0,5) |
| 58 | 18,5 (0–104) | 0 (0–1139) | %0 | %34 (%19–%54) | %12 (%4,8–%43) | 0 (0–1) |
| 59 | 0 (0–50,5) | 0 (0–256) | %0 | %35 (%19–%50) | %13 (%3,9–%39) | 0 |
| 60 | 16 (0–82,5) | 0 (0–202) | %0 | %34 (%18–%51) | %16 (%4–%38) | 0 |

### Altın ve ambar (3/3)

| Yıl | Açlıktan ölen | Kıtlık yardımı (sevkiyat) | Kıtlıkta yüz çeviren | Kıtlık akını | Ambarın yettiği gün (medeniyet medyanı) |
|---|---|---|---|---|---|
| 1 | 0 (0–22,5) | 0 (0–1) | 0 (0–1) | 0 | 43,7 (22,7–51,5) |
| 2 | 0 (0–4,5) | 0 | 0 | 0 | 35,5 (18,5–54,7) |
| 3 | 0 (0–12) | 0 | 0 (0–0,5) | 0 | 37,5 (29–47,3) |
| 4 | 0 (0–19) | 0 | 0 | 0 | 42 (22–52,2) |
| 5 | 0 (0–34,5) | 0 (0–0,5) | 0 | 0 | 42,2 (17,9–53) |
| 6 | 0 (0–19,5) | 0 (0–1,5) | 0 | 0 | 42,8 (16,2–54) |
| 7 | 0 (0–25) | 0 | 0 | 0 (0–0,5) | 39,4 (10,7–55,7) |
| 8 | 0 (0–19,5) | 0 (0–2,5) | 0 | 0 | 34,7 (11,5–53,7) |
| 9 | 0 (0–27) | 0 | 0 | 0 (0–0,5) | 36,1 (18,9–53,7) |
| 10 | 0 (0–15) | 0 | 0 | 0 (0–0,5) | 33,4 (16,7–53,5) |
| 11 | 0 (0–15,5) | 0 | 0 | 0 (0–0,5) | 36,7 (23,6–50,7) |
| 12 | 0 (0–14) | 0 (0–1) | 0 (0–3,5) | 0 | 37,5 (21,2–51,9) |
| 13 | 0 (0–17) | 0 (0–1,5) | 0 | 0 (0–0,5) | 36,6 (25,4–53,4) |
| 14 | 0 (0–8,5) | 0 | 0 | 0 | 38,3 (21,2–51,9) |
| 15 | 0 (0–8,5) | 0 (0–1) | 0 (0–2) | 0 (0–0,5) | 35,4 (22,6–54,6) |
| 16 | 0 (0–8,5) | 0 (0–0,5) | 0 (0–0,5) | 0 (0–1) | 31,7 (21,4–51,5) |
| 17 | 0 (0–14) | 0 (0–0,5) | 0 | 0 (0–0,5) | 30,3 (23,1–49,7) |
| 18 | 0 (0–10,5) | 0 | 0 (0–1) | 0 | 33,2 (24,2–53) |
| 19 | 0 (0–14) | 0 | 0 | 0 (0–0,5) | 31,3 (21,3–51,3) |
| 20 | 0 (0–14) | 0 (0–0,5) | 0 (0–0,5) | 0 (0–0,5) | 31,8 (21–49,8) |
| 21 | 0 (0–15,5) | 0 (0–0,5) | 0 (0–1,5) | 0 | 34,6 (19,9–48,4) |
| 22 | 0 (0–29) | 0 (0–1) | 0 (0–2) | 0 | 35,7 (24–49,7) |
| 23 | 0 (0–27,5) | 0 (0–2,5) | 0 | 0 (0–1) | 36,4 (22,7–53) |
| 24 | 0 (0–15,5) | 0 | 0 | 0 | 37,4 (24,1–49,1) |
| 25 | 0 (0–12) | 0 (0–0,5) | 0 (0–1) | 0 (0–0,5) | 35,7 (25,1–48) |
| 26 | 0 (0–14) | 0 (0–1) | 0 | 0 | 33,8 (21,6–46,6) |
| 27 | 0 (0–16,5) | 0 | 0 (0–1) | 0 (0–0,5) | 33,1 (20,2–48,1) |
| 28 | 0 (0–14,5) | 0 | 0 | 0 | 35,8 (20,8–49,5) |
| 29 | 0 (0–21,5) | 0 | 0 | 0 (0–1) | 38,9 (20,2–49,4) |
| 30 | 0 (0–12) | 0 | 0 | 0 | 41,7 (22,5–52,7) |
| 31 | 0,5 (0–11,5) | 0 | 0 (0–2) | 0 (0–0,5) | 36,8 (22,4–47,2) |
| 32 | 0 (0–11,5) | 0 | 0 | 0 | 35,9 (18,4–48,6) |
| 33 | 0 (0–6,5) | 0 (0–1,5) | 0 (0–1,5) | 0 (0–0,5) | 34,4 (17,3–44,3) |
| 34 | 0 (0–12) | 0 (0–1) | 0 (0–1,5) | 0 (0–0,5) | 31,5 (18,6–43,2) |
| 35 | 0 (0–25) | 0 (0–1) | 0 (0–2) | 0 | 30,2 (14–45,4) |
| 36 | 0 (0–11,5) | 0 | 0 (0–1,5) | 0 (0–2) | 36 (15,6–53,6) |
| 37 | 0,5 (0–17,5) | 0 (0–1) | 0 (0–2) | 0 (0–1) | 29,2 (15,6–47,7) |
| 38 | 0 (0–26,5) | 0 | 0 (0–0,5) | 0 (0–1) | 26 (17,4–49,2) |
| 39 | 0 (0–21) | 0 (0–3) | 0 (0–2) | 0 (0–1,5) | 28,3 (16,3–49,1) |
| 40 | 0 (0–12) | 0 (0–1) | 0 (0–2) | 0 | 29,7 (18,2–47,5) |
| 41 | 1 (0–14) | 0 (0–1,5) | 0 (0–2) | 0 (0–1) | 30,5 (13,9–43,3) |
| 42 | 4,5 (0–32) | 0 (0–0,5) | 0 (0–0,5) | 0 (0–0,5) | 31,3 (14,7–48,4) |
| 43 | 0,5 (0–18) | 0 (0–2) | 0 (0–3) | 0 (0–1) | 28,7 (10,3–40,5) |
| 44 | 1 (0–12,5) | 0 (0–0,5) | 0 (0–1) | 0 | 25,6 (9,74–40,1) |
| 45 | 1,5 (0–12,5) | 0 | 0 (0–0,5) | 0 | 23,5 (9,28–45,8) |
| 46 | 0,5 (0–6,5) | 0 (0–0,5) | 0 (0–1,5) | 0 (0–1,5) | 27,7 (12,3–43,1) |
| 47 | 0 (0–8,5) | 0 | 0 (0–0,5) | 0 | 27,7 (14,4–38,4) |
| 48 | 0 (0–1) | 0 (0–0,5) | 0 (0–0,5) | 0 (0–0,5) | 27,7 (10,8–43,6) |
| 49 | 0 (0–8) | 0 | 0 | 0 | 26,1 (11–45,1) |
| 50 | 0 (0–9) | 0 (0–0,5) | 0 (0–0,5) | 0 | 24,5 (6,53–40,8) |
| 51 | 0 (0–5) | 0 | 0 | 0 | 21,2 (6,09–41,5) |
| 52 | 0 (0–28,5) | 0 | 0 | 0 | 22,9 (7,91–42,6) |
| 53 | 0 (0–17) | 0 | 0 (0–1,5) | 0 | 23,7 (7,64–44,2) |
| 54 | 2 (0–17,5) | 0 (0–1) | 0 (0–2) | 0 (0–0,5) | 26,5 (9,19–44,5) |
| 55 | 3 (0–14,5) | 0 (0–2,5) | 0 | 0 (0–0,5) | 20,3 (7,46–39,1) |
| 56 | 1,5 (0–14) | 0 (0–3) | 0 (0–4) | 0 (0–1) | 17,1 (7,72–41,2) |
| 57 | 0 (0–11) | 0 (0–3) | 0 (0–2) | 0 (0–1) | 22 (5,49–42,8) |
| 58 | 0,5 (0–18) | 0 (0–1) | 0 (0–2) | 0 | 24 (4,42–38,1) |
| 59 | 1,5 (0–16) | 0 | 0 | 0 | 25,1 (5,8–33,7) |
| 60 | 2 (0–12,5) | 0 | 0 | 0 | 23,7 (7,33–34,4) |

### Yerleşim kademesi

| Yıl | Ortalama yerleşim kademesi | Köy+ yerleşim | Kasaba+ yerleşim | Şehir | Ortalama başkent kademesi | Kademe değişimi (yerleşim, yıl içinde) |
|---|---|---|---|---|---|---|
| 1 | 1,11 (0,85–1,3) | 61,5 (44–70) | 20 (11–31) | 5 (4–6) | 2,5 (2,25–2,69) | 34 (22,5–40,5) |
| 2 | 1,1 (0,85–1,29) | 62 (42,5–70,5) | 20 (10,5–30) | 5 (4–6) | 2,5 (2,33–2,76) | 31,5 (24,5–43,5) |
| 3 | 1,11 (0,75–1,34) | 64,5 (38,5–70) | 19,5 (8–31) | 6 (3,5–6) | 2,56 (2,21–2,69) | 28 (20–40,5) |
| 4 | 1,11 (0,81–1,26) | 62,5 (38,5–72) | 18,5 (9,5–25,5) | 5 (3,5–6) | 2,44 (2,07–2,83) | 33,5 (20–39,5) |
| 5 | 1,08 (0,85–1,29) | 62 (40–70) | 18 (10,5–29,5) | 4,5 (3,5–6) | 2,46 (2,2–2,86) | 31 (17,5–40) |
| 6 | 1,14 (0,86–1,28) | 63,5 (37,5–71,5) | 19,5 (13–28) | 5,5 (4–6) | 2,6 (2,29–2,8) | 25,5 (12,5–35) |
| 7 | 1,14 (0,98–1,31) | 64 (43,5–70,5) | 18 (12–29) | 5 (3,5–6) | 2,5 (2,25–2,75) | 25,5 (16,5–35) |
| 8 | 1,13 (0,98–1,35) | 62,5 (44,5–70) | 20,5 (12–32,5) | 6 (4–6) | 2,57 (2,21–2,82) | 27 (12,5–32) |
| 9 | 1,2 (0,97–1,34) | 66 (43,5–71) | 21 (11,5–31) | 5 (4–6) | 2,57 (2,35–2,73) | 27 (17,5–33) |
| 10 | 1,13 (0,94–1,28) | 64 (44,5–71,5) | 20 (11,5–28,5) | 4 (3,5–6) | 2,5 (2,21–2,67) | 27 (12,5–34,5) |
| 11 | 1,15 (0,94–1,32) | 65 (42,5–71,5) | 19 (14–29,5) | 5 (4–6) | 2,5 (2,31–2,67) | 27 (13–47) |
| 12 | 1,18 (0,96–1,27) | 66 (43,5–72) | 21 (12,5–29) | 4,5 (3–6) | 2,44 (2,17–2,73) | 28 (10–44) |
| 13 | 1,16 (0,98–1,28) | 64 (47–72,5) | 17,5 (13,5–27,5) | 5 (3,5–6) | 2,56 (2,31–2,65) | 27 (21,5–41,5) |
| 14 | 1,17 (1–1,32) | 64,5 (48,5–71) | 22 (14–29,5) | 5 (4–6) | 2,53 (2,29–2,76) | 28 (18–38,5) |
| 15 | 1,15 (0,99–1,3) | 65,5 (45,5–71) | 19 (13,5–28) | 5 (4–6) | 2,5 (2,35–2,83) | 30,5 (22,5–39,5) |
| 16 | 1,15 (0,99–1,29) | 66 (47–72,5) | 18,5 (15–27) | 5,5 (4–6) | 2,59 (2,2–2,85) | 25,5 (14–35) |
| 17 | 1,16 (0,89–1,31) | 65,5 (43,5–72) | 22 (14–29,5) | 6 (4,5–6) | 2,57 (2,33–2,76) | 28 (14,5–39) |
| 18 | 1,14 (0,92–1,32) | 65 (44,5–73) | 21 (14,5–27) | 6 (4,5–6) | 2,65 (2,29–2,78) | 25 (19–36) |
| 19 | 1,13 (0,89–1,33) | 65,5 (43,5–72,5) | 20 (14–26) | 6 (4–6) | 2,5 (2,35–2,76) | 27,5 (18–34) |
| 20 | 1,13 (0,85–1,31) | 65 (43–73,5) | 19,5 (13–27) | 6 (3,5–6) | 2,5 (2,29–2,78) | 23,5 (16–32) |
| 21 | 1,17 (0,87–1,31) | 63,5 (44–72) | 19 (13,5–31) | 5,5 (4–6) | 2,46 (2,33–2,71) | 26,5 (16,5–41,5) |
| 22 | 1,12 (0,87–1,36) | 65,5 (44–71,5) | 18,5 (13,5–29,5) | 6 (4,5–6) | 2,5 (2,29–2,71) | 23,5 (16,5–36,5) |
| 23 | 1,15 (0,88–1,34) | 64,5 (43–72,5) | 19,5 (12–29) | 6 (5–6) | 2,55 (2,4–2,76) | 23,5 (15–34,5) |
| 24 | 1,12 (0,91–1,33) | 62,5 (42,5–72,5) | 20 (10,5–28,5) | 6 (4–6) | 2,54 (2,2–2,71) | 27,5 (18,5–33,5) |
| 25 | 1,16 (0,91–1,36) | 64,5 (42–75) | 20 (12–30) | 6 (4–6) | 2,54 (2,25–2,77) | 26,5 (17–38,5) |
| 26 | 1,14 (0,94–1,38) | 65,5 (46–74,5) | 19 (10–30) | 5,5 (4–6) | 2,54 (2,27–2,77) | 27 (14–33) |
| 27 | 1,13 (0,97–1,39) | 63 (47–77) | 21 (11,5–29) | 6 (4–6) | 2,61 (2,33–2,8) | 24 (19–32) |
| 28 | 1,13 (0,98–1,34) | 63 (47,5–72,5) | 20,5 (13–31) | 5 (4–6) | 2,6 (2,35–2,69) | 25 (15,5–34) |
| 29 | 1,15 (0,92–1,35) | 62 (49–72) | 18,5 (13–32) | 6 (4–6) | 2,55 (2,35–2,83) | 24 (15,5–32,5) |
| 30 | 1,13 (0,98–1,34) | 62 (50,5–72,5) | 19 (12–30) | 6 (5–6) | 2,55 (2,44–2,83) | 21,5 (16,5–31) |
| 31 | 1,16 (0,95–1,35) | 65,5 (51,5–71,5) | 18 (12–31,5) | 6 (4–6) | 2,5 (2,44–2,76) | 27,5 (21,5–34,5) |
| 32 | 1,14 (0,97–1,3) | 63,5 (51,5–72) | 20 (9,5–30,5) | 6 (4–6) | 2,63 (2,42–2,73) | 22,5 (17,5–32) |
| 33 | 1,11 (0,98–1,27) | 64 (50–70,5) | 19 (10,5–27) | 6 (4–6) | 2,5 (2,35–2,8) | 29,5 (20–38) |
| 34 | 1,11 (0,92–1,31) | 63,5 (44,5–72,5) | 19,5 (11,5–30) | 6 (4–6) | 2,65 (2,26–2,83) | 27 (18,5–35) |
| 35 | 1,1 (1,01–1,34) | 63 (47–73) | 19 (11,5–27,5) | 5,5 (4–6) | 2,57 (2,26–2,69) | 25 (18,5–34) |
| 36 | 1,09 (0,99–1,35) | 62,5 (47–70,5) | 20 (11–31,5) | 5 (4–6) | 2,57 (2,4–2,73) | 21 (15,5–33,5) |
| 37 | 1,1 (0,92–1,31) | 62,5 (46,5–70,5) | 19,5 (11–28) | 6 (4–6) | 2,52 (2,31–2,73) | 26 (17,5–31,5) |
| 38 | 1,09 (0,9–1,31) | 61,5 (46–70) | 17 (10,5–27) | 5,5 (4–6) | 2,65 (2,38–2,82) | 23,5 (15–31,5) |
| 39 | 1,06 (0,95–1,25) | 63,5 (48–69,5) | 16,5 (10–27) | 5,5 (4–6) | 2,67 (2,33–2,82) | 26 (16,5–29,5) |
| 40 | 1,06 (0,92–1,26) | 62,5 (45–70) | 16 (10,5–26) | 5 (4–6) | 2,6 (2,26–2,82) | 18,5 (9–29,5) |
| 41 | 1,06 (0,91–1,3) | 62,5 (48–71,5) | 18,5 (9,5–29) | 5,5 (4–6) | 2,59 (2,38–2,83) | 17 (12,5–29) |
| 42 | 1,07 (0,89–1,32) | 62 (47–69) | 16 (10–30,5) | 5 (4–6) | 2,65 (2,44–2,76) | 22,5 (14–37,5) |
| 43 | 1,08 (0,86–1,28) | 64 (47,5–70) | 17,5 (10–27,5) | 5 (4–6) | 2,6 (2,35–2,83) | 20,5 (11–32,5) |
| 44 | 1,11 (0,88–1,31) | 64,5 (49–69,5) | 18,5 (9–28) | 6 (4–6) | 2,67 (2,46–2,85) | 19 (12–29) |
| 45 | 1,06 (0,87–1,35) | 65 (44–71) | 16 (10–31) | 6 (4–6) | 2,6 (2,38–2,83) | 25 (15–31,5) |
| 46 | 1,06 (0,9–1,38) | 64 (45–72) | 18 (9–32,5) | 5,5 (4–6) | 2,65 (2,43–2,83) | 21 (12,5–26,5) |
| 47 | 1,06 (0,91–1,4) | 65,5 (46,5–71,5) | 16 (10–34) | 6 (4–6) | 2,6 (2,43–2,83) | 23 (11,5–33) |
| 48 | 1,03 (0,88–1,38) | 61,5 (46–70) | 16,5 (10,5–33) | 5 (4–6) | 2,65 (2,21–2,77) | 21 (12,5–29) |
| 49 | 1,04 (0,89–1,38) | 62,5 (47–71,5) | 16 (8,5–35,5) | 5,5 (4–6) | 2,5 (2,24–2,83) | 18 (9,5–27) |
| 50 | 1,02 (0,88–1,32) | 61,5 (48–72) | 15,5 (10–31) | 5 (4–6) | 2,54 (2,43–2,77) | 19,5 (13–27,5) |
| 51 | 1,03 (0,87–1,29) | 62,5 (48,5–71,5) | 14,5 (10,5–30) | 5 (3,5–6) | 2,65 (2,36–2,83) | 19,5 (9,5–33) |
| 52 | 0,99 (0,87–1,29) | 62,5 (49,5–72,5) | 13,5 (9–28,5) | 5,5 (4–6) | 2,58 (2,33–2,79) | 19,5 (12–27) |
| 53 | 1 (0,9–1,27) | 61 (46–74) | 14 (8–29) | 5 (4–6) | 2,63 (2,25–2,83) | 23,5 (7–33) |
| 54 | 0,97 (0,89–1,29) | 59,5 (45,5–71) | 15 (9,5–30,5) | 4,5 (4–6) | 2,67 (2,41–2,82) | 22 (9–32) |
| 55 | 1,02 (0,86–1,28) | 60 (48,5–69,5) | 13,5 (8,5–30) | 4,5 (4–5,5) | 2,59 (2,38–2,83) | 21,5 (10–31,5) |
| 56 | 1,01 (0,85–1,31) | 62 (47,5–69,5) | 14,5 (10–29,5) | 4,5 (4–6) | 2,61 (2,35–2,82) | 22 (10–30,5) |
| 57 | 1,01 (0,87–1,24) | 61,5 (47–70) | 14,5 (9,5–25) | 4 (3–5) | 2,46 (2,29–2,73) | 18 (10,5–30) |
| 58 | 0,99 (0,9–1,3) | 61,5 (45–71) | 13,5 (11–28,5) | 5 (4–6) | 2,54 (2,42–2,82) | 17 (12–30) |
| 59 | 1 (0,9–1,26) | 61,5 (48,5–71,5) | 13,5 (10–24,5) | 5 (4–6) | 2,59 (2,33–2,83) | 20,5 (11,5–31) |
| 60 | 1,02 (0,9–1,33) | 63,5 (47–72,5) | 13 (9,5–31) | 5 (4–6) | 2,67 (2,41–2,83) | 19 (10–33,5) |

### Deniz

| Yıl | Liman (tersane) | Gemi (koga/tekne) | Kadırga | Denizaşırı yerleşim | Deniz ticaret yolu (yıl sonu) | Deniz seferi (ticaret) |
|---|---|---|---|---|---|---|
| 1 | 18 (16,5–22,5) | 44,5 (34,5–54,5) | 21,5 (13,5–27) | 7,5 (4,5–9,5) | 14 (7–18,5) | 22 (9–35,5) |
| 2 | 18,5 (16,5–22) | 44,5 (30,5–55) | 21,5 (14,5–27) | 7,5 (4,5–9,5) | 13,5 (7,5–20) | 22,5 (10–35,5) |
| 3 | 18,5 (16–23) | 46 (36–56) | 21 (14–27) | 7,5 (4,5–9) | 13,5 (8–20) | 23 (10,5–37,5) |
| 4 | 18 (15–22) | 45,5 (37–55) | 21,5 (14–27) | 7 (4,5–9,5) | 13,5 (8,5–20) | 23 (13–36) |
| 5 | 18 (14,5–22) | 46 (39,5–55,5) | 22 (14–27) | 7 (4–9,5) | 15 (8–21) | 25,5 (13,5–37) |
| 6 | 18 (15–21,5) | 48 (39–55) | 21,5 (13–27) | 7 (4–9,5) | 13 (8–22,5) | 26,5 (13,5–45) |
| 7 | 18 (15–21) | 47 (37,5–57) | 21,5 (13–27) | 7 (4–9) | 13 (8,5–24) | 26,5 (11,5–43,5) |
| 8 | 18 (15–21) | 47 (38–56,5) | 20,5 (13–27) | 7 (4–9) | 13 (9–24) | 23,5 (14,5–44,5) |
| 9 | 19 (15,5–22) | 48,5 (38–60) | 20,5 (14–26,5) | 7 (4–9) | 13 (8–24,5) | 22,5 (14–48,5) |
| 10 | 19 (17–22) | 48 (37–61,5) | 17,5 (14,5–26) | 7 (4–9) | 14 (8–25) | 24 (14–48) |
| 11 | 20 (17–22,5) | 48 (37,5–61,5) | 19 (14,5–26) | 7 (4–9) | 14,5 (8–25,5) | 22 (10–48) |
| 12 | 20 (16–22) | 48 (37–62,5) | 17,5 (14,5–26,5) | 7 (4–9,5) | 14,5 (7–26,5) | 29,5 (12,5–48) |
| 13 | 19 (16,5–23) | 50 (37–63) | 17,5 (14–28,5) | 7 (4–9) | 15 (6,5–26,5) | 26,5 (9,5–51) |
| 14 | 19,5 (15–23,5) | 49 (36,5–63,5) | 19,5 (13–28) | 7 (4–9) | 15 (6,5–26,5) | 27 (10,5–60,5) |
| 15 | 19,5 (15,5–24) | 50 (37–63) | 19 (13–28) | 7 (4–9) | 14,5 (7–27,5) | 27,5 (10,5–59,5) |
| 16 | 20,5 (17–23) | 50,5 (37,5–63,5) | 19 (14–26,5) | 7 (4–9) | 15 (8–27,5) | 33 (10–60) |
| 17 | 20 (17–24) | 50,5 (39–62) | 18,5 (14–26,5) | 7 (4–9,5) | 15 (8,5–28) | 26 (12–60,5) |
| 18 | 20,5 (19–23) | 49,5 (38–61,5) | 18,5 (14–26,5) | 7 (4–9) | 15 (8,5–28) | 26,5 (13,5–66,5) |
| 19 | 21 (18,5–23,5) | 50 (37,5–62) | 18,5 (14–26,5) | 7 (4–9) | 15,5 (8–30) | 26,5 (11,5–66,5) |
| 20 | 21 (19,5–24) | 50 (38,5–59,5) | 18 (15–24,5) | 6,5 (4–9) | 15 (8,5–30) | 23 (10–70) |
| 21 | 22 (19–24,5) | 53 (38,5–64,5) | 18,5 (15–24,5) | 6,5 (4–9,5) | 15 (9,5–30,5) | 27,5 (11–74,5) |
| 22 | 21,5 (19,5–23,5) | 53,5 (39,5–66) | 19 (15–24,5) | 6,5 (4–10) | 14,5 (10–29,5) | 31 (9,5–65) |
| 23 | 21,5 (18,5–24) | 50 (39–65) | 19 (15–24) | 6,5 (4–10) | 14,5 (10–30,5) | 32 (11,5–68) |
| 24 | 21 (16,5–25,5) | 50 (38–66,5) | 18,5 (12,5–25) | 6 (3,5–10) | 14 (8–30) | 29 (11,5–71) |
| 25 | 20,5 (16,5–26) | 48,5 (39–68,5) | 18 (12,5–24) | 6 (3,5–10) | 14 (9–30) | 25,5 (9,5–63) |
| 26 | 20,5 (17–25,5) | 49 (41,5–70,5) | 17,5 (12,5–23,5) | 6 (3,5–10) | 14,5 (9–30,5) | 26,5 (12,5–72,5) |
| 27 | 20 (16–25,5) | 49 (42–70,5) | 18 (12,5–23,5) | 5,5 (3,5–9,5) | 14,5 (9–30,5) | 26 (15–68) |
| 28 | 20 (16–25,5) | 49,5 (41–71,5) | 19 (12,5–23,5) | 5,5 (3,5–10) | 14,5 (8,5–30,5) | 24,5 (13–73) |
| 29 | 20,5 (16–26) | 49 (38,5–71) | 19,5 (12,5–24) | 5,5 (3,5–10) | 14,5 (8,5–29,5) | 27,5 (13,5–76) |
| 30 | 20 (18–25,5) | 49 (40–68,5) | 19 (12–24,5) | 5,5 (3,5–9,5) | 14,5 (8,5–27) | 27 (17–73) |
| 31 | 20,5 (17,5–26) | 48,5 (40,5–71,5) | 19 (12–24,5) | 5,5 (3,5–9,5) | 14,5 (8–26,5) | 25,5 (12,5–59) |
| 32 | 20 (18,5–23,5) | 48,5 (41,5–71,5) | 19 (11–24,5) | 5,5 (4–9,5) | 15 (8,5–26) | 32,5 (17,5–64) |
| 33 | 20 (17–24,5) | 50 (42–71) | 19 (11–24) | 5,5 (4–9,5) | 15,5 (9–26) | 25,5 (13–62,5) |
| 34 | 20,5 (17,5–24) | 50 (40–68,5) | 19 (11–24) | 5,5 (3,5–9,5) | 15,5 (9,5–25,5) | 25,5 (13–58) |
| 35 | 20,5 (17,5–24) | 51 (39,5–69) | 18,5 (11–25) | 6 (3,5–9,5) | 15,5 (9,5–25,5) | 27 (11,5–54,5) |
| 36 | 20 (17–24) | 50,5 (37,5–64,5) | 18,5 (11–24,5) | 6,5 (3,5–9,5) | 15,5 (9,5–25,5) | 26,5 (10,5–56,5) |
| 37 | 19,5 (17–24) | 49,5 (39,5–65) | 20 (11–24) | 6,5 (3,5–9,5) | 15,5 (9,5–26) | 24 (10,5–62) |
| 38 | 20 (17–23,5) | 49,5 (39–63,5) | 20 (11–24,5) | 6,5 (3,5–9,5) | 14 (9,5–26) | 29,5 (10,5–61,5) |
| 39 | 20 (17–23,5) | 50,5 (37,5–67) | 20 (11–24,5) | 6,5 (3,5–9,5) | 14,5 (9,5–26) | 25,5 (12,5–58,5) |
| 40 | 20,5 (17,5–23,5) | 50,5 (37,5–68,5) | 20 (11–24,5) | 6,5 (3,5–10) | 15 (9,5–26,5) | 25,5 (10,5–64) |
| 41 | 20,5 (17–24) | 52,5 (38–68,5) | 19,5 (11–24,5) | 6,5 (3,5–9,5) | 15,5 (9,5–26,5) | 23 (14–68) |
| 42 | 21 (18–24) | 52,5 (38–68,5) | 18,5 (11–23,5) | 6,5 (3,5–9,5) | 16 (7,5–27) | 28 (10–65,5) |
| 43 | 20 (18,5–24,5) | 51 (37,5–71) | 18 (9–23,5) | 6,5 (3,5–9,5) | 17 (7,5–27) | 26,5 (10–63) |
| 44 | 21 (18–24) | 50,5 (37–72) | 18 (9–23,5) | 6,5 (4–10) | 17 (8,5–27) | 28 (9,5–61,5) |
| 45 | 20,5 (17,5–24) | 51 (37–71) | 18 (9–23,5) | 6,5 (3,5–9,5) | 17 (8–27,5) | 30,5 (10–69,5) |
| 46 | 20 (18–24,5) | 51,5 (37–72) | 18 (11,5–23,5) | 6,5 (3–9,5) | 17 (7,5–28) | 29,5 (10–68,5) |
| 47 | 20 (18–24) | 50,5 (36,5–69) | 19 (11,5–23,5) | 6,5 (3–9,5) | 17 (7–28) | 27,5 (11–69) |
| 48 | 20,5 (18–23,5) | 49,5 (37,5–70,5) | 19 (11–23,5) | 6,5 (3–9,5) | 17 (7–28) | 29 (12–71) |
| 49 | 20,5 (18,5–24,5) | 51 (38,5–69) | 19 (11–23,5) | 6,5 (3–9,5) | 17 (7–25,5) | 28,5 (12–63,5) |
| 50 | 21,5 (18,5–23,5) | 50,5 (40–69,5) | 19 (11–23,5) | 6,5 (3–9,5) | 17 (7–26) | 30 (10,5–54,5) |
| 51 | 21,5 (19–24,5) | 50 (40,5–70,5) | 18,5 (11–22) | 6 (3–9,5) | 17,5 (7–26) | 30 (13–53,5) |
| 52 | 21,5 (19–24) | 50 (41–68,5) | 18,5 (11–21,5) | 6 (3–9,5) | 17,5 (7–26) | 31,5 (14–60,5) |
| 53 | 21 (18–24) | 51,5 (41–72) | 18,5 (11–21,5) | 6 (3–9,5) | 18,5 (7–26) | 29,5 (15–60) |
| 54 | 21 (18–23) | 51,5 (40,5–72,5) | 19 (10,5–22) | 6 (2,5–9,5) | 18,5 (6,5–26) | 28,5 (12–62) |
| 55 | 21 (19–23,5) | 51,5 (41–68,5) | 19 (10,5–22) | 6 (2,5–9,5) | 18,5 (6,5–27) | 30,5 (10,5–57,5) |
| 56 | 21 (18,5–23,5) | 51,5 (40,5–73) | 18,5 (10,5–22) | 6 (3–9,5) | 19 (7–27) | 33 (10–55) |
| 57 | 21,5 (19–24,5) | 51,5 (41–75) | 18,5 (10,5–22) | 6 (3–9,5) | 19 (8–28) | 33 (10–57) |
| 58 | 21,5 (19–24) | 51 (37,5–74,5) | 17,5 (12–22) | 6 (3–9,5) | 19 (8–28,5) | 32,5 (11–58,5) |
| 59 | 21 (18,5–23,5) | 50 (38,5–76,5) | 17 (13–21) | 6 (3,5–9,5) | 20 (7–28,5) | 35,5 (10,5–62) |
| 60 | 21,5 (18,5–23,5) | 50 (39,5–75) | 17 (14–21) | 6 (3,5–9,5) | 20,5 (7–28,5) | 34 (11–59) |

### v3: durum değişimi (1/2)

| Yıl | Yaşayan yerleşim (yıl ort.) | Büyük şehir (Şehir kademesi, yıl ort.) | El değiştiren yerleşim (fetih + bölünme) | El değiştiren büyük şehir | Büyük şehre hücum (kuşatma muharebesi) | Büyük şehir yağmalandı, tutulmadı |
|---|---|---|---|---|---|---|
| 1 | 77,1 (69,2–78,8) | 5 (4,16–5,5) | 4 (2–6,5) | 1 (0–2,5) | 0,5 (0–1) | 0 (0–0,5) |
| 2 | 78,4 (68,4–80) | 4,75 (4,06–5,71) | 3 (1–5,5) | 1 (0–2,5) | 0 (0–2) | 0 (0–0,5) |
| 3 | 78,5 (67,7–80) | 5,25 (4,14–6) | 4 (2–6,5) | 1 (0–3) | 0 (0–2) | 0 (0–0,5) |
| 4 | 78,4 (68,1–80) | 5,3 (3,39–5,79) | 3 (1,5–7) | 1,5 (0–2,5) | 0 (0–1,5) | 0 |
| 5 | 78 (61,7–80) | 5,03 (3,66–5,94) | 5 (1,5–7,5) | 1 (0–3) | 0 (0–3) | 0 (0–2) |
| 6 | 78 (60,4–80) | 4,78 (3,99–5,98) | 3 (1,5–5,5) | 1 (0–2) | 0 (0–1,5) | 0 (0–1) |
| 7 | 78 (57,7–79,6) | 4,96 (4,08–6) | 4 (1,5–6) | 1 (0–2) | 0 (0–1) | 0 (0–1) |
| 8 | 77,9 (58,5–79,6) | 5,29 (4,11–5,98) | 3 (1,5–5,5) | 1 (0–1) | 0 (0–1) | 0 (0–1) |
| 9 | 77,9 (59,3–80,4) | 5,36 (4–5,94) | 2 (1–5,5) | 1 (0–2) | 1 (0–2) | 0 (0–1) |
| 10 | 78 (60,2–80) | 4,78 (3,96–6) | 4 (1–6) | 1 (0–2) | 0 (0–1,5) | 0 |
| 11 | 77,8 (59,8–80) | 4,65 (3,68–5,94) | 3,5 (1,5–7) | 1 (0–3) | 0 (0–3) | 0 (0–1) |
| 12 | 78,5 (60,8–80,1) | 4,7 (3,35–6) | 2 (1–4,5) | 1 (0–2) | 0 (0–2) | 0 (0–1) |
| 13 | 78,4 (60,3–79,9) | 4,6 (3,4–5,94) | 3 (1,5–5,5) | 1 (0–2) | 0 (0–1) | 0 |
| 14 | 78,4 (60,4–80) | 5,14 (4,01–6) | 3 (0,5–5,5) | 1 (0–2) | 0 (0–2) | 0 (0–1) |
| 15 | 78,7 (60,8–80) | 5 (3,98–5,96) | 3 (1–5,5) | 1 (0–2) | 0 (0–1) | 0 |
| 16 | 79 (61,4–80) | 5,46 (4–5,98) | 3,5 (2–5,5) | 1,5 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 17 | 78,5 (63,1–80) | 5,59 (4,34–6) | 3,5 (1–6) | 1 (0–3,5) | 0 (0–2) | 0 (0–0,5) |
| 18 | 78,1 (63,7–79,9) | 5,8 (4,76–6) | 3,5 (1–4) | 1 (0–2,5) | 0 (0–0,5) | 0 |
| 19 | 78 (63,6–79,6) | 5,65 (4,48–6) | 3 (1–5,5) | 1 (0–2,5) | 0 (0–2) | 0 |
| 20 | 78 (65,2–79,6) | 5,66 (3,75–6) | 3 (1–5,5) | 1 (0–2,5) | 0 (0–2) | 0 (0–1,5) |
| 21 | 78,1 (66–79,8) | 5,73 (4,01–6) | 3 (2–5,5) | 1 (0–2) | 0 (0–2) | 0 (0–1) |
| 22 | 77,9 (67,1–79,6) | 5,85 (4,21–6) | 3 (1–4,5) | 0 (0–2) | 0 (0–2) | 0 (0–1) |
| 23 | 77,7 (66,8–79,8) | 5,94 (4,65–6) | 3 (1,5–5,5) | 1 (0–2) | 0 (0–2) | 0 (0–0,5) |
| 24 | 78,4 (64–80,1) | 5,8 (4,44–6) | 3 (2–4,5) | 1 (0–2) | 0 (0–1,5) | 0 (0–0,5) |
| 25 | 78,5 (63,1–80,1) | 5,9 (4,04–6) | 3 (0,5–6) | 1 (0–2,5) | 0 | 0 |
| 26 | 78,7 (65–80) | 5,64 (4,11–6) | 2,5 (0–4,5) | 0 (0–1) | 0 (0–0,5) | 0 (0–0,5) |
| 27 | 79 (65,3–80,1) | 5,44 (3,95–5,98) | 2,5 (1–5,5) | 1 (0–1,5) | 0 (0–1,5) | 0 (0–1) |
| 28 | 78,9 (65–80) | 5,38 (4,21–6) | 3 (1,5–4,5) | 1 (0–2) | 0 (0–1) | 0 (0–1) |
| 29 | 78 (64,7–79,9) | 5,3 (4,15–6) | 2 (0–3,5) | 0 (0–2,5) | 0 (0–1) | 0 (0–0,5) |
| 30 | 78,1 (65–79,1) | 5,94 (4,4–6) | 3 (1–5,5) | 1 (0–3) | 0 (0–1,5) | 0 |
| 31 | 78,4 (65,9–79) | 5,73 (4,64–6) | 3 (2–4,5) | 1 (0–2) | 0 (0–2) | 0 (0–1) |
| 32 | 78,1 (66,6–79) | 5,7 (4,14–6) | 2 (0,5–5,5) | 1 (0–3) | 0 (0–1) | 0 |
| 33 | 78,4 (67–79,3) | 5,69 (4,56–6) | 3 (1,5–5,5) | 1 (0–2,5) | 0 (0–2) | 0 (0–1) |
| 34 | 78 (67–80) | 5,49 (4,38–6) | 2 (0,5–3,5) | 1 (0–3) | 0,5 (0–2) | 0 (0–1) |
| 35 | 78,3 (65,3–80,1) | 5,26 (4,34–6) | 3 (1–5) | 1 (0–2) | 0 (0–2) | 0 |
| 36 | 78 (64,3–80,1) | 5,18 (3,89–6) | 3 (2–5) | 1 (0–2,5) | 0 (0–1) | 0 |
| 37 | 78,3 (65,3–80,5) | 5,26 (4,2–5,99) | 4 (1,5–5) | 1 (0–2,5) | 0 (0–2,5) | 0 |
| 38 | 78,3 (65,3–80,7) | 5,83 (4–6) | 3 (1–5) | 1 (0–3,5) | 0 (0–1) | 0 |
| 39 | 78,8 (63,5–80,5) | 5,58 (3,98–6) | 3,5 (1–5,5) | 1 (0–3) | 0 (0–1) | 0 (0–1) |
| 40 | 78,2 (64,1–80,3) | 5,71 (4,18–6) | 3 (1–4) | 0,5 (0–2) | 0 (0–0,5) | 0 |
| 41 | 78,2 (65,2–80) | 5,49 (4,09–6) | 3 (1–5) | 1 (0–2,5) | 0 (0–1,5) | 0 (0–1) |
| 42 | 77,9 (64,8–80) | 5,04 (4,49–6) | 3 (1,5–4,5) | 1 (0–2) | 0 (0–2) | 0 (0–1) |
| 43 | 77,9 (64,7–80) | 5 (4,14–6) | 2,5 (1–4,5) | 1 (0–2,5) | 0 (0–1,5) | 0 |
| 44 | 78,3 (65,9–79,9) | 5,58 (4,56–6) | 2 (1–4,5) | 1 (0–1,5) | 0 (0–2) | 0 (0–1) |
| 45 | 78,8 (66,2–80) | 5,7 (4,48–6) | 2 (1–4,5) | 1 (0–2,5) | 1 (0–2,5) | 0 (0–1) |
| 46 | 78,6 (67,1–79,5) | 5,65 (4,28–6) | 2,5 (1–4,5) | 1 (0,5–2) | 0 (0–2,5) | 0 (0–1,5) |
| 47 | 78,7 (67,5–80) | 5,71 (3,95–6) | 3,5 (2–5) | 1,5 (0–2,5) | 0 (0–2) | 0 (0–0,5) |
| 48 | 78,5 (67,5–80) | 5,29 (4,01–6) | 3 (2–5,5) | 1 (0–2) | 0 (0–1) | 0 (0–1) |
| 49 | 78,5 (66,5–80) | 5,48 (3,91–6) | 2,5 (1–4) | 1 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 50 | 78,5 (66,5–81) | 5 (3,99–6) | 3 (2–5) | 1 (0–3) | 0 (0–1,5) | 0 (0–0,5) |
| 51 | 78,3 (66,4–80,6) | 5,16 (4,06–5,98) | 3,5 (2–6) | 1,5 (0–3) | 0 (0–1,5) | 0 |
| 52 | 78,4 (67,4–80,3) | 5,38 (3,94–5,98) | 3,5 (1,5–6) | 1 (0–2,5) | 0 (0–1) | 0 (0–1) |
| 53 | 77,9 (67,1–80,3) | 5,33 (4–6) | 3 (1–4) | 1 (0–2) | 0 (0–1,5) | 0 (0–0,5) |
| 54 | 78 (66,8–80,4) | 4,7 (4–6) | 3 (1,5–7) | 1 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 55 | 77,6 (66,1–80,2) | 4,5 (3,94–5,88) | 2,5 (1–5,5) | 0,5 (0–1,5) | 0 (0–1) | 0 (0–1) |
| 56 | 77,8 (66,6–79,8) | 4,66 (3,85–5,96) | 3,5 (1–5,5) | 1 (0–2,5) | 0 (0–1) | 0 (0–0,5) |
| 57 | 78,2 (67,1–80,5) | 4,61 (3,8–5,51) | 2,5 (1–6,5) | 1 (0–2) | 0 (0–2) | 0 (0–1) |
| 58 | 78 (67,4–80,6) | 4,55 (3,68–5,08) | 3 (1–5) | 1 (0–1,5) | 0 (0–1) | 0 (0–1) |
| 59 | 77,6 (67,5–80,5) | 4,84 (3,88–5,99) | 2 (1–4) | 1 (0–1,5) | 0 (0–1) | 0 |
| 60 | 78,1 (67,5–80) | 4,99 (4,21–5,98) | 3 (2–5,5) | 1 (0–2,5) | 0 (0–1) | 0 (0–1) |

### v3: durum değişimi (2/2)

| Yıl | Yerleşim durum değişimi (kuruluş hariç hepsi) | Orta halka kaydı (Köy/Kasaba: kademe değişimi ya da terk) | Açlık başlayan yerleşim | Salgın başlayan yerleşim | Yakılan/yanan yerleşim | Harabeye yeniden yerleşim |
|---|---|---|---|---|---|---|
| 1 | 57,5 (44–76,5) | 26,5 (14–35) | 0 (0–16,5) | 0 (0–2) | 11 (5–14,5) | 1 (0–2) |
| 2 | 47 (40–65) | 22 (16–37) | 0 (0–6) | 0 (0–2) | 7 (3–11,5) | 0 (0–1,5) |
| 3 | 47 (36,5–64,5) | 20,5 (12,5–30) | 0 (0–13,5) | 1 (0–1) | 8 (3,5–11,5) | 0 (0–1) |
| 4 | 50 (37,5–75) | 24 (14–30,5) | 0 (0–19,5) | 0 (0–1) | 9,5 (6–13) | 0 (0–0,5) |
| 5 | 60 (37,5–72) | 21,5 (12–31,5) | 0 (0–23) | 0 (0–2) | 10,5 (1,5–19) | 0 |
| 6 | 48 (26,5–75,5) | 20 (11–27) | 0 (0–30,5) | 0 (0–1) | 6 (4,5–10,5) | 0 (0–1) |
| 7 | 48,5 (31–73,5) | 19,5 (8,5–26,5) | 0 (0–11,5) | 0 (0–2) | 10 (4–19) | 0 (0–1) |
| 8 | 47,5 (29–60,5) | 21,5 (9–26,5) | 0 (0–14,5) | 0 (0–1,5) | 9 (4,5–12) | 0,5 (0–1,5) |
| 9 | 41,5 (28,5–65,5) | 20 (13–26,5) | 0 (0–14) | 0,5 (0–2) | 5,5 (2–13,5) | 0 (0–2,5) |
| 10 | 44,5 (29–59) | 21 (5,5–30,5) | 0 (0–3,5) | 0 (0–1) | 8,5 (4,5–12,5) | 0,5 (0–2,5) |
| 11 | 43,5 (28,5–73,5) | 20,5 (8–37,5) | 0 (0–3) | 0 (0–1,5) | 7,5 (3,5–10) | 0 (0–1,5) |
| 12 | 49,5 (23–64,5) | 20,5 (6–34,5) | 0 (0–12,5) | 0 (0–1) | 8,5 (2,5–15) | 0 (0–1,5) |
| 13 | 46,5 (39,5–61,5) | 21 (14–36) | 0 (0–15) | 0 (0–1,5) | 7,5 (4–12) | 0 (0–1) |
| 14 | 46 (34–65) | 22 (10,5–29,5) | 0 (0–8) | 0 (0–2) | 8,5 (5,5–14) | 0 (0–2,5) |
| 15 | 52,5 (34–66) | 24,5 (16–31,5) | 0 (0–10,5) | 0 (0–1,5) | 10,5 (5,5–14) | 0 (0–2) |
| 16 | 45,5 (33–56) | 19,5 (10–30,5) | 0 (0–15,5) | 1 (0–2) | 9 (5,5–14) | 0 (0–1) |
| 17 | 51,5 (35–64,5) | 24,5 (11–30,5) | 0 (0–16) | 0,5 (0–2) | 11 (6,5–15,5) | 0 (0–3) |
| 18 | 43 (39–66,5) | 20,5 (12,5–27) | 0 (0–14,5) | 0 (0–1) | 8,5 (6,5–14,5) | 0 (0–2) |
| 19 | 45 (33,5–65) | 22 (13,5–28) | 0 (0–7,5) | 0 (0–2,5) | 8,5 (5–14,5) | 0 (0–1) |
| 20 | 41 (28–68) | 18,5 (12–23,5) | 0 (0–7,5) | 0 (0–2,5) | 9 (2,5–15,5) | 0 (0–1) |
| 21 | 50 (38–62,5) | 22 (12,5–34) | 0 (0–12) | 1 (0–3) | 7,5 (6–12,5) | 1 (0–2) |
| 22 | 50 (29–71,5) | 18 (13–30,5) | 0 (0–24,5) | 0 (0–2) | 8 (4–15,5) | 0 (0–0,5) |
| 23 | 46 (35–76,5) | 18,5 (9–28) | 0 (0–36,5) | 0,5 (0–2) | 7,5 (3,5–13,5) | 0,5 (0–2,5) |
| 24 | 47 (34–64) | 22,5 (15–25,5) | 0 (0–11) | 0 (0–1,5) | 9,5 (3,5–13,5) | 0 (0–1,5) |
| 25 | 40,5 (30–62) | 20,5 (12–31,5) | 0 (0–4) | 0 (0–1) | 6,5 (3,5–13,5) | 0,5 (0–2) |
| 26 | 40,5 (23–60) | 19,5 (9–29) | 0 (0–4) | 0 (0–2) | 7,5 (2–15,5) | 0 (0–1,5) |
| 27 | 41,5 (34–57) | 19 (10,5–28,5) | 0 (0–14) | 0 (0–2) | 8,5 (4–16,5) | 0 (0–1,5) |
| 28 | 41 (33–59,5) | 20 (10–27,5) | 0 (0–8) | 1 (0–1,5) | 10,5 (5–14,5) | 0 (0–2) |
| 29 | 41,5 (33–56,5) | 19 (11,5–26,5) | 0 (0–14,5) | 1 (0–1) | 9,5 (3–14) | 0,5 (0–2) |
| 30 | 37 (27–62) | 16,5 (12,5–25,5) | 0 (0–10,5) | 0 (0–3) | 6,5 (2,5–14) | 0 (0–2,5) |
| 31 | 47 (35–55) | 20,5 (12,5–28,5) | 0,5 (0–5,5) | 0 (0–0,5) | 9 (4,5–14,5) | 0 (0–1) |
| 32 | 36,5 (28–54) | 18 (10,5–27) | 0 (0–6,5) | 1,5 (0–3) | 7 (3–15) | 0 (0–1) |
| 33 | 46,5 (39,5–68) | 21 (13,5–31) | 0 (0–11) | 0,5 (0–2,5) | 9,5 (5–17) | 0 (0–0,5) |
| 34 | 47,5 (30,5–82) | 20 (14–26,5) | 1 (0–37) | 0 (0–1) | 7 (4,5–12) | 0,5 (0–1,5) |
| 35 | 48,5 (30–81,5) | 18 (11–27,5) | 1 (0–24) | 1 (0–2) | 9,5 (3,5–15) | 0 (0–1) |
| 36 | 42 (32–60,5) | 15 (11,5–29,5) | 0,5 (0–9,5) | 0 (0–1,5) | 9,5 (5–17) | 1 (0–2) |
| 37 | 53 (40,5–65) | 19,5 (13,5–25) | 2 (0–21,5) | 0 (0–1) | 11 (5–15) | 0 (0–2) |
| 38 | 44 (29,5–56) | 18,5 (12–25) | 1 (0–8) | 1 (0–2) | 8,5 (5,5–15) | 0 (0–2) |
| 39 | 47 (27–57) | 19 (10,5–27) | 1,5 (0–14,5) | 0 (0–1) | 9,5 (4–16) | 0 (0–1) |
| 40 | 36,5 (22–66,5) | 14,5 (7–24) | 0 (0–10,5) | 0 (0–1) | 8 (4,5–19,5) | 1 (0–2,5) |
| 41 | 41 (25,5–75,5) | 12 (8–22) | 1 (0–33) | 0 (0–2) | 8,5 (2–16,5) | 0 (0–1,5) |
| 42 | 49,5 (28–65) | 20 (10,5–28) | 1,5 (0–21) | 0 (0–2) | 9 (5–13) | 0 (0–1) |
| 43 | 38 (29–64,5) | 16 (8,5–26) | 1,5 (0–29,5) | 0 (0–1) | 6 (2–11,5) | 0 (0–3) |
| 44 | 41,5 (31–69,5) | 15,5 (7–23) | 0 (0–24,5) | 1 (0–3) | 10 (2,5–20) | 0 (0–3,5) |
| 45 | 43,5 (23,5–52,5) | 18,5 (12,5–25) | 0 (0–6) | 0 (0–1) | 8,5 (3,5–12) | 0 (0–1,5) |
| 46 | 40 (24–56,5) | 16 (7–21) | 0,5 (0–13) | 0 (0–1) | 7 (3,5–18) | 0 (0–1,5) |
| 47 | 48 (20,5–67) | 15,5 (8–27,5) | 0 (0–15,5) | 0 (0–1,5) | 10 (4,5–16,5) | 0 (0–2) |
| 48 | 41,5 (23,5–66) | 17 (8,5–24) | 0 (0–14,5) | 0 (0–3) | 10,5 (3,5–17,5) | 0 (0–1) |
| 49 | 41,5 (16–53) | 14 (6–21,5) | 0 (0–12,5) | 0 (0–1) | 10,5 (2,5–18) | 0 (0–1) |
| 50 | 37 (27,5–61) | 15 (9–23,5) | 1 (0–26,5) | 0 (0–1,5) | 7,5 (3,5–15) | 0 (0–1) |
| 51 | 42 (29–73,5) | 14 (8–25,5) | 0 (0–31) | 0 (0–2,5) | 8 (3–19) | 0 (0–1,5) |
| 52 | 40,5 (24–59) | 14,5 (8–20,5) | 0 (0–10) | 0 (0–1) | 9,5 (3–13) | 0 (0–1) |
| 53 | 45,5 (23,5–61,5) | 18 (5–28) | 0 (0–27,5) | 0,5 (0–1,5) | 6,5 (3–15) | 0,5 (0–1) |
| 54 | 43 (24–69,5) | 16,5 (5,5–26) | 0 (0–25) | 0 (0–0,5) | 8,5 (4–14,5) | 0 (0–2,5) |
| 55 | 40 (27–77) | 17 (8–25,5) | 0 (0–15) | 0 (0–2,5) | 7,5 (3–18) | 0 (0–3) |
| 56 | 43 (25–70,5) | 15,5 (8,5–24,5) | 1 (0–30,5) | 0 (0–1) | 8 (5–16) | 0,5 (0–1,5) |
| 57 | 45,5 (25,5–75,5) | 12,5 (5,5–23) | 3 (0–30,5) | 0 (0–1) | 7 (4,5–20,5) | 0 (0–1) |
| 58 | 37,5 (27,5–81,5) | 12,5 (7,5–22) | 4 (0–29) | 0 (0–1,5) | 9 (4,5–16,5) | 0 (0–1) |
| 59 | 36 (28–75,5) | 13,5 (9,5–22) | 0 (0–25,5) | 1 (0–2) | 8 (3,5–12,5) | 0 (0–3) |
| 60 | 43 (21–75) | 14,5 (8–28) | 0,5 (0–26) | 1 (0–2) | 8,5 (4–16) | 0 (0–0,5) |

### Devlet, inanç ve örgüt (1/5)

| Yıl | Yaşayan örgüt (yıl sonu) | Örgüt şubesi (yıl sonu) | Gizli şube (yıl sonu) | Örgüt üyesi (yıl sonu) | Açılan şube | Kapanan şube |
|---|---|---|---|---|---|---|
| 1 | 13 (12,5–13) | 306 (208–381) | 15,5 (10–26) | 1304 (1166–1576) | 21,5 (17–24,5) | 22 (13–29) |
| 2 | 13 (12–13) | 297 (208–364) | 11,5 (9,5–22) | 1291 (1133–1544) | 21 (16,5–23,5) | 27,5 (18–45) |
| 3 | 13 (12–13) | 297 (186–340) | 11 (7,5–22,5) | 1294 (1086–1527) | 15 (10,5–20,5) | 22 (15–37) |
| 4 | 13 (12–13) | 294 (179–329) | 11 (6,5–21) | 1312 (1094–1506) | 16,5 (12,5–19) | 20 (14,5–32) |
| 5 | 13 (12–13) | 291 (184–324) | 9,5 (6–17,5) | 1280 (1104–1545) | 17 (13,5–20) | 20 (11,5–29) |
| 6 | 13 (12–13) | 288 (186–325) | 11 (6,5–17) | 1265 (1075–1542) | 15,5 (11,5–19) | 15 (10–19,5) |
| 7 | 13 (12,5–13) | 286 (192–325) | 11 (6,5–16) | 1289 (1058–1580) | 16 (15–18,5) | 16 (11–22) |
| 8 | 13 | 290 (200–330) | 11,5 (6,5–16,5) | 1302 (1060–1611) | 15 (11,5–22) | 12 (6–19,5) |
| 9 | 13 | 290 (211–332) | 9,5 (6–14) | 1306 (1070–1630) | 16 (11,5–21,5) | 13 (8–20,5) |
| 10 | 13 | 290 (212–338) | 10 (6–12,5) | 1308 (1068–1632) | 15,5 (10,5–20) | 14,5 (9–22) |
| 11 | 13 (12,5–13) | 294 (211–335) | 9 (5–13) | 1329 (1058–1659) | 15 (13,5–20) | 13 (7–24) |
| 12 | 13 (12–13) | 292 (218–339) | 7,5 (5–12) | 1336 (1087–1658) | 16 (14–20) | 13,5 (9–19,5) |
| 13 | 13 (12–13) | 292 (224–335) | 7 (5,5–10,5) | 1360 (1121–1648) | 15 (11,5–18,5) | 13,5 (8,5–24,5) |
| 14 | 13 (12–13) | 288 (224–334) | 7,5 (5–10,5) | 1349 (1140–1644) | 16 (12–20,5) | 15 (7,5–24,5) |
| 15 | 13 (12–13) | 290 (228–334) | 8 (4–9,5) | 1357 (1172–1662) | 16 (13–20) | 14 (8,5–21) |
| 16 | 13 (12–13) | 291 (234–336) | 7,5 (4,5–10) | 1393 (1176–1657) | 15 (12–19) | 13 (8,5–21) |
| 17 | 13 (12,5–13) | 291 (238–340) | 8 (4,5–11,5) | 1404 (1172–1664) | 15,5 (10,5–17,5) | 14,5 (10,5–19) |
| 18 | 13 (12–13) | 286 (246–339) | 8 (3,5–11) | 1435 (1164–1640) | 16 (11–20,5) | 16 (6–22) |
| 19 | 13 (12–13) | 290 (240–344) | 7,5 (4–12,5) | 1453 (1146–1664) | 15 (11–21) | 12,5 (9–24,5) |
| 20 | 13 (12–13) | 292 (236–350) | 8 (3–11,5) | 1447 (1135–1696) | 16 (14,5–19) | 15 (9–19) |
| 21 | 13 (12–13) | 286 (244–351) | 8,5 (4,5–11,5) | 1463 (1156–1718) | 17 (11–20) | 14 (6,5–22) |
| 22 | 13 (12–13) | 291 (248–358) | 8 (4,5–11) | 1477 (1185–1717) | 18 (14–20,5) | 11,5 (7–21,5) |
| 23 | 13 (12–13) | 291 (243–368) | 8 (5–12) | 1512 (1165–1707) | 16,5 (13–20) | 12,5 (7–22,5) |
| 24 | 13 (12–13) | 288 (239–368) | 7,5 (4,5–11,5) | 1516 (1158–1728) | 16,5 (11,5–20,5) | 14 (10–24) |
| 25 | 13 (12–13) | 290 (242–375) | 8 (5–13,5) | 1473 (1193–1733) | 17 (13–22,5) | 13 (9–22) |
| 26 | 13 (12–13) | 292 (248–378) | 7 (4,5–11,5) | 1483 (1184–1727) | 16 (11–20,5) | 12,5 (9–21) |
| 27 | 13 (12–13) | 298 (249–377) | 8,5 (3,5–12) | 1483 (1180–1736) | 16,5 (12,5–20,5) | 14,5 (7–21,5) |
| 28 | 13 (12–13) | 301 (242–380) | 8,5 (4,5–12,5) | 1500 (1182–1742) | 17 (9–19,5) | 13,5 (7–18,5) |
| 29 | 13 (12–13) | 305 (240–381) | 7,5 (4,5–11,5) | 1487 (1188–1744) | 16,5 (12–20,5) | 15 (11–24) |
| 30 | 13 (12–13) | 306 (242–385) | 8 (4,5–12,5) | 1501 (1197–1790) | 16,5 (11–20,5) | 12,5 (9,5–19) |
| 31 | 13 (12–13) | 309 (246–379) | 8 (5–12,5) | 1538 (1214–1782) | 16 (11,5–20) | 13 (10–19) |
| 32 | 13 (12–13) | 309 (250–380) | 7 (4,5–12) | 1548 (1230–1814) | 17 (13–20,5) | 13 (10–24) |
| 33 | 13 (12–13) | 315 (248–381) | 8,5 (4–12,5) | 1562 (1222–1818) | 15 (12,5–19,5) | 15 (10,5–20) |
| 34 | 13 (12–13) | 313 (248–378) | 6,5 (4–11) | 1567 (1234–1810) | 15,5 (12–19,5) | 17,5 (11–27) |
| 35 | 13 (12–13) | 316 (240–382) | 8 (4–12) | 1548 (1222–1838) | 18,5 (14–21) | 17 (12,5–22) |
| 36 | 13 (12–13) | 320 (237–382) | 8 (5–11) | 1534 (1236–1880) | 16,5 (12,5–20) | 15,5 (9,5–23,5) |
| 37 | 13 (12–13) | 326 (242–380) | 7,5 (4–12) | 1562 (1240–1867) | 15 (10,5–20,5) | 14,5 (8–19) |
| 38 | 13 (12–13) | 325 (237–378) | 6,5 (4–11,5) | 1538 (1258–1881) | 15,5 (11–20,5) | 19 (7–22,5) |
| 39 | 13 (12–13) | 322 (238–382) | 6,5 (4–10) | 1528 (1243–1903) | 15 (12–22) | 13,5 (8,5–23,5) |
| 40 | 13 (12–13) | 324 (245–374) | 6,5 (4,5–10) | 1548 (1276–1904) | 14,5 (11–19,5) | 13,5 (8,5–22,5) |
| 41 | 13 (12–13) | 326 (243–373) | 6,5 (4–10) | 1552 (1260–1934) | 16,5 (12,5–21,5) | 12,5 (6–21,5) |
| 42 | 13 (12–13) | 330 (243–376) | 6,5 (3,5–9,5) | 1541 (1254–1888) | 16,5 (12–20,5) | 15 (8,5–22) |
| 43 | 13 (12–13) | 320 (242–378) | 6 (4–10) | 1552 (1262–1859) | 15 (12–20) | 16 (9–22) |
| 44 | 13 (12–13) | 326 (247–382) | 7 (4,5–11) | 1550 (1290–1884) | 15,5 (11,5–18) | 11,5 (7–16) |
| 45 | 13 (12–13) | 327 (246–385) | 6,5 (4,5–11,5) | 1541 (1270–1840) | 15 (12–22,5) | 14,5 (9–21,5) |
| 46 | 13 (12–13) | 332 (246–388) | 7 (3–10) | 1544 (1276–1848) | 17,5 (8,5–20,5) | 15 (10–22,5) |
| 47 | 13 (12–13) | 336 (248–387) | 7 (4–11) | 1546 (1294–1856) | 15 (13,5–21) | 12 (7–21,5) |
| 48 | 12,5 (12–13) | 336 (250–380) | 7 (4–11,5) | 1587 (1273–1848) | 15,5 (10–19,5) | 12,5 (8–25) |
| 49 | 13 (12–13) | 330 (248–384) | 6,5 (4,5–10) | 1566 (1266–1836) | 15,5 (11,5–19) | 15,5 (9,5–19) |
| 50 | 12,5 (12–13) | 330 (248–386) | 6,5 (3–12,5) | 1574 (1268–1810) | 14 (9,5–20) | 15,5 (9,5–18,5) |
| 51 | 13 (12–13) | 332 (242–382) | 7 (2,5–9) | 1577 (1264–1818) | 13,5 (10,5–17) | 14 (8,5–21) |
| 52 | 13 (12–13) | 336 (247–380) | 7 (2,5–10,5) | 1563 (1260–1839) | 15 (11–18) | 12,5 (6,5–19) |
| 53 | 13 (12–13) | 327 (242–384) | 6,5 (2,5–8,5) | 1544 (1280–1838) | 14 (8,5–21) | 15,5 (11–27) |
| 54 | 13 (12–13) | 320 (240–378) | 6 (3–10) | 1536 (1282–1866) | 13 (10,5–16) | 15,5 (10–23,5) |
| 55 | 12,5 (12–13) | 322 (246–380) | 5,5 (2,5–10,5) | 1533 (1259–1864) | 17 (12–19) | 15 (7–19) |
| 56 | 13 (12–13) | 324 (252–374) | 7 (3–12) | 1532 (1274–1883) | 15 (12–19) | 16 (8–24) |
| 57 | 13 (12–13) | 322 (252–376) | 6,5 (2,5–11) | 1524 (1290–1890) | 15 (9–18,5) | 13,5 (7–22,5) |
| 58 | 13 (12–13) | 324 (256–378) | 7 (4–11) | 1512 (1294–1872) | 15,5 (11–19) | 15 (4–18) |
| 59 | 13 (12–13) | 326 (259–386) | 6,5 (4,5–13) | 1550 (1288–1878) | 16 (9–20) | 13,5 (9,5–18,5) |
| 60 | 13 (12–13) | 329 (262–395) | 7,5 (4–11) | 1572 (1286–1896) | 14,5 (9,5–19) | 12 (6–18) |

### Devlet, inanç ve örgüt (2/5)

| Yıl | Dağılan örgüt | Yeniden kurulan örgüt | Gölge savaşı eylemi | Gölge savaşında öldürülen usta ya da lider | Gizli şubeye baskın | Lobiyle yasa değişikliği |
|---|---|---|---|---|---|---|
| 1 | 0 (0–0,5) | 0 | 7 (5–10) | 1 (0–4) | 1 (0–2,5) | 1 (0,5–3) |
| 2 | 0 | 0 | 7,5 (6–11,5) | 2 (0,5–4) | 1 (0–2) | 1 (0–2) |
| 3 | 0 | 0 | 8 (4–13,5) | 2 (0–4) | 1 (0–2,5) | 1 (0–2,5) |
| 4 | 0 | 0 | 7 (4,5–11,5) | 1 (0–3) | 1 (0–3) | 1 (0–1,5) |
| 5 | 0 | 0 | 7,5 (3–12) | 1 (0–2,5) | 1 (0–2) | 0 (0–2,5) |
| 6 | 0 | 0 | 7,5 (4–10) | 1,5 (0–4) | 0,5 (0–2) | 0 (0–2,5) |
| 7 | 0 | 0 | 7 (4–10,5) | 1 (0,5–2,5) | 0 (0–1) | 1 (0–2,5) |
| 8 | 0 | 0 | 7,5 (5,5–11,5) | 1 (0–3) | 0 (0–1) | 0,5 (0–3) |
| 9 | 0 | 0 | 6 (3–9) | 1 (0–2,5) | 1 (0–2) | 1 (0–2) |
| 10 | 0 | 0 | 7 (4–10,5) | 1 (0–2,5) | 0 (0–1,5) | 0,5 (0–2) |
| 11 | 0 | 0 | 7 (3–10,5) | 1 (1–3,5) | 0,5 (0–2) | 1 (0–3) |
| 12 | 0 (0–0,5) | 0 | 7 (5–9) | 1,5 (0,5–4) | 0 (0–2,5) | 1,5 (0–2,5) |
| 13 | 0 | 0 | 7,5 (2,5–12,5) | 3 (1–3,5) | 0 (0–1) | 0,5 (0–2) |
| 14 | 0 | 0 | 6 (4,5–9,5) | 1 (0–2) | 0 (0–1) | 1 (0–2) |
| 15 | 0 | 0 | 8 (3,5–9,5) | 1,5 (0–2,5) | 1 (0–1,5) | 1 (0–2) |
| 16 | 0 | 0 | 7 (3–11) | 1 (0–3) | 1 (0–2) | 1 (0–2) |
| 17 | 0 | 0 (0–0,5) | 8 (4–10) | 1,5 (0,5–3,5) | 1 (0–1,5) | 1 (0–2,5) |
| 18 | 0 (0–1) | 0 | 6,5 (2–9,5) | 1 (0–2,5) | 0,5 (0–2) | 1,5 (0–3) |
| 19 | 0 (0–0,5) | 0 | 6,5 (1,5–11,5) | 1 (0–3) | 0 (0–2) | 0,5 (0–3) |
| 20 | 0 | 0 | 6 (3,5–9,5) | 1 (0–2) | 0 (0–1,5) | 1 (0–2,5) |
| 21 | 0 | 0 | 7 (2,5–9,5) | 1 (0–3,5) | 0 (0–2) | 1 (0–2) |
| 22 | 0 | 0 | 8 (3,5–10) | 1 (0–3,5) | 0 (0–1) | 1 (0–3) |
| 23 | 0 | 0 | 7 (2,5–10,5) | 1 (0–2,5) | 0 (0–1) | 1 (0–3) |
| 24 | 0 | 0 (0–0,5) | 6 (3–9) | 1 (0–2) | 1 (0–1,5) | 1 (0–2,5) |
| 25 | 0 | 0 | 6 (3–10) | 1 (0–3) | 1 (0–2) | 1 (0–2) |
| 26 | 0 | 0 (0–0,5) | 7,5 (2,5–10) | 1 (0–3) | 0 (0–1) | 1 (0–3) |
| 27 | 0 (0–0,5) | 0 | 8 (3–11,5) | 1 (0–3,5) | 0,5 (0–1) | 1 (0–2,5) |
| 28 | 0 | 0 | 8 (3–10) | 2 (0,5–2) | 0 (0–1) | 1 (0–2) |
| 29 | 0 | 0 | 6 (2,5–11,5) | 1,5 (0–4) | 0 (0–2) | 1 (0–2) |
| 30 | 0 | 0 | 6 (2–10) | 1,5 (0,5–3,5) | 0 (0–1) | 1 (0–2) |
| 31 | 0 | 0 | 5,5 (4–10) | 0,5 (0–2,5) | 0 (0–1,5) | 1 (0–3) |
| 32 | 0 | 0 | 5 (3–10) | 1 (0–2) | 1 (0–1) | 1 (0–3) |
| 33 | 0 (0–0,5) | 0 (0–0,5) | 5,5 (3–9) | 1 (0–2) | 0 (0–1,5) | 0,5 (0–1,5) |
| 34 | 0 | 0 (0–0,5) | 6 (4–8,5) | 1 (0,5–3,5) | 1 (0–1,5) | 0,5 (0–2,5) |
| 35 | 0 | 0 | 6 (3,5–10,5) | 1 (0–2,5) | 0 (0–1) | 0 (0–3,5) |
| 36 | 0 | 0 | 7 (4–10,5) | 2 (0–4,5) | 0 (0–2) | 1 (0–2,5) |
| 37 | 0 (0–0,5) | 0 | 6 (3–8,5) | 1 (0–2,5) | 0 (0–1) | 1 (0–3,5) |
| 38 | 0 | 0 | 6 (3–10) | 1 (0–3) | 1 (0–2) | 1 (0–4) |
| 39 | 0 (0–0,5) | 0 (0–1) | 5 (2,5–9) | 1 (0–2) | 0 (0–1) | 1 (0–2) |
| 40 | 0 | 0 | 5 (4–10) | 1 (0–2,5) | 0 (0–1,5) | 1 (0–2,5) |
| 41 | 0 | 0 | 5 (4–10,5) | 1 (1–3) | 0 (0–1,5) | 1 (0–3,5) |
| 42 | 0 | 0 | 6 (3,5–9) | 0,5 (0–3) | 0 (0–1) | 1 (0–3) |
| 43 | 0 | 0 | 5 (2–9,5) | 1 (0–2,5) | 0 (0–1,5) | 0,5 (0–3) |
| 44 | 0 | 0 | 5,5 (2,5–10,5) | 1 (0–2,5) | 0 (0–1) | 1 (0–2,5) |
| 45 | 0 | 0 | 5,5 (3–9,5) | 1 (0–3) | 1 (0–2) | 1,5 (0–3) |
| 46 | 0 (0–0,5) | 0 (0–0,5) | 7 (3–10) | 2 (0–3) | 0,5 (0–2) | 0 (0–2) |
| 47 | 0 | 0 | 6,5 (3–11) | 2 (0,5–3) | 0 (0–1) | 1 (0–3) |
| 48 | 0 (0–0,5) | 0 | 6,5 (2,5–9,5) | 2 (0–2,5) | 0 (0–0,5) | 1 (0–3,5) |
| 49 | 0 | 0 (0–0,5) | 6 (3–10) | 1 (0–3) | 0 (0–2) | 2 (0–3) |
| 50 | 0 (0–0,5) | 0 | 5,5 (2,5–7,5) | 1 (0–3) | 0 (0–1) | 1 (0–2) |
| 51 | 0 | 0 | 6,5 (4–9,5) | 1,5 (0,5–3) | 0 (0–1) | 1 (0–3) |
| 52 | 0 | 0 | 6 (3–9,5) | 1 (0–2) | 0 (0–1,5) | 1 (0–3,5) |
| 53 | 0 | 0 | 7 (2,5–11) | 1 (0–2,5) | 0 (0–1) | 1,5 (0–3) |
| 54 | 0 (0–0,5) | 0 (0–0,5) | 7 (3,5–10) | 1 (1–3) | 0 (0–2) | 1 (0–3) |
| 55 | 0 (0–0,5) | 0 | 5,5 (4–9,5) | 1 (0–3) | 0 (0–1) | 1 (0–2,5) |
| 56 | 0 | 0 (0–0,5) | 7 (4–9,5) | 1 (0,5–2,5) | 0 (0–1) | 1,5 (0–4) |
| 57 | 0 | 0 (0–0,5) | 7,5 (4–9,5) | 2 (0–3) | 0 (0–1,5) | 2 (0–4) |
| 58 | 0 | 0 (0–0,5) | 6 (3,5–8,5) | 1 (0–2,5) | 0 (0–1) | 2 (0,5–4,5) |
| 59 | 0 | 0 | 6,5 (4–9,5) | 2 (0–3,5) | 0,5 (0–2) | 2 (0–3) |
| 60 | 0 (0–0,5) | 0 | 5,5 (4–8,5) | 1 (0–3) | 0 (0–1) | 2 (0–4) |

### Devlet, inanç ve örgüt (3/5)

| Yıl | Darbe girişimi | Örgüt ilanı (Avcılar) | Örgüt üyesi kahraman payı (yıl sonu) | Yönetici değişimi | Veraset krizi | Meşruiyet ortalaması (yıl sonu) |
|---|---|---|---|---|---|---|
| 1 | 0 | 2 (1–3) | %100 (%99–%100) | 1 (0–2,5) | 0 | 62,7 (57,9–68) |
| 2 | 0 | 0 (0–2) | %100 (%99–%100) | 1 (0–2,5) | 0 | 64,5 (58,1–69,2) |
| 3 | 0 | 0,5 (0–2) | %100 (%99–%100) | 1 (0–3) | 0 | 62,3 (56–71,9) |
| 4 | 0 | 1 (0–1,5) | %100 (%99–%100) | 1,5 (0–3,5) | 0 | 59,9 (53,6–72,4) |
| 5 | 0 | 1 (0–2) | %100 (%99–%100) | 1,5 (0–3,5) | 0 | 60,4 (52,5–70,4) |
| 6 | 0 | 1 (0–1) | %100 (%99–%100) | 1 (0–3) | 0 | 62,2 (57,1–68) |
| 7 | 0 | 0,5 (0–2) | %100 (%99–%100) | 1 (0–2) | 0 | 61,3 (57,5–69,1) |
| 8 | 0 | 0,5 (0–2) | %100 (%99–%100) | 1 (0–2,5) | 0 | 63 (58,5–67,9) |
| 9 | 0 | 0,5 (0–1) | %100 (%99–%100) | 1 (0–2,5) | 0 | 64,3 (59–69,6) |
| 10 | 0 | 1 (0–2) | %100 (%99–%100) | 1 (0–2) | 0 | 63,7 (60–70) |
| 11 | 0 | 1 (0–1) | %100 (%99–%100) | 1 (0–4) | 0 | 62,9 (56,2–67,8) |
| 12 | 0 | 1 (0–2) | %100 (%99–%100) | 1 (0–2) | 0 | 63,7 (58,6–67,9) |
| 13 | 0 (0–1) | 0,5 (0–1,5) | %100 (%99–%100) | 1 (0,5–2) | 0 | 63,1 (59,8–66,6) |
| 14 | 0 | 1 (0–1) | %100 (%99–%100) | 1 (0–2,5) | 0 | 63,8 (61,2–66,7) |
| 15 | 0 | 0,5 (0–1,5) | %100 (%99–%100) | 1 (0–2) | 0 | 65,1 (62,4–70) |
| 16 | 0 | 1 (0–1) | %100 (%99–%100) | 2 (0–3) | 0 | 62,8 (57–69,6) |
| 17 | 0 (0–1) | 1 (0–2,5) | %100 (%99–%100) | 1 (0–3,5) | 0 | 62,8 (59,5–69,8) |
| 18 | 0 (0–0,5) | 1 (0–3) | %100 (%99–%100) | 1 (0–2) | 0 | 63,9 (59,2–69,5) |
| 19 | 0 | 1 (0–2) | %100 (%99–%100) | 1,5 (0–3) | 0 | 63,3 (59,3–69,9) |
| 20 | 0 (0–0,5) | 1 (0–1,5) | %100 (%99–%100) | 1 (0–3) | 0 | 64,7 (57,1–66,4) |
| 21 | 0 | 0 (0–1) | %100 (%99–%100) | 1 (0–2,5) | 0 | 65,1 (57,8–68,8) |
| 22 | 0 | 1 (0–1,5) | %100 (%99–%100) | 0,5 (0–1,5) | 0 | 65,9 (60,3–71,4) |
| 23 | 0 | 1 (0–1,5) | %100 (%99–%100) | 2 (0–3) | 0 | 63,8 (56–68) |
| 24 | 0 (0–0,5) | 1 (0–1,5) | %100 (%99–%100) | 1 (0–2) | 0 | 62,3 (57,3–70,2) |
| 25 | 0 (0–0,5) | 1 (0–1,5) | %100 (%99–%100) | 1 (0–3,5) | 0 | 64,4 (57,2–70,9) |
| 26 | 0 | 1 (0–1) | %99 (%99–%100) | 0,5 (0–3,5) | 0 | 65,2 (58,8–70,5) |
| 27 | 0 | 0 (0–1) | %99 (%99–%100) | 1 (0–2,5) | 0 | 64,8 (59,1–70,9) |
| 28 | 0 | 1 (0–1) | %100 (%99–%100) | 1 (0–2,5) | 0 | 63,8 (58,2–72,5) |
| 29 | 0 | 1 (0–1) | %99 (%99–%100) | 0 (0–3) | 0 | 64,4 (61–72,2) |
| 30 | 0 | 1 (0–1) | %99 (%99–%100) | 1 (0–3) | 0 | 63,6 (59,7–70,6) |
| 31 | 0 | 1 (0–1) | %100 (%99–%100) | 1 (0–2) | 0 | 65,1 (59,8–71,1) |
| 32 | 0 | 0 (0–2) | %99 (%99–%100) | 1 (0–3) | 0 | 66 (61,9–67,7) |
| 33 | 0 | 1 (0–3) | %100 (%99–%100) | 1,5 (0–2,5) | 0 | 65,8 (59,7–69,5) |
| 34 | 0 | 1 (0–1,5) | %100 (%99–%100) | 1 (0–2,5) | 0 | 65,5 (59–70,2) |
| 35 | 0 | 1 (0–2) | %100 (%99–%100) | 1 (0,5–2) | 0 | 63,5 (58,4–69,8) |
| 36 | 0 | 0 (0–1) | %100 (%99–%100) | 1 (0,5–3) | 0 | 65,2 (57,3–70,7) |
| 37 | 0 | 1 (0–1,5) | %100 (%99–%100) | 1,5 (0–2) | 0 | 63 (55–71,5) |
| 38 | 0 | 1 (0–1,5) | %99 (%99–%100) | 1 (0–3) | 0 | 62,9 (55,8–70,8) |
| 39 | 0 | 1 (0–1) | %99 (%99–%100) | 1,5 (0–3) | 0 | 63,2 (58,6–70,1) |
| 40 | 0 (0–0,5) | 1 (0–1,5) | %99 (%99–%100) | 1 (0–2,5) | 0 | 65,4 (58,8–71,4) |
| 41 | 0 | 1 (0–2) | %99 (%99–%100) | 2 (0–3) | 0 | 63,1 (56,6–68) |
| 42 | 0 (0–0,5) | 1 (0–2) | %99 (%98–%100) | 1 (0–2) | 0 | 65,2 (59,3–68,5) |
| 43 | 0 | 1 (0–2) | %99 (%98–%100) | 1 (0,5–2,5) | 0 (0–0,5) | 65,3 (61–69,7) |
| 44 | 0 | 1 (0–2) | %99 (%98–%100) | 1,5 (0–2) | 0 | 65 (60,7–69,3) |
| 45 | 0 | 1 (0–1,5) | %99 (%98–%100) | 1 (0–2,5) | 0 | 64,1 (59–69,6) |
| 46 | 0 (0–0,5) | 1 (0–1,5) | %99 (%98–%100) | 1 (0–2) | 0 | 63,3 (57,9–73,5) |
| 47 | 0 | 1 (0–1,5) | %100 (%99–%100) | 1 (0–3) | 0 | 64,2 (57,5–69,7) |
| 48 | 0 | 1 (0–1,5) | %100 (%99–%100) | 1 (0,5–2) | 0 | 63,9 (60–68,7) |
| 49 | 0 | 1 (0–2) | %100 (%99–%100) | 1 (0,5–2,5) | 0 | 64,7 (58,6–70,4) |
| 50 | 0 | 1 (0–2) | %100 (%99–%100) | 2 (1–3) | 0 | 64 (57,6–70,2) |
| 51 | 0 | 1 (0–1) | %100 (%99–%100) | 2 (0,5–3,5) | 0 | 61,9 (56,5–67,7) |
| 52 | 0 | 1 (0–2,5) | %100 (%99–%100) | 2 (0–3,5) | 0 | 61 (56,6–69,3) |
| 53 | 0 | 0 (0–1,5) | %100 (%99–%100) | 1 (0–3) | 0 | 63,4 (55,3–69,6) |
| 54 | 0 | 1 (1–2) | %99 (%99–%100) | 1 (0–3) | 0 | 62,9 (52,6–71,1) |
| 55 | 0 | 1 (0–1) | %99 (%99–%100) | 1 (0–1,5) | 0 | 64,6 (54,8–71,6) |
| 56 | 0 (0–0,5) | 1 (0–1) | %99 (%99–%100) | 1 (0–3) | 0 | 64,6 (55,1–69,5) |
| 57 | 0 | 0,5 (0–1) | %99 (%99–%100) | 1 (0,5–3,5) | 0 | 62,5 (54,8–67) |
| 58 | 0 | 0 (0–1,5) | %99 (%99–%100) | 1 (0–2) | 0 | 61,7 (55,8–68,6) |
| 59 | 0 (0–0,5) | 1 (0–1) | %99 (%99–%100) | 1 (0–2) | 0 | 62,8 (56,6–69,2) |
| 60 | 0 | 1 (0–1,5) | %100 (%99–%100) | 1,5 (0–2,5) | 0 | 61,3 (58,2–68,6) |

### Devlet, inanç ve örgüt (4/5)

| Yıl | Pakt'a bağlı yönetici (yıl sonu) | Köle (yıl sonu) | Köle payı (nüfusun, yıl sonu) | Hapis madeninde mahkûm (yıl sonu) | Esarete düşen | Kurtulan köle (Özgürlük Ağı, kaçış, azat) |
|---|---|---|---|---|---|---|
| 1 | 0 (0–1) | 11,5 (0,5–44,5) | %0,5 (%0–%1,7) | 0 (0–2) | 4,5 (1–13) | 9,5 (0–20,5) |
| 2 | 0 (0–1) | 6,5 (0–36) | %0,2 (%0–%1,3) | 0 (0–2,5) | 3 (0,5–11,5) | 8 (2–20,5) |
| 3 | 0 (0–1) | 3 (0–34) | %0,1 (%0–%1,2) | 0 (0–3,5) | 4 (0–17) | 6 (0–15) |
| 4 | 0 (0–1) | 5,5 (1,5–37,5) | %0,2 (%0,1–%1,4) | 0 (0–4,5) | 6 (1–12) | 3,5 (0–12) |
| 5 | 0 (0–1) | 5 (1–29,5) | %0,2 (%0–%1,1) | 0 (0–7) | 4,5 (0,5–13) | 4,5 (1,5–18,5) |
| 6 | 0 (0–1) | 5,5 (1–32,5) | %0,2 (%0–%1,2) | 1 (0–8) | 4 (0–13,5) | 6 (0–9,5) |
| 7 | 0 (0–1) | 7,5 (0–41,5) | %0,3 (%0–%1,4) | 1,5 (0–10,5) | 5 (0–18) | 4,5 (0–12,5) |
| 8 | 0 (0–1) | 6,5 (1–41) | %0,3 (%0–%1,4) | 0,5 (0–11) | 5 (0,5–14) | 3,5 (1–16) |
| 9 | 0 | 5 (1–36,5) | %0,2 (%0–%1,3) | 0,5 (0–15) | 6 (0,5–13) | 5,5 (0,5–16,5) |
| 10 | 0 (0–0,5) | 9,5 (1–54,5) | %0,4 (%0–%1,8) | 1 (0–19) | 8 (1–23) | 5,5 (1–13,5) |
| 11 | 0 (0–0,5) | 10,5 (0–51,5) | %0,4 (%0–%1,7) | 1 (0–19) | 6 (0–14,5) | 4,5 (0,5–13) |
| 12 | 0 (0–0,5) | 11,5 (1,5–47,5) | %0,4 (%0,1–%1,6) | 0 (0–20) | 5 (1–13) | 5 (1–11) |
| 13 | 0 (0–1) | 10 (1–44,5) | %0,3 (%0–%1,4) | 1 (0–18,5) | 4,5 (0–10,5) | 6,5 (0,5–11) |
| 14 | 0 (0–1) | 10,5 (0,5–45) | %0,4 (%0–%1,5) | 1,5 (0–15) | 5,5 (0–14) | 6 (0–12) |
| 15 | 0 (0–1) | 10 (1–44,5) | %0,3 (%0–%1,5) | 0 (0–13,5) | 4 (0,5–12) | 5,5 (2–10,5) |
| 16 | 0 | 11,5 (0,5–44,5) | %0,4 (%0–%1,4) | 1 (0–16) | 4,5 (0,5–11,5) | 7 (1–9) |
| 17 | 0 | 11,5 (1–52,5) | %0,5 (%0–%1,7) | 1,5 (0–15) | 8 (0–29) | 7,5 (0,5–18) |
| 18 | 0 | 8 (2,5–44,5) | %0,3 (%0,1–%1,4) | 1 (0–13,5) | 5 (0,5–13,5) | 6 (1,5–15,5) |
| 19 | 0 (0–0,5) | 13,5 (2–38) | %0,5 (%0,1–%1,2) | 1 (0–13,5) | 6,5 (2,5–14) | 6,5 (2,5–17) |
| 20 | 0 | 11,5 (1,5–46,5) | %0,6 (%0,1–%1,5) | 0,5 (0–13,5) | 7 (0,5–17,5) | 3,5 (1–14) |
| 21 | 0 (0–0,5) | 13,5 (0–50,5) | %0,5 (%0–%1,5) | 1 (0–13,5) | 4,5 (0–18) | 7 (1,5–12,5) |
| 22 | 0 (0–1) | 16 (3–49,5) | %0,6 (%0,1–%1,5) | 0,5 (0–18,5) | 10 (3,5–17,5) | 5,5 (0–15) |
| 23 | 0 (0–0,5) | 11,5 (0,5–45) | %0,5 (%0–%1,7) | 0,5 (0–16,5) | 6 (0–16) | 6 (2–12,5) |
| 24 | 0 (0–0,5) | 11,5 (1,5–51) | %0,5 (%0,1–%1,7) | 0 (0–14) | 4 (2–12,5) | 6 (1–13,5) |
| 25 | 0 (0–0,5) | 16 (0,5–49,5) | %0,6 (%0–%1,6) | 0,5 (0–9,5) | 7 (0–14,5) | 5,5 (1–12,5) |
| 26 | 0 | 14,5 (1–50) | %0,5 (%0–%1,5) | 0 (0–7) | 5,5 (0–10,5) | 4 (0,5–13,5) |
| 27 | 0 | 17 (0–38,5) | %0,6 (%0–%1,3) | 1 (0–8) | 5 (0–14) | 6,5 (0–14) |
| 28 | 0 | 15 (0,5–39) | %0,5 (%0–%1,3) | 0,5 (0–6,5) | 6,5 (0,5–16) | 10 (0–15) |
| 29 | 0 (0–1) | 11 (1–48,5) | %0,4 (%0–%1,7) | 0 (0–7) | 4,5 (0–21,5) | 6 (1–17,5) |
| 30 | 0 (0–0,5) | 12 (0,5–50) | %0,4 (%0–%1,6) | 0,5 (0–7,5) | 5 (0–16,5) | 6 (0,5–16) |
| 31 | 0 (0–0,5) | 13,5 (0,5–48) | %0,5 (%0–%1,5) | 0,5 (0–8) | 7,5 (0–13) | 6,5 (0–13) |
| 32 | 0 | 19,5 (2–50) | %0,7 (%0,1–%1,7) | 0 (0–11) | 8,5 (0–30) | 4,5 (0–18) |
| 33 | 0 (0–0,5) | 21,5 (0–52) | %0,8 (%0–%1,6) | 1 (0–8,5) | 6,5 (0,5–14,5) | 4,5 (1–15,5) |
| 34 | 0 (0–0,5) | 20,5 (0–48) | %0,7 (%0–%1,4) | 1 (0–6,5) | 3 (0–16,5) | 7 (0,5–14,5) |
| 35 | 0 (0–0,5) | 20,5 (2–44) | %0,7 (%0,1–%1,4) | 1 (0–5) | 5,5 (0,5–21) | 6,5 (0–19) |
| 36 | 0 (0–0,5) | 18 (2,5–45,5) | %0,7 (%0,1–%1,5) | 0,5 (0–1,5) | 6 (1–19,5) | 5,5 (0–12,5) |
| 37 | 0 (0–0,5) | 19,5 (2–42) | %0,7 (%0,1–%1,3) | 0 (0–2,5) | 7 (0–16) | 6 (0–18) |
| 38 | 0 | 12,5 (2–49) | %0,6 (%0,1–%1,5) | 0 (0–5,5) | 2,5 (0–18,5) | 8,5 (0,5–12,5) |
| 39 | 0 | 13,5 (1,5–46,5) | %0,6 (%0,1–%1,5) | 1 (0–5) | 5,5 (0–19) | 5 (0–16,5) |
| 40 | 0 (0–0,5) | 16 (2,5–57) | %0,5 (%0,1–%1,9) | 1 (0–2,5) | 7,5 (2–23,5) | 6,5 (2–21,5) |
| 41 | 0 (0–1) | 19 (2–54,5) | %0,7 (%0,1–%1,7) | 0 (0–1,5) | 3 (0–15) | 7 (1–11,5) |
| 42 | 0 (0–1) | 22,5 (0,5–50,5) | %0,8 (%0–%1,7) | 0 (0–2) | 9 (1–18) | 6 (2–12) |
| 43 | 0 (0–1) | 23 (1–50) | %0,7 (%0–%1,6) | 0 (0–3) | 5 (0–15,5) | 7 (0–12) |
| 44 | 0 (0–1) | 22,5 (2–40) | %0,8 (%0,1–%1,3) | 1 (0–1,5) | 8,5 (0–16) | 7 (0–17,5) |
| 45 | 0 | 18,5 (0,5–44,5) | %0,6 (%0–%1,3) | 0,5 (0–2,5) | 4 (0,5–19,5) | 5,5 (2,5–16) |
| 46 | 0 (0–0,5) | 22 (0,5–42) | %0,7 (%0–%1,3) | 0 (0–3,5) | 4,5 (0–13,5) | 5,5 (0–14,5) |
| 47 | 0 | 18,5 (0,5–48,5) | %0,7 (%0–%1,4) | 0,5 (0–4) | 4 (0–16) | 4,5 (0–15,5) |
| 48 | 0 (0–0,5) | 16 (0,5–43,5) | %0,7 (%0–%1,4) | 0 (0–3) | 4,5 (0–14) | 6,5 (0–17,5) |
| 49 | 0 | 14,5 (1,5–37,5) | %0,5 (%0,1–%1,2) | 1 (0–6) | 3,5 (0,5–12,5) | 5 (0–13,5) |
| 50 | 0 | 16,5 (2,5–33,5) | %0,5 (%0,1–%1,1) | 0 (0–1,5) | 4,5 (1–12) | 4,5 (0,5–13,5) |
| 51 | 0 | 18 (2–35,5) | %0,6 (%0,1–%1,2) | 0,5 (0–2,5) | 5 (1–13) | 4,5 (2–11) |
| 52 | 0 (0–0,5) | 18 (3–30,5) | %0,6 (%0,1–%1) | 0 (0–4,5) | 4 (1–12,5) | 5,5 (0–12) |
| 53 | 0 (0–0,5) | 19 (5,5–34,5) | %0,6 (%0,2–%1,1) | 0,5 (0–5) | 8,5 (0,5–20,5) | 7 (0,5–11,5) |
| 54 | 0 | 20,5 (4,5–29,5) | %0,7 (%0,2–%1,2) | 1 (0–6) | 6 (0–13,5) | 8 (0–12,5) |
| 55 | 0 | 18,5 (4,5–29,5) | %0,6 (%0,2–%1,1) | 0 (0–6) | 7,5 (0–12,5) | 7,5 (0–11,5) |
| 56 | 0 (0–0,5) | 18 (3–30) | %0,7 (%0,1–%1) | 0 (0–6) | 8,5 (0,5–14,5) | 10 (1–18,5) |
| 57 | 0 (0–1) | 18 (4,5–29) | %0,7 (%0,1–%1) | 0 (0–7) | 5,5 (0–10) | 5,5 (0–9) |
| 58 | 0 (0–1) | 18,5 (5–33,5) | %0,7 (%0,2–%1,1) | 0 (0–2) | 8,5 (2,5–14) | 7,5 (2–11) |
| 59 | 0 (0–0,5) | 18,5 (3,5–38) | %0,7 (%0,1–%1,2) | 0,5 (0–3) | 4,5 (1–18) | 7,5 (2–9,5) |
| 60 | 0 | 22,5 (3–43,5) | %0,8 (%0,1–%1,4) | 0 (0–2,5) | 7,5 (1–20,5) | 7,5 (0,5–12) |

### Devlet, inanç ve örgüt (5/5)

| Yıl | Özgürlük Ağı'nın kurtardığı köle | Esir kahraman (yıl sonu) | Aç haydut kampı (yıl sonu) | Haydut olan aç halk | Aç ya da ekmeksiz yerleşim payı (köy+) | Devriye durdurması |
|---|---|---|---|---|---|---|
| 1 | 8 (0–14) | 0 (0–1,5) | 0 (0–1,5) | 6 (0–19,5) | %4,4 (%0,7–%10) | 50 (33,5–69,5) |
| 2 | 4,5 (1,5–10,5) | 0 (0–1,5) | 0,5 (0–2) | 3 (0–14) | %3,8 (%1,4–%8,7) | 50 (38,5–82,5) |
| 3 | 4,5 (0–9,5) | 0 (0–1) | 0 (0–2) | 5,5 (0–25,5) | %4 (%0,9–%13) | 51 (32,5–82) |
| 4 | 3 (0–8,5) | 0 (0–2,5) | 0 (0–1,5) | 5 (0–20) | %4,2 (%1,1–%20) | 50,5 (28–69,5) |
| 5 | 4 (0–10) | 0 (0–2) | 0 (0–2) | 3 (0–23) | %3,3 (%1–%18) | 51 (34,5–65,5) |
| 6 | 5,5 (0–8,5) | 0 (0–2,5) | 0 (0–2) | 9,5 (0–26,5) | %5,1 (%1,5–%16) | 54 (32,5–66,5) |
| 7 | 4,5 (0–10) | 0 (0–2,5) | 0 (0–1,5) | 8 (0–20) | %4,6 (%0,8–%18) | 59 (31–72,5) |
| 8 | 3 (1–14) | 0 (0–1) | 1 (0–2) | 11,5 (0–33,5) | %5,7 (%1,2–%17) | 49 (33,5–72,5) |
| 9 | 5 (0,5–12) | 0 (0–2,5) | 1 (0–2) | 6 (0–21) | %6,6 (%1,4–%17) | 48,5 (37–71) |
| 10 | 3,5 (0–10,5) | 0 (0–3) | 0 (0–2) | 4 (1,5–17,5) | %6,5 (%1,1–%15) | 47 (40,5–68) |
| 11 | 4 (0,5–8,5) | 0,5 (0–2,5) | 0 (0–2) | 7 (0–20) | %5,4 (%0,6–%14) | 52 (36,5–73,5) |
| 12 | 4 (1–7) | 0,5 (0–2,5) | 0 (0–1,5) | 3 (0–18) | %6,3 (%0,6–%9,3) | 55 (29–76,5) |
| 13 | 4,5 (0,5–9) | 0 (0–3) | 0 (0–1) | 10,5 (0–21,5) | %5,4 (%0,8–%13) | 58 (29,5–76,5) |
| 14 | 4,5 (0–9) | 0,5 (0–2) | 1 (0–2,5) | 5,5 (1,5–25,5) | %5,6 (%1,2–%14) | 53,5 (34,5–72,5) |
| 15 | 4 (1–8) | 0 (0–3,5) | 1 (0–2) | 7 (0–35) | %4 (%1–%13) | 53,5 (34,5–76,5) |
| 16 | 4,5 (0–8) | 0 (0–2,5) | 0 (0–1) | 5 (0–23) | %6,8 (%0,9–%10) | 53 (32,5–77) |
| 17 | 4,5 (0–12) | 0 (0–2) | 1 (0–2) | 7 (3–15) | %5,2 (%1,7–%15) | 51,5 (31,5–75,5) |
| 18 | 4,5 (1–11,5) | 0 (0–1) | 0 (0–1,5) | 5,5 (0–21) | %5,2 (%1,3–%13) | 54,5 (36,5–70) |
| 19 | 4,5 (0–12) | 0 (0–2,5) | 0 (0–1,5) | 8,5 (1,5–19,5) | %5,3 (%2,3–%13) | 54,5 (29,5–80) |
| 20 | 3 (1–9,5) | 0 (0–2) | 0 (0–2) | 5 (1,5–20) | %5,9 (%2,2–%14) | 54,5 (32–66,5) |
| 21 | 4 (0–10,5) | 0 (0–3) | 1 (0–2) | 8,5 (0–20,5) | %6,3 (%2,3–%13) | 55,5 (36,5–77) |
| 22 | 4,5 (0–11,5) | 0 (0–3) | 0 (0–1) | 11 (0–23,5) | %7,8 (%2,6–%14) | 50 (28–68,5) |
| 23 | 5 (1–9,5) | 0 (0–1) | 2 (0–3,5) | 12 (0–27) | %6,4 (%2,3–%15) | 52,5 (30,5–73,5) |
| 24 | 4 (0–10) | 1 (0–2) | 1 (0–1,5) | 6 (0–12) | %6,7 (%1,5–%14) | 59,5 (28–76) |
| 25 | 5 (0–8,5) | 0 (0–1) | 0 (0–1,5) | 11 (3–18,5) | %6,1 (%2,3–%16) | 54 (29–78) |
| 26 | 3 (0–9) | 0 (0–1,5) | 1 (0–2) | 12 (3–22) | %4,9 (%2,6–%12) | 55 (25–76) |
| 27 | 6 (0–12) | 0 (0–2) | 1 (0–3) | 9,5 (1,5–14,5) | %3,8 (%2–%13) | 52,5 (28,5–87,5) |
| 28 | 5,5 (0–13) | 0 (0–3) | 0 (0–1) | 6 (0–19,5) | %4,4 (%2,8–%11) | 51 (29–80,5) |
| 29 | 4,5 (0–14,5) | 0,5 (0–2) | 0 (0–2) | 9 (1,5–28) | %5,4 (%2,7–%14) | 50 (32–78,5) |
| 30 | 4,5 (0–8,5) | 0 (0–2) | 1 (0–2) | 10 (1,5–25,5) | %7,2 (%2,3–%16) | 52,5 (24–79) |
| 31 | 3,5 (0–11,5) | 0 (0–2,5) | 0 (0–2) | 5 (0–23,5) | %5,4 (%2,4–%16) | 63,5 (33,5–85,5) |
| 32 | 4 (0–13) | 0 (0–2) | 1 (0–1,5) | 5,5 (3–25,5) | %4 (%3,4–%11) | 60 (30–74) |
| 33 | 3 (0–8) | 0 (0–2,5) | 0 (0–1) | 10,5 (0–17,5) | %5,1 (%2,8–%13) | 57 (28,5–73) |
| 34 | 6 (0,5–11) | 0,5 (0–2) | 0 (0–2) | 8,5 (0–31) | %6,8 (%3,1–%16) | 55,5 (27–71) |
| 35 | 4,5 (0–9,5) | 0 (0–1,5) | 1 (0–3) | 12,5 (2,5–39) | %6,5 (%2,5–%25) | 59,5 (28–68,5) |
| 36 | 4,5 (0–11) | 1 (0–3) | 0,5 (0–2,5) | 12,5 (1,5–37,5) | %5,9 (%2,9–%26) | 56 (37,5–80) |
| 37 | 5,5 (0–11,5) | 0 (0–1,5) | 0 (0–1) | 10 (0–30) | %5,6 (%2,4–%27) | 58 (31,5–71) |
| 38 | 4,5 (0–9,5) | 0 (0–1) | 1 (0–2,5) | 12 (1,5–28) | %6,1 (%1,6–%31) | 60 (32,5–67,5) |
| 39 | 4 (0–11,5) | 1 (0–2) | 1 (0–1) | 6,5 (0–35) | %8,1 (%1,9–%24) | 59,5 (24,5–75,5) |
| 40 | 5 (1–12,5) | 0 (0–1) | 0,5 (0–1) | 11 (3,5–22) | %6,7 (%3,1–%21) | 60,5 (32,5–68,5) |
| 41 | 3,5 (1–7,5) | 0 (0–2) | 1 (0–2,5) | 20,5 (0–48,5) | %8,3 (%2,3–%29) | 64 (23–76) |
| 42 | 5 (1–10,5) | 1 (0–1,5) | 1 (0–3) | 12,5 (0–32,5) | %12 (%4–%31) | 57,5 (34,5–80,5) |
| 43 | 4,5 (0–9,5) | 0 (0–3) | 0 (0–2) | 15 (1,5–28,5) | %8,6 (%3,5–%24) | 60 (37,5–74) |
| 44 | 4,5 (0–11) | 0 (0–2) | 0 (0–2,5) | 13,5 (0–33) | %14 (%3,1–%24) | 64 (29,5–73,5) |
| 45 | 5 (2–11) | 0 (0–1) | 0,5 (0–2) | 13,5 (2–27) | %12 (%3–%22) | 62,5 (33,5–82,5) |
| 46 | 4 (0–9) | 0 (0–2,5) | 0 (0–1) | 6 (0–24) | %9,4 (%2,9–%19) | 67 (32,5–84) |
| 47 | 4,5 (0–10,5) | 0 (0–2,5) | 0,5 (0–3) | 12,5 (3–25) | %8,5 (%3,4–%19) | 60,5 (27,5–78,5) |
| 48 | 5 (0–13) | 0 (0–2) | 1 (0–2,5) | 9,5 (1,5–20) | %6,8 (%4,4–%17) | 71,5 (26–84) |
| 49 | 3 (0–9) | 0 (0–1,5) | 0 (0–2) | 9,5 (1,5–20) | %8,1 (%3,7–%15) | 61,5 (35,5–81,5) |
| 50 | 3,5 (0–8,5) | 0 (0–2) | 1 (0–1,5) | 12,5 (0–30) | %7,6 (%3,4–%18) | 61,5 (36,5–88) |
| 51 | 3 (1–7) | 0 (0–1) | 1 (0–2,5) | 11 (1,5–39,5) | %8,3 (%1,4–%25) | 59 (31–82,5) |
| 52 | 4,5 (0–8) | 0 (0–1) | 0 (0–2) | 9,5 (0–34) | %8,6 (%2,1–%32) | 61 (34–82) |
| 53 | 4,5 (0–8,5) | 0 (0–1) | 0,5 (0–2,5) | 11 (5,5–42,5) | %5,9 (%2,6–%29) | 62 (30–81,5) |
| 54 | 6 (0–10) | 0 (0–1) | 1 (0–2,5) | 9,5 (0–41,5) | %9,8 (%3–%34) | 56,5 (29–73) |
| 55 | 6 (0–9) | 0 (0–1,5) | 0,5 (0–1) | 17 (1,5–32,5) | %8,5 (%2,3–%28) | 61 (33,5–81) |
| 56 | 5,5 (0–12) | 0 (0–1) | 1 (0–2) | 13,5 (3,5–35,5) | %12 (%4,2–%19) | 66,5 (31,5–84,5) |
| 57 | 4,5 (0–7) | 0 (0–1,5) | 1 (0–2,5) | 16,5 (6,5–31) | %9,4 (%3–%25) | 56 (33–84) |
| 58 | 7 (2–9) | 0 (0–1) | 0 (0–2) | 15 (5,5–34) | %12 (%3,3–%32) | 57,5 (28,5–89) |
| 59 | 5 (0–7) | 0 (0–1) | 1 (0–2,5) | 15,5 (3–27,5) | %11 (%2,4–%28) | 61 (32–82) |
| 60 | 6 (0,5–10) | 0 (0–1) | 1 (0–3) | 15,5 (1,5–32,5) | %12 (%2–%32) | 65 (32,5–78,5) |

