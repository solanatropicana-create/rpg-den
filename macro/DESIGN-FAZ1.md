# Faz 1 tasarımı: dünya çekirdeği değişiklikleri (C#)

**Durum:** TS simülasyonunun C# portu bitti ve altın testten geçti: 3 seed × 7200 gün, her gün birebir (git etiketi `port-exact`). Bundan sonra C# kendi yolunda ilerler; TS'ye karşı altın test artık beklenmez. Yerine üç güvence gelir:

1. **Determinizm:** aynı seed, aynı tarih.
2. **Kayıt/yükleme eşdeğerliği:** kaydet → yükle → devam et, kesintisiz koşuyla gün gün aynı.
3. **Ölçüm panosu:** 16 dünya × 60 yıl; aşağıdaki bitiş ölçütleri otomatik denetlenir.

Kural: değişiklikler `PORTING.md`'deki deterministik yazım kurallarına uyar (JsMath, J.Sort, JsObj sırası, RNG sırası). Statik değişken durum yok; bütün durum `World`'de.

## Dalga A

### A1. Ölçüm aracı (`FD.Macro.Run stats`)

- `stats --seeds 1-16 --years 60 --out <dir> [--jobs N]`: dünyaları paralel koşar.
- Her dünya ve her yıl için ölçüm:
  - medeniyet, yerleşim ve nüfus; çağ dağılımı; araştırma ağacının yüzde kaçı bitti;
  - büyük olay sayısı ve türlere göre olaylar;
  - savaş, muharebe, fetih, kurulan ve terk edilen yerleşim, yok olan medeniyet, çöküş;
  - yaşayan, doğan ve temizlenen kamp, baskın;
  - kahraman: doğan, ölen (nedeniyle), emekli, efsane; seviye dağılımı;
  - han, ilan; altın medyanı, boştaki iş gücü, ticaret.
- Çıktı:
  - dünya başına JSON;
  - toplu rapor (Markdown + JSON): her ölçüt için yıllık medyan, p10 ve p90;
  - bitiş ölçütleri tablosu (✓/✗ ve değerler).
- Temel ölçüm `port-exact` üzerinde alınır ve `reports/` altına yazılır.

### A2. Kayıt/yükleme

- `Sim.Save(path)` / `Sim.Load(path)`: sıkıştırılmış JSON.
- Nesne paylaşımı korunur (`$id`/`$ref`). Örnekler: savaş nesnesi, aynı yol listesini paylaşan ajan ve rota, handaki misafir ile yolcu ajanı.
- Yol ve deniz önbellekleri ile `ShoreW` de kaydedilir; bunlar sonucu etkiler.
- Test: farklı K günlerinde kaydet → yükle → N'ye kadar devam et. Gün gün kanonik hash kesintisiz koşuyla aynı olmalı.

### A3a. Kahraman kimliği ve ilerlemesi (#2, ve #1'in kahraman kısmı)

**Seviye ve XP**
- Doğuş seviyesi: Sv1, %30 olasılıkla Sv2; medeniyetin `heroLevel` etkisi +1 ekler; tavan Sv3. Yıl geçtikçe artmaz; bugün `Year/6` ile şişiyor.
- Tavan Sv10. XP eşikleri D&D 5e'deki gibi: 0, 300, 900, 2700, 6500, 14000, 23000, 34000, 48000, 64000.
- XP ödülleri meydan okumaya göre ölçeklenir. Hedef, 60 yıllık bir dünyada:
  - çoğu kahraman Sv1–5'te ölür ya da emekli olur;
  - bazıları Sv6–8'e çıkar;
  - Sv9–10 nadirdir.
- Seviye atlama: can artar; yeterlik bonusu Sv1–4'te 2, Sv5–8'de 3, Sv9–10'da 4. Sv5'teki özellikler (ateş topu, iki saldırı) korunur; Sv7'de +1 AC; Sv9'da sınıfa göre ölçülü bir güç.

**Efsanelik**
- Seviyeden bağımsızdır, **ün (renown)** puanıyla gelir.
- Ün kaynakları: kamp önderini öldürmek, kamp temizlemek, ilan bitirmek, kahramanın sağ kaldığı zafer, düello, doğal 20, kasaba savunmak. İleride ejderha ve trol de eklenecek.
- Hedef: dünya başına 60 yılda 1–6 efsane.

**Ölüm ve kimlik**
- Ölüm kalıcıdır. Rahibin diriltme gücü istisna olarak kalır.
- Ad:
  - her ırk için en az 24 ad ve ırka göre soyad ya da lakap havuzu;
  - tam ad ("Ad Soyad"), yaşayan ya da son 10 yılda ölen bir kahramanınkiyle çakışmaz.
