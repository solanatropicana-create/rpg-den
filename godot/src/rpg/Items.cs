using System;
using System.Collections.Generic;

namespace FD.Rpg;

public enum ItemKind { Weapon, Armor, Shield, Potion, Bandage, Ration, Herb, Loot, Misc }

/// <summary>
/// Faz 2 item table (D&amp;D 5e PHB weapons and armour, a few consumables). Prices are in silver at the macro base price of the
/// linked good (<see cref="Good"/>: arms, leather, potion, bread, herbs); the shop multiplies by the village's current price
/// ratio (Local.Price / base), so a war that makes arms dear makes swords dear in the smithy too.
/// </summary>
public sealed class ItemDef
{
    public string Id, Name, Plural;
    public ItemKind Kind;
    public float Weight;          // kg
    public int Price;             // silver (gümüş) at base
    public string Good;           // macro good that sets the price
    // weapons
    public int DiceN, DiceS;      // damage dice
    public string DmgType = "";   // ezici / delici / kesici
    public bool Finesse, Light, TwoHanded, Versatile;
    public float Range;           // m; 0 = melee (reach 1.6 m)
    public string Prop;           // human.glb / goblin.glb tool mesh shown in the hand (tool_axe, tool_spear, tool_hammer, tool_club) or procedural
    // armour
    public int ArmorBase, DexMax = 99; public bool StealthDis; public int StrReq;
    // consumables
    public int HealN, HealS, HealB;
    public string Desc = "";

    public bool IsWeapon => Kind == ItemKind.Weapon;
    public string DamageText => DiceN > 0 ? $"{DiceN}d{DiceS} {DmgType}" : "";
}

public static class Items
{
    public static readonly Dictionary<string, ItemDef> All = new();

    static ItemDef W(string id, string name, int n, int s, string type, float kg, int sp, string prop = null, bool fin = false, bool light = false, bool two = false, float range = 0, string desc = "")
    {
        var d = new ItemDef { Id = id, Name = name, Kind = ItemKind.Weapon, DiceN = n, DiceS = s, DmgType = type, Weight = kg, Price = sp, Good = "arms", Prop = prop, Finesse = fin, Light = light, TwoHanded = two, Range = range, Desc = desc };
        All[id] = d; return d;
    }
    static ItemDef A(string id, string name, ItemKind kind, int ac, int dexMax, float kg, int sp, string good = "arms", bool stealth = false, int str = 0, string desc = "")
    {
        var d = new ItemDef { Id = id, Name = name, Kind = kind, ArmorBase = ac, DexMax = dexMax, Weight = kg, Price = sp, Good = good, StealthDis = stealth, StrReq = str, Desc = desc };
        All[id] = d; return d;
    }
    static ItemDef C(string id, string name, ItemKind kind, float kg, int sp, string good, int hn = 0, int hs = 0, int hb = 0, string desc = "")
    {
        var d = new ItemDef { Id = id, Name = name, Kind = kind, Weight = kg, Price = sp, Good = good, HealN = hn, HealS = hs, HealB = hb, Desc = desc };
        All[id] = d; return d;
    }

