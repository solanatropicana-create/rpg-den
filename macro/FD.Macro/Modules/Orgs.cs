using System;
using System.Collections.Generic;

// Faz 1b-6: örgütler (claude/devlet-orgut-spec.md §4–5). Sınıflar artık örgütlerde yaşar: Güneş Kilisesi (rahip), Güneş Tarikatı
// (paladin), Kara Pakt (paktçı), Druid Çemberi, Avcılar Locası (korucu), Hırsızlar Loncası (haydut), Büyücü Akademisi (sihirbaz),
// Paralı Bölükler (savaşçı), Ozanlar Koleji (ozan), Tüccarlar Loncası, Harabe Kâşifleri, Köle Avcıları, Özgürlük Ağı. Korsanlar
// eskisi gibi korsan koylarıdır (Monsters/Sea), ayrı bir örgüt kaydı yok.
//
// Dünya üretimi (tarih öncesinin sonunda, Sim.Genesis): merkezler çekirdek şehirlere dağıtılır (yasal ya da himayeli devlette;
// gizli örgütler yasak yerde de), başlangıç şubeleri talebe göre açılır, yaşayan kahramanlar yollarına göre üye olur.

namespace FD.Macro;

public static partial class Orgs
{
    /// <summary>şube açma talebinin eşiği; düzey 2 ve 3 eşikleri</summary>
    public const double BRANCH_MIN = 0.6, BRANCH_L2 = 1.6, BRANCH_L3 = 3;

    public static Org ById(Sim s, int id)
    {
        foreach (var o in s.W.Orgs) if (o.Id == id) return o;
        return null;
    }

    /// <summary>Türün yaşayan örgütü (her türden bir tane).</summary>
    public static Org Find(Sim s, string kind)
    {
        foreach (var o in s.W.Orgs) if (o.Alive && o.Kind == kind) return o;
        return null;
    }

    public static OrgBranch Branch(Org o, int settlement)
    {
        if (o == null) return null;
        foreach (var b in o.Branches) if (b.Settlement == settlement) return b;
        return null;
    }

    /// <summary>Yerleşimde bu türün şubesi var mı (merkez de şubedir).</summary>
    public static bool HasBranch(Sim s, Settlement st, string kind) => st != null && Branch(Find(s, kind), st.Id) != null;

    /// <summary>Şubenin düzeyi (yoksa 0).</summary>
    public static int BranchLevel(Sim s, Settlement st, string kind) => st == null ? 0 : Branch(Find(s, kind), st.Id)?.Level ?? 0;

    private static Civ CivOf(Sim s, Settlement st) => st.Civ >= 0 && st.Civ < s.W.Civs.Count ? s.W.Civs[st.Civ] : null;

