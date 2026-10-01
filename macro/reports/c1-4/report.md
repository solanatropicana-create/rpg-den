# Ölçüm raporu: c1-4

16 dünya (seed 1-16) × 60 yıl (7200 gün) · 2026-10-01 13:22 · `FD.Macro.Run stats --seeds 1-16 --years 60 --jobs 2 --verify 1 --saveload 1`

Süre: 7 dk 14 sn duvar saati, 2 iş parçacığı; dünya başına 48,9 sn (en az 36,8, en çok 59,2; yıl sonu hash'leri dâhil).

## Bitiş ölçütleri

DESIGN-FAZ1.md, "Bitiş ölçütleri". ✓ geçti · ✗ kaldı · — ölçülemedi.

| # | Ölçüt | Koşul | Ölçülen | Sonuç |
|---|---|---|---|---|
| 1 | Donma yok | 41–60. yılların yıllık büyük olay medyanı ≥ 0,8 × 6–20. yılların medyanı | 56 / 58 = 0,97 kat | ✓ |
| 2 | Çöküş | dünyaların ≥ %75'inde 60 yılda ≥ 1 çöküş (yok olma ya da başkent kaybı) | %88 (14/16 dünya); toplam 146 çöküş: 12 yok olma, 134 başkent kaybı | ✓ |
| 3 | Kamplar | 41–60. yıllarda yaşayan kamp medyanı ≥ 6–20. yılların medyanı | 10,2 ≥ 8,31 (yıl sonu sayımıyla 10 / 8) | ✓ |
| 4a | Kahraman: doğuş seviyesi | her on yılda doğanların ortalama seviyesi ≤ 2 | 1,38 · 1,31 · 1,33 · 1,38 · 1,42 · 1,45 (on yıllar sırasıyla) | ✓ |
| 4b | Kahraman: Sv8+ | dünyaların ≥ yarısında en az bir kahraman Sv8 ve üstüne çıkar | %94 (15/16 dünya); dünyadaki en yüksek seviye: medyan Sv8,5, en çok Sv10 | ✓ |
| 4c | Kahraman: efsane | dünya başına efsane medyanı 1–6 | medyan 3 (p10–p90: 2–6; toplam 64) | ✓ |
| 4d | Kahraman: ölüm payı | doğan kahramanların %30–80'i ölür | %44 (1356/3116); dünya medyanı %42 (%36–%50) | ✓ |
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
| Araştırma ağacı erken bitiyor | ~19. yılda bitiyor; 30. yılda medeniyetlerin %98'i bitirmiş | başlangıç medeniyetlerinde ağaç bitişi (son çağ, araştıracak düğüm yok) medyanı 21. yıl (125/128 bitirdi); 30. yılda bitirmiş medeniyet payı %91, araştırması duran (her çağ) %93; ağacın biten payı 19. yılda %84, 30. yılda %96 | evet |
| Medeniyetler 5 yerleşimde takılıyor | 98 medeniyetin 74'ü (%76) tam 5 yerleşimde | tam 5 kara yerleşimli medeniyet payı 30. yılda %5,5, 60. yılda %3,4 (denizaşırı koloniler dâhil 30. yılda tam 5: %3,1, 5+: %97); medeniyet başına 8,88 yerleşim (30. yıl) | hayır |
| Kamp sayısı düşüyor | 6,8'den 3,5'e iniyor | 3 (1. yıl) → en yüksek 12 (59. yıl) → 9 (30. yıl) → 12 (60. yıl); yıl sonu, yıllık dünya medyanı | hayır |
| Altın birikiyor | altın medyanı 78'den 6.503'e çıkıyor | 8,42 (1. yıl) → 4682 (30. yıl) → 5585 (60. yıl) | evet |
| İş gücü boşta | iş gücünün %43'ü boşta | 30. yılda %50, 60. yılda %47 (işe yerleşemeyen `zanaatçı` / bütün iş gücü, askerler dâhil; yıl içi ortalama) | evet |
| Büyük olaylar seyreliyor | yıllık büyük olay 59'dan 28'e düşüyor | en yüksek 78 (12. yıl) → 54 (30. yıl) → 66 (60. yıl), yıllık dünya medyanı | hayır |
| Doğuş seviyesi şişiyor | 24. yıldan sonra herkes Sv5 doğuyor; efsane mekaniği ölü | 25–60. yıllarda Sv5+ doğanların payı %0; on yıllık doğuş seviyesi ortalaması 1,38 · 1,31 · 1,33 · 1,38 · 1,42 · 1,45 | hayır |
| Başkent düşmüyor | başkent fethedilemiyor (agents.ts:673) | 16 dünyada 134 başkent kaybı, 12 yok olma; 932 yerleşim fethi | hayır |

## On yıllık özet

Hücre: dünyalar arası medyan (p10–p90). Her dünyada on yılın yıllık değerlerinin ortalaması alınır: akış ölçülerinde yıllık ortalama, stok ölçülerinde yıl sonu değerlerinin ortalaması. Yüzdeler 0–1 paylardır.

| Ölçü | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| **Medeniyet** | | | | | | |
| Yaşayan medeniyet | 8 (7–9) | 8 (7–9) | 8 (7–9) | 8 (7–9,4) | 8,45 (7–10) | 9 (7,05–10,9) |
| Yeni medeniyet (yeniden doğan) | 0 | 0 | 0 (0–0,1) | 0 (0–0,1) | 0,1 (0–0,15) | 0,05 (0–0,1) |
| Yok olan medeniyet | 0 | 0 (0–0,1) | 0 (0–0,05) | 0 | 0 (0–0,1) | 0 (0–0,05) |
| Başkent kaybı (medeniyet yaşarken) | 0 (0–0,05) | 0 (0–0,3) | 0,1 (0–0,25) | 0,1 (0–0,4) | 0,1 (0–0,5) | 0,1 (0–0,5) |
| Çöküş (yok olma + başkent kaybı) | 0 (0–0,1) | 0 (0–0,35) | 0,1 (0–0,3) | 0,1 (0–0,4) | 0,1 (0–0,55) | 0,15 (0–0,5) |
| Yaşayan yerleşim | 15,2 (12,8–17,3) | 48,6 (40,5–52,7) | 65,9 (57,2–75,1) | 75,6 (64,7–84,4) | 83,4 (66,6–92,4) | 86,1 (70,6–98,5) |
| Medeniyet başına yerleşim | 1,94 (1,69–2,04) | 6 (5,56–6,31) | 8,21 (7,8–8,47) | 9,11 (8,53–10) | 9,23 (8,27–10,6) | 9,6 (8,26–11,1) |
| 5+ kara yerleşimli medeniyet payı | %5,6 (%2,9–%8,3) | %84 (%69–%93) | %97 (%89–%100) | %98 (%87–%100) | %88 (%84–%100) | %87 (%78–%97) |
| Kurulan yerleşim | 2,3 (1,95–2,65) | 2,6 (2,15–3,15) | 1,25 (0,85–1,75) | 0,85 (0,45–1,25) | 0,7 (0,4–1,2) | 0,5 (0,25–1,55) |
| Fethedilen yerleşim | 0 (0–0,2) | 0,8 (0,35–1,2) | 1,1 (0,6–1,55) | 1,2 (0,6–1,8) | 1,45 (0,6–2) | 1,35 (0,25–2,35) |
| Terk edilen yerleşim | 0 | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,3) | 0,15 (0,05–0,35) | 0,15 (0–1,05) |
| Toplam nüfus | 277 (215–312) | 1425 (1090–1521) | 2471 (2078–2699) | 3109 (2747–3491) | 3570 (3142–4133) | 4049 (3385–4626) |
| Ortalama çağ | 1,9 (1,83–1,94) | 3,45 (3,19–3,53) | 3,94 (3,78–3,99) | 4 (3,97–4) | 4 | 4 |
| Araştırma ağacının biten payı (ort.) | %19 (%18–%20) | %71 (%66–%73) | %93 (%90–%95) | %96 (%94–%99) | %97 (%96–%99) | %97 (%96–%99) |
| Ağacı bitmiş medeniyet payı | %0 | %19 (%11–%26) | %81 (%65–%90) | %99 (%88–%100) | %98 (%93–%100) | %100 (%93–%100) |
| Araştırması duran medeniyet payı | %0 (%0–%2,4) | %45 (%37–%52) | %89 (%78–%94) | %99 (%92–%100) | %98 (%93–%100) | %100 (%93–%100) |
| Altın medyanı (medeniyetler) | 52,1 (42,6–78) | 903 (578–1082) | 3759 (2418–5329) | 5245 (3785–12401) | 5941 (3576–20393) | 5669 (4545–30484) |
| Boştaki iş gücü payı | %0,7 (%0,3–%0,9) | %17 (%14–%21) | %45 (%42–%49) | %49 (%47–%53) | %48 (%42–%54) | %48 (%43–%54) |
| Bölünme (ayrılıp kurulan medeniyet) | 0 | 0 | 0 (0–0,1) | 0 (0–0,1) | 0,1 (0–0,15) | 0,05 (0–0,1) |
| En büyük medeniyetin yerleşimi | 2,65 (2,4–2,8) | 8,15 (7,25–8,55) | 11,7 (9,7–13,4) | 14,4 (12,6–17,4) | 16,7 (11,6–19,5) | 17,6 (12,6–20,8) |
| **Olaylar** | | | | | | |
| Olay | 82,6 (65,6–99,7) | 191 (158–219) | 169 (128–198) | 152 (129–210) | 180 (144–226) | 196 (133–272) |
| Büyük olay | 30,4 (25,8–37,5) | 69,3 (56,4–76,5) | 51,7 (41,5–62,6) | 48,1 (36,5–62,7) | 54,3 (40,6–68,9) | 59,6 (37,3–75,3) |
| **Savaş** | | | | | | |
| Muharebe | 4,3 (3,25–4,75) | 8,4 (6,05–9,1) | 8,8 (6,45–10,4) | 9 (7,25–12,3) | 9,8 (7,7–12,8) | 9,65 (7,25–13,8) |
| Başlayan savaş | 0 (0–0,2) | 0,95 (0,35–1,5) | 1,35 (0,6–2,2) | 1,5 (0,7–2,35) | 1,8 (1–2,6) | 2 (0,55–3,1) |
| Süren savaş (yıl sonu) | 0 (0–0,1) | 0,2 (0,1–0,35) | 0,35 (0,1–0,75) | 0,4 (0,15–0,6) | 0,5 (0,3–1) | 0,85 (0,3–1,2) |
| Yıl içinde süren savaş | 0 (0–0,25) | 1,2 (0,5–1,8) | 1,75 (0,7–2,65) | 1,9 (0,9–2,85) | 2,3 (1,35–3,5) | 2,85 (1,3–4,3) |
| Yağma akını (medeniyet) | 0 (0–0,25) | 0,9 (0,1–1,5) | 1,65 (0,3–2,8) | 1,65 (0,1–3,6) | 2,1 (0–4,5) | 1,8 (0–5,05) |
| Tarihî hak savaşı | 0 | 0,1 (0–0,2) | 0,2 (0,05–0,3) | 0,15 (0–0,4) | 0,2 (0–0,5) | 0,1 (0–0,4) |
| Pakt gereği savaş | 0 | 0 (0–0,25) | 0,15 (0–0,55) | 0,1 (0–0,5) | 0,2 (0–0,55) | 0,15 (0–0,8) |
| Kutsal Sefer çağrısı | 0 | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) |
| İhanet (pakt çiğnendi) | 0 | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,15) | 0 (0–0,1) | 0 (0–0,05) |
| Savunma paktı (yıl sonu) | 0 | 0,15 (0–1,15) | 1 (0–2,05) | 1 (0–1,7) | 1 (0,05–1,95) | 1,2 (0,35–2,6) |
| **Canavarlar** | | | | | | |
| Yaşayan kamp (yıl sonu) | 7,45 (6,85–8,4) | 6,65 (6,2–9,2) | 8,1 (7,5–8,65) | 8,85 (8–9,85) | 9,9 (7,25–11,1) | 10,8 (7,85–12,6) |
| Yaşayan kamp (yıl ort.) | 7,12 (6,5–8,05) | 6,88 (6,27–9,35) | 7,89 (7,53–8,53) | 9,02 (8,15–9,84) | 9,83 (7,4–11) | 10,6 (7,75–12,5) |
| Doğan kamp | 0,75 (0,7–0,85) | 1,55 (0,95–1,95) | 1,8 (1,5–2,65) | 1,95 (1,3–2,45) | 2,2 (1,55–2,75) | 2,05 (1,35–2,75) |
| Temizlenen kamp | 0 (0–0,15) | 1,8 (1,45–2,3) | 1,7 (1,25–2,2) | 1,8 (1,3–2,65) | 2,1 (1,4–2,65) | 2 (1,15–2,65) |
| Canavar baskını | 3,65 (2,95–4,3) | 3,85 (3,05–5,05) | 3,55 (2,75–4,35) | 4 (2,95–4,85) | 3,4 (2,6–5,1) | 3,6 (2–5,6) |
| Yaşayan trol ini (yıl sonu) | 0 | 0,2 (0–0,45) | 0,7 (0,15–2,1) | 1,25 (0,5–1,95) | 1,5 (0,5–2,5) | 2,05 (1,2–2,8) |
| Yaşayan ejderha (yıl sonu) | 0 | 0,05 (0–0,3) | 1 (0,7–1) | 1 (0,5–1) | 1 (0,05–1) | 1 (0–1) |
| Ejderha akını | 0 | 0,05 (0–0,25) | 0,8 (0,5–1,15) | 0,7 (0,05–1) | 0,5 (0,05–0,8) | 0,15 (0–1,05) |
| Kriz (anlatıcı) | 0 (0–0,05) | 0,3 (0,1–0,35) | 0,3 (0,15–0,5) | 0,35 (0,1–0,55) | 0,4 (0,15–0,5) | 0,4 (0,1–0,55) |
| Rahatlama dönemi (anlatıcı) | 0,2 (0,1–0,3) | 0,1 (0–0,15) | 0,1 (0–0,2) | 0,1 (0–0,2) | 0,1 (0–0,2) | 0,1 (0–0,2) |
| **Kahramanlar** | | | | | | |
| Doğan kahraman | 1,35 (0,95–2,4) | 3 (1,9–3,7) | 3,7 (2,55–4,8) | 3,65 (2,9–4,95) | 3,75 (2,05–4,65) | 4,1 (2,3–5,05) |
| Ölen kahraman | 0,15 (0–0,6) | 1 (0,6–1,6) | 1,2 (0,55–1,95) | 1,5 (1,15–2,2) | 1,8 (1,1–3,05) | 2,1 (1,2–3,6) |
| Emekli olan kahraman | 0 | 0 | 0 (0–0,1) | 0,2 (0,05–0,5) | 0,7 (0,35–1,05) | 1,05 (0,6–1,2) |
| Diyarı terk eden kahraman | 0,05 (0–0,1) | 0 (0–0,05) | 0,1 (0–0,25) | 0,4 (0,05–0,65) | 0,65 (0,3–1,1) | 0,6 (0,1–1,1) |
| Ölümden dönen kahraman | 0 | 0 | 0 | 0 | 0 | 0 |
| Efsane olan kahraman | 0 | 0 (0–0,05) | 0,05 (0–0,2) | 0 (0–0,25) | 0,1 (0–0,3) | 0,1 (0–0,2) |
| Yaşayan kahraman (yıl sonu) | 4,4 (2,7–7,25) | 23 (14,1–28,5) | 45,3 (32,9–51,3) | 61,1 (53,9–72) | 72,8 (59,4–89,2) | 73,6 (63–86,3) |
| Doğuş seviyesi (ort.) | 1,38 (1,22–1,51) | 1,27 (1,15–1,41) | 1,33 (1,26–1,49) | 1,35 (1,24–1,44) | 1,45 (1,25–1,52) | 1,46 (1,32–1,62) |
| Ölüm seviyesi (ort.) | 1,33 (1–1,92) | 1,59 (1,11–1,88) | 2,24 (1,48–2,77) | 2,3 (1,8–3) | 2,63 (2,26–3,29) | 3,43 (2,67–3,87) |
| Yaşayan kahraman seviyesi (ort.) | 1,48 (1,14–1,72) | 1,98 (1,72–2,48) | 2,55 (2,14–2,79) | 2,87 (2,56–3,29) | 3,28 (2,92–3,59) | 3,43 (3,15–3,98) |
| En yüksek seviye (şimdiye dek) | 1,8 (1,15–2,05) | 3,75 (3,2–4,45) | 5,3 (4,25–6,15) | 6,9 (5,8–7,25) | 7,5 (6,8–8,6) | 8,15 (7,4–9,6) |
| **Han ve ticaret** | | | | | | |
| Ayakta han | 3,05 (2,85–3,75) | 3,95 (3,15–4,45) | 5 (4,65–5,5) | 5,1 (4,95–5,85) | 5,15 (5–5,95) | 5,35 (4,95–6,1) |
| Asılan ilan | 1,5 (1–1,9) | 2,6 (2,2–3,25) | 3,2 (2,1–4,45) | 3,35 (2,3–4,75) | 4 (2,25–5,35) | 3,05 (2,2–5,3) |
| Biten ilan | 0 (0–0,1) | 0,7 (0,3–0,9) | 0,5 (0,3–0,95) | 0,95 (0,5–1,7) | 1,5 (0,65–2,6) | 1,6 (0,5–2,05) |
| Ticaret seferi (kervan) | 5,8 (4,5–11,3) | 40,9 (26–66,3) | 69,3 (40,8–97,2) | 91,3 (47,4–107) | 98 (51,8–135) | 108 (53,3–177) |
| İkmal seferi | 0 | 6,7 (4,1–10,9) | 18 (13,5–23,5) | 23,5 (13,8–35,2) | 23 (12,7–43,4) | 30,2 (11,2–45,5) |

## Kahraman seviyeleri

### Doğuş seviyesi (bütün dünyalar, on yıl içinde doğanlar)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 |
|---|---|---|---|---|---|
| 1–10 | 251 | 1,38 | %64 | %35 | %1,6 |
| 11–20 | 461 | 1,31 | %69 | %30 | %0,9 |
| 21–30 | 590 | 1,33 | %68 | %31 | %1,2 |
| 31–40 | 617 | 1,38 | %64 | %34 | %1,9 |
| 41–50 | 571 | 1,42 | %62 | %35 | %3,5 |
| 51–60 | 626 | 1,45 | %59 | %36 | %4,6 |

### Ölüm seviyesi (bütün dünyalar, on yıl içinde ölenler)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 | Sv4 | Sv5 | Sv6 | Sv7 | Sv8 | Sv9 |
|---|---|---|---|---|---|---|---|---|---|---|---|
| 1–10 | 40 | 1,38 | %62 | %38 | %0 | %0 | %0 | %0 | %0 | %0 | %0 |
| 11–20 | 180 | 1,65 | %54 | %33 | %7,2 | %4,4 | %1,1 | %0 | %0 | %0 | %0 |
| 21–30 | 199 | 2,05 | %39 | %30 | %22 | %8,5 | %1 | %0,5 | %0 | %0 | %0 |
| 31–40 | 256 | 2,53 | %21 | %35 | %24 | %12 | %4,7 | %1,2 | %1,6 | %0 | %0 |
| 41–50 | 323 | 2,67 | %17 | %36 | %25 | %14 | %5 | %1,9 | %0,9 | %0,6 | %0 |
| 51–60 | 358 | 3,18 | %9,5 | %25 | %33 | %15 | %12 | %3,6 | %1,7 | %0,6 | %0,6 |

