using System;
using System.Collections.Generic;

// Faz 1b-6 (2): örgütlerin makro eylemleri (spec §4, yaklaşık 10 günde bir karar): şube açar/kapatır/büyütür (talep, güvenlik,
// yasallık), üye toplar (nüfus ve inanç), görev üretir (Avcılar'ın ödül ilanları ortak panoya; diğer örgütlerin işleri üyelere ve
// üye kahramanlara), devleti etkiler (lobi yasayı değiştirir, darbe meşruiyeti düşürür), gölge savaşı (suikast, sabotaj, ihbar),
// hizmet satar (paralı birlik, şifa, eser, kıtlık siparişi). Yasak yerdeki gizli şubeye devriye baskın yapabilir. Şubesi kalmayan
// örgüt dağılır ve bir süre sonra yeniden kurulur. Yıllık olaylar eski sınıf olaylarının yerine (Güneş Bayramı, Koru Ayini, Ozan
// Şenliği, Bölük Turnuvası, Kara Ayin, Kutsal Sefer çağrısı…).

namespace FD.Macro;

public static partial class Orgs
{
    /// <summary>örgüt kararı (gün; yol haritası v3: ~10 günde bir)</summary>
    public const double ORG_DAYS = 10;
    /// <summary>dağılan örgütün yeniden kurulması (gün)</summary>
    public const double REBIRTH_MIN = 120, REBIRTH_MAX = 360;
    /// <summary>aidat (altın / üye / karar) ve şube bakımı (altın / şube düzeyi / karar)</summary>
    public const double DUES = 0.15, UPKEEP = 0.4;
    /// <summary>yeni şube ve düzey yükseltme bedeli (altın × düzey)</summary>
    public const double BRANCH_COST = 15;
    /// <summary>gizli şubeye baskın olasılığı (karar başına): devriye × sertlik × düzey × RAID_K</summary>
    public const double RAID_K = 0.05;
    /// <summary>gölge savaşı: düşman örgütle karar başına olasılık</summary>
    public const double SHADOW_P = 0.3;
    /// <summary>lobi bedeli ve olasılığı</summary>
    public const double LOBBY_COST = 30, LOBBY_P = 0.2;
    /// <summary>Avcılar Locası'nın aynı anda açık en çok ilanı</summary>
    public const int HUNT_QUESTS = 2;

    /// <summary>Her gün: sırası gelen örgüt karar verir (ORG_DAYS'te bir); dağılmış örgüt zamanı gelince yeniden kurulur.</summary>
    public static void Tick(Sim s)
    {
        var w = s.W;
        if (w.Orgs.Count == 0) return;
        for (int i = 0; i < w.Orgs.Count; i++)
        {
            var o = w.Orgs[i];
            if (!o.Alive)
            {
                if (o.RebirthDay != null && s.Day >= o.RebirthDay.Value) Reborn(s, o);
                continue;
            }
            if (!s.Every(ORG_DAYS, i * 0.77)) continue;
            Decide(s, o);
        }
    }

    private static Settlement Hq(Sim s, Org o) => s.Settlement(o.Hq);

    /// <summary>Örgütün o yerleşimin 6 fersah içinde, evde ya da tavernada bekleyen üye kahramanları.</summary>
    public static List<Hero> MemberHeroes(Sim s, Org o, Settlement st, double range = 6)
    {
        var outp = new List<Hero>();
        foreach (var h in s.W.Heroes)
            if ((h.State == "home" || h.State == "tavern") && MemberOf(h, o) != null && s.G.Dist(h.Pos, st.Tile) <= range) outp.Add(h);
        return outp;
    }

    /// <summary>Üye kahraman işi üstlenir: XP, ün ve örgüt içi itibar; itibar ve seviyeyle rütbe.</summary>
    private static Hero Employ(Sim s, Org o, Settlement st, double xp, string deed)
    {
        var hs = MemberHeroes(s, o, st);
        if (hs.Count == 0) return null;
        var h = s.Rng.Pick(hs);
        Heroes.GainXp(s, h, xp, "org");
        Heroes.AddRenown(s, h, 0.4, "org");
        var m = MemberOf(h, o);
        m.Rep += 5; m.LastTask = s.Day;
        int rank = Math.Max(m.Rank, Math.Min(RankFor(h.Level), 1 + (int)Math.Floor(m.Rep / 40)));
        if (rank > m.Rank)
        {
            m.Rank = rank;
            if (rank >= 4) s.Log("org", $"{s.HeroTitle(h)}, {Tr.Ek(o.Name, "in")} iç çemberine alındı: {Polity.ORGS[o.Kind].Inner} sırrını öğrendi.", tile: h.Pos);
        }
        if (deed != null) Lore.Deed(s, h, "org", deed, st.Tile, o.Name);
        o.Tally.Add("heroTask", 1);
        return h;
    }

    // ------------------------------------------------------------ karar
    private static void Decide(Sim s, Org o)
    {
        o.LastAct = s.Day;
        Sync(s, o);
        if (!o.Alive) return;
        Budget(s, o);
        Raids(s, o);
        if (!o.Alive) return;
        Expand(s, o);
        Operate(s, o);
        if (s.Rng.Chance(SHADOW_P)) ShadowWar(s, o);
        if (o.Gold >= LOBBY_COST * 2 && s.Rng.Chance(LOBBY_P)) Lobby(s, o);
        Leadership(s, o);
        Recount(o);
        if (o.Branches.Count == 0) Dissolve(s, o, "Son şubesi de kapandı");
    }

