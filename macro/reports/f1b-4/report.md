# Ölçüm raporu: f1b-4

16 dünya (seed 1-16) × 60 yıl (7200 gün), 1 yıl = 120 gün · 2026-10-01 19:58 · `FD.Macro.Run stats --seeds 1-16 --years 60 --jobs 2 --verify 1 --saveload 1`

Süre: 7 dk 0 sn duvar saati, 2 iş parçacığı; dünya başına 45,1 sn (en az 33,6, en çok 58,5; yıl sonu hash'leri dâhil).

## Bitiş ölçütleri

DESIGN-FAZ1.md, "Bitiş ölçütleri" (1–5) ve yol haritası v3 (6–7, Faz 1b-4). ✓ geçti · ✗ kaldı · — ölçülemedi · ○ bilgi (hedef yok) · † gelecek zaman ölçeğine bağlı (yeni takvime yansıtılmış değerle değerlendirilir; bkz. "v3: durum değişimi").

| # | Ölçüt | Koşul | Ölçülen | Sonuç |
|---|---|---|---|---|
| 1 | Donma yok | 41–60. yılların yıllık büyük olay medyanı ≥ 0,8 × 6–20. yılların medyanı | 45,5 / 44 = 1,03 kat | ✓ |
| 2 | Çöküş | dünyaların ≥ %75'inde 60 yılda ≥ 1 çöküş (yok olma ya da başkent kaybı) | %88 (14/16 dünya); toplam 40 çöküş: 11 yok olma, 29 başkent kaybı | ✓ |
| 3 | Kamplar | 41–60. yıllarda yaşayan kamp medyanı ≥ 6–20. yılların medyanı | 9 ≥ 8,75 (yıl sonu sayımıyla 9 / 9) | ✓ |
| 4a | Kahraman: doğuş seviyesi | her on yılda doğanların ortalama seviyesi ≤ 2 | 1,3 · 1,3 · 1,29 · 1,35 · 1,38 · 1,38 (on yıllar sırasıyla) | ✓ |
| 4b | Kahraman: Sv8+ | dünyaların ≥ yarısında en az bir kahraman Sv8 ve üstüne çıkar | %81 (13/16 dünya); dünyadaki en yüksek seviye: medyan Sv9, en çok Sv10 | ✓ |
| 4c | Kahraman: efsane | dünya başına efsane medyanı 1–6 | medyan 6 (p10–p90: 2–10; toplam 98) | ✓ |
| 4d | Kahraman: ölüm payı | doğan kahramanların %30–80'i ölür | %42 (1301/3115); dünya medyanı %41 (%35–%49) | ✓ |
| 5a | Determinizm | aynı seed → aynı tarih (toplayıcılı ve toplayıcısız koşu, her yıl sonu hash'i) | seed 1: 60/60 yıl sonu aynı | ✓ |
| 5b | Kayıt/yükleme | kaydet → yükle → devam = kesintisiz koşu (her yıl sonu hash'i) | seed 1, gün 3637: yüklenen dünya aynı, sonraki 30/30 yıl sonu aynı | ✓ |
| 6a | Yerleşim sayısı sabit, ısınmadan sonra (21–60. yıl) | dünya başına yaşayan yerleşim sayısının (günlük) değişim katsayısı medyanı ≤ %10 | değişim katsayısı %9,5 (%7,6–%13); ortalama 76,2 yerleşim, en az ve en çok ortalamanın %77 ve %114 kadarı | ✓ |
| 6b | Yerleşim sayısı sabit, bütün koşu (1–60. yıl) | dünya başına yaşayan yerleşim sayısının (günlük) değişim katsayısı medyanı ≤ %10 (dünya hâlâ 8 kamptan başlayıp büyüyor; dünya üretimi sonraki adım) | değişim katsayısı %36 (%33–%37); ortalama 62,8 yerleşim, en az ve en çok ortalamanın %14 ve %139 kadarı | ✗ |
| 6c | El değiştirme (her yerleşim) | fetih + bölünme, 100 günde, dünya başına ve yerleşim başına (21–60. yıl); hedef yok | dünyada 0,77 (0,16–1,07) / 100 gün; yerleşim başına 1,1 / 10 bin gün; yeni takvimde ×4: 3,08 / 100 gün | ○ |
| 6d | Durum dalgalanması (yerleşim başına) | açlık, salgın, kuşatma, yakılma, kademe, el değiştirme, terk, yeniden yerleşim: yerleşim başına 100 günde (21–60. yıl); hedef yok | yerleşim başına 0,15 (0,09–0,19) / 100 gün, yani ~675 günde bir; dünyada 11,4 / 100 gün; yeni takvimde ×4: yerleşim başına ~169 günde bir | ○ |
| 6e | Orta halka: kademe değişimi † | Köy/Kasaba başına kademe değişimi ya da terk, yeni takvimde 60–150 günde bir (21–60. yıl) | yerleşim başına 1783 (1493–2349) günde bir; yeni takvimde 446 günde bir | ✗ |
| 6f | Büyük şehir el değiştirmesi † | büyük şehir (Şehir kademesi) bütün dünyada yeni takvimde 100 günde 2–4 kez el değiştirir (21–60. yıl); felaketle düşmez | dünyada 0,02 (0–0,05) / 100 gün; yeni takvimde 0,08 / 100 gün; şehir başına 0 / 100 gün; dünyada ortalama 7,3 büyük şehir; toplam 19 el değiştirme (0 bölünme). Ayrıştırma: dünyada 1,03 savaş / 100 gün (yeni takvimde 4,13), hedefi büyük şehir olan %15; büyük şehre 112 hücum, düşüşle bitenlerin payı %17 | ✗ |
| 6g | Büyük şehir: uyarı süresi † | büyük şehrin düşüşünden önce yeni takvimde 5–10 gün uyarı (kuşatmanın başı → düşüş, medyan; 21–60. yıl) | kuşatmanın başından 20 gün (7,4–20; n = 19); savaşın başından 52 gün (33,8–79,2); yeni takvimde 5 / 13 gün | ✓ |
| 6h | Savaş süresi † | savaş (ilandan barışa) yeni takvimde 10–40 gün (medyan; 21–60. yıl başlayıp biten savaşlar) | 40 gün (20–140; n = 729); yeni takvimde 10 gün (5–35) | ✓ |
| 6i | Kuşatma süresi † | büyük şehir kuşatması (karargâhtan hücuma) yeni takvimde 2–6 gün (medyan; 21–60. yıl) | büyük şehir 21 gün (21–21; n = 112); diğer yerleşimler 1 gün (1–21; n = 634); yeni takvimde 5,25 / 0,25 gün | ✓ |
| 6j | Başkent kaybı (medeniyet başına) | hiçbir medeniyet başkentini 3 kereden çok kaybetmez (yol haritası: "en fazla birkaç kez") | en çok 2; 3'ten çok kaybeden 0 medeniyet; toplam 29 başkent kaybı, 26/141 medeniyette (1×: 23, 2×: 3) | ✓ |
| 7 | Felaket büyük şehri düşürmez | Şehir kademesine varmış yerleşim hiç terk edilmez; ejderha akını büyük şehrin nüfusunu Şehir eşiğinin (85) altına indiremez; akından sonraki 60 günde terk yok | terk edilen eski Şehir: 0; ejderha akını 561 (büyük şehre 70, orada 828 ölü): akınla eşiğin altına inen 0, sonraki 60 günde terk 0; bilgi: akın günü başka nedenlerle (ordu, öncü) eşiğin altına inen 4, 60 gün içinde kademe düşüşü 20, el değiştirme 0 | ✓ |

Tanımlar:

- **1, 3:** "41–60. yılların medyanı": 41–60. yıllardaki bütün (dünya, yıl) değerlerinin medyanı (16 dünya × 20 yıl = 320 değer); 6–20. yıllar için de aynı. Pencereler koşunun uzunluğuna göre: erken = koşunun %10'u – üçte biri, geç = son üçte biri (1 yıl = 120 gün). Büyük olay = `GameEvent.Major == true` olan olaylar (`Sim.OnEvent` ile sayılır, `World.Events` kırpılmasından etkilenmez). Kamp ölçütünde yıllık değer, o yıl her gün sayılan yaşayan kamp sayısının ortalamasıdır (korsan koyları dâhil; yıl sonu sayımıyla değer ayrıca verilmiştir).
- **2:** Çöküş: medeniyet yok olur (`Civ.Alive` false olur) ya da medeniyet yaşarken başkentini kaybeder: bir önceki gün sonunda başkenti olan yerleşim (`Sim.Capital`, en kalabalık yerleşim) artık yaşamıyor ya da başka medeniyetin; medeniyetin o gün hâlâ yerleşimi vardır. Son yerleşimin düşmesi yok olma olarak bir kez sayılır.
- **4a:** Her on yılda doğan bütün kahramanların (bütün dünyalar) doğuş seviyelerinin ortalaması; her on yıl ≤ 2 olmalı. Doğuş seviyesi kahraman `World.Heroes`'a girdiği anda okunur.
- **4b:** Dünyada koşu boyunca herhangi bir kahramanın ulaştığı en yüksek seviye ≥ 8 olan dünyaların payı.
- **4c:** Dünya başına koşu boyunca efsane olan (`Hero.Legend`) kahraman sayısının dünyalar arası medyanı.
- **4d:** Koşu boyunca doğan bütün kahramanlardan (bütün dünyalar) koşu sonunda ölü (`State == "dead"`) olanların payı; emekli olanlar ve diyarı terk edenler ölü sayılmaz.
- **5a:** İlk `--verify` seed'i aynı süreçte toplayıcı bağlanmadan yeniden koşulur; her yıl sonundaki kanonik dünya hash'i (golden testteki FNV-1a) toplayıcılı koşuyla karşılaştırılır. Bu, determinizmi ve toplayıcının simülasyonu değiştirmediğini birlikte denetler.
- **5b:** `Sim.Save(string)` ve `static Sim Sim.Load(string)` varsa ilk `--saveload` seed'i yıl ortasında (gün = yıl/2 × 120 + 37) kaydedilip yüklenir ve sonuna dek koşulur; yüklenen dünyanın hash'i ve sonraki yıl sonu hash'leri kesintisiz koşuyla karşılaştırılır.

- **6–7 (v3):** bkz. "v3: durum değişimi" bölümü. Pencere: ısınmadan sonra (koşunun son üçte ikisi); 6b bütün koşu. Büyük şehir = Şehir kademesi (`Sim.BIG_TIER`). El değiştirme = fetih ya da bölünme. Kuşatma = hedefin önünde ilk savaş ordusunun karargâh kurduğu günden (`Agent.Muster`) hücum muharebesine; karargâhsız hücum 1 gün. Uyarı = kuşatmanın (ya da savaşın) başından büyük şehrin el değiştirdiği güne. Savaş = `War.Since`'ten savaşın ilişkiden düştüğü güne.

## v3: durum değişimi

Yol haritası v3: dünya büyüyerek değil, durum değiştirerek yaşar. Bütün değerler dünya başına; hücre: dünyalar arası medyan (p10–p90). "100 günde": pencerede sayılan / pencerenin günü × 100; "yerleşim başına": yaşayan yerleşim-günlerine bölünür. Büyük şehir = Şehir kademesi (kademe 3, nüfus ≥ 100; histerezisle ≥ 85). Isınma: koşunun ilk üçte biri (2400 gün). Dünya hâlâ 8 kamptan başlayıp ilk on yıllarda büyüyor (dünya üretimi sonraki adım); bu yüzden bütün koşunun değerleri ayrıca verilmiştir.
†: gelecek zaman ölçeğine bağlı. Yol haritası v3'ün hedefleri yeni takvimdedir (1 yıl = 40 gün; eski 120 günlük yıl ≈ 30 gün): bugünkü ölçekte ölçülen oranlar ×4, süreler ÷4 ile yansıtılır (`--proj`). Yeni takvime geçince yansıtma 1 olur.

| Ölçü | bütün koşu (1–60. yıl) | ısınmadan sonra (21–60. yıl) | yeni takvimde (×4 / ÷4, ısınmadan sonra) |
|---|---|---|---|
| Yaşayan yerleşim (günlük ortalama) | 62,8 (47,1–68,4) | 76,2 (56,9–83,4) |  |
| Yaşayan yerleşim: en az (ortalamaya oranı) | %14 (%13–%15) | %77 (%74–%80) |  |
| Yaşayan yerleşim: en çok (ortalamaya oranı) | %139 (%130–%148) | %114 (%108–%122) |  |
| Yaşayan yerleşim: değişim katsayısı | %36 (%33–%37) | %9,5 (%7,6–%13) |  |
| Büyük şehir (günlük ortalama) | 5,01 (3,92–5,95) | 7,3 (5,76–8,7) |  |
| El değiştirme (fetih + bölünme), dünyada / 100 gün | 0,58 (0,18–0,92) | 0,77 (0,16–1,07) | 3,08 (0,63–4,29) |
| El değiştirme, yerleşim başına / 10 bin gün | 1,09 (0,32–1,42) | 1,1 (0,21–1,38) | 4,41 (0,85–5,51) |
| Durum değişimi (kuruluş hariç), yerleşim başına / 100 gün | 0,16 (0,1–0,19) | 0,15 (0,09–0,19) | 0,59 (0,34–0,75) |
| Durum değişimi, dünyada / 100 gün | 10,1 (5,18–12,3) | 11,4 (5,17–14,9) | 45,6 (20,7–59,5) |
| Orta halka (Köy/Kasaba) kaydı: yerleşim başına kaç günde bir | 1664 (1421–2161) | 1783 (1493–2349) | 446 (373–587) |
| Büyük şehir el değiştirdi, dünyada / 100 gün | 0,01 (0–0,04) | 0,02 (0–0,05) | 0,08 (0–0,21) |
| Büyük şehir el değiştirdi, şehir başına / 100 gün | 0 (0–0,01) | 0 (0–0,01) | 0,01 (0–0,03) |
| Büyük şehre hücum, dünyada / 100 gün | 0,08 (0–0,21) | 0,13 (0–0,31) | 0,5 (0–1,25) |
| Büyük şehir yağmalandı ama tutulmadı, dünyada / 100 gün | 0,05 (0–0,12) | 0,07 (0–0,18) | 0,29 (0–0,71) |

Durum değişimi türleri (yerleşim başına 10 bin günde; bölünme dışında olayın kademesi olaydan önceki):

| Tür | bütün koşu (1–60. yıl) | ısınmadan sonra (21–60. yıl) |
|---|---|---|
| Fetih (`capture`) | 1,06 (0,32–1,41) | 1,07 (0,21–1,36) |
| Bölünme (`secede`) | 0,01 (0–0,04) | 0,01 (0–0,04) |
| Açlık başladı (`famine`) | 0 (0–1,04) | 0 (0–1,29) |
| Salgın başladı (`plague`) | 0,47 (0,31–0,65) | 0,49 (0,33–0,62) |
| Kuşatma (hücum) (`siege`) | 1,39 (0,35–1,89) | 1,52 (0,25–1,95) |
| Yakıldı / yandı (`burn`) | 5,56 (3,42–6,53) | 5,23 (3,18–6,65) |
| Kademe yükseldi (`tierUp`) | 4,73 (3,91–5,16) | 3,54 (2,56–4,13) |
| Kademe düştü (`tierDown`) | 2,45 (1,64–3,08) | 2,59 (1,56–3,21) |
| Terk (harabe) (`abandon`) | 0,1 (0,03–0,21) | 0,12 (0,03–0,25) |
| Harabeye yeniden yerleşim (`resettle`) | 0,04 (0,01–0,14) | 0,05 (0,01–0,18) |
| Kuruluş (durum değişimi sayılmaz) (`found`) | 1,78 (1,69–1,91) | 0,84 (0,69–0,97) |

Süreler (gün; bütün dünyalar havuzlanmış; savaş: ilandan ilişkiden düştüğü güne, koşu sonunda süren savaşlar hariç; kuşatma: hedefin önündeki ilk karargâhtan hücuma, karargâhsız hücum 1 gün; uyarı: büyük şehrin düşüşünden geriye):

| Süre | Pencere | n | p10 | medyan | p90 | ortalama | en çok | medyan, yeni takvimde (÷4) |
|---|---|---|---|---|---|---|---|---|
| Savaş (hepsi) | bütün koşu (1–60. yıl) | 893 | 20 | 40 | 140 | 62,7 | 370 | 10 |
| Savaş: sıradan | bütün koşu (1–60. yıl) | 508 | 20 | 40 | 150 | 65,8 | 310 | 10 |
| Savaş: tarihî hak | bütün koşu (1–60. yıl) | 100 | 20 | 40 | 101 | 53,9 | 310 | 10 |
| Savaş: Kutsal Sefer | bütün koşu (1–60. yıl) | 42 | 25,5 | 80 | 259 | 116 | 310 | 20 |
| Savaş: savunma paktı | bütün koşu (1–60. yıl) | 136 | 20 | 35 | 65 | 45,1 | 370 | 8,75 |
| Savaş: müttefik çağrısı | bütün koşu (1–60. yıl) | 107 | 20 | 40 | 130 | 58,1 | 200 | 10 |
| Kuşatma: büyük şehir | bütün koşu (1–60. yıl) | 114 | 21 | 21 | 21 | 20,6 | 25 | 5,25 |
| Kuşatma: diğer yerleşimler | bütün koşu (1–60. yıl) | 773 | 1 | 1 | 21 | 5,4 | 30 | 0,25 |
| Uyarı: kuşatmanın başı → büyük şehrin düşüşü | bütün koşu (1–60. yıl) | 20 | 7,7 | 20 | 20 | 17,8 | 24 | 5 |
| Uyarı: savaşın başı → büyük şehrin düşüşü | bütün koşu (1–60. yıl) | 20 | 33,9 | 55 | 76,6 | 61,6 | 192 | 13,8 |
| Savaş (hepsi) | ısınmadan sonra (21–60. yıl) | 729 | 20 | 40 | 140 | 64,9 | 370 | 10 |
| Savaş: sıradan | ısınmadan sonra (21–60. yıl) | 389 | 20 | 40 | 150 | 68,3 | 310 | 10 |
| Savaş: tarihî hak | ısınmadan sonra (21–60. yıl) | 87 | 20 | 40 | 104 | 54,4 | 310 | 10 |
| Savaş: Kutsal Sefer | ısınmadan sonra (21–60. yıl) | 35 | 30 | 110 | 290 | 131 | 310 | 27,5 |
| Savaş: savunma paktı | ısınmadan sonra (21–60. yıl) | 123 | 20 | 40 | 70 | 46,8 | 370 | 10 |
| Savaş: müttefik çağrısı | ısınmadan sonra (21–60. yıl) | 95 | 20 | 40 | 130 | 59,6 | 200 | 10 |
| Kuşatma: büyük şehir | ısınmadan sonra (21–60. yıl) | 112 | 21 | 21 | 21 | 20,6 | 25 | 5,25 |
| Kuşatma: diğer yerleşimler | ısınmadan sonra (21–60. yıl) | 634 | 1 | 1 | 21 | 5,78 | 29 | 0,25 |
| Uyarı: kuşatmanın başı → büyük şehrin düşüşü | ısınmadan sonra (21–60. yıl) | 19 | 7,4 | 20 | 20 | 17,8 | 24 | 5 |
| Uyarı: savaşın başı → büyük şehrin düşüşü | ısınmadan sonra (21–60. yıl) | 19 | 33,8 | 52 | 79,2 | 61,6 | 192 | 13 |

Koşu sonunda süren savaş: 13 (sürelere girmedi).

Kuşatma sonuçları (bütün koşu): büyük şehir: 114 hücum: 20 el değiştirdi, 66 yağmalandı (tutulmadı), 28 püskürtüldü; diğer: 773 hücum: 651 el değiştirdi, 104 yağmalandı (tutulmadı), 18 püskürtüldü.

Büyük şehrin el değiştirmesi (bütün koşu, 20; ilk 60):

| Seed | Gün (yıl) | Şehir | Nasıl | Önceki → yeni sahip | Kuşatma → düşüş | Savaş → düşüş |
|---|---|---|---|---|---|---|
| 1 | 4673 (39) | Sisliyamaç | fetih | Kanlıdiş Kabileleri → Rüzgâr Manastırı | 20 gün | 33 gün |
| 4 | 2340 (20) | Altınkapı | fetih | Güneştacı Krallığı → Kanlıdiş Kabileleri | 17 gün | 60 gün |
| 4 | 2800 (24) | Kalkanova | fetih | Güneştacı Krallığı → Tatlıçayır Loncası | 1 gün | 40 gün |
| 4 | 4300 (36) | Altınkapı | fetih | Rüzgâr Manastırı → Pulzırh Lejyonu | 20 gün | 100 gün |
| 4 | 5790 (49) | Yıldıztepe | fetih | Kanlıdiş Kabileleri → Örsyürek Tapınak Klanı | 20 gün | 50 gün |
| 5 | 3568 (30) | Gölpınar | fetih | Kızılboynuz Soyu → Kanlıdiş Kabileleri | 20 gün | 48 gün |
| 9 | 6708 (56) | Közsaray | fetih | Pulzırh Lejyonu → Kızılboynuz Soyu | 20 gün | 58 gün |
| 10 | 3304 (28) | Fıçıköy | fetih | Tatlıçayır Loncası → Karaörs Derinlikleri | 20 gün | 34 gün |
| 10 | 3606 (31) | Fıçıköy | fetih | Karaörs Derinlikleri → Kanlıdiş Kabileleri | 20 gün | 26 gün |
| 11 | 4530 (38) | Alazvadi | fetih | Kızılboynuz Soyu → Rüzgâr Manastırı | 8 gün | 60 gün |
| 12 | 6664 (56) | Karagöl | fetih | Yeşilyaprak Çemberi → Karaörs Derinlikleri | 20 gün | 74 gün |
| 12 | 6900 (58) | Karagöl | fetih | Karaörs Derinlikleri → Yeşilyaprak Çemberi | 5 gün | 50 gün |
| 13 | 4613 (39) | Sessizkoru | fetih | Kanlıdiş Kabileleri → Pulzırh Lejyonu | 20 gün | 63 gün |
| 13 | 5169 (44) | Kırkkapı | fetih | Tatlıçayır Loncası → Kanlıdiş Kabileleri | 20 gün | 69 gün |
| 13 | 5230 (44) | Kurtkent | fetih | Kanlıdiş Kabileleri → Pulzırh Lejyonu | 20 gün | 50 gün |
| 13 | 6275 (53) | Karlıtepe | fetih | Kanlıdiş Kabileleri → Pulzırh Lejyonu | 24 gün | 35 gün |
| 13 | 6583 (55) | Bozkent | fetih | Kanlıdiş Kabileleri → Pulzırh Lejyonu | 20 gün | 63 gün |
| 13 | 7172 (60) | Kızılkül | fetih | Kızılboynuz Soyu → Pulzırh Lejyonu | 20 gün | 192 gün |
| 15 | 3732 (32) | Tamburlu | fetih | Lirsesi Şehirleri → Kanlıdiş Kabileleri | 20 gün | 52 gün |
| 16 | 6284 (53) | Yelköy | fetih | Kanlıdiş Kabileleri → Pulzırh Lejyonu | 20 gün | 74 gün |

## Eski analizdeki sorunlar

Eski analiz: TS v0.23, 12 seed × 30 yıl ve 3 seed × 60 yıl (Proje: `analiz-5-ajan-oneriler.md`). "Sürüyor mu" kaba bir eşiktir: araştırma ağacı Faz 1b-3'te kaldırıldı; 30. yılda tam 5 kara yerleşimli medeniyet ≥ %50; kamp (30. yıl) < 0,75 × en yüksek yıl; altın (30. yıl) ≥ 10 × altın (1. yıl); boştaki iş gücü (30. yıl) ≥ %30; büyük olay (30. yıl) ≤ 0,6 × en yüksek yıl; 25. yıldan sonra doğanların ≥ %50'si Sv5+; hiç başkent kaybı yok.

| Bulgu | Eski analiz | Bu ölçüm | Sürüyor mu? |
|---|---|---|---|
| Araştırma ağacı erken bitiyor | ~19. yılda bitiyor; 30. yılda medeniyetlerin %98'i bitirmiş | ağaç ve çağlar kaldırıldı (Faz 1b-3); başlangıç medeniyetlerinin ilk kasabası medyan 9,5. yılda (122/128), ilk şehri 22. yılda (119/128); 30. yılda başkent kademesi ortalaması 2,67 | hayır |
| Medeniyetler 5 yerleşimde takılıyor | 98 medeniyetin 74'ü (%76) tam 5 yerleşimde | tam 5 kara yerleşimli medeniyet payı 30. yılda %6,3, 60. yılda %5,4 (denizaşırı koloniler dâhil 30. yılda tam 5: %3,9, 5+: %96); medeniyet başına 8,39 yerleşim (30. yıl) | hayır |
| Kamp sayısı düşüyor | 6,8'den 3,5'e iniyor | 7 (1. yıl) → en yüksek 10 (20. yıl) → 9 (30. yıl) → 9 (60. yıl); yıl sonu, yıllık dünya medyanı | hayır |
| Altın birikiyor | altın medyanı 78'den 6.503'e çıkıyor | 8,33 (1. yıl) → 427 (30. yıl) → 676 (60. yıl) | evet |
| İş gücü boşta | iş gücünün %43'ü boşta | 30. yılda %21, 60. yılda %17 (işe yerleşemeyen `zanaatçı` / bütün iş gücü, askerler dâhil; yıl içi ortalama) | hayır |
| Büyük olaylar seyreliyor | yıllık büyük olay 59'dan 28'e düşüyor | en yüksek 59,5 (58. yıl) → 46 (30. yıl) → 44,5 (60. yıl), yıllık dünya medyanı | hayır |
| Doğuş seviyesi şişiyor | 24. yıldan sonra herkes Sv5 doğuyor; efsane mekaniği ölü | 25–60. yıllarda Sv5+ doğanların payı %0; on yıllık doğuş seviyesi ortalaması 1,3 · 1,3 · 1,29 · 1,35 · 1,38 · 1,38 | hayır |
| Başkent düşmüyor | başkent fethedilemiyor (agents.ts:673) | 16 dünyada 29 başkent kaybı, 11 yok olma; 681 yerleşim fethi | hayır |

## On yıllık özet

Hücre: dünyalar arası medyan (p10–p90). Her dünyada on yılın yıllık değerlerinin ortalaması alınır: akış ölçülerinde yıllık ortalama, stok ölçülerinde yıl sonu değerlerinin ortalaması. Yüzdeler 0–1 paylardır.

| Ölçü | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| **Medeniyet** | | | | | | |
| Yaşayan medeniyet | 8 (6,9–9) | 8 (7–9) | 8 (7–9,05) | 8 (7–9) | 8 (7–9) | 8 (7–9,3) |
| Yeni medeniyet (yeniden doğan) | 0 | 0 | 0 (0–0,1) | 0 (0–0,05) | 0 (0–0,1) | 0 (0–0,05) |
| Yok olan medeniyet | 0 (0–0,1) | 0 | 0 (0–0,05) | 0 | 0 | 0 (0–0,05) |
| Başkent kaybı (medeniyet yaşarken) | 0 | 0 (0–0,05) | 0 (0–0,1) | 0 (0–0,2) | 0 (0–0,1) | 0 (0–0,1) |
| Çöküş (yok olma + başkent kaybı) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,25) | 0 (0–0,1) | 0 (0–0,15) |
| Yaşayan yerleşim | 21,7 (17,1–23,7) | 49,9 (38–56,3) | 63,5 (50,5–71,8) | 73,5 (55,8–82,4) | 80,5 (59–89) | 84,4 (62,6–94,5) |
| Medeniyet başına yerleşim | 2,63 (2,47–2,85) | 6,17 (5,64–6,54) | 7,83 (7,21–8,44) | 9,05 (8,12–9,55) | 9,4 (8,32–10,4) | 9,9 (8,59–10,9) |
| 5+ kara yerleşimli medeniyet payı | %13 (%11–%17) | %88 (%77–%93) | %95 (%86–%100) | %97 (%87–%100) | %93 (%86–%100) | %94 (%83–%100) |
| Kurulan yerleşim | 2,9 (2,2–3,3) | 1,95 (1,4–2,25) | 1,2 (0,95–1,65) | 0,6 (0,3–1,2) | 0,55 (0,4–0,9) | 0,5 (0,3–0,85) |
| Fethedilen yerleşim | 0,2 (0–0,3) | 0,65 (0–1,35) | 0,7 (0,2–1,35) | 0,9 (0,25–1,4) | 1,05 (0,05–1,35) | 0,95 (0–1,4) |
| Terk edilen yerleşim | 0 (0–0,1) | 0 | 0,05 (0–0,15) | 0,05 (0–0,3) | 0,1 (0–0,3) | 0,1 (0–0,3) |
| Toplam nüfus | 387 (297–440) | 1584 (1170–1832) | 2456 (1935–2878) | 2968 (2579–3477) | 3339 (2894–3999) | 3626 (3047–4243) |
| Altın medyanı (medeniyetler) | 35,5 (23,8–47,8) | 205 (139–289) | 329 (246–481) | 544 (394–643) | 620 (454–839) | 645 (495–872) |
| Boştaki iş gücü payı | %1,2 (%0,3–%2,7) | %13 (%10–%16) | %21 (%16–%24) | %18 (%17–%21) | %18 (%16–%20) | %17 (%14–%21) |
| Bölünme (ayrılıp kurulan medeniyet) | 0 | 0 | 0 (0–0,1) | 0 (0–0,05) | 0 (0–0,1) | 0 (0–0,05) |
| En büyük medeniyetin yerleşimi | 3,6 (3,3–3,9) | 8,15 (7,65–9,1) | 10,5 (9,05–13,4) | 13 (9,85–17,1) | 16,4 (10,3–19,7) | 18 (10,6–22,1) |
| **Olaylar** | | | | | | |
| Olay | 98,7 (77,8–118) | 158 (134–198) | 161 (133–195) | 159 (107–189) | 150 (102–191) | 162 (125–232) |
| Büyük olay | 30,7 (24,6–37,8) | 46,7 (39,9–56,9) | 47,7 (38–60,2) | 46,7 (30,8–57,2) | 43,9 (29,1–60,1) | 50,3 (33,9–81) |
| **Savaş** | | | | | | |
| Muharebe | 5 (4,1–5,3) | 7,55 (6,9–9,3) | 9 (6,2–10,8) | 9,05 (5,95–11,2) | 8,25 (5,35–10,5) | 9,05 (6,25–12) |
| Başlayan savaş | 0,2 (0–0,4) | 0,8 (0,1–1,7) | 0,75 (0,25–2) | 1 (0,3–1,9) | 1,35 (0,05–2,05) | 1,35 (0–1,95) |
| Süren savaş (yıl sonu) | 0,05 (0–0,2) | 0,3 (0,05–0,75) | 0,65 (0,05–1,05) | 0,7 (0,15–1,05) | 0,7 (0–1,25) | 0,7 (0–1,3) |
| Yıl içinde süren savaş | 0,25 (0–0,5) | 1,1 (0,2–2,15) | 1,4 (0,35–2,85) | 1,65 (0,5–3,15) | 2,05 (0,05–3,4) | 2,1 (0–3,2) |
| Yağma akını (medeniyet) | 0 (0–0,35) | 0,4 (0–2,05) | 0,75 (0–2,1) | 1 (0–2,55) | 1,3 (0–2,2) | 1 (0–2,45) |
| Tarihî hak savaşı | 0 (0–0,05) | 0 (0–0,2) | 0,1 (0–0,35) | 0,1 (0–0,45) | 0,1 (0–0,3) | 0,1 (0–0,25) |
| Pakt gereği savaş | 0 | 0 (0–0,25) | 0,1 (0–0,6) | 0,1 (0–0,4) | 0,1 (0–0,45) | 0,2 (0–0,5) |
| Kutsal Sefer çağrısı | 0 | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,05) | 0 (0–0,1) |
| İhanet (pakt çiğnendi) | 0 | 0 (0–0,05) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) |
| Savunma paktı (yıl sonu) | 0 (0–0,15) | 0,65 (0–1,35) | 1 (0–1,9) | 1,05 (0–2) | 1 (0,1–2,15) | 1,15 (0,4–2,2) |
| **Canavarlar** | | | | | | |
| Yaşayan kamp (yıl sonu) | 8,4 (8–8,6) | 8,8 (8,15–9,5) | 9,25 (9–9,85) | 9,3 (8,85–10,2) | 9,45 (8,45–10,4) | 9,1 (7,8–10,3) |
| Yaşayan kamp (yıl ort.) | 8,12 (7,79–8,34) | 8,58 (8,01–9,41) | 9,11 (8,94–9,97) | 9,37 (8,79–10,2) | 9,32 (8,45–10,3) | 9,12 (8,11–10,4) |
| Doğan kamp | 0,85 (0,7–1,1) | 1,75 (1,35–2,7) | 2 (1,15–2,95) | 1,65 (1,1–2,1) | 1,5 (0,7–2,45) | 1,4 (0,85–3,05) |
| Temizlenen kamp | 0,25 (0,1–0,55) | 1,75 (1,35–2,7) | 2 (1,25–2,8) | 1,7 (1,05–2,2) | 1,45 (0,6–2,35) | 1,5 (0,9–3,2) |
| Canavar baskını | 4,05 (3,6–4,35) | 3,85 (3,15–4,85) | 4,2 (3,05–5,05) | 4,15 (3,1–5,05) | 3,55 (3,05–4,85) | 3,8 (2,8–5,15) |
| Yaşayan trol ini (yıl sonu) | 0 | 0,15 (0–0,65) | 0,65 (0,25–1,75) | 1,35 (0,75–2,45) | 1,7 (0,75–3) | 2 (0,6–2,95) |
| Yaşayan ejderha (yıl sonu) | 0 | 0,1 (0–0,3) | 1 (0,95–1) | 1 (0,85–1) | 1 (0,5–1) | 1 (0,35–1) |
| Ejderha akını | 0 | 0 (0–0,3) | 1 (0,75–1,2) | 0,8 (0,6–1,2) | 0,9 (0,2–1,2) | 0,85 (0,15–1,25) |
| Kriz (anlatıcı) | 0 (0–0,1) | 0,3 (0,1–0,5) | 0,25 (0–0,4) | 0,2 (0,1–0,55) | 0,4 (0,1–0,55) | 0,35 (0,15–0,55) |
| Rahatlama dönemi (anlatıcı) | 0,2 (0,15–0,3) | 0,1 (0–0,1) | 0,1 (0,05–0,2) | 0,1 (0–0,2) | 0,05 (0–0,1) | 0,05 (0–0,25) |
| **Kahramanlar** | | | | | | |
| Doğan kahraman | 2,85 (2,35–3,2) | 2,9 (2,2–3,4) | 3,35 (2,85–4,25) | 3,2 (2,25–3,95) | 3,25 (2,25–4,6) | 3,5 (2,2–5,4) |
| Ölen kahraman | 0,35 (0,1–0,55) | 1 (0,65–1,65) | 1,35 (0,9–2,15) | 1,35 (0,75–2,6) | 1,6 (0,65–2,95) | 2 (0,75–3,15) |
| Emekli olan kahraman | 0 | 0 | 0,1 (0–0,15) | 0,4 (0,2–0,7) | 0,9 (0,4–1,2) | 0,8 (0,5–1,3) |
| Diyarı terk eden kahraman | 0,1 (0–0,25) | 0,1 (0–0,3) | 0,15 (0–0,4) | 0,55 (0,15–1,05) | 0,6 (0,2–1,15) | 0,75 (0,3–1,35) |
| Ölümden dönen kahraman | 0 | 0 | 0 | 0 | 0 | 0 |
| Efsane olan kahraman | 0 | 0 (0–0,2) | 0,1 (0–0,35) | 0,1 (0–0,2) | 0,1 (0–0,35) | 0,2 (0,1–0,3) |
| Yaşayan kahraman (yıl sonu) | 10,5 (8,3–12,4) | 30,6 (28,3–39,7) | 47,1 (40,9–59,8) | 59,3 (51,5–71,6) | 64,1 (59,2–74,6) | 69,4 (62–80,7) |
| Doğuş seviyesi (ort.) | 1,34 (1,16–1,45) | 1,31 (1,18–1,47) | 1,26 (1,16–1,38) | 1,35 (1,23–1,44) | 1,37 (1,22–1,55) | 1,39 (1,28–1,55) |
| Ölüm seviyesi (ort.) | 1,17 (1–1,56) | 1,52 (1,23–1,82) | 2,47 (1,8–3,08) | 2,6 (2,11–3,88) | 2,82 (2,32–3,45) | 3,06 (2,37–3,89) |
| Yaşayan kahraman seviyesi (ort.) | 1,44 (1,2–1,55) | 2,26 (1,8–2,44) | 2,84 (2,41–3,19) | 3,35 (2,68–3,48) | 3,19 (2,84–3,69) | 3,25 (2,76–3,93) |
| En yüksek seviye (şimdiye dek) | 1,8 (1,7–2) | 4,3 (3,7–4,7) | 5,7 (4,85–6,85) | 7,4 (6–8,3) | 8 (6,3–9,25) | 8,6 (7–10) |
| **Han ve ticaret** | | | | | | |
| Ayakta han | 3,3 (2,55–4) | 4,3 (3,65–4,8) | 5 (4,55–6,2) | 5,05 (4,75–6,3) | 5 (5–6,45) | 5,2 (4,55–6,75) |
| Asılan ilan | 1,75 (1,5–2,1) | 2,25 (1,75–3,15) | 3,2 (2,3–4,2) | 2,95 (2,2–3,8) | 2,75 (1,9–4,95) | 2,8 (2–8,35) |
| Biten ilan | 0,2 (0,05–0,5) | 0,8 (0,55–1,2) | 0,55 (0,3–1,3) | 0,8 (0,5–1,25) | 0,8 (0,25–1,7) | 1 (0,3–2,95) |
| Ticaret seferi (kervan) | 14,7 (9,3–25,3) | 45,7 (33,4–64,1) | 57,3 (40,7–86,3) | 77,8 (44–108) | 82,8 (45,9–131) | 92,1 (52,6–141) |
| İkmal seferi | 0,15 (0–1,15) | 12,2 (6,2–20,4) | 22,7 (13,4–29,4) | 28,1 (16,7–37) | 30,4 (15,8–45,5) | 32 (13,3–45,4) |
| **Altın ve ambar** | | | | | | |
| Altın p90 (medeniyetler) | 139 (82,9–154) | 640 (502–755) | 972 (713–1216) | 1211 (985–1269) | 1335 (1075–1461) | 1390 (1180–1545) |
| Bakım gideri (altın; asker, kahraman, L2–L3) | 111 (80,1–160) | 819 (642–1039) | 1516 (1302–1792) | 2016 (1594–2248) | 2165 (1610–2339) | 2175 (1689–2551) |
| Kamu işlerine (imar) harcanan altın | 10,3 (0,47–22,8) | 705 (458–1169) | 1542 (949–2008) | 2243 (1530–2783) | 2697 (1843–3226) | 2877 (2122–3994) |
| Ambarla beslenen amele tayını (gıda) | 24,5 (13,8–78,7) | 1272 (791–1713) | 2696 (1953–3351) | 1957 (1488–2958) | 966 (432–2048) | 918 (271–1349) |
| Kamu işlerindeki (amele) iş gücü payı | %0,6 (%0,2–%1,1) | %13 (%11–%15) | %20 (%19–%22) | %20 (%19–%23) | %20 (%17–%22) | %19 (%18–%23) |
| İmar ortalaması (köy+, 0–100) | 0,25 (0,01–0,41) | 10,9 (6,92–16,3) | 24,1 (17,4–28,8) | 26 (21,6–31,1) | 27 (21,6–32,2) | 27,3 (18,7–32,5) |
| Canavar baskınında yitirilen altın | 9,95 (3–17,8) | 6,05 (1,75–20,5) | 7,4 (1,95–13,2) | 8,85 (2,5–25,1) | 3,8 (2,6–16,9) | 7,9 (2,3–14,6) |
| Ejderhaya giden altın (haraç + akın) | 0 | 0 (0–27,2) | 290 (187–393) | 431 (216–726) | 508 (43,5–888) | 549 (129–952) |
| Hazinesi boş medeniyet payı | %0 | %0 | %0 | %0 | %0 | %0 |
| Kent tüketiminde yokluk payı (köy+; ekmek, bira ya da alet) | %0,8 (%0,3–%1,8) | %1,6 (%0,4–%4,2) | %9,4 (%2,5–%23) | %32 (%12–%44) | %44 (%30–%56) | %42 (%31–%51) |
| Ekmek ya da bira yokluğu payı (köy+) | %0,8 (%0,3–%1,6) | %0,9 (%0–%3,6) | %2,4 (%0,5–%8,1) | %4,2 (%1,1–%10) | %7,3 (%1,2–%13) | %5 (%2,5–%11) |
| Kıtlık (büyük olay) | 0 (0–0,05) | 0 (0–0,05) | 0 | 0 | 0 (0–0,1) | 0 (0–0,2) |
| Açlıktan ölen | 0 (0–0,05) | 0 | 0 | 0 | 0 | 0 (0–1,05) |
| Kıtlık yardımı (sevkiyat) | 0 | 0 (0–0,05) | 0 | 0 | 0 (0–0,4) | 0 (0–0,8) |
| Kıtlıkta yüz çeviren | 0 | 0 (0–0,05) | 0 | 0 | 0 | 0 (0–0,45) |
| Kıtlık akını | 0 | 0 (0–0,05) | 0 | 0 | 0 (0–0,1) | 0 (0–0,15) |
| Ambarın yettiği gün (medeniyet medyanı) | 81,4 (73–95,1) | 226 (155–271) | 270 (231–297) | 233 (183–290) | 183 (145–239) | 176 (129–244) |
| **Yerleşim kademesi** | | | | | | |
| Ortalama yerleşim kademesi | 0,55 (0,51–0,59) | 1,02 (0,96–1,08) | 1,17 (1,11–1,24) | 1,23 (1,17–1,3) | 1,27 (1,2–1,38) | 1,27 (1,23–1,4) |
| Köy+ yerleşim | 12,4 (9,05–13,9) | 35,3 (27,1–39,4) | 47,6 (37,8–52,2) | 56,7 (41,2–63,1) | 62,9 (43,4–70,1) | 66,7 (47,2–79) |
| Kasaba+ yerleşim | 1,5 (0,8–2,2) | 15,2 (11,3–17,5) | 22,4 (17,1–26,5) | 26,8 (20,1–31,3) | 29,9 (22,6–36,4) | 30,4 (22,5–38,8) |
| Şehir | 0 | 0,9 (0,2–1,45) | 4,95 (3,8–6,1) | 7,1 (5,65–8,45) | 8,6 (6,5–10) | 8,85 (6,85–11,4) |
| Ortalama başkent kademesi | 0,92 (0,79–1,01) | 2 (1,77–2,07) | 2,53 (2,39–2,69) | 2,67 (2,46–2,85) | 2,69 (2,61–2,85) | 2,73 (2,59–2,92) |
| Kademe değişimi (yerleşim, yıl içinde) | 4,05 (3,15–5,1) | 5,4 (3,65–7,2) | 5,95 (3,6–7,8) | 6,05 (2,55–7,35) | 4,5 (2,7–7,65) | 5 (1,9–7,2) |
| **Deniz** | | | | | | |
| Liman (tersane) | 2,05 (0,55–3,2) | 9 (4,25–13,5) | 12,6 (9,65–16,1) | 17,1 (10,3–20,7) | 17,5 (10,4–23,6) | 19,2 (11–25,9) |
| Gemi (koga/tekne) | 2,65 (0,7–5,25) | 17,9 (8,45–29,6) | 32,3 (24,8–38,4) | 40,7 (28,5–51,5) | 50,3 (29,6–59,1) | 58,1 (31–65,2) |
| Kadırga | 0 | 1,3 (0–3,05) | 10,4 (5,4–13,5) | 14,1 (8,85–21,2) | 15,8 (10,7–24,1) | 14,9 (11,2–24) |
| Denizaşırı yerleşim | 0,1 (0–0,6) | 4,35 (2,3–7,6) | 7 (3,5–9,35) | 7,5 (3,5–12,1) | 7,5 (3,5–13,6) | 7,5 (3,5–14,8) |
| Deniz ticaret yolu (yıl sonu) | 0,1 (0–0,6) | 4,5 (2,4–10,5) | 9,6 (5–14) | 10,6 (5–17,3) | 12,1 (5,2–21,2) | 13,9 (5,5–24) |
| Deniz seferi (ticaret) | 0 | 1,45 (0–5,35) | 6,1 (1,9–11,8) | 11 (4,25–14,8) | 14,7 (5,25–22,5) | 19 (6,55–30,4) |
| **v3: durum değişimi** | | | | | | |
| Yaşayan yerleşim (yıl ort.) | 20,2 (15,9–22) | 48,9 (37,2–55,2) | 62,8 (50–71,2) | 73,1 (55,6–82) | 80,4 (58,8–88,6) | 84,3 (62,4–94,3) |
| Büyük şehir (Şehir kademesi, yıl ort.) | 0 | 0,76 (0,13–1,19) | 4,75 (3,55–6,17) | 7,15 (5,49–8,33) | 8,55 (6,42–10) | 8,97 (6,85–11,1) |
| El değiştiren yerleşim (fetih + bölünme) | 0,2 (0–0,3) | 0,65 (0–1,35) | 0,7 (0,2–1,35) | 0,9 (0,25–1,4) | 1,05 (0,05–1,35) | 0,95 (0–1,4) |
| El değiştiren büyük şehir | 0 | 0 | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,05) | 0 (0–0,15) |
| Büyük şehre hücum (kuşatma muharebesi) | 0 | 0 | 0,1 (0–0,45) | 0,15 (0–0,45) | 0,1 (0–0,4) | 0,1 (0–0,45) |
| Büyük şehir yağmalandı, tutulmadı | 0 | 0 | 0,1 (0–0,15) | 0,1 (0–0,3) | 0,1 (0–0,25) | 0,1 (0–0,3) |
| Yerleşim durum değişimi (kuruluş hariç hepsi) | 5,75 (4,9–7,35) | 9,5 (5,75–15,3) | 11,9 (7,35–17,2) | 14,3 (5,9–18,4) | 13,9 (6,15–18,2) | 13,2 (4,75–19,7) |
| Orta halka kaydı (Köy/Kasaba: kademe değişimi ya da terk) | 1,2 (0,8–1,65) | 2,85 (1,7–4,1) | 3,5 (2,3–4,65) | 3,65 (1,7–4,7) | 2,8 (1,7–4,9) | 3,15 (1,1–4,75) |
| Açlık başlayan yerleşim | 0 (0–0,05) | 0 (0–0,3) | 0 | 0 | 0 (0–1,1) | 0 (0–2,4) |
| Salgın başlayan yerleşim | 0 | 0,2 (0–0,65) | 0,4 (0,1–0,75) | 0,3 (0,1–0,55) | 0,4 (0,25–1) | 0,45 (0,1–0,75) |
| Yakılan/yanan yerleşim | 1,45 (1–2,15) | 2,35 (1,5–4,9) | 4 (2,6–6,05) | 4,65 (2,2–7,05) | 4,8 (1,95–6,9) | 4,95 (2,05–6,6) |
| Harabeye yeniden yerleşim | 0 | 0 | 0 (0–0,1) | 0 (0–0,15) | 0,05 (0–0,3) | 0,1 (0–0,15) |

