# Fantastik Dünya Simülasyonu
## Oyun Tasarım Dokümanı — v0.1

Bu doküman, tasarım görüşmesinde oluşan oyun fikrini ve sistem kararlarını bir araya getirir. İlk tasarım sürümüdür. **Net kararlar**, **çalışma yönleri** ve **açık konular** metinde ayrıştırılmıştır. Sayısal örnekler denge ayarı için başlangıç fikirleridir.

## 1. Oyun fikri

Büyük, üç boyutlu ve bakması keyifli bir fantastik dünyada küçük topluluklar yerleşimler kurar. Bu yerleşimler kendi ihtiyaçlarını karşılar, nüfuslarını artırır, araştırma yapar ve zamanla farklı alanlarda gelişir. Genişledikçe başka medeniyetlerle karşılaşır; ticaret, göç, dostluk, rekabet ve savaş ortaya çıkar.

Kahramanlar bu toplu akışın içindeki istisnai karakterlerdir. Tavernalarda ortaya çıkabilir, bağımsız dolaşabilir ve bir medeniyet tarafından bünyeye katılabilirler. Dünyadaki ihtiyaçlar ve tehlikeler, kahramanlar için görevler üretir.

Temel deneyim, küçük bir kampın gelişimini ve bu gelişimin dünyada yarattığı sonuçları izlemektir. Oyun, nedenleri anlaşılabilen olaylardan kendi hikâyelerini üretir.

**Mevcut aşama:** Genel oyun tasarımı. Oyuncunun rolü ileride belirlenecek; bu sürüm prototip kapsamı veya geliştirme prompt’u tanımlamaz.

## 2. Tasarım ilkeleri

- **Kendi kendine yaşayan dünya:** Yerleşimler, oyuncu müdahalesi olmadan temel faaliyetlerini sürdürebilir.
- **Yerleşim ölçeğinde simülasyon:** Halk; nüfus, iş gücü, ihtiyaçlar ve üretim üzerinden ele alınır. Kahramanlar ayrı takip edilebilir.
- **Görünür gelişim:** Araştırma ve büyüme; binalarda, yollarda, üretimde ve hareketlilikte hissedilir.
- **Nedeni anlaşılabilen olaylar:** Çatışma, göç veya görevler dünyadaki koşullardan doğar.
- **Basit araştırma paketleri, kümülatif ilerleme:** Araştırmalar anlaşılır ve anlamlı gelişmeleri açar. Derinlik, önkoşulların ve farklı dalların birleşmesinden gelir.
- **Farklılaşan medeniyetler:** Coğrafya, nüfus bileşimi, sınıf ve geçmiş olaylar gelişim yollarını etkiler.
- **Gerçek sonuçlar:** Yerleşimler büyüyebilir, gerileyebilir ve yok olabilir.

## 3. Temel kavramlar

| Kavram | Anlamı |
|---|---|
| Irk | İnsan, ork veya cüce gibi fantastik köken. Bazı alanlardaki yatkınlıkları etkileyebilir. |
| Medeniyet / ulus | Ortak siyasi bağlılığa, kaynak yönetimine ve araştırma ilerlemesine sahip topluluk. |
| Yerleşim | Bir medeniyete bağlı kamp, köy veya şehir. Bir medeniyet zamanla birden fazla yerleşime yayılabilir. |
| Medeniyet sınıfı | Medeniyetin uzmanlaşma kimliği ve özel araştırma ağacını belirleyen yönelim. |
| Kahraman | İsmi, yetenekleri ve geçmişiyle ayrı takip edilebilen istisnai karakter. |
| Bağımsız tehlike odağı | Medeniyetlerin dışında bulunan ve çevresinde tehlike üreten kamp veya benzeri dünya unsuru. |

Irk, medeniyet ve sınıf ayrı kavramlardır. Aynı ırktan farklı medeniyetler bulunabilir. Bir medeniyet göç ve ilişkiler yoluyla birden fazla ırkı barındırabilir. İnsanlarla orkların birlikte yaşadığı bir zanaatkâr veya büyücü medeniyeti mümkündür.

