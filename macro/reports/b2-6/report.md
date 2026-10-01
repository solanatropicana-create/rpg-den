# Ölçüm raporu: b2-6

16 dünya (seed 1-16) × 60 yıl (7200 gün) · 2026-10-01 10:44 · `FD.Macro.Run stats --seeds 1-16 --years 60 --jobs 2 --verify 1 --saveload 1`

Süre: 7 dk 32 sn duvar saati, 2 iş parçacığı; dünya başına 50,4 sn (en az 39,1, en çok 63,3; yıl sonu hash'leri dâhil).

## Bitiş ölçütleri

DESIGN-FAZ1.md, "Bitiş ölçütleri". ✓ geçti · ✗ kaldı · — ölçülemedi.

| # | Ölçüt | Koşul | Ölçülen | Sonuç |
|---|---|---|---|---|
| 1 | Donma yok | 41–60. yılların yıllık büyük olay medyanı ≥ 0,8 × 6–20. yılların medyanı | 54 / 58 = 0,93 kat | ✓ |
| 2 | Çöküş | dünyaların ≥ %75'inde 60 yılda ≥ 1 çöküş (yok olma ya da başkent kaybı) | %100 (16/16 dünya); toplam 153 çöküş: 23 yok olma, 130 başkent kaybı | ✓ |
| 3 | Kamplar | 41–60. yıllarda yaşayan kamp medyanı ≥ 6–20. yılların medyanı | 10,3 ≥ 8,3 (yıl sonu sayımıyla 10 / 8) | ✓ |
| 4a | Kahraman: doğuş seviyesi | her on yılda doğanların ortalama seviyesi ≤ 2 | 1,38 · 1,33 · 1,35 · 1,36 · 1,42 · 1,44 (on yıllar sırasıyla) | ✓ |
| 4b | Kahraman: Sv8+ | dünyaların ≥ yarısında en az bir kahraman Sv8 ve üstüne çıkar | %100 (16/16 dünya); dünyadaki en yüksek seviye: medyan Sv10, en çok Sv10 | ✓ |
| 4c | Kahraman: efsane | dünya başına efsane medyanı 1–6 | medyan 13,5 (p10–p90: 8–21; toplam 227) | ✗ |
| 4d | Kahraman: ölüm payı | doğan kahramanların %30–80'i ölür | %43 (1358/3164); dünya medyanı %43 (%33–%51) | ✓ |
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
| Araştırma ağacı erken bitiyor | ~19. yılda bitiyor; 30. yılda medeniyetlerin %98'i bitirmiş | başlangıç medeniyetlerinde ağaç bitişi (son çağ, araştıracak düğüm yok) medyanı 20. yıl (122/128 bitirdi); 30. yılda bitirmiş medeniyet payı %95, araştırması duran (her çağ) %96; ağacın biten payı 19. yılda %84, 30. yılda %96 | evet |
| Medeniyetler 5 yerleşimde takılıyor | 98 medeniyetin 74'ü (%76) tam 5 yerleşimde | tam 5 kara yerleşimli medeniyet payı 30. yılda %3,1, 60. yılda %5,4 (denizaşırı koloniler dâhil 30. yılda tam 5: %1,6, 5+: %94); medeniyet başına 8,67 yerleşim (30. yıl) | hayır |
| Kamp sayısı düşüyor | 6,8'den 3,5'e iniyor | 3 (1. yıl) → en yüksek 11,5 (49. yıl) → 9 (30. yıl) → 11 (60. yıl); yıl sonu, yıllık dünya medyanı | hayır |
| Altın birikiyor | altın medyanı 78'den 6.503'e çıkıyor | 8,42 (1. yıl) → 4795 (30. yıl) → 7157 (60. yıl) | evet |
| İş gücü boşta | iş gücünün %43'ü boşta | 30. yılda %49, 60. yılda %49 (işe yerleşemeyen `zanaatçı` / bütün iş gücü, askerler dâhil; yıl içi ortalama) | evet |
| Büyük olaylar seyreliyor | yıllık büyük olay 59'dan 28'e düşüyor | en yüksek 76 (12. yıl) → 46,5 (30. yıl) → 61,5 (60. yıl), yıllık dünya medyanı | hayır |
| Doğuş seviyesi şişiyor | 24. yıldan sonra herkes Sv5 doğuyor; efsane mekaniği ölü | 25–60. yıllarda Sv5+ doğanların payı %0; on yıllık doğuş seviyesi ortalaması 1,38 · 1,33 · 1,35 · 1,36 · 1,42 · 1,44 | hayır |
| Başkent düşmüyor | başkent fethedilemiyor (agents.ts:673) | 16 dünyada 130 başkent kaybı, 23 yok olma; 938 yerleşim fethi | hayır |

## On yıllık özet

Hücre: dünyalar arası medyan (p10–p90). Her dünyada on yılın yıllık değerlerinin ortalaması alınır: akış ölçülerinde yıllık ortalama, stok ölçülerinde yıl sonu değerlerinin ortalaması. Yüzdeler 0–1 paylardır.

| Ölçü | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| **Medeniyet** | | | | | | |
| Yaşayan medeniyet | 8 (7–9) | 8 (7–9) | 8 (7–8,9) | 8 (7,05–9) | 8 (7–9) | 8,1 (7–9,5) |
| Yeni medeniyet (yeniden doğan) | 0 | 0 | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) |
| Yok olan medeniyet | 0 | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) |
| Başkent kaybı (medeniyet yaşarken) | 0 (0–0,05) | 0 (0–0,3) | 0,15 (0–0,45) | 0,1 (0–0,35) | 0,1 (0–0,35) | 0,2 (0–0,45) |
| Çöküş (yok olma + başkent kaybı) | 0 (0–0,1) | 0,05 (0–0,3) | 0,15 (0–0,45) | 0,2 (0–0,45) | 0,1 (0–0,35) | 0,2 (0–0,5) |
| Yaşayan yerleşim | 15,2 (12,8–17,3) | 48,4 (40,6–52,9) | 64,9 (57,4–71,3) | 76,5 (64,5–81,9) | 84,9 (68,5–92,8) | 88,9 (69,1–98,7) |
| Medeniyet başına yerleşim | 1,94 (1,69–2,04) | 6 (5,57–6,37) | 8,04 (7,62–8,6) | 9,18 (8,47–10,2) | 9,91 (8,96–11,1) | 10,4 (9,21–11,9) |
| 5+ kara yerleşimli medeniyet payı | %5,6 (%2,9–%8,3) | %85 (%69–%92) | %92 (%88–%100) | %97 (%84–%100) | %92 (%82–%100) | %91 (%84–%98) |
| Kurulan yerleşim | 2,3 (1,95–2,65) | 2,7 (2,2–3,1) | 1,2 (1–1,8) | 0,8 (0,5–1,4) | 0,75 (0,35–1,15) | 0,55 (0,3–0,9) |
| Fethedilen yerleşim | 0 (0–0,2) | 0,85 (0,3–1,2) | 1,1 (0,55–1,5) | 1,4 (0,85–1,7) | 1,35 (0,85–2) | 1,35 (0,8–1,85) |
| Terk edilen yerleşim | 0 | 0 (0–0,1) | 0 (0–0,15) | 0 (0–0,2) | 0,1 (0,05–0,4) | 0,15 (0–0,45) |
| Toplam nüfus | 277 (215–312) | 1429 (1089–1525) | 2438 (2037–2736) | 3066 (2678–3522) | 3482 (3034–4012) | 3744 (3335–4374) |
| Ortalama çağ | 1,9 (1,83–1,94) | 3,45 (3,19–3,54) | 3,94 (3,78–4) | 4 (3,96–4) | 4 | 4 |
| Araştırma ağacının biten payı (ort.) | %19 (%18–%20) | %71 (%65–%74) | %93 (%91–%94) | %96 (%95–%99) | %98 (%97–%100) | %99 (%97–%100) |
| Ağacı bitmiş medeniyet payı | %0 | %19 (%12–%27) | %81 (%70–%88) | %98 (%90–%100) | %100 (%95–%100) | %100 (%95–%100) |
| Araştırması duran medeniyet payı | %0 (%0–%2,4) | %45 (%36–%52) | %87 (%82–%94) | %98 (%91–%100) | %100 (%95–%100) | %100 (%95–%100) |
| Altın medyanı (medeniyetler) | 52,1 (42,6–78) | 897 (672–1106) | 3369 (2403–5067) | 4551 (3262–9107) | 5921 (3816–12354) | 7355 (4483–17367) |
| Boştaki iş gücü payı | %0,7 (%0,3–%0,9) | %17 (%14–%21) | %45 (%42–%48) | %49 (%46–%51) | %48 (%43–%52) | %48 (%43–%51) |
| Bölünme (ayrılıp kurulan medeniyet) | 0 | 0 | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) |
| En büyük medeniyetin yerleşimi | 2,65 (2,4–2,8) | 8,15 (7,15–8,6) | 12,1 (9,7–13,5) | 15,8 (12,2–18,7) | 18,8 (12,9–22,6) | 20,9 (13,9–25,4) |
| **Olaylar** | | | | | | |
| Olay | 82,6 (65,6–99,7) | 196 (159–220) | 170 (139–187) | 157 (124–204) | 169 (133–225) | 184 (134–253) |
| Büyük olay | 30,4 (25,8–37,5) | 69,7 (57,1–76,9) | 53,3 (45,7–60,7) | 48,2 (41,1–60,4) | 50,6 (40,4–73,3) | 55,5 (39,3–84,8) |
| **Savaş** | | | | | | |
| Muharebe | 4,3 (3,25–4,75) | 8 (6,1–9,25) | 8,25 (6,35–9,3) | 9,15 (8,15–11,3) | 9,9 (6,95–14,1) | 10,6 (7,25–14,4) |
| Başlayan savaş | 0 (0–0,2) | 0,95 (0,3–1,45) | 1,25 (0,8–1,8) | 1,55 (1–2,6) | 1,6 (1–2,6) | 1,7 (1–2,6) |
| Süren savaş (yıl sonu) | 0 (0–0,1) | 0,2 (0,1–0,3) | 0,4 (0,1–0,95) | 0,5 (0,15–0,8) | 0,45 (0,1–0,9) | 0,65 (0,2–0,8) |
| Yıl içinde süren savaş | 0 (0–0,25) | 1,1 (0,45–1,8) | 1,75 (0,95–2,65) | 1,85 (1,25–3,3) | 2,2 (1,25–3,3) | 2,15 (1,3–3,25) |
| Yağma akını (medeniyet) | 0 (0–0,25) | 0,9 (0,1–1,5) | 1,3 (0,4–2,35) | 1,7 (0,45–3,2) | 2,2 (0,2–4,9) | 2,05 (0,2–5,15) |
| Tarihî hak savaşı | 0 | 0,1 (0–0,2) | 0,1 (0–0,2) | 0,1 (0,1–0,55) | 0,15 (0–0,4) | 0,1 (0–0,5) |
| Pakt gereği savaş | 0 | 0,05 (0–0,25) | 0,15 (0–0,4) | 0,2 (0–0,6) | 0,2 (0–0,5) | 0,1 (0–0,5) |
| Kutsal Sefer çağrısı | 0 | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,2) |
| İhanet (pakt çiğnendi) | 0 | 0 | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,15) | 0 (0–0,15) |
| Savunma paktı (yıl sonu) | 0 | 0,25 (0–1,05) | 0,6 (0,15–1,95) | 1 (0–1,95) | 0,95 (0–2,05) | 0,8 (0–2,15) |
| **Canavarlar** | | | | | | |
| Yaşayan kamp (yıl sonu) | 7,45 (6,85–8,4) | 7 (6,2–9,15) | 7,9 (7,1–8,55) | 9,3 (7,95–10,1) | 10,4 (8–11,7) | 10,5 (8,35–12,5) |
| Yaşayan kamp (yıl ort.) | 7,12 (6,5–8,05) | 6,99 (6,35–9,35) | 7,93 (7,2–8,6) | 9,48 (7,88–10) | 10,4 (7,82–11,6) | 10,4 (8,32–12,4) |
| Doğan kamp | 0,75 (0,7–0,85) | 1,65 (0,95–2,1) | 1,9 (1,55–2,4) | 1,85 (1,05–2,45) | 2,05 (1,05–2,7) | 1,95 (1,4–2,8) |
| Temizlenen kamp | 0 (0–0,15) | 2 (1,45–2,4) | 1,65 (1,3–2,25) | 1,9 (1–2,35) | 1,85 (0,85–2,7) | 1,9 (1,35–3,05) |
| Canavar baskını | 3,65 (2,95–4,3) | 3,85 (3,1–4,9) | 3,3 (2,25–3,85) | 3,6 (2,75–4,85) | 3,6 (2,6–5,25) | 3,8 (2,5–4,8) |
| Yaşayan trol ini (yıl sonu) | 0 | 0,05 (0–0,45) | 0,45 (0,2–1,55) | 1 (0,2–2,3) | 2,2 (0,7–2,7) | 2,4 (1–2,75) |
| Yaşayan ejderha (yıl sonu) | 0 | 0,05 (0–0,3) | 1 (0,85–1) | 1 (0,05–1) | 1 (0–1) | 0,75 (0–1) |
| Ejderha akını | 0 | 0,05 (0–0,35) | 0,9 (0,4–1,1) | 0,8 (0–1,1) | 0,5 (0–0,95) | 0,55 (0–0,95) |
| Kriz (anlatıcı) | 0 (0–0,05) | 0,3 (0,15–0,4) | 0,3 (0,25–0,4) | 0,4 (0–0,55) | 0,25 (0,05–0,55) | 0,25 (0–0,55) |
| Rahatlama dönemi (anlatıcı) | 0,2 (0,1–0,3) | 0,1 (0–0,2) | 0,1 (0–0,25) | 0,1 (0–0,25) | 0,1 (0–0,2) | 0,1 (0–0,2) |
| **Kahramanlar** | | | | | | |
| Doğan kahraman | 1,35 (0,95–2,4) | 2,9 (2,1–3,6) | 4,15 (2,4–4,7) | 4,3 (2,05–4,8) | 3,45 (2,4–4,8) | 4,05 (2,65–5,15) |
| Ölen kahraman | 0,15 (0–0,6) | 1 (0,4–1,45) | 1,4 (0,7–2,05) | 1,6 (0,65–2,55) | 1,5 (0,9–2,8) | 2,55 (1,5–3,25) |
| Emekli olan kahraman | 0 | 0 | 0 (0–0,1) | 0,25 (0,05–0,5) | 0,8 (0,45–1,35) | 0,9 (0,55–1,3) |
| Diyarı terk eden kahraman | 0,05 (0–0,1) | 0 (0–0,1) | 0,1 (0–0,3) | 0,5 (0,2–0,8) | 0,45 (0,2–1,4) | 0,8 (0,35–1,35) |
| Ölümden dönen kahraman | 0 | 0 | 0 | 0 | 0 | 0 |
| Efsane olan kahraman | 0 | 0,05 (0–0,2) | 0,2 (0–0,3) | 0,2 (0,05–0,65) | 0,35 (0,1–0,75) | 0,4 (0,2–0,65) |
| Yaşayan kahraman (yıl sonu) | 4,4 (2,7–7,25) | 23,2 (14,8–30,2) | 46,9 (33,9–52,8) | 63,2 (53,7–72,7) | 74,4 (59,4–81,8) | 71,7 (60,3–87,7) |
| Doğuş seviyesi (ort.) | 1,38 (1,22–1,51) | 1,31 (1,18–1,39) | 1,32 (1,22–1,42) | 1,37 (1,24–1,47) | 1,39 (1,32–1,52) | 1,45 (1,28–1,58) |
| Ölüm seviyesi (ort.) | 1,33 (1–1,92) | 1,47 (1,21–1,88) | 2,01 (1,62–2,82) | 2,52 (1,76–3,07) | 2,72 (2,23–3,48) | 3,01 (2,5–3,78) |
| Yaşayan kahraman seviyesi (ort.) | 1,48 (1,14–1,72) | 2,04 (1,74–2,42) | 2,63 (2,38–2,87) | 2,91 (2,66–3,42) | 3,34 (2,69–3,88) | 3,48 (3–3,85) |
| En yüksek seviye (şimdiye dek) | 1,8 (1,15–2,05) | 3,9 (3,25–4,8) | 5,8 (4,6–6,6) | 7,15 (6–8,65) | 8,8 (7,35–9,8) | 9,6 (8,2–10) |
| **Han ve ticaret** | | | | | | |
| Ayakta han | 3,05 (2,85–3,75) | 4 (3,15–4,45) | 5,25 (4,8–5,85) | 5,15 (4,95–5,9) | 5,3 (5–6) | 5,5 (5,2–6,15) |
| Asılan ilan | 1,5 (1–1,9) | 2,75 (2,3–3,25) | 3,15 (2,55–4) | 3 (1,9–4,25) | 3,45 (2,2–4,5) | 3,15 (1,85–5,65) |
| Biten ilan | 0 (0–0,1) | 0,75 (0,35–1) | 0,6 (0,25–1) | 0,9 (0,2–1,2) | 1,35 (0,35–1,75) | 1,2 (0,6–2,25) |
| Ticaret seferi (kervan) | 5,8 (4,5–11,3) | 40,8 (26–65,9) | 72,7 (41,3–91,8) | 84,4 (52,1–118) | 88,1 (60,9–134) | 101 (64,8–162) |
| İkmal seferi | 0 | 6,7 (3,9–10,9) | 19,5 (14,2–24,8) | 24,3 (17,6–37,1) | 31 (20,4–43,4) | 33,4 (20,8–50,9) |

