using System;
using System.Collections.Generic;

// Faz 1b-6 (3): esaretin makro istatistikleri (claude/devlet-orgut-spec.md §6). Esarete düşme yolları: köle avcıları yolda zayıf
// kafileleri ve yalnız, zayıf kahramanları yakalar; boylar fethettikleri şehirden esir alır; cumhuriyette borcunu ödemeyen borç
// esaretine düşer; teokraside suçlu ve "kâfir" hapis madenine gönderilir (gizli şube baskınında yakalananlar da). Köleler ve
// mahkûmlar yerleşimin nüfusunun içindedir (Settlement.Slaves / Prisoners; çalışırlar, ayrılamazlar). Kurtulma: kaçış, Özgürlük
// Ağı, köleliği yasak devletin fethi (azat), cezanın bitişi; kahraman için fidye. Yerleşimde köle ve mahkûm toplamı nüfusun en çok
// SLAVE_CAP payı. Köleliğin yasak olduğu devlette köle tutulmaz.

namespace FD.Macro;

public static class Bondage
{
    /// <summary>yerleşimde köle + mahkûm en çok nüfusun bu payı</summary>
    public const double SLAVE_CAP = 0.1;
    /// <summary>mahkûmların her tikte (WORLD_DAYS) salıverilen payı (ceza ortalama ~25 tik ≈ 190 gün)</summary>
    public const double PRISON_RELEASE = 0.04;
    /// <summary>kölelerin her tikte kaçan payı</summary>
    public const double ESCAPE = 0.01;
    /// <summary>köle avcılarının yol baskını menzili (fersah)</summary>
    public const double CAPTURE_RANGE = 8;
    /// <summary>kahraman mahkûmiyeti (gün)</summary>
    public const double HERO_PRISON_MIN = 10, HERO_PRISON_MAX = 30;

    private static Civ CivOf(Sim s, Settlement st) => st != null && st.Civ >= 0 && st.Civ < s.W.Civs.Count ? s.W.Civs[st.Civ] : null;

    private static double Room(Sim s, Settlement st) => JsMath.Max(0, Math.Floor(s.Pop(st) * SLAVE_CAP) - (st.Slaves ?? 0) - (st.Prisoners ?? 0));

    /// <summary>Yerleşimin nüfusundan n kişi yakalandı: teokraside hapis madenine, köleliğin serbest olduğu yerde köleliğe, yoksa hapse
    /// (kısa; yalnız sayılır).</summary>
    public static void Imprison(Sim s, Settlement st, double n, string who)
    {
        var c = CivOf(s, st);
        if (c?.Law == null || n <= 0) return;
        double k = JsMath.Min(n, Room(s, st));
        if (c.Law.PrisonMine) { st.Prisoners = (st.Prisoners ?? 0) + k; s.Metric("prisonMine", k); }
        else if (c.Law.Slavery) { st.Slaves = (st.Slaves ?? 0) + k; s.Metric("enslaved", k); s.Metric("enslaved_raid", k); }
        else s.Metric("jailed", n);
    }

    /// <summary>Yerleşime dışarıdan esir getirilir (nüfusa eklenir, köle sayılır).</summary>
    private static double AddCaptives(Sim s, Settlement st, JsObj<double> pop, string how)
    {
        double n = 0;
        foreach (var kv in pop) { if (kv.Value <= 0) continue; s.AddPop(st, kv.Key, kv.Value); n += kv.Value; }
        st.Slaves = (st.Slaves ?? 0) + n;
        s.Metric("enslaved", n);
        s.Metric("enslaved_" + how, n);
        return n;
    }

