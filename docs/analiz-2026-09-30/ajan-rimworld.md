# Fantastik Dünya: RimWorld gözüyle (v0.23)

Kanıtlar `evidence/rimworld/` altında: `tempo-seed1..5.txt` (5 seed × 30 yıl, tüm olaylar ve ölüm nedenleri), `kimlik.txt`, `kitlik-seed3.txt`, `r1-kitlik-yakin.png`. Bunları üreten betikler de aynı klasörde: `tempo.ts`, `kimlik.mjs`, `kitlik3.ts`.

## 1. İlk izlenim
Açılış çekimi insanı içine çekiyor. Ama bir kasabalıyı bir yıl sonra yeniden açtığımda aynı evde, aynı yaşta, bu kez insan olan bir "Gareth" buldum. Nüfus bir sayaç, figürler de onun kostümü. Kahramanlar gerçek, köylüler dekor.

## 2. Neyi iyi yapıyor
- **Krizin ekonomiye etkisi somut.** Baskında yanan maden ya da tarla yeniden kurulana dek üretmiyor (agents.ts:474–482). Barakalar salgın riskini artırıyor (events.ts:32). Ekranda ekmek kuyruğu, karantina bayrağı ve ceset arabası çıkıyor, salgında sokaktaki figür sayısı %35'e iniyor (diorama.ts:2421; r1-kitlik-yakin.png).
- **Kahramanlar gerçek bireyler.** Zarları, izleri ve lakapları var. Doğduğu yeri yakan kampa intikam, yakan medeniyete kin duyuyorlar ve bunlar kararlarını değiştiriyor (will.ts:117–127).
- **"← neden" alanı önemli olayların %81–88'inde var.** Savaş ilanında RimWorld'ün düşünce listesi gibi okunuyor: "İlişki −37: farklı değerler, dökülen kan, geçmiş savaşın izleri…"
- **Savaşçı dünyada gerçek felaket yayı oluşuyor.** Seed 3'te Yıl 25–28 arasında kıtlık, 55 açlık ölümü ve 2 terk edilmiş yerleşim var.

## 3. Neyi eksik/zayıf
1. **Kasabalılar dekor.** Simülasyonda nüfus ırk başına bir sayı (types.ts:6). Ad, yaş, soyad ve iş, figürün sıra numarasından hash ile üretiliyor (diorama.ts:2164–2169, 2237). Ölüm, rastgele bir ırkın sayısından 1 düşmek (sim.ts:66). kimlik.txt'teki sonuçlar:
   - Aynı 10 kasabalıdan 1 yıl sonra 5'i, 3 ev eklenince 30 günde hiçbiri aynı kişi değil.
   - Yaş hiç artmıyor.
   - 61 cüceli kasabaya gelen tek insan, figürlerin yarısını insana çeviriyor (:2167).
   - 20 kişinin 13'ünde kişinin işi kendi hanesinin üye listesinde yok.
2. **Anlatıcı yok.** Kriz olasılıkları sabit (events.ts:33, 53).
   - İlk 3 yılda neredeyse hiç kriz yok.
   - Barışçıl seed 1 ve 4'te Yıl 7'den sonra ayların %68–71'i krizsiz, en uzun sessizlik 10–12 ay.
   - Seed 1'de nüfus 67'den 3194'e çıkıyor ve bir kez bile düşmüyor.
   - Savaşçı seed'lerde krizsiz ay oranı %18–26.
3. **Krizler sessiz geçiyor.**
   - Salgın başına ~3 ölü düşüyor.
   - Seed 1'deki ölümlerin %94'ü olay akışına düşmeyen "doğal" ölüm (economy.ts:252).
   - Açlıktan ölüm hiç kaydedilmiyor (economy.ts:234), kıtlık da önemli olay sayılmıyor (:231).
   - Seed 3'te Çarkyıldız'ın nüfusu 195'ten 55'e inerken akışta yalnızca 2 göç ve 2 terk görünüyor. "Kıtlık başladı" ise ±1 yıl içinde 18 kez tekrarlanıyor.
   - Ambarında 5–15 bin gıda olan komşular 2,5 yıl boyunca tepki vermiyor.
4. **Neden zinciri tek halkalı, bazen de yanlış.**
   - Olayların önceki olaya bağlantısı yok, neden düz metin (types.ts:238).
   - "Gıda stoğu tükendi" nedeni olayın kendisini tekrar ediyor.
   - Kahraman salgını geriletse de bitiş "Hastalık kendi yolunu tamamladı" diye yazılıyor (events.ts:24).
   - Kahraman ölümünün nedeni "0 düşman devirmişti". Nerede ve kime karşı öldüğü yazmıyor (agents.ts:113).
5. **Kaybın izi kalmıyor.**
   - Mezar yalnızca salgında ekleniyor (events.ts:22). Baskında 11 kişisini yitiren kasabaya tek taş eklenmiyor.
   - Yoldaş ölünce tek tepki, yalnızca partinin tek sağ kalanına verilen "Yalnız Kurt" izi (will.ts:156).
   - Kahraman günlüğü 6 satırla sınırlı (will.ts:31).
   - "Kayıp kardeşini arıyor" gibi dürtüler yalnızca süs (heroes.ts:51).
