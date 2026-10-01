using System;
using System.Collections.Generic;
using System.Linq;

// Kahraman iradesi: serbest kahramanlar kiralanana dek kendi yollarını izler.
// Yol (avcı, gezgin, şifacı, bilge, paralı, karanlık), 30 günde bir hedef seçimi, izler ve efsane.
// Port of src/sim/will.ts.

namespace FD.Macro;

/// <summary>will.ts <c>interface Option</c>: chooseGoal'un değerlendirdiği aday hedef.</summary>
public sealed class Option
{
    public string Kind;          // GoalKind
    public int Tile;
    public int? Target;
    public double Score;
    public string Text;
    public List<Hero> Crew;
    public Quest Quest;
    public Camp Camp;
}

/// <summary>will.ts nearestRest sonucu <c>{ id, tile, inn }</c> (null = undefined).</summary>
public sealed class RestSpot
{
    public int Id;
    public int Tile;
    public bool Inn;
}

public static class Will
{
    // ------------------------------------------------------------ doğuş
    /// <summary>Doğuşta hizalama ve yol zarı (sınıfa göre).</summary>
    public static (string Align, string Path) RollWill(Sim s, string cls)
    {
        double r = s.Rng.Next();
        string align;
        if (cls == "paladin") align = "good";
        else if (cls == "cleric") align = r < 0.7 ? "good" : r < 0.92 ? "neutral" : "evil";
        else if (cls == "rogue" || cls == "barbarian") align = r < 0.35 ? "good" : r < 0.72 ? "neutral" : "evil";
        else align = r < 0.42 ? "good" : r < 0.82 ? "neutral" : "evil";
        string path = D.CLASS_PATH[cls];
        if (cls == "paladin" && s.Rng.Chance(0.5)) path = "hunter";
        if (align == "evil" && s.Rng.Chance(0.4)) path = "dark";
        else if (align != "good" && s.Rng.Chance(0.3)) path = "mercenary";
        return (align, path);
    }

    /// <summary>günlük tavanı: taşınca en eski satır atılır (kilometre taşları ayrıca Hero.Deeds'te durur)</summary>
    public const int JOURNAL_CAP = 400;

    /// <summary>Kahramanın günlüğüne satır ekler (son 400 satır tutulur).</summary>
    public static void Note(Sim s, Hero h, string text)
    {
        h.Journal.Add(new JournalEntry { Day = s.Day, Text = text });
        if (h.Journal.Count > JOURNAL_CAP) J.Shift(h.Journal);
    }

    /// <summary>Kahramanın bu izi var mı.</summary>
    public static bool HasTrait(Hero h, string t) => h.Traits.Contains(t);

    /// <summary>Yeni iz kazandırır (günlük, metrik, olay kaydı); zaten varsa bir şey yapmaz.</summary>
    public static void AddTrait(Sim s, Hero h, string t, string why)
    {
        if (HasTrait(h, t)) return;
        h.Traits.Add(t);
        Note(s, h, $"{D.TRAITS[t].Name} izi: {why}");
        s.Metric("trait_" + t);
        s.Log("hero", $"{h.Name} yeni bir iz kazandı: {D.TRAITS[t].Name}.", tile: h.Pos, civ: h.Civ >= 0 ? h.Civ : (int?)null, cause: why, major: t != "avenger");
    }

    /// <summary>Kahramanın adı, lakabı varsa «lakap» ile.</summary>
    public static string HeroLabel(Hero h) => J.T(h.Epithet) ? $"{h.Name} «{h.Epithet}»" : h.Name;

    /// <summary>kötü kahraman paladine, iyi kahraman Paktçı'ya çalışmaz; kinli olduğu medeniyete de</summary>
    public static bool HeroWillServe(Civ c, Hero h)
    {
        if (h.Grudge == c.Id) return false;
        if (h.Align == "good" && c.Align.Good < -0.3) return false;
        if (h.Align == "evil" && c.Cls == "paladin") return false;
        return true;
    }

    /// <summary>kahramanın savaştaki hâli: iz bonusları dâhil</summary>
    public static Combatant HeroSide(Hero h, string side, string vs = null, bool alone = false)
    {
        var c = Combat.HeroCombatant(h, side);
        if (vs == "goblin" && HasTrait(h, "goblinslayer")) c.Atk += 2;
        if (alone && HasTrait(h, "lonewolf")) { c.Atk += 1; c.Ac += 1; }
        if (h.Align == "evil") c.Evil = true;
        return c;
    }

    // ------------------------------------------------------------ yuva
    /// <summary>Id'ye göre han (ölü olsa da); id null ise null.</summary>
    public static Inn InnById(Sim s, int? id) => id == null ? null : J.Find(s.W.Inns, i => i.Id == id);

    /// <summary>Karodaki canlı han (null = yok).</summary>
    public static Inn InnAt(Sim s, int tile)
    {
        int? id = s.W.Tiles[tile].Inn;
        return id == null ? null : J.Find(s.W.Inns, i => i.Id == id && i.Alive);
    }

    /// <summary>en yakın canlı han ya da taverna (null = yok)</summary>
    public static RestSpot NearestRest(Sim s, int from)
    {
        RestSpot best = null; double bd = 1e9;
        foreach (var i in s.W.Inns) if (i.Alive) { double d = s.G.Dist(i.Tile, from) - 3; if (d < bd) { bd = d; best = new RestSpot { Id = i.Id, Tile = i.Tile, Inn = true }; } }
        foreach (var st in s.W.Settlements) if (st.Alive && J.T(st.Civics.Get("tavern"))) { double d = s.G.Dist(st.Tile, from); if (d < bd) { bd = d; best = new RestSpot { Id = st.Id, Tile = st.Tile, Inn = false }; } }
        return best;
    }

    /// <summary>Kahramanın yuvasının karosu (han/taverna yoksa null).</summary>
    public static int? BaseTile(Sim s, Hero h)
    {
        if (h.BaseInn) { var i = InnById(s, h.Base); return i != null && i.Alive ? i.Tile : null; }
        var st = s.Settlement(h.Base);
        return st != null && st.Alive && J.T(st.Civics.Get("tavern")) ? st.Tile : null;
    }

    /// <summary>serbest kahramanı yuvasına (yıkıldıysa en yakın dinlenme yerine) gönderir</summary>
    public static void ReturnToBase(Sim s, Hero h)
    {
        if (h.State == "dead" || h.State == "gone" || h.State == "retired") return;
        h.Goal = null;
        int? t = BaseTile(s, h);
        if (t == null)
        {
            var r = NearestRest(s, h.Pos);
            if (r == null) { h.State = "gone"; s.Log("hero", $"{h.Name} dinlenecek bir yer bulamayınca uzak diyarlara gitti.", tile: h.Pos); return; }
            h.Base = r.Id; h.BaseInn = r.Inn; h.BaseDay = s.Day; t = r.Tile;
        }
        Heroes.SendHero(s, h, t.Value, "tavern");
        if (h.State == "tavern") { h.Tavern = h.Base; h.IdleSince = s.Day; }
    }

