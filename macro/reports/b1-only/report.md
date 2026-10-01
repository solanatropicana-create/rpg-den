# Ölçüm raporu: B1 yalnız (wave A + B1)

16 dünya (seed 1-16) × 60 yıl (7200 gün) · 2026-10-01 08:34 · `FD.Macro.Run stats --seeds 1-16 --years 60 --jobs 2 --verify 1 --saveload 1`

Ağaç: git HEAD (dalga A) + yalnız B1 değişiklikleri (Diplomacy.cs, Agents.cs Siege/Defenders, Sim.Extinct, Types.cs alanları, Stats.cs ölçüleri); B2 (anlatıcı, ejderha, troller, kamp hedefi) dâhil değil. Ortak ağaçtaki (B1 + B2) ölçüm: `../b1-6/report.md`.

Süre: 7 dk 31 sn duvar saati, 2 iş parçacığı; dünya başına 48,8 sn (en az 31,1, en çok 77,5; yıl sonu hash'leri dâhil).

## Bitiş ölçütleri

DESIGN-FAZ1.md, "Bitiş ölçütleri". ✓ geçti · ✗ kaldı · — ölçülemedi.

| # | Ölçüt | Koşul | Ölçülen | Sonuç |
|---|---|---|---|---|
| 1 | Donma yok | 41–60. yılların yıllık büyük olay medyanı ≥ 0,8 × 6–20. yılların medyanı | 21 / 56,5 = 0,37 kat | ✗ |
| 2 | Çöküş | dünyaların ≥ %75'inde 60 yılda ≥ 1 çöküş (yok olma ya da başkent kaybı) | %88 (14/16 dünya); toplam 105 çöküş: 17 yok olma, 88 başkent kaybı | ✓ |
| 3 | Kamplar | 41–60. yıllarda yaşayan kamp medyanı ≥ 6–20. yılların medyanı | 2,51 < 7,95 (yıl sonu sayımıyla 2 / 8) | ✗ |
| 4a | Kahraman: doğuş seviyesi | her on yılda doğanların ortalama seviyesi ≤ 2 | 1,31 · 1,34 · 1,39 · 1,38 · 1,43 · 1,49 (on yıllar sırasıyla) | ✓ |
| 4b | Kahraman: Sv8+ | dünyaların ≥ yarısında en az bir kahraman Sv8 ve üstüne çıkar | %50 (8/16 dünya); dünyadaki en yüksek seviye: medyan Sv7,5, en çok Sv10 | ✓ |
| 4c | Kahraman: efsane | dünya başına efsane medyanı 1–6 | medyan 1 (p10–p90: 0–4,5; toplam 29) | ✓ |
| 4d | Kahraman: ölüm payı | doğan kahramanların %30–80'i ölür | %34 (709/2107); dünya medyanı %35 (%18–%43) | ✓ |
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
| Araştırma ağacı erken bitiyor | ~19. yılda bitiyor; 30. yılda medeniyetlerin %98'i bitirmiş | başlangıç medeniyetlerinde ağaç bitişi (son çağ, araştıracak düğüm yok) medyanı 20. yıl (121/128 bitirdi); 30. yılda bitirmiş medeniyet payı %89, araştırması duran (her çağ) %95; ağacın biten payı 19. yılda %83, 30. yılda %95 | evet |
| Medeniyetler 5 yerleşimde takılıyor | 98 medeniyetin 74'ü (%76) tam 5 yerleşimde | tam 5 kara yerleşimli medeniyet payı 30. yılda %2,4, 60. yılda %2,5 (denizaşırı koloniler dâhil 30. yılda tam 5: %2,4, 5+: %97); medeniyet başına 8,81 yerleşim (30. yıl) | hayır |
| Kamp sayısı düşüyor | 6,8'den 3,5'e iniyor | 3 (1. yıl) → en yüksek 10 (7. yıl) → 2,5 (30. yıl) → 2 (60. yıl); yıl sonu, yıllık dünya medyanı | evet |
| Altın birikiyor | altın medyanı 78'den 6.503'e çıkıyor | 8,36 (1. yıl) → 6491 (30. yıl) → 32176 (60. yıl) | evet |
| İş gücü boşta | iş gücünün %43'ü boşta | 30. yılda %52, 60. yılda %52 (işe yerleşemeyen `zanaatçı` / bütün iş gücü, askerler dâhil; yıl içi ortalama) | evet |
| Büyük olaylar seyreliyor | yıllık büyük olay 59'dan 28'e düşüyor | en yüksek 73,5 (12. yıl) → 27 (30. yıl) → 16,5 (60. yıl), yıllık dünya medyanı | evet |
| Doğuş seviyesi şişiyor | 24. yıldan sonra herkes Sv5 doğuyor; efsane mekaniği ölü | 25–60. yıllarda Sv5+ doğanların payı %0; on yıllık doğuş seviyesi ortalaması 1,31 · 1,34 · 1,39 · 1,38 · 1,43 · 1,49 | hayır |
| Başkent düşmüyor | başkent fethedilemiyor (agents.ts:673) | 16 dünyada 88 başkent kaybı, 17 yok olma; 663 yerleşim fethi | hayır |

## On yıllık özet

Hücre: dünyalar arası medyan (p10–p90). Her dünyada on yılın yıllık değerlerinin ortalaması alınır: akış ölçülerinde yıllık ortalama, stok ölçülerinde yıl sonu değerlerinin ortalaması. Yüzdeler 0–1 paylardır.

| Ölçü | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| **Medeniyet** | | | | | | |
| Yaşayan medeniyet | 8 (7–9) | 8 (7–8,95) | 7,75 (7–8,65) | 7,5 (7–8,5) | 7,65 (7–8,9) | 8 (7–8,55) |
| Yeni medeniyet (yeniden doğan) | 0 | 0 (0–0,05) | 0 | 0 (0–0,05) | 0 (0–0,1) | 0 |
| Yok olan medeniyet | 0 | 0 (0–0,1) | 0 (0–0,1) | 0 | 0 (0–0,1) | 0 (0–0,05) |
| Başkent kaybı (medeniyet yaşarken) | 0 (0–0,05) | 0 (0–0,3) | 0 (0–0,25) | 0,1 (0–0,35) | 0,1 (0–0,3) | 0,1 (0–0,25) |
| Çöküş (yok olma + başkent kaybı) | 0 (0–0,1) | 0 (0–0,35) | 0 (0–0,3) | 0,1 (0–0,4) | 0,15 (0–0,4) | 0,1 (0–0,3) |
| Yaşayan yerleşim | 15,6 (12,3–16,8) | 47 (38,3–50,6) | 63,4 (54,1–69,3) | 70,7 (61,1–79,7) | 73,8 (65,6–87,7) | 78,2 (68,8–92,5) |
| Medeniyet başına yerleşim | 1,82 (1,67–2,07) | 5,7 (5,32–6,11) | 8,1 (7,48–8,8) | 9,37 (8,39–9,75) | 9,69 (9–10,7) | 10,5 (9,68–11,4) |
| 5+ kara yerleşimli medeniyet payı | %4,7 (%2,4–%8,6) | %78 (%71–%91) | %96 (%86–%100) | %99 (%93–%100) | %99 (%84–%100) | %97 (%86–%100) |
| Kurulan yerleşim | 2,4 (1,7–2,55) | 2,65 (2,2–3,1) | 1,25 (0,7–1,6) | 0,55 (0,3–1,25) | 0,55 (0,2–0,95) | 0,35 (0,1–0,85) |
| Fethedilen yerleşim | 0 (0–0,15) | 0,7 (0–1,05) | 0,75 (0,05–1,9) | 1,2 (0–1,7) | 0,95 (0–1,95) | 0,8 (0–1,6) |
| Terk edilen yerleşim | 0 | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,25) | 0 (0–0,25) | 0 (0–0,15) |
| Toplam nüfus | 274 (220–307) | 1399 (1095–1483) | 2560 (2102–2706) | 3259 (2864–3601) | 3690 (3247–4153) | 4010 (3588–4604) |
| Ortalama çağ | 1,89 (1,86–1,95) | 3,39 (3,16–3,55) | 3,89 (3,65–4) | 4 (3,87–4) | 4 (3,97–4) | 4 |
| Araştırma ağacının biten payı (ort.) | %19 (%18–%21) | %70 (%65–%74) | %92 (%86–%96) | %96 (%92–%98) | %97 (%95–%98) | %98 (%97–%99) |
| Ağacı bitmiş medeniyet payı | %0 | %20 (%11–%29) | %82 (%64–%93) | %99 (%81–%100) | %100 (%93–%100) | %100 (%96–%100) |
| Araştırması duran medeniyet payı | %0,6 (%0–%3,1) | %41 (%36–%51) | %90 (%85–%95) | %99 (%90–%100) | %100 (%96–%100) | %100 (%96–%100) |
| Altın medyanı (medeniyetler) | 55,1 (42,6–70,5) | 812 (463–1081) | 4333 (3063–6303) | 11042 (6143–18479) | 13564 (5658–33950) | 21806 (6469–49718) |
| Boştaki iş gücü payı | %0,6 (%0,4–%1,3) | %19 (%15–%21) | %49 (%43–%52) | %54 (%47–%58) | %51 (%47–%55) | %52 (%46–%55) |
| Bölünme (ayrılıp kurulan medeniyet) | 0 | 0 | 0 | 0 (0–0,05) | 0 (0–0,1) | 0 |
| En büyük medeniyetin yerleşimi | 2,65 (2,4–2,8) | 7,9 (6,9–8,35) | 10,9 (9,65–12,6) | 13 (10,4–17,8) | 14,6 (11–22) | 17,7 (11,2–25,6) |
| **Olaylar** | | | | | | |
| Olay | 81,6 (63,9–92) | 168 (142–218) | 118 (80,8–146) | 92,8 (58,3–136) | 94 (57–146) | 89,6 (53,3–121) |
| Büyük olay | 30,3 (26–36) | 61,5 (52,7–76,8) | 30,7 (22,4–46,2) | 24,1 (11,5–44,5) | 24,4 (12,2–47,3) | 21,7 (12–36,2) |
| **Savaş** | | | | | | |
| Muharebe | 3,95 (3,15–4,4) | 6,75 (4,8–8,9) | 4,3 (0,95–7,2) | 3,95 (0,7–7,2) | 4,45 (0,9–8,9) | 4,1 (1,25–7,3) |
| Başlayan savaş | 0 (0–0,2) | 0,95 (0–1,4) | 1,1 (0,05–2,1) | 1,5 (0–2,5) | 1 (0–2,55) | 1,1 (0–2,2) |
| Süren savaş (yıl sonu) | 0 (0–0,1) | 0,2 (0–0,55) | 0,2 (0–0,6) | 0,35 (0–0,75) | 0,2 (0–0,75) | 0,25 (0–0,9) |
| Yıl içinde süren savaş | 0 (0–0,25) | 1,25 (0–1,8) | 1,6 (0,05–2,6) | 1,8 (0–3,4) | 1,25 (0–3,3) | 1,45 (0–3,05) |
| Yağma akını (medeniyet) | 0 (0–0,3) | 0,95 (0–1,95) | 1,25 (0–2,6) | 1,7 (0–3,3) | 2,1 (0–4,4) | 1,85 (0–3,5) |
| Tarihî hak savaşı | 0 | 0 (0–0,3) | 0 (0–0,4) | 0,15 (0–0,5) | 0 (0–0,35) | 0 (0–0,35) |
| Pakt gereği savaş | 0 | 0 (0–0,1) | 0,05 (0–0,35) | 0,1 (0–0,65) | 0,05 (0–0,65) | 0 (0–0,25) |
| Kutsal Sefer çağrısı | 0 | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) |
| İhanet (pakt çiğnendi) | 0 | 0 | 0 (0–0,05) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,15) |
| Savunma paktı (yıl sonu) | 0 (0–0,05) | 0,1 (0–1) | 0,65 (0–1,05) | 1 (0–1,9) | 1,05 (0–2) | 0,75 (0–2) |
| **Canavarlar** | | | | | | |
| Yaşayan kamp (yıl sonu) | 7,45 (6,95–8,3) | 5,6 (4,95–7,35) | 2,5 (1,2–4,85) | 2,35 (1,9–3,05) | 2,4 (1,8–4,7) | 2,2 (1,65–3,6) |
| Yaşayan kamp (yıl ort.) | 7,16 (6,57–7,89) | 6,04 (5,18–7,61) | 2,63 (1,11–4,96) | 2,29 (1,94–3,21) | 2,35 (1,79–4,55) | 2,25 (1,76–3,7) |
| Doğan kamp | 0,75 (0,7–0,8) | 0,75 (0,4–1,4) | 0,4 (0,1–0,9) | 0,45 (0,2–0,7) | 0,45 (0,25–1,1) | 0,5 (0,1–0,95) |
| Temizlenen kamp | 0 (0–0,15) | 1,55 (0,9–1,9) | 0,45 (0,1–1,1) | 0,45 (0,15–0,75) | 0,5 (0,15–0,95) | 0,5 (0,25–0,9) |
| Canavar baskını | 3,45 (2,75–3,95) | 3,35 (2,35–4,65) | 0,5 (0,05–1,7) | 0,5 (0,1–1,05) | 0,7 (0,25–1,35) | 0,7 (0,15–1) |
| **Kahramanlar** | | | | | | |
| Doğan kahraman | 1,5 (0,9–1,95) | 2,5 (2,2–3,5) | 2,05 (1,4–2,75) | 1,95 (1,2–3) | 2,25 (1,4–3) | 2,45 (1,35–4,5) |
| Ölen kahraman | 0,2 (0,1–0,6) | 0,9 (0,35–1,3) | 0,55 (0,05–1,15) | 0,8 (0,15–1,6) | 0,75 (0,2–1,8) | 0,85 (0,25–1,7) |
| Emekli olan kahraman | 0 | 0 | 0 (0–0,1) | 0,3 (0–0,4) | 0,8 (0,5–1,25) | 0,9 (0,6–1,2) |
| Diyarı terk eden kahraman | 0 (0–0,1) | 0 (0–0,05) | 0,1 (0–0,4) | 0,4 (0,05–0,6) | 0,6 (0,3–0,8) | 0,6 (0,4–1,05) |
| Ölümden dönen kahraman | 0 | 0 | 0 | 0 | 0 | 0 |
| Efsane olan kahraman | 0 | 0 (0–0,05) | 0 (0–0,15) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,15) |
| Yaşayan kahraman (yıl sonu) | 4,15 (2,75–5,5) | 21,5 (17,2–28,4) | 36,1 (32,6–40) | 44,4 (39,4–53,8) | 47,5 (39,1–59,5) | 51,9 (40,7–61,6) |
| Doğuş seviyesi (ort.) | 1,26 (1,18–1,49) | 1,36 (1,2–1,5) | 1,38 (1,29–1,51) | 1,36 (1,22–1,54) | 1,41 (1,3–1,75) | 1,5 (1,41–1,73) |
| Ölüm seviyesi (ort.) | 1,33 (1–1,93) | 1,46 (1,15–1,76) | 2 (1,55–2,7) | 2 (1,75–3,12) | 2,71 (1,95–3,52) | 3,24 (2,36–4,54) |
| Yaşayan kahraman seviyesi (ort.) | 1,21 (1,15–1,49) | 2,01 (1,64–2,22) | 2,45 (2,08–2,73) | 2,54 (2,16–3,17) | 2,66 (2,13–3,38) | 2,75 (2,17–3,15) |
| En yüksek seviye (şimdiye dek) | 1,45 (1–1,75) | 3,9 (2,9–4,65) | 5 (4,05–6,35) | 5,65 (4,5–7,75) | 6,65 (4,75–9,05) | 7,25 (5–9,5) |
| **Han ve ticaret** | | | | | | |
| Ayakta han | 3,3 (2,7–3,85) | 4,3 (3–4,65) | 5 (4,75–6) | 5 (5–6) | 5 (5–6) | 5 (5–6) |
| Asılan ilan | 1,45 (1–1,55) | 1,8 (1,4–2,7) | 0,3 (0–1,15) | 0,3 (0,05–0,65) | 0,45 (0,1–0,75) | 0,4 (0,05–0,8) |
| Biten ilan | 0 (0–0,05) | 0,3 (0,2–0,75) | 0,1 (0–0,45) | 0,1 (0–0,25) | 0,1 (0–0,4) | 0,15 (0–0,35) |
| Ticaret seferi (kervan) | 6,6 (2,8–12,6) | 42,1 (21,5–69,6) | 62,1 (33,1–87,9) | 72,9 (41,3–89,1) | 80,1 (43,5–117) | 85,5 (50,8–144) |
| İkmal seferi | 0 | 6,8 (2,15–9,65) | 17,2 (11,3–26,4) | 24 (16,3–31,2) | 23,8 (18,3–36,6) | 27,8 (18,2–40,9) |

## Kahraman seviyeleri

