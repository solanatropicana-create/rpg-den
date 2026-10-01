using System;
using System.Collections.Generic;
using System.Linq;

// Haritadaki hareketli birimler ve çatışmalar: kervan, ordu, baskın, kahraman, öncü, kâşif.
// Exact port of src/sim/agents.ts (see macro/PORTING.md).

namespace FD.Macro;

public static class Agents
{
    // ------------------------------------------------------------ birlik kompozisyonu
    private static readonly List<string> BASE = new() { "holyguard", "legionary", "shadowguard", "blessed" };

    /// <summary>Medeniyetin n kişilik birliği: araştırılmış özel birimler + temel asker (teçhizat ve etkiler dâhil).</summary>
    public static List<Combatant> CivTroops(Sim s, Civ c, double n, string side)
    {
        var @out = new List<Combatant>();
        if (n <= 0) return @out;
        var race = D.RACES[c.Race];
        bool armed = s.Has(c, "smithing");
        var gearN = Gear.GearFor(s, c, n);
        double ench = 0, mith = 0;
        var units = J.Filter(J.Map(c.Research.Done, t => D.TECH[t].Unit), u0 => J.T(u0));
        double remaining = n;
        var specials = new List<(string Id, double K)>();
        foreach (var id in units)
        {
            if (BASE.Contains(id)) continue;
            var u = D.UNITS[id];
            if (!J.T(u?.Per)) continue;
            double k = Math.Floor(n * u.Per.Value);
            if ((id == "golem" || id == "treant" || id == "dragonguard") && n >= 4) k = JsMath.Max(1, k);
            if (id == "knight") k = JsMath.Min(Math.Floor(s.St(c, "horses")), k);
            if (id == "wolves" && k == 0 && n >= 3) k = 1;
            if (k > 0) specials.Add((id, k));
        }
        string baseId = J.Find(BASE, b => units.Contains(b)) ?? "soldier";
        Combatant Mk(string id)
        {
            var u = D.UNITS[id];
            bool e = ench++ < gearN.Ench, m = mith++ < gearN.Mith;
            var cb = Combat.Unit(u, side, id == "soldier" || BASE.Contains(id) ? "soldier" : "unique", race.Hp + s.E(c, "soldierHp"), race.Atk + s.E(c, "soldierAtk") + (armed ? 1 : 0) + (e ? 2 : 0));
            cb.Ac += s.E(c, "soldierAc") + (armed && id == "soldier" ? 2 : 0) + (m ? 2 : 0);
            cb.Dmg[2] += s.E(c, "soldierDmg");
            if (s.E(c, "smite") > 0) cb.Smite = 1 + s.E(c, "smite");
            if (c.Align.Good < -0.3) cb.Evil = true;
            return cb;
        }
        foreach (var (id, k) in specials) for (int i = 0; i < k && remaining > 0; i++) { @out.Add(Mk(id)); remaining--; }
        for (int i = 0; i < remaining; i++) @out.Add(Mk(baseId));
        return @out;
    }

    /// <summary>En çok askeri olan yerleşimlerden n asker çeker; çekilenlerin ırk dağılımını (Pop) döndürür.</summary>
    public static JsObj<double> DrawSoldiers(Sim s, Civ c, double n)
    {
        var @out = new JsObj<double>();
        double left = n;
        foreach (var st in J.Sort(s.CivSettlements(c), (a, b) => b.Soldiers - a.Soldiers))
        {
            double k = JsMath.Min(left, st.Soldiers);
            var got = s.RemovePop(st, k);
            foreach (var kv in got) @out.Set(kv.Key, (@out.Get(kv.Key) ?? 0) + (got.Get(kv.Key) ?? 0));
            st.Soldiers -= k; left -= k;
            if (!J.T(left)) break;
        }
        return @out;
    }

    /// <summary>Yerleşim savunucuları: askerler, milis, sur/kalkan AC bonusları, evdeki kahramanlar, savaş avatarı.</summary>
    public static List<Combatant> Defenders(Sim s, Settlement st, string side, bool vsMonsters)
    {
        var c = s.W.Civs[st.Civ];
        var cs = CivTroops(s, c, st.Soldiers, side);
        double militia = Math.Floor((s.Pop(st) - st.Soldiers) * (vsMonsters ? 0.5 : 0.35));
        for (int i = 0; i < militia; i++) cs.Add(Combat.Unit(D.UNITS["militia"], side, "militia", D.RACES[c.Race].Hp));
        double ac = (J.T(st.Civics.Get("palisade")) ? 1 : 0) + (J.T(st.Civics.Get("stonewall")) ? 2 : 0) + (J.T(st.Civics.Get("castle")) ? 3 : 0) + s.E(c, "defAc");
        if (vsMonsters && s.E(c, "foresee") > 0) ac += 2;
        if (s.E(c, "shield") > 0 && s.Capital(c)?.Id == st.Id) ac += 4;
        if (c.Cls == "druid" && J.Some(s.G.Neighbors(st.Tile), nb => s.W.Tiles[nb].Terrain == "forest" || s.W.Tiles[nb].Terrain == "oldforest")) ac += 2;
        foreach (var x in cs) x.Ac += ac;
        foreach (var h in s.CivHeroes(c)) if (h.State == "home" && h.Pos == st.Tile) cs.Add(Combat.HeroCombatant(h, side));
        if (s.E(c, "avatar") > 0) cs.Add(Combat.Unit(new UnitStats { Name = "Savaş Avatarı", Hp = 80, Ac = 19, Atk = 9, Dmg = new List<double> { 2, 12, 5 }, Attacks = 2 }, side, "unique"));
        return cs;
    }

    private static BattleOpts CivOpts(Sim s, Civ c, string side, BattleOpts o, Civ enemy = null)
    {
        if (c != null)
        {
            if (s.E(c, "noRout") > 0) { if (side == "A") o.NoRoutA = true; else o.NoRoutB = true; }
            if (s.E(c, "firstStrike") > 0) { if (side == "A") o.FirstStrikeA = s.E(c, "firstStrike"); else o.FirstStrikeB = s.E(c, "firstStrike"); }
            if (s.E(c, "meteor") > 0) { if (side == "A") o.MeteorA = true; else o.MeteorB = true; }
        }
        if (enemy != null && s.E(enemy, "enemyMorale") > 0)
        {
            if (side == "A") o.MoraleA = (o.MoraleA ?? 0.6) - s.E(enemy, "enemyMorale");
            else o.MoraleB = (o.MoraleB ?? 0.6) - s.E(enemy, "enemyMorale");
        }
        return o;
    }

    /// <summary>Savaşı kaydeder (son 250); ayrıntılı tekrar yalnızca son savaşlarda tutulur (bellek). Savaş id'sini döndürür.</summary>
    public static int RecordBattle(Sim s, Battle b)
    {
        s.W.Battles.Add(b);
        if (s.W.Battles.Count > 250) J.Shift(s.W.Battles);
        // ayrıntılı tekrar yalnızca son savaşlarda tutulur (bellek)
        int old = s.W.Battles.Count - 1 - Combat.REPLAY_KEEP;
        if (old >= 0 && s.W.Battles[old].Replay != null) s.W.Battles[old].Replay = null;
        s.Metric("battles");
        return b.Id;
    }

    /// <summary>Savaş sonrası kahramanların canı/öldürmeleri işlenir: ölenler (diriliş denenir) ölür, sağ kalanlar XP alır.</summary>
    public static void SyncHeroes(Sim s, List<Combatant> cs, double xpBonus = 0)
    {
        foreach (var x in cs)
        {
            if (x.Hero == null) continue;
            var h = x.Hero;
            h.Hp = JsMath.Max(0, JsMath.Min(h.MaxHp, x.Hp));
            h.Kills += x.Kills;
            if (x.Hp <= 0)
            {
                if (Heroes.TryRevive(s, h)) continue;
                h.State = "dead"; h.DeathDay = s.Day; h.Auction = null; h.Goal = null;
                if (h.Contract != null) h.Contract = null;
                var lb = J.At(s.W.Battles, s.W.Battles.Count - 1);
                if (lb != null && lb.Day == s.Day) h.Pos = lb.Tile;
                s.Metric("heroDeath");
                s.Log("death", $"{s.HeroTitle(h)} öldü.", tile: h.Pos, civ: h.Civ >= 0 ? h.Civ : (int?)null, major: true, cause: $"{J.S(h.Kills)} düşman devirmişti");
            }
            else Heroes.GainXp(s, h, x.Kills * 50 + xpBonus);
        }
    }

    private static void ApplyDefLosses(Sim s, Settlement st, List<Combatant> cs, double cap = 1)
    {
        var c = s.W.Civs[st.Civ];
        double deadS = J.Filter(cs, x => (x.Kind == "soldier" || x.Kind == "unique") && x.Hp <= 0).Count;
        // sivillerin çoğu kaçıp saklanır: milis kaybı nüfusun belli bir oranıyla sınırlı
        double deadM = JsMath.Min(J.Filter(cs, x => x.Kind == "militia" && x.Hp <= 0).Count, JsMath.Max(1, Math.Floor(s.Pop(st) * cap)));
        double back = Math.Floor(deadS * JsMath.Min(0.8, s.E(c, "healBack")));
        back += Gear.HealWounded(s, c, deadS - back);
        s.RemovePop(st, deadS - back + deadM);
        st.Soldiers = JsMath.Max(0, st.Soldiers - deadS + back);
        SyncHeroes(s, cs);
    }

    /// <summary>Yılda bir kaybedilen savaşı yeniden atma (Talihin Dokunuşu / Kader Dokuması)</summary>
    private static Battle WithLuck(Sim s, Civ c, Func<Battle> fight, Func<Battle, bool> lost)
    {
        var b = fight();
        if (c != null && lost(b) && s.E(c, "luck") > 0 && c.Yearly.Get("luck") != s.Year)
        {
            c.Yearly.Set("luck", s.Year);
            var b2 = fight();
            J.Unshift(b2.Lines, new BattleLine { T = $"Talih {c.Name} tarafına döndü!", Crit = true });
            b = b2;
        }
        return b;
    }

