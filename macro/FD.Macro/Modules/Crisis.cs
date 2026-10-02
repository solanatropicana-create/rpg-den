using System;
using System.Collections.Generic;

// Faz 1b-7: büyük şehrin istikrarı ve hükümet tipine göre iç çöküş yolları (yol haritası v3 "Büyük şehrin istikrarı";
// claude/devlet-orgut-spec.md §2 "Çöküş yolları"). İstikrar (0–100) garnizondan, açlıktan, vergi yükünden (savaş, boş hazine),
// yöneticinin meşruiyetinden, savaş yorgunluğundan ve yerleşimin durumundan beslenir. Büyük şehirde ve taht şehrinde durum zarı
// istikrar düştükçe iç krize döner: kriz 5–10 gün belirti verir (söylenti, mülteci, fiyatlar, sokaktaki asker) ve sonunda ya
// bastırılır ya da şehir içeriden düşer. Düşüşün yolu tipe göre:
//   krallık: veraset kavgası (varissiz ya da çocuk kral; taht başka hanedana, kimi zaman evlilik ittifakıyla komşu krala geçer, iç
//            savaşta bir şehir ayrılır) ve soylu isyanı (taşrada şehir ayrılır, başkentte kral devrilir);
//   boylar:   meydan okuma (reis düelloda ölür) ve boyların ayrılması (taşra şehri kendi boyunu kurar);
//   cumhuriyet: darbe (konsey devrilir, kimi zaman krallığa döner) ve iflas (hazine boşken paralı askerler şehri alır);
//   teokrasi: mezhep bölünmesi (ılımlılar başkenti alır ya da taşra şehri ayrılır); aforoz (States.ExposePact) ve Kutsal Sefer
//            yenilgisi (meşruiyet) de bu yola besler.
// Taht şehrinin düşüşü devleti yıkmaz, yönetimi değiştirir (yeni yönetici, düşük meşruiyet, kimi zaman yeni tip); taşradaki büyük
// şehrin düşüşü yeni bir devlet doğurur ya da şehir komşuya geçer. Her ikisi de büyük şehrin el değiştirmesidir (ölçüt 6f).

namespace FD.Macro;

public static class Crisis
{
    /// <summary>kriz durumları (belirti 5–10 gün)</summary>
    public static readonly Dictionary<string, StatusDef> DEFS = new()
    {
        ["feud"] = new StatusDef { Id = "feud", Name = "Veraset kavgası", Kind = "crisis", Min = 5, Max = 10, Growth = 0.5, Prod = 0.85, Stab = -10, Push = 0.05 },
        ["revolt"] = new StatusDef { Id = "revolt", Name = "Soylu isyanı", Kind = "crisis", Min = 5, Max = 10, Growth = 0.5, Prod = 0.8, Stab = -12, Push = 0.06 },
        ["separatism"] = new StatusDef { Id = "separatism", Name = "Ayrılık", Kind = "crisis", Min = 5, Max = 10, Growth = 0.6, Prod = 0.85, Stab = -10, Push = 0.05 },
        ["coup"] = new StatusDef { Id = "coup", Name = "Darbe söylentisi", Kind = "crisis", Min = 5, Max = 10, Growth = 0.7, Prod = 0.9, Stab = -10, Push = 0.04 },
        ["mutiny"] = new StatusDef { Id = "mutiny", Name = "Paralı askerler huzursuz", Kind = "crisis", Min = 5, Max = 10, Growth = 0.6, Prod = 0.85, Stab = -12, Push = 0.06 },
        ["schism"] = new StatusDef { Id = "schism", Name = "Mezhep çatışması", Kind = "crisis", Min = 5, Max = 10, Growth = 0.6, Prod = 0.85, Stab = -10, Push = 0.05 },
        ["challenge"] = new StatusDef { Id = "challenge", Name = "Meydan okuma", Kind = "crisis", Min = 5, Max = 10, Growth = 0.8, Prod = 0.9, Stab = -8, Push = 0.03 },
    };

