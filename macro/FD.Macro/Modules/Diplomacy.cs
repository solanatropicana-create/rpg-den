using System;
using System.Collections.Generic;
using System.Linq;

// İlişkiler, kaynak anlaşmazlıkları, antlaşmalar, savaş, ticaret, genişleme, göç. Port of src/sim/diplomacy.ts.

namespace FD.Macro;

public static class Diplomacy
{
    private static readonly JsObj<List<string>> GATE_GOOD = new JsObj<List<string>>
    {
        ["copper"] = new() { "copper" }, ["tin"] = new() { "tin" }, ["iron"] = new() { "iron" }, ["gold"] = new() { "gold" }, ["mana"] = new() { "mana" },
        ["mithril"] = new() { "mithril" }, ["horses"] = new() { "horses" }, ["herbs"] = new() { "herbs" }, ["clay"] = new() { "bricks" },
    };
    private static readonly JsObj<List<string>> DEP_FOR = new JsObj<List<string>>
    {
        ["copper"] = new() { "copper" }, ["tin"] = new() { "tin" }, ["iron"] = new() { "iron" }, ["gold"] = new() { "gold", "silver" }, ["mana"] = new() { "mana" },
        ["mithril"] = new() { "mithril" }, ["horses"] = new() { "horses" }, ["herbs"] = new() { "herbs" }, ["salt"] = new() { "salt" }, ["bricks"] = new() { "clay" },
    };
    /// <summary>TS <c>[]</c> fallback of <c>X[k] ?? []</c> (read only, never mutated).</summary>
    private static readonly List<string> NONE = new();

    /// <summary>TS <c>GATE_GOOD[k]</c> (undefined → null, also for an undefined key).</summary>
    private static List<string> GateGood(string k) => k != null && GATE_GOOD.TryGet(k, out var l) ? l : null;
    /// <summary>TS <c>DEP_FOR[g]</c> (undefined → null, also for an undefined key).</summary>
    private static List<string> DepFor(string g) => g != null && DEP_FOR.TryGet(g, out var l) ? l : null;

    /// <summary>Her 10 günde: temas, mod sönümü, değerler/sınır/ortak düşman/sınıf yakınlıkları, toprak açlığı, anlaşmazlıklar, savaş ve barış.</summary>
    public static void RelationsTick(Sim s)
    {
        var civs = J.Filter(s.W.Civs, c => c.Alive);
        foreach (var a in civs) foreach (var b in civs)
        {
            if (a.Id >= b.Id) continue;
            var r = s.Rel(a.Id, b.Id);
            if (!r.Contact)
            {
                bool close = J.Some(s.CivSettlements(a), x => J.Some(s.CivSettlements(b), y => s.G.Dist(x.Tile, y.Tile) <= 16));
                if (close) Agents.MakeContact(s, a, b, "Sınırları birbirine yaklaştı"); else continue;
            }
            foreach (var (x, y) in new[] { (a.Id, b.Id), (b.Id, a.Id) })
            {
                var rr = s.Rel(x, y);
                foreach (var m in rr.Mods) if (J.T(m.Decay)) m.Value = m.Value > 0 ? JsMath.Max(0, m.Value - m.Decay * 10) : JsMath.Min(0, m.Value + m.Decay * 10);
                rr.Mods = J.Filter(rr.Mods, m => m.Decay == 0 || Math.Abs(m.Value) >= 0.5);
            }
            double diff = Math.Abs(a.Align.Law - b.Align.Law) + Math.Abs(a.Align.Good - b.Align.Good);
            if (diff > 0.3) s.SetMod(a.Id, b.Id, "values", "Farklı değerler", -JsMath.Round(diff * 6)); else s.SetMod(a.Id, b.Id, "values", "Benzer değerler", 6);
            var dists = new List<double>();
            foreach (var x in s.CivSettlements(a)) foreach (var y in s.CivSettlements(b)) dists.Add(s.G.Dist(x.Tile, y.Tile));
            double minD = JsMath.Min(dists.ToArray());
            if (minD <= 9) s.SetMod(a.Id, b.Id, "border", "Sınır sürtüşmesi", -8); else s.RemoveMod(a.Id, b.Id, "border");
            if (s.Day - a.LastRaidedDay < 240 && s.Day - b.LastRaidedDay < 240) s.SetMod(a.Id, b.Id, "enemy", "Ortak düşman: canavarlar", 10); else s.RemoveMod(a.Id, b.Id, "enemy");
            // sınıf yakınlıkları
            var pair = new List<Civ> { a, b };
            var pal = J.Find(pair, c => c.Cls == "paladin" && c.Subclass == "ancients");
            var dru = J.Find(pair, c => c.Cls == "druid");
            if (pal != null && dru != null) s.SetMod(a.Id, b.Id, "ancients", "Kadim Yemin dostluğu", 18);
            var paladin = J.Find(pair, c => c.Cls == "paladin");
            var other = J.Find(pair, c => c != paladin);
            if (paladin != null && other != null && other.Align.Good < -0.3) s.SetMod(a.Id, b.Id, "evil", "Paladin kötülüğe tahammül etmez", -20);
            var priest = J.Find(pair, c => c.Cls == "cleric");
            var dark = J.Find(pair, c => c != priest);
            if (priest != null && dark != null && dark.Align.Good < -0.3) s.SetMod(a.Id, b.Id, "evil2", "Tapınak karanlıkla uzlaşmaz", -12);
            foreach (var (x, y) in new[] { (a, b), (b, a) })
            {
                double ch = s.E(x, "charm");
                if (ch > 0) s.SetMod(y.Id, x.Id, "charm", "Ozanların şarkıları", JsMath.Round(6 * ch));
                // toprak açlığı: kalabalık, saldırgan ve sınırdaş olan komşusunun toprağına göz diker
                var rr = s.Rel(x.Id, y.Id);
                double agg = D.CLASSES[x.Cls].Aggression;
                bool crowded = s.CivPop(x) >= 26 && (s.CivSettlements(x).Count >= 4 || s.Day - x.LastExpand > 400);
                if (minD <= 10 && agg >= 0.2 && crowded && !J.T(rr.Treaty)) rr.Land = JsMath.Min(30, (rr.Land ?? 0) + 0.25 + agg * 0.9);
                else rr.Land = JsMath.Max(0, (rr.Land ?? 0) - 0.4);
                if ((rr.Land ?? 0) >= 8) s.SetMod(x.Id, y.Id, "land", "Toprak hırsı", -JsMath.Min(25, JsMath.Round(rr.Land.Value * 1.2)), 0, false);
                else s.RemoveMod(x.Id, y.Id, "land", false);
            }
            Disputes(s, a, b);
            Disputes(s, b, a);
            WarPeace(s, a, b);
            WarPeace(s, b, a);
        }
    }

