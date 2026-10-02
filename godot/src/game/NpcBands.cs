using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using FD.Rpg;
using FD.Sim.Life;
using FD.World;
using V2 = System.Numerics.Vector2;
using M = FD.Macro;

namespace FD.Game;

/// <summary>
/// Faz 2 G (simetri): a band of the sim's heroes that took the camp's quest comes through the region and fights the camp here, by the
/// same rules as the player. When the macro band reaches the bound camp (<c>Sim.LocalCamp</c>), its heroes appear at the inn (the inn's
/// own guests get up from their benches) and walk the road and the forest trail to the camp; there the camp's goblins (sleepers waking)
/// and the heroes fight a real-time <see cref="Fight"/> — visible to a player nearby, run anyway when no one watches. The result goes
/// back to the sim like any of its battles (<c>Local.LocalBandResult</c>: XP, deaths, the quest, the camp cleared or holding); the dead
/// lie where they fell, the living walk back and leave. While the player is fighting the camp the band waits.
/// </summary>
public partial class NpcBands : Node
{
    sealed class Member { public M.Hero Hero; public Person P; public Character C; public Fighter F; public float Lag; }
    sealed class Band
    {
        public List<int> Agents;
        public readonly List<Member> Members = new();
        public List<V2> Path;
        public float S;
        public int Phase;          // 0 walking in, 1 fighting, 2 walking out, 3 gone
        public Fight Fight;
        public string Result;
    }

    Region _r;
    Session _s;
    readonly List<Band> _bands = new();
    readonly List<(Person p, V2 to, float left)> _runOff = new();
    public const float Pace = 2.0f;
    public event Action<string> Finished;
    public int Active => _bands.Count(b => b.Phase < 3);
    public bool Fighting => _bands.Any(b => b.Phase == 1);

    public void Init(Region r)
    {
        _r = r; _s = r.Session;
        Name = "NpcBands";
        _s.Macro.LocalCamp = OnArrive;
    }

    public override void _ExitTree() { if (_s?.Macro != null && _s.Macro.LocalCamp == OnArrive) _s.Macro.LocalCamp = null; }

    /// <summary>The way in: from the inn's door to the road, west to the trail, up the trail and the goblins' footpath.</summary>
    static List<V2> PathIn(V2 from)
    {
        var p = new List<V2> { from };
        foreach (var q in new[] { new Vector2(220, 30), new Vector2(80, 60) }) p.Add(new V2(q.X, q.Y));
        foreach (var q in RegionSpec.Trail.Points) p.Add(new V2(q.X, q.Y));
        foreach (var q in RegionSpec.CampSpurControl.Skip(1)) p.Add(new V2(q.X, q.Y));
        return p;
    }

    static (V2 pos, V2 dir) At(List<V2> path, float s)
    {
        for (int i = 0; i + 1 < path.Count; i++)
        {
            float L = V2.Distance(path[i], path[i + 1]);
            if (s <= L) { var d = V2.Normalize(path[i + 1] - path[i] + new V2(1e-4f, 0)); return (path[i] + d * s, d); }
            s -= L;
        }
        var e = path[^1]; return (e, V2.Normalize(path[^1] - path[^2] + new V2(1e-4f, 0)));
    }

    static float Length(List<V2> path) { float L = 0; for (int i = 0; i + 1 < path.Count; i++) L += V2.Distance(path[i], path[i + 1]); return L; }

    bool OnArrive(List<M.Agent> agents, M.Camp cp)
    {
        var ids = agents.Select(a => a.Id).ToList();
        if (_bands.Any(b => b.Phase < 3 && b.Agents.Intersect(ids).Any())) return true;   // already on its way
        var heroes = agents.SelectMany(a => a.Heroes ?? new List<int>()).Select(id => _s.Macro.Hero(id)).Where(h => h != null && h.State != "dead").ToList();
        if (heroes.Count == 0) return false;
        var inn = _r.Life.PlaceOf(PlaceKind.Inn);
        var start = inn?.Door ?? new V2(300, 0);
        var band = new Band { Agents = ids, Path = PathIn(start) };
        int k = 0;
        foreach (var h in heroes)
        {
            var p = _r.Life.People.FirstOrDefault(x => x.Guest && x.HeroId == h.Id);
            if (p == null)
            {
                var age = RegionBind.Ages(h.Race);
                p = Census.AddGuest(_r.Life, new GuestSpec
                {
                    Name = string.IsNullOrEmpty(h.Given) ? h.Name.Split(' ')[0] : h.Given, Surname = h.Surname ?? "", Race = h.Race, Cls = h.Cls,
                    Level = (int)h.Level, Align = h.Align ?? "neutral", HeroId = h.Id, Female = CharacterFactory.IsFemaleName(h), Age = age.adult + 3 + (int)h.Level * 2,
                }, (ulong)(h.Id * 977 + 13));
                _r.LifeWorld.AddActor(p);
            }
            if (p.Act?.Spot != null && p.Act.Spot.TakenBy == p.Id) p.Act.Spot.TakenBy = -1;
            p.Present = true; p.InFight = true; p.Motion = Motion.Doing; p.FightAnim = "Walk"; p.Down = false; p.Dead = false;
            p.Pos = start; p.Path.Clear();
            p.Note($"{H.Clock(_r.Life.Now)} {cp.Name} ilanının peşinde kampa yürüyor");
            band.Members.Add(new Member { Hero = h, P = p, C = CharacterFactory.FromHero(h), Lag = k * 1.6f });
            k++;
        }
        _bands.Add(band);
        string names = string.Join(", ", heroes.Select(h => h.Name));
        GD.Print($"[NpcBands] {names} {cp.Name} için yola çıktı ({Length(band.Path):F0} m)");
        if (Near(start, 120f)) _r.Hud.Toast($"{names} silahlarını kuşanıp handan çıktı: {cp.Name} ilanının peşindeler.", 6f);
        return true;
    }