### Yaşayan kahramanların seviyesi (bütün dünyalar, on yılın son yılının sonunda)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 | Sv4 | Sv5 | Sv6 | Sv7 | Sv8 | Sv9 | Sv10 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 10 | 202 | 1,5 | %55 | %39 | %5,4 | %0 | %0 | %0 | %0 | %0 | %0 | %0 |
| 20 | 481 | 2,32 | %26 | %35 | %23 | %12 | %4 | %0 | %0 | %0 | %0 | %0 |
| 30 | 850 | 2,67 | %20 | %32 | %22 | %14 | %7,3 | %2,9 | %0,7 | %0 | %0 | %0 |
| 40 | 1108 | 3 | %16 | %30 | %20 | %15 | %11 | %4,8 | %2,1 | %0,5 | %0,1 | %0 |
| 50 | 1155 | 3,43 | %11 | %24 | %25 | %15 | %13 | %5,8 | %3,4 | %2,6 | %0,8 | %0,1 |
| 60 | 1218 | 3,47 | %12 | %24 | %23 | %14 | %13 | %5,3 | %4,4 | %2,1 | %1,6 | %0,2 |

### Ölüm nedenleri (bütün dünyalar)

| Neden | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 | Toplam |
|---|---|---|---|---|---|---|---|
| Kuşatma | 3 | 42 | 93 | 92 | 110 | 131 | 471 (%35) |
| Kamp saldırısı | 20 | 98 | 28 | 65 | 104 | 100 | 415 (%31) |
| Ejderha | 0 | 3 | 20 | 34 | 22 | 31 | 110 (%8,1) |
| Trol | 0 | 7 | 36 | 28 | 22 | 6 | 99 (%7,3) |
| Bilinmiyor | 0 | 0 | 0 | 1 | 10 | 40 | 51 (%3,8) |
| Yağma akını | 0 | 3 | 5 | 9 | 20 | 13 | 50 (%3,7) |
| Han baskını (canavar) | 14 | 7 | 3 | 5 | 7 | 9 | 45 (%3,3) |
| Suikast | 0 | 5 | 10 | 8 | 11 | 9 | 43 (%3,2) |
| Yol pususu | 0 | 4 | 0 | 6 | 7 | 6 | 23 (%1,7) |
| Kervan soygunu | 0 | 2 | 2 | 3 | 8 | 6 | 21 (%1,5) |
| Yerleşim baskını (canavar) | 3 | 7 | 1 | 0 | 0 | 0 | 11 (%0,8) |
| Han baskını (medeniyet) | 0 | 1 | 1 | 4 | 1 | 3 | 10 (%0,7) |
| Düello | 0 | 1 | 0 | 1 | 1 | 4 | 7 (%0,5) |

Neden, ölümün kaydedildiği andaki son muharebenin türünden (başlık ve taraflar) ya da suikast olayından çıkarılır.

## Olay türleri

Dünya başına yıllık olay sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Kahraman (`hero`) | 3,15 (2,35–5,75) | 22,2 (10,3–25,5) | 31,7 (19,9–45,1) | 48,2 (37,8–67,3) | 67,6 (55,5–84,4) | 72 (56,7–106) |
| İnşaat (`build`) | 25,9 (18,9–31,3) | 83,7 (68,5–99,1) | 53,1 (40,7–65,9) | 29,7 (20,7–44,9) | 21,1 (14,5–28,8) | 20,5 (8,75–39) |
| Göç (`migration`) | 1,6 (0,7–2,55) | 9 (6,65–17,4) | 17,2 (12,4–29,2) | 18,8 (10,7–28) | 23,9 (15,6–33,3) | 22,8 (14,2–37,7) |
| Savaş (`war`) | 0,45 (0–1,25) | 6,9 (2,05–9,6) | 10,7 (4,55–17,2) | 11,3 (4,15–21,6) | 15,9 (4,65–26,7) | 17,2 (2,2–31,6) |
| Sefer/ilan (`quest`) | 1,55 (1,1–2,75) | 8,55 (5,45–9,9) | 7,85 (5,45–12,5) | 12,3 (7,55–21,5) | 16,4 (10,2–25,7) | 15,4 (9,4–26,7) |
| Araştırma (`research`) | 16,7 (13,6–18,5) | 16,3 (14,3–19,1) | 3,15 (2,25–5,65) | 0,35 (0–1,95) | 0,4 (0–0,95) | 0,1 (0–1,05) |
| Sınıf (`class`) | 4,6 (3,5–5,95) | 6,45 (5,1–7,3) | 5,35 (4,5–6,6) | 4,55 (3,35–5,45) | 4,4 (3,2–5,55) | 4,2 (3,1–5,65) |
| Kamp (`lair`) | 2 (1,9–2,4) | 4,6 (2,95–5,35) | 4,8 (3,6–6,6) | 5,25 (3,65–7,1) | 5,85 (4,5–7,35) | 5,85 (3,75–7,55) |
| Deniz (`sea`) | 0,4 (0–1,45) | 5,7 (3,4–8,65) | 4,75 (2,1–10) | 3,8 (1,25–7,6) | 3 (1,2–8,85) | 4,45 (1,35–9,25) |
| Baskın (`raid`) | 3,75 (2,95–4,35) | 3,8 (2,95–4,95) | 2,95 (1,65–3,55) | 3,4 (2,5–4,3) | 3,2 (2,1–4,45) | 3,05 (2–4,9) |
| Han (`inn`) | 2,8 (2,2–3,2) | 3,35 (2,5–4,65) | 3,1 (2–4,5) | 2,45 (1,6–3,85) | 2,55 (2–3,8) | 2,8 (2,2–3,85) |
| Yerleşim (`settle`) | 4,8 (4,15–5,35) | 4,8 (3,9–5,75) | 2,4 (1,65–3,1) | 1,5 (0,85–2,4) | 1,35 (0,7–2,3) | 0,95 (0,4–3,05) |
| Ekonomi (`economy`) | 0,75 (0,55–0,95) | 2 (1,3–3,5) | 3,75 (2,15–4,75) | 2,8 (2–4,55) | 2,8 (1,4–3,9) | 3 (1–6,2) |
| Keşif (`discover`) | 6,45 (4,9–8) | 2,55 (1,95–3,65) | 1,55 (0,65–2,05) | 0,95 (0,65–2,05) | 0,65 (0,4–2) | 0,75 (0,15–2,55) |
| Ticaret (`trade`) | 1,3 (0,9–2,2) | 2,25 (1,1–3,75) | 1,65 (0,7–2,65) | 1 (0,25–2,6) | 0,9 (0–2,95) | 1,35 (0,05–3,85) |
| Ölüm/terk (`death`) | 0,2 (0,05–0,6) | 0,95 (0,7–1,6) | 1,2 (0,55–2,05) | 1,45 (1,15–2,4) | 2,15 (1,35–3,25) | 2,25 (1,2–4,1) |
| epitaph | 0,15 (0–0,6) | 1 (0,6–1,6) | 1,2 (0,55–1,95) | 1,5 (1,15–2,2) | 1,8 (1,1–3,05) | 2,1 (1,2–3,6) |
| Ejderha (`dragon`) | 0 | 0,15 (0–0,65) | 1,8 (1,3–2,15) | 1,7 (0,55–2) | 1,5 (0,3–1,8) | 1,15 (0–2,05) |
| Dünya (`world`) | 0,15 (0–0,3) | 0,65 (0,4–1,2) | 0,65 (0,5–1,1) | 0,9 (0,55–1,25) | 1,35 (0,95–2,5) | 1,7 (0,75–2,4) |
| Büyüme (`growth`) | 1,7 (1,35–1,95) | 1,3 (1,05–1,5) | 0,2 (0,1–0,35) | 0 (0–0,2) | 0,1 (0–0,3) | 0,1 (0–0,35) |
| Gerginlik (`tension`) | 0,7 (0,3–1,3) | 0,55 (0,25–1,2) | 0,4 (0,1–0,9) | 0,4 (0,05–0,7) | 0,25 (0–0,9) | 0,6 (0–1,3) |
| Diplomasi (`diplomacy`) | 0,6 (0,3–1,05) | 0,55 (0,1–1,35) | 0,5 (0,1–0,9) | 0,3 (0,05–0,85) | 0,35 (0,05–1,2) | 0,55 (0–1,5) |
| Çağ (`era`) | 1,45 (1,25–1,6) | 0,75 (0,6–0,95) | 0,2 (0,05–0,35) | 0 (0–0,1) | 0 | 0 |
| Temas (`contact`) | 1 (0,6–1,4) | 0,65 (0,3–1) | 0,25 (0,05–0,4) | 0,1 (0–0,45) | 0 (0–0,25) | 0,05 (0–0,25) |
| Kriz (anlatıcı) (`crisis`) | 0 (0–0,05) | 0,3 (0,1–0,35) | 0,3 (0,15–0,5) | 0,35 (0,1–0,55) | 0,4 (0,1–0,45) | 0,4 (0,1–0,6) |
| Rahatlama (anlatıcı) (`relief`) | 0,2 (0,1–0,3) | 0,1 (0–0,2) | 0,1 (0–0,2) | 0,1 (0–0,2) | 0,1 (0–0,2) | 0,1 (0–0,2) |
| Harika (`wonder`) | 0 | 0,25 (0,1–0,5) | 0,25 (0,1–0,5) | 0,1 (0–0,2) | 0 (0–0,1) | 0 (0–0,1) |

## Büyük olay türleri

Dünya başına yıllık büyük olay sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Sefer/ilan (`quest`) | 1,55 (1,1–2,45) | 6,3 (4,5–7,4) | 6,25 (4,2–9,85) | 9,2 (5,6–14,7) | 11,4 (7,15–17,3) | 10,5 (6,45–19,2) |
| Savaş (`war`) | 0,35 (0–0,7) | 5,3 (1,55–7,2) | 7,45 (3,45–12,1) | 7,9 (3,2–13,9) | 9,45 (3,2–17,5) | 9,55 (1,55–19,9) |
| Kahraman (`hero`) | 0,6 (0,25–1) | 3,75 (2,4–4,75) | 5,05 (3,2–5,9) | 6,5 (5,3–8,75) | 8,8 (7,25–11,1) | 9,85 (6,6–14,8) |
| Araştırma (`research`) | 6,45 (4,95–7,85) | 14,5 (12,4–17,3) | 3 (2,2–5,45) | 0,35 (0–1,85) | 0,35 (0–0,85) | 0,1 (0–0,9) |
| Kamp (`lair`) | 2 (1,9–2,4) | 4,15 (2,95–4,7) | 4 (3,1–5,55) | 4 (3,2–5,55) | 4,7 (3,65–5,7) | 4,4 (2,9–6,05) |
| İnşaat (`build`) | 2,2 (1,55–2,6) | 8,4 (7,15–14,9) | 5,1 (3,5–9,25) | 1,9 (1,15–4,05) | 1,2 (0,2–2,2) | 1,4 (0,3–2,05) |
| Baskın (`raid`) | 3,35 (2,35–4) | 2,7 (2,15–3,8) | 1,65 (1–2,5) | 2,4 (1,35–3) | 2,3 (1,55–3,5) | 2,05 (1,65–4,1) |
| Deniz (`sea`) | 0,3 (0–0,9) | 2,5 (1,55–3,7) | 2,2 (0,85–5,2) | 2,45 (0,75–5,5) | 1,65 (0,6–4,95) | 2,55 (0,7–5,3) |
| Han (`inn`) | 1,45 (1,2–1,7) | 2,25 (1,75–3,1) | 1,8 (1,1–2,55) | 1,55 (0,9–2,35) | 1,75 (1,15–2,55) | 2,2 (1,65–2,85) |
| Sınıf (`class`) | 1,85 (1,3–2,9) | 2,35 (1,5–3,5) | 2,4 (1,6–2,95) | 1,45 (0,9–2,4) | 1,35 (0,5–2,3) | 1,25 (0,6–2,1) |
| Ölüm/terk (`death`) | 0,2 (0,05–0,6) | 0,95 (0,7–1,6) | 1,2 (0,55–2,05) | 1,45 (1,15–2,4) | 2,15 (1,35–3,25) | 2,25 (1,2–4,1) |
| Yerleşim (`settle`) | 2,3 (1,9–2,65) | 2,6 (2,15–3,15) | 1,25 (0,85–1,75) | 0,85 (0,45–1,25) | 0,7 (0,4–1,2) | 0,5 (0,25–1,55) |
| epitaph | 0,15 (0–0,6) | 1 (0,6–1,6) | 1,2 (0,55–1,95) | 1,5 (1,15–2,2) | 1,8 (1,1–3,05) | 2,1 (1,2–3,6) |
| Ejderha (`dragon`) | 0 | 0,15 (0–0,65) | 1,8 (1,3–2,15) | 1,7 (0,55–2) | 1,5 (0,3–1,8) | 1,15 (0–2,05) |
| Keşif (`discover`) | 2,2 (1,8–3,2) | 1,25 (0,8–1,85) | 0,7 (0,3–1,1) | 0,55 (0,3–1,2) | 0,4 (0,1–0,85) | 0,45 (0,05–1,2) |
| Ekonomi (`economy`) | 0 | 1,1 (0,65–1,9) | 1,8 (1,25–2,1) | 1 (0,4–1,3) | 0,4 (0,2–0,7) | 0,25 (0,05–0,6) |
| Ticaret (`trade`) | 0,7 (0,45–1,1) | 0,85 (0,5–1,7) | 0,65 (0,35–1,05) | 0,55 (0,05–1,1) | 0,35 (0–1,5) | 0,7 (0–2) |
| Dünya (`world`) | 0,15 (0–0,25) | 0,4 (0,3–0,95) | 0,45 (0,3–0,65) | 0,6 (0,35–0,85) | 1 (0,6–1,5) | 1,1 (0,6–1,65) |
| Gerginlik (`tension`) | 0,7 (0,3–1,3) | 0,55 (0,25–1,2) | 0,4 (0,1–0,9) | 0,4 (0,05–0,7) | 0,25 (0–0,9) | 0,6 (0–1,3) |
| Büyüme (`growth`) | 0,9 (0,7–1,1) | 1,25 (1,05–1,5) | 0,2 (0,1–0,35) | 0 (0–0,2) | 0,1 (0–0,3) | 0 (0–0,35) |
| Çağ (`era`) | 1,45 (1,25–1,6) | 0,75 (0,6–0,95) | 0,2 (0,05–0,35) | 0 (0–0,1) | 0 | 0 |
| Temas (`contact`) | 1 (0,6–1,4) | 0,65 (0,3–1) | 0,25 (0,05–0,4) | 0,1 (0–0,45) | 0 (0–0,25) | 0,05 (0–0,25) |
| Kriz (anlatıcı) (`crisis`) | 0 (0–0,05) | 0,3 (0,1–0,35) | 0,3 (0,15–0,5) | 0,35 (0,1–0,55) | 0,4 (0,1–0,45) | 0,4 (0,1–0,6) |
| Diplomasi (`diplomacy`) | 0,2 (0,05–0,4) | 0,35 (0,05–0,8) | 0,25 (0,1–0,65) | 0,2 (0,05–0,45) | 0,1 (0,05–0,65) | 0,25 (0–0,7) |
| Göç (`migration`) | 0,2 (0,05–0,35) | 0,2 (0,05–0,35) | 0,2 (0,05–0,45) | 0,1 (0,05–0,3) | 0,15 (0–0,35) | 0,2 (0,1–0,45) |
| Rahatlama (anlatıcı) (`relief`) | 0,2 (0,1–0,3) | 0,1 (0–0,2) | 0,1 (0–0,2) | 0,1 (0–0,2) | 0,1 (0–0,2) | 0,1 (0–0,2) |
| Harika (`wonder`) | 0 | 0,25 (0,1–0,5) | 0,25 (0,1–0,5) | 0,1 (0–0,2) | 0 (0–0,1) | 0 (0–0,1) |

## Muharebe türleri

Dünya başına yıllık muharebe sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Kamp saldırısı (`camp`) | 0 (0–0,25) | 2,15 (1,5–2,4) | 1,75 (1,05–2,2) | 1,75 (1,25–2,4) | 1,95 (1,55–2,65) | 1,95 (1,1–2,65) |
| Yapı baskını (canavar) (`extRaid`) | 2,1 (1,45–2,8) | 2,05 (1,2–2,85) | 1,2 (0,6–1,75) | 1,3 (0,7–2,2) | 1,35 (0,65–2,15) | 1,2 (0,7–2,15) |
| Yağma akını (`plunder`) | 0 (0–0,25) | 0,9 (0,1–1,5) | 1,65 (0,3–2,8) | 1,65 (0,1–3,6) | 2,1 (0–4,5) | 1,8 (0–5,05) |
| Yerleşim baskını (canavar) (`raid`) | 1,7 (0,85–2,05) | 1,45 (1,05–1,9) | 0,85 (0,45–1,45) | 1,05 (0,45–1,6) | 0,7 (0,35–1,25) | 0,8 (0,2–1,5) |
| Kuşatma (`siege`) | 0 (0–0,2) | 0,9 (0,4–1,35) | 1,15 (0,6–1,65) | 1,2 (0,6–1,75) | 1,4 (0,8–2,15) | 1,45 (0,35–2,5) |
| Trol (`troll`) | 0 | 0,1 (0–0,45) | 0,7 (0,15–1,65) | 1,15 (0,35–1,9) | 1,35 (0,8–2) | 1,25 (0,65–2,1) |
| Ejderha (`dragon`) | 0 | 0,05 (0–0,25) | 0,8 (0,5–1,15) | 0,7 (0,05–0,9) | 0,6 (0,05–0,85) | 0,15 (0–1) |
| Han baskını (canavar) (`innMonster`) | 0,2 (0,1–0,45) | 0,1 (0–0,2) | 0,1 (0–0,2) | 0,05 (0–0,2) | 0,1 (0–0,2) | 0 (0–0,15) |
| Korsan savaşı (`pirate`) | 0 | 0 (0–0,25) | 0,1 (0–0,35) | 0,05 (0–0,25) | 0 (0–0,1) | 0,1 (0–0,15) |
| Deniz savaşı (`naval`) | 0 | 0 | 0 (0–0,45) | 0 (0–0,7) | 0 (0–0,4) | 0,15 (0–0,35) |
| Kervan soygunu (`robbery`) | 0 | 0 (0–0,1) | 0 (0–0,05) | 0 (0–0,1) | 0 (0–0,15) | 0,05 (0–0,2) |
| Yol pususu (`ambush`) | 0 (0–0,1) | 0 (0–0,2) | 0 | 0 (0–0,15) | 0 (0–0,2) | 0 (0–0,1) |
| Düello (`duel`) | 0 | 0 | 0 | 0 | 0 | 0 (0–0,1) |
| Han baskını (medeniyet) (`innCiv`) | 0 | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) |

## Kamp türleri

