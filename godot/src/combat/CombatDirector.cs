using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using FD.Actors;
using FD.Game;
using FD.Life;
using FD.Rpg;
using FD.Sim.Life;
using FD.World;
using V2 = System.Numerics.Vector2;
using M = FD.Macro;

namespace FD.Combat;

/// <summary>What a finished fight left behind (the summary panel, the macro write-back, the tests).</summary>
public sealed class FightOutcome
{
    public bool Won;
    public int Killed, Fled, PartyDown, PartyDead;
    public bool BossKilled, CampCleared, PlayerDead, AllDown;
    public int XpEach;
    /// <summary>D: what the goblins did with the fallen party: rob | capture | kill (null after a won fight)</summary>
    public string Fate;
    public Character Captive;
    public bool GameOver;
    public Character NewLeader;
    public readonly List<string> Lines = new();
}

/// <summary>
/// Faz 2 C: runs a real-time d20 fight in the region. A goblin that catches the player (<see cref="LifeSim.GoblinStrike"/>) starts
/// it: the party (player and companions) and every goblin near enough become <see cref="Fighter"/>s; goblins asleep in their
/// tents wake and join a few seconds later, goblins that reach the fight later join it. The fight moves the bodies (the player is
/// scripted, LifeSim leaves people with <c>InFight</c> alone); the camera goes up to the tactical view; the first time the fight
/// pauses by itself (setting). Space pauses (the whole scene tree stops: animation frozen, clock stopped), orders go to the
/// selected party members. At the end the results go back to the characters (HP, the fallen, XP, levels), the people (bodies stay
/// where they fell, the routed run off and hide), and the macro world (goblins killed, camp cleared).
/// </summary>
public partial class CombatDirector : Node
{
    public static CombatDirector Instance { get; private set; }
    public Fight Fight { get; private set; }
    public bool Active => Fight != null;
    public bool Paused { get; private set; }
    public bool Summary { get; private set; }
    public FightOutcome Outcome { get; private set; }
    public readonly List<Fighter> Selected = new();
    public Fighter Hover { get; private set; }
    /// <summary>a spell waiting for its target (aim mode)</summary>
    public SpellDef Aim { get; private set; }
    public Fighter AimCaster { get; private set; }
    public TacticalCamera Camera { get; private set; }
    public CombatHud Hud { get; private set; }
    public Rect2? Box { get; private set; }
    public event Action<FightOutcome> Ended;
    /// <summary>tests and shots: never auto-pause, close the summary by itself</summary>
    public bool Unattended;
    /// <summary>dev: pause when the fight clock reaches this (screenshots of the paused view)</summary>
    public float? PauseAt;

    Region _r;
    LifeSim _sim;
    Session _s;
    readonly Dictionary<Fighter, Node3D> _bodies = new();
    readonly List<(Person p, float at, string why)> _wake = new();
    readonly List<(Person p, V2 to, float left)> _runOff = new();
    Transform3D _camFrom;
    float _blend = 1f;
    bool _pressing, _boxing, _rpressing;
    Vector2 _press, _rpress;
    int _campPlace = -1;
    V2 _camp;
    string _hint;
    float _hintT;
    public string Hint => _hintT > 0 ? _hint : null;
    /// <summary>routed goblins still running off (driven here after the fight)</summary>
    public int RunningOff => _runOff.Count;

    public void Init(Region r)
    {
        Instance = this;
        Name = "CombatDirector";
        ProcessMode = ProcessModeEnum.Always;
        _r = r; _sim = r.Life; _s = r.Session;
        Camera = new TacticalCamera { Name = "TacticalCamera", ProcessMode = ProcessModeEnum.Always };
        AddChild(Camera);
        Hud = new CombatHud { Name = "CombatHud" };
        AddChild(Hud);
        Hud.Init(this);
        var cp = _sim.PlaceOf(PlaceKind.Camp);
        _campPlace = cp?.Id ?? -1;
        _camp = new V2(RegionSpec.GoblinCamp.X, RegionSpec.GoblinCamp.Y);
        _sim.GoblinStrike += OnStrike;
    }

    public override void _ExitTree()
    {
        if (Instance == this) Instance = null;
        if (_sim != null) _sim.GoblinStrike -= OnStrike;
        if (Paused && IsInsideTree()) GetTree().Paused = false;
    }

    void OnStrike(Person g)
    {
        if (g.Dead || g.InFight) return;
        if (Fight == null) Start(g, $"{g.Name} pusudan saldırdı!");
        else if (!Summary) AddGoblin(g, true, $"{g.Name} kavgaya daldı.");
    }

    void Say(string text, float secs = 2.5f) { _hint = text; _hintT = secs; }

    // ------------------------------------------------------------------------------------------------ start
    M.Hero HeroOf(Character c)
    {
        if (c.HeroId is int id && _s.Macro.Hero(id) is M.Hero h) return h;
        return new M.Hero { Id = -c.Id, Name = c.Name, Cls = c.Cls, Level = c.Level, Race = c.Race, Stats = new M.JsObj<double>(), Align = c.Align };
    }

