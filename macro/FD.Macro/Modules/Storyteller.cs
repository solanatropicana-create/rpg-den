using System;
using System.Collections.Generic;
using System.Linq;

// Faz 1 B2: anlatıcı. Dünyanın gerilimini 2 yıllık kayan pencerede ölçer: muharebeler ve ölüleri, canavar baskınları,
// yanan yapılar ve evler, salgın ve açlık ölüleri, kıtlık günleri, fetih ve yağmalar, kahraman ölümleri. Uzun sessizlikte
// dünyanın durumuna göre bir kriz seçer (istila dalgası, Kızıl Ay, kuraklık, kara veba, trol çetesi, ejderha akını); gerilim
// zirve yapınca bir süre rahatlama gelir (yeni kriz yok; bereketli hasat ya da şenlik). Krizler kalıcı kronikte büyük olay
// ("crisis") olarak Türkçe nedenleriyle yazılır. Ejderhanın günlük işi de buradan çağrılır (Dragon.Tick). Durum: World.Story.

namespace FD.Macro;

public static class Storyteller
{
    /// <summary>gerilim kovası (gün) ve pencere (kova): 24 × 2,5 gün = 60 gün (eski 24 × 10 gün = 2 eski yıl; Faz 1b-5)</summary>
    public const double BUCKET = 10 / Sim.PACE;
    public const int WINDOW = 24;
    /// <summary>bu kadar gün art arda sakin geçerse kriz gelebilir (eski 60)</summary>
    public const double CALM_DAYS = 60 / Sim.PACE;
    /// <summary>iki kriz arası en az (gün; eski 1,5 yıl)</summary>
    public const double MIN_GAP = 1.5 * Sim.OLD_YEAR;
    /// <summary>gerilim bütçesi yerleşim başına (2 yıllık pencere / yaşayan yerleşim, en az 10): bunun altı sakinlik, PEAK üstü zirve</summary>
    public const double CALM = 1.8, PEAK = 3.2;
    /// <summary>göreli zirve: dünya büyüyüp olgunlaşınca yerleşim başına gerilim PEAK'e pek varmaz; dünyanın kendi olağanının
    /// (BASE_YEARS yıllık üssel ortalama) PEAK_REL katını ve PEAK_FLOOR'u aşan bir dalga da zirve sayılır</summary>
    public const double PEAK_REL = 1.3, PEAK_FLOOR = 2.0, BASE_YEARS = 4;
    /// <summary>ilk krizler bu (dinamik) yıldan sonra (genç dünya önce kendi hikâyesini yazar)</summary>
    public const int FIRST_YEAR = 5;

    /// <summary>Anlatıcının durumu (ilk çağrıda kurulur; zar atmaz).</summary>
    public static StoryState State(Sim s) => s.W.Story ??= new StoryState();

    /// <summary>"Kızıl Ay" sürüyor mu (kamplar daha kalabalık akın eder).</summary>
    public static bool Surging(Sim s) => s.W.Story != null && s.Day < s.W.Story.SurgeUntil;

    /// <summary>Günlük (Sim.Step, kamplardan hemen sonra): gerilim ölçümü, ejderha, kuraklığın bitişi, rahatlama olayı; 10 günde
    /// bir pencere kapanır ve kriz/rahatlama kararı verilir.</summary>
    public static void Tick(Sim s)
    {
        var st = State(s);
        Measure(s, st);
        Dragon.Tick(s);
        DroughtTick(s, st);
        if (st.ReliefEvent > 0 && s.Day >= st.ReliefEvent) { st.ReliefEvent = 0; ReliefEvent(s); }
        if (s.Every(BUCKET)) { Close(st); Decide(s, st); }
    }

    // ------------------------------------------------------------ gerilim
    private static readonly string[] KEYS = { "raids", "extBurned", "plagueDead", "starved", "townFire", "shantyFire", "heroDeath", "conquest", "plunder", "innRuined", "famineMigration", "dragonRaid" };
    private static readonly double[] WEIGHTS = { 1.5, 1.5, 0.5, 1, 2, 1, 2, 6, 2, 3, 2, 4 };