    // ------------------------------------------------------------ hareket
    /// <summary>Günlük hareket: ajanlar yolda ilerler, baskın grupları kervan kovalar, yol sonunda varış işlenir.</summary>
    public static void AgentsTick(Sim s)
    {
        var w = s.W;
        // JS for...of: the iterator keeps the array it started with and re-reads its length (agents pushed meanwhile are visited)
        var list = w.Agents;
        for (int ai = 0; ai < list.Count; ai++)
        {
            var a = list[ai];
            if (J.T(a.Dead)) continue;
            // handa konaklayan kervan sabah yola çıkar
            if (a.RestUntil != null) { if (s.Day < a.RestUntil.Value) continue; a.RestUntil = null; }
            if (a.Step < a.Path.Count - 1)
            {
                int next = a.Path[a.Step + 1];
                if (J.T(w.Tiles[next].Sea)) a.Progress += Sea.ShipSpeed(s, a);
                else
                {
                    double cost = s.MoveCost(next);
                    if (a.Civ >= 0 && s.E(w.Civs[a.Civ], "forestMove") > 0 && (w.Tiles[next].Terrain == "forest" || w.Tiles[next].Terrain == "oldforest")) cost = 0.5;
                    a.Progress += a.Speed / cost;
                }
                while (a.Progress >= 1 && a.Step < a.Path.Count - 1)
                {
                    a.Progress -= 1; a.Step++;
                    int here = a.Path[a.Step], prev = a.Path[a.Step - 1];
                    if (J.T(w.Tiles[prev].Sea) && !J.T(w.Tiles[here].Sea)) Sea.Disembark(s, a, prev, here);
                    if (a.Kind == "hero" || a.Kind == "party") foreach (var hid in a.Heroes ?? new List<int>()) s.Hero(hid).Pos = here;
                    if (a.Kind == "scout") ScoutSight(s, a);
                    if (a.Kind == "ship" && a.Purpose != null && a.Purpose.StartsWith("explore", StringComparison.Ordinal)) Sea.ExploreSight(s, a);
                    if (a.Kind == "army" && (a.Purpose == "war" || a.Purpose == "plunder") && !J.T(a.Returning)) PillageTile(s, a, here);
                    if (a.Kind == "caravan" && InnLife.CaravanPassInn(s, a)) { a.Progress = 0; break; }
                }
            }
            if (a.Kind == "raid" && a.Purpose == "caravan" && !J.T(a.Returning))
            {
                var cv = J.Find(w.Agents, x => x.Id == a.To && !J.T(x.Dead));
                if (cv == null) { RaidReturn(s, a); continue; }
                int here = TileOf(a), cvt = TileOf(cv);
                if (s.G.Dist(here, cvt) <= 1) { Ambush(s, a, cv); continue; }
                if (s.Day % 3 == 0) { var p = s.Path(here, cv.Path[Math.Min(cv.Path.Count - 1, cv.Step + 2)]); if (p != null) { a.Path = p; a.Step = 0; a.Progress = 0; } }
                if (a.Chase == null) a.Chase = s.Day;
                if (s.Day - a.Chase > 25) RaidReturn(s, a);
                continue;
            }
            if (a.Step >= a.Path.Count - 1 && Arrive(s, a)) a.Dead = true;
        }
        w.Agents = J.Filter(w.Agents, a => !J.T(a.Dead));
    }

    /// <summary>Ajanın bulunduğu karo: path[min(step, path.length - 1)].</summary>
    public static int TileOf(Agent a) => a.Path[Math.Min(a.Step, a.Path.Count - 1)];

    private static void Reroute(Sim s, Agent a, int to) { var p = s.Path(TileOf(a), to); a.Path = p ?? new List<int> { TileOf(a) }; a.Step = 0; a.Progress = 0; }

    private static readonly string[] SCOUT_FINDS = { "tin", "gold", "silver", "iron", "mana", "mithril", "horses", "salt", "copper" };
    private static readonly string[] SCOUT_MAJOR = { "tin", "gold", "iron" };

    /// <summary>Kâşif gözü: yakındaki yataklar, canavar kampları ve yeni medeniyetlerle ilk temas.</summary>
    public static void ScoutSight(Sim s, Agent a)
    {
        var c = s.W.Civs[a.Civ];
        int here = TileOf(a);
        foreach (var d in s.W.Deposits)
        {
            if (d.KnownBy.Contains(c.Id) || !J.Some(d.Tiles, t => s.G.Dist(t, here) <= 4)) continue;
            d.KnownBy.Add(c.Id);
            if (s.DepositVisible(c, d) && Array.IndexOf(SCOUT_FINDS, d.Kind) >= 0)
                s.Log("discover", $"{c.Name} kâşifleri bir {(d.Kind == "silver" ? "gümüş damarı" : d.Kind == "horses" ? "yaban at sürüsü" : TECH_RES.TryGetValue(d.Kind, out var tr) ? tr : d.Kind)} buldu.", civ: c.Id, tile: d.Tiles[0], major: Array.IndexOf(SCOUT_MAJOR, d.Kind) >= 0);
        }
        foreach (var cp in s.W.Camps) if (cp.Alive && s.G.Dist(here, cp.Tile) <= 4 && !J.T(c.Yearly.Get("saw" + J.S(cp.Id))))
            {
                c.Yearly.Set("saw" + J.S(cp.Id), 1); c.Threat += 0.1;
                s.Log("discover", $"{c.Name} kâşifleri {Tr.Ek(cp.Name, "i")} gördü: {J.S(cp.Count)} {J.TrLower(Monsters.MonsterName(cp.Kind, false))}.", civ: c.Id, tile: cp.Tile);
            }
        foreach (var o in s.W.Civs)
        {
            if (o.Id == c.Id || !o.Alive || s.Rel(c.Id, o.Id).Contact) continue;
            if (J.Some(s.CivSettlements(o), x => s.G.Dist(x.Tile, here) <= 5)) MakeContact(s, c, o, $"{c.Name} kâşifleri {s.Capital(o)?.Name ?? "yerleşimlerine"} ulaştı");
        }
    }
    private static readonly Dictionary<string, string> TECH_RES = new() { ["tin"] = "kalay damarı", ["gold"] = "altın damarı", ["iron"] = "demir damarı", ["mana"] = "mana kristali yatağı", ["mithril"] = "mithril damarı", ["salt"] = "tuz yatağı", ["copper"] = "bakır damarı" };

    /// <summary>İki medeniyet ilk kez karşılaşır (karşılıklı temas + "İlk karşılaşma merakı").</summary>
    public static void MakeContact(Sim s, Civ a, Civ b, string why)
    {
        s.Rel(a.Id, b.Id).Contact = true;
        s.Rel(b.Id, a.Id).Contact = true;
        s.SetMod(a.Id, b.Id, "first", "İlk karşılaşma merakı", 10, 0.02);
        s.Log("contact", $"{a.Name} ile {b.Name} ilk kez karşılaştı.", civ: a.Id, cause: why, major: true);
        s.Metric("contact");
    }

    private static bool Arrive(Sim s, Agent a)
    {
        var w = s.W;
        int tile = TileOf(a);
        switch (a.Kind)
        {
            case "scout":
                {
                    if (!J.T(a.Returning)) { var home = s.Capital(w.Civs[a.Civ]); if (home != null) { Reroute(s, a, home.Tile); a.Returning = true; return false; } }
                    return true;
                }
            case "settlers":
                {
                    if (a.Purpose == "homeless")
                    {
                        var to = s.Settlement(a.To); var home = s.Settlement(a.From);
                        var dest = to != null && to.Alive ? to : home != null && home.Alive ? home : null;
                        if (dest != null) { s.MergePop(dest, a.Pop); s.Log("migration", $"{J.S(s.PopSize(a.Pop))} evsiz {Tr.Ek(dest.Name, "da")} yeni evlerine yerleşti.", civ: dest.Civ, tile: dest.Tile); }
                        return true;
                    }
                    if (a.Purpose == "refugee")
                    {
                        var to = s.Settlement(a.To);
                        var home = s.Settlement(a.From);
                        var dest = to != null && to.Alive ? to : home != null && home.Alive ? home : null;
                        if (dest != null)
                        {
                            s.MergePop(dest, a.Pop);
                            if (dest.Civ != a.Civ) s.AddMod(dest.Civ, a.Civ, "refugees", "Açlara kapı açtık", 4, 12, 0.01, false);
                            s.Log("migration", $"{J.S(s.PopSize(a.Pop))} aç göçmen {Tr.Ek(dest.Name, "a")} sığındı.", civ: dest.Civ, tile: dest.Tile);
                        }
                        return true;
                    }
                    return FoundSettlement(s, a);
                }
            case "hero":
                {
                    var hs = a.Heroes ?? new List<int>();
                    for (int i = 0; i < hs.Count; i++)
                    {
                        var h = s.Hero(hs[i]); h.Pos = tile;
                        if (h.State == "dead") continue;
                        if (a.Purpose == "goal") { Will.ArriveGoal(s, h); continue; }
                        if (a.Purpose == "tavern")
                        {
                            var inn = Will.InnAt(s, tile);
                            var t = inn != null ? null : J.Find(w.Settlements, x => x.Alive && x.Tile == tile && J.T(x.Civics.Get("tavern")));
                            if (inn != null || t != null) { h.State = "tavern"; h.Tavern = inn != null ? inn.Id : t.Id; h.IdleSince = s.Day; } else if (h.Civ == -1) Will.ReturnToBase(s, h); else h.State = "gone";
                        }
                        else h.State = "home";
                    }
                    return true;
                }
            case "caravan": return CaravanArrive(s, a);
            case "keeper": return InnLife.KeeperArrive(s, a);
            case "traveler": return InnLife.TravelerArrive(s, a);
            case "supply": return InnLife.SupplyArrive(s, a);
            case "ship": return a.Purpose == "explore" ? Sea.ExploreTurn(s, a) : Sea.IsFleet(a) ? Sea.FleetArrive(s, a) : true;
            case "raid":
                {
                    if (J.T(a.Returning)) { RaidHome(s, a); return true; }
                    if (a.Purpose == "prey") return false; // korsan avını kovalıyor (sea.ts)
                    if (a.Purpose == "ext") { RaidExt(s, a, a.TargetTile ?? tile); return !J.T(a.Returning); }
                    if (a.Purpose == "inn") { Inns.InnMonsterRaid(s, a); return !J.T(a.Returning); }
                    var st = s.Settlement(a.To);
                    if (st == null || !st.Alive) { RaidReturn(s, a); return false; }
                    RaidSettlement(s, a, st);
                    return !J.T(a.Returning);
                }
            case "party":
                {
                    if (J.T(a.Returning)) { PartyHome(s, a); return true; }
                    var cp = J.Find(w.Camps, x => x.Id == a.To);
                    if (cp == null || !cp.Alive) { a.Muster = null; PartyReturn(s, a); return false; }
                    Engage(s, a); return false;
                }
            case "army":
                {
                    if (J.T(a.Returning)) { ArmyHome(s, a); return true; }
                    if (a.Purpose == "innraid") { Inns.InnRaidArrive(s, a); return false; }
                    if (a.Purpose == "expedition")
                    {
                        var cp = J.Find(w.Camps, x => x.Id == a.To);
                        if (cp == null || !cp.Alive) { a.Muster = null; ArmyReturn(s, a); return false; }
                        Engage(s, a); return false;
                    }
                    var st = s.Settlement(a.To);
                    // hedef el değiştirdiyse ya da barış yapıldıysa ordu geri döner
                    if (st == null || !st.Alive || st.Civ == a.Civ || (a.Purpose == "war" && !s.AtWar(a.Civ, st.Civ))) { a.Muster = null; ArmyReturn(s, a); return false; }
                    if (a.Purpose == "plunder") Plunder(s, a, st); else Engage(s, a);
                    return false;
                }
        }
        return true;
    }

