using System;
using System.Collections.Generic;
using System.Linq;

// Faz 2: 1:1 bölgeyle (Godot dilimi) bağ. Bölgedeki köy, han ve goblin kampı simdeki bir yerleşime, hana ve kampa bağlanır
// (World.Region). Simden yerele: ad, devlet, nüfus, ırk, iş ve inanç dağılımı, köyün durumu, fiyatlar ve stok, ilanlar, yakın
// olaylar, handaki kahramanlar. Yerelden sime: Local.* olayları (kamp temizlendi, goblin öldü, alışveriş, oyuncu bayıldı…).
// Başsız ölçüm dünyalarında World.Region null'dur; bu dosyadaki hiçbir şey çağrılmaz.

namespace FD.Macro;

/// <summary>Bölgedeki köyün simden gelen özeti (1:1 köyün nüfusu, haneleri ve sokak hâli bundan kurulur).</summary>
public sealed class VillageInfo
{
    public string Name, CivName, Gov, GovName, Ruler, RulerTitle, Race, Status, StatusName, Crisis;
    public double Pop, Stability, Legit;
    public int Tier;
    /// <summary>ırk → kişi (yerleşimin nüfusu)</summary>
    public Dictionary<string, double> Races = new();
    /// <summary>iş → kişi (Economy'nin iş dağılımı; mal adı, atölye adı, "toplayıcı", "zanaatçı", "amele", "inşaatçı")</summary>
    public Dictionary<string, double> Jobs = new();
    /// <summary>inanç → pay (sun, old, pact, none)</summary>
    public Dictionary<string, double> Faith = new();
}

public static class Local
{
    /// <summary>köye bağlanacak hanın ve goblin kampının en uzak mesafesi (fersah); kamp bu kadar yakında yoksa köyün çevresine kurulur</summary>
    public const double BIND_INN = 16, BIND_CAMP = 14;
    /// <summary>bağlanırken kurulan kampın köye uzaklığı (fersah) ve başlangıç kalabalığı</summary>
    public const double SPAWN_MIN = 4, SPAWN_MAX = 9, SPAWN_COUNT = 7;

    public static RegionLink Link(Sim s) => s.W.Region;
    public static Settlement Village(Sim s) => s.W.Region == null ? null : s.Settlement(s.W.Region.Village);
    public static Inn Inn(Sim s) => s.W.Region == null || s.W.Region.Inn < 0 ? null : J.Find(s.W.Inns, i => i.Id == s.W.Region.Inn);
    public static Camp Camp(Sim s) => s.W.Region == null ? null : J.Find(s.W.Camps, c => c.Id == s.W.Region.Camp);
    /// <summary>köyün bugünkü sahibi (köy el değiştirebilir)</summary>
    public static Civ Civ(Sim s) { var v = Village(s); return v != null && v.Civ >= 0 ? s.W.Civs[v.Civ] : null; }
    public static Hero Player(Sim s) => s.W.Region?.Player is int id ? s.Hero(id) : null;