6. **Kahraman kimlikleri bulanık.**
   - Kahramanların %49–58'i tam adını başka bir kahramanla paylaşıyor. Seed 3'te aynı mevsimde üç ayrı "Ovak" var.
   - ~250 kahramandan 0–1'i efsane oluyor. %25–32'si "kimse tutmayınca uzak diyarlara gitti".
   - Kahramanların kendi seçtiği hedefler çoğunlukla kütüphane ve hac (seed 3: 434'e karşı 11 av).

## 4. Kalite artırma önerileri (hiçbiri oyuncu rolü gerektirmiyor)
1. **Kalıcı kasabalı kaydı**
   - **Ne yapılır:** Yerleşime `people: Person[]` eklenir: id, ad, hane, doğum, iş, ev, eş/ebeveyn, 2 huy, 5 anı. `pop` bu listeden hesaplanır. Doğum haneye çocuk ekler. Ölüm nedene göre birini seçer ve adıyla kaydedilir. Görünüm figürü sıra numarasıyla değil kişi id'siyle çizer.
   - **Neden:** Tıkladığın kişi yarın da aynı kişi olur; "Taşkalp hanesinden Helga vebadan öldü" yazılabilir.
   - **Efor:** M (ad/yaş/hane), L (akrabalık, ruh hâli).
   - **Dosyalar:** types.ts, sim.ts, economy.ts, events.ts, agents.ts, diorama.ts, main.ts
2. **Anlatıcı**
   - **Ne yapılır:** `storyteller.ts` bir gerilim puanı tutar: son 2 yılın ölüleri, yangınları ve kıtlık günleri. Uzun sessizlikte kriz tetikler (kuraklık, sert kış, canavar göçü, ejderha). Zirveden sonra soğuma dönemi ve rahatlama olayları gelir (bolluk hasadı, gezgin kervanı). Olay olasılıkları bu çarpanla çalışır. Kıyas raporuna gerilim çizgisi eklenir.
   - **Neden:** %70 krizsiz ayı kırar ve geç oyuna hedef verir.
   - **Efor:** M
   - **Dosyalar:** storyteller.ts (yeni), sim.ts, events.ts, monsters.ts, main.ts
3. **Okunur neden zinciri**
   - **Ne yapılır:** Olay kaydına `parent?: number` eklenir: baskın → tarla yandı → kıtlık → göç → terk. Kıtlığın gerçek nedeni hesaplanıp yazılır. Bitiş satırları kahramanı anar, kahraman ölümünde yer ve düşman yazar. Kronik'te bir olaya tıklayınca zincir açılır. Aynı tam adla ikinci bir kahraman üretilmez.
   - **Neden:** "Neden oldu?" sorusunun her zaman bir cevabı olur.
   - **Efor:** S–M
   - **Dosyalar:** types.ts, sim.ts, economy.ts, events.ts, agents.ts, heroes.ts, main.ts
4. **Yas ve anma**
   - **Ne yapılır:** Her ölüm nedeniyle birlikte mezara yazılır. 5'ten fazla ölümde kasaba bir mevsim yas tutar: kara bayrak asılır, bayram iptal edilir, verim −%10 olur, anma olayı düşer. Birlikte iki sefere çıkan kahramanlar yoldaş olur. Yoldaşı ölen kahraman o kampa kan davası açar. Kalıcı anılar 6'dan 20'ye çıkar.
   - **Neden:** Kayıp ağırlık kazanır, sonraki olaylar geçmişe atıf yapar.
   - **Efor:** M
   - **Dosyalar:** events.ts, agents.ts, will.ts, types.ts, diorama.ts
5. **Kıtlık gerçek bir krize dönüşsün**
   - **Ne yapılır:** Kıtlık bir kez, önemli olay olarak başlar ve bitince özet düşer ("açlıktan 55 kişi öldü"). Açlık ölümleri kaydedilir. Komşular yardım gönderir ya da reddeder (ilişki ±). Aç medeniyet yağmaya döner. Görünümde boş ambarlar ve çökmüş figürler olur.
   - **Neden:** Seed 3'teki sessiz ölüm sarmalı güçlü bir hikâyeye dönüşür.
   - **Efor:** S–M
   - **Dosyalar:** economy.ts:227–235, diplomacy.ts, events.ts, diorama.ts

**Rol gerektiren (ayrı):** Hancı rolü seçilirse oyuncu anlatıcının rahatlama bütçesini yönlendirebilir (kıtlık köyüne kervan göndermek gibi).

## 5. Tek cümle
Eren yalnızca bir şey yapacaksa, o şey şu olmalı: kasabalıları simülasyonda adı, hanesi ve yaşı olan kalıcı kişilere çevirip her ölümü adıyla, nedeniyle ve mezarıyla kaydetmek; anlatıcı, yas ve zincirler ancak ortada kaybedilecek biri olduğunda anlam kazanır.
