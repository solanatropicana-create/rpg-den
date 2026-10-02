using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using FD.Game;
using FD.Rpg;
using FD.World;

namespace FD.UI;

/// <summary>Base of the full-screen panels (inventory, trade): frees the mouse and stops the hero while open.</summary>
public abstract partial class PanelLayer : CanvasLayer
{
    protected Region R;
    protected Control Root;
    protected PanelContainer Frame;
    protected VBoxContainer Body;
    public bool IsOpen => Visible;
    public static PanelLayer OpenPanel { get; private set; }

    protected void Build(Region r, string name, Vector2 size)
    {
        R = r;
        Name = name;
        Layer = 8;
        Visible = false;
        Root = new Control { MouseFilter = Control.MouseFilterEnum.Ignore, Theme = Ui.MakeTheme(16) };
        Root.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        AddChild(Root);
        var dim = new ColorRect { Color = new Color(0, 0, 0, 0.45f), MouseFilter = Control.MouseFilterEnum.Stop };
        dim.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        Root.AddChild(dim);
        Frame = new PanelContainer { CustomMinimumSize = size };
        Frame.SetAnchorsPreset(Control.LayoutPreset.Center);
        Frame.GrowHorizontal = Control.GrowDirection.Both; Frame.GrowVertical = Control.GrowDirection.Both;
        Root.AddChild(Frame);
        Body = new VBoxContainer();
        Body.AddThemeConstantOverride("separation", 6);
        Frame.AddChild(Body);
    }

    protected void Show()
    {
        if (OpenPanel != null && OpenPanel != this) OpenPanel.Close();
        OpenPanel = this;
        Visible = true;
        R.Player.InputEnabled = false;
        Refresh();
    }

    public virtual void Close()
    {
        if (!Visible) return;
        Visible = false;
        if (OpenPanel == this) OpenPanel = null;
        if (R.Combat?.Active != true) R.Player.InputEnabled = true;
    }

    protected abstract void Refresh();

    protected void Clear(Container c) { foreach (var n in c.GetChildren()) { c.RemoveChild(n); n.QueueFree(); } }

    protected static Label L(string t, int size = 16, Color? c = null, bool wrap = false) => Ui.Label(t, size, c, wrap);

    public override void _UnhandledInput(InputEvent e)
    {
        if (!Visible) return;
        if (e.IsActionPressed("release_mouse") || (e is InputEventKey k && k.Pressed && !k.Echo && CloseKey(k.PhysicalKeycode)))
        {
            Close();
            GetViewport().SetInputAsHandled();
        }
        else if (e is InputEventKey or InputEventMouseButton) GetViewport().SetInputAsHandled();
    }

    protected virtual bool CloseKey(Key k) => k == Key.Escape;
}

/// <summary>
/// Faz 2 F: the party's packs (I). Each member's items with weight and value; equip/unequip weapons, armour and shields; drink a
/// potion, bind a wound with a bandage (1d4), give an item to another member, throw it away. The load (Strength × 7 kg; small folk ×¾)
/// slows: over half 15 %, over the limit 40 %. "Kamp kur": eight hours' rest anywhere safe for a ration each (HP and spells back;
/// a companion of the opposite alignment leaves).
/// </summary>
public partial class InventoryPanel : PanelLayer
{
    Character _who;
    HBoxContainer _tabs;
    Label _head, _load, _gear;
    VBoxContainer _rows, _book;
    Label _msg;

