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
/// Faz 2 C, Tur 1 C: what the player sees of a fight — no panel of its own (the party bar, the clock and the speed are the HUD's,
/// the same as out of a fight). Over the 3D view (<see cref="FightOverlay"/>): rings under the party (bright when selected), a red
/// ring under the hovered foe, health bars over heads, death-save marks over the fallen, and small pale numbers rising over the hit
/// (the damage; a critical in gold, a miss faint; setting). The dice log (every roll broken down) opens with <b>L</b>; a small
/// "Savaş" line with the fight clock at the top; the summary only when the fight is lost.
/// </summary>
public partial class CombatHud : CanvasLayer
{
    CombatDirector _d;
    Control _root;
    FightOverlay _overlay;
    Label _banner, _hint;
    PanelContainer _logPanel, _summary;
    VBoxContainer _sumBody;
    RichTextLabel _log;
    /// <summary>Tur 1 C: the dice log is shown (L toggles it)</summary>
    public static bool ShowLog;
    Label _sumTitle;
    ColorRect _flash;
    float _flashT;
    Color _flashColor;
    readonly List<FightEvent> _events = new();
    bool _logDirty;
    bool _logPausedView;
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

        _banner = Ui.Label("", 17, new Color(0.85f, 0.72f, 0.5f), outline: true);
        _banner.SetAnchorsPreset(Control.LayoutPreset.CenterTop);
        _banner.HorizontalAlignment = HorizontalAlignment.Center;
        _banner.Position = new Vector2(-300, 44); _banner.Size = new Vector2(600, 26);
        _root.AddChild(_banner);
        _hint = Ui.Label("", 17, Ui.Text, outline: true);
        _hint.SetAnchorsPreset(Control.LayoutPreset.CenterTop);
        _hint.HorizontalAlignment = HorizontalAlignment.Center;
        _hint.Position = new Vector2(-500, 70); _hint.Size = new Vector2(1000, 26);
        _root.AddChild(_hint);

        // dice log (right)
        _logPanel = new PanelContainer { MouseFilter = Control.MouseFilterEnum.Ignore };
        _logPanel.AddThemeStyleboxOverride("panel", Ui.Box(new Color(0.07f, 0.06f, 0.05f, 0.78f), new Color(0.55f, 0.43f, 0.24f, 0.7f), 2, 6, 10));
        _logPanel.SetAnchorsPreset(Control.LayoutPreset.TopRight);
        _logPanel.GrowHorizontal = Control.GrowDirection.Begin;
        _logPanel.Position = new Vector2(-16, 90);
        _logPanel.Visible = false;
        _root.AddChild(_logPanel);
        var lv = new VBoxContainer();
        _logPanel.AddChild(lv);
        lv.AddChild(Ui.Label("Zar günlüğü  [L] kapat", 16, Ui.Gold));
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
        _logDirty = true;
        foreach (var e in f.Log) _events.Add(e);
    }

    public void End()
    {
        Visible = false;
        _summary.Visible = false;
        Floaters.Clear();
    }

    // ------------------------------------------------------------------------------------------------ events
    public void OnEvent(FightEvent e)
    {
        _events.Add(e);
        if (_events.Count > 300) _events.RemoveAt(0);
        _logDirty = true;
        var at = e.B ?? e.A;
        if (at != null && FD.Game.Settings.Current.DiceOverHeads && Float(e) is var (text, col, size) && text != null)
        {
            float lift = 0;
            foreach (var fl in Floaters) if (fl.At == at && fl.T < 0.6f) lift += 0.32f;
            Floaters.Add(new Floater { At = at, Text = text, Color = col, Size = size, Lift = lift, Life = e.Crit ? 2.4f : 1.8f });
        }
        if (e.Crit) Flash(new Color(1f, 0.85f, 0.4f), 0.3f);
        else if (e.Kind == "down" && e.A?.Side == FSide.Party) Flash(new Color(0.85f, 0.08f, 0.04f), 0.35f);
        else if (e.Kind == "attack" && e.Hit && e.B?.IsPlayer == true) Flash(new Color(0.8f, 0.05f, 0.02f), 0.18f);
        if (e.Kind == "rout") Floaters.Add(new Floater { At = null, Text = "BOZGUN!", Color = Ui.Gold, Size = 34, Life = 2.5f });
    }

    void Flash(Color c, float strength) { _flashColor = c; _flashT = strength; }

    /// <summary>Tur 1 C: small and pale over heads — the damage, a critical marked, a miss faint; the details live in the dice log (L).</summary>
    static (string text, Color col, int size) Float(FightEvent e)
    {
        var pale = new Color(0.86f, 0.84f, 0.8f, 0.85f);
        switch (e.Kind)
        {
            case "attack":
            case "spell":
                if (e.B == null) return (null, pale, 0);
                if (e.Crit) return ($"KRİTİK {e.Value}", new Color(1f, 0.8f, 0.3f), 19);
                if (e.Fumble) return ("ıska (1)", new Color(0.62f, 0.6f, 0.58f, 0.75f), 12);
                if (!e.Hit) return (e.Value > 0 ? $"{e.Value}" : "ıska", new Color(0.62f, 0.6f, 0.58f, 0.75f), 12);
                return ($"{e.Value}", e.B.Side == FSide.Party ? new Color(1f, 0.62f, 0.55f, 0.9f) : pale, 14);
            case "heal": case "potion": case "wind": return (e.Value > 0 ? $"+{e.Value}" : e.Short, new Color(0.6f, 0.9f, 0.55f, 0.9f), 14);
            case "up": case "stable": return (e.Short, new Color(0.6f, 0.9f, 0.55f, 0.85f), 13);
            case "down": return ("yere düştü", new Color(1f, 0.42f, 0.36f), 16);
            case "dead": return ("öldü", new Color(1f, 0.38f, 0.32f), 16);
            case "wound": return (e.Short, new Color(1f, 0.5f, 0.4f), 15);
            case "sleep": return ("uyudu", new Color(0.75f, 0.8f, 1f, 0.85f), 13);
            case "flee": case "caught": case "join": return (e.Short, new Color(0.9f, 0.82f, 0.62f, 0.85f), 13);
            default: return (null, pale, 0);
        }
    }

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
        int foes = 0; foreach (var f in fight.F) if (f.Side == FSide.Foe && f.Standing && !f.Fleeing) foes++;
        _banner.Text = _d.Summary ? "" : $"Savaş · {secs / 60}:{secs % 60:00} · ayakta {foes} düşman";
        _hint.Text = _d.Hint ?? "";
        _logPanel.Visible = ShowLog && !_d.Summary;
        if (_logPanel.Visible && (_logDirty || _logPausedView != frozen)) RebuildLog(frozen);
        _overlay.QueueRedraw();
    }

    public static Color HpColor(float k) => k > 0.6f ? new Color(0.35f, 0.75f, 0.3f) : k > 0.3f ? new Color(0.85f, 0.7f, 0.25f) : new Color(0.85f, 0.25f, 0.18f);

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
            }
            else if (!f.Dead)
            {
                if (hov) Ring(f, f.Radius + 0.3f, new Color(1f, 0.3f, 0.2f, 0.95f), 3f);
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
            Text(p, fl.Text, (int)(fl.Size * pop), new Color(fl.Color, fl.Color.A * a), 4);
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
            if (_d.Camera == null || _d.Camera.IsPositionBehind(w)) return;
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
