using System;
using System.Collections.Generic;
using System.Linq;

// Goblin istilası: büyüyen, yayılan, üst kademeye evrilen kamplar. Port of src/sim/monsters.ts.
// Faz 1 B2: hedef kamp sayısı (Faz 1b-4: sabit bant CAMP_LOW–CAMP_HIGH; eksikse yeni inler birer birer gelir, tür yıla göre), trol çeteleri
// (15. yıldan sonra; güçlü, yaralarını kapatır, ateş durdurur) ve ejderhanın kampı (Kind "dragon"; davranışı Dragon.cs).

namespace FD.Macro;

public static class Monsters
{
    private static readonly JsObj<double> CAP = new JsObj<double> { ["goblin"] = 14, ["hobgoblin"] = 10, ["bugbear"] = 4, ["pirate"] = 14, ["troll"] = 5, ["dragon"] = 1 };
    /// <summary>kampın günlük büyümesi (Faz 1b-5: eski günde goblin 0,035 …; ×PACE)</summary>
    private static readonly JsObj<double> GROW = new JsObj<double> { ["goblin"] = 0.035 * Sim.PACE, ["hobgoblin"] = 0.028 * Sim.PACE, ["bugbear"] = 0.008 * Sim.PACE, ["pirate"] = 0.022 * Sim.PACE, ["troll"] = 0.006 * Sim.PACE, ["dragon"] = 0 };
    /// <summary>Faz 1b-5: eski günlük olasılık p'nin yeni gündeki karşılığı (1 − (1 − p)^PACE)</summary>
    internal static double Daily(double p) => 1 - JsMath.Pow(1 - p, Sim.PACE);

    /// <summary>Kamp türünün savaşçıları: n canavar (+ bugbear, trol ve ejderha dışında önder), hepsi kötü (evil). Ejderha önder
    /// (boss) bayrağı taşır; adı ve kalıcı yaraları için kamptan <see cref="CampSide"/> kullanılır.</summary>
    public static List<Combatant> MonsterSide(string kind, double n, bool boss, string side)
    {
        var cs = new List<Combatant>();
        UnitStats @base = kind == "goblin" ? D.MONSTERS["goblin"] : kind == "hobgoblin" ? D.MONSTERS["hobgoblin"] : kind == "pirate" ? D.MONSTERS["pirate"]
            : kind == "troll" ? D.MONSTERS["troll"] : kind == "dragon" ? D.MONSTERS["dragon"] : D.MONSTERS["bugbear"];
        for (int i = 0; i < n; i++) { var u = Combat.Unit(@base, side, "monster"); u.Evil = true; if (kind == "dragon") u.Boss = true; cs.Add(u); }
        if (boss && kind != "bugbear" && kind != "troll" && kind != "dragon") { var u = Combat.Unit(kind == "goblin" ? D.MONSTERS["goblinBoss"] : kind == "pirate" ? D.MONSTERS["pirateCaptain"] : D.MONSTERS["hobCaptain"], side, "boss"); u.Evil = true; cs.Add(u); }
        return cs;
    }

    /// <summary>Kampın savunucuları (inine saldırılınca): ejderha adıyla ve yaralarıyla; diğerleri <see cref="MonsterSide"/>, inin
    /// tahkimatı (<see cref="Fort"/>) zırhlarına eklenir.</summary>
    public static List<Combatant> CampSide(Sim s, Camp cp, string side)
    {
        if (cp.Kind == "dragon") return Dragon.Side(s, cp, side);
        var cs = MonsterSide(cp.Kind, cp.Count, cp.Boss, side);
        double f = Fort(s, cp);
        if (f > 0) foreach (var u in cs) u.Ac += f;
        return cs;
    }

    /// <summary>Kampın bütün gücü (güç tahmini için): akında olanlar dâhil, tahkimatıyla; ejderha adıyla ve yaralarıyla.</summary>
    public static List<Combatant> ForceSide(Sim s, Camp cp)
    {
        if (cp.Kind == "dragon") return Dragon.Side(s, cp, "B");
        var cs = MonsterSide(cp.Kind, cp.Count + CampAway(s, cp), cp.Boss, "B");
        double f = Fort(s, cp);
        if (f > 0) foreach (var u in cs) u.Ac += f;
        return cs;
    }

    /// <summary>İnin tahkimatı (Faz 1 B2): yerleşmiş in yıllar geçtikçe hendek, kazık ve tuzakla çevrilir; her 2 yılda +1 zırh, en çok
    /// +4. Yalnız inini savunurken sayılır (akına çıkanlar surlarını arkada bırakır). Dünyanın ilk 15 yılında in tahkim edilmez
    /// (sayaç 15. yılda başlar); korsan koyu ve ejderha tahkim edilmez. Eski inler sıradan kahraman gruplarının harcı olmaz.</summary>
    public static double Fort(Sim s, Camp cp)
    {
        if (cp.Kind == "pirate" || cp.Kind == "dragon") return 0;
        double age = s.Day - JsMath.Max(cp.Founded, FORT_FROM * Sim.OLD_YEAR);
        return age <= 0 ? 0 : JsMath.Min(FORT_MAX, Math.Floor(age / (FORT_YEARS * Sim.OLD_YEAR)));
    }