## Kahraman seviyeleri

### Doğuş seviyesi (bütün dünyalar, on yıl içinde doğanlar)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 |
|---|---|---|---|---|---|
| 1–10 | 251 | 1,38 | %64 | %35 | %1,6 |
| 11–20 | 463 | 1,33 | %67 | %32 | %0,4 |
| 21–30 | 604 | 1,35 | %67 | %32 | %1,3 |
| 31–40 | 607 | 1,36 | %65 | %33 | %1,5 |
| 41–50 | 573 | 1,42 | %62 | %35 | %3,5 |
| 51–60 | 666 | 1,44 | %61 | %35 | %4,4 |

### Ölüm seviyesi (bütün dünyalar, on yıl içinde ölenler)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 | Sv4 | Sv5 | Sv6 | Sv7 | Sv8 | Sv9 | Sv10 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 1–10 | 40 | 1,38 | %62 | %38 | %0 | %0 | %0 | %0 | %0 | %0 | %0 | %0 |
| 11–20 | 161 | 1,52 | %57 | %36 | %6,8 | %0,6 | %0 | %0 | %0 | %0 | %0 | %0 |
| 21–30 | 220 | 2,23 | %34 | %31 | %22 | %8,2 | %3,6 | %1,4 | %0,5 | %0 | %0 | %0 |
| 31–40 | 251 | 2,61 | %24 | %26 | %29 | %12 | %5,2 | %2 | %1,6 | %0 | %0 | %0 |
| 41–50 | 282 | 2,94 | %13 | %32 | %27 | %16 | %7,4 | %2,1 | %1,8 | %0,4 | %1,1 | %0 |
| 51–60 | 404 | 3,23 | %13 | %23 | %30 | %14 | %9,7 | %6,4 | %1,7 | %1,7 | %0,5 | %0,2 |

### Yaşayan kahramanların seviyesi (bütün dünyalar, on yılın son yılının sonunda)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 | Sv4 | Sv5 | Sv6 | Sv7 | Sv8 | Sv9 | Sv10 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 10 | 202 | 1,5 | %55 | %39 | %5,4 | %0 | %0 | %0 | %0 | %0 | %0 | %0 |
| 20 | 501 | 2,46 | %24 | %32 | %27 | %12 | %5,6 | %0,6 | %0 | %0 | %0 | %0 |
| 30 | 863 | 2,82 | %19 | %30 | %24 | %15 | %8 | %3,6 | %1,5 | %0,3 | %0 | %0 |
| 40 | 1095 | 3,11 | %18 | %29 | %19 | %12 | %11 | %6,1 | %2,8 | %1,1 | %0,5 | %0,1 |
| 50 | 1152 | 3,35 | %14 | %28 | %23 | %11 | %9,3 | %6,1 | %5,5 | %1,7 | %0,6 | %1,2 |
| 60 | 1189 | 3,48 | %13 | %27 | %23 | %12 | %8,2 | %6,6 | %3,8 | %2,4 | %2,3 | %1,7 |

### Ölüm nedenleri (bütün dünyalar)

| Neden | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 | Toplam |
|---|---|---|---|---|---|---|---|
| Kuşatma | 3 | 42 | 99 | 101 | 112 | 124 | 481 (%35) |
| Kamp saldırısı | 20 | 97 | 48 | 62 | 66 | 108 | 401 (%30) |
| Ejderha | 0 | 1 | 26 | 36 | 29 | 34 | 126 (%9,3) |
| Yağma akını | 0 | 3 | 15 | 13 | 15 | 27 | 73 (%5,4) |
| Trol | 0 | 2 | 15 | 6 | 15 | 20 | 58 (%4,3) |
| Suikast | 0 | 3 | 10 | 13 | 19 | 10 | 55 (%4,1) |
| Bilinmiyor | 0 | 0 | 0 | 2 | 10 | 43 | 55 (%4,1) |
| Han baskını (canavar) | 14 | 4 | 3 | 4 | 5 | 10 | 40 (%2,9) |
| Yol pususu | 0 | 2 | 3 | 7 | 4 | 13 | 29 (%2,1) |
| Kervan soygunu | 0 | 2 | 0 | 2 | 5 | 7 | 16 (%1,2) |
| Yerleşim baskını (canavar) | 3 | 3 | 1 | 1 | 0 | 1 | 9 (%0,7) |
| Han baskını (medeniyet) | 0 | 1 | 0 | 4 | 1 | 2 | 8 (%0,6) |
| Düello | 0 | 1 | 0 | 0 | 1 | 5 | 7 (%0,5) |

Neden, ölümün kaydedildiği andaki son muharebenin türünden (başlık ve taraflar) ya da suikast olayından çıkarılır.

## Olay türleri

Dünya başına yıllık olay sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Kahraman (`hero`) | 3,15 (2,35–5,75) | 23,7 (12,3–27,8) | 35,2 (19,4–45,1) | 48,8 (35,6–66,6) | 63,1 (49,6–82) | 79 (53,4–95,9) |
| İnşaat (`build`) | 25,9 (18,9–31,3) | 85,4 (68,5–101) | 54,6 (39,2–65,3) | 27,8 (18,6–41,9) | 22,7 (13,5–32,8) | 16,8 (10,2–32,2) |
| Göç (`migration`) | 1,6 (0,7–2,55) | 9,1 (6,75–16,9) | 17,8 (11,7–26) | 19,9 (13,5–28,9) | 23,1 (13,6–30,4) | 22 (15,2–30) |
| Sefer/ilan (`quest`) | 1,55 (1,1–2,75) | 8,35 (5,35–10,7) | 9,25 (5,65–11,3) | 10,1 (6,45–15,9) | 17,2 (7,85–22,1) | 17,3 (8,5–24) |
| Savaş (`war`) | 0,45 (0–1,25) | 6,4 (2,1–9,5) | 9,7 (5,55–15) | 12,9 (7,25–20,9) | 13,6 (7,2–28,9) | 13,8 (6,65–30,1) |
| Araştırma (`research`) | 16,7 (13,6–18,5) | 16 (14,3–19,3) | 3,15 (2,3–4,65) | 0,55 (0–1,7) | 0,25 (0–0,85) | 0 (0–0,75) |
| Sınıf (`class`) | 4,6 (3,5–5,95) | 6,4 (5,05–7,25) | 5,3 (4,7–6,7) | 4,2 (3,8–5,15) | 4,1 (3,55–5,05) | 4,1 (3,4–5,05) |
| Kamp (`lair`) | 2 (1,9–2,4) | 4,7 (3,35–5,5) | 4,85 (3,85–6,1) | 4,95 (3,15–6,45) | 5,65 (2,5–7,45) | 6,5 (4,2–8,35) |
| Deniz (`sea`) | 0,4 (0–1,45) | 6,15 (3,4–8,65) | 5,45 (2,05–8,45) | 4,65 (2,75–8,65) | 5,3 (2,6–8,8) | 4,6 (1,65–8,75) |
| Baskın (`raid`) | 3,75 (2,95–4,35) | 3,85 (2,95–4,75) | 2,5 (1,7–3,2) | 3,1 (2,5–4) | 3,1 (2,2–4,7) | 3 (2,45–4,75) |
| Han (`inn`) | 2,8 (2,2–3,2) | 3,15 (2,65–4,8) | 3,75 (2,85–4,25) | 2,7 (1,75–3,05) | 2,7 (1,55–3,5) | 2,7 (2,15–4,75) |
| Ekonomi (`economy`) | 0,75 (0,55–0,95) | 1,8 (1,25–3,45) | 4,05 (2,65–4,5) | 3,2 (2,2–4,7) | 3,1 (2,25–5,5) | 2,8 (1,9–3,65) |
| Yerleşim (`settle`) | 4,8 (4,15–5,35) | 4,85 (3,9–5,65) | 2,3 (1,8–3,05) | 1,4 (0,75–2,55) | 1,2 (0,6–2,2) | 1 (0,55–1,75) |
| Keşif (`discover`) | 6,45 (4,9–8) | 2,65 (1,9–3,75) | 1,7 (0,9–2,7) | 1,3 (0,4–3,05) | 1,1 (0,65–1,6) | 0,9 (0,15–1,6) |
| Ölüm/terk (`death`) | 0,2 (0,05–0,6) | 0,95 (0,4–1,5) | 1,5 (0,75–2) | 1,55 (0,65–2,4) | 1,6 (1–3) | 2,75 (1,7–3,95) |
| epitaph | 0,15 (0–0,6) | 1 (0,4–1,45) | 1,4 (0,7–2,05) | 1,6 (0,65–2,55) | 1,5 (0,9–2,8) | 2,55 (1,5–3,25) |
| Ticaret (`trade`) | 1,3 (0,9–2,2) | 2,1 (1,1–3,65) | 1,7 (0,45–2,8) | 1,2 (0,4–2,1) | 0,95 (0,05–2,15) | 0,8 (0,3–2,05) |
| Ejderha (`dragon`) | 0 | 0,15 (0–0,75) | 1,85 (1,35–2,1) | 1,8 (0,2–2,1) | 1,5 (0–2) | 1,55 (0–1,95) |
| Dünya (`world`) | 0,15 (0–0,3) | 0,6 (0,35–1,25) | 0,9 (0,3–1,6) | 1,1 (0,5–2,1) | 0,95 (0,65–1,7) | 1,15 (0,7–1,9) |
| Büyüme (`growth`) | 1,7 (1,35–1,95) | 1,3 (1,05–1,45) | 0,2 (0,1–0,3) | 0,05 (0–0,2) | 0 (0–0,2) | 0 (0–0,15) |
| Diplomasi (`diplomacy`) | 0,6 (0,3–1,05) | 0,5 (0,1–1,25) | 0,55 (0,1–0,9) | 0,35 (0,05–0,65) | 0,3 (0,05–0,55) | 0,3 (0,05–0,75) |
| Gerginlik (`tension`) | 0,7 (0,3–1,3) | 0,55 (0,25–1,25) | 0,4 (0,1–0,85) | 0,25 (0,05–0,7) | 0,35 (0,05–0,45) | 0,2 (0–0,75) |
| Çağ (`era`) | 1,45 (1,25–1,6) | 0,75 (0,65–1) | 0,2 (0–0,3) | 0 | 0 | 0 |
| Temas (`contact`) | 1 (0,6–1,4) | 0,55 (0,3–1) | 0,15 (0,05–0,45) | 0,15 (0–0,25) | 0,1 (0–0,3) | 0 (0–0,1) |
| Kriz (anlatıcı) (`crisis`) | 0 (0–0,05) | 0,3 (0,15–0,4) | 0,3 (0,25–0,4) | 0,4 (0,05–0,55) | 0,25 (0,05–0,55) | 0,25 (0–0,55) |
| Harika (`wonder`) | 0 | 0,2 (0,1–0,45) | 0,4 (0,2–0,5) | 0,1 (0–0,2) | 0 (0–0,1) | 0 (0–0,05) |
| Rahatlama (anlatıcı) (`relief`) | 0,2 (0,1–0,3) | 0,1 (0–0,2) | 0,1 (0–0,2) | 0,1 (0–0,25) | 0,1 (0–0,15) | 0,1 (0–0,2) |

## Büyük olay türleri

Dünya başına yıllık büyük olay sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Sefer/ilan (`quest`) | 1,55 (1,1–2,45) | 6,5 (4,55–7,55) | 6,6 (4,85–8,95) | 6,9 (4,9–11,2) | 11,5 (5,5–14,4) | 11,7 (5,55–16,9) |
| Kahraman (`hero`) | 0,6 (0,25–1) | 4 (2,9–5) | 5,5 (4,05–7,2) | 7,8 (5,45–10,1) | 8,75 (6,65–13,5) | 12,6 (7,6–15,3) |
| Savaş (`war`) | 0,35 (0–0,7) | 4,85 (1,6–7,25) | 6,85 (4,2–10,2) | 8,95 (5,4–13) | 8,25 (4,65–17,3) | 8,15 (4,2–17,3) |
| Araştırma (`research`) | 6,45 (4,95–7,85) | 14,4 (12,6–17,6) | 3,05 (2,25–4,4) | 0,45 (0–1,45) | 0,15 (0–0,7) | 0 (0–0,55) |
| Kamp (`lair`) | 2 (1,9–2,4) | 4,05 (3,25–5,05) | 3,9 (3,4–4,95) | 3,75 (2,4–5,1) | 4,5 (2–5,85) | 4,95 (3,25–6,55) |
| İnşaat (`build`) | 2,2 (1,55–2,6) | 9,25 (7,35–16,3) | 5,35 (3,4–9) | 1,85 (1,05–3,45) | 1,15 (0,4–2,3) | 1 (0,5–1,95) |
| Baskın (`raid`) | 3,35 (2,35–4) | 2,95 (2–3,7) | 1,55 (1,1–2,25) | 2 (1,35–2,65) | 2,25 (1,45–3,2) | 2,55 (1,7–3,15) |
| Deniz (`sea`) | 0,3 (0–0,9) | 2,65 (1,4–3,8) | 2,95 (0,85–4,3) | 2,95 (1,65–5,15) | 3,15 (1,55–5,9) | 2,6 (1–6,9) |
| Han (`inn`) | 1,45 (1,2–1,7) | 2 (1,65–3,15) | 2,05 (1,6–2,6) | 1,7 (1,1–2) | 1,9 (1,2–2,5) | 2,15 (1,6–3,25) |
| Sınıf (`class`) | 1,85 (1,3–2,9) | 2,6 (1,55–3,4) | 2,3 (1,55–3,05) | 1,35 (0,7–2,15) | 1,25 (0,65–2,1) | 1,1 (0,55–2,15) |
| Ölüm/terk (`death`) | 0,2 (0,05–0,6) | 0,95 (0,4–1,5) | 1,5 (0,75–2) | 1,55 (0,65–2,4) | 1,6 (1–3) | 2,75 (1,7–3,95) |
| Yerleşim (`settle`) | 2,3 (1,9–2,65) | 2,7 (2,2–3,1) | 1,2 (1–1,8) | 0,8 (0,4–1,4) | 0,65 (0,35–1,15) | 0,55 (0,3–0,9) |
| epitaph | 0,15 (0–0,6) | 1 (0,4–1,45) | 1,4 (0,7–2,05) | 1,6 (0,65–2,55) | 1,5 (0,9–2,8) | 2,55 (1,5–3,25) |
| Ejderha (`dragon`) | 0 | 0,15 (0–0,75) | 1,85 (1,35–2,1) | 1,8 (0,2–2,1) | 1,5 (0–2) | 1,55 (0–1,95) |
| Keşif (`discover`) | 2,2 (1,8–3,2) | 1,35 (0,8–1,8) | 0,8 (0,45–1,45) | 0,6 (0,25–1,25) | 0,4 (0,15–0,85) | 0,3 (0,05–0,95) |
| Ekonomi (`economy`) | 0 | 1,1 (0,6–1,85) | 1,95 (1,3–2,45) | 1 (0,55–1,3) | 0,35 (0,2–0,8) | 0,2 (0,05–0,5) |
| Ticaret (`trade`) | 0,7 (0,45–1,1) | 0,8 (0,5–1,65) | 0,75 (0,25–1,4) | 0,6 (0,2–1) | 0,5 (0,05–1,05) | 0,35 (0,15–1,05) |
| Dünya (`world`) | 0,15 (0–0,25) | 0,4 (0,25–0,8) | 0,7 (0,2–1,1) | 0,75 (0,4–1,2) | 0,7 (0,5–1,25) | 0,65 (0,55–1,2) |
| Gerginlik (`tension`) | 0,7 (0,3–1,3) | 0,55 (0,25–1,25) | 0,4 (0,1–0,85) | 0,25 (0,05–0,7) | 0,35 (0,05–0,45) | 0,2 (0–0,75) |
| Çağ (`era`) | 1,45 (1,25–1,6) | 0,75 (0,65–1) | 0,2 (0–0,3) | 0 | 0 | 0 |
| Büyüme (`growth`) | 0,9 (0,7–1,1) | 1,2 (1,05–1,45) | 0,15 (0,1–0,3) | 0 (0–0,2) | 0 (0–0,15) | 0 (0–0,15) |
| Temas (`contact`) | 1 (0,6–1,4) | 0,55 (0,3–1) | 0,15 (0,05–0,45) | 0,15 (0–0,25) | 0,1 (0–0,3) | 0 (0–0,1) |
| Kriz (anlatıcı) (`crisis`) | 0 (0–0,05) | 0,3 (0,15–0,4) | 0,3 (0,25–0,4) | 0,4 (0,05–0,55) | 0,25 (0,05–0,55) | 0,25 (0–0,55) |
| Diplomasi (`diplomacy`) | 0,2 (0,05–0,4) | 0,35 (0,05–0,75) | 0,3 (0,1–0,65) | 0,2 (0–0,3) | 0,2 (0,05–0,5) | 0,2 (0–0,45) |
| Göç (`migration`) | 0,2 (0,05–0,35) | 0,2 (0,05–0,35) | 0,2 (0,05–0,4) | 0,15 (0,1–0,4) | 0,15 (0,05–0,35) | 0,15 (0,05–0,3) |
| Harika (`wonder`) | 0 | 0,2 (0,1–0,45) | 0,4 (0,2–0,5) | 0,1 (0–0,2) | 0 (0–0,1) | 0 (0–0,05) |
| Rahatlama (anlatıcı) (`relief`) | 0,2 (0,1–0,3) | 0,1 (0–0,2) | 0,1 (0–0,2) | 0,1 (0–0,25) | 0,1 (0–0,15) | 0,1 (0–0,2) |