    /// <summary>Şubeler dünyaya uyar: ölen yerleşimdeki şube kapanır; el değiştiren yerleşimde yasaklanan (gizli olmayan) örgüt sürülür;
    /// yasal durum değişince gizlilik güncellenir; üyeler talebe doğru kayar (üye toplama). Merkez düşerse en büyük şubeye taşınır.</summary>
    private static void Sync(Sim s, Org o)
    {
        var d = Polity.ORGS[o.Kind];
        for (int i = o.Branches.Count - 1; i >= 0; i--)
        {
            var b = o.Branches[i];
            var st = s.Settlement(b.Settlement);
            if (st == null || !st.Alive) { CloseBranch(s, o, i, null); continue; }
            var c = CivOf(s, st);
            bool banned = c == null || Polity.Legal(c, o.Kind) == Polity.BANNED;
            if (banned && !d.Secret)
            {
                CloseBranch(s, o, i, $"{c?.Name ?? "Yeni efendiler"} {Tr.Ek(o.Name, "ı")} yasakladı; {d.Branch} kapandı, üyeler sürüldü");
                s.Metric("orgExpelled");
                continue;
            }
            b.Hidden = banned;
            double target = BranchMembers(s, d, st, b.Level);
            double step = JsMath.Max(1, Math.Floor(Math.Abs(target - b.Members) * 0.15));
            if (b.Members < target) { b.Members = JsMath.Min(target, b.Members + step); o.Tally.Add("recruited", step); }
            else if (b.Members > target) b.Members = JsMath.Max(target, b.Members - step);
        }
        if (o.Branches.Count == 0) { Dissolve(s, o, "Bütün şubeleri dağıldı"); return; }
        if (Branch(o, o.Hq ?? -1) == null)
        {
            var nb = J.Sort(new List<OrgBranch>(o.Branches), (x, y) => J.Or(y.Level - x.Level, J.Or(y.Members - x.Members, x.Settlement - y.Settlement)))[0];
            var old = o.Hq;
            o.Hq = nb.Settlement;
            nb.Level = 3;
            var st = s.Settlement(nb.Settlement);
            s.Metric("orgHqMoved");
            s.Log("org", $"{o.Name} merkezini {Tr.Ek(st.Name, "a")} taşıdı.", civ: st.Civ, tile: st.Tile, cause: $"{s.Settlement(old)?.Name ?? "Eski merkez"} kaybedildi", major: true);
        }
    }

    private static void CloseBranch(Sim s, Org o, int i, string why)
    {
        var b = o.Branches[i];
        o.Branches.RemoveAt(i);
        o.Tally.Add("closed", 1);
        s.Metric("orgBranchClose");
        var m = States.PersonById(s, b.Master);
        if (m != null && m.Died == null) States.Die(s, m, "şubesi kapandı");
        if (why != null)
        {
            var st = s.Settlement(b.Settlement);
            s.Log("org", $"{Tr.Ek(o.Name, "in")} {st?.Name ?? "?"} {Polity.ORGS[o.Kind].Branch}ı kapandı.", civ: st?.Civ, tile: st?.Tile, cause: why, major: b.Level >= 2);
        }
    }

    /// <summary>Kasa: aidat (yasak yerde yarısı), himaye eden devletin katkısı (şube düzeyi başına), şube bakımı.</summary>
    private static void Budget(Sim s, Org o)
    {
        double income = 0, upkeep = 0;
        foreach (var b in o.Branches)
        {
            income += b.Members * DUES * (b.Hidden ? 0.5 : 1);
            upkeep += b.Level * UPKEEP;
            var st = s.Settlement(b.Settlement);
            var c = st != null ? CivOf(s, st) : null;
            if (c != null && !b.Hidden && Polity.Legal(c, o.Kind) == Polity.PATRON && s.St(c, "gold") > 60)
            {
                if (o.Gold >= GoldCap(o)) continue;
                double sub = JsMath.Min(b.Level * 0.3, s.St(c, "gold") * 0.01);
                s.Add(c, "gold", -sub); income += sub;
            }
        }
        o.Gold = JsMath.Max(0, o.Gold + income - upkeep);
        // kasa boşsa ve aidat bakımı karşılamıyorsa en zayıf şube kapanır (merkez hariç)
        if (o.Gold <= 0 && income < upkeep && o.Branches.Count > 1 && s.Rng.Chance(0.3))
        {
            int wi = -1; double wv = double.PositiveInfinity;
            for (int i = 0; i < o.Branches.Count; i++) { var b = o.Branches[i]; if (b.Settlement == o.Hq) continue; double v = b.Level * 10 + b.Members; if (v < wv) { wv = v; wi = i; } }
            if (wi >= 0) CloseBranch(s, o, wi, "Kasa boştu");
        }
    }

    /// <summary>Yasak yerdeki gizli şubeye devriye baskını (devletin devriye sıklığı × sertliği × şubenin düzeyi): şube kapanır ya da
    /// küçülür, yakalanan üyeler hapse (teokraside hapis madenine) ya da köleliğe (Faz 1b-6c: Bondage).</summary>
    private static void Raids(Sim s, Org o)
    {
        for (int i = o.Branches.Count - 1; i >= 0; i--)
        {
            var b = o.Branches[i];
            if (!b.Hidden) continue;
            var st = s.Settlement(b.Settlement);
            var c = CivOf(s, st);
            if (c?.Law == null) continue;
            double p = c.Law.Patrol * c.Law.Harsh * b.Level * RAID_K * (1 - c.Law.Bribe * 0.5);
            if (!s.Rng.Chance(p)) continue;
            double caught = JsMath.Max(1, Math.Floor(b.Members * 0.4));
            b.Members -= caught;
            o.Tally.Add("raided", 1);
            s.Metric("orgRaid");
            s.Metric("orgRaid_" + c.Gov);
            Bondage.Imprison(s, st, caught, $"{o.Name} üyesi");
            if (b.Level <= 1 || b.Members <= 1)
            {
                CloseBranch(s, o, i, $"{c.Name} muhafızları gizli {Polity.ORGS[o.Kind].Branch}ı bastı; {J.S(caught)} üye yakalandı");
                if (o.Branches.Count == 0) return;
            }
            else
            {
                b.Level--;
                s.Log("org", $"{c.Name} muhafızları {Tr.Ek(st.Name, "da")} {Tr.Ek(o.Name, "in")} gizli {Polity.ORGS[o.Kind].Branch}ını bastı; {J.S(caught)} üye yakalandı.", civ: c.Id, tile: st.Tile);
            }
        }
    }

    /// <summary>Şube açar (en yüksek talep, zarla) ya da bir şubeyi büyütür; talebi sönen şube kapanır.</summary>
    private static void Expand(Sim s, Org o)
    {
        var d = Polity.ORGS[o.Kind];
        // talebi sönen şube (merkez hariç)
        for (int i = o.Branches.Count - 1; i >= 0; i--)
        {
            var b = o.Branches[i];
            if (b.Settlement == o.Hq) continue;
            var st = s.Settlement(b.Settlement);
            double dem = Demand(s, d, st);
            if (dem < 0.25 && s.Rng.Chance(0.3)) { CloseBranch(s, o, i, "Talep kalmadı"); continue; }
            int want = LevelFor(dem);
            if (want > b.Level && o.Gold >= BRANCH_COST * (b.Level + 1) && s.Rng.Chance(0.25))
            {
                o.Gold -= BRANCH_COST * (b.Level + 1);
                b.Level++;
                o.Tally.Add("levelUp", 1);
            }
            else if (want < b.Level - 1 && s.Rng.Chance(0.2)) b.Level--;
        }
        if (o.Gold < BRANCH_COST || !s.Rng.Chance(0.5)) return;
        Settlement best = null; double bs = 0;
        foreach (var st in s.W.Settlements)
        {
            if (!st.Alive || Branch(o, st.Id) != null) continue;
            double dem = Demand(s, d, st);
            if (dem < BRANCH_MIN) continue;
            double sc = dem * (0.5 + s.Rng.Next());
            if (sc > bs) { bs = sc; best = st; }
        }
        if (best == null) return;
        o.Gold -= BRANCH_COST;
        OpenBranch(s, o, best, 1);
    }

