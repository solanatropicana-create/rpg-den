using System;
using System.Collections.Generic;
using System.Linq;
using FD.Rpg;
using M = FD.Macro;

namespace FD.Game;

public enum ShopKind { Inn, Smith, Priest }

/// <summary>One line of a shop: what it sells today (stock from the sim), at what price, and what it pays for yours.</summary>
public sealed class Offer
{
    public string Id;
    public int Stock;
    public int Buy;
    public ItemDef Def => Items.Get(Id);
}

public sealed class Shop
{
    public ShopKind Kind;
    public string Name, Keeper;
    public readonly List<Offer> Offers = new();
    /// <summary>what this shop buys and at what share of the price (1 = full: trade goods; 0.5: used gear)</summary>
    public Func<ItemDef, double> Buys = _ => 0;
}

/// <summary>
/// Faz 2 F: prices and stock from the sim. An item's price is its PHB price (silver) times the village realm's current price ratio
/// for the linked good (<c>Local.Price / base</c>: a war makes arms dear in the smithy). What a shop has today follows the realm's
/// stockpile (no arms in the realm, no swords at the smith). Buying takes the good out of the realm's stock and puts the money in its
/// treasury; selling the other way (<c>Local.Trade</c>). Shops buy trade goods (herbs, trinkets) at full price, used gear at half.
/// </summary>
public static class Economy
{
    public static double Ratio(Session s, string good)
    {
        var g = M.D.GOODS.GetOr(good, null);
        if (g == null) return 1;
        return Math.Clamp(M.Local.Price(s.Macro, good) / g.Base, 0.4, 3.0);
    }

    public static int PriceOf(Session s, string id)
    {
        var d = Items.Get(id);
        if (d == null) return 0;
        return Math.Max(1, (int)Math.Round(d.Price * Ratio(s, d.Good)));
    }

    public static int SellPrice(Session s, Shop shop, string id)
    {
        var d = Items.Get(id);
        double share = d != null ? shop.Buys(d) : 0;
        return share <= 0 ? 0 : Math.Max(1, (int)Math.Floor(PriceOf(s, id) * share));
    }

    static readonly Dictionary<string, int> _sold = new();
    static string Key(Session s, ShopKind k, string id) => $"{k}:{id}:{s.Macro.W.Day}";

    static int Stock(Session s, string good, double per, int cap, int min = 0)
    {
        double st = M.Local.Stock(s.Macro, good);
        return Math.Clamp((int)Math.Floor(st / per), min, cap);
    }

    /// <summary>The shop as it is today.</summary>
    public static Shop Open(Session s, ShopKind k, string keeper)
    {
        var shop = new Shop { Kind = k, Keeper = keeper };
        void O(string id, int stock)
        {
            int left = Math.Max(0, stock - _sold.GetValueOrDefault(Key(s, k, id)));
            shop.Offers.Add(new Offer { Id = id, Stock = left, Buy = PriceOf(s, id) });
        }
        switch (k)
        {
            case ShopKind.Inn:
                shop.Name = $"{s.Names().inn} Hanı";
                O("ration", Stock(s, "bread", 4, 20, 4));
                O("bandage", Stock(s, "herbs", 2, 10, 2));
                O("potion", Stock(s, "potion", 3, 4));
                shop.Buys = d => d.Kind is ItemKind.Herb or ItemKind.Loot ? 1.0 : d.Kind is ItemKind.Ration or ItemKind.Potion or ItemKind.Bandage ? 0.5 : 0;
                break;
            case ShopKind.Smith:
                shop.Name = "Demirci";
                // simple weapons the smith forges himself (iron or tools in the realm: up to two each, at least one); martial weapons and
                // mail from the realm's armoury (arms)
                int arms = Stock(s, "arms", 6, 3);
                int simple = Math.Max(Stock(s, "iron", 8, 2, 1), Stock(s, "tools", 8, 2, 1));
                O("club", 3); O("staff", 2);
                foreach (var id in new[] { "dagger", "spear", "handaxe", "mace" }) O(id, simple);
                foreach (var id in new[] { "shortsword", "scimitar", "longsword", "shortbow" }) O(id, Math.Min(arms, id is "longsword" or "shortbow" ? 1 : 2));
                O("leather", Stock(s, "leather", 6, 2));
                O("shield", Math.Min(arms, 2));
                O("chainshirt", Math.Min(arms, 1));
                O("chainmail", arms >= 3 ? 1 : 0);
                shop.Buys = d => d.Kind is ItemKind.Weapon or ItemKind.Armor or ItemKind.Shield ? 0.5 : 0;
                break;
            case ShopKind.Priest:
                shop.Name = "Tapınak";
                O("potion", Stock(s, "potion", 5, 2));
                O("bandage", Stock(s, "herbs", 3, 6, 1));
                shop.Buys = d => d.Kind == ItemKind.Herb ? 1.0 : d.Kind == ItemKind.Potion ? 0.5 : 0;
                break;
        }
        return shop;
    }