    private static readonly string[] TOWN_A = { "Kara", "Ak", "Yeşil", "Demir", "Taş", "Kurt", "Göl", "Çam", "Kızıl", "Gök", "Yel", "Ay", "Gün", "Kuzey", "Sarı", "Boz", "Koca", "Ulu", "Yıldız", "Tuz", "Bal", "Söğüt", "Kartal", "Ceylan", "Pınar", "Meşe", "Duman", "Sis", "Karlı", "Alaca" };
    private static readonly string[] TOWN_B = { "köy", "yurt", "tepe", "ova", "pınar", "kale", "dere", "bük", "kaya", "burç", "geçit", "yayla", "çayır", "koru", "köprü", "kent", "hisar", "oba", "yazı", "sırt" };
    /// <summary>yeni yerleşim adı: karodan türetilen, tekrarsız Türkçe yer adı</summary>
    private static string TownName(Sim s, int tile, HashSet<string> used)
    {
        for (int k = 0; k < 60; k++)
        {
            string a = TOWN_A[(tile * 7 + k * 13) % TOWN_A.Length], b = TOWN_B[(tile * 3 + k * 7) % TOWN_B.Length];
            string n = a + b;
            if (!used.Contains(n)) return n;
        }
        return $"Yeni {TOWN_A[tile % TOWN_A.Length]}{TOWN_B[s.W.Settlements.Count % TOWN_B.Length]}";
    }
    private static bool FoundSettlement(Sim s, Agent a)
    {
        var w = s.W;
        var c = w.Civs[a.Civ];
        int t = a.TargetTile.Value;
        bool free = w.Tiles[t].Owner < 0 && w.Tiles[t].Camp == null && !J.Some(w.Settlements, x => x.Alive && s.G.Dist(x.Tile, t) < 4);
        if (!free || !c.Alive)
        {
            var home = s.Settlement(a.From);
            if (home != null && home.Alive) s.MergePop(home, a.Pop);
            s.Log("settle", $"{c.Name} öncüleri hedefledikleri toprağı dolu buldu ve geri döndü.", civ: c.Id, tile: t);
            return true;
        }
        var used = new HashSet<string>(J.Map(w.Settlements, x => x.Name));
        string name = J.Find(D.CLASSES[c.Cls].Towns, n => !used.Contains(n)) ?? TownName(s, t, used);
        var st = WorldGen.MakeSettlement(s.Id(), c.Id, name, t, a.Pop ?? new JsObj<double>(), s.Day);
        if (a.Landing != null) st.Overseas = true;
        w.Settlements.Add(st);
        s.UpdateTerritory();
        s.Discover();
        s.Metric("settle");
        s.Metric("settle_" + J.S(c.Id));
        if (J.T(st.Overseas))
        {
            s.Metric("seaColony");
            var isl = J.T(w.Tiles[t].Isle) ? (w.Isles == null ? null : J.Find(w.Isles, x => x.Id == w.Tiles[t].Isle)) : null;
            s.Log("settle", $"{c.Name}{(isl != null ? $", {Tr.Ek(isl.Name, "da")}" : "")} denizaşırı koloni {Tr.Ek(name, "i")} kurdu.", civ: c.Id, tile: t, cause: a.Purpose, major: true);
        }
        else s.Log("settle", $"{c.Name} yeni yerleşimi {Tr.Ek(name, "i")} kurdu.", civ: c.Id, tile: t, cause: a.Purpose, major: true);
        return true;
    }

    // ------------------------------------------------------------ kervanlar ve ticaret
    private static readonly List<string> SUPPLY_GOODS = new() { "wood", "grain", "tools" };

    /// <summary>Kalkış günü gelen ticaret / antlaşma / ikmal yollarından yüklü kervan çıkarır.</summary>
    public static void RoutesTick(Sim s)
    {
        var routes = s.W.Routes;
        for (int ri = 0; ri < routes.Count; ri++)
        {
            var r = routes[ri];
            if (!r.Alive || s.Day < r.NextDepart) continue;
            var A = s.Settlement(r.A); var B = s.Settlement(r.B);
            if (A == null || B == null || !A.Alive || !B.Alive) { r.Alive = false; continue; }
            bool reverse = r.Kind == "trade" && r.Trips % 2 == 1;
            var src = reverse ? B : A; var dst = reverse ? A : B;
            var cs = s.W.Civs[src.Civ]; var cd = s.W.Civs[dst.Civ];
            if (cs.Id != cd.Id && s.AtWar(cs.Id, cd.Id) && !(s.E(cs, "blackMarket") > 0)) { r.NextDepart = s.Day + 30; continue; }
            // deniz yolu: kalkış limanında boş gemi yoksa sıra öbür uca geçer
            if (J.T(r.Sea) && (!J.T(src.Civics.Get("shipyard")) || src.Port == null || Sea.FreeHulls(s, src) < 1)) { r.Trips++; r.NextDepart = s.Day + 12; continue; }
            r.NextDepart = s.Day + (r.Kind == "treaty" ? 35 : 40);
            var cargo = new JsObj<double>();
            if (r.Kind == "treaty")
            {
                string g = r.Good;
                if (s.Rel(cs.Id, cd.Id).Treaty != g && s.Rel(cd.Id, cs.Id).Treaty != g) { r.Alive = false; continue; }
                double q = JsMath.Min(10, Math.Floor(s.St(cs, g) * 0.4));
                if (q < 2) { r.Trips++; continue; }
                s.Add(cs, g, -q); cargo = new JsObj<double> { [g] = q };
            }
            else if (cs.Id == cd.Id)
            {
                // ikmal: koloniye kereste, tahıl ve alet (stok medeniyet çapında; yük görünür taşınır)
                string g = J.Find(SUPPLY_GOODS, x => s.St(cs, x) >= 12);
                if (!J.T(g)) { r.Trips++; continue; }
                double q = Math.Floor(JsMath.Min(10, s.St(cs, g) * 0.1));
                s.Add(cs, g, -q); cargo = new JsObj<double> { [g] = q };
            }
            else
            {
                string best = null; double bv = 0;
                foreach (var g in D.GOOD_IDS)
                {
                    if (g == "gold") continue;
                    double have = s.St(cs, g);
                    if (have < 6) continue;
                    double gain = s.Price(cd, g) - s.Price(cs, g) * 1.15;
                    double v = gain * JsMath.Min(12, have * 0.25);
                    if (v > bv) { bv = v; best = g; }
                }
                if (!J.T(best)) { r.Trips++; continue; }
                double hold = J.T(r.Sea) ? (s.Has(cs, "seatrade") ? 24 : 18) : 12;
                double q = Math.Floor(JsMath.Min(hold, s.St(cs, best) * 0.25));
                s.Add(cs, best, -q); cargo = new JsObj<double> { [best] = q };
            }
            double guards = s.Has(cs, "training") ? 3 : 2;
            List<int> @base;
            if (reverse) { @base = new List<int>(r.Path); @base.Reverse(); } else @base = r.Path;
            var path = J.T(r.Sea) ? @base : InnLife.InnDetour(s, @base); // deniz yolu hana uğramaz
            bool horse = Gear.TakeHorse(s, cs);
            double speed = horse ? 0.9 : 0.65;
            s.W.Agents.Add(new Agent { Id = s.Id(), Kind = "caravan", Civ = cs.Id, Path = path, Step = 0, Progress = 0, Speed = speed, Cargo = cargo, Horse = horse, Troops = guards, From = src.Id, To = dst.Id, Route = r.Id, Hull = J.T(r.Sea) ? src.Id : (int?)null });
        }
    }

