using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using FD.Game;
using FD.Sim.Life;
using FD.World;
using V2 = System.Numerics.Vector2;
using M = FD.Macro;

namespace FD.Dev;

/// <summary>
/// Faz 2 G (--gtest): people talk from their state and the world's (a hurt hero hears about it), rumours come from the sim's events
/// (each told once), the board shows the camp's quest and the player can take it (the village then posts no new one), and — symmetry —
/// a band of the sim's heroes that took the quest walks from the inn to the camp and fights it in the region by the same rules; the
/// sim gets the result (camp cleared or heroes repelled, quest closed or failed, the band goes home). PASS/FAIL, exit code 0/1.
/// </summary>
public partial class GTest : Node
{
    Region _r;
    bool _ok = true;
    int _phase;
    float _t;
    string _result;
    M.Quest _q;
    List<M.Hero> _crew;
    V2 _start;

    public static void Run(Node host, Region r) => host.AddChild(new GTest { Name = "GTest", _r = r });
    void Check(string name, bool pass, string info) { _ok &= pass; GD.Print($"[GTest] {name}: {info} {(pass ? "PASS" : "FAIL")}"); }

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        var s = _r.Session; var life = _r.Life;
        // talk
        var me = _r.Player.Character;
        var villagers = life.People.Where(p => p.Household >= 0 && p.Role != Role.Goblin).Take(12).ToList();
        var lines = villagers.Select(p => _r.Talk.Line(p)).ToList();
        Check("laf", lines.All(l => !string.IsNullOrEmpty(l)), string.Join(" | ", lines.Take(5)));
        me.Hp = 2;
        var hurt = villagers.Select(p => _r.Talk.Line(p)).ToList();
        Check("lafta kahramanın hâli", hurt.Count(l => l.Contains("Kan kaybediyorsun")) >= 2, $"can 2/{me.MaxHp}: {hurt.Count(l => l.Contains("Kan kaybediyorsun"))}/12 kişi kanı fark etti");
        me.Hp = me.MaxHp;
        // rumours
        var news = M.Local.News(s.Macro, s.Macro.W.Day - 60, 24);
        var inn = life.People.First(p => p.Role == Role.Innkeeper);
        _r.Talk.Rumor(inn); _r.Talk.Rumor(inn);
        Check("söylenti simden", news.Count > 0, $"{news.Count} haber; ilki: {news.FirstOrDefault()?.Text}");
        // the board
        var q = M.Local.CampQuest(s.Macro);
        var quests = _r.Board.Quests().ToList();
        Check("pano", quests.Count > 0 && (q == null || quests.Contains(q)), string.Join(" | ", quests.Select(x => $"{x.Bounty:F0} altın, {(x.Open ? "açık" : "alınmış")}")));
        // symmetry first (the quest must still be open for the heroes), then the player takes the next one
        _q = q ?? M.Local.EnsureQuest(s.Macro);
        var cp = s.Camp;
        _crew = s.Macro.W.Heroes.Where(h => h.State == "tavern" && h.Civ == -1).OrderByDescending(h => h.Level).Take(3).ToList();
        var a = new M.Agent { Id = s.Macro.Id(), Kind = "party", Civ = -1, Path = new List<int> { cp.Tile }, Step = 0, Progress = 0, Speed = 1, Heroes = _crew.Select(h => h.Id).ToList(), To = cp.Id, Quest = _q?.Id, Purpose = "quest" };
        if (_q != null) { _q.TakenBy = _crew.Select(h => h.Id).ToList(); _q.Open = false; }
        foreach (var h in _crew) { h.State = "quest"; h.Tavern = -1; }
        s.Macro.W.Agents.Add(a);
        bool taken = s.Macro.LocalCamp(new List<M.Agent> { a }, cp);
        Check("kahramanlar bölgeye geldi", taken && _r.Bands.Active == 1, $"{string.Join(", ", _crew.Select(h => $"{h.Name} ({h.Cls} Sv{h.Level}, can {h.Hp})"))} → {cp.Name} ({cp.Count:F0} goblin{(cp.Boss ? " + şef" : "")})");
        var p0 = life.People.First(p => p.HeroId == _crew[0].Id);
        _start = p0.Pos;
        _r.Bands.Finished += r => _result = r;
        Engine.TimeScale = 8.0;
    }

    public override void _Process(double delta)
    {
        _t += (float)delta;
        var s = _r.Session;
        if (_phase == 0)
        {
            if (_result == null && _t < 400f) return;
            Engine.TimeScale = 1.0;
            var cp = s.Camp;
            var p0 = _r.Life.People.First(p => p.HeroId == _crew[0].Id);
            var ev = s.Macro.W.Events.LastOrDefault(e => e.Kind == "lair");
            Check("kampa yürüdüler ve dövüştüler", _result != null && V2.Distance(_start, p0.Pos) > 50f, $"{_result} (tarih: {ev?.Text})");
            bool closed = _q == null || _q.Done != null || _q.Open || (_q.Failures ?? 0) > 0;
            Check("sim sonucu aldı", closed && (cp.Alive ? ev?.Text.Contains("püskürtüldü") == true : ev?.Text.Contains("yerle bir") == true) && _crew.All(h => h.State != "quest"),
                $"kamp {(cp.Alive ? $"ayakta ({cp.Count:F0})" : "temizlendi")}, ilan {(_q?.Done != null ? "tamam" : _q?.Open == true ? "yeniden açık" : "?")}; kahramanlar: {string.Join(", ", _crew.Select(h => $"{h.Name} {h.State} can {h.Hp:F0} XP {h.Xp:F0}"))}");
            // the player takes the (re)opened quest
            if (cp.Alive)
            {
                var q2 = M.Local.CampQuest(s.Macro) ?? M.Local.EnsureQuest(s.Macro);
                var took = _r.Board.Take();
                for (int i = 0; i < 3; i++) { s.Macro.Step(); M.Local.DayTick(s.Macro); }
                int open = s.Macro.W.Quests.Count(x => x.Camp == cp.Id && x.Open);
                Check("ilanı oyuncu aldı", took != null && took.TakenBy.Contains(s.Link.Player ?? -1) && open == 0, $"{took?.Bounty:F0} altınlık ilan oyuncunun; 3 gün sonra açık ilan {open}");
            }
            GD.Print(_ok ? "GTEST PASS" : "GTEST FAIL");
            GetTree().Quit(_ok ? 0 : 1);
            _phase = 1;
        }
    }
}
