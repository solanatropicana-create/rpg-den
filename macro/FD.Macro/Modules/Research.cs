using System;
using System.Collections.Generic;
using System.Linq;

// Araştırma seçimi, çağ atlama, alt sınıf doktrinleri ve yıllık sınıf yetenekleri.
// Port of src/sim/research.ts.

namespace FD.Macro;

/// <summary>research.ts <c>Surge</c> (local type of wildSurge): one wild-magic surge outcome.</summary>
public sealed class Surge
{
    public bool Good;
    /// <summary>TS <c>run: () =&gt; string</c>: applies the surge, returns its description.</summary>
    public Func<string> Run;
}

public static class Research
{
    /// <summary>Techs the civ can research now (own tree, era, all reqs done, resource gates ok).</summary>
    public static List<TechDef> AvailableTechs(Sim s, Civ c)
    {
        return J.Filter(D.ALL_TECHS, t => !s.Has(c, t.Id) && (t.Tree == "main" || t.Tree == c.Cls) && t.Era <= c.Era
            && J.Every(t.Req, r => s.Has(c, r)) && GateOk(s, c, t));
    }

    /// <summary>Resource gates of a tech: every <c>gate</c> key and some <c>gateAny</c> key accessible.</summary>
    public static bool GateOk(Sim s, Civ c, TechDef t)
    {
        return (t.Gate == null || J.Every(t.Gate, k => s.Access(c, k))) && (t.GateAny == null || J.Some(t.GateAny, k => s.Access(c, k)));
    }

    /// <summary>Techs that would be available but are blocked by a resource gate.</summary>
    public static List<TechDef> BlockedByGate(Sim s, Civ c)
    {
        return J.Filter(D.ALL_TECHS, t => !s.Has(c, t.Id) && (t.Tree == "main" || t.Tree == c.Cls) && t.Era <= c.Era && J.Every(t.Req, r => s.Has(c, r)) && !GateOk(s, c, t));
    }

    private static readonly string[] FOOD_TECHS = { "agriculture", "husbandry", "fishing" };

    /// <summary>TS <c>o[k]</c> on a number record with a possibly-undefined key (<c>o[undefined]</c> reads key "undefined").</summary>
    private static double? GetKey(JsObj<double> o, string k) => o.Get(k ?? "undefined");