    /// <summary>Kölelerden k kişi yerleşimden ayrılır (kaçış, kurtarılma): nüfustan düşer, köleliği yasak en yakın devletin yerleşimine
    /// sığınır (yoksa dağılır).</summary>
    private static double Flee(Sim s, Settlement st, double k, string metric)
    {
        k = JsMath.Min(k, st.Slaves ?? 0);
        if (k <= 0) return 0;
        st.Slaves -= k;
        var gone = s.RemovePop(st, k);
        Settlement haven = null; double bd = double.PositiveInfinity;
        foreach (var x in s.W.Settlements)
        {
            if (!x.Alive || x.Id == st.Id) continue;
            var c = CivOf(s, x);
            if (c?.Law == null || c.Law.Slavery) continue;
            double d = s.G.Dist(x.Tile, st.Tile);
            if (d < bd) { bd = d; haven = x; }
        }
        if (haven != null && bd <= 30) s.MergePop(haven, gone);
        s.Metric(metric, k);
        return k;
    }

    // ------------------------------------------------------------ tik (WORLD_DAYS)
    /// <summary>Her WORLD_DAYS günde: köle ve mahkûm sayısı nüfusa sığdırılır; köleliği yasak devlette köleler azat edilir; mahkûmlar
    /// cezasını bitirir; köleler kaçar; cumhuriyette borç esareti, teokraside kâfirler hapis madenine; esir kahramanlar.</summary>
    public static void Tick(Sim s)
    {
        foreach (var st in s.W.Settlements)
        {
            if (!st.Alive) { st.Slaves = null; st.Prisoners = null; continue; }
            var c = CivOf(s, st);
            var law = c?.Law;
            double P = s.Pop(st);
            if (st.Slaves != null) st.Slaves = JsMath.Min(st.Slaves.Value, Math.Floor(P * 0.5));
            if (st.Prisoners != null) st.Prisoners = JsMath.Min(st.Prisoners.Value, Math.Floor(P * 0.5));
            if ((st.Slaves ?? 0) > 0 && law != null && !law.Slavery)
            {
                double k = st.Slaves.Value;
                st.Slaves = null;
                s.Metric("manumitted", k);
                if (k >= 3) s.Log("politics", $"{Tr.Ek(st.Name, "da")} {J.S(k)} köle azat edildi.", civ: st.Civ, tile: st.Tile, cause: $"{c.Name} köleliği tanımıyor");
            }
            if ((st.Prisoners ?? 0) > 0)
            {
                double r = Math.Ceiling(st.Prisoners.Value * PRISON_RELEASE);
                st.Prisoners -= r;
                s.Metric("released", r);
                if (st.Prisoners <= 0) st.Prisoners = null;
            }
            if ((st.Slaves ?? 0) > 0 && s.Rng.Chance(JsMath.Min(1, st.Slaves.Value * ESCAPE))) Flee(s, st, 1, "escaped");
            if (law == null || st.Tier < 1) continue;
            // cumhuriyette borç esareti: yoksulluk (ekmek yokluğu, açlık) borç doğurur
            if (c.Gov == "republic" && law.Slavery && (st.Starving > 0 || (st.Hunger ?? 0) > 0 || (st.Lack != null && st.Lack.Has("bread"))) && s.Rng.Chance(0.25))
            {
                double n = JsMath.Min(Room(s, st), 1 + Math.Floor(P * 0.01));
                if (n > 0) { st.Slaves = (st.Slaves ?? 0) + n; s.Metric("enslaved", n); s.Metric("enslaved_debt", n); }
            }
            // teokraside kâfirler ve "şüpheli" ırklar hapis madenine
            if (law.PrisonMine && st.Faith != null)
            {
                double heretics = P * ((st.Faith.Get("pact") ?? 0) + (st.Faith.Get("old") ?? 0) * (1 - (law.FaithTol.Get("old") ?? 1)) * 0.2) * law.Harsh * 0.02;
                foreach (var kv in law.RaceTol) if (kv.Value < 0.3) heretics += (st.Pop.Get(kv.Key) ?? 0) * 0.01;
                if (s.Rng.Chance(heretics - Math.Floor(heretics))) heretics = Math.Ceiling(heretics); else heretics = Math.Floor(heretics);
                double n = JsMath.Min(Room(s, st), heretics);
                if (n > 0) { st.Prisoners = (st.Prisoners ?? 0) + n; s.Metric("prisonMine", n); s.Metric("prisonMine_heretic", n); }
            }
        }
        CaptiveHeroes(s);
    }

