using System;
using System.Linq;
using Godot;
using FD.Game;
using FD.Rpg;
using FD.Sim.Life;
using FD.World;
using V2 = System.Numerics.Vector2;
using M = FD.Macro;

namespace FD.Dev;

/// <summary>
/// Faz 2 F (--econtest): shops from the sim (stock follows the realm's stores, prices its market), buying and selling move the realm's
/// stock and treasury, herbs grow back, weight slows, the dead are searched, the camp chest, a night at the inn, the reward, the temple's
/// cure. PASS/FAIL, exit code 0/1.
/// </summary>
public static class EconTest
{
    public static void Run(Node host, Region r)
    {
        bool ok = true;
        void Check(string name, bool pass, string info) { ok &= pass; GD.Print($"[EconTest] {name}: {info} {(pass ? "PASS" : "FAIL")}"); }
        var s = r.Session; var me = s.Player; var civ = s.Civ;
        me.Inv.Silver = 200;
        // shops
        var inn = Economy.Open(s, ShopKind.Inn, "hancı");
        var smith = Economy.Open(s, ShopKind.Smith, "demirci");
        GD.Print($"[EconTest] {civ?.Name}: ekmek {M.Local.Stock(s.Macro, "bread"):F0} (fiyat ×{Economy.Ratio(s, "bread"):F2}), silah {M.Local.Stock(s.Macro, "arms"):F0} (×{Economy.Ratio(s, "arms"):F2}), iksir {M.Local.Stock(s.Macro, "potion"):F0}, ot {M.Local.Stock(s.Macro, "herbs"):F0}");
        Check("han", inn.Offers.First(o => o.Id == "ration").Stock > 0, string.Join(", ", inn.Offers.Select(o => $"{Items.Get(o.Id).Name} {o.Stock}×{o.Buy}")));
        Check("demirci", smith.Offers.Count > 5, string.Join(", ", smith.Offers.Select(o => $"{Items.Get(o.Id).Name} {o.Stock}×{o.Buy}")));
        // buy a ration
        double bread0 = M.Local.Stock(s.Macro, "bread"), gold0 = s.Macro.St(civ, "gold");
        int price = inn.Offers.First(o => o.Id == "ration").Buy, sil0 = me.Inv.Silver;
        bool bought = Economy.Buy(s, inn, "ration", me, out _);
        Check("alış simde", bought && me.Inv.Silver == sil0 - price && me.Inv.Has("ration") && M.Local.Stock(s.Macro, "bread") < bread0 && s.Macro.St(civ, "gold") > gold0,
            $"erzak {price} gümüş; ekmek stoğu {bread0:F2} → {M.Local.Stock(s.Macro, "bread"):F2}, hazine {gold0:F1} → {s.Macro.St(civ, "gold"):F1} altın");
        // herbs
        var g = r.Gathering;
        Check("şifalı ot yerleri", g.HerbPositions.Count == Gathering.Patches, $"{g.HerbPositions.Count} öbek");
        var hp = g.HerbPositions[0];
        r.Player.Teleport(new Vector2(hp.X + 0.8f, hp.Y), 0, r.Heightfield);
        var pr = r.Hud.Prompts.Select(f => f()).FirstOrDefault(x => x != null);
        int herbs0 = me.Inv.Count("herb");
        pr?.act();
        int got = me.Inv.Count("herb") - herbs0;
        var pr2 = r.Hud.Prompts.Select(f => f()).FirstOrDefault(x => x != null);
        Check("ot topla", pr?.text.Contains("ot") == true && got >= 1 && (pr2 == null || !pr2.Value.text.Contains("ot topla")), $"\"{pr?.text}\" → +{got} ot; öbek {Gathering.RegrowDays} gün sonra yeniden biter");
        // sell herbs to the inn
        double herbs1 = M.Local.Stock(s.Macro, "herbs"); int sil1 = me.Inv.Silver;
        int sp = Economy.SellPrice(s, inn, "herb");
        bool sold = Economy.Sell(s, inn, "herb", me, out _);
        Check("satış simde", sold && me.Inv.Silver == sil1 + sp && M.Local.Stock(s.Macro, "herbs") > herbs1, $"ot hana {sp} gümüş; ot stoğu {herbs1:F2} → {M.Local.Stock(s.Macro, "herbs"):F2}");
        // weight
        float f0 = me.SpeedFactor;
        int n = 0;
        while (!me.Overloaded && n < 20) { me.Inv.Add("chainmail"); n++; }
        float f1 = me.SpeedFactor;
        Check("ağırlık yavaşlatır", me.Overloaded && f1 < f0 * 0.65f, $"{me.Inv.Weight:F0}/{me.Capacity:F0} kg: hız ×{f0:F2} → ×{f1:F2}");
        me.Inv.Remove("chainmail", n);
        // the dead
        var gob = r.Life.People.First(p => p.Role == Role.Goblin && !p.IsBoss);
        gob.Dead = true;
        var loot = g.Loot(gob);
        Check("ceset aranır", loot.Items.Count > 0 && me.Inv.Items.Any(i => loot.Items.Any(l => l.Id == i.Id)), $"{gob.Name}: {string.Join(", ", loot.Items.Select(i => $"{i.Count} {i.Id}"))}, {loot.Silver} gümüş");
        // camp chest after the camp fell
        s.CampChest.Add("potion"); s.CampChest.Silver += 20;
        var cp = s.Camp; cp.Alive = false; s.Link.CampLoot = 12;
        r.Player.Teleport(CampSite.ChestPos, 0, r.Heightfield);
        int sil2 = me.Inv.Silver;
        var chest = g.OpenChest();
        Check("kamp sandığı", chest != null && me.Inv.Silver == sil2 + 20 + 120 && s.CampChest.Items.Count == 0 && s.Link.CampLoot == 0, $"sandıktan {string.Join(", ", chest?.Items.Select(i => i.Id) ?? Array.Empty<string>())} + {chest?.Silver} gümüş");
        cp.Alive = true;
        // a night at the inn
        GameClock.SetHour(20f);
        me.Hp = 1;
        int sil3 = me.Inv.Silver; int day0 = GameClock.Day;
        bool slept = r.Services.Sleep();
        Check("handa uyku", slept && GameClock.Day == day0 + 1 && Math.Abs(GameClock.Hour - 7f) < 0.01f && me.Hp == me.MaxHp && me.Inv.Silver == sil3 - Services.RoomSilver * s.Party.Count(c => !c.Dead),
            $"{GameClock.TimeString}, can {me.Hp}/{me.MaxHp}, kese {sil3} → {me.Inv.Silver}");
        // the reward
        s.Link.Reward = 55;
        int sil4 = me.Inv.Silver;
        r.Services.Reward();
        Check("ödül", me.Inv.Silver == sil4 + 550 && s.Link.Reward == 0 && s.Macro.W.Events.Any(e => e.Text.Contains("ödülünü aldı")), $"+550 gümüş");
        // temple cure
        me.AddWound("scar", 1, "deneme"); int cha = me.Stats[Rules.CHA];
        me.Inv.Silver += 400;
        r.Services.Cure(me, "scar");
        Check("tapınakta tedavi", me.Wounds.Count == 0 && me.Stats[Rules.CHA] == cha + 1 && me.Epithet == null, $"iz geçti, Karizma {cha} → {me.Stats[Rules.CHA]}");
        GD.Print(ok ? "ECONTEST PASS" : "ECONTEST FAIL");
        host.GetTree().Quit(ok ? 0 : 1);
    }
}
