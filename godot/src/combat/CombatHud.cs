using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;
using FD.Rpg;
using FD.UI;
using V2 = System.Numerics.Vector2;

namespace FD.Combat;

/// <summary>
/// Faz 2 C: what the player sees in a fight. Over the 3D view (drawn each frame by <see cref="FightOverlay"/>): rings under the
/// party (bright when selected), a red ring under the hovered foe, health bars over heads, death-save marks over the fallen, the
/// dice of every swing rising over the target ("17+5 → 22 vs ZS 15 · isabet · 6"; a natural 20 in gold and bigger, with a flash),
/// the selection box. Panels: the party (HP, spell slots, state, click to select), the actions of the selected member (spells 1–5,
/// potion Q, bandage B, second wind F, retreat R, hold H, free G), the dice log (when paused: every roll broken down), the banner
/// (fight clock or PAUSED) and the summary when it ends.
/// </summary>
public partial class CombatHud : CanvasLayer
{
    CombatDirector _d;
    Control _root;
    FightOverlay _overlay;
    Label _banner, _hint, _help;
    PanelContainer _partyPanel, _actionPanel, _logPanel, _summary;
    VBoxContainer _partyList, _sumBody;
    HBoxContainer _actions;
    RichTextLabel _log;
    Label _sumTitle;
    ColorRect _flash;
    float _flashT;
    Color _flashColor;
    readonly List<FightEvent> _events = new();
    bool _logDirty;
    bool _logPausedView;
    string _actionsKey, _partyKey;
    readonly List<(Fighter f, PanelContainer row, Label name, ProgressBar hp, Label hpText, Label state)> _rows = new();
    public readonly List<Floater> Floaters = new();

    public sealed class Floater
    {
        public Fighter At;
        public string Text;
        public Color Color;
        public float T, Life = 2.2f, Lift;
        public int Size = 18;
    }

