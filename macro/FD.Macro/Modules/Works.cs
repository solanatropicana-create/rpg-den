using System;
using System.Collections.Generic;

// Faz 1b-7: tepki inşaatı ve büyük projeler (yol haritası v3: "İnşaat tepkidir, plan değil"). Sur ancak baskından, kuşatmadan,
// yağmadan, ejderha ya da deniz akınından sonra (Settlement.Alarm, ALARM_DAYS içinde) ya da savaşta sınır boyunda kurulur; yanan evler
// onarılır (Economy.RepairTick); kıtlıktan sonra yeni tarla açılır (yiyecek veren çıkarma yapılarının puanı artar). Kale ve fener kulesi
// sıradan inşaat listesinde yok: büyük proje olarak, olay gibi ve nadiren gelir (sınır kalesi rakibe bakan Kasaba ve üstü yerleşimde,
// fener kulesi tersaneli Şehir limanında); 15–30 gün sürer ve iş üretir: rakip devlet ya da Hırsızlar Loncası şantiyeyi sabote edebilir.

namespace FD.Macro;

public static class Works
{
    /// <summary>tehditten sonra sur kurma isteğinin sürdüğü gün</summary>
    public const double ALARM_DAYS = 40;
    /// <summary>kıtlıktan sonra yeni tarla isteğinin sürdüğü gün</summary>
    public const double FARM_DAYS = 40;
    /// <summary>devlet başına günlük büyük proje olasılığı</summary>
    public const double GRAND_RATE = 1.0 / 400;
    /// <summary>süren büyük projede WORLD_DAYS başına sabotaj olasılığı ve geri giden iş payı</summary>
    public const double SABOTAGE_P = 0.08, SABOTAGE_LOSS = 0.2;

    /// <summary>Yerleşim tehdit gördü (baskın, kuşatma, yağma, akın).</summary>
    public static void Alarm(Sim s, Settlement st)
    {
        if (st == null || !st.Alive) return;
        bool walled = J.T(st.Civics.Get("palisade")) && (st.Tier < 2 || J.T(st.Civics.Get("stonewall")));
        if (!walled && (st.Alarm == null || s.Day - st.Alarm.Value > ALARM_DAYS)) s.Metric("alarmOpen");
        st.Alarm = s.Day;
    }

    public static bool Alarmed(Sim s, Settlement st) => st.Alarm != null && s.Day - st.Alarm.Value <= ALARM_DAYS;

    /// <summary>Kıtlık sürüyor ya da yakında bitti (yeni tarla).</summary>
    public static bool Hungry(Sim s, Settlement st) => st.Status == "shortage" || st.Status == "hunger" || (st.ShortDay != null && s.Day - st.ShortDay.Value <= FARM_DAYS);

    /// <summary>Savaştaki devletin sınır yerleşimi: düşmanın bir yerleşimine 12 fersah yakın.</summary>
    public static bool Frontier(Sim s, Civ c, Settlement st)
    {
        foreach (var o in s.W.Civs)
        {
            if (!o.Alive || o.Id == c.Id || !s.AtWar(c.Id, o.Id)) continue;
            foreach (var x in s.CivSettlements(o)) if (s.G.Dist(x.Tile, st.Tile) <= 12) return true;
        }
        return false;
    }

    /// <summary>Sur projesi başladı (Economy.ChooseBuilds): tehditten bu yana geçen gün ölçülür.</summary>
    public static void WallStarted(Sim s, Settlement st, string kind)
    {
        s.Metric("reactWall"); s.Metric("reactWall_" + kind);
        if (Alarmed(s, st)) { s.Metric("reactWallAlarm"); s.Metric("reactWallDays", s.Day - st.Alarm.Value); }
    }