    /// <summary>tahkimat: sayacın başladığı (dinamik) yıl, bir kademe için (dinamik) yıl, en çok kademe (zırh)</summary>
    public const double FORT_FROM = 15, FORT_YEARS = 2, FORT_MAX = 4;

    /// <summary>gizli in (Faz 1 B2) bu kadar gün sonra söylentilerle bilinir olur (ilk akını ya da bir kâşif onu daha önce açığa çıkarabilir)</summary>
    public const double HIDE_DAYS = Sim.OLD_YEAR;

    /// <summary>Gizli in bilinir olur: kahramanlar, hanlar ve medeniyetler artık onu hedef alabilir. <paramref name="by"/>: onu gören
    /// (kâşifler); null ise ilk akını ya da söylentiler.</summary>
    public static void Reveal(Sim s, Camp c, string by)
    {
        if (!J.T(c.Hidden)) return;
        c.Hidden = null;
        s.Metric("lairRevealed");
        s.Log("lair", by != null ? $"{by}, şimdiye dek kimsenin bilmediği {Lore.Ek(c.Name, "i")} buldu: {J.S(c.Count)} {J.TrLower(MonsterName(c.Kind, false))}."
                : $"{Lore.Ek(c.Name, "in")} yeri ortaya çıktı: {J.S(c.Count)} {J.TrLower(MonsterName(c.Kind, false))} orada gizlice güçlenmiş.",
            tile: c.Tile, cause: by != null ? "Kâşifler ıssız sınır boylarını dolaşıyor" : "Akıncıların izleri ve köylülerin fısıltıları");
    }

    /// <summary>Canavar türünün adı: çoğul ("Goblinler") ya da tekil ("Goblin").</summary>
    public static string MonsterName(string kind, bool plural = true) =>
        kind == "goblin" ? (plural ? "Goblinler" : "Goblin") : kind == "hobgoblin" ? (plural ? "Hobgoblinler" : "Hobgoblin") : kind == "pirate" ? (plural ? "Korsanlar" : "Korsan")
        : kind == "troll" ? (plural ? "Troller" : "Trol") : kind == "dragon" ? "Ejderha" : (plural ? "Bugbearlar" : "Bugbear");

    /// <summary>Kamptan akında olan (ölmemiş raid ajanlarındaki) canavar sayısı.</summary>
    public static double CampAway(Sim s, Camp c)
    {
        double n = 0;
        foreach (var a in J.Filter(s.W.Agents, a => a.Kind == "raid" && a.From == c.Id && !J.T(a.Dead))) n = n + (a.Troops ?? 0);
        return n;
    }

