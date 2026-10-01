using System;
using System.Collections.Generic;
using System.Linq;

// Kahraman iradesi: serbest kahramanlar kiralanana dek kendi yollarını izler.
// Yol (avcı, gezgin, şifacı, bilge, paralı, karanlık), her mevsim hedef seçimi, izler ve efsane.
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

    /// <summary>Kahramanın günlüğüne satır ekler (son 6 satır tutulur).</summary>
    public static void Note(Sim s, Hero h, string text)
    {
        h.Journal.Add(new JournalEntry { Day = s.Day, Text = text });
        if (h.Journal.Count > 6) J.Shift(h.Journal);
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
            h.Base = r.Id; h.BaseInn = r.Inn; t = r.Tile;
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

    /// <summary>Kahraman efsane olur (iz, heykel, olay kaydı); bir kez.</summary>
    public static void BecomeLegend(Sim s, Hero h)
    {
        if (J.T(h.Legend)) return;
        h.Legend = true;
        AddTrait(s, h, "legend", "Seviye tavanına ulaştı");
        string where = h.BaseInn ? $"{InnById(s, h.Base)?.Name ?? "Han"} Hanı'nın" : $"{s.Settlement(h.Base)?.Name ?? "Yurdunun"}";
        s.Metric("legend");
        s.Log("hero", $"Ozanlar {HeroLabel(h)} için şarkı yakıyor; {where} önüne heykeli dikildi.", tile: h.Pos, civ: h.Civ >= 0 ? h.Civ : (int?)null, major: true, cause: $"{J.S(h.Kills)} düşman, {J.S(h.Traits.Count)} iz");
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
                AddTrait(s, h, "avenger", $"{Tr.Ek(byCamp.Name, "in")} {Tr.Ek(st.Name, "i")} yakmasının intikamı");
            }
            if (byCiv != null && h.Grudge != byCiv.Id) { h.Grudge = byCiv.Id; Note(s, h, $"{byCiv.Name} doğduğu {Tr.Ek(st.Name, "i")} yaktı; onlara asla hizmet etmeyecek"); }
        }
    }

    /// <summary>kamp savaşı sonrası: sayaçlar, izler, lakap, yol değişimi</summary>
    public static void AfterCampFight(Sim s, List<Combatant> side, Camp cp, bool won, bool hadBoss, List<BattleRoll> rolls)
    {
        var hs = J.Map(J.Filter(side, x => x.Hero != null), x => x.Hero);
        var alive = J.Filter(hs, h => h.State != "dead");
        foreach (var x in side)
        {
            var h = x.Hero; if (h == null || h.State == "dead") continue;
            h.Tally.Set("kills", (h.Tally.Get("kills") ?? 0) + x.Kills);
            if (cp.Kind == "goblin") { h.Tally.Set("goblin", (h.Tally.Get("goblin") ?? 0) + x.Kills); if (J.N(h.Tally.Get("goblin")) >= 10) AddTrait(s, h, "goblinslayer", $"{J.S(J.N(h.Tally.Get("goblin")))} goblin devirdi"); }
            if (won)
            {
                h.Tally.Set("fails", 0); h.Tally.Set("camps", (h.Tally.Get("camps") ?? 0) + 1);
                if (J.N(h.Tally.Get("camps")) >= 5 && h.Level >= 4) BecomeLegend(s, h);
                Note(s, h, $"{cp.Name} yerle bir edildi");
                if (h.Vendetta == cp.Id) { h.Vendetta = null; Note(s, h, "intikamını aldı"); }
                if (hadBoss && !J.T(h.Epithet) && J.Some(rolls, r => r.D20 == 20 && r.Who == h.Name))
                {
                    h.Epithet = $"{cp.Name.Split(' ')[0]} Belası";
                    s.Metric("epithet");
                    s.Log("hero", $"{h.Name} artık «{h.Epithet}» diye anılıyor.", tile: cp.Tile, civ: h.Civ >= 0 ? h.Civ : (int?)null, major: true, cause: "Kamp önderini doğal 20 ile devirdi");
                }
            }
            else
            {
                h.Tally.Set("fails", (h.Tally.Get("fails") ?? 0) + 1);
                Note(s, h, $"{Tr.Ek(cp.Name, "da")} püskürtüldü");
                if (h.Civ == -1 && h.Align == "evil" && h.Path != "dark" && J.N(h.Tally.Get("fails")) >= 2)
                {
                    h.Path = "dark";
                    s.Log("hero", $"Art arda yenilgiler {Tr.Ek(h.Name, "i")} karanlık yola itti; artık kervan soyuyor.", tile: h.Pos, major: true, cause: "Kötü hizalı, iki başarısız sefer");
                }
            }
        }
        if (hs.Count >= 2 && alive.Count == 1)
        {
            var h = alive[0];
            h.SoloUntil = s.Day + 2 * Sim.YEAR;
            AddTrait(s, h, "lonewolf", "Partisinin tek sağ kalanı");
        }
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

    /// <summary>Serbest kahramanlar: hedefte bekleyenler ilerler, tavernadakiler mevsimde bir kez hedef seçer.</summary>
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
            if ((Math.Floor(s.Day / 5.0) + h.Id) % 6 != 0) continue;   // her kahraman mevsimde bir kez karar verir
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
            return pa * greed >= pb * needMul;
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
        var probe = new Agent { Id = -1, Kind = "party", Civ = -1, Path = path, Step = 0, Progress = 0, Speed = 0.85, Heroes = new List<int> { h.Id }, To = cp.Id, Purpose = "quest" };
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
                if (cp == null || s.G.Dist(h.Pos, cp.Tile) > 30) continue;
                if (q.Civ >= 0 && h.Grudge == q.Civ) continue;
                double greed = 1 + JsMath.Min(0.4, q.Bounty / 300);
                // önce tavernadan kendi grubunu kurmayı dener; yetmezse yoldaki dostlara katılmayı hesaplar
                var al = NO_ALLIES;
                var crew = PartyFor(s, h, cp, 1.0, greed, al.Cs);
                if (crew == null) { al = CampAllies(s, h, cp); if (al.Agents.Count > 0) crew = PartyFor(s, h, cp, 1.0, greed, al.Cs); }
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
                if (!cp.Alive || s.G.Dist(h.Pos, cp.Tile) > 22 || J.T(w.Tiles[cp.Tile].Isle)) continue;
                if (J.Some(w.Quests, q => q.Camp == cp.Id && q.Open)) continue;
                double needMul = h.Vendetta == cp.Id ? 1.0 : 1.2;
                var al = NO_ALLIES;
                var crew = PartyFor(s, h, cp, needMul, 1, al.Cs);
                if (crew == null) { al = CampAllies(s, h, cp); if (al.Agents.Count > 0) crew = PartyFor(s, h, cp, needMul, 1, al.Cs); }
                if (crew == null) continue;
                double risk = CampRisk(s, cp, crew, al.Cs) * needMul;
                double venge = h.Vendetta == cp.Id ? 4 : 1;
                opts.Add(new Option { Kind = "hunt", Tile = cp.Tile, Score = JsMath.Max(Fit(h, "hunt"), venge > 1 ? 1 : 0) * venge * (cp.Loot / crew.Count + 25) / (JsMath.Max(0.3, risk) * DistPen(s, h.Pos, cp.Tile)), Text = venge > 1 ? $"{Tr.Ek(cp.Name, "dan")} intikam" : $"{cp.Name} avı", Crew = crew, Camp = cp });
            }
        }
        // 3) harabeler
        if (Fit(h, "ruin") > 0)
        {
            var sts = w.Settlements;
            for (int si = 0; si < sts.Count; si++)
            {
                var st = sts[si];
                if (st.Alive || (h.Seen ?? new List<int>()).Contains(st.Id) || J.Some(w.Settlements, o => o.Alive && o.Tile == st.Tile) || w.Tiles[st.Tile].Camp != null) continue;
                if (s.G.Dist(h.Pos, st.Tile) > 26) continue;
                opts.Add(new Option { Kind = "ruin", Tile = st.Tile, Target = st.Id, Score = Fit(h, "ruin") * 35 / (0.5 * DistPen(s, h.Pos, st.Tile)), Text = $"{st.Name} harabesi" });
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
                if (st.Plague != null && Fit(h, "plague") > 0 && st.Plague.Until - s.Day > 15 && !J.Some(w.Heroes, x => x.Goal != null && x.Goal.Kind == "plague" && x.Goal.Target == st.Id))
                    opts.Add(new Option { Kind = "plague", Tile = st.Tile, Target = st.Id, Score = Fit(h, "plague") * (35 + st.Plague.Severity * 20) / ((HasTrait(h, "plaguewalker") ? 0.35 : 0.7) * DistPen(s, h.Pos, st.Tile)), Text = $"salgınlı {st.Name}" });
                if (J.T(st.Civics.Get("temple")) && Fit(h, "temple") > 0 && !(h.Seen ?? new List<int>()).Contains(st.Id) && s.Day - (h.Tally.Get("templeDay") ?? -9999) > Sim.YEAR)
                    opts.Add(new Option { Kind = "temple", Tile = st.Tile, Target = st.Id, Score = Fit(h, "temple") * 12 / DistPen(s, h.Pos, st.Tile), Text = $"{Tr.Ek(st.Name, "in")} sunağına hac" });
                if (J.T(st.Civics.Get("library")) && Fit(h, "library") > 0 && !(h.Seen ?? new List<int>()).Contains(st.Id) && s.Day - (h.Tally.Get("libDay") ?? -9999) > Sim.YEAR)
                    opts.Add(new Option { Kind = "library", Tile = st.Tile, Target = st.Id, Score = Fit(h, "library") * 26 / (0.6 * DistPen(s, h.Pos, st.Tile)), Text = $"{Tr.Ek(st.Name, "in")} kütüphanesi" });
            }
        }
        // 7) kervan soygunu
        if (Fit(h, "rob") > 0 && s.Day - (h.Tally.Get("robDay") ?? -9999) > Sim.YEAR / 2.0)
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
                if (o.Civ != -1 || o.Path != "dark" || !J.T(o.Tally.Get("rob") ?? 0) || ReferenceEquals(o, h) || (o.State != "tavern" && o.State != "quest") || s.G.Dist(h.Pos, o.Pos) > 20) continue;
                double risk = Combat.PowerOf(new List<Combatant> { HeroSide(o, "B") }) / me;
                if (risk > 1.1) continue;
                opts.Add(new Option { Kind = "duel", Tile = o.Pos, Target = o.Id, Score = Fit(h, "duel") * 30 / (JsMath.Max(0.3, risk) * DistPen(s, h.Pos, o.Pos)), Text = $"kara yola sapan {o.Name}" });
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
                Note(s, x, o.Kind == "quest" ? $"{cp.Name} ilanını kopardı" : $"{Tr.Ek(cp.Name, "a")} ava çıktı");
            }
            w.Agents.Add(new Agent { Id = s.Id(), Kind = "party", Civ = -1, Path = path, Step = 0, Progress = 0, Speed = 0.85, Heroes = J.Map(crew, x => x.Id), To = cp.Id, Quest = o.Quest?.Id, Purpose = "quest" });
            s.Metric(o.Kind == "quest" ? "questTaken" : "goalHunt");
            string poster = o.Quest != null ? (o.Quest.Civ >= 0 ? w.Civs[o.Quest.Civ].Name : $"{InnById(s, o.Quest.Inn)?.Name ?? "Han"} Hanı") : "";
            s.Log("quest", crew.Count > 1
                ? $"Bir macera grubu kuruldu: {string.Join(", ", J.Map(crew, x => s.HeroTitle(x)))}. Hedef: {cp.Name}."
                : o.Quest != null ? $"{s.HeroTitle(h)}, {Tr.Ek(poster, "in")} ilanını kopardı ve {Tr.Ek(cp.Name, "a")} yola çıktı." : $"{s.HeroTitle(h)} kendi başına {Tr.Ek(cp.Name, "a")} ava çıktı.",
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
        w.Agents.Add(new Agent { Id = s.Id(), Kind = "hero", Civ = -1, Path = path2, Step = 0, Progress = 0, Speed = 0.9, Heroes = new List<int> { h.Id }, Purpose = "goal" });
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
                    (h.Seen ??= new List<int>()).Add(g.Target.Value);
                    double bonus = HasTrait(h, "ruinrat") ? 2 : 0;
                    double roll = s.Rng.Dice(1, 20) + Rng.Mod(J.N(h.Stats.Get("dex"))) + bonus;
                    if (roll < 11)
                    {
                        double dmg = s.Rng.Dice(JsMath.Max(1, Math.Ceiling(h.Level / 2.0)), 8);
                        h.Hp = JsMath.Max(1, h.Hp - dmg);
                        Note(s, h, $"{st?.Name ?? "harabe"} tuzağında yaralandı");
                        s.Log("hero", $"{h.Name}, {st?.Name ?? "bir"} harabesinde tuzağa düştü ({J.S(dmg)} hasar).", tile: h.Pos, cause: $"d20 {J.S(roll)}");
                    }
                    else
                    {
                        double gold = s.Rng.Int(10, 40) + bonus * 5;
                        h.Gold += gold;
                        string found = "";
                        if (s.Rng.Chance(0.12 + bonus * 0.04)) { h.Bonus.Atk++; found = " ve eski bir büyülü silah"; }
                        Note(s, h, $"{st?.Name ?? "harabe"} harabesinde {J.S(gold)} altın{found} buldu");
                        s.Log("hero", $"{h.Name}, {st?.Name ?? "bir"} harabesinde {J.S(gold)} altın{found} buldu.", tile: h.Pos, major: J.T(found), cause: $"{D.PATH_TR[h.Path]} yolu");
                        Heroes.GainXp(s, h, 70);
                    }
                    h.Tally.Set("ruins", (h.Tally.Get("ruins") ?? 0) + 1);
                    if (J.N(h.Tally.Get("ruins")) >= 3) AddTrait(s, h, "ruinrat", $"{J.S(J.N(h.Tally.Get("ruins")))} harabe keşfetti");
                    g.Stay = s.Day + 5;
                    return;
                }
            case "plague":
                {
                    var st = s.Settlement(g.Target);
                    if (st == null || !st.Alive || st.Plague == null) { ReturnToBase(s, h); return; }
                    var pl = st.Plague;
                    double heal = h.Cls == "cleric" || h.Cls == "druid" || h.Cls == "paladin" ? 0.55 : 0.75;
                    pl.Severity *= heal; pl.Until = JsMath.Max(s.Day + 8, pl.Until - 25);
                    s.Metric("plagueHealed");
                    var c = w.Civs[st.Civ];
                    h.Rep.Set(c.Id, (h.Rep.Get(c.Id) ?? 0) + 1);
                    h.Tally.Set("plague", (h.Tally.Get("plague") ?? 0) + 1);
                    if (!HasTrait(h, "plaguewalker") && s.Rng.Dice(1, 20) + Rng.Mod(J.N(h.Stats.Get("con"))) < 10)
                    {
                        double dmg = Math.Ceiling(h.MaxHp * 0.35);
                        h.Hp = JsMath.Max(1, h.Hp - dmg);
                        Note(s, h, $"{Tr.Ek(st.Name, "da")} hastalığa yakalandı ama atlattı");
                    }
                    AddTrait(s, h, "plaguewalker", $"Salgınlı {Tr.Ek(st.Name, "da")} şifa dağıttı");
                    Note(s, h, $"{Tr.Ek(st.Name, "da")} şifa dağıttı");
                    s.Log("hero", $"{s.HeroTitle(h)} salgınlı {Tr.Ek(st.Name, "a")} şifa taşıdı; hastalık geriliyor.", tile: st.Tile, civ: c.Id, major: true, cause: $"{D.PATH_TR[h.Path]} yolu");
                    Heroes.GainXp(s, h, 90);
                    g.Stay = s.Day + 15;
                    return;
                }
            case "temple":
                {
                    (h.Seen ??= new List<int>()).Add(g.Target.Value);
                    h.Hp = h.MaxHp;
                    h.Tally.Set("temple", (h.Tally.Get("temple") ?? 0) + 1); h.Tally.Set("templeDay", s.Day);
                    Heroes.GainXp(s, h, 30);
                    Note(s, h, $"{s.Settlement(g.Target)?.Name ?? "bir"} sunağında dua etti");
                    g.Stay = s.Day + 5;
                    return;
                }
            case "library":
                {
                    (h.Seen ??= new List<int>()).Add(g.Target.Value);
                    g.Stay = s.Day + 20;
                    return;
                }
            case "rob": g.Stay = s.Day + 40; return;
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
            s.Log("hero", $"{h.Name}, {Tr.Ek(st?.Name ?? "kütüphane", "in")} kütüphanesinde eski kitaplardan yeni büyüler öğrendi.", tile: h.Pos, civ: st?.Civ, cause: $"{D.PATH_TR[h.Path]} yolu");
            Heroes.GainXp(s, h, h.Path == "sage" ? 110 : 60);
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
        Agents.SyncHeroes(s, side, b.Winner == "A" ? 60 : 10);
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
            var r = J.Find(s.W.Routes, x => x.Id == cv.Route); if (r != null) r.NextDepart = s.Day + 60;
            c.Threat += 0.15;
            Note(s, h, $"{c.Name} kervanını soydu ({J.S(gold)} altın)");
            s.Log("raid", $"{HeroLabel(h)}, {c.Name} kervanını soydu ve {J.S(gold)} altınlık yükle kayboldu.", civ: c.Id, tile: h.Pos, battle: b.Id, major: true, cause: "Karanlık yol");
        }
        else if (h.State != "dead")
        {
            Note(s, h, $"{c.Name} kervanının muhafızlarına yenildi");
            s.Log("raid", $"{c.Name} kervanı, {Tr.Ek(h.Name, "in")} pususunu savuşturdu.", civ: c.Id, tile: h.Pos, battle: b.Id);
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
        var ab = new List<Combatant>(A);
        ab.AddRange(B);
        Agents.SyncHeroes(s, ab, 80);
        s.Metric("duel");
        bool win = b.Winner == "A";
        static bool Dead(Hero x) => x.State == "dead";
        s.Log("hero", win ? $"{HeroLabel(h)}, kara yola sapan {Tr.Ek(o.Name, "i")} düelloda {(Dead(o) ? "öldürdü" : "kaçırdı")}." : $"{HeroLabel(o)}, peşine düşen {Tr.Ek(h.Name, "i")} düelloda {(Dead(h) ? "öldürdü" : "kaçırdı")}.",
            tile: h.Pos, battle: b.Id, major: true, cause: "İyi yürekli kahramanlar haydutları avlar");
        Note(s, h, win ? $"{o.Name} ile düelloyu kazandı" : $"{o.Name} ile düelloyu kaybetti");
        if (!Dead(o)) { Note(s, o, win ? $"{h.Name} ile düelloyu kaybetti" : $"{h.Name} ile düelloyu kazandı"); if (o.State != "traveling") ReturnToBase(s, o); }
        if (!Dead(h)) ReturnToBase(s, h);
    }

    // ------------------------------------------------------------ emeklilik
    /// <summary>Yaşlı ve ünlü kahraman emekli olabilir (handa öğretmen ya da hancı olur).</summary>
    public static bool MaybeRetire(Sim s, Hero h)
    {
        if (h.Level < 4 || s.Day - h.Born < 12 * Sim.YEAR || !s.Rng.Chance(0.25)) return false;
        var inns = J.Sort(J.Filter(s.W.Inns, i => i.Alive), (a, b) => s.G.Dist(a.Tile, h.Pos) - s.G.Dist(b.Tile, h.Pos));
        var inn = h.BaseInn && InnById(s, h.Base)?.Alive == true ? InnById(s, h.Base) : J.At(inns, 0);
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
            s.Log("inn", $"{HeroLabel(h)} kılıcını astı: {inn.Name} Hanı'nda {role}.", tile: inn.Tile, major: true, cause: $"{J.S(Math.Floor((s.Day - h.Born) / Sim.YEAR))} yıllık macera, Sv {J.S(h.Level)}");
        }
        else
        {
            Note(s, h, "emekli oldu");
            s.Log("hero", $"{HeroLabel(h)} kılıcını astı ve yurduna çekildi.", tile: h.Pos, major: true);
        }
        return true;
    }
}