    // ------------------------------------------------------------ bağlama
    /// <summary>
    /// Oyun başı: bir krallığın orta halkasından (Köy kademesi, taht şehri değil) bir köy seçilir; en yakın han ve en yakın goblin
    /// kampı ona bağlanır. Puan: krallık (yoksa herhangi bir devlet), hana ve kampa yakınlık, istikrar. Yakında goblin kampı yoksa
    /// köyün 4–9 fersah çevresine bir goblin kampı kurulur (dünya olayı olarak). Gizli kamp ortaya çıkar. Kampa ilan yoksa köyün
    /// devleti ilan asar (<see cref="EnsureQuest"/>). Deterministik (sim zarıyla).
    /// </summary>
    public static RegionLink Bind(Sim s)
    {
        var w = s.W;
        Settlement best = null; Inn bInn = null; Camp bCamp = null; double bs = double.PositiveInfinity;
        foreach (var st in w.Settlements)
        {
            if (!st.Alive || st.Hub != null || st.Tier != 1 || st.Civ < 0) continue;
            var c = w.Civs[st.Civ];
            if (!c.Alive || s.Capital(c)?.Id == st.Id || J.T(w.Tiles[st.Tile].Isle)) continue;
            Inn inn = null; double di = double.PositiveInfinity;
            foreach (var i in w.Inns) { if (!i.Alive) continue; double d = s.G.Dist(i.Tile, st.Tile); if (d < di) { di = d; inn = i; } }
            Camp cp = NearestGoblins(s, st, out double dc);
            double P = s.Pop(st);
            // 1:1 köyün on evi var: 22–45 kişilik köy ona en iyi oturur
            double popPen = P < 22 ? (22 - P) * 0.8 : P > 45 ? (P - 45) * 0.3 : 0;
            double sc = (c.Gov == "kingdom" ? 0 : 25) + JsMath.Min(di, 40) + (cp != null && dc <= BIND_CAMP ? dc : 18)
                - (st.Stability ?? 60) / 20 + (st.Crisis != null ? 6 : 0) + (di > BIND_INN ? 15 : 0) + popPen;
            if (sc < bs || (sc == bs && best != null && st.Id < best.Id)) { bs = sc; best = st; bInn = di <= BIND_INN * 2 ? inn : null; bCamp = cp != null && dc <= BIND_CAMP ? cp : null; }
        }
        if (best == null) throw new InvalidOperationException("Local.Bind: dünyada bağlanacak köy yok");
        var link = new RegionLink { Village = best.Id, Inn = bInn?.Id ?? -1, Since = s.Day };
        if (bCamp == null)
        {
            bCamp = SpawnGoblinsNear(s, best);
            link.SpawnedCamp = true;
        }
        if (bCamp == null) throw new InvalidOperationException("Local.Bind: köyün çevresine kamp kurulamadı");
        link.Camp = bCamp.Id;
        w.Region = link;
        if (J.T(bCamp.Hidden)) Monsters.Reveal(s, bCamp, $"{best.Name} köylüleri");
        s.Metric("regionBound");
        EnsureQuest(s);
        return link;
    }

    /// <summary>Köye en yakın canlı kara goblin kampı (ada kampı değil) ve uzaklığı.</summary>
    public static Camp NearestGoblins(Sim s, Settlement st, out double dist)
    {
        Camp cp = null; dist = double.PositiveInfinity;
        foreach (var c in s.W.Camps)
        {
            if (!c.Alive || c.Kind != "goblin" || J.T(s.W.Tiles[c.Tile].Isle)) continue;
            double d = s.G.Dist(c.Tile, st.Tile);
            if (d < dist) { dist = d; cp = c; }
        }
        return cp;
    }