    /// <summary>Günlük kamp güncellemesi: büyüme, şef, yayılma, hobgoblin evrimi ve işgali, korsan kaptanı, akınlar, bugbear ini,
    /// hedef kamp sayısına göre yeni inler (Faz 1 B2). Ejderhanın kampı burada değil, Dragon.Tick'te yaşar.</summary>
    public static void CampsTick(Sim s)
    {
        var w = s.W;
        var alive = J.Filter(w.Camps, c => c.Alive);
        foreach (var c in alive)
        {
            if (c.Kind == "dragon") continue;
            if (J.T(c.Hidden) && s.Day - c.Founded >= HIDE_DAYS) Reveal(s, c, null);   // söylentiler yayıldı
            double away = CampAway(s, c);
            c.GrowthAcc += J.N(GROW.Get(c.Kind)) + JsMath.Min(0.02, c.Loot / 3000) * Sim.PACE;
            if (c.GrowthAcc >= 1 && c.Count + away < Cap(s, c.Kind)) { c.GrowthAcc -= 1; c.Count++; if (c.GrowthAcc >= 1) c.GrowthAcc = 0; }
            bool bossAway = J.Some(w.Agents, a => a.Kind == "raid" && a.From == c.Id && J.T(a.Boss));
            if (c.Kind == "goblin" && !c.Boss && !bossAway && !c.HadBoss && c.Count >= 11)
            {
                c.HadBoss = true; c.Boss = true;
                s.Log("lair", $"{Tr.Ek(c.Name, "da")} goblinlerin başına acımasız bir şef geçti.", tile: c.Tile, cause: $"Kamp kalabalıklaştı ({J.S(c.Count)} goblin)", major: true);
            }
            // yayılma
            if (c.Kind == "goblin" && c.Count >= 12 && J.Filter(w.Camps, x => x.Alive).Count < 10 && LandCamps(w.Camps) < CAMP_LOW && s.Rng.Chance(Daily(1.0 / 200)))   // Faz 1b-4: yalnız bandın altındayken
            {
                var avoid = J.Map(J.Filter(w.Settlements, x => x.Alive), x => x.Tile);
                avoid.AddRange(J.Map(alive, x => x.Tile));
                var near = J.Filter(s.G.Within(c.Tile, 10), t => s.G.Dist(t, c.Tile) >= 6);
                int t = near.Count > 0 ? PickNear(s, near, avoid) : -1;
                if (t >= 0)
                {
                    var nc = WorldGen.MakeCamp(s.Id(), "goblin", t, FreshName(s, WorldGen.GOBLIN_CAMP_NAMES), s.Day, s.Rng);
                    nc.NextRaid = s.Day + s.Rng.Int(25, 45);   // Faz 1b-5: eski 100–180 gün
                    c.Count -= 5; nc.Count = 5;
                    w.Camps.Add(nc); w.Tiles[t].Camp = nc.Id; w.Tiles[t].Owner = -1;
                    s.Metric("campSpread");
                    s.Log("lair", $"{c.Name} kalabalıklaştı; goblinler {nc.Name} adıyla yeni bir kamp kurdu.", tile: t, major: true, cause: "Goblin istilası yayılıyor");
                }
            }
            // hobgoblin karakoluna evrilme
            int goblinCamps = J.Filter(alive, x => x.Kind == "goblin").Count;
            int hobs = J.Filter(alive, x => x.Kind == "hobgoblin").Count;
            if (c.Kind == "goblin" && hobs < 3 && s.Day > 4 * Sim.OLD_YEAR && (s.Day - c.Founded > 3 * Sim.OLD_YEAR || goblinCamps >= 4) && c.Count >= 10 && s.Rng.Chance(Daily(1.0 / 500)))
            {
                c.Kind = "hobgoblin";
                c.Name = FreshName(s, WorldGen.HOB_NAMES);
                c.Count = JsMath.Max(6, Math.Floor(c.Count * 0.6));
                c.Boss = true; c.HadBoss = true;
                s.Metric("hobgoblin");
                s.Log("lair", $"Hobgoblin lejyonerleri kampı ele geçirdi: {c.Name} kuruldu.", tile: c.Tile, major: true, cause: "Uzun süre temizlenmeyen goblin kampı disiplinli bir orduya dönüştü");
            }
            // hobgoblin işgali: terk edilmiş yerleşim ya da sahipsiz yatak
            if (c.Kind == "hobgoblin" && c.Count >= 10 && hobs < 4 && J.Filter(w.Camps, x => x.Alive).Count < 7 && LandCamps(w.Camps) < CAMP_LOW && s.Rng.Chance(Daily(1.0 / 300)))
            {
                var cand = J.Map(J.Filter(w.Settlements, x => !x.Alive && s.G.Dist(x.Tile, c.Tile) <= 12 && w.Tiles[x.Tile].Owner < 0 && w.Tiles[x.Tile].Camp == null && w.Tiles[x.Tile].InnZone == null), x => x.Tile);
                var depTiles = new List<int>();
                foreach (var d in J.Filter(w.Deposits, d => !d.Depleted)) depTiles.AddRange(d.Tiles);
                cand.AddRange(J.Filter(depTiles, t => s.G.Dist(t, c.Tile) <= 10 && s.G.Dist(t, c.Tile) >= 4 && w.Tiles[t].Owner < 0 && w.Tiles[t].Camp == null && w.Tiles[t].InnZone == null && w.Tiles[t].Terrain != "mountain"));
                if (cand.Count > 0)
                {
                    int t = s.Rng.Pick(cand);
                    var nc = WorldGen.MakeCamp(s.Id(), "hobgoblin", t, FreshName(s, WorldGen.HOB_NAMES), s.Day, s.Rng);
                    c.Count -= 5; nc.Count = 5;
                    w.Camps.Add(nc); w.Tiles[t].Camp = nc.Id;
                    s.Metric("occupation");
                    s.Log("lair", $"Hobgoblinler {(w.Tiles[t].Deposit >= 0 ? "sahipsiz bir yatağı" : "terk edilmiş bir yerleşimi")} işgal etti.", tile: t, major: true, cause: $"{c.Name} genişliyor");
                }
            }
            if (c.Kind == "pirate")
            {
                if (!c.Boss && !bossAway && c.Count >= 9 && s.Rng.Chance(Daily(1.0 / 120))) { c.Boss = true; c.Captain = Sea.NewCaptain(s); s.Log("lair", $"{Tr.Ek(c.Name, "da")} korsanlar yeni kaptanlarını seçti: {c.Captain}.", tile: c.Tile, major: true, cause: $"Koy kalabalıklaştı ({J.S(c.Count)} korsan)"); }
                if (s.Day >= c.NextRaid && c.Count >= 5) Sea.LaunchPirates(s, c);
                continue;
            }
            if (s.Day >= c.NextRaid && c.Count >= (c.Kind == "goblin" ? 7 : c.Kind == "hobgoblin" ? 6 : 2)) LaunchRaid(s, c);
        }
        // bugbear ini
        if (s.DynYear >= 4 && !J.Some(alive, c => c.Kind == "bugbear") && CampRoom(s) > 0 && s.Rng.Chance(Daily(1.0 / 500)))
        {
            var forest = new List<int>();
            for (int i = 0; i < w.Tiles.Count; i++)
            {
                var t = w.Tiles[i];
                int v = (t.Terrain == "forest" || t.Terrain == "oldforest") && t.Owner < 0 && t.Camp == null && t.InnZone == null && !J.T(t.Isle) ? i : -1;
                if (v >= 0) forest.Add(v);
            }
            var avoid = J.Map(J.Filter(w.Settlements, x => x.Alive), x => x.Tile);
            int tt = PickNear(s, forest, avoid, 9);
            if (tt >= 0)
            {
                var c = WorldGen.MakeCamp(s.Id(), "bugbear", tt, FreshName(s, WorldGen.BUGBEAR_NAMES), s.Day, s.Rng);
                w.Camps.Add(c); w.Tiles[tt].Camp = c.Id;
                s.Log("lair", $"Ormanın derinliklerinde bir bugbear ini belirdi: {c.Name}.", tile: tt, major: true, cause: "Yolcuları ve kahramanları pusuya düşürürler");
            }
        }
        // Faz 1b-4: kamp bandı (eskiden büyüyen hedef 3 + yıl/6; Faz 1 B2): kara kampları (korsan koyu ve ejderha sayılmaz)
        // CAMP_LOW'un altındaysa yeni inler birer birer, en az 20–40 gün arayla gelir (eskiden 40–80); eksik büyüdükçe daha çabuk; goblin
        // yayılması ve hobgoblin işgali de yalnız bandın altında. Bandın içinde yalnız bugbear ini ve anlatıcının krizleri (istila, trol
        // çetesi) CAMP_HIGH'a dek in açabilir (bkz. CampRoom).
        // Tür yıla göre: önce goblinler, sonra hobgoblin, bugbear ve (15. yıldan sonra) trol.
        var story = Storyteller.State(s);
        int land = LandCamps(alive);
        double target = CampTarget(s);
        if (land < target && s.Day >= story.NextCampSpawn && s.Rng.Chance(Daily(JsMath.Min(0.25, (target - land) / CAMP_REFILL))))
        {
            var nc = SpawnLair(s, LateKind(s, alive), null, true, land == 0 ? "Boşalan topraklar yeni yağmacıları çekti" : null, 12, true);
            story.NextCampSpawn = s.Day + (nc != null ? s.Rng.Int(1, 3) : 1);   // Faz 1b-5: eski 20–40 eski gün (1–3 gün); kahramanlar daha çok kamp temizliyor
        }
    }

