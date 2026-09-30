# Fantastik Dünya: İzleyici Raporu

Not: Container'da FPS 0–1 olduğu için akıcılığı yargılamadım. Kareler `evidence/oyuncu-izleme/` altında: a* = gün 0, b* = 5. ve 30. dk, c* = yakın plan. Olay sayımı `olay-yogunlugu.txt` dosyasında.

## 1. İlk izlenim
"Güzel bir oyuncak diorama açtım ama içinde henüz kimse yok. Ormanlar, zirveler, göller var; bir de bir bayrakla iki çadır. Neyi izlediğimi kimse söylemiyor."

- **60 sn:** Manzaraya "vay" diyorum, sonra bakacak bir şey arıyorum. Kasabalar 6 kişilik, kamera boş çayıra ve kaya yüzüne süzülüyor (a01–a06). Akışa 3–4 satır düşüyor. Ses yok. 79. saniyede ekran kararıyor (a07).
- **5 dk (Yıl 3):** Akışta çoğunlukla "kalay damarı keşfetti" var. 20×'e basınca gece, kar ve tek satır kalıyor (b02–b04). Bırakma noktası burası.
- **30 dk (Yıl 16):** Surlu kasabalar, yollar, kervanlar, bayramlar var (b07–b09). Yakın plan büyüleyici: 42 yaşındaki toplayıcı Dorian Kayalı'yı günlük planıyla izleyebiliyorum (c02). Ama "kim kazanıyor, ne uğruna?" sorusunun cevabı ekranda yok.

## 2. İyi yaptıkları
1. **Görsel kimlik güçlü.** Mevsim renkleri, arazi rölyefi ve surlu kasaba yakın planı "yaşayan bir dünya" hissi veriyor (c01, c02, 12).
2. **Anlatı malzemesi zengin.** Her olayın bir nedeni var; kahraman günlükleri ve kasabalı kartları duygusal bağın tohumu.
3. **Sinema modu varsayılan açık ve olaylara öncelik veriyor** (main.ts:81, 557–583).
4. **Genel bakış okunaklı.** Kıyas raporu (04) ve Tepe görünümü (b10) dünyayı tek bakışta anlatıyor.

## 3. Eksik / zayıf
1. **İlk 5 dakika boş.** 1× hızda ilk çarpışma 6 seed'in 5'inde 5–7,5. dakikada geliyor. İlk çağ atlaması 5,3–6. dk'da, ilk yerleşim 7,8–9,5. dk'da. İlk savaş 18–25. dk'da; 3 seed'de hiç çıkmıyor. 11 açılış karesinin hiçbirinde kadrajın öznesi bir insan ya da olay değil (a01–a11).
2. **Gece çok uzun ve çok karanlık.** Döngü simülasyondan bağımsız, 150 sn'lik gerçek zaman (diorama.ts:2849). Bunun ~%45'i tam karanlık, yani ilk 5 dk'nın ~2 dk 15 sn'si gece geçiyor (a07, a08, b02, b07, 14).
3. **Olaylar vurgusuz ve hızda kayboluyor.**
   - Tüm olaylar aynı küçük kartta çıkıyor (template.html:62).
   - Akış 1,3 sn'de bir kart gösteriyor, kuyruk 12 olayla sınırlı (main.ts:512–545). 20×'te olayların ~%82–91'i, 60×'te ~%95'i hiç görünmüyor.
   - Teknik mesaj hikâyeye karışıyor (main.ts:1220; b01). Bir metinde yinelenen kelime var: "yatağı yatağını" (economy.ts:328).
   - Ses varsayılan kapalı (audio.ts:29, main.ts:1197) ve olaylara bağlı ses işareti yok.
4. **Sinema kamerası nedenini söylemiyor.** Olay 25 sn eski olabiliyor ve altyazı çıkmıyor (main.ts:561–567). Olay yoksa kamera rastgele bir kasabaya ya da kervana gidiyor (571–582). Sonuç: kaya ön planı (a05), boş orman (b01), "Kervan: 5 kömür" (c01). Çarpışma yalnızca 3,2 sn görünüyor.
5. **Kahramanlara bağlanamıyorum.**
   - Ad havuzu çok dar (data/heroes.ts:24–33, sim/heroes.ts:47). Seed 1'de 239 kahramana 157 ad düşüyor; Tarek ve Ysolde sekizer kez geçiyor.
   - Headless kaydında önce "Ysolde öldü… Gruk öldü", hemen ardından "…Ysolde, Gruk yerle bir etti!" yazıyor (satır 51–54).
   - Uzaktan kahraman etiketinde yalnızca seviye rakamı var (diorama.ts:2798; 13).
