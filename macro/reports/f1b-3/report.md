# Ölçüm raporu: f1b-3

16 dünya (seed 1-16) × 60 yıl (7200 gün) · 2026-10-01 17:45 · `FD.Macro.Run stats --seeds 1-16 --years 60 --jobs 2 --verify 1 --saveload 1`

Süre: 7 dk 53 sn duvar saati, 2 iş parçacığı; dünya başına 46,9 sn (en az 39,1, en çok 70,5; yıl sonu hash'leri dâhil).

## Bitiş ölçütleri

DESIGN-FAZ1.md, "Bitiş ölçütleri". ✓ geçti · ✗ kaldı · — ölçülemedi.

| # | Ölçüt | Koşul | Ölçülen | Sonuç |
|---|---|---|---|---|
| 1 | Donma yok | 41–60. yılların yıllık büyük olay medyanı ≥ 0,8 × 6–20. yılların medyanı | 53 / 44 = 1,2 kat | ✓ |
| 2 | Çöküş | dünyaların ≥ %75'inde 60 yılda ≥ 1 çöküş (yok olma ya da başkent kaybı) | %94 (15/16 dünya); toplam 130 çöküş: 17 yok olma, 113 başkent kaybı | ✓ |
| 3 | Kamplar | 41–60. yıllarda yaşayan kamp medyanı ≥ 6–20. yılların medyanı | 10,2 ≥ 8,16 (yıl sonu sayımıyla 10 / 8) | ✓ |
| 4a | Kahraman: doğuş seviyesi | her on yılda doğanların ortalama seviyesi ≤ 2 | 1,3 · 1,3 · 1,28 · 1,36 · 1,43 · 1,43 (on yıllar sırasıyla) | ✓ |
| 4b | Kahraman: Sv8+ | dünyaların ≥ yarısında en az bir kahraman Sv8 ve üstüne çıkar | %94 (15/16 dünya); dünyadaki en yüksek seviye: medyan Sv9, en çok Sv10 | ✓ |
| 4c | Kahraman: efsane | dünya başına efsane medyanı 1–6 | medyan 5,5 (p10–p90: 3–11; toplam 99) | ✓ |
| 4d | Kahraman: ölüm payı | doğan kahramanların %30–80'i ölür | %39 (1157/2942); dünya medyanı %39 (%29–%49) | ✓ |
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

Eski analiz: TS v0.23, 12 seed × 30 yıl ve 3 seed × 60 yıl (Proje: `analiz-5-ajan-oneriler.md`). "Sürüyor mu" kaba bir eşiktir: araştırma ağacı Faz 1b-3'te kaldırıldı; 30. yılda tam 5 kara yerleşimli medeniyet ≥ %50; kamp (30. yıl) < 0,75 × en yüksek yıl; altın (30. yıl) ≥ 10 × altın (1. yıl); boştaki iş gücü (30. yıl) ≥ %30; büyük olay (30. yıl) ≤ 0,6 × en yüksek yıl; 25. yıldan sonra doğanların ≥ %50'si Sv5+; hiç başkent kaybı yok.

| Bulgu | Eski analiz | Bu ölçüm | Sürüyor mu? |
|---|---|---|---|
| Araştırma ağacı erken bitiyor | ~19. yılda bitiyor; 30. yılda medeniyetlerin %98'i bitirmiş | ağaç ve çağlar kaldırıldı (Faz 1b-3); başlangıç medeniyetlerinin ilk kasabası medyan 9,5. yılda (128/128), ilk şehri 22. yılda (122/128); 30. yılda başkent kademesi ortalaması 2,69 | hayır |
| Medeniyetler 5 yerleşimde takılıyor | 98 medeniyetin 74'ü (%76) tam 5 yerleşimde | tam 5 kara yerleşimli medeniyet payı 30. yılda %0,8, 60. yılda %0,8 (denizaşırı koloniler dâhil 30. yılda tam 5: %0, 5+: %96); medeniyet başına 8,69 yerleşim (30. yıl) | hayır |
| Kamp sayısı düşüyor | 6,8'den 3,5'e iniyor | 3 (1. yıl) → en yüksek 11,5 (58. yıl) → 8 (30. yıl) → 11 (60. yıl); yıl sonu, yıllık dünya medyanı | evet |
| Altın birikiyor | altın medyanı 78'den 6.503'e çıkıyor | 8,32 (1. yıl) → 488 (30. yıl) → 694 (60. yıl) | evet |
| İş gücü boşta | iş gücünün %43'ü boşta | 30. yılda %21, 60. yılda %17 (işe yerleşemeyen `zanaatçı` / bütün iş gücü, askerler dâhil; yıl içi ortalama) | hayır |
| Büyük olaylar seyreliyor | yıllık büyük olay 59'dan 28'e düşüyor | en yüksek 62,5 (47. yıl) → 44,5 (30. yıl) → 56 (60. yıl), yıllık dünya medyanı | hayır |
| Doğuş seviyesi şişiyor | 24. yıldan sonra herkes Sv5 doğuyor; efsane mekaniği ölü | 25–60. yıllarda Sv5+ doğanların payı %0; on yıllık doğuş seviyesi ortalaması 1,3 · 1,3 · 1,28 · 1,36 · 1,43 · 1,43 | hayır |
| Başkent düşmüyor | başkent fethedilemiyor (agents.ts:673) | 16 dünyada 113 başkent kaybı, 17 yok olma; 820 yerleşim fethi | hayır |

## On yıllık özet

Hücre: dünyalar arası medyan (p10–p90). Her dünyada on yılın yıllık değerlerinin ortalaması alınır: akış ölçülerinde yıllık ortalama, stok ölçülerinde yıl sonu değerlerinin ortalaması. Yüzdeler 0–1 paylardır.

| Ölçü | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| **Medeniyet** | | | | | | |
| Yaşayan medeniyet | 8 (7–9) | 8 (7–9) | 8 (7–9,2) | 8 (7–9,35) | 8 (7–9) | 8 (7–9,35) |
| Yeni medeniyet (yeniden doğan) | 0 | 0 (0–0,05) | 0 (0–0,1) | 0 (0–0,05) | 0 (0–0,1) | 0 (0–0,1) |
| Yok olan medeniyet | 0 | 0 | 0 (0–0,1) | 0 | 0 (0–0,1) | 0 (0–0,1) |
| Başkent kaybı (medeniyet yaşarken) | 0 (0–0,05) | 0,15 (0–0,25) | 0,1 (0–0,3) | 0,1 (0–0,35) | 0,1 (0–0,3) | 0,1 (0–0,3) |
| Çöküş (yok olma + başkent kaybı) | 0 (0–0,05) | 0,15 (0–0,3) | 0,1 (0–0,45) | 0,15 (0–0,35) | 0,15 (0–0,35) | 0,1 (0–0,4) |
| Yaşayan yerleşim | 21,6 (18,6–25,1) | 51,4 (40,7–57,3) | 66,2 (54,1–73,4) | 72,8 (61,4–83,5) | 75,4 (66,3–91,5) | 78,1 (68,2–97,5) |
| Medeniyet başına yerleşim | 2,71 (2,6–2,87) | 6,29 (5,81–6,54) | 8,06 (7,7–8,28) | 9,18 (8,2–9,81) | 9,55 (8,51–10,6) | 10,2 (8,94–11,3) |
| 5+ kara yerleşimli medeniyet payı | %16 (%11–%18) | %85 (%75–%93) | %96 (%87–%100) | %99 (%84–%100) | %95 (%81–%100) | %93 (%82–%100) |
| Kurulan yerleşim | 2,95 (2,4–3,45) | 2,15 (1,65–2,45) | 1,1 (0,8–1,35) | 0,7 (0,4–1,05) | 0,65 (0,3–1) | 0,6 (0,05–0,95) |
| Fethedilen yerleşim | 0,2 (0–0,4) | 0,7 (0,35–1,25) | 1,05 (0,55–1,7) | 1,05 (0,45–1,7) | 1 (0,65–1,55) | 1,2 (0,2–1,65) |
| Terk edilen yerleşim | 0 | 0 (0–0,2) | 0 (0–0,15) | 0 (0–0,2) | 0,2 (0–0,35) | 0,2 (0–0,75) |
| Toplam nüfus | 391 (314–462) | 1629 (1284–1868) | 2578 (2107–2967) | 3254 (2662–3626) | 3536 (3013–4110) | 3661 (3140–4453) |
| Altın medyanı (medeniyetler) | 44,1 (26,4–61,3) | 201 (177–334) | 398 (269–479) | 550 (380–626) | 638 (402–754) | 731 (476–964) |
| Boştaki iş gücü payı | %0,7 (%0,2–%1,7) | %13 (%11–%15) | %21 (%16–%25) | %20 (%16–%22) | %17 (%16–%20) | %17 (%14–%20) |
| Bölünme (ayrılıp kurulan medeniyet) | 0 | 0 (0–0,05) | 0 (0–0,1) | 0 (0–0,05) | 0 (0–0,1) | 0 (0–0,1) |
| En büyük medeniyetin yerleşimi | 3,7 (3,4–3,9) | 8,45 (7,8–9,65) | 11 (9,4–13,4) | 14,1 (10,4–17) | 15,3 (11,4–20,3) | 16,9 (13,9–24,1) |
| **Olaylar** | | | | | | |
| Olay | 101 (81,2–118) | 169 (130–202) | 166 (135–190) | 163 (114–195) | 170 (134–214) | 173 (149–227) |
| Büyük olay | 32,6 (24,9–38,9) | 50,2 (35,6–57,2) | 49,6 (37,5–56,8) | 49,9 (35,2–61,6) | 51,8 (39,8–64,3) | 54,9 (45,6–70,5) |
| **Savaş** | | | | | | |
| Muharebe | 4,85 (3,8–6,2) | 7,85 (5,7–9,25) | 8,4 (5,9–10,2) | 9,45 (7,35–11,2) | 9,1 (6,85–11,1) | 9,1 (7,15–11,6) |
| Başlayan savaş | 0,2 (0–0,45) | 0,8 (0,35–1,55) | 1,2 (0,85–2,25) | 1,25 (0,75–2,55) | 1,45 (0,85–2,1) | 1,6 (0,25–2,2) |
| Süren savaş (yıl sonu) | 0 (0–0,15) | 0,2 (0–0,6) | 0,3 (0,2–0,55) | 0,4 (0,05–1,15) | 0,45 (0,1–0,85) | 0,35 (0,1–0,6) |
| Yıl içinde süren savaş | 0,2 (0–0,55) | 1 (0,4–1,7) | 1,6 (1,05–2,85) | 1,5 (1–3,5) | 1,85 (1–2,8) | 2 (0,35–2,65) |
| Yağma akını (medeniyet) | 0,05 (0–0,65) | 0,65 (0–1,7) | 1,4 (0,2–2,6) | 1,85 (0–2,5) | 1,55 (0,05–2,4) | 1,5 (0–3,1) |
| Tarihî hak savaşı | 0 (0–0,05) | 0,1 (0–0,15) | 0,2 (0–0,35) | 0,2 (0–0,55) | 0 (0–0,2) | 0 (0–0,3) |
| Pakt gereği savaş | 0 | 0 (0–0,15) | 0,1 (0–0,5) | 0,1 (0–0,6) | 0,2 (0–0,45) | 0,25 (0–0,55) |
| Kutsal Sefer çağrısı | 0 (0–0,05) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0,05 (0–0,1) | 0 (0–0,1) |
| İhanet (pakt çiğnendi) | 0 | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) |
| Savunma paktı (yıl sonu) | 0 | 0,4 (0–1,25) | 1 (0–1,75) | 1 (0–2,25) | 1,3 (0,2–2,55) | 1,75 (0,5–2,5) |
| **Canavarlar** | | | | | | |
| Yaşayan kamp (yıl sonu) | 7,45 (6,5–8,4) | 7,1 (6,25–9,2) | 7,85 (7–9) | 8,55 (7,35–10,1) | 9,75 (7,85–11,8) | 11,1 (8,35–13,5) |
| Yaşayan kamp (yıl ort.) | 7,12 (6,27–7,96) | 7,23 (6,29–9,1) | 7,51 (7,03–8,98) | 8,46 (7,72–10,2) | 9,55 (8,09–11,8) | 11 (8,09–13,3) |
| Doğan kamp | 1 (0,85–1,25) | 1,8 (1,25–2,15) | 1,9 (1,45–2,15) | 2,2 (1,15–2,6) | 2,25 (1,45–2,55) | 2,25 (1,8–3,3) |
| Temizlenen kamp | 0,4 (0,05–0,8) | 1,9 (1,35–2,35) | 1,8 (1,4–2,1) | 2,15 (1,05–2,7) | 2,1 (1,3–2,6) | 2,1 (1,55–3,25) |
| Canavar baskını | 3,6 (2,7–4,95) | 3,45 (2,7–5) | 3,45 (2,8–3,9) | 3,75 (2,3–4,65) | 3,75 (2,75–4,9) | 4,05 (2,9–4,8) |
| Yaşayan trol ini (yıl sonu) | 0 | 0,3 (0–0,55) | 0,55 (0,15–1,65) | 1,2 (0,7–2,4) | 1,75 (0,5–2,75) | 2,05 (0,8–2,85) |
| Yaşayan ejderha (yıl sonu) | 0 | 0,1 (0–0,2) | 1 (0,95–1) | 1 | 1 | 1 (0,95–1) |
| Ejderha akını | 0 | 0 (0–0,25) | 1 (0,6–1,1) | 0,8 (0,4–1,1) | 0,8 (0,3–1,2) | 0,8 (0,15–1,25) |
| Kriz (anlatıcı) | 0,1 (0–0,2) | 0,4 (0,1–0,45) | 0,3 (0,15–0,65) | 0,3 (0,05–0,55) | 0,4 (0,15–0,6) | 0,45 (0,05–0,6) |
| Rahatlama dönemi (anlatıcı) | 0,2 (0,2–0,3) | 0,05 (0–0,15) | 0,1 (0–0,2) | 0,1 (0–0,2) | 0 (0–0,15) | 0,1 (0–0,1) |
| **Kahramanlar** | | | | | | |
| Doğan kahraman | 2,65 (2,05–3,35) | 2,45 (2,05–3,55) | 3,15 (2,1–4,4) | 2,95 (1,9–4,4) | 3,25 (2,5–3,9) | 3,55 (2,4–5,05) |
| Ölen kahraman | 0,3 (0,1–0,65) | 0,75 (0,3–1,25) | 0,9 (0,4–1,9) | 1,3 (0,9–2,1) | 1,55 (0,8–2,4) | 1,9 (0,8–3) |
| Emekli olan kahraman | 0 | 0 | 0 (0–0,15) | 0,5 (0,2–0,65) | 1,05 (0,6–1,4) | 0,85 (0,5–1,65) |
| Diyarı terk eden kahraman | 0,05 (0–0,15) | 0,1 (0–0,15) | 0,1 (0–0,4) | 0,3 (0,15–0,55) | 0,35 (0,15–1) | 0,55 (0,2–0,85) |
| Ölümden dönen kahraman | 0 | 0 | 0 | 0 | 0 | 0 |
| Efsane olan kahraman | 0 | 0 (0–0,1) | 0,1 (0–0,15) | 0,1 (0–0,3) | 0,1 (0–0,5) | 0,2 (0–0,3) |
| Yaşayan kahraman (yıl sonu) | 10 (6,8–13) | 32,4 (27,1–38,4) | 54,3 (43,1–60,3) | 64,7 (54,2–73,2) | 68,2 (58,4–77,6) | 73,9 (62–84,4) |
| Doğuş seviyesi (ort.) | 1,27 (1,19–1,4) | 1,31 (1,17–1,42) | 1,31 (1,17–1,43) | 1,37 (1,27–1,43) | 1,42 (1,25–1,55) | 1,45 (1,32–1,51) |
| Ölüm seviyesi (ort.) | 1,33 (1–1,5) | 1,69 (1,17–2,08) | 2,5 (2–3,01) | 2,85 (1,98–3,45) | 3,06 (1,97–3,67) | 3,68 (2,94–4,73) |
| Yaşayan kahraman seviyesi (ort.) | 1,37 (1,24–1,5) | 2,24 (2,06–2,48) | 2,8 (2,63–3,02) | 3,26 (3,02–3,39) | 3,44 (3,13–3,75) | 3,41 (3,15–3,83) |
| En yüksek seviye (şimdiye dek) | 1,8 (1,45–2,15) | 4,1 (3,45–4,65) | 5,5 (5–6,2) | 7,15 (6,1–7,9) | 8,45 (7,25–9) | 9 (8,15–9,9) |
| **Han ve ticaret** | | | | | | |
| Ayakta han | 3,45 (3,2–3,75) | 4,6 (3,55–4,9) | 5 (4,55–5,95) | 5 (4,7–6) | 5,8 (4,95–6) | 5,55 (4,95–6) |
| Asılan ilan | 1,55 (1,1–2,05) | 2,25 (1,4–2,9) | 2,8 (2,3–3,75) | 3,1 (2,25–4,9) | 3,05 (2,65–5,15) | 3,1 (2,65–5,25) |
| Biten ilan | 0,15 (0–0,6) | 0,8 (0,45–1,5) | 0,8 (0,35–1) | 1,1 (0,45–1,75) | 1,15 (0,7–1,55) | 1,15 (0,8–2,1) |
| Ticaret seferi (kervan) | 16,2 (10,4–28,9) | 52,3 (29,8–78,2) | 73 (37,2–110) | 77,1 (45,8–136) | 93,8 (49,5–149) | 90,2 (50,5–166) |
| İkmal seferi | 0,3 (0–0,95) | 12,5 (6,95–19,7) | 24,6 (15–27) | 27,1 (20,4–34,2) | 30,1 (19,9–48,1) | 32,4 (20,7–50,7) |
| **Altın ve ambar** | | | | | | |
| Altın p90 (medeniyetler) | 166 (115–192) | 666 (518–772) | 1051 (833–1249) | 1258 (1011–1446) | 1329 (1047–1662) | 1424 (1124–1939) |
| Bakım gideri (altın; asker, kahraman, L2–L3) | 111 (92,8–163) | 892 (674–1047) | 1637 (1377–1860) | 2038 (1717–2287) | 2098 (1775–2506) | 2144 (1869–2534) |
| Kamu işlerine (imar) harcanan altın | 12,6 (0,63–27,5) | 785 (540–1124) | 1663 (1255–2229) | 2167 (1661–3143) | 2479 (1955–3688) | 2913 (2056–4385) |
| Ambarla beslenen amele tayını (gıda) | 30,3 (14,7–61,2) | 1303 (927–1629) | 2856 (1896–3596) | 2062 (1451–2602) | 1348 (827–2438) | 796 (315–2469) |
| Kamu işlerindeki (amele) iş gücü payı | %0,5 (%0,3–%0,9) | %13 (%11–%16) | %21 (%18–%24) | %20 (%18–%24) | %20 (%17–%23) | %19 (%17–%23) |
| İmar ortalaması (köy+, 0–100) | 0,26 (0,02–0,53) | 12,3 (10,1–16,3) | 25,9 (21,6–29,3) | 25,4 (21,5–31,1) | 25,5 (21,7–31,5) | 26,3 (22,1–33,5) |
| Canavar baskınında yitirilen altın | 8,45 (4,45–15,6) | 6,6 (0,95–21,4) | 5,2 (2,1–7,8) | 4,75 (0,95–13,2) | 3,3 (1,2–12) | 8,85 (1,9–17,8) |
| Ejderhaya giden altın (haraç + akın) | 0 | 0 (0–38,9) | 255 (157–502) | 429 (230–732) | 555 (275–896) | 555 (282–944) |
| Hazinesi boş medeniyet payı | %0 | %0 | %0 | %0 | %0 | %0 |
| Kent tüketiminde yokluk payı (köy+; ekmek, bira ya da alet) | %0,9 (%0,2–%1,9) | %2,2 (%0,2–%6,3) | %13 (%7,3–%19) | %36 (%27–%48) | %47 (%39–%58) | %48 (%37–%59) |
| Ekmek ya da bira yokluğu payı (köy+) | %0,8 (%0,2–%1,9) | %1,6 (%0–%5,4) | %3,5 (%0,3–%10) | %7,3 (%0,7–%13) | %7 (%2,5–%14) | %9,9 (%2,2–%14) |
| Kıtlık (büyük olay) | 0 | 0 | 0 | 0 | 0 (0–0,1) | 0 (0–0,05) |
| Açlıktan ölen | 0 | 0 | 0 | 0 | 0 | 0 (0–0,05) |
| Kıtlık yardımı (sevkiyat) | 0 | 0 | 0 | 0 | 0 (0–0,3) | 0 |
| Kıtlıkta yüz çeviren | 0 | 0 | 0 | 0 | 0 (0–0,1) | 0 (0–0,05) |
| Kıtlık akını | 0 | 0 | 0 | 0 | 0 (0–0,05) | 0 |
| Ambarın yettiği gün (medeniyet medyanı) | 79,5 (70,3–90,6) | 221 (206–257) | 257 (220–289) | 237 (176–268) | 201 (148–224) | 177 (142–211) |
| **Yerleşim kademesi** | | | | | | |
| Ortalama yerleşim kademesi | 0,56 (0,52–0,58) | 1 (0,94–1,03) | 1,17 (1,11–1,25) | 1,22 (1,16–1,32) | 1,27 (1,18–1,36) | 1,3 (1,18–1,4) |
| Köy+ yerleşim | 12,6 (10,7–14,5) | 35,4 (28–40,2) | 48,1 (38,8–53,7) | 54,8 (44,2–62,7) | 57,4 (47,9–68,5) | 62,5 (49,5–77,8) |
| Kasaba+ yerleşim | 1,65 (0,85–2,15) | 15,5 (11,3–17,1) | 24,3 (19,6–26,8) | 27,9 (21,9–32,5) | 31 (23,5–40,2) | 33 (24,6–42,4) |
| Şehir | 0 | 1,25 (0,3–1,8) | 5,15 (3,4–7,45) | 7,75 (6,15–10,5) | 9,3 (6,75–11,1) | 10,2 (7,75–12,1) |
| Ortalama başkent kademesi | 0,96 (0,9–1,03) | 2,03 (1,9–2,1) | 2,55 (2,43–2,74) | 2,79 (2,62–2,89) | 2,83 (2,65–2,92) | 2,87 (2,73–2,99) |
| Kademe değişimi (yerleşim, yıl içinde) | 3,85 (3,35–4,65) | 5,85 (3,7–8,05) | 6,25 (4,7–7,2) | 5,15 (3,25–6,7) | 4,5 (3,25–6,7) | 4,3 (3–7,25) |
| **Deniz** | | | | | | |
| Liman (tersane) | 1,95 (0,75–2,85) | 8,45 (6,4–11,3) | 12,6 (9,55–16,4) | 13,7 (9,9–19) | 15,3 (12,6–23,1) | 17,3 (13,1–26,7) |
| Gemi (koga/tekne) | 2,65 (0,95–4,7) | 17,7 (13,1–26,6) | 28,7 (22,1–41,9) | 34,4 (27–53) | 39,8 (31,4–62,5) | 46,6 (33,5–67,7) |
| Kadırga | 0 | 1,15 (0–4,15) | 9,25 (6,2–18) | 13,9 (10,4–22) | 16,8 (10,9–23,1) | 16,3 (10,5–23,2) |
| Denizaşırı yerleşim | 0,2 (0–0,65) | 4,8 (2,2–7,55) | 7 (3,85–9,8) | 7,75 (4,7–11,7) | 8,3 (6–13,8) | 9 (6–16,2) |
| Deniz ticaret yolu (yıl sonu) | 0,2 (0–0,75) | 5,4 (3,45–10) | 8,9 (5,45–15) | 9,75 (5,95–20,2) | 11,6 (8–25) | 12,6 (8,4–30,6) |
| Deniz seferi (ticaret) | 0 (0–0,15) | 2,35 (0–6,6) | 5,25 (0,75–15,1) | 6 (3,8–23,1) | 10,6 (5,7–26,7) | 14,5 (6,1–37,1) |

## Kahraman seviyeleri

### Doğuş seviyesi (bütün dünyalar, on yıl içinde doğanlar)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 |
|---|---|---|---|---|---|
| 1–10 | 430 | 1,3 | %70 | %30 | %0 |
| 11–20 | 423 | 1,3 | %70 | %30 | %0 |
| 21–30 | 512 | 1,28 | %72 | %28 | %0 |
| 31–40 | 477 | 1,36 | %66 | %33 | %1,5 |
| 41–50 | 516 | 1,43 | %60 | %37 | %3,3 |
| 51–60 | 584 | 1,43 | %60 | %37 | %3,1 |