    /// <summary>Örgütün bu yerleşimde şube talebi (0: yok). Yasaklı yerde yalnız gizli örgüt şube açar (gizli hücre).</summary>
    public static double Demand(Sim s, OrgDef d, Settlement st)
    {
        var c = CivOf(s, st);
        if (c == null || !st.Alive || st.Hub != null || st.Tier < d.MinTier) return 0;   // Faz 1b-7: fırsat merkezinde şube yok
        string lg = Polity.Legal(c, d.Id);
        if (lg == Polity.BANNED && !d.Secret) return 0;
        double legal = lg == Polity.PATRON ? 1.3 : lg == Polity.LEGAL ? 1 : 0.6;
        var f = st.Faith;
        double sun = f?.Get("sun") ?? 0.4, old = f?.Get("old") ?? 0.3, pact = f?.Get("pact") ?? 0.03;
        int t = st.Tier;
        double P = s.Pop(st);
        bool market = J.T(st.Civics.Get("market")), tavern = J.T(st.Civics.Get("tavern"));
        double v;
        switch (d.Id)
        {
            case "church": v = sun * (1 + t) * (J.T(st.Civics.Get("temple")) ? 1.5 : 1); break;
            case "order": v = sun * t * 0.8; break;
            case "pact":
            {
                double tief = (st.Pop.Get("tiefling") ?? 0) / JsMath.Max(1, P);
                double dark = (States.Ruler(s, c)?.Align.Good ?? 0) < 0 ? 0.2 : 0;
                v = t * (pact * 15 + tief * 1.5 + dark);
                break;
            }
            case "circle":
            {
                bool forest = J.Some(s.G.Within(st.Tile, 3), i => s.W.Tiles[i].Terrain == "forest" || s.W.Tiles[i].Terrain == "oldforest");
                v = old * (1 + 0.5 * t) * (forest ? 1.3 : 0.8);
                break;
            }
            case "hunters":
            {
                int camps = J.Filter(s.W.Camps, cp => cp.Alive && cp.Kind != "pirate" && s.G.Dist(cp.Tile, st.Tile) <= 14).Count;
                v = camps * 0.5 + 0.3 * t;
                break;
            }
            case "thieves": v = t * 0.5 * (market ? 1.5 : 1) * (P > 60 ? 1.3 : 1); break;
            case "academy":
            {
                double arc = ((st.Pop.Get("gnome") ?? 0) + (st.Pop.Get("tiefling") ?? 0) + (st.Pop.Get("elf") ?? 0)) / JsMath.Max(1, P);
                v = t * 0.45 * (J.T(st.Civics.Get("library")) ? 1.6 : 0.8) + arc * 2;
                break;
            }
            case "companies": v = t * 0.5 * (s.InWar(c) || c.Threat > 0.3 ? 1.5 : 1); break;
            case "bards": v = (tavern ? 0.6 : 0.25) * (1 + t) + (J.Some(s.W.Inns, inn => inn.Alive && s.G.Dist(inn.Tile, st.Tile) <= 8) ? 0.4 : 0); break;
            case "merchants": v = market ? 0.5 * (1 + t) : 0.15 * t; break;
            case "explorers":
            {
                int ruins = J.Filter(s.W.Settlements, x => !x.Alive && s.G.Dist(x.Tile, st.Tile) <= 15).Count;
                int relics = J.Filter(s.W.Deposits, dp => (dp.Kind == "heartwood" || dp.Kind == "mana" || dp.Kind == "mithril") && s.G.Dist(dp.Tiles[0], st.Tile) <= 10).Count;
                v = (ruins * 0.6 + relics * 0.4) * (t >= 1 ? 1 : 0);
                break;
            }
            case "slavers": v = c.Law?.Slavery == true ? 0.35 * (1 + t) * (c.Gov == "clans" ? 1.3 : 1) : 0; break;
            case "freedom":
            {
                // köleliğin serbest olduğu yerde gizli sığınak; köleliği yasak devlette sınıra yakın yasal sığınak
                if (c.Law?.Slavery == true) v = 0.3 * t;
                else v = J.Some(s.W.Settlements, x => x.Alive && x.Civ != st.Civ && s.W.Civs[x.Civ].Law?.Slavery == true && s.G.Dist(x.Tile, st.Tile) <= 14) ? 0.35 * (1 + t) : 0;
                break;
            }
            default: v = 0; break;
        }
        return v * legal;
    }

    public static int LevelFor(double demand) => demand >= BRANCH_L3 ? 3 : demand >= BRANCH_L2 ? 2 : demand >= BRANCH_MIN ? 1 : 0;

    /// <summary>Şubenin üye sayısı: düzey × (2 + nüfusun inanca/işe uygun payı).</summary>
    public static double BranchMembers(Sim s, OrgDef d, Settlement st, int level)
    {
        double P = s.Pop(st);
        double share = d.Faith != null ? (st.Faith?.Get(d.Faith) ?? 0.3) * 0.05 : 0.015;
        return Math.Floor(level * (2 + P * share));
    }

