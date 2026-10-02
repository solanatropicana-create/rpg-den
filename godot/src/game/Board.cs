using System;
using System.Linq;
using Godot;
using FD.Core;
using FD.Rpg;
using FD.UI;
using FD.World;
using M = FD.Macro;

namespace FD.Game;

/// <summary>
/// Faz 2 G: the notice board by the inn's steps. It shows what the sim has posted: the bound camp's quest (who posted it, the bounty,
/// whether it is still open, which heroes took it and are on their way, or that it is done), the inn's other quests (far away), and
/// a reward waiting. The player can tear off the camp's quest (<c>Local.TakeQuest</c>): no hero of the sim can take it then, the
/// reward is the player's when the camp falls. A sheet of paper per open quest is pinned on the board.
/// </summary>
public partial class Board : Node3D
{
    Region _r;
    Session _s;
    Vector2 _pos;
    Node3D _papers;
    BoardPanel _panel;

    public void Init(Region r)
    {
        _r = r; _s = r.Session;
        Name = "Board";
        _pos = InnSite.BoardPos;
        BuildMesh();
        _panel = new BoardPanel();
        r.AddChild(_panel);
        _panel.Init(r, this);
        r.Hud.Prompts.Add(Prompt);
        Refresh();
    }

    void BuildMesh()
    {
        var hf = _r.Heightfield;
        var k = new MeshKit();
        var wood = MeshKit.C(0x5a3f28); var dark = MeshKit.C(0x3f2a1a);
        k.Box(new Vector3(-0.75f, 0.95f, 0), new Vector3(0.1f, 1.9f, 0.1f), dark);
        k.Box(new Vector3(0.75f, 0.95f, 0), new Vector3(0.1f, 1.9f, 0.1f), dark);
        k.Box(new Vector3(0, 1.45f, 0.02f), new Vector3(1.55f, 0.9f, 0.06f), wood);
        k.Box(new Vector3(0, 1.98f, 0.0f), new Vector3(1.75f, 0.08f, 0.3f), dark);
        var mi = new MeshInstance3D { Mesh = k.ToMesh(Models.MaterialFor(MatKind.Static)), Name = "BoardMesh" };
        AddChild(mi);
        _papers = new Node3D { Name = "Papers" };
        AddChild(_papers);
        Position = new Vector3(_pos.X, hf.Height(_pos.X, _pos.Y), _pos.Y);
        Rotation = new Vector3(0, InnSite.BoardYaw, 0);
    }

    /// <summary>One pinned sheet per open quest of the inn (and the camp's, taken or not, until it is done).</summary>
    public void Refresh()
    {
        foreach (var c in _papers.GetChildren()) c.QueueFree();
        int n = Quests().Count(q => q.Done == null);
        var paper = MeshKit.C(0xe8dcc0);
        for (int i = 0; i < Math.Min(n, 6); i++)
        {
            var k = new MeshKit();
            float x = -0.5f + (i % 3) * 0.5f, y = 1.62f - (i / 3) * 0.38f;
            k.Box(new Vector3(x, y, 0.06f), new Vector3(0.3f, 0.32f, 0.01f), paper);
            _papers.AddChild(new MeshInstance3D { Mesh = k.ToMesh(Models.MaterialFor(MatKind.Static)) });
        }
    }

    public System.Collections.Generic.IEnumerable<M.Quest> Quests()
    {
        var link = _s.Link; var cp = _s.Camp;
        int n = 0;
        foreach (var q in _s.Macro.W.Quests.AsEnumerable().Reverse())
        {
            bool camp = cp != null && q.Camp == cp.Id;
            bool inn = link != null && link.Inn >= 0 && q.Inn == link.Inn;
            if (!(camp || inn)) continue;
            if (q.Done != null && (!camp || _s.Macro.W.Day - q.Done.Value > 20)) continue;
            bool onWay = !q.Open && q.Done == null && q.TakenBy.Count > 0 && (q.TakenBy.Contains(link?.Player ?? -1) || _s.Macro.W.Agents.Any(a => a.Quest == q.Id && !M.J.T(a.Dead)));
            if (!q.Open && q.Done == null && !onWay) continue;
            if (!camp && n >= 6) continue;
            if (!camp) n++;
            yield return q;
        }
    }

    float _t;
    public override void _Process(double delta) { _t -= (float)delta; if (_t <= 0) { _t = 3f; Refresh(); } }