    // ------------------------------------------------------------ kamp bandı ve yeni inler (Faz 1 B2, Faz 1b-4)
    /// <summary>Faz 1b-4: kamp bandı (yaşayan kara kampı; korsan koyu ve ejderha sayılmaz). Dünya büyüdükçe artan hedefin yerine sabit
    /// bant: altında yeni inler gelir, üstüne hiçbir yoldan çıkılmaz. Ölçümle seçildi: f1b-3'te kara kampı 1–10. yıllarda ~7–9,5 (erken
    /// goblin yayılması), 11–40. yıllarda ~6, 51–60. yıllarda ~9; bantla bütün yıllarda ~8 (geç yıllarda ejderha ve korsanlarla ~9).
    /// Canavar baskısı yıllar boyu aynı kalır; geç inler büyüklükleri ve trollerle sertleşir. Dar tutuldu: üst sınırı erken goblin
    /// yayılması doldurursa erken yıllar geç yıllardan kalabalık olur (ölçüt 3).</summary>
    public const int CAMP_LOW = 8, CAMP_HIGH = 10;
    /// <summary>bandın altında eski günlük yeni in olasılığı eksik / CAMP_REFILL (en çok %25; eskiden eksik / 40, Faz 1b-4'te / 15): temizlenen
    /// in çabuk yerine gelir (Faz 1b-5: yeni günde <see cref="Daily"/>; kahramanlar daha çok kamp temizlediği için 15 → 4)</summary>
    public const double CAMP_REFILL = 4;

    /// <summary>bandın alt sınırı: hedef kara kampı sayısı (eskiden ⌊3 + yıl/6⌋)</summary>
    public static double CampTarget(Sim s) => CAMP_LOW;

    /// <summary>yaşayan kara kampı (korsan koyu ve ejderha hariç) sayısı</summary>
    public static int LandCamps(List<Camp> camps)
    {
        int n = 0;
        foreach (var c in camps) if (c.Alive && c.Kind != "pirate" && c.Kind != "dragon") n++;
        return n;
    }