    /// <summary>Kahramanı taşıyan canlı ajan (null = yok).</summary>
    public static Agent HeroAgent(Sim s, Hero h) => J.Find(s.W.Agents, a => !J.T(a.Dead) && a.Heroes != null && a.Heroes.Contains(h.Id));

    // ------------------------------------------------------------ gelişim
    /// <summary>seviye atlarken kazanılan bonus, yaşananlara göre</summary>
    public static void GrowFromExperience(Sim s, Hero h)
    {
        var t = h.Tally;
        var opts = new List<(double W, Func<string> F)>
        {
            ((t.Get("goblin") ?? 0) * 0.3 + (t.Get("kills") ?? 0) * 0.1, () => { h.Bonus.Atk++; return "kamp savaşlarında bileği güçlendi (+1 saldırı)"; }),
            ((t.Get("plague") ?? 0) * 1.5 + (t.Get("temple") ?? 0) * 0.5, () => { h.MaxHp += 4; h.Hp += 4; return "hastalık yatağında dayanıklılığı arttı (+4 can)"; }),
            ((t.Get("ruins") ?? 0) * 1.2 + (t.Get("rob") ?? 0) * 0.8, () => { h.Ac++; return "tuzaklardan sıyrılmayı öğrendi (+1 zırh)"; }),
            ((t.Get("lib") ?? 0) * 1.5, () => { string k = h.Cls == "wizard" ? "int" : "wis"; h.Stats.Set(k, J.N(h.Stats.Get(k)) + 1); return "kütüphanelerde yeni bilgiler öğrendi"; }),
        };
        // opts.reduce((a, b) => (b[0] > a[0] ? b : a)) — no initial value: starts from opts[0]
        var best = opts[0];
        for (int i = 1; i < opts.Count; i++) { var b = opts[i]; best = b.W > best.W ? b : best; }
        if (best.W < 0.5) return;
        // template literal: h.level is read before best[1]() runs
        string lvl = J.S(h.Level);
        Note(s, h, $"Sv {lvl}: {best.F()}");
    }

    /// <summary>Kahraman efsane olur (iz, kilometre taşı, heykel, olay kaydı); bir kez. Efsanelik seviyeden değil
    /// ünden gelir (Heroes.AddRenown, eşik Heroes.LEGEND_RENOWN).</summary>
    public static void BecomeLegend(Sim s, Hero h)
    {
        if (J.T(h.Legend)) return;
        h.Legend = true;
        AddTrait(s, h, "legend", "Ünü dilden dile dolaştı");
        Lore.Deed(s, h, "legend", "ozanların şarkılarına girdi", h.Pos);
        string where = h.BaseInn ? $"{InnById(s, h.Base)?.Name ?? "Han"} Hanı'nın" : $"{s.Settlement(h.Base)?.Name ?? "Yurdunun"}";
        s.Metric("legend");
        s.Log("hero", $"Ozanlar {HeroLabel(h)} için şarkı yakıyor; {where} önüne heykeli dikildi.", tile: h.Pos, civ: h.Civ >= 0 ? h.Civ : (int?)null, major: true,
            cause: $"Ün {J.S(JsMath.Round(h.Renown))}: {LegendWhy(h)}Sv {J.S(h.Level)}");
    }

    /// <summary>Efsanelik gerekçesi: ejderha, devrilen önderler, düellolar, savunmalar, ilanlar, kamplar ("2 önder, 3 savunma, 9 kamp, ").</summary>
    private static string LegendWhy(Hero h)
    {
        int Of(string k) => h.Deeds == null ? 0 : J.Filter(h.Deeds, d => d.Kind == k).Count;
        var parts = new List<string>();
        if (Of("dragon") > 0) parts.Add("ejderha");
        void Add(double n, string what) { if (n > 0) parts.Add($"{J.S(n)} {what}"); }
        Add(Of("boss"), "önder");
        Add(Of("duel"), "düello");
        Add(h.Tally.Get("defends") ?? 0, "savunma");
        Add(h.Tally.Get("quests") ?? 0, "ilan");
        Add(h.Tally.Get("camps") ?? 0, "kamp");
        return parts.Count > 0 ? string.Join(", ", parts) + ", " : $"{J.S(h.Kills)} düşman, ";
    }

    // ------------------------------------------------------------ olaylara tepki
    /// <summary>yerleşim yandı: orada doğmuş serbest kahramanlar intikamcı olur</summary>
    public static void OnHomeBurned(Sim s, Settlement st, Camp byCamp = null, Civ byCiv = null)
    {
        var heroes = s.W.Heroes;
        for (int i = 0; i < heroes.Count; i++)
        {
            var h = heroes[i];
            if (h.Birth != st.Id || h.State == "dead" || h.State == "gone" || h.State == "retired") continue;
            if (byCamp != null && h.Vendetta != byCamp.Id)
            {
                h.Vendetta = byCamp.Id;
                if (h.Civ == -1 && h.Path != "healer") h.Path = "hunter";
                AddTrait(s, h, "avenger", $"{Lore.Ek(byCamp.Name, "in")} {Lore.Ek(st.Name, "i")} yakmasının intikamı");
            }
            if (byCiv != null && h.Grudge != byCiv.Id) { h.Grudge = byCiv.Id; Note(s, h, $"{byCiv.Name} doğduğu {Lore.Ek(st.Name, "i")} yaktı; onlara asla hizmet etmeyecek"); }
        }
    }