    /// <summary>o medeniyetinin, h'nin elindeki bir kaynağa duyduğu ihtiyaç</summary>
    private static void Disputes(Sim s, Civ o, Civ h)
    {
        var r = s.Rel(o.Id, h.Id);
        var want = new JsMap<string, double>();
        foreach (var g in D.CLASSES[o.Cls].Desires) want.Set(g, 1.2);
        foreach (var t in Research.BlockedByGate(s, o))
        {
            var ks = new List<string>();
            if (t.Gate != null) ks.AddRange(t.Gate);
            if (t.GateAny != null) ks.AddRange(t.GateAny);
            foreach (var k in ks) foreach (var g in GateGood(k) ?? NONE) if (!s.Access(o, k)) want.Set(g, JsMath.Max(want.TryGet(g, out var wv) ? wv : 0, 1));
        }
        string worstG = null; double worstV = 0;
        foreach (var kv in want)
        {
            string g = kv.Key; double w = kv.Value;
            var kinds = DepFor(g) ?? NONE;
            bool lacks = !J.Some(kinds, k => s.OwnsDeposit(o, k));
            bool held = lacks && J.Some(s.W.Deposits, d => kinds.Contains(d.Kind) && !d.Depleted && d.KnownBy.Contains(o.Id) && J.Some(d.Tiles, t => s.TileCiv(t) == h.Id));
            double cur = r.Tension.Get(g) ?? 0;
            if (!held || r.Treaty == g) { if (cur > 0) r.Tension.Set(g, JsMath.Max(0, cur - 0.6)); continue; }
            double nv = cur + w * (o.Race == "dwarf" ? 1.3 : 1);
            r.Tension.Set(g, nv);
            if (cur < 6 && nv >= 6)
            {
                s.Metric("tension");
                string gName = J.TrLower(D.GOODS[g].Name);
                s.Log("tension", $"{o.Name}, {Tr.Ek(h.Name, "in")} elindeki {gName} kaynağına göz dikti.", civ: o.Id,
                    cause: $"{(D.CLASSES[o.Cls].Desires.Contains(g) ? $"{D.CLASSES[o.Cls].Name} medeniyeti {gName} ister" : "Araştırması bu kaynağa takıldı")}; kendi yatağı yok", major: true);
            }
            if (worstG == null || nv > worstV) { worstG = g; worstV = nv; }
        }
        if (worstG != null) s.SetMod(o.Id, h.Id, "dispute", $"{D.GOODS[worstG].Name} anlaşmazlığı", -JsMath.Min(40, JsMath.Round(worstV * 1.6)), 0, false);
        else s.RemoveMod(o.Id, h.Id, "dispute", false);
        // müzakere
        if (worstG == null || r.War != null) return;
        string gg = worstG; double tension = worstV;
        double talkAt = o.Align.Law > 0.3 ? 10 : 16;
        if (tension < talkAt || s.Day - r.LastTalk < 120 || s.RelValue(o.Id, h.Id) < -65) return;
        r.LastTalk = s.Day;
        double p = 0.35 + h.Align.Good * 0.3 + h.Align.Law * 0.1 + s.RelValue(h.Id, o.Id) / 150 + (h.Cls == "rogue" ? 0.15 : 0);
        if (s.Rng.Chance(p))
        {
            s.Rel(o.Id, h.Id).Treaty = gg; s.Rel(h.Id, o.Id).Treaty = gg;
            r.Tension.Set(gg, 0);
            s.RemoveMod(o.Id, h.Id, "dispute", false);
            s.SetMod(o.Id, h.Id, "treaty", $"{D.GOODS[gg].Name} antlaşması", 15);
            var kinds = DepFor(gg) ?? NONE;
            int? srcTile = null;
            var dep = J.Find(s.W.Deposits, d => kinds.Contains(d.Kind) && J.Some(d.Tiles, t => s.TileCiv(t) == h.Id));
            if (dep != null) { int k = J.FindIndex(dep.Tiles, t => s.TileCiv(t) == h.Id); if (k >= 0) srcTile = dep.Tiles[k]; }
            var src = srcTile != null ? s.Settlement(s.W.Tiles[srcTile.Value].Owner) : s.Capital(h);
            var dst = s.Capital(o);
            var path = src != null && dst != null ? s.Path(src.Tile, dst.Tile) : null;
            if (src != null && dst != null && path != null) s.W.Routes.Add(new TradeRoute { Id = s.Id(), A = src.Id, B = dst.Id, Kind = "treaty", Good = gg, Path = path, NextDepart = s.Day + 3, Trips = 0, Alive = true, Since = s.Day });
            s.Metric("treaty");
            s.Log("diplomacy", $"{o.Name} ile {h.Name} {D.GOODS[gg].Name} Antlaşması imzaladı: {J.TrLower(D.GOODS[gg].Name)} altın karşılığında düzenli taşınacak.", civ: o.Id, cause: $"Kavga yerine pazarlık ({h.Name} teklife açıktı)", major: true);
        }
        else
        {
            s.AddMod(o.Id, h.Id, "refused", "Reddedilen teklif", -12, -24, 0.03, false);
            s.Log("diplomacy", $"{h.Name}, {Tr.Ek(o.Name, "in")} {J.TrLower(D.GOODS[gg].Name)} paylaşımı teklifini reddetti.", civ: o.Id, cause: "Kaynağı paylaşmak istemiyorlar");
        }
    }