    /// <summary>Faz 1b-4: bandın üst sınırına dek açılabilecek kara kampı (bugbear ini, istila ve trol krizi bunu aşamaz; yayılma ve işgal
    /// zaten yalnız CAMP_LOW'un altında).</summary>
    public static int CampRoom(Sim s) => CAMP_HIGH - LandCamps(s.W.Camps);

    /// <summary>Kamp türünün nüfus tavanı; 20. yıldan sonra dünya yaşlandıkça büyür (geç gelen inler gerçek tehdit olur, sıradan
    /// bir kahraman grubu onları kolayca deviremez): goblin 14 + (yıl−20)/4 (en çok 24), hobgoblin 10 + (yıl−20)/5 (en çok 18),
    /// bugbear 4 + (yıl−20)/20, trol 5 + (yıl−20)/15; korsan ve ejderha sabit (bilinmeyen tür: NaN).</summary>
    public static double Cap(Sim s, string kind)
    {
        double late = JsMath.Max(0, s.DynYear - 20);
        return kind switch
        {
            "goblin" => JsMath.Min(24, 14 + Math.Floor(late / 4)),
            "hobgoblin" => JsMath.Min(18, 10 + Math.Floor(late / 5)),
            "bugbear" => 4 + Math.Floor(late / 20),
            "troll" => 5 + Math.Floor(late / 15),
            _ => J.N(CAP.Get(kind)),
        };
    }

    private static readonly List<string> LATE_KINDS = new() { "goblin", "hobgoblin", "bugbear", "troll" };

    /// <summary>Yeni inin türü yıla göre: ilk yıllarda goblin; 8. yıldan sonra hobgoblin; bugbear (en çok 2); 15. yıldan sonra
    /// trol (en çok 3 çete, ağırlığı yıllarla artar).</summary>
    public static string LateKind(Sim s, List<Camp> alive)
    {
        double y = s.DynYear;
        int trolls = 0, bugs = 0;
        foreach (var c in alive) { if (!c.Alive) continue; if (c.Kind == "troll") trolls++; if (c.Kind == "bugbear") bugs++; }
        return s.Rng.Weighted(LATE_KINDS, k => k switch
        {
            "goblin" => y < 10 ? 1 : y < 25 ? 0.5 : 0.25,
            "hobgoblin" => y < 8 ? 0 : y < 25 ? 0.35 : 0.4,
            "bugbear" => y < 6 || bugs >= 2 ? 0 : 0.12,
            "troll" => y < 15 || trolls >= 3 ? 0 : 0.1 + 0.006 * (y - 15),
            _ => 0,
        });
    }

    /// <summary>15. yıldan sonra gelen inler daha kalabalık başlar: goblin 5 + (yıl−15)/6 (en çok 12), hobgoblin 7 + (yıl−15)/8
    /// (en çok 12), trol 3 (35. yıldan sonra 4), bugbear 3 (40. yıldan sonra 4).</summary>
    private static double StartCount(Sim s, string kind)
    {
        double late = JsMath.Max(0, s.DynYear - 15);
        return kind switch
        {
            "goblin" => JsMath.Min(12, 5 + Math.Floor(late / 6)),
            "hobgoblin" => JsMath.Min(12, 7 + Math.Floor(late / 8)),
            "troll" => s.DynYear >= 35 ? 4 : 3,
            _ => s.DynYear >= 40 ? 4 : 3,
        };
    }

    // geç gelen inlerin ad havuzları (eski adlar önce; kullanılmayanlardan zarla seçilir, bitince Roma rakamı eklenir)
    private static readonly List<string> GOBLIN_MORE = new() { "Pistırnak Kampı", "Yamukburun Kampı", "Kuruçalı Kampı", "Kırmızıgöz Kampı", "Leşyiyen Kampı", "Taşatan Kampı", "Çamurdiş Kampı", "Dikenli Kampı", "Kanlıtırnak Kampı", "Sinsikuyruk Kampı", "Kemirgen Kampı", "Karabıçak Kampı", "Yarıkkulak Kampı", "Bataklıkdiş Kampı", "Çatlakkafa Kampı", "Pislikçukuru Kampı" };
    private static readonly List<string> HOB_MORE = new() { "Demirkapı Karakolu", "Kanlıkalkan Karakolu", "Karamızrak Karakolu", "Taşyürek Karakolu", "Kızılmiğfer Karakolu", "Çeliksöz Karakolu", "Kara Kartal Karakolu", "Ateşsancak Karakolu", "Gürzbaş Karakolu", "Demirtuğ Karakolu", "Kemiksancak Karakolu", "Karadavul Karakolu" };
    public static readonly List<string> TROLL_NAMES = new() { "Taşyumruk İni", "Yosunsırt İni", "Kemikkıran İni", "Kayadiş İni", "Çamurpençe İni", "Karakök İni", "Gecegöz İni", "Kanlıçene İni", "Balçıkpençe İni", "Yarıkdiş İni", "Taşkafa İni", "Kökyiyen İni", "Kayakemik İni", "Sisgöz İni", "Kancaparmak İni" };
    private static readonly List<string> BUGBEAR_MORE = new() { "Sessizpençe İni", "Kıllıgölge İni", "Dikenkulak İni", "Sinsiadım İni", "Karayele İni", "Gölgepençe İni", "Kürkgölge İni", "Pusukulak İni", "Sessizdiş İni" };