    /// <summary>Fethedilen yerleşim: köleliği yasak fatih köleleri azat eder; köleliği serbest boylar halktan esir alır.</summary>
    public static void OnConquest(Sim s, Settlement st, Civ wc)
    {
        if (wc?.Law == null) return;
        if (!wc.Law.Slavery) return;   // azat Tick'te
        if (wc.Gov != "clans") return;
        double n = JsMath.Min(Room(s, st), Math.Floor(s.Pop(st) * 0.06));
        if (n <= 0) return;
        st.Slaves = (st.Slaves ?? 0) + n;
        s.Metric("enslaved", n);
        s.Metric("enslaved_war", n);
        s.Log("war", $"{wc.Name}, {Tr.Ek(st.Name, "dan")} {J.S(n)} esir aldı.", civ: wc.Id, tile: st.Tile, cause: "Boylar savaşta esir alır");
    }

    // ------------------------------------------------------------ örgüt eylemleri
    /// <summary>Köle Avcıları: şubelerinin yakınından geçen zayıf kafileleri (öncü, göçmen, mülteci) ya da yalnız ve zayıf bir kahramanı
    /// yakalar; esirler şubenin yerleşimine köle olarak satılır (örgüte altın). Kafilesi kaçırılan devlet, avcıların yurduna kin tutar.</summary>
    public static void OpSlavers(Sim s, Org o)
    {
        var w = s.W;
        foreach (var b in o.Branches)
        {
            var st = s.Settlement(b.Settlement);
            var host = CivOf(s, st);
            if (host?.Law == null || !host.Law.Slavery || b.Hidden || Room(s, st) < 1) continue;
            Agent prey = null; double bd = double.PositiveInfinity;
            foreach (var a in w.Agents)
            {
                if (J.T(a.Dead) || a.Hull != null) continue;
                bool weak = (a.Kind == "settlers" && (a.Troops ?? 0) <= 0) || (a.Kind == "hero" && a.Heroes?.Count == 1 && WeakHero(s, s.Hero(a.Heroes[0])));
                if (!weak) continue;
                if (a.Kind == "settlers" && a.Civ == host.Id) continue;   // kendi devletinin kafilesine dokunmaz
                int t = a.Path[Math.Min(a.Step, a.Path.Count - 1)];
                if (w.Tiles[t].Sea == true) continue;
                double d = s.G.Dist(t, st.Tile);
                if (d <= CAPTURE_RANGE && d < bd) { bd = d; prey = a; }
            }
            if (prey == null) continue;
            if (!s.Rng.Chance(prey.Kind == "hero" ? 0.12 + 0.04 * b.Level : 0.35 + 0.1 * b.Level)) { o.Tally.Add("huntFail", 1); return; }
            if (prey.Kind == "hero")
            {
                var h = s.Hero(prey.Heroes[0]);
                prey.Dead = true;
                Capture(s, h, st, "slave", null);
                o.Gold += 15;
                o.Tally.Add("heroCaptured", 1);
                return;
            }
            var victim = prey.Civ >= 0 && prey.Civ < w.Civs.Count ? w.Civs[prey.Civ] : null;
            double take = JsMath.Min(Room(s, st), JsMath.Min(s.PopSize(prey.Pop), 3 + b.Level));
            if (take <= 0) return;
            var got = new JsObj<double>();
            for (int i = 0; i < take; i++)
            {
                var rs = J.Filter(prey.Pop.Keys(), k => (prey.Pop.Get(k) ?? 0) > 0);
                if (rs.Count == 0) break;
                string r = s.Rng.Weighted(rs, k => prey.Pop.Get(k) ?? 0);
                prey.Pop.Set(r, (prey.Pop.Get(r) ?? 1) - 1);
                got.Add(r, 1);
            }
            if (s.PopSize(prey.Pop) <= 0) prey.Dead = true;
            double n = AddCaptives(s, st, got, "hunt");
            o.Gold += 8 * n;
            o.Tally.Add("captured", n);
            if (victim != null && victim.Alive && victim.Id != host.Id)
                s.AddMod(victim.Id, host.Id, "slavers", "Köle avcıları halkımızı kaçırdı", -6, -24, 0.02, false);
            s.Log("org", $"Köle avcıları {(victim != null ? Tr.Ek(victim.Name, "in") + " " : "")}{(prey.Purpose == "refugee" || prey.Purpose == "homeless" ? "göçmen" : "öncü")} kafilesini {Tr.Ek(st.Name, "a")} yakın bastı: {J.S(n)} kişi zincire vuruldu.",
                civ: st.Civ, tile: st.Tile, cause: $"{o.Name}: {host.Name} köleliği serbest bırakıyor");
            return;
        }
    }