### Doğuş seviyesi (bütün dünyalar, on yıl içinde doğanlar)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 |
|---|---|---|---|---|---|
| 1–10 | 233 | 1,31 | %69 | %31 | %0 |
| 11–20 | 424 | 1,34 | %67 | %32 | %1,2 |
| 21–30 | 332 | 1,39 | %63 | %36 | %1,5 |
| 31–40 | 326 | 1,38 | %63 | %35 | %1,5 |
| 41–50 | 353 | 1,43 | %61 | %35 | %4 |
| 51–60 | 439 | 1,49 | %57 | %38 | %5,5 |

### Ölüm seviyesi (bütün dünyalar, on yıl içinde ölenler)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 | Sv4 | Sv5 | Sv6 | Sv7 | Sv8 | Sv9 |
|---|---|---|---|---|---|---|---|---|---|---|---|
| 1–10 | 46 | 1,39 | %61 | %39 | %0 | %0 | %0 | %0 | %0 | %0 | %0 |
| 11–20 | 139 | 1,58 | %59 | %27 | %12 | %0,7 | %1,4 | %0 | %0 | %0 | %0 |
| 21–30 | 91 | 2,18 | %34 | %40 | %13 | %6,6 | %3,3 | %2,2 | %0 | %1,1 | %0 |
| 31–40 | 135 | 2,3 | %28 | %38 | %19 | %6,7 | %6,7 | %1,5 | %0 | %0 | %0 |
| 41–50 | 154 | 2,71 | %17 | %36 | %20 | %14 | %12 | %0 | %0 | %0,6 | %0 |
| 51–60 | 144 | 3,09 | %20 | %25 | %18 | %15 | %15 | %3,5 | %1,4 | %2,1 | %0,7 |

### Yaşayan kahramanların seviyesi (bütün dünyalar, on yılın son yılının sonunda)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 | Sv4 | Sv5 | Sv6 | Sv7 | Sv8 | Sv9 | Sv10 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 10 | 183 | 1,43 | %60 | %37 | %2,2 | %0,5 | %0 | %0 | %0 | %0 | %0 | %0 |
| 20 | 466 | 2,3 | %28 | %32 | %27 | %9 | %3,4 | %0,2 | %0,2 | %0 | %0 | %0 |
| 30 | 681 | 2,47 | %26 | %34 | %17 | %15 | %6,5 | %1,6 | %0 | %0 | %0 | %0 |
| 40 | 779 | 2,73 | %23 | %30 | %20 | %13 | %10 | %1,8 | %1,2 | %0,8 | %0,3 | %0 |
| 50 | 768 | 2,77 | %21 | %29 | %24 | %13 | %7 | %2,9 | %1,8 | %0,1 | %0,5 | %0,3 |
| 60 | 854 | 2,68 | %18 | %36 | %24 | %12 | %5,4 | %2,7 | %0,9 | %0,7 | %0,2 | %0,2 |

### Ölüm nedenleri (bütün dünyalar)

| Neden | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 | Toplam |
|---|---|---|---|---|---|---|---|
| Kuşatma | 2 | 36 | 53 | 96 | 76 | 57 | 320 (%45) |
| Kamp saldırısı | 26 | 82 | 10 | 17 | 21 | 30 | 186 (%26) |
| Suikast | 0 | 4 | 11 | 14 | 17 | 13 | 59 (%8,3) |
| Bilinmiyor | 0 | 0 | 0 | 0 | 18 | 38 | 56 (%7,9) |
| Yağma akını | 0 | 5 | 12 | 4 | 17 | 2 | 40 (%5,6) |
| Han baskını (canavar) | 15 | 4 | 0 | 0 | 0 | 0 | 19 (%2,7) |
| Yerleşim baskını (canavar) | 3 | 6 | 1 | 0 | 0 | 0 | 10 (%1,4) |
| Yol pususu | 0 | 1 | 3 | 2 | 2 | 0 | 8 (%1,1) |
| Han baskını (medeniyet) | 0 | 1 | 1 | 1 | 2 | 1 | 6 (%0,8) |
| Kervan soygunu | 0 | 0 | 0 | 1 | 1 | 3 | 5 (%0,7) |

Neden, ölümün kaydedildiği andaki son muharebenin türünden (başlık ve taraflar) ya da suikast olayından çıkarılır.

## Olay türleri

Dünya başına yıllık olay sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| İnşaat (`build`) | 25,2 (18,1–28,3) | 83,7 (62,4–100) | 52,4 (40,2–60) | 28,4 (15–38,8) | 15,7 (8,4–33,2) | 10,4 (6–20,4) |
| Kahraman (`hero`) | 2,95 (1,45–6,35) | 16,6 (10,8–24,4) | 14,5 (8,55–19,1) | 19,6 (12–27,1) | 22,3 (15,4–32,4) | 27,9 (21,5–44) |
| Göç (`migration`) | 1,4 (1–3,05) | 6,8 (4,8–10,8) | 13 (4,65–19,9) | 13,2 (10,1–23,3) | 15,5 (7,9–29,3) | 15,5 (8,75–25,3) |
| Savaş (`war`) | 0,5 (0–1,35) | 6,8 (0,2–10,4) | 8,75 (0,2–17,6) | 11,4 (0–20,9) | 11,8 (0–25,3) | 9,55 (0–21,3) |
| Araştırma (`research`) | 16 (13,7–18,1) | 16 (13,6–18,5) | 3,35 (1,65–4,15) | 0,5 (0–2,95) | 0,2 (0–1,3) | 0,05 (0–0,75) |
| Sınıf (`class`) | 4,5 (3,4–6,05) | 6,05 (5,1–7,3) | 4,9 (3,6–5,8) | 4,05 (2,9–5,75) | 3,8 (2,45–5,25) | 3,1 (2,05–4,75) |
| Deniz (`sea`) | 0,35 (0,1–1,05) | 5,1 (3–8,9) | 4,3 (2,3–8,05) | 3 (0,9–6,05) | 3,1 (0,8–7,1) | 2,35 (0,7–6,1) |
| Yerleşim (`settle`) | 4,8 (3,45–5,35) | 4,55 (4,1–5,7) | 2,3 (1,2–2,9) | 1,1 (0,6–2,15) | 1 (0,4–1,8) | 0,7 (0,15–1,65) |
| Ekonomi (`economy`) | 0,7 (0,5–1,25) | 1,9 (1,55–2,95) | 3,3 (1,75–4,5) | 3,45 (0,65–4,45) | 2,5 (0,35–3,55) | 2,05 (0,45–2,7) |
| Sefer/ilan (`quest`) | 1,55 (1–2,15) | 5,15 (3,45–8,7) | 1,2 (0,05–4,15) | 1,7 (0,15–3,05) | 1,65 (0,45–4,25) | 2,25 (0,35–4,35) |
| Keşif (`discover`) | 6,65 (5,35–7,85) | 2,75 (1,55–3,9) | 1,65 (0,85–2,75) | 0,95 (0,25–1,75) | 0,5 (0,05–1,85) | 0,45 (0,05–1,5) |
| Han (`inn`) | 2,45 (2,15–3) | 2,95 (1,75–4,25) | 1,95 (1,3–2,7) | 1,7 (1,05–2,55) | 1,7 (1,15–2,6) | 1,9 (1,3–2,6) |
| Baskın (`raid`) | 3,5 (2,75–3,95) | 3,35 (2,4–4,7) | 0,5 (0,1–1,7) | 0,5 (0,1–1,05) | 0,7 (0,25–1,4) | 0,7 (0,15–1,05) |
| Kamp (`lair`) | 2,1 (1,85–2,35) | 2,65 (1,7–4,1) | 0,9 (0,45–2,05) | 1,15 (0,35–1,55) | 1,15 (0,45–2,35) | 1,2 (0,45–2,05) |
| Ticaret (`trade`) | 1,2 (0,5–2,5) | 2 (1,1–2,65) | 0,8 (0,2–2,25) | 0,7 (0–1,8) | 0,95 (0–3,85) | 0,5 (0–1,25) |
| Dünya (`world`) | 0,1 (0–0,4) | 0,45 (0,1–0,75) | 0,65 (0,2–1) | 1 (0,55–1,7) | 1,4 (0,9–2,1) | 1,15 (0,75–2,15) |
| epitaph | 0,2 (0,1–0,6) | 0,9 (0,35–1,3) | 0,55 (0,05–1,15) | 0,8 (0,15–1,6) | 0,75 (0,2–1,8) | 0,85 (0,25–1,7) |
| Ölüm/terk (`death`) | 0,2 (0,1–0,6) | 0,9 (0,4–1,2) | 0,5 (0,05–1,1) | 0,8 (0,15–1,4) | 0,7 (0,2–1,95) | 0,85 (0,25–1,65) |
| Büyüme (`growth`) | 1,65 (1,3–1,9) | 1,3 (1,05–1,4) | 0,2 (0,1–0,35) | 0 (0–0,2) | 0 (0–0,15) | 0 (0–0,05) |
| Çağ (`era`) | 1,45 (1,25–1,65) | 0,7 (0,6–0,85) | 0,1 (0,05–0,3) | 0 (0–0,15) | 0 (0–0,05) | 0 |
| Temas (`contact`) | 0,9 (0,4–1,5) | 0,65 (0,25–0,85) | 0,3 (0,05–0,45) | 0,15 (0–0,35) | 0 (0–0,25) | 0,05 (0–0,15) |
| Gerginlik (`tension`) | 0,7 (0,3–1,35) | 0,5 (0,05–0,9) | 0,3 (0–0,8) | 0,35 (0–0,7) | 0,15 (0–0,7) | 0,05 (0–0,45) |
| Diplomasi (`diplomacy`) | 0,65 (0,1–1,15) | 0,4 (0–0,85) | 0,25 (0–0,75) | 0,3 (0–1,05) | 0,1 (0–0,8) | 0,05 (0–0,4) |
| Harika (`wonder`) | 0 (0–0,05) | 0,25 (0,1–0,3) | 0,2 (0–0,4) | 0,15 (0–0,2) | 0 (0–0,2) | 0 (0–0,1) |

## Büyük olay türleri

Dünya başına yıllık büyük olay sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Savaş (`war`) | 0,4 (0–1,05) | 4,9 (0,15–7,65) | 5,85 (0,15–11,8) | 7,8 (0–13,3) | 7,6 (0–16,4) | 6,6 (0–13,4) |
| Araştırma (`research`) | 6,45 (5,1–7,5) | 14,3 (12–16,6) | 3,05 (1,55–4) | 0,4 (0–2,9) | 0,2 (0–1,15) | 0 (0–0,65) |
| İnşaat (`build`) | 2,1 (1,6–2,85) | 10,2 (6,85–15,1) | 6,1 (2,65–8,2) | 2,45 (0,6–4,3) | 0,95 (0,35–3,15) | 0,9 (0,3–1,45) |
| Kahraman (`hero`) | 0,6 (0,25–1) | 3,6 (1,9–4,35) | 1,9 (1,25–3,05) | 3,05 (1,3–4,85) | 3,25 (1,65–5,35) | 3,25 (2,4–6,25) |
| Sefer/ilan (`quest`) | 1,55 (1–1,9) | 4,3 (3–6,3) | 0,85 (0,05–2,95) | 1,1 (0,15–2,05) | 1,25 (0,35–2,8) | 1,65 (0,35–2,8) |
| Sınıf (`class`) | 1,8 (1,25–2,9) | 2,4 (1,6–3,75) | 1,95 (0,8–3,1) | 1,15 (0,35–2,2) | 1,1 (0,05–2,1) | 1,05 (0–1,85) |
| Deniz (`sea`) | 0,3 (0,1–0,6) | 2,3 (1,5–4,25) | 1,95 (0,9–4,2) | 1,55 (0,45–3,85) | 1,8 (0,45–4,5) | 1,55 (0,5–4,2) |
| Kamp (`lair`) | 2,1 (1,85–2,35) | 2,65 (1,7–4,1) | 0,9 (0,45–2,05) | 1,15 (0,35–1,55) | 1,15 (0,45–2,35) | 1,2 (0,45–2,05) |
| Han (`inn`) | 1,3 (1,15–1,55) | 1,9 (1,15–2,95) | 1,35 (0,85–1,9) | 1,2 (0,55–1,65) | 1,2 (0,8–1,95) | 1,15 (0,75–2,2) |
| Yerleşim (`settle`) | 2,4 (1,7–2,55) | 2,65 (2,2–3,1) | 1,25 (0,7–1,6) | 0,55 (0,3–1,25) | 0,55 (0,2–0,95) | 0,35 (0,1–0,85) |
| Baskın (`raid`) | 3,1 (2,3–3,5) | 2,75 (1,95–4,5) | 0,45 (0,05–1,55) | 0,3 (0,1–0,6) | 0,45 (0,15–1,05) | 0,4 (0,1–0,7) |
| Keşif (`discover`) | 2,45 (1,7–3,05) | 1,45 (0,75–2) | 0,9 (0,45–1,2) | 0,45 (0,1–0,85) | 0,3 (0–1,05) | 0,2 (0–1) |
| Ekonomi (`economy`) | 0 | 1 (0,7–1,8) | 1,75 (1,05–2,25) | 1 (0,45–1,65) | 0,4 (0–1,05) | 0,2 (0–0,4) |
| epitaph | 0,2 (0,1–0,6) | 0,9 (0,35–1,3) | 0,55 (0,05–1,15) | 0,8 (0,15–1,6) | 0,75 (0,2–1,8) | 0,85 (0,25–1,7) |
| Ölüm/terk (`death`) | 0,2 (0,1–0,6) | 0,9 (0,4–1,2) | 0,5 (0,05–1,1) | 0,8 (0,15–1,4) | 0,7 (0,2–1,95) | 0,85 (0,25–1,65) |
| Dünya (`world`) | 0,1 (0–0,35) | 0,35 (0,1–0,7) | 0,45 (0,15–0,65) | 0,7 (0,35–1,25) | 0,9 (0,6–1,3) | 0,8 (0,55–1,4) |
| Ticaret (`trade`) | 0,7 (0,2–1,45) | 0,85 (0,5–1,15) | 0,35 (0,1–1,05) | 0,25 (0–0,9) | 0,45 (0–1,9) | 0,2 (0–0,7) |
| Büyüme (`growth`) | 0,9 (0,65–1,05) | 1,2 (1,05–1,3) | 0,2 (0,1–0,3) | 0 (0–0,2) | 0 (0–0,15) | 0 (0–0,05) |
| Çağ (`era`) | 1,45 (1,25–1,65) | 0,7 (0,6–0,85) | 0,1 (0,05–0,3) | 0 (0–0,15) | 0 (0–0,05) | 0 |
| Temas (`contact`) | 0,9 (0,4–1,5) | 0,65 (0,25–0,85) | 0,3 (0,05–0,45) | 0,15 (0–0,35) | 0 (0–0,25) | 0,05 (0–0,15) |
| Gerginlik (`tension`) | 0,7 (0,3–1,35) | 0,5 (0,05–0,9) | 0,3 (0–0,8) | 0,35 (0–0,7) | 0,15 (0–0,7) | 0,05 (0–0,45) |
| Göç (`migration`) | 0,1 (0–0,35) | 0,2 (0,05–0,45) | 0,2 (0–0,3) | 0,15 (0,05–0,35) | 0,2 (0,05–0,45) | 0,2 (0,1–0,45) |
| Diplomasi (`diplomacy`) | 0,25 (0,1–0,75) | 0,2 (0–0,5) | 0,2 (0–0,5) | 0,2 (0–0,4) | 0,1 (0–0,5) | 0,05 (0–0,3) |
| Harika (`wonder`) | 0 (0–0,05) | 0,25 (0,1–0,3) | 0,2 (0–0,4) | 0,15 (0–0,2) | 0 (0–0,2) | 0 (0–0,1) |

## Muharebe türleri

Dünya başına yıllık muharebe sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Yağma akını (`plunder`) | 0 (0–0,3) | 0,95 (0–1,95) | 1,25 (0–2,6) | 1,7 (0–3,3) | 2,1 (0–4,4) | 1,85 (0–3,5) |
| Yapı baskını (canavar) (`extRaid`) | 1,8 (1,3–2,3) | 2,05 (1,2–3,05) | 0,3 (0,05–1,4) | 0,4 (0,1–0,65) | 0,4 (0,15–1,05) | 0,4 (0,1–0,75) |
| Kuşatma (`siege`) | 0 (0–0,15) | 0,8 (0–1) | 0,95 (0,05–2,05) | 1,2 (0–1,9) | 0,9 (0–1,9) | 0,9 (0–1,7) |
| Kamp saldırısı (`camp`) | 0,1 (0–0,3) | 1,8 (1,1–2,2) | 0,5 (0,1–1,2) | 0,5 (0,2–0,8) | 0,6 (0,2–1) | 0,55 (0,25–1,05) |
| Yerleşim baskını (canavar) (`raid`) | 1,55 (1,2–2,1) | 1,4 (0,65–1,75) | 0,1 (0–0,45) | 0,1 (0–0,35) | 0,2 (0–0,4) | 0,05 (0–0,4) |
| Korsan savaşı (`pirate`) | 0 | 0,05 (0–0,1) | 0,1 (0–0,3) | 0,15 (0–0,4) | 0,1 (0–0,45) | 0,15 (0–0,35) |
| Han baskını (canavar) (`innMonster`) | 0,15 (0,05–0,3) | 0 (0–0,2) | 0 | 0 | 0 | 0 |
| Yol pususu (`ambush`) | 0 (0–0,1) | 0 (0–0,15) | 0 (0–0,1) | 0 (0–0,05) | 0 (0–0,05) | 0 |
| Han baskını (medeniyet) (`innCiv`) | 0 | 0 (0–0,05) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,05) |
| Deniz savaşı (`naval`) | 0 | 0 | 0 (0–0,5) | 0 (0–0,55) | 0 (0–0,2) | 0 (0–0,3) |
| Kervan soygunu (`robbery`) | 0 | 0 | 0 | 0 | 0 | 0 (0–0,1) |

