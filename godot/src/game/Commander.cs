using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using FD.Actors;
using FD.Combat;
using FD.Life;
using FD.Rpg;
using FD.Sim.Life;
using FD.UI;
using FD.World;
using V2 = System.Numerics.Vector2;

namespace FD.Game;

/// <summary>Tur 1 B: something in the world a right click can use (a herb patch, a corpse, the chest, the board, the cage, a door).</summary>
public sealed class ClickTarget
{
    public Vector3 Pos;
    public float Height = 1f, Radius = 0.6f;
    /// <summary>what the hover line says ("Topla: şifalı ot")</summary>
    public string Label;
    public CursorKit.Kind Cursor = CursorKit.Kind.Hand;
    /// <summary>how near the character must walk before <see cref="Act"/></summary>
    public float Reach = 1.8f;
    public Action Act;
    /// <summary>left click (optional)</summary>
    public Action Info;
}

/// <summary>
/// Tur 1 A–C: the player's hand on the game, the same in and out of a fight.
/// <list type="bullet">
/// <item><b>Selection</b>: left click on a party member (Shift adds), drag a box, <b>Tab</b> the next one, <b>1–4</b> straight to one.
/// The selected lead is the body WASD walks (selecting someone hands them the body); with nobody selected (left click on empty
/// ground) WASD pans the camera.</item>
/// <item><b>Right click</b> does what fits the thing under the mouse (the cursor shows it): a goblin — attack; the ground — go
/// there (in a fight: run there); a person — talk; a herb, a corpse, the chest, the board, the cage — take, search, open, read;
/// a door — the house's keeper. Far things are walked to first. <b>Left click</b> on a person: their card.</item>
/// <item><b>Time</b>: <b>Space</b> pauses and resumes anywhere (orders can be given while paused), <b>+ / −</b> set the speed
/// 1× · 2× · 3× (also the buttons by the clock); the speed falls back to 1× when a threat comes into sight (setting).</item>
/// <item><b>Stances</b>: <b>Shift+1…5</b> for the selected (Saldırgan, Savunmada, Yerini koru, Kaç, Pasif), or the party bar.
/// An aggressive member who sees a goblin starts the fight by themselves.</item>
/// <item><b>L</b>: the dice log.</item>
/// </list>
/// </summary>
public partial class Commander : Node
{
    public static Commander Instance { get; private set; }
    Region _r;
    /// <summary>the selected party members (the first is the lead; out of a fight the lead is the player's body)</summary>
    public readonly List<Character> Sel = new();
    /// <summary>things a right click can use (registered by Gathering, Board, Captivity…)</summary>
    public readonly List<Func<IEnumerable<ClickTarget>>> Targets = new();
    public int Speed { get; private set; } = 1;
    public bool IsPaused => IsInsideTree() && GetTree().Paused;
    /// <summary>the hover line under the cursor (what a right click would do)</summary>
    public string HoverText { get; private set; }
    public Rect2? Box { get; private set; }
    bool _pressing, _boxing, _rpressing;
    Vector2 _press, _rpress;
    float _scanT, _threatT;
    (Vector3 at, float t)? _mark;
    Overlay _overlay;
    bool _menuPaused;

    public void Init(Region r)
    {
        Instance = this;
        Name = "Commander";
        ProcessMode = ProcessModeEnum.Always;
        _r = r;
        var cam = new GameCamera { Name = "GameCamera" };
        r.AddChild(cam);
        cam.MakeCurrent();
        cam.Exclude(r.Player.GetRid());
        cam.Target = CameraTarget;
        cam.WasdPans = () => Sel.Count == 0 && PanelLayer.OpenPanel == null;
        cam.Yaw = r.Player.Facing + MathF.PI;
        cam.Focus = r.Player.GlobalPosition;
        cam.SnapNow();
        Player.WasdDrives = () => Sel.Count > 0 && _r.Combat?.Active != true;
        Sel.Add(r.Player.Character);
        _overlay = new Overlay { Name = "CommandOverlay" };
        var layer = new CanvasLayer { Name = "CommandLayer", Layer = 4 };
        AddChild(layer);
        layer.AddChild(_overlay);
        _overlay.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        _overlay.MouseFilter = Control.MouseFilterEnum.Ignore;
        _overlay.C = this;
        Targets.Add(DoorTargets);
        if (r.Gathering != null) Targets.Add(r.Gathering.ClickTargets);
        if (r.Board != null) Targets.Add(r.Board.ClickTargets);
        if (r.Captivity != null) Targets.Add(r.Captivity.ClickTargets);
        Engine.TimeScale = 1;
    }