## Kahraman seviyeleri

### Doğuş seviyesi (bütün dünyalar, on yıl içinde doğanlar)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 |
|---|---|---|---|---|---|
| 1–10 | 449 | 1,3 | %70 | %30 | %0 |
| 11–20 | 456 | 1,3 | %70 | %30 | %0 |
| 21–30 | 552 | 1,29 | %71 | %29 | %0 |
| 31–40 | 510 | 1,35 | %66 | %33 | %1,2 |
| 41–50 | 539 | 1,38 | %63 | %35 | %1,9 |
| 51–60 | 609 | 1,38 | %65 | %32 | %3,4 |

### Ölüm seviyesi (bütün dünyalar, on yıl içinde ölenler)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 | Sv4 | Sv5 | Sv6 | Sv7 | Sv8 | Sv9 |
|---|---|---|---|---|---|---|---|---|---|---|---|
| 1–10 | 57 | 1,3 | %72 | %26 | %1,8 | %0 | %0 | %0 | %0 | %0 | %0 |
| 11–20 | 179 | 1,56 | %61 | %26 | %10 | %3,4 | %0 | %0 | %0 | %0 | %0 |
| 21–30 | 227 | 2,52 | %24 | %31 | %26 | %9,3 | %6,6 | %1,8 | %0,9 | %0 | %0 |
| 31–40 | 248 | 2,82 | %19 | %29 | %29 | %8,5 | %8,5 | %4,4 | %0,4 | %1,6 | %0 |
| 41–50 | 266 | 2,88 | %19 | %28 | %21 | %20 | %7,1 | %2,3 | %0,8 | %1,9 | %0 |
| 51–60 | 324 | 3,01 | %15 | %29 | %29 | %10 | %7,1 | %3,7 | %2,2 | %2,5 | %0,6 |

### Yaşayan kahramanların seviyesi (bütün dünyalar, on yılın son yılının sonunda)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 | Sv4 | Sv5 | Sv6 | Sv7 | Sv8 | Sv9 | Sv10 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 10 | 370 | 1,59 | %51 | %39 | %9,7 | %0,3 | %0 | %0 | %0 | %0 | %0 | %0 |
| 20 | 632 | 2,64 | %18 | %32 | %28 | %15 | %6,8 | %0,8 | %0 | %0 | %0 | %0 |
| 30 | 919 | 2,98 | %18 | %30 | %17 | %15 | %12 | %4,5 | %2,2 | %0,5 | %0 | %0 |
| 40 | 1017 | 3,24 | %16 | %26 | %22 | %13 | %10 | %7 | %4,1 | %1,8 | %0,5 | %0 |
| 50 | 1069 | 3,24 | %15 | %29 | %20 | %13 | %10 | %5,1 | %3,8 | %2,7 | %0,7 | %0,1 |
| 60 | 1158 | 3,4 | %15 | %26 | %23 | %13 | %8,9 | %5,6 | %4 | %3,9 | %1,4 | %0,5 |

### Ölüm nedenleri (bütün dünyalar)

| Neden | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 | Toplam |
|---|---|---|---|---|---|---|---|
| Kuşatma | 0 | 25 | 95 | 112 | 99 | 124 | 455 (%35) |
| Kamp saldırısı | 29 | 122 | 38 | 61 | 78 | 65 | 393 (%30) |
| Ejderha | 0 | 1 | 44 | 36 | 21 | 38 | 140 (%11) |
| Trol | 0 | 6 | 23 | 10 | 16 | 24 | 79 (%6,1) |
| Bilinmiyor | 0 | 0 | 0 | 3 | 21 | 53 | 77 (%5,9) |
| Han baskını (canavar) | 16 | 9 | 8 | 7 | 8 | 4 | 52 (%4) |
| Kervan soygunu | 1 | 4 | 4 | 7 | 10 | 7 | 33 (%2,5) |
| Yol pususu | 0 | 3 | 6 | 3 | 6 | 3 | 21 (%1,6) |
| Yağma akını | 1 | 4 | 7 | 4 | 0 | 0 | 16 (%1,2) |
| Düello | 0 | 0 | 1 | 3 | 4 | 5 | 13 (%1) |
| Yerleşim baskını (canavar) | 9 | 3 | 0 | 0 | 0 | 0 | 12 (%0,9) |
| Han baskını (medeniyet) | 1 | 2 | 1 | 2 | 3 | 1 | 10 (%0,8) |

Neden, ölümün kaydedildiği andaki son muharebenin türünden (başlık ve taraflar) ya da suikast olayından çıkarılır.

## Olay türleri

Dünya başına yıllık olay sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Kahraman (`hero`) | 10,9 (9–15,4) | 30,2 (21,4–35,1) | 43 (26–62,9) | 49 (35,2–69,6) | 55,5 (40,5–73,8) | 68,8 (59–102) |
| İnşaat (`build`) | 37,5 (28,6–51) | 70,4 (49,9–80,5) | 46 (36,9–55,1) | 23,3 (16,9–43,3) | 15,5 (12,5–26,3) | 14,5 (8,4–21,2) |
| Göç (`migration`) | 2,2 (1,3–3,5) | 7,8 (3,8–13,8) | 15,2 (9,4–22,9) | 17,2 (8,05–23,1) | 15,4 (7,85–23,1) | 16,8 (9,25–25,4) |
| Sefer/ilan (`quest`) | 3,15 (1,85–4,7) | 9,35 (5,95–12,5) | 9,7 (6,6–13,5) | 10,3 (7,5–15,7) | 11,1 (4,5–20,7) | 14,5 (7–34,4) |
| Savaş (`war`) | 1,25 (0–2,2) | 4,6 (0,3–12,2) | 7,8 (1,5–17,6) | 9,9 (1,6–17,6) | 11,5 (0,25–16,3) | 9,35 (0–16) |
| Kamp (`lair`) | 3 (2,4–3,6) | 5,65 (3,8–8,05) | 6,15 (4,05–8,05) | 5,25 (2,75–6,9) | 4,15 (1,45–6,85) | 4,05 (2,1–9,35) |
| Sınıf (`class`) | 3,4 (2,7–4,35) | 4,05 (3,2–5,2) | 4,05 (3,2–5,5) | 3,6 (3,05–5,05) | 3,65 (2,9–5,05) | 3,45 (2,65–5) |
| Deniz (`sea`) | 1,7 (0,45–5,75) | 4,3 (2,95–6,45) | 5,75 (2,35–8,85) | 3,1 (0,95–6,55) | 3,2 (1,55–6,4) | 3,15 (0,65–6,4) |
| Baskın (`raid`) | 4,15 (3,6–4,4) | 4 (3,15–5,15) | 3,3 (2,4–4,15) | 3,3 (2,45–4,1) | 3,1 (2,45–3,95) | 3,2 (2,05–3,9) |
| Büyüme (`growth`) | 5,5 (4,1–6,2) | 4,6 (3,8–5,25) | 2,65 (2,2–3,25) | 2,35 (1,05–2,85) | 1,45 (0,9–2,25) | 1,35 (0,6–2,15) |
| Ekonomi (`economy`) | 0,2 (0,1–0,4) | 2,65 (1,3–3,5) | 3,45 (2,05–4,45) | 4,1 (2,4–5,25) | 3,75 (1,9–5,4) | 3,5 (2–4,4) |
| Han (`inn`) | 2,7 (2,3–3,5) | 3,2 (2,55–4,15) | 2,55 (1,85–4,3) | 2,5 (1,3–3,65) | 2,1 (1,35–2,9) | 2,5 (1,45–3,5) |
| Yerleşim (`settle`) | 5,75 (4,2–6,75) | 3,45 (2,65–3,9) | 2,25 (1,9–2,7) | 1,25 (0,55–2,4) | 1,15 (0,7–1,5) | 1 (0,4–1,65) |
| Keşif (`discover`) | 7,65 (5,7–9,05) | 2,45 (0,95–3,15) | 1,45 (0,75–2,55) | 1,1 (0,45–1,55) | 0,7 (0,1–1,7) | 0,65 (0,2–1,85) |
| Ölüm/terk (`death`) | 0,4 (0,15–0,6) | 1 (0,7–1,65) | 1,35 (0,95–2,2) | 1,45 (0,85–2,65) | 1,65 (0,8–3,1) | 2,05 (0,85–3,4) |
| Ejderha (`dragon`) | 0 | 0,1 (0–0,65) | 2 (1,8–2,3) | 1,8 (1,6–2,2) | 1,9 (0,7–2,2) | 1,85 (0,65–2,25) |
| epitaph | 0,35 (0,1–0,55) | 1 (0,65–1,65) | 1,35 (0,9–2,15) | 1,35 (0,75–2,6) | 1,6 (0,65–2,95) | 2 (0,75–3,15) |
| Ticaret (`trade`) | 2,15 (1,05–3,15) | 1,1 (0,6–2,2) | 0,9 (0,35–2,2) | 0,8 (0–2,55) | 0,8 (0,25–2,45) | 1 (0–1,7) |
| Dünya (`world`) | 0,05 (0–0,3) | 0,75 (0,45–1,7) | 1,3 (0,9–2,15) | 1,15 (0,55–2,05) | 1,55 (1–2,95) | 1,6 (0,8–2,4) |
| Diplomasi (`diplomacy`) | 0,75 (0,3–1,3) | 0,45 (0,2–1,15) | 0,35 (0,1–1,1) | 0,25 (0–0,75) | 0,4 (0,05–1,35) | 0,35 (0–1,7) |
| Gerginlik (`tension`) | 0,7 (0,4–1,25) | 0,35 (0,1–1,2) | 0,3 (0,1–0,9) | 0,25 (0–0,75) | 0,35 (0,1–0,95) | 0,3 (0–1,1) |
| Temas (`contact`) | 1,15 (0,5–1,65) | 0,3 (0,15–0,7) | 0,2 (0,1–0,4) | 0,05 (0–0,25) | 0,1 (0–0,25) | 0 (0–0,3) |
| Kriz (anlatıcı) (`crisis`) | 0 (0–0,1) | 0,3 (0,1–0,5) | 0,25 (0–0,4) | 0,2 (0,1–0,55) | 0,4 (0,1–0,55) | 0,35 (0,15–0,55) |
| Rahatlama (anlatıcı) (`relief`) | 0,2 (0,15–0,3) | 0,1 (0–0,1) | 0,1 (0,05–0,2) | 0,1 (0–0,2) | 0 (0–0,1) | 0,05 (0–0,25) |

