using System;
using System.Collections.Generic;

// Faz 1b-7: yerleşim durum tablosu (yol haritası v3, "Dünya yapısı"). Dünya büyüyerek değil, durum değiştirerek yaşar: her
// yerleşim 5–15 günde bir zar atar; durum (refah, ticaret patlaması, festival, kaynak bulundu, göç dalgası, kıtlık, canavar tehdidi,
// kaynak tükendi) 3–15 gün sürer; ağırlıklar simden gelir. Salgın, kuşatma, açlık, işgal ve yeni lord simden zorunlu gelir; iç krizler
// (veraset kavgası, isyan, ayrılık, darbe, paralı askerlerin huzursuzluğu, mezhep çatışması, meydan okuma) Crisis.cs'te.
//
// Durumun etkileri: büyüme ve üretim çarpanı (Economy), istikrar (Crisis.Stability) ve göç. İyi durum çevreden göçmen çeker, kötü durum
// halkı kaçırır (göçmen kafileleri; Agents "migrants"). Basamak kayması (orta halka, v3: yerleşim başına 60–150 günde bir) durumla
// gelir: göç yerleşimi kademe eşiğinin öbür yanına geçirecek kadar büyük olabilir (Up/Down olasılığı); kademe nüfusla kalır
// (Sim.TierOf) ve kademelerin büyüme tavanı farklı olduğundan kayma kalıcıdır (köy ve kasaba iki ayrı denge).

namespace FD.Macro;

/// <summary>Durum tanımı. Kind: good | bad | neutral | forced.</summary>
public sealed class StatusDef
{
    public string Id, Name, Kind;
    /// <summary>süre (gün; zorunlu durumlar koşul sürdükçe)</summary>
    public int Min = 3, Max = 15;
    /// <summary>büyüme ve üretim (çıkarma ve toplayıcı) çarpanı, istikrara katkı</summary>
    public double Growth = 1, Prod = 1, Stab;
    /// <summary>başlangıçta kaçan / gelen nüfus payı</summary>
    public double Push, Pull;
    /// <summary>başlangıçta göçün yerleşimi bir kademe yukarı / aşağı taşıma olasılığı (eşik yakınsa)</summary>
    public double Up, Down;
}

public static class Status
{
    /// <summary>zar aralığı (gün; v3: her yerleşimde 5–15 günde bir)</summary>
    public const int ROLL_MIN = 5, ROLL_MAX = 15;
    /// <summary>zarda "olağan" (durum yok) ağırlığı</summary>
    public const double CALM_W = 4;
    /// <summary>göç menzili (fersah)</summary>
    public const double MIG_R = 14;
    /// <summary>basamak kayması için göçün en çok nüfus payı (yukarı, aşağı)</summary>
    public const double UP_MAX = 0.7, DOWN_MAX = 0.5;
    /// <summary>büyük şehir bağış yapınca bu nüfusun altına inmez (Şehir eşiği ve histerezis payı)</summary>
    public const double BIG_KEEP = 92;

    /// <summary>zarın sırası (ağırlıklar Weights'te)</summary>
    public static readonly string[] ORDER = { "prosper", "boom", "festival", "found", "migration", "shortage", "monsters", "depleted" };