    bool Near(V2 p, float d) => V2.Distance(p, new V2(_r.Player.GlobalPosition.X, _r.Player.GlobalPosition.Z)) < d;

    public override void _Process(double delta)
    {
        float dt = (float)Math.Min(delta, 0.1);
        StepRunOff(dt);
        foreach (var b in _bands)
        {
            switch (b.Phase)
            {
                case 0: Walk(b, dt, +1); break;
                case 1: FightStep(b, dt); break;
                case 2: Walk(b, dt, -1); break;
            }
        }
    }

    void Walk(Band b, float dt, int dir)
    {
        float L = Length(b.Path);
        b.S += dir * Pace * dt;
        foreach (var m in b.Members)
        {
            if (m.P.Dead || !m.P.InFight) continue;
            float s = Math.Clamp(b.S - dir * m.Lag, 0, L);
            var (pos, d) = At(b.Path, s);
            var side = new V2(-d.Y, d.X) * ((m.Lag / 1.6f) % 2 == 0 ? 0.5f : -0.5f);
            m.P.Pos = pos + side; m.P.Dir = dir > 0 ? d : -d; m.P.FightAnim = "Walk";
        }
        if (dir > 0 && b.S >= L - 2f)
        {
            if (_r.Combat?.Active == true) { b.S = L - 2f; foreach (var m in b.Members) m.P.FightAnim = "Idle"; return; }   // the player is fighting here: wait
            StartFight(b);
        }
        if (dir < 0 && b.S <= 0)
        {
            foreach (var m in b.Members) if (!m.P.Dead) { m.P.InFight = false; m.P.FightAnim = null; m.P.Present = false; m.P.Act = null; m.P.Motion = Motion.Inside; }
            b.Phase = 3;
        }
    }

    void StartFight(Band b)
    {
        var camp = new V2(RegionSpec.GoblinCamp.X, RegionSpec.GoblinCamp.Y);
        var f = b.Fight = new Fight(_s.Seed * 104729 + Math.Floor(GameClock.TotalHours * 600));
        foreach (var m in b.Members)
        {
            m.F = f.AddCharacter(m.C, m.Hero, FSide.Party, m.P.Pos, "hero");
            m.F.LifeId = m.P.Id;
        }
        int gob = 0;
        foreach (var p in _r.Life.People)
        {
            if (p.Role != Role.Goblin || p.Dead || p.InFight || !p.Present) continue;
            if (V2.Distance(p.Pos, camp) > 80f && p.Act?.Place != _r.Life.PlaceOf(PlaceKind.Camp)?.Id) continue;
            bool archer = !p.IsBoss && p.Work % 4 == 3;
            if (p.Motion == Motion.Inside && p.Act != null) p.Pos = p.Act.Target;
            var fg = f.AddMonster(p.IsBoss ? "goblinBoss" : "goblin", p.Name, FSide.Foe, p.Pos, p.IsBoss ? "boss" : archer ? "archer" : "goblin", p.Id, archer ? 16f : 0f);
            fg.Dir = p.Dir;
            if (p.Act?.Spot != null && p.Act.Spot.TakenBy == p.Id) p.Act.Spot.TakenBy = -1;
            p.InFight = true; p.FightAnim = "Idle"; p.FightTool = archer ? "proc_bow" : null; p.Motion = Motion.Doing; p.Path.Clear();
            gob++;
        }
        f.FoeHome = camp + V2.Normalize(camp - b.Members[0].P.Pos + new V2(1e-3f, 0)) * 75f;
        f.Begin($"{string.Join(", ", b.Members.Select(m => m.Hero.Name))} kampa saldırdı!");
        b.Phase = 1;
        GD.Print($"[NpcBands] savaş: {b.Members.Count} kahraman vs {gob} goblin");
        if (Near(camp, 150f)) _r.Hud.Toast($"{string.Join(", ", b.Members.Select(m => m.Hero.Name))} goblin kampına saldırdı!", 5f);
    }