    /// <summary>Start a fight with the goblins near the player (the trigger first). Also the "attack" verb (G) and dev.</summary>
    public void Start(Person trigger, string why)
    {
        if (Fight != null || _s?.Player == null) return;
        var pl = _r.Player;
        var ppos = new V2(pl.GlobalPosition.X, pl.GlobalPosition.Z);
        Fight = new Fight(_s.Seed * 7919 + Math.Floor(GameClock.TotalHours * 600));
        _bodies.Clear(); Selected.Clear(); _wake.Clear();
        Outcome = null; Summary = false; Aim = null; Box = null;

        // the party
        var me = pl.Character ?? _s.Player;
        var lead = Fight.AddCharacter(me, HeroOf(me), FSide.Party, ppos, "player");
        lead.Dir = new V2(MathF.Sin(pl.Facing), MathF.Cos(pl.Facing));
        lead.Stable = me.Stable;
        _bodies[lead] = pl;
        foreach (var comp in _r.Companions)
        {
            if (comp.Char.Dead || comp.Char.Captive || comp.Char.Down) continue;
            var cpos = new V2(comp.GlobalPosition.X, comp.GlobalPosition.Z);
            var f = Fight.AddCharacter(comp.Char, HeroOf(comp.Char), FSide.Party, cpos, comp.Char.HeroId != null ? "hero" : "companion");
            f.Kind = "companion";
            f.Stable = comp.Char.Stable;
            f.Dir = new V2(MathF.Sin(comp.Rotation.Y), MathF.Cos(comp.Rotation.Y));
            _bodies[f] = comp;
            comp.Scripted = true;
        }
        // the goblins: the trigger, those chasing, those around; near the camp the whole camp comes (sleepers a few seconds later)
        bool atCamp = V2.Distance(ppos, _camp) < 65f;
        if (trigger != null && trigger.Role == Role.Goblin) AddGoblin(trigger, false, null);
        foreach (var p in _sim.People)
        {
            if (p.Role != Role.Goblin || p.Dead || p.InFight || !p.Present) continue;
            float d = V2.Distance(p.Pos, ppos);
            bool outside = p.Motion != Motion.Inside;
            if (outside && (d < 30f || p.Act?.Kind == ActKind.Chase || (atCamp && d < 70f))) AddGoblin(p, false, null);
            else if (!outside && atCamp && p.Act?.Place == _campPlace)
                _wake.Add((p, 3.5f + (float)H.Hash(p.Id, 77) * 7f, $"{p.Name} gürültüye uyandı ve çadırından fırladı!"));
        }
        Fight.FoeHome = atCamp ? _camp + Norm(_camp - ppos) * 75f : _camp;
        Fight.Begin(why);
        Fight.Ev += OnFightEvent;

        pl.Scripted = true; pl.InputEnabled = false; pl.Velocity = Vector3.Zero;
        Selected.Add(lead);
        // camera: from the player's view up to the tactical one
        var cur = GetViewport().GetCamera3D();
        _camFrom = cur?.GlobalTransform ?? pl.Camera.GlobalTransform;
        var center = FocusPoint();
        Camera.Begin(_r.Heightfield, new Vector3(center.X, _r.Heightfield.Height(center.X, center.Y), center.Y), pl.CameraYaw);
        Camera.MakeCurrent();
        _blend = 0f;
        Input.MouseMode = Input.MouseModeEnum.Visible;
        Hud.Begin(Fight);
        GD.Print($"[Combat] start: {why} — {Fight.Of(FSide.Party).Count()} vs {Fight.Of(FSide.Foe).Count()} (+{_wake.Count} uyuyan)");
        if (Settings.Current.AutoPause && !Unattended) SetPaused(true, "Savaş başladı — duraklatıldı. Emir ver; Boşluk ile sürdür.");
    }

    static V2 Norm(V2 v) { float l = v.Length(); return l > 1e-4f ? v / l : new V2(0, 1); }

    Fighter AddGoblin(Person p, bool join, string why)
    {
        bool boss = p.IsBoss;
        bool archer = !boss && p.Work % 4 == 3;
        string kind = boss ? "boss" : archer ? "archer" : "goblin";
        float range = archer ? 16f : 0f;
        if (p.Motion == Motion.Inside && p.Act != null) p.Pos = p.Act.Target;
        var f = join ? Fight.Reinforce(boss ? "goblinBoss" : "goblin", p.Name, p.Pos, kind, p.Id, range, why ?? $"{p.Name} katıldı.")
                     : Fight.AddMonster(boss ? "goblinBoss" : "goblin", p.Name, FSide.Foe, p.Pos, kind, p.Id, range);
        f.Dir = p.Dir;
        if (p.Act?.Spot != null && p.Act.Spot.TakenBy == p.Id) p.Act.Spot.TakenBy = -1;
        p.InFight = true; p.FightAnim = "Idle"; p.Down = false;
        p.FightTool = archer ? "proc_bow" : null;
        p.Motion = Motion.Doing; p.Path.Clear(); p.Wait = 0;
        p.Note($"{H.Clock(_sim.Now)} kavgaya girdi");
        return f;
    }

    // ------------------------------------------------------------------------------------------------ pause and orders
    public void SetPaused(bool on, string why = null)
    {
        if (Fight == null) return;
        Paused = on;
        Fight.Paused = on;
        GetTree().Paused = on || Summary;
        if (why != null) Say(why, 4f);
    }

    public void TogglePause() { if (Fight != null && !Summary) SetPaused(!Paused); }

    public Fighter Lead => Selected.FirstOrDefault(f => f.Standing && f.Char != null) ?? Selected.FirstOrDefault();

    public IEnumerable<Fighter> Party => Fight?.Of(FSide.Party) ?? Enumerable.Empty<Fighter>();

    void Order(Fighter f, string order, Fighter target = null, V2? point = null, string spell = null)
    {
        if (f == null || !f.Standing || f.Fleeing) return;
        f.Order = order; f.OrderTarget = target; f.OrderSpell = spell;
        if (point is V2 p) f.OrderPoint = p;
        f.Cd = MathF.Max(f.Cd, 0f);
    }