    public static readonly Dictionary<string, StatusDef> DEFS = new()
    {
        ["prosper"] = new StatusDef { Id = "prosper", Name = "Refah", Kind = "good", Min = 6, Max = 15, Growth = 1.8, Prod = 1.1, Stab = 8, Pull = 0.08, Up = 0.35 },
        ["boom"] = new StatusDef { Id = "boom", Name = "Ticaret patlaması", Kind = "good", Min = 4, Max = 10, Growth = 1.4, Stab = 5, Pull = 0.1, Up = 0.35 },
        ["festival"] = new StatusDef { Id = "festival", Name = "Festival", Kind = "good", Min = 3, Max = 5, Stab = 10 },
        ["found"] = new StatusDef { Id = "found", Name = "Kaynak bulundu", Kind = "good", Min = 5, Max = 12, Growth = 1.3, Prod = 1.15, Stab = 4, Pull = 0.08, Up = 0.3 },
        ["migration"] = new StatusDef { Id = "migration", Name = "Göç dalgası", Kind = "neutral", Min = 4, Max = 10, Stab = -3, Pull = 0.15, Up = 0.45 },
        ["shortage"] = new StatusDef { Id = "shortage", Name = "Kıtlık", Kind = "bad", Min = 5, Max = 15, Growth = 0, Prod = 0.6, Stab = -10, Push = 0.12, Down = 0.6 },
        ["monsters"] = new StatusDef { Id = "monsters", Name = "Canavar tehdidi", Kind = "bad", Min = 4, Max = 12, Growth = 0.5, Prod = 0.8, Stab = -5, Push = 0.1, Down = 0.55 },
        ["depleted"] = new StatusDef { Id = "depleted", Name = "Kaynak tükendi", Kind = "bad", Min = 5, Max = 15, Growth = 0.6, Prod = 0.8, Stab = -4, Push = 0.12, Down = 0.6 },
        // zorunlu: simden (salgın, kuşatma, açlık) ya da olaydan (işgal: fetih; yeni lord: bölünme, iç düşüş)
        ["plague"] = new StatusDef { Id = "plague", Name = "Salgın", Kind = "forced", Growth = 0, Stab = -10, Push = 0.05 },
        ["siege"] = new StatusDef { Id = "siege", Name = "Kuşatma", Kind = "forced", Growth = 0, Prod = 0.3, Stab = -8 },
        ["hunger"] = new StatusDef { Id = "hunger", Name = "Kıtlık", Kind = "forced", Growth = 0, Prod = 1, Stab = -12, Push = 0.06 },
        ["occupation"] = new StatusDef { Id = "occupation", Name = "İşgal", Kind = "forced", Min = 8, Max = 14, Growth = 0.5, Prod = 0.7, Stab = -12, Push = 0.08, Down = 0.3 },
        ["newlord"] = new StatusDef { Id = "newlord", Name = "Yeni lord", Kind = "forced", Min = 5, Max = 10, Stab = -6 },
    };

    /// <summary>zorunlu durumların önceliği (büyük olan küçüğün yerini alır; sıradan durum 0)</summary>
    private static int Rank(string id) => id switch
    {
        "occupation" => 6, "siege" => 5, "plague" => 4, "hunger" => 3, "newlord" => 1, _ => 0,   // işgal fethin ardından kuşatmanın yerini alır
    };

    public static StatusDef Def(string id) => id != null && DEFS.TryGetValue(id, out var d) ? d : Crisis.Def(id);

    /// <summary>Durumun ve krizin büyüme çarpanı (Economy).</summary>
    public static double GrowthMul(Settlement st)
    {
        double m = 1;
        if (st.Status != null && Def(st.Status) is { } d) m *= d.Growth;
        if (st.Crisis != null && Crisis.Def(st.Crisis) is { } k) m *= k.Growth;
        return m;
    }

    /// <summary>Durumun ve krizin üretim çarpanı (çıkarma yapıları ve toplayıcılar; Economy).</summary>
    public static double ProdMul(Settlement st)
    {
        double m = 1;
        if (st.Status != null && Def(st.Status) is { } d) m *= d.Prod;
        if (st.Crisis != null && Crisis.Def(st.Crisis) is { } k) m *= k.Prod;
        return m;
    }

    /// <summary>İyi durum (göçmen çeker).</summary>
    public static bool Pulls(Settlement st) => st.Status != null && DEFS.TryGetValue(st.Status, out var d) && d.Pull > 0 && d.Kind != "forced";

    /// <summary>Kötü durum (halk kaçar).</summary>
    public static bool Pushes(Settlement st) => (st.Status != null && Def(st.Status) is { } d && d.Push > 0) || st.Crisis != null;

    // ------------------------------------------------------------ günlük
    /// <summary>Her gün: zorunlu durumlar (kuşatma, salgın, açlık), biten durumlar, durumun günlük etkileri, zar (5–15 günde bir).</summary>
    public static void Tick(Sim s)
    {
        var w = s.W;
        HashSet<int> sieged = null;
        foreach (var a in w.Agents)
            if (a.Kind == "army" && a.Purpose == "war" && a.Muster != null && a.To != null && a.Dead != true && a.Returning != true) (sieged ??= new()).Add(a.To.Value);
        for (int i = 0; i < w.Settlements.Count; i++)
        {
            var st = w.Settlements[i];
            if (!st.Alive || st.Hub != null) continue;
            string forced = sieged != null && sieged.Contains(st.Id) ? "siege" : st.Plague != null ? "plague" : st.Starving > 0 ? "hunger" : null;
            if (forced != null)
            {
                if (st.Status == forced) st.StatusUntil = JsMath.Max(st.StatusUntil ?? 0, s.Day + 1);
                else if (Rank(st.Status) < Rank(forced)) Begin(s, st, forced, 1, null);
            }
            if (st.Status != null && s.Day >= (st.StatusUntil ?? 0)) End(s, st);
            else if (st.Status != null) Daily(s, st);
            st.StatusRoll ??= s.Day + s.Rng.Int(1, ROLL_MAX);   // ilk zarlar dağılır
            if (s.Day >= st.StatusRoll)
            {
                st.StatusRoll = s.Day + s.Rng.Int(ROLL_MIN, ROLL_MAX);
                s.Metric("statusRoll");
                Roll(s, st);
            }
        }
    }

