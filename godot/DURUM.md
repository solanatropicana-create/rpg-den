# Fantastik Dünya — açık dünya dikey dilimi (durum, 2 Ekim 2026, Faz 2)

## Faz 2 · Oyuncu, ekip ve d20 savaş (sürüyor)

Brief: `claude/faz2-brief.md` (proje). Sıra: A simülasyon bağlantısı → B karakter yaratma → C duraklatmalı d20 savaş →
D bayılma ve yara → E ekip → F envanter ve ekonomi → G etkileşim ve pano → H Demir mod kaydı → kabul testleri.

### A · Simülasyon bağlantısı (bitti: A1)

- **FD.Macro Godot'ya bağlı** (`FantastikDunya.csproj` → `../macro/FD.Macro`). İkisi de hata ayıklama yapılandırmasında bile
  eniyilenmiş derlenir (`<Optimize>`): tarih öncesi (1600 gün) ~18–25 sn (eniyilemesiz ~38 sn).
- **Oyun başı** (`src/game/Session.cs`, `macro/FD.Macro/Modules/Local.cs`): `Session.NewWorld(seed)` → `new Sim(seed)` (tarih
  öncesi, ilerleme geri çağrısıyla) → `Local.Bind`: bir krallığın orta halkasından (Köy kademesi, taht şehri değil) köy seçilir;
  puan: krallık, hana ve goblin kampına yakınlık, istikrar, 22–45 kişilik nüfus (1:1 köyün on evi var). En yakın han ve en yakın
  kara goblin kampı bağlanır; 14 fersahta goblin kampı yoksa köyün 4–9 fersah çevresine kurulur (dünya olayı). Gizli kamp ortaya
  çıkar. Bağ `World.Region`'da (`RegionLink`: köy, han, kamp, oyuncu, ekip; kayıtta, sürüm 7).