    /// <summary>Picks the next research by weighted reasons (hard, costlier path when everything is gated).</summary>
    public static void ChooseResearch(Sim s, Civ c)
    {
        var avail = AvailableTechs(s, c);
        c.Research.Hard = false;
        if (avail.Count == 0)
        {
            // kaynak kapısına takıldı: uzun ve pahalı yoldan (başka yöntemlerle) öğrenmeye çalışır
            // kıyısı olmayan denizcilik öğrenemez
            avail = J.Filter(BlockedByGate(s, c), t => !(t.Gate ?? new List<string>()).Contains("coast"));
            if (avail.Count == 0) { c.Research.Current = null; return; }
            c.Research.Hard = true;
        }
        var cls = D.CLASSES[c.Cls];
        var ss = s.CivSettlements(c);
        double pop = s.CivPop(c);
        double daysFood = s.FoodTotal(c) / JsMath.Max(0.1, pop * Sim.FOOD_PER_POP);
        bool housingFull = J.Some(ss, x => s.Pop(x) >= s.Housing(x) - 1);
        bool war = s.InWar(c);
        double threat = JsMath.Min(1, c.Threat);
        double eraNeed = c.Era < 4 ? D.ERA_RULE[c.Era + 1].Nodes - J.Filter(s.W.Civs[c.Id].Research.Done, t => D.TECH[t].Tree == "main" && D.TECH[t].Era == c.Era).Count : 0;
        // sahip olunan yapılar ve yükseltme potansiyeli
        var extCount = new JsObj<double>();
        foreach (var t in s.W.Tiles) if (t.Ext != null && t.Owner >= 0 && s.Settlement(t.Owner)?.Civ == c.Id) extCount.Set(t.Ext.Kind, (extCount.Get(t.Ext.Kind) ?? 0) + 1);
        var reasons = new Dictionary<string, List<(double V, string Why)>>();
        void Add(string id, double v, string why)
        {
            if (!reasons.TryGetValue(id, out var l)) { l = new List<(double V, string Why)>(); reasons[id] = l; }
            l.Add((v, why));
        }
        foreach (var t in avail)
        {
            double pref = t.Tree == "main" ? (GetKey(cls.Prefer, t.Chain) ?? 1) : (cls.Prefer.Get("class") ?? 1.2);
            Add(t.Id, 3 * pref, t.Tree == "main" ? $"{cls.Name} geleneği" : $"{cls.Name} yolu");
            if (t.Tree == "main" && t.Era == c.Era && eraNeed > 0) Add(t.Id, 4, $"{D.ERA_TR[c.Era + 1]} çağına geçmek için");
            if (J.T(t.Subclass)) Add(t.Id, c.Era >= 3 ? 25 : 9, "Kimliğini seçme zamanı");
            // yükseltme açan düğümler
            foreach (var k in D.EXTRACTS.Keys())
            {
                int lv = D.EXTRACTS[k].Tech.IndexOf(t.Id);
                if (lv >= 0 && J.T(extCount.Get(k))) Add(t.Id, JsMath.Min(10, extCount.Get(k).Value * 2.5), $"{J.S(extCount.Get(k).Value)} {J.TrLower(D.EXTRACTS[k].Names[Math.Max(0, lv - 1)])} yükseltilebilir");
                if (lv == 0 && k == "mine" && J.Some(s.W.Deposits, d => D.DEPOSITS[d.Kind].Building == "mine" && s.DepositVisible(c, d) && J.Some(d.Tiles, x => s.TileCiv(x) == c.Id))) Add(t.Id, 7, "Topraklarında maden damarı var");
            }
            if (daysFood < 40 && t.Chain == "gida" && t.Tree == "main") Add(t.Id, 14, "Kıtlık kapıda");
            if (Array.IndexOf(FOOD_TECHS, t.Id) >= 0 && !FOOD_TECHS.Any(x => s.Has(c, x))) Add(t.Id, 9, "Kalıcı bir gıda kaynağı");
            switch (t.Id)
            {
                case "agriculture": Add(t.Id, daysFood < 60 ? 22 : 5, daysFood < 60 ? "Gıda stoğu azalıyor" : "Düzenli gıda"); break;
                case "woodwork": Add(t.Id, housingFull ? 10 : 4, housingFull ? "Barınak yetersiz" : "Kalıcı evler"); break;
                case "pottery": Add(t.Id, 5, "Depo ve fırın"); break;
                case "bronze": Add(t.Id, 6, "Bronz; ilk askerlerin yolu"); break;
                case "training": Add(t.Id, 2 + threat * 10 + (war ? 12 : 0) + cls.Aggression * 8, threat > 0.3 ? "Goblin baskınları" : war ? "Savaş" : "Ordu kurma isteği"); break;
                case "tavern": Add(t.Id, 5 + threat * 4, "Kahramanları çekmek"); break;
                case "roads": Add(t.Id, pop > 13 ? 8 : 2, pop > 13 ? "Kalabalık yeni toprak istiyor" : "Bağlantı"); break;
                case "barter": Add(t.Id, 4, "Komşularla alışveriş"); break;
                case "caravans": Add(t.Id, 4 + (cls.Prefer.Get("ticaret") ?? 1) * 2, "Uzak pazarlar"); break;
                case "stonewalls": Add(t.Id, 2 + threat * 8 + (war ? 8 : 0), "Surlar"); break;
                case "smithing": Add(t.Id, 6 + (war ? 6 : 0), "Çelik"); break;
                case "arcana1": Add(t.Id, c.Cls == "wizard" ? 10 : 2, "Büyünün sırrı"); break;
                case "writing": Add(t.Id, 5, "Bilgiyi kayda geçirmek"); break;
                case "boatbuilding": Add(t.Id, 4 + JsMath.Min(6, (extCount.Get("dock") ?? 0) * 2), J.T(extCount.Get("dock") ?? 0) ? "İskeleler kalabalık, açığa çıkmak" : "Kıyıdan açılmak"); break;
                case "shipbuilding": { bool isl = J.Some(s.W.Tiles, x => J.T(x.Isle) && x.Owner < 0); Add(t.Id, 3 + (isl ? 5 : 0) + (ss.Count >= 4 ? 5 : 0), ss.Count >= 4 ? "Anakara daralıyor; denizaşırı topraklar" : isl ? "Ufukta bakir adalar" : "Yük gemileri"); break; }
                case "navigation": Add(t.Id, 4 + (J.Some(ss, x => J.T(x.Overseas)) ? 4 : 0), "Ufkun ötesi"); break;
                case "seatrade": Add(t.Id, 3 + J.Filter(s.W.Routes, r => r.Alive && J.T(r.Sea) && (s.Settlement(r.A)?.Civ == c.Id || s.Settlement(r.B)?.Civ == c.Id)).Count * 3, "Deniz yolları"); break;
                case "navy": Add(t.Id, 2 + cls.Aggression * 9 + (war ? 8 : 0), war ? "Savaşı denize taşımak" : "Denizde güç"); break;
            }
        }
        double Score(string id)
        {
            double sum = 0;
            foreach (var (v, _) in reasons[id]) sum = sum + v;
            return sum * JsMath.Pow(150 / D.TechCost(D.TECH[id]), 0.3);
        }
        var pick = s.Rng.Weighted(avail, t => { double sc = Score(t.Id); return sc * sc; });
        var top = J.Sorted(reasons[pick.Id], (a, b) => b.V - a.V)[0];
        c.Research.Current = pick.Id;
        c.Research.Progress = 0;
        c.Research.Reason = J.T(c.Research.Hard)
            ? $"Kaynağı yok ({string.Join(", ", J.Map(J.Filter(pick.Gate ?? pick.GateAny ?? new List<string>(), k => !s.Access(c, k)), k => GATE_TR.TryGetValue(k, out var tr) ? tr : k))}); uzun ve pahalı yoldan"
            : top.Why;
    }