    /// <summary>Buy one; false with a reason when it cannot be done.</summary>
    public static bool Buy(Session s, Shop shop, string id, Character who, out string why)
    {
        why = null;
        var o = shop.Offers.FirstOrDefault(x => x.Id == id);
        if (o == null || o.Stock <= 0) { why = "Kalmadı."; return false; }
        if (who.Inv.Silver < o.Buy) { why = $"Kesende {Rules.Money(who.Inv.Silver)} var; {Rules.Money(o.Buy)} gerek."; return false; }
        who.Inv.Silver -= o.Buy;
        who.Inv.Add(id);
        o.Stock--;
        _sold[Key(s, shop.Kind, id)] = _sold.GetValueOrDefault(Key(s, shop.Kind, id)) + 1;
        WriteMacro(s, id, o.Buy, +1);
        return true;
    }

    public static bool Sell(Session s, Shop shop, string id, Character who, out string why)
    {
        why = null;
        int p = SellPrice(s, shop, id);
        if (p <= 0) { why = "Bunu almıyorlar."; return false; }
        if (!who.Inv.Has(id)) { why = "Elinde yok."; return false; }
        if (who.Weapon == id && who.Inv.Count(id) == 1) who.Weapon = null;
        if (who.Armor == id && who.Inv.Count(id) == 1) who.Armor = null;
        if (who.Shield == id && who.Inv.Count(id) == 1) who.Shield = null;
        who.Inv.Remove(id);
        who.Inv.Silver += p;
        var o = shop.Offers.FirstOrDefault(x => x.Id == id);
        if (o != null) o.Stock++;
        WriteMacro(s, id, p, -1);
        return true;
    }

    /// <summary>The trade in the sim: the good's units worth the money (silver/10 gold ÷ base price).</summary>
    static void WriteMacro(Session s, string id, int silver, int dir)
    {
        var d = Items.Get(id);
        var g = d != null ? M.D.GOODS.GetOr(d.Good, null) : null;
        if (g == null) return;
        double gold = silver / (double)Rules.SilverPerGold;
        M.Local.Trade(s.Macro, d.Good, dir * gold / g.Base, gold);
    }

    // ------------------------------------------------------------------------------------------------ loot
    /// <summary>What a dead goblin carries: its weapon (the archer's bow, a scimitar, a club or a spear), trinkets, a few silver; the
    /// chief more. Deterministic for the goblin.</summary>
    public static Inventory GoblinLoot(int seed, bool boss, bool archer, bool spear)
    {
        var r = new M.Rng(seed * 7919 + 31);
        var inv = new Inventory();
        if (archer) { if (r.Chance(0.6)) inv.Add("shortbow"); }
        else if (boss || r.Chance(0.35)) inv.Add("scimitar");
        else inv.Add(spear ? "spear" : "club");
        int t = (int)r.Int(0, boss ? 3 : 2);
        if (t > 0) inv.Add("trinket", t);
        inv.Silver = (int)(boss ? r.Int(8, 30) : r.Int(0, 6));
        if (boss && r.Chance(0.4)) inv.Add("potion");
        if (!boss && r.Chance(0.15)) inv.Add("herb");
        return inv;
    }
}