    /// <summary>kamp savaşı sonrası: sayaçlar, izler, lakap, yol değişimi</summary>
    public static void AfterCampFight(Sim s, List<Combatant> side, Camp cp, bool won, bool hadBoss, List<BattleRoll> rolls)
    {
        var hs = J.Map(J.Filter(side, x => x.Hero != null), x => x.Hero);
        var alive = J.Filter(hs, h => h.State != "dead");
        // kamp ünü kahramanların savaştaki payıyla: kendi partisiyle basan tam alır, kalabalık bir ordunun içindeki az
        double sv = 0, hv = 0;
        foreach (var x in side) { double v = Heroes.XpValue(x); sv += v; if (x.Hero != null) hv += v; }
        double campFame = Heroes.R_CAMP * JsMath.Min(1, 2 * hv / JsMath.Max(1, sv));   // her yeni kamp biraz daha az ün getirir (aşağıda)
        foreach (var x in side)
        {
            var h = x.Hero; if (h == null || h.State == "dead") continue;
            h.Tally.Set("kills", (h.Tally.Get("kills") ?? 0) + x.Kills);
            if (cp.Kind == "goblin") { h.Tally.Set("goblin", (h.Tally.Get("goblin") ?? 0) + x.Kills); if (J.N(h.Tally.Get("goblin")) >= 10) AddTrait(s, h, "goblinslayer", $"{J.S(J.N(h.Tally.Get("goblin")))} goblin devirdi"); }
            if (won)
            {
                h.Tally.Set("fails", 0); h.Tally.Set("camps", (h.Tally.Get("camps") ?? 0) + 1);
                Note(s, h, $"{cp.Name} yerle bir edildi");
                if (cp.Kind != "dragon") Lore.Deed(s, h, "camp", $"{Lore.Ek(cp.Name, "i")} yerle bir etti", cp.Tile, cp.Name);   // ejderhanın taşı Dragon.cs'te
                Heroes.AddRenown(s, h, campFame * Heroes.Repeat(J.N(h.Tally.Get("camps"))), "camp");
                if (h.Vendetta == cp.Id) { h.Vendetta = null; Note(s, h, "intikamını aldı"); }
                if (hadBoss && !J.T(h.Epithet) && J.Some(rolls, r => r.D20 == 20 && r.Who == h.Name))
                {
                    h.Epithet = $"{EpithetRoot(cp.Name)} Belası";
                    s.Metric("epithet");
                    Lore.Deed(s, h, "epithet", $"«{h.Epithet}» diye anılır oldu", cp.Tile);
                    s.Log("hero", $"{h.Name} artık «{h.Epithet}» diye anılıyor.", tile: cp.Tile, civ: h.Civ >= 0 ? h.Civ : (int?)null, major: true, cause: "Kamp önderini doğal 20 ile devirdi");
                }
            }
            else
            {
                h.Tally.Set("fails", (h.Tally.Get("fails") ?? 0) + 1);
                Note(s, h, $"{Lore.Ek(cp.Name, "da")} püskürtüldü");
                if (h.Civ == -1 && h.Align == "evil" && h.Path != "dark" && J.N(h.Tally.Get("fails")) >= 2)
                {
                    h.Path = "dark";
                    s.Log("hero", $"Art arda yenilgiler {Lore.Ek(h.Name, "i")} karanlık yola itti; artık kervan soyuyor.", tile: h.Pos, major: true, cause: "Kötü hizalı, iki başarısız sefer");
                }
            }
        }
        if (hs.Count >= 2 && alive.Count == 1)
        {
            var h = alive[0];
            h.SoloUntil = s.Day + 2 * Sim.OLD_YEAR;
            AddTrait(s, h, "lonewolf", "Partisinin tek sağ kalanı");
        }
    }

    // ------------------------------------------------------------ savaş dışı XP
    /// <summary>harabe keşfi (tuzakta yarısı), salgına şifa, hac, kütüphane (bilge yolunda daha çok)</summary>
    public const double XP_RUIN = 80, XP_PLAGUE = 150, XP_TEMPLE = 40, XP_LIBRARY = 80, XP_LIBRARY_SAGE = 160;
    /// <summary>serbest kahramanın ilan ve av için gözünü diktiği en uzak kamp (fersah; Faz 1: 30 → 40, 22 → 30)</summary>
    public const double QUEST_RANGE = 40, HUNT_RANGE = 30;
    /// <summary>iyi yürekli kahramanın kara yola sapmış (kervan soymuş) birinin peşine düştüğü en uzak mesafe ve hedefin çekiciliği
    /// (Faz 1 C1: 20 fersah ve 30 iken ilanlar hep ağır basıyor, düello hiç olmuyordu)</summary>
    public const double DUEL_RANGE = 30, DUEL_PULL = 60;

    /// <summary>temizlenmiş bir kampın terk edilmiş ini bu kadar (eski) yıl keşfedilebilir</summary>
    public const double LAIR_YEARS = 6;
    /// <summary>Faz 1b-5: serbest kahramanın hedef seçme aralığı (gün; eskiden 30 eski gün)</summary>
    public const double DECIDE_DAYS = 30 / Sim.PACE;

    /// <summary>Harabe hedefinin adı: ölü yerleşim, yanmış han ("X Hanı") ya da temizlenmiş kamp; bulunamazsa null.</summary>
    private static string RuinName(Sim s, int? id)
    {
        if (id == null) return null;
        var st = s.Settlement(id); if (st != null) return st.Name;
        var inn = InnById(s, id); if (inn != null) return Inns.InnName(inn);
        return J.Find(s.W.Camps, c => c.Id == id)?.Name;
    }

    private static readonly HashSet<string> CAMP_WORDS = new() { "Kampı", "Koyu", "Karakolu", "İni", "Obası", "Yuvası", "Kalesi" };

    /// <summary>Lakap kökü: kamp adından tür sözcüğü ve sıra numarası atılır ("Kara Bayrak Koyu 2" → "Kara Bayrak",
    /// "Kırıkdiş Kampı II" → "Kırıkdiş"); eskiden yalnız ilk sözcük alınıyordu ("Kara Belası").</summary>
    private static string EpithetRoot(string camp)
    {
        var ws = new List<string>(camp.Split(' '));
        while (ws.Count > 1 && (ws[ws.Count - 1].All(char.IsDigit) || ws[ws.Count - 1].All(ch => "IVX".IndexOf(ch) >= 0))) ws.RemoveAt(ws.Count - 1);
        if (ws.Count > 1 && CAMP_WORDS.Contains(ws[ws.Count - 1])) ws.RemoveAt(ws.Count - 1);
        return string.Join(" ", ws);
    }

    // ------------------------------------------------------------ hedef seçimi
    private static readonly Dictionary<string, Dictionary<string, double>> FIT = new()
    {
        ["quest"] = new() { ["hunter"] = 1.2, ["wanderer"] = 0.8, ["healer"] = 0.6, ["sage"] = 0.5, ["mercenary"] = 1.7, ["dark"] = 0.6 },
        ["hunt"] = new() { ["hunter"] = 1.3, ["mercenary"] = 0.3, ["dark"] = 0.3, ["healer"] = 0.15, ["wanderer"] = 0.2 },
        ["ruin"] = new() { ["wanderer"] = 1.6, ["sage"] = 0.6, ["mercenary"] = 0.4, ["dark"] = 0.6, ["hunter"] = 0.3, ["healer"] = 0.2 },
        ["plague"] = new() { ["healer"] = 1.8 },
        ["temple"] = new() { ["healer"] = 0.7, ["hunter"] = 0.1, ["sage"] = 0.2 },
        ["library"] = new() { ["sage"] = 1.6, ["wanderer"] = 0.3, ["healer"] = 0.2 },
        ["rob"] = new() { ["dark"] = 1.6 },
        ["duel"] = new() { ["hunter"] = 0.9, ["healer"] = 0.6 },
    };

    /// <summary>Serbest kahramanlar: hedefte bekleyenler ilerler, tavernadakiler DECIDE_DAYS günde bir kez hedef seçer.</summary>
    public static void WillTick(Sim s)
    {
        var w = s.W;
        var heroes = w.Heroes;
        for (int i = 0; i < heroes.Count; i++)
        {
            var h = heroes[i];
            if (h.Civ != -1) continue;
            if (h.State == "quest" && h.Goal != null && HeroAgent(s, h) == null) { ProgressGoal(s, h); continue; }
            if (h.State != "tavern") continue;
            if (h.Hp < h.MaxHp * 0.8) continue;
            if (!Sim.Tick(s.Day, DECIDE_DAYS, h.Id * 1.25)) continue;   // her kahraman DECIDE_DAYS günde bir kez karar verir (Faz 1b-5: eski 30 gün)
            ChooseGoal(s, h);
        }
    }

