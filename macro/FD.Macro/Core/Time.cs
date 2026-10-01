using System;

// Faz 1b-5: yol haritası v3 zaman ölçeği.
//
// Saat ve takvim: 1 oyun günü = 30 gerçek dakika (gündüz ~22, gece ~8 dk); 1 oyun saati = 75 gerçek saniye. Mevsim yok.
// 1 hafta = 5 gün (ödeme günü), 1 ay = 10 gün, 1 yıl = 40 gün. Yıl yalnız kronik ve yaş için bir etikettir.
//
// Dönüşüm kuralı: eski simülasyonun 120 günlük yılı yeni takvimde ~30 gündür (OLD_YEAR). Eski günlük oranlar yeni günde ×PACE (4),
// eski gün cinsinden süreler ÷PACE; "yıl" tabanlı süreler ve pencereler (N yıl içinde, yıl kapıları) OLD_YEAR ile küçültüldü, sonra
// v3'ün hedef tablosuna göre ayarlandı. Takvim yılına bağlı kalanlar yalnız yıllık diye anlatılan olaylar (yaş, sınıf bayramları,
// ejderhanın yıllık haracı, hanın yıllık defteri) ve etiketlerdir.
//
// Mesafe: 1 hex ≈ 200 m; koşar adım 4 m/s ≈ 1,5 hex / oyun saati (komşu yerleşimler 6–12 saat, haritanın bir ucundan öbürüne 2–3 gün).
// Ajan hızları (Pace) fiziksel hızdan ve günde kaç saat yol alındığından hesaplanır (hex / gün).

namespace FD.Macro;

public sealed partial class Sim
{
    /// <summary>takvim: 1 yıl = 40 gün = 4 ay; 1 ay = 10 gün = 2 hafta; 1 hafta = 5 gün (ödeme günü)</summary>
    public const int YEAR = 40, MONTH = 10, WEEK = 5;

    /// <summary>eski 120 günlük yılın yeni takvimdeki karşılığı (gün): eski "yıl" tabanlı süreler ve pencereler buna göre küçültüldü</summary>
    public const int OLD_YEAR = 30;

    /// <summary>eski günün yeni gündeki karşılığı: eski günlük oranlar ×PACE, eski gün cinsinden süreler ÷PACE (1 eski yıl = 120 eski gün ≈ 30 gün)</summary>
    public const double PACE = 4;

    /// <summary>takvim yılı (1 tabanlı; gün 0–39 → 1)</summary>
    public int Year => W.Day / YEAR + 1;

    /// <summary>Dinamik yıl: dünyanın yaşı eski takvimle (1 + gün / <see cref="OLD_YEAR"/>). Eskiden yıla bağlı kapılar ve büyüme eğrileri
    /// (troller 15. yıldan sonra, kampların büyüyen nüfus tavanı, ilk krizler…) buna bakar: dünyanın gelişme hızı değişmedi, yalnız ölçek.</summary>
    public int DynYear => W.Day / OLD_YEAR + 1;

    /// <summary>Düzenli tik: ortalama <paramref name="period"/> günde bir (çeyrek gün çözünürlüğü; 2,5 → 2 ve 3 gün arayla). Tamsayı
    /// dönemde eski <c>(gün + faz) % dönem == 0</c> ile aynıdır. Dönem en az 1 gün.</summary>
    public bool Every(double period, double phase = 0) => Tick(W.Day, period, phase);

    /// <summary><see cref="Every"/>'nin gün verilmiş hâli (tamsayı aritmetiği; deterministik).</summary>
    public static bool Tick(int day, double period, double phase = 0)
    {
        long q = (long)Math.Round(Math.Max(1, period) * 4), t = (long)Math.Round((day + phase) * 4);
        if (t < 4) return false;
        return t / q != (t - 4) / q;
    }

    /// <summary>takvim: yılın ayı (1–4) ve ayın günü (1–10); gün 0 = 1. yılın 1. ayının 1. günü</summary>
    public static (int Year, int Month, int Day) Calendar(double d)
    {
        int day = (int)Math.Floor(d);
        int y = (int)Math.Floor(day / (double)YEAR), dy = day - y * YEAR;
        return (y + 1, dy / MONTH + 1, dy % MONTH + 1);
    }

    public string DateStr() => DateStr(W.Day);

    /// <summary>Faz 1b-5: "Yıl 12, 3. ay, 7. gün" (yol haritası v3 takvimi: 40 günlük yıl, 10 günlük ay)</summary>
    public string DateStr(double d)
    {
        var (y, m, dd) = Calendar(d);
        return $"Yıl {y}, {m}. ay, {dd}. gün";
    }
}