    private static bool CaravanArrive(Sim s, Agent a)
    {
        var r = J.Find(s.W.Routes, x => x.Id == a.Route);
        var dst = s.Settlement(a.To); var src = s.Settlement(a.From);
        if (dst == null || src == null || r == null) return true;
        var cs = s.W.Civs[src.Civ]; var cd = s.W.Civs[dst.Civ];
        double value = 0;
        if (a.Cargo != null)
            foreach (var kv in a.Cargo)
            {
                string g = kv.Key;
                double q = a.Cargo.Get(g) ?? 0;
                s.Add(cd, g, q);
                value += q * (s.Price(cs, g) + s.Price(cd, g)) / 2;
            }
        r.Trips++;
        if (J.T(a.Horse)) s.Add(cs, "horses", 1); // kervan atı döner
        if (cs.Id == cd.Id) { s.Metric("supplyTrips"); return true; } // iç ikmal: kazanç yok
        double pay = JsMath.Min(s.St(cd, "gold"), value);
        s.Add(cd, "gold", -pay);
        s.Add(cs, "gold", pay + 3 * (1 + s.E(cs, "tradeGold")));
        s.Add(cd, "gold", 2 * (1 + s.E(cd, "tradeGold")));
        cs.Stats.Traded++; cd.Stats.Traded++;
        if (cs.Id != cd.Id) s.AddMod(cs.Id, cd.Id, "trade", "Süren ticaret", 3, 30, 0.015);
        s.Metric("caravanTrips");
        if (J.T(r.Sea)) { s.Metric("seaTrips"); s.Add(cs, "gold", 2 * (1 + s.E(cs, "tradeGold"))); }
        if (r.Trips == 4 && !J.T(r.Sea))
        {
            foreach (int t in r.Path) if (!J.T(s.W.Tiles[t].Road) && !J.T(s.W.Tiles[t].Sea)) s.W.Tiles[t].Road = 1;
            s.ClearPaths();
            s.Log("trade", $"{src.Name}–{dst.Name} kervan yolu çiğnenip gerçek bir yola dönüştü.", civ: cs.Id, tile: dst.Tile);
        }
        return true;
    }

    /// <summary>Her 4 günde bir: yol tekniğini bilen medeniyet başkentten yerleşimlerine birer karo yol döşer.</summary>
    public static void RoadsTick(Sim s)
    {
        if (s.Day % 4 != 0) return;
        foreach (var c in s.W.Civs)
        {
            if (!c.Alive || !s.Has(c, "roads")) continue;
            var cap = s.Capital(c);
            if (cap == null) continue;
            foreach (var st in s.CivSettlements(c))
            {
                if (st.Id == cap.Id) continue;
                var p = s.Path(cap.Tile, st.Tile);
                if (p == null) continue;
                // p.find(...) over tile numbers: "not found" must be told apart from tile 0
                int ni = J.FindIndex(p, t => !J.T(s.W.Tiles[t].Road));
                if (ni < 0 || s.St(c, "wood") < 2 || s.St(c, "stone") < 1) continue;
                int next = p[ni];
                s.Add(c, "wood", -1.5); s.Add(c, "stone", -0.8);
                s.W.Tiles[next].Road = 1;
                s.ClearPaths();
                if (!J.Some(p, t => !J.T(s.W.Tiles[t].Road))) s.Log("build", $"{cap.Name} ile {st.Name} arasındaki yol tamamlandı.", civ: c.Id, tile: st.Tile);
            }
        }
    }

    // ------------------------------------------------------------ canavar baskınları
    /// <summary>Baskın grubu kampına döner (korsanlar deniz yoluyla); kamp yoksa olduğu yerde kalır.</summary>
    public static void RaidReturn(Sim s, Agent a)
    {
        var cp = J.Find(s.W.Camps, x => x.Id == a.From);
        a.Returning = true; a.Purpose = "return";
        if (cp == null || !cp.Alive) { a.Path = new List<int> { TileOf(a) }; a.Step = 0; return; }
        if (a.Monster == "pirate") { var p = Sea.PirateReturnPath(s, a, cp.Tile); if (p != null) { a.Path = p; a.Step = 0; a.Progress = 0; return; } }
        Reroute(s, a, cp.Tile);
    }
    private static void RaidHome(Sim s, Agent a)
    {
        if (a.Monster == "pirate") Sea.PirateSmuggle(s, a);
        var cp = J.Find(s.W.Camps, x => x.Id == a.From);
        if (cp != null && cp.Alive) { cp.Count = JsMath.Min(18, cp.Count + (a.Troops ?? 0)); if (J.T(a.Boss)) cp.Boss = true; cp.Loot += a.Loot ?? 0; }
    }

    private static void RaidSettlement(Sim s, Agent a, Settlement st)
    {
        var c = s.W.Civs[st.Civ];
        string kind = a.Monster ?? "goblin";
        var cp = J.Find(s.W.Camps, x => x.Id == a.From);
        var mons = Monsters.MonsterSide(kind, a.Troops ?? 0, J.T(a.Boss), "B");
        // TS hangs _def / _m on the battle; withLuck always keeps the LAST fight, so the last locals are the same arrays
        List<Combatant> lastDef = null, lastM = null;
        Battle Fight()
        {
            var def0 = Defenders(s, st, "A", true);
            var mcopy = J.Map(mons, m => { var mc0 = m.Clone(); mc0.Uses = new JsObj<double>(); mc0.Kills = 0; return mc0; });
            var b0 = Combat.ResolveBattle(s.Rng, def0, mcopy, CivOpts(s, c, "A", new BattleOpts { Id = s.Id(), Day = s.Day, Tile = st.Tile, Title = $"{st.Name} baskını", SideA = $"{st.Name} savunucuları", SideB = Monsters.MonsterName(kind), MoraleA = 0.7, MoraleB = kind == "hobgoblin" ? 0.6 : 0.45, TimeoutWinner = "A", CivA = c.Id }));
            lastDef = def0;
            lastM = mcopy;
            return b0;
        }
        var b = WithLuck(s, c, Fight, x => x.Winner == "B");
        var def = lastDef; var mc = lastM;
        RecordBattle(s, b);
        ApplyDefLosses(s, st, def, 0.15);
        a.Troops = J.Filter(mc, x => x.Kind == "monster" && x.Hp > 0).Count;
        bool bossAlive = J.Some(mc, x => J.T(x.Boss) && x.Hp > 0);
        if (J.T(a.Boss) && !bossAlive) { if (cp != null) cp.HadBoss = false; s.Log("raid", $"{(kind == "goblin" ? "Goblin şefi" : kind == "pirate" ? "Korsan kaptanı" : "Hobgoblin yüzbaşısı")} {Tr.Ek(st.Name, "da")} öldürüldü!", civ: c.Id, tile: st.Tile, battle: b.Id, major: true); }
        a.Boss = bossAlive;
        c.LastRaidedDay = s.Day;
        if (kind == "pirate") c.Yearly.Set("pirateHit", s.Year);
        s.Metric("raids");
        if (b.Winner == "B")
        {
            double food = Math.Floor(s.St(c, "grain") * 0.25), gold = Math.Floor(s.St(c, "gold") * 0.3);
            s.Add(c, "grain", -food); s.Add(c, "gold", -gold);
            a.Loot = food + gold;
            double burnt = s.Rng.Int(1, 3);
            st.BurnedHouses = (st.BurnedHouses ?? 0) + burnt; st.BurnedAt = s.Day;
            c.Threat += 0.45;
            c.Stats.BattlesLost++;
            if (cp != null) Will.OnHomeBurned(s, st, cp);
            s.Log("raid", $"{Monsters.MonsterName(kind)} {Tr.Ek(st.Name, "i")} yağmaladı: {J.S(b.LossesA)} ölü, {J.S(food)} tahıl ve {J.S(gold)} altın kayıp.", civ: c.Id, tile: st.Tile, battle: b.Id, cause: $"{cp?.Name ?? "Kamp"} yakınlarda büyüyordu", major: true);
        }
        else
        {
            c.Threat += 0.2;
            c.Stats.BattlesWon++;
            s.Log("raid", $"{st.Name} {J.TrLower(Monsters.MonsterName(kind))} baskınını püskürttü ({J.S(b.LossesB)} düşman öldü, {J.S(b.LossesA)} kayıp).", civ: c.Id, tile: st.Tile, battle: b.Id, cause: $"{cp?.Name ?? "Kamp"} yağma arıyor");
        }
        if ((a.Troops ?? 0) > 0 || bossAlive) RaidReturn(s, a); else a.Returning = false;
    }