    /// <summary>Zar: durum sürüyorsa yok; büyük şehir ve taht şehrinde önce iç kriz (istikrara göre, Crisis.Consider); sonra durum
    /// tablosu (ağırlıklar simden) ya da olağan.</summary>
    private static void Roll(Sim s, Settlement st)
    {
        if (st.Status != null) return;
        if (Crisis.Consider(s, st)) return;
        var c = s.W.Civs[st.Civ];
        var ws = Weights(s, c, st);
        double total = CALM_W;
        foreach (var x in ws) total += x;
        double r = s.Rng.Next() * total - CALM_W;
        if (r < 0) { s.Metric("statusCalm"); return; }
        for (int k = 0; k < ORDER.Length; k++)
        {
            r -= ws[k];
            if (r < 0 || k == ORDER.Length - 1)
            {
                var d = DEFS[ORDER[k]];
                Begin(s, st, d.Id, (int)s.Rng.Int(d.Min, d.Max), null);
                return;
            }
        }
    }

    /// <summary>Durumların ağırlıkları (ORDER sırası): simden.</summary>
    private static double[] Weights(Sim s, Civ c, Settlement st)
    {
        var w = s.W;
        double P = s.Pop(st);
        bool war = s.InWar(c);
        double daysFood = s.FoodTotal(c) / JsMath.Max(0.1, s.CivPop(c) * Sim.FOOD_PER_POP);
        // refah: imar, bolluk, barış; yokluk ve savaş söndürür
        double prosper = 0.2 + (st.Imar ?? 0) / 80 + (daysFood > 20 ? 0.3 : 0) + (c.Legit >= 65 ? 0.2 : 0) - (war ? 0.3 : 0) - Economy.LackCount(st, Economy.LACK_DAYS) * 0.2;
        // ticaret patlaması: yollar, pazar
        double routes = 0;
        foreach (var r in w.Routes) if (r.Alive && (r.A == st.Id || r.B == st.Id)) routes++;
        double boom = st.Tier >= 1 ? 0.08 + routes * 0.15 + (J.T(st.Civics.Get("market")) ? 0.2 : 0) : 0;
        // festival: meşru ve tok yönetim
        double festival = st.Tier >= 1 ? 0.15 + (c.Legit >= 60 ? 0.2 : 0) + (daysFood > 15 ? 0.1 : 0) : 0.05;
        // kaynak bulundu: çevrede işlenmemiş yatak
        double unworked = 0;
        foreach (var d in w.Deposits)
        {
            if (d.Depleted) continue;
            foreach (int t in d.Tiles) if (s.G.Dist(t, st.Tile) <= s.RadiusOf(st) + 3 && w.Tiles[t].Ext == null) { unworked++; break; }
        }
        double found = 0.12 + JsMath.Min(3, unworked) * 0.15;
        // göç dalgası: konutu boş, aç olmayan yerleşim; savaş göçü artırır
        double room = s.Housing(st) - P;
        double migration = st.Starving > 0 ? 0 : 0.05 + JsMath.Min(0.25, JsMath.Max(0, room) / 30) + (war ? 0.1 : 0);
        // kıtlık (yerel kuraklık, hasat kaybı): asıl kuraklık ve boş ambar getirir (gerçek ekonomi; aç haydutlar buna bağlı, ölçüt 9f)
        double shortage = 0.06 + (Economy.Drought(s) ? 1.2 : 0) + (daysFood < 8 ? 0.8 : daysFood < 15 ? 0.3 : 0) + (Economy.LackCount(st, Economy.LACK_DAYS, true) > 0 ? 0.3 : 0);
        // canavar tehdidi: yakındaki bilinen kamp
        double monsters = 0;
        foreach (var cp in w.Camps)
            if (cp.Alive && cp.Kind != "dragon" && !J.T(cp.Hidden) && s.G.Dist(cp.Tile, st.Tile) <= 8) monsters += cp.Kind == "bandit" ? 0.3 : 0.5;
        // kaynak tükendi: çevredeki tükenmiş yapı ya da yatak
        double depleted = 0;
        foreach (int t in s.G.Within(st.Tile, s.RadiusOf(st)))
        {
            var tt = w.Tiles[t];
            if (tt.Owner == st.Id && tt.Ext != null && J.T(tt.Ext.Depleted)) depleted += 0.35;
        }
        depleted = JsMath.Min(1, depleted) + 0.05;
        return new[] { JsMath.Max(0, prosper), boom, festival, found, migration, shortage, JsMath.Min(1.5, monsters), depleted };
    }