/// <summary>
/// Faz 1b-5: ajan hızları (hex / gün) fiziksel hızdan: hız (m/s, oyunun gerçek zaman ölçeğinde) × 75 sn × günde yol alınan saat / 200 m.
/// Koşar adım (oyuncu) 4 m/s = 1,5 hex/saat; at ~1,6 kat yürüyüş; öküz arabası, göçmen ve ordu yavaş. Eski hızlar (hex / eski gün)
/// yorumlarda; yeni hızlar eskinin ~6 katı (zaman ölçeği ×4 + mesafenin fiziksel ölçeği).
/// </summary>
public static class Pace
{
    /// <summary>1 m/s ile bir oyun saatinde alınan hex: 75 sn × 1 m/s / 200 m</summary>
    public const double H = 75.0 / 200.0;

    /// <summary>koşar adım (oyuncu; başvuru): 4 m/s = 1,5 hex/saat</summary>
    public const double RUNNER_HOUR = 4.0 * H;

    /// <summary>öküz arabalı kervan: 1,0 m/s, günde 10 saat (eski 0,65)</summary>
    public const double CARAVAN = 1.0 * H * 10;
    /// <summary>atlı kervan: 1,6 m/s (öküzün 1,6 katı), günde 10 saat (eski 0,9)</summary>
    public const double CARAVAN_HORSE = 1.6 * H * 10;
    /// <summary>han erzak arabası: 1,1 m/s, günde 10 saat (eski 0,7)</summary>
    public const double SUPPLY = 1.1 * H * 10;
    /// <summary>hancı kafilesi (atlı araba, eşya): 1,6 m/s, günde 10 saat (eski 0,55; v3: han kurulumu 5–10 gün, yol dâhil)</summary>
    public const double KEEPER = 1.6 * H * 10;
    /// <summary>öncüler (aileler, arabalar, hayvanlar): 0,9 m/s, günde 9 saat (eski 0,5)</summary>
    public const double SETTLERS = 0.9 * H * 9;
    /// <summary>evsiz ve aç göçmenler: 0,8 m/s, günde 9 saat (eski 0,45)</summary>
    public const double MIGRANTS = 0.8 * H * 9;
    /// <summary>savaş ordusu (ağırlıklı piyade kolu): 1,1 m/s, günde 9 saat (eski 0,55)</summary>
    public const double ARMY = 1.1 * H * 9;
    /// <summary>kamp seferi ve korsan avı: 1,2 m/s, günde 9 saat (eski 0,6)</summary>
    public const double EXPEDITION = 1.2 * H * 9;
    /// <summary>han baskını (hafif bölük): 1,4 m/s, günde 9 saat (eski 0,7)</summary>
    public const double INNRAID = 1.4 * H * 9;
    /// <summary>yağma akıncıları (hafif, hızlı): 1,6 m/s, günde 9 saat (eski 0,8)</summary>
    public const double PLUNDER = 1.6 * H * 9;
    /// <summary>goblin ve hobgoblin akını: 1,5 m/s, günde 10 saat (geceleri de) (eski 0,9)</summary>
    public const double RAID = 1.5 * H * 10;
    /// <summary>trol akını: 1,6 m/s, günde 10 saat (eski 1,0)</summary>
    public const double RAID_TROLL = 1.6 * H * 10;
    /// <summary>bugbear pususu: 1,8 m/s, günde 10 saat (eski 1,1)</summary>
    public const double RAID_BUGBEAR = 1.8 * H * 10;
    /// <summary>tek kahraman (yaya): 1,5 m/s, günde 10 saat (eski 0,9)</summary>
    public const double HERO = 1.5 * H * 10;
    /// <summary>macera grubu: 1,4 m/s, günde 10 saat (eski 0,85)</summary>
    public const double PARTY = 1.4 * H * 10;
    /// <summary>kâşifler (hafif, çoğu atlı): 1,8 m/s, günde 12 saat (eski 1,1)</summary>
    public const double SCOUT = 1.8 * H * 12;
    /// <summary>yolcu (yaya): 1,3 m/s, günde 10 saat (eski 0,8)</summary>
    public const double TRAVELER = 1.3 * H * 10;
    /// <summary>soylu (atlı maiyet): 1,3 × 1,6 m/s, günde 10 saat (eski 1,0)</summary>
    public const double NOBLE = 1.3 * 1.6 * H * 10;
    /// <summary>gemi (koga, tekne; kıyı boyunca): 1,5 m/s (~3 knot), günde 16 saat (geceleri çoğu kez demirler) (eski 0,85)</summary>
    public const double SHIP = 1.5 * H * 16;
    /// <summary>korsan kayıkları: 2,2 m/s, günde 16 saat (eski 1,25)</summary>
    public const double PIRATE = 2.2 * H * 16;
    /// <summary>eski gemi hızı formülünün tabanı (0,85): eski artılar (açık deniz +0,2, deniz ticareti +0,1, filo +0,05) bunun payı olarak</summary>
    public const double SHIP_OLD = 0.85;
}
