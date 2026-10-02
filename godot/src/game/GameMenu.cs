using System;
using Godot;
using FD.UI;
using FD.World;

namespace FD.Game;

/// <summary>
/// Faz 2 H: the in-game menu (Esc) and iron-mode autosave. One slot, written by itself: when the game is closed (also mid-fight:
/// the fight's hit points are written to the characters first), every new day, and after the moments that matter (a fight, a
/// hire, a night's sleep, a camp, a quest, the reward, the chest). The menu: continue, settings (auto-pause at the start of a fight,
/// dice over heads), save and go to the title, save and quit. There is no loading of older saves.
/// </summary>
public partial class GameMenu : PanelLayer
{
    Label _info;
    Button _pause, _dice, _slow, _edge;
    bool _quitting;

    public void Init(Region r)
    {
        Build(r, "GameMenu", new Vector2(520, 0));
        Body.AddChild(L("Oyun", 28, Ui.Gold));
        _info = L("", 15, Ui.Dim); _info.AutowrapMode = TextServer.AutowrapMode.WordSmart; _info.CustomMinimumSize = new Vector2(480, 0);
        Body.AddChild(_info);
        Body.AddChild(new HSeparator());
        Body.AddChild(Ui.Button("Devam", Close, 18));
        Body.AddChild(L("Ayarlar", 18, Ui.Gold));
        _pause = Ui.Button("", () => { Settings.Current.AutoPause = !Settings.Current.AutoPause; Settings.Save(); Refresh(); }, 16, toggle: true);
        Body.AddChild(_pause);
        _dice = Ui.Button("", () => { Settings.Current.DiceOverHeads = !Settings.Current.DiceOverHeads; Settings.Save(); Refresh(); }, 16, toggle: true);
        Body.AddChild(_dice);
        _slow = Ui.Button("", () => { Settings.Current.SlowOnThreat = !Settings.Current.SlowOnThreat; Settings.Save(); Refresh(); }, 16, toggle: true);
        Body.AddChild(_slow);
        _edge = Ui.Button("", () => { Settings.Current.EdgeScroll = !Settings.Current.EdgeScroll; Settings.Save(); Refresh(); }, 16, toggle: true);
        Body.AddChild(_edge);
        Body.AddChild(L("Tuşlar: Boşluk duraklat · + / − hız · sol tık seç / bilgi · sağ tık emir · WASD yürü · teker yakınlaş · orta tuş ya da Alt+sürükle döndür · F izle · 1–4 seç · Tab sıradaki · Shift+1…5 duruş · L zar günlüğü · I envanter · M harita · E yakındakiyle etkileşim", 13, Ui.Faint));
        Body.AddChild(new HSeparator());
        Body.AddChild(Ui.Button("Kaydet ve ana menüye dön", () => SaveAndLeave(false), 18));
        Body.AddChild(Ui.Button("Kaydet ve çık", () => SaveAndLeave(true), 18));
        GetTree().AutoAcceptQuit = false;
        r.Director.NewDay += _ => Autosave("yeni gün");
    }

    public override void _ExitTree() { if (!_quitting && IsInsideTree()) GetTree().AutoAcceptQuit = true; }

    protected override void Refresh()
    {
        var s = R.Session;
        string last = SaveGame.LastSaved != null ? $"Son kayıt: {SaveGame.LastSaved}." : "";
        _info.Text = $"{s.Player?.FullName} · {GameClock.TimeString} · {s.Names().village}\nDemir mod: tek kayıt yuvası; oyun kendiliğinden kaydeder (çıkışta, her gün, savaştan, uykudan, kamptan sonra). Ölüm kalıcıdır. {last}";
        _pause.Text = $"Savaş başında duraklat: {(Settings.Current.AutoPause ? "açık" : "kapalı")}";
        _pause.ButtonPressed = Settings.Current.AutoPause;
        _dice.Text = $"Zar sayıları başların üstünde: {(Settings.Current.DiceOverHeads ? "açık" : "kapalı")}";
        _dice.ButtonPressed = Settings.Current.DiceOverHeads;
        _slow.Text = $"Tehlike görününce hız 1×'e düşsün: {(Settings.Current.SlowOnThreat ? "açık" : "kapalı")}";
        _slow.ButtonPressed = Settings.Current.SlowOnThreat;
        _edge.Text = $"Ekran kenarında kamera kaysın: {(Settings.Current.EdgeScroll ? "açık" : "kapalı")}";
        _edge.ButtonPressed = Settings.Current.EdgeScroll;
    }

    public void Open() { Show(); R.Commander?.MenuOpened(); }

    public override void Close()
    {
        bool was = Visible;
        base.Close();
        if (was) R.Commander?.MenuClosed();
    }

    public override void _UnhandledInput(InputEvent e)
    {
        if (!Visible)
        {
            if (e.IsActionPressed("release_mouse") && OpenPanel == null && R.Combat?.Active != true && R.Hud != null && !R.Hud.MapOpen)
            {
                Open();
                GetViewport().SetInputAsHandled();
            }
            return;
        }
        base._UnhandledInput(e);
    }

    /// <summary>Save now unless the world has ended or a fight is on (the fight saves when it ends).</summary>
    public void Autosave(string why)
    {
        if (Session.Current == null || R.Combat?.Active == true) return;
        SaveGame.Save(R.Session, why);
    }

    void SaveAndLeave(bool quit)
    {
        if (Session.Current != null) SaveGame.Save(R.Session, quit ? "çıkış" : "ana menü");
        _quitting = true;
        GetTree().Paused = false;
        Engine.TimeScale = 1;
        if (quit) GetTree().Quit();
        else { Session.Current = null; GetTree().ChangeSceneToFile("res://scenes/Boot.tscn"); }
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            if (Session.Current != null)
            {
                R.Combat?.WriteBackNow();
                SaveGame.Save(R.Session, "pencere kapandı");
            }
            _quitting = true;
            GetTree().Quit();
        }
    }
}