    /// <summary>Medeniyetin askerî gücü: askerler + kahramanlar + milis (powerOf).</summary>
    public static double MilitaryPower(Sim s, Civ c)
    {
        double sol = J.Sum(s.CivSettlements(c), x => x.Soldiers);
        var cs = new List<Combatant>(Agents.CivTroops(s, c, sol, "A"));
        cs.AddRange(J.Map(s.CivHeroes(c), h => Combat.HeroCombatant(h, "A")));
        double militia = Math.Floor(s.CivPop(c) * 0.2);
        for (int i = 0; i < militia; i++) cs.Add(Combat.Unit(new UnitStats { Name = "Milis", Hp = 6, Ac = 10, Atk = 1, Dmg = new List<double> { 1, 4, 0 } }, "A", "militia"));
        return Combat.PowerOf(cs);
    }

    private static double WarThreshold(Sim s, Civ c)
    {
        double t = -32 - c.Align.Good * 22 - c.Align.Law * 10 + D.CLASSES[c.Cls].Aggression * 28;
        if (c.Subclass == "vengeance") t += 15;
        return t;
    }

    private static void WarPeace(Sim s, Civ o, Civ t)
    {
        var r = s.Rel(o.Id, t.Id);
        double rv = s.RelValue(o.Id, t.Id);
        if (r.War == null)
        {
            if (s.Rel(t.Id, o.Id).War != null || s.InWar(o)) return;
            if (s.Day - (r.PeaceDay ?? -9999) < 2 * Sim.YEAR || s.Day - (o.LastWarEnd ?? -9999) < Sim.YEAR * 1.5) return;
            var tvals = new List<double> { 0 };
            tvals.AddRange(r.Tension.Values());
            double tension = JsMath.Max(tvals.ToArray());
            double land = r.Land ?? 0;
            bool crusade = s.E(o, "crusade") > 0 && t.Align.Good < -0.3 && o.Align.Good > 0.3;
            if (!crusade && (rv > WarThreshold(s, o) || (tension < 10 && land < 12))) return;
            if (crusade && rv > 0) return;
            if (o.Cls == "paladin" && (J.T(r.Treaty) || J.T(s.Rel(t.Id, o.Id).Treaty))) return; // yemin: antlaşma bozulmaz
            if (MilitaryPower(s, o) < MilitaryPower(s, t) * 0.75) return;
            if (s.CivPop(o) < 22 || J.Sum(s.CivSettlements(o), x => x.Soldiers) < 4) return;
            // hedef: çekişilen kaynağa ya da (toprak savaşında) sınırdaki en yakın yerleşim
            bool byLand = tension < 10 && !crusade;
            string goodKey = J.At(J.Sort(r.Tension.Keys(), (x, y) => (r.Tension.Get(y) ?? 0) - (r.Tension.Get(x) ?? 0)), 0);
            var kinds = byLand ? NONE : DepFor(goodKey) ?? NONE;
            int? depTile = null;
            var dep = J.Find(s.W.Deposits, d => kinds.Contains(d.Kind) && J.Some(d.Tiles, x => s.TileCiv(x) == t.Id));
            if (dep != null) { int k = J.FindIndex(dep.Tiles, x => s.TileCiv(x) == t.Id); if (k >= 0) depTile = dep.Tiles[k]; }
            var oCap = s.Capital(o);
            var border = oCap != null ? J.At(J.Sort(J.Slice(s.CivSettlements(t)), (x, y) => s.G.Dist(x.Tile, oCap.Tile) - s.G.Dist(y.Tile, oCap.Tile)), 0) : null;
            var target = crusade ? s.Capital(t) : depTile != null ? s.Settlement(s.W.Tiles[depTile.Value].Owner) : byLand ? border : s.Capital(t);
            if (target == null) return;
            string goodName = (goodKey != null ? D.GOODS[goodKey]?.Name : null) ?? "Toprak";
            var war = new War
            {
                Since = s.Day, Attacker = o.Id, Target = target.Id, Attacks = 0, LastArmy = -999,
                Goal = crusade ? $"Kutsal Sefer: {Tr.Ek(t.Name, "in")} karanlık paktını yıkmak" : byLand ? $"{Tr.Ek(target.Name, "i")} ve çevresindeki toprakları almak" : $"{goodName} kaynağını ele geçirmek",
            };
            r.War = war; s.Rel(t.Id, o.Id).War = war;
            s.Metric("war");
            s.Log("war", $"{o.Name}, {Tr.Ek(t.Name, "a")} SAVAŞ İLAN ETTİ! Hedef: {target.Name}.", civ: o.Id, tile: target.Tile,
                cause: $"İlişki {J.S(rv)}: {string.Join(", ", J.Map(J.Filter(r.Mods, m => m.Value < 0), m => J.TrLower(m.Text)))}", major: true);
            CallAllies(s, o, t, war, target);
            // Paladin kutsal seferi: iyi medeniyetler kötüye karşı yakınlaşır
            return;
        }
        if (r.War.Attacker != o.Id) return;
        var tgt = s.Settlement(r.War.Target);
        bool won = tgt == null || !tgt.Alive || tgt.Civ != t.Id;   // hedef artık düşmanın değil (biz ya da müttefik aldı)
        bool isLong = s.Day - r.War.Since > 300, tired = r.War.Attacks >= 3;
        if (won || isLong || tired)
        {
            string why = won ? "Savaş hedefi ele geçirildi" : tired ? "Ordular yıprandı" : "Savaş uzadı, halk yoruldu";
            r.War = null; s.Rel(t.Id, o.Id).War = null;
            r.PeaceDay = s.Day; s.Rel(t.Id, o.Id).PeaceDay = s.Day; o.LastWarEnd = s.Day;
            foreach (var k in r.Tension.Keys()) r.Tension.Set(k, (r.Tension.Get(k) ?? 0) * (won ? 0 : 0.3));
            r.Land = (r.Land ?? 0) * (won ? 0 : 0.2);
            s.AddMod(o.Id, t.Id, "pastwar", "Geçmiş savaşın izleri", -30, -40, 0.02);
            s.SetMod(o.Id, t.Id, "peace", "Barış antlaşması", 20, 0.03);
            s.Log("war", $"{o.Name} ile {t.Name} barış yaptı.", civ: o.Id, cause: why, major: true);
        }
    }