    public override void _ExitTree()
    {
        if (Instance == this) Instance = null;
        Engine.TimeScale = 1;
        Player.WasdDrives = null;
        CursorKit.Set(CursorKit.Kind.Arrow);
    }

    Vector3? CameraTarget()
    {
        var cd = _r.Combat;
        if (cd?.Active == true)
        {
            var lead = cd.Lead;
            if (lead != null && !lead.Fled) return cd.WorldOf(lead);
            var c = cd.FocusPoint();
            return new Vector3(c.X, _r.Heightfield.Height(c.X, c.Y), c.Y);
        }
        return _r.Player.GlobalPosition;
    }

    // ------------------------------------------------------------------------------------------------ time
    public void SetPaused(bool on, string why = null)
    {
        GetTree().Paused = on;
        if (why != null) _r.Hud?.Toast(why, 3f);
    }

    public void TogglePause() => SetPaused(!IsPaused);

    public void SetSpeed(int n, string why = null)
    {
        Speed = Math.Clamp(n, 1, 3);
        Engine.TimeScale = Speed;
        _r.Hud?.Toast(why ?? $"Hız {Speed}×", 1.6f);
    }

    /// <summary>the game menu pauses the game while it is open</summary>
    public void MenuOpened() { _menuPaused = !IsPaused; if (_menuPaused) GetTree().Paused = true; }
    public void MenuClosed() { if (_menuPaused) GetTree().Paused = false; _menuPaused = false; }

    // ------------------------------------------------------------------------------------------------ selection
    public bool IsSelected(Character c) => Sel.Contains(c);
    public Character Lead => Sel.FirstOrDefault();

    /// <summary>the living party in order (the bar, 1–4, Tab)</summary>
    public List<Character> Members() => _r.Session.Party.Where(c => !c.Dead).ToList();

    public void Select(Character c, bool add)
    {
        if (c == null || c.Dead) return;
        var cd = _r.Combat;
        if (!add) Sel.Clear();
        if (add && Sel.Contains(c)) Sel.Remove(c); else if (!Sel.Contains(c)) Sel.Add(c);
        if (cd?.Active == true)
        {
            cd.Selected.Clear();
            foreach (var s in Sel) { var f = cd.Fight.F.FirstOrDefault(x => x.Char == s && !x.Dead && !x.Fled); if (f != null) cd.Selected.Add(f); }
            return;
        }
        // out of a fight the lead walks with WASD: the player's body becomes theirs
        var lead = Sel.FirstOrDefault(x => !x.Down && !x.Captive);
        if (lead != null && lead != _r.Player.Character && _r.Party.SwitchTo(lead)) GameCamera.Instance?.SnapNow();
        if (lead != null && Sel[0] != lead) { Sel.Remove(lead); Sel.Insert(0, lead); }
    }

    public void ClearSelection()
    {
        Sel.Clear();
        _r.Combat?.Selected.Clear();
    }

    void SelectIndex(int i)
    {
        var m = Members();
        if (i < 0 || i >= m.Count) return;
        bool again = Sel.Count == 1 && Sel[0] == m[i];
        Select(m[i], false);
        if (again && GameCamera.Instance != null) GameCamera.Instance.Follow = true;
    }

    void SelectNext(int dir)
    {
        var m = Members().Where(c => !c.Down && !c.Captive).ToList();
        if (m.Count == 0) return;
        int i = Sel.Count > 0 ? m.IndexOf(Sel[0]) : -1;
        i = ((i + dir) % m.Count + m.Count) % m.Count;
        Select(m[i], false);
        if (GameCamera.Instance != null) GameCamera.Instance.Follow = true;
    }

    public void SetStance(string st, IEnumerable<Character> who = null)
    {
        var list = (who ?? Sel).ToList();
        if (list.Count == 0) return;
        foreach (var c in list) c.Stance = st;
        _r.Hud?.Toast($"{(list.Count == 1 ? list[0].Name : "Seçilenler")}: {Stances.Name(st)} — {Stances.Desc(st)}", 2.5f);
    }

