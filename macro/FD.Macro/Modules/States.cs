using System;
using System.Collections.Generic;

// Faz 1b-6: devletin yaşamı (claude/devlet-orgut-spec.md §2–3): yönetici ve halef (tipine göre: veraset, düello, konsey oyu,
// tarikat seçimi), meşruiyet, hizalama (yönetici + yasa), yerleşimlerin inanç dağılımı ve tipin yıllık olayı (teokrasinin
// Güneş Bayramı; eski Rahip bayramı). Çöküş yolları (veraset krizinin iç savaşa dönmesi, darbe, mezhep bölünmesi…) Faz 1b/7'de;
// burada yalnız halef seçimi ve meşruiyetin düşüşü var.

namespace FD.Macro;

public static class States
{
    /// <summary>meşruiyetin dinlendiği değer ve her tikte (WORLD_DAYS) ona yaklaşma payı</summary>
    public const double LEGIT_BASE = 60, LEGIT_PULL = 0.05;
    /// <summary>cumhuriyette seçim aralığı (gün; 3 yıl)</summary>
    public const double ELECTION_DAYS = 3 * Sim.YEAR;
    /// <summary>yerleşimin inancı her tikte hedefe bu payla yaklaşır (WORLD_DAYS ≈ 7,5 gün; yarı ömür ~100 gün)</summary>
    public const double FAITH_PULL = 0.05;

    // ------------------------------------------------------------ kişiler
    public static Person PersonById(Sim s, int? id)
    {
        if (id == null) return null;
        foreach (var p in s.W.People) if (p.Id == id.Value) return p;
        return null;
    }

    public static Person Ruler(Sim s, Civ c) => PersonById(s, c.Ruler);

    /// <summary>Yaş (yıl; 40 günlük yıl).</summary>
    public static double Age(Sim s, Person p) => (s.Day - p.Born) / Sim.YEAR;

    /// <summary>yaklaşık normal dağılımlı sapma (üç tekdüze toplamı; sd ≈ sigma)</summary>
    private static double Noise(Sim s, double sigma) => (s.Rng.Next() + s.Rng.Next() + s.Rng.Next() - 1.5) * 2 * sigma;

    private static double Clamp(double v, double lo = -1, double hi = 1) => JsMath.Max(lo, JsMath.Min(hi, v));

    /// <summary>Yeni kişi: ırkın ad havuzundan; yaş (yıl) verilir, doğum günü ondan geriye hesaplanır.</summary>
    public static Person NewPerson(Sim s, string race, string role, int civ, int? org, double ageYears, Alignment al, string faith, string house = null)
    {
        house ??= s.Rng.Pick(D.HERO_SURNAMES.GetOr(race, null) ?? D.HERO_SURNAMES["human"]);
        string given = s.Rng.Pick(D.HERO_NAMES.GetOr(race, null) ?? D.HERO_NAMES["human"]);
        var p = new Person
        {
            Id = s.Id(), Name = $"{given} {house}", Race = race, Role = role, Civ = civ, Org = org, Born = s.Day - ageYears * Sim.YEAR,
            Align = al, Faith = faith, House = house, Since = s.Day,
        };
        s.W.People.Add(p);
        return p;
    }

    /// <summary>Yöneticinin hizalaması: tipin kanun ekseni ve kültürün iyilik ortalaması çevresinde.</summary>
    private static Alignment RulerAlign(Sim s, Civ c) => new Alignment
    {
        Law = Clamp(Polity.Gov(c).Law + Noise(s, 0.25)),
        Good = Clamp(Polity.Culture(c).Good + Noise(s, 0.4)),
    };

    /// <summary>Yöneticinin inancı: resmî inanç varsa o, yoksa kültürün en yaygın inancı.</summary>
    private static string RulerFaith(Civ c)
    {
        if (c.Law?.Faith != null) return c.Law.Faith;
        var f = Polity.RACE_FAITH.GetOr(c.Race, null) ?? Polity.RACE_FAITH["human"];
        string best = "sun"; double bv = -1;
        foreach (var kv in f) if (kv.Value > bv) { bv = kv.Value; best = kv.Key; }
        return best;
    }