    /// <summary>Savaşta saldıran taraf hedef şehre ordu yollar (karadan ya da donanmayla denizden).</summary>
    public static void ConsiderWarAction(Sim s, Civ c)
    {
        for (int oi = 0; oi < s.W.Civs.Count; oi++)
        {
            var o = s.W.Civs[oi];
            if (o.Id == c.Id) continue;
            var war = s.Rel(c.Id, o.Id).War;
            if (war == null || war.Attacker != c.Id) continue;
            if (J.Some(s.W.Agents, a => a.Kind == "army" && a.Civ == c.Id && a.Purpose == "war")) continue;
            if (s.Day - war.LastArmy < 50) continue;
            var tgt = s.Settlement(war.Target);
            if (tgt == null || !tgt.Alive || tgt.Civ != o.Id) continue;
            double sol = Math.Floor(J.Sum(s.CivSettlements(c), x => x.Soldiers) * 0.75);
            var heroes = J.Filter(s.CivHeroes(c), h => h.State == "home" && h.Hp > h.MaxHp * 0.6);
            if (sol + heroes.Count * 3 < 4) continue;
            var cap = s.Capital(c);
            var route = ArmyRoute(s, c, cap.Tile, tgt.Tile);
            if (route == null) continue;
            var pop = Agents.DrawSoldiers(s, c, sol);
            foreach (var h in heroes) h.State = "army";
            war.LastArmy = s.Day; war.Attacks++;
            double gal = route.Hull != null ? JsMath.Min(3, Sea.FreeGalleys(s, route.Hull)) : 0;
            s.W.Agents.Add(new Agent
            {
                Id = s.Id(), Kind = "army", Civ = c.Id, Path = route.Path, Step = 0, Progress = 0, Speed = 0.55, Heroes = J.Map(heroes, h => h.Id), Troops = sol, Pop = pop,
                From = cap.Id, To = tgt.Id, Purpose = "war", Hull = route.Hull?.Id, Galleys = J.T(gal) ? gal : (double?)null,
            });
            if (route.Hull != null)
            {
                s.Metric("seaInvasion");
                s.Log("sea", $"{c.Name} donanması{(J.T(gal) ? $" ({J.S(gal)} kadırga)" : "")} {J.S(sol)} askerle {Tr.Ek(route.Hull.Name, "dan")} denize açıldı. Hedef: {tgt.Name}.", civ: c.Id, tile: route.Hull.Port, cause: war.Goal, major: true);
            }
            else s.Log("war", $"{c.Name} ordusu ({J.S(sol)} asker{(heroes.Count > 0 ? ", " + string.Join(", ", J.Map(heroes, h => h.Name)) : "")}) {tgt.Name} üzerine yürüyor.", civ: c.Id, tile: cap.Tile, cause: war.Goal);
        }
    }

    /// <summary>Ortak düşman: saldırganla arası iyi, hedefe kin duyan bir komşu savaşa katılır ve aynı şehri hedefler</summary>
    private static void CallAllies(Sim s, Civ o, Civ t, War war, Settlement target)
    {
        var cands = J.Filter(s.W.Civs, y => y.Alive && y.Id != o.Id && y.Id != t.Id && !s.InWar(y)
            && s.Rel(y.Id, t.Id).Contact && s.Rel(y.Id, o.Id).Contact && s.Day - (y.LastWarEnd ?? -9999) >= Sim.YEAR);
        var scored = new List<(Civ Y, double Hate, double Love)>();
        foreach (var y in cands)
        {
            double hate = s.RelValue(y.Id, t.Id), love = s.RelValue(y.Id, o.Id);
            if (hate > WarThreshold(s, y) + 18 || hate > -8 || love < 12) continue;
            if (y.Cls == "paladin" && (J.T(s.Rel(y.Id, t.Id).Treaty) || J.T(s.Rel(t.Id, y.Id).Treaty))) continue;
            if (J.Sum(s.CivSettlements(y), x => x.Soldiers) < 4) continue;
            var cap = s.Capital(y);
            if (cap == null || s.G.Dist(cap.Tile, target.Tile) > 40 || ArmyRoute(s, y, cap.Tile, target.Tile) == null) continue;
            scored.Add((y, hate, love));
        }
        J.Sort(scored, (a, b) => (b.Love - b.Hate) - (a.Love - a.Hate));
        if (scored.Count == 0) return;
        var pick = scored[0];
        var w2 = new War { Since = s.Day, Attacker = pick.Y.Id, Target = target.Id, Attacks = 0, LastArmy = -999, Goal = $"{Tr.Ek(o.Name, "in")} yanında: {war.Goal}", Ally = o.Id };
        s.Rel(pick.Y.Id, t.Id).War = w2; s.Rel(t.Id, pick.Y.Id).War = w2;
        s.SetMod(pick.Y.Id, o.Id, "brothers", "Silah arkadaşlığı", 14, 0.01);
        s.Metric("coalition");
        s.Log("war", $"{pick.Y.Name}, {Tr.Ek(o.Name, "in")} yanında {Tr.Ek(t.Name, "a")} savaş ilan etti! Ortak hedef: {target.Name}.", civ: pick.Y.Id, tile: target.Tile,
            cause: $"{t.Name} ile ilişki {J.S(pick.Hate)}, {o.Name} ile {J.S(pick.Love)}", major: true);
    }

    /// <summary>ordu yolu: karadan; Donanma varsa (ve daha kısaysa) denizden</summary>
    private static CivPathResult ArmyRoute(Sim s, Civ c, int from, int to)
    {
        var land = s.Path(from, to);
        if (!s.Has(c, "navy")) return land != null ? new CivPathResult { Path = land } : null;
        var r = Sea.CivPath(s, c, from, to);
        if (r != null && r.Hull != null && Sea.HasSea(s, r.Path) && (land == null || r.Path.Count < land.Count * 0.8)) return r;
        return land != null ? new CivPathResult { Path = land } : null;
    }

