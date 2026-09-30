# Fantastik Dünya — simülasyon (v0.3)

- `src/data/` — içerik: `goods.ts` (mallar, yataklar, L1–L3 çıkarma, atölyeler, yapılar), `techs.ts` (42 düğümlü ana ağaç + sınıf ağaçları), `classes.ts` (12 D&D sınıfının hepsi oynanır; Paktçı ilk kötü medeniyet; harikalar; ırklar, birimler, canavarlar), `heroes.ts`, `inn.ts` (han kademeleri, personel, misafir türleri)
- `src/sim/` — görüntüden bağımsız, seed'li, deterministik simülasyon
  - `sim.ts` çekirdek ve gün döngüsü · `economy.ts` iş gücü, üretim, tükenme, inşaat · `research.ts` ağaç, çağlar, alt sınıflar
  - `diplomacy.ts` ilişkiler, kaynak anlaşmazlıkları, antlaşma, savaş, ticaret, genişleme · `monsters.ts` goblin istilası
  - `heroes.ts` taverna ve görevler · `inns.ts` tarafsız hanlar, açık artırma, misafir hakkı · `innlife.ts` yaşayan han: hancı kafilesi, inşaat, yolcular, kiler, erzak alımı, personel, ün, kademe, defter · `will.ts` kahraman iradesi (yollar, hedef seçimi, izler, efsane) · `sea.ts` denizcilik: kıyı suları, liman, kara + deniz yol bulma, gemi hesabı, keşif, fırtına, deniz savaşı, kadırga akını, korsan koyları ve korsan avı, yanardağ · `agents.ts` kervan/ordu/baskın, çatışmalar ve ortak saldırı (hedefte toplanma) · `combat.ts` d20, moral, güç tahmini ve savaş kaydı · `worldgen.ts` 110×75 organik kıta, seed'e göre ada profili (kıta / takımada / büyük adalar), adlı ve türlü adalar
- `src/view/` — görünüm: `diorama.ts` 3B dünya (arazi, ırka göre binalar, surlar, takip ve sinema kamerası), `terrain.ts` hex'e hizalı yükseklik alanı ve gürültü, `freecam.ts` gezgin kamerası (uç / yürü), `figures.ts` figürler (ırk/sınıf/iş hareketleri), `atmosphere.ts` deniz dalgası, bulut, kar, yağmur, yıldızlar (gemiler ve tersaneler `diorama.ts` içinde); `main.ts` panel, Cepheler sekmesi ve 2B debug harita, `innpanel.ts` hana tıklayınca açılan sağ panel, `battleview.ts` Savaş Tiyatrosu (zar zar tekrar), `dice3d.ts` 3B zar tepsisi
- `src/headless.ts` — görselsiz koşu ve başarı kriterleri

```
npm i
npx tsx src/headless.ts 1,2,3,4,5 1800 -v   # 5 seed, 30 dk'lık oyun süresi
node build.mjs                               # dist/fantastik-dunya.html
```

Komutlar: `npm run sim`, `npm run sim:v`, `npm run build`, `npm run typecheck`.

## Dokümanlar
- `docs/art-pipeline.md` — Blender → oyun asset hattı (`assets/blender/*.py` script'le üretilen modeller, `assets/models/*.glb`)
- `docs/gdd-v0.1-gpt-taslak.md` — ilk GPT taslağı (referans)
- GDD v0.2 (güncel): https://claude.ai/code/artifact/36f0d7a4-b4ae-435d-bd75-ee7ac7e7d330
- Debug görünümü (yayınlanmış): https://claude.ai/artifact/UoSarqUa8VuAo28idCwh9p