    private static double DistPen(Sim s, int a, int b) => 1 + s.G.Dist(a, b) / 12.0;

    /// <summary>kampa karşı yeterli grup: kendisi + tavernadaki arkadaşlar (+ aynı anda varacak dostlar)</summary>
    private static List<Hero> PartyFor(Sim s, Hero h, Camp cp, double needMul, double greed, List<Combatant> allies)
    {
        var mons = Heroes.CampForce(s, cp);
        bool Enough(List<Hero> crew0)
        {
            var cs = J.Map(crew0, c => HeroSide(c, "A", cp.Kind, crew0.Count == 1 && allies.Count == 0));
            var all = new List<Combatant>(cs);
            all.AddRange(allies);
            var (pa, pb) = Combat.PowerVs(all, mons);
            return pa * greed * Daring(s, h) >= pb * needMul;
        }
        if (Enough(new List<Hero> { h })) return new List<Hero> { h };
        if ((h.SoloUntil ?? 0) > s.Day) return null;
        var mates = J.Sort(J.Filter(s.W.Heroes, x => !ReferenceEquals(x, h) && x.Civ == -1 && x.State == "tavern" && x.Tavern == h.Tavern && x.Hp >= x.MaxHp * 0.8
            && (x.SoloUntil ?? 0) <= s.Day && !(x.Align == "good" && h.Align == "evil") && !(x.Align == "evil" && h.Align == "good") && !(x.Auction != null && x.Auction.Bids.Count > 0)),
            (a, b) => J.Or(b.Level - a.Level, a.Id - b.Id));
        var crew = new List<Hero> { h };
        foreach (var m in mates)
        {
            if (J.Some(crew, c => (c.Align == "good" && m.Align == "evil") || (c.Align == "evil" && m.Align == "good"))) continue;
            crew.Add(m);
            if (Enough(crew)) return crew;
            if (crew.Count >= 4) break;
        }
        return null;
    }

    /// <summary>Uzun süre kılıç çekmeyen kahraman huzursuzlanır, daha büyük riske girer: son savaşından (hiç savaşmadıysa
    /// doğumundan) bu yana geçen her yıl için +%20 cesaret, en çok iki kat.</summary>
    public static double Daring(Sim s, Hero h)
    {
        double since = s.Day - (h.Tally.Get("lastFight") ?? h.Born);
        return 1 + JsMath.Min(1, since / Sim.OLD_YEAR * 0.2);
    }

    /// <summary>risk: kampın gücü / grubun (ve dostlarının) gücü</summary>
    private static double CampRisk(Sim s, Camp cp, List<Hero> crew, List<Combatant> allies)
    {
        var all = J.Map(crew, c => HeroSide(c, "A", cp.Kind, crew.Count == 1 && allies.Count == 0));
        all.AddRange(allies);
        var (pa, pb) = Combat.PowerVs(all, Heroes.CampForce(s, cp));
        return pb / JsMath.Max(0.01, pa);
    }

    /// <summary>campAllies sonucu <c>{ agents, cs }</c>.</summary>
    private sealed class Allies
    {
        public List<Agent> Agents;
        public List<Combatant> Cs;
    }

    /// <summary>TS NO_ALLIES: shared empty sentinel (never mutated).</summary>
    private static readonly Allies NO_ALLIES = new Allies { Agents = new List<Agent>(), Cs = new List<Combatant>() };

    /// <summary>bu kampa, kahramanla yaklaşık aynı anda varacak dost gruplar</summary>
    private static Allies CampAllies(Sim s, Hero h, Camp cp)
    {
        var path = s.Path(h.Pos, cp.Tile);
        if (path == null) return new Allies { Agents = new List<Agent>(), Cs = new List<Combatant>() };
        var probe = new Agent { Id = -1, Kind = "party", Civ = -1, Path = path, Step = 0, Progress = 0, Speed = Pace.PARTY, Heroes = new List<int> { h.Id }, To = cp.Id, Purpose = "quest" };
        var agents = Agents.ProbeAllies(s, probe, Agents.EtaDays(s, probe), Agents.MUSTER_CAMP);
        var cs = new List<Combatant>();
        foreach (var b in agents) cs.AddRange(Agents.AgentCombatants(s, b, cp.Kind));
        return new Allies { Agents = agents, Cs = cs };
    }

    /// <summary>TS <c>FIT[k][h.path] ?? 0</c>.</summary>
    private static double Fit(Hero h, string k) => FIT[k].TryGetValue(h.Path, out double v) ? v : 0;