    /// <summary>çıkarma yapısına canavar baskını: işçiler savaşır; kaybederlerse yapı yanar</summary>
    private static void RaidExt(Sim s, Agent a, int tile)
    {
        var w = s.W; var t = w.Tiles[tile];
        var st = t.Owner >= 0 ? s.Settlement(t.Owner) : null;
        if (t.Ext == null || !s.ExtWorking(t) || st == null || !st.Alive) { RaidReturn(s, a); return; }
        var c = w.Civs[st.Civ];
        string kind = a.Monster ?? "goblin";
        string extName = J.TrLower(D.EXTRACTS[t.Ext.Kind].Names[t.Ext.Level - 1]);
        var def = new List<Combatant>();
        double workers = JsMath.Max(1, t.Ext.Workers);
        for (int i = 0; i < workers; i++) def.Add(Combat.Unit(D.UNITS["militia"], "A", "militia", D.RACES[c.Race].Hp));
        double near = s.G.Dist(tile, st.Tile) <= 2 ? JsMath.Min(st.Soldiers, 3) : 0;
        def.AddRange(CivTroops(s, c, near, "A"));
        var mons = Monsters.MonsterSide(kind, a.Troops ?? 0, J.T(a.Boss), "B");
        var b = Combat.ResolveBattle(s.Rng, def, mons, CivOpts(s, c, "A", new BattleOpts { Id = s.Id(), Day = s.Day, Tile = tile, Title = $"{Tr.Ek(st.Name, "in")} {extName} baskını", SideA = $"{st.Name} işçileri", SideB = Monsters.MonsterName(kind), MoraleA = 0.45, MoraleB = 0.5, TimeoutWinner = "A", CivA = c.Id }));
        RecordBattle(s, b);
        double deadW = J.Filter(def, x => x.Kind == "militia" && x.Hp <= 0).Count;
        double deadS = J.Filter(def, x => x.Kind != "militia" && x.Hp <= 0).Count - Gear.HealWounded(s, c, J.Filter(def, x => x.Kind != "militia" && x.Hp <= 0).Count);
        if (J.T(deadW)) s.RemovePop(st, deadW);
        if (J.T(deadS)) { st.Soldiers = JsMath.Max(0, st.Soldiers - deadS); s.RemovePop(st, deadS); }
        a.Troops = J.Filter(mons, x => x.Kind == "monster" && x.Hp > 0).Count;
        a.Boss = J.Some(mons, x => J.T(x.Boss) && x.Hp > 0);
        c.LastRaidedDay = s.Day;
        if (kind == "pirate") c.Yearly.Set("pirateHit", s.Year);
        s.Metric("raids");
        if (b.Winner == "B")
        {
            t.Ext.Burned = s.Day + s.Rng.Int(40, 90); t.Ext.BurnedAt = s.Day; t.Ext.Workers = 0;
            string g = EXT_GOOD.TryGetValue(t.Ext.Kind, out var eg) ? eg : null;
            double q = J.T(g) ? Math.Floor(JsMath.Min(s.St(c, g) * 0.12, 30)) : 0;
            if (J.T(g) && q > 0) s.Add(c, g, -q);
            a.Loot = (a.Loot ?? 0) + q;
            c.Threat += 0.3;
            c.Stats.BattlesLost++;
            s.Metric("extBurned");
            s.Log("raid", $"{Monsters.MonsterName(kind, false)} baskını: {Tr.Ek(st.Name, "in")} {D.ExtPoss(t.Ext.Kind, t.Ext.Level)} yandı{(J.T(deadW + deadS) ? $", {J.S(deadW + deadS)} kişi öldü" : "")}{(J.T(q) ? $", {J.S(q)} {J.TrLower(D.GOODS[g].Name)} çalındı" : "")}.", civ: c.Id, tile: tile, battle: b.Id, major: true, cause: "Korumasız işçiler; yeniden kurulana dek üretim durdu");
        }
        else
        {
            c.Threat += 0.1;
            c.Stats.BattlesWon++;
            s.Log("raid", $"{st.Name}: {extName} işçileri {J.TrLower(Monsters.MonsterName(kind, false))} baskınını püskürttü.", civ: c.Id, tile: tile, battle: b.Id, cause: J.T(deadW) ? $"{J.S(deadW)} işçi hayatını kaybetti" : "Kayıp yok");
        }
        RaidReturn(s, a);
    }
    private static readonly Dictionary<string, string> EXT_GOOD = new() { ["farm"] = "grain", ["dock"] = "fish", ["hunt"] = "meat", ["lumber"] = "wood", ["quarry"] = "stone", ["mine"] = "iron", ["pasture"] = "horses", ["herbalist"] = "herbs", ["claypit"] = "bricks", ["crystal"] = "mana" };

    /// <summary>savaş ve yağma orduları geçtikleri düşman topraklarındaki yapıları yakar</summary>
    public static void PillageTile(Sim s, Agent a, int tile)
    {
        var w = s.W; var t = w.Tiles[tile];
        if (t.Ext == null || !s.ExtWorking(t) || t.Owner < 0 || a.Civ < 0) return;
        var st = s.Settlement(t.Owner);
        if (st == null || st.Civ == a.Civ || !s.AtWar(a.Civ, st.Civ) && a.Purpose != "plunder") return;
        if (!s.Rng.Chance(0.35)) return;
        _ = J.TrLower(D.EXTRACTS[t.Ext.Kind].Names[t.Ext.Level - 1]); // TS computes extName here but never uses it
        double dead = JsMath.Min(t.Ext.Workers, s.Rng.Int(0, 2));
        if (J.T(dead)) s.RemovePop(st, dead);
        t.Ext.Burned = s.Day + s.Rng.Int(40, 80); t.Ext.BurnedAt = s.Day; t.Ext.Workers = 0;
        s.Metric("extBurned");
        var att = w.Civs[a.Civ];
        s.Log("war", $"{att.Name} askerleri geçerken {Tr.Ek(st.Name, "in")} {D.ExtPoss(t.Ext.Kind, t.Ext.Level)} yakıldı{(J.T(dead) ? $", {J.S(dead)} kişi öldü" : "")}.", civ: att.Id, tile: tile, cause: "Savaşın bedelini köylüler öder");
    }

    /// <summary>JS template of a possibly undefined string: <c>`${x}`</c> prints "undefined".</summary>
    private static string Undef(string x) => x ?? "undefined";

    private static void Ambush(Sim s, Agent a, Agent target)
    {
        string kind = a.Monster ?? "goblin";
        var c = target.Civ >= 0 ? s.W.Civs[target.Civ] : null;
        if (c != null && target.Kind == "caravan" && s.E(c, "raidEvade") > 0 && s.Rng.Chance(s.E(c, "raidEvade")))
        {
            s.Log("raid", $"{c.Name} kervanı gizli yollardan geçerek {J.TrLower(Monsters.MonsterName(kind))} pususunu atlattı.", civ: c.Id, tile: TileOf(target));
            RaidReturn(s, a); return;
        }
        var mons = Monsters.MonsterSide(kind, a.Troops ?? 0, J.T(a.Boss), "B");
        var side = new List<Combatant>();
        if (target.Kind == "caravan" && c != null) side.AddRange(CivTroops(s, c, target.Troops ?? 2, "A"));
        foreach (var hid in target.Heroes ?? new List<int>()) { var h = s.Hero(hid); if (h.State != "dead") side.Add(Combat.HeroCombatant(h, "A")); }
        if (target.Kind == "scout" && c != null) side.Add(Combat.Unit(D.UNITS["militia"], "A", "militia"));
        if (side.Count == 0) { RaidReturn(s, a); return; }
        int tile = TileOf(target);
        string name = target.Kind == "caravan" ? $"{Undef(c?.Name)} kervanı" : target.Kind == "scout" ? $"{Undef(c?.Name)} kâşifleri" : string.Join(", ", J.Map(target.Heroes ?? new List<int>(), hid2 => s.Hero(hid2).Name));
        var b = Combat.ResolveBattle(s.Rng, side, mons, CivOpts(s, c, "A", new BattleOpts { Id = s.Id(), Day = s.Day, Tile = tile, Title = $"{(kind == "bugbear" ? "Bugbear" : "Yol")} pususu", SideA = name, SideB = Monsters.MonsterName(kind), MoraleA = 0.5, MoraleB = 0.45, CivA = c?.Id }));
        RecordBattle(s, b);
        SyncHeroes(s, side);
        a.Troops = J.Filter(mons, x => x.Kind == "monster" && x.Hp > 0).Count;
        a.Boss = J.Some(mons, x => J.T(x.Boss) && x.Hp > 0);
        s.Metric("raids");
        if (c != null) c.LastRaidedDay = s.Day;
        if (b.Winner == "B")
        {
            target.Dead = true;
            if (target.Kind == "caravan")
            {
                double loot = 0;
                if (target.Cargo != null) foreach (var v in target.Cargo.Values()) loot = loot + v;
                a.Loot = loot;
                var r = J.Find(s.W.Routes, x => x.Id == target.Route);
                if (r != null) r.NextDepart = s.Day + 70;
                if (c != null) c.Threat += 0.35;
            }
            foreach (var hid in target.Heroes ?? new List<int>()) { var h = s.Hero(hid); if (h.State != "dead") { if (h.Civ >= 0) { h.State = "traveling"; Heroes.SendHero(s, h, s.Capital(s.W.Civs[h.Civ])?.Tile ?? h.Pos, "home"); } else { Will.Note(s, h, "yolda pusuya düştü"); Will.ReturnToBase(s, h); } } }
            s.Log("raid", $"{Monsters.MonsterName(kind)} {Tr.Ek(name, "i")} pusuya düşürdü.", civ: c?.Id, tile: tile, battle: b.Id, cause: kind == "bugbear" ? "Bugbearlar yolları sessizce avlıyor" : "Yol kampa yakın geçiyor", major: true);
        }
        else
        {
            s.Log("raid", $"{name} {J.TrLower(Monsters.MonsterName(kind))} pususunu savuşturdu.", civ: c?.Id, tile: tile, battle: b.Id);
        }
        RaidReturn(s, a);
    }

    // ------------------------------------------------------------ ortak saldırı: hedefte toplanma
    /// <summary>kamp baskınında dostların bekleneceği en uzun süre (gün)</summary>
    public const double MUSTER_CAMP = 16;
    /// <summary>kuşatmada müttefik ordunun bekleneceği en uzun süre (gün)</summary>
    public const double MUSTER_CITY = 30;

    /// <summary>aynı hedefe giden grupları eşleştiren anahtar: c&lt;kamp&gt; ya da s&lt;yerleşim&gt; (yoksa null)</summary>
    public static string MusterKey(Agent a)
    {
        if (J.T(a.Returning) || J.T(a.Dead) || a.To == null) return null;
        if (a.Kind == "party") return "c" + J.S(a.To.Value);
        if (a.Kind == "army" && a.Purpose == "expedition") return "c" + J.S(a.To.Value);
        if (a.Kind == "army" && a.Purpose == "war") return "s" + J.S(a.To.Value);
        return null;
    }
    private static List<Hero> LiveHeroes(Sim s, Agent a) => J.Filter(J.Map(a.Heroes ?? new List<int>(), id => s.Hero(id)), h => h.State != "dead");

    /// <summary>iki grup omuz omuza saldırır mı</summary>
    public static bool Friendly(Sim s, Agent a, Agent b)
    {
        var ha = LiveHeroes(s, a); var hb = LiveHeroes(s, b);
        // iyi ve kötü hizalı kahramanlar aynı safta vuruşmaz
        if ((J.Some(ha, x => x.Align == "good") && J.Some(hb, x => x.Align == "evil")) || (J.Some(ha, x => x.Align == "evil") && J.Some(hb, x => x.Align == "good"))) return false;
        if (a.Civ >= 0 && b.Civ >= 0)
        {
            if (a.Civ == b.Civ) return true;
            if (s.AtWar(a.Civ, b.Civ)) return false;
            return s.RelValue(a.Civ, b.Civ) > -10;
        }
        int civ = a.Civ >= 0 ? a.Civ : b.Civ;
        if (civ >= 0) return !J.Some(a.Civ >= 0 ? hb : ha, h => h.Grudge == civ);   // kin tutan kahraman o bayrağın yanına gelmez
        return true;
    }