    public void Init(CombatDirector d)
    {
        _d = d;
        Layer = 6;
        ProcessMode = ProcessModeEnum.Always;
        Visible = false;
        _root = new Control { Name = "Root", MouseFilter = Control.MouseFilterEnum.Ignore, Theme = Ui.MakeTheme(16) };
        _root.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        AddChild(_root);

        _overlay = new FightOverlay { Name = "Overlay", MouseFilter = Control.MouseFilterEnum.Ignore };
        _overlay.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        _overlay.Init(d, this);
        _root.AddChild(_overlay);

        _flash = new ColorRect { Color = new Color(1, 1, 1, 0), MouseFilter = Control.MouseFilterEnum.Ignore };
        _flash.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        _root.AddChild(_flash);

        _banner = Ui.Label("", 26, Ui.Gold, outline: true);
        _banner.SetAnchorsPreset(Control.LayoutPreset.CenterTop);
        _banner.HorizontalAlignment = HorizontalAlignment.Center;
        _banner.Position = new Vector2(-400, 14); _banner.Size = new Vector2(800, 36);
        _root.AddChild(_banner);
        _hint = Ui.Label("", 18, Ui.Text, outline: true);
        _hint.SetAnchorsPreset(Control.LayoutPreset.CenterTop);
        _hint.HorizontalAlignment = HorizontalAlignment.Center;
        _hint.Position = new Vector2(-500, 52); _hint.Size = new Vector2(1000, 28);
        _root.AddChild(_hint);

        // party (bottom left)
        _partyPanel = new PanelContainer { MouseFilter = Control.MouseFilterEnum.Ignore };
        _partyPanel.AddThemeStyleboxOverride("panel", Ui.Box(new Color(0.07f, 0.06f, 0.05f, 0.82f), new Color(0.55f, 0.43f, 0.24f, 0.8f), 2, 6, 10));
        _partyPanel.SetAnchorsPreset(Control.LayoutPreset.BottomLeft);
        _partyPanel.GrowVertical = Control.GrowDirection.Begin;
        _partyPanel.Position = new Vector2(16, -16);
        _root.AddChild(_partyPanel);
        _partyList = new VBoxContainer();
        _partyList.AddThemeConstantOverride("separation", 6);
        _partyPanel.AddChild(_partyList);

        // actions (bottom centre)
        _actionPanel = new PanelContainer { MouseFilter = Control.MouseFilterEnum.Ignore };
        _actionPanel.AddThemeStyleboxOverride("panel", Ui.Box(new Color(0.07f, 0.06f, 0.05f, 0.82f), new Color(0.55f, 0.43f, 0.24f, 0.8f), 2, 6, 8));
        _actionPanel.SetAnchorsPreset(Control.LayoutPreset.CenterBottom);
        _actionPanel.GrowHorizontal = Control.GrowDirection.Both;
        _actionPanel.GrowVertical = Control.GrowDirection.Begin;
        _actionPanel.Position = new Vector2(0, -16);
        _root.AddChild(_actionPanel);
        var av = new VBoxContainer();
        av.AddThemeConstantOverride("separation", 4);
        _actionPanel.AddChild(av);
        _actions = new HBoxContainer();
        _actions.AddThemeConstantOverride("separation", 6);
        av.AddChild(_actions);
        _help = Ui.Label("Sol tık: seç · yürü · saldır  ·  sürükle: kutuyla seç  ·  sağ sürükle: döndür  ·  WASD: kaydır  ·  teker: yakınlaş  ·  Tab: sıradaki  ·  Boşluk: duraklat", 13, Ui.Faint);
        _help.HorizontalAlignment = HorizontalAlignment.Center;
        av.AddChild(_help);

        // dice log (right)
        _logPanel = new PanelContainer { MouseFilter = Control.MouseFilterEnum.Ignore };
        _logPanel.AddThemeStyleboxOverride("panel", Ui.Box(new Color(0.07f, 0.06f, 0.05f, 0.78f), new Color(0.55f, 0.43f, 0.24f, 0.7f), 2, 6, 10));
        _logPanel.SetAnchorsPreset(Control.LayoutPreset.TopRight);
        _logPanel.GrowHorizontal = Control.GrowDirection.Begin;
        _logPanel.Position = new Vector2(-16, 90);
        _root.AddChild(_logPanel);
        var lv = new VBoxContainer();
        _logPanel.AddChild(lv);
        lv.AddChild(Ui.Label("Zar günlüğü", 16, Ui.Gold));
        _log = new RichTextLabel
        {
            BbcodeEnabled = true, ScrollFollowing = true, FitContent = false, CustomMinimumSize = new Vector2(430, 380),
            MouseFilter = Control.MouseFilterEnum.Ignore, SelectionEnabled = false,
        };
        _log.AddThemeFontSizeOverride("normal_font_size", 14);
        _log.AddThemeFontSizeOverride("bold_font_size", 14);
        _log.AddThemeColorOverride("default_color", Ui.Text);
        lv.AddChild(_log);

        // summary (centre)
        _summary = new PanelContainer { Visible = false };
        _summary.AddThemeStyleboxOverride("panel", Ui.PanelStyle());
        _summary.SetAnchorsPreset(Control.LayoutPreset.Center);
        _summary.GrowHorizontal = Control.GrowDirection.Both;
        _summary.GrowVertical = Control.GrowDirection.Both;
        _summary.CustomMinimumSize = new Vector2(520, 0);
        _root.AddChild(_summary);
        _sumBody = new VBoxContainer();
        _sumBody.AddThemeConstantOverride("separation", 6);
        _summary.AddChild(_sumBody);
        _sumTitle = Ui.Label("", 34, Ui.Gold);
        _sumTitle.HorizontalAlignment = HorizontalAlignment.Center;
    }

    public void Begin(Fight f)
    {
        Visible = true;
        _events.Clear(); Floaters.Clear();
        _summary.Visible = false;
        _actionsKey = _partyKey = null;
        _logDirty = true;
        foreach (var e in f.Log) _events.Add(e);
    }

    public void End()
    {
        Visible = false;
        _summary.Visible = false;
        Floaters.Clear();
        _events.Clear();
        foreach (var r in _rows) r.row.QueueFree();
        _rows.Clear();
    }