    /// <summary>Köyün çevresine (SPAWN_MIN–SPAWN_MAX fersah; ıssız, sahipsiz, başka kampa ≥ 4 fersah) goblin kampı kurar.</summary>
    private static Camp SpawnGoblinsNear(Sim s, Settlement st)
    {
        var w = s.W;
        int bt = -1; double bsc = double.NegativeInfinity;
        for (int pass = 0; pass < 2 && bt < 0; pass++)
            for (int i = 0; i < w.Tiles.Count; i++)
            {
                var t = w.Tiles[i];
                if (t.Terrain == "water" || t.Terrain == "mountain" || J.T(t.Sea) || t.Camp != null || t.Ext != null || J.T(t.Isle) || t.Inn != null) continue;
                if (pass == 0 && (t.Owner >= 0 || t.InnZone != null)) continue;
                double d = s.G.Dist(st.Tile, i);
                if (d < SPAWN_MIN || d > SPAWN_MAX + pass * 4) continue;
                double dc = double.PositiveInfinity;
                foreach (var cp in w.Camps) if (cp.Alive) dc = JsMath.Min(dc, s.G.Dist(cp.Tile, i));
                if (dc < 4) continue;
                double sc = -Math.Abs(d - 6) + (t.Terrain == "forest" || t.Terrain == "oldforest" ? 3 : t.Terrain == "hill" ? 1.5 : 0) + JsMath.Min(dc, 10) * 0.2 - i * 1e-7;
                if (sc > bsc) { bsc = sc; bt = i; }
            }
        if (bt < 0) return null;
        var used = new HashSet<string>(w.Camps.Where(c => c.Alive || c.ClearedDay is double cd && s.Day - cd < Will.LAIR_YEARS * Sim.OLD_YEAR).Select(c => c.Name));
        string name = J.Find(WorldGen.GOBLIN_CAMP_NAMES, n => !used.Contains(n)) ?? $"Kırıkdiş Kampı {w.Camps.Count}";
        var camp = WorldGen.MakeCamp(s.Id(), "goblin", bt, name, s.Day, s.Rng);
        camp.Count = SPAWN_COUNT;
        w.Camps.Add(camp); w.Tiles[bt].Camp = camp.Id; w.Tiles[bt].Owner = -1;
        s.Metric("lairSpawn"); s.Metric("lairSpawnGoblin");
        s.Log("lair", $"{Lore.Ek(st.Name, "in")} ormanlarına goblinler yerleşti: {name}.", tile: bt, major: true, cause: "Ormanın derinliklerinden gelip kazık çaktılar; köyün koyunları kayboluyor");
        return camp;
    }

    /// <summary>
    /// Bölgenin kampı için açık ilan: yoksa köyün sahibi devlet (muhtarın çağrısıyla) hanın panosuna asar; ödül devletin hazinesinden
    /// (yetmezse köyün topladığıyla en az 25 altın). Kamp yoksa ya da temizlendiyse null. Günde bir kez çağrılır (ilan süresi dolunca
    /// köy yeniden ister).
    /// </summary>
    public static Quest EnsureQuest(Sim s)
    {
        var link = s.W.Region; var cp = Camp(s); var v = Village(s);
        if (link == null || cp == null || !cp.Alive || v == null) return null;
        var open = J.Find(s.W.Quests, q => q.Open && q.Camp == cp.Id);
        if (open != null) return open;
        // biri ilanı kopardıysa ve yoldaysa köy yeniden ilan asmaz
        if (J.Some(s.W.Quests, q => !q.Open && q.Camp == cp.Id && q.Done == null && q.TakenBy.Count > 0 && J.Some(s.W.Agents, a => a.Quest == q.Id && !J.T(a.Dead)))) return null;
        var c = Civ(s);
        double gold = c != null ? s.St(c, "gold") : 0;
        double bounty = JsMath.Round(JsMath.Max(25, JsMath.Min(gold * 0.5, 30 + (c?.Threat ?? 0) * 40 + cp.Count * 3)));
        if (c != null) s.Add(c, "gold", -JsMath.Min(gold, bounty));
        var q2 = new Quest { Id = s.Id(), Civ = c?.Id ?? -1, Camp = cp.Id, Bounty = bounty, Posted = s.Day, TakenBy = new List<int>(), Open = true, Inn = link.Inn >= 0 ? link.Inn : null, Expires = s.Day + Heroes.QUEST_DAYS };
        s.W.Quests.Add(q2);
        s.Metric("questPosted"); s.Metric("regionQuest");
        var inn = Inn(s);
        string where = inn != null ? $"{Lore.Ek(Inns.InnName(inn), "in")} panosuna" : "tavernalara";
        s.Log("quest", $"{Lore.Ek(v.Name, "in")} muhtarı {where} ilan astı: \"{cp.Name} temizlensin, ödül {J.S(bounty)} altın.\"",
            civ: c?.Id, tile: v.Tile, cause: "Goblinler köyün koyunlarını çalıyor, yolcuları soyuyor", major: true);
        return q2;
    }

