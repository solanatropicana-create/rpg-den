# Fantastik Dünya: Civ gözüyle rapor (v0.23)

Kanıtlar: `evidence/civ/` (trace, headless ve png dosyaları).

## 1. İlk izlenim
İlk 10 yıl gerçekten "bir tur daha" dedirtiyor. Kasabalar kuruluyor, goblin karakolları büyüyor ve her olayın bir "nedeni" var. Ama 15. yıldan sonra dünya kendini bitiriyor. Seed 1'de 31. yılda sekiz armanın hepsi IV, hepsinin 5 yerleşimi ve 46 düğümü var (`civ-s1-y31-kiyas.png`). Harita büyüyor ama tarih yazmıyor.

## 2. Neyi iyi yapıyor
- **Nedensellik okunuyor:** Büyük olayların hepsinde "←neden" satırı var (örnek: "İlişki -76: yağma baskını, toprak hırsı, dökülen kan"). İlişki paneli etkenleri tek tek sayıyor (`02-rapor-medeniyetler.png`).
- **Alt sınıflar gerçekten çeşitli:** 12 seed'de her sınıf 2–3 farklı doktrine gidiyor (`subclass-12seed.txt`).
- **Sınıfların yıllık olayları kişilik veriyor:** Kan Büyücüsü'nün büyü dalgası, Paktçı'nın ödediği bedel, Ozan festivali.
- **Saldırgan dünyalarda yerleşimler el değiştiriyor:** Seed 5'te 20 fetih var, nüfus liderliği 12 kez el değiştiriyor (`trace2`).

## 3. Neyi eksik/zayıf
1. **Yakınsama sert bir tavandan geliyor.** `diplomacy.ts:223` koşulu (`ss.length >= 5`) yüzünden fetih yapmayan her medeniyet 5 yerleşimde donuyor. Barışçıl dünyalarda 13. yıldan sonra yeni yerleşim olayı çıkmıyor (seed 1/4, `trace-1456`). 30. yılda nüfuslar birbirine yakın (seed 1: 299–457).
2. **Ağaç dallanmıyor, bitiyor.** Birbirini dışlayan düğüm yok, herkes 38+8 düğümün hepsini alıyor. Ağaç 14.–28. yıllar arasında tükeniyor. Çoğu medeniyet 30 yılın 11–17'sini "Açık düğüm kalmadı" durumunda geçiriyor (`trace2`, `civ-s1-y31-medeniyetler.png`). Medeniyetleri ayıran tek şey araştırma sırası.
3. **Çağ geçişleri toplu ve sessiz.** Bütün medeniyetler 2,6–4,5. yıllar arasında Köy çağına geçiyor, Kasaba'ya geçişlerin çoğu 6,3–8,3. yıllar arasında. Geçişi yalnızca "Yeni çağ!" halkası gösteriyor (`main.ts:91`); dünya ölçeğinde bir çağ anı yok. Takılan medeniyet boşta çürüyor: seed 5'teki Rahip 75 nüfus şartını tutturamadığı için 15 yıl III. çağda araştırmasız kalıyor.
4. **Savaşların sonucu kalıcı değil.** Başkent fethedilemiyor (`agents.ts:673` `if (!isCap)`). Savaş tek kuşatmadan ibaret: hedef düşünce barış geliyor (`diplomacy.ts:162`). Kaybeden kaybettiğini yeniden kuruyor: seed 5'te Paktçı 8 yerleşim kaybedip 9 yenisini kuruyor. Kutsal Sefer 6 kez açılıyor ve her seferinde tek bir kasaba alıyor; "paktı yıkmak" hedefine hiç ulaşılmıyor. Hiçbir medeniyet yok olmuyor, test ölçütünün kendisi de "yokOlmaz" (`headless.ts:52`). Barışçıl dünyalarda (seed 1, 4) 30 yılda tek savaş çıkmıyor.
5. **Diplomasi seyrek, blok yok.** Temas yalnızca iki yerleşim en fazla 16 hex yakınsa kuruluyor (`diplomacy.ts:24`). Seed 1'de 28 çiftten yalnızca 12'si 30 yılda tanışıyor. İttifak mekaniği yok: "Kutsal Sefer: ittifak çağrısı" metni var ama kodda karşılığı yalnızca bir yorum (`diplomacy.ts:157`). Barışçıl dünyalarda 20. yıldan sonra ilişki etiketleri yılda 0–1 kez değişiyor.
6. **Geç oyun boş, sayılar şişiyor.** Seed 1'in 26–30. yıllarında yılda ~17 büyük olay var, çoğu macera grubu ve müzayede; araştırma, yerleşim ve savaş sıfır. Altın 20. yıldan 30. yıla 3–10 kat artıyor (Paladin'de 5.357'den 15.287'ye) ve harcanacak yer yok (`trace3-seed1.txt`). Harikalar sınıfa özel olduğu için yarış çıkmıyor (`economy.ts:507`). Kıyas yalnızca nüfusu çiziyor (`main.ts:848`); `history` altın ve bilgiyi tutuyor ama grafikte göstermiyor. Kronik 2500 olayda kırpılıyor (`sim.ts:53`), ilk yıllar kayboluyor.