    public static StatusDef Def(string id) => id != null && DEFS.TryGetValue(id, out var d) ? d : null;

    /// <summary>istikrarın dinlendiği değer (her şey yolundayken)</summary>
    public const double STAB_BASE = 62;
    /// <summary>zar başına kriz olasılığı: CR_BASE + (CR_S0 − istikrar) × CR_K (CR_MIN–CR_MAX)</summary>
    public const double CR_BASE = 0.06, CR_S0 = 70, CR_K = 0.009, CR_MIN = 0.015, CR_MAX = 0.6;
    /// <summary>krizin sonunda düşüş olasılığı: FALL_BASE + (FALL_S0 − istikrar) × FALL_K (FALL_MIN–FALL_MAX); güçlü garnizon × FALL_GUARD</summary>
    public const double FALL_BASE = 0.3, FALL_S0 = 60, FALL_K = 0.015, FALL_MIN = 0.08, FALL_MAX = 0.9, FALL_GUARD = 0.75;
    /// <summary>iki kriz arası en az (gün; krizin başından)</summary>
    public const double CR_COOLDOWN = 25;
    /// <summary>taşra şehri ayrılıp yeni devlet kurarken yaşayan devlet sayısının üst sınırı (üstünde komşuya geçer)</summary>
    public const int CR_MAX_STATES = 6;

    // ------------------------------------------------------------ istikrar
    /// <summary>
    /// İstikrar 0–100 (yol haritası v3): STAB_BASE ve garnizon (asker / nüfus), açlık (aç şehir, kıtlık ilanı, ekmek ya da bira yokluğu),
    /// vergi yükü (savaş vergisi, boş hazine), meşruiyet ((meşruiyet − 60) × 0,45), savaş yorgunluğu (uzayan savaş, art arda hücum),
    /// salgın, zorla alınmışlık, yabancı çoğunluk, resmî inanca soğukluk, imar, kanun, taht (saray muhafızları), krallıkta varissiz yaşlı
    /// ya da çocuk kral, Pakt'a bağlı yönetici söylentisi, örgüt şubeleri (Hırsızlar −, teokraside Kilise +) ve durumun katkısı.
    /// Nedenler <paramref name="why"/>'a.
    /// </summary>
    public static double Stability(Sim s, Settlement st, List<string> why = null)
    {
        var c = s.W.Civs[st.Civ];
        double P = JsMath.Max(1, s.Pop(st));
        double S = STAB_BASE;
        void Add(double v, string w) { S += v; if (w != null && v < 0) why?.Add(w); }
        double g = st.Soldiers / P;
        if (g >= 0.1) Add(5, null); else if (g < Diplomacy.GARRISON_THIN) Add(-12, "garnizon eridi"); else if (g < Diplomacy.GARRISON_LOW) Add(-5, "garnizon zayıf");
        if (st.Starving > 0) Add(-18, "şehir aç"); else if (c.Famine != null && c.Famine.Declared) Add(-8, "kıtlık ilan edilmiş");
        if (Economy.LackCount(st, Economy.LACK_UNREST, true) > 0) Add(-6, "pazarda ekmek ya da bira yok");
        if (s.InWar(c)) Add(-4, "savaş vergisi");
        if (c.Broke != null) Add(-12, "hazine boş, maaşlar ödenmiyor");
        Add((c.Legit - 60) * 0.45, c.Legit < 45 ? $"yönetimin meşruiyeti düşük ({J.S(JsMath.Round(c.Legit))})" : null);
        if (Diplomacy.LongestWar(s, c) > Diplomacy.LONG_WAR) Add(-8, "savaş bitmek bilmiyor");
        double wear = Diplomacy.Wear(s, st);
        if (wear > 0) Add(-6 * wear, "art arda hücumlar halkı yıprattı");
        if (st.Plague != null) Add(-8, "salgın kol geziyor");
        if (st.Founder != null && st.Founder != st.Civ && st.LostDay != null && s.Day - st.LostDay.Value < 10 * Sim.OLD_YEAR) Add(-8, "şehir zorla alınmıştı");
        string maj = Diplomacy.MajorityRace(st);
        if (maj != null && maj != c.Race) Add(-5 * (1 - JsMath.Min(1, c.Law?.RaceTol.Get(maj) ?? 1) * 0.5), $"halkının çoğu {J.TrLower(D.RACES[maj].Plural)}");
        if (c.Law?.Faith != null && st.Faith != null && (st.Faith.Get(c.Law.Faith) ?? 0) < 0.4) Add(-6, "halk resmî inanca soğuk");
        Add(Economy.ImarCalm(st) * 12, null);
        Add(c.Align.Law * 4, c.Align.Law < -0.5 ? "kanunsuzluk" : null);
        bool seat = Diplomacy.IsSeat(s, c, st);
        if (seat) Add(4, null);
        var r = States.Ruler(s, c);
        if (c.Gov == "kingdom" && r != null && seat)
        {
            var a = D.HERO_AGE.GetOr(r.Race, null) ?? D.HERO_AGE["human"];
            double age = States.Age(s, r);
            var heir = States.PersonById(s, c.Heir);
            if (age >= a[2] && (heir == null || heir.Died != null)) Add(-7, "yaşlı kralın varisi yok");
            else if (age < a[0]) Add(-6, "tahtta bir çocuk var");
        }
        if (States.PactBound(r)) Add(-3, "yöneticinin karanlık bağları fısıldanıyor");
        int thieves = Orgs.BranchLevel(s, st, "thieves");
        if (thieves > 0) Add(-1.5 * thieves, "sokaklarda Hırsızlar Loncası");
        if (c.Gov == "theocracy") Add(2 * Orgs.BranchLevel(s, st, "church"), null);
        var d = Status.Def(st.Status);
        if (d != null) Add(d.Stab, d.Stab < 0 ? J.TrLower(d.Name) : null);
        var k = Def(st.Crisis);
        if (k != null) Add(k.Stab, null);
        return JsMath.Max(0, JsMath.Min(100, S));
    }