    /// <summary>Göreve gelen yöneticinin yaşı: ırkın yola çıkış yaşının üstü ile yaşlılığın başı arasında.</summary>
    private static double RulerAge(Sim s, string race)
    {
        var a = D.HERO_AGE.GetOr(race, null) ?? D.HERO_AGE["human"];
        double lo = a[1], hi = a[1] + (a[2] - a[1]) * 0.6;
        return lo + s.Rng.Next() * (hi - lo);
    }

    /// <summary>Yeni devlet (dünya üretimi, yeni kurucular, bölünme): yasa, yönetici, (krallıkta) varis, meşruiyet, hizalama.</summary>
    public static void InitCiv(Sim s, Civ c, double legit = 70)
    {
        c.Law ??= Polity.DefaultLaw(c.Gov ?? "kingdom");
        c.Legit = legit;
        var r = NewPerson(s, c.Race, "ruler", c.Id, null, RulerAge(s, c.Race), RulerAlign(s, c), RulerFaith(c), c.Gov == "kingdom" ? c.House : null);
        if (c.Gov == "kingdom") c.House ??= r.House;
        c.Ruler = r.Id;
        if (c.Gov == "republic") c.Elected = s.Day;
        if (c.Gov == "kingdom" && s.Rng.Chance(0.7)) NewHeir(s, c, r);
        Realign(s, c);
    }

    private static void NewHeir(Sim s, Civ c, Person r)
    {
        var h = NewPerson(s, r.Race, "heir", c.Id, null, s.Rng.Next() * 15, new Alignment { Law = Clamp(r.Align.Law + Noise(s, 0.2)), Good = Clamp(r.Align.Good + Noise(s, 0.35)) }, r.Faith, r.House);
        c.Heir = h.Id;
    }

    /// <summary>Hizalama: kanun = tipin ekseni, yöneticinin kanunu ve yasanın sertliği; iyilik = yönetici ve kültür (kölelik −0,1).</summary>
    public static void Realign(Sim s, Civ c)
    {
        var r = Ruler(s, c);
        var g = Polity.Gov(c);
        double rl = r?.Align.Law ?? g.Law, rg = r?.Align.Good ?? Polity.Culture(c).Good;
        double harsh = c.Law?.Harsh ?? g.Harsh;
        c.Align = new Alignment
        {
            Law = Clamp(0.5 * g.Law + 0.3 * rl + 0.2 * (2 * harsh - 1)),
            Good = Clamp(0.75 * rg + 0.25 * Polity.Culture(c).Good - (c.Law?.Slavery == true ? 0.1 : 0)),
        };
    }

    /// <summary>Kişi gizlice Pakt'a bağlı mı (Kara Pakt'ın sızması).</summary>
    public static bool PactBound(Person p) => p?.Orgs != null && p.Orgs.Exists(m => m.Kind == "pact");

    /// <summary>Kilise ya da Tarikat, yöneticinin Pakt bağını ortaya çıkarır: meşruiyet çöker; teokraside ve krallıkta yönetici
    /// tahttan indirilir (aforoz), diğerlerinde kalır ama devlet kötü bilinir.</summary>
    public static void ExposePact(Sim s, Civ c, Org by)
    {
        var r = Ruler(s, c);
        if (!PactBound(r)) return;
        r.Orgs.RemoveAll(m => m.Kind == "pact");
        c.Legit = JsMath.Max(0, c.Legit - 25);
        s.Metric("pactExposed");
        bool depose = c.Gov == "theocracy" || c.Gov == "kingdom";
        if (depose) s.Metric("collapse_aforoz_" + c.Gov);
        s.Log("politics", $"{c.Name}: {RulerTitle(s, c)} Kara Pakt'a bağlı çıktı!", civ: c.Id, tile: s.Capital(c)?.Tile, cause: $"{by.Name} ortaya çıkardı{(depose ? "; aforoz edildi" : "")}", major: true);
        if (depose)
        {
            Die(s, r, "Pakt'a bağlandığı ortaya çıkınca aforoz edilip idam edildi");
            Succeed(s, c, r, "Yönetici aforoz edildi");
            var seat = s.Capital(c);
            if (seat != null) { Crisis.MarkRegime(s, seat, "aforoz", 0); Status.Set(s, seat, "newlord", "Aforoz"); }   // Faz 1b-7: taht içeriden düştü
        }
        else s.RecomputeEff(c);
    }