## 4. Kalite artırma önerileri (hiçbiri oyuncu rolü gerektirmiyor)
1. **Tavanları kaldır.**
   - Ne yapılır: Sabit 5 yerleşim sınırı çağa bağlı yumuşak bir sınıra dönüşür (2 + çağ); her ek yerleşim huzursuzluk ve bakım maliyeti getirir. Başkent, medeniyetin 2'den az yerleşimi kaldığında ya da 3. başarılı kuşatmada düşer. Sonuç vasallık (yıllık haraç, efendinin savaşlarına katılma) ya da yok oluştur. Vasal 10 yıl sonra bağımsızlık savaşı açabilir.
   - Neden: Medeniyetler farklılaşır, yükselen ve düşen eğriler ortaya çıkar, harita kalıcı olarak değişir.
   - Efor: M.
   - Dosyalar: `diplomacy.ts`, `agents.ts`, `types.ts`, `headless.ts` ("yokOlmaz" ölçütü yerine "saldırgan dünyada en az 1 vasal/yok oluş").
2. **Doktrin çatalları.**
   - Ne yapılır: III–IV. çağda 4–5 düğüm çifti `excl` alanı alır (Taş Surlar ↔ Kuşatma, Yüksek Büyü ↔ Taç ve Kanun, Ticaret Ağları ↔ Derin Kazı…); biri alınınca öteki kilitlenir. IV. çağ maliyeti yaklaşık 2 katına çıkar, böylece ağaç 30 yılda bitmez. Ağaç raporunda kapanan dal üstü çizili görünür.
   - Neden: Medeniyetler farklı yerlere varır ve ağaç okunabilir bir hikâyeye dönüşür.
   - Efor: M.
   - Dosyalar: `techs.ts`, `research.ts` (availableTechs, chooseResearch), `main.ts` (renderTree).
3. **Dünya Harikaları ve Yükseliş Yarışı.**
   - Ne yapılır: 6 tekil dünya harikası eklenir: ilk bitiren alır, kaybeden kaynağının %50'sini geri alır ve kazanana karşı −15 kıskançlık modu edinir. Ardından üç zafer izi açılır: Fetih (başkentlerin yarısı), Efsane (3 efsane kahraman + 2 harika), Büyük Ritüel (Yüksek Büyü sonrası 5 yıllık proje). Bir iz tamamlanınca çağ kapanır. Lider bir izde %60'ı geçince öbür medeniyetler "ortak tehdit" modu alır ve savaş eşikleri düşer.
   - Neden: Geç oyuna bir yükseliş eğrisi, altına anlam, harika yarışı ve lidere çullanma dramı gelir.
   - Efor: L.
   - Dosyalar: yeni `victory.ts`, `economy.ts:507`, `diplomacy.ts`, `main.ts`.
4. **İttifak, çağrı, ihanet.**
   - Ne yapılır: İlişki iki yıl boyunca ≥40 kalırsa Savunma Paktı kurulur. Savaşta müttefik yardım çağrısını kabul ya da reddeder (ret: −25 "Yüzüstü bıraktı"). Paktı bozmak −40'lık, yavaş sönen bir "İhanet" modu getirir. Kutsal Sefer bütün iyi hizalı medeniyetleri çağırır. Temas kâşif, kervan ve han üzerinden de kurulabilir. Medeniyetler raporuna bir blok matrisi eklenir.
   - Neden: Diplomasi okunur hale gelir; bloklar ve ihanet hikâyesi doğar.
   - Efor: M.
   - Dosyalar: `diplomacy.ts`, `agents.ts` (considerScout), `types.ts`, `main.ts`.
5. **Kıyas'ı tarih kitabına çevir.**
   - Ne yapılır: `history`'ye toprak (hex sayısı) ve askerî güç eklenir. Kıyas'a Nüfus / Toprak / Bilgi / Altın (log ölçek) / Güç sekmeleri gelir. Eğrilerin üstüne ⚔ savaş, ⚑ fetih, ★ harika ve çağ işaretleri, ayrıca "lider değişti" bandı konur. Çağ, savaş, fetih, harika ve yok oluş olayları ayrı ve kırpılmayan bir `w.chronicle`'da tutulur.
   - Neden: Tarihi geriye bakıp okumak, Civ'in en tatlı anlarından biri.
   - Efor: S.
   - Dosyalar: `sim.ts`, `types.ts`, `main.ts` (popChart, renderCompare).

*Oyuncu rolü gerektiren ek öneri:* Hancı rolü gelirse, 4. öneriye hancının pakt arabuluculuğu eklenebilir.

## 5. Tek cümle
Eren yalnızca bir şey yapacaksa, o şey şu olmalı: dünyaya bir bitiş çizgisi koymak, yani tekil Dünya Harikaları ve zafer yarışıyla herkesin 15. yıldan sonra aynı noktada durduğu anı, lidere karşı birleşilen bir yükseliş eğrisine çevirmek.