    /// <summary>Her WORLD_DAYS günde: yerleşimlerin istikrarı.</summary>
    public static void StabilityTick(Sim s)
    {
        foreach (var st in s.W.Settlements)
        {
            if (!st.Alive || st.Hub != null || st.Civ < 0) continue;
            st.Stability = JsMath.Round(Stability(s, st) * 10) / 10;
        }
    }

    // ------------------------------------------------------------ kriz
    /// <summary>İç krize açık yerleşim: büyük şehir ya da Köy ve üstü taht şehri.</summary>
    public static bool Eligible(Sim s, Settlement st)
    {
        if (!st.Alive || st.Hub != null || st.Civ < 0) return false;
        var c = s.W.Civs[st.Civ];
        if (!c.Alive) return false;
        return Sim.IsBig(st) || (st.Tier >= 1 && Diplomacy.IsSeat(s, c, st));
    }

    /// <summary>Durum zarında (Status.Roll): uygun yerleşimde istikrara göre iç kriz başlar mı.</summary>
    public static bool Consider(Sim s, Settlement st)
    {
        if (st.Crisis != null || !Eligible(s, st)) return false;
        if (st.CrisisSince != null && s.Day - st.CrisisSince.Value < CR_COOLDOWN) return false;
        double S = st.Stability ?? Stability(s, st);
        double p = JsMath.Max(CR_MIN, JsMath.Min(CR_MAX, CR_BASE + (CR_S0 - S) * CR_K));
        if (!s.Rng.Chance(p)) return false;
        Start(s, st, null, null, null, 0);
        return st.Crisis != null;
    }