    // ------------------------------------------------------------ simden yerele
    /// <summary>Köyün özeti: ad, devlet, yönetici, nüfus ve dağılımlar, durum, istikrar.</summary>
    public static VillageInfo Info(Sim s)
    {
        var v = Village(s); if (v == null) return null;
        var c = Civ(s);
        var vi = new VillageInfo { Name = v.Name, Pop = s.Pop(v), Tier = v.Tier, Status = v.Status, Crisis = v.Crisis, Stability = v.Stability ?? 60 };
        vi.StatusName = v.Status != null ? FD.Macro.Status.Def(v.Status)?.Name : null;
        if (c != null)
        {
            vi.CivName = c.Name; vi.Gov = c.Gov; vi.GovName = Polity.Gov(c).Name; vi.Race = c.Race; vi.Legit = c.Legit;
            vi.Ruler = States.Ruler(s, c)?.Name; vi.RulerTitle = States.RulerTitle(s, c);
        }
        foreach (var kv in v.Pop) if (kv.Value > 0) vi.Races[kv.Key] = kv.Value;
        foreach (var kv in v.Jobs) if (kv.Value > 0) vi.Jobs[kv.Key] = kv.Value;
        if (v.Faith != null) foreach (var kv in v.Faith) if (kv.Value > 0) vi.Faith[kv.Key] = kv.Value;
        return vi;
    }

    /// <summary>Malın köyün devletindeki fiyatı (altın) ve stoğu.</summary>
    public static double Price(Sim s, string good) { var c = Civ(s); return c != null ? s.Price(c, good) : D.GOODS[good].Base; }
    public static double Stock(Sim s, string good) { var c = Civ(s); return c != null ? s.St(c, good) : 0; }

    /// <summary>Bölgenin kulağına gelen olaylar (yeniden eskiye): köyün ve kampın 12 fersah çevresindekiler, köyün devletinin büyük olayları
    /// ve dünyanın büyük olayları (savaş, ejderha, yönetici değişimi). <paramref name="sinceDay"/>'den sonrası, en çok <paramref name="max"/>.</summary>
    public static List<GameEvent> News(Sim s, double sinceDay, int max = 12)
    {
        var o = new List<GameEvent>();
        var v = Village(s); if (v == null) return o;
        var cp = Camp(s);
        int civ = v.Civ;
        var ev = s.W.Events;
        for (int i = ev.Count - 1; i >= 0 && o.Count < max; i--)
        {
            var e = ev[i];
            if (e.Day < sinceDay) break;
            bool near = e.Tile is int t && (s.G.Dist(t, v.Tile) <= 12 || (cp != null && s.G.Dist(t, cp.Tile) <= 6));
            bool ours = e.Civ == civ && J.T(e.Major);
            bool world = J.T(e.Major) && (e.Kind == "war" || e.Kind == "dragon" || e.Kind == "politics" || e.Kind == "collapse" || e.Kind == "lair");
            if (near || ours || world) o.Add(e);
        }
        return o;
    }

    /// <summary>Hanın şimdi orada olan serbest kahramanları (tavernada bekleyen; ilan alıp yola çıkmamış).</summary>
    public static List<Hero> InnHeroes(Sim s)
    {
        var inn = Inn(s); if (inn == null) return new List<Hero>();
        return J.Filter(s.W.Heroes, h => h.Civ == -1 && h.State == "tavern" && h.BaseInn && h.Base == inn.Id);
    }

    /// <summary>Bölgenin kampına açık ilan (yoksa null).</summary>
    public static Quest CampQuest(Sim s)
    {
        var cp = Camp(s); if (cp == null) return null;
        return J.Find(s.W.Quests, q => q.Open && q.Camp == cp.Id);
    }