    /// <summary>Ajan yolunun sonunda mı (step &gt;= path.length - 1).</summary>
    public static bool AtTarget(Agent a) => a.Step >= a.Path.Count - 1;

    /// <summary>hedefe kaç günde varır (yaklaşık)</summary>
    public static double EtaDays(Sim s, Agent a)
    {
        double days = 0;
        for (int i = a.Step + 1; i < a.Path.Count; i++)
        {
            int t = a.Path[i];
            double per = J.T(s.W.Tiles[t].Sea) ? 1 / JsMath.Max(0.1, Sea.ShipSpeed(s, a)) : s.MoveCost(t) / JsMath.Max(0.05, a.Speed);
            days += i == a.Step + 1 ? per * JsMath.Max(0, 1 - a.Progress) : per;
        }
        return days;
    }

    /// <summary>aynı hedefe giden dost gruplar (yoldakiler ve bekleyenler)</summary>
    public static List<Agent> MusterAllies(Sim s, Agent a)
    {
        string key = MusterKey(a);
        if (!J.T(key)) return new List<Agent>();
        return J.Filter(s.W.Agents, b => !ReferenceEquals(b, a) && !J.T(b.Dead) && MusterKey(b) == key && Friendly(s, a, b));
    }

    /// <summary>Grubun görünen adı: "X ordusu/seferi" ya da sağ kahramanların adları ("Macera grubu").</summary>
    public static string BandName(Sim s, Agent a)
    {
        var civ = a.Civ >= 0 ? s.W.Civs[a.Civ] : null;
        if (a.Kind == "army") return civ != null ? $"{civ.Name} {(a.Purpose == "expedition" ? "seferi" : "ordusu")}" : "Ordu";
        var hs = LiveHeroes(s, a);
        return hs.Count > 0 ? string.Join(", ", J.Map(hs, h => h.Name)) : "Macera grubu";
    }
    private static string JoinNames(List<string> xs) => xs.Count <= 1 ? string.Join("", xs) : $"{string.Join(", ", J.Slice(xs, 0, -1))} ve {xs[xs.Count - 1]}";

    /// <summary>hedefe varan grup: yoldaki dostlarını bekler ya da bekleyenlerle birlikte saldırır</summary>
    private static void Engage(Sim s, Agent a)
    {
        string key = MusterKey(a);
        var w = s.W;
        var allies = MusterAllies(s, a);
        var waiting = J.Filter(allies, b => b.Muster != null && AtTarget(b));
        bool camp = key[0] == 'c';
        var cp = camp ? J.Find(w.Camps, x => x.Id == a.To) : null;
        var st = camp ? null : s.Settlement(a.To);
        string tname = camp ? cp.Name : st.Name;
        if (a.Muster == null)
        {
            double until = waiting.Count > 0 ? J.MinOf(waiting, b => b.Muster.Until) : s.Day + (camp ? MUSTER_CAMP : MUSTER_CITY);
            a.Muster = new Muster { Since = s.Day, Until = until };
            var coming0 = J.Filter(allies, b => !AtTarget(b) && EtaDays(s, b) <= until - s.Day);
            if (waiting.Count > 0)
            {
                s.Log(camp ? "quest" : "war", $"{BandName(s, a)}, {Tr.Ek(tname, "da")} bekleyen {JoinNames(J.Map(waiting, b => BandName(s, b)))} ile buluştu.", tile: TileOf(a), civ: a.Civ >= 0 ? a.Civ : (int?)null, cause: coming0.Count > 0 ? $"{J.S(coming0.Count)} grup daha yolda" : "Ortak hücum başlıyor");
            }
            else if (coming0.Count > 0)
            {
                s.Metric("musterWait");
                s.Log(camp ? "quest" : "war", camp
                    ? $"{BandName(s, a)}, {Tr.Ek(tname, "in")} yakınında mola verdi: {JoinNames(J.Map(coming0, b => BandName(s, b)))} gelince birlikte saldıracaklar."
                    : $"{BandName(s, a)} {Tr.Ek(tname, "in")} önünde karargâh kurdu; {JoinNames(J.Map(coming0, b => BandName(s, b)))} gelince hücum edecek.",
                    tile: TileOf(a), civ: a.Civ >= 0 ? a.Civ : (int?)null, major: !camp, cause: $"En çok {J.S(until - s.Day)} gün beklenir");
            }
        }
        var coming = J.Filter(allies, b => !AtTarget(b) && EtaDays(s, b) <= a.Muster.Until - s.Day);
        if (coming.Count > 0 && s.Day < a.Muster.Until) return;   // bekle
        var band = new List<Agent> { a };
        band.AddRange(waiting);
        // ordu grubu öne (sefer etkileri onun medeniyetinden gelir)
        J.Sort(band, (x, y) => (y.Kind == "army" ? 1 : 0) - (x.Kind == "army" ? 1 : 0));
        foreach (var bb in band) bb.Muster = null;
        if (band.Count > 1) s.Metric("jointBattle");
        if (cp != null) FightCamp(s, band, cp);
        else if (st != null) Siege(s, band, st);
    }

    /// <summary>yoldaki bir grubun savaşçıları (karar verirken güç tahmini için); vs: kamp türü (CampKind) ya da null</summary>
    public static List<Combatant> AgentCombatants(Sim s, Agent a, string vs = null)
    {
        var civ = a.Civ >= 0 ? s.W.Civs[a.Civ] : null;
        var cs = J.Map(LiveHeroes(s, a), h => J.T(vs) ? Will.HeroSide(h, "A", vs) : Combat.HeroCombatant(h, "A"));
        if (civ != null) cs.AddRange(CivTroops(s, civ, a.Troops ?? 0, "A"));
        return cs;
    }

    /// <summary>henüz yola çıkmamış bir grup için: aynı hedefe gidip ~aynı zamanda varacak dostlar</summary>
    public static List<Agent> ProbeAllies(Sim s, Agent probe, double myEta, double window)
    {
        return J.Filter(MusterAllies(s, probe), b => Math.Abs((AtTarget(b) ? 0 : EtaDays(s, b)) - myEta) <= window);
    }

    /// <summary>grupların savaşçıları; her savaşçı grubunun dizinini taşır</summary>
    private static (List<Combatant> Side, List<ReplayGroup> Groups, List<List<Combatant>> Per) BandSide(Sim s, List<Agent> band, string vs = null)
    {
        var side = new List<Combatant>(); var groups = new List<ReplayGroup>(); var per = new List<List<Combatant>>();
        for (int i = 0; i < band.Count; i++)
        {
            var a = band[i];
            var heroes = LiveHeroes(s, a);
            var civ = a.Civ >= 0 ? s.W.Civs[a.Civ] : null;
            bool alone = band.Count == 1 && heroes.Count == 1 && !J.T(a.Troops ?? 0);
            var cs = J.Map(heroes, h => J.T(vs) ? Will.HeroSide(h, "A", vs, alone) : Combat.HeroCombatant(h, "A"));
            if (civ != null) cs.AddRange(CivTroops(s, civ, a.Troops ?? 0, "A"));
            foreach (var c in cs) c.Grp = i;
            side.AddRange(cs); per.Add(cs);
            groups.Add(new ReplayGroup { Name = BandName(s, a), Side = "A", Civ = civ?.Id, Kind = a.Kind == "party" ? "party" : a.Purpose });
        }
        return (side, groups, per);
    }