    /// <summary>Örgütün asıl işi (hedeflerinden): bir operasyon.</summary>
    private static void Operate(Sim s, Org o)
    {
        switch (o.Kind)
        {
            case "church": OpChurch(s, o); break;
            case "order": OpOrder(s, o); break;
            case "pact": OpPact(s, o); break;
            case "circle": OpCircle(s, o); break;
            case "hunters": OpHunters(s, o); break;
            case "thieves": OpThieves(s, o); break;
            case "academy": OpAcademy(s, o); break;
            case "companies": OpCompanies(s, o); break;
            case "bards": OpBards(s, o); break;
            case "merchants": OpMerchants(s, o); break;
            case "explorers": OpExplorers(s, o); break;
            case "slavers": Bondage.OpSlavers(s, o); break;
            case "freedom": Bondage.OpFreedom(s, o); break;
        }
    }

    private static OrgBranch PickBranch(Sim s, Org o, Func<OrgBranch, Settlement, bool> ok = null)
    {
        var cands = new List<OrgBranch>();
        foreach (var b in o.Branches) { var st = s.Settlement(b.Settlement); if (st != null && st.Alive && (ok == null || ok(b, st))) cands.Add(b); }
        if (cands.Count == 0) return null;
        return s.Rng.Weighted(cands, b => b.Level + b.Members * 0.05);
    }

    /// <summary>Kasanın tavanı: GOLD_BASE + şube başına GOLD_PER; fazlası harcanır (üyelere, rüşvete, hayır işlerine; "spent" sayacı).</summary>
    public const double GOLD_BASE = 100, GOLD_PER = 25;

    public static double GoldCap(Org o) => GOLD_BASE + GOLD_PER * o.Branches.Count;

    private static void Done(Sim s, Org o, string op, double gold = 0)
    {
        o.Gold += gold;
        double cap = GoldCap(o);
        if (o.Gold > cap) { o.Tally.Add("spent", o.Gold - cap); o.Gold = cap; }
        o.Tally.Add("op_" + op, 1);
        s.Metric("orgOp");
        s.Metric("orgOp_" + op);
    }

    // ------------------------------------------------------------ operasyonlar
    /// <summary>Kilise: salgında şifa taşır (salgını kısaltır, ölümleri azaltır), hacı kafilelerini korur (bağış, himaye eden devlete meşruiyet).</summary>
    private static void OpChurch(Sim s, Org o)
    {
        if (Unmask(s, o)) return;
        var b = PickBranch(s, o, (x, st) => st.Plague != null);
        if (b != null)
        {
            var st = s.Settlement(b.Settlement);
            st.Plague.Severity *= 0.7;
            st.Plague.Until = JsMath.Max(s.Day + 1, st.Plague.Until - 1);
            var h = Employ(s, o, st, 60, $"{Tr.Ek(st.Name, "a")} salgında şifa taşıdı");
            Done(s, o, "heal", 4);
            s.Log("org", $"{o.Name} {Tr.Ek(st.Name, "a")} şifacılar gönderdi{(h != null ? $"; başlarında {h.Name}" : "")}.", civ: st.Civ, tile: st.Tile, cause: "Salgın");
            return;
        }
        b = PickBranch(s, o);
        if (b == null) return;
        var s2 = s.Settlement(b.Settlement);
        var c = CivOf(s, s2);
        Employ(s, o, s2, 30, "hacı kafilesini korudu");
        if (c != null && Polity.Legal(c, o.Kind) == Polity.PATRON) c.Legit = JsMath.Min(100, c.Legit + 0.3);
        Done(s, o, "pilgrims", 3 + b.Level);
    }

    /// <summary>Tarikat: Pakt hücresi avı gölge savaşında; burada yollarda zırhlı devriye (himaye eden devletin yakınındaki kampları sindirir).</summary>
    private static void OpOrder(Sim s, Org o)
    {
        if (Unmask(s, o)) return;
        var b = PickBranch(s, o, (x, st) => !x.Hidden);
        if (b == null) return;
        var st = s.Settlement(b.Settlement);
        var cp = J.Find(s.W.Camps, x => x.Alive && x.Kind != "dragon" && x.Kind != "pirate" && s.G.Dist(x.Tile, st.Tile) <= 10);
        if (cp != null && s.Rng.Chance(0.3 + 0.1 * b.Level))
        {
            cp.Count = JsMath.Max(1, cp.Count - 1);
            Employ(s, o, st, 80, $"{Tr.Ek(cp.Name, "a")} karşı zırhlı devriyeye çıktı");
            Done(s, o, "patrol", 2);
            return;
        }
        Done(s, o, "vigil", 2);
    }

