# Ölçüm raporu: f1b-5

16 dünya (seed 1-16) × 60 yıl (2400 gün), 1 yıl = 40 gün · 2026-10-01 21:26 · `FD.Macro.Run stats --seeds 1-16 --years 60 --jobs 2 --verify 1 --saveload 1`

Süre: 7 dk 7 sn duvar saati, 2 iş parçacığı; dünya başına 45,2 sn (en az 34,4, en çok 80,7; yıl sonu hash'leri dâhil).

## Bitiş ölçütleri

DESIGN-FAZ1.md, "Bitiş ölçütleri" (1–5), yol haritası v3 (6–7, Faz 1b-4) ve v3'ün süre tablosu (8, Faz 1b-5). ✓ geçti · ✗ kaldı · — ölçülemedi · ○ bilgi (hedef yok) · † zaman ölçeğine bağlı (Faz 1b-5'ten beri takvim yeni ölçekte: 1 yıl = 40 gün, değerler doğrudan okunur).

| # | Ölçüt | Koşul | Ölçülen | Sonuç |
|---|---|---|---|---|
| 1 | Donma yok | 41–60. yılların yıllık büyük olay medyanı ≥ 0,8 × 6–20. yılların medyanı | 119 / 88 = 1,35 kat | ✓ |
| 2 | Çöküş | dünyaların ≥ %75'inde 60 yılda ≥ 1 çöküş (yok olma ya da başkent kaybı) | %88 (14/16 dünya); toplam 73 çöküş: 9 yok olma, 64 başkent kaybı | ✓ |
| 3 | Kamplar | 41–60. yıllarda yaşayan kamp medyanı ≥ 6–20. yılların medyanı | 8,9 ≥ 8,83 (yıl sonu sayımıyla 9 / 9) | ✓ |
| 4a | Kahraman: doğuş seviyesi | her on yılda doğanların ortalama seviyesi ≤ 2 | 1,31 · 1,31 · 1,31 · 1,45 · 1,55 · 1,6 (on yıllar sırasıyla) | ✓ |
| 4b | Kahraman: Sv8+ | dünyaların ≥ yarısında en az bir kahraman Sv8 ve üstüne çıkar | %100 (16/16 dünya); dünyadaki en yüksek seviye: medyan Sv10, en çok Sv10 | ✓ |
| 4c | Kahraman: efsane | dünya başına efsane medyanı 1–6 | medyan 4 (p10–p90: 1,5–7,5; toplam 67) | ✓ |
| 4d | Kahraman: ölüm payı | doğan kahramanların %30–80'i ölür | %33 (3996/12052); dünya medyanı %32 (%29–%42) | ✓ |
| 5a | Determinizm | aynı seed → aynı tarih (toplayıcılı ve toplayıcısız koşu, her yıl sonu hash'i) | seed 1: 60/60 yıl sonu aynı | ✓ |
| 5b | Kayıt/yükleme | kaydet → yükle → devam = kesintisiz koşu (her yıl sonu hash'i) | seed 1, gün 1237: yüklenen dünya aynı, sonraki 30/30 yıl sonu aynı | ✓ |
| 6a | Yerleşim sayısı sabit, ısınmadan sonra (21–60. yıl) | dünya başına yaşayan yerleşim sayısının (günlük) değişim katsayısı medyanı ≤ %10 | değişim katsayısı %8,4 (%4,8–%16); ortalama 79,5 yerleşim, en az ve en çok ortalamanın %81 ve %113 kadarı | ✓ |
| 6b | Yerleşim sayısı sabit, bütün koşu (1–60. yıl) | dünya başına yaşayan yerleşim sayısının (günlük) değişim katsayısı medyanı ≤ %10 (dünya hâlâ 8 kamptan başlayıp büyüyor; dünya üretimi sonraki adım) | değişim katsayısı %33 (%29–%38); ortalama 65,6 yerleşim, en az ve en çok ortalamanın %12 ve %136 kadarı | ✗ |
| 6c | El değiştirme (her yerleşim) | fetih + bölünme, 100 günde, dünya başına ve yerleşim başına (21–60. yıl); hedef yok | dünyada 2,25 (0,41–5,06) / 100 gün; yerleşim başına 2,93 / 10 bin gün: 2,25 / 100 gün | ○ |
| 6d | Durum dalgalanması (yerleşim başına) | açlık, salgın, kuşatma, yakılma, kademe, el değiştirme, terk, yeniden yerleşim: yerleşim başına 100 günde (21–60. yıl); hedef yok | yerleşim başına 0,5 (0,26–0,85) / 100 gün, yani ~199 günde bir; dünyada 38,8 / 100 gün: yerleşim başına ~199 günde bir | ○ |
| 6e | Orta halka: kademe değişimi † | Köy/Kasaba başına kademe değişimi ya da terk, yeni takvimde 60–150 günde bir (21–60. yıl) | yerleşim başına 722 (533–1771) günde bir | ✗ |
| 6f | Büyük şehir el değiştirmesi † | büyük şehir (Şehir kademesi) bütün dünyada yeni takvimde 100 günde 2–4 kez el değiştirir (21–60. yıl); felaketle düşmez | dünyada 0,03 (0–0,22) / 100 gün; şehir başına 0 / 100 gün; dünyada ortalama 6,42 büyük şehir; toplam 22 el değiştirme (0 bölünme). Ayrıştırma: dünyada 3,69 savaş / 100 gün, hedefi büyük şehir olan %12; büyük şehre 134 hücum, düşüşle bitenlerin payı %16 | ✗ |
| 6g | Büyük şehir: uyarı süresi † | büyük şehrin düşüşünden önce yeni takvimde 5–10 gün uyarı (kuşatmanın başı → düşüş, medyan; 21–60. yıl) | kuşatmanın başından 5 gün (5–5; n = 22); savaşın başından 12,5 gün (10,1–31,7) | ✓ |
| 6h | Savaş süresi † | savaş (ilandan barışa) yeni takvimde 10–40 gün (medyan; 21–60. yıl başlayıp biten savaşlar) | 13 gün (10–35; n = 1097) | ✓ |
| 6i | Kuşatma süresi † | büyük şehir kuşatması (karargâhtan hücuma) yeni takvimde 2–6 gün (medyan; 21–60. yıl) | büyük şehir 6 gün (6–6; n = 134); diğer yerleşimler 3 gün (3–6; n = 853) | ✓ |
| 6j | Başkent kaybı (medeniyet başına) | hiçbir medeniyet başkentini 3 kereden çok kaybetmez (yol haritası: "en fazla birkaç kez") | en çok 3; 3'ten çok kaybeden 0 medeniyet; toplam 64 başkent kaybı, 50/144 medeniyette (1×: 39, 2×: 8, 3×: 3) | ✓ |
| 7 | Felaket büyük şehri düşürmez | Şehir kademesine varmış yerleşim hiç terk edilmez; ejderha akını büyük şehrin nüfusunu Şehir eşiğinin (85) altına indiremez; akından sonraki 60 günde terk yok | terk edilen eski Şehir: 0; ejderha akını 569 (büyük şehre 74, orada 926 ölü): akınla eşiğin altına inen 0, sonraki 60 günde terk 0; bilgi: akın günü başka nedenlerle (ordu, öncü) eşiğin altına inen 2, 60 gün içinde kademe düşüşü 22, el değiştirme 0 | ✓ |
| 8a | Salgın süresi | salgın başladığı günden bittiği güne: 5–10 gün (medyan) | 6 gün (4–7; n = 609) | ✓ |
| 8b | Tepki inşaatı: yanan ev | yanan her ev yandığı günden onarıldığı güne (ilk yanan ilk onarılır): 1–3 gün (medyan) | 3 gün (2–5; n = 12978) | ✓ |
| 8c | Tepki inşaatı: sur | palisat ve taş sur: projenin başından bitişine: 5–10 gün (medyan) | 8 gün (6,7–10; n = 2188); palisat 8, taş sur 7 gün | ✓ |
| 8d | Büyük proje (kale, kule) | kale ve fener kulesi: projenin başından bitişine: 15–30 gün (medyan) | 20 gün (18–23; n = 173); kale 20 (n = 70), kule 23 gün (n = 103) | ✓ |
| 8e | Temizlenen kamp → yeni köy | temizlenen kara kampının 6 fersah yakınına 60 gün içinde ilk yerleşimin kurulması: 10–20 gün (medyan) | 15 gün (6,1–51; n = 192); temizlenen 3384 kampın 192 tanesine (%5,7) köy kuruldu, 20 gün içinde %3,5 | ✓ |
| 8f | Han kurulumu | yeni han: hancının yola çıktığı günden kapıların açıldığı güne: 5–10 gün (medyan) | 8 gün (5–13; n = 98); harabeyi yeniden kurma 3 gün (n = 15) | ✓ |
| 8g | Kahraman doğumu (han) | han başına 10–20 günde bir (açık han-günü / handa doğan kahraman) | 18 günde bir (11253 doğum / 202596 han-günü); aynı handa iki doğum arası medyan 14 gün (3–40); bilgi: taverna başına 933 günde bir (799 doğum) | ✓ |
| 8h | Kahramanın efsaneye yükselişi | doğumundan efsane olduğu güne: 100–300 gün (medyan) | 263 gün (56,4–748; n = 67) | ✓ |
| 8i | İlan ömrü | alınmayan ilanın asıldığı günden kapandığı güne (süresi doldu ya da kampı başkası temizledi): 10–20 gün (medyan) | 15 gün (15–15; n = 4619); bütün ilanlar 15 gün (n = 7460): biten %22 (asılıştan 6 günde), süresi dolan %60; başarısız sefer ödülü %25 artırır (Heroes.QuestFailed) | ✓ |
| 8j | Yoldaş/kahraman maaşı | haftalık (5 gün) | kural: medeniyetin kahramanları her 5. gün (Economy.PayHeroes), han personeli haftada bir (InnLife.PayWages); maaşı 3 hafta ödenmeyen kahraman ayrılır | ○ |

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

Yol haritası v3: dünya büyüyerek değil, durum değiştirerek yaşar. Bütün değerler dünya başına; hücre: dünyalar arası medyan (p10–p90). "100 günde": pencerede sayılan / pencerenin günü × 100; "yerleşim başına": yaşayan yerleşim-günlerine bölünür. Büyük şehir = Şehir kademesi (kademe 3, nüfus ≥ 100; histerezisle ≥ 85). Isınma: koşunun ilk üçte biri (800 gün). Dünya hâlâ 8 kamptan başlayıp ilk on yıllarda büyüyor (dünya üretimi sonraki adım); bu yüzden bütün koşunun değerleri ayrıca verilmiştir.

| Ölçü | bütün koşu (1–60. yıl) | ısınmadan sonra (21–60. yıl) |
|---|---|---|
| Yaşayan yerleşim (günlük ortalama) | 65,6 (53,8–75,4) | 79,5 (63,5–90,5) |
| Yaşayan yerleşim: en az (ortalamaya oranı) | %12 (%11–%14) | %81 (%73–%88) |
| Yaşayan yerleşim: en çok (ortalamaya oranı) | %136 (%124–%150) | %113 (%106–%124) |
| Yaşayan yerleşim: değişim katsayısı | %33 (%29–%38) | %8,4 (%4,8–%16) |
| Büyük şehir (günlük ortalama) | 4,63 (3,92–5,84) | 6,42 (5,69–8,22) |
| El değiştirme (fetih + bölünme), dünyada / 100 gün | 2,1 (0,58–4,33) | 2,25 (0,41–5,06) |
| El değiştirme, yerleşim başına / 10 bin gün | 3,46 (0,88–5,93) | 2,93 (0,54–6,08) |
| Durum değişimi (kuruluş hariç), yerleşim başına / 100 gün | 0,53 (0,32–0,86) | 0,5 (0,26–0,85) |
| Durum değişimi, dünyada / 100 gün | 34,4 (19,9–59,2) | 38,8 (20–69,3) |
| Orta halka (Köy/Kasaba) kaydı: yerleşim başına kaç günde bir | 656 (495–1130) | 722 (533–1771) |
| Büyük şehir el değiştirdi, dünyada / 100 gün | 0,04 (0–0,15) | 0,03 (0–0,22) |
| Büyük şehir el değiştirdi, şehir başına / 100 gün | 0,01 (0–0,03) | 0 (0–0,03) |
| Büyük şehre hücum, dünyada / 100 gün | 0,27 (0–0,85) | 0,41 (0–1,19) |
| Büyük şehir yağmalandı ama tutulmadı, dünyada / 100 gün | 0,19 (0–0,52) | 0,25 (0–0,69) |

Durum değişimi türleri (yerleşim başına 10 bin günde; bölünme dışında olayın kademesi olaydan önceki):

| Tür | bütün koşu (1–60. yıl) | ısınmadan sonra (21–60. yıl) |
|---|---|---|
| Fetih (`capture`) | 3,46 (0,88–5,81) | 2,93 (0,54–5,89) |
| Bölünme (`secede`) | 0 (0–0,16) | 0 (0–0,2) |
| Açlık başladı (`famine`) | 0,16 (0–2,66) | 0,2 (0–3,04) |
| Salgın başladı (`plague`) | 2,29 (1,73–3,2) | 2,37 (1,65–3,87) |
| Kuşatma (hücum) (`siege`) | 4,62 (1,21–8,71) | 4,57 (1,15–8,49) |
| Yakıldı / yandı (`burn`) | 23,4 (13,3–38,5) | 23,2 (12,4–39,7) |
| Kademe yükseldi (`tierUp`) | 13,1 (10,5–16,5) | 9,29 (5,26–13,1) |
| Kademe düştü (`tierDown`) | 6,61 (2,75–9,51) | 7,18 (2,36–9,99) |
| Terk (harabe) (`abandon`) | 0,29 (0,08–0,8) | 0,32 (0,07–0,94) |
| Harabeye yeniden yerleşim (`resettle`) | 0,1 (0–0,46) | 0,12 (0–0,55) |
| Kuruluş (durum değişimi sayılmaz) (`found`) | 5,42 (4,82–6,13) | 2,31 (1,37–3,45) |

Süreler (gün; bütün dünyalar havuzlanmış; savaş: ilandan ilişkiden düştüğü güne, koşu sonunda süren savaşlar hariç; kuşatma: hedefin önündeki ilk karargâhtan hücuma, karargâhsız hücum 1 gün; uyarı: büyük şehrin düşüşünden geriye):

| Süre | Pencere | n | p10 | medyan | p90 | ortalama | en çok |
|---|---|---|---|---|---|---|---|
| Savaş (hepsi) | bütün koşu (1–60. yıl) | 1387 | 10 | 13 | 35 | 19 | 78 |
| Savaş: sıradan | bütün koşu (1–60. yıl) | 726 | 10 | 13 | 37 | 19,5 | 78 |
| Savaş: haraç | bütün koşu (1–60. yıl) | 4 | 13,6 | 26 | 34,9 | 24,8 | 37 |
| Savaş: tarihî hak | bütün koşu (1–60. yıl) | 105 | 8,8 | 13 | 77 | 23,3 | 78 |
| Savaş: Kutsal Sefer | bütün koşu (1–60. yıl) | 117 | 10 | 25 | 38,8 | 24,9 | 78 |
| Savaş: savunma paktı | bütün koşu (1–60. yıl) | 165 | 10 | 12 | 17,6 | 13 | 47 |
| Savaş: müttefik çağrısı | bütün koşu (1–60. yıl) | 270 | 10 | 12 | 33 | 17 | 61 |
| Kuşatma: büyük şehir | bütün koşu (1–60. yıl) | 143 | 6 | 6 | 6 | 6 | 8 |
| Kuşatma: diğer yerleşimler | bütün koşu (1–60. yıl) | 1124 | 3 | 3 | 6 | 3,88 | 8 |
| Uyarı: kuşatmanın başı → büyük şehrin düşüşü | bütün koşu (1–60. yıl) | 23 | 5 | 5 | 5 | 4,87 | 5 |
| Uyarı: savaşın başı → büyük şehrin düşüşü | bütün koşu (1–60. yıl) | 23 | 10,2 | 13 | 31,8 | 17,8 | 40 |
| Savaş (hepsi) | ısınmadan sonra (21–60. yıl) | 1097 | 10 | 13 | 35 | 19,5 | 78 |
| Savaş: sıradan | ısınmadan sonra (21–60. yıl) | 538 | 10 | 13 | 37 | 19,9 | 78 |
| Savaş: haraç | ısınmadan sonra (21–60. yıl) | 4 | 13,6 | 26 | 34,9 | 24,8 | 37 |
| Savaş: tarihî hak | ısınmadan sonra (21–60. yıl) | 89 | 10 | 13 | 77 | 25,3 | 78 |
| Savaş: Kutsal Sefer | ısınmadan sonra (21–60. yıl) | 96 | 10 | 25 | 40 | 26,2 | 78 |
| Savaş: savunma paktı | ısınmadan sonra (21–60. yıl) | 134 | 10 | 10 | 17 | 12,7 | 47 |
| Savaş: müttefik çağrısı | ısınmadan sonra (21–60. yıl) | 236 | 10 | 13 | 34 | 17,4 | 61 |
| Kuşatma: büyük şehir | ısınmadan sonra (21–60. yıl) | 134 | 6 | 6 | 6 | 6 | 8 |
| Kuşatma: diğer yerleşimler | ısınmadan sonra (21–60. yıl) | 853 | 3 | 3 | 6 | 3,91 | 8 |
| Uyarı: kuşatmanın başı → büyük şehrin düşüşü | ısınmadan sonra (21–60. yıl) | 22 | 5 | 5 | 5 | 4,86 | 5 |
| Uyarı: savaşın başı → büyük şehrin düşüşü | ısınmadan sonra (21–60. yıl) | 22 | 10,1 | 12,5 | 31,7 | 17,2 | 40 |

Koşu sonunda süren savaş: 13 (sürelere girmedi).

Kuşatma sonuçları (bütün koşu): büyük şehir: 143 hücum: 23 el değiştirdi, 89 yağmalandı (tutulmadı), 31 püskürtüldü; diğer: 1124 hücum: 860 el değiştirdi, 248 yağmalandı (tutulmadı), 16 püskürtüldü.

Büyük şehrin el değiştirmesi (bütün koşu, 23; ilk 60):

| Seed | Gün (yıl) | Şehir | Nasıl | Önceki → yeni sahip | Kuşatma → düşüş | Savaş → düşüş |
|---|---|---|---|---|---|---|
| 3 | 1915 (48) | Sisliyamaç | fetih | Kanlıdiş Kabileleri → Tatlıçayır Loncası | 5 gün | 12 gün |
| 3 | 2362 (60) | Sisliyamaç | fetih | Kanlıdiş Kabileleri → Örsyürek Tapınak Klanı | 5 gün | 24 gün |
| 3 | 2367 (60) | Kristalköy | fetih | Güneştacı Krallığı → Kanlıdiş Kabileleri | 5 gün | 29 gün |
| 4 | 1006 (26) | İzsürer | fetih | Kanlıdiş Kabileleri → Güneştacı Krallığı | 5 gün | 11 gün |
| 5 | 909 (23) | Boynuztepe | fetih | Kanlıdiş Kabileleri → Pulzırh Lejyonu | 5 gün | 16 gün |
| 5 | 2339 (59) | Aysırt | fetih | Tatlıçayır Loncası → Örsyürek Tapınak Klanı | 5 gün | 26 gün |
| 7 | 639 (16) | Nağmeköy | fetih | Lirsesi Şehirleri → Karaörs Derinlikleri | 5 gün | 31 gün |
| 10 | 1775 (45) | Nağmeköy | fetih | Lirsesi Şehirleri → Karaörs Derinlikleri | 5 gün | 40 gün |
| 10 | 1998 (50) | Yeşilyazı | fetih | Kanlıdiş Kabileleri → Karaörs Derinlikleri | 2 gün | 35 gün |
| 10 | 2162 (55) | Okyayı | fetih | Kanlıdiş Kabileleri → Örsyürek Tapınak Klanı | 5 gün | 12 gün |
| 10 | 2247 (57) | Pusulakule | fetih | Karaörs Derinlikleri → Lirsesi Şehirleri | 5 gün | 9 gün |
| 10 | 2270 (57) | Okyayı | fetih | Örsyürek Tapınak Klanı → Karaörs Derinlikleri | 5 gün | 32 gün |
| 12 | 857 (22) | Kemikçadır | fetih | Karaörs Derinlikleri → Yeşilyaprak Çemberi | 5 gün | 13 gün |
| 13 | 1811 (46) | Çankule | fetih | Kızılboynuz Soyu → Pulzırh Lejyonu | 5 gün | 13 gün |
| 13 | 1962 (50) | Balköprü | fetih | Kanlıdiş Kabileleri → Pulzırh Lejyonu | 5 gün | 9 gün |
| 13 | 2039 (51) | Pınarhisar | fetih | Kızılboynuz Soyu → Rüzgâr Manastırı | 5 gün | 11 gün |
| 13 | 2091 (53) | Yeşilkaya | fetih | Kızılboynuz Soyu → Pulzırh Lejyonu | 5 gün | 11 gün |
| 15 | 1525 (39) | Kristalköy | fetih | Pulzırh Lejyonu → Kanlıdiş Kabileleri | 5 gün | 12 gün |
| 15 | 2258 (57) | Çamkoru | fetih | Kanlıdiş Kabileleri → Pulzırh Lejyonu | 5 gün | 15 gün |
| 15 | 2273 (57) | Kartalyazı | fetih | Kızılboynuz Soyu → Kanlıdiş Kabileleri | 5 gün | 10 gün |
| 16 | 1062 (27) | Taşdere | fetih | Kanlıdiş Kabileleri → Pulzırh Lejyonu | 5 gün | 12 gün |
| 16 | 1408 (36) | Derinmihrap | fetih | Kanlıdiş Kabileleri → Pulzırh Lejyonu | 5 gün | 15 gün |
| 16 | 1634 (41) | Kavşakpazar | fetih | Kanlıdiş Kabileleri → Pulzırh Lejyonu | 5 gün | 11 gün |

## v3: süre tablosu (Faz 1b-5)

Yol haritası v3'ün hedef süreleri (oyun günü; 1 yıl = 40 gün, 1 ay = 10, 1 hafta = 5). Bütün dünyalar havuzlanmış; 6e–6i ısınmadan sonra, 8a–8i bütün koşu. Ajan hızları `Core/Time.cs` (`Pace`).

| Süreç | Hedef | Ölçülen | Sonuç |
|---|---|---|---|
| Orta halkada kademe kayması (6e) | yerleşim başına 60–150 günde bir | yerleşim başına 722 (533–1771) günde bir | ✗ |
| Büyük şehir el değiştirmesi (6f) | dünyada 100 günde 2–4 | dünyada 0,03 (0–0,22) / 100 gün; şehir başına 0 / 100 gün; dünyada ortalama 6,42 büyük şehir; toplam 22 el değiştirme (0 bölünme). Ayrıştırma: dünyada 3,69 savaş / 100 gün, hedefi büyük şehir olan %12; büyük şehre 134 hücum, düşüşle bitenlerin payı %16 | ✗ |
| ↳ uyarı (6g) | 5–10 gün önceden | kuşatmanın başından 5 gün (5–5; n = 22); savaşın başından 12,5 gün (10,1–31,7) | ✓ |
| Savaş (6h) | 10–40 gün | 13 gün (10–35; n = 1097) | ✓ |
| ↳ kuşatma (6i) | 2–6 gün | büyük şehir 6 gün (6–6; n = 134); diğer yerleşimler 3 gün (3–6; n = 853) | ✓ |
| Salgın (8a) | 5–10 gün | 6 gün (4–7; n = 609) | ✓ |
| Tepki inşaatı: yanan ev (8b) | 1–3 gün | 3 gün (2–5; n = 12978) | ✓ |
| Tepki inşaatı: sur (8c) | 5–10 gün | 8 gün (6,7–10; n = 2188); palisat 8, taş sur 7 gün | ✓ |
| Büyük proje: kale, kule (8d) | 15–30 gün | 20 gün (18–23; n = 173); kale 20 (n = 70), kule 23 gün (n = 103) | ✓ |
| Temizlenen kamp → yeni köy (8e) | 10–20 gün | 15 gün (6,1–51; n = 192); temizlenen 3384 kampın 192 tanesine (%5,7) köy kuruldu, 20 gün içinde %3,5 | ✓ |
| Han kurulumu (8f) | 5–10 gün | 8 gün (5–13; n = 98); harabeyi yeniden kurma 3 gün (n = 15) | ✓ |
| Kahraman doğumu (8g) | han başına 10–20 günde bir | 18 günde bir (11253 doğum / 202596 han-günü); aynı handa iki doğum arası medyan 14 gün (3–40); bilgi: taverna başına 933 günde bir (799 doğum) | ✓ |
| Efsaneye yükseliş (8h) | 100–300 gün | 263 gün (56,4–748; n = 67) | ✓ |
| İlan ömrü (8i) | 10–20 gün (başarısız sefer ödülü +%25) | 15 gün (15–15; n = 4619); bütün ilanlar 15 gün (n = 7460): biten %22 (asılıştan 6 günde), süresi dolan %60; başarısız sefer ödülü %25 artırır (Heroes.QuestFailed) | ✓ |
| Yoldaş maaşı (8j) | haftalık (5 gün) | kural: medeniyetin kahramanları her 5. gün (Economy.PayHeroes), han personeli haftada bir (InnLife.PayWages); maaşı 3 hafta ödenmeyen kahraman ayrılır | ○ |

Proje süreleri, yapı türüne göre (gün; bütün koşu):

| Yapı | n | p10 | medyan | p90 |
|---|---|---|---|---|
| castle | 70 | 20 | 20 | 20 |
| extract | 26080 | 1 | 1 | 1 |
| guild | 235 | 1 | 1 | 1 |
| house | 14810 | 1 | 1 | 1 |
| hut | 531 | 1 | 1 | 1 |
| library | 465 | 1 | 1 | 2 |
| lighthouse | 103 | 18 | 23 | 23 |
| market | 1244 | 1 | 1 | 1 |
| mint | 949 | 1 | 1 | 1 |
| palisade | 1550 | 6 | 8 | 10 |
| ship | 1848 | 1 | 1 | 2 |
| shipyard | 382 | 1 | 3 | 3 |
| stonehouse | 2888 | 1 | 1 | 1 |
| stonewall | 638 | 7 | 7 | 9 |
| tavern | 507 | 1 | 1 | 2 |
| temple | 1556 | 1 | 1 | 2 |
| unique | 485 | 1 | 1 | 2 |
| upgrade | 13080 | 1 | 1 | 2 |
| workshop | 7552 | 1 | 1 | 2 |

## Eski analizdeki sorunlar

Eski analiz: TS v0.23, 12 seed × 30 yıl ve 3 seed × 60 yıl (Proje: `analiz-5-ajan-oneriler.md`). "Sürüyor mu" kaba bir eşiktir: araştırma ağacı Faz 1b-3'te kaldırıldı; 30. yılda tam 5 kara yerleşimli medeniyet ≥ %50; kamp (30. yıl) < 0,75 × en yüksek yıl; altın (30. yıl) ≥ 10 × altın (1. yıl); boştaki iş gücü (30. yıl) ≥ %30; büyük olay (30. yıl) ≤ 0,6 × en yüksek yıl; 25. yıldan sonra doğanların ≥ %50'si Sv5+; hiç başkent kaybı yok.

| Bulgu | Eski analiz | Bu ölçüm | Sürüyor mu? |
|---|---|---|---|
| Araştırma ağacı erken bitiyor | ~19. yılda bitiyor; 30. yılda medeniyetlerin %98'i bitirmiş | ağaç ve çağlar kaldırıldı (Faz 1b-3); başlangıç medeniyetlerinin ilk kasabası medyan 8. yılda (126/128), ilk şehri 19. yılda (117/128); 30. yılda başkent kademesi ortalaması 2,67 | hayır |
| Medeniyetler 5 yerleşimde takılıyor | 98 medeniyetin 74'ü (%76) tam 5 yerleşimde | tam 5 kara yerleşimli medeniyet payı 30. yılda %1,6, 60. yılda %7,4 (denizaşırı koloniler dâhil 30. yılda tam 5: %0,8, 5+: %92); medeniyet başına 9,17 yerleşim (30. yıl) | hayır |
| Kamp sayısı düşüyor | 6,8'den 3,5'e iniyor | 8 (1. yıl) → en yüksek 9,5 (20. yıl) → 9 (30. yıl) → 9 (60. yıl); yıl sonu, yıllık dünya medyanı | hayır |
| Altın birikiyor | altın medyanı 78'den 6.503'e çıkıyor | 9,78 (1. yıl) → 480 (30. yıl) → 622 (60. yıl) | evet |
| İş gücü boşta | iş gücünün %43'ü boşta | 30. yılda %20, 60. yılda %16 (işe yerleşemeyen `zanaatçı` / bütün iş gücü, askerler dâhil; yıl içi ortalama) | hayır |
| Büyük olaylar seyreliyor | yıllık büyük olay 59'dan 28'e düşüyor | en yüksek 148 (43. yıl) → 122 (30. yıl) → 120 (60. yıl), yıllık dünya medyanı | hayır |
| Doğuş seviyesi şişiyor | 24. yıldan sonra herkes Sv5 doğuyor; efsane mekaniği ölü | 25–60. yıllarda Sv5+ doğanların payı %0; on yıllık doğuş seviyesi ortalaması 1,31 · 1,31 · 1,31 · 1,45 · 1,55 · 1,6 | hayır |
| Başkent düşmüyor | başkent fethedilemiyor (agents.ts:673) | 16 dünyada 64 başkent kaybı, 9 yok olma; 899 yerleşim fethi | hayır |

## On yıllık özet

Hücre: dünyalar arası medyan (p10–p90). Her dünyada on yılın yıllık değerlerinin ortalaması alınır: akış ölçülerinde yıllık ortalama, stok ölçülerinde yıl sonu değerlerinin ortalaması. Yüzdeler 0–1 paylardır.

| Ölçü | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| **Medeniyet** | | | | | | |
| Yaşayan medeniyet | 8 (7–9) | 8 (7–9) | 8 (7–9) | 8 (7–9,2) | 8 (7–9,6) | 8 (7–9,7) |
| Yeni medeniyet (yeniden doğan) | 0 | 0 | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 |
| Yok olan medeniyet | 0 (0–0,05) | 0 | 0 | 0 | 0 (0–0,05) | 0 (0–0,1) |
| Başkent kaybı (medeniyet yaşarken) | 0 (0–0,1) | 0,1 (0–0,1) | 0 (0–0,2) | 0 (0–0,25) | 0,05 (0–0,2) | 0 (0–0,2) |
| Çöküş (yok olma + başkent kaybı) | 0 (0–0,1) | 0,1 (0–0,1) | 0 (0–0,2) | 0 (0–0,25) | 0,05 (0–0,25) | 0,1 (0–0,2) |
| Yaşayan yerleşim | 27,3 (22,7–31,1) | 57,8 (45,1–65,8) | 71,3 (55,5–79,9) | 77 (63,2–87,7) | 82,4 (66,9–94,9) | 86,5 (69,8–103) |
| Medeniyet başına yerleşim | 3,32 (3,23–3,58) | 7,06 (6,44–7,51) | 8,76 (7,93–9,21) | 9,44 (8,63–10) | 9,8 (9,32–10,4) | 10,3 (9,62–11,1) |
| 5+ kara yerleşimli medeniyet payı | %29 (%23–%31) | %92 (%82–%98) | %97 (%88–%100) | %95 (%78–%100) | %89 (%81–%100) | %90 (%82–%100) |
| Kurulan yerleşim | 3,8 (3,15–4,35) | 1,75 (1,35–2,45) | 1,1 (0,65–1,4) | 0,65 (0,2–1,3) | 0,7 (0,3–1,4) | 0,6 (0,25–1,3) |
| Fethedilen yerleşim | 0,4 (0–0,7) | 0,75 (0,2–1,9) | 0,8 (0,25–2,05) | 0,9 (0,15–1,95) | 0,65 (0,05–2,3) | 0,85 (0–2,2) |
| Terk edilen yerleşim | 0 | 0 (0–0,2) | 0 (0–0,35) | 0,05 (0–0,25) | 0,1 (0–0,4) | 0,1 (0–0,4) |
| Toplam nüfus | 600 (478–691) | 2098 (1632–2374) | 3009 (2399–3267) | 3516 (2965–3854) | 3712 (3321–4274) | 4156 (3422–4479) |
| Altın medyanı (medeniyetler) | 41,9 (33,4–64,9) | 213 (138–254) | 355 (259–481) | 505 (366–638) | 489 (428–658) | 613 (455–808) |
| Boştaki iş gücü payı | %2,5 (%1,6–%3,8) | %18 (%15–%21) | %21 (%17–%24) | %19 (%16–%20) | %17 (%14–%19) | %16 (%15–%19) |
| Bölünme (ayrılıp kurulan medeniyet) | 0 | 0 | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 |
| En büyük medeniyetin yerleşimi | 4,8 (4,3–5,2) | 10,6 (8,7–12,3) | 13,8 (10,8–16) | 16,7 (12,8–20,1) | 19,3 (13,7–24,4) | 20,6 (14,8–27,1) |
| **Olaylar** | | | | | | |
| Olay | 218 (184–250) | 347 (312–436) | 396 (303–507) | 404 (350–548) | 460 (329–639) | 497 (338–800) |
| Büyük olay | 64,6 (55,9–72,7) | 97,8 (74,6–118) | 100 (77–151) | 114 (79,2–162) | 123 (67,2–192) | 112 (70,8–229) |
| **Savaş** | | | | | | |
| Muharebe | 8,35 (7,55–9,8) | 12,5 (10,5–15,7) | 12,6 (10,6–17,5) | 13,8 (9,75–17,9) | 13,9 (8,7–17,2) | 12,2 (8,1–18,7) |
| Başlayan savaş | 0,5 (0,1–0,95) | 1,3 (0,3–2,55) | 1,65 (0,3–3,35) | 1,35 (0,3–3,15) | 1,35 (0,15–3,15) | 1,4 (0,3–3,3) |
| Süren savaş (yıl sonu) | 0,15 (0–0,3) | 0,5 (0–1,1) | 0,75 (0,15–1,4) | 0,6 (0,2–2,1) | 0,65 (0,1–1,8) | 0,8 (0,1–1,55) |
| Yıl içinde süren savaş | 0,65 (0,1–1,2) | 1,6 (0,4–3,55) | 2,3 (0,5–4,4) | 1,9 (0,45–5,15) | 2 (0,4–4,75) | 2,35 (0,65–4,5) |
| Yağma akını (medeniyet) | 0,3 (0–0,95) | 1,25 (0–3,15) | 1,9 (0–3,9) | 2,3 (0–4,65) | 1,9 (0–3,9) | 1,45 (0–5,45) |
| Tarihî hak savaşı | 0 (0–0,1) | 0 (0–0,25) | 0 (0–0,35) | 0,05 (0–0,25) | 0 (0–0,35) | 0,1 (0–0,55) |
| Pakt gereği savaş | 0 (0–0,1) | 0,05 (0–0,55) | 0,1 (0–0,55) | 0 (0–0,45) | 0,05 (0–0,75) | 0,05 (0–0,65) |
| Kutsal Sefer çağrısı | 0 (0–0,1) | 0 (0–0,15) | 0 (0–0,2) | 0 (0–0,1) | 0 (0–0,2) | 0 (0–0,1) |
| İhanet (pakt çiğnendi) | 0 | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) |
| Savunma paktı (yıl sonu) | 0,15 (0–0,5) | 0,7 (0–1,65) | 1,1 (0,1–2,15) | 1,65 (0,1–2,3) | 1,15 (0,5–2) | 1,8 (0,3–2,8) |
| **Canavarlar** | | | | | | |
| Yaşayan kamp (yıl sonu) | 8,4 (7,95–8,6) | 9 (8,45–9,6) | 8,95 (8,65–9,75) | 8,85 (8,1–9,6) | 8,85 (8,05–9,35) | 8,95 (8,1–9,65) |
| Yaşayan kamp (yıl ort.) | 8,28 (7,99–8,43) | 8,84 (8,5–9,56) | 9,1 (8,49–9,56) | 8,95 (8,14–9,54) | 8,86 (8,14–9,32) | 8,83 (7,99–9,6) |
| Doğan kamp | 1,75 (1,5–2,35) | 3,55 (2,4–5) | 4,3 (1,8–5,75) | 5,05 (2,5–7) | 4,45 (1,75–7,2) | 4,9 (0,8–8,05) |
| Temizlenen kamp | 1,2 (1–1,75) | 3,4 (2,3–5) | 4,45 (1,75–5,7) | 5,05 (2,55–7,05) | 4,5 (1,7–7,05) | 4,85 (0,9–8,15) |
| Canavar baskını | 5,1 (4,55–5,85) | 5,25 (3,7–5,9) | 4,7 (3,4–6,25) | 4,25 (2,7–5,25) | 4,15 (2,3–5,7) | 4,2 (3,25–6,05) |
| Yaşayan trol ini (yıl sonu) | 0 | 1,15 (0,15–1,45) | 1,95 (0,85–2,85) | 2,2 (1,4–2,95) | 2,8 (1,8–3) | 2,7 (2,25–3) |
| Yaşayan ejderha (yıl sonu) | 0 | 0,6 (0,45–0,7) | 1 (0–1) | 1 (0–1) | 1 (0–1) | 1 (0–1) |
| Ejderha akını | 0 | 0,8 (0,55–0,9) | 1,05 (0,05–1,45) | 0,85 (0–1,3) | 0,65 (0–1,3) | 0,6 (0–1,3) |
| Kriz (anlatıcı) | 0 (0–0,15) | 0,3 (0–0,6) | 0,3 (0–0,55) | 0,4 (0–0,7) | 0,5 (0,1–0,75) | 0,55 (0,2–0,7) |
| Rahatlama dönemi (anlatıcı) | 0,25 (0,15–0,35) | 0,1 (0,05–0,3) | 0,1 (0–0,25) | 0,1 (0,05–0,2) | 0,1 (0–0,2) | 0,1 (0–0,2) |
| **Kahramanlar** | | | | | | |
| Doğan kahraman | 11,6 (10,7–13) | 12,8 (12–14,1) | 13,2 (11,1–15,2) | 12,8 (11,6–14,5) | 12,7 (11,1–14,4) | 12,1 (9,75–14,1) |
| Ölen kahraman | 2,65 (2,2–3,25) | 4,7 (2,9–5,45) | 4,1 (3,15–6,55) | 4,5 (2,8–5,6) | 5 (2,95–6,7) | 4,6 (2,55–5,45) |
| Emekli olan kahraman | 0 | 0 | 0,2 (0,05–0,35) | 0,95 (0,45–1,2) | 1,45 (1,05–2,3) | 1,2 (0,9–2,9) |
| Diyarı terk eden kahraman | 2,9 (2,45–3,55) | 4,35 (3,45–5) | 5,35 (4,4–8,1) | 5,85 (4,45–8,85) | 5,9 (4,05–8,25) | 5,4 (3,75–6,9) |
| Ölümden dönen kahraman | 0 | 0 | 0 | 0 | 0 | 0 |
| Efsane olan kahraman | 0 (0–0,2) | 0,05 (0–0,2) | 0,05 (0–0,2) | 0,1 (0–0,2) | 0 (0–0,15) | 0 (0–0,1) |
| Yaşayan kahraman (yıl sonu) | 34,9 (31,4–40,1) | 88 (75,3–97,1) | 113 (102–137) | 128 (111–154) | 136 (113–170) | 148 (117–181) |
| Doğuş seviyesi (ort.) | 1,31 (1,27–1,36) | 1,3 (1,27–1,35) | 1,31 (1,27–1,36) | 1,43 (1,36–1,57) | 1,57 (1,42–1,64) | 1,59 (1,48–1,72) |
| Ölüm seviyesi (ort.) | 1,59 (1,39–1,85) | 2,14 (2–2,51) | 2,73 (2,24–3,34) | 3,12 (2,54–3,7) | 3,43 (3,11–3,92) | 3,82 (3,31–4,99) |
| Yaşayan kahraman seviyesi (ort.) | 1,61 (1,54–1,69) | 2,49 (2,29–2,69) | 3,06 (2,59–3,44) | 3,54 (2,87–3,85) | 3,51 (3,11–4,27) | 3,57 (3,05–4,29) |
| En yüksek seviye (şimdiye dek) | 3,1 (2,7–3,55) | 5,75 (4,9–7,15) | 7,95 (7,15–9,35) | 9,85 (8,15–10) | 10 (8,75–10) | 10 (9,55–10) |
| **Han ve ticaret** | | | | | | |
| Ayakta han | 3,9 (3,55–4,15) | 5 (4,8–5,7) | 5,65 (5–6,1) | 6 (4,95–6,05) | 6 (5,1–6) | 5,95 (5–6) |
| Asılan ilan | 7,4 (5,4–8,75) | 5,9 (4,95–7,6) | 8,1 (3,45–9,65) | 8,3 (2–12,9) | 7,95 (3,6–14,5) | 9,15 (3,55–14,7) |
| Biten ilan | 1,05 (0,55–1,4) | 1,55 (0,75–2,8) | 1,3 (0,55–2,45) | 1,05 (0,45–2,9) | 1,25 (0,4–5) | 1,8 (0,2–6,45) |
| Ticaret seferi (kervan) | 27,3 (20,8–43,5) | 81,8 (61,1–105) | 103 (71,7–154) | 115 (82,4–175) | 131 (87,3–213) | 147 (86,7–246) |
| İkmal seferi | 3 (2,05–7,55) | 26,4 (19,5–35,2) | 40,2 (25,6–50,5) | 41,9 (27,6–52,4) | 47,6 (28,9–65,8) | 51 (32,2–72,7) |
| **Altın ve ambar** | | | | | | |
| Altın p90 (medeniyetler) | 158 (123–209) | 652 (497–756) | 1015 (701–1142) | 1157 (910–1320) | 1260 (1050–1385) | 1315 (1066–1461) |
| Bakım gideri (altın; asker, kahraman, L2–L3) | 344 (278–404) | 1879 (1450–2102) | 2709 (2275–3136) | 3109 (2434–3658) | 3185 (2579–4003) | 3351 (2716–4171) |
| Kamu işlerine (imar) harcanan altın | 57,5 (14,4–74,3) | 1027 (522–1254) | 2192 (1002–2572) | 2813 (2237–3371) | 3229 (2540–4161) | 3471 (2754–4291) |
| Ambarla beslenen amele tayını (gıda) | 233 (141–428) | 3366 (2712–4042) | 4151 (3224–5777) | 2683 (1308–4264) | 2612 (833–3681) | 1593 (520–3419) |
| Kamu işlerindeki (amele) iş gücü payı | %2 (%1,1–%2,7) | %17 (%14–%19) | %20 (%18–%22) | %19 (%18–%21) | %19 (%16–%22) | %17 (%15–%22) |
| İmar ortalaması (köy+, 0–100) | 1,09 (0,49–1,62) | 15,4 (10,2–16,3) | 22,3 (16,3–27,4) | 23,8 (18,7–28,2) | 22 (19,2–27,3) | 22,7 (15,7–30,1) |
| Canavar baskınında yitirilen altın | 10,9 (6,2–13,1) | 9,3 (0,7–16,5) | 8,15 (1,6–21) | 12 (3,15–26,5) | 5,6 (1,05–20,2) | 7,7 (0,45–21,6) |
| Ejderhaya giden altın (haraç + akın) | 0 | 122 (57,1–155) | 502 (0,95–745) | 489 (0–899) | 530 (0–931) | 548 (0–1123) |
| Hazinesi boş medeniyet payı | %0 | %0 | %0 | %0 | %0 | %0 |
| Kent tüketiminde yokluk payı (köy+; ekmek, bira ya da alet) | %1,4 (%0,8–%1,9) | %1,8 (%0,2–%6,3) | %22 (%9,1–%31) | %41 (%33–%48) | %41 (%38–%53) | %47 (%36–%58) |
| Ekmek ya da bira yokluğu payı (köy+) | %1,2 (%0,7–%1,8) | %1 (%0–%4,3) | %2,1 (%0,4–%6,2) | %5,2 (%1,2–%8,6) | %5,4 (%2,6–%11) | %7,5 (%3,7–%13) |
| Kıtlık (büyük olay) | 0 | 0 (0–0,05) | 0 (0–0,1) | 0 (0–0,05) | 0 | 0 |
| Açlıktan ölen | 0 | 0 | 0 | 0 (0–0,15) | 0 | 0 (0–0,45) |
| Kıtlık yardımı (sevkiyat) | 0 | 0 (0–0,1) | 0 (0–0,2) | 0 (0–0,45) | 0 | 0 |
| Kıtlıkta yüz çeviren | 0 | 0 (0–0,05) | 0 (0–0,1) | 0 (0–0,05) | 0 | 0 |
| Kıtlık akını | 0 | 0 | 0 (0–0,1) | 0 | 0 | 0 |
| Ambarın yettiği gün (medeniyet medyanı) | 25,2 (21,9–28,2) | 65,5 (54,4–71,1) | 61,7 (55,7–72,6) | 49,2 (44,1–56,2) | 46,9 (37,8–53,6) | 42,9 (31,5–53,7) |
| **Yerleşim kademesi** | | | | | | |
| Ortalama yerleşim kademesi | 0,67 (0,62–0,69) | 1,14 (1,06–1,19) | 1,25 (1,18–1,31) | 1,32 (1,22–1,4) | 1,35 (1,29–1,47) | 1,35 (1,26–1,47) |
| Köy+ yerleşim | 18 (14,7–19,8) | 45,4 (35,6–51) | 55,7 (45–65,4) | 62,7 (50,4–74,6) | 69 (56,4–81,8) | 73,5 (61,4–89,5) |
| Kasaba+ yerleşim | 3,4 (2,35–4,45) | 19,2 (14–22,4) | 25,8 (22,1–30,4) | 31,5 (26,4–35,1) | 33,4 (28–38,2) | 35,9 (32,1–40,2) |
| Şehir | 0 (0–0,05) | 2,25 (0,6–2,85) | 5,25 (4,4–6,5) | 6,35 (5,05–8,35) | 6,95 (5,95–9,25) | 7,55 (6,35–10,3) |
| Ortalama başkent kademesi | 1,15 (1,06–1,19) | 2,19 (2,03–2,29) | 2,57 (2,44–2,65) | 2,63 (2,48–2,85) | 2,71 (2,51–2,9) | 2,72 (2,61–2,88) |
| Kademe değişimi (yerleşim, yıl içinde) | 5,45 (4,15–6,25) | 5,55 (3,75–8,2) | 5,3 (3–7,7) | 4,25 (1,6–8,4) | 4,9 (1,7–9,8) | 4,9 (1,15–7,55) |
| **Deniz** | | | | | | |
| Liman (tersane) | 3,35 (1,95–5,4) | 10,7 (8,3–15,6) | 15,3 (12,2–20,6) | 17,8 (13,6–22) | 20,1 (15,4–25,2) | 21,8 (16,4–28,3) |
| Gemi (koga/tekne) | 5,8 (3,5–10,5) | 26,4 (19,6–39,5) | 37,6 (31,8–49,5) | 44,4 (34,5–55,6) | 46,8 (35,1–61) | 54,5 (35,3–66) |
| Kadırga | 0 | 5,6 (1,25–10,2) | 14 (8,45–21,9) | 17,1 (10,5–25) | 16,5 (11,2–28,4) | 18,3 (11,5–29) |
| Denizaşırı yerleşim | 1,05 (0,75–2,55) | 6,15 (3,95–9,3) | 8,8 (4,5–12,5) | 9,5 (4,5–13,1) | 9,9 (4,5–13,6) | 10 (4,5–14) |
| Deniz ticaret yolu (yıl sonu) | 1,2 (0,85–3,25) | 7,6 (5,8–14,7) | 11,8 (8–20,3) | 14,2 (8,3–23,7) | 17,5 (9,4–26,6) | 18,5 (10,1–32) |
| Deniz seferi (ticaret) | 0,4 (0–1,45) | 6,5 (2,95–19,9) | 13,1 (8,5–36) | 24,4 (10,8–41,7) | 25,8 (11,2–54) | 30,6 (11,2–58,1) |
| **v3: durum değişimi** | | | | | | |
| Yaşayan yerleşim (yıl ort.) | 25,5 (21,1–28,8) | 56,9 (44,5–64,5) | 70,9 (55,1–79,3) | 76,8 (62,9–87,4) | 82,3 (66,5–94,5) | 86,4 (69,6–103) |
| Büyük şehir (Şehir kademesi, yıl ort.) | 0 (0–0,02) | 1,96 (0,52–2,53) | 5,19 (4,15–6,32) | 6,36 (4,91–8,42) | 6,99 (5,84–9,1) | 7,45 (6,29–10,3) |
| El değiştiren yerleşim (fetih + bölünme) | 0,4 (0–0,7) | 0,75 (0,2–1,9) | 0,8 (0,25–2,05) | 0,9 (0,15–1,95) | 0,65 (0,05–2,3) | 0,85 (0–2,2) |
| El değiştiren büyük şehir | 0 | 0 | 0 (0–0,1) | 0 (0–0,05) | 0 (0–0,15) | 0 (0–0,2) |
| Büyük şehre hücum (kuşatma muharebesi) | 0 | 0 (0–0,25) | 0,1 (0–0,4) | 0,1 (0–0,5) | 0,1 (0–0,65) | 0,15 (0–0,55) |
| Büyük şehir yağmalandı, tutulmadı | 0 | 0 (0–0,15) | 0,1 (0–0,25) | 0,05 (0–0,4) | 0,1 (0–0,4) | 0,1 (0–0,3) |
| Yerleşim durum değişimi (kuruluş hariç hepsi) | 8,65 (7,15–11,3) | 12 (7,5–21,7) | 15,2 (8,8–25,7) | 14,5 (6,5–30,3) | 14,9 (6,15–30,6) | 16,2 (6,4–29,2) |
| Orta halka kaydı (Köy/Kasaba: kademe değişimi ya da terk) | 1,75 (1,3–2,15) | 3,05 (1,9–4,4) | 3,15 (1,55–4,65) | 2,55 (0,8–5,05) | 3,15 (1,1–6) | 3,2 (0,75–4,95) |
| Açlık başlayan yerleşim | 0 | 0 (0–0,25) | 0 (0–0,75) | 0 (0–1,7) | 0 (0–0,4) | 0 (0–0,95) |
| Salgın başlayan yerleşim | 0,1 (0–0,35) | 0,5 (0,15–0,95) | 0,6 (0,35–1,35) | 0,45 (0,2–1,4) | 0,9 (0,6–1,15) | 0,8 (0,25–1,4) |
| Yakılan/yanan yerleşim | 2,6 (1,7–3,55) | 4,75 (2,35–9,55) | 7,05 (3,75–11,8) | 6,9 (3,6–13,2) | 7,1 (2,95–15,7) | 7,4 (3,8–15,3) |
| Harabeye yeniden yerleşim | 0 | 0 (0–0,1) | 0 (0–0,15) | 0 (0–0,15) | 0 (0–0,1) | 0,1 (0–0,25) |

## Kahraman seviyeleri

### Doğuş seviyesi (bütün dünyalar, on yıl içinde doğanlar)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 |
|---|---|---|---|---|---|
| 1–10 | 1872 | 1,31 | %69 | %31 | %0 |
| 11–20 | 2065 | 1,31 | %69 | %31 | %0 |
| 21–30 | 2101 | 1,31 | %69 | %30 | %0,5 |
| 31–40 | 2062 | 1,45 | %60 | %35 | %5 |
| 41–50 | 2015 | 1,55 | %53 | %40 | %7,6 |
| 51–60 | 1937 | 1,6 | %49 | %41 | %9,2 |

### Ölüm seviyesi (bütün dünyalar, on yıl içinde ölenler)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 | Sv4 | Sv5 | Sv6 | Sv7 | Sv8 | Sv9 | Sv10 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 1–10 | 431 | 1,59 | %52 | %39 | %8,1 | %1,2 | %0 | %0 | %0 | %0 | %0 | %0 |
| 11–20 | 699 | 2,31 | %25 | %35 | %27 | %9,3 | %2,6 | %0,3 | %0,1 | %0,1 | %0 | %0 |
| 21–30 | 743 | 2,82 | %22 | %25 | %25 | %15 | %8,3 | %2,2 | %1,9 | %0,9 | %0 | %0 |
| 31–40 | 712 | 3,17 | %17 | %21 | %29 | %15 | %6,6 | %5,5 | %3,1 | %1,8 | %0,1 | %0,7 |
| 41–50 | 751 | 3,51 | %12 | %18 | %25 | %20 | %12 | %5,6 | %2 | %1,5 | %1,5 | %1,2 |
| 51–60 | 660 | 3,94 | %7,4 | %14 | %27 | %22 | %12 | %5,3 | %4,2 | %3,6 | %1,4 | %2,3 |

### Yaşayan kahramanların seviyesi (bütün dünyalar, on yılın son yılının sonunda)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 | Sv4 | Sv5 | Sv6 | Sv7 | Sv8 | Sv9 | Sv10 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 10 | 971 | 2,03 | %32 | %42 | %20 | %5,3 | %1,1 | %0,3 | %0 | %0 | %0 | %0 |
| 20 | 1644 | 2,79 | %20 | %31 | %21 | %12 | %10 | %3,6 | %1,3 | %0,4 | %0 | %0 |
| 30 | 2018 | 3,26 | %16 | %26 | %23 | %13 | %8,2 | %6 | %4,1 | %1,8 | %1 | %0,8 |
| 40 | 2231 | 3,5 | %13 | %24 | %25 | %14 | %7,8 | %5,2 | %4,3 | %3 | %2 | %1,6 |
| 50 | 2302 | 3,61 | %11 | %21 | %29 | %15 | %8,8 | %4,8 | %3,4 | %3,3 | %2 | %2 |
| 60 | 2563 | 3,76 | %7,5 | %19 | %30 | %18 | %10 | %4,9 | %3,7 | %2 | %2 | %2,8 |

### Ölüm nedenleri (bütün dünyalar)

| Neden | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 | Toplam |
|---|---|---|---|---|---|---|---|
| Kamp saldırısı | 335 | 384 | 267 | 240 | 263 | 169 | 1658 (%41) |
| Kuşatma | 24 | 134 | 190 | 165 | 204 | 187 | 904 (%23) |
| Trol | 0 | 48 | 117 | 140 | 106 | 50 | 461 (%12) |
| Ejderha | 0 | 75 | 114 | 85 | 63 | 95 | 432 (%11) |
| Düello | 12 | 22 | 17 | 37 | 44 | 32 | 164 (%4,1) |
| Bilinmiyor | 0 | 0 | 0 | 11 | 35 | 94 | 140 (%3,5) |
| Kervan soygunu | 8 | 13 | 15 | 20 | 23 | 25 | 104 (%2,6) |
| Han baskını (canavar) | 32 | 6 | 3 | 2 | 4 | 2 | 49 (%1,2) |
| Yağma akını | 2 | 7 | 2 | 9 | 3 | 2 | 25 (%0,6) |
| Yol pususu | 1 | 7 | 13 | 1 | 2 | 0 | 24 (%0,6) |
| Han baskını (medeniyet) | 3 | 3 | 4 | 2 | 4 | 4 | 20 (%0,5) |
| Yerleşim baskını (canavar) | 14 | 0 | 1 | 0 | 0 | 0 | 15 (%0,4) |

Neden, ölümün kaydedildiği andaki son muharebenin türünden (başlık ve taraflar) ya da suikast olayından çıkarılır.

## Olay türleri

Dünya başına yıllık olay sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Kahraman (`hero`) | 63,2 (49,7–83,1) | 163 (119–226) | 224 (155–273) | 243 (200–302) | 272 (218–383) | 333 (214–515) |
| Sefer/ilan (`quest`) | 17 (13,8–23,5) | 34,6 (22,6–54,1) | 49,4 (24,9–96,2) | 67,4 (30,5–114) | 68,8 (18,1–152) | 68,1 (19,7–179) |
| İnşaat (`build`) | 60,2 (48,7–72,3) | 66,5 (54,4–87,7) | 38 (26,8–47,4) | 20,4 (12,5–38,9) | 19,8 (10,4–39,2) | 13,9 (10,8–27,8) |
| Han (`inn`) | 13,6 (12,6–15,5) | 14,8 (13–16,6) | 14,6 (12,6–16,5) | 13,7 (12,1–16,2) | 14,6 (12,2–16,8) | 14 (11,6–16,3) |
| Kamp (`lair`) | 6,7 (5,6–7,95) | 11,8 (7,05–15,8) | 13,9 (5,8–18,7) | 16,9 (8,7–22,5) | 14,2 (4,65–22,9) | 15 (2,15–25,5) |
| Savaş (`war`) | 3,25 (0,25–6,4) | 9,75 (0,95–21,3) | 13,9 (1,55–29,2) | 13,3 (1,25–28,4) | 13,5 (0,8–34) | 13,8 (1,75–38,7) |
| Göç (`migration`) | 2,25 (0,9–3,35) | 4,4 (1,8–6,5) | 6,45 (3,75–9,85) | 7,45 (4,3–13,6) | 7,75 (4,35–13) | 9,2 (4,3–16,4) |
| Ölüm/terk (`death`) | 2,65 (2,2–3,3) | 4,7 (3,1–5,5) | 4,45 (3,25–6,55) | 4,8 (2,95–5,6) | 5,1 (3,15–6,95) | 4,75 (2,75–5,85) |
| epitaph | 2,65 (2,2–3,25) | 4,7 (2,9–5,45) | 4,1 (3,15–6,55) | 4,5 (2,8–5,6) | 5 (2,95–6,7) | 4,6 (2,55–5,45) |
| Baskın (`raid`) | 5,4 (4,95–6,6) | 4,5 (3,45–5,6) | 4,05 (2,65–5,55) | 3,8 (2,4–4,7) | 3,9 (2,45–4,85) | 3,85 (3,1–5,25) |
| Deniz (`sea`) | 4,3 (3,15–8,3) | 5,7 (3,1–7,95) | 4 (2,05–7,95) | 2,65 (0,65–7) | 3,5 (0,8–9,5) | 4,6 (1,15–9,3) |
| Ekonomi (`economy`) | 0,75 (0,35–1,2) | 4,15 (2,65–5,25) | 4,95 (3,45–7) | 5,15 (2,75–6,9) | 4,75 (2,15–7,9) | 4,15 (1,6–5,9) |
| Sınıf (`class`) | 3,55 (2,75–4,9) | 4,3 (3,45–5,7) | 4,05 (3,4–5,5) | 3,9 (3–5,1) | 3,7 (3–5,1) | 3,4 (3–5) |
| Büyüme (`growth`) | 7,15 (5,85–8,2) | 4,2 (3,2–5,2) | 2,5 (1,5–3,1) | 1,95 (0,9–2,85) | 1,75 (0,9–3,2) | 1,4 (0,65–2,85) |
| Yerleşim (`settle`) | 7,3 (6–8,2) | 3,3 (2,65–4,45) | 2,05 (1,15–2,7) | 1,3 (0,4–2,5) | 1,4 (0,7–2,8) | 1,1 (0,5–2,45) |
| Keşif (`discover`) | 8,55 (6,45–10,3) | 2,2 (1,5–3,45) | 1,25 (0,45–2,75) | 0,95 (0,3–2,3) | 1 (0,3–3,15) | 0,65 (0–2,8) |
| Dünya (`world`) | 0,35 (0,05–1,05) | 1,55 (0,7–2,3) | 2,05 (1,5–3,25) | 1,8 (1,2–3,75) | 2,7 (2,1–4,05) | 2,75 (1,7–4) |
| Ejderha (`dragon`) | 0 | 1,65 (1,25–1,95) | 2,35 (0,1–2,75) | 2,2 (0–2,65) | 2 (0–2,65) | 1,9 (0–2,6) |
| Ticaret (`trade`) | 2,5 (1,85–3,7) | 1,6 (0,75–3,1) | 1,05 (0,25–3,2) | 0,65 (0–3,7) | 0,95 (0,2–2,95) | 0,85 (0–3,1) |
| Diplomasi (`diplomacy`) | 1,2 (0,55–2,05) | 0,5 (0,25–1,2) | 0,3 (0–1,2) | 0,3 (0–1,6) | 0,1 (0–3,85) | 0,25 (0–2,7) |
| Gerginlik (`tension`) | 1,05 (0,6–1,9) | 0,4 (0,15–1,45) | 0,35 (0,05–1,2) | 0,2 (0–1,45) | 0,1 (0–2,2) | 0,25 (0–2) |
| Kriz (anlatıcı) (`crisis`) | 0 (0–0,15) | 0,3 (0–0,6) | 0,3 (0–0,55) | 0,4 (0–0,7) | 0,5 (0,1–0,75) | 0,55 (0,2–0,7) |
| Temas (`contact`) | 1,25 (0,95–1,9) | 0,4 (0,2–0,8) | 0,25 (0–0,45) | 0 (0–0,25) | 0,1 (0–0,2) | 0 (0–0,1) |
| Rahatlama (anlatıcı) (`relief`) | 0,25 (0,15–0,35) | 0,1 (0,05–0,3) | 0,1 (0–0,25) | 0,1 (0,05–0,2) | 0,1 (0–0,2) | 0,1 (0–0,25) |

## Büyük olay türleri

Dünya başına yıllık büyük olay sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Sefer/ilan (`quest`) | 13,2 (10,6–17,1) | 22,9 (16,1–36) | 34,4 (18–65,2) | 43,5 (21,7–77,4) | 44,8 (13,2–99,2) | 45 (15,5–121) |
| Kahraman (`hero`) | 6,2 (4,2–8,3) | 16,6 (12,1–23,6) | 21,2 (13,7–26,7) | 22,2 (14,2–28,1) | 25,6 (17,5–35,4) | 27,3 (18,4–47,5) |
| Kamp (`lair`) | 5,45 (4,45–6,25) | 8,8 (5,45–11,6) | 10,1 (4,55–13,6) | 12,3 (6,6–16,2) | 11 (3,85–16,2) | 11,1 (1,9–18,1) |
| Savaş (`war`) | 2,3 (0,25–4,9) | 6,5 (0,9–14,3) | 8,85 (1,4–18,7) | 9 (1–19,1) | 8,35 (0,65–19) | 8,35 (1,3–22,9) |
| Ölüm/terk (`death`) | 2,65 (2,2–3,3) | 4,7 (3,1–5,5) | 4,45 (3,25–6,55) | 4,8 (2,95–5,6) | 5,1 (3,15–6,95) | 4,75 (2,75–5,85) |
| epitaph | 2,65 (2,2–3,25) | 4,7 (2,9–5,45) | 4,1 (3,15–6,55) | 4,5 (2,8–5,6) | 5 (2,95–6,7) | 4,6 (2,55–5,45) |
| Baskın (`raid`) | 4,35 (3,95–5,65) | 3,55 (2,75–4,95) | 3,45 (2,1–4,6) | 3,35 (1,85–4,25) | 3,45 (2,25–4,25) | 3,2 (2,75–4,55) |
| İnşaat (`build`) | 5,6 (4,3–6,35) | 6,2 (3,8–8,65) | 2,35 (1,2–3,2) | 1,1 (0,25–2,35) | 1,25 (0,25–2,55) | 0,9 (0,05–1,5) |
| Han (`inn`) | 2,7 (1,75–3,2) | 2,8 (1,6–3,8) | 1,95 (1,25–2,75) | 1,7 (1,3–2,3) | 2,4 (1,75–3,45) | 2,55 (1,25–4,05) |
| Deniz (`sea`) | 2,05 (1,55–3,9) | 2,4 (1,4–3,55) | 1,7 (0,75–3,85) | 1,45 (0,3–4,45) | 1,7 (0,45–5,35) | 2,8 (0,75–5,7) |
| Ejderha (`dragon`) | 0 | 1,65 (1,25–1,95) | 2,35 (0,1–2,75) | 2,2 (0–2,65) | 2 (0–2,65) | 1,9 (0–2,6) |
| Yerleşim (`settle`) | 3,8 (3,15–4,35) | 1,75 (1,35–2,45) | 1,1 (0,65–1,4) | 0,65 (0,2–1,3) | 0,7 (0,3–1,4) | 0,6 (0,25–1,3) |
| Dünya (`world`) | 0,25 (0,05–0,75) | 1 (0,5–1,45) | 1,45 (1,1–2,05) | 1,4 (0,95–2,4) | 1,95 (1,55–2,95) | 2,15 (1,35–2,9) |
| Sınıf (`class`) | 1 (0,55–2) | 1,55 (1,1–2,4) | 1,3 (0,7–2,1) | 1 (0,5–2,25) | 1,15 (0,55–2) | 1 (0,2–2) |
| Keşif (`discover`) | 3 (2,2–4,1) | 1 (0,65–1,75) | 0,55 (0,15–1,3) | 0,6 (0,05–1,1) | 0,5 (0,05–1,7) | 0,3 (0–1,45) |
| Ekonomi (`economy`) | 0,2 (0,1–0,65) | 2,2 (1,6–2,6) | 1,2 (0,65–1,9) | 0,6 (0,2–0,9) | 0,35 (0,1–0,7) | 0,1 (0–0,45) |
| Büyüme (`growth`) | 2,45 (2,05–2,9) | 1,05 (0,75–1,2) | 0,3 (0,1–0,5) | 0,1 (0–0,5) | 0,1 (0–0,3) | 0 (0–0,4) |
| Ticaret (`trade`) | 1,15 (0,9–1,95) | 0,75 (0,25–1,4) | 0,45 (0,1–1,55) | 0,3 (0–1,8) | 0,45 (0,1–1,05) | 0,4 (0–1,5) |
| Gerginlik (`tension`) | 1,05 (0,6–1,9) | 0,4 (0,15–1,45) | 0,35 (0,05–1,2) | 0,2 (0–1,45) | 0,1 (0–2,2) | 0,25 (0–2) |
| Kriz (anlatıcı) (`crisis`) | 0 (0–0,15) | 0,3 (0–0,6) | 0,3 (0–0,55) | 0,4 (0–0,7) | 0,5 (0,1–0,75) | 0,55 (0,2–0,7) |
| Temas (`contact`) | 1,25 (0,95–1,9) | 0,4 (0,2–0,8) | 0,25 (0–0,45) | 0 (0–0,25) | 0,1 (0–0,2) | 0 (0–0,1) |
| Diplomasi (`diplomacy`) | 0,5 (0,3–1,1) | 0,4 (0,1–0,75) | 0,3 (0–0,75) | 0,3 (0–0,75) | 0,1 (0–2,1) | 0,25 (0–1,35) |
| Göç (`migration`) | 0,3 (0,2–0,5) | 0,3 (0,05–0,45) | 0,2 (0,1–0,4) | 0,3 (0,15–0,45) | 0,2 (0,05–0,45) | 0,2 (0–0,55) |
| Rahatlama (anlatıcı) (`relief`) | 0,25 (0,15–0,35) | 0,1 (0,05–0,3) | 0,1 (0–0,25) | 0,1 (0,05–0,2) | 0,1 (0–0,2) | 0,1 (0–0,25) |

## Muharebe türleri

Dünya başına yıllık muharebe sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Kamp saldırısı (`camp`) | 1,85 (1,4–2,35) | 4,05 (2,65–4,85) | 4,3 (2,4–5,45) | 4,5 (2,95–6,2) | 4,35 (1,8–6) | 4,5 (1,05–6,4) |
| Trol (`troll`) | 0 | 1,2 (0,15–2,15) | 2,2 (0,9–3,35) | 2,7 (1,35–3,8) | 2,45 (1,35–3,8) | 2,7 (1,1–4,8) |
| Yapı baskını (canavar) (`extRaid`) | 2,65 (2,2–3,6) | 1,9 (1,2–3,2) | 1,45 (0,45–3,05) | 1,2 (0,25–2,15) | 1,35 (0,7–1,95) | 1,2 (0,55–1,75) |
| Yağma akını (`plunder`) | 0,3 (0–0,95) | 1,25 (0–3,15) | 1,9 (0–3,9) | 2,3 (0–4,65) | 1,9 (0–3,9) | 1,45 (0–5,45) |
| Kuşatma (`siege`) | 0,45 (0–0,95) | 1 (0,2–2,4) | 1,45 (0,3–2,85) | 1,35 (0,3–3) | 1,1 (0,15–3) | 1,4 (0,15–3,05) |
| Yerleşim baskını (canavar) (`raid`) | 2,35 (1,95–2,75) | 1,2 (0,9–1,7) | 0,8 (0,3–1,35) | 0,35 (0,15–1,25) | 0,55 (0,25–1) | 0,5 (0,2–1,2) |
| Ejderha (`dragon`) | 0 | 0,7 (0,45–0,95) | 1 (0,1–1,4) | 0,8 (0–1,25) | 0,65 (0–1,3) | 0,6 (0–1,3) |
| Kervan soygunu (`robbery`) | 0,1 (0–0,75) | 0,2 (0–0,55) | 0,15 (0,05–0,35) | 0,3 (0,1–0,55) | 0,3 (0,15–0,5) | 0,2 (0,1–0,65) |
| Düello (`duel`) | 0 (0–0,2) | 0,1 (0–0,35) | 0,1 (0–0,2) | 0,2 (0–0,55) | 0,3 (0–0,45) | 0,1 (0–0,45) |
| Han baskını (canavar) (`innMonster`) | 0,2 (0,05–0,45) | 0,1 (0–0,1) | 0 (0–0,15) | 0 | 0 (0–0,05) | 0 |
| Yol pususu (`ambush`) | 0 (0–0,1) | 0,05 (0–0,1) | 0 (0–0,15) | 0 (0–0,05) | 0 (0–0,1) | 0 |
| Korsan savaşı (`pirate`) | 0 | 0 (0–0,15) | 0,05 (0–0,2) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,15) |
| Han baskını (medeniyet) (`innCiv`) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,05) | 0 (0–0,05) |
| Deniz savaşı (`naval`) | 0 | 0 (0–0,15) | 0 (0–0,2) | 0 (0–0,25) | 0 (0–0,15) | 0 (0–0,15) |