    public static string RulerTitle(Sim s, Civ c)
    {
        var r = Ruler(s, c);
        return r == null ? Polity.Gov(c).Title : $"{Polity.Gov(c).Title} {r.Name}";
    }

    // ------------------------------------------------------------ tik (Sim.WORLD_DAYS)
    /// <summary>Her WORLD_DAYS günde: yöneticiler yaşlanır ve ölür, halef seçilir (tipine göre), cumhuriyette seçim, boylarda
    /// meydan okuma; meşruiyet dinlenme değerine yaklaşır; yerleşimlerin inancı hedefe kayar.</summary>
    public static void Tick(Sim s)
    {
        var w = s.W;
        for (int i = 0; i < w.Civs.Count; i++)
        {
            var c = w.Civs[i];
            if (!c.Alive) continue;
            if (c.Law == null) InitCiv(s, c);
            var r = Ruler(s, c);
            if (r == null || r.Died != null) { Succeed(s, c, r, "Taht boş kaldı"); r = Ruler(s, c); }
            // yaşlılık
            if (DiesOfAge(s, r)) { Die(s, r, "yaşlılıktan öldü"); Succeed(s, c, r, $"{RulerTitleOf(c, r)} yaşlılıktan öldü"); r = Ruler(s, c); }
            var heir = PersonById(s, c.Heir);
            if (heir != null && heir.Died == null && DiesOfAge(s, heir)) { Die(s, heir, "yaşlılıktan öldü"); c.Heir = null; }
            // krallıkta varis doğar
            if (c.Gov == "kingdom" && c.Heir == null && r != null && Age(s, r) < (D.HERO_AGE.GetOr(r.Race, null) ?? D.HERO_AGE["human"])[2]
                && s.Rng.Chance(0.15 * Sim.WORLD_DAYS / Sim.YEAR)) NewHeir(s, c, r);
            // cumhuriyette seçim
            if (c.Gov == "republic" && s.Day - (c.Elected ?? s.Day) >= ELECTION_DAYS) Election(s, c, r);
            // boylarda meydan okuma: meşruiyeti düşük reise yılda bir kez kadar (Faz 1b-7: 5–10 gün belirtili kriz, sonunda düello; Crisis)
            if (c.Gov == "clans" && c.Legit < 50 && s.Rng.Chance((0.08 + (50 - c.Legit) / 200) * Sim.WORLD_DAYS / Sim.YEAR))
            {
                var seat = s.Capital(c);
                if (seat != null && seat.Crisis == null) Crisis.Start(s, seat, "challenge", $"Reisin meşruiyeti düşük ({J.S(JsMath.Round(c.Legit))})", null, 0.1);
            }
            // meşruiyet
            c.Legit = JsMath.Max(0, JsMath.Min(100, c.Legit + (LegitTarget(s, c) - c.Legit) * LEGIT_PULL));
        }
        FaithTick(s);
    }

    private static string RulerTitleOf(Civ c, Person r) => r == null ? Polity.Gov(c).Title : $"{Polity.Gov(c).Title} {r.Name}";

    public static bool DiesOfAge(Sim s, Person p)
    {
        if (p == null || p.Died != null) return false;
        var a = D.HERO_AGE.GetOr(p.Race, null) ?? D.HERO_AGE["human"];
        double age = Age(s, p), old = a[2], max = a[3];
        if (age >= max) return true;
        if (age < old) return false;
        double pYear = 0.15 * (1 + 3 * (age - old) / JsMath.Max(1, max - old));
        return s.Rng.Chance(pYear * Sim.WORLD_DAYS / Sim.YEAR);
    }

    public static void Die(Sim s, Person p, string fate)
    {
        if (p == null || p.Died != null) return;
        p.Died = s.Day;
        p.Fate = fate;
    }

    /// <summary>Meşruiyetin dinlendiği değer: barış ve bayındırlık yükseltir; kıtlık, uzun savaş, yakın başkent kaybı düşürür.</summary>
    private static double LegitTarget(Sim s, Civ c)
    {
        double t = LEGIT_BASE;
        if (!s.InWar(c)) t += 5;
        if (c.Famine != null && c.Famine.Declared) t -= 15;
        if (s.Day - (c.CapitalLostDay ?? -99999) < 3 * Sim.OLD_YEAR) t -= 15;
        if (c.Broke != null) t -= 10;
        // resmî inanç halkın çoğunun inancı değilse
        if (c.Law?.Faith != null)
        {
            double all = 0, fit = 0;
            foreach (var st in s.CivSettlements(c)) { double p = s.Pop(st); all += p; fit += p * (st.Faith?.Get(c.Law.Faith) ?? 0.5); }
            if (all > 0 && fit / all < 0.4) t -= 10;
        }
        return t;
    }