    public void Init(Region r)
    {
        Build(r, "InventoryPanel", new Vector2(820, 600));
        var title = L("Envanter", 26, Ui.Gold);
        Body.AddChild(title);
        _tabs = new HBoxContainer(); _tabs.AddThemeConstantOverride("separation", 6); Body.AddChild(_tabs);
        _head = L(""); Body.AddChild(_head);
        _load = L("", 15, Ui.Dim); Body.AddChild(_load);
        _gear = L("", 15, Ui.Dim); _gear.AutowrapMode = TextServer.AutowrapMode.WordSmart; Body.AddChild(_gear);
        // Tur 1 C: the spellbook (the caster uses the first spell that fits, top down; the player only orders and switches them)
        _book = new VBoxContainer();
        _book.AddThemeConstantOverride("separation", 2);
        Body.AddChild(_book);
        Body.AddChild(new HSeparator());
        var scroll = new ScrollContainer { CustomMinimumSize = new Vector2(780, 330), HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled };
        Body.AddChild(scroll);
        _rows = new VBoxContainer { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
        _rows.AddThemeConstantOverride("separation", 4);
        scroll.AddChild(_rows);
        Body.AddChild(new HSeparator());
        _msg = L("", 15, new Color(1f, 0.9f, 0.6f)); _msg.AutowrapMode = TextServer.AutowrapMode.WordSmart; Body.AddChild(_msg);
        var foot = new HBoxContainer(); foot.AddThemeConstantOverride("separation", 10); Body.AddChild(foot);
        foot.AddChild(Ui.Button("Kamp kur (8 saat, kişi başı 1 erzak)", Camp, 16));
        foot.AddChild(Ui.Button("Kapat  [I]", Close, 16));
    }

    public void Toggle() { if (Visible) Close(); else { _who = R.Player.Character; _msg.Text = ""; Show(); } }
    protected override bool CloseKey(Key k) => k is Key.Escape or Key.I;

    protected override void Refresh()
    {
        var s = R.Session;
        _who ??= R.Player.Character;
        Clear(_tabs);
        foreach (var c in s.Party.Where(c => !c.Dead))
        {
            var cc = c;
            var b = Ui.Button(c == R.Player.Character ? $"▶ {c.Name}" : c.Name, () => { _who = cc; Refresh(); }, 15, toggle: true);
            b.ButtonPressed = c == _who;
            _tabs.AddChild(b);
        }
        var w = _who;
        _head.Text = $"{w.FullName} · {Rules.RaceName(w.Race)} {Rules.ClassName(w.Cls)} Sv{w.Level} · can {w.Hp}/{w.MaxHp} · ZS {w.Ac} · kese {Rules.Money(w.Inv.Silver)}";
        float kg = w.Inv.Weight, cap = w.Capacity;
        _load.Text = $"Yük: {kg:F1} / {cap:F0} kg{(w.Overloaded ? "  — AŞIRI YÜK: %40 yavaş" : kg > cap * 0.5f ? "  — ağır: %15 yavaş" : "")}" +
                     (w.SlotsMax > 0 ? $" · büyü yuvası {w.Slots}/{w.SlotsMax}" : "") + (w.Spells.Count > 0 ? $" · büyüler: {string.Join(", ", w.Spells.Select(x => Spells.Get(x)?.Name))}" : "");
        var at = w.Attack();
        string wounds = w.Wounds.Count > 0 ? $"\nYaralar: {string.Join("; ", w.Wounds.Select(x => $"{Wound.Name(x.Kind)} ({Wound.Effect(x.Kind)})"))}" : "";
        _gear.Text = $"Silah: {Items.Get(w.Weapon)?.Name ?? "yok (yumruk)"} — saldırı {Rules.Signed(at.atk)}, {at.n}d{at.sides}{Rules.Signed(at.bonus)}{(at.range > 0 ? $", {at.range:F0} m" : "")} · Zırh: {Items.Get(w.Armor)?.Name ?? "yok"} · Kalkan: {Items.Get(w.Shield)?.Name ?? "yok"}{wounds}";
        Clear(_book);
        if (w.Spells.Count > 0)
        {
            _book.AddChild(L($"Büyü kitabı — {w.Name} savaşta yukarıdan aşağı, işe yarayan ilk büyüyü kendisi yapar (yuvalar elverdikçe). Sen sırayı ve açık/kapalıyı düzenlersin.", 14, Ui.Dim, true));
            var book = w.BookAll();
            for (int i = 0; i < book.Count; i++)
            {
                var sp = Spells.Get(book[i]);
                if (sp == null) continue;
                string id = sp.Id;
                bool off = w.SpellOff.Contains(id);
                var row = new HBoxContainer(); row.AddThemeConstantOverride("separation", 6);
                var nm = L($"{i + 1}. {sp.Name}{(sp.Level > 0 ? " (yuva)" : "")}", 15, off ? Ui.Faint : Ui.Text);
                nm.CustomMinimumSize = new Vector2(220, 0); nm.TooltipText = sp.Desc; nm.MouseFilter = Control.MouseFilterEnum.Pass;
                row.AddChild(nm);
                var up = Ui.Button("▲", () => { w.MoveSpell(id, -1); Refresh(); }, 13); up.Disabled = i == 0; row.AddChild(up);
                var dn = Ui.Button("▼", () => { w.MoveSpell(id, +1); Refresh(); }, 13); dn.Disabled = i == book.Count - 1; row.AddChild(dn);
                row.AddChild(Ui.Button(off ? "kapalı" : "açık", () => { w.ToggleSpell(id); Refresh(); }, 13, toggle: true));
                var when = L(sp.Id switch
                {
                    "sleep" => "3+ düşman bir aradayken", "burninghands" => "2+ düşman önünde, dibindeyken", "magicmissile" => "şef ya da can çekişen düşmana",
                    "curewounds" => "yere düşen ya da ağır yaralı yoldaşa", _ => "menzildeki düşmana",
                }, 13, Ui.Faint);
                row.AddChild(when);
                _book.AddChild(row);
            }
        }
        Clear(_rows);
        if (w.Inv.Items.Count == 0) _rows.AddChild(L("Sırt çantası boş.", 15, Ui.Faint));
        foreach (var st in w.Inv.Items.ToList())
        {
            var d = st.Def;
            if (d == null) continue;
            var row = new HBoxContainer(); row.AddThemeConstantOverride("separation", 8);
            bool eq = w.Weapon == d.Id || w.Armor == d.Id || w.Shield == d.Id;
            var name = L($"{(eq ? "● " : "")}{d.Name}{(st.Count > 1 ? $" ×{st.Count}" : "")}", 16, eq ? Ui.Gold : Ui.Text);
            name.CustomMinimumSize = new Vector2(230, 0);
            name.TooltipText = d.Desc; name.MouseFilter = Control.MouseFilterEnum.Pass;
            row.AddChild(name);
            var info = L($"{d.Weight * st.Count:F1} kg · {Rules.Money(Economy.PriceOf(R.Session, d.Id))}{(d.IsWeapon ? $" · {d.DamageText}" : d.ArmorBase > 0 ? $" · ZS {(d.Kind == ItemKind.Shield ? "+" : "")}{d.ArmorBase}" : "")}", 14, Ui.Dim);
            info.CustomMinimumSize = new Vector2(220, 0);
            row.AddChild(info);
            string id = d.Id;
            if (d.Kind is ItemKind.Weapon or ItemKind.Armor or ItemKind.Shield)
                row.AddChild(Ui.Button(eq ? "Çıkar" : "Kuşan", () => { Equip(w, id, !eq); }, 14));
            if (d.Kind == ItemKind.Potion) row.AddChild(Ui.Button("İç", () => Use(w, id), 14));
            if (d.Kind == ItemKind.Bandage) row.AddChild(Ui.Button("Sar", () => Use(w, id), 14));
            var others = R.Session.Party.Where(c => c != w && !c.Dead && !c.Captive).ToList();
            if (others.Count > 0) row.AddChild(Ui.Button($"→ {others[0].Name.Split(' ')[0]}", () => Give(w, others[0], id), 14));
            row.AddChild(Ui.Button("At", () => { if (eq) Equip(w, id, false); w.Inv.Remove(id); _msg.Text = $"{Items.Get(id).Name} atıldı."; Refresh(); }, 14));
            _rows.AddChild(row);
        }
    }

    void Equip(Character w, string id, bool on)
    {
        var d = Items.Get(id);
        if (on && d.Kind == ItemKind.Armor && d.StrReq > 0 && w.Stats[Rules.STR] < d.StrReq) { _msg.Text = $"{d.Name} Güç {d.StrReq} ister; yine de giyildi ama ağır."; }
        switch (d.Kind)
        {
            case ItemKind.Weapon: w.Weapon = on ? id : null; if (on && d.TwoHanded && w.Shield != null) { w.Shield = null; _msg.Text = "İki elle tutulur: kalkan çıkarıldı."; } break;
            case ItemKind.Armor: w.Armor = on ? id : null; break;
            case ItemKind.Shield:
                if (on && Items.Get(w.Weapon)?.TwoHanded == true) { _msg.Text = "İki elli silahla kalkan tutulmaz."; return; }
                w.Shield = on ? id : null; break;
        }
        if (w == R.Player.Character) R.Player.SetCharacter(w);
        else R.Companions.FirstOrDefault(c => c.Char == w)?.Become(w);
        Refresh();
    }

    void Use(Character w, string id)
    {
        var d = Items.Get(id);
        var r = new FD.Macro.Rng(GameClock.TotalHours * 977 + w.Id);
        if (d.Kind == ItemKind.Potion)
        {
            if (w.Hp >= w.MaxHp) { _msg.Text = $"{w.Name} zaten sağlıklı."; return; }
            int heal = (int)(r.Int(1, 4) + r.Int(1, 4)) + 2;
            w.Inv.Remove(id); w.Hp = Math.Min(w.MaxHp, w.Hp + heal);
            _msg.Text = $"{w.Name} şifa iksirini içti: +{heal} can ({w.Hp}/{w.MaxHp}).";
        }
        else if (d.Kind == ItemKind.Bandage)
        {
            if (w.Hp >= w.MaxHp) { _msg.Text = $"{w.Name} yarasız."; return; }
            int heal = (int)r.Int(1, 4);
            w.Inv.Remove(id); w.Hp = Math.Min(w.MaxHp, w.Hp + heal);
            _msg.Text = $"{w.Name} yarasını sardı: +{heal} can ({w.Hp}/{w.MaxHp}).";
        }
        Refresh();
    }

    void Give(Character from, Character to, string id)
    {
        bool eq = from.Weapon == id || from.Armor == id || from.Shield == id;
        if (eq && from.Inv.Count(id) == 1) Equip(from, id, false);
        if (!from.Inv.Remove(id)) return;
        to.Inv.Add(id);
        _msg.Text = $"{Items.Get(id).Name}: {from.Name} → {to.Name}.";
        Refresh();
    }

    /// <summary>Eight hours' rest outside (a ration each): HP, spell slots and second wind back; alignment can part the party.</summary>
    void Camp()
    {
        var s = R.Session;
        if (R.Combat?.Active == true) return;
        var here = new System.Numerics.Vector2(R.Player.GlobalPosition.X, R.Player.GlobalPosition.Z);
        var gob = R.Life.People.FirstOrDefault(p => p.Role == FD.Sim.Life.Role.Goblin && !p.Dead && p.Visible && System.Numerics.Vector2.Distance(p.Pos, here) < 45f);
        if (gob != null) { _msg.Text = $"Burada kamp kurulmaz: {gob.Name} çok yakında."; return; }
        if (R.Player.Character.Captive) { _msg.Text = "Kafeste kamp kurulmaz."; return; }
        var members = s.Party.Where(c => !c.Dead && !c.Captive).ToList();
        int have = members.Sum(c => c.Inv.Count("ration"));
        if (have < members.Count) { _msg.Text = $"Erzak yetmiyor: {members.Count} kişiye {have} erzak var (handa satılır)."; return; }
        int need = members.Count;
        foreach (var c in members) while (need > 0 && c.Inv.Remove("ration")) need--;
        GameClock.SetTotalHours(GameClock.TotalHours + 8);
        foreach (var c in members) c.Rest();
        R.Party.OnRest();
        _msg.Text = $"Ateş yakıldı, nöbet tutuldu; sekiz saat sonra herkes dinç ({GameClock.TimeString}).";
        SaveGame.Save(s, "kamp");
        Refresh();
    }
}

/// <summary>Faz 2 F: buying and selling with a keeper (inn, smith, temple): their stock and prices today come from the sim.</summary>
public partial class TradePanel : PanelLayer
{
    Shop _shop;
    Character _who;
    Label _title, _info, _purse, _msg;
    VBoxContainer _sell, _mine;