    /// <summary>Pakt: ruh sözleşmesi (üye ve altın; biri kaybolur), yetkililere sızmak (yönetici gizlice Pakt'a bağlanır: devlet
    /// kötüleşir), suikast (kutsal devletin yöneticisi ya da varisi).</summary>
    private static void OpPact(Sim s, Org o)
    {
        var b = PickBranch(s, o);
        if (b == null) return;
        var st = s.Settlement(b.Settlement);
        var c = CivOf(s, st);
        double r = s.Rng.Next();
        if (r < 0.5)
        {
            b.Members += 1 + b.Level;
            if (s.Pop(st) > 20) s.RemovePop(st, 1);
            if (st.Faith != null) { st.Faith.Set("pact", JsMath.Min(0.4, (st.Faith.Get("pact") ?? 0) + 0.01)); }
            Done(s, o, "soul", 6 + 2 * b.Level);
            s.Log("org", $"{Tr.Ek(st.Name, "da")} biri bir gece kayboldu; mahzenlerde mum ışığı görüldüğü fısıldanıyor.", civ: st.Civ, tile: st.Tile, cause: "Kara Pakt: ruh sözleşmesi");
            return;
        }
        var ruler = c != null ? States.Ruler(s, c) : null;
        if (r < 0.85 && ruler != null && !States.PactBound(ruler))
        {
            double p = 0.06 * b.Level * (1 - c.Law.Harsh * 0.6) * (ruler.Align.Good < 0.2 ? 1.6 : 0.5) * (s.Capital(c)?.Id == st.Id ? 1.5 : 1);
            if (s.Rng.Chance(p))
            {
                (ruler.Orgs ??= new List<Membership>()).Add(new Membership { Org = o.Id, Kind = "pact", Rank = 3, Rep = 50, Since = s.Day });
                s.RecomputeEff(c);
                Done(s, o, "infiltrate", 5);
                s.Metric("pactInfiltrate");
                s.Log("org", $"{c.Name} sarayında karanlık fısıltılar: {States.RulerTitle(s, c)} geceleri mahzenlere iniyor.", civ: c.Id, tile: s.Capital(c)?.Tile, cause: "Kara Pakt yetkililere sızdı");
                return;
            }
        }
        // suikast: kutsal devletin yöneticisi
        if (c != null && Polity.Holy(c) && ruler != null && s.Rng.Chance(0.04 * b.Level))
        {
            States.Die(s, ruler, "Kara Pakt'ın suikastına kurban gitti");
            s.Metric("pactAssassination");
            Done(s, o, "assassinate", 10);
            States.Succeed(s, c, ruler, $"{States.RulerTitle(s, c)} bir gece zehirlendi; mahzende Pakt işareti bulundu");
            c.Legit = JsMath.Max(0, c.Legit - 10);
            return;
        }
        Done(s, o, "whisper", 2);
    }

    /// <summary>Kilise ve Tarikat: başkentinde şubesi olduğu devletin yöneticisi Pakt'a bağlıysa ortaya çıkarabilir.</summary>
    private static bool Unmask(Sim s, Org o)
    {
        foreach (var c in s.W.Civs)
        {
            if (!c.Alive || !States.PactBound(States.Ruler(s, c))) continue;
            var cap = s.Capital(c);
            var b = cap != null ? Branch(o, cap.Id) : null;
            if (b == null || !s.Rng.Chance(0.06 * b.Level)) continue;
            Employ(s, o, cap, 120, $"{c.Name} sarayındaki Pakt bağını ortaya çıkardı");
            Done(s, o, "unmask", 5);
            States.ExposePact(s, c, o);
            return true;
        }
        return false;
    }

    /// <summary>Druid Çemberi: ağaç kesimini ve kirleten madeni sabote eder (bölgedeki bir bıçkıhane ya da maden yanar).</summary>
    private static void OpCircle(Sim s, Org o)
    {
        var b = PickBranch(s, o);
        if (b == null) return;
        var st = s.Settlement(b.Settlement);
        if (s.Rng.Chance(0.35))
        {
            foreach (int ti in s.G.Within(st.Tile, 6))
            {
                var t = s.W.Tiles[ti];
                if (t.Ext == null || J.T(t.Ext.Burned) || (t.Ext.Kind != "lumber" && t.Ext.Kind != "mine") || t.Ext.Level < 2) continue;
                if (Polity.OldWays(s.Settlement(t.Owner))) continue;   // kendi korusunu korur
                t.Ext.Burned = 1; t.Ext.BurnedAt = s.Day;
                var ow = s.Settlement(t.Owner);
                Employ(s, o, st, 50, "ormanı korumak için bir bıçkıhaneyi ateşe verdi");
                Done(s, o, "sabotage", 3);
                s.Log("org", $"{ow?.Name ?? "Bir"} {(t.Ext.Kind == "lumber" ? "bıçkıhanesi" : "madeni")} bir gece yandı; yakında taş halkaya adanmış işaretler bulundu.", civ: ow?.Civ, tile: ti, cause: "Druid Çemberi: ağaç kesimine sabotaj");
                return;
            }
        }
        Employ(s, o, st, 25, "korudaki ayine katıldı");
        Done(s, o, "rite", 3);
    }

    /// <summary>Avcılar Locası: şubelerinin yakınındaki kamplara ödül ilanı asar (ortak pano; ödül locanın kasasından).</summary>
    private static void OpHunters(Sim s, Org o)
    {
        int open = J.Filter(s.W.Quests, q => q.Open && q.Org == o.Id).Count;
        if (open >= HUNT_QUESTS || o.Gold < 12) { Done(s, o, "track", 3); return; }
        Camp best = null; OrgBranch bb = null; double bd = double.PositiveInfinity;
        foreach (var b in o.Branches)
        {
            var st = s.Settlement(b.Settlement);
            if (st == null) continue;
            foreach (var cp in s.W.Camps)
            {
                if (!cp.Alive || J.T(cp.Hidden) || cp.Kind == "dragon" || cp.Kind == "pirate" || J.T(s.W.Tiles[cp.Tile].Isle)) continue;
                if (J.Some(s.W.Quests, q => q.Open && q.Camp == cp.Id)) continue;
                double dd = s.G.Dist(cp.Tile, st.Tile);
                if (dd <= 14 && dd < bd) { bd = dd; best = cp; bb = b; }
            }
        }
        if (best == null) { Done(s, o, "track", 2); return; }
        // ödül panosu: locanın kasasından üçte biri, kampın tehdit ettiği devletin hazinesinden kalanı
        var bst = s.Settlement(bb.Settlement);
        var host = CivOf(s, bst);
        double bounty = JsMath.Round(20 + best.Count * 4);
        double own = JsMath.Min(o.Gold, Math.Ceiling(bounty / 3));
        double fund = host != null ? JsMath.Min(bounty - own, Math.Floor(s.St(host, "gold") * 0.2)) : 0;
        if (host != null) s.Add(host, "gold", -fund);
        o.Gold -= own;
        bounty = own + fund;
        if (bounty < 12) { o.Gold += own; if (host != null) s.Add(host, "gold", fund); Done(s, o, "track", 3); return; }
        s.W.Quests.Add(new Quest { Id = s.Id(), Civ = -1, Camp = best.Id, Bounty = bounty, Posted = s.Day, TakenBy = new List<int>(), Open = true, Inn = null, Expires = s.Day + Heroes.QUEST_DAYS, Org = o.Id });
        s.Metric("orgQuest");
        Done(s, o, "bounty");
        s.Log("quest", $"{o.Name}, {Tr.Ek(bst.Name, "daki")} av köşküne ilan astı: \"{best.Name} temizlensin, ödül {J.S(bounty)} altın.\"", civ: bst.Civ, tile: bst.Tile, cause: $"{best.Name} köşke {J.S(bd)} fersah");
    }