    /// <summary>a fight began: the speed comes down if the setting says so</summary>
    public void OnFightStart()
    {
        if (Speed > 1 && Settings.Current.SlowOnThreat) SetSpeed(1, "Savaş — hız 1×");
    }

    /// <summary>a fight ended: the selection goes on with those who are still up</summary>
    public void OnFightEnd(List<Character> selected)
    {
        Sel.Clear();
        foreach (var c in selected) if (!c.Dead && _r.Session.Party.Contains(c)) Sel.Add(c);
        if (Sel.Count == 0 && _r.Player.Character != null) Sel.Add(_r.Player.Character);
        var lead = Sel.FirstOrDefault(x => !x.Down && !x.Captive);
        if (lead != null && lead != _r.Player.Character) _r.Party.SwitchTo(lead);
        if (!Sel.Contains(_r.Player.Character)) Sel.Insert(0, _r.Player.Character);
        GameCamera.Instance?.SnapNow();
    }

    /// <summary>the party bar's "bekle / gel"</summary>
    public void ToggleWait(Character c)
    {
        var comp = _r.Companions.FirstOrDefault(x => x.Char == c);
        if (comp == null) { _r.Hud?.Toast("Kontrol ettiğin karakter bekleyemez; önce başka birini seç.", 2.5f); return; }
        comp.Waiting = !comp.Waiting;
        _r.Hud?.Toast(comp.Waiting ? $"{c.Name} burada bekleyecek." : $"{c.Name} peşinden geliyor.", 2f);
    }

    public bool IsWaiting(Character c) => _r.Companions.FirstOrDefault(x => x.Char == c)?.Waiting == true;

    // ------------------------------------------------------------------------------------------------ input
    public override void _UnhandledInput(InputEvent e)
    {
        if (_r?.Player == null) return;
        var cd = _r.Combat;
        if (e is InputEventKey k && k.Pressed && !k.Echo)
        {
            if (PanelLayer.OpenPanel is GameMenu) return;   // the menu keeps the game paused
            if (cd?.Summary == true && k.PhysicalKeycode is Key.Space) { cd.Continue(); GetViewport().SetInputAsHandled(); return; }
            if (k.PhysicalKeycode == Key.Space) { TogglePause(); GetViewport().SetInputAsHandled(); return; }
            if (k.Unicode == '+' || k.Keycode is Key.Plus or Key.KpAdd) { SetSpeed(Speed + 1); GetViewport().SetInputAsHandled(); return; }
            if (k.Unicode == '-' || k.Keycode is Key.Minus or Key.KpSubtract) { SetSpeed(Speed - 1); GetViewport().SetInputAsHandled(); return; }
            if (PanelLayer.OpenPanel != null) return;
            if (k.PhysicalKeycode >= Key.Key1 && k.PhysicalKeycode <= Key.Key5 && k.ShiftPressed)
            {
                SetStance(Stances.All[(int)(k.PhysicalKeycode - Key.Key1)]);
                GetViewport().SetInputAsHandled(); return;
            }
            if (k.PhysicalKeycode >= Key.Key1 && k.PhysicalKeycode <= Key.Key4 && !k.CtrlPressed && !k.AltPressed)
            {
                SelectIndex((int)(k.PhysicalKeycode - Key.Key1));
                GetViewport().SetInputAsHandled(); return;
            }
            if (k.PhysicalKeycode == Key.Tab) { SelectNext(k.ShiftPressed ? -1 : 1); GetViewport().SetInputAsHandled(); return; }
            if (k.PhysicalKeycode == Key.L) { CombatHud.ShowLog = !CombatHud.ShowLog; GetViewport().SetInputAsHandled(); return; }
            return;
        }
        if (PanelLayer.OpenPanel != null || _r.Hud?.MapOpen == true) return;
        if (e is InputEventMouseButton mb)
        {
            if (mb.ButtonIndex == MouseButton.Left && !mb.AltPressed && GameCamera.Instance?.Rotating != true)
            {
                if (mb.Pressed) { _pressing = true; _boxing = false; _press = mb.Position; }
                else if (_pressing)
                {
                    _pressing = false;
                    if (_boxing) BoxSelect(new Rect2(_press, mb.Position - _press).Abs(), mb.ShiftPressed);
                    else LeftClick(mb.Position, mb.ShiftPressed);
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
                    if ((mb.Position - _rpress).Length() < 8f) RightClick(mb.Position);
                }
                GetViewport().SetInputAsHandled();
            }
        }
        else if (e is InputEventMouseMotion mm && _pressing)
        {
            if ((mm.Position - _press).Length() > 8f) _boxing = true;
            if (_boxing) Box = new Rect2(_press, mm.Position - _press).Abs();
        }
    }

