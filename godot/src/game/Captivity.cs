using System;
using System.Linq;
using Godot;
using FD.Actors;
using FD.Core;
using FD.Rpg;
using FD.World;
using M = FD.Macro;

namespace FD.Game;

/// <summary>
/// Faz 2 D: the goblin camp's cage. A caged player stays behind the bars (kept inside, the cage's solid box ignored) and can try
/// the door once a game hour: d20 + the better of Strength (wrench it) and Dexterity (work the latch; a rogue adds proficiency)
/// against DC 15. A free party member at the door opens it for a caged companion (the latch is outside). Offered through the
/// HUD's E prompt.
/// </summary>
public partial class Captivity : Node
{
    public const int DC = 15;
    Region _r;
    Session _s;
    uint _savedMask;
    bool _clamped;

    public void Init(Region r)
    {
        _r = r; _s = r.Session;
        Name = "Captivity";
        r.Hud.Prompts.Add(Prompt);
    }

    bool PlayerCaptive => _s.Player?.Captive == true;
    double NextTry
    {
        get => _s.Flags.TryGetValue("cageNextTry", out var v) ? v : 0;
        set => _s.Flags["cageNextTry"] = value;
    }

    Companion CaptiveCompanion() => _r.Companions.FirstOrDefault(c => c.Char.Captive && !c.Char.Dead);

    (string text, Action act)? Prompt()
    {
        var pl = _r.Player;
        var p = new Vector2(pl.GlobalPosition.X, pl.GlobalPosition.Z);
        if (PlayerCaptive)
        {
            if (p.DistanceTo(CampSite.CagePrisoner) > 2.5f) return null;
            if (GameClock.TotalHours < NextTry)
                return ($"Kafesin kapısı — ellerin hâlâ titriyor ({Clock(NextTry)}'te yeniden dene)", () => _r.Hud.Toast("Biraz soluklan; bir saat sonra yeniden dene."));
            var (what, bonus) = Best(_s.Player);
            return ($"Kafesin kapısını zorla ({what} {Rules.Signed(bonus)} vs ZD {DC})", TryEscape);
        }
        var cap = CaptiveCompanion();
        if (cap != null && p.DistanceTo(CampSite.CageDoor) < 2.8f) return ($"Kafesi aç: {cap.Char.Name} kurtulsun", () => Free(cap));
        return null;
    }

    static string Clock(double totalHours) { double h = totalHours % 24; return $"{(int)h:00}:{(int)(h % 1 * 60):00}"; }

    static (string what, int bonus) Best(Character c)
    {
        int str = c.Mod(Rules.STR), dex = c.Mod(Rules.DEX) + (c.Cls == "rogue" ? c.Prof : 0);
        return dex >= str ? ("Çeviklik", dex) : ("Güç", str);
    }

    void TryEscape()
    {
        var c = _s.Player;
        var (what, bonus) = Best(c);
        var rng = new M.Rng(_s.Seed * 31 + Math.Floor(GameClock.TotalHours * 60));
        int d = (int)rng.D20();
        int total = d + bonus;
        if (total >= DC || d == 20)
        {
            c.Captive = false;
            Release();
            _r.Hud.Toast($"d20 {d} {Rules.Signed(bonus)} = {total} vs ZD {DC} — {(what == "Güç" ? "parmaklıkları büküp" : "mandalı kurcalayıp")} kapıyı açtın! Sessiz ol...", 6f);
            if (c.HeroId is int hid && _s.Macro.Hero(hid) is M.Hero h) M.Will.Note(_s.Macro, h, "goblin kafesinden kaçtı");
            foreach (var comp in _r.Companions) comp.Hold = false;
        }
        else
        {
            NextTry = GameClock.TotalHours + 1.0;
            _r.Hud.Toast($"d20 {d} {Rules.Signed(bonus)} = {total} vs ZD {DC} — kapı yerinden oynamadı. Bir saat sonra yeniden dene.", 5f);
        }
    }

    void Free(Companion comp)
    {
        comp.Char.Captive = false;
        comp.SnapToLeader();
        _r.Hud.Toast($"{comp.Char.Name} kafesten çıktı: \"Seni gördüğüme hiç bu kadar sevinmemiştim!\"", 5f);
    }

    /// <summary>Out of the cage: the bars are solid again, the player steps out at the door.</summary>
    void Release()
    {
        var pl = _r.Player;
        if (_clamped) { pl.CollisionMask = _savedMask; _clamped = false; }
        pl.Teleport(CampSite.CageDoor, CampSite.CageYaw, _r.Heightfield);
    }

    /// <summary>Put the player in the cage (after a lost fight).</summary>
    public void Cage()
    {
        var pl = _r.Player;
        if (!_clamped) { _savedMask = pl.CollisionMask; pl.CollisionMask = App.LayerTerrain; _clamped = true; }
        pl.Teleport(CampSite.CagePrisoner, CampSite.CageYaw + MathF.PI, _r.Heightfield);
        NextTry = GameClock.TotalHours + 0.5;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!PlayerCaptive) { if (_clamped) { _r.Player.CollisionMask = _savedMask; _clamped = false; } return; }
        var pl = _r.Player;
        if (!_clamped) { _savedMask = pl.CollisionMask; pl.CollisionMask = App.LayerTerrain; _clamped = true; }
        var p = new Vector2(pl.GlobalPosition.X, pl.GlobalPosition.Z);
        var d = p - CampSite.CagePrisoner;
        const float R = 0.5f;
        if (d.Length() > R)
        {
            var q = CampSite.CagePrisoner + d.Normalized() * R;
            pl.GlobalPosition = new Vector3(q.X, _r.Heightfield.Height(q.X, q.Y) + 0.02f, q.Y);
        }
    }
}
