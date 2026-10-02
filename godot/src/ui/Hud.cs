using System;
using System.Collections.Generic;
using System.Text;
using Godot;
using FD.Actors;
using FD.Core;
using FD.Life;
using FD.Sim.Life;
using FD.World;

namespace FD.UI;

/// <summary>
/// Minimal RPG HUD: clock and place name, the E prompt for the person in front of the hero, the person card
/// (who, doing what, why, next, needs, recent log), the region map (M), a hit flash, and a debug overlay (F3).
/// </summary>
public partial class Hud : CanvasLayer
{
    LifeWorld _life;
    Region _region;
    Label _clock, _prompt, _debug;
    PanelContainer _card;
    Label _cardName, _cardSub, _cardNow, _cardWhy, _cardNext, _cardLog;
    ProgressBar _food, _energy, _social;
    Person _cardPerson, _promptPerson;
    Control _mapRoot;
    TextureRect _mapTex;
    Control _mapMarkers;
    Polygon2D _mapPlayer;
    ColorRect _flash;
    float _flashT, _shakeT;
    bool _debugOn;
    /// <summary>Faz 2: other things to do with E (the cage, the board, a chest…): when nobody stands in front of the hero, the
    /// first provider that returns something is offered.</summary>
    public readonly List<Func<(string text, Action act)?>> Prompts = new();
    (string text, Action act)? _extra;
    Label _toast;
    float _toastT;
    /// <summary>Faz 2 E/G: what can be done with the person whose card is open (Kirala, Söylenti sor, Ticaret, Saldır…), keys 1–5.</summary>
    public readonly List<Func<Person, IEnumerable<(string label, Action act)>>> Verbs = new();
    readonly List<(string label, Action act)> _verbs = new();
    Label _cardVerbs, _party;
    readonly List<(Label l, Vector2 world)> _mapLabels = new();
    const int MapPx = 600;

    public void Init(Region region, LifeWorld life)
    {
        _region = region;
        _life = life;
        Layer = 5;
        var theme = new Theme();
        theme.DefaultFontSize = 17;
        var root = new Control { Name = "Root", MouseFilter = Control.MouseFilterEnum.Ignore };
        root.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        root.Theme = theme;
        AddChild(root);

        _clock = MakeLabel(20, new Color(1, 0.96f, 0.88f));
        _clock.Position = new Vector2(22, 16);
        root.AddChild(_clock);

        _prompt = MakeLabel(20, new Color(1, 0.97f, 0.9f));
        _prompt.SetAnchorsPreset(Control.LayoutPreset.CenterBottom);
        _prompt.HorizontalAlignment = HorizontalAlignment.Center;
        _prompt.Position = new Vector2(-300, -110);
        _prompt.Size = new Vector2(600, 30);
        root.AddChild(_prompt);

        _toast = MakeLabel(19, new Color(1f, 0.93f, 0.75f));
        _toast.SetAnchorsPreset(Control.LayoutPreset.CenterBottom);
        _toast.HorizontalAlignment = HorizontalAlignment.Center;
        _toast.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        _toast.Position = new Vector2(-450, -190);
        _toast.Size = new Vector2(900, 70);
        _toast.VerticalAlignment = VerticalAlignment.Bottom;
        root.AddChild(_toast);

        _debug = MakeLabel(14, new Color(0.85f, 1f, 0.85f));
        _debug.Position = new Vector2(22, 52);
        _debug.Visible = false;
        root.AddChild(_debug);

        BuildCard(root);
        BuildMap(root);
        _party = MakeLabel(15, new Color(0.9f, 0.92f, 0.85f));
        _party.SetAnchorsPreset(Control.LayoutPreset.BottomLeft);
        _party.GrowVertical = Control.GrowDirection.Begin;
        _party.Position = new Vector2(22, -22);
        _party.VerticalAlignment = VerticalAlignment.Bottom;
        root.AddChild(_party);

        _flash = new ColorRect { Color = new Color(0.8f, 0.05f, 0.02f, 0f), MouseFilter = Control.MouseFilterEnum.Ignore };
        _flash.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        root.AddChild(_flash);

        life.Struck += g =>
        {
            _flashT = 0.35f; _shakeT = 0.3f;
            var a = life.ActorOf(g);
            if (a != null && H.Hash(g.Id, (int)(life.Sim.Now * 3)) < 0.35f) a.Say(new[] { "Hıyaaah!", "Al sana!", "Grraah!" }[(int)(H.Hash(g.Id, 9, (int)life.Sim.Now) * 3)], 1.6f);
        };
    }