    /// <summary>
    /// Araştırmadan öğrenilen düğüm (casusluk, peri paktı, yağma): Done'a eklenir, etkiler yeniden hesaplanır,
    /// stolenTech sayılır. Bilinen düğüm yeniden eklenmez. Çalınan düğüm o an araştırılan düğümse araştırma boşa
    /// gitmez: yeni düğüm hemen seçilir ve birikmiş ilerleme ona aktarılır. (Eskiden aynı düğüm araştırılmaya devam
    /// eder, bitince Done'a ikinci kez eklenirdi; etkileri iki kez toplanır, çağ sayımı şişerdi.)
    /// </summary>
    public static void StealTech(Sim s, Civ c, string id)
    {
        if (s.Has(c, id)) return;
        c.Research.Done.Add(id);
        s.RecomputeEff(c);
        s.Metric("stolenTech");
        if (c.Research.Current != id) return;
        double carry = c.Research.Progress;
        c.Research.Current = null;
        c.Research.Progress = 0;
        s.Metric("stolenCurrent");
        ChooseResearch(s, c);
        if (J.T(c.Research.Current)) c.Research.Progress = carry;
    }

    /// <summary>Tech completion: subclass pick / capstone / effects, logs, then chooseResearch.</summary>
    public static void OnTechDone(Sim s, Civ c, string id)
    {
        var t = D.TECH[id];
        var cls = D.CLASSES[c.Cls];
        s.Metric("techs");
        s.Metric($"research_{c.Id}");
        if (J.T(t.Subclass))
        {
            var ctx = new PickCtx { War = s.InWar(c), Threat = JsMath.Min(1, c.Threat), Forest = ForestShare(s, c), Law = c.Align.Law, Good = c.Align.Good, Pop = s.CivPop(c) };
            var sub = s.Rng.Weighted(cls.Subclasses, x => x.Pick(ctx));
            c.Subclass = sub.Id;
            s.RecomputeEff(c);
            s.Metric("subclass");
            s.Log("class", $"{c.Name} {t.Name} ile yolunu seçti: {sub.Name}.", civ: c.Id, major: true, cause: $"{JsStr(sub.Desc)}");
        }
        else if (J.T(t.Capstone))
        {
            s.RecomputeEff(c);
            s.Log("class", $"{c.Name} sınıfının doruğuna ulaştı: {s.CapName(c)}!", civ: c.Id, major: true, cause: J.Find(D.CLASSES[c.Cls].Subclasses, x => x.Id == c.Subclass)?.CapDesc);
        }
        else
        {
            s.RecomputeEff(c);
            bool big = t.Tree != "main" || t.Era >= 3 || J.T(t.Unit) || Array.IndexOf(BIG_TECHS, id) >= 0;
            s.Log("research", $"{c.Name} {t.Name} araştırmasını tamamladı: {J.TrLower(CharAt0(t.Unlock))}{SliceFrom1(t.Unlock)}.", civ: c.Id, cause: c.Research.Reason, major: big);
        }
        if (J.T(t.Unit)) s.Log("class", $"{c.Name} yeni bir birlik kurabiliyor: {D.UNITS[t.Unit].Name}.", civ: c.Id);
        if (id == "arcana1" || id == "deepmine")
        {
            string kind = id == "arcana1" ? "mana" : "mithril";
            var seen = J.Filter(s.W.Deposits, d => d.Kind == kind && d.KnownBy.Contains(c.Id));
            if (seen.Count > 0) s.Log("discover", $"{c.Name} artık {J.TrLower(D.DEPOSITS[kind].Name)} yataklarını görebiliyor ({seen.Count}).", civ: c.Id, tile: seen[0].Tiles[0], major: true);
        }
        ChooseResearch(s, c);
    }