    public void OrderAttack(Fighter t)
    {
        if (t == null || !t.Standing || t.Side != FSide.Foe) return;
        foreach (var f in Selected) Order(f, "attack", t);
        if (Selected.Count > 0) Fight.Post("order", Selected[0], $"Emir: {Names(Selected)} → {t.Name} hedefine saldır.");
    }

    public void OrderMove(V2 at)
    {
        int i = 0, n = Selected.Count(f => f.Standing);
        foreach (var f in Selected)
        {
            if (!f.Standing) continue;
            // loose line across the click point
            float off = (i - (n - 1) * 0.5f) * 1.3f;
            var right = new V2(MathF.Cos(Camera.Yaw), -MathF.Sin(Camera.Yaw));
            Order(f, "move", null, at + right * off);
            i++;
        }
        if (n > 0) Fight.Post("order", Selected[0], $"Emir: {Names(Selected)} oraya yürüsün.");
    }

    public void OrderAll(string order, string text)
    {
        foreach (var f in Selected) Order(f, order);
        if (Selected.Count > 0) Fight.Post("order", Selected[0], $"Emir: {Names(Selected)} {text}.");
    }

    public void ClearOrders()
    {
        foreach (var f in Selected) { f.Order = null; f.OrderTarget = null; }
        if (Selected.Count > 0) Fight.Post("order", Selected[0], $"{Names(Selected)} kendi bildiği gibi dövüşüyor.");
    }

    static string Names(List<Fighter> fs) => fs.Count == 1 ? fs[0].Name : string.Join(", ", fs.Select(f => f.Name));

    /// <summary>Spell n (1-based) of the lead: cast now when the target is clear (hovered, or self for a heal with nothing hovered),
    /// otherwise aim mode.</summary>
    public void SpellKey(int n)
    {
        var f = Lead;
        if (f?.Char == null || !f.Standing) return;
        var list = SpellList(f.Char);
        if (n < 1 || n > list.Count) return;
        var sp = list[n - 1];
        if (sp.Level > 0 && f.Char.Slots <= 0) { Say("Büyü yuvası kalmadı (uzun dinlenmede dolar)."); return; }
        if (Hover != null && ValidTarget(sp, Hover)) { CastOrder(f, sp, Hover); return; }
        Aim = sp; AimCaster = f;
        Say($"{sp.Name}: hedefi tıkla (sağ tık / Esc: vazgeç)", 6f);
    }

    public static List<SpellDef> SpellList(Character c)
    {
        var r = new List<SpellDef>();
        foreach (var id in c.Spells) { var s = Spells.Get(id); if (s != null && s.Level == 0) r.Add(s); }
        foreach (var id in c.Spells) { var s = Spells.Get(id); if (s != null && s.Level > 0) r.Add(s); }
        return r;
    }

    static bool ValidTarget(SpellDef sp, Fighter t) =>
        sp.Kind == SpellKind.Heal ? t.Side == FSide.Party && !t.Dead && !t.Fled : t.Side == FSide.Foe && t.Standing;

    void CastOrder(Fighter f, SpellDef sp, Fighter t)
    {
        Order(f, "cast", t, null, sp.Id);
        Fight.Post("order", f, $"Emir: {f.Name} → {sp.Name} ({t.Name}).");
        Aim = null; AimCaster = null;
    }

    public void PotionKey()
    {
        var f = Lead;
        if (f?.Char == null || !f.Standing) return;
        if (!f.Char.Inv.Has("potion")) { Say($"{f.Name}: iksir yok."); return; }
        var who = Hover != null && Hover.Side == FSide.Party && !Hover.Dead && Hover != f ? Hover : f;
        Order(f, "potion", who);
        Fight.Post("order", f, who == f ? $"Emir: {f.Name} iksir içsin." : $"Emir: {f.Name}, {who.Name}'a iksir içirsin.");
    }

    public void BandageKey()
    {
        var f = Lead;
        if (f?.Char == null || !f.Standing) return;
        if (!f.Char.Inv.Has("bandage")) { Say($"{f.Name}: sargı bezi yok."); return; }
        var who = Hover != null && Hover.Side == FSide.Party && Hover.Down ? Hover
            : Party.Where(o => o.Down && !o.Dead).OrderBy(o => V2.Distance(o.Pos, f.Pos)).FirstOrDefault();
        if (who == null) { Say("Yerde yatan yok."); return; }
        Order(f, "bandage", who);
        Fight.Post("order", f, $"Emir: {f.Name}, {who.Name}'ın yarasını sarsın.");
    }

    public void WindKey()
    {
        var f = Lead;
        if (f?.Char == null || !f.Standing) return;
        if (f.Char.Uses.GetValueOrDefault("secondWind") <= 0) { Say(f.Char.Cls == "fighter" ? "Derin nefes kullanıldı (dinlenince gelir)." : "Yalnız savaşçılar."); return; }
        Order(f, "wind");
    }

    public void SelectNext(int dir)
    {
        var ps = Party.Where(f => !f.Dead && !f.Fled).ToList();
        if (ps.Count == 0) return;
        int i = Selected.Count > 0 ? ps.IndexOf(Selected[0]) : -1;
        i = ((i + dir) % ps.Count + ps.Count) % ps.Count;
        Selected.Clear(); Selected.Add(ps[i]);
    }

    public void Select(Fighter f, bool add)
    {
        if (f == null || f.Side != FSide.Party) return;
        if (!add) Selected.Clear();
        if (add && Selected.Contains(f)) Selected.Remove(f); else Selected.Add(f);
    }