    /// <summary>Durum başlar: göç (basamak kayması olasılığıyla), tek seferlik etkiler, akış.</summary>
    public static void Begin(Sim s, Settlement st, string id, int days, string cause)
    {
        if (st.Status != null && st.Status != id) s.Metric("statusReplaced");
        var d = Def(id);
        st.Status = id; st.StatusSince = s.Day; st.StatusUntil = s.Day + JsMath.Max(1, days);
        s.Metric("status_" + id);
        var c = s.W.Civs[st.Civ];
        double P = s.Pop(st);
        string text = null;
        bool major = false;
        switch (id)
        {
            case "prosper": text = $"{Tr.Ek(st.Name, "da")} bolluk: pazarlar dolu, yollar kalabalık."; break;
            case "boom": text = $"{Tr.Ek(st.Name, "da")} ticaret patlaması: kervanlar kapıda sıraya giriyor."; break;
            case "festival":
                text = $"{Tr.Ek(st.Name, "da")} festival var: meydanlarda şenlik.";
                c.Legit = JsMath.Min(100, c.Legit + 0.5);
                s.Add(c, "grain", -JsMath.Min(s.St(c, "grain"), P * 0.3));
                break;
            case "found": text = $"{st.Name} yakınlarında zengin bir damar bulundu; çevreden insanlar akın ediyor."; Hubs.FromFind(s, st); break;
            case "migration": text = $"{Tr.Ek(st.Name, "a")} bir göç dalgası geliyor."; break;
            case "shortage": text = $"{Tr.Ek(st.Name, "da")} hasat kötü geçti: tarlalar az veriyor, halk kaçmaya başladı."; break;
            case "monsters": text = $"{st.Name} çevresinde canavarlar dolaşıyor: tarlalara çıkan yok."; break;
            case "depleted": text = $"{Tr.Ek(st.Name, "in")} damarları ve ormanları tükeniyor; iş azaldı."; break;
            case "occupation": text = null; break;   // fetih akışı yeterli
            case "newlord": text = null; break;
        }
        if (text != null) s.Log("state", text, civ: st.Civ, tile: st.Tile, cause: cause, major: major);
        Migrate(s, st, d);
    }

    /// <summary>Durum biter.</summary>
    private static void End(Sim s, Settlement st)
    {
        string id = st.Status;
        st.Status = null; st.StatusSince = null; st.StatusUntil = null;
        if (id == "shortage" || id == "hunger") st.ShortDay = s.Day;
    }

    /// <summary>Durumun günlük etkileri: ticaret patlaması altın, refah imar getirir.</summary>
    private static void Daily(Sim s, Settlement st)
    {
        var c = s.W.Civs[st.Civ];
        switch (st.Status)
        {
            case "boom": s.Add(c, "gold", s.Pop(st) * 0.02); break;
            case "prosper": st.Imar = JsMath.Min(100, (st.Imar ?? 0) + 0.15); break;
        }
    }

    /// <summary>Elle durum: olay doğurur (işgal: fetih; yeni lord: bölünme, iç düşüş). Daha öncelikli zorunlu durumu ezmez.</summary>
    public static void Set(Sim s, Settlement st, string id, string cause = null)
    {
        if (st == null || !st.Alive || st.Hub != null) return;
        if (Rank(st.Status) > Rank(id)) return;
        var d = DEFS[id];
        Begin(s, st, id, (int)s.Rng.Int(d.Min, d.Max), cause);
    }