    (string, Action)? Prompt()
    {
        var p = new Vector2(_r.Player.GlobalPosition.X, _r.Player.GlobalPosition.Z);
        if (p.DistanceTo(_pos) > 2.6f) return null;
        return ("İlan panosu", () => _panel.Open());
    }

    public M.Quest Take()
    {
        var q = M.Local.TakeQuest(_s.Macro);
        Refresh();
        if (q != null) SaveGame.Save(_s, "ilan");
        return q;
    }
}

/// <summary>The board's page: the camp's quest and the inn's others, as the sim has them today.</summary>
public partial class BoardPanel : PanelLayer
{
    Board _b;
    VBoxContainer _list;
    Label _msg;

    public void Init(Region r, Board b)
    {
        _b = b;
        Build(r, "BoardPanel", new Vector2(760, 520));
        Body.AddChild(L($"{r.Session.Names().inn} Hanı — ilan panosu", 26, Ui.Gold));
        var sc = new ScrollContainer { CustomMinimumSize = new Vector2(720, 380), HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled };
        Body.AddChild(sc);
        _list = new VBoxContainer { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
        _list.AddThemeConstantOverride("separation", 10);
        sc.AddChild(_list);
        _msg = L("", 15, new Color(1f, 0.9f, 0.6f)); Body.AddChild(_msg);
        Body.AddChild(Ui.Button("Kapat  [Esc]", Close, 16));
    }

    public void Open() { _msg.Text = ""; Show(); }

    protected override void Refresh()
    {
        Clear(_list);
        var s = R.Session; var cp = s.Camp; var w = s.Macro.W;
        int pid = s.Link?.Player ?? -1;
        bool any = false;
        foreach (var q in _b.Quests().OrderBy(q => cp != null && q.Camp == cp.Id ? 0 : 1))
        {
            any = true;
            var camp = M.J.Find(w.Camps, c => c.Id == q.Camp);
            bool ours = cp != null && q.Camp == cp.Id;
            string poster = q.Civ >= 0 ? $"{w.Civs[q.Civ].Name}" : q.Org != null ? "Avcılar Locası" : "Hancı";
            var box = new VBoxContainer();
            box.AddChild(L($"\"{camp?.Name ?? "Bir in"} temizlensin!\"  —  {q.Bounty:F0} altın", 20, ours ? Ui.Gold : Ui.Text));
            string state;
            if (q.Done != null) state = $"Tamamlandı ({w.Day - q.Done.Value:F0} gün önce).";
            else if (q.Open) state = $"Açık. Asan: {poster}; {(q.Expires != null ? $"{q.Expires.Value - w.Day:F0} gün daha asılı kalacak" : "süresiz")}{(q.Failures > 0 ? $"; {q.Failures:F0} kez başarısız olunmuş, ödül arttı" : "")}.";
            else if (q.TakenBy.Contains(pid)) state = "Sen aldın. Kamp düşünce ödülü hancıdan (ya da muhtardan) al.";
            else
            {
                var hs = q.TakenBy.Select(id => s.Macro.Hero(id)).Where(h => h != null).Select(h => $"{h.Name} ({Rules.ClassName(CharacterFactory.LocalClass(h.Cls))} Sv{h.Level})");
                state = $"{string.Join(", ", hs)} kopardı; yolda.";
            }
            box.AddChild(L(state, 15, Ui.Dim));
            if (!ours && camp != null && cp != null) box.AddChild(L($"Uzak: buradan {s.Macro.G.Dist(camp.Tile, cp.Tile):F0} fersah ötede.", 14, Ui.Faint));
            if (ours && q.Open && q.Done == null)
            {
                var b = Ui.Button("İlanı kopar (al)", () => { var t = _b.Take(); _msg.Text = t != null ? $"İlan senin: {t.Bounty:F0} altın. Kamp düşünce ödülü al." : "Alınamadı."; Refresh(); }, 16);
                box.AddChild(b);
            }
            _list.AddChild(box);
            _list.AddChild(new HSeparator());
        }
        if (s.Link?.Reward > 0) _list.AddChild(L($"Hancıda seni bekleyen ödül: {s.Link.Reward:F0} altın.", 17, Ui.Good));
        if (!any) _list.AddChild(L(cp != null && !cp.Alive ? $"{cp.Name} düştü; panoda yeni ilan yok." : "Panoda ilan yok. Muhtar yakında asar.", 16, Ui.Faint));
    }
}