    /// <summary>Halef: krallıkta varis (yoksa veraset krizi), boylarda düello ya da boy meclisi, cumhuriyette konsey oyu, teokraside
    /// tarikatın seçimi. Yeni yönetici yeni hizalama demektir.</summary>
    public static void Succeed(Sim s, Civ c, Person old, string why)
    {
        Person nr = null;
        string how;
        bool major = true, feud = false;
        double legit = c.Legit;
        switch (c.Gov)
        {
            case "kingdom":
            {
                var heir = PersonById(s, c.Heir);
                if (heir != null && heir.Died == null)
                {
                    nr = heir; nr.Role = "ruler"; nr.Since = s.Day; c.Heir = null;
                    double minAge = (D.HERO_AGE.GetOr(nr.Race, null) ?? D.HERO_AGE["human"])[0];
                    bool child = Age(s, nr) < minAge;
                    legit = JsMath.Max(20, legit - (child ? 15 : 5));
                    how = child ? $"Varis {nr.Name} henüz çocuk; naipler adına yönetecek" : $"{nr.House} hanedanından {nr.Name} tahta çıktı";
                    major = child;
                    s.Metric("succession_heir");
                }
                else
                {
                    // Faz 1b-7: varissiz taht: soylulardan biri tahta oturur ama tanınmaz; başkentte veraset kavgası başlar (Crisis "feud";
                    // sonunda taht başka hanedana geçebilir, küçük krallık evlilik ittifakıyla komşuya katılabilir)
                    nr = NewPerson(s, c.Race, "ruler", c.Id, null, RulerAge(s, c.Race), RulerAlign(s, c), RulerFaith(c));
                    c.House = nr.House;
                    legit = JsMath.Min(legit, 30);
                    how = $"VERASET KRİZİ: {old?.House ?? c.House ?? "eski"} hanedanı varissiz kaldı; {nr.Name} tahta oturdu ama soylular onu tanımıyor";
                    s.Metric("succession_crisis");
                    feud = true;
                }
                if (c.Heir == null && s.Rng.Chance(0.5)) NewHeir(s, c, nr);
                break;
            }
            case "clans":
            {
                bool duel = s.Rng.Chance(0.5);
                nr = NewPerson(s, c.Race, "ruler", c.Id, null, RulerAge(s, c.Race), RulerAlign(s, c), RulerFaith(c));
                legit = duel ? 50 + s.Rng.Next() * 15 : 60;
                how = duel ? $"Boyların reisliği için düello yapıldı; {nr.Name} ({nr.House} boyu) kazandı" : $"Boy meclisi {Tr.Ek(nr.Name, "i")} ({nr.House} boyu) reis seçti";
                s.Metric(duel ? "succession_duel" : "succession_council");
                break;
            }
            case "republic":
            {
                nr = NewPerson(s, c.Race, "ruler", c.Id, null, RulerAge(s, c.Race), RulerAlign(s, c), RulerFaith(c));
                legit = 55;
                c.Elected = s.Day;
                how = $"Lonca konseyi oylamayla {Tr.Ek(nr.Name, "i")} konsey başı seçti";
                major = false;
                s.Metric("succession_vote");
                break;
            }
            default:
            {
                nr = NewPerson(s, c.Race, "ruler", c.Id, null, RulerAge(s, c.Race), RulerAlign(s, c), "sun");
                legit = JsMath.Max(legit, 60);
                how = $"Güneş Tarikatı {Tr.Ek(nr.Name, "i")} başrahip seçti";
                major = false;
                s.Metric("succession_order");
                break;
            }
        }
        if (old != null && old.Died == null) { old.Role = "former"; old.Fate ??= "görevden ayrıldı"; }
        c.Ruler = nr.Id;
        c.Legit = legit;
        Realign(s, c);
        s.RecomputeEff(c);
        s.Metric("succession");
        var cap = s.Capital(c);
        s.Log("politics", $"{c.Name}: {how}.", civ: c.Id, tile: cap?.Tile, cause: why, major: major);
        if (feud && cap != null && cap.Crisis == null) Crisis.Start(s, cap, "feud", "Taht varissiz kaldı", null, 0.2);
    }

