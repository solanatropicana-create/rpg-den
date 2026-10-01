using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using FD.Macro;

// C# twin of golden/ts/innlife_world.ts (mode "world") and golden/ts/innlife_unit.ts (mode "unit"); see InnLifeCheck.csproj.
namespace InnLifeCheck;

public static class Program
{
    public static int Main(string[] args)
    {
        D.Init();
        string mode = args.Length > 0 ? args[0] : "world";
        var o = new StreamWriter(Console.OpenStandardOutput(), new UTF8Encoding(false), 1 << 16);
        try
        {
            if (mode == "world") World(o, args);
            else if (mode == "unit") Unit(o, args);
            else throw new ArgumentException("mode: world | unit");
        }
        finally { o.Flush(); }
        return 0;
    }

    // ------------------------------------------------------------ world
    private static void World(StreamWriter o, string[] args)
    {
        double seed = double.Parse(args.Length > 1 ? args[1] : "1", CultureInfo.InvariantCulture);
        int days = int.Parse(args.Length > 2 ? args[2] : "600", CultureInfo.InvariantCulture);
        int scen = int.Parse(args.Length > 3 ? args[3] : "0", CultureInfo.InvariantCulture);
        double? target = args.Length > 4 ? double.Parse(args[4], CultureInfo.InvariantCulture) : null;
        var sim = new Sim(seed);
        var w = sim.W;
        if (target != null) { w.InnPlan.Target = target.Value; w.InnPlan.Wave = JsMath.Max(w.InnPlan.Wave, JsMath.Min(target.Value, 4)); }

        // ---------------------------------------------------------- scripted hero scenario (scen=1), see innlife_world.ts
        string[] CLS = { "ranger", "fighter", "rogue", "cleric", "wizard", "paladin", "druid", "barbarian" };
        double[] GOLD = { 0, 0.5, 3, 25, 0, 1, 0, 12 };
        string[] NAMES = { "Aldric", "Mira", "Tomas", "Elena", "Borin", "Helga", "Pip", "Gruk" };
        string[] RACES = { "human", "human", "human", "elf", "dwarf", "dwarf", "halfling", "halforc" };
        Hero MkHero(int k, Inn inn) => new Hero
        {
            Id = 900000 + k, Name = NAMES[k], Race = RACES[k], Cls = CLS[k], Level = 1 + (k % 4), Xp = 0,
            Stats = new JsObj<double> { ["str"] = 10, ["dex"] = 10, ["con"] = 10, ["int"] = 10, ["wis"] = 10, ["cha"] = 10 },
            MaxHp = 20, Hp = 20, Ac = 12, Civ = -1, Pos = inn.Tile, Tavern = inn.Id, State = "tavern", Born = k == 0 ? w.Day : 0, IdleSince = w.Day, Kills = 0, Gold = GOLD[k], Bio = "test",
            Align = "neutral", Path = "wanderer", Traits = new List<string>(), Epithet = k % 3 == 1 ? "Cesur" : null, Tally = new JsObj<double>(), Bonus = new HeroBonus { Atk = 0 }, Rep = new JsNumObj<double>(),
            Journal = k % 2 == 0 ? new List<JournalEntry> { new JournalEntry { Day = 0, Text = $"günlük {k}" } } : new List<JournalEntry>(), Base = k == 4 ? inn.Id : -5, BaseInn = k == 4, Birth = -5, Legend = k == 4 ? true : null,
        };
        var touched = new HashSet<int>();
        Hero tempCiv = null;
        void ScriptBefore()
        {
            int d = w.Day;
            foreach (var a in w.Agents)
            {
                if (a.Kind != "keeper" || J.T(a.Dead) || touched.Contains(a.Id)) continue;
                touched.Add(a.Id);
                int tt = a.TargetTile.Value;
                if (a.Id % 3 == 0) w.Tiles[tt].InnZone = 999999;
                else if (a.Id % 3 == 1) foreach (int t in sim.G.Within(tt, 4)) w.Tiles[t].InnZone = 999999;
            }
            var hinn = J.Find(w.Inns, i => i.Alive);
            if (hinn == null) return;
            if (w.Heroes.Count == 0) for (int k = 0; k < 8; k++) w.Heroes.Add(MkHero(k, hinn));
            foreach (var h in w.Heroes)
            {
                int k = h.Id - 900000;
                bool present = (d + k * 7) % 40 < 25;
                h.Civ = -1; h.Tavern = hinn.Id; h.State = present ? "tavern" : "traveling";
                h.Goal = !present && k == 2 ? new HeroGoal { Kind = "ruin", Tile = hinn.Tile, Text = "eski harabeyi keşfetmek", Since = d } : null;
                if (k == 5 && d % 97 == 13) h.State = "dead";
                if (k == 6 && d % 83 == 29) h.State = "retired";
                if (k == 7 && d % 71 == 41) h.State = "gone";
                if (k == 3 && d % 59 == 17) { h.Civ = 0; h.State = "home"; tempCiv = h; }
                if (d % 50 == 0) h.Gold += k * 3;
            }
            if (d % 97 == 50) InnLife.InnIncome(sim, hinn, 12.5, $"Test geliri {d}");
            if (d % 131 == 70) InnLife.InnScatter(sim, hinn, "Test baskını", false);
            if (scen >= 2) Script2(d, hinn);
        }
        Settlement NearestAlive(int tile) => J.At(J.Sort(J.Filter(w.Settlements, x => x.Alive), (a, b) => J.Or((double)(sim.G.Dist(a.Tile, tile) - sim.G.Dist(b.Tile, tile)), a.Id - b.Id)), 0);
        Settlement starvedSt = null; double starvedOld = 0;
        void Script2(int d, Inn hinn)
        {
            var near = NearestAlive(hinn.Tile);
            if ((d + hinn.Id) % 30 == 0)
            {
                int phase = (int)Math.Floor(d / 30.0) % 3;
                if (phase == 0)
                {
                    hinn.Staff.Add(new InnStaff { Name = $"Ekstra{d}a", Race = "human", Role = "asci", Since = d - 5, From = "test" });
                    hinn.Staff.Add(new InnStaff { Name = $"Ekstra{d}b", Race = "elf", Role = "garson", Since = d - 2, From = "test" });
                    hinn.Staff.Add(new InnStaff { Name = $"Ekstra{d}c", Race = "gnome", Role = "bekci", Since = d - 2, From = "test" });
                    hinn.Gold = 1;
                }
                else if (phase == 1 && hinn.Hist.Count >= 6)
                {
                    foreach (var h in J.Slice(hinn.Hist, -6)) h.Guests = 99;
                    hinn.Gold = JsMath.Max(hinn.Gold, 200);
                }
            }
            if (d % 5 == 0 && near != null) { starvedSt = near; starvedOld = near.Starving; near.Starving = 2; }
            if (near == null) return;
            var path = sim.Path(near.Tile, hinn.Tile);
            if (path == null || path.Count < 2) return;
            void Push(Agent a) { a.Id = sim.Id(); a.Civ = near.Civ; a.Path = path; a.Step = 0; a.Progress = 0; a.Speed = 0.7; w.Agents.Add(a); }
            if (d % 150 == 75) Push(new Agent { Kind = "supply", Cargo = new JsObj<double> { ["bread"] = 3, ["beer"] = 2, ["fish"] = 1, ["meat"] = 1, ["stone"] = 2, ["wood"] = 3, ["grain"] = 5 }, From = near.Id, To = hinn.Id, Inn = hinn.Id, Purpose = "supply" });
            if (d % 150 == 76) Push(new Agent { Kind = "supply", Cargo = new JsObj<double>(), From = -1, To = hinn.Id, Inn = hinn.Id, Purpose = "supply" });
            if (d % 150 == 77) Push(new Agent { Kind = "traveler", From = near.Id, To = hinn.Id, Inn = hinn.Id, Purpose = "come", Guest = new Guest { Id = sim.Id(), Kind = "wanderer", Name = "Yabancı", Race = "elf", N = 1, Civ = -1, From = -1, FromName = "uzaklar", To = -1, ToName = "bilinmez", Why = "test", Purse = 5, Spent = 0, Nights = 2, Arrived = d, Mood = 0.7 } });
            if (d % 150 == 78) Push(new Agent { Kind = "traveler", From = near.Id, To = hinn.Id, Inn = hinn.Id, Purpose = "come" });
            if (d % 150 == 79) Push(new Agent { Kind = "keeper", From = near.Id, To = 424242, Inn = 424242, Purpose = "found", TargetTile = hinn.Tile });
        }
        void ScriptAfter()
        {
            if (tempCiv != null) { tempCiv.Civ = -1; tempCiv = null; }
            if (starvedSt != null) { starvedSt.Starving = starvedOld; starvedSt = null; }
        }
        Dictionary<string, object> Probes(int d)
        {
            if (scen < 2 || d % 100 != 0) return null;
            var hinn = J.Find(w.Inns, i => i.Alive);
            int tile = hinn != null ? hinn.Tile : w.Settlements[0].Tile;
            var alive = J.Map(w.Settlements, x => x.Alive);
            foreach (var x in w.Settlements) x.Alive = false;
            string r0 = InnLife.WhereStr(sim, tile);
            w.Settlements[0].Alive = true;
            string r1 = InnLife.WhereStr(sim, tile);
            for (int i = 0; i < w.Settlements.Count; i++) w.Settlements[i].Alive = alive[i];
            var shortP = new List<int> { tile, tile, tile };
            return new Dictionary<string, object> { ["r0"] = r0, ["r1"] = r1, ["shortSame"] = ReferenceEquals(InnLife.InnDetour(sim, shortP), shortP) };
        }

        void CivAI(Civ c)
        {
            if (sim.CivSettlements(c).Count == 0) { sim.Extinct(c); return; }
            if (!J.T(c.Research.Current)) Research.ChooseResearch(sim, c);
            Research.EraCheck(sim, c);
            Economy.ChooseBuilds(sim, c);
            Economy.RecruitTick(sim, c);
            Diplomacy.ConsiderExpansion(sim, c);
            Diplomacy.ConsiderScout(sim, c);
            Diplomacy.ConsiderTrade(sim, c);
            Diplomacy.ConsiderWarAction(sim, c);
            Diplomacy.ConsiderRaid(sim, c);
        }

        void Step()
        {
            w.Day++;
            foreach (var c in w.Civs) if (c.Alive) Economy.EconomyTick(sim, c);
            if (w.Day % 5 == 0) foreach (var c in w.Civs) if (c.Alive) Economy.RepairTick(sim, c);
            foreach (var c in w.Civs) if (c.Alive && (w.Day + c.Id * 3) % 10 == 0) CivAI(c);
            if (w.Day % 10 == 0) { sim.UpdateTerritory(); Diplomacy.RelationsTick(sim); }
            if (w.Day % 30 == 0) { sim.Discover(); Diplomacy.WorldTick(sim); Events.DisastersTick(sim); }
            if (w.Day % Sim.YEAR == 0) foreach (var c in w.Civs) if (c.Alive) Research.ClassYearly(sim, c);
            if (scen == 0) Monsters.CampsTick(sim);
            if (scen != 0) ScriptBefore();
            InnLife.InnLifeTick(sim);
            if (scen != 0) ScriptAfter();
            Agents.AgentsTick(sim);
            Agents.RoutesTick(sim);
            Agents.RoadsTick(sim);
            foreach (var c in w.Civs) c.Threat = JsMath.Min(1.5, JsMath.Max(0, c.Threat - 0.0015));
            if (w.Day % 60 == 0) foreach (var c in w.Civs) if (c.Alive) c.History.Add(new HistPoint { Day = w.Day, Pop = sim.CivPop(c), Gold = JsMath.Round(sim.St(c, "gold")), Techs = c.Research.Done.Count });
            w.RngState = sim.Rng.State();
        }

        var innAgents = new HashSet<string> { "keeper", "traveler", "supply", "caravan" };
        int lastEv = -1;
        void Dump()
        {
            int d = w.Day;
            var events = J.Filter(w.Events, e => e.Id > lastEv);
            foreach (var e in events) lastEv = Math.Max(lastEv, e.Id);
            var doc = new Dictionary<string, object>
            {
                ["day"] = d, ["rng"] = sim.Rng.State(), ["nextId"] = w.NextId, ["metrics"] = w.Metrics, ["innPlan"] = w.InnPlan, ["inns"] = w.Inns,
                ["agents"] = d % 10 == 0 ? w.Agents : J.Filter(w.Agents, a => innAgents.Contains(a.Kind)), ["events"] = events,
                ["civs"] = d % 30 == 0 ? w.Civs : null, ["settlements"] = d % 30 == 0 ? w.Settlements : null,
                ["routes"] = d % 30 == 0 ? w.Routes : null, ["camps"] = d % 30 == 0 ? w.Camps : null, ["quests"] = d % 30 == 0 ? w.Quests : null,
                ["tiles"] = d % 90 == 0 ? w.Tiles : null,
                ["heroes"] = scen != 0 ? w.Heroes : null,
                ["probes"] = Probes(d),
                ["detour"] = scen != 0 && d % 10 == 0 ? J.Map(J.Filter(w.Routes, r => r.Alive), r => { var p = InnLife.InnDetour(sim, r.Path); return new Dictionary<string, object> { ["id"] = r.Id, ["same"] = ReferenceEquals(p, r.Path), ["len"] = p.Count, ["head"] = J.Slice(p, 0, 3) }; }) : null,
            };
            o.Write(Json.Serialize(doc));
            o.Write('\n');
        }

        Dump();
        for (int k = 1; k <= days; k++)
        {
            try { Step(); }
            catch (Exception e)
            {
                o.Write(Json.Serialize(new Dictionary<string, object> { ["day"] = w.Day, ["error"] = e.GetType().Name + ": " + e.Message, ["stack"] = e.StackTrace }));
                o.Write('\n');
                break;
            }
            Dump();
        }
    }