## Büyük olay türleri

Dünya başına yıllık büyük olay sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Kahraman (`hero`) | 2 (1,35–3,1) | 5,3 (3,7–6,1) | 6,75 (5,45–11,2) | 7,75 (5,3–11,2) | 8,15 (6,1–11,6) | 10,7 (7,05–14,5) |
| Sefer/ilan (`quest`) | 2,7 (1,8–3,6) | 6,55 (4,4–8,55) | 7,2 (5,3–9,55) | 7,3 (5,45–11) | 7,75 (3,65–14,6) | 9,1 (5,3–24,3) |
| Savaş (`war`) | 0,85 (0–1,65) | 3,25 (0,3–9,15) | 4,5 (1,15–10,8) | 5,65 (1,2–12,2) | 8 (0,15–10,2) | 6,6 (0–10,5) |
| Kamp (`lair`) | 2,45 (1,9–2,9) | 4,35 (3,15–6,65) | 4,65 (3–6,3) | 3,8 (2,25–5,15) | 3,2 (1,25–5,1) | 3,2 (1,8–7,1) |
| İnşaat (`build`) | 3,6 (2,55–4,75) | 8,05 (3,75–10,6) | 3,3 (2,2–4,1) | 1,4 (0,7–2,55) | 0,95 (0,4–1,7) | 0,65 (0,3–1,5) |
| Baskın (`raid`) | 3,5 (2,7–3,8) | 2,9 (2,4–4,1) | 2,6 (1,85–3,45) | 2,85 (2,15–3,8) | 2,55 (2,15–3,25) | 2,65 (1,85–3,65) |
| Deniz (`sea`) | 1,1 (0,3–3,25) | 1,8 (1,1–2,1) | 2,25 (0,9–4,45) | 1,8 (0,35–3,6) | 1,85 (0,7–3,3) | 2,1 (0,25–3,95) |
| Han (`inn`) | 1,4 (1,2–1,9) | 2,15 (1,8–2,65) | 1,65 (1,35–2,55) | 1,5 (1,05–2,4) | 1,35 (0,95–2,4) | 1,95 (0,9–2,45) |
| Ölüm/terk (`death`) | 0,4 (0,15–0,6) | 1 (0,7–1,65) | 1,35 (0,95–2,2) | 1,45 (0,85–2,65) | 1,65 (0,8–3,1) | 2,05 (0,85–3,4) |
| Ejderha (`dragon`) | 0 | 0,1 (0–0,65) | 2 (1,8–2,3) | 1,8 (1,6–2,2) | 1,9 (0,7–2,2) | 1,85 (0,65–2,25) |
| epitaph | 0,35 (0,1–0,55) | 1 (0,65–1,65) | 1,35 (0,9–2,15) | 1,35 (0,75–2,6) | 1,6 (0,65–2,95) | 2 (0,75–3,15) |
| Yerleşim (`settle`) | 2,9 (2,15–3,3) | 1,9 (1,4–2,25) | 1,2 (0,95–1,65) | 0,6 (0,3–1,2) | 0,55 (0,4–0,9) | 0,5 (0,3–0,85) |
| Sınıf (`class`) | 1 (0,5–2) | 1,45 (0,7–2,2) | 1,4 (1,1–2,55) | 1,15 (0,55–2,05) | 1 (0,45–2) | 1 (0,05–2) |
| Keşif (`discover`) | 2,45 (1,8–3,45) | 1,05 (0,4–1,6) | 0,75 (0,35–1,25) | 0,4 (0,15–0,8) | 0,3 (0,05–1,05) | 0,2 (0,05–1,15) |
| Dünya (`world`) | 0 (0–0,25) | 0,55 (0,35–1,15) | 1 (0,75–1,3) | 0,9 (0,5–1,55) | 1,15 (0,65–1,95) | 1,15 (0,5–1,8) |
| Ekonomi (`economy`) | 0 (0–0,1) | 1,45 (0,85–2,15) | 1,4 (1,15–1,65) | 0,9 (0,55–1,15) | 0,5 (0,1–0,85) | 0,25 (0,1–0,5) |
| Büyüme (`growth`) | 1,8 (1,3–2,15) | 1,45 (1,25–1,65) | 0,5 (0,25–0,65) | 0,1 (0–0,35) | 0 (0–0,25) | 0 (0–0,2) |
| Ticaret (`trade`) | 1,1 (0,5–1,5) | 0,4 (0,2–1,1) | 0,45 (0,15–1,15) | 0,5 (0–0,85) | 0,4 (0,15–1,05) | 0,5 (0–0,85) |
| Gerginlik (`tension`) | 0,7 (0,4–1,25) | 0,35 (0,1–1,2) | 0,3 (0,1–0,9) | 0,25 (0–0,75) | 0,35 (0,1–0,95) | 0,3 (0–1,1) |
| Temas (`contact`) | 1,15 (0,5–1,65) | 0,3 (0,15–0,7) | 0,2 (0,1–0,4) | 0,05 (0–0,25) | 0,1 (0–0,25) | 0 (0–0,3) |
| Kriz (anlatıcı) (`crisis`) | 0 (0–0,1) | 0,3 (0,1–0,5) | 0,25 (0–0,4) | 0,2 (0,1–0,55) | 0,4 (0,1–0,55) | 0,35 (0,15–0,55) |
| Diplomasi (`diplomacy`) | 0,2 (0,1–0,75) | 0,3 (0,1–0,65) | 0,2 (0–0,6) | 0,1 (0–0,6) | 0,25 (0–0,75) | 0,2 (0–1,05) |
| Göç (`migration`) | 0,15 (0–0,35) | 0,2 (0,1–0,3) | 0,15 (0–0,4) | 0,1 (0,05–0,35) | 0,15 (0,05–0,45) | 0,3 (0,05–0,4) |
| Rahatlama (anlatıcı) (`relief`) | 0,2 (0,15–0,3) | 0,1 (0–0,1) | 0,1 (0,05–0,2) | 0,1 (0–0,2) | 0 (0–0,1) | 0,05 (0–0,25) |

## Muharebe türleri

Dünya başına yıllık muharebe sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Yapı baskını (canavar) (`extRaid`) | 2,2 (1,5–2,8) | 2,05 (1,5–2,7) | 1,5 (0,95–2,4) | 1,55 (1,05–2,3) | 1,4 (0,8–2,2) | 1,15 (0,9–1,9) |
| Kamp saldırısı (`camp`) | 0,3 (0,1–0,65) | 1,85 (1,3–2,9) | 1,85 (1,15–2,5) | 1,65 (0,95–2,1) | 1,45 (0,5–2,15) | 1,4 (0,8–2,75) |
| Yerleşim baskını (canavar) (`raid`) | 1,75 (1,25–2,3) | 1,35 (0,95–2,45) | 0,8 (0,35–1,4) | 0,7 (0,35–1,15) | 0,7 (0,25–1,2) | 0,7 (0,3–1,15) |
| Kuşatma (`siege`) | 0,2 (0–0,3) | 0,65 (0–1,45) | 1,05 (0,3–2,1) | 1,2 (0,3–1,95) | 1,4 (0,05–1,8) | 1,3 (0–1,9) |
| Trol (`troll`) | 0 | 0,25 (0–0,65) | 1 (0,2–1,6) | 1,15 (0,25–1,6) | 1,05 (0,3–1,95) | 1,25 (0,6–2,55) |
| Yağma akını (`plunder`) | 0 (0–0,35) | 0,4 (0–2,05) | 0,75 (0–2,1) | 1 (0–2,55) | 1,3 (0–2,2) | 1 (0–2,45) |
| Ejderha (`dragon`) | 0 | 0 (0–0,3) | 1 (0,65–1,2) | 0,85 (0,6–1,15) | 0,85 (0,15–1,2) | 0,8 (0,15–1,2) |
| Han baskını (canavar) (`innMonster`) | 0,2 (0–0,3) | 0,1 (0–0,2) | 0,1 (0–0,2) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,15) |
| Korsan savaşı (`pirate`) | 0 | 0,1 (0–0,3) | 0,05 (0–0,25) | 0,1 (0–0,35) | 0 (0–0,3) | 0,1 (0–0,35) |
| Deniz savaşı (`naval`) | 0 | 0 | 0,05 (0–0,5) | 0,05 (0–0,2) | 0,1 (0–0,25) | 0,1 (0–0,3) |
| Kervan soygunu (`robbery`) | 0 (0–0,05) | 0 (0–0,2) | 0 (0–0,25) | 0,05 (0–0,35) | 0,1 (0–0,2) | 0 (0–0,4) |
| Yol pususu (`ambush`) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,15) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) |
| Düello (`duel`) | 0 | 0 | 0 | 0 (0–0,05) | 0 (0–0,1) | 0 (0–0,05) |
| Han baskını (medeniyet) (`innCiv`) | 0 | 0 (0–0,1) | 0 (0–0,05) | 0 (0–0,1) | 0 | 0 (0–0,1) |

## Kamp türleri

Dünya başına yaşayan kamp (yıl sonu değerlerinin on yıllık ortalaması): dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Hobgoblin (`hobgoblin`) | 1,4 (0,9–1,85) | 2,8 (1,8–3,5) | 3,1 (2,55–4,6) | 3,5 (2,95–4,45) | 3,75 (2,75–4,65) | 3,3 (2,5–4,35) |
| Goblin (`goblin`) | 6,2 (5,8–7) | 4 (3,2–4,6) | 2,65 (1,2–3,55) | 1,6 (0,5–3,35) | 1,2 (0,45–2,55) | 1,2 (0,75–2,75) |
| Trol (`troll`) | 0 | 0,15 (0–0,65) | 0,65 (0,25–1,75) | 1,35 (0,75–2,45) | 1,7 (0,75–3) | 2 (0,6–2,95) |
| Bugbear (`bugbear`) | 0,4 (0,05–0,7) | 0,9 (0,7–1,3) | 1,35 (0,5–1,85) | 1 (0,6–1,65) | 1,1 (0,85–1,75) | 1 (0,25–1,6) |
| Ejderha (`dragon`) | 0 | 0,1 (0–0,3) | 1 (0,95–1) | 1 (0,85–1) | 1 (0,5–1) | 1 (0,35–1) |
| Korsan (`pirate`) | 0,15 (0–0,5) | 0,7 (0,25–1,7) | 0,4 (0,15–1,05) | 0,4 (0,15–1,15) | 0,55 (0,05–1,2) | 0,35 (0–1,25) |

## Kademe dağılımı

Yaşayan yerleşimlerin kademelere dağılımı, bütün dünyalar (yıl sonu; parantezde sayı).

| Yıl | 0 Kamp | 1 Köy | 2 Kasaba | 3 Şehir |
|---|---|---|---|---|
| 10 | %33 (194) | %49 (283) | %18 (106) | %0 (0) |
| 20 | %26 (228) | %39 (347) | %30 (265) | %4,4 (39) |
| 30 | %27 (290) | %37 (391) | %27 (287) | %9,4 (100) |
| 40 | %24 (274) | %37 (433) | %28 (331) | %11 (125) |
| 50 | %22 (279) | %39 (482) | %28 (344) | %11 (141) |
| 60 | %20 (259) | %42 (542) | %27 (354) | %12 (151) |

Başkentlerin kademelere dağılımı, bütün dünyalar (yıl sonu; parantezde sayı).

| Yıl | 0 Kamp | 1 Köy | 2 Kasaba | 3 Şehir |
|---|---|---|---|---|
| 10 | %1,6 (2) | %37 (46) | %62 (77) | %0 (0) |
| 20 | %0,8 (1) | %4,8 (6) | %63 (79) | %31 (39) |
| 30 | %1,6 (2) | %2,4 (3) | %28 (35) | %69 (87) |
| 40 | %1,6 (2) | %2,3 (3) | %26 (33) | %70 (90) |
| 50 | %0,8 (1) | %1,5 (2) | %22 (28) | %76 (99) |
| 60 | %1,5 (2) | %0,8 (1) | %19 (25) | %78 (102) |

## Dünyalar

| Seed | Medeniyet | Yerleşim | Nüfus | Çöküş | Efsane | En yüksek Sv | Doğan / ölü kahraman | İlk şehir (yıl, medyan) | Süre (sn) | Son hash |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | 9 | 95 | 4145 | 2 | 2 | 8 | 198 / 96 | 19,5 | 58,5 | `ea518c4edcbf6601` |
| 2 | 7 | 60 | 3578 | 0 | 7 | 7 | 149 / 45 | 23 | 43,9 | `8fe8c1d0fe3092b8` |
| 3 | 9 | 97 | 4199 | 1 | 2 | 8 | 208 / 98 | 20 | 49,7 | `f9269ef487869cf7` |
| 4 | 8 | 91 | 3796 | 0 | 6 | 9 | 234 / 95 | 21,5 | 52,1 | `4862c1c6bce75c6d` |
| 5 | 9 | 96 | 4436 | 3 | 8 | 9 | 213 / 80 | 22 | 53,7 | `9a33c4ad5b72d166` |
| 6 | 7 | 68 | 3233 | 3 | 7 | 7 | 181 / 64 | 28 | 36,9 | `6ac103ff0b0c6302` |
| 7 | 8 | 88 | 3979 | 1 | 3 | 9 | 211 / 86 | 19 | 48,9 | `6a5fe031794f7743` |
| 8 | 8 | 83 | 4005 | 1 | 10 | 10 | 148 / 59 | 24,5 | 43,1 | `bad9410dc1c177a0` |
| 9 | 10 | 88 | 4531 | 3 | 2 | 9 | 191 / 81 | 23 | 52 | `ad79120e6bac4d22` |
| 10 | 9 | 86 | 3629 | 6 | 3 | 6 | 198 / 77 | 21 | 46,4 | `a757dabd914f18da` |
| 11 | 7 | 69 | 2929 | 1 | 6 | 10 | 203 / 79 | 21 | 37,1 | `ed0ee35aba56463d` |
| 12 | 8 | 68 | 3489 | 2 | 6 | 9 | 160 / 54 | 23 | 35 | `49b3dd3c7c523bf1` |
| 13 | 8 | 85 | 3418 | 6 | 6 | 9 | 209 / 96 | 21 | 43 | `97668d36892a1e77` |
| 14 | 6 | 52 | 2709 | 1 | 10 | 8 | 177 / 77 | 25,5 | 33,6 | `bfb2f0473f2d749c` |
| 15 | 10 | 98 | 3895 | 4 | 11 | 10 | 253 / 125 | 21,5 | 56,2 | `f8cc69fa9192a5d8` |
| 16 | 7 | 82 | 3406 | 6 | 9 | 10 | 182 / 89 | 25 | 43,1 | `af784d09926083db` |

Çöküşler:

- seed 1, 32. yıl (gün 3812): Ayışığı Korusu Boyu başkenti kaybetti: Ayışığı Korusu (Güneştacı Krallığı aldı)
- seed 1, 39. yıl (gün 4673): Kanlıdiş Kabileleri başkenti kaybetti: Sisliyamaç (Rüzgâr Manastırı aldı)
- seed 3, 41. yıl (gün 4838): Karaörs Derinlikleri başkenti kaybetti: Ulukoru (Güneştacı Krallığı aldı)
- seed 5, 14. yıl (gün 1625): Kızılboynuz Soyu başkenti kaybetti: Kızılkül (Kanlıdiş Kabileleri aldı)
- seed 5, 20. yıl (gün 2362): Karaörs Derinlikleri başkenti kaybetti: Sisliyamaç (Rüzgâr Manastırı aldı)
- seed 5, 31. yıl (gün 3608): Kanlıdiş Kabileleri başkenti kaybetti: Gölpınar (Pulzırh Lejyonu aldı)
- seed 6, 7. yıl (gün 743): Pulzırh Lejyonu yok oldu
- seed 6, 52. yıl (gün 6206): Sarıdere Bekçileri başkenti kaybetti: Sarıdere (Sınır Bekçileri aldı)
- seed 6, 56. yıl (gün 6717): Sarıdere Bekçileri yok oldu
- seed 7, 18. yıl (gün 2126): Yeşilyaprak Çemberi yok oldu
- seed 8, 31. yıl (gün 3610): Kızılboynuz Soyu başkenti kaybetti: Meşekent (Örsyürek Tapınak Klanı aldı)
- seed 9, 9. yıl (gün 1001): Karaörs Derinlikleri başkenti kaybetti: Külçukur (Güneştacı Krallığı aldı)
- seed 9, 25. yıl (gün 2910): Kartalkaya Beyliği yok oldu
- seed 9, 45. yıl (gün 5350): Örsyürek Tapınak Klanı başkenti kaybetti: Kutsalörs (Kızılboynuz Soyu aldı)
- seed 10, 25. yıl (gün 2929): Yeşilyaprak Çemberi yok oldu
- seed 10, 28. yıl (gün 3304): Tatlıçayır Loncası başkenti kaybetti: Fıçıköy (Karaörs Derinlikleri aldı)
- seed 10, 31. yıl (gün 3606): Karaörs Derinlikleri başkenti kaybetti: Fıçıköy (Kanlıdiş Kabileleri aldı)
- seed 10, 31. yıl (gün 3711): Günyazı Şehir Devleti yok oldu
- seed 10, 34. yıl (gün 4071): Karaörs Derinlikleri başkenti kaybetti: Gölgeçarşı (Kanlıdiş Kabileleri aldı)
- seed 10, 59. yıl (gün 7072): Çarkyıldız Akademisi başkenti kaybetti: Yıldızgöz (Kanlıdiş Kabileleri aldı)
- seed 11, 52. yıl (gün 6139): Örsyürek Tapınak Klanı başkenti kaybetti: Balgeçit (Kanlıdiş Kabileleri aldı)
- seed 12, 9. yıl (gün 1067): Kanlıdiş Kabileleri yok oldu
- seed 12, 23. yıl (gün 2686): Çarkyıldız Akademisi başkenti kaybetti: Pusulakule (Karaörs Derinlikleri aldı)
- seed 13, 7. yıl (gün 764): Sınır Bekçileri yok oldu
- seed 13, 34. yıl (gün 3979): Yeşilyaprak Çemberi başkenti kaybetti: Sessizkoru (Kanlıdiş Kabileleri aldı)
- seed 13, 39. yıl (gün 4613): Kanlıdiş Kabileleri başkenti kaybetti: Sessizkoru (Pulzırh Lejyonu aldı)
- seed 13, 44. yıl (gün 5169): Tatlıçayır Loncası başkenti kaybetti: Kırkkapı (Kanlıdiş Kabileleri aldı)
- seed 13, 46. yıl (gün 5433): Kocaköprü Boyu başkenti kaybetti: Kocaköprü (Kanlıdiş Kabileleri aldı)
- seed 13, 60. yıl (gün 7172): Kızılboynuz Soyu başkenti kaybetti: Kızılkül (Pulzırh Lejyonu aldı)
- seed 14, 10. yıl (gün 1101): Sınır Bekçileri yok oldu
- seed 15, 32. yıl (gün 3732): Lirsesi Şehirleri başkenti kaybetti: Tamburlu (Kanlıdiş Kabileleri aldı)
- seed 15, 32. yıl (gün 3750): Kızılboynuz Soyu başkenti kaybetti: Boynuztepe (Güneştacı Krallığı aldı)
- seed 15, 37. yıl (gün 4388): Kızılboynuz Soyu başkenti kaybetti: İzsürer (Güneştacı Krallığı aldı)
- seed 15, 57. yıl (gün 6818): Yelköy Beyliği başkenti kaybetti: Yelköy (Kızılboynuz Soyu aldı)
- seed 16, 20. yıl (gün 2340): Tatlıçayır Loncası başkenti kaybetti: Kavşakpazar (Karaörs Derinlikleri aldı)
- seed 16, 30. yıl (gün 3587): Karaörs Derinlikleri başkenti kaybetti: Karagöl (Sınır Bekçileri aldı)
- seed 16, 38. yıl (gün 4486): Karaörs Derinlikleri başkenti kaybetti: Ayburç (Örsyürek Tapınak Klanı aldı)
- seed 16, 46. yıl (gün 5428): Karaörs Derinlikleri yok oldu
- seed 16, 53. yıl (gün 6284): Kanlıdiş Kabileleri başkenti kaybetti: Yelköy (Pulzırh Lejyonu aldı)
- seed 16, 56. yıl (gün 6674): Pınarhisar Serbest Şehri yok oldu

## Yıllık ayrıntı

Hücre: medyan (p10–p90), 16 dünya. Yıl y = (y−1)·120+1 … y·120. günler. Bütün değerler `report.json` içinde (`metrics`), dünya başına değerler `../runs/f1b-4` altında.

### Medeniyet (1/3)

| Yıl | Yaşayan medeniyet | Yeni medeniyet (yeniden doğan) | Yok olan medeniyet | Başkent kaybı (medeniyet yaşarken) | Çöküş (yok olma + başkent kaybı) | Yaşayan yerleşim |
|---|---|---|---|---|---|---|
| 1 | 8 (7–9) | 0 | 0 | 0 | 0 | 8 (7–9) |
| 2 | 8 (7–9) | 0 | 0 | 0 | 0 | 8 (7–9) |
| 3 | 8 (7–9) | 0 | 0 | 0 | 0 | 10,5 (8–12) |
| 4 | 8 (7–9) | 0 | 0 | 0 | 0 | 15 (12–17,5) |
| 5 | 8 (7–9) | 0 | 0 | 0 | 0 | 19 (15–21,5) |
| 6 | 8 (7–9) | 0 | 0 | 0 | 0 | 24 (18,5–26,5) |
| 7 | 8 (7–9) | 0 | 0 (0–0,5) | 0 | 0 (0–0,5) | 28,5 (21,5–31) |
| 8 | 8 (7–9) | 0 | 0 | 0 | 0 | 31,5 (24,5–34,5) |
| 9 | 8 (7–9) | 0 | 0 | 0 | 0 (0–0,5) | 34,5 (25–39,5) |
| 10 | 8 (6,5–9) | 0 | 0 | 0 | 0 | 37 (28,5–42) |
| 11 | 8 (7–9) | 0 | 0 | 0 | 0 | 40 (31–45,5) |
| 12 | 8 (7–9) | 0 | 0 | 0 | 0 | 41,5 (33–50) |
| 13 | 8 (7–9) | 0 | 0 | 0 | 0 | 45,5 (34–51,5) |
| 14 | 8 (7–9) | 0 | 0 | 0 | 0 | 48,5 (35,5–54) |
| 15 | 8 (7–9) | 0 | 0 | 0 | 0 | 49,5 (38–56) |
| 16 | 8 (7–9) | 0 | 0 | 0 | 0 | 52,5 (38–57,5) |
| 17 | 8 (7–9) | 0 | 0 | 0 | 0 | 54 (40,5–59) |
| 18 | 8 (7–9) | 0 | 0 | 0 | 0 | 55 (43–62) |
| 19 | 8 (7–9) | 0 | 0 | 0 | 0 | 56 (43,5–62,5) |
| 20 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 57 (44–64) |
| 21 | 8 (7–9) | 0 | 0 | 0 | 0 | 58,5 (45,5–66) |
| 22 | 8 (7–9) | 0 | 0 | 0 | 0 | 59 (47–67) |
| 23 | 8 (7–9) | 0 | 0 | 0 | 0 | 61,5 (49–68,5) |
| 24 | 8 (7–9) | 0 | 0 | 0 | 0 | 61,5 (49–69) |
| 25 | 8 (7–9) | 0 | 0 (0–0,5) | 0 | 0 (0–0,5) | 62,5 (50,5–70,5) |
| 26 | 8 (7–9) | 0 | 0 | 0 | 0 | 63,5 (51,5–71,5) |
| 27 | 8 (7–9) | 0 | 0 | 0 | 0 | 64,5 (51,5–72,5) |
| 28 | 8 (7–9) | 0 | 0 | 0 | 0 | 66,5 (53–75,5) |
| 29 | 8 (7–9) | 0 | 0 | 0 | 0 | 66,5 (53–77,5) |
| 30 | 8 (7–9) | 0 | 0 | 0 | 0 | 69 (54,5–78,5) |
| 31 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 70 (54,5–79) |
| 32 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 70,5 (55–80) |
| 33 | 8 (7–9) | 0 | 0 | 0 | 0 | 71 (55,5–79,5) |
| 34 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 72 (55–80) |
| 35 | 8 (7–9) | 0 | 0 | 0 | 0 | 73 (55–81,5) |
| 36 | 8 (7–9) | 0 | 0 | 0 | 0 | 73,5 (55–83,5) |
| 37 | 8 (7–9) | 0 | 0 | 0 | 0 | 74,5 (55,5–84,5) |
| 38 | 8 (7–9) | 0 | 0 | 0 | 0 | 75 (56–85) |
| 39 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 76 (57–85,5) |
| 40 | 8 (7–9) | 0 | 0 | 0 | 0 | 76 (57–86) |
| 41 | 8 (7–9) | 0 | 0 | 0 | 0 | 78 (57–86,5) |
| 42 | 8 (7–9) | 0 | 0 | 0 | 0 | 80 (57–87) |
| 43 | 8 (7–9) | 0 | 0 | 0 | 0 | 80 (57,5–87,5) |
| 44 | 8 (7–9) | 0 | 0 | 0 | 0 | 80 (57,5–88) |
| 45 | 8 (7–9) | 0 | 0 | 0 | 0 | 79,5 (59–88,5) |
| 46 | 8 (7–9) | 0 | 0 | 0 | 0 (0–0,5) | 80,5 (59,5–89,5) |
| 47 | 8 (7–9) | 0 | 0 | 0 | 0 | 80,5 (60–90) |
| 48 | 8 (7–9) | 0 | 0 | 0 | 0 | 81 (60–91) |
| 49 | 8 (7–9) | 0 | 0 | 0 | 0 | 81,5 (60,5–92,5) |
| 50 | 8 (7–9) | 0 | 0 | 0 | 0 | 82 (61–93) |
| 51 | 8 (7–9) | 0 | 0 | 0 | 0 | 82,5 (61,5–93) |
| 52 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 82,5 (62–93) |
| 53 | 8 (7–9) | 0 | 0 | 0 | 0 | 83,5 (62–94) |
| 54 | 8 (7–9) | 0 | 0 | 0 | 0 | 83,5 (62–95) |
| 55 | 8 (7–9,5) | 0 | 0 | 0 | 0 | 85 (62–94,5) |
| 56 | 8 (7–9,5) | 0 | 0 (0–0,5) | 0 | 0 (0–0,5) | 85,5 (62–94,5) |
| 57 | 8 (7–9,5) | 0 | 0 | 0 | 0 | 85 (62,5–95) |
| 58 | 8 (7–9,5) | 0 | 0 | 0 | 0 | 84,5 (63,5–95,5) |
| 59 | 8 (7–9,5) | 0 | 0 | 0 | 0 | 85 (64–95,5) |
| 60 | 8 (7–9,5) | 0 | 0 | 0 | 0 | 85,5 (64–96,5) |

### Medeniyet (2/3)