    /// <summary>Kahraman için adayları (ilan, av, harabe, salgın, hac, kütüphane, soygun, düello) puanlar ve en iyisini başlatır.</summary>
    public static void ChooseGoal(Sim s, Hero h)
    {
        var w = s.W;
        var opts = new List<Option>();
        double me = Combat.PowerOf(new List<Combatant> { HeroSide(h, "A", null, true) });
        // 1) panodaki ilanlar
        if (Fit(h, "quest") > 0)
        {
            var quests = w.Quests;
            for (int qi = 0; qi < quests.Count; qi++)
            {
                var q = quests[qi];
                if (!q.Open) continue;
                var cp = J.Find(w.Camps, c => c.Id == q.Camp && c.Alive);
                if (cp == null || s.G.Dist(h.Pos, cp.Tile) > QUEST_RANGE) continue;
                if (q.Civ >= 0 && h.Grudge == q.Civ) continue;
                double greed = 1 + JsMath.Min(0.4, q.Bounty / 300);
                double need = cp.Kind == "dragon" ? Dragon.HERO_NEED : 1.0;   // Faz 1 B2: ejderhaya ancak güçlü bir grup gider
                // önce tavernadan kendi grubunu kurmayı dener; yetmezse yoldaki dostlara katılmayı hesaplar
                var al = NO_ALLIES;
                var crew = PartyFor(s, h, cp, need, greed, al.Cs);
                if (crew == null) { al = CampAllies(s, h, cp); if (al.Agents.Count > 0) crew = PartyFor(s, h, cp, need, greed, al.Cs); }
                if (crew == null) continue;
                double risk = CampRisk(s, cp, crew, al.Cs);
                double venge = h.Vendetta == cp.Id ? 3 : 1;
                double rep = q.Civ >= 0 && (h.Rep.Get(q.Civ) ?? 0) >= 3 ? 1.15 : 1;
                opts.Add(new Option { Kind = "quest", Tile = cp.Tile, Score = Fit(h, "quest") * venge * rep * (q.Bounty / crew.Count + 30) / (JsMath.Max(0.3, risk) * DistPen(s, h.Pos, cp.Tile)), Text = $"{cp.Name} ilanı", Crew = crew, Quest = q, Camp = cp });
            }
        }
        // 2) ilansız av
        if (Fit(h, "hunt") > 0 || h.Vendetta != null)
        {
            var camps = w.Camps;
            for (int ci = 0; ci < camps.Count; ci++)
            {
                var cp = camps[ci];
                if (!cp.Alive || J.T(cp.Hidden) || s.G.Dist(h.Pos, cp.Tile) > HUNT_RANGE || J.T(w.Tiles[cp.Tile].Isle)) continue;   // gizli in (B2) avlanamaz
                if (J.Some(w.Quests, q => q.Camp == cp.Id && q.Open)) continue;
                double needMul = (h.Vendetta == cp.Id ? 1.0 : 1.2) * (cp.Kind == "dragon" ? Dragon.HERO_NEED : 1);
                var al = NO_ALLIES;
                var crew = PartyFor(s, h, cp, needMul, 1, al.Cs);
                if (crew == null) { al = CampAllies(s, h, cp); if (al.Agents.Count > 0) crew = PartyFor(s, h, cp, needMul, 1, al.Cs); }
                if (crew == null) continue;
                double risk = CampRisk(s, cp, crew, al.Cs) * needMul;
                double venge = h.Vendetta == cp.Id ? 4 : 1;
                opts.Add(new Option { Kind = "hunt", Tile = cp.Tile, Score = JsMath.Max(Fit(h, "hunt"), venge > 1 ? 1 : 0) * venge * (cp.Loot / crew.Count + 25) / (JsMath.Max(0.3, risk) * DistPen(s, h.Pos, cp.Tile)), Text = venge > 1 ? $"{Lore.Ek(cp.Name, "dan")} intikam" : $"{cp.Name} avı", Crew = crew, Camp = cp });
            }
        }
        // 3) harabeler: terk edilmiş yerleşimler, yanmış hanlar ve temizlenmiş kampların terk edilmiş inleri (son 6 yıl).
        // Faz 1: eskiden yalnız ölü yerleşimler sayılıyordu; çoğu dünyada hiç yerleşim ölmediği için bu hedef hiç oluşmuyordu.
        if (Fit(h, "ruin") > 0)
        {
            var seen = h.Seen ?? new List<int>();
            var sts = w.Settlements;
            for (int si = 0; si < sts.Count; si++)
            {
                var st = sts[si];
                if (st.Alive || seen.Contains(st.Id) || J.Some(w.Settlements, o => o.Alive && o.Tile == st.Tile) || w.Tiles[st.Tile].Camp != null) continue;
                if (s.G.Dist(h.Pos, st.Tile) > 26) continue;
                opts.Add(new Option { Kind = "ruin", Tile = st.Tile, Target = st.Id, Score = Fit(h, "ruin") * 35 / (0.5 * DistPen(s, h.Pos, st.Tile)), Text = $"{st.Name} harabesi" });
            }
            var inns = w.Inns;
            for (int ii = 0; ii < inns.Count; ii++)
            {
                var inn = inns[ii];
                if (inn.Alive || inn.Stage != "ruin" || seen.Contains(inn.Id) || s.G.Dist(h.Pos, inn.Tile) > 26) continue;
                opts.Add(new Option { Kind = "ruin", Tile = inn.Tile, Target = inn.Id, Score = Fit(h, "ruin") * 30 / (0.5 * DistPen(s, h.Pos, inn.Tile)), Text = $"yanmış {inn.Name} Hanı" });
            }
            var camps = w.Camps;
            for (int ci = 0; ci < camps.Count; ci++)
            {
                var cp = camps[ci];
                if (cp.Alive || cp.ClearedDay == null || s.Day - cp.ClearedDay.Value > LAIR_YEARS * Sim.OLD_YEAR || seen.Contains(cp.Id)) continue;
                if (w.Tiles[cp.Tile].Camp != null || J.T(w.Tiles[cp.Tile].Isle) || s.G.Dist(h.Pos, cp.Tile) > 26 || J.Some(w.Settlements, o => o.Alive && o.Tile == cp.Tile)) continue;
                opts.Add(new Option { Kind = "ruin", Tile = cp.Tile, Target = cp.Id, Score = Fit(h, "ruin") * 25 / (0.5 * DistPen(s, h.Pos, cp.Tile)), Text = $"terk edilmiş {cp.Name}" });
            }
        }
        // 4) salgın, 5) hac, 6) kütüphane
        {
            var sts = w.Settlements;
            for (int si = 0; si < sts.Count; si++)
            {
                var st = sts[si];
                if (!st.Alive || s.G.Dist(h.Pos, st.Tile) > 26) continue;
                var c = w.Civs[st.Civ];
                if (h.Grudge == c.Id) continue;
                if (st.Plague != null && Fit(h, "plague") > 0 && st.Plague.Until - s.Day > 3 && !J.Some(w.Heroes, x => x.Goal != null && x.Goal.Kind == "plague" && x.Goal.Target == st.Id))
                    opts.Add(new Option { Kind = "plague", Tile = st.Tile, Target = st.Id, Score = Fit(h, "plague") * (35 + st.Plague.Severity * 20) / ((HasTrait(h, "plaguewalker") ? 0.35 : 0.7) * DistPen(s, h.Pos, st.Tile)), Text = $"salgınlı {st.Name}" });
                if (J.T(st.Civics.Get("temple")) && Fit(h, "temple") > 0 && !(h.Seen ?? new List<int>()).Contains(st.Id) && s.Day - (h.Tally.Get("templeDay") ?? -9999) > Sim.OLD_YEAR)
                    opts.Add(new Option { Kind = "temple", Tile = st.Tile, Target = st.Id, Score = Fit(h, "temple") * 12 / DistPen(s, h.Pos, st.Tile), Text = $"{Lore.Ek(st.Name, "in")} sunağına hac" });
                if (J.T(st.Civics.Get("library")) && Fit(h, "library") > 0 && !(h.Seen ?? new List<int>()).Contains(st.Id) && s.Day - (h.Tally.Get("libDay") ?? -9999) > Sim.OLD_YEAR)
                    opts.Add(new Option { Kind = "library", Tile = st.Tile, Target = st.Id, Score = Fit(h, "library") * 26 / (0.6 * DistPen(s, h.Pos, st.Tile)), Text = $"{Lore.Ek(st.Name, "in")} kütüphanesi" });
            }
        }
        // 7) kervan soygunu
        if (Fit(h, "rob") > 0 && s.Day - (h.Tally.Get("robDay") ?? -9999) > Sim.OLD_YEAR / 2.0)
        {
            var routes = w.Routes;
            for (int ri = 0; ri < routes.Count; ri++)
            {
                var r = routes[ri];
                if (!r.Alive || r.Path.Count < 6) continue;
                int t = r.Path[(int)Math.Floor(r.Path.Count / 2.0)];
                if (s.G.Dist(h.Pos, t) > 18 || w.Tiles[t].Owner >= 0 && s.Settlement(w.Tiles[t].Owner)?.Tile == t) continue;
                double guard = Combat.PowerOf(Agents.CivTroops(s, w.Civs[s.Settlement(r.A)?.Civ ?? 0], 3, "B"));
                double risk = guard / me;
                if (risk > 1.3) continue;
                opts.Add(new Option { Kind = "rob", Tile = t, Target = r.Id, Score = Fit(h, "rob") * 35 / (JsMath.Max(0.3, risk) * DistPen(s, h.Pos, t)), Text = "kervan yolunda pusu" });
            }
        }
        // 8) karanlık yola sapmış kahramanı avla
        if (Fit(h, "duel") > 0 && h.Align == "good")
        {
            var heroes = w.Heroes;
            for (int oi = 0; oi < heroes.Count; oi++)
            {
                var o = heroes[oi];
                if (o.Civ != -1 || o.Path != "dark" || !J.T(o.Tally.Get("rob") ?? 0) || ReferenceEquals(o, h) || (o.State != "tavern" && o.State != "quest") || s.G.Dist(h.Pos, o.Pos) > DUEL_RANGE) continue;
                double risk = Combat.PowerOf(new List<Combatant> { HeroSide(o, "B") }) / me;
                if (risk > 1.1) continue;
                opts.Add(new Option { Kind = "duel", Tile = o.Pos, Target = o.Id, Score = Fit(h, "duel") * DUEL_PULL / (JsMath.Max(0.3, risk) * DistPen(s, h.Pos, o.Pos)), Text = $"kara yola sapan {o.Name}" });
            }
        }
        if (opts.Count == 0) return;
        J.Sort(opts, (a, b) => J.Or(b.Score - a.Score, a.Tile - b.Tile));
        var best = opts[0];
        if (best.Score < 5) return;
        StartGoal(s, h, best);
    }