### Ölüm seviyesi (bütün dünyalar, on yıl içinde ölenler)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 | Sv4 | Sv5 | Sv6 | Sv7 | Sv8 | Sv9 | Sv10 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 1–10 | 60 | 1,35 | %68 | %28 | %3,3 | %0 | %0 | %0 | %0 | %0 | %0 | %0 |
| 11–20 | 122 | 1,69 | %50 | %35 | %11 | %4,1 | %0 | %0 | %0 | %0 | %0 | %0 |
| 21–30 | 171 | 2,56 | %22 | %28 | %27 | %19 | %2,9 | %1,2 | %0 | %0 | %0 | %0 |
| 31–40 | 230 | 2,69 | %21 | %27 | %32 | %11 | %3,9 | %3,5 | %0,4 | %0,4 | %0,9 | %0 |
| 41–50 | 251 | 3,02 | %13 | %28 | %30 | %13 | %10 | %2,4 | %0,8 | %1,6 | %0,8 | %0 |
| 51–60 | 323 | 3,43 | %12 | %25 | %20 | %19 | %12 | %5,9 | %3,7 | %1,5 | %1,2 | %0,3 |

### Yaşayan kahramanların seviyesi (bütün dünyalar, on yılın son yılının sonunda)

| Yıl | n | ort. | Sv1 | Sv2 | Sv3 | Sv4 | Sv5 | Sv6 | Sv7 | Sv8 | Sv9 | Sv10 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 10 | 360 | 1,63 | %49 | %41 | %8,9 | %1,4 | %0 | %0 | %0 | %0 | %0 | %0 |
| 20 | 650 | 2,6 | %18 | %32 | %27 | %18 | %5,2 | %0,2 | %0 | %0 | %0 | %0 |
| 30 | 955 | 2,98 | %17 | %25 | %23 | %17 | %10 | %4,7 | %1,5 | %0 | %0 | %0 |
| 40 | 1079 | 3,35 | %11 | %26 | %23 | %16 | %13 | %6,8 | %2,9 | %1,8 | %0,1 | %0 |
| 50 | 1130 | 3,43 | %12 | %26 | %21 | %15 | %12 | %6,3 | %4,7 | %2,1 | %0,7 | %0,2 |
| 60 | 1222 | 3,49 | %12 | %23 | %24 | %15 | %11 | %6,3 | %4,1 | %1,9 | %1,6 | %0,7 |

### Ölüm nedenleri (bütün dünyalar)

| Neden | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 | Toplam |
|---|---|---|---|---|---|---|---|
| Kamp saldırısı | 33 | 79 | 59 | 66 | 84 | 121 | 442 (%38) |
| Kuşatma | 2 | 33 | 41 | 71 | 70 | 30 | 247 (%21) |
| Ejderha | 0 | 0 | 50 | 39 | 37 | 41 | 167 (%14) |
| Bilinmiyor | 0 | 0 | 0 | 1 | 15 | 77 | 93 (%8) |
| Trol | 0 | 0 | 13 | 35 | 19 | 23 | 90 (%7,8) |
| Han baskını (canavar) | 13 | 3 | 3 | 2 | 3 | 7 | 31 (%2,7) |
| Kervan soygunu | 3 | 3 | 1 | 6 | 6 | 8 | 27 (%2,3) |
| Yağma akını | 2 | 3 | 0 | 4 | 7 | 10 | 26 (%2,2) |
| Yol pususu | 0 | 0 | 3 | 2 | 7 | 4 | 16 (%1,4) |
| Yerleşim baskını (canavar) | 7 | 1 | 0 | 0 | 0 | 0 | 8 (%0,7) |
| Düello | 0 | 0 | 0 | 3 | 3 | 1 | 7 (%0,6) |
| Han baskını (medeniyet) | 0 | 0 | 1 | 1 | 0 | 1 | 3 (%0,3) |

Neden, ölümün kaydedildiği andaki son muharebenin türünden (başlık ve taraflar) ya da suikast olayından çıkarılır.

## Olay türleri

Dünya başına yıllık olay sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Kahraman (`hero`) | 9,9 (6,7–14) | 27,4 (23,7–37,6) | 42,6 (34,6–51,6) | 53,3 (37,7–81,8) | 67,5 (51,8–86,7) | 79,8 (54,7–100) |
| İnşaat (`build`) | 39,7 (32–49,6) | 76,5 (58,1–83,1) | 46,5 (36,7–57,5) | 26,1 (16,1–35,8) | 18 (11,8–23) | 15,7 (10,6–29,3) |
| Göç (`migration`) | 1,75 (1,35–3,6) | 8,9 (4,2–15,6) | 15,6 (11,8–22,1) | 17,8 (9,55–23,2) | 18,7 (11,4–25,8) | 15,1 (11,7–26,8) |
| Sefer/ilan (`quest`) | 2,8 (1,7–5,1) | 9,1 (5,8–12,9) | 10,1 (7,2–12,4) | 14,6 (7,4–21,1) | 15,6 (8,9–28) | 17,1 (12,2–32,3) |
| Savaş (`war`) | 0,8 (0–3,1) | 5,05 (2,05–12,6) | 9,45 (4,4–18,8) | 11,6 (3,2–21,3) | 11 (4,1–18) | 11,8 (1,75–18,8) |
| Kamp (`lair`) | 2,55 (2,1–3,5) | 4,65 (3,15–5,25) | 5,05 (3,7–6,15) | 6,15 (3,1–7,35) | 6,4 (4–7,3) | 6,45 (4,45–8,4) |
| Sınıf (`class`) | 3,45 (2,7–4,85) | 4,1 (3,3–5,4) | 4,25 (3,15–5,35) | 3,5 (3–4,6) | 3,35 (2,6–4,25) | 3,05 (2,2–4,3) |
| Deniz (`sea`) | 2,05 (0,5–4,3) | 4,05 (2,85–7,55) | 4 (2,25–8,75) | 2,55 (0,7–8,6) | 4,05 (1,15–5,65) | 2,6 (0,45–4,45) |
| Ekonomi (`economy`) | 0,2 (0–0,5) | 2,45 (1,3–3,25) | 4,7 (2,8–5,1) | 4,35 (2,55–5,95) | 3,6 (2,65–5,4) | 4 (1,75–5,75) |
| Baskın (`raid`) | 3,75 (2,85–5) | 3,45 (2,55–5) | 2,5 (1,95–3,1) | 3,35 (1,65–4,1) | 2,9 (2,35–4,2) | 3,25 (2,5–4,5) |
| Büyüme (`growth`) | 5,6 (4,25–6,4) | 4,85 (3,9–5,75) | 2,85 (2,45–3,25) | 1,95 (1,25–2,75) | 1,55 (0,95–2,6) | 1,2 (0,75–2,1) |
| Han (`inn`) | 2,9 (2,45–3,45) | 3,35 (2,05–4,25) | 2,6 (1,5–3,6) | 2,5 (1,45–3,3) | 2,4 (1,55–4) | 2,65 (2,1–3,35) |
| Yerleşim (`settle`) | 6 (4,75–6,65) | 3,8 (2,95–4,5) | 2 (1,6–2,6) | 1,35 (0,8–2,05) | 1,15 (0,55–2) | 1,2 (0,1–2) |
| Keşif (`discover`) | 7,65 (5,5–9) | 2,05 (0,95–3,6) | 1,75 (0,95–2,9) | 0,8 (0,15–2,35) | 0,9 (0,3–1,55) | 0,35 (0,1–1,65) |
| Ticaret (`trade`) | 2,45 (1,15–3,5) | 1,55 (0,75–2,6) | 1,15 (0,2–3,3) | 1,25 (0,15–2,5) | 0,75 (0,35–1,9) | 0,5 (0–1,75) |
| Ejderha (`dragon`) | 0 | 0,1 (0–0,5) | 2 (1,6–2,1) | 1,8 (1,25–2,1) | 1,8 (1,3–2,2) | 1,8 (1,15–2,25) |
| Ölüm/terk (`death`) | 0,3 (0,1–0,65) | 0,8 (0,3–1,4) | 1,05 (0,45–2) | 1,4 (0,95–2,2) | 1,8 (0,95–2,55) | 2 (1,25–3,5) |
| Dünya (`world`) | 0,1 (0–0,55) | 0,95 (0,55–2,3) | 1,35 (1,05–1,9) | 1,4 (0,75–2,2) | 1,7 (1,2–2,4) | 1,85 (1,3–2,8) |
| epitaph | 0,3 (0,1–0,65) | 0,75 (0,3–1,25) | 0,9 (0,4–1,9) | 1,3 (0,9–2,1) | 1,55 (0,8–2,4) | 1,9 (0,8–3) |
| Diplomasi (`diplomacy`) | 0,75 (0,25–1,5) | 0,65 (0,2–1,05) | 0,45 (0,2–1,7) | 0,3 (0–0,75) | 0,25 (0,05–0,9) | 0,4 (0–1,1) |
| Gerginlik (`tension`) | 0,8 (0,35–1,7) | 0,75 (0,15–1,2) | 0,5 (0,1–1,45) | 0,1 (0–0,75) | 0,35 (0–1,2) | 0,3 (0–1,65) |
| Kriz (anlatıcı) (`crisis`) | 0,1 (0–0,2) | 0,4 (0,1–0,45) | 0,3 (0,15–0,65) | 0,3 (0,05–0,55) | 0,4 (0,15–0,6) | 0,45 (0,05–0,6) |
| Temas (`contact`) | 1,25 (0,6–1,9) | 0,35 (0,1–0,65) | 0,1 (0–0,35) | 0,1 (0–0,3) | 0 (0–0,2) | 0 (0–0,15) |
| Rahatlama (anlatıcı) (`relief`) | 0,2 (0,15–0,3) | 0,1 (0–0,1) | 0,1 (0–0,2) | 0,1 (0–0,15) | 0 (0–0,15) | 0 (0–0,1) |

## Büyük olay türleri

Dünya başına yıllık büyük olay sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Sefer/ilan (`quest`) | 2,45 (1,7–3,75) | 6,65 (4,2–8,65) | 7,2 (5,6–8,65) | 10,4 (5,1–14,5) | 10,3 (6,55–18,9) | 11,2 (8,25–21,7) |
| Kahraman (`hero`) | 2,1 (1,1–2,65) | 5,2 (3,85–5,95) | 6,8 (5,1–8,9) | 8,25 (5,9–11,9) | 9,95 (8,5–13,3) | 10,3 (7,8–13,2) |
| Savaş (`war`) | 0,65 (0–2,6) | 3,85 (1,25–8,05) | 6,7 (3,35–11,3) | 8,1 (2,2–12,5) | 6,6 (3,2–10,8) | 8,35 (1,25–12,8) |
| Kamp (`lair`) | 2,55 (2,1–3,5) | 4,25 (3,05–5,2) | 4,2 (3,05–5) | 4,75 (2,4–5,85) | 4,7 (3,1–5,7) | 4,95 (3,45–6,75) |
| İnşaat (`build`) | 3,5 (2,75–4,45) | 7,25 (5,75–10,9) | 3,55 (2,1–5,35) | 1,3 (0,7–2,8) | 0,6 (0,2–1,45) | 0,6 (0–1,65) |
| Baskın (`raid`) | 3,15 (2,2–3,9) | 2,75 (1,85–4,2) | 1,9 (1,4–2,65) | 2,6 (1,2–3,6) | 2,6 (1,85–3,7) | 2,9 (1,75–4,2) |
| Han (`inn`) | 1,6 (1,2–1,85) | 2,25 (1,4–2,95) | 1,65 (1–2,05) | 1,7 (0,8–2,4) | 1,8 (1,2–2,9) | 1,9 (1,45–2,25) |
| Deniz (`sea`) | 1,2 (0,35–2,35) | 1,5 (1,05–3,1) | 1,75 (0,85–3,9) | 1,4 (0,5–4,9) | 2,65 (0,55–3,4) | 1,75 (0,25–2,7) |
| Yerleşim (`settle`) | 2,95 (2,4–3,45) | 2,15 (1,65–2,45) | 1,1 (0,8–1,35) | 0,7 (0,4–1,05) | 0,65 (0,3–1) | 0,6 (0,05–0,95) |
| Ejderha (`dragon`) | 0 | 0,1 (0–0,5) | 2 (1,6–2,1) | 1,8 (1,25–2,1) | 1,8 (1,3–2,2) | 1,8 (1,15–2,25) |
| Ölüm/terk (`death`) | 0,3 (0,1–0,65) | 0,8 (0,3–1,4) | 1,05 (0,45–2) | 1,4 (0,95–2,2) | 1,8 (0,95–2,55) | 2 (1,25–3,5) |
| Sınıf (`class`) | 1 (0,5–2) | 1,45 (0,75–2,2) | 1,4 (1,05–2,3) | 1 (0,5–2,1) | 1 (0,1–2) | 1 (0–2) |
| epitaph | 0,3 (0,1–0,65) | 0,75 (0,3–1,25) | 0,9 (0,4–1,9) | 1,3 (0,9–2,1) | 1,55 (0,8–2,4) | 1,9 (0,8–3) |
| Keşif (`discover`) | 2,55 (1,85–3,7) | 0,9 (0,45–1,65) | 0,85 (0,55–1,5) | 0,5 (0,1–0,95) | 0,45 (0,1–0,8) | 0,2 (0,1–1,05) |
| Dünya (`world`) | 0,1 (0–0,35) | 0,65 (0,35–1,6) | 0,95 (0,7–1,3) | 1 (0,6–1,45) | 1,25 (0,9–1,95) | 1,4 (0,95–1,8) |
| Ekonomi (`economy`) | 0 (0–0,1) | 1,4 (0,8–2,05) | 1,6 (1,25–2,15) | 0,75 (0,4–1) | 0,4 (0,25–0,7) | 0,2 (0,05–0,6) |
| Büyüme (`growth`) | 1,9 (1,3–2,15) | 1,6 (1,45–1,95) | 0,4 (0,25–0,65) | 0,1 (0–0,4) | 0 (0–0,2) | 0 (0–0,15) |
| Ticaret (`trade`) | 1,25 (0,55–1,65) | 0,75 (0,35–1,2) | 0,55 (0–1,45) | 0,65 (0,05–1,2) | 0,3 (0,1–0,85) | 0,2 (0–0,85) |
| Gerginlik (`tension`) | 0,8 (0,35–1,7) | 0,75 (0,15–1,2) | 0,5 (0,1–1,45) | 0,1 (0–0,75) | 0,35 (0–1,2) | 0,3 (0–1,65) |
| Kriz (anlatıcı) (`crisis`) | 0,1 (0–0,2) | 0,4 (0,1–0,45) | 0,3 (0,15–0,65) | 0,3 (0,05–0,55) | 0,4 (0,15–0,6) | 0,45 (0,05–0,6) |
| Temas (`contact`) | 1,25 (0,6–1,9) | 0,35 (0,1–0,65) | 0,1 (0–0,35) | 0,1 (0–0,3) | 0 (0–0,2) | 0 (0–0,15) |
| Diplomasi (`diplomacy`) | 0,4 (0,05–0,75) | 0,3 (0,1–0,55) | 0,3 (0,1–0,65) | 0,2 (0–0,5) | 0,2 (0–0,55) | 0,25 (0–1,05) |
| Göç (`migration`) | 0,1 (0–0,35) | 0,15 (0–0,45) | 0,1 (0–0,5) | 0,2 (0–0,3) | 0,2 (0,1–0,3) | 0,1 (0,1–0,3) |
| Rahatlama (anlatıcı) (`relief`) | 0,2 (0,15–0,3) | 0,1 (0–0,1) | 0,1 (0–0,2) | 0,1 (0–0,15) | 0 (0–0,15) | 0 (0–0,1) |

## Muharebe türleri

Dünya başına yıllık muharebe sayısı: dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Kamp saldırısı (`camp`) | 0,45 (0,1–0,95) | 2,05 (1,55–2,6) | 1,8 (1,4–2,05) | 1,95 (1,05–2,5) | 1,95 (1,3–2,55) | 2,05 (1,45–3) |
| Yapı baskını (canavar) (`extRaid`) | 1,9 (1,25–2,65) | 1,75 (1,1–3,25) | 0,95 (0,45–1,9) | 1,45 (0,5–2,2) | 1,55 (0,9–2,25) | 1,75 (0,65–3,15) |
| Yağma akını (`plunder`) | 0,05 (0–0,65) | 0,65 (0–1,7) | 1,4 (0,2–2,6) | 1,85 (0–2,5) | 1,55 (0,05–2,4) | 1,5 (0–3,1) |
| Yerleşim baskını (canavar) (`raid`) | 1,75 (1,15–2,25) | 1,4 (0,85–2,05) | 0,8 (0,3–1) | 0,7 (0,25–1,25) | 0,5 (0,2–1,05) | 0,6 (0,3–1,05) |
| Kuşatma (`siege`) | 0,2 (0–0,4) | 0,7 (0,35–1,25) | 1,1 (0,55–1,7) | 1,05 (0,45–1,75) | 1 (0,6–1,55) | 1,2 (0,15–1,6) |
| Trol (`troll`) | 0 | 0,15 (0–0,5) | 0,9 (0,15–1,3) | 1,25 (0,5–1,7) | 1,1 (0,55–1,8) | 1 (0,6–1,9) |
| Ejderha (`dragon`) | 0 | 0 (0–0,25) | 1 (0,6–1,05) | 0,75 (0,35–1,1) | 0,8 (0,3–1,2) | 0,75 (0,15–1,2) |
| Korsan savaşı (`pirate`) | 0 (0–0,05) | 0 (0–0,2) | 0,05 (0–0,35) | 0,1 (0–0,3) | 0,1 (0–0,6) | 0 (0–0,35) |
| Han baskını (canavar) (`innMonster`) | 0,1 (0,05–0,25) | 0 (0–0,15) | 0 (0–0,15) | 0 (0–0,15) | 0 (0–0,2) | 0 (0–0,15) |
| Yol pususu (`ambush`) | 0 (0–0,2) | 0 | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) |
| Düello (`duel`) | 0 | 0 | 0 | 0 (0–0,05) | 0 (0–0,1) | 0 |
| Han baskını (medeniyet) (`innCiv`) | 0 | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,05) |
| Deniz savaşı (`naval`) | 0 | 0 | 0 (0–0,35) | 0 (0–0,4) | 0 (0–0,3) | 0 (0–0,1) |
| Kervan soygunu (`robbery`) | 0 (0–0,1) | 0 (0–0,1) | 0 (0–0,15) | 0 (0–0,6) | 0 (0–0,2) | 0 (0–0,2) |

## Kamp türleri

Dünya başına yaşayan kamp (yıl sonu değerlerinin on yıllık ortalaması): dünyalar arası medyan (p10–p90).

| Tür | 1–10 | 11–20 | 21–30 | 31–40 | 41–50 | 51–60 |
|---|---|---|---|---|---|---|
| Hobgoblin (`hobgoblin`) | 1,15 (0,7–1,65) | 2,1 (1,5–3) | 2,8 (1,3–3,8) | 3,45 (2–4,4) | 3,7 (2,2–4,95) | 4,55 (3,25–6,05) |
| Goblin (`goblin`) | 5,65 (4,95–6,25) | 3,15 (1,9–4,75) | 1,65 (1,1–2,5) | 1,75 (0,75–2,5) | 1,55 (0,85–2,95) | 1,8 (0,75–3,45) |
| Trol (`troll`) | 0 | 0,3 (0–0,55) | 0,55 (0,15–1,65) | 1,2 (0,7–2,4) | 1,75 (0,5–2,75) | 2,05 (0,8–2,85) |
| Bugbear (`bugbear`) | 0,45 (0,1–0,65) | 0,65 (0,35–1,15) | 1 (0,25–1,35) | 0,8 (0,3–1,45) | 1 (0,45–1,15) | 1 (0,25–1,15) |
| Ejderha (`dragon`) | 0 | 0,1 (0–0,2) | 1 (0,95–1) | 1 | 1 | 1 (0,95–1) |
| Korsan (`pirate`) | 0,15 (0–0,4) | 0,7 (0,35–1,1) | 0,6 (0,2–1,45) | 0,5 (0–1,25) | 0,5 (0–1,5) | 0,5 (0,05–1,1) |

## Kademe dağılımı

Yaşayan yerleşimlerin kademelere dağılımı, bütün dünyalar (yıl sonu; parantezde sayı).

| Yıl | 0 Kamp | 1 Köy | 2 Kasaba | 3 Şehir |
|---|---|---|---|---|
| 10 | %31 (188) | %52 (311) | %17 (103) | %0 (0) |
| 20 | %29 (274) | %35 (330) | %30 (278) | %5,2 (48) |
| 30 | %26 (290) | %36 (400) | %27 (303) | %10 (113) |
| 40 | %25 (305) | %36 (429) | %27 (326) | %12 (142) |
| 50 | %24 (307) | %35 (453) | %29 (372) | %12 (149) |
| 60 | %24 (314) | %36 (477) | %28 (372) | %13 (167) |

Başkentlerin kademelere dağılımı, bütün dünyalar (yıl sonu; parantezde sayı).

| Yıl | 0 Kamp | 1 Köy | 2 Kasaba | 3 Şehir |
|---|---|---|---|---|
| 10 | %0 (0) | %38 (49) | %62 (79) | %0 (0) |
| 20 | %0 (0) | %2,3 (3) | %64 (83) | %33 (43) |
| 30 | %0 (0) | %1,6 (2) | %26 (33) | %73 (93) |
| 40 | %0 (0) | %3,1 (4) | %15 (19) | %82 (106) |
| 50 | %0 (0) | %0,8 (1) | %16 (20) | %84 (107) |
| 60 | %0 (0) | %4,7 (6) | %8,5 (11) | %87 (112) |

## Dünyalar

| Seed | Medeniyet | Yerleşim | Nüfus | Çöküş | Efsane | En yüksek Sv | Doğan / ölü kahraman | İlk şehir (yıl, medyan) | Süre (sn) | Son hash |
|---|---|---|---|---|---|---|---|---|---|---|
| 1 | 11 | 116 | 5316 | 9 | 4 | 9 | 191 / 67 | 21,5 | 70,5 | `bc3a0162ce7eebdc` |
| 2 | 7 | 59 | 3205 | 0 | 5 | 9 | 167 / 54 | 26 | 43,2 | `7ac548b46017c8a7` |
| 3 | 8 | 82 | 4405 | 16 | 13 | 10 | 195 / 96 | 19 | 47,5 | `16d376d273be85d7` |
| 4 | 8 | 78 | 4169 | 3 | 7 | 9 | 170 / 39 | 20 | 55,1 | `ceb8f24680d66374` |
| 5 | 8 | 96 | 4067 | 13 | 13 | 10 | 160 / 68 | 18 | 48,9 | `d8d659a083e2be8d` |
| 6 | 7 | 73 | 3244 | 1 | 1 | 9 | 134 / 37 | 25 | 42,6 | `9db5d52116686b92` |
| 7 | 10 | 98 | 4266 | 9 | 9 | 10 | 204 / 76 | 24 | 59,6 | `a4113f6504c910b5` |
| 8 | 8 | 79 | 3908 | 1 | 7 | 10 | 166 / 55 | 23 | 45,9 | `70d320d95e979704` |
| 9 | 9 | 102 | 4854 | 12 | 6 | 9 | 187 / 56 | 20 | 63,9 | `5c81290d26ae2d74` |
| 10 | 10 | 95 | 4434 | 7 | 7 | 9 | 193 / 86 | 19 | 57,5 | `5ae175c6ffd9362e` |
| 11 | 7 | 76 | 3508 | 12 | 5 | 10 | 198 / 81 | 18 | 44,2 | `5e3a8f744e1ff2e8` |
| 12 | 7 | 69 | 3187 | 3 | 4 | 10 | 160 / 54 | 27 | 40,2 | `9c257820d53a4872` |
| 13 | 7 | 73 | 3279 | 10 | 2 | 9 | 229 / 117 | 24 | 46,4 | `b2731873c6610cbb` |
| 14 | 9 | 80 | 3082 | 14 | 5 | 9 | 162 / 65 | 31 | 39,1 | `27d05f72432f9c16` |
| 15 | 7 | 68 | 3401 | 7 | 4 | 7 | 228 / 112 | 21 | 50,7 | `94326ff69710d0ff` |
| 16 | 6 | 86 | 3396 | 13 | 7 | 10 | 198 / 94 | 25 | 45,1 | `d21c3bdd72a9657d` |

Çöküşler:

- seed 1, 14. yıl (gün 1602): Rüzgâr Manastırı başkenti kaybetti: Sessiztepe (Kanlıdiş Kabileleri aldı)
- seed 1, 17. yıl (gün 2020): Rüzgâr Manastırı başkenti kaybetti: Sisliyamaç (Kanlıdiş Kabileleri aldı)
- seed 1, 19. yıl (gün 2216): Dinginpınar Tarikatı başkenti kaybetti: Dinginpınar (Kanlıdiş Kabileleri aldı)
- seed 1, 21. yıl (gün 2407): Rüzgâr Manastırı başkenti kaybetti: Taşbasamak (Kanlıdiş Kabileleri aldı)
- seed 1, 22. yıl (gün 2601): Tatlıçayır Loncası başkenti kaybetti: Balköprü (Kanlıdiş Kabileleri aldı)
- seed 1, 26. yıl (gün 3056): Kavşakpazar Beyliği başkenti kaybetti: Kavşakpazar (Tatlıçayır Loncası aldı)
- seed 1, 39. yıl (gün 4650): Tatlıçayır Loncası başkenti kaybetti: Keseli (Kanlıdiş Kabileleri aldı)
- seed 1, 53. yıl (gün 6295): Kanlıdiş Kabileleri başkenti kaybetti: Keseli (Kavşakpazar Beyliği aldı)
- seed 1, 57. yıl (gün 6754): Kanlıdiş Kabileleri başkenti kaybetti: Karageçit (Güneştacı Krallığı aldı)
- seed 3, 13. yıl (gün 1457): Karaörs Derinlikleri başkenti kaybetti: Külçukur (Örsyürek Tapınak Klanı aldı)
- seed 3, 18. yıl (gün 2129): Parşömenli Kara Beyliği başkenti kaybetti: Parşömenli (Karaörs Derinlikleri aldı)
- seed 3, 21. yıl (gün 2414): Parşömenli Kara Beyliği yok oldu
- seed 3, 26. yıl (gün 3085): Karaörs Derinlikleri başkenti kaybetti: Sessizocak (Güneştacı Krallığı aldı)
- seed 3, 28. yıl (gün 3354): Kanlıdiş Kabileleri başkenti kaybetti: Uluova (Sınır Bekçileri aldı)
- seed 3, 30. yıl (gün 3568): Karaörs Derinlikleri başkenti kaybetti: Gölgeörs (Güneştacı Krallığı aldı)
- seed 3, 32. yıl (gün 3836): Karaörs Derinlikleri başkenti kaybetti: Külçukur (Örsyürek Tapınak Klanı aldı)
- seed 3, 34. yıl (gün 4075): Kanlıdiş Kabileleri başkenti kaybetti: Dişlivadi (Sınır Bekçileri aldı)
- seed 3, 38. yıl (gün 4462): Kanlıdiş Kabileleri başkenti kaybetti: Savaşçukur (Örsyürek Tapınak Klanı aldı)
- seed 3, 38. yıl (gün 4520): Karaörs Derinlikleri başkenti kaybetti: Pınardere (Çarkyıldız Akademisi aldı)
- seed 3, 41. yıl (gün 4844): Karaörs Derinlikleri yok oldu
- seed 3, 41. yıl (gün 4894): Kanlıdiş Kabileleri başkenti kaybetti: Toynakbaş (Güneştacı Krallığı aldı)
- seed 3, 44. yıl (gün 5171): Kanlıdiş Kabileleri başkenti kaybetti: Kafatepe (Güneştacı Krallığı aldı)
- seed 3, 46. yıl (gün 5458): Kanlıdiş Kabileleri başkenti kaybetti: Dumanköprü (Güneştacı Krallığı aldı)
- seed 3, 47. yıl (gün 5624): Kanlıdiş Kabileleri yok oldu
- seed 3, 48. yıl (gün 5691): Pınardere Kara Beyliği yok oldu
- seed 4, 35. yıl (gün 4154): Pulzırh Lejyonu başkenti kaybetti: Közburç (Güneştacı Krallığı aldı)
- seed 4, 35. yıl (gün 4176): Sınır Bekçileri başkenti kaybetti: İzsürer (Pulzırh Lejyonu aldı)
- seed 4, 37. yıl (gün 4434): Sınır Bekçileri başkenti kaybetti: Yabanyurt (Pulzırh Lejyonu aldı)
- seed 5, 13. yıl (gün 1499): Karaörs Derinlikleri başkenti kaybetti: Zincirkaya (Güneştacı Krallığı aldı)
- seed 5, 15. yıl (gün 1762): Karaörs Derinlikleri başkenti kaybetti: Kara Mihrap (Güneştacı Krallığı aldı)
- seed 5, 18. yıl (gün 2045): Karaörs Derinlikleri yok oldu
- seed 5, 38. yıl (gün 4453): Kızılboynuz Soyu başkenti kaybetti: Kızılkül (Kanlıdiş Kabileleri aldı)
- seed 5, 39. yıl (gün 4653): Tatlıçayır Loncası başkenti kaybetti: Kavşakpazar (Kanlıdiş Kabileleri aldı)
- seed 5, 41. yıl (gün 4838): Kızılboynuz Soyu başkenti kaybetti: Aysırt (Pulzırh Lejyonu aldı)
- seed 5, 41. yıl (gün 4872): Kızılboynuz Soyu başkenti kaybetti: Söğütburç (Kanlıdiş Kabileleri aldı)
- seed 5, 47. yıl (gün 5565): Kızılboynuz Soyu başkenti kaybetti: Bozkale (Kanlıdiş Kabileleri aldı)
- seed 5, 50. yıl (gün 5969): Kızılboynuz Soyu yok oldu
- seed 5, 51. yıl (gün 6005): Kızıltepe Boyu başkenti kaybetti: Kızıltepe (Pulzırh Lejyonu aldı)
- seed 5, 58. yıl (gün 6952): Kanlıdiş Kabileleri başkenti kaybetti: Meşekent (Pulzırh Lejyonu aldı)
- seed 5, 59. yıl (gün 6986): Kızıltepe Boyu başkenti kaybetti: Kafatepe (Pulzırh Lejyonu aldı)
- seed 5, 60. yıl (gün 7164): Kızıltepe Boyu başkenti kaybetti: Gölköprü (Kanlıdiş Kabileleri aldı)
- seed 6, 38. yıl (gün 4478): Lirsesi Şehirleri başkenti kaybetti: Tamburlu (Karaörs Derinlikleri aldı)
- seed 7, 16. yıl (gün 1815): Kızılboynuz Soyu başkenti kaybetti: Közsaray (Karaörs Derinlikleri aldı)
- seed 7, 19. yıl (gün 2255): Kızılboynuz Soyu başkenti kaybetti: Kıvılcımlı (Karaörs Derinlikleri aldı)
- seed 7, 25. yıl (gün 2931): Kızılboynuz Soyu başkenti kaybetti: Aysırt (Karaörs Derinlikleri aldı)
- seed 7, 34. yıl (gün 4026): Kızılboynuz Soyu başkenti kaybetti: Dumanpınar (Karaörs Derinlikleri aldı)
- seed 7, 37. yıl (gün 4440): Kızılboynuz Soyu başkenti kaybetti: Kocaköprü (Karaörs Derinlikleri aldı)
- seed 7, 40. yıl (gün 4700): Kızılboynuz Soyu başkenti kaybetti: Söğütburç (Karaörs Derinlikleri aldı)
- seed 7, 43. yıl (gün 5122): Tatlıçayır Loncası başkenti kaybetti: Balköprü (Kanlıdiş Kabileleri aldı)
- seed 7, 53. yıl (gün 6343): Karaörs Derinlikleri başkenti kaybetti: Yabanyurt (Sınır Bekçileri aldı)
- seed 7, 59. yıl (gün 7044): Közsaray Hanedanı başkenti kaybetti: Közsaray (Karaörs Derinlikleri aldı)
- seed 8, 56. yıl (gün 6685): Örsyürek Tapınak Klanı başkenti kaybetti: Taşkandil (Pulzırh Lejyonu aldı)
- seed 9, 14. yıl (gün 1632): Sınır Bekçileri başkenti kaybetti: Okyayı (Karaörs Derinlikleri aldı)
- seed 9, 22. yıl (gün 2567): Sınır Bekçileri başkenti kaybetti: Gözcüağaç (Karaörs Derinlikleri aldı)
- seed 9, 23. yıl (gün 2758): Karaörs Derinlikleri başkenti kaybetti: Kara Mihrap (Güneştacı Krallığı aldı)
- seed 9, 26. yıl (gün 3018): Karaörs Derinlikleri başkenti kaybetti: Zincirkaya (Güneştacı Krallığı aldı)
- seed 9, 26. yıl (gün 3081): Sınır Bekçileri başkenti kaybetti: Yabanyurt (Kızılboynuz Soyu aldı)
- seed 9, 28. yıl (gün 3308): Karaörs Derinlikleri başkenti kaybetti: Sessizocak (Güneştacı Krallığı aldı)
- seed 9, 28. yıl (gün 3338): Gözcüağaç Beyliği başkenti kaybetti: Gözcüağaç (Kızılboynuz Soyu aldı)
- seed 9, 30. yıl (gün 3576): Karaörs Derinlikleri yok oldu
- seed 9, 32. yıl (gün 3754): Gözcüağaç Beyliği başkenti kaybetti: Gölpınar (Kızılboynuz Soyu aldı)
- seed 9, 34. yıl (gün 3971): Sınır Bekçileri başkenti kaybetti: Yelgeçit (Pulzırh Lejyonu aldı)
- seed 9, 47. yıl (gün 5575): Rüzgâr Manastırı başkenti kaybetti: Çankule (Pulzırh Lejyonu aldı)
- seed 9, 47. yıl (gün 5629): Okyayı Kara Beyliği yok oldu
- seed 10, 12. yıl (gün 1344): Sınır Bekçileri başkenti kaybetti: Gözcüağaç (Kanlıdiş Kabileleri aldı)
- seed 10, 34. yıl (gün 4053): Karaörs Derinlikleri başkenti kaybetti: Sessiztepe (Rüzgâr Manastırı aldı)
- seed 10, 38. yıl (gün 4465): Karaörs Derinlikleri başkenti kaybetti: Yıldızgöz (Sınır Bekçileri aldı)
- seed 10, 39. yıl (gün 4620): Karaörs Derinlikleri başkenti kaybetti: Keseli (Rüzgâr Manastırı aldı)
- seed 10, 47. yıl (gün 5604): Karaörs Derinlikleri başkenti kaybetti: Keseli (Tatlıçayır Loncası aldı)
- seed 10, 53. yıl (gün 6347): Karaörs Derinlikleri başkenti kaybetti: Keseli (Tatlıçayır Loncası aldı)
- seed 10, 58. yıl (gün 6857): Karaörs Derinlikleri başkenti kaybetti: Keseli (Tatlıçayır Loncası aldı)
- seed 11, 6. yıl (gün 710): Kızılboynuz Soyu başkenti kaybetti: Kızılkül (Kanlıdiş Kabileleri aldı)
- seed 11, 16. yıl (gün 1841): Kızılboynuz Soyu başkenti kaybetti: Közsaray (Kanlıdiş Kabileleri aldı)
- seed 11, 19. yıl (gün 2266): Kızılboynuz Soyu başkenti kaybetti: Alazvadi (Kanlıdiş Kabileleri aldı)
- seed 11, 25. yıl (gün 2903): Rüzgâr Manastırı başkenti kaybetti: Çankule (Kanlıdiş Kabileleri aldı)
- seed 11, 31. yıl (gün 3713): Rüzgâr Manastırı başkenti kaybetti: Sessiztepe (Kanlıdiş Kabileleri aldı)
- seed 11, 33. yıl (gün 3907): Çarkyıldız Akademisi başkenti kaybetti: Pusulakule (Kanlıdiş Kabileleri aldı)
- seed 11, 35. yıl (gün 4116): Rüzgâr Manastırı başkenti kaybetti: Dinginpınar (Kanlıdiş Kabileleri aldı)
- seed 11, 38. yıl (gün 4543): Kızılboynuz Soyu başkenti kaybetti: Ceylanoba (Kanlıdiş Kabileleri aldı)
- seed 11, 40. yıl (gün 4765): Çarkyıldız Akademisi başkenti kaybetti: Parşömenli (Kanlıdiş Kabileleri aldı)
- seed 11, 47. yıl (gün 5625): Çarkyıldız Akademisi başkenti kaybetti: Parşömenli (Kanlıdiş Kabileleri aldı)
- seed 11, 49. yıl (gün 5825): Rüzgâr Manastırı başkenti kaybetti: Taşbasamak (Kanlıdiş Kabileleri aldı)
- seed 11, 58. yıl (gün 6921): Kızılboynuz Soyu başkenti kaybetti: Karamum (Kanlıdiş Kabileleri aldı)
- seed 12, 12. yıl (gün 1399): Kızılboynuz Soyu başkenti kaybetti: Közsaray (Pulzırh Lejyonu aldı)
- seed 12, 46. yıl (gün 5498): Kanlıdiş Kabileleri başkenti kaybetti: Kemikçadır (Pulzırh Lejyonu aldı)
- seed 12, 48. yıl (gün 5757): Karamum Lejyonu yok oldu
- seed 13, 16. yıl (gün 1893): Sınır Bekçileri başkenti kaybetti: Gözcüağaç (Kızılboynuz Soyu aldı)
- seed 13, 19. yıl (gün 2242): Kızılboynuz Soyu başkenti kaybetti: Kurtgeçit (Kanlıdiş Kabileleri aldı)
- seed 13, 21. yıl (gün 2463): Sınır Bekçileri başkenti kaybetti: Kartalyazı (Kanlıdiş Kabileleri aldı)
- seed 13, 23. yıl (gün 2741): Akburç Boyu başkenti kaybetti: Akburç (Kızılboynuz Soyu aldı)
- seed 13, 25. yıl (gün 2899): Sınır Bekçileri yok oldu
- seed 13, 25. yıl (gün 2929): Akburç Boyu yok oldu
- seed 13, 29. yıl (gün 3477): Kızılboynuz Soyu başkenti kaybetti: Kızılkül (Rüzgâr Manastırı aldı)
- seed 13, 42. yıl (gün 4965): Kızılboynuz Soyu başkenti kaybetti: Karamum (Kanlıdiş Kabileleri aldı)
- seed 13, 44. yıl (gün 5275): Kızılboynuz Soyu başkenti kaybetti: Alazvadi (Kanlıdiş Kabileleri aldı)
- seed 13, 52. yıl (gün 6131): Kızılboynuz Soyu başkenti kaybetti: Kızılkül (Kanlıdiş Kabileleri aldı)
- seed 14, 10. yıl (gün 1081): Sınır Bekçileri başkenti kaybetti: Gözcüağaç (Karaörs Derinlikleri aldı)
- seed 14, 19. yıl (gün 2176): Karaörs Derinlikleri başkenti kaybetti: Geyikyurt (Yeşilyaprak Çemberi aldı)
- seed 14, 19. yıl (gün 2225): Sınır Bekçileri başkenti kaybetti: Kurtgeçit (Karaörs Derinlikleri aldı)
- seed 14, 23. yıl (gün 2668): Sınır Bekçileri başkenti kaybetti: Çamgözcü (Karaörs Derinlikleri aldı)
- seed 14, 28. yıl (gün 3255): Çarkyıldız Akademisi başkenti kaybetti: Pusulakule (Karaörs Derinlikleri aldı)
- seed 14, 40. yıl (gün 4792): Çarkyıldız Akademisi başkenti kaybetti: Mürekkeptepe (Karaörs Derinlikleri aldı)
- seed 14, 42. yıl (gün 5024): Karaörs Derinlikleri başkenti kaybetti: Gölköprü (Örsyürek Tapınak Klanı aldı)
- seed 14, 45. yıl (gün 5385): Sınır Bekçileri başkenti kaybetti: Alacayayla (Karaörs Derinlikleri aldı)
- seed 14, 49. yıl (gün 5838): Karaörs Derinlikleri başkenti kaybetti: Mürekkeptepe (Güneştacı Krallığı aldı)
- seed 14, 53. yıl (gün 6318): Karaörs Derinlikleri başkenti kaybetti: Ceylanbük (Yeşilyaprak Çemberi aldı)
- seed 14, 55. yıl (gün 6580): Karaörs Derinlikleri başkenti kaybetti: Siskoru (Örsyürek Tapınak Klanı aldı)
- seed 14, 58. yıl (gün 6885): Karaörs Derinlikleri başkenti kaybetti: Ceylanbük (Güneştacı Krallığı aldı)
- seed 14, 59. yıl (gün 6982): Kara Mihrap Çemberi başkenti kaybetti: Kara Mihrap (Karaörs Derinlikleri aldı)
- seed 14, 60. yıl (gün 7189): Karaörs Derinlikleri başkenti kaybetti: Sessizocak (Örsyürek Tapınak Klanı aldı)
- seed 15, 23. yıl (gün 2711): Sınır Bekçileri başkenti kaybetti: İzsürer (Kanlıdiş Kabileleri aldı)
- seed 15, 30. yıl (gün 3544): Sınır Bekçileri başkenti kaybetti: Gözcüağaç (Kanlıdiş Kabileleri aldı)
- seed 15, 33. yıl (gün 3958): Sınır Bekçileri başkenti kaybetti: Okyayı (Kanlıdiş Kabileleri aldı)
- seed 15, 50. yıl (gün 5969): Güneştacı Krallığı başkenti kaybetti: Işıkdere (Kanlıdiş Kabileleri aldı)
- seed 15, 52. yıl (gün 6225): Güneştacı Krallığı yok oldu
- seed 15, 57. yıl (gün 6806): Akburç Boyu başkenti kaybetti: Akburç (Kızılboynuz Soyu aldı)
- seed 15, 60. yıl (gün 7099): Akburç Boyu yok oldu
- seed 16, 13. yıl (gün 1553): Tatlıçayır Loncası başkenti kaybetti: Kavşakpazar (Kanlıdiş Kabileleri aldı)
- seed 16, 16. yıl (gün 1907): Tatlıçayır Loncası başkenti kaybetti: Keseli (Kanlıdiş Kabileleri aldı)
- seed 16, 20. yıl (gün 2334): Tatlıçayır Loncası başkenti kaybetti: Kırkkapı (Kanlıdiş Kabileleri aldı)
- seed 16, 25. yıl (gün 2909): Tatlıçayır Loncası başkenti kaybetti: Taşkandil (Kanlıdiş Kabileleri aldı)
- seed 16, 29. yıl (gün 3420): Aksırt Boyu yok oldu
- seed 16, 37. yıl (gün 4332): Tatlıçayır Loncası başkenti kaybetti: Kızıltepe (Kanlıdiş Kabileleri aldı)
- seed 16, 39. yıl (gün 4589): Tatlıçayır Loncası yok oldu
- seed 16, 48. yıl (gün 5730): Karaörs Derinlikleri başkenti kaybetti: Sessizocak (Güneştacı Krallığı aldı)
- seed 16, 49. yıl (gün 5775): Karaörs Derinlikleri başkenti kaybetti: Karagöl (Sınır Bekçileri aldı)
- seed 16, 51. yıl (gün 6002): Özgür Aksırt başkenti kaybetti: Aksırt (Kanlıdiş Kabileleri aldı)
- seed 16, 51. yıl (gün 6069): Karaörs Derinlikleri başkenti kaybetti: Gölgeörs (Güneştacı Krallığı aldı)
- seed 16, 54. yıl (gün 6363): Karaörs Derinlikleri yok oldu
- seed 16, 57. yıl (gün 6790): Özgür Aksırt yok oldu

## Yıllık ayrıntı

Hücre: medyan (p10–p90), 16 dünya. Yıl y = (y−1)·120+1 … y·120. günler. Bütün değerler `report.json` içinde (`metrics`), dünya başına değerler `../runs/f1b-3` altında.

### Medeniyet (1/3)

