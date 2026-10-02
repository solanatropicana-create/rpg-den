using System;
using System.Collections.Generic;
using FD.Rpg;
using M = FD.Macro;

namespace FD.Game;

/// <summary>
/// One playthrough (Faz 2): the macro world (<see cref="M.Sim"/>, deterministic, seed + prehistory), its link to the 1:1 region
/// (<see cref="M.RegionLink"/>: village, inn, goblin camp) and the clock coupling. Godot-free so headless tests can drive it.
/// <para>Time: the region's game clock (GameClock, 1 day = 30 real minutes) and the macro day are the same day. Local day
/// <see cref="BaseLocalDay"/> is macro day <see cref="BaseMacroDay"/>; <see cref="SyncDay"/> steps the macro sim once per local
/// midnight (and catches up after a skipped night).</para>
/// </summary>
public sealed class Session
{
    public static Session Current { get; set; }

    public double Seed { get; private set; }
    public M.Sim Macro { get; private set; }
    public M.RegionLink Link => Macro.W.Region;
    /// <summary>local day (GameClock.Day) that corresponds to <see cref="BaseMacroDay"/></summary>
    public int BaseLocalDay;
    public int BaseMacroDay;
    /// <summary>raised after each macro day step (on the main thread, from <see cref="SyncDay"/>)</summary>
    public event Action<int> MacroDayStepped;

    /// <summary>Faz 2: the player character (null until created) and the party (companions, the player first)</summary>
    public Character Player;
    public readonly List<Character> Party = new();
    int _nextCharId = 1;
    public int NextCharId() => _nextCharId++;
    public int PeekNextCharId() => _nextCharId;
    public void SetNextCharId(int v) => _nextCharId = Math.Max(1, v);
    /// <summary>region facts the macro world does not hold (saved with the local state)</summary>
    public readonly Dictionary<string, double> Flags = new();

    /// <summary>Register the created character in the macro world (hero record, State "player") and make them the party leader.</summary>
    public M.Hero AddPlayer(Character c)
    {
        var st = new M.JsObj<double>();
        for (int i = 0; i < 6; i++) st.Set(Rules.StatIds[i], c.Stats[i]);
        var h = M.Local.CreatePlayer(Macro, new M.Local.PlayerSpec
        {
            Name = c.Name, Race = c.Race, Cls = c.Cls, Align = c.Align, Faith = c.Faith, Stats = st, MaxHp = c.MaxHp, Ac = c.Ac,
            Gold = c.Inv.Silver / (double)Rules.SilverPerGold, Age = RegionBind.Ages(c.Race).adult + 2,
        });
        c.HeroId = h.Id;
        c.IsPlayer = true;
        if (c.Id == 0) c.Id = NextCharId();
        Player = c;
        Party.Remove(c);
        Party.Insert(0, c);
        return h;
    }

    /// <summary>Write the player's local numbers back to the macro hero record (HP, level, XP, purse, faith, wounds as epithet).</summary>
    public void SyncPlayerToMacro()
    {
        if (Player?.HeroId is not int id) return;
        var h = Macro.Hero(id);
        if (h == null) return;
        h.Hp = Player.Hp; h.MaxHp = Player.MaxHp; h.Ac = Player.Ac; h.Level = Player.Level; h.Xp = Player.Xp;
        h.Gold = Player.Inv.Silver / (double)Rules.SilverPerGold;
        h.Faith = Player.Faith; h.Epithet = Player.Epithet;
        if (Player.Dead && h.State != "dead") { h.State = "dead"; h.DeathDay = Macro.W.Day; }
    }

    /// <summary>
    /// New world: <c>new Sim(seed)</c> with prehistory (progress: day, total), then the region link (<see cref="M.Local.Bind"/>).
    /// Safe to call from a worker thread (the sim has no static mutable state).
    /// </summary>
    public static Session NewWorld(double seed, Action<int, int> progress = null, int prehistory = M.Sim.PREHISTORY_DAYS)
    {
        var sim = new M.Sim(seed, prehistory, progress);
        M.Local.Bind(sim);
        return new Session { Seed = seed, Macro = sim, BaseMacroDay = sim.W.Day, BaseLocalDay = 0 };
    }

    /// <summary>Wrap an already loaded macro world (save/load).</summary>
    public static Session FromSim(M.Sim sim, double seed, int baseLocalDay, int baseMacroDay) =>
        new() { Seed = seed, Macro = sim, BaseLocalDay = baseLocalDay, BaseMacroDay = baseMacroDay };

    /// <summary>Macro day that the given local day maps to.</summary>
    public int MacroDayFor(int localDay) => BaseMacroDay + (localDay - BaseLocalDay);

    /// <summary>Step the macro world until it reaches the local day. Returns the number of steps taken.</summary>
    public int SyncDay(int localDay)
    {
        int want = MacroDayFor(localDay), n = 0;
        while (Macro.W.Day < want)
        {
            Macro.Step();
            M.Local.DayTick(Macro);
            n++;
            MacroDayStepped?.Invoke(Macro.W.Day);
        }
        return n;
    }

    public M.VillageInfo Village() => M.Local.Info(Macro);
    public M.Settlement VillageSettlement => M.Local.Village(Macro);
    public M.Inn Inn => M.Local.Inn(Macro);
    public M.Camp Camp => M.Local.Camp(Macro);
    public M.Civ Civ => M.Local.Civ(Macro);

    /// <summary>Region names as the 1:1 slice shows them: village, inn (without "Hanı"), camp.</summary>
    public (string village, string inn, string camp) Names()
    {
        var v = VillageSettlement;
        var inn = Inn;
        var cp = Camp;
        return (v?.Name ?? "Sessiztepe", inn?.Name ?? "Yorgun Katır", cp?.Name ?? "Kırık Diş kampı");
    }

    /// <summary>One-line summary for logs / dev tools.</summary>
    public string Describe()
    {
        var vi = Village();
        var (v, i, c) = Names();
        var cp = Camp;
        var q = M.Local.CampQuest(Macro);
        var tq = q == null ? M.Local.TakenQuest(Macro) : null;
        string races = vi == null ? "" : string.Join(", ", RacesSorted(vi));
        return $"tohum {Seed} · gün {Macro.W.Day} · {v} ({vi?.CivName}, {vi?.GovName}; {vi?.RulerTitle}) nüfus {vi?.Pop:F0} [{races}] · " +
               $"{i} Hanı · {c} ({cp?.Count:F0} goblin{(cp != null && cp.Boss ? " + şef" : "")}) · ilan {(q != null ? $"{q.Bounty:F0} altın" : tq != null ? $"{tq.Value.q.Bounty:F0} altın, {string.Join(", ", tq.Value.heroes.ConvertAll(h => h.Name))} aldı" : "yok")}";
    }

    public static IEnumerable<string> RacesSorted(M.VillageInfo vi)
    {
        var l = new List<KeyValuePair<string, double>>(vi.Races);
        l.Sort((a, b) => b.Value.CompareTo(a.Value));
        foreach (var kv in l) yield return $"{kv.Key} {kv.Value:F0}";
    }
}