    private static void StartGoal(Sim s, Hero h, Option o)
    {
        var w = s.W;
        h.LastGoal = s.Day;
        if (o.Kind == "quest" || o.Kind == "hunt")
        {
            var crew = o.Crew; var cp = o.Camp;
            var path = s.Path(h.Pos, cp.Tile);
            if (path == null) return;
            if (o.Quest != null) { o.Quest.TakenBy = J.Map(crew, x => x.Id); o.Quest.Open = false; }
            foreach (var x in crew)
            {
                x.State = "quest"; x.Tavern = -1; x.LastGoal = s.Day;
                x.Goal = new HeroGoal { Kind = o.Kind, Tile = cp.Tile, Target = cp.Id, Text = o.Text, Since = s.Day };
                Note(s, x, o.Kind == "quest" ? $"{cp.Name} ilanını kopardı" : $"{Lore.Ek(cp.Name, "a")} ava çıktı");
            }
            w.Agents.Add(new Agent { Id = s.Id(), Kind = "party", Civ = -1, Path = path, Step = 0, Progress = 0, Speed = Pace.PARTY, Heroes = J.Map(crew, x => x.Id), To = cp.Id, Quest = o.Quest?.Id, Purpose = "quest" });
            s.Metric(o.Kind == "quest" ? "questTaken" : "goalHunt");
            string poster = o.Quest != null ? (o.Quest.Civ >= 0 ? w.Civs[o.Quest.Civ].Name : $"{InnById(s, o.Quest.Inn)?.Name ?? "Han"} Hanı") : "";
            s.Log("quest", crew.Count > 1
                ? $"Bir macera grubu kuruldu: {string.Join(", ", J.Map(crew, x => s.HeroTitle(x)))}. Hedef: {cp.Name}."
                : o.Quest != null ? $"{s.HeroTitle(h)}, {Lore.Ek(poster, "in")} ilanını kopardı ve {Lore.Ek(cp.Name, "a")} yola çıktı." : $"{s.HeroTitle(h)} kendi başına {Lore.Ek(cp.Name, "a")} ava çıktı.",
                tile: h.Pos, civ: o.Quest != null && o.Quest.Civ >= 0 ? o.Quest.Civ : (int?)null, cause: o.Quest != null ? $"Ödül: {J.S(o.Quest.Bounty)} altın" : h.Vendetta == cp.Id ? "İntikam" : $"{D.PATH_TR[h.Path]} yolu", major: true);
            return;
        }
        h.Goal = new HeroGoal { Kind = o.Kind, Tile = o.Tile, Target = o.Target, Text = o.Text, Since = s.Day };
        Note(s, h, $"Hedef: {o.Text}");
        s.Metric("goal_" + o.Kind);
        if (o.Kind != "temple") s.Log("hero", $"{s.HeroTitle(h)} yola çıktı: {o.Text}.", tile: h.Pos, cause: $"{D.PATH_TR[h.Path]} yolu");
        var path2 = s.Path(h.Pos, o.Tile);
        if (path2 == null || path2.Count < 2) { h.State = "quest"; h.Tavern = -1; ArriveGoal(s, h); return; }
        h.State = "traveling"; h.Tavern = -1;
        w.Agents.Add(new Agent { Id = s.Id(), Kind = "hero", Civ = -1, Path = path2, Step = 0, Progress = 0, Speed = Pace.HERO, Heroes = new List<int> { h.Id }, Purpose = "goal" });
    }

