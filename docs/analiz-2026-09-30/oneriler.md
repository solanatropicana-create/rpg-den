# Fantastik Dünya — 5 ajan analizi ve 10 öneri (v0.23, 30 Eylül 2026)

Ajanlar: D&D tutkunu · Civilization tutkunu · RimWorld tutkunu · usta oyuncu (izleme deneyimi) · usta oyuncu (sistem/veri).
Veri: 12 seed × 30 yıl, 3 seed × 60 yıl headless koşu + gerçek oyundan ~50 ekran görüntüsü. Ayrı raporlar bu klasörde.

## Ortak teşhis (5 ajan da aynı yere vardı)

1. **İlk 12–15 yıl canlı, sonra dünya donuyor.**
   - Araştırma ağacı ~19. yılda bitiyor. 30. yılda medeniyetlerin %98'i ağacı tamamlamış oluyor.
   - 98 medeniyetin 74'ü tam 5 yerleşimde kalıyor (`diplomacy.ts:223`).
   - Başkent düşmüyor (`agents.ts:673`).
   - Kamp sayısı 6,8'den 3,5'e iniyor.
   - Altın medyanı 78'den 6.503'e çıkıyor, iş gücünün %43'ü boşta kalıyor.
   - Yıllık büyük olay sayısı 59'dan 28'e düşüyor.
2. **İzleyici kimseyi hatırlayamıyor.**
   - Kahraman adı havuzu ırk başına 5–12 ad (yarı-elfte 5). Kahramanların yarısı adını başka bir kahramanla paylaşıyor.
   - Kasabalılar simülasyonda kişi değil; ırk başına bir sayı olarak tutuluyor (`types.ts:6`). Figürlerin kimliği hash ile üretiliyor ve kendini koruyamıyor.
3. **Olaylar görünmüyor.**
   - İlk 5 dakika boş geçiyor.
   - Gece ekran süresinin ~%45'ini kaplıyor.
   - 20× hızda olayların %82–91'i akışta hiç çıkmıyor.
   - Ekrandaki "nat 20" yazılarının %86'sı adsız askerlere ait.

## 10 öneri