    private static List<string> NamesFor(string kind)
    {
        List<string> a = kind == "goblin" ? WorldGen.GOBLIN_CAMP_NAMES : kind == "hobgoblin" ? WorldGen.HOB_NAMES : null;
        List<string> b = kind == "goblin" ? GOBLIN_MORE : kind == "hobgoblin" ? HOB_MORE : kind == "troll" ? TROLL_NAMES : BUGBEAR_MORE;
        if (a == null) return b;
        var all = new List<string>(a);
        all.AddRange(b);
        return all;
    }

    /// <summary>Kullanılmamış adlardan zarla (hiç kalmadıysa <see cref="FreshName"/>: Roma rakamıyla).</summary>
    private static string FreshPick(Sim s, List<string> list)
    {
        var used = new HashSet<string>();
        foreach (var c in s.W.Camps) used.Add(c.Name);
        var free = J.Filter(list, n => !used.Contains(n));
        return free.Count > 0 ? s.Rng.Pick(free) : FreshName(s, list);
    }

    private static string LairText(string kind, string name) => kind switch
    {
        "goblin" => $"Dağlardan yeni bir goblin kabilesi indi ve {Lore.Ek(name, "i")} kurdu.",
        "hobgoblin" => $"Bir hobgoblin bölüğü sınır boylarında {Lore.Ek(name, "i")} kurdu.",
        "troll" => $"Dağlardan bir trol çetesi indi ve {Lore.Ek(name, "a")} yerleşti.",
        _ => $"Ormanın derinliklerinde bir bugbear ini belirdi: {name}.",
    };

    private static string LairCause(string kind) => kind switch
    {
        "goblin" => "Issız sınır boyları yeni yağmacıları çekti",
        "hobgoblin" => "Hobgoblin lejyonları sınır boylarında üs arıyor",
        "troll" => "Troller yaralarını kapatır; onları ancak ateş durdurur",
        _ => "Yolcuları ve kahramanları pusuya düşürürler",
    };

    /// <summary>Yeni in kurar (tür, isteğe bağlı bir medeniyetin sınırına yakın): yer seçimi <see cref="PickLairTile"/>; inler
    /// yerleşimlerden en az <paramref name="minD"/> fersah uzakta, ıssız sınır boylarında kurulur (12: sıradan medeniyet seferinin
    /// "sınıra fazla yakın" diye kendiliğinden geldiği 11 fersahın dışında, ama akın menzilinde); yer yoksa daraltarak. Başlangıç
    /// kalabalığı yıla göre. <paramref name="log"/> ise "lair" büyük olayı yazılır. Yer bulunamazsa null.</summary>
    public static Camp SpawnLair(Sim s, string kind, Civ near, bool log, string cause, double minD = 12, bool hidden = false)
    {
        var w = s.W;
        bool hills = kind == "troll";
        int t = PickLairTile(s, minD, minD + 4, near, hills);
        if (t < 0) t = PickLairTile(s, minD - 1, minD + 6, near, hills);
        if (t < 0) t = PickLairTile(s, minD - 3, minD + 8, null, hills);
        if (t < 0) t = PickLairTile(s, 6, 24, null, hills);   // kalabalık dünyada kıyıda köşede de olsa yer bulurlar
        if (t < 0) return null;
        var c = WorldGen.MakeCamp(s.Id(), kind, t, FreshPick(s, NamesFor(kind)), s.Day, s.Rng);
        c.Count = StartCount(s, kind);
        if (kind == "troll") c.NextRaid = s.Day + s.Rng.Int(8, 22);   // troller aç gelir (Faz 1b-5: eski 30–90 gün)
        if (hidden) c.Hidden = true;
        w.Camps.Add(c); w.Tiles[t].Camp = c.Id; w.Tiles[t].Owner = -1;
        s.Metric("lairSpawn"); s.Metric("lairSpawn" + Heroes.UpperFirst(kind));
        if (log) s.Log("lair", LairText(kind, c.Name), tile: t, major: true, cause: cause ?? LairCause(kind));
        return c;
    }