    /// <summary>Hırsızlar Loncası: soygun (yerleşimin devletinin hazinesinden), kaçakçılık (pazarlı kentte).</summary>
    private static void OpThieves(Sim s, Org o)
    {
        var b = PickBranch(s, o);
        if (b == null) return;
        var st = s.Settlement(b.Settlement);
        var c = CivOf(s, st);
        if (c == null) return;
        if (s.Rng.Chance(0.5))
        {
            double take = JsMath.Round(JsMath.Min(s.St(c, "gold") * 0.03, 6 * b.Level));
            if (take >= 1)
            {
                s.Add(c, "gold", -take);
                Employ(s, o, st, 40, $"{Tr.Ek(st.Name, "da")} bir hazine arabasını soydu");
                Done(s, o, "rob", take);
                s.Metric("orgRobGold", take);
                return;
            }
        }
        double gain = (J.T(st.Civics.Get("market")) ? 4 : 2) * b.Level * (1 + (c.Law?.Bribe ?? 0.5) - (c.Law?.Smuggle ?? 0.5));
        Done(s, o, "smuggle", JsMath.Max(1, gain));
    }

    /// <summary>Büyücü Akademisi: antik eser getirir (altın; himaye eden devlete mana), büyü kazası temizliği.</summary>
    private static void OpAcademy(Sim s, Org o)
    {
        var b = PickBranch(s, o);
        if (b == null) return;
        var st = s.Settlement(b.Settlement);
        var c = CivOf(s, st);
        if (c != null && Polity.Legal(c, o.Kind) == Polity.PATRON) s.Add(c, "mana", 1);
        Employ(s, o, st, 45, "akademiye bir antik eser getirdi");
        Done(s, o, "artifact", 4 + b.Level);
    }

    /// <summary>Paralı Bölükler: savaştaki devlete birlik kiralar (başkente asker, kasaya altın); barışta kervan korur.</summary>
    private static void OpCompanies(Sim s, Org o)
    {
        foreach (var b in o.Branches)
        {
            var st = s.Settlement(b.Settlement);
            var c = st != null ? CivOf(s, st) : null;
            if (c == null || !s.InWar(c) || s.St(c, "gold") < 60) continue;
            var cap = s.Capital(c);
            if (cap == null) continue;
            double n = 2 + b.Level;
            double price = 12 * n;
            if (s.St(c, "gold") < price * 1.5) continue;
            s.Add(c, "gold", -price);
            cap.Soldiers += n;
            s.AddPop(cap, s.Rng.Pick(new List<string> { "human", "dragonborn", "halforc", "dwarf" }), n);
            Employ(s, o, st, 70, $"{Tr.Ek(c.Name, "a")} kiralık kılıç oldu");
            Done(s, o, "hire", price);
            s.Metric("orgHireTroops", n);
            s.Log("org", $"{c.Name}, {Tr.Ek(o.Name, "dan")} {J.S(n)} paralı asker kiraladı ({J.S(price)} altın).", civ: c.Id, tile: cap.Tile, cause: "Savaş");
            return;
        }
        var bb = PickBranch(s, o);
        if (bb == null) return;
        Employ(s, o, s.Settlement(bb.Settlement), 30, "bir kervana muhafızlık etti");
        Done(s, o, "escort", 3 + bb.Level);
    }

    /// <summary>Ozanlar Koleji: haber taşır (şubesi olan iki devlet arasında ilişki), şarkılar kahramanın ününü yayar.</summary>
    private static void OpBards(Sim s, Org o)
    {
        var civs = new List<Civ>();
        foreach (var b in o.Branches) { var st = s.Settlement(b.Settlement); var c = st != null ? CivOf(s, st) : null; if (c != null && c.Alive && !civs.Contains(c)) civs.Add(c); }
        if (civs.Count >= 2)
        {
            var a = s.Rng.Pick(civs);
            var x = s.Rng.Pick(J.Filter(civs, y => y != a));
            if (s.Rel(a.Id, x.Id).Contact && !s.AtWar(a.Id, x.Id)) s.AddMod(a.Id, x.Id, "bards", "Ozanların şarkıları", 2, 8, 0.01);
        }
        var bb = PickBranch(s, o);
        if (bb == null) return;
        var bst = s.Settlement(bb.Settlement);
        foreach (var h in s.W.Heroes) if (h.State != "dead" && h.State != "gone" && J.T(h.Legend) && s.G.Dist(h.Pos, bst.Tile) <= 12) { Heroes.AddRenown(s, h, 0.3, "song"); break; }
        Done(s, o, "news", 2 + bb.Level);
    }

    /// <summary>Tüccarlar Loncası: kervan işleri; kıtlıktaki devlete başka devletin ambarından tahıl siparişi (aç devlet öder).</summary>
    private static void OpMerchants(Sim s, Org o)
    {
        var w = s.W;
        foreach (var c in w.Civs)
        {
            if (!c.Alive || c.Famine == null || !c.Famine.Declared) continue;
            if (!J.Some(o.Branches, b => s.Settlement(b.Settlement)?.Civ == c.Id)) continue;
            var seller = J.Find(w.Civs, x => x.Alive && x.Id != c.Id && !s.AtWar(x.Id, c.Id) && s.St(x, "grain") > 80 && J.Some(o.Branches, b => s.Settlement(b.Settlement)?.Civ == x.Id));
            if (seller == null) break;
            double q = 20 * Sim.PACE / 4 * 4, price = JsMath.Round(q * s.Price(seller, "grain") * 1.3);
            if (s.St(c, "gold") < price) break;
            s.Add(seller, "grain", -q); s.Add(c, "grain", q);
            s.Add(c, "gold", -price); s.Add(seller, "gold", price * 0.8);
            Done(s, o, "famineOrder", price * 0.2);
            s.Metric("orgFamineOrder");
            s.Log("org", $"{o.Name}, {Tr.Ek(seller.Name, "dan")} {Tr.Ek(c.Name, "a")} {J.S(q)} tahıl taşıdı.", civ: c.Id, cause: "Kıtlık siparişi");
            return;
        }
        double markets = J.Filter(o.Branches, b => J.T(s.Settlement(b.Settlement)?.Civics.Get("market"))).Count;
        Done(s, o, "caravan", 2 + markets * 0.5);
    }