    // ------------------------------------------------------------------------------------------------ picking
    enum PickKind { None, Member, Fighter, Person, Target }

    struct Pick
    {
        public PickKind Kind;
        public Character Member;
        public Fighter Fighter;
        public Person Person;
        public ClickTarget Target;
        public float D;
    }

    static float SegDist(Vector2 p, Vector2 a, Vector2 b)
    {
        var ab = b - a; float L2 = ab.LengthSquared();
        float t = L2 > 0 ? Math.Clamp((p - a).Dot(ab) / L2, 0f, 1f) : 0f;
        return p.DistanceTo(a + ab * t);
    }

    /// <summary>screen distance from the mouse to a standing capsule (feet → head), or ∞ when off screen</summary>
    float Hit(Vector2 m, Vector3 feet, float height, float radius)
    {
        var cam = GameCamera.Instance;
        if (cam == null) return float.MaxValue;
        var a = cam.Screen(feet + Vector3.Up * 0.1f);
        var b = cam.Screen(feet + Vector3.Up * height);
        if (a is not Vector2 sa || b is not Vector2 sb) return float.MaxValue;
        var right = cam.GlobalBasis.X;
        var r = cam.Screen(feet + Vector3.Up * height * 0.5f + right * radius);
        var mid = cam.Screen(feet + Vector3.Up * height * 0.5f);
        float rad = r is Vector2 sr && mid is Vector2 sm ? sr.DistanceTo(sm) : 10f;
        float d = SegDist(m, sa, sb);
        return d <= MathF.Max(12f, rad + 4f) ? d : float.MaxValue;
    }

    Pick PickAt(Vector2 m)
    {
        var best = new Pick { Kind = PickKind.None, D = float.MaxValue };
        void Offer(Pick p) { if (p.D < best.D) best = p; }
        var cd = _r.Combat;
        bool fight = cd?.Active == true;
        if (fight)
        {
            var f = cd.PickAt(m);
            if (f != null && !(f.Dead && f.Side == FSide.Foe)) Offer(new Pick { Kind = PickKind.Fighter, Fighter = f, D = 1f });
        }
        else
        {
            // the party's bodies
            var pl = _r.Player;
            if (pl.Visible) Offer(new Pick { Kind = PickKind.Member, Member = pl.Character, D = Hit(m, pl.GlobalPosition, 1.8f, 0.4f) });
            foreach (var comp in _r.Companions)
                if (!comp.Char.Dead && comp.Visible) Offer(new Pick { Kind = PickKind.Member, Member = comp.Char, D = Hit(m, comp.GlobalPosition, 1.8f, 0.4f) });
        }
        // people (not in the fight's hands)
        var life = _r.LifeWorld;
        if (life != null)
            foreach (var p in _r.Life.People)
            {
                if (!p.Visible || p.Dead || p.InFight) continue;
                var a = life.ActorOf(p);
                if (a == null || !a.Visible) continue;
                float h = p.Role == Role.Goblin ? 1.35f : p.Role == Role.Child ? 1.2f : 1.75f;
                Offer(new Pick { Kind = PickKind.Person, Person = p, D = Hit(m, a.GlobalPosition, h, 0.4f) + 2f });
            }
        // things
        if (!fight)
            foreach (var src in Targets)
                foreach (var t in src())
                    Offer(new Pick { Kind = PickKind.Target, Target = t, D = Hit(m, t.Pos, t.Height, t.Radius) + 10f });   // people first
        return best;
    }

    bool Hostile(Person p) => p.Role == Role.Goblin && !p.Dead;

