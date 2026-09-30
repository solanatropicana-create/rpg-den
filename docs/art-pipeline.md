# Art pipeline: Blender → oyun

Modeller Blender'da **Python script'iyle** (bpy) üretilir. Script kaynak, `.blend` ve `.glb` onun çıktısıdır. Böylece her model yeniden üretilebilir, varyasyon almak için tohumu değiştirmek yeter, elle yapılan rötuş da script'e geri yazılabilir.

```
assets/blender/fdkit.py          ortak kit: palet, parça üreticiler, dışa aktarım, önizleme render'ı
assets/blender/<model>.py        bir model seti (ör. human_house.py)
assets/blender/<model>.blend     script'in kaydettiği sahne (Blender'da açıp bakmak/rötuş için)
assets/models/<model>.glb        oyunun okuduğu dosya (build'e gömülür)
src/view/glb.ts                  eşzamanlı .glb okuyucu + medeniyet maskeli malzeme
```

## Üretmek

```
# Blender kuruluysa (Windows):
"C:\Program Files\Blender Foundation\Blender 5.0\blender.exe" --background --python assets\blender\human_house.py -- --render
# ya da Python'da bpy modülüyle (pip install bpy==5.0.1, Python 3.11):
python assets/blender/human_house.py --render
node build.mjs
```

`--render` verilirse Cycles ile önizleme PNG'leri de çıkar. `.blend` Blender 5.0 ile kaydedilir. Daha eski bir Blender açamazsa script'i o sürümle çalıştırmak dosyayı yeniden üretir.

## Stil kuralları

- **Ölçek:** Blender'da metre kullanılır. 1 oyun birimi = 15 m (kasabalı ~1,8 m).
- **Yön:** Ön yüz Blender'da **−Y**'ye bakar (oyunda +Z), yukarı +Z'dir. Obje orijini zeminde ve gövde ortasındadır.
- **Doku yok, UV yok.** Renk yüz köşelerine yazılır (`Col`, RGBA, doğrusal). Düz gölgeleme (flat) kullanılır.
  - RGB paletten gelir (`fdkit.PAL`, oyundaki renk ailesi).
  - **A medeniyet maskesidir:** A=1 olan parça oyunda örnek rengiyle çarpılır (çatı, sancak). O parçada RGB gri bir gölge çarpanıdır (1 = tam medeniyet rengi). Kışın kar tonu da yalnız maskeli parçaya biner.
  - Blender önizlemesinde maskeli parçalar kırmızı görünür (`FD_VertexCol` malzemesindeki "Medeniyet rengi" düğümü).
- **İki sürüm (LOD):** `<ad>` uzak/sade (~50–100 üçgen) ve `<ad>_hi` yakın/ayrıntılı. Dış ölçüleri aynıdır.
- **Üçgen bütçesi (yakın sürüm):** ev ≤ 3,5 bin, figür ≤ 500, küçük eşya ≤ 300. Kutuların kenarını pahlamak (bevel) üçgeni 4 katına çıkarır, yalnız göze batan kenarlarda kullanılır. Düzensizlik için `Builder.jitter` kullanılır.
- **Norm:** Dışa aktarımda model kendi normuna bölünür (ev: x,y / 5 m, z / 4 m). Oyun, mevcut yerleşim kodunun verdiği (sx, sy, sx) ile ölçekler. Böylece evin boyu ve genişliği tek tek değişebilir.
- **Işık ve etkileşim yuvaları:** Pencere ışığı, kapı ve baca dumanı oyundaki ayrı örneklerdir. Model onların yerini çerçeveyle bırakır. Yuva ölçüleri model script'inin başında, oyundaki karşılığı `diorama.ts` içindeki ilgili fonksiyonda yazar. Biri değişirse öteki de değişmeli.

## Oyuna bağlamak

1. `diorama.ts`: `import xGlb from '../../assets/models/x.glb'` ekle. `buildRegistry` içinde `for (const [name, g] of parseGlb(xGlb)) r(name, g, this.assetMat);` yaz.
2. LOD: ilgili katmanın `lod` tablosuna `<ad>: ['<ad>_hi', mesafe]` ekle.
3. Kış: maskeli parçada kar isteniyorsa `L.tint` tablosuna `<ad>: snowRoof` ekle.
4. Çizim: `L.add('<ad>', x, y, z, sx, sy, sz, açı, medeniyetRengi)`.

## Kontrol

`node tools/shot.mjs out.png <gün> "" <mesafe> <ev sırası> <fov>` komutu headless Chromium'da oyunu açar ve bir insan kasabasındaki eve bakar. Ortam değişkenleri: `EL` bakış yüksekliği, `CYC` günün saati (0,42 öğle, 0,75 gece yarısı), `HTML` başka bir build, `EVAL` sayfada çalıştırılacak ifade. Örneğin `renderer.info` ile üçgen ve çizim sayısı alınır. Playwright ister: `npm i -D playwright`.

## Durum

| Model | Script | Yakın / uzak üçgen | Oyunda |
|---|---|---|---|
| İnsan evi A (bacalı, çiçeklikli) | human_house.py | 3.104 / 80 | ✓ (`house()` insan dalı) |
| İnsan evi B (odunluklu, mor kepenk) | human_house.py | 3.308 / 68 | ✓ |

Aynı kasaba görünümünde toplam üçgen ~%8 arttı (1,12 M → 1,21 M), çizim çağrısı +1.