    static Label MakeLabel(int size, Color c)
    {
        var l = new Label { MouseFilter = Control.MouseFilterEnum.Ignore };
        l.AddThemeFontSizeOverride("font_size", size);
        l.AddThemeColorOverride("font_color", c);
        l.AddThemeColorOverride("font_outline_color", new Color(0.05f, 0.04f, 0.03f, 0.9f));
        l.AddThemeConstantOverride("outline_size", 6);
        return l;
    }

    static StyleBoxFlat PanelStyle()
    {
        return new StyleBoxFlat
        {
            BgColor = new Color(0.09f, 0.075f, 0.06f, 0.88f), BorderColor = new Color(0.72f, 0.58f, 0.32f, 0.9f),
            BorderWidthLeft = 2, BorderWidthRight = 2, BorderWidthTop = 2, BorderWidthBottom = 2,
            CornerRadiusTopLeft = 6, CornerRadiusTopRight = 6, CornerRadiusBottomLeft = 6, CornerRadiusBottomRight = 6,
            ContentMarginLeft = 16, ContentMarginRight = 16, ContentMarginTop = 12, ContentMarginBottom = 14,
        };
    }

    void BuildCard(Control root)
    {
        _card = new PanelContainer { Visible = false, MouseFilter = Control.MouseFilterEnum.Ignore };
        _card.AddThemeStyleboxOverride("panel", PanelStyle());
        _card.SetAnchorsPreset(Control.LayoutPreset.TopRight);
        _card.Position = new Vector2(-440, 20);
        _card.CustomMinimumSize = new Vector2(420, 0);
        root.AddChild(_card);
        var v = new VBoxContainer();
        v.AddThemeConstantOverride("separation", 6);
        _card.AddChild(v);
        Color gold = new(0.95f, 0.82f, 0.52f), text = new(0.93f, 0.9f, 0.84f), dim = new(0.72f, 0.68f, 0.6f);
        _cardName = MakeLabel(24, gold); v.AddChild(_cardName);
        _cardSub = MakeLabel(15, dim); _cardSub.AutowrapMode = TextServer.AutowrapMode.WordSmart; v.AddChild(_cardSub);
        v.AddChild(new HSeparator());
        _cardNow = MakeLabel(18, text); _cardNow.AutowrapMode = TextServer.AutowrapMode.WordSmart; v.AddChild(_cardNow);
        _cardWhy = MakeLabel(15, dim); _cardWhy.AutowrapMode = TextServer.AutowrapMode.WordSmart; v.AddChild(_cardWhy);
        _cardNext = MakeLabel(15, dim); _cardNext.AutowrapMode = TextServer.AutowrapMode.WordSmart; v.AddChild(_cardNext);
        var needs = new GridContainer { Columns = 2 };
        needs.AddThemeConstantOverride("h_separation", 10);
        v.AddChild(needs);
        ProgressBar Bar(string name, Color c)
        {
            needs.AddChild(MakeLabel(15, text) is Label l ? SetText(l, name) : null);
            var b = new ProgressBar { MinValue = 0, MaxValue = 1, Step = 0.01, ShowPercentage = false, CustomMinimumSize = new Vector2(250, 12) };
            b.AddThemeStyleboxOverride("fill", new StyleBoxFlat { BgColor = c, CornerRadiusTopLeft = 3, CornerRadiusBottomLeft = 3, CornerRadiusTopRight = 3, CornerRadiusBottomRight = 3 });
            b.AddThemeStyleboxOverride("background", new StyleBoxFlat { BgColor = new Color(0.2f, 0.17f, 0.14f), CornerRadiusTopLeft = 3, CornerRadiusBottomLeft = 3, CornerRadiusTopRight = 3, CornerRadiusBottomRight = 3 });
            needs.AddChild(b);
            return b;
        }
        _food = Bar("Tokluk", new Color(0.78f, 0.55f, 0.25f));
        _energy = Bar("Dinçlik", new Color(0.35f, 0.62f, 0.85f));
        _social = Bar("Sosyallik", new Color(0.62f, 0.78f, 0.38f));
        v.AddChild(new HSeparator());
        var lt = MakeLabel(14, gold); lt.Text = "Günlük"; v.AddChild(lt);
        _cardLog = MakeLabel(14, dim); v.AddChild(_cardLog);
        _cardVerbs = MakeLabel(16, new Color(1f, 0.9f, 0.6f)); _cardVerbs.AutowrapMode = TextServer.AutowrapMode.WordSmart; v.AddChild(_cardVerbs);
        var hint = MakeLabel(13, new Color(0.6f, 0.56f, 0.5f)); hint.Text = "[E] kapat"; v.AddChild(hint);
    }