| Yıl | Medeniyet başına yerleşim | 5+ kara yerleşimli medeniyet payı | Kurulan yerleşim | Fethedilen yerleşim | Terk edilen yerleşim | Toplam nüfus |
|---|---|---|---|---|---|---|
| 1 | 1 | %0 | 0 | 0 | 0 | 62,5 (53–72) |
| 2 | 1 | %0 | 0 | 0 | 0 | 86,5 (70–98) |
| 3 | 1,27 (1,13–1,47) | %0 | 2 (1–4) | 0 | 0 | 124 (101–146) |
| 4 | 1,87 (1,54–2,13) | %0 | 4,5 (3–7) | 0 | 0 | 182 (138–206) |
| 5 | 2,28 (2,06–2,65) | %0 | 4 (3–5,5) | 0 | 0 | 258 (192–300) |
| 6 | 2,78 (2,54–3,25) | %0 | 4 (3–5,5) | 0 | 0 | 359 (274–424) |
| 7 | 3,43 (3,17–3,65) | %12 (%0–%23) | 4 (2,5–6) | 0 (0–1) | 0 | 487 (367–572) |
| 8 | 3,82 (3,46–4) | %25 (%12–%33) | 3 (1–5) | 0 (0–1) | 0 | 614 (473–718) |
| 9 | 4,25 (3,86–4,63) | %43 (%29–%59) | 3 (2–5,5) | 0 (0–1) | 0 (0–0,5) | 757 (568–880) |
| 10 | 4,65 (4,39–5,06) | %56 (%43–%71) | 3 (2–4,5) | 0 (0–1,5) | 0 | 909 (660–1050) |
| 11 | 4,94 (4,5–5,46) | %67 (%56–%83) | 3 (1,5–4) | 0 (0–1,5) | 0 | 1027 (756–1198) |
| 12 | 5,29 (4,71–5,77) | %76 (%51–%88) | 3 (1–4) | 0 (0–1) | 0 | 1155 (847–1372) |
| 13 | 5,65 (4,86–6,06) | %85 (%64–%89) | 2 (1–4) | 0 (0–1) | 0 | 1276 (944–1540) |
| 14 | 5,94 (5,29–6,32) | %87 (%71–%100) | 2,5 (1–4) | 0 (0–1) | 0 | 1424 (1040–1673) |
| 15 | 6,13 (5,57–6,59) | %88 (%78–%100) | 2 (1–3) | 1 (0–2) | 0 | 1562 (1141–1788) |
| 16 | 6,35 (5,71–6,75) | %94 (%82–%100) | 2 (0,5–3) | 0,5 (0–1) | 0 | 1682 (1236–1932) |
| 17 | 6,5 (6–6,94) | %94 (%71–%100) | 1,5 (0–2,5) | 0 (0–3) | 0 | 1772 (1302–2052) |
| 18 | 6,82 (6,29–7,13) | %94 (%86–%100) | 1,5 (0–3) | 0,5 (0–1,5) | 0 | 1856 (1383–2192) |
| 19 | 6,88 (6,43–7,33) | %100 (%87–%100) | 1 (0,5–2) | 0 (0–1,5) | 0 | 2009 (1498–2275) |
| 20 | 7 (6,49–7,59) | %100 (%82–%100) | 1 (0–2) | 1 (0–2) | 0 | 2070 (1553–2367) |
| 21 | 7,29 (6,56–7,8) | %100 (%82–%100) | 1 (0–3) | 0,5 (0–2) | 0 | 2184 (1636–2471) |
| 22 | 7,41 (6,69–7,94) | %100 (%83–%100) | 1 (0,5–2) | 1 (0–1,5) | 0 | 2192 (1676–2554) |
| 23 | 7,54 (6,94–8) | %100 (%86–%100) | 1 (0–3) | 0 (0–2) | 0 | 2326 (1780–2718) |
| 24 | 7,63 (7–8,27) | %100 (%86–%100) | 1 (0–2) | 0 (0–2,5) | 0 | 2266 (1835–2774) |
| 25 | 7,94 (7,23–8,38) | %100 (%87–%100) | 1 (0–2,5) | 0,5 (0–1,5) | 0 (0–0,5) | 2382 (1941–2792) |
| 26 | 7,94 (7,31–8,56) | %100 (%87–%100) | 1 (0–2) | 1 (0–3) | 0 | 2498 (1959–3017) |
| 27 | 8 (7,33–8,81) | %100 (%88–%100) | 1 (0–2) | 0,5 (0–2,5) | 0 (0–1) | 2540 (2026–3054) |
| 28 | 8,29 (7,58–8,75) | %89 (%86–%100) | 1 (0–3) | 1 (0–2,5) | 0 | 2632 (2122–3114) |
| 29 | 8,42 (7,57–9) | %100 (%87–%100) | 1 (0–2) | 0,5 (0–2) | 0 (0–0,5) | 2711 (2119–3173) |
| 30 | 8,39 (7,79–9) | %100 (%87–%100) | 1 (0–2,5) | 0,5 (0–1,5) | 0 | 2632 (2268–3186) |
| 31 | 8,6 (7,94–9,18) | %100 (%88–%100) | 1 (0–2) | 1 (0–2,5) | 0 | 2762 (2336–3278) |
| 32 | 8,66 (7,94–9,24) | %100 (%87–%100) | 1 (0–1) | 0 (0–3) | 0 (0–0,5) | 2823 (2384–3400) |
| 33 | 8,69 (8,02–9,35) | %100 (%87–%100) | 1 (0–2) | 0,5 (0–1,5) | 0 (0–0,5) | 2902 (2427–3394) |
| 34 | 8,78 (7,86–9,41) | %100 (%87–%100) | 0 (0–1) | 1 (0–2) | 0 | 2878 (2505–3480) |
| 35 | 9 (7,94–9,53) | %100 (%87–%100) | 1 (0–2,5) | 0 (0–1,5) | 0 | 2968 (2576–3427) |
| 36 | 9,17 (7,94–9,65) | %100 (%87–%100) | 0 (0–1,5) | 1 (0–2) | 0 | 3065 (2647–3474) |
| 37 | 9,28 (8,06–9,65) | %94 (%82–%100) | 0 (0–1,5) | 0,5 (0–1,5) | 0 | 3051 (2592–3420) |
| 38 | 9,33 (8,14–9,81) | %100 (%82–%100) | 1 (0–2) | 1 (0–2) | 0 | 3030 (2606–3584) |
| 39 | 9,38 (8,25–9,83) | %94 (%87–%100) | 0 (0–1,5) | 0 (0–3) | 0 | 3144 (2626–3566) |
| 40 | 9,17 (8,13–9,94) | %89 (%87–%100) | 0 (0–1) | 1 (0–2) | 0 (0–1) | 3181 (2706–3692) |
| 41 | 9,28 (8,01–10) | %89 (%83–%100) | 1 (0–1,5) | 1 (0–2) | 0 | 3262 (2766–3832) |
| 42 | 9,28 (8,18–10,1) | %89 (%88–%100) | 0 (0–1) | 1 (0–2,5) | 0 (0–1) | 3276 (2805–3904) |
| 43 | 9,28 (8,18–10,1) | %89 (%87–%100) | 1 (0–1) | 0,5 (0–2) | 0 (0–1) | 3281 (2796–3896) |
| 44 | 9,35 (8,42–10,1) | %89 (%87–%100) | 0 (0–2) | 1 (0–2) | 0 (0–1) | 3244 (2858–3936) |
| 45 | 9,35 (8,47–10,1) | %89 (%88–%100) | 0,5 (0–1,5) | 1 (0–2) | 0 | 3364 (2856–3962) |
| 46 | 9,42 (8,48–10,3) | %100 (%88–%100) | 1 (0–1) | 0,5 (0–2,5) | 0 | 3358 (2916–3972) |
| 47 | 9,42 (8,31–10,5) | %89 (%82–%100) | 1 (0–1) | 1 (0–1,5) | 0 (0–1) | 3410 (2866–4016) |
| 48 | 9,42 (8,31–10,5) | %89 (%87–%100) | 0 (0–1,5) | 0,5 (0–2,5) | 0 | 3416 (2960–4004) |
| 49 | 9,56 (8,38–10,6) | %89 (%88–%100) | 1 (0–2,5) | 1 (0–1,5) | 0 (0–1) | 3498 (2972–4144) |
| 50 | 9,56 (8,1–10,6) | %89 (%88–%100) | 0 (0–1) | 1 (0–2) | 0 | 3467 (2945–4066) |
| 51 | 9,54 (8,35–10,7) | %89 (%78–%100) | 0,5 (0–1,5) | 0,5 (0–1,5) | 0 (0–0,5) | 3504 (2956–4036) |
| 52 | 9,69 (8,35–10,7) | %89 (%76–%100) | 0 (0–1,5) | 1 (0–2,5) | 0 | 3502 (3027–4112) |
| 53 | 9,82 (8,35–10,6) | %89 (%83–%100) | 1 (0–1) | 1 (0–2) | 0 (0–0,5) | 3582 (2976–4204) |
| 54 | 9,87 (8,35–10,6) | %89 (%88–%100) | 0 (0–1) | 1 (0–2) | 0 (0–0,5) | 3618 (3040–4204) |
| 55 | 9,93 (8,47–10,7) | %89 (%84–%100) | 0 (0–1,5) | 1 (0–2) | 0 (0–0,5) | 3668 (2988–4274) |
| 56 | 9,99 (8,54–11) | %100 (%83–%100) | 1 (0–1) | 1,5 (0–3) | 0 (0–1) | 3656 (3058–4284) |
| 57 | 10,1 (8,54–11) | %95 (%83–%100) | 0 (0–2) | 0 (0–1) | 0 (0–1) | 3628 (2973–4158) |
| 58 | 10,1 (8,54–11,1) | %95 (%79–%100) | 0 (0–1) | 1 (0–2,5) | 0 (0–1) | 3606 (3104–4180) |
| 59 | 10,1 (8,54–11,2) | %95 (%79–%100) | 0 (0–1) | 0,5 (0–1) | 0 | 3720 (3096–4384) |
| 60 | 10,1 (8,62–11,2) | %89 (%79–%100) | 0 (0–1) | 1 (0–2) | 0 | 3712 (3081–4318) |

### Medeniyet (3/3)

| Yıl | Altın medyanı (medeniyetler) | Boştaki iş gücü payı | Bölünme (ayrılıp kurulan medeniyet) | En büyük medeniyetin yerleşimi |
|---|---|---|---|---|
| 1 | 8,33 (8,2–14) | %0 | 0 | 1 |
| 2 | 17,7 (12,4–39,1) | %0 | 0 | 1 |
| 3 | 24,6 (18–44,2) | %0 | 0 | 2 |
| 4 | 33,8 (21,5–49,7) | %0 | 0 | 3 (2–3) |
| 5 | 33,9 (28,2–50) | %0 (%0–%0,1) | 0 | 3 (3–3,5) |
| 6 | 37,9 (27,3–56,2) | %0,2 (%0–%2,4) | 0 | 4 |
| 7 | 34 (22,6–55,8) | %0,8 (%0–%4) | 0 | 5 (4–5) |
| 8 | 33,9 (10,4–81) | %1,7 (%0,4–%5,5) | 0 | 5 (5–6) |
| 9 | 53,2 (18,7–98,2) | %2,9 (%1–%7) | 0 | 6 (5–6,5) |
| 10 | 49,3 (21,5–91,5) | %4,1 (%1,3–%6,2) | 0 | 6 (5,5–7,5) |
| 11 | 60,6 (18,7–148) | %4,7 (%2,4–%7,7) | 0 | 7 (6–7,5) |
| 12 | 79,9 (30,3–159) | %6,9 (%3,7–%10) | 0 | 7 (7–8) |
| 13 | 167 (33,1–301) | %7,9 (%3,7–%13) | 0 | 8 (7–8) |
| 14 | 130 (53,2–354) | %11 (%6,7–%15) | 0 | 8 (7–8,5) |
| 15 | 180 (47,6–349) | %12 (%8,2–%17) | 0 | 8 (8–9) |
| 16 | 212 (121–411) | %16 (%10–%20) | 0 | 8 (8–9) |
| 17 | 236 (135–396) | %18 (%12–%24) | 0 | 9 (8–10) |
| 18 | 300 (123–439) | %17 (%13–%21) | 0 | 9 (8–10) |
| 19 | 280 (112–476) | %19 (%14–%23) | 0 | 9 (8–10,5) |
| 20 | 292 (147–505) | %20 (%16–%25) | 0 | 9 (8–11) |
| 21 | 273 (189–455) | %20 (%17–%25) | 0 | 9,5 (8–12) |
| 22 | 306 (168–444) | %23 (%17–%27) | 0 | 10 (8,5–12) |
| 23 | 256 (153–498) | %22 (%17–%25) | 0 | 10 (9–12,5) |
| 24 | 368 (137–535) | %21 (%18–%25) | 0 | 10 (9–12,5) |
| 25 | 348 (193–546) | %22 (%17–%25) | 0 | 10 (9–13) |
| 26 | 276 (154–564) | %20 (%17–%23) | 0 | 10,5 (9–13,5) |
| 27 | 373 (202–558) | %21 (%16–%23) | 0 | 11 (9–14) |
| 28 | 346 (231–543) | %20 (%15–%26) | 0 | 11 (9,5–14,5) |
| 29 | 378 (210–581) | %20 (%16–%24) | 0 | 11 (9,5–15) |
| 30 | 427 (343–530) | %21 (%17–%25) | 0 | 11 (9,5–15) |
| 31 | 422 (326–660) | %20 (%16–%25) | 0 | 11,5 (9,5–15,5) |
| 32 | 523 (317–653) | %20 (%15–%23) | 0 | 12 (10–16) |
| 33 | 528 (225–663) | %19 (%14–%22) | 0 | 12,5 (10–16) |
| 34 | 450 (248–690) | %20 (%16–%22) | 0 | 12,5 (10–16,5) |
| 35 | 530 (295–736) | %19 (%16–%21) | 0 | 12,5 (9,5–16,5) |
| 36 | 558 (291–688) | %19 (%16–%21) | 0 | 13 (10–17,5) |
| 37 | 507 (339–708) | %19 (%17–%21) | 0 | 13,5 (10–18) |
| 38 | 553 (262–745) | %19 (%15–%22) | 0 | 14 (10–18) |
| 39 | 628 (309–827) | %18 (%16–%21) | 0 | 14 (10–18) |
| 40 | 638 (229–812) | %18 (%16–%22) | 0 | 15 (10–17,5) |
| 41 | 621 (182–779) | %18 (%15–%20) | 0 | 15 (10–18) |
| 42 | 609 (404–853) | %18 (%15–%22) | 0 | 15 (10–18,5) |
| 43 | 705 (414–862) | %18 (%16–%21) | 0 | 15 (10–18,5) |
| 44 | 612 (372–840) | %18 (%16–%22) | 0 | 15,5 (10–19) |
| 45 | 607 (365–918) | %17 (%14–%21) | 0 | 16 (10–19,5) |
| 46 | 579 (390–811) | %18 (%15–%20) | 0 | 16,5 (10,5–20) |
| 47 | 554 (458–876) | %17 (%14–%22) | 0 | 17 (10,5–20) |
| 48 | 575 (455–970) | %17 (%15–%21) | 0 | 17,5 (11–20) |
| 49 | 603 (457–797) | %17 (%15–%19) | 0 | 17 (10,5–20,5) |
| 50 | 662 (441–896) | %18 (%15–%20) | 0 | 18 (10,5–20,5) |
| 51 | 707 (393–874) | %18 (%14–%21) | 0 | 17,5 (10–20,5) |
| 52 | 594 (359–850) | %17 (%14–%21) | 0 | 18 (10–21) |
| 53 | 676 (496–821) | %16 (%13–%21) | 0 | 17,5 (10,5–21,5) |
| 54 | 579 (508–903) | %16 (%13–%21) | 0 | 18 (10,5–22) |
| 55 | 644 (433–906) | %17 (%13–%20) | 0 | 18,5 (10,5–21,5) |
| 56 | 623 (384–934) | %16 (%13–%21) | 0 | 18 (10,5–22) |
| 57 | 623 (463–917) | %16 (%14–%22) | 0 | 18 (10,5–22) |
| 58 | 682 (462–878) | %16 (%14–%21) | 0 | 17,5 (11–23,5) |
| 59 | 628 (437–1127) | %16 (%13–%20) | 0 | 18,5 (11–23,5) |
| 60 | 676 (370–998) | %17 (%14–%21) | 0 | 18,5 (11–24) |

### Olaylar

| Yıl | Olay | Büyük olay |
|---|---|---|
| 1 | 53 (44–62) | 19 (16,5–23,5) |
| 2 | 64,5 (48–77) | 25 (18,5–30,5) |
| 3 | 57,5 (41–76,5) | 21,5 (13–30) |
| 4 | 70,5 (61,5–93) | 24 (19,5–31,5) |
| 5 | 78 (71,5–102) | 26 (20–34) |
| 6 | 90 (82–101) | 28 (23,5–33,5) |
| 7 | 114 (91–138) | 35 (27–41,5) |
| 8 | 118 (94–166) | 37,5 (28–51) |
| 9 | 146 (106–189) | 42,5 (29–64) |
| 10 | 162 (122–190) | 44,5 (33,5–55,5) |
| 11 | 188 (129–212) | 46,5 (37–61,5) |
| 12 | 174 (136–210) | 52 (34,5–65,5) |
| 13 | 160 (130–223) | 45,5 (38,5–61,5) |
| 14 | 175 (109–207) | 50 (27,5–60,5) |
| 15 | 168 (121–189) | 44,5 (36,5–57,5) |
| 16 | 147 (110–196) | 46 (26,5–65) |
| 17 | 164 (118–227) | 45 (33,5–65,5) |
| 18 | 157 (112–198) | 49,5 (35,5–58,5) |
| 19 | 148 (116–174) | 45 (29,5–54) |
| 20 | 155 (110–196) | 49 (34,5–66) |
| 21 | 148 (122–209) | 44 (33–59) |
| 22 | 168 (130–210) | 48 (35–68) |
| 23 | 164 (123–238) | 46,5 (38–69) |
| 24 | 161 (105–189) | 46 (28,5–70) |
| 25 | 167 (124–211) | 47,5 (35,5–68,5) |
| 26 | 155 (138–194) | 46,5 (35–62,5) |
| 27 | 164 (134–200) | 49,5 (36–65,5) |
| 28 | 178 (131–212) | 51 (35,5–68,5) |
| 29 | 163 (120–204) | 47,5 (34,5–54) |
| 30 | 164 (112–204) | 46 (31,5–65) |
| 31 | 179 (109–214) | 46,5 (28–74) |
| 32 | 150 (90–188) | 47 (22,5–59,5) |
| 33 | 146 (101–208) | 43,5 (24–57) |
| 34 | 146 (112–184) | 46 (26–54,5) |
| 35 | 146 (114–210) | 42,5 (34–73) |
| 36 | 140 (91–218) | 37 (28,5–64,5) |
| 37 | 146 (106–224) | 47,5 (26–65) |
| 38 | 150 (98–204) | 46,5 (23,5–65) |
| 39 | 134 (80,5–194) | 42 (27–62,5) |
| 40 | 148 (91,5–200) | 37,5 (32,5–63,5) |
| 41 | 154 (95–213) | 45 (27,5–63,5) |
| 42 | 138 (89–208) | 37,5 (23,5–62,5) |
| 43 | 128 (97–192) | 40 (26,5–57) |
| 44 | 151 (97–186) | 48 (25,5–62) |
| 45 | 138 (88–210) | 44,5 (28–69) |
| 46 | 137 (88–181) | 47 (30–58,5) |
| 47 | 146 (110–196) | 44,5 (32,5–64,5) |
| 48 | 135 (93–220) | 42,5 (22–71) |
| 49 | 152 (112–218) | 47,5 (26,5–69,5) |
| 50 | 142 (102–194) | 34,5 (24–67,5) |
| 51 | 154 (102–208) | 44 (29–78) |
| 52 | 153 (132–224) | 50,5 (32–63) |
| 53 | 150 (108–226) | 38 (23,5–76) |
| 54 | 180 (112–231) | 47,5 (30–78,5) |
| 55 | 180 (95–216) | 54 (22,5–77,5) |
| 56 | 176 (130–228) | 56,5 (28,5–70) |
| 57 | 151 (117–259) | 47,5 (22–96) |
| 58 | 169 (128–253) | 59,5 (42,5–86) |
| 59 | 178 (114–232) | 43 (30,5–73,5) |
| 60 | 149 (106–289) | 44,5 (31,5–104) |

### Savaş (1/2)

| Yıl | Muharebe | Başlayan savaş | Süren savaş (yıl sonu) | Yıl içinde süren savaş | Yağma akını (medeniyet) | Tarihî hak savaşı |
|---|---|---|---|---|---|---|
| 1 | 0 | 0 | 0 | 0 | 0 | 0 |
| 2 | 0 | 0 | 0 | 0 | 0 | 0 |
| 3 | 0 | 0 | 0 | 0 | 0 | 0 |
| 4 | 4 (3–5) | 0 | 0 | 0 | 0 | 0 |
| 5 | 7 (6–9) | 0 | 0 | 0 | 0 | 0 |
| 6 | 7,5 (5,5–9) | 0 | 0 | 0 | 0 (0–0,5) | 0 |
| 7 | 7,5 (4–9) | 0 (0–1) | 0 (0–1) | 0 (0–1) | 0 | 0 |
| 8 | 8 (4,5–9) | 0 (0–1) | 0 | 0,5 (0–1,5) | 0 (0–1) | 0 |
| 9 | 8 (6,5–9,5) | 0 (0–1) | 0 (0–1) | 0 (0–1,5) | 0 (0–1,5) | 0 |
| 10 | 7,5 (5–9,5) | 0 (0–1,5) | 0 (0–0,5) | 0 (0–2,5) | 0 (0–1) | 0 (0–0,5) |
| 11 | 8 (6–12,5) | 0 (0–1,5) | 0 (0–0,5) | 0,5 (0–2) | 0 (0–1,5) | 0 |
| 12 | 8 (5,5–11) | 0 (0–1,5) | 0 (0–1) | 0,5 (0–2) | 0 (0–1,5) | 0 |
| 13 | 7,5 (5–11) | 0 (0–2) | 0 (0–1) | 1 (0–2) | 1 (0–3) | 0 |
| 14 | 6,5 (4,5–9,5) | 0 (0–3) | 0 (0–2) | 1 (0–3) | 0 (0–1) | 0 (0–0,5) |
| 15 | 8 (5–10) | 0 (0–2) | 0 (0–0,5) | 1 (0–2,5) | 1 (0–2) | 0 |
| 16 | 7,5 (4–11) | 1 (0–2) | 0 (0–1) | 1 (0–2,5) | 0 (0–3) | 0 |
| 17 | 7,5 (6–11) | 1 (0–2,5) | 0 (0–1) | 1 (0–3) | 0 (0–2,5) | 0 |
| 18 | 8 (6,5–12) | 1 (0–2,5) | 0 (0–1,5) | 1 (0–2,5) | 1 (0–3) | 0 (0–0,5) |
| 19 | 6,5 (4–10) | 0,5 (0–2,5) | 0 (0–2,5) | 1 (0–3,5) | 0 (0–2) | 0 (0–1) |
| 20 | 8,5 (5,5–11) | 1 (0–2,5) | 0 (0–2) | 1,5 (0,5–3,5) | 0,5 (0–2,5) | 0 |
| 21 | 9 (6–10,5) | 0 (0–1,5) | 0 (0–1) | 0,5 (0–3) | 0 (0–2) | 0 |
| 22 | 7,5 (5–10,5) | 1,5 (0–2,5) | 1 (0–1,5) | 2 (0–3) | 0 (0–1) | 0 (0–0,5) |
| 23 | 9 (4,5–12,5) | 1 (0–2) | 0 (0–2) | 1,5 (0–3,5) | 1 (0–2,5) | 0 (0–0,5) |
| 24 | 9 (4–12,5) | 1 (0–2,5) | 1 (0–2) | 1 (0–4) | 1 (0–1,5) | 0 (0–1) |
| 25 | 7,5 (5,5–12,5) | 0,5 (0–2,5) | 0,5 (0–3) | 1,5 (0,5–4) | 0 (0–2) | 0 (0–1) |
| 26 | 8 (6,5–13,5) | 0 (0–2) | 0 (0–1) | 1 (0–4,5) | 0 (0–3) | 0 (0–0,5) |
| 27 | 9 (6,5–13) | 0 (0–2,5) | 0 (0–1) | 1 (0–3) | 1 (0–2,5) | 0 (0–1) |
| 28 | 9 (6,5–14) | 1 (0–4) | 0 (0–1) | 1 (0–4,5) | 1 (0–2) | 0 (0–0,5) |
| 29 | 7,5 (4,5–13) | 0,5 (0–2,5) | 0 (0–1) | 1 (0–3) | 1 (0–3,5) | 0 |
| 30 | 9 (6–12,5) | 1 (0–3) | 1 (0–2) | 1 (0–3,5) | 1 (0–2) | 0 |
| 31 | 9,5 (5–13) | 1 (0–2) | 0 (0–1,5) | 2 (0–4) | 1 (0–4) | 0 (0–1) |
| 32 | 9 (5–12) | 1 (0–4) | 0,5 (0–1) | 1,5 (0–5) | 0 (0–3) | 0 (0–1) |
| 33 | 8 (5–12) | 0 (0–2) | 0 (0–2) | 1 (0–3) | 0,5 (0–3) | 0 |
| 34 | 9 (5,5–11,5) | 1 (0–2,5) | 0,5 (0–2) | 2 (0–3,5) | 1 (0–3) | 0 (0–1) |
| 35 | 9 (5,5–13) | 1 (0–2) | 0,5 (0–2) | 2 (0–3) | 0 (0–2,5) | 0 (0–0,5) |
| 36 | 9,5 (4,5–13,5) | 1 (0–2) | 0 (0–1) | 1,5 (0,5–4) | 1 (0–3) | 0 (0–0,5) |
| 37 | 8,5 (5,5–12,5) | 1 (0–3) | 0 (0–2,5) | 2 (0–3) | 1 (0–4,5) | 0 (0–1) |
| 38 | 9,5 (6–12,5) | 1 (0–2,5) | 0 (0–2) | 2 (0,5–3,5) | 0,5 (0–2,5) | 0 (0–0,5) |
| 39 | 8 (4,5–11,5) | 1 (0–4,5) | 0,5 (0–1) | 1 (0–5) | 1 (0–3) | 0 (0–0,5) |
| 40 | 7,5 (5,5–11,5) | 1 (0–2) | 0 (0–1,5) | 2 (0–3) | 0,5 (0–3) | 0 |
| 41 | 9 (6–11) | 1 (0–2) | 0 (0–1,5) | 1 (0–3) | 1 (0–3) | 0 (0–0,5) |
| 42 | 8 (4,5–10,5) | 0 (0–4) | 0 (0–1) | 1 (0–4,5) | 1,5 (0–3) | 0 (0–0,5) |
| 43 | 7 (3,5–12) | 1,5 (0–3,5) | 0 (0–2,5) | 2 (0–3,5) | 1 (0–3) | 0 (0–1) |
| 44 | 8,5 (5,5–13) | 0,5 (0–4) | 1 (0–2) | 2 (0–4) | 1,5 (0–3) | 0 (0–0,5) |
| 45 | 8 (5–12) | 1 (0–4) | 1 (0–3) | 2 (0–4,5) | 0,5 (0–2) | 0 (0–1) |
| 46 | 9 (5,5–12) | 0,5 (0–2) | 0 (0–1,5) | 1,5 (0–3,5) | 1 (0–2,5) | 0 (0–0,5) |
| 47 | 7,5 (4,5–11,5) | 1 (0–2) | 0 (0–2) | 1,5 (0–3,5) | 1 (0–2) | 0 |
| 48 | 7,5 (3,5–11) | 1 (0–4) | 0 (0–2,5) | 2 (0–4,5) | 1 (0–2,5) | 0 (0–0,5) |
| 49 | 8,5 (3,5–12) | 0,5 (0–2) | 0 (0–1) | 1,5 (0–4) | 1,5 (0–3) | 0 |
| 50 | 7,5 (4,5–13) | 1 (0–4) | 0 (0–2,5) | 2 (0–4) | 1 (0–2,5) | 0 (0–0,5) |
| 51 | 8,5 (5–12,5) | 1 (0–3) | 0 (0–2,5) | 1,5 (0–5) | 0 (0–2,5) | 0 (0–1) |
| 52 | 9,5 (5–12,5) | 1 (0–3,5) | 0 (0–1,5) | 2 (0–4,5) | 2 (0–3) | 0 (0–1) |
| 53 | 8,5 (3,5–13) | 1 (0–3) | 0 (0–2) | 1,5 (0–4) | 1 (0–2) | 0 |
| 54 | 9 (5–11) | 0 (0–3,5) | 0 (0–1) | 2 (0–4) | 1 (0–3) | 0 |
| 55 | 8 (4,5–12) | 1,5 (0–2) | 0 (0–2) | 2 (0–3,5) | 0,5 (0–3) | 0 (0–1) |
| 56 | 10,5 (5,5–12) | 2 (0–2,5) | 0 (0–1) | 2 (0–4) | 1 (0–3) | 0 (0–1) |
| 57 | 7 (4,5–13,5) | 0,5 (0–3) | 0,5 (0–2) | 1 (0–3,5) | 1,5 (0–2,5) | 0 |
| 58 | 11 (5–13,5) | 1 (0–3,5) | 1 (0–2) | 3 (0–4) | 1 (0–2) | 0 |
| 59 | 8 (5–10) | 0 (0–2,5) | 0 (0–2,5) | 1 (0–3) | 0 (0–2) | 0 |
| 60 | 9,5 (5,5–14) | 1,5 (0–3) | 1 (0–2) | 2 (0–5) | 1 (0–3) | 0 (0–0,5) |

