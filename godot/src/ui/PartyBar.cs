using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using FD.Game;
using FD.Rpg;
using FD.World;

namespace FD.UI;

/// <summary>
/// Tur 1 A/C: the party bar (bottom left), the same in and out of a fight. One row per member: name, class and level, health (the
/// fight's numbers during a fight), spell slots, state; the five stance buttons (Saldırgan · Savunmada · Yerini koru · Kaç · Pasif);
/// "bekle / gel" for the companions. A click on a row selects that member (Shift adds), a double click also brings the camera.
/// </summary>
public partial class PartyBar : PanelContainer
{
    Region _r;
    VBoxContainer _list;
    string _key;
    float _t;
    readonly List<Row> _rows = new();

    sealed class Row
    {
        public Character C;
        public PanelContainer Panel;
        public Label Name, State, HpText;
        public ProgressBar Hp;
        public Button[] Stance;
        public Button Wait;
    }

    public void Init(Region r)
    {
        _r = r;
        Theme = Ui.MakeTheme(14);
        MouseFilter = MouseFilterEnum.Ignore;
        AddThemeStyleboxOverride("panel", new StyleBoxEmpty());
        SetAnchorsPreset(LayoutPreset.TopLeft);
        _list = new VBoxContainer { MouseFilter = MouseFilterEnum.Ignore };
        _list.AddThemeConstantOverride("separation", 5);
        AddChild(_list);
    }

    public override void _Process(double delta)
    {
        if (_r?.Session == null) return;
        _t -= (float)delta;
        if (_t > 0) return;
        _t = 0.12f;
        var members = _r.Session.Party.Where(c => !c.Dead).ToList();
        string key = string.Join(",", members.Select(c => c.Id)) + "|" + _r.Companions.Count;
        if (key != _key) Rebuild(members, key);
        Refresh();
        // bottom left, growing upward with the rows
        var min = GetCombinedMinimumSize();
        Size = min;
        Position = new Vector2(14, GetParentAreaSize().Y - min.Y - 14);
    }

    void Rebuild(List<Character> members, string key)
    {
        _key = key;
        foreach (var r in _rows) r.Panel.QueueFree();
        _rows.Clear();
        int idx = 1;
        foreach (var c in members)
        {
            var row = new Row { C = c };
            row.Panel = new PanelContainer { MouseFilter = MouseFilterEnum.Stop, CustomMinimumSize = new Vector2(330, 0) };
            var who = c;
            row.Panel.GuiInput += ev =>
            {
                if (ev is InputEventMouseButton mb && mb.Pressed && mb.ButtonIndex == MouseButton.Left)
                {
                    Commander.Instance?.Select(who, mb.ShiftPressed);
                    if (mb.DoubleClick && FD.Actors.GameCamera.Instance != null) FD.Actors.GameCamera.Instance.Follow = true;
                    row.Panel.AcceptEvent();
                }
            };
            var v = new VBoxContainer { MouseFilter = MouseFilterEnum.Ignore };
            v.AddThemeConstantOverride("separation", 2);
            row.Panel.AddChild(v);
            var head = new HBoxContainer { MouseFilter = MouseFilterEnum.Ignore };
            v.AddChild(head);
            row.Name = Ui.Label("", 15, Ui.Gold);
            row.Name.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            head.AddChild(row.Name);
            row.State = Ui.Label("", 12, Ui.Dim);
            head.AddChild(row.State);
            var hb = new Control { CustomMinimumSize = new Vector2(310, 14), MouseFilter = MouseFilterEnum.Ignore };
            row.Hp = new ProgressBar { MinValue = 0, MaxValue = 1, ShowPercentage = false, MouseFilter = MouseFilterEnum.Ignore };
            row.Hp.SetAnchorsPreset(LayoutPreset.FullRect);
            hb.AddChild(row.Hp);
            row.HpText = Ui.Label("", 11, Ui.Text, outline: true);
            row.HpText.SetAnchorsPreset(LayoutPreset.FullRect);
            row.HpText.HorizontalAlignment = HorizontalAlignment.Center; row.HpText.VerticalAlignment = VerticalAlignment.Center;
            hb.AddChild(row.HpText);
            v.AddChild(hb);
            var st = new HBoxContainer { MouseFilter = MouseFilterEnum.Ignore };
            st.AddThemeConstantOverride("separation", 3);
            v.AddChild(st);
            row.Stance = new Button[Stances.All.Length];
            for (int i = 0; i < Stances.All.Length; i++)
            {
                string s = Stances.All[i];
                var b = Ui.Button(Stances.Short(s), () => Commander.Instance?.SetStance(s, new[] { who }), 12, toggle: true);
                b.CustomMinimumSize = new Vector2(44, 22);
                b.TooltipText = $"{Stances.Name(s)} (Shift+{i + 1}): {Stances.Desc(s)}";
                st.AddChild(b);
                row.Stance[i] = b;
            }
            if (!c.IsPlayer || members.Count > 1)
            {
                row.Wait = Ui.Button("bekle", () => Commander.Instance?.ToggleWait(who), 12, toggle: true);
                row.Wait.CustomMinimumSize = new Vector2(56, 22);
                row.Wait.TooltipText = "Burada beklesin / peşinden gelsin";
                st.AddChild(row.Wait);
            }
            var num = Ui.Label($"{idx}", 11, Ui.Faint);
            num.TooltipText = $"{idx} tuşu: seç";
            st.AddChild(num);
            idx++;
            _list.AddChild(row.Panel);
            _rows.Add(row);
        }
    }