    static Label SetText(Label l, string t) { l.Text = t; return l; }

    void BuildMap(Control root)
    {
        _mapRoot = new Control { Visible = false, MouseFilter = Control.MouseFilterEnum.Ignore };
        _mapRoot.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        root.AddChild(_mapRoot);
        var dim = new ColorRect { Color = new Color(0, 0, 0, 0.55f), MouseFilter = Control.MouseFilterEnum.Ignore };
        dim.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        _mapRoot.AddChild(dim);
        var frame = new PanelContainer { MouseFilter = Control.MouseFilterEnum.Ignore };
        frame.AddThemeStyleboxOverride("panel", PanelStyle());
        frame.SetAnchorsPreset(Control.LayoutPreset.Center);
        frame.Position = new Vector2(-MapPx / 2 - 16, -MapPx / 2 - 40);
        _mapRoot.AddChild(frame);
        var v = new VBoxContainer();
        frame.AddChild(v);
        var title = MakeLabel(22, new Color(0.95f, 0.82f, 0.52f));
        title.Text = "Bölge Haritası — Sessiztepe ve çevresi";
        v.AddChild(title);
        var holder = new Control { CustomMinimumSize = new Vector2(MapPx, MapPx), MouseFilter = Control.MouseFilterEnum.Ignore };
        v.AddChild(holder);
        _mapTex = new TextureRect { Size = new Vector2(MapPx, MapPx), MouseFilter = Control.MouseFilterEnum.Ignore };
        holder.AddChild(_mapTex);
        _mapMarkers = new Control { Size = new Vector2(MapPx, MapPx), MouseFilter = Control.MouseFilterEnum.Ignore };
        holder.AddChild(_mapMarkers);
        _mapPlayer = new Polygon2D { Polygon = new[] { new Vector2(0, -13), new Vector2(9, 10), new Vector2(0, 5), new Vector2(-9, 10) }, Color = new Color(1f, 0.85f, 0.2f), ZIndex = 5 };
        var outline = new Line2D { Points = new[] { new Vector2(0, -13), new Vector2(9, 10), new Vector2(0, 5), new Vector2(-9, 10), new Vector2(0, -13) }, Width = 2f, DefaultColor = new Color(0.15f, 0.08f, 0.02f) };
        _mapPlayer.AddChild(outline);
        _mapMarkers.AddChild(_mapPlayer);
        var hint = MakeLabel(14, new Color(0.7f, 0.66f, 0.58f)); hint.Text = "[M] kapat · sarı ok: sen"; v.AddChild(hint);
    }

    Vector2 MapPos(Vector2 world) => new((world.X + RegionSpec.HalfSize) / RegionSpec.Size * MapPx, (world.Y + RegionSpec.HalfSize) / RegionSpec.Size * MapPx);

