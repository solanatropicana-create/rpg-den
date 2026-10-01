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
            Pacts(s, a, b);   // B1: savunma paktı kurulur ya da dağılır
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
            // B1: tarihî hak: o'nun kurup kaybettiği (ya da elinden ayrılan) bir yerleşim t'nin elinde; eşik gevşer
            var claim = crusade ? null : ClaimTarget(s, o, t);
            bool reclaim = claim != null && rv <= WarThreshold(s, o) + CLAIM_EASE;
            if (!crusade && !reclaim && (rv > WarThreshold(s, o) || (tension < 10 && land < 12))) return;
            if (crusade && rv > 0) return;
            if (o.Cls == "paladin" && (J.T(r.Treaty) || J.T(s.Rel(t.Id, o.Id).Treaty))) return; // yemin: antlaşma bozulmaz
            double powO = MilitaryPower(s, o), powT = MilitaryPower(s, t);
            if (powO < powT * 0.75) return;
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
            var target = crusade ? s.Capital(t) : reclaim ? claim : depTile != null ? s.Settlement(s.W.Tiles[depTile.Value].Owner) : byLand ? border : s.Capital(t);
            if (target == null) return;
            string goodName = (goodKey != null ? D.GOODS[goodKey]?.Name : null) ?? "Toprak";
            // B1: kaynak savaşında yatak yoksa hedef başkenttir: ordu haraç ve ganimet için gelir, şehri tutmaz (yağmalar)
            bool tribute = !crusade && !reclaim && !byLand && depTile == null;
            // B1: başkente yürümek ezici üstünlük ister; yoksa (başka yerleşimi varsa) ordu en yakın taşra kasabasını hedefler
            var tCap = s.Capital(t);
            if (!reclaim && !tribute && tCap != null && target.Id == tCap.Id && powO < powT * CAPITAL_ODDS && oCap != null)
            {
                var alt = J.At(J.Sort(J.Filter(s.CivSettlements(t), x => x.Id != tCap.Id), (x, y) => s.G.Dist(x.Tile, oCap.Tile) - s.G.Dist(y.Tile, oCap.Tile)), 0);
                if (alt != null) target = alt;
            }
            var war = new War
            {
                Since = s.Day, Attacker = o.Id, Target = target.Id, Attacks = 0, LastArmy = -999,
                Goal = crusade ? $"Kutsal Sefer: {Tr.Ek(t.Name, "in")} karanlık paktını yıkmak" : reclaim ? ReclaimGoal(s, o, target) : byLand ? $"{Tr.Ek(target.Name, "i")} ve çevresindeki toprakları almak" : $"{goodName} kaynağını ele geçirmek",
                Kind = crusade ? "crusade" : reclaim ? "reclaim" : tribute ? "tribute" : null,
            };
            r.War = war; s.Rel(t.Id, o.Id).War = war;
            s.Metric("war");
            if (reclaim) s.Metric("reclaimWar");
            s.Log("war", $"{o.Name}, {Tr.Ek(t.Name, "a")} SAVAŞ İLAN ETTİ! Hedef: {target.Name}.", civ: o.Id, tile: target.Tile,
                cause: $"İlişki {J.S(rv)}: {string.Join(", ", J.Map(J.Filter(r.Mods, m => m.Value < 0), m => J.TrLower(m.Text)))}{(reclaim ? $"; tarihî hak: {ClaimWhy(s, o, target)}" : "")}", major: true);
            CallAllies(s, o, t, war, target);
            // B1: pakt ortaklarının yardımı ya da ihaneti; kötü saldırgana karşı Kutsal Sefer
            OnWarDeclared(s, o, t);
            // Paladin kutsal seferi: iyi medeniyetler kötüye karşı yakınlaşır (B1: çağrıya başkaları da katılabilir)
            if (crusade) CallCrusade(s, t, null, $"{o.Name} {Tr.Ek(t.Name, "a")} karşı Kutsal Sefer ilan etti");
            return;
        }
        if (r.War.Attacker != o.Id) return;
        var tgt = s.Settlement(r.War.Target);
        bool won = tgt == null || !tgt.Alive || tgt.Civ != t.Id;   // hedef artık düşmanın değil (biz ya da müttefik aldı)
        bool isLong = s.Day - r.War.Since > 300, tired = r.War.Attacks >= 3;
        // B1: pakt savaşı, korunan ortağın savaşı bitince biter
        var ally = r.War.Kind == "pact" && r.War.Ally != null ? J.At(s.W.Civs, r.War.Ally.Value) : null;
        bool allyDone = ally != null && !s.AtWar(ally.Id, t.Id);
        if (won || isLong || tired || allyDone)
        {
            string why = won ? "Savaş hedefi ele geçirildi" : allyDone ? (ally.Alive ? $"{ally.Name} barış yaptı; pakt yükümlülüğü bitti" : $"{ally.Name} yok oldu; korunacak kimse kalmadı")
                : tired ? "Ordular yıprandı" : "Savaş uzadı, halk yoruldu";
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
            if (HasPact(s, y, t)) continue;   // B1: pakt ortağına saldırıya gönüllü olunmaz
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

    /// <summary>
    /// B1: anakara yerleşim tavanı <c>2 + çağ + ⌊nüfus/200⌋</c> (eskiden sabit 5): Kamp çağında 3, Krallık çağında 6 ve
    /// her 200 nüfusa bir fazlası. Yalnız öncü göndermeyi sınırlar; fetihle tavan aşılabilir. Denizaşırı koloniler ayrı
    /// haktır (Gemicilik 1, Seyir 2) ve bu tavana sayılmaz.
    /// </summary>
    public static int LandCap(Sim s, Civ c) => 2 + c.Era + (int)Math.Floor(s.CivPop(c) / 200);

    /// <summary>Öncü (settlers) gönderme: anakarada yerleşim tavanına (<see cref="LandCap"/>) dek, Gemicilik/Seyir ile denizaşırı koloni.</summary>
    public static void ConsiderExpansion(Sim s, Civ c)
    {
        var ss = s.CivSettlements(c);
        var cap = s.Capital(c);
        if (cap == null || !s.Has(c, "roads")) return;
        // B1: savaşta ya da başkentini yeni kaybetmişken kimse öncü yollamaz (yıkılan medeniyet köy kurarak ayakta kalmaz)
        if (s.InWar(c) || s.Day - (c.CapitalLostDay ?? -99999) < 3 * Sim.YEAR) return;
        // anakarada LandCap kadar yerleşim; Gemicilik ve Seyir birer denizaşırı koloni hakkı daha açar
        int over = J.Filter(ss, x => J.T(x.Overseas)).Count;
        bool landOk = ss.Count - over < LandCap(s, c);
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
            // B1: boşalan topraklar yeni kurucuları çeker; fethedilerek yok olanın toprağı fatihindir (boş değildir)
            if (c.FallCause == null) SpawnFounders(s);
        }
        // B1: bölünme: başkente uzak, yaralı ya da mutsuz bir yerleşim ayrılıp yeni bir medeniyet kurabilir
        // (liste döngüde büyüyebilir: yeni doğan medeniyet de gezilir, ama 10 yıl bölünemez)
        for (int i = 0; i < w.Civs.Count; i++)
        {
            var c = w.Civs[i];
            if (c.Alive) ConsiderSecession(s, c);
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

    // ================================================================ B1: yükseliş ve çöküş
    // Başkent düşebilir (Agents.Siege → CanHoldCapital / CapitalFell), kaybedilen yerleşim üzerinde tarihî hak kalır
    // (savaş türü "reclaim"), uzak ve mutsuz yerleşimler ayrılıp yeni medeniyet kurar (ConsiderSecession), dostlar savunma
    // paktı kurar (Pacts / CallPact; sırt çeviren ya da ortağına saldıran "İhanet" damgası yer), kötünün saldırısı Kutsal
    // Sefer çağrısı doğurur (CallCrusade).

    /// <summary>bölünme: medeniyet başına en az bu kadar gün arayla (10 yıl)</summary>
    public const double SECEDE_GAP = 10 * Sim.YEAR;
    /// <summary>bölünebilmek için en az bu kadar yerleşim (ana medeniyete en az 2 kalır)</summary>
    public const int SECEDE_MIN_SETTLEMENTS = 3;
    /// <summary>ayrılacak yerleşimin en az nüfusu (bir devlet kuracak kadar kalabalık)</summary>
    public const double SECEDE_POP = 16;
    /// <summary>huzursuzluk bu eşiği aşınca ayrılık olasılığı başlar (bkz. Unrest: süregelen ve şimdiki yara birlikte gerekir)</summary>
    public const double UNREST_MIN = 3.25;
    /// <summary>diyarda iki bölünme arasında en az (bir "karışıklık çağı" art arda bölünmeleri sınırlar)</summary>
    public const double SECEDE_WORLD_GAP = 3 * Sim.YEAR;
    /// <summary>yaşayan medeniyet tavanı (bölünme bunu aşamaz)</summary>
    public const int MAX_CIVS = 14;
    /// <summary>W.Civs listesinin (ölüler dâhil) tavanı: ilişki tablosu sınırsız büyümesin</summary>
    public const int MAX_CIV_SLOTS = 40;
    /// <summary>tarihî hakkın süresi (gün)</summary>
    public const double CLAIM_DAYS = 15 * Sim.YEAR;
    /// <summary>tarihî hak, toprak ya da kaynak gerekçesinin yerini tutar ve savaş eşiğini bu kadar gevşetir</summary>
    public const double CLAIM_EASE = 5;
    /// <summary>pakt kurmak için karşılıklı ilişki en az; pakt bunun altına düşünce (PACT_KEEP) dağılır</summary>
    public const double PACT_MIN = 40, PACT_KEEP = 0;
    /// <summary>pakt yalnız başkentleri bu kadar yakın komşular arasında kurulur</summary>
    public const double PACT_RANGE = 35;
    /// <summary>medeniyet başına en çok pakt (tek ittifak: zincirleme dünya savaşları olmasın)</summary>
    public const int PACT_MAX = 1;
    /// <summary>ortak tehdidin gücü (iki medeniyetin ona duyduğu düşmanlık toplamı, savaş +40, kötülük +15)</summary>
    public const double PACT_THREAT = 50;
    /// <summary>aynı kötü medeniyete karşı iki Kutsal Sefer çağrısı arasında en az</summary>
    public const double CRUSADE_GAP = 8 * Sim.YEAR;
    /// <summary>başkenti tutmak için sağ kalan güç (asker + kahraman × 3): en az HOLD_MIN ve şehrin nüfusunun HOLD_SHARE payı</summary>
    public const double HOLD_MIN = 4, HOLD_SHARE = 0.12;
    /// <summary>başkente yürümek için gereken askerî üstünlük (MilitaryPower oranı); altında taşra kasabası hedeflenir</summary>
    public const double CAPITAL_ODDS = 1.5;
    /// <summary>başkenti düşen medeniyetin yeni başkenti bu kadar gün savunmada +RALLY_AC zırh alır (halk kenetlenir)</summary>
    public const double RALLY_DAYS = 3 * Sim.YEAR, RALLY_AC = 2;

    /// <summary>Kötü medeniyet: iyiliği düşük ya da karanlık bir patrona paktla bağlı (Paktçı).</summary>
    public static bool IsEvil(Sim s, Civ c) => c.Align.Good <= -0.3 || s.E(c, "pact") > 0;

    public static bool HasPact(Sim s, Civ a, Civ b) => s.Rel(a.Id, b.Id).Pact != null;

    private static int PactCount(Sim s, Civ c)
    {
        int n = 0;
        foreach (var o in s.W.Civs) if (o.Alive && o.Id != c.Id && s.Rel(c.Id, o.Id).Pact != null) n++;
        return n;
    }

    /// <summary>x, y'yi hâlâ hain biliyor mu (sönmemiş İhanet damgası ya da çiğnenmiş pakt kini).</summary>
    private static bool Stained(Sim s, Civ x, Civ y) => J.Some(s.Rel(x.Id, y.Id).Mods, m => (m.Key == "ihanet" || m.Key == "betrayed") && m.Value <= -2);

    private static void EndPact(Sim s, Civ a, Civ b)
    {
        s.Rel(a.Id, b.Id).Pact = null; s.Rel(b.Id, a.Id).Pact = null;
        s.RemoveMod(a.Id, b.Id, "defpact");
    }

    private static string JoinNames(List<string> xs) => xs.Count <= 1 ? string.Join("", xs) : $"{string.Join(", ", J.Slice(xs, 0, -1))} ve {xs[xs.Count - 1]}";

    /// <summary>of medeniyetinin, to medeniyetinin başkentine en yakın yerleşimi (yoksa null).</summary>
    private static Settlement NearestSettlement(Sim s, Civ of, Civ to)
    {
        var cap = s.Capital(to);
        if (cap == null) return null;
        Settlement best = null; double bd = double.PositiveInfinity;
        foreach (var st in s.CivSettlements(of)) { double d = s.G.Dist(st.Tile, cap.Tile); if (d < bd) { bd = d; best = st; } }
        return best;
    }

    // ------------------------------------------------------------ tarihî hak
    /// <summary>t'nin elindeki, o'nun tarihî hakkı olan yerleşimlerden o'nun başkentine en yakını (yoksa null).</summary>
    private static Settlement ClaimTarget(Sim s, Civ o, Civ t)
    {
        var cap = s.Capital(o);
        if (cap == null) return null;
        Settlement best = null; double bd = double.PositiveInfinity;
        foreach (var st in s.W.Settlements)
        {
            if (!st.Alive || st.Civ != t.Id || st.ClaimBy != o.Id || !(s.Day < (st.ClaimUntil ?? 0))) continue;
            double d = s.G.Dist(st.Tile, cap.Tile);
            if (d < bd) { bd = d; best = st; }
        }
        return best;
    }

    private static bool BrokeAway(Sim s, Settlement st, Civ o) => st.Civ >= 0 && st.Civ < s.W.Civs.Count && s.W.Civs[st.Civ].Parent == o.Id && st.Founder != st.Civ;

    private static string ReclaimGoal(Sim s, Civ o, Settlement st) =>
        BrokeAway(s, st, o) ? $"Asi {Tr.Ek(st.Name, "i")} yeniden bayrağa bağlamak" : $"Kayıp {Tr.Ek(st.Name, "i")} geri almak";

    private static string ClaimWhy(Sim s, Civ o, Settlement st) =>
        BrokeAway(s, st, o) ? $"{st.Name} bir zamanlar onlarındı, ayrılıp kendi yolunu tuttu" : $"{Tr.Ek(st.Name, "i")} onlar kurmuştu, {Lore.DateTr(st.LostDay ?? s.Day)} ellerinden alındı";

    // ------------------------------------------------------------ savaş ilanının yankıları
    /// <summary>Sıradan bir savaş ilanından sonra: saldırgan paktlı ortağına saldırdıysa ihanet; hedefin pakt ortakları
    /// yardıma koşar ya da sırt çevirir; saldırgan kötüyse iyi medeniyetler Kutsal Sefere çağrılır.</summary>
    private static void OnWarDeclared(Sim s, Civ att, Civ def)
    {
        if (HasPact(s, att, def))
        {
            EndPact(s, att, def);
            Betray(s, att, def, $"{att.Name}, pakt ortağı {Tr.Ek(def.Name, "a")} savaş açtı");
        }
        CallPact(s, att, def);
        if (IsEvil(s, att) && !IsEvil(s, def)) CallCrusade(s, att, def, $"{att.Name}, {Tr.Ek(def.Name, "a")} saldırdı");
    }

    /// <summary>İhanet: x'i tanıyan herkes ona damga vurur (yavaş söner); ihanete uğrayan ayrıca daha ağır bir kin tutar.</summary>
    private static void Betray(Sim s, Civ x, Civ victim, string why)
    {
        foreach (var o in s.W.Civs)
        {
            if (!o.Alive || o.Id == x.Id || !s.Rel(o.Id, x.Id).Contact) continue;
            if (o.Id == victim.Id) s.AddMod(o.Id, x.Id, "betrayed", "Paktımızı çiğnedi", -30, -50, 0.01, false);
            s.AddMod(o.Id, x.Id, "ihanet", "İhanet damgası", -20, -40, 0.015, false);
        }
        s.Metric("betrayal");
        s.Log("diplomacy", $"İhanet! {x.Name} savunma paktını çiğnedi; diyar onu artık hain biliyor.", civ: x.Id, cause: why, major: true);
    }

    /// <summary>Saldırıya uğrayan def'in pakt ortakları: gücü ve yolu olan çoğunlukla savaşa girer (saldırganın en yakın
    /// yerleşimine yürür); gönülsüz olan sırt çevirir ve İhanet damgası yer; gücü yetmeyenin paktı sessizce çözülür.</summary>
    private static void CallPact(Sim s, Civ att, Civ def)
    {
        for (int i = 0; i < s.W.Civs.Count; i++)
        {
            var q = s.W.Civs[i];
            if (!q.Alive || q.Id == att.Id || q.Id == def.Id || !HasPact(s, q, def) || s.AtWar(q.Id, att.Id)) continue;
            if (HasPact(s, q, att))
            {
                s.Log("diplomacy", $"{q.Name}, iki pakt ortağı {att.Name} ile {def.Name} arasındaki savaşta tarafsız kaldı.", civ: q.Id, cause: "İki yana da ant içmişti");
                continue;
            }
            var target = NearestSettlement(s, att, q);
            var cap = s.Capital(q);
            bool able = cap != null && target != null && J.Sum(s.CivSettlements(q), x => x.Soldiers) >= 3 && ArmyRoute(s, q, cap.Tile, target.Tile) != null;
            if (!able)
            {
                EndPact(s, q, def);
                s.AddMod(def.Id, q.Id, "nohelp", "Yardıma gelmedi", -10, -20, 0.02, false);
                s.Log("diplomacy", $"{q.Name}, pakt ortağı {Tr.Ek(def.Name, "in")} yardımına koşamadı; pakt çözüldü.", civ: q.Id, cause: "Ne ordusu ne de yolu yetiyordu");
                continue;
            }
            double rel = s.RelValue(q.Id, att.Id);
            double p = JsMath.Max(0.3, JsMath.Min(0.97, 0.9 + q.Align.Law * 0.1 - (rel > 40 ? 0.25 : 0) - (MilitaryPower(s, q) < MilitaryPower(s, att) * 0.4 ? 0.2 : 0)));
            if (q.Cls == "paladin") p = JsMath.Max(p, 0.97);   // yemin: paladin ant bozmaz
            if (s.Rng.Chance(p))
            {
                var w2 = new War { Since = s.Day, Attacker = q.Id, Target = target.Id, Attacks = 0, LastArmy = -999, Goal = $"Savunma paktı: {Tr.Ek(def.Name, "i")} korumak", Ally = def.Id, Kind = "pact" };
                s.Rel(q.Id, att.Id).War = w2; s.Rel(att.Id, q.Id).War = w2;
                s.SetMod(q.Id, def.Id, "brothers", "Silah arkadaşlığı", 14, 0.01);
                s.Metric("pactCall");
                s.Log("war", $"{q.Name}, savunma paktı gereği {Tr.Ek(att.Name, "a")} savaş ilan etti! Hedef: {target.Name}.", civ: q.Id, tile: target.Tile,
                    cause: $"{att.Name}, pakt ortağı {Tr.Ek(def.Name, "a")} saldırdı", major: true);
            }
            else
            {
                EndPact(s, q, def);
                Betray(s, q, def, $"{q.Name}, saldırıya uğrayan pakt ortağı {Tr.Ek(def.Name, "in")} çağrısına kulak tıkadı{(rel > 40 ? $" ({att.Name} ile arası iyiydi)" : "")}");
            }
        }
    }

    /// <summary>Kutsal Sefer: kötü e'nin saldırısı ya da fethi (ya da bir paladin seferi) üzerine, onu tanıyan iyi
    /// medeniyetlerden en istekli ikisi (savaşta değilse, yolu varsa) e'nin kendilerine en yakın yerleşimine yürür.</summary>
    private static void CallCrusade(Sim s, Civ e, Civ victim, string why)
    {
        if (!e.Alive || s.Day - (e.CrusadeDay ?? -99999) < CRUSADE_GAP) return;
        var cands = new List<(Civ G, Settlement T, double Sc)>();
        foreach (var g in s.W.Civs)
        {
            if (!g.Alive || g.Id == e.Id || (victim != null && g.Id == victim.Id) || g.Align.Good < 0.3 || IsEvil(s, g)) continue;
            if (!s.Rel(g.Id, e.Id).Contact || s.AtWar(g.Id, e.Id) || HasPact(s, g, e) || s.InWar(g) || s.Day - (g.LastWarEnd ?? -9999) < Sim.YEAR) continue;
            double hate = s.RelValue(g.Id, e.Id);
            if (hate > 10 || J.Sum(s.CivSettlements(g), x => x.Soldiers) < 4) continue;
            var cap = s.Capital(g);
            var tg = NearestSettlement(s, e, g);
            if (cap == null || tg == null || s.G.Dist(cap.Tile, tg.Tile) > 45 || ArmyRoute(s, g, cap.Tile, tg.Tile) == null) continue;
            double sc = g.Align.Good * 30 - hate + (victim != null ? JsMath.Max(0, s.RelValue(g.Id, victim.Id)) * 0.3 : 0)
                + (s.E(g, "crusade") > 0 || g.Cls == "paladin" || g.Cls == "cleric" ? 20 : 0);
            cands.Add((g, tg, sc));
        }
        if (cands.Count == 0) return;
        J.Sort(cands, (a, b) => b.Sc - a.Sc);
        var joined = new List<(Civ G, Settlement T)>();
        foreach (var (g, tg, sc) in J.Slice(cands, 0, 2))
        {
            double p = JsMath.Min(0.9, 0.35 + g.Align.Good * 0.4 + (sc >= 40 ? 0.15 : 0));
            if (!s.Rng.Chance(p)) continue;
            var w2 = new War { Since = s.Day, Attacker = g.Id, Target = tg.Id, Attacks = 0, LastArmy = -999, Goal = $"Kutsal Sefer: {Tr.Ek(e.Name, "in")} karanlığına karşı", Ally = victim?.Id, Kind = "crusade" };
            s.Rel(g.Id, e.Id).War = w2; s.Rel(e.Id, g.Id).War = w2;
            joined.Add((g, tg));
        }
        if (joined.Count == 0) return;
        e.CrusadeDay = s.Day;
        for (int i = 0; i < joined.Count; i++) for (int k = i + 1; k < joined.Count; k++) s.SetMod(joined[i].G.Id, joined[k].G.Id, "brothers", "Silah arkadaşlığı", 14, 0.01);
        s.Metric("crusade");
        s.Metric("crusadeCivs", joined.Count);
        string names = JoinNames(J.Map(joined, x => x.G.Name));
        string targets = string.Join(", ", J.Unique(J.Map(joined, x => x.T.Name)));
        s.Log("war", $"KUTSAL SEFER! {names} {Tr.Ek(e.Name, "a")} karşı silaha sarıldı. Hedef: {targets}.", civ: joined[0].G.Id, tile: joined[0].T.Tile,
            cause: $"{why}; karanlığın gölgesi büyüyor", major: true);
    }

    // ------------------------------------------------------------ savunma paktı
    /// <summary>Her 10 günde bir çift için: dost (karşılıklı ilişki ≥ PACT_MIN), komşu (başkentler PACT_RANGE içinde) ve ortak bir tehdidin (ikisine de düşman,
    /// birine savaş açmış ya da kötü bir medeniyet) karşısındaki iki medeniyet savunma paktı kurabilir (medeniyet başına
    /// en çok PACT_MAX). İlişki 0'ın altına düşünce pakt sessizce dağılır.</summary>
    private static void Pacts(Sim s, Civ a, Civ b)
    {
        var r = s.Rel(a.Id, b.Id);
        if (!a.Alive || !b.Alive || !r.Contact) return;
        double ab = s.RelValue(a.Id, b.Id), ba = s.RelValue(b.Id, a.Id);
        if (r.Pact != null)
        {
            if (r.War == null && ab >= PACT_KEEP && ba >= PACT_KEEP) return;
            EndPact(s, a, b);
            if (r.War == null) s.Log("diplomacy", $"{a.Name} ile {b.Name} arasındaki savunma paktı sona erdi.", civ: a.Id, cause: $"İlişkiler soğudu ({J.S(JsMath.Min(ab, ba))})");
            return;
        }
        if (r.War != null || ab < PACT_MIN || ba < PACT_MIN) return;
        if (PactCount(s, a) >= PACT_MAX || PactCount(s, b) >= PACT_MAX) return;
        var ca0 = s.Capital(a); var cb0 = s.Capital(b);
        if (ca0 == null || cb0 == null || s.G.Dist(ca0.Tile, cb0.Tile) > PACT_RANGE) return;   // yalnız komşular ant içer
        if (Stained(s, a, b) || Stained(s, b, a)) return;   // damgası sönmemiş hainle ant içilmez
        Civ threat = null; double tv = 0;
        foreach (var t in s.W.Civs)
        {
            if (!t.Alive || t.Id == a.Id || t.Id == b.Id) continue;
            bool ca = s.Rel(a.Id, t.Id).Contact, cb = s.Rel(b.Id, t.Id).Contact;
            if (!ca && !cb) continue;
            double v = -(ca ? s.RelValue(a.Id, t.Id) : 0) - (cb ? s.RelValue(b.Id, t.Id) : 0);
            if (s.AtWar(a.Id, t.Id) || s.AtWar(b.Id, t.Id)) v += 40;
            if (IsEvil(s, t) && !IsEvil(s, a) && !IsEvil(s, b)) v += 15;
            if (v > tv) { tv = v; threat = t; }
        }
        if (threat == null || tv < PACT_THREAT || !s.Rng.Chance(0.08)) return;
        r.Pact = s.Day; s.Rel(b.Id, a.Id).Pact = s.Day;
        s.SetMod(a.Id, b.Id, "defpact", "Savunma paktı", 10);
        s.Metric("defPact");
        s.Log("diplomacy", $"{a.Name} ile {b.Name} savunma paktı imzaladı: birine saldıran ikisini birden karşısında bulacak.", civ: a.Id,
            cause: $"Ortak tehdit: {threat.Name}; karşılıklı ilişki {J.S(ab)}/{J.S(ba)}", major: true);
    }

    // ------------------------------------------------------------ başkentin düşüşü (Agents.Siege)
    /// <summary>Kuşatmayı kazanan orduların sağ kalan gücü (asker + kahraman × 3) başkenti tutmaya yetiyor mu: en az
    /// HOLD_MIN ve şehrin (kayıplardan sonraki) nüfusunun HOLD_SHARE payı. Haraç seferi (yatağı olmayan kaynak savaşı)
    /// başkenti hiç tutmaz, yağmalar.</summary>
    public static bool CanHoldCapital(Sim s, List<Agent> band, Settlement st, War war)
    {
        if (war != null && war.Kind == "tribute") return false;
        double hold = 0;
        foreach (var a in band) hold += JsMath.Max(0, a.Troops ?? 0) + (a.Heroes?.Count ?? 0) * 3;
        return hold >= JsMath.Max(HOLD_MIN, s.Pop(st) * HOLD_SHARE);
    }

    /// <summary>Başkent yağmalanıp tutulamadıysa kroniğe düşen neden.</summary>
    public static string SackWhy(War war) => war != null && war.Kind == "tribute" ? "Haraç ve ganimet için gelmişlerdi, şehri tutmadılar" : "Şehri tutmaya güçleri yetmedi; yağmalayıp çekildiler";

    /// <summary>Fethedilen her yerleşim için (Agents.Siege, şehir el değiştirdikten sonra): kurucu ve tarihî hak; kötü
    /// bir fatih Kutsal Sefer çağrısı doğurur. Hak, şehri kuranındır (kaybedince doğar, geri alınca düşer); bölünmeden
    /// doğan hak (ana medeniyetin asi şehir üzerindeki hakkı) şehir üçüncü bir ele geçince düşer.</summary>
    public static void AfterConquest(Sim s, Settlement st, Civ wc, Civ dfc)
    {
        st.Founder ??= dfc.Id;
        st.LostDay = s.Day;
        if (st.ClaimBy == wc.Id) { st.ClaimBy = null; st.ClaimUntil = null; }   // hak sahibi geri aldı
        else if (st.Founder == dfc.Id) { st.ClaimBy = dfc.Id; st.ClaimUntil = s.Day + CLAIM_DAYS; }
        else if (st.ClaimBy != null && st.ClaimBy != st.Founder) { st.ClaimBy = null; st.ClaimUntil = null; }
        if (IsEvil(s, wc) && !IsEvil(s, dfc)) CallCrusade(s, wc, dfc, $"{wc.Name}, {Tr.Ek(st.Name, "i")} ele geçirdi");
    }

    /// <summary>Başkent düştü (şehir el değiştirdikten sonra): başka yerleşimi varsa başkent en kalabalığına taşınır
    /// (iki büyük olay), yoksa medeniyet son kalesiyle birlikte tarihten silinir.</summary>
    public static void CapitalFell(Sim s, Settlement st, Civ wc, Civ dfc, string nameA, string loot, int battle, string goal)
    {
        s.Metric("capitalFall");
        if (s.CivSettlements(dfc).Count == 0)
        {
            s.Metric("lastStand");
            dfc.FallCause = $"Son kalesi {st.Name}, {Tr.Ek(wc.Name, "in")} eline geçti";
            s.Log("war", $"{nameA}, {Tr.Ek(dfc.Name, "in")} son kalesi {Tr.Ek(st.Name, "i")} düşürdü! Ganimet: {loot}.", civ: wc.Id, tile: st.Tile, battle: battle, cause: goal, major: true);
            s.Extinct(dfc);
            return;
        }
        dfc.CapitalLostDay = s.Day;
        var ncap = s.Capital(dfc);
        s.Log("war", $"{nameA} {Tr.Ek(dfc.Name, "in")} başkenti {Tr.Ek(st.Name, "i")} düşürdü! Şehir artık {Tr.Ek(wc.Name, "in")}; ganimet: {loot}.", civ: wc.Id, tile: st.Tile, battle: battle, cause: goal, major: true);
        s.Log("war", $"{dfc.Name} başkentini {Tr.Ek(ncap.Name, "a")} taşıdı.", civ: dfc.Id, tile: ncap.Tile, cause: $"{st.Name} düştü; saray ve hazine {Tr.Ek(ncap.Name, "a")} kaçırıldı", major: true);
        foreach (var h in s.CivHeroes(dfc)) if (h.State == "home" && h.Pos == st.Tile) Heroes.SendHero(s, h, ncap.Tile, "home");
    }

    // ------------------------------------------------------------ bölünme
    private static string MajorityRace(Settlement st)
    {
        string best = null; double bv = 0;
        foreach (var kv in st.Pop) if (kv.Value > bv) { bv = kv.Value; best = kv.Key; }
        return best;
    }

    private static bool IsWarTarget(Sim s, Settlement st)
    {
        foreach (var row in s.W.Relations) foreach (var r in row) if (r.War != null && r.War.Target == st.Id) return true;
        return false;
    }

    /// <summary>Medeniyetin sürmekte olan en uzun savaşı (gün; yoksa 0).</summary>
    private static double LongestWar(Sim s, Civ c)
    {
        double m = 0;
        foreach (var o in s.W.Civs) { if (o.Id == c.Id) continue; var war = s.Rel(c.Id, o.Id).War; if (war != null) m = JsMath.Max(m, s.Day - war.Since); }
        return m;
    }

    /// <summary>
    /// Yerleşimin huzursuzluğu (0: ayrılamaz). İki tür yara birlikte gerekir: <b>süregelen</b> (başkente uzaklık ya da
    /// denizaşırılık, farklı çoğunluk ırkı, son 20 yılda zorla alınmış olmak) ve <b>şimdiki</b> bir sarsıntı (başkentin
    /// düşmesi, kıtlık, salgın, uzayan savaş, yakın zamanda yakılıp yıkılmak). Taşranın genişliği ekler, kanunlu
    /// medeniyet daha sağlam tutar. Nedenler why'a yazılır.
    /// </summary>
    private static double Unrest(Sim s, Civ c, Settlement st, Settlement cap, int count, List<string> why)
    {
        double d = s.G.Dist(st.Tile, cap.Tile);
        bool overseas = J.T(st.Overseas) || (s.W.Tiles[st.Tile].Isle ?? 0) != (s.W.Tiles[cap.Tile].Isle ?? 0);
        // süregelen yaralar
        double chronic = 0;
        double far = overseas ? 1.5 : JsMath.Max(0, JsMath.Min(1.5, (d - 10) / 8));
        if (far > 0) { chronic += far; why.Add(overseas ? "denizin ötesinde, başkentten kopuk" : d >= 18 ? "başkentten çok uzakta" : "başkentten uzakta"); }
        string maj = MajorityRace(st);
        if (maj != null && maj != c.Race) { chronic += 1; why.Add($"halkının çoğu {J.TrLower(D.RACES[maj].Plural)}"); }
        if (st.Founder != null && st.Founder != c.Id && st.LostDay != null && s.Day - st.LostDay.Value < 20 * Sim.YEAR)
        {
            chronic += s.Day - st.LostDay.Value < 10 * Sim.YEAR ? 1 : 0.5;
            var f = J.At(s.W.Civs, st.Founder.Value);
            why.Add(f != null ? $"{Tr.Ek(f.Name, "in")} kurduğu bu şehir zorla alınmıştı" : "zorla alınmıştı");
        }
        if (chronic < 1.5) return 0;
        // şimdiki sarsıntı
        double acute = 0;
        if (c.CapitalLostDay != null && s.Day - c.CapitalLostDay.Value < 3 * Sim.YEAR) { acute += 1.5; why.Add("başkent düştü, taht sarsıldı"); }
        if (st.Starving > 0) { acute += 1; why.Add("kıtlık kapıda"); }
        if (st.Plague != null) { acute += 1; why.Add("salgın kol geziyor"); }
        if (LongestWar(s, c) > 200) { acute += 0.75; why.Add("savaş bitmek bilmiyor"); }
        if (st.BurnedAt != null && s.Day - st.BurnedAt.Value < Sim.YEAR) { acute += 0.5; why.Add("evleri yakılıp yıkıldı"); }
        if (acute < 1) return 0;
        double u = chronic + acute;
        if (count >= 9) { u += JsMath.Min(1, (count - 8) * 0.25); why.Add("taşra yönetilemeyecek kadar geniş"); }
        u -= c.Align.Law * 0.5;
        return u;
    }

    /// <summary>Her 30 günde (medeniyet başına): en huzursuz uzak yerleşim, eşiği aşınca olasılıkla ayrılır. Medeniyet
    /// başına 10 yılda en çok bir bölünme; yaşayan medeniyet sayısı MAX_CIVS'i aşamaz.</summary>
    private static void ConsiderSecession(Sim s, Civ c)
    {
        var w = s.W;
        if (s.Day - (c.LastSecession ?? -99999) < SECEDE_GAP || w.Civs.Count >= MAX_CIV_SLOTS) return;
        foreach (var x in w.Civs) if (x.Parent != null && s.Day - x.Founded < SECEDE_WORLD_GAP) return;
        var ss = s.CivSettlements(c);
        if (ss.Count < SECEDE_MIN_SETTLEMENTS) return;
        int alive = 0;
        foreach (var x in w.Civs) if (x.Alive) alive++;
        if (alive >= MAX_CIVS) return;
        var cap = s.Capital(c);
        if (cap == null) return;
        Settlement best = null; double bu = 0; List<string> bwhy = null;
        foreach (var st in ss)
        {
            if (st.Id == cap.Id || J.T(st.Civics.Get("wonder")) || s.Pop(st) < SECEDE_POP || s.Pop(st) >= s.Pop(cap) - 2 || IsWarTarget(s, st)) continue;
            var why = new List<string>();
            double u = Unrest(s, c, st, cap, ss.Count, why);
            if (u > bu) { bu = u; best = st; bwhy = why; }
        }
        if (best == null || bu < UNREST_MIN) return;
        if (!s.Rng.Chance(JsMath.Min(0.08, 0.015 + (bu - UNREST_MIN) * 0.03))) return;
        Secede(s, c, best, cap, bwhy);
    }

    private static readonly Dictionary<string, string> STATE_NAME = new()
    {
        ["paladin"] = "{0} Beyliği", ["cleric"] = "{0} Klanı", ["druid"] = "{0} Çemberi", ["rogue"] = "{0} Serbest Şehri",
        ["wizard"] = "{0} Şehir Devleti", ["barbarian"] = "{0} Boyu", ["bard"] = "Özgür {0}", ["fighter"] = "{0} Lejyonu",
        ["monk"] = "{0} Tarikatı", ["ranger"] = "{0} Bekçileri", ["sorcerer"] = "{0} Hanedanı", ["warlock"] = "{0} Kara Beyliği",
    };
    private static readonly string[] STATE_COLORS = { "#2e8b8b", "#9b6a2f", "#c9a227", "#5662b8", "#8e3f8e", "#6b8e23", "#c0504d", "#4a6fa5", "#a0522d", "#3c8d5a", "#b8860b", "#7d5ba6" };

    /// <summary>Ayrılan yerleşimin sınıfı: halkı ana medeniyetle aynı ırksa aynı sınıf; değilse şehri kuran medeniyetin
    /// sınıfı (o ırktansa), yoksa o ırkın yaşayanlarda olmayan, o da yoksa ilk sınıfı.</summary>
    private static string SecessionClass(Sim s, Civ c, Settlement st, string maj)
    {
        if (maj == c.Race) return c.Cls;
        var f = st.Founder != null ? J.At(s.W.Civs, st.Founder.Value) : null;
        if (f != null && f.Race == maj) return f.Cls;
        var pool = J.Filter(D.CLASS_IDS, k => D.CLASSES[k].Implemented && D.CLASSES[k].Race == maj);
        if (pool.Count == 0) return c.Cls;
        var free = J.Filter(pool, k => !J.Some(s.W.Civs, x => x.Alive && x.Cls == k));
        return (free.Count > 0 ? free : pool)[0];
    }

    private static string SecessionName(Sim s, string cls, string town)
    {
        string pat = STATE_NAME.TryGetValue(cls, out var p) ? p : "{0} Serbest Şehri";
        foreach (var n in new[] { pat.Replace("{0}", town), $"Özgür {town}", $"Yeni {pat.Replace("{0}", town)}" })
            if (!J.Some(s.W.Civs, x => x.Name == n)) return n;
        return $"{pat.Replace("{0}", town)} {J.S(s.W.Civs.Count)}";
    }

    private static string SecessionColor(Sim s)
    {
        foreach (var col in STATE_COLORS) if (!J.Some(s.W.Civs, x => x.Alive && x.Color == col)) return col;
        return STATE_COLORS[s.W.Civs.Count % STATE_COLORS.Length];
    }

    /// <summary>Yerleşim ayrılır: yeni medeniyet (id = dizin) sınıfını, adını ve rengini alır; ana medeniyetin bilgisini
    /// (sınıf aynıysa sınıf ağacını ve yolunu da), çağını, tanıdıklarını ve bildiği yatakları devralır; ambardan nüfus payı
    /// kadar mal götürür. Ana medeniyet şehir üzerinde tarihî hak tutar (savaş türü "reclaim").</summary>
    private static void Secede(Sim s, Civ c, Settlement st, Settlement cap, List<string> why)
    {
        var w = s.W;
        string maj = MajorityRace(st) ?? c.Race;
        string cls = SecessionClass(s, c, st, maj);
        int id = w.Civs.Count;
        var nc = WorldGen.MakeCiv(id, cls, s.Day);
        nc.Name = SecessionName(s, cls, st.Name);
        nc.Color = SecessionColor(s);
        nc.Race = maj;
        var al = cls == c.Cls ? c.Align : D.CLASSES[cls].Align;
        nc.Align = new Alignment { Law = JsMath.Max(-1, al.Law - 0.2), Good = al.Good };   // isyancılar biraz daha başına buyruk
        nc.Parent = c.Id;
        nc.LastSecession = s.Day;
        nc.ScoutSent = true;
        nc.LastExpand = s.Day;
        nc.LastWarEnd = s.Day;
        nc.Threat = c.Threat;
        nc.LastRaidedDay = c.LastRaidedDay;
        nc.Research.Done = cls == c.Cls ? new List<string>(c.Research.Done) : J.Filter(c.Research.Done, t => D.TECH[t].Tree == "main");
        nc.Subclass = cls == c.Cls ? c.Subclass : null;
        nc.Era = c.Era;
        nc.EraDay = new List<double>(c.EraDay);
        nc.Price = c.Price.Clone();
        double share = s.Pop(st) / JsMath.Max(1, s.CivPop(c));
        var stock = new JsObj<double>();
        foreach (var g in c.Stock.Keys())
        {
            double q = Math.Floor((c.Stock.Get(g) ?? 0) * share);
            if (q > 0) { s.Add(c, g, -q); stock.Set(g, q); }
        }
        nc.Stock = stock;
        nc.Stats.PeakPop = s.Pop(st);
        w.Civs.Add(nc);
        foreach (var row in w.Relations) row.Add(WorldGen.EmptyRel());
        w.Relations.Add(J.Map(w.Civs, _ => WorldGen.EmptyRel()));
        // şehir el değiştirir; ana medeniyet tarihî hak tutar
        st.Founder ??= c.Id;
        st.LostDay = s.Day;
        st.ClaimBy = c.Id; st.ClaimUntil = s.Day + CLAIM_DAYS;
        st.Civ = id;
        foreach (var d in w.Deposits) if (d.KnownBy.Contains(c.Id) && !d.KnownBy.Contains(id)) d.KnownBy.Add(id);
        // tanıdıklar: ana medeniyet ve onun temas kurduğu herkes
        foreach (var o in w.Civs)
        {
            if (!o.Alive || o.Id == id || (o.Id != c.Id && !s.Rel(c.Id, o.Id).Contact)) continue;
            s.Rel(id, o.Id).Contact = true; s.Rel(o.Id, id).Contact = true;
        }
        // halk silahlanır; taç isyanı bastırmadan önce (barış süresi kadar) toparlanır
        st.Soldiers += Math.Floor(s.Pop(st) * 0.1);
        s.Rel(c.Id, id).PeaceDay = s.Day; s.Rel(id, c.Id).PeaceDay = s.Day;
        // ana medeniyet asi görür, asi eski boyunduruğu; ana medeniyetin düşmanları yeni devleti hoş karşılar
        s.SetMod(c.Id, id, "rebel", $"Asi eyalet: {st.Name}", -30, 0.02, false);
        s.SetMod(id, c.Id, "yoke", "Eski boyunduruk", -25, 0.02, false);
        foreach (var o in w.Civs)
            if (o.Alive && o.Id != c.Id && o.Id != id && s.Rel(o.Id, id).Contact && (s.AtWar(o.Id, c.Id) || s.RelValue(o.Id, c.Id) <= -30))
                s.SetMod(o.Id, id, "foesfoe", $"{c.Name} düşmanı", 15, 0.02);
        // ana medeniyetin bu şehre iç ikmal yolları kesilir
        foreach (var r in w.Routes)
        {
            if (!r.Alive || (r.A != st.Id && r.B != st.Id)) continue;
            var other = s.Settlement(r.A == st.Id ? r.B : r.A);
            if (other != null && other.Civ == c.Id) r.Alive = false;
        }
        // orada bekleyen ana medeniyet kahramanları başkente döner
        foreach (var h in s.CivHeroes(c)) if (h.State == "home" && h.Pos == st.Tile) Heroes.SendHero(s, h, cap.Tile, "home");
        c.LastSecession = s.Day;
        s.UpdateTerritory();
        s.RecomputeEff(nc);
        s.RecomputeEff(c);
        Research.ChooseResearch(s, nc);
        s.Metric("secession");
        s.Log("world", $"{st.Name}, {Tr.Ek(c.Name, "dan")} ayrılıp bağımsızlığını ilan etti: {nc.Name} ({D.RACES[maj].Plural}, {D.CLASSES[cls].Name}).", civ: id, tile: st.Tile,
            cause: J.TrCap(string.Join(", ", why)), major: true);
    }
}