    /// <summary>Günün gerilimi: izlenen sayaçların artışı (ağırlıklı), yeni muharebeler (1 + ölüler × 0,3), kıtlıktaki yerleşimler (0,1/gün).</summary>
    private static void Measure(Sim s, StoryState st)
    {
        var w = s.W;
        double t = 0;
        for (int i = 0; i < KEYS.Length; i++)
        {
            double v = w.Metrics.Get(KEYS[i]) ?? 0, prev = st.Seen.Get(KEYS[i]) ?? 0;
            if (v > prev) t += (v - prev) * WEIGHTS[i];
            st.Seen.Set(KEYS[i], v);
        }
        var bs = w.Battles;
        int k = bs.Count - 1;
        while (k >= 0 && bs[k].Id > st.LastBattle) k--;
        for (int j = k + 1; j < bs.Count; j++) { t += 1 + (bs[j].LossesA + bs[j].LossesB) * 0.3; st.LastBattle = bs[j].Id; }
        foreach (var x in w.Settlements) if (x.Alive && x.Starving > 0) t += 0.1;
        st.Bucket += t;
    }

    private static void Close(StoryState st)
    {
        st.Buckets.Add(st.Bucket);
        st.Bucket = 0;
        if (st.Buckets.Count > WINDOW) st.Buckets.RemoveAt(0);
        double sum = 0;
        foreach (var b in st.Buckets) sum += b;
        st.Tension = sum;
    }

    /// <summary>Yerleşim başına gerilim: 2 yıllık pencere / yaşayan yerleşim sayısı (en az 10). Dünya büyüdükçe olaylar da çoğalır;
    /// bütçe dünyanın büyüklüğüne göre ölçülür.</summary>
    public static double Pressure(Sim s, StoryState st)
    {
        double n = 0;
        foreach (var x in s.W.Settlements) if (x.Alive) n++;
        return st.Tension / JsMath.Max(10, n);
    }

    /// <summary>
    /// 10 günde bir: zirve (yerleşim başına gerilim ≥ <see cref="PEAK"/>, ya da dünyanın olağanının <see cref="PEAK_REL"/> katı ve
    /// en az <see cref="PEAK_FLOOR"/>): rahatlama dönemi başlar (iki zirve arası en az 2 yıl). Sakinlik (&lt; <see cref="CALM"/>)
    /// en az 60 gün sürerse ve son krizden 1,5 yıl geçtiyse her 10 günde %25 olasılıkla kriz gelir. Yaşlanan dünya sakinleştikçe
    /// krizler sıklaşır; krizin getirdiği kan ve yangın bütçeyi yeniden doldurur.
    /// </summary>
    private static void Decide(Sim s, StoryState st)
    {
        if (st.Buckets.Count < WINDOW || s.DynYear < FIRST_YEAR) return;
        double p = Pressure(s, st);
        if (!(st.Base > 0)) st.Base = p;
        double peak = JsMath.Min(PEAK, JsMath.Max(PEAK_FLOOR, PEAK_REL * st.Base));
        st.Base += (p - st.Base) * BUCKET / (BASE_YEARS * Sim.OLD_YEAR);
        if (p >= peak && s.Day >= st.ReliefUntil && s.Day - st.LastPeak >= 2 * Sim.OLD_YEAR) { StartRelief(s, st); return; }
        if (s.Day < st.ReliefUntil) { st.CalmDays = 0; return; }
        st.CalmDays = p < CALM ? st.CalmDays + BUCKET : 0;
        if (st.CalmDays >= CALM_DAYS && s.Day - st.LastCrisis >= MIN_GAP && s.Rng.Chance(0.25)) Crisis(s, st);
    }

    // ------------------------------------------------------------ kriz
    private static readonly List<string> KINDS = new() { "campWave", "raidSurge", "drought", "plague", "trolls", "dragon" };