    /// <summary>what the cursor and the hover line say for a pick</summary>
    (CursorKit.Kind cursor, string text) Describe(Pick p, bool overGround)
    {
        switch (p.Kind)
        {
            case PickKind.Member: return (CursorKit.Kind.Arrow, $"{p.Member.Name} — sol tık: seç");
            case PickKind.Fighter:
                return p.Fighter.Side == FSide.Foe ? (CursorKit.Kind.Sword, $"Saldır: {p.Fighter.Name}  ({Math.Max(0, (int)p.Fighter.Hp)}/{(int)p.Fighter.MaxHp})")
                    : (CursorKit.Kind.Arrow, $"{p.Fighter.Name} — sol tık: seç");
            case PickKind.Person:
                return Hostile(p.Person) ? (CursorKit.Kind.Sword, $"Saldır: {p.Person.Name}") : (CursorKit.Kind.Mouth, $"Konuş: {p.Person.FullName} — {Census.RoleName(p.Person)}");
            case PickKind.Target: return (p.Target.Cursor, p.Target.Label);
        }
        return overGround ? (CursorKit.Kind.Foot, null) : (CursorKit.Kind.Arrow, null);
    }

    // ------------------------------------------------------------------------------------------------ clicks
    /// <summary>tests: a click at a screen point (as the mouse would)</summary>
    public void LeftClickAt(Vector2 m, bool shift = false) => LeftClick(m, shift);
    public void RightClickAt(Vector2 m) => RightClick(m);
    public void BoxAt(Rect2 r, bool add = false) => BoxSelect(r, add);
    /// <summary>tests: what the cursor would show over a screen point</summary>
    public CursorKit.Kind CursorAt(Vector2 m)
    {
        var p = PickAt(m);
        bool ground = p.Kind == PickKind.None && GameCamera.Instance?.Ground(m) != null;
        return Describe(p, ground).cursor;
    }

    void LeftClick(Vector2 m, bool shift)
    {
        var p = PickAt(m);
        var cd = _r.Combat;
        switch (p.Kind)
        {
            case PickKind.Member: Select(p.Member, shift); return;
            case PickKind.Fighter:
                if (p.Fighter.Side == FSide.Party && p.Fighter.Char != null) Select(p.Fighter.Char, shift);
                else if (p.Fighter.LifeId >= 0) _r.Hud.OpenCardFor(_r.Life.People[p.Fighter.LifeId]);
                return;
            case PickKind.Person: _r.Hud.OpenCardFor(p.Person, talk: false); return;
            case PickKind.Target: p.Target.Info?.Invoke(); return;
        }
        if (!shift) ClearSelection();
    }

    void EnsureSelection()
    {
        if (Sel.Count > 0) return;
        Select(_r.Player.Character, false);
    }

    void RightClick(Vector2 m)
    {
        var p = PickAt(m);
        var cd = _r.Combat;
        EnsureSelection();
        if (cd?.Active == true)
        {
            if (cd.Summary) return;
            if (p.Kind == PickKind.Fighter && p.Fighter.Side == FSide.Foe && p.Fighter.Standing) { cd.OrderAttack(p.Fighter); return; }
            if (p.Kind == PickKind.Person && Hostile(p.Person))
            {
                var f = cd.Fight.F.FirstOrDefault(x => x.LifeId == p.Person.Id);
                if (f != null) { cd.OrderAttack(f); return; }
            }
            if (GameCamera.Instance?.Ground(m) is Vector3 g) { cd.OrderMove(new V2(g.X, g.Z)); Mark(g); }
            return;
        }
        var pl = _r.Player;
        if (!pl.InputEnabled || pl.Character?.Captive == true && p.Kind != PickKind.Target) { if (pl.Character?.Captive == true) _r.Hud.Toast("Kafestesin. Kapıyı zorla (kapıya sağ tık).", 3f); return; }
        // the companions of the selection come along
        foreach (var comp in _r.Companions) if (Sel.Contains(comp.Char)) comp.Waiting = false;
        switch (p.Kind)
        {
            case PickKind.Person:
            {
                var who = p.Person;
                if (Hostile(who)) { Attack(who); return; }
                Approach(() => new Vector2(who.Pos.X, who.Pos.Y), 2.8f, () => { if (who.Visible) _r.Hud.OpenCardFor(who, talk: true); }, $"{who.Name} ile konuşmaya gidiyor");
                return;
            }
            case PickKind.Target:
            {
                var t = p.Target;
                Approach(() => new Vector2(t.Pos.X, t.Pos.Z), t.Reach, t.Act, null);
                return;
            }
            case PickKind.Member: return;
        }
        if (GameCamera.Instance?.Ground(m) is Vector3 gp)
        {
            pl.GoTo(new Vector2(gp.X, gp.Z), 0.45f);
            Mark(gp);
        }
    }