    /// <summary>Saldırgan sınıfların savaş ilan etmeden yaptığı yağma akınları</summary>
    public static void ConsiderRaid(Sim s, Civ c)
    {
        if (D.CLASSES[c.Cls].Aggression < 0.6 || s.InWar(c)) return;
        double sol = J.Sum(s.CivSettlements(c), x => x.Soldiers);
        if (sol < 5 || J.Some(s.W.Agents, a => a.Civ == c.Id && a.Purpose == "plunder")) return;
        var cap = s.Capital(c);
        int reach = s.Has(c, "navy") ? 34 : 20;
        var targets = J.Filter(s.W.Settlements, x => x.Alive && x.Civ != c.Id && s.Rel(c.Id, x.Civ).Contact && s.RelValue(c.Id, x.Civ) < 0
            && s.Day - s.Rel(c.Id, x.Civ).LastRaid > 180 && s.G.Dist(x.Tile, cap.Tile) <= reach && !J.T(s.Rel(c.Id, x.Civ).Treaty));
        if (targets.Count == 0) return;
        var st = J.Sort(targets, (a, b) => (a.Soldiers * 3 + s.Pop(a) * 0.4) - (b.Soldiers * 3 + s.Pop(b) * 0.4))[0];
        double n = Math.Floor(sol * 0.5);
        if (n * 3 < st.Soldiers * 3 + s.Pop(st) * 0.3) return;
        var route = ArmyRoute(s, c, cap.Tile, st.Tile);
        if (route == null) return;
        s.Rel(c.Id, st.Civ).LastRaid = s.Day;
        var pop = Agents.DrawSoldiers(s, c, n);
        double gal = route.Hull != null ? JsMath.Min(2, Sea.FreeGalleys(s, route.Hull)) : 0;
        s.W.Agents.Add(new Agent
        {
            Id = s.Id(), Kind = "army", Civ = c.Id, Path = route.Path, Step = 0, Progress = 0, Speed = 0.8, Troops = n, Pop = pop, From = cap.Id, To = st.Id, Purpose = "plunder",
            Hull = route.Hull?.Id, Galleys = J.T(gal) ? gal : (double?)null,
        });
        if (route.Hull != null)
        {
            s.Metric("seaRaid");
            s.Log("sea", $"{c.Name} deniz akıncıları {Tr.Ek(route.Hull.Name, "dan")} {Tr.Ek(st.Name, "a")} doğru yelken açtı.", civ: c.Id, tile: route.Hull.Port, cause: $"{D.CLASSES[c.Cls].Feature}: ganimet ve şan", major: true);
        }
        else s.Log("war", $"{c.Name} akıncıları {Tr.Ek(st.Name, "a")} doğru yola çıktı.", civ: c.Id, tile: cap.Tile, cause: $"{D.CLASSES[c.Cls].Feature}: ganimet ve şan", major: true);
    }

    /// <summary>Öncü (settlers) gönderme: anakarada 5 yerleşime dek, Gemicilik/Seyir ile denizaşırı koloni.</summary>
    public static void ConsiderExpansion(Sim s, Civ c)
    {
        var ss = s.CivSettlements(c);
        var cap = s.Capital(c);
        if (cap == null || !s.Has(c, "roads")) return;
        // anakarada 5 yerleşim; Gemicilik ve Seyir birer denizaşırı koloni hakkı daha açar
        int over = J.Filter(ss, x => J.T(x.Overseas)).Count;
        bool landOk = ss.Count - over < 5;
        bool seaOk = s.Has(c, "shipbuilding") && over < (s.Has(c, "navigation") ? 2 : 1) && J.Some(Sea.Ports(s, c), x => Sea.FreeHulls(s, x) > 0);
        if (!landOk && !seaOk) return;
        if (s.Pop(cap) < 12 + ss.Count * 5 || s.FoodTotal(c) < 30 || s.Day - c.LastExpand < 160) return;
        if (J.Some(s.W.Agents, a => a.Kind == "settlers" && a.Civ == c.Id)) return;
        var target = PickSettleTarget(s, c, landOk, seaOk, cap);
        if (target == null) return;
        var pop = s.RemovePop(cap, 5);
        s.Add(c, "grain", -15); s.Add(c, "wood", -10);
        c.LastExpand = s.Day;
        s.W.Agents.Add(new Agent { Id = s.Id(), Kind = "settlers", Civ = c.Id, Path = target.Path, Step = 0, Progress = 0, Speed = 0.5, Pop = pop, From = cap.Id, TargetTile = target.Tile, Purpose = target.Why, Hull = target.Hull?.Id });
        if (target.Hull != null)
        {
            s.Metric("seaVoyage");
            s.Log("sea", $"{c.Name} 5 öncüyü {Tr.Ek(target.Hull.Name, "dan")} bir {Sea.HullName(s, c)} ile denizaşırı topraklara gönderdi.", civ: c.Id, tile: target.Hull.Port, cause: target.Why, major: true);
        }
        else s.Log("settle", $"{c.Name} 5 öncüyü yeni bir yerleşim kurmaya gönderdi.", civ: c.Id, tile: cap.Tile, cause: target.Why);
    }

    /// <summary>TS inline type <c>{ tile; why; path; hull? }</c> of pickSettleTarget.</summary>
    private sealed class SettleTarget
    {
        public int Tile;
        public string Why;
        public List<int> Path;
        public Settlement Hull;
    }

    /// <summary>TS inline type <c>{ i; sc; why; sea }</c> (pickSettleTarget candidates).</summary>
    private sealed class SettleCand
    {
        public int I;
        public double Sc;
        public string Why;
        public bool Sea;
    }