    private static readonly string[] BIG_TECHS = { "tavern", "training", "roads", "bronze", "smithing", "arcana1", "deepmine", "currency" };

    /// <summary>JS <c>`${x}`</c> for a possibly-undefined string.</summary>
    private static string JsStr(string x) => x ?? "undefined";

    /// <summary>JS <c>s.charAt(0)</c> ("" for an empty string).</summary>
    private static string CharAt0(string x) => x.Length > 0 ? x.Substring(0, 1) : "";

    /// <summary>JS <c>s.slice(1)</c>.</summary>
    private static string SliceFrom1(string x) => x.Length > 1 ? x.Substring(1) : "";

    private static readonly Dictionary<string, string> GATE_TR = new()
    {
        ["water"] = "su kenarı", ["coast"] = "deniz kıyısı", ["fertile"] = "verimli ova", ["clay"] = "kil", ["copper"] = "bakır", ["tin"] = "kalay", ["iron"] = "demir", ["gold"] = "altın", ["silver"] = "gümüş", ["horses"] = "at sürüsü", ["salt"] = "tuz", ["herbs"] = "şifalı ot", ["mana"] = "mana", ["mithril"] = "mithril", ["heartwood"] = "kadim ağaç",
    };

    private static double ForestShare(Sim s, Civ c)
    {
        double f = 0, n = 0;
        foreach (var t in s.W.Tiles) if (t.Owner >= 0 && s.Settlement(t.Owner)?.Civ == c.Id) { n++; if (t.Terrain == "forest" || t.Terrain == "oldforest") f++; }
        return J.T(n) ? f / n : 0;
    }