    // ------------------------------------------------------------------------------------------------ input
    public override void _UnhandledInput(InputEvent e)
    {
        if (Fight == null) return;
        if (Summary)
        {
            if (e is InputEventKey sk && sk.Pressed && !sk.Echo && (sk.PhysicalKeycode is Key.Space or Key.Enter or Key.KpEnter or Key.Escape))
            { Continue(); GetViewport().SetInputAsHandled(); }
            return;
        }
        if (e is InputEventKey k && k.Pressed && !k.Echo)
        {
            bool handled = true;
            switch (k.PhysicalKeycode)
            {
                case Key.Space: TogglePause(); break;
                case Key.Tab: SelectNext(k.ShiftPressed ? -1 : 1); break;
                case Key.Escape: if (Aim != null) { Aim = null; Say("Vazgeçildi."); } else handled = false; break;
                case Key.Key1: SpellKey(1); break;
                case Key.Key2: SpellKey(2); break;
                case Key.Key3: SpellKey(3); break;
                case Key.Key4: SpellKey(4); break;
                case Key.Key5: SpellKey(5); break;
                case Key.Q: PotionKey(); break;
                case Key.B: BandageKey(); break;
                case Key.F: WindKey(); break;
                case Key.R: OrderAll("retreat", "geri çekilsin"); break;
                case Key.H: OrderAll("hold", "yerinde beklesin"); break;
                case Key.G: ClearOrders(); break;
                default: handled = false; break;
            }
            if (handled) GetViewport().SetInputAsHandled();
            return;
        }
        if (e is InputEventMouseButton mb)
        {
            if (mb.ButtonIndex == MouseButton.Left)
            {
                if (mb.Pressed) { _pressing = true; _boxing = false; _press = mb.Position; }
                else if (_pressing)
                {
                    _pressing = false;
                    if (_boxing) BoxSelect(new Rect2(_press, mb.Position - _press).Abs(), mb.ShiftPressed);
                    else Click(mb.Position, mb.ShiftPressed, false);
                    _boxing = false; Box = null;
                }
                GetViewport().SetInputAsHandled();
            }
            else if (mb.ButtonIndex == MouseButton.Right)
            {
                if (mb.Pressed) { _rpressing = true; _rpress = mb.Position; }
                else if (_rpressing)
                {
                    _rpressing = false;
                    if ((mb.Position - _rpress).Length() < 6f) Click(mb.Position, false, true);
                }
            }
        }
        else if (e is InputEventMouseMotion mm && _pressing)
        {
            if ((mm.Position - _press).Length() > 8f) _boxing = true;
            if (_boxing) Box = new Rect2(_press, mm.Position - _press).Abs();
        }
    }

    void Click(Vector2 screen, bool shift, bool right)
    {
        var h = PickAt(screen);
        if (Aim != null)
        {
            if (right) { Aim = null; Say("Vazgeçildi."); return; }
            if (h != null && ValidTarget(Aim, h)) CastOrder(AimCaster, Aim, h);
            else Say(Aim.Kind == SpellKind.Heal ? "Bir yoldaşı tıkla." : "Ayakta bir düşmanı tıkla.");
            return;
        }
        if (h != null && h.Side == FSide.Party && !right) { Select(h, shift); return; }
        if (Selected.Count == 0) return;
        if (h != null && h.Side == FSide.Foe && h.Standing) { OrderAttack(h); return; }
        var g = Camera.Ground(screen);
        if (g is Vector3 p) OrderMove(new V2(p.X, p.Z));
    }

    void BoxSelect(Rect2 r, bool add)
    {
        if (!add) Selected.Clear();
        foreach (var f in Party)
        {
            if (f.Dead || f.Fled) continue;
            var s = ScreenOf(f, 0.9f);
            if (s is Vector2 sp && r.HasPoint(sp) && !Selected.Contains(f)) Selected.Add(f);
        }
    }

    public Vector3 WorldOf(Fighter f, float up = 0f) => new(f.Pos.X, _r.Heightfield.Height(f.Pos.X, f.Pos.Y) + up, f.Pos.Y);

    public Vector2? ScreenOf(Fighter f, float up)
    {
        var w = WorldOf(f, up);
        if (Camera.IsPositionBehind(w)) return null;
        return Camera.UnprojectPosition(w);
    }

    Fighter PickAt(Vector2 screen)
    {
        Fighter best = null; float bd = 34f;
        foreach (var f in Fight.F)
        {
            if (f.Dead && f.Side == FSide.Foe || f.Fled) continue;
            foreach (float up in new[] { 0.3f, 0.9f, 1.5f })
            {
                var s = ScreenOf(f, f.Kind is "goblin" or "archer" ? up * 0.75f : up);
                if (s is not Vector2 sp) continue;
                float d = (sp - screen).Length();
                if (d < bd) { bd = d; best = f; }
            }
        }
        return best;
    }