## Muharebe türleri

Dünya başına yıllık muharebe sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Yapı baskını (canavar) (`extRaid`) | 2,1 (1,45–2,8) | 2,1 (1,3–2,85) | 1,2 (0,75–1,65) | 1,45 (0,95–1,85) | 1,45 (1–2,2) | 1,6 (1,05–2,15) |
| Kamp saldırısı (`camp`) | 0 (0–0,25) | 2,2 (1,7–2,55) | 1,7 (1,35–2,2) | 1,7 (0,85–2,5) | 1,7 (0,8–2,5) | 1,9 (1,4–2,75) |
| Yağma akını (`plunder`) | 0 (0–0,25) | 0,9 (0,1–1,5) | 1,3 (0,4–2,35) | 1,7 (0,45–3,2) | 2,2 (0,2–4,9) | 2,05 (0,2–5,15) |
| Yerleşim baskını (canavar) (`raid`) | 1,7 (0,85–2,05) | 1,5 (1,15–1,9) | 0,7 (0,4–0,95) | 0,8 (0,2–1,7) | 0,8 (0,35–1,4) | 0,75 (0,5–1,25) |
| Kuşatma (`siege`) | 0 (0–0,2) | 0,9 (0,3–1,45) | 1,2 (0,6–1,65) | 1,4 (0,8–1,9) | 1,4 (0,85–2,25) | 1,3 (0,85–2) |
| Trol (`troll`) | 0 | 0 (0–0,3) | 0,5 (0,25–1,3) | 0,75 (0,05–2,1) | 1,15 (0,65–1,65) | 1,05 (0,25–1,85) |
| Ejderha (`dragon`) | 0 | 0 (0–0,35) | 0,85 (0,5–1,1) | 0,8 (0,05–1,05) | 0,5 (0–0,95) | 0,45 (0–0,95) |
| Han baskını (canavar) (`innMonster`) | 0,2 (0,1–0,45) | 0,1 (0–0,2) | 0 (0–0,1) | 0 (0–0,15) | 0 (0–0,1) | 0,1 (0–0,2) |
| Deniz savaşı (`naval`) | 0 | 0 | 0 (0–0,35) | 0,1 (0–0,6) | 0,1 (0–0,5) | 0,15 (0–0,55) |
| Korsan savaşı (`pirate`) | 0 | 0 (0–0,2) | 0,1 (0–0,35) | 0,1 (0,05–0,3) | 0 (0–0,2) | 0,05 (0–0,35) |
| Yol pususu (`ambush`) | 0 (0–0,1) | 0 (0–0,2) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0,05 (0–0,2) |
| Düello (`duel`) | 0 | 0 | 0 | 0 | 0 | 0 (0–0,1) |
| Han baskını (medeniyet) (`innCiv`) | 0 | 0 (0–0,1) | 0 (0–0,05) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) |
| Kervan soygunu (`robbery`) | 0 | 0 (0–0,1) | 0 | 0 | 0 (0–0,1) | 0 (0–0,2) |

## Kamp türleri

Dünya başına yaşayan kamp (yıl sonu değerlerinin on yıllık ortalaması): dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Hobgoblin (`hobgoblin`) | 1,45 (1,15–1,8) | 2,3 (1,65–3,1) | 2,5 (1,75–3,7) | 3,9 (2,55–4,75) | 3,8 (2,95–4,85) | 3,5 (2,4–5,8) |
| Goblin (`goblin`) | 5,75 (4,85–6,3) | 3,35 (2,3–4,7) | 1,85 (0,95–3,05) | 1,75 (0,95–3,3) | 1,8 (1,2–3,15) | 2,1 (1,4–3,25) |
| Trol (`troll`) | 0 | 0,05 (0–0,45) | 0,45 (0,2–1,55) | 1 (0,2–2,3) | 2,2 (0,7–2,7) | 2,4 (1–2,75) |
| Bugbear (`bugbear`) | 0,45 (0,25–0,7) | 0,85 (0,3–1) | 1,2 (0,65–1,65) | 1 (0,75–1,9) | 1,2 (0,6–1,75) | 1,05 (0,6–1,45) |
| Ejderha (`dragon`) | 0 | 0,05 (0–0,3) | 1 (0,85–1) | 1 (0,05–1) | 1 (0–1) | 0,75 (0–1) |
| Korsan (`pirate`) | 0 | 0,4 (0,25–0,8) | 0,5 (0,25–0,85) | 0,4 (0,15–0,85) | 0,3 (0,1–0,9) | 0,25 (0–1,25) |

## Çağ dağılımı

Yaşayan medeniyetlerin çağlara dağılımı, bütün dünyalar (yıl sonu).

| Yıl | I Kamp | II Köy | III Kasaba | IV Krallık |
|---|---|---|---|---|
| 10 | %0,8 | %29 | %64 | %6,3 |
| 20 | %0 | %1,6 | %22 | %76 |
| 30 | %0 | %0,8 | %1,6 | %98 |
| 40 | %0 | %0 | %0 | %100 |
| 50 | %0 | %0,8 | %0 | %99 |
| 60 | %0 | %0,8 | %0 | %99 |

## Dünyalar

| Seed | Medeniyet | Yerleşim | Nüfus | Çöküş | Efsane | En yüksek Sv | Doğan / ölü kahraman | Ağaç bitişi (yıl, medyan) | Süre (sn) | Son hash |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | 9 | 96 | 4427 | 8 | 13 | 8 | 180 / 78 | 20 | 58 | `7c43b373de6c26fc` |
| 2 | 8 | 67 | 3432 | 6 | 7 | 8 | 192 / 45 | 23 | 43,7 | `154dd1e08e047624` |
| 3 | 9 | 91 | 4122 | 6 | 21 | 10 | 164 / 71 | 18 | 50,2 | `730578b902256b64` |
| 4 | 10 | 97 | 4584 | 9 | 20 | 9 | 212 / 106 | 19,5 | 54 | `e9389927293875ef` |
| 5 | 7 | 92 | 3825 | 16 | 22 | 10 | 224 / 105 | 20 | 51,4 | `131436825a72b502` |
| 6 | 7 | 71 | 3512 | 2 | 15 | 10 | 131 / 37 | 22 | 39,1 | `8b9d7109970c75d7` |
| 7 | 10 | 98 | 4509 | 9 | 15 | 10 | 265 / 150 | 21 | 62 | `601f8822892b8735` |
| 8 | 9 | 101 | 4142 | 15 | 9 | 10 | 240 / 97 | 18,5 | 63,3 | `f357093147a69c4c` |
| 9 | 9 | 84 | 4013 | 20 | 13 | 10 | 211 / 111 | 19 | 50,2 | `31fab4370cc83a0c` |
| 10 | 9 | 100 | 4283 | 10 | 14 | 9 | 194 / 81 | 21 | 56,5 | `681fbe0aa63f1865` |
| 11 | 8 | 102 | 3934 | 10 | 9 | 10 | 185 / 81 | 20 | 50,6 | `7a4257fc70a31503` |
| 12 | 7 | 78 | 3361 | 7 | 19 | 10 | 128 / 48 | 18 | 39,3 | `055c8ac40ee07e5a` |
| 13 | 6 | 66 | 3448 | 8 | 7 | 10 | 199 / 78 | 18,5 | 48,5 | `90dd651191203e7b` |
| 14 | 7 | 68 | 3246 | 2 | 21 | 9 | 196 / 84 | 23 | 40,3 | `3b0bf2753e57c35d` |
| 15 | 8 | 78 | 3791 | 16 | 13 | 9 | 238 / 104 | 22 | 51,1 | `d56a4e1dae2ebabc` |
| 16 | 7 | 89 | 3527 | 9 | 9 | 10 | 205 / 82 | 22 | 45,9 | `7eafdca3b48b691a` |

Çöküşler:

- seed 1, 22. yıl (gün 2626): Kızılboynuz Soyu başkenti kaybetti: Közsaray (Kanlıdiş Kabileleri aldı)
- seed 1, 27. yıl (gün 3225): Bulutkapı Tarikatı başkenti kaybetti: Bulutkapı (Kızılboynuz Soyu aldı)
- seed 1, 30. yıl (gün 3533): Bulutkapı Tarikatı yok oldu
- seed 1, 32. yıl (gün 3780): Rüzgâr Manastırı başkenti kaybetti: Sessiztepe (Kanlıdiş Kabileleri aldı)
- seed 1, 34. yıl (gün 3986): Kızılboynuz Soyu başkenti kaybetti: Sisliyamaç (Kanlıdiş Kabileleri aldı)
- seed 1, 46. yıl (gün 5455): Örsyürek Tapınak Klanı başkenti kaybetti: Demirçan (Kanlıdiş Kabileleri aldı)
- seed 1, 59. yıl (gün 6996): Rüzgâr Manastırı başkenti kaybetti: Dinginpınar (Kanlıdiş Kabileleri aldı)
- seed 1, 59. yıl (gün 7006): Kartalkaya Beyliği başkenti kaybetti: Kartalkaya (Kızılboynuz Soyu aldı)
- seed 2, 31. yıl (gün 3606): Tatlıçayır Loncası başkenti kaybetti: Kavşakpazar (Pulzırh Lejyonu aldı)
- seed 2, 31. yıl (gün 3680): Yeşilyaprak Çemberi yok oldu
- seed 2, 34. yıl (gün 3996): Rüzgâr Manastırı yok oldu
- seed 2, 34. yıl (gün 4010): Gökyayla Şehir Devleti başkenti kaybetti: Gökyayla (Tatlıçayır Loncası aldı)
- seed 2, 40. yıl (gün 4753): Rüzgâr Manastırı yok oldu
- seed 2, 44. yıl (gün 5174): Kanlıdiş Kabileleri yok oldu
- seed 3, 22. yıl (gün 2638): Karaörs Derinlikleri başkenti kaybetti: Karagöl (Çarkyıldız Akademisi aldı)
- seed 3, 26. yıl (gün 3010): Karaörs Derinlikleri başkenti kaybetti: Gölgeörs (Çarkyıldız Akademisi aldı)
- seed 3, 27. yıl (gün 3142): Sınır Bekçileri başkenti kaybetti: Gözcüağaç (Kanlıdiş Kabileleri aldı)
- seed 3, 50. yıl (gün 5938): Tatlıçayır Loncası başkenti kaybetti: Kavşakpazar (Kanlıdiş Kabileleri aldı)
- seed 3, 55. yıl (gün 6582): Kanlıdiş Kabileleri başkenti kaybetti: Çankule (Rüzgâr Manastırı aldı)
- seed 3, 60. yıl (gün 7190): Rüzgâr Manastırı başkenti kaybetti: Çankule (Kanlıdiş Kabileleri aldı)
- seed 4, 22. yıl (gün 2610): Kanlıdiş Kabileleri başkenti kaybetti: Alevgeçit (Pulzırh Lejyonu aldı)
- seed 4, 46. yıl (gün 5503): Sınır Bekçileri başkenti kaybetti: İzsürer (Pulzırh Lejyonu aldı)
- seed 4, 48. yıl (gün 5660): Sınır Bekçileri başkenti kaybetti: Ayburç (Kanlıdiş Kabileleri aldı)
- seed 4, 49. yıl (gün 5878): Rüzgâr Manastırı başkenti kaybetti: Sessiztepe (Kanlıdiş Kabileleri aldı)
- seed 4, 51. yıl (gün 6080): Sınır Bekçileri başkenti kaybetti: Kurtgeçit (Kanlıdiş Kabileleri aldı)
- seed 4, 52. yıl (gün 6143): Çankule Boyu başkenti kaybetti: Çankule (Rüzgâr Manastırı aldı)
- seed 4, 56. yıl (gün 6709): Rüzgâr Manastırı başkenti kaybetti: Karlıçayır (Kanlıdiş Kabileleri aldı)
- seed 4, 60. yıl (gün 7133): Rüzgâr Manastırı başkenti kaybetti: Sisliyamaç (Kanlıdiş Kabileleri aldı)
- seed 4, 60. yıl (gün 7195): Rüzgâr Manastırı başkenti kaybetti: Bulutkapı (Çankule Boyu aldı)
- seed 5, 10. yıl (gün 1188): Karaörs Derinlikleri başkenti kaybetti: Külçukur (Kanlıdiş Kabileleri aldı)
- seed 5, 13. yıl (gün 1502): Karaörs Derinlikleri yok oldu
- seed 5, 21. yıl (gün 2412): Tatlıçayır Loncası başkenti kaybetti: Kırkkapı (Kanlıdiş Kabileleri aldı)
- seed 5, 23. yıl (gün 2721): Tatlıçayır Loncası başkenti kaybetti: Keseli (Kızılboynuz Soyu aldı)
- seed 5, 26. yıl (gün 3090): Güneştacı Krallığı başkenti kaybetti: Kalkanova (Pulzırh Lejyonu aldı)
- seed 5, 27. yıl (gün 3230): Tatlıçayır Loncası başkenti kaybetti: Kuzeybük (Kanlıdiş Kabileleri aldı)
- seed 5, 29. yıl (gün 3373): Tatlıçayır Loncası başkenti kaybetti: Söğütsırt (Kızılboynuz Soyu aldı)
- seed 5, 30. yıl (gün 3501): Güneştacı Krallığı başkenti kaybetti: Altınkapı (Pulzırh Lejyonu aldı)
- seed 5, 32. yıl (gün 3739): Tatlıçayır Loncası yok oldu
- seed 5, 33. yıl (gün 3918): Güneştacı Krallığı başkenti kaybetti: Yeminköprü (Pulzırh Lejyonu aldı)
- seed 5, 50. yıl (gün 5971): Kızılboynuz Soyu başkenti kaybetti: Közsaray (Kanlıdiş Kabileleri aldı)
- seed 5, 52. yıl (gün 6185): Kanlıdiş Kabileleri başkenti kaybetti: Közsaray (Pulzırh Lejyonu aldı)
- seed 5, 54. yıl (gün 6461): Pulzırh Lejyonu başkenti kaybetti: Közsaray (Kanlıdiş Kabileleri aldı)
- seed 5, 56. yıl (gün 6660): Kızılboynuz Soyu başkenti kaybetti: Alazvadi (Kanlıdiş Kabileleri aldı)
- seed 5, 58. yıl (gün 6873): Keseli Serbest Şehri yok oldu
- seed 5, 58. yıl (gün 6920): Kızılboynuz Soyu başkenti kaybetti: Söğütburç (Kanlıdiş Kabileleri aldı)
- seed 6, 33. yıl (gün 3865): Karagöl Bekçileri yok oldu
- seed 6, 40. yıl (gün 4763): Karaörs Derinlikleri başkenti kaybetti: Gölgeörs (Pulzırh Lejyonu aldı)
- seed 7, 12. yıl (gün 1415): Kızılboynuz Soyu başkenti kaybetti: Közsaray (Karaörs Derinlikleri aldı)
- seed 7, 33. yıl (gün 3929): Yeşilyaprak Çemberi başkenti kaybetti: Yosunpınar (Karaörs Derinlikleri aldı)
- seed 7, 34. yıl (gün 4006): Rüzgâr Manastırı başkenti kaybetti: Dinginpınar (Kızılboynuz Soyu aldı)
- seed 7, 36. yıl (gün 4232): Yeşilyaprak Çemberi başkenti kaybetti: Dumanköprü (Karaörs Derinlikleri aldı)
- seed 7, 40. yıl (gün 4724): Yeşilyaprak Çemberi başkenti kaybetti: Yeşilyazı (Karaörs Derinlikleri aldı)
- seed 7, 48. yıl (gün 5722): Rüzgâr Manastırı başkenti kaybetti: Ulukoru (Kızılboynuz Soyu aldı)
- seed 7, 54. yıl (gün 6398): Kanlıdiş Kabileleri başkenti kaybetti: Kızılkül (Karaörs Derinlikleri aldı)
- seed 7, 59. yıl (gün 6961): Kanlıdiş Kabileleri başkenti kaybetti: Közsaray (Kızılboynuz Soyu aldı)
- seed 7, 60. yıl (gün 7179): Kanlıdiş Kabileleri başkenti kaybetti: Uluova (Karaörs Derinlikleri aldı)
- seed 8, 14. yıl (gün 1564): Sınır Bekçileri başkenti kaybetti: Gözcüağaç (Kızılboynuz Soyu aldı)
- seed 8, 16. yıl (gün 1824): Örsyürek Tapınak Klanı başkenti kaybetti: Boynuztepe (Kızılboynuz Soyu aldı)
- seed 8, 18. yıl (gün 2047): Sınır Bekçileri başkenti kaybetti: Okyayı (Kızılboynuz Soyu aldı)
- seed 8, 22. yıl (gün 2546): Örsyürek Tapınak Klanı başkenti kaybetti: Kutsalörs (Kızılboynuz Soyu aldı)
- seed 8, 24. yıl (gün 2810): Kızılboynuz Soyu başkenti kaybetti: Közsaray (Pulzırh Lejyonu aldı)
- seed 8, 25. yıl (gün 2978): Kızılboynuz Soyu başkenti kaybetti: Alazvadi (Pulzırh Lejyonu aldı)
- seed 8, 28. yıl (gün 3298): Sınır Bekçileri başkenti kaybetti: Çamgözcü (Kızılboynuz Soyu aldı)
- seed 8, 30. yıl (gün 3511): Demirçan Klanı başkenti kaybetti: Demirçan (Kızılboynuz Soyu aldı)
- seed 8, 44. yıl (gün 5218): Sınır Bekçileri başkenti kaybetti: Yıldızçayır (Kızılboynuz Soyu aldı)
- seed 8, 52. yıl (gün 6178): Sınır Bekçileri başkenti kaybetti: Kuzeyoba (Kızılboynuz Soyu aldı)
- seed 8, 55. yıl (gün 6579): Kızılboynuz Soyu başkenti kaybetti: Kuzeyoba (Rüzgâr Manastırı aldı)
- seed 8, 55. yıl (gün 6591): Kocapınar Kara Beyliği başkenti kaybetti: Kocapınar (Sınır Bekçileri aldı)
- seed 8, 56. yıl (gün 6603): Sınır Bekçileri başkenti kaybetti: Yıldıztepe (Kızılboynuz Soyu aldı)
- seed 8, 58. yıl (gün 6888): Kocapınar Kara Beyliği yok oldu
- seed 8, 59. yıl (gün 7017): Kızılboynuz Soyu başkenti kaybetti: Parşömenli (Rüzgâr Manastırı aldı)
- seed 9, 10. yıl (gün 1195): Sınır Bekçileri başkenti kaybetti: İzsürer (Karaörs Derinlikleri aldı)
- seed 9, 14. yıl (gün 1579): Pulzırh Lejyonu başkenti kaybetti: Ejderkale (Kanlıdiş Kabileleri aldı)
- seed 9, 14. yıl (gün 1630): Sınır Bekçileri başkenti kaybetti: Gözcüağaç (Karaörs Derinlikleri aldı)
- seed 9, 17. yıl (gün 1924): Sınır Bekçileri yok oldu
- seed 9, 18. yıl (gün 2120): Pulzırh Lejyonu başkenti kaybetti: Pulkalkan (Kanlıdiş Kabileleri aldı)
- seed 9, 20. yıl (gün 2398): Pulzırh Lejyonu başkenti kaybetti: Kılıçyurt (Kanlıdiş Kabileleri aldı)
- seed 9, 23. yıl (gün 2671): Pulzırh Lejyonu yok oldu
- seed 9, 33. yıl (gün 3848): Güneştacı Krallığı başkenti kaybetti: Karagöl (Karaörs Derinlikleri aldı)
- seed 9, 35. yıl (gün 4107): Rüzgâr Manastırı başkenti kaybetti: Sessiztepe (Kanlıdiş Kabileleri aldı)
- seed 9, 39. yıl (gün 4640): Güneştacı Krallığı başkenti kaybetti: Sarıhisar (Kanlıdiş Kabileleri aldı)
- seed 9, 41. yıl (gün 4855): Karaörs Derinlikleri başkenti kaybetti: Karagöl (Kızılboynuz Soyu aldı)
- seed 9, 41. yıl (gün 4907): Kanlıdiş Kabileleri başkenti kaybetti: Sarıhisar (Rüzgâr Manastırı aldı)
- seed 9, 43. yıl (gün 5079): Kanlıdiş Kabileleri başkenti kaybetti: Kızılkül (Rüzgâr Manastırı aldı)
- seed 9, 45. yıl (gün 5300): Karaörs Derinlikleri başkenti kaybetti: Yelgeçit (Güneştacı Krallığı aldı)
- seed 9, 47. yıl (gün 5551): Karaörs Derinlikleri başkenti kaybetti: Kara Mihrap (Güneştacı Krallığı aldı)
- seed 9, 49. yıl (gün 5823): Güneştacı Krallığı başkenti kaybetti: Kara Mihrap (Karaörs Derinlikleri aldı)
- seed 9, 52. yıl (gün 6209): Karaörs Derinlikleri başkenti kaybetti: Kara Mihrap (Güneştacı Krallığı aldı)
- seed 9, 56. yıl (gün 6663): Karaörs Derinlikleri başkenti kaybetti: Külçukur (Örsyürek Tapınak Klanı aldı)
- seed 9, 57. yıl (gün 6827): Karaörs Derinlikleri başkenti kaybetti: Gölgeörs (Örsyürek Tapınak Klanı aldı)
- seed 9, 58. yıl (gün 6856): Kanlıdiş Kabileleri başkenti kaybetti: Kıvılcımlı (Kızılboynuz Soyu aldı)
- seed 10, 23. yıl (gün 2670): Çarkyıldız Akademisi başkenti kaybetti: Dişlivadi (Kanlıdiş Kabileleri aldı)
- seed 10, 24. yıl (gün 2875): Sınır Bekçileri başkenti kaybetti: Gözcüağaç (Kanlıdiş Kabileleri aldı)
- seed 10, 29. yıl (gün 3469): Çarkyıldız Akademisi yok oldu
- seed 10, 32. yıl (gün 3726): Sınır Bekçileri başkenti kaybetti: Yabanyurt (Kanlıdiş Kabileleri aldı)
- seed 10, 43. yıl (gün 5119): Karaörs Derinlikleri başkenti kaybetti: Ceylanbük (Örsyürek Tapınak Klanı aldı)
- seed 10, 49. yıl (gün 5857): Karaörs Derinlikleri başkenti kaybetti: Ceylanbük (Tatlıçayır Loncası aldı)
- seed 10, 51. yıl (gün 6084): Karaörs Derinlikleri başkenti kaybetti: Kara Mihrap (Rüzgâr Manastırı aldı)
- seed 10, 51. yıl (gün 6114): Sınır Bekçileri başkenti kaybetti: Okyayı (Karaörs Derinlikleri aldı)
- seed 10, 53. yıl (gün 6308): Tatlıçayır Loncası başkenti kaybetti: Söğütsırt (Kanlıdiş Kabileleri aldı)
- seed 10, 55. yıl (gün 6538): Sınır Bekçileri başkenti kaybetti: Karlıçayır (Kanlıdiş Kabileleri aldı)
- seed 11, 15. yıl (gün 1744): Çarkyıldız Akademisi başkenti kaybetti: Pusulakule (Kızılboynuz Soyu aldı)
- seed 11, 17. yıl (gün 1961): Örsyürek Tapınak Klanı başkenti kaybetti: Demirçan (Kanlıdiş Kabileleri aldı)
- seed 11, 18. yıl (gün 2047): Çarkyıldız Akademisi başkenti kaybetti: Kristalköy (Kızılboynuz Soyu aldı)
- seed 11, 22. yıl (gün 2589): Kızılboynuz Soyu başkenti kaybetti: Közsaray (Kanlıdiş Kabileleri aldı)
- seed 11, 24. yıl (gün 2783): Örsyürek Tapınak Klanı başkenti kaybetti: Közkapı (Kanlıdiş Kabileleri aldı)
- seed 11, 29. yıl (gün 3388): Yeşilyaprak Çemberi başkenti kaybetti: Çiyardı (Kanlıdiş Kabileleri aldı)
- seed 11, 29. yıl (gün 3465): Çarkyıldız Akademisi başkenti kaybetti: Mürekkeptepe (Kızılboynuz Soyu aldı)
- seed 11, 33. yıl (gün 3923): Çarkyıldız Akademisi başkenti kaybetti: Yıldıztepe (Kızılboynuz Soyu aldı)
- seed 11, 48. yıl (gün 5715): Çarkyıldız Akademisi başkenti kaybetti: Sisova (Kanlıdiş Kabileleri aldı)
- seed 11, 49. yıl (gün 5875): Rüzgâr Manastırı başkenti kaybetti: Sessiztepe (Kızılboynuz Soyu aldı)
- seed 12, 36. yıl (gün 4243): Kızılboynuz Soyu başkenti kaybetti: Karamum (Pulzırh Lejyonu aldı)
- seed 12, 40. yıl (gün 4702): Ceylanbük Bekçileri yok oldu
- seed 12, 41. yıl (gün 4814): Kızılboynuz Soyu başkenti kaybetti: Alazvadi (Pulzırh Lejyonu aldı)
- seed 12, 42. yıl (gün 5014): Sınır Bekçileri başkenti kaybetti: Gözcüağaç (Kanlıdiş Kabileleri aldı)
- seed 12, 46. yıl (gün 5448): Sınır Bekçileri başkenti kaybetti: Kurtgeçit (Kanlıdiş Kabileleri aldı)
- seed 12, 49. yıl (gün 5813): Karaörs Derinlikleri başkenti kaybetti: Alevgeçit (Pulzırh Lejyonu aldı)
- seed 12, 60. yıl (gün 7111): Karaörs Derinlikleri başkenti kaybetti: Karagöl (Pulzırh Lejyonu aldı)
- seed 13, 14. yıl (gün 1646): Kanlıdiş Kabileleri başkenti kaybetti: Kurtgeçit (Sınır Bekçileri aldı)
- seed 13, 23. yıl (gün 2675): Sınır Bekçileri başkenti kaybetti: Gözcüağaç (Kanlıdiş Kabileleri aldı)
- seed 13, 36. yıl (gün 4204): Sınır Bekçileri başkenti kaybetti: Kurtgeçit (Kızılboynuz Soyu aldı)
- seed 13, 39. yıl (gün 4677): Sınır Bekçileri başkenti kaybetti: Siskoru (Kızılboynuz Soyu aldı)
- seed 13, 42. yıl (gün 4945): Gökyurt Boyu başkenti kaybetti: Gökyurt (Sınır Bekçileri aldı)
- seed 13, 44. yıl (gün 5252): Gökyurt Boyu yok oldu
- seed 13, 48. yıl (gün 5728): Sınır Bekçileri yok oldu
- seed 13, 60. yıl (gün 7121): Kanlıdiş Kabileleri yok oldu
- seed 14, 26. yıl (gün 3017): Sınır Bekçileri başkenti kaybetti: Gözcüağaç (Karaörs Derinlikleri aldı)
- seed 14, 60. yıl (gün 7192): Karaörs Derinlikleri başkenti kaybetti: Kocapınar (Çarkyıldız Akademisi aldı)
- seed 15, 18. yıl (gün 2063): Sınır Bekçileri başkenti kaybetti: Okyayı (Kanlıdiş Kabileleri aldı)
- seed 15, 24. yıl (gün 2776): Sınır Bekçileri başkenti kaybetti: Kurtgeçit (Kanlıdiş Kabileleri aldı)
- seed 15, 27. yıl (gün 3177): Kanlıdiş Kabileleri başkenti kaybetti: Kurtgeçit (Güneştacı Krallığı aldı)
- seed 15, 27. yıl (gün 3207): Sınır Bekçileri başkenti kaybetti: Çamgözcü (Kanlıdiş Kabileleri aldı)
- seed 15, 31. yıl (gün 3644): Sınır Bekçileri başkenti kaybetti: Meşekale (Kanlıdiş Kabileleri aldı)
- seed 15, 31. yıl (gün 3656): Kartalkaya Klanı başkenti kaybetti: Kartalkaya (Kızılboynuz Soyu aldı)
- seed 15, 32. yıl (gün 3746): Kartalkaya Klanı yok oldu
- seed 15, 39. yıl (gün 4607): Kanlıdiş Kabileleri başkenti kaybetti: Kurtgeçit (Güneştacı Krallığı aldı)
- seed 15, 39. yıl (gün 4630): Kanlıdiş Kabileleri başkenti kaybetti: Boynuztepe (Kızılboynuz Soyu aldı)
- seed 15, 40. yıl (gün 4703): Çamgözcü Boyu başkenti kaybetti: Çamgözcü (Kanlıdiş Kabileleri aldı)
- seed 15, 46. yıl (gün 5463): Kanlıdiş Kabileleri başkenti kaybetti: Sarıdere (Güneştacı Krallığı aldı)
- seed 15, 48. yıl (gün 5650): Yabanyurt Bekçileri yok oldu
- seed 15, 49. yıl (gün 5873): Çamgözcü Boyu başkenti kaybetti: Ceylanbük (Kanlıdiş Kabileleri aldı)
- seed 15, 51. yıl (gün 6026): Kanlıdiş Kabileleri başkenti kaybetti: Ceylanbük (Güneştacı Krallığı aldı)
- seed 15, 54. yıl (gün 6420): Çamgözcü Boyu başkenti kaybetti: Bozkent (Kanlıdiş Kabileleri aldı)
- seed 15, 56. yıl (gün 6655): Çamgözcü Boyu yok oldu
- seed 16, 7. yıl (gün 808): Güneştacı Krallığı yok oldu
- seed 16, 13. yıl (gün 1451): Rüzgâr Manastırı yok oldu
- seed 16, 16. yıl (gün 1808): Örsyürek Tapınak Klanı başkenti kaybetti: Demirçan (Karaörs Derinlikleri aldı)
- seed 16, 19. yıl (gün 2212): Örsyürek Tapınak Klanı başkenti kaybetti: Közkapı (Karaörs Derinlikleri aldı)
- seed 16, 25. yıl (gün 2953): Tatlıçayır Loncası başkenti kaybetti: Kavşakpazar (Kanlıdiş Kabileleri aldı)
- seed 16, 30. yıl (gün 3488): Örsyürek Tapınak Klanı başkenti kaybetti: Granitsunak (Karaörs Derinlikleri aldı)
- seed 16, 34. yıl (gün 3988): Örsyürek Tapınak Klanı başkenti kaybetti: Meşekent (Kanlıdiş Kabileleri aldı)
- seed 16, 54. yıl (gün 6442): Örsyürek Tapınak Klanı başkenti kaybetti: Söğütsırt (Kanlıdiş Kabileleri aldı)
- seed 16, 58. yıl (gün 6887): Örsyürek Tapınak Klanı yok oldu

## Yıllık ayrıntı