    /// <summary>
    /// Yeni in için karo (bütün harita taranır): su, dağ, sahipli, kamp, çıkarma yapısı, ada, han ve han halkası değil; en yakın
    /// yaşayan yerleşime uzaklığı [minD, maxD]; başka bir kampa en az 5 fersah. Puan: yerleşime minD+2 uzaklık, orman/tepe,
    /// çevredeki kamp kalabalığı (eksi), <paramref name="hills"/> ise dağ eteği; <paramref name="near"/> verilirse o medeniyete
    /// yakınlık. En iyi 10 aday arasından puana göre zarla seçilir; aday yoksa -1.
    /// </summary>
    public static int PickLairTile(Sim s, double minD, double maxD, Civ near, bool hills)
    {
        var w = s.W;
        var setts = J.Filter(w.Settlements, x => x.Alive);
        var camps = J.Filter(w.Camps, x => x.Alive);
        var mine = near != null ? s.CivSettlements(near) : null;
        var cand = new List<int>();
        var score = new List<double>();
        for (int i = 0; i < w.Tiles.Count; i++)
        {
            var t = w.Tiles[i];
            if (t.Terrain == "water" || t.Terrain == "mountain" || J.T(t.Sea) || t.Owner >= 0 || t.Camp != null || t.Ext != null || J.T(t.Isle) || t.InnZone != null || t.Inn != null) continue;
            double d = double.PositiveInfinity;
            foreach (var x in setts) { double dd = s.G.Dist(x.Tile, i); if (dd < d) { d = dd; if (d < minD) break; } }
            if (d < minD || d > maxD) continue;
            double dc = double.PositiveInfinity, crowd = 0;
            foreach (var cp in camps) { double e = s.G.Dist(cp.Tile, i); if (e < dc) dc = e; if (e <= 12) crowd++; }
            if (dc < 5) continue;
            double sc = -Math.Abs(d - minD - 2) - crowd * 0.8 + (t.Terrain == "forest" || t.Terrain == "oldforest" || t.Terrain == "hill" ? 2 : 0);
            if (hills)
            {
                double m = 0;
                foreach (int nb in s.G.Neighbors(i)) if (w.Tiles[nb].Terrain == "mountain") m++;
                sc += JsMath.Min(3, m) + (t.Terrain == "hill" ? 1 : 0);
            }
            if (mine != null && mine.Count > 0) sc -= J.MinOf(mine, x => s.G.Dist(x.Tile, i)) / 3.0;
            cand.Add(i); score.Add(sc);
        }
        if (cand.Count == 0) return -1;
        var idx = J.From(cand.Count, k => k);
        J.Sort(idx, (a, b) => J.Or(score[b] - score[a], cand[a] - cand[b]));
        var top = J.Slice(idx, 0, 10);
        double low = score[top[top.Count - 1]];
        int pick = s.Rng.Weighted(top, k => score[k] - low + 1);
        return cand[pick];
    }

    private static int PickNear(Sim s, List<int> cands, List<int> avoid, double minD = 7)
    {
        int best = -1; double bs = double.NegativeInfinity;
        foreach (int i in cands)
        {
            var t = s.W.Tiles[i];
            if (t.Terrain == "water" || t.Terrain == "mountain" || t.Owner >= 0 || t.Camp != null || J.T(t.Isle) || t.InnZone != null) continue;
            double d = avoid.Count > 0 ? J.MinOf(avoid, a => s.G.Dist(a, i)) : 20;
            if (d < minD) continue;
            double sc = JsMath.Min(d, 12) + s.Rng.Next() * 3;
            if (sc > bs) { bs = sc; best = i; }
        }
        return best;
    }

    private static readonly string[] ROMAN = { "II", "III", "IV", "V", "VI", "VII", "VIII" };

    private static string FreshName(Sim s, List<string> list)
    {
        var used = new HashSet<string>();
        foreach (var c in s.W.Camps) used.Add(c.Name);
        var free = J.Find(list, n => !used.Contains(n));
        if (J.T(free)) return free;
        foreach (var r in ROMAN) foreach (var n in list) if (!used.Contains($"{n} {r}")) return $"{n} {r}";
        return $"{(list.Count > 0 ? list[0] : "undefined")} {s.W.Camps.Count}";
    }