    // ------------------------------------------------------------ Faz 1b-7: iç düşüş, birleşme, savaşın sonucu
    /// <summary>Taht şehri içeriden düştü (Crisis): eski yönetici ölür ya da sürülür, yeni yönetici (yeni hanedan, reis, konsey başı,
    /// başrahip) düşük meşruiyetle gelir; darbe ve paralı askerler kanunu, mezhep bölünmesi yasayı yumuşatır; <paramref name="newGov"/>
    /// verilirse devletin tipi değişir (adı ve yasası da). Çöküş nedeni (ölçüt 9c) eski tipe yazılır.</summary>
    public static void Usurp(Sim s, Civ c, Settlement seat, string path, string kind, string newGov, double lead, string cause)
    {
        var old = Ruler(s, c);
        string oldTitle = RulerTitleOf(c, old);
        string prevGov = c.Gov;
        string fate = kind switch
        {
            "challenge" => "düelloda öldü",
            "feud" => "veraset savaşında tahtını kaybetti",
            "revolt" => "soylularca tahttan indirildi",
            "coup" => "darbeyle devrildi",
            "mutiny" => "paralı askerlerce devrildi",
            "schism" => "mezhep bölünmesinde makamını yitirdi",
            _ => "devrildi",
        };
        if (old != null && old.Died == null)
        {
            if (kind == "challenge" || s.Rng.Chance(0.5)) Die(s, old, fate);
            else { old.Role = "former"; old.Fate = fate + "; sürgüne gitti"; }
        }
        var heir = PersonById(s, c.Heir);
        if (heir != null && heir.Died == null) { heir.Role = "former"; heir.Fate ??= "tahttan uzaklaştırıldı"; }
        c.Heir = null;
        if (newGov != null && newGov != c.Gov)
        {
            c.Gov = newGov;
            c.Law = Polity.DefaultLaw(newGov);
            c.Name = Diplomacy.FreeName(s, newGov, c.Stem ?? seat.Name, c.Id);
            s.Metric("govChange"); s.Metric("govChange_" + prevGov + "_" + newGov);
        }
        var nr = NewPerson(s, c.Race, "ruler", c.Id, null, RulerAge(s, c.Race), RulerAlign(s, c), RulerFaith(c));
        if (kind == "coup" || kind == "mutiny") nr.Align.Law = Clamp(nr.Align.Law - 0.3);
        if (kind == "schism" && c.Law != null)
        {
            c.Law.Harsh = JsMath.Max(0.3, c.Law.Harsh - 0.25);
            c.Law.FaithTol.Set("old", JsMath.Min(1, (c.Law.FaithTol.Get("old") ?? 1) + 0.2));
        }
        if (c.Gov == "kingdom") { c.House = nr.House; if (s.Rng.Chance(0.4)) NewHeir(s, c, nr); }
        if (c.Gov == "republic") c.Elected = s.Day;
        c.Ruler = nr.Id;
        c.Legit = 38 + s.Rng.Next() * 12;
        Realign(s, c);
        s.RecomputeEff(c);
        Crisis.MarkRegime(s, seat, path, lead);
        s.Metric("collapse_" + path + "_" + prevGov);
        s.Metric("succession");
        if (kind == "challenge") { s.Metric("chiefDuel"); s.Metric("chiefDuelLost"); }
        string gov = newGov != null && newGov != prevGov ? $"; devlet artık {Tr.Ek(c.Name, "dır")}" : "";
        string how = kind switch
        {
            "feud" => $"veraset savaşı bitti: {old?.House ?? "eski"} hanedanı düştü, {nr.House} hanedanından {nr.Name} tahta çıktı",
            "revolt" => $"soylular {Tr.Ek(oldTitle, "i")} tahttan indirdi; {nr.Name} ({nr.House} hanedanı) yeni lord",
            "challenge" => $"{oldTitle} meydan okumada düelloda öldü; {nr.Name} ({nr.House} boyu) yeni reis",
            "coup" => $"DARBE: {oldTitle} devrildi, iktidarı {nr.Name} aldı",
            "mutiny" => $"maaşı ödenmeyen paralı askerler {Tr.Ek(seat.Name, "i")} ele geçirdi; bölük kaptanı {nr.Name} iktidarda",
            "schism" => $"mezhep bölünmesi: ılımlılar Başkatedral'i aldı, {nr.Name} yeni başrahip",
            _ => $"{oldTitle} devrildi; {nr.Name} iktidarda",
        };
        if (kind == "mutiny") s.Add(c, "gold", -s.St(c, "gold") * 0.6);
        s.Log("politics", $"{c.Name}: {how}{gov}.", civ: c.Id, tile: seat.Tile, cause: cause, major: true);
        Status.Set(s, seat, "newlord", "Yeni yönetim");
    }