## Kamp türleri

Dünya başına yaşayan kamp (yıl sonu değerlerinin on yıllık ortalaması): dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Goblin (`goblin`) | 5,7 (5–6,4) | 2,5 (1,55–3,65) | 0,15 (0–0,8) | 0,05 (0–0,9) | 0,45 (0–1,2) | 0 (0–1) |
| Bugbear (`bugbear`) | 0,55 (0–0,65) | 0,7 (0,35–0,9) | 1 (0,3–1) | 1 (0,6–1) | 0,8 (0,4–1) | 0,95 (0,25–1) |
| Hobgoblin (`hobgoblin`) | 1,35 (1,15–1,6) | 2,25 (1,1–2,85) | 0,35 (0–3,2) | 0,4 (0–1,55) | 0,1 (0–2,1) | 0,1 (0–2,35) |
| Korsan (`pirate`) | 0 (0–0,1) | 0,6 (0,2–1,3) | 0,55 (0,25–1,05) | 0,6 (0,1–1,4) | 0,55 (0,1–1,85) | 0,6 (0,3–1,5) |

## Çağ dağılımı

Yaşayan medeniyetlerin çağlara dağılımı, bütün dünyalar (yıl sonu).

| Yıl | I Kamp | II Köy | III Kasaba | IV Krallık |
|---|---|---|---|---|
| 10 | %0,8 | %27 | %65 | %7 |
| 20 | %0,8 | %4 | %20 | %75 |
| 30 | %0 | %1,6 | %4,9 | %93 |
| 40 | %0 | %0 | %1,6 | %98 |
| 50 | %0 | %0 | %0 | %100 |
| 60 | %0 | %0 | %0 | %100 |

## Dünyalar

| Seed | Medeniyet | Yerleşim | Nüfus | Çöküş | Efsane | En yüksek Sv | Doğan / ölü kahraman | Ağaç bitişi (yıl, medyan) | Süre (sn) | Son hash |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | 8 | 93 | 4449 | 4 | 1 | 5 | 132 / 28 | 21,5 | 77,5 | `27d49e3ba98a2e1e` |
| 2 | 7 | 69 | 3912 | 2 | 1 | 5 | 99 / 18 | 18,5 | 64 | `e5c6bc9c5515d153` |
| 3 | 7 | 84 | 3674 | 7 | 0 | 5 | 122 / 49 | 17 | 64,6 | `5c65a3f8a09825ac` |
| 4 | 8 | 79 | 4867 | 0 | 0 | 5 | 84 / 12 | 17 | 68,1 | `40b5ce7608099c18` |
| 5 | 8 | 88 | 4306 | 15 | 2 | 8 | 160 / 71 | 17,5 | 71 | `96e1f49d8e4951b5` |
| 6 | 7 | 69 | 3957 | 0 | 2 | 6 | 117 / 40 | 23 | 51,9 | `1c4a1e2f2d92ea83` |
| 7 | 10 | 105 | 5015 | 14 | 2 | 8 | 155 / 56 | 22 | 70 | `d63478dd17a9a069` |
| 8 | 7 | 69 | 4075 | 4 | 1 | 7 | 107 / 20 | 18 | 52,6 | `4273630d90eab919` |
| 9 | 7 | 74 | 4528 | 7 | 2 | 7 | 120 / 28 | 18 | 36,9 | `605dc772b00e8aee` |
| 10 | 8 | 94 | 4093 | 13 | 6 | 8 | 190 / 70 | 21 | 45,8 | `8f91e7d868f2bbca` |
| 11 | 7 | 79 | 3375 | 4 | 0 | 8 | 144 / 65 | 21 | 36 | `cb8789e12d51e2f6` |
| 12 | 8 | 76 | 3610 | 4 | 0 | 10 | 125 / 41 | 20 | 35,6 | `124f0069ccbe36c4` |
| 13 | 8 | 83 | 4779 | 8 | 0 | 10 | 163 / 68 | 19,5 | 42,1 | `f0f93b6d8cc7dde6` |
| 14 | 6 | 69 | 3760 | 8 | 8 | 9 | 118 / 37 | 19 | 31,1 | `e4a7552dcee99c54` |
| 15 | 8 | 88 | 3868 | 7 | 1 | 7 | 138 / 52 | 25 | 38,8 | `05cbb8e13e0680d1` |
| 16 | 8 | 92 | 4037 | 8 | 3 | 8 | 133 / 54 | 20 | 35,9 | `71721793b1f47c81` |

Çöküşler:

- seed 1, 36. yıl (gün 4215): Rüzgâr Manastırı başkenti kaybetti: Sessiztepe (Kanlıdiş Kabileleri aldı)
- seed 1, 39. yıl (gün 4615): Rüzgâr Manastırı başkenti kaybetti: Çankule (Kanlıdiş Kabileleri aldı)
- seed 1, 41. yıl (gün 4866): Rüzgâr Manastırı başkenti kaybetti: Sisliyamaç (Kanlıdiş Kabileleri aldı)
- seed 1, 44. yıl (gün 5257): Rüzgâr Manastırı başkenti kaybetti: Dinginpınar (Kanlıdiş Kabileleri aldı)
- seed 2, 5. yıl (gün 547): Sınır Bekçileri yok oldu
- seed 2, 51. yıl (gün 6010): Çarkyıldız Akademisi başkenti kaybetti: Dişlivadi (Pulzırh Lejyonu aldı)
- seed 3, 15. yıl (gün 1732): Karaörs Derinlikleri başkenti kaybetti: Külçukur (Çarkyıldız Akademisi aldı)
- seed 3, 17. yıl (gün 2023): Karaörs Derinlikleri başkenti kaybetti: Kara Mihrap (Örsyürek Tapınak Klanı aldı)
- seed 3, 20. yıl (gün 2303): Karaörs Derinlikleri başkenti kaybetti: Sessizocak (Çarkyıldız Akademisi aldı)
- seed 3, 20. yıl (gün 2350): Karaörs Derinlikleri yok oldu
- seed 3, 26. yıl (gün 3097): Sınır Bekçileri yok oldu
- seed 3, 31. yıl (gün 3686): Güneştacı Krallığı başkenti kaybetti: Şafaktepe (Kanlıdiş Kabileleri aldı)
- seed 3, 37. yıl (gün 4324): Örsyürek Tapınak Klanı başkenti kaybetti: Demirçan (Kanlıdiş Kabileleri aldı)
- seed 5, 10. yıl (gün 1182): Karaörs Derinlikleri başkenti kaybetti: Kara Mihrap (Güneştacı Krallığı aldı)
- seed 5, 13. yıl (gün 1465): Karaörs Derinlikleri başkenti kaybetti: Külçukur (Güneştacı Krallığı aldı)
- seed 5, 15. yıl (gün 1727): Karaörs Derinlikleri başkenti kaybetti: Sessizocak (Güneştacı Krallığı aldı)
- seed 5, 17. yıl (gün 2008): Karaörs Derinlikleri yok oldu
- seed 5, 23. yıl (gün 2724): Kızılboynuz Soyu başkenti kaybetti: Közsaray (Kanlıdiş Kabileleri aldı)
- seed 5, 25. yıl (gün 2987): Kızılboynuz Soyu başkenti kaybetti: Kızılçayır (Pulzırh Lejyonu aldı)
- seed 5, 30. yıl (gün 3539): Tatlıçayır Loncası başkenti kaybetti: Kavşakpazar (Kanlıdiş Kabileleri aldı)
- seed 5, 42. yıl (gün 4931): Kızılboynuz Soyu başkenti kaybetti: Neşeliova (Kanlıdiş Kabileleri aldı)
- seed 5, 44. yıl (gün 5212): Lirsesi Şehirleri başkenti kaybetti: Şarkıdere (Pulzırh Lejyonu aldı)
- seed 5, 46. yıl (gün 5520): Tatlıçayır Loncası başkenti kaybetti: Gölgeçarşı (Kanlıdiş Kabileleri aldı)
- seed 5, 47. yıl (gün 5577): Telliköprü Boyu başkenti kaybetti: Telliköprü (Pulzırh Lejyonu aldı)
- seed 5, 49. yıl (gün 5839): Telliköprü Boyu yok oldu
- seed 5, 50. yıl (gün 5925): Tatlıçayır Loncası başkenti kaybetti: Gökyayla (Kanlıdiş Kabileleri aldı)
- seed 5, 54. yıl (gün 6452): Tatlıçayır Loncası başkenti kaybetti: Yeşilkaya (Kanlıdiş Kabileleri aldı)
- seed 5, 59. yıl (gün 7024): Tatlıçayır Loncası başkenti kaybetti: Yıldıztepe (Kanlıdiş Kabileleri aldı)
- seed 7, 24. yıl (gün 2822): Karaörs Derinlikleri başkenti kaybetti: Külçukur (Kızılboynuz Soyu aldı)
- seed 7, 26. yıl (gün 3063): Karaörs Derinlikleri başkenti kaybetti: Sessizocak (Pulzırh Lejyonu aldı)
- seed 7, 26. yıl (gün 3099): Karaörs Derinlikleri başkenti kaybetti: Zincirkaya (Kızılboynuz Soyu aldı)
- seed 7, 26. yıl (gün 3109): Yeşilyaprak Çemberi başkenti kaybetti: Sessizkoru (Karaörs Derinlikleri aldı)
- seed 7, 30. yıl (gün 3508): Karaörs Derinlikleri başkenti kaybetti: Kara Mihrap (Pulzırh Lejyonu aldı)
- seed 7, 30. yıl (gün 3592): Kızılboynuz Soyu başkenti kaybetti: Külçukur (Rüzgâr Manastırı aldı)
- seed 7, 32. yıl (gün 3822): Ayburç Boyu başkenti kaybetti: Ayburç (Karaörs Derinlikleri aldı)
- seed 7, 36. yıl (gün 4214): Özgür Boynuztepe başkenti kaybetti: Boynuztepe (Kızılboynuz Soyu aldı)
- seed 7, 39. yıl (gün 4645): Rüzgâr Manastırı başkenti kaybetti: Sessiztepe (Kanlıdiş Kabileleri aldı)
- seed 7, 43. yıl (gün 5102): Rüzgâr Manastırı başkenti kaybetti: Çankule (Kanlıdiş Kabileleri aldı)
- seed 7, 46. yıl (gün 5482): Kızılboynuz Soyu başkenti kaybetti: Sessizocak (Pulzırh Lejyonu aldı)
- seed 7, 50. yıl (gün 5905): Rüzgâr Manastırı başkenti kaybetti: Taşbasamak (Kanlıdiş Kabileleri aldı)
- seed 7, 50. yıl (gün 5998): Karaörs Derinlikleri yok oldu
- seed 7, 53. yıl (gün 6326): Rüzgâr Manastırı başkenti kaybetti: Kuzeyoba (Kanlıdiş Kabileleri aldı)
- seed 8, 14. yıl (gün 1573): Kızılboynuz Soyu başkenti kaybetti: Közsaray (Örsyürek Tapınak Klanı aldı)
- seed 8, 15. yıl (gün 1714): Kızılboynuz Soyu başkenti kaybetti: Alazvadi (Güneştacı Krallığı aldı)
- seed 8, 19. yıl (gün 2190): Kızılboynuz Soyu başkenti kaybetti: Kıvılcımlı (Pulzırh Lejyonu aldı)
- seed 8, 20. yıl (gün 2351): Kızılboynuz Soyu yok oldu
- seed 9, 10. yıl (gün 1104): Sınır Bekçileri başkenti kaybetti: Gözcüağaç (Karaörs Derinlikleri aldı)
- seed 9, 16. yıl (gün 1894): Pulzırh Lejyonu başkenti kaybetti: Pulkalkan (Kanlıdiş Kabileleri aldı)
- seed 9, 19. yıl (gün 2170): Pulzırh Lejyonu yok oldu
- seed 9, 20. yıl (gün 2357): Karaörs Derinlikleri başkenti kaybetti: Gözcüağaç (Güneştacı Krallığı aldı)
- seed 9, 23. yıl (gün 2641): Karaörs Derinlikleri başkenti kaybetti: Alacayayla (Güneştacı Krallığı aldı)
- seed 9, 25. yıl (gün 2906): Karaörs Derinlikleri başkenti kaybetti: Gölgeörs (Güneştacı Krallığı aldı)
- seed 9, 26. yıl (gün 3021): Karaörs Derinlikleri yok oldu
- seed 10, 12. yıl (gün 1387): Tatlıçayır Loncası başkenti kaybetti: Kavşakpazar (Karaörs Derinlikleri aldı)
- seed 10, 18. yıl (gün 2113): Karaörs Derinlikleri başkenti kaybetti: Sessizocak (Kanlıdiş Kabileleri aldı)
- seed 10, 20. yıl (gün 2323): Çarkyıldız Akademisi başkenti kaybetti: Pusulakule (Kanlıdiş Kabileleri aldı)
- seed 10, 31. yıl (gün 3647): Çarkyıldız Akademisi başkenti kaybetti: Dişlivadi (Kanlıdiş Kabileleri aldı)
- seed 10, 35. yıl (gün 4106): Sınır Bekçileri başkenti kaybetti: Okyayı (Kanlıdiş Kabileleri aldı)
- seed 10, 37. yıl (gün 4376): Sınır Bekçileri başkenti kaybetti: İzsürer (Kanlıdiş Kabileleri aldı)
- seed 10, 40. yıl (gün 4766): Sınır Bekçileri başkenti kaybetti: Gözcüağaç (Kanlıdiş Kabileleri aldı)
- seed 10, 41. yıl (gün 4815): Kanlıdiş Kabileleri başkenti kaybetti: İzsürer (Örsyürek Tapınak Klanı aldı)
- seed 10, 44. yıl (gün 5196): Sınır Bekçileri başkenti kaybetti: Gökyurt (Kanlıdiş Kabileleri aldı)
- seed 10, 51. yıl (gün 6042): Sınır Bekçileri başkenti kaybetti: Karlıçayır (Kanlıdiş Kabileleri aldı)
- seed 10, 54. yıl (gün 6442): Sınır Bekçileri başkenti kaybetti: Ceylanoba (Kanlıdiş Kabileleri aldı)
- seed 10, 58. yıl (gün 6846): Sınır Bekçileri yok oldu
- seed 10, 60. yıl (gün 7188): Karaörs Derinlikleri başkenti kaybetti: Karlıtepe (Rüzgâr Manastırı aldı)
- seed 11, 43. yıl (gün 5084): Kızılboynuz Soyu başkenti kaybetti: Közsaray (Örsyürek Tapınak Klanı aldı)
- seed 11, 44. yıl (gün 5176): Rüzgâr Manastırı başkenti kaybetti: Sessiztepe (Kanlıdiş Kabileleri aldı)
- seed 11, 50. yıl (gün 5988): Kızılboynuz Soyu başkenti kaybetti: Alazvadi (Kanlıdiş Kabileleri aldı)
- seed 11, 57. yıl (gün 6799): Kızılboynuz Soyu başkenti kaybetti: Kızılçayır (Örsyürek Tapınak Klanı aldı)
- seed 12, 33. yıl (gün 3945): Sınır Bekçileri başkenti kaybetti: Gözcüağaç (Kızılboynuz Soyu aldı)
- seed 12, 39. yıl (gün 4664): Sınır Bekçileri başkenti kaybetti: Taşhisar (Kanlıdiş Kabileleri aldı)
- seed 12, 42. yıl (gün 5022): Sınır Bekçileri başkenti kaybetti: Okyayı (Kızılboynuz Soyu aldı)
- seed 12, 59. yıl (gün 6961): Yeşilyaprak Çemberi başkenti kaybetti: Sessizkoru (Kanlıdiş Kabileleri aldı)
- seed 13, 24. yıl (gün 2804): Kurtoba Serbest Şehri yok oldu
- seed 13, 36. yıl (gün 4232): Kanlıdiş Kabileleri başkenti kaybetti: Tuzyurt (Yeşilyaprak Çemberi aldı)
- seed 13, 39. yıl (gün 4601): Kanlıdiş Kabileleri başkenti kaybetti: Dumanköprü (Rüzgâr Manastırı aldı)
- seed 13, 50. yıl (gün 5893): Kanlıdiş Kabileleri başkenti kaybetti: Taşdere (Rüzgâr Manastırı aldı)
- seed 13, 51. yıl (gün 6090): Kanlıdiş Kabileleri başkenti kaybetti: Dumanköprü (Rüzgâr Manastırı aldı)
- seed 13, 52. yıl (gün 6220): Kanlıdiş Kabileleri başkenti kaybetti: Kuzeyoba (Yeşilyaprak Çemberi aldı)
- seed 13, 54. yıl (gün 6415): Kanlıdiş Kabileleri başkenti kaybetti: Tuzyayla (Sınır Bekçileri aldı)
- seed 13, 55. yıl (gün 6512): Kanlıdiş Kabileleri yok oldu
- seed 14, 16. yıl (gün 1878): Sınır Bekçileri başkenti kaybetti: Gözcüağaç (Karaörs Derinlikleri aldı)
- seed 14, 31. yıl (gün 3638): Karaörs Derinlikleri başkenti kaybetti: Çiyardı (Yeşilyaprak Çemberi aldı)
- seed 14, 33. yıl (gün 3918): Karaörs Derinlikleri başkenti kaybetti: Ceylanoba (Güneştacı Krallığı aldı)
- seed 14, 36. yıl (gün 4207): Karaörs Derinlikleri başkenti kaybetti: Sessizocak (Güneştacı Krallığı aldı)
- seed 14, 38. yıl (gün 4491): Karaörs Derinlikleri başkenti kaybetti: Gözcüağaç (Güneştacı Krallığı aldı)
- seed 14, 40. yıl (gün 4779): Karaörs Derinlikleri başkenti kaybetti: Karagöl (Güneştacı Krallığı aldı)
- seed 14, 43. yıl (gün 5046): Karaörs Derinlikleri başkenti kaybetti: Kara Mihrap (Güneştacı Krallığı aldı)
- seed 14, 43. yıl (gün 5078): Karaörs Derinlikleri yok oldu
- seed 15, 31. yıl (gün 3659): Kızılboynuz Soyu başkenti kaybetti: Kızılkül (Kanlıdiş Kabileleri aldı)
- seed 15, 35. yıl (gün 4165): Kızılboynuz Soyu başkenti kaybetti: Gökyayla (Güneştacı Krallığı aldı)
- seed 15, 38. yıl (gün 4534): Kızılboynuz Soyu başkenti kaybetti: Sisova (Kanlıdiş Kabileleri aldı)
- seed 15, 40. yıl (gün 4750): Kızılboynuz Soyu yok oldu
- seed 15, 46. yıl (gün 5414): Sınır Bekçileri başkenti kaybetti: Okyayı (Kanlıdiş Kabileleri aldı)
- seed 15, 50. yıl (gün 5956): Gölköprü Boyu başkenti kaybetti: Gölköprü (Sınır Bekçileri aldı)
- seed 15, 57. yıl (gün 6740): Güneştacı Krallığı başkenti kaybetti: Işıkdere (Kanlıdiş Kabileleri aldı)
- seed 16, 13. yıl (gün 1523): Güneştacı Krallığı yok oldu
- seed 16, 18. yıl (gün 2125): Kızılboynuz Soyu yok oldu
- seed 16, 41. yıl (gün 4919): Örsyürek Tapınak Klanı başkenti kaybetti: Kutsalörs (Kanlıdiş Kabileleri aldı)
- seed 16, 45. yıl (gün 5282): Sessizocak Klanı başkenti kaybetti: Sessizocak (Örsyürek Tapınak Klanı aldı)
- seed 16, 47. yıl (gün 5606): Sessizocak Klanı başkenti kaybetti: Karlıçayır (Örsyürek Tapınak Klanı aldı)
- seed 16, 48. yıl (gün 5732): Sessizocak Klanı yok oldu
- seed 16, 56. yıl (gün 6711): Rüzgâr Manastırı başkenti kaybetti: Çankule (Pulzırh Lejyonu aldı)
- seed 16, 59. yıl (gün 6973): Rüzgâr Manastırı başkenti kaybetti: Dinginpınar (Pulzırh Lejyonu aldı)

