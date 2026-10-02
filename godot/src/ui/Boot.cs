using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using FD.Actors;
using FD.Game;
using FD.Rpg;
using FD.World;

namespace FD.UI;

/// <summary>
/// Faz 2 main scene: title menu → character creation (race, class, 27-point stats, alignment, name, look; wizard picks a first-level
/// spell) → the world is born (macro world + prehistory on a worker thread, progress bar) → the region. Dev runs with test/shot
/// arguments skip straight to the region (Region builds a dev session and a default character).
/// </summary>
public partial class Boot : Control
{
    Control _menu, _create, _gen;
    Theme _theme;
    // creation state
    string _race = "human", _cls = "fighter", _align = "neutral", _spell = "magicmissile";
    int[] _base = Rules.SuggestedBase("fighter");
    Look _look = new();
    LineEdit _name, _seed;
    Label _points, _summary, _raceInfo, _clsInfo, _genText;
    ProgressBar _genBar;
    readonly Label[] _statVal = new Label[6], _statRace = new Label[6], _statTot = new Label[6], _statMod = new Label[6];
    readonly List<Button> _raceBtns = new(), _clsBtns = new(), _alignBtns = new(), _spellBtns = new(), _skinBtns = new(), _hairBtns = new(), _tunicBtns = new(), _styleBtns = new();
    HBoxContainer _skinRow, _hairRow, _tunicRow;
    VBoxContainer _spellBox;
    Button _beard, _female;
    HSlider _height;
    CharacterPreview _preview;
    Button _start;
    readonly Random _rnd = new();
    // world generation
    volatile int _genDay, _genTotal = FD.Macro.Sim.PREHISTORY_DAYS;
    Task<Session> _genTask;
    Character _pending;
    double _genT;

    public override void _Ready()
    {
        var dev = FD.Dev.Dev.Instance;
        if (dev != null && dev.SkipMenu)
        {
            CallDeferred(nameof(GoRegion));
            return;
        }
        _theme = Ui.MakeTheme(18);
        Theme = _theme;
        SetAnchorsPreset(LayoutPreset.FullRect);
        var bg = new ColorRect { Color = new Color(0.07f, 0.06f, 0.05f) };
        bg.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(bg);
        var tex = new TextureRect { Texture = MakeVignette(), ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize, StretchMode = TextureRect.StretchModeEnum.Scale, MouseFilter = MouseFilterEnum.Ignore };
        tex.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(tex);
        BuildMenu();
        BuildCreate();
        BuildGen();
        ShowOnly(_menu);
        if (dev?.UiScreen == "create") { ShowOnly(_create); Refresh(); }
        if (dev?.NewGame != null)
        {
            var a = dev.NewGame.Split(',');
            _race = a[0]; _cls = a.Length > 1 ? a[1] : "fighter"; _base = Rules.SuggestedBase(_cls);
            if (a.Length > 2) _seed.Text = a[2];
            if (a.Length > 3) _name.Text = a[3];
            ShowOnly(_create); Refresh();
            StartGame();
        }
    }

    int _frames;
    public override void _PhysicsProcess(double delta)
    {
        var dev = FD.Dev.Dev.Instance;
        if (dev?.UiScreen == null || dev.ShotPath == null || dev.NewGame != null) return;
        if (++_frames != Math.Max(10, dev.Warm)) return;
        var img = GetViewport().GetTexture().GetImage();
        img.SavePng(dev.ShotPath);
        GD.Print($"[Boot] screenshot {dev.ShotPath}");
        GetTree().Quit();
    }

    void GoRegion() => GetTree().ChangeSceneToFile("res://scenes/Region.tscn");

    static Texture2D MakeVignette()
    {
        var g = new Gradient();
        g.SetColor(0, new Color(0.22f, 0.15f, 0.09f, 0.55f));
        g.SetColor(1, new Color(0, 0, 0, 0f));
        return new GradientTexture2D { Gradient = g, Fill = GradientTexture2D.FillEnum.Radial, FillFrom = new Vector2(0.5f, 0.42f), FillTo = new Vector2(1.05f, 1.05f), Width = 512, Height = 512 };
    }