    /// <summary>Devletler birleşir (evlilik ittifakı): küçük devletin yerleşimleri, ambarı, kahramanları ve bildiği yataklar büyüğe geçer;
    /// küçük devlet tarihten silinir.</summary>
    public static void Merge(Sim s, Civ small, Civ big, string how, string cause)
    {
        foreach (var st in s.CivSettlements(small))
        {
            st.Founder ??= small.Id;
            st.LostDay = s.Day;
            if (st.ClaimBy == big.Id) { st.ClaimBy = null; st.ClaimUntil = null; }
            st.Civ = big.Id;
            Status.Set(s, st, "newlord", "Birleşme");
        }
        foreach (var h in s.W.Heroes)
        {
            if (h.Civ != small.Id || h.State == "dead" || h.State == "gone") continue;
            h.Civ = big.Id;
            if (h.Contract != null) h.Contract.Civ = big.Id;
        }
        foreach (var g in small.Stock.Keys()) s.Add(big, g, small.Stock.Get(g) ?? 0);
        small.Stock = new JsObj<double>();
        foreach (var d in s.W.Deposits) if (d.KnownBy.Contains(small.Id) && !d.KnownBy.Contains(big.Id)) d.KnownBy.Add(big.Id);
        var r = Ruler(s, small);
        if (r != null && r.Died == null) { r.Role = "former"; r.Fate ??= "tahtını birleşmeye bıraktı"; }
        small.FallCause = how;
        s.Metric("union");
        s.Log("politics", $"{how}: {small.Name} artık {Tr.Ek(big.Name, "in")} parçası.", civ: big.Id, tile: s.Capital(big)?.Tile, cause: cause, major: true);
        s.UpdateTerritory();
        s.Extinct(small);
        s.RecomputeEff(big);
    }

    /// <summary>Savaş bitti (saldıranın gözünden): kazanan yönetim güçlenir, kaybeden sarsılır. Teokrasinin Kutsal Sefer yenilgisi
    /// meşruiyeti çökertir ve başkentte mezhep çatışmasını açar (spec: teokrasinin çöküş yolu).</summary>
    public static void WarEnded(Sim s, Civ att, Civ def, bool won, string kind)
    {
        if (won) { att.Legit = JsMath.Min(100, att.Legit + 4); def.Legit = JsMath.Max(0, def.Legit - 6); return; }
        att.Legit = JsMath.Max(0, att.Legit - 6);
        if (kind != "crusade" || att.Gov != "theocracy") return;
        att.Legit = JsMath.Max(0, att.Legit - 12);
        s.Metric("crusadeLost");
        var seat = s.Capital(att);
        s.Log("politics", $"{att.Name} Kutsal Sefer'den eli boş döndü; Başkatedral'in otoritesi sarsıldı.", civ: att.Id, tile: seat?.Tile, cause: $"{def.Name} karşısında yenilgi", major: true);
        if (seat != null && seat.Crisis == null) Crisis.Start(s, seat, "schism", "Kutsal Sefer yenilgisi", null, 0.15);
    }

    /// <summary>Cumhuriyette seçim: konsey başı meşruiyetiyle yeniden seçilir ya da yerini başkasına bırakır.</summary>
    private static void Election(Sim s, Civ c, Person r)
    {
        c.Elected = s.Day;
        s.Metric("election");
        if (r != null && r.Died == null && s.Rng.Chance(0.35 + c.Legit / 150))
        {
            c.Legit = JsMath.Min(100, c.Legit + 5);
            return;
        }
        Succeed(s, c, r, r != null ? $"{r.Name} seçimi kaybetti" : "Seçim");
    }