    /// <summary>Tipin ve yerin krizi: krallıkta varissiz/çocuk kral ya da düşük meşruiyetle veraset kavgası, yoksa soylu isyanı; boylarda
    /// taht şehrinde meydan okuma, taşrada ayrılık; cumhuriyette boş hazineyle paralı askerler, taht şehrinde darbe, taşrada ayrılık;
    /// teokraside mezhep çatışması.</summary>
    public static string Choose(Sim s, Civ c, Settlement st)
    {
        bool seat = Diplomacy.IsSeat(s, c, st);
        switch (c.Gov)
        {
            case "kingdom":
            {
                if (!seat) return "revolt";
                var r = States.Ruler(s, c);
                var heir = States.PersonById(s, c.Heir);
                var a = r != null ? D.HERO_AGE.GetOr(r.Race, null) ?? D.HERO_AGE["human"] : null;
                bool weakLine = r == null || ((heir == null || heir.Died != null) && States.Age(s, r) >= a[2] * 0.85) || States.Age(s, r) < a[0];
                return weakLine || c.Legit < 40 ? "feud" : "revolt";
            }
            case "clans": return seat ? "challenge" : "separatism";
            case "republic": return c.Broke != null && s.Day - c.Broke.Value >= 3 ? "mutiny" : seat ? "coup" : "separatism";
            default: return "schism";
        }
    }

    /// <summary>Kriz başlar (belirti 5–10 gün): söylenti (akış), mülteci (göç), fiyatlar (tahıl), sokaktaki asker (garnizon).
    /// <paramref name="kind"/> null ise tipin krizi; <paramref name="bonus"/> sonundaki düşüş olasılığına eklenir (darbeyi bir örgüt
    /// besliyor, taht varissiz kaldı…).</summary>
    public static void Start(Sim s, Settlement st, string kind, string cause, Org org, double bonus)
    {
        if (st == null || !st.Alive || st.Crisis != null || st.Hub != null) return;
        var c = s.W.Civs[st.Civ];
        kind ??= Choose(s, c, st);
        var d = DEFS[kind];
        st.Crisis = kind; st.CrisisSince = s.Day; st.CrisisUntil = s.Day + s.Rng.Int(d.Min, d.Max);
        st.CrisisOrg = org?.Id;
        st.CrisisBonus = bonus != 0 ? bonus : null;
        st.CrisisCiv = st.Civ;
        s.Metric("crisis"); s.Metric("crisis_" + kind);
        if (Sim.IsBig(st)) s.Metric("crisisBig");
        var why = new List<string>();
        double S = Stability(s, st, why);
        st.Stability = JsMath.Round(S * 10) / 10;
        string ruler = States.RulerTitle(s, c);
        string text = kind switch
        {
            "feud" => $"{Tr.Ek(st.Name, "da")} veraset kavgası: soylu hanedanlar {Tr.Ek(c.Name, "in")} tahtı için silahlanıyor.",
            "revolt" => Diplomacy.IsSeat(s, c, st) ? $"{Tr.Ek(st.Name, "da")} soylular {Tr.Ek(ruler, "a")} karşı birleşiyor; saray kapılarında asker var." : $"{Tr.Ek(st.Name, "in")} lordu {Tr.Ek(c.Name, "a")} baş kaldırmaya hazırlanıyor.",
            "separatism" => $"{Tr.Ek(st.Name, "da")} ayrılık sesleri yükseliyor: {Tr.Ek(c.Name, "dan")} kopmak isteyenler meydanlarda.",
            "coup" => $"{Tr.Ek(st.Name, "da")} darbe söylentileri: subaylar ve lonca başları gece toplantılarında.",
            "mutiny" => $"{Tr.Ek(st.Name, "da")} maaşı ödenmeyen paralı askerler homurdanıyor; sokaklarda devriye yerine yağmacı var.",
            "schism" => $"{Tr.Ek(st.Name, "da")} mezhep çatışması: ılımlı rahipler Başkatedral'e kafa tutuyor.",
            _ => $"{Tr.Ek(st.Name, "da")} boy beyleri {Tr.Ek(ruler, "a")} meydan okuyor.",
        };
        string because = cause ?? (why.Count > 0 ? J.TrCap(string.Join(", ", why.Count > 4 ? why.GetRange(0, 4) : why)) + $" (istikrar {J.S(JsMath.Round(S))})" : $"İstikrar {J.S(JsMath.Round(S))}");
        if (org != null) because += $"; {org.Name} perde arkasında";
        s.Log("politics", text, civ: c.Id, tile: st.Tile, cause: because, major: Sim.IsBig(st) || Diplomacy.IsSeat(s, c, st));
        // belirtiler: mülteci, fiyat, sokaktaki asker
        double P = s.Pop(st);
        double flee = Math.Round(P * d.Push * (0.6 + s.Rng.Next() * 0.8));
        if (Sim.IsBig(st)) flee = JsMath.Min(flee, JsMath.Max(0, P - Status.BIG_KEEP));
        if (flee >= 1) Status.Emigrate(s, st, flee, d.Name);
        c.Price.Set("grain", s.Price(c, "grain") * 1.1);
        if (kind != "mutiny") st.Soldiers += Math.Floor(P * 0.02);
    }

