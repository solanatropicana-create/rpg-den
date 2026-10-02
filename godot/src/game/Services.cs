using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using FD.Rpg;
using FD.Sim.Life;
using FD.UI;
using FD.World;
using M = FD.Macro;

namespace FD.Game;

/// <summary>
/// Faz 2 F/G: what the village's keepers do for the party (verbs on their cards). Hancı: trade (rations, bandages, potions; buys herbs
/// and trinkets), a room for the night (5 silver a head: sleep until morning — the night is skipped, everyone rests), the quest reward
/// once the camp has fallen. Demirci: weapons and armour. Rahip (şifacı): herbs bought, potions; tending wounds for a small offering;
/// curing a lasting wound (expensive). Muhtar pays the reward where there is no inn. Key I opens the packs.
/// </summary>
public partial class Services : Node
{
    public const int RoomSilver = 5, TendSilver = 5, BedFrom = 17;
    Region _r;
    Session _s;

    public void Init(Region r)
    {
        _r = r; _s = r.Session;
        Name = "Services";
        r.Hud.Verbs.Add(Verbs);
    }

    public override void _UnhandledInput(InputEvent e)
    {
        if (e is InputEventKey k && k.Pressed && !k.Echo && k.PhysicalKeycode == Key.I && _r.Combat?.Active != true && PanelLayer.OpenPanel == null)
        {
            _r.Inventory.Toggle();
            GetViewport().SetInputAsHandled();
        }
    }

    Character Me => _r.Player.Character;

    IEnumerable<(string, Action)> Verbs(Person p)
    {
        if (!p.Visible || p.Dead) yield break;
        switch (p.Role)
        {
            case Role.Innkeeper:
                yield return ("Ticaret — erzak, sargı, iksir; ot ve ıvır zıvır alır", () => _r.Trade.Open(Economy.Open(_s, ShopKind.Inn, p.FullName)));
                if (_s.Link?.Reward > 0 && _s.Link.Inn >= 0) yield return ($"Ödülü al — {_s.Link.Reward:F0} altın", Reward);
                int heads = _s.Party.Count(c => !c.Dead && !c.Captive);
                if (GameClock.Hour >= BedFrom || GameClock.Hour < 5)
                    yield return ($"Oda tut ve sabaha dek uyu — {Rules.Money(RoomSilver * heads)} ({heads} kişi)", () => Sleep());
                else yield return ($"Oda tut — akşam {BedFrom}:00'den sonra", null);
                break;
            case Role.Smith:
            case Role.Apprentice:
                yield return ("Ticaret — silah ve zırh", () => _r.Trade.Open(Economy.Open(_s, ShopKind.Smith, p.FullName)));
                break;
            case Role.Priest:
                yield return ("Ticaret — şifalı ot alır, iksir satar", () => _r.Trade.Open(Economy.Open(_s, ShopKind.Priest, p.FullName)));
                if (_s.Party.Any(c => !c.Dead && c.Hp < c.MaxHp)) yield return ($"Yaraları sardır — bağış {Rules.Money(TendSilver)}", Tend);
                var wounded = _s.Party.FirstOrDefault(c => !c.Dead && c.Wounds.Count > 0);
                if (wounded != null)
                {
                    var w = wounded.Wounds[0];
                    yield return ($"{wounded.Name}: {Wound.Name(w.Kind).ToLowerInvariant()} tedavisi — {Rules.Money(Wound.CureSilver)}", () => Cure(wounded, w.Kind));
                }
                break;
            case Role.Headman:
                if (_s.Link?.Reward > 0 && _s.Link.Inn < 0) yield return ($"Ödülü al — {_s.Link.Reward:F0} altın", Reward);
                break;
        }
    }

    public void Reward()
    {
        double gold = M.Local.CollectReward(_s.Macro);
        int silver = (int)Math.Round(gold * Rules.SilverPerGold);
        _s.Player.Inv.Silver += silver;
        _r.Hud.Toast($"Ödül: {gold:F0} altın ({Rules.Money(silver)}) keseye girdi. \"Köy sana minnettar.\"", 6f);
        SaveGame.Save(_s, "ödül");
    }

    /// <summary>A room at the inn: everyone pays a bed and sleeps until seven; the night passes, hit points and spells come back, the party
    /// talks things over (alignment).</summary>
    public bool Sleep()
    {
        var members = _s.Party.Where(c => !c.Dead && !c.Captive).ToList();
        int cost = RoomSilver * members.Count;
        int purse = members.Sum(c => c.Inv.Silver);
        if (purse < cost) { _r.Hud.Toast($"Oda {Rules.Money(cost)}; kesede {Rules.Money(purse)} var.", 4f); return false; }
        foreach (var c in members.OrderBy(c => c == _s.Player ? 0 : 1)) { int t = Math.Min(c.Inv.Silver, cost); c.Inv.Silver -= t; cost -= t; if (cost <= 0) break; }
        double now = GameClock.TotalHours;
        double wake = Math.Floor(now / 24.0) * 24.0 + 7.0;
        if (wake <= now + 1) wake += 24.0;
        GameClock.SetTotalHours(wake);
        foreach (var c in members) c.Rest();
        _r.Party.OnRest();
        _r.Hud.CloseCardPublic();
        _r.Hud.Toast($"Gece geçti. {GameClock.TimeString} — herkes dinç uyandı.", 5f);
        SaveGame.Save(_s, "uyku");
        return true;
    }

    public void Tend()
    {
        if (!Pay(TendSilver)) return;
        foreach (var c in _s.Party.Where(c => !c.Dead)) c.Hp = c.MaxHp;
        _r.Hud.Toast("Rahip yaraları temizleyip sardı; herkes kendine geldi. \"Işık yolunu aydınlatsın.\"", 5f);
    }

    public void Cure(Character c, string kind)
    {
        if (!Pay(Wound.CureSilver)) return;
        c.CureWound(kind);
        if (c.HeroId is int hid && _s.Macro.Hero(hid) is M.Hero h) { h.Epithet = c.Epithet; M.Will.Note(_s.Macro, h, $"tapınakta iyileşti: {Wound.Name(kind).ToLowerInvariant()}"); }
        _r.Hud.Toast($"Rahip dua etti, merhem sürdü: {c.Name}'ın {Wound.Name(kind).ToLowerInvariant()} geçti.", 6f);
        if (c == _r.Player.Character) _r.Player.SetCharacter(c);
    }

    bool Pay(int silver)
    {
        int purse = _s.Party.Where(c => !c.Dead).Sum(c => c.Inv.Silver);
        if (purse < silver) { _r.Hud.Toast($"{Rules.Money(silver)} gerek; kesede {Rules.Money(purse)} var.", 4f); return false; }
        foreach (var c in _s.Party.Where(c => !c.Dead).OrderBy(c => c == _s.Player ? 0 : 1)) { int t = Math.Min(c.Inv.Silver, silver); c.Inv.Silver -= t; silver -= t; if (silver <= 0) break; }
        return true;
    }
}