    // ------------------------------------------------------------ inanç
    /// <summary>Yerleşimin inanç hedefi: halkın ırklarının eğilimi (nüfusa ağırlıklı), devletin resmî inancının çekimi (yasanın
    /// sertliğiyle), hoşgörünün baskısı (Pakt hoşgörüsüzlükte söner), tapınak (Güneş) ve kutsal koru (Eski İnanç).</summary>
    public static JsObj<double> FaithTarget(Sim s, Settlement st)
    {
        var t = new JsObj<double>();
        foreach (var f in Polity.FAITH_IDS) t.Set(f, 0);
        double all = 0;
        foreach (var kv in st.Pop)
        {
            if (kv.Value <= 0) continue;
            var rf = Polity.RACE_FAITH.GetOr(kv.Key, null) ?? Polity.RACE_FAITH["human"];
            foreach (var f in Polity.FAITH_IDS) t.Set(f, (t.Get(f) ?? 0) + kv.Value * (rf.Get(f) ?? 0));
            all += kv.Value;
        }
        if (all <= 0) return Polity.RACE_FAITH["human"].Clone();
        foreach (var f in Polity.FAITH_IDS) t.Set(f, (t.Get(f) ?? 0) / all);
        var c = st.Civ >= 0 && st.Civ < s.W.Civs.Count ? s.W.Civs[st.Civ] : null;
        var law = c?.Law;
        if (law != null)
        {
            if (law.Faith != null) t.Set(law.Faith, (t.Get(law.Faith) ?? 0) + 0.15 + 0.25 * law.Harsh);
            foreach (var f in Polity.FAITH_IDS) t.Set(f, (t.Get(f) ?? 0) * (0.4 + 0.6 * (law.FaithTol.Get(f) ?? 1)));
        }
        if (J.T(st.Civics.Get("temple"))) t.Set("sun", (t.Get("sun") ?? 0) + 0.08);
        if (J.Some(s.G.Within(st.Tile, s.RadiusOf(st)), i => s.W.Tiles[i].Ext?.Kind == "grove" && s.W.Tiles[i].Owner == st.Id)) t.Set("old", (t.Get("old") ?? 0) + 0.08);
        double sum = 0;
        foreach (var f in Polity.FAITH_IDS) sum += t.Get(f) ?? 0;
        foreach (var f in Polity.FAITH_IDS) t.Set(f, (t.Get(f) ?? 0) / JsMath.Max(1e-9, sum));
        return t;
    }

    /// <summary>Her yerleşimin inancı hedefine yaklaşır (yeni yerleşim hedefle doğar).</summary>
    public static void FaithTick(Sim s)
    {
        foreach (var st in s.W.Settlements)
        {
            if (!st.Alive) continue;
            var t = FaithTarget(s, st);
            if (st.Faith == null) { st.Faith = t; continue; }
            foreach (var f in Polity.FAITH_IDS)
            {
                double v = st.Faith.Get(f) ?? 0;
                st.Faith.Set(f, v + ((t.Get(f) ?? 0) - v) * FAITH_PULL);
            }
        }
    }

    /// <summary>Yerleşimin en yaygın inancı.</summary>
    public static string MajorFaith(Settlement st)
    {
        if (st?.Faith == null) return null;
        string best = null; double bv = -1;
        foreach (var kv in st.Faith) if (kv.Value > bv) { bv = kv.Value; best = kv.Key; }
        return best;
    }

    // ------------------------------------------------------------ yıllık
    /// <summary>Takvim yılında bir (eski sınıf yeteneklerinin devlete kalanı): teokraside Güneş Bayramı (eski Rahip bayramı:
    /// ambar bereketlenir, kahramanlar iyileşir, meşruiyet artar).</summary>
    public static void Yearly(Sim s, Civ c)
    {
        var cap = s.Capital(c);
        if (cap == null) return;
        if (s.E(c, "festival") > 0)
        {
            s.Add(c, "grain", 40);
            foreach (var h in s.CivHeroes(c)) h.Hp = h.MaxHp;
            c.Legit = JsMath.Min(100, c.Legit + 3);
            s.Log("class", $"{c.Name} Güneş Bayramı'nı kutladı: ambarlar bereketlendi.", civ: c.Id, tile: cap.Tile);
        }
    }
}
