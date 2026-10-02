# Fantastik Dünya — açık dünya dikey dilimi (durum, 30 Eylül 2026)

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
