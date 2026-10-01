# Ölçüm raporu: f1b-2

16 dünya (seed 1-16) × 60 yıl (7200 gün) · 2026-10-01 16:41 · `FD.Macro.Run stats --seeds 1-16 --years 60 --jobs 2 --verify 1 --saveload 1`

Süre: 6 dk 29 sn duvar saati, 2 iş parçacığı; dünya başına 44,5 sn (en az 34,7, en çok 54,3; yıl sonu hash'leri dâhil).

## Bitiş ölçütleri

DESIGN-FAZ1.md, "Bitiş ölçütleri". ✓ geçti · ✗ kaldı · — ölçülemedi.

| # | Ölçüt | Koşul | Ölçülen | Sonuç |
|---|---|---|---|---|
| 1 | Donma yok | 41–60. yılların yıllık büyük olay medyanı ≥ 0,8 × 6–20. yılların medyanı | 53 / 59 = 0,9 kat | ✓ |
| 2 | Çöküş | dünyaların ≥ %75'inde 60 yılda ≥ 1 çöküş (yok olma ya da başkent kaybı) | %88 (14/16 dünya); toplam 98 çöküş: 12 yok olma, 86 başkent kaybı | ✓ |
| 3 | Kamplar | 41–60. yıllarda yaşayan kamp medyanı ≥ 6–20. yılların medyanı | 9,31 ≥ 8,67 (yıl sonu sayımıyla 9 / 9) | ✓ |
| 4a | Kahraman: doğuş seviyesi | her on yılda doğanların ortalama seviyesi ≤ 2 | 1,34 · 1,34 · 1,35 · 1,32 · 1,46 · 1,5 (on yıllar sırasıyla) | ✓ |
| 4b | Kahraman: Sv8+ | dünyaların ≥ yarısında en az bir kahraman Sv8 ve üstüne çıkar | %88 (14/16 dünya); dünyadaki en yüksek seviye: medyan Sv9, en çok Sv10 | ✓ |
| 4c | Kahraman: efsane | dünya başına efsane medyanı 1–6 | medyan 4 (p10–p90: 2–8,5; toplam 71) | ✓ |
| 4d | Kahraman: ölüm payı | doğan kahramanların %30–80'i ölür | %40 (1136/2864); dünya medyanı %38 (%28–%50) | ✓ |
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
| Araştırma ağacı erken bitiyor | ~19. yılda bitiyor; 30. yılda medeniyetlerin %98'i bitirmiş | başlangıç medeniyetlerinde ağaç bitişi (son çağ, araştıracak düğüm yok) medyanı 20. yıl (123/128 bitirdi); 30. yılda bitirmiş medeniyet payı %91, araştırması duran (her çağ) %97; ağacın biten payı 19. yılda %84, 30. yılda %95 | evet |
| Medeniyetler 5 yerleşimde takılıyor | 98 medeniyetin 74'ü (%76) tam 5 yerleşimde | tam 5 kara yerleşimli medeniyet payı 30. yılda %1,6, 60. yılda %2,3 (denizaşırı koloniler dâhil 30. yılda tam 5: %0,8, 5+: %96); medeniyet başına 8,59 yerleşim (30. yıl) | hayır |
| Kamp sayısı düşüyor | 6,8'den 3,5'e iniyor | 3 (1. yıl) → en yüksek 10,5 (58. yıl) → 9 (30. yıl) → 10 (60. yıl); yıl sonu, yıllık dünya medyanı | hayır |
| Altın birikiyor | altın medyanı 78'den 6.503'e çıkıyor | 8,42 (1. yıl) → 480 (30. yıl) → 781 (60. yıl) | evet |
| İş gücü boşta | iş gücünün %43'ü boşta | 30. yılda %20, 60. yılda %18 (işe yerleşemeyen `zanaatçı` / bütün iş gücü, askerler dâhil; yıl içi ortalama) | hayır |
| Büyük olaylar seyreliyor | yıllık büyük olay 59'dan 28'e düşüyor | en yüksek 78,5 (13. yıl) → 49,5 (30. yıl) → 53 (60. yıl), yıllık dünya medyanı | hayır |
| Doğuş seviyesi şişiyor | 24. yıldan sonra herkes Sv5 doğuyor; efsane mekaniği ölü | 25–60. yıllarda Sv5+ doğanların payı %0; on yıllık doğuş seviyesi ortalaması 1,34 · 1,34 · 1,35 · 1,32 · 1,46 · 1,5 | hayır |
| Başkent düşmüyor | başkent fethedilemiyor (agents.ts:673) | 16 dünyada 86 başkent kaybı, 12 yok olma; 787 yerleşim fethi | hayır |

## On yıllık özet

Hücre: dünyalar arası medyan (p10–p90). Her dünyada on yılın yıllık değerlerinin ortalaması alınır: akış ölçülerinde yıllık ortalama, stok ölçülerinde yıl sonu değerlerinin ortalaması. Yüzdeler 0–1 paylardır.

| Ölçü | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| **Medeniyet** | | | | | | |
| Yaşayan medeniyet | 8 (7–9) | 8 (7–9) | 8 (7–9) | 8 (7–9) | 8 (7–9,55) | 8 (7–9,15) |
| Yeni medeniyet (yeniden doğan) | 0 | 0 | 0 (0–0,1) | 0 | 0 (0–0,1) | 0 (0–0,1) |
| Yok olan medeniyet | 0 | 0 (0–0,05) | 0 (0–0,05) | 0 (0–0,05) | 0 (0–0,1) | 0 |
| Başkent kaybı (medeniyet yaşarken) | 0 | 0,05 (0–0,15) | 0 (0–0,2) | 0,1 (0–0,3) | 0,05 (0–0,3) | 0,1 (0–0,3) |
| Çöküş (yok olma + başkent kaybı) | 0 (0–0,05) | 0,05 (0–0,25) | 0,05 (0–0,2) | 0,1 (0–0,35) | 0,05 (0–0,35) | 0,1 (0–0,3) |
| Yaşayan yerleşim | 15,1 (12,8–17,5) | 48,6 (39,8–53,6) | 65,2 (52,9–71,7) | 72,9 (61–80,6) | 79,6 (65–86,3) | 84,8 (68,1–92,4) |
| Medeniyet başına yerleşim | 1,94 (1,68–2,06) | 6,03 (5,52–6,28) | 8,13 (7,29–8,45) | 8,98 (8,34–9,76) | 9,85 (8,7–10,6) | 10,1 (9,27–11,4) |
| 5+ kara yerleşimli medeniyet payı | %5,3 (%1,4–%8,8) | %86 (%77–%91) | %99 (%87–%100) | %100 (%86–%100) | %98 (%87–%100) | %98 (%85–%100) |
| Kurulan yerleşim | 2,35 (1,95–2,8) | 2,6 (1,85–3,25) | 1,1 (0,85–1,4) | 0,75 (0,45–1,35) | 0,55 (0,4–1) | 0,5 (0,3–1,35) |
| Fethedilen yerleşim | 0 (0–0,15) | 0,7 (0,45–1,1) | 0,95 (0,3–1,8) | 1,15 (0,1–1,8) | 1,25 (0–2) | 1 (0–1,85) |
| Terk edilen yerleşim | 0 (0–0,05) | 0 (0–0,1) | 0 (0–0,15) | 0 (0–0,25) | 0,1 (0–0,45) | 0,2 (0–0,4) |
| Toplam nüfus | 272 (210–312) | 1390 (1119–1551) | 2510 (2020–2738) | 3266 (2641–3464) | 3634 (3006–3954) | 3810 (3304–4432) |
| Ortalama çağ | 1,9 (1,79–1,93) | 3,36 (3,14–3,57) | 3,9 (3,7–3,99) | 4 (3,92–4) | 4 | 4 |
| Araştırma ağacının biten payı (ort.) | %19 (%18–%20) | %71 (%65–%74) | %92 (%88–%95) | %96 (%94–%99) | %98 (%95–%100) | %99 (%97–%100) |
| Ağacı bitmiş medeniyet payı | %0 | %19 (%7,3–%26) | %84 (%60–%89) | %99 (%86–%100) | %100 (%97–%100) | %100 (%94–%100) |
| Araştırması duran medeniyet payı | %0 (%0–%2) | %45 (%37–%52) | %90 (%80–%97) | %99 (%92–%100) | %100 (%97–%100) | %100 (%94–%100) |
| Altın medyanı (medeniyetler) | 52,5 (40–66,2) | 209 (165–241) | 360 (296–476) | 589 (490–708) | 678 (524–842) | 769 (654–949) |
| Boştaki iş gücü payı | %0,3 (%0,1–%0,6) | %6,9 (%5,5–%9,3) | %20 (%16–%23) | %20 (%18–%24) | %20 (%16–%23) | %19 (%17–%20) |
| Bölünme (ayrılıp kurulan medeniyet) | 0 | 0 | 0 (0–0,1) | 0 | 0 (0–0,1) | 0 (0–0,1) |
| En büyük medeniyetin yerleşimi | 2,65 (2,4–2,85) | 7,9 (7,2–8,6) | 10,4 (9,7–12,8) | 12,9 (11–16) | 15,5 (12–18,5) | 17,5 (12,2–21,1) |
| **Olaylar** | | | | | | |
| Olay | 83,2 (65,3–96,1) | 191 (153–218) | 175 (129–200) | 168 (120–189) | 162 (128–232) | 164 (122–239) |
| Büyük olay | 30,2 (26,1–36,4) | 69,9 (52,9–78,3) | 54,8 (39,3–64,6) | 49,7 (33,8–62,7) | 52 (35,6–72,1) | 53 (39,8–74,3) |
| **Savaş** | | | | | | |
| Muharebe | 4,25 (3,25–4,75) | 8,3 (6,7–9,3) | 8,5 (6,15–10,4) | 9,05 (6,05–11,4) | 10 (7,7–12,3) | 9,9 (7,45–12,5) |
| Başlayan savaş | 0,1 (0–0,25) | 0,95 (0,45–1,3) | 1,15 (0,35–1,95) | 1,5 (0,1–2,5) | 1,55 (0–2,4) | 1,4 (0–2,6) |
| Süren savaş (yıl sonu) | 0 (0–0,1) | 0,25 (0,1–0,4) | 0,3 (0–0,6) | 0,35 (0–1) | 0,5 (0–0,75) | 0,65 (0–1) |
| Yıl içinde süren savaş | 0,1 (0–0,25) | 1,25 (0,6–1,5) | 1,45 (0,35–2,6) | 2 (0,1–3) | 2,15 (0–3) | 2,15 (0–3,3) |
| Yağma akını (medeniyet) | 0,1 (0–0,3) | 1 (0–1,85) | 1,55 (0–3,4) | 1,45 (0–3,5) | 1,8 (0–3,65) | 1,3 (0–4,5) |
| Tarihî hak savaşı | 0 | 0,1 (0–0,3) | 0,15 (0–0,35) | 0,1 (0–0,4) | 0,1 (0–0,55) | 0,15 (0–0,5) |
| Pakt gereği savaş | 0 | 0,05 (0–0,2) | 0,1 (0–0,45) | 0,15 (0–0,65) | 0,15 (0–0,55) | 0,1 (0–0,5) |
| Kutsal Sefer çağrısı | 0 | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) |
| İhanet (pakt çiğnendi) | 0 | 0 (0–0,05) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,15) | 0 (0–0,1) |
| Savunma paktı (yıl sonu) | 0 (0–0,05) | 0,3 (0–1,2) | 1,05 (0,15–1,95) | 1,2 (0,35–2) | 1,25 (0,35–2,1) | 1 (0,15–2) |
| **Canavarlar** | | | | | | |
| Yaşayan kamp (yıl sonu) | 7,5 (6,8–8,5) | 7,4 (5,85–8,9) | 7,65 (6,75–8,9) | 8,8 (7,7–9,95) | 9,1 (7,9–11,2) | 9,9 (7,05–12,4) |
| Yaşayan kamp (yıl ort.) | 7,2 (6,49–8,01) | 7,59 (6,28–9,18) | 7,47 (6,65–8,87) | 9,03 (7,78–9,97) | 9,08 (7,91–11,2) | 9,73 (7,3–12,2) |
| Doğan kamp | 0,8 (0,7–0,9) | 1,55 (1,05–1,95) | 2,05 (1,4–2,4) | 1,9 (1,15–2,7) | 2,4 (1,45–2,9) | 2,45 (1,9–3,35) |
| Temizlenen kamp | 0 (0–0,1) | 1,8 (1,4–2,45) | 1,8 (1,25–2,25) | 2 (0,9–2,7) | 2,45 (1,4–3,05) | 2,65 (1,7–3,25) |
| Canavar baskını | 3,8 (2,8–4,25) | 4 (2,95–4,8) | 3,25 (2,7–4,55) | 3,55 (2,55–4,85) | 3,8 (2,6–6,15) | 3,35 (2,85–6,2) |
| Yaşayan trol ini (yıl sonu) | 0 | 0,2 (0–0,8) | 0,85 (0,35–1,65) | 1,1 (0,2–1,95) | 1,3 (0,7–2,45) | 1,6 (1,15–2,75) |
| Yaşayan ejderha (yıl sonu) | 0 | 0,05 (0–0,3) | 1 (0,9–1) | 1 (0,6–1) | 1 (0–1) | 0,9 (0–1) |
| Ejderha akını | 0 | 0,05 (0–0,3) | 0,95 (0,55–1,2) | 0,8 (0,4–1,15) | 0,7 (0–1,05) | 0,3 (0–1,05) |
| Kriz (anlatıcı) | 0 (0–0,1) | 0,3 (0,2–0,4) | 0,35 (0,1–0,55) | 0,3 (0,05–0,5) | 0,4 (0,1–0,6) | 0,5 (0,15–0,6) |
| Rahatlama dönemi (anlatıcı) | 0,2 (0,1–0,3) | 0,1 (0–0,15) | 0,1 (0–0,2) | 0,1 (0–0,2) | 0,1 (0–0,25) | 0,05 (0–0,1) |
| **Kahramanlar** | | | | | | |
| Doğan kahraman | 1,55 (0,9–2,05) | 2,95 (2,1–3,75) | 3,4 (2,4–4,8) | 3,3 (2,35–4,5) | 3,15 (2–4,15) | 3,65 (2,3–4,3) |
| Ölen kahraman | 0,2 (0–0,35) | 0,85 (0,25–1,7) | 1 (0,25–1,9) | 1,2 (0,55–2,85) | 1,25 (0,85–2,8) | 1,75 (0,85–2,55) |
| Emekli olan kahraman | 0 | 0 | 0 | 0,3 (0–0,4) | 0,7 (0,45–0,95) | 0,9 (0,7–1,55) |
| Diyarı terk eden kahraman | 0,1 (0–0,1) | 0 (0–0,1) | 0,1 (0–0,6) | 0,5 (0,1–0,85) | 0,45 (0,15–0,85) | 0,6 (0,35–1) |
| Ölümden dönen kahraman | 0 | 0 | 0 | 0 | 0 | 0 |
| Efsane olan kahraman | 0 | 0 (0–0,05) | 0 (0–0,1) | 0,05 (0–0,25) | 0,1 (0–0,3) | 0,1 (0–0,25) |
| Yaşayan kahraman (yıl sonu) | 3,8 (2,4–6,4) | 23,5 (19,5–28,6) | 44,8 (34,9–51,9) | 63,4 (55–69,5) | 71,9 (56,7–80,8) | 75,9 (58,9–88,8) |
| Doğuş seviyesi (ort.) | 1,31 (1,13–1,52) | 1,35 (1,24–1,51) | 1,34 (1,2–1,47) | 1,31 (1,22–1,47) | 1,43 (1,36–1,54) | 1,48 (1,36–1,74) |
| Ölüm seviyesi (ort.) | 1,5 (1,23–1,93) | 1,5 (1,12–2,02) | 2 (1,35–2,65) | 2,55 (1,69–3,13) | 3 (2,43–3,58) | 3,51 (2,77–4,3) |
| Yaşayan kahraman seviyesi (ort.) | 1,44 (1,14–1,78) | 1,96 (1,75–2,19) | 2,63 (2,34–2,8) | 2,99 (2,66–3,29) | 3,33 (3–3,7) | 3,58 (3,16–3,92) |
| En yüksek seviye (şimdiye dek) | 1,75 (1,2–2) | 3,8 (3,15–4,2) | 5 (4,8–6,6) | 6,3 (5,6–8,05) | 7,5 (6,15–9,45) | 8,85 (6,95–10) |
| **Han ve ticaret** | | | | | | |
| Ayakta han | 3,15 (2,85–3,8) | 4 (3,5–4,65) | 5 (4,65–5,95) | 5,5 (5–5,9) | 5,55 (5–6) | 5,8 (4,9–6,35) |
| Asılan ilan | 1,45 (1–1,9) | 2,6 (2,05–3,15) | 3,3 (2,25–4,2) | 3,15 (2,3–4,95) | 3,3 (1,9–5,1) | 3,6 (2,05–5,3) |
| Biten ilan | 0 | 0,75 (0,55–1,2) | 0,6 (0,3–1,15) | 0,95 (0,6–1,55) | 1,25 (0,5–2,1) | 1,3 (0,9–2,6) |
| Ticaret seferi (kervan) | 6,15 (3,7–12,9) | 40,7 (26,3–67,7) | 60,5 (41,7–85,3) | 69 (49,9–101) | 76,4 (48,5–128) | 82,3 (51,4–140) |
| İkmal seferi | 0 | 7,65 (3,75–9,65) | 18,7 (8,2–22,8) | 23,9 (15,8–32,4) | 26,7 (16,2–36,5) | 27,2 (17,2–40,6) |
| **Altın ve ambar** | | | | | | |
| Altın p90 (medeniyetler) | 130 (109–178) | 687 (529–842) | 1123 (804–1298) | 1273 (1048–1436) | 1483 (1246–1863) | 1537 (1279–1813) |
| Bakım gideri (altın; asker, kahraman, L2–L3) | 38 (18,2–47,8) | 614 (490–804) | 1432 (1220–1671) | 2014 (1773–2212) | 2194 (1962–2504) | 2367 (2050–2844) |
| Kamu işlerine (imar) harcanan altın | 0,05 (0–4,83) | 764 (491–1004) | 1935 (1127–2228) | 2500 (1981–2881) | 3202 (2430–4402) | 3311 (2322–4660) |
| Ambarla beslenen amele tayını (gıda) | 4,03 (0,87–10,6) | 598 (415–864) | 2287 (1979–3399) | 2636 (1840–3412) | 2240 (1092–3402) | 1278 (612–2682) |
| Kamu işlerindeki (amele) iş gücü payı | %0,1 (%0,1–%0,3) | %7,8 (%6,2–%9,3) | %19 (%17–%22) | %22 (%20–%24) | %22 (%20–%24) | %22 (%19–%25) |
| İmar ortalaması (köy+, 0–100) | 0,01 (0–0,16) | 11,8 (8,91–14,9) | 25,1 (21–28,6) | 30 (24,6–35,5) | 31,8 (27–36,7) | 31,8 (28,2–38,5) |
| Canavar baskınında yitirilen altın | 11,4 (6,3–18,7) | 8,2 (3–24,2) | 6,7 (2–25,8) | 4,95 (0,55–9,95) | 6,05 (0,5–14) | 4,15 (0,55–11,2) |
| Ejderhaya giden altın (haraç + akın) | 0 | 5,95 (0–56,4) | 266 (147–458) | 556 (94,2–770) | 474 (0–709) | 403 (0–768) |
| Hazinesi boş medeniyet payı | %0 | %0 | %0 | %0 | %0 | %0 |
| Kent tüketiminde yokluk payı (köy+; ekmek, bira ya da alet) | %2,4 (%1,1–%6,4) | %4,3 (%1,2–%6,7) | %14 (%5,5–%22) | %34 (%20–%43) | %42 (%35–%53) | %48 (%38–%58) |
| Ekmek ya da bira yokluğu payı (köy+) | %1,5 (%0,7–%5,8) | %1,3 (%0,4–%3,2) | %0,8 (%0–%6,7) | %2,4 (%0,8–%6,8) | %4,8 (%1,5–%8,4) | %5,8 (%2,8–%11) |
| Kıtlık (büyük olay) | 0 | 0 | 0 | 0 | 0 (0–0,15) | 0 (0–0,1) |
| Açlıktan ölen | 0 | 0 | 0 | 0 | 0 | 0 |
| Kıtlık yardımı (sevkiyat) | 0 | 0 | 0 | 0 (0–0,3) | 0 (0–0,35) | 0 (0–0,45) |
| Kıtlıkta yüz çeviren | 0 | 0 | 0 | 0 (0–0,05) | 0 (0–0,05) | 0 (0–0,15) |
| Kıtlık akını | 0 | 0 | 0 | 0 (0–0,05) | 0 | 0 (0–0,05) |

## Kahraman seviyeleri

### Doğuş seviyesi (bütün dünyalar, on yıl içinde doğanlar)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 |
|---|---|---|---|---|---|
| 1–10 | 235 | 1,34 | %66 | %34 | %0 |
| 11–20 | 471 | 1,34 | %67 | %32 | %0,6 |
| 21–30 | 568 | 1,35 | %66 | %33 | %1,1 |
| 31–40 | 530 | 1,32 | %70 | %27 | %2,5 |
| 41–50 | 519 | 1,46 | %58 | %38 | %4 |
| 51–60 | 541 | 1,5 | %56 | %38 | %5,9 |