    /// <summary>Kamptan akın: han, korumasız çıkarma yapısı, kervan ya da yerleşim hedefi seçip raid ajanı yollar.</summary>
    public static void LaunchRaid(Sim s, Camp c)
    {
        var w = s.W;
        c.NextRaid = s.Day + s.Rng.Int(c.Kind == "hobgoblin" ? 28 : c.Kind == "troll" ? 25 : 22, 43);   // Faz 1b-5: eski 110/100/90–170 gün
        int range = c.Kind == "bugbear" ? 9 : c.Kind == "troll" ? 14 : 16;
        var caravans = J.Filter(w.Agents, a => (a.Kind == "caravan" || (c.Kind == "bugbear" && (a.Kind == "party" || a.Kind == "scout" || a.Kind == "hero"))) && !J.T(a.Dead) && a.Hull == null
            && !J.Some(J.Slice(a.Path, a.Step, a.Step + 4), t => J.T(w.Tiles[t].Sea)) && PathDist(s, a, c.Tile) <= range - 3);
        var setts = c.Kind == "bugbear" ? new List<Settlement>() : J.Filter(w.Settlements, x => x.Alive && s.G.Dist(x.Tile, c.Tile) <= range && s.Pop(x) >= 10);
        int targetTile = -1, to = -1;
        string purpose = "";
        // korumasız çıkarma yapıları: iskele, tarla, maden, av kampı...
        var exts = new List<int>();
        if (c.Kind != "bugbear")
        {
            for (int i = 0; i < w.Tiles.Count; i++)
            {
                var t = w.Tiles[i];
                if (t.Ext != null && s.ExtWorking(t) && t.Owner >= 0 && s.G.Dist(i, c.Tile) <= range - 2) exts.Add(i);
            }
            exts = J.Filter(exts, i => { var st = s.Settlement(w.Tiles[i].Owner); return st != null && st.Alive && s.G.Dist(i, st.Tile) >= 1 && s.E(w.Civs[st.Civ], "pact") <= 0; });
        }
        bool pickExt = exts.Count > 0 && (setts.Count == 0 || s.Rng.Chance(0.55));
        var inns = c.Kind == "bugbear" ? new List<Inn>() : J.Filter(w.Inns, i => i.Alive && s.G.Dist(i.Tile, c.Tile) <= range - 2);
        if (inns.Count > 0 && s.Rng.Chance(0.2))
        {
            var inn = J.Sort(inns, (a, b) => s.G.Dist(a.Tile, c.Tile) - s.G.Dist(b.Tile, c.Tile))[0];
            targetTile = inn.Tile; to = inn.Id; purpose = "inn";
        }
        else if (pickExt && !(caravans.Count > 0 && s.Rng.Chance(0.4)))
        {
            int t = s.Rng.Weighted(exts, i => { var st = s.Settlement(w.Tiles[i].Owner); return (1 + s.G.Dist(i, st.Tile)) / (1 + st.Soldiers * 0.5) / (1 + s.G.Dist(i, c.Tile) * 0.15); });
            targetTile = t; to = t; purpose = "ext";
        }
        else if (caravans.Count > 0 && (s.Rng.Chance(0.65) || setts.Count == 0))
        {
            var cv = s.Rng.Pick(caravans);
            targetTile = cv.Path[Math.Min(cv.Path.Count - 1, cv.Step + 3)]; to = cv.Id; purpose = "caravan";
        }
        else if (setts.Count > 0)
        {
            var st = s.Rng.Weighted(setts, x => (s.E(s.W.Civs[x.Civ], "pact") > 0 ? 0.25 : 1) / (1 + x.Soldiers * 3 + s.Pop(x) * 0.4 + (J.T(x.Civics.Get("palisade")) ? 6 : 0) + (J.T(x.Civics.Get("stonewall")) ? 12 : 0)) / (1 + s.G.Dist(x.Tile, c.Tile) * 0.1));
            targetTile = st.Tile; to = st.Id; purpose = "settlement";
        }
        if (targetTile < 0) return;
        double n = c.Kind == "bugbear" ? c.Count : c.Kind == "troll" ? JsMath.Min(c.Count, JsMath.Max(2, Math.Ceiling(c.Count * 0.6))) : JsMath.Min(Math.Ceiling(c.Count * 0.5), 3 + Math.Floor(s.DynYear / 2.0));
        // Kızıl Ay (anlatıcının baskın dalgası): kamplar yarısından fazlasıyla akına çıkar
        if (c.Kind != "bugbear" && Storyteller.Surging(s)) n = JsMath.Min(c.Count, Math.Ceiling(n * 1.5));
        bool boss = c.Boss && c.Count >= 10 && s.Rng.Chance(0.5);
        var path = s.Path(c.Tile, targetTile);
        if (path == null) return;
        c.Count -= n;
        if (boss) c.Boss = false;
        Reveal(s, c, null);   // Faz 1 B2: gizli in ilk akınıyla kendini ele verir
        s.W.Agents.Add(new Agent { Id = s.Id(), Kind = "raid", Civ = -1, Path = path, Step = 0, Progress = 0, Speed = c.Kind == "bugbear" ? Pace.RAID_BUGBEAR : c.Kind == "troll" ? Pace.RAID_TROLL : Pace.RAID, Troops = n, From = c.Id, To = to, Purpose = purpose, Boss = boss, Monster = c.Kind, TargetTile = targetTile });
    }

    /// <summary>JS <c>s.g.dist(a.path[Math.min(a.step, a.path.length - 1)], tile)</c>: NaN when the path is empty (undefined tile).</summary>
    private static double PathDist(Sim s, Agent a, int tile)
    {
        int k = Math.Min(a.Step, a.Path.Count - 1);
        return k >= 0 && k < a.Path.Count ? s.G.Dist(a.Path[k], tile) : double.NaN;
    }
}