    // ------------------------------------------------------------ kahramanlar ve seferler kamplara karşı
    /// <summary>Grup(lar) kampa saldırır: savaş, ganimet/ödül paylaşımı, ilanlar, dönüş.</summary>
    public static void FightCamp(Sim s, List<Agent> band, Camp cp)
    {
        var w = s.W;
        var lead = band[0];
        var civ = lead.Civ >= 0 ? w.Civs[lead.Civ] : null;
        var (side, groups, per) = BandSide(s, band, cp.Kind);
        bool hadBoss = cp.Boss;
        var mons = Monsters.MonsterSide(cp.Kind, cp.Count, cp.Boss, "B");
        foreach (var m in mons) m.Grp = groups.Count;
        groups.Add(new ReplayGroup { Name = cp.Name, Side = "B", Kind = cp.Kind });
        // ganimet payı: grubun savaş gücü
        var pw = J.Map(per, cs => Combat.PowerOf(cs, Combat.AvgAc(mons)));
        double pwSum = J.Or(J.Reduce(pw, (x, y) => x + y, 0.0), 1);
        var share = J.Map(pw, p => p / pwSum);
        bool joint = band.Count > 1;
        string whoBefore = joint ? JoinNames(J.Map(J.Filter(groups, g => g.Side == "A"), g => g.Name)) : groups[0].Name;
        var b = Combat.ResolveBattle(s.Rng, side, mons, CivOpts(s, civ, "A", new BattleOpts { Id = s.Id(), Day = s.Day, Tile = cp.Tile, Title = $"{cp.Name} baskını", SideA = whoBefore, SideB = Monsters.MonsterName(cp.Kind), MoraleA = 0.55, MoraleB = cp.Kind == "hobgoblin" ? 0.65 : 0.5, MaxRounds = 20, TimeoutWinner = "B", Groups = groups, CivA = civ?.Id }));
        RecordBattle(s, b);
        Quest QOf(Agent qa) => J.T(qa.Quest) ? J.Find(w.Quests, x => x.Id == qa.Quest) : null;
        SyncHeroes(s, side, b.Winner == "A" ? 100 : 20);
        Will.AfterCampFight(s, side, cp, b.Winner == "A", hadBoss, b.Rolls);
        for (int i = 0; i < band.Count; i++)
        {
            var a = band[i];
            double deadS = J.Filter(per[i], x => (x.Kind == "soldier" || x.Kind == "unique") && x.Hp <= 0).Count;
            if (a.Civ >= 0 && J.T(a.Troops)) deadS -= Gear.HealWounded(s, w.Civs[a.Civ], deadS); // iksir yaralıları kurtarır
            if (J.T(a.Troops)) { a.Troops -= deadS; RemoveFromPop(s, a.Pop, deadS); }
            a.Heroes = J.Map(LiveHeroes(s, a), h => h.Id);
        }
        // zafer satırında yalnızca sağ kalanlar anılır
        string who = joint ? whoBefore : J.Or(J.T(lead.Heroes?.Count) ? string.Join(", ", J.Map(LiveHeroes(s, lead), h => h.Name)) : $"{Undef(civ?.Name)} askerleri", whoBefore);
        if (b.Winner == "A")
        {
            cp.Alive = false; cp.Count = 0; cp.ClearedDay = s.Day;
            w.Tiles[cp.Tile].Camp = null;
            double loot = JsMath.Round(cp.Loot);
            s.Metric("campCleared");
            var helped = new List<Civ>();   // TS Set<Civ>: insertion order, iterated
            for (int i = 0; i < band.Count; i++)
            {
                var a = band[i];
                var q = QOf(a);
                double part = loot * share[i];
                var acv = a.Civ >= 0 ? w.Civs[a.Civ] : null;
                var hs = a.Heroes ?? new List<int>();
                if (q != null)
                {
                    q.Done = s.Day;
                    s.Metric("questDone");
                    if (q.Civ < 0) s.Metric("innQuestDone");
                    var payer = q.Civ >= 0 ? w.Civs[q.Civ] : null;
                    double each = Math.Floor((q.Bounty + part * 0.5) / Math.Max(1, hs.Count));
                    foreach (int id in hs) { var h = s.Hero(id); h.Gold += each; if (q.Civ >= 0) h.Rep.Set(q.Civ, (h.Rep.Get(q.Civ) ?? 0) + 1); }
                    if (payer != null) { s.Add(payer, "gold", JsMath.Round(part * 0.5)); if (!helped.Contains(payer)) helped.Add(payer); }
                    if (!joint) s.Log("lair", $"{who}, {Tr.Ek(cp.Name, "i")} yerle bir etti! {(payer != null ? payer.Name : "Hancı")} ödülü ödedi.", tile: cp.Tile, civ: payer?.Id, battle: b.Id, cause: $"İlan: {J.S(q.Bounty)} altın; {J.S(loot)} değerinde ganimet", major: true);
                }
                else if (acv != null)
                {
                    s.Add(acv, "gold", JsMath.Round(part));
                    if (!helped.Contains(acv)) helped.Add(acv);
                    if (!joint) s.Log("lair", $"{acv.Name} seferi {Tr.Ek(cp.Name, "i")} yerle bir etti ve {J.S(loot)} değerinde ganimetle döndü!", tile: cp.Tile, civ: acv.Id, battle: b.Id, cause: "Kahramanlar ve askerler omuz omuza", major: true);
                }
                else
                {
                    double each = Math.Floor(part / Math.Max(1, hs.Count));
                    foreach (int id in hs) s.Hero(id).Gold += each;
                    s.Metric("huntDone");
                    if (!joint) s.Log("lair", $"{who}, kimse istemeden {Tr.Ek(cp.Name, "i")} yerle bir etti ve {J.S(loot)} değerinde ganimeti paylaştı.", tile: cp.Tile, battle: b.Id, cause: "Kahramanın kendi yolu", major: true);
                }
            }
            if (joint)
            {
                s.Metric("jointWin");
                var first = J.At(helped, 0);
                var groupsA = J.Filter(groups, g => g.Side == "A");
                var shares = new List<string>();
                for (int gi = 0; gi < groupsA.Count; gi++) shares.Add($"{groupsA[gi].Name} %{J.S(JsMath.Round(share[gi] * 100))}");
                s.Log("lair", $"Ortak saldırı! {whoBefore} birlikte {Tr.Ek(cp.Name, "i")} yerle bir etti.", tile: cp.Tile, civ: first?.Id, battle: b.Id, cause: $"{J.S(band.Count)} grup aynı hedefte buluştu; {J.S(loot)} değerindeki ganimet güç payına göre bölüşüldü ({string.Join(", ", shares)})", major: true);
            }
            if (helped.Count > 0)
            {
                foreach (var hc in helped)
                {
                    hc.Threat *= 0.4;
                    foreach (var o in w.Civs) if (o.Alive && o.Id != hc.Id && s.Rel(o.Id, hc.Id).Contact && s.G.Dist(s.Capital(o)?.Tile ?? 0, cp.Tile) <= 20)
                            s.AddMod(o.Id, hc.Id, "camphelp", $"{hc.Name} bir canavar kampını temizletti", 10, 20, 0.02);
                }
            }
            else
            {
                // ilansız temizlenen kamp: yakındaki herkes rahatlar
                foreach (var o in w.Civs) if (o.Alive && J.Some(s.CivSettlements(o), x => s.G.Dist(x.Tile, cp.Tile) <= 18)) o.Threat *= 0.6;
            }
            foreach (var oq in w.Quests) if (oq.Camp == cp.Id && oq.Open)
                {
                    oq.Open = false;
                    if (oq.Civ >= 0) s.Add(w.Civs[oq.Civ], "gold", oq.Bounty); else { var inn = J.Find(w.Inns, x => x.Id == oq.Inn); if (inn != null) inn.Gold += oq.Bounty; }
                }
        }
        else
        {
            cp.Count = J.Filter(mons, x => x.Kind == "monster" && x.Hp > 0).Count;
            cp.Boss = J.Some(mons, x => J.T(x.Boss) && x.Hp > 0);
            var qs = J.Filter(J.Map(band, QOf), q => q != null);
            foreach (var q in qs) Heroes.QuestFailed(s, q);
            int? lc = J.Find(band, ba => ba.Civ >= 0)?.Civ ?? J.Find(qs, q => q.Civ >= 0)?.Civ;
            s.Log("lair", $"{(joint ? "Ortak saldırı: " : "")}{whoBefore}, {Tr.Ek(cp.Name, "da")} püskürtüldü.", tile: cp.Tile, civ: lc != null && lc >= 0 ? lc : null, battle: b.Id, cause: $"{Monsters.MonsterName(cp.Kind)} {J.S(cp.Count)} kişiyle kampı tuttu{(b.Replay?.End == "timeout" ? " (gün battı, kamp düşmedi)" : "")}{(qs.Count > 0 ? $"; ilan {string.Join("/", J.Map(qs, q => J.S(q.Bounty)))} altına çıktı" : "")}", major: true);
        }
        foreach (var a in band) { if (a.Kind == "army") ArmyReturn(s, a); else PartyReturn(s, a); }
    }

    /// <summary>Ajanın taşıdığı nüfustan n kişiyi (ırkı rastgele seçerek) düşer; p null ise bir şey yapmaz.</summary>
    public static void RemoveFromPop(Sim s, JsObj<double> p, double n)
    {
        if (p == null) return;
        for (int i = 0; i < n; i++)
        {
            var rs = J.Filter(p.Keys(), k => (p.Get(k) ?? 0) > 0);
            if (rs.Count == 0) return;
            string r = s.Rng.Pick(rs); p.Set(r, (p.Get(r) ?? 1) - 1); if (!J.T(p.Get(r))) p.Delete(r);
        }
    }

    private static void PartyReturn(Sim s, Agent a)
    {
        var alive = J.Filter(J.Map(a.Heroes ?? new List<int>(), id => s.Hero(id)), h => h.State != "dead");
        a.Returning = true;
        a.Path = new List<int> { TileOf(a) }; a.Step = 0; a.Progress = 0;
        // her kahraman kendi yuvasına döner
        foreach (var h in alive) { h.Pos = TileOf(a); h.Goal = null; if (h.Civ == -1) Will.ReturnToBase(s, h); }
        a.Heroes = new List<int>();
    }
    private static void PartyHome(Sim _s, Agent _a) { /* kahramanlar partyReturn'de dağıldı */ }

    /// <summary>Ordu başkente döner (denizaşırı seferden kıyıda bekleyen kogalara binerek); başkent yoksa yerinde kalır.</summary>
    public static void ArmyReturn(Sim s, Agent a)
    {
        var c = J.At(s.W.Civs, a.Civ);
        var cap = c != null ? s.Capital(c) : null;
        a.Returning = true;
        if (cap == null) { a.Path = new List<int> { TileOf(a) }; a.Step = 0; a.Progress = 0; return; }
        // denizaşırı seferden dönüş: kıyıda bekleyen kogalara binilir
        var sp = Sea.SeaReturnPath(s, a, cap.Tile);
        if (sp != null) { a.Path = sp; a.Step = 0; a.Progress = 0; } else Reroute(s, a, cap.Tile);
    }
    private static void ArmyHome(Sim s, Agent a)
    {
        var c = J.At(s.W.Civs, a.Civ);
        var cap = c != null ? s.Capital(c) : null;
        foreach (int id in a.Heroes ?? new List<int>()) { var h = s.Hero(id); if (h.State != "dead") { h.State = cap != null ? "home" : "gone"; if (cap != null) h.Pos = cap.Tile; } }
        if (cap != null) { s.MergePop(cap, a.Pop); cap.Soldiers += JsMath.Max(0, a.Troops ?? 0); }
    }