    /// <summary>Advances the civ's era when enough main-tree nodes of the current era and population are reached.</summary>
    public static void EraCheck(Sim s, Civ c)
    {
        if (c.Era >= 4) return;
        int next = c.Era + 1;
        var rule = D.ERA_RULE[next];
        int done = J.Filter(c.Research.Done, t => D.TECH[t].Tree == "main" && D.TECH[t].Era == c.Era).Count;
        // Math.max(0, ...pops)
        double maxPop = 0;
        foreach (var x in s.CivSettlements(c)) maxPop = JsMath.Max(maxPop, s.Pop(x));
        if (done >= rule.Nodes && maxPop >= rule.Pop)
        {
            c.Era = next;
            // JS c.eraDay[next] = day: eraDay starts as [0, day] and grows one era at a time, so this is an append
            if (next < c.EraDay.Count) c.EraDay[next] = s.Day;
            else { while (c.EraDay.Count < next) c.EraDay.Add(0); c.EraDay.Add(s.Day); }
            s.Metric($"era{next}");
            s.UpdateTerritory();
            s.Log("era", $"{c.Name} {D.ERA_TR[next]} çağına geçti!", civ: c.Id, major: true, cause: $"{done} {D.ERA_TR[next - 1]} düğümü tamam, en büyük yerleşim {J.S(maxPop)} nüfus");
            if (!J.T(c.Research.Current)) ChooseResearch(s, c);
        }
    }