    /// <summary>Harabe Kâşifleri: şubelerinin yakınındaki harabelerde eser kurtarma (altın, kahramana ün).</summary>
    private static void OpExplorers(Sim s, Org o)
    {
        var b = PickBranch(s, o);
        if (b == null) return;
        var st = s.Settlement(b.Settlement);
        var ruin = J.Find(s.W.Settlements, x => !x.Alive && s.G.Dist(x.Tile, st.Tile) <= 15);
        if (ruin != null && s.Rng.Chance(0.5))
        {
            Employ(s, o, st, 60, $"{ruin.Name} harabesinden bir eser kurtardı");
            Done(s, o, "ruin", 6 + 2 * b.Level);
            return;
        }
        Done(s, o, "map", 3);
    }

    // ------------------------------------------------------------ gölge savaşı
    /// <summary>Düşman örgütle (ilişki ≤ −50) gölge savaşı: aynı yerleşimde ya da yakında iki şube; suikast (şube ustası ya da merkezdeyse
    /// lider ölür, şube küçülür), sabotaj (üyeler dağılır, kasa yağmalanır) ya da ihbar (yasak yerdeki gizli şubeyi devlete ihbar eder:
    /// devlet basar). Başarı iki tarafın yerel gücüne göre; başarısız ajan yakalanır.</summary>
    private static void ShadowWar(Sim s, Org o)
    {
        var w = s.W;
        var foes = J.Filter(w.Orgs, x => x.Alive && x != o && (o.Rel.Get(x.Id) ?? 0) <= -50);
        if (foes.Count == 0) return;
        var e = s.Rng.Pick(foes);
        // hedef şube: en yakın düşman şubesi (bizim bir şubemize 12 fersah)
        OrgBranch mine = null, theirs = null; double bd = double.PositiveInfinity;
        foreach (var mb in o.Branches)
        {
            var ms = s.Settlement(mb.Settlement);
            if (ms == null) continue;
            foreach (var eb in e.Branches)
            {
                var es = s.Settlement(eb.Settlement);
                if (es == null) continue;
                double dd = s.G.Dist(ms.Tile, es.Tile);
                if (dd <= 12 && dd < bd) { bd = dd; mine = mb; theirs = eb; }
            }
        }
        if (theirs == null) return;
        var est = s.Settlement(theirs.Settlement);
        var host = CivOf(s, est);
        double ours = mine.Level * 2 + mine.Members * 0.1 + MemberHeroes(s, o, est, 10).Count * 2;
        double them = theirs.Level * 2 + theirs.Members * 0.1 + MemberHeroes(s, e, est, 10).Count * 2;
        double p = 0.8 * ours / JsMath.Max(1, ours + them);
        // tür: gizli düşman şubesi, onu yasaklayan devlette ve biz orada yasalsak ihbar; Pakt ve Hırsızlar suikastı sever
        string kind;
        bool canDenounce = theirs.Hidden && host != null && Polity.Legal(host, o.Kind) != Polity.BANNED;
        if (canDenounce && s.Rng.Chance(0.6)) kind = "denounce";
        else if ((o.Kind == "pact" || o.Kind == "thieves" || o.Kind == "slavers") && s.Rng.Chance(0.6)) kind = "assassinate";
        else kind = s.Rng.Chance(0.5) ? "sabotage" : "assassinate";
        s.Metric("shadowWar");
        s.Metric("shadow_" + kind);
        o.Tally.Add("shadow", 1);
        o.Rel.Set(e.Id, JsMath.Max(-100, (o.Rel.Get(e.Id) ?? 0) - 2)); e.Rel.Set(o.Id, JsMath.Max(-100, (e.Rel.Get(o.Id) ?? 0) - 4));
        if (!s.Rng.Chance(p))
        {
            mine.Members = JsMath.Max(0, mine.Members - 1);
            o.Tally.Add("shadowFail", 1);
            s.Metric("shadowFail");
            if (mine.Hidden && s.Rng.Chance(0.3))
            {
                int i = o.Branches.IndexOf(mine);
                var ms = s.Settlement(mine.Settlement);
                if (i >= 0) CloseBranch(s, o, i, $"{Tr.Ek(e.Name, "a")} karşı başarısız bir işin ardından ajanları yakalandı; gizli {Polity.ORGS[o.Kind].Branch} ortaya çıktı");
                if (ms != null) Bondage.Imprison(s, ms, 1, $"{o.Name} ajanı");
            }
            return;
        }
        int ti = e.Branches.IndexOf(theirs);
        string place = est.Name;
        switch (kind)
        {
            case "denounce":
            {
                double caught = JsMath.Max(1, Math.Floor(theirs.Members * 0.5));
                Bondage.Imprison(s, est, caught, $"{e.Name} üyesi");
                CloseBranch(s, e, ti, $"{o.Name} ihbar etti; {host.Name} muhafızları gizli {Polity.ORGS[e.Kind].Branch}ı bastı, {J.S(caught)} üye yakalandı");
                host.Legit = JsMath.Min(100, host.Legit + 1);
                Employ(s, o, est, 60, $"{Tr.Ek(e.Name, "in")} gizli yuvasını ihbar etti");
                break;
            }
            case "sabotage":
            {
                theirs.Members = Math.Floor(theirs.Members * 0.7);
                double loot = JsMath.Min(e.Gold, 10 + 5 * theirs.Level);
                e.Gold -= loot; o.Gold += loot * 0.5;
                if (theirs.Members <= 0 && ti >= 0) CloseBranch(s, e, ti, $"{o.Name} sabotajı");
                Employ(s, o, est, 40, $"{Tr.Ek(place, "da")} {Tr.Ek(e.Name, "a")} sabotaj yaptı");
                s.Log("org", $"{Tr.Ek(place, "da")} gölgelerde bir savaş: {o.Name}, {Tr.Ek(e.Name, "in")} {Polity.ORGS[e.Kind].Branch}ını sabote etti.", civ: est.Civ, tile: est.Tile);
                break;
            }
            default:
            {
                bool atHq = theirs.Settlement == e.Hq && s.Rng.Chance(0.25);
                var victim = States.PersonById(s, atHq ? e.Leader : theirs.Master);
                string who = victim != null && victim.Died == null ? victim.Name : "bir şube ustası";
                if (victim != null && victim.Died == null) States.Die(s, victim, $"{Tr.Ek(o.Name, "in")} suikastına kurban gitti");
                if (atHq) e.Leader = null; else theirs.Master = null;
                theirs.Level = Math.Max(1, theirs.Level - 1);
                theirs.Members = JsMath.Max(0, theirs.Members - 2);
                Employ(s, o, est, 70, $"{Tr.Ek(e.Name, "in")} bir ustasını ortadan kaldırdı");
                s.Metric("shadowKill");
                s.Log("org", $"{Tr.Ek(place, "da")} {Tr.Ek(e.Name, "in")} {(atHq ? "lideri" : "şube ustası")} {who} ölü bulundu.", civ: est.Civ, tile: est.Tile, cause: $"Gölge savaşı: {o.Name}", major: atHq);
                break;
            }
        }
    }

