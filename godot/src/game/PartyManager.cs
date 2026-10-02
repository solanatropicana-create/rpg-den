using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using FD.Actors;
using FD.Life;
using FD.Rpg;
using FD.Sim.Life;
using FD.World;
using M = FD.Macro;

namespace FD.Game;

/// <summary>
/// Faz 2 E: the party outside fights. Hiring at the inn (an adventurer's card: "Kirala" — a free hero of the macro world keeps
/// their record and goes "party" there; a mercenary has none), weekly wages (5 days, paid from the party's purses on payday, the
/// first one a week after hiring; unpaid → they leave), alignment (a companion whose alignment is the opposite of the leader's —
/// good and evil — leaves when the party makes camp / sleeps), and Tab: the controlled body changes (the party follows whoever
/// is controlled).
/// </summary>
public partial class PartyManager : Node
{
    public const int MaxCompanions = 2, WeekDays = 5;
    Region _r;
    Session _s;

    public void Init(Region r)
    {
        _r = r; _s = r.Session;
        Name = "PartyManager";
        r.Director.NewDay += OnNewDay;
        r.Hud.Verbs.Add(HireVerbs);
    }

    public override void _ExitTree() { if (_r?.Director != null) _r.Director.NewDay -= OnNewDay; }

    public IEnumerable<Character> Companions => _s.Party.Where(c => c != _s.Player && !c.Dead);

    public static bool ForHire(Person p) => p.Guest && CharacterFactory.LocalClass(p.Cls) is "fighter" or "rogue";
    public static int WageFor(Person p) => p.HeroId >= 0 ? 25 * Math.Max(1, p.Level) + 25 : 40;
    static bool Opposed(string a, string b) => (a == "good" && b == "evil") || (a == "evil" && b == "good");

    // ------------------------------------------------------------------------------------------------ hiring
    IEnumerable<(string label, Action act)> HireVerbs(Person p)
    {
        if (!p.Guest || p.Hired || !p.Visible) yield break;
        if (!ForHire(p)) { yield return ("Kirala", () => _r.LifeWorld.ActorOf(p)?.Say("Kiralık değilim, dostum; kendi yolumdayım.")); yield break; }
        int wage = WageFor(p);
        string al = Rules.AlignName(p.Align ?? "neutral").ToLowerInvariant();
        string warn = Opposed(p.Align, _s.Player.Align) ? $" · {al}: yolları ilk konaklamada ayrılır" : $" · {al}";
        if (Companions.Count() >= MaxCompanions) { yield return ($"Kirala — ekip dolu (en çok {MaxCompanions} yoldaş)", null); yield break; }
        yield return ($"Kirala — haftalığı {Rules.Money(wage)}, ilk ödeme {WeekDays} gün sonra{warn}", () => Hire(p));
    }

    public Character Hire(Person p)
    {
        int wage = WageFor(p);
        Character c;
        if (p.HeroId >= 0 && _s.Macro.Hero(p.HeroId) is M.Hero h) { c = CharacterFactory.FromHero(h); c.Id = _s.NextCharId(); }
        else c = CharacterFactory.Mercenary(p.FullName, p.Race, p.Female, CharacterFactory.LocalClass(p.Cls), p.Align ?? "neutral", _s.NextCharId());
        c.Look = LookOf(p);
        c.WageSilver = wage; c.HiredDay = GameClock.Day; c.PaidUntil = GameClock.Day + WeekDays;
        _s.Party.Add(c);
        if (c.HeroId is int hid) M.Local.Hire(_s.Macro, hid, wage);
        var actor = _r.LifeWorld.ActorOf(p);
        var at = actor != null ? actor.GlobalPosition : _r.Player.GlobalPosition;
        p.Hired = true; p.Present = false; p.Act = null; p.Motion = Motion.Inside; p.Path.Clear();
        p.Note($"{H.Clock(_r.Life.Now)} {_s.Player.Name} ile anlaştı, ekibe katıldı");
        var comp = _r.AddCompanion(c, new Vector2(at.X, at.Z));
        comp.Hold = _r.Player.Character.Captive;
        _r.Hud.Toast($"{c.Name} ekibe katıldı: \"Anlaştık! {(c.Cls == "rogue" ? "Hançerim" : "Kılıcım")} seninle.\" Haftalığı {Rules.Money(wage)}; ilk ödeme {WeekDays} gün sonra.", 6f);
        _r.Hud.CloseCardPublic();
        SaveGame.Save(_s, "kiralama");
        return c;
    }

    /// <summary>The companion's look follows the person at the inn (colours, hair, beard, height).</summary>
    static Look LookOf(Person p)
    {
        var ap = Appearance.For(p);
        int Hex(Color c) => (int)(c.ToRgba32() >> 8);
        var rl = Appearance.Look(p.Race);
        var look = new Look
        {
            Female = p.Female, Skin = Hex(ap.Skin), Hair = Hex(ap.Hair), Cloth1 = Hex(ap.Cloth1), Cloth2 = Hex(ap.Cloth2),
            HairStyle = ap.Outfit.Contains("hair_long") ? "hair_long" : ap.Outfit.Contains("hair_bun") ? "hair_bun" : ap.Outfit.Contains("hair_short") ? "hair_short" : "none",
            Beard = ap.Outfit.Contains("beard"),
            Height = Math.Clamp(ap.Scale / (rl.Height * (p.Female ? 0.95f : 1f)), 0.94f, 1.06f),
        };
        return look;
    }