    // ------------------------------------------------------------------------------------------------ frame
    public override void _Process(double delta)
    {
        float dt = (float)Math.Min(delta, 0.1);
        if (_hintT > 0) _hintT -= dt;
        if (!GetTree().Paused) StepRunOff(dt);
        if (Fight == null) return;
        if (_blend < 1f)
        {
            _blend = MathF.Min(1f, _blend + dt / 0.7f);
            float k = _blend * _blend * (3 - 2 * _blend);
            var to = Camera.GlobalTransform;
            Camera.GlobalTransform = _camFrom.InterpolateWith(to, k);
        }
        Hover = Summary ? null : PickAt(GetViewport().GetMousePosition());
        if (!Paused && !Summary)
        {
            Fight.Update(dt);
            for (int i = _wake.Count - 1; i >= 0; i--)
            {
                var (p, at, why) = _wake[i];
                if (Fight.T < at) continue;
                _wake.RemoveAt(i);
                if (!p.Dead && !p.InFight && p.Present && !Fight.Over) AddGoblin(p, true, why);
            }
            if (PauseAt is float pa && Fight.T >= pa) { PauseAt = null; SetPaused(true, "Duraklatıldı."); }
        }
        Selected.RemoveAll(f => f.Dead || f.Fled);
        if (Selected.Count == 0) { var l = Party.FirstOrDefault(f => f.IsPlayer && !f.Dead) ?? Party.FirstOrDefault(f => f.Standing); if (l != null) Selected.Add(l); }
        SyncBodies();
        var c = FocusPoint();
        Camera.Track(new Vector3(c.X, _r.Heightfield.Height(c.X, c.Y), c.Y));
        if (Fight.Over && !Summary) Finish();
    }

    V2 FocusPoint()
    {
        V2 sum = V2.Zero; float w = 0;
        V2 party = V2.Zero; int pn = 0;
        foreach (var f in Fight.F) if (f.Side == FSide.Party && !f.Fled) { party += f.Pos; pn++; }
        if (pn > 0) party /= pn;
        foreach (var f in Fight.F)
        {
            if (f.Fled || f.Fleeing || (f.Side == FSide.Foe && (f.Dead || V2.Distance(f.Pos, party) > 22f))) continue;
            float k = f.Side == FSide.Party ? 2f : 1f;
            sum += f.Pos * k; w += k;
        }
        return w > 0 ? sum / w : party;
    }

    void SyncBodies()
    {
        var hf = _r.Heightfield;
        foreach (var f in Fight.F)
        {
            if (f.LifeId >= 0)
            {
                var p = _sim.People[f.LifeId];
                if (!p.InFight) continue;
                p.Pos = f.Pos; p.Dir = f.Dir; p.FightAnim = f.Anim; p.Down = f.Down; p.Dead = f.Dead;
                if (f.Dead) { p.InFight = false; p.Note($"{H.Clock(_sim.Now)} öldü"); }
                else if (f.Fled) Release(p, f.FleeTo);
                continue;
            }
            if (!_bodies.TryGetValue(f, out var body)) continue;
            var pos3 = new Vector3(f.Pos.X, hf.Height(f.Pos.X, f.Pos.Y) + (body is Player ? 0.05f : 0f), f.Pos.Y);
            float yaw = MathF.Atan2(f.Dir.X, f.Dir.Y);
            string anim = f.Down || f.Dead ? "Die" : f.Anim;
            float sp = f.Moving ? f.Speed : 0f;
            if (body is Player pl) pl.SetPose(pos3, yaw, anim, sp);
            else if (body is Companion c) c.SetPose(pos3, yaw, anim, sp);
        }
    }

    /// <summary>A routed goblin left the fight: it keeps running (driven here) to where it fled, then hides there for a few game hours
    /// (or for good when its camp is gone) and LifeSim takes it back.</summary>
    void Release(Person p, V2 to)
    {
        if (_runOff.Exists(x => x.p == p)) return;
        p.FightAnim = "Run";
        _runOff.Add((p, to, 25f));
    }

    void StepRunOff(float dt)
    {
        for (int i = _runOff.Count - 1; i >= 0; i--)
        {
            var (p, to, left) = _runOff[i];
            var d = to - p.Pos; float L = d.Length();
            left -= dt;
            if (L > 0.5f && left > 0)
            {
                p.Pos += d / L * MathF.Min(L, 4.4f * dt);
                p.Dir = d / L; p.FightAnim = "Run";
                _runOff[i] = (p, to, left);
                continue;
            }
            _runOff.RemoveAt(i);
            p.InFight = false; p.FightAnim = null; p.Running = false;
            bool campGone = _s.Camp == null || !_s.Camp.Alive;
            if (campGone) { p.Present = false; p.Motion = Motion.Inside; p.Act = null; p.NextDecision = double.MaxValue; p.Note($"{H.Clock(_sim.Now)} kamp düştü; kaçıp gitti"); continue; }
            p.Motion = Motion.Inside;
            p.Act = new Activity
            {
                Kind = ActKind.Rest, Inside = true, Place = -1, Target = p.Pos, Until = _sim.Now + 240, Label = "Ormanda saklanıyor",
                Reason = "Bozguna uğradı; tehlike geçene dek ormanda saklanıyor.",
            };
            p.NextDecision = p.Act.Until;
            p.Note($"{H.Clock(_sim.Now)} kaçtı, ormanda saklanıyor");
        }
    }

    void OnFightEvent(FightEvent e) => Hud.OnEvent(e);