    private static SettleTarget PickSettleTarget(Sim s, Civ c, bool landOk, bool seaOk, Settlement cap)
    {
        var w = s.W;
        var own = s.CivSettlements(c);
        var all = J.Filter(w.Settlements, x => x.Alive);
        var like = D.CLASSES[c.Cls].TerrainLike;
        var desires = D.CLASSES[c.Cls].Desires;
        var needGoods = new HashSet<string>();
        foreach (var tech in Research.BlockedByGate(s, c)) foreach (var k in tech.Gate ?? NONE) needGoods.Add(k);
        int seaMax = s.Has(c, "navigation") ? 45 : 26;
        var cands = new List<SettleCand>();
        int best = -1; double bs = double.NegativeInfinity; string bwhy = "";
        for (int i = 0; i < w.Tiles.Count; i++)
        {
            var t = w.Tiles[i];
            if (t.Terrain == "water" || t.Terrain == "mountain" || J.T(t.Sea) || t.Owner >= 0 || t.Camp != null || t.Deposit >= 0) continue;
            if (J.T(t.Isle) && !seaOk) continue;
            if (J.Some(w.Inns, inn => s.G.Dist(inn.Tile, i) < 4)) continue;
            double dOwn = J.MinOf(own, x => s.G.Dist(x.Tile, i));
            if (dOwn < 5) continue;
            // denizaşırı aday yalnız adalar: anakaranın ıssız kıyıları canavarlara ve yeni kabilelere kalır
            bool coastal = J.Some(s.G.Neighbors(i), n => J.T(w.Tiles[n].Sea));
            bool sea = J.T(t.Isle);
            if (sea ? !seaOk || dOwn > seaMax : !landOk || dOwn > 16) continue;
            if (J.Some(all, x => s.G.Dist(x.Tile, i) < 5)) continue;
            if (J.Some(w.Camps, cp => cp.Alive && s.G.Dist(cp.Tile, i) < 5)) continue;
            double sc = -dOwn * (sea ? 0.3 : 0.7) + s.Rng.Next() * 2 + (sea ? 3 : 0);
            string why = sea ? "Bakir bir ada" : "Verimli topraklar"; double whyV = 0;
            // liman kurulabilecek kıyı yeri, denizci medeniyetler için değerli
            if (coastal && s.Has(c, "fishing")) sc += 1.5 * (D.CLASSES[c.Cls].Prefer.Get("deniz") ?? 1);
            var seenDep = new HashSet<int>();
            foreach (int n in s.G.Within(i, 2))
            {
                var tt = w.Tiles[n];
                sc += (like.Get(tt.Terrain) ?? 0.4) * 0.6;
                if (tt.Deposit < 0 || seenDep.Contains(tt.Deposit) || tt.Owner >= 0) continue;
                var d = J.Find(w.Deposits, x => x.Id == tt.Deposit);
                seenDep.Add(d.Id);
                if (!s.DepositVisible(c, d) || d.Depleted || (d.Kind == "heartwood" && c.Cls != "druid")) continue;
                string g = D.DEPOSITS[d.Kind].Good;
                double v = D.GOODS[g].Base * 1.5;
                if (desires.Contains(g)) v += 18;
                if (needGoods.Contains(d.Kind) || (d.Kind == "silver" && needGoods.Contains("gold"))) v += 22;
                if (!s.OwnsDeposit(c, d.Kind)) v *= 1.5;
                sc += v;
                if (v > whyV) { whyV = v; why = $"{(sea ? "Ada: " : "")}{D.DEPOSITS[d.Kind].Name} için"; }
            }
            if (sea) cands.Add(new SettleCand { I = i, Sc = sc, Why = why, Sea = sea });
            else if (sc > bs) { bs = sc; best = i; bwhy = why; }
        }
        if (best >= 0) cands.Add(new SettleCand { I = best, Sc = bs, Why = bwhy, Sea = false });
        J.Sort(cands, (a, b) => b.Sc - a.Sc);
        foreach (var cd in J.Slice(cands, 0, 6))
        {
            if (!cd.Sea) { var p = s.Path(cap.Tile, cd.I); if (p != null) return new SettleTarget { Tile = cd.I, Why = cd.Why, Path = p }; continue; }
            var r = Sea.CivPath(s, c, cap.Tile, cd.I);
            if (r != null && r.Hull != null && Sea.HasSea(s, r.Path)) return new SettleTarget { Tile = cd.I, Why = cd.Why, Path = r.Path, Hull = r.Hull };
        }
        return null;
    }

    /// <summary>Pazarı olan medeniyet komşularıyla kara ticaret yolu (ve ardından deniz ticareti / ikmal) açar.</summary>
    public static void ConsiderTrade(Sim s, Civ c)
    {
        if (!s.Has(c, "barter")) return;
        var src = J.Find(s.CivSettlements(c), x => J.T(x.Civics.Get("market")));
        if (src == null) return;
        int maxD = s.Has(c, "caravans") ? 40 : 22;
        for (int oi = 0; oi < s.W.Civs.Count; oi++)
        {
            var o = s.W.Civs[oi];
            if (o.Id == c.Id || !o.Alive) continue;
            var r = s.Rel(c.Id, o.Id);
            if (!r.Contact || (r.War != null && !J.T(s.E(c, "blackMarket"))) || s.RelValue(c.Id, o.Id) < -5) continue;
            if (J.Some(s.W.Routes, rt => rt.Alive && rt.Kind == "trade" && (s.Settlement(rt.A)?.Civ == c.Id || s.Settlement(rt.B)?.Civ == c.Id) && (s.Settlement(rt.A)?.Civ == o.Id || s.Settlement(rt.B)?.Civ == o.Id))) continue;
            var theirs = J.At(J.Sort(s.CivSettlements(o), (a, b) => s.G.Dist(a.Tile, src.Tile) - s.G.Dist(b.Tile, src.Tile)), 0);
            if (theirs == null || s.G.Dist(theirs.Tile, src.Tile) > maxD) continue;
            var path = s.Path(src.Tile, theirs.Tile);
            if (path == null) continue;
            s.W.Routes.Add(new TradeRoute { Id = s.Id(), A = src.Id, B = theirs.Id, Kind = "trade", Path = path, NextDepart = s.Day + 5, Trips = 0, Alive = true, Since = s.Day });
            s.Metric("tradeRoute");
            s.Log("trade", $"{src.Name} ile {theirs.Name} arasında ticaret yolu açıldı.", civ: c.Id, tile: src.Tile, cause: $"Pazar kuruldu; {o.Name} ile ilişki {J.S(s.RelValue(c.Id, o.Id))}", major: true);
        }
        ConsiderSeaTrade(s, c);
    }