    void ShowOnly(Control c)
    {
        _menu.Visible = c == _menu; _create.Visible = c == _create; _gen.Visible = c == _gen;
    }

    // ------------------------------------------------------------------------------------------------ menu
    void BuildMenu()
    {
        _menu = new CenterContainer();
        _menu.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(_menu);
        var v = new VBoxContainer { CustomMinimumSize = new Vector2(420, 0) };
        v.AddThemeConstantOverride("separation", 14);
        _menu.AddChild(v);
        var title = Ui.Label("Fantastik Dünya", 56, Ui.Gold, outline: true);
        title.HorizontalAlignment = HorizontalAlignment.Center;
        v.AddChild(title);
        var sub = Ui.Label("Dünya seni umursamıyor. Sen onun içinde bir şey ol.", 18, Ui.Dim);
        sub.HorizontalAlignment = HorizontalAlignment.Center;
        v.AddChild(sub);
        v.AddChild(Ui.Gap(24));
        bool hasSave = SaveGame.Exists();
        if (hasSave)
        {
            var info = SaveGame.Describe();
            v.AddChild(Ui.Button($"Devam et{(info != null ? "  ·  " + info : "")}", Continue, 22));
        }
        v.AddChild(Ui.Button("Yeni oyun (Demir mod)", () => { ShowOnly(_create); Refresh(); }, 22));
        v.AddChild(Ui.Button("Çık", () => GetTree().Quit(), 22));
        v.AddChild(Ui.Gap(18));
        var note = Ui.Label(hasSave ? "Demir mod: tek kayıt yuvası. Yeni oyun eskisinin üstüne yazılır." : "Demir mod: tek kayıt yuvası, oyun kendiliğinden kaydeder. Ölüm kalıcıdır.", 14, Ui.Faint, wrap: true);
        note.HorizontalAlignment = HorizontalAlignment.Center;
        v.AddChild(note);
    }

    void Continue()
    {
        ShowOnly(_gen);
        _genText.Text = "Kayıt açılıyor…";
        _genBar.Value = 0;
        CallDeferred(nameof(LoadSave));
    }

    void LoadSave()
    {
        try
        {
            SaveGame.Load();
            _genText.Text = "Bölge kuruluyor…";
            CallDeferred(nameof(GoRegionLater));
        }
        catch (Exception e)
        {
            GD.PushError($"[Boot] kayıt açılamadı: {e}");
            _genText.Text = $"Kayıt açılamadı: {e.Message}";
        }
    }