    /// <summary>Her gün: süresi dolan krizler çözülür.</summary>
    public static void Tick(Sim s)
    {
        var w = s.W;
        for (int i = 0; i < w.Settlements.Count; i++)
        {
            var st = w.Settlements[i];
            if (st.Crisis == null) continue;
            if (!st.Alive || st.Civ < 0 || !w.Civs[st.Civ].Alive || st.Civ != st.CrisisCiv) { Clear(st); continue; }   // şehir el değiştirdi: kriz söndü
            if (s.Day >= (st.CrisisUntil ?? 0)) Resolve(s, st);
        }
    }

    private static void Clear(Settlement st)
    {
        st.Crisis = null; st.CrisisUntil = null; st.CrisisOrg = null; st.CrisisBonus = null; st.CrisisCiv = null;
    }

    /// <summary>Krizin sonu: düşüş olasılığı istikrardan (o günkü), güçlü garnizon bastırır. Düşerse tipin yolu, yoksa kriz bastırılır.</summary>
    private static void Resolve(Sim s, Settlement st)
    {
        var c = s.W.Civs[st.Civ];
        string kind = st.Crisis;
        var org = st.CrisisOrg != null ? Orgs.ById(s, st.CrisisOrg.Value) : null;
        double lead = s.Day - (st.CrisisSince ?? s.Day);
        double bonus = st.CrisisBonus ?? 0;
        var why = new List<string>();
        double S = Stability(s, st, why);
        double p = FALL_BASE + (FALL_S0 - S) * FALL_K + bonus;
        if (st.Soldiers / JsMath.Max(1, s.Pop(st)) >= 0.1) p *= FALL_GUARD;
        p = JsMath.Max(FALL_MIN, JsMath.Min(FALL_MAX, p));
        Clear(st);
        if (!s.Rng.Chance(p)) { Crushed(s, c, st, kind); return; }
        Fall(s, c, st, kind, lead, org, why, S);
    }

    /// <summary>Kriz bastırıldı: meşruiyet artar (yönetim sınavı geçti), birkaç asi ölür.</summary>
    private static void Crushed(Sim s, Civ c, Settlement st, string kind)
    {
        s.Metric("crisisCrushed");
        c.Legit = JsMath.Min(100, c.Legit + (kind == "challenge" ? 12 : 5));
        if (kind == "challenge") s.Metric("chiefDuel");
        double dead = JsMath.Min(3, Math.Floor(s.Pop(st) * 0.02));
        if (dead > 0 && !(Sim.IsCore(st) && s.Pop(st) <= Sim.CORE_MIN + dead)) s.RemovePop(st, dead);
        string ruler = States.RulerTitle(s, c);
        string text = kind switch
        {
            "feud" => $"{Tr.Ek(c.Name, "in")} veraset kavgası yatıştı: {ruler} tahtını korudu.",
            "challenge" => $"{ruler} kendisine meydan okuyan boy beyini düelloda yendi.",
            "coup" => $"{Tr.Ek(st.Name, "da")} darbe girişimi bastırıldı; {ruler} komplocuları astırdı.",
            "mutiny" => $"{Tr.Ek(st.Name, "da")} paralı askerlere son anda ödeme yapıldı; isyan dağıldı.",
            "schism" => $"{Tr.Ek(st.Name, "da")} mezhep çatışması yatıştı; ılımlılar susturuldu.",
            "separatism" => $"{Tr.Ek(st.Name, "da")} ayrılıkçılar dağıtıldı.",
            _ => $"{Tr.Ek(st.Name, "da")} soylu isyanı bastırıldı.",
        };
        if (kind == "mutiny") s.Add(c, "gold", -JsMath.Min(s.St(c, "gold"), 20));
        s.Log("politics", text, civ: c.Id, tile: st.Tile, cause: "Kriz bastırıldı", major: false);
    }

