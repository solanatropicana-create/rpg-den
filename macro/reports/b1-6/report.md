# Ölçüm raporu: b1-6

16 dünya (seed 1-16) × 60 yıl (7200 gün) · 2026-10-01 08:24 · `FD.Macro.Run stats --seeds 1-16 --years 60 --jobs 2 --verify 1 --saveload 1`

Süre: 10 dk 37 sn duvar saati, 2 iş parçacığı; dünya başına 73,6 sn (en az 51,8, en çok 98,1; yıl sonu hash'leri dâhil).

## Bitiş ölçütleri

DESIGN-FAZ1.md, "Bitiş ölçütleri". ✓ geçti · ✗ kaldı · — ölçülemedi.

| # | Ölçüt | Koşul | Ölçülen | Sonuç |
|---|---|---|---|---|
| 1 | Donma yok | 41–60. yılların yıllık büyük olay medyanı ≥ 0,8 × 6–20. yılların medyanı | 51,5 / 56 = 0,92 kat | ✓ |
| 2 | Çöküş | dünyaların ≥ %75'inde 60 yılda ≥ 1 çöküş (yok olma ya da başkent kaybı) | %88 (14/16 dünya); toplam 119 çöküş: 13 yok olma, 106 başkent kaybı | ✓ |
| 3 | Kamplar | 41–60. yıllarda yaşayan kamp medyanı ≥ 6–20. yılların medyanı | 9,21 ≥ 8,64 (yıl sonu sayımıyla 9 / 9) | ✓ |
| 4a | Kahraman: doğuş seviyesi | her on yılda doğanların ortalama seviyesi ≤ 2 | 1,37 · 1,35 · 1,33 · 1,38 · 1,39 · 1,48 (on yıllar sırasıyla) | ✓ |
| 4b | Kahraman: Sv8+ | dünyaların ≥ yarısında en az bir kahraman Sv8 ve üstüne çıkar | %100 (16/16 dünya); dünyadaki en yüksek seviye: medyan Sv10, en çok Sv10 | ✓ |
| 4c | Kahraman: efsane | dünya başına efsane medyanı 1–6 | medyan 15,5 (p10–p90: 11–25,5; toplam 273) | ✗ |
| 4d | Kahraman: ölüm payı | doğan kahramanların %30–80'i ölür | %45 (1180/2608); dünya medyanı %43 (%37–%54) | ✓ |
| 5a | Determinizm | aynı seed → aynı tarih (toplayıcılı ve toplayıcısız koşu, her yıl sonu hash'i) | seed 1: 60/60 yıl sonu aynı | ✓ |
| 5b | Kayıt/yükleme | kaydet → yükle → devam = kesintisiz koşu (her yıl sonu hash'i) | seed 1, gün 3637: yüklenen dünya aynı, sonraki 30/30 yıl sonu aynı | ✓ |

Tanımlar:

- **1, 3:** "41–60. yılların medyanı": 41–60. yıllardaki bütün (dünya, yıl) değerlerinin medyanı (16 dünya × 20 yıl = 320 değer); 6–20. yıllar için de aynı. Büyük olay = `GameEvent.Major == true` olan olaylar (`Sim.OnEvent` ile sayılır, `World.Events` kırpılmasından etkilenmez). Kamp ölçütünde yıllık değer, o yıl her gün sayılan yaşayan kamp sayısının ortalamasıdır (korsan koyları dâhil; yıl sonu sayımıyla değer ayrıca verilmiştir).
- **2:** Çöküş: medeniyet yok olur (`Civ.Alive` false olur) ya da medeniyet yaşarken başkentini kaybeder: bir önceki gün sonunda başkenti olan yerleşim (`Sim.Capital`, en kalabalık yerleşim) artık yaşamıyor ya da başka medeniyetin; medeniyetin o gün hâlâ yerleşimi vardır. Son yerleşimin düşmesi yok olma olarak bir kez sayılır.
- **4a:** Her on yılda doğan bütün kahramanların (bütün dünyalar) doğuş seviyelerinin ortalaması; her on yıl ≤ 2 olmalı. Doğuş seviyesi kahraman `World.Heroes`'a girdiği anda okunur.
- **4b:** Dünyada koşu boyunca herhangi bir kahramanın ulaştığı en yüksek seviye ≥ 8 olan dünyaların payı.
- **4c:** Dünya başına koşu boyunca efsane olan (`Hero.Legend`) kahraman sayısının dünyalar arası medyanı.
- **4d:** Koşu boyunca doğan bütün kahramanlardan (bütün dünyalar) koşu sonunda ölü (`State == "dead"`) olanların payı; emekli olanlar ve diyarı terk edenler ölü sayılmaz.
- **5a:** İlk `--verify` seed'i aynı süreçte toplayıcı bağlanmadan yeniden koşulur; her yıl sonundaki kanonik dünya hash'i (golden testteki FNV-1a) toplayıcılı koşuyla karşılaştırılır. Bu, determinizmi ve toplayıcının simülasyonu değiştirmediğini birlikte denetler.
- **5b:** `Sim.Save(string)` ve `static Sim Sim.Load(string)` varsa ilk `--saveload` seed'i yıl ortasında (gün = yıl/2 × 120 + 37) kaydedilip yüklenir ve sonuna dek koşulur; yüklenen dünyanın hash'i ve sonraki yıl sonu hash'leri kesintisiz koşuyla karşılaştırılır.

## Eski analizdeki sorunlar

Eski analiz: TS v0.23, 12 seed × 30 yıl ve 3 seed × 60 yıl (Proje: `analiz-5-ajan-oneriler.md`). "Sürüyor mu" kaba bir eşiktir: ağaç bitişi medyanı ≤ 35. yıl; 30. yılda tam 5 kara yerleşimli medeniyet ≥ %50; kamp (30. yıl) < 0,75 × en yüksek yıl; altın (30. yıl) ≥ 10 × altın (1. yıl); boştaki iş gücü (30. yıl) ≥ %30; büyük olay (30. yıl) ≤ 0,6 × en yüksek yıl; 25. yıldan sonra doğanların ≥ %50'si Sv5+; hiç başkent kaybı yok.

| Bulgu | Eski analiz | Bu ölçüm | Sürüyor mu? |
|---|---|---|---|
| Araştırma ağacı erken bitiyor | ~19. yılda bitiyor; 30. yılda medeniyetlerin %98'i bitirmiş | başlangıç medeniyetlerinde ağaç bitişi (son çağ, araştıracak düğüm yok) medyanı 20. yıl (123/128 bitirdi); 30. yılda bitirmiş medeniyet payı %94, araştırması duran (her çağ) %96; ağacın biten payı 19. yılda %83, 30. yılda %96 | evet |
| Medeniyetler 5 yerleşimde takılıyor | 98 medeniyetin 74'ü (%76) tam 5 yerleşimde | tam 5 kara yerleşimli medeniyet payı 30. yılda %1,6, 60. yılda %3 (denizaşırı koloniler dâhil 30. yılda tam 5: %0,8, 5+: %98); medeniyet başına 8,8 yerleşim (30. yıl) | hayır |
| Kamp sayısı düşüyor | 6,8'den 3,5'e iniyor | 3 (1. yıl) → en yüksek 10 (7. yıl) → 8 (30. yıl) → 8,5 (60. yıl); yıl sonu, yıllık dünya medyanı | hayır |
| Altın birikiyor | altın medyanı 78'den 6.503'e çıkıyor | 8,42 (1. yıl) → 5866 (30. yıl) → 25734 (60. yıl) | evet |
| İş gücü boşta | iş gücünün %43'ü boşta | 30. yılda %51, 60. yılda %46 (işe yerleşemeyen `zanaatçı` / bütün iş gücü, askerler dâhil; yıl içi ortalama) | evet |
| Büyük olaylar seyreliyor | yıllık büyük olay 59'dan 28'e düşüyor | en yüksek 76,5 (14. yıl) → 42 (30. yıl) → 56,5 (60. yıl), yıllık dünya medyanı | evet |
| Doğuş seviyesi şişiyor | 24. yıldan sonra herkes Sv5 doğuyor; efsane mekaniği ölü | 25–60. yıllarda Sv5+ doğanların payı %0; on yıllık doğuş seviyesi ortalaması 1,37 · 1,35 · 1,33 · 1,38 · 1,39 · 1,48 | hayır |
| Başkent düşmüyor | başkent fethedilemiyor (agents.ts:673) | 16 dünyada 106 başkent kaybı, 13 yok olma; 920 yerleşim fethi | hayır |

## On yıllık özet

Hücre: dünyalar arası medyan (p10–p90). Her dünyada on yılın yıllık değerlerinin ortalaması alınır: akış ölçülerinde yıllık ortalama, stok ölçülerinde yıl sonu değerlerinin ortalaması. Yüzdeler 0–1 paylardır.

| Ölçü | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| **Medeniyet** | | | | | | |
| Yaşayan medeniyet | 8 (7–9) | 8 (7–9) | 8 (7–9) | 8 (7–9,1) | 8 (7–9,1) | 8,1 (6,75–9,9) |
| Yeni medeniyet (yeniden doğan) | 0 | 0 | 0 | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) |
| Yok olan medeniyet | 0 | 0 (0–0,05) | 0 (0–0,05) | 0 (0–0,1) | 0 (0–0,1) | 0 |
| Başkent kaybı (medeniyet yaşarken) | 0 (0–0,05) | 0 (0–0,25) | 0,1 (0–0,25) | 0,1 (0–0,35) | 0,2 (0–0,3) | 0,15 (0–0,3) |
| Çöküş (yok olma + başkent kaybı) | 0 (0–0,1) | 0 (0–0,3) | 0,1 (0–0,25) | 0,1 (0–0,4) | 0,2 (0,05–0,3) | 0,15 (0–0,3) |
| Yaşayan yerleşim | 14,7 (13–17,4) | 47,3 (40,2–53,7) | 65,7 (55–72) | 74,5 (60,3–80,8) | 79,4 (64,4–87) | 86,4 (68,3–95,9) |
| Medeniyet başına yerleşim | 1,9 (1,7–2,04) | 5,94 (5,63–6,24) | 8,25 (7,72–8,52) | 9,31 (8,52–9,78) | 9,58 (9,02–11,1) | 10,2 (9,23–11,7) |
| 5+ kara yerleşimli medeniyet payı | %4,4 (%2,7–%8,2) | %86 (%71–%90) | %99 (%90–%100) | %98 (%88–%100) | %95 (%86–%100) | %92 (%85–%100) |
| Kurulan yerleşim | 2,2 (1,85–2,65) | 2,8 (2,1–3,3) | 1,2 (0,8–1,65) | 0,75 (0,45–1) | 0,75 (0,1–1,05) | 0,85 (0,3–1,15) |
| Fethedilen yerleşim | 0 (0–0,15) | 0,55 (0,15–1,05) | 0,95 (0,5–1,95) | 1,15 (0,6–2,2) | 1,3 (0,75–2,05) | 1,25 (0,65–2,05) |
| Terk edilen yerleşim | 0 | 0 (0–0,1) | 0 (0–0,2) | 0 (0–0,3) | 0 (0–0,25) | 0,1 (0–0,3) |
| Toplam nüfus | 270 (221–314) | 1360 (1124–1532) | 2447 (2062–2673) | 3096 (2730–3483) | 3489 (3233–3984) | 3799 (3542–4430) |
| Ortalama çağ | 1,89 (1,84–1,94) | 3,41 (3,23–3,59) | 3,91 (3,81–4) | 4 | 4 | 4 |
| Araştırma ağacının biten payı (ort.) | %19 (%18–%21) | %71 (%66–%75) | %93 (%91–%96) | %97 (%96–%99) | %98 (%96–%99) | %99 (%96–%99) |
| Ağacı bitmiş medeniyet payı | %0 | %20 (%10–%28) | %82 (%66–%89) | %99 (%96–%100) | %100 (%97–%100) | %100 (%96–%100) |
| Araştırması duran medeniyet payı | %0 (%0–%3,1) | %42 (%37–%52) | %87 (%83–%93) | %99 (%96–%100) | %100 (%97–%100) | %100 (%96–%100) |
| Altın medyanı (medeniyetler) | 55,1 (42,1–76,9) | 831 (410–1269) | 3578 (2358–5773) | 9680 (5988–14745) | 10721 (6537–26206) | 20987 (6885–38390) |
| Boştaki iş gücü payı | %0,7 (%0,4–%0,9) | %18 (%14–%20) | %46 (%41–%48) | %50 (%48–%52) | %47 (%44–%52) | %46 (%42–%52) |
| Bölünme (ayrılıp kurulan medeniyet) | 0 | 0 | 0 | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) |
| En büyük medeniyetin yerleşimi | 2,55 (2,35–2,85) | 7,85 (7,15–8,35) | 11,3 (9,55–13,2) | 13,5 (10,4–16,4) | 15 (12,4–19,5) | 17,1 (13–24) |
| **Olaylar** | | | | | | |
| Olay | 80,8 (67,5–94,1) | 193 (152–212) | 173 (142–189) | 152 (122–185) | 159 (136–187) | 161 (121–207) |
| Büyük olay | 30 (26,3–35,5) | 65,4 (53,7–76,2) | 55,6 (44,1–60,7) | 47,7 (37,6–60,6) | 51,2 (42,5–62,3) | 54,6 (41,3–65,2) |
| **Savaş** | | | | | | |
| Muharebe | 4 (3,55–4,95) | 7,8 (6,15–8,9) | 8,7 (6,45–10,8) | 9,7 (7,3–12,3) | 9,8 (8,65–12,9) | 9,7 (8,05–14,2) |
| Başlayan savaş | 0,05 (0–0,15) | 0,8 (0,15–1,2) | 1,15 (0,6–2,6) | 1,5 (0,95–2,75) | 1,7 (1,1–3) | 1,7 (0,75–2,95) |
| Süren savaş (yıl sonu) | 0 (0–0,1) | 0,2 (0–0,5) | 0,45 (0–0,85) | 0,4 (0,1–0,85) | 0,5 (0,2–0,95) | 0,5 (0,15–0,95) |
| Yıl içinde süren savaş | 0,05 (0–0,2) | 1 (0,2–1,5) | 1,5 (0,65–3,3) | 2 (1,25–3,65) | 2,05 (1,55–3,95) | 2,3 (1,05–4) |
| Yağma akını (medeniyet) | 0 (0–0,3) | 0,85 (0–1,8) | 1,45 (0,25–2,95) | 1,9 (0,25–4,45) | 2,1 (0,3–4,65) | 2,2 (0,3–5,1) |
| Tarihî hak savaşı | 0 | 0,05 (0–0,2) | 0,15 (0–0,4) | 0,2 (0–0,5) | 0,1 (0–0,4) | 0,1 (0–0,3) |
| Pakt gereği savaş | 0 | 0,05 (0–0,2) | 0,1 (0–0,5) | 0,2 (0–0,6) | 0,2 (0–0,55) | 0,15 (0–0,5) |
| Kutsal Sefer çağrısı | 0 (0–0,05) | 0 (0–0,1) | 0 (0–0,15) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) |
| İhanet (pakt çiğnendi) | 0 | 0 | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,15) |
| Savunma paktı (yıl sonu) | 0 | 0,45 (0–0,9) | 1 (0,15–1,8) | 1 (0,05–2,05) | 1,2 (0,35–2,1) | 1 (0,55–2,5) |
| **Canavarlar** | | | | | | |
| Yaşayan kamp (yıl sonu) | 7,45 (7,1–8,35) | 7,4 (6,2–9,1) | 7,45 (6,85–8,65) | 8,25 (7,1–9,4) | 8,95 (7,6–10,6) | 9,1 (7,8–10,8) |
| Yaşayan kamp (yıl ort.) | 7,11 (6,75–7,94) | 7,63 (6,35–9,39) | 7,43 (6,89–8,7) | 8,35 (7,15–9,42) | 9,24 (7,59–10,5) | 8,97 (7,72–10,7) |
| Doğan kamp | 0,75 (0,7–0,85) | 1,65 (1,25–1,85) | 2 (1,5–2,2) | 2,3 (1,5–2,75) | 2,55 (2,05–3,3) | 2,35 (1,75–3,45) |
| Temizlenen kamp | 0 (0–0,1) | 2 (1,4–2,1) | 2 (1,4–2,45) | 2,2 (1,6–2,8) | 2,6 (1,7–3,05) | 2,45 (1,8–3,55) |
| Canavar baskını | 3,6 (3,1–4,35) | 3,8 (3,1–4,95) | 3,3 (2,85–3,85) | 3,2 (2,65–4,35) | 3,15 (2,35–4,15) | 3,45 (2,45–4,5) |
| **Kahramanlar** | | | | | | |
| Doğan kahraman | 1,4 (1–1,85) | 3 (2,45–3,75) | 3,25 (2,75–4,05) | 2,85 (1,8–3,95) | 2,85 (1,75–3,6) | 2,8 (1,85–3,8) |
| Ölen kahraman | 0,25 (0,05–0,45) | 0,8 (0,45–1,3) | 1,6 (0,5–2,25) | 1,45 (0,65–2,75) | 1,7 (0,95–2,2) | 1,3 (0,75–2,5) |
| Emekli olan kahraman | 0 | 0 | 0 (0–0,05) | 0,3 (0,1–0,5) | 0,65 (0,4–0,9) | 0,95 (0,7–1,4) |
| Diyarı terk eden kahraman | 0,1 (0–0,2) | 0 (0–0,1) | 0,1 (0–0,15) | 0,1 (0–0,45) | 0,3 (0,05–0,6) | 0,3 (0,1–0,7) |
| Ölümden dönen kahraman | 0 | 0 | 0 | 0 | 0 | 0 |
| Efsane olan kahraman | 0 | 0 (0–0,1) | 0,2 (0–0,55) | 0,3 (0,1–0,75) | 0,5 (0,25–0,95) | 0,4 (0,2–0,75) |
| Yaşayan kahraman (yıl sonu) | 3,7 (2,85–5,95) | 23,5 (18,5–29,7) | 43,6 (36,3–50,5) | 55,8 (45,7–59,5) | 57,8 (53,8–69,4) | 61,9 (51,1–72,5) |
| Doğuş seviyesi (ort.) | 1,38 (1,27–1,53) | 1,34 (1,21–1,48) | 1,32 (1,17–1,47) | 1,45 (1,22–1,56) | 1,38 (1,27–1,51) | 1,45 (1,32–1,66) |
| Ölüm seviyesi (ort.) | 1,33 (1–1,95) | 1,62 (1,05–1,98) | 2,11 (1,68–3,4) | 2,56 (1,92–3,21) | 2,82 (2,36–3,35) | 3,92 (3,24–5,43) |
| Yaşayan kahraman seviyesi (ort.) | 1,48 (1,14–1,73) | 2,05 (1,81–2,24) | 2,68 (2,44–2,86) | 3,32 (2,86–3,49) | 3,63 (3,29–4,07) | 3,92 (3,52–4,59) |
| En yüksek seviye (şimdiye dek) | 1,75 (1,15–2) | 3,85 (3,1–4,7) | 5,9 (4,8–6,9) | 7,5 (6,35–9,05) | 9,1 (7,45–10) | 10 (9,2–10) |
| **Han ve ticaret** | | | | | | |
| Ayakta han | 3,05 (2,8–3,75) | 3,75 (3,2–4,7) | 5 (4,65–5,6) | 5 (4,65–5,7) | 5,05 (4,95–6) | 5,65 (5–6,05) |
| Asılan ilan | 1,5 (1–1,8) | 2,6 (2,1–3,25) | 2,75 (2,25–4,05) | 2,45 (1,95–3) | 2,5 (1,75–3,4) | 2,45 (1,75–4,6) |
| Biten ilan | 0 | 0,7 (0,4–1) | 0,6 (0,15–1,45) | 1,2 (0,4–1,45) | 1 (0,7–1,65) | 1,35 (0,65–2,2) |
| Ticaret seferi (kervan) | 5,35 (3,4–11,3) | 42,2 (25,2–66,1) | 76,4 (35,3–95,9) | 82,6 (38,3–124) | 93,7 (38,3–135) | 100 (46,6–172) |
| İkmal seferi | 0 | 7,75 (3,25–10,1) | 20 (13,3–24,6) | 24,8 (14–34,3) | 29,4 (14,8–37,5) | 30,8 (16,6–45,3) |