    /// <summary>deniz ticaret yolu: kara yoluyla ulaşılamayan (ya da çok uzak) limanlar arasında</summary>
    private static void ConsiderSeaTrade(Sim s, Civ c)
    {
        if (!s.Has(c, "boatbuilding")) return;
        var mine = J.Filter(Sea.Ports(s, c), x => (x.Ships ?? 0) > 0);
        if (mine.Count == 0) return;
        int maxD = s.Has(c, "seatrade") ? 80 : s.Has(c, "navigation") ? 60 : s.Has(c, "shipbuilding") ? 42 : 24;
        var alive = J.Filter(s.W.Routes, rt => rt.Alive && rt.Kind == "trade");
        for (int oi = 0; oi < s.W.Civs.Count; oi++)
        {
            var o = s.W.Civs[oi];
            if (o.Id == c.Id || !o.Alive) continue;
            var r = s.Rel(c.Id, o.Id);
            if (!r.Contact || r.War != null || s.RelValue(c.Id, o.Id) < -5 || s.Day - (r.SeaTry ?? -9999) < 90) continue;
            var between = J.Filter(alive, rt => (s.Settlement(rt.A)?.Civ == c.Id || s.Settlement(rt.B)?.Civ == c.Id) && (s.Settlement(rt.A)?.Civ == o.Id || s.Settlement(rt.B)?.Civ == o.Id));
            if (J.Some(between, rt => J.T(rt.Sea)) || between.Count >= 1 && !s.Has(c, "shipbuilding")) continue;
            r.SeaTry = s.Day; s.Rel(o.Id, c.Id).SeaTry = s.Day;
            var theirs = Sea.Ports(s, o);
            Settlement pa = null, pb = null;
            double bd = double.PositiveInfinity;
            foreach (var a in mine) foreach (var b in theirs) { double d = s.G.Dist(a.Port.Value, b.Port.Value); if (d < bd) { bd = d; pa = a; pb = b; } }
            if (pa == null || pb == null || bd > maxD || bd < 6) continue;
            // kara yolu kısaysa deniz yoluna gerek yok
            var land = s.Path(pa.Tile, pb.Tile);
            if (land != null && land.Count <= bd * 1.25 && between.Count != 0) continue;
            var path = Sea.NavPath(s, pa.Tile, pb.Tile, new NavOpts { Embark = new List<int> { pa.Port.Value }, Open = s.Has(c, "navigation"), LandOnly = new List<int> { pb.Port.Value } });
            if (path == null || !Sea.HasSea(s, path)) continue;
            s.W.Routes.Add(new TradeRoute { Id = s.Id(), A = pa.Id, B = pb.Id, Kind = "trade", Path = path, NextDepart = s.Day + 5, Trips = 0, Alive = true, Since = s.Day, Sea = true });
            s.Metric("seaRoute");
            s.Log("sea", $"{pa.Name} ile {pb.Name} arasında deniz ticaret yolu açıldı.", civ: c.Id, tile: pa.Port, cause: $"{J.S(bd)} karo deniz; {o.Name} ile ilişki {J.S(s.RelValue(c.Id, o.Id))}", major: true);
        }
        // denizaşırı kolonilere ikmal gemileri
        if (!s.Has(c, "shipbuilding")) return;
        foreach (var col in s.CivSettlements(c))
        {
            if (!J.T(col.Overseas) || J.Some(alive, rt => J.T(rt.Sea) && (rt.A == col.Id || rt.B == col.Id))) continue;
            if (c.Yearly.Get("supply" + col.Id) == s.Year) continue;
            c.Yearly.Set("supply" + col.Id, s.Year);
            var home = J.At(J.Sort(J.Filter(mine, x => x.Id != col.Id && !J.T(x.Overseas)), (a, b) => s.G.Dist(a.Port.Value, col.Tile) - s.G.Dist(b.Port.Value, col.Tile)), 0);
            if (home == null) continue;
            var land = J.T(col.Civics.Get("shipyard")) && col.Port != null ? new List<int> { col.Port.Value } : null;
            var path = Sea.NavPath(s, home.Tile, col.Tile, new NavOpts { Embark = new List<int> { home.Port.Value }, Open = s.Has(c, "navigation"), LandOnly = land });
            if (path == null || !Sea.HasSea(s, path)) continue;
            s.W.Routes.Add(new TradeRoute { Id = s.Id(), A = home.Id, B = col.Id, Kind = "trade", Path = path, NextDepart = s.Day + 5, Trips = 0, Alive = true, Since = s.Day, Sea = true });
            s.Metric("seaRoute");
            s.Log("sea", $"{home.Name} ile denizaşırı {col.Name} arasında ikmal gemileri işlemeye başladı.", civ: c.Id, tile: home.Port, major: false);
        }
    }

    /// <summary>Bir kez: ufku keşfetmek için kâşif gönderir.</summary>
    public static void ConsiderScout(Sim s, Civ c)
    {
        if (c.ScoutSent || s.Day < 60 + c.Id * 20) return;
        var cap = s.Capital(c);
        if (cap == null) return;
        c.ScoutSent = true;
        int W = s.W.Width, H = s.W.Height;
        int cx = s.G.Col(cap.Tile), cy = s.G.Row(cap.Tile);
        int tx = cx < W / 2.0 ? Math.Min(W - 3, cx + 26) : Math.Max(2, cx - 26);
        int ty = cy < H / 2.0 ? Math.Min(H - 3, cy + 14) : Math.Max(2, cy - 14);
        // hedef noktaya en yakın anakara karosu
        int want = s.G.Idx(tx, ty);
        int goal = -1; double gd = double.PositiveInfinity;
        foreach (int i in s.G.Within(want, 12)) { var t = s.W.Tiles[i]; if (J.T(t.Sea) || J.T(t.Isle) || t.Terrain == "water" || t.Terrain == "mountain") continue; double d = s.G.Dist(i, want); if (d < gd) { gd = d; goal = i; } }
        if (goal < 0) return;
        var p = s.Path(cap.Tile, goal);
        if (p == null) return;
        s.W.Agents.Add(new Agent { Id = s.Id(), Kind = "scout", Civ = c.Id, Path = p, Step = 0, Progress = 0, Speed = 1.1, Purpose = "scout" });
        s.Log("discover", $"{c.Name} ufku keşfetmek için kâşifler gönderdi.", civ: c.Id, tile: cap.Tile);
    }

    private static readonly string[] REFUGEE_RACES = { "elf", "halfling", "gnome", "dragonborn", "tiefling", "human" };
    private static readonly string[] REFUGEE_WHY = { "Uzak diyarlardaki bir savaştan kaçtılar", "Ormanları yanmıştı", "Kıtlıktan kaçtılar", "Bir ejderhanın gölgesinden kaçtılar" };