    /// <summary>walk there first if needed, then act</summary>
    public void Approach(Func<Vector2> where, float reach, Action act, string note)
    {
        var pl = _r.Player;
        var here = new Vector2(pl.GlobalPosition.X, pl.GlobalPosition.Z);
        if (here.DistanceTo(where()) <= reach) { act?.Invoke(); return; }
        pl.GoTo(where, reach, act);
        var g = where();
        Mark(new Vector3(g.X, _r.Heightfield.Height(g.X, g.Y), g.Y));
    }

    /// <summary>the party's own attack on a goblin: within sight it starts at once (the selected go for it), farther the lead walks
    /// up first</summary>
    public void Attack(Person g)
    {
        var pl = _r.Player;
        void Go() { if (g.Visible && !g.Dead && _r.Combat?.Active != true) _r.Combat.Start(g, $"{pl.Character.Name} saldırıya geçti!", attack: true); }
        Approach(() => new Vector2(g.Pos.X, g.Pos.Y), 18f, Go, null);
    }

    void BoxSelect(Rect2 r, bool add)
    {
        var cd = _r.Combat;
        var inside = new List<Character>();
        if (cd?.Active == true) inside.AddRange(cd.InBox(r).Where(f => f.Char != null).Select(f => f.Char));
        else
        {
            var cam = GameCamera.Instance;
            if (cam?.Screen(_r.Player.GlobalPosition + Vector3.Up * 0.9f) is Vector2 sp && r.HasPoint(sp)) inside.Add(_r.Player.Character);
            foreach (var comp in _r.Companions)
                if (!comp.Char.Dead && cam?.Screen(comp.GlobalPosition + Vector3.Up * 0.9f) is Vector2 cp && r.HasPoint(cp)) inside.Add(comp.Char);
        }
        if (!add) Sel.Clear();
        // the controlled body first (it keeps being the lead when it is in the box)
        inside.Sort((a, b) => (b == _r.Player.Character).CompareTo(a == _r.Player.Character));
        foreach (var c in inside) if (!Sel.Contains(c)) Sel.Add(c);
        if (Sel.Count == 0) { ClearSelection(); return; }
        if (cd?.Active == true) { cd.Selected.Clear(); foreach (var c in Sel) { var f = cd.Fight.F.FirstOrDefault(x => x.Char == c && !x.Dead && !x.Fled); if (f != null) cd.Selected.Add(f); } return; }
        var lead = Sel.FirstOrDefault(x => !x.Down && !x.Captive);
        if (lead != null && lead != _r.Player.Character && _r.Party.SwitchTo(lead)) GameCamera.Instance?.SnapNow();
        if (lead != null && Sel[0] != lead) { Sel.Remove(lead); Sel.Insert(0, lead); }
    }

    void Mark(Vector3 at) => _mark = (at, 0f);

    // ------------------------------------------------------------------------------------------------ doors (Tur 1 B: "kapı → gir")
    IEnumerable<ClickTarget> DoorTargets()
    {
        foreach (var pl in _r.Life.Places)
        {
            if (pl.Kind is not (PlaceKind.Inn or PlaceKind.Smithy or PlaceKind.Chapel or PlaceKind.Home)) continue;
            var d = pl.Door;
            var pos = new Vector3(d.X, _r.Heightfield.Height(d.X, d.Y), d.Y);
            var place = pl;
            string what = pl.Kind switch { PlaceKind.Inn => "Hana gir", PlaceKind.Smithy => "Demirhaneye gir", PlaceKind.Chapel => "Tapınağa gir", _ => "Kapı" };
            yield return new ClickTarget { Pos = pos, Height = 2.2f, Radius = 0.9f, Label = $"{what}: {place.Name}", Cursor = CursorKit.Kind.Hand, Reach = 2.6f, Act = () => Enter(place) };
        }
    }