## Kahraman seviyeleri

### Doğuş seviyesi (bütün dünyalar, on yıl içinde doğanlar)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 |
|---|---|---|---|---|---|
| 1–10 | 227 | 1,37 | %64 | %35 | %1,3 |
| 11–20 | 488 | 1,35 | %67 | %31 | %2,3 |
| 21–30 | 545 | 1,33 | %68 | %31 | %0,9 |
| 31–40 | 453 | 1,38 | %64 | %34 | %2,4 |
| 41–50 | 443 | 1,39 | %64 | %32 | %3,6 |
| 51–60 | 452 | 1,48 | %57 | %38 | %5,3 |

### Ölüm seviyesi (bütün dünyalar, on yıl içinde ölenler)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 | Sv4 | Sv5 | Sv6 | Sv7 | Sv8 | Sv9 | Sv10 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 1–10 | 38 | 1,39 | %61 | %39 | %0 | %0 | %0 | %0 | %0 | %0 | %0 | %0 |
| 11–20 | 135 | 1,57 | %55 | %36 | %7,4 | %2,2 | %0 | %0 | %0 | %0 | %0 | %0 |
| 21–30 | 251 | 2,49 | %27 | %31 | %21 | %14 | %4,8 | %1,2 | %1,2 | %0,4 | %0 | %0 |
| 31–40 | 265 | 2,6 | %22 | %31 | %25 | %14 | %5,7 | %0,4 | %0,8 | %0,4 | %0,8 | %0 |
| 41–50 | 255 | 2,96 | %15 | %28 | %29 | %15 | %6,7 | %3,1 | %2,4 | %1,2 | %0 | %0 |
| 51–60 | 236 | 3,72 | %9,7 | %24 | %26 | %12 | %11 | %4,7 | %5,1 | %5,1 | %1,3 | %2,1 |

### Yaşayan kahramanların seviyesi (bütün dünyalar, on yılın son yılının sonunda)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 | Sv4 | Sv5 | Sv6 | Sv7 | Sv8 | Sv9 | Sv10 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 10 | 176 | 1,48 | %57 | %38 | %5,1 | %0 | %0 | %0 | %0 | %0 | %0 | %0 |
| 20 | 526 | 2,38 | %24 | %34 | %25 | %13 | %3 | %0,4 | %0,2 | %0 | %0 | %0 |
| 30 | 806 | 2,91 | %18 | %30 | %22 | %12 | %10 | %4,7 | %2,1 | %0,2 | %0,2 | %0 |
| 40 | 920 | 3,48 | %12 | %25 | %22 | %14 | %12 | %7,7 | %3,6 | %1,8 | %1,1 | %0,9 |
| 50 | 968 | 3,83 | %11 | %22 | %21 | %12 | %12 | %8,8 | %6,3 | %3,3 | %1,5 | %1,9 |
| 60 | 1019 | 4 | %9,4 | %21 | %22 | %15 | %11 | %5,5 | %5,7 | %4,2 | %2,9 | %3,5 |

### Ölüm nedenleri (bütün dünyalar)

| Neden | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 | Toplam |
|---|---|---|---|---|---|---|---|
| Kuşatma | 0 | 22 | 59 | 92 | 110 | 72 | 355 (%30) |
| Kamp saldırısı | 14 | 75 | 17 | 46 | 83 | 53 | 288 (%24) |
| Ejderha | 0 | 17 | 125 | 59 | 0 | 1 | 202 (%17) |
| Suikast | 1 | 4 | 23 | 23 | 18 | 18 | 87 (%7,4) |
| Trol | 0 | 4 | 7 | 18 | 11 | 19 | 59 (%5) |
| Bilinmiyor | 0 | 0 | 0 | 1 | 9 | 46 | 56 (%4,7) |
| Yağma akını | 0 | 4 | 11 | 13 | 5 | 15 | 48 (%4,1) |
| Han baskını (canavar) | 19 | 1 | 5 | 2 | 4 | 1 | 32 (%2,7) |
| Kervan soygunu | 1 | 2 | 1 | 3 | 9 | 2 | 18 (%1,5) |
| Yol pususu | 0 | 1 | 3 | 5 | 1 | 7 | 17 (%1,4) |
| Han baskını (medeniyet) | 0 | 2 | 0 | 0 | 3 | 2 | 7 (%0,6) |
| Yerleşim baskını (canavar) | 3 | 3 | 0 | 0 | 0 | 0 | 6 (%0,5) |
| Düello | 0 | 0 | 0 | 3 | 2 | 0 | 5 (%0,4) |

Neden, ölümün kaydedildiği andaki son muharebenin türünden (başlık ve taraflar) ya da suikast olayından çıkarılır.

## Olay türleri

Dünya başına yıllık olay sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| İnşaat (`build`) | 25,4 (19,4–30,6) | 86 (67,7–97,3) | 54,5 (49,2–65,4) | 26,5 (19,6–41,2) | 19,6 (13,2–31,9) | 18 (9,05–29) |
| Kahraman (`hero`) | 3,35 (2,05–4,9) | 21 (14,2–27,9) | 32,6 (21,4–42) | 40,1 (32–54,6) | 54,9 (41–66,4) | 55,6 (40,8–75,9) |
| Göç (`migration`) | 1,45 (0,6–2,65) | 7,4 (5,4–13,6) | 15,7 (8,5–27,7) | 21,2 (10,7–32) | 21,3 (15–30,9) | 20,4 (11,8–30,9) |
| Sefer/ilan (`quest`) | 1,6 (1–2,15) | 7,65 (6,4–10,3) | 9,05 (4,6–14,7) | 12,1 (9,05–15,1) | 13,2 (10,3–18,8) | 15,5 (9,85–22,8) |
| Savaş (`war`) | 0,45 (0–1,1) | 5,15 (1,2–8,6) | 8,95 (3,7–18,2) | 12,6 (5,45–27,4) | 14,3 (6,95–27,3) | 13 (4,8–28,3) |
| Araştırma (`research`) | 16,2 (13,8–18,3) | 17,2 (13,6–18,1) | 3,3 (2,35–4,95) | 0,3 (0–1,1) | 0 (0–0,6) | 0 (0–0,8) |
| Kamp (`lair`) | 2,05 (1,9–2,25) | 4,4 (3,05–4,85) | 5,05 (3,6–6,3) | 6 (4,3–7,1) | 7,15 (5,4–8,8) | 7 (5,35–8,9) |
| Sınıf (`class`) | 4,65 (3,5–5,95) | 6,4 (5,25–7,75) | 5,35 (4,5–6,55) | 4,25 (3,85–5,15) | 4,2 (3,3–5,05) | 4 (3,15–5,3) |
| Deniz (`sea`) | 0,35 (0–1,2) | 5,7 (3,85–8,25) | 5,3 (2–9,55) | 2,9 (0,7–5,75) | 3,35 (0,75–7,45) | 4,45 (0,3–8,1) |
| Baskın (`raid`) | 3,7 (3,15–4,35) | 3,8 (2,95–4,85) | 2,55 (2,1–3,35) | 3,25 (2,55–4,1) | 3,3 (2,35–4,35) | 3,5 (2,55–4,55) |
| Han (`inn`) | 2,55 (2,2–3,3) | 3,2 (2,1–4) | 3,1 (1,85–3,95) | 2,25 (1,1–3,65) | 2,4 (1,45–3,25) | 2,85 (2–3,9) |
| Yerleşim (`settle`) | 4,6 (3,9–5,55) | 4,85 (3,75–6,25) | 2,35 (1,6–3,15) | 1,5 (0,85–1,75) | 1,35 (0,2–2) | 1,5 (0,6–2,2) |
| Ekonomi (`economy`) | 0,7 (0,6–0,95) | 1,9 (0,9–2,9) | 3,6 (2,1–4,55) | 2,85 (1,65–4,7) | 2,75 (2,1–3,6) | 2,6 (1,45–4,1) |
| Keşif (`discover`) | 6,3 (5–8,15) | 2,7 (1,8–3,2) | 1,75 (0,9–3,15) | 1,2 (0,45–2,05) | 1,35 (0,45–1,65) | 0,8 (0,3–1,9) |
| Ticaret (`trade`) | 1,25 (0,65–2,25) | 2,35 (0,85–3,3) | 1 (0,15–2,85) | 1,9 (0,15–2,55) | 0,65 (0,15–2,9) | 1,1 (0,2–2,55) |
| epitaph | 0,25 (0,05–0,45) | 0,8 (0,45–1,3) | 1,6 (0,5–2,25) | 1,45 (0,65–2,75) | 1,7 (0,95–2,2) | 1,3 (0,75–2,5) |
| Ölüm/terk (`death`) | 0,25 (0,1–0,4) | 0,8 (0,5–1,4) | 1,4 (0,6–2,25) | 1,5 (0,7–2,35) | 1,75 (1–2,25) | 1,4 (0,95–2,2) |
| Dünya (`world`) | 0,05 (0–0,25) | 0,45 (0,3–0,85) | 0,65 (0,35–1,05) | 1,25 (0,6–2) | 1,4 (0,75–1,75) | 1,3 (0,7–2,1) |
| Büyüme (`growth`) | 1,7 (1,35–1,9) | 1,3 (1,15–1,55) | 0,2 (0,1–0,35) | 0 (0–0,05) | 0 (0–0,3) | 0 (0–0,2) |
| Diplomasi (`diplomacy`) | 0,6 (0,25–0,95) | 0,5 (0,05–0,9) | 0,4 (0,1–1,15) | 0,25 (0,1–0,85) | 0,45 (0,05–1) | 0,4 (0,05–1,25) |
| Gerginlik (`tension`) | 0,55 (0,3–1,25) | 0,6 (0,1–0,8) | 0,3 (0–1) | 0,35 (0–0,95) | 0,3 (0,05–1,15) | 0,5 (0,1–1,45) |
| Çağ (`era`) | 1,5 (1,25–1,6) | 0,7 (0,55–0,9) | 0,2 (0,05–0,35) | 0 (0–0,05) | 0 | 0 |
| Ejderha (`dragon`) | 0 | 0,15 (0–0,7) | 1,8 (1,1–2,45) | 0,05 (0–1,25) | 0 (0–0,4) | 0 |
| Temas (`contact`) | 0,9 (0,55–1,5) | 0,65 (0,4–1) | 0,2 (0–0,45) | 0,1 (0–0,45) | 0,1 (0–0,2) | 0 (0–0,15) |
| Kriz (anlatıcı) (`crisis`) | 0 (0–0,05) | 0,3 (0,15–0,4) | 0,4 (0,15–0,55) | 0,35 (0–0,6) | 0,35 (0,15–0,6) | 0,4 (0,15–0,6) |
| Harika (`wonder`) | 0 | 0,15 (0,05–0,45) | 0,3 (0,15–0,45) | 0,1 (0–0,25) | 0,05 (0–0,2) | 0 (0–0,1) |
| Rahatlama (anlatıcı) (`relief`) | 0,2 (0,1–0,3) | 0 (0–0,15) | 0,05 (0–0,1) | 0,05 (0–0,1) | 0 | 0 |

## Büyük olay türleri

Dünya başına yıllık büyük olay sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Sefer/ilan (`quest`) | 1,6 (1–2,05) | 5,95 (4,85–7,2) | 7,2 (3,8–10) | 8,55 (6,3–10,1) | 9,25 (6,9–12,6) | 9,9 (6,45–16) |
| Savaş (`war`) | 0,3 (0–0,9) | 4,2 (0,9–6,55) | 6,45 (2,85–13,1) | 8,25 (3,8–17,5) | 9,55 (4,9–17,4) | 9,5 (3,05–18,6) |
| Kahraman (`hero`) | 0,6 (0,3–0,95) | 3,65 (2,85–4,4) | 5,55 (4,2–7,15) | 7,85 (6,2–9,4) | 10 (7,9–11) | 9,7 (8,35–12,3) |
| Kamp (`lair`) | 2,05 (1,9–2,25) | 4 (3,05–4,5) | 4,2 (2,95–5,15) | 4,8 (3,3–5,65) | 5,75 (4,15–7,2) | 5,3 (4,1–7,15) |
| Araştırma (`research`) | 6,1 (5,15–7,45) | 14,8 (12–16,6) | 3,15 (2,25–4,9) | 0,3 (0–1) | 0 (0–0,55) | 0 (0–0,7) |
| İnşaat (`build`) | 2,2 (1,55–2,5) | 9,95 (8,1–15) | 6,05 (2,8–8,65) | 2,15 (0,7–2,65) | 1,05 (0,55–1,7) | 0,95 (0,4–1,5) |
| Baskın (`raid`) | 3,3 (2,5–4,15) | 3,25 (2,05–3,7) | 1,8 (1,25–2,5) | 2,1 (1,55–2,5) | 2,25 (1,85–3,05) | 2,5 (1,8–3,8) |
| Deniz (`sea`) | 0,3 (0–0,65) | 2,55 (1,65–3,9) | 2,45 (0,75–4,4) | 1,7 (0,3–3,5) | 2,05 (0,25–4,7) | 2,9 (0,3–5,2) |
| Han (`inn`) | 1,4 (1,1–1,7) | 2 (1,35–2,75) | 1,95 (1,05–2,7) | 1,45 (0,6–2,7) | 1,65 (1,05–2,25) | 2,2 (1,5–2,7) |
| Sınıf (`class`) | 1,85 (1,3–2,85) | 2,4 (1,4–3,55) | 2,35 (1,3–3,25) | 1,45 (0,8–2,25) | 1,2 (0,6–2,2) | 1,1 (0,45–2,05) |
| Yerleşim (`settle`) | 2,2 (1,85–2,65) | 2,8 (2,1–3,3) | 1,2 (0,8–1,65) | 0,75 (0,45–1) | 0,75 (0,1–1,05) | 0,85 (0,3–1,15) |
| epitaph | 0,25 (0,05–0,45) | 0,8 (0,45–1,3) | 1,6 (0,5–2,25) | 1,45 (0,65–2,75) | 1,7 (0,95–2,2) | 1,3 (0,75–2,5) |
| Ölüm/terk (`death`) | 0,25 (0,1–0,4) | 0,8 (0,5–1,4) | 1,4 (0,6–2,25) | 1,5 (0,7–2,35) | 1,75 (1–2,25) | 1,4 (0,95–2,2) |
| Keşif (`discover`) | 2,25 (1,75–3,1) | 1,3 (0,7–1,8) | 1 (0,4–1,45) | 0,6 (0,2–1,3) | 0,65 (0,2–1) | 0,2 (0–0,8) |
| Ekonomi (`economy`) | 0 | 1,05 (0,55–1,6) | 1,8 (1,4–2,4) | 0,95 (0,65–1,45) | 0,55 (0,2–0,85) | 0,2 (0–0,5) |
| Ticaret (`trade`) | 0,65 (0,35–1,3) | 1 (0,4–1,45) | 0,45 (0,05–1,1) | 0,95 (0,1–1,35) | 0,3 (0,05–1,25) | 0,55 (0,1–1,25) |
| Dünya (`world`) | 0,05 (0–0,2) | 0,4 (0,2–0,55) | 0,4 (0,25–0,8) | 0,75 (0,4–1,2) | 0,85 (0,5–1,2) | 0,8 (0,5–1,3) |
| Gerginlik (`tension`) | 0,55 (0,3–1,25) | 0,6 (0,1–0,8) | 0,3 (0–1) | 0,35 (0–0,95) | 0,3 (0,05–1,15) | 0,5 (0,1–1,45) |
| Çağ (`era`) | 1,5 (1,25–1,6) | 0,7 (0,55–0,9) | 0,2 (0,05–0,35) | 0 (0–0,05) | 0 | 0 |
| Büyüme (`growth`) | 0,9 (0,7–1,05) | 1,25 (1,1–1,5) | 0,2 (0,1–0,35) | 0 (0–0,05) | 0 (0–0,2) | 0 (0–0,2) |
| Ejderha (`dragon`) | 0 | 0,15 (0–0,7) | 1,8 (1,1–2,45) | 0,05 (0–1,25) | 0 (0–0,4) | 0 |
| Temas (`contact`) | 0,9 (0,55–1,5) | 0,65 (0,4–1) | 0,2 (0–0,45) | 0,1 (0–0,45) | 0,1 (0–0,2) | 0 (0–0,15) |
| Kriz (anlatıcı) (`crisis`) | 0 (0–0,05) | 0,3 (0,15–0,4) | 0,4 (0,15–0,55) | 0,35 (0–0,6) | 0,35 (0,15–0,6) | 0,4 (0,15–0,6) |
| Diplomasi (`diplomacy`) | 0,15 (0–0,45) | 0,35 (0,05–0,6) | 0,3 (0,1–0,65) | 0,15 (0–0,65) | 0,25 (0–0,65) | 0,2 (0–1,1) |
| Göç (`migration`) | 0,2 (0,05–0,4) | 0,2 (0,05–0,4) | 0,2 (0,05–0,35) | 0,2 (0–0,55) | 0,2 (0,1–0,35) | 0,2 (0,1–0,35) |
| Harika (`wonder`) | 0 | 0,15 (0,05–0,45) | 0,3 (0,15–0,45) | 0,1 (0–0,25) | 0,05 (0–0,2) | 0 (0–0,1) |
| Rahatlama (anlatıcı) (`relief`) | 0,2 (0,1–0,3) | 0 (0–0,15) | 0,05 (0–0,1) | 0,05 (0–0,1) | 0 | 0 |

