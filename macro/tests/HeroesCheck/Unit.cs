using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FD.Macro;

namespace HeroesCheck;

/// <summary>
/// C# twin of golden/ts/heroes_unit.ts: calls Heroes/Will/Inns functions (public, and private ones by reflection)
/// directly on a fresh world with synthetic inns, heroes, goals, routes and raids; one JSON line per step.
/// Every step must stay statement-for-statement identical to the TS driver.
/// </summary>
public static class Unit
{
    private static Sim sim;
    private static World w;
    private static StreamWriter o;
    private static int lastEv = -1;

    private static readonly string[] RACES = { "human", "dwarf", "elf", "halfling", "gnome", "halfelf", "halforc", "dragonborn", "tiefling" };
    private static readonly string[] PATHS = { "hunter", "wanderer", "healer", "sage", "mercenary", "dark" };

    private static object Call(Type t, string name, params object[] args)
    {
        var m = t.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static) ?? throw new MissingMethodException(t.Name, name);
        try { return m.Invoke(null, args); }
        catch (TargetInvocationException e) { throw e.InnerException ?? e; }
    }

    private static void Out(string label, Dictionary<string, object> extra = null)
    {
        var events = J.Filter(w.Events, e => e.Id > lastEv);
        foreach (var e in events) lastEv = Math.Max(lastEv, e.Id);
        var line = new Dictionary<string, object> { ["label"] = label, ["day"] = w.Day, ["rng"] = sim.Rng.State(), ["nextId"] = w.NextId, ["events"] = events };
        if (extra != null) foreach (var kv in extra) line[kv.Key] = kv.Value;
        o.Write(Json.Serialize(line));
        o.Write('\n');
    }

    private static Dictionary<string, object> All() => new()
    {
        ["heroes"] = w.Heroes, ["agents"] = w.Agents, ["quests"] = w.Quests, ["inns"] = w.Inns, ["civs"] = w.Civs, ["battles"] = w.Battles, ["metrics"] = w.Metrics,
    };

    private static bool Alive(Hero h) => h.State != "dead" && h.State != "gone" && h.State != "retired";

    public static int Run(double seed, StreamWriter output)
    {
        o = output;
        sim = new Sim(seed);
        w = sim.W;
        var sts = w.Settlements;
        var camps = w.Camps;
        var civs = w.Civs;

        // A) rollWill
        {
            var wills = new List<object>();
            foreach (var cls in D.HERO_CLASS_IDS) for (int i = 0; i < 40; i++) { var r = Will.RollWill(sim, cls); wills.Add(new List<string> { r.Align, r.Path }); }
            Out("rollWill", new() { ["wills"] = wills });
        }
        // B) makeHero for every race × class × level
        {
            int k = 0;
            foreach (var race in RACES) foreach (var cls in D.HERO_CLASS_IDS) for (int level = 1; level <= 5; level++)
                    {
                        var st = sts[k % sts.Count];
                        Heroes.MakeHero(sim, (race, cls, level, st.Tile, st.Id, k % 7 == 3));
                        k++;
                    }
            Out("makeHero", new() { ["heroes"] = w.Heroes });
        }
        // C) gainXp with every growFromExperience branch
        {
            for (int i = 0; i < w.Heroes.Count; i++)
            {
                var h = w.Heroes[i];
                int m = i % 5;
                // ties (1.5 vs 1.5) on low-level heroes: the earlier option must win
                if (m == 0) { if (i % 10 == 5) { h.Tally.Set("goblin", 5); h.Tally.Set("plague", 1); } else { h.Tally.Set("goblin", 3); h.Tally.Set("kills", 4); } }
                else if (m == 1) { if (i % 10 == 6) { h.Tally.Set("plague", 1); h.Tally.Set("lib", 1); } else { h.Tally.Set("plague", 1); h.Tally.Set("temple", 1); } }
                else if (m == 2) { h.Tally.Set("ruins", 1); h.Tally.Set("rob", 1); }
                else if (m == 3) { h.Tally.Set("lib", 1); }
                if (i % 7 == 0) h.Civ = i % civs.Count;
            }
            for (int i = 0; i < w.Heroes.Count; i++) Heroes.GainXp(sim, w.Heroes[i], 250 * (1 + (i % 60)));
            Out("gainXp", new() { ["heroes"] = w.Heroes });
        }
        // D) labels, costs, willServe, isPact
        {
            for (int i = 0; i < w.Heroes.Count; i++) { var h = w.Heroes[i]; if (i % 3 == 0) h.Epithet = "Kırık Belası"; if (i % 4 == 1) h.Grudge = i % civs.Count; }
            for (int i = 0; i < civs.Count; i++) { var c = civs[i]; if (i % 3 == 1) c.Align.Good = -0.4; if (i % 3 == 2) c.Align.Good = -0.6; }
            var costs = new List<object>();
            foreach (var c in civs) foreach (var h in J.Slice(w.Heroes, 0, 60)) { var kk = Heroes.HeroCost(sim, c, h); costs.Add(new List<object> { kk.Gold, kk.Food, Will.HeroWillServe(c, h), Will.HeroLabel(h), Heroes.HeroBaseCost(h), Will.HasTrait(h, "legend"), Inns.IsPact(sim, c) }); }
            Out("costs", new() { ["costs"] = costs });
        }
        // E) afterCampFight
        {
            for (int j = 0; j < 40; j++)
            {
                var cp = camps[j % camps.Count];
                if (j % 8 == 5) cp.Kind = "hobgoblin";
                var hs = J.Slice(w.Heroes, j * 3, j * 3 + 3);
                if (j % 5 == 0) foreach (var h in hs) { h.Civ = -1; h.Align = "evil"; h.Path = "hunter"; h.Tally.Set("fails", 1); }
                if (j % 6 == 0) hs[1].Vendetta = cp.Id;
                int jj = j;
                var side = J.Map(hs, h => Will.HeroSide(h, "A", cp.Kind, jj % 2 == 0));
                for (int q = 0; q < side.Count; q++) side[q].Kills = ((j + q) % 4) * 4;
                if (j % 3 == 0) hs[0].State = "dead";
                if (j % 9 == 3) hs[2].State = "dead";
                var rolls = new List<BattleRoll> { new BattleRoll { D20 = 20, Side = "A", Who = hs[1].Name }, new BattleRoll { D20 = 13, Side = "B", Who = "x" } };
                Will.AfterCampFight(sim, side, cp, j % 2 == 0, j % 4 < 2, rolls);
                foreach (var h in hs) if (h.State == "dead") h.State = "tavern";
            }
            Out("afterCampFight", new() { ["heroes"] = w.Heroes });
        }
        // F) onHomeBurned
        {
            var six = J.Slice(sts, 0, 6);
            for (int j = 0; j < six.Count; j++) { var st = six[j]; Will.OnHomeBurned(sim, st, j % 2 == 0 ? camps[j % camps.Count] : null, j % 3 != 1 ? civs[(st.Civ + 1) % civs.Count] : null); }
            Out("onHomeBurned", new() { ["heroes"] = w.Heroes });
        }
        // G) threatCamp / campPower / campForce
        {
            var tc = J.Map(civs, c => (object)(Heroes.ThreatCamp(sim, c)?.Id ?? -1));
            var cpw = J.Map(camps, cp => (object)new List<double> { Heroes.CampPower(sim, cp), Heroes.CampPower(sim, cp, 12.5), Heroes.CampForce(sim, cp).Count });
            Out("camps", new() { ["tc"] = tc, ["cpw"] = cpw });
        }
        // H) synthetic inns, taverns, temples, libraries
        Inn MkInn(int k)
        {
            var st = sts[k % sts.Count];
            var cand = J.Filter(sim.G.Within(st.Tile, 3), t => !J.T(w.Tiles[t].Sea) && w.Tiles[t].Terrain != "water" && w.Tiles[t].Terrain != "mountain" && sim.G.Dist(t, st.Tile) == 3 && w.Tiles[t].Inn == null);
            int tile = cand.Count > 0 ? cand[0] : st.Tile;
            var inn = new Inn
            {
                Id = sim.Id(), Tile = tile, Name = $"Deneme{k}", Keeper = $"Hancı{k}", Founded = 0, Alive = true, Gold = 60 + 90 * k, Raids = 0, Stage = "open", Level = 1 + (k % 3),
                KeeperRace = "human", Origin = st.Id, OriginName = st.Name, Stock = new InnStock { Food = 40, Ale = 40, Wood = 5 }, Fame = 25 * k, Staff = new(), Guests = new(), Tabs = new(), Log = new(),
                Books = new(), Hist = new(), Total = new InnTotal { Guests = 0, Nights = 0, Income = 0, Turned = 0, Bought = 0 }, Turned = 0, Sat = 0.5, Traffic = 0,
            };
            w.Inns.Add(inn); w.Tiles[tile].Inn = inn.Id;
            return inn;
        }
        var inns = new List<Inn> { MkInn(0), MkInn(1), MkInn(2), MkInn(3) };
        for (int i = 0; i < sts.Count; i++) { var st = sts[i]; if (i % 2 == 0) st.Civics.Set("tavern", 1); if (i % 3 == 0) st.Civics.Set("temple", 1); if (i % 4 == 1) st.Civics.Set("library", 1); }
        // I) heroes into inns; basics
        {
            for (int i = 0; i < w.Heroes.Count; i++)
            {
                var h = w.Heroes[i];
                if (h.State == "dead") continue;
                if (i % 4 == 0) { var inn = inns[i % inns.Count]; h.Base = inn.Id; h.BaseInn = true; h.Pos = inn.Tile; h.Tavern = inn.Id; h.State = "tavern"; h.Civ = -1; h.Contract = null; }
            }
            var nr = new List<object>();
            for (int t = 0; t < w.Tiles.Count; t += 97) { var r = Will.NearestRest(sim, t); nr.Add(r != null ? new List<object> { r.Id, r.Tile, r.Inn } : null); }
            var bt = J.Map(w.Heroes, h => (object)(Will.BaseTile(sim, h) ?? -1));
            var ia = J.Map(inns, inn => (object)(Will.InnAt(sim, inn.Tile)?.Id ?? -1));
            ia.Add(Will.InnAt(sim, 0)?.Id ?? -1);
            var pools = J.Map(inns, inn => (object)J.Map(Inns.InnPool(sim, inn), h => h.Id));
            var iho = J.Map(civs, c => (object)J.Map(Inns.InnHeroesOf(sim, c), h => h.Id));
            var names = J.Map(inns, inn => Inns.InnName(inn));
            var byId = new List<string> { Will.InnById(sim, inns[2].Id)?.Name ?? "-", Will.InnById(sim, null)?.Name ?? "-", Will.InnById(sim, -5)?.Name ?? "-" };
            var dp = new List<double> { (double)Call(typeof(Will), "DistPen", sim, 0, 500), (double)Call(typeof(Will), "DistPen", sim, sts[0].Tile, sts[1 % sts.Count].Tile) };
            Out("innsBasics", new() { ["nr"] = nr, ["bt"] = bt, ["ia"] = ia, ["pools"] = pools, ["iho"] = iho, ["names"] = names, ["byId"] = byId, ["dp"] = dp });
        }
        // J) spawns (inn heroes with and without a retired teacher, tavern heroes)
        {
            var rh = w.Heroes[5]; rh.State = "retired"; inns[1].Teacher = rh.Id;
            inns[3].Teacher = w.Heroes[6].Id;
            for (int i = 0; i < 12; i++) Call(typeof(Inns), "SpawnInnHero", sim, inns[i % inns.Count]);
            for (int i = 0; i < 6; i++) Heroes.SpawnHero(sim, sts[i % sts.Count]);
            Out("spawn", new() { ["heroes"] = J.Slice(w.Heroes, -18) });
        }
        // K) auctions and bids
        {
            for (int i = 0; i < w.Heroes.Count; i++) { var h = w.Heroes[i]; if (i % 5 == 0) h.Rep.Set(i % civs.Count, 3); }
            foreach (var h in w.Heroes) if (h.BaseInn && h.Civ == -1 && h.State == "tavern" && h.Auction == null) h.Auction = new Auction { End = sim.Day + 120, Bids = new List<Bid>() };
            for (int i = 0; i < civs.Count; i++) { var c = civs[i]; sim.Add(c, "gold", 300 + 50 * i); c.Threat = i % 2 != 0 ? 0.5 : 0.1; }
            for (int r = 0; r < 3; r++) foreach (var c in civs) Inns.ConsiderInnBids(sim, c);
            var eff = J.Map(J.Filter(w.Heroes, h => h.Auction != null && h.Auction.Bids.Count > 0), h => (object)J.Map(h.Auction.Bids, b => (double)Call(typeof(Inns), "EffBid", sim, h, b)));
            Out("bids", new() { ["auctions"] = J.Map(w.Heroes, h => (object)h.Auction), ["eff"] = eff });
        }
        // L) closeAuction (one bidder busy in a party)
        {
            var busy = J.Find(w.Heroes, h => h.Auction != null && h.Auction.Bids.Count > 0);
            if (busy != null) w.Agents.Add(new Agent { Id = sim.Id(), Kind = "party", Civ = -1, Path = new List<int> { busy.Pos }, Step = 0, Progress = 0, Speed = 0.85, Heroes = new List<int> { busy.Id }, Purpose = "quest" });
            foreach (var h in new List<Hero>(w.Heroes)) if (h.Auction != null) { var inn = Will.InnById(sim, h.Base); if (inn != null) Call(typeof(Inns), "CloseAuction", sim, inn, h); }
            var d = All(); d["gold"] = J.Map(civs, c => sim.St(c, "gold"));
            Out("closeAuction", d);
        }
        // M) contractEnd (renew, leave, retire)
        {
            for (int i = 0; i < w.Heroes.Count; i++) { var h = w.Heroes[i]; if (h.Contract != null) { h.State = "home"; if (i % 2 == 0) { h.Level = Math.Max(h.Level, 4); h.Born = -2000; } h.Rep.Set(h.Contract.Civ, i % 4); } }
            foreach (var h in new List<Hero>(w.Heroes)) if (h.Contract != null) Call(typeof(Inns), "ContractEnd", sim, h);
            Out("contractEnd", All());
        }
        // N) postGuardQuest (topping a civ quest, posting inn quests)
        {
            w.Quests.Add(new Quest { Id = sim.Id(), Civ = 0, Camp = camps[0].Id, Bounty = 50, Posted = 0, TakenBy = new List<int>(), Open = true, Inn = inns[2].Id, Expires = 360 });
            inns[2].Gold = 400; inns[0].Gold = 30;
            var near1 = J.Filter(sim.G.Within(inns[1].Tile, 4), t => !J.T(w.Tiles[t].Sea) && w.Tiles[t].Terrain != "water" && sim.G.Dist(t, inns[1].Tile) == 4 && w.Tiles[t].Camp == null && w.Tiles[t].Inn == null);
            var mv = J.Find(camps, c => c.Alive && sim.G.Dist(c.Tile, inns[1].Tile) > 12);
            if (near1.Count > 0 && mv != null) { w.Tiles[mv.Tile].Camp = null; mv.Tile = near1[0]; w.Tiles[near1[0]].Camp = mv.Id; }
            foreach (var inn in inns) Call(typeof(Inns), "PostGuardQuest", sim, inn);
            Out("postGuardQuest", new() { ["quests"] = w.Quests, ["inns"] = w.Inns });
        }
        // O) considerHero / considerQuest
        {
            var thr = new[] { 0.1, 0.3, 0.5 };
            for (int i = 0; i < civs.Count; i++) { var c = civs[i]; c.Threat = thr[i % 3]; sim.Add(c, "grain", 200); }
            for (int i = 0; i < w.Heroes.Count; i++) { var h = w.Heroes[i]; if (h.Civ >= 0 && Alive(h) && i % 2 == 0) h.State = "home"; }
            foreach (var c in civs) if (c.Alive && sim.CivSettlements(c).Count > 0) { Heroes.ConsiderHero(sim, c); Heroes.ConsiderQuest(sim, c); }
            Out("consider", All());
        }
        // P) questFailed
        {
            foreach (var q in new List<Quest>(w.Quests)) Heroes.QuestFailed(sim, q);
            Out("questFailed", new() { ["quests"] = w.Quests, ["inns"] = w.Inns, ["gold"] = J.Map(civs, c => sim.St(c, "gold")) });
        }
        // Q) chooseGoal over free heroes (all paths; plague, ruin, temple, library, rob route, dark heroes)
        var ruinCand = J.Filter(sim.G.Within(sts[0].Tile, 5), t => !J.T(w.Tiles[t].Sea) && w.Tiles[t].Terrain != "water" && sim.G.Dist(t, sts[0].Tile) == 5 && w.Tiles[t].Camp == null);
        int ruinT = ruinCand.Count > 0 ? ruinCand[0] : sts[0].Tile;
        var ruin = new Settlement { Id = sim.Id(), Civ = 0, Name = "Yıkıkköy", Tile = ruinT, Founded = 0, Pop = new(), GrowthAcc = 0, Civics = new(), Workshops = new(), Project = null, Jobs = new(), Soldiers = 0, Alive = false, Starving = 0, Tier = 0, MixedSince = new() };
        sts.Add(ruin);
        var routePath = sim.Path(sts[0].Tile, sts[1].Tile) ?? new List<int>();
        var route = new TradeRoute { Id = sim.Id(), A = sts[0].Id, B = sts[1].Id, Kind = "trade", Path = routePath, NextDepart = 0, Trips = 0, Alive = true, Since = 0 };
        w.Routes.Add(route);
        {
            sts[1].Plague = new PlagueInfo { Since = 0, Until = 300, Severity = 0.7, Dead = 0 };
            for (int i = 0; i < w.Heroes.Count; i++)
            {
                var h = w.Heroes[i];
                if (h.Civ != -1 || !Alive(h)) continue;
                h.Path = PATHS[i % 6];
                if (i % 11 == 0) h.Align = "good";
                if (h.Path == "dark") h.Tally.Set("rob", 1);
                if (i % 13 == 0) h.SoloUntil = 500;
                h.Hp = h.MaxHp;
                h.State = "tavern";
                if (i % 3 == 0) { h.Pos = sts[i % sts.Count].Tile; h.Tavern = sts[i % sts.Count].Id; }
            }
            foreach (var h in new List<Hero>(w.Heroes)) if (h.Civ == -1 && h.State == "tavern") Will.ChooseGoal(sim, h);
            Out("chooseGoal", All());
        }
        // R) arriveGoal for every kind, progressGoal (library, rob with a caravan), duel
        {
            var free = J.Filter(w.Heroes, h => h.Civ == -1 && Alive(h));
            var kinds = new[] { "ruin", "plague", "temple", "library", "rob", "quest" };
            var fs = J.Slice(free, 0, 24);
            for (int i = 0; i < fs.Count; i++)
            {
                var h = fs[i];
                string kind = kinds[i % kinds.Length];
                int target = kind == "ruin" ? ruin.Id : kind == "plague" ? sts[1].Id : kind == "temple" || kind == "library" ? sts[i % sts.Count].Id : kind == "rob" ? route.Id : camps[0].Id;
                h.Goal = new HeroGoal { Kind = kind, Tile = h.Pos, Target = target, Text = $"test {kind}", Since = 0 };
                if (i % 12 == 6) h.Tally.Set("ruins", 2);
                Will.ArriveGoal(sim, h);
            }
            Out("arriveGoal", All());
            // duel: a good hunter meets a dark hero on the same tile
            var good = J.Slice(J.Filter(free, h => Alive(h) && h.Align == "good"), 0, 3);
            var dark = J.Slice(J.Filter(free, h => Alive(h) && h.Align != "good"), 0, 3);
            for (int i = 0; i < good.Count; i++)
            {
                var g = good[i];
                var od = J.At(dark, i); if (od == null) continue;
                od.Pos = g.Pos; od.State = i == 1 ? "traveling" : "tavern";
                g.Goal = new HeroGoal { Kind = "duel", Tile = g.Pos, Target = od.Id, Text = "düello", Since = 0 };
                Will.ArriveGoal(sim, g);
            }
            // a duel whose target is gone
            if (good.Count > 0 && Alive(good[0])) { good[0].Goal = new HeroGoal { Kind = "duel", Tile = good[0].Pos, Target = -77, Text = "yok", Since = 0 }; Will.ArriveGoal(sim, good[0]); }
            Out("duel", All());
            // progressGoal: library (stay over) and rob with a caravan nearby
            w.Day = 30;
            for (int i = 0; i < free.Count; i++)
            {
                var h = free[i];
                if (!Alive(h) || h.Goal == null) continue;
                if (h.Goal.Kind == "library") { h.Goal.Stay = 10; Call(typeof(Will), "ProgressGoal", sim, h); }
                else if (h.Goal.Kind == "rob" && i % 2 == 0)
                {
                    var cvPath = routePath.Count > 0 ? routePath : new List<int> { h.Pos };
                    int step = (int)Math.Floor(cvPath.Count / 2.0);
                    h.Pos = cvPath[step];
                    w.Agents.Add(new Agent { Id = sim.Id(), Kind = "caravan", Civ = sts[0].Civ, Path = cvPath, Step = step, Progress = 0, Speed = 0.65, Cargo = new JsObj<double> { ["grain"] = 10, ["iron"] = 3 }, Troops = 1 + (i % 3), From = sts[0].Id, To = sts[1].Id, Route = route.Id });
                    Call(typeof(Will), "ProgressGoal", sim, h);
                }
                else Call(typeof(Will), "ProgressGoal", sim, h);
            }
            Out("progressGoal", All());
        }
        // S) maybeRetire, tryRevive, sendHero / returnToBase / heroAgent
        {
            w.Day = 1500;
            var olds = J.Slice(J.Filter(w.Heroes, h => Alive(h)), 0, 40);
            for (int i = 0; i < olds.Count; i++) { var h = olds[i]; h.Level = 4 + (i % 2); h.Born = 0; if (i % 3 == 0) { h.BaseInn = true; h.Base = inns[i % inns.Count].Id; } }
            inns[0].Teacher = null;
            var ret = J.Map(olds, h => (object)Will.MaybeRetire(sim, h));
            var d = All(); d["ret"] = ret;
            Out("maybeRetire", d);
            civs[0].Eff.Set("revive", 1); civs[1 % civs.Count].Eff.Set("revive", 1);
            var cand = J.Slice(J.Filter(w.Heroes, h => Alive(h)), 0, 12);
            for (int i = 0; i < cand.Count; i++) { var h = cand[i]; h.Civ = i % 3 == 2 ? -1 : i % 2; if (i == 4) h.Revived = true; }
            var rev = J.Map(cand, h => (object)Heroes.TryRevive(sim, h));
            var ha = J.Map(J.Slice(w.Heroes, 0, 50), h => (object)(Will.HeroAgent(sim, h)?.Id ?? -1));
            for (int i = 0; i < cand.Count; i++) { var h = cand[i]; if (i % 2 == 0) Heroes.SendHero(sim, h, sts[i % sts.Count].Tile, "home"); else Will.ReturnToBase(sim, h); }
            var d2 = All(); d2["rev"] = rev; d2["ha"] = ha;
            Out("revive", d2);
        }
        // T) monster raid on an inn (won, lost → ruinInn), pact inn raid (considerInnRaid → innRaidArrive), breakGuestRight
        {
            w.Day = 720;
            foreach (var h in w.Heroes) if (h.Civ == -1 && Alive(h) && h.BaseInn) { var inn = Will.InnById(sim, h.Base); if (inn != null) { h.State = "tavern"; h.Pos = inn.Tile; h.Tavern = h.Base; } }
            Agent Mk(Inn inn, double troops, bool boss, string monster) => new Agent { Id = sim.Id(), Kind = "raid", Civ = -1, Path = new List<int> { inn.Tile }, Step = 0, Progress = 0, Speed = 0.9, Troops = troops, From = camps[0].Id, To = inn.Id, Purpose = "inn", Boss = boss, Monster = monster, TargetTile = inn.Tile };
            var r1 = Mk(inns[1], 2, false, "goblin"); w.Agents.Add(r1); Inns.InnMonsterRaid(sim, r1);
            var r2 = Mk(inns[2], 16, true, "hobgoblin"); w.Agents.Add(r2); Inns.InnMonsterRaid(sim, r2);
            var r3 = Mk(inns[2], 3, false, "bugbear"); w.Agents.Add(r3); Inns.InnMonsterRaid(sim, r3);
            var d3 = All(); d3["tiles"] = J.Map(inns, inn => w.Tiles[inn.Tile]);
            Out("monsterRaid", d3);
            var pc = civs[0];
            pc.Align.Good = -0.8;
            foreach (var st in sim.CivSettlements(pc)) st.Soldiers = 14;
            foreach (var oc in civs) if (oc.Id != pc.Id) { w.Relations[pc.Id][oc.Id].War = null; w.Relations[oc.Id][pc.Id].War = null; }
            pc.InnBanUntil = null;
            inns[0].Alive = true; inns[0].Gold = 500;
            int kept = 0;
            foreach (var h in w.Heroes) if (h.Tavern == inns[0].Id && h.State == "tavern") { if (kept++ >= 1) h.State = "traveling"; }
            int tries = 0;
            while (tries < 300 && !J.Some(w.Agents, a => a.Civ == pc.Id && a.Purpose == "innraid")) { Call(typeof(Inns), "ConsiderInnRaid", sim, pc); tries++; }
            var raid = J.Find(w.Agents, a => a.Civ == pc.Id && a.Purpose == "innraid");
            if (raid != null) Inns.InnRaidArrive(sim, raid);
            civs[1 % civs.Count].Eff.Set("crusade", 1);
            Inns.BreakGuestRight(sim, civs[1 % civs.Count], inns[3]);
            var defs = J.Map(inns, inn => (object)((List<Combatant>)Call(typeof(Inns), "InnDefenders", sim, inn, "B")).Count);
            var d4 = All(); d4["tries"] = tries; d4["defs"] = defs; d4["relations"] = w.Relations;
            Out("innRaid", d4);
        }
        // U) the ticks on chosen days (taverns, inns incl. expiring quests and auctions, heroes incl. migration)
        {
            foreach (int dd in new[] { 735, 750, 840, 960, 1080, 1200, 1500, 1800 })
            {
                w.Day = dd;
                Heroes.TavernsTick(sim);
                Inns.InnsTick(sim);
                Heroes.HeroesTick(sim);
                Out($"ticks{dd}", All());
            }
        }
        // V) nowhere to rest: every inn closed, no taverns
        {
            foreach (var inn in w.Inns) inn.Alive = false;
            foreach (var st in sts) st.Civics.Delete("tavern");
            var lost = J.Slice(J.Filter(w.Heroes, h => Alive(h)), 0, 3);
            foreach (var h in lost) Will.ReturnToBase(sim, h);
            var nr2 = Will.NearestRest(sim, 0);
            Out("noRest", new() { ["heroes"] = lost, ["nr2"] = nr2 != null ? new List<int> { nr2.Id } : null });
        }
        return 0;
    }
}