- **Simden 1:1'e:** köyün, hanın ve kampın adı (`LifeSim.VillageName/InnName/CampName`); köylüler `Census.Populate(…,
  RegionBind.Spec)`: sim nüfusunun %90'ı (12–42 kişi) on eve dağılır, ırklar simdeki oranla (en büyük kalan yöntemi, hane
  çoğunlukla tek ırk), işler simin iş dağılımından (tahıl/balık/toplayıcı → çiftçi, odun/taş/maden → oduncu, et/deri → çoban,
  atölyeler → demirci ve çırak), rahip Güneş inancı ≥ %12 ise, hancı hanın sahibinin ırkından, adlar ve yaşlar simin ırk
  tablolarından (`HERO_NAMES`, `HERO_SURNAMES`, `HERO_AGE`: cüce köyünde Örsdövenler ve uzun ömürler). Goblin sayısı kampın
  kalabalığı (2–10; şef varsa ayrıca büyük, miğferli "Goblin şefi").
- **Irkların görünüşü** (insan modeli; `Appearance.Look`): boy, en (cüce ve yarı-ork tıknaz, elf ince), ten (yarı-ork gri-yeşil,
  ejderdoğan pullu renkler ve saçsız, tiefling kızıl-mor), kulak (`Humanoid.Ears`: elf uzun sivri, yarı-elf, buçukluk, gnom,
  yarı-ork, tiefling kısa sivri; kafa kemiğine bağlı ten renkli koniler), sakal (cüce erkeklerin hepsi, kadınların %20'si; elf
  hiç). Gerçek ırk modelleri sonra.
- **Çocuk ve köpek yok** (karar): `Census` çocuk üretmez, köpek kulübesi kalktı.
- **Saat:** `GameClock.TimeScale` 20 → **48** (1 gün = 30 gerçek dakika = makronun bir günü). Güneş yolu enlem 50°, eğim 23°:
  doğuş ~04:47, batış ~20:50, gece (güneş −6°'nin altında) 21:41–03:56 ≈ 6,25 saat ≈ **8 gerçek dakika**.
- **Makro gün adımı** (`src/game/Director.cs`): yerel gece yarısı makro `Sim.Step()` bir kez (~35–45 ms; atlanan gece
  yetişir), ardından `Local.DayTick` (kampa ilan yoksa köy asar). `Session.MacroDayFor(yerel gün)`.
- **Yaşam simi 30 dakikalık güne uyarlandı** (`Brain`): insanlar gerçek hızda yürür, saat 2,4 kat hızlı akar; yürüme süresi
  `LifeSim.TimeScale` ile oyun saatine çevrilir (`Brain.TravelH`): eve yatma saatine yetişecek kadar erken çıkılır, akşam
  etkinliği eve dönüşe yer bırakır (yoksa evde oturulur), öğle yemeği evde ancak yol 12 oyun dakikasından kısaysa, handa içmeye
  ancak gidip dönmeye vakit varsa gidilir (köyden hana ~5 oyun saati: köylüler artık akşam hana pek gitmez); işe çıkış kişiye
  göre ±25 dk yayılır (herkes 07:00'de yola dökülmesin). Goblin rolleri 6'lık döngüde (en çok 10 goblin).
- **Testler:** `--worldtest=N` (bağ, adlar, nüfus/ırk/iş/goblin simden, ilan, N gün = N makro adım), `--lifetest=N`'e
  çocuk = 0 ve köylü sayısı = simden beklenen eklendi; kalabalık ölçütü goblin ateşini saymaz. Tohum 1–6'da lifetest PASS
  (gündüz dışarıda ort %35–39, gece %0, öbek ≤ 7), worldtest tohum 1–3 PASS, selftest PASS.
- Makroda bağdan bağımsız iki değişiklik: kamp adları yalnız yaşayan ve yakın zamanda temizlenmiş kamplarda "kullanılıyor"
  sayılır (uzun tarihte "Kırıkdiş Kampı 269" gibi adlar çıkıyordu; eski boyun adı yıllar sonra geri dönebilir); oyuncu ve
  ekibindekiler han havuzuna (`Inns.InnPool`) sayılmaz.
- **Açık:** bağlı kampın ilanını simdeki bir kahraman alıp kampa yürüyebilir (makro kampı uzaktan çözer); bunun bölgede yerel
  savaşla çözülmesi G'de. Karar: G'ye dek makro yolu (`Agents.FightCamp`) sürer.

### B · Karakter yaratma ve oyuncu kişisi (bitti)

- **Açılış** (`scenes/Boot.tscn`, `src/ui/Boot.cs`; ana sahne artık bu): başlık menüsü (Devam et — kayıt varsa, adı, sınıfı,
  seviyesi ve günüyle; Yeni oyun (Demir mod); Çık) → karakter ekranı → "Dünya doğuyor" (makro dünya ve tarih öncesi işçi
  iş parçacığında, yıl/gün ilerleme çubuğu) → bölge. Test ve çekim argümanları menüyü atlar (`Dev.SkipMenu`); `--play` menüsüz
  oyun, `--ui=create --shot=…` ekranın çekimi, `--newgame=ırk,sınıf[,tohum[,ad]]` akışın başsız koşusu (yaratma → dünya → kayıt →
  bölge).
- **Karakter ekranı:** 9 ırk (simin ırkları; kısa açıklama ve bonuslar), 4 sınıf (savaşçı, haydut, büyücü, rahip; can zarı ve
  özellik), 27 puanlık dağıtım (8–15, 14 ve 15 ikişer puan; "sınıfa göre dağıt" sınıfın önceliğiyle 15/14/13/12/10/8), ırk bonusu
  (simin `RACE_STAT`'ı: insan her yetenekte +1, cüce Güç ve Dayanıklılık +2…), toplam ve değiştirici, hizalama (iyi, tarafsız,
  kötü), ad (ırkın ad havuzundan rastgele), görünüş (kadın/erkek, ten — ırkın paleti, saç rengi ve biçimi, sakal, giysi rengi,
  boy ±%6) ve dönen 3B önizleme (`CharacterPreview`: kendi dünyası, ışığı, kamerası; fareyle döner). Büyücü kitabındaki 1. seviye
  büyüyü seçer (Sihirli Füze, Yanan Eller, Uyku). "Başlangıç" kutusu can, zırh sınıfı, saldırı, taşıma, inanç ve büyüleri yazar.
- **Kurallar** (`src/rpg/`, Godot'dan bağımsız): `Rules` (D&D 5e: değiştirici, puan dağıtımı, can = tam zar + DAY, sonra ortalama;
  yeterlilik; seviye simin XP tablosundan; yük kapasitesi Güç × 7 kg, küçük ırklarda ×0,75; cüce/buçukluk/gnom %15 yavaş; büyü
  zorluk derecesi ve saldırısı; 1. seviye yuvalar 2/3/4; para: 1 altın = 10 gümüş), `Items` (PHB silahları: sopa, hançer, asa,
  mızrak, el baltası, pala, kısa kılıç, topuz, uzun kılıç, kısa yay; deri zırh, zincir gömlek, zincir zırh, kalkan; şifa iksiri
  2d4+2, sargı bezi, erzak, şifalı ot, goblin ıvır zıvırı — fiyatlar gümüş, simdeki bir mala bağlı), `Spells` (Ateş Oku, Buz Işını,
  Sihirli Füze, Yanan Eller, Uyku; Kutsal Alev, Yara Sarma), `Character` (statlar, can, envanter + kese, takılı silah/zırh/kalkan,
  zırh sınıfı, saldırı — silahsız 1 + Güç, büyüler ve yuvalar, kalıcı yaralar ve lakap, maaş, baygınlık ve ölüm zarları),
  `CharacterFactory` (oyuncu; simdeki kahramandan yerel kişi — sınıfın varsaydığı takımla).
- **Başlangıç:** üstündeki giysi ve 5–15 gümüş, silah yok (savaşçı yumrukla +5 isabet, 4 hasar). Büyücü: Ateş Oku, Buz Işını ve
  seçtiği büyü, 2 yuva. Rahip: Güneş Kilisesi'ne bağlı, Kutsal Alev ve Yara Sarma, 2 yuva.
- **Oyuncu simde bir kahraman** (`Local.CreatePlayer`, `Session.AddPlayer`): kahraman kaydı, State "player" (simin yapay zekâsı
  ona dokunmaz: hedef seçmez, handa beklemez, yaşlanıp ölmez), bağımsız, yuvasız; inanç, hizalama, kese (gümüş / 10 altın),
  itibar ve üyelik kayıtta; köye "kimsenin tanımadığı bir yabancı geldi" diye yazılır. `Session.SyncPlayerToMacro` can, seviye,
  XP, kese ve lakabı geri yazar.
- **Görünüş:** `LookKit` karakteri insan modelinde giydirir (ırkın boyu, eni, kulakları; seçilen renkler; zincir zırhta miğfer;
  elde silah — glb aletleri: sopa, mızrak, balta, çekiç; yordamsal: hançer, kısa kılıç, uzun kılıç, pala, asa, yay sağ el
  kemiğinde). Oyuncu bedeni `Player.SetCharacter` ile.
- **Kayıt (ilk sürüm, H'de genişleyecek):** `SaveGame` tek yuva (`user://save/`): makro dünya `Sim.Save` (gzip) + yerel durum
  JSON (ekip, saat, oyuncunun yeri, bölge bayrakları), geçici dosyaya yazılıp yer değiştirilir. Yeni dünya yaratılınca kaydedilir;
  menüdeki "Devam et" açar.

### C · Duraklatmalı gerçek zamanlı d20 savaş (bitti)

- **Ortak kural** (`macro/FD.Macro/Modules/Combat.cs` → `Combat.Strike`): simdeki savaşların saldırı satırı ayrı bir işleve
  alındı (d20 + saldırı ↔ zırh sınıfı, doğal 20 kritik — 3. seviye savaşçıda 19 — zarları ikiler, doğal 1 ıska, avantaj/
  dezavantaj, sinsi saldırı, öfke, paladin vuruşu, korucu işareti). `ResolveBattle` ve bölgedeki savaş aynı işlevi çağırır
  (simetri). Makro çıktısı değişmedi: `hash 1 2400` aynı (59308e383f41fe8f).