## Muharebe türleri

Dünya başına yıllık muharebe sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Kamp saldırısı (`camp`) | 0,1 (0–0,2) | 2,05 (1,6–2,35) | 1,7 (1,1–2,25) | 1,9 (1,4–2,85) | 2,4 (1,55–3,2) | 2 (1,9–2,85) |
| Yapı baskını (canavar) (`extRaid`) | 2,05 (1,35–2,8) | 2,15 (1,35–3,05) | 1,3 (0,95–2,1) | 1,5 (0,8–2,35) | 1,5 (0,9–2) | 1,4 (0,95–2,1) |
| Yağma akını (`plunder`) | 0 (0–0,3) | 0,85 (0–1,8) | 1,45 (0,25–2,95) | 1,9 (0,25–4,45) | 2,1 (0,3–4,65) | 2,2 (0,3–5,1) |
| Yerleşim baskını (canavar) (`raid`) | 1,65 (1,15–2,2) | 1,5 (1,05–1,85) | 0,75 (0,55–1,05) | 0,95 (0,75–1,4) | 0,8 (0,5–1,35) | 0,95 (0,5–1,3) |
| Kuşatma (`siege`) | 0,05 (0–0,2) | 0,55 (0,15–1,15) | 1 (0,5–2,2) | 1,25 (0,7–2,35) | 1,4 (0,75–2,2) | 1,25 (0,65–2,05) |
| Trol (`troll`) | 0 | 0 (0–0,35) | 0,65 (0,1–1,1) | 0,85 (0,3–1,9) | 1,2 (0,85–1,6) | 1,5 (1,1–2,25) |
| Ejderha (`dragon`) | 0 | 0,05 (0–0,3) | 0,75 (0,4–1,3) | 0,05 (0–0,6) | 0 (0–0,15) | 0 |
| Han baskını (canavar) (`innMonster`) | 0,2 (0,05–0,35) | 0,1 (0–0,2) | 0 (0–0,1) | 0 (0–0,25) | 0 (0–0,2) | 0 (0–0,1) |
| Korsan savaşı (`pirate`) | 0 | 0 (0–0,2) | 0,15 (0–0,25) | 0 (0–0,2) | 0 (0–0,4) | 0,1 (0–0,2) |
| Deniz savaşı (`naval`) | 0 | 0 | 0 (0–0,25) | 0,05 (0–0,35) | 0 (0–0,2) | 0,1 (0–0,3) |
| Kervan soygunu (`robbery`) | 0 | 0 (0–0,05) | 0 (0–0,05) | 0 (0–0,15) | 0,1 (0–0,2) | 0 (0–0,15) |
| Yol pususu (`ambush`) | 0 (0–0,1) | 0 (0–0,15) | 0 (0–0,1) | 0 (0–0,05) | 0 (0–0,05) | 0 (0–0,1) |
| Düello (`duel`) | 0 | 0 | 0 | 0 (0–0,05) | 0 (0–0,05) | 0 |
| Han baskını (medeniyet) (`innCiv`) | 0 | 0 (0–0,1) | 0 (0–0,1) | 0 | 0 (0–0,1) | 0 (0–0,1) |

## Kamp türleri

Dünya başına yaşayan kamp (yıl sonu değerlerinin on yıllık ortalaması): dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Hobgoblin (`hobgoblin`) | 1,45 (1,1–1,8) | 2,35 (1,95–2,8) | 2,75 (1,85–3,4) | 3,45 (2,25–5,05) | 3,4 (2,65–5,3) | 4,2 (3,2–5,35) |
| Goblin (`goblin`) | 5,7 (5,2–6,35) | 3,35 (2,3–5,35) | 1,9 (0,75–3,2) | 2,05 (0,6–2,85) | 2 (1,05–2,8) | 1,35 (0,55–2,35) |
| Trol (`troll`) | 0 | 0 (0–0,45) | 0,65 (0,1–1) | 1,25 (0,25–1,95) | 1,55 (1,05–2,3) | 2 (1,6–2,85) |
| Bugbear (`bugbear`) | 0,5 (0,15–0,7) | 0,8 (0,4–1) | 1 (0,45–1,35) | 1,05 (0,3–1,95) | 0,95 (0,5–1,85) | 1,1 (0,4–1,55) |
| Korsan (`pirate`) | 0 | 0,45 (0,25–0,75) | 0,55 (0,25–1,05) | 0,25 (0–1,4) | 0,5 (0,1–1,3) | 0,3 (0,05–1,05) |
| Ejderha (`dragon`) | 0 | 0,05 (0–0,3) | 0,9 (0,4–1) | 0 (0–0,8) | 0 (0–0,15) | 0 |

## Çağ dağılımı

Yaşayan medeniyetlerin çağlara dağılımı, bütün dünyalar (yıl sonu).

| Yıl | I Kamp | II Köy | III Kasaba | IV Krallık |
|---|---|---|---|---|
| 10 | %0,8 | %29 | %60 | %10 |
| 20 | %0 | %0 | %26 | %74 |
| 30 | %0 | %0 | %1,6 | %98 |
| 40 | %0 | %0 | %0 | %100 |
| 50 | %0 | %0 | %0 | %100 |
| 60 | %0 | %0 | %0 | %100 |

## Dünyalar

| Seed | Medeniyet | Yerleşim | Nüfus | Çöküş | Efsane | En yüksek Sv | Doğan / ölü kahraman | Ağaç bitişi (yıl, medyan) | Süre (sn) | Son hash |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | 7 | 81 | 3954 | 13 | 5 | 9 | 200 / 103 | 21,5 | 98,1 | `9c8891783767afaf` |
| 2 | 7 | 63 | 3864 | 0 | 34 | 10 | 130 / 48 | 21 | 83 | `351dc89d3924552c` |
| 3 | 10 | 93 | 4689 | 7 | 18 | 10 | 177 / 66 | 18 | 84,1 | `5271488755cb20d2` |
| 4 | 10 | 93 | 4801 | 7 | 11 | 10 | 158 / 72 | 20 | 81,4 | `acf7aebd3dadba74` |
| 5 | 9 | 101 | 4506 | 14 | 16 | 10 | 198 / 87 | 17 | 83,5 | `0472a5146a0a9966` |
| 6 | 7 | 71 | 3370 | 0 | 13 | 10 | 125 / 51 | 22 | 58,3 | `2946d2433ec0c9e8` |
| 7 | 10 | 103 | 4891 | 3 | 18 | 10 | 203 / 104 | 19 | 90,9 | `aa1d31fe7b8c15f8` |
| 8 | 9 | 97 | 4187 | 4 | 29 | 10 | 133 / 42 | 17,5 | 74,3 | `db24b8a39aeafcd3` |
| 9 | 6 | 82 | 3672 | 18 | 21 | 10 | 172 / 98 | 19 | 70 | `641cc27be08202d5` |
| 10 | 10 | 99 | 4488 | 4 | 11 | 10 | 168 / 88 | 24 | 82,2 | `408dde28806efd5a` |
| 11 | 9 | 103 | 4196 | 16 | 14 | 10 | 186 / 104 | 20 | 72,9 | `59a06248f4a470b3` |
| 12 | 8 | 81 | 3715 | 6 | 15 | 10 | 131 / 49 | 18 | 63,3 | `4a8bffd7137ca24e` |
| 13 | 7 | 85 | 3772 | 7 | 15 | 9 | 157 / 62 | 18,5 | 72 | `f23e64874a7eb108` |
| 14 | 6 | 67 | 3615 | 5 | 22 | 10 | 131 / 52 | 23 | 65,8 | `20b9a36a0d33197f` |
| 15 | 9 | 93 | 3897 | 7 | 16 | 10 | 190 / 91 | 23,5 | 57,4 | `17cce7730f4741cb` |
| 16 | 8 | 86 | 3736 | 8 | 15 | 10 | 149 / 63 | 22 | 51,8 | `233fa3fd2658d6ad` |

Çöküşler:

- seed 1, 15. yıl (gün 1785): Tatlıçayır Loncası başkenti kaybetti: Kavşakpazar (Örsyürek Tapınak Klanı aldı)
- seed 1, 18. yıl (gün 2103): Tatlıçayır Loncası başkenti kaybetti: Gölgeçarşı (Örsyürek Tapınak Klanı aldı)
- seed 1, 22. yıl (gün 2631): Rüzgâr Manastırı başkenti kaybetti: Sessiztepe (Kanlıdiş Kabileleri aldı)
- seed 1, 24. yıl (gün 2836): Tatlıçayır Loncası başkenti kaybetti: Balköprü (Örsyürek Tapınak Klanı aldı)
- seed 1, 25. yıl (gün 2887): Rüzgâr Manastırı başkenti kaybetti: Sisliyamaç (Kanlıdiş Kabileleri aldı)
- seed 1, 27. yıl (gün 3234): Tatlıçayır Loncası başkenti kaybetti: Kartalkaya (Örsyürek Tapınak Klanı aldı)
- seed 1, 32. yıl (gün 3837): Tatlıçayır Loncası başkenti kaybetti: Meşekent (Örsyürek Tapınak Klanı aldı)
- seed 1, 37. yıl (gün 4344): Tatlıçayır Loncası başkenti kaybetti: Demirbük (Örsyürek Tapınak Klanı aldı)
- seed 1, 38. yıl (gün 4560): Fıçıköy Serbest Şehri yok oldu
- seed 1, 39. yıl (gün 4636): Tatlıçayır Loncası yok oldu
- seed 1, 45. yıl (gün 5390): Rüzgâr Manastırı başkenti kaybetti: Dinginpınar (Kanlıdiş Kabileleri aldı)
- seed 1, 47. yıl (gün 5566): Rüzgâr Manastırı başkenti kaybetti: Karageçit (Kızılboynuz Soyu aldı)
- seed 1, 56. yıl (gün 6720): Rüzgâr Manastırı başkenti kaybetti: Pınardere (Kanlıdiş Kabileleri aldı)
- seed 3, 23. yıl (gün 2676): Karaörs Derinlikleri başkenti kaybetti: Karagöl (Örsyürek Tapınak Klanı aldı)
- seed 3, 25. yıl (gün 2990): Karaörs Derinlikleri başkenti kaybetti: Gölgeörs (Örsyürek Tapınak Klanı aldı)
- seed 3, 32. yıl (gün 3730): Tatlıçayır Loncası başkenti kaybetti: Fıçıköy (Kanlıdiş Kabileleri aldı)
- seed 3, 35. yıl (gün 4155): Tatlıçayır Loncası başkenti kaybetti: Balköprü (Kanlıdiş Kabileleri aldı)
- seed 3, 37. yıl (gün 4418): Tatlıçayır Loncası başkenti kaybetti: Kavşakpazar (Kanlıdiş Kabileleri aldı)
- seed 3, 44. yıl (gün 5261): Tatlıçayır Loncası başkenti kaybetti: Söğütburç (Kanlıdiş Kabileleri aldı)
- seed 3, 54. yıl (gün 6468): Kanlıdiş Kabileleri başkenti kaybetti: Balgeçit (Rüzgâr Manastırı aldı)
- seed 4, 40. yıl (gün 4715): Kanlıdiş Kabileleri başkenti kaybetti: Savaşçukur (Güneştacı Krallığı aldı)
- seed 4, 43. yıl (gün 5066): Güneştacı Krallığı başkenti kaybetti: Savaşçukur (Kanlıdiş Kabileleri aldı)
- seed 4, 43. yıl (gün 5106): Kanlıdiş Kabileleri başkenti kaybetti: Savaşçukur (Pulzırh Lejyonu aldı)
- seed 4, 43. yıl (gün 5122): Kanlıdiş Kabileleri başkenti kaybetti: Dumanköprü (Sınır Bekçileri aldı)
- seed 4, 51. yıl (gün 6015): Kanlıdiş Kabileleri başkenti kaybetti: Dinginpınar (Güneştacı Krallığı aldı)
- seed 4, 52. yıl (gün 6160): Kanlıdiş Kabileleri başkenti kaybetti: Tuzyayla (Pulzırh Lejyonu aldı)
- seed 4, 53. yıl (gün 6312): Kanlıdiş Kabileleri başkenti kaybetti: Çamova (Pulzırh Lejyonu aldı)
- seed 5, 10. yıl (gün 1163): Tatlıçayır Loncası başkenti kaybetti: Kavşakpazar (Karaörs Derinlikleri aldı)
- seed 5, 12. yıl (gün 1342): Tatlıçayır Loncası başkenti kaybetti: Gölgeçarşı (Kanlıdiş Kabileleri aldı)
- seed 5, 14. yıl (gün 1591): Tatlıçayır Loncası yok oldu
- seed 5, 20. yıl (gün 2308): Karaörs Derinlikleri başkenti kaybetti: Kara Mihrap (Pulzırh Lejyonu aldı)
- seed 5, 25. yıl (gün 2888): Karaörs Derinlikleri başkenti kaybetti: Külçukur (Rüzgâr Manastırı aldı)
- seed 5, 25. yıl (gün 2962): Karaörs Derinlikleri yok oldu
- seed 5, 32. yıl (gün 3831): Rüzgâr Manastırı başkenti kaybetti: Sessiztepe (Pulzırh Lejyonu aldı)
- seed 5, 35. yıl (gün 4137): Rüzgâr Manastırı başkenti kaybetti: Külçukur (Kanlıdiş Kabileleri aldı)
- seed 5, 39. yıl (gün 4634): Kanlıdiş Kabileleri başkenti kaybetti: Ejderkale (Pulzırh Lejyonu aldı)
- seed 5, 42. yıl (gün 5001): Bulutkapı Tarikatı başkenti kaybetti: Yelköy (Kanlıdiş Kabileleri aldı)
- seed 5, 42. yıl (gün 5016): Kanlıdiş Kabileleri başkenti kaybetti: Karlıçayır (Pulzırh Lejyonu aldı)
- seed 5, 49. yıl (gün 5852): Kanlıdiş Kabileleri başkenti kaybetti: Karagöl (Pulzırh Lejyonu aldı)
- seed 5, 51. yıl (gün 6006): Kanlıdiş Kabileleri başkenti kaybetti: Sessizocak (Güneştacı Krallığı aldı)
- seed 5, 52. yıl (gün 6230): Gölköprü Boyu başkenti kaybetti: Günkaya (Güneştacı Krallığı aldı)
- seed 7, 48. yıl (gün 5680): Kızılboynuz Soyu başkenti kaybetti: Kızılkül (Kanlıdiş Kabileleri aldı)
- seed 7, 54. yıl (gün 6447): Yeşilyazı Kara Beyliği başkenti kaybetti: Yeşilyazı (Kızılboynuz Soyu aldı)
- seed 7, 60. yıl (gün 7197): Yeşilyazı Kara Beyliği başkenti kaybetti: Gölköprü (Kızılboynuz Soyu aldı)
- seed 8, 27. yıl (gün 3123): Kızılboynuz Soyu başkenti kaybetti: Közsaray (Pulzırh Lejyonu aldı)
- seed 8, 31. yıl (gün 3639): Kızılboynuz Soyu başkenti kaybetti: Boynuztepe (Pulzırh Lejyonu aldı)
- seed 8, 45. yıl (gün 5284): Kızılboynuz Soyu başkenti kaybetti: Alazvadi (Pulzırh Lejyonu aldı)
- seed 8, 56. yıl (gün 6604): Kızılboynuz Soyu başkenti kaybetti: Gökyurt (Pulzırh Lejyonu aldı)
- seed 9, 10. yıl (gün 1195): Sınır Bekçileri başkenti kaybetti: İzsürer (Karaörs Derinlikleri aldı)
- seed 9, 14. yıl (gün 1579): Pulzırh Lejyonu başkenti kaybetti: Ejderkale (Kanlıdiş Kabileleri aldı)
- seed 9, 14. yıl (gün 1630): Sınır Bekçileri başkenti kaybetti: Gözcüağaç (Karaörs Derinlikleri aldı)
- seed 9, 17. yıl (gün 1924): Sınır Bekçileri yok oldu
- seed 9, 18. yıl (gün 2120): Pulzırh Lejyonu başkenti kaybetti: Pulkalkan (Kanlıdiş Kabileleri aldı)
- seed 9, 20. yıl (gün 2398): Pulzırh Lejyonu başkenti kaybetti: Kılıçyurt (Kanlıdiş Kabileleri aldı)
- seed 9, 23. yıl (gün 2671): Pulzırh Lejyonu yok oldu
- seed 9, 36. yıl (gün 4206): Karaörs Derinlikleri başkenti kaybetti: Kara Mihrap (Güneştacı Krallığı aldı)
- seed 9, 36. yıl (gün 4254): Kanlıdiş Kabileleri başkenti kaybetti: Sisliyamaç (Rüzgâr Manastırı aldı)
- seed 9, 38. yıl (gün 4520): Boynuztepe Boyu yok oldu
- seed 9, 39. yıl (gün 4626): Karaörs Derinlikleri başkenti kaybetti: Közsaray (Kızılboynuz Soyu aldı)
- seed 9, 41. yıl (gün 4870): Karaörs Derinlikleri başkenti kaybetti: Yıldızçayır (Kızılboynuz Soyu aldı)
- seed 9, 42. yıl (gün 4930): Karaörs Derinlikleri başkenti kaybetti: Zincirkaya (Örsyürek Tapınak Klanı aldı)
- seed 9, 44. yıl (gün 5165): Karaörs Derinlikleri başkenti kaybetti: Okyayı (Güneştacı Krallığı aldı)
- seed 9, 46. yıl (gün 5452): Kızılboynuz Soyu başkenti kaybetti: Kızılkül (Kanlıdiş Kabileleri aldı)
- seed 9, 47. yıl (gün 5619): Karaörs Derinlikleri yok oldu
- seed 9, 57. yıl (gün 6835): Kızılboynuz Soyu başkenti kaybetti: Yıldızçayır (Kanlıdiş Kabileleri aldı)
- seed 9, 60. yıl (gün 7091): Kanlıdiş Kabileleri başkenti kaybetti: Yıldızçayır (Güneştacı Krallığı aldı)
- seed 10, 38. yıl (gün 4480): Karaörs Derinlikleri başkenti kaybetti: Kurtkale (Sınır Bekçileri aldı)
- seed 10, 41. yıl (gün 4854): Taşbasamak Boyu yok oldu
- seed 10, 42. yıl (gün 4941): Karaörs Derinlikleri başkenti kaybetti: Taşbasamak (Kanlıdiş Kabileleri aldı)
- seed 10, 47. yıl (gün 5583): Karaörs Derinlikleri başkenti kaybetti: Dişlivadi (Sınır Bekçileri aldı)
- seed 11, 15. yıl (gün 1744): Çarkyıldız Akademisi başkenti kaybetti: Pusulakule (Kızılboynuz Soyu aldı)
- seed 11, 17. yıl (gün 1961): Örsyürek Tapınak Klanı başkenti kaybetti: Demirçan (Kanlıdiş Kabileleri aldı)
- seed 11, 18. yıl (gün 2047): Çarkyıldız Akademisi başkenti kaybetti: Kristalköy (Kızılboynuz Soyu aldı)
- seed 11, 22. yıl (gün 2589): Kızılboynuz Soyu başkenti kaybetti: Közsaray (Kanlıdiş Kabileleri aldı)
- seed 11, 24. yıl (gün 2783): Örsyürek Tapınak Klanı başkenti kaybetti: Közkapı (Kanlıdiş Kabileleri aldı)
- seed 11, 29. yıl (gün 3465): Çarkyıldız Akademisi başkenti kaybetti: Mürekkeptepe (Kızılboynuz Soyu aldı)
- seed 11, 31. yıl (gün 3678): Rüzgâr Manastırı başkenti kaybetti: Çankule (Kızılboynuz Soyu aldı)
- seed 11, 33. yıl (gün 3873): Çarkyıldız Akademisi başkenti kaybetti: Yıldıztepe (Kızılboynuz Soyu aldı)
- seed 11, 39. yıl (gün 4657): Rüzgâr Manastırı başkenti kaybetti: Sessiztepe (Kanlıdiş Kabileleri aldı)
- seed 11, 40. yıl (gün 4765): Çarkyıldız Akademisi başkenti kaybetti: Kızılçayır (Kızılboynuz Soyu aldı)
- seed 11, 45. yıl (gün 5373): Rüzgâr Manastırı başkenti kaybetti: Yıldızçayır (Kızılboynuz Soyu aldı)
- seed 11, 50. yıl (gün 5999): Çarkyıldız Akademisi başkenti kaybetti: Kuzeyoba (Kızılboynuz Soyu aldı)
- seed 11, 52. yıl (gün 6216): Rüzgâr Manastırı başkenti kaybetti: Ayburç (Kızılboynuz Soyu aldı)
- seed 11, 53. yıl (gün 6257): Rüzgâr Manastırı başkenti kaybetti: Karageçit (Kanlıdiş Kabileleri aldı)
- seed 11, 54. yıl (gün 6414): Çarkyıldız Akademisi başkenti kaybetti: Günkaya (Kızılboynuz Soyu aldı)
- seed 11, 58. yıl (gün 6936): Rüzgâr Manastırı başkenti kaybetti: Aksırt (Kanlıdiş Kabileleri aldı)
- seed 12, 28. yıl (gün 3254): Sınır Bekçileri başkenti kaybetti: Gözcüağaç (Kanlıdiş Kabileleri aldı)
- seed 12, 47. yıl (gün 5521): Kanlıdiş Kabileleri başkenti kaybetti: Gölgeçarşı (Sınır Bekçileri aldı)
- seed 12, 47. yıl (gün 5525): Tatlıçayır Loncası başkenti kaybetti: Fıçıköy (Kanlıdiş Kabileleri aldı)
- seed 12, 49. yıl (gün 5775): Sınır Bekçileri başkenti kaybetti: Gölgeçarşı (Kızılboynuz Soyu aldı)
- seed 12, 54. yıl (gün 6385): Tatlıçayır Loncası başkenti kaybetti: Kartalkaya (Kanlıdiş Kabileleri aldı)
- seed 12, 56. yıl (gün 6601): Sınır Bekçileri başkenti kaybetti: Sarıhisar (Kızılboynuz Soyu aldı)
- seed 13, 14. yıl (gün 1646): Kanlıdiş Kabileleri başkenti kaybetti: Kurtgeçit (Sınır Bekçileri aldı)
- seed 13, 23. yıl (gün 2675): Sınır Bekçileri başkenti kaybetti: Gözcüağaç (Kanlıdiş Kabileleri aldı)
- seed 13, 31. yıl (gün 3630): Sınır Bekçileri başkenti kaybetti: Yabanyurt (Kızılboynuz Soyu aldı)
- seed 13, 37. yıl (gün 4329): Sınır Bekçileri başkenti kaybetti: Siskoru (Kızılboynuz Soyu aldı)
- seed 13, 37. yıl (gün 4435): Sınır Bekçileri başkenti kaybetti: Yeşilyazı (Kanlıdiş Kabileleri aldı)
- seed 13, 39. yıl (gün 4634): Kızılboynuz Soyu başkenti kaybetti: Siskoru (Kanlıdiş Kabileleri aldı)
- seed 13, 41. yıl (gün 4849): Sınır Bekçileri yok oldu
- seed 14, 48. yıl (gün 5706): Karaörs Derinlikleri başkenti kaybetti: Külçukur (Güneştacı Krallığı aldı)
- seed 14, 50. yıl (gün 5964): Karaörs Derinlikleri başkenti kaybetti: Kara Mihrap (Güneştacı Krallığı aldı)
- seed 14, 52. yıl (gün 6224): Karaörs Derinlikleri başkenti kaybetti: Zincirkaya (Güneştacı Krallığı aldı)
- seed 14, 55. yıl (gün 6484): Karaörs Derinlikleri başkenti kaybetti: Sessizocak (Güneştacı Krallığı aldı)
- seed 14, 56. yıl (gün 6649): Karaörs Derinlikleri yok oldu
- seed 15, 30. yıl (gün 3554): Sınır Bekçileri başkenti kaybetti: Gözcüağaç (Kanlıdiş Kabileleri aldı)
- seed 15, 34. yıl (gün 4019): Kızılboynuz Soyu başkenti kaybetti: Boynuztepe (Kanlıdiş Kabileleri aldı)
- seed 15, 44. yıl (gün 5189): Kızılboynuz Soyu başkenti kaybetti: Pınardere (Kanlıdiş Kabileleri aldı)
- seed 15, 46. yıl (gün 5421): Sınır Bekçileri başkenti kaybetti: Kurtgeçit (Kanlıdiş Kabileleri aldı)
- seed 15, 52. yıl (gün 6223): Kızılboynuz Soyu başkenti kaybetti: Taşdere (Pulzırh Lejyonu aldı)
- seed 15, 55. yıl (gün 6493): Kızılboynuz Soyu başkenti kaybetti: Bozkent (Pulzırh Lejyonu aldı)
- seed 15, 57. yıl (gün 6739): Kanlıdiş Kabileleri başkenti kaybetti: Pınardere (Pulzırh Lejyonu aldı)
- seed 16, 7. yıl (gün 778): Güneştacı Krallığı yok oldu
- seed 16, 21. yıl (gün 2493): Karaörs Derinlikleri başkenti kaybetti: Kara Mihrap (Sınır Bekçileri aldı)
- seed 16, 29. yıl (gün 3418): Örsyürek Tapınak Klanı başkenti kaybetti: Taşkandil (Karaörs Derinlikleri aldı)
- seed 16, 31. yıl (gün 3664): Karaörs Derinlikleri başkenti kaybetti: Fıçıköy (Sınır Bekçileri aldı)
- seed 16, 32. yıl (gün 3837): Örsyürek Tapınak Klanı başkenti kaybetti: Közkapı (Karaörs Derinlikleri aldı)
- seed 16, 35. yıl (gün 4125): Örsyürek Tapınak Klanı başkenti kaybetti: Gökyurt (Kanlıdiş Kabileleri aldı)
- seed 16, 38. yıl (gün 4460): Örsyürek Tapınak Klanı yok oldu
- seed 16, 44. yıl (gün 5220): Karaörs Derinlikleri başkenti kaybetti: Demirçan (Kanlıdiş Kabileleri aldı)

## Yıllık ayrıntı

Hücre: medyan (p10–p90), 16 dünya. Yıl y = (y−1)·120+1 … y·120. günler. Bütün değerler `report.json` içinde (`metrics`), dünya başına değerler `../runs/b1-6` altında.

### Medeniyet (1/4)