Medeniyet sınıfı, diplomatik davranışı tek başına belirlemez. Savaşçı bir medeniyet güvenilir bir müttefik olabilir.

## 4. Dünya ve görsel yön

### Görsel hedef

Üç boyutlu, şık, sevimli ve izlemesi keyifli bir dünya hedeflenir. Low poly yaklaşımı güçlü bir adaydır. Voxel kullanımı ve grid tabanlı dünya düzeni seçenek olarak açıktır. Üretilen bir dünya fikri vardır; üretim yöntemi ve kuralları henüz belirlenmemiştir.

Gelişim çevrede okunabilir olmalıdır: tarlalar genişler, küçük evlerin yanında atölyeler açılır, yollar üzerinde taşıma başlar, pazarlar ve tavernalar hareketlenir. Sınıfların ilerleyen uzmanlıkları da yerleşim görünümüne yansıyabilir.

### Coğrafyanın rolü — çalışma yönü

Kaynakların ve arazinin dağılımı yerleşimlerin gelişimini etkiler. Verimli bölgeler tarımı, maden bulunan bölgeler üretimi, yolların kesiştiği yerler ticareti destekleyebilir. Bu farklılıklar medeniyetlerin birbirine ihtiyaç duymasını sağlar.

Tam biyom listesi, harita boyutu, arazi değişiklikleri ve keşif kuralları açıktır.

## 5. Yerleşimler, nüfus ve ekonomi

### Başlangıç

Dünya küçük kurucu topluluklarla başlar. Üç-beş kişilik başlangıç temel örnektir; tek kurucuyla başlama da konuşulan olasılıklardandır. Kesin başlangıç nüfusu henüz belirlenmemiştir.

Topluluk temel hayatta kalma ve gündelik yaşam becerilerine sahiptir. Bu gündelik beceriler ayrı ayrı araştırma düğümlerine bölünmez. Erken gelişim, kalıcı üretim ve yerleşim olanaklarını açan büyük araştırma paketleriyle ilerler.

### Halkın modellenmesi

Normal nüfusun ana işlevi toplu simülasyonu beslemektir. İş gücü üretim, yapım, taşıma ve araştırma gibi faaliyetlere ayrılır. Bu işler ekranda karakter hareketleriyle temsil edilir. Her sıradan kişiye ayrıntılı bir hayat hikâyesi veya kişisel yetenek ağacı verilmesi tasarımın odağı değildir.

Besin, barınma ve güvenlik yerleşimin gelişimini etkileyen temel ihtiyaçlardır. Nüfus artışı ve göçün kesin kuralları ayrıca tasarlanacaktır.

### Büyüme ve genişleme

Yerleşim büyüdükçe yeni ihtiyaçlar ve kaynak arayışları doğar. Uzak bir kaynağı kullanmak için aynı medeniyete bağlı ikinci bir yerleşim kurulabilir. Yerleşimler arasında mal taşınır ve yollar önem kazanır.

Araştırma bir olanağı açar; onu kullanacak bina, üretim ve iş gücü ayrıca sağlanır. Örneğin madenciliği öğrenen bir medeniyet, uygun kaynağa erişip maden kurarak üretime geçer.

Kaynak listesi, para sistemi, üretim oranları, bakım maliyetleri ve yerleşimler arası kaynak paylaşımının ayrıntıları açıktır.

## 6. Diplomasi ve medeniyet ilişkileri

Diplomasi medeniyetler arasında yürür. İlişkiler; ihtiyaçlardan, karşılıklı çıkarlardan, güven birikiminden ve geçmiş olaylardan etkilenir.

Çalışma yönündeki ilişki örnekleri:

- Kaynakları birbirini tamamlayan medeniyetler ticaret yapar.
- Süren ticaret ve yardımlaşma, daha yakın ilişkileri destekler.
- Uygun ilişkiler göçe ve karma nüfusa zemin oluşturabilir.
- Aynı kaynak veya bölgeye genişlemek anlaşmazlık yaratabilir.
- Ortak bir tehlike, iş birliğini veya ittifakı teşvik edebilir.
- Bir medeniyete bağlı kahramanın komşuya yardım etmesi ilişkilere yansıyabilir.

Dostluk, gerilim, savaş ve barışın görülebilir nedenleri olmalıdır. Bir medeniyet incelendiğinde başka bir medeniyetle neden iyi veya kötü ilişki içinde olduğu anlaşılabilmelidir.

Antlaşma türleri, savaş ilanı koşulları, barış süreçleri, sınır hakları ve bilgi paylaşımı henüz ayrıntılandırılmamıştır.

## 7. Bağımsız tehlikeler, görevler ve yok oluş

Haritada medeniyetlerden ayrı tehlike odakları bulunur. Çevredeki yollara baskın yapan bir kamp temel örnektir. Bu odakların adlandırılması ve tam tür listesi açıktır.

Örnek olay zinciri:

**Kamp faaliyeti → yol güvenliğinin bozulması → taşımanın aksaması → yerleşimin zorlanması → savunma veya kahraman ihtiyacı.**

Tehlike, haritada bir kaynaktan doğduğu için gelişimi izlenebilir. Müdahalenin sonucu da dünya üzerinde görülür. Baskı altında kalan yerleşimler gerileyebilir ve yok olabilir; bunun sıklığı henüz belirlenmemiştir.

Görevler dünyanın ihtiyaçlarından doğar. Bir tehlikeyi gidermek, bir güzergâhı korumak veya yeni bir bölgeyi araştırmak olası görev örnekleridir. Kesin görev türleri ve çözüm kuralları ileride seçilecektir.

## 8. Tavernalar ve kahramanlar

### Tavernanın rolü

Mevcut çalışma modelinde tavernalar, kahramanların ortaya çıktığı ve medeniyetlerle buluştuğu merkezlerdir. Taverna, yerleşimin gelişiminde anlamlı bir dönüm noktası olur.

Kahramanların yalnızca tavernadan ortaya çıkması konuşulan tasarım yönüdür. Tavernanın kesin açılma koşulları ve kahraman oluşturma sıklığı henüz sayısallaştırılmamıştır.

### Bağlılık

İki temel kahraman durumu vardır:

1. **Bağımsız:** Henüz bir medeniyete katılmamış kahraman.
2. **Medeniyete bağlı:** Bir ulusun bünyesine kattığı kahraman. Bu katılım kalıcı bağlılık olarak ele alınır; kahraman savunma, sefer ve görevlerde o medeniyet adına kullanılır.

Kahraman edinmenin bir maliyeti vardır. İlk kahraman, küçük bir yerleşim için önemli bir yatırım olmalıdır. Ortaya çıkma maliyetiyle işe alma maliyetinin nasıl ayrılacağı açıktır.

Kahramanlar isim, yetenek ve geçmiş üzerinden farklılaşabilir. Kişisel ilerleme, donanım, ölüm ve savaş kuralları henüz tasarlanmamıştır. Araştırmalar ileride kahramanların eğitim veya donanım olanaklarını geliştirebilir.

### Açık kahraman konuları

- Medeniyetler arasında kahraman verme veya aktarma fikrinin kuralları.
- Bağımsız kahramanlara tek seferlik görev verilip verilemeyeceği.
- Bir medeniyet yok olduğunda ona bağlı, hayatta kalan kahramanların durumu.
- Kahraman sınıfları, sayısı ve görev seçimi.

Bu başlıklar mevcut kurallara eklenmiş kesin mekanikler değildir.

## 9. Medeniyet sınıfları

Beş sınıf genel çerçeve olarak benimsenmiştir:

| Sınıf | Gelişim kimliği | Özel ağacın kapsayabileceği olanaklar |
|---|---|---|
| Savaşçı | Askerî örgütlenme ve savunma | İleri savunma, birlik düzeni, sefer olanakları, kahraman liderliği. |
| Büyücü | Büyü bilgisi ve uygulamaları | Büyü yapıları, koruma etkileri, efsunlama ve özel donanım. |
| Zanaatkâr | Üretim, işçilik ve mühendislik | Usta atölyeleri, gelişmiş malzemeler ve üretim olanakları. |
| Tüccar | Ticaret ağları ve ekonomik ilişkiler | Loncalar, gelişmiş kervan örgütlenmesi, yabancı yerleşimlerde şubeler. |
| Doğa odaklı | Canlılar ve çevreyle uyum | Korular, doğa büyüsü, hayvan yoldaşları ve çevreyle bütünleşen yapılar. |

Ortak ağaç bütün medeniyetlere temel gelişim olanakları sağlar. Özel ağaçlar sınıfın güçlü yönlerini derinleştirir. Bir sınıfa sahip olmak, temel ticaret veya savunma gibi ortak faaliyetleri diğer sınıflara kapatmaz.

Tablodaki özel olanaklar çalışma örnekleridir; tam araştırma listeleri değildir. Sınıfın kuruluşta nasıl belirlendiği, değişip değişemeyeceği ve karma uzmanlaşma kuralları açıktır.

## 10. Araştırma sistemi

### 10.1 İki ağaç, bağımsız medeniyet ilerlemesi

Her medeniyetin iki araştırma ağacı vardır:

- **Ana ağaç:** İçeriği bütün medeniyetlerde ortaktır. Her medeniyet bu araştırmaları kendi başına tamamlar.
- **Özel ağaç:** Medeniyetin sınıfına özgüdür. O sınıfın uzmanlıklarını geliştirir.

Ana ve özel ağaç, ilkel yerleşim döneminden gelişmiş medeniyet düzeyine uzanan kümülatif yollar sunar. Özel ağaç, yalnızca çok ileri dönemde başlayan bir ek sistem olarak düşünülmez.

### 10.2 Araştırma düğümünün ölçeği

**Her araştırma, anlaşılır bir gelişim paketidir.** Birkaç bağlantılı olanağı birlikte açabilir.

| Örnek ana araştırma | Açtığı gelişim paketi |
|---|---|
| Tarım | Tarla kurma, ekim ve hasat yoluyla düzenli gıda üretimi. |
| İnşaat | Kalıcı ev ve depo yapımı. |
| Hayvancılık | Sürü yetiştirme ve hayvansal üretim. |
| Madencilik | Maden kurma ve cevher çıkarma. |

İp yapma, yiyecek kurutma ve sepet örme gibi küçük gündelik işler ayrı araştırma düğümleri değildir. Araştırma listesinin bu ayrıntılarla sınırsız biçimde büyütülmesi istenmez.

İlkel başlangıç, medeniyetin ilk üretim ve yerleşim sistemleriyle ilgilidir. İlerleyen araştırmalar bunları genişletir: sulama, gelişmiş yapı yöntemleri, demircilik veya daha büyük taşıma ağları gibi. Kesin düğüm sayısı ve tam ağaç henüz belirlenmemiştir.

### 10.3 Kümülatif derinlik ve önkoşullar

İleri araştırmalar, önceki gelişmelerin üzerine kurulur. Bazı düğümler farklı dallardan birden fazla önkoşul ister. Ana ağaçla özel ağaç da bu şekilde bağlanabilir.

Tasarım örnekleri:

- Gelişmiş kervan olanakları; üretim, taşıma ve bağlantı altyapısının olgunlaşmasına dayanabilir.
- Savaşçı medeniyetin ileri sınır savunması, ana ağaçtaki taş yapı bilgisine ihtiyaç duyabilir.
- Ağır bir muhafız birliği, donanım üretimi ile askerî örgütlenmenin birleşmesini gerektirebilir.
- Büyülü donanım, ilgili üretim bilgisi ve büyücü özel ağacındaki efsunlama bilgisini birlikte gerektirebilir.