    /// <summary>Bölgenin kampına alınmış (kahramanları yolda) ilan ve onu alan kahramanlar (yoksa null).</summary>
    public static (Quest q, List<Hero> heroes)? TakenQuest(Sim s)
    {
        var cp = Camp(s); if (cp == null) return null;
        foreach (var q in s.W.Quests)
        {
            if (q.Open || q.Camp != cp.Id || q.Done != null || q.TakenBy.Count == 0) continue;
            var a = J.Find(s.W.Agents, x => x.Quest == q.Id && !J.T(x.Dead));
            if (a == null) continue;
            var hs = new List<Hero>();
            foreach (int id in a.Heroes ?? new List<int>()) { var h = s.Hero(id); if (h != null) hs.Add(h); }
            return (q, hs);
        }
        return null;
    }

    // ------------------------------------------------------------ oyuncu
    /// <summary>Oyuncunun simdeki kaydı: yaratılan karakter (ırk, sınıf, statlar, hizalama, inanç, can, zırh, kese).</summary>
    public sealed class PlayerSpec
    {
        public string Name, Race, Cls, Align = "neutral", Faith = "none";
        public JsObj<double> Stats = new();
        public double MaxHp, Ac, Gold, Age;
    }

    /// <summary>
    /// Oyuncu simde bir kahramandır (simetri): kahraman kaydı, State "player" (simin yapay zekâsı ona dokunmaz: hedef seçmez,
    /// handa beklemez, yaşlanıp ölmez), bağımsız (Civ −1), yuvasız. İnanç, itibar (Rep), üyelik (Orgs) ve yaralar bu kayıtta
    /// taşınır; oyuncu öldüğünde kayıt "dead" olur. Bağlı köye yeni bir yabancının geldiği yazılır.
    /// </summary>
    public static Hero CreatePlayer(Sim s, PlayerSpec p)
    {
        var link = s.W.Region ?? throw new InvalidOperationException("Local.CreatePlayer: bölge bağlı değil");
        var v = Village(s);
        int id = s.Id();
        var parts = (p.Name ?? "Yabancı").Trim().Split(' ', 2);
        var h = new Hero
        {
            Id = id, Name = p.Name, Race = p.Race, Cls = p.Cls, Level = 1, Xp = 0, Stats = p.Stats, MaxHp = p.MaxHp, Hp = p.MaxHp, Ac = p.Ac,
            Civ = -1, Pos = v?.Tile ?? 0, Tavern = -1, State = "player", Born = s.Day, IdleSince = s.Day, Kills = 0, Gold = p.Gold,
            Bio = "Kimsenin tanımadığı bir yolcu.", Align = p.Align, Path = "wanderer", Traits = new List<string>(), Tally = new JsObj<double>(),
            Bonus = new HeroBonus { Atk = 0 }, Rep = new JsNumObj<double>(), Journal = new List<JournalEntry>(), Base = -1, BaseInn = false,
            Birth = v?.Id ?? -1, Given = parts[0], Surname = parts.Length > 1 ? parts[1] : "", BirthLevel = 1, BirthAge = p.Age,
            Deeds = new List<HeroDeed>(), Faith = p.Faith,
        };
        s.W.Heroes.Add(h);
        link.Player = h.Id;
        s.Metric("playerBorn");
        Will.Note(s, h, $"{(v != null ? Lore.Ek(v.Name, "a") : "bölgeye")} geldi");
        s.Log("hero", $"{(v != null ? Lore.Ek(v.Name, "a") : "Bölgeye")} kimsenin tanımadığı bir yabancı geldi: {h.Name} ({D.RACES.GetOr(h.Race, null)?.Name ?? h.Race}).",
            tile: v?.Tile, cause: "Cebinde birkaç gümüş, sırtında yol giysisi");
        return h;
    }

    /// <summary>Gün başında (Sim.Step'ten sonra) bölgenin bakımı: kamp yaşıyorsa ilan.</summary>
    public static void DayTick(Sim s)
    {
        if (s.W.Region == null) return;
        EnsureQuest(s);
    }
}