    void EnsureMapImage()
    {
        if (_mapTex.Texture != null) return;
        var hf = _region.Heightfield;
        var img = Image.CreateEmpty(MapPx, MapPx, false, Image.Format.Rgb8);
        float scale = RegionSpec.Size / MapPx;
        Vector3 L = new Vector3(-1, 1.5f, -1).Normalized();
        for (int j = 0; j < MapPx; j++)
            for (int i = 0; i < MapPx; i++)
            {
                float x = -RegionSpec.HalfSize + (i + 0.5f) * scale, z = -RegionSpec.HalfSize + (j + 0.5f) * scale;
                var bw = hf.Biome(x, z);
                Color c = new Color(0.72f, 0.74f, 0.52f) * bw.Grass + new Color(0.76f, 0.78f, 0.55f) * bw.Meadow
                        + new Color(0.42f, 0.52f, 0.36f) * bw.Forest + new Color(0.66f, 0.55f, 0.4f) * bw.Dirt
                        + new Color(0.62f, 0.6f, 0.56f) * bw.Rock + new Color(0.8f, 0.74f, 0.58f) * bw.Sand
                        + new Color(0.62f, 0.5f, 0.36f) * bw.Field;
                float fd = hf.ForestDensity(x, z);
                c = c.Lerp(new Color(0.36f, 0.46f, 0.3f), Math.Clamp(fd, 0, 1) * 0.7f);
                float shade = Math.Clamp(hf.Normal(x, z).Dot(L) * 1.05f, 0.55f, 1.12f);
                c = new Color(c.R * shade, c.G * shade, c.B * shade);
                if (hf.RoadDistance(x, z) < 2.6f || hf.TrailDistance(x, z) < 1.6f) c = new Color(0.55f, 0.42f, 0.28f);
                if (hf.IsWater(x, z)) c = new Color(0.36f, 0.55f, 0.7f);
                float h = hf.Height(x, z);
                if (MathF.Abs(h / 5f - MathF.Round(h / 5f)) < 0.05f) c = c.Darkened(0.12f);
                // parchment tint
                c = c.Lerp(new Color(0.86f, 0.78f, 0.6f), 0.18f);
                img.SetPixel(i, j, c);
            }
        // buildings (homes, chapel, smithy, inn) as dark squares
        foreach (var pl in _life.Sim.Places)
        {
            if (pl.Kind is not (PlaceKind.Home or PlaceKind.Chapel or PlaceKind.Smithy or PlaceKind.Inn)) continue;
            var d = pl.Door; var f = pl.DoorFace;
            Vector2 c0 = new(d.X - f.X * (pl.Kind == PlaceKind.Inn ? 12f : 5f), d.Y - f.Y * (pl.Kind == PlaceKind.Inn ? 12f : 5f));
            int r = pl.Kind == PlaceKind.Inn ? 6 : 2;
            var mp = MapPos(c0);
            for (int dy = -r; dy <= r; dy++) for (int dx = -r; dx <= r; dx++)
            {
                int px = (int)mp.X + dx, py = (int)mp.Y + dy;
                if (px >= 0 && py >= 0 && px < MapPx && py < MapPx) img.SetPixel(px, py, new Color(0.4f, 0.22f, 0.16f));
            }
        }
        _mapTex.Texture = ImageTexture.CreateFromImage(img);
        void Lbl(string t, Vector2 w, Color? col = null)
        {
            var l = MakeLabel(15, col ?? new Color(0.18f, 0.12f, 0.08f));
            l.AddThemeColorOverride("font_outline_color", new Color(0.95f, 0.9f, 0.78f, 0.9f));
            l.AddThemeConstantOverride("outline_size", 5);
            l.Text = t;
            _mapMarkers.AddChild(l);
            _mapLabels.Add((l, w));
        }
        Lbl(_life.Sim.VillageName, RegionSpec.VillageCenter + new Vector2(-30, -40));
        Lbl($"{_life.Sim.InnName} Hanı", RegionSpec.Inn + new Vector2(-40, 20));
        Lbl("Goblin kampı?", RegionSpec.GoblinCamp + new Vector2(-40, 18), new Color(0.5f, 0.08f, 0.05f));
        Lbl("Eski Gözcü Kulesi", RegionSpec.Ruin + new Vector2(-60, -30));
        Lbl("Mera", RegionSpec.Pasture + new Vector2(-15, -8));
        Lbl("Kara Orman", new Vector2(40, -420));
        Lbl("Kayalık Tepeler", RegionSpec.RockyHills + new Vector2(-40, 0));
        Lbl("odun kesim yeri", OutskirtsSite.LoggingCenter + new Vector2(-50, -20));
        foreach (var (l, w) in _mapLabels) l.Position = MapPos(w);
        _mapMarkers.MoveChild(_mapPlayer, -1);
    }