Araştırmanın tamamlanması bilgiyi açar. İlgili yapı veya birimin kurulması ayrıca kaynak ve emek gerektirir.

### 10.4 Tek aktif odak

**Her medeniyet aynı anda tek bir araştırmaya odaklanır.** Bu araştırma ana ağaçtan veya özel ağaçtan seçilir.

Birden fazla yerleşimde araştırma masası bulunuyorsa hepsi medeniyetin aynı aktif araştırmasına katkıda bulunur. Medeniyetin ihtiyaçları ve gelişim yönü, araştırma önceliğini etkiler. Oyuncunun bu seçime etkisi, oyuncu rolüyle birlikte belirlenecektir.

### 10.5 Araştırma masası ve iş gücü

Araştırma, dünyada fiziksel karşılığı olan bir araştırma masasında yürütülür. Nüfustan araştırmaya ayrılanlar masa başında çalışırken görülebilir.

Başlangıçta kurulabilen basit araştırma masası çalışma modelidir. Daha ileri araştırma yapılarının hangi aşamalarda açılacağı ayrıca tasarlanacaktır.

Araştırmaya ayrılan iş gücü günlük üretim ve diğer ihtiyaçlarla birlikte değerlendirilir. Araştırmacı sayısının, yapıların ve koşulların hız üzerindeki kesin hesabı açıktır.

### 10.6 Yatkınlıklar ve karma nüfus

Irksal veya sınıfsal yatkınlıklar, herkese açık belirli araştırmaları daha hızlı öğrenmeyi sağlayabilir. Örneğin cücelerin madencilik araştırmasında **yüzde otuz hız bonusu** alması konuşulan bir örnektir. Bu oran kesin denge kararı veya resmî D&D kuralı değildir.

Karma nüfus için çalışma modeli, ilgili bonusun araştırmada çalışan nüfusun bileşimine göre oluşmasıdır. Böylece medeniyetin ırksal çeşitliliği araştırma kapasitesine yansıyabilir. Bonusların birleşme yöntemi ve sınırları henüz belirlenmemiştir.

### 10.7 Dünya olaylarının sınırlı katkısı

**Araştırmanın ana kaynağı masa başında yapılan çalışmadır.** Dünyadaki keşifler ve önemli olaylar ara sıra küçük hız artışları sağlayabilir. Bu katkının etkisi sınırlı tutulur.

Araştırma ilerlemesi büyük ölçüde olay kovalamaya, görev zincirlerine veya sık tetiklenen bonuslara dayanmaz. Olay örnekleri, bonus süreleri ve sayıları ileride seçilecektir.

## 11. Gözlem deneyimi

Oyuncunun nihai rolü açık olsa da dünyanın anlaşılır biçimde izlenebilmesi temel hedeftir.

Önerilen izleme araçları:

- Dünyayı farklı uzaklıklardan görmek ve yerleşimlere yaklaşmak.
- Simülasyon zamanını duraklatmak ve hızlandırmak.
- Bir medeniyetin yerleşimlerini, nüfusunu ve mevcut araştırma odağını incelemek.
- Bir yerleşimin üretimini, ihtiyaçlarını ve yaşadığı sorunları görmek.
- Diplomatik ilişkilerin önemli nedenlerini öğrenmek.
- Kahramanları ve dünyada sonuç yaratan olayları takip etmek.

Bu araçlar gözlem ihtiyacını tarif eder. Oyuncuya yerleşim kurma, doğrudan emir verme veya tanrısal müdahale yetkisi verilmesi henüz kararlaştırılmamıştır.

## 12. Açılış deneyimi örneği