    // ------------------------------------------------------------------------------------------------ creation
    void BuildCreate()
    {
        _create = new MarginContainer();
        _create.SetAnchorsPreset(LayoutPreset.FullRect);
        foreach (var side in new[] { "margin_left", "margin_right" }) _create.AddThemeConstantOverride(side, 36);
        _create.AddThemeConstantOverride("margin_top", 18);
        _create.AddThemeConstantOverride("margin_bottom", 22);
        AddChild(_create);
        var root = new VBoxContainer();
        root.AddThemeConstantOverride("separation", 12);
        _create.AddChild(root);
        root.AddChild(Ui.Label("Karakter", 34, Ui.Gold, outline: true));
        var cols = new HBoxContainer { SizeFlagsVertical = SizeFlags.ExpandFill };
        cols.AddThemeConstantOverride("separation", 18);
        root.AddChild(cols);

        // left: race, class, alignment, name
        var left = Panel(cols, 470);
        left.AddChild(Ui.Label("Irk", 20, Ui.Gold));
        var rg = new GridContainer { Columns = 3 };
        rg.AddThemeConstantOverride("h_separation", 6); rg.AddThemeConstantOverride("v_separation", 6);
        left.AddChild(rg);
        foreach (var r in Rules.Races)
        {
            string id = r;
            var b = Ui.Button(Rules.RaceName(r), () => { _race = id; _look.Skin = LookKit.SkinsFor(id)[0]; RebuildSkins(); Refresh(); }, 17, true);
            b.CustomMinimumSize = new Vector2(140, 36);
            _raceBtns.Add(b); rg.AddChild(b);
        }
        _raceInfo = Ui.Label("", 14, Ui.Dim, wrap: true);
        _raceInfo.CustomMinimumSize = new Vector2(440, 0);
        left.AddChild(_raceInfo);
        left.AddChild(Ui.Gap(4));
        left.AddChild(Ui.Label("Sınıf", 20, Ui.Gold));
        var cg = new HBoxContainer();
        cg.AddThemeConstantOverride("separation", 6);
        left.AddChild(cg);
        foreach (var c in Rules.Classes)
        {
            string id = c;
            var b = Ui.Button(Rules.ClassName(c), () => { _cls = id; _base = Rules.SuggestedBase(id); Refresh(); }, 17, true);
            b.CustomMinimumSize = new Vector2(106, 36);
            _clsBtns.Add(b); cg.AddChild(b);
        }
        _clsInfo = Ui.Label("", 14, Ui.Dim, wrap: true);
        _clsInfo.CustomMinimumSize = new Vector2(440, 0);
        left.AddChild(_clsInfo);
        _spellBox = new VBoxContainer();
        left.AddChild(_spellBox);
        _spellBox.AddChild(Ui.Label("Kitabındaki 1. seviye büyü", 16, Ui.Gold));
        var sg = new HBoxContainer();
        sg.AddThemeConstantOverride("separation", 6);
        _spellBox.AddChild(sg);
        foreach (var sid in Spells.WizardFirst)
        {
            string id = sid;
            var b = Ui.Button(Spells.Get(sid).Name, () => { _spell = id; Refresh(); }, 15, true);
            b.CustomMinimumSize = new Vector2(140, 32);
            _spellBtns.Add(b); sg.AddChild(b);
        }
        left.AddChild(Ui.Gap(4));
        left.AddChild(Ui.Label("Hizalama", 20, Ui.Gold));
        var ag = new HBoxContainer();
        ag.AddThemeConstantOverride("separation", 6);
        left.AddChild(ag);
        foreach (var a in Rules.Aligns)
        {
            string id = a;
            var b = Ui.Button(Rules.AlignName(a), () => { _align = id; Refresh(); }, 17, true);
            b.CustomMinimumSize = new Vector2(140, 36);
            _alignBtns.Add(b); ag.AddChild(b);
        }
        left.AddChild(Ui.Gap(4));
        left.AddChild(Ui.Label("Ad", 20, Ui.Gold));
        var nh = new HBoxContainer();
        nh.AddThemeConstantOverride("separation", 6);
        left.AddChild(nh);
        _name = new LineEdit { CustomMinimumSize = new Vector2(320, 36), MaxLength = 40, PlaceholderText = "Adın" };
        _name.TextChanged += _ => Refresh();
        nh.AddChild(_name);
        nh.AddChild(Ui.Button("Rastgele", () => { _name.Text = CharacterFactory.RandomName(_race, _look.Female, _rnd); Refresh(); }, 16));

        // middle: stats
        var mid = Panel(cols, 470);
        var hdr = new HBoxContainer();
        hdr.AddChild(Ui.Label("Yetenekler", 20, Ui.Gold));
        var sp = new Control { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        hdr.AddChild(sp);
        _points = Ui.Label("", 18, Ui.Text);
        hdr.AddChild(_points);
        mid.AddChild(hdr);
        mid.AddChild(Ui.Label("27 puan: 8 bedava, 13'e dek puanı 1, 14 ve 15 ikişer. Irk bonusu üstüne eklenir.", 13, Ui.Faint, wrap: true));
        var grid = new GridContainer { Columns = 7 };
        grid.AddThemeConstantOverride("h_separation", 10);
        grid.AddThemeConstantOverride("v_separation", 6);
        mid.AddChild(grid);
        foreach (var h in new[] { "", "", "Taban", "", "Irk", "Toplam", "Değ." }) grid.AddChild(Ui.Label(h, 13, Ui.Faint));
        for (int i = 0; i < 6; i++)
        {
            int k = i;
            var nl = Ui.Label(Rules.StatTr[i], 17, Ui.Text);
            nl.CustomMinimumSize = new Vector2(120, 0);
            grid.AddChild(nl);
            var minus = Ui.Button("−", () => { if (_base[k] > Rules.BuyMin) { _base[k]--; Refresh(); } }, 18);
            minus.CustomMinimumSize = new Vector2(36, 32);
            grid.AddChild(minus);
            _statVal[i] = Ui.Label("", 18, Ui.Text); _statVal[i].CustomMinimumSize = new Vector2(30, 0); _statVal[i].HorizontalAlignment = HorizontalAlignment.Center;
            grid.AddChild(_statVal[i]);
            var plus = Ui.Button("+", () =>
            {
                if (_base[k] >= Rules.BuyMax) return;
                _base[k]++;
                if (Rules.Spent(_base) > Rules.PointBuy) _base[k]--;
                Refresh();
            }, 18);
            plus.CustomMinimumSize = new Vector2(36, 32);
            grid.AddChild(plus);
            _statRace[i] = Ui.Label("", 16, Ui.Good); grid.AddChild(_statRace[i]);
            _statTot[i] = Ui.Label("", 20, Ui.Gold); _statTot[i].HorizontalAlignment = HorizontalAlignment.Center; grid.AddChild(_statTot[i]);
            _statMod[i] = Ui.Label("", 17, Ui.Dim); grid.AddChild(_statMod[i]);
        }
        var sb = new HBoxContainer();
        sb.AddThemeConstantOverride("separation", 6);
        sb.AddChild(Ui.Button("Sınıfa göre dağıt", () => { _base = Rules.SuggestedBase(_cls); Refresh(); }, 15));
        sb.AddChild(Ui.Button("Sıfırla (hepsi 8)", () => { _base = new[] { 8, 8, 8, 8, 8, 8 }; Refresh(); }, 15));
        mid.AddChild(sb);
        mid.AddChild(Ui.Gap(6));
        mid.AddChild(Ui.Label("Başlangıç", 20, Ui.Gold));
        _summary = Ui.Label("", 15, Ui.Text, wrap: true);
        _summary.CustomMinimumSize = new Vector2(440, 0);
        mid.AddChild(_summary);

        // right: preview + look
        var right = Panel(cols, 400);
        _preview = new CharacterPreview { CustomMinimumSize = new Vector2(370, 330) };
        right.AddChild(_preview);
        right.AddChild(Ui.Label("sürükle: döndür", 12, Ui.Faint));
        var gh = new HBoxContainer();
        gh.AddThemeConstantOverride("separation", 12);
        right.AddChild(gh);
        _female = Ui.Button("Kadın", null, 16, true);
        _female.Toggled += on => { _look.Female = on; if (on && _look.HairStyle == "hair_short") _look.HairStyle = "hair_long"; if (on) _look.Beard = false; Refresh(); };
        gh.AddChild(_female);
        _beard = Ui.Button("Sakal", null, 16, true);
        _beard.Toggled += on => { _look.Beard = on; Refresh(); };
        gh.AddChild(_beard);
        right.AddChild(Ui.Label("Ten", 14, Ui.Dim));
        _skinRow = new HBoxContainer(); _skinRow.AddThemeConstantOverride("separation", 4); right.AddChild(_skinRow);
        right.AddChild(Ui.Label("Saç", 14, Ui.Dim));
        _hairRow = new HBoxContainer(); _hairRow.AddThemeConstantOverride("separation", 4); right.AddChild(_hairRow);
        foreach (int c in LookKit.HairColors) { int col = c; var b = Ui.Swatch(Ui.Hex(c), () => { _look.Hair = col; Refresh(); }, 28); _hairBtns.Add(b); _hairRow.AddChild(b); }
        var st = new HBoxContainer(); st.AddThemeConstantOverride("separation", 4); right.AddChild(st);
        foreach (var (id, nm) in new[] { ("hair_short", "Kısa"), ("hair_long", "Uzun"), ("hair_bun", "Topuz"), ("none", "Kel") })
        {
            string sid = id;
            var b = Ui.Button(nm, () => { _look.HairStyle = sid; Refresh(); }, 14, true);
            _styleBtns.Add(b); st.AddChild(b);
        }
        right.AddChild(Ui.Label("Giysi", 14, Ui.Dim));
        _tunicRow = new HBoxContainer(); _tunicRow.AddThemeConstantOverride("separation", 4); right.AddChild(_tunicRow);
        foreach (int c in LookKit.Tunics) { int col = c; var b = Ui.Swatch(Ui.Hex(c), () => { _look.Cloth1 = col; Refresh(); }, 28); _tunicBtns.Add(b); _tunicRow.AddChild(b); }
        var hh = new HBoxContainer(); right.AddChild(hh);
        hh.AddChild(Ui.Label("Boy", 14, Ui.Dim));
        _height = new HSlider { MinValue = 0.94, MaxValue = 1.06, Step = 0.01, Value = 1, CustomMinimumSize = new Vector2(260, 24) };
        _height.ValueChanged += v => { _look.Height = (float)v; Refresh(); };
        hh.AddChild(_height);

        // bottom bar
        var bar = new HBoxContainer();
        bar.AddThemeConstantOverride("separation", 10);
        root.AddChild(bar);
        bar.AddChild(Ui.Button("Geri", () => ShowOnly(_menu), 18));
        var fill = new Control { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        bar.AddChild(fill);
        bar.AddChild(Ui.Label("Dünya tohumu", 15, Ui.Dim));
        _seed = new LineEdit { CustomMinimumSize = new Vector2(140, 36), PlaceholderText = "rastgele" };
        bar.AddChild(_seed);
        _start = Ui.Button("Dünyayı yarat ve başla", StartGame, 20);
        _start.CustomMinimumSize = new Vector2(300, 44);
        bar.AddChild(_start);

        _look.Skin = LookKit.SkinsFor(_race)[2];
        _look.Hair = LookKit.HairColors[1];
        _look.Cloth1 = LookKit.Tunics[0];
        RebuildSkins();
        _name.Text = CharacterFactory.RandomName(_race, false, _rnd);
    }

    static VBoxContainer Panel(HBoxContainer parent, float w)
    {
        var p = new PanelContainer { CustomMinimumSize = new Vector2(w, 0), SizeFlagsVertical = SizeFlags.ExpandFill };
        parent.AddChild(p);
        var v = new VBoxContainer();
        v.AddThemeConstantOverride("separation", 8);
        p.AddChild(v);
        return v;
    }

    void RebuildSkins()
    {
        foreach (var b in _skinBtns) b.QueueFree();
        _skinBtns.Clear();
        foreach (int c in LookKit.SkinsFor(_race))
        {
            int col = c;
            var b = Ui.Swatch(Ui.Hex(c), () => { _look.Skin = col; Refresh(); }, 28);
            _skinBtns.Add(b); _skinRow.AddChild(b);
        }
        if (Array.IndexOf(LookKit.SkinsFor(_race), _look.Skin) < 0) _look.Skin = LookKit.SkinsFor(_race)[0];
    }

    Character Draft()
    {
        var c = CharacterFactory.Player(string.IsNullOrWhiteSpace(_name?.Text) ? "Adsız" : _name.Text.Trim(), _race, _cls, _align, _base, _look, _spell, 10);
        return c;
    }

    void Refresh()
    {
        if (_create == null) return;
        for (int i = 0; i < Rules.Races.Length; i++) _raceBtns[i].SetPressedNoSignal(Rules.Races[i] == _race);
        for (int i = 0; i < Rules.Classes.Length; i++) _clsBtns[i].SetPressedNoSignal(Rules.Classes[i] == _cls);
        for (int i = 0; i < Rules.Aligns.Length; i++) _alignBtns[i].SetPressedNoSignal(Rules.Aligns[i] == _align);
        for (int i = 0; i < Spells.WizardFirst.Length; i++) _spellBtns[i].SetPressedNoSignal(Spells.WizardFirst[i] == _spell);
        string[] styles = { "hair_short", "hair_long", "hair_bun", "none" };
        for (int i = 0; i < _styleBtns.Count; i++) _styleBtns[i].SetPressedNoSignal(styles[i] == _look.HairStyle);
        var skins = LookKit.SkinsFor(_race);
        for (int i = 0; i < _skinBtns.Count && i < skins.Length; i++) _skinBtns[i].SetPressedNoSignal(skins[i] == _look.Skin);
        for (int i = 0; i < _hairBtns.Count; i++) _hairBtns[i].SetPressedNoSignal(LookKit.HairColors[i] == _look.Hair);
        for (int i = 0; i < _tunicBtns.Count; i++) _tunicBtns[i].SetPressedNoSignal(LookKit.Tunics[i] == _look.Cloth1);
        _female.SetPressedNoSignal(_look.Female);
        _beard.SetPressedNoSignal(_look.Beard);
        var rl = FD.Life.Appearance.Look(_race);
        _beard.Disabled = rl.BeardMale <= 0;
        _spellBox.Visible = _cls == "wizard";

        int left = Rules.PointBuy - Rules.Spent(_base);
        _points.Text = $"Kalan puan: {left}";
        _points.AddThemeColorOverride("font_color", left == 0 ? Ui.Good : left < 0 ? Ui.Bad : Ui.Gold);
        for (int i = 0; i < 6; i++)
        {
            int rb = Rules.RaceBonus(_race, i), tot = _base[i] + rb;
            _statVal[i].Text = _base[i].ToString();
            _statRace[i].Text = rb != 0 ? Rules.Signed(rb) : "";
            _statTot[i].Text = tot.ToString();
            _statMod[i].Text = Rules.Signed(Rules.Mod(tot));
        }
        _raceInfo.Text = RaceText(_race);
        _clsInfo.Text = ClassText(_cls);
        var c = Draft();
        var at = c.Attack();
        string spells = c.Spells.Count > 0 ? "\nBüyüler: " + string.Join(", ", c.Spells.ConvertAll(s => Spells.Get(s).Name + (Spells.Get(s).Level == 0 ? "" : " (1. sv)"))) + (c.SlotsMax > 0 ? $"; 1. seviye yuva {c.SlotsMax}" : "") : "";
        _summary.Text = $"Can {c.MaxHp} · Zırh sınıfı {c.Ac} (zırhsız) · Hız {(Rules.SpeedFactor(_race) < 1 ? "yavaş (kısa boylu)" : "olağan")}\n" +
                        $"Saldırı: {at.name} {Rules.Signed(at.atk)} isabet, {(at.n == 1 && at.sides == 1 ? $"{1 + at.bonus} hasar" : $"{at.n}d{at.sides}{(at.bonus != 0 ? Rules.Signed(at.bonus) : "")}")}\n" +
                        $"Taşıma: {c.Capacity:F0} kg · İnanç: {Rules.FaithName(c.Faith)}{spells}\n" +
                        "Üstündeki yol giysisi ve 5–15 gümüş. Silah yok: ilk iş, bir silah bulmak ya da kazanmak.";
        _start.Disabled = left < 0 || string.IsNullOrWhiteSpace(_name.Text);
        _preview.Show(c);
    }

    static string RaceText(string r) => r switch
    {
        "human" => "İnsan: her yetenekte +1. Kısa ömürlü, her yerde.",
        "dwarf" => "Cüce: Güç ve Dayanıklılık +2. Kısa boylu ve yavaş, ama taş gibi.",
        "elf" => "Elf: Çeviklik +2, Zekâ +1. Uzun ömürlü, ince ve hızlı.",
        "halfling" => "Buçukluk: Çeviklik +2, Karizma +1. Küçük ve yavaş, şanslı ve sessiz; az yük taşır.",
        "gnome" => "Gnom: Zekâ +2, Dayanıklılık +1. Küçük ve yavaş, meraklı; az yük taşır.",
        "halfelf" => "Yarı-elf: Karizma +2, Çeviklik ve Dayanıklılık +1. İki dünyanın arasında.",
        "halforc" => "Yarı-ork: Güç +2, Dayanıklılık +1. İri, sert; köylüler yan bakabilir.",
        "dragonborn" => "Ejderdoğan: Güç +2, Karizma +1. Pullu ve gururlu.",
        "tiefling" => "Tiefling: Karizma +2, Zekâ +1. Kızıl tenli; Güneş Kilisesi'nin gözü üstünde.",
        _ => "",
    };

    static string ClassText(string c) => c switch
    {
        "fighter" => "Savaşçı (can zarı d10): öne atılır, darbeyi karşılar. Bir kez derin nefes (1d10+seviye can). Ana yetenek Güç.",
        "rogue" => "Haydut (d8): arkadan ya da dostunun yanındaki hedefe sinsi saldırı (+1d6; hançer, kısa kılıç, yay ister). Ana yetenek Çeviklik.",
        "wizard" => "Büyücü (d6): mesafe korur. Kitabında iki basit büyü (Ateş Oku, Buz Işını) ve seçtiği bir 1. seviye büyü; günde 2 yuva. Ana yetenek Zekâ.",
        "cleric" => "Rahip (d8): Güneş Kilisesi'ne bağlı. Kutsal Alev ve Yara Sarma duaları; günde 2 yuva. Yaralıyı kaldırır. Ana yetenek Bilgelik.",
        _ => "",
    };

    // ------------------------------------------------------------------------------------------------ world generation
    void BuildGen()
    {
        _gen = new CenterContainer();
        _gen.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(_gen);
        var v = new VBoxContainer { CustomMinimumSize = new Vector2(620, 0) };
        v.AddThemeConstantOverride("separation", 14);
        _gen.AddChild(v);
        var t = Ui.Label("Dünya doğuyor", 34, Ui.Gold, outline: true);
        t.HorizontalAlignment = HorizontalAlignment.Center;
        v.AddChild(t);
        _genText = Ui.Label("", 17, Ui.Dim, wrap: true);
        _genText.HorizontalAlignment = HorizontalAlignment.Center;
        _genText.CustomMinimumSize = new Vector2(600, 0);
        v.AddChild(_genText);
        _genBar = new ProgressBar { MinValue = 0, MaxValue = 1, Step = 0.001, CustomMinimumSize = new Vector2(600, 22), ShowPercentage = false };
        v.AddChild(_genBar);
    }

    void StartGame()
    {
        _pending = Draft();
        _pending.Inv.Silver = 5 + _rnd.Next(11);
        double seed = double.TryParse(_seed.Text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var sd) ? sd : _rnd.Next(1, 999999);
        ShowOnly(_gen);
        _genDay = 0;
        _genT = 0;
        _genText.Text = $"Tohum {seed}. Tarih öncesi: devletler kuruluyor, şehirler yükseliyor ve yıkılıyor…";
        _genTask = Task.Run(() => Session.NewWorld(seed, (d, total) => { _genDay = d; _genTotal = total; }));
    }

    public override void _Process(double delta)
    {
        if (_genTask == null) return;
        _genT += delta;
        int total = Math.Max(1, _genTotal);
        _genBar.Value = Math.Min(1, _genDay / (double)total);
        if (!_genTask.IsCompleted)
        {
            int years = _genDay / FD.Macro.Sim.YEAR;
            _genText.Text = $"Tarih öncesi: {years}. yıl ({_genDay} / {total} gün) · {_genT:F0} sn";
            return;
        }
        var task = _genTask;
        _genTask = null;
        if (task.IsFaulted)
        {
            _genText.Text = $"Dünya doğamadı: {task.Exception?.GetBaseException().Message}";
            GD.PushError(task.Exception?.ToString());
            return;
        }
        var s = task.Result;
        s.BaseLocalDay = GameClock.Day;
        s.AddPlayer(_pending);
        Session.Current = s;
        var vi = s.Village();
        _genText.Text = $"{vi?.Name} köyü, {vi?.CivName}. {vi?.RulerTitle}. Bölge kuruluyor…";
        _genBar.Value = 1;
        SaveGame.Save(s, "yeni dünya");
        CallDeferred(nameof(GoRegionLater));
    }

    void GoRegionLater()
    {
        // let the "Bölge kuruluyor…" frame show before the (synchronous) region build
        GetTree().CreateTimer(0.1).Timeout += GoRegion;
    }
}