    // ------------------------------------------------------------------------------------------------ the end
    void Finish()
    {
        Summary = true;
        Aim = null; Box = null;
        var cp = _s.Camp;
        if (Fight.Winner != FSide.Party) Fight.SettleFallen();
        Fight.WriteBack((int)_s.Macro.W.Day, cp?.Name);
        var o = Outcome = new FightOutcome { Won = Fight.Winner == FSide.Party };
        foreach (var f in Fight.F)
        {
            if (f.Side == FSide.Foe) { if (f.Dead) { o.Killed++; if (f.IsBoss) o.BossKilled = true; } else if (f.Fled) o.Fled++; }
            else { if (f.Dead) o.PartyDead++; else if (f.Down) o.PartyDown++; }
        }
        var partyAll = Party.ToList();
        o.AllDown = partyAll.All(f => f.Dead || f.Down || f.Fled);
        o.XpEach = Fight.XpEach();
        o.CampCleared = o.Won && CampBroken();
        o.Lines.Add(o.Won ? $"{o.Killed} goblin öldü, {o.Fled} goblin kaçtı{(o.BossKilled ? "; şefleri düştü" : "")}."
            : o.Killed > 0 ? $"Ekip yere serildi ({o.Killed} goblin de öldü)." : "Ekip yere serildi.");
        // D: what the goblins do with the fallen
        if (!o.Won) Fate(o, partyAll);
        foreach (var f in partyAll)
        {
            var c = f.Char;
            if (c == null) continue;
            if (c.Dead) { o.Lines.Add($"{c.Name} öldü."); if (c.HeroId is int hid) M.Local.Died(_s.Macro, hid, cp?.Name); continue; }
            if (f.NewWound != null)
            {
                o.Lines.Add($"{c.Name} kalıcı bir yara aldı: {Wound.Name(f.NewWound)} — {Wound.Effect(f.NewWound)} Artık «{c.Epithet}» diye anılıyor.");
                if (c.HeroId is int hid) M.Local.Wounded(_s.Macro, hid, Wound.Name(f.NewWound), c.Epithet);
            }
            if (f.Fled) { o.Lines.Add($"{c.Name} kaçtı."); continue; }
            if (o.XpEach > 0)
            {
                c.Xp += o.XpEach;
                while (Rules.LevelFor(c.Xp) > c.Level) o.Lines.Add(LevelUp(c));
            }
            if (o.Won && c.Down) { c.Down = false; c.Stable = false; c.Hp = Math.Max(1, c.Hp); c.DeathOk = c.DeathFail = 0; o.Lines.Add($"{c.Name} kendine geldi (1 can)."); }
        }
        o.PlayerDead = _s.Player.Dead;
        if (o.PlayerDead)
        {
            var ctl = _r.Player.Character;
            var heir = ctl != null && ctl != _s.Player && !ctl.Dead ? ctl
                : _s.Party.FirstOrDefault(c => c != _s.Player && !c.Dead && !c.Captive) ?? _s.Party.FirstOrDefault(c => c != _s.Player && !c.Dead);
            if (heir == null) { o.GameOver = true; o.Lines.Add("Ekipten kimse kalmadı. Demir mod: bu dünya kapandı."); }
            else { o.NewLeader = heir; o.Lines.Add($"Ekibin başına {heir.Name} geçiyor."); }
        }
        if (o.XpEach > 0) o.Lines.Add($"Her biri {o.XpEach} TP kazandı.");
        WriteMacro(o);
        if (o.GameOver) SaveGame.Delete();
        _s.SyncPlayerToMacro();
        GD.Print($"[Combat] end: {(o.Won ? "zafer" : $"yenilgi ({o.Fate})")} öldürülen {o.Killed} kaçan {o.Fled} şef {o.BossKilled} kamp {(o.CampCleared ? "temizlendi" : "duruyor")} TP {o.XpEach} süre {Fight.T:F0} sn{(o.GameOver ? " — OYUN BİTTİ" : "")}");
        Hud.ShowSummary(o);
        SetPaused(Paused);   // tree pauses while the summary shows
        if (Unattended) CallDeferred(nameof(Continue));
    }

    /// <summary>D: chance that the goblins finish off the fallen (rare) or drag one to their cage (sometimes); otherwise they rob them
    /// and leave them in the forest.</summary>
    public const double FinishOffChance = 0.08, CaptureChance = 0.22;

    void Fate(FightOutcome o, List<Fighter> party)
    {
        var fallen = party.Where(f => f.Char != null && !f.Char.Dead && !f.Fled).ToList();
        if (fallen.Count == 0) return;
        var cp = _s.Camp;
        double r = Fight.Rng.Next();
        if (ForceFate != null) { r = ForceFate == "kill" ? 0 : ForceFate == "capture" ? FinishOffChance + 0.01 : 0.99; }
        bool camp = cp != null && cp.Alive;
        if (r < FinishOffChance)
        {
            o.Fate = "kill";
            foreach (var f in fallen) { f.Char.Dead = true; f.Char.Down = false; }
            o.Lines.Add("Goblinler yerde yatanların işini bitirdi.");
            return;
        }
        if (r < FinishOffChance + CaptureChance && camp)
        {
            o.Fate = "capture";
            var who = fallen.Count == 1 ? fallen[0] : fallen[(int)(Fight.Rng.Next() * fallen.Count) % fallen.Count];
            o.Captive = who.Char;
            who.Char.Captive = true;
            o.Lines.Add($"Goblinler {who.Char.Name} adlı yolcuyu kampa sürükleyip kafese kapattı.");
            M.Local.Captured(_s.Macro, who.Char.HeroId, who.Char.Name);
            var rest = fallen.Where(f => f != who).ToList();
            if (rest.Count > 0) Rob(o, rest, "Ötekileri soyup ormanın kıyısına attılar.");
            else Rob(o, new List<Fighter> { who }, null);
            return;
        }
        o.Fate = "rob";
        Rob(o, fallen, "Goblinler baygın ekibi soyup kampın dışına attı.");
    }

    /// <summary>dev/tests: force the defeat outcome (rob | capture | kill)</summary>
    public string ForceFate;