    /// <summary>A companion leaves (unpaid, alignment): back to the inn (a macro hero waits there again), out of the party.</summary>
    public void Leave(Character c, string why, string line)
    {
        _s.Party.Remove(c);
        var comp = _r.Companions.FirstOrDefault(x => x.Char == c);
        if (comp != null) { _r.Companions.Remove(comp); comp.QueueFree(); }
        if (c.HeroId is int hid) M.Local.Dismiss(_s.Macro, hid, why);
        var p = _r.Life.People.FirstOrDefault(x => x.Guest && x.Hired && (c.HeroId is int id ? x.HeroId == id : x.FullName == c.Name));
        if (p != null) { p.Hired = false; p.Present = true; p.NextDecision = _r.Life.Now; p.Act = null; }
        _r.Hud.Toast($"{c.Name} ekipten ayrıldı ({why}): \"{line}\"", 6f);
        GD.Print($"[Party] {c.Name} ayrıldı: {why}");
    }

    // ------------------------------------------------------------------------------------------------ wages and camp
    void OnNewDay(int macroDay)
    {
        int day = GameClock.Day;
        // the inn's adventurers follow the sim: a hero who left the inn (took a quest, was hired by a realm) is gone from it
        var inn = _s.Inn;
        foreach (var p in _r.Life.People)
        {
            if (!p.Guest || p.Hired || p.HeroId < 0) continue;
            var h = _s.Macro.Hero(p.HeroId);
            bool here = h != null && h.State == "tavern" && h.BaseInn && inn != null && h.Base == inn.Id;
            if (!here && p.Present) { p.Present = false; p.Act = null; p.Motion = Motion.Inside; p.Note($"{H.Clock(_r.Life.Now)} handan ayrıldı{(h?.State == "quest" ? ": bir ilanın peşine düştü" : h?.State == "dead" ? "" : "")}"); }
            else if (here && !p.Present) { p.Present = true; p.NextDecision = _r.Life.Now; }
        }
        foreach (var c in Companions.ToList())
        {
            if (c.WageSilver <= 0 || c.Captive) continue;
            if (day >= c.PaidUntil) Payday(c);
            else if (day == c.PaidUntil - 1) _r.Hud.Toast($"Yarın {c.Name} haftalığını bekliyor: {Rules.Money(c.WageSilver)}.", 5f);
        }
    }

    /// <summary>Pay a week from the party's purses (the leader's first); not enough → they leave.</summary>
    public bool Payday(Character c)
    {
        int need = c.WageSilver;
        int have = _s.Party.Where(x => !x.Dead).Sum(x => x.Inv.Silver);
        if (have < need)
        {
            Leave(c, "haftalığı ödenmedi", "Parasız yol yürünmez. Hoşça kal.");
            return false;
        }
        foreach (var x in _s.Party.Where(x => !x.Dead).OrderBy(x => x == _s.Player ? 0 : x == c ? 2 : 1))
        {
            int take = Math.Min(x.Inv.Silver, need);
            x.Inv.Silver -= take; need -= take;
            if (need <= 0) break;
        }
        c.PaidUntil += WeekDays;
        _r.Hud.Toast($"{c.Name} haftalığını aldı: {Rules.Money(c.WageSilver)}.", 4f);
        return true;
    }

    /// <summary>The party makes camp or sleeps (H: a night at the inn): a companion whose alignment opposes the leader's leaves.</summary>
    public void OnRest()
    {
        foreach (var c in Companions.ToList())
            if (Opposed(c.Align, _s.Player.Align))
                Leave(c, "yolları ayrıldı", c.Align == "good" ? "Senin yaptıklarına ortak olamam." : "Senin gibi bir azizle yol yürünmez.");
    }

    // ------------------------------------------------------------------------------------------------ Tab: who is controlled
    public override void _UnhandledInput(InputEvent e)
    {
        if (e is not InputEventKey k || !k.Pressed || k.Echo || k.PhysicalKeycode != Key.Tab) return;
        if (_r.Combat?.Active == true || !_r.Player.InputEnabled) return;
        SwitchControl(k.ShiftPressed ? -1 : 1);
        GetViewport().SetInputAsHandled();
    }

    /// <summary>The player's body takes the next party member (their look, place and facing); that member's companion body takes the
    /// one who was controlled. The party follows whoever is controlled.</summary>
    public void SwitchControl(int dir)
    {
        var cur = _r.Player.Character;
        var list = new List<Character> { cur };
        foreach (var comp in _r.Companions) if (!comp.Char.Dead && !comp.Char.Down) list.Add(comp.Char);
        if (list.Count < 2) return;
        var next = list[((dir % list.Count) + list.Count) % list.Count];
        var body = _r.Companions.FirstOrDefault(x => x.Char == next);
        if (body == null || next == cur) return;
        var pl = _r.Player;
        var pPos = pl.GlobalPosition; float pYaw = pl.Facing;
        var cPos = body.GlobalPosition; float cYaw = body.Rotation.Y;
        pl.SetCharacter(next);
        pl.Teleport(new Vector2(cPos.X, cPos.Z), cYaw, _r.Heightfield);
        body.Become(cur);
        body.GlobalPosition = pPos;
        body.Rotation = new Vector3(0, pYaw, 0);
        _s.Controlled = next;
        foreach (var comp in _r.Companions) comp.Hold = next.Captive;
        _r.Hud.Toast($"Kontrol: {next.FullName}", 2f);
    }
}