1. Nehir yakınında küçük bir insan topluluğu kamp kurar. Uzakta başka bir topluluk, örneğin orklar, kendi yerleşimini başlatır.
2. İnsan topluluğu besin ve barınma ihtiyacını karşılamaya çalışır. Küçük bir araştırma masasında tek bir konu üzerinde çalışılır.
3. Tarım veya inşaat gibi ilk paketler tamamlandıkça gerekli kaynaklar ayrılır; tarlalar, evler ve depolar görünür hâle gelir.
4. Yerleşimin artan ihtiyaçları yeni araştırma ve kaynak arayışlarını doğurur. Zamanla taverna kurulabilecek koşullar oluşur.
5. Bağımsız bir kahraman tavernada ortaya çıkar. Medeniyet kaynak ayırarak onu bünyesine katabilir.
6. Uzak bir kaynağın kullanımı için ikinci bir yerleşim kurulur. İki yerleşim arasında taşıma ve yol güvenliği önem kazanır.
7. Başka medeniyetle temas, ticaret veya anlaşmazlık yaratır. Bir tehlike odağı ortak soruna dönüşebilir.
8. Medeniyetlerin seçtiği araştırmalar, diplomatik ilişkiler ve kahramanların eylemleri dünyanın sonraki gelişimini etkiler.

Bu akış örnek bir hikâyedir. Başlangıçların aynı olayları aynı sırada üretmesi zorunlu değildir. Kesin süreler ve ilerleme hızı henüz belirlenmemiştir.

## 13. Açık tasarım konuları

| Başlık | Belirlenecek konu |
|---|---|
| Oyuncu rolü | Gözlem, dolaylı müdahale veya doğrudan yönetimin kapsamı. |
| Harita ve görsel üretim | Grid, voxel ve low poly ilişkisi; dünya üretimi, ölçek ve biyomlar. |
| Nüfus | Başlangıç sayıları, büyüme, göç, ırk karışımı ve iş gücü dağılımı. |
| Medeniyet sınıfları | Sınıfın belirlenmesi, değişimi ve kesin özel araştırma ağaçları. |
| Ekonomi | Kaynaklar, para, üretim, bakım ve yerleşimler arası paylaşım. |
| Araştırma | Tam düğüm listesi, maliyetler, süreler, bonus hesabı ve teknolojik son nokta. |
| Diplomasi | Antlaşmalar, sınırlar, savaş ve barış koşulları. |
| Savaş | Birliklerin ve kahramanların çatışma çözümü, kayıplar ve fetih. |
| Kahramanlar | Sınıflar, ilerleme, donanım, edinme maliyeti, aktarım ve ölüm. |
| Görevler ve tehlikeler | Türler, ortaya çıkma koşulları ve dünya üzerindeki baskı düzeyi. |
| Oyun süresi ve amaç | Açık uçlu simülasyon, hedefler ve olası bitiş koşulları. |

Bu konular ilk tasarım dokümanının hazırlanmasına engel değildir. Sonraki tasarım görüşmeleri bu sürümü ayrıntılandıracaktır.

## 14. Araştırma tasarımına kaynak olan inceleme

RimWorld’de üretim olanaklarının araştırmayla açılması ve bazı ilerlemelerin farklı bilgi dallarını gerektirmesi incelendi. Plaka zırhın demircilik ve giysi bilgisine dayanması, birden fazla önkoşul için somut örnektir. [RimWorld Wiki — Research](https://mail.rimworldwiki.com/wiki/Research)

Ludeon’un tasarım notunda düşük teknoloji seçeneklerinin genişletilmesi ve bazı araştırmalar için farklı araştırma tesisleri kullanılması açıklanır. [Ludeon — Alpha 13 tasarım notu](https://ludeon.com/blog/2016/03/features-summary-alpha-13/)

https://www.civilopedia.net/en-US/gathering-storm/technologies/tech_education/)



Bu oyunun araştırma paketleri, iki ağaçlı yapısı ve medeniyet simülasyonuna uyarlaması bu görüşmeye ait tasarım önerileridir.