    // ------------------------------------------------------------------------------------------------ events
    public void OnEvent(FightEvent e)
    {
        _events.Add(e);
        if (_events.Count > 300) _events.RemoveAt(0);
        _logDirty = true;
        var at = e.B ?? e.A;
        if (at != null && !string.IsNullOrEmpty(e.Short) && FD.Game.Settings.Current.DiceOverHeads)
        {
            var col = e.Crit ? new Color(1f, 0.82f, 0.25f)
                : e.Kind is "heal" or "potion" or "up" or "wind" or "stable" ? new Color(0.55f, 0.95f, 0.5f)
                : e.Kind is "down" or "dead" ? new Color(1f, 0.35f, 0.3f)
                : e.Kind is "save" ? new Color(0.85f, 0.75f, 1f)
                : e.Kind is "attack" or "spell" ? (e.Hit ? (at.Side == FSide.Party ? new Color(1f, 0.55f, 0.45f) : Colors.White) : new Color(0.7f, 0.7f, 0.7f))
                : new Color(0.9f, 0.88f, 0.8f);
            float lift = 0;
            foreach (var fl in Floaters) if (fl.At == at && fl.T < 0.6f) lift += 0.38f;
            Floaters.Add(new Floater { At = at, Text = e.Short, Color = col, Size = e.Crit ? 26 : e.Kind is "down" or "dead" or "rout" ? 22 : 18, Lift = lift, Life = e.Crit ? 2.8f : 2.2f });
        }
        if (e.Crit) Flash(new Color(1f, 0.85f, 0.4f), 0.3f);
        else if (e.Kind == "down" && e.A?.Side == FSide.Party) Flash(new Color(0.85f, 0.08f, 0.04f), 0.35f);
        else if (e.Kind == "attack" && e.Hit && e.B?.IsPlayer == true) Flash(new Color(0.8f, 0.05f, 0.02f), 0.18f);
        if (e.Kind == "rout") Floaters.Add(new Floater { At = null, Text = "BOZGUN!", Color = Ui.Gold, Size = 34, Life = 2.5f });
    }

    void Flash(Color c, float strength) { _flashColor = c; _flashT = strength; }

    // ------------------------------------------------------------------------------------------------ frame
    public override void _Process(double delta)
    {
        if (!Visible || _d.Fight == null) return;
        float dt = (float)Math.Min(delta, 0.1);
        var fight = _d.Fight;
        bool frozen = _d.Paused || _d.Summary;
        if (!frozen) for (int i = Floaters.Count - 1; i >= 0; i--) { Floaters[i].T += dt; if (Floaters[i].T > Floaters[i].Life) Floaters.RemoveAt(i); }
        if (_flashT > 0) { _flashT -= dt; _flash.Color = new Color(_flashColor, Math.Clamp(_flashT, 0, 0.35f) * 0.8f); }
        else _flash.Color = new Color(1, 1, 1, 0);

        int secs = (int)fight.T;
        _banner.Text = _d.Summary ? "" : _d.Paused ? "⏸  DURAKLATILDI  —  Boşluk: sürdür" : $"SAVAŞ  ·  {secs / 60}:{secs % 60:00}";
        _banner.AddThemeColorOverride("font_color", _d.Paused ? new Color(1f, 0.92f, 0.6f) : Ui.Gold);
        _hint.Text = _d.Hint ?? (_d.Aim != null ? $"{_d.Aim.Name}: hedefi tıkla" : "");

        UpdateParty(fight);
        UpdateActions();
        if (_logDirty || _logPausedView != frozen) RebuildLog(frozen);
        _overlay.QueueRedraw();
    }