| Yıl | Yaşayan medeniyet | Yeni medeniyet (yeniden doğan) | Yok olan medeniyet | Başkent kaybı (medeniyet yaşarken) | Çöküş (yok olma + başkent kaybı) | Yaşayan yerleşim |
|---|---|---|---|---|---|---|
| 1 | 8 (7–9) | 0 | 0 | 0 | 0 | 8 (7–9) |
| 2 | 8 (7–9) | 0 | 0 | 0 | 0 | 8 (7–9) |
| 3 | 8 (7–9) | 0 | 0 | 0 | 0 | 8 (7–9) |
| 4 | 8 (7–9) | 0 | 0 | 0 | 0 | 8 (7–9,5) |
| 5 | 8 (7–9) | 0 | 0 | 0 | 0 | 10 (8–12) |
| 6 | 8 (7–9) | 0 | 0 | 0 | 0 | 13 (11–15) |
| 7 | 8 (7–9) | 0 | 0 | 0 | 0 | 17 (14–20) |
| 8 | 8 (7–9) | 0 | 0 | 0 | 0 | 21 (18–25,5) |
| 9 | 8 (7–9) | 0 | 0 | 0 | 0 | 25,5 (22–31,5) |
| 10 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 29,5 (26–35,5) |
| 11 | 8 (7–9) | 0 | 0 | 0 | 0 | 34 (29,5–39,5) |
| 12 | 8 (7–9) | 0 | 0 | 0 | 0 | 36 (32–42,5) |
| 13 | 8 (7–9) | 0 | 0 | 0 | 0 | 40 (35–47) |
| 14 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–1) | 42,5 (37–48) |
| 15 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 46 (40,5–52) |
| 16 | 8 (7–9) | 0 | 0 | 0 | 0 | 50 (42,5–55,5) |
| 17 | 8 (7–9) | 0 | 0 | 0 | 0 (0–0,5) | 53 (44–59,5) |
| 18 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 54,5 (44,5–62) |
| 19 | 8 (7–9) | 0 | 0 | 0 | 0 | 56 (46–64,5) |
| 20 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 57 (49–65) |
| 21 | 8 (7–9) | 0 | 0 | 0 | 0 | 59 (49,5–66) |
| 22 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 61 (52–67,5) |
| 23 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–1) | 62,5 (52,5–68) |
| 24 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 64 (54–70) |
| 25 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 65,5 (54,5–71,5) |
| 26 | 8 (7–9) | 0 | 0 | 0 | 0 | 66,5 (55–73,5) |
| 27 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 68 (57–75) |
| 28 | 8 (7–9) | 0 | 0 | 0 | 0 | 69 (58–76) |
| 29 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 70 (58–76,5) |
| 30 | 8 (7–9) | 0 | 0 | 0 | 0 | 70,5 (58–77,5) |
| 31 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 71 (58,5–78,5) |
| 32 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 72 (58,5–79) |
| 33 | 8 (7–9) | 0 | 0 | 0 | 0 | 72,5 (58,5–79,5) |
| 34 | 8 (7–9) | 0 | 0 | 0 | 0 | 73 (60–80,5) |
| 35 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 74 (60,5–81,5) |
| 36 | 8 (7–9) | 0 (0–0,5) | 0 | 0 | 0 | 74,5 (60,5–81) |
| 37 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 76 (61–81) |
| 38 | 8 (7–9) | 0 | 0 (0–1) | 0 | 0 (0–1) | 76 (61,5–81,5) |
| 39 | 8 (7–9,5) | 0 (0–1) | 0 | 0 (0–1) | 0 (0–1) | 76,5 (62–82) |
| 40 | 8 (7–9,5) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 77 (62,5–83,5) |
| 41 | 8 (7–9) | 0 | 0 (0–0,5) | 0 | 0 (0–1) | 77 (62,5–84,5) |
| 42 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 77,5 (63,5–85) |
| 43 | 8 (7–9) | 0 | 0 | 0 | 0 | 78 (63,5–85) |
| 44 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 78 (63,5–85) |
| 45 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 78,5 (63,5–86) |
| 46 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 79,5 (64–87) |
| 47 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 80,5 (65–88) |
| 48 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 80,5 (65,5–89) |
| 49 | 8 (7–9,5) | 0 (0–0,5) | 0 | 0 (0–0,5) | 0 (0–0,5) | 83 (66,5–89,5) |
| 50 | 8 (7–9,5) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 83,5 (66,5–90,5) |
| 51 | 8 (7–9,5) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 83,5 (67–92) |
| 52 | 8 (7–9,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 84 (67,5–92) |
| 53 | 8 (7–10) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 85 (68–93) |
| 54 | 8 (7–10) | 0 | 0 | 0 (0–1) | 0 (0–1) | 85,5 (68–94) |
| 55 | 8 (7–10) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 86 (68,5–94,5) |
| 56 | 8 (6,5–10) | 0 | 0 | 0 (0–1) | 0 (0–1) | 86 (68,5–96,5) |
| 57 | 8 (6,5–10) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 87,5 (68,5–97,5) |
| 58 | 8 (6,5–10) | 0 | 0 | 0 | 0 | 88,5 (68,5–99) |
| 59 | 8,5 (6,5–10) | 0 | 0 | 0 | 0 | 88 (69–100) |
| 60 | 8,5 (6,5–10) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 89,5 (69–102) |

### Medeniyet (2/4)

| Yıl | Medeniyet başına yerleşim | 5+ kara yerleşimli medeniyet payı | Kurulan yerleşim | Fethedilen yerleşim | Terk edilen yerleşim | Toplam nüfus |
|---|---|---|---|---|---|---|
| 1 | 1 | %0 | 0 | 0 | 0 | 64,5 (55–75) |
| 2 | 1 | %0 | 0 | 0 | 0 | 85,5 (72–99) |
| 3 | 1 | %0 | 0 | 0 | 0 | 112 (94,5–129) |
| 4 | 1 (1–1,06) | %0 | 0 (0–0,5) | 0 | 0 | 142 (117–166) |
| 5 | 1,25 (1,12–1,4) | %0 | 2 (1–3) | 0 | 0 | 190 (144–220) |
| 6 | 1,6 (1,4–1,86) | %0 | 3 (1,5–4,5) | 0 | 0 | 238 (188–276) |
| 7 | 2,18 (1,8–2,46) | %0 | 4 (2,5–6) | 0 | 0 | 308 (242–356) |
| 8 | 2,71 (2,28–3,07) | %0 (%0–%5,6) | 4,5 (3–6) | 0 | 0 | 392 (315–468) |
| 9 | 3,29 (2,78–3,64) | %12 (%0–%29) | 5 (3–6) | 0 | 0 | 512 (424–606) |
| 10 | 3,88 (3,29–4,14) | %33 (%18–%50) | 4 (2,5–6) | 0 (0–1,5) | 0 | 630 (532–742) |
| 11 | 4,25 (3,82–4,57) | %50 (%33–%64) | 3,5 (2,5–5) | 0 (0–1) | 0 | 732 (622–886) |
| 12 | 4,67 (4,25–5,07) | %73 (%44–%86) | 3,5 (1,5–5,5) | 0 (0–2) | 0 | 872 (740–1026) |
| 13 | 5,06 (4,6–5,29) | %76 (%53–%88) | 3 (1,5–5) | 0 (0–1) | 0 | 998 (852–1173) |
| 14 | 5,33 (5,14–6) | %87 (%69–%100) | 3 (1–5) | 0 (0–2,5) | 0 | 1126 (960–1330) |
| 15 | 5,76 (5,38–6,32) | %88 (%69–%100) | 3 (1–4,5) | 0 (0–1,5) | 0 | 1284 (1066–1482) |
| 16 | 6,14 (5,83–6,63) | %100 (%80–%100) | 3 (2–5) | 0 (0–1,5) | 0 (0–0,5) | 1430 (1198–1632) |
| 17 | 6,6 (6,21–6,88) | %100 (%87–%100) | 3 (1–4,5) | 0 (0–2) | 0 | 1587 (1297–1780) |
| 18 | 6,82 (6,31–7,18) | %100 (%88–%100) | 1,5 (1–3,5) | 1 (0–2) | 0 | 1717 (1394–1894) |
| 19 | 7,06 (6,51–7,35) | %100 (%88–%100) | 1 (0,5–3) | 1 (0–1,5) | 0 | 1862 (1524–2053) |
| 20 | 7,17 (6,87–7,7) | %100 (%88–%100) | 2 (1–4) | 1 (0–2) | 0 | 1969 (1595–2172) |
| 21 | 7,47 (7–7,93) | %100 (%88–%100) | 1,5 (0–3) | 1 (0–2) | 0 | 2091 (1737–2286) |
| 22 | 7,63 (7,21–8,06) | %100 (%87–%100) | 2 (0–3) | 0,5 (0–2) | 0 | 2100 (1761–2318) |
| 23 | 7,8 (7,31–8,27) | %100 (%87–%100) | 1 (0,5–3) | 1 (0–2) | 0 | 2224 (1926–2422) |
| 24 | 7,88 (7,56–8,27) | %100 (%88–%100) | 1 (0–2,5) | 0,5 (0–2) | 0 | 2311 (1988–2524) |
| 25 | 8,25 (7,67–8,63) | %100 (%88–%100) | 1 (0–2,5) | 1 (0–2,5) | 0 | 2414 (2074–2643) |
| 26 | 8,4 (7,79–8,75) | %100 (%88–%100) | 1 (0,5–2) | 0,5 (0–3) | 0 (0–1) | 2446 (2154–2762) |
| 27 | 8,51 (7,99–8,94) | %100 (%94–%100) | 1 (0,5–2) | 1 (0–3) | 0 | 2557 (2218–2809) |
| 28 | 8,57 (8,19–9,13) | %100 (%88–%100) | 1 (0–2) | 1 (0–2) | 0 | 2620 (2238–2958) |
| 29 | 8,69 (8,22–9,2) | %100 (%88–%100) | 1 (0–2) | 1 (0–3,5) | 0 (0–0,5) | 2698 (2319–3016) |
| 30 | 8,8 (8,28–9,33) | %100 (%88–%100) | 1 (0–2) | 1 (0–2) | 0 | 2804 (2354–3098) |
| 31 | 8,88 (8,36–9,53) | %100 (%88–%100) | 1 (0–1,5) | 1 (0–3) | 0 (0–0,5) | 2884 (2492–3083) |
| 32 | 9 (8,36–9,56) | %100 (%88–%100) | 0,5 (0–2) | 1 (0–2,5) | 0 | 2898 (2582–3242) |
| 33 | 9,06 (8,36–9,66) | %100 (%88–%100) | 1 (0–1) | 1 (0–2) | 0 (0–0,5) | 2904 (2622–3300) |
| 34 | 9,06 (8,57–9,75) | %100 (%88–%100) | 1 (0–2) | 1 (0–3,5) | 0 | 2963 (2630–3410) |
| 35 | 9,13 (8,38–9,75) | %100 (%88–%100) | 1 (0–1,5) | 1 (0–2) | 0 | 3062 (2734–3490) |
| 36 | 9,25 (8,43–9,81) | %95 (%88–%100) | 0 (0–2) | 2 (0–3,5) | 0 | 3143 (2782–3581) |
| 37 | 9,3 (8,49–9,88) | %100 (%88–%100) | 0,5 (0–1,5) | 1,5 (0–3) | 0 (0–0,5) | 3174 (2783–3527) |
| 38 | 9,5 (8,57–10,5) | %100 (%88–%100) | 0,5 (0–1) | 1 (0–3) | 0 (0–0,5) | 3214 (2820–3610) |
| 39 | 9,31 (8,54–10,4) | %100 (%88–%100) | 1 (0–1) | 1 (0–2) | 0 | 3222 (2868–3726) |
| 40 | 9,44 (8,6–10,6) | %100 (%88–%100) | 1 (0–1,5) | 1 (0–2) | 0 (0–1) | 3328 (2934–3768) |
| 41 | 9,59 (8,71–10,9) | %100 (%87–%100) | 1 (0–2) | 1 (0–3) | 0 | 3406 (3028–3788) |
| 42 | 9,59 (8,76–10,9) | %100 (%87–%100) | 0 (0–1,5) | 1 (0–2,5) | 0 | 3386 (2981–3776) |
| 43 | 9,65 (8,81–10,9) | %100 (%87–%100) | 0,5 (0–1) | 1,5 (0–2,5) | 0 (0–0,5) | 3403 (3026–3846) |
| 44 | 9,53 (8,71–11) | %95 (%87–%100) | 0 (0–1,5) | 2 (0–3) | 0 (0–0,5) | 3464 (3200–3910) |
| 45 | 9,56 (8,71–11) | %100 (%87–%100) | 0 (0–1,5) | 1 (0–2) | 0 (0–1) | 3489 (3134–3929) |
| 46 | 9,67 (8,82–11) | %95 (%87–%100) | 0 (0–1,5) | 1 (0–3,5) | 0 | 3506 (3220–3984) |
| 47 | 9,67 (9–11,1) | %100 (%83–%100) | 0 (0–2) | 1 (0–3,5) | 0 | 3571 (3283–4097) |
| 48 | 9,8 (9–11,2) | %100 (%83–%100) | 1 (0–1) | 1 (0–2,5) | 0 | 3548 (3314–4098) |
| 49 | 9,74 (8,95–11,4) | %95 (%84–%100) | 1 (0–2) | 1,5 (0–3) | 0 | 3566 (3366–4178) |
| 50 | 9,8 (9,07–11,4) | %100 (%83–%100) | 1 (0–2) | 1 (0–2,5) | 0 | 3584 (3324–4182) |
| 51 | 9,88 (9,12–11,5) | %100 (%82–%100) | 0,5 (0–1,5) | 1 (0–3) | 0 (0–0,5) | 3748 (3480–4223) |
| 52 | 9,94 (9,24–11,6) | %89 (%82–%100) | 1 (0–1) | 1 (0,5–2,5) | 0 | 3742 (3446–4231) |
| 53 | 10,1 (9–11,7) | %90 (%78–%100) | 1 (0–2) | 1 (0–2,5) | 0 (0–1) | 3775 (3508–4278) |
| 54 | 10,1 (9–11,7) | %95 (%79–%100) | 1 (0–1) | 1 (0–3) | 0 (0–0,5) | 3774 (3415–4352) |
| 55 | 10,2 (9,11–11,8) | %95 (%83–%100) | 0,5 (0–2) | 1 (0–2) | 0 | 3742 (3388–4434) |
| 56 | 10,4 (9,33–12) | %100 (%88–%100) | 0,5 (0–2) | 1,5 (0–2,5) | 0 (0–0,5) | 3863 (3550–4380) |
| 57 | 10,6 (9,05–12,1) | %95 (%84–%100) | 0,5 (0–2) | 1 (0–2,5) | 0 | 3919 (3536–4495) |
| 58 | 10,5 (9,2–11,9) | %89 (%84–%100) | 0,5 (0–2) | 1 (0–2) | 0 | 3908 (3637–4554) |
| 59 | 10,4 (9,2–11,9) | %90 (%88–%100) | 1 (0–1) | 1 (0–3) | 0 (0–0,5) | 3874 (3575–4678) |
| 60 | 10,5 (9,3–11,9) | %95 (%84–%100) | 1 (0–2) | 1 (0–3) | 0 (0–0,5) | 3926 (3644–4745) |

### Medeniyet (3/4)

| Yıl | Ortalama çağ | Araştırma ağacının biten payı (ort.) | Ağacı bitmiş medeniyet payı | Araştırması duran medeniyet payı | Altın medyanı (medeniyetler) | Boştaki iş gücü payı |
|---|---|---|---|---|---|---|
| 1 | 1 | %0,4 (%0–%0,8) | %0 | %0 | 8,42 (8,29–15,9) | %0 |
| 2 | 1 | %4 (%3,5–%4,3) | %0 | %0 | 24,4 (13,2–37,5) | %0 |
| 3 | 1,13 (1–1,29) | %8 (%7,4–%8,5) | %0 | %0 | 35,7 (21,2–48,2) | %0,3 (%0–%2) |
| 4 | 1,88 (1,76–2) | %12 (%11–%13) | %0 | %0 | 42,1 (29,3–60,1) | %0,3 (%0–%1,2) |
| 5 | 2 (1,86–2) | %15 (%14–%16) | %0 | %0 | 48,2 (38,1–68,4) | %0,2 (%0–%1,2) |
| 6 | 2 (2–2,13) | %19 (%18–%20) | %0 | %0 | 54 (44–77,3) | %0,3 (%0–%1,6) |
| 7 | 2,13 (2,06–2,29) | %24 (%23–%26) | %0 | %0 | 72,2 (39,5–99,3) | %0,9 (%0,1–%2,1) |
| 8 | 2,38 (2,27–2,6) | %29 (%28–%33) | %0 | %0 (%0–%5,6) | 78,1 (54,8–104) | %1 (%0,2–%2) |
| 9 | 2,57 (2,37–2,73) | %35 (%33–%39) | %0 | %0 (%0–%12) | 94,2 (47,9–170) | %0,9 (%0,4–%2,2) |
| 10 | 2,78 (2,67–2,94) | %42 (%39–%46) | %0 | %0 (%0–%18) | 98,3 (57,7–151) | %1,2 (%0,4–%2,3) |
| 11 | 2,89 (2,71–3,14) | %49 (%45–%53) | %0 | %0 (%0–%27) | 126 (46,4–199) | %2,1 (%0,4–%3,6) |
| 12 | 3,13 (3–3,24) | %55 (%51–%60) | %0 | %12 (%0–%25) | 143 (54,3–252) | %3,6 (%1–%4,8) |
| 13 | 3,14 (3–3,44) | %61 (%57–%67) | %0 (%0–%12) | %25 (%5,6–%44) | 224 (130–370) | %7,1 (%2,8–%10) |
| 14 | 3,27 (3,07–3,53) | %66 (%63–%72) | %0 (%0–%14) | %27 (%12–%57) | 367 (122–812) | %8,6 (%4,9–%16) |
| 15 | 3,41 (3,11–3,67) | %72 (%67–%76) | %12 (%0–%14) | %33 (%20–%64) | 483 (193–972) | %15 (%8,8–%20) |
| 16 | 3,53 (3,29–3,69) | %76 (%71–%79) | %13 (%5,6–%29) | %53 (%31–%65) | 574 (248–1021) | %20 (%12–%23) |
| 17 | 3,63 (3,33–3,83) | %79 (%74–%83) | %29 (%12–%46) | %62 (%40–%81) | 1092 (317–1368) | %24 (%19–%27) |
| 18 | 3,63 (3,4–3,88) | %82 (%76–%85) | %38 (%22–%60) | %71 (%54–%88) | 1303 (422–2078) | %27 (%23–%35) |
| 19 | 3,73 (3,5–3,88) | %83 (%78–%89) | %46 (%25–%67) | %73 (%57–%88) | 1663 (507–2808) | %34 (%26–%39) |
| 20 | 3,73 (3,56–3,94) | %85 (%81–%91) | %54 (%27–%75) | %75 (%62–%88) | 1951 (1164–3292) | %35 (%28–%41) |
| 21 | 3,75 (3,65–4) | %87 (%83–%93) | %57 (%27–%71) | %75 (%56–%88) | 2319 (1313–3937) | %36 (%32–%43) |
| 22 | 3,86 (3,69–4) | %89 (%85–%93) | %62 (%35–%76) | %75 (%62–%88) | 2354 (1078–3792) | %40 (%35–%44) |
| 23 | 3,88 (3,71–4) | %91 (%87–%94) | %71 (%54–%83) | %86 (%71–%94) | 2481 (1242–4685) | %41 (%36–%44) |
| 24 | 3,89 (3,75–4) | %93 (%89–%95) | %73 (%57–%94) | %86 (%73–%100) | 2628 (1347–5534) | %43 (%39–%48) |
| 25 | 3,94 (3,79–4) | %93 (%91–%96) | %86 (%69–%100) | %94 (%82–%100) | 3279 (2211–4905) | %48 (%42–%50) |
| 26 | 4 (3,79–4) | %94 (%92–%96) | %87 (%73–%100) | %100 (%82–%100) | 3970 (2409–5752) | %48 (%42–%52) |
| 27 | 4 (3,87–4) | %95 (%92–%96) | %88 (%73–%100) | %94 (%86–%100) | 4036 (2523–6050) | %49 (%43–%54) |
| 28 | 4 (3,87–4) | %96 (%93–%97) | %89 (%73–%100) | %100 (%86–%100) | 4651 (2617–6859) | %49 (%42–%55) |
| 29 | 4 (3,87–4) | %96 (%94–%97) | %94 (%79–%100) | %100 (%86–%100) | 4949 (2672–9209) | %51 (%44–%53) |
| 30 | 4 (3,94–4) | %96 (%94–%97) | %100 (%87–%100) | %100 (%88–%100) | 5866 (3089–10606) | %51 (%47–%53) |
| 31 | 4 | %97 (%95–%98) | %100 (%87–%100) | %100 (%87–%100) | 7614 (2820–11621) | %51 (%47–%55) |
| 32 | 4 | %97 (%95–%99) | %100 (%88–%100) | %100 (%88–%100) | 8410 (4049–12710) | %51 (%48–%53) |
| 33 | 4 | %97 (%96–%99) | %100 (%88–%100) | %100 (%88–%100) | 9008 (4456–12888) | %50 (%45–%55) |
| 34 | 4 | %97 (%96–%99) | %100 | %100 | 10860 (6154–14498) | %52 (%48–%56) |
| 35 | 4 | %97 (%95–%99) | %100 (%94–%100) | %100 (%94–%100) | 9973 (6501–16158) | %50 (%46–%55) |
| 36 | 4 | %97 (%95–%99) | %100 (%89–%100) | %100 (%89–%100) | 9504 (6230–14828) | %49 (%46–%51) |
| 37 | 4 | %97 (%96–%99) | %100 (%88–%100) | %100 (%88–%100) | 8811 (6230–15372) | %50 (%47–%53) |
| 38 | 4 | %97 (%96–%99) | %100 | %100 | 11000 (5074–16635) | %50 (%46–%53) |
| 39 | 4 | %97 (%96–%99) | %100 (%89–%100) | %100 (%89–%100) | 12804 (4779–20491) | %49 (%46–%53) |
| 40 | 4 | %97 (%96–%99) | %100 (%89–%100) | %100 (%89–%100) | 12559 (5045–19842) | %48 (%45–%52) |
| 41 | 4 | %97 (%96–%99) | %100 (%94–%100) | %100 (%94–%100) | 11454 (5473–22053) | %47 (%41–%52) |
| 42 | 4 | %98 (%96–%99) | %100 | %100 | 9820 (5485–22740) | %47 (%43–%51) |
| 43 | 4 | %98 (%96–%99) | %100 (%94–%100) | %100 (%94–%100) | 9119 (4302–25998) | %47 (%45–%54) |
| 44 | 4 | %98 (%96–%99) | %100 (%95–%100) | %100 (%95–%100) | 9386 (4204–25260) | %48 (%43–%52) |
| 45 | 4 | %98 (%96–%99) | %100 (%94–%100) | %100 (%94–%100) | 10150 (5843–26689) | %47 (%45–%54) |
| 46 | 4 | %98 (%96–%99) | %100 | %100 | 11011 (4480–25591) | %47 (%44–%53) |
| 47 | 4 | %98 (%96–%99) | %100 (%94–%100) | %100 (%94–%100) | 13143 (5486–28346) | %48 (%44–%53) |
| 48 | 4 | %98 (%96–%99) | %100 | %100 | 11416 (5784–29613) | %47 (%44–%52) |
| 49 | 4 | %98 (%96–%99) | %100 (%95–%100) | %100 (%95–%100) | 14598 (5959–30619) | %48 (%44–%54) |
| 50 | 4 | %98 (%96–%99) | %100 | %100 | 16598 (6222–30831) | %49 (%44–%53) |
| 51 | 4 | %98 (%96–%99) | %100 | %100 | 18732 (5116–32466) | %47 (%42–%53) |
| 52 | 4 | %98 (%96–%99) | %100 | %100 | 22026 (4853–34385) | %49 (%43–%53) |
| 53 | 4 | %99 (%96–%99) | %100 (%90–%100) | %100 (%90–%100) | 21375 (5530–35692) | %49 (%41–%52) |
| 54 | 4 | %99 (%96–%99) | %100 (%95–%100) | %100 (%95–%100) | 22462 (5510–35703) | %48 (%42–%54) |
| 55 | 4 | %99 (%96–%99) | %100 | %100 | 22124 (5780–36786) | %46 (%41–%52) |
| 56 | 4 | %99 (%96–%99) | %100 (%95–%100) | %100 (%95–%100) | 22827 (6428–38500) | %45 (%41–%52) |
| 57 | 4 | %99 (%96–%100) | %100 | %100 | 22873 (5150–39767) | %48 (%40–%51) |
| 58 | 4 | %99 (%96–%100) | %100 | %100 | 23969 (5769–40760) | %46 (%43–%51) |
| 59 | 4 | %99 (%96–%100) | %100 | %100 | 26037 (5185–41885) | %47 (%39–%53) |
| 60 | 4 | %99 (%96–%100) | %100 | %100 | 25734 (6540–43200) | %46 (%41–%51) |

### Medeniyet (4/4)

| Yıl | Bölünme (ayrılıp kurulan medeniyet) | En büyük medeniyetin yerleşimi |
|---|---|---|
| 1 | 0 | 1 |
| 2 | 0 | 1 |
| 3 | 0 | 1 |
| 4 | 0 | 1 (1–1,5) |
| 5 | 0 | 2 |
| 6 | 0 | 3 (2–3) |
| 7 | 0 | 3 (3–4) |
| 8 | 0 | 4 (3,5–4,5) |
| 9 | 0 | 5 (4–5) |
| 10 | 0 | 5 (5–6) |
| 11 | 0 | 6 (5,5–6) |
| 12 | 0 | 6 (6–7) |
| 13 | 0 | 7 (6–8) |
| 14 | 0 | 7 (6–8) |
| 15 | 0 | 8 (7–9) |
| 16 | 0 | 8 (7–9) |
| 17 | 0 | 9 (7,5–9) |
| 18 | 0 | 9 (8–9,5) |
| 19 | 0 | 9 (8,5–10) |
| 20 | 0 | 9 (9–10,5) |
| 21 | 0 | 9 (9–11) |
| 22 | 0 | 10 (9–11,5) |
| 23 | 0 | 11 (9–12) |
| 24 | 0 | 10,5 (9–12,5) |
| 25 | 0 | 11,5 (9–13) |
| 26 | 0 | 11,5 (9–14) |
| 27 | 0 | 11,5 (9,5–14) |
| 28 | 0 | 12,5 (10–14) |
| 29 | 0 | 12,5 (10–15) |
| 30 | 0 | 13 (10–15) |
| 31 | 0 | 13 (10–15,5) |
| 32 | 0 | 13,5 (10–16) |
| 33 | 0 | 13,5 (10–16,5) |
| 34 | 0 | 14 (10,5–16,5) |
| 35 | 0 | 14 (10,5–16) |
| 36 | 0 (0–0,5) | 14 (10,5–16,5) |
| 37 | 0 | 14 (10,5–17) |
| 38 | 0 | 13,5 (11–17,5) |
| 39 | 0 (0–1) | 14 (11–18) |
| 40 | 0 | 14 (11–18) |
| 41 | 0 | 14 (11–18,5) |
| 42 | 0 | 14,5 (11–18,5) |
| 43 | 0 | 14,5 (12–19) |
| 44 | 0 | 14,5 (11,5–19) |
| 45 | 0 | 14,5 (12–19) |
| 46 | 0 | 15 (12–19,5) |
| 47 | 0 | 15 (12–19,5) |
| 48 | 0 | 15 (12–20,5) |
| 49 | 0 (0–0,5) | 15 (12,5–20,5) |
| 50 | 0 | 15,5 (13–21) |
| 51 | 0 | 16 (12,5–21,5) |
| 52 | 0 | 16 (12,5–22) |
| 53 | 0 | 16 (13–23) |
| 54 | 0 | 16,5 (13–23,5) |
| 55 | 0 | 17 (12,5–24) |
| 56 | 0 | 17 (13–24) |
| 57 | 0 | 17,5 (13,5–24,5) |
| 58 | 0 | 18 (13,5–25,5) |
| 59 | 0 | 18 (13–25,5) |
| 60 | 0 | 18 (12,5–26,5) |

### Olaylar

| Yıl | Olay | Büyük olay |
|---|---|---|
| 1 | 37 (28–43,5) | 14 (12–19) |
| 2 | 60,5 (44,5–68,5) | 20,5 (14,5–23,5) |
| 3 | 34 (26,5–48) | 6 (3,5–11) |
| 4 | 47,5 (41,5–63) | 17 (14–23) |
| 5 | 57 (45,5–71) | 22,5 (17,5–30) |
| 6 | 79 (62–87,5) | 30 (23–40) |
| 7 | 95,5 (81–123) | 41 (33,5–50) |
| 8 | 114 (99,5–136) | 46,5 (40–54) |
| 9 | 126 (96–152) | 49 (40–58,5) |
| 10 | 152 (122–197) | 54 (46–70) |
| 11 | 166 (130–223) | 67 (52–86,5) |
| 12 | 177 (148–226) | 71,5 (55,5–90,5) |
| 13 | 193 (150–236) | 69,5 (58–87,5) |
| 14 | 194 (160–243) | 76,5 (53,5–93) |
| 15 | 204 (151–228) | 70 (54–87,5) |
| 16 | 186 (152–238) | 63 (51,5–84) |
| 17 | 188 (136–244) | 61 (47–92) |
| 18 | 178 (140–232) | 57 (44,5–77,5) |
| 19 | 176 (127–215) | 58,5 (41,5–68) |
| 20 | 167 (144–205) | 53 (42,5–73) |
| 21 | 161 (138–203) | 55,5 (44–72,5) |
| 22 | 193 (150–222) | 65,5 (52,5–82,5) |
| 23 | 181 (150–222) | 56,5 (47–77,5) |
| 24 | 170 (133–196) | 53,5 (42,5–68,5) |
| 25 | 182 (124–205) | 59,5 (32–75) |
| 26 | 156 (122–196) | 52 (29,5–73,5) |
| 27 | 164 (116–223) | 46,5 (36,5–65) |
| 28 | 158 (106–194) | 48 (32–57) |
| 29 | 158 (108–196) | 48,5 (30–70) |
| 30 | 156 (115–195) | 42 (32–73,5) |
| 31 | 155 (122–215) | 49,5 (32–83,5) |
| 32 | 146 (92–206) | 47,5 (26,5–74,5) |
| 33 | 158 (112–185) | 46 (30,5–62,5) |
| 34 | 152 (120–210) | 49,5 (37,5–61) |
| 35 | 146 (109–206) | 49 (28–59,5) |
| 36 | 134 (104–212) | 43 (31–67) |
| 37 | 152 (108–190) | 55 (36–63) |
| 38 | 136 (106–194) | 46,5 (32–60) |
| 39 | 157 (124–194) | 48,5 (36,5–54) |
| 40 | 150 (122–192) | 49,5 (35–67,5) |
| 41 | 150 (108–189) | 50 (32–65,5) |
| 42 | 148 (114–210) | 46,5 (36–59) |
| 43 | 148 (116–195) | 48,5 (33–71,5) |
| 44 | 155 (112–174) | 45,5 (30–61,5) |
| 45 | 168 (122–230) | 57 (41–80) |
| 46 | 169 (132–208) | 47,5 (37,5–73,5) |
| 47 | 174 (134–204) | 54,5 (38,5–68,5) |
| 48 | 158 (120–208) | 52,5 (34–66,5) |
| 49 | 166 (134–206) | 52,5 (39–71) |
| 50 | 156 (119–216) | 52,5 (30–85) |
| 51 | 158 (122–210) | 46,5 (34,5–61) |
| 52 | 166 (118–200) | 56 (41,5–76) |
| 53 | 160 (130–210) | 51,5 (34,5–73,5) |
| 54 | 168 (129–192) | 59 (42,5–70,5) |
| 55 | 148 (119–190) | 49 (36–62) |
| 56 | 160 (128–202) | 58 (39,5–70) |
| 57 | 168 (112–212) | 59,5 (33,5–74) |
| 58 | 164 (113–206) | 48 (31,5–69,5) |
| 59 | 176 (110–242) | 55,5 (32,5–81) |
| 60 | 162 (114–231) | 56,5 (34–78,5) |

### Savaş (1/2)

| Yıl | Muharebe | Başlayan savaş | Süren savaş (yıl sonu) | Yıl içinde süren savaş | Yağma akını (medeniyet) | Tarihî hak savaşı |
|---|---|---|---|---|---|---|
| 1 | 0 | 0 | 0 | 0 | 0 | 0 |
| 2 | 0 | 0 | 0 | 0 | 0 | 0 |
| 3 | 0 (0–1) | 0 | 0 | 0 | 0 | 0 |
| 4 | 4 (3–5) | 0 | 0 | 0 | 0 | 0 |
| 5 | 4,5 (2,5–6) | 0 | 0 | 0 | 0 | 0 |
| 6 | 6 (4–8) | 0 | 0 | 0 | 0 | 0 |
| 7 | 6 (4–7,5) | 0 | 0 | 0 | 0 | 0 |
| 8 | 6 (5–8,5) | 0 (0–0,5) | 0 | 0 (0–0,5) | 0 (0–0,5) | 0 |
| 9 | 7 (3,5–9) | 0 (0–1) | 0 (0–1) | 0 (0–1) | 0 (0–1,5) | 0 |
| 10 | 9 (6–11) | 0 (0–1,5) | 0 | 0 (0–1,5) | 0 (0–1) | 0 |
| 11 | 8,5 (6–14,5) | 0 (0–1,5) | 0 (0–0,5) | 0 (0–1,5) | 1 (0–2) | 0 |
| 12 | 8,5 (6,5–14) | 0 (0–2) | 0 | 0 (0–2) | 0 (0–1,5) | 0 (0–0,5) |
| 13 | 9 (6,5–11) | 0 (0–2) | 0 (0–0,5) | 0,5 (0–2) | 0,5 (0–2,5) | 0 (0–0,5) |
| 14 | 9 (5,5–12) | 0 (0–2) | 0 (0–1,5) | 0,5 (0–2,5) | 1 (0–2) | 0 (0–0,5) |
| 15 | 7,5 (4,5–10,5) | 0 (0–1) | 0 | 0,5 (0–2,5) | 0 (0–2) | 0 |
| 16 | 5,5 (3,5–11,5) | 1 (0–2) | 0 (0–1) | 1 (0–2) | 1 (0–3) | 0 |
| 17 | 7 (3–12) | 0,5 (0–2,5) | 0 (0–1) | 1 (0–3) | 1 (0–2) | 0 (0–0,5) |
| 18 | 7 (4–9,5) | 1 (0–2) | 0 (0–1,5) | 1 (0,5–2) | 0,5 (0–2) | 0 (0–0,5) |
| 19 | 7,5 (5,5–9,5) | 0 (0–2,5) | 0 (0–0,5) | 1 (0–3) | 1,5 (0–2,5) | 0 (0–0,5) |
| 20 | 5,5 (3–8,5) | 1 (0–3) | 0 (0–1,5) | 1 (0–3,5) | 0,5 (0–2) | 0 |
| 21 | 7,5 (6,5–10) | 1 (0–2,5) | 0 (0–1) | 1,5 (0–2,5) | 1 (0–3) | 0 |
| 22 | 9,5 (6–13) | 1 (0–3) | 0 (0–1,5) | 1 (0–3,5) | 1 (0–3) | 0 (0–1) |
| 23 | 8 (5–12) | 1 (0–3) | 0 (0–1) | 1,5 (0–4) | 1 (0–3) | 0 (0–1) |
| 24 | 7 (5–11,5) | 1 (0–2) | 0 (0–1,5) | 2 (0–3) | 2 (0,5–3) | 0 |
| 25 | 8 (5,5–12,5) | 1 (0–3) | 0 (0–1) | 1 (0,5–5) | 1,5 (0–3,5) | 0 |
| 26 | 7 (4,5–12,5) | 1 (0–3,5) | 0 (0–2) | 1,5 (0–4) | 1,5 (0–3) | 0 (0–0,5) |
| 27 | 8 (4,5–12,5) | 1,5 (0–4) | 0 (0–2) | 2 (0–4) | 2 (0,5–3,5) | 0 (0–1) |
| 28 | 8,5 (5,5–11,5) | 1 (0–3,5) | 0 (0–2) | 2 (0–3,5) | 2 (0–3) | 0 (0–1) |
| 29 | 10 (6–13,5) | 1,5 (0–3,5) | 0 (0–0,5) | 2 (0–5) | 2 (0–5) | 0 (0–1) |
| 30 | 8 (4–11,5) | 1,5 (0–3) | 0 (0–2) | 2 (0–3,5) | 1 (0–3) | 0 (0–1) |
| 31 | 8 (5–11) | 1 (0–4) | 0 (0–2,5) | 2 (0–5) | 2 (0–4) | 0 (0–1,5) |
| 32 | 9,5 (5–13) | 1 (0–4) | 0 (0–0,5) | 1,5 (0–5) | 1,5 (0–4,5) | 0 |
| 33 | 10 (5–13) | 1 (0–3) | 0 (0–1,5) | 1 (0–3) | 1,5 (0–4,5) | 0 (0–1) |
| 34 | 9 (6,5–13,5) | 1,5 (0–4,5) | 0 (0–1) | 2 (0–5,5) | 1,5 (0–3,5) | 0 (0–1) |
| 35 | 11 (6–13) | 2 (0–2,5) | 0 (0–2,5) | 2 (0–3,5) | 2 (0–5,5) | 0 (0–1) |
| 36 | 9 (5,5–12) | 2 (0–3,5) | 0 (0–1) | 3 (0,5–4) | 2 (0–5) | 0 (0–1) |
| 37 | 10 (6,5–15) | 2 (0,5–3,5) | 0 (0–1) | 2 (0,5–5) | 2 (0–4) | 0 |
| 38 | 10 (7–15) | 2 (0–2) | 0 (0–1) | 2 (0–3,5) | 2 (0–4) | 0 (0–1) |
| 39 | 9,5 (5–14) | 1,5 (0–3,5) | 0 (0–1,5) | 2 (0–3,5) | 2 (0–5) | 0 (0–1) |
| 40 | 10 (7–12) | 1 (0–3) | 0 (0–2) | 2 (0–4) | 2 (0,5–5) | 0 (0–0,5) |
| 41 | 9,5 (6,5–13,5) | 1,5 (0–4) | 0 (0–2) | 2,5 (0,5–4) | 2 (0–5) | 0 (0–0,5) |
| 42 | 9 (5,5–12,5) | 1,5 (0–3,5) | 0 (0–2) | 2 (0–3,5) | 2 (0–4) | 0 (0–0,5) |
| 43 | 9,5 (6–13) | 2,5 (0–5) | 0 (0–1) | 3 (0–5) | 2 (0,5–5) | 0 (0–1,5) |
| 44 | 9 (5,5–14) | 2 (0–2) | 0 (0–1) | 2 (0–3) | 2 (0–5) | 0 (0–1) |
| 45 | 10 (6,5–14) | 2 (0–3,5) | 0 (0–2) | 2 (0,5–3,5) | 2 (0,5–4,5) | 0 (0–1) |
| 46 | 10 (5,5–15) | 2 (0–5) | 0 (0–2) | 2 (1–5,5) | 2 (0–5) | 0 |
| 47 | 10 (8–14) | 1,5 (0–4) | 0 (0–1,5) | 2 (0,5–4,5) | 2 (0–6) | 0 (0–0,5) |
| 48 | 10 (7–15) | 2 (0–3) | 0 (0–1) | 2 (0–3,5) | 2 (0–5) | 0 (0–1) |
| 49 | 11 (6,5–13,5) | 1,5 (0–4,5) | 0 (0–1,5) | 2,5 (0–4,5) | 2 (0–5) | 0 |
| 50 | 10 (7,5–15) | 2 (0–4) | 0,5 (0–2) | 2 (0,5–5) | 2,5 (0–5,5) | 0 |
| 51 | 10 (5–13) | 0,5 (0–4,5) | 0 (0–1) | 2 (0–5) | 2 (0–5) | 0 (0–1) |
| 52 | 10 (8–17) | 2 (1–4) | 0 (0–2) | 2 (1–4,5) | 2 (0,5–5,5) | 0 |
| 53 | 10 (7–14,5) | 1 (0–3) | 0 (0–2) | 2 (0–4) | 2 (0–5,5) | 0 (0–0,5) |
| 54 | 10,5 (7,5–15) | 2 (0–4) | 0 (0–1) | 2 (0–5) | 2 (0,5–4,5) | 0 |
| 55 | 10 (7–13) | 2 (0,5–3) | 1 (0–2) | 2 (1–3) | 2 (0–5,5) | 0 (0–1) |
| 56 | 10 (6,5–15,5) | 1 (0–3) | 0 (0–1) | 2,5 (0,5–4) | 2 (0–5) | 0 (0–0,5) |
| 57 | 10,5 (4,5–13,5) | 1,5 (0–3,5) | 0 (0–1) | 2 (0–4) | 2 (0–5,5) | 0 (0–1) |
| 58 | 9 (5,5–13) | 2 (0–4) | 0 (0–2) | 2 (0–4) | 2,5 (0–4,5) | 0 (0–1) |
| 59 | 11 (4,5–18,5) | 1 (0–3) | 0 (0–1) | 1 (0–4) | 2,5 (0–6) | 0 |
| 60 | 10 (7,5–15,5) | 2 (0–4,5) | 1 (0–1) | 2 (0–4,5) | 2 (0–5,5) | 0 (0–1) |

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
| 9 | 0 | 0 | 0 | 0 |
| 10 | 0 | 0 (0–0,5) | 0 | 0 |
| 11 | 0 | 0 | 0 | 0 (0–0,5) |
| 12 | 0 | 0 | 0 | 0 (0–0,5) |
| 13 | 0 | 0 | 0 | 0 (0–1) |
| 14 | 0 | 0 | 0 | 0 (0–1) |
| 15 | 0 | 0 | 0 | 0 (0–1) |
| 16 | 0 | 0 | 0 | 0 (0–1) |
| 17 | 0 (0–1) | 0 | 0 | 1 (0–1) |
| 18 | 0 (0–0,5) | 0 | 0 | 1 (0–1) |
| 19 | 0 (0–0,5) | 0 | 0 | 1 (0–1,5) |
| 20 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 21 | 0 (0–0,5) | 0 (0–1) | 0 | 1 (0–2) |
| 22 | 0 (0–1) | 0 | 0 (0–1) | 1 (0–1,5) |
| 23 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 24 | 0 (0–0,5) | 0 | 0 (0–0,5) | 1 (0–1,5) |
| 25 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 26 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 27 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 28 | 0 | 0 | 0 | 1 (0–2) |
| 29 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 30 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 31 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 32 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 33 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 34 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 35 | 0 (0–1) | 0 (0–0,5) | 0 | 1 (0–2) |
| 36 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 37 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 38 | 0 (0–1) | 0 | 0 (0–1) | 1 (0–2) |
| 39 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 40 | 0 | 0 (0–0,5) | 0 | 1 (0–2) |
| 41 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 42 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 43 | 0 (0–1) | 0 | 0 | 1 (0–2,5) |
| 44 | 0 (0–1) | 0 | 0 | 1 (0,5–2,5) |
| 45 | 0 (0–1) | 0 | 0 | 1,5 (0,5–2) |
| 46 | 0 (0–1,5) | 0 | 0 (0–0,5) | 1 (0,5–2) |
| 47 | 0 (0–0,5) | 0 | 0 (0–0,5) | 1 (0,5–2) |
| 48 | 0 (0–0,5) | 0 | 0 | 1 (0,5–2) |
| 49 | 0 (0–0,5) | 0 | 0 | 1 (0,5–2) |
| 50 | 0 (0–1) | 0 | 0 | 1 (0,5–2) |
| 51 | 0 (0–1) | 0 | 0 | 1 (0,5–2) |
| 52 | 0 (0–1) | 0 | 0 | 1 (1–2) |
| 53 | 0 (0–1) | 0 | 0 | 1 (1–2) |
| 54 | 0 (0–0,5) | 0 | 0 | 1 (0,5–2) |
| 55 | 0 (0–1) | 0 | 0 (0–0,5) | 1 (0–2,5) |
| 56 | 0 (0–1) | 0 | 0 | 1 (0–2,5) |
| 57 | 0 (0–1) | 0 | 0 | 1 (0–2,5) |
| 58 | 0 (0–1) | 0 | 0 | 1 (0,5–2,5) |
| 59 | 0 | 0 | 0 (0–0,5) | 1 (0,5–2,5) |
| 60 | 0 (0–1) | 0 | 0 | 1 (0,5–2,5) |

### Canavarlar

| Yıl | Yaşayan kamp (yıl sonu) | Yaşayan kamp (yıl ort.) | Doğan kamp | Temizlenen kamp | Canavar baskını |
|---|---|---|---|---|---|
| 1 | 3 | 3 | 0 | 0 | 0 |
| 2 | 4 (3–5) | 3,1 (3–3,48) | 1 (0–2) | 0 | 0 |
| 3 | 5 (4–6,5) | 4,3 (3,4–5,59) | 1 (0–2,5) | 0 | 0 (0–0,5) |
| 4 | 7 (4,5–8,5) | 6,13 (4,08–7,23) | 2 (1–3) | 0 | 3,5 (2,5–5) |
| 5 | 8 (7–10) | 7,11 (6,19–9,71) | 1 (0,5–2,5) | 0 | 4,5 (2–6) |
| 6 | 9 (7,5–10) | 8,73 (7,12–10) | 1 (0–2) | 0 | 6 (3,5–7,5) |
| 7 | 10 (8,5–10) | 9,9 (8,22–10) | 0 (0–1,5) | 0 | 5 (4–7) |
| 8 | 10 (10–11) | 10 (9,03–10,2) | 0 (0–1,5) | 0 | 6 (4–8) |
| 9 | 10 (10–11) | 10 (10–11) | 0 (0–1) | 0 | 6,5 (3,5–8) |
| 10 | 10 (10–11) | 10 (9,93–11) | 0 (0–0,5) | 0 (0–1) | 7 (5–8,5) |
| 11 | 10 (8,5–11,5) | 10,1 (9,09–11,3) | 1 (0–1) | 1 (0–2,5) | 6,5 (4,5–8) |
| 12 | 9 (5,5–10,5) | 9,67 (6,54–11) | 1 (0–2) | 2 (0,5–4) | 5 (3–8) |
| 13 | 9 (5,5–10,5) | 8,61 (5,8–10) | 1 (0–3) | 2 (0,5–3) | 5,5 (2,5–7) |
| 14 | 7 (4–9,5) | 8,13 (4,72–9,59) | 1 (0–2,5) | 2 (1–5) | 4 (2–5) |
| 15 | 7 (4–10) | 6,87 (4,39–9,82) | 2 (0,5–3) | 2 (0,5–4) | 4 (2–6,5) |
| 16 | 6 (5–9) | 6,64 (4,37–9,02) | 1 (0–2,5) | 1,5 (0–3) | 3 (1–4) |
| 17 | 6 (4,5–8,5) | 6,34 (4,75–7,98) | 2 (0–3) | 2 (0,5–4) | 3 (2–5,5) |
| 18 | 6 (5–8) | 6,02 (5,29–7,92) | 2 (1,5–3,5) | 2 (1–2,5) | 2,5 (1–4,5) |
| 19 | 6 (5–9,5) | 6,61 (5,24–9,28) | 2 (1–3,5) | 2 (1–3) | 3 (1–4) |
| 20 | 7,5 (6–9) | 7,09 (6,34–9,13) | 2 (1–3,5) | 1 (0–2,5) | 2,5 (1–4) |
| 21 | 8 (6–9,5) | 7,15 (6,55–9,41) | 2 (0,5–4) | 2 (1–3) | 3 (2–5) |
| 22 | 7 (6–9) | 7,62 (6,61–8,81) | 2 (0,5–3) | 2 (1–4) | 3,5 (3–6,5) |
| 23 | 7 (6–9) | 7,35 (5,87–8,72) | 2 (0,5–3,5) | 1,5 (1–3) | 3 (2–4) |
| 24 | 7,5 (6,5–9) | 7,3 (6,68–9,21) | 2 (1–3) | 2 (0,5–3) | 3 (2–5) |
| 25 | 7 (6–9,5) | 7,48 (6,28–9) | 2 (0,5–3) | 2 (0,5–3) | 4 (2,5–4,5) |
| 26 | 8 (6–9,5) | 7,6 (6,49–9,38) | 1 (1–3) | 1 (1–3) | 3 (2–5) |
| 27 | 8 (6,5–9,5) | 7,53 (6,23–9,74) | 2 (0,5–3) | 2 (0,5–3) | 3 (1–5) |
| 28 | 8 (7–9) | 7,72 (6,66–9,4) | 2 (1–3) | 2 (1–3) | 3 (2–5) |
| 29 | 7,5 (5–8,5) | 7,54 (6,23–8,48) | 2 (0–2,5) | 2 (1–3) | 3,5 (2–4,5) |
| 30 | 8 (6–9,5) | 7,63 (5,87–8,85) | 2 (1,5–3,5) | 2 (1–3) | 3 (1–6,5) |
| 31 | 8 (6,5–9) | 7,84 (6,74–9,8) | 2 (1–3,5) | 1,5 (0,5–3,5) | 2 (1–3,5) |
| 32 | 8 (6,5–9,5) | 8,33 (6,69–9,71) | 2 (1–4) | 2 (0,5–3) | 4 (1–6) |
| 33 | 8 (6–10) | 8,22 (6,59–9,14) | 2 (1–3) | 2 (1–3,5) | 4 (2–5) |
| 34 | 8 (6–10) | 7,78 (6,29–9,85) | 2,5 (1–5) | 3 (1,5–3) | 4 (1,5–4,5) |
| 35 | 8 (6,5–10) | 8,06 (6,32–9,9) | 2 (2–3) | 2 (1–4) | 3,5 (1–5,5) |
| 36 | 9 (7–10,5) | 8,52 (6,39–10,1) | 2,5 (1,5–4) | 2 (1–4) | 3 (1–5) |
| 37 | 8,5 (6,5–10) | 8,87 (7,32–9,99) | 2 (1–4) | 2 (1–4,5) | 3 (1–5,5) |
| 38 | 8,5 (7–10) | 8,65 (6,75–10,4) | 2 (1–4) | 2 (1–3) | 4 (2–5,5) |
| 39 | 8,5 (6–10,5) | 8,24 (6,9–9,88) | 2 (1–4) | 2 (1–4) | 4 (1,5–5,5) |
| 40 | 8 (5,5–9,5) | 8,55 (5,87–9,71) | 2 (0,5–3,5) | 2,5 (1–3,5) | 3 (1,5–4,5) |
| 41 | 8 (5–10) | 8,63 (5,73–9,39) | 2 (1–4) | 2 (1–4) | 3 (1–5) |
| 42 | 9 (6,5–10) | 8,6 (6,32–10) | 3 (1–3) | 2 (0,5–3,5) | 3 (1–5,5) |
| 43 | 9 (6,5–11) | 8,92 (6,79–10) | 2 (1–5) | 2 (1–3,5) | 3,5 (0,5–5) |
| 44 | 8,5 (7–11) | 8,89 (6,86–10,8) | 2 (1–3,5) | 2 (1–4,5) | 2 (1–5,5) |
| 45 | 9 (7–10,5) | 8,52 (7,3–10,6) | 3 (2–5) | 3 (2–4) | 2,5 (1–5) |
| 46 | 9,5 (7–11) | 9,35 (7,08–10,6) | 3 (1,5–5) | 3 (1–4) | 3 (1–4,5) |
| 47 | 10 (8–11) | 9,7 (7,33–11,3) | 2,5 (1–3,5) | 3 (1–4) | 3 (1,5–5,5) |
| 48 | 9 (8–12) | 9,86 (7,93–11,9) | 2 (1–3,5) | 2 (1–4) | 3,5 (2–6,5) |
| 49 | 9,5 (7–11) | 9,78 (7,39–12) | 2 (1–4,5) | 3 (2–4,5) | 3 (1–5) |
| 50 | 9,5 (8–12) | 9,58 (7,74–11,4) | 2 (1–4,5) | 2,5 (0,5–4,5) | 3,5 (2–6) |
| 51 | 9 (7,5–10,5) | 9 (7,7–11,2) | 2 (1–3,5) | 2 (1–4) | 3 (1–4,5) |
| 52 | 9,5 (8–10,5) | 9,26 (8,1–10,7) | 2 (1–5) | 2,5 (1–4) | 4 (1–8,5) |
| 53 | 9,5 (8–11) | 9,34 (8,35–10,5) | 2 (1,5–6) | 3 (1–4,5) | 3,5 (1,5–5,5) |
| 54 | 10 (7–11) | 9,29 (7,75–11) | 2,5 (1–4) | 2,5 (1–4) | 3 (1–6) |
| 55 | 9,5 (7–12) | 9,41 (7,04–11,4) | 2 (1,5–3,5) | 2,5 (1–4) | 3 (2–6) |
| 56 | 9,5 (6,5–11) | 9,04 (7,02–11,5) | 2 (1–4) | 3 (1,5–4) | 3 (1–5) |
| 57 | 9,5 (6,5–12) | 9,87 (6,83–12) | 2 (1–4) | 2 (1–4,5) | 4 (0,5–5) |
| 58 | 9,5 (7,5–12) | 9,51 (7,2–11,9) | 2,5 (1–4) | 2 (1–4,5) | 2,5 (2–4,5) |
| 59 | 9,5 (7–11,5) | 9,6 (7,33–12) | 2 (1–3) | 3 (1–4) | 4 (1,5–8,5) |
| 60 | 8,5 (6,5–12,5) | 9,23 (6,42–12) | 3 (2–4,5) | 3 (1,5–5,5) | 3,5 (1–5) |

### Kahramanlar (1/2)

| Yıl | Doğan kahraman | Ölen kahraman | Emekli olan kahraman | Diyarı terk eden kahraman | Ölümden dönen kahraman | Efsane olan kahraman |
|---|---|---|---|---|---|---|
| 1 | 0 (0–0,5) | 0 | 0 | 0 | 0 | 0 |
| 2 | 1 (0–2) | 0 | 0 | 0 | 0 | 0 |
| 3 | 1 (0–2) | 0 | 0 | 0 | 0 | 0 |
| 4 | 0 (0–2) | 0 (0–1) | 0 | 0 (0–0,5) | 0 | 0 |
| 5 | 1 (0–2,5) | 0 (0–0,5) | 0 | 0 (0–0,5) | 0 | 0 |
| 6 | 1 (0–2) | 0 (0–0,5) | 0 | 0 (0–1) | 0 | 0 |
| 7 | 1 (0–3) | 0 (0–1,5) | 0 | 0 (0–0,5) | 0 | 0 |
| 8 | 1,5 (0–4) | 0 (0–1,5) | 0 | 0 | 0 | 0 |
| 9 | 2,5 (0,5–4,5) | 0 (0–1,5) | 0 | 0 | 0 | 0 |
| 10 | 2,5 (2–5) | 0 (0–1) | 0 | 0 (0–0,5) | 0 | 0 |
| 11 | 4 (0,5–6,5) | 0 (0–2) | 0 | 0 | 0 | 0 |
| 12 | 3 (1–4) | 1 (0–2,5) | 0 | 0 | 0 | 0 |
| 13 | 5 (2–6) | 0 (0–2) | 0 | 0 | 0 | 0 |
| 14 | 2 (1–5,5) | 1 (0–2) | 0 | 0 | 0 | 0 |
| 15 | 4 (1–6) | 0 (0–2,5) | 0 | 0 | 0 | 0 |
| 16 | 2,5 (1–4) | 0 (0–2) | 0 | 0 | 0 | 0 |
| 17 | 2 (1–5) | 0,5 (0–2) | 0 | 0 | 0 | 0 |
| 18 | 2,5 (1–5) | 1 (0–3) | 0 | 0 | 0 | 0 |
| 19 | 3 (1–6) | 0 (0–1,5) | 0 | 0 | 0 | 0 |
| 20 | 3 (1–4) | 0 (0–3) | 0 | 0 | 0 | 0 (0–0,5) |
| 21 | 3 (1–4,5) | 0 (0–2,5) | 0 | 0 | 0 | 0 (0–0,5) |
| 22 | 4 (2,5–6) | 0 (0–3,5) | 0 | 0 | 0 | 0 |
| 23 | 4 (2,5–7) | 1 (0–3) | 0 | 0 | 0 | 0 (0–0,5) |
| 24 | 3 (0–6) | 1 (0–2) | 0 | 0 | 0 | 0 (0–1) |
| 25 | 3 (1–5,5) | 0 (0–7) | 0 | 0 | 0 | 0 (0–0,5) |
| 26 | 3 (1–4,5) | 0 (0–5) | 0 | 0 (0–0,5) | 0 | 0 (0–1) |
| 27 | 3,5 (1,5–5) | 1 (0–4) | 0 | 0 (0–0,5) | 0 | 0 (0–0,5) |
| 28 | 3 (1–6) | 1 (0–5,5) | 0 | 0 (0–1) | 0 | 0 (0–1) |
| 29 | 2,5 (1–4,5) | 0 (0–3,5) | 0 | 0 | 0 | 0 (0–1) |
| 30 | 3 (2–5) | 0,5 (0–2) | 0 | 0 | 0 | 0 (0–2) |
| 31 | 3 (1–6) | 0,5 (0–9,5) | 0 | 0 (0–0,5) | 0 | 0 (0–2) |
| 32 | 3,5 (2–5) | 2 (0–4,5) | 0 | 0 (0–0,5) | 0 | 0 (0–0,5) |
| 33 | 3 (1–4) | 0 (0–3,5) | 0 | 0 (0–0,5) | 0 | 0 (0–1) |
| 34 | 3 (1–4) | 0 (0–2,5) | 0 (0–0,5) | 0 (0–1) | 0 | 0 (0–1) |
| 35 | 2 (1–3,5) | 1 (0–4) | 0 (0–0,5) | 0 (0–1) | 0 | 0 (0–1) |
| 36 | 2,5 (1–5,5) | 0 (0–3,5) | 0 (0–1) | 0 (0–0,5) | 0 | 0 (0–1) |
| 37 | 3 (1–5) | 1 (0–4,5) | 0 (0–1) | 0 (0–1,5) | 0 | 0 (0–1,5) |
| 38 | 2 (0,5–5) | 0,5 (0–5) | 0 (0–1,5) | 0 | 0 | 0 (0–2) |
| 39 | 3 (1,5–5,5) | 1 (0–5) | 0 (0–1) | 0 (0–1) | 0 | 0 (0–1) |
| 40 | 2 (1–4,5) | 1 (0–4) | 0 (0–2) | 0 (0–0,5) | 0 | 0 (0–1) |
| 41 | 3 (0,5–5,5) | 1 (0–4) | 0 (0–1,5) | 0 (0–1) | 0 | 0 (0–1) |
| 42 | 2 (0,5–4) | 0,5 (0–4) | 1 (0–2) | 0 (0–0,5) | 0 | 0 (0–0,5) |
| 43 | 3 (0,5–6) | 1 (0–2) | 0 (0–2) | 0 (0–0,5) | 0 | 0,5 (0–1) |
| 44 | 2 (0–4,5) | 0,5 (0–3) | 0 (0–1) | 0 (0–1) | 0 | 0 (0–1) |
| 45 | 3,5 (2–5) | 1,5 (0–5) | 0,5 (0–2) | 0,5 (0–1,5) | 0 | 0 (0–1,5) |
| 46 | 3 (2–4) | 1 (0–3) | 1 (0–1) | 0 (0–1) | 0 | 0 (0–1,5) |
| 47 | 3 (0,5–4) | 1,5 (0–4,5) | 1 (0–2) | 0 (0–0,5) | 0 | 0 (0–1,5) |
| 48 | 2,5 (0,5–4) | 1 (0–5) | 0,5 (0–1,5) | 0 (0–1,5) | 0 | 0 (0–3) |
| 49 | 2,5 (1–5) | 1 (0–3) | 0,5 (0–1) | 0 (0–1) | 0 | 0 (0–2) |
| 50 | 2 (1–4) | 1 (0–6,5) | 0,5 (0–1) | 0 (0–1) | 0 | 1 (0–2) |
| 51 | 3 (0,5–4,5) | 1 (0–3) | 0,5 (0–2) | 0 (0–1) | 0 | 0 (0–1,5) |
| 52 | 4 (0,5–6,5) | 1 (0–5) | 1 (0–2) | 0 (0–1) | 0 | 0,5 (0–1) |
| 53 | 2 (0–3) | 1 (0–2) | 1 (0–2,5) | 0 (0–1,5) | 0 | 0 (0–2) |
| 54 | 3 (0,5–5,5) | 1 (0–4) | 1 (0–3) | 0 (0–1) | 0 | 1 (0–2) |
| 55 | 2,5 (1–4) | 1 (0–3,5) | 1 (0–2,5) | 0 | 0 | 0 (0–1) |
| 56 | 3 (0–6,5) | 1 (0–6) | 1 (0–1) | 0 (0–1,5) | 0 | 0 (0–1) |
| 57 | 2,5 (0,5–5,5) | 0 (0–3) | 2 (0,5–3) | 0 (0–1) | 0 | 0 (0–1) |
| 58 | 2 (1–4) | 1,5 (0–4) | 1 (0–1) | 0 (0–1) | 0 | 0 (0–1,5) |
| 59 | 3 (1–5,5) | 0 (0–2) | 0,5 (0–2) | 1 (0–1) | 0 | 0 (0–1) |
| 60 | 3 (1–4,5) | 0,5 (0–6) | 0 (0–1) | 0 (0–1) | 0 | 0 (0–1) |

### Kahramanlar (2/2)

| Yıl | Yaşayan kahraman (yıl sonu) | Doğuş seviyesi (ort.) | Ölüm seviyesi (ort.) | Yaşayan kahraman seviyesi (ort.) | En yüksek seviye (şimdiye dek) |
|---|---|---|---|---|---|
| 1 | 0 (0–0,5) | 2 | – | 2 | 0 (0–1) |
| 2 | 1 (0–2) | 1 (1–2) | – | 1,25 (1–2) | 1 (0–2) |
| 3 | 2 (1–3) | 1 (1–2) | 2 | 1,33 (1–2) | 2 (1–2) |
| 4 | 2 (0–4,5) | 1 (1–1,7) | 1 (1–1,27) | 1,25 (1–1,95) | 2 (1–2) |
| 5 | 3 (1–5,5) | 1,17 (1–2) | 1 | 1,4 (1–2) | 2 (1–2) |
| 6 | 3,5 (1,5–6) | 1,5 (1–2) | 1,33 (1,07–1,6) | 1,46 (1–2) | 2 (1–2) |
| 7 | 4 (2–7,5) | 1 (1–1,8) | 1,25 (1–2) | 1,5 (1–2) | 2 (1–2,5) |
| 8 | 6 (3,5–9) | 1,33 (1–1,93) | 1 (1–2) | 1,51 (1,17–1,8) | 2 (2–3) |
| 9 | 7,5 (4–13,5) | 1,5 (1–2) | 1,5 (1,1–1,63) | 1,5 (1,31–1,62) | 2 (2–3) |
| 10 | 9,5 (7–15) | 1,25 (1–1,8) | 1,5 (1–2) | 1,5 (1,22–1,63) | 2 (2–3) |
| 11 | 14 (10–18) | 1,5 (1,22–1,95) | 1,33 (1–1,9) | 1,56 (1,32–1,76) | 3 (2–3) |
| 12 | 16,5 (10,5–21) | 1,33 (1–1,6) | 1 (1–2) | 1,72 (1,35–1,89) | 3 (2–4) |
| 13 | 18,5 (13,5–24,5) | 1,2 (1–1,56) | 1,33 (1–2) | 1,76 (1,46–1,9) | 3 (3–4) |
| 14 | 20,5 (14–27) | 1,33 (1–2) | 1,25 (1–2) | 1,95 (1,54–2,19) | 4 (3–5) |
| 15 | 23 (17,5–29,5) | 1,25 (1–1,66) | 1 (1–1,4) | 2,03 (1,65–2,25) | 4 (3–5) |
| 16 | 25,5 (19–31,5) | 1 (1–1,9) | 1,5 (1–2,5) | 2,1 (1,83–2,35) | 4 (3–5) |
| 17 | 26 (21,5–33) | 1,06 (1–1,75) | 1 (1–1,65) | 2,14 (1,96–2,51) | 4 (4–5) |
| 18 | 28,5 (22–36) | 1,37 (1–1,58) | 1,5 (1–2,93) | 2,29 (2,14–2,51) | 4 (4–5) |
| 19 | 30,5 (23,5–39) | 1,33 (1–2) | 2 (1–2,7) | 2,32 (2,17–2,56) | 4,5 (4–5,5) |
| 20 | 33 (25–41,5) | 1,29 (1–2) | 2,17 (1,33–2,83) | 2,33 (2,15–2,7) | 5 (4–6) |
| 21 | 35,5 (25,5–45) | 1,25 (1–1,83) | 2 (1–3) | 2,44 (2,23–2,78) | 5 (4–6) |
| 22 | 37 (29,5–47) | 1,29 (1–1,5) | 1,45 (1–2) | 2,51 (2,3–2,73) | 5 (4,5–6) |
| 23 | 40,5 (33,5–50) | 1,25 (1–1,58) | 2 (1–2,99) | 2,54 (2,34–2,71) | 5 (4,5–6,5) |
| 24 | 43 (33–51,5) | 1,43 (1,04–1,67) | 2 (1–3,93) | 2,56 (2,37–2,75) | 6 (5–6,5) |
| 25 | 43 (35–52) | 1,25 (1–1,46) | 1,93 (1,36–4,35) | 2,66 (2,48–2,89) | 6 (5–7) |
| 26 | 42,5 (37–52) | 1,33 (1–1,58) | 2,89 (1,75–3) | 2,68 (2,41–2,95) | 6 (5–7) |
| 27 | 44,5 (37,5–54) | 1,4 (1–1,72) | 1,5 (1–3,2) | 2,74 (2,39–2,95) | 6 (5–7) |
| 28 | 46,5 (38,5–56) | 1,37 (1–2) | 2,67 (1,5–3,5) | 2,8 (2,45–3,03) | 6 (5–7,5) |
| 29 | 46 (39,5–57,5) | 1,27 (1–2) | 2,88 (1,3–3,57) | 2,91 (2,53–3,23) | 6,5 (5,5–8) |
| 30 | 49,5 (40–60) | 1,13 (1–1,55) | 2,25 (1,85–4,9) | 2,94 (2,54–3,21) | 7 (5,5–8) |
| 31 | 49,5 (39,5–61,5) | 1,42 (1–2) | 2 (1–2,09) | 3,04 (2,6–3,24) | 7 (6–8,5) |
| 32 | 50,5 (41–60) | 1,33 (1–1,5) | 2 (1,3–3,8) | 3,03 (2,65–3,21) | 7 (6–8,5) |
| 33 | 52 (42,5–61) | 1,5 (1–2) | 2 (1,56–2,66) | 3,12 (2,75–3,41) | 7 (6–8,5) |
| 34 | 55 (45–62) | 1,29 (1–1,8) | 2 (1–3,5) | 3,28 (2,75–3,49) | 7 (6–8,5) |
| 35 | 56 (45–58,5) | 1,25 (1–2) | 2,5 (1,68–4) | 3,37 (2,86–3,57) | 7,5 (6–9) |
| 36 | 55 (49–62) | 1,42 (1–1,83) | 3 (2,33–3,5) | 3,39 (2,88–3,59) | 7,5 (6–9,5) |
| 37 | 57 (49–61,5) | 1,47 (1–2) | 2,27 (1,9–3,1) | 3,39 (3,01–3,6) | 7,5 (6–9,5) |
| 38 | 57,5 (51–63,5) | 1,45 (1–1,67) | 2 (1,18–3,8) | 3,47 (3,07–3,7) | 7,5 (6,5–9,5) |
| 39 | 57,5 (49,5–63,5) | 1,5 (1–1,67) | 3 (1,73–4) | 3,47 (3,13–3,7) | 8 (7–10) |
| 40 | 56,5 (50–64,5) | 1,25 (1–2) | 3 (2–3,97) | 3,43 (3,07–3,84) | 8,5 (7–10) |
| 41 | 56,5 (50,5–68) | 1,42 (1–1,9) | 2,67 (1,9–3,1) | 3,48 (3,01–3,91) | 8,5 (7–10) |
| 42 | 56,5 (50–68,5) | 1,29 (1–1,85) | 2 (1,35–2,86) | 3,53 (3,11–4,05) | 8,5 (7–10) |
| 43 | 57,5 (52–68) | 1,33 (1–1,67) | 4 (3–5,04) | 3,62 (3,06–4,09) | 9 (7–10) |
| 44 | 58,5 (52,5–69,5) | 1,45 (1–2) | 2,17 (1,35–3,65) | 3,61 (3,18–4,09) | 9 (7–10) |
| 45 | 60 (52–68,5) | 1,4 (1–1,72) | 2,25 (1,08–2,98) | 3,64 (3,33–4,05) | 9 (7–10) |
| 46 | 61 (54–70) | 1,33 (1,1–1,5) | 2,42 (2–3,65) | 3,71 (3,37–4,09) | 9,5 (7,5–10) |
| 47 | 61 (52,5–70) | 1,25 (1–1,93) | 2,8 (2,27–4,1) | 3,65 (3,42–4,14) | 9,5 (8–10) |
| 48 | 57,5 (53,5–70) | 1,42 (1–1,73) | 3 (1,4–4,38) | 3,7 (3,43–4,1) | 10 (8–10) |
| 49 | 59 (53,5–71,5) | 1,5 (1–2) | 3 (2–5,18) | 3,7 (3,54–4,1) | 10 (8–10) |
| 50 | 58,5 (50,5–72) | 1,33 (1–1,9) | 3 (2,27–4,08) | 3,84 (3,49–4,16) | 10 (8–10) |
| 51 | 61 (49,5–72,5) | 1,37 (1,08–1,9) | 3 (1,67–5) | 3,78 (3,58–4,26) | 10 (9–10) |
| 52 | 63 (50,5–73) | 1,46 (1–1,75) | 2,8 (1,93–4,4) | 3,8 (3,42–4,41) | 10 (9–10) |
| 53 | 62 (51–72) | 1,33 (1–2,23) | 3 (1,8–5,4) | 3,9 (3,51–4,44) | 10 (9–10) |
| 54 | 63 (50–73) | 1,38 (1–2) | 3 (1,9–5,28) | 3,96 (3,39–4,53) | 10 (9–10) |
| 55 | 62 (48,5–71) | 1,33 (1–2) | 3 (2,43–8) | 3,93 (3,41–4,59) | 10 (9–10) |
| 56 | 61,5 (51–73) | 1,56 (1,26–2,13) | 4 (2,17–7,5) | 3,97 (3,53–4,66) | 10 (9–10) |
| 57 | 61,5 (50–71) | 1,4 (1,05–2) | 4 (2,4–5,15) | 3,81 (3,53–4,7) | 10 (9,5–10) |
| 58 | 59,5 (50,5–69) | 1,42 (1–1,92) | 4,25 (2,8–8,8) | 3,79 (3,54–4,68) | 10 (9,5–10) |
| 59 | 62,5 (51,5–71) | 1,6 (1–2) | 4,5 (3–5,7) | 3,82 (3,54–4,59) | 10 (9,5–10) |
| 60 | 62 (53,5–74,5) | 1,33 (1–1,65) | 4,33 (1,95–7,6) | 3,97 (3,57–4,57) | 10 (9,5–10) |

### Han ve ticaret

| Yıl | Ayakta han | Asılan ilan | Biten ilan | Ticaret seferi (kervan) | İkmal seferi |
|---|---|---|---|---|---|
| 1 | 2 (1–3) | 0 (0–1) | 0 | 0 | 0 |
| 2 | 4 | 0 (0–1) | 0 | 0 | 0 |
| 3 | 4 | 0 (0–1) | 0 | 0 | 0 |
| 4 | 4 (3–4) | 1 (0–2) | 0 | 0 (0–0,5) | 0 |
| 5 | 4 (3–4) | 2,5 (1–3,5) | 0 | 0 (0–2,5) | 0 |
| 6 | 3,5 (2,5–4) | 1 (1–2,5) | 0 | 2 (0–7) | 0 |
| 7 | 3 (2–4) | 2 (0–3,5) | 0 | 6 (2–15) | 0 |
| 8 | 2,5 (1,5–4) | 2 (1–4,5) | 0 | 10,5 (6–22) | 0 |
| 9 | 3 (1,5–4) | 2 (0,5–3,5) | 0 | 16 (10–29) | 0 |
| 10 | 3 (2–4) | 2 (1–4) | 0 | 20 (12,5–38) | 0 |
| 11 | 3 (1,5–4) | 2 (1–5,5) | 0 (0–1) | 25 (15,5–46,5) | 0 (0–1) |
| 12 | 3 (2–4) | 3 (1–7,5) | 1 (0–2,5) | 29,5 (18,5–51,5) | 2 (0–3) |
| 13 | 3 (2–4,5) | 2,5 (1–5,5) | 0,5 (0–3) | 35,5 (21–57,5) | 3 (0–5,5) |
| 14 | 3 (2–4,5) | 2,5 (1–5,5) | 1 (0–2) | 39,5 (25–64) | 5 (0–6) |
| 15 | 3,5 (3–5) | 2 (1–3,5) | 1 (0–2) | 42,5 (25–65,5) | 5 (1–11) |
| 16 | 4 (3–5) | 1,5 (0,5–2,5) | 0,5 (0–1) | 43 (26–71,5) | 8,5 (3–12,5) |
| 17 | 4 (4–5) | 2 (0,5–4) | 1 (0–1,5) | 46 (24,5–70,5) | 11 (5,5–13,5) |
| 18 | 4 (4–5) | 2 (1–6) | 1 (0–1) | 52 (26–77) | 12 (5–15,5) |
| 19 | 4 (3,5–5) | 2 (1–5) | 0 (0–1) | 52,5 (27–79,5) | 12,5 (6–21,5) |
| 20 | 5 (4–5,5) | 2,5 (0–4,5) | 0 (0–1) | 60,5 (28,5–79) | 14 (6,5–21,5) |
| 21 | 5 (4–5) | 2 (0–4,5) | 0,5 (0–1) | 61,5 (29–82,5) | 15 (10–21,5) |
| 22 | 5 (4–6) | 5 (1,5–7,5) | 0 (0–1,5) | 63 (29,5–89) | 15,5 (10–24,5) |
| 23 | 5 (4–6) | 3 (1,5–5) | 0,5 (0–2) | 69,5 (31–93) | 17,5 (11,5–23,5) |
| 24 | 5 (4–5,5) | 2 (1,5–5) | 0 (0–1,5) | 68,5 (34–98) | 20 (12–24,5) |
| 25 | 5 (5–6) | 2 (0–5,5) | 0,5 (0–2,5) | 75,5 (35–99) | 19,5 (11–24,5) |
| 26 | 5 (5–6) | 2 (1,5–7) | 0 (0–1) | 76 (37,5–102) | 21,5 (13–25,5) |
| 27 | 5 (5–6) | 2,5 (1–5,5) | 0,5 (0–2) | 77 (36–102) | 20,5 (13–25) |
| 28 | 5 (5–6) | 3 (1–4) | 0 (0–1,5) | 76,5 (35,5–98) | 20,5 (14–26,5) |
| 29 | 5 (5–6) | 2,5 (1–5,5) | 1 (0–1) | 80 (37–103) | 21,5 (14,5–28,5) |
| 30 | 5 (5–6) | 2,5 (1–5,5) | 1 (0–2,5) | 80 (35,5–110) | 22,5 (14,5–30) |
| 31 | 5 (5–5,5) | 2,5 (0,5–4) | 0 (0–2,5) | 79 (37,5–112) | 23,5 (14,5–31) |
| 32 | 5 | 2 (1–5) | 0 (0–2,5) | 77,5 (39,5–115) | 25 (12,5–33,5) |
| 33 | 5 | 3 (1–5) | 1 (0–3) | 81 (41–113) | 24 (13,5–33,5) |
| 34 | 5 (4,5–6) | 3 (0,5–5,5) | 1 (0–2,5) | 81 (39–117) | 24,5 (12,5–33,5) |
| 35 | 5 (4,5–6) | 2 (0,5–6,5) | 1 (0–3) | 80,5 (38–116) | 25 (14,5–33,5) |
| 36 | 5 (4,5–6) | 1,5 (0,5–3) | 0,5 (0–2,5) | 84,5 (38,5–118) | 25 (14,5–36,5) |
| 37 | 5 (4–6) | 2 (1–4,5) | 0,5 (0–1,5) | 85,5 (39,5–126) | 26 (15,5–36) |
| 38 | 5 (4–6) | 2,5 (1–4,5) | 1 (0–2) | 88,5 (35–128) | 27,5 (14–35,5) |
| 39 | 5 (5–6) | 2 (1–3,5) | 1 (0–2,5) | 85,5 (36,5–132) | 24 (14–38,5) |
| 40 | 5 (5–6) | 2 (1–4) | 1 (0–2) | 89 (38,5–131) | 25,5 (14,5–35) |
| 41 | 5 (5–6) | 3 (1–4) | 1 (0–2) | 86 (37–134) | 28,5 (14–37,5) |
| 42 | 5 (5–6) | 2 (0–4,5) | 1 (0–3,5) | 92 (35,5–136) | 29 (14–39,5) |
| 43 | 5 (5–6) | 2,5 (1–4,5) | 0 (0–2) | 90,5 (38–138) | 28,5 (15–37,5) |
| 44 | 5 (5–6) | 2 (0–5,5) | 1 (0–3,5) | 89,5 (38–126) | 27 (13–36) |
| 45 | 5 (5–6) | 3,5 (1–6) | 2 (0–2,5) | 92 (40–132) | 28 (12,5–37,5) |
| 46 | 5 (5–6) | 2 (0–4) | 1 (0–3) | 93 (38–136) | 31 (13,5–37,5) |
| 47 | 5 (5–6) | 2 (1–6) | 0,5 (0–2,5) | 95 (40–143) | 29 (13,5–37,5) |
| 48 | 5 (4,5–6) | 2 (1–4) | 1 (0–3) | 94 (37,5–140) | 30 (15–35,5) |
| 49 | 5 (5–6) | 2 (1–4) | 1 (0–2,5) | 97 (40–138) | 30 (16–37) |
| 50 | 5 (5–6) | 2 (0,5–4,5) | 1 (0–2) | 99,5 (39,5–156) | 28,5 (17–37) |
| 51 | 5 (5–6) | 1 (0–4,5) | 1 (0–2,5) | 99,5 (40,5–165) | 29 (16,5–39) |
| 52 | 5 (5–6) | 3 (1–6,5) | 1 (0–3) | 102 (42,5–161) | 31 (17–42) |
| 53 | 5 (5–6) | 2 (0,5–6,5) | 1,5 (0,5–3) | 101 (44–158) | 31,5 (16–45) |
| 54 | 5,5 (5–6) | 2,5 (1–5) | 1 (0–3,5) | 100 (44,5–170) | 29 (17–45) |
| 55 | 5,5 (5–6) | 2,5 (0,5–4) | 1 (0–3) | 104 (48–166) | 31 (17–44) |
| 56 | 5,5 (5–6) | 2 (1–4,5) | 1 (0–2,5) | 98,5 (46–180) | 30 (16,5–47) |
| 57 | 6 (5–6) | 2,5 (0–7) | 1,5 (1–2,5) | 102 (45–181) | 33,5 (16–46,5) |
| 58 | 5,5 (5–6) | 2 (0,5–5,5) | 1 (0–4) | 106 (50,5–174) | 31,5 (17,5–47) |
| 59 | 5,5 (5–6,5) | 3 (1–6) | 1 (0–3) | 112 (51,5–173) | 31 (16,5–48) |
| 60 | 5 (5–6) | 2,5 (1–6) | 1,5 (0,5–3,5) | 109 (53,5–174) | 31 (16,5–49) |