### Ölüm seviyesi (bütün dünyalar, on yıl içinde ölenler)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 | Sv4 | Sv5 | Sv6 | Sv7 | Sv8 | Sv9 | Sv10 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 1–10 | 32 | 1,53 | %47 | %53 | %0 | %0 | %0 | %0 | %0 | %0 | %0 | %0 |
| 11–20 | 143 | 1,62 | %56 | %29 | %13 | %2,8 | %0 | %0 | %0 | %0 | %0 | %0 |
| 21–30 | 167 | 2,04 | %36 | %33 | %23 | %7,8 | %0,6 | %0 | %0 | %0 | %0 | %0 |
| 31–40 | 249 | 2,67 | %20 | %30 | %25 | %15 | %6,8 | %2,4 | %0,4 | %0 | %0 | %0 |
| 41–50 | 256 | 2,99 | %13 | %25 | %33 | %17 | %7,4 | %3,5 | %1,2 | %0 | %0,4 | %0 |
| 51–60 | 289 | 3,38 | %8,3 | %24 | %29 | %17 | %11 | %6,6 | %2,1 | %0,3 | %0,7 | %0,7 |

### Yaşayan kahramanların seviyesi (bütün dünyalar, on yılın son yılının sonunda)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 | Sv4 | Sv5 | Sv6 | Sv7 | Sv8 | Sv9 | Sv10 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 10 | 192 | 1,41 | %62 | %34 | %3,1 | %0 | %0 | %0 | %0 | %0 | %0 | %0 |
| 20 | 516 | 2,34 | %24 | %36 | %25 | %11 | %2,9 | %0,4 | %0 | %0 | %0 | %0 |
| 30 | 885 | 2,74 | %21 | %28 | %25 | %15 | %8,1 | %2,9 | %0,6 | %0,3 | %0 | %0 |
| 40 | 1062 | 3,12 | %15 | %27 | %23 | %16 | %11 | %4,8 | %2,2 | %0,9 | %0,3 | %0,2 |
| 50 | 1135 | 3,41 | %11 | %26 | %24 | %14 | %13 | %5,6 | %3 | %1,8 | %0,9 | %0,6 |
| 60 | 1165 | 3,56 | %9,7 | %24 | %24 | %15 | %12 | %6,4 | %3,6 | %2,5 | %0,9 | %1,1 |

### Ölüm nedenleri (bütün dünyalar)

| Neden | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 | Toplam |
|---|---|---|---|---|---|---|---|
| Kamp saldırısı | 9 | 82 | 30 | 64 | 62 | 99 | 346 (%30) |
| Kuşatma | 1 | 15 | 49 | 89 | 50 | 75 | 279 (%25) |
| Ejderha | 0 | 8 | 22 | 38 | 67 | 13 | 148 (%13) |
| Trol | 0 | 18 | 37 | 18 | 30 | 19 | 122 (%11) |
| Suikast | 1 | 5 | 14 | 14 | 19 | 14 | 67 (%5,9) |
| Bilinmiyor | 0 | 0 | 0 | 2 | 7 | 39 | 48 (%4,2) |
| Han baskını (canavar) | 15 | 5 | 4 | 4 | 10 | 7 | 45 (%4) |
| Yağma akını | 1 | 3 | 5 | 14 | 2 | 4 | 29 (%2,6) |
| Kervan soygunu | 0 | 2 | 2 | 1 | 3 | 7 | 15 (%1,3) |
| Yol pususu | 0 | 1 | 2 | 3 | 3 | 5 | 14 (%1,2) |
| Han baskını (medeniyet) | 2 | 0 | 0 | 2 | 3 | 1 | 8 (%0,7) |
| Yerleşim baskını (canavar) | 3 | 3 | 2 | 0 | 0 | 0 | 8 (%0,7) |
| Düello | 0 | 1 | 0 | 0 | 0 | 6 | 7 (%0,6) |

Neden, ölümün kaydedildiği andaki son muharebenin türünden (başlık ve taraflar) ya da suikast olayından çıkarılır.

## Olay türleri

Dünya başına yıllık olay sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Kahraman (`hero`) | 3,3 (1,5–5,95) | 21,9 (14,5–30,1) | 34,5 (22,6–44,1) | 47,9 (38,4–63,7) | 62,2 (42,1–93,2) | 71,6 (50,1–107) |
| İnşaat (`build`) | 26,4 (18,3–32) | 85,4 (62,5–103) | 54,4 (41–68,6) | 30,9 (20,4–39,2) | 21 (18–32,4) | 16 (13–29,2) |
| Göç (`migration`) | 1,35 (0,55–2,25) | 8,4 (5,9–12,6) | 17,1 (10,5–26,5) | 19,8 (10,3–28,3) | 17,4 (11,8–29,9) | 16 (8,85–27,3) |
| Sefer/ilan (`quest`) | 1,5 (1,05–2,15) | 8,2 (6,4–10,5) | 8,6 (5,95–12,9) | 10,9 (7,8–19,6) | 18,7 (9,45–29,7) | 18,3 (10,8–33,7) |
| Savaş (`war`) | 0,6 (0–1,45) | 6,55 (2,1–9,25) | 9,15 (1,35–20,8) | 10,3 (0,3–21) | 13,6 (0–22,6) | 11,2 (0–27,5) |
| Araştırma (`research`) | 16,2 (13,8–18,5) | 16,1 (13,2–18) | 3,4 (1,8–5,1) | 0,55 (0–2,5) | 0,05 (0–0,65) | 0 (0–0,6) |
| Kamp (`lair`) | 2 (1,85–2,25) | 4,15 (2,9–5,55) | 4,75 (3,45–6,6) | 5,55 (3,3–7,2) | 6,5 (3,9–8) | 7,45 (4,85–9,05) |
| Sınıf (`class`) | 4,6 (3,45–6) | 6,1 (4,9–7,45) | 5,3 (3,8–6,7) | 4,2 (2,7–5,55) | 4,1 (2,85–4,9) | 3,9 (2,75–5,15) |
| Ekonomi (`economy`) | 0,95 (0,65–1,3) | 2,9 (2,4–3,3) | 4,9 (3,1–6,3) | 5,15 (3,05–6,45) | 5 (2,45–6,35) | 4,3 (2,85–5,9) |
| Deniz (`sea`) | 0,55 (0,05–1,3) | 4,95 (3,55–7,6) | 4,9 (2,8–8,7) | 4,95 (2,05–7,4) | 3,3 (1,25–6,9) | 3,5 (1–7,55) |
| Baskın (`raid`) | 3,8 (2,8–4,3) | 4 (2,85–4,85) | 2,35 (1,9–3,6) | 2,7 (2,3–4,1) | 3,4 (2–5,6) | 3,3 (2,7–5,2) |
| Han (`inn`) | 2,75 (2,3–3,3) | 3,45 (2,5–3,85) | 3,05 (2–4,55) | 2,25 (1,7–3,5) | 2,65 (1,6–4,15) | 2,85 (2,05–4,05) |
| Yerleşim (`settle`) | 4,65 (4,2–5,6) | 4,8 (3,5–6,05) | 2,1 (1,55–2,8) | 1,45 (0,9–2,5) | 1 (0,8–2,05) | 1,05 (0,5–2,6) |
| Keşif (`discover`) | 6,2 (5,05–8,05) | 2,75 (1,35–3,6) | 1,8 (1,05–2,7) | 1,1 (0,5–2,15) | 0,7 (0,3–1,75) | 0,7 (0,2–1,35) |
| Ejderha (`dragon`) | 0 | 0,15 (0–0,7) | 1,95 (1,55–2,2) | 1,8 (1,15–2,15) | 1,7 (0–2,05) | 1,3 (0–2,05) |
| Ölüm/terk (`death`) | 0,2 (0,1–0,35) | 0,85 (0,25–1,7) | 1,05 (0,3–1,75) | 1,25 (0,55–2,7) | 1,55 (0,8–2,8) | 1,8 (1,05–2,6) |
| Ticaret (`trade`) | 1,35 (0,75–2,65) | 1,95 (0,85–3,1) | 1,1 (0,4–1,7) | 0,85 (0,25–1,95) | 0,65 (0,1–2,3) | 0,45 (0–2,25) |
| epitaph | 0,2 (0–0,35) | 0,85 (0,25–1,7) | 1 (0,25–1,9) | 1,2 (0,55–2,85) | 1,25 (0,85–2,8) | 1,75 (0,85–2,55) |
| Dünya (`world`) | 0,05 (0–0,3) | 0,3 (0,1–0,9) | 0,9 (0,5–1,3) | 1,05 (0,65–1,5) | 1,2 (0,45–2,25) | 1 (0,65–2) |
| Büyüme (`growth`) | 1,65 (1,35–1,95) | 1,35 (1–1,6) | 0,1 (0,05–0,3) | 0 (0–0,15) | 0 (0–0,1) | 0 (0–0,1) |
| Diplomasi (`diplomacy`) | 0,6 (0,3–1,4) | 0,7 (0,2–1,25) | 0,4 (0,05–0,85) | 0,35 (0,1–0,85) | 0,15 (0,1–0,7) | 0,3 (0–0,8) |
| Çağ (`era`) | 1,4 (1,2–1,55) | 0,8 (0,65–0,9) | 0,2 (0,05–0,3) | 0 (0–0,1) | 0 | 0 |
| Gerginlik (`tension`) | 0,7 (0,25–1,5) | 0,6 (0,15–1,25) | 0,3 (0–1,05) | 0,25 (0–0,75) | 0,1 (0–0,45) | 0,1 (0–0,9) |
| Temas (`contact`) | 0,95 (0,6–1,55) | 0,55 (0,2–0,95) | 0,25 (0,1–0,45) | 0,1 (0–0,35) | 0,1 (0–0,2) | 0 (0–0,1) |
| Kriz (anlatıcı) (`crisis`) | 0 (0–0,1) | 0,3 (0,2–0,4) | 0,35 (0,1–0,55) | 0,3 (0,05–0,5) | 0,4 (0,1–0,6) | 0,5 (0,15–0,6) |
| Rahatlama (anlatıcı) (`relief`) | 0,2 (0,1–0,3) | 0,1 (0–0,15) | 0,1 (0–0,2) | 0,1 (0–0,2) | 0,1 (0–0,25) | 0,05 (0–0,1) |
| Harika (`wonder`) | 0 | 0,2 (0,05–0,3) | 0,3 (0,2–0,45) | 0,05 (0–0,2) | 0 (0–0,2) | 0 (0–0,15) |

## Büyük olay türleri

Dünya başına yıllık büyük olay sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Sefer/ilan (`quest`) | 1,5 (1,05–2,15) | 6,25 (4,95–7,35) | 6,6 (4,7–9,45) | 7,85 (5,55–13,8) | 12,2 (6,55–19,3) | 12,1 (7,95–22,3) |
| Kahraman (`hero`) | 0,6 (0,3–0,9) | 4,6 (3,05–5,65) | 5,6 (3,9–7,3) | 6,8 (5–9,15) | 9,55 (6,7–12,5) | 9,5 (6,95–13,3) |
| Savaş (`war`) | 0,4 (0–1,05) | 4,7 (1,6–6,95) | 6,6 (1–13,7) | 6,65 (0,25–13,8) | 8,25 (0–13,5) | 7,2 (0–16,3) |
| Kamp (`lair`) | 2 (1,85–2,25) | 3,8 (2,65–5) | 3,95 (2,8–5,2) | 4,35 (2,5–5,75) | 5,1 (3,1–6,45) | 5,7 (3,9–7,25) |
| Araştırma (`research`) | 5,9 (4,9–7,6) | 14,8 (11,9–16,1) | 3,25 (1,7–4,9) | 0,5 (0–2,2) | 0,05 (0–0,6) | 0 (0–0,6) |
| İnşaat (`build`) | 2,15 (1,65–2,75) | 9,7 (6,7–17,3) | 5 (2,8–10,4) | 1,4 (0,75–3,9) | 1 (0,45–2,85) | 1,1 (0,2–2,8) |
| Baskın (`raid`) | 3,45 (2,35–4,15) | 3,3 (2,4–4,1) | 1,55 (1,05–2,75) | 2,05 (1,5–2,95) | 2,4 (1,25–4,2) | 2,55 (1,75–4) |
| Deniz (`sea`) | 0,4 (0,05–0,8) | 2,2 (1,6–3,35) | 2,4 (1–4,5) | 2,8 (1,15–4,05) | 1,75 (0,85–4,65) | 2,35 (0,4–4,8) |
| Han (`inn`) | 1,4 (1,25–1,8) | 2,35 (1,8–2,65) | 1,75 (1,1–2,7) | 1,5 (0,9–2,2) | 1,85 (1–2,5) | 2,1 (1,45–3,1) |
| Sınıf (`class`) | 1,9 (1,3–2,75) | 2,4 (1,5–3,4) | 2,3 (1,05–3,15) | 1,2 (0,25–2,2) | 1,25 (0,1–2,2) | 1,15 (0,05–2,05) |
| Yerleşim (`settle`) | 2,35 (1,95–2,8) | 2,6 (1,85–3,25) | 1,1 (0,85–1,4) | 0,75 (0,45–1,35) | 0,55 (0,4–1) | 0,5 (0,3–1,35) |
| Ejderha (`dragon`) | 0 | 0,15 (0–0,7) | 1,95 (1,55–2,2) | 1,8 (1,15–2,15) | 1,7 (0–2,05) | 1,3 (0–2,05) |
| Ölüm/terk (`death`) | 0,2 (0,1–0,35) | 0,85 (0,25–1,7) | 1,05 (0,3–1,75) | 1,25 (0,55–2,7) | 1,55 (0,8–2,8) | 1,8 (1,05–2,6) |
| epitaph | 0,2 (0–0,35) | 0,85 (0,25–1,7) | 1 (0,25–1,9) | 1,2 (0,55–2,85) | 1,25 (0,85–2,8) | 1,75 (0,85–2,55) |
| Keşif (`discover`) | 2,2 (1,7–3,15) | 1,3 (0,55–1,85) | 0,7 (0,45–1,75) | 0,6 (0,15–1,05) | 0,3 (0,15–0,85) | 0,3 (0,05–0,8) |
| Ekonomi (`economy`) | 0 | 1,2 (0,65–2) | 2 (1,35–2,6) | 1,05 (0,7–1,35) | 0,55 (0,2–0,7) | 0,2 (0,1–0,55) |
| Dünya (`world`) | 0,05 (0–0,25) | 0,25 (0,1–0,65) | 0,8 (0,35–0,9) | 0,7 (0,4–0,95) | 0,9 (0,3–1,45) | 0,75 (0,45–1,25) |
| Ticaret (`trade`) | 0,8 (0,45–1,35) | 0,8 (0,35–1,2) | 0,5 (0,15–0,8) | 0,35 (0,1–1) | 0,3 (0,05–1,3) | 0,3 (0–0,85) |
| Çağ (`era`) | 1,4 (1,2–1,55) | 0,8 (0,65–0,9) | 0,2 (0,05–0,3) | 0 (0–0,1) | 0 | 0 |
| Büyüme (`growth`) | 0,9 (0,65–1,1) | 1,3 (1–1,5) | 0,1 (0,05–0,3) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) |
| Gerginlik (`tension`) | 0,7 (0,25–1,5) | 0,6 (0,15–1,25) | 0,3 (0–1,05) | 0,25 (0–0,75) | 0,1 (0–0,45) | 0,1 (0–0,9) |
| Temas (`contact`) | 0,95 (0,6–1,55) | 0,55 (0,2–0,95) | 0,25 (0,1–0,45) | 0,1 (0–0,35) | 0,1 (0–0,2) | 0 (0–0,1) |
| Kriz (anlatıcı) (`crisis`) | 0 (0–0,1) | 0,3 (0,2–0,4) | 0,35 (0,1–0,55) | 0,3 (0,05–0,5) | 0,4 (0,1–0,6) | 0,5 (0,15–0,6) |
| Göç (`migration`) | 0,1 (0–0,3) | 0,2 (0,05–0,35) | 0,3 (0,15–0,4) | 0,15 (0,05–0,3) | 0,2 (0,1–0,3) | 0,2 (0–0,4) |
| Diplomasi (`diplomacy`) | 0,2 (0,1–0,55) | 0,3 (0,15–0,8) | 0,2 (0,05–0,65) | 0,2 (0,1–0,5) | 0,1 (0,1–0,45) | 0,15 (0–0,45) |
| Rahatlama (anlatıcı) (`relief`) | 0,2 (0,1–0,3) | 0,1 (0–0,15) | 0,1 (0–0,2) | 0,1 (0–0,2) | 0,1 (0–0,25) | 0,05 (0–0,1) |
| Harika (`wonder`) | 0 | 0,2 (0,05–0,3) | 0,3 (0,2–0,45) | 0,05 (0–0,2) | 0 (0–0,2) | 0 (0–0,15) |

## Muharebe türleri

Dünya başına yıllık muharebe sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Kamp saldırısı (`camp`) | 0 (0–0,15) | 2 (1,5–2,6) | 1,7 (1,25–2,1) | 1,85 (0,95–2,7) | 2,05 (1,2–2,75) | 2,4 (1,6–3,15) |
| Yapı baskını (canavar) (`extRaid`) | 2,2 (1,45–2,75) | 2,35 (1,55–2,8) | 1 (0,65–1,9) | 1,2 (0,85–2,2) | 1,55 (0,8–2,6) | 1,2 (1–2,05) |
| Yağma akını (`plunder`) | 0,1 (0–0,3) | 1 (0–1,85) | 1,55 (0–3,4) | 1,45 (0–3,5) | 1,8 (0–3,65) | 1,3 (0–4,5) |
| Yerleşim baskını (canavar) (`raid`) | 1,65 (1–2,1) | 1,35 (0,9–1,95) | 0,7 (0,35–1,1) | 0,8 (0,4–0,95) | 0,9 (0,5–1,35) | 0,8 (0,25–1,35) |
| Kuşatma (`siege`) | 0,05 (0–0,2) | 0,75 (0,45–1,25) | 0,95 (0,3–1,85) | 1,2 (0,1–1,75) | 1,4 (0–1,95) | 1,1 (0–1,85) |
| Trol (`troll`) | 0 | 0,25 (0–0,55) | 0,85 (0,35–1,6) | 0,8 (0,5–1,5) | 1,25 (0,8–1,95) | 1,6 (1–2,35) |
| Ejderha (`dragon`) | 0 | 0 (0–0,3) | 0,95 (0,55–1,2) | 0,8 (0,4–1,1) | 0,75 (0–1) | 0,3 (0–0,95) |
| Korsan savaşı (`pirate`) | 0 | 0,1 (0–0,25) | 0,05 (0–0,1) | 0,1 (0–0,25) | 0,1 (0–0,25) | 0,05 (0–0,25) |
| Han baskını (canavar) (`innMonster`) | 0,2 (0,05–0,35) | 0,1 (0–0,2) | 0,05 (0–0,15) | 0 (0–0,2) | 0 (0–0,25) | 0 (0–0,25) |
| Deniz savaşı (`naval`) | 0 | 0 | 0 (0–0,3) | 0,1 (0–0,45) | 0,05 (0–0,55) | 0 (0–0,5) |
| Yol pususu (`ambush`) | 0 (0–0,1) | 0 (0–0,1) | 0 | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) |
| Düello (`duel`) | 0 | 0 | 0 | 0 | 0 | 0 (0–0,15) |
| Han baskını (medeniyet) (`innCiv`) | 0 | 0 (0–0,1) | 0 (0–0,05) | 0 (0–0,1) | 0 (0–0,05) | 0 (0–0,1) |
| Kervan soygunu (`robbery`) | 0 | 0 (0–0,05) | 0 (0–0,1) | 0 | 0 (0–0,15) | 0 (0–0,25) |

## Kamp türleri

Dünya başına yaşayan kamp (yıl sonu değerlerinin on yıllık ortalaması): dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Goblin (`goblin`) | 5,85 (4,65–6,4) | 3,25 (2,1–4,25) | 1,9 (1,2–2,75) | 1,6 (1,05–2,85) | 1,9 (1–2,9) | 2,05 (1,2–3,15) |
| Hobgoblin (`hobgoblin`) | 1,5 (1,3–1,8) | 2,35 (1,4–3,1) | 2,25 (1,3–3,65) | 3,2 (2–4,2) | 3,6 (2,5–4,8) | 3,2 (2,6–5,3) |
| Bugbear (`bugbear`) | 0,45 (0–0,7) | 0,9 (0,4–1,2) | 0,8 (0,3–1,45) | 1,4 (0,45–1,95) | 1 (0,3–1,85) | 0,9 (0,45–1,7) |
| Trol (`troll`) | 0 | 0,2 (0–0,8) | 0,85 (0,35–1,65) | 1,1 (0,2–1,95) | 1,3 (0,7–2,45) | 1,6 (1,15–2,75) |
| Ejderha (`dragon`) | 0 | 0,05 (0–0,3) | 1 (0,9–1) | 1 (0,6–1) | 1 (0–1) | 0,9 (0–1) |
| Korsan (`pirate`) | 0 (0–0,1) | 0,7 (0,2–1,25) | 0,45 (0,15–1,2) | 0,75 (0,15–1,4) | 0,6 (0,05–1,05) | 0,3 (0,1–1) |

## Çağ dağılımı

Yaşayan medeniyetlerin çağlara dağılımı, bütün dünyalar (yıl sonu).