    static Items()
    {
        // silahlar (PHB): fiyat gümüş (1 altın = 10 gümüş)
        W("club", "Sopa", 1, 4, "ezici", 1f, 1, "tool_club", light: true, desc: "Kalın bir meşe dalı. Ucuz ve kaba.");
        W("dagger", "Hançer", 1, 4, "delici", 0.5f, 20, "proc_dagger", fin: true, light: true, desc: "Çevik ellerde ölümcül; haydutun sinsi saldırısına uygun.");
        W("staff", "Asa", 1, 6, "ezici", 2f, 2, "proc_staff", two: false, desc: "Yürürken baston, kavgada sopa.");
        W("spear", "Mızrak", 1, 6, "delici", 1.5f, 10, "tool_spear", desc: "Uzun saplı, sağlam uçlu.");
        W("handaxe", "El baltası", 1, 6, "kesici", 1f, 50, "tool_axe", light: true, desc: "Oduncunun da savaşçının da işini görür.");
        W("scimitar", "Pala", 1, 6, "kesici", 1.5f, 100, "proc_scimitar", fin: true, light: true, desc: "Goblinlerin sevdiği kavisli kılıç.");
        W("shortsword", "Kısa kılıç", 1, 6, "delici", 1f, 100, "proc_shortsword", fin: true, light: true, desc: "Çevik ve hafif.");
        W("mace", "Topuz", 1, 6, "ezici", 2f, 50, "tool_hammer", desc: "Rahiplerin sevdiği demir başlı topuz.");
        W("longsword", "Uzun kılıç", 1, 8, "kesici", 1.5f, 150, "proc_longsword", desc: "Savaşçının kılıcı.");
        W("shortbow", "Kısa yay", 1, 6, "delici", 1f, 250, "proc_bow", two: true, range: 24f, desc: "24 m'ye dek vurur; oklar kılıfta.");
        // zırhlar
        A("leather", "Deri zırh", ItemKind.Armor, 11, 99, 5f, 100, "leather", desc: "Sertleştirilmiş deri. Hafif.");
        A("chainshirt", "Zincir gömlek", ItemKind.Armor, 13, 2, 10f, 500, desc: "Orta zırh: çevikliğin en çok +2'si sayılır.");
        A("chainmail", "Zincir zırh", ItemKind.Armor, 16, 0, 25f, 750, stealth: true, str: 13, desc: "Ağır zırh: çeviklik sayılmaz, Güç 13 ister.");
        A("shield", "Kalkan", ItemKind.Shield, 2, 99, 3f, 100, desc: "+2 zırh sınıfı.");
        // tüketilebilirler
        C("potion", "Şifa iksiri", ItemKind.Potion, 0.5f, 110, "potion", 2, 4, 2, "İçen 2d4+2 can bulur; baygını ayağa kaldırır.");
        C("bandage", "Sargı bezi", ItemKind.Bandage, 0.2f, 3, "herbs", 0, 0, 1, "Baygın bir dostun kanamasını durdurur ve onu 1 canla kaldırır; savaş dışında 1d4 can sarar.");
        C("ration", "Erzak", ItemKind.Ration, 0.8f, 3, "bread", desc: "Bir günlük yol azığı: ekmek, peynir, kuru et.");
        C("herb", "Şifalı ot", ItemKind.Herb, 0.1f, 2, "herbs", desc: "Ormanda ve dere boyunda biter. Şifacı ve hancı alır.");
        C("trinket", "Goblin ıvır zıvırı", ItemKind.Loot, 0.2f, 4, "salt", desc: "Boncuk, kırık tarak, bir kemik zar… Hancı birkaç gümüş verir.");
        All["potion"].Plural = "Şifa iksirleri";
    }

    public static ItemDef Get(string id) => id != null && All.TryGetValue(id, out var d) ? d : null;
}

/// <summary>A stack of identical items.</summary>
public sealed class ItemStack
{
    public string Id;
    public int Count = 1;
    public ItemDef Def => Items.Get(Id);
}

/// <summary>Weight-limited item list + purse (silver).</summary>
public sealed class Inventory
{
    public List<ItemStack> Items = new();
    public int Silver;

    public float Weight
    {
        get { float w = 0; foreach (var s in Items) w += (s.Def?.Weight ?? 0) * s.Count; return w; }
    }

    public int Count(string id) { int n = 0; foreach (var s in Items) if (s.Id == id) n += s.Count; return n; }
    public bool Has(string id, int n = 1) => Count(id) >= n;

    public void Add(string id, int n = 1)
    {
        if (n <= 0 || Rpg.Items.Get(id) == null) return;
        foreach (var s in Items) if (s.Id == id) { s.Count += n; return; }
        Items.Add(new ItemStack { Id = id, Count = n });
    }

    public bool Remove(string id, int n = 1)
    {
        if (!Has(id, n)) return false;
        for (int i = Items.Count - 1; i >= 0 && n > 0; i--)
        {
            var s = Items[i];
            if (s.Id != id) continue;
            int k = Math.Min(n, s.Count);
            s.Count -= k; n -= k;
            if (s.Count <= 0) Items.RemoveAt(i);
        }
        return true;
    }

    /// <summary>Move everything (items and silver) into another inventory (looting, robbery).</summary>
    public void MoveAllTo(Inventory o, Predicate<ItemStack> keep = null)
    {
        for (int i = Items.Count - 1; i >= 0; i--)
        {
            var s = Items[i];
            if (keep != null && keep(s)) continue;
            o.Add(s.Id, s.Count);
            Items.RemoveAt(i);
        }
        o.Silver += Silver; Silver = 0;
    }
}