    /// <summary>Her 30 günde: orman yenilenmesi, göç, mülteciler, melez doğumlar, yeni kurucular.</summary>
    public static void WorldTick(Sim s)
    {
        var w = s.W;
        Economy.RegrowForests(s);
        var civs = J.Filter(w.Civs, c => c.Alive);
        // Göç
        foreach (var a in civs) foreach (var b in civs)
        {
            if (a.Id == b.Id) continue;
            var r = s.Rel(a.Id, b.Id);
            double charm = s.E(a, "charm");
            if (!r.Contact || r.War != null || s.RelValue(a.Id, b.Id) < 15 - charm * 10) continue;
            double pa = Prosperity(s, a) * (1 + charm * 0.25), pb = Prosperity(s, b);
            var from = s.Capital(b); var to = s.Capital(a);
            if (from == null || to == null || s.Pop(from) < 14 || pa < pb * 1.5 + 0.3 || s.Pop(to) >= s.Housing(to) + 2) continue;
            if (!s.Rng.Chance(0.3)) continue;
            var moved = s.RemovePop(from, s.Rng.Int(1, 3));
            s.MergePop(to, moved);
            s.Metric("migration");
            s.Log("migration", $"{s.RaceStr(moved)} göçmen {Tr.Ek(from.Name, "dan")} {Tr.Ek(to.Name, "a")} yerleşti.", civ: a.Id, tile: to.Tile, cause: $"{a.Name} daha müreffeh, ilişkiler iyi");
        }
        // Mülteciler
        if (s.Year >= 3 && s.Rng.Chance(0.05))
        {
            var cands = J.Filter(w.Settlements, x => x.Alive && s.Pop(x) + 3 <= s.Housing(x) + 2);
            if (cands.Count > 0)
            {
                var st = s.Rng.Pick(cands);
                string race = s.Rng.Pick(REFUGEE_RACES);
                double n = s.Rng.Int(2, 4);
                s.AddPop(st, race, n);
                s.Metric("refugees");
                string text = $"{J.S(n)} {D.RACES[race].Name} mülteci {Tr.Ek(st.Name, "a")} sığındı.";
                s.Log("migration", text, civ: st.Civ, tile: st.Tile, cause: s.Rng.Pick(REFUGEE_WHY), major: true);
            }
        }
        // Melez doğumlar
        foreach (var st in w.Settlements)
        {
            if (!st.Alive) continue;
            const string k = "human-elf";
            if ((st.Pop.Get("human") ?? 0) >= 3 && (st.Pop.Get("elf") ?? 0) >= 2)
            {
                if (st.MixedSince.Get(k) == null) st.MixedSince.Set(k, s.Day);
                if (s.Day - st.MixedSince.Get(k).Value > Sim.YEAR && s.Rng.Chance(0.08))
                {
                    bool first = !J.Some(w.Settlements, o => (o.Pop.Get("halfelf") ?? 0) > 0);
                    s.AddPop(st, "halfelf", 1);
                    s.Metric("halfbreed");
                    if (first) s.Log("migration", $"{Tr.Ek(st.Name, "da")} ilk Yarı-elf doğdu.", civ: st.Civ, tile: st.Tile, cause: "İnsanlar ve Elfler bir yıldır aynı ocakta yaşıyor", major: true);
                }
            }
            else st.MixedSince.Delete(k);
        }
        // Yeni kurucular
        for (int i = 0; i < w.Civs.Count; i++)
        {
            var c = w.Civs[i];
            if (c.Alive || J.T(c.Respawned) || s.Day - (c.ExtinctDay ?? 0) < 150) continue;
            c.Respawned = true;
            SpawnFounders(s);
        }
    }

    private static double Prosperity(Sim s, Civ c)
    {
        double pop = s.CivPop(c);
        if (!J.T(pop)) return 0;
        double food = JsMath.Min(3, s.FoodTotal(c) / (pop * Sim.FOOD_PER_POP * 60));
        double hous = J.Sum(s.CivSettlements(c), x => s.Housing(x)) / pop;
        return food + JsMath.Min(1.5, hous);
    }

    private static void SpawnFounders(Sim s)
    {
        var w = s.W;
        var used = new HashSet<string>();
        foreach (var x in w.Civs) if (x.Alive) used.Add(x.Cls);
        var pool = J.Filter(D.CLASS_IDS, k => D.CLASSES[k].Implemented && !used.Contains(k));
        if (pool.Count == 0) return;
        string cls = s.Rng.Pick(pool);
        var avoid = J.Map(J.Filter(w.Settlements, x => x.Alive), x => x.Tile);
        int best = -1; double bs = double.NegativeInfinity;
        for (int i = 0; i < w.Tiles.Count; i++)
        {
            var t = w.Tiles[i];
            if (t.Terrain != "grass" || t.Owner >= 0 || t.Camp != null) continue;
            double d = avoid.Count > 0 ? J.MinOf(avoid, a => s.G.Dist(a, i)) : 20;
            if (d < 10) continue;
            double sc = JsMath.Min(d, 15) + s.Rng.Next() * 3;
            if (sc > bs) { bs = sc; best = i; }
        }
        if (best < 0) return;
        int id = w.Civs.Count;
        var c = WorldGen.MakeCiv(id, cls, s.Day);
        w.Civs.Add(c);
        foreach (var row in w.Relations) row.Add(WorldGen.EmptyRel());
        w.Relations.Add(J.Map(w.Civs, _ => WorldGen.EmptyRel()));
        w.Settlements.Add(WorldGen.MakeSettlement(s.Id(), id, D.CLASSES[cls].Capital, best, new JsObj<double> { [D.CLASSES[cls].Race] = 6 }, s.Day));
        s.UpdateTerritory();
        s.RecomputeEff(c);
        Research.ChooseResearch(s, c);
        s.Log("world", $"Ufukta yeni bir topluluk belirdi: {c.Name} ({D.RACES[c.Race].Plural}, {D.CLASSES[cls].Name}).", civ: id, tile: best, cause: "Boşalan topraklar yeni yerleşimcileri çekti", major: true);
        // void TECH;
    }
}