    /// <summary>Dünyanın durumuna göre kriz: kamp azsa istila, kamp çoksa Kızıl Ay, büyük kent varsa veba, 15. yıldan sonra
    /// troller, ejderha uyanıksa akın; kuraklık en az 6 yılda bir (Faz 1b-3: mevsimlerle birlikte kalkan sert kışın yerine). Bir önceki krizin türü daha az seçilir. Kriz tutmazsa
    /// (yer yok, hedef yok) bir sonraki 10 günde yeniden denenir.</summary>
    private static void Crisis(Sim s, StoryState st)
    {
        var w = s.W;
        var land = J.Filter(w.Camps, c => c.Alive && c.Kind != "pirate" && c.Kind != "dragon");
        int trolls = J.Filter(land, c => c.Kind == "troll").Count;
        var big = BigTown(s);
        bool dragon = Dragon.CanRaid(s);
        double target = Monsters.CampTarget(s);
        int room = Monsters.CampRoom(s);   // Faz 1b-4: istila ve trol çetesi kamp bandının üstüne çıkamaz
        string kind = s.Rng.Weighted(KINDS, k =>
        {
            double v = k switch
            {
                "campWave" => room <= 0 ? 0 : 1 + 0.25 * JsMath.Max(0, target - land.Count),
                "raidSurge" => land.Count >= 3 ? 0.5 + 0.1 * land.Count : 0,
                "drought" => s.DynYear - st.LastDrought >= 6 && st.DroughtUntil <= s.Day ? 0.8 : 0,
                "plague" => big != null ? 0.7 : 0,
                "trolls" => s.DynYear >= 15 && trolls < 3 && room > 0 ? 0.6 + s.DynYear / 50.0 : 0,
                "dragon" => dragon ? 1.4 : 0,
                _ => 0,
            };
            return k == st.LastKind ? v * 0.3 : v;
        });
        if (kind == null) return;
        bool ok = kind switch
        {
            "campWave" => CampWave(s),
            "raidSurge" => RaidSurge(s, st, land),
            "drought" => StartDrought(s, st),
            "plague" => Plague(s, big),
            "trolls" => TrollBand(s),
            "dragon" => DragonRage(s),
            _ => false,
        };
        if (!ok) return;
        st.LastCrisis = s.Day; st.LastKind = kind; st.CalmDays = 0; st.Crises++;
        s.Metric("crisis"); s.Metric("crisis" + Heroes.UpperFirst(kind));
    }

    /// <summary>Krizin vuracağı medeniyet: kalabalık ve kendini güvende sanan (tehdidi düşük) olanlar daha çok.</summary>
    private static Civ PickVictim(Sim s)
    {
        var civs = J.Filter(s.W.Civs, c => c.Alive && s.Capital(c) != null);
        if (civs.Count == 0) return null;
        return s.Rng.Weighted(civs, c => (1 + s.CivPop(c) / 100) / (1 + c.Threat * 2));
    }

    /// <summary>Veba için kent: en kalabalık (en az 50 nüfus), salgınsız yerleşim (eski bağışıklık yeni hastalığı tutmaz).</summary>
    private static Settlement BigTown(Sim s)
    {
        Settlement best = null; double bp = 49;
        foreach (var x in s.W.Settlements) if (x.Alive && x.Plague == null) { double p = s.Pop(x); if (p > bp) { bp = p; best = x; } }
        return best;
    }

    /// <summary>İstila: bir medeniyetin sınırına 2 (30. yıldan sonra 3) yeni in birden (Faz 1b-4: kamp bandının üst sınırına dek); 10. yıldan sonra çoğunlukla hobgoblin.</summary>
    private static bool CampWave(Sim s)
    {
        var civ = PickVictim(s);
        if (civ == null) return false;
        string kind = s.DynYear < 10 ? "goblin" : s.Rng.Chance(0.6) ? "hobgoblin" : "goblin";
        int n = Math.Min(s.DynYear >= 30 ? 3 : 2, Monsters.CampRoom(s));   // Faz 1b-4: kamp bandının üstüne çıkmaz
        var made = new List<Camp>();
        for (int i = 0; i < n; i++)
        {
            var c = Monsters.SpawnLair(s, kind, civ, false, null, 11);
            if (c == null) break;
            c.NextRaid = s.Day + s.Rng.Int(5, 15);   // Faz 1b-5: eski 20–60 gün
            made.Add(c);
        }
        if (made.Count == 0) return false;
        var why = new List<string> { "Uzun sessizlik sınır boylarını gevşetti; dağlardaki kabileler bunu sezdi", kind == "goblin" ? "Kıtlık dağ kabilelerini ovaya indirdi" : "Bir savaş ağası lejyonları tek bayrak altında topladı" };
        if (s.InWar(civ)) why.Add($"{civ.Name} ordusu başka cephelerde; sınır karakolları boş");
        s.Log("crisis", $"İstila! {(kind == "goblin" ? "Goblin kabileleri" : "Hobgoblin lejyonları")} {Tr.Ek(civ.Name, "in")} sınırlarına akın etti: {Lore.JoinVe(J.Map(made, c => c.Name))} kuruldu.", civ: civ.Id, tile: made[0].Tile, major: true,
            cause: s.Rng.Pick(why));
        return true;
    }