    // ------------------------------------------------------------ unit
    private static void Unit(StreamWriter o, string[] args)
    {
        // input: one JSON {inn} per line (TS innlife_unit.ts output); output: the pure helpers' results
        foreach (var line in File.ReadLines(args[1]))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var doc = System.Text.Json.JsonDocument.Parse(line);
            var inn = System.Text.Json.JsonSerializer.Deserialize<Inn>(doc.RootElement.GetProperty("inn").GetRawText(), Json.Options);
            var pr = InnLife.InnPrices(inn);
            var occ = InnLife.InnOcc(inn);
            var res = new Dictionary<string, object>
            {
                ["title"] = InnLife.InnTitle(inn), ["levelName"] = InnLife.LevelName(inn),
                ["prices"] = new Dictionary<string, object> { ["room"] = pr.Room, ["meal"] = pr.Meal, ["ale"] = pr.Ale },
                ["occ"] = new Dictionary<string, object> { ["rooms"] = occ.Rooms, ["stable"] = occ.Stable, ["persons"] = occ.Persons },
                ["serveCap"] = InnLife.ServeCap(inn), ["wages"] = InnLife.Wages(inn), ["stage"] = InnLife.BuildStageName(inn), ["pct"] = InnLife.BuildPct(inn), ["pctS"] = J.S(InnLife.BuildPct(inn)),
            };
            o.Write(Json.Serialize(new Dictionary<string, object> { ["out"] = res }));
            o.Write('\n');
        }
    }
}