    // ------------------------------------------------------------ göç ve basamak kayması
    /// <summary>Kademe atlamak için gereken göçmen (Kamp → Köy, Köy → Kasaba; Kasaba ve Şehir yukarı kaymaz).</summary>
    private static double UpGap(Sim s, Settlement st) => st.Tier >= 2 ? double.PositiveInfinity : Sim.TIER_POP[st.Tier + 1] - s.Pop(st);

    /// <summary>Kademeden düşmek için gidecek nüfus (Köy → Kamp, Kasaba → Köy; Şehir aşağı kaymaz; küçük kamp terk edilir).</summary>
    private static double DownGap(Sim s, Settlement st)
    {
        double P = s.Pop(st);
        if (st.Tier >= Sim.BIG_TIER || Sim.IsCore(st)) return double.PositiveInfinity;
        if (st.Tier == 0) return P <= 6 ? P : double.PositiveInfinity;
        return P - (Math.Ceiling(Sim.TIER_POP[st.Tier] * Sim.TIER_KEEP) - 1);
    }

    /// <summary>Durumun başındaki göç: kötü durum halkı kaçırır (Push), iyi durum çevreden göçmen çeker (Pull). Up/Down olasılığıyla göç
    /// yerleşimi kademe eşiğinin öbür yanına geçirecek kadar büyür (eşiğe uzaklık en çok UP_MAX / DOWN_MAX × nüfus).</summary>
    private static void Migrate(Sim s, Settlement st, StatusDef d)
    {
        double P = s.Pop(st);
        if (P <= 0) return;
        if (d.Push > 0)
        {
            double n = Math.Round(P * d.Push * (0.7 + s.Rng.Next() * 0.6));
            double gap = DownGap(s, st);
            if (d.Down > 0 && gap > 0 && gap <= P * DOWN_MAX && s.Rng.Chance(d.Down)) { n = gap; s.Metric("shiftTry_down"); }
            if (Sim.IsBig(st)) n = JsMath.Min(n, JsMath.Max(0, P - BIG_KEEP));
            if (Sim.IsCore(st)) n = JsMath.Min(n, JsMath.Max(0, P - Sim.CORE_MIN - 2));
            if (n >= 1) Emigrate(s, st, n, Def(st.Status)?.Name ?? d.Name);
        }
        if (d.Pull > 0)
        {
            double n = Math.Round(P * d.Pull * (0.7 + s.Rng.Next() * 0.6));
            double gap = UpGap(s, st);
            if (d.Up > 0 && gap > 0 && gap <= JsMath.Max(6, P * UP_MAX) && s.Rng.Chance(d.Up)) { n = gap; s.Metric("shiftTry_up"); }
            n = JsMath.Min(n, JsMath.Max(0, s.Housing(st) + 12 - P));   // barakalar dolunca gelen durur
            if (n >= 1) Attract(s, st, n, d.Name);
        }
    }

    /// <summary>Halk kaçar: göçmen kafileleri iyi durumdaki komşuya, yoksa kendi devletinde yeri olan yerleşime, yoksa başkente. Gerçekten
    /// yola çıkan sayıyı döndürür.</summary>
    public static double Emigrate(Sim s, Settlement st, double n, string why)
    {
        var w = s.W;
        var c = w.Civs[st.Civ];
        double left = n, sent = 0;
        for (int k = 0; k < 2 && left >= 1; k++)
        {
            Settlement best = null; double bs = double.NegativeInfinity;
            foreach (var o in w.Settlements)
            {
                if (!o.Alive || o.Id == st.Id || o.Civ < 0) continue;
                if (o.Hub != null && !Hubs.Open(s, o)) continue;
                double dd = s.G.Dist(o.Tile, st.Tile);
                if (dd > MIG_R * 1.5) continue;
                if (o.Civ != st.Civ && s.AtWar(o.Civ, st.Civ)) continue;
                if (o.Status == "siege" || o.Status == "plague") continue;
                double sc = -dd / 4 + (Pulls(o) ? 4 : 0) + (o.Hub != null ? 4 : 0) + (o.Civ == st.Civ ? 1.5 : 0) + JsMath.Min(3, (s.Housing(o) - s.Pop(o)) / 4) - (Pushes(o) ? 3 : 0);
                if (sc > bs) { bs = sc; best = o; }
            }
            if (best == null) break;
            double take = k == 0 && left >= 4 ? Math.Ceiling(left * 0.6) : left;
            double moved = SendMigrants(s, st, best, take, why);
            if (moved <= 0) break;
            sent += moved; left -= moved;
        }
        if (sent > 0) s.Metric("emigrants", sent);
        return sent;
    }