## Kamp türleri

Dünya başına yaşayan kamp (yıl sonu değerlerinin on yıllık ortalaması): dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Hobgoblin (`hobgoblin`) | 2,05 (1,45–2,5) | 3,4 (3–3,95) | 3,7 (3,05–4,7) | 3,55 (2,7–4,5) | 3,4 (2,6–4,25) | 3,5 (3–4,15) |
| Goblin (`goblin`) | 5,75 (5–6,1) | 2,6 (1,6–3,4) | 1,55 (1,1–2) | 1,6 (0,9–1,95) | 1,5 (0,45–2,15) | 1,2 (0,45–1,7) |
| Trol (`troll`) | 0 | 1,15 (0,15–1,45) | 1,95 (0,85–2,85) | 2,2 (1,4–2,95) | 2,8 (1,8–3) | 2,7 (2,25–3) |
| Ejderha (`dragon`) | 0 | 0,6 (0,45–0,7) | 1 (0–1) | 1 (0–1) | 1 (0–1) | 1 (0–1) |
| Bugbear (`bugbear`) | 0,55 (0,1–0,9) | 1,1 (0,5–1,35) | 0,6 (0,4–1,1) | 0,55 (0,3–1,15) | 0,5 (0,1–0,95) | 0,5 (0,1–0,9) |
| Korsan (`pirate`) | 0,25 (0,05–0,5) | 0,45 (0,2–1,1) | 0,3 (0–0,95) | 0,1 (0–0,75) | 0,1 (0–0,55) | 0,1 (0–0,7) |