    // ------------------------------------------------------------ savaş, kuşatma, yağma
    private static void Siege(Sim s, List<Agent> band, Settlement st)
    {
        var w = s.W;
        var lead = band[0];
        var att = w.Civs[lead.Civ]; var dfc = w.Civs[st.Civ];
        var civsIn = J.Unique(J.Map(band, ba => w.Civs[ba.Civ]));
        bool isCap = s.Capital(dfc)?.Id == st.Id;
        bool joint = band.Count > 1;
        string nameA = civsIn.Count > 1 ? $"{JoinNames(J.Map(civsIn, ci => ci.Name))} orduları" : joint ? $"{att.Name} orduları" : $"{att.Name} ordusu";
        var per = new List<List<Combatant>>();
        // TS hangs _side / _def / _per on the battle and reads them back from the chosen one, which is always the last fight
        List<Combatant> lastSide = null, lastDef = null;
        Battle Fight()
        {
            var bs = BandSide(s, band);
            per = bs.Per;
            var side0 = bs.Side; var groups = bs.Groups;
            var def0 = Defenders(s, st, "B", false);
            if (J.Some(civsIn, ci => s.Has(ci, "siege"))) foreach (var d in def0) d.Ac -= 2;
            foreach (var d in def0) d.Grp = groups.Count;
            groups.Add(new ReplayGroup { Name = $"{st.Name} savunucuları", Side = "B", Civ = dfc.Id });
            var o = new BattleOpts { Id = s.Id(), Day = s.Day, Tile = st.Tile, Title = $"{st.Name} kuşatması", SideA = nameA, SideB = $"{st.Name} savunucuları", MoraleA = 0.55, MoraleB = 0.65, MaxRounds = 20, TimeoutWinner = "B", Groups = groups, CivA = att.Id, CivB = dfc.Id };
            o = CivOpts(s, att, "A", o, dfc); o = CivOpts(s, dfc, "B", o, att);
            var b0 = Combat.ResolveBattle(s.Rng, side0, def0, o);
            lastSide = side0; lastDef = def0;
            return b0;
        }
        var b = WithLuck(s, att, Fight, x => x.Winner == "B");
        if (b.Winner == "A" && J.T(dfc.Eff.Get("luck")) && dfc.Yearly.Get("luck") != s.Year) { dfc.Yearly.Set("luck", s.Year); b = Fight(); J.Unshift(b.Lines, new BattleLine { T = $"Talih {dfc.Name} tarafına döndü!", Crit = true }); }
        var side = lastSide; var def = lastDef;
        // per = b._per: already the last fight's per
        RecordBattle(s, b);
        SyncHeroes(s, side, b.Winner == "A" ? 100 : 20);
        ApplyDefLosses(s, st, def);
        double deadAll = 0;
        for (int i = 0; i < band.Count; i++)
        {
            var a = band[i];
            var c = w.Civs[a.Civ];
            double dead = J.Filter(per[i], x => (x.Kind == "soldier" || x.Kind == "unique") && x.Hp <= 0).Count;
            double back = Math.Floor(dead * JsMath.Min(0.8, s.E(c, "healBack")));
            back += Gear.HealWounded(s, c, dead - back); // iksir yaralıları kurtarır
            a.Troops = (a.Troops ?? 0) - dead + back; RemoveFromPop(s, a.Pop, dead - back);
            a.Heroes = J.Map(LiveHeroes(s, a), h => h.Id);
            deadAll += dead - back;
        }
        if (b.Winner == "A") { st.BurnedHouses = (st.BurnedHouses ?? 0) + s.Rng.Int(2, 4); st.BurnedAt = s.Day; Will.OnHomeBurned(s, st, null, att); }
        if (b.Winner == "A")
        {
            foreach (var c in civsIn) c.Stats.BattlesWon++;
            dfc.Stats.BattlesLost++;
            if (joint) s.Metric("jointWin");
            if (!isCap)
            {
                // şehir, en çok askeri ayakta kalan orduya geçer
                var win = J.Sorted(band, (x, y) => J.Or((y.Troops ?? 0) - (x.Troops ?? 0), ReferenceEquals(x, lead) ? -1 : 1))[0];
                var wc = w.Civs[win.Civ];
                st.Civ = wc.Id; st.Soldiers = 0;
                s.MergePop(st, win.Pop); win.Pop = new JsObj<double>(); win.Troops = 0;
                s.UpdateTerritory();
                s.Metric("conquest");
                if (joint) s.Log("war", $"{nameA} birlikte {Tr.Ek(st.Name, "i")} düşürdü! Şehir {Tr.Ek(wc.Name, "in")} bayrağı altına geçti.", civ: wc.Id, tile: st.Tile, battle: b.Id, cause: $"Ortak kuşatma: {JoinNames(J.Map(band, ba => $"{BandName(s, ba)} ({J.S(ba.Troops ?? 0)} asker kaldı)"))}", major: true);
                else s.Log("war", $"{att.Name}, {Tr.Ek(st.Name, "i")} fethetti! Halkı ({s.RaceStr(st.Pop)}) artık onların bayrağı altında.", civ: att.Id, tile: st.Tile, battle: b.Id, cause: s.Rel(att.Id, dfc.Id).War?.Goal, major: true);
            }
            else
            {
                // başkent yağması: her orduya ayakta kalan gücü oranında pay
                var weight = J.Map(civsIn, ci => J.Reduce(J.Filter(band, ba => ba.Civ == ci.Id), (n, ba) => n + (ba.Troops ?? 0) + (ba.Heroes?.Count ?? 0) * 3, 0.0) + 0.01);
                double tot = J.Reduce(weight, (x, y) => x + y, 0.0);
                var parts = new List<string>();
                for (int i = 0; i < civsIn.Count; i++) { var ci = civsIn[i]; parts.Add($"{(civsIn.Count > 1 ? ci.Name + ": " : "")}{LootFrom(s, ci, dfc, 0.35 * weight[i] / tot)}"); }
                s.Log("war", $"{nameA} {Tr.Ek(st.Name, "i")} yağmaladı: {string.Join(" · ", parts)}.", civ: att.Id, tile: st.Tile, battle: b.Id, major: true, cause: joint ? "Ortak kuşatma" : null);
            }
        }
        else
        {
            foreach (var c in civsIn) c.Stats.BattlesLost++;
            dfc.Stats.BattlesWon++;
            s.Log("war", $"{st.Name}, {Tr.Ek(nameA, "in")} saldırısını püskürttü! Saldıranlar {J.S(deadAll)} kayıp verdi.", civ: dfc.Id, tile: st.Tile, battle: b.Id, major: true, cause: b.Replay?.End == "timeout" ? "Gün battı, surlar düşmedi" : null);
        }
        foreach (var c in civsIn) s.AddMod(c.Id, dfc.Id, "blood", "Dökülen kan", -8, -30, 0.02);
        if (civsIn.Count > 1) foreach (var x in civsIn) foreach (var y in civsIn) if (!ReferenceEquals(x, y)) s.SetMod(x.Id, y.Id, "brothers", "Silah arkadaşlığı", 14, 0.01);
        foreach (var a in band) ArmyReturn(s, a);
    }

    private static void Plunder(Sim s, Agent a, Settlement st)
    {
        var w = s.W;
        var att = w.Civs[a.Civ]; var dfc = w.Civs[st.Civ];
        var side = CivTroops(s, att, a.Troops ?? 0, "A");
        var def = Defenders(s, st, "B", false);
        var o = new BattleOpts { Id = s.Id(), Day = s.Day, Tile = st.Tile, Title = $"{st.Name} yağması", SideA = $"{att.Name} akıncıları", SideB = $"{st.Name} savunucuları", MoraleA = 0.5, MoraleB = 0.6, TimeoutWinner = "B", CivA = att.Id, CivB = dfc.Id };
        o = CivOpts(s, att, "A", o, dfc);
        var b = Combat.ResolveBattle(s.Rng, side, def, o);
        RecordBattle(s, b);
        ApplyDefLosses(s, st, def);
        double dead = J.Filter(side, x => x.Hp <= 0).Count - Gear.HealWounded(s, att, J.Filter(side, x => x.Hp <= 0).Count);
        a.Troops = (a.Troops ?? 0) - dead; RemoveFromPop(s, a.Pop, dead);
        s.Metric("plunder");
        if (b.Winner == "A")
        {
            st.BurnedHouses = (st.BurnedHouses ?? 0) + s.Rng.Int(1, 3); st.BurnedAt = s.Day;
            string loot = LootFrom(s, att, dfc, 0.2 * (1 + s.E(att, "lootMult")));
            string stolen = "";
            if (s.E(att, "lootTech") > 0 && s.Rng.Chance(s.E(att, "lootTech")))
            {
                var cand = J.Filter(dfc.Research.Done, td => !s.Has(att, td) && D.TECH[td].Tree == "main" && D.TECH[td].Era <= att.Era && J.Every(D.TECH[td].Req, rq => s.Has(att, rq)));
                if (cand.Count > 0) { string t = s.Rng.Pick(cand); att.Research.Done.Add(t); s.RecomputeEff(att); stolen = $" ve {D.TECH[t].Name} bilgisini zorla öğrendi"; s.Metric("stolenTech"); }
            }
            s.Log("war", $"{att.Name} akıncıları {Tr.Ek(st.Name, "i")} yağmaladı: {loot}{stolen}.", civ: att.Id, tile: st.Tile, battle: b.Id, major: true, cause: $"{D.CLASSES[att.Cls].Feature}: yağma ekonomisi");
        }
        else
        {
            s.Log("war", $"{st.Name}, {att.Name} akıncılarını geri püskürttü.", civ: dfc.Id, tile: st.Tile, battle: b.Id, major: true);
        }
        s.AddMod(dfc.Id, att.Id, "plunder", "Yağma baskını", -15, -45, 0.015, false);
        s.AddMod(att.Id, dfc.Id, "blood", "Dökülen kan", -4, -20, 0.02, false);
        ArmyReturn(s, a);
    }

    private static string LootFrom(Sim s, Civ att, Civ dfc, double frac)
    {
        var parts = new List<string>();
        double gold = Math.Floor(s.St(dfc, "gold") * frac);
        if (gold > 0) { s.Add(dfc, "gold", -gold); s.Add(att, "gold", gold); parts.Add($"{J.S(gold)} altın"); }
        var goods = J.Slice(J.Sort(J.Filter(D.GOOD_IDS, gd => gd != "gold" && s.St(dfc, gd) * D.GOODS[gd].Base > 20), (x, y) => s.St(dfc, y) * D.GOODS[y].Base - s.St(dfc, x) * D.GOODS[x].Base), 0, 2);
        foreach (var g in goods) { double q = Math.Floor(s.St(dfc, g) * frac); if (q > 0) { s.Add(dfc, g, -q); s.Add(att, g, q); parts.Add($"{J.S(q)} {J.TrLower(D.GOODS[g].Name)}"); } }
        return J.Or(string.Join(", ", parts), "boş ambarlar");
    }
}