- Günlük:
  - 6 satır sınırı kalkar; tavan 400 satır, taşınca en eskisi atılır;
  - ayrıca kilometre taşları (`Deeds`) tutulur: ilk kan, ilk doğal 20, sözleşme, önder öldürme, efsanelik, ölüm yeri ve katili.
- Kahraman ölünce kalıcı kronikte bir paragraflık **destan** yazılır. Aynı metin `Hero.Epitaph`'a da kaydedilir.

**Hatalar**
- Zafer satırı ölen kahramanları da sayıyor (`agents.ts:558`).
- Harabe hedefi çoğu dünyada hiç oluşmuyor (`will.ts:239`).

### A3b. Temizlik (#1'in geri kalanı) ve kalıcı kronik

- **Kalıcı kronik:** `World.Chronicle` her büyük olayı tutar ve kırpılmaz. `World.Events` 2500 kayıtta kesilmeye devam eder.
- **Çalınan teknoloji:** o an araştırılan düğüm çalınınca iki kez sayılıyor; düzeltilir.
- **Okunmayan etkiler** veri açıklamalarına göre çalışır hâle gelir:
  - `prodWood`: kereste verimi;
  - `harvest`: hasat kayıpsız;
  - `teleport`: kendi yerleşimleri arasında hızlı yolculuk.
- **Darphane:** gelir yerleşim başına olur, büyüklükle ölçeklenir. Bugün her yerleşimde kurulduğu hâlde gelir medeniyet başına tek.
- Bulunan diğer açık hatalar raporlanır.

## Dalga B (A1'in temel ölçümünden sonra kesinleşir)

### B1. Yükseliş ve çöküş (#5)

- **Yerleşim tavanı:** sabit 5 yerine `2 + çağ + ⌊nüfus/200⌋`.
- **Başkent düşebilir.** Başka yerleşim varsa başkent en büyüğüne geçer; son yerleşim de düşerse medeniyet yok olur.
- **Bölünme:** başkente uzak, yaralı ya da mutsuz bir yerleşim (yağma, kıtlık, farklı çoğunluk ırkı) ayrılıp yeni medeniyet kurabilir. Medeniyet başına 10 yılda en fazla 1 bölünme olur.
- **Kutsal Sefer:** kötü bir medeniyetin saldırısında iyi hizalı medeniyetler çağrılır.
- **Savunma paktı:** paktlı birine saldıran, ortağını da karşısında bulur. Paktı bozan herkesle "İhanet" damgası yer.

### B2. Anlatıcı ve geç tehdit (#7)

- **Gerilim bütçesi:** son 2 yılın ölüleri, yangınları ve kıtlık günleri. Uzun sessizlikte kriz gelir; zirveden sonra bir süre rahatlama olur.
- **Hedef kamp sayısı:** `3 + yıl/6`. Bugün yeni kamp ancak 2'den az kamp kaldığında doğuyor.
- **Trol çeteleri:** 15. yıldan sonra çıkar; güçlüdür ve kendini iyileştirir.
- **Ejderha:** 18–22. yıllar arasında dağdaki inine uyanır.
  - Yakar, hazine biriktirir; hanlara ilan düşer.
  - Medeniyetler ya ittifakla saldırır ya da haraç öder.
  - Ejderhayı öldüren parti efsane olur.

### B3. Kronik zinciri (#4)

- `GameEvent.Parent`: olay, kendisine yol açan olaya bağlanır.
- Zincirler:
  - baskın → yanan tarla → kıtlık → göç → terk;
  - salgın → ölümler → terk;
  - savaş ilanı → muharebe → fetih.

## Faz sonu

- **#6** Sınıflar farklı bitsin.
- **#8** Altın ve ambar anlam kazansın.

Kapsamları dalga B'nin ölçümünden sonra belirlenir.

## Bitiş ölçütleri (16 dünya × 60 yıl, ölçüm aracı denetler)

1. **Donma yok:** 41–60. yılların yıllık büyük olay medyanı, 6–20. yılların medyanının en az 0,8 katı.
2. **Çöküş:** dünyaların en az %75'inde, 60 yıl içinde en az bir çöküş olur (medeniyet yok olur ya da başkentini kaybeder).
3. **Kamplar:** 41–60. yıllarda yaşayan kamp medyanı, 6–20. yılların medyanından az değil.
4. **Kahramanlar:**
   - doğuş seviyesi her on yılda ortalama en fazla 2;
   - dünyaların en az yarısında en az bir kahraman Sv8 ve üstüne çıkar;
   - dünya başına efsane medyanı 1–6;
   - doğan kahramanların %30–80'i ölür.
5. **Determinizm ve kayıt/yükleme:** her koşuda birebir aynı.