## Kademe dağılımı

Yaşayan yerleşimlerin kademelere dağılımı, bütün dünyalar (yıl sonu; parantezde sayı).

| Yıl | 0 Kamp | 1 Köy | 2 Kasaba | 3 Şehir |
|---|---|---|---|---|
| 10 | %29 (211) | %47 (343) | %24 (175) | %0,3 (2) |
| 20 | %22 (221) | %43 (434) | %30 (301) | %5,9 (60) |
| 30 | %20 (228) | %40 (470) | %31 (366) | %8,7 (102) |
| 40 | %16 (207) | %42 (533) | %33 (419) | %8,4 (106) |
| 50 | %14 (198) | %45 (618) | %31 (428) | %9 (123) |
| 60 | %15 (214) | %45 (649) | %31 (455) | %9,3 (135) |

Başkentlerin kademelere dağılımı, bütün dünyalar (yıl sonu; parantezde sayı).

| Yıl | 0 Kamp | 1 Köy | 2 Kasaba | 3 Şehir |
|---|---|---|---|---|
| 10 | %0,8 (1) | %17 (21) | %81 (102) | %1,6 (2) |
| 20 | %0 (0) | %2,4 (3) | %53 (67) | %44 (56) |
| 30 | %0 (0) | %3,9 (5) | %29 (38) | %67 (86) |
| 40 | %0,7 (1) | %0,7 (1) | %31 (41) | %68 (91) |
| 50 | %0 (0) | %2,9 (4) | %24 (33) | %73 (101) |
| 60 | %0 (0) | %0,7 (1) | %19 (25) | %81 (109) |

## Dünyalar

| Seed | Medeniyet | Yerleşim | Nüfus | Çöküş | Efsane | En yüksek Sv | Doğan / ölü kahraman | İlk şehir (yıl, medyan) | Süre (sn) | Son hash |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | 8 | 81 | 4202 | 0 | 4 | 10 | 790 / 229 | 16 | 45,8 | `8aa42b4bbe03b9ce` |
| 2 | 7 | 59 | 3779 | 0 | 2 | 10 | 735 / 135 | 18 | 40,3 | `06997c687a5da750` |
| 3 | 10 | 108 | 4393 | 7 | 8 | 10 | 769 / 330 | 17 | 49,5 | `498f72c3de7ec395` |
| 4 | 8 | 91 | 4217 | 2 | 2 | 10 | 744 / 241 | 19,5 | 49,8 | `bd86b3ea99ac3aca` |
| 5 | 13 | 153 | 5540 | 11 | 3 | 10 | 885 / 298 | 17 | 80,7 | `fbc02b722b3d77e8` |
| 6 | 7 | 68 | 3585 | 2 | 4 | 10 | 674 / 198 | 24 | 35,7 | `ce8a0128cc794bfa` |
| 7 | 9 | 102 | 4522 | 5 | 5 | 10 | 809 / 261 | 18 | 58,4 | `9a5f9fba646182d3` |
| 8 | 8 | 84 | 4291 | 1 | 3 | 10 | 819 / 249 | 22 | 43,8 | `f07a5dce0bdc9e52` |
| 9 | 8 | 81 | 4451 | 3 | 6 | 10 | 653 / 242 | 19,5 | 40,5 | `2ea9c3aa08e3956a` |
| 10 | 10 | 94 | 3666 | 10 | 1 | 10 | 740 / 232 | 17 | 51,7 | `2e92cb8e56099a07` |
| 11 | 7 | 75 | 3502 | 1 | 6 | 10 | 731 / 224 | 15 | 34,4 | `828e5afaaa309c8b` |
| 12 | 9 | 102 | 4210 | 11 | 8 | 10 | 707 / 218 | 21 | 45,4 | `7782e64a544b6591` |
| 13 | 9 | 90 | 4425 | 5 | 3 | 10 | 765 / 283 | 20 | 43,6 | `d1c136b65cabdd7d` |
| 14 | 7 | 74 | 3363 | 2 | 0 | 10 | 720 / 206 | 22,5 | 38,9 | `8546b68f688635cc` |
| 15 | 6 | 84 | 3665 | 5 | 5 | 10 | 756 / 345 | 20 | 48,2 | `9356be0c8189419f` |
| 16 | 9 | 107 | 4562 | 8 | 7 | 10 | 755 / 305 | 22 | 45,1 | `7a9750626795aae2` |

Çöküşler:

- seed 3, 18. yıl (gün 709): Lirsesi Şehirleri başkenti kaybetti: Nağmeköy (Kanlıdiş Kabileleri aldı)
- seed 3, 21. yıl (gün 834): Tatlıçayır Loncası başkenti kaybetti: Fıçıköy (Karaörs Derinlikleri aldı)
- seed 3, 23. yıl (gün 901): Karaörs Derinlikleri başkenti kaybetti: Fıçıköy (Sınır Bekçileri aldı)
- seed 3, 41. yıl (gün 1639): Çarkyıldız Akademisi başkenti kaybetti: İzsürer (Kanlıdiş Kabileleri aldı)
- seed 3, 48. yıl (gün 1915): Kanlıdiş Kabileleri başkenti kaybetti: Sisliyamaç (Tatlıçayır Loncası aldı)
- seed 3, 54. yıl (gün 2153): Özgür Telliköprü başkenti kaybetti: Telliköprü (Kanlıdiş Kabileleri aldı)
- seed 3, 60. yıl (gün 2362): Kanlıdiş Kabileleri başkenti kaybetti: Sisliyamaç (Örsyürek Tapınak Klanı aldı)
- seed 4, 26. yıl (gün 1006): Kanlıdiş Kabileleri başkenti kaybetti: İzsürer (Güneştacı Krallığı aldı)
- seed 4, 28. yıl (gün 1096): Kanlıdiş Kabileleri başkenti kaybetti: Sisliyamaç (Güneştacı Krallığı aldı)
- seed 5, 5. yıl (gün 184): Karaörs Derinlikleri başkenti kaybetti: Kara Mihrap (Güneştacı Krallığı aldı)
- seed 5, 11. yıl (gün 403): Tatlıçayır Loncası başkenti kaybetti: Fıçıköy (Pulzırh Lejyonu aldı)
- seed 5, 23. yıl (gün 909): Kanlıdiş Kabileleri başkenti kaybetti: Boynuztepe (Pulzırh Lejyonu aldı)
- seed 5, 31. yıl (gün 1230): Kanlıdiş Kabileleri başkenti kaybetti: Kırkkapı (Karaörs Derinlikleri aldı)
- seed 5, 32. yıl (gün 1267): Karaörs Derinlikleri başkenti kaybetti: Kırkkapı (Lirsesi Şehirleri aldı)
- seed 5, 38. yıl (gün 1519): Kızıltepe Beyliği başkenti kaybetti: Kızıltepe (Kanlıdiş Kabileleri aldı)
- seed 5, 42. yıl (gün 1678): Sisova Boyu başkenti kaybetti: Sisova (Pulzırh Lejyonu aldı)
- seed 5, 45. yıl (gün 1794): Sisova Boyu başkenti kaybetti: Meşekale (Pulzırh Lejyonu aldı)
- seed 5, 49. yıl (gün 1941): Taşdere Serbest Şehri başkenti kaybetti: Taşdere (Karaörs Derinlikleri aldı)
- seed 5, 53. yıl (gün 2101): Sisova Boyu başkenti kaybetti: Yeni Aysırt (Kızılboynuz Soyu aldı)
- seed 5, 55. yıl (gün 2161): Güneştacı Krallığı başkenti kaybetti: Altınkapı (Kanlıdiş Kabileleri aldı)
- seed 6, 42. yıl (gün 1655): Karaörs Derinlikleri başkenti kaybetti: Taşhisar (Sınır Bekçileri aldı)
- seed 6, 52. yıl (gün 2070): Ceylanbük Klanı yok oldu
- seed 7, 9. yıl (gün 345): Kızılboynuz Soyu başkenti kaybetti: Kızılkül (Karaörs Derinlikleri aldı)
- seed 7, 16. yıl (gün 639): Lirsesi Şehirleri başkenti kaybetti: Nağmeköy (Karaörs Derinlikleri aldı)
- seed 7, 31. yıl (gün 1207): Karaörs Derinlikleri başkenti kaybetti: Geyikyurt (Yeşilyaprak Çemberi aldı)
- seed 7, 36. yıl (gün 1427): Karaörs Derinlikleri başkenti kaybetti: Geyikyurt (Kızılboynuz Soyu aldı)
- seed 7, 51. yıl (gün 2037): Karaörs Derinlikleri başkenti kaybetti: Söğütsırt (Kızılboynuz Soyu aldı)
- seed 8, 11. yıl (gün 431): Kızılboynuz Soyu başkenti kaybetti: Közsaray (Örsyürek Tapınak Klanı aldı)
- seed 9, 8. yıl (gün 291): Sınır Bekçileri başkenti kaybetti: Gözcüağaç (Karaörs Derinlikleri aldı)
- seed 9, 10. yıl (gün 369): Sınır Bekçileri yok oldu
- seed 9, 25. yıl (gün 968): Karaörs Derinlikleri başkenti kaybetti: Tuzyurt (Güneştacı Krallığı aldı)
- seed 10, 9. yıl (gün 329): Tatlıçayır Loncası başkenti kaybetti: Kavşakpazar (Kanlıdiş Kabileleri aldı)
- seed 10, 12. yıl (gün 457): Sınır Bekçileri başkenti kaybetti: Gözcüağaç (Kanlıdiş Kabileleri aldı)
- seed 10, 28. yıl (gün 1105): Çarkyıldız Akademisi başkenti kaybetti: Mürekkeptepe (Kanlıdiş Kabileleri aldı)
- seed 10, 42. yıl (gün 1677): Çarkyıldız Akademisi yok oldu
- seed 10, 45. yıl (gün 1775): Lirsesi Şehirleri başkenti kaybetti: Nağmeköy (Karaörs Derinlikleri aldı)
- seed 10, 48. yıl (gün 1897): Kurtoba Serbest Şehri başkenti kaybetti: Kurtoba (Kanlıdiş Kabileleri aldı)
- seed 10, 55. yıl (gün 2162): Kanlıdiş Kabileleri başkenti kaybetti: Okyayı (Örsyürek Tapınak Klanı aldı)
- seed 10, 57. yıl (gün 2247): Karaörs Derinlikleri başkenti kaybetti: Pusulakule (Lirsesi Şehirleri aldı)
- seed 10, 57. yıl (gün 2266): Kanlıdiş Kabileleri başkenti kaybetti: Ayburç (Rüzgâr Manastırı aldı)
- seed 10, 57. yıl (gün 2270): Örsyürek Tapınak Klanı başkenti kaybetti: Okyayı (Karaörs Derinlikleri aldı)
- seed 11, 6. yıl (gün 210): Kızılboynuz Soyu başkenti kaybetti: Közsaray (Kanlıdiş Kabileleri aldı)
- seed 12, 11. yıl (gün 430): Kızılboynuz Soyu başkenti kaybetti: Közsaray (Karaörs Derinlikleri aldı)
- seed 12, 13. yıl (gün 517): Kızılkül Boyu başkenti kaybetti: Kızılkül (Pulzırh Lejyonu aldı)
- seed 12, 16. yıl (gün 606): Kızılkül Boyu yok oldu
- seed 12, 24. yıl (gün 942): Kızılboynuz Soyu başkenti kaybetti: Kocapınar (Kanlıdiş Kabileleri aldı)
- seed 12, 27. yıl (gün 1058): Kılıçyurt Boyu başkenti kaybetti: Kılıçyurt (Pulzırh Lejyonu aldı)
- seed 12, 35. yıl (gün 1374): Karaörs Derinlikleri başkenti kaybetti: Közsaray (Kızılboynuz Soyu aldı)
- seed 12, 35. yıl (gün 1380): Kanlıdiş Kabileleri başkenti kaybetti: Sessizocak (Karaörs Derinlikleri aldı)
- seed 12, 41. yıl (gün 1617): Kılıçyurt Boyu başkenti kaybetti: Sessizocak (Pulzırh Lejyonu aldı)
- seed 12, 46. yıl (gün 1814): Karaörs Derinlikleri yok oldu
- seed 12, 56. yıl (gün 2230): Gökyayla Boyu başkenti kaybetti: Gökyayla (Pulzırh Lejyonu aldı)
- seed 12, 59. yıl (gün 2343): Gökyayla Boyu başkenti kaybetti: Yıldıztepe (Kızılboynuz Soyu aldı)
- seed 13, 6. yıl (gün 214): Sınır Bekçileri yok oldu
- seed 13, 37. yıl (gün 1451): Tatlıçayır Loncası başkenti kaybetti: Balköprü (Kanlıdiş Kabileleri aldı)
- seed 13, 46. yıl (gün 1811): Kızılboynuz Soyu başkenti kaybetti: Çankule (Pulzırh Lejyonu aldı)
- seed 13, 50. yıl (gün 1964): Yeşilyaprak Çemberi başkenti kaybetti: Boynuztepe (Kanlıdiş Kabileleri aldı)
- seed 13, 51. yıl (gün 2027): Yeşilkaya Tarikatı başkenti kaybetti: Yeşilkaya (Kızılboynuz Soyu aldı)
- seed 14, 10. yıl (gün 391): Yeşilyaprak Çemberi başkenti kaybetti: Sessizkoru (Karaörs Derinlikleri aldı)
- seed 14, 13. yıl (gün 508): Karaörs Derinlikleri başkenti kaybetti: Gözcüağaç (Çarkyıldız Akademisi aldı)
- seed 15, 12. yıl (gün 472): Lirsesi Şehirleri başkenti kaybetti: Nağmeköy (Kanlıdiş Kabileleri aldı)
- seed 15, 39. yıl (gün 1525): Pulzırh Lejyonu başkenti kaybetti: Kristalköy (Kanlıdiş Kabileleri aldı)
- seed 15, 43. yıl (gün 1705): Yeşilyaprak Çemberi başkenti kaybetti: Sessizkoru (Kanlıdiş Kabileleri aldı)
- seed 15, 57. yıl (gün 2257): Yeşilyaprak Çemberi yok oldu
- seed 15, 59. yıl (gün 2340): Çarkyıldız Akademisi yok oldu
- seed 16, 18. yıl (gün 711): Karaörs Derinlikleri başkenti kaybetti: Gölgeçarşı (Kanlıdiş Kabileleri aldı)
- seed 16, 27. yıl (gün 1062): Kanlıdiş Kabileleri başkenti kaybetti: Taşdere (Pulzırh Lejyonu aldı)
- seed 16, 35. yıl (gün 1365): Derinmihrap Klanı başkenti kaybetti: Derinmihrap (Kanlıdiş Kabileleri aldı)
- seed 16, 36. yıl (gün 1408): Kanlıdiş Kabileleri başkenti kaybetti: Derinmihrap (Pulzırh Lejyonu aldı)
- seed 16, 39. yıl (gün 1542): Balköprü Serbest Şehri başkenti kaybetti: Balköprü (Kanlıdiş Kabileleri aldı)
- seed 16, 41. yıl (gün 1634): Kanlıdiş Kabileleri başkenti kaybetti: Kavşakpazar (Pulzırh Lejyonu aldı)
- seed 16, 44. yıl (gün 1750): Tatlıçayır Loncası başkenti kaybetti: Sarıdere (Kanlıdiş Kabileleri aldı)
- seed 16, 54. yıl (gün 2151): Karaörs Derinlikleri yok oldu

## Yıllık ayrıntı

Hücre: medyan (p10–p90), 16 dünya. Yıl y = (y−1)·40+1 … y·40. günler. Bütün değerler `report.json` içinde (`metrics`), dünya başına değerler `../runs/f1b-5` altında.

### Medeniyet (1/3)