- **Savaş çekirdeği** (`src/rpg/Fight.cs`, Godot'suz, tohumla belirlenimli): her savaşan ~3 gerçek saniyede bir davranır
  (Çeviklikle 2,5–3,4 sn); yürüme ve menzil metre cinsinden. Sınıflar kendi kararıyla dövüşür (Kenshi gibi), emir verilmedikçe:
  savaşçı yere düşen ya da yaralı yoldaşın başındaki düşmana koşar, yarı canda derin nefes alır; haydut bir yoldaşın dövüştüğü
  düşmanı seçer ya da arkasına dolanır (sinsi saldırı); büyücü mesafe korur (3,5 m'den yakına gelen düşmandan geri çekilir), 3+
  düşman kümesine Uyku, yakın kümeye Yanan Eller, şefe ve yaralılara Sihirli Füze, gerisi Ateş Oku/Buz Işını; rahip yerdekini
  kaldırır, ağır yaralıyı iyileştirir, yoksa Kutsal Alev. Goblinler yakındakilerin en zayıfına üşüşür, okçular mesafe korur.
  Yere düşen ekip üyesi 3 saniyede bir ölüm zarı atar (10+ başarı, 1 iki kayıp, 20 kalkar; yerdeyken yediği darbe bir kayıp,
  kritik iki; büyük hasar doğrudan öldürür); uyuyan ya da yerdeki hedefe yakın dövüşte avantaj ve isabet kritik. Moral: goblinlerin
  yarısı düşünce ya da şefleri düşünce bozgun — gerçekten kaçarlar; ekibin %75'i düşünce yaralı yoldaşlar kaçar (oyuncu kendiliğinden
  kaçmaz). XP: goblin 50, şef 200, sağ kalanlara bölünür; seviye atlanınca can artar, büyücü yeni büyü öğrenir.
- **Bölgede savaş** (`src/combat/CombatDirector.cs`): kovalayan goblin oyuncuya yetişip vurunca (`LifeSim.GoblinStrike`) savaş
  başlar. Ekip (oyuncu + yoldaşlar) ve 30 m içindeki ya da kovalayan goblinler katılır; savaş kampın 65 m yakınındaysa kamptaki
  bütün goblinler, çadırda uyuyanlar 3,5–10,5 sn sonra uyanıp ("gürültüye uyandı ve çadırından fırladı!"). Sonradan yetişen goblin
  de savaşa katılır. Okçu goblinler (her dört goblinden biri) yay taşır. Savaş bedenleri sürer (oyuncu betik kipinde, `Person.InFight`);
  kaçan goblinler ormana koşup birkaç oyun saati saklanır (kamp düştüyse bir daha dönmez), ölüler yerde kalır.
- **Duraklatma:** Boşluk bütün sahne ağacını durdurur (canlandırma donar, saat durur); emirler duraklıyken de verilir. Savaş
  başında bir kez kendiliğinden duraklar (ayar: `Settings.AutoPause`, `user://settings.json`; H'de ayarlar ekranı).
- **Taktik kamera** (`TacticalCamera`): savaş başlayınca oyuncunun kamerasından yukarı süzülür (~58°, 19 m), savaşın ortasını
  izler; WASD kaydırır (5 sn sonra yeniden izler), teker 8–42 m, sağ sürükle döndürür. Fare serbest. Bitince üçüncü şahsa dönülür.
- **Emirler:** sol tık yoldaşı seçer (Shift ekler), sürükle kutuyla seçer, düşmana tık saldırı, yere tık yürü (sağ tık da emir);
  Tab sıradaki; 1–5 büyüler (hedef gerekiyorsa nişan kipi: tıkla, sağ tık/Esc vazgeç; fare zaten uygun hedefteyse hemen), Q iksir
  (fare bir yoldaştaysa ona), B sargı (yerdekini 1 canla kaldırır), F derin nefes, R geri çekil, H bekle, G serbest (kendi kararı).
- **Görünenler** (`CombatHud`): ekibin altında halka (seçili parlak), fareyle gösterilen düşmanda kırmızı halka, baş üstünde can
  çubuğu, yerdekinin üstünde ölüm zarları (✔○○ ✖○○), uyuyanda Zzz; her atış hedefin başının üstünde yükselir
  (`17+5 → 22 vs ZS 15 · isabet · 6`; doğal 20 altın renkli ve büyük, ekran parlar; ıska gri; ekibe isabet kırmızımsı). Panolar:
  ekip (can, büyü yuvaları, durum ya da emir; tıklayınca seçer), seçilinin eylemleri (düğmeler, tuşlarıyla), zar günlüğü
  (duraklıyken her atış açılır: `d20 17 + 5 vs ZS 15; hasar zarları 6+3`), üstte savaş saati ya da DURAKLATILDI, sonunda özet
  (zafer/yenilgi, öldürülen, kaçan, TP, seviye, ilan).
- **Simle bağ** (`Local.LocalCampFight`): ölen goblinler kampın sayısından düşer, şef ölürse kamp şefsiz kalır. Kamp **kırıldıysa**
  (şefi düştü ve en çok bir goblini ayakta kaldı, ya da hiç kalmadı) simde de temizlenir: ilan kapanır, ödül handa bekler
  (`RegionLink.Reward`), ganimet kampın sandığında (`CampLoot`, F), köyün devletinin tehdidi azalır, oyuncunun günlüğüne ve
  tarihe yazılır ("…, Kırıkdiş Kampı'nı yerle bir etti!"), **10–20 gün sonra** boşalan vadiye öncüler gelmeye başlar: simin
  "verimli vadi" fırsat merkezi şansa bakmadan başlar (söylenti → hücum: çevreden çiftçiler iner; dünyada yer varsa kalıcı köy olur;
  vadiye yer yoksa öncüler köye yerleşir). İlanı simdeki bir kahraman almışsa ödül onun değildir; ilanı boşa çıkar.
- **Yoldaş bedeni** (`src/actors/Companion.cs`): oyuncunun ardında gevşek bir dizide yürür, geride kalınca koşar, çok uzakta kalırsa
  yanına gelir; savaşta betik kipinde. `--party=fighter,wizard` geliştirici yoldaşları (Sv2, sınıf takımıyla). İşe alma E'de.
- **Testler:** `--fighttest` (kural: d20 ki-kare, %5 kritik/ıska, %50 isabet; 5 ekip × 60 savaş: sınıf davranışları; belirlenim),
  `--camptest=N` (başsız uçtan uca: oyuncu kamp patikasından yürür, goblinler görüp kovalar, savaş kendi başına biter; ölüler yerde,
  kaçanlar gitti, kimse savaşta kalmadı, üçüncü şahsa dönüldü, TP verildi, simdeki kamp tam öldürülen kadar azaldı; kamp düştüyse
  simde temizlendi, ilan kapandı, 10–20 gün sonra yerleşimciler geldi). Tohum 1, 2, 3, 5'te ekip (savaşçı + büyücü) kampı düşürdü
  (14 goblin + şefli kamp dahil); tek başına Sv1 oyuncu yenildi (yenilgi akışı D'de).
- **Kararlar:** bozgunda goblinler kampa değil kampın ötesine (ormana) kaçar; kamp, şef düşünce en çok bir goblini kaldıysa kırılmış
  sayılır (kalanlar dağılır).

### D · Bayılma, ölüm zarı, kalıcı yara (bitti)

- **Bayılma:** canı 0'a düşen ekip üyesi yere düşer ve 3 saniyede bir ölüm zarı atar (C). Ayaktaki bir dost sargıyla (B) ya da iksirle
  (Q, fare yerdekinin üstündeyken) kaldırır; rahibin Yara Sarma'sı da. Kazanılan savaştan sonra yerdekiler 1 canla kendine gelir.
- **Kalıcı yara** (`Fight.MaybeWound`, `Character.AddWound`): kötü giden ölüm zarında (1–9) ya da yerdeyken yenen kritikte dörtte bir
  ihtimalle — kör göz (¼: uzak saldırıda −2), topallık (¼: %20 yavaş), derin iz (½: Karizma −1); savaş başına en çok bir. Lakap
  gelir (Tek Göz, Topal, Yaralı Yüz) ve simdeki kahraman kaydına, günlüğüne yazılır (`Local.Wounded`). Tedavi tapınakta, pahalı
  (400 gümüş; rahibin fiili G'de; `Character.CureWound`).
- **Kaybedilen savaş** (ekibin hepsi yerde): yerdekiler goblinler keseleri boşaltırken ölüm zarlarını sürdürür (`Fight.SettleFallen`;
  her kayıp yine yara getirebilir; üçüncü kayıp burada ölüm değil, "ölümün eşiğinden kıl payı döndü"). Sonra goblinlerin kararı
  (`CombatDirector.Fate`): **%70 soyar**: bütün gümüş, iksirler, ıvır zıvır ve otlar, yarı yarıya silah, %40 kalkan → kampın sandığına
  (`Session.CampChest`, kayıtta; kamp düşünce orada bulunur), simde kampın ganimeti büyür, tarihe "…goblinleri X'i yere serip soydu"
  (`Local.Robbed`); **%22 esir alır** (kamp ayaktaysa): biri (yalnızsa oyuncu) kampın kafesine, ötekiler soyulup atılır
  (`Local.Captured`); **%8 bitirir**: yerdekiler ölür. Ekip 2,5–4 oyun saati sonra 1 canla, kamptan ~70 m uzakta, patikanın başında
  uyanır (savaş kamptan uzaktaysa düştüğü yerde); ekranda ne olduğunu, kimin kafeste olduğunu ve yeni yaraları söyleyen satır.
- **Kafes** (`src/game/Captivity.cs`): kafesteki oyuncu parmaklıkların ardında tutulur; saatte bir kapıyı zorlayabilir (E): d20 + Güç
  ya da Çeviklik (haydut yeterliliğini ekler) ≥ ZD 15. Kafesteki yoldaşın kapısını ayaktaki bir ekip üyesi dışarıdan açar (E). Oyuncu
  kafesteyken yoldaşlar uyandıkları yerde bekler. HUD'a genel E istemi eklendi (`Hud.Prompts`, `Hud.Toast`).
- **Ölüm:** ölen ekip üyesi simde de ölür (`Local.Died` → `Heroes.Die`: olay, destan); bedeni düştüğü yerde kalır, ekipten çıkar.
  Oyuncu ölür ama ekipten biri yaşarsa **başa o geçer** (`Session.PromoteToPlayer`, `Local.Promote`: simde artık o "oyuncu";
  kiralık askere sessizce kayıt açılır); oyuncunun bedeni onun görünüşünü alır, yoldaş bedeni eski liderin cesedi olur. **Herkes
  ölürse** Demir mod: kayıt silinir, başlık ekranına dönülür.
- **Testler:** `--fighttest`'e kalıcı yara ölçütü (200 yalnız büyücü savaşında 65 yara: iz 37, göz 16, topallık 12; lakap, yer ve
  Karizma doğru). `--camptest --fate=rob|capture|kill`: soyulma (kese boş, sandıkta, 72 m ötede, 2,5 saat sonra, tarihte), kafes
  (oyuncu kafeste, olay, kapı zorlanıp çıkıldı), bitirme (kimse kalmadı, kayıt silindi). Tohum 3'te soyulan oyuncu "Yaralı Yüz" oldu.
- **Kararlar:** kaybedilen savaşta ölüm zarlarının üçüncü kaybı öldürmez (brief: kaybeden bayılır, soyulur, uyanır; goblinler nadiren
  bitirir); kafes için zaman sınırı yok (kaçış her saat denenebilir).

### E · Ekip (bitti)

- **Handaki maceracılar** (`RegionBind.Guests`, `Census`, `Brain.InnGuest`): simde bağlı handa bekleyen serbest kahramanlar (en
  çok 3, seviyesi yüksek olan önce) handa birer kişi olur: sabah ortak salonda iş bekler, öğleden sonra avluda kılıç talimi yapar,
  akşam içer, geceyi handaki odasında geçirir; sınıfına göre giyimli ve elinde silahı. Kiralık olanlar (savaşçı ve haydut; paladin,
  barbar ve korucu savaşçı sayılır) ikiden azsa köyün halklarından 1. seviye paralı askerler (savaşçı, haydut) eklenir. Kartta:
  "Maceracı · savaşçı Sv2" (paralıysa "paralı asker"). Simde handan ayrılan kahraman (ilan aldı, bir devlete kiralandı) ertesi gün
  handan da gider.
- **Kiralama** (`PartyManager`): kişinin kartında **[1] Kirala** — haftalığı (5 gün) simdeki kahramanda 25 × seviye + 25 gümüş
  (Sv2: 75), paralı askerde 40 gümüş; ilk ödeme bir hafta sonra (başlangıçtaki 5–15 gümüşle de kiralanabilsin diye: yoldaş ödülü
  bekler). Hizalaması liderinkine zıtsa (iyi–kötü) kartta "yolları ilk konaklamada ayrılır" yazar. En çok 2 yoldaş. Simdeki kahraman
  kaydını korur (State "party", `RegionLink.Party`; `Local.Hire`), paralı askerin kaydı yok. Yoldaşın görünüşü handaki kişininkidir.
- **Maaş günü:** gün başında, ödeme günü gelen yoldaşın haftalığı ekibin keselerinden (önce liderin) ödenir; yetmezse ayrılır
  ("Parasız yol yürünmez"), simdeki kahraman hana döner (`Local.Dismiss`), handaki kişi yeniden görünür. Bir gün önce uyarı.
- **Sadakat:** hizalaması liderinkine zıt yoldaş ekip **konaklayınca** (handa uyuma — H) ayrılır. Karar: brief'teki "kampta"
  konaklama/kamp kurma olarak okundu.
- **Keşifte:** yoldaşlar kontrol edilen kişinin ardında gevşek bir dizide yürür (geride kalınca koşar, 40 m'den uzakta yanına gelir).
  **Tab** kontrolü sıradaki ekip üyesine geçirir: oyuncu bedeni onun görünüşünü ve yerini alır, eski kişi yoldaş bedenine geçer; ekip
  yeni kontrol edileni izler (`Session.Controlled`, kayıtta). Savaşta Tab yine seçimi değiştirir. Kafesteki kişiye geçilirse
  parmaklıkların ardında kalır; kafesteki ana karaktere başka bir üyeyle gidip kapıyı dışarıdan açmak böyle olur.
- **HUD:** sol altta ekip şeridi (kontrol edilen ▶, sınıf, seviye, can, kafes/baygın, maaşa kalan gün); kartta fiiller (1–5).
- Simdeki yoldaşların canı, seviyesi, XP'si ve lakabı simdeki kayda yazılır (`SyncPlayerToMacro`).
- **Test:** `--partytest` (handaki maceracılar simden + paralı, kiralama → simde "party", izleme ≤ 4,5 m, Tab ileri/geri, maaş ödenir,
  ödenmeyince ayrılır ve hana döner — simdeki kahraman hemen yeni bir ilana çıkabildi —, zıt hizalama konaklamada ayrılır). Tohum
  1–6 PASS (3–6'da simden kahraman kiralandı: Laucian Gökçeorman korucu Sv3 → savaşçı, haftalık 100 gümüş).
- Kalan: savaşta kafesteki ya da baygın yoldaş savaşa girmez; handa oturma yerleri içerideyse kişiler görünmez (öğleden sonra avluda).

---

# Açık dünya dikey dilimi (30 Eylül 2026)

**Dilim tamam:** 1:1 ölçekte 1200 × 1200 m'lik "Bölge"de üçüncü şahıs yürünür. Sessiztepe köyü (10 hane, 43 kişi),
yolun 500 m ötesinde Yorgun Katır Hanı, ormanda Kırık Diş goblin kampı, tepede Eski Gözcü Kulesi. Herkesin evi,
işi, ihtiyaçları ve gün planı var; yaptığı her şeyin kartta okunan bir nedeni var. E: kişi kartı, M: harita.

## Çalıştırma (Windows)

1. .NET 8 SDK kurulu olmalı (`dotnet --list-sdks` 8.x göstermeli; yoksa `winget install Microsoft.DotNet.SDK.8`).
2. `RPG Den\OYNA.bat` → derler, modelleri içe aktarır, oyunu açar. `EDITOR.bat` → Godot editörü.
   (Godot 4.7.2 .NET, `RPG Den\_tools\` altında.)

## Kontroller

W A S D yürü (koşar adım 4 m/s) · **Shift hızlı koşu 11 m/s** (keşif için; köyden hana ~50 sn) · Ctrl yavaş yürü · Space zıpla · fare kamera · tekerlek yakınlaştır ·
**E** önündeki kişiyle konuş / kartını aç-kapat · **M** bölge haritası · **F3** geliştirici bilgisi (FPS, köyde
dışarıdaki kişi oranı) · Esc fareyi bırak · F11 tam ekran.

Saat: **1 oyun günü = 72 gerçek dakika** (TimeScale 20). 100 m yürümek ≈ 25 oyun dakikası; köyden hana ~2 oyun saati.

## Yaşam simülasyonu (src/sim/life — Godot'dan bağımsız C#)

- **LifeSim**: yerler (Place: ev, meydan, kuyu, pazar, tapınak, demirhane, tarla, mera, ağıl, kesim yeri,
  yıkama yeri, han, avlu, kamp, pusu yeri, yol uçları), yerlerdeki noktalar (Spot: örs, ocak, oturak, konuşma halkası,
  kuyu başı, tezgâh, dua yeri, ağaç başı, yalak, ahır bölmesi, çadır…), yol ağı (PathGraph: yol, patika, köy
  sokakları, kapı yolları, evin arkasına dolanan izler, tarla yolları; A*), haneler, kişiler, koyun sürüsü.
- Zaman: kararlar oyun saatiyle, yürüme gerçek zamanla (kişiler gerçek hızda yürür). Büyük saat sıçramasında
  herkes o anki etkinliğinin yerinde yeniden "maddeleşir".
- **Brain**: rol başına gün (çiftçi, ev hanımı, oduncu, demirci + çırak, çoban + yamak, muhtar, rahip, hancı,
  hizmetkâr, seyis, çocuk, yaşlı; gezgin tüccar (öküz arabasıyla), hacı, maceracı; goblinler). Uyku/kahvaltı,
  sabah-öğleden sonra işi, öğle yemeği (iş yeri uzaksa azık), su taşıma (kova elde), çamaşır (derede), bahçe,
  pazar, tapınakta ders (çocuklar), meydanda sohbet, kapı önünde oturma, han. Her kararın Türkçe etiketi ve nedeni.
- **Census**: 10 hane şablonu (muhtar, demirci, çoban, oduncu, 6 çiftçi), rahip, han ahalisi (4), 3 yolcu, 6 goblin.
- **Goblinler**: devriye, ateş başı, şiş çeviren aşçı, çadırda uyuyan; gece üç kişi patika kenarında pusuda
  (meşaleyle gidip gelir). Oyuncuyu görürse (gündüz 16 m, gece 9 m, depar ×1,6) kovalar ve saldırır (şimdilik
  hasar yok: ekran kızarır ve sarsılır); 48 m'den fazla uzaklaşınca ya da 38 m'den kaçınca bırakır. Yakındaki
  köylüler goblin kovalarken eve kaçar.

## Dünyadaki yerleşim (src/world)

- **VillageSite** (Sessiztepe): yol boyu köy; meydan (kuyu, pazar tezgâhı, banklar, fenerler, gölge ağacı),
  tapınak, demirhane (ocak ateşi + ışık + duman), muhtarın büyük evi, 9 küçük ev (A/B), her evin arkasında lahana
  bahçesi ve çiti, yan duvarda bank, odunluk/çamaşır ipi/köpek kulübesi, meraya bakan ağıl (çit + kapı), tarlalarda
  korkuluk ve saman. Kuzey ve güney sokakları, kapı yolları, evlerin arkasına dolanan izler.
- **OutskirtsSite**: batı ve güney tarla yolları, orman kıyısında odun kesim yeri (kütük yığını, kesme kütüğü;
  gerçek ağaçların başında kesim noktaları), köyden oraya giden patika, derede yıkama yeri, yol uçları, tabelalar.
- **InnSite**: han (sundurma masaları, tezgâh, ahır, yalak), kapı fenerleri, yol kenarında meşaleler, baca dumanı.
- **CampSite**: kazıklı çit halkası (giriş patikaya bakar), üç çadır + sundurma, kamp ateşi (ışık + alev + duman),
  şiş, totem, kafes, kemik yığını, sancaklar; patika kenarında gece pusu yeri.
- **RuinSite**: tepede yıkık gözcü kulesi (içinde yürünür merdiven).
- **PropKit**: Blender modellerini manifestleriyle yerleştirir (hi/lo LOD, MultiMesh toplu eşya, manifest
  çarpışma kutuları, noktalar), **NightLight** (akşam yanan, titreşen ışık), **Fx** (alev ve duman parçacıkları).

## Görünür hayat (src/life, src/ui)

- **LifeWorld**: her kişi için PersonActor (human.glb / goblin.glb, rol ve yaşa göre boy, ten, saç, kıyafet,
  şapka/önlük/başlık), yaptığı işin animasyonu ve aleti (çapa, balta, çekiç, kova, çuval, yaba, mızrak, meşale),
  konuşanlar birbirine döner, yanlarında duran kahramana bakarlar, eve girerken kapıdan içeri yürüyüp kaybolurlar.
  Koyun sürüsü (merada otlar, çobanın peşinden ağıla döner), tüccarın öküz arabası (tekerlekler döner), içinde
  uyanık biri olan evin pencereleri yanar, sabah/öğle/akşam bacası tüter.
- **Hud**: saat + bulunduğun yerin adı, E istemi, kişi kartı (ad, yaş, meslek, hane, şu an ne yaptığı, neden,
  sıradaki plan, tokluk/dinçlik/sosyallik, günlük), M haritası (parşömen tonlu bölge haritası, yer adları, sen),
  F3 geliştirici bilgisi.

## Testler

```
godot --headless --path . -- --selftest        # dünya: 13 kontrol
godot --headless --path . -- --lifetest=3      # yaşam: 3 oyun günü (~1,2 s)
```

Son `--lifetest=3` sonucu: köyde gündüz (09–17) dışarıda ort. %42 (en çok %65); gece (23–04) %0;
en kalabalık 5 m'lik öbek 8 kişi (akşam meydanda sohbet); nedensiz dışarıdaki kişi 0; takılan 0;
bina içinden geçen örnek 1/12 009; düz çizgi yedek rota 0; yol ağı tek parça (540 düğüm);
köylü günlük yürüyüş ortalama ~880 m. **PASS**. Planda gündüz için %35 yazmıştım; ölçünce köyde gündüz
dışarıdakilerin çoğunun amaçlı olduğu (oynayan çocuklar, kapı önünde yaşlılar, bahçe, kuyu, demirhane) görüldü,
asıl sorunu (yığılma ve amaçsızlık) ayrı ölçütlerle ölçüyorum: öbek ≤ 8 ve nedensiz 0.

Yeni geliştirme argümanları: `--lifetest=N`, `--follow=Ad` (kişiyi izleyen kamera), `--followcam=uzaklık,yükseklik,açı`,
`--card=Ad` (kartı aç), `--debughud`, `--map`.


---

# Dünya temeli (arazi, su, gök, bitki örtüsü, oyuncu)

1:1 ölçekte (metre) yürünen 1200 × 1200 m'lik "Bölge": arazi, dere ve gölet, orman ve korular, çayır/tarla,
gökyüzü ve gündüz/gece, üçüncü şahıs oyuncu. Köy, han ve goblin kampı **yapıcıları sonra gelecek**; bu katman
onların üzerine oturacağı zemini, API'leri ve görsel dili sağlar. Godot 4.7.2 .NET (C#), Forward+, Jolt fizik.

## Çalıştırma

```
cd godot
dotnet build                               # C# derlemesi (uyarısız)
godot --path .                             # oyunu aç (geliştirmede pencereli 1600×900)
godot -e --path .                          # editör
godot --headless --path . --import         # dışa aktarmadan önce glb'leri içe aktar
godot --headless --path . -- --selftest    # dünya testleri; PASS/FAIL, çıkış kodu 0/1
```

Ekran görüntüsü (bulut, lavapipe CPU Vulkan):
```
xvfb-run -a -s "-screen 0 1280x720x24" godot --path . --rendering-driver vulkan --resolution 1280x720 \
  -- --shot=/yol/cikti.png --warm=14 --hour=8.5
```

Geliştirme argümanları (`--` sonrasına, hepsi `src/dev/Dev.cs` içinde):

| Argüman | Ne yapar |
|---|---|
| `--shot=/p.png` `--warm=N` | N kare ısındıktan sonra görüntüyü kaydet ve çık (varsayılan 40; çekimde saat durur) |
| `--hour=H` `--timescale=K` | saati ayarla (ör. 19.5); oyun sn / gerçek sn (60 = 24 dakikalık gün, 0 = dondur) |
| `--cam=x,y,z:tx,ty,tz` | serbest kamera; `y` yerine `@h` = o noktada zeminden h m yukarı (ör. `--cam=-97,@1.65,101:-104,@2.6,88`) |
| `--player=x,z[,yaw]` | oyuncuyu zemine oturtarak taşı; yaw: 0 güney(+z), 90 doğu, 180 kuzey |
| `--campitch=deg` `--zoom=m` `--fov=deg` | oyuncu kamerası açısı/uzaklığı, görüş açısı |
| `--hold=move_forward,sprint` | eylemleri basılı tut (yürüme/koşu animasyonunu çekmek için) |
| `--bench=N` | ısınmadan sonra N kare ölç; FPS, çizim çağrısı, primitif, nesne, bitki/çimen sayıları |
| `--selftest` | başsız dünya testleri |
| `--mapdump=/p.png` | yükseklik + zemin örtüsü + dere haritası (layout kontrolü) |
| `--probe=x,z;x,z` | noktalarda yükseklik, eğim, zemin örtüsü, yol/dere mesafesi yaz ve çık |
| `--dumpmodel=human` | bir glb'nin düğüm ağacını ve animasyonlarını yaz |
| `--noassets` | glb'leri yok say (prosedürel yer tutucularla dünya) |
| `--tonemap=agx\|filmic\|aces` `--exposure=K` | görsel ayar denemesi |
| `--fullscreen` / `--windowed` | pencere kipi (yayın derlemesinde varsayılan tam ekran) |

## Kısayollar

W A S D / oklar: yürü (kameraya göre) · varsayılan **koşar adım 4,0 m/s** · **Shift** basılı: hızlı koşu 11 m/s ·
**Ctrl** (veya Alt) basılı: yürüme 1,6 m/s · **Space**: zıpla (~0,9 m) · fare: kamera (yakalanır) ·
tekerlek: yakınlaştır 2–12 m · **Esc**: fareyi bırak (tıklayınca geri) · **F11** / **Alt+Enter**: tam ekran.
Köyden hana hızlı koşu ~50 sn, koşar adım ~2 dk.

## Ne var

- **RegionSpec** (`src/world/RegionSpec.cs`): paylaşılan spec'teki bütün yerleşim (köy V, han I + yola bakan açı,
  kamp G, yıkık kule R, gölet, mera, 11 tarla parseli, tepeler, orman kuralları, dışlama mesafeleri, oyuncu
  başlangıcı) + düzgünleştirilmiş yollar: `Road`, `Trail`, `CampSpur` (patika ucundan kampa 1,6 m iz),
  `Stream` (spec çizgisi + ±8 m doğal kıvrım, köprüde sönümlü). `BridgePoint` ≈ (−44.5, 91.3).
- **Heightfield** (`src/world/Heightfield.cs`): 601×601 örnek, 2 m. Yuvarlanan taban (±8 m), GB'de yüksek tepeler
  (tepe düzlüğü 45,6 m, en yüksek nokta; 12 m düz), KB'de kayalık sırtlar, kenarlarda ufuk tepeleri, köy pad'i
  (±2 m, hafif eğimli düzlem), han düzlüğü (40×30, tamamen düz), kamp açıklığı, dere vadisi (akış güneyden kuzeye,
  tekdüze inen kıyı profili, yatak 1,5–2,5 m aşağıda, 6–10 m geniş, düzensiz kıyılar), köprü geçişi (7,2 m geniş,
  1,8 m derin kanal; yol kıyı seviyesine sabitlenmiş), yol/patika yatakları (profil yumuşatma + hafif çukur),
  gölet çanağı. Zemin örtüsü 1 m/texel splat dokularında (yol/tarla/kaya/kum, orman/çayır/saban yönü/çukur,
  ekin/aşınma/ıslaklık).
- **Terrain** (`src/world/Terrain.cs`, `assets/shaders/terrain.gdshader`): 15×15 parça × 80 m; LOD0 2 m, LOD1 4 m,
  LOD2 8 m (geçiş 190 / 470 m, ince LOD 12 m'de kaybolarak), eteklikler (1,5/3,5/7 m), parça başına
  ConcavePolygonShape3D (tam çözünürlük, aynı üçgen bölünmesi). Shader: splat + NoiseTexture2D ile boyacı
  (painterly) çimen tonları, keskin gürültülü kenarlı toprak yol, sürülmüş tarla + ekin, eğime göre toprak/kaya,
  çukur gölgesi, uzaktan çayır çiçeği benekleri, yakında hücresel normal kabartı.
- **Water** (`src/world/Water.cs`, `water.gdshader`): dere şeridi (su seviyesi profili, kıyıların altına 1 m taşar,
  akışa göre kayan normaller) + gölet diski; derinliğe göre renk, fresnel, kıyı köpüğü, saydam.
- **DayNight** (`src/world/DayNight.cs`, `sky.gdshader`): `GameClock` (statik; 1 gün = 24 gerçek dakika),
  güneş (4 kaskad, 300 m gölge) + ayrı ay ışığı (mavi, 2 kaskad), gökyüzü shader'ı (gradyan, güneş/ay diski, bulut,
  yıldız), üstel sis + hava perspektifi (+ şafakta hafif vadi sisi), Filmic tonemap, SSAO, glow, gece doygunluk
  düşüşü ve pozlama telafisi. Global shader parametreleri: `night_light`, `fd_cam_pos`, `fd_wind`.
- **Vegetation** (`src/world/Vegetation.cs`, `PlaceholderVeg.cs`): nature.glb'den 18 tür (meşe/çam/huş ×,
  çalı ×3, eğrelti, kaya s/m/l/uçurum, kütük, devrik ağaç). Parça başına belirlenimci yerleşim (orman maskesi +
  gürültü + titreşimli ızgara), dere boyu ağaç/çalı şeridi, kıyı taşları, yalnız ağaçlar, orman kenarı çalıları.
  ~13 000 ağaç, ~42 000 örnek. Yakın ("hi") MultiMesh parça başına, uzak ("lo") 240 m gruplarda; geçiş örnek
  başına shader'da, ekran uzayı titreşimle (dither). Gövde silindiri / kaya kutusu çarpışması (PhysicsServer3D,
  parça başına tek statik gövde). glb yoksa prosedürel yer tutucular.
- **GrassField** (`src/world/GrassField.cs`, `grass.gdshader`): kamera çevresinde ~54 m, 24 m karolar; çimen,
  çiçek, dere/gölet kenarında saz, tarlada buğday/arpa sapları ve lahana sıraları (village.glb). Rengi altındaki
  zeminden alır (`ground_color.gdshaderinc`), rüzgârda sallanır, kenarda yere batarak kaybolur.
- **Player** (`src/actors/Player.cs`, `scenes/Player.tscn`) + **Humanoid** (`src/actors/Humanoid.cs`):
  CharacterBody3D kapsül 0,35×1,8 m, 46° eğim sınırı, zemin yakalama, fizik interpolasyonu, SpringArm kamera.
  human.glb varsa Idle/Walk/Run/Jump geçişli ve hıza göre oynatma hızı (Walk 1,4 / Run 4,5 m/s); kıyafet
  seçimi `Humanoid.Outfit` (varsayılan kısa saç + sakal). Yoksa blok yer tutucu figür + prosedürel adım.
- **RoadBridge** (`src/world/RoadBridge.cs`): village.glb `bridge` modeli dere geçişinde (manifest çarpışma
  kutularıyla); yoksa tahta yer tutucu. (Geçici ScaleProbe evleri kaldırıldı; köyü VillageSite kuruyor.)
- **Dev** (`src/dev/Dev.cs`, autoload) + **SelfTest** (`src/dev/SelfTest.cs`), **App** (`src/core/App.cs`,
  autoload: girdi eylemleri, pencere kipi). `src/core`: Noise (belirlenimci gürültü), Polyline/DistanceField,
  FMath, MeshKit (düz gölgeli köşe renkli örgü üretici), Models (glb yükleyici).

## Diğer sistemler için API (köy/han/kamp yapıcı, NPC yapay zekâsı)

- Yeni bir yapıcı `IRegionFeature` uygular (`Shape(Heightfield)` → bake öncesi pad/yol/dışlama;
  `Build(Region)` → düğümler) ve `Region.CreateFeatures()` içine eklenir.
- `hf.AddPad(merkez, yarıBoyut, açıDeg, yükseklik=NaN, blend, wear, vegetationMargin)` → döndürür: gerçek taban
  yüksekliği (NaN = ayak izi altındaki ortalama). Açı: parselin U ekseninin (x,z) düzlemindeki yönü, atan2(z,x).
  `hf.AddPadCircle(...)`, `hf.AddPath(noktalar, genişlik, flatten, wear)` (köy sokakları: düzleştirir, toprak boyar,
  bitkiyi temizler), `hf.ExcludeVegetation(merkez, yarıçap)`. Hepsi **Bake'ten önce**.
- Sorgular: `Height(x,z)` (çizilen/çarpışan örgüyle birebir üçgen enterpolasyonu), `HeightBilinear`, `Normal`,
  `Slope` (derece), `Biome(x,z)` → BiomeWeights (grass/meadow/forest/dirt/rock/sand/field),
  `ForestDensity`, `Clearance(x,z,ClearKind)`, `RoadDistance`, `TrailDistance`, `StreamDistance(x,z,out s)`,
  `StreamWaterAt(s)`, `StreamBankAt(s)`, `IsWater`, `PondLevel`, `InnPadHeight`, `RuinTopHeight`,
  `BridgeRoadHeight`, `RoadProfileAt(s)`. `Vegetation.Trees` (gövde konumları).
- Saat: `GameClock.Hour`, `.Day`, `.IsNight`, `.Daylight`, `.TimeScale`, `.SetHour()`, `.HourChanged`,
  `GameClock.SunDirection(hour)`.
- Modeller: `Models.Exists("x")`, `Models.Meshes("x", MatKind.Static|Multi|Grass|Character)` (düğüm adına göre
  örgüler, malzeme takılı), `Models.Mesh(...)`, `Models.Instantiate("human")`, `Models.Manifest("x")`.
- Shader'lar: `fd_vc` (MeshInstance; instance uniform `tint` doğrusal RGB, `lit` 0..1), `fd_vc_mm`
  (MultiMesh; `use_custom_data`: custom.rgb = ton, custom.a = lit; `lod_min/lod_max/lod_fade`, `wind_amount`,
  `sway_height`, `flutter`, `leaf_backlight`), `grass` (custom.a=0 zeminden renk, 1 = mutlak ton),
  `fd_char` (instance `skin_color`, `cloth1_color`, `cloth2_color`, `hair_color`, doğrusal).
  Köşe renkleri glb'den doğrusal gelir (plaster 0xe9dcc0 → 0.812,0.714,0.525 doğrulandı) — Blender önizlemesiyle aynı.
- Fizik katmanları: 1 terrain · 2 props · 3 player · 4 actors. Jolt **açıkça** seçili (bu sürümde DEFAULT = GodotPhysics).

## Sayılar (1280×720, lavapipe CPU Vulkan — FPS temsil etmez; çizim çağrısı ve primitif temsil eder)

| Görünüm | Çizim çağrısı | Primitif (gölge geçişleri dahil) | Nesne | Not |
|---|---|---|---|---|
| Başlangıç (−110,105), 08:30 | 627 | 2,47 M | 627 | 25 çimen karosu, 21 105 çimen örneği |
| Genel bakış (GB tepe, 13 m yukarıdan), 09:00 | 676 | 1,94 M | 678 | tüm bölge görünür |
| Orman içi (150,−165), 10:30 | 739 | 5,61 M | 739 | en ağır görünüm |

Video belleği ~167 MB, düğüm ~4 400, fizik < 1,5 ms. Açılış (2 çekirdek): yükseklik ~0,8–1,5 s, bake ~1,5–3 s,
arazi ~1–2 s, bitki ~0,7–1,7 s → toplam 5–12 s. Primitif sayısı 4 gölge kaskadını ve shader'da gizlenen
(uzak/yakın LOD) örnekleri de sayar; gerçek GPU'da tepe noktası işi ucuzdur.

## Ekran görüntüleri (`godot/shots/`)

`shot_start.png` (başlangıç, doğuya), `shot_village.png` (göz hizası, kapıda figür: 1,73 m / kapı 2,1 m /
mahya 7,4 m), `shot_forest.png` (orman patikası), `shot_overview.png` (yıkık kule tepesinden KD'ye),
`shot_dusk.png` (19:30, yanan pencereler), `shot_night.png` (23:00), `shot_bridge.png`, `shot_run.png` (depar pozu).

## Bilinen konular / sonraki adımlar

- Gerçek FPS kullanıcının ekran kartında ölçülmeli. Yük düğmeleri: `VegSpecies.HiRange` (ağaç 85 m),
  `Terrain.LodSwitch`, gölge mesafesi 300 m, `GrassField.Radius/Spacing`.
- Arazi LOD geçişi: ince LOD 12 m'lik bantta saydamlaşarak kaybolur (Godot "Self" fade → o bantta alfa geçişi);
  kaba LOD opak başlar, delik oluşmaz. Fade kapalıyken marjlar histerezis olduğu için kullanılmadı.
- Godot'nun yükseklik sisi ışın boyunca değil sabit uygulanıyor; bu yüzden yalnız şafakta çok hafif vadi sisi var.
- Su çarpışmasız: dere yatağında yürünür (yüzme yok). Köprü village.glb'den; yapıcılar isterse taşıyabilir.
- Tarlada 3B ekinler yalnız yakında (~54 m).
- Editör derlemesinde glb'ler çalışma anında GLTFDocument ile okunur (her zaman güncel); dışa aktarmadan önce
  `--import` çalıştırılmalı (dışa aktarılan oyun içe aktarılmış kaynakları kullanır).
- KB kayalık tepeler sade kalıyor (kaya kapakları + uçurum kayaları); ileride elle yerleştirilmiş kaya grupları.
- Gece: ay ışığı + pozlama telafisi ile okunur; köy fenerleri/meşaleler (village.glb lamp/torch) köy yapıcısında.