### Savaş (2/2)

| Yıl | Pakt gereği savaş | Kutsal Sefer çağrısı | İhanet (pakt çiğnendi) | Savunma paktı (yıl sonu) |
|---|---|---|---|---|
| 1 | 0 | 0 | 0 | 0 |
| 2 | 0 | 0 | 0 | 0 |
| 3 | 0 | 0 | 0 | 0 |
| 4 | 0 | 0 | 0 | 0 |
| 5 | 0 | 0 | 0 | 0 |
| 6 | 0 | 0 | 0 | 0 |
| 7 | 0 | 0 | 0 | 0 |
| 8 | 0 | 0 | 0 | 0 |
| 9 | 0 | 0 | 0 | 0 (0–0,5) |
| 10 | 0 | 0 | 0 | 0 (0–1) |
| 11 | 0 | 0 | 0 | 0 (0–1) |
| 12 | 0 | 0 | 0 | 0 (0–1) |
| 13 | 0 | 0 | 0 | 0 (0–1,5) |
| 14 | 0 (0–0,5) | 0 | 0 | 0 (0–1,5) |
| 15 | 0 (0–0,5) | 0 | 0 | 1 (0–1,5) |
| 16 | 0 | 0 | 0 | 1 (0–1,5) |
| 17 | 0 (0–0,5) | 0 | 0 | 1 (0–1,5) |
| 18 | 0 | 0 | 0 | 1 (0–2) |
| 19 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 20 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 21 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 22 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 23 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 24 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 25 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 26 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 27 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 28 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 29 | 0 | 0 | 0 | 1 (0–2) |
| 30 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 31 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 32 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 33 | 0 | 0 | 0 | 1 (0–2) |
| 34 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 35 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 36 | 0 | 0 | 0 | 1 (0–2) |
| 37 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 38 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 39 | 0 (0–0,5) | 0 (0–0,5) | 0 (0–1) | 1 (0–2) |
| 40 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 41 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 42 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 43 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 44 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 45 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 46 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 47 | 0 (0–0,5) | 0 | 0 | 1 (0–2,5) |
| 48 | 0 (0–1) | 0 | 0 | 1 (0–2,5) |
| 49 | 0 | 0 | 0 | 1 (0–2,5) |
| 50 | 0 | 0 | 0 | 1 (0–2,5) |
| 51 | 0 (0–1) | 0 | 0 | 1 (0–2,5) |
| 52 | 0 (0–1) | 0 | 0 | 1 (0–2,5) |
| 53 | 0 (0–1) | 0 | 0 | 1 (0–2,5) |
| 54 | 0 (0–1) | 0 | 0 | 1,5 (0,5–2,5) |
| 55 | 0 (0–1) | 0 | 0 (0–0,5) | 1,5 (0,5–2) |
| 56 | 0 (0–0,5) | 0 | 0 | 1 (0,5–2) |
| 57 | 0 (0–1) | 0 | 0 | 1 (0,5–2) |
| 58 | 0 (0–1) | 0 | 0 | 1 (0,5–2) |
| 59 | 0 (0–0,5) | 0 | 0 | 1 (0,5–2) |
| 60 | 0 (0–1) | 0 | 0 | 1 (0,5–2) |

### Canavarlar (1/2)

| Yıl | Yaşayan kamp (yıl sonu) | Yaşayan kamp (yıl ort.) | Doğan kamp | Temizlenen kamp | Canavar baskını | Yaşayan trol ini (yıl sonu) |
|---|---|---|---|---|---|---|
| 1 | 7 (6–7) | 5,3 (4,98–5,5) | 4 (3–4) | 0 | 0 | 0 |
| 2 | 8 | 7,78 (7,56–7,93) | 1 (1–2) | 0 | 0 | 0 |
| 3 | 8 | 8 | 0 | 0 | 0 | 0 |
| 4 | 8 (8–9) | 8 (8–8,39) | 0 (0–1) | 0 | 4 (3–5) | 0 |
| 5 | 8 (8–9) | 8 (8–9) | 0 (0–1) | 0 | 7 (5,5–9) | 0 |
| 6 | 8 (8–9) | 8,25 (8–9,01) | 0 (0–0,5) | 0 (0–0,5) | 7 (5–8,5) | 0 |
| 7 | 9 (8–9,5) | 8,9 (8–9,03) | 0 (0–1,5) | 0 (0–0,5) | 7 (4–7,5) | 0 |
| 8 | 9 (8–9) | 8,88 (7,96–9,3) | 0 (0–1) | 0 (0–1) | 6 (3–7,5) | 0 |
| 9 | 9 (8–9) | 8,64 (8–9) | 1 (0–2) | 1 (0–2) | 6 (4–7) | 0 |
| 10 | 9 (8–9,5) | 8,77 (8,21–9,25) | 1 (0–2,5) | 1 (0–2) | 4,5 (2,5–6) | 0 |
| 11 | 9 (7,5–10) | 8,89 (7,82–9,35) | 2 (0–2,5) | 1,5 (0,5–3) | 5 (4–8,5) | 0 |
| 12 | 9 (7,5–10) | 8,7 (7,95–9,78) | 2 (1–3) | 1,5 (1–3,5) | 5 (2–6,5) | 0 |
| 13 | 8,5 (7–9,5) | 8,72 (7,32–9,32) | 2 (0,5–3) | 2 (1–3) | 3 (2–5,5) | 0 |
| 14 | 9 (7–10) | 8,43 (7,42–9,87) | 1,5 (0–3) | 1 (0–3) | 4 (2–6) | 0 |
| 15 | 9 (8–10) | 8,7 (7,9–9,81) | 1,5 (1–3) | 1,5 (0–3,5) | 4 (1,5–6) | 0 (0–1) |
| 16 | 9 (8–10) | 8,92 (7,96–9,78) | 1 (0,5–3,5) | 1 (0,5–3) | 3,5 (2–7) | 0 (0–1) |
| 17 | 9 (8–10,5) | 8,7 (7,81–9,93) | 2 (1–3) | 2 (1–3) | 4 (2–5,5) | 0 (0–1,5) |
| 18 | 9 (8–10) | 8,26 (7,19–9,66) | 3 (1–3,5) | 3 (1–4) | 3,5 (3–5) | 1 (0–1,5) |
| 19 | 9 (8–10,5) | 8,88 (7,98–9,95) | 2 (1–3,5) | 1 (1–3,5) | 3,5 (1,5–5) | 0 (0–2) |
| 20 | 10 (8–10,5) | 8,76 (7,93–9,83) | 2 (1–4,5) | 2,5 (0,5–3,5) | 4 (2–6,5) | 0 (0–2) |
| 21 | 9 (8,5–10) | 9,56 (8,04–10,1) | 2 (0–3,5) | 2 (1–4) | 4 (2,5–6) | 0 (0–1,5) |
| 22 | 9 (8–10) | 9,01 (8,49–9,87) | 1 (1–4,5) | 2 (0,5–3,5) | 4 (2,5–5,5) | 0,5 (0–1,5) |
| 23 | 9 (8,5–10) | 8,89 (8,3–9,85) | 2 (0,5–3) | 2 (1–3,5) | 3,5 (2–6) | 0 (0–2) |
| 24 | 9 (8–10) | 9,23 (8,5–9,95) | 2 (0,5–3,5) | 2 (0–3) | 4,5 (2–6) | 0 (0–2) |
| 25 | 9 (8–10) | 9,25 (8,38–9,99) | 2,5 (1–4) | 2 (1–4) | 4 (2,5–5) | 0 (0–2) |
| 26 | 9 (9–10) | 9,34 (8,65–10,2) | 2 (0,5–3,5) | 1,5 (0,5–3,5) | 4 (3–6,5) | 1 (0–2) |
| 27 | 9 (8–10,5) | 9,04 (8,55–10,5) | 2,5 (0–3) | 2 (1–4) | 4 (3–5) | 1 (0–2) |
| 28 | 9 (9–11) | 9,2 (8,61–10,4) | 2,5 (1–4) | 2 (0,5–3) | 4 (2,5–6) | 1 (0–2) |
| 29 | 9 (9–10,5) | 8,99 (8,74–10,6) | 1,5 (0,5–2,5) | 1,5 (0,5–3) | 4 (1,5–6) | 1 (0–2) |
| 30 | 9 (8–11) | 8,99 (8,58–10,8) | 1,5 (1–3) | 2 (1–3) | 4,5 (2–7,5) | 1 (0–2,5) |
| 31 | 9,5 (8,5–11) | 9,49 (8,65–10,5) | 2 (1–3) | 1,5 (0,5–3) | 4 (2,5–6,5) | 1 (0–2,5) |
| 32 | 9,5 (8,5–11,5) | 9,43 (8,4–11) | 2 (0–3,5) | 2 (0–3) | 4 (2–5) | 1 (1–2) |
| 33 | 9 (9–10) | 9,46 (8,4–10,8) | 1 (0–3,5) | 2 (0,5–3) | 4 (2–6) | 1,5 (0,5–2,5) |
| 34 | 9 (8–10) | 9,23 (8,61–10,1) | 1 (0–2) | 1,5 (0–3) | 5 (2,5–6) | 1 (0,5–3) |
| 35 | 9 (8–10) | 9,14 (8,4–10) | 1 (1–3) | 2 (0–3) | 4,5 (1,5–6,5) | 1,5 (1–3) |
| 36 | 9 (8–10) | 9,28 (8,63–10) | 1 (0–3,5) | 1,5 (0–2,5) | 4 (1–7) | 1,5 (0,5–3) |
| 37 | 9 (8–11) | 9,07 (8,46–10,5) | 2 (0,5–3,5) | 1 (0,5–2,5) | 4 (2,5–6) | 1,5 (1–3) |
| 38 | 10 (8–11) | 9,33 (8–10,7) | 1,5 (0–3) | 2 (0,5–3) | 4 (2,5–5) | 1 (1–3) |
| 39 | 9,5 (8,5–10) | 9,45 (8,6–10,5) | 2 (0,5–3,5) | 2 (0,5–3) | 4 (2–5) | 1 (0–3) |
| 40 | 9 (9–10) | 9,3 (8,69–10,3) | 1 (0–2,5) | 1 (0–2,5) | 4 (3–6) | 1 (0,5–3) |
| 41 | 9 (8–10) | 9,19 (8,4–9,97) | 2 (0–3) | 2 (0,5–3) | 4 (3–5) | 1,5 (0–3) |
| 42 | 9 (8,5–10,5) | 9 (8,29–10,4) | 1 (0–2,5) | 1 (0–3,5) | 4 (1,5–5) | 2 (0–3) |
| 43 | 10 (7,5–11) | 9,81 (8,29–10,3) | 2 (0–3) | 1 (0–2) | 3,5 (2–6,5) | 1,5 (0–3) |
| 44 | 9,5 (9–11,5) | 10 (7,72–11,1) | 1,5 (0–3) | 1 (0,5–2,5) | 4 (2–6) | 1,5 (0,5–3) |
| 45 | 9 (7,5–10) | 9,5 (8,31–10,6) | 1 (0–3) | 2 (0,5–3,5) | 4 (1,5–5,5) | 2 (0,5–3) |
| 46 | 9 (7–10) | 8,98 (8,03–10,1) | 1 (0–2,5) | 1 (0–3) | 4 (2–7) | 2 (0,5–3) |
| 47 | 9 (8–10,5) | 9 (8,45–10) | 2 (0–3) | 1 (0–2,5) | 4 (2,5–6,5) | 2,5 (0,5–3) |
| 48 | 9 (8,5–11) | 8,99 (8,29–10,4) | 1,5 (0–4) | 2 (0–3) | 3 (2–5) | 2 (0,5–3) |
| 49 | 9,5 (8,5–11) | 9,22 (8,2–10,6) | 2 (0–4) | 1 (0–4,5) | 3 (1–5,5) | 2 (0,5–3) |
| 50 | 9,5 (8,5–11,5) | 9,46 (8,35–11,2) | 1 (0–4,5) | 1 (0–3) | 4 (3–6) | 2 (1–3) |
| 51 | 9 (7–11,5) | 8,95 (8,12–11,1) | 1 (0–3,5) | 2 (0–5) | 4 (3–5) | 2 (0,5–3) |
| 52 | 9 (9–10) | 9,33 (8,2–10,2) | 2 (0,5–4,5) | 2 (0,5–4) | 4 (1,5–5) | 2 (0–3) |
| 53 | 9 (8–10) | 9 (8,45–9,77) | 1 (0–3) | 1 (0–4,5) | 3,5 (1–6) | 2 (0,5–3) |
| 54 | 9 (7,5–9,5) | 8,87 (7,68–9,9) | 1 (0,5–3,5) | 2 (1–4) | 3,5 (2–5,5) | 2 (0,5–3) |
| 55 | 9 (8–10) | 8,8 (8,01–9,75) | 1,5 (0,5–3,5) | 1,5 (0,5–3) | 3,5 (2,5–6,5) | 2 (0,5–3) |
| 56 | 9 (8–10,5) | 8,98 (8,13–9,7) | 2,5 (0–4) | 1,5 (0–4) | 3 (1–5,5) | 2 (1–3) |
| 57 | 9 (8–11,5) | 9,05 (8,26–10,9) | 1 (0–4) | 1 (0–4) | 3 (2,5–5,5) | 2 (0,5–3) |
| 58 | 9 (8–11,5) | 9,18 (8,15–11,2) | 2 (0,5–4) | 2 (1–4) | 4,5 (2,5–6) | 2 (0–3) |
| 59 | 9 (7,5–10,5) | 8,92 (8,14–10,3) | 2 (0–3,5) | 2 (0,5–3,5) | 4 (1–6) | 2 (0–3) |
| 60 | 9 (6,5–10) | 8,96 (7,37–10,7) | 1 (0,5–4) | 2 (1–4,5) | 3,5 (2–5) | 2 (0–3) |

### Canavarlar (2/2)

| Yıl | Yaşayan ejderha (yıl sonu) | Ejderha akını | Kriz (anlatıcı) | Rahatlama dönemi (anlatıcı) |
|---|---|---|---|---|
| 1 | 0 | 0 | 0 | 0 |
| 2 | 0 | 0 | 0 | 0 |
| 3 | 0 | 0 | 0 | 0 |
| 4 | 0 | 0 | 0 | 0 |
| 5 | 0 | 0 | 0 (0–1) | 1 |
| 6 | 0 | 0 | 0 | 0 |
| 7 | 0 | 0 | 0 | 1 (0–1) |
| 8 | 0 | 0 | 0 | 0 (0–1) |
| 9 | 0 | 0 | 0 | 0 (0–1) |
| 10 | 0 | 0 | 0 | 0 |
| 11 | 0 | 0 | 0 (0–1) | 0 (0–1) |
| 12 | 0 | 0 | 0 (0–1) | 0 |
| 13 | 0 | 0 | 0 (0–0,5) | 0 |
| 14 | 0 | 0 | 0,5 (0–1) | 0 |
| 15 | 0 | 0 | 0 (0–1) | 0 |
| 16 | 0 | 0 | 0,5 (0–1) | 0 |
| 17 | 0 | 0 | 0 (0–1) | 0 |
| 18 | 0 (0–1) | 0 (0–0,5) | 0 (0–1) | 0 (0–0,5) |
| 19 | 0 (0–1) | 0 (0–1) | 0 (0–1) | 0 |
| 20 | 1 (0–1) | 0 (0–1,5) | 0 (0–1) | 0 (0–1) |
| 21 | 1 | 1 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 22 | 1 | 1 (0–1,5) | 0 (0–1) | 0 (0–1) |
| 23 | 1 | 1 (0–1,5) | 0 (0–1) | 0 (0–1) |
| 24 | 1 | 1 (0,5–2) | 0 (0–1) | 0 (0–1) |
| 25 | 1 | 1 (0–1,5) | 0 (0–1) | 0 |
| 26 | 1 | 1 (1–2) | 0 (0–0,5) | 0 (0–1) |
| 27 | 1 | 1 (1–2) | 0 | 0 (0–0,5) |
| 28 | 1 | 1 | 0 (0–1) | 0 |
| 29 | 1 | 1 (0–1,5) | 0 (0–1) | 0 |
| 30 | 1 | 1 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 31 | 1 | 1 (1–2) | 0 (0–1) | 0 (0–1) |
| 32 | 1 | 1 (0–1) | 0 (0–1) | 0 |
| 33 | 1 | 1 (0–1,5) | 0 (0–1) | 0 |
| 34 | 1 | 1 (0–2) | 0 (0–1) | 0 |
| 35 | 1 | 1 (0–1) | 0 (0–1) | 0 |
| 36 | 1 | 1 (0–1,5) | 0 (0–1) | 0 (0–0,5) |
| 37 | 1 | 1 (0–1,5) | 0 (0–1) | 0 (0–0,5) |
| 38 | 1 (0,5–1) | 1 (0–1) | 0 | 0 (0–1) |
| 39 | 1 (0,5–1) | 1 (0–2) | 0 (0–1) | 0 |
| 40 | 1 (0,5–1) | 0,5 (0–1) | 0 (0–1) | 0 |
| 41 | 1 (0,5–1) | 1 (0–1) | 0 (0–1) | 0 |
| 42 | 1 (0,5–1) | 1 (0–1,5) | 0 (0–1) | 0 |
| 43 | 1 (0,5–1) | 1 (0–2) | 0,5 (0–1) | 0 |
| 44 | 1 (0,5–1) | 1 (0–1) | 0 (0–1) | 0 |
| 45 | 1 (0,5–1) | 1 (0–1,5) | 0 (0–1) | 0 (0–0,5) |
| 46 | 1 (0,5–1) | 1 (0–1) | 0 (0–1) | 0 |
| 47 | 1 (0,5–1) | 1 (0–2) | 0 (0–1) | 0 |
| 48 | 1 (0,5–1) | 1 (0–1) | 0,5 (0–1) | 0 |
| 49 | 1 (0,5–1) | 0,5 (0–2) | 0 (0–0,5) | 0 |
| 50 | 1 (0,5–1) | 1 (0–1,5) | 0,5 (0–1) | 0 |
| 51 | 1 (0,5–1) | 1 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 52 | 1 (0,5–1) | 1 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 53 | 1 (0,5–1) | 1 (0–1,5) | 0 (0–1) | 0 |
| 54 | 1 (0,5–1) | 1 (0–1,5) | 0 (0–1) | 0 |
| 55 | 1 (0,5–1) | 1 (0–2) | 0 (0–1) | 0 (0–1) |
| 56 | 1 (0,5–1) | 0,5 (0–1,5) | 0 (0–1) | 0 |
| 57 | 1 (0,5–1) | 1 (0–1) | 0 (0–1) | 0 (0–0,5) |
| 58 | 1 (0–1) | 1 (0–1,5) | 1 (0–1) | 0 |
| 59 | 1 (0–1) | 1 (0–1,5) | 0 (0–1) | 0 |
| 60 | 1 (0–1) | 1 (0–2) | 0 (0–1) | 0 (0–0,5) |

### Kahramanlar (1/2)

| Yıl | Doğan kahraman | Ölen kahraman | Emekli olan kahraman | Diyarı terk eden kahraman | Ölümden dönen kahraman | Efsane olan kahraman |
|---|---|---|---|---|---|---|
| 1 | 0 (0–0,5) | 0 | 0 | 0 | 0 | 0 |
| 2 | 1 (0–2) | 0 | 0 | 0 | 0 | 0 |
| 3 | 1 (1–5) | 0 | 0 | 0 | 0 | 0 |
| 4 | 2 (1–3) | 0 (0–0,5) | 0 | 0 (0–1) | 0 | 0 |
| 5 | 4 (2,5–5) | 0 (0–0,5) | 0 | 0 (0–1) | 0 | 0 |
| 6 | 3 (2–5,5) | 0 (0–1,5) | 0 | 0 | 0 | 0 |
| 7 | 3,5 (3–7) | 0 (0–1,5) | 0 | 0 (0–1) | 0 | 0 |
| 8 | 3 (1,5–6,5) | 0 (0–2) | 0 | 0 (0–0,5) | 0 | 0 |
| 9 | 4 (1–7) | 0 (0–2) | 0 | 0 (0–1,5) | 0 | 0 |
| 10 | 3 (1,5–4,5) | 1,5 (0–2,5) | 0 | 0 | 0 | 0 |
| 11 | 3 (1,5–5,5) | 0,5 (0–2) | 0 | 0 | 0 | 0 |
| 12 | 3,5 (1,5–6) | 1 (0–5,5) | 0 | 0 (0–0,5) | 0 | 0 |
| 13 | 2 (0,5–5) | 1 (0–2,5) | 0 | 0 (0–0,5) | 0 | 0 |
| 14 | 3 (1–5,5) | 1 (0–3,5) | 0 | 0 (0–1) | 0 | 0 |
| 15 | 3 (1–5,5) | 1 (0–2,5) | 0 | 0 | 0 | 0 |
| 16 | 2 (0–5) | 1 (0–3,5) | 0 | 0 | 0 | 0 (0–0,5) |
| 17 | 3 (1–5) | 0 (0–3) | 0 | 0 | 0 | 0 |
| 18 | 2 (0,5–4) | 0,5 (0–3,5) | 0 | 0 | 0 | 0 (0–0,5) |
| 19 | 1,5 (0–4,5) | 0 (0–2) | 0 | 0 (0–1) | 0 | 0 |
| 20 | 3 (0,5–5) | 0,5 (0–2) | 0 | 0 | 0 | 0 (0–0,5) |
| 21 | 3 (0,5–3) | 0 (0–2) | 0 | 0 | 0 | 0 (0–0,5) |
| 22 | 4 (2–7) | 1 (0–4) | 0 | 0 | 0 | 0 |
| 23 | 3 (1–6) | 1 (0–3,5) | 0 | 0 (0–1) | 0 | 0 (0–1) |
| 24 | 4 (0,5–5) | 1 (0–5) | 0 | 0 | 0 | 0 (0–0,5) |
| 25 | 3,5 (0,5–5,5) | 0 (0–3,5) | 0 | 0 | 0 | 0 |
| 26 | 4 (3–7,5) | 0 (0–1,5) | 0 | 0 (0–0,5) | 0 | 0 |
| 27 | 3,5 (2–6,5) | 0,5 (0–4) | 0 | 0 (0–1) | 0 | 0 (0–1) |
| 28 | 4 (2–6,5) | 1 (0–2,5) | 0 (0–1) | 0 (0–1) | 0 | 0 (0–1) |
| 29 | 3 (1–5) | 2 (0–4,5) | 0 | 0 (0–1) | 0 | 0 (0–1) |
| 30 | 3 (2–4,5) | 1 (0–3,5) | 0 (0–1) | 0 (0–1) | 0 | 0 (0–1) |
| 31 | 3,5 (1–5,5) | 1 (0–5,5) | 0 | 0,5 (0–1,5) | 0 | 0 (0–1) |
| 32 | 2,5 (1–4) | 0,5 (0–3) | 0 (0–1,5) | 0 (0–1,5) | 0 | 0 (0–0,5) |
| 33 | 3 (1,5–6) | 0 (0–2) | 0 (0–1) | 0 (0–1,5) | 0 | 0 |
| 34 | 3 (0,5–4,5) | 1 (0–3) | 0 (0–0,5) | 0 (0–2,5) | 0 | 0 |
| 35 | 3,5 (1–5,5) | 1 (0–5) | 0 (0–1) | 0 (0–1) | 0 | 0 (0–1) |
| 36 | 2 (1–3,5) | 1 (0–5) | 0 (0–1,5) | 1 (0–1,5) | 0 | 0 |
| 37 | 3 (2–5) | 2 (0–5,5) | 0 (0–1) | 1 (0–1) | 0 | 0 (0–0,5) |
| 38 | 2,5 (1,5–5) | 1,5 (0–5,5) | 0,5 (0–1) | 0 (0–2) | 0 | 0 (0–0,5) |
| 39 | 3 (1–4,5) | 0 (0–5) | 0,5 (0–2) | 0 (0–2,5) | 0 | 0 |
| 40 | 4 (2–8,5) | 1 (0–3) | 1 (0–1,5) | 1 (0–1) | 0 | 0 |
| 41 | 3 (1–6) | 0,5 (0–4) | 1 (0–2) | 0 (0–1) | 0 | 0 |
| 42 | 3 (1–6) | 0,5 (0–3) | 1 (0–2,5) | 0,5 (0–1) | 0 | 0 |
| 43 | 3 (1,5–4) | 0 (0–5) | 0 (0–1,5) | 0 (0–1) | 0 | 0 |
| 44 | 3 (1–4) | 1 (0–3) | 0 (0–1,5) | 0 (0–1,5) | 0 | 0 |
| 45 | 3 (1–6) | 1 (0–6,5) | 0,5 (0–2,5) | 0,5 (0–2) | 0 | 0 (0–0,5) |
| 46 | 4 (2,5–6,5) | 1,5 (0–3,5) | 1 (0–2,5) | 0 (0–1,5) | 0 | 0 (0–0,5) |
| 47 | 4,5 (1–5,5) | 2 (0,5–5,5) | 0 (0–1,5) | 0,5 (0–1,5) | 0 | 0 (0–1) |
| 48 | 3 (1–5) | 1 (0–3) | 0 (0–2) | 1 (0–3) | 0 | 0 (0–0,5) |
| 49 | 3 (2–6) | 1,5 (0–4,5) | 1 (0–3) | 0,5 (0–1,5) | 0 | 0 (0–0,5) |
| 50 | 3,5 (1–5,5) | 1 (0–6) | 1 (0–1) | 0 (0–1,5) | 0 | 0 (0–0,5) |
| 51 | 4 (1–8,5) | 0,5 (0–4,5) | 0 (0–2) | 0,5 (0–2) | 0 | 0 (0–1) |
| 52 | 3,5 (1,5–6,5) | 1 (0–3,5) | 0 (0–1) | 0 (0–3) | 0 | 0 (0–1) |
| 53 | 3,5 (1,5–7,5) | 0,5 (0–3,5) | 1 (0–2) | 0 (0–1,5) | 0 | 0 (0–1) |
| 54 | 3,5 (1,5–5) | 1 (0–4,5) | 1 (0–2) | 0 (0–1,5) | 0 | 0 |
| 55 | 3,5 (1–5,5) | 2,5 (0–5,5) | 1,5 (0–2) | 0,5 (0–3) | 0 | 0 |
| 56 | 3 (1,5–6) | 1 (0–3,5) | 0 (0–2) | 0 (0–2,5) | 0 | 0 |
| 57 | 3,5 (1–5,5) | 2,5 (0–8) | 1 (0–2) | 0,5 (0–2,5) | 0 | 0 (0–1,5) |
| 58 | 3,5 (1,5–8) | 1,5 (0–5) | 1 (0–2) | 0 (0–1) | 0 | 0 (0–1) |
| 59 | 3 (1–5,5) | 1 (0–4) | 0 (0–2) | 0 (0–1) | 0 | 0 (0–0,5) |
| 60 | 4,5 (0,5–7) | 1 (0–6,5) | 1 (0–2) | 1 (0–2) | 0 | 0 (0–1) |