    // ------------------------------------------------------------ dünya üretimi (tarih öncesinin sonu)
    /// <summary>Örgütleri kurar: merkezleri çekirdek şehirlere dağıtır, başlangıç şubelerini açar, liderleri ve şube ustalarını
    /// atar, yaşayan kahramanları üye yapar.</summary>
    public static void Genesis(Sim s)
    {
        var w = s.W;
        var cores = J.Filter(w.Settlements, x => x.Alive && Sim.IsCore(x));
        if (cores.Count < 3) cores = J.Filter(w.Settlements, x => x.Alive && x.Tier >= 2);
        if (cores.Count == 0) cores = J.Filter(w.Settlements, x => x.Alive);
        J.Sort(cores, (a, b) => J.Or(s.Pop(b) - s.Pop(a), a.Id - b.Id));
        var load = new JsNumObj<double>();
        foreach (var kind in Polity.ORG_IDS)
        {
            var d = Polity.ORGS[kind];
            Settlement best = null; double bs = double.NegativeInfinity;
            foreach (var st in cores)
            {
                var c = CivOf(s, st);
                string lg = Polity.Legal(c, kind);
                if (lg == Polity.BANNED && !d.Secret) continue;
                double sc = Demand(s, d, st) + (lg == Polity.PATRON ? 2.5 : 0) - (load.Get(st.Id) ?? 0) * 1.2 + s.Rng.Next() * 0.6;
                if (d.Secret && lg == Polity.BANNED) sc -= 0.5;
                if (sc > bs) { bs = sc; best = st; }
            }
            best ??= cores[0];
            load.Add(best.Id, 1);
            Found(s, kind, best, "kuruluş");
        }
        // başlangıç şubeleri
        foreach (var o in w.Orgs)
        {
            if (!o.Alive) continue;
            var d = Polity.ORGS[o.Kind];
            foreach (var st in w.Settlements)
            {
                if (!st.Alive || Branch(o, st.Id) != null) continue;
                int lv = LevelFor(Demand(s, d, st));
                if (lv == 0 || !s.Rng.Chance(0.75)) continue;
                OpenBranch(s, o, st, lv, quiet: true);
            }
            Recount(o);
        }
        foreach (var h in w.Heroes) if (h.State != "dead" && h.State != "gone") JoinFor(s, h, quiet: true);
        int nb = 0;
        foreach (var o in w.Orgs) nb += o.Branches.Count;
        s.Log("world", $"Örgütler kuruldu: {w.Orgs.Count} örgüt, {nb} şube. {string.Join(", ", J.Map(w.Orgs, o => $"{o.Name} ({s.Settlement(o.Hq)?.Name ?? "merkezsiz"})"))}.", major: true,
            cause: "Sınıflar artık örgütlerde yaşıyor: tapınaklar, localar, loncalar ve gizli hücreler");
    }

    /// <summary>Yeni örgüt (ya da yeniden doğuş): merkez şubesi düzey 3, lider.</summary>
    public static Org Found(Sim s, string kind, Settlement hq, string why)
    {
        var d = Polity.ORGS[kind];
        var c = CivOf(s, hq);
        var o = new Org { Id = s.Id(), Kind = kind, Name = d.Name, Hq = hq.Id, Alive = true, Founded = s.Day, Gold = 60 };
        o.Goals.AddRange(J.Slice(d.Goals, 0, 3));
        s.W.Orgs.Add(o);
        OpenBranch(s, o, hq, 3, quiet: true);
        var leaderRace = s.Rng.Weighted(J.Filter(hq.Pop.Keys(), r => (hq.Pop.Get(r) ?? 0) > 0), r => hq.Pop.Get(r) ?? 0) ?? c?.Race ?? "human";
        var lead = States.NewPerson(s, leaderRace, "leader", -1, o.Id, 30 + s.Rng.Next() * 25 * (leaderRace == "elf" || leaderRace == "dwarf" ? 4 : 1),
            new Alignment { Law = d.Law, Good = d.Good }, d.Faith ?? States.MajorFaith(hq) ?? "none");
        o.Leader = lead.Id;
        foreach (var e in d.Enemies) { var eo = Find(s, e); if (eo != null) { o.Rel.Set(eo.Id, -60); eo.Rel.Set(o.Id, -60); } }
        foreach (var e in d.Rivals) { var eo = Find(s, e); if (eo != null) { o.Rel.Set(eo.Id, -20); eo.Rel.Set(o.Id, -20); } }
        foreach (var x in s.W.Orgs)
            if (x != o && x.Alive && Polity.ORGS[x.Kind].Enemies.Contains(kind)) { x.Rel.Set(o.Id, -60); o.Rel.Set(x.Id, -60); }
        Recount(o);
        s.Metric("orgFound");
        return o;
    }

    /// <summary>Şube açar (yasak yerde gizli). Şube ustası atanır.</summary>
    public static OrgBranch OpenBranch(Sim s, Org o, Settlement st, int level, bool quiet = false)
    {
        var d = Polity.ORGS[o.Kind];
        var c = CivOf(s, st);
        bool hidden = c != null && Polity.Legal(c, o.Kind) == Polity.BANNED;
        var b = new OrgBranch { Settlement = st.Id, Level = level, Hidden = hidden, Since = s.Day, Members = BranchMembers(s, d, st, level) };
        o.Branches.Add(b);
        o.Tally.Add("opened", 1);
        s.Metric("orgBranchOpen");
        if (!quiet)
            s.Log("org", $"{o.Name} {Tr.Ek(st.Name, "da")} {(hidden ? "gizli bir " : "")}{d.Branch} açtı.", civ: st.Civ, tile: st.Tile);
        return b;
    }

    /// <summary>Toplam üye sayısı şubelerin toplamıdır.</summary>
    public static void Recount(Org o)
    {
        double m = 0;
        foreach (var b in o.Branches) m += b.Members;
        o.Members = m;
    }