## Yıllık ayrıntı

Hücre: medyan (p10–p90), 16 dünya. Yıl y = (y−1)·120+1 … y·120. günler. Bütün değerler `report.json` içinde (`metrics`), dünya başına değerler `../runs/b1-only` altında.

### Medeniyet (1/4)

| Yıl | Yaşayan medeniyet | Yeni medeniyet (yeniden doğan) | Yok olan medeniyet | Başkent kaybı (medeniyet yaşarken) | Çöküş (yok olma + başkent kaybı) | Yaşayan yerleşim |
|---|---|---|---|---|---|---|
| 1 | 8 (7–9) | 0 | 0 | 0 | 0 | 8 (7–9) |
| 2 | 8 (7–9) | 0 | 0 | 0 | 0 | 8 (7–9) |
| 3 | 8 (7–9) | 0 | 0 | 0 | 0 | 8 (7–9) |
| 4 | 8 (7–9) | 0 | 0 | 0 | 0 | 8 (7–9) |
| 5 | 8 (7–9) | 0 | 0 | 0 | 0 | 10 (8–11) |
| 6 | 8 (7–9) | 0 | 0 | 0 | 0 | 13 (10–15,5) |
| 7 | 8 (7–9) | 0 | 0 | 0 | 0 | 17 (14–19,5) |
| 8 | 8 (7–9) | 0 | 0 | 0 | 0 | 22 (17–25,5) |
| 9 | 8 (7–9) | 0 | 0 | 0 | 0 | 27,5 (20,5–30,5) |
| 10 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 32 (24–34) |
| 11 | 8 (7–9) | 0 | 0 | 0 | 0 | 34 (27–37,5) |
| 12 | 8 (7–9) | 0 | 0 | 0 | 0 | 37,5 (29–42) |
| 13 | 8 (7–9) | 0 | 0 | 0 | 0 (0–0,5) | 40 (33–44,5) |
| 14 | 8 (7–9) | 0 | 0 | 0 | 0 | 43 (34,5–47,5) |
| 15 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 46 (36,5–50) |
| 16 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 49,5 (39,5–53,5) |
| 17 | 8 (7–9) | 0 | 0 | 0 | 0 (0–0,5) | 51 (42,5–56,5) |
| 18 | 8 (7–9) | 0 | 0 | 0 | 0 (0–0,5) | 53 (44–58) |
| 19 | 8 (7–9) | 0 | 0 | 0 | 0 (0–0,5) | 55 (47–59,5) |
| 20 | 8 (7–9) | 0 | 0 (0–0,5) | 0 (0–1) | 0 (0–1) | 56,5 (48–62) |
| 21 | 8 (7–9) | 0 | 0 | 0 | 0 | 58,5 (48–64) |
| 22 | 8 (7–9) | 0 | 0 | 0 | 0 | 59,5 (50–65,5) |
| 23 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 60,5 (51,5–66) |
| 24 | 8 (7–8,5) | 0 | 0 | 0 | 0 (0–0,5) | 62 (53–68,5) |
| 25 | 8 (7–8,5) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 63,5 (54,5–69,5) |
| 26 | 7,5 (7–8,5) | 0 | 0 (0–0,5) | 0 | 0 (0–1) | 63,5 (55,5–71,5) |
| 27 | 7,5 (7–8,5) | 0 | 0 | 0 | 0 | 64,5 (56–72) |
| 28 | 7,5 (7–8,5) | 0 | 0 | 0 | 0 | 66 (57–72,5) |
| 29 | 7,5 (7–8,5) | 0 | 0 | 0 | 0 | 67 (57–73) |
| 30 | 7,5 (7–8,5) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 67,5 (57,5–74,5) |
| 31 | 7,5 (7–8,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 68,5 (58–75,5) |
| 32 | 7,5 (7–8,5) | 0 | 0 | 0 | 0 | 70 (59–76,5) |
| 33 | 7,5 (7–8,5) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 70 (59,5–77,5) |
| 34 | 7,5 (7–8,5) | 0 | 0 | 0 | 0 | 70 (61–78) |
| 35 | 7,5 (7–8,5) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 70,5 (61,5–78,5) |
| 36 | 7,5 (7–8,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 70,5 (61,5–79,5) |
| 37 | 7,5 (7–8,5) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 71 (62–80,5) |
| 38 | 7,5 (7–8,5) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 72 (62,5–82) |
| 39 | 7,5 (7–8,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 72 (62,5–83) |
| 40 | 7,5 (7–8,5) | 0 | 0 | 0 (0–0,5) | 0 (0–1) | 72 (63–84,5) |
| 41 | 7,5 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 72,5 (63,5–85) |
| 42 | 7,5 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 73 (64–86) |
| 43 | 7,5 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 73 (64,5–86,5) |
| 44 | 7,5 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 73,5 (65–87) |
| 45 | 7,5 (7–9) | 0 | 0 | 0 | 0 | 74 (65,5–88) |
| 46 | 7,5 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 74 (65,5–88,5) |
| 47 | 7,5 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 74 (66–89) |
| 48 | 8 (7–9) | 0 | 0 | 0 | 0 | 74 (66,5–88) |
| 49 | 8 (7–9) | 0 | 0 | 0 | 0 | 74,5 (67–88,5) |
| 50 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 75,5 (68–89,5) |
| 51 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 76 (68–90) |
| 52 | 8 (7–9) | 0 | 0 | 0 | 0 | 76,5 (68–91) |
| 53 | 8 (7–9) | 0 | 0 | 0 | 0 | 76,5 (69–92) |
| 54 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 77 (69–91,5) |
| 55 | 8 (7–8,5) | 0 | 0 | 0 | 0 | 77,5 (69–92,5) |
| 56 | 8 (7–8,5) | 0 | 0 | 0 | 0 | 78 (69–93,5) |
| 57 | 8 (7–8,5) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 79 (69–93,5) |
| 58 | 8 (7–8) | 0 | 0 | 0 | 0 | 79,5 (69–93,5) |
| 59 | 8 (7–8) | 0 | 0 | 0 (0–1) | 0 (0–1) | 80,5 (69–93,5) |
| 60 | 8 (7–8) | 0 | 0 | 0 | 0 | 81 (69–93,5) |

### Medeniyet (2/4)

| Yıl | Medeniyet başına yerleşim | 5+ kara yerleşimli medeniyet payı | Kurulan yerleşim | Fethedilen yerleşim | Terk edilen yerleşim | Toplam nüfus |
|---|---|---|---|---|---|---|
| 1 | 1 | %0 | 0 | 0 | 0 | 65 (54,5–74,5) |
| 2 | 1 | %0 | 0 | 0 | 0 | 87 (71–96) |
| 3 | 1 | %0 | 0 | 0 | 0 | 110 (95,5–124) |
| 4 | 1 (1–1,13) | %0 | 0 (0–1) | 0 | 0 | 144 (122–165) |
| 5 | 1,22 (1,06–1,38) | %0 | 1 (0,5–3) | 0 | 0 | 188 (160–213) |
| 6 | 1,56 (1,27–2) | %0 | 3 (1,5–5) | 0 | 0 | 242 (206–272) |
| 7 | 2,06 (1,88–2,42) | %0 | 4 (2–5) | 0 | 0 | 316 (258–348) |
| 8 | 2,56 (2,36–3,13) | %0 | 5 (2,5–6) | 0 | 0 | 420 (311–465) |
| 9 | 3,28 (2,8–3,77) | %12 (%0–%29) | 5 (3–7) | 0 (0–1) | 0 | 530 (392–590) |
| 10 | 3,72 (3,27–4,19) | %35 (%22–%56) | 3,5 (2,5–5) | 0 (0–1) | 0 | 662 (499–745) |
| 11 | 4,11 (3,86–4,6) | %56 (%43–%62) | 4 (1,5–6) | 0 (0–1) | 0 | 781 (602–882) |
| 12 | 4,44 (4,14–5,13) | %67 (%56–%87) | 3 (1,5–4) | 0 (0–1) | 0 | 920 (698–1016) |
| 13 | 4,93 (4,54–5,44) | %71 (%57–%88) | 3 (1–4,5) | 0 (0–2) | 0 | 1052 (804–1150) |
| 14 | 5,2 (4,82–5,69) | %73 (%62–%88) | 3 (1–3,5) | 0 (0–1,5) | 0 | 1178 (939–1282) |
| 15 | 5,54 (5,06–6,06) | %78 (%65–%88) | 3 (2–4,5) | 1 (0–2) | 0 | 1332 (1039–1404) |
| 16 | 5,93 (5,46–6,4) | %86 (%73–%100) | 3 (2–4,5) | 0 (0–1,5) | 0 | 1466 (1157–1586) |
| 17 | 6,27 (5,73–6,81) | %88 (%78–%100) | 2 (1–4) | 1 (0–1,5) | 0 | 1590 (1264–1705) |
| 18 | 6,56 (6,17–7,19) | %87 (%78–%100) | 2 (1–3) | 0 (0–2) | 0 (0–0,5) | 1730 (1348–1854) |
| 19 | 6,87 (6,18–7,47) | %88 (%82–%100) | 2 (0–4) | 0 (0–1,5) | 0 | 1867 (1492–1996) |
| 20 | 7 (6,46–7,92) | %89 (%86–%100) | 1 (0,5–4) | 1 (0–2,5) | 0 | 1990 (1572–2074) |
| 21 | 7,24 (6,67–8) | %89 (%86–%100) | 2 (0–3) | 0 (0–1) | 0 | 2110 (1701–2225) |
| 22 | 7,35 (6,81–8,13) | %88 (%86–%100) | 1 (0–2) | 1 (0–3) | 0 | 2255 (1764–2332) |
| 23 | 7,63 (7–8,21) | %94 (%86–%100) | 1 (0,5–3) | 0,5 (0–2,5) | 0 | 2323 (1886–2472) |
| 24 | 8 (7,19–8,46) | %100 (%87–%100) | 2 (1–2,5) | 0,5 (0–3) | 0 | 2474 (1967–2576) |
| 25 | 8,07 (7,31–8,61) | %100 (%87–%100) | 1 (0–2) | 0 (0–2) | 0 | 2496 (2077–2703) |
| 26 | 8,27 (7,61–8,94) | %100 (%88–%100) | 1 (0–1) | 1 (0–2,5) | 0 | 2656 (2145–2764) |
| 27 | 8,46 (7,73–9) | %100 (%88–%100) | 1 (0–2) | 0 (0–2) | 0 | 2673 (2282–2888) |
| 28 | 8,66 (7,68–9,21) | %100 (%87–%100) | 1 (1–2) | 1 (0–3,5) | 0 | 2780 (2316–2981) |
| 29 | 8,74 (7,73–9,36) | %100 (%88–%100) | 1 (0–2) | 0,5 (0–2) | 0 | 2854 (2433–3104) |
| 30 | 8,81 (7,83–9,5) | %100 (%87–%100) | 1 (0–2) | 0,5 (0–2,5) | 0 | 2908 (2479–3182) |
| 31 | 8,94 (7,95–9,64) | %100 (%88–%100) | 1 (0–2) | 0 (0–2) | 0 | 2983 (2534–3288) |
| 32 | 9 (8,14–9,71) | %100 (%88–%100) | 1 (0–2,5) | 0,5 (0–2) | 0 | 3036 (2600–3352) |
| 33 | 9,2 (8,14–9,79) | %100 (%88–%100) | 1 (0–2) | 1 (0–2,5) | 0 | 3115 (2702–3465) |
| 34 | 9,25 (8,36–9,79) | %100 (%88–%100) | 0 (0–2) | 1 (0–2) | 0 (0–0,5) | 3182 (2792–3542) |
| 35 | 9,31 (8,43–9,79) | %100 (%88–%100) | 0 (0–1) | 0 (0–1,5) | 0 | 3226 (2842–3617) |
| 36 | 9,43 (8,5–9,8) | %100 (%88–%100) | 0 (0–1,5) | 1 (0–2) | 0 | 3291 (2902–3674) |
| 37 | 9,46 (8,5–9,94) | %100 (%87–%100) | 0,5 (0–1) | 0 (0–2,5) | 0 | 3350 (2958–3713) |
| 38 | 9,5 (8,57–9,94) | %100 (%86–%100) | 0,5 (0–1,5) | 1 (0–2) | 0 (0–0,5) | 3392 (2962–3742) |
| 39 | 9,63 (8,57–10,1) | %100 (%86–%100) | 1 (0–1) | 0,5 (0–3) | 0 | 3444 (3088–3808) |
| 40 | 9,71 (8,71–10,1) | %100 (%80–%100) | 0,5 (0–2) | 1 (0–2,5) | 0 | 3440 (3052–3878) |
| 41 | 9,61 (8,76–10,1) | %100 (%80–%100) | 0,5 (0–2) | 0 (0–2) | 0 (0–0,5) | 3595 (3164–3948) |
| 42 | 9,73 (8,76–10,3) | %100 (%84–%100) | 0 (0–1,5) | 1 (0–2) | 0 | 3556 (3198–3991) |
| 43 | 9,57 (8,72–10,6) | %100 (%80–%100) | 0 (0–1) | 1 (0–2) | 0 (0–1) | 3620 (3174–4022) |
| 44 | 9,64 (8,77–10,8) | %100 (%80–%100) | 0,5 (0–1) | 1 (0–1,5) | 0 | 3598 (3162–4098) |
| 45 | 9,36 (8,83–10,9) | %100 (%85–%100) | 1 (0–1) | 0,5 (0–3) | 0 | 3635 (3240–4108) |
| 46 | 9,38 (8,88–10,9) | %100 (%85–%100) | 0 (0–1) | 1 (0–2) | 0 | 3654 (3340–4212) |
| 47 | 9,44 (9–10,9) | %100 (%85–%100) | 0 (0–1) | 1 (0–3) | 0 | 3728 (3339–4254) |
| 48 | 9,71 (9,06–10,6) | %100 (%85–%100) | 0 (0–1) | 1 (0–2) | 0 (0–0,5) | 3734 (3346–4298) |
| 49 | 10,1 (9,13–10,8) | %100 (%85–%100) | 1 (0–1,5) | 0 (0–2) | 0 | 3799 (3432–4292) |
| 50 | 10,1 (9,3–10,9) | %100 (%83–%100) | 0,5 (0–1,5) | 1 (0–2,5) | 0 | 3874 (3404–4365) |
| 51 | 10,1 (9,36–10,9) | %100 (%82–%100) | 0 (0–1) | 1 (0–2) | 0 | 3829 (3474–4425) |
| 52 | 10,1 (9,36–11,1) | %100 (%82–%100) | 0 (0–1,5) | 0,5 (0–2) | 0 (0–0,5) | 3856 (3458–4422) |
| 53 | 10,2 (9,38–11,2) | %100 (%82–%100) | 0,5 (0–1) | 0,5 (0–2) | 0 | 3906 (3577–4540) |
| 54 | 10,3 (9,38–11,2) | %100 (%82–%100) | 0 (0–1,5) | 0 (0–2) | 0 (0–0,5) | 3975 (3542–4506) |
| 55 | 10,4 (9,61–11,4) | %100 (%87–%100) | 0 (0–1,5) | 1 (0–2) | 0 | 4056 (3594–4636) |
| 56 | 10,4 (9,61–11,5) | %100 (%87–%100) | 0 (0–1,5) | 0 (0–2) | 0 | 4050 (3572–4644) |
| 57 | 10,5 (9,68–11,5) | %95 (%86–%100) | 0 (0–1) | 0,5 (0–2) | 0 | 4064 (3674–4670) |
| 58 | 10,7 (9,8–11,6) | %100 (%86–%100) | 0 (0–1) | 0 (0–1) | 0 | 4084 (3598–4686) |
| 59 | 10,7 (9,8–11,7) | %100 (%86–%100) | 0 (0–1) | 1 (0–3) | 0 | 4116 (3666–4744) |
| 60 | 10,8 (9,86–11,7) | %100 (%87–%100) | 0 (0–1) | 0 (0–2) | 0 | 4056 (3642–4823) |

### Medeniyet (3/4)

| Yıl | Ortalama çağ | Araştırma ağacının biten payı (ort.) | Ağacı bitmiş medeniyet payı | Araştırması duran medeniyet payı | Altın medyanı (medeniyetler) | Boştaki iş gücü payı |
|---|---|---|---|---|---|---|
| 1 | 1 | %0,4 (%0–%0,8) | %0 | %0 | 8,36 (8,26–8,47) | %0 |
| 2 | 1 | %4 (%3,6–%4,3) | %0 | %0 | 24,8 (13,4–33,4) | %0 |
| 3 | 1,13 (1,06–1,31) | %8 (%7,6–%8,5) | %0 | %0 | 34,9 (20,8–47,9) | %0,2 (%0–%2,1) |
| 4 | 1,88 (1,78–2) | %12 (%11–%13) | %0 | %0 | 38,4 (30,3–53,9) | %0,3 (%0–%2,4) |
| 5 | 2 | %15 (%14–%16) | %0 | %0 | 51,8 (34,3–67,9) | %0,1 (%0–%1,5) |
| 6 | 2 | %19 (%17–%21) | %0 | %0 | 67,9 (44,1–87,4) | %0,2 (%0–%0,8) |
| 7 | 2,12 (2–2,25) | %24 (%22–%27) | %0 | %0 (%0–%13) | 65,9 (38,3–95,9) | %0,7 (%0,1–%2,1) |
| 8 | 2,38 (2,24–2,73) | %30 (%27–%32) | %0 | %0 (%0–%11) | 72,7 (45,9–102) | %1,1 (%0,4–%2,3) |
| 9 | 2,65 (2,44–2,81) | %35 (%33–%39) | %0 | %0 | 87 (47,9–133) | %1,1 (%0,6–%2,6) |
| 10 | 2,76 (2,63–2,94) | %42 (%39–%47) | %0 | %0 (%0–%13) | 108 (70,9–189) | %1,1 (%0,6–%3,8) |
| 11 | 3 (2,82–3,12) | %48 (%45–%52) | %0 | %12 (%0–%22) | 169 (109–280) | %2 (%0,7–%5,2) |
| 12 | 3 (2,87–3,29) | %54 (%51–%59) | %0 | %12 (%0–%22) | 162 (93,7–368) | %4 (%1,2–%7,8) |
| 13 | 3,14 (3,06–3,44) | %60 (%57–%65) | %0 (%0–%5,6) | %20 (%0–%38) | 249 (153–445) | %6,5 (%3,3–%9,3) |
| 14 | 3,29 (3,06–3,53) | %66 (%61–%70) | %0 (%0–%12) | %31 (%13–%46) | 408 (154–620) | %11 (%6,9–%14) |
| 15 | 3,38 (3,18–3,67) | %71 (%66–%74) | %12 (%0–%22) | %43 (%25–%53) | 573 (153–916) | %14 (%9,8–%17) |
| 16 | 3,5 (3,21–3,67) | %74 (%69–%79) | %18 (%5,6–%31) | %44 (%33–%62) | 755 (282–1187) | %21 (%16–%22) |
| 17 | 3,53 (3,29–3,73) | %77 (%72–%83) | %29 (%12–%47) | %56 (%43–%75) | 904 (242–1329) | %26 (%19–%30) |
| 18 | 3,63 (3,4–3,82) | %79 (%76–%85) | %40 (%24–%62) | %62 (%40–%88) | 984 (474–1680) | %31 (%25–%37) |
| 19 | 3,67 (3,38–3,94) | %83 (%77–%88) | %47 (%25–%62) | %67 (%46–%88) | 1624 (692–2034) | %33 (%25–%39) |
| 20 | 3,75 (3,4–3,94) | %84 (%78–%91) | %57 (%31–%75) | %75 (%56–%88) | 1952 (1111–2867) | %38 (%27–%42) |
| 21 | 3,75 (3,4–4) | %86 (%80–%93) | %57 (%44–%76) | %75 (%60–%94) | 2464 (1213–3530) | %40 (%33–%44) |
| 22 | 3,82 (3,54–4) | %88 (%81–%94) | %62 (%49–%86) | %82 (%65–%88) | 2705 (1669–4085) | %45 (%39–%49) |
| 23 | 3,86 (3,57–4) | %89 (%83–%95) | %75 (%56–%94) | %88 (%75–%100) | 3352 (1796–4605) | %47 (%37–%49) |
| 24 | 3,87 (3,57–4) | %91 (%83–%95) | %75 (%56–%100) | %88 (%75–%100) | 3577 (2159–5218) | %47 (%41–%53) |
| 25 | 3,88 (3,6–4) | %91 (%83–%96) | %80 (%65–%100) | %88 (%75–%100) | 4173 (2517–6637) | %49 (%43–%54) |
| 26 | 3,88 (3,69–4) | %93 (%86–%97) | %86 (%69–%100) | %94 (%86–%100) | 4747 (2620–6580) | %52 (%46–%54) |
| 27 | 3,88 (3,69–4) | %94 (%87–%97) | %88 (%73–%100) | %100 (%86–%100) | 5439 (3412–8739) | %52 (%47–%57) |
| 28 | 3,94 (3,69–4) | %94 (%88–%97) | %88 (%69–%100) | %100 (%86–%100) | 5821 (3241–8145) | %54 (%46–%56) |
| 29 | 4 (3,69–4) | %95 (%89–%97) | %87 (%69–%100) | %100 (%80–%100) | 6035 (4196–9875) | %52 (%45–%58) |
| 30 | 4 (3,75–4) | %95 (%89–%98) | %94 (%73–%100) | %100 (%88–%100) | 6491 (3877–10934) | %52 (%48–%58) |
| 31 | 4 (3,76–4) | %95 (%89–%98) | %100 (%73–%100) | %100 (%83–%100) | 8217 (4020–12160) | %54 (%45–%59) |
| 32 | 4 (3,86–4) | %96 (%90–%99) | %100 (%73–%100) | %100 (%79–%100) | 8747 (4822–13505) | %54 (%48–%58) |
| 33 | 4 (3,87–4) | %96 (%91–%99) | %100 (%72–%100) | %100 (%76–%100) | 9758 (5082–15556) | %56 (%46–%59) |
| 34 | 4 (3,87–4) | %96 (%92–%99) | %100 (%79–%100) | %100 (%87–%100) | 9896 (6052–17484) | %55 (%45–%59) |
| 35 | 4 (3,87–4) | %96 (%92–%99) | %100 (%86–%100) | %100 (%95–%100) | 10320 (6193–19411) | %55 (%47–%59) |
| 36 | 4 (3,87–4) | %96 (%93–%99) | %100 (%86–%100) | %100 (%94–%100) | 10827 (6448–22110) | %52 (%47–%58) |
| 37 | 4 (3,87–4) | %96 (%93–%99) | %100 (%86–%100) | %100 | 9252 (6700–20940) | %54 (%47–%57) |
| 38 | 4 (3,87–4) | %96 (%93–%99) | %100 (%86–%100) | %100 | 10235 (6445–22526) | %54 (%47–%56) |
| 39 | 4 (3,87–4) | %96 (%93–%99) | %100 (%86–%100) | %100 (%94–%100) | 11571 (5800–26381) | %51 (%46–%57) |
| 40 | 4 (3,94–4) | %96 (%94–%98) | %100 (%87–%100) | %100 (%88–%100) | 13295 (6540–25297) | %52 (%45–%56) |
| 41 | 4 (3,94–4) | %97 (%95–%98) | %100 (%88–%100) | %100 | 15065 (4904–26736) | %50 (%45–%55) |
| 42 | 4 (3,94–4) | %97 (%95–%98) | %100 (%88–%100) | %100 | 12329 (4651–28081) | %51 (%47–%55) |
| 43 | 4 (3,94–4) | %97 (%95–%98) | %100 (%88–%100) | %100 | 13188 (6111–29596) | %51 (%47–%55) |
| 44 | 4 (3,94–4) | %97 (%95–%99) | %100 (%94–%100) | %100 | 14484 (5009–31479) | %52 (%47–%56) |
| 45 | 4 (3,94–4) | %97 (%95–%99) | %100 (%89–%100) | %100 | 13458 (4410–33311) | %53 (%47–%55) |
| 46 | 4 (3,94–4) | %97 (%95–%99) | %100 (%89–%100) | %100 | 13878 (4795–34840) | %52 (%45–%55) |
| 47 | 4 | %97 (%95–%99) | %100 (%87–%100) | %100 (%89–%100) | 14908 (4881–36371) | %53 (%47–%54) |
| 48 | 4 | %97 (%96–%98) | %100 (%88–%100) | %100 (%94–%100) | 16130 (3941–38314) | %51 (%47–%55) |
| 49 | 4 | %98 (%96–%98) | %100 (%87–%100) | %100 (%88–%100) | 15812 (4634–39666) | %51 (%45–%54) |
| 50 | 4 | %98 (%96–%99) | %100 (%94–%100) | %100 (%94–%100) | 15253 (5830–41107) | %50 (%47–%55) |
| 51 | 4 | %98 (%96–%99) | %100 (%94–%100) | %100 (%94–%100) | 14132 (5605–42713) | %51 (%45–%55) |
| 52 | 4 | %98 (%96–%99) | %100 (%94–%100) | %100 (%94–%100) | 15840 (6885–43940) | %52 (%46–%54) |
| 53 | 4 | %98 (%97–%99) | %100 | %100 | 16416 (5859–43430) | %52 (%45–%55) |
| 54 | 4 | %98 (%97–%99) | %100 (%88–%100) | %100 (%88–%100) | 23581 (5592–45323) | %52 (%46–%54) |
| 55 | 4 | %98 (%97–%99) | %100 (%94–%100) | %100 (%94–%100) | 21251 (5971–47401) | %52 (%46–%54) |
| 56 | 4 | %98 (%97–%99) | %100 (%88–%100) | %100 (%88–%100) | 22315 (4527–49486) | %52 (%45–%55) |
| 57 | 4 | %98 (%97–%99) | %100 | %100 | 21441 (4370–51840) | %51 (%46–%56) |
| 58 | 4 | %98 (%97–%99) | %100 | %100 | 22716 (6138–54039) | %52 (%45–%55) |
| 59 | 4 | %99 (%97–%99) | %100 | %100 | 24020 (5244–56079) | %52 (%47–%56) |
| 60 | 4 | %99 (%97–%99) | %100 | %100 | 32176 (7538–57842) | %52 (%48–%55) |

### Medeniyet (4/4)

| Yıl | Bölünme (ayrılıp kurulan medeniyet) | En büyük medeniyetin yerleşimi |
|---|---|---|
| 1 | 0 | 1 |
| 2 | 0 | 1 |
| 3 | 0 | 1 |
| 4 | 0 | 1 (1–2) |
| 5 | 0 | 2 (1,5–2) |
| 6 | 0 | 3 (2–3) |
| 7 | 0 | 3 (3–4) |
| 8 | 0 | 4 |
| 9 | 0 | 5 (4–5) |
| 10 | 0 | 5 (5–6) |
| 11 | 0 | 6 (5–6) |
| 12 | 0 | 6 (5,5–7) |
| 13 | 0 | 7 (6–7,5) |
| 14 | 0 | 7 (7–8) |
| 15 | 0 | 8 (7–8,5) |
| 16 | 0 | 8,5 (7–9) |
| 17 | 0 | 9 (7,5–9) |
| 18 | 0 | 9 (8–10) |
| 19 | 0 | 9 (8,5–10) |
| 20 | 0 | 9 (8,5–11) |
| 21 | 0 | 9,5 (8,5–11) |
| 22 | 0 | 10 (9–11) |
| 23 | 0 | 10 (9–11,5) |
| 24 | 0 | 10,5 (9,5–12) |
| 25 | 0 | 11 (10–12) |
| 26 | 0 | 11 (10–13) |
| 27 | 0 | 11 (10–13) |
| 28 | 0 | 11 (10–14) |
| 29 | 0 | 12 (10–14,5) |
| 30 | 0 | 12 (10–15) |
| 31 | 0 | 12 (10–15) |
| 32 | 0 | 12 (10–15,5) |
| 33 | 0 | 12 (10–16,5) |
| 34 | 0 | 13 (10–17) |
| 35 | 0 | 13 (10–17,5) |
| 36 | 0 | 13 (10–18) |
| 37 | 0 | 13 (10,5–18,5) |
| 38 | 0 | 13 (11–19,5) |
| 39 | 0 | 13,5 (11–19,5) |
| 40 | 0 | 13,5 (11–20,5) |
| 41 | 0 | 13,5 (11–20) |
| 42 | 0 | 14 (11–21) |
| 43 | 0 | 14 (11–21) |
| 44 | 0 | 14 (11–21) |
| 45 | 0 | 14,5 (11–22) |
| 46 | 0 | 15 (11–22) |
| 47 | 0 | 15,5 (11–22,5) |
| 48 | 0 | 16 (11–22,5) |
| 49 | 0 | 15,5 (11–23) |
| 50 | 0 | 15,5 (11–23,5) |
| 51 | 0 | 16,5 (11–24) |
| 52 | 0 | 16,5 (11–24,5) |
| 53 | 0 | 17 (11–24,5) |
| 54 | 0 | 17 (11–25) |
| 55 | 0 | 17 (11–25,5) |
| 56 | 0 | 18 (11–25,5) |
| 57 | 0 | 18 (11–26) |
| 58 | 0 | 18 (11,5–26) |
| 59 | 0 | 17,5 (11,5–27,5) |
| 60 | 0 | 18,5 (11,5–27,5) |

### Olaylar

| Yıl | Olay | Büyük olay |
|---|---|---|
| 1 | 38 (29–44) | 15,5 (13,5–19) |
| 2 | 58 (44–69,5) | 19 (13–23) |
| 3 | 34 (26,5–41) | 6,5 (3–9,5) |
| 4 | 49 (40,5–57) | 16,5 (13–20) |
| 5 | 55,5 (51–65,5) | 22,5 (17–25,5) |
| 6 | 75,5 (58,5–93) | 29 (21–43,5) |
| 7 | 96 (80–125) | 37 (30,5–52,5) |
| 8 | 122 (87–140) | 49 (35–55,5) |
| 9 | 131 (100–149) | 54,5 (42,5–63) |
| 10 | 154 (108–181) | 55,5 (42,5–70,5) |
| 11 | 172 (132–222) | 63 (54,5–78,5) |
| 12 | 185 (134–243) | 73,5 (54–82,5) |
| 13 | 174 (146–240) | 67 (53–87) |
| 14 | 184 (132–240) | 69,5 (51–89) |
| 15 | 183 (146–262) | 68,5 (53,5–91) |
| 16 | 160 (134–246) | 59,5 (43–81,5) |
| 17 | 158 (138–204) | 58,5 (42–73,5) |
| 18 | 171 (122–222) | 51 (31,5–74) |
| 19 | 156 (106–212) | 48,5 (35,5–75) |
| 20 | 131 (103–208) | 39,5 (27,5–78,5) |
| 21 | 128 (81–192) | 41,5 (21–60) |
| 22 | 134 (89,5–176) | 39,5 (22,5–63,5) |
| 23 | 126 (82–184) | 38 (23–65,5) |
| 24 | 136 (76–178) | 35,5 (16,5–57,5) |
| 25 | 109 (62,5–150) | 27 (16,5–41,5) |
| 26 | 116 (68–159) | 28 (16–47,5) |
| 27 | 101 (67,5–130) | 24 (12,5–34) |
| 28 | 106 (68,5–134) | 25,5 (17,5–45,5) |
| 29 | 95,5 (65–130) | 21,5 (17–38,5) |
| 30 | 102 (64–152) | 27 (10,5–47) |
| 31 | 97 (76–150) | 30 (14,5–46,5) |
| 32 | 89 (55–154) | 20 (9–49,5) |
| 33 | 98,5 (64–147) | 26,5 (11–43,5) |
| 34 | 101 (58,5–136) | 23 (10,5–47,5) |
| 35 | 77 (48–138) | 19 (8,5–39,5) |
| 36 | 82,5 (60–153) | 26,5 (10,5–41) |
| 37 | 95,5 (59,5–136) | 21 (7,5–43) |
| 38 | 106 (52,5–144) | 25,5 (9–46,5) |
| 39 | 92 (53–155) | 19 (8–51,5) |
| 40 | 88,5 (37–152) | 28,5 (5,5–45,5) |
| 41 | 91 (42–160) | 24 (7–58) |
| 42 | 85,5 (41,5–155) | 20 (7,5–43,5) |
| 43 | 88,5 (46–164) | 21,5 (7–56) |
| 44 | 91 (49–143) | 27,5 (11,5–53) |
| 45 | 88,5 (38,5–137) | 24,5 (8–44,5) |
| 46 | 93 (40–156) | 20 (4,5–48,5) |
| 47 | 84 (55–171) | 23 (10–53) |
| 48 | 86 (61–155) | 23,5 (12–47) |
| 49 | 87 (49,5–137) | 20,5 (8–45,5) |
| 50 | 89 (53,5–170) | 26,5 (12–46,5) |
| 51 | 88,5 (49,5–110) | 21 (8–35) |
| 52 | 92,5 (54,5–154) | 27 (12–40,5) |
| 53 | 76,5 (51,5–126) | 16 (8,5–41) |
| 54 | 88,5 (34,5–148) | 26 (6,5–45) |
| 55 | 89,5 (40–151) | 21 (9,5–40,5) |
| 56 | 86 (46,5–138) | 17,5 (6,5–40) |
| 57 | 82,5 (55–130) | 21,5 (12,5–32) |
| 58 | 80,5 (46,5–119) | 16,5 (7–34,5) |
| 59 | 73 (40–154) | 18 (7,5–48,5) |
| 60 | 75,5 (53–126) | 16,5 (13,5–48,5) |

### Savaş (1/2)

| Yıl | Muharebe | Başlayan savaş | Süren savaş (yıl sonu) | Yıl içinde süren savaş | Yağma akını (medeniyet) | Tarihî hak savaşı |
|---|---|---|---|---|---|---|
| 1 | 0 | 0 | 0 | 0 | 0 | 0 |
| 2 | 0 | 0 | 0 | 0 | 0 | 0 |
| 3 | 0 (0–0,5) | 0 | 0 | 0 | 0 | 0 |
| 4 | 3 (2,5–4) | 0 | 0 | 0 | 0 | 0 |
| 5 | 4 (3–5,5) | 0 | 0 | 0 | 0 | 0 |
| 6 | 4,5 (2,5–6) | 0 | 0 | 0 | 0 | 0 |
| 7 | 5,5 (4–7) | 0 | 0 | 0 | 0 | 0 |
| 8 | 6 (4–7,5) | 0 | 0 | 0 | 0 (0–1) | 0 |
| 9 | 7 (5,5–9,5) | 0 (0–1) | 0 (0–1) | 0 (0–1) | 0 (0–1,5) | 0 |
| 10 | 9 (5,5–9) | 0 (0–1) | 0 | 0 (0–1) | 0 (0–1) | 0 |
| 11 | 8 (5–11) | 0 (0–2) | 0 | 0 (0–2) | 0,5 (0–2) | 0 |
| 12 | 10 (7,5–12) | 0 (0–1,5) | 0 (0–1) | 0 (0–1,5) | 0,5 (0–1,5) | 0 |
| 13 | 10 (5–12) | 1 (0–3) | 0 (0–2) | 1 (0–3) | 1 (0–2) | 0 (0–1) |
| 14 | 7 (5–11) | 0 (0–1,5) | 0 (0–1) | 0 (0–2,5) | 1 (0–2) | 0 |
| 15 | 7 (5–11) | 0,5 (0–3) | 0 | 1 (0–3) | 0,5 (0–2,5) | 0 |
| 16 | 6 (3–9,5) | 0 (0–2,5) | 0 | 0 (0–2,5) | 1 (0–2) | 0 |
| 17 | 6 (2–9,5) | 1 (0–2,5) | 0 (0–1) | 1 (0–2,5) | 1 (0–2,5) | 0 (0–0,5) |
| 18 | 5 (1,5–8) | 1 (0–2,5) | 0 (0–1,5) | 1 (0–3) | 1 (0–2,5) | 0 (0–0,5) |
| 19 | 5 (2–8,5) | 0 (0–2) | 0 (0–1,5) | 0,5 (0–2,5) | 1,5 (0–2) | 0 (0–1) |
| 20 | 4,5 (1,5–8) | 1 (0–3) | 0 (0–1) | 1 (0–3) | 0 (0–2,5) | 0 (0–0,5) |
| 21 | 4 (1–5) | 0 (0–1,5) | 0 (0–1,5) | 1 (0–2) | 1 (0–2) | 0 (0–0,5) |
| 22 | 4,5 (1–11) | 1 (0–3) | 0 (0–2) | 1 (0–3,5) | 1 (0–2,5) | 0 (0–0,5) |
| 23 | 3 (1–9,5) | 0 (0–3) | 0 (0–1) | 0,5 (0–4) | 2 (0–2,5) | 0 |
| 24 | 5 (0–7) | 0 (0–3) | 0 | 0,5 (0–3) | 2 (0–2,5) | 0 (0–0,5) |
| 25 | 3 (0–7) | 0 (0–3) | 0 (0–1) | 0,5 (0–3) | 2 (0–3) | 0 |
| 26 | 3 (0–11) | 1 (0–4,5) | 0 (0–1,5) | 1 (0–5) | 1 (0–3) | 0 (0–1) |
| 27 | 4 (0–6) | 1 (0–2,5) | 0 (0–2) | 1,5 (0–3,5) | 0,5 (0–3) | 0 (0–1) |
| 28 | 4 (0,5–7,5) | 0 (0–2,5) | 0 | 1 (0–4) | 2 (0–3) | 0 (0–0,5) |
| 29 | 4 (0,5–6,5) | 0 (0–2,5) | 0 (0–0,5) | 0,5 (0–2,5) | 1 (0–3,5) | 0 |
| 30 | 3,5 (0–8) | 0,5 (0–4) | 0 (0–1) | 1 (0–4) | 2 (0–3,5) | 0 (0–1) |
| 31 | 3 (0–7) | 1 (0–3,5) | 0 (0–1,5) | 1 (0–3,5) | 1 (0–3) | 0 (0–1) |
| 32 | 3 (0–8) | 0,5 (0–3) | 0 (0–2) | 1,5 (0–4) | 1 (0–3,5) | 0 (0–1) |
| 33 | 3,5 (0–8) | 0 (0–3,5) | 0 (0–1) | 1,5 (0–4) | 2 (0–3,5) | 0 (0–0,5) |
| 34 | 3 (0,5–7,5) | 1 (0–3,5) | 0 (0–1,5) | 1 (0–3,5) | 1 (0–3) | 0 (0–1) |
| 35 | 3 (0–6) | 0 (0–2,5) | 0 (0–1,5) | 1 (0–3) | 1,5 (0–3) | 0 (0–1) |
| 36 | 4 (0,5–7,5) | 1 (0–3,5) | 0 (0–1) | 1 (0–4) | 2 (0–4) | 0 (0–1) |
| 37 | 4,5 (0–7) | 1 (0–4,5) | 0 (0–2,5) | 1 (0–5,5) | 2 (0–3,5) | 0 (0–1) |
| 38 | 4 (1–7) | 1 (0–4) | 0 (0–1,5) | 2 (0–4,5) | 1 (0–3) | 0 (0–0,5) |
| 39 | 4 (0–10) | 0,5 (0–3,5) | 0 | 0,5 (0–5) | 1,5 (0–5) | 0 (0–0,5) |
| 40 | 3,5 (1–8,5) | 1 (0–4,5) | 0 (0–1) | 1 (0–4,5) | 2 (0–3) | 0 |
| 41 | 3,5 (0,5–8,5) | 0,5 (0–3,5) | 0 (0–1,5) | 1 (0–3,5) | 2 (0–5) | 0 (0–0,5) |
| 42 | 4 (1–8,5) | 1 (0–2,5) | 0 (0–1,5) | 1 (0–5) | 2 (0–3) | 0 (0–0,5) |
| 43 | 4,5 (0–7) | 1 (0–2,5) | 0 (0–0,5) | 1 (0–4) | 1,5 (0–4) | 0 (0–0,5) |
| 44 | 5,5 (1–8,5) | 1 (0–3) | 0 (0–1) | 1 (0–3) | 2 (0–4) | 0 (0–1) |
| 45 | 5 (0–8,5) | 0,5 (0–3) | 0 (0–1,5) | 1 (0–3) | 2 (0–4) | 0 |
| 46 | 3 (0,5–9) | 1 (0–4) | 0 (0–2) | 1 (0–4) | 1,5 (0–5,5) | 0 |
| 47 | 4 (1,5–10) | 1 (0–2,5) | 0 | 1 (0–5) | 2 (0–5) | 0 |
| 48 | 4,5 (1–7,5) | 0,5 (0–4) | 0 (0–0,5) | 0,5 (0–4) | 2,5 (0–4) | 0 (0–0,5) |
| 49 | 4 (0–9) | 0 (0–3) | 0 (0–1) | 0 (0–3) | 2 (0–5) | 0 (0–0,5) |
| 50 | 4 (1–7,5) | 1 (0–3) | 0 (0–1) | 1 (0–4) | 1,5 (0–4) | 0 (0–1) |
| 51 | 3 (1–7) | 0,5 (0–3,5) | 0 (0–3) | 1 (0–4) | 2 (0–3) | 0 |
| 52 | 4,5 (0,5–7,5) | 0 (0–3) | 0 (0–0,5) | 1 (0–4) | 2 (0–4) | 0 |
| 53 | 3,5 (1,5–8,5) | 0,5 (0–2) | 0 | 0,5 (0–2,5) | 1 (0–3,5) | 0 |
| 54 | 3,5 (1–7,5) | 0 (0–4) | 0 (0–2) | 0 (0–4) | 2 (0–4) | 0 (0–0,5) |
| 55 | 4 (0,5–8) | 0,5 (0–2) | 0 (0–1,5) | 1,5 (0–2,5) | 1,5 (0–3,5) | 0 |
| 56 | 3 (0,5–6,5) | 1 (0–3) | 0 (0–1,5) | 1 (0–4) | 1,5 (0–3,5) | 0 (0–0,5) |
| 57 | 3 (1–7) | 0 (0–3,5) | 0 (0–2,5) | 0,5 (0–4) | 1 (0–4) | 0 |
| 58 | 2,5 (0–5,5) | 0,5 (0–3) | 0,5 (0–2) | 1 (0–4) | 1 (0–3) | 0 (0–1) |
| 59 | 3,5 (0,5–7) | 0 (0–2,5) | 0 (0–1) | 1 (0–4) | 0,5 (0–4) | 0 |
| 60 | 3 (1–8) | 0 (0–3) | 0 (0–1) | 0 (0–3,5) | 1 (0–3) | 0 |

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
| 11 | 0 | 0 | 0 | 0 (0–1) |
| 12 | 0 | 0 | 0 | 0 (0–1) |
| 13 | 0 | 0 | 0 | 0 (0–1) |
| 14 | 0 | 0 | 0 | 0 (0–1) |
| 15 | 0 | 0 (0–0,5) | 0 | 0 (0–1) |
| 16 | 0 | 0 | 0 | 0 (0–1) |
| 17 | 0 | 0 | 0 | 0 (0–1) |
| 18 | 0 | 0 | 0 | 0 (0–1) |
| 19 | 0 | 0 | 0 | 0,5 (0–1) |
| 20 | 0 (0–1) | 0 | 0 | 0,5 (0–1) |
| 21 | 0 | 0 | 0 | 0,5 (0–1) |
| 22 | 0 (0–0,5) | 0 | 0 | 0,5 (0–1) |
| 23 | 0 | 0 | 0 | 0,5 (0–1) |
| 24 | 0 | 0 | 0 | 1 (0–1) |
| 25 | 0 (0–1) | 0 | 0 | 1 (0–1) |
| 26 | 0 | 0 | 0 | 1 (0–1) |
| 27 | 0 (0–1) | 0 | 0 | 1 (0–1) |
| 28 | 0 | 0 | 0 | 1 (0–1) |
| 29 | 0 (0–0,5) | 0 | 0 | 1 (0–1) |
| 30 | 0 (0–1,5) | 0 | 0 | 1 (0–1,5) |
| 31 | 0 (0–1) | 0 (0–0,5) | 0 | 1 (0–2) |
| 32 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 33 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 34 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 35 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 36 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 37 | 0 (0–1,5) | 0 | 0 | 1 (0–2) |
| 38 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 39 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 40 | 0 (0–1,5) | 0 (0–0,5) | 0 | 1 (0–2) |
| 41 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 42 | 0 | 0 | 0 | 1 (0–2) |
| 43 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 44 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 45 | 0 (0–0,5) | 0 | 0 (0–0,5) | 1 (0–2) |
| 46 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 47 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 48 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 49 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 50 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 51 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 52 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 53 | 0 | 0 | 0 | 1 (0–2) |
| 54 | 0 | 0 | 0 (0–0,5) | 0,5 (0–2) |
| 55 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 56 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 57 | 0 | 0 | 0 | 1 (0–2) |
| 58 | 0 | 0 | 0 | 1 (0–2) |
| 59 | 0 | 0 | 0 | 1 (0–2) |
| 60 | 0 | 0 | 0 | 1 (0–2) |

### Canavarlar

| Yıl | Yaşayan kamp (yıl sonu) | Yaşayan kamp (yıl ort.) | Doğan kamp | Temizlenen kamp | Canavar baskını |
|---|---|---|---|---|---|
| 1 | 3 | 3 | 0 | 0 | 0 |
| 2 | 3 (3–4,5) | 3 (3–3,44) | 0 (0–1,5) | 0 | 0 |
| 3 | 5 (4–5,5) | 4,05 (3,33–5,08) | 1 (0,5–2) | 0 | 0 (0–0,5) |
| 4 | 6 (5–8) | 5,15 (4,27–6,82) | 1 (0,5–3) | 0 | 3 (2–4) |
| 5 | 7,5 (6–10) | 6,97 (5,03–8,3) | 1 (0,5–2,5) | 0 | 4 (3–5,5) |
| 6 | 9 (8–10) | 8,65 (7,07–10) | 1 (0–3) | 0 | 4 (1,5–5,5) |
| 7 | 10 (9,5–10,5) | 9,89 (8,28–10,3) | 1 (0–2) | 0 | 5,5 (4–6,5) |
| 8 | 10 (9,5–10,5) | 10 (9,5–10,5) | 0 | 0 | 6 (3–7) |
| 9 | 10 (10–10,5) | 10 (9,71–10,5) | 0 (0–0,5) | 0 | 6 (5–8,5) |
| 10 | 10 (9,5–11) | 10 (9,51–11) | 0 (0–1) | 0 (0–1) | 6 (4–8) |
| 11 | 10 (8,5–11) | 9,97 (8,96–11) | 0 (0–1,5) | 1 (0–2) | 5,5 (3,5–9,5) |
| 12 | 9 (7–10,5) | 9,72 (6,77–10,6) | 1,5 (0–3) | 2 (1,5–3,5) | 6 (3,5–8,5) |
| 13 | 7 (6,5–8,5) | 7,94 (6,59–9,45) | 1 (0–2,5) | 2 (1–4) | 5,5 (2,5–7,5) |
| 14 | 6,5 (5,5–8,5) | 7,09 (6–7,85) | 1 (0–2) | 1 (0–3,5) | 3,5 (2–5) |
| 15 | 7 (4–8) | 6,9 (4,41–8,24) | 1 (0–2) | 2 (0,5–3) | 3 (1–5,5) |
| 16 | 6,5 (2,5–8) | 6,81 (3,22–7,92) | 0,5 (0–2) | 1 (0–2) | 3 (1–5) |
| 17 | 4,5 (2,5–8) | 5,04 (2,52–7,95) | 0 (0–2) | 2 (0–3,5) | 2,5 (0–4) |
| 18 | 3,5 (1–7,5) | 3,94 (2,2–7,89) | 0 (0–1) | 1 (0–2) | 2 (0–4,5) |
| 19 | 3 (2–7) | 3,08 (1,65–7,28) | 0 (0–1) | 1 (0–2) | 2 (0–4) |
| 20 | 3 (1–7) | 2,92 (1,11–6,9) | 0 (0–2) | 1 (0–2,5) | 1 (0–4) |
| 21 | 2,5 (1–7,5) | 2,8 (1–6,6) | 0 (0–2) | 0 (0–2) | 1 (0–2,5) |
| 22 | 2 (1–7) | 2,44 (1–6,75) | 0 (0–1) | 0 (0–2,5) | 1 (0–4) |
| 23 | 2,5 (1–5) | 2,38 (1–6,18) | 0 (0–1,5) | 1 (0–2,5) | 1 (0–2) |
| 24 | 2 (0,5–5) | 2,22 (0,57–5,2) | 0 (0–1,5) | 0 (0–1) | 0 (0–2) |
| 25 | 2 (1–5,5) | 1,75 (0,73–5,16) | 0 (0–1,5) | 0 (0–1,5) | 0 (0–1,5) |
| 26 | 2 (1–4,5) | 2 (1–5,1) | 0 (0–1) | 0 (0–1,5) | 0 (0–1,5) |
| 27 | 2 (1–5) | 2 (1–4,58) | 0 (0–1) | 0 (0–1) | 0,5 (0–1,5) |
| 28 | 2,5 (1–4,5) | 2,25 (1,02–4,9) | 0,5 (0–2) | 0 (0–1) | 1 (0–2) |
| 29 | 2,5 (1–4) | 2,45 (1,15–4,34) | 0 (0–1,5) | 0,5 (0–1) | 0 (0–2) |
| 30 | 2,5 (1,5–4) | 2,33 (1,25–4,3) | 0,5 (0–1) | 0 (0–1,5) | 0 (0–2,5) |
| 31 | 2 (2–4) | 2,56 (1,89–3,73) | 1 (0–1,5) | 0,5 (0–2) | 0 (0–1,5) |
| 32 | 2 (1,5–4) | 2,47 (1,86–3,75) | 0 (0–1) | 0 (0–1,5) | 0 (0–1,5) |
| 33 | 2 (1–4) | 2,25 (1,03–4) | 0 (0–1) | 0 (0–1) | 0 (0–1,5) |
| 34 | 2 (1–3) | 2,18 (1–3,42) | 0 (0–1) | 0 (0–2) | 0,5 (0–1) |
| 35 | 2 (2–3) | 2,02 (1,09–3,19) | 0 (0–1) | 0 (0–1) | 0 (0–1) |
| 36 | 2 (2–3) | 2,39 (2–3,2) | 0,5 (0–1) | 0 (0–1) | 0,5 (0–1,5) |
| 37 | 2 (1,5–3,5) | 2,14 (1,74–3,48) | 0 (0–1) | 0 (0–1,5) | 0 (0–1,5) |
| 38 | 2,5 (1–4,5) | 2,29 (1,31–4,06) | 0 (0–1,5) | 0 (0–1) | 0 (0–1) |
| 39 | 2 (1–4) | 2,03 (1,11–4,51) | 0 (0–1) | 0,5 (0–1,5) | 0 (0–1) |
| 40 | 2,5 (1–3,5) | 2,51 (0,72–4,03) | 0 (0–1) | 0 (0–1) | 0 (0–1) |
| 41 | 2,5 (1–5,5) | 2,53 (1,23–4,69) | 1 (0–1,5) | 0 (0–1) | 0 (0–1) |
| 42 | 3 (0,5–4,5) | 2,75 (0,65–4,94) | 0 (0–1) | 0 (0–1) | 0,5 (0–2) |
| 43 | 3 (1–4,5) | 3 (0,75–4,89) | 0,5 (0–1) | 0 (0–1) | 1 (0–2) |
| 44 | 3 (1,5–4) | 3 (1,23–4,56) | 1 (0–2) | 1 (0–1) | 1 (0–1,5) |
| 45 | 3 (1–4) | 3 (1,43–4,43) | 0,5 (0–2) | 1 (0–2) | 1 (0–2) |
| 46 | 3 (1–4,5) | 3 (1,23–4,37) | 0,5 (0–1,5) | 0 (0–1) | 0 (0–1,5) |
| 47 | 3 (1–4,5) | 3 (1–5,02) | 0 (0–1) | 0 (0–1,5) | 1 (0–2) |
| 48 | 2,5 (1–5) | 2,82 (1,37–4,57) | 0 (0–1) | 0 (0–1) | 1 (0–1,5) |
| 49 | 2,5 (1,5–4) | 2,64 (1,34–4,67) | 0 (0–1) | 0 (0–2) | 0,5 (0–2,5) |
| 50 | 3 (1,5–5) | 2,6 (1,5–4,19) | 0,5 (0–1,5) | 0 (0–1) | 1 (0–2) |
| 51 | 2 (1,5–5) | 2,58 (1,5–4,84) | 0 (0–1) | 0,5 (0–2) | 1 (0–1,5) |
| 52 | 2 (1,5–4,5) | 2 (1,83–4,9) | 0,5 (0–1,5) | 0 (0–1,5) | 0 (0–2) |
| 53 | 2,5 (1,5–3,5) | 2,31 (1,31–4,05) | 0 (0–1) | 0,5 (0–1) | 1 (0–1) |
| 54 | 2 (1–3,5) | 2,2 (1,32–4,03) | 0 (0–1) | 1 (0–1,5) | 0 (0–1) |
| 55 | 2 (2–3,5) | 2,09 (1,64–3,68) | 0,5 (0–1) | 0 (0–1) | 1 (0–1,5) |
| 56 | 2 (2–3,5) | 2,08 (2–3,65) | 0 (0–1) | 0 (0–1) | 0 (0–1) |
| 57 | 2 (1,5–3) | 2,39 (1,74–3,18) | 0 (0–1) | 0,5 (0–1) | 1 (0–1) |
| 58 | 2,5 (1–3) | 2,63 (1,08–3) | 0 (0–1,5) | 1 (0–1) | 0 (0–1) |
| 59 | 2,5 (1–5) | 2,5 (1–4,04) | 0 (0–2) | 0 (0–1) | 0 (0–1) |
| 60 | 2 (1–4) | 2,35 (1–5,07) | 0,5 (0–1) | 1 (0–1,5) | 1 (0–1,5) |

### Kahramanlar (1/2)

| Yıl | Doğan kahraman | Ölen kahraman | Emekli olan kahraman | Diyarı terk eden kahraman | Ölümden dönen kahraman | Efsane olan kahraman |
|---|---|---|---|---|---|---|
| 1 | 0 | 0 | 0 | 0 | 0 | 0 |
| 2 | 0 (0–2) | 0 | 0 | 0 | 0 | 0 |
| 3 | 1 (0–1) | 0 | 0 | 0 | 0 | 0 |
| 4 | 1 (0–1) | 0 (0–0,5) | 0 | 0 | 0 | 0 |
| 5 | 1 (0–2) | 0 (0–1) | 0 | 0 (0–0,5) | 0 | 0 |
| 6 | 1 (0–2) | 0 (0–1,5) | 0 | 0 | 0 | 0 |
| 7 | 1 (0–4) | 0 (0–0,5) | 0 | 0 | 0 | 0 |
| 8 | 2,5 (2–5,5) | 0 (0–2) | 0 | 0 | 0 | 0 |
| 9 | 2 (1–4,5) | 0 (0–1) | 0 | 0 | 0 | 0 |
| 10 | 2,5 (0,5–7) | 0,5 (0–2) | 0 | 0 | 0 | 0 |
| 11 | 3 (1,5–5) | 0 (0–2) | 0 | 0 | 0 | 0 |
| 12 | 4 (1,5–6,5) | 1 (0–2) | 0 | 0 | 0 | 0 |
| 13 | 3 (1,5–5,5) | 0 (0–3) | 0 | 0 | 0 | 0 |
| 14 | 2 (0,5–3,5) | 1,5 (0–3,5) | 0 | 0 | 0 | 0 |
| 15 | 2 (1–6,5) | 0 (0–3) | 0 | 0 | 0 | 0 |
| 16 | 2 (1–3) | 0,5 (0–1,5) | 0 | 0 | 0 | 0 |
| 17 | 1,5 (0–4,5) | 0 (0–2) | 0 | 0 | 0 | 0 |
| 18 | 2 (1–4) | 0 (0–2,5) | 0 | 0 | 0 | 0 |
| 19 | 2 (1–3,5) | 0 (0–2) | 0 | 0 | 0 | 0 |
| 20 | 2,5 (0–4) | 0 (0–2,5) | 0 | 0 | 0 | 0 |
| 21 | 1 (0–3,5) | 0 (0–2,5) | 0 | 0 (0–0,5) | 0 | 0 |
| 22 | 2 (0–3) | 0 (0–1,5) | 0 | 0 | 0 | 0 (0–0,5) |
| 23 | 2 (0,5–4) | 0 (0–2) | 0 | 0 (0–1) | 0 | 0 |
| 24 | 2 (1–4,5) | 0 (0–2,5) | 0 | 0 (0–0,5) | 0 | 0 |
| 25 | 2 (0,5–3,5) | 0 (0–1) | 0 | 0 (0–0,5) | 0 | 0 |
| 26 | 2 (1–3,5) | 0 (0–1) | 0 | 0 (0–0,5) | 0 | 0 |
| 27 | 2 (1–5,5) | 0 (0–3,5) | 0 | 0 (0–0,5) | 0 | 0 |
| 28 | 1,5 (1–2,5) | 0 (0–1) | 0 | 0 (0–1) | 0 | 0 |
| 29 | 1,5 (0–5) | 0 | 0 | 0 | 0 | 0 |
| 30 | 2 (1–4) | 0 (0–2) | 0 (0–0,5) | 0 (0–1) | 0 | 0 |
| 31 | 1 (1–3) | 0 (0–2) | 0 (0–1) | 0 (0–1) | 0 | 0 |
| 32 | 2 (0–3) | 0 (0–1,5) | 0 | 0 (0–1) | 0 | 0 |
| 33 | 2 (1–4) | 0 (0–3,5) | 0 | 0 (0–0,5) | 0 | 0 |
| 34 | 2 (0,5–4) | 0 (0–1,5) | 0 (0–1) | 0 (0–2) | 0 | 0 (0–0,5) |
| 35 | 1,5 (0,5–3) | 0 (0–3) | 0 (0–0,5) | 0 (0–1) | 0 | 0 |
| 36 | 2 (1–3,5) | 0 (0–2,5) | 0 (0–1) | 0 (0–1,5) | 0 | 0 |
| 37 | 2 (0–3,5) | 0 (0–3) | 0 (0–1,5) | 0 (0–1) | 0 | 0 |
| 38 | 2 (1–4,5) | 0 (0–2,5) | 0 (0–0,5) | 0 (0–1) | 0 | 0 |
| 39 | 1,5 (0,5–4,5) | 0 (0–6) | 0 (0–1) | 0,5 (0–1,5) | 0 | 0 |
| 40 | 1,5 (1–4,5) | 0 (0–3) | 0 (0–1,5) | 0 (0–1) | 0 | 0 |
| 41 | 2 (0–3,5) | 0,5 (0–2) | 0,5 (0–1,5) | 0 (0–1) | 0 | 0 |
| 42 | 1,5 (0–4,5) | 0 (0–1) | 0 (0–3) | 1 (0–1,5) | 0 | 0 |
| 43 | 2 (0–3) | 0 (0–4,5) | 0,5 (0–1,5) | 0 (0–1) | 0 | 0 |
| 44 | 2,5 (0,5–3,5) | 0 (0–4,5) | 1 (0–2) | 0 (0–2) | 0 | 0 |
| 45 | 2 (0–3,5) | 0 (0–2) | 1 (0–2) | 0 (0–1) | 0 | 0 |
| 46 | 2,5 (0,5–4,5) | 0 (0–4) | 1 (0–1,5) | 0 (0–1,5) | 0 | 0 |
| 47 | 2 (0–4) | 1 (0–2) | 1 (0,5–2,5) | 0 (0–1,5) | 0 | 0 |
| 48 | 2 (1–3) | 0 (0–2,5) | 0 (0–1,5) | 1 (0–1) | 0 | 0 |
| 49 | 2 (0–4) | 0 (0–2) | 0 (0–1,5) | 0 (0–1,5) | 0 | 0 |
| 50 | 3 (1–5) | 0 (0–3,5) | 1 (0–3,5) | 0 (0–2) | 0 | 0 |
| 51 | 2,5 (0,5–5) | 0 (0–2) | 1 (0–2) | 0,5 (0–1) | 0 | 0 (0–0,5) |
| 52 | 2 (1–4,5) | 1 (0–2,5) | 1 (0–1,5) | 1 (0–1) | 0 | 0 |
| 53 | 1,5 (0,5–3,5) | 0,5 (0–1) | 1 (0–2) | 0 | 0 | 0 |
| 54 | 3 (1–6) | 0 (0–3) | 1 (0–2) | 0 (0–2) | 0 | 0 |
| 55 | 2 (1–4) | 0 (0–2) | 1 (0,5–1,5) | 0 (0–1) | 0 | 0 |
| 56 | 2,5 (1–5,5) | 0 (0–1,5) | 1 (0–1,5) | 1 (0–2) | 0 | 0 |
| 57 | 2 (1–5,5) | 0 (0–2) | 1 (0–2,5) | 0,5 (0–1,5) | 0 | 0 |
| 58 | 3 (0–5,5) | 1 (0–1,5) | 0,5 (0–2) | 1 (0–3) | 0 | 0 |
| 59 | 3 (0,5–5) | 0 (0–4,5) | 1 (0–2) | 1 (0–2) | 0 | 0 |
| 60 | 2,5 (1–5) | 0,5 (0–3) | 0 (0–1) | 1 (0–1,5) | 0 | 0 |

### Kahramanlar (2/2)

| Yıl | Yaşayan kahraman (yıl sonu) | Doğuş seviyesi (ort.) | Ölüm seviyesi (ort.) | Yaşayan kahraman seviyesi (ort.) | En yüksek seviye (şimdiye dek) |
|---|---|---|---|---|---|
| 1 | 0 | 2 | – | 2 | 0 |
| 2 | 0 (0–2,5) | 1 (1–1,27) | – | 1 (1–1,57) | 0 (0–1,5) |
| 3 | 1 (0–2,5) | 1 (1–1,2) | – | 1 (1–1,4) | 1 (0–1,5) |
| 4 | 2 (0,5–3,5) | 1 (1–1,1) | 1,5 (1,1–1,9) | 1 (1–1,5) | 1 (0,5–2) |
| 5 | 3 (1–5) | 1,25 (1–2) | 1,5 (1,1–1,9) | 1 (1–1,6) | 1,5 (1–2) |
| 6 | 3 (2–5) | 1 (1–2) | 1,5 (1,1–1,9) | 1,29 (1–1,63) | 2 (1–2) |
| 7 | 4 (3–7,5) | 1,33 (1–2) | 1,5 (1,1–1,9) | 1,33 (1,06–1,67) | 2 (1,5–2) |
| 8 | 7 (4–10) | 1,5 (1–1,54) | 1 (1–1,5) | 1,39 (1,22–1,68) | 2 |
| 9 | 9,5 (4,5–13) | 1,33 (1–2) | 1,5 (1,1–1,9) | 1,39 (1,18–1,7) | 2 (2–2,5) |
| 10 | 11,5 (4,5–17,5) | 1,25 (1–1,62) | 1,25 (1–2) | 1,41 (1,23–1,73) | 2 (2–3) |
| 11 | 15 (7–21) | 1,4 (1–1,72) | 1,5 (1–2) | 1,55 (1,24–1,84) | 3 (2–3) |
| 12 | 16 (11,5–23) | 1,18 (1–1,75) | 1 (1–1,67) | 1,68 (1,34–1,97) | 3 (2–4) |
| 13 | 18,5 (13,5–26) | 1,33 (1–1,69) | 1,33 (1–2) | 1,78 (1,33–2,02) | 4 (2,5–4,5) |
| 14 | 20 (13,5–27,5) | 1,13 (1–1,5) | 1,25 (1–1,53) | 1,89 (1,49–2,22) | 4 (2,5–5) |
| 15 | 21 (16–31) | 1,25 (1–1,9) | 1,75 (1–2) | 2,03 (1,6–2,27) | 4 (3–5) |
| 16 | 22,5 (18–31) | 1,5 (1,17–2) | 1,5 (1–2,15) | 2,11 (1,65–2,37) | 4 (3–5) |
| 17 | 24,5 (18,5–31,5) | 1,23 (1–1,5) | 1 (1–2) | 2,15 (1,75–2,45) | 4 (3–5) |
| 18 | 25,5 (20,5–31,5) | 1,21 (1–2) | 2 (1–2,9) | 2,18 (1,85–2,57) | 4 (3–5) |
| 19 | 28 (22–32,5) | 1 (1–2) | 2 (1–3) | 2,25 (1,85–2,5) | 4,5 (3,5–5) |
| 20 | 27,5 (25–34,5) | 1,25 (1–1,87) | 1,5 (1–1,8) | 2,31 (1,97–2,59) | 4,5 (4–5) |
| 21 | 29 (26–33) | 1,29 (1–1,95) | 2 (1–2,75) | 2,35 (1,95–2,56) | 5 (4–5,5) |
| 22 | 30 (27,5–35) | 1,5 (1,02–2) | 2 (1,2–2,6) | 2,42 (1,97–2,59) | 5 (4–6) |
| 23 | 31 (29–37) | 1 (1–1,9) | 2 (1,2–2) | 2,44 (1,98–2,64) | 5 (4–6) |
| 24 | 33 (31–37) | 1,2 (1–2) | 1,25 (1–2,73) | 2,48 (2,05–2,67) | 5 (4–6) |
| 25 | 35,5 (31,5–39) | 1,5 (1–2,35) | 2,25 (1,15–3) | 2,46 (2,09–2,8) | 5 (4–6) |
| 26 | 36,5 (32–41) | 1,25 (1–1,5) | 2 (2–3,33) | 2,43 (2,11–2,84) | 5 (4–6,5) |
| 27 | 39,5 (31,5–43) | 1,5 (1–1,78) | 2 (1,2–2,43) | 2,5 (2,1–2,76) | 5 (4–6,5) |
| 28 | 40 (34–44) | 1,33 (1–2) | 2 (2–2,14) | 2,46 (2,08–2,79) | 5 (4–6,5) |
| 29 | 41,5 (35,5–46,5) | 1 (1–1,63) | – | 2,48 (2,09–2,88) | 5 (4–6,5) |
| 30 | 41,5 (38–49) | 1,33 (1–2) | 2,7 (1,25–5,5) | 2,33 (2,11–2,92) | 5 (4,5–6,5) |
| 31 | 42,5 (39–49) | 1,25 (1–2) | 2 (1,3–3) | 2,48 (2,16–2,92) | 5 (4,5–6,5) |
| 32 | 43 (38,5–51) | 1,5 (1–2) | 1,5 (1–3,2) | 2,5 (2,17–3,01) | 5 (4,5–7) |
| 33 | 45 (37,5–51) | 1,5 (1–1,87) | 2 (1,3–2,67) | 2,47 (2,17–3,06) | 5 (4,5–7) |
| 34 | 45 (38–54) | 1,33 (1–2) | 2,5 (2,1–3) | 2,54 (2,19–3,16) | 5,5 (4,5–7,5) |
| 35 | 45,5 (38,5–56,5) | 1,58 (1–2) | 2,5 (2–4,63) | 2,57 (2,17–3,2) | 5,5 (4,5–8) |
| 36 | 45,5 (39,5–56) | 1 (1–1,83) | 1,33 (1–3,53) | 2,55 (2,16–3,22) | 5,5 (4,5–8) |
| 37 | 46,5 (39,5–55) | 1,5 (1–1,63) | 2,9 (1,54–3,28) | 2,57 (2,1–3,24) | 5,5 (4,5–8) |
| 38 | 45,5 (43–57) | 1,5 (1–1,92) | 1,83 (1,55–2,7) | 2,54 (2,08–3,43) | 6 (4,5–8) |
| 39 | 46,5 (40,5–57,5) | 1,17 (1–1,68) | 2,25 (1,08–2,53) | 2,56 (2,1–3,43) | 6 (4,5–8) |
| 40 | 49,5 (40–58) | 1 (1–1,76) | 1,75 (1,2–2,8) | 2,59 (2,14–3,58) | 6 (4,5–8,5) |
| 41 | 48 (40–58) | 1,33 (1–1,94) | 2,75 (1–4,3) | 2,63 (2,2–3,46) | 6 (4,5–8,5) |
| 42 | 47 (38–60,5) | 1,5 (1–2) | 1,5 (1,1–1,9) | 2,54 (2,18–3,46) | 6,5 (4,5–9) |
| 43 | 47,5 (40–60,5) | 1,42 (1–1,97) | 2 (1–3,4) | 2,63 (2,13–3,56) | 6,5 (4,5–9) |
| 44 | 46,5 (39–59,5) | 1,42 (1–1,91) | 2 (1,7–2,93) | 2,65 (2,14–3,38) | 6,5 (4,5–9) |
| 45 | 46 (39,5–59,5) | 1,5 (1–2) | 2,25 (1,75–4,5) | 2,71 (2,14–3,36) | 6,5 (4,5–9) |
| 46 | 45,5 (37,5–59) | 1,29 (1–2) | 2,5 (2,25–6,27) | 2,75 (2,07–3,29) | 6,5 (5–9) |
| 47 | 46,5 (37,5–58,5) | 1,67 (1–2) | 2,25 (2–4,2) | 2,8 (2,08–3,3) | 7 (5–9) |
| 48 | 46,5 (38,5–62) | 1,5 (1–2,3) | 2 (2–4,33) | 2,84 (2,11–3,36) | 7 (5–9) |
| 49 | 47 (36,5–59,5) | 1,5 (1–1,63) | 2,92 (2–4) | 2,85 (2,13–3,41) | 7 (5–9,5) |
| 50 | 47 (36,5–59) | 1,5 (1,08–2) | 3 (2,72–4) | 2,77 (2,16–3,29) | 7 (5–9,5) |
| 51 | 47,5 (36–63) | 1,9 (1,28–2) | 2,33 (1,5–5) | 2,73 (2,19–3,27) | 7 (5–9,5) |
| 52 | 48,5 (36–61) | 1,5 (1–1,94) | 2 (1,4–5) | 2,8 (2,15–3,27) | 7 (5–9,5) |
| 53 | 49,5 (38–59,5) | 1,38 (1–2) | 2,6 (1–6,3) | 2,85 (2,2–3,2) | 7 (5–9,5) |
| 54 | 51,5 (38,5–60) | 1,43 (1–2,3) | 2,67 (1–3,25) | 2,76 (2,13–3,29) | 7 (5–9,5) |
| 55 | 51,5 (40–62,5) | 1,5 (1–2) | 2,33 (1,3–2,9) | 2,72 (2,2–3,22) | 7 (5–9,5) |
| 56 | 52,5 (42–61,5) | 1,5 (1–1,67) | 4 (3,09–4,8) | 2,72 (2,15–3,19) | 7,5 (5–9,5) |
| 57 | 55 (40,5–64) | 1,5 (1–1,92) | 4,13 (1,67–5) | 2,76 (2,1–3,12) | 7,5 (5–9,5) |
| 58 | 53,5 (39–65,5) | 1,45 (1,01–1,74) | 4 (2,2–5,6) | 2,75 (2,13–3,08) | 7,5 (5–9,5) |
| 59 | 53,5 (39–65,5) | 1,54 (1–2) | 3,5 (1,43–4,2) | 2,73 (2,09–3,06) | 7,5 (5–9,5) |
| 60 | 54 (38–66,5) | 1,67 (1,13–2,3) | 3,75 (3,08–8,3) | 2,74 (2,14–3,09) | 7,5 (5–9,5) |

### Han ve ticaret

| Yıl | Ayakta han | Asılan ilan | Biten ilan | Ticaret seferi (kervan) | İkmal seferi |
|---|---|---|---|---|---|
| 1 | 3 (2–4) | 0 (0–1) | 0 | 0 | 0 |
| 2 | 4 | 0 (0–1) | 0 | 0 | 0 |
| 3 | 4 | 0 (0–0,5) | 0 | 0 (0–0,5) | 0 |
| 4 | 4 (3–4) | 1 (0–3) | 0 | 0 (0–3,5) | 0 |
| 5 | 4 (2–4) | 2 (0,5–2,5) | 0 | 1,5 (0–4) | 0 |
| 6 | 3 (2–4) | 1 (0,5–2,5) | 0 | 3,5 (0–10) | 0 |
| 7 | 3 (2–4) | 2 (0,5–3) | 0 | 8 (2–18) | 0 |
| 8 | 3 (2–4) | 2 (0,5–3,5) | 0 | 14 (4–25,5) | 0 |
| 9 | 3 (2–4) | 2 (1–4) | 0 | 16,5 (5–34) | 0 |
| 10 | 3 (1–3,5) | 2 (1–3,5) | 0 | 20 (9–45) | 0 |
| 11 | 3 (1,5–4) | 2 (1–4,5) | 0 (0–1) | 30,5 (11,5–53,5) | 0 (0–1) |
| 12 | 3 (1,5–4) | 4 (2–6) | 0 (0–1,5) | 35 (13–56,5) | 0 (0–2) |
| 13 | 4 (2–4) | 2 (1–4,5) | 0 (0–1,5) | 38 (15–62) | 0,5 (0–3) |
| 14 | 4 (3–5) | 1,5 (0,5–5,5) | 0 (0–2) | 41 (16,5–63,5) | 2,5 (0–8) |
| 15 | 4,5 (3–5) | 2 (0,5–3,5) | 0 (0–1,5) | 42 (20–68,5) | 5,5 (1–10) |
| 16 | 5 (3,5–5,5) | 2 (0–3) | 0 (0–1,5) | 47 (22,5–75,5) | 8 (1,5–11,5) |
| 17 | 5 (3,5–5,5) | 1 (0–3) | 0 (0–1) | 46 (22,5–78) | 11 (2–15) |
| 18 | 5 (4–6) | 1 (0–4) | 0 (0–2) | 48 (26–73,5) | 11,5 (4–16) |
| 19 | 5 (4–5,5) | 1 (0–2,5) | 0 (0–1) | 50 (30,5–75,5) | 12 (3,5–19) |
| 20 | 5 (4–5,5) | 0,5 (0–2,5) | 0 (0–1) | 49 (34,5–76) | 14 (6,5–22) |
| 21 | 5 (4–6) | 0 (0–1) | 0 (0–1) | 53,5 (33,5–78,5) | 14 (7–23,5) |
| 22 | 5 (4,5–6) | 0 (0–1,5) | 0 (0–0,5) | 54 (32–85,5) | 13 (9,5–25,5) |
| 23 | 5 (4,5–6) | 0 (0–2) | 0 (0–1) | 59 (31,5–87,5) | 14,5 (9–24,5) |
| 24 | 5 (4,5–6) | 0 (0–2,5) | 0 (0–1) | 64,5 (32,5–88) | 16,5 (10,5–26,5) |
| 25 | 5 (5–6) | 0 (0–1) | 0 | 63 (34–86,5) | 16 (10–26,5) |
| 26 | 5 (5–6) | 0 (0–1) | 0 | 64,5 (34,5–88) | 18 (11,5–27) |
| 27 | 5 (5–6) | 0 (0–0,5) | 0 (0–1) | 63 (35,5–88,5) | 18 (12,5–26,5) |
| 28 | 5 (5–6) | 0,5 (0–1,5) | 0 (0–1) | 63 (36–94,5) | 18 (12,5–29) |
| 29 | 5 (5–6) | 0 (0–1) | 0 (0–1) | 65 (37–95) | 18,5 (12,5–29) |
| 30 | 5 (5–6) | 0 (0–2) | 0 (0–0,5) | 67,5 (36–92) | 21 (14,5–30,5) |
| 31 | 5 (5–6) | 0 (0–2) | 0 (0–0,5) | 70 (41,5–90,5) | 22 (13–31) |
| 32 | 5 (5–6) | 0 (0–1) | 0 (0–1) | 71,5 (39–89,5) | 23 (13–31) |
| 33 | 5 (5–6) | 0 (0–1) | 0 | 72 (39,5–90,5) | 24,5 (14,5–31,5) |
| 34 | 5 (5–6) | 0 (0–1,5) | 0 (0–1) | 71 (40,5–90) | 25 (15–31,5) |
| 35 | 5 (5–6) | 0 (0–1) | 0 | 71 (44,5–90) | 24 (16–33) |
| 36 | 5 (5–6) | 0 (0–1) | 0 | 71,5 (41–87,5) | 23,5 (16,5–33) |
| 37 | 5 (5–6) | 0 (0–1) | 0 (0–1) | 71,5 (44,5–96) | 24 (16,5–32) |
| 38 | 5 (5–6) | 0 (0–1) | 0 (0–0,5) | 70,5 (42–90,5) | 23,5 (15–30) |
| 39 | 5 (5–6) | 0 (0–1) | 0 (0–0,5) | 72 (41–89) | 23 (17–32,5) |
| 40 | 5 (5–6) | 0 (0–1) | 0 | 77,5 (41–98,5) | 24 (18,5–31,5) |
| 41 | 5 (5–6) | 0 (0–1) | 0 | 74 (38,5–101) | 24 (17,5–33,5) |
| 42 | 5 (5–6) | 0 (0–0,5) | 0 | 79 (39,5–102) | 23,5 (17,5–36) |
| 43 | 5 (5–6) | 0 (0–1) | 0 (0–1) | 76,5 (39,5–108) | 24,5 (18–36) |
| 44 | 5 (5–6) | 0,5 (0–2,5) | 0 (0–0,5) | 77 (42,5–116) | 22,5 (16,5–35) |
| 45 | 5 (5–6) | 1 (0–1,5) | 0 (0–1) | 81 (44,5–116) | 22,5 (17–38,5) |
| 46 | 5 (5–6) | 0 (0–1) | 0 (0–1) | 79,5 (42,5–132) | 22,5 (17,5–37) |
| 47 | 5 (5–6) | 0 (0–2) | 0 | 83,5 (43,5–132) | 25 (18–38,5) |
| 48 | 5 (5–6) | 0 (0–1) | 0 (0–1) | 84 (44,5–132) | 25,5 (18–37) |
| 49 | 5 (5–6) | 0,5 (0–1) | 0 (0–1) | 85 (45–124) | 25,5 (17–39) |
| 50 | 5 (5–6) | 0 (0–1) | 0 (0–1) | 80,5 (45–136) | 27,5 (16–40) |
| 51 | 5 (5–6) | 0 (0–2) | 0 (0–1) | 86 (44–136) | 27 (17,5–41,5) |
| 52 | 5 (5–6) | 0 (0–1) | 0 (0–1) | 81,5 (48–138) | 28 (18–38,5) |
| 53 | 5 (5–6) | 0 (0–1) | 0 | 85,5 (49,5–140) | 29 (20–41,5) |
| 54 | 5 (5–6) | 0,5 (0–1) | 0 (0–1) | 81 (51–133) | 27,5 (17–39,5) |
| 55 | 5 (5–6) | 0 (0–1) | 0 (0–0,5) | 84,5 (51–154) | 27 (17,5–41,5) |
| 56 | 5 (5–6) | 0 (0–1) | 0 (0–0,5) | 85 (52–154) | 29 (17,5–43,5) |
| 57 | 5 (5–6) | 0 (0–1) | 0 (0–0,5) | 80,5 (53–155) | 28 (19,5–42,5) |
| 58 | 5 (5–6) | 0 (0–1) | 0 (0–1) | 86 (47,5–144) | 28 (17,5–41) |
| 59 | 5 (5–6) | 0 (0–1,5) | 0 | 85,5 (54–149) | 29 (18–44) |
| 60 | 5 (5–6) | 0 (0–1) | 0 (0–0,5) | 89,5 (56–137) | 29 (17,5–45) |