    // ------------------------------------------------------------ hedefte
    /// <summary>Kahraman hedefine vardı: hedef türüne göre sonuç (tuzak/hazine, şifa, dua, kütüphane, pusu, düello).</summary>
    public static void ArriveGoal(Sim s, Hero h)
    {
        var g = h.Goal;
        if (g == null || h.State == "dead") { ReturnToBase(s, h); return; }
        h.State = "quest"; h.Tavern = -1;
        var w = s.W;
        switch (g.Kind)
        {
            case "ruin":
                {
                    var st = s.Settlement(g.Target);
                    string rname = RuinName(s, g.Target);   // yerleşim, han ya da kamp
                    (h.Seen ??= new List<int>()).Add(g.Target.Value);
                    double bonus = HasTrait(h, "ruinrat") ? 2 : 0;
                    double roll = s.Rng.Dice(1, 20) + Rng.Mod(J.N(h.Stats.Get("dex"))) + bonus;
                    if (roll < 11)
                    {
                        double dmg = s.Rng.Dice(JsMath.Max(1, Math.Ceiling(h.Level / 2.0)), 8);
                        h.Hp = JsMath.Max(1, h.Hp - dmg);
                        Note(s, h, $"{rname ?? "harabe"} tuzağında yaralandı");
                        s.Log("hero", $"{h.Name}, {rname ?? "bir"} harabesinde tuzağa düştü ({J.S(dmg)} hasar).", tile: h.Pos, cause: $"d20 {J.S(roll)}");
                        Heroes.GainXp(s, h, XP_RUIN / 2, "ruin");
                    }
                    else
                    {
                        double gold = s.Rng.Int(10, 40) + bonus * 5;
                        h.Gold += gold;
                        string found = "";
                        if (s.Rng.Chance((st != null ? 0.12 : 0.06) + bonus * 0.04)) { h.Bonus.Atk++; found = " ve eski bir büyülü silah"; }
                        Note(s, h, $"{rname ?? "harabe"} harabesinde {J.S(gold)} altın{found} buldu");
                        s.Log("hero", $"{h.Name}, {rname ?? "bir"} harabesinde {J.S(gold)} altın{found} buldu.", tile: h.Pos, major: J.T(found), cause: $"{D.PATH_TR[h.Path]} yolu");
                        if (J.T(found)) Lore.Deed(s, h, "relic", $"{rname ?? "bir"} harabesinde eski bir büyülü silah buldu", h.Pos, rname);
                        Heroes.GainXp(s, h, XP_RUIN, "ruin");
                    }
                    h.Tally.Set("ruins", (h.Tally.Get("ruins") ?? 0) + 1);
                    if (J.N(h.Tally.Get("ruins")) >= 3) AddTrait(s, h, "ruinrat", $"{J.S(J.N(h.Tally.Get("ruins")))} harabe keşfetti");
                    g.Stay = s.Day + 1;   // Faz 1b-5: kalış süreleri ÷PACE (eski 5 / 15 / 5 / 20 / 40 gün)
                    return;
                }
            case "plague":
                {
                    var st = s.Settlement(g.Target);
                    if (st == null || !st.Alive || st.Plague == null) { ReturnToBase(s, h); return; }
                    var pl = st.Plague;
                    double heal = h.Cls == "cleric" || h.Cls == "druid" || h.Cls == "paladin" ? 0.55 : 0.75;
                    pl.Severity *= heal; pl.Until = JsMath.Max(s.Day + 2, pl.Until - 2);   // Faz 1b-5: salgın 5–10 gün (eskiden en az 8, 25 gün kısalır)
                    s.Metric("plagueHealed");
                    var c = w.Civs[st.Civ];
                    h.Rep.Set(c.Id, (h.Rep.Get(c.Id) ?? 0) + 1);
                    h.Tally.Set("plague", (h.Tally.Get("plague") ?? 0) + 1);
                    if (!HasTrait(h, "plaguewalker") && s.Rng.Dice(1, 20) + Rng.Mod(J.N(h.Stats.Get("con"))) < 10)
                    {
                        double dmg = Math.Ceiling(h.MaxHp * 0.35);
                        h.Hp = JsMath.Max(1, h.Hp - dmg);
                        Note(s, h, $"{Lore.Ek(st.Name, "da")} hastalığa yakalandı ama atlattı");
                    }
                    AddTrait(s, h, "plaguewalker", $"Salgınlı {Lore.Ek(st.Name, "da")} şifa dağıttı");
                    Note(s, h, $"{Lore.Ek(st.Name, "da")} şifa dağıttı");
                    Lore.Deed(s, h, "heal", $"salgınlı {Lore.Ek(st.Name, "a")} şifa taşıdı", st.Tile, st.Name);
                    s.Log("hero", $"{s.HeroTitle(h)} salgınlı {Lore.Ek(st.Name, "a")} şifa taşıdı; hastalık geriliyor.", tile: st.Tile, civ: c.Id, major: true, cause: $"{D.PATH_TR[h.Path]} yolu");
                    Heroes.GainXp(s, h, XP_PLAGUE, "plague");
                    g.Stay = s.Day + 4;
                    return;
                }
            case "temple":
                {
                    (h.Seen ??= new List<int>()).Add(g.Target.Value);
                    h.Hp = h.MaxHp;
                    h.Tally.Set("temple", (h.Tally.Get("temple") ?? 0) + 1); h.Tally.Set("templeDay", s.Day);
                    Heroes.GainXp(s, h, XP_TEMPLE, "temple");
                    Note(s, h, $"{s.Settlement(g.Target)?.Name ?? "bir"} sunağında dua etti");
                    g.Stay = s.Day + 1;
                    return;
                }
            case "library":
                {
                    (h.Seen ??= new List<int>()).Add(g.Target.Value);
                    g.Stay = s.Day + 5;
                    return;
                }
            case "rob": g.Stay = s.Day + 10; return;
            case "duel": Duel(s, h); return;
            default: ReturnToBase(s, h); return;
        }
    }

    private static void ProgressGoal(Sim s, Hero h)
    {
        var g = h.Goal;
        if (g.Kind == "rob")
        {
            var cv = J.Find(s.W.Agents, a => a.Kind == "caravan" && !J.T(a.Dead) && !J.T(s.W.Tiles[Agents.TileOf(a)].Sea) && s.G.Dist(Agents.TileOf(a), h.Pos) <= 2);
            if (cv != null) { RobCaravan(s, h, cv); return; }
        }
        if (g.Stay != null && s.Day < g.Stay.Value) return;
        if (g.Kind == "library")
        {
            var st = s.Settlement(g.Target);
            h.Tally.Set("lib", (h.Tally.Get("lib") ?? 0) + 1); h.Tally.Set("libDay", s.Day);
            Note(s, h, $"{st?.Name ?? "bir"} kütüphanesinde çalıştı");
            s.Log("hero", $"{h.Name}, {Lore.Ek(st?.Name ?? "kütüphane", "in")} kütüphanesinde eski kitaplardan yeni büyüler öğrendi.", tile: h.Pos, civ: st?.Civ, cause: $"{D.PATH_TR[h.Path]} yolu");
            Heroes.GainXp(s, h, h.Path == "sage" ? XP_LIBRARY_SAGE : XP_LIBRARY, "library");
        }
        h.IdleSince = s.Day;
        ReturnToBase(s, h);
    }

    private static void RobCaravan(Sim s, Hero h, Agent cv)
    {
        var c = s.W.Civs[cv.Civ];
        var side = new List<Combatant> { HeroSide(h, "A", null, true) };
        var guards = Agents.CivTroops(s, c, cv.Troops ?? 2, "B");
        var b = Combat.ResolveBattle(s.Rng, side, guards, new BattleOpts { Id = s.Id(), Day = s.Day, Tile = h.Pos, Title = "Kervan soygunu", SideA = h.Name, SideB = $"{c.Name} kervanı", MoraleA = 0.4, MoraleB = 0.55 });
        Agents.RecordBattle(s, b);
        Agents.SyncHeroes(s, side, b.Winner == "A" ? 60 : 10, guards, c.Name, "rob");
        h.Tally.Set("rob", (h.Tally.Get("rob") ?? 0) + 1); h.Tally.Set("robDay", s.Day);
        s.Metric("goalRob");
        h.Rep.Set(c.Id, (h.Rep.Get(c.Id) ?? 0) - 2);
        if (b.Winner == "A")
        {
            double v = 0;
            foreach (var kv in cv.Cargo ?? new JsObj<double>()) v += (cv.Cargo.Get(kv.Key) ?? 0) * (D.GOODS[kv.Key]?.Base ?? 1);
            double gold = JsMath.Max(8, JsMath.Round(v * 0.6));
            h.Gold += gold;
            cv.Dead = true;
            var r = J.Find(s.W.Routes, x => x.Id == cv.Route); if (r != null) r.NextDepart = s.Day + 60 / Sim.PACE;
            c.Threat += 0.15;
            Note(s, h, $"{c.Name} kervanını soydu ({J.S(gold)} altın)");
            Lore.Deed(s, h, "rob", $"{c.Name} kervanını soydu", h.Pos, c.Name);
            s.Log("raid", $"{HeroLabel(h)}, {c.Name} kervanını soydu ve {J.S(gold)} altınlık yükle kayboldu.", civ: c.Id, tile: h.Pos, battle: b.Id, major: true, cause: "Karanlık yol");
        }
        else if (h.State != "dead")
        {
            Note(s, h, $"{c.Name} kervanının muhafızlarına yenildi");
            s.Log("raid", $"{c.Name} kervanı, {Lore.Ek(h.Name, "in")} pususunu savuşturdu.", civ: c.Id, tile: h.Pos, battle: b.Id);
        }
        if (h.State != "dead") ReturnToBase(s, h);
    }