    /// <summary>Yılda bir tetiklenen sınıf yetenekleri</summary>
    public static void ClassYearly(Sim s, Civ c)
    {
        var cap = s.Capital(c);
        if (cap == null) return;
        // NB: contacts is sorted in place further down (legendSteal, puppet) — later users see that order, like TS
        var contacts = J.Filter(s.W.Civs, o => o.Id != c.Id && o.Alive && s.Rel(c.Id, o.Id).Contact);
        // Rahip: Kanalize İlahiyat bayramı
        if (s.E(c, "festival") > 0)
        {
            string what = "";
            switch (c.Subclass)
            {
                case "forge": s.Add(c, "arms", 2); s.Add(c, "tools", 3); what = "Örs tanrısı silah ve alet bağışladı"; break;
                case "war": foreach (var st in s.CivSettlements(c)) st.Soldiers += 1; what = "Her yerleşime bir kutsal savaşçı katıldı"; break;
                case "life": s.AddPop(cap, c.Race, 3); what = "Başkentte üç sağlıklı çocuk doğdu"; break;
                default: s.Add(c, "grain", 40); what = "Ambarlar bereketlendi"; break;
            }
            foreach (var h in s.CivHeroes(c)) h.Hp = h.MaxHp;
            s.Log("class", $"{c.Name} Kanalize İlahiyat bayramını kutladı: {J.TrLower(what)}.", civ: c.Id, tile: cap.Tile);
        }
        // Druid: orman büyür
        double fg = s.E(c, "forestGrow");
        if (fg > 0)
        {
            int grown = 0;
            foreach (var st in s.CivSettlements(c))
            {
                foreach (int ti in s.G.Within(st.Tile, s.RadiusOf(st)))
                {
                    if (grown >= fg * 2) break;
                    var t = s.W.Tiles[ti];
                    if (t.Owner != st.Id || t.Terrain != "grass" || t.Deposit >= 0 || t.Ext != null || ti == st.Tile) continue;
                    if (!J.Some(s.G.Neighbors(ti), n => s.W.Tiles[n].Terrain == "forest" || s.W.Tiles[n].Terrain == "oldforest")) continue;
                    t.Terrain = "forest"; t.Wood = 160; grown++;
                }
            }
            if (J.T(grown)) { s.ClearPaths(); s.Log("class", $"{c.Name} topraklarında orman {grown} hex genişledi.", civ: c.Id, tile: cap.Tile); }
        }
        // Haydut: casusluk
        double spy = s.E(c, "spy");
        if (spy > 0 && contacts.Count > 0)
        {
            var o = s.Rng.Pick(contacts);
            var cand = J.Filter(o.Research.Done, t => !s.Has(c, t) && D.TECH[t].Tree == "main" && D.TECH[t].Era <= c.Era && J.Every(D.TECH[t].Req, r => s.Has(c, r)));
            if (cand.Count > 0 && s.Rng.Chance(spy))
            {
                string t = s.Rng.Pick(cand);
                StealTech(s, c, t);
                bool caught = s.Rng.Chance(0.35);
                if (caught) s.AddMod(o.Id, c.Id, "spy", "Yakalanan casuslar", -12, -30, 0.02, false);
                s.Log("class", $"{c.Name} casusları {Tr.Ek(o.Name, "dan")} {D.TECH[t].Name} bilgisini çaldı{(caught ? " ama yakalandılar" : "")}.", civ: c.Id, major: true);
            }
        }
        // Suikast
        double dt = s.E(c, "deathTouch");
        if (dt > 0)
        {
            var enemies = J.Filter(contacts, o => s.AtWar(c.Id, o.Id) || s.RelValue(c.Id, o.Id) < -30);
            var victims = new List<Hero>();
            foreach (var o in enemies) victims.AddRange(s.CivHeroes(o));
            if (victims.Count > 0 && s.Rng.Chance(dt))
            {
                var h = s.Rng.Pick(victims);
                h.State = "dead"; h.DeathDay = s.Day; h.Hp = 0;
                s.Metric("assassination");
                s.AddMod(h.Civ, c.Id, "assassin", "Suikast şüphesi", -20, -40, 0.02, false);
                s.Log("class", $"{s.HeroTitle(h)} karanlık bir sokakta suikaste kurban gitti.", civ: c.Id, major: true, cause: $"{c.Name} gölgelerinin işi olduğu fısıldanıyor");
            }
        }
        // Usta hırsız
        if (s.E(c, "legendSteal") > 0 && contacts.Count > 0)
        {
            var rich = J.Sort(contacts, (a, b) => s.St(b, "gold") - s.St(a, "gold"))[0];
            string g = s.St(rich, "mithril") >= 1 ? "mithril" : s.St(rich, "mana") >= 2 ? "mana" : "gold";
            double q = g == "gold" ? JsMath.Min(40, Math.Floor(s.St(rich, "gold") * 0.2)) : g == "mana" ? 2 : 1;
            if (q > 0) { s.Add(rich, g, -q); s.Add(c, g, q); s.Log("class", $"{c.Name} hırsızları {rich.Name} hazinesinden {J.S(q)} {(g == "gold" ? "altın" : g == "mana" ? "mana kristali" : "mithril")} aşırdı.", civ: c.Id); }
        }
        // Gölge Kral
        if (s.E(c, "puppet") > 0 && !J.T(c.Yearly.Get("puppet")) && contacts.Count > 0)
        {
            var o = J.Sort(contacts, (a, b) => s.CivPop(a) - s.CivPop(b))[0];
            c.Yearly.Set("puppet", o.Id + 1);
            s.SetMod(o.Id, c.Id, "puppet", "Gölge Kral'ın sözü", 35, 0, false);
            s.Log("class", $"{o.Name} artık gizlice {c.Name} çıkarlarına hizmet ediyor.", civ: c.Id, major: true);
        }
        // İlahi Müdahale
        if (s.E(c, "divine") > 0 && (s.InWar(c) || J.Some(s.CivSettlements(c), x => x.Starving > 0)) && s.Rng.Chance(s.E(c, "divine")))
        {
            s.Add(c, "grain", 80);
            foreach (var st in s.CivSettlements(c)) st.Soldiers += 2;
            s.Log("class", $"Tanrılar {c.Name} için müdahale etti: ambarlar doldu, savunucular çoğaldı.", civ: c.Id, major: true);
        }
        // Ozan: şarkı festivali — altın, dostluk, göçmen
        double charm = s.E(c, "charm");
        if (charm > 0)
        {
            double gold = JsMath.Round(6 + contacts.Count * 4 * charm);
            s.Add(c, "gold", gold);
            foreach (var o in contacts) s.AddMod(o.Id, c.Id, "festival", "Ozan festivaline davet", 6 * charm, 18, 0.02, false);
            string came = "";
            if (contacts.Count > 0 && s.Pop(cap) < s.Housing(cap) + 1 && s.Rng.Chance(0.5 + charm * 0.2))
            {
                var o = s.Rng.Pick(contacts);
                double n = s.Rng.Int(1, 2);
                s.AddPop(cap, o.Race, n);
                came = $" {J.S(n)} {J.TrLower(D.RACES[o.Race].Name)} şarkılara kapılıp kaldı";
            }
            s.Log("class", $"{Tr.Ek(cap.Name, "da")} büyük ozan festivali: {J.S(gold)} altın toplandı{(J.T(came) ? "," + came : "")}.", civ: c.Id, tile: cap.Tile, cause: "İlham: komşular davetli");
        }
        // Savaşçı: yıllık turnuva
        if (c.Cls == "fighter" && s.Has(c, "training"))
        {
            cap.Soldiers += 1;
            var hs = J.Filter(s.CivHeroes(c), h => h.State == "home");
            foreach (var h in hs) Heroes.GainXp(s, h, 150);
            s.Log("class", $"{Tr.Ek(cap.Name, "da")} lejyon turnuvası düzenlendi{(hs.Count > 0 ? $"; {string.Join(", ", J.Map(hs, h => h.Name))} şan kazandı" : "")}.", civ: c.Id, tile: cap.Tile, cause: "Aksiyon Dalgası: savaş sanatı sürekli bilenir");
        }
        // Keşiş: meditasyon inzivası
        if (c.Cls == "monk" && J.T(c.Research.Current))
        {
            var t = D.TECH[c.Research.Current];
            c.Research.Progress += D.TechCost(t) * 0.3;
            s.Log("class", $"{c.Name} keşişleri inzivaya çekildi; {t.Name} araştırması hızlandı.", civ: c.Id, tile: cap.Tile, cause: "Disiplin");
        }
        // Paktçı: patronun hediyesi ve bedeli
        if (s.E(c, "pact") > 0)
        {
            string gift = "";
            switch (c.Subclass)
            {
                case "fiend": cap.Soldiers += 3; s.Add(c, "arms", 3); gift = "kara alevden üç savaşçı ve silahlar geldi"; break;
                case "archfey":
                {
                    s.Add(c, "gold", 40);
                    var o = contacts.Count > 0 ? s.Rng.Pick(contacts) : null;
                    var cand = o != null ? J.Filter(o.Research.Done, t => !s.Has(c, t) && D.TECH[t].Tree == "main" && D.TECH[t].Era <= c.Era && J.Every(D.TECH[t].Req, r => s.Has(c, r))) : new List<string>();
                    if (o != null && cand.Count > 0) { string t = s.Rng.Pick(cand); StealTech(s, c, t); gift = $"40 altın ve {Tr.Ek(o.Name, "in")} rüyalarından çalınan {D.TECH[t].Name} bilgisi"; }
                    else gift = "peri altını (40)";
                    break;
                }
                case "oldone": if (J.T(c.Research.Current)) c.Research.Progress += D.TechCost(D.TECH[c.Research.Current]) * 0.45; s.Add(c, "mana", 4); gift = "yıldızların ötesinden gelen fısıltılar araştırmayı hızlandırdı"; break;
                default: s.Add(c, "gold", 25); gift = "25 altın"; break;
            }
            double n = s.Pop(cap) > 40 && s.Rng.Chance(0.3) ? 2 : s.Pop(cap) > 20 ? 1 : 0;
            if (J.T(n)) s.RemovePop(cap, n);
            foreach (var o in contacts) if (o.Align.Good > 0.3) s.AddMod(o.Id, c.Id, "pact", "Karanlık pakt söylentileri", -6, -24, 0.01, false);
            s.Metric("pact");
            s.Log("class", $"{c.Name} patronundan hediyesini aldı: {gift}.", civ: c.Id, tile: cap.Tile, major: true, cause: J.T(n) ? $"Bedeli: {J.S(n)} kişi bir gece gölgelere karışıp kayboldu" : "Bu yıl bedel ertelendi");
        }
        // Kan Büyücüsü: yabani büyü dalgası
        if (s.E(c, "wild") > 0) WildSurge(s, c, cap);
    }