    void Refresh()
    {
        var cmd = Commander.Instance;
        var cd = _r.Combat;
        var ctl = _r.Player.Character;
        foreach (var row in _rows)
        {
            var c = row.C;
            bool sel = cmd?.IsSelected(c) == true;
            row.Panel.AddThemeStyleboxOverride("panel", Ui.Box(sel ? new Color(0.15f, 0.2f, 0.11f, 0.9f) : new Color(0.08f, 0.07f, 0.06f, 0.78f),
                sel ? new Color(0.55f, 0.9f, 0.42f) : new Color(0.38f, 0.3f, 0.2f, 0.8f), sel ? 2 : 1, 5, 7));
            row.Name.Text = $"{(c == ctl ? "▶ " : "")}{c.FullName} · {Rules.ClassName(c.Cls)} Sv{c.Level}";
            var f = cd?.Active == true ? cd.Fight.F.FirstOrDefault(x => x.Char == c) : null;
            float hp = f != null ? f.Hp : c.Hp, max = f != null ? f.MaxHp : c.MaxHp;
            float k = max > 0 ? Math.Clamp(hp / max, 0, 1) : 0;
            row.Hp.Value = k;
            row.Hp.AddThemeStyleboxOverride("fill", Ui.Box(FD.Combat.CombatHud.HpColor(k), FD.Combat.CombatHud.HpColor(k).Lightened(0.2f), 1, 3, 2));
            string slots = c.SlotsMax > 0 ? $"   büyü {new string('●', Math.Max(0, c.Slots))}{new string('○', Math.Max(0, c.SlotsMax - c.Slots))}" : "";
            row.HpText.Text = $"{Math.Max(0, (int)hp)} / {(int)max}{slots}";
            string state = c.Captive ? "kafeste" : (f?.Down ?? c.Down) ? (f != null && !f.Stable ? $"yerde {f.DeathOk}✔ {f.DeathFail}✖" : "baygın")
                : f != null && f.Fled ? "kaçtı" : f != null && f.Fleeing ? "kaçıyor" : f?.Asleep(cd.Fight.T) == true ? "uyuyor"
                : f != null && f.Order == "move" ? "koşuyor" : f != null && f.Order == "attack" ? $"→ {f.OrderTarget?.Name}"
                : f != null && f.Target != null && f.Target.Side != f.Side ? $"dövüşüyor: {f.Target.Name}"
                : cmd?.IsWaiting(c) == true ? "bekliyor" : "";
            if (c.WageSilver > 0 && !c.IsPlayer && cd?.Active != true) state += (state.Length > 0 ? " · " : "") + $"maaş {Math.Max(0, c.PaidUntil - GameClock.Day)} gün";
            row.State.Text = state;
            row.State.AddThemeColorOverride("font_color", c.Down || c.Captive ? Ui.Bad : Ui.Dim);
            for (int i = 0; i < row.Stance.Length; i++) row.Stance[i].SetPressedNoSignal(c.Stance == Stances.All[i]);
            if (row.Wait != null)
            {
                bool waiting = cmd?.IsWaiting(c) == true;
                row.Wait.SetPressedNoSignal(waiting);
                row.Wait.Text = waiting ? "gel" : "bekle";
                row.Wait.Visible = c != ctl && cd?.Active != true;
            }
        }
    }
}