6. **Hikâyenin yayı ve lider görünmüyor.**
   - Arma şeridi sabit sıralı ve yalnızca çağı gösteriyor (main.ts:988–1001); 31. yılda hepsi "IV" (12).
   - Seed 1'in sonunda 8 medeniyetin hepsi 4. çağda, 5 yerleşimli ve ağacını bitirmiş (headless:3–10).
   - Yıllık büyük olay Y10'da 65, Y24'te 6. Yıl afişi 3,6 sn görünüyor (main.ts:518–529).

## 4. Öneriler (hiçbiri oyuncu rolü gerektirmez)
1. **Anlatıcı-yönetmen**
   - Ne yapılır: cineTick'e olay puanı ve tempo kontrolü eklenir. Puanı ≥4 olan olayda (çarpışma, savaş, harika, çağ, ölüm) hız geçici 1×'e iner, kamera oraya gider ve alt ortada 2 satırlık altyazı çıkar (ne oldu + `cause`). Çekim 8–10 sn sürer, sonra eski hıza dönülür. 20 sn olay olmazsa hız kendiliğinden 5–20×'e çıkar.
   - Neden: Boş başlangıcı sarar, hızda kaybolan olayları yakalar, kameraya anlam verir.
   - Efor: M
   - Dosyalar: main.ts, template.html, diorama.ts
2. **Olay hiyerarşisi ve ses**
   - Ne yapılır: feedQ öncelik kuyruğuna çevrilir: manşet (büyük kart, ikon, 12 sn), normal olay ve yalnızca Kronik. Hızda aynı türden olaylar birleştirilir. Kalite mesajı FPS köşesine taşınır. audio.ts'ye `cue(kind)` eklenir (davul, boru, çan). Yıl afişi 3 manşetli bir özete döner. Armalar nüfusa göre sıralanır ve altlarına nüfus çubuğu eklenir.
   - Neden: Gözün nereye bakacağını ve kimin önde olduğunu söyler.
   - Efor: S–M
   - Dosyalar: main.ts:509–546, 988–1001; template.html:61–88; audio.ts
3. **Kısa ve aydınlık gece**
   - Ne yapılır: Döngü eşlemesi doğrusal olmaktan çıkarılır; gece ~%20'ye iner. Gece ortam ışığı ~%35 tabanda tutulur. Döngü öğleden başlar.
   - Neden: Ekran süresinin ~%45'i şu an okunmuyor. En ucuz ve büyük kazanç bu.
   - Efor: S
   - Dosyalar: diorama.ts:2848–2870, atmosphere.ts
4. **Önsöz açılışı**
   - Ne yapılır: Simülasyon açılışta sessizce ~gün 300'e sarılır (1–2 sn). Bu sırada bir başlık kartı görünür: dünyanın adı ve 8 arma, her biri için tek satır kimlik. Karta tıklamak sesi de açar. Ardından Tepe görünümünde bir kuruluş çekimi gelir ve kamera ilk manşete dalar.
   - Neden: "Neyi izliyorum?" sorusunu cevaplar ve boş 5 dakikayı atlar.
   - Efor: S–M
   - Dosyalar: main.ts:54–74, 1270–1276; diorama.ts:525; template.html
5. **Başrol kahraman**
   - Ne yapılır: Her dünyada adlar benzersiz olur ve her kahramana lakap ya da soyadı verilir. Havuz ırk başına en az 20 ada çıkar. Uzak etikette sınıf ikonu ve ad görünür. Sinema bir başrol seçer ve 2–3 dk'da bir ona döner; başrol ölünce bir anma kartı çıkar.
   - Neden: İzleyiciye tutunacağı bir yüz verir; derin kahraman sistemi görünür hâle gelir.
   - Efor: M
   - Dosyalar: data/heroes.ts, sim/heroes.ts:47, diorama.ts:2798, main.ts:571–577

*Oyuncu rolü gerektiren fikir (sonraya):* başrol kahramana sponsor olmak ya da hancı olarak ilan asmak.

## 5.
**Eren yalnızca bir şey yapacaksa, o şey şu olmalı:** Sinema modunu, önemli olayda hızı düşürüp kamerayı altyazıyla oraya götüren ve sessiz dönemleri kendisi saran bir "anlatıcı-yönetmen"e çevirmek.