Dünya başına yaşayan kamp (yıl sonu değerlerinin on yıllık ortalaması): dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Goblin (`goblin`) | 5,75 (4,85–6,3) | 3,4 (1,75–5,05) | 2,65 (1,4–3,25) | 2,15 (0,8–3,1) | 1,8 (1,05–3,25) | 2,2 (0,9–3,1) |
| Hobgoblin (`hobgoblin`) | 1,45 (1,15–1,8) | 2,55 (1,6–3,25) | 2,3 (1,5–2,8) | 3,3 (2,6–4) | 3,5 (2,65–4,95) | 4,1 (3,3–5,15) |
| Bugbear (`bugbear`) | 0,45 (0,25–0,7) | 0,8 (0,4–1,1) | 0,9 (0,4–1,4) | 1,05 (0,7–2) | 1,3 (0,55–1,95) | 1,4 (0,6–2) |
| Trol (`troll`) | 0 | 0,2 (0–0,45) | 0,7 (0,15–2,1) | 1,25 (0,5–1,95) | 1,5 (0,5–2,5) | 2,05 (1,2–2,8) |
| Ejderha (`dragon`) | 0 | 0,05 (0–0,3) | 1 (0,7–1) | 1 (0,5–1) | 1 (0,05–1) | 1 (0–1) |
| Korsan (`pirate`) | 0 | 0,5 (0,2–0,7) | 0,55 (0,1–1) | 0,3 (0,05–0,7) | 0,25 (0–0,5) | 0,3 (0,1–0,6) |

## Çağ dağılımı

Yaşayan medeniyetlerin çağlara dağılımı, bütün dünyalar (yıl sonu).

| Yıl | I Kamp | II Köy | III Kasaba | IV Krallık |
|---|---|---|---|---|
| 10 | %0,8 | %29 | %64 | %6,3 |
| 20 | %0 | %1,6 | %24 | %75 |
| 30 | %0 | %0 | %2,3 | %98 |
| 40 | %0 | %0 | %0 | %100 |
| 50 | %0 | %0 | %0 | %100 |
| 60 | %0 | %0 | %0 | %100 |

## Dünyalar

| Seed | Medeniyet | Yerleşim | Nüfus | Çöküş | Efsane | En yüksek Sv | Doğan / ölü kahraman | Ağaç bitişi (yıl, medyan) | Süre (sn) | Son hash |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | 9 | 72 | 4310 | 2 | 3 | 8 | 144 / 59 | 20 | 48,2 | `20c9085ff9cd062a` |
| 2 | 7 | 65 | 3296 | 0 | 3 | 8 | 225 / 88 | 25 | 40,8 | `ebe5b84e9b870627` |
| 3 | 11 | 105 | 4664 | 15 | 3 | 7 | 236 / 117 | 21 | 55,8 | `08ecab6d6766eac5` |
| 4 | 10 | 101 | 5018 | 7 | 5 | 10 | 184 / 65 | 19 | 53,8 | `6d79476a2c483405` |
| 5 | 8 | 87 | 3936 | 8 | 7 | 8 | 194 / 76 | 19,5 | 45,8 | `23c2deebe45ff930` |
| 6 | 7 | 74 | 3642 | 0 | 4 | 8 | 140 / 50 | 22 | 37,9 | `6aa24cc55be9f4a0` |
| 7 | 12 | 105 | 4823 | 11 | 2 | 9 | 233 / 114 | 21 | 59,2 | `45ffaae26bdf20e6` |
| 8 | 9 | 92 | 4401 | 11 | 5 | 8 | 201 / 94 | 18,5 | 51,7 | `0ebf07df64f4bce7` |
| 9 | 7 | 71 | 3828 | 20 | 3 | 8 | 219 / 120 | 19,5 | 49,5 | `1a34551a7d544487` |
| 10 | 11 | 101 | 4904 | 11 | 2 | 9 | 221 / 87 | 21 | 56,6 | `dff8d59b604afb3b` |
| 11 | 9 | 91 | 4366 | 20 | 3 | 9 | 202 / 99 | 19 | 45,8 | `e5c449976afe1e77` |
| 12 | 9 | 77 | 3371 | 3 | 3 | 8 | 140 / 71 | 18 | 36,8 | `ae89ccb7e18d17ea` |
| 13 | 11 | 90 | 4458 | 4 | 1 | 9 | 181 / 63 | 19 | 50,5 | `c9abe385eb349d99` |
| 14 | 9 | 77 | 3562 | 8 | 3 | 9 | 209 / 93 | 23 | 42,5 | `2f1583f917b42c7b` |
| 15 | 8 | 86 | 4212 | 6 | 12 | 10 | 191 / 81 | 24 | 47,7 | `808ae39729a555bd` |
| 16 | 10 | 103 | 3896 | 20 | 5 | 10 | 196 / 79 | 23 | 51,7 | `b54a005eeee3951a` |

Çöküşler:

- seed 1, 26. yıl (gün 3089): Kızılboynuz Soyu başkenti kaybetti: Boynuztepe (Kanlıdiş Kabileleri aldı)
- seed 1, 28. yıl (gün 3340): Kızılboynuz Soyu başkenti kaybetti: Alazvadi (Kanlıdiş Kabileleri aldı)
- seed 3, 22. yıl (gün 2536): Sınır Bekçileri başkenti kaybetti: Gözcüağaç (Kanlıdiş Kabileleri aldı)
- seed 3, 28. yıl (gün 3323): Karaörs Derinlikleri başkenti kaybetti: Sessizocak (Örsyürek Tapınak Klanı aldı)
- seed 3, 31. yıl (gün 3646): Karaörs Derinlikleri başkenti kaybetti: Taşhisar (Örsyürek Tapınak Klanı aldı)
- seed 3, 36. yıl (gün 4284): Pınardere Kara Beyliği başkenti kaybetti: Pınardere (Karaörs Derinlikleri aldı)
- seed 3, 38. yıl (gün 4511): Tatlıçayır Loncası başkenti kaybetti: Balköprü (Kanlıdiş Kabileleri aldı)
- seed 3, 42. yıl (gün 4931): Karaörs Derinlikleri başkenti kaybetti: Taşdere (Örsyürek Tapınak Klanı aldı)
- seed 3, 43. yıl (gün 5148): Tatlıçayır Loncası başkenti kaybetti: Kavşakpazar (Kanlıdiş Kabileleri aldı)
- seed 3, 45. yıl (gün 5294): Karaörs Derinlikleri yok oldu
- seed 3, 48. yıl (gün 5663): Kanlıdiş Kabileleri başkenti kaybetti: Gökyayla (Tatlıçayır Loncası aldı)
- seed 3, 48. yıl (gün 5729): Okyayı Boyu başkenti kaybetti: Okyayı (Sınır Bekçileri aldı)
- seed 3, 49. yıl (gün 5813): Pınardere Kara Beyliği başkenti kaybetti: Yıldızgöz (Çarkyıldız Akademisi aldı)
- seed 3, 51. yıl (gün 6063): Tatlıçayır Loncası başkenti kaybetti: Gökyayla (Kanlıdiş Kabileleri aldı)
- seed 3, 54. yıl (gün 6470): Kanlıdiş Kabileleri başkenti kaybetti: Gökyayla (Rüzgâr Manastırı aldı)
- seed 3, 57. yıl (gün 6764): Aksırt Boyu başkenti kaybetti: Aksırt (Kanlıdiş Kabileleri aldı)
- seed 3, 59. yıl (gün 6981): Pınardere Kara Beyliği başkenti kaybetti: Günkaya (Örsyürek Tapınak Klanı aldı)
- seed 4, 25. yıl (gün 2908): Kanlıdiş Kabileleri başkenti kaybetti: Közburç (Pulzırh Lejyonu aldı)
- seed 4, 31. yıl (gün 3690): Rüzgâr Manastırı başkenti kaybetti: Sessiztepe (Kanlıdiş Kabileleri aldı)
- seed 4, 45. yıl (gün 5385): Kanlıdiş Kabileleri başkenti kaybetti: Söğütburç (Sınır Bekçileri aldı)
- seed 4, 53. yıl (gün 6322): Kanlıdiş Kabileleri başkenti kaybetti: Söğütburç (Rüzgâr Manastırı aldı)
- seed 4, 56. yıl (gün 6619): Kanlıdiş Kabileleri başkenti kaybetti: Meşekent (Pulzırh Lejyonu aldı)
- seed 4, 58. yıl (gün 6871): Savaşçukur Beyliği başkenti kaybetti: Savaşçukur (Kanlıdiş Kabileleri aldı)
- seed 4, 58. yıl (gün 6901): Kanlıdiş Kabileleri başkenti kaybetti: Tuzyayla (Pulzırh Lejyonu aldı)
- seed 5, 10. yıl (gün 1188): Karaörs Derinlikleri başkenti kaybetti: Külçukur (Kanlıdiş Kabileleri aldı)
- seed 5, 13. yıl (gün 1502): Karaörs Derinlikleri yok oldu
- seed 5, 17. yıl (gün 2018): Tatlıçayır Loncası başkenti kaybetti: Fıçıköy (Kanlıdiş Kabileleri aldı)
- seed 5, 23. yıl (gün 2681): Tatlıçayır Loncası başkenti kaybetti: Kavşakpazar (Kanlıdiş Kabileleri aldı)
- seed 5, 27. yıl (gün 3160): Kanlıdiş Kabileleri başkenti kaybetti: Ejderkale (Pulzırh Lejyonu aldı)
- seed 5, 30. yıl (gün 3500): Tatlıçayır Loncası yok oldu
- seed 5, 31. yıl (gün 3717): Güneştacı Krallığı başkenti kaybetti: Altınkapı (Pulzırh Lejyonu aldı)
- seed 5, 55. yıl (gün 6525): Kanlıdiş Kabileleri başkenti kaybetti: Demirkanat (Pulzırh Lejyonu aldı)
- seed 7, 12. yıl (gün 1415): Kızılboynuz Soyu başkenti kaybetti: Közsaray (Karaörs Derinlikleri aldı)
- seed 7, 43. yıl (gün 5042): Yeşilyaprak Çemberi başkenti kaybetti: Sessizkoru (Karaörs Derinlikleri aldı)
- seed 7, 44. yıl (gün 5275): Karaörs Derinlikleri başkenti kaybetti: Sessizkoru (Yeşilyaprak Çemberi aldı)
- seed 7, 49. yıl (gün 5790): Rüzgâr Manastırı başkenti kaybetti: Dinginpınar (Kızılboynuz Soyu aldı)
- seed 7, 50. yıl (gün 5989): Yeşilyaprak Çemberi başkenti kaybetti: Yosunpınar (Karaörs Derinlikleri aldı)
- seed 7, 52. yıl (gün 6182): Dumanköprü Çemberi başkenti kaybetti: Dumanköprü (Karaörs Derinlikleri aldı)
- seed 7, 53. yıl (gün 6259): Kocaköprü Boyu başkenti kaybetti: Kocaköprü (Rüzgâr Manastırı aldı)
- seed 7, 54. yıl (gün 6419): Kocaköprü Boyu başkenti kaybetti: Söğütburç (Sınır Bekçileri aldı)
- seed 7, 57. yıl (gün 6821): Kocaköprü Boyu başkenti kaybetti: Karageçit (Sınır Bekçileri aldı)
- seed 7, 59. yıl (gün 6988): Kızılboynuz Soyu başkenti kaybetti: Dinginpınar (Karaörs Derinlikleri aldı)
- seed 7, 60. yıl (gün 7104): Kocaköprü Boyu yok oldu
- seed 8, 14. yıl (gün 1564): Sınır Bekçileri başkenti kaybetti: Gözcüağaç (Kızılboynuz Soyu aldı)
- seed 8, 16. yıl (gün 1824): Örsyürek Tapınak Klanı başkenti kaybetti: Boynuztepe (Kızılboynuz Soyu aldı)
- seed 8, 18. yıl (gün 2047): Sınır Bekçileri başkenti kaybetti: Okyayı (Kızılboynuz Soyu aldı)
- seed 8, 20. yıl (gün 2330): Sınır Bekçileri başkenti kaybetti: İzsürer (Kızılboynuz Soyu aldı)
- seed 8, 20. yıl (gün 2382): Kızılboynuz Soyu başkenti kaybetti: Közsaray (Pulzırh Lejyonu aldı)
- seed 8, 30. yıl (gün 3596): Sınır Bekçileri başkenti kaybetti: Çamgözcü (Kızılboynuz Soyu aldı)
- seed 8, 32. yıl (gün 3810): Kızılboynuz Soyu başkenti kaybetti: Çamgözcü (Pulzırh Lejyonu aldı)
- seed 8, 34. yıl (gün 4045): Sınır Bekçileri başkenti kaybetti: Yabanyurt (Kızılboynuz Soyu aldı)
- seed 8, 40. yıl (gün 4728): Sınır Bekçileri başkenti kaybetti: Kartalkaya (Kızılboynuz Soyu aldı)
- seed 8, 40. yıl (gün 4784): Çarkyıldız Akademisi başkenti kaybetti: Pusulakule (Pulzırh Lejyonu aldı)
- seed 8, 42. yıl (gün 4988): Sınır Bekçileri başkenti kaybetti: Balköy (Kızılboynuz Soyu aldı)
- seed 9, 10. yıl (gün 1195): Sınır Bekçileri başkenti kaybetti: İzsürer (Karaörs Derinlikleri aldı)
- seed 9, 14. yıl (gün 1579): Pulzırh Lejyonu başkenti kaybetti: Ejderkale (Kanlıdiş Kabileleri aldı)
- seed 9, 14. yıl (gün 1630): Sınır Bekçileri başkenti kaybetti: Gözcüağaç (Karaörs Derinlikleri aldı)
- seed 9, 17. yıl (gün 1924): Sınır Bekçileri yok oldu
- seed 9, 20. yıl (gün 2400): Pulzırh Lejyonu başkenti kaybetti: Kılıçyurt (Kanlıdiş Kabileleri aldı)
- seed 9, 25. yıl (gün 2961): Pulzırh Lejyonu başkenti kaybetti: Alevgeçit (Kanlıdiş Kabileleri aldı)
- seed 9, 28. yıl (gün 3333): Karaörs Derinlikleri başkenti kaybetti: Külçukur (Örsyürek Tapınak Klanı aldı)
- seed 9, 29. yıl (gün 3404): Karaörs Derinlikleri başkenti kaybetti: Kara Mihrap (Güneştacı Krallığı aldı)
- seed 9, 30. yıl (gün 3567): Rüzgâr Manastırı başkenti kaybetti: Sessiztepe (Kanlıdiş Kabileleri aldı)
- seed 9, 34. yıl (gün 3975): Rüzgâr Manastırı başkenti kaybetti: Çankule (Kanlıdiş Kabileleri aldı)
- seed 9, 40. yıl (gün 4742): Güneştacı Krallığı başkenti kaybetti: Kızılkül (Lirsesi Şehirleri aldı)
- seed 9, 40. yıl (gün 4794): Kanlıdiş Kabileleri başkenti kaybetti: Şafaktepe (Pulzırh Lejyonu aldı)
- seed 9, 42. yıl (gün 4957): Kanlıdiş Kabileleri başkenti kaybetti: Taşkandil (Güneştacı Krallığı aldı)
- seed 9, 43. yıl (gün 5095): Okyayı Boyu başkenti kaybetti: Okyayı (Karaörs Derinlikleri aldı)
- seed 9, 44. yıl (gün 5165): Karaörs Derinlikleri başkenti kaybetti: Gölgeörs (Örsyürek Tapınak Klanı aldı)
- seed 9, 46. yıl (gün 5506): Okyayı Boyu yok oldu
- seed 9, 47. yıl (gün 5566): Karaörs Derinlikleri başkenti kaybetti: Tuzyurt (Güneştacı Krallığı aldı)
- seed 9, 50. yıl (gün 5883): Karaörs Derinlikleri başkenti kaybetti: Karaköy (Güneştacı Krallığı aldı)
- seed 9, 52. yıl (gün 6203): Karaörs Derinlikleri yok oldu
- seed 9, 54. yıl (gün 6366): Kanlıdiş Kabileleri başkenti kaybetti: Kıvılcımlı (Kızılboynuz Soyu aldı)
- seed 10, 31. yıl (gün 3690): Karaörs Derinlikleri başkenti kaybetti: Pınardere (Tatlıçayır Loncası aldı)
- seed 10, 35. yıl (gün 4159): Gölgeçarşı Beyliği başkenti kaybetti: Gölgeçarşı (Karaörs Derinlikleri aldı)
- seed 10, 37. yıl (gün 4395): Karaörs Derinlikleri başkenti kaybetti: Sessizocak (Kanlıdiş Kabileleri aldı)
- seed 10, 42. yıl (gün 5016): Karaörs Derinlikleri başkenti kaybetti: Kırkkapı (Tatlıçayır Loncası aldı)
- seed 10, 47. yıl (gün 5606): Karaörs Derinlikleri başkenti kaybetti: Demiroba (Kanlıdiş Kabileleri aldı)
- seed 10, 47. yıl (gün 5614): Gölgeçarşı Kara Beyliği yok oldu
- seed 10, 51. yıl (gün 6016): Karaörs Derinlikleri başkenti kaybetti: Kavşakpazar (Tatlıçayır Loncası aldı)
- seed 10, 53. yıl (gün 6310): Karaörs Derinlikleri başkenti kaybetti: Sisliyamaç (Çarkyıldız Akademisi aldı)
- seed 10, 54. yıl (gün 6438): Karaörs Derinlikleri başkenti kaybetti: Yelköy (Rüzgâr Manastırı aldı)
- seed 10, 57. yıl (gün 6734): Karaörs Derinlikleri başkenti kaybetti: Sisliyamaç (Rüzgâr Manastırı aldı)
- seed 10, 60. yıl (gün 7081): Sınır Bekçileri başkenti kaybetti: Kurtgeçit (Kanlıdiş Kabileleri aldı)
- seed 11, 15. yıl (gün 1744): Çarkyıldız Akademisi başkenti kaybetti: Pusulakule (Kızılboynuz Soyu aldı)
- seed 11, 17. yıl (gün 1961): Örsyürek Tapınak Klanı başkenti kaybetti: Demirçan (Kanlıdiş Kabileleri aldı)
- seed 11, 18. yıl (gün 2047): Çarkyıldız Akademisi başkenti kaybetti: Kristalköy (Kızılboynuz Soyu aldı)
- seed 11, 24. yıl (gün 2830): Kızılboynuz Soyu başkenti kaybetti: Alazvadi (Kanlıdiş Kabileleri aldı)
- seed 11, 28. yıl (gün 3331): Sarıdere Beyliği başkenti kaybetti: Sarıdere (Kızılboynuz Soyu aldı)
- seed 11, 29. yıl (gün 3437): Rüzgâr Manastırı başkenti kaybetti: Sisliyamaç (Kanlıdiş Kabileleri aldı)
- seed 11, 32. yıl (gün 3733): Sarıdere Beyliği başkenti kaybetti: Gökyurt (Kızılboynuz Soyu aldı)
- seed 11, 32. yıl (gün 3827): Rüzgâr Manastırı başkenti kaybetti: Sessiztepe (Kanlıdiş Kabileleri aldı)
- seed 11, 35. yıl (gün 4197): Çarkyıldız Akademisi başkenti kaybetti: Mürekkeptepe (Kızılboynuz Soyu aldı)
- seed 11, 36. yıl (gün 4224): Rüzgâr Manastırı başkenti kaybetti: Çankule (Kanlıdiş Kabileleri aldı)
- seed 11, 39. yıl (gün 4626): Rüzgâr Manastırı başkenti kaybetti: Kızıltepe (Kanlıdiş Kabileleri aldı)
- seed 11, 42. yıl (gün 5006): Çarkyıldız Akademisi başkenti kaybetti: Yıldıztepe (Kızılboynuz Soyu aldı)
- seed 11, 42. yıl (gün 5028): Çarkyıldız Akademisi başkenti kaybetti: Kocaköprü (Kanlıdiş Kabileleri aldı)
- seed 11, 44. yıl (gün 5216): Rüzgâr Manastırı başkenti kaybetti: Yıldızçayır (Kanlıdiş Kabileleri aldı)
- seed 11, 46. yıl (gün 5402): Çarkyıldız Akademisi başkenti kaybetti: Aksırt (Kızılboynuz Soyu aldı)
- seed 11, 47. yıl (gün 5608): Rüzgâr Manastırı başkenti kaybetti: Kuzeybük (Kanlıdiş Kabileleri aldı)
- seed 11, 51. yıl (gün 6012): Çarkyıldız Akademisi başkenti kaybetti: Kurtkale (Kanlıdiş Kabileleri aldı)
- seed 11, 54. yıl (gün 6420): Çarkyıldız Akademisi başkenti kaybetti: Meşekale (Kanlıdiş Kabileleri aldı)
- seed 11, 56. yıl (gün 6641): Rüzgâr Manastırı başkenti kaybetti: Ayburç (Kanlıdiş Kabileleri aldı)
- seed 11, 57. yıl (gün 6767): Çarkyıldız Akademisi başkenti kaybetti: Ceylanbük (Dumanpınar Beyliği aldı)
- seed 12, 27. yıl (gün 3136): Kızılboynuz Soyu başkenti kaybetti: Kızılkül (Pulzırh Lejyonu aldı)
- seed 12, 39. yıl (gün 4657): Sınır Bekçileri başkenti kaybetti: İzsürer (Kanlıdiş Kabileleri aldı)
- seed 12, 57. yıl (gün 6759): Kanlıdiş Kabileleri başkenti kaybetti: Gölgeçarşı (Tatlıçayır Loncası aldı)
- seed 13, 14. yıl (gün 1646): Kanlıdiş Kabileleri başkenti kaybetti: Kurtgeçit (Sınır Bekçileri aldı)
- seed 13, 40. yıl (gün 4731): Yeşilyaprak Çemberi başkenti kaybetti: Sessizkoru (Pulzırh Lejyonu aldı)
- seed 13, 50. yıl (gün 5896): Yeşilyaprak Çemberi başkenti kaybetti: Kızılçayır (Pulzırh Lejyonu aldı)
- seed 13, 57. yıl (gün 6839): Yeşilyaprak Çemberi başkenti kaybetti: Siskoru (Kanlıdiş Kabileleri aldı)
- seed 14, 26. yıl (gün 3017): Sınır Bekçileri başkenti kaybetti: Gözcüağaç (Karaörs Derinlikleri aldı)
- seed 14, 33. yıl (gün 3896): Karaörs Derinlikleri başkenti kaybetti: Sessizocak (Yeşilyaprak Çemberi aldı)
- seed 14, 42. yıl (gün 5039): Sınır Bekçileri başkenti kaybetti: Taşhisar (Karaörs Derinlikleri aldı)
- seed 14, 50. yıl (gün 5885): Kocapınar Bekçileri başkenti kaybetti: Kocapınar (Karaörs Derinlikleri aldı)
- seed 14, 53. yıl (gün 6272): Karaörs Derinlikleri başkenti kaybetti: Taşhisar (Örsyürek Tapınak Klanı aldı)
- seed 14, 55. yıl (gün 6500): Karaörs Derinlikleri başkenti kaybetti: Kızılçayır (Çarkyıldız Akademisi aldı)
- seed 14, 58. yıl (gün 6849): Karaörs Derinlikleri başkenti kaybetti: Pınardere (Güneştacı Krallığı aldı)
- seed 14, 60. yıl (gün 7144): Kocapınar Bekçileri başkenti kaybetti: Yelgeçit (Karaörs Derinlikleri aldı)
- seed 15, 27. yıl (gün 3135): Kızılboynuz Soyu başkenti kaybetti: Kızılkül (Kanlıdiş Kabileleri aldı)
- seed 15, 30. yıl (gün 3534): Kızılboynuz Soyu başkenti kaybetti: Közsaray (Kanlıdiş Kabileleri aldı)
- seed 15, 32. yıl (gün 3742): Sınır Bekçileri başkenti kaybetti: Çamgözcü (Kanlıdiş Kabileleri aldı)
- seed 15, 37. yıl (gün 4364): Kızılboynuz Soyu başkenti kaybetti: Kıvılcımlı (Kanlıdiş Kabileleri aldı)
- seed 15, 42. yıl (gün 5004): Tuzyayla Boyu yok oldu
- seed 15, 52. yıl (gün 6236): Sınır Bekçileri başkenti kaybetti: Kurtgeçit (Kanlıdiş Kabileleri aldı)
- seed 16, 7. yıl (gün 808): Güneştacı Krallığı yok oldu
- seed 16, 13. yıl (gün 1451): Rüzgâr Manastırı yok oldu
- seed 16, 16. yıl (gün 1808): Örsyürek Tapınak Klanı başkenti kaybetti: Demirçan (Karaörs Derinlikleri aldı)
- seed 16, 19. yıl (gün 2212): Örsyürek Tapınak Klanı başkenti kaybetti: Közkapı (Karaörs Derinlikleri aldı)
- seed 16, 23. yıl (gün 2710): Taşkandil Beyliği başkenti kaybetti: Taşkandil (Örsyürek Tapınak Klanı aldı)
- seed 16, 25. yıl (gün 2931): Taşkandil Beyliği yok oldu
- seed 16, 28. yıl (gün 3347): Örsyürek Tapınak Klanı başkenti kaybetti: Granitsunak (Karaörs Derinlikleri aldı)
- seed 16, 32. yıl (gün 3745): Örsyürek Tapınak Klanı başkenti kaybetti: Kuzeybük (Karaörs Derinlikleri aldı)
- seed 16, 34. yıl (gün 4073): Tatlıçayır Loncası başkenti kaybetti: Kavşakpazar (Kanlıdiş Kabileleri aldı)
- seed 16, 36. yıl (gün 4319): Örsyürek Tapınak Klanı başkenti kaybetti: Yıldıztepe (Kanlıdiş Kabileleri aldı)
- seed 16, 39. yıl (gün 4649): Örsyürek Tapınak Klanı başkenti kaybetti: Alacayayla (Karaörs Derinlikleri aldı)
- seed 16, 45. yıl (gün 5328): Sınır Bekçileri başkenti kaybetti: Kırkkapı (Kanlıdiş Kabileleri aldı)
- seed 16, 48. yıl (gün 5718): Kanlıdiş Kabileleri başkenti kaybetti: Kırkkapı (Sınır Bekçileri aldı)
- seed 16, 48. yıl (gün 5739): Tatlıçayır Loncası başkenti kaybetti: Kartalyazı (Kanlıdiş Kabileleri aldı)
- seed 16, 49. yıl (gün 5806): Örsyürek Tapınak Klanı başkenti kaybetti: Siskoru (Karaörs Derinlikleri aldı)
- seed 16, 51. yıl (gün 6055): Örsyürek Tapınak Klanı başkenti kaybetti: Kuzeyoba (Karaörs Derinlikleri aldı)
- seed 16, 52. yıl (gün 6181): Dumanköprü Boyu başkenti kaybetti: Dumanköprü (Kanlıdiş Kabileleri aldı)
- seed 16, 54. yıl (gün 6480): Örsyürek Tapınak Klanı başkenti kaybetti: Gölköprü (Karaörs Derinlikleri aldı)
- seed 16, 56. yıl (gün 6624): Sınır Bekçileri başkenti kaybetti: Kırkkapı (Kanlıdiş Kabileleri aldı)
- seed 16, 58. yıl (gün 6925): Örsyürek Tapınak Klanı başkenti kaybetti: Çamova (Karaörs Derinlikleri aldı)