    // ------------------------------------------------------------ üyelik (spec §5)
    /// <summary>Kahramanın doğal örgütü: sınıfa uygun örgüt en kolay yol (paladin → Tarikat, druid → Çember, korucu → Avcılar,
    /// sihirbaz → Akademi, rahip → Kilise, savaşçı → Bölükler, hırsız → Hırsızlar), sonra yolu (Avcı → Avcılar, Şifacı → Kilise,
    /// Bilge → Akademi, Paralı → Bölük, Gezgin → Harabe Kâşifleri, Karanlık → Hırsızlar ya da Pakt). İnanç önkoşulu: Kilise ve
    /// Tarikat Güneş'e, Çember Eski İnanç'a, Pakt kötüye ya da Pakt inancına.</summary>
    public static string NaturalOrg(Sim s, Hero h)
    {
        var cands = new List<string>();
        if (h.Path == "wanderer") cands.Add("explorers");   // gezgin hırsız harabelere, karanlık yoldaki loncaya
        foreach (var k in Polity.ORG_IDS) if (Polity.ORGS[k].Classes.Contains(h.Cls) && !cands.Contains(k)) cands.Add(k);
        foreach (var k in Polity.ORG_IDS) if (h.Path != null && Polity.ORGS[k].Paths.Contains(h.Path) && !cands.Contains(k)) cands.Add(k);
        if (h.Path == "dark" && (h.Align == "evil" || h.Faith == "pact")) cands.Insert(0, "pact");
        foreach (var k in cands)
        {
            var d = Polity.ORGS[k];
            if (d.Faith == "sun" && h.Faith != "sun") continue;
            if (d.Faith == "old" && h.Faith != "old") continue;
            if (k == "pact" && h.Align != "evil" && h.Faith != "pact") continue;
            if (d.Good >= 0.5 && h.Align == "evil") continue;
            if (d.Good <= -0.5 && h.Align == "good") continue;
            if (Find(s, k) != null) return k;
        }
        return null;
    }

    /// <summary>Seviyeye göre rütbe: Sv1–2 aday/üye, Sv5 rütbe, Sv7 yüksek rütbe, Sv9 iç çember.</summary>
    public static int RankFor(int level) => level >= 9 ? 4 : level >= 7 ? 3 : level >= 5 ? 2 : level >= 2 ? 1 : 0;

    public static Membership MemberOf(Hero h, Org o)
    {
        if (h.Orgs == null || o == null) return null;
        foreach (var m in h.Orgs) if (m.Org == o.Id) return m;
        return null;
    }

    /// <summary>Kahraman doğal örgütüne katılır (zaten üyeyse bir şey olmaz).</summary>
    public static Membership JoinFor(Sim s, Hero h, bool quiet = false)
    {
        string k = NaturalOrg(s, h);
        if (k == null) return null;
        var o = Find(s, k);
        var m = MemberOf(h, o);
        if (m != null) return m;
        m = new Membership { Org = o.Id, Kind = o.Kind, Rank = RankFor(h.Level), Rep = 10, Since = s.Day };
        (h.Orgs ??= new List<Membership>()).Add(m);
        o.Tally.Add("heroJoin", 1);
        s.Metric("orgHeroJoin");
        if (!quiet) s.Log("org", $"{s.HeroTitle(h)}, {Tr.Ek(o.Name, "a")} katıldı.", tile: h.Pos, civ: h.Civ >= 0 ? h.Civ : (int?)null);
        return m;
    }

    /// <summary>Kahramanın inancı: rahip ve paladin Güneş, druid Eski İnanç, korucu çoğunlukla Eski İnanç; diğerleri doğduğu
    /// yerin inanç dağılımından (yer bilinmiyorsa ırkın eğiliminden).</summary>
    public static string HeroFaith(Sim s, Hero h, Settlement st)
    {
        switch (h.Cls)
        {
            case "cleric": case "paladin": return "sun";
            case "druid": return "old";
            case "ranger": if (s.Rng.Chance(0.6)) return "old"; break;
        }
        var f = st?.Faith ?? Polity.RACE_FAITH.GetOr(h.Race, null) ?? Polity.RACE_FAITH["human"];
        var ks = J.Filter(f.Keys(), k => (f.Get(k) ?? 0) > 0);
        string pick = ks.Count > 0 ? s.Rng.Weighted(ks, k => f.Get(k) ?? 0) : "none";
        if (pick == "pact" && h.Align == "good") pick = "none";
        return pick;
    }
}