    // ------------------------------------------------------------ devleti etkileme
    /// <summary>Lobi: en çok şubesi olan (yasal) devlette yasayı örgütün çıkarına bir adım kaydırır; darbe: meşruiyeti düşük devlette
    /// meşruiyeti sarsar (çöküş yolları Faz 1b/7).</summary>
    private static void Lobby(Sim s, Org o)
    {
        var count = new JsNumObj<double>();
        foreach (var b in o.Branches)
        {
            if (b.Hidden) continue;
            var st = s.Settlement(b.Settlement);
            if (st != null) count.Add(st.Civ, b.Level);
        }
        Civ c = null; double bv = 0;
        foreach (var kv in count) { var x = s.W.Civs[kv.Key]; if (x.Alive && kv.Value > bv) { bv = kv.Value; c = x; } }
        // darbe: Hırsızlar, Pakt ve Tüccarlar meşruiyeti düşük devlette (gizli şubesi de yeter)
        if ((o.Kind == "thieves" || o.Kind == "pact" || o.Kind == "merchants") && o.Gold >= 120)
        {
            foreach (var b in o.Branches)
            {
                var st = s.Settlement(b.Settlement);
                var x = st != null ? CivOf(s, st) : null;
                if (x == null || x.Legit >= 40 || s.Capital(x)?.Id != st.Id || !s.Rng.Chance(0.25)) continue;
                o.Gold -= 80;
                x.Legit = JsMath.Max(0, x.Legit - 15);
                s.Metric("orgCoup");
                s.Metric("collapse_coup_" + x.Gov);
                s.Log("politics", $"{x.Name} başkentinde darbe girişimi: {States.RulerTitle(s, x)} zor kurtuldu.", civ: x.Id, tile: st.Tile, cause: $"{o.Name} perde arkasında; meşruiyet {J.S(JsMath.Round(x.Legit))}", major: true);
                return;
            }
        }
        if (c?.Law == null) return;
        var law = c.Law;
        string what = null;
        switch (o.Kind)
        {
            case "church":
                if (law.Faith == "sun" && (law.FaithTol.Get("old") ?? 1) > 0.3) { law.FaithTol.Set("old", (law.FaithTol.Get("old") ?? 1) - 0.05); what = "Eski İnanç'a hoşgörü azaldı"; }
                else if (law.Harsh < 0.9) { law.Harsh += 0.03; what = "yasa sertleşti"; }
                break;
            case "order": if (law.Harsh < 0.9) { law.Harsh += 0.03; what = "yasa sertleşti"; } break;
            case "circle": if ((law.FaithTol.Get("old") ?? 1) < 1) { law.FaithTol.Set("old", JsMath.Min(1, (law.FaithTol.Get("old") ?? 1) + 0.05)); what = "Eski İnanç'a hoşgörü arttı"; } break;
            case "merchants": if (law.Smuggle < 0.8) { law.Smuggle += 0.04; what = "kaçakçılık cezası arttı"; } break;
            case "academy": if ((law.RaceTol.Get("tiefling") ?? 1) < 1) { law.RaceTol.Set("tiefling", JsMath.Min(1, (law.RaceTol.Get("tiefling") ?? 1) + 0.05)); what = "tieflinglere hoşgörü arttı"; } break;
            case "slavers":
                if (!law.Slavery && c.Gov != "theocracy" && c.Align.Good < 0 && s.Rng.Chance(0.1)) { law.Slavery = true; what = "KÖLELİK SERBEST BIRAKILDI"; }
                break;
            case "freedom":
                if (law.Slavery && c.Align.Good > 0.3 && s.Rng.Chance(0.1)) { law.Slavery = false; what = "KÖLELİK YASAKLANDI"; }
                break;
        }
        if (what == null) return;
        o.Gold -= LOBBY_COST;
        s.Metric("orgLobby");
        States.Realign(s, c);
        bool major = what.Contains("KÖLELİK");
        if (major) s.Metric("slaveryLaw");
        s.Log("politics", $"{c.Name}: {what}.", civ: c.Id, tile: s.Capital(c)?.Tile, cause: $"{o.Name} lobisi", major: major);
    }

    // ------------------------------------------------------------ yaşam döngüsü
    /// <summary>Lider yaşlanır ve ölür; yeni lider üyelerin arasından.</summary>
    private static void Leadership(Sim s, Org o)
    {
        var l = States.PersonById(s, o.Leader);
        if (l != null && l.Died == null && !States.DiesOfAge(s, l)) return;
        if (l != null && l.Died == null) States.Die(s, l, "yaşlılıktan öldü");
        var hq = Hq(s, o);
        if (hq == null) return;
        var d = Polity.ORGS[o.Kind];
        var race = s.Rng.Weighted(J.Filter(hq.Pop.Keys(), r => (hq.Pop.Get(r) ?? 0) > 0), r => hq.Pop.Get(r) ?? 0) ?? "human";
        var nl = States.NewPerson(s, race, "leader", -1, o.Id, 35 + s.Rng.Next() * 20 * (race == "elf" || race == "dwarf" ? 4 : 1), new Alignment { Law = d.Law, Good = d.Good }, d.Faith ?? States.MajorFaith(hq) ?? "none");
        o.Leader = nl.Id;
        s.Metric("orgLeader");
    }

    /// <summary>Örgüt dağılır; bir süre sonra (REBIRTH_MIN–MAX gün) yeniden kurulur.</summary>
    public static void Dissolve(Sim s, Org o, string why)
    {
        if (!o.Alive) return;
        o.Alive = false;
        o.Died = s.Day;
        o.RebirthDay = s.Day + s.Rng.Int(REBIRTH_MIN, REBIRTH_MAX);
        o.Branches.Clear();
        o.Members = 0;
        o.Gold = 0;
        var l = States.PersonById(s, o.Leader);
        if (l != null && l.Died == null) { l.Role = "former"; l.Fate ??= "örgütü dağıldı"; }
        s.Metric("orgDissolve");
        s.Log("org", $"{o.Name} dağıldı.", cause: why, major: true);
    }