## Yıllık ayrıntı

Hücre: medyan (p10–p90), 16 dünya. Yıl y = (y−1)·120+1 … y·120. günler. Bütün değerler `report.json` içinde (`metrics`), dünya başına değerler `../runs/c1-4` altında.

### Medeniyet (1/4)

| Yıl | Yaşayan medeniyet | Yeni medeniyet (yeniden doğan) | Yok olan medeniyet | Başkent kaybı (medeniyet yaşarken) | Çöküş (yok olma + başkent kaybı) | Yaşayan yerleşim |
|---|---|---|---|---|---|---|
| 1 | 8 (7–9) | 0 | 0 | 0 | 0 | 8 (7–9) |
| 2 | 8 (7–9) | 0 | 0 | 0 | 0 | 8 (7–9) |
| 3 | 8 (7–9) | 0 | 0 | 0 | 0 | 8 (7–9) |
| 4 | 8 (7–9) | 0 | 0 | 0 | 0 | 8 (7–9,5) |
| 5 | 8 (7–9) | 0 | 0 | 0 | 0 | 10 (8–12) |
| 6 | 8 (7–9) | 0 | 0 | 0 | 0 | 13 (10,5–15) |
| 7 | 8 (7–9) | 0 | 0 | 0 | 0 | 18 (13,5–20,5) |
| 8 | 8 (7–9) | 0 | 0 | 0 | 0 | 21 (18–25,5) |
| 9 | 8 (7–9) | 0 | 0 | 0 | 0 | 26 (22–31) |
| 10 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 31,5 (26,5–35) |
| 11 | 8 (7–9) | 0 | 0 | 0 | 0 | 35 (29–40) |
| 12 | 8 (7–9) | 0 | 0 | 0 | 0 | 39 (32–44) |
| 13 | 8 (7–9) | 0 | 0 (0–0,5) | 0 | 0 (0–0,5) | 42,5 (36–46) |
| 14 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 46 (37–49) |
| 15 | 8 (7–9) | 0 | 0 | 0 | 0 | 48 (40,5–52,5) |
| 16 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 51 (41–56) |
| 17 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–1) | 53 (44,5–58) |
| 18 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 54,5 (46–59,5) |
| 19 | 8 (7–9) | 0 | 0 | 0 | 0 | 56,5 (47,5–62) |
| 20 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 57 (49,5–64) |
| 21 | 8 (7–9) | 0 | 0 | 0 | 0 | 58,5 (52–67,5) |
| 22 | 8 (7–9) | 0 | 0 | 0 | 0 | 61 (53–69,5) |
| 23 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 63,5 (54,5–72) |
| 24 | 8 (7–9) | 0 | 0 | 0 | 0 | 64,5 (55–73) |
| 25 | 8 (7–9) | 0 (0–0,5) | 0 | 0 (0–0,5) | 0 (0–1) | 65,5 (56–74,5) |
| 26 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 66 (58–76,5) |
| 27 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 67 (58,5–77) |
| 28 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 68,5 (58,5–78,5) |
| 29 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 69,5 (59–79) |
| 30 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 70,5 (60–81) |
| 31 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 71,5 (61,5–81,5) |
| 32 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 72,5 (62,5–81,5) |
| 33 | 8 (7–9,5) | 0 (0–0,5) | 0 | 0 | 0 | 73 (62,5–82,5) |
| 34 | 8 (7–9,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 73,5 (63,5–83,5) |
| 35 | 8 (7–9,5) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 75,5 (65–84,5) |
| 36 | 8 (7–9,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 76,5 (65,5–85) |
| 37 | 8 (7–9,5) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 77,5 (66–85,5) |
| 38 | 8 (7–9,5) | 0 | 0 | 0 | 0 | 78 (66–85,5) |
| 39 | 8 (7–9,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 78,5 (66,5–86) |
| 40 | 8 (7–9,5) | 0 | 0 | 0 (0–1,5) | 0 (0–1,5) | 79 (66,5–87) |
| 41 | 8,5 (7–9,5) | 0 | 0 | 0 | 0 | 79 (66,5–89) |
| 42 | 8 (7–9,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 79,5 (66–90) |
| 43 | 8 (7–9,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 81 (64–91) |
| 44 | 8,5 (7–9,5) | 0 (0–0,5) | 0 | 0 (0–1) | 0 (0–1) | 82,5 (65–91) |
| 45 | 8,5 (7–10) | 0 (0–1) | 0 | 0 (0–0,5) | 0 (0–1) | 83 (65–91,5) |
| 46 | 8 (7–10) | 0 | 0 | 0 | 0 (0–0,5) | 84 (65–92,5) |
| 47 | 8,5 (7–10) | 0 (0–0,5) | 0 | 0 (0–1) | 0 (0–1) | 84 (65–94) |
| 48 | 9 (7–10) | 0 | 0 | 0 (0–1) | 0 (0–1) | 85 (66–94) |
| 49 | 9 (7–10,5) | 0 (0–0,5) | 0 | 0 (0–1) | 0 (0–1) | 86,5 (67,5–94,5) |
| 50 | 9 (7–10,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 86,5 (67,5–94,5) |
| 51 | 9 (7–10,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 87 (68–95) |
| 52 | 9 (7–10,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 87 (68,5–95,5) |
| 53 | 9 (7–10,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 87 (68,5–97) |
| 54 | 9 (7–11) | 0 | 0 | 0 (0–1) | 0 (0–1) | 86,5 (69–97) |
| 55 | 9 (7–11) | 0 (0–0,5) | 0 | 0 (0–0,5) | 0 (0–0,5) | 86 (70–97,5) |
| 56 | 9 (7–11) | 0 | 0 | 0 (0–1) | 0 (0–1) | 86,5 (71–98) |
| 57 | 9 (7–11) | 0 | 0 | 0 (0–1) | 0 (0–1) | 87 (72,5–101) |
| 58 | 9 (7–11) | 0 | 0 | 0 (0–1) | 0 (0–1) | 88 (72,5–102) |
| 59 | 9 (7–11) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 88 (71,5–103) |
| 60 | 9 (7–11) | 0 (0–0,5) | 0 | 0 (0–0,5) | 0 (0–1) | 88,5 (71,5–104) |

### Medeniyet (2/4)

| Yıl | Medeniyet başına yerleşim | 5+ kara yerleşimli medeniyet payı | Kurulan yerleşim | Fethedilen yerleşim | Terk edilen yerleşim | Toplam nüfus |
|---|---|---|---|---|---|---|
| 1 | 1 | %0 | 0 | 0 | 0 | 64,5 (55–75) |
| 2 | 1 | %0 | 0 | 0 | 0 | 85,5 (72–99) |
| 3 | 1 | %0 | 0 | 0 | 0 | 112 (94,5–129) |
| 4 | 1 (1–1,06) | %0 | 0 (0–0,5) | 0 | 0 | 142 (117–166) |
| 5 | 1,25 (1,12–1,4) | %0 | 2 (1–3) | 0 | 0 | 190 (144–220) |
| 6 | 1,63 (1,44–1,86) | %0 | 3 (2–4,5) | 0 | 0 | 242 (188–284) |
| 7 | 2,25 (1,86–2,54) | %0 | 4,5 (3–6) | 0 | 0 | 320 (235–366) |
| 8 | 2,82 (2,28–3,13) | %0 | 4,5 (2,5–6) | 0 | 0 | 409 (302–470) |
| 9 | 3,44 (2,88–3,76) | %12 (%0–%29) | 5 (3,5–7) | 0 (0–0,5) | 0 | 535 (407–596) |
| 10 | 3,94 (3,41–4,25) | %38 (%18–%62) | 4,5 (2,5–6) | 0 (0–1,5) | 0 | 676 (524–742) |
| 11 | 4,38 (3,87–4,75) | %50 (%31–%75) | 4 (2–5) | 0,5 (0–1) | 0 | 792 (622–884) |
| 12 | 4,82 (4,26–5,25) | %71 (%44–%87) | 4 (1,5–4,5) | 0 (0–1) | 0 | 920 (738–1020) |
| 13 | 5,29 (4,72–5,63) | %86 (%56–%88) | 3 (1–4,5) | 1 (0–1) | 0 | 1080 (828–1166) |
| 14 | 5,47 (5,13–6) | %88 (%67–%100) | 3 (1–5) | 1 (0–2) | 0 | 1220 (934–1342) |
| 15 | 5,94 (5,38–6,31) | %88 (%69–%100) | 3 (1–5,5) | 1 (0–2) | 0 | 1364 (1031–1479) |
| 16 | 6,18 (5,67–6,75) | %88 (%69–%100) | 1,5 (1–4) | 1 (0–1,5) | 0 | 1531 (1160–1604) |
| 17 | 6,54 (6,06–7) | %94 (%73–%100) | 3 (1–4) | 0 (0–2) | 0 | 1642 (1244–1745) |
| 18 | 6,87 (6,21–7,25) | %94 (%76–%100) | 2 (0,5–3,5) | 1 (0,5–2,5) | 0 | 1764 (1350–1888) |
| 19 | 7,06 (6,54–7,47) | %100 (%82–%100) | 3 (0,5–3) | 0,5 (0–1) | 0 | 1881 (1482–2046) |
| 20 | 7,31 (6,87–7,75) | %94 (%86–%100) | 2 (0,5–3) | 1 (0–2,5) | 0 | 1978 (1560–2126) |
| 21 | 7,43 (7,06–7,88) | %88 (%86–%100) | 1 (0–3,5) | 1 (0–2,5) | 0 | 2084 (1690–2288) |
| 22 | 7,66 (7,33–8) | %100 (%87–%100) | 2 (1–3) | 1 (0–2) | 0 | 2189 (1780–2370) |
| 23 | 7,88 (7,46–8,2) | %100 (%87–%100) | 1,5 (0–3) | 1,5 (0–2,5) | 0 | 2280 (1880–2478) |
| 24 | 8,06 (7,6–8,33) | %100 (%88–%100) | 1 (0–3) | 1 (0–2) | 0 | 2360 (1966–2569) |
| 25 | 8,24 (7,58–8,5) | %100 (%88–%100) | 1 (0–2,5) | 1 (0–2) | 0 | 2397 (2057–2650) |
| 26 | 8,29 (7,94–8,71) | %100 (%88–%100) | 2 (0–3) | 1 (0–2) | 0 | 2566 (2135–2774) |
| 27 | 8,4 (8–8,76) | %100 (%88–%100) | 1 (0–1,5) | 1 (0–2) | 0 | 2648 (2200–2874) |
| 28 | 8,46 (8,18–8,94) | %100 (%88–%100) | 1,5 (0–2) | 1 (0–2) | 0 | 2642 (2297–2976) |
| 29 | 8,6 (8,26–9,06) | %100 (%88–%100) | 1 (0–2) | 1 (0–2) | 0 | 2730 (2346–3056) |
| 30 | 8,88 (8,01–9,38) | %100 (%88–%100) | 1 (0–2) | 1 (0–2,5) | 0 | 2786 (2403–3130) |
| 31 | 9 (8,14–9,44) | %100 (%88–%100) | 1 (0–1,5) | 1 (0–2) | 0 | 2895 (2436–3072) |
| 32 | 9 (8,3–9,69) | %100 (%88–%100) | 1 (0–2,5) | 1,5 (0–2,5) | 0 (0–0,5) | 2888 (2524–3224) |
| 33 | 8,94 (8,24–9,75) | %100 (%87–%100) | 0 (0–1) | 1 (0–2,5) | 0 | 2968 (2612–3332) |
| 34 | 9,12 (8,26–9,81) | %100 (%87–%100) | 1 (0–2) | 1 (0–2) | 0 (0–0,5) | 3015 (2684–3367) |
| 35 | 9,29 (8,42–9,95) | %100 (%87–%100) | 1 (0–2) | 1 (0–2,5) | 0 | 3076 (2758–3474) |
| 36 | 9,36 (8,5–10) | %95 (%87–%100) | 0,5 (0–2) | 1 (0–2) | 0 | 3181 (2804–3625) |
| 37 | 9,37 (8,58–10,1) | %95 (%87–%100) | 0 (0–2) | 2 (0–2) | 0 (0–1) | 3181 (2827–3579) |
| 38 | 9,37 (8,63–10,2) | %100 (%87–%100) | 1 (0–1,5) | 1 (0–2) | 0 (0–0,5) | 3221 (2828–3720) |
| 39 | 9,21 (8,6–10,4) | %100 (%87–%100) | 1 (0–1,5) | 1 (0–3) | 0 | 3328 (2900–3795) |
| 40 | 9,27 (8,53–10,5) | %89 (%87–%100) | 0,5 (0–2) | 1 (0–3) | 0 (0–1) | 3327 (2994–3794) |
| 41 | 9,26 (8,59–10,3) | %89 (%87–%100) | 0,5 (0–1) | 1 (0–2,5) | 0 | 3424 (2982–3922) |
| 42 | 9,61 (8,6–10,4) | %89 (%83–%100) | 1 (0–1,5) | 1 (0–3) | 0 (0–1) | 3430 (2983–3932) |
| 43 | 9,72 (8,32–10,5) | %88 (%79–%100) | 1 (0–1,5) | 1,5 (0–2,5) | 0 (0–0,5) | 3434 (3080–4060) |
| 44 | 9,5 (8,23–10,5) | %88 (%78–%100) | 0,5 (0–2) | 1 (0–3) | 0 | 3550 (3120–4041) |
| 45 | 9,17 (8,14–10,7) | %89 (%82–%100) | 0 (0–1) | 1 (0–3) | 0 (0–1) | 3564 (3150–4080) |
| 46 | 9,22 (8,18–10,8) | %88 (%79–%100) | 0,5 (0–1) | 1 (0–3) | 0 | 3637 (3104–4176) |
| 47 | 9,31 (8,1–10,9) | %88 (%79–%100) | 1 (0–2) | 1,5 (0–3) | 0 (0–0,5) | 3563 (3146–4198) |
| 48 | 9,37 (8,16–10,8) | %88 (%78–%100) | 1 (0–1,5) | 1 (0–2,5) | 0 (0–1) | 3709 (3184–4312) |
| 49 | 9,41 (8,06–10,8) | %87 (%78–%100) | 1,5 (0–2) | 1 (0–3) | 0 (0–1) | 3781 (3282–4293) |
| 50 | 9,41 (8,06–10,8) | %87 (%78–%100) | 0,5 (0–1,5) | 1 (0–2,5) | 0 (0–0,5) | 3830 (3307–4386) |
| 51 | 9,46 (8,21–10,9) | %88 (%78–%100) | 0,5 (0–2) | 1,5 (0–3,5) | 0 (0–1) | 3817 (3316–4478) |
| 52 | 9,54 (8,13–11,3) | %87 (%78–%100) | 0 (0–1,5) | 1 (0–2) | 0 (0–1,5) | 3873 (3282–4409) |
| 53 | 9,64 (8,18–11,4) | %88 (%76–%100) | 0,5 (0–3) | 1,5 (0–2,5) | 0 (0–1) | 4010 (3358–4543) |
| 54 | 9,71 (8,01–11,3) | %88 (%76–%100) | 1 (0–2) | 1 (0–3,5) | 0 (0–0,5) | 4068 (3367–4464) |
| 55 | 9,54 (8,17–11,4) | %88 (%78–%100) | 1 (0–2) | 1 (0–2,5) | 0 (0–1) | 4063 (3348–4522) |
| 56 | 9,69 (8,25–11,5) | %88 (%78–%100) | 0,5 (0–2) | 1 (0–2,5) | 0 (0–1) | 4080 (3374–4642) |
| 57 | 9,76 (8,33–10,8) | %88 (%78–%100) | 1 (0–3,5) | 1 (0–2) | 0 (0–1) | 4072 (3422–4715) |
| 58 | 9,76 (8,47–10,8) | %88 (%79–%100) | 1 (0–1,5) | 1,5 (0–3,5) | 0 (0–0,5) | 4166 (3435–4795) |
| 59 | 9,82 (8,32–10,6) | %86 (%78–%91) | 0 (0–1,5) | 1 (0–2) | 0 (0–1,5) | 4188 (3466–4778) |
| 60 | 9,82 (8,37–10,7) | %85 (%78–%91) | 0 (0–2) | 1,5 (0–3) | 0 (0–1) | 4261 (3466–4864) |

### Medeniyet (3/4)

| Yıl | Ortalama çağ | Araştırma ağacının biten payı (ort.) | Ağacı bitmiş medeniyet payı | Araştırması duran medeniyet payı | Altın medyanı (medeniyetler) | Boştaki iş gücü payı |
|---|---|---|---|---|---|---|
| 1 | 1 | %0,4 (%0–%0,8) | %0 | %0 | 8,42 (8,29–15,9) | %0 |
| 2 | 1 | %4 (%3,5–%4,3) | %0 | %0 | 24,4 (13,2–37,5) | %0 |
| 3 | 1,13 (1–1,29) | %8 (%7,4–%8,5) | %0 | %0 | 35,7 (21,2–48,2) | %0,3 (%0–%2) |
| 4 | 1,88 (1,76–2) | %12 (%11–%13) | %0 | %0 | 42,1 (29,3–60,1) | %0,3 (%0–%1,2) |
| 5 | 2 (1,86–2) | %15 (%14–%16) | %0 | %0 | 50,4 (38,1–65,6) | %0,1 (%0–%1,3) |
| 6 | 2 (2–2,13) | %19 (%18–%20) | %0 | %0 | 58,9 (42,1–78,6) | %0,3 (%0–%1,5) |
| 7 | 2,14 (2–2,27) | %24 (%23–%26) | %0 | %0 | 68,1 (53,2–99,5) | %0,6 (%0,1–%2,1) |
| 8 | 2,44 (2,24–2,6) | %29 (%28–%32) | %0 | %0 (%0–%5,6) | 71,2 (48,6–101) | %0,9 (%0,1–%2) |
| 9 | 2,56 (2,36–2,75) | %35 (%33–%39) | %0 | %0 (%0–%5,6) | 73,3 (54,7–137) | %0,9 (%0,2–%1,9) |
| 10 | 2,76 (2,6–2,94) | %42 (%40–%46) | %0 | %0 (%0–%17) | 87,3 (59,2–161) | %1,4 (%0,1–%2,7) |
| 11 | 3 (2,79–3,13) | %49 (%46–%52) | %0 | %0 (%0–%25) | 152 (97,8–220) | %2,2 (%0,6–%3,6) |
| 12 | 3,06 (2,93–3,18) | %55 (%52–%59) | %0 | %13 (%0–%31) | 177 (66,8–294) | %3,4 (%1,2–%5,7) |
| 13 | 3,13 (3–3,35) | %61 (%59–%66) | %0 | %33 (%12–%44) | 246 (149–412) | %5,4 (%2,2–%8,9) |
| 14 | 3,38 (3–3,49) | %66 (%63–%70) | %0 (%0–%14) | %31 (%12–%60) | 436 (181–592) | %10 (%6,7–%14) |
| 15 | 3,38 (3–3,63) | %72 (%65–%75) | %12 (%0–%14) | %33 (%12–%76) | 636 (312–916) | %13 (%11–%19) |
| 16 | 3,56 (3,13–3,73) | %76 (%67–%79) | %14 (%12–%31) | %53 (%40–%71) | 837 (368–1190) | %18 (%13–%23) |
| 17 | 3,59 (3,31–3,73) | %79 (%71–%82) | %27 (%13–%40) | %57 (%40–%76) | 1102 (459–1619) | %23 (%20–%28) |
| 18 | 3,63 (3,34–3,75) | %82 (%75–%84) | %38 (%24–%50) | %73 (%56–%82) | 1526 (669–1749) | %29 (%23–%36) |
| 19 | 3,71 (3,38–3,88) | %84 (%77–%86) | %47 (%24–%60) | %73 (%47–%87) | 1731 (960–2621) | %32 (%26–%39) |
| 20 | 3,73 (3,5–3,94) | %86 (%78–%89) | %47 (%27–%62) | %65 (%56–%87) | 2223 (1402–2951) | %33 (%30–%43) |
| 21 | 3,82 (3,59–3,94) | %88 (%81–%91) | %56 (%33–%75) | %75 (%60–%87) | 2552 (1341–3529) | %38 (%33–%43) |
| 22 | 3,86 (3,71–3,94) | %89 (%85–%93) | %62 (%33–%83) | %78 (%60–%88) | 2837 (1521–4039) | %42 (%37–%45) |
| 23 | 3,86 (3,71–4) | %91 (%87–%93) | %71 (%50–%94) | %87 (%67–%100) | 3391 (1588–4180) | %44 (%38–%50) |
| 24 | 3,94 (3,71–4) | %92 (%88–%94) | %76 (%60–%94) | %88 (%75–%100) | 2768 (1622–4374) | %43 (%41–%51) |
| 25 | 4 (3,75–4) | %93 (%90–%95) | %86 (%69–%89) | %88 (%75–%100) | 3331 (2007–4518) | %46 (%41–%54) |
| 26 | 4 (3,75–4) | %94 (%90–%95) | %88 (%75–%94) | %89 (%86–%100) | 3350 (2014–5667) | %46 (%40–%52) |
| 27 | 4 (3,76–4) | %95 (%90–%96) | %89 (%75–%100) | %100 (%81–%100) | 4391 (2083–6200) | %48 (%45–%50) |
| 28 | 4 (3,83–4) | %95 (%90–%96) | %89 (%75–%100) | %100 (%75–%100) | 4457 (2371–7203) | %48 (%45–%52) |
| 29 | 4 (3,83–4) | %96 (%91–%97) | %89 (%75–%100) | %100 (%87–%100) | 4913 (2541–8219) | %48 (%44–%53) |
| 30 | 4 (3,88–4) | %96 (%92–%98) | %89 (%76–%100) | %89 (%87–%100) | 4682 (3081–9166) | %50 (%43–%53) |
| 31 | 4 (3,88–4) | %96 (%92–%99) | %94 (%82–%100) | %100 (%87–%100) | 5194 (2892–10033) | %49 (%46–%55) |
| 32 | 4 (3,88–4) | %96 (%93–%99) | %100 (%87–%100) | %100 (%87–%100) | 7092 (3246–10567) | %50 (%47–%53) |
| 33 | 4 (3,94–4) | %96 (%93–%99) | %100 (%84–%100) | %100 (%87–%100) | 5379 (2627–10308) | %49 (%45–%53) |
| 34 | 4 | %96 (%94–%99) | %100 (%89–%100) | %100 (%95–%100) | 5392 (2911–10752) | %49 (%46–%55) |
| 35 | 4 | %96 (%94–%99) | %100 (%88–%100) | %100 (%88–%100) | 4948 (3695–12415) | %50 (%48–%53) |
| 36 | 4 | %96 (%95–%99) | %100 (%88–%100) | %100 (%88–%100) | 5783 (4097–12467) | %49 (%46–%54) |
| 37 | 4 | %96 (%95–%99) | %100 (%89–%100) | %100 (%89–%100) | 5143 (3257–13495) | %51 (%47–%55) |
| 38 | 4 | %96 (%96–%99) | %100 (%89–%100) | %100 (%89–%100) | 6180 (3579–16283) | %49 (%44–%54) |
| 39 | 4 | %96 (%95–%99) | %100 (%95–%100) | %100 (%95–%100) | 5487 (3359–15008) | %50 (%43–%53) |
| 40 | 4 | %97 (%96–%99) | %100 (%89–%100) | %100 (%89–%100) | 5577 (3477–14092) | %48 (%43–%53) |
| 41 | 4 | %97 (%96–%99) | %100 (%95–%100) | %100 (%95–%100) | 5487 (3514–14923) | %48 (%41–%54) |
| 42 | 4 | %97 (%96–%99) | %100 (%88–%100) | %100 (%88–%100) | 6668 (3779–18401) | %49 (%42–%52) |
| 43 | 4 | %97 (%96–%99) | %100 | %100 | 6494 (3311–16235) | %50 (%41–%52) |
| 44 | 4 | %97 (%96–%99) | %100 (%90–%100) | %100 (%90–%100) | 5757 (3536–17599) | %49 (%40–%54) |
| 45 | 4 | %97 (%96–%99) | %100 (%89–%100) | %100 (%89–%100) | 5060 (3701–18721) | %49 (%43–%55) |
| 46 | 4 | %97 (%96–%99) | %100 (%89–%100) | %100 (%89–%100) | 5167 (3688–20417) | %49 (%40–%54) |
| 47 | 4 | %97 (%96–%99) | %100 (%89–%100) | %100 (%89–%100) | 5681 (2917–21545) | %50 (%42–%55) |
| 48 | 4 | %97 (%96–%99) | %100 (%89–%100) | %100 (%89–%100) | 6211 (3718–23770) | %48 (%39–%55) |
| 49 | 4 | %97 (%96–%99) | %100 (%88–%100) | %100 (%88–%100) | 6853 (3249–26171) | %48 (%39–%56) |
| 50 | 4 | %97 (%96–%99) | %100 (%89–%100) | %100 (%89–%100) | 7104 (3240–25989) | %49 (%40–%56) |
| 51 | 4 | %97 (%96–%99) | %100 (%91–%100) | %100 (%91–%100) | 6192 (3393–26030) | %48 (%40–%56) |
| 52 | 4 | %97 (%96–%99) | %100 (%95–%100) | %100 (%95–%100) | 6324 (3208–27419) | %47 (%41–%56) |
| 53 | 4 | %97 (%96–%99) | %100 | %100 | 6165 (2889–29484) | %48 (%44–%54) |
| 54 | 4 | %97 (%96–%99) | %100 (%95–%100) | %100 (%95–%100) | 6575 (3737–24068) | %47 (%42–%54) |
| 55 | 4 | %97 (%96–%99) | %100 (%89–%100) | %100 (%89–%100) | 6974 (4118–27461) | %48 (%42–%54) |
| 56 | 4 | %97 (%96–%99) | %100 (%90–%100) | %100 (%90–%100) | 7267 (3524–28184) | %48 (%43–%54) |
| 57 | 4 | %98 (%96–%99) | %100 (%90–%100) | %100 (%90–%100) | 5369 (3908–41788) | %47 (%41–%53) |
| 58 | 4 | %97 (%96–%99) | %100 (%90–%100) | %100 (%90–%100) | 5225 (3988–38064) | %47 (%42–%54) |
| 59 | 4 | %98 (%96–%99) | %100 (%90–%100) | %100 (%90–%100) | 5187 (3375–36520) | %48 (%43–%56) |
| 60 | 4 | %97 (%97–%99) | %100 (%89–%100) | %100 (%89–%100) | 5585 (3383–37704) | %47 (%42–%56) |

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
| 8 | 0 | 4 |
| 9 | 0 | 5 (4–5) |
| 10 | 0 | 5 (5–6) |
| 11 | 0 | 6 (5,5–6) |
| 12 | 0 | 6 (6–7) |
| 13 | 0 | 7 (6–8) |
| 14 | 0 | 8 (6–8) |
| 15 | 0 | 8 (7–9) |
| 16 | 0 | 9 (7–10) |
| 17 | 0 | 9 (7,5–10) |
| 18 | 0 | 9 (8–10) |
| 19 | 0 | 9 (9–10) |
| 20 | 0 | 9,5 (9–11) |
| 21 | 0 | 10 (9–11) |
| 22 | 0 | 10 (9–12,5) |
| 23 | 0 | 11 (9–12,5) |
| 24 | 0 | 11 (9,5–13) |
| 25 | 0 (0–0,5) | 11,5 (10–13) |
| 26 | 0 | 12 (10–13,5) |
| 27 | 0 | 12,5 (9,5–14) |
| 28 | 0 | 13 (10–14,5) |
| 29 | 0 | 13 (10–15) |
| 30 | 0 | 13 (11–15,5) |
| 31 | 0 | 13 (11–16) |
| 32 | 0 | 13,5 (12–16) |
| 33 | 0 (0–0,5) | 14 (11,5–17) |
| 34 | 0 | 14 (12–17) |
| 35 | 0 | 14 (12,5–18) |
| 36 | 0 | 14,5 (12,5–18) |
| 37 | 0 | 15 (13–18) |
| 38 | 0 | 15 (13,5–18) |
| 39 | 0 | 15,5 (13–18,5) |
| 40 | 0 | 15,5 (13–19,5) |
| 41 | 0 | 15,5 (12,5–18,5) |
| 42 | 0 | 16 (13–19) |
| 43 | 0 | 17 (11,5–19,5) |
| 44 | 0 (0–0,5) | 17 (11–19,5) |
| 45 | 0 (0–1) | 16,5 (11–19,5) |
| 46 | 0 | 16,5 (11,5–20) |
| 47 | 0 (0–0,5) | 17,5 (11–20) |
| 48 | 0 | 17,5 (11,5–19,5) |
| 49 | 0 (0–0,5) | 17,5 (12–20) |
| 50 | 0 | 17,5 (12,5–20) |
| 51 | 0 | 17 (12–20,5) |
| 52 | 0 | 17,5 (12–20) |
| 53 | 0 | 17,5 (12–20,5) |
| 54 | 0 | 18 (12,5–20,5) |
| 55 | 0 (0–0,5) | 17 (12,5–21) |
| 56 | 0 | 17,5 (13–21) |
| 57 | 0 | 17,5 (13–21) |
| 58 | 0 | 17,5 (13–21,5) |
| 59 | 0 | 17,5 (13–21,5) |
| 60 | 0 (0–0,5) | 17,5 (13–21,5) |

### Olaylar

| Yıl | Olay | Büyük olay |
|---|---|---|
| 1 | 37 (28–43,5) | 14 (12–19) |
| 2 | 60,5 (44,5–68,5) | 20,5 (14,5–23,5) |
| 3 | 34 (26,5–48) | 6 (3,5–11) |
| 4 | 47,5 (41,5–63) | 17 (14–23) |
| 5 | 57,5 (45,5–69,5) | 21,5 (17,5–31,5) |
| 6 | 79 (59–87) | 31 (23–36,5) |
| 7 | 93 (81–124) | 35,5 (31,5–55) |
| 8 | 118 (100–142) | 47,5 (41–59) |
| 9 | 126 (104–148) | 49 (40,5–57,5) |
| 10 | 157 (126–224) | 57 (48–83,5) |
| 11 | 178 (127–246) | 59 (52,5–86,5) |
| 12 | 207 (148–254) | 78 (60–93,5) |
| 13 | 204 (158–234) | 72,5 (59–86,5) |
| 14 | 192 (152–240) | 71 (56–92,5) |
| 15 | 200 (156–250) | 69 (54–95) |
| 16 | 198 (146–252) | 71 (46–89,5) |
| 17 | 172 (126–246) | 55,5 (44–83,5) |
| 18 | 190 (154–240) | 61 (51,5–86) |
| 19 | 166 (148–212) | 58 (44,5–68,5) |
| 20 | 169 (146–220) | 59 (48,5–74) |
| 21 | 174 (146–216) | 57 (43–75) |
| 22 | 176 (124–203) | 57 (38,5–77) |
| 23 | 180 (131–231) | 52,5 (35,5–72) |
| 24 | 158 (134–202) | 49 (36,5–66) |
| 25 | 182 (130–228) | 57 (42–73,5) |
| 26 | 157 (116–209) | 49 (29–71) |
| 27 | 156 (106–192) | 47 (31–55,5) |
| 28 | 148 (114–206) | 48,5 (31,5–70,5) |
| 29 | 162 (128–212) | 46 (35–63) |
| 30 | 168 (120–222) | 54 (33,5–77) |
| 31 | 161 (126–245) | 52,5 (40–83) |
| 32 | 150 (118–238) | 45,5 (31–72) |
| 33 | 154 (99,5–200) | 43,5 (27,5–71) |
| 34 | 156 (121–202) | 50,5 (33,5–61,5) |
| 35 | 152 (108–202) | 43 (28,5–72) |
| 36 | 168 (102–210) | 46,5 (28,5–63,5) |
| 37 | 180 (110–202) | 56 (28–70) |
| 38 | 153 (105–178) | 48 (26,5–60,5) |
| 39 | 162 (120–208) | 52 (31–64,5) |
| 40 | 154 (103–244) | 56 (21–79,5) |
| 41 | 155 (108–215) | 40,5 (25–64,5) |
| 42 | 168 (136–226) | 55,5 (43–74) |
| 43 | 172 (138–236) | 50 (40,5–75,5) |
| 44 | 179 (142–236) | 50,5 (33–81) |
| 45 | 194 (125–224) | 53 (29–70,5) |
| 46 | 175 (136–233) | 54 (39,5–74) |
| 47 | 194 (132–247) | 69 (42–83) |
| 48 | 202 (123–254) | 58 (35–82,5) |
| 49 | 192 (118–248) | 50,5 (23,5–83) |
| 50 | 190 (146–233) | 58,5 (39–73) |
| 51 | 187 (120–254) | 57,5 (29–81,5) |
| 52 | 186 (116–238) | 58 (29,5–76,5) |
| 53 | 190 (121–256) | 59,5 (34,5–81) |
| 54 | 167 (132–268) | 50 (37,5–86,5) |
| 55 | 187 (122–255) | 64,5 (31,5–82,5) |
| 56 | 189 (108–293) | 58 (26,5–88) |
| 57 | 185 (120–256) | 53 (34–71,5) |
| 58 | 196 (135–248) | 56 (40–74) |
| 59 | 188 (134–292) | 58,5 (36–78) |
| 60 | 216 (127–311) | 66 (38,5–84) |

### Savaş (1/2)

| Yıl | Muharebe | Başlayan savaş | Süren savaş (yıl sonu) | Yıl içinde süren savaş | Yağma akını (medeniyet) | Tarihî hak savaşı |
|---|---|---|---|---|---|---|
| 1 | 0 | 0 | 0 | 0 | 0 | 0 |
| 2 | 0 | 0 | 0 | 0 | 0 | 0 |
| 3 | 0 (0–1) | 0 | 0 | 0 | 0 | 0 |
| 4 | 4 (3–5) | 0 | 0 | 0 | 0 | 0 |
| 5 | 4,5 (2,5–6) | 0 | 0 | 0 | 0 | 0 |
| 6 | 5 (4–7) | 0 | 0 | 0 | 0 | 0 |
| 7 | 6 (4–7) | 0 | 0 | 0 | 0 | 0 |
| 8 | 6,5 (4–8) | 0 | 0 | 0 | 0 (0–0,5) | 0 |
| 9 | 6,5 (4–9) | 0 (0–0,5) | 0 (0–0,5) | 0 (0–1) | 0 (0–1) | 0 |
| 10 | 8,5 (6–11) | 0 (0–2) | 0 | 0 (0–2) | 0 (0–1,5) | 0 |
| 11 | 8,5 (5–13,5) | 0,5 (0–1) | 0 | 1 (0–1) | 0 (0–2) | 0 |
| 12 | 10,5 (6–13) | 0 (0–1,5) | 0 (0–0,5) | 0,5 (0–1,5) | 0,5 (0–1,5) | 0 (0–0,5) |
| 13 | 9 (7–13) | 1 (0–2) | 0 (0–1) | 1 (0–2,5) | 1 (0–2) | 0 (0–0,5) |
| 14 | 8 (6–10,5) | 0 (0–2) | 0 | 1 (0–2,5) | 1 (0–2) | 0 |
| 15 | 8 (3–12) | 0,5 (0–2) | 0 (0–0,5) | 1 (0–2) | 0 (0–1) | 0 |
| 16 | 8 (2–12) | 0,5 (0–2) | 0 (0–0,5) | 1 (0–2) | 1 (0–2,5) | 0 (0–0,5) |
| 17 | 6 (2,5–10,5) | 1 (0–2,5) | 0 (0–1) | 1 (0–2,5) | 1 (0–1,5) | 0 (0–1) |
| 18 | 6 (4,5–11,5) | 1 (0–2,5) | 0 | 1 (0,5–3,5) | 0,5 (0–2) | 0 (0–1) |
| 19 | 6 (5–8,5) | 1 (0–2) | 0 (0–1) | 1 (0–2) | 1 (0–2) | 0 (0–0,5) |
| 20 | 7 (4,5–10) | 1 (0–3,5) | 0 (0–1) | 1 (1–3,5) | 1 (0–2) | 0 |
| 21 | 8,5 (5–12) | 1 (0–2,5) | 0 (0–1) | 1 (0–3) | 1 (0–3,5) | 0 |
| 22 | 7,5 (5,5–11,5) | 1 (0–4) | 0 (0–2,5) | 1,5 (0–4) | 2 (0,5–2) | 0 (0–1) |
| 23 | 7,5 (6,5–10,5) | 1 (0–2) | 0 (0–0,5) | 2 (1–3) | 1 (0–2,5) | 0 (0–0,5) |
| 24 | 7 (4,5–10) | 1,5 (0–3) | 0 (0–2,5) | 2 (0–3) | 1,5 (0–2,5) | 0 (0–0,5) |
| 25 | 9,5 (4,5–13) | 1 (0–3) | 0 (0–2) | 1,5 (0–3,5) | 2 (0–3) | 0 (0–1) |
| 26 | 8 (5–11) | 1 (0–3) | 0 (0–1) | 2 (0–3) | 1 (0–2,5) | 0 |
| 27 | 9 (4–12) | 1 (0–3) | 0 (0–1) | 1,5 (0,5–3) | 1,5 (0,5–3) | 0 (0–1) |
| 28 | 8 (5–11) | 1,5 (0–3) | 0 (0–1,5) | 2 (0–3) | 1,5 (0–4,5) | 0 (0–1) |
| 29 | 9,5 (6,5–12) | 1 (0–2,5) | 0 (0–2) | 1 (0,5–3,5) | 1 (0–3,5) | 0 (0–1) |
| 30 | 9 (5,5–13) | 1 (0–3,5) | 0 (0–1) | 1 (0–4) | 2 (0,5–3) | 0 |
| 31 | 9 (5–14,5) | 1 (0–3,5) | 0 (0–1) | 1 (0,5–4) | 1,5 (0–3,5) | 0 (0–1) |
| 32 | 9,5 (7–13) | 2 (0–3) | 0 (0–1) | 2,5 (0–3) | 1,5 (0–3) | 0 (0–1) |
| 33 | 10,5 (5–16) | 1 (0–3,5) | 0 (0–1) | 2 (1–3,5) | 2 (0–5) | 0 |
| 34 | 8,5 (5–11,5) | 1 (0–2) | 0 (0–0,5) | 1,5 (0–3) | 1,5 (0–4,5) | 0 |
| 35 | 8,5 (4–12) | 1 (0–5) | 0 (0–1,5) | 1 (0,5–5) | 2 (0–3,5) | 0 (0–1) |
| 36 | 9,5 (5–11) | 1 (0–3) | 0 (0–2) | 2 (0–3) | 1,5 (0–4) | 0 (0–1) |
| 37 | 11 (5,5–13) | 1 (0–3,5) | 0 (0–1,5) | 2 (0–4,5) | 1 (0–3) | 0 (0–1) |
| 38 | 10 (5,5–13,5) | 1 (0–2,5) | 0 (0–1) | 1,5 (0–3) | 2 (0–4,5) | 0 (0–0,5) |
| 39 | 10 (6–13,5) | 1 (0–3,5) | 0 (0–0,5) | 1,5 (0–4) | 2 (0–3) | 0 |
| 40 | 9 (4,5–12,5) | 2 (0–3,5) | 0 (0–2) | 2 (0,5–3,5) | 1,5 (0–4) | 0 (0–1) |
| 41 | 9 (6–14) | 2 (0–3,5) | 0 (0–1,5) | 2,5 (0–3,5) | 2 (0–3,5) | 0 (0–1) |
| 42 | 10 (6–14,5) | 1 (0–4) | 0 (0–1) | 1,5 (0–4) | 1 (0–3,5) | 0 (0–1) |
| 43 | 10 (7,5–14,5) | 2,5 (0–4) | 0 (0–2) | 3 (0–4) | 2 (0–4) | 0 (0–1) |
| 44 | 9,5 (7–15,5) | 2 (0–3,5) | 0,5 (0–2) | 2,5 (1–4) | 2 (0–5,5) | 0 (0–0,5) |
| 45 | 10 (6,5–13) | 2 (0–3,5) | 0 (0–2) | 2,5 (0,5–5) | 2 (0–4) | 0 (0–1) |
| 46 | 10,5 (7–15) | 1,5 (0–3,5) | 0 (0–2,5) | 2 (0–4,5) | 1,5 (0–5,5) | 0 (0–1) |
| 47 | 11,5 (7–15,5) | 2 (0–4,5) | 0 (0–1,5) | 2,5 (0,5–4,5) | 2,5 (0–4,5) | 0 (0–0,5) |
| 48 | 9 (6,5–12,5) | 0 (0–3,5) | 0 (0–1) | 1 (0–3,5) | 1,5 (0–4,5) | 0 |
| 49 | 8,5 (5,5–14) | 1,5 (0–5) | 0 (0–2) | 2 (1–5) | 2 (0–4,5) | 0 (0–1) |
| 50 | 9,5 (5–13,5) | 1 (0–2,5) | 0 (0–1,5) | 2 (0–4) | 2 (0–6) | 0 |
| 51 | 11 (4,5–14,5) | 2 (0–5) | 0 (0–1) | 2,5 (0,5–5,5) | 2,5 (0–4,5) | 0 |
| 52 | 9,5 (6–13,5) | 1,5 (0–3,5) | 1 (0–2,5) | 2 (0,5–4,5) | 1,5 (0–4) | 0 (0–0,5) |
| 53 | 9 (7–12,5) | 2 (0–4,5) | 0 (0–2,5) | 3 (1–5) | 1,5 (0–4,5) | 0 (0–1) |
| 54 | 10 (5,5–13,5) | 2 (0–5,5) | 1 (0–2) | 3 (0,5–5,5) | 1 (0–4,5) | 0 (0–1) |
| 55 | 10 (6,5–16) | 1,5 (0–4,5) | 0 (0–2,5) | 2 (0,5–5) | 2 (0–5,5) | 0 |
| 56 | 10 (6–14) | 2 (0–4) | 1 (0–1,5) | 2,5 (1–5) | 1 (0–4,5) | 0 (0–1) |
| 57 | 9 (5,5–13) | 1 (0–5) | 1 (0–2) | 2 (1–5) | 2 (0–5) | 0 (0–1) |
| 58 | 8,5 (4–15) | 2 (0–3,5) | 0 (0–2,5) | 3 (1–5) | 1,5 (0–5) | 0 (0–1) |
| 59 | 10 (6,5–15) | 2 (0–2) | 0,5 (0–2) | 2 (0–5) | 1,5 (0–5) | 0 |
| 60 | 11 (7–16) | 2 (0–3,5) | 0 (0–2) | 3 (1–5) | 1,5 (0–6) | 0 (0–0,5) |

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
| 10 | 0 | 0 | 0 | 0 |
| 11 | 0 | 0 | 0 | 0 |
| 12 | 0 | 0 | 0 | 0 (0–0,5) |
| 13 | 0 | 0 | 0 | 0 (0–1) |
| 14 | 0 | 0 (0–0,5) | 0 | 0 (0–1) |
| 15 | 0 | 0 | 0 | 0 (0–1) |
| 16 | 0 (0–0,5) | 0 | 0 | 0 (0–1,5) |
| 17 | 0 (0–1) | 0 | 0 | 0 (0–1,5) |
| 18 | 0 (0–0,5) | 0 | 0 | 0 (0–2) |
| 19 | 0 | 0 | 0 | 0,5 (0–2) |
| 20 | 0 (0–1) | 0 | 0 (0–0,5) | 0 (0–2) |
| 21 | 0 | 0 | 0 | 1 (0–2) |
| 22 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 23 | 0 (0–1) | 0 (0–0,5) | 0 | 1 (0–2) |
| 24 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 25 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 26 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 27 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 28 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 29 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 30 | 0 (0–1,5) | 0 | 0 | 1 (0–2,5) |
| 31 | 0 | 0 | 0 | 1 (0–2) |
| 32 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 33 | 0 (0–1) | 0 (0–0,5) | 0 | 1 (0–2) |
| 34 | 0 | 0 | 0 | 1 (0–2) |
| 35 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 36 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 37 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 38 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 39 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 40 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 41 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 42 | 0 (0–0,5) | 0 (0–0,5) | 0 | 1 (0–2) |
| 43 | 0 (0–1) | 0 (0–1) | 0 | 1 (0–2) |
| 44 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 45 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 46 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 47 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 48 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 49 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 50 | 0 (0–1) | 0 | 0 | 1,5 (0–2) |
| 51 | 0 (0–1) | 0 | 0 | 1,5 (0–2) |
| 52 | 0 (0–1) | 0 (0–0,5) | 0 | 1 (0–2) |
| 53 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 54 | 0 (0–1) | 0 | 0 | 1 (0,5–2) |
| 55 | 0 (0–1) | 0 | 0 | 1 (0,5–2,5) |
| 56 | 0 (0–1) | 0 | 0 | 1 (0,5–2,5) |
| 57 | 0 (0–0,5) | 0 | 0 | 1 (0,5–3) |
| 58 | 0 (0–1) | 0 | 0 | 1,5 (0,5–2,5) |
| 59 | 0 (0–1) | 0 | 0 | 1,5 (0,5–3) |
| 60 | 0 (0–1) | 0 | 0 | 1,5 (0,5–3) |

### Canavarlar (1/2)

| Yıl | Yaşayan kamp (yıl sonu) | Yaşayan kamp (yıl ort.) | Doğan kamp | Temizlenen kamp | Canavar baskını | Yaşayan trol ini (yıl sonu) |
|---|---|---|---|---|---|---|
| 1 | 3 | 3 | 0 | 0 | 0 | 0 |
| 2 | 4 (3–5) | 3,1 (3–3,48) | 1 (0–2) | 0 | 0 | 0 |
| 3 | 5 (4–6,5) | 4,3 (3,4–5,59) | 1 (0–2,5) | 0 | 0 (0–0,5) | 0 |
| 4 | 7 (4,5–8,5) | 6,13 (4,08–7,23) | 2 (1–3) | 0 | 3,5 (2,5–5) | 0 |
| 5 | 7 (6,5–10) | 7 (6–9,71) | 1 (0–2,5) | 0 | 4 (2,5–6) | 0 |
| 6 | 9 (7–10,5) | 8,38 (6,7–10) | 1 (0–2) | 0 | 5 (3,5–6) | 0 |
| 7 | 10 (8–11) | 9,85 (7,8–10,7) | 0,5 (0–2) | 0 | 5 (3,5–7) | 0 |
| 8 | 10 (10–11) | 10 (8,93–11) | 0 (0–1,5) | 0 | 6 (3,5–8) | 0 |
| 9 | 10 (10–11) | 10 (10–11) | 0 | 0 | 5,5 (3,5–7,5) | 0 |
| 10 | 10 (9–11) | 10 (9,9–11) | 0 (0–1) | 0 (0–1,5) | 7 (5,5–8) | 0 |
| 11 | 10 (8,5–11) | 9,9 (8,62–10,9) | 0,5 (0–1) | 1 (0–2,5) | 6 (4–8) | 0 |
| 12 | 9,5 (6,5–11) | 9,5 (7,3–11,1) | 1 (0–2,5) | 2 (0,5–4) | 6 (3–8,5) | 0 |
| 13 | 7,5 (4,5–11,5) | 8,33 (5,86–11,3) | 1 (0–2) | 2 (0–5) | 5 (3,5–7) | 0 |
| 14 | 7 (4–10,5) | 6,92 (4,45–11) | 1 (0–3) | 2 (0,5–3,5) | 4 (2–6) | 0 |
| 15 | 5,5 (4–10) | 6,1 (4–9,89) | 2 (0,5–3) | 2 (1–4) | 3,5 (1,5–6) | 0 (0–0,5) |
| 16 | 7 (4,5–9) | 5,38 (4,03–8,79) | 2 (0–4) | 2 (1–3) | 3,5 (1–6) | 0 (0–0,5) |
| 17 | 6 (5–7) | 6,54 (4,84–8,11) | 1,5 (0–2) | 2 (0,5–3,5) | 3 (1–5) | 0 (0–1) |
| 18 | 6,5 (5–7,5) | 6,68 (5,49–7,54) | 2 (0,5–2,5) | 2 (0–2) | 2 (1–5) | 0 (0–1) |
| 19 | 7 (5,5–8) | 6,58 (5,41–8,18) | 2 (0,5–3) | 2 (0–3) | 3 (2–4) | 0,5 (0–1) |
| 20 | 7 (6–8,5) | 7,01 (6,21–8,16) | 2 (1–2,5) | 2 (0,5–3) | 3 (1,5–4) | 0 (0–1) |
| 21 | 7 (6–8,5) | 6,7 (5,95–7,98) | 2 (1–4) | 2 (1–3,5) | 3,5 (1,5–4,5) | 0 (0–1,5) |
| 22 | 7 (6–9) | 7,14 (6,25–8,57) | 2 (1–3,5) | 2 (1–3) | 4 (1–5) | 0,5 (0–1,5) |
| 23 | 7 (6,5–8,5) | 7,42 (6,46–8,69) | 1,5 (0–2) | 1 (1–3) | 3,5 (2–5) | 0 (0–2,5) |
| 24 | 8 (7–10) | 8,03 (7,55–9) | 2 (1–4) | 1 (0–2) | 3 (2–4) | 1 (0–3) |
| 25 | 8 (7–9,5) | 8,06 (7,2–9,49) | 1,5 (0,5–3,5) | 2 (0–4,5) | 3 (1,5–5,5) | 1 (0–2,5) |
| 26 | 8 (7–9) | 8,04 (7,28–9,02) | 1 (0–2) | 1 (0–3) | 3,5 (1,5–5,5) | 1 (0–2,5) |
| 27 | 8 (7–9) | 8,03 (7,25–9,28) | 2 (1–3) | 2 (0–3) | 4 (1–6) | 1 (0–2,5) |
| 28 | 9 (7,5–9,5) | 8,03 (7,65–9,2) | 2 (0,5–3) | 1 (0–2,5) | 3,5 (2–5) | 1 (0–2) |
| 29 | 8 (6,5–9,5) | 8,5 (6,88–9,57) | 1 (0–3) | 2 (1–3,5) | 3,5 (2–5) | 1 (0–2) |
| 30 | 9 (8–11) | 8,84 (7,78–10,1) | 3 (1,5–4,5) | 2 (0,5–4) | 3,5 (2–6) | 1 (0–2,5) |
| 31 | 9 (7,5–10) | 8,93 (7,77–10,3) | 2 (1–3,5) | 2 (1–3,5) | 3 (1–7) | 1 (0–3) |
| 32 | 9 (6,5–10) | 8,98 (7,45–10,2) | 2 (0,5–3,5) | 2,5 (1–3) | 4 (1–6) | 1 (0–2,5) |
| 33 | 9 (6,5–10) | 8,77 (7,08–9,86) | 1,5 (1–2,5) | 2 (1–3,5) | 3,5 (2–6) | 1 (0–2) |
| 34 | 8 (6,5–9,5) | 8,34 (7–9,59) | 2 (1–3) | 2 (0,5–3) | 3,5 (2–5) | 1 (0–2) |
| 35 | 8,5 (7,5–10,5) | 8,51 (6,88–9,53) | 2 (1–3,5) | 1 (1–3) | 3 (2–5) | 1 (0,5–2) |
| 36 | 10 (7,5–10,5) | 9,28 (7,25–11,2) | 2 (1–3) | 1,5 (0–3) | 4 (1–5,5) | 2 (0,5–3) |
| 37 | 10 (7,5–10) | 9,63 (8,76–10,4) | 1 (1–3) | 2,5 (1–3,5) | 5 (2,5–6,5) | 1 (0–2,5) |
| 38 | 9 (8–11) | 9,51 (8,01–10,9) | 2 (0–3,5) | 2 (0–3,5) | 4 (2–7) | 1 (0–3) |
| 39 | 9 (7–10,5) | 9,69 (7,71–10,5) | 2 (0,5–3) | 2 (0,5–4,5) | 5 (2–6,5) | 1 (0,5–2) |
| 40 | 10 (6–11) | 9,62 (6,87–10,4) | 2 (1–3,5) | 2 (0–2,5) | 4 (1,5–5) | 1 (0–2,5) |
| 41 | 9,5 (6,5–10,5) | 9,65 (7,06–10,4) | 1,5 (0–3) | 2 (0–3,5) | 3,5 (1,5–6) | 1 (0–2,5) |
| 42 | 9 (7–11,5) | 9,74 (6,98–10,8) | 2 (1,5–4) | 2 (1–3) | 4 (1,5–7) | 1 (0–3) |
| 43 | 9,5 (6,5–11,5) | 9,02 (7,09–11,4) | 2 (0,5–3,5) | 2 (1–3) | 4 (2–6) | 2 (0–3) |
| 44 | 10 (7–11,5) | 9,34 (7,44–11,4) | 2 (1,5–3,5) | 2 (1–3,5) | 3,5 (1,5–6,5) | 1 (0–3) |
| 45 | 10 (8–11,5) | 9,64 (6,98–11,6) | 2 (1–3,5) | 2 (1–3) | 4 (2,5–5) | 1 (0–3) |
| 46 | 10 (8,5–12) | 10,1 (7,68–12) | 2 (1–3) | 2,5 (0,5–3) | 4 (2–6) | 2 (0,5–3) |
| 47 | 10 (6,5–11) | 9,93 (7,49–11,2) | 2 (1–3,5) | 2 (1–4,5) | 3,5 (2–5,5) | 2 (1–3) |
| 48 | 9 (6,5–11) | 9,36 (6,8–10,6) | 2 (1,5–3,5) | 2,5 (2–4) | 3 (1–5) | 1 (0–3) |
| 49 | 10 (6,5–12) | 9,33 (6,95–11,5) | 2 (1–4,5) | 1 (0–4) | 3 (1–5,5) | 2 (0–3) |
| 50 | 11 (6,5–12) | 10,4 (6,62–11,9) | 2 (1–3,5) | 2 (0,5–3) | 3 (0,5–5,5) | 2 (0,5–3) |
| 51 | 10,5 (7–12) | 10,3 (6,55–11,8) | 2 (1–3) | 2 (0–3,5) | 3 (1–6) | 2 (0,5–3) |
| 52 | 11 (7–13,5) | 10,6 (7,28–12,5) | 2,5 (1–4,5) | 2 (1–4) | 3,5 (1–5,5) | 2 (0,5–3) |
| 53 | 11 (7,5–12,5) | 10,9 (7,09–12,4) | 2 (0–3,5) | 1 (1–3) | 3 (1–6,5) | 2 (0,5–3) |
| 54 | 11 (7,5–12,5) | 10,9 (7,32–12,6) | 2 (1–3) | 2,5 (1–3,5) | 3,5 (1–7) | 2 (1–3) |
| 55 | 10 (8–12,5) | 10,7 (7,72–12,4) | 1 (1–3) | 2 (1–3,5) | 3,5 (2–6) | 2 (1–3) |
| 56 | 10 (7,5–13) | 10,3 (8,27–12,7) | 2 (1–4) | 2 (1–3) | 4 (1–5) | 2 (1,5–3) |
| 57 | 11 (6,5–13) | 10,8 (7,61–12,9) | 2 (1–3) | 1 (0,5–2) | 3,5 (1,5–5,5) | 2 (1–3) |
| 58 | 11 (8–13) | 11 (7,13–13,2) | 2 (1–2,5) | 1 (1–3,5) | 3 (1–6) | 2 (1–3) |
| 59 | 12 (8–13) | 11,9 (8–13,5) | 3 (0–4) | 2 (1–3) | 3 (1,5–6) | 2 (1–3) |
| 60 | 12 (8,5–14) | 11,7 (8,48–13,7) | 2 (1–3) | 2 (0–3,5) | 4 (1–6,5) | 2 (1–3) |

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
| 8 | 0 | 0 | 0 | 0 (0–0,5) |
| 9 | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) |
| 10 | 0 | 0 | 0 | 0 (0–1) |
| 11 | 0 | 0 | 0 | 0 (0–1) |
| 12 | 0 | 0 | 0 | 0 (0–0,5) |
| 13 | 0 | 0 | 0 | 0 (0–1) |
| 14 | 0 | 0 | 0 | 0 |
| 15 | 0 | 0 | 0 (0–1) | 0 |
| 16 | 0 | 0 | 0 (0–1) | 0 |
| 17 | 0 | 0 | 0,5 (0–1) | 0 |
| 18 | 0 (0–1) | 0 (0–1) | 0 (0–1) | 0 |
| 19 | 0 (0–1) | 0 (0–1) | 1 (0–1) | 0 |
| 20 | 0,5 (0–1) | 0 (0–1) | 0 (0–1) | 0 |
| 21 | 1 (0–1) | 0 (0–1,5) | 0 (0–1) | 0 |
| 22 | 1 | 1 (0–2) | 0 (0–1) | 0 |
| 23 | 1 | 1 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 24 | 1 | 1 (0,5–1,5) | 0 (0–1) | 0 |
| 25 | 1 | 1 (0–1) | 0 (0–1) | 0 (0–0,5) |
| 26 | 1 (0,5–1) | 1 (0–2) | 0 (0–1) | 0 |
| 27 | 1 (0,5–1) | 0,5 (0–1,5) | 0 (0–1) | 0 |
| 28 | 1 (0,5–1) | 1 (0–1,5) | 0,5 (0–1) | 0 (0–0,5) |
| 29 | 1 (0,5–1) | 1 (0–2) | 0 (0–1) | 0 (0–1) |
| 30 | 1 (0,5–1) | 1 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 31 | 1 (0,5–1) | 0 (0–1,5) | 0 (0–1) | 0 |
| 32 | 1 (0,5–1) | 0,5 (0–1) | 1 (0–1) | 0 (0–1) |
| 33 | 1 (0,5–1) | 1 (0–2) | 0 (0–0,5) | 0 (0–0,5) |
| 34 | 1 (0,5–1) | 0 (0–1) | 0 (0–1) | 0 |
| 35 | 1 (0,5–1) | 0 (0–1) | 0,5 (0–1) | 0 |
| 36 | 1 (0,5–1) | 1 (0–1,5) | 0 (0–1) | 0 |
| 37 | 1 (0,5–1) | 1 (0–1) | 0 (0–1) | 0 |
| 38 | 1 (0,5–1) | 0 (0–1) | 0 (0–1) | 0 (0–0,5) |
| 39 | 1 (0,5–1) | 1 (0–2) | 0 (0–1) | 0 |
| 40 | 1 (0,5–1) | 1 (0–1) | 0 (0–1) | 0 (0–1) |
| 41 | 1 (0,5–1) | 0,5 (0–1,5) | 0 (0–1) | 0 |
| 42 | 1 (0–1) | 1 (0–1) | 0,5 (0–1) | 0 |
| 43 | 1 (0–1) | 0,5 (0–1) | 0 (0–1) | 0 (0–1) |
| 44 | 1 (0–1) | 0 (0–1,5) | 0 (0–1) | 0 (0–1) |
| 45 | 1 (0–1) | 0 (0–1) | 0 (0–1) | 0 |
| 46 | 1 (0–1) | 0 (0–1) | 0,5 (0–1) | 0 (0–0,5) |
| 47 | 1 (0–1) | 0 (0–1,5) | 0 (0–1) | 0 (0–0,5) |
| 48 | 1 (0–1) | 0 (0–1) | 0 (0–1) | 0 (0–1) |
| 49 | 1 (0–1) | 0 (0–1) | 0 (0–1) | 0 |
| 50 | 1 (0–1) | 0 (0–1) | 0,5 (0–1) | 0 |
| 51 | 1 (0–1) | 0 (0–1,5) | 0 (0–1) | 0 |
| 52 | 1 (0–1) | 0 (0–1) | 1 (0–1) | 0 |
| 53 | 1 (0–1) | 0 (0–1) | 0 (0–1) | 0 (0–0,5) |
| 54 | 1 (0–1) | 0 (0–1) | 0,5 (0–1) | 0 (0–0,5) |
| 55 | 1 (0–1) | 0 (0–1,5) | 0 (0–1) | 0 (0–0,5) |
| 56 | 1 (0–1) | 0 (0–1,5) | 0 (0–1) | 0 (0–0,5) |
| 57 | 1 (0–1) | 0 (0–1) | 0 (0–1) | 0 |
| 58 | 1 (0–1) | 0 (0–1) | 0 (0–1) | 0 (0–0,5) |
| 59 | 1 (0–1) | 0 (0–1) | 0,5 (0–1) | 0 |
| 60 | 1 (0–1) | 0 (0–1) | 0 (0–0,5) | 0 (0–1) |

### Kahramanlar (1/2)

| Yıl | Doğan kahraman | Ölen kahraman | Emekli olan kahraman | Diyarı terk eden kahraman | Ölümden dönen kahraman | Efsane olan kahraman |
|---|---|---|---|---|---|---|
| 1 | 0 (0–0,5) | 0 | 0 | 0 | 0 | 0 |
| 2 | 1 (0–2) | 0 | 0 | 0 | 0 | 0 |
| 3 | 1 (0–2) | 0 | 0 | 0 | 0 | 0 |
| 4 | 0 (0–2) | 0 (0–1) | 0 | 0 (0–0,5) | 0 | 0 |
| 5 | 1 (0–2) | 0 (0–1) | 0 | 0 (0–0,5) | 0 | 0 |
| 6 | 1 (0–1,5) | 0 | 0 | 0 (0–0,5) | 0 | 0 |
| 7 | 2 (0–3,5) | 0 (0–0,5) | 0 | 0 | 0 | 0 |
| 8 | 1 (0–4) | 0 (0–1) | 0 | 0 | 0 | 0 |
| 9 | 3,5 (1–6,5) | 0 (0–0,5) | 0 | 0 | 0 | 0 |
| 10 | 2,5 (1–7) | 0 (0–2) | 0 | 0 | 0 | 0 |
| 11 | 3 (1,5–6,5) | 0 (0–2,5) | 0 | 0 | 0 | 0 |
| 12 | 3 (0,5–6,5) | 1 (0–4,5) | 0 | 0 | 0 | 0 |
| 13 | 3 (0–6) | 1 (0–2,5) | 0 | 0 | 0 | 0 |
| 14 | 2 (0,5–6) | 1 (0–2,5) | 0 | 0 | 0 | 0 |
| 15 | 1 (0–4,5) | 1 (0–2,5) | 0 | 0 | 0 | 0 |
| 16 | 2,5 (1–4,5) | 0,5 (0–2,5) | 0 | 0 | 0 | 0 |
| 17 | 2 (0–4) | 0 (0–1) | 0 | 0 | 0 | 0 |
| 18 | 2,5 (1–4,5) | 0 (0–4) | 0 | 0 | 0 | 0 |
| 19 | 3 (2–4,5) | 1 (0–2) | 0 | 0 | 0 | 0 |
| 20 | 2 (1–6) | 0,5 (0–3,5) | 0 | 0 | 0 | 0 |
| 21 | 3,5 (0,5–4,5) | 0 (0–3) | 0 | 0 | 0 | 0 (0–0,5) |
| 22 | 4 (1,5–6) | 0,5 (0–3,5) | 0 | 0 | 0 | 0 |
| 23 | 3 (2–7,5) | 1 (0–2) | 0 | 0 (0–0,5) | 0 | 0 (0–0,5) |
| 24 | 4 (2–5,5) | 1 (0–3) | 0 | 0 | 0 | 0 |
| 25 | 3,5 (1,5–6) | 0,5 (0–2,5) | 0 | 0 (0–1) | 0 | 0 |
| 26 | 4 (0,5–5) | 0,5 (0–4) | 0 | 0 (0–0,5) | 0 | 0 (0–0,5) |
| 27 | 3,5 (1–7) | 0 (0–2,5) | 0 | 0 | 0 | 0 |
| 28 | 2 (0,5–6) | 1 (0–3,5) | 0 (0–0,5) | 0 (0–0,5) | 0 | 0 |
| 29 | 4 (1,5–6,5) | 1 (0–4) | 0 | 0 (0–1) | 0 | 0 |
| 30 | 4 (1,5–5,5) | 0,5 (0–4,5) | 0 | 0 (0–1) | 0 | 0 (0–0,5) |
| 31 | 5 (0,5–8) | 2 (0–5,5) | 0 | 0 (0–1) | 0 | 0 (0–0,5) |
| 32 | 3 (1–5,5) | 0 (0–2) | 0 (0–1) | 0 | 0 | 0 |
| 33 | 3 (1,5–6,5) | 0 (0–4,5) | 0 | 0 (0–1,5) | 0 | 0 (0–0,5) |
| 34 | 4 (2–5,5) | 1 (0–4,5) | 0 (0–0,5) | 0 (0–1) | 0 | 0 (0–0,5) |
| 35 | 3 (1,5–6) | 0,5 (0–5) | 0 (0–1,5) | 0 (0–1) | 0 | 0 |
| 36 | 4 (2–5) | 1 (0–4,5) | 0 (0–1,5) | 0 (0–2) | 0 | 0 (0–0,5) |
| 37 | 4 (1,5–8) | 2,5 (0–5) | 0 | 1 (0–1) | 0 | 0 |
| 38 | 4 (1–7) | 1 (0–3,5) | 0 | 0 (0–1) | 0 | 0 |
| 39 | 4,5 (2–6) | 0 (0–3,5) | 0 (0–0,5) | 0 (0–1,5) | 0 | 0 |
| 40 | 2 (1–6,5) | 0,5 (0–6) | 0,5 (0–2) | 0 (0–1) | 0 | 0 (0–0,5) |
| 41 | 3 (1–6) | 0 (0–2) | 0 (0–1) | 1 (0–2) | 0 | 0 |
| 42 | 4,5 (1,5–6,5) | 3 (1–5) | 0,5 (0–1) | 0,5 (0–2) | 0 | 0 (0–1) |
| 43 | 4 (1,5–5,5) | 1 (0–6) | 0 (0–1,5) | 0 (0–1,5) | 0 | 0 |
| 44 | 4,5 (1,5–8,5) | 1,5 (0–5) | 0,5 (0–1) | 0 (0–1) | 0 | 0 |
| 45 | 3 (2–6) | 0,5 (0–3) | 0 (0–2,5) | 1 (0–2) | 0 | 0 (0–1) |
| 46 | 2,5 (1–5,5) | 2 (0,5–4,5) | 1 (0–2,5) | 0 (0–2) | 0 | 0 |
| 47 | 3 (1–5,5) | 2,5 (0,5–4,5) | 1 (0–2) | 0 (0–2) | 0 | 0 (0–1) |
| 48 | 4 (0,5–5,5) | 1 (0–6) | 0 (0–1,5) | 0 (0–1) | 0 | 0 (0–1) |
| 49 | 3,5 (0–6,5) | 0,5 (0–5,5) | 0 (0–1,5) | 1 (0–2,5) | 0 | 0 |
| 50 | 3 (1–6) | 1 (0–7) | 1 (0–2) | 0 (0–2) | 0 | 0 (0–0,5) |
| 51 | 3,5 (2–6,5) | 1 (0–7) | 0,5 (0–1,5) | 0 (0–1) | 0 | 0 (0–0,5) |
| 52 | 3,5 (1,5–7,5) | 1 (0–3,5) | 0 (0–1,5) | 0 (0–1,5) | 0 | 0 (0–1) |
| 53 | 3,5 (1,5–5) | 2,5 (0–5,5) | 0 (0–3) | 0 (0–2) | 0 | 0 |
| 54 | 3 (0,5–5) | 2 (0–2,5) | 1 (0–3) | 0 (0–1,5) | 0 | 0 (0–1) |
| 55 | 4 (2–6) | 2 (0–8,5) | 1 (0–2) | 0,5 (0–1) | 0 | 0 (0–1) |
| 56 | 4,5 (1–6,5) | 2 (0–6,5) | 0 (0–2) | 0 (0–1,5) | 0 | 0 |
| 57 | 4 (2–6) | 1,5 (0,5–4,5) | 1 (0–2) | 0 (0–1,5) | 0 | 0 (0–0,5) |
| 58 | 4 (2,5–7) | 1 (0–3) | 1 (0–2,5) | 0,5 (0–2) | 0 | 0 |
| 59 | 4,5 (1–9,5) | 2 (0–5) | 1 (0,5–2) | 1 (0–3) | 0 | 0 |
| 60 | 3,5 (1,5–6,5) | 1,5 (0–6) | 1 (0–2,5) | 0 (0–2) | 0 | 0 (0–0,5) |

### Kahramanlar (2/2)

| Yıl | Yaşayan kahraman (yıl sonu) | Doğuş seviyesi (ort.) | Ölüm seviyesi (ort.) | Yaşayan kahraman seviyesi (ort.) | En yüksek seviye (şimdiye dek) |
|---|---|---|---|---|---|
| 1 | 0 (0–0,5) | 2 | – | 2 | 0 (0–1) |
| 2 | 1 (0–2) | 1 (1–2) | – | 1,25 (1–2) | 1 (0–2) |
| 3 | 2 (1–3) | 1 (1–2) | 2 | 1,33 (1–2) | 2 (1–2) |
| 4 | 2 (0–4,5) | 1 (1–1,7) | 1 (1–1,27) | 1,25 (1–1,95) | 2 (1–2) |
| 5 | 2 (1–5) | 1 (1–2) | 1 (1–1,8) | 1,2 (1–2) | 2 (1–2) |
| 6 | 2 (1,5–6) | 2 (1–2) | 1,67 | 1,5 (1–2) | 2 (1–2) |
| 7 | 5 (1,5–8) | 1,29 (1–1,67) | 1,75 (1,55–1,95) | 1,54 (1,07–2) | 2 (1,5–3) |
| 8 | 6,5 (2–10) | 1 (1–1,5) | 1 (1–1,8) | 1,47 (1,08–1,93) | 2 (2–3) |
| 9 | 10 (4,5–16,5) | 1,54 (1–2) | 1,5 (1,1–1,9) | 1,42 (1,32–1,69) | 2 (2–3) |
| 10 | 11,5 (6,5–20,5) | 1,33 (1–2) | 1,33 (1–2) | 1,5 (1,28–1,63) | 2 (2–3) |
| 11 | 14,5 (10–22) | 1,33 (1–1,77) | 1,13 (1–1,54) | 1,56 (1,31–1,76) | 3 (2–3) |
| 12 | 17 (10,5–23,5) | 1,24 (1–1,76) | 1,5 (1–2) | 1,66 (1,4–2,02) | 3 (2–4) |
| 13 | 18,5 (10,5–27,5) | 1,4 (1–1,63) | 1 (1–2) | 1,76 (1,64–2,24) | 3 (3–4) |
| 14 | 20,5 (13–28,5) | 1 (1–1,5) | 1 (1–2) | 1,95 (1,57–2,38) | 4 (3–4) |
| 15 | 22,5 (12,5–28,5) | 1,37 (1–1,98) | 1 (1–1,55) | 2,13 (1,75–2,62) | 4 (3–4,5) |
| 16 | 23 (14,5–29,5) | 1 (1–1,5) | 2 (1,5–2,5) | 2,12 (1,72–2,66) | 4 (3,5–5) |
| 17 | 24,5 (17–30) | 1,33 (1–2) | 1,75 (1–3) | 2,21 (1,77–2,7) | 4 (3,5–5) |
| 18 | 25,5 (19,5–31,5) | 1,13 (1–1,9) | 2,5 (1,12–2,85) | 2,23 (1,9–2,81) | 4 (4–5) |
| 19 | 29 (20,5–34,5) | 1,1 (1–1,5) | 2 (1–2,4) | 2,18 (1,95–2,81) | 4,5 (4–5) |
| 20 | 31 (21–38) | 1 (1–1,58) | 1,3 (1–2,39) | 2,27 (2,02–2,77) | 5 (4–5) |
| 21 | 34 (21,5–38) | 1,25 (1–1,62) | 1,69 (1–2,5) | 2,37 (2,1–2,77) | 5 (4–5,5) |
| 22 | 36,5 (23,5–42) | 1,3 (1–1,6) | 2,17 (1–3,15) | 2,34 (2,04–2,84) | 5 (4–6) |
| 23 | 40,5 (25–45,5) | 1,29 (1–2) | 1,25 (1–2) | 2,43 (2,06–2,86) | 5 (4–6) |
| 24 | 43 (28–49) | 1,2 (1–1,5) | 2 (1–2,74) | 2,38 (2–2,8) | 5 (4–6) |
| 25 | 45 (30–51) | 1,25 (1–1,67) | 2 (1–2,77) | 2,51 (2,04–2,77) | 5 (4–6) |
| 26 | 45,5 (33–53,5) | 1,33 (1–1,93) | 1,63 (1,23–2,4) | 2,47 (2,14–2,81) | 5 (4–6) |
| 27 | 50 (35,5–57) | 1,42 (1–1,71) | 3 (1,8–3,4) | 2,54 (2,17–2,84) | 5,5 (4–6,5) |
| 28 | 51 (38–56) | 1,38 (1–2) | 1,5 (1,16–3,4) | 2,6 (2,15–2,92) | 6 (5–6,5) |
| 29 | 53 (40,5–56,5) | 1,29 (1,07–1,65) | 2 (1–3) | 2,59 (2,22–3,01) | 6 (5–6,5) |
| 30 | 55,5 (41,5–60,5) | 1,29 (1–1,83) | 2,75 (2–3,6) | 2,67 (2,26–3,1) | 6 (5–7) |
| 31 | 55,5 (44,5–62,5) | 1,29 (1–1,57) | 2,08 (1,57–2,64) | 2,73 (2,36–3,15) | 6 (5–7) |
| 32 | 59 (47,5–63,5) | 1,5 (1–1,75) | 2 (1,3–3,7) | 2,75 (2,46–3,21) | 6 (5–7) |
| 33 | 58,5 (51–67,5) | 1,33 (1–1,8) | 2,78 (2–4) | 2,82 (2,44–3,25) | 7 (5–7) |
| 34 | 61,5 (54–71,5) | 1,25 (1–1,9) | 2 (1,27–3,25) | 2,88 (2,52–3,24) | 7 (5,5–7) |
| 35 | 61 (51,5–73) | 1,33 (1–1,67) | 2,75 (1,85–3,53) | 2,85 (2,53–3,23) | 7 (6–7) |
| 36 | 61,5 (53,5–74) | 1,5 (1–1,63) | 2 (1–3,45) | 2,85 (2,59–3,17) | 7 (6–7) |
| 37 | 63 (54,5–74,5) | 1,25 (1–1,87) | 2,5 (1,67–3,8) | 2,88 (2,45–3,37) | 7 (6–7,5) |
| 38 | 65 (54–79) | 1,25 (1–1,62) | 2 (1,27–3,6) | 2,9 (2,55–3,45) | 7 (6–8) |
| 39 | 68,5 (58,5–84) | 1,37 (1–1,63) | 2,67 (1,67–3,75) | 2,93 (2,58–3,32) | 7 (6–8) |
| 40 | 66,5 (59–83,5) | 1,37 (1–1,81) | 2,17 (1,7–3,65) | 3,02 (2,64–3,34) | 7 (6–8) |
| 41 | 69 (59,5–84,5) | 1,38 (1–1,64) | 2,4 (1,75–3) | 3,04 (2,73–3,43) | 7 (6–8) |
| 42 | 69,5 (58–86,5) | 1,5 (1,1–1,83) | 2,33 (1,7–6,52) | 3,04 (2,73–3,43) | 7 (6–8) |
| 43 | 69 (58,5–85,5) | 1,4 (1–2) | 2 (1,23–3,33) | 3,13 (2,77–3,45) | 7 (6,5–8) |
| 44 | 74,5 (58,5–86,5) | 1,45 (1,06–2) | 2 (1,5–4) | 3,18 (2,77–3,47) | 7 (7–8,5) |
| 45 | 74 (60,5–91,5) | 1,33 (1–1,88) | 2,25 (1–3,83) | 3,22 (2,83–3,54) | 7 (7–8,5) |
| 46 | 70 (59–96) | 1,4 (1–1,87) | 2,88 (2–3,85) | 3,27 (2,9–3,59) | 7,5 (7–9) |
| 47 | 69,5 (58,5–96,5) | 1,4 (1–1,9) | 2 (1,71–3,18) | 3,35 (3,04–3,64) | 7,5 (7–9) |
| 48 | 70,5 (56,5–93,5) | 1,45 (1–1,71) | 3 (1,91–3,98) | 3,38 (3,09–3,73) | 8 (7–9) |
| 49 | 72,5 (58,5–90) | 1,33 (1,21–1,71) | 2,83 (2,27–3,25) | 3,35 (3,15–3,85) | 8 (7–9) |
| 50 | 70 (59–89,5) | 1,33 (1–1,83) | 2,67 (1–3,83) | 3,45 (3,04–3,95) | 8 (7–9) |
| 51 | 71 (62–90,5) | 1,46 (1,08–2) | 2,4 (2–3) | 3,52 (3,09–3,9) | 8 (7–9,5) |
| 52 | 72 (64,5–91,5) | 1,5 (1,19–2) | 3,75 (2,98–6,1) | 3,43 (3,06–3,88) | 8 (7–9,5) |
| 53 | 72 (63,5–90,5) | 1,25 (1–1,72) | 3,71 (2–5) | 3,41 (3,09–3,95) | 8 (7–9,5) |
| 54 | 71 (64,5–86,5) | 1,46 (1–2) | 3 (2,5–5) | 3,45 (3,16–4,05) | 8 (7–9,5) |
| 55 | 72 (62–86,5) | 1,48 (1,23–2) | 3 (2–4,72) | 3,45 (3,1–3,99) | 8 (7–9,5) |
| 56 | 74 (61,5–86) | 1,4 (1–1,74) | 3 (2,41–3,97) | 3,45 (3,18–4) | 8 (7–9,5) |
| 57 | 75 (61–86,5) | 1,45 (1,25–1,88) | 3 (2,1–4,85) | 3,47 (3,15–4,01) | 8 (8–9,5) |
| 58 | 75 (61,5–89,5) | 1,33 (1–1,65) | 3,5 (2,1–4,9) | 3,45 (3,11–3,99) | 8,5 (8–9,5) |
| 59 | 79 (61–91) | 1,38 (1–1,83) | 3,42 (2,52–4,9) | 3,41 (3,06–3,97) | 8,5 (8–10) |
| 60 | 77 (61–87,5) | 1,5 (1,07–1,9) | 3,45 (1,67–4,25) | 3,45 (3,01–4,02) | 8,5 (8–10) |

### Han ve ticaret

| Yıl | Ayakta han | Asılan ilan | Biten ilan | Ticaret seferi (kervan) | İkmal seferi |
|---|---|---|---|---|---|
| 1 | 2 (1–3) | 0 (0–1) | 0 | 0 | 0 |
| 2 | 4 | 0 (0–1) | 0 | 0 | 0 |
| 3 | 4 | 0 (0–1) | 0 | 0 | 0 |
| 4 | 4 (3–4) | 1 (0–2) | 0 | 0 (0–0,5) | 0 |
| 5 | 3,5 (3–4) | 2 (1–3,5) | 0 | 0 (0–2,5) | 0 |
| 6 | 3 (2,5–4) | 1 (0,5–2) | 0 | 2 (0–8) | 0 |
| 7 | 3 (2–4) | 2 (0–4,5) | 0 | 6 (3–14,5) | 0 |
| 8 | 3 (2–4) | 2 (1–3,5) | 0 | 13 (6,5–21,5) | 0 |
| 9 | 3 (2–4) | 1,5 (0–3,5) | 0 | 17,5 (13–31) | 0 |
| 10 | 3 (2–4) | 2,5 (1–4) | 0 (0–1) | 22,5 (15–38) | 0 |
| 11 | 3 (2–3) | 2,5 (1–6) | 0 (0–1,5) | 26,5 (14,5–46) | 0 (0–0,5) |
| 12 | 3 (2–4) | 2,5 (0,5–8) | 1 (0–2,5) | 30 (17–53,5) | 0 (0–3,5) |
| 13 | 3 (2–4) | 2,5 (0,5–5,5) | 2 (0–3) | 36 (18,5–57,5) | 2,5 (0–5) |
| 14 | 3 (2,5–4,5) | 3 (0,5–5,5) | 1 (0–1,5) | 40 (21,5–61) | 5 (0–9) |
| 15 | 3,5 (3–5) | 2 (1–4) | 1 (0–1,5) | 40 (25–64,5) | 5 (1,5–11,5) |
| 16 | 4 (3–5) | 2 (1–3) | 0 (0–1) | 42 (28,5–67,5) | 7,5 (3,5–14,5) |
| 17 | 5 (4–5) | 2 (1–3) | 0 (0–1) | 43 (29,5–71) | 10 (6,5–15) |
| 18 | 5 (3,5–5,5) | 2 (0–6) | 0 (0–1) | 46 (30–72,5) | 10,5 (7–18,5) |
| 19 | 5 (3–5,5) | 2 (0,5–3,5) | 0 (0–1) | 49 (33,5–77,5) | 12 (9–18,5) |
| 20 | 4,5 (3,5–5,5) | 3 (1–6) | 0 (0–1) | 52,5 (32,5–81) | 13,5 (10–21,5) |
| 21 | 5 (4–5) | 2 (1–5) | 0 (0–1,5) | 56,5 (36–85,5) | 16 (10,5–19) |
| 22 | 5 (4–5) | 4 (1–6,5) | 0 (0–1) | 59,5 (38–90,5) | 17 (11–20) |
| 23 | 5 (5–5,5) | 3 (1–5) | 0 (0–1) | 65,5 (40,5–95,5) | 16,5 (12–21,5) |
| 24 | 5 (4,5–5) | 2,5 (1–5,5) | 0 (0–1) | 65,5 (41,5–97) | 16 (11–23,5) |
| 25 | 5 (4,5–5) | 3,5 (1–5,5) | 0 (0–3) | 67,5 (39,5–99) | 19,5 (11–23,5) |
| 26 | 5 (4,5–6) | 3 (1–6) | 0 (0–1) | 73 (43–100) | 20 (12–27) |
| 27 | 5 (4,5–6) | 3 (0,5–5) | 0 (0–2,5) | 77,5 (42–104) | 18 (11,5–26,5) |
| 28 | 5 (5–6) | 2 (1–5) | 0 (0–1,5) | 74,5 (42,5–102) | 19 (12,5–26) |
| 29 | 5 (4,5–6) | 4 (1–4) | 1 (0–3) | 80,5 (44–103) | 19,5 (14–28) |
| 30 | 5 (4,5–6) | 4 (1,5–7) | 0,5 (0–1) | 81,5 (46,5–96) | 20 (12,5–30) |
| 31 | 5 (5–6) | 4 (1–6) | 1 (0–3) | 78,5 (48–105) | 20,5 (13–33,5) |
| 32 | 5 (4,5–6) | 3 (2–6) | 0,5 (0–2,5) | 80 (47–100) | 21 (13,5–33) |
| 33 | 5 (5–6) | 2 (1–5,5) | 0,5 (0–3) | 84,5 (48–106) | 20,5 (14–34,5) |
| 34 | 5 (5–6) | 3 (2–5,5) | 1 (0–2,5) | 93 (48–108) | 22,5 (14–35,5) |
| 35 | 5 (5–6) | 2,5 (1,5–6) | 0 (0–3) | 85,5 (46,5–110) | 22 (12,5–36) |
| 36 | 5 (5–6) | 3 (1–6) | 0 (0–2) | 92,5 (46–108) | 23,5 (15–36,5) |
| 37 | 5 (5–6) | 3 (2–6,5) | 1 (0–2,5) | 89,5 (47–111) | 22,5 (13–36,5) |
| 38 | 5 (4–6) | 4,5 (2–6) | 0 (0–2,5) | 99,5 (47–110) | 24 (12,5–34,5) |
| 39 | 5 (4,5–6) | 3 (1–6,5) | 1,5 (0–2,5) | 96 (49–114) | 23 (13–36,5) |
| 40 | 5 (5–6) | 3 (1–5,5) | 1 (0–2) | 91,5 (46,5–114) | 24 (13–38) |
| 41 | 5 (5–6) | 3 (1,5–4) | 0 (0–1,5) | 91 (48,5–112) | 24,5 (13–39,5) |
| 42 | 5,5 (5–6) | 5 (2,5–7,5) | 1 (0–2,5) | 97,5 (47,5–120) | 24 (14,5–43,5) |
| 43 | 5,5 (5–6) | 3 (2–7) | 1 (0–3,5) | 97 (48–127) | 23 (14–47,5) |
| 44 | 5 (5–6,5) | 3 (0,5–6,5) | 1 (0–3,5) | 96 (51,5–122) | 24 (12–47) |
| 45 | 5 (5–6) | 3 (1,5–8,5) | 1 (0–3) | 100 (52–134) | 22,5 (11–45) |
| 46 | 5 (5–6) | 4 (2–7,5) | 1 (0–3,5) | 96,5 (54,5–140) | 23,5 (12–45) |
| 47 | 5 (4–6) | 4,5 (1–6,5) | 1 (0–3,5) | 97,5 (50–142) | 22 (11–43) |
| 48 | 5 (4,5–6) | 3,5 (1–7,5) | 1,5 (0–5,5) | 95 (52,5–156) | 22 (10–41) |
| 49 | 5 (5–6) | 2 (0–7,5) | 1,5 (0–3,5) | 106 (54,5–156) | 25 (12–39,5) |
| 50 | 5 (5–6) | 5 (1–7) | 1 (0–2,5) | 108 (54,5–156) | 28 (11,5–38) |
| 51 | 5 (5–6) | 3 (0,5–4,5) | 1 (0–3) | 102 (52,5–162) | 28,5 (12,5–42,5) |
| 52 | 5 (5–6) | 3 (1,5–9) | 1 (0–2,5) | 106 (49–164) | 28,5 (13–43,5) |
| 53 | 5,5 (5–6,5) | 3 (1,5–5,5) | 1 (0–2,5) | 105 (54,5–168) | 31,5 (12–45,5) |
| 54 | 5,5 (5–6) | 3,5 (0,5–6) | 1 (0–4) | 106 (50–178) | 32 (10–45) |
| 55 | 5 (5–6) | 3,5 (1–6) | 1,5 (0–3) | 107 (54–178) | 30 (10–48) |
| 56 | 5 (5–6,5) | 3 (1–7,5) | 2 (0–3) | 110 (49,5–176) | 29 (10,5–48,5) |
| 57 | 5 (5–6) | 2 (0,5–5,5) | 1 (0–2) | 102 (55–178) | 31,5 (10–50) |
| 58 | 5 (5–6) | 2 (0–6) | 1 (0–2,5) | 112 (49–182) | 31,5 (11,5–47,5) |
| 59 | 5 (4–6) | 4 (2–10) | 1 (0–3) | 110 (54,5–188) | 34 (13–47) |
| 60 | 5 (4–6) | 3 (0,5–7) | 1 (0–3) | 106 (51,5–196) | 33,5 (12,5–46) |