    void UpdateParty(Fight fight)
    {
        var party = fight.Of(FSide.Party).ToList();
        string key = string.Join(",", party.Select(f => f.Id));
        if (key != _partyKey)
        {
            _partyKey = key;
            foreach (var r in _rows) r.row.QueueFree();
            _rows.Clear();
            foreach (var f in party)
            {
                var row = new PanelContainer { MouseFilter = Control.MouseFilterEnum.Stop, CustomMinimumSize = new Vector2(300, 0) };
                var fighter = f;
                row.GuiInput += ev =>
                {
                    if (ev is InputEventMouseButton mb && mb.Pressed && mb.ButtonIndex == MouseButton.Left)
                    { _d.Select(fighter, mb.ShiftPressed); row.AcceptEvent(); }
                };
                var v = new VBoxContainer { MouseFilter = Control.MouseFilterEnum.Ignore };
                v.AddThemeConstantOverride("separation", 2);
                row.AddChild(v);
                var name = Ui.Label("", 16, Ui.Gold);
                v.AddChild(name);
                var hb = new Control { CustomMinimumSize = new Vector2(280, 16), MouseFilter = Control.MouseFilterEnum.Ignore };
                var hp = new ProgressBar { MinValue = 0, MaxValue = 1, ShowPercentage = false, MouseFilter = Control.MouseFilterEnum.Ignore };
                hp.SetAnchorsPreset(Control.LayoutPreset.FullRect);
                hb.AddChild(hp);
                var hpText = Ui.Label("", 12, Ui.Text, outline: true);
                hpText.SetAnchorsPreset(Control.LayoutPreset.FullRect);
                hpText.HorizontalAlignment = HorizontalAlignment.Center; hpText.VerticalAlignment = VerticalAlignment.Center;
                hb.AddChild(hpText);
                v.AddChild(hb);
                var state = Ui.Label("", 13, Ui.Dim);
                v.AddChild(state);
                _partyList.AddChild(row);
                _rows.Add((f, row, name, hp, hpText, state));
            }
        }
        foreach (var (f, row, name, hp, hpText, state) in _rows)
        {
            bool sel = _d.Selected.Contains(f);
            row.AddThemeStyleboxOverride("panel", Ui.Box(sel ? new Color(0.18f, 0.24f, 0.12f, 0.92f) : new Color(0.12f, 0.1f, 0.08f, 0.85f),
                sel ? new Color(0.6f, 0.95f, 0.45f) : new Color(0.4f, 0.32f, 0.2f), sel ? 2 : 1, 5, 7));
            var c = f.Char;
            name.Text = c != null ? $"{c.FullName}  ·  {Rules.ClassName(c.Cls)} Sv{c.Level}  ·  ZS {(int)f.Cb.Ac}" : f.Name;
            float k = f.MaxHp > 0 ? Math.Clamp(f.Hp / f.MaxHp, 0, 1) : 0;
            hp.Value = k;
            hp.AddThemeStyleboxOverride("fill", Ui.Box(HpColor(k), HpColor(k).Lightened(0.2f), 1, 3, 2));
            string slots = c != null && c.SlotsMax > 0 ? $"   büyü {new string('●', c.Slots)}{new string('○', Math.Max(0, c.SlotsMax - c.Slots))}" : "";
            hpText.Text = $"{Math.Max(0, (int)f.Hp)} / {(int)f.MaxHp}{slots}";
            state.Text = StateText(f);
            state.AddThemeColorOverride("font_color", f.Down || f.Dead ? Ui.Bad : f.Fleeing ? new Color(0.95f, 0.75f, 0.4f) : Ui.Dim);
        }
    }

    public static Color HpColor(float k) => k > 0.6f ? new Color(0.35f, 0.75f, 0.3f) : k > 0.3f ? new Color(0.85f, 0.7f, 0.25f) : new Color(0.85f, 0.25f, 0.18f);