    private static readonly string[] SURGE_RACES = { "tiefling", "dragonborn", "human" };

    private static void WildSurge(Sim s, Civ c, Settlement cap)
    {
        bool lucky = s.E(c, "luck") > 0;
        var surges = new List<Surge>
        {
            new Surge { Good = true, Run = () => { double g = s.Rng.Int(20, 45); s.Add(c, "gold", g); return $"gökten {J.S(g)} altın yağdı"; } },
            new Surge { Good = true, Run = () => { if (J.T(c.Research.Current)) c.Research.Progress += D.TechCost(D.TECH[c.Research.Current]) * 0.5; return "bilginlerin zihninde bir kıvılcım çaktı, araştırma sıçradı"; } },
            new Surge { Good = true, Run = () => { if (s.Pop(cap) < 20) { s.Add(c, "grain", 30); return "ambarlar sıcak ekmekle doldu"; } cap.Soldiers += 3; return "alevlerin içinden üç savaşçı yürüyerek çıktı"; } },
            new Surge { Good = true, Run = () => { s.AddPop(cap, s.Rng.Pick(SURGE_RACES), 2); return "kızıl bir sisten iki yabancı belirdi ve kaldı"; } },
            new Surge { Good = false, Run = () => { double q = Math.Floor(s.St(c, "grain") * 0.25); s.Add(c, "grain", -q); return $"kontrolden çıkan alev ambarda {J.S(q)} tahılı kül etti"; } },
            new Surge { Good = false, Run = () => { if (s.Pop(cap) > 8) s.RemovePop(cap, 1); return "bir çırak kendi büyüsüne kurban gitti"; } },
            new Surge { Good = false, Run = () => { foreach (var h in s.CivHeroes(c)) h.Hp = JsMath.Max(1, Math.Floor(h.Hp / 2)); return "kahramanlar lanetli bir ateşle yaralandı"; } },
            new Surge { Good = true, Run = () =>
            {
                var cand = J.Filter(s.G.Within(cap.Tile, 3), t => s.W.Tiles[t].Terrain == "grass" && s.W.Tiles[t].Owner >= 0 && s.W.Tiles[t].Ext == null && t != cap.Tile);
                if (cand.Count == 0) { s.Add(c, "mana", 5); return "havada mana kıvılcımları uçuştu"; }
                int t = s.Rng.Pick(cand); s.W.Tiles[t].Terrain = "forest"; s.W.Tiles[t].Wood = 160; s.ClearPaths(); return "bir gecede kızıl yapraklı bir koru bitti";
            } },
        };
        var pick = s.Rng.Pick(surges);
        if (!pick.Good && lucky) pick = s.Rng.Pick(J.Filter(surges, x => x.Good));
        string what = pick.Run();
        s.Metric("wildSurge");
        s.Log("class", $"Yabani büyü dalgası {c.Name} topraklarını sardı: {what}.", civ: c.Id, tile: cap.Tile, major: true, cause: pick.Good ? "Kan Soyu: şans bu kez yüzlerine güldü" : "Kan Soyu: büyünün bedeli");
    }
}