    /// <summary>Şehir içeriden düştü. Taht şehri: yönetim değişir (States.Usurp; krallıkta veraset savaşında taşradan bir şehir de ayrılabilir,
    /// küçük krallık evlilik ittifakıyla komşuya katılabilir). Taşra şehri: yeni devlet (Diplomacy.SecedeAs) ya da komşuya geçer
    /// (Diplomacy.Defect).</summary>
    private static void Fall(Sim s, Civ c, Settlement st, string kind, double lead, Org org, List<string> why, double S)
    {
        var w = s.W;
        bool seat = Diplomacy.IsSeat(s, c, st);
        string path = Path(c.Gov, kind, seat);
        s.Metric("crisisFall");
        if (Sim.IsBig(st)) s.Metric("crisisFallBig");
        string cause = (why.Count > 0 ? J.TrCap(string.Join(", ", why.Count > 4 ? why.GetRange(0, 4) : why)) + $" (istikrar {J.S(JsMath.Round(S))})" : $"İstikrar {J.S(JsMath.Round(S))}")
            + (org != null ? $"; {org.Name} perde arkasında" : "");
        if (seat)
        {
            // krallık: küçük krallık varissiz kalınca evlilik ittifakıyla komşu krala geçer (devletler birleşir)
            if (kind == "feud" && Union(s, c, st, lead, cause)) return;
            // tip nadiren değişir, her yöne: darbe ve paralı askerler kimi zaman krallık kurar, mezhep bölünmesi tahtı ılımlı bir
            // krallığa bırakabilir; soylular kimi zaman tüccar konseyine, veraset savaşında Güneş Tarikatı tahta el koyabilir
            string newGov = kind switch
            {
                "coup" => s.Rng.Chance(0.15) ? "kingdom" : null,
                "mutiny" => s.Rng.Chance(0.25) ? "kingdom" : null,
                "schism" => s.Rng.Chance(0.15) ? "kingdom" : null,
                "revolt" => Orgs.BranchLevel(s, st, "merchants") >= 2 && s.Rng.Chance(0.15) ? "republic" : null,
                "feud" => Orgs.BranchLevel(s, st, "order") >= 1 && s.Rng.Chance(0.3) ? "theocracy" : null,
                _ => null,
            };
            States.Usurp(s, c, st, path, kind, newGov, lead, cause);
            if (org != null) { org.Gold += 40; org.Tally.Set("coup", (org.Tally.Get("coup") ?? 0) + 1); }
            // veraset savaşı: kaybeden hanedan taşradaki en büyük şehri alıp ayrılır
            if (kind == "feud" && s.Rng.Chance(0.35))
            {
                Settlement best = null;
                foreach (var x in s.CivSettlements(c)) if (x.Id != st.Id && x.Tier >= 1 && s.Pop(x) >= Diplomacy.SECEDE_POP && (best == null || s.Pop(x) > s.Pop(best))) best = x;
                if (best != null && CanFoundState(s)) Diplomacy.SecedeAs(s, c, best, "kingdom", "succession", new List<string> { "veraset savaşını kaybeden hanedan taşraya çekildi" });
            }
            return;
        }
        // taşra şehri
        string gov = kind switch
        {
            "revolt" => s.Rng.Chance(0.6) ? "kingdom" : null,   // null: ayrılan halkın yatkınlığı (Diplomacy.SecessionGov)
            "mutiny" => s.Rng.Chance(0.5) ? "kingdom" : "republic",
            "schism" => s.Rng.Chance(0.6) ? "theocracy" : "kingdom",
            _ => c.Gov,
        };
        var reasons = new List<string>(why);
        reasons.Insert(0, kind switch
        {
            "revolt" => "soylu isyanı",
            "separatism" => c.Gov == "clans" ? "boyların ayrılması" : "ayrılıkçılar şehri aldı",
            "mutiny" => "paralı askerler şehri ele geçirdi",
            "schism" => "mezhep bölünmesi",
            _ => Def(kind).Name,
        });
        if (CanFoundState(s)) { Diplomacy.SecedeAs(s, c, st, gov, path, reasons); MarkRegime(s, st, path, lead); return; }
        var to = Diplomacy.DefectTarget(s, c, st);
        if (to != null) { Diplomacy.Defect(s, c, st, to, path, cause); MarkRegime(s, st, path, lead); return; }
        Crushed(s, c, st, kind);
    }