### Kahramanlar (2/2)

| Yıl | Yaşayan kahraman (yıl sonu) | Doğuş seviyesi (ort.) | Ölüm seviyesi (ort.) | Yaşayan kahraman seviyesi (ort.) | En yüksek seviye (şimdiye dek) |
|---|---|---|---|---|---|
| 1 | 0 (0–0,5) | 1,5 (1,1–1,9) | – | 1,5 (1,1–1,9) | 0 (0–0,5) |
| 2 | 1 (0–2) | 1 (1–1,2) | – | 1 (1–1,1) | 1 (0–1) |
| 3 | 3 (1–5) | 1,4 (1–2) | – | 1,33 (1–1,58) | 2 (1–2) |
| 4 | 5 (2–8) | 1,33 (1–2) | 1 | 1,37 (1,07–1,67) | 2 (1,5–2) |
| 5 | 8,5 (6–12,5) | 1,25 (1–1,7) | 1,5 (1,1–1,9) | 1,33 (1,19–1,58) | 2 |
| 6 | 12 (8–14,5) | 1,27 (1–1,5) | 1,75 (1,15–2) | 1,4 (1,15–1,54) | 2 |
| 7 | 15 (12–17) | 1,33 (1–1,61) | 1,17 (1–1,45) | 1,39 (1,22–1,58) | 2 (2–2,5) |
| 8 | 18 (14–22) | 1,27 (1–1,75) | 1 (1–1,25) | 1,42 (1,28–1,67) | 2 (2–3) |
| 9 | 20,5 (17–26,5) | 1,25 (1,06–1,56) | 1 (1–1,38) | 1,5 (1,29–1,78) | 3 (2–3) |
| 10 | 22,5 (19–27,5) | 1 (1–1,58) | 1,25 (1–2) | 1,6 (1,3–1,79) | 3 |
| 11 | 25 (20–30,5) | 1,18 (1–1,75) | 1 (1–1,77) | 1,74 (1,42–1,98) | 3,5 (3–4) |
| 12 | 26,5 (22,5–33) | 1,5 (1,06–2) | 1,13 (1–1,97) | 1,87 (1,6–2,13) | 4 (3–4) |
| 13 | 27,5 (24,5–34,5) | 1,29 (1–1,5) | 1,5 (1,27–2,2) | 1,93 (1,69–2,26) | 4 (3–4) |
| 14 | 29,5 (25,5–37) | 1,13 (1–1,5) | 1,5 (1–2) | 2,05 (1,68–2,43) | 4 (3–4) |
| 15 | 31 (27,5–39,5) | 1,29 (1–1,87) | 1 (1–1,4) | 2,2 (1,69–2,49) | 4 (4–5) |
| 16 | 31 (28,5–42) | 1,33 (1–1,73) | 1,33 (1–3,2) | 2,26 (1,84–2,49) | 4 (4–5) |
| 17 | 32,5 (30–44,5) | 1 (1–1,75) | 1,67 (1–3,27) | 2,35 (1,9–2,66) | 4 (4–5) |
| 18 | 33,5 (30,5–46) | 1,13 (1–1,5) | 1,29 (1–2,3) | 2,47 (1,98–2,92) | 5 (4–5) |
| 19 | 35 (31–48) | 1,4 (1–2) | 1,25 (1–2,38) | 2,54 (2–2,98) | 5 (4–5,5) |
| 20 | 38,5 (32,5–48) | 1,29 (1–1,57) | 1,75 (1–2,6) | 2,69 (2,16–3,02) | 5 (4–6) |
| 21 | 39 (34,5–51,5) | 1,27 (1–1,45) | 2 (1,25–2,5) | 2,7 (2,3–3,07) | 5 (4–6) |
| 22 | 41 (35–53) | 1,23 (1–1,5) | 2 (1–2,6) | 2,76 (2,32–3,1) | 5 (4,5–6) |
| 23 | 44,5 (34–53,5) | 1,1 (1–1,55) | 2,67 (1,27–3,4) | 2,89 (2,35–3,1) | 5 (4,5–6) |
| 24 | 47,5 (35–54) | 1,23 (1–1,57) | 3 (1–3,19) | 2,88 (2,29–3,09) | 5,5 (5–7) |
| 25 | 46,5 (39,5–58) | 1,1 (1–1,5) | 3 (2,42–4,67) | 2,88 (2,31–3,17) | 6 (5–7) |
| 26 | 48 (41,5–63) | 1,27 (1–1,71) | 2,5 (1–4,2) | 2,8 (2,36–3,19) | 6 (5–7) |
| 27 | 49,5 (43,5–65) | 1,27 (1–1,47) | 2,55 (2,03–3,72) | 2,83 (2,41–3,29) | 6 (5–7,5) |
| 28 | 52 (46–68,5) | 1,2 (1–1,54) | 2 (1,4–3,8) | 2,82 (2,48–3,31) | 6 (5–7,5) |
| 29 | 54 (46,5–70,5) | 1,33 (1–1,5) | 2,63 (1,68–4,05) | 2,98 (2,47–3,33) | 6 (5–7,5) |
| 30 | 55,5 (46,5–71) | 1,25 (1–1,55) | 2 (1–4,55) | 3 (2,51–3,45) | 6,5 (5–8) |
| 31 | 57,5 (49–73) | 1,38 (1–1,88) | 3 (1–3,37) | 3,11 (2,54–3,5) | 7 (5–8) |
| 32 | 56,5 (49–73,5) | 1,29 (1–2) | 2,5 (1,23–3,65) | 3,17 (2,63–3,55) | 7 (5,5–8) |
| 33 | 58,5 (52,5–72,5) | 1,24 (1–1,67) | 2 (1,5–2,75) | 3,24 (2,56–3,66) | 7 (6–8) |
| 34 | 60,5 (53–74,5) | 1,33 (1–1,5) | 3 (1,27–5,73) | 3,31 (2,64–3,56) | 7 (6–8) |
| 35 | 61,5 (53–75) | 1 (1–1,58) | 2,4 (1,57–3,98) | 3,3 (2,64–3,57) | 7,5 (6–8) |
| 36 | 61 (51–73,5) | 1 (1–1,46) | 3 (1,8–5,26) | 3,22 (2,69–3,64) | 7,5 (6–8) |
| 37 | 60 (52–72,5) | 1,5 (1–2) | 2,63 (1,1–4,45) | 3,37 (2,76–3,62) | 7,5 (6–8) |
| 38 | 60,5 (54,5–71,5) | 1,5 (1,1–1,83) | 2 (1,75–3,67) | 3,38 (2,77–3,63) | 7,5 (6–9) |
| 39 | 61 (52–69) | 1,33 (1–1,67) | 2,33 (1,9–3,8) | 3,28 (2,8–3,6) | 8 (6–9) |
| 40 | 63,5 (56,5–73) | 1,44 (1,2–1,87) | 2 (1–4) | 3,31 (2,87–3,58) | 8 (6–9) |
| 41 | 62,5 (58,5–75) | 1,5 (1–1,77) | 1,88 (1–3,83) | 3,3 (2,86–3,59) | 8 (6–9) |
| 42 | 63,5 (58,5–72) | 1,33 (1–1,7) | 2,75 (1,67–4,4) | 3,27 (2,79–3,62) | 8 (6–9) |
| 43 | 63,5 (60–73) | 1,33 (1–1,83) | 2,33 (2,11–3,49) | 3,23 (2,84–3,64) | 8 (6–9) |
| 44 | 66 (60–75) | 1,5 (1–2) | 3 (1,87–4,12) | 3,18 (2,82–3,63) | 8 (6–9) |
| 45 | 66,5 (57–72,5) | 1,33 (1–1,9) | 3 (1,5–4,3) | 3,21 (2,92–3,65) | 8 (6–9) |
| 46 | 63,5 (59,5–74) | 1,4 (1–1,72) | 2,5 (1,9–3,73) | 3,14 (2,89–3,63) | 8 (6–9,5) |
| 47 | 64,5 (58,5–75,5) | 1,35 (1–1,76) | 2 (1–3,86) | 3,15 (2,79–3,71) | 8 (6,5–9,5) |
| 48 | 63,5 (58–76,5) | 1,33 (1–2) | 2 (1,27–4,8) | 3,15 (2,8–3,73) | 8 (6,5–9,5) |
| 49 | 64,5 (56,5–77) | 1,33 (1–1,78) | 2,33 (1,96–5,2) | 3,2 (2,83–3,74) | 8 (7–9,5) |
| 50 | 66 (57–77,5) | 1,17 (1–1,6) | 3 (1–4,23) | 3,16 (2,81–3,8) | 8 (7–9,5) |
| 51 | 65 (59–79) | 1,36 (1–2) | 2 (1,23–3,15) | 3,15 (2,75–3,87) | 8 (7–10) |
| 52 | 66 (62–78,5) | 1,29 (1–1,67) | 3 (2–3,67) | 3,15 (2,87–3,92) | 8 (7–10) |
| 53 | 70 (63–80,5) | 1,4 (1,13–1,75) | 3,75 (2,47–5) | 3,12 (2,8–4,03) | 8 (7–10) |
| 54 | 70 (61,5–84) | 1,5 (1–1,92) | 2,9 (1,95–4,53) | 3,19 (2,81–3,95) | 8,5 (7–10) |
| 55 | 69,5 (60–80) | 1,33 (1–1,92) | 3 (2–4) | 3,31 (2,73–3,91) | 8,5 (7–10) |
| 56 | 70 (62–81) | 1,27 (1–1,58) | 3 (1,77–5,1) | 3,28 (2,71–3,93) | 9 (7–10) |
| 57 | 69,5 (62–78,5) | 1,37 (1–2) | 3 (2,15–4,39) | 3,37 (2,7–3,94) | 9 (7–10) |
| 58 | 69,5 (62,5–81) | 1,4 (1–2) | 2,4 (1,89–4,8) | 3,42 (2,79–3,86) | 9 (7–10) |
| 59 | 68,5 (63,5–83) | 1,37 (1–1,9) | 3,17 (2,3–4,13) | 3,42 (2,77–4,01) | 9 (7–10) |
| 60 | 68,5 (62,5–85) | 1,17 (1–1,5) | 3 (2,53–4,03) | 3,37 (2,71–4,07) | 9 (7–10) |

### Han ve ticaret

| Yıl | Ayakta han | Asılan ilan | Biten ilan | Ticaret seferi (kervan) | İkmal seferi |
|---|---|---|---|---|---|
| 1 | 3 (1,5–4) | 0,5 (0–1) | 0 | 0 | 0 |
| 2 | 4 (3,5–4) | 0 (0–1,5) | 0 | 0 | 0 |
| 3 | 4 (3,5–4) | 0 (0–1,5) | 0 | 1 (0–6,5) | 0 |
| 4 | 4 (3–4) | 1 (0–3,5) | 0 | 8 (1–16) | 0 |
| 5 | 3,5 (3–4) | 2 (2–4) | 0 | 11 (6–23) | 0 |
| 6 | 3 (2–4) | 2 (0,5–3,5) | 0 | 15 (8–34,5) | 0 |
| 7 | 3 (2–4) | 2 (1–3) | 0 (0–0,5) | 19,5 (12–40) | 0 |
| 8 | 3 (2–4) | 3 (1–4) | 0 (0–1) | 25 (15–43,5) | 0 (0–1) |
| 9 | 3 (2–4) | 2 (1–4) | 1 (0–2) | 34 (16–48,5) | 0 (0–4) |
| 10 | 3 (1,5–4) | 2 (0,5–5) | 1 (0–2) | 36,5 (17,5–50,5) | 1,5 (0–7,5) |
| 11 | 3 (2–4) | 3 (0,5–4,5) | 0,5 (0–3) | 36,5 (19,5–51,5) | 2,5 (0–13,5) |
| 12 | 3 (2–4) | 2,5 (1–4) | 1 (0–2) | 39,5 (23–54,5) | 6 (1–18,5) |
| 13 | 4 (3–5) | 2 (1–3,5) | 1 (0–2) | 41 (29,5–58,5) | 9 (1,5–20) |
| 14 | 4 (3,5–5) | 2 (1–3,5) | 0 (0–1,5) | 42 (32,5–61) | 11,5 (2,5–21) |
| 15 | 5 (4–5) | 2 (0–4) | 0,5 (0–2,5) | 44,5 (30–63,5) | 12,5 (5,5–22) |
| 16 | 4 (4–5) | 1,5 (0–3) | 0,5 (0–2) | 48 (36,5–66,5) | 13,5 (8–22) |
| 17 | 5 (4–5) | 2 (1–3,5) | 1 (0–1) | 50,5 (37–68) | 15,5 (9–22,5) |
| 18 | 5 (4–5) | 3 (1–6,5) | 1 (0–2,5) | 51 (38,5–70) | 15 (9,5–24) |
| 19 | 5 (4–6) | 2 (1–3) | 0 (0–1,5) | 52,5 (38,5–73,5) | 17 (8–23,5) |
| 20 | 5 (4–6) | 2,5 (0,5–4) | 1 (0–1,5) | 54 (38,5–79) | 17 (9,5–25) |
| 21 | 5 (4–6) | 3 (1,5–6) | 0 (0–1,5) | 54 (38–82) | 18 (9,5–24,5) |
| 22 | 5 (4–6) | 3 (1–6) | 1 (0–2) | 55 (37,5–84,5) | 19,5 (9–26) |
| 23 | 5 (4–6,5) | 2 (1–4) | 0 (0–1,5) | 54,5 (38–84) | 22 (10–27,5) |
| 24 | 5 (4–6,5) | 3 (1–4) | 0,5 (0–1) | 57 (38–90) | 20 (10–29) |
| 25 | 5 (5–6,5) | 4 (0,5–6,5) | 1 (0–2) | 57,5 (37,5–84,5) | 22,5 (12–32) |
| 26 | 5 (4,5–7) | 3,5 (1–6) | 0 (0–1,5) | 59 (38–88,5) | 21 (13,5–31) |
| 27 | 5 (5–7) | 3,5 (2–6,5) | 0,5 (0–2) | 62,5 (39,5–89,5) | 24 (14,5–32,5) |
| 28 | 5 (4,5–7) | 3,5 (2–4) | 1 (0–1,5) | 62 (40,5–87) | 22,5 (16–31,5) |
| 29 | 5 (4–6,5) | 3 (1–5) | 1 (0–1,5) | 72,5 (42,5–89) | 26,5 (16–32) |
| 30 | 5 (4–6,5) | 2 (1–4,5) | 1 (0–1,5) | 65 (42,5–94,5) | 26,5 (17–35,5) |
| 31 | 5 (4–6,5) | 3 (1–5,5) | 1 (0–1,5) | 69 (41,5–102) | 27,5 (15,5–34,5) |
| 32 | 5 (5–6,5) | 2,5 (1–4) | 0 (0–2) | 79 (44–97,5) | 27,5 (16,5–35) |
| 33 | 5 (5–6,5) | 3 (1–6) | 1 (0–2,5) | 80 (44,5–107) | 27 (14,5–35,5) |
| 34 | 5 (5–6,5) | 2,5 (1,5–5,5) | 0,5 (0–2,5) | 79 (44,5–112) | 27,5 (16,5–35,5) |
| 35 | 5 (5–6,5) | 3 (1–5) | 1 (0–2) | 76,5 (44,5–112) | 27,5 (17,5–36,5) |
| 36 | 5 (4,5–6,5) | 3 (1,5–4) | 1 (0–2) | 81,5 (44–112) | 27 (16,5–37,5) |
| 37 | 5 (4,5–6) | 3,5 (1–5,5) | 1 (0–2) | 78 (44–110) | 27 (16,5–42) |
| 38 | 5 (4,5–6) | 3 (1–6) | 0,5 (0–1,5) | 81 (43,5–114) | 27,5 (15–41,5) |
| 39 | 5 (4,5–6) | 2 (0–4) | 0 (0–1,5) | 78 (44–118) | 28,5 (17–40,5) |
| 40 | 5 (4,5–6) | 2,5 (1–4) | 0 (0–2) | 80 (45–114) | 27,5 (15,5–43) |
| 41 | 5 (5–6,5) | 3 (1–5) | 1 (0–1,5) | 79 (46,5–120) | 28 (17–41,5) |
| 42 | 5 (5–6,5) | 2 (1–5,5) | 1 (0–2) | 84 (46–124) | 28 (17–44,5) |
| 43 | 5 (5–6,5) | 2 (1–5,5) | 0 (0–2,5) | 84,5 (45–128) | 28,5 (16–44) |
| 44 | 5 (5–6,5) | 3 (0,5–4) | 1 (0–1,5) | 81 (46–133) | 27,5 (15–47) |
| 45 | 5 (5–6,5) | 3 (1,5–5,5) | 1 (0–2) | 77,5 (44,5–134) | 30 (15,5–47) |
| 46 | 5 (5–6,5) | 2,5 (0,5–6,5) | 1 (0–2,5) | 81 (44,5–134) | 32,5 (14–49) |
| 47 | 5 (5–6) | 3,5 (1–6) | 1 (0–2) | 81 (44,5–138) | 31 (14,5–47) |
| 48 | 5 (5–6) | 2 (1–5,5) | 1 (0–2,5) | 87 (46,5–126) | 32,5 (14,5–47,5) |
| 49 | 5 (5–6,5) | 3,5 (1–5) | 0,5 (0–2,5) | 82 (47–136) | 31 (13,5–45,5) |
| 50 | 5 (5–6) | 2,5 (1–7,5) | 1 (0–2,5) | 87 (48,5–142) | 33,5 (15–41,5) |
| 51 | 5 (4,5–6,5) | 3 (1–9) | 0,5 (0–4) | 86 (49,5–140) | 31,5 (14,5–41) |
| 52 | 5 (4–6,5) | 2 (1–7,5) | 1 (0–2) | 88 (50,5–144) | 32,5 (15–45,5) |
| 53 | 5 (4–7) | 3,5 (1–6) | 0 (0–2) | 83,5 (50–133) | 32 (14,5–42,5) |
| 54 | 5,5 (5–7) | 3,5 (1–6,5) | 1 (0–4,5) | 95,5 (53–136) | 32 (13,5–43,5) |
| 55 | 5,5 (5–7) | 2,5 (0–7) | 1 (0–3) | 93 (55–142) | 30,5 (14,5–46,5) |
| 56 | 5 (4,5–7) | 3 (1,5–6,5) | 1 (0–2) | 96 (54–144) | 28,5 (14–48) |
| 57 | 5 (4,5–6,5) | 3 (1–9) | 0 (0–3) | 96,5 (53–148) | 30,5 (12,5–47,5) |
| 58 | 5 (4,5–6,5) | 3 (1,5–8,5) | 1 (0–3) | 94,5 (54,5–144) | 30,5 (11,5–46,5) |
| 59 | 5 (4,5–6,5) | 2 (1–10,5) | 1 (0–3,5) | 96,5 (53,5–150) | 33 (11–50) |
| 60 | 5 (4,5–7) | 3 (0,5–10) | 1 (0–4,5) | 90 (50,5–144) | 34 (12–47,5) |

### Altın ve ambar (1/3)

| Yıl | Altın p90 (medeniyetler) | Bakım gideri (altın; asker, kahraman, L2–L3) | Kamu işlerine (imar) harcanan altın | Ambarla beslenen amele tayını (gıda) | Kamu işlerindeki (amele) iş gücü payı | İmar ortalaması (köy+, 0–100) |
|---|---|---|---|---|---|---|
| 1 | 25,9 (12,3–35,8) | 0 | 0 | 0 | %0 | – |
| 2 | 67 (43,5–77,5) | 0,47 (0,03–0,86) | 0 | 0 | %0 | 0 |
| 3 | 83,4 (52,6–117) | 6,05 (3,29–11,1) | 0 | 0 | %0 | 0 |
| 4 | 97,4 (69,9–133) | 16,7 (9,94–34,2) | 0 | 0 | %0 | 0 |
| 5 | 110 (70,1–152) | 37,4 (19,7–83,6) | 0 (0–0,08) | 0 (0–2,1) | %0 (%0–%0,1) | 0 |
| 6 | 142 (59,6–175) | 77 (47,1–132) | 0 (0–11,9) | 0 (0–11,2) | %0 (%0–%0,6) | 0 (0–0,35) |
| 7 | 142 (68,5–204) | 143 (93,8–197) | 0 (0–1,1) | 0,5 (0–52,4) | %0,1 (%0–%1,2) | 0 (0–0,36) |
| 8 | 153 (109–202) | 221 (150–293) | 0,36 (0–21,3) | 12,9 (0,4–104) | %0,6 (%0–%2,1) | 0,04 (0–0,47) |
| 9 | 191 (105–252) | 280 (217–406) | 20 (0,06–84,7) | 74,7 (22,2–183) | %1,7 (%0,8–%2,7) | 0,55 (0–1,14) |
| 10 | 265 (104–388) | 359 (258–492) | 60,9 (3,21–155) | 162 (66–489) | %2,9 (%1,3–%5,2) | 1,15 (0,07–2,15) |
| 11 | 328 (160–471) | 466 (340–579) | 140 (1,78–243) | 332 (172–707) | %5 (%2,4–%7,6) | 2,16 (0,36–3,83) |
| 12 | 425 (276–620) | 537 (392–700) | 291 (37,8–522) | 479 (330–991) | %7,4 (%4,3–%10) | 4,02 (0,65–6,73) |
| 13 | 459 (281–633) | 658 (454–797) | 424 (124–603) | 791 (433–1102) | %9,6 (%7,2–%12) | 6,16 (2,41–9,44) |
| 14 | 523 (397–761) | 726 (504–921) | 518 (189–840) | 1181 (737–1417) | %12 (%10–%14) | 8,06 (4,04–12,9) |
| 15 | 602 (457–835) | 774 (578–1018) | 635 (274–1040) | 1308 (865–1720) | %13 (%10–%15) | 9,68 (5,62–16,2) |
| 16 | 644 (471–837) | 860 (657–1116) | 718 (289–1504) | 1441 (818–2050) | %15 (%12–%17) | 11,8 (6,25–19,4) |
| 17 | 707 (550–852) | 943 (763–1179) | 952 (564–1401) | 1300 (903–2172) | %16 (%12–%18) | 14,1 (7,72–21,8) |
| 18 | 774 (571–999) | 1052 (840–1297) | 1150 (741–1594) | 1516 (895–2276) | %16 (%14–%19) | 16,4 (9,79–23,8) |
| 19 | 778 (678–987) | 1076 (930–1467) | 1108 (809–1859) | 1865 (1070–2386) | %18 (%15–%19) | 18,6 (12,5–24,2) |
| 20 | 834 (664–1210) | 1199 (1018–1449) | 1205 (818–1805) | 2290 (1193–2698) | %19 (%17–%20) | 20,2 (13,9–26,3) |
| 21 | 806 (611–1010) | 1234 (1078–1532) | 1245 (702–1715) | 2530 (1529–3028) | %19 (%17–%21) | 21,5 (15,4–26,7) |
| 22 | 914 (607–1070) | 1319 (1145–1545) | 1186 (692–1627) | 2593 (1441–3521) | %19 (%17–%22) | 21,7 (16,4–26,2) |
| 23 | 907 (714–1118) | 1314 (1234–1646) | 1384 (690–1751) | 2859 (1418–3798) | %20 (%17–%22) | 22,8 (16,6–26,8) |
| 24 | 953 (743–1180) | 1363 (1238–1823) | 1340 (905–2029) | 2729 (2095–3695) | %20 (%18–%23) | 22,8 (16,7–28) |
| 25 | 940 (760–1023) | 1508 (1253–1793) | 1511 (937–1961) | 2652 (1828–4116) | %20 (%17–%22) | 23,1 (17,1–28,9) |
| 26 | 993 (795–1432) | 1568 (1263–1810) | 1571 (978–1905) | 2662 (1660–4179) | %20 (%18–%22) | 23,8 (17,6–29,1) |
| 27 | 1012 (723–1233) | 1612 (1325–1899) | 1649 (969–2196) | 2903 (1824–3946) | %21 (%19–%23) | 24,2 (18,6–30,7) |
| 28 | 1146 (782–1445) | 1610 (1414–1964) | 1723 (994–2365) | 3110 (2013–4302) | %21 (%19–%23) | 25,5 (19,3–31) |
| 29 | 1088 (756–1445) | 1709 (1398–2056) | 1790 (1013–2661) | 2543 (1784–3793) | %21 (%19–%23) | 25,7 (20,3–31,5) |
| 30 | 1100 (802–1466) | 1742 (1409–2127) | 1877 (1049–2769) | 2356 (1640–3593) | %21 (%18–%24) | 25,6 (21,1–31,8) |
| 31 | 1172 (770–1371) | 1779 (1482–2166) | 1994 (1169–2525) | 2331 (1139–3365) | %21 (%18–%23) | 24,8 (21,2–32) |
| 32 | 1118 (860–1319) | 1827 (1508–2247) | 2095 (1388–2533) | 2153 (931–3390) | %21 (%19–%25) | 25,2 (21,3–32,2) |
| 33 | 1088 (921–1407) | 1924 (1480–2297) | 1973 (1365–2513) | 2002 (806–3466) | %21 (%18–%23) | 25,5 (21,2–30,6) |
| 34 | 1152 (901–1355) | 1970 (1518–2256) | 2059 (1302–2692) | 1877 (1011–3835) | %21 (%19–%24) | 25 (21,1–29,9) |
| 35 | 1080 (943–1262) | 1967 (1598–2316) | 2044 (1350–2686) | 2257 (1046–3764) | %21 (%19–%24) | 25,7 (20,9–29,9) |
| 36 | 1172 (939–1492) | 1955 (1656–2276) | 2092 (1571–2734) | 2014 (1620–3819) | %20 (%19–%23) | 25,8 (20,6–29,8) |
| 37 | 1216 (932–1393) | 2026 (1641–2270) | 2260 (1653–2795) | 2302 (1017–3684) | %21 (%19–%23) | 26,5 (20,8–31,2) |
| 38 | 1235 (1026–1538) | 2083 (1719–2307) | 2520 (1527–2869) | 1980 (1238–2880) | %21 (%18–%22) | 27,6 (21,3–31,8) |
| 39 | 1151 (941–1464) | 2093 (1644–2328) | 2468 (1518–3028) | 1908 (955–2799) | %21 (%18–%23) | 27 (20,7–32,4) |
| 40 | 1225 (940–1504) | 2106 (1620–2353) | 2414 (1728–3013) | 1612 (701–2986) | %21 (%17–%23) | 27,3 (21,3–32,7) |
| 41 | 1299 (970–1537) | 2116 (1564–2300) | 2527 (1841–3141) | 1592 (411–2905) | %19 (%17–%23) | 28,2 (22,2–32,5) |
| 42 | 1255 (1030–1417) | 2173 (1612–2358) | 2518 (1766–3089) | 1491 (499–2834) | %20 (%18–%23) | 27,1 (21,5–33,4) |
| 43 | 1177 (986–1432) | 2228 (1533–2372) | 2466 (1721–3271) | 1051 (619–2001) | %20 (%17–%23) | 27,3 (22,4–32,8) |
| 44 | 1309 (1035–1561) | 2209 (1549–2387) | 2693 (1709–3331) | 858 (318–1807) | %20 (%17–%24) | 27 (22,5–32,5) |
| 45 | 1240 (1010–1441) | 2137 (1554–2364) | 2717 (1796–3211) | 1074 (373–2327) | %21 (%17–%24) | 27,7 (21,5–32,2) |
| 46 | 1272 (1087–1556) | 2062 (1607–2355) | 2645 (1805–3405) | 738 (304–1842) | %19 (%18–%23) | 27,2 (20,1–32,4) |
| 47 | 1203 (1072–1599) | 2132 (1617–2434) | 2605 (1820–3317) | 687 (260–1947) | %19 (%17–%23) | 26,7 (19,6–32) |
| 48 | 1390 (1193–1633) | 2129 (1673–2400) | 2615 (1887–3286) | 875 (57,7–2341) | %20 (%16–%22) | 26,5 (19–31,7) |
| 49 | 1342 (1066–1548) | 2099 (1679–2461) | 2773 (1952–3420) | 863 (203–1609) | %20 (%17–%23) | 27 (18,7–31,9) |
| 50 | 1324 (1062–1601) | 2103 (1631–2517) | 2842 (2059–3448) | 640 (172–1840) | %20 (%17–%24) | 27,2 (19,3–31,7) |
| 51 | 1283 (1090–1508) | 2063 (1624–2407) | 2949 (1974–3600) | 712 (114–1634) | %20 (%17–%23) | 27,1 (19,8–32,1) |
| 52 | 1362 (1101–1687) | 2173 (1682–2493) | 2815 (2151–3563) | 721 (186–1878) | %19 (%16–%23) | 26,4 (18,9–31,8) |
| 53 | 1272 (1083–1537) | 2189 (1760–2507) | 2721 (2072–3597) | 871 (135–1761) | %19 (%17–%23) | 26,1 (18,3–31,5) |
| 54 | 1382 (1052–1652) | 2137 (1701–2477) | 2673 (2099–3627) | 663 (193–1558) | %19 (%17–%23) | 25,9 (18,2–32) |
| 55 | 1305 (1098–1487) | 2080 (1744–2550) | 2854 (2209–3774) | 607 (142–1698) | %20 (%17–%23) | 26,4 (18,5–32,4) |
| 56 | 1450 (1161–1582) | 2129 (1706–2568) | 2761 (2284–3746) | 833 (248–1974) | %20 (%18–%23) | 26,4 (18,8–32,6) |
| 57 | 1327 (1068–1465) | 2157 (1518–2558) | 2837 (2007–3662) | 975 (176–2019) | %20 (%18–%23) | 26,8 (19,1–33,2) |
| 58 | 1352 (1118–1590) | 2163 (1674–2615) | 3017 (2180–4009) | 821 (263–2168) | %19 (%18–%24) | 26,9 (19–33,9) |
| 59 | 1334 (1134–1747) | 2199 (1694–2609) | 3009 (2121–3968) | 628 (86,3–2288) | %19 (%17–%23) | 27,5 (19,2–35,5) |
| 60 | 1391 (1130–1795) | 2239 (1650–2693) | 3036 (2080–4050) | 781 (117–1892) | %19 (%18–%23) | 27,8 (19,9–35,2) |