    /// <summary>Kızıl Ay: bütün kara kampları ikişer canavarla güçlenir, bir ay içinde akına çıkar; 60 gün boyunca akınlar kalabalıktır.</summary>
    private static bool RaidSurge(Sim s, StoryState st, List<Camp> land)
    {
        if (land.Count < 3) return false;
        foreach (var c in land)
        {
            c.Count = JsMath.Min(Monsters.Cap(s, c.Kind), c.Count + 2);
            c.NextRaid = JsMath.Min(c.NextRaid, s.Day + s.Rng.Int(1, 8));   // Faz 1b-5: eski 3–30 gün
        }
        st.SurgeUntil = s.Day + 60 / Sim.PACE;
        s.Log("crisis", "Kızıl Ay doğdu! İnlerde canavarlar kudurdu; akın davulları gece boyu çalıyor.", tile: land[0].Tile, major: true,
            cause: $"{J.S(land.Count)} in aynı anda kan kokusu aldı");
        return true;
    }

    private static readonly List<string> DROUGHT_WHY = new() { "Yağmurlar aylardır yağmadı; dereler kurudu", "Güneyden esen kavurucu rüzgâr ekinleri yaktı", "Nemli ambarlarda kara pas başladı, tarlalara da sıçradı" };
    private static readonly List<string> PLAGUE_WHY = new() { "Uzak diyarlardan gelen bir kervan hastalığı taşıdı; kalabalık sokaklarda hızla yayılıyor", "Kirli kuyular ve tıklım tıklım pazarlar hastalığı büyüttü", "Ambarlardan sokaklara taşan fareler hastalığı yaydı" };
    private static readonly List<string> TROLL_WHY = new() { "Troller yaralarını kapatır; onları ancak ateş durdurur", "Dağ geçitlerindeki av tükenince troller ovaya indi", "Yaşlı bir trol anası yavrularını yeni av yerlerine saldı" };

    /// <summary>Kuraklık (Faz 1b-3: sert kışın yerine, mevsimsiz): 15–23 gün (Faz 1b-5; eski 60–90) boyunca tarla ve toplayıcı verimi yarıya iner
    /// (Economy.DROUGHT_FARM), kasaba yangınları sıklaşır; ambarlardaki tahılın %20–40'ını kara pas çürütür. Kıtlık ve açlık
    /// olursa Economy'nin kıtlık düzeninden gelir.</summary>
    private static bool StartDrought(Sim s, StoryState st)
    {
        if (s.DynYear - st.LastDrought < 6 || st.DroughtUntil > s.Day) return false;
        double days = s.Rng.Int(15, 23);
        double share = 0.2 + Math.Floor(s.Rng.Next() * 5) * 0.05;
        st.DroughtUntil = s.Day + days; st.LastDrought = s.DynYear;
        double lost = 0;
        foreach (var c in s.W.Civs)
        {
            if (!c.Alive) continue;
            double q = Math.Floor(s.St(c, "grain") * share);
            if (q > 0) { s.Add(c, "grain", -q); lost += q; }
        }
        s.Metric("drought"); s.Metric("droughtGrain", lost);
        s.Log("crisis", $"Kuraklık! Yağmurlar kesildi, tarlalar çatladı; ambarlardaki tahılın %{J.S(JsMath.Round(share * 100))} kadarını kara pas sardı.", major: true,
            cause: $"{s.Rng.Pick(DROUGHT_WHY)}; hasat {J.S(days)} gün boyunca yarıya düşecek");
        return true;
    }

    /// <summary>Kuraklığın bittiği gün kroniğe düşer.</summary>
    private static void DroughtTick(Sim s, StoryState st)
    {
        if (st.DroughtUntil <= 0 || s.Day != st.DroughtUntil) return;
        s.Log("world", "Yağmurlar geri döndü; kuraklık sona erdi.", cause: "Toprak yeniden yeşeriyor");
    }

    /// <summary>Kara veba: en kalabalık kentte ağır bir salgın (salgının yayılması ve ölümleri Events.DisastersTick'te).</summary>
    private static bool Plague(Sim s, Settlement big)
    {
        if (big == null) return false;
        double until = s.Day + s.Rng.Int(8, 10);   // Faz 1b-5 (v3: salgın 5–10 gün; eski 90–160)
        double sev = 1.2 + s.Rng.Next() * 0.6;
        big.Plague = new PlagueInfo { Since = s.Day, Until = until, Severity = sev, Dead = 0 };
        s.Metric("plague");
        s.Log("crisis", $"Kara veba! {Tr.Ek(big.Name, "da")} ölümcül bir salgın başladı.", civ: big.Civ, tile: big.Tile, major: true,
            cause: s.Rng.Pick(PLAGUE_WHY));
        return true;
    }