    /// <summary>Yeni devlet kurulabilir mi (yaşayan devlet CR_MAX_STATES'ten az, devlet yuvası var).</summary>
    public static bool CanFoundState(Sim s)
    {
        int alive = 0;
        foreach (var x in s.W.Civs) if (x.Alive) alive++;
        return alive < CR_MAX_STATES && s.W.Civs.Count < Diplomacy.MAX_CIV_SLOTS;
    }

    /// <summary>Çöküş yolu (ölçüt 9c): krallık succession / nobles, boylar duel / clansplit, cumhuriyet coup / mutiny / secession, teokrasi schism.</summary>
    public static string Path(string gov, string kind, bool seat) => kind switch
    {
        "feud" => "succession",
        "revolt" => "nobles",
        "challenge" => "duel",
        "coup" => "coup",
        "mutiny" => "mutiny",
        "schism" => "schism",
        "separatism" => gov == "clans" ? "clansplit" : "secession",
        _ => kind,
    };

    /// <summary>Yerleşimin içeriden düşüşü kaydedilir (ölçüm: büyük şehrin el değiştirmesi).</summary>
    public static void MarkRegime(Sim s, Settlement st, string path, double lead)
    {
        st.Regimes = (st.Regimes ?? 0) + 1;
        st.RegimeHow = path;
        st.RegimeLead = lead;
        s.Metric("regime"); s.Metric("regime_" + path);
        if (Sim.IsBig(st)) s.Metric("regimeBig");
    }

    /// <summary>Krallıkta veraset kavgası ve küçük devlet: evlilik ittifakıyla en iyi ilişkideki komşu krallığa katılır (devletler birleşir).
    /// Yaşayan devlet 5'ten azsa ya da uygun komşu yoksa olmaz.</summary>
    private static bool Union(Sim s, Civ c, Settlement st, double lead, string cause)
    {
        int alive = 0;
        foreach (var x in s.W.Civs) if (x.Alive) alive++;
        if (alive <= 5 || s.CivSettlements(c).Count > 4 || !s.Rng.Chance(0.5)) return false;
        Civ best = null; double bv = 20;
        foreach (var o in s.W.Civs)
        {
            if (!o.Alive || o.Id == c.Id || o.Gov != "kingdom" || s.AtWar(o.Id, c.Id) || !s.Rel(o.Id, c.Id).Contact) continue;
            double v = s.RelValue(c.Id, o.Id) + (s.Rel(c.Id, o.Id).Pact != null ? 30 : 0) + (o.Race == c.Race ? 10 : 0);
            if (v > bv) { bv = v; best = o; }
        }
        if (best == null) return false;
        MarkRegime(s, st, "succession", lead);
        s.Metric("collapse_succession_" + c.Gov);
        States.Merge(s, c, best, $"Varissiz kalan {Tr.Ek(c.Name, "in")} tahtı evlilik ittifakıyla {Tr.Ek(States.RulerTitle(s, best), "a")} geçti", cause);
        return true;
    }
}