| Yıl | Yaşayan medeniyet | Yeni medeniyet (yeniden doğan) | Yok olan medeniyet | Başkent kaybı (medeniyet yaşarken) | Çöküş (yok olma + başkent kaybı) | Yaşayan yerleşim |
|---|---|---|---|---|---|---|
| 1 | 8 (7–9) | 0 | 0 | 0 | 0 | 8 (7–9) |
| 2 | 8 (7–9) | 0 | 0 | 0 | 0 | 9,5 (8–11,5) |
| 3 | 8 (7–9) | 0 | 0 | 0 | 0 | 14,5 (11,5–17) |
| 4 | 8 (7–9) | 0 | 0 | 0 | 0 | 20 (16–23) |
| 5 | 8 (7–9) | 0 | 0 | 0 | 0 | 26 (21–29) |
| 6 | 8 (7–9) | 0 | 0 | 0 | 0 (0–0,5) | 30 (25,5–35) |
| 7 | 8 (7–9) | 0 | 0 | 0 | 0 | 35,5 (28,5–39,5) |
| 8 | 8 (7–9) | 0 | 0 | 0 | 0 | 39 (32–44) |
| 9 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 43,5 (35,5–49) |
| 10 | 8 (7–9) | 0 | 0 | 0 | 0 (0–0,5) | 46 (38,5–52,5) |
| 11 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 49 (38,5–54,5) |
| 12 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 51 (40–57) |
| 13 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 52,5 (42,5–59,5) |
| 14 | 8 (7–9) | 0 | 0 | 0 | 0 | 56 (43–62) |
| 15 | 8 (7–9) | 0 | 0 | 0 | 0 | 58 (45,5–64) |
| 16 | 8 (7–9) | 0 | 0 | 0 | 0 (0–0,5) | 59,5 (46,5–67,5) |
| 17 | 8 (7–9) | 0 | 0 | 0 | 0 | 61 (47–70) |
| 18 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 62 (49,5–72) |
| 19 | 8 (7–9) | 0 | 0 | 0 | 0 | 63 (50,5–72,5) |
| 20 | 8 (7–9) | 0 | 0 | 0 | 0 | 65 (51–74) |
| 21 | 8 (7–9) | 0 | 0 | 0 | 0 | 68 (51,5–75) |
| 22 | 8 (7–9) | 0 | 0 | 0 | 0 | 70 (52–76,5) |
| 23 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 70 (52–78) |
| 24 | 8 (7–9) | 0 | 0 | 0 | 0 | 71 (54–79) |
| 25 | 8 (7–9) | 0 | 0 | 0 | 0 | 71,5 (55–80) |
| 26 | 8 (7–9) | 0 | 0 | 0 | 0 | 72 (56,5–80,5) |
| 27 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 72 (57–81,5) |
| 28 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 73,5 (58–82) |
| 29 | 8 (7–9) | 0 | 0 | 0 | 0 | 74,5 (59–82,5) |
| 30 | 8 (7–9) | 0 | 0 | 0 | 0 | 75 (60–83,5) |
| 31 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 76 (60,5–84,5) |
| 32 | 8 (7–9) | 0 | 0 | 0 | 0 | 76 (61,5–85) |
| 33 | 8 (7–9) | 0 | 0 | 0 | 0 | 76 (62–86) |
| 34 | 8 (7–9) | 0 | 0 | 0 | 0 | 76,5 (62,5–87) |
| 35 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 77 (62,5–88) |
| 36 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 77,5 (63,5–88) |
| 37 | 8 (7–9,5) | 0 (0–0,5) | 0 | 0 | 0 | 77,5 (63,5–88,5) |
| 38 | 8 (7–9,5) | 0 | 0 | 0 | 0 | 78,5 (64–89,5) |
| 39 | 8 (7–9,5) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 79 (64–90) |
| 40 | 8 (7–9,5) | 0 | 0 | 0 | 0 | 80,5 (63,5–90,5) |
| 41 | 8 (7–9,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 81 (64,5–91,5) |
| 42 | 8 (7–9,5) | 0 | 0 | 0 (0–0,5) | 0 (0–1) | 81 (65,5–92) |
| 43 | 8 (7–9,5) | 0 | 0 | 0 | 0 | 82 (65,5–93,5) |
| 44 | 8 (7–9,5) | 0 | 0 | 0 | 0 | 82 (66,5–94,5) |
| 45 | 8 (7–9,5) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 82 (66–94,5) |
| 46 | 8 (7–9,5) | 0 | 0 | 0 | 0 (0–0,5) | 82,5 (66,5–95) |
| 47 | 8 (7–9,5) | 0 | 0 | 0 | 0 | 83 (67,5–95,5) |
| 48 | 8 (7–9,5) | 0 (0–1) | 0 | 0 (0–0,5) | 0 (0–0,5) | 83 (67,5–97) |
| 49 | 8 (7–10) | 0 | 0 | 0 | 0 | 84 (69–97,5) |
| 50 | 8 (7–10) | 0 | 0 | 0 | 0 | 84,5 (69–98,5) |
| 51 | 8 (7–10) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 86 (68,5–99) |
| 52 | 8 (7–10) | 0 | 0 | 0 | 0 | 86 (69–99,5) |
| 53 | 8 (7–10) | 0 | 0 | 0 | 0 | 86 (69,5–102) |
| 54 | 8 (7–9,5) | 0 | 0 | 0 | 0 (0–0,5) | 86 (69–102) |
| 55 | 8 (7–9,5) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 86,5 (69–102) |
| 56 | 8 (7–9,5) | 0 | 0 | 0 | 0 | 86,5 (70–104) |
| 57 | 8 (7–10) | 0 | 0 | 0 | 0 (0–0,5) | 86 (70,5–105) |
| 58 | 8 (7–10) | 0 | 0 | 0 | 0 | 86,5 (70,5–106) |
| 59 | 8 (7–10) | 0 | 0 | 0 | 0 (0–0,5) | 87 (71–106) |
| 60 | 8 (7–10) | 0 | 0 | 0 | 0 | 87 (71–108) |

### Medeniyet (2/3)

| Yıl | Medeniyet başına yerleşim | 5+ kara yerleşimli medeniyet payı | Kurulan yerleşim | Fethedilen yerleşim | Terk edilen yerleşim | Toplam nüfus |
|---|---|---|---|---|---|---|
| 1 | 1 | %0 | 0 | 0 | 0 | 69,5 (60,5–80) |
| 2 | 1,13 (1–1,38) | %0 | 1 (0–3) | 0 | 0 | 108 (86,5–126) |
| 3 | 1,87 (1,54–2) | %0 | 5 (4–7) | 0 | 0 | 172 (136–207) |
| 4 | 2,44 (2,13–2,76) | %0 | 5 (4–6,5) | 0 | 0 | 283 (206–334) |
| 5 | 3,22 (2,93–3,47) | %0 (%0–%12) | 6 (4–7,5) | 0 (0–1) | 0 | 432 (330–516) |
| 6 | 3,88 (3,54–4,06) | %29 (%5,6–%35) | 5 (3–6) | 0,5 (0–1,5) | 0 | 607 (474–713) |
| 7 | 4,35 (4,06–4,59) | %44 (%29–%60) | 4 (3–5) | 0 (0–1,5) | 0 | 796 (624–935) |
| 8 | 4,87 (4,51–5,12) | %56 (%43–%69) | 4 (2–5,5) | 1 (0–2) | 0 | 992 (782–1126) |
| 9 | 5,4 (4,93–5,65) | %73 (%59–%87) | 4 (2,5–5,5) | 1 (0–1,5) | 0 | 1188 (942–1336) |
| 10 | 5,78 (5,2–6,19) | %86 (%57–%94) | 3 (1,5–5,5) | 1 (0–2,5) | 0 | 1368 (1090–1543) |
| 11 | 6 (5,42–6,5) | %86 (%64–%94) | 2 (0–3,5) | 0 (0–2,5) | 0 | 1512 (1203–1716) |
| 12 | 6,31 (5,54–6,63) | %88 (%73–%100) | 2 (1–4) | 1 (0–2) | 0 | 1648 (1332–1904) |
| 13 | 6,47 (5,88–6,94) | %88 (%73–%100) | 2 (1–3) | 1 (0–2) | 0 | 1818 (1438–2062) |
| 14 | 6,89 (6–7,18) | %88 (%82–%100) | 2 (1–4) | 0 (0–1,5) | 0 | 1938 (1507–2229) |
| 15 | 7,06 (6,34–7,53) | %89 (%80–%100) | 2 (1–3) | 1 (0–2) | 0 | 2082 (1580–2382) |
| 16 | 7,27 (6,64–7,89) | %100 (%83–%100) | 2 (1–3,5) | 1 (0–2,5) | 0 | 2185 (1668–2483) |
| 17 | 7,46 (6,71–7,94) | %100 (%87–%100) | 1 (1–3,5) | 1 (0–2,5) | 0 (0–1) | 2326 (1805–2596) |
| 18 | 7,53 (7,07–8,17) | %100 (%87–%100) | 1,5 (0,5–4) | 1 (0–2,5) | 0 | 2408 (1861–2740) |
| 19 | 7,71 (7,19–8,35) | %100 (%83–%100) | 1 (0–2) | 0,5 (0–3) | 0 | 2468 (1950–2758) |
| 20 | 8,12 (7,29–8,53) | %100 (%83–%100) | 2 (1–3) | 0,5 (0–2) | 0 | 2605 (1990–2862) |
| 21 | 8,22 (7,36–8,69) | %100 (%87–%100) | 1 (0–2,5) | 0,5 (0–3) | 0 (0–0,5) | 2687 (2070–2909) |
| 22 | 8,38 (7,43–8,94) | %100 (%87–%100) | 1 (0,5–2,5) | 1 (0–2) | 0 | 2666 (2142–3054) |
| 23 | 8,67 (7,43–8,94) | %100 (%88–%100) | 0,5 (0–2) | 1 (0–2,5) | 0 (0–0,5) | 2794 (2184–3138) |
| 24 | 8,75 (7,71–9,06) | %100 (%89–%100) | 1 (0–3) | 0,5 (0–2) | 0 | 2890 (2280–3189) |
| 25 | 8,86 (7,86–9,19) | %100 (%87–%100) | 1 (0–2) | 1 (0–2) | 0 (0–0,5) | 2989 (2282–3279) |
| 26 | 8,82 (7,87–9,41) | %100 (%83–%100) | 1 (0–3) | 0 (0–2) | 0 (0–1) | 3106 (2496–3300) |
| 27 | 8,83 (7,93–9,47) | %100 (%82–%100) | 0,5 (0–1) | 1 (0–4) | 0 (0–0,5) | 3130 (2510–3352) |
| 28 | 9,06 (7,99–9,58) | %100 (%87–%100) | 1 (0–2) | 1 (0–3,5) | 0 (0–0,5) | 3144 (2676–3421) |
| 29 | 9,13 (8,13–9,69) | %100 (%82–%100) | 1 (0–2) | 1 (0–3) | 0 | 3206 (2662–3536) |
| 30 | 9,17 (8,24–9,66) | %100 (%79–%100) | 1 (0–2) | 1 (0–2) | 0 | 3250 (2681–3652) |
| 31 | 9,28 (8,29–9,73) | %95 (%82–%100) | 1 (0–1,5) | 1 (0–3) | 0 | 3346 (2796–3612) |
| 32 | 9,37 (8,37–9,87) | %100 (%79–%100) | 1 (0–2) | 0 (0–1) | 0 | 3368 (2809–3780) |
| 33 | 9,37 (8,44–10) | %100 (%78–%100) | 0 (0–1,5) | 0,5 (0–2,5) | 0 | 3362 (2966–3780) |
| 34 | 9,42 (8,56–10,1) | %100 (%78–%100) | 1 (0–2) | 0,5 (0–2) | 0 (0–0,5) | 3436 (2950–3806) |
| 35 | 9,47 (8,34–10,1) | %100 (%78–%100) | 0 (0–1,5) | 0,5 (0–2,5) | 0 | 3486 (2909–3885) |
| 36 | 9,53 (8,47–10,1) | %100 (%78–%100) | 1 (0–2) | 1 (0–2,5) | 0 (0–1) | 3576 (2938–3804) |
| 37 | 9,5 (8,57–10,1) | %94 (%78–%100) | 1 (0–1,5) | 1 (0–2) | 0 | 3596 (2934–3928) |
| 38 | 9,54 (8,72–10,1) | %88 (%79–%100) | 0 (0–2) | 1 (0–3) | 0 (0–1) | 3538 (3100–3942) |
| 39 | 9,54 (8,78–10,1) | %88 (%78–%100) | 0 (0–1) | 0 (0–2,5) | 0 (0–1) | 3563 (3123–4003) |
| 40 | 9,46 (8,8–10,2) | %88 (%78–%100) | 1 (0–1,5) | 1 (0–2,5) | 0 | 3651 (3072–4024) |
| 41 | 9,54 (8,88–10,3) | %88 (%78–%100) | 1 (0–1) | 1 (0–3,5) | 0 | 3686 (3190–4106) |
| 42 | 9,64 (9,06–10,4) | %88 (%78–%100) | 1 (0–1,5) | 1 (0–1,5) | 0 | 3668 (3220–4244) |
| 43 | 9,79 (9,1–10,4) | %89 (%78–%100) | 1 (0–2) | 1 (0–2,5) | 0 (0–0,5) | 3534 (3218–4126) |
| 44 | 9,8 (9,29–10,4) | %89 (%76–%100) | 0,5 (0–2,5) | 1 (0–3) | 0 (0–0,5) | 3751 (3287–4244) |
| 45 | 9,88 (9,13–10,4) | %89 (%76–%100) | 1 (0–1,5) | 0,5 (0–3) | 0 (0–1) | 3767 (3287–4314) |
| 46 | 9,96 (9,13–10,6) | %89 (%75–%100) | 1 (0–1) | 1 (0–1,5) | 0 (0–1) | 3684 (3354–4316) |
| 47 | 10 (9,06–10,6) | %89 (%80–%100) | 1 (0–2) | 1 (0–1,5) | 0 (0–1) | 3687 (3391–4346) |
| 48 | 9,81 (8,92–10,4) | %88 (%75–%100) | 0,5 (0–2,5) | 1 (0–3) | 0 (0–1) | 3716 (3358–4372) |
| 49 | 9,77 (9,1–10,5) | %89 (%76–%100) | 1 (0–2) | 1 (0–2,5) | 0 (0–0,5) | 3782 (3426–4384) |
| 50 | 9,95 (9,1–10,6) | %88 (%76–%100) | 1 (0–2,5) | 0,5 (0–2) | 0 (0–0,5) | 3840 (3435–4350) |
| 51 | 10 (9,15–10,8) | %88 (%76–%100) | 1 (0–2,5) | 0,5 (0–2,5) | 0 (0–1) | 3908 (3270–4386) |
| 52 | 10,1 (9,67–10,8) | %88 (%79–%100) | 0,5 (0–1) | 1 (0–3) | 0 (0–0,5) | 4026 (3363–4578) |
| 53 | 10,2 (9,78–10,9) | %89 (%85–%100) | 1 (0–1,5) | 1 (0–2) | 0 | 4068 (3354–4444) |
| 54 | 10,3 (9,82–11) | %94 (%78–%100) | 0,5 (0–2) | 1 (0–3) | 0,5 (0–1) | 4119 (3394–4444) |
| 55 | 10,4 (9,82–11,1) | %96 (%83–%100) | 0 (0–1,5) | 1 (0–2) | 0 (0–0,5) | 4154 (3527–4514) |
| 56 | 10,4 (9,87–11,2) | %88 (%82–%100) | 1 (0–2,5) | 0 (0–3) | 0 | 4168 (3438–4630) |
| 57 | 10,5 (9,31–11,5) | %88 (%83–%100) | 0 (0–2) | 1 (0–4,5) | 0 (0–1) | 4202 (3444–4610) |
| 58 | 10,6 (9,36–11,6) | %89 (%82–%100) | 1 (0–2) | 0 (0–2) | 0 (0–1) | 4194 (3499–4616) |
| 59 | 10,6 (9,46–11,7) | %89 (%82–%100) | 0 (0–2) | 1 (0–3) | 0 | 4188 (3484–4452) |
| 60 | 10,6 (9,56–11,8) | %91 (%87–%100) | 0 (0–1,5) | 0,5 (0–3) | 0 (0–0,5) | 4214 (3544–4542) |

### Medeniyet (3/3)

| Yıl | Altın medyanı (medeniyetler) | Boştaki iş gücü payı | Bölünme (ayrılıp kurulan medeniyet) | En büyük medeniyetin yerleşimi |
|---|---|---|---|---|
| 1 | 9,78 (9,54–22) | %0 | 0 | 1 |
| 2 | 27,4 (16–49,4) | %0 | 0 | 2 (1–2) |
| 3 | 31,2 (24–47,4) | %0 | 0 | 3 (2–3) |
| 4 | 29,2 (19,8–41,3) | %0 (%0–%0,1) | 0 | 4 (3–4) |
| 5 | 34,5 (21,2–59,3) | %0,4 (%0–%1,8) | 0 | 4 (4–5) |
| 6 | 29,6 (20,4–58,2) | %1,4 (%0–%3,9) | 0 | 5 (4,5–6) |
| 7 | 41,4 (24,7–106) | %3 (%1,2–%5,6) | 0 | 6 (5–6,5) |
| 8 | 42,6 (26,1–95,7) | %4,4 (%1,8–%8,1) | 0 | 7 (6–7,5) |
| 9 | 80,2 (53,4–125) | %6,5 (%2,1–%9,9) | 0 | 8 (7–9) |
| 10 | 64,4 (32,7–133) | %8,3 (%6,2–%11) | 0 | 8 (8–9) |
| 11 | 110 (48,9–191) | %12 (%9,1–%14) | 0 | 8,5 (8–10) |
| 12 | 126 (59,5–181) | %15 (%11–%19) | 0 | 9 (8–10) |
| 13 | 142 (76,9–248) | %17 (%13–%19) | 0 | 10 (8–11) |
| 14 | 214 (64,1–308) | %17 (%15–%23) | 0 | 10 (8–12) |
| 15 | 179 (116–335) | %19 (%15–%22) | 0 | 10 (8–12,5) |
| 16 | 248 (77,3–322) | %20 (%16–%23) | 0 | 10,5 (9–12,5) |
| 17 | 205 (77,3–377) | %21 (%15–%24) | 0 | 11,5 (9–13,5) |
| 18 | 247 (145–441) | %20 (%18–%26) | 0 | 12 (9–14) |
| 19 | 207 (166–365) | %21 (%18–%25) | 0 | 12 (9,5–14,5) |
| 20 | 294 (218–464) | %23 (%18–%26) | 0 | 13 (10–14,5) |
| 21 | 289 (193–536) | %22 (%19–%25) | 0 | 13 (10–15) |
| 22 | 312 (180–456) | %21 (%19–%24) | 0 | 12,5 (10,5–15,5) |
| 23 | 319 (158–440) | %23 (%17–%25) | 0 | 13 (10,5–15,5) |
| 24 | 297 (218–516) | %22 (%15–%25) | 0 | 13,5 (10,5–16) |
| 25 | 308 (145–522) | %22 (%16–%26) | 0 | 13,5 (10,5–16) |
| 26 | 373 (196–519) | %20 (%15–%25) | 0 | 13,5 (10,5–16) |
| 27 | 411 (225–582) | %20 (%16–%25) | 0 | 14 (10,5–16,5) |
| 28 | 414 (218–595) | %19 (%17–%23) | 0 | 14,5 (10,5–17) |
| 29 | 458 (283–648) | %20 (%17–%23) | 0 | 15 (10,5–17) |
| 30 | 480 (270–672) | %20 (%16–%22) | 0 | 15,5 (11–18) |
| 31 | 506 (259–693) | %18 (%15–%22) | 0 | 15,5 (11,5–19) |
| 32 | 500 (240–631) | %18 (%15–%22) | 0 | 15,5 (12–19) |
| 33 | 558 (315–674) | %18 (%16–%22) | 0 | 16 (12–19,5) |
| 34 | 538 (324–664) | %19 (%16–%22) | 0 | 16 (12,5–20) |
| 35 | 547 (171–671) | %19 (%16–%22) | 0 | 16 (12,5–20) |
| 36 | 497 (247–687) | %18 (%16–%20) | 0 | 17 (13–20) |
| 37 | 488 (280–677) | %18 (%16–%20) | 0 (0–0,5) | 17 (13–20) |
| 38 | 515 (379–658) | %17 (%15–%20) | 0 | 17,5 (13–20,5) |
| 39 | 562 (449–677) | %18 (%15–%20) | 0 | 17,5 (13–21) |
| 40 | 527 (365–710) | %18 (%15–%22) | 0 | 17,5 (13–21,5) |
| 41 | 562 (411–667) | %17 (%15–%20) | 0 | 19 (13–22) |
| 42 | 546 (442–628) | %17 (%14–%19) | 0 | 18,5 (13–23) |
| 43 | 534 (374–717) | %16 (%14–%20) | 0 | 19 (13,5–23) |
| 44 | 457 (306–659) | %16 (%14–%19) | 0 | 19 (13,5–24) |
| 45 | 531 (431–659) | %17 (%14–%19) | 0 | 19,5 (13,5–24,5) |
| 46 | 546 (379–846) | %18 (%15–%20) | 0 | 19,5 (14–25) |
| 47 | 538 (332–674) | %17 (%11–%20) | 0 | 19,5 (14–25) |
| 48 | 531 (279–659) | %16 (%13–%20) | 0 (0–1) | 19,5 (13,5–25,5) |
| 49 | 515 (322–756) | %17 (%14–%19) | 0 | 20 (13,5–26) |
| 50 | 566 (432–782) | %17 (%12–%20) | 0 | 20 (14–26) |
| 51 | 473 (242–724) | %17 (%14–%19) | 0 | 20 (14–26,5) |
| 52 | 579 (405–842) | %16 (%14–%20) | 0 | 20,5 (14–26,5) |
| 53 | 513 (417–761) | %16 (%13–%20) | 0 | 20,5 (14,5–26,5) |
| 54 | 573 (361–910) | %15 (%14–%19) | 0 | 21 (15–27) |
| 55 | 607 (449–1028) | %16 (%14–%19) | 0 | 20 (15–27) |
| 56 | 609 (553–848) | %15 (%14–%20) | 0 | 20,5 (15–27) |
| 57 | 655 (385–900) | %17 (%14–%20) | 0 | 21 (15–27,5) |
| 58 | 617 (371–831) | %17 (%14–%20) | 0 | 21 (15–27,5) |
| 59 | 593 (431–847) | %17 (%15–%20) | 0 | 21 (15–27,5) |
| 60 | 622 (460–879) | %16 (%13–%19) | 0 | 21 (15,5–28) |

### Olaylar

| Yıl | Olay | Büyük olay |
|---|---|---|
| 1 | 96 (85,5–109) | 33 (30,5–38,5) |
| 2 | 99,5 (77–148) | 32 (18–49) |
| 3 | 134 (115–166) | 37,5 (29,5–49,5) |
| 4 | 176 (144–208) | 54,5 (36,5–64,5) |
| 5 | 210 (176–250) | 63,5 (50–74,5) |
| 6 | 248 (198–294) | 79,5 (57,5–106) |
| 7 | 286 (198–326) | 88,5 (57,5–102) |
| 8 | 288 (239–347) | 82 (67,5–112) |
| 9 | 320 (288–388) | 86 (75–112) |
| 10 | 320 (292–374) | 84,5 (63,5–100) |
| 11 | 324 (282–375) | 77,5 (69–107) |
| 12 | 328 (266–406) | 91,5 (63,5–108) |
| 13 | 329 (276–411) | 84 (68,5–106) |
| 14 | 358 (302–430) | 89,5 (65–126) |
| 15 | 346 (312–473) | 96,5 (75,5–120) |
| 16 | 384 (299–491) | 107 (83–142) |
| 17 | 366 (272–462) | 99 (62,5–142) |
| 18 | 349 (272–474) | 95,5 (68–112) |
| 19 | 350 (252–486) | 106 (51–140) |
| 20 | 390 (292–476) | 116 (68,5–138) |
| 21 | 389 (283–496) | 104 (76,5–130) |
| 22 | 390 (258–470) | 106 (65–153) |
| 23 | 407 (272–458) | 99 (67–148) |
| 24 | 371 (274–494) | 95,5 (68–143) |
| 25 | 394 (304–510) | 100 (74,5–144) |
| 26 | 414 (270–514) | 95 (65,5–176) |
| 27 | 404 (334–530) | 103 (75–164) |
| 28 | 408 (318–496) | 99 (69,5–142) |
| 29 | 391 (308–548) | 102 (74–162) |
| 30 | 387 (322–572) | 122 (62,5–167) |
| 31 | 384 (320–532) | 102 (66–160) |
| 32 | 383 (264–554) | 95,5 (55–158) |
| 33 | 427 (286–548) | 120 (50,5–162) |
| 34 | 416 (328–559) | 110 (82,5–176) |
| 35 | 396 (306–526) | 99 (64–169) |
| 36 | 414 (296–514) | 103 (72–169) |
| 37 | 402 (299–565) | 116 (67–169) |
| 38 | 414 (342–564) | 116 (70–176) |
| 39 | 434 (318–604) | 117 (72,5–189) |
| 40 | 444 (348–624) | 133 (78–189) |
| 41 | 455 (322–671) | 122 (76,5–189) |
| 42 | 444 (293–654) | 126 (78,5–161) |
| 43 | 501 (372–643) | 148 (72,5–190) |
| 44 | 510 (312–624) | 128 (61–174) |
| 45 | 482 (276–598) | 118 (53,5–184) |
| 46 | 442 (297–584) | 120 (47–199) |
| 47 | 428 (302–636) | 124 (40,5–200) |
| 48 | 456 (267–636) | 128 (45–210) |
| 49 | 440 (288–759) | 125 (51–222) |
| 50 | 479 (342–626) | 114 (64,5–192) |
| 51 | 464 (326–668) | 118 (60–216) |
| 52 | 430 (322–704) | 99 (46,5–203) |
| 53 | 475 (367–764) | 114 (75–202) |
| 54 | 498 (377–794) | 107 (70,5–242) |
| 55 | 440 (282–889) | 108 (45,5–242) |
| 56 | 502 (309–871) | 116 (57–268) |
| 57 | 538 (328–898) | 119 (56–256) |
| 58 | 554 (284–844) | 124 (49–240) |
| 59 | 532 (286–834) | 122 (62,5–256) |
| 60 | 504 (328–876) | 120 (63,5–232) |

### Savaş (1/2)

| Yıl | Muharebe | Başlayan savaş | Süren savaş (yıl sonu) | Yıl içinde süren savaş | Yağma akını (medeniyet) | Tarihî hak savaşı |
|---|---|---|---|---|---|---|
| 1 | 0 (0–1) | 0 | 0 | 0 | 0 | 0 |
| 2 | 1 (0–2) | 0 | 0 | 0 | 0 | 0 |
| 3 | 7 (5,5–9) | 0 | 0 | 0 | 0 | 0 |
| 4 | 9 (8–11,5) | 0 | 0 | 0 | 0 | 0 |
| 5 | 11 (8,5–13) | 0 (0–1) | 0 (0–0,5) | 0 (0–1) | 0 (0–1,5) | 0 |
| 6 | 11 (8–15) | 0 (0–2) | 0 (0–0,5) | 1 (0–2) | 0 (0–1) | 0 |
| 7 | 11,5 (7,5–15) | 1 (0–1,5) | 0 (0–1) | 1 (0–2) | 0 (0–2) | 0 |
| 8 | 11,5 (8,5–15,5) | 0 (0–1,5) | 0 (0–1) | 1 (0–2) | 1 (0–2) | 0 |
| 9 | 11 (9–15,5) | 1 (0–2,5) | 0 (0–1,5) | 1 (0–2,5) | 1 (0–1,5) | 0 |
| 10 | 10 (8–14) | 1 (0–2,5) | 0 (0–1) | 2 (0–3,5) | 1 (0–3) | 0 (0–1) |
| 11 | 10 (7,5–13,5) | 0,5 (0–2,5) | 0 (0–1,5) | 1 (0–3) | 1,5 (0–3) | 0 (0–1) |
| 12 | 12,5 (7,5–16,5) | 1 (0–3) | 0 (0–1,5) | 1,5 (0–4) | 1 (0–3) | 0 |
| 13 | 12 (7,5–15,5) | 1 (0–2,5) | 0 (0–1) | 1,5 (0–3) | 0 (0–3) | 0 |
| 14 | 11,5 (9–16,5) | 0 (0–2,5) | 0 (0–1) | 1 (0–3) | 1,5 (0–3) | 0 |
| 15 | 12 (8,5–16,5) | 1 (0–4) | 0 (0–1,5) | 1 (0–4,5) | 1 (0–3) | 0 (0–0,5) |
| 16 | 15 (10–19) | 1 (0–2,5) | 0 (0–2) | 1 (0–3,5) | 1,5 (0–4) | 0 |
| 17 | 13 (7,5–18,5) | 1 (0–3,5) | 0 (0–1) | 2 (0–5) | 1,5 (0–5,5) | 0 |
| 18 | 13 (11–18) | 1 (0–4) | 0 (0–1) | 1 (0–4) | 2 (0–5) | 0 (0–0,5) |
| 19 | 12,5 (7,5–18) | 1 (0–4,5) | 0 (0–2,5) | 1 (0–6) | 1 (0–4) | 0 |
| 20 | 14,5 (9,5–18) | 0,5 (0–3) | 0 (0–2) | 2,5 (0–3) | 1,5 (0–5,5) | 0 |
| 21 | 13 (9,5–18,5) | 1,5 (0–4) | 0,5 (0–2,5) | 1,5 (0–6) | 1 (0–4) | 0 (0–1) |
| 22 | 11,5 (7,5–19,5) | 1 (0–2,5) | 0 (0–2) | 2 (0–4) | 2 (0–3,5) | 0 (0–1) |
| 23 | 14 (9,5–19) | 1 (0–5) | 0,5 (0–2,5) | 2 (0–6) | 2 (0–4,5) | 0 (0–1) |
| 24 | 13 (9,5–15,5) | 1,5 (0–2,5) | 0 (0–2) | 2 (0–4) | 1,5 (0–5) | 0 |
| 25 | 13 (9–17) | 2 (0–4) | 0 (0–2) | 2,5 (0–5) | 2 (0–4) | 0 (0–0,5) |
| 26 | 14 (8,5–17,5) | 1 (0–2,5) | 1 (0–2) | 1,5 (0–4) | 2 (0–4,5) | 0 |
| 27 | 17 (9–21) | 2 (0–4,5) | 0 (0–2) | 3 (0–6) | 2 (0–4) | 0 (0–0,5) |
| 28 | 12 (9–17) | 1 (0–3,5) | 0 (0–1,5) | 1,5 (0–4,5) | 1,5 (0–3,5) | 0 |
| 29 | 12,5 (10,5–17) | 2 (0,5–4) | 0,5 (0–2,5) | 2 (0,5–5) | 2 (0–4) | 0 |
| 30 | 14 (9–20,5) | 1 (0–4) | 0 (0–2) | 2 (0,5–4) | 1 (0–5) | 0 (0–0,5) |
| 31 | 13 (8–19,5) | 2 (0–3,5) | 0 (0–1) | 2,5 (0,5–4) | 2 (0–3,5) | 0 |
| 32 | 12 (8,5–18) | 1 (0–4) | 1 (0–2) | 1,5 (0–4) | 1 (0–4,5) | 0 (0–1) |
| 33 | 12,5 (7,5–22,5) | 0,5 (0–3) | 0 (0–2) | 1 (0–4,5) | 1 (0–5) | 0 |
| 34 | 13 (9,5–18,5) | 1,5 (0–4) | 0 (0–2,5) | 2 (0–4) | 1 (0–4) | 0 (0–1) |
| 35 | 15 (7,5–19,5) | 2 (0–3,5) | 1 (0–3) | 2 (0,5–4) | 2,5 (0–5) | 0 |
| 36 | 13,5 (9,5–18,5) | 0,5 (0–5,5) | 0 (0–4) | 2 (0–6) | 1 (0–5) | 0 |
| 37 | 15 (7–18) | 1 (0–3,5) | 0 (0–2) | 2 (0–5,5) | 1 (0–4,5) | 0 (0–0,5) |
| 38 | 14 (9,5–19) | 2 (0–4,5) | 0 (0–3) | 2 (0–5) | 2 (0–6,5) | 0 |
| 39 | 13,5 (7,5–19,5) | 1 (0–4,5) | 0,5 (0–3,5) | 2 (0–6,5) | 2 (0–5,5) | 0 (0–1) |
| 40 | 14,5 (8,5–21,5) | 0,5 (0–3,5) | 1 (0–3) | 2 (0–5) | 2 (0–4,5) | 0 (0–0,5) |
| 41 | 14,5 (8,5–22,5) | 2 (0–3,5) | 0 (0–2,5) | 3 (0,5–6) | 2,5 (0–4) | 0 (0–0,5) |
| 42 | 14 (8,5–17) | 2 (0–5,5) | 1 (0–3) | 3 (0–6) | 1,5 (0–3) | 0 (0–1) |
| 43 | 14 (8,5–23) | 0,5 (0–4) | 0 (0–3) | 2 (0–5,5) | 1,5 (0–4) | 0 |
| 44 | 14,5 (10–17) | 1,5 (0–3) | 0 (0–1,5) | 2 (0–6) | 1 (0–4) | 0 |
| 45 | 12,5 (6–18) | 2 (0–3) | 1 (0–3) | 2,5 (0–5) | 2 (0–3,5) | 0 (0–0,5) |
| 46 | 14 (7–18,5) | 1,5 (0–3) | 0,5 (0–3) | 2,5 (0–5,5) | 1,5 (0–3,5) | 0 (0–1) |
| 47 | 12,5 (5,5–17) | 0 (0–2) | 0 (0–1,5) | 2 (0–4,5) | 1,5 (0–4) | 0 |
| 48 | 13,5 (6,5–21,5) | 2 (0–5) | 1 (0–2) | 2 (0–6) | 2 (0–4,5) | 0 (0–1) |
| 49 | 14 (6–21) | 1 (0–3) | 0,5 (0–2,5) | 1,5 (0–5) | 1 (0–7) | 0 |
| 50 | 12,5 (6,5–18) | 1,5 (0–4) | 1 (0–2) | 2 (0–4,5) | 1 (0–4,5) | 0 (0–0,5) |
| 51 | 10 (8–22) | 2 (0–4,5) | 0,5 (0–2,5) | 2,5 (0–5,5) | 0,5 (0–4,5) | 0 (0–1) |
| 52 | 11,5 (6,5–19,5) | 1 (0–4,5) | 0 (0–3) | 2 (0–6) | 1 (0–4) | 0 (0–0,5) |
| 53 | 14 (11,5–17,5) | 1,5 (0–4) | 1 (0–2) | 2,5 (0–5,5) | 1 (0–5) | 0 |
| 54 | 12,5 (7,5–21) | 0,5 (0–4,5) | 0,5 (0–2) | 2 (0–5,5) | 1,5 (0–3,5) | 0 (0–1) |
| 55 | 14 (6–20,5) | 2 (0–4) | 0,5 (0–2) | 3 (0,5–4) | 2 (0–6,5) | 0 (0–1) |
| 56 | 13 (6–19) | 2 (0–4) | 1 (0–2,5) | 2 (0,5–5) | 0,5 (0–5,5) | 0 |
| 57 | 12,5 (7–19,5) | 2 (0–4,5) | 0 (0–1,5) | 3 (1–6) | 2 (0–3,5) | 0 (0–1) |
| 58 | 14 (7–20) | 2 (0–5) | 1 (0–4) | 2 (0–5) | 1 (0–6,5) | 0 (0–0,5) |
| 59 | 14,5 (6,5–23,5) | 1 (0–5,5) | 0 (0–2) | 2,5 (0,5–6) | 2 (0–5) | 0 (0–1) |
| 60 | 13 (5,5–24) | 1 (0–5,5) | 0 (0–3,5) | 1,5 (0–6) | 1,5 (0–8,5) | 0 (0–1) |

### Savaş (2/2)

| Yıl | Pakt gereği savaş | Kutsal Sefer çağrısı | İhanet (pakt çiğnendi) | Savunma paktı (yıl sonu) |
|---|---|---|---|---|
| 1 | 0 | 0 | 0 | 0 |
| 2 | 0 | 0 | 0 | 0 |
| 3 | 0 | 0 | 0 | 0 |
| 4 | 0 | 0 | 0 | 0 |
| 5 | 0 | 0 | 0 | 0 |
| 6 | 0 | 0 | 0 | 0 (0–0,5) |
| 7 | 0 | 0 | 0 | 0 (0–1) |
| 8 | 0 | 0 | 0 | 0 (0–1) |
| 9 | 0 | 0 | 0 | 0,5 (0–1) |
| 10 | 0 (0–1) | 0 | 0 | 0,5 (0–1) |
| 11 | 0 (0–0,5) | 0 | 0 | 0,5 (0–1) |
| 12 | 0 (0–1) | 0 | 0 | 1 (0–1,5) |
| 13 | 0 | 0 | 0 | 1 (0–2) |
| 14 | 0 | 0 | 0 (0–0,5) | 1 (0–1,5) |
| 15 | 0 (0–1) | 0 | 0 (0–0,5) | 0,5 (0–1,5) |
| 16 | 0 (0–0,5) | 0 | 0 | 0,5 (0–1,5) |
| 17 | 0 (0–1,5) | 0 | 0 | 0,5 (0–2) |
| 18 | 0 | 0 | 0 | 1 (0–2) |
| 19 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 20 | 0 (0–0,5) | 0 (0–0,5) | 0 | 1 (0–2,5) |
| 21 | 0 (0–1) | 0 | 0 | 1 (0–2,5) |
| 22 | 0 (0–0,5) | 0 (0–0,5) | 0 | 1 (0–2,5) |
| 23 | 0 (0–0,5) | 0 | 0 | 1 (0–2,5) |
| 24 | 0 | 0 | 0 (0–0,5) | 1 (0–2) |
| 25 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 26 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 27 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 28 | 0 (0–1) | 0 (0–0,5) | 0 | 1 (0,5–2) |
| 29 | 0 (0–1) | 0 | 0 | 1,5 (0–2) |
| 30 | 0 (0–1) | 0 | 0 | 2 (0–2) |
| 31 | 0 (0–1) | 0 | 0 | 2 (0–2) |
| 32 | 0 | 0 | 0 | 1,5 (0–2) |
| 33 | 0 | 0 | 0 | 2 (0–2) |
| 34 | 0 (0–1) | 0 | 0 | 1,5 (0–2) |
| 35 | 0 (0–1) | 0 | 0 | 1,5 (0–2,5) |
| 36 | 0 (0–1) | 0 | 0 | 1,5 (0–2,5) |
| 37 | 0 (0–0,5) | 0 | 0 | 1,5 (0–2,5) |
| 38 | 0 (0–0,5) | 0 | 0 | 1,5 (0–2) |
| 39 | 0 (0–0,5) | 0 | 0 | 1,5 (0,5–3) |
| 40 | 0 (0–1) | 0 | 0 | 2 (0,5–2,5) |
| 41 | 0 (0–1) | 0 (0–0,5) | 0 | 2 (0,5–2,5) |
| 42 | 0 (0–1) | 0 | 0 (0–0,5) | 1 (0,5–2) |
| 43 | 0 (0–0,5) | 0 | 0 | 1 (0,5–2) |
| 44 | 0 (0–1) | 0 | 0 | 1 (0,5–2) |
| 45 | 0 (0–1) | 0 | 0 | 1 (0,5–2) |
| 46 | 0 | 0 | 0 | 1 (0,5–2) |
| 47 | 0 (0–0,5) | 0 | 0 | 1 (0,5–2) |
| 48 | 0 (0–1) | 0 (0–0,5) | 0 | 1 (0,5–2) |
| 49 | 0 (0–1) | 0 | 0 | 1 (0,5–2) |
| 50 | 0 (0–1) | 0 | 0 | 1 (0,5–2) |
| 51 | 0 | 0 | 0 | 1 (0,5–2) |
| 52 | 0 (0–1,5) | 0 | 0 | 2 (0,5–2) |
| 53 | 0 | 0 | 0 | 1,5 (0,5–2) |
| 54 | 0 (0–1) | 0 | 0 | 2 (0,5–2,5) |
| 55 | 0 (0–0,5) | 0 | 0 | 2 (0,5–3) |
| 56 | 0 (0–1) | 0 | 0 (0–0,5) | 1,5 (0,5–3) |
| 57 | 0 (0–1) | 0 | 0 | 1,5 (0–3,5) |
| 58 | 0 (0–0,5) | 0 | 0 | 2 (0–3,5) |
| 59 | 0 (0–1,5) | 0 | 0 | 2 (0–3,5) |
| 60 | 0 (0–1,5) | 0 | 0 | 2 (0–3,5) |

### Canavarlar (1/2)

| Yıl | Yaşayan kamp (yıl sonu) | Yaşayan kamp (yıl ort.) | Doğan kamp | Temizlenen kamp | Canavar baskını | Yaşayan trol ini (yıl sonu) |
|---|---|---|---|---|---|---|
| 1 | 8 | 7,33 (7,16–7,49) | 5 (5–6) | 0 (0–1) | 0 | 0 |
| 2 | 8 | 8 (7,98–8) | 0 (0–1,5) | 0 (0–1,5) | 0 | 0 |
| 3 | 8 (8–8,5) | 8 (8–8,18) | 0 (0–0,5) | 0 | 7 (5,5–8) | 0 |
| 4 | 8 (8–9) | 8 (7,98–8,61) | 1 (0–1) | 1 (0–1) | 7 (6–9) | 0 |
| 5 | 8 (8–9) | 8 (7,94–9) | 1 (0–2) | 0,5 (0–2) | 8 (6–10) | 0 |
| 6 | 8 (7,5–9) | 8,19 (7,93–9,06) | 1 (0–2,5) | 2 (0–3) | 7 (5,5–10) | 0 |
| 7 | 8,5 (8–9,5) | 8,53 (7,86–9,14) | 2 (1–4) | 2 (0,5–4) | 6 (4–9) | 0 |
| 8 | 9 (8–9,5) | 8,84 (7,94–9,05) | 2 (0,5–3,5) | 2 (1–3,5) | 6 (3,5–10) | 0 |
| 9 | 8 (8–10) | 8,78 (7,96–9,48) | 3,5 (1–4,5) | 3 (2–5) | 5 (3–7,5) | 0 |
| 10 | 8 (7,5–9) | 8,43 (7,89–9,34) | 2 (1,5–4,5) | 3 (1,5–4,5) | 4 (3–7) | 0 |
| 11 | 9 (8–9) | 8,08 (7,86–8,9) | 3,5 (1,5–5) | 3 (1–4,5) | 5 (2,5–6,5) | 0 (0–0,5) |
| 12 | 8 (8–9,5) | 8,51 (7,9–9,11) | 3 (1,5–4) | 3 (1–5) | 5 (2–7,5) | 0 (0–1) |
| 13 | 8 (7,5–10) | 8,23 (7,88–9,75) | 3 (2–4) | 3 (2–4,5) | 4 (2–5,5) | 0,5 (0–1) |
| 14 | 9 (8–10) | 8,64 (8,03–10,2) | 4 (2,5–5) | 3 (2–5,5) | 6 (3–7,5) | 1 (0–1,5) |
| 15 | 9 (8–10) | 8,78 (8,09–9,21) | 4 (2–5,5) | 4 (2,5–5) | 4 (2–6,5) | 1 (0–2,5) |
| 16 | 9 (8–10) | 8,93 (8,16–9,69) | 4 (2–7) | 4 (1–7) | 5,5 (4–8) | 1 (0,5–2) |
| 17 | 9 (7,5–11) | 9,01 (8,64–10,5) | 3,5 (1,5–6) | 3 (1–7) | 4 (3–7,5) | 1 (0–2) |
| 18 | 9 (8,5–11) | 9,08 (8,8–10,7) | 3 (1,5–7) | 3 (2–6) | 5 (3,5–7,5) | 2 (0–2) |
| 19 | 9 (8,5–10,5) | 9,16 (8,69–10,5) | 3 (1–7) | 2 (1–8,5) | 4,5 (3–8) | 2 (0–2) |
| 20 | 9,5 (9–10,5) | 9,36 (8,79–10,2) | 5 (2–7) | 5 (1,5–7) | 6 (3–8) | 2 (0–3) |
| 21 | 9 (8,5–11) | 9,4 (8,71–10,6) | 4,5 (1–5,5) | 4 (1,5–6) | 5 (2,5–7,5) | 1,5 (0–2) |
| 22 | 9 (8–10) | 9,23 (8,29–10,6) | 3,5 (2–6,5) | 3,5 (2–7) | 4,5 (1,5–7) | 2 (1–2,5) |
| 23 | 9 (8–10,5) | 8,9 (8,55–9,95) | 4 (1–8) | 5 (1,5–7) | 4 (3–7,5) | 2 (0–3) |
| 24 | 9 (9–10,5) | 8,96 (8,55–10,1) | 4 (2–6) | 4 (2–6) | 5 (2,5–6) | 2,5 (0,5–3) |
| 25 | 9 (8–9,5) | 8,95 (8,63–9,74) | 4 (1–7) | 4 (1–7) | 4 (3–6) | 2 (1–3) |
| 26 | 9 (8–10) | 8,86 (7,94–9,93) | 4,5 (1,5–7) | 4,5 (1–7) | 5 (2,5–7,5) | 2,5 (0,5–3) |
| 27 | 9 (8–11,5) | 8,94 (7,83–10,3) | 5 (3–6,5) | 4,5 (1,5–6,5) | 6 (3,5–9) | 2,5 (1–3) |
| 28 | 9 (8–10) | 9,01 (8,18–10,5) | 4 (0–5) | 4 (1,5–5,5) | 3,5 (2–7,5) | 2 (0,5–3) |
| 29 | 9 (8–10) | 8,91 (8,49–9,44) | 4,5 (1–6,5) | 4 (2–6,5) | 4 (1,5–6,5) | 2 (1–3) |
| 30 | 9 (8–10) | 8,81 (8,26–10,1) | 5,5 (2,5–7,5) | 5 (2–7,5) | 4,5 (1,5–6,5) | 2 (0–3) |
| 31 | 9 (8–10) | 8,93 (8,35–9,9) | 4 (1–6) | 4,5 (1–6,5) | 4,5 (2–6,5) | 3 (1–3) |
| 32 | 9 (8–10,5) | 8,81 (8,19–9,8) | 5 (1,5–7) | 5 (1,5–7) | 5 (2–6) | 2 (0–3) |
| 33 | 9 (8–10,5) | 8,95 (8,06–9,93) | 5 (3–7) | 5 (2,5–7) | 4,5 (2,5–7,5) | 3 (0,5–3) |
| 34 | 9 (8–10) | 8,94 (8,04–9,83) | 4,5 (2–8) | 5 (2–7,5) | 3,5 (1,5–6) | 2 (1–3) |
| 35 | 9 (8–11) | 8,83 (7,95–10,4) | 4,5 (1,5–8) | 4 (1–7) | 4,5 (1,5–6) | 2 (0,5–3) |
| 36 | 9 (8–10) | 8,96 (8,11–10,3) | 4,5 (2–6,5) | 4,5 (2,5–7) | 5 (2–6) | 2,5 (0,5–3) |
| 37 | 9 (7,5–9) | 8,85 (8,01–9,48) | 5 (2–8) | 6 (2–8) | 4 (3–6) | 2 (1–3) |
| 38 | 9 (8–9) | 8,81 (7,9–9,13) | 4,5 (1,5–7,5) | 4,5 (1,5–7) | 3 (2,5–7) | 2,5 (1–3) |
| 39 | 9 (8–9) | 8,78 (7,88–9,26) | 5 (1,5–7,5) | 5 (1,5–7) | 3,5 (1–5,5) | 2,5 (1,5–3) |
| 40 | 9 (8–10) | 8,94 (8,06–9,34) | 6 (2–8,5) | 5,5 (1,5–9) | 4,5 (2,5–7) | 3 (1–3) |
| 41 | 9 (7,5–9) | 8,81 (8–9,45) | 5 (1–7) | 5 (2–7) | 3 (0,5–5,5) | 2,5 (2–3) |
| 42 | 9 (7,5–9,5) | 8,8 (7,99–9,48) | 5,5 (2–8) | 5,5 (2,5–8) | 3,5 (1,5–5,5) | 3 (0,5–3) |
| 43 | 9 (8–10) | 8,89 (7,98–9,63) | 5 (2,5–7,5) | 5 (2–7,5) | 4 (0,5–7) | 3 (0,5–3) |
| 44 | 9 (8–9,5) | 8,83 (7,83–9,26) | 5 (0,5–8) | 5 (2–7) | 4 (2–6,5) | 3 (1–3) |
| 45 | 9 (8–10) | 8,95 (7,98–9,19) | 4 (1–6,5) | 4 (1–6,5) | 3,5 (1–7) | 3 (2–3) |
| 46 | 9 (8–9) | 8,96 (7,93–9,69) | 3,5 (1–7,5) | 3,5 (1–8) | 4,5 (2,5–7,5) | 3 (1,5–3) |
| 47 | 9 (7,5–9) | 8,9 (8,13–9,16) | 3,5 (1–7) | 4,5 (1–8) | 4 (1,5–7) | 3 (2–3) |
| 48 | 9 (8–9) | 8,89 (7,83–9,08) | 3,5 (0–7,5) | 4 (0–7) | 4 (2–7,5) | 3 (2–3) |
| 49 | 9 (8–9,5) | 8,95 (7,85–9,39) | 4 (1–8,5) | 4 (0,5–7,5) | 5 (3–7,5) | 3 (2–3) |
| 50 | 9 (8–10,5) | 9 (7,75–9,63) | 4,5 (1,5–6,5) | 4 (1,5–6) | 4,5 (2–6) | 3 (2–3) |
| 51 | 9 (8–10) | 8,93 (7,83–9,75) | 4 (0–7,5) | 3,5 (1–7) | 5 (2–7) | 3 (2–3) |
| 52 | 9 (8–10,5) | 9 (7,88–9,61) | 4 (0,5–7,5) | 3,5 (0,5–8) | 4 (1,5–6) | 3 (1,5–3) |
| 53 | 9 (8–10) | 8,98 (8,04–10,6) | 5 (0,5–7,5) | 5 (1,5–7,5) | 5 (2,5–6,5) | 3 (2,5–3) |
| 54 | 9 (7,5–9) | 8,85 (8,15–9,23) | 5 (0,5–7,5) | 5 (0,5–9) | 4,5 (3–6) | 3 (1,5–3) |
| 55 | 9 (8–9,5) | 8,69 (7,88–9,26) | 4 (1–8,5) | 4,5 (1–8) | 4 (1–7,5) | 3 (2–3) |
| 56 | 9 (8–10) | 8,95 (7,69–9,24) | 4 (1–10) | 3,5 (0,5–9,5) | 5 (2,5–6) | 3 (2–3) |
| 57 | 9 (8–10) | 8,74 (7,95–9,69) | 5 (0,5–9) | 5 (1–9) | 3,5 (1,5–7,5) | 3 (2–3) |
| 58 | 8,5 (8–9,5) | 8,73 (7,99–9,26) | 5 (0–8,5) | 6 (0–8,5) | 3,5 (2–7) | 3 (2–3) |
| 59 | 8,5 (7,5–9,5) | 8,8 (7,89–9,08) | 4,5 (0,5–7) | 4 (0–7) | 5 (2–7) | 3 (2–3) |
| 60 | 9 (8–10,5) | 8,83 (7,69–9,93) | 4,5 (1–8,5) | 4 (1–8) | 3 (2–7,5) | 3 (2–3) |

### Canavarlar (2/2)

| Yıl | Yaşayan ejderha (yıl sonu) | Ejderha akını | Kriz (anlatıcı) | Rahatlama dönemi (anlatıcı) |
|---|---|---|---|---|
| 1 | 0 | 0 | 0 | 0 |
| 2 | 0 | 0 | 0 | 0 |
| 3 | 0 | 0 | 0 | 0 |
| 4 | 0 | 0 | 0 | 1 |
| 5 | 0 | 0 | 0 | 0 (0–1) |
| 6 | 0 | 0 | 0 | 0,5 (0–1) |
| 7 | 0 | 0 | 0 | 0 (0–1) |
| 8 | 0 | 0 | 0 | 0 |
| 9 | 0 | 0 | 0 (0–1) | 0 (0–0,5) |
| 10 | 0 | 0 | 0 | 0 |
| 11 | 0 | 0 | 0,5 (0–1) | 0 |
| 12 | 0 | 0 | 0 (0–1) | 0 |
| 13 | 0 | 0 | 0 (0–1) | 0 (0–0,5) |
| 14 | 0 (0–1) | 0 (0–1) | 0,5 (0–1) | 0 |
| 15 | 1 (0–1) | 0 (0–2) | 0 | 0 (0–0,5) |
| 16 | 1 (0,5–1) | 1,5 (0–2) | 0 (0–1) | 0 (0–1) |
| 17 | 1 | 1 (1–2) | 0 (0–0,5) | 0 (0–1) |
| 18 | 1 | 1 (1–2) | 0 (0–1) | 0 (0–0,5) |
| 19 | 1 | 1 (0,5–2) | 0 (0–1) | 0 (0–1) |
| 20 | 1 (0,5–1) | 1 (0,5–2) | 0 (0–1) | 0 (0–1) |
| 21 | 1 (0–1) | 1 (0–2) | 0 (0–0,5) | 0 (0–1) |
| 22 | 1 (0–1) | 1 (0–2) | 0 (0–1) | 0 (0–1) |
| 23 | 1 (0–1) | 0,5 (0–1,5) | 0 (0–1) | 0 |
| 24 | 1 (0–1) | 1 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 25 | 1 (0–1) | 1 (0–2) | 0 (0–1) | 0 |
| 26 | 1 (0–1) | 1 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 27 | 1 (0–1) | 1 (0–2) | 0 (0–1) | 0 |
| 28 | 1 (0–1) | 0,5 (0–2) | 0 (0–1) | 0 |
| 29 | 1 (0–1) | 1 (0–1,5) | 0 (0–1) | 0 (0–0,5) |
| 30 | 1 (0–1) | 1 (0–2) | 0 (0–1) | 0 |
| 31 | 1 (0–1) | 1 (0–1) | 0 (0–1) | 0 (0–0,5) |
| 32 | 1 (0–1) | 1 (0–2) | 1 (0–1) | 0 |
| 33 | 1 (0–1) | 1 (0–1) | 1 (0–1) | 0 |
| 34 | 1 (0–1) | 0 (0–2) | 0 (0–1) | 0 (0–1) |
| 35 | 1 (0–1) | 1 (0–2) | 0 (0–1) | 0 |
| 36 | 1 (0–1) | 1 (0–1,5) | 0,5 (0–1) | 0 (0–1) |
| 37 | 1 (0–1) | 0,5 (0–2) | 0 (0–1) | 0 (0–1) |
| 38 | 1 (0–1) | 1 (0–2) | 0 (0–1) | 0 |
| 39 | 1 (0–1) | 0,5 (0–1,5) | 0 (0–1) | 0 (0–1) |
| 40 | 1 (0–1) | 0,5 (0–1,5) | 1 (0–1) | 0 (0–1) |
| 41 | 1 (0–1) | 0 (0–1,5) | 0 (0–1) | 0 (0–0,5) |
| 42 | 1 (0–1) | 0,5 (0–1,5) | 0,5 (0–1) | 0 |
| 43 | 1 (0–1) | 0,5 (0–2) | 1 (0–1) | 0 (0–0,5) |
| 44 | 1 (0–1) | 0 (0–2) | 0 (0–1) | 0 |
| 45 | 1 (0–1) | 0,5 (0–1,5) | 0 (0–1) | 0 |
| 46 | 1 (0–1) | 0,5 (0–2) | 1 (0–1) | 0 (0–0,5) |
| 47 | 1 (0–1) | 0 (0–1) | 0 (0–1) | 0 |
| 48 | 1 (0–1) | 0 (0–2) | 1 (0–1) | 0 |
| 49 | 1 (0–1) | 0 (0–1) | 0 (0–1) | 0 (0–0,5) |
| 50 | 1 (0–1) | 0 (0–2) | 1 (0–1) | 0 (0–0,5) |
| 51 | 1 (0–1) | 0 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 52 | 1 (0–1) | 0 (0–1,5) | 0 (0–1) | 0 |
| 53 | 1 (0–1) | 0 (0–1,5) | 1 (0–1) | 0 |
| 54 | 1 (0–1) | 0 (0–2) | 1 (0–1) | 0 (0–1) |
| 55 | 1 (0–1) | 0 (0–1) | 0 (0–1) | 0 |
| 56 | 1 (0–1) | 1 (0–2) | 1 (0–1) | 0 (0–0,5) |
| 57 | 1 (0–1) | 0,5 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 58 | 1 (0–1) | 0 (0–2) | 1 (0–1) | 0 |
| 59 | 1 (0–1) | 0 (0–1,5) | 1 (0–1) | 0 |
| 60 | 1 (0–1) | 0 (0–1,5) | 0 (0–1) | 0 (0–1) |

### Kahramanlar (1/2)

| Yıl | Doğan kahraman | Ölen kahraman | Emekli olan kahraman | Diyarı terk eden kahraman | Ölümden dönen kahraman | Efsane olan kahraman |
|---|---|---|---|---|---|---|
| 1 | 11,5 (9–13,5) | 0 (0–1,5) | 0 | 0 | 0 | 0 |
| 2 | 11,5 (9–14,5) | 1 (0–3) | 0 | 6 (1,5–8) | 0 | 0 |
| 3 | 13 (11,5–14,5) | 0 (0–0,5) | 0 | 3 (1,5–6) | 0 | 0 |
| 4 | 14 (7,5–16,5) | 2,5 (0–6) | 0 | 3 (2–5,5) | 0 | 0 |
| 5 | 10 (7,5–14) | 3 (0,5–7) | 0 | 4 (2–5) | 0 | 0 (0–0,5) |
| 6 | 11 (7,5–13) | 3,5 (0–7,5) | 0 | 3 (0,5–4,5) | 0 | 0 (0–1) |
| 7 | 10,5 (7–14) | 4,5 (0,5–8,5) | 0 | 3 (1–5) | 0 | 0 |
| 8 | 10,5 (8,5–15) | 4 (0,5–6) | 0 | 3 (0,5–5) | 0 | 0 (0–1) |
| 9 | 11 (7–15) | 4 (1–5,5) | 0 | 2,5 (0,5–6) | 0 | 0 |
| 10 | 14 (10–19) | 2 (0,5–6,5) | 0 | 2 (0,5–3,5) | 0 | 0 |
| 11 | 13 (7–16,5) | 2 (0–7) | 0 | 4 (2–5) | 0 | 0 (0–1) |
| 12 | 12 (10–17) | 3,5 (1–7,5) | 0 | 4 (2–6) | 0 | 0 |
| 13 | 14 (10,5–17,5) | 4,5 (1,5–6,5) | 0 | 3,5 (1–7) | 0 | 0 (0–0,5) |
| 14 | 13 (8,5–17) | 2 (0–6,5) | 0 | 4 (2,5–6) | 0 | 0 |
| 15 | 12,5 (8–16) | 3,5 (1–9,5) | 0 | 4 (2–6) | 0 | 0 |
| 16 | 13 (11–16,5) | 6 (1,5–11) | 0 | 4 (1,5–8,5) | 0 | 0 |
| 17 | 13 (10,5–17) | 4 (1–9,5) | 0 | 5 (2,5–7) | 0 | 0 |
| 18 | 13,5 (8–18) | 4,5 (1–8) | 0 | 4 (2,5–8) | 0 | 0 |
| 19 | 11 (9,5–17) | 3 (0–10) | 0 | 5 (2–7,5) | 0 | 0 |
| 20 | 13,5 (10,5–15,5) | 4 (1,5–9,5) | 0 | 4,5 (2–7,5) | 0 | 0 (0–1) |
| 21 | 14,5 (9,5–16) | 5,5 (1–12) | 0 | 6 (2–9) | 0 | 0 |
| 22 | 13 (9,5–16,5) | 4 (0–9) | 0 | 4,5 (3–8) | 0 | 0 (0–0,5) |
| 23 | 13,5 (8–18,5) | 4 (1,5–7) | 0 | 5 (3–9,5) | 0 | 0 |
| 24 | 13 (10–16) | 3,5 (0,5–9) | 0 | 5 (2,5–10) | 0 | 0 |
| 25 | 14 (10–18) | 3,5 (0–10,5) | 0 (0–0,5) | 5 (3–9,5) | 0 | 0 |
| 26 | 14,5 (9–16) | 4 (0–10) | 0 (0–1) | 6,5 (3,5–11) | 0 | 0 |
| 27 | 13 (10–16) | 3,5 (1,5–9,5) | 0 (0–1) | 6 (2,5–11) | 0 | 0 (0–0,5) |
| 28 | 14 (9,5–19) | 4,5 (0,5–7,5) | 0 (0–1) | 5 (3–9) | 0 | 0 |
| 29 | 12 (7,5–15) | 3 (0–5) | 0 (0–1) | 7 (3,5–9,5) | 0 | 0 |
| 30 | 13,5 (8,5–17,5) | 4,5 (2–10) | 1 (0–1) | 6 (4–9,5) | 0 | 0 (0–1) |
| 31 | 13 (9–15,5) | 4 (0–7) | 0 (0–1) | 6 (4–11) | 0 | 0 |
| 32 | 13,5 (9,5–16,5) | 3 (1–11) | 0,5 (0–2) | 5,5 (2,5–7) | 0 | 0 (0–0,5) |
| 33 | 12 (8,5–17,5) | 5 (0–10) | 1 (0–2) | 6 (2,5–13) | 0 | 0 (0–0,5) |
| 34 | 13 (9,5–15,5) | 4 (2–11) | 0,5 (0–1) | 5,5 (2,5–9) | 0 | 0 |
| 35 | 12,5 (9–16,5) | 4,5 (0,5–8) | 0 (0–3) | 6 (4–11,5) | 0 | 0 |
| 36 | 12 (9–16,5) | 3 (0–11,5) | 0,5 (0–2) | 7 (4–10) | 0 | 0 |
| 37 | 13,5 (9,5–18) | 3 (0–10) | 1 (0–2,5) | 4,5 (2,5–8) | 0 | 0 (0–1) |
| 38 | 13 (10,5–18,5) | 3 (0–10) | 1 (0–2) | 7 (2,5–12) | 0 | 0 |
| 39 | 10,5 (8,5–15,5) | 3 (0,5–9) | 1 (0,5–2) | 6 (2–10) | 0 | 0 |
| 40 | 13 (11–17) | 4 (0–8,5) | 1 (0–1,5) | 4,5 (3–11) | 0 | 0 |
| 41 | 13 (8–16,5) | 6 (2,5–12,5) | 1 (0–3) | 6,5 (2,5–9,5) | 0 | 0 |
| 42 | 13 (8,5–18,5) | 2 (0,5–9,5) | 1,5 (1–3) | 6 (3–8,5) | 0 | 0 |
| 43 | 11,5 (8–15) | 3,5 (1–11) | 1 (0–2,5) | 6,5 (4–9,5) | 0 | 0 |
| 44 | 14 (10–17,5) | 3 (1–7,5) | 2 (0,5–4,5) | 6 (4–9,5) | 0 | 0 |
| 45 | 11 (7,5–15,5) | 3 (1–9,5) | 0 (0–2,5) | 6 (3,5–7,5) | 0 | 0 |
| 46 | 12 (9–18) | 4 (1–7,5) | 1,5 (0–3) | 6 (2–7,5) | 0 | 0 |
| 47 | 11,5 (10,5–16) | 5 (1,5–11) | 1 (0–2,5) | 6,5 (3,5–9,5) | 0 | 0 |
| 48 | 12 (8,5–15) | 2,5 (0–5) | 1 (0–4,5) | 5 (1,5–8) | 0 | 0 (0–0,5) |
| 49 | 12 (9,5–17) | 3,5 (1–10) | 2 (0–3,5) | 6 (3,5–8) | 0 | 0 (0–1) |
| 50 | 13,5 (7,5–17,5) | 4 (1–7,5) | 2 (0–2) | 6,5 (3,5–9,5) | 0 | 0 (0–0,5) |
| 51 | 13 (10–16) | 7 (1–11) | 1 (0–2) | 5,5 (2,5–9) | 0 | 0 |
| 52 | 10,5 (6,5–16) | 3 (0,5–9,5) | 1 (0–3) | 5,5 (3–9) | 0 | 0 |
| 53 | 12,5 (8,5–17) | 3,5 (1,5–7,5) | 1,5 (0,5–3) | 5 (2,5–7,5) | 0 | 0 |
| 54 | 12,5 (9–15,5) | 4 (0,5–7) | 1 (0–2) | 4,5 (2,5–11) | 0 | 0 |
| 55 | 12 (9,5–18) | 2 (1–3,5) | 1 (0–4) | 4,5 (2–10) | 0 | 0 |
| 56 | 12,5 (8–15) | 3,5 (0,5–8) | 1 (0–3,5) | 6 (2–8,5) | 0 | 0 |
| 57 | 11 (7,5–14) | 2,5 (0,5–11) | 1 (0–4) | 5 (2–8,5) | 0 | 0 |
| 58 | 14 (9,5–17,5) | 4 (1–8) | 1 (0–5,5) | 5 (2,5–9,5) | 0 | 0 (0–0,5) |
| 59 | 12 (7–14) | 2,5 (1–8,5) | 2 (1–3) | 5 (2,5–9) | 0 | 0 |
| 60 | 11 (6–15) | 4 (1–8) | 1 (0–3,5) | 6 (2,5–7) | 0 | 0 |

### Kahramanlar (2/2)

| Yıl | Yaşayan kahraman (yıl sonu) | Doğuş seviyesi (ort.) | Ölüm seviyesi (ort.) | Yaşayan kahraman seviyesi (ort.) | En yüksek seviye (şimdiye dek) |
|---|---|---|---|---|---|
| 1 | 11 (7,5–13,5) | 1,33 (1,19–1,52) | 1 (1–1,23) | 1,33 (1,22–1,58) | 2 |
| 2 | 16 (13–18) | 1,32 (1,23–1,44) | 1,33 (1–2) | 1,33 (1,25–1,46) | 2 (2–2,5) |
| 3 | 25,5 (21,5–28) | 1,32 (1,23–1,42) | 1,08 (1,02–1,15) | 1,35 (1,3–1,44) | 2 (2–2,5) |
| 4 | 30,5 (27–36) | 1,31 (1,23–1,54) | 1,33 (1–1,63) | 1,41 (1,32–1,52) | 2 (2–3) |
| 5 | 34,5 (28,5–38) | 1,33 (1,16–1,54) | 1,37 (1–1,91) | 1,52 (1,38–1,6) | 3 (2–3) |
| 6 | 38 (32–45,5) | 1,26 (1,05–1,43) | 1,6 (1,3–2) | 1,6 (1,46–1,74) | 3 (3–4) |
| 7 | 40,5 (33,5–50) | 1,35 (1,2–1,48) | 1,5 (1,04–1,95) | 1,71 (1,61–1,94) | 3,5 (3–4,5) |
| 8 | 45,5 (40,5–53,5) | 1,24 (1,13–1,43) | 1,6 (1–2,47) | 1,77 (1,65–2,02) | 4 (3–5) |
| 9 | 50,5 (46–58) | 1,32 (1,15–1,45) | 2 (1,31–2,73) | 1,92 (1,79–2,17) | 4 (4–5,5) |
| 10 | 60 (52,5–71) | 1,33 (1,2–1,45) | 1,9 (1,5–3) | 2 (1,87–2,21) | 5 (4–5,5) |
| 11 | 66 (57–74,5) | 1,31 (1,19–1,48) | 1,8 (1–2,27) | 2,12 (2,01–2,38) | 5 (4–6) |
| 12 | 71 (64–79,5) | 1,28 (1,15–1,41) | 2 (1,5–2,75) | 2,19 (2,02–2,4) | 5 (4–6) |
| 13 | 77,5 (67,5–85,5) | 1,29 (1,13–1,54) | 2 (1,6–2,29) | 2,28 (2,1–2,52) | 5 (4–7) |
| 14 | 83,5 (71–90,5) | 1,37 (1,13–1,5) | 1,88 (1,07–2,91) | 2,39 (2,21–2,59) | 5 (4,5–7) |
| 15 | 85,5 (71–102) | 1,26 (1,14–1,45) | 2 (1,25–2,85) | 2,5 (2,37–2,66) | 5 (5–7) |
| 16 | 90,5 (77–102) | 1,34 (1,21–1,5) | 2,67 (2,05–3,43) | 2,49 (2,36–2,79) | 6 (5–7,5) |
| 17 | 91 (82–104) | 1,29 (1,2–1,37) | 2,25 (1–3) | 2,59 (2,35–2,89) | 6 (5–7,5) |
| 18 | 91,5 (83–114) | 1,31 (1,21–1,4) | 2,2 (1,85–2,94) | 2,68 (2,4–2,92) | 6 (5–7,5) |
| 19 | 96 (84,5–114) | 1,26 (1,14–1,38) | 2,58 (2,08–3) | 2,7 (2,46–3,09) | 6,5 (5–8) |
| 20 | 102 (88–117) | 1,3 (1,13–1,46) | 2,75 (1–3,55) | 2,77 (2,49–3,16) | 6,5 (6–8) |
| 21 | 104 (88–118) | 1,33 (1,24–1,55) | 2,52 (1,63–3,34) | 2,79 (2,55–3,16) | 7 (6–8) |
| 22 | 108 (90,5–123) | 1,32 (1,13–1,43) | 3,03 (1,05–3,88) | 2,87 (2,57–3,25) | 7 (6–9) |
| 23 | 111 (91–128) | 1,28 (1,13–1,42) | 2,33 (1,63–4,15) | 2,93 (2,6–3,3) | 7,5 (7–9) |
| 24 | 110 (98,5–134) | 1,33 (1,2–1,45) | 2,38 (1,5–3,5) | 2,93 (2,64–3,38) | 8 (7–9) |
| 25 | 115 (100–142) | 1,24 (1,13–1,42) | 2,67 (1,33–3,25) | 3 (2,62–3,43) | 8 (7–9) |
| 26 | 116 (106–142) | 1,27 (1,16–1,4) | 2,67 (1,43–3,72) | 3,1 (2,55–3,43) | 8 (7–10) |
| 27 | 118 (105–142) | 1,3 (1,18–1,41) | 2,67 (2–4) | 3,2 (2,59–3,54) | 8 (7–10) |
| 28 | 120 (107–144) | 1,31 (1,19–1,46) | 3 (1,3–5,01) | 3,18 (2,64–3,52) | 8 (7,5–10) |
| 29 | 120 (107–148) | 1,41 (1,15–1,61) | 2 (1,5–3,3) | 3,24 (2,74–3,61) | 8,5 (7,5–10) |
| 30 | 122 (108–149) | 1,31 (1,18–1,48) | 2,92 (2–4,33) | 3,33 (2,76–3,66) | 9 (7,5–10) |
| 31 | 126 (105–150) | 1,41 (1,21–1,59) | 2,5 (2–4,24) | 3,32 (2,77–3,75) | 9 (8–10) |
| 32 | 128 (110–146) | 1,46 (1,26–1,6) | 3 (2,24–4,07) | 3,43 (2,79–3,74) | 9,5 (8–10) |
| 33 | 130 (110–147) | 1,42 (1,27–1,67) | 3 (2,07–4,77) | 3,47 (2,81–3,78) | 10 (8–10) |
| 34 | 126 (111–150) | 1,34 (1,21–1,54) | 2,88 (1,71–3,67) | 3,47 (2,87–3,82) | 10 (8–10) |
| 35 | 126 (113–149) | 1,49 (1,27–1,69) | 3,17 (1,26–4,45) | 3,52 (2,91–3,87) | 10 (8–10) |
| 36 | 128 (112–156) | 1,39 (1,2–1,61) | 3,28 (2,2–4,35) | 3,57 (2,91–3,87) | 10 (8–10) |
| 37 | 130 (114–160) | 1,47 (1,26–1,76) | 2,67 (2,24–3,93) | 3,57 (2,98–3,88) | 10 (8–10) |
| 38 | 130 (116–161) | 1,53 (1,25–1,75) | 3,45 (2,01–4,23) | 3,55 (2,9–3,93) | 10 (8,5–10) |
| 39 | 129 (114–162) | 1,39 (1,27–1,71) | 2,75 (2–4,93) | 3,59 (2,94–3,88) | 10 (8,5–10) |
| 40 | 133 (113–170) | 1,46 (1,33–1,68) | 3,07 (2,11–4,85) | 3,64 (2,95–3,94) | 10 (8,5–10) |
| 41 | 132 (112–166) | 1,51 (1,29–1,88) | 3 (2,43–3,57) | 3,55 (2,97–4,01) | 10 (8,5–10) |
| 42 | 130 (112–171) | 1,55 (1,4–1,85) | 3 (2–3,88) | 3,53 (3–4,02) | 10 (8,5–10) |
| 43 | 130 (114–164) | 1,57 (1,48–1,76) | 3,6 (1,59–4,81) | 3,63 (3,12–4,13) | 10 (8,5–10) |
| 44 | 132 (114–168) | 1,43 (1,26–1,73) | 3 (2,4–4,53) | 3,51 (3,08–4,22) | 10 (8,5–10) |
| 45 | 136 (112–169) | 1,5 (1,23–1,78) | 3,5 (1,83–4,8) | 3,52 (3,09–4,21) | 10 (8,5–10) |
| 46 | 137 (114–172) | 1,57 (1,29–1,77) | 4 (2,16–5,37) | 3,42 (3,09–4,29) | 10 (9–10) |
| 47 | 138 (110–168) | 1,52 (1,34–1,73) | 3,5 (3–4,63) | 3,44 (3,08–4,38) | 10 (9–10) |
| 48 | 140 (112–172) | 1,59 (1,33–1,76) | 3,1 (1,1–4,35) | 3,47 (3,09–4,39) | 10 (9–10) |
| 49 | 142 (111–176) | 1,47 (1,26–1,84) | 3,3 (2–4,76) | 3,41 (3,08–4,38) | 10 (9–10) |
| 50 | 142 (110–182) | 1,59 (1,23–1,77) | 4 (2,1–5,47) | 3,51 (3,07–4,26) | 10 (9–10) |
| 51 | 142 (110–172) | 1,58 (1,4–1,81) | 3,79 (2,83–7,6) | 3,52 (3,02–4,28) | 10 (9–10) |
| 52 | 146 (114–174) | 1,5 (1,35–1,72) | 3,72 (2,38–5,88) | 3,58 (3,12–4,27) | 10 (9–10) |
| 53 | 146 (115–175) | 1,56 (1,31–1,79) | 3,67 (2,25–4,92) | 3,56 (3,15–4,24) | 10 (9–10) |
| 54 | 148 (118–182) | 1,67 (1,36–1,85) | 3,65 (2,9–4,75) | 3,51 (3,11–4,27) | 10 (9,5–10) |
| 55 | 152 (119–184) | 1,54 (1,31–1,76) | 3,67 (2,5–6,1) | 3,49 (3,07–4,34) | 10 (9,5–10) |
| 56 | 151 (116–184) | 1,54 (1,34–2,05) | 3,9 (2,34–6) | 3,49 (3,01–4,26) | 10 (9,5–10) |
| 57 | 154 (120–187) | 1,59 (1,4–1,84) | 4 (2,15–5,9) | 3,57 (3,03–4,29) | 10 |
| 58 | 152 (119–191) | 1,57 (1,46–1,93) | 4 (3–5,63) | 3,6 (2,96–4,31) | 10 |
| 59 | 152 (120–190) | 1,59 (1,42–1,78) | 3,83 (2,7–7,8) | 3,63 (2,95–4,33) | 10 |
| 60 | 152 (125–192) | 1,55 (1,3–1,84) | 3,5 (2–5,37) | 3,6 (2,98–4,35) | 10 |

### Han ve ticaret

| Yıl | Ayakta han | Asılan ilan | Biten ilan | Ticaret seferi (kervan) | İkmal seferi |
|---|---|---|---|---|---|
| 1 | 4 (3–4) | 2 (0–3,5) | 0 (0–1) | 0 | 0 |
| 2 | 4 | 2 (0–3,5) | 0 (0–1) | 1 (0–2) | 0 |
| 3 | 4 | 6 (2–8) | 0 | 7,5 (3–21) | 0 |
| 4 | 4 (3–4) | 10,5 (6,5–14) | 0,5 (0–2) | 15,5 (9–35,5) | 0 |
| 5 | 3 (3–4) | 10,5 (7,5–13) | 0,5 (0–2) | 21 (18–43) | 0 |
| 6 | 4 (3–4) | 11 (8–16,5) | 1,5 (0–3,5) | 31,5 (24,5–51,5) | 0 (0–2) |
| 7 | 4 (3–4) | 8,5 (6–12) | 1 (0–3) | 38 (29–62,5) | 2,5 (0–6) |
| 8 | 4 (3–4) | 7 (4,5–11) | 2 (0–3) | 48 (31–68,5) | 4,5 (2–16) |
| 9 | 4 (3–5) | 6,5 (4–11) | 2 (0,5–3,5) | 54 (38–78) | 9 (6,5–23,5) |
| 10 | 5 (4–5) | 6 (3–9,5) | 2 (0–3) | 60 (40–84) | 15 (11–29) |
| 11 | 5 (4–6) | 5 (2,5–7) | 2 (0–3,5) | 63,5 (48,5–96,5) | 19 (12,5–31) |
| 12 | 5 (5–6) | 4,5 (1,5–6) | 1 (0–3) | 67,5 (51,5–96) | 20,5 (13–31) |
| 13 | 5 (5–6) | 4 (2–7) | 1 (0,5–2,5) | 70,5 (54–97,5) | 24 (14,5–32,5) |
| 14 | 5 (4,5–6) | 4,5 (1,5–9,5) | 2 (0–4) | 80 (59–98) | 24,5 (15,5–35) |
| 15 | 5 (4,5–5) | 5 (3–7) | 2 (0,5–3,5) | 82 (61–102) | 27 (17,5–35) |
| 16 | 5 (5–6) | 7 (4–9,5) | 1 (0–5,5) | 83,5 (64–112) | 30 (17,5–39) |
| 17 | 5 (5–6) | 8,5 (4,5–10,5) | 1,5 (0–3) | 87,5 (66–123) | 30,5 (19,5–41,5) |
| 18 | 5 (4,5–6) | 7 (4–9) | 0,5 (0–3) | 89 (62,5–116) | 31,5 (23,5–40,5) |
| 19 | 5 (4,5–6) | 6 (4,5–11,5) | 1 (0–3,5) | 89,5 (66–126) | 32 (23–42) |
| 20 | 5 (5–6) | 8 (5–13) | 1 (0–3) | 96 (65,5–136) | 35 (22,5–42) |
| 21 | 5,5 (5–6) | 7 (5–9,5) | 1 (0–3) | 96,5 (61,5–130) | 36,5 (23–44,5) |
| 22 | 5,5 (5–6) | 8 (4,5–10) | 1,5 (0,5–4) | 99 (68–152) | 35,5 (25,5–45,5) |
| 23 | 5,5 (5–6) | 8 (4–10,5) | 1,5 (0–3) | 100 (68–150) | 38 (25–47,5) |
| 24 | 5 (5–6) | 7 (4–9,5) | 1 (0–2) | 104 (68–155) | 38,5 (25–51,5) |
| 25 | 6 (5–6) | 8 (4–10) | 1 (0–4) | 106 (70,5–150) | 39,5 (25–52) |
| 26 | 6 (5–6) | 8,5 (4,5–12,5) | 1 (0–4) | 107 (73,5–148) | 40,5 (25,5–53) |
| 27 | 6 (5–6,5) | 8 (3–11) | 1 (0–3) | 110 (74–145) | 39 (25–55,5) |
| 28 | 6 (5–6,5) | 7 (2,5–12) | 0 (0–2,5) | 106 (74,5–169) | 41 (23,5–54,5) |
| 29 | 5,5 (5–6,5) | 7,5 (2–14) | 1 (0–3) | 111 (78,5–175) | 42,5 (27,5–52,5) |
| 30 | 6 (5–6,5) | 9 (0,5–13,5) | 1 (0–3) | 114 (77,5–169) | 40 (27,5–52) |
| 31 | 6 (5–6,5) | 8 (2,5–11,5) | 1 (0–2) | 117 (78,5–179) | 43 (27–52) |
| 32 | 6 (5–6) | 9 (1–13) | 1 (0–3) | 118 (80,5–174) | 42 (28–51,5) |
| 33 | 6 (5–6) | 8 (1,5–10,5) | 0,5 (0–3,5) | 118 (78,5–164) | 42 (27,5–51,5) |
| 34 | 6 (5–6) | 9 (2,5–14) | 1 (0–2,5) | 118 (79,5–172) | 41 (28–52) |
| 35 | 6 (5–6) | 8,5 (0,5–13) | 1,5 (0–3,5) | 119 (83–170) | 40 (27,5–51,5) |
| 36 | 6 (5–6) | 8,5 (1–12) | 1 (0–2) | 116 (80,5–194) | 40 (27–51) |
| 37 | 6 (5–6) | 9,5 (1,5–12) | 1 (0–5) | 111 (85–182) | 41,5 (27–57) |
| 38 | 6 (5–6) | 7,5 (3–15) | 1 (0–4) | 118 (86,5–180) | 43,5 (27,5–57) |
| 39 | 6 (5–6) | 7,5 (3,5–14) | 1,5 (0–4) | 110 (87–188) | 45 (27–54,5) |
| 40 | 6 (5–6) | 9 (4–15,5) | 1 (0,5–5,5) | 110 (85–206) | 43 (29–51,5) |
| 41 | 6 (5–6) | 7,5 (1,5–14,5) | 1,5 (0–4,5) | 116 (89–212) | 45,5 (28–53) |
| 42 | 6 (5–6) | 7 (3–14) | 2 (0–5) | 121 (86–210) | 47,5 (26,5–55) |
| 43 | 6 (5–6) | 9,5 (3,5–16,5) | 2 (0–4) | 124 (83–203) | 47,5 (27,5–64) |
| 44 | 6 (5–6) | 6 (4–14,5) | 1 (0–5,5) | 124 (84–210) | 47,5 (29–66) |
| 45 | 6 (5–6) | 7,5 (2,5–13,5) | 1 (0–5) | 125 (83,5–216) | 47,5 (29–65,5) |
| 46 | 6 (5–6) | 8 (3,5–13) | 1 (0–4) | 120 (86–204) | 47 (29,5–69) |
| 47 | 6 (5–6) | 8 (3,5–14,5) | 1 (0–4,5) | 134 (88–204) | 48 (29–70) |
| 48 | 6 (5–6) | 9 (3,5–14,5) | 1,5 (0–7) | 135 (87,5–210) | 50 (29,5–69,5) |
| 49 | 6 (5–6) | 9 (2,5–14,5) | 1,5 (0–5,5) | 137 (88–230) | 48,5 (29,5–71) |
| 50 | 6 (5–6) | 9,5 (3–13,5) | 2 (0–5) | 140 (89–235) | 47,5 (31–72,5) |
| 51 | 6 (5–6) | 8 (3,5–14) | 2 (0–5,5) | 136 (91–234) | 50 (31,5–71,5) |
| 52 | 6 (5–6) | 7,5 (2,5–14,5) | 2 (0–7,5) | 142 (92,5–235) | 50,5 (30–72) |
| 53 | 6 (5–6) | 8,5 (3,5–14) | 2 (0–7) | 148 (85,5–234) | 50,5 (34,5–72,5) |
| 54 | 6 (5–6) | 8,5 (3,5–16) | 3 (0–7) | 153 (88,5–236) | 50,5 (35,5–72,5) |
| 55 | 6 (5–6) | 7 (3,5–15,5) | 1,5 (0–8) | 156 (82,5–246) | 51 (34,5–71) |
| 56 | 6 (5–6) | 9,5 (4–18) | 0,5 (0–7) | 150 (83,5–238) | 51,5 (29,5–74,5) |
| 57 | 6 (5–6) | 9,5 (4,5–17) | 2,5 (0–5) | 150 (85,5–240) | 51 (31,5–75,5) |
| 58 | 6 (5–6) | 9,5 (2,5–16) | 2,5 (0–7,5) | 142 (84–261) | 51 (31–75) |
| 59 | 6 (5–6) | 9,5 (4–17) | 1 (0–7) | 145 (88,5–262) | 52 (32–75) |
| 60 | 6 (5–6) | 9 (3–16) | 1,5 (0–4,5) | 147 (83,5–271) | 53 (32–76) |

### Altın ve ambar (1/3)

| Yıl | Altın p90 (medeniyetler) | Bakım gideri (altın; asker, kahraman, L2–L3) | Kamu işlerine (imar) harcanan altın | Ambarla beslenen amele tayını (gıda) | Kamu işlerindeki (amele) iş gücü payı | İmar ortalaması (köy+, 0–100) |
|---|---|---|---|---|---|---|
| 1 | 38,9 (21,3–62,1) | 0 | 0 | 0 | %0 | – |
| 2 | 73,2 (42,6–96,1) | 4,2 (2,18–10,2) | 0 | 0 | %0 | 0 |
| 3 | 81,1 (53,6–103) | 38,5 (17,3–65,3) | 0 | 0 | %0 | 0 |
| 4 | 86,1 (64,4–131) | 87 (54,6–119) | 0 | 0 (0–1,8) | %0 (%0–%0,1) | 0 (0–0,01) |
| 5 | 102 (66,9–121) | 171 (116–221) | 0 | 1,6 (0–25,4) | %0 (%0–%0,5) | 0 (0–0,07) |
| 6 | 123 (82–188) | 301 (223–360) | 0 (0–1,75) | 34,4 (5–110) | %0,5 (%0,1–%1,2) | 0,12 (0,01–0,23) |
| 7 | 164 (75,9–258) | 416 (341–522) | 5,09 (0–36,9) | 176 (37–403) | %1,7 (%0,4–%3,7) | 0,45 (0,1–0,95) |
| 8 | 205 (128–324) | 621 (494–744) | 16,5 (0–128) | 361 (98,8–787) | %3,1 (%1–%5,6) | 1,09 (0,36–2,09) |
| 9 | 306 (198–461) | 827 (647–938) | 112 (7,54–260) | 659 (321–1470) | %5,6 (%2,9–%8,8) | 2,86 (1,01–4,33) |
| 10 | 367 (267–513) | 1024 (789–1175) | 307 (105–558) | 1288 (727–1782) | %8,6 (%5,8–%11) | 5,22 (2,52–7,8) |
| 11 | 424 (315–508) | 1195 (925–1434) | 439 (242–675) | 1733 (1138–2328) | %11 (%9,4–%13) | 7,54 (5,19–10,6) |
| 12 | 446 (358–559) | 1355 (1116–1618) | 521 (269–749) | 2196 (1641–2678) | %13 (%12–%15) | 9,61 (6,66–11,1) |
| 13 | 548 (394–639) | 1516 (1201–1787) | 651 (317–906) | 2381 (2028–3228) | %14 (%13–%16) | 11,4 (8,55–12,9) |
| 14 | 611 (453–785) | 1664 (1295–1938) | 744 (462–1220) | 2828 (2054–3543) | %16 (%14–%18) | 13,1 (10,2–15,6) |
| 15 | 714 (473–828) | 1797 (1423–2048) | 944 (388–1494) | 3427 (2378–3789) | %18 (%15–%19) | 14,9 (10,2–17,7) |
| 16 | 626 (454–753) | 1997 (1497–2253) | 956 (351–1382) | 3748 (2566–4571) | %18 (%14–%20) | 16,5 (9,52–18,9) |
| 17 | 660 (526–846) | 2109 (1644–2385) | 1115 (603–1389) | 3903 (2981–4898) | %19 (%16–%21) | 16,9 (10,1–20,5) |
| 18 | 752 (575–876) | 2233 (1678–2454) | 1236 (673–1898) | 4004 (2864–5714) | %19 (%16–%21) | 18,7 (11,4–21,8) |
| 19 | 838 (616–971) | 2341 (1863–2597) | 1235 (776–1782) | 4351 (3249–5882) | %20 (%17–%22) | 19,5 (13,3–22) |
| 20 | 768 (574–987) | 2340 (1986–2703) | 1401 (726–1937) | 4535 (3267–5801) | %20 (%17–%22) | 19,8 (15,6–23) |
| 21 | 882 (726–1116) | 2359 (2072–2822) | 1618 (792–2310) | 4309 (2472–6314) | %19 (%18–%23) | 20,6 (15,9–23,6) |
| 22 | 943 (606–1087) | 2459 (2160–2949) | 1832 (704–2365) | 4534 (2595–6266) | %20 (%16–%23) | 21,9 (15,9–25,2) |
| 23 | 997 (648–1183) | 2522 (2152–2997) | 1937 (943–2607) | 4826 (2826–6131) | %20 (%18–%23) | 22,9 (16,4–28,2) |
| 24 | 995 (565–1236) | 2660 (2323–3064) | 2027 (885–2680) | 4603 (3261–5905) | %19 (%17–%23) | 23,6 (16,4–28,3) |
| 25 | 1014 (702–1141) | 2669 (2312–3139) | 2130 (897–2624) | 4856 (4026–6427) | %19 (%18–%23) | 23,2 (17,8–28,8) |
| 26 | 1019 (562–1218) | 2795 (2329–3335) | 2004 (1125–2626) | 4594 (3691–6004) | %20 (%17–%23) | 21,9 (17,6–28,9) |
| 27 | 961 (750–1225) | 2696 (2332–3204) | 2099 (979–2905) | 4331 (2730–6106) | %21 (%18–%22) | 22 (17,4–29,2) |
| 28 | 1023 (757–1292) | 2832 (2324–3295) | 2327 (1322–2975) | 3761 (1980–5759) | %20 (%18–%22) | 22,9 (17,8–27,9) |
| 29 | 968 (764–1183) | 2842 (2330–3326) | 2383 (1292–3195) | 3664 (1512–5361) | %20 (%17–%22) | 23,1 (17,7–28) |
| 30 | 1024 (902–1283) | 2887 (2395–3464) | 2562 (1597–3095) | 3497 (1476–5920) | %20 (%18–%21) | 23,7 (17,1–27,7) |
| 31 | 1029 (910–1309) | 2912 (2412–3438) | 2801 (1815–3199) | 3166 (1204–6033) | %19 (%18–%22) | 24,3 (17,3–28,1) |
| 32 | 1016 (888–1254) | 3001 (2475–3520) | 2746 (1803–3178) | 2599 (977–6242) | %19 (%18–%21) | 23,8 (17,3–28,2) |
| 33 | 1047 (780–1314) | 2912 (2541–3601) | 2648 (1992–3221) | 2269 (1403–4920) | %19 (%18–%22) | 24,2 (17,4–29,2) |
| 34 | 1155 (857–1434) | 3070 (2459–3600) | 2643 (2084–3320) | 2904 (1348–4387) | %19 (%16–%21) | 24 (17,9–29,2) |
| 35 | 1139 (849–1434) | 3128 (2487–3585) | 2706 (2097–3408) | 2546 (1461–4775) | %19 (%17–%22) | 23,7 (19,1–29,6) |
| 36 | 1198 (927–1348) | 3116 (2473–3668) | 2744 (1766–3566) | 2744 (1012–4767) | %19 (%16–%22) | 23,5 (19,1–29,2) |
| 37 | 1089 (943–1428) | 3107 (2410–3807) | 2854 (2028–3190) | 2627 (1082–4330) | %18 (%16–%22) | 23,5 (17,6–28,1) |
| 38 | 1095 (842–1351) | 3162 (2449–3852) | 2780 (2038–3285) | 2359 (1026–3294) | %18 (%16–%22) | 23,3 (17,7–27,7) |
| 39 | 1130 (880–1441) | 3158 (2483–3775) | 2875 (2379–3552) | 2548 (1118–3988) | %19 (%17–%22) | 23,8 (18,4–27,6) |
| 40 | 1128 (948–1406) | 3186 (2472–3810) | 3018 (2105–3819) | 2154 (881–3936) | %19 (%17–%22) | 24,2 (19–27,5) |
| 41 | 1081 (954–1279) | 3206 (2490–3853) | 2917 (2226–3634) | 2407 (669–3592) | %19 (%17–%22) | 23,7 (18,7–27,8) |
| 42 | 1209 (971–1368) | 3208 (2520–3967) | 3106 (2333–3531) | 2311 (610–5173) | %19 (%17–%22) | 23,2 (18,7–27,4) |
| 43 | 1190 (1045–1452) | 3172 (2690–3811) | 3078 (2327–3855) | 2359 (814–4679) | %18 (%16–%22) | 22,4 (19,3–27,3) |
| 44 | 1237 (917–1488) | 3228 (2586–3927) | 3169 (2195–4047) | 2773 (845–4870) | %18 (%16–%22) | 22,4 (19,4–27) |
| 45 | 1251 (1058–1450) | 3167 (2637–4015) | 3226 (2396–4198) | 2432 (685–4805) | %18 (%16–%22) | 22,4 (19–27,3) |
| 46 | 1240 (973–1462) | 3234 (2544–4085) | 3364 (2486–4203) | 2561 (397–4233) | %19 (%16–%22) | 22,3 (19–27,1) |
| 47 | 1187 (987–1340) | 3187 (2488–3992) | 3219 (2242–4109) | 3294 (641–3785) | %19 (%16–%22) | 22,1 (18,5–27,2) |
| 48 | 1129 (938–1366) | 3247 (2552–4115) | 2942 (2406–4171) | 2368 (854–3976) | %19 (%15–%23) | 21,6 (18–26,8) |
| 49 | 1283 (1085–1752) | 3274 (2606–4136) | 3086 (2500–4565) | 2038 (544–3400) | %19 (%14–%21) | 21,4 (17,3–28,8) |
| 50 | 1236 (968–1698) | 3159 (2667–4259) | 3430 (2618–4713) | 1688 (875–3827) | %18 (%15–%21) | 20,9 (16,1–31,1) |
| 51 | 1287 (1105–1491) | 3203 (2617–4132) | 3257 (2782–4342) | 1692 (840–3923) | %17 (%15–%21) | 21,5 (16,1–31,2) |
| 52 | 1264 (1097–1534) | 3291 (2749–4202) | 3126 (2772–4219) | 1657 (647–4425) | %17 (%14–%22) | 21 (15,3–30,8) |
| 53 | 1225 (1013–1479) | 3269 (2696–4138) | 3348 (2834–4251) | 1457 (531–4662) | %18 (%16–%21) | 20,8 (15,3–30,7) |
| 54 | 1256 (1153–1366) | 3329 (2785–4209) | 3416 (2675–4081) | 1543 (296–3754) | %18 (%15–%22) | 21,1 (15,3–30,1) |
| 55 | 1261 (1091–1501) | 3256 (2789–4354) | 3528 (2557–4242) | 1518 (444–2977) | %18 (%15–%21) | 21,6 (15,8–29,6) |
| 56 | 1277 (993–1494) | 3325 (2640–4233) | 3538 (2504–4560) | 1936 (440–3953) | %17 (%15–%22) | 22 (16,3–29,9) |
| 57 | 1315 (981–1586) | 3350 (2593–4266) | 3533 (2809–4601) | 1689 (259–3992) | %17 (%15–%22) | 23,3 (16,5–30,8) |
| 58 | 1305 (1041–1578) | 3400 (2705–4153) | 3535 (2616–4730) | 1804 (556–3105) | %17 (%14–%22) | 23,3 (16,3–30) |
| 59 | 1324 (974–1514) | 3515 (2565–4197) | 3560 (2478–4774) | 1520 (323–3116) | %18 (%13–%22) | 23,2 (16,4–28,8) |
| 60 | 1360 (943–1572) | 3529 (2597–4169) | 3604 (2495–4733) | 1350 (294–2706) | %17 (%15–%22) | 22,1 (16,2–28) |

### Altın ve ambar (2/3)

| Yıl | Canavar baskınında yitirilen altın | Ejderhaya giden altın (haraç + akın) | Hazinesi boş medeniyet payı | Kent tüketiminde yokluk payı (köy+; ekmek, bira ya da alet) | Ekmek ya da bira yokluğu payı (köy+) | Kıtlık (büyük olay) |
|---|---|---|---|---|---|---|
| 1 | 0 | 0 | %0 | – | – | 0 |
| 2 | 0 | 0 | %0 | %3 (%1,3–%4,6) | %3 (%1,3–%4,6) | 0 |
| 3 | 11,5 (0–35,5) | 0 | %0 | %1,6 (%1–%6,3) | %1,6 (%1–%6,3) | 0 |
| 4 | 16,5 (0–46,5) | 0 | %0 | %0,7 (%0–%5,4) | %0,7 (%0–%5,4) | 0 |
| 5 | 20 (3–38) | 0 | %0 | %0,1 (%0–%3,2) | %0 (%0–%3,1) | 0 |
| 6 | 7,5 (0–36,5) | 0 | %0 | %0 (%0–%0,8) | %0 (%0–%0,3) | 0 |
| 7 | 6 (0,5–29,5) | 0 | %0 | %0,1 (%0–%1,2) | %0 (%0–%0,2) | 0 |
| 8 | 7 (0–17) | 0 | %0 | %0,2 (%0–%1,8) | %0 (%0–%1,1) | 0 |
| 9 | 4 (0–21,5) | 0 | %0 | %0,5 (%0–%2) | %0,2 (%0–%1,4) | 0 |
| 10 | 3 (0–20,5) | 0 | %0 | %1 (%0–%5,4) | %0,1 (%0–%5,4) | 0 |
| 11 | 0,5 (0–41,5) | 0 | %0 | %1,1 (%0–%5,8) | %0 (%0–%5,8) | 0 |
| 12 | 6,5 (0–30) | 0 | %0 | %0,3 (%0–%6,4) | %0 (%0–%6,4) | 0 |
| 13 | 1 (0–33) | 0 | %0 | %0,4 (%0–%7,8) | %0 (%0–%7,8) | 0 |
| 14 | 0 (0–29,5) | 0 (0–90,5) | %0 | %1,1 (%0–%6) | %0,2 (%0–%6) | 0 |
| 15 | 0 (0–35) | 0 (0–97,5) | %0 | %0,8 (%0–%6) | %0,6 (%0–%5,7) | 0 |
| 16 | 1 (0–25,5) | 118 (0–259) | %0 | %1,1 (%0–%5,5) | %0,7 (%0–%4,5) | 0 |
| 17 | 0 (0–7,5) | 152 (43–454) | %0 | %1,9 (%0–%4,7) | %1,3 (%0–%3,4) | 0 |
| 18 | 0,5 (0–16) | 226 (84,5–326) | %0 | %3,1 (%0,1–%7,7) | %1,2 (%0–%4,4) | 0 |
| 19 | 5,5 (0–34) | 180 (44–411) | %0 | %2 (%0–%12) | %0,9 (%0–%5,1) | 0 |
| 20 | 0 (0–34) | 273 (10,5–566) | %0 | %2,3 (%0,1–%12) | %0,3 (%0–%3,9) | 0 |
| 21 | 0 (0–23,5) | 263 (2–511) | %0 | %5,5 (%0,1–%25) | %1,1 (%0–%6,1) | 0 (0–0,5) |
| 22 | 0 (0–6,5) | 330 (0–848) | %0 | %7,2 (%0,1–%34) | %0,7 (%0–%4,6) | 0 |
| 23 | 1 (0–26,5) | 354 (0–1007) | %0 | %10 (%0–%35) | %1,4 (%0–%4,5) | 0 (0–0,5) |
| 24 | 2 (0–25,5) | 354 (0–640) | %0 | %6,9 (%0,5–%32) | %1,4 (%0–%4,3) | 0 |
| 25 | 0 (0–18,5) | 416 (0–800) | %0 | %13 (%0–%29) | %1,1 (%0–%5,3) | 0 |
| 26 | 3,5 (0–50) | 522 (0–1265) | %0 | %17 (%2,7–%30) | %2 (%0–%7,5) | 0 |
| 27 | 5 (0–37,5) | 393 (0–707) | %0 | %25 (%2,6–%40) | %2,3 (%0–%7,6) | 0 |
| 28 | 0 (0–18) | 394 (0–1070) | %0 | %34 (%13–%43) | %2,3 (%0–%7,1) | 0 |
| 29 | 3,5 (0–37) | 620 (0–1278) | %0 | %35 (%14–%43) | %3 (%0–%8,3) | 0 |
| 30 | 0,5 (0–22,5) | 375 (0–797) | %0 | %40 (%19–%45) | %3,3 (%0,2–%8,5) | 0 |
| 31 | 1,5 (0–15,5) | 444 (0–908) | %0 | %39 (%23–%46) | %3 (%1,1–%9,1) | 0 |
| 32 | 7,5 (0–25,5) | 648 (0–1338) | %0 | %38 (%31–%47) | %4,8 (%0,8–%8,9) | 0 |
| 33 | 0 (0–38,5) | 352 (0–833) | %0 | %40 (%32–%48) | %4,6 (%1,1–%8,3) | 0 |
| 34 | 4 (0–23,5) | 314 (0–730) | %0 | %41 (%32–%48) | %5,5 (%1,2–%9,2) | 0 |
| 35 | 6 (0–40) | 436 (0–1798) | %0 | %44 (%30–%52) | %6,5 (%1–%8,3) | 0 |
| 36 | 2,5 (0–38) | 441 (0–799) | %0 | %41 (%32–%50) | %5,8 (%0,9–%9,2) | 0 |
| 37 | 0,5 (0–28,5) | 402 (0–939) | %0 | %43 (%34–%52) | %5,7 (%2,1–%9,4) | 0 |
| 38 | 5,5 (0–54,5) | 554 (0–1468) | %0 | %44 (%35–%52) | %4,9 (%1–%9,2) | 0 |
| 39 | 0 (0–12,5) | 378 (0–902) | %0 | %40 (%33–%54) | %3,5 (%0,7–%9,2) | 0 |
| 40 | 9 (0–71) | 370 (0–1042) | %0 | %45 (%35–%55) | %4,8 (%0,9–%10) | 0 |
| 41 | 0 (0–9,5) | 536 (0–1262) | %0 | %43 (%36–%55) | %4,4 (%1,1–%9,8) | 0 |
| 42 | 0 (0–8) | 358 (0–909) | %0 | %40 (%32–%57) | %5,8 (%2,3–%9,9) | 0 |
| 43 | 2 (0–22) | 386 (0–938) | %0 | %40 (%32–%56) | %5,2 (%1,1–%8,9) | 0 |
| 44 | 0 (0–14) | 581 (0–1164) | %0 | %43 (%30–%57) | %4,1 (%1,5–%9,3) | 0 |
| 45 | 3 (0–16) | 438 (0–838) | %0 | %42 (%30–%56) | %5,3 (%1,5–%10) | 0 |
| 46 | 0 (0–31,5) | 400 (0–830) | %0 | %43 (%37–%56) | %6,6 (%1,6–%12) | 0 |
| 47 | 0,5 (0–22) | 420 (0–1358) | %0 | %44 (%37–%54) | %6,5 (%2–%15) | 0 |
| 48 | 0 (0–30) | 418 (0–803) | %0 | %46 (%35–%54) | %6,4 (%2,6–%11) | 0 |
| 49 | 4 (0–53) | 438 (0–876) | %0 | %43 (%34–%54) | %6 (%2,8–%11) | 0 |
| 50 | 0 (0–28,5) | 548 (0–1536) | %0 | %45 (%31–%55) | %4,9 (%2,5–%11) | 0 |
| 51 | 0 (0–30) | 530 (0–991) | %0 | %44 (%36–%54) | %7,4 (%1,7–%13) | 0 |
| 52 | 0 (0–7) | 496 (0–918) | %0 | %48 (%31–%56) | %6,5 (%2,2–%13) | 0 |
| 53 | 1,5 (0–16,5) | 426 (0–1569) | %0 | %49 (%34–%55) | %7,3 (%2,8–%13) | 0 |
| 54 | 0 (0–26,5) | 472 (0–938) | %0 | %50 (%33–%54) | %7,2 (%2,3–%13) | 0 |
| 55 | 0 (0–26) | 364 (0–1092) | %0 | %48 (%36–%54) | %8,1 (%3,7–%13) | 0 |
| 56 | 6 (0–34,5) | 582 (0–1534) | %0 | %47 (%37–%57) | %7,3 (%2,6–%15) | 0 |
| 57 | 4,5 (0–27,5) | 442 (0–928) | %0 | %45 (%37–%58) | %7,6 (%2,7–%13) | 0 |
| 58 | 4,5 (0–45,5) | 500 (0–1162) | %0 | %48 (%37–%59) | %7,8 (%2,9–%13) | 0 |
| 59 | 5 (0–35,5) | 602 (0–2189) | %0 | %48 (%34–%60) | %9,2 (%2,9–%13) | 0 |
| 60 | 0,5 (0–11,5) | 644 (0–1164) | %0 | %47 (%35–%61) | %11 (%3,6–%14) | 0 |

### Altın ve ambar (3/3)

| Yıl | Açlıktan ölen | Kıtlık yardımı (sevkiyat) | Kıtlıkta yüz çeviren | Kıtlık akını | Ambarın yettiği gün (medeniyet medyanı) |
|---|---|---|---|---|---|
| 1 | 0 | 0 | 0 | 0 | 11,3 (6,27–13) |
| 2 | 0 | 0 | 0 | 0 | 11,4 (9,37–18) |
| 3 | 0 | 0 | 0 | 0 | 11,3 (9,43–15,6) |
| 4 | 0 | 0 | 0 | 0 | 16,6 (11,9–20,6) |
| 5 | 0 | 0 | 0 | 0 | 20,3 (15,8–24,8) |
| 6 | 0 | 0 | 0 | 0 | 24,8 (20,6–31,8) |
| 7 | 0 | 0 | 0 | 0 | 29,7 (20,4–36,3) |
| 8 | 0 | 0 | 0 | 0 | 36,2 (28,5–42,1) |
| 9 | 0 | 0 | 0 | 0 | 41,9 (34,1–49,6) |
| 10 | 0 | 0 | 0 | 0 | 46,2 (39,5–57,9) |
| 11 | 0 | 0 | 0 | 0 | 50 (39,2–64,8) |
| 12 | 0 | 0 | 0 | 0 | 57,4 (44,9–67,5) |
| 13 | 0 | 0 | 0 | 0 | 59,1 (49,6–69,8) |
| 14 | 0 | 0 | 0 | 0 | 61,9 (51,2–72,1) |
| 15 | 0 | 0 | 0 | 0 | 72,4 (55,5–75,4) |
| 16 | 0 | 0 | 0 | 0 | 68,9 (57,6–76,5) |
| 17 | 0 | 0 | 0 | 0 | 71,9 (60,2–79,4) |
| 18 | 0 | 0 | 0 | 0 | 70 (60,3–77,8) |
| 19 | 0 | 0 | 0 | 0 | 71,6 (59,1–77,2) |
| 20 | 0 | 0 | 0 | 0 | 69,5 (61,9–75,2) |
| 21 | 0 | 0 (0–1,5) | 0 | 0 (0–0,5) | 66,8 (58,8–76,2) |
| 22 | 0 | 0 | 0 | 0 | 65,4 (60,3–78,7) |
| 23 | 0 | 0 (0–0,5) | 0 | 0 | 67,9 (60,2–80,6) |
| 24 | 0 | 0 | 0 | 0 (0–0,5) | 65,6 (56,9–80,9) |
| 25 | 0 | 0 | 0 | 0 | 66 (56,1–80,2) |
| 26 | 0 | 0 | 0 | 0 | 64,5 (51,5–76,4) |
| 27 | 0 | 0 | 0 | 0 | 57,3 (49,1–68,7) |
| 28 | 0 | 0 | 0 | 0 | 53 (47,7–61,6) |
| 29 | 0 | 0 | 0 | 0 | 53 (45,8–73,5) |
| 30 | 0 | 0 | 0 | 0 | 54,9 (45,2–70,2) |
| 31 | 0 | 0 | 0 | 0 | 51 (46,4–61,8) |
| 32 | 0 | 0 | 0 | 0 | 50,4 (42–62,3) |
| 33 | 0 | 0 | 0 | 0 | 50 (43,9–64,1) |
| 34 | 0 | 0 | 0 | 0 | 51,3 (42,8–61,2) |
| 35 | 0 | 0 | 0 | 0 | 49,7 (43,9–60,9) |
| 36 | 0 | 0 | 0 | 0 | 47,7 (41,3–59,7) |
| 37 | 0 | 0 | 0 | 0 | 43,7 (39,1–54,8) |
| 38 | 0 | 0 | 0 | 0 | 45 (39,4–53,2) |
| 39 | 0 | 0 | 0 | 0 | 47,9 (41,9–54,6) |
| 40 | 0 | 0 | 0 | 0 | 46,5 (42,6–63,2) |
| 41 | 0 | 0 | 0 | 0 | 49,8 (43,7–60,6) |
| 42 | 0 | 0 | 0 | 0 | 45,1 (39,3–55,8) |
| 43 | 0 | 0 | 0 | 0 | 49,5 (38–52,5) |
| 44 | 0 | 0 | 0 | 0 | 48,7 (35,6–56,7) |
| 45 | 0 | 0 | 0 | 0 | 45,3 (37,2–55,2) |
| 46 | 0 | 0 | 0 | 0 | 45,1 (37,4–56,4) |
| 47 | 0 | 0 | 0 | 0 | 43 (35,5–56,4) |
| 48 | 0 | 0 | 0 | 0 | 45,7 (35–56,2) |
| 49 | 0 | 0 | 0 | 0 | 44,3 (33,7–52,2) |
| 50 | 0 | 0 | 0 | 0 | 45,1 (30,7–53,8) |
| 51 | 0 | 0 | 0 | 0 | 40 (32,4–53,1) |
| 52 | 0 | 0 | 0 | 0 | 41,4 (34,6–56,4) |
| 53 | 0 | 0 | 0 | 0 | 41 (31,5–56,6) |
| 54 | 0 | 0 | 0 | 0 | 43,5 (32,4–49,3) |
| 55 | 0 | 0 | 0 | 0 | 40,9 (29,7–50,1) |
| 56 | 0 | 0 | 0 | 0 | 44,3 (30,3–55,2) |
| 57 | 0 | 0 | 0 | 0 | 40,8 (30,7–55,4) |
| 58 | 0 | 0 | 0 | 0 | 42,9 (29,8–56,3) |
| 59 | 0 | 0 | 0 | 0 | 41,8 (28,2–52,5) |
| 60 | 0 | 0 | 0 | 0 | 37,5 (27,4–54,8) |

### Yerleşim kademesi

| Yıl | Ortalama yerleşim kademesi | Köy+ yerleşim | Kasaba+ yerleşim | Şehir | Ortalama başkent kademesi | Kademe değişimi (yerleşim, yıl içinde) |
|---|---|---|---|---|---|---|
| 1 | 0 | 0 | 0 | 0 | 0 | 0 |
| 2 | 0,61 (0,44–0,73) | 6 (4–7) | 0 | 0 | 0,71 (0,51–0,87) | 6 (4–7) |
| 3 | 0,53 (0,47–0,63) | 8 (6–9) | 0 | 0 | 0,94 (0,82–1) | 3 (2–5,5) |
| 4 | 0,53 (0,47–0,57) | 10,5 (8–12,5) | 0 | 0 | 1 (0,86–1) | 3 (1,5–5) |
| 5 | 0,62 (0,52–0,68) | 15 (12–18) | 0,5 (0–1) | 0 | 1 (0,94–1,13) | 6 (4–8) |
| 6 | 0,71 (0,65–0,79) | 20 (16–23,5) | 2 (0,5–4) | 0 | 1,22 (1–1,44) | 7 (4–10,5) |
| 7 | 0,84 (0,75–0,92) | 25 (20–29) | 5 (1,5–6,5) | 0 | 1,38 (1,2–1,56) | 8 (5,5–12,5) |
| 8 | 0,92 (0,84–0,97) | 29 (24–31,5) | 7 (4,5–9) | 0 | 1,6 (1,43–1,73) | 7 (3,5–10,5) |
| 9 | 0,94 (0,88–1) | 31,5 (25,5–34) | 8,5 (6,5–12,5) | 0 | 1,75 (1,6–1,88) | 8 (4–10) |
| 10 | 0,95 (0,92–1) | 32,5 (27–38,5) | 11 (8–14,5) | 0 (0–0,5) | 1,86 (1,69–1,94) | 4,5 (2,5–8) |
| 11 | 1,02 (0,97–1,05) | 35,5 (29–40) | 13 (9,5–16) | 0 (0–1) | 1,88 (1,78–2) | 5 (2,5–6,5) |
| 12 | 1,08 (1,03–1,11) | 39 (31–43) | 15 (11,5–18) | 0,5 (0–1) | 2 (1,82–2,06) | 5,5 (3–9,5) |
| 13 | 1,1 (1,05–1,19) | 41 (32–45,5) | 17,5 (13–19,5) | 1 (0–1) | 2 (1,87–2,13) | 5 (2,5–10) |
| 14 | 1,12 (1,02–1,22) | 42,5 (34–47,5) | 17 (14–21) | 1,5 (0–2) | 2,06 (1,88–2,24) | 6 (2–9,5) |
| 15 | 1,13 (1,04–1,23) | 44,5 (35,5–50,5) | 19,5 (13–22) | 2 (0–3) | 2,12 (1,94–2,31) | 6,5 (2–13) |
| 16 | 1,17 (1,04–1,24) | 48 (37–54) | 21 (13,5–24,5) | 2,5 (1–3,5) | 2,24 (2,06–2,4) | 5 (3–9,5) |
| 17 | 1,2 (1,09–1,27) | 49 (38–56) | 21,5 (15–26,5) | 2,5 (1–4,5) | 2,27 (2,12–2,49) | 5,5 (1,5–12) |
| 18 | 1,19 (1,13–1,25) | 49,5 (37,5–57) | 23 (16–26,5) | 3 (1–4,5) | 2,38 (2,13–2,44) | 5 (1–8,5) |
| 19 | 1,2 (1,12–1,25) | 50,5 (39–58) | 22,5 (17,5–26) | 3 (1–5) | 2,38 (2,13–2,59) | 5,5 (1,5–8,5) |
| 20 | 1,21 (1,14–1,25) | 52 (40–58) | 23,5 (17,5–27) | 4 (1–6) | 2,43 (2,13–2,65) | 6,5 (2,5–8) |
| 21 | 1,22 (1,15–1,27) | 52,5 (42,5–59,5) | 24 (18,5–27,5) | 4 (1,5–6) | 2,47 (2,21–2,6) | 6 (3,5–12,5) |
| 22 | 1,2 (1,15–1,28) | 53 (43–60) | 25 (18,5–27,5) | 4,5 (3–6) | 2,5 (2,31–2,6) | 3,5 (0,5–10) |
| 23 | 1,22 (1,14–1,29) | 53,5 (43–62) | 24,5 (18,5–29) | 5 (4–5) | 2,53 (2,4–2,63) | 6 (3–9) |
| 24 | 1,22 (1,12–1,28) | 54 (43,5–63) | 25 (19,5–29,5) | 5 (4–6) | 2,56 (2,44–2,67) | 4,5 (2–9,5) |
| 25 | 1,23 (1,17–1,29) | 56 (45–64) | 25 (20,5–30,5) | 5 (4–6) | 2,57 (2,46–2,71) | 6 (2–9,5) |
| 26 | 1,25 (1,2–1,33) | 57 (46–67,5) | 26 (22–30) | 5,5 (4–6) | 2,57 (2,4–2,75) | 4,5 (2–8,5) |
| 27 | 1,28 (1,21–1,35) | 57 (46,5–68) | 27 (23,5–31) | 6 (4–7,5) | 2,57 (2,4–2,76) | 6,5 (2,5–9,5) |
| 28 | 1,3 (1,21–1,37) | 57,5 (46,5–69) | 27,5 (24–32,5) | 6 (5–7) | 2,65 (2,47–2,75) | 4 (0,5–9) |
| 29 | 1,3 (1,21–1,37) | 58 (47–69,5) | 28 (25–33) | 6 (5–7,5) | 2,67 (2,44–2,76) | 5 (1–7,5) |
| 30 | 1,3 (1,21–1,38) | 58 (48–70) | 29,5 (24–34) | 6 (5–8) | 2,67 (2,5–2,76) | 4 (1,5–7,5) |
| 31 | 1,31 (1,22–1,4) | 58,5 (49–70,5) | 29,5 (24–34) | 7 (5,5–8,5) | 2,65 (2,5–2,87) | 5,5 (1–11) |
| 32 | 1,32 (1,21–1,41) | 59,5 (49–72) | 30 (25–35) | 7 (5,5–9) | 2,69 (2,56–2,87) | 4 (1–8,5) |
| 33 | 1,33 (1,2–1,39) | 61 (48,5–73) | 30 (26–34) | 6 (5,5–9) | 2,69 (2,5–2,87) | 3,5 (1–9,5) |
| 34 | 1,33 (1,19–1,4) | 61,5 (48,5–74) | 30,5 (26,5–35) | 6 (5–9) | 2,69 (2,42–2,88) | 2,5 (1–9) |
| 35 | 1,32 (1,21–1,41) | 62,5 (49–74) | 30,5 (27–35) | 6 (4–9) | 2,65 (2,44–2,88) | 3 (1,5–7,5) |
| 36 | 1,32 (1,22–1,42) | 63 (50,5–75) | 32 (28–35) | 7 (4–9) | 2,57 (2,44–2,83) | 4 (2–11,5) |
| 37 | 1,34 (1,2–1,41) | 63,5 (52,5–75,5) | 32 (25,5–35) | 6 (4–9) | 2,57 (2,31–2,83) | 4,5 (1,5–8,5) |
| 38 | 1,34 (1,2–1,41) | 64 (53–77) | 32 (26,5–35,5) | 6,5 (5–8) | 2,65 (2,41–2,88) | 6 (0,5–11) |
| 39 | 1,34 (1,24–1,42) | 66,5 (53–77) | 31,5 (26,5–37) | 6,5 (5,5–7,5) | 2,67 (2,4–2,88) | 4 (1–13) |
| 40 | 1,34 (1,25–1,44) | 65,5 (53,5–78) | 33 (26–39) | 6,5 (5,5–8,5) | 2,71 (2,42–2,88) | 5,5 (0,5–11) |
| 41 | 1,35 (1,27–1,44) | 67 (54,5–79) | 33 (26,5–39) | 7 (5–9) | 2,69 (2,48–2,88) | 4,5 (1,5–10,5) |
| 42 | 1,35 (1,28–1,43) | 68 (55,5–80,5) | 32,5 (26,5–36,5) | 7 (5,5–9) | 2,73 (2,44–2,88) | 4,5 (0,5–10,5) |
| 43 | 1,34 (1,26–1,45) | 68 (56–81,5) | 33 (26,5–37) | 7 (5,5–9,5) | 2,71 (2,42–2,88) | 5 (1–9,5) |
| 44 | 1,35 (1,28–1,47) | 68 (57–81,5) | 34 (27–38) | 7 (6–9) | 2,75 (2,54–2,88) | 6 (2–13,5) |
| 45 | 1,35 (1,29–1,48) | 69,5 (57–81) | 34,5 (28–38) | 7 (6–9) | 2,73 (2,53–2,88) | 4,5 (0–9) |
| 46 | 1,35 (1,29–1,48) | 69,5 (57–81,5) | 34,5 (28–39) | 7 (6–9) | 2,73 (2,46–2,88) | 4 (1–11) |
| 47 | 1,36 (1,28–1,49) | 69 (56,5–83) | 34 (29–40) | 7 (6–9) | 2,71 (2,46–2,88) | 7 (1,5–10) |
| 48 | 1,34 (1,28–1,5) | 69,5 (55,5–83,5) | 34,5 (29–40,5) | 7,5 (6–9) | 2,71 (2,42–2,88) | 4,5 (1–10,5) |
| 49 | 1,33 (1,25–1,48) | 69,5 (56,5–84) | 34,5 (28,5–39) | 7,5 (6–9) | 2,69 (2,4–2,88) | 7,5 (1–11,5) |
| 50 | 1,35 (1,27–1,46) | 71 (57,5–85,5) | 34 (29,5–39) | 7 (6–10) | 2,73 (2,45–2,94) | 4,5 (0,5–6,5) |
| 51 | 1,34 (1,25–1,45) | 72 (59–86) | 35 (29,5–39,5) | 7 (5,5–10) | 2,65 (2,45–2,88) | 3,5 (2–7) |
| 52 | 1,37 (1,27–1,47) | 73,5 (60,5–88) | 35,5 (31–40) | 7 (6–10) | 2,71 (2,56–2,88) | 4,5 (0,5–10,5) |
| 53 | 1,36 (1,28–1,47) | 73 (60,5–88,5) | 35,5 (31–39,5) | 7 (6,5–10,5) | 2,71 (2,56–2,88) | 3,5 (1–7,5) |
| 54 | 1,35 (1,26–1,49) | 74 (60,5–88,5) | 36 (30,5–39,5) | 7 (5,5–10) | 2,73 (2,56–2,88) | 3 (1–12) |
| 55 | 1,36 (1,27–1,51) | 73 (61,5–88,5) | 36 (32–40,5) | 7 (6–10) | 2,75 (2,59–2,89) | 5 (1,5–15) |
| 56 | 1,35 (1,27–1,47) | 72 (61,5–90,5) | 36 (31–40,5) | 7,5 (6–10) | 2,71 (2,56–2,89) | 4 (0–11,5) |
| 57 | 1,36 (1,23–1,5) | 73 (62–92) | 36 (30,5–41) | 8 (6,5–10,5) | 2,78 (2,67–2,89) | 5 (1–12,5) |
| 58 | 1,37 (1,24–1,5) | 74 (62,5–92,5) | 36 (31–40,5) | 7 (6,5–10,5) | 2,78 (2,64–2,88) | 3,5 (0,5–7,5) |
| 59 | 1,33 (1,23–1,5) | 73,5 (62–92,5) | 36 (31–40) | 7,5 (6,5–10) | 2,83 (2,61–2,94) | 6,5 (0,5–14) |
| 60 | 1,37 (1,23–1,5) | 73,5 (62–93) | 36,5 (31,5–42,5) | 8 (7–10,5) | 2,86 (2,68–2,94) | 3 (0–7) |

### Deniz

| Yıl | Liman (tersane) | Gemi (koga/tekne) | Kadırga | Denizaşırı yerleşim | Deniz ticaret yolu (yıl sonu) | Deniz seferi (ticaret) |
|---|---|---|---|---|---|---|
| 1 | 0 (0–1) | 0 | 0 | 0 | 0 | 0 |
| 2 | 1 (0–2) | 0,5 (0–1) | 0 | 0 | 0 | 0 |
| 3 | 1 (0–2,5) | 1 (0–2,5) | 0 | 0 | 0 | 0 |
| 4 | 2 (1–3) | 1,5 (1–3) | 0 | 0 | 0 | 0 |
| 5 | 2 (1–4) | 2 (1–4,5) | 0 | 0 | 0 | 0 |
| 6 | 2,5 (1–4,5) | 4 (1–9,5) | 0 | 0 (0–1) | 0 (0–1) | 0 |
| 7 | 3,5 (2–7) | 7,5 (3–15) | 0 | 1 (0–3) | 1 (0–3,5) | 0 |
| 8 | 5 (2,5–9,5) | 10 (6,5–21,5) | 0 | 2 (1–5,5) | 2 (1–6,5) | 0 (0–2,5) |
| 9 | 6,5 (3,5–11) | 15,5 (9–24,5) | 0 | 3,5 (2,5–7,5) | 4 (3–10) | 1 (0–5,5) |
| 10 | 8 (5,5–13) | 16 (11,5–28) | 0 | 4,5 (3–8) | 5 (4–11) | 3 (0–8) |
| 11 | 8,5 (5,5–13) | 17,5 (12–30) | 0 | 5 (3–8,5) | 6 (4–12) | 4 (0–10,5) |
| 12 | 9 (6–13) | 18,5 (13,5–32) | 0 (0–2,5) | 5,5 (3–8,5) | 6 (4–12) | 4 (0–11) |
| 13 | 10 (7,5–14,5) | 20,5 (15,5–34,5) | 1 (0–6,5) | 6 (3,5–8,5) | 6 (4,5–12,5) | 4 (0–11,5) |
| 14 | 10 (8–15) | 24 (17,5–38) | 3 (0–8,5) | 6 (3,5–8,5) | 6 (5–13,5) | 8 (0–17) |
| 15 | 10 (8–15,5) | 26 (18,5–39) | 5 (0–10,5) | 6 (3,5–9) | 8 (5,5–14) | 6,5 (1,5–18,5) |
| 16 | 11,5 (9–16) | 27,5 (20,5–40,5) | 6 (1,5–12) | 6 (4–10,5) | 8 (6–14,5) | 8 (2–19) |
| 17 | 12 (9–17,5) | 30 (21,5–40,5) | 8 (1,5–13,5) | 6 (4,5–11) | 8,5 (6–15) | 9,5 (3,5–22,5) |
| 18 | 12 (9,5–17,5) | 31,5 (23,5–42,5) | 8,5 (1,5–15) | 7 (4,5–11) | 9 (6–15) | 10 (3–25) |
| 19 | 12 (10–18) | 32 (23,5–42,5) | 10,5 (2–18,5) | 7,5 (4,5–11,5) | 9,5 (6–17) | 12 (3–29,5) |
| 20 | 13 (11–18) | 32,5 (25,5–45,5) | 12 (3–19,5) | 7,5 (4,5–11,5) | 9,5 (6,5–17,5) | 11,5 (3,5–29,5) |
| 21 | 13,5 (11,5–19,5) | 37 (26,5–47,5) | 12,5 (5–19) | 7,5 (4,5–12) | 10,5 (7–18,5) | 12 (3,5–31,5) |
| 22 | 15 (11,5–20) | 37 (27,5–47) | 13 (8–19) | 8 (4,5–12,5) | 11 (7–19) | 12 (4–32,5) |
| 23 | 15 (12–20) | 37 (28–47) | 13 (8–20) | 8 (4,5–12,5) | 11,5 (7–19) | 12 (5,5–34) |
| 24 | 15 (12–20,5) | 37 (28–47,5) | 14 (8–22) | 8,5 (4,5–12,5) | 12 (7,5–19,5) | 13,5 (6–37) |
| 25 | 15,5 (12–21) | 37 (30–46,5) | 14,5 (8–23,5) | 8,5 (4,5–12,5) | 12 (7,5–20) | 13,5 (6,5–40) |
| 26 | 15,5 (12–21) | 37 (31,5–51) | 14,5 (8,5–23,5) | 8,5 (4,5–12,5) | 12 (8–20) | 12 (7,5–39) |
| 27 | 15,5 (12–21) | 38 (32–51) | 15,5 (9–23,5) | 9 (4,5–12,5) | 12,5 (7,5–21) | 15,5 (7–38,5) |
| 28 | 15,5 (12–21) | 38,5 (31,5–51,5) | 15,5 (10–23,5) | 9 (4,5–12,5) | 12,5 (7,5–21,5) | 16 (8–38) |
| 29 | 16 (12–21) | 39,5 (32–52) | 15,5 (10–24,5) | 9,5 (4,5–12,5) | 12,5 (7,5–22) | 16,5 (8–36,5) |
| 30 | 16 (12–21) | 41 (33–53,5) | 16 (10–24,5) | 9,5 (4,5–13) | 13,5 (7,5–22) | 20 (10–40,5) |
| 31 | 16,5 (12,5–21) | 41 (34,5–53) | 16 (10–24,5) | 9,5 (4,5–13) | 13,5 (8–23) | 21 (10,5–41,5) |
| 32 | 17 (12,5–21) | 41,5 (34,5–53,5) | 16,5 (10,5–24,5) | 9,5 (4,5–13) | 13,5 (8–23) | 21 (9,5–39,5) |
| 33 | 17 (12,5–21,5) | 41,5 (34,5–54) | 17 (10,5–24,5) | 9,5 (4,5–13) | 13,5 (8–23) | 23,5 (10–41,5) |
| 34 | 17 (13,5–21,5) | 41,5 (34,5–54,5) | 17,5 (10,5–24,5) | 9,5 (4,5–13) | 14 (8–23,5) | 21,5 (9–42,5) |
| 35 | 17,5 (13,5–21,5) | 43 (34,5–55,5) | 17 (10,5–24,5) | 9,5 (4,5–13) | 14 (8–23,5) | 23 (9,5–45) |
| 36 | 18 (13,5–22) | 45,5 (34,5–56) | 17 (10,5–25,5) | 9,5 (4,5–13) | 14 (8–24) | 24,5 (9–42) |
| 37 | 18 (13,5–22,5) | 45,5 (34,5–56) | 16,5 (10,5–25,5) | 9,5 (4,5–13) | 14 (8–24) | 22 (9,5–44) |
| 38 | 18 (14–23) | 45,5 (34,5–58) | 16,5 (10,5–25,5) | 9,5 (4,5–13) | 15,5 (9–24) | 23,5 (9,5–40,5) |
| 39 | 18,5 (14–23) | 45,5 (34,5–57,5) | 16,5 (10,5–25,5) | 9,5 (4,5–13) | 15,5 (9–24) | 23 (11–43) |
| 40 | 18,5 (14–23) | 45,5 (34,5–57,5) | 16,5 (10,5–28) | 9,5 (4,5–13,5) | 17 (9–24,5) | 22 (12–43,5) |
| 41 | 19 (14,5–23) | 45,5 (34,5–58) | 16,5 (12–27,5) | 9,5 (4,5–13,5) | 17 (9–24,5) | 24,5 (11,5–47,5) |
| 42 | 19,5 (14,5–23) | 46,5 (34,5–59,5) | 16,5 (12–27) | 9,5 (4,5–13,5) | 17,5 (9–25,5) | 24 (12–51) |
| 43 | 19,5 (14,5–25) | 46 (35,5–62,5) | 16,5 (12–27,5) | 10 (4,5–13,5) | 17,5 (9–25,5) | 25 (11,5–50,5) |
| 44 | 19,5 (15–25,5) | 46 (35,5–61) | 16,5 (12–27,5) | 10 (4,5–13,5) | 17,5 (9–26,5) | 23,5 (11,5–55,5) |
| 45 | 19,5 (15,5–25,5) | 46,5 (35,5–61) | 16,5 (12–28,5) | 10 (4,5–13,5) | 17,5 (9,5–26,5) | 25 (11–55,5) |
| 46 | 20 (15,5–25,5) | 46,5 (35,5–60,5) | 16,5 (11,5–28,5) | 10 (4,5–13,5) | 17,5 (9,5–26,5) | 27,5 (12–54) |
| 47 | 20 (16–25,5) | 47 (35,5–60,5) | 16,5 (11,5–28,5) | 10 (4,5–13,5) | 17,5 (9,5–26,5) | 29 (11–54,5) |
| 48 | 20,5 (16–26) | 46,5 (35,5–62) | 16,5 (11,5–28,5) | 10 (4,5–13,5) | 17,5 (9,5–27,5) | 30 (11–58,5) |
| 49 | 21 (16,5–26,5) | 48,5 (34,5–62,5) | 16,5 (11,5–29) | 10 (4,5–14) | 17,5 (9,5–28) | 29 (10,5–59) |
| 50 | 21 (16–26,5) | 50 (34,5–62,5) | 16,5 (11,5–29) | 10 (4,5–14) | 17,5 (9,5–28,5) | 29,5 (10–58) |
| 51 | 21,5 (16–27) | 50 (34,5–64,5) | 16,5 (11,5–29) | 10 (4,5–14) | 17,5 (9,5–29,5) | 28 (11–62) |
| 52 | 21,5 (16–27) | 53 (34,5–65) | 16,5 (11,5–29) | 10 (4,5–14) | 17,5 (9,5–30) | 29 (11–58) |
| 53 | 21,5 (16,5–28) | 53,5 (34,5–65,5) | 16,5 (11,5–29) | 10 (4,5–14) | 17,5 (9,5–31) | 30 (10,5–57) |
| 54 | 21,5 (16,5–28,5) | 55 (34–63) | 16,5 (11,5–29) | 10 (4,5–14) | 18,5 (9,5–31) | 30 (11–54,5) |
| 55 | 21,5 (16,5–29) | 55 (35–66) | 18 (11,5–29) | 10 (4,5–14) | 18,5 (10,5–31) | 33,5 (11–60,5) |
| 56 | 21,5 (16–29) | 54,5 (35,5–66,5) | 19 (11,5–29) | 10 (4,5–14) | 18,5 (10,5–31) | 34 (11–62) |
| 57 | 21,5 (16–29) | 54,5 (35,5–66) | 19 (11,5–29) | 10 (4,5–14) | 18,5 (10,5–32,5) | 29 (11,5–62,5) |
| 58 | 22,5 (16,5–29) | 55 (36,5–66) | 19 (11,5–29) | 10 (4,5–14) | 18,5 (10,5–32,5) | 32,5 (10,5–58,5) |
| 59 | 22,5 (16,5–29) | 54,5 (36,5–68) | 19 (11,5–30,5) | 10 (4,5–14) | 19 (10,5–33,5) | 29,5 (12,5–63) |
| 60 | 22,5 (16,5–29,5) | 54,5 (36,5–69) | 19 (11,5–30,5) | 10 (4,5–14) | 19 (10,5–33,5) | 31,5 (12–67) |

### v3: durum değişimi (1/2)

| Yıl | Yaşayan yerleşim (yıl ort.) | Büyük şehir (Şehir kademesi, yıl ort.) | El değiştiren yerleşim (fetih + bölünme) | El değiştiren büyük şehir | Büyük şehre hücum (kuşatma muharebesi) | Büyük şehir yağmalandı, tutulmadı |
|---|---|---|---|---|---|---|
| 1 | 8 (7–9) | 0 | 0 | 0 | 0 | 0 |
| 2 | 8,16 (7,18–9,36) | 0 | 0 | 0 | 0 | 0 |
| 3 | 12,1 (9,79–14,1) | 0 | 0 | 0 | 0 | 0 |
| 4 | 17,2 (14,2–20,4) | 0 | 0 | 0 | 0 | 0 |
| 5 | 23,2 (19,1–26,7) | 0 | 0 (0–1) | 0 | 0 | 0 |
| 6 | 28,8 (23,2–32,4) | 0 | 0,5 (0–1,5) | 0 | 0 | 0 |
| 7 | 33,4 (27,3–37,1) | 0 | 0 (0–1,5) | 0 | 0 | 0 |
| 8 | 37,7 (30,2–41,7) | 0 | 1 (0–2) | 0 | 0 | 0 |
| 9 | 41,6 (34–46,9) | 0 | 1 (0–1,5) | 0 | 0 | 0 |
| 10 | 45,5 (37,3–51,1) | 0 (0–0,16) | 1 (0–2,5) | 0 | 0 | 0 |
| 11 | 48 (38,5–53,4) | 0 (0–0,76) | 0 (0–2,5) | 0 | 0 | 0 |
| 12 | 50,1 (39,3–56,1) | 0,04 (0–0,81) | 1 (0–2) | 0 | 0 | 0 |
| 13 | 51,4 (41,1–58,8) | 0,91 (0–1,04) | 1 (0–2) | 0 | 0 | 0 |
| 14 | 54,3 (42,9–61) | 1,09 (0–1,91) | 0 (0–1,5) | 0 | 0 | 0 |
| 15 | 57 (44–62,9) | 1,45 (0,25–2,51) | 1 (0–2) | 0 | 0 | 0 |
| 16 | 58,5 (46–65,9) | 2,01 (0,54–3,45) | 1 (0–2,5) | 0 | 0 (0–1) | 0 (0–0,5) |
| 17 | 60,4 (46,9–68,9) | 2,43 (0,63–3,74) | 1 (0–2,5) | 0 | 0 | 0 |
| 18 | 61,5 (48,2–71,3) | 2,98 (1–4,3) | 1 (0–2,5) | 0 | 0 | 0 |
| 19 | 62,9 (50,1–72,2) | 3,73 (1–4,63) | 0,5 (0–3) | 0 | 0 (0–1) | 0 (0–0,5) |
| 20 | 64,6 (50,7–72,9) | 3,81 (1–5,38) | 0,5 (0–2) | 0 | 0 | 0 |
| 21 | 67 (51,5–74,7) | 4 (1,35–5,93) | 0,5 (0–3) | 0 | 0 (0–1) | 0 (0–1) |
| 22 | 68,4 (52–75,8) | 4,44 (2,76–5,71) | 1 (0–2) | 0 | 0 (0–0,5) | 0 |
| 23 | 70 (52–77,2) | 4,68 (2,95–5,49) | 1 (0–2,5) | 0 | 0 (0–1) | 0 (0–0,5) |
| 24 | 70,5 (52,7–78,3) | 4,76 (3,65–5,8) | 0,5 (0–2) | 0 | 0 (0–0,5) | 0 (0–0,5) |
| 25 | 71,4 (54,3–79,3) | 4,88 (3,78–5,83) | 1 (0–2) | 0 | 0 | 0 |
| 26 | 71,8 (55,6–80,5) | 5,08 (4,1–6,1) | 0 (0–2) | 0 | 0 (0–0,5) | 0 |
| 27 | 71,9 (57–81) | 5,38 (4,26–6,18) | 1 (0–4) | 0 | 0 (0–0,5) | 0 |
| 28 | 72,6 (57,4–81,6) | 6 (4,89–7,3) | 1 (0–3,5) | 0 | 0 (0–1) | 0 (0–0,5) |
| 29 | 74 (58,7–82,1) | 6,05 (5–6,9) | 1 (0–3) | 0 | 0 | 0 |
| 30 | 74,9 (59,4–82,7) | 6,25 (5–7,68) | 1 (0–2) | 0 | 0 | 0 |
| 31 | 75,5 (60,2–84,1) | 6,41 (5,1–7,73) | 1 (0–3) | 0 | 0 | 0 |
| 32 | 76 (60,8–84,7) | 7 (5,26–8,43) | 0 (0–1) | 0 | 0 (0–1) | 0 (0–1) |
| 33 | 76 (61,8–85,9) | 6,8 (5,36–8,59) | 0,5 (0–2,5) | 0 | 0 (0–0,5) | 0 (0–0,5) |
| 34 | 76,1 (62,4–86,3) | 6 (5,34–8,88) | 0,5 (0–2) | 0 | 0 (0–0,5) | 0 (0–0,5) |
| 35 | 77 (62,4–87,7) | 6 (4,74–9,16) | 0,5 (0–2,5) | 0 | 0 (0–1) | 0 (0–0,5) |
| 36 | 77 (63,2–87,9) | 6,86 (4,08–9,01) | 1 (0–2,5) | 0 | 0 (0–1) | 0 (0–1) |
| 37 | 77,5 (63,6–88,3) | 6,86 (4,18–8,95) | 1 (0–2) | 0 | 0 | 0 |
| 38 | 77,7 (63,9–89) | 6,73 (4,54–8,45) | 1 (0–3) | 0 | 0 | 0 |
| 39 | 78,6 (64,1–89,9) | 6,61 (4,9–7,79) | 0 (0–2,5) | 0 | 0 (0–1) | 0 (0–0,5) |
| 40 | 79,6 (63,6–90,5) | 6,65 (5,06–7,5) | 1 (0–2,5) | 0 | 0 (0–0,5) | 0 |
| 41 | 80,7 (64,1–91,4) | 6,4 (5,8–8,41) | 1 (0–3,5) | 0 | 0 (0–1) | 0 (0–1) |
| 42 | 81,2 (64,7–91,9) | 6,99 (5,39–9) | 1 (0–1,5) | 0 | 0 (0–0,5) | 0 (0–0,5) |
| 43 | 81,4 (65,9–92,4) | 7,09 (5,13–9,19) | 1 (0–2,5) | 0 | 0 (0–1) | 0 (0–1) |
| 44 | 82 (66–93,8) | 6,93 (5,59–9,11) | 1 (0–3) | 0 | 0 (0–0,5) | 0 (0–0,5) |
| 45 | 82 (65,6–94,4) | 7 (6,31–9,39) | 0,5 (0–3) | 0 | 0 (0–0,5) | 0 |
| 46 | 82 (66,3–94,6) | 7,1 (5,98–8,63) | 1 (0–1,5) | 0 | 0 (0–2) | 0 (0–1) |
| 47 | 83 (67,1–95,3) | 7,51 (6,33–8,81) | 1 (0–1,5) | 0 | 0 (0–1) | 0 (0–1) |
| 48 | 83 (67,5–96,5) | 7,36 (6,15–9) | 1 (0–3) | 0 | 0 (0–1) | 0 (0–0,5) |
| 49 | 83,4 (68,1–97,5) | 7,29 (6,01–9,09) | 1 (0–2,5) | 0 | 0 | 0 |
| 50 | 84,1 (68,9–97,9) | 7 (6,05–9,86) | 0,5 (0–2) | 0 (0–0,5) | 0 (0–1) | 0 (0–0,5) |
| 51 | 85,3 (68,8–98,5) | 7 (5,5–9,71) | 0,5 (0–2,5) | 0 | 0 (0–1,5) | 0 (0–1) |
| 52 | 86,1 (68,7–99) | 7 (5,99–10) | 1 (0–3) | 0 | 0 | 0 |
| 53 | 86 (69,1–101) | 7,21 (5,85–10,1) | 1 (0–2) | 0 | 0 (0–2) | 0 (0–0,5) |
| 54 | 85,9 (68,9–102) | 7,13 (6,06–10,3) | 1 (0–3) | 0 | 0 (0–1,5) | 0 (0–1) |
| 55 | 86,3 (69–102) | 7,04 (5,76–10,2) | 1 (0–2) | 0 | 0 | 0 |
| 56 | 86,5 (69,7–103) | 7,59 (6,03–9,96) | 0 (0–3) | 0 | 0 | 0 |
| 57 | 86,4 (70,2–105) | 7,91 (6,25–9,84) | 1 (0–4,5) | 0 (0–1) | 0 (0–1,5) | 0 (0–1) |
| 58 | 86,4 (70,3–106) | 7,93 (6,41–10,2) | 0 (0–2) | 0 | 0 (0–0,5) | 0 |
| 59 | 86,7 (70,5–106) | 7,29 (6,29–10,6) | 1 (0–3) | 0 | 0 (0–1,5) | 0 (0–0,5) |
| 60 | 87 (71–107) | 7,86 (6,7–10) | 0,5 (0–3) | 0 | 0 (0–0,5) | 0 |

### v3: durum değişimi (2/2)

| Yıl | Yerleşim durum değişimi (kuruluş hariç hepsi) | Orta halka kaydı (Köy/Kasaba: kademe değişimi ya da terk) | Açlık başlayan yerleşim | Salgın başlayan yerleşim | Yakılan/yanan yerleşim | Harabeye yeniden yerleşim |
|---|---|---|---|---|---|---|
| 1 | 0 | 0 | 0 | 0 | 0 | 0 |
| 2 | 6 (4–7) | 0 | 0 | 0 | 0 | 0 |
| 3 | 5,5 (3,5–6,5) | 0,5 (0–1) | 0 | 0 | 1,5 (0–3) | 0 |
| 4 | 6 (3,5–8) | 0 (0–1,5) | 0 | 0 | 3 (0,5–4,5) | 0 |
| 5 | 10 (6,5–14) | 1 (0–2) | 0 | 0 | 3,5 (1,5–6) | 0 |
| 6 | 10 (8–20,5) | 2 (0–4,5) | 0 | 0 (0–0,5) | 3 (1–6) | 0 |
| 7 | 12,5 (8–17) | 3 (1–6) | 0 | 0 | 3 (1,5–4) | 0 |
| 8 | 14 (7,5–16) | 3 (1,5–5,5) | 0 | 0 (0–3) | 3,5 (1,5–7) | 0 |
| 9 | 14 (7–18,5) | 4 (1,5–5,5) | 0 | 0 (0–1) | 4 (1,5–5,5) | 0 |
| 10 | 9 (6–22) | 3,5 (1–5) | 0 | 0 (0–1) | 3 (2–6,5) | 0 |
| 11 | 9,5 (6–17,5) | 2 (1–4) | 0 | 0 (0–3) | 4 (1–6) | 0 |
| 12 | 14 (7–18,5) | 2 (1–5,5) | 0 | 0 (0–1) | 4 (2–8) | 0 |
| 13 | 13 (7–17,5) | 3 (1,5–5,5) | 0 | 0 (0–2) | 3 (1–9,5) | 0 |
| 14 | 11,5 (6,5–15,5) | 3 (0,5–5) | 0 | 0 (0–1,5) | 3 (2–7,5) | 0 |
| 15 | 12 (6–24,5) | 3,5 (1–8) | 0 | 0 (0–1,5) | 4 (1,5–7) | 0 |
| 16 | 13 (5,5–25,5) | 3 (1–4) | 0 | 0 (0–1,5) | 5,5 (2,5–11) | 0 |
| 17 | 14,5 (6,5–33) | 3,5 (1–7,5) | 0 | 0 (0–1,5) | 6 (2–12) | 0 |
| 18 | 14 (4,5–22,5) | 3 (0,5–5) | 0 | 0 (0–1,5) | 5,5 (2–8,5) | 0 |
| 19 | 13 (6–33,5) | 2,5 (0,5–5,5) | 0 | 0 (0–1) | 5,5 (3–15) | 0 |
| 20 | 16 (8–23,5) | 3,5 (0,5–5) | 0 | 0 (0–2) | 6 (2–12) | 0 |
| 21 | 16,5 (7–34,5) | 3,5 (1,5–8) | 0 (0–2,5) | 0 (0–2) | 6 (3–13) | 0 |
| 22 | 12 (8–23,5) | 2,5 (0–7) | 0 | 0 (0–1) | 6 (2,5–13) | 0 (0–0,5) |
| 23 | 19 (6,5–29) | 4 (1,5–6) | 0 (0–2,5) | 0 (0–1,5) | 7 (2,5–11,5) | 0 |
| 24 | 16 (6–25) | 2,5 (1–6,5) | 0 | 1 (0–1,5) | 6,5 (1,5–14) | 0 |
| 25 | 18 (5,5–27,5) | 3 (1,5–6) | 0 | 0 (0–1,5) | 6,5 (2,5–15) | 0 |
| 26 | 11 (7–33) | 2 (1–4,5) | 0 | 0 (0–2,5) | 4,5 (3–12) | 0 |
| 27 | 18 (9–33) | 3,5 (1–6) | 0 | 0,5 (0–3,5) | 7,5 (3–12,5) | 0 |
| 28 | 15,5 (6–31,5) | 2 (0–6) | 0 | 0 (0–1,5) | 7,5 (3,5–14,5) | 0 (0–0,5) |
| 29 | 14 (5,5–26) | 3 (0,5–6) | 0 | 0 (0–2) | 5,5 (3–11) | 0 |
| 30 | 15,5 (13–23,5) | 2,5 (1–5,5) | 0 | 0,5 (0–3,5) | 8,5 (4,5–12) | 0 |
| 31 | 16 (3,5–31,5) | 3 (1–7) | 0 | 0 (0–2) | 6 (2–14,5) | 0 (0–0,5) |
| 32 | 12,5 (4,5–22,5) | 1,5 (0–4,5) | 0 | 0 (0–1) | 7 (3–10,5) | 0 |
| 33 | 13 (5–28,5) | 2 (1–5) | 0 (0–6) | 0 (0–1) | 9 (2–14) | 0 |
| 34 | 15,5 (6–27,5) | 2,5 (0–7) | 0 (0–1,5) | 0 (0–2) | 7 (3–13,5) | 0 |
| 35 | 14,5 (6–36) | 2 (0,5–4,5) | 0 | 0 (0–2) | 7 (3,5–15,5) | 0 (0–0,5) |
| 36 | 16,5 (6,5–30) | 2,5 (0,5–7) | 0 | 0 (0–2) | 6 (2,5–14,5) | 0 |
| 37 | 11,5 (4–32) | 2 (0–6) | 0 | 0,5 (0–3) | 4 (1–16,5) | 0 |
| 38 | 16 (4–28,5) | 2,5 (0–8) | 0 | 0 (0–2) | 5,5 (4–13) | 0 |
| 39 | 15,5 (4–32) | 2 (1–6) | 0 | 0 (0–2) | 8 (1–15) | 0 (0–0,5) |
| 40 | 15,5 (4,5–29,5) | 3,5 (0,5–6) | 0 | 0 (0–3,5) | 7 (2–14,5) | 0 |
| 41 | 13 (4,5–36) | 2,5 (0–6) | 0 | 0,5 (0–2,5) | 5,5 (1,5–17) | 0 |
| 42 | 15 (5–32) | 2,5 (0,5–5,5) | 0 | 0 (0–3,5) | 8 (3–13,5) | 0 |
| 43 | 14,5 (5–27,5) | 2 (1–5) | 0 | 0,5 (0–1) | 7,5 (3–11,5) | 0 |
| 44 | 13,5 (4–36,5) | 3,5 (1–7) | 0 | 0 (0–1) | 8 (1–17) | 0 |
| 45 | 15,5 (4,5–26) | 3,5 (0–5) | 0 | 0,5 (0–3) | 6,5 (3–13) | 0 |
| 46 | 15,5 (3,5–34,5) | 2 (0–7,5) | 0 | 0 (0–1,5) | 7,5 (2–17,5) | 0 (0–0,5) |
| 47 | 20,5 (5–30) | 3,5 (1–7) | 0 | 0 (0–2,5) | 8 (2,5–16,5) | 0 |
| 48 | 13 (4–36,5) | 3 (0–7,5) | 0 | 0 (0–2) | 5 (1–16,5) | 0 |
| 49 | 17 (5–39,5) | 4 (1–9) | 0 | 1 (0–2) | 6,5 (1–16,5) | 0 |
| 50 | 15,5 (6–26) | 2,5 (0,5–4,5) | 0 (0–2) | 0,5 (0–4) | 7 (3–16) | 0 |
| 51 | 13 (7–28,5) | 2 (0,5–5) | 0 | 0 (0–1) | 6 (3–16) | 0 (0–0,5) |
| 52 | 12,5 (5–32) | 2,5 (0–8,5) | 0 | 0 (0–4) | 6,5 (2,5–15,5) | 0 |
| 53 | 16,5 (5–23,5) | 2 (0,5–5) | 0 | 1 (0–3) | 8 (1–13,5) | 0 |
| 54 | 13,5 (4,5–31) | 2 (0,5–8,5) | 0 | 0 (0–1) | 7 (3–13,5) | 0 (0–1) |
| 55 | 18 (7,5–30) | 3,5 (0,5–10) | 0 | 0 (0–2) | 8 (2,5–15,5) | 0 |
| 56 | 12 (5–36,5) | 3 (0–7) | 0 (0–0,5) | 0 (0–2) | 6,5 (2,5–17,5) | 0 (0–1) |
| 57 | 16,5 (8–49,5) | 3,5 (0,5–9) | 0 | 0 (0–3,5) | 8,5 (3–23,5) | 0 |
| 58 | 14 (3–30) | 2 (0–5) | 0 | 0,5 (0–1) | 6 (1,5–18) | 0 (0–1) |
| 59 | 20 (6,5–39,5) | 4,5 (0,5–9,5) | 0 | 0 (0–3,5) | 8 (3–19) | 0 |
| 60 | 14,5 (3,5–33) | 3 (0–6) | 0 | 0 (0–1,5) | 7 (2–20,5) | 0 |