| Yıl | I Kamp | II Köy | III Kasaba | IV Krallık |
|---|---|---|---|---|
| 10 | %0,8 | %33 | %62 | %4,7 |
| 20 | %0 | %1,6 | %27 | %71 |
| 30 | %0 | %0,8 | %4,7 | %94 |
| 40 | %0 | %0,8 | %0 | %99 |
| 50 | %0 | %0 | %0 | %100 |
| 60 | %0 | %0 | %0 | %100 |

## Dünyalar

| Seed | Medeniyet | Yerleşim | Nüfus | Çöküş | Efsane | En yüksek Sv | Doğan / ölü kahraman | Ağaç bitişi (yıl, medyan) | Süre (sn) | Son hash |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | 8 | 79 | 4153 | 0 | 4 | 10 | 127 / 35 | 19,5 | 46,7 | `6922b0fc1f0de1c2` |
| 2 | 7 | 60 | 3348 | 2 | 2 | 8 | 198 / 59 | 24 | 41,9 | `27cf9ca4e10bbefa` |
| 3 | 8 | 94 | 4318 | 7 | 2 | 6 | 199 / 96 | 18,5 | 45,2 | `31b95a9c633e8603` |
| 4 | 10 | 96 | 4715 | 11 | 9 | 9 | 207 / 97 | 20 | 50,6 | `4c09f0c4b9b32329` |
| 5 | 8 | 92 | 4188 | 14 | 1 | 8 | 194 / 97 | 17 | 46,7 | `d360a17c4d51a251` |
| 6 | 8 | 84 | 3559 | 2 | 4 | 10 | 128 / 34 | 23 | 35,8 | `1d73c1524c034385` |
| 7 | 9 | 87 | 4062 | 7 | 2 | 8 | 187 / 59 | 20 | 54,3 | `422bf407ad2bd03e` |
| 8 | 7 | 69 | 3769 | 5 | 2 | 9 | 131 / 55 | 18 | 36,3 | `ce6ed3f6f1dc9003` |
| 9 | 7 | 84 | 3805 | 12 | 8 | 9 | 225 / 121 | 20 | 45,4 | `c43e43d036cfa729` |
| 10 | 9 | 94 | 4640 | 7 | 5 | 9 | 203 / 71 | 23 | 48,5 | `9297d7db35e01bce` |
| 11 | 7 | 80 | 3547 | 8 | 4 | 7 | 164 / 67 | 18 | 39,9 | `9153156ad68085b1` |
| 12 | 8 | 83 | 3750 | 2 | 2 | 9 | 147 / 43 | 21 | 36 | `92a016b3cad7700f` |
| 13 | 8 | 88 | 3741 | 7 | 7 | 10 | 198 / 82 | 19 | 43,9 | `14bf2cad497ec1f0` |
| 14 | 7 | 68 | 3362 | 0 | 9 | 10 | 194 / 64 | 21 | 34,7 | `9b3b7d0848227b6d` |
| 15 | 11 | 103 | 4719 | 10 | 5 | 10 | 223 / 111 | 24 | 51,1 | `691a57667cad6a10` |
| 16 | 8 | 88 | 3569 | 4 | 5 | 10 | 139 / 45 | 22 | 37,6 | `0ba6621b4018d936` |

Çöküşler:

- seed 2, 22. yıl (gün 2548): Tatlıçayır Loncası başkenti kaybetti: Kavşakpazar (Pulzırh Lejyonu aldı)
- seed 2, 24. yıl (gün 2796): Tatlıçayır Loncası başkenti kaybetti: Balköprü (Pulzırh Lejyonu aldı)
- seed 3, 18. yıl (gün 2082): Karaörs Derinlikleri başkenti kaybetti: Karagöl (Örsyürek Tapınak Klanı aldı)
- seed 3, 26. yıl (gün 3014): Karaörs Derinlikleri başkenti kaybetti: Sessizocak (Güneştacı Krallığı aldı)
- seed 3, 28. yıl (gün 3329): Karaörs Derinlikleri başkenti kaybetti: Bozkent (Güneştacı Krallığı aldı)
- seed 3, 31. yıl (gün 3633): Karaörs Derinlikleri yok oldu
- seed 3, 39. yıl (gün 4562): Tatlıçayır Loncası başkenti kaybetti: Kavşakpazar (Kanlıdiş Kabileleri aldı)
- seed 3, 47. yıl (gün 5607): Rüzgâr Manastırı başkenti kaybetti: Sessiztepe (Kanlıdiş Kabileleri aldı)
- seed 3, 51. yıl (gün 6018): Rüzgâr Manastırı başkenti kaybetti: Dinginpınar (Kanlıdiş Kabileleri aldı)
- seed 4, 18. yıl (gün 2160): Rüzgâr Manastırı başkenti kaybetti: Sessiztepe (Kanlıdiş Kabileleri aldı)
- seed 4, 29. yıl (gün 3407): Kanlıdiş Kabileleri başkenti kaybetti: İzsürer (Pulzırh Lejyonu aldı)
- seed 4, 31. yıl (gün 3637): Sınır Bekçileri başkenti kaybetti: Yabanyurt (Kanlıdiş Kabileleri aldı)
- seed 4, 31. yıl (gün 3678): Kanlıdiş Kabileleri başkenti kaybetti: Yabanyurt (Pulzırh Lejyonu aldı)
- seed 4, 33. yıl (gün 3859): Rüzgâr Manastırı başkenti kaybetti: Taşbasamak (Kanlıdiş Kabileleri aldı)
- seed 4, 36. yıl (gün 4283): Rüzgâr Manastırı başkenti kaybetti: Bulutkapı (Kanlıdiş Kabileleri aldı)
- seed 4, 38. yıl (gün 4457): Sınır Bekçileri başkenti kaybetti: Gözcüağaç (Pulzırh Lejyonu aldı)
- seed 4, 39. yıl (gün 4674): Kanlıdiş Kabileleri başkenti kaybetti: Çamgözcü (Pulzırh Lejyonu aldı)
- seed 4, 41. yıl (gün 4872): Sınır Bekçileri başkenti kaybetti: Söğütburç (Pulzırh Lejyonu aldı)
- seed 4, 53. yıl (gün 6318): Kanlıdiş Kabileleri başkenti kaybetti: Gökyurt (Pulzırh Lejyonu aldı)
- seed 4, 58. yıl (gün 6874): Kanlıdiş Kabileleri başkenti kaybetti: Demiroba (Pulzırh Lejyonu aldı)
- seed 5, 10. yıl (gün 1181): Karaörs Derinlikleri başkenti kaybetti: Külçukur (Pulzırh Lejyonu aldı)
- seed 5, 13. yıl (gün 1449): Karaörs Derinlikleri yok oldu
- seed 5, 16. yıl (gün 1809): Tatlıçayır Loncası başkenti kaybetti: Fıçıköy (Kanlıdiş Kabileleri aldı)
- seed 5, 34. yıl (gün 4023): Tatlıçayır Loncası başkenti kaybetti: Kavşakpazar (Kanlıdiş Kabileleri aldı)
- seed 5, 41. yıl (gün 4847): Kanlıdiş Kabileleri başkenti kaybetti: Kavşakpazar (Pulzırh Lejyonu aldı)
- seed 5, 44. yıl (gün 5242): Kızılboynuz Soyu başkenti kaybetti: Kızılkül (Güneştacı Krallığı aldı)
- seed 5, 49. yıl (gün 5764): Kanlıdiş Kabileleri başkenti kaybetti: Günkaya (Pulzırh Lejyonu aldı)
- seed 5, 50. yıl (gün 5973): Tatlıçayır Loncası başkenti kaybetti: Meşekale (Kanlıdiş Kabileleri aldı)
- seed 5, 51. yıl (gün 6029): Kanlıdiş Kabileleri başkenti kaybetti: Meşekale (Pulzırh Lejyonu aldı)
- seed 5, 52. yıl (gün 6190): Kızılboynuz Soyu başkenti kaybetti: Aysırt (Güneştacı Krallığı aldı)
- seed 5, 52. yıl (gün 6216): Keseli Beyliği başkenti kaybetti: Keseli (Pulzırh Lejyonu aldı)
- seed 5, 54. yıl (gün 6417): Kanlıdiş Kabileleri başkenti kaybetti: Kurtoba (Pulzırh Lejyonu aldı)
- seed 5, 56. yıl (gün 6611): Keseli Beyliği başkenti kaybetti: Taşdere (Pulzırh Lejyonu aldı)
- seed 5, 59. yıl (gün 7031): Keseli Beyliği yok oldu
- seed 6, 39. yıl (gün 4596): Yeşilyaprak Çemberi başkenti kaybetti: Sessizkoru (Kızılboynuz Soyu aldı)
- seed 6, 54. yıl (gün 6381): Yeşilyaprak Çemberi başkenti kaybetti: Yelköy (Pulzırh Lejyonu aldı)
- seed 7, 32. yıl (gün 3752): Kızılboynuz Soyu başkenti kaybetti: Közsaray (Pulzırh Lejyonu aldı)
- seed 7, 36. yıl (gün 4297): Kızılboynuz Soyu başkenti kaybetti: Sisova (Karaörs Derinlikleri aldı)
- seed 7, 40. yıl (gün 4783): Kızılboynuz Soyu başkenti kaybetti: Taşhisar (Pulzırh Lejyonu aldı)
- seed 7, 45. yıl (gün 5365): Kurtkale Lejyonu yok oldu
- seed 7, 47. yıl (gün 5586): Ayburç Klanı yok oldu
- seed 7, 47. yıl (gün 5594): Pulzırh Lejyonu başkenti kaybetti: Zincirkaya (Yeşilyaprak Çemberi aldı)
- seed 7, 55. yıl (gün 6528): Tatlıçayır Loncası başkenti kaybetti: Kavşakpazar (Kanlıdiş Kabileleri aldı)
- seed 8, 14. yıl (gün 1566): Kızılboynuz Soyu başkenti kaybetti: Közsaray (Örsyürek Tapınak Klanı aldı)
- seed 8, 16. yıl (gün 1828): Kızılboynuz Soyu başkenti kaybetti: Boynuztepe (Örsyürek Tapınak Klanı aldı)
- seed 8, 18. yıl (gün 2095): Kızılboynuz Soyu başkenti kaybetti: Alazvadi (Örsyürek Tapınak Klanı aldı)
- seed 8, 20. yıl (gün 2368): Kızılboynuz Soyu başkenti kaybetti: Karamum (Örsyürek Tapınak Klanı aldı)
- seed 8, 22. yıl (gün 2637): Kızılboynuz Soyu yok oldu
- seed 9, 13. yıl (gün 1511): Sınır Bekçileri başkenti kaybetti: Okyayı (Karaörs Derinlikleri aldı)
- seed 9, 15. yıl (gün 1800): Sınır Bekçileri başkenti kaybetti: Gözcüağaç (Karaörs Derinlikleri aldı)
- seed 9, 18. yıl (gün 2094): Sınır Bekçileri yok oldu
- seed 9, 27. yıl (gün 3214): Karaörs Derinlikleri başkenti kaybetti: Kara Mihrap (Güneştacı Krallığı aldı)
- seed 9, 30. yıl (gün 3496): Karaörs Derinlikleri başkenti kaybetti: Gölköprü (Güneştacı Krallığı aldı)
- seed 9, 32. yıl (gün 3806): Kızılboynuz Soyu başkenti kaybetti: Boynuztepe (Güneştacı Krallığı aldı)
- seed 9, 35. yıl (gün 4100): Karaörs Derinlikleri başkenti kaybetti: Yeşilkaya (Güneştacı Krallığı aldı)
- seed 9, 39. yıl (gün 4574): Karaörs Derinlikleri yok oldu
- seed 9, 39. yıl (gün 4629): Kızılboynuz Soyu başkenti kaybetti: Boynuztepe (Rüzgâr Manastırı aldı)
- seed 9, 43. yıl (gün 5050): Kızılboynuz Soyu başkenti kaybetti: Karamum (Kanlıdiş Kabileleri aldı)
- seed 9, 51. yıl (gün 6095): Kızılboynuz Soyu başkenti kaybetti: Közsaray (Kanlıdiş Kabileleri aldı)
- seed 9, 60. yıl (gün 7173): Kanlıdiş Kabileleri başkenti kaybetti: Közsaray (Pulzırh Lejyonu aldı)
- seed 10, 11. yıl (gün 1313): Karaörs Derinlikleri başkenti kaybetti: Kara Mihrap (Rüzgâr Manastırı aldı)
- seed 10, 28. yıl (gün 3254): Rüzgâr Manastırı başkenti kaybetti: Sessiztepe (Karaörs Derinlikleri aldı)
- seed 10, 30. yıl (gün 3513): Karaörs Derinlikleri başkenti kaybetti: Sessiztepe (Rüzgâr Manastırı aldı)
- seed 10, 39. yıl (gün 4580): Karaörs Derinlikleri başkenti kaybetti: Kurtkent (Rüzgâr Manastırı aldı)
- seed 10, 41. yıl (gün 4859): Karaörs Derinlikleri başkenti kaybetti: Dişlivadi (Rüzgâr Manastırı aldı)
- seed 10, 43. yıl (gün 5086): Zincirkaya Beyliği başkenti kaybetti: Zincirkaya (Karaörs Derinlikleri aldı)
- seed 10, 45. yıl (gün 5345): Zincirkaya Beyliği yok oldu
- seed 11, 19. yıl (gün 2231): Kızılboynuz Soyu başkenti kaybetti: Közsaray (Kanlıdiş Kabileleri aldı)
- seed 11, 22. yıl (gün 2626): Kızılboynuz Soyu başkenti kaybetti: Boynuztepe (Kanlıdiş Kabileleri aldı)
- seed 11, 29. yıl (gün 3447): Kızılboynuz Soyu başkenti kaybetti: Karamum (Kanlıdiş Kabileleri aldı)
- seed 11, 31. yıl (gün 3658): Örsyürek Tapınak Klanı başkenti kaybetti: Kutsalörs (Kanlıdiş Kabileleri aldı)
- seed 11, 41. yıl (gün 4901): Kızılboynuz Soyu başkenti kaybetti: Ceylanoba (Kanlıdiş Kabileleri aldı)
- seed 11, 44. yıl (gün 5267): Taşhisar Beyliği yok oldu
- seed 11, 45. yıl (gün 5302): Kızılboynuz Soyu başkenti kaybetti: Kıvılcımlı (Kanlıdiş Kabileleri aldı)
- seed 11, 58. yıl (gün 6946): Kızılboynuz Soyu başkenti kaybetti: Kocaköprü (Kanlıdiş Kabileleri aldı)
- seed 12, 32. yıl (gün 3838): Kanlıdiş Kabileleri başkenti kaybetti: Kıvılcımlı (Pulzırh Lejyonu aldı)
- seed 12, 60. yıl (gün 7093): Sınır Bekçileri başkenti kaybetti: Kurtgeçit (Pulzırh Lejyonu aldı)
- seed 13, 21. yıl (gün 2425): Tatlıçayır Loncası başkenti kaybetti: Fıçıköy (Rüzgâr Manastırı aldı)
- seed 13, 22. yıl (gün 2624): Kızılyurt Serbest Şehri yok oldu
- seed 13, 23. yıl (gün 2742): Sınır Bekçileri başkenti kaybetti: Gözcüağaç (Rüzgâr Manastırı aldı)
- seed 13, 30. yıl (gün 3588): Sınır Bekçileri başkenti kaybetti: Okyayı (Kanlıdiş Kabileleri aldı)
- seed 13, 34. yıl (gün 3994): Sınır Bekçileri başkenti kaybetti: Kurtgeçit (Kanlıdiş Kabileleri aldı)
- seed 13, 37. yıl (gün 4349): Tatlıçayır Loncası başkenti kaybetti: Balköprü (Kızılboynuz Soyu aldı)
- seed 13, 54. yıl (gün 6373): Tatlıçayır Loncası başkenti kaybetti: Alacayayla (Kızılboynuz Soyu aldı)
- seed 15, 20. yıl (gün 2281): Kanlıdiş Kabileleri başkenti kaybetti: İzsürer (Sınır Bekçileri aldı)
- seed 15, 35. yıl (gün 4099): Kızılboynuz Soyu başkenti kaybetti: Kızılkül (Kanlıdiş Kabileleri aldı)
- seed 15, 41. yıl (gün 4836): Toynakbaş Boyu başkenti kaybetti: Bozkale (Güneştacı Krallığı aldı)
- seed 15, 43. yıl (gün 5045): Kızılboynuz Soyu başkenti kaybetti: Kızıltepe (Kanlıdiş Kabileleri aldı)
- seed 15, 48. yıl (gün 5726): Kurtoba Boyu başkenti kaybetti: Kurtoba (Toynakbaş Boyu aldı)
- seed 15, 50. yıl (gün 5945): Kızılboynuz Soyu başkenti kaybetti: Kuzeybük (Kanlıdiş Kabileleri aldı)
- seed 15, 52. yıl (gün 6240): Kurtoba Boyu başkenti kaybetti: Kocapınar (Toynakbaş Boyu aldı)
- seed 15, 55. yıl (gün 6493): Kızılboynuz Soyu başkenti kaybetti: Meşekent (Kanlıdiş Kabileleri aldı)
- seed 15, 59. yıl (gün 6963): Kocaköprü Boyu başkenti kaybetti: Kocaköprü (Kurtoba Boyu aldı)
- seed 15, 60. yıl (gün 7083): Kurtoba Boyu başkenti kaybetti: Karlıtepe (Toynakbaş Boyu aldı)
- seed 16, 7. yıl (gün 808): Güneştacı Krallığı yok oldu
- seed 16, 32. yıl (gün 3744): Karaörs Derinlikleri başkenti kaybetti: Sessizocak (Örsyürek Tapınak Klanı aldı)
- seed 16, 55. yıl (gün 6527): Karaörs Derinlikleri başkenti kaybetti: Keseli (Tatlıçayır Loncası aldı)
- seed 16, 59. yıl (gün 6995): Örsyürek Tapınak Klanı başkenti kaybetti: Derinmihrap (Karaörs Derinlikleri aldı)

## Yıllık ayrıntı

Hücre: medyan (p10–p90), 16 dünya. Yıl y = (y−1)·120+1 … y·120. günler. Bütün değerler `report.json` içinde (`metrics`), dünya başına değerler `../runs/f1b-2` altında.

### Medeniyet (1/4)