    public void Init(Region r)
    {
        Build(r, "TradePanel", new Vector2(980, 600));
        _title = L("", 26, Ui.Gold); Body.AddChild(_title);
        _info = L("", 14, Ui.Dim); _info.AutowrapMode = TextServer.AutowrapMode.WordSmart; Body.AddChild(_info);
        var cols = new HBoxContainer(); cols.AddThemeConstantOverride("separation", 18); Body.AddChild(cols);
        VBoxContainer Col(string t)
        {
            var v = new VBoxContainer { CustomMinimumSize = new Vector2(470, 400) };
            v.AddChild(L(t, 18, Ui.Gold));
            var sc = new ScrollContainer { CustomMinimumSize = new Vector2(470, 380), HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled };
            v.AddChild(sc);
            var inner = new VBoxContainer { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
            inner.AddThemeConstantOverride("separation", 4);
            sc.AddChild(inner);
            cols.AddChild(v);
            return inner;
        }
        _sell = Col("Satılık");
        _mine = Col("Senin çantan");
        _msg = L("", 15, new Color(1f, 0.9f, 0.6f)); Body.AddChild(_msg);
        var foot = new HBoxContainer(); foot.AddThemeConstantOverride("separation", 10); Body.AddChild(foot);
        _purse = L("", 17, Ui.Gold); _purse.CustomMinimumSize = new Vector2(300, 0); foot.AddChild(_purse);
        foot.AddChild(Ui.Button("Kapat  [Esc]", Close, 16));
    }

    public void Open(Shop shop)
    {
        _shop = shop;
        _who = R.Player.Character;
        _msg.Text = "";
        Show();
    }

    protected override void Refresh()
    {
        var s = R.Session;
        _title.Text = $"{_shop.Name} — {_shop.Keeper}";
        string good = _shop.Kind == ShopKind.Smith ? "arms" : _shop.Kind == ShopKind.Inn ? "bread" : "potion";
        double ratio = Economy.Ratio(s, good);
        _info.Text = $"Fiyatlar {s.Civ?.Name ?? "köyün"} pazarından: {(_shop.Kind == ShopKind.Smith ? "silah" : _shop.Kind == ShopKind.Inn ? "ekmek" : "iksir")} bugün olağan fiyatın ×{ratio:F2}'i. Stok devletin ambarından. " +
                     (_shop.Kind == ShopKind.Smith ? "Kullanılmış silah ve zırhı yarı fiyatına alır." : _shop.Kind == ShopKind.Inn ? "Şifalı otu ve ıvır zıvırı tam fiyata, erzak ve iksiri yarıya alır." : "Şifalı otu tam fiyata alır.");
        _purse.Text = $"Kesen: {Rules.Money(_who.Inv.Silver)} · yük {_who.Inv.Weight:F1}/{_who.Capacity:F0} kg";
        Clear(_sell);
        foreach (var o in _shop.Offers)
        {
            var d = o.Def; if (d == null) continue;
            var row = new HBoxContainer(); row.AddThemeConstantOverride("separation", 8);
            var n = L($"{d.Name}", 16, o.Stock > 0 ? Ui.Text : Ui.Faint); n.CustomMinimumSize = new Vector2(170, 0); n.TooltipText = d.Desc; n.MouseFilter = Control.MouseFilterEnum.Pass; row.AddChild(n);
            var i = L($"{(o.Stock > 0 ? $"{o.Stock} tane" : "yok")} · {Rules.Money(o.Buy)}{(d.IsWeapon ? $" · {d.DamageText}" : d.ArmorBase > 0 ? $" · ZS {d.ArmorBase}" : "")}", 14, Ui.Dim);
            i.CustomMinimumSize = new Vector2(200, 0); row.AddChild(i);
            string id = o.Id;
            var b = Ui.Button("Al", () => { if (Economy.Buy(s, _shop, id, _who, out var why)) _msg.Text = $"{Items.Get(id).Name} alındı."; else _msg.Text = why; Refresh(); }, 14);
            b.Disabled = o.Stock <= 0 || _who.Inv.Silver < o.Buy;
            row.AddChild(b);
            _sell.AddChild(row);
        }
        Clear(_mine);
        foreach (var st in _who.Inv.Items.ToList())
        {
            var d = st.Def; if (d == null) continue;
            int p = Economy.SellPrice(s, _shop, d.Id);
            var row = new HBoxContainer(); row.AddThemeConstantOverride("separation", 8);
            var n = L($"{d.Name}{(st.Count > 1 ? $" ×{st.Count}" : "")}", 16, p > 0 ? Ui.Text : Ui.Faint); n.CustomMinimumSize = new Vector2(190, 0); row.AddChild(n);
            var i = L(p > 0 ? $"alır: {Rules.Money(p)}" : "almaz", 14, Ui.Dim); i.CustomMinimumSize = new Vector2(150, 0); row.AddChild(i);
            string id = d.Id;
            var b = Ui.Button("Sat", () => { if (Economy.Sell(s, _shop, id, _who, out var why)) _msg.Text = $"{Items.Get(id).Name} satıldı: +{Rules.Money(p)}."; else _msg.Text = why; Refresh(); }, 14);
            b.Disabled = p <= 0;
            row.AddChild(b);
            if (p > 0 && st.Count > 1)
                row.AddChild(Ui.Button("Hepsini sat", () => { int k = 0; while (_who.Inv.Has(id) && Economy.Sell(s, _shop, id, _who, out _)) k++; _msg.Text = $"{k} {Items.Get(id).Name.ToLowerInvariant()} satıldı."; Refresh(); }, 14));
            _mine.AddChild(row);
        }
        if (_who.Inv.Items.Count == 0) _mine.AddChild(L("Çantan boş.", 15, Ui.Faint));
    }
}