    /// <summary>Özgürlük Ağı: şubelerinin 14 fersah yakınındaki bir yerleşimden köle kaçırır (köleliği yasak yere sığınırlar) ya da esir
    /// bir kahramanı kurtarır. Başarısız olursa kurtarıcılar yakalanır.</summary>
    public static void OpFreedom(Sim s, Org o)
    {
        var w = s.W;
        foreach (var b in o.Branches)
        {
            var bst = s.Settlement(b.Settlement);
            if (bst == null) continue;
            var hero = J.Find(w.Heroes, h => h.State == "captive" && h.CaptiveAt != null && s.Settlement(h.CaptiveAt)?.Alive == true && s.G.Dist(s.Settlement(h.CaptiveAt).Tile, bst.Tile) <= 14);
            if (hero != null && s.Rng.Chance(0.4 + 0.1 * b.Level))
            {
                Release(s, hero, $"{o.Name} onu zincirlerinden kurtardı");
                o.Tally.Add("heroFreed", 1);
                s.Metric("freedHero");
                return;
            }
            var st = J.Find(w.Settlements, x => x.Alive && (x.Slaves ?? 0) >= 1 && s.G.Dist(x.Tile, bst.Tile) <= 14);
            if (st == null) continue;
            if (!s.Rng.Chance(0.4 + 0.1 * b.Level))
            {
                b.Members = JsMath.Max(0, b.Members - 1);
                Imprison(s, st, 1, $"{o.Name} kurtarıcısı");
                o.Tally.Add("rescueFail", 1);
                s.Metric("rescueFail");
                return;
            }
            double k = Flee(s, st, JsMath.Min(st.Slaves ?? 0, 2 + b.Level), "freed");
            o.Tally.Add("freed", k);
            o.Gold += 3 * k;   // köleliğe karşı olanların bağışları
            var host = CivOf(s, st);
            var bh = CivOf(s, bst);
            if (host != null && bh != null && host.Id != bh.Id && s.Rel(host.Id, bh.Id).Contact)
                s.AddMod(host.Id, bh.Id, "freedom", "Kaçak kölelere yataklık ediyorlar", -4, -16, 0.02, false);
            s.Log("org", $"{Tr.Ek(st.Name, "dan")} {J.S(k)} köle bir gece kayboldu.", civ: st.Civ, tile: st.Tile, cause: $"{o.Name}: gizli yollar ve sığınaklar");
            return;
        }
    }

    /// <summary>Yıllık: Köle Avcıları'nın Büyük Pazarı (kasaya altın), Özgürlük Ağı'nın Kaçış Gecesi (bir yerleşimde toplu kaçış).</summary>
    public static void Yearly(Sim s, Org o)
    {
        if (o.Kind == "slavers")
        {
            double slaves = 0;
            foreach (var st in s.W.Settlements) if (st.Alive) slaves += st.Slaves ?? 0;
            o.Gold += JsMath.Min(60, slaves * 0.5);
            return;
        }
        Settlement best = null;
        foreach (var st in s.W.Settlements) if (st.Alive && (st.Slaves ?? 0) > (best?.Slaves ?? 2)) best = st;
        if (best == null) return;
        double k = Flee(s, best, Math.Ceiling(best.Slaves.Value * 0.4), "freed");
        o.Tally.Add("freed", k);
        s.Log("org", $"Kaçış Gecesi: {Tr.Ek(best.Name, "dan")} {J.S(k)} köle kaçtı.", civ: best.Civ, tile: best.Tile, cause: o.Name, major: k >= 5);
    }