    /// <summary>The goblins empty the purses and take what glitters: all silver, potions, trinkets and herbs; a weapon half the time,
    /// a shield sometimes. It goes into their camp's chest (found there when the camp falls).</summary>
    void Rob(FightOutcome o, List<Fighter> who, string line)
    {
        int silver = 0, value = 0;
        var taken = new List<string>();
        var heroes = new List<int>();
        foreach (var f in who)
        {
            var c = f.Char;
            if (c.HeroId is int hid) heroes.Add(hid);
            silver += c.Inv.Silver; value += c.Inv.Silver;
            _s.CampChest.Silver += c.Inv.Silver;
            c.Inv.Silver = 0;
            foreach (var id in new[] { "potion", "trinket", "herb" })
            {
                int n = c.Inv.Count(id);
                if (n <= 0) continue;
                c.Inv.Remove(id, n); _s.CampChest.Add(id, n);
                value += (Items.Get(id)?.Price ?? 0) * n;
                taken.Add($"{n} {Items.Get(id)?.Name?.ToLowerInvariant() ?? id}");
            }
            if (c.Weapon != null && Fight.Rng.Chance(0.5)) { var w = c.Weapon; c.Inv.Remove(w); c.Weapon = null; _s.CampChest.Add(w); value += Items.Get(w)?.Price ?? 0; taken.Add($"{c.Name}'ın {Items.Get(w)?.Name?.ToLowerInvariant()}"); }
            if (c.Shield != null && Fight.Rng.Chance(0.4)) { var sh = c.Shield; c.Inv.Remove(sh); c.Shield = null; _s.CampChest.Add(sh); value += Items.Get(sh)?.Price ?? 0; taken.Add("bir kalkan"); }
        }
        if (silver > 0) taken.Insert(0, $"{Rules.Money(silver)}");
        string what = taken.Count > 0 ? string.Join(", ", taken) : "";
        if (line != null) o.Lines.Add(line);
        o.Lines.Add(taken.Count > 0 ? $"Götürdükleri: {what}." : "Götürecek bir şey bulamadılar.");
        M.Local.Robbed(_s.Macro, heroes, value, what);
        _s.Flags["robbedDay"] = _s.Macro.W.Day;
    }

    /// <summary>The camp is broken when its chief fell and no goblin of it still stands outside the fight (the rest scatter), or when
    /// none of its goblins is left at all.</summary>
    bool CampBroken()
    {
        int left = 0; bool bossAlive = false;
        foreach (var p in _sim.People)
        {
            if (p.Role != Role.Goblin || p.Dead || !p.Present) continue;
            if (_runOff.Exists(x => x.p == p)) continue;
            var f = Fight.F.FirstOrDefault(x => x.LifeId == p.Id);
            if (f != null && f.Fled) continue;
            left++;
            if (p.IsBoss) bossAlive = true;
        }
        return left == 0 || (!bossAlive && left <= 1);
    }

    static string LevelUp(Character c)
    {
        c.Level++;
        c.Recalc();
        string extra = "";
        if (c.Cls == "wizard")
        {
            var learn = Spells.WizardFirst.FirstOrDefault(s => !c.Spells.Contains(s));
            if (learn != null) { c.Spells.Add(learn); extra = $", yeni büyü: {Spells.Get(learn).Name}"; }
        }
        return $"{c.Name} {c.Level}. seviyeye yükseldi! (en yüksek can {c.MaxHp}{extra})";
    }

    /// <summary>Goblins killed here are gone from the macro camp too; a broken camp is cleared in the macro world (quest, settlers:
    /// <see cref="M.Local.LocalCampFight"/>).</summary>
    void WriteMacro(FightOutcome o)
    {
        if (o.Killed == 0 && !o.CampCleared && o.Won) return;
        var heroes = new List<int>();
        foreach (var f in Party) if (f.Char?.HeroId is int id && !f.Dead) heroes.Add(id);
        var res = M.Local.LocalCampFight(_s.Macro, new M.Local.LocalFight
        {
            Killed = o.Killed - (o.BossKilled ? 1 : 0), BossKilled = o.BossKilled, Cleared = o.CampCleared, Won = o.Won, Heroes = heroes,
            PlayerDowned = !o.Won,
        });
        if (res != null) o.Lines.Add(res);
    }

    /// <summary>Close the summary: bodies back to their owners, the third-person camera back, the clock runs again.</summary>
    public void Continue()
    {
        if (Fight == null || !Summary) return;
        var o = Outcome;
        Summary = false;
        Paused = false; Fight.Paused = false;
        GetTree().Paused = false;
        // goblins still standing in the fight go back to their lives (after a lost fight: they rob and leave — D)
        foreach (var f in Fight.F)
        {
            if (f.LifeId < 0) continue;
            var p = _sim.People[f.LifeId];
            if (!p.InFight || _runOff.Exists(x => x.p == p)) continue;
            if (f.Dead) { p.InFight = false; continue; }
            p.InFight = false; p.FightAnim = null; p.FightTool = null; p.Down = false; p.Running = false;
            p.Act = null; p.Path.Clear(); p.Motion = Motion.Doing; p.NextDecision = _sim.Now;
        }
        // bodies
        var pl = _r.Player;
        var lead = Fight.F.FirstOrDefault(f => f.IsPlayer);
        pl.Scripted = false; pl.InputEnabled = true;
        if (lead != null) pl.Teleport(lead.Pos.ToGodot(), MathF.Atan2(lead.Dir.X, lead.Dir.Y), _r.Heightfield);
        foreach (var f in Fight.F)
            if (f.Char != null && _bodies.TryGetValue(f, out var b) && b is Companion cb)
                cb.GlobalPosition = new Vector3(f.Pos.X, _r.Heightfield.Height(f.Pos.X, f.Pos.Y), f.Pos.Y);
        pl.Camera.MakeCurrent();
        foreach (var comp in _r.Companions) comp.Scripted = false;
        Input.MouseMode = Input.MouseModeEnum.Captured;
        Fight.Ev -= OnFightEvent;
        Hud.End();
        var done = Fight;
        Fight = null;
        Selected.Clear(); Hover = null;
        if (o.GameOver) { GameOver(); return; }
        if (o.NewLeader != null) PassLeadership(o.NewLeader);
        if (_r.Player.Character.Dead) _r.Party.SwitchControl(1);   // a fallen companion was controlled
        // the fallen are gone from the party (their bodies stay where they fell)
        _s.Party.RemoveAll(c => c.Dead);
        if (!o.Won) Wake(o);
        else SaveGame.Save(_s, "savaş");
        Ended?.Invoke(o);
        GD.Print($"[Combat] closed ({done.Log.Count} olay)");
    }

