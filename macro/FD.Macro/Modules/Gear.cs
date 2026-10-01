using System;
using System.Collections.Generic;
using System.Linq;

// Tüketilen mallar: teçhizat (efsunlu silah, mithril zırh askerin üstündedir), iksir (salgın ve yaralı),
// at (kervan atı). Faz 1b-3: atlar artık yaşlanmaz; deri kış giysisi değil (kentin ve askerin deri tüketimi Economy'de).
// Port of src/sim/gear.ts.

namespace FD.Macro;

public static class Gear
{
    private static readonly HashSet<string> ARMY_KINDS = new() { "army" };

    /// <summary>medeniyetin toplam askeri: yerleşimlerdekiler + yoldaki ordular</summary>
    public static double TotalSoldiers(Sim s, Civ c)
    {
        double n = 0;
        foreach (var st in s.CivSettlements(c)) n += st.Soldiers;
        foreach (var a in s.W.Agents) if (a.Civ == c.Id && ARMY_KINDS.Contains(a.Kind) && !J.T(a.Dead)) n += a.Troops ?? 0;
        return n;
    }

    /// <summary>TS <c>gear(c)</c>: <c>c.gear ??= { ench: 0, mith: 0, n: 0 }</c> (renamed: a member cannot share the class name).</summary>
    private static GearState GearOf(Civ c) => c.Gear ??= new GearState { Ench = 0, Mith = 0, N = 0 };

    /// <summary>Günlük: asker sayısı düştüyse (ölüm, dağılma) teçhizat orantılı kaybolur; fazlası kırpılır.</summary>
    public static void GearTick(Sim s, Civ c)
    {
        var g = GearOf(c);
        double n = TotalSoldiers(s, c);
        if (n < g.N && g.N > 0)
        {
            double keep = n / g.N;
            g.Ench *= keep; g.Mith *= keep;
        }
        g.Ench = JsMath.Min(g.Ench, n); g.Mith = JsMath.Min(g.Mith, n);
        g.N = n;
    }

    /// <summary>Askere efsun ve mithril zırh giydirilir (her 10 günde bir, sivil yapı kararlarından sonra).</summary>
    public static void EquipTick(Sim s, Civ c)
    {
        var g = GearOf(c);
        double n = TotalSoldiers(s, c);
        g.N = n;
        int enc = 0, mit = 0;
        int ct = s.CivTier(c);
        if (ct >= Gate.ENCHANTING)
        {
            // yeni askerlere silah ayrıldıktan sonra artan silah efsunlanır: 1 silah + 1 mana
            for (int i = 0; i < 3 && Math.Floor(g.Ench) < n && s.St(c, "mana") >= 1 && s.St(c, "arms") >= 2; i++)
            {
                s.Add(c, "mana", -1); s.Add(c, "arms", -1); g.Ench += 1; enc++;
            }
        }
        if (ct >= Gate.MITHRILWORK)
        {
            for (int i = 0; i < 3 && Math.Floor(g.Mith) < n && s.St(c, "mithril") >= 1; i++) { s.Add(c, "mithril", -1); g.Mith += 1; mit++; }
        }
        bool First(string k) => !J.T(c.Yearly.Get("gear_" + k));
        if (J.T(enc) && First("ench")) { c.Yearly.Set("gear_ench", s.Day); s.Log("class", $"{c.Name} ilk kez askerlerinin kılıçlarını mana kristaliyle efsunladı.", civ: c.Id, major: true, cause: "Efsunlu kılıç saldırıya +2 verir; asker ölünce kılıç da gider"); }
        if (J.T(mit) && First("mith")) { c.Yearly.Set("gear_mith", s.Day); s.Log("class", $"{c.Name} askerleri ilk mithril zırhlarını kuşandı.", civ: c.Id, major: true, cause: "Mithril zırh AC +2 verir; asker ölünce zırh da gider"); }
    }

    /// <summary>n kişilik birliğin kaçı efsunlu, kaçı mithril zırhlı (oran medeniyet genelinde)</summary>
    public static (double Ench, double Mith) GearFor(Sim s, Civ c, double n)
    {
        var g = c.Gear;
        if (g == null || n <= 0) return (0, 0);
        double T = JsMath.Max(1, TotalSoldiers(s, c));
        return (JsMath.Min(n, JsMath.Round(n * JsMath.Min(1, g.Ench / T))), JsMath.Min(n, JsMath.Round(n * JsMath.Min(1, g.Mith / T))));
    }

    /// <summary>savaştan sonra yaralı askerler: iksir varsa bir kısmı kurtarılır. Kurtarılan sayıyı döndürür.</summary>
    public static double HealWounded(Sim s, Civ c, double dead)
    {
        if (dead <= 0 || !s.CivAt(c, Gate.MEDICINE)) return 0;
        double saved = JsMath.Min(Math.Floor(s.St(c, "potion")), Math.Floor(dead * 0.4));
        if (saved <= 0) return 0;
        s.Add(c, "potion", -saved);
        s.Metric("potionSaved", saved);
        return saved;
    }

    /// <summary>salgında iksir: bu ay ölecek hasta sayısı için iksir harcanır; koruma oranı (0–0,35) döner</summary>
    public static double PlaguePotions(Sim s, Civ c, double sick)
    {
        if (sick <= 0) return 0;
        double use = JsMath.Min(Math.Floor(s.St(c, "potion")), sick);
        if (use <= 0) return 0;
        s.Add(c, "potion", -use);
        s.Metric("potionPlague", use);
        return 0.35 * use / sick;
    }

    /// <summary>kervan atı: kalkışta ambardan alınır, varışta döner; kervan pusuya düşerse at da gider</summary>
    public static bool TakeHorse(Sim s, Civ c)
    {
        if (s.St(c, "horses") < 1) return false;
        s.Add(c, "horses", -1);
        return true;
    }
}