### Altın ve ambar (2/3)

| Yıl | Canavar baskınında yitirilen altın | Ejderhaya giden altın (haraç + akın) | Hazinesi boş medeniyet payı | Kent tüketiminde yokluk payı (köy+; ekmek, bira ya da alet) | Ekmek ya da bira yokluğu payı (köy+) | Kıtlık (büyük olay) |
|---|---|---|---|---|---|---|
| 1 | 0 | 0 | %0 | – | – | 0 |
| 2 | 0 | 0 | %0 | %1,4 (%0–%3) | %1,4 (%0–%3) | 0 |
| 3 | 0 | 0 | %0 | %0,7 (%0,4–%1,6) | %0,7 (%0,4–%1,6) | 0 |
| 4 | 15,5 (2,5–59) | 0 | %0 | %0,3 (%0,1–%3,3) | %0,3 (%0,1–%3,3) | 0 |
| 5 | 16 (3–52,5) | 0 | %0 | %0,2 (%0–%3,8) | %0,2 (%0–%3,8) | 0 |
| 6 | 17,5 (0–36,5) | 0 | %0 | %0 (%0–%1,6) | %0 (%0–%1,6) | 0 |
| 7 | 9 (0–32,5) | 0 | %0 | %0 (%0–%2) | %0 (%0–%2) | 0 |
| 8 | 5 (0–34) | 0 | %0 | %0,1 (%0–%2) | %0 (%0–%2) | 0 |
| 9 | 11 (0–21) | 0 | %0 | %0,3 (%0–%1,5) | %0 (%0–%1) | 0 |
| 10 | 0 (0–20,5) | 0 | %0 | %0,4 (%0–%2,3) | %0 (%0–%1,5) | 0 |
| 11 | 0 (0–29,5) | 0 | %0 | %0,4 (%0–%4) | %0,2 (%0–%2,8) | 0 |
| 12 | 3,5 (0–29) | 0 | %0 | %0,8 (%0–%7,8) | %0,4 (%0–%7,7) | 0 |
| 13 | 0 (0–18,5) | 0 | %0 | %1,8 (%0–%4,6) | %1,1 (%0–%4,6) | 0 |
| 14 | 6,5 (0–20) | 0 | %0 | %1,1 (%0–%7,1) | %0,3 (%0–%3,9) | 0 |
| 15 | 1,5 (0–19,5) | 0 | %0 | %1,3 (%0–%3,2) | %0 (%0–%1,7) | 0 |
| 16 | 1 (0–69,5) | 0 | %0 | %0,4 (%0–%3,1) | %0 (%0–%1,8) | 0 |
| 17 | 0 (0–35,5) | 0 | %0 | %0,5 (%0–%5,5) | %0,2 (%0–%4) | 0 |
| 18 | 3,5 (0–38) | 0 (0–4,5) | %0 | %1,3 (%0–%8,9) | %0,3 (%0–%6,7) | 0 |
| 19 | 1 (0–23) | 0 (0–104) | %0 | %2,6 (%0–%8,5) | %0,6 (%0–%7,3) | 0 |
| 20 | 0 (0–31) | 0 (0–162) | %0 | %3,2 (%0–%11) | %0,6 (%0–%7,5) | 0 |
| 21 | 3,5 (0–12) | 188 (6,5–346) | %0 | %2 (%0–%9,1) | %0,5 (%0–%9,1) | 0 |
| 22 | 0 (0–12) | 180 (11,5–298) | %0 | %3,8 (%0–%9,6) | %1,5 (%0–%8,7) | 0 |
| 23 | 0 (0–29) | 160 (38–278) | %0 | %5 (%0,6–%12) | %1,3 (%0–%8,7) | 0 |
| 24 | 0 (0–6,5) | 198 (111–566) | %0 | %6,7 (%0–%18) | %2 (%0–%9,6) | 0 |
| 25 | 0,5 (0–43,5) | 282 (29,5–442) | %0 | %8,4 (%0,5–%27) | %1,5 (%0–%11) | 0 |
| 26 | 5 (0–39) | 273 (145–422) | %0 | %10 (%1–%28) | %1,9 (%0–%8,8) | 0 |
| 27 | 0 (0–32) | 252 (37–402) | %0 | %8,4 (%4,6–%37) | %2,3 (%0,4–%7,8) | 0 |
| 28 | 0 (0–17,5) | 330 (159–535) | %0 | %9,2 (%3,3–%45) | %4,1 (%1–%9,2) | 0 |
| 29 | 0 (0–24,5) | 373 (183–598) | %0 | %8,9 (%1,4–%41) | %4,4 (%0,4–%7,8) | 0 |
| 30 | 0 (0–26,5) | 430 (182–676) | %0 | %15 (%3,8–%35) | %4,6 (%0,4–%8,7) | 0 |
| 31 | 0 (0–65,5) | 429 (89,5–732) | %0 | %24 (%3,3–%39) | %4,4 (%0,7–%8) | 0 |
| 32 | 0 (0–21) | 404 (182–883) | %0 | %26 (%4,8–%46) | %3,3 (%1,2–%8,3) | 0 |
| 33 | 0 (0–18,5) | 544 (200–770) | %0 | %30 (%8,5–%50) | %4,7 (%0,2–%9,9) | 0 |
| 34 | 0 (0–22,5) | 464 (253–707) | %0 | %32 (%7,7–%50) | %4,1 (%0,3–%11) | 0 |
| 35 | 7 (0–55,5) | 359 (0,5–674) | %0 | %31 (%6,1–%49) | %3,5 (%0,3–%13) | 0 |
| 36 | 1 (0–46) | 394 (160–720) | %0 | %27 (%6,8–%45) | %3,2 (%0–%13) | 0 |
| 37 | 0 (0–87,5) | 294 (86–626) | %0 | %29 (%9,2–%43) | %2,3 (%0,4–%13) | 0 |
| 38 | 0 (0–11,5) | 354 (53–651) | %0 | %39 (%18–%49) | %5,7 (%0,5–%12) | 0 |
| 39 | 1,5 (0–45) | 514 (43–826) | %0 | %39 (%21–%52) | %4,7 (%0,7–%13) | 0 |
| 40 | 0 (0–31) | 417 (68,5–979) | %0 | %41 (%23–%54) | %6,1 (%1,2–%13) | 0 |
| 41 | 0 (0–25) | 493 (20–694) | %0 | %42 (%21–%56) | %7,1 (%1,7–%10) | 0 |
| 42 | 0 (0–6,5) | 396 (79,5–908) | %0 | %43 (%17–%56) | %7,5 (%2,3–%11) | 0 |
| 43 | 0 (0–17,5) | 460 (27,5–815) | %0 | %44 (%34–%58) | %8 (%1,8–%13) | 0 |
| 44 | 1 (0–28,5) | 504 (46,5–1044) | %0 | %44 (%31–%60) | %7,4 (%2,4–%14) | 0 |
| 45 | 4 (0–31) | 578 (34,5–998) | %0 | %42 (%29–%58) | %7,5 (%0,2–%13) | 0 |
| 46 | 0 (0–29,5) | 530 (0–810) | %0 | %44 (%32–%58) | %6,8 (%0,5–%16) | 0 |
| 47 | 1 (0–15) | 494 (45,5–863) | %0 | %45 (%32–%57) | %7,2 (%0,4–%15) | 0 |
| 48 | 0 (0–8,5) | 468 (0,5–1042) | %0 | %44 (%27–%53) | %6,2 (%1,5–%14) | 0 |
| 49 | 0 (0–2) | 400 (12–1180) | %0 | %46 (%30–%57) | %6 (%1,1–%16) | 0 |
| 50 | 0 (0–29) | 565 (62,5–792) | %0 | %43 (%30–%54) | %6,9 (%0,6–%18) | 0 |
| 51 | 0 (0–31) | 490 (72–1288) | %0 | %42 (%29–%55) | %6,6 (%1,8–%13) | 0 (0–0,5) |
| 52 | 0 (0–19) | 406 (60–1057) | %0 | %43 (%32–%53) | %6,9 (%2,2–%12) | 0 |
| 53 | 0 (0–12,5) | 526 (63–1119) | %0 | %44 (%31–%52) | %6,2 (%3,1–%13) | 0 |
| 54 | 0 (0–9,5) | 512 (34–1218) | %0 | %41 (%30–%52) | %6,5 (%3–%12) | 0 (0–0,5) |
| 55 | 0 (0–9) | 548 (176–1084) | %0 | %40 (%29–%50) | %4,6 (%1,9–%10) | 0 |
| 56 | 1,5 (0–32) | 622 (42–1021) | %0 | %41 (%28–%54) | %4,7 (%0,9–%10) | 0 |
| 57 | 1,5 (0–60) | 615 (186–1111) | %0 | %41 (%28–%56) | %4 (%1,1–%9,4) | 0 |
| 58 | 6,5 (0–38) | 426 (68,5–720) | %0 | %42 (%28–%56) | %4 (%0,3–%12) | 0 |
| 59 | 3,5 (0–27) | 452 (0–815) | %0 | %38 (%30–%56) | %3,4 (%1–%13) | 0 |
| 60 | 2,5 (0–28,5) | 565 (0–1008) | %0 | %40 (%31–%56) | %3,5 (%0,9–%12) | 0 |

### Altın ve ambar (3/3)

| Yıl | Açlıktan ölen | Kıtlık yardımı (sevkiyat) | Kıtlıkta yüz çeviren | Kıtlık akını | Ambarın yettiği gün (medeniyet medyanı) |
|---|---|---|---|---|---|
| 1 | 0 | 0 | 0 | 0 | 35,4 (24,4–51,8) |
| 2 | 0 | 0 | 0 | 0 | 61,1 (31–76,6) |
| 3 | 0 | 0 | 0 | 0 | 50,9 (38,9–66,3) |
| 4 | 0 | 0 | 0 | 0 | 49,3 (42,7–74,8) |
| 5 | 0 | 0 | 0 | 0 | 66,7 (44,4–94,4) |
| 6 | 0 | 0 | 0 | 0 | 84 (68,9–103) |
| 7 | 0 | 0 | 0 | 0 | 98 (76,9–129) |
| 8 | 0 | 0 | 0 | 0 | 108 (80,7–142) |
| 9 | 0 | 0 | 0 | 0 | 114 (81,4–148) |
| 10 | 0 | 0 | 0 | 0 | 143 (104–170) |
| 11 | 0 | 0 | 0 | 0 | 151 (95,6–211) |
| 12 | 0 | 0 | 0 | 0 | 173 (106–234) |
| 13 | 0 | 0 | 0 | 0 | 192 (140–257) |
| 14 | 0 | 0 | 0 | 0 | 219 (144–277) |
| 15 | 0 | 0 | 0 | 0 | 239 (145–279) |
| 16 | 0 | 0 | 0 | 0 | 244 (144–291) |
| 17 | 0 | 0 | 0 | 0 | 263 (153–280) |
| 18 | 0 | 0 | 0 | 0 | 262 (169–282) |
| 19 | 0 | 0 | 0 | 0 | 271 (183–296) |
| 20 | 0 | 0 | 0 | 0 | 272 (198–307) |
| 21 | 0 | 0 | 0 | 0 | 282 (203–308) |
| 22 | 0 | 0 | 0 | 0 | 285 (212–317) |
| 23 | 0 | 0 | 0 | 0 | 289 (220–322) |
| 24 | 0 | 0 | 0 | 0 | 283 (232–315) |
| 25 | 0 | 0 | 0 | 0 | 273 (227–315) |
| 26 | 0 | 0 | 0 | 0 | 260 (230–300) |
| 27 | 0 | 0 | 0 | 0 | 251 (187–297) |
| 28 | 0 | 0 | 0 | 0 | 255 (203–304) |
| 29 | 0 | 0 | 0 | 0 | 275 (208–292) |
| 30 | 0 | 0 | 0 | 0 | 252 (205–297) |
| 31 | 0 | 0 | 0 | 0 | 258 (200–318) |
| 32 | 0 | 0 | 0 | 0 | 252 (193–300) |
| 33 | 0 | 0 | 0 | 0 | 245 (195–312) |
| 34 | 0 | 0 | 0 | 0 | 222 (172–314) |
| 35 | 0 | 0 | 0 | 0 | 235 (164–304) |
| 36 | 0 | 0 | 0 | 0 | 235 (177–299) |
| 37 | 0 | 0 | 0 | 0 | 242 (206–286) |
| 38 | 0 | 0 | 0 | 0 | 232 (193–263) |
| 39 | 0 | 0 | 0 | 0 | 221 (180–251) |
| 40 | 0 | 0 | 0 | 0 | 199 (168–258) |
| 41 | 0 | 0 (0–1) | 0 | 0 | 189 (163–247) |
| 42 | 0 | 0 | 0 | 0 | 193 (160–251) |
| 43 | 0 | 0 | 0 | 0 | 186 (156–246) |
| 44 | 0 | 0 | 0 | 0 | 189 (144–249) |
| 45 | 0 | 0 | 0 | 0 | 186 (148–256) |
| 46 | 0 | 0 | 0 | 0 | 176 (131–248) |
| 47 | 0 | 0 | 0 | 0 | 172 (129–246) |
| 48 | 0 | 0 | 0 | 0 | 179 (133–240) |
| 49 | 0 | 0 | 0 | 0 | 181 (130–248) |
| 50 | 0 | 0 | 0 | 0 | 181 (135–237) |
| 51 | 0 | 0 (0–1,5) | 0 | 0 | 184 (134–241) |
| 52 | 0 | 0 | 0 | 0 | 174 (134–245) |
| 53 | 0 (0–1) | 0 | 0 | 0 | 175 (125–244) |
| 54 | 0 (0–1) | 0 (0–2) | 0 | 0 (0–0,5) | 165 (127–254) |
| 55 | 0 | 0 | 0 | 0 | 171 (133–253) |
| 56 | 0 | 0 | 0 | 0 | 176 (135–247) |
| 57 | 0 | 0 | 0 | 0 | 176 (132–244) |
| 58 | 0 | 0 | 0 | 0 | 169 (127–239) |
| 59 | 0 | 0 | 0 | 0 | 165 (123–234) |
| 60 | 0 | 0 | 0 | 0 | 168 (121–228) |

### Yerleşim kademesi

| Yıl | Ortalama yerleşim kademesi | Köy+ yerleşim | Kasaba+ yerleşim | Şehir | Ortalama başkent kademesi | Kademe değişimi (yerleşim, yıl içinde) |
|---|---|---|---|---|---|---|
| 1 | 0 | 0 | 0 | 0 | 0 | 0 |
| 2 | 0,25 (0,13–0,44) | 2 (1–4) | 0 | 0 | 0,25 (0,13–0,44) | 2 (1–4) |
| 3 | 0,65 (0,57–0,75) | 7 (5–8) | 0 | 0 | 0,86 (0,67–1) | 4,5 (3–6,5) |
| 4 | 0,53 (0,44–0,6) | 8 (6,5–9) | 0 | 0 | 1 (0,78–1) | 2 (0,5–4) |
| 5 | 0,5 (0,44–0,53) | 9 (7–11) | 0 | 0 | 0,94 (0,78–1) | 3 (1,5–5) |
| 6 | 0,56 (0,45–0,61) | 13 (9–15) | 0 (0–1) | 0 | 0,94 (0,75–1,11) | 4 (1,5–6) |
| 7 | 0,62 (0,56–0,72) | 17 (11,5–20) | 1 (0–2) | 0 | 1,11 (0,94–1,25) | 6 (3,5–8) |
| 8 | 0,73 (0,62–0,83) | 20 (14–23) | 2,5 (1–5) | 0 | 1,25 (0,93–1,44) | 6,5 (3,5–9) |
| 9 | 0,79 (0,69–0,83) | 21,5 (16–25) | 4 (1,5–6,5) | 0 | 1,47 (1,13–1,61) | 6 (3–8,5) |
| 10 | 0,86 (0,79–0,92) | 24,5 (19,5–29) | 7 (5–8) | 0 | 1,6 (1,4–1,78) | 6,5 (4–8,5) |
| 11 | 0,89 (0,84–0,96) | 27 (21–32,5) | 8 (5,5–11) | 0 | 1,63 (1,46–1,88) | 5,5 (2,5–10) |
| 12 | 0,93 (0,85–1) | 29 (22,5–34) | 10 (6–13,5) | 0 (0–0,5) | 1,81 (1,57–2) | 5 (2,5–7,5) |
| 13 | 0,96 (0,89–1,01) | 30,5 (23,5–35) | 12,5 (8–15) | 0 (0–0,5) | 1,86 (1,71–2) | 4,5 (2–7,5) |
| 14 | 0,97 (0,9–1,05) | 32 (24,5–37,5) | 14 (9,5–16) | 0 (0–1) | 1,94 (1,75–2,06) | 5 (2–9,5) |
| 15 | 1 (0,93–1,09) | 34 (27–39) | 15,5 (11–17) | 0 (0–1) | 2 (1,75–2,06) | 4 (3,5–7,5) |
| 16 | 1,03 (0,95–1,08) | 35,5 (28,5–39,5) | 16 (11,5–18,5) | 0,5 (0–1) | 2 (1,73–2,13) | 4,5 (2,5–7,5) |
| 17 | 1,04 (1,02–1,13) | 38 (31–42,5) | 17,5 (12–19,5) | 1 (0–2) | 2 (1,79–2,18) | 6 (3,5–11) |
| 18 | 1,09 (1,01–1,16) | 40 (32–43,5) | 18 (14,5–20) | 1 (0–2,5) | 2 (1,82–2,25) | 6 (3–7,5) |
| 19 | 1,12 (1,05–1,17) | 41,5 (32,5–45) | 18,5 (14,5–22) | 2 (1–3) | 2,24 (1,87–2,38) | 5 (2–8,5) |
| 20 | 1,13 (1,07–1,2) | 42 (34–46,5) | 19 (15,5–23) | 2 (1–4,5) | 2,21 (2,06–2,53) | 5 (3–9) |
| 21 | 1,13 (1,1–1,21) | 43 (35–48,5) | 21 (16,5–24) | 3 (1–4,5) | 2,38 (2,12–2,53) | 6 (1,5–10,5) |
| 22 | 1,16 (1,09–1,21) | 45 (35–50) | 20,5 (16,5–24,5) | 3 (1–5) | 2,38 (2,12–2,59) | 5,5 (2–11,5) |
| 23 | 1,17 (1,09–1,22) | 44,5 (36,5–51) | 22 (15,5–26) | 4 (2–6) | 2,44 (2,24–2,67) | 6 (3–11) |
| 24 | 1,18 (1,09–1,23) | 45,5 (37,5–51,5) | 22 (16–26) | 4 (2–6) | 2,43 (2,14–2,69) | 5,5 (2–9) |
| 25 | 1,16 (1,1–1,27) | 46,5 (38–52,5) | 22,5 (16,5–26,5) | 4 (2,5–6,5) | 2,53 (2,36–2,76) | 5,5 (3,5–11) |
| 26 | 1,16 (1,11–1,26) | 48 (37,5–53,5) | 22,5 (17–27,5) | 5 (3,5–7) | 2,57 (2,35–2,76) | 5 (3–7,5) |
| 27 | 1,18 (1,11–1,29) | 48 (38–53) | 23,5 (17,5–28) | 6 (4,5–7) | 2,63 (2,47–2,78) | 5 (3–6) |
| 28 | 1,2 (1,12–1,29) | 49 (38–55) | 24 (17,5–28,5) | 7 (5–7) | 2,71 (2,57–2,83) | 6,5 (3,5–9) |
| 29 | 1,21 (1,12–1,3) | 50 (39–54,5) | 24 (18–29,5) | 7 (5–8) | 2,73 (2,54–2,88) | 4,5 (1,5–10,5) |
| 30 | 1,19 (1,11–1,27) | 51 (40,5–56) | 24,5 (18,5–29) | 6 (6–7,5) | 2,67 (2,53–2,76) | 5 (3–9,5) |
| 31 | 1,21 (1,12–1,26) | 52 (40,5–57) | 25 (19–29,5) | 6,5 (5,5–8) | 2,69 (2,5–2,82) | 6 (3–11) |
| 32 | 1,22 (1,15–1,3) | 53,5 (40,5–60) | 25,5 (19,5–30) | 7 (5–8) | 2,71 (2,47–2,87) | 5 (2–8) |
| 33 | 1,23 (1,17–1,3) | 55 (40–61,5) | 26 (18,5–30) | 7 (4,5–9) | 2,69 (2,44–2,82) | 6 (1,5–8) |
| 34 | 1,25 (1,14–1,3) | 56 (41–63) | 26 (18,5–30,5) | 7,5 (4,5–9) | 2,73 (2,36–2,87) | 6 (2,5–9,5) |
| 35 | 1,25 (1,15–1,3) | 56,5 (42–64) | 27 (18,5–30) | 7 (5–9) | 2,71 (2,47–2,86) | 4,5 (1–8,5) |
| 36 | 1,24 (1,18–1,31) | 58,5 (41–64,5) | 27,5 (20,5–31) | 7 (5,5–8,5) | 2,69 (2,38–2,87) | 5 (1–9,5) |
| 37 | 1,24 (1,17–1,3) | 57,5 (41–64,5) | 27 (20,5–32) | 7 (5–8,5) | 2,67 (2,51–2,86) | 4,5 (1–10) |
| 38 | 1,22 (1,18–1,32) | 58,5 (40,5–64,5) | 28 (20,5–32,5) | 7 (5–9) | 2,69 (2,35–2,93) | 4 (1,5–6) |
| 39 | 1,24 (1,18–1,33) | 58 (41–65) | 28 (19,5–34,5) | 8 (6–9) | 2,69 (2,44–2,93) | 4,5 (2,5–8) |
| 40 | 1,26 (1,18–1,36) | 58,5 (42–66) | 29 (20–35,5) | 8 (6,5–9,5) | 2,63 (2,47–2,93) | 5 (1–12,5) |
| 41 | 1,24 (1,19–1,37) | 58,5 (42,5–66,5) | 29 (21–35,5) | 7,5 (6,5–9,5) | 2,67 (2,47–2,93) | 4,5 (1–10) |
| 42 | 1,27 (1,2–1,39) | 61 (43–67,5) | 28,5 (22–36) | 8 (7–10) | 2,69 (2,59–2,93) | 4,5 (1,5–10) |
| 43 | 1,27 (1,18–1,4) | 60 (42,5–68) | 29 (22,5–36,5) | 8 (6–10) | 2,71 (2,47–2,93) | 4 (2–8) |
| 44 | 1,28 (1,16–1,42) | 61,5 (43,5–70) | 29,5 (21,5–37) | 8 (6–10) | 2,67 (2,44–2,93) | 4 (1,5–8,5) |
| 45 | 1,27 (1,19–1,4) | 63 (43–71,5) | 29,5 (21–37) | 8 (6–10) | 2,71 (2,49–2,89) | 5,5 (3–9,5) |
| 46 | 1,29 (1,18–1,38) | 63,5 (44,5–71,5) | 30 (21,5–36) | 8 (6–10) | 2,69 (2,53–2,93) | 5 (1,5–10) |
| 47 | 1,27 (1,2–1,37) | 62,5 (43,5–71,5) | 30,5 (22–35,5) | 9 (6–10) | 2,71 (2,47–2,87) | 3 (1,5–8,5) |
| 48 | 1,28 (1,22–1,37) | 64 (43,5–73) | 30 (24–34,5) | 8,5 (6–10,5) | 2,73 (2,47–2,94) | 5 (1,5–10) |
| 49 | 1,29 (1,21–1,36) | 63,5 (44–73) | 30 (23,5–35) | 9 (6,5–11) | 2,76 (2,56–2,89) | 4 (0,5–7,5) |
| 50 | 1,27 (1,2–1,37) | 63,5 (43,5–72,5) | 30,5 (23–36,5) | 9 (6–10,5) | 2,76 (2,5–2,94) | 5 (2,5–8) |
| 51 | 1,28 (1,19–1,37) | 64 (44–73,5) | 30,5 (22,5–36) | 9 (7–10,5) | 2,75 (2,44–2,94) | 4 (1,5–7,5) |
| 52 | 1,28 (1,2–1,39) | 64 (45–75,5) | 30,5 (22–36,5) | 9 (6–10) | 2,71 (2,47–2,89) | 5 (2–9,5) |
| 53 | 1,28 (1,21–1,4) | 65,5 (46–77,5) | 30,5 (21,5–38) | 8,5 (6–10,5) | 2,69 (2,47–2,94) | 4,5 (1,5–11,5) |
| 54 | 1,26 (1,22–1,42) | 65 (47–79) | 29,5 (22–38,5) | 9 (6–11) | 2,75 (2,5–2,95) | 4,5 (0,5–10) |
| 55 | 1,27 (1,22–1,4) | 66,5 (47–79) | 30 (22,5–40) | 8,5 (7–11) | 2,68 (2,53–2,9) | 3,5 (0–9,5) |
| 56 | 1,29 (1,22–1,43) | 67,5 (48,5–80,5) | 31,5 (23–40,5) | 9 (7,5–11,5) | 2,75 (2,6–2,94) | 5,5 (1–8,5) |
| 57 | 1,28 (1,2–1,42) | 66,5 (48,5–81) | 30 (22,5–39,5) | 8,5 (6,5–12) | 2,76 (2,49–2,94) | 4 (2–9,5) |
| 58 | 1,3 (1,21–1,42) | 67,5 (49–81,5) | 30,5 (22,5–40) | 8,5 (6,5–12,5) | 2,8 (2,56–2,95) | 3,5 (2–11,5) |
| 59 | 1,29 (1,24–1,42) | 67,5 (48,5–80) | 30 (22,5–41) | 10 (7–13) | 2,83 (2,67–2,95) | 3,5 (1,5–8,5) |
| 60 | 1,3 (1,21–1,42) | 68 (48–80,5) | 30 (23–41) | 9 (7–12,5) | 2,75 (2,56–3) | 3,5 (1–5,5) |

### Deniz