    public override void _Process(double delta)
    {
        float dt = (float)delta;
        if (_life == null) return;
        var sim = _life.Sim;
        var player = _region.Player;
        _clock.Text = $"{GameClock.TimeString}  ·  {PlaceName(player)}";

        // interaction prompt
        _promptPerson = null;
        if (player != null && player.InputEnabled && !_mapRoot.Visible)
        {
            var fwd = -player.Camera.GlobalBasis.Z;
            _promptPerson = _life.Facing(player.GlobalPosition, fwd);
        }
        if (_cardPerson != null && _cardPerson != _promptPerson)
        {
            var a = _life.ActorOf(_cardPerson);
            if (a == null || !_cardPerson.Visible || player == null || a.GlobalPosition.DistanceTo(player.GlobalPosition) > 9f) CloseCard();
        }
        _extra = null;
        if (_promptPerson == null && player != null && player.InputEnabled && !_mapRoot.Visible)
            foreach (var pr in Prompts) { var r = pr(); if (r != null) { _extra = r; break; } }
        _prompt.Text = _promptPerson != null && _cardPerson != _promptPerson ? $"[E]  {_promptPerson.FullName} — {Census.RoleName(_promptPerson)}"
            : _extra != null ? $"[E]  {_extra.Value.text}" : "";
        UpdateParty();
        if (_toastT > 0) { _toastT -= dt; _toast.Modulate = new Color(1, 1, 1, Math.Clamp(_toastT, 0, 1)); if (_toastT <= 0) _toast.Text = ""; }
        if (_cardPerson != null) FillCard(_cardPerson);

        // map
        if (_mapRoot.Visible && player != null)
        {
            var pp = player.GlobalPosition;
            _mapPlayer.Position = MapPos(new Vector2(pp.X, pp.Z));
            var f = -player.Camera.GlobalBasis.Z;
            _mapPlayer.Rotation = MathF.Atan2(f.X, -f.Z);
        }

        // hit feedback
        if (_flashT > 0)
        {
            _flashT -= dt;
            _flash.Color = new Color(0.8f, 0.05f, 0.02f, Math.Clamp(_flashT, 0, 0.35f) * 0.7f);
        }
        if (player != null)
        {
            if (_shakeT > 0)
            {
                _shakeT -= dt;
                player.Camera.HOffset = (GD.Randf() - 0.5f) * 0.12f;
                player.Camera.VOffset = (GD.Randf() - 0.5f) * 0.12f;
            }
            else { player.Camera.HOffset = 0; player.Camera.VOffset = 0; }
        }

        if (_debugOn)
        {
            var (o, t) = sim.CountOutside(new System.Numerics.Vector2(RegionSpec.VillageCenter.X, RegionSpec.VillageCenter.Y), 95f);
            int vis = 0; foreach (var p in sim.People) if (p.Visible) vis++;
            _debug.Text = $"FPS {Engine.GetFramesPerSecond():F0} · çizim {Performance.GetMonitor(Performance.Monitor.RenderTotalDrawCallsInFrame)} · " +
                          $"primitif {Performance.GetMonitor(Performance.Monitor.RenderTotalPrimitivesInFrame) / 1000:F0}k\n" +
                          $"köyde dışarıda {o}/{t} ({(t > 0 ? 100f * o / t : 0):F0}%) · görünür kişi {vis}/{sim.People.Count}";
        }
    }

    string PlaceName(Player player)
    {
        if (player == null) return "";
        var p = new Vector2(player.GlobalPosition.X, player.GlobalPosition.Z);
        if ((p - RegionSpec.VillageCenter).Length() < 95f) return _life.Sim.VillageName;
        if ((p - RegionSpec.Inn).Length() < 45f) return $"{_life.Sim.InnName} Hanı";
        if ((p - RegionSpec.GoblinCamp).Length() < 45f) return "Goblin kampı";
        if ((p - RegionSpec.Ruin).Length() < 40f) return "Eski Gözcü Kulesi";
        if ((p - RegionSpec.Pasture).Length() < RegionSpec.PastureRadius) return "Mera";
        if (_region.Heightfield.ForestDensity(p.X, p.Y) > 0.5f) return "Kara Orman";
        if (_region.Heightfield.RoadDistance(p.X, p.Y) < 5f) return "Kral Yolu";
        return "Kırlar";
    }