    private static void Duel(Sim s, Hero h)
    {
        var o = J.Find(s.W.Heroes, x => x.Id == h.Goal.Target);
        if (o == null || o.Civ != -1 || o.State == "dead" || o.State == "gone" || s.G.Dist(o.Pos, h.Pos) > 2)
        {
            Note(s, h, "aradığı haydudu bulamadı"); ReturnToBase(s, h); return;
        }
        var A = new List<Combatant> { HeroSide(h, "A", null, true) };
        var B = new List<Combatant> { HeroSide(o, "B", null, true) };
        var b = Combat.ResolveBattle(s.Rng, A, B, new BattleOpts { Id = s.Id(), Day = s.Day, Tile = h.Pos, Title = "Düello", SideA = HeroLabel(h), SideB = HeroLabel(o), MoraleA = 0.25, MoraleB = 0.35 });
        Agents.RecordBattle(s, b);
        Agents.SyncHeroes(s, A, 80, B, o.Name, "duel");
        Agents.SyncHeroes(s, B, 80, A, h.Name, "duel");
        s.Metric("duel");
        bool win = b.Winner == "A";
        static bool Dead(Hero x) => x.State == "dead";
        var victor = win ? h : o; var loser = win ? o : h;
        if (!Dead(victor))
        {
            Lore.Deed(s, victor, "duel", $"{(win ? "kara yola sapan" : "peşine düşen")} {Lore.Ek(loser.Name, "i")} düelloda {(Dead(loser) ? "öldürdü" : "kaçırdı")}", victor.Pos, loser.Name);
            Heroes.AddRenown(s, victor, Heroes.R_DUEL, "duel");
        }
        s.Log("hero", win ? $"{HeroLabel(h)}, kara yola sapan {Lore.Ek(o.Name, "i")} düelloda {(Dead(o) ? "öldürdü" : "kaçırdı")}." : $"{HeroLabel(o)}, peşine düşen {Lore.Ek(h.Name, "i")} düelloda {(Dead(h) ? "öldürdü" : "kaçırdı")}.",
            tile: h.Pos, battle: b.Id, major: true, cause: "İyi yürekli kahramanlar haydutları avlar");
        Note(s, h, win ? $"{o.Name} ile düelloyu kazandı" : $"{o.Name} ile düelloyu kaybetti");
        if (!Dead(o)) { Note(s, o, win ? $"{h.Name} ile düelloyu kaybetti" : $"{h.Name} ile düelloyu kazandı"); if (o.State != "traveling") ReturnToBase(s, o); }
        if (!Dead(h)) ReturnToBase(s, h);
    }

    // ------------------------------------------------------------ emeklilik
    /// <summary>Yıllık emeklilik olasılığı: yaşlılık eşiğini geçen %50, eşiğe yaklaşan (%85) %20; 20 yılı aşkın macerası
    /// olan ünlü (Sv8+ ya da efsane) %10; 30 yılı aşkın macera %8; yoksa 0.</summary>
    public static double RetireChance(Sim s, Hero h)
    {
        double career = (s.Day - h.Born) / Sim.YEAR, age = Heroes.Age(s, h), old = D.HERO_AGE[h.Race][2];
        if (age >= old) return 0.5;
        if (age >= old * 0.85) return 0.2;
        if (career >= 20 && (h.Level >= 8 || J.T(h.Legend))) return 0.1;
        if (career >= 30) return 0.08;
        return 0;
    }

    /// <summary>Yaşlı ya da yılları bulmuş kahraman emekli olabilir (handa öğretmen ya da hancı olur); serbestler
    /// tavernada, kiralıklar yurtta (sözleşmesiz) ya da sözleşme bitiminde sorulur.</summary>
    public static bool MaybeRetire(Sim s, Hero h)
    {
        double p = RetireChance(s, h);
        if (p <= 0 || !s.Rng.Chance(p)) return false;
        var inns = J.Sort(J.Filter(s.W.Inns, i => i.Alive), (a, b) => s.G.Dist(a.Tile, h.Pos) - s.G.Dist(b.Tile, h.Pos));
        // handa öğretmen ya da hancı olmak için ün gerekir (Sv4+); daha az deneyimli olan yurduna çekilir
        var inn = h.Level < 4 ? null : h.BaseInn && InnById(s, h.Base)?.Alive == true ? InnById(s, h.Base) : J.At(inns, 0);
        h.State = "retired"; h.Civ = -1; h.Contract = null; h.Goal = null; h.Auction = null;
        foreach (var a in s.W.Agents) if (a.Heroes != null && a.Heroes.Contains(h.Id) && a.Kind == "hero") a.Dead = true;
        s.Metric("retire");
        if (inn != null)
        {
            h.Pos = inn.Tile; h.Base = inn.Id; h.BaseInn = true;
            string role;
            if (inn.Teacher == null || J.Find(s.W.Heroes, x => x.Id == inn.Teacher)?.State != "retired") { inn.Teacher = h.Id; role = "öğretmenlik yapıyor; burada yetişenler bir adım önde doğacak"; }
            else { inn.Keeper = h.Name; role = "hanı devraldı ve artık hancı"; }
            Note(s, h, $"emekli oldu: {role}");
            Lore.Deed(s, h, "retired", $"kılıcını asıp {Lore.Ek(Inns.InnName(inn), "da")} {(inn.Teacher == h.Id ? "öğretmen" : "hancı")} oldu", inn.Tile, Inns.InnName(inn));
            s.Log("inn", $"{HeroLabel(h)} kılıcını astı: {inn.Name} Hanı'nda {role}.", tile: inn.Tile, major: true, cause: $"{J.S(Math.Floor((s.Day - h.Born) / Sim.YEAR))} yıllık macera, {J.S(Math.Floor(Heroes.Age(s, h)))} yaşında, Sv {J.S(h.Level)}");
        }
        else
        {
            Note(s, h, "emekli oldu");
            Lore.Deed(s, h, "retired", "kılıcını asıp yurduna çekildi", h.Pos);
            s.Log("hero", $"{HeroLabel(h)} kılıcını astı ve yurduna çekildi.", tile: h.Pos, major: true);
        }
        return true;
    }
}