    // ------------------------------------------------------------ büyük projeler
    /// <summary>Her WORLD_DAYS günde: devlet başına nadiren büyük proje; süren büyük projelere sabotaj.</summary>
    public static void Tick(Sim s)
    {
        var w = s.W;
        foreach (var c in w.Civs)
        {
            if (!c.Alive || !s.Rng.Chance(GRAND_RATE * Sim.WORLD_DAYS)) continue;
            TryGrand(s, c);
        }
        foreach (var st in w.Settlements)
        {
            if (!st.Alive || st.Project == null || !J.T(st.Project.Grand) || !s.Rng.Chance(SABOTAGE_P)) continue;
            var c = w.Civs[st.Civ];
            var rival = Rival(s, c, st);
            bool thieves = rival == null && Orgs.BranchLevel(s, st, "thieves") > 0;
            if (rival == null && !thieves) continue;
            st.Project.Left += st.Project.Total * SABOTAGE_LOSS;
            s.Metric("grandSabotage");
            s.Log("build", $"{Tr.Ek(st.Name, "da")} {J.TrLower(Economy.CivicName(c, st.Project.Kind))} şantiyesi sabote edildi; iş geriledi.", civ: c.Id, tile: st.Tile,
                cause: rival != null ? $"{rival.Name} ajanları" : "Hırsızlar Loncası taş kervanını soydu", major: false);
        }
    }

    /// <summary>Rakip: savaşta olunan ya da ilişkisi −20'nin altında olan, yerleşimi 16 fersah içinde olan devlet.</summary>
    private static Civ Rival(Sim s, Civ c, Settlement st)
    {
        Civ best = null; double bd = 16;
        foreach (var o in s.W.Civs)
        {
            if (!o.Alive || o.Id == c.Id || !s.Rel(c.Id, o.Id).Contact) continue;
            if (!s.AtWar(c.Id, o.Id) && s.RelValue(c.Id, o.Id) > -20) continue;
            foreach (var x in s.CivSettlements(o)) { double d = s.G.Dist(x.Tile, st.Tile); if (d <= bd) { bd = d; best = o; } }
        }
        return best;
    }

    /// <summary>sınır kalesinin bedeli (büyük proje)</summary>
    public static readonly JsObj<double> CASTLE_COST = new JsObj<double> { ["stone"] = 80, ["wood"] = 40, ["gold"] = 60 };

    /// <summary>Büyük proje: sınır kalesi (rakibe bakan Kasaba ve üstü, kalesi yok) ya da fener kulesi (tersaneli Şehir limanı, kulesi yok).
    /// Bedel ambardan; yetmezse olmaz.</summary>
    private static void TryGrand(Sim s, Civ c)
    {
        var cands = new List<(Settlement St, string Kind, Civ Rival)>();
        foreach (var st in s.CivSettlements(c))
        {
            if (st.Project != null || st.Hub != null) continue;
            if (st.Tier >= 2 && !J.T(st.Civics.Get("castle")))
            {
                var r = Rival(s, c, st);
                if (r != null) cands.Add((st, "castle", r));
            }
            if (st.Tier >= Sim.BIG_TIER && J.T(st.Civics.Get("shipyard")) && st.Port != null && !J.T(st.Civics.Get("lighthouse"))) cands.Add((st, "lighthouse", null));
        }
        if (cands.Count == 0) return;
        var pick = cands[(int)s.Rng.Int(0, cands.Count - 1)];
        // sınır kalesi devletin işidir: taş ve kereste ambardan, demir ve aletin yerine hazineden usta ücreti (demir ve alet geç yıllarda kıt)
        var cost = pick.Kind == "castle" ? CASTLE_COST : D.CIVICS[pick.Kind].Cost;
        if (!s.CanPay(c, cost)) { foreach (var g in cost.Keys()) c.Want.Set(g, (c.Want.Get(g) ?? 0) + (cost.Get(g) ?? 0)); return; }
        s.Pay(c, cost);
        double work = Economy.CivicWork(pick.Kind);
        pick.St.Project = new Project { Type = "civic", Kind = pick.Kind, Left = work, Total = work, Grand = true };
        s.Metric("grandProject"); s.Metric("grandProject_" + pick.Kind);
        string what = pick.Kind == "castle" ? "bir sınır kalesi" : "bir fener kulesi";
        s.Log("build", $"{c.Name}, {Tr.Ek(pick.St.Name, "da")} {what} yaptırıyor: taş kervanları yolda, şantiyede iş var.", civ: c.Id, tile: pick.St.Tile,
            cause: pick.Rival != null ? $"Sınırda {pick.Rival.Name}" : "Limanın gemileri gece yolunu yitiriyor", major: true);
    }
}