    void FillCard(Person p)
    {
        var sim = _life.Sim;
        _cardName.Text = p.FullName;
        string hh = "";
        if (p.Household >= 0)
        {
            var h = sim.Households[p.Household];
            hh = $" · {h.Surname} hanesi ({h.Members.Count} kişi)";
        }
        _cardSub.Text = p.Role == Role.Goblin ? $"Goblin · {sim.CampName}" : $"{p.Age} yaş · {Census.RoleName(p)}{hh}";
        var a = p.Act;
        string now = a == null ? "—" : p.Motion == Motion.Walking && a.Kind != ActKind.Chase ? (string.IsNullOrEmpty(a.GoLabel) ? a.Label : a.GoLabel) : a.Label;
        if (p.Wait > 0) now = "Seninle konuşuyor";
        _cardNow.Text = $"Şu an: {now}";
        _cardWhy.Text = a == null ? "" : $"Neden: {a.Reason}";
        _cardNext.Text = a == null || a.Kind == ActKind.Chase ? "" : $"Sonra: {sim.NextPlanText(p)}";
        _food.Value = p.Food; _energy.Value = p.Energy; _social.Value = p.Social;
        var sb = new StringBuilder();
        for (int i = p.Log.Count - 1; i >= 0 && i >= p.Log.Count - 4; i--) sb.AppendLine(p.Log[i]);
        _cardLog.Text = sb.ToString().TrimEnd();
        _verbs.Clear();
        foreach (var vp in Verbs) foreach (var vb in vp(p)) _verbs.Add(vb);
        var vs = new StringBuilder();
        for (int i = 0; i < _verbs.Count && i < 5; i++) vs.AppendLine(_verbs[i].act != null ? $"[{i + 1}] {_verbs[i].label}" : $"     {_verbs[i].label}");
        _cardVerbs.Text = vs.ToString().TrimEnd();
        _cardVerbs.Visible = _verbs.Count > 0;
    }

    void OpenCard(Person p)
    {
        _cardPerson = p;
        _card.Visible = true;
        _life.Greet(p);
        FillCard(p);
    }

    void CloseCard() { _cardPerson = null; _card.Visible = false; }
    public void CloseCardPublic() => CloseCard();

    public override void _UnhandledInput(InputEvent e)
    {
        if (_life == null) return;
        if (e.IsActionPressed("interact"))
        {
            if (_cardPerson != null && (_promptPerson == null || _promptPerson == _cardPerson)) CloseCard();
            else if (_promptPerson != null) OpenCard(_promptPerson);
            else if (_extra != null) _extra.Value.act();
            GetViewport().SetInputAsHandled();
        }
        else if (_cardPerson != null && e is InputEventKey vk && vk.Pressed && !vk.Echo && vk.PhysicalKeycode >= Key.Key1 && vk.PhysicalKeycode <= Key.Key5)
        {
            int i = (int)(vk.PhysicalKeycode - Key.Key1);
            if (i < _verbs.Count && _verbs[i].act != null) { var act = _verbs[i].act; act(); }
            GetViewport().SetInputAsHandled();
        }
        else if (e.IsActionPressed("map"))
        {
            ToggleMap();
            GetViewport().SetInputAsHandled();
        }
        else if (e is InputEventKey k && k.Pressed && !k.Echo && k.PhysicalKeycode == Key.F3)
        {
            _debugOn = !_debugOn;
            _debug.Visible = _debugOn;
        }
        else if (e.IsActionPressed("release_mouse") && _mapRoot.Visible)
        {
            ToggleMap();
        }
    }

    public void ToggleMap()
    {
        EnsureMapImage();
        _mapRoot.Visible = !_mapRoot.Visible;
    }

    public void OpenCardFor(Person p) => OpenCard(p);
    public bool MapOpen => _mapRoot != null && _mapRoot.Visible;

    float _partyT;
    void UpdateParty()
    {
        _partyT -= (float)GetProcessDeltaTime();
        if (_partyT > 0) return;
        _partyT = 0.25f;
        var s = _region.Session;
        bool fight = _region.Combat?.Active == true;
        _party.Visible = !fight && s != null && s.Party.Count > 1;
        if (!_party.Visible) return;
        var ctl = _region.Player?.Character;
        var sb = new StringBuilder();
        foreach (var c in s.Party)
        {
            if (c.Dead) continue;
            string mark = c == ctl ? "▶ " : "   ";
            string st = c.Captive ? " · kafeste" : c.Down ? " · baygın" : "";
            string pay = c.WageSilver > 0 && !c.IsPlayer ? $" · maaşa {Math.Max(0, c.PaidUntil - GameClock.Day)} gün" : "";
            sb.AppendLine($"{mark}{c.FullName} · {FD.Rpg.Rules.ClassName(c.Cls)} Sv{c.Level} · can {c.Hp}/{c.MaxHp}{st}{pay}");
        }
        sb.Append("   [Tab] kontrol değiştir");
        _party.Text = sb.ToString();
    }

    /// <summary>A line of feedback over the prompt (a dice check, what happened) for a few seconds.</summary>
    public void Toast(string text, float seconds = 4f) { _toast.Text = text; _toastT = seconds; _toast.Modulate = Colors.White; }
    public void SetDebug(bool on) { _debugOn = on; _debug.Visible = on; }
}