    string StateText(Fighter f)
    {
        var fight = _d.Fight;
        if (f.Dead) return "ÖLDÜ";
        if (f.Fled) return "kaçtı";
        if (f.Down) return f.Stable ? "baygın, dengede" : $"YERDE — ölüm zarları {new string('✔', f.DeathOk)}{new string('·', 3 - Math.Min(3, f.DeathOk))} {new string('✖', f.DeathFail)}{new string('·', 3 - Math.Min(3, f.DeathFail))}";
        if (f.Asleep(fight.T)) return "uyuyor";
        if (f.Fleeing) return "kaçıyor!";
        string o = f.Order switch
        {
            "attack" => $"emir: {f.OrderTarget?.Name} hedefine saldır",
            "move" => "emir: yürü",
            "retreat" => "emir: geri çekil",
            "hold" => "emir: bekle",
            "cast" => $"emir: {Spells.Get(f.OrderSpell)?.Name} → {f.OrderTarget?.Name}",
            "potion" => "emir: iksir",
            "bandage" => $"emir: {f.OrderTarget?.Name} sarılacak",
            _ => null,
        };
        if (o != null) return o;
        return f.Target != null && f.Target.Side != f.Side ? $"kendi kararıyla: {f.Target.Name}" : "kendi kararıyla";
    }

    void UpdateActions()
    {
        var f = _d.Lead;
        var c = f?.Char;
        string key = c == null ? "-" : $"{f.Id}|{c.Slots}|{c.Inv.Count("potion")}|{c.Inv.Count("bandage")}|{c.Uses.GetValueOrDefault("secondWind")}|{string.Join(",", c.Spells)}|{f.Standing}|{_d.Summary}";
        if (key == _actionsKey) return;
        _actionsKey = key;
        foreach (var n in _actions.GetChildren()) n.QueueFree();
        _actionPanel.Visible = c != null && !_d.Summary;
        if (c == null) return;
        _actions.AddChild(Ui.Label(c.Name + ":", 16, Ui.Gold));
        int i = 1;
        foreach (var sp in CombatDirector.SpellList(c))
        {
            int n = i;
            string slot = sp.Level > 0 ? $" ({c.Slots})" : "";
            var b = Ui.Button($"[{n}] {sp.Name}{slot}", () => _d.SpellKey(n), 15);
            b.Disabled = !f.Standing || (sp.Level > 0 && c.Slots <= 0);
            b.TooltipText = sp.Desc;
            _actions.AddChild(b);
            i++;
        }
        int pots = c.Inv.Count("potion"), bands = c.Inv.Count("bandage");
        var pb = Ui.Button($"[Q] İksir ×{pots}", _d.PotionKey, 15); pb.Disabled = pots == 0 || !f.Standing; pb.TooltipText = "Şifa iksiri: 2d4+2 can (fareyle bir yoldaşın üstündeysen ona içirir)"; _actions.AddChild(pb);
        var bb = Ui.Button($"[B] Sargı ×{bands}", _d.BandageKey, 15); bb.Disabled = bands == 0 || !f.Standing; bb.TooltipText = "Yerde yatan bir yoldaşı sarıp 1 canla kaldırır"; _actions.AddChild(bb);
        if (c.Cls == "fighter")
        {
            var wb = Ui.Button("[F] Derin nefes", _d.WindKey, 15); wb.Disabled = c.Uses.GetValueOrDefault("secondWind") <= 0 || !f.Standing;
            wb.TooltipText = "1d10 + seviye can (dinlenene dek bir kez)"; _actions.AddChild(wb);
        }
        _actions.AddChild(Ui.Button("[R] Geri çekil", () => _d.OrderAll("retreat", "geri çekilsin"), 15));
        _actions.AddChild(Ui.Button("[H] Bekle", () => _d.OrderAll("hold", "yerinde beklesin"), 15));
        _actions.AddChild(Ui.Button("[G] Serbest", _d.ClearOrders, 15));
    }