    // ------------------------------------------------------------ esir kahramanlar (simetri: kahramanlar da esir düşer)
    /// <summary>Yalnız ve zayıf kahraman: serbest, Sv3 ya da altı, ya da canı yarının altında.</summary>
    public static bool WeakHero(Sim s, Hero h) => h != null && h.Civ == -1 && h.State == "traveling" && (h.Level <= 2 || h.Hp < h.MaxHp * 0.5);

    /// <summary>Kahraman esir düşer: köle (avcılar) ya da mahkûm (devriye; süreli). Durum "captive", yeri esir tutulduğu yerleşim.</summary>
    public static void Capture(Sim s, Hero h, Settlement st, string kind, double? days)
    {
        foreach (var a in s.W.Agents) if (!J.T(a.Dead) && a.Kind == "hero" && a.Heroes != null && a.Heroes.Contains(h.Id)) a.Dead = true;
        h.State = "captive";
        h.Goal = null; h.Auction = null;
        h.Pos = st.Tile;
        h.CaptiveAt = st.Id;
        h.CaptiveKind = kind;
        h.CaptiveUntil = days != null ? s.Day + days.Value : null;
        s.Metric("heroCaptive");
        s.Metric("heroCaptive_" + kind);
        Lore.Deed(s, h, "captive", kind == "slave" ? $"{Tr.Ek(st.Name, "da")} köle avcılarının eline düştü" : $"{Tr.Ek(st.Name, "da")} zindana atıldı", st.Tile, st.Name);
        s.Log("hero", $"{s.HeroTitle(h)} {(kind == "slave" ? "köle avcılarına yakalandı" : "tutuklandı")}; {Tr.Ek(st.Name, "da")} {(kind == "slave" ? "zincirde" : "zindanda")}.", tile: st.Tile, civ: st.Civ, major: h.Level >= 4);
    }

    /// <summary>Esir kahraman serbest kalır ve yuvasına döner.</summary>
    public static void Release(Sim s, Hero h, string why)
    {
        h.State = "traveling";
        h.CaptiveAt = null; h.CaptiveUntil = null; h.CaptiveKind = null;
        s.Metric("heroReleased");
        Lore.Deed(s, h, "freed", why, h.Pos);
        s.Log("hero", $"{s.HeroTitle(h)} özgür: {why}.", tile: h.Pos);
        Will.ReturnToBase(s, h);
    }

    /// <summary>Esir kahramanlar: cezası biter, fidyesini öder (kesesi yeterse), kaçar (çeviklikle) ya da tutulduğu yer yıkılınca kurtulur.</summary>
    private static void CaptiveHeroes(Sim s)
    {
        foreach (var h in s.W.Heroes)
        {
            if (h.State != "captive") continue;
            var st = s.Settlement(h.CaptiveAt);
            if (st == null || !st.Alive) { Release(s, h, "tutulduğu yer yıkıldı"); continue; }
            if (h.CaptiveUntil != null && s.Day >= h.CaptiveUntil.Value) { Release(s, h, "cezasını çekti"); continue; }
            if (h.CaptiveKind == "slave" && CivOf(s, st)?.Law?.Slavery != true) { Release(s, h, "yeni efendiler köleliği tanımadı"); continue; }
            if (h.CaptiveKind == "slave" && h.Gold >= 25 && s.Rng.Chance(0.3)) { h.Gold -= 25; s.Metric("ransom"); Release(s, h, "fidyesini ödedi"); continue; }
            if (s.Rng.Chance(0.03 + Rng.Mod(J.N(h.Stats.Get("dex"))) * 0.01)) { s.Metric("heroEscaped"); Release(s, h, "zincirlerini kırıp kaçtı"); continue; }
        }
    }
}