| Yıl | Liman (tersane) | Gemi (koga/tekne) | Kadırga | Denizaşırı yerleşim | Deniz ticaret yolu (yıl sonu) | Deniz seferi (ticaret) |
|---|---|---|---|---|---|---|
| 1 | 0 | 0 | 0 | 0 | 0 | 0 |
| 2 | 0,5 (0–1,5) | 0,5 (0–1) | 0 | 0 | 0 | 0 |
| 3 | 1 (0–2) | 0,5 (0–2) | 0 | 0 | 0 | 0 |
| 4 | 1 (0,5–3) | 1 (0–3) | 0 | 0 | 0 | 0 |
| 5 | 1 (0,5–3) | 1 (0,5–3) | 0 | 0 | 0 | 0 |
| 6 | 1,5 (0,5–3,5) | 1,5 (0,5–3,5) | 0 | 0 | 0 | 0 |
| 7 | 2 (0,5–4) | 2 (0,5–5) | 0 | 0 | 0 | 0 |
| 8 | 2,5 (1–5) | 4 (1–7,5) | 0 | 0 | 0 | 0 |
| 9 | 4 (1–6) | 7 (1–13) | 0 | 0 (0–2) | 0 (0–2) | 0 |
| 10 | 4,5 (1,5–8) | 8 (2,5–17,5) | 0 | 1 (0–4) | 1 (0–4,5) | 0 |
| 11 | 5,5 (2–10,5) | 9 (3,5–22) | 0 | 2 (0–6) | 2 (0–7,5) | 0 (0–1,5) |
| 12 | 6,5 (2,5–12) | 13 (5–25) | 0 | 3 (0,5–7) | 3 (0,5–9) | 0 (0–4) |
| 13 | 7 (2,5–12,5) | 15,5 (5,5–27) | 0 | 4 (1–7) | 4 (1,5–9,5) | 0 (0–4) |
| 14 | 8 (4–12,5) | 16,5 (8–27) | 0 (0–1) | 4 (1,5–7,5) | 4 (1,5–10) | 0 (0–5) |
| 15 | 9 (4,5–12,5) | 18 (8,5–28,5) | 0 (0–2,5) | 4 (2–7,5) | 4,5 (2,5–10) | 0 (0–5) |
| 16 | 10 (5–13) | 18 (9,5–29) | 0 (0–2,5) | 4,5 (3–8) | 5 (3,5–11) | 1,5 (0–6) |
| 17 | 10 (5,5–14) | 19 (10–31,5) | 1 (0–3) | 5 (3–8) | 5 (3,5–11) | 1,5 (0–5,5) |
| 18 | 10 (5,5–15,5) | 21,5 (11–33) | 2,5 (0–4,5) | 5 (3–8) | 6 (3,5–11,5) | 2,5 (0–7,5) |
| 19 | 10 (6–16) | 22,5 (11–34) | 3 (0–8,5) | 5,5 (3–8,5) | 6 (3,5–12,5) | 3 (0–8) |
| 20 | 10 (6–16) | 23,5 (13–38,5) | 3 (0–9) | 5,5 (3–8,5) | 6 (3,5–12,5) | 3 (0–10) |
| 21 | 10,5 (6,5–16) | 25 (15–38,5) | 3,5 (1–10) | 6 (3–8,5) | 7 (4–12,5) | 3 (0–11) |
| 22 | 11 (8–16) | 26 (19,5–37) | 4,5 (2–10) | 6,5 (3,5–8,5) | 7,5 (5–12,5) | 5 (0–10) |
| 23 | 12 (8,5–16) | 28 (22–37,5) | 6,5 (3–13) | 7 (3,5–8,5) | 7,5 (5–12,5) | 6 (0–11) |
| 24 | 12 (9,5–16) | 29 (24–38,5) | 11 (4,5–14,5) | 7 (3,5–8,5) | 8,5 (5–12,5) | 6 (1,5–12,5) |
| 25 | 12,5 (10–16) | 30,5 (24–38,5) | 11 (6,5–16) | 7 (3,5–8,5) | 8,5 (5–12,5) | 7 (2,5–11) |
| 26 | 12,5 (10–15,5) | 31 (24,5–39) | 11,5 (7–16) | 7 (3,5–9,5) | 9,5 (5–14) | 7 (2,5–12,5) |
| 27 | 13 (10–16) | 33 (26–40,5) | 12,5 (7–16,5) | 7 (3,5–10) | 10 (5–15) | 6,5 (3–13,5) |
| 28 | 14,5 (10,5–18) | 36,5 (26–42,5) | 12,5 (8–17) | 7 (3,5–10,5) | 10 (5–15) | 7,5 (3–15) |
| 29 | 15 (10,5–18) | 37,5 (28–45,5) | 13,5 (8–17,5) | 7 (3,5–11) | 10 (5–16) | 8,5 (3–16) |
| 30 | 15 (10,5–18) | 39 (28,5–45,5) | 12 (8–17,5) | 7,5 (3,5–11) | 10,5 (5–17) | 8 (4–15,5) |
| 31 | 16 (10,5–18,5) | 38,5 (28,5–47) | 13 (8–17,5) | 7,5 (3,5–11) | 10,5 (5–17) | 9 (4–14) |
| 32 | 16 (10,5–19) | 38 (28,5–47,5) | 14 (8–18,5) | 7,5 (3,5–11,5) | 10,5 (5–17) | 9,5 (4–16) |
| 33 | 16,5 (10,5–20) | 39,5 (28,5–48,5) | 14 (8–20,5) | 7,5 (3,5–11,5) | 10,5 (5–17) | 9,5 (4–15,5) |
| 34 | 16,5 (10,5–20,5) | 41 (28,5–49) | 14,5 (8–20,5) | 7,5 (3,5–12) | 10,5 (5–17) | 9,5 (4–15,5) |
| 35 | 16,5 (10,5–21,5) | 41 (28,5–53) | 14,5 (9–20,5) | 7,5 (3,5–12,5) | 11 (5–17,5) | 8,5 (4–16) |
| 36 | 17 (10,5–21,5) | 41 (28,5–54,5) | 13,5 (9–22) | 7,5 (3,5–12,5) | 11 (5–18) | 11,5 (4–16,5) |
| 37 | 17 (10–21,5) | 41,5 (28,5–55,5) | 14 (10–23,5) | 7,5 (3,5–12,5) | 11 (5–18) | 11 (3–18) |
| 38 | 17 (10–22,5) | 42,5 (28,5–55) | 14 (10–23,5) | 7,5 (3,5–12,5) | 11 (5–18) | 12 (3,5–16) |
| 39 | 16,5 (10–22,5) | 44 (28,5–55) | 14,5 (10–24) | 7,5 (3,5–12,5) | 11 (5–18) | 13,5 (4,5–16) |
| 40 | 16,5 (10–22) | 44,5 (28,5–56) | 15 (10–24) | 7,5 (3,5–13) | 11 (5–18) | 11,5 (4,5–17) |
| 41 | 17,5 (10–22) | 46,5 (28,5–57) | 16 (10–24) | 7,5 (3,5–13) | 11,5 (5,5–18,5) | 13 (4,5–18,5) |
| 42 | 17,5 (9,5–22,5) | 47 (29,5–57) | 16 (10–24) | 7,5 (3,5–13) | 11,5 (5–18,5) | 13,5 (5–18) |
| 43 | 17,5 (10–23) | 47 (30–57) | 16 (10–24,5) | 7,5 (3,5–13) | 12 (5–20) | 13,5 (6–21) |
| 44 | 17,5 (10–22,5) | 50 (29,5–58) | 15 (10,5–24,5) | 7,5 (3,5–13,5) | 12 (5–20,5) | 14,5 (6–20,5) |
| 45 | 17,5 (10–23,5) | 50 (29–58) | 15,5 (10,5–25) | 7,5 (3,5–13,5) | 12 (5–21) | 14 (5–22) |
| 46 | 17,5 (10,5–24,5) | 50,5 (29–61,5) | 16 (10,5–25) | 7,5 (3,5–13,5) | 12 (5–22,5) | 12 (5–24) |
| 47 | 18 (11–24,5) | 50,5 (29–61,5) | 15,5 (10,5–24,5) | 7,5 (3,5–13,5) | 12 (5–22,5) | 14,5 (4,5–25,5) |
| 48 | 18 (11–25) | 50,5 (30–62) | 16,5 (10,5–24,5) | 7,5 (3,5–13,5) | 12 (5,5–22,5) | 15,5 (5,5–23) |
| 49 | 18,5 (11–25) | 50,5 (30–63,5) | 16,5 (10,5–24,5) | 7,5 (3,5–14) | 13 (5,5–22,5) | 13 (6–27,5) |
| 50 | 18 (11–25) | 52,5 (31–63,5) | 16,5 (10,5–24,5) | 7,5 (3,5–14) | 13 (5,5–22,5) | 16 (4,5–26) |
| 51 | 18 (11–25) | 52,5 (31–62,5) | 15,5 (10,5–24,5) | 7,5 (3,5–14) | 13 (5,5–22,5) | 16 (5–27,5) |
| 52 | 18,5 (11–25) | 52,5 (31–63,5) | 16 (10,5–24,5) | 7,5 (3,5–14) | 13 (5,5–22,5) | 14,5 (5,5–26) |
| 53 | 18,5 (11–25,5) | 53,5 (31–65) | 16 (10,5–23,5) | 7,5 (3,5–14) | 14 (5,5–22,5) | 15,5 (6–30,5) |
| 54 | 19,5 (11–25,5) | 54,5 (31–66,5) | 16 (10,5–23,5) | 7,5 (3,5–14) | 14 (5,5–24) | 20 (6,5–29,5) |
| 55 | 19,5 (11–25,5) | 57 (31–66) | 15,5 (10,5–23,5) | 7,5 (3,5–15) | 14 (5,5–24) | 19,5 (6,5–31) |
| 56 | 19,5 (11–25,5) | 58 (31–68) | 14,5 (10,5–23,5) | 7,5 (3,5–15,5) | 14 (5,5–25) | 16,5 (6,5–34) |
| 57 | 19,5 (11–26,5) | 60 (31–68) | 15,5 (11,5–23,5) | 7,5 (3,5–15,5) | 14 (5,5–25,5) | 19,5 (6–36) |
| 58 | 19,5 (11–27) | 60 (31–67,5) | 15,5 (11,5–23,5) | 7,5 (3,5–15,5) | 14 (5,5–25,5) | 18 (7–33,5) |
| 59 | 19,5 (11–27,5) | 60,5 (31–69) | 15,5 (11,5–23,5) | 7,5 (3,5–15,5) | 14 (5,5–26) | 17 (6,5–36,5) |
| 60 | 19,5 (11–27,5) | 59,5 (31–70,5) | 15,5 (11–21) | 7,5 (3,5–15,5) | 14,5 (5,5–26) | 19,5 (7–31,5) |

### v3: durum değişimi (1/2)

| Yıl | Yaşayan yerleşim (yıl ort.) | Büyük şehir (Şehir kademesi, yıl ort.) | El değiştiren yerleşim (fetih + bölünme) | El değiştiren büyük şehir | Büyük şehre hücum (kuşatma muharebesi) | Büyük şehir yağmalandı, tutulmadı |
|---|---|---|---|---|---|---|
| 1 | 8 (7–9) | 0 | 0 | 0 | 0 | 0 |
| 2 | 8 (7–9) | 0 | 0 | 0 | 0 | 0 |
| 3 | 8,61 (7,65–10) | 0 | 0 | 0 | 0 | 0 |
| 4 | 12,8 (9,72–14,6) | 0 | 0 | 0 | 0 | 0 |
| 5 | 17,5 (13,5–19) | 0 | 0 | 0 | 0 | 0 |
| 6 | 21,6 (16,8–23,9) | 0 | 0 | 0 | 0 | 0 |
| 7 | 26,5 (19,8–28,1) | 0 | 0 (0–1) | 0 | 0 | 0 |
| 8 | 29,6 (22,8–32,7) | 0 | 0 (0–1) | 0 | 0 | 0 |
| 9 | 32,9 (25–36,7) | 0 | 0 (0–1) | 0 | 0 | 0 |
| 10 | 35,6 (27,3–40,9) | 0 | 0 (0–1,5) | 0 | 0 | 0 |
| 11 | 37,6 (29,8–44,1) | 0 | 0 (0–1,5) | 0 | 0 | 0 |
| 12 | 41,1 (31,6–47,6) | 0 (0–0,17) | 0 (0–1) | 0 | 0 | 0 |
| 13 | 43,4 (33,7–50,6) | 0 (0–0,54) | 0 (0–1) | 0 | 0 | 0 |
| 14 | 45,9 (35,2–53,2) | 0 (0–0,96) | 0 (0–1) | 0 | 0 | 0 |
| 15 | 49,1 (36,4–55,5) | 0,04 (0–1) | 1 (0–2) | 0 | 0 | 0 |
| 16 | 50,5 (38–56,8) | 0,25 (0–1) | 0,5 (0–1) | 0 | 0 | 0 |
| 17 | 53,1 (39,9–58,2) | 0,63 (0–1,59) | 0 (0–3) | 0 | 0 | 0 |
| 18 | 54,8 (41,7–60,4) | 1 (0–2,13) | 0,5 (0–1,5) | 0 | 0 | 0 |
| 19 | 55,4 (43–62,1) | 1,66 (0,34–2,73) | 0 (0–1,5) | 0 | 0 | 0 |
| 20 | 56,8 (43,9–63,2) | 2,04 (1–3,26) | 1 (0–2) | 0 | 0 | 0 |
| 21 | 57,7 (44,7–65) | 2,75 (1–4,3) | 0,5 (0–2) | 0 | 0 | 0 |
| 22 | 58,7 (46,5–66,6) | 3,58 (1–4,75) | 1 (0–1,5) | 0 | 0 (0–1) | 0 (0–1) |
| 23 | 60,1 (48,2–68,1) | 3,79 (1,84–5,2) | 0 (0–2) | 0 | 0 | 0 |
| 24 | 61,5 (49–69,2) | 4,63 (2–6,09) | 0 (0–2,5) | 0 | 0 (0–0,5) | 0 |
| 25 | 62,3 (49,5–69,8) | 4,35 (2,75–6,46) | 0,5 (0–1,5) | 0 | 0 (0–1) | 0 (0–1) |
| 26 | 62,8 (51,2–70,8) | 4,7 (3,2–6,58) | 1 (0–3) | 0 | 0 | 0 |
| 27 | 64,1 (51,5–72) | 5,63 (4,22–6,95) | 0,5 (0–2,5) | 0 | 0 (0–0,5) | 0 |
| 28 | 65,3 (52,4–74,3) | 5,97 (4,63–7,06) | 1 (0–2,5) | 0 | 0 (0–0,5) | 0 |
| 29 | 66,4 (53–76,4) | 6,64 (5,05–7,72) | 0,5 (0–2) | 0 | 0 | 0 |
| 30 | 67,2 (53,7–78,2) | 6,19 (5,38–7,17) | 0,5 (0–1,5) | 0 | 0 (0–1) | 0 (0–0,5) |
| 31 | 69,5 (54,5–78,8) | 6,34 (5,97–7,67) | 1 (0–2,5) | 0 | 0 (0–1) | 0 |
| 32 | 70 (54,6–79,6) | 7 (4,88–8) | 0 (0–3) | 0 | 0 (0–1) | 0 (0–0,5) |
| 33 | 71 (55,1–79,4) | 7 (5–8,3) | 0,5 (0–1,5) | 0 | 0 | 0 |
| 34 | 71,6 (55,1–79,9) | 7,13 (4,13–8,65) | 1 (0–2) | 0 | 0 (0–0,5) | 0 (0–0,5) |
| 35 | 72,6 (55–80,5) | 7,37 (4,58–8,63) | 0 (0–1,5) | 0 | 0 (0–1) | 0 (0–0,5) |
| 36 | 73,1 (55–82,5) | 7 (5,5–8,98) | 1 (0–2) | 0 | 0 (0–1,5) | 0 (0–1) |
| 37 | 74 (55,4–84,1) | 6,95 (5,26–8,38) | 0,5 (0–1,5) | 0 | 0 (0–1) | 0 (0–1) |
| 38 | 74,7 (56–84,6) | 6,96 (5,18–8,71) | 1 (0–2) | 0 | 0 (0–0,5) | 0 |
| 39 | 75,7 (56,4–85,2) | 7,8 (5,38–9,04) | 0 (0–3) | 0 (0–0,5) | 0 (0–0,5) | 0 |
| 40 | 76 (57–85,9) | 7,6 (6,25–9,42) | 1 (0–2) | 0 | 0 (0–1) | 0 (0–1) |
| 41 | 77,6 (57–86,4) | 7,75 (6,1–9,68) | 1 (0–2) | 0 | 0 (0–0,5) | 0 (0–0,5) |
| 42 | 79,1 (56,9–86,5) | 7,8 (6,8–10,1) | 1 (0–2,5) | 0 | 0 | 0 |
| 43 | 80 (57,1–87) | 8 (6,24–10,5) | 0,5 (0–2) | 0 | 0 | 0 |
| 44 | 80 (57,4–87,7) | 8 (6,05–10,4) | 1 (0–2) | 0 | 0 (0–1) | 0 (0–0,5) |
| 45 | 79,9 (58,3–88,4) | 8,23 (6,03–9,77) | 1 (0–2) | 0 | 0 | 0 |
| 46 | 80,4 (59,2–89) | 8,13 (6,19–10) | 0,5 (0–2,5) | 0 | 0 | 0 |
| 47 | 80,5 (59,4–89,8) | 8,38 (6,31–10) | 1 (0–1,5) | 0 | 0 | 0 |
| 48 | 80,9 (60–90,3) | 8,78 (6–10,5) | 0,5 (0–2,5) | 0 | 0 (0–0,5) | 0 (0–0,5) |
| 49 | 81,1 (60,5–91,8) | 9,2 (6,11–10,6) | 1 (0–1,5) | 0 | 0 (0–1) | 0 (0–0,5) |
| 50 | 81,9 (60,9–92,7) | 9 (6,04–11,2) | 1 (0–2) | 0 | 0 (0–1) | 0 (0–0,5) |
| 51 | 82,2 (61,6–93) | 8,79 (6,61–10,2) | 0,5 (0–1,5) | 0 | 0 (0–1) | 0 (0–1) |
| 52 | 82,5 (61,7–93) | 9 (6,7–10,2) | 1 (0–2,5) | 0 | 0 | 0 |
| 53 | 83 (61,9–93,6) | 9,04 (6,14–10,3) | 1 (0–2) | 0 (0–0,5) | 0 (0–1) | 0 (0–1) |
| 54 | 83,7 (62–94,6) | 8,93 (5,96–10,8) | 1 (0–2) | 0 | 0 | 0 |
| 55 | 84,6 (62–94,7) | 8,54 (6,68–11) | 1 (0–2) | 0 | 0 (0–0,5) | 0 (0–0,5) |
| 56 | 85 (61,9–94,3) | 8,64 (7,1–11,3) | 1,5 (0–3) | 0 (0–0,5) | 0 (0–0,5) | 0 |
| 57 | 85,4 (62,5–94,9) | 8,54 (6,74–11,8) | 0 (0–1) | 0 | 0 (0–0,5) | 0 (0–0,5) |
| 58 | 85,1 (63,1–95,5) | 8,85 (6,8–11,8) | 1 (0–2,5) | 0 | 0 (0–1,5) | 0 (0–0,5) |
| 59 | 84,8 (63,6–95,5) | 9,71 (6,88–13) | 0,5 (0–1) | 0 | 0 (0–1) | 0 |
| 60 | 85,4 (64–96,2) | 9,75 (6,89–12,9) | 1 (0–2) | 0 | 0 (0–1) | 0 (0–1) |

### v3: durum değişimi (2/2)

| Yıl | Yerleşim durum değişimi (kuruluş hariç hepsi) | Orta halka kaydı (Köy/Kasaba: kademe değişimi ya da terk) | Açlık başlayan yerleşim | Salgın başlayan yerleşim | Yakılan/yanan yerleşim | Harabeye yeniden yerleşim |
|---|---|---|---|---|---|---|
| 1 | 0 | 0 | 0 | 0 | 0 | 0 |
| 2 | 2 (1–4) | 0 | 0 | 0 | 0 | 0 |
| 3 | 4,5 (3–6,5) | 0 | 0 | 0 | 0 | 0 |
| 4 | 3,5 (2–6,5) | 0 (0–2) | 0 | 0 | 1,5 (0,5–3) | 0 |
| 5 | 5 (2,5–8) | 1 (0–1) | 0 | 0 | 2 (1–3) | 0 |
| 6 | 6 (4–10) | 0 (0–2) | 0 | 0 | 1,5 (0,5–3) | 0 |
| 7 | 8 (6–11) | 1,5 (0,5–3) | 0 | 0 | 2 (0,5–3) | 0 |
| 8 | 10 (5–13,5) | 3 (1–4) | 0 | 0 | 2 (1–4,5) | 0 |
| 9 | 10 (5–14) | 3 (1–5) | 0 | 0 | 2 (1–4,5) | 0 |
| 10 | 9 (5,5–15,5) | 3 (1,5–4,5) | 0 | 0 | 1,5 (0,5–4) | 0 |
| 11 | 10 (4–16,5) | 2,5 (1–5,5) | 0 | 0 (0–1) | 2 (1–5) | 0 |
| 12 | 8,5 (4–13,5) | 3 (1–5) | 0 | 0 | 3 (0,5–4) | 0 |
| 13 | 7 (4–14) | 2 (1–4,5) | 0 | 0 (0–0,5) | 1 (0,5–5) | 0 |
| 14 | 7 (3–15) | 3 (0,5–5,5) | 0 | 0 | 1,5 (0,5–4,5) | 0 |
| 15 | 10,5 (4–16,5) | 2 (1–4,5) | 0 | 0 (0–1) | 3 (0–4) | 0 |
| 16 | 9 (3,5–16) | 2,5 (0,5–4,5) | 0 | 0,5 (0–2) | 2,5 (0,5–5,5) | 0 |
| 17 | 8,5 (5,5–24) | 3 (0,5–5,5) | 0 | 0 (0–1,5) | 2 (0,5–7) | 0 |
| 18 | 10 (5,5–14,5) | 2 (1–4,5) | 0 | 0 (0–0,5) | 3 (1,5–5) | 0 |
| 19 | 9 (4,5–14,5) | 3,5 (1–5) | 0 | 0 (0–0,5) | 2 (1–5) | 0 |
| 20 | 9 (5,5–17,5) | 3 (0–6) | 0 | 0 (0–1) | 3 (1–6,5) | 0 |
| 21 | 13 (4–19,5) | 4 (0,5–7) | 0 | 0 (0–1) | 4 (2–6,5) | 0 |
| 22 | 11 (5–19) | 2,5 (1,5–6) | 0 | 0 (0–1,5) | 3 (2–4,5) | 0 |
| 23 | 9 (5–22,5) | 3 (1,5–8) | 0 | 0 (0–1,5) | 3 (1–7) | 0 |
| 24 | 10,5 (5–20,5) | 3 (1–5) | 0 | 0 (0–1,5) | 4 (1,5–6,5) | 0 |
| 25 | 12 (6,5–23,5) | 3,5 (2–8) | 0 | 0 (0–1) | 3,5 (2–7,5) | 0 |
| 26 | 12 (6,5–23,5) | 3 (2–5,5) | 0 | 0 (0–1) | 4,5 (2,5–8) | 0 |
| 27 | 12 (7–17,5) | 3 (2–4,5) | 0 | 0 (0–1) | 4 (2,5–6,5) | 0 (0–0,5) |
| 28 | 12,5 (7–22,5) | 4,5 (1,5–5,5) | 0 | 0 (0–1) | 4 (1–8) | 0 |
| 29 | 10,5 (5–21,5) | 2 (1–6) | 0 | 0 (0–2,5) | 3 (2–7) | 0 |
| 30 | 12 (6,5–21) | 3,5 (1–6) | 0 | 0 (0–1) | 5 (3–7) | 0 |
| 31 | 12 (6–25,5) | 4 (2–7) | 0 | 0 (0–0,5) | 4,5 (2–8,5) | 0 |
| 32 | 9,5 (4,5–22) | 1,5 (0–4,5) | 0 | 0 (0–1) | 4 (1–8) | 0 |
| 33 | 13,5 (4,5–18) | 3 (0,5–5) | 0 | 0 (0–1,5) | 4 (2–6) | 0 (0–0,5) |
| 34 | 13,5 (7–20,5) | 3,5 (1–7) | 0 | 0 (0–1,5) | 5 (3–7) | 0 |
| 35 | 11 (3–20) | 3 (0,5–5,5) | 0 | 0 (0–1) | 3,5 (2–9) | 0 |
| 36 | 13 (3,5–21) | 2 (0,5–6) | 0 | 0 | 5 (1,5–8) | 0 |
| 37 | 13,5 (2,5–20) | 4 (1–6) | 0 | 0 (0–1) | 6 (2–7) | 0 |
| 38 | 11,5 (3–18,5) | 3 (1–4) | 0 | 0 (0–1) | 4,5 (1–7,5) | 0 (0–0,5) |
| 39 | 11,5 (4–22,5) | 3 (1–5) | 0 | 0,5 (0–1) | 5 (1–8) | 0 |
| 40 | 15 (3–25) | 3 (0,5–8,5) | 0 | 0 (0–1,5) | 3,5 (1–7) | 0 (0–0,5) |
| 41 | 15 (4–23) | 2,5 (1–6) | 0 (0–3) | 0 (0–1) | 4,5 (1–7) | 0 |
| 42 | 11 (4–23) | 2,5 (0,5–5,5) | 0 | 0 (0–1) | 4,5 (1–7) | 0 (0–0,5) |
| 43 | 11 (6–20,5) | 2,5 (1–5) | 0 | 0 (0–1) | 5 (2,5–8) | 0 (0–0,5) |
| 44 | 12 (5,5–25) | 2,5 (0–5,5) | 0 | 0 (0–2) | 4 (2–10) | 0 (0–0,5) |
| 45 | 13 (7–21) | 3,5 (1,5–6) | 0 | 0 (0–1) | 4,5 (2–8) | 0 |
| 46 | 14 (3,5–23) | 4 (1–6) | 0 | 0 (0–2,5) | 5 (2–8) | 0 |
| 47 | 11 (3,5–22) | 2 (1–6,5) | 0 | 0 (0–2,5) | 5 (1–7,5) | 0 (0–0,5) |
| 48 | 13 (4,5–20) | 3 (0,5–5,5) | 0 | 0,5 (0–1,5) | 4 (0–7) | 0 |
| 49 | 11 (4–16,5) | 2,5 (0,5–5,5) | 0 | 0 (0–1,5) | 4 (1–6) | 0 (0–0,5) |
| 50 | 10,5 (6–19,5) | 4 (1,5–5,5) | 0 | 0 (0–2) | 4,5 (2–7,5) | 0 |
| 51 | 10 (4,5–22,5) | 2,5 (0,5–4,5) | 0 (0–3) | 0 (0–2,5) | 4 (1–7,5) | 0 |
| 52 | 15 (5,5–22,5) | 2,5 (1–7,5) | 0 | 0 (0–1) | 5 (1,5–7,5) | 0 (0–0,5) |
| 53 | 13 (4,5–22,5) | 2,5 (0–6) | 0 (0–2) | 0 (0–1) | 3,5 (1–6,5) | 0 |
| 54 | 14,5 (3–23) | 3,5 (0–6,5) | 0 (0–7) | 0 (0–1) | 5 (2–6,5) | 0 |
| 55 | 8 (0,5–22) | 3 (0–5,5) | 0 | 0 (0–1,5) | 3,5 (0–7,5) | 0 |
| 56 | 16 (5–24,5) | 3,5 (0–5,5) | 0 | 0 (0–1) | 5,5 (3,5–8,5) | 0 (0–0,5) |
| 57 | 9 (5–23,5) | 2 (1–6) | 0 (0–1,5) | 0 (0–1,5) | 4 (1,5–7,5) | 0 |
| 58 | 14 (5–25) | 3 (0,5–5,5) | 0 | 1 (0–2) | 5,5 (1,5–8) | 0 (0–0,5) |
| 59 | 9,5 (4,5–17,5) | 2 (1–6,5) | 0 | 0 (0–1) | 4 (2–6) | 0 |
| 60 | 13,5 (4–20,5) | 2 (1–4,5) | 0 (0–3,5) | 0 (0–1) | 6 (1,5–8) | 0 |