    void RebuildLog(bool detail)
    {
        _logDirty = false;
        _logPausedView = detail;
        var sb = new StringBuilder();
        int n = detail ? 12 : 16;
        int start = Math.Max(0, _events.Count - n);
        for (int i = start; i < _events.Count; i++)
        {
            var e = _events[i];
            if (string.IsNullOrEmpty(e.Text)) continue;
            string col = e.Crit ? "#ffd166" : e.Kind is "down" or "dead" ? "#ff7766" : e.Kind is "heal" or "potion" or "up" or "wind" or "stable" ? "#9be08a"
                : e.Kind is "order" ? "#a8c8ff" : e.Kind is "rout" or "join" or "start" or "end" ? "#f2d28a" : (e.Kind is "attack" or "spell") && !e.Hit ? "#a09a90" : "#ece6d6";
            int s = (int)e.T;
            sb.Append($"[color=#7d766a]{s / 60}:{s % 60:00}[/color] [color={col}]{Esc(e.Text)}[/color]\n");
            if (detail && !string.IsNullOrEmpty(e.Detail)) sb.Append($"      [color=#9c9484][i]{Esc(e.Detail)}[/i][/color]\n");
        }
        _log.Text = sb.ToString();
    }

    static string Esc(string s) => s.Replace("[", "[lb]");

    // ------------------------------------------------------------------------------------------------ summary
    public void ShowSummary(FightOutcome o)
    {
        foreach (var n in _sumBody.GetChildren()) { if (n != _sumTitle) n.QueueFree(); }
        if (_sumTitle.GetParent() == null) _sumBody.AddChild(_sumTitle);
        _sumTitle.Text = o.Won ? "ZAFER" : o.PlayerDead ? "ÖLDÜN" : "YENİLGİ";
        _sumTitle.AddThemeColorOverride("font_color", o.Won ? Ui.Gold : Ui.Bad);
        foreach (var l in o.Lines) _sumBody.AddChild(Ui.Label(l, 18, Ui.Text, wrap: true));
        _sumBody.AddChild(Ui.Gap(8));
        var b = Ui.Button("Devam  [Boşluk]", _d.Continue, 20);
        _sumBody.AddChild(b);
        _summary.Visible = true;
        _actionPanel.Visible = false;
    }
}

/// <summary>Draws the fight over the 3D view: rings, health bars, death saves, dice text, the selection box, the aim cursor.</summary>
public partial class FightOverlay : Control
{
    CombatDirector _d;
    CombatHud _hud;
    Font _font;

    public void Init(CombatDirector d, CombatHud hud) { _d = d; _hud = hud; _font = ThemeDB.FallbackFont; }

    float HeadHeight(Fighter f) => f.Kind switch { "boss" => 1.75f, "goblin" or "archer" => 1.45f, _ => 2.0f };