    /// <summary>D: after a lost fight. Hours later, robbed: the fallen come to at 1 HP outside the camp (or where they fell, if that
    /// was far from it); a caged player wakes behind the bars; the fled come back to the others.</summary>
    void Wake(FightOutcome o)
    {
        double hours = 2.0 + (o.Fate == "capture" ? 1.5 : 0.5);
        GameClock.SetTotalHours(GameClock.TotalHours + hours);
        foreach (var c in _s.Party)
        {
            if (c.Dead) continue;
            if (c.Down || c.Hp <= 0) { c.Down = false; c.Stable = false; c.DeathOk = c.DeathFail = 0; c.Hp = Math.Max(1, c.Hp); }
        }
        var pl = _r.Player;
        var here = new Vector2(pl.GlobalPosition.X, pl.GlobalPosition.Z);
        bool atCamp = (here - RegionSpec.GoblinCamp).Length() < 70f;
        var away = (RegionSpec.CampSpurControl[0] - RegionSpec.GoblinCamp).Normalized();
        var wake = atCamp ? RegionSpec.CampSpurControl[0] + away * 22f : here + new Vector2(3, 3);
        string text;
        if (_r.Player.Character.Captive)
        {
            _r.Captivity.Cage();
            foreach (var comp in _r.Companions) { if (comp.Char.Captive || comp.Char.Dead) continue; comp.GlobalPosition = new Vector3(wake.X, _r.Heightfield.Height(wake.X, wake.Y), wake.Y); comp.Hold = true; }
            text = "Saatler sonra… Demir parmaklıkların ardında uyanıyorsun. Goblinlerin kafesi.\nKapıyı zorlayabilirsin (E) — ya da biri seni kurtarır.";
        }
        else
        {
            pl.Teleport(wake, MathF.Atan2(-away.X, -away.Y), _r.Heightfield);
            foreach (var comp in _r.Companions) { comp.Hold = false; if (!comp.Char.Captive && !comp.Char.Dead) comp.SnapToLeader(); }
            text = o.Captive != null
                ? $"Saatler sonra… Ormanın kıyısında, başın zonklayarak uyanıyorsun. Keseler boş.\n{o.Captive.Name} yok: goblinler onu kafese kapattı."
                : "Saatler sonra… Ormanın kıyısında, başın zonklayarak uyanıyorsun.\nKesen boş; goblinler seni soyup kampın dışına atmış.";
        }
        foreach (var c in _s.Party) if (!c.Dead && c.Wounds.Count > 0 && c.Wounds[^1].Day == _s.Macro.W.Day) text += $"\n{c.Name}: {Wound.Name(c.Wounds[^1].Kind).ToLowerInvariant()} — «{c.Epithet}».";
        _r.Hud.Toast(text, 9f);
        _s.SyncPlayerToMacro();
        SaveGame.Save(_s, "yenilgi");
    }

    /// <summary>D: the leader died and someone of the party lives: they lead now (the world knows them as the player). The player's body
    /// takes their look and place; their companion body becomes the fallen leader's corpse.</summary>
    void PassLeadership(Character heir)
    {
        var old = _s.Player;
        var pl = _r.Player;
        old.IsPlayer = false;
        heir.IsPlayer = true;
        _s.Party.Remove(heir); _s.Party.Insert(0, heir);
        _s.Player = heir;
        _s.PromoteToPlayer(heir);
        if (pl.Character == old)
        {
            // the fallen leader was the controlled body: the heir takes it, the heir's companion body becomes the corpse
            var comp = _r.Companions.FirstOrDefault(c => c.Char == heir);
            var oldPos = pl.GlobalPosition; float oldYaw = pl.Facing;
            Vector3 heirPos = comp?.GlobalPosition ?? oldPos;
            pl.SetCharacter(heir);
            pl.Teleport(new Vector2(heirPos.X, heirPos.Z), oldYaw, _r.Heightfield);
            if (comp != null) { comp.Become(old); comp.GlobalPosition = oldPos; }
        }
        _s.Controlled = pl.Character;
        _r.Hud.Toast($"{old.Name} düştü. Ekibin başında artık {heir.Name} var.", 7f);
    }

    /// <summary>D: everyone is dead. Iron mode: the slot is already gone; back to the title.</summary>
    void GameOver()
    {
        Session.Current = null;
        GetTree().Paused = false;
        Input.MouseMode = Input.MouseModeEnum.Visible;
        GD.Print("[Combat] oyun bitti: dünya silindi");
        if (Unattended) { Ended?.Invoke(Outcome); return; }
        GetTree().ChangeSceneToFile("res://scenes/Boot.tscn");
    }
}

static class V2Ext
{
    public static Vector2 ToGodot(this V2 v) => new(v.X, v.Y);
}