    /// <summary>Trol çetesi: bir medeniyetin sınırına yakın dağ eteğine yerleşir, kısa sürede akına çıkar (Faz 1b-4: kamp bandı doluysa gelmez).</summary>
    private static bool TrollBand(Sim s)
    {
        if (Monsters.CampRoom(s) <= 0) return false;
        var civ = PickVictim(s);
        var c = Monsters.SpawnLair(s, "troll", civ, false, null);
        if (c == null) return false;
        c.NextRaid = s.Day + s.Rng.Int(3, 8);   // Faz 1b-5: eski 10–30 gün
        s.Log("crisis", $"Trol çetesi! Dağlardan inen troller {Lore.Ek(c.Name, "a")} yerleşti; {(civ != null ? $"{Tr.Ek(civ.Name, "in")} köyleri" : "sınır köyleri")} tehlikede.", civ: civ?.Id, tile: c.Tile, major: true,
            cause: s.Rng.Pick(TROLL_WHY));
        return true;
    }

    /// <summary>Ejderhanın öfkesi: ejderha hemen akına çıkar.</summary>
    private static bool DragonRage(Sim s)
    {
        var d = s.W.Dragon; var cp = Dragon.CampOf(s);
        if (d == null || cp == null || !Dragon.CanRaid(s)) return false;
        s.Log("crisis", $"Gökyüzü kızıla boyandı: ejderha {d.Name} öfkeyle inini terk etti.", tile: cp.Tile, major: true, cause: "Uzun uykusu ejderhayı acıktırdı");
        return Dragon.Raid(s, d, cp, null, "Ejderhanın öfkesi dinmiyor");
    }

    // ------------------------------------------------------------ rahatlama
    /// <summary>Rahatlama dönemi (30–45 gün; eski 1–1,5 yıl): yeni kriz yok; 8–23 gün içinde bereketli bir hasat ya da şenlik.</summary>
    public static void StartRelief(Sim s, StoryState st)
    {
        st.ReliefUntil = s.Day + s.Rng.Int(30, 45);
        st.ReliefEvent = s.Day + s.Rng.Int(8, 23);
        st.LastPeak = s.Day; st.CalmDays = 0;
        s.Metric("relief");
    }

    /// <summary>Bereket yılı (bütün medeniyetlere ~15 günlük tahıl, tehdit azalır) ya da şenlik (acı çekmiş bir medeniyet komşularını
    /// sofraya çağırır: ilişkiler iyileşir, tehdit yarıya iner).</summary>
    private static void ReliefEvent(Sim s)
    {
        var civs = J.Filter(s.W.Civs, c => c.Alive && s.Capital(c) != null);
        if (civs.Count == 0) return;
        if (s.Rng.Chance(0.5))
        {
            foreach (var c in civs) { s.Add(c, "grain", Math.Floor(s.CivPop(c) * 1.5)); c.Threat *= 0.7; }
            s.Metric("reliefHarvest");
            s.Log("relief", "Bereket yılı: zor günlerin ardından tarlalar iki kat verdi, ambarlar taştı.", major: true, cause: "Yağmurlar zamanında geldi; halk derin bir nefes aldı");
            return;
        }
        var host = s.Rng.Weighted(civs, c => 1 + c.Threat + s.CivPop(c) / 200);
        var cap = s.Capital(host);
        double cost = Math.Floor(JsMath.Min(s.St(host, "gold") * 0.05, 300));
        if (cost > 0) s.Add(host, "gold", -cost);
        foreach (var o in civs) if (o.Id != host.Id && s.Rel(host.Id, o.Id).Contact && !s.AtWar(host.Id, o.Id)) s.AddMod(host.Id, o.Id, "feast", "Şenlik dostluğu", 6, 12, 0.02);
        host.Threat *= 0.5;
        s.Metric("reliefFeast");
        s.Log("relief", $"{Tr.Ek(cap.Name, "da")} zor yılların bitişi şenlikle kutlandı; komşu halklar da sofraya davet edildi.", civ: host.Id, tile: cap.Tile, major: true,
            cause: $"{host.Name} yaralarını sarıyor");
    }
}