    /// <summary>Göçmen çekilir: menzildeki yerleşimlerden (önce kötü durumdakiler, kalabalıklar) birkaç kafile. Gerçekten yola çıkanı döndürür.</summary>
    public static double Attract(Sim s, Settlement st, double n, string why)
    {
        var w = s.W;
        double left = n, got = 0;
        var used = new List<int>();
        for (int k = 0; k < 4 && left >= 1; k++)
        {
            Settlement best = null; double bs = 0;
            foreach (var o in w.Settlements)
            {
                if (!o.Alive || o.Id == st.Id || used.Contains(o.Id) || o.Civ < 0) continue;
                if (o.Hub != null && !Hubs.Leaving(s, o)) continue;
                double dd = s.G.Dist(o.Tile, st.Tile);
                if (dd > MIG_R) continue;
                if (o.Civ != st.Civ && s.AtWar(o.Civ, st.Civ)) continue;
                if (o.Status == "siege" || Pulls(o)) continue;
                double spare = Spare(s, o);
                if (spare < 1) continue;
                double sc = spare * (Pushes(o) ? 3 : 1) * (o.Civ == st.Civ ? 1.5 : 1) / (1 + dd / 5);
                if (sc > bs) { bs = sc; best = o; }
            }
            if (best == null) break;
            used.Add(best.Id);
            double take = JsMath.Min(left, JsMath.Max(1, Math.Floor(Spare(s, best) * 0.35)));
            double moved = SendMigrants(s, best, st, take, why);
            got += moved; left -= moved;
        }
        if (got > 0) s.Metric("immigrants", got);
        return got;
    }

    /// <summary>Bağışçının verebileceği nüfus (büyük şehir Şehir eşiğinin altına inmez, çekirdek şehir tabanının).</summary>
    private static double Spare(Sim s, Settlement o)
    {
        double P = s.Pop(o);
        if (Sim.IsBig(o)) return JsMath.Max(0, P - BIG_KEEP);
        if (Sim.IsCore(o)) return JsMath.Max(0, P - Sim.CORE_MIN - 4);
        return JsMath.Max(0, P - 3);
    }

    /// <summary>Göçmen kafilesi (yolda ajan; varınca katılır; köle avcıları ve haydutlar yolda yakalayabilir). Yol yoksa göç olmaz.</summary>
    public static double SendMigrants(Sim s, Settlement from, Settlement to, double n, string why)
    {
        n = JsMath.Min(n, Spare(s, from) + (from.Tier == 0 && !Sim.IsCore(from) ? 3 : 0));
        if (n < 1) return 0;
        var path = s.Path(from.Tile, to.Tile);
        if (path == null) return 0;
        var pop = s.RemovePop(from, n);
        double moved = s.PopSize(pop);
        if (moved <= 0) return 0;
        s.W.Agents.Add(new Agent { Id = s.Id(), Kind = "settlers", Civ = from.Civ, Path = path, Step = 0, Progress = 0, Speed = Pace.MIGRANTS, Pop = pop, From = from.Id, To = to.Id, Purpose = "migrants" });
        s.Metric("migrantGroups");
        if (moved >= 5) s.Log("migration", $"{Tr.Ek(from.Name, "dan")} {J.S(moved)} kişi {Tr.Ek(to.Name, "a")} göç ediyor.", civ: from.Civ, tile: from.Tile, cause: why);
        return moved;
    }

    /// <summary>Göçmen kafilesi vardı (Agents): hedef yaşıyorsa ona, yoksa geldiği yere, o da yoksa en yakın yerleşime katılır.</summary>
    public static void MigrantsArrive(Sim s, Agent a, int tile)
    {
        var to = s.Settlement(a.To); var home = s.Settlement(a.From);
        var dest = to != null && to.Alive ? to : home != null && home.Alive ? home : null;
        if (dest == null)
        {
            double bd = double.PositiveInfinity;
            foreach (var x in s.W.Settlements) if (x.Alive) { double d = s.G.Dist(x.Tile, tile); if (d < bd) { bd = d; dest = x; } }
        }
        if (dest != null) s.MergePop(dest, a.Pop);
    }
}