Hücre: medyan (p10–p90), 16 dünya. Yıl y = (y−1)·120+1 … y·120. günler. Bütün değerler `report.json` içinde (`metrics`), dünya başına değerler `../runs/b2-6` altında.

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
| 14 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 46 (37–48,5) |
| 15 | 8 (7–9) | 0 | 0 | 0 | 0 | 48 (40,5–53) |
| 16 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 51 (41,5–56) |
| 17 | 8 (7–9) | 0 | 0 | 0 | 0 (0–0,5) | 53 (44–58,5) |
| 18 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 54 (46–60,5) |
| 19 | 8 (7–9) | 0 | 0 | 0 | 0 | 56 (47,5–63) |
| 20 | 8 (7–9) | 0 | 0 | 0 | 0 | 57 (49,5–64) |
| 21 | 8 (7–9) | 0 | 0 | 0 | 0 | 58 (50,5–66) |
| 22 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 61 (53,5–68) |
| 23 | 8 (7–9) | 0 (0–0,5) | 0 | 0 (0–1) | 0 (0–1) | 61,5 (54,5–69) |
| 24 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 63,5 (55–69,5) |
| 25 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 64,5 (57,5–72) |
| 26 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 65,5 (58–72) |
| 27 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 66,5 (58,5–72,5) |
| 28 | 8 (7–9) | 0 | 0 | 0 | 0 | 68 (59,5–73) |
| 29 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–1) | 71 (60–75) |
| 30 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 71 (60,5–77) |
| 31 | 8 (7–9) | 0 (0–0,5) | 0 | 0 (0–0,5) | 0 (0–1) | 72 (61,5–78,5) |
| 32 | 8 (7–9) | 0 | 0 (0–0,5) | 0 (0–0,5) | 0 (0–1) | 72,5 (61,5–79,5) |
| 33 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 73,5 (62,5–80) |
| 34 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 74,5 (63–80,5) |
| 35 | 8 (7–9) | 0 | 0 | 0 | 0 | 76 (64–82) |
| 36 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 76,5 (65,5–82) |
| 37 | 8 (7–9) | 0 | 0 | 0 | 0 | 77,5 (66–83,5) |
| 38 | 8 (7–9) | 0 | 0 | 0 | 0 | 79 (66,5–84,5) |
| 39 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 80 (67–86) |
| 40 | 8 (7–9) | 0 | 0 (0–0,5) | 0 (0–1) | 0 (0–1) | 81,5 (67,5–86,5) |
| 41 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 82 (67,5–87) |
| 42 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 82,5 (67,5–89) |
| 43 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 83 (67,5–91) |
| 44 | 8 (7–9) | 0 | 0 (0–0,5) | 0 | 0 (0–1) | 83,5 (68–91,5) |
| 45 | 8 (7–9) | 0 (0–0,5) | 0 | 0 | 0 | 85 (68,5–93,5) |
| 46 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 85 (68,5–93,5) |
| 47 | 8 (7–9) | 0 | 0 | 0 | 0 | 86 (68,5–94) |
| 48 | 8 (7–9) | 0 | 0 (0–0,5) | 0 (0–1) | 0 (0–1) | 86,5 (69–95) |
| 49 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 87 (69–96) |
| 50 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 87,5 (69,5–96,5) |
| 51 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 88 (69,5–97,5) |
| 52 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 88,5 (68,5–97) |
| 53 | 8 (7–9,5) | 0 | 0 | 0 | 0 | 88 (68,5–97,5) |
| 54 | 8 (7–9,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 88 (68,5–98) |
| 55 | 8 (7–9,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 88,5 (69–99) |
| 56 | 8 (7–10) | 0 | 0 | 0 (0–1) | 0 (0–1) | 89 (69–99) |
| 57 | 8 (7–10) | 0 | 0 | 0 | 0 | 89,5 (68,5–99) |
| 58 | 8 (7–9,5) | 0 | 0 (0–1) | 0 (0–0,5) | 0 (0–1) | 89,5 (67–99) |
| 59 | 8 (7–9,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 89,5 (67–100) |
| 60 | 8 (7–9,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 90 (67,5–100) |

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
| 12 | 4,82 (4,26–5,25) | %71 (%44–%87) | 4 (1,5–5) | 0 (0–1) | 0 | 920 (738–1020) |
| 13 | 5,29 (4,72–5,63) | %86 (%56–%88) | 3 (1–4,5) | 1 (0–1) | 0 | 1080 (828–1166) |
| 14 | 5,47 (5,13–6) | %88 (%67–%100) | 3 (1–5) | 1 (0–2) | 0 | 1220 (934–1342) |
| 15 | 5,94 (5,29–6,31) | %88 (%69–%100) | 2,5 (1–5,5) | 1 (0–2) | 0 | 1364 (1031–1479) |
| 16 | 6,18 (5,67–6,75) | %88 (%69–%100) | 2 (1–4) | 0,5 (0–1,5) | 0 | 1524 (1160–1604) |
| 17 | 6,56 (6,06–7,06) | %94 (%73–%100) | 3 (1–4) | 0 (0–2) | 0 | 1654 (1244–1749) |
| 18 | 6,87 (6,29–7,31) | %88 (%82–%100) | 2 (1–3,5) | 1 (0–1,5) | 0 | 1759 (1350–1893) |
| 19 | 7,06 (6,54–7,53) | %94 (%82–%100) | 2 (0,5–3) | 1 (0–2) | 0 | 1870 (1489–2042) |
| 20 | 7,25 (6,93–7,83) | %94 (%86–%100) | 2 (1–3) | 1 (0–2,5) | 0 | 1972 (1566–2160) |
| 21 | 7,44 (7,06–7,94) | %100 (%86–%100) | 1 (1–2) | 1 (0–1) | 0 | 2097 (1697–2320) |
| 22 | 7,69 (7,41–8,13) | %100 (%87–%100) | 2 (0–3,5) | 1 (0–2) | 0 | 2102 (1770–2402) |
| 23 | 7,76 (7,28–8,27) | %94 (%87–%100) | 1 (0–2) | 1 (0–2) | 0 | 2170 (1866–2471) |
| 24 | 7,86 (7,44–8,47) | %94 (%88–%100) | 1 (0–2,5) | 1 (0–2,5) | 0 | 2354 (1964–2568) |
| 25 | 8,06 (7,4–8,53) | %94 (%88–%100) | 1 (0–2,5) | 1 (0–2) | 0 | 2378 (2030–2664) |
| 26 | 8,13 (7,53–8,72) | %89 (%88–%100) | 1 (0,5–2,5) | 1 (0–2) | 0 | 2468 (2062–2847) |
| 27 | 8,19 (7,7–8,83) | %89 (%88–%100) | 1 (0–2) | 1 (0–3) | 0 (0–0,5) | 2526 (2158–2936) |
| 28 | 8,25 (7,75–9) | %89 (%88–%100) | 1 (0–3) | 1 (0–1,5) | 0 (0–1) | 2606 (2238–3024) |
| 29 | 8,46 (8,06–9,25) | %94 (%87–%100) | 1,5 (0–3) | 1 (0–2) | 0 | 2706 (2266–3056) |
| 30 | 8,67 (8–9,5) | %94 (%87–%100) | 1 (0–2) | 1 (0–2,5) | 0 | 2828 (2346–3204) |
| 31 | 8,82 (7,82–9,56) | %100 (%87–%100) | 1 (0–2) | 1 (0–2) | 0 | 2814 (2448–3217) |
| 32 | 8,88 (8,1–10) | %100 (%88–%100) | 1 (0–2) | 1 (0–2) | 0 (0–0,5) | 2902 (2476–3314) |
| 33 | 9,06 (8,11–10) | %100 (%83–%100) | 1 (0–2,5) | 1,5 (1–3,5) | 0 (0–1) | 2918 (2558–3432) |
| 34 | 9,2 (8,28–10) | %100 (%87–%100) | 1 (0–1,5) | 1 (0–2) | 0 | 3010 (2596–3392) |
| 35 | 9,33 (8,28–10,1) | %100 (%83–%100) | 1 (0–2) | 1 (0–2) | 0 | 3091 (2636–3454) |
| 36 | 9,47 (8,39–10,1) | %100 (%88–%100) | 0 (0–1,5) | 1 (0–3,5) | 0 | 3062 (2726–3599) |
| 37 | 9,47 (8,39–10,3) | %94 (%88–%100) | 0,5 (0–2) | 1 (0–1,5) | 0 | 3132 (2795–3640) |
| 38 | 9,38 (8,36–10,4) | %89 (%83–%100) | 0 (0–2) | 1,5 (0–2) | 0 | 3227 (2806–3684) |
| 39 | 9,56 (8,39–10,6) | %100 (%78–%100) | 1 (0–2) | 1 (0–3) | 0 | 3236 (2836–3710) |
| 40 | 9,69 (8,44–10,6) | %100 (%78–%100) | 1 (0–2) | 1 (0–3) | 0 (0–0,5) | 3288 (2819–3692) |
| 41 | 9,69 (8,5–10,8) | %100 (%78–%100) | 0,5 (0–2,5) | 1 (0–2,5) | 0 | 3334 (2908–3809) |
| 42 | 9,71 (8,56–10,9) | %100 (%78–%100) | 1 (0–1) | 1 (0–2,5) | 0 | 3384 (2928–3827) |
| 43 | 9,79 (8,56–11,3) | %100 (%78–%100) | 1 (0–2) | 2 (0,5–4,5) | 0 (0–1) | 3423 (2978–3894) |
| 44 | 9,86 (9,22–11) | %94 (%86–%100) | 1 (0–1) | 1 (0–2,5) | 0 (0–1) | 3474 (2940–4058) |
| 45 | 9,87 (8,78–11,2) | %89 (%82–%100) | 1 (0–2,5) | 1,5 (0–3) | 0 (0–0,5) | 3496 (3062–4016) |
| 46 | 9,88 (8,73–11,3) | %89 (%82–%100) | 0 (0–1,5) | 1 (0–2) | 0 (0–1) | 3550 (3058–4018) |
| 47 | 9,87 (8,69–11,2) | %89 (%75–%100) | 0,5 (0–1) | 1 (0–3) | 0 (0–1,5) | 3564 (3070–4140) |
| 48 | 10,1 (8,85–11,6) | %94 (%76–%100) | 1 (0–1,5) | 1 (0–3,5) | 0 (0–1) | 3525 (3124–4110) |
| 49 | 10,2 (8,85–11,7) | %100 (%76–%100) | 0,5 (0–1) | 1 (0–2,5) | 0 | 3544 (3224–4168) |
| 50 | 10,1 (8,98–11,8) | %94 (%82–%100) | 0 (0–1) | 1 (0–2) | 0 (0–1) | 3656 (3230–4210) |
| 51 | 10,3 (9,04–11,8) | %94 (%82–%100) | 1 (0–2) | 1 (0–2) | 0 | 3653 (3270–4262) |
| 52 | 10,4 (8,67–11,8) | %94 (%79–%100) | 0 (0–1,5) | 2 (0–3,5) | 0 (0–0,5) | 3645 (3285–4321) |
| 53 | 10,2 (8,72–11,9) | %89 (%79–%100) | 0 (0–1) | 1 (0–2,5) | 0 (0–0,5) | 3752 (3324–4240) |
| 54 | 10,3 (8,77–11,5) | %89 (%79–%100) | 1 (0–1) | 1 (0–3) | 0 (0–1) | 3730 (3261–4332) |
| 55 | 10,3 (8,88–11,5) | %89 (%79–%100) | 0 (0–1) | 1 (0–2,5) | 0 | 3735 (3343–4264) |
| 56 | 10,1 (9,51–11,5) | %89 (%83–%100) | 0,5 (0–2) | 1,5 (0–3) | 0 (0–1,5) | 3750 (3364–4392) |
| 57 | 10,1 (9,56–11,3) | %89 (%86–%100) | 1 (0–1) | 1 (0–2,5) | 0 (0–1) | 3760 (3396–4501) |
| 58 | 10,3 (9,46–12,6) | %89 (%86–%100) | 0 (0–1) | 1 (0–2) | 0 (0–1) | 3874 (3386–4359) |
| 59 | 10,1 (9,33–12,6) | %89 (%82–%100) | 0 (0–1) | 1 (0–2) | 0 | 3790 (3454–4592) |
| 60 | 10,4 (9,52–12,7) | %89 (%82–%100) | 1 (0–1) | 1 (0–2,5) | 0 (0–1) | 3880 (3396–4468) |

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
| 12 | 3,06 (2,93–3,22) | %55 (%52–%59) | %0 | %13 (%0–%31) | 177 (66,8–294) | %3,4 (%1,2–%5,7) |
| 13 | 3,13 (3–3,38) | %61 (%59–%66) | %0 | %31 (%12–%44) | 246 (139–412) | %5,3 (%2,2–%8,9) |
| 14 | 3,38 (3–3,49) | %66 (%63–%70) | %0 (%0–%14) | %31 (%12–%60) | 436 (188–592) | %10 (%6,7–%14) |
| 15 | 3,38 (3–3,63) | %72 (%65–%75) | %12 (%0–%14) | %33 (%12–%76) | 636 (268–896) | %14 (%11–%19) |
| 16 | 3,56 (3,13–3,73) | %76 (%67–%79) | %18 (%12–%31) | %53 (%40–%71) | 837 (357–1136) | %19 (%13–%23) |
| 17 | 3,63 (3,31–3,75) | %79 (%71–%82) | %25 (%13–%40) | %56 (%40–%73) | 1070 (532–1410) | %23 (%19–%28) |
| 18 | 3,65 (3,31–3,75) | %82 (%74–%85) | %38 (%24–%53) | %71 (%56–%81) | 1529 (486–1791) | %27 (%22–%36) |
| 19 | 3,75 (3,46–3,89) | %84 (%76–%87) | %44 (%24–%62) | %62 (%50–%80) | 1903 (999–2661) | %31 (%28–%39) |
| 20 | 3,75 (3,5–3,94) | %86 (%79–%89) | %50 (%31–%69) | %75 (%56–%87) | 2262 (1556–3103) | %33 (%29–%41) |
| 21 | 3,75 (3,67–4) | %88 (%82–%91) | %56 (%33–%65) | %73 (%50–%82) | 2320 (1750–3593) | %37 (%33–%45) |
| 22 | 3,86 (3,75–4) | %90 (%85–%93) | %62 (%46–%88) | %76 (%64–%88) | 2598 (1602–3779) | %40 (%36–%46) |
| 23 | 3,88 (3,75–4) | %91 (%88–%93) | %73 (%62–%94) | %86 (%71–%100) | 2851 (1697–3647) | %41 (%35–%46) |
| 24 | 4 (3,76–4) | %93 (%90–%94) | %76 (%60–%89) | %87 (%64–%100) | 2990 (1657–4880) | %43 (%38–%49) |
| 25 | 4 (3,76–4) | %94 (%90–%95) | %82 (%67–%100) | %88 (%73–%100) | 3117 (2081–4836) | %46 (%43–%52) |
| 26 | 4 (3,76–4) | %94 (%91–%96) | %88 (%73–%100) | %89 (%76–%100) | 3256 (2324–6687) | %47 (%43–%51) |
| 27 | 4 (3,76–4) | %95 (%91–%97) | %89 (%75–%100) | %100 (%80–%100) | 3638 (2683–6520) | %49 (%41–%53) |
| 28 | 4 (3,76–4) | %95 (%92–%98) | %100 (%76–%100) | %100 (%87–%100) | 4051 (2364–5689) | %47 (%45–%53) |
| 29 | 4 (3,83–4) | %96 (%92–%98) | %100 (%87–%100) | %100 (%88–%100) | 3932 (2529–6060) | %50 (%46–%53) |
| 30 | 4 (3,88–4) | %96 (%93–%98) | %100 (%87–%100) | %100 (%89–%100) | 4795 (2280–6384) | %49 (%46–%52) |
| 31 | 4 (3,94–4) | %96 (%94–%99) | %100 (%87–%100) | %100 (%87–%100) | 4293 (2629–6870) | %51 (%46–%55) |
| 32 | 4 (3,94–4) | %96 (%95–%99) | %100 (%87–%100) | %100 (%87–%100) | 4205 (2635–8068) | %50 (%41–%55) |
| 33 | 4 (3,94–4) | %96 (%95–%99) | %100 (%83–%100) | %100 (%83–%100) | 4384 (2887–8343) | %49 (%42–%53) |
| 34 | 4 | %96 (%95–%99) | %100 (%88–%100) | %100 (%89–%100) | 4456 (3050–8821) | %49 (%45–%53) |
| 35 | 4 (3,94–4) | %96 (%95–%99) | %100 (%88–%100) | %100 (%89–%100) | 4412 (2951–9020) | %49 (%47–%54) |
| 36 | 4 (3,94–4) | %97 (%95–%99) | %100 (%88–%100) | %100 (%89–%100) | 4047 (2924–10490) | %49 (%47–%53) |
| 37 | 4 | %97 (%95–%99) | %100 (%88–%100) | %100 (%88–%100) | 4509 (2966–11025) | %50 (%46–%53) |
| 38 | 4 | %97 (%96–%99) | %100 (%88–%100) | %100 (%88–%100) | 4998 (3302–9305) | %49 (%47–%54) |
| 39 | 4 | %97 (%96–%99) | %100 (%82–%100) | %100 (%82–%100) | 4742 (3239–10129) | %47 (%44–%49) |
| 40 | 4 | %98 (%96–%99) | %100 (%89–%100) | %100 (%89–%100) | 5273 (2968–11100) | %48 (%44–%52) |
| 41 | 4 | %98 (%97–%99) | %100 (%89–%100) | %100 (%89–%100) | 5720 (3070–8636) | %48 (%46–%52) |
| 42 | 4 | %98 (%97–%99) | %100 (%94–%100) | %100 (%94–%100) | 5635 (3534–8780) | %49 (%43–%51) |
| 43 | 4 | %98 (%97–%99) | %100 (%88–%100) | %100 (%88–%100) | 6030 (3809–10577) | %47 (%42–%52) |
| 44 | 4 | %98 (%97–%99) | %100 | %100 | 5885 (3359–11458) | %47 (%41–%52) |
| 45 | 4 | %99 (%97–%100) | %100 (%94–%100) | %100 (%94–%100) | 5422 (3968–11628) | %48 (%41–%52) |
| 46 | 4 | %99 (%97–%100) | %100 (%94–%100) | %100 (%94–%100) | 5513 (3005–12625) | %48 (%41–%52) |
| 47 | 4 | %99 (%97–%99) | %100 (%89–%100) | %100 (%89–%100) | 6186 (3716–16281) | %48 (%43–%52) |
| 48 | 4 | %99 (%97–%100) | %100 (%94–%100) | %100 (%94–%100) | 6301 (3780–16669) | %49 (%43–%53) |
| 49 | 4 | %99 (%97–%100) | %100 (%89–%100) | %100 (%89–%100) | 6297 (4201–14247) | %47 (%44–%53) |
| 50 | 4 | %99 (%97–%100) | %100 | %100 | 6595 (3629–14196) | %49 (%42–%53) |
| 51 | 4 | %99 (%97–%100) | %100 | %100 | 6023 (3999–14797) | %49 (%42–%52) |
| 52 | 4 | %99 (%97–%100) | %100 | %100 | 6154 (4456–15176) | %48 (%42–%54) |
| 53 | 4 | %99 (%97–%100) | %100 (%95–%100) | %100 (%95–%100) | 6397 (4465–17562) | %46 (%43–%52) |
| 54 | 4 | %99 (%97–%100) | %100 (%89–%100) | %100 (%89–%100) | 6576 (3414–18636) | %48 (%41–%53) |
| 55 | 4 | %99 (%97–%100) | %100 (%89–%100) | %100 (%89–%100) | 6810 (3431–18416) | %48 (%44–%53) |
| 56 | 4 | %99 (%97–%100) | %100 (%89–%100) | %100 (%89–%100) | 8480 (4164–17058) | %50 (%43–%52) |
| 57 | 4 | %99 (%97–%100) | %100 (%89–%100) | %100 (%89–%100) | 7125 (4106–17819) | %48 (%41–%51) |
| 58 | 4 | %99 (%97–%100) | %100 (%95–%100) | %100 (%95–%100) | 7451 (4555–20159) | %49 (%44–%53) |
| 59 | 4 | %99 (%97–%100) | %100 (%89–%100) | %100 (%89–%100) | 6501 (4464–21903) | %48 (%43–%51) |
| 60 | 4 | %99 (%97–%100) | %100 (%89–%100) | %100 (%89–%100) | 7157 (4606–23086) | %49 (%43–%52) |

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
| 22 | 0 | 11 (9–12,5) |
| 23 | 0 (0–0,5) | 11 (9–12,5) |
| 24 | 0 | 11 (9–13) |
| 25 | 0 | 12 (9,5–13) |
| 26 | 0 | 12 (10–14) |
| 27 | 0 | 13 (10–14) |
| 28 | 0 | 13 (10–15) |
| 29 | 0 | 14 (10,5–15,5) |
| 30 | 0 | 14 (10,5–17) |
| 31 | 0 (0–0,5) | 14 (10,5–17) |
| 32 | 0 | 14,5 (11–17,5) |
| 33 | 0 | 15 (11–17,5) |
| 34 | 0 | 15 (11,5–18) |
| 35 | 0 | 16 (12–18,5) |
| 36 | 0 | 16 (12–19) |
| 37 | 0 | 16,5 (12,5–19,5) |
| 38 | 0 | 16 (12,5–19,5) |
| 39 | 0 | 16,5 (12,5–20) |
| 40 | 0 | 17,5 (13–20) |
| 41 | 0 | 17,5 (13–21) |
| 42 | 0 | 18 (13–21) |
| 43 | 0 | 18 (12,5–22) |
| 44 | 0 | 18 (12,5–22) |
| 45 | 0 | 18 (13–23) |
| 46 | 0 | 18,5 (13–23) |
| 47 | 0 | 19 (12,5–23,5) |
| 48 | 0 | 19 (12,5–24) |
| 49 | 0 | 20 (12,5–24) |
| 50 | 0 | 20 (13,5–25) |
| 51 | 0 | 20 (13,5–25) |
| 52 | 0 | 19,5 (13,5–24,5) |
| 53 | 0 | 20 (13,5–24,5) |
| 54 | 0 | 20,5 (14–24,5) |
| 55 | 0 | 21,5 (13,5–24,5) |
| 56 | 0 | 21 (13,5–25,5) |
| 57 | 0 | 21 (14–25,5) |
| 58 | 0 | 21 (14–26) |
| 59 | 0 | 20,5 (15–26) |
| 60 | 0 | 21 (14,5–26,5) |

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
| 12 | 206 (148–254) | 76 (60–93,5) |
| 13 | 209 (158–234) | 72,5 (59–86,5) |
| 14 | 192 (152–240) | 70 (56,5–92,5) |
| 15 | 200 (156–252) | 68 (51,5–95,5) |
| 16 | 191 (140–252) | 66 (47,5–89,5) |
| 17 | 188 (128–240) | 61,5 (40–87) |
| 18 | 192 (152–242) | 61 (47,5–86,5) |
| 19 | 175 (154–230) | 58 (48,5–70,5) |
| 20 | 176 (150–210) | 61 (49–79) |
| 21 | 152 (132–183) | 52 (44–63) |
| 22 | 183 (139–214) | 64 (47–77) |
| 23 | 184 (128–201) | 53,5 (42–75) |
| 24 | 174 (160–220) | 58 (49–77,5) |
| 25 | 155 (134–208) | 53 (40–71) |
| 26 | 168 (140–200) | 50,5 (37,5–62) |
| 27 | 174 (130–213) | 57,5 (39,5–81) |
| 28 | 152 (130–210) | 45,5 (26,5–71) |
| 29 | 158 (121–220) | 48,5 (31,5–62) |
| 30 | 164 (109–191) | 46,5 (28,5–58) |
| 31 | 149 (108–177) | 43,5 (33–60,5) |
| 32 | 171 (106–207) | 48 (29–69,5) |
| 33 | 146 (126–207) | 48,5 (34,5–64) |
| 34 | 147 (102–208) | 45,5 (30–73) |
| 35 | 170 (106–206) | 53 (31,5–67,5) |
| 36 | 168 (110–211) | 46,5 (36,5–64,5) |
| 37 | 146 (108–216) | 45 (30–67) |
| 38 | 160 (118–210) | 45 (28–63) |
| 39 | 154 (112–219) | 50,5 (28–67) |
| 40 | 165 (148–241) | 56 (39,5–79,5) |
| 41 | 172 (118–230) | 48 (29,5–86) |
| 42 | 160 (124–190) | 45,5 (34–68,5) |
| 43 | 166 (130–260) | 48,5 (33,5–79,5) |
| 44 | 170 (129–221) | 52,5 (25,5–69) |
| 45 | 184 (132–242) | 60 (38,5–81,5) |
| 46 | 178 (113–242) | 51,5 (37–73,5) |
| 47 | 186 (124–202) | 44,5 (33–69) |
| 48 | 198 (124–236) | 52,5 (29,5–74) |
| 49 | 176 (121–242) | 52 (32,5–85,5) |
| 50 | 162 (132–224) | 53,5 (27–87,5) |
| 51 | 183 (114–252) | 54 (34,5–79) |
| 52 | 178 (127–259) | 56,5 (29,5–93) |
| 53 | 163 (105–262) | 47 (27–92,5) |
| 54 | 176 (116–211) | 54 (24,5–68) |
| 55 | 182 (132–236) | 62 (42,5–86) |
| 56 | 190 (138–259) | 62,5 (41,5–79) |
| 57 | 184 (138–292) | 58,5 (35,5–89,5) |
| 58 | 204 (140–260) | 62 (35–87) |
| 59 | 194 (149–271) | 53 (43,5–91) |
| 60 | 194 (142–284) | 61,5 (31,5–92) |

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
| 12 | 10 (6–13) | 0 (0–1,5) | 0 (0–0,5) | 0,5 (0–1,5) | 0,5 (0–1,5) | 0 (0–0,5) |
| 13 | 9 (7–13) | 1 (0–2) | 0 (0–1) | 1 (0–2,5) | 1 (0–2) | 0 (0–0,5) |
| 14 | 8 (5–10,5) | 0 (0–2) | 0 | 1 (0–2,5) | 1 (0–2) | 0 |
| 15 | 7,5 (3–12) | 0,5 (0–2) | 0 | 1 (0–2) | 0 (0–1) | 0 |
| 16 | 7,5 (2–12) | 0,5 (0–2) | 0 (0–0,5) | 1 (0–2) | 1 (0–2,5) | 0 (0–0,5) |
| 17 | 5,5 (2,5–11,5) | 1 (0–2) | 0 (0–1) | 1 (0–2) | 1 (0–1,5) | 0 (0–0,5) |
| 18 | 7 (4–11) | 1 (0–2) | 0 (0–0,5) | 1 (0,5–2,5) | 0,5 (0–2) | 0 (0–0,5) |
| 19 | 7 (4–11) | 0,5 (0–2,5) | 0 (0–0,5) | 1 (0–2,5) | 1 (0–2) | 0 (0–0,5) |
| 20 | 6 (4–9) | 1 (0–3) | 0 (0–1) | 1 (0–3) | 1 (0–1,5) | 0 |
| 21 | 7 (5–9,5) | 1 (0–2,5) | 0 (0–1,5) | 2 (0–3,5) | 1 (0–3,5) | 0 |
| 22 | 7,5 (5,5–10) | 1 (0–3) | 0 (0–1,5) | 2 (0–4,5) | 1 (0,5–2) | 0 (0–0,5) |
| 23 | 7,5 (6–11,5) | 1 (0–2,5) | 0 (0–1) | 2 (0,5–3) | 1 (0–2,5) | 0 (0–1) |
| 24 | 8,5 (5,5–11) | 1 (0–3) | 0 (0–1,5) | 1,5 (0–3) | 1,5 (0–3) | 0 |
| 25 | 8 (4,5–11) | 1 (0–2) | 0 (0–2) | 2 (1–2) | 1,5 (0–3,5) | 0 |
| 26 | 6,5 (3–10) | 1 (0–3) | 1 (0–2) | 2 (0,5–4) | 1 (0–2) | 0 (0–0,5) |
| 27 | 9 (5–14) | 1 (0–3) | 0 (0–0,5) | 2 (0,5–4) | 2 (0,5–3) | 0 (0–1) |
| 28 | 7,5 (4,5–11,5) | 1 (0–2) | 0 (0–1) | 1,5 (0–2,5) | 2 (0–2,5) | 0 (0–0,5) |
| 29 | 7,5 (5–11,5) | 1,5 (1–3,5) | 0 (0–1,5) | 2 (1–4,5) | 1,5 (0–3) | 0 (0–1) |
| 30 | 9 (4,5–12) | 0,5 (0–1,5) | 0 (0–1) | 1 (0–3) | 1 (0–3,5) | 0 (0–0,5) |
| 31 | 7,5 (5,5–10) | 2 (0–4) | 0,5 (0–1,5) | 2 (0–4) | 1 (0–2) | 0 |
| 32 | 8,5 (6–12) | 1 (0–2) | 0 (0–1,5) | 2 (0–4) | 2 (0–3) | 0 (0–1) |
| 33 | 10 (6,5–13) | 2 (0,5–3,5) | 0 (0–1,5) | 2,5 (1–4,5) | 1 (0–4) | 0 (0–1) |
| 34 | 9,5 (6–13,5) | 1 (0–2,5) | 0 (0–1,5) | 1 (0–3,5) | 2 (0–2) | 0 |
| 35 | 11 (7,5–13,5) | 2 (1–3) | 0 (0–2) | 2 (1–3) | 2 (0–4) | 0 (0–0,5) |
| 36 | 9,5 (5–13) | 1 (0–3) | 0 (0–1) | 2 (0,5–4,5) | 1,5 (0–4) | 0 (0–1) |
| 37 | 8,5 (6–11) | 1 (0–3) | 0 (0–2) | 1,5 (0–3,5) | 2,5 (0–4) | 0 (0–1) |
| 38 | 10 (5–12,5) | 2,5 (0,5–4) | 0,5 (0–2) | 3 (1–5) | 1,5 (0–4) | 0 (0–1) |
| 39 | 9 (5,5–14) | 1 (0–3,5) | 0 (0–1,5) | 2 (0–4) | 2,5 (0–3,5) | 0 (0–0,5) |
| 40 | 11,5 (4,5–15,5) | 1 (0–3,5) | 0 (0–1) | 2 (0–4) | 2 (0–4,5) | 0 (0–1) |
| 41 | 9,5 (5,5–14,5) | 1,5 (0–4) | 0 (0–1) | 2 (0–4) | 2,5 (0–5) | 0 (0–1) |
| 42 | 9,5 (6–15) | 2 (0–4) | 0,5 (0–3,5) | 2,5 (0,5–4) | 2 (0–5,5) | 0 |
| 43 | 10 (5,5–16) | 1 (0–3,5) | 0 (0–1) | 2,5 (1–5,5) | 2 (0,5–4) | 0 (0–1) |
| 44 | 9 (4,5–13) | 1 (0–4) | 0 (0–1,5) | 2 (0,5–4) | 2 (0–4) | 0 |
| 45 | 11 (7,5–16,5) | 1,5 (0–4) | 0 (0–1) | 2,5 (0,5–4) | 2 (0–4) | 0 (0–1) |
| 46 | 8,5 (5,5–13) | 1,5 (0–3,5) | 0 (0–1,5) | 2 (0–4) | 2,5 (0–4,5) | 0 (0–1) |
| 47 | 9,5 (5–15,5) | 2 (0–3,5) | 0 (0–2) | 2 (0–4,5) | 1,5 (0–5,5) | 0 (0–1) |
| 48 | 9,5 (7–15,5) | 1,5 (0–4) | 0 (0–1) | 2,5 (0–4,5) | 2 (0,5–4,5) | 0 (0–1) |
| 49 | 9,5 (5–17) | 1 (0–3,5) | 0 (0–1) | 2 (0,5–4) | 2 (0–5) | 0 (0–1) |
| 50 | 10 (3,5–14) | 1 (0–2,5) | 0 (0–1) | 1,5 (0,5–3,5) | 2 (0,5–4,5) | 0 |
| 51 | 11,5 (5,5–15) | 2 (0–4,5) | 0 (0–1,5) | 2,5 (0–5) | 2 (0–6) | 0 (0–1) |
| 52 | 11,5 (5–16) | 1,5 (0–2,5) | 0 (0–1) | 2 (0,5–4) | 2,5 (0–5,5) | 0 (0–0,5) |
| 53 | 9 (4–16) | 2 (0–4) | 0 (0–1,5) | 2 (0–4,5) | 2 (0–6,5) | 0 (0–1) |
| 54 | 11 (3,5–13) | 1,5 (0–3,5) | 0 (0–2) | 2 (0,5–4) | 2,5 (0–4,5) | 0 (0–0,5) |
| 55 | 12 (7–14,5) | 2 (0–4) | 0 (0–1,5) | 2 (0–4,5) | 2 (0–6) | 0 (0–1) |
| 56 | 11 (9–15) | 1,5 (0–2,5) | 0 (0–1) | 2 (0,5–3,5) | 2 (0,5–5,5) | 0 |
| 57 | 10 (5,5–15) | 2 (0–3,5) | 0 (0–0,5) | 2 (0,5–3,5) | 2 (0–5) | 0 (0–0,5) |
| 58 | 11 (6,5–16,5) | 2 (0–3,5) | 0 (0–2) | 2 (0–4) | 2 (0–5,5) | 0 (0–1,5) |
| 59 | 10 (6–13,5) | 1,5 (0–3,5) | 0 (0–2,5) | 2 (0,5–4) | 2 (0,5–4,5) | 0 |
| 60 | 10 (4,5–15) | 1,5 (0–4) | 0,5 (0–2) | 2,5 (1–4) | 1,5 (0–4) | 0 (0–0,5) |

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
| 17 | 0 (0–0,5) | 0 | 0 | 0,5 (0–1,5) |
| 18 | 0 (0–0,5) | 0 | 0 | 0,5 (0–2) |
| 19 | 0 (0–1) | 0 | 0 | 0,5 (0–1,5) |
| 20 | 0 (0–1) | 0 | 0 | 1 (0–1,5) |
| 21 | 0 (0–1) | 0 | 0 | 1 (0–1,5) |
| 22 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 23 | 0 (0–1) | 0 (0–0,5) | 0 | 1 (0–2) |
| 24 | 0 (0–1) | 0 | 0 (0–0,5) | 0,5 (0–2) |
| 25 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 26 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 27 | 0 (0–0,5) | 0 | 0 (0–0,5) | 1 (0–2) |
| 28 | 0 | 0 | 0 | 1 (0–2) |
| 29 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 30 | 0 | 0 | 0 | 1 (0–2) |
| 31 | 0 | 0 | 0 (0–1) | 1 (0–2) |
| 32 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 33 | 0 (0–0,5) | 0 (0–1) | 0 | 1 (0–2) |
| 34 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 35 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 36 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 37 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 38 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 39 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 40 | 0 (0–1) | 0 | 0 (0–0,5) | 1 (0–2) |
| 41 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 42 | 0 (0–1) | 0 (0–1) | 0 | 1 (0–2) |
| 43 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 44 | 0 (0–1) | 0 | 0 (0–1) | 1 (0–2) |
| 45 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 46 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 47 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 48 | 0 (0–1) | 0 | 0 (0–0,5) | 1 (0–2) |
| 49 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 50 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 51 | 0 (0–0,5) | 0 (0–1) | 0 | 0,5 (0–2) |
| 52 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 53 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 54 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 55 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 56 | 0 (0–1) | 0 | 0 | 1 (0–2,5) |
| 57 | 0 (0–0,5) | 0 | 0 | 1 (0–2,5) |
| 58 | 0 (0–0,5) | 0 | 0 (0–0,5) | 1 (0–2) |
| 59 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 60 | 0 (0–0,5) | 0 (0–1) | 0 | 1 (0–2) |

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
| 12 | 9,5 (6,5–11) | 9,5 (7,3–11,1) | 1 (0–2,5) | 2 (0,5–3,5) | 6 (3–8,5) | 0 |
| 13 | 7,5 (4,5–11,5) | 8,33 (5,86–11,3) | 1 (0–2) | 2 (0–5) | 5 (3,5–7) | 0 |
| 14 | 7 (4–10,5) | 6,9 (4,3–11) | 1 (0–3) | 2 (0,5–3,5) | 3,5 (2–5,5) | 0 |
| 15 | 6 (4–10) | 6,1 (4–9,89) | 1,5 (0,5–2,5) | 2 (1–3,5) | 3 (1,5–6) | 0 |
| 16 | 6 (4,5–8,5) | 5,47 (4,09–8,59) | 1,5 (0–3) | 2 (0,5–3,5) | 3,5 (1–5,5) | 0 |
| 17 | 6 (5–8,5) | 6,32 (4,68–7,97) | 2 (0,5–3) | 2 (0–3) | 2,5 (1–5,5) | 0 (0–1) |
| 18 | 7 (5,5–8) | 6,62 (5,45–8,42) | 2 (1–3,5) | 2 (1–2,5) | 2,5 (1–5,5) | 0 (0–1) |
| 19 | 7 (5–8,5) | 6,92 (5,25–7,98) | 2 (1,5–3,5) | 2 (1–4) | 2 (1–5,5) | 0 (0–1) |
| 20 | 7 (5,5–8,5) | 6,9 (6,23–8,26) | 2 (1–3) | 2 (0–3) | 3 (0,5–5) | 0,5 (0–1) |
| 21 | 7 (6–9) | 6,95 (5,72–7,57) | 1,5 (0,5–4) | 1,5 (1–3) | 3 (1,5–4) | 0,5 (0–1) |
| 22 | 8 (7–9) | 7,48 (6,77–8,68) | 2 (1–3) | 1 (0,5–2,5) | 3 (1,5–5) | 0,5 (0–2) |
| 23 | 7 (6–9) | 7,55 (6,62–8,88) | 1 (0–3) | 2 (0–3) | 3 (2–5,5) | 0,5 (0–1,5) |
| 24 | 8 (7–9) | 7,75 (6,82–9,53) | 2 (1,5–3,5) | 2 (1–3,5) | 3 (2–5) | 0,5 (0–1) |
| 25 | 8 (7–8,5) | 8,01 (7,36–9,01) | 2 (1–4,5) | 2 (0–3,5) | 3 (1–5) | 0,5 (0–1) |
| 26 | 8 (6,5–9) | 7,83 (6,96–8,75) | 1 (1–3) | 1 (0–3) | 2 (1–4,5) | 0 (0–1) |
| 27 | 8 (5,5–9,5) | 8,25 (7,01–9,35) | 2 (1–3,5) | 2 (1–3) | 4 (1,5–6,5) | 0 (0–2) |
| 28 | 8 (6–9) | 8,17 (6,37–9,47) | 1 (1–2,5) | 2 (0,5–2,5) | 4 (1,5–5) | 1 (0–1,5) |
| 29 | 8 (6–9,5) | 8,29 (6,56–9,58) | 2 (1–4) | 2 (1–3,5) | 3 (1,5–4,5) | 1 (0–2) |
| 30 | 9 (8–10) | 8,93 (7,38–9,76) | 2 (1–3) | 1 (0,5–3,5) | 4 (2,5–5) | 1 (0–2) |
| 31 | 9 (7–10,5) | 9,34 (7,32–9,89) | 2 (1–2,5) | 2 (1–2,5) | 3,5 (2,5–4,5) | 1 (0–2) |
| 32 | 9,5 (7,5–11) | 9,41 (7,48–10,4) | 2 (0,5–4) | 1 (1–3) | 3,5 (1–4,5) | 1 (0–2) |
| 33 | 9 (8–10) | 9,38 (7,82–10,8) | 1 (0–2) | 1 (0–2) | 4 (2–6,5) | 1 (0–2) |
| 34 | 9 (8–10,5) | 9,54 (8,33–10,5) | 1,5 (0–2,5) | 2 (0–3) | 4 (2,5–5) | 1 (0–2) |
| 35 | 9 (6,5–11) | 9,32 (7,31–10,2) | 2 (0–3,5) | 2 (0,5–4) | 4 (2,5–5,5) | 1 (0–2,5) |
| 36 | 8,5 (8–10,5) | 9,07 (7,28–10,5) | 2 (1–2) | 1,5 (0,5–2,5) | 3 (1–5) | 1 (0–2,5) |
| 37 | 9,5 (8,5–10) | 9,37 (8,33–9,78) | 1,5 (0,5–3,5) | 1,5 (0–3) | 4 (2–5,5) | 1 (0–2) |
| 38 | 10 (8–11) | 9,49 (7,79–10,5) | 2 (0,5–3) | 2 (0–3) | 3 (2–5) | 1 (0–2) |
| 39 | 10 (7–11) | 9,8 (7,2–10,9) | 2 (0–3) | 1,5 (0,5–3,5) | 3 (1,5–6) | 1 (0–2) |
| 40 | 10 (7,5–11) | 9,8 (7,92–10,9) | 2 (0,5–4) | 1,5 (0–4) | 4 (2–7) | 1,5 (0,5–3) |
| 41 | 9 (6,5–10) | 9,73 (6,64–10,7) | 1,5 (0–3) | 2 (1–3,5) | 3,5 (1–5,5) | 1 (0–3) |
| 42 | 10 (7,5–11,5) | 9,6 (7,67–11,1) | 2,5 (1–4,5) | 1 (0,5–3) | 4 (2–7) | 2 (0,5–3) |
| 43 | 10,5 (7,5–11,5) | 10,4 (7,25–11,6) | 2 (1–3) | 1 (0,5–3,5) | 4 (2–7) | 2 (0–3) |
| 44 | 10 (7,5–12,5) | 9,83 (7,88–11,9) | 1,5 (0–3,5) | 2 (0–3) | 3,5 (1,5–5) | 2 (0–3) |
| 45 | 10 (7–12,5) | 10,5 (7,86–12,4) | 2 (1–3,5) | 2 (0,5–4) | 4 (3–6,5) | 2 (0–3) |
| 46 | 10 (8–13) | 9,96 (7,89–12,1) | 2 (0,5–4) | 1,5 (1–3,5) | 3 (2–4) | 2 (0,5–3) |
| 47 | 10 (6,5–11,5) | 9,7 (7,71–12,2) | 1 (0–2) | 2 (1–3,5) | 4 (1–6) | 2 (0–3) |
| 48 | 11 (7,5–13,5) | 10,9 (6,68–12,5) | 2 (1–5) | 1,5 (0,5–3) | 3 (2–6,5) | 3 (0,5–3) |
| 49 | 11,5 (7–13) | 11,2 (7,6–12,9) | 1,5 (0,5–4) | 1 (0–4,5) | 3 (1–7) | 2,5 (0–3) |
| 50 | 11 (9–13) | 11,1 (7,83–13) | 1 (0,5–3,5) | 1,5 (0–3) | 4 (1,5–6,5) | 2,5 (1–3) |
| 51 | 11 (8,5–13) | 10,9 (8,26–12,8) | 2 (0–3) | 2 (0,5–3) | 4 (2–5) | 2 (0,5–3) |
| 52 | 10 (7,5–12) | 11 (8,79–12,5) | 1 (0,5–3,5) | 2 (0–3,5) | 4 (2–6) | 2 (1–3) |
| 53 | 10 (8–12,5) | 10,4 (8,76–12,1) | 1,5 (0,5–4) | 1,5 (1–3) | 3 (2–5) | 2,5 (0,5–3) |
| 54 | 10,5 (8,5–12,5) | 10,6 (8,23–12,5) | 2 (1,5–2,5) | 2 (0–3) | 3,5 (1,5–6) | 2 (0,5–3) |
| 55 | 10 (8,5–13) | 10,3 (8,52–12,6) | 2 (1–3,5) | 2 (1–3,5) | 3,5 (1,5–6,5) | 2 (0–3) |
| 56 | 10 (8–13) | 10,2 (8,88–13) | 2 (0,5–4) | 2 (1–4) | 4 (2,5–6) | 2 (0–3) |
| 57 | 10 (8,5–13) | 10,1 (8,15–12,9) | 2,5 (1–3,5) | 2 (0,5–3) | 3,5 (2–5) | 2,5 (0,5–3) |
| 58 | 10 (7–13) | 10,2 (7,7–13) | 2 (1–3,5) | 3 (1–4) | 4 (1–6,5) | 2 (1,5–3) |
| 59 | 10 (7–13) | 10,2 (7–12,9) | 2 (1–3,5) | 2 (1–4) | 3,5 (1,5–6) | 2 (1,5–3) |
| 60 | 11 (7,5–14) | 10,5 (7,22–13,2) | 2 (1,5–3,5) | 2 (0–4) | 3 (2–5,5) | 2 (1–3) |

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
| 18 | 0 (0–1) | 0 (0–1) | 0,5 (0–1) | 0 |
| 19 | 0 (0–1) | 0 (0–1) | 1 (0–1) | 0 |
| 20 | 0,5 (0–1) | 0 (0–1) | 0 (0–1) | 0 (0–0,5) |
| 21 | 1 (0–1) | 1 (0–1) | 0 (0–1) | 0 |
| 22 | 1 | 1 (1–2) | 0 (0–1) | 0 (0–0,5) |
| 23 | 1 | 1 (0–1) | 0 (0–1) | 0 (0–0,5) |
| 24 | 1 | 1 (0–1,5) | 0 (0–1) | 0 (0–1) |
| 25 | 1 | 1 (0–1) | 0 (0–1) | 0 |
| 26 | 1 | 1 (0–1) | 0 | 0 |
| 27 | 1 | 1 (0–1,5) | 0 (0–1) | 0 (0–1) |
| 28 | 1 | 1 (0–2) | 0 (0–1) | 0 |
| 29 | 1 (0,5–1) | 1 (0–1) | 0 (0–1) | 0 |
| 30 | 1 (0,5–1) | 1 (0–1) | 0,5 (0–1) | 0 |
| 31 | 1 (0,5–1) | 1 (0–2) | 0 (0–1) | 0 |
| 32 | 1 (0–1) | 0,5 (0–1,5) | 0 (0–1) | 0 (0–0,5) |
| 33 | 1 (0–1) | 1 (0–1) | 0 (0–0,5) | 0 |
| 34 | 1 (0–1) | 1 (0–1) | 0 (0–1) | 0 (0–1) |
| 35 | 1 (0–1) | 1 (0–2) | 0 (0–1) | 0 |
| 36 | 1 (0–1) | 1 (0–1) | 0 (0–1) | 0 (0–0,5) |
| 37 | 1 (0–1) | 1 (0–2) | 0 (0–1) | 0 |
| 38 | 1 (0–1) | 0 (0–1) | 0 (0–1) | 0 (0–0,5) |
| 39 | 1 (0–1) | 1 (0–1) | 0 (0–1) | 0 (0–0,5) |
| 40 | 1 (0–1) | 1 (0–1) | 0 (0–1) | 0 (0–0,5) |
| 41 | 1 (0–1) | 0 (0–1) | 0 (0–0,5) | 0 (0–1) |
| 42 | 1 (0–1) | 0,5 (0–1,5) | 0 (0–1) | 0 |
| 43 | 1 (0–1) | 0,5 (0–1,5) | 0 (0–1) | 0 (0–0,5) |
| 44 | 1 (0–1) | 0,5 (0–1) | 0 (0–1) | 0 |
| 45 | 1 (0–1) | 0 (0–1,5) | 0 (0–1) | 0 (0–0,5) |
| 46 | 1 (0–1) | 0 (0–1) | 0 (0–1) | 0 |
| 47 | 1 (0–1) | 0 (0–1) | 0 (0–1) | 0 |
| 48 | 1 (0–1) | 1 (0–1) | 0 (0–1) | 0 |
| 49 | 1 (0–1) | 1 (0–1) | 0 (0–1) | 0 (0–0,5) |
| 50 | 1 (0–1) | 0 (0–1) | 0 (0–1) | 0 |
| 51 | 1 (0–1) | 0 (0–1) | 0 (0–1) | 0 (0–1) |
| 52 | 1 (0–1) | 1 (0–2) | 0 (0–1) | 0 (0–1) |
| 53 | 1 (0–1) | 0 (0–1) | 0 (0–1) | 0 (0–0,5) |
| 54 | 1 (0–1) | 1 (0–1) | 0 (0–1) | 0 (0–0,5) |
| 55 | 1 (0–1) | 0 (0–1) | 0 (0–1) | 0 (0–0,5) |
| 56 | 0,5 (0–1) | 0 (0–1) | 0 (0–1) | 0 (0–0,5) |
| 57 | 0,5 (0–1) | 0 (0–1,5) | 0 (0–1) | 0 |
| 58 | 0,5 (0–1) | 0 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 59 | 0,5 (0–1) | 0,5 (0–1) | 0 (0–1) | 0 (0–0,5) |
| 60 | 0,5 (0–1) | 0 (0–1) | 0 (0–1) | 0 |

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
| 13 | 3 (0–5,5) | 1 (0–2,5) | 0 | 0 (0–0,5) | 0 | 0 |
| 14 | 2 (1–6) | 1 (0–2,5) | 0 | 0 | 0 | 0 |
| 15 | 1 (0–4,5) | 1 (0–2,5) | 0 | 0 | 0 | 0 (0–0,5) |
| 16 | 2 (1–5) | 0,5 (0–2) | 0 | 0 | 0 | 0 (0–0,5) |
| 17 | 2 (1–4) | 0 (0–1,5) | 0 | 0 | 0 | 0 (0–0,5) |
| 18 | 4 (1,5–6,5) | 0 (0–2,5) | 0 | 0 | 0 | 0 |
| 19 | 2,5 (1–4,5) | 0,5 (0–1) | 0 | 0 | 0 | 0 (0–1) |
| 20 | 2 (1–4) | 0,5 (0–2,5) | 0 | 0 | 0 | 0 (0–1) |
| 21 | 3 (1,5–6,5) | 1 (0–1,5) | 0 | 0 | 0 | 0 |
| 22 | 3 (2–7) | 0 (0–3) | 0 | 0 | 0 | 0 |
| 23 | 4 (2–7) | 1 (0–3) | 0 | 0 | 0 | 0 |
| 24 | 3,5 (1–6) | 2 (0–5) | 0 | 0 (0–0,5) | 0 | 0 (0–0,5) |
| 25 | 3 (1–6,5) | 1,5 (0–4) | 0 | 0 | 0 | 0 (0–1) |
| 26 | 4,5 (2,5–6,5) | 0 (0–2) | 0 | 0 (0–1) | 0 | 0 (0–1) |
| 27 | 4 (1,5–5) | 2 (0–4,5) | 0 | 0 (0–1) | 0 | 0 (0–1) |
| 28 | 3 (1,5–5) | 0 (0–4) | 0 (0–0,5) | 0 (0–0,5) | 0 | 0 (0–1) |
| 29 | 3,5 (0,5–7) | 1 (0–3,5) | 0 | 0 (0–1) | 0 | 0 (0–1) |
| 30 | 3 (1–7,5) | 1 (0–2,5) | 0 | 0 (0–0,5) | 0 | 0 (0–0,5) |
| 31 | 3 (1,5–5) | 0 (0–2) | 0 | 0 (0–1) | 0 | 0 (0–1) |
| 32 | 3 (1–4,5) | 1 (0–3,5) | 0 (0–1) | 0 (0–1) | 0 | 0 (0–1) |
| 33 | 3,5 (1,5–7) | 1 (0–5) | 0 (0–1) | 0 (0–1,5) | 0 | 0 (0–1) |
| 34 | 4 (2–6) | 1 (0–5,5) | 0 (0–0,5) | 0,5 (0–1) | 0 | 0 (0–1) |
| 35 | 3 (1–6) | 1,5 (0–4,5) | 0 (0–0,5) | 0 (0–1,5) | 0 | 0 (0–0,5) |
| 36 | 4 (0,5–6) | 1 (0–3,5) | 0 (0–1) | 0,5 (0–1) | 0 | 0 (0–1) |
| 37 | 5 (1–7) | 0 (0–3) | 0 (0–1) | 0,5 (0–1) | 0 | 0 (0–1) |
| 38 | 5 (2–7) | 0,5 (0–3,5) | 0 (0–1) | 0 (0–1,5) | 0 | 0 (0–1) |
| 39 | 4 (1,5–6) | 0,5 (0–5,5) | 0 (0–1) | 0 (0–1) | 0 | 0 (0–1) |
| 40 | 3,5 (1,5–5,5) | 0,5 (0–5,5) | 0 (0–1,5) | 0,5 (0–2) | 0 | 0 (0–1) |
| 41 | 3 (1–6,5) | 0 (0–6,5) | 0 (0–1) | 0 (0–1) | 0 | 0 (0–1,5) |
| 42 | 2 (1–8) | 0,5 (0–3) | 0 (0–2) | 0 (0–1,5) | 0 | 0 (0–1) |
| 43 | 2,5 (1,5–6,5) | 1 (0,5–3) | 1 (0–2) | 0 (0–2) | 0 | 0 (0–2) |
| 44 | 4 (2,5–6) | 1 (0–4,5) | 1 (0–2) | 0,5 (0–1,5) | 0 | 0 (0–1,5) |
| 45 | 3 (1–6) | 1 (0–5) | 0 (0–2) | 0 (0–2) | 0 | 0 (0–2) |
| 46 | 3,5 (0,5–7) | 1,5 (0–5) | 0 (0–2) | 0 (0–3) | 0 | 0 (0–2,5) |
| 47 | 4,5 (3–6) | 1 (0–3) | 1 (0–2) | 0 (0–1) | 0 | 0 (0–1) |
| 48 | 3,5 (1–6,5) | 1 (0–3,5) | 1 (0–2) | 1 (0–1,5) | 0 | 0 (0–1) |
| 49 | 3 (0,5–5,5) | 1,5 (0–5) | 1 (0–2) | 0 (0–3) | 0 | 0 (0–1) |
| 50 | 3 (0,5–5) | 1 (0–5,5) | 1 (0–2,5) | 0,5 (0–3,5) | 0 | 0 (0–1) |
| 51 | 3,5 (2–5,5) | 2 (0–3,5) | 0 (0–2) | 0 (0–3) | 0 | 1 (0–3) |
| 52 | 3 (1,5–4) | 2 (0–7) | 1 (0–2) | 0,5 (0–3) | 0 | 0 (0–1) |
| 53 | 4,5 (1–6) | 1 (0–5,5) | 0,5 (0–3) | 0 (0–2,5) | 0 | 0 (0–1) |
| 54 | 4 (2,5–7,5) | 1 (0–3) | 1 (0–1,5) | 1 (0–3) | 0 | 0 |
| 55 | 4 (1,5–6) | 2,5 (0,5–5,5) | 1 (0–2,5) | 0 (0–2) | 0 | 0 (0–1) |
| 56 | 5 (2–8,5) | 4,5 (0–5) | 1 (0–2) | 0 (0–2) | 0 | 0 (0–1) |
| 57 | 6 (3–7,5) | 1 (0–5) | 0,5 (0–2) | 0 (0–1) | 0 | 0 (0–1) |
| 58 | 3 (1–7) | 2,5 (1–6) | 1 (0–2) | 1 (0–4) | 0 | 0 (0–1,5) |
| 59 | 4 (0,5–5,5) | 1 (0–6) | 1 (0–3) | 0 (0–2) | 0 | 0 (0–1) |
| 60 | 4,5 (1–8,5) | 2 (0–6) | 0 (0–1,5) | 1 (0–1,5) | 0 | 1 (0–1,5) |

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
| 12 | 17 (10,5–23,5) | 1,24 (1–1,76) | 1,5 (1–2) | 1,66 (1,4–2,05) | 3 (2–4) |
| 13 | 17 (10,5–27) | 1,33 (1–1,63) | 1 (1–2) | 1,76 (1,64–2,24) | 3,5 (3–4) |
| 14 | 20,5 (13–29) | 1,25 (1–1,8) | 1 (1–2) | 1,95 (1,57–2,42) | 4 (3–4,5) |
| 15 | 22,5 (12,5–29) | 1,33 (1–1,75) | 1 (1–1,55) | 2,14 (1,75–2,58) | 4 (3–5) |
| 16 | 22,5 (14,5–31,5) | 1 (1–1,5) | 1,5 (1–2,15) | 2,14 (1,76–2,7) | 4 (3,5–5) |
| 17 | 25 (17–32,5) | 1 (1–1,6) | 2 (1,2–2) | 2,26 (1,89–2,7) | 4,5 (3,5–5) |
| 18 | 28,5 (20,5–36) | 1,33 (1–1,62) | 2,5 (1,08–2,87) | 2,28 (1,88–2,81) | 5 (3,5–5,5) |
| 19 | 31,5 (21,5–37) | 1,33 (1–1,75) | 2 (1–2) | 2,37 (2–2,75) | 5 (3,5–6) |
| 20 | 31,5 (23,5–39) | 1,33 (1–1,8) | 1,83 (1–2,3) | 2,42 (2,17–2,73) | 5 (3,5–6) |
| 21 | 34,5 (24,5–42,5) | 1,33 (1–1,67) | 2 (1–3,4) | 2,47 (2,13–2,8) | 5 (4–6) |
| 22 | 38 (25,5–46) | 1,32 (1–1,56) | 2,29 (1,4–3,2) | 2,52 (2,11–2,78) | 5 (4–6) |
| 23 | 41,5 (27,5–48,5) | 1,29 (1–1,5) | 2 (1–4,4) | 2,52 (2,21–2,71) | 5 (4–6,5) |
| 24 | 43,5 (30–51) | 1,33 (1–1,55) | 2 (1–3,71) | 2,59 (2,31–2,79) | 5 (4,5–6,5) |
| 25 | 45 (31,5–54,5) | 1,38 (1–1,62) | 2,25 (1–2,6) | 2,66 (2,3–2,87) | 6 (4,5–6,5) |
| 26 | 49,5 (34,5–56) | 1,4 (1–1,67) | 2 (1,3–3,43) | 2,61 (2,38–2,87) | 6 (5–6,5) |
| 27 | 49,5 (36,5–57) | 1,29 (1–1,5) | 2 (1,1–3) | 2,7 (2,45–2,96) | 6 (5–7) |
| 28 | 51 (37,5–59,5) | 1,33 (1–1,87) | 1,63 (1–1,92) | 2,78 (2,44–3,04) | 6 (5–7) |
| 29 | 51 (43–63) | 1,47 (1–1,67) | 2 (1–3) | 2,83 (2,48–3,12) | 6,5 (5–7,5) |
| 30 | 52,5 (46–64) | 1,33 (1–1,67) | 1,5 (1–2,2) | 2,92 (2,39–3,16) | 7 (5–7,5) |
| 31 | 55 (48–66,5) | 1,29 (1–1,5) | 1 (1–2,57) | 3 (2,45–3,21) | 7 (5–8) |
| 32 | 56 (50–68,5) | 1,17 (1–1,67) | 1,75 (1–2,73) | 2,88 (2,54–3,24) | 7 (5,5–8,5) |
| 33 | 58 (51–67) | 1,37 (1–1,6) | 2,17 (1–4) | 2,93 (2,52–3,38) | 7 (6–8,5) |
| 34 | 61 (51,5–69) | 1,29 (1–1,73) | 2,33 (1–3,68) | 2,91 (2,56–3,47) | 7 (6–8,5) |
| 35 | 61,5 (51–69,5) | 1,42 (1–2) | 2,17 (1,37–3) | 2,93 (2,68–3,66) | 7 (6–8,5) |
| 36 | 63 (50–73,5) | 1,23 (1–1,71) | 2,33 (1,2–3,3) | 2,94 (2,77–3,62) | 7 (6–8,5) |
| 37 | 66,5 (51,5–76) | 1,35 (1,17–2) | 3,57 (1,6–5,8) | 2,94 (2,69–3,54) | 7,5 (6–9) |
| 38 | 69,5 (56–75,5) | 1,24 (1–1,58) | 2 (1,23–3,4) | 2,99 (2,67–3,57) | 7,5 (6–9) |
| 39 | 71 (59–76) | 1,37 (1–1,86) | 2,38 (2–3,76) | 2,98 (2,69–3,6) | 8 (6–9) |
| 40 | 68 (59,5–77,5) | 1,58 (1–2) | 3 (1,63–3,83) | 3,1 (2,65–3,64) | 8 (6,5–9) |
| 41 | 72,5 (60–76,5) | 1,5 (1–1,83) | 3,25 (1,93–4,43) | 3,12 (2,67–3,78) | 8 (7–9,5) |
| 42 | 73,5 (59,5–78,5) | 1,27 (1–2) | 2,25 (2–5,6) | 3,17 (2,59–3,84) | 8,5 (7–9,5) |
| 43 | 73 (58–82) | 1,5 (1–1,73) | 2,17 (1,65–3) | 3,17 (2,62–3,88) | 8,5 (7–9,5) |
| 44 | 71 (58,5–86) | 1,33 (1–1,74) | 2 (1,6–3,16) | 3,28 (2,63–3,85) | 8,5 (7–9,5) |
| 45 | 71,5 (58–85,5) | 1,33 (1–1,79) | 2 (1,9–3,65) | 3,33 (2,64–3,85) | 8,5 (7–10) |
| 46 | 71,5 (58–84,5) | 1,46 (1,04–1,93) | 2,5 (1,5–4) | 3,43 (2,69–3,99) | 8,5 (7,5–10) |
| 47 | 75,5 (58,5–85,5) | 1,33 (1–1,69) | 2,67 (2–5,8) | 3,35 (2,74–3,8) | 9 (7,5–10) |
| 48 | 76 (59,5–85,5) | 1,29 (1–1,76) | 2,67 (2–4) | 3,38 (2,77–3,78) | 9 (7,5–10) |
| 49 | 76,5 (58–85,5) | 1,45 (1–1,67) | 3 (1,9–4,1) | 3,41 (2,77–3,96) | 9,5 (8–10) |
| 50 | 71 (57–88) | 1,63 (1,2–2) | 4 (2,3–5,08) | 3,34 (2,82–3,95) | 9,5 (8–10) |
| 51 | 72 (57,5–90,5) | 1,45 (1–1,83) | 2 (1,6–3,93) | 3,5 (2,93–3,96) | 9,5 (8–10) |
| 52 | 69,5 (56,5–83) | 1,33 (1–1,87) | 3 (2,03–4,98) | 3,5 (2,93–4,01) | 9,5 (8–10) |
| 53 | 70,5 (57–83) | 1,41 (1–1,67) | 3 (2–4,6) | 3,5 (2,93–3,93) | 9,5 (8–10) |
| 54 | 71 (58,5–84) | 1,25 (1–1,57) | 3,5 (2,07–5,07) | 3,42 (2,99–3,86) | 9,5 (8–10) |
| 55 | 72 (58–83,5) | 1,4 (1–2,1) | 3 (1,68–4,8) | 3,41 (2,97–3,81) | 9,5 (8–10) |
| 56 | 71 (61,5–87,5) | 1,33 (1–1,64) | 2,8 (2–3,72) | 3,4 (2,85–3,79) | 9,5 (8–10) |
| 57 | 72,5 (62,5–90,5) | 1,54 (1,17–1,83) | 2,25 (1–4) | 3,41 (2,84–3,76) | 9,5 (8,5–10) |
| 58 | 71 (63–90,5) | 1,56 (1,25–2) | 3 (2,13–4,7) | 3,48 (3–3,83) | 10 (8,5–10) |
| 59 | 72 (61,5–91) | 1,37 (1–1,93) | 2,6 (1,8–3,32) | 3,53 (3,02–3,96) | 10 (8,5–10) |
| 60 | 73 (60–89) | 1,5 (1,08–2) | 3,5 (2–5) | 3,48 (3,04–4,02) | 10 (8,5–10) |

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
| 12 | 3 (2–4) | 2,5 (0,5–7,5) | 1 (0–2,5) | 30 (17–53,5) | 0 (0–3,5) |
| 13 | 3 (2–4) | 2,5 (0,5–5,5) | 1,5 (0–3) | 36 (18,5–57,5) | 2,5 (0–5) |
| 14 | 3 (2,5–4,5) | 2,5 (1–5,5) | 1 (0–1,5) | 40 (21,5–61) | 5 (0–9) |
| 15 | 3,5 (3–5) | 2 (1–4) | 0,5 (0–1) | 40 (25,5–64,5) | 5 (1,5–11,5) |
| 16 | 4 (3–5) | 2 (1–3) | 0 (0–1) | 42 (29–70,5) | 7,5 (3,5–14,5) |
| 17 | 5 (4–5) | 1 (1–4) | 0 (0–1,5) | 43,5 (29–71,5) | 10 (6–15) |
| 18 | 4,5 (3,5–5,5) | 2 (1–7) | 1 (0–1,5) | 44,5 (29,5–76,5) | 10,5 (8–18,5) |
| 19 | 4 (3–6) | 2,5 (1,5–5) | 0,5 (0–1,5) | 49,5 (33,5–78) | 11,5 (9–19,5) |
| 20 | 5 (4–6) | 3 (0,5–6) | 0 (0–1,5) | 54 (34–78,5) | 13,5 (9,5–20,5) |
| 21 | 5 (4–6) | 2 (0–4) | 0 (0–2) | 59 (34–84,5) | 15 (10,5–19,5) |
| 22 | 5 (4–6) | 4 (1–6,5) | 0 (0–1) | 58 (37–83) | 16,5 (10–20,5) |
| 23 | 5 (4,5–6) | 3,5 (1–5,5) | 0 (0–1,5) | 65,5 (39–88) | 16,5 (12,5–22) |
| 24 | 5,5 (4,5–6) | 3,5 (1–6) | 0,5 (0–3,5) | 70 (39–86) | 17,5 (14,5–22) |
| 25 | 5,5 (5–6) | 3 (2–5) | 0 (0–1,5) | 72,5 (41,5–92,5) | 20 (14–24) |
| 26 | 5,5 (5–6) | 3 (0,5–7) | 0,5 (0–1,5) | 73 (41–90,5) | 20 (15–27,5) |
| 27 | 5 (5–6) | 3 (1,5–6) | 0 (0–3) | 75 (43,5–98) | 20 (14–28) |
| 28 | 5 (5–6) | 3 (1–5) | 0 (0–1) | 79 (43,5–99,5) | 22 (15–31) |
| 29 | 5 (5–6) | 3 (1–4,5) | 1 (0–2,5) | 81,5 (43–100) | 23 (16,5–30) |
| 30 | 5 (5–6) | 3 (1–7) | 0 (0–1) | 83 (42,5–109) | 21 (15,5–34) |
| 31 | 5 (5–6) | 2,5 (0,5–5,5) | 0,5 (0–1) | 80 (46,5–114) | 21,5 (14–34,5) |
| 32 | 5 (4,5–6) | 3 (1,5–5) | 0,5 (0–1) | 80 (48,5–114) | 21,5 (15,5–38,5) |
| 33 | 5 (5–6) | 3 (1,5–4,5) | 0 (0–1) | 82 (49–114) | 25,5 (17–40) |
| 34 | 5 (5–6) | 2,5 (1–6) | 1 (0–2,5) | 82 (50–119) | 25,5 (16–38) |
| 35 | 5 (5–6) | 2 (1–4,5) | 0,5 (0–2,5) | 82 (50,5–122) | 24 (17,5–38) |
| 36 | 5 (5–6) | 2,5 (1–7) | 1 (0–2) | 83 (51,5–114) | 24 (16–39,5) |
| 37 | 5 (5–6) | 3 (1–6) | 0,5 (0–3) | 84,5 (56–125) | 26,5 (18–41) |
| 38 | 5 (5–6) | 2,5 (1–5) | 1 (0–2) | 81,5 (55–122) | 27,5 (17,5–40) |
| 39 | 5 (5–6) | 3,5 (1–5,5) | 1 (0–2) | 86 (56–126) | 28 (16–39) |
| 40 | 5 (5–6) | 2 (0–7,5) | 0 (0–3) | 85,5 (59–122) | 26,5 (18–39) |
| 41 | 5 (5–6) | 2 (1–5,5) | 0,5 (0–2) | 86 (56–124) | 27 (18–41) |
| 42 | 5 (5–6) | 4 (1–7) | 1 (0–2,5) | 88 (58–130) | 32,5 (20,5–42,5) |
| 43 | 5 (5–6) | 3 (0–5,5) | 0,5 (0–3) | 84,5 (59,5–136) | 29,5 (21–44) |
| 44 | 5,5 (5–6) | 3 (2–6,5) | 1 (0–3) | 85,5 (62–136) | 29,5 (19,5–44) |
| 45 | 6 (5–6) | 3 (1,5–5) | 1 (0–3) | 90,5 (59–131) | 30,5 (18,5–43,5) |
| 46 | 6 (5–6) | 3 (1,5–5,5) | 1 (0–2,5) | 97 (63,5–134) | 30,5 (19–45) |
| 47 | 6 (5–6) | 3 (1,5–4) | 1 (0–2) | 94,5 (61,5–141) | 30 (20,5–42,5) |
| 48 | 5,5 (5–6) | 3 (1–5,5) | 1 (0–3) | 95,5 (62,5–141) | 31 (20,5–45,5) |
| 49 | 5,5 (4,5–6) | 3 (0,5–6) | 1 (0–3,5) | 98,5 (60,5–146) | 30,5 (21,5–45) |
| 50 | 5,5 (5–6) | 3 (2–5,5) | 0,5 (0–2) | 103 (58,5–146) | 30,5 (21,5–52,5) |
| 51 | 5,5 (5–6) | 3 (1–4) | 0 (0–3) | 107 (60–144) | 30,5 (19–54) |
| 52 | 6 (5–6) | 2 (0,5–6,5) | 0,5 (0–4) | 104 (58–150) | 31 (19,5–53,5) |
| 53 | 6 (5–6) | 3 (1–5,5) | 0 (0–2,5) | 99 (63–161) | 31 (20,5–50,5) |
| 54 | 6 (5–6) | 3 (1–6) | 0,5 (0–1) | 97,5 (65–169) | 30 (19,5–48) |
| 55 | 6 (5–6) | 3 (1,5–8,5) | 1 (0–3,5) | 102 (66,5–173) | 33,5 (21–47) |
| 56 | 5 (5–6) | 3,5 (1–5,5) | 1 (0–4) | 112 (65–170) | 33 (20,5–49,5) |
| 57 | 5,5 (5–6) | 3 (1–8) | 1 (0–4,5) | 102 (67–164) | 31,5 (22–49,5) |
| 58 | 6 (5–6,5) | 4 (1–8,5) | 1,5 (0,5–2,5) | 96,5 (64,5–164) | 35,5 (20–52) |
| 59 | 6 (5–6,5) | 2,5 (1–6,5) | 1 (0,5–3,5) | 101 (67–158) | 35,5 (21,5–52,5) |
| 60 | 6 (5–6) | 2 (1,5–6,5) | 1,5 (0–3) | 97,5 (66,5–164) | 30,5 (20–52) |