    public override void _Draw()
    {
        var fight = _d.Fight;
        if (fight == null) return;
        var lead = _d.Lead;
        foreach (var f in fight.F)
        {
            if (f.Fled) continue;
            bool sel = _d.Selected.Contains(f);
            bool hov = _d.Hover == f;
            if (f.Side == FSide.Party)
            {
                if (!f.Dead) Ring(f, f.Radius + 0.25f, sel ? new Color(0.45f, 1f, 0.35f, 0.95f) : new Color(0.45f, 0.85f, 0.35f, 0.4f), sel ? 3f : 1.5f);
                if (hov && _d.Aim?.Kind == SpellKind.Heal) Ring(f, f.Radius + 0.45f, new Color(1f, 0.9f, 0.4f), 2.5f);
            }
            else if (!f.Dead)
            {
                if (hov) Ring(f, f.Radius + 0.3f, _d.Aim != null ? new Color(1f, 0.9f, 0.4f) : new Color(1f, 0.3f, 0.2f, 0.95f), 3f);
                else if (lead != null && (lead.Target == f || lead.OrderTarget == f)) Ring(f, f.Radius + 0.3f, new Color(1f, 0.55f, 0.2f, 0.7f), 1.5f);
            }
            if (f.Dead) continue;
            var head = _d.ScreenOf(f, HeadHeight(f) + 0.15f);
            if (head is not Vector2 hp) continue;
            bool showBar = f.Side == FSide.Party || hov || f.Hp < f.MaxHp || f.IsBoss;
            if (f.Down)
            {
                string ds = f.Stable ? "dengede" : $"{new string('✔', f.DeathOk)}{new string('○', Math.Max(0, 3 - f.DeathOk))}  {new string('✖', f.DeathFail)}{new string('○', Math.Max(0, 3 - f.DeathFail))}";
                Text(hp - new Vector2(0, 2), ds, 15, f.Stable ? new Color(0.7f, 0.9f, 0.7f) : new Color(1f, 0.6f, 0.55f));
            }
            else if (showBar)
            {
                float w = f.IsBoss ? 64 : 46, h = 6;
                float k = Math.Clamp(f.Hp / Math.Max(1f, f.MaxHp), 0, 1);
                var r = new Rect2(hp.X - w / 2, hp.Y - h, w, h);
                DrawRect(r.Grow(1.5f), new Color(0, 0, 0, 0.7f));
                DrawRect(new Rect2(r.Position, new Vector2(w * k, h)), CombatHud.HpColor(k));
                if (hov || f.IsBoss || f.Side == FSide.Party) Text(hp - new Vector2(0, 10), hov ? $"{f.Name}  {Math.Max(0, (int)f.Hp)}/{(int)f.MaxHp} · ZS {(int)f.Cb.Ac}" : f.Name, 13, f.Side == FSide.Party ? new Color(0.8f, 1f, 0.75f) : new Color(1f, 0.85f, 0.8f));
            }
            if (f.Asleep(fight.T)) Text(hp - new Vector2(-18, 22), "Zzz", 15, new Color(0.75f, 0.8f, 1f));
        }
        // dice and words rising over heads
        foreach (var fl in _hud.Floaters)
        {
            Vector2 p;
            if (fl.At == null) p = new Vector2(Size.X / 2, Size.Y * 0.3f);
            else
            {
                var s = _d.ScreenOf(fl.At, HeadHeight(fl.At) + 0.55f + fl.Lift + fl.T * 0.45f);
                if (s is not Vector2 sp) continue;
                p = sp;
            }
            float a = Math.Clamp((fl.Life - fl.T) / 0.5f, 0, 1);
            float pop = fl.T < 0.12f ? 1f + (0.12f - fl.T) * 3f : 1f;
            Text(p, fl.Text, (int)(fl.Size * pop), new Color(fl.Color, a), 7);
        }
        if (_d.Box is Rect2 box)
        {
            DrawRect(box, new Color(0.5f, 1f, 0.4f, 0.12f));
            DrawRect(box, new Color(0.5f, 1f, 0.4f, 0.8f), false, 1.5f);
        }
        if (_d.Aim != null)
        {
            var m = GetLocalMousePosition();
            Text(m + new Vector2(0, -22), $"✦ {_d.Aim.Name}", 16, new Color(1f, 0.9f, 0.5f));
        }
    }

    void Text(Vector2 at, string s, int size, Color c, int outline = 5)
    {
        var w = _font.GetStringSize(s, HorizontalAlignment.Left, -1, size).X;
        var p = new Vector2(at.X - w / 2, at.Y);
        DrawStringOutline(_font, p, s, HorizontalAlignment.Left, -1, size, outline, new Color(0.04f, 0.03f, 0.02f, c.A * 0.9f));
        DrawString(_font, p, s, HorizontalAlignment.Left, -1, size, c);
    }

    void Ring(Fighter f, float r, Color c, float width)
    {
        const int N = 24;
        var pts = new Vector2[N + 1];
        for (int i = 0; i <= N; i++)
        {
            float a = i * MathF.Tau / N;
            var fake = new V2(f.Pos.X + MathF.Cos(a) * r, f.Pos.Y + MathF.Sin(a) * r);
            var w = new Vector3(fake.X, 0, fake.Y);
            w.Y = Region3.Height(fake.X, fake.Y) + 0.06f;
            if (_d.Camera.IsPositionBehind(w)) return;
            pts[i] = _d.Camera.UnprojectPosition(w);
        }
        DrawPolyline(pts, c, width, true);
    }
}

/// <summary>ground height for the overlay (the region's heightfield)</summary>
static class Region3
{
    public static float Height(float x, float z) => FD.World.Region.Current?.Heightfield.Height(x, z) ?? 0f;
}