| Yıl | Yaşayan medeniyet | Yeni medeniyet (yeniden doğan) | Yok olan medeniyet | Başkent kaybı (medeniyet yaşarken) | Çöküş (yok olma + başkent kaybı) | Yaşayan yerleşim |
|---|---|---|---|---|---|---|
| 1 | 8 (7–9) | 0 | 0 | 0 | 0 | 8 (7–9) |
| 2 | 8 (7–9) | 0 | 0 | 0 | 0 | 8 (7–9) |
| 3 | 8 (7–9) | 0 | 0 | 0 | 0 | 8 (7–9) |
| 4 | 8 (7–9) | 0 | 0 | 0 | 0 | 8 (7–9,5) |
| 5 | 8 (7–9) | 0 | 0 | 0 | 0 | 10 (8–12) |
| 6 | 8 (7–9) | 0 | 0 | 0 | 0 | 13 (10–15) |
| 7 | 8 (7–9) | 0 | 0 | 0 | 0 | 18 (13,5–20,5) |
| 8 | 8 (7–9) | 0 | 0 | 0 | 0 | 21 (17,5–26) |
| 9 | 8 (7–9) | 0 | 0 | 0 | 0 | 25,5 (21,5–31,5) |
| 10 | 8 (7–9) | 0 | 0 | 0 | 0 | 31,5 (27–36,5) |
| 11 | 8 (7–9) | 0 | 0 | 0 | 0 | 35 (30,5–41) |
| 12 | 8 (7–9) | 0 | 0 | 0 | 0 | 39 (32,5–44) |
| 13 | 8 (7–9) | 0 | 0 | 0 | 0 (0–0,5) | 41,5 (36–47) |
| 14 | 8 (7–9) | 0 | 0 | 0 | 0 | 44 (37,5–50) |
| 15 | 8 (7–9) | 0 | 0 | 0 | 0 | 48 (39–52,5) |
| 16 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 50,5 (41–56,5) |
| 17 | 8 (7–9) | 0 | 0 | 0 | 0 | 54 (42–59,5) |
| 18 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 55 (45–62) |
| 19 | 8 (7–9) | 0 | 0 | 0 | 0 | 56 (46,5–63) |
| 20 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 57 (47,5–64) |
| 21 | 8 (7–9) | 0 | 0 | 0 | 0 | 58 (49–66) |
| 22 | 8 (7–9) | 0 | 0 (0–0,5) | 0 (0–0,5) | 0 (0–1) | 60 (49,5–66,5) |
| 23 | 8 (7–9) | 0 | 0 | 0 | 0 | 62 (49,5–67,5) |
| 24 | 8 (7–9) | 0 | 0 | 0 | 0 | 63 (51–70) |
| 25 | 8 (7–9) | 0 | 0 | 0 | 0 | 65,5 (52–72,5) |
| 26 | 8 (7–9) | 0 | 0 | 0 | 0 | 66 (54–73) |
| 27 | 8 (7–9) | 0 | 0 | 0 | 0 | 68 (55–74,5) |
| 28 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 69 (56–75) |
| 29 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 70 (56–76,5) |
| 30 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 70,5 (56,5–76,5) |
| 31 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–1) | 71 (57,5–77) |
| 32 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 71 (58,5–77,5) |
| 33 | 8 (7–9) | 0 | 0 | 0 | 0 | 71,5 (59,5–78,5) |
| 34 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 72 (60,5–79,5) |
| 35 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 72,5 (61–80) |
| 36 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 73 (61,5–81) |
| 37 | 8 (7–9) | 0 | 0 | 0 | 0 | 74 (62–81,5) |
| 38 | 8 (7–9) | 0 | 0 | 0 | 0 | 74,5 (62,5–83) |
| 39 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 74,5 (62,5–83) |
| 40 | 8 (7–9) | 0 | 0 | 0 | 0 | 76 (63–83,5) |
| 41 | 8 (7–9,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 76,5 (64–83,5) |
| 42 | 8 (7–9,5) | 0 (0–0,5) | 0 | 0 | 0 | 78 (64,5–84) |
| 43 | 8 (7–9,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 78,5 (64,5–85,5) |
| 44 | 8 (7–10) | 0 | 0 | 0 | 0 (0–0,5) | 78,5 (65–86) |
| 45 | 8 (7–9,5) | 0 | 0 (0–0,5) | 0 | 0 (0–1) | 79 (64,5–86) |
| 46 | 8 (7–9,5) | 0 | 0 | 0 | 0 | 79,5 (65–86,5) |
| 47 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 80 (65–87,5) |
| 48 | 8 (7–9) | 0 | 0 | 0 | 0 | 80,5 (65–89,5) |
| 49 | 8 (7–9) | 0 | 0 | 0 | 0 | 81,5 (66–89,5) |
| 50 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 82 (66,5–90) |
| 51 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 83 (67–90) |
| 52 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 83,5 (67–91) |
| 53 | 8 (7–9) | 0 | 0 | 0 | 0 | 83,5 (67,5–91,5) |
| 54 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 84 (68,5–91,5) |
| 55 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 85 (68,5–91,5) |
| 56 | 8 (7–9) | 0 | 0 | 0 | 0 | 85 (68,5–92) |
| 57 | 8 (7–9) | 0 | 0 | 0 | 0 | 86 (68,5–93) |
| 58 | 8 (7–9,5) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 85 (68,5–93,5) |
| 59 | 8 (7–9,5) | 0 | 0 | 0 (0–0,5) | 0 (0–1) | 85 (68,5–94,5) |
| 60 | 8 (7–9,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 85,5 (68,5–95) |

### Medeniyet (2/4)

| Yıl | Medeniyet başına yerleşim | 5+ kara yerleşimli medeniyet payı | Kurulan yerleşim | Fethedilen yerleşim | Terk edilen yerleşim | Toplam nüfus |
|---|---|---|---|---|---|---|
| 1 | 1 | %0 | 0 | 0 | 0 | 64,5 (55–75) |
| 2 | 1 | %0 | 0 | 0 | 0 | 85,5 (72–99) |
| 3 | 1 | %0 | 0 | 0 | 0 | 112 (94,5–129) |
| 4 | 1 (1–1,06) | %0 | 0 (0–0,5) | 0 | 0 | 142 (117–166) |
| 5 | 1,25 (1,12–1,4) | %0 | 2 (1–3) | 0 | 0 | 190 (144–220) |
| 6 | 1,65 (1,38–1,86) | %0 | 3 (2–4,5) | 0 | 0 | 240 (188–284) |
| 7 | 2,25 (1,86–2,54) | %0 | 4 (3–6) | 0 | 0 | 312 (230–360) |
| 8 | 2,76 (2,29–3,13) | %0 (%0–%5,6) | 4,5 (2,5–6) | 0 | 0 | 410 (298–466) |
| 9 | 3,44 (2,76–3,77) | %12 (%0–%29) | 4 (3,5–7) | 0 (0–1) | 0 | 524 (390–604) |
| 10 | 4 (3,53–4,39) | %40 (%14–%57) | 5 (3,5–6,5) | 0 (0–1) | 0 | 642 (506–755) |
| 11 | 4,43 (3,94–4,75) | %56 (%35–%75) | 4 (2–5) | 0,5 (0–1,5) | 0 | 770 (644–912) |
| 12 | 4,82 (4,34–5,13) | %73 (%50–%94) | 3 (2–5) | 0 (0–1) | 0 | 894 (748–1048) |
| 13 | 5,18 (4,75–5,63) | %76 (%56–%88) | 2,5 (1–5) | 1 (0–1,5) | 0 | 1040 (838–1189) |
| 14 | 5,5 (5,29–5,94) | %87 (%67–%100) | 3 (1–4,5) | 1 (0–1,5) | 0 | 1168 (939–1370) |
| 15 | 5,88 (5,36–6,24) | %100 (%73–%100) | 2,5 (2–4) | 1 (0–1,5) | 0 | 1336 (1052–1522) |
| 16 | 6,27 (5,67–6,53) | %94 (%80–%100) | 3 (1,5–4,5) | 1 (0–1,5) | 0 | 1471 (1171–1661) |
| 17 | 6,54 (5,93–6,88) | %100 (%87–%100) | 3 (1–3,5) | 1 (0–1,5) | 0 | 1612 (1282–1817) |
| 18 | 6,89 (6,27–7,35) | %100 (%87–%100) | 2 (1–3) | 1 (0–2) | 0 | 1730 (1380–1932) |
| 19 | 7 (6,53–7,49) | %100 (%87–%100) | 1 (0–2,5) | 1 (0–2) | 0 | 1860 (1482–2024) |
| 20 | 7,18 (6,73–7,65) | %100 (%87–%100) | 1 (0–3) | 1 (0–2) | 0 | 1982 (1608–2162) |
| 21 | 7,25 (6,82–7,87) | %94 (%87–%100) | 2 (0–3) | 0,5 (0–2,5) | 0 (0–0,5) | 2071 (1692–2298) |
| 22 | 7,53 (6,93–8) | %100 (%87–%100) | 1 (0–2,5) | 1 (0–2) | 0 | 2182 (1757–2388) |
| 23 | 7,83 (7–8,21) | %100 (%87–%100) | 1 (0–2) | 1 (0–3,5) | 0 | 2278 (1852–2524) |
| 24 | 7,94 (7,07–8,35) | %100 (%87–%100) | 1,5 (0–3,5) | 0,5 (0–2) | 0 | 2410 (1908–2574) |
| 25 | 8,18 (7,21–8,5) | %100 (%87–%100) | 1 (0,5–3) | 0,5 (0–2,5) | 0 | 2463 (1971–2757) |
| 26 | 8,26 (7,36–8,69) | %100 (%87–%100) | 1 (0–2,5) | 1 (0–2) | 0 | 2555 (2056–2781) |
| 27 | 8,38 (7,51–8,81) | %100 (%87–%100) | 1 (0–2) | 1 (0–2,5) | 0 | 2630 (2144–2896) |
| 28 | 8,44 (7,62–8,88) | %100 (%87–%100) | 0,5 (0–1,5) | 0,5 (0–2,5) | 0 | 2741 (2208–3004) |
| 29 | 8,56 (7,79–8,94) | %100 (%87–%100) | 1 (0–2,5) | 1 (0–3) | 0 | 2834 (2275–3068) |
| 30 | 8,59 (7,91–9,06) | %100 (%87–%100) | 1 (0–1) | 1 (0–3) | 0 | 2912 (2338–3181) |
| 31 | 8,73 (7,99–9,25) | %100 (%87–%100) | 1 (0–1) | 1 (0–2) | 0 | 2990 (2376–3248) |
| 32 | 8,71 (8–9,33) | %100 (%87–%100) | 1 (0–1) | 1 (0–3,5) | 0 | 3062 (2467–3270) |
| 33 | 8,86 (8,14–9,33) | %100 (%87–%100) | 1 (0–2) | 1 (0–2) | 0 | 3108 (2504–3352) |
| 34 | 8,94 (8,21–9,4) | %100 (%87–%100) | 1 (0–2) | 1 (0–1) | 0 | 3150 (2596–3396) |
| 35 | 9 (8,26–9,46) | %100 (%87–%100) | 0,5 (0–1) | 1,5 (0–2,5) | 0 | 3238 (2651–3448) |
| 36 | 9,12 (8,39–9,67) | %100 (%87–%100) | 1 (0–2,5) | 0,5 (0–2) | 0 | 3272 (2681–3561) |
| 37 | 9,17 (8,39–9,88) | %100 (%86–%100) | 1 (0–2) | 1,5 (0–2) | 0 (0–1) | 3332 (2660–3572) |
| 38 | 9,33 (8,46–9,93) | %100 (%86–%100) | 1 (0–2) | 0 (0–2) | 0 | 3332 (2802–3650) |
| 39 | 9,31 (8,63–10,1) | %100 (%86–%100) | 0 (0–1) | 1 (0–2) | 0 (0–0,5) | 3437 (2827–3676) |
| 40 | 9,5 (8,7–10,3) | %100 (%86–%100) | 1 (0–2) | 1 (0–2) | 0 | 3538 (2831–3766) |
| 41 | 9,56 (8,35–10,3) | %100 (%86–%100) | 1 (0–1) | 1 (0–3) | 0 | 3507 (2907–3780) |
| 42 | 9,56 (8,34–10,5) | %100 (%83–%100) | 1 (0–1,5) | 1 (0–2) | 0 | 3531 (2862–3802) |
| 43 | 9,56 (8,44–10,5) | %100 (%83–%100) | 0,5 (0–1) | 1 (0–3) | 0 (0–0,5) | 3606 (2932–3896) |
| 44 | 9,81 (8,24–10,5) | %100 (%86–%100) | 0 (0–1,5) | 1 (0–2) | 0 (0–1) | 3624 (3056–3901) |
| 45 | 9,94 (8,4–10,6) | %100 (%86–%100) | 0 (0–1,5) | 1 (0–2) | 0 (0–0,5) | 3614 (2962–4056) |
| 46 | 9,94 (8,45–10,6) | %100 (%87–%100) | 0,5 (0–1) | 1 (0–3) | 0 (0–1) | 3661 (3018–4042) |
| 47 | 9,94 (8,92–10,7) | %100 (%88–%100) | 1 (0–1,5) | 0 (0–2,5) | 0 (0–1) | 3773 (3064–4014) |
| 48 | 10,1 (9,02–10,7) | %100 (%88–%100) | 0 (0–2) | 1 (0–2) | 0 (0–0,5) | 3588 (3075–4068) |
| 49 | 10,1 (9,09–10,8) | %100 (%88–%100) | 1 (0–1,5) | 0,5 (0–2) | 0 (0–1) | 3728 (3046–4153) |
| 50 | 10,1 (9–10,9) | %100 (%88–%100) | 1 (0–1,5) | 0 (0–3) | 0 (0–1) | 3732 (3145–4130) |
| 51 | 10,1 (9,11–11,1) | %100 (%87–%100) | 0 (0–2) | 0,5 (0–4) | 0 (0–1) | 3730 (3199–4218) |
| 52 | 10,2 (9,22–11,1) | %100 (%87–%100) | 1 (0–1) | 1 (0–2,5) | 0 (0–0,5) | 3892 (3169–4339) |
| 53 | 10,2 (8,95–11,3) | %100 (%84–%100) | 0,5 (0–2) | 1 (0–2) | 0 (0–0,5) | 3825 (3172–4354) |
| 54 | 10 (9,01–11,3) | %100 (%80–%100) | 1 (0–1) | 0 (0–2) | 0 (0–0,5) | 3816 (3274–4381) |
| 55 | 9,99 (9,06–11,4) | %100 (%80–%100) | 0 (0–1) | 1 (0–2) | 0 (0–0,5) | 3825 (3237–4522) |
| 56 | 10,1 (9,15–11,5) | %100 (%76–%100) | 0 (0–1) | 1 (0–2,5) | 0 (0–1) | 3801 (3360–4418) |
| 57 | 9,99 (9,24–11,5) | %95 (%82–%100) | 1 (0–2) | 0,5 (0–2) | 0 (0–1) | 3832 (3379–4404) |
| 58 | 9,94 (9,25–11,5) | %95 (%78–%100) | 0,5 (0–1,5) | 1 (0–3) | 0 (0–1) | 3852 (3446–4492) |
| 59 | 10,3 (9,39–11,6) | %95 (%87–%100) | 0 (0–2) | 0 (0–2) | 0 (0–1) | 3920 (3412–4448) |
| 60 | 10,4 (9,48–11,6) | %100 (%87–%100) | 1 (0–1,5) | 1 (0–3) | 0 | 3787 (3454–4678) |

### Medeniyet (3/4)

| Yıl | Ortalama çağ | Araştırma ağacının biten payı (ort.) | Ağacı bitmiş medeniyet payı | Araştırması duran medeniyet payı | Altın medyanı (medeniyetler) | Boştaki iş gücü payı |
|---|---|---|---|---|---|---|
| 1 | 1 | %0,4 (%0–%0,8) | %0 | %0 | 8,42 (8,29–15,9) | %0 |
| 2 | 1 | %4 (%3,5–%4,3) | %0 | %0 | 24,4 (13,2–37,5) | %0 |
| 3 | 1,13 (1–1,29) | %8 (%7,4–%8,5) | %0 | %0 | 35,7 (20,9–48,2) | %0,3 (%0–%2) |
| 4 | 1,88 (1,76–2) | %12 (%11–%13) | %0 | %0 | 42,1 (29,2–59,9) | %0,2 (%0–%1) |
| 5 | 2 (1,86–2) | %15 (%14–%16) | %0 | %0 | 49,9 (37,7–63,1) | %0,1 (%0–%1,2) |
| 6 | 2 (2–2,12) | %19 (%18–%20) | %0 | %0 | 63,1 (42,5–78,6) | %0 (%0–%0,6) |
| 7 | 2,13 (2–2,27) | %24 (%23–%26) | %0 | %0 | 75,8 (37,1–94,3) | %0,4 (%0–%0,9) |
| 8 | 2,4 (2,25–2,6) | %30 (%28–%32) | %0 | %0 (%0–%5,6) | 71,8 (44,2–84) | %0,3 (%0–%1) |
| 9 | 2,56 (2,36–2,79) | %36 (%33–%38) | %0 | %0 | 81,1 (39,2–114) | %0,2 (%0–%0,9) |
| 10 | 2,71 (2,56–2,88) | %42 (%39–%45) | %0 | %0 (%0–%13) | 65,9 (39,3–129) | %0,4 (%0–%1,7) |
| 11 | 2,87 (2,71–3,06) | %49 (%45–%52) | %0 | %11 (%0–%27) | 87 (45,5–168) | %0,7 (%0,1–%2,2) |
| 12 | 3 (2,88–3,2) | %55 (%52–%59) | %0 | %13 (%0–%24) | 94,9 (55,1–201) | %1,5 (%0,2–%3,2) |
| 13 | 3,17 (3–3,41) | %62 (%57–%65) | %0 | %27 (%12–%47) | 138 (70,5–262) | %2,1 (%0,7–%4,1) |
| 14 | 3,28 (3,06–3,54) | %67 (%62–%71) | %0 (%0–%13) | %29 (%22–%54) | 177 (90,3–284) | %3,5 (%2,4–%5,3) |
| 15 | 3,38 (3,07–3,73) | %71 (%66–%75) | %5,6 (%0–%14) | %41 (%25–%67) | 162 (75,7–301) | %4,2 (%3,7–%8,4) |
| 16 | 3,5 (3,14–3,75) | %75 (%69–%79) | %12 (%0–%31) | %50 (%29–%67) | 221 (112–340) | %6,7 (%4,7–%10) |
| 17 | 3,59 (3,2–3,75) | %78 (%71–%82) | %27 (%6,3–%44) | %60 (%47–%76) | 256 (107–371) | %10 (%6,8–%13) |
| 18 | 3,65 (3,27–3,76) | %82 (%74–%85) | %38 (%14–%54) | %73 (%46–%87) | 272 (162–378) | %11 (%9–%17) |
| 19 | 3,75 (3,43–3,87) | %84 (%75–%87) | %41 (%14–%54) | %71 (%46–%76) | 284 (178–407) | %13 (%10–%18) |
| 20 | 3,75 (3,53–3,87) | %86 (%78–%90) | %47 (%31–%73) | %75 (%57–%88) | 325 (212–450) | %15 (%13–%21) |
| 21 | 3,78 (3,56–3,94) | %88 (%82–%91) | %62 (%35–%69) | %75 (%60–%89) | 338 (204–457) | %17 (%12–%23) |
| 22 | 3,88 (3,62–4) | %89 (%84–%93) | %69 (%38–%82) | %80 (%57–%94) | 334 (111–414) | %16 (%15–%23) |
| 23 | 3,88 (3,62–4) | %91 (%86–%94) | %73 (%47–%87) | %88 (%65–%100) | 326 (129–459) | %18 (%15–%22) |
| 24 | 3,88 (3,69–4) | %92 (%87–%94) | %75 (%57–%88) | %87 (%73–%100) | 335 (184–472) | %19 (%15–%23) |
| 25 | 3,89 (3,69–4) | %93 (%88–%96) | %86 (%62–%89) | %88 (%86–%100) | 404 (205–477) | %19 (%15–%24) |
| 26 | 3,94 (3,75–4) | %93 (%89–%97) | %88 (%67–%100) | %100 (%87–%100) | 390 (210–516) | %20 (%16–%24) |
| 27 | 3,94 (3,75–4) | %93 (%89–%97) | %88 (%67–%100) | %100 (%87–%100) | 364 (258–650) | %21 (%16–%25) |
| 28 | 3,94 (3,75–4) | %94 (%90–%97) | %94 (%67–%100) | %100 (%87–%100) | 462 (259–597) | %21 (%18–%25) |
| 29 | 4 (3,79–4) | %94 (%90–%98) | %88 (%78–%100) | %100 (%87–%100) | 437 (163–664) | %22 (%18–%24) |
| 30 | 4 (3,79–4) | %95 (%90–%98) | %89 (%86–%100) | %100 (%87–%100) | 480 (275–777) | %20 (%18–%26) |
| 31 | 4 (3,79–4) | %96 (%90–%98) | %94 (%86–%100) | %100 (%86–%100) | 533 (319–678) | %21 (%16–%25) |
| 32 | 4 (3,87–4) | %96 (%91–%98) | %95 (%79–%100) | %100 (%79–%100) | 571 (386–672) | %20 (%18–%23) |
| 33 | 4 (3,94–4) | %96 (%92–%98) | %100 (%86–%100) | %100 (%87–%100) | 617 (497–809) | %20 (%17–%24) |
| 34 | 4 (3,94–4) | %96 (%93–%99) | %100 (%87–%100) | %100 (%88–%100) | 613 (437–729) | %21 (%15–%24) |
| 35 | 4 (3,94–4) | %96 (%93–%99) | %100 (%87–%100) | %100 (%95–%100) | 587 (415–743) | %20 (%17–%25) |
| 36 | 4 (3,94–4) | %97 (%93–%99) | %100 (%86–%100) | %100 (%88–%100) | 604 (457–804) | %20 (%18–%24) |
| 37 | 4 (3,94–4) | %97 (%93–%99) | %100 (%87–%100) | %100 (%95–%100) | 635 (397–843) | %20 (%16–%24) |
| 38 | 4 (3,94–4) | %97 (%94–%99) | %100 (%88–%100) | %100 (%95–%100) | 593 (331–821) | %20 (%17–%23) |
| 39 | 4 (3,94–4) | %97 (%95–%100) | %100 (%89–%100) | %100 | 628 (429–827) | %21 (%17–%24) |
| 40 | 4 | %97 (%95–%100) | %100 (%89–%100) | %100 (%95–%100) | 648 (495–800) | %19 (%15–%23) |
| 41 | 4 | %98 (%95–%100) | %100 (%95–%100) | %100 | 599 (325–761) | %20 (%15–%23) |
| 42 | 4 | %98 (%95–%99) | %100 (%89–%100) | %100 (%89–%100) | 657 (475–844) | %20 (%16–%24) |
| 43 | 4 | %98 (%95–%99) | %100 (%89–%100) | %100 (%89–%100) | 681 (390–849) | %20 (%16–%26) |
| 44 | 4 | %98 (%95–%100) | %100 (%90–%100) | %100 (%90–%100) | 649 (374–895) | %19 (%16–%23) |
| 45 | 4 | %98 (%95–%100) | %100 | %100 | 628 (364–859) | %19 (%15–%25) |
| 46 | 4 | %98 (%96–%100) | %100 | %100 | 666 (448–902) | %20 (%17–%25) |
| 47 | 4 | %98 (%96–%100) | %100 | %100 | 729 (385–957) | %19 (%16–%25) |
| 48 | 4 | %98 (%96–%100) | %100 | %100 | 675 (469–1003) | %21 (%15–%23) |
| 49 | 4 | %98 (%96–%100) | %100 | %100 | 766 (623–1094) | %20 (%16–%24) |
| 50 | 4 | %98 (%96–%100) | %100 (%94–%100) | %100 (%94–%100) | 754 (469–1045) | %19 (%16–%23) |
| 51 | 4 | %98 (%96–%100) | %100 (%94–%100) | %100 (%94–%100) | 781 (578–1032) | %19 (%16–%22) |
| 52 | 4 | %98 (%96–%100) | %100 (%94–%100) | %100 (%94–%100) | 759 (560–1030) | %20 (%16–%22) |
| 53 | 4 | %99 (%97–%100) | %100 (%95–%100) | %100 (%95–%100) | 773 (568–1045) | %19 (%16–%22) |
| 54 | 4 | %99 (%97–%100) | %100 (%90–%100) | %100 (%90–%100) | 809 (490–1002) | %19 (%16–%22) |
| 55 | 4 | %99 (%97–%100) | %100 (%90–%100) | %100 (%90–%100) | 754 (644–1010) | %19 (%17–%22) |
| 56 | 4 | %99 (%97–%100) | %100 (%90–%100) | %100 (%90–%100) | 791 (623–1008) | %17 (%16–%21) |
| 57 | 4 | %99 (%97–%100) | %100 (%90–%100) | %100 (%90–%100) | 780 (548–942) | %18 (%16–%20) |
| 58 | 4 | %99 (%97–%100) | %100 (%95–%100) | %100 (%95–%100) | 795 (580–908) | %17 (%15–%20) |
| 59 | 4 | %99 (%97–%100) | %100 | %100 | 750 (509–1121) | %19 (%16–%20) |
| 60 | 4 | %99 (%97–%100) | %100 | %100 | 781 (653–897) | %18 (%16–%22) |

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
| 11 | 0 | 6 (5–6) |
| 12 | 0 | 6 (5–7) |
| 13 | 0 | 7 (6–8) |
| 14 | 0 | 7 (6,5–8) |
| 15 | 0 | 7,5 (7–9) |
| 16 | 0 | 8 (8–9) |
| 17 | 0 | 9 (8–9,5) |
| 18 | 0 | 9 (8–10) |
| 19 | 0 | 9 (9–10) |
| 20 | 0 | 9 (9–11) |
| 21 | 0 | 9 (9–11) |
| 22 | 0 | 10 (9–11) |
| 23 | 0 | 10 (9–11,5) |
| 24 | 0 | 10 (9–12) |
| 25 | 0 | 10 (10–12,5) |
| 26 | 0 | 10,5 (10–13) |
| 27 | 0 | 11 (10–13,5) |
| 28 | 0 | 11 (10–14) |
| 29 | 0 | 11 (10–15) |
| 30 | 0 | 11 (10–15) |
| 31 | 0 | 12 (10–15,5) |
| 32 | 0 | 12 (10–16) |
| 33 | 0 | 12 (10–16) |
| 34 | 0 | 13 (10–16) |
| 35 | 0 | 13 (10,5–16) |
| 36 | 0 | 13 (11–16) |
| 37 | 0 | 13 (11–16) |
| 38 | 0 | 13 (11,5–16) |
| 39 | 0 | 14 (12–16,5) |
| 40 | 0 | 14 (12–16,5) |
| 41 | 0 | 14 (12–17) |
| 42 | 0 (0–0,5) | 15 (12–17) |
| 43 | 0 | 15 (12–18) |
| 44 | 0 | 15 (12–18) |
| 45 | 0 | 16 (11,5–18) |
| 46 | 0 | 15,5 (12–18,5) |
| 47 | 0 | 16 (12–18,5) |
| 48 | 0 | 15,5 (12–20) |
| 49 | 0 | 16 (12–19,5) |
| 50 | 0 | 16 (12–20,5) |
| 51 | 0 | 16,5 (12–20,5) |
| 52 | 0 | 16,5 (12–20,5) |
| 53 | 0 | 17 (12–21,5) |
| 54 | 0 | 17 (12–22) |
| 55 | 0 | 18 (12–22) |
| 56 | 0 | 18 (12–22) |
| 57 | 0 | 17,5 (12,5–22) |
| 58 | 0 | 18 (12,5–21,5) |
| 59 | 0 | 18 (12,5–22) |
| 60 | 0 | 18,5 (12,5–23) |

### Olaylar

| Yıl | Olay | Büyük olay |
|---|---|---|
| 1 | 37 (28–43,5) | 14 (12–19) |
| 2 | 60,5 (44,5–68,5) | 20,5 (14,5–23,5) |
| 3 | 34 (26,5–48) | 6 (3,5–11) |
| 4 | 47,5 (41,5–63) | 17 (14–23) |
| 5 | 57,5 (44–69,5) | 22 (17–31,5) |
| 6 | 77 (58,5–86,5) | 30 (23–38) |
| 7 | 102 (80–120) | 38 (32–50) |
| 8 | 114 (92–136) | 45 (38,5–56) |
| 9 | 131 (106–166) | 51 (43–64,5) |
| 10 | 157 (120–190) | 58,5 (47–70) |
| 11 | 182 (132–234) | 68 (48–97,5) |
| 12 | 183 (139–244) | 68,5 (51–87,5) |
| 13 | 205 (158–260) | 78,5 (56,5–98,5) |
| 14 | 210 (168–244) | 70,5 (56,5–94) |
| 15 | 186 (162–239) | 65,5 (54–93,5) |
| 16 | 200 (126–266) | 69,5 (33,5–87,5) |
| 17 | 185 (132–246) | 64,5 (37,5–80,5) |
| 18 | 183 (142–248) | 62 (52,5–82) |
| 19 | 162 (130–224) | 59,5 (38,5–72) |
| 20 | 166 (140–212) | 57 (41–77,5) |
| 21 | 179 (137–220) | 62 (39–70,5) |
| 22 | 180 (132–224) | 60 (41–73) |
| 23 | 163 (130–234) | 54,5 (39–81,5) |
| 24 | 174 (132–207) | 52 (42,5–74) |
| 25 | 166 (112–208) | 55 (32,5–70,5) |
| 26 | 156 (124–213) | 54 (36,5–68,5) |
| 27 | 160 (106–201) | 46,5 (29,5–72,5) |
| 28 | 156 (110–192) | 44 (24,5–57) |
| 29 | 158 (103–208) | 55,5 (29,5–78,5) |
| 30 | 170 (110–204) | 49,5 (33,5–63,5) |
| 31 | 154 (114–194) | 48 (32,5–66) |
| 32 | 161 (114–212) | 46 (29,5–82,5) |
| 33 | 148 (120–196) | 46 (31,5–65) |
| 34 | 170 (124–210) | 51 (34–67,5) |
| 35 | 153 (110–212) | 41 (28,5–64) |
| 36 | 164 (113–194) | 43,5 (27,5–67) |
| 37 | 175 (114–209) | 48,5 (32–68,5) |
| 38 | 160 (93–230) | 51 (21–71) |
| 39 | 176 (103–204) | 48,5 (28–75,5) |
| 40 | 152 (92,5–207) | 45,5 (21,5–75) |
| 41 | 170 (118–236) | 54 (38,5–79,5) |
| 42 | 165 (129–202) | 46 (33,5–61,5) |
| 43 | 176 (122–234) | 53 (31,5–75) |
| 44 | 183 (100–248) | 54,5 (25,5–86,5) |
| 45 | 174 (126–280) | 55,5 (31,5–83,5) |
| 46 | 166 (112–226) | 54 (30,5–73,5) |
| 47 | 168 (107–245) | 50 (33,5–83,5) |
| 48 | 156 (113–246) | 46 (32–80,5) |
| 49 | 172 (126–270) | 54,5 (34–75) |
| 50 | 169 (110–222) | 49 (31–73,5) |
| 51 | 173 (108–238) | 53 (29,5–89) |
| 52 | 160 (136–213) | 50 (33–68,5) |
| 53 | 175 (118–220) | 56 (32,5–79) |
| 54 | 184 (121–246) | 61 (32,5–71) |
| 55 | 178 (116–248) | 61 (41,5–86) |
| 56 | 167 (119–224) | 52 (29,5–68,5) |
| 57 | 156 (98–252) | 42,5 (26,5–77) |
| 58 | 172 (112–276) | 52 (36–81) |
| 59 | 180 (118–280) | 60 (38–80,5) |
| 60 | 162 (128–268) | 53 (39–94,5) |

### Savaş (1/2)

| Yıl | Muharebe | Başlayan savaş | Süren savaş (yıl sonu) | Yıl içinde süren savaş | Yağma akını (medeniyet) | Tarihî hak savaşı |
|---|---|---|---|---|---|---|
| 1 | 0 | 0 | 0 | 0 | 0 | 0 |
| 2 | 0 | 0 | 0 | 0 | 0 | 0 |
| 3 | 0 (0–1) | 0 | 0 | 0 | 0 | 0 |
| 4 | 4 (3–5) | 0 | 0 | 0 | 0 | 0 |
| 5 | 4,5 (2,5–6) | 0 | 0 | 0 | 0 | 0 |
| 6 | 5,5 (4–7) | 0 | 0 | 0 | 0 | 0 |
| 7 | 6 (3,5–7) | 0 | 0 | 0 | 0 | 0 |
| 8 | 6,5 (4–8) | 0 (0–0,5) | 0 (0–0,5) | 0 (0–0,5) | 0 (0–1) | 0 |
| 9 | 7 (5–10,5) | 0 (0–1) | 0 | 0 (0–1,5) | 0 (0–1) | 0 |
| 10 | 9 (6–10) | 0 (0–1) | 0 (0–0,5) | 0 (0–1,5) | 0 (0–1) | 0 |
| 11 | 8 (6–12,5) | 1 (0–2) | 0 (0–1) | 1 (0–2) | 0 (0–1,5) | 0 (0–0,5) |
| 12 | 10,5 (6–12) | 0 (0–2) | 0 (0–1) | 1 (0–2,5) | 1 (0–2) | 0 |
| 13 | 9,5 (6,5–13) | 1 (0–2) | 0 (0–1,5) | 1 (0–2) | 0,5 (0–2) | 0 (0–1) |
| 14 | 8,5 (4,5–13,5) | 0,5 (0–1,5) | 0 (0–0,5) | 1 (0–2) | 1 (0–2) | 0 (0–0,5) |
| 15 | 8 (5–10,5) | 1 (0–1,5) | 0 (0–1) | 1 (0–1,5) | 1 (0–2) | 0 |
| 16 | 7 (3,5–11) | 1 (0–2) | 0 (0–0,5) | 1 (0–2) | 1 (0–2) | 0 |
| 17 | 6 (3,5–9) | 2 (0–2,5) | 0 (0–1) | 2 (0–2,5) | 1 (0–2) | 0 (0–1) |
| 18 | 8 (5,5–13) | 1 (0–2) | 0 (0–1) | 1 (0–2,5) | 1 (0–2,5) | 0 (0–1) |
| 19 | 6 (4,5–9,5) | 1 (0–2) | 0 (0–1) | 1 (0–2,5) | 1 (0–3) | 0 (0–1) |
| 20 | 7 (4,5–9,5) | 1 (0–3,5) | 0 (0–1,5) | 1,5 (0–3,5) | 1 (0–3) | 0 (0–0,5) |
| 21 | 8,5 (4,5–11,5) | 0 (0–3) | 0 (0–2) | 1,5 (0–4) | 1,5 (0–4) | 0 (0–0,5) |
| 22 | 7,5 (5–11,5) | 1 (0–2) | 0 (0–1) | 2 (0–2,5) | 1 (0–3) | 0 (0–1) |
| 23 | 7,5 (5,5–11) | 0,5 (0–4) | 0 | 1,5 (0–4) | 1,5 (0–3,5) | 0 (0–1) |
| 24 | 8,5 (4,5–10) | 0,5 (0–2,5) | 0 (0–0,5) | 1 (0–3) | 1 (0–5) | 0 |
| 25 | 8 (4,5–10,5) | 1 (0–3) | 0 (0–1) | 1 (0–3) | 1 (0–4) | 0 (0–1) |
| 26 | 8 (6–12,5) | 1,5 (0–3,5) | 0 (0–2) | 1,5 (0–4) | 1 (0–4) | 0 (0–0,5) |
| 27 | 8,5 (5–11) | 1 (0–2) | 0 (0–1) | 1 (0–3,5) | 2 (0–4,5) | 0 (0–0,5) |
| 28 | 8 (3–13) | 0,5 (0–3) | 0 (0–2) | 1 (0–3) | 1 (0–3) | 0 (0–0,5) |
| 29 | 8 (6–13) | 1 (0–3,5) | 0 (0–1) | 2 (0–4,5) | 1 (0–3) | 0 (0–1) |
| 30 | 9 (6–12,5) | 1 (0–2) | 0 (0–0,5) | 1 (0–3) | 1,5 (0–4,5) | 0 |
| 31 | 9 (5,5–11,5) | 1,5 (0–4,5) | 0 (0–3) | 2 (0–4,5) | 1 (0–3,5) | 0 (0–0,5) |
| 32 | 8 (6,5–12) | 1 (0–2) | 0 (0–1) | 1 (0–4) | 1 (0–3,5) | 0 |
| 33 | 8,5 (6–14,5) | 1 (0–2,5) | 0 (0–1,5) | 1 (0–3) | 2 (0–4) | 0 (0–0,5) |
| 34 | 9,5 (4,5–14) | 1,5 (0–4) | 0 (0–3) | 2 (0–4) | 2 (0–4,5) | 0 (0–1) |
| 35 | 10 (5,5–13) | 0 (0–3) | 0 (0–1) | 1,5 (0–4,5) | 1,5 (0–4) | 0 (0–0,5) |
| 36 | 9,5 (4–11) | 1 (0–2,5) | 0 (0–1) | 1,5 (0–3) | 2 (0–3) | 0 (0–0,5) |
| 37 | 9 (5,5–13,5) | 1,5 (0–3,5) | 0 (0–1) | 2 (0–4) | 2 (0–3,5) | 0 (0–0,5) |
| 38 | 8,5 (4,5–11,5) | 1 (0–2,5) | 0,5 (0–2) | 1 (0–3) | 1,5 (0–3) | 0 (0–1) |
| 39 | 8,5 (5–13) | 1,5 (0–3) | 0 (0–1,5) | 2 (0–4) | 2 (0–4,5) | 0 (0–1) |
| 40 | 8,5 (4–12) | 1 (0–2,5) | 0 (0–1) | 1,5 (0–3) | 1 (0–3,5) | 0 |
| 41 | 10 (6–13,5) | 1 (0–4) | 0 (0–1) | 1 (0–4) | 2 (0–3,5) | 0 |
| 42 | 9 (6,5–12) | 1 (0–3) | 0 (0–1) | 1,5 (0–3) | 1,5 (0–4) | 0 (0–1) |
| 43 | 10 (7,5–14,5) | 1 (0–4) | 0 (0–1,5) | 2 (0–4,5) | 2,5 (0–3) | 0 (0–2) |
| 44 | 11 (5,5–13,5) | 1 (0–3) | 0 (0–1) | 1 (0–3) | 2 (0–4,5) | 0 (0–1) |
| 45 | 10,5 (7–13) | 1,5 (0–4,5) | 0 (0–2) | 2 (0–4,5) | 1,5 (0–5) | 0 (0–1) |
| 46 | 10 (7,5–13) | 1 (0–3,5) | 0 (0–1) | 1 (0–4,5) | 1,5 (0–3) | 0 |
| 47 | 9 (5,5–15,5) | 0 (0–3) | 0 (0–1) | 1 (0–3,5) | 2 (0–4) | 0 (0–0,5) |
| 48 | 9 (6,5–15) | 1 (0–4) | 0 (0–1,5) | 1 (0–4,5) | 1 (0–3) | 0 (0–1) |
| 49 | 10,5 (6,5–15) | 0,5 (0–3,5) | 0 (0–1) | 2 (0–3,5) | 2 (0–4,5) | 0 |
| 50 | 9,5 (5,5–11,5) | 1 (0–2,5) | 0 (0–1,5) | 1 (0–3,5) | 1,5 (0–4) | 0 (0–1) |
| 51 | 9,5 (7–14,5) | 1,5 (0–4,5) | 0,5 (0–2) | 2 (0–5) | 1 (0–3,5) | 0 (0–1) |
| 52 | 11 (6–15,5) | 0,5 (0–3) | 0 (0–1) | 1,5 (0–3) | 1 (0–3,5) | 0 |
| 53 | 7,5 (5–14,5) | 1 (0–3) | 0 (0–1,5) | 2 (0–3) | 0,5 (0–5) | 0 (0–1) |
| 54 | 9 (7–12) | 0 (0–4,5) | 0 (0–2) | 1,5 (0–4,5) | 1,5 (0–4,5) | 0 (0–1) |
| 55 | 10,5 (6–15) | 1 (0–3) | 0 (0–2) | 2 (0–3,5) | 1 (0–4) | 0 (0–1,5) |
| 56 | 11 (5,5–14,5) | 0,5 (0–2,5) | 0 (0–1) | 2 (0–3) | 1 (0–4) | 0 (0–0,5) |
| 57 | 8,5 (5–14) | 1,5 (0–3,5) | 0 (0–2) | 2 (0–3,5) | 1,5 (0–5) | 0 (0–1) |
| 58 | 11 (4,5–11,5) | 1 (0–3,5) | 0 (0–1,5) | 1,5 (0–4,5) | 1 (0–4,5) | 0 |
| 59 | 11 (5,5–15) | 0 (0–4) | 0 (0–2,5) | 1 (0–4,5) | 2 (0–5) | 0 (0–0,5) |
| 60 | 10,5 (7–13,5) | 2 (0–3,5) | 0 (0–1,5) | 2,5 (0–4) | 1 (0–3) | 0 (0–1) |

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
| 10 | 0 | 0 | 0 | 0 (0–0,5) |
| 11 | 0 | 0 (0–0,5) | 0 | 0 (0–0,5) |
| 12 | 0 | 0 | 0 | 0 (0–1) |
| 13 | 0 | 0 | 0 | 0 (0–1) |
| 14 | 0 | 0 | 0 | 0 (0–1) |
| 15 | 0 | 0 | 0 | 0 (0–1) |
| 16 | 0 | 0 | 0 | 0 (0–1,5) |
| 17 | 0 (0–1) | 0 | 0 | 0 (0–1,5) |
| 18 | 0 | 0 | 0 | 1 (0–1,5) |
| 19 | 0 (0–0,5) | 0 | 0 | 1 (0–1,5) |
| 20 | 0 (0–0,5) | 0 | 0 | 1 (0–1,5) |
| 21 | 0 (0–1) | 0 | 0 | 1 (0–1,5) |
| 22 | 0 | 0 | 0 | 1 (0–2) |
| 23 | 0 (0–1) | 0 (0–0,5) | 0 | 1 (0–2) |
| 24 | 0 | 0 | 0 (0–0,5) | 1 (0–2) |
| 25 | 0 | 0 | 0 | 1 (0–2) |
| 26 | 0 | 0 | 0 | 1 (0–2) |
| 27 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 28 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 29 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 30 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 31 | 0 (0–1,5) | 0 (0–0,5) | 0 | 1 (0–2) |
| 32 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 33 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 34 | 0 (0–1) | 0 | 0 | 1 (0,5–2) |
| 35 | 0 (0–1) | 0 | 0 | 1 (0,5–2) |
| 36 | 0 (0–0,5) | 0 | 0 (0–0,5) | 1 (0,5–2) |
| 37 | 0 (0–1) | 0 | 0 | 1 (0,5–2) |
| 38 | 0 (0–0,5) | 0 | 0 | 1 (0,5–2) |
| 39 | 0 | 0 | 0 | 1 (0,5–2) |
| 40 | 0 (0–1) | 0 | 0 | 1 (0,5–2) |
| 41 | 0 (0–1) | 0 (0–1) | 0 | 1 (0,5–2) |
| 42 | 0 (0–1) | 0 | 0 | 1 (0,5–2) |
| 43 | 0 (0–1) | 0 | 0 | 2 (0,5–2) |
| 44 | 0 (0–0,5) | 0 | 0 (0–0,5) | 1,5 (0–2) |
| 45 | 0 (0–1) | 0 | 0 (0–0,5) | 1 (0–2) |
| 46 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 47 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 48 | 0 (0–1) | 0 | 0 | 1 (0,5–2) |
| 49 | 0 (0–1) | 0 | 0 | 1 (0,5–2) |
| 50 | 0 (0–1) | 0 | 0 (0–0,5) | 1 (1–2) |
| 51 | 0 (0–1) | 0 | 0 | 1 (0,5–2) |
| 52 | 0 (0–0,5) | 0 | 0 | 1 (0,5–2,5) |
| 53 | 0 | 0 | 0 | 1 (0–2,5) |
| 54 | 0 (0–1) | 0 | 0 | 1 (0–2,5) |
| 55 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 56 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 57 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 58 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 59 | 0 | 0 | 0 | 1 (0–2) |
| 60 | 0 (0–1) | 0 | 0 | 1 (0–2) |

### Canavarlar (1/2)

| Yıl | Yaşayan kamp (yıl sonu) | Yaşayan kamp (yıl ort.) | Doğan kamp | Temizlenen kamp | Canavar baskını | Yaşayan trol ini (yıl sonu) |
|---|---|---|---|---|---|---|
| 1 | 3 | 3 | 0 | 0 | 0 | 0 |
| 2 | 4 (3–5) | 3,1 (3–3,48) | 1 (0–2) | 0 | 0 | 0 |
| 3 | 5 (4–6,5) | 4,3 (3,4–5,59) | 1 (0–2,5) | 0 | 0 (0–0,5) | 0 |
| 4 | 7 (4,5–8,5) | 6,13 (4,08–7,23) | 2 (1–3) | 0 | 3,5 (2,5–5) | 0 |
| 5 | 7 (6,5–10) | 7 (6–9,71) | 1 (0–2,5) | 0 | 4 (2,5–6) | 0 |
| 6 | 9,5 (7–10,5) | 8,73 (6,88–10) | 1 (0–3) | 0 | 5,5 (3,5–6) | 0 |
| 7 | 10 (8,5–10,5) | 9,94 (8,17–10,5) | 0 (0–1,5) | 0 | 5 (3–7) | 0 |
| 8 | 10 (9–11) | 10 (8,7–10,9) | 0 (0–1) | 0 | 6 (3–8) | 0 |
| 9 | 10 (9,5–11) | 10 (9,39–11) | 0 (0–1) | 0 | 5 (4,5–8,5) | 0 |
| 10 | 10 (10–11,5) | 10,3 (9,5–11,3) | 0 (0–1) | 0 (0–0,5) | 7 (5–9) | 0 |
| 11 | 10 (8–12) | 10,2 (9,37–11,8) | 0,5 (0–1,5) | 0,5 (0–3) | 5,5 (5–8) | 0 |
| 12 | 9 (7–11) | 9,75 (7,78–11,4) | 0,5 (0–2) | 1,5 (0–3) | 6 (5–8,5) | 0 |
| 13 | 8 (6,5–11) | 8,29 (6,93–10,9) | 1 (0–3) | 2 (0–5) | 5 (3–7) | 0 |
| 14 | 6,5 (4,5–10,5) | 7,82 (5,1–10,7) | 1 (0–2,5) | 2,5 (0–5) | 4,5 (1,5–6,5) | 0 |
| 15 | 7 (5–10) | 6,51 (4,8–9,79) | 2 (1–3,5) | 2 (0,5–3) | 4 (1–6,5) | 0 (0–1) |
| 16 | 6 (4–9) | 6,66 (5,02–9,5) | 1,5 (0–3) | 2 (0–4,5) | 3 (0,5–5) | 0 (0–1) |
| 17 | 6 (5–8,5) | 6,17 (4,93–8,68) | 1 (0–3) | 1 (0–2,5) | 3 (0,5–5) | 0 (0–1) |
| 18 | 7 (5–8) | 6,81 (5,04–8,42) | 3 (1,5–4) | 3 (0,5–4) | 3 (1–6) | 0 (0–1,5) |
| 19 | 6,5 (5–9) | 6,78 (4,54–8,88) | 1 (0,5–2,5) | 1,5 (0–3) | 3 (1–4,5) | 1 (0–1,5) |
| 20 | 7 (5–9) | 6,97 (5,12–8,84) | 2 (1–4) | 2 (0,5–3) | 3 (1–4) | 1 (0–1,5) |
| 21 | 7 (6–8,5) | 6,83 (5,91–8,04) | 2 (0,5–4) | 1,5 (1–4,5) | 2,5 (1,5–6,5) | 1 (0–2) |
| 22 | 7 (5–7,5) | 6,9 (5,85–8,2) | 1 (0–3) | 1,5 (0,5–3,5) | 3 (2–5) | 0,5 (0–2) |
| 23 | 7 (6–8) | 7,05 (5,92–7,76) | 2 (1–3) | 1 (0–2,5) | 3 (2–4,5) | 1 (0–2) |
| 24 | 8 (7–9) | 7,84 (6,64–8,89) | 2 (1–4) | 2 (0–3,5) | 3 (2–5,5) | 0 (0–2) |
| 25 | 8 (6,5–10) | 7,75 (6,71–9,54) | 2 (1–3) | 2,5 (0–4) | 3 (1,5–4) | 1 (0–2) |
| 26 | 8 (7–9) | 7,83 (6,46–9,53) | 1,5 (0–3) | 1,5 (1–3) | 3,5 (2–6,5) | 1 (0–2) |
| 27 | 8 (6–10) | 7,95 (6,74–9,69) | 1,5 (1–3) | 1,5 (0,5–3) | 3 (2–5) | 1 (0–2) |
| 28 | 8 (6,5–10) | 7,79 (6,14–9,55) | 1,5 (0–3) | 2 (0–3) | 3,5 (1,5–6) | 1 (1–2) |
| 29 | 8 (7–10,5) | 7,91 (6,83–9,87) | 2 (0,5–3,5) | 1 (1–2,5) | 3 (2–4,5) | 1 (0–2) |
| 30 | 9 (6,5–10,5) | 8,42 (7,22–10,5) | 2 (1–3,5) | 2 (1–3,5) | 3,5 (2,5–5) | 1 (0–2) |
| 31 | 9 (7–10) | 9,18 (7,25–10,4) | 2 (0,5–3) | 2 (1–3) | 4 (2,5–5) | 1 (0–2) |
| 32 | 9 (7,5–10,5) | 9,02 (7,74–10) | 1,5 (1–3,5) | 2 (1–3) | 3 (1,5–6) | 1 (0–2) |
| 33 | 9 (7,5–10) | 9,09 (7,58–9,94) | 2 (0,5–3,5) | 1,5 (1–3) | 4 (2–6) | 1 (0–2) |
| 34 | 9 (8–10) | 8,9 (7,88–10) | 1 (0,5–3) | 1 (1–3) | 4 (1,5–6,5) | 1 (0–2,5) |
| 35 | 9 (7–10) | 8,48 (7,32–9,75) | 2 (0,5–2,5) | 2 (1–3,5) | 4 (2–5,5) | 1 (0–2,5) |
| 36 | 9 (6,5–10,5) | 8,94 (6,85–9,91) | 2 (1–3,5) | 2 (0,5–3,5) | 3,5 (1,5–6) | 1 (0–2) |
| 37 | 9 (7–10,5) | 8,99 (7,48–9,98) | 2 (1–3) | 2 (0,5–4) | 3,5 (1,5–5,5) | 1 (0–2) |
| 38 | 9 (7–10,5) | 8,99 (7,13–10,5) | 2 (0,5–3,5) | 2 (0–3) | 3 (2–4,5) | 1 (0–2) |
| 39 | 9,5 (6,5–11) | 9,42 (6,33–10,5) | 3 (1–4) | 2 (0,5–3,5) | 3 (2–6) | 1 (0–2,5) |
| 40 | 10 (7,5–11) | 9,61 (8,02–10,7) | 2,5 (0,5–4) | 1 (0–4) | 3 (0,5–6) | 1 (0–3) |
| 41 | 9 (8–11) | 9,41 (7,65–11,1) | 1 (0,5–4,5) | 2 (1–3,5) | 4 (1–8,5) | 1 (0–2,5) |
| 42 | 9,5 (8–12) | 9,7 (7,35–11,3) | 2 (1–4) | 2 (0–4) | 4 (2,5–6,5) | 1 (0–2,5) |
| 43 | 9,5 (8–12) | 9,38 (7,4–11,6) | 2 (1–3,5) | 2 (1,5–3) | 4,5 (2–7) | 1 (0–3) |
| 44 | 9 (7–12) | 9,43 (7,03–12,2) | 2 (0,5–3,5) | 3 (0,5–3,5) | 4 (2–6) | 1 (0,5–3) |
| 45 | 9,5 (7–11) | 9,07 (6,47–11,5) | 2 (0,5–4,5) | 2 (1–4,5) | 3 (1–8) | 1,5 (0–3) |
| 46 | 9 (6,5–11) | 9,15 (6,67–10,8) | 2 (1–3,5) | 2,5 (0,5–3) | 4 (2,5–8) | 2 (1–3) |
| 47 | 9 (6,5–11,5) | 9,16 (6,73–11,3) | 2 (1–3) | 2 (1–3) | 3 (1,5–6,5) | 2 (0,5–3) |
| 48 | 9,5 (8–11,5) | 9,15 (7,15–10,9) | 2 (1–4) | 2 (1–4) | 4 (3–6,5) | 2 (0–3) |
| 49 | 9 (7,5–11) | 9,07 (7,04–11,3) | 2,5 (1,5–4,5) | 3 (2–4) | 3 (1,5–6,5) | 1 (0–2,5) |
| 50 | 9,5 (7–11,5) | 9,2 (7,2–11,9) | 2 (2–4) | 2 (1–3) | 3 (2–6) | 1,5 (0,5–3) |
| 51 | 10 (7–12,5) | 9,27 (7,22–12,3) | 3 (1–5) | 3 (1–4) | 4 (1,5–6) | 2 (1–2,5) |
| 52 | 8,5 (6,5–11,5) | 9,66 (7–11,6) | 2 (1–3) | 3 (1,5–5) | 3,5 (1–7) | 2 (0,5–2,5) |
| 53 | 9,5 (7,5–12) | 8,77 (7,13–11,2) | 2,5 (1–3,5) | 2 (1–3) | 4 (2–5) | 2 (1–3) |
| 54 | 9 (7,5–12) | 8,76 (7,63–12,1) | 2 (1–4,5) | 2,5 (1–4,5) | 3,5 (2–5,5) | 2 (1–3) |
| 55 | 9,5 (6–12,5) | 9,59 (6,93–12,4) | 2 (1–4,5) | 3 (1,5–5) | 3 (2–6,5) | 2 (0,5–3) |
| 56 | 10 (6–11,5) | 9,22 (6,57–11,9) | 2 (2–5) | 3 (1–4) | 3 (1,5–7) | 2 (1–3) |
| 57 | 10 (5,5–13) | 10,2 (6,13–12,3) | 3 (1–3) | 2 (1–4) | 3,5 (1,5–5,5) | 2 (1–3) |
| 58 | 10,5 (6,5–13) | 9,66 (6,62–12,7) | 2 (1–4) | 2 (1–3) | 3 (1,5–6,5) | 2 (0–3) |
| 59 | 9,5 (5,5–12,5) | 9,52 (6,7–13,1) | 2 (1–2) | 3 (0,5–4,5) | 3,5 (2–9) | 2 (1–3) |
| 60 | 10 (6–14,5) | 9,38 (6,35–13,5) | 3 (2–5,5) | 3 (1–4,5) | 3,5 (1,5–9) | 2 (1–3) |

### Canavarlar (2/2)

| Yıl | Yaşayan ejderha (yıl sonu) | Ejderha akını | Kriz (anlatıcı) | Rahatlama dönemi (anlatıcı) |
|---|---|---|---|---|
| 1 | 0 | 0 | 0 | 0 |
| 2 | 0 | 0 | 0 | 0 |
| 3 | 0 | 0 | 0 | 0 |
| 4 | 0 | 0 | 0 | 0 |
| 5 | 0 | 0 | 0 | 1 |
| 6 | 0 | 0 | 0 | 0 |
| 7 | 0 | 0 | 0 | 1 (0–1) |
| 8 | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) |
| 9 | 0 | 0 | 0 | 0 (0–1) |
| 10 | 0 | 0 | 0 | 0 |
| 11 | 0 | 0 | 0 | 0 (0–1) |
| 12 | 0 | 0 | 0 | 0 |
| 13 | 0 | 0 | 0 | 0 |
| 14 | 0 | 0 | 0 (0–0,5) | 0 |
| 15 | 0 | 0 | 0 (0–1) | 0 (0–1) |
| 16 | 0 | 0 | 0 (0–1) | 0 |
| 17 | 0 | 0 | 0 (0–1) | 0 |
| 18 | 0 (0–1) | 0 (0–1) | 0,5 (0–1) | 0 |
| 19 | 0 (0–1) | 0 (0–1) | 0 (0–1) | 0 (0–0,5) |
| 20 | 0,5 (0–1) | 0 (0–1) | 0 (0–1) | 0 |
| 21 | 1 (0–1) | 0,5 (0–1,5) | 0 (0–1) | 0 |
| 22 | 1 | 1 (1–2) | 0 (0–1) | 0 (0–1) |
| 23 | 1 | 1 (1–2) | 0 (0–0,5) | 0 (0–0,5) |
| 24 | 1 | 1 (1–1,5) | 0 (0–1) | 0 |
| 25 | 1 | 1 (0–2) | 0 (0–1) | 0 |
| 26 | 1 | 1 (0–2) | 0 (0–1) | 0 |
| 27 | 1 | 1 (0–2) | 0,5 (0–1) | 0 |
| 28 | 1 | 1 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 29 | 1 | 1 (0–1) | 0 (0–1) | 0 |
| 30 | 1 | 1 (0–1,5) | 0 (0–1) | 0 |
| 31 | 1 | 1 (1–2) | 0 (0–1) | 0 (0–1) |
| 32 | 1 | 1 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 33 | 1 | 1 (0–1) | 0 (0–1) | 0 |
| 34 | 1 | 1 (0–1) | 0 (0–1) | 0 |
| 35 | 1 | 1 (0–1,5) | 0 (0–1) | 0 (0–0,5) |
| 36 | 1 | 1 (0,5–1,5) | 0 (0–1) | 0 |
| 37 | 1 (0–1) | 1 (0–1,5) | 0 (0–1) | 0 (0–1) |
| 38 | 1 (0–1) | 1 (0–1) | 0 (0–1) | 0 |
| 39 | 1 (0–1) | 0 (0–1,5) | 0 (0–1) | 0 (0–1) |
| 40 | 1 (0–1) | 1 (0–1) | 0 (0–1) | 0 |
| 41 | 1 (0–1) | 1 (0–1) | 0 (0–1) | 0 (0–0,5) |
| 42 | 1 (0–1) | 0,5 (0–1,5) | 0 (0–1) | 0 (0–0,5) |
| 43 | 1 (0–1) | 1 (0–1) | 0 (0–1) | 0 |
| 44 | 1 (0–1) | 0,5 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 45 | 1 (0–1) | 0,5 (0–1) | 0 (0–1) | 0 (0–0,5) |
| 46 | 1 (0–1) | 1 (0–2) | 0,5 (0–1) | 0 (0–0,5) |
| 47 | 1 (0–1) | 0 (0–1) | 0 (0–1) | 0 (0–1) |
| 48 | 1 (0–1) | 0,5 (0–1) | 0 (0–1) | 0 (0–0,5) |
| 49 | 1 (0–1) | 0 (0–1) | 0 (0–1) | 0 (0–0,5) |
| 50 | 1 (0–1) | 0 (0–1) | 0 (0–1) | 0 |
| 51 | 1 (0–1) | 0 (0–1) | 0 (0–1) | 0 |
| 52 | 1 (0–1) | 0 (0–1) | 0 (0–1) | 0 |
| 53 | 1 (0–1) | 0 (0–1) | 1 (0–1) | 0 |
| 54 | 1 (0–1) | 0,5 (0–1) | 0 (0–0,5) | 0 |
| 55 | 1 (0–1) | 0 (0–1) | 0 (0–1) | 0 |
| 56 | 1 (0–1) | 0 (0–1) | 0,5 (0–1) | 0 |
| 57 | 1 (0–1) | 0 (0–1) | 0,5 (0–1) | 0 |
| 58 | 1 (0–1) | 0 (0–1) | 1 (0–1) | 0 |
| 59 | 0,5 (0–1) | 0 (0–1) | 0 (0–1) | 0 (0–1) |
| 60 | 0,5 (0–1) | 0 (0–1) | 0 (0–1) | 0 (0–1) |

### Kahramanlar (1/2)

| Yıl | Doğan kahraman | Ölen kahraman | Emekli olan kahraman | Diyarı terk eden kahraman | Ölümden dönen kahraman | Efsane olan kahraman |
|---|---|---|---|---|---|---|
| 1 | 0 (0–0,5) | 0 | 0 | 0 | 0 | 0 |
| 2 | 1 (0–2) | 0 | 0 | 0 | 0 | 0 |
| 3 | 1 (0–2) | 0 | 0 | 0 | 0 | 0 |
| 4 | 0 (0–2) | 0 (0–1) | 0 | 0 (0–0,5) | 0 | 0 |
| 5 | 1 (0–2) | 0 (0–1) | 0 | 0 (0–0,5) | 0 | 0 |
| 6 | 0,5 (0–1,5) | 0 | 0 | 0 (0–0,5) | 0 | 0 |
| 7 | 1 (0–3) | 0 (0–1) | 0 | 0 (0–0,5) | 0 | 0 |
| 8 | 1,5 (0,5–4) | 0 (0–0,5) | 0 | 0 | 0 | 0 |
| 9 | 3 (1–7) | 0 (0–1) | 0 | 0 | 0 | 0 |
| 10 | 3 (1–5,5) | 0 (0–1,5) | 0 | 0 | 0 | 0 |
| 11 | 3 (1,5–5) | 0 (0–2) | 0 | 0 | 0 | 0 |
| 12 | 2 (1,5–3,5) | 0 (0–1,5) | 0 | 0 | 0 | 0 |
| 13 | 3,5 (2–7) | 1 (0–3,5) | 0 | 0 | 0 | 0 |
| 14 | 2,5 (2–4,5) | 0 (0–2,5) | 0 | 0 | 0 | 0 |
| 15 | 2,5 (1–5,5) | 0,5 (0–2,5) | 0 | 0 | 0 | 0 |
| 16 | 3 (1–4,5) | 0 (0–4,5) | 0 | 0 | 0 | 0 |
| 17 | 2,5 (1–4) | 1 (0–2) | 0 | 0 | 0 | 0 |
| 18 | 2,5 (1–4,5) | 1 (0–5) | 0 | 0 | 0 | 0 |
| 19 | 2 (0,5–4) | 0 (0–1) | 0 | 0 | 0 | 0 |
| 20 | 3 (1–5,5) | 0 (0–1,5) | 0 | 0 | 0 | 0 |
| 21 | 2 (1–4) | 0,5 (0–2) | 0 | 0 (0–0,5) | 0 | 0 |
| 22 | 3 (0,5–6,5) | 1 (0–4) | 0 | 0 | 0 | 0 |
| 23 | 3,5 (0–7) | 0 (0–1,5) | 0 | 0 (0–1) | 0 | 0 |
| 24 | 4,5 (1–7) | 0 (0–2) | 0 | 0 | 0 | 0 (0–0,5) |
| 25 | 3 (0,5–6) | 0 (0–2) | 0 | 0 | 0 | 0 |
| 26 | 3 (1,5–5) | 1 (0–3,5) | 0 | 0 (0–1) | 0 | 0 |
| 27 | 4 (1,5–7,5) | 0 (0–3) | 0 | 0 (0–1,5) | 0 | 0 |
| 28 | 3,5 (1,5–4,5) | 0,5 (0–3) | 0 | 0 (0–1,5) | 0 | 0 |
| 29 | 4 (2,5–6) | 1 (0–3) | 0 | 0 | 0 | 0 |
| 30 | 3 (1–6) | 0 (0–1,5) | 0 | 0,5 (0–1) | 0 | 0 |
| 31 | 3 (1–6) | 0 (0–5) | 0 | 0 (0–1) | 0 | 0 |
| 32 | 3 (1–6) | 0 (0–4,5) | 0 | 0 (0–1) | 0 | 0 (0–0,5) |
| 33 | 3 (2–5) | 0,5 (0–5) | 0 (0–1) | 0 (0–1,5) | 0 | 0 (0–0,5) |
| 34 | 3 (1,5–5) | 1,5 (0–4) | 0 | 1 (0–2) | 0 | 0 |
| 35 | 4 (1,5–7) | 0,5 (0–3,5) | 0 (0–1) | 0 (0–1) | 0 | 0 |
| 36 | 3 (1–5) | 1 (0–2,5) | 0 (0–0,5) | 0 (0–1,5) | 0 | 0 |
| 37 | 2,5 (1–4,5) | 0 (0–2,5) | 0 (0–0,5) | 0 (0–1) | 0 | 0 (0–0,5) |
| 38 | 4 (1–6) | 2 (0–5) | 0 (0–1) | 0 (0–0,5) | 0 | 0 |
| 39 | 2,5 (1–5,5) | 1 (0–5) | 0 (0–1,5) | 0 (0–1) | 0 | 0 |
| 40 | 3 (1,5–4) | 1 (0–3,5) | 0 (0–1) | 0 (0–1) | 0 | 0 (0–0,5) |
| 41 | 4 (1–5) | 1 (0–7,5) | 0 (0–1) | 0 (0–1) | 0 | 0 |
| 42 | 3,5 (1,5–5,5) | 0 (0–2,5) | 0 (0–1) | 0 (0–2) | 0 | 0 |
| 43 | 4 (0,5–6) | 1 (0–3,5) | 1 (0–1) | 0 (0–1) | 0 | 0 |
| 44 | 2,5 (0,5–5,5) | 1 (0–5,5) | 1 (0–2) | 0,5 (0–2) | 0 | 0 (0–1) |
| 45 | 2,5 (1,5–5) | 0,5 (0–4) | 0 (0–2) | 1 (0–1) | 0 | 0 (0–1) |
| 46 | 3,5 (0,5–6) | 0,5 (0–6) | 0,5 (0–1,5) | 0 (0–1,5) | 0 | 0 |
| 47 | 2,5 (1–5) | 1 (0–5) | 1 (0–2,5) | 0 (0–2) | 0 | 0 (0–1) |
| 48 | 3,5 (1–6) | 1 (0–4,5) | 1 (0–1) | 0 (0–1) | 0 | 0 (0–0,5) |
| 49 | 3 (0,5–6) | 1 (0–4,5) | 1 (0–2) | 0 (0–1,5) | 0 | 0 (0–1) |
| 50 | 3 (1,5–5) | 0,5 (0–3) | 1 (0–1) | 0 (0–1) | 0 | 0 |
| 51 | 2,5 (0,5–5) | 0,5 (0–3,5) | 1 (0,5–2,5) | 0,5 (0–2,5) | 0 | 0 |
| 52 | 2,5 (0,5–5) | 0,5 (0–3) | 0,5 (0–1) | 0 (0–1) | 0 | 0 (0–1) |
| 53 | 4 (1,5–5,5) | 1 (0–4) | 1 (0–3) | 0 (0–1,5) | 0 | 0 (0–0,5) |
| 54 | 2 (1,5–5,5) | 2 (0–4,5) | 0,5 (0–1) | 0 (0–1) | 0 | 0 (0–1) |
| 55 | 4 (0,5–6,5) | 1 (0–6,5) | 1 (0–2) | 0 (0–1) | 0 | 0 (0–1) |
| 56 | 3 (1–5) | 1 (0–3,5) | 1 (0–2) | 0 (0–1) | 0 | 0 (0–1) |
| 57 | 4 (1,5–7) | 0 (0–4) | 1 (0–2,5) | 0 (0–1,5) | 0 | 0 (0–0,5) |
| 58 | 3 (2–5) | 1 (0–5) | 1 (0–2) | 0 (0–2,5) | 0 | 0 (0–0,5) |
| 59 | 4 (1,5–5) | 1 (0–5) | 1 (0–3) | 1 (0–2) | 0 | 0 |
| 60 | 3 (2–5,5) | 2 (0–6) | 1 (0–2) | 0 (0–1) | 0 | 0 |

### Kahramanlar (2/2)

| Yıl | Yaşayan kahraman (yıl sonu) | Doğuş seviyesi (ort.) | Ölüm seviyesi (ort.) | Yaşayan kahraman seviyesi (ort.) | En yüksek seviye (şimdiye dek) |
|---|---|---|---|---|---|
| 1 | 0 (0–0,5) | 2 | – | 2 | 0 (0–1) |
| 2 | 1 (0–2) | 1 (1–2) | – | 1,25 (1–2) | 1 (0–2) |
| 3 | 2 (1–3) | 1 (1–2) | 2 | 1,33 (1–2) | 2 (1–2) |
| 4 | 2 (0–4,5) | 1 (1–1,7) | 1 (1–1,27) | 1,25 (1–1,95) | 2 (1–2) |
| 5 | 2 (1–5) | 1 (1–2) | 1 (1–1,8) | 1,2 (1–2) | 2 (1–2) |
| 6 | 2 (1,5–5,5) | 1,42 (1–2) | 1,67 | 1,4 (1–2) | 2 (1–2) |
| 7 | 3,5 (1–8) | 1,1 (1–1,97) | 1,5 (1–2) | 1,46 (1–2) | 2 (1,5–2) |
| 8 | 6,5 (1,5–9,5) | 1,08 (1–1,67) | 2 | 1,44 (1,18–1,8) | 2 |
| 9 | 9 (4,5–13) | 1,29 (1–1,7) | 1 (1–2) | 1,44 (1,29–1,67) | 2 (2–3) |
| 10 | 13 (6–18) | 1 (1–1,6) | 2 (1,25–2) | 1,4 (1,23–1,54) | 2 (2–3) |
| 11 | 15,5 (8,5–20) | 1,33 (1–1,5) | 1,5 (1,2–1,8) | 1,45 (1,24–1,7) | 2,5 (2–3) |
| 12 | 18 (11–21) | 1,42 (1–1,75) | 1 | 1,61 (1,29–1,84) | 3 (2–3,5) |
| 13 | 21,5 (12,5–24) | 1,35 (1–1,56) | 1,33 (1–2) | 1,63 (1,42–2,07) | 3 (2–4) |
| 14 | 21 (17–26) | 1,27 (1–1,58) | 1 (1–1,35) | 1,84 (1,55–2,2) | 3,5 (3–4) |
| 15 | 23 (19–28,5) | 1,37 (1–2) | 1 (1–1,55) | 1,98 (1,58–2,26) | 4 (3–4) |
| 16 | 25 (21–30) | 1,46 (1–2) | 1,5 (1,12–2,08) | 2,05 (1,76–2,33) | 4 (3–4,5) |
| 17 | 25,5 (22–32,5) | 1,13 (1–1,83) | 1,5 (1–2,37) | 2,18 (1,85–2,35) | 4 (4–4,5) |
| 18 | 26 (23–36) | 1,1 (1–1,88) | 1,9 (1,18–2,82) | 2,21 (1,91–2,55) | 4 (4–5) |
| 19 | 29 (24,5–38) | 1,27 (1–1,85) | 2 (1,4–3) | 2,29 (2,03–2,61) | 4 (4–5) |
| 20 | 31,5 (27–41) | 1,5 (1–1,87) | 3 (2,2–3,8) | 2,33 (2,07–2,65) | 5 (4–5,5) |
| 21 | 33 (28,5–41) | 1 (1–1,82) | 1,83 (1–2) | 2,43 (2,23–2,74) | 5 (4–5,5) |
| 22 | 33 (27–41) | 1,2 (1–1,75) | 2 (1,5–3,25) | 2,49 (2,09–2,72) | 5 (4,5–5,5) |
| 23 | 37 (31–45) | 1,4 (1,15–1,63) | 2,5 (1,76–3) | 2,53 (2,17–2,79) | 5 (4,5–6) |
| 24 | 41 (32,5–49) | 1,29 (1–1,47) | 2 (1,3–3,4) | 2,5 (2,25–2,77) | 5 (4,5–6) |
| 25 | 44,5 (34–51,5) | 1,5 (1,06–1,9) | 2 (1–3) | 2,58 (2,27–2,89) | 5 (4,5–6,5) |
| 26 | 46 (33–53) | 1,2 (1–1,33) | 1,67 (1–2,2) | 2,65 (2,32–2,94) | 5 (5–6,5) |
| 27 | 48,5 (36,5–57,5) | 1,25 (1–1,6) | 1,67 (1–2,7) | 2,65 (2,35–2,95) | 5 (5–7) |
| 28 | 50,5 (39–60) | 1,5 (1–2) | 2,25 (1,7–3,3) | 2,64 (2,41–2,96) | 5 (5–7) |
| 29 | 51 (43,5–63,5) | 1,29 (1–1,83) | 2,33 (1,8–3,2) | 2,71 (2,41–2,92) | 5,5 (5–7) |
| 30 | 56 (46–65) | 1,42 (1–1,8) | 2 (1–2,8) | 2,75 (2,46–3,01) | 6 (5–7,5) |
| 31 | 57,5 (48–68,5) | 1 (1–1,6) | 3,17 (1,75–4,38) | 2,77 (2,44–2,99) | 6 (5–7,5) |
| 32 | 58 (50,5–68) | 1,33 (1–2) | 2 (1–2,74) | 2,82 (2,57–3,13) | 6 (5–7,5) |
| 33 | 57,5 (52–66,5) | 1,33 (1–1,5) | 2,5 (1,53–3,15) | 2,85 (2,54–3,14) | 6 (5–7,5) |
| 34 | 58,5 (52–69) | 1,33 (1–1,5) | 2 (1,05–3,2) | 2,9 (2,58–3,32) | 6 (5,5–7,5) |
| 35 | 61 (54,5–69,5) | 1,33 (1–1,63) | 2,6 (1–4,9) | 2,91 (2,58–3,32) | 6 (5,5–8) |
| 36 | 64 (56–71) | 1,37 (1–1,88) | 2,17 (1–3,55) | 2,97 (2,6–3,33) | 6 (5,5–8,5) |
| 37 | 66 (57–72,5) | 1,33 (1–2) | 2 (1,25–4,05) | 3,04 (2,7–3,45) | 6,5 (6–8,5) |
| 38 | 66 (57–73) | 1,29 (1–1,9) | 2,25 (1,45–3,25) | 3,05 (2,64–3,45) | 7 (6–8,5) |
| 39 | 64 (56,5–75,5) | 1,33 (1–1,92) | 2,9 (2–3) | 3,14 (2,7–3,49) | 7 (6–8,5) |
| 40 | 67,5 (56–75,5) | 1,13 (1–1,5) | 3 (1,8–4,67) | 3,15 (2,66–3,55) | 7 (6–8,5) |
| 41 | 69 (57–74) | 1,6 (1,13–2) | 2,67 (1–3,1) | 3,24 (2,79–3,64) | 7 (6–8,5) |
| 42 | 71 (58,5–76,5) | 1,5 (1–1,82) | 3 (2,75–3,75) | 3,22 (2,89–3,62) | 7 (6–9) |
| 43 | 71 (58–80) | 1,38 (1–2) | 2 (1–3,4) | 3,27 (2,89–3,61) | 7 (6–9,5) |
| 44 | 69,5 (56–81,5) | 1,5 (1–1,62) | 3 (1,4–4,24) | 3,34 (2,97–3,65) | 7,5 (6–9,5) |
| 45 | 73 (54,5–82,5) | 1,33 (1–2) | 2,63 (1,62–3,83) | 3,37 (3,01–3,68) | 7,5 (6–9,5) |
| 46 | 71 (56–84,5) | 1,45 (1–1,91) | 3,13 (1,7–3,74) | 3,34 (2,93–3,75) | 7,5 (6–9,5) |
| 47 | 72 (54,5–84,5) | 1,33 (1–2) | 3 (2,2–5) | 3,35 (3,08–3,77) | 8 (6–9,5) |
| 48 | 68 (56,5–86) | 1,23 (1–1,71) | 3,04 (2,45–5,1) | 3,36 (3,08–3,79) | 8 (6,5–10) |
| 49 | 72 (54,5–85) | 1,67 (1,22–1,96) | 3 (1,9–5) | 3,4 (3,14–3,97) | 8 (6,5–10) |
| 50 | 74 (56,5–85,5) | 1,4 (1–1,72) | 3 (2–3,53) | 3,39 (3,09–3,91) | 8 (6,5–10) |
| 51 | 71 (56–86,5) | 1,45 (1–1,94) | 3 (1,85–4,63) | 3,48 (3,07–3,8) | 8 (6,5–10) |
| 52 | 72 (57–88,5) | 1,55 (1–2) | 3 (2,11–3,3) | 3,51 (3,2–3,93) | 8 (6,5–10) |
| 53 | 73,5 (57,5–86,5) | 1,5 (1,1–1,78) | 3 (2,42–4,1) | 3,48 (3,09–3,93) | 9 (6,5–10) |
| 54 | 74 (56–88) | 1,5 (1–2) | 2,8 (1,5–4,25) | 3,57 (3,22–3,87) | 9 (6,5–10) |
| 55 | 76,5 (56,5–86,5) | 1,38 (1,18–1,88) | 3,11 (2,5–5) | 3,6 (3,17–3,95) | 9 (7–10) |
| 56 | 75,5 (57,5–88,5) | 1,5 (1–2) | 2,5 (1,95–3,9) | 3,67 (3,21–4,01) | 9 (7–10) |
| 57 | 76,5 (61–88,5) | 1,35 (1,13–1,63) | 3 (2,4–4,97) | 3,59 (3,15–3,98) | 9 (7–10) |
| 58 | 76 (60,5–88,5) | 1,5 (1–2,17) | 3,7 (2,9–6,1) | 3,56 (3,12–3,97) | 9 (7,5–10) |
| 59 | 78,5 (55–88,5) | 1,5 (1,28–2) | 3 (2–4,9) | 3,52 (3,15–4,12) | 9 (7,5–10) |
| 60 | 74,5 (55,5–87,5) | 1,5 (1,33–2) | 4,1 (2,75–5,92) | 3,56 (3,13–4,04) | 9 (7,5–10) |

### Han ve ticaret

| Yıl | Ayakta han | Asılan ilan | Biten ilan | Ticaret seferi (kervan) | İkmal seferi |
|---|---|---|---|---|---|
| 1 | 2 (1–3) | 0 (0–1) | 0 | 0 | 0 |
| 2 | 4 | 0 (0–1) | 0 | 0 | 0 |
| 3 | 4 | 0 (0–1) | 0 | 0 | 0 |
| 4 | 4 (3–4) | 1 (0–2) | 0 | 0 (0–0,5) | 0 |
| 5 | 3,5 (3–4) | 2 (1–3,5) | 0 | 0 (0–2,5) | 0 |
| 6 | 3 (2,5–4) | 1 (0,5–2) | 0 | 2 (0–8) | 0 |
| 7 | 3 (2–4) | 2 (0,5–3,5) | 0 | 6 (2,5–14,5) | 0 |
| 8 | 3 (2–4) | 2 (1–4) | 0 | 12 (6,5–24) | 0 |
| 9 | 3 (2–4) | 1,5 (0–3) | 0 | 17 (10,5–36) | 0 |
| 10 | 3 (2–4) | 2 (1–3,5) | 0 | 23,5 (15–47,5) | 0 |
| 11 | 3 (2–4) | 3 (2–5) | 0 (0–2,5) | 28,5 (15–53,5) | 0 (0–2) |
| 12 | 3 (2–4) | 3 (0,5–5,5) | 1 (0–2) | 31,5 (20–60,5) | 0 (0–5) |
| 13 | 3 (2–4,5) | 3 (1–5) | 1 (0–3) | 36,5 (21–60,5) | 4 (0–5) |
| 14 | 4 (3–5) | 2,5 (0,5–6) | 0 (0–2) | 40 (24–64,5) | 5 (1,5–7,5) |
| 15 | 4 (3–5) | 2,5 (0,5–4) | 1 (0–1,5) | 43,5 (25,5–68) | 7 (4,5–10,5) |
| 16 | 4 (3–5) | 2 (0,5–3) | 0,5 (0–3,5) | 44,5 (26–70,5) | 8 (4,5–14) |
| 17 | 4 (4–5) | 2 (1–3) | 0 (0–1,5) | 45,5 (28–71,5) | 11 (5–13) |
| 18 | 4 (4–5) | 2 (0–5) | 1 (0–2) | 51 (31–76) | 11,5 (4,5–16) |
| 19 | 5 (4–5,5) | 2 (1–4) | 0 (0–2) | 54,5 (33–78) | 13 (5,5–17) |
| 20 | 5 (4–5,5) | 2 (0–4,5) | 1 (0–1,5) | 57 (34–80) | 13,5 (5,5–18) |
| 21 | 5 (4–6) | 2,5 (0,5–4,5) | 0 (0–2,5) | 55 (35,5–79,5) | 15 (6,5–20,5) |
| 22 | 5 (4–6) | 4 (1,5–7) | 0 (0–1,5) | 60 (36,5–81) | 15 (6,5–21) |
| 23 | 5 (4,5–6) | 3 (1–4) | 0 (0–1) | 60,5 (40,5–83,5) | 18 (8–21,5) |
| 24 | 5 (4–6) | 2,5 (1–6,5) | 0,5 (0–2) | 60 (42–86) | 19,5 (7,5–23,5) |
| 25 | 5 (4,5–6) | 3,5 (1,5–5,5) | 0,5 (0–1,5) | 60,5 (39–85,5) | 19 (8–24,5) |
| 26 | 5 (5–6) | 3,5 (2–6) | 0 (0–1,5) | 58 (38–88) | 19 (8–27) |
| 27 | 5 (5–6) | 3 (1,5–4) | 0 (0–1) | 62 (39,5–87,5) | 21 (9–29) |
| 28 | 5 (4–6) | 2,5 (1–5) | 0,5 (0–1) | 64,5 (42,5–93) | 18,5 (8–25,5) |
| 29 | 5 (5–6) | 3 (2–6) | 1 (0–1,5) | 65,5 (46–89,5) | 19,5 (8,5–27) |
| 30 | 5 (4–6) | 3 (1–6,5) | 1 (0–2) | 72 (50,5–102) | 21 (11,5–27) |
| 31 | 5,5 (5–6) | 3 (1–6) | 0,5 (0–2,5) | 66,5 (48–98,5) | 21 (11,5–30) |
| 32 | 6 (5–6) | 3 (1,5–7,5) | 1 (0–2) | 67,5 (50,5–98) | 22,5 (12,5–28,5) |
| 33 | 6 (5–6) | 3,5 (1–5,5) | 0 (0–1,5) | 64,5 (48,5–97,5) | 22,5 (15–30,5) |
| 34 | 5,5 (5–6) | 4,5 (1–8) | 0 (0–2,5) | 66,5 (49–101) | 22 (14–30,5) |
| 35 | 5,5 (5–6) | 3 (1,5–5) | 1 (0–3) | 65 (50,5–102) | 24 (15–31) |
| 36 | 5,5 (5–6) | 3 (1–4,5) | 0,5 (0–3) | 69 (49–99) | 23,5 (15,5–32,5) |
| 37 | 5,5 (5–6) | 3,5 (1–5,5) | 1,5 (0–2,5) | 72,5 (50,5–104) | 25 (16–33) |
| 38 | 5 (5–6) | 4 (1–6,5) | 1 (0–2,5) | 75,5 (48–110) | 24,5 (16–34) |
| 39 | 5 (5–6) | 4 (2–6,5) | 1 (0–3) | 72 (49,5–104) | 25 (16–38) |
| 40 | 5 (5–6) | 2,5 (0,5–6,5) | 0 (0–2,5) | 69,5 (48–108) | 25,5 (15,5–38) |
| 41 | 5 (4–6) | 3 (1–7,5) | 1 (0–3) | 72,5 (47,5–112) | 25,5 (15,5–37,5) |
| 42 | 5 (4,5–6) | 3 (1,5–5,5) | 2 (0–3) | 73 (48,5–128) | 26,5 (17–37) |
| 43 | 5 (4–6) | 3 (1–7,5) | 1 (0–2,5) | 80 (48–132) | 26,5 (15,5–38,5) |
| 44 | 5,5 (5–6) | 4 (1–7) | 1 (0–2,5) | 79 (51,5–132) | 25 (16–36,5) |
| 45 | 5,5 (4–6) | 3 (1–5) | 1,5 (0–3) | 74,5 (51,5–133) | 26 (16–36,5) |
| 46 | 6 (5–6) | 3,5 (0,5–8,5) | 1 (0–3) | 79,5 (48–134) | 25,5 (16–36,5) |
| 47 | 5 (5–6) | 3,5 (1–5) | 1 (0–2) | 77,5 (46,5–126) | 27 (15,5–36,5) |
| 48 | 5 (5–6,5) | 2 (1–5,5) | 1 (0–2) | 80 (49–131) | 25,5 (16–37) |
| 49 | 5,5 (5–6) | 3 (1,5–6) | 2 (0,5–4,5) | 81 (50,5–122) | 24,5 (17–37) |
| 50 | 6 (5–6) | 4 (0,5–8) | 1 (0–1,5) | 73,5 (48,5–130) | 24,5 (16,5–34,5) |
| 51 | 6 (4–6) | 3 (1,5–7) | 1,5 (0–4) | 76 (48–126) | 26,5 (17,5–38) |
| 52 | 5,5 (5–6) | 3 (0,5–4,5) | 2 (0–3) | 76,5 (47–132) | 27,5 (15,5–39,5) |
| 53 | 5,5 (4,5–6) | 4 (1–7) | 0,5 (0–2) | 76 (52,5–134) | 30,5 (17,5–40) |
| 54 | 5,5 (5–6,5) | 3,5 (1,5–6) | 2 (0,5–4) | 78,5 (46,5–142) | 27,5 (16–41) |
| 55 | 6 (5–6) | 3 (1,5–8,5) | 2 (0,5–3,5) | 78 (51–142) | 28,5 (16,5–44,5) |
| 56 | 6 (5–6) | 2,5 (0–6,5) | 2 (0–5) | 84,5 (50–140) | 28 (17–43,5) |
| 57 | 6 (5–6,5) | 2,5 (1–4,5) | 1 (0–4) | 83 (54,5–144) | 27,5 (17–40,5) |
| 58 | 6 (5–7) | 4 (0,5–7,5) | 1 (0–3,5) | 86,5 (51–154) | 27,5 (17–42) |
| 59 | 6 (5–7) | 2,5 (1–5,5) | 1 (0–3,5) | 87,5 (53–163) | 29 (18,5–44) |
| 60 | 6 (4,5–7) | 3 (0,5–7,5) | 1,5 (0–4) | 92,5 (50,5–148) | 28,5 (16,5–41) |

### Altın ve ambar (1/3)

| Yıl | Altın p90 (medeniyetler) | Bakım gideri (altın; asker, kahraman, L2–L3) | Kamu işlerine (imar) harcanan altın | Ambarla beslenen amele tayını (gıda) | Kamu işlerindeki (amele) iş gücü payı | İmar ortalaması (köy+, 0–100) |
|---|---|---|---|---|---|---|
| 1 | 21,5 (11,7–36,9) | 0 | 0 | 0 | %0 | – |
| 2 | 51,4 (42,6–72,4) | 0 | 0 | 0 | %0 | – |
| 3 | 81,7 (63,2–103) | 0 | 0 | 0 | %0 | 0 |
| 4 | 103 (68,1–129) | 0 | 0 | 0,75 (0–4,25) | %0,1 (%0–%0,3) | 0 |
| 5 | 110 (80,5–145) | 0 (0–12,5) | 0 | 1 (0–4,35) | %0,1 (%0–%0,2) | 0 |
| 6 | 135 (92,2–185) | 2,33 (0–25) | 0 | 4,6 (0–16,3) | %0,2 (%0–%0,7) | 0 |
| 7 | 168 (108–246) | 24,1 (1,14–43,3) | 0 (0–1,95) | 9,75 (1,85–30,2) | %0,4 (%0,1–%1) | 0 (0–0,04) |
| 8 | 196 (155–241) | 52,6 (17,3–83,3) | 0 (0–4,7) | 11 (0,1–26) | %0,3 (%0–%0,7) | 0 (0–0,23) |
| 9 | 209 (132–343) | 108 (53–137) | 0 (0–2,75) | 4,65 (0–23,4) | %0,1 (%0–%0,5) | 0,02 (0–0,13) |
| 10 | 291 (170–475) | 174 (95,3–228) | 0,08 (0–26,4) | 1,45 (0–19,5) | %0,1 (%0–%0,4) | 0,02 (0–0,73) |
| 11 | 314 (191–619) | 236 (166–362) | 5,78 (0–158) | 8,6 (0–37,6) | %0,3 (%0,1–%1,1) | 0,17 (0–1,82) |
| 12 | 446 (364–763) | 315 (227–468) | 92,9 (18,2–404) | 25,3 (0–122) | %1,2 (%0,5–%1,8) | 1,35 (0,23–4,73) |
| 13 | 589 (377–1007) | 387 (286–588) | 267 (37,9–666) | 63,1 (7,05–165) | %2 (%1–%3,6) | 4,48 (0,72–8,36) |
| 14 | 637 (538–994) | 471 (367–691) | 512 (294–1093) | 144 (52,3–307) | %3,8 (%2,7–%4,6) | 8,42 (3,62–12,9) |
| 15 | 702 (576–1037) | 609 (423–762) | 756 (441–1313) | 320 (85,5–694) | %5,9 (%4,1–%8,5) | 10,5 (6,99–15,7) |
| 16 | 735 (512–1064) | 684 (516–846) | 832 (573–1459) | 644 (190–878) | %9,3 (%5,8–%11) | 13,4 (10,6–19,1) |
| 17 | 751 (588–836) | 764 (586–944) | 913 (640–1487) | 847 (422–1562) | %12 (%8,2–%14) | 15,2 (13,8–21,3) |
| 18 | 698 (556–970) | 861 (668–1005) | 1027 (711–1422) | 1102 (608–1432) | %13 (%10–%15) | 17,4 (15–22,6) |
| 19 | 838 (592–1100) | 898 (798–1114) | 1239 (704–1498) | 1501 (901–1763) | %14 (%12–%17) | 18,6 (16,1–23,7) |
| 20 | 821 (562–1216) | 1055 (848–1207) | 1400 (761–1890) | 1592 (967–2387) | %16 (%13–%18) | 21,4 (16,7–25,7) |
| 21 | 953 (713–1153) | 1112 (918–1281) | 1480 (785–1874) | 1796 (1281–2366) | %17 (%13–%19) | 21,7 (17,8–27,8) |
| 22 | 878 (655–1317) | 1198 (944–1346) | 1527 (934–2060) | 2188 (1611–2659) | %18 (%15–%20) | 22,7 (19,2–28,6) |
| 23 | 922 (717–1380) | 1272 (1043–1435) | 1449 (864–2106) | 2429 (1503–3291) | %18 (%16–%20) | 24,1 (19,8–27,5) |
| 24 | 1042 (618–1367) | 1325 (1148–1599) | 1498 (884–2364) | 2245 (1612–3288) | %19 (%16–%21) | 24,6 (19,7–27,8) |
| 25 | 1138 (835–1436) | 1413 (1261–1665) | 1976 (968–2186) | 2579 (1964–3512) | %20 (%17–%22) | 24,7 (19,2–28,9) |
| 26 | 1095 (792–1337) | 1479 (1304–1793) | 1923 (1001–2358) | 2491 (2055–3663) | %20 (%18–%22) | 25,6 (20,3–28,7) |
| 27 | 1110 (865–1307) | 1553 (1384–1834) | 1912 (1161–2380) | 2546 (1868–4595) | %21 (%18–%23) | 25,9 (19,9–31) |
| 28 | 1138 (820–1386) | 1634 (1382–1990) | 2152 (1233–2495) | 2314 (1727–3954) | %22 (%17–%23) | 26,4 (20,5–31) |
| 29 | 1140 (958–1504) | 1685 (1415–1966) | 2190 (1449–2655) | 2582 (1709–4188) | %21 (%19–%25) | 27,6 (21,7–31,3) |
| 30 | 1197 (948–1576) | 1768 (1412–2014) | 2297 (1523–2772) | 2662 (1552–4609) | %22 (%18–%24) | 28,9 (21,6–32,9) |
| 31 | 1260 (898–1582) | 1829 (1547–2033) | 2142 (1596–2882) | 2928 (2209–3939) | %22 (%20–%24) | 28,5 (22,4–33,4) |
| 32 | 1189 (882–1487) | 1910 (1601–2137) | 2237 (1764–2944) | 2971 (1749–4345) | %22 (%20–%24) | 29,2 (23,7–34,9) |
| 33 | 1269 (999–1393) | 1920 (1673–2192) | 2352 (1927–2953) | 2792 (1824–3901) | %22 (%20–%24) | 28,1 (24,3–34,8) |
| 34 | 1199 (934–1546) | 1926 (1725–2160) | 2508 (1959–2937) | 2554 (1715–4066) | %22 (%21–%24) | 29 (24,7–37,1) |
| 35 | 1201 (923–1394) | 1968 (1783–2273) | 2530 (1913–2797) | 2241 (1243–3941) | %22 (%20–%24) | 28,8 (24,9–37,3) |
| 36 | 1197 (919–1439) | 2085 (1796–2211) | 2420 (1841–2863) | 2661 (1224–3500) | %22 (%20–%25) | 29,2 (24,6–34,8) |
| 37 | 1325 (1029–1629) | 2106 (1822–2335) | 2606 (1991–3078) | 2284 (1055–4026) | %22 (%20–%24) | 30,5 (24,8–36,2) |
| 38 | 1324 (1041–1653) | 2144 (1876–2327) | 2522 (2064–3226) | 2216 (1068–3833) | %23 (%20–%25) | 31 (24,8–37,6) |
| 39 | 1253 (1029–1799) | 2135 (1911–2399) | 2681 (1969–3325) | 2354 (1132–3736) | %23 (%20–%24) | 31,5 (24,8–37,5) |
| 40 | 1261 (1002–1713) | 2200 (1949–2490) | 2702 (2032–3375) | 2113 (639–3196) | %21 (%20–%24) | 31,9 (24,7–36,6) |
| 41 | 1257 (1076–1699) | 2129 (1898–2418) | 2674 (2198–3473) | 2092 (592–4440) | %23 (%18–%24) | 30,9 (24,1–36) |
| 42 | 1196 (960–1485) | 2161 (1923–2367) | 2678 (2240–3366) | 1871 (685–4488) | %22 (%19–%25) | 31,3 (24,9–35,4) |
| 43 | 1281 (1025–1863) | 2168 (1969–2432) | 2967 (2168–3448) | 2327 (535–3369) | %22 (%19–%24) | 30,8 (24,8–36,5) |
| 44 | 1266 (1123–1863) | 2216 (1998–2415) | 2827 (2282–3349) | 2243 (498–4138) | %22 (%19–%24) | 32,3 (25,1–37,3) |
| 45 | 1571 (1124–1888) | 2189 (1924–2560) | 2886 (2240–3582) | 2194 (752–4529) | %21 (%18–%24) | 32,6 (24,2–37,2) |
| 46 | 1461 (1273–2109) | 2208 (2013–2656) | 3029 (2428–4979) | 2121 (821–4016) | %22 (%19–%24) | 32,5 (25,8–37,2) |
| 47 | 1546 (1266–1995) | 2200 (1954–2574) | 3038 (2579–4950) | 1623 (399–4222) | %21 (%18–%24) | 32,2 (24,8–37,4) |
| 48 | 1518 (1200–1755) | 2300 (1965–2579) | 3066 (2415–4647) | 1931 (287–4448) | %22 (%17–%24) | 32,3 (24,4–37,3) |
| 49 | 1476 (1299–3353) | 2225 (1968–2618) | 3231 (2320–8540) | 1326 (563–4442) | %22 (%18–%24) | 33,3 (26,2–38,7) |
| 50 | 1605 (1163–2348) | 2272 (1971–2605) | 3284 (2560–7072) | 1771 (426–3845) | %22 (%20–%25) | 33,5 (26,6–39,2) |
| 51 | 1350 (1214–2017) | 2280 (2091–2683) | 3361 (2391–5600) | 1168 (333–3277) | %22 (%20–%24) | 33,3 (25,7–38,9) |
| 52 | 1501 (1261–1785) | 2274 (1943–2697) | 3170 (2524–4766) | 1125 (505–2815) | %22 (%19–%25) | 33,2 (25,8–39,4) |
| 53 | 1583 (1264–1824) | 2302 (2032–2791) | 3183 (2453–4585) | 1262 (477–3109) | %22 (%19–%24) | 33,7 (25,7–39,8) |
| 54 | 1485 (1233–1888) | 2324 (2077–2765) | 3306 (2281–4396) | 1386 (351–3334) | %22 (%19–%25) | 32,4 (26,5–40) |
| 55 | 1404 (1166–1788) | 2392 (2022–2868) | 3269 (2177–4150) | 1005 (396–2808) | %21 (%18–%26) | 32 (27,2–38,6) |
| 56 | 1434 (1248–1898) | 2412 (2038–2911) | 3348 (2179–4290) | 1059 (476–2527) | %22 (%18–%25) | 31,9 (27,5–38,5) |
| 57 | 1512 (1318–1771) | 2438 (2074–2815) | 3334 (2435–4505) | 1034 (218–2631) | %21 (%18–%25) | 32,3 (28,2–38,1) |
| 58 | 1439 (1283–1892) | 2418 (2083–2827) | 3291 (2511–4363) | 1290 (416–2365) | %21 (%19–%24) | 32,6 (28,8–37,5) |
| 59 | 1506 (1272–1782) | 2330 (2011–2865) | 3588 (2280–4791) | 1127 (658–2502) | %22 (%20–%24) | 32,8 (29,2–38,1) |
| 60 | 1556 (1108–2198) | 2310 (2018–2961) | 3430 (2119–4978) | 1393 (254–2853) | %22 (%19–%25) | 31,6 (28,7–37,8) |

### Altın ve ambar (2/3)

| Yıl | Canavar baskınında yitirilen altın | Ejderhaya giden altın (haraç + akın) | Hazinesi boş medeniyet payı | Kent tüketiminde yokluk payı (köy+; ekmek, bira ya da alet) | Ekmek ya da bira yokluğu payı (köy+) | Kıtlık (büyük olay) |
|---|---|---|---|---|---|---|
| 1 | 0 | 0 | %0 | – | – | 0 |
| 2 | 0 | 0 | %0 | – | – | 0 |
| 3 | 0 (0–5) | 0 | %0 | %0 | %0 | 0 |
| 4 | 14,5 (0–41,5) | 0 | %0 | %0 | %0 | 0 |
| 5 | 13 (0–50) | 0 | %0 | %0 (%0–%0,8) | %0 (%0–%0,8) | 0 |
| 6 | 10 (0–34,5) | 0 | %0 | %1,4 (%0,3–%3,5) | %1,4 (%0,3–%3,5) | 0 |
| 7 | 9 (0–22) | 0 | %0 | %2,3 (%0,5–%7,2) | %2,3 (%0,5–%7,2) | 0 |
| 8 | 12,5 (0–60) | 0 | %0 | %3,9 (%1–%19) | %3,1 (%0,9–%19) | 0 |
| 9 | 11,5 (0,5–48,5) | 0 | %0 | %4,7 (%0,8–%9,8) | %2,4 (%0–%9,7) | 0 |
| 10 | 17 (0–35,5) | 0 | %0 | %5 (%0,8–%8) | %1,6 (%0,1–%6,9) | 0 |
| 11 | 5 (0–56,5) | 0 | %0 | %5,4 (%1,3–%7,9) | %1,5 (%0,3–%5,6) | 0 |
| 12 | 6 (0–38,5) | 0 | %0 | %3,9 (%1,7–%9,1) | %2,5 (%0,7–%3,6) | 0 |
| 13 | 6 (0–19) | 0 | %0 | %4,4 (%0,6–%6,3) | %1,5 (%0,1–%4,1) | 0 |
| 14 | 1,5 (0–12,5) | 0 | %0 | %4 (%0,3–%6,5) | %0,5 (%0–%3,9) | 0 |
| 15 | 6 (0–54) | 0 | %0 | %3,3 (%0,4–%6,1) | %0,8 (%0–%3,5) | 0 |
| 16 | 0 (0–22) | 0 | %0 | %2,8 (%0,1–%6,2) | %0,9 (%0–%2,5) | 0 |
| 17 | 1 (0–47) | 0 | %0 | %2,3 (%0–%6,8) | %0,1 (%0–%3) | 0 |
| 18 | 0 (0–35,5) | 0 (0–186) | %0 | %2,2 (%0–%11) | %0,1 (%0–%3) | 0 |
| 19 | 0 (0–28) | 0 (0–171) | %0 | %3,4 (%0,1–%13) | %0,5 (%0–%3,3) | 0 |
| 20 | 0 (0–21,5) | 28 (0–348) | %0 | %2,9 (%0,1–%13) | %0,1 (%0–%3,4) | 0 |
| 21 | 0 (0–56) | 44,5 (0–299) | %0 | %4 (%0–%12) | %0,1 (%0–%4,2) | 0 |
| 22 | 0 (0–8) | 136 (40–259) | %0 | %8,1 (%1,7–%18) | %0,2 (%0–%4,7) | 0 |
| 23 | 0 (0–15) | 136 (49,5–304) | %0 | %11 (%2,2–%18) | %1,1 (%0–%3,9) | 0 |
| 24 | 0 (0–14) | 247 (56–481) | %0 | %11 (%1,5–%18) | %0,6 (%0–%6,8) | 0 |
| 25 | 0 (0–16,5) | 170 (73,5–408) | %0 | %12 (%1,3–%25) | %0,7 (%0–%8,8) | 0 |
| 26 | 0 (0–34) | 222 (81–606) | %0 | %15 (%0,5–%28) | %0,8 (%0–%8) | 0 |
| 27 | 0 (0–16) | 317 (156–666) | %0 | %16 (%4,8–%24) | %0,7 (%0–%6) | 0 |
| 28 | 0 (0–42) | 347 (192–617) | %0 | %20 (%6–%28) | %0,6 (%0–%7,1) | 0 |
| 29 | 0 (0–13) | 386 (130–792) | %0 | %22 (%7–%37) | %1,2 (%0–%8,7) | 0 |
| 30 | 0 (0–6) | 442 (190–558) | %0 | %17 (%8,9–%39) | %0,6 (%0–%7) | 0 |
| 31 | 0 (0–15) | 488 (144–710) | %0 | %24 (%13–%37) | %1 (%0–%5,7) | 0 |
| 32 | 0 (0–11,5) | 508 (159–695) | %0 | %29 (%11–%41) | %1,6 (%0–%6,9) | 0 |
| 33 | 0 (0–28,5) | 416 (58–710) | %0 | %27 (%12–%45) | %2,4 (%0,6–%5,4) | 0 |
| 34 | 0 (0–9) | 374 (33,5–634) | %0 | %35 (%9,9–%43) | %2,4 (%0,6–%7,2) | 0 |
| 35 | 0 (0–11,5) | 618 (7–861) | %0 | %36 (%19–%48) | %3 (%0,5–%8,2) | 0 |
| 36 | 0 (0–30,5) | 550 (39,5–856) | %0 | %35 (%14–%50) | %2,5 (%0,5–%7,5) | 0 |
| 37 | 0 (0–39) | 612 (0–1050) | %0 | %35 (%17–%50) | %3,5 (%0,7–%7) | 0 |
| 38 | 0 (0–12) | 564 (0–1176) | %0 | %37 (%16–%49) | %2,8 (%0,2–%6,7) | 0 |
| 39 | 0 (0–7) | 530 (0–814) | %0 | %34 (%20–%51) | %2,1 (%0,1–%7,4) | 0 |
| 40 | 0 (0–12,5) | 424 (0–990) | %0 | %42 (%24–%51) | %2,8 (%0,6–%6,9) | 0 |
| 41 | 0 (0–42,5) | 552 (0–934) | %0 | %41 (%22–%50) | %3,4 (%0,8–%7,9) | 0 |
| 42 | 0 | 486 (0–896) | %0 | %43 (%28–%53) | %2,6 (%1–%9,5) | 0 (0–0,5) |
| 43 | 0 (0–5) | 568 (0–958) | %0 | %41 (%26–%54) | %3,2 (%0,8–%9,7) | 0 |
| 44 | 0 (0–28) | 618 (0–1041) | %0 | %39 (%31–%56) | %3,5 (%0,3–%9,5) | 0 |
| 45 | 0 (0–4,5) | 506 (0–902) | %0 | %43 (%32–%56) | %4,9 (%1,5–%9,4) | 0 |
| 46 | 0 (0–4,5) | 364 (0–777) | %0 | %43 (%33–%59) | %5,9 (%2–%10) | 0 |
| 47 | 0 (0–46,5) | 372 (0–740) | %0 | %42 (%31–%57) | %4,4 (%1,1–%11) | 0 |
| 48 | 0 (0–12) | 462 (0–760) | %0 | %45 (%30–%59) | %4,5 (%1,4–%10) | 0 |
| 49 | 0 (0–36,5) | 230 (0–666) | %0 | %50 (%34–%58) | %4,5 (%1,1–%10) | 0 |
| 50 | 0 (0–26) | 256 (0–768) | %0 | %52 (%28–%57) | %5,3 (%1,5–%11) | 0 |
| 51 | 0 (0–7) | 276 (0–878) | %0 | %49 (%36–%57) | %4,8 (%0,8–%10) | 0 |
| 52 | 0 (0–40,5) | 260 (0–748) | %0 | %45 (%37–%59) | %4,8 (%1,4–%10) | 0 |
| 53 | 0 (0–13) | 158 (0–846) | %0 | %45 (%32–%57) | %3,4 (%1,2–%11) | 0 |
| 54 | 0 (0–15,5) | 228 (0–883) | %0 | %49 (%32–%57) | %5,4 (%1,3–%11) | 0 |
| 55 | 0 (0–10,5) | 303 (0–902) | %0 | %48 (%36–%59) | %6,1 (%1,8–%13) | 0 |
| 56 | 0 (0–22,5) | 366 (0–912) | %0 | %50 (%39–%60) | %6,2 (%2,8–%12) | 0 |
| 57 | 0 (0–24,5) | 352 (0–920) | %0 | %50 (%39–%62) | %7 (%3–%13) | 0 |
| 58 | 0 (0–7,5) | 212 (0–572) | %0 | %48 (%37–%64) | %7,9 (%4,4–%9,8) | 0 |
| 59 | 0 (0–11,5) | 112 (0–797) | %0 | %49 (%36–%63) | %7,5 (%3,4–%11) | 0 (0–0,5) |
| 60 | 0 (0–44) | 222 (0–778) | %0 | %50 (%41–%63) | %7,3 (%3,8–%9,8) | 0 |

### Altın ve ambar (3/3)

| Yıl | Açlıktan ölen | Kıtlık yardımı (sevkiyat) | Kıtlıkta yüz çeviren | Kıtlık akını |
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
| 10 | 0 | 0 | 0 | 0 |
| 11 | 0 | 0 | 0 | 0 |
| 12 | 0 | 0 | 0 | 0 |
| 13 | 0 | 0 | 0 | 0 |
| 14 | 0 | 0 | 0 | 0 |
| 15 | 0 | 0 | 0 | 0 |
| 16 | 0 | 0 | 0 | 0 |
| 17 | 0 | 0 | 0 | 0 |
| 18 | 0 | 0 | 0 | 0 |
| 19 | 0 | 0 | 0 | 0 |
| 20 | 0 | 0 | 0 | 0 |
| 21 | 0 | 0 | 0 | 0 |
| 22 | 0 | 0 | 0 | 0 |
| 23 | 0 | 0 | 0 | 0 |
| 24 | 0 | 0 | 0 | 0 |
| 25 | 0 | 0 | 0 | 0 |
| 26 | 0 | 0 | 0 | 0 |
| 27 | 0 | 0 | 0 | 0 |
| 28 | 0 | 0 | 0 | 0 |
| 29 | 0 | 0 | 0 | 0 |
| 30 | 0 | 0 | 0 | 0 |
| 31 | 0 | 0 | 0 | 0 |
| 32 | 0 | 0 | 0 | 0 |
| 33 | 0 | 0 | 0 | 0 |
| 34 | 0 | 0 | 0 | 0 |
| 35 | 0 | 0 | 0 | 0 |
| 36 | 0 | 0 | 0 | 0 |
| 37 | 0 | 0 | 0 | 0 |
| 38 | 0 | 0 | 0 | 0 |
| 39 | 0 | 0 | 0 | 0 |
| 40 | 0 | 0 | 0 | 0 |
| 41 | 0 | 0 | 0 | 0 |
| 42 | 0 | 0 (0–1) | 0 (0–0,5) | 0 |
| 43 | 0 | 0 | 0 | 0 |
| 44 | 0 | 0 | 0 | 0 |
| 45 | 0 | 0 | 0 | 0 |
| 46 | 0 | 0 | 0 | 0 |
| 47 | 0 | 0 | 0 | 0 |
| 48 | 0 | 0 | 0 | 0 |
| 49 | 0 | 0 | 0 | 0 |
| 50 | 0 | 0 | 0 | 0 |
| 51 | 0 | 0 | 0 | 0 |
| 52 | 0 | 0 | 0 | 0 |
| 53 | 0 | 0 | 0 | 0 |
| 54 | 0 | 0 | 0 | 0 |
| 55 | 0 | 0 | 0 | 0 |
| 56 | 0 | 0 | 0 | 0 |
| 57 | 0 | 0 | 0 | 0 |
| 58 | 0 | 0 | 0 | 0 |
| 59 | 0 | 0 (0–1) | 0 | 0 |
| 60 | 0 | 0 (0–2) | 0 | 0 (0–0,5) |

