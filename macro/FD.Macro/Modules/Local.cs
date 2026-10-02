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
        // biri ilanı kopardıysa ve yoldaysa (ya da oyuncu aldıysa) köy yeniden ilan asmaz
        if (J.Some(s.W.Quests, q => !q.Open && q.Camp == cp.Id && q.Done == null && q.TakenBy.Count > 0 && J.Some(s.W.Agents, a => a.Quest == q.Id && !J.T(a.Dead)))) return null;
        if (link.Player is int pid && J.Some(s.W.Quests, q => !q.Open && q.Camp == cp.Id && q.Done == null && q.TakenBy.Contains(pid))) return null;
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
    public static Hero CreatePlayer(Sim s, PlayerSpec p, bool announce = true)
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
        if (announce) s.Log("hero", $"{(v != null ? Lore.Ek(v.Name, "a") : "Bölgeye")} kimsenin tanımadığı bir yabancı geldi: {h.Name} ({D.RACES.GetOr(h.Race, null)?.Name ?? h.Race}).",
            tile: v?.Tile, cause: "Cebinde birkaç gümüş, sırtında yol giysisi");
        return h;
    }

    // ------------------------------------------------------------ yerelden sime
    /// <summary>Bölgede oynanan bir kamp savaşının sonucu (Faz 2 C/A).</summary>
    public sealed class LocalFight
    {
        /// <summary>ölen sıradan goblinler (şef ayrı)</summary>
        public int Killed;
        public bool BossKilled, Cleared, Won, PlayerDowned;
        /// <summary>savaşı kazanan ekipteki kahraman kayıtları (oyuncu ve simden gelen yoldaşlar)</summary>
        public List<int> Heroes = new();
    }

    public const int SETTLERS_MIN = 10, SETTLERS_MAX = 20;

    /// <summary>
    /// Bölgede oynanan kamp savaşı sime yazılır: ölen goblinler kampın sayısından düşer, şef ölürse kamp şefsiz kalır; kamp
    /// kırıldıysa (şef düştü ve kalanlar dağıldı) sim'de de temizlenir: ilan kapanır (ödül handa ya da muhtarda alınmak üzere
    /// bekler), ganimet kampın sandığında kalır, köyün devletinin tehdidi azalır, oyuncunun kaydına yazılır ve 10–20 gün sonra
    /// boşalan vadiye öncüler gelmeye başlar (verimli vadi merkezi; yer yoksa köye yerleşirler). Oyuncuya söylenecek tek satırı döndürür (ya da null).
    /// </summary>
    public static string LocalCampFight(Sim s, LocalFight f)
    {
        var w = s.W; var link = w.Region; var cp = Camp(s); var v = Village(s);
        if (link == null || cp == null || !cp.Alive) return null;
        var c = Civ(s);
        cp.Count = JsMath.Max(0, cp.Count - f.Killed);
        if (f.BossKilled) cp.Boss = false;
        var heroes = new List<Hero>();
        foreach (int id in f.Heroes) { var h = s.Hero(id); if (h != null) heroes.Add(h); }
        foreach (var h in heroes) h.Kills += f.Killed + (f.BossKilled ? 1 : 0);
        string names = heroes.Count == 0 ? "Bir yabancı" : heroes.Count == 1 ? heroes[0].Name : string.Join(", ", J.Map(heroes, h => h.Name));
        s.Metric("localFight");
        if (!f.Cleared)
        {
            if (f.Killed + (f.BossKilled ? 1 : 0) > 0)
                s.Log("lair", $"{names} {Lore.Ek(cp.Name, "in")} goblinleriyle çarpıştı: {J.S(f.Killed + (f.BossKilled ? 1 : 0))} goblin öldü{(f.BossKilled ? ", şefleri de" : "")}.",
                    tile: cp.Tile, civ: v?.Civ, cause: f.Won ? "Goblinler bozguna uğrayıp kampa kaçtı" : "Yabancılar yere serildi", major: false);
            if (cp.Count <= 0 && !cp.Boss) f.Cleared = true;
            else return null;
        }
        // kamp temizlendi
        double loot = JsMath.Round(cp.Loot);
        cp.Alive = false; cp.Count = 0; cp.ClearedDay = s.Day;
        w.Tiles[cp.Tile].Camp = null;
        s.Metric("campCleared"); s.Metric("localCampCleared");
        link.CampLoot += loot;
        string reward = null;
        foreach (var q in w.Quests)
        {
            if (q.Camp != cp.Id || q.Done != null) continue;
            bool ours = q.Open || J.Some(q.TakenBy, id => f.Heroes.Contains(id));
            if (!ours) { q.Open = false; continue; }   // başka kahramanın ilanı boşa çıktı
            q.Open = false; q.Done = s.Day;
            s.Metric("questDone"); s.Metric("regionQuestDone");
            link.Reward += q.Bounty;
            reward = $"İlan kapandı: {J.S(q.Bounty)} altın ödül {(link.Inn >= 0 ? "handa" : "muhtarda")} seni bekliyor.";
            if (q.Civ >= 0) foreach (var h in heroes) h.Rep.Set(q.Civ, (h.Rep.Get(q.Civ) ?? 0) + 1);
        }
        if (c != null) c.Threat *= 0.4;
        foreach (var h in heroes) Will.Note(s, h, $"{Tr.Ek(cp.Name, "i")} yerle bir etti");
        link.SettlersDay = s.Day + s.Rng.Int(SETTLERS_MIN, SETTLERS_MAX);
        s.Log("lair", $"{names}, {Tr.Ek(cp.Name, "i")} yerle bir etti!{(v != null ? $" {v.Name} rahat bir nefes aldı." : "")}", tile: cp.Tile, civ: v?.Civ,
            cause: f.BossKilled ? "Şefleri düşünce goblinler dağıldı" : "Kampta ayakta goblin kalmadı", major: true);
        return reward ?? $"{cp.Name} temizlendi.";
    }

    /// <summary>D: bölgede yere serilen ekibi goblinler soydu (gümüş ve eşya değeri kampın ganimetine katılır). Tarihe küçük bir olay.</summary>
    public static void Robbed(Sim s, List<int> heroes, double silverValue, string what)
    {
        var cp = Camp(s); var v = Village(s);
        if (cp != null && cp.Alive) cp.Loot += silverValue / 10.0;
        s.Metric("localRobbed");
        string names = Names(s, heroes);
        foreach (int id in heroes) { var h = s.Hero(id); if (h != null) { Will.Note(s, h, $"{(cp != null ? Lore.Ek(cp.Name, "in") : "goblinlerin")} goblinlerine yenilip soyuldu"); h.Gold = JsMath.Max(0, h.Gold - silverValue / 10.0 / Math.Max(1, heroes.Count)); } }
        s.Log("lair", $"{(cp != null ? Lore.Ek(cp.Name, "in") : "Ormanın")} goblinleri {names} yere serip soydu.", tile: cp?.Tile ?? v?.Tile, civ: v?.Civ,
            cause: string.IsNullOrEmpty(what) ? "Baygın yolcuların keselerini boşalttılar" : $"Götürdükleri: {what}", major: false);
    }

    /// <summary>D: goblinler birini kampın kafesine kapattı.</summary>
    public static void Captured(Sim s, int? heroId, string name)
    {
        var cp = Camp(s); var v = Village(s);
        s.Metric("localCaptured");
        var h = heroId is int id ? s.Hero(id) : null;
        if (h != null) Will.Note(s, h, $"{(cp != null ? Lore.Ek(cp.Name, "in") : "goblinlerin")} kafesine kapatıldı");
        s.Log("lair", $"{(cp != null ? Lore.Ek(cp.Name, "in") : "Ormanın")} goblinleri {name} adlı yolcuyu kampın kafesine kapattı.", tile: cp?.Tile ?? v?.Tile, civ: v?.Civ,
            cause: "Fidye mi, akşam yemeği mi, belli değil", major: false);
    }

    /// <summary>D: bölgede kalıcı yara (göz, topallık, iz): kahramanın günlüğüne ve lakabına.</summary>
    public static void Wounded(Sim s, int heroId, string woundName, string epithet)
    {
        var h = s.Hero(heroId); if (h == null) return;
        Will.Note(s, h, $"kalıcı bir yara aldı: {woundName.ToLowerInvariant()}");
        if (!string.IsNullOrEmpty(epithet)) h.Epithet = epithet;
        s.Metric("localWound");
    }

    /// <summary>D: bölgede ölen kahraman (oyuncu, yoldaş): simde de ölür (olay, destan).</summary>
    public static void Died(Sim s, int heroId, string foe)
    {
        var h = s.Hero(heroId); if (h == null || h.State == "dead") return;
        var cp = Camp(s);
        h.Pos = cp?.Tile ?? Village(s)?.Tile ?? h.Pos;
        Heroes.Die(s, h, null, foe ?? cp?.Name, "local", null);
        s.Metric("localDeath");
    }

    static string Names(Sim s, List<int> ids)
    {
        var n = new List<string>();
        foreach (int id in ids) { var h = s.Hero(id); if (h != null) n.Add(Tr.Ek(h.Name, "i")); }
        return n.Count == 0 ? "yolcuları" : string.Join(", ", n);
    }

    /// <summary>F: bölgede alım satım: köyün devletinin stoğu ve hazinesi değişir (alınan mal stoktan düşer, ödenen para hazineye girer;
    /// satılan mal stoğa eklenir, hazine öder). <paramref name="units"/> malın kendi biriminde (artı: oyuncu aldı), <paramref name="gold"/>
    /// ödenen/alınan altın.</summary>
    public static void Trade(Sim s, string good, double units, double gold)
    {
        var c = Civ(s); if (c == null || D.GOODS.GetOr(good, null) == null) return;
        s.Add(c, good, -units);
        s.Add(c, "gold", units > 0 ? gold : -JsMath.Min(gold, s.St(c, "gold")));
        s.Metric(units > 0 ? "localBuy" : "localSell");
    }

    /// <summary>G: oyuncu panodan bağlı kampın ilanını kopardı (simdeki kahramanlar artık alamaz; köy yeni ilan asmaz).</summary>
    public static Quest TakeQuest(Sim s)
    {
        var q = CampQuest(s); var pl = Player(s);
        if (q == null || pl == null) return null;
        q.Open = false;
        q.TakenBy = new List<int> { pl.Id };
        q.Expires = null;
        Will.Note(s, pl, $"{J.S(q.Bounty)} altınlık ilanı panodan kopardı");
        s.Metric("playerQuest");
        var cp = Camp(s);
        s.Log("quest", $"{pl.Name}, {(cp != null ? Lore.Ek(cp.Name, "in") : "kampın")} ilanını panodan kopardı.", tile: Village(s)?.Tile, civ: Village(s)?.Civ, major: false,
            cause: $"Ödül {J.S(q.Bounty)} altın");
        return q;
    }

    /// <summary>
    /// G: simdeki kahraman grubu bölgede kampla dövüştü (<see cref="Sim.LocalCamp"/>): sonuç simdeki savaş gibi yazılır. Kahramanlar
    /// bölgedeki savaştan kalan canlarıyla savaşan sayılır (ölen ölür, XP ve ün <c>AfterBattle</c>'dan), ölen goblinler kamptan düşer;
    /// kamp kırıldıysa temizlenir, ilan kapanır ve ödül kahramanlara ödenir; kırılmadıysa ilan "başarısız" olur ve ödül artar. Grup
    /// yuvasına döner.
    /// </summary>
    public static string LocalBandResult(Sim s, List<int> agentIds, Dictionary<int, double> heroHp, int killed, bool bossKilled, bool cleared, Dictionary<int, int> heroKills = null)
    {
        var w = s.W; var link = w.Region; var cp = Camp(s); var v = Village(s);
        var band = new List<Agent>();
        foreach (int id in agentIds) { var a = J.Find(w.Agents, x => x.Id == id && !J.T(x.Dead)); if (a != null) band.Add(a); }
        if (cp == null || band.Count == 0) return null;
        var cs = new List<Combatant>();
        foreach (var a in band)
            foreach (int hid in a.Heroes ?? new List<int>())
            {
                var h = s.Hero(hid); if (h == null || h.State == "dead") continue;
                var c = Combat.HeroCombatant(h, "A");
                if (heroHp.TryGetValue(hid, out var hp)) c.Hp = hp;
                if (heroKills != null && heroKills.TryGetValue(hid, out var k)) c.Kills = k;
                cs.Add(c);
            }
        var foes = new List<Combatant>();
        for (int i = 0; i < killed; i++) { var u = Combat.Unit(D.MONSTERS["goblin"], "B", "monster"); u.Hp = 0; foes.Add(u); }
        if (bossKilled) { var u = Combat.Unit(D.MONSTERS["goblinBoss"], "B", "boss"); u.Hp = 0; foes.Add(u); }
        Agents.SyncHeroes(s, cs, cleared ? 100 : 20, foes, cp.Name, "camp");
        var alive = J.Filter(cs, c => c.Hero != null && c.Hero.State != "dead");
        string names = alive.Count > 0 ? string.Join(", ", J.Map(alive, c => c.Hero.Name)) : string.Join(", ", J.Map(cs, c => c.Hero.Name));
        cp.Count = JsMath.Max(0, cp.Count - killed);
        if (bossKilled) cp.Boss = false;
        s.Metric("localBandFight");
        string line;
        if (cleared && alive.Count > 0)
        {
            double loot = JsMath.Round(cp.Loot);
            cp.Alive = false; cp.Count = 0; cp.ClearedDay = s.Day;
            w.Tiles[cp.Tile].Camp = null;
            s.Metric("campCleared"); s.Metric("localBandCleared");
            link.CampLoot += loot * 0.5;   // kahramanlar ganimetin yarısını götürür, yarısı sandıkta kalır
            foreach (var a in band)
            {
                var q = a.Quest is int qid ? J.Find(w.Quests, x => x.Id == qid) : null;
                if (q == null) continue;
                q.Done = s.Day; q.Open = false;
                s.Metric("questDone");
                double each = Math.Floor((q.Bounty + loot * 0.25) / Math.Max(1, alive.Count));
                foreach (var c in alive) { c.Hero.Gold += each; if (q.Civ >= 0) c.Hero.Rep.Set(q.Civ, (c.Hero.Rep.Get(q.Civ) ?? 0) + 1); Heroes.QuestDone(s, c.Hero, q, cp); }
            }
            foreach (var oq in w.Quests) if (oq.Camp == cp.Id && oq.Open) oq.Open = false;
            var civ = Civ(s); if (civ != null) civ.Threat *= 0.4;
            link.SettlersDay = s.Day + s.Rng.Int(SETTLERS_MIN, SETTLERS_MAX);
            s.Log("lair", $"{names}, {Tr.Ek(cp.Name, "i")} yerle bir etti!{(v != null ? $" {v.Name} rahat bir nefes aldı." : "")}", tile: cp.Tile, civ: v?.Civ,
                cause: "İlanı panodan koparıp kampa yürüdüler", major: true);
            line = $"{names}, {Tr.Ek(cp.Name, "i")} temizledi.";
        }
        else
        {
            foreach (var a in band) { var q = a.Quest is int qid ? J.Find(w.Quests, x => x.Id == qid) : null; if (q != null && q.Done == null) Heroes.QuestFailed(s, q); }
            s.Log("lair", $"{names}, {Tr.Ek(cp.Name, "da")} püskürtüldü.", tile: cp.Tile, civ: v?.Civ,
                cause: $"Goblinler {J.S(cp.Count)} kişiyle kampı tuttu{(killed > 0 ? $"; {J.S(killed)} goblin öldü" : "")}", major: true);
            line = $"{names}, {Tr.Ek(cp.Name, "da")} püskürtüldü.";
        }
        foreach (var a in band) { a.Muster = null; Agents.PartyReturn(s, a); }
        return line;
    }

    /// <summary>G: oyuncu bekleyen ilan ödülünü handa ya da muhtarda aldı (altın).</summary>
    public static double CollectReward(Sim s)
    {
        var link = s.W.Region; if (link == null || link.Reward <= 0) return 0;
        double r = link.Reward; link.Reward = 0;
        var pl = Player(s); var v = Village(s);
        if (pl != null) { pl.Gold += r; Will.Note(s, pl, $"{J.S(r)} altın ilan ödülünü aldı"); }
        s.Metric("regionRewardPaid");
        s.Log("quest", $"{(pl != null ? pl.Name : "Bir yabancı")} {J.S(r)} altınlık ödülünü aldı.", tile: v?.Tile, civ: v?.Civ, major: false);
        return r;
    }

    /// <summary>E: handaki serbest kahraman oyuncunun ekibine kiralandı (State "party": simin yapay zekâsı ona dokunmaz, han havuzunda
    /// sayılmaz).</summary>
    public static void Hire(Sim s, int heroId, double wageSilver)
    {
        var link = s.W.Region; var h = s.Hero(heroId); var pl = Player(s);
        if (link == null || h == null) return;
        h.State = "party"; h.Goal = null; h.Auction = null;
        if (!link.Party.Contains(h.Id)) link.Party.Add(h.Id);
        Will.Note(s, h, $"{(pl != null ? Tr.Ek(pl.Name, "a") : "bir yabancıya")} haftalığı {J.S(wageSilver)} gümüşe yoldaş oldu");
        s.Metric("partyHire");
        s.Log("hero", $"{h.Name}, {(pl != null ? pl.Name : "bir yabancı")} ile yola çıktı.", tile: Village(s)?.Tile, cause: $"Haftalığı {J.S(wageSilver)} gümüş", major: false);
    }

    /// <summary>E: yoldaş ekipten ayrıldı (maaş ödenmedi, hizalama uyuşmadı): hana döner, yeniden iş bekler.</summary>
    public static void Dismiss(Sim s, int heroId, string why)
    {
        var link = s.W.Region; var h = s.Hero(heroId); var inn = Inn(s);
        if (link == null || h == null) return;
        link.Party.Remove(h.Id);
        if (h.State == "dead") return;
        h.State = "tavern"; h.Civ = -1;
        if (inn != null) { h.Base = inn.Id; h.BaseInn = true; h.Pos = inn.Tile; }
        h.IdleSince = s.Day;
        Will.Note(s, h, $"ekipten ayrıldı: {why}");
        s.Metric("partyLeave");
    }

    /// <summary>D: oyuncu öldü, ekipten biri başa geçti: simde artık o "oyuncu"dur (State "player", bağ onu gösterir).</summary>
    public static void Promote(Sim s, Hero h)
    {
        var link = s.W.Region; if (link == null || h == null) return;
        h.State = "player"; h.Civ = -1; h.Tavern = -1; h.Base = -1; h.BaseInn = false;
        link.Player = h.Id;
        link.Party.Remove(h.Id);
        Will.Note(s, h, "ekibin başına geçti");
        s.Metric("playerPromoted");
    }

    /// <summary>Gün başında (Sim.Step'ten sonra) bölgenin bakımı: kamp yaşıyorsa ilan; temizlenen kampın vadisine vakti gelince öncüler.</summary>
    public static void DayTick(Sim s)
    {
        var link = s.W.Region;
        if (link == null) return;
        EnsureQuest(s);
        if (link.SettlersDay is double sd && s.Day >= sd)
        {
            link.SettlersDay = null;
            var v = Village(s); var cp = Camp(s);
            if (cp != null && Hubs.ForceValley(s, cp)) { s.Metric("regionValley"); return; }
            // vadiye yer yoksa öncüler köye yerleşir
            if (v != null && v.Alive)
            {
                var c = Civ(s);
                string race = c?.Race ?? "human";
                double n = s.Rng.Int(4, 8);
                s.AddPop(v, race, n);
                s.Metric("regionSettlers");
                s.Log("hub", $"{(cp != null ? Tr.Ek(cp.Name, "in") : "Goblinlerin")} boşalttığı ormana öncüler geldi; {Tr.Ek(v.Name, "a")} {J.S(n)} yeni can katıldı.",
                    tile: v.Tile, civ: v.Civ, cause: "Goblinler gidince ormanın kıyısı güvenli oldu", major: false);
            }
        }
    }
}