    void FightStep(Band b, float dt)
    {
        var f = b.Fight;
        f.Update(dt);
        foreach (var x in f.F)
        {
            if (x.LifeId < 0) continue;
            var p = _r.Life.People[x.LifeId];
            if (!p.InFight) continue;
            p.Pos = x.Pos; p.Dir = x.Dir; p.FightAnim = x.Anim; p.Down = x.Down; p.Dead = x.Dead;
            if (x.Dead) { p.InFight = x.Side == FSide.Party ? false : false; p.Note($"{H.Clock(_r.Life.Now)} öldü"); }
            else if (x.Fled && x.Side == FSide.Foe && !_runOff.Exists(r => r.p == p)) _runOff.Add((p, x.FleeTo, 25f));
        }
        if (!f.Over) return;
        Finish(b);
    }

    void Finish(Band b)
    {
        var f = b.Fight;
        int killed = 0; bool boss = false;
        foreach (var x in f.F) if (x.Side == FSide.Foe && x.Dead) { killed++; if (x.IsBoss) boss = true; }
        // the camp is broken when its chief fell and at most one goblin still stands (or none at all)
        int left = 0; bool bossAlive = false;
        foreach (var p in _r.Life.People)
        {
            if (p.Role != Role.Goblin || p.Dead || !p.Present || _runOff.Exists(r => r.p == p)) continue;
            var fx = f.F.FirstOrDefault(x => x.LifeId == p.Id);
            if (fx != null && fx.Fled) continue;
            left++; if (p.IsBoss) bossAlive = true;
        }
        bool won = f.Winner == FSide.Party;
        bool cleared = won && (left == 0 || (!bossAlive && left <= 1));
        var hp = new Dictionary<int, double>(); var kills = new Dictionary<int, int>();
        foreach (var m in b.Members)
        {
            if (m.F == null) continue;
            hp[m.Hero.Id] = m.F.Dead ? 0 : Math.Max(1, m.F.Hp);
            kills[m.Hero.Id] = m.F.Kills;
        }
        GD.Print($"[NpcBands] kahramanların canı: {string.Join(", ", b.Members.Select(m => $"{m.Hero.Name} {m.F?.Hp:F0}/{m.F?.MaxHp:F0}{(m.F?.Down == true ? " yerde" : "")}{(m.F?.Dead == true ? " öldü" : "")}"))}");
        b.Result = M.Local.LocalBandResult(_s.Macro, b.Agents, hp, killed - (boss ? 1 : 0), boss, cleared, kills);
        GD.Print($"[NpcBands] sonuç: {(won ? "zafer" : "yenilgi")} öldürülen {killed} şef {boss} kamp {(cleared ? "temizlendi" : "duruyor")} — {b.Result}");
        _r.Hud.Toast($"Haber: {b.Result}", 7f);
        // goblins still standing go back to their lives
        foreach (var x in f.F)
        {
            if (x.LifeId < 0 || x.Side != FSide.Foe) continue;
            var p = _r.Life.People[x.LifeId];
            if (!p.InFight || x.Dead || _runOff.Exists(r => r.p == p)) continue;
            p.InFight = false; p.FightAnim = null; p.FightTool = null; p.Act = null; p.Motion = Motion.Doing; p.NextDecision = _r.Life.Now;
        }
        // the heroes: the dead lie there, the rest walk back out
        foreach (var m in b.Members)
        {
            if (m.F != null && m.F.Dead) { m.P.Dead = true; m.P.InFight = false; continue; }
            m.P.Down = false; m.P.FightAnim = "Walk";
        }
        b.S = Length(b.Path) - 2f;
        b.Phase = b.Members.Any(m => !m.P.Dead) ? 2 : 3;
        Finished?.Invoke(b.Result);
        SaveGame.Save(_s, "kahramanlar");
    }

    void StepRunOff(float dt)
    {
        for (int i = _runOff.Count - 1; i >= 0; i--)
        {
            var (p, to, left) = _runOff[i];
            var d = to - p.Pos; float L = d.Length();
            left -= dt;
            if (L > 0.5f && left > 0) { p.Pos += d / L * MathF.Min(L, 4.4f * dt); p.Dir = d / L; p.FightAnim = "Run"; _runOff[i] = (p, to, left); continue; }
            _runOff.RemoveAt(i);
            p.InFight = false; p.FightAnim = null;
            bool gone = _s.Camp == null || !_s.Camp.Alive;
            p.Motion = Motion.Inside;
            if (gone) { p.Present = false; p.Act = null; p.NextDecision = double.MaxValue; continue; }
            p.Act = new Activity { Kind = ActKind.Rest, Inside = true, Place = -1, Target = p.Pos, Until = _r.Life.Now + 240, Label = "Ormanda saklanıyor", Reason = "Bozguna uğradı; tehlike geçene dek saklanıyor." };
            p.NextDecision = p.Act.Until;
        }
    }
}