    /// <summary>Dağılmış örgüt yeniden kurulur: yasal olduğu (gizliyse herhangi bir) çekirdek şehirde, en yüksek talebe göre.</summary>
    private static void Reborn(Sim s, Org o)
    {
        var d = Polity.ORGS[o.Kind];
        Settlement best = null; double bs = double.NegativeInfinity;
        foreach (var st in s.W.Settlements)
        {
            if (!st.Alive || st.Tier < 2) continue;
            var c = CivOf(s, st);
            string lg = Polity.Legal(c, o.Kind);
            if (lg == Polity.BANNED && !d.Secret) continue;
            double sc = Demand(s, d, st) + (Sim.IsCore(st) ? 1 : 0) + (lg == Polity.PATRON ? 1.5 : 0) + s.Rng.Next() * 0.5;
            if (sc > bs) { bs = sc; best = st; }
        }
        if (best == null || bs < BRANCH_MIN) { o.RebirthDay = s.Day + 60; return; }
        o.Alive = true;
        o.Founded = s.Day;
        o.Rebirths++;
        o.Hq = best.Id;
        o.Gold = 30;
        o.RebirthDay = null;
        OpenBranch(s, o, best, 2, quiet: true);
        Leadership(s, o);
        Recount(o);
        s.Metric("orgReborn");
        s.Log("org", $"{o.Name} {Tr.Ek(best.Name, "da")} yeniden kuruldu.", civ: best.Civ, tile: best.Tile, cause: $"Dağılalı {J.S(s.Day - (o.Died ?? s.Day))} gün oldu", major: true);
    }

    // ------------------------------------------------------------ yıllık olaylar (eski sınıf olayları)
    /// <summary>Takvim yılında bir: Güneş Bayramı (Kilise), Koru Ayini (Çember), Ozan Şenliği, Bölük Turnuvası, Kara Ayin (Pakt),
    /// Kutsal Sefer çağrısı (Tarikat: Pakt'a bağlı ya da kötü bir yönetici varsa), Büyük Av, Büyük Panayır, Yıldız Gecesi.</summary>
    public static void Yearly(Sim s)
    {
        foreach (var o in s.W.Orgs)
        {
            if (!o.Alive) continue;
            var hq = Hq(s, o);
            if (hq == null) continue;
            var d = Polity.ORGS[o.Kind];
            switch (o.Kind)
            {
                case "church":
                    foreach (var b in o.Branches) { var st = s.Settlement(b.Settlement); if (st == null) continue; foreach (var h in s.W.Heroes) if (h.State != "dead" && h.State != "gone" && h.Faith == "sun" && h.Pos == st.Tile) h.Hp = h.MaxHp; }
                    foreach (var c in s.W.Civs) if (c.Alive && Polity.Legal(c, o.Kind) == Polity.PATRON) c.Legit = JsMath.Min(100, c.Legit + 1);
                    break;
                case "circle":
                {
                    int grown = 0;
                    foreach (var b in o.Branches)
                    {
                        var st = s.Settlement(b.Settlement);
                        if (st == null) continue;
                        foreach (int ti in s.G.Within(st.Tile, 3))
                        {
                            if (grown >= 4) break;
                            var t = s.W.Tiles[ti];
                            if (t.Owner != st.Id || t.Terrain != "grass" || t.Deposit >= 0 || t.Ext != null || ti == st.Tile) continue;
                            if (!J.Some(s.G.Neighbors(ti), n => s.W.Tiles[n].Terrain == "forest" || s.W.Tiles[n].Terrain == "oldforest")) continue;
                            t.Terrain = "forest"; t.Wood = 160; grown++;
                        }
                    }
                    if (grown > 0) s.ClearPaths();
                    break;
                }
                case "bards":
                    foreach (var c in s.W.Civs) foreach (var x in s.W.Civs)
                        if (c.Alive && x.Alive && c.Id < x.Id && s.Rel(c.Id, x.Id).Contact && !s.AtWar(c.Id, x.Id)) s.AddMod(c.Id, x.Id, "festival", "Ozan şenliğine birlikte gittiler", 3, 9, 0.02);
                    o.Gold += 10;
                    break;
                case "companies":
                    foreach (var h in MemberHeroes(s, o, hq, 40)) Heroes.GainXp(s, h, 150, "tournament");
                    break;
                case "pact":
                    o.Gold += 25;
                    foreach (var b in o.Branches) { var st = s.Settlement(b.Settlement); if (st != null && s.Pop(st) > 20 && s.Rng.Chance(0.3)) s.RemovePop(st, 1); }
                    break;
                case "order":
                    CrusadeCall(s, o);
                    break;
                case "academy":
                    foreach (var c in s.W.Civs) if (c.Alive && Polity.Legal(c, o.Kind) == Polity.PATRON) s.Add(c, "mana", 3);
                    break;
                case "merchants":
                    foreach (var c in s.W.Civs) if (c.Alive && Polity.Legal(c, o.Kind) == Polity.PATRON) s.Add(c, "gold", 15);
                    o.Gold += 15;
                    break;
                case "slavers": case "freedom":
                    Bondage.Yearly(s, o);
                    break;
            }
            s.Metric("orgYearly");
            s.Log("org", $"{o.Name}: {d.Yearly} ({hq.Name}).", civ: hq.Civ, tile: hq.Tile);
        }
    }

    /// <summary>Tarikatın Kutsal Sefer çağrısı: yöneticisi Pakt'a bağlı ya da kötü bir devlete, kutsal devletler savaş açar
    /// (Diplomacy.CrusadeAgainst; eski paladin seferi).</summary>
    private static void CrusadeCall(Sim s, Org o)
    {
        Civ target = null; double worst = 0;
        foreach (var c in s.W.Civs)
        {
            if (!c.Alive || !Diplomacy.IsEvil(s, c)) continue;
            double v = (States.PactBound(States.Ruler(s, c)) ? 2 : 1) - c.Align.Good;
            if (v > worst) { worst = v; target = c; }
        }
        if (target == null) return;
        s.Metric("orgCrusadeCall");
        Diplomacy.CrusadeAgainst(s, target, $"{o.Name} Kutsal Sefer çağrısı yaptı: {States.RulerTitle(s, target)} karanlığa bağlandı");
    }
}