    /// <summary>"Enter" a building: there are no rooms inside yet, so the door brings out whoever keeps the place (the innkeeper, the
    /// smith, the priest) to talk and trade; a home's door stays shut.</summary>
    void Enter(Place pl)
    {
        Role? role = pl.Kind switch { PlaceKind.Inn => Role.Innkeeper, PlaceKind.Smithy => Role.Smith, PlaceKind.Chapel => Role.Priest, _ => null };
        if (role == null)
        {
            var home = _r.Life.People.FirstOrDefault(p => p.Home == pl.Id && p.Visible && !p.Dead && new Vector2(p.Pos.X, p.Pos.Y).DistanceTo(new Vector2(pl.Door.X, pl.Door.Y)) < 12f);
            if (home != null) { _r.Hud.OpenCardFor(home, talk: true); return; }
            _r.Hud.Toast($"{pl.Name}: kapı kapalı. İçeriden ses gelmiyor.", 2.5f);
            return;
        }
        var keeper = _r.Life.People.Where(p => p.Role == role && !p.Dead && p.Present).OrderBy(p => V2.Distance(p.Pos, pl.Door)).FirstOrDefault()
                     ?? (role == Role.Smith ? _r.Life.People.FirstOrDefault(p => p.Role == Role.Apprentice && !p.Dead && p.Present) : null);
        if (keeper == null) { _r.Hud.Toast($"{pl.Name}: bakan kimse yok.", 2.5f); return; }
        if (keeper.Visible && V2.Distance(keeper.Pos, pl.Door) < 25f) { _r.Hud.OpenCardFor(keeper, talk: true); return; }
        // inside, or away: they come to the door
        _r.Hud.OpenCardFor(keeper, talk: true);
        _r.Hud.Toast($"{keeper.Name} kapıya geldi.", 2f);
    }

    // ------------------------------------------------------------------------------------------------ frame
    public override void _Process(double delta)
    {
        if (_r?.Player == null) return;
        float real = (float)delta / MathF.Max(0.05f, (float)Engine.TimeScale);
        var cd = _r.Combat;
        Sel.RemoveAll(c => c.Dead || !_r.Session.Party.Contains(c));
        // WASD in a fight: the lead runs where the keys point
        if (cd?.Active == true && !IsPaused && Sel.Count > 0 && PanelLayer.OpenPanel == null)
        {
            var inp = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
            if (inp.LengthSquared() > 0.01f && GameCamera.Instance is GameCamera cam)
            {
                var fwd = new Vector2(-MathF.Sin(cam.Yaw), -MathF.Cos(cam.Yaw));
                var right = new Vector2(MathF.Cos(cam.Yaw), -MathF.Sin(cam.Yaw));
                var w = (right * inp.X - fwd * inp.Y).Normalized();
                cd.Steer(new V2(w.X, w.Y));
                cam.Follow = true;
            }
        }
        else if (Sel.Count > 0 && Input.GetVector("move_left", "move_right", "move_forward", "move_back").LengthSquared() > 0.01f && PanelLayer.OpenPanel == null && GameCamera.Instance != null)
            GameCamera.Instance.Follow = true;
        // hover: cursor and line
        var m = GetViewport().GetMousePosition();
        var hovered = GetViewport().GuiGetHoveredControl();
        if (hovered != null && hovered.MouseFilter != Control.MouseFilterEnum.Ignore || PanelLayer.OpenPanel != null || _r.Hud?.MapOpen == true)
        {
            CursorKit.Set(CursorKit.Kind.Arrow); HoverText = null;
        }
        else
        {
            var p = PickAt(m);
            bool ground = p.Kind == PickKind.None && Sel.Count > 0 && GameCamera.Instance?.Ground(m) != null;
            var (cur, text) = Describe(p, ground);
            CursorKit.Set(cur);
            HoverText = text;
        }
        if (_mark is var (at, t)) { t += real; _mark = t > 0.7f ? null : (at, t); }
        if (!IsPaused)
        {
            _scanT -= real;
            if (_scanT <= 0) { _scanT = 0.3f; Scan(); }
        }
        _overlay.QueueRedraw();
    }