| Yıl | Yaşayan medeniyet | Yeni medeniyet (yeniden doğan) | Yok olan medeniyet | Başkent kaybı (medeniyet yaşarken) | Çöküş (yok olma + başkent kaybı) | Yaşayan yerleşim |
|---|---|---|---|---|---|---|
| 1 | 8 (7–9) | 0 | 0 | 0 | 0 | 8 (7–9) |
| 2 | 8 (7–9) | 0 | 0 | 0 | 0 | 8 (7–9) |
| 3 | 8 (7–9) | 0 | 0 | 0 | 0 | 10 (9–12) |
| 4 | 8 (7–9) | 0 | 0 | 0 | 0 | 15 (12–17,5) |
| 5 | 8 (7–9) | 0 | 0 | 0 | 0 | 19,5 (16–22,5) |
| 6 | 8 (7–9) | 0 | 0 | 0 | 0 | 24,5 (20,5–27,5) |
| 7 | 8 (7–9) | 0 | 0 | 0 | 0 | 28 (23–31,5) |
| 8 | 8 (7–9) | 0 | 0 | 0 | 0 | 32,5 (27,5–37) |
| 9 | 8 (7–9) | 0 | 0 | 0 | 0 | 35 (29–41) |
| 10 | 8 (7–9) | 0 | 0 | 0 | 0 | 37,5 (31–43) |
| 11 | 8 (7–9) | 0 | 0 | 0 | 0 | 41 (33,5–47,5) |
| 12 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 43,5 (35,5–50,5) |
| 13 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 47 (37–53,5) |
| 14 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 49,5 (38,5–56) |
| 15 | 8 (7–9) | 0 | 0 | 0 | 0 | 51 (40–56,5) |
| 16 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 53 (41,5–59) |
| 17 | 8 (7–9) | 0 | 0 | 0 | 0 | 54,5 (44–61) |
| 18 | 8 (7–9) | 0 | 0 | 0 | 0 (0–0,5) | 56 (44,5–63,5) |
| 19 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 58 (46,5–64,5) |
| 20 | 8 (7–9) | 0 | 0 | 0 | 0 | 59,5 (48–67) |
| 21 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–1) | 60 (49–68) |
| 22 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 60,5 (50,5–69,5) |
| 23 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 63 (51,5–70,5) |
| 24 | 8 (7–9) | 0 | 0 | 0 | 0 | 64 (52,5–72,5) |
| 25 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 66 (53,5–74) |
| 26 | 8 (7–9,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 68 (54–74,5) |
| 27 | 8 (7–9,5) | 0 | 0 | 0 | 0 | 68,5 (56–74,5) |
| 28 | 8 (7–9,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 69,5 (56,5–75,5) |
| 29 | 8 (7–9,5) | 0 | 0 | 0 | 0 (0–0,5) | 70,5 (57,5–76,5) |
| 30 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–1) | 71 (58–77,5) |
| 31 | 8 (7–9) | 0 | 0 | 0 | 0 | 71 (58,5–78,5) |
| 32 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 71 (59–80) |
| 33 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 72 (60–81,5) |
| 34 | 8 (7–9,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 73 (60–83) |
| 35 | 8 (7–9,5) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 73 (60–84) |
| 36 | 8 (7–9,5) | 0 | 0 | 0 | 0 | 73,5 (61–84,5) |
| 37 | 8 (7–9,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 73,5 (62–85,5) |
| 38 | 8 (7–9,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 73,5 (63,5–85,5) |
| 39 | 8 (7–9,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 73,5 (63,5–86,5) |
| 40 | 8 (7–9,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 74 (64–86,5) |
| 41 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–1) | 73,5 (64–88) |
| 42 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 74 (65–88,5) |
| 43 | 8 (7–9) | 0 | 0 | 0 | 0 | 74,5 (64,5–90) |
| 44 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 75,5 (65–90) |
| 45 | 8 (7–9) | 0 | 0 | 0 | 0 | 75,5 (64,5–91,5) |
| 46 | 8 (7–9,5) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 76 (65,5–92) |
| 47 | 8 (7–9) | 0 (0–1) | 0 (0–0,5) | 0 (0–1) | 0 (0–1) | 76 (67–93,5) |
| 48 | 8 (7–9) | 0 | 0 (0–0,5) | 0 | 0 (0–1) | 76 (67–94) |
| 49 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 76,5 (67–95) |
| 50 | 8 (7–9) | 0 | 0 | 0 | 0 (0–0,5) | 77 (68–95) |
| 51 | 8 (7–9) | 0 | 0 | 0 (0–0,5) | 0 (0–0,5) | 77,5 (67,5–95) |
| 52 | 8 (7–9) | 0 | 0 | 0 | 0 (0–0,5) | 77 (67,5–94,5) |
| 53 | 8 (7–9) | 0 | 0 | 0 (0–1) | 0 (0–1) | 78 (68–95,5) |
| 54 | 8 (7–9) | 0 | 0 | 0 | 0 | 78 (68–96,5) |
| 55 | 8 (7–9,5) | 0 (0–1) | 0 | 0 | 0 | 79 (67,5–97) |
| 56 | 8 (7–9,5) | 0 | 0 | 0 | 0 | 79 (68–98) |
| 57 | 8 (7–9,5) | 0 | 0 | 0 (0–0,5) | 0 (0–1) | 78,5 (68,5–98,5) |
| 58 | 8 (7–9,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 79,5 (69–99,5) |
| 59 | 8 (7–9,5) | 0 | 0 | 0 (0–1) | 0 (0–1) | 80 (69–100) |
| 60 | 8 (7–10) | 0 | 0 | 0 (0–0,5) | 0 (0–1) | 79,5 (68,5–100) |

### Medeniyet (2/3)

| Yıl | Medeniyet başına yerleşim | 5+ kara yerleşimli medeniyet payı | Kurulan yerleşim | Fethedilen yerleşim | Terk edilen yerleşim | Toplam nüfus |
|---|---|---|---|---|---|---|
| 1 | 1 | %0 | 0 | 0 | 0 | 62 (54–72) |
| 2 | 1 | %0 | 0 | 0 | 0 | 86 (75–100) |
| 3 | 1,33 (1,17–1,43) | %0 | 3 (1,5–3) | 0 | 0 | 128 (102–149) |
| 4 | 1,86 (1,56–2,06) | %0 | 5 (2,5–6) | 0 | 0 | 175 (146–214) |
| 5 | 2,4 (2,25–2,63) | %0 | 4,5 (3–6) | 0 | 0 | 258 (213–314) |
| 6 | 3 (2,8–3,24) | %0 (%0–%13) | 5 (3,5–6,5) | 0 (0–1) | 0 | 368 (298–442) |
| 7 | 3,47 (3,29–3,81) | %12 (%0–%27) | 4 (2,5–5) | 0 (0–0,5) | 0 | 500 (399–588) |
| 8 | 4 (3,71–4,3) | %31 (%14–%47) | 4 (2–5) | 0 (0–2) | 0 | 635 (509–757) |
| 9 | 4,4 (4,11–4,62) | %44 (%33–%65) | 3,5 (1,5–5) | 0 (0–1) | 0 | 776 (628–916) |
| 10 | 4,69 (4,39–4,99) | %53 (%43–%69) | 3 (0,5–3) | 1 (0–2) | 0 | 930 (710–1077) |
| 11 | 5,12 (4,79–5,53) | %75 (%57–%87) | 3,5 (2–5) | 0 (0–1) | 0 | 1060 (847–1239) |
| 12 | 5,44 (5,07–5,87) | %78 (%65–%88) | 2,5 (1–4) | 1 (0–1,5) | 0 | 1212 (932–1407) |
| 13 | 5,73 (5,29–6,06) | %78 (%60–%94) | 2,5 (1–4) | 1 (0–1) | 0 | 1338 (1070–1550) |
| 14 | 6,06 (5,5–6,31) | %88 (%69–%100) | 2 (1–4) | 0,5 (0–1,5) | 0 | 1483 (1160–1716) |
| 15 | 6,22 (5,59–6,4) | %87 (%76–%100) | 2 (0–3,5) | 1 (0–2) | 0 | 1575 (1264–1832) |
| 16 | 6,51 (5,76–6,65) | %88 (%71–%100) | 2 (1–3) | 1 (0–1) | 0 | 1718 (1344–1938) |
| 17 | 6,67 (6,07–6,88) | %89 (%79–%100) | 2 (0,5–3,5) | 1 (0–1,5) | 0 (0–1) | 1839 (1446–2051) |
| 18 | 6,87 (6,27–7,12) | %89 (%82–%100) | 1,5 (0,5–2) | 0,5 (0–2) | 0 | 1954 (1543–2204) |
| 19 | 7,06 (6,54–7,38) | %89 (%86–%100) | 1 (0,5–3) | 1 (0,5–2) | 0 | 2017 (1604–2324) |
| 20 | 7,25 (6,83–7,56) | %100 (%86–%100) | 1,5 (0,5–3) | 0 (0–1) | 0 | 2130 (1692–2457) |
| 21 | 7,44 (6,79–7,76) | %100 (%82–%100) | 1 (0–2) | 1 (0–2,5) | 0 | 2204 (1767–2520) |
| 22 | 7,54 (6,91–7,94) | %100 (%87–%100) | 1 (0–3) | 0 (0–1,5) | 0 | 2338 (1852–2665) |
| 23 | 7,76 (7,14–8,06) | %94 (%82–%100) | 1 (0–2,5) | 1 (0,5–2) | 0 | 2365 (1966–2730) |
| 24 | 7,88 (7,1–8,24) | %100 (%82–%100) | 1 (0–2) | 1 (0–2) | 0 | 2487 (2019–2854) |
| 25 | 8,06 (7,56–8,5) | %100 (%87–%100) | 1 (0–2,5) | 1 (0–2) | 0 | 2540 (2090–2930) |
| 26 | 8,07 (7,54–8,63) | %100 (%83–%100) | 1 (0–2) | 1 (0–2,5) | 0 | 2621 (2150–3010) |
| 27 | 8,13 (7,53–8,69) | %100 (%79–%100) | 1 (0–2) | 1 (0–3) | 0 | 2734 (2212–3104) |
| 28 | 8,33 (7,69–8,94) | %100 (%83–%100) | 1 (0–2,5) | 1 (0–3,5) | 0 | 2778 (2287–3216) |
| 29 | 8,51 (7,92–9) | %100 (%88–%100) | 1 (0–3) | 0,5 (0–3) | 0 | 2953 (2315–3278) |
| 30 | 8,69 (8,2–9,06) | %100 (%88–%100) | 0,5 (0–2) | 1 (0–2,5) | 0 (0–0,5) | 2888 (2410–3320) |
| 31 | 8,75 (8,33–9,21) | %100 (%88–%100) | 0,5 (0–1,5) | 1 (0–2) | 0 | 3064 (2424–3416) |
| 32 | 8,88 (8,4–9,27) | %100 (%88–%100) | 0 (0–2,5) | 1 (0–3) | 0 | 3062 (2488–3408) |
| 33 | 9 (8,3–9,47) | %100 (%88–%100) | 1 (0–2) | 1 (0–2) | 0 (0–0,5) | 3134 (2539–3466) |
| 34 | 9,06 (8,19–9,54) | %100 (%84–%100) | 1 (0–2) | 1 (0–2,5) | 0 | 3184 (2606–3600) |
| 35 | 9,06 (8,26–9,69) | %100 (%83–%100) | 1 (0–2) | 1 (0–2,5) | 0 (0–1) | 3228 (2550–3598) |
| 36 | 9,25 (8,26–9,75) | %100 (%83–%100) | 1 (0–1) | 1 (0–2) | 0 | 3205 (2689–3660) |
| 37 | 9,3 (8,32–9,94) | %100 (%83–%100) | 1 (0–2,5) | 1 (0–1,5) | 0 (0–0,5) | 3308 (2770–3792) |
| 38 | 9,4 (8,32–10) | %100 (%83–%100) | 0 (0–1,5) | 1 (0–2) | 0 | 3402 (2774–3774) |
| 39 | 9,33 (8,32–10,2) | %100 (%78–%100) | 0 (0–1) | 1 (0–2,5) | 0 (0–1) | 3388 (2824–3882) |
| 40 | 9,4 (8,32–10,3) | %100 (%78–%100) | 0 (0–1,5) | 1 (0–2) | 0 (0–0,5) | 3475 (2832–3906) |
| 41 | 9,36 (8,64–10,4) | %100 (%78–%100) | 0,5 (0–1) | 1 (0–2) | 0 | 3471 (2905–3885) |
| 42 | 9,43 (8,63–10,4) | %100 (%78–%100) | 1 (0–1) | 1 (0–2) | 0 | 3502 (2942–4004) |
| 43 | 9,4 (8,63–10,5) | %100 (%78–%100) | 0,5 (0–1,5) | 1 (0–3) | 0 | 3552 (2897–4094) |
| 44 | 9,54 (8,26–10,6) | %100 (%78–%100) | 1 (0–2) | 1 (0–2,5) | 0 (0–1) | 3526 (2960–3996) |
| 45 | 9,54 (8,26–10,5) | %100 (%76–%100) | 0 (0–2) | 1 (0–3) | 0 (0–0,5) | 3505 (2978–4150) |
| 46 | 9,63 (8,13–10,6) | %95 (%75–%100) | 1 (0–1,5) | 1 (0–2) | 0 (0–0,5) | 3563 (3040–4167) |
| 47 | 9,63 (8,38–10,6) | %88 (%76–%100) | 0,5 (0–2) | 1,5 (0,5–3) | 0 (0–1) | 3520 (3033–4140) |
| 48 | 9,86 (8,5–10,6) | %89 (%75–%100) | 0 (0–1) | 0,5 (0–1,5) | 0 (0–0,5) | 3516 (3030–4266) |
| 49 | 9,89 (8,52–10,7) | %89 (%75–%100) | 0,5 (0–1,5) | 1 (0–3) | 0 (0–1) | 3584 (3072–4296) |
| 50 | 10,1 (8,75–10,8) | %95 (%76–%100) | 0 (0–2,5) | 1 (0–1,5) | 0 (0–1) | 3564 (3100–4294) |
| 51 | 10,2 (8,75–11) | %89 (%76–%100) | 0 (0–1) | 1 (0–3) | 0 | 3628 (3005–4330) |
| 52 | 10,2 (9,31–11) | %88 (%82–%100) | 0 (0–1) | 1 (0–2) | 0 (0–1) | 3689 (3040–4384) |
| 53 | 10,2 (9,44–11,1) | %95 (%82–%100) | 1 (0–2) | 0,5 (0–2,5) | 0 (0–1) | 3634 (3016–4436) |
| 54 | 10,3 (9,44–11,3) | %100 (%86–%100) | 0 (0–1) | 1 (0–2) | 0 | 3638 (3154–4412) |
| 55 | 10,2 (8,39–11,3) | %100 (%76–%100) | 1 (0–1,5) | 1 (0–3) | 0,5 (0–1,5) | 3708 (3148–4314) |
| 56 | 10,3 (8,59–11,4) | %94 (%80–%100) | 1 (0–2) | 1 (0–2) | 0 | 3714 (3112–4520) |
| 57 | 10,3 (8,65–11,5) | %100 (%80–%100) | 0,5 (0–1) | 1 (0–1,5) | 0 (0–0,5) | 3720 (3184–4523) |
| 58 | 10,3 (8,76–11,5) | %100 (%76–%100) | 0 (0–1,5) | 1 (0–3) | 0 (0–1) | 3677 (3216–4534) |
| 59 | 10,3 (8,69–11,6) | %100 (%76–%100) | 0 (0–1,5) | 1 (0–2) | 0 (0–1) | 3702 (3224–4521) |
| 60 | 10,1 (9,19–11,7) | %89 (%82–%100) | 0 (0–2) | 1 (0–1,5) | 0 (0–1) | 3708 (3196–4644) |

### Medeniyet (3/3)

| Yıl | Altın medyanı (medeniyetler) | Boştaki iş gücü payı | Bölünme (ayrılıp kurulan medeniyet) | En büyük medeniyetin yerleşimi |
|---|---|---|---|---|
| 1 | 8,32 (8,22–8,39) | %0 | 0 | 1 |
| 2 | 18,9 (13–25,8) | %0 | 0 | 1 |
| 3 | 29,1 (18,2–38,6) | %0 | 0 | 2 |
| 4 | 32,3 (24,1–53,7) | %0 | 0 | 3 (2–3) |
| 5 | 36,2 (25,7–56,4) | %0 | 0 | 3 (3–4) |
| 6 | 32,5 (20–73) | %0,1 (%0–%0,3) | 0 | 4 (4–5) |
| 7 | 49,6 (29,1–79,5) | %0,4 (%0–%2,6) | 0 | 5 (4–5) |
| 8 | 44,1 (24,9–112) | %1,1 (%0–%4,2) | 0 | 5 (5–6) |
| 9 | 75,7 (37,7–109) | %2,3 (%0,6–%4,4) | 0 | 6 (5–6,5) |
| 10 | 76 (24,2–172) | %3 (%1,4–%5,3) | 0 | 6,5 (5–7,5) |
| 11 | 111 (44,4–185) | %4,7 (%2,7–%6,8) | 0 | 7 (6–8) |
| 12 | 131 (49,3–244) | %5,9 (%4,4–%9) | 0 | 8 (7–9) |
| 13 | 154 (77,7–276) | %9,1 (%7–%11) | 0 | 8 (7–9) |
| 14 | 178 (113–350) | %12 (%8,6–%15) | 0 | 8 (8–9) |
| 15 | 203 (98,5–327) | %12 (%10–%18) | 0 | 8,5 (8–9) |
| 16 | 199 (92,9–371) | %14 (%11–%18) | 0 | 9 (8–10) |
| 17 | 271 (132–397) | %15 (%13–%20) | 0 | 9 (8–10) |
| 18 | 303 (175–428) | %17 (%14–%19) | 0 | 9 (8–10,5) |
| 19 | 364 (230–508) | %20 (%13–%22) | 0 | 9,5 (8–11) |
| 20 | 399 (115–461) | %21 (%14–%23) | 0 | 10 (8–11,5) |
| 21 | 367 (249–515) | %21 (%14–%25) | 0 | 10 (8,5–11,5) |
| 22 | 356 (209–494) | %20 (%14–%26) | 0 | 10 (9–12) |
| 23 | 365 (174–535) | %21 (%15–%27) | 0 | 10,5 (9–12) |
| 24 | 368 (194–446) | %23 (%15–%27) | 0 | 10 (9–13) |
| 25 | 390 (188–560) | %22 (%15–%25) | 0 | 11 (9–13) |
| 26 | 392 (192–522) | %19 (%15–%27) | 0 | 11 (9,5–14) |
| 27 | 398 (206–629) | %21 (%16–%24) | 0 | 11,5 (9,5–14,5) |
| 28 | 390 (239–584) | %21 (%16–%24) | 0 | 11,5 (10–15) |
| 29 | 425 (263–564) | %20 (%18–%25) | 0 | 12 (10–15,5) |
| 30 | 488 (297–652) | %21 (%15–%26) | 0 | 12 (10–16) |
| 31 | 423 (286–679) | %21 (%16–%25) | 0 | 12,5 (10–16,5) |
| 32 | 497 (272–627) | %21 (%16–%26) | 0 | 13 (10–16) |
| 33 | 499 (287–591) | %21 (%16–%26) | 0 | 13 (10–16,5) |
| 34 | 562 (421–656) | %20 (%16–%23) | 0 | 14 (10–17) |
| 35 | 541 (298–841) | %20 (%16–%23) | 0 | 14 (10–17) |
| 36 | 558 (305–712) | %19 (%15–%23) | 0 | 14,5 (10–17) |
| 37 | 566 (428–772) | %19 (%16–%22) | 0 | 14 (10,5–17,5) |
| 38 | 581 (273–708) | %18 (%16–%21) | 0 | 14 (11–18) |
| 39 | 557 (376–744) | %18 (%16–%22) | 0 | 14,5 (11–18,5) |
| 40 | 620 (261–770) | %18 (%16–%23) | 0 | 15 (10,5–19,5) |
| 41 | 524 (325–672) | %19 (%16–%21) | 0 | 15 (10,5–19,5) |
| 42 | 616 (414–833) | %19 (%14–%23) | 0 | 15 (10,5–19,5) |
| 43 | 556 (365–865) | %17 (%15–%20) | 0 | 15 (11–19,5) |
| 44 | 696 (412–859) | %18 (%15–%21) | 0 | 15 (11–20) |
| 45 | 593 (353–883) | %18 (%15–%22) | 0 | 15 (11–20) |
| 46 | 618 (270–814) | %17 (%15–%22) | 0 | 15,5 (11,5–20,5) |
| 47 | 624 (252–785) | %17 (%16–%22) | 0 (0–1) | 15,5 (11,5–21) |
| 48 | 619 (336–885) | %17 (%14–%21) | 0 | 15,5 (12–21,5) |
| 49 | 609 (446–844) | %16 (%14–%20) | 0 | 15,5 (12–22) |
| 50 | 648 (532–807) | %17 (%15–%21) | 0 | 15,5 (12,5–23) |
| 51 | 689 (409–961) | %16 (%14–%21) | 0 | 15,5 (12,5–23,5) |
| 52 | 729 (346–947) | %18 (%13–%21) | 0 | 16 (13–23,5) |
| 53 | 703 (433–1009) | %17 (%13–%20) | 0 | 16,5 (13–23,5) |
| 54 | 729 (578–963) | %17 (%14–%20) | 0 | 17 (13,5–23,5) |
| 55 | 680 (379–999) | %18 (%14–%20) | 0 (0–1) | 17 (13,5–24) |
| 56 | 730 (541–978) | %17 (%13–%20) | 0 | 17,5 (14–24) |
| 57 | 784 (416–1021) | %17 (%14–%21) | 0 | 18 (13,5–24,5) |
| 58 | 709 (432–1093) | %18 (%13–%20) | 0 | 17,5 (14,5–25) |
| 59 | 716 (401–902) | %16 (%13–%20) | 0 | 18 (14,5–25,5) |
| 60 | 694 (409–1082) | %17 (%14–%20) | 0 | 18 (15–26) |

### Olaylar

| Yıl | Olay | Büyük olay |
|---|---|---|
| 1 | 48,5 (39–58) | 15 (10,5–19,5) |
| 2 | 60 (42–65) | 22,5 (16–26,5) |
| 3 | 55,5 (40,5–79,5) | 19,5 (14–35,5) |
| 4 | 79 (59,5–93) | 27,5 (19,5–32,5) |
| 5 | 82,5 (64,5–104) | 26,5 (18,5–36) |
| 6 | 92 (71,5–113) | 34 (23,5–40,5) |
| 7 | 111 (80,5–148) | 39,5 (27–54,5) |
| 8 | 132 (106–176) | 41,5 (35,5–53,5) |
| 9 | 157 (122–176) | 43,5 (34–59) |
| 10 | 156 (120–206) | 41,5 (29–63) |
| 11 | 186 (117–200) | 53 (28–64) |
| 12 | 175 (132–214) | 51,5 (36,5–66) |
| 13 | 172 (124–218) | 46 (27,5–71) |
| 14 | 174 (133–212) | 49 (40,5–61) |
| 15 | 168 (134–215) | 46,5 (32–61,5) |
| 16 | 178 (122–210) | 49,5 (35,5–72) |
| 17 | 163 (120–200) | 46 (32–55) |
| 18 | 163 (116–192) | 42 (30,5–55,5) |
| 19 | 160 (126–228) | 47 (39,5–65) |
| 20 | 149 (106–208) | 42,5 (25–62,5) |
| 21 | 156 (115–199) | 41 (32,5–59,5) |
| 22 | 157 (112–209) | 44 (30,5–64) |
| 23 | 173 (133–232) | 52,5 (37–66) |
| 24 | 184 (136–194) | 46 (33–59,5) |
| 25 | 172 (124–216) | 52 (31,5–63,5) |
| 26 | 170 (116–218) | 50 (32–75) |
| 27 | 160 (122–194) | 42 (36–60) |
| 28 | 162 (122–206) | 48 (33–71,5) |
| 29 | 150 (120–210) | 47 (35,5–61) |
| 30 | 166 (132–206) | 44,5 (37,5–65,5) |
| 31 | 150 (116–212) | 42,5 (32,5–76,5) |
| 32 | 166 (99–208) | 45 (26–65,5) |
| 33 | 156 (114–210) | 50,5 (37,5–59,5) |
| 34 | 152 (123–206) | 43 (33,5–67) |
| 35 | 151 (116–242) | 49 (35–79,5) |
| 36 | 155 (102–229) | 45 (29–70,5) |
| 37 | 172 (123–240) | 50,5 (37–75) |
| 38 | 154 (118–187) | 45 (33,5–65,5) |
| 39 | 157 (104–206) | 50,5 (26–70) |
| 40 | 154 (95,5–232) | 47,5 (20,5–63,5) |
| 41 | 150 (104–226) | 48 (29–68) |
| 42 | 163 (110–196) | 54 (37,5–67) |
| 43 | 154 (132–206) | 49,5 (26–76,5) |
| 44 | 162 (133–198) | 52 (41–65) |
| 45 | 175 (126–216) | 52 (32–73,5) |
| 46 | 158 (124–210) | 47,5 (35,5–66,5) |
| 47 | 188 (146–238) | 62,5 (46–80,5) |
| 48 | 164 (124–222) | 46 (30,5–68,5) |
| 49 | 174 (142–216) | 48,5 (31–75) |
| 50 | 164 (127–242) | 54 (36–75,5) |
| 51 | 165 (136–244) | 53 (43–82) |
| 52 | 172 (136–224) | 51,5 (41,5–67) |
| 53 | 162 (131–262) | 50,5 (26,5–83) |
| 54 | 185 (136–272) | 56,5 (45–78) |
| 55 | 174 (131–218) | 55 (44–76,5) |
| 56 | 171 (144–230) | 51 (36–64,5) |
| 57 | 180 (128–226) | 53 (42–78) |
| 58 | 164 (127–222) | 51,5 (39–74) |
| 59 | 181 (135–242) | 55 (34,5–81) |
| 60 | 176 (146–262) | 56 (37,5–68,5) |

### Savaş (1/2)

| Yıl | Muharebe | Başlayan savaş | Süren savaş (yıl sonu) | Yıl içinde süren savaş | Yağma akını (medeniyet) | Tarihî hak savaşı |
|---|---|---|---|---|---|---|
| 1 | 0 | 0 | 0 | 0 | 0 | 0 |
| 2 | 0 | 0 | 0 | 0 | 0 | 0 |
| 3 | 0 (0–1) | 0 | 0 | 0 | 0 | 0 |
| 4 | 3 (3–4) | 0 | 0 | 0 | 0 | 0 |
| 5 | 4,5 (3–9) | 0 | 0 | 0 | 0 | 0 |
| 6 | 6 (4–8,5) | 0 (0–1) | 0 | 0 (0–1) | 0 (0–1,5) | 0 |
| 7 | 7 (4–11) | 0 (0–1) | 0 (0–0,5) | 0 (0–1) | 0 (0–1) | 0 |
| 8 | 8 (5–11) | 0 (0–1,5) | 0 | 0 (0–2) | 0 (0–1,5) | 0 |
| 9 | 9 (6–11,5) | 0 (0–1,5) | 0 | 0 (0–1,5) | 0,5 (0–1) | 0 |
| 10 | 10 (6,5–12) | 0,5 (0–2) | 0 (0–0,5) | 1 (0–2) | 0 (0–2) | 0 (0–0,5) |
| 11 | 10 (5,5–12) | 0 (0–1,5) | 0 (0–0,5) | 0 (0–1,5) | 0 (0–2) | 0 |
| 12 | 8,5 (6–10) | 1 (0–2) | 0 (0–1) | 1 (0–2,5) | 0,5 (0–2) | 0 (0–0,5) |
| 13 | 6 (2,5–11,5) | 0,5 (0–1) | 0 (0–1) | 1 (0–2) | 0 (0–1,5) | 0 |
| 14 | 7,5 (6–9,5) | 0,5 (0–1) | 0 (0–0,5) | 1 (0–1,5) | 0,5 (0–1,5) | 0 (0–0,5) |
| 15 | 7,5 (4–11,5) | 1 (0–3) | 0 (0–1) | 1 (0–3) | 0,5 (0–1,5) | 0 (0–0,5) |
| 16 | 7 (5–10,5) | 0,5 (0–2) | 0 (0–1) | 1 (0–2) | 0 (0–2,5) | 0 |
| 17 | 7 (2,5–9,5) | 1 (0–2) | 0 (0–2) | 1 (0,5–2) | 1 (0–2) | 0 |
| 18 | 7 (3,5–9) | 0 (0–2) | 0 (0–1) | 1 (0–3) | 1 (0–2) | 0 |
| 19 | 7 (3,5–11) | 1 (1–2) | 0 (0–0,5) | 1 (1–2,5) | 0,5 (0–3) | 0 |
| 20 | 7,5 (5–10) | 0,5 (0–2,5) | 0 (0–1,5) | 1 (0–2,5) | 1 (0–2,5) | 0 (0–1) |
| 21 | 7 (5,5–10,5) | 2 (0,5–2,5) | 0 (0–1) | 2 (1–4) | 0,5 (0–2,5) | 0 (0–1) |
| 22 | 7,5 (4,5–12,5) | 0 (0–1) | 0 (0–1) | 1 (0–2) | 2 (0–3) | 0 (0–1) |
| 23 | 9 (5–11) | 2 (1–3,5) | 0 (0–1,5) | 2 (1–4) | 2 (0–2,5) | 0 (0–1) |
| 24 | 7 (4,5–11) | 1 (0–2,5) | 0 (0–1) | 1 (0–3,5) | 1,5 (0–2,5) | 0 (0–0,5) |
| 25 | 7,5 (3,5–11,5) | 1,5 (0–3) | 0 (0–2) | 2 (0–4) | 2 (0,5–3) | 0 (0–1) |
| 26 | 8 (5–12,5) | 1 (0–3,5) | 0 (0–1,5) | 2 (1–4,5) | 1 (0–3) | 0 (0–1) |
| 27 | 8,5 (6,5–11) | 1 (0–2,5) | 0 (0–1) | 1 (0–4) | 2 (0–3) | 0 (0–1) |
| 28 | 8,5 (5–12) | 2 (0–3) | 0 (0–1,5) | 2 (0–4) | 1 (0–3,5) | 0 (0–0,5) |
| 29 | 9 (6–11,5) | 0 (0–3) | 0 | 0,5 (0–3,5) | 2 (0–3) | 0 (0–0,5) |
| 30 | 9 (5–17) | 1 (0–4) | 0 | 1 (0–4,5) | 1,5 (0–3,5) | 0 (0–0,5) |
| 31 | 9,5 (4,5–13) | 2 (0–3) | 0 (0–1) | 2 (0–3) | 1,5 (0–3) | 0 (0–1) |
| 32 | 9,5 (6–14) | 1,5 (0–4,5) | 0 (0–1,5) | 2 (0–4,5) | 2 (0–3) | 0 (0–1) |
| 33 | 10 (7,5–12) | 1 (0–3) | 0 (0–1,5) | 2 (0–3,5) | 1 (0–2,5) | 0 (0–0,5) |
| 34 | 9,5 (6–13) | 1 (0–2) | 0 (0–2) | 1,5 (0–4) | 2 (0–3) | 0 (0–1) |
| 35 | 10 (7–11,5) | 1 (0–3,5) | 0 (0–2) | 2 (0–4) | 2 (0–3,5) | 0 (0–1) |
| 36 | 8,5 (5–12,5) | 2 (0–3,5) | 0 (0–2) | 2 (0,5–4) | 1,5 (0–3) | 0 (0–1) |
| 37 | 10,5 (6–15) | 1,5 (0–3) | 0,5 (0–2,5) | 2 (0–3,5) | 2 (0–3) | 0 |
| 38 | 7,5 (4–11) | 1 (0–2) | 0 (0–1,5) | 2 (1–5) | 1,5 (0–2,5) | 0 (0–0,5) |
| 39 | 8,5 (6–11) | 1 (0–3) | 0 (0–2) | 1 (0–3,5) | 2 (0–2,5) | 0 (0–1) |
| 40 | 9 (4–12,5) | 1 (0–3) | 0 (0–1,5) | 2 (0–4) | 1 (0–3) | 0 (0–0,5) |
| 41 | 9 (6–12,5) | 0,5 (0–2,5) | 0 (0–1) | 1 (0–4) | 1,5 (0–3) | 0 |
| 42 | 9 (5,5–12) | 1 (0–3,5) | 0 (0–1) | 1 (0–4) | 2 (0–3) | 0 (0–0,5) |
| 43 | 8,5 (5–12) | 2 (0–2) | 0 (0–2) | 2 (0–3,5) | 1 (0–2,5) | 0 |
| 44 | 9 (6,5–13) | 0,5 (0–3,5) | 0 (0–0,5) | 2 (0–3,5) | 2 (0–3) | 0 (0–0,5) |
| 45 | 10 (6–14,5) | 1 (0–4) | 0 (0–1) | 1 (0–4) | 1 (0–3) | 0 |
| 46 | 9,5 (5–13) | 1 (0–3) | 0 (0–1,5) | 1,5 (0–3) | 2 (0–2,5) | 0 |
| 47 | 10,5 (7–15) | 2 (0–4,5) | 0 (0–1,5) | 2,5 (0–5) | 2 (0–3) | 0 (0–0,5) |
| 48 | 8 (5–11) | 1 (0–3) | 1 (0–2) | 1,5 (0–4) | 1,5 (0–3) | 0 |
| 49 | 8,5 (5,5–11) | 1,5 (0–4) | 0 (0–2) | 2 (0–5,5) | 1,5 (0–3) | 0 (0–0,5) |
| 50 | 8,5 (4,5–12) | 0,5 (0–3) | 0 (0–1,5) | 2 (0–3,5) | 1,5 (0–3) | 0 |
| 51 | 9,5 (6,5–12) | 1,5 (0–3,5) | 0 (0–1,5) | 2 (0–4) | 1 (0–3,5) | 0 |
| 52 | 9 (7–12) | 0,5 (0–3,5) | 0 | 1,5 (0–3,5) | 2 (0–3) | 0 |
| 53 | 8 (4,5–14) | 2 (0–4) | 0 (0–1,5) | 2 (0–4) | 1 (0–4) | 0 (0–1) |
| 54 | 11 (6,5–16,5) | 1 (0–2,5) | 0 (0–1) | 2 (0–3,5) | 2 (0–3) | 0 |
| 55 | 8,5 (6–13) | 0,5 (0–4) | 0 (0–2) | 1 (0–4) | 1,5 (0–3) | 0 |
| 56 | 8,5 (5–13) | 1 (0–2,5) | 0 (0–0,5) | 2 (0–3) | 2 (0–3,5) | 0 (0–0,5) |
| 57 | 9 (7–14) | 2 (0–2,5) | 0 (0–1,5) | 2 (0–3,5) | 1,5 (0–4) | 0 |
| 58 | 7,5 (6–13) | 2 (0–4) | 0 (0–1,5) | 2 (0–4) | 1 (0–2,5) | 0 (0–0,5) |
| 59 | 10 (7,5–12) | 0,5 (0–2) | 0 (0–1) | 1,5 (0–3) | 1 (0–3,5) | 0 |
| 60 | 8 (6,5–13) | 2 (0–3) | 0 (0–2) | 2 (0–3) | 1 (0–3) | 0 (0–0,5) |

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
| 11 | 0 | 0 (0–0,5) | 0 | 0 (0–0,5) |
| 12 | 0 | 0 | 0 | 0 (0–1) |
| 13 | 0 | 0 | 0 | 0 (0–1) |
| 14 | 0 | 0 | 0 | 0 (0–1) |
| 15 | 0 | 0 | 0 | 0 (0–1) |
| 16 | 0 | 0 | 0 | 0 (0–1,5) |
| 17 | 0 (0–0,5) | 0 | 0 | 1 (0–1,5) |
| 18 | 0 | 0 | 0 | 0,5 (0–1,5) |
| 19 | 0 | 0 (0–1) | 0 | 1 (0–1,5) |
| 20 | 0 (0–1) | 0 | 0 | 1 (0–1,5) |
| 21 | 0 (0–1) | 0 | 0 (0–0,5) | 1 (0–2) |
| 22 | 0 | 0 | 0 | 1 (0–2) |
| 23 | 0 (0–0,5) | 0 | 0 | 1 (0–1,5) |
| 24 | 0 (0–1) | 0 | 0 | 1 (0–1,5) |
| 25 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 26 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 27 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 28 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 29 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 30 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 31 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 32 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 33 | 0 (0–1) | 0 | 0 | 1 (0–2,5) |
| 34 | 0 (0–1) | 0 | 0 | 1 (0–2,5) |
| 35 | 0 (0–1) | 0 | 0 | 1 (0–2,5) |
| 36 | 0 (0–1) | 0 | 0 | 1 (0–2,5) |
| 37 | 0 (0–0,5) | 0 (0–0,5) | 0 | 1 (0–2,5) |
| 38 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 39 | 0 | 0 | 0 | 1 (0–2) |
| 40 | 0 (0–1) | 0 | 0 | 1 (0–2) |
| 41 | 0 (0–0,5) | 0 | 0 | 1 (0–2) |
| 42 | 0 (0–0,5) | 0 | 0 | 1,5 (0–2,5) |
| 43 | 0 (0–1) | 0 | 0 | 1,5 (0,5–2,5) |
| 44 | 0 (0–1) | 0 (0–0,5) | 0 (0–0,5) | 1 (0–2,5) |
| 45 | 0 (0–1) | 0 | 0 | 1 (0–2,5) |
| 46 | 0 | 0 | 0 | 1 (0–2,5) |
| 47 | 0 (0–1) | 0 (0–1) | 0 | 1 (0–2,5) |
| 48 | 0 (0–1) | 0 | 0 | 1,5 (0,5–2,5) |
| 49 | 0 (0–1) | 0 | 0 | 1,5 (0,5–3) |
| 50 | 0 (0–0,5) | 0 | 0 | 1,5 (0,5–3) |
| 51 | 0 (0–1) | 0 | 0 | 1,5 (0,5–2,5) |
| 52 | 0 (0–1) | 0 | 0 | 1,5 (0,5–2,5) |
| 53 | 0 (0–1) | 0 (0–1) | 0 | 1,5 (0,5–2,5) |
| 54 | 0 (0–1) | 0 | 0 | 2 (0,5–2,5) |
| 55 | 0 (0–1) | 0 | 0 | 2 (0,5–2,5) |
| 56 | 0 (0–1) | 0 | 0 | 2 (0,5–2,5) |
| 57 | 0 (0–1) | 0 | 0 | 2 (0,5–2,5) |
| 58 | 0 (0–1,5) | 0 | 0 | 2 (0,5–2,5) |
| 59 | 0 | 0 | 0 | 2 (0,5–2,5) |
| 60 | 0 (0–1) | 0 | 0 (0–0,5) | 1,5 (0,5–2,5) |

### Canavarlar (1/2)

| Yıl | Yaşayan kamp (yıl sonu) | Yaşayan kamp (yıl ort.) | Doğan kamp | Temizlenen kamp | Canavar baskını | Yaşayan trol ini (yıl sonu) |
|---|---|---|---|---|---|---|
| 1 | 3 | 3 | 0 | 0 | 0 | 0 |
| 2 | 3 (3–4,5) | 3 (3–3,43) | 0 (0–1,5) | 0 | 0 | 0 |
| 3 | 5 (4–6) | 4,07 (3,48–5,16) | 2 (0–2) | 0 | 0 (0–1) | 0 |
| 4 | 6 (4,5–9) | 5,02 (4,11–6,75) | 1 (0–3) | 0 | 3 (2–4) | 0 |
| 5 | 8 (6,5–10) | 7,24 (5,44–9,41) | 2 (0,5–3,5) | 0 | 4 (3–8,5) | 0 |
| 6 | 9 (8–11) | 8,3 (7,24–10,1) | 2 (0–2) | 0 (0–0,5) | 5 (4–7) | 0 |
| 7 | 10 (8,5–11) | 9,77 (8,4–11) | 1 (0–2) | 0 (0–1,5) | 6 (4–8) | 0 |
| 8 | 10 (9–12) | 9,77 (8,4–11,4) | 1 (0–2) | 0 (0–2) | 5,5 (3,5–9) | 0 |
| 9 | 10 (8,5–11) | 9,98 (8,84–11,4) | 1 (0–2) | 1 (0–2,5) | 6 (3,5–7,5) | 0 |
| 10 | 9 (7,5–10,5) | 9,92 (8,18–10,8) | 1 (0–2) | 1,5 (0–2,5) | 6,5 (3,5–9) | 0 |
| 11 | 8,5 (6–10) | 8,91 (6,78–10,4) | 1 (0–2) | 2 (1–3) | 5,5 (2,5–8) | 0 |
| 12 | 8 (5–9,5) | 8,2 (6,1–9,83) | 1 (0,5–3) | 1 (0–4) | 4 (3–7) | 0 |
| 13 | 8 (5,5–10) | 8,07 (5,4–9,79) | 2 (1–3) | 2 (0–3,5) | 3 (1–5) | 0 |
| 14 | 7 (5–9,5) | 7,29 (5,12–9,17) | 2 (0–3) | 2 (1–3,5) | 3 (2–6) | 0 |
| 15 | 7 (5,5–9) | 7,15 (5,45–9,15) | 1,5 (0–3) | 1 (0–3) | 4 (2–5,5) | 0 |
| 16 | 7 (4–8,5) | 6,91 (5,05–8,42) | 1,5 (0,5–3) | 2 (1–3) | 4 (2–5) | 0 (0–0,5) |
| 17 | 6 (5–8) | 6,43 (4,5–8,54) | 2 (0,5–3) | 2 (0,5–4) | 3 (1,5–4,5) | 0 (0–1) |
| 18 | 6 (5,5–8,5) | 6,55 (5,1–8,56) | 2 (1–3,5) | 1,5 (1–3) | 2,5 (1,5–3,5) | 1 (0–1) |
| 19 | 8 (5,5–9,5) | 7,2 (5,63–9,03) | 2 (1–3,5) | 1 (0–2,5) | 3 (0,5–5,5) | 1 (0–1,5) |
| 20 | 7 (6–9) | 7,48 (6,29–8,72) | 1 (0–4) | 2 (0,5–4) | 3 (2–5,5) | 1 (0–2) |
| 21 | 8 (6,5–9,5) | 7,43 (6,63–8,94) | 2 (0,5–3) | 1 (1–2,5) | 3 (2–4,5) | 1 (0–2) |
| 22 | 8 (6,5–8,5) | 7,68 (6,11–8,8) | 2 (0,5–3) | 1,5 (1–3,5) | 3 (1–6) | 0,5 (0–2) |
| 23 | 7,5 (6–9) | 7,04 (6,48–9,02) | 2 (1–3) | 2 (0,5–3) | 3,5 (1,5–5,5) | 0,5 (0–1,5) |
| 24 | 7 (6–9,5) | 7,25 (6,27–9,2) | 2 (1–2,5) | 2 (0–3) | 3 (2–5,5) | 0 (0–2) |
| 25 | 7 (6,5–9,5) | 7,54 (6,15–9,29) | 2 (0,5–3) | 1 (1–3) | 3 (1–4) | 1 (0–2,5) |
| 26 | 7 (6,5–10) | 7,48 (6,62–9,25) | 2 (1–3) | 2 (0,5–3) | 2 (2–5,5) | 1 (0–2) |
| 27 | 8 (6–9) | 7,33 (6,46–9,02) | 2 (0,5–3,5) | 2 (1,5–3) | 3 (2–4,5) | 1 (0–2) |
| 28 | 8 (6,5–10) | 8,27 (7,2–8,95) | 2 (0,5–3) | 1 (0–3) | 4 (1,5–6) | 1 (0–2) |
| 29 | 9 (6,5–9,5) | 8,05 (6,67–9,58) | 2 (1–3) | 2 (0,5–3) | 3 (1,5–5) | 1 (0–2,5) |
| 30 | 8 (7–10,5) | 8,44 (7,08–10,2) | 2 (1–2) | 2 (0,5–3,5) | 4,5 (2–6) | 1 (0–2,5) |
| 31 | 8,5 (7–10) | 8,55 (7,08–9,63) | 2 (1–4) | 2 (0–4,5) | 3,5 (2–5,5) | 1 (0–2) |
| 32 | 9,5 (7,5–10) | 8,7 (7,7–9,92) | 2 (0–3,5) | 2 (0–3,5) | 3 (1,5–6) | 1 (0–2,5) |
| 33 | 8,5 (6–11) | 9,03 (6,75–10,3) | 2 (1–3,5) | 3 (1–4) | 4 (2–6) | 1 (0–2,5) |
| 34 | 8 (6,5–10) | 8,37 (7,09–10,3) | 1 (0,5–3) | 2 (1–3,5) | 4 (2–5,5) | 1 (0–3) |
| 35 | 9 (6,5–10) | 8,68 (7,09–9,75) | 1,5 (0–3,5) | 1 (0–2,5) | 4,5 (3–5,5) | 1 (0–2,5) |
| 36 | 9 (7–11,5) | 8,98 (6,93–10,6) | 2,5 (1–3) | 2 (1–4) | 3 (1–5,5) | 1,5 (1–2,5) |
| 37 | 8,5 (7–11) | 9,05 (7,05–11,1) | 2 (1–4,5) | 2 (1–4) | 5 (1,5–7) | 1 (1–2,5) |
| 38 | 9 (6,5–10,5) | 8,67 (7,18–10,4) | 2 (0,5–3) | 1 (0–3,5) | 3 (1–5) | 1 (0–3) |
| 39 | 9 (6,5–11) | 9,32 (6,84–10,6) | 2 (0,5–3) | 2,5 (1–3) | 3 (2–5,5) | 1,5 (0–3) |
| 40 | 9 (6,5–11) | 9 (6,98–11,1) | 2 (0–3) | 1,5 (0,5–3,5) | 4 (1,5–6) | 2 (0,5–3) |
| 41 | 10 (7–11,5) | 9,39 (7,12–11,1) | 2 (0,5–3) | 1 (1–3,5) | 4 (3–6) | 2 (1–3) |
| 42 | 10 (7–12) | 9,37 (7,3–11,5) | 2,5 (1,5–4,5) | 2 (2–3) | 3,5 (2–5,5) | 2 (1–3) |
| 43 | 10 (7–12) | 9,87 (7,09–12,3) | 2 (0,5–3,5) | 2 (0–4) | 4 (2–5) | 2 (0,5–3) |
| 44 | 10 (6–12,5) | 10,4 (7,18–12,7) | 2 (1–4,5) | 2 (1–4) | 3,5 (2–6,5) | 1 (1–3) |
| 45 | 9,5 (7,5–13) | 9,93 (7,16–12,5) | 2 (1–3) | 2 (1–3,5) | 4 (2–7) | 1 (0–3) |
| 46 | 10 (7–11,5) | 9,59 (7,24–12,1) | 2 (1–3) | 2 (1–3) | 4 (1,5–6) | 2 (0–2,5) |
| 47 | 9 (7,5–11,5) | 9,74 (7,88–11,8) | 2 (1–3) | 2 (1–3,5) | 4,5 (2–6,5) | 2 (0–3) |
| 48 | 9 (6,5–14) | 8,82 (7,35–12,5) | 2 (1,5–4) | 1,5 (0–4) | 3 (2–5) | 2 (0,5–3) |
| 49 | 10 (8–14) | 9,21 (6,91–14) | 2 (1–3) | 1 (0–2) | 3,5 (2–5) | 2 (1–3) |
| 50 | 10 (7,5–13) | 9,96 (7,8–13,5) | 2 (0,5–3,5) | 2 (1–4,5) | 3 (2–5) | 2 (0–3) |
| 51 | 11 (7,5–13) | 10,5 (7,55–13,1) | 2 (1–4) | 2 (0–3) | 4 (1,5–5) | 2 (1–3) |
| 52 | 10,5 (7–14) | 10,8 (7,3–13,2) | 2 (1–3,5) | 2 (0,5–3,5) | 4 (2–6) | 2 (0–3) |
| 53 | 11 (8,5–13,5) | 10,9 (7,48–13,7) | 2 (0–3,5) | 2 (1–3) | 3 (1,5–6,5) | 2 (0,5–3) |
| 54 | 11 (7–14,5) | 11,3 (8,01–13,8) | 2 (1–5) | 2 (1–4) | 4 (2–6) | 1 (1–3) |
| 55 | 11 (8,5–13,5) | 11 (7,89–14,3) | 2 (1–4) | 2 (1–4) | 3,5 (2–6,5) | 2 (0,5–3) |
| 56 | 11 (7,5–14,5) | 10,9 (7,65–13,2) | 2 (1–2,5) | 1,5 (1–3,5) | 3,5 (2–5,5) | 2 (1–3) |
| 57 | 11 (8,5–13) | 11,2 (7,56–13,8) | 2 (0,5–3) | 2 (1–3,5) | 4 (1,5–7,5) | 2 (0,5–3) |
| 58 | 11,5 (9–13,5) | 10,6 (8,3–13,3) | 2,5 (1–3) | 2 (1–4) | 3 (1–6) | 2 (1–3) |
| 59 | 11 (8,5–14) | 11,4 (8,3–13,5) | 2 (1–3) | 2 (1–4) | 4,5 (2,5–7) | 2 (0,5–3) |
| 60 | 11 (8–15,5) | 11,3 (8,63–14,2) | 2,5 (1–4) | 2 (1–3) | 4,5 (1,5–6,5) | 2 (0,5–3) |

### Canavarlar (2/2)

| Yıl | Yaşayan ejderha (yıl sonu) | Ejderha akını | Kriz (anlatıcı) | Rahatlama dönemi (anlatıcı) |
|---|---|---|---|---|
| 1 | 0 | 0 | 0 | 0 |
| 2 | 0 | 0 | 0 | 0 |
| 3 | 0 | 0 | 0 | 0 |
| 4 | 0 | 0 | 0 | 0 |
| 5 | 0 | 0 | 0 (0–1) | 1 (0–1) |
| 6 | 0 | 0 | 0 (0–0,5) | 0 (0–1) |
| 7 | 0 | 0 | 0 (0–0,5) | 0 (0–1) |
| 8 | 0 | 0 | 0 (0–1) | 0 (0–1) |
| 9 | 0 | 0 | 0 | 0 (0–1) |
| 10 | 0 | 0 | 0 (0–0,5) | 0 (0–1) |
| 11 | 0 | 0 | 0 | 0 (0–1) |
| 12 | 0 | 0 | 0 (0–1) | 0 |
| 13 | 0 | 0 | 0 (0–1) | 0 |
| 14 | 0 | 0 | 0 (0–1) | 0 |
| 15 | 0 | 0 | 0 (0–1) | 0 |
| 16 | 0 | 0 | 0 (0–1) | 0 |
| 17 | 0 | 0 | 0,5 (0–1) | 0 |
| 18 | 0 | 0 | 0 (0–1) | 0 |
| 19 | 0 (0–1) | 0 (0–1) | 1 (0–1) | 0 |
| 20 | 1 (0–1) | 0 (0–1) | 0 (0–1) | 0 |
| 21 | 1 (0,5–1) | 1 (0–1,5) | 1 (0–1) | 0 (0–1) |
| 22 | 1 | 1 (0–1,5) | 0 (0–1) | 0 (0–0,5) |
| 23 | 1 | 1 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 24 | 1 | 1 (0,5–2) | 0 (0–1) | 0 |
| 25 | 1 | 1 (0–1) | 0 (0–1) | 0 (0–1) |
| 26 | 1 | 1 (0–1) | 0 (0–1) | 0 |
| 27 | 1 | 1 (0,5–1,5) | 0 (0–1) | 0 (0–0,5) |
| 28 | 1 | 1 (0–2) | 0 (0–1) | 0 (0–1) |
| 29 | 1 | 1 (0–1) | 0 (0–1) | 0 (0–0,5) |
| 30 | 1 | 1 (0,5–1,5) | 0 (0–1) | 0 |
| 31 | 1 | 1 (0–1,5) | 0 (0–1) | 0 (0–1) |
| 32 | 1 | 1 (0–1,5) | 0 (0–1) | 0 |
| 33 | 1 | 1 (0–1,5) | 0 (0–1) | 0 |
| 34 | 1 | 1 (0–2) | 0 (0–1) | 0 |
| 35 | 1 | 1 (0–2) | 0 (0–1) | 0 (0–1) |
| 36 | 1 | 0,5 (0–1) | 0 (0–1) | 0 |
| 37 | 1 | 1 (0–1) | 0 (0–1) | 0 |
| 38 | 1 | 0,5 (0–1) | 0,5 (0–1) | 0 |
| 39 | 1 | 1 (0–1) | 0 (0–1) | 0 |
| 40 | 1 | 1 (0–1) | 0 (0–1) | 0 (0–0,5) |
| 41 | 1 | 1 (0–1,5) | 0 (0–1) | 0 (0–0,5) |
| 42 | 1 | 1 (0–1) | 0 (0–1) | 0 |
| 43 | 1 | 0,5 (0–2) | 0 (0–1) | 0 |
| 44 | 1 | 1 (0–1) | 1 (0–1) | 0 |
| 45 | 1 | 1 (0–1) | 0 (0–1) | 0 |
| 46 | 1 | 1 (0–2) | 0 (0–1) | 0 |
| 47 | 1 | 1 (0–1,5) | 0 (0–1) | 0 |
| 48 | 1 | 1 (0–1,5) | 0 (0–1) | 0 |
| 49 | 1 | 1 (0–1,5) | 0 (0–1) | 0 |
| 50 | 1 | 1 (0–1) | 0 (0–1) | 0 (0–1) |
| 51 | 1 | 1 (0–1,5) | 0 (0–1) | 0 |
| 52 | 1 | 0,5 (0–1,5) | 0 (0–1) | 0 |
| 53 | 1 | 0 (0–1) | 0 (0–1) | 0 |
| 54 | 1 | 1 (0–2) | 0 (0–1) | 0 (0–0,5) |
| 55 | 1 | 1 (0–2) | 0 (0–1) | 0 |
| 56 | 1 | 1 (0–1) | 0 (0–1) | 0 (0–0,5) |
| 57 | 1 | 1 (0–1) | 0 (0–1) | 0 |
| 58 | 1 | 1 (0–1,5) | 0 (0–1) | 0 |
| 59 | 1 | 1 (0–1) | 0 (0–1) | 0 |
| 60 | 1 (0,5–1) | 0,5 (0–1,5) | 0,5 (0–1) | 0 (0–1) |

### Kahramanlar (1/2)

| Yıl | Doğan kahraman | Ölen kahraman | Emekli olan kahraman | Diyarı terk eden kahraman | Ölümden dönen kahraman | Efsane olan kahraman |
|---|---|---|---|---|---|---|
| 1 | 0 (0–1,5) | 0 | 0 | 0 | 0 | 0 |
| 2 | 0,5 (0–1) | 0 | 0 | 0 | 0 | 0 |
| 3 | 1,5 (0–3) | 0 | 0 | 0 | 0 | 0 |
| 4 | 2 (1–3) | 0 (0–0,5) | 0 | 0 (0–0,5) | 0 | 0 |
| 5 | 3 (1–5,5) | 0 (0–1,5) | 0 | 0 | 0 | 0 |
| 6 | 3 (1–4,5) | 0 (0–1) | 0 | 0 | 0 | 0 |
| 7 | 5 (1,5–6,5) | 0 (0–3) | 0 | 0 | 0 | 0 |
| 8 | 4 (2–6,5) | 0 (0–1) | 0 | 0 | 0 | 0 |
| 9 | 3 (1–4,5) | 1 (0–2,5) | 0 | 0 | 0 | 0 |
| 10 | 4 (3–6) | 0,5 (0–1,5) | 0 | 0 (0–0,5) | 0 | 0 |
| 11 | 2 (0–5) | 1 (0–2) | 0 | 0 (0–0,5) | 0 | 0 |
| 12 | 2,5 (1–5,5) | 0 (0–2) | 0 | 0 | 0 | 0 |
| 13 | 2 (1–5) | 0 (0–4,5) | 0 | 0 | 0 | 0 |
| 14 | 2 (1–5,5) | 0 (0–2) | 0 | 0 | 0 | 0 (0–0,5) |
| 15 | 3,5 (0–6) | 0 (0–2) | 0 | 0 | 0 | 0 |
| 16 | 3 (1–6,5) | 1 (0–3,5) | 0 | 0 | 0 | 0 |
| 17 | 2 (0–4) | 0 (0–2) | 0 | 0 (0–1) | 0 | 0 |
| 18 | 2 (0,5–3) | 0 (0–1) | 0 | 0 | 0 | 0 |
| 19 | 1,5 (0–6) | 0 (0–2) | 0 | 0 (0–0,5) | 0 | 0 |
| 20 | 3 (0,5–4,5) | 0 (0–1,5) | 0 | 0 | 0 | 0 |
| 21 | 3 (1,5–4) | 0 (0–1) | 0 | 0 | 0 | 0 |
| 22 | 2 (0,5–6,5) | 0 (0–1,5) | 0 | 0 | 0 | 0 |
| 23 | 3,5 (1–5) | 0 (0–2) | 0 | 0 (0–1) | 0 | 0 |
| 24 | 3 (1,5–7) | 0 (0–3,5) | 0 | 0 | 0 | 0 (0–0,5) |
| 25 | 3 (1,5–4,5) | 0 (0–2,5) | 0 | 0 (0–0,5) | 0 | 0 (0–0,5) |
| 26 | 3 (1–7) | 0,5 (0–4,5) | 0 | 0 (0–1) | 0 | 0 |
| 27 | 2,5 (1–5,5) | 0 (0–4) | 0 | 0 (0–1) | 0 | 0 |
| 28 | 3 (1–5,5) | 1 (0–3,5) | 0 | 0 (0–1) | 0 | 0 |
| 29 | 3 (0,5–4,5) | 1 (0–3) | 0 (0–0,5) | 0 (0–0,5) | 0 | 0 (0–0,5) |
| 30 | 3 (1–5,5) | 1 (0–2,5) | 0 (0–1) | 0 (0–1) | 0 | 0 (0–0,5) |
| 31 | 2,5 (1–6,5) | 1 (0–5,5) | 0 (0–1) | 0 (0–1) | 0 | 0 |
| 32 | 2 (1–5,5) | 0 (0–3) | 0 | 0 (0–1) | 0 | 0 (0–0,5) |
| 33 | 2 (1,5–5) | 0,5 (0–2) | 0 (0–1) | 0 (0–1) | 0 | 0 (0–0,5) |
| 34 | 2,5 (0,5–6,5) | 1 (0–2,5) | 1 (0–1,5) | 0 | 0 | 0 |
| 35 | 3,5 (0,5–6,5) | 0,5 (0–4) | 0 (0–1) | 0 (0–1) | 0 | 0 (0–1) |
| 36 | 2 (1–5) | 0 (0–5) | 0 (0–1,5) | 0 (0–1) | 0 | 0 (0–0,5) |
| 37 | 3,5 (1–6) | 1,5 (0–3) | 1 (0–1) | 0 (0–1) | 0 | 0 (0–0,5) |
| 38 | 2 (1–4,5) | 1 (0–6) | 1 (0–2) | 0 (0–1,5) | 0 | 0 (0–0,5) |
| 39 | 2 (0–4) | 0,5 (0–4) | 0 (0–1) | 0,5 (0–1) | 0 | 0 (0–1,5) |
| 40 | 3 (1–7) | 0 (0–2) | 0,5 (0–2) | 0 (0–1) | 0 | 0 (0–1) |
| 41 | 2 (1–6) | 1 (0–6) | 1 (0–2) | 0 (0–1) | 0 | 0 (0–1) |
| 42 | 3 (1–5) | 0 (0–4,5) | 1 (0–2,5) | 0 (0–1,5) | 0 | 0 (0–0,5) |
| 43 | 3 (0,5–5) | 1 (0–4) | 1 (0–2,5) | 0 (0–2) | 0 | 0 (0–1) |
| 44 | 2 (1,5–5,5) | 1 (0–3) | 1 (0–1,5) | 0 (0–1) | 0 | 0 (0–1) |
| 45 | 2,5 (1–5,5) | 1 (0–3,5) | 0 (0–1,5) | 0 (0–1) | 0 | 0 (0–1) |
| 46 | 3 (1–7) | 1 (0–3) | 0,5 (0–3) | 0 (0–1,5) | 0 | 0 (0–0,5) |
| 47 | 4 (2,5–6) | 2,5 (0–5) | 1 (0,5–2,5) | 0 (0–1) | 0 | 0 |
| 48 | 4 (2–5,5) | 1 (0–3,5) | 1 (0–2) | 0,5 (0–2) | 0 | 0 |
| 49 | 3 (1–6) | 0 (0–3,5) | 1 (0–2,5) | 0 (0–1) | 0 | 0 (0–0,5) |
| 50 | 2,5 (1–5) | 0,5 (0–2,5) | 1 (0–2,5) | 0 (0–1,5) | 0 | 0 (0–1) |
| 51 | 3 (2–5,5) | 0,5 (0–4,5) | 0,5 (0–2) | 0 (0–2) | 0 | 0 (0–0,5) |
| 52 | 3 (2–5) | 1,5 (0–4,5) | 1 (0–2,5) | 0,5 (0–1,5) | 0 | 0 (0–0,5) |
| 53 | 3 (1–9,5) | 2 (1–4) | 0,5 (0–2,5) | 0 (0–1) | 0 | 0 (0–0,5) |
| 54 | 2 (1–4,5) | 1,5 (0–5,5) | 1 (0–1,5) | 0 (0–1,5) | 0 | 0 (0–0,5) |
| 55 | 2 (1,5–4,5) | 1,5 (0–4) | 1 (0–2) | 0 (0–1,5) | 0 | 0 (0–1) |
| 56 | 4 (1,5–5,5) | 2 (0–5,5) | 1 (0–3) | 0 (0–1) | 0 | 0 (0–0,5) |
| 57 | 4 (1–6,5) | 2 (0–4,5) | 1 (0–2) | 0 (0–1,5) | 0 | 0 (0–0,5) |
| 58 | 4,5 (1–6,5) | 1 (0–3) | 1 (0–1,5) | 0 (0–1) | 0 | 0 (0–0,5) |
| 59 | 4 (2–6,5) | 1,5 (0–5) | 1 (0–2,5) | 0 (0–1) | 0 | 0 (0–1) |
| 60 | 3 (1–7) | 1 (0–5) | 1 (0–3) | 1 (0–1,5) | 0 | 0 (0–0,5) |

### Kahramanlar (2/2)

| Yıl | Yaşayan kahraman (yıl sonu) | Doğuş seviyesi (ort.) | Ölüm seviyesi (ort.) | Yaşayan kahraman seviyesi (ort.) | En yüksek seviye (şimdiye dek) |
|---|---|---|---|---|---|
| 1 | 0 (0–1,5) | 1 (1–1,5) | – | 1 (1–1,5) | 0 (0–1) |
| 2 | 1 (0–3) | 1 (1–1,15) | – | 1 (1–1,13) | 1 (0–1) |
| 3 | 3 (0–4) | 1 (1–1,5) | – | 1 (1–1,33) | 1 (0–2) |
| 4 | 4 (2,5–6,5) | 1,5 (1–1,87) | 1 | 1,33 (1–1,65) | 2 (1–2) |
| 5 | 8 (4–10) | 1,25 (1–2) | 1 (1–1,8) | 1,33 (1,13–1,5) | 2 |
| 6 | 10 (6–12,5) | 1,1 (1–1,42) | 1,5 (1–2) | 1,3 (1,11–1,5) | 2 |
| 7 | 13 (9,5–17,5) | 1,33 (1,17–2) | 1,5 (1,1–1,87) | 1,36 (1,19–1,59) | 2 (2–3) |
| 8 | 17 (12,5–22) | 1,23 (1–1,62) | 1,5 (1–2) | 1,4 (1,19–1,62) | 2 (2–3) |
| 9 | 18,5 (15–24,5) | 1,25 (1–1,54) | 1 (1–1,55) | 1,51 (1,32–1,78) | 3 (2–3) |
| 10 | 22,5 (18–28,5) | 1,25 (1–1,63) | 1 (1–2,1) | 1,58 (1,36–1,93) | 3 (2–3,5) |
| 11 | 23,5 (18,5–31) | 1,4 (1–2) | 1 (1–2,1) | 1,77 (1,52–2,14) | 3 (3–4) |
| 12 | 25,5 (21–33) | 1,31 (1–1,8) | 1,08 (1–3) | 1,86 (1,65–2,28) | 3 (3–4) |
| 13 | 26 (22–34) | 1,3 (1–2) | 1,67 (1,32–2) | 2 (1,78–2,35) | 4 (3–4) |
| 14 | 28,5 (23,5–36,5) | 1 (1–1,56) | 2 (1,13–2,6) | 2,11 (1,88–2,42) | 4 (3–4,5) |
| 15 | 30,5 (26,5–39) | 1,2 (1–1,48) | 2 (1–2,6) | 2,19 (2,01–2,5) | 4 (3–5) |
| 16 | 34 (28,5–40) | 1,33 (1–1,46) | 1,75 (1–2,1) | 2,35 (2,04–2,52) | 4 (4–5) |
| 17 | 35 (30–42) | 1,33 (1–1,92) | 1,5 (1–2,17) | 2,37 (2,19–2,57) | 4 (4–5) |
| 18 | 38 (30,5–44) | 1 (1–1,45) | 1 (1–1,7) | 2,44 (2,17–2,67) | 5 (4–5) |
| 19 | 39,5 (31,5–44,5) | 1,33 (1–1,5) | 2 (1–3,4) | 2,56 (2,24–2,77) | 5 (4–5) |
| 20 | 40,5 (33,5–49) | 1,33 (1–2) | 2,25 (2–2,85) | 2,6 (2,38–2,91) | 5 (4–5) |
| 21 | 45 (35–52,5) | 1,25 (1–1,58) | 2 (2–2,6) | 2,62 (2,42–2,94) | 5 (4–6) |
| 22 | 45,5 (37–55) | 1,25 (1–1,65) | 2 (1,6–3,33) | 2,71 (2,4–2,92) | 5 (4–6) |
| 23 | 48,5 (39,5–56) | 1,29 (1–1,88) | 2,58 (1,5–3) | 2,74 (2,41–2,97) | 5 (5–6) |
| 24 | 52 (42,5–57,5) | 1,13 (1–1,43) | 3 (1–4,07) | 2,71 (2,46–3) | 5 (5–6) |
| 25 | 53,5 (45–61,5) | 1,33 (1–1,5) | 3 (1–3,85) | 2,78 (2,52–3,03) | 5,5 (5–6) |
| 26 | 56 (45–63,5) | 1,29 (1,06–1,6) | 2,5 (2,04–3,59) | 2,86 (2,51–3,11) | 6 (5–6) |
| 27 | 57 (45–64) | 1,5 (1–1,67) | 2,5 (2,2–3,6) | 2,9 (2,59–3,1) | 6 (5–6) |
| 28 | 59,5 (47–65) | 1,2 (1–1,33) | 2 (1–3,83) | 2,92 (2,68–3,21) | 6 (5–7) |
| 29 | 60 (45,5–67,5) | 1,23 (1–1,45) | 2 (1,67–3,27) | 2,97 (2,71–3,28) | 6 (5–7) |
| 30 | 60 (49–70) | 1,33 (1–2) | 2 (1,63–3,8) | 3,01 (2,73–3,25) | 6 (5–7) |
| 31 | 60,5 (52–73) | 1,4 (1–1,87) | 2,75 (1,96–3,2) | 3,04 (2,74–3,31) | 6 (5,5–7) |
| 32 | 61,5 (52,5–71) | 1,37 (1–1,67) | 2 (1,24–3,4) | 3,13 (2,8–3,37) | 6,5 (5,5–7,5) |
| 33 | 62,5 (53–71) | 1,3 (1–1,55) | 2,75 (2–4) | 3,2 (2,89–3,42) | 7 (6–8) |
| 34 | 64,5 (55–71) | 1,08 (1–1,73) | 1,5 (1–5) | 3,18 (2,95–3,43) | 7 (6–8) |
| 35 | 65,5 (54,5–77) | 1,29 (1–2) | 3 (2,23–4,38) | 3,18 (2,92–3,44) | 7 (6–8) |
| 36 | 65,5 (54–77) | 1,25 (1–2) | 2,43 (1,6–3) | 3,28 (2,89–3,42) | 7 (6–8) |
| 37 | 66,5 (56,5–79) | 1,29 (1–1,83) | 2,25 (1,3–4,4) | 3,32 (3–3,39) | 7,5 (6,5–8) |
| 38 | 66 (53,5–78) | 1,2 (1–1,8) | 2,92 (1,93–4,07) | 3,36 (2,98–3,48) | 7,5 (6,5–8,5) |
| 39 | 64,5 (54–78) | 1,33 (1–1,96) | 3 (2,5–4,6) | 3,38 (3,02–3,53) | 8 (7–8,5) |
| 40 | 64,5 (58–81) | 1,5 (1,1–2) | 3,08 (1,8–3,9) | 3,42 (3,02–3,56) | 8 (7–9) |
| 41 | 66 (57,5–79) | 1,46 (1–1,73) | 3 (1,86–4) | 3,38 (3,02–3,62) | 8 (7–9) |
| 42 | 66 (56–79,5) | 1,4 (1–2) | 2 (1,5–3,92) | 3,43 (3,01–3,69) | 8 (7–9) |
| 43 | 67,5 (57–77) | 1,33 (1–2) | 3 (1,4–3,27) | 3,49 (3,07–3,77) | 8 (7–9) |
| 44 | 68 (57–74,5) | 1,45 (1–1,83) | 2,67 (1–2,87) | 3,47 (3,11–3,85) | 8 (7–9) |
| 45 | 69 (56,5–77,5) | 1 (1–1,5) | 3 (2–3,67) | 3,44 (3,11–3,85) | 8 (7–9) |
| 46 | 69 (55,5–78,5) | 1,5 (1–2) | 3 (1,27–3,6) | 3,43 (3,15–3,82) | 8,5 (7,5–9) |
| 47 | 68,5 (55,5–78,5) | 1,37 (1,08–1,67) | 3 (1,53–3,74) | 3,42 (3,11–3,83) | 9 (7,5–9) |
| 48 | 69 (57,5–78,5) | 1,37 (1,1–2) | 3,67 (2,7–5,2) | 3,31 (3,08–3,88) | 9 (7,5–9) |
| 49 | 72 (59–81) | 1,5 (1–1,74) | 3,73 (2,08–5,75) | 3,3 (3,01–3,88) | 9 (7,5–9) |
| 50 | 71 (60,5–81) | 1,5 (1–2,1) | 3,25 (1,23–5,55) | 3,38 (3,09–3,88) | 9 (7,5–9) |
| 51 | 72,5 (63–81,5) | 1,15 (1–1,6) | 3 (1,47–4,44) | 3,34 (3,15–3,9) | 9 (7,5–9,5) |
| 52 | 72 (61,5–83) | 1,67 (1–2) | 3 (2–6) | 3,39 (3,15–3,88) | 9 (7,5–9,5) |
| 53 | 73 (64–81) | 1,42 (1–2) | 3,25 (1–5) | 3,4 (3,12–3,89) | 9 (7,5–10) |
| 54 | 73 (59,5–82,5) | 1,5 (1–1,87) | 3 (2,35–5) | 3,41 (3,15–3,95) | 9 (8–10) |
| 55 | 76 (55–84) | 1,5 (1–2) | 4 (2,33–6) | 3,42 (3,09–4,1) | 9 (8–10) |
| 56 | 73 (59,5–84) | 1,59 (1–2) | 4 (3,06–5,8) | 3,42 (3,12–3,92) | 9 (8–10) |
| 57 | 73,5 (60,5–84) | 1,5 (1–1,79) | 3 (1,83–6,5) | 3,48 (3,15–3,87) | 9 (8–10) |
| 58 | 76,5 (62,5–87) | 1,33 (1–1,58) | 4 (2–5,9) | 3,53 (3,04–3,76) | 9 (8,5–10) |
| 59 | 75,5 (60,5–89) | 1,33 (1,08–1,71) | 4 (2–4,95) | 3,57 (3,02–3,82) | 9 (9–10) |
| 60 | 74,5 (61,5–89,5) | 1,41 (1–2) | 4,78 (3,4–5,1) | 3,53 (3,1–3,89) | 9 (9–10) |

### Han ve ticaret

| Yıl | Ayakta han | Asılan ilan | Biten ilan | Ticaret seferi (kervan) | İkmal seferi |
|---|---|---|---|---|---|
| 1 | 3 (1,5–4) | 0 (0–1) | 0 | 0 | 0 |
| 2 | 4 | 0 | 0 | 0 | 0 |
| 3 | 4 | 0 (0–1,5) | 0 | 2 (0–6) | 0 |
| 4 | 4 (3–4) | 1 (0–3,5) | 0 | 7,5 (2,5–19,5) | 0 |
| 5 | 4 (3–4) | 2 (1–3,5) | 0 | 14 (9–26) | 0 |
| 6 | 3 (3–4) | 1,5 (0–4) | 0 | 15,5 (9,5–37,5) | 0 |
| 7 | 3 (3–4) | 3 (1–4,5) | 0 (0–1,5) | 25 (14,5–44) | 0 |
| 8 | 3 (3–4) | 2 (1–3) | 0 (0–1) | 30 (18–50,5) | 0 |
| 9 | 3 (3–4) | 2 (0,5–3,5) | 0 (0–2) | 37,5 (18,5–56,5) | 0 (0–4) |
| 10 | 3 (2,5–4) | 3 (1–4) | 0 (0–3,5) | 43 (19,5–58,5) | 3 (0–6) |
| 11 | 3 (2,5–4) | 3 (0–5) | 0,5 (0–2,5) | 42,5 (21–58) | 4 (1,5–9) |
| 12 | 4 (2,5–4) | 2 (1–4,5) | 1 (0–2) | 46 (24–61) | 6,5 (3–15) |
| 13 | 4 (3–5) | 2 (0,5–5,5) | 0 (0–3) | 48,5 (25–59,5) | 11 (4–19,5) |
| 14 | 5 (3–5) | 2 (1–4) | 1 (0–3) | 50 (26,5–70) | 12 (5,5–21,5) |
| 15 | 5 (4–5) | 2 (0–4) | 0 (0–2,5) | 54,5 (31–74,5) | 12,5 (5,5–22) |
| 16 | 5 (4–5) | 1,5 (0–3,5) | 1 (0–2,5) | 54,5 (34–85) | 13 (7–22,5) |
| 17 | 5 (3,5–5) | 1,5 (0,5–4) | 0 (0–1,5) | 57 (33,5–90,5) | 15,5 (8–22,5) |
| 18 | 5 (3,5–5) | 1 (1–4) | 0,5 (0–2) | 58,5 (35–94) | 15,5 (8,5–21,5) |
| 19 | 5 (4–5) | 2 (0–5) | 0 (0–1) | 58,5 (35,5–99,5) | 18 (10–25) |
| 20 | 5 (4–5) | 2 (1–3) | 0 (0–2,5) | 63,5 (35–102) | 17,5 (9,5–26) |
| 21 | 5 (4–5,5) | 2,5 (0–5,5) | 0 (0–1,5) | 65,5 (36,5–99,5) | 21 (10,5–25,5) |
| 22 | 5 (4–6) | 3,5 (2–5,5) | 0 (0–1) | 68 (35,5–98,5) | 21 (12–27,5) |
| 23 | 5 (4–6) | 2 (1–5) | 0,5 (0–2) | 68,5 (37,5–98,5) | 22 (13,5–26,5) |
| 24 | 5 (4–6) | 1,5 (0,5–3,5) | 1 (0–2) | 71,5 (36,5–100) | 24 (14–28) |
| 25 | 5 (4–6) | 3,5 (1–5,5) | 1 (0–2) | 72 (36–104) | 24 (14,5–27,5) |
| 26 | 5 (4,5–6) | 3,5 (2–6,5) | 1 (0–2) | 74,5 (35,5–110) | 24,5 (15,5–29,5) |
| 27 | 5 (4–6) | 3 (1–4,5) | 1 (0–2) | 71,5 (38–122) | 25,5 (16–29) |
| 28 | 5 (4–6) | 2,5 (1,5–3) | 1 (0–1) | 78 (39,5–130) | 24,5 (17,5–28,5) |
| 29 | 5 (5–6) | 2,5 (1–6,5) | 0 (0–2) | 73 (44–127) | 24,5 (15,5–30) |
| 30 | 5 (4,5–6) | 3 (1,5–4) | 1 (0–2) | 77,5 (43–126) | 26 (18,5–32) |
| 31 | 5 (4,5–6) | 3 (1,5–6,5) | 1 (0–2) | 73,5 (44–132) | 26 (18,5–32,5) |
| 32 | 5 (5–6) | 3 (2–5) | 1 (0–3,5) | 79,5 (44,5–135) | 25,5 (19–33,5) |
| 33 | 5 (4,5–6) | 4 (2–6) | 2 (0–2) | 77 (46,5–136) | 26,5 (17,5–35,5) |
| 34 | 5 (5–6) | 3 (1–7) | 1 (0–2,5) | 77,5 (46–138) | 27 (20–34,5) |
| 35 | 5 (4–6) | 3 (1–6) | 1 (0–2,5) | 80,5 (45,5–137) | 26,5 (20–36,5) |
| 36 | 5 (4–6) | 2,5 (0,5–5) | 1 (0–2,5) | 78 (44,5–135) | 27,5 (21–33,5) |
| 37 | 5 (4,5–6) | 4 (1–6,5) | 1 (0–3) | 78 (45–138) | 29 (19,5–35,5) |
| 38 | 5 (4–6) | 2,5 (0,5–5) | 0 (0–1,5) | 82 (46–136) | 28 (19,5–36,5) |
| 39 | 5 (5–6) | 3,5 (0,5–6) | 1 (0–2,5) | 84 (45,5–138) | 29 (21–41) |
| 40 | 5 (4,5–6) | 2,5 (1–6,5) | 0 (0–3) | 90 (43,5–136) | 29,5 (21–40) |
| 41 | 5 (4,5–6) | 3 (1,5–6) | 1 (0–1) | 90,5 (46–142) | 30,5 (19,5–44,5) |
| 42 | 5,5 (4,5–6) | 3 (2–5,5) | 1 (0–2,5) | 93 (45–144) | 30,5 (20–44) |
| 43 | 6 (4,5–6) | 3 (1,5–4,5) | 1 (0–3) | 87,5 (47–144) | 28 (20,5–46) |
| 44 | 6 (5–6) | 4 (1–6,5) | 1 (0–2,5) | 85,5 (44,5–144) | 28,5 (19–48) |
| 45 | 6 (5–6) | 4 (1–6,5) | 1 (0–2) | 82 (48–142) | 28 (18,5–51,5) |
| 46 | 6 (5–6) | 3 (2–6,5) | 1 (0–2) | 91,5 (45–148) | 29 (17,5–49,5) |
| 47 | 6 (5–6) | 3,5 (1,5–6) | 1 (0–2,5) | 89 (55–154) | 30,5 (20,5–50) |
| 48 | 6 (5–6) | 2,5 (0,5–5) | 1 (0–3,5) | 93 (49–152) | 29,5 (17,5–47,5) |
| 49 | 6 (5–6) | 2,5 (1–6) | 1 (0–1) | 97,5 (50,5–166) | 33 (21–50) |
| 50 | 6 (5–6) | 3 (0,5–9) | 1 (0–4,5) | 92,5 (53,5–160) | 30 (19,5–49) |
| 51 | 6 (5–6,5) | 3 (1–7) | 1 (0–2,5) | 99,5 (51–164) | 32 (20–46) |
| 52 | 6 (5–6) | 3 (1,5–5) | 1 (0–2) | 89,5 (48–164) | 32 (22–48,5) |
| 53 | 6 (5–6) | 4 (1–8) | 1 (0–3) | 92,5 (49–167) | 32 (20,5–49) |
| 54 | 5,5 (5–6) | 3 (1,5–6) | 1 (0–5) | 95,5 (51–170) | 30 (22–51) |
| 55 | 5,5 (4,5–6) | 3 (2–5) | 1 (0–3) | 90 (51,5–163) | 32,5 (21–53) |
| 56 | 5 (5–6) | 3 (1,5–4) | 1 (0–2,5) | 93,5 (47,5–169) | 30 (20,5–54) |
| 57 | 5 (5–6) | 3,5 (1,5–9) | 1 (0–3) | 92 (52–168) | 32 (20–50,5) |
| 58 | 5,5 (5–6) | 3,5 (1–5,5) | 1 (0–3,5) | 88 (48,5–166) | 32 (21,5–50,5) |
| 59 | 5 (5–6) | 3,5 (1,5–7) | 1 (0,5–2,5) | 91,5 (52,5–165) | 32 (20,5–49,5) |
| 60 | 5,5 (4,5–6) | 3 (0,5–6) | 1 (0–3,5) | 86 (48,5–162) | 31,5 (18,5–49) |

### Altın ve ambar (1/3)

| Yıl | Altın p90 (medeniyetler) | Bakım gideri (altın; asker, kahraman, L2–L3) | Kamu işlerine (imar) harcanan altın | Ambarla beslenen amele tayını (gıda) | Kamu işlerindeki (amele) iş gücü payı | İmar ortalaması (köy+, 0–100) |
|---|---|---|---|---|---|---|
| 1 | 24,6 (15,1–35,6) | 0 | 0 | 0 | %0 | – |
| 2 | 65,8 (44–78,5) | 0,62 (0–1,1) | 0 | 0 | %0 | 0 |
| 3 | 87,4 (60,6–134) | 6,63 (4,62–16,3) | 0 | 0 | %0 | 0 |
| 4 | 91 (71,7–152) | 21 (7,24–35,2) | 0 | 0 | %0 | 0 |
| 5 | 133 (78,2–173) | 45,3 (25–81,4) | 0 | 0 (0–0,8) | %0 (%0–%0,1) | 0 |
| 6 | 156 (101–240) | 85,1 (47,4–126) | 0 (0–1,14) | 0 (0–11,8) | %0 (%0–%0,4) | 0 (0–0,05) |
| 7 | 183 (127–246) | 129 (92,7–212) | 0 (0–14,3) | 2,55 (0–30) | %0,2 (%0–%0,7) | 0 (0–0,2) |
| 8 | 206 (115–302) | 208 (161–307) | 3,23 (0–21,2) | 7,85 (0–42,1) | %0,4 (%0,1–%1,3) | 0,11 (0–0,39) |
| 9 | 267 (171–330) | 296 (227–398) | 31 (0–75,8) | 89,6 (36–215) | %1,5 (%0,9–%3,1) | 0,47 (0–1,48) |
| 10 | 336 (258–437) | 363 (309–497) | 94,7 (3,23–161) | 210 (88,6–327) | %3 (%1,5–%4,8) | 1,35 (0,15–2,92) |
| 11 | 433 (298–627) | 477 (367–568) | 219 (80,1–589) | 380 (211–586) | %4,9 (%3,7–%8,1) | 3,19 (0,81–5,93) |
| 12 | 476 (320–820) | 559 (423–703) | 332 (180–747) | 554 (405–823) | %8,1 (%6,4–%11) | 5,82 (2,51–9,8) |
| 13 | 548 (374–751) | 669 (467–786) | 460 (243–995) | 821 (555–1222) | %10 (%8,6–%13) | 7,62 (4,03–12,1) |
| 14 | 606 (465–742) | 721 (540–894) | 608 (394–1071) | 1124 (530–1533) | %12 (%10–%14) | 10,6 (7,53–15,5) |
| 15 | 605 (436–692) | 842 (637–981) | 774 (424–1046) | 1398 (755–2183) | %14 (%11–%16) | 12,7 (9,89–16,6) |
| 16 | 646 (569–788) | 905 (711–1081) | 757 (508–1164) | 1418 (1028–1891) | %14 (%13–%17) | 14,8 (10,3–18,7) |
| 17 | 722 (501–966) | 1006 (776–1252) | 1067 (636–1366) | 1704 (1099–2279) | %15 (%13–%19) | 17 (12–19,9) |
| 18 | 734 (608–926) | 1160 (886–1325) | 1127 (638–1424) | 1796 (1064–2451) | %17 (%14–%19) | 19,2 (12,9–21,6) |
| 19 | 740 (649–897) | 1211 (977–1398) | 1185 (821–1666) | 1767 (1241–2470) | %19 (%15–%20) | 19,6 (15–22,4) |
| 20 | 912 (737–1104) | 1306 (1012–1510) | 1272 (863–1867) | 2042 (1217–2729) | %19 (%15–%21) | 22,4 (16–24,9) |
| 21 | 823 (617–1173) | 1450 (1077–1584) | 1414 (972–1891) | 2393 (1210–3664) | %20 (%16–%22) | 23,2 (17,4–26,3) |
| 22 | 922 (756–1273) | 1505 (1167–1651) | 1440 (1154–1900) | 2346 (1471–3512) | %21 (%17–%22) | 24,8 (18,7–26,9) |
| 23 | 981 (806–1268) | 1570 (1206–1687) | 1562 (1186–2085) | 2546 (1664–4211) | %21 (%17–%23) | 25,3 (20–28,7) |
| 24 | 1072 (750–1472) | 1580 (1265–1735) | 1651 (1182–2275) | 2700 (1892–4797) | %22 (%18–%23) | 26,5 (21,9–29,7) |
| 25 | 1110 (852–1519) | 1598 (1303–1810) | 1696 (1149–2527) | 3270 (1802–4789) | %22 (%18–%24) | 27,1 (22,5–32,6) |
| 26 | 1055 (762–1359) | 1692 (1420–1901) | 1602 (1210–2400) | 3221 (1839–4459) | %22 (%18–%24) | 26 (22,4–30,7) |
| 27 | 1051 (835–1151) | 1663 (1454–2063) | 1615 (1334–2386) | 2691 (1943–3924) | %22 (%18–%24) | 26,6 (22,6–31,1) |
| 28 | 1039 (808–1339) | 1695 (1512–2101) | 1771 (1412–2515) | 2628 (1847–3869) | %22 (%18–%24) | 26,4 (22,6–30,3) |
| 29 | 1075 (868–1399) | 1836 (1524–2111) | 1780 (1358–2635) | 2821 (1630–4398) | %22 (%18–%24) | 26,1 (22,3–29,6) |
| 30 | 1118 (828–1233) | 1836 (1581–2209) | 1926 (1292–2551) | 2358 (1421–3749) | %21 (%18–%24) | 25,1 (21,9–29,2) |
| 31 | 1138 (901–1294) | 1862 (1580–2148) | 1925 (1254–2832) | 1919 (1333–3347) | %20 (%18–%24) | 23,9 (21,8–31,4) |
| 32 | 1120 (932–1502) | 1937 (1675–2217) | 1830 (1466–2926) | 2619 (1338–3257) | %20 (%16–%24) | 24 (21,2–30,7) |
| 33 | 1172 (992–1489) | 1954 (1711–2245) | 2050 (1617–2959) | 2491 (960–3233) | %20 (%18–%24) | 24,2 (21,3–29,9) |
| 34 | 1136 (1019–1558) | 2031 (1726–2321) | 2085 (1759–3027) | 2010 (1190–2975) | %20 (%17–%23) | 25,6 (20,9–29,2) |
| 35 | 1239 (1092–1517) | 2014 (1680–2312) | 2203 (1780–3142) | 1949 (1417–2733) | %20 (%17–%23) | 26,3 (20,8–30,7) |
| 36 | 1215 (926–1593) | 1971 (1724–2385) | 2128 (1582–3123) | 1852 (1297–3106) | %21 (%18–%23) | 26,6 (20,9–31,7) |
| 37 | 1233 (907–1594) | 2023 (1780–2322) | 2187 (1557–3405) | 1800 (1063–3055) | %20 (%18–%24) | 26,2 (21,1–31,5) |
| 38 | 1293 (982–1624) | 2087 (1836–2393) | 2191 (1754–3485) | 1860 (813–3030) | %20 (%18–%24) | 27 (21,5–30,2) |
| 39 | 1299 (1104–1627) | 2061 (1752–2463) | 2405 (1815–3431) | 1933 (842–2724) | %21 (%18–%23) | 26,3 (22,4–30,3) |
| 40 | 1357 (947–1723) | 2064 (1767–2386) | 2550 (1795–3434) | 1703 (1008–3015) | %20 (%18–%23) | 26,4 (22,1–30,8) |
| 41 | 1321 (1038–1696) | 2082 (1734–2460) | 2451 (1591–3471) | 1657 (868–2902) | %20 (%18–%23) | 26,4 (21,6–30,6) |
| 42 | 1234 (1099–1606) | 2084 (1677–2403) | 2424 (1890–3452) | 1464 (599–2811) | %21 (%18–%23) | 26,5 (21,9–30,3) |
| 43 | 1359 (1140–1613) | 2092 (1770–2487) | 2378 (1933–3634) | 1555 (741–3085) | %20 (%17–%22) | 26,7 (22,2–30,8) |
| 44 | 1285 (916–1825) | 2154 (1748–2473) | 2709 (1910–3719) | 1160 (235–2672) | %21 (%17–%23) | 26,4 (22,3–31,2) |
| 45 | 1461 (1070–1728) | 2106 (1729–2459) | 2632 (1907–3689) | 1040 (531–2446) | %20 (%17–%24) | 26,7 (22,5–31,2) |
| 46 | 1327 (1085–1599) | 2147 (1801–2503) | 2505 (2124–3681) | 1434 (651–2494) | %20 (%17–%23) | 25,7 (21,6–31,4) |
| 47 | 1303 (948–1673) | 2081 (1785–2550) | 2508 (1980–3718) | 1395 (620–2853) | %20 (%17–%23) | 25,2 (21,5–31,9) |
| 48 | 1326 (976–1713) | 2077 (1735–2576) | 2604 (1888–3790) | 974 (459–2858) | %19 (%16–%23) | 25,8 (20,7–33,1) |
| 49 | 1324 (997–1809) | 2082 (1790–2568) | 2711 (1813–4006) | 1163 (608–2410) | %20 (%17–%24) | 25 (20,3–33,1) |
| 50 | 1437 (996–1728) | 2032 (1791–2553) | 2515 (1971–4125) | 1084 (408–2337) | %20 (%16–%24) | 24,5 (20,3–32,8) |
| 51 | 1455 (1066–1836) | 2070 (1779–2514) | 2727 (2087–4115) | 1196 (474–2136) | %20 (%16–%24) | 24,5 (19,7–32,8) |
| 52 | 1325 (1079–1839) | 2110 (1872–2537) | 2871 (1942–4161) | 1250 (302–2012) | %19 (%16–%24) | 25,3 (20,5–32,8) |
| 53 | 1432 (1199–1707) | 2134 (1765–2481) | 3034 (1947–4201) | 1099 (349–2409) | %20 (%17–%24) | 26,7 (20,9–33,4) |
| 54 | 1321 (1177–1731) | 2148 (1774–2418) | 2922 (2232–4533) | 512 (229–2958) | %21 (%17–%24) | 27,1 (21,8–33,5) |
| 55 | 1374 (1115–1790) | 2168 (1763–2515) | 2894 (2045–4367) | 624 (224–2837) | %19 (%17–%23) | 26,6 (21,9–34,5) |
| 56 | 1406 (1091–1800) | 2245 (1830–2523) | 2873 (2068–4460) | 854 (215–2692) | %19 (%16–%24) | 26,7 (22,7–34,6) |
| 57 | 1364 (1118–1812) | 2164 (1884–2598) | 2931 (2015–4390) | 888 (221–1920) | %19 (%15–%24) | 26,6 (22,8–34) |
| 58 | 1562 (1102–1702) | 2215 (1893–2641) | 3027 (2131–4495) | 773 (171–2103) | %19 (%16–%23) | 26,7 (23–34) |
| 59 | 1495 (1161–1854) | 2189 (1958–2579) | 3242 (2125–4427) | 519 (216–1780) | %19 (%17–%22) | 27,7 (23–33,5) |
| 60 | 1315 (1063–1921) | 2169 (1895–2725) | 3035 (1994–4579) | 605 (77,5–2662) | %19 (%16–%23) | 28,3 (23,1–34) |

### Altın ve ambar (2/3)

| Yıl | Canavar baskınında yitirilen altın | Ejderhaya giden altın (haraç + akın) | Hazinesi boş medeniyet payı | Kent tüketiminde yokluk payı (köy+; ekmek, bira ya da alet) | Ekmek ya da bira yokluğu payı (köy+) | Kıtlık (büyük olay) |
|---|---|---|---|---|---|---|
| 1 | 0 | 0 | %0 | – | – | 0 |
| 2 | 0 | 0 | %0 | %1 (%0–%1,9) | %1 (%0–%1,9) | 0 |
| 3 | 0 | 0 | %0 | %0,7 (%0,4–%2,7) | %0,7 (%0,4–%2,7) | 0 |
| 4 | 11 (0–45) | 0 | %0 | %0,7 (%0,2–%3,7) | %0,7 (%0,2–%3,7) | 0 |
| 5 | 8 (0–30,5) | 0 | %0 | %0,1 (%0–%3) | %0,1 (%0–%3) | 0 |
| 6 | 6 (0–31,5) | 0 | %0 | %0,1 (%0–%3,8) | %0 (%0–%3,8) | 0 |
| 7 | 4 (0–19) | 0 | %0 | %0 (%0–%2,3) | %0 (%0–%2,3) | 0 |
| 8 | 13,5 (0–21) | 0 | %0 | %0,1 (%0–%0,5) | %0 (%0–%0,5) | 0 |
| 9 | 11 (0–23) | 0 | %0 | %0 (%0–%1,9) | %0 (%0–%1,6) | 0 |
| 10 | 5,5 (0–40,5) | 0 | %0 | %0,2 (%0–%3,1) | %0,2 (%0–%3,1) | 0 |
| 11 | 1,5 (0–16) | 0 | %0 | %0,4 (%0–%3,1) | %0,2 (%0–%2,5) | 0 |
| 12 | 2 (0–42) | 0 | %0 | %1,6 (%0–%5,4) | %1,5 (%0–%3,2) | 0 |
| 13 | 0 (0–40,5) | 0 | %0 | %1,2 (%0–%7,3) | %1,2 (%0–%4,4) | 0 |
| 14 | 0 (0–35,5) | 0 | %0 | %2,1 (%0–%6,4) | %2,1 (%0–%5,5) | 0 |
| 15 | 0,5 (0–6,5) | 0 | %0 | %1,7 (%0–%6,9) | %0,2 (%0–%6,9) | 0 |
| 16 | 2 (0–26,5) | 0 | %0 | %2,6 (%0–%9) | %1,1 (%0–%9) | 0 |
| 17 | 0 (0–26,5) | 0 | %0 | %1,3 (%0–%7,3) | %0,5 (%0–%6,8) | 0 |
| 18 | 0 (0–39) | 0 | %0 | %1,7 (%0–%6,8) | %1,2 (%0–%6,4) | 0 |
| 19 | 0 (0–25) | 0 (0–71,5) | %0 | %1,1 (%0–%6,3) | %0,8 (%0–%6,1) | 0 |
| 20 | 0 (0–16,5) | 0 (0–230) | %0 | %1,8 (%0–%6,9) | %1,2 (%0–%6,8) | 0 |
| 21 | 0 (0–18,5) | 145 (0–354) | %0 | %2,9 (%0–%10) | %2,2 (%0–%8,3) | 0 |
| 22 | 0 (0–19) | 132 (20–264) | %0 | %2,7 (%0–%12) | %1,3 (%0–%12) | 0 |
| 23 | 0 (0–24,5) | 220 (84–374) | %0 | %3,6 (%0,4–%11) | %1,7 (%0–%11) | 0 |
| 24 | 0 (0–19) | 192 (110–394) | %0 | %6,5 (%1–%15) | %3,6 (%0–%10) | 0 |
| 25 | 0 (0–14) | 202 (47,5–402) | %0 | %11 (%4,4–%22) | %3,3 (%0–%12) | 0 |
| 26 | 0 (0–12) | 297 (88–479) | %0 | %15 (%3,7–%26) | %3,7 (%0–%11) | 0 |
| 27 | 0 (0–20) | 212 (168–580) | %0 | %14 (%6,3–%31) | %3,4 (%0–%9,9) | 0 |
| 28 | 0 (0–13,5) | 363 (71,5–801) | %0 | %19 (%5–%36) | %2,8 (%0–%14) | 0 |
| 29 | 0 (0–11,5) | 444 (156–770) | %0 | %20 (%4,7–%41) | %4,3 (%0,1–%10) | 0 |
| 30 | 0 (0–16) | 373 (196–727) | %0 | %28 (%15–%44) | %5,1 (%0,3–%12) | 0 |
| 31 | 0 (0–22) | 516 (216–654) | %0 | %28 (%22–%46) | %5,8 (%0,3–%15) | 0 |
| 32 | 0 (0–8) | 432 (208–678) | %0 | %30 (%15–%44) | %7,8 (%0–%15) | 0 |
| 33 | 0 (0–33,5) | 452 (200–862) | %0 | %32 (%16–%47) | %8,8 (%0,3–%16) | 0 |
| 34 | 0 (0–6,5) | 364 (162–728) | %0 | %33 (%16–%50) | %7,8 (%0,6–%14) | 0 |
| 35 | 0 (0–17,5) | 450 (193–878) | %0 | %34 (%22–%51) | %5,8 (%0,4–%12) | 0 |
| 36 | 0 (0–9) | 359 (25,5–828) | %0 | %37 (%26–%48) | %6,1 (%0–%13) | 0 |
| 37 | 0 (0–10) | 456 (138–868) | %0 | %40 (%23–%56) | %5,7 (%0,5–%14) | 0 |
| 38 | 0 (0–38,5) | 425 (278–844) | %0 | %40 (%28–%53) | %6,5 (%0,3–%13) | 0 |
| 39 | 0 | 432 (98–780) | %0 | %39 (%29–%56) | %6,4 (%0–%13) | 0 |
| 40 | 0 (0–21) | 384 (162–783) | %0 | %41 (%32–%57) | %6,7 (%0,4–%11) | 0 |
| 41 | 0 (0–25) | 506 (314–984) | %0 | %46 (%29–%58) | %6,6 (%2–%10) | 0 |
| 42 | 0 (0–15,5) | 450 (187–836) | %0 | %47 (%39–%55) | %8 (%2,4–%10) | 0 |
| 43 | 0 (0–30) | 442 (155–980) | %0 | %45 (%39–%56) | %6,1 (%3,1–%11) | 0 |
| 44 | 0 (0–12) | 538 (137–853) | %0 | %46 (%37–%56) | %6,5 (%1,3–%13) | 0 |
| 45 | 0 (0–27,5) | 642 (114–985) | %0 | %46 (%36–%54) | %7,1 (%2,3–%17) | 0 |
| 46 | 0 (0–11,5) | 566 (82,5–1140) | %0 | %48 (%35–%58) | %8,3 (%1,9–%16) | 0 |
| 47 | 0 (0–8) | 604 (249–936) | %0 | %47 (%36–%60) | %8,6 (%2,1–%16) | 0 |
| 48 | 0 (0–5,5) | 521 (168–934) | %0 | %50 (%34–%65) | %8,6 (%1,7–%14) | 0 |
| 49 | 3 (0–9,5) | 466 (92,5–974) | %0 | %48 (%38–%62) | %8,3 (%1,5–%15) | 0 |
| 50 | 0 (0–26) | 596 (288–995) | %0 | %49 (%36–%63) | %6,6 (%0,6–%17) | 0 |
| 51 | 0 (0–21,5) | 556 (72–778) | %0 | %51 (%35–%63) | %7,2 (%0,8–%13) | 0 |
| 52 | 0 (0–54) | 533 (208–874) | %0 | %50 (%34–%61) | %7,8 (%0,8–%13) | 0 |
| 53 | 0 (0–9) | 591 (328–888) | %0 | %48 (%34–%61) | %8,3 (%1,6–%16) | 0 |
| 54 | 2,5 (0–34) | 673 (42–968) | %0 | %48 (%35–%60) | %9,1 (%1,6–%14) | 0 |
| 55 | 1,5 (0–16,5) | 560 (146–1132) | %0 | %48 (%37–%62) | %9,9 (%2,8–%16) | 0 (0–0,5) |
| 56 | 0 (0–28) | 562 (80–1042) | %0 | %49 (%31–%61) | %9,9 (%2,4–%15) | 0 |
| 57 | 0 (0–53) | 493 (140–842) | %0 | %47 (%34–%58) | %9,6 (%2–%13) | 0 |
| 58 | 0 (0–14,5) | 508 (77,5–873) | %0 | %47 (%34–%57) | %9 (%2,9–%14) | 0 |
| 59 | 5 (0–17) | 576 (95,5–914) | %0 | %49 (%36–%56) | %8,9 (%1,1–%14) | 0 |
| 60 | 9,5 (0–45) | 570 (223–1036) | %0 | %49 (%38–%57) | %10 (%1,7–%17) | 0 |

### Altın ve ambar (3/3)

| Yıl | Açlıktan ölen | Kıtlık yardımı (sevkiyat) | Kıtlıkta yüz çeviren | Kıtlık akını | Ambarın yettiği gün (medeniyet medyanı) |
|---|---|---|---|---|---|
| 1 | 0 | 0 | 0 | 0 | 33,7 (25,1–51,9) |
| 2 | 0 | 0 | 0 | 0 | 50,5 (31,3–71,5) |
| 3 | 0 | 0 | 0 | 0 | 45,7 (38,1–68) |
| 4 | 0 | 0 | 0 | 0 | 47,9 (43,2–69) |
| 5 | 0 | 0 | 0 | 0 | 57,7 (35,7–73,4) |
| 6 | 0 | 0 | 0 | 0 | 79,6 (55,8–89,7) |
| 7 | 0 | 0 | 0 | 0 | 89,3 (75,1–114) |
| 8 | 0 | 0 | 0 | 0 | 107 (79,4–130) |
| 9 | 0 | 0 | 0 | 0 | 128 (93,6–150) |
| 10 | 0 | 0 | 0 | 0 | 145 (125–175) |
| 11 | 0 | 0 | 0 | 0 | 166 (151–196) |
| 12 | 0 | 0 | 0 | 0 | 179 (161–234) |
| 13 | 0 | 0 | 0 | 0 | 196 (176–250) |
| 14 | 0 | 0 | 0 | 0 | 199 (183–248) |
| 15 | 0 | 0 | 0 | 0 | 216 (187–267) |
| 16 | 0 | 0 | 0 | 0 | 237 (203–277) |
| 17 | 0 | 0 | 0 | 0 | 243 (209–273) |
| 18 | 0 | 0 | 0 | 0 | 251 (220–298) |
| 19 | 0 | 0 | 0 | 0 | 254 (216–306) |
| 20 | 0 | 0 | 0 | 0 | 254 (208–323) |
| 21 | 0 | 0 | 0 | 0 | 267 (229–305) |
| 22 | 0 | 0 | 0 | 0 | 268 (219–304) |
| 23 | 0 | 0 | 0 | 0 | 266 (215–326) |
| 24 | 0 | 0 | 0 | 0 | 259 (213–312) |
| 25 | 0 | 0 | 0 | 0 | 263 (228–311) |
| 26 | 0 | 0 | 0 | 0 | 259 (220–287) |
| 27 | 0 | 0 | 0 | 0 | 252 (233–293) |
| 28 | 0 | 0 | 0 | 0 | 260 (207–294) |
| 29 | 0 | 0 | 0 | 0 | 250 (176–281) |
| 30 | 0 | 0 | 0 | 0 | 245 (183–286) |
| 31 | 0 | 0 | 0 | 0 | 247 (190–284) |
| 32 | 0 | 0 | 0 | 0 | 246 (165–292) |
| 33 | 0 | 0 | 0 | 0 | 245 (174–303) |
| 34 | 0 | 0 | 0 | 0 | 230 (176–301) |
| 35 | 0 | 0 | 0 | 0 | 229 (176–291) |
| 36 | 0 | 0 | 0 | 0 | 229 (169–270) |
| 37 | 0 | 0 | 0 | 0 | 235 (154–273) |
| 38 | 0 | 0 | 0 | 0 | 218 (161–253) |
| 39 | 0 | 0 | 0 | 0 | 211 (161–257) |
| 40 | 0 | 0 | 0 | 0 | 205 (156–246) |
| 41 | 0 | 0 | 0 | 0 | 205 (149–240) |
| 42 | 0 | 0 | 0 | 0 | 208 (161–239) |
| 43 | 0 | 0 | 0 | 0 | 200 (144–232) |
| 44 | 0 | 0 | 0 | 0 | 211 (143–237) |
| 45 | 0 | 0 | 0 | 0 | 205 (151–248) |
| 46 | 0 | 0 | 0 | 0 | 206 (157–253) |
| 47 | 0 | 0 | 0 | 0 | 201 (132–232) |
| 48 | 0 | 0 | 0 | 0 (0–0,5) | 190 (133–234) |
| 49 | 0 | 0 | 0 | 0 | 187 (142–217) |
| 50 | 0 | 0 | 0 | 0 | 189 (149–233) |
| 51 | 0 | 0 | 0 | 0 | 189 (152–238) |
| 52 | 0 | 0 | 0 | 0 | 187 (153–222) |
| 53 | 0 | 0 | 0 | 0 | 184 (143–234) |
| 54 | 0 | 0 | 0 | 0 | 180 (140–230) |
| 55 | 0 | 0 | 0 | 0 | 171 (128–211) |
| 56 | 0 | 0 | 0 | 0 | 179 (140–225) |
| 57 | 0 | 0 | 0 | 0 | 169 (140–232) |
| 58 | 0 | 0 | 0 | 0 | 180 (132–205) |
| 59 | 0 | 0 | 0 | 0 | 161 (125–183) |
| 60 | 0 | 0 | 0 | 0 | 164 (122–190) |

### Yerleşim kademesi

| Yıl | Ortalama yerleşim kademesi | Köy+ yerleşim | Kasaba+ yerleşim | Şehir | Ortalama başkent kademesi | Kademe değişimi (yerleşim, yıl içinde) |
|---|---|---|---|---|---|---|
| 1 | 0 | 0 | 0 | 0 | 0 | 0 |
| 2 | 0,29 (0,11–0,47) | 2 (1–4) | 0 | 0 | 0,29 (0,11–0,47) | 2 (1–4) |
| 3 | 0,61 (0,5–0,78) | 7 (5–8) | 0 | 0 | 0,78 (0,71–1) | 4 (2,5–6,5) |
| 4 | 0,53 (0,47–0,62) | 8 (6,5–9) | 0 | 0 | 1 (0,79–1) | 2 (0–4,5) |
| 5 | 0,5 (0,44–0,57) | 9,5 (8–11) | 0 (0–0,5) | 0 | 1 (0,88–1) | 2,5 (1–4) |
| 6 | 0,52 (0,44–0,61) | 13 (9–14) | 0 (0–1) | 0 | 1 (0,88–1,13) | 3,5 (2–5,5) |
| 7 | 0,66 (0,58–0,74) | 17,5 (13,5–19,5) | 1 (0,5–2,5) | 0 | 1,14 (1–1,31) | 6,5 (4,5–10,5) |
| 8 | 0,69 (0,63–0,78) | 21 (16,5–23,5) | 3 (1–4,5) | 0 | 1,29 (1,14–1,46) | 5 (3–7) |
| 9 | 0,79 (0,73–0,86) | 24 (19–27) | 5 (3–6,5) | 0 | 1,47 (1,33–1,65) | 6,5 (4–8,5) |
| 10 | 0,86 (0,77–0,94) | 27 (21–31) | 7 (4–8,5) | 0 | 1,63 (1,43–1,78) | 5 (3,5–8,5) |
| 11 | 0,88 (0,82–0,96) | 28,5 (23–32) | 8 (5–10,5) | 0 (0–0,5) | 1,78 (1,57–1,88) | 6 (3–8) |
| 12 | 0,92 (0,85–0,99) | 31 (23,5–34,5) | 10 (6,5–13) | 0 (0–0,5) | 1,86 (1,63–2) | 6 (3–7,5) |
| 13 | 0,93 (0,85–0,99) | 32,5 (24–35,5) | 11 (8,5–13,5) | 0 (0–1) | 1,87 (1,71–2) | 4 (2–8) |
| 14 | 0,95 (0,88–0,99) | 32,5 (24,5–38) | 14 (10–16,5) | 0 (0–1) | 1,89 (1,76–2,06) | 5,5 (3–8,5) |
| 15 | 1 (0,91–1,06) | 34 (27–40) | 15 (11–17) | 0,5 (0–1) | 2 (1,8–2,06) | 6 (3–8,5) |
| 16 | 1,03 (0,95–1,1) | 37 (28,5–41) | 16,5 (11,5–19) | 1 (0–2) | 2 (1,86–2,17) | 6 (1–10,5) |
| 17 | 1,03 (1,01–1,09) | 37,5 (30,5–44) | 18 (12,5–20,5) | 1 (0–2) | 2 (1,94–2,18) | 6,5 (1,5–9) |
| 18 | 1,07 (1,01–1,13) | 39,5 (31,5–44,5) | 19 (14–21,5) | 1,5 (0–3,5) | 2,13 (2–2,36) | 6 (2,5–9) |
| 19 | 1,09 (1,01–1,18) | 42 (32,5–46,5) | 21 (14,5–23) | 2 (0,5–4,5) | 2,18 (2–2,44) | 6 (2,5–10,5) |
| 20 | 1,1 (1,03–1,18) | 42,5 (33,5–47,5) | 21 (16–23,5) | 2,5 (1–5,5) | 2,29 (2,13–2,53) | 5,5 (2–9) |
| 21 | 1,16 (1,05–1,2) | 44 (34,5–50,5) | 21,5 (17,5–24,5) | 3 (1–6) | 2,38 (2,13–2,67) | 4,5 (3–10,5) |
| 22 | 1,16 (1,05–1,21) | 44 (36,5–50,5) | 22 (17–24,5) | 5 (2–6) | 2,4 (2,2–2,69) | 6 (1,5–9) |
| 23 | 1,17 (1,06–1,22) | 45,5 (37–51,5) | 23 (17,5–25) | 4 (2,5–6,5) | 2,5 (2,21–2,73) | 5,5 (3,5–11,5) |
| 24 | 1,18 (1,07–1,25) | 46,5 (37–52,5) | 23,5 (18,5–27) | 5 (3–7) | 2,53 (2,33–2,69) | 5,5 (3,5–10) |
| 25 | 1,17 (1,1–1,24) | 49 (38–54) | 24 (19–27) | 6 (3–7) | 2,56 (2,4–2,69) | 6,5 (1,5–9) |
| 26 | 1,19 (1,12–1,25) | 50 (39–55) | 24 (20–28) | 6 (3–9) | 2,54 (2,43–2,87) | 5,5 (2–9) |
| 27 | 1,18 (1,11–1,27) | 50 (38,5–55) | 24,5 (20,5–28,5) | 6,5 (3–9) | 2,61 (2,43–2,86) | 5 (2,5–11) |
| 28 | 1,19 (1,14–1,27) | 51 (40–56,5) | 25 (21–28) | 7 (4,5–9) | 2,71 (2,29–2,88) | 6,5 (3–13,5) |
| 29 | 1,21 (1,14–1,26) | 51 (40–57) | 25,5 (21,5–29) | 7,5 (4,5–9) | 2,68 (2,53–2,94) | 5 (1–8,5) |
| 30 | 1,22 (1,12–1,32) | 51,5 (42,5–58) | 26,5 (22–29,5) | 7 (4,5–9,5) | 2,69 (2,56–2,88) | 6,5 (3–10,5) |
| 31 | 1,2 (1,12–1,32) | 51,5 (42,5–57,5) | 27 (21,5–30) | 7 (5–9,5) | 2,71 (2,56–2,88) | 5,5 (2–11,5) |
| 32 | 1,23 (1,13–1,31) | 53 (43–60,5) | 27 (21–32) | 7 (6–10) | 2,82 (2,56–2,89) | 5 (4–9,5) |
| 33 | 1,23 (1,12–1,3) | 54,5 (43–60,5) | 27,5 (21–32) | 7,5 (5,5–9,5) | 2,73 (2,62–2,88) | 4,5 (1–9) |
| 34 | 1,24 (1,15–1,32) | 55 (43,5–62,5) | 28 (21–32,5) | 8 (5,5–10) | 2,79 (2,53–2,94) | 4 (2–7) |
| 35 | 1,22 (1,14–1,33) | 54 (44–64) | 28 (21–32) | 8,5 (5,5–10) | 2,73 (2,56–2,88) | 5,5 (2–8) |
| 36 | 1,23 (1,14–1,33) | 55 (45–64) | 27,5 (22–33) | 8,5 (6–10,5) | 2,82 (2,61–2,95) | 4,5 (2,5–8) |
| 37 | 1,23 (1,18–1,34) | 56 (45–64) | 28 (22,5–34) | 8 (6–10) | 2,75 (2,59–2,95) | 5 (1,5–7,5) |
| 38 | 1,24 (1,15–1,36) | 58 (45,5–65) | 28,5 (23–34) | 8 (7–11) | 2,73 (2,61–3) | 4 (1–8) |
| 39 | 1,24 (1,17–1,37) | 58 (46–65,5) | 29 (23,5–34) | 8 (6,5–11,5) | 2,78 (2,63–2,95) | 4 (2,5–7) |
| 40 | 1,24 (1,16–1,38) | 57 (45–64,5) | 29,5 (23–35,5) | 8,5 (6,5–12) | 2,79 (2,6–3) | 5 (3–10) |
| 41 | 1,25 (1,14–1,37) | 55,5 (45–65,5) | 29,5 (23,5–36) | 8,5 (6–11) | 2,86 (2,6–3) | 5,5 (2–9) |
| 42 | 1,26 (1,15–1,36) | 55,5 (47–65,5) | 29,5 (23–36) | 8,5 (7–11) | 2,87 (2,67–3) | 4,5 (1,5–6,5) |
| 43 | 1,27 (1,16–1,36) | 56,5 (47–66) | 30 (23,5–37) | 9 (7–11) | 2,87 (2,65–3) | 4 (2–7,5) |
| 44 | 1,26 (1,18–1,33) | 56,5 (47,5–67,5) | 30 (24,5–38,5) | 9,5 (6,5–11) | 2,86 (2,65–3) | 5 (1–9) |
| 45 | 1,25 (1,17–1,36) | 57,5 (47–68) | 30,5 (23,5–40,5) | 9,5 (7–11) | 2,86 (2,65–2,95) | 5 (1,5–9) |
| 46 | 1,27 (1,19–1,37) | 58 (49–70,5) | 31,5 (23,5–40,5) | 9 (6,5–11,5) | 2,86 (2,66–2,95) | 4 (2,5–6) |
| 47 | 1,27 (1,19–1,36) | 58 (48,5–70,5) | 30,5 (24–43) | 9 (6,5–11,5) | 2,78 (2,59–2,89) | 4,5 (3–10) |
| 48 | 1,27 (1,19–1,34) | 57,5 (48–70) | 31,5 (23,5–43) | 9,5 (6–11) | 2,87 (2,59–3) | 3,5 (1,5–8) |
| 49 | 1,28 (1,17–1,38) | 60,5 (48,5–70,5) | 32 (24,5–43) | 9,5 (7–11,5) | 2,87 (2,58–3) | 6 (2–8,5) |
| 50 | 1,29 (1,15–1,37) | 60,5 (49–75,5) | 31,5 (24,5–43,5) | 9,5 (7–12) | 2,86 (2,65–3) | 4 (1–8) |
| 51 | 1,29 (1,2–1,4) | 62,5 (49–74,5) | 32,5 (24,5–42) | 9,5 (7,5–12,5) | 2,89 (2,63–3) | 5 (3–8) |
| 52 | 1,28 (1,22–1,41) | 61,5 (50–74,5) | 32,5 (24,5–42,5) | 10 (7,5–12) | 2,89 (2,7–3) | 4,5 (1,5–7,5) |
| 53 | 1,3 (1,19–1,4) | 61,5 (49–77,5) | 33,5 (24,5–42,5) | 10 (7,5–11,5) | 2,89 (2,74–3) | 5 (1,5–11) |
| 54 | 1,3 (1,18–1,41) | 61,5 (48,5–78,5) | 34 (24,5–43) | 10 (7–12) | 2,88 (2,67–3) | 4 (3–9,5) |
| 55 | 1,31 (1,2–1,4) | 62,5 (49–78,5) | 34 (24,5–43,5) | 10 (7–12,5) | 2,87 (2,64–3) | 3 (1–5,5) |
| 56 | 1,31 (1,17–1,41) | 63,5 (50–79,5) | 33,5 (24–44) | 10,5 (7,5–13) | 2,86 (2,64–3) | 4 (1–8) |
| 57 | 1,3 (1,15–1,39) | 62,5 (49–79) | 33 (24–43,5) | 10 (7,5–12,5) | 2,89 (2,63–3) | 4,5 (3–10,5) |
| 58 | 1,32 (1,17–1,41) | 63,5 (51–80) | 33,5 (24–42,5) | 10 (8–13) | 2,88 (2,66–3) | 3,5 (2–7,5) |
| 59 | 1,32 (1,13–1,4) | 63 (47,5–80,5) | 33 (24,5–41,5) | 10 (8–13) | 2,86 (2,61–3) | 4 (1–8) |
| 60 | 1,31 (1,14–1,41) | 62 (45,5–79) | 34 (24–41,5) | 10,5 (8–12,5) | 2,86 (2,66–3) | 5 (2–8,5) |

### Deniz

| Yıl | Liman (tersane) | Gemi (koga/tekne) | Kadırga | Denizaşırı yerleşim | Deniz ticaret yolu (yıl sonu) | Deniz seferi (ticaret) |
|---|---|---|---|---|---|---|
| 1 | 0 | 0 | 0 | 0 | 0 | 0 |
| 2 | 0 (0–1,5) | 0 (0–1) | 0 | 0 | 0 | 0 |
| 3 | 1 (0–2) | 1 (0–1,5) | 0 | 0 | 0 | 0 |
| 4 | 1 (0–2,5) | 1 (0–2) | 0 | 0 | 0 | 0 |
| 5 | 1,5 (1–3) | 1 (1–3) | 0 | 0 | 0 | 0 |
| 6 | 2 (1–3) | 2 (1–3) | 0 | 0 | 0 | 0 |
| 7 | 2,5 (1–3,5) | 2 (1–5,5) | 0 | 0 | 0 | 0 |
| 8 | 2,5 (1–4) | 4 (1–7) | 0 | 0 (0–0,5) | 0 | 0 |
| 9 | 2,5 (1–5) | 6,5 (1,5–13) | 0 | 1 (0–2,5) | 1 (0–3) | 0 |
| 10 | 4 (1–6,5) | 8 (2,5–15) | 0 | 1 (0–3,5) | 1 (0–4) | 0 (0–1,5) |
| 11 | 5,5 (2–7) | 11 (4–19,5) | 0 | 2 (0,5–5) | 2,5 (0,5–7) | 0 (0–1,5) |
| 12 | 6 (4–8,5) | 11,5 (8–21) | 0 | 4 (0,5–6,5) | 4 (1–8) | 0 (0–3,5) |
| 13 | 7 (5,5–11,5) | 15 (10–24) | 0 | 4 (1,5–8) | 4 (2,5–9) | 0 (0–3,5) |
| 14 | 8 (5,5–12) | 17 (11,5–25) | 0 | 4 (1,5–8) | 5 (2,5–9,5) | 1,5 (0–5) |
| 15 | 8,5 (6–12) | 17,5 (12,5–27) | 0 (0–2,5) | 4,5 (1,5–8) | 5,5 (3–10) | 2,5 (0–6,5) |
| 16 | 9,5 (6,5–12,5) | 19,5 (13–27) | 0,5 (0–5,5) | 5 (2,5–8) | 6 (3,5–10,5) | 3 (0–8) |
| 17 | 10 (6,5–12,5) | 20,5 (13–28,5) | 2 (0–5,5) | 6 (2,5–8) | 6,5 (3,5–10,5) | 3 (0–8,5) |
| 18 | 10,5 (7–12,5) | 21 (14,5–31) | 2,5 (0–7,5) | 6 (3–8) | 7 (4–11,5) | 3 (0–10) |
| 19 | 10,5 (7,5–13) | 22,5 (16–33,5) | 2,5 (0–10,5) | 6,5 (3–8,5) | 7,5 (4–12,5) | 3 (0–10,5) |
| 20 | 10,5 (8–13,5) | 23 (17,5–34) | 3,5 (0–10,5) | 6,5 (3,5–8,5) | 7,5 (4,5–13) | 3 (0–11,5) |
| 21 | 10,5 (8,5–13,5) | 24 (17,5–38) | 5 (1–15) | 6,5 (3,5–9) | 7,5 (4,5–13,5) | 3 (0–11) |
| 22 | 11 (9–14) | 25 (19–38) | 5 (2–16) | 6,5 (3,5–9) | 7,5 (5–13,5) | 3 (0–10,5) |
| 23 | 11,5 (9,5–15) | 28 (19,5–40) | 6,5 (4,5–17) | 7 (3,5–9,5) | 8,5 (5–14) | 4 (0–12) |
| 24 | 12 (9,5–16,5) | 29 (20,5–40,5) | 8 (5–17,5) | 7 (3,5–9,5) | 9 (5–14) | 5 (0–14) |
| 25 | 13 (9,5–16,5) | 29 (22–40,5) | 11 (6,5–19) | 7 (3,5–10) | 9 (5–14,5) | 3,5 (0–14,5) |
| 26 | 13 (9,5–17,5) | 30 (22,5–41) | 11,5 (6,5–18,5) | 7 (3,5–10) | 9,5 (5–14,5) | 5,5 (0,5–15) |
| 27 | 13 (9,5–17,5) | 31,5 (22,5–41,5) | 12 (7,5–20) | 7 (4–10) | 9,5 (5,5–14,5) | 6 (1,5–14,5) |
| 28 | 13 (9,5–17,5) | 31,5 (23–44) | 11,5 (7,5–20,5) | 7 (4,5–10,5) | 9,5 (5,5–16,5) | 5,5 (1–17,5) |
| 29 | 13,5 (10–17,5) | 31,5 (23,5–47,5) | 12 (9–20) | 7 (4,5–10,5) | 9,5 (5,5–17) | 5,5 (1,5–19) |
| 30 | 13 (10–17,5) | 31 (24,5–48) | 13 (9–19,5) | 7 (4,5–11) | 9,5 (5,5–17,5) | 5 (2–16,5) |
| 31 | 13 (10–17,5) | 31,5 (25,5–48,5) | 13,5 (9,5–20,5) | 7 (4,5–11) | 9,5 (5,5–17,5) | 6 (3–18,5) |
| 32 | 14 (10–18) | 32,5 (25,5–48,5) | 13,5 (9,5–21,5) | 7 (4,5–11) | 9,5 (6–18,5) | 6 (2,5–20,5) |
| 33 | 13,5 (10–18) | 34,5 (25,5–52,5) | 14,5 (9,5–23) | 7,5 (4,5–11) | 9,5 (6–19) | 6 (3–22,5) |
| 34 | 13,5 (10–18,5) | 35,5 (25,5–53,5) | 15 (9,5–22,5) | 8 (4,5–11,5) | 9,5 (6–19,5) | 6 (3–23,5) |
| 35 | 13,5 (10–19,5) | 35,5 (27,5–54) | 15 (10–22,5) | 8 (4,5–12) | 10 (6–20,5) | 6 (3,5–23) |
| 36 | 13,5 (10–19,5) | 34,5 (27,5–53) | 14,5 (10,5–23) | 8 (4,5–12) | 10 (6–20,5) | 6 (3–24) |
| 37 | 14 (10–19,5) | 34,5 (27,5–54) | 14,5 (11–22) | 8 (4,5–12) | 10,5 (6–21,5) | 6 (3–24) |
| 38 | 14 (9,5–19,5) | 35 (27,5–55) | 14,5 (10,5–22) | 8 (5–12) | 10,5 (6,5–21,5) | 6 (4–24,5) |
| 39 | 14 (10,5–19,5) | 35 (28–56,5) | 16 (10,5–23) | 8 (5–12) | 10,5 (6,5–21,5) | 6 (4–25,5) |
| 40 | 14,5 (11,5–20) | 35,5 (29–54,5) | 17 (10,5–23,5) | 8 (5,5–12) | 10,5 (7–22) | 7 (3,5–26) |
| 41 | 14,5 (11,5–21) | 36 (29–56,5) | 17 (10,5–23,5) | 8 (5,5–12,5) | 10,5 (7,5–22,5) | 8,5 (3,5–25) |
| 42 | 14,5 (11,5–21) | 36 (29–55,5) | 17 (11–23,5) | 8 (5,5–12,5) | 10,5 (7,5–23) | 7 (3,5–27) |
| 43 | 15 (12–21,5) | 36,5 (29,5–58) | 17 (11–23,5) | 8 (6–12,5) | 11 (8–23) | 7,5 (5–25,5) |
| 44 | 15 (12,5–22) | 38 (31–61) | 17 (11–23) | 8 (6–13) | 11 (8–23,5) | 7,5 (5–20,5) |
| 45 | 15 (13–22,5) | 38 (32–64) | 17 (11–23) | 8 (6–14) | 11,5 (8–24,5) | 9 (5–21,5) |
| 46 | 15,5 (13–23,5) | 39 (32–64,5) | 16,5 (11–22,5) | 8,5 (6–14,5) | 12 (8–25,5) | 9,5 (5,5–26,5) |
| 47 | 16 (13–24,5) | 40,5 (32,5–64,5) | 16,5 (11–23) | 8,5 (6–15) | 12 (8–25,5) | 12,5 (6–25,5) |
| 48 | 16 (13–25) | 42,5 (32,5–66,5) | 16,5 (11–24,5) | 8,5 (6–15) | 12 (8–27,5) | 11 (6–29,5) |
| 49 | 16 (13–25) | 44 (32,5–67) | 16 (10,5–24,5) | 8,5 (6–15) | 12 (8–27,5) | 13 (6,5–33,5) |
| 50 | 17 (13–25) | 44,5 (32,5–67) | 16 (10,5–24,5) | 9 (6–15) | 12,5 (8–27,5) | 13 (4,5–32) |
| 51 | 17 (13–25,5) | 44,5 (32,5–66,5) | 16 (10,5–23) | 9 (6–15) | 12,5 (8–27,5) | 15 (6–32,5) |
| 52 | 17 (13–25,5) | 46 (33–67,5) | 16 (10,5–23) | 9 (6–15) | 12,5 (8–28) | 13,5 (5,5–33) |
| 53 | 17 (13–25,5) | 47,5 (33,5–67,5) | 16 (10,5–23) | 9 (6–15,5) | 12,5 (8–29,5) | 16 (6,5–38,5) |
| 54 | 17 (13–26) | 47,5 (33,5–68,5) | 16 (10,5–23) | 9 (6–16) | 12,5 (8–31) | 16 (5,5–38) |
| 55 | 17 (13–26,5) | 46 (33,5–68,5) | 16 (10,5–23) | 9 (6–16) | 12 (8,5–31) | 14 (7–39) |
| 56 | 17 (13–26,5) | 46 (33,5–69) | 16 (10,5–23,5) | 9 (6–16,5) | 12 (8,5–31,5) | 12,5 (5,5–36,5) |
| 57 | 17,5 (13–27) | 46 (33,5–69) | 16 (10,5–23,5) | 9 (6–16,5) | 12,5 (8,5–31,5) | 14 (7–37) |
| 58 | 18 (13–27) | 46 (34–69) | 16 (10,5–23,5) | 9 (6–17) | 12,5 (8,5–32) | 12,5 (6–37) |
| 59 | 17,5 (13,5–27,5) | 46,5 (34,5–68,5) | 16 (10,5–23,5) | 9 (6–17) | 12,5 (8,5–32) | 14,5 (6,5–40) |
| 60 | 17,5 (12,5–27,5) | 47,5 (34,5–69) | 16,5 (10,5–23,5) | 9 (6–17) | 12,5 (8,5–32) | 11,5 (5–40) |