1. **Temizlik paketi** · S
   - Olay listesi 2500 kayıtta kesiliyor (`sim.ts:53`). Önemli olaylar ayrı, kırpılmayan bir kronikte tutulmalı.
   - Zafer satırında ölen kahramanlar da sayılıyor, çünkü `who` savaştan önce hesaplanıyor (`agents.ts:558`).
   - Çalınan teknoloji o an araştırılan düğümse iki kez sayılıyor (98 medeniyetin 11'inde).
   - Doğuş seviyesi yılla şişiyor (`heroes.ts:31`, `inns.ts:33`). 24. yıldan sonra herkes Sv5 doğduğu için efsane mekaniği ölü.
   - Hiçbir yerde okunmayan etkiler var: `teleport`, `harvest`, `prodWood`.
   - Darphane her yerleşimde kuruluyor ama gelir medeniyet başına tek.
   - Harabe hedefi 12 dünyanın 8'inde hiç oluşmuyor (`will.ts:239`).
2. **Kahramana kalıcı kimlik ve Destan Defteri** · S–M
   - Her ırk için en az 20 ad, ayrıca soyad ya da köken. Yaşayan ya da son 10 yılda ölen biriyle aynı ad çekilmez.
   - 6 satırlık günlük sınırı kalkar. Kilometre taşları ayrı tutulur: ilk kan, nat 20, sözleşme, ölüm yeri ve katili.
   - Kahraman ölünce Kronik'e ve mezar taşına bir paragraflık destan yazılır.
   - Uzaktaki etikette ad ve sınıf ikonu görünür.
   - Doğuş seviyesi 1–3, tavan 10. Efsanelik, kahramanın yaptığı işlere göre verilir.
3. **Anlatıcı-yönetmen, önsöz ve kısa gece** · gece ve önsöz S, gerisi M
   - Sinema modu olay puanıyla çalışır. Kahramanın nat 20'si, boss öldürme, savaş, çağ ya da ölüm olduğunda:
     - hız 1×'e iner, kamera olayın yerine gider;
     - altta 2 satır altyazı çıkar (ne oldu ve neden);
     - çekim 8–10 sn sürer, sonra eski hıza dönülür.
   - 20 sn hiç olay olmazsa hız kendiliğinden artar.
   - Olay akışı öncelik kuyruğuna döner (manşet / normal / yalnız kronik). Olaylara ses işareti gelir: davul, boru, çan.
   - Adsız askerlerin zar yazıları kalkar. Kahramanın nat 20'si ağır çekimde, üstünde isimli d20 ile gösterilir.
   - Açılışta simülasyon sessizce ~300. güne sarılır. Başlık kartında dünyanın adı ve 8 arma görünür; karta tıklamak sesi açar.
   - Gece ekran süresinin ~%20'sine iner, gece ortam ışığının tabanı yükselir (`diorama.ts:2849`).
4. **Kronik'i tarih kitabına çevir** · S–M
   - Olayda `parent` alanı olur. Zincir şöyle okunur: baskın → tarla yandı → kıtlık → göç → terk.
   - Kronik'te bir olaya tıklayınca zincirin tamamı açılır.
   - Kıyas'a sekmeler gelir: Nüfus, Toprak, Bilgi, Altın, Güç.
   - Eğrilerin üstünde ⚔ savaş, ⚑ fetih, ★ harika ve çağ işaretleri, ayrıca lider değişim bandı görünür.
5. **Yükseliş ve çöküş** · M
   - Yerleşim tavanı 5 yerine `2 + çağ (+ nüfus/200)` olur.
   - Başkent, medeniyetin başka yerleşimi varsa düşebilir. Son yerleşim de düşerse medeniyet yok olur.
   - Uzak ve yaralı bir yerleşim ayrılıp yeni bir medeniyet kurabilir.
   - Kutsal Sefer bütün iyi hizalı medeniyetleri çağırır. Savunma paktı kurulabilir; paktı bozan "İhanet" damgası yer.
   - Headless'taki `yokOlmaz` ölçütü, "saldırgan dünyada en az 1 çöküş" ile değiştirilir.
6. **Sınıflar farklı bitsin** · M
   - III. ve IV. çağda birbirini dışlayan düğüm grupları gelir (ör. Taş Surlar ↔ Kuşatma, Yüksek Büyü ↔ Taç ve Kanun).
   - IV. çağın maliyeti ×2 olur. Ağaç bitince araştırma, tekrarlanabilir "Kadim Bilgi" düğümlerine akar.
   - Her sınıfa bir yemin ve bedel gelir:
     - tarla yakan Paladin Yeminbozan olur;
     - Paktçı'nın patronu her yıl daha büyük bedel ister;
     - kadim ormanı kesen Druid'in çemberi bölünür;
     - Yabani Büyü gerçek bir d20 tablosuna bağlanır (bugün 26 dalganın 26'sı iyi sonuçlanıyor).
7. **Anlatıcı ve geç tehdit** · M
   - Bir gerilim bütçesi tutulur: son 2 yılın ölüleri, yangınları ve kıtlık günleri. Uzun sessizlikte kriz gelir, zirveden sonra rahatlama.
   - Hedef kamp sayısı `3 + yıl/6` olur. Bugün yeni kamp ancak 2'den az kamp kaldığında doğuyor (`monsters.ts:90`).
   - 15. yıldan sonra trol çeteleri çıkar.
   - 18–22. yıllar arasında bir ejderha uyanır: yakar, hazine biriktirir, hanlara ilan düşer. Medeniyetler ya ittifak kurar ya haraç öder. Ejderhayı öldüren parti efsane olur.
8. **Altın ve ambar anlam kazansın** · M
   - Boştaki işçinin altını 0,02'den 0,004'e iner (`economy.ts:190`).
   - Bakım gelir: kiralık kahraman, L2–L3 yapılar, askerler. Kasabalar bira, ekmek ve alet tüketir.
   - Tekil dünya harikaları yarışı: ilk bitiren alır, kaybeden kaynağının %50'sini geri alır ve kazanana −15 kıskançlık duyar.
   - Kıtlık tek bir büyük olay olarak yaşanır:
     - açlık ölümleri kaydedilir;
     - komşular yardım eder ya da reddeder (ilişki ±);
     - aç medeniyet yağmaya döner.
9. **Kasabalılar gerçek kişi olsun** · ad, yaş ve hane M; akrabalık ve huy L
   - Simülasyonda `Person[]` tutulur: ad, hane, doğum yılı, iş, eş/ebeveyn, 2 huy. Nüfus bu listeden sayılır.
   - Görünümdeki figür kişinin id'sine bağlanır.
   - Her ölüm adı ve nedeniyle mezara yazılır.
   - 5'ten fazla ölümde kasaba bir mevsim yas tutar: kara bayrak asılır, bayram iptal edilir.
   - Yoldaşı ölen kahraman o kampa kan davası açar.
10. **Zindanlar ve adlı eşyalar** · L
    - Harabeler ve dağ içleri 3–5 odalı zindanlara dönüşür. Her odada bir zar: tuzak (DEX), bekçi (savaş), mühür (INT).
    - Zindanın sonunda boss ve adlı bir eşya bekler (ör. "Kızılsancak Baltası").
    - Eşya kahramanla birlikte gezer, kahraman ölünce mezara gömülür, mezar soyulabilir.
    - Veba mezarlıkları ölüler zindanı doğurur; Rahip'in Ölüleri Kovma'sı böylece anlam kazanır.

## Önerilen sıra

- Önce **1 → 2 → 3'ün S kısmı (gece, önsöz) → 5**. Hepsi S–M ve dondurmayı en hızlı bu adımlar çözer.
- Sonra **7 → 4 → 6 → 8**.
- En son büyük yatırımlar: **9 ve 10**.

## Ajanların "yalnız bir şey yap" cevabı

| Ajan | Tek öneri |
|---|---|
| D&D | Kahramana tekil ad, silinmeyen destan defteri, yaşadıklarına göre büyüme |
| Civ | Bitiş çizgisi: tekil harikalar ve lidere karşı birleşilen yükseliş yarışı |
| RimWorld | Kasabalıları kalıcı kişilere çevirmek; her ölüm adla, nedenle, mezarla |
| Usta oyuncu (izleme) | Sinema modunu hızı ve kamerayı yöneten anlatıcı-yönetmene çevirmek |
| Usta oyuncu (sistem) | Başkentin de düşebildiği çöküş ve bölünme mekaniği |

Hiçbir ajan önerisi için oyuncu rolü gerekmedi. Hancı rolü sonraya kalabilir.