    /// <summary>aggressive members start fights with goblins they see; a threat in sight brings the speed down</summary>
    void Scan()
    {
        var cd = _r.Combat;
        if (cd == null || _r.Life == null) return;
        var pl = _r.Player;
        var me = new V2(pl.GlobalPosition.X, pl.GlobalPosition.Z);
        Person threat = null; float td = 40f;
        foreach (var p in _r.Life.People)
        {
            if (!Hostile(p) || !p.Visible || p.InFight) continue;
            float d = V2.Distance(p.Pos, me);
            bool chasing = p.Act?.Kind == ActKind.Chase;
            if ((d < 26f || (chasing && d < 40f)) && d < td) { threat = p; td = d; }
        }
        if (threat != null && Speed > 1 && Settings.Current.SlowOnThreat) SetSpeed(1, $"Tehlike: {threat.Name} yakında — hız 1×");
        if (cd.Active || pl.Character == null || pl.Character.Captive || !pl.InputEnabled) return;
        // aggressive stance: anyone of the party who sees a goblin within 22 m goes for it
        var bodies = new List<(Character c, V2 at)> { (pl.Character, me) };
        foreach (var comp in _r.Companions) if (!comp.Char.Dead && !comp.Char.Down && !comp.Char.Captive) bodies.Add((comp.Char, new V2(comp.GlobalPosition.X, comp.GlobalPosition.Z)));
        foreach (var (c, at) in bodies)
        {
            if (c.Stance != Stances.Aggressive || c.Down) continue;
            foreach (var p in _r.Life.People)
            {
                if (!Hostile(p) || !p.Visible || p.InFight || V2.Distance(p.Pos, at) > 22f) continue;
                cd.Start(p, $"{c.Name} {p.Name} hedefini gördü ve saldırdı! (duruş: Saldırgan)", attack: c == pl.Character || Sel.Contains(c));
                return;
            }
        }
    }

    /// <summary>box, click marker and the hover line</summary>
    public partial class Overlay : Control
    {
        public Commander C;
        public override void _Draw()
        {
            if (C == null) return;
            var font = ThemeDB.FallbackFont;
            if (C.Box is Rect2 box)
            {
                DrawRect(box, new Color(0.6f, 0.85f, 0.5f, 0.1f));
                DrawRect(box, new Color(0.6f, 0.85f, 0.5f, 0.75f), false, 1.5f);
            }
            var cam = GameCamera.Instance;
            if (C._mark is var (at, t) && cam != null)
            {
                float r = 0.25f + t * 0.9f;
                var pts = new Vector2[25];
                bool ok = true;
                for (int i = 0; i <= 24; i++)
                {
                    float a = i * MathF.Tau / 24f;
                    var w = at + new Vector3(MathF.Cos(a) * r, 0.05f, MathF.Sin(a) * r);
                    w.Y = (Region.Current?.Heightfield.Height(w.X, w.Z) ?? w.Y) + 0.05f;
                    if (cam.Screen(w) is Vector2 s) pts[i] = s; else { ok = false; break; }
                }
                if (ok) DrawPolyline(pts, new Color(0.85f, 0.8f, 0.55f, 1f - t / 0.7f), 2f, true);
            }
            // the selected out of a fight: a faint ring at the feet
            if (C._r.Combat?.Active != true && cam != null)
            {
                foreach (var c in C.Sel)
                {
                    Node3D body = c == C._r.Player.Character ? C._r.Player : C._r.Companions.FirstOrDefault(x => x.Char == c);
                    if (body == null) continue;
                    var p = body.GlobalPosition;
                    var pts = new Vector2[21];
                    bool ok = true;
                    for (int i = 0; i <= 20; i++)
                    {
                        float a = i * MathF.Tau / 20f;
                        var w = p + new Vector3(MathF.Cos(a) * 0.55f, 0.06f, MathF.Sin(a) * 0.55f);
                        if (cam.Screen(w) is Vector2 s) pts[i] = s; else { ok = false; break; }
                    }
                    if (ok) DrawPolyline(pts, new Color(0.5f, 0.95f, 0.4f, 0.55f), 1.5f, true);
                }
            }
            if (!string.IsNullOrEmpty(C.HoverText))
            {
                var m = GetLocalMousePosition() + new Vector2(20, 30);
                DrawStringOutline(font, m, C.HoverText, HorizontalAlignment.Left, -1, 15, 5, new Color(0.04f, 0.03f, 0.02f, 0.85f));
                DrawString(font, m, C.HoverText, HorizontalAlignment.Left, -1, 15, new Color(0.95f, 0.92f, 0.84f));
            }
        }
    }
}
