using System;
using System.Collections.Generic;

// Faz 1b-7: fırsat merkezi döngüsü (yol haritası v3, geçici halka; patlama ve çöküş):
//   söylenti (1–2 gün): tavernada duyulur;
//   hücum (3–6): merkez kurulur, çevreden insanlar akın eder;
//   zirve (4–10): para ve haydutlar gelir, sahibine altın akar;
//   tükeniş (2–6): kaçış başlar;
//   hayalet (3–6): kasaba boşalır, goblinler yerleşebilir (harabe).
// Tetikleyiciler: maden bulundu (yerleşimin "kaynak bulundu" durumu ya da tepelerde yeni damar), savaş cephesi (karargâh kurmuş ordunun
// çevresinde tüccar ve kumar kasabası), antik harabe (terk edilmiş yerleşim; Harabe Kâşifleri), kutsal kalıntı (hac), yeni yol
// (yolun ıssız bir yerinde konak), temizlenmiş in (verimli vadi; dünyada yer varsa kalıcı köy olur: v3 "kamp → köy 10–20 gün").
// Oyun bir maden oyununa dönmesin: türler eşit ağırlıkta, madenin payı küçük.
// Merkez sıradan bir yerleşimdir (Settlement.Hub): ekonomisi, baskını, fethi aynı kurallarla; inşa etmez, şubesi olmaz, çadırda yaşar,
// dünyanın yerleşim tavanına ve ölçümün kalıcı yerleşim sayısına girmez.

namespace FD.Macro;

public static class Hubs
{
    public static readonly int[] RUMOR = { 1, 2 }, RUSH = { 3, 6 }, PEAK = { 4, 10 }, BUST = { 2, 6 }, GHOST = { 3, 6 };
    /// <summary>aynı anda en çok bu kadar merkez</summary>
    public const int MAX_ACTIVE = 2;
    /// <summary>iki tetikleyici arası (gün)</summary>
    public static readonly int[] GAP = { 12, 24 };
    /// <summary>merkezin hücumda ve zirvede çektiği nüfus (kişi; çevrenin gücüyle sınırlı)</summary>
    public static readonly int[] RUSH_POP = { 10, 18 }, PEAK_POP = { 6, 12 };
    /// <summary>zirvede kişi başı günlük altın (sahibine; tür çarpanı KIND_GOLD)</summary>
    public const double GOLD_PER = 0.05;

    private static readonly Dictionary<string, double> KIND_GOLD = new() { ["mine"] = 1.4, ["front"] = 1.2, ["ruin"] = 0.6, ["relic"] = 0.5, ["road"] = 0.8, ["valley"] = 0.3 };
    private static readonly string[] KINDS = { "front", "ruin", "relic", "road", "mine" };
    private static readonly Dictionary<string, string> KIND_TR = new() { ["mine"] = "maden", ["front"] = "ordu pazarı", ["ruin"] = "antik harabe", ["relic"] = "kutsal kalıntı", ["road"] = "yol konağı", ["valley"] = "verimli vadi" };
    public static string KindName(string k) => KIND_TR.TryGetValue(k, out var v) ? v : k;

    private static Hub HubOf(Sim s, Settlement st)
    {
        if (st?.Hub == null) return null;
        foreach (var h in s.W.Hubs) if (h.Id == st.Hub.Value) return h;
        return null;
    }

    /// <summary>Merkez göçmen alıyor mu (hücum, zirve).</summary>
    public static bool Open(Sim s, Settlement st) => HubOf(s, st) is { } h && (h.Phase == "rush" || h.Phase == "peak");

    /// <summary>Merkezden kaçılıyor mu (tükeniş, hayalet): göçmen bağışlar.</summary>
    public static bool Leaving(Sim s, Settlement st) => HubOf(s, st) is { } h && (h.Phase == "bust" || h.Phase == "ghost");

    private static int Active(Sim s)
    {
        int n = 0;
        foreach (var h in s.W.Hubs) if (h.Phase != "done") n++;
        return n;
    }

    // ------------------------------------------------------------ günlük
    public static void Tick(Sim s)
    {
        var w = s.W;
        for (int i = 0; i < w.Hubs.Count; i++)
        {
            var h = w.Hubs[i];
            if (h.Phase == "done") continue;
            var st = h.Settlement != null ? s.Settlement(h.Settlement.Value) : null;
            if (h.Phase != "rumor" && (st == null || !st.Alive)) { Done(s, h, "lost"); continue; }
            if (st != null) { h.Civ = st.Civ; h.PeakPop = JsMath.Max(h.PeakPop, s.Pop(st)); }
            if (h.Phase == "peak") Yield(s, h, st);
            // savaş cephesi: ordu çekilince ya da barış gelince pazar söner
            if (h.Kind == "front" && (h.Phase == "rush" || h.Phase == "peak") && !FrontAlive(s, h)) h.Until = JsMath.Min(h.Until, s.Day);
            if (s.Day < h.Until) continue;
            Advance(s, h, st);
        }
        if (s.Day < w.NextHub) return;
        w.NextHub = s.Day + s.Rng.Int(GAP[0], GAP[1]);
        if (Active(s) >= MAX_ACTIVE) return;
        // tür eşit olasılıkla seçilir; seçilen tutmazsa maden dışındakiler denenir (maden yedek olmasın: oyun bir maden oyununa dönmesin)
        var order = s.Rng.Shuffle(new List<string>(KINDS));
        for (int k = 0; k < order.Count; k++)
        {
            if (k > 0 && order[k] == "mine") continue;
            if (TryTrigger(s, order[k])) return;
        }
    }

    private static void Advance(Sim s, Hub h, Settlement st)
    {
        var w = s.W;
        switch (h.Phase)
        {
            case "rumor":
            {
                st = Found(s, h);
                if (st == null) { Done(s, h, "gone"); return; }
                h.Phase = "rush"; h.Rush = s.Day; h.Until = s.Day + s.Rng.Int(RUSH[0], RUSH[1]);
                Status.Attract(s, st, s.Rng.Int(RUSH_POP[0], RUSH_POP[1]), $"{KindName(h.Kind)}: hücum");
                s.Log("hub", $"{Tr.Ek(h.Name, "a")} hücum başladı: {RushText(h)}", civ: st.Civ, tile: h.Tile, cause: $"{J.TrCap(KindName(h.Kind))} haberi yayıldı", major: true);
                break;
            }
            case "rush":
            {
                h.Phase = "peak"; h.Peak = s.Day; h.Until = s.Day + s.Rng.Int(PEAK[0], PEAK[1]);
                Status.Attract(s, st, s.Rng.Int(PEAK_POP[0], PEAK_POP[1]), $"{KindName(h.Kind)}: zirve");
                // para ve haydutlar gelir
                if (s.Rng.Chance(0.35)) Bandits(s, h, st);
                s.Log("hub", $"{h.Name} zirvede: hanlar dolu, kumar masaları kalabalık.", civ: st.Civ, tile: h.Tile, cause: $"{J.S(s.Pop(st))} kişi", major: false);
                break;
            }
            case "peak":
            {
                h.Phase = "bust"; h.Bust = s.Day; h.Until = s.Day + s.Rng.Int(BUST[0], BUST[1]);
                double P = s.Pop(st);
                Status.Emigrate(s, st, Math.Floor(P * 0.6), $"{h.Name} tükeniyor");
                s.Log("hub", $"{h.Name} tükeniyor: {BustText(h)}", civ: st.Civ, tile: h.Tile, major: false);
                break;
            }
            case "bust":
            {
                // verimli vadi: tükenişte fazlası gider, kalanlar köy kurar (dünyada ve devletin toprağında yer varsa; v3: kamp → köy 10–20 gün)
                if (h.Kind == "valley" && Diplomacy.PermanentCount(s) < (w.SettleCap > 0 ? w.SettleCap : Diplomacy.WorldCap(s)) + Diplomacy.FREED_SLACK
                    && LandRoom(s, w.Civs[st.Civ]))
                {
                    st.Hub = null; st.FromCamp = h.Origin;
                    s.Metric("hubVillage");
                    s.Log("settle", $"{h.Name} kalıcı bir köy oldu: kalan çiftçiler vadiye yerleşti.", civ: st.Civ, tile: h.Tile, cause: "Temizlenen vadinin toprağı verimli", major: true);
                    Done(s, h, "village");
                    return;
                }
                h.Phase = "ghost"; h.Ghost = s.Day; h.Until = s.Day + s.Rng.Int(GHOST[0], GHOST[1]);
                double P = s.Pop(st);
                Status.Emigrate(s, st, JsMath.Max(0, P - s.Rng.Int(1, 3)), $"{h.Name} boşalıyor");
                s.Log("hub", $"{h.Name} hayalet kasabaya döndü; birkaç inatçı kaldı.", civ: st.Civ, tile: h.Tile, major: false);
                break;
            }
            default:
            {
                // hayalet: son kalanlar da gider, goblinler yerleşebilir
                double P = s.Pop(st);
                if (P > 0) Status.Emigrate(s, st, P, $"{h.Name} terk edildi");
                if (s.Pop(st) > 0) { var rest = s.RemovePop(st, s.Pop(st)); var to = Nearest(s, st); if (to != null) s.MergePop(to, rest); }
                int tile = st.Tile;
                s.Abandon(st, "Fırsat bitti, kasaba boşaldı");
                if (Monsters.CampRoom(s) > 0 && s.Rng.Chance(0.4) && w.Tiles[tile].Camp == null && w.Tiles[tile].Owner < 0)
                {
                    var cp = WorldGen.MakeCamp(s.Id(), "goblin", tile, $"{h.Name} Harabesi", s.Day, s.Rng);
                    cp.Count = 4; cp.NextRaid = s.Day + s.Rng.Int(15, 30);
                    w.Camps.Add(cp); w.Tiles[tile].Camp = cp.Id;
                    s.Metric("hubGoblins");
                    s.Log("lair", $"Goblinler boşalan {Tr.Ek(h.Name, "a")} yerleşti.", tile: tile, cause: "Hayalet kasaba", major: true);
                }
                Done(s, h, "ghost");
                break;
            }
        }
    }

    /// <summary>Devletin anakara yerleşim tavanında (Diplomacy.LandCap) yer var mı (fırsat merkezleri ve denizaşırı koloniler sayılmaz).</summary>
    private static bool LandRoom(Sim s, Civ c)
    {
        int n = 0;
        foreach (var x in s.CivSettlements(c)) if (x.Hub == null && !J.T(x.Overseas)) n++;
        return n < Diplomacy.LandCap(s, c) + 1;
    }

    private static void Done(Sim s, Hub h, string outcome)
    {
        h.Phase = "done"; h.End = s.Day; h.Outcome = outcome;
        // zirvede gelen haydutlar parayla birlikte gider
        if (h.Settlement != null)
            foreach (var cp in s.W.Camps)
                if (cp.Alive && cp.Kind == "bandit" && !J.T(cp.Hungry) && cp.Home == h.Settlement) { cp.Alive = false; cp.Count = 0; cp.ClearedDay = s.Day; s.W.Tiles[cp.Tile].Camp = null; s.Metric("hubBanditsLeft"); }
        s.Metric("hubDone"); s.Metric("hubDone_" + outcome);
    }

    private static Settlement Nearest(Sim s, Settlement st)
    {
        Settlement best = null; double bd = double.PositiveInfinity;
        foreach (var x in s.W.Settlements) { if (!x.Alive || x.Id == st.Id || x.Hub != null) continue; double d = s.G.Dist(x.Tile, st.Tile); if (d < bd) { bd = d; best = x; } }
        return best;
    }

    /// <summary>Zirvede sahibine akan (tür çarpanıyla): altın; harabede Harabe Kâşifleri'ne, kutsal kalıntıda Kilise'ye; vadide tahıl.</summary>
    private static void Yield(Sim s, Hub h, Settlement st)
    {
        var c = s.W.Civs[st.Civ];
        double v = s.Pop(st) * GOLD_PER * (KIND_GOLD.TryGetValue(h.Kind, out double m) ? m : 1);
        if (h.Kind == "valley") { s.Add(c, "grain", v * 3); return; }
        s.Add(c, "gold", v); h.Gold += v;
        string ok = h.Kind == "ruin" ? "explorers" : h.Kind == "relic" ? "church" : h.Kind == "front" ? "companies" : null;
        var o = ok != null ? Orgs.Find(s, ok) : null;
        if (o != null) { o.Gold += v * 0.5; o.Tally.Set("hub", (o.Tally.Get("hub") ?? 0) + v * 0.5); }
    }

    /// <summary>Zirvede haydutlar gelir (tok haydut kampı: yağma için saldırır).</summary>
    private static void Bandits(Sim s, Hub h, Settlement st)
    {
        var w = s.W;
        int best = -1; double bs = double.NegativeInfinity;
        foreach (int i in s.G.Within(st.Tile, 6))
        {
            var t = w.Tiles[i];
            double d = s.G.Dist(i, st.Tile);
            if (d < 3 || t.Terrain == "water" || t.Terrain == "mountain" || J.T(t.Sea) || t.Owner >= 0 || t.Camp != null || t.Inn != null || t.InnZone != null) continue;
            double sc = (t.Terrain == "forest" || t.Terrain == "hill" ? 2 : 0) - Math.Abs(d - 4) + s.Rng.Next();
            if (sc > bs) { bs = sc; best = i; }
        }
        if (best < 0) return;
        var cp = WorldGen.MakeCamp(s.Id(), "bandit", best, $"{h.Name} Haydutları", s.Day, s.Rng);
        cp.Count = s.Rng.Int(3, 6); cp.Loot = 0; cp.NextRaid = s.Day + s.Rng.Int(2, 6);
        cp.Home = st.Id;   // merkez bitince parayla birlikte giderler (Done)
        w.Camps.Add(cp); w.Tiles[best].Camp = cp.Id;
        s.Metric("hubBandits");
        s.Log("lair", $"{h.Name} çevresine haydutlar yerleşti: kervanlar ve altın onları çekti.", tile: best, civ: st.Civ, cause: "Fırsat merkezi zirvede", major: false);
    }

    /// <summary>Savaş cephesi sürüyor mu (saldıran ile saldırılan hâlâ savaşta).</summary>
    private static bool FrontAlive(Sim s, Hub h) => h.Origin != null && h.Front != null && s.AtWar(h.Origin.Value, h.Front.Value);

    private static string RushText(Hub h) => h.Kind switch
    {
        "mine" => "kazmalar, çadırlar, fiyatlar uçuyor.",
        "front" => "ordunun ardından tüccarlar, aşçılar ve kumarbazlar geldi.",
        "ruin" => "kâşifler ve hazine avcıları kazıya koşuyor.",
        "relic" => "hacı kafileleri yollarda.",
        "road" => "yeni yolun kervanları konakta mola veriyor.",
        _ => "çiftçiler boşalan vadiye iniyor.",
    };

    private static string BustText(Hub h) => h.Kind switch
    {
        "mine" => "damar inceldi, kazmalar susuyor.",
        "front" => "ordu çekildi, tüccarlar çadırlarını topluyor.",
        "ruin" => "harabenin değerli ne varsa çıkarıldı.",
        "relic" => "hacıların ardı kesildi.",
        "road" => "kervanlar başka yolu seçti.",
        _ => "vadide herkese yetecek toprak yok.",
    };

    // ------------------------------------------------------------ tetikleyiciler
    /// <summary>Merkez için boş karo: kara (su, dağ, deniz değil), sahipsiz, kamp/han yok, adada değil, yaşayan yerleşime en az 3 fersah.</summary>
    private static bool FreeTile(Sim s, int i)
    {
        var t = s.W.Tiles[i];
        if (t.Terrain == "water" || t.Terrain == "mountain" || J.T(t.Sea) || J.T(t.Isle) || t.Owner >= 0 || t.Camp != null || t.Inn != null || t.InnZone != null) return false;
        foreach (var x in s.W.Settlements) if (x.Alive && s.G.Dist(x.Tile, i) < 3) return false;
        return true;
    }

    /// <summary>Merkezin sahibi: karoya en yakın yerleşimi 16 fersah içindeki devlet; yoksa null.</summary>
    private static Settlement Patron(Sim s, int tile)
    {
        Settlement best = null; double bd = 16;
        foreach (var x in s.W.Settlements)
        {
            if (!x.Alive || x.Hub != null || x.Civ < 0 || !s.W.Civs[x.Civ].Alive) continue;
            double d = s.G.Dist(x.Tile, tile);
            if (d < bd) { bd = d; best = x; }
        }
        return best;
    }

    /// <summary>Çevrede boş bir karo (merkeze en yakın; karonun kendisi de olur).</summary>
    private static int NearFree(Sim s, int center, int r)
    {
        int best = -1; double bd = double.PositiveInfinity;
        foreach (int i in s.G.Within(center, r))
        {
            if (!FreeTile(s, i)) continue;
            double d = s.G.Dist(i, center) + s.Rng.Next() * 0.5;
            if (d < bd) { bd = d; best = i; }
        }
        return best;
    }

    private static bool TryTrigger(Sim s, string kind)
    {
        var w = s.W;
        switch (kind)
        {
            case "front":
            {
                // savaşın cephesi: hedef şehrin 3–7 fersah ötesinde, saldıranın başkentine bakan yanda (ordunun karargâhı, tüccar ve kumar)
                foreach (var att in w.Civs)
                {
                    if (!att.Alive || J.Some(w.Hubs, x => x.Phase != "done" && x.Kind == "front" && x.Origin == att.Id)) continue;
                    var cap = s.Capital(att);
                    if (cap == null) continue;
                    foreach (var def in w.Civs)
                    {
                        var war = def.Id != att.Id ? s.Rel(att.Id, def.Id).War : null;
                        if (war == null || war.Attacker != att.Id) continue;
                        var tgt = s.Settlement(war.Target);
                        if (tgt == null || !tgt.Alive || tgt.Civ != def.Id) continue;
                        int t = -1; double bd = double.PositiveInfinity;
                        foreach (int i2 in s.G.Within(tgt.Tile, 7))
                        {
                            double d0 = s.G.Dist(i2, tgt.Tile);
                            if (d0 < 3 || !FreeTile(s, i2)) continue;
                            double d = s.G.Dist(i2, cap.Tile) + s.Rng.Next();
                            if (d < bd) { bd = d; t = i2; }
                        }
                        if (t < 0) continue;
                        if (!Begin(s, "front", t, $"{att.Stem ?? att.Name} Ordugâhı", att.Id, $"{att.Name} ordusu {Tr.Ek(tgt.Name, "a")} karşı karargâh kurdu", att.Id)) continue;
                        w.Hubs[w.Hubs.Count - 1].Front = def.Id;
                        return true;
                    }
                }
                return false;
            }
            case "ruin":
            {
                var ruins = J.Filter(w.Settlements, x => !x.Alive && x.Hub == null && s.Day - x.Founded > 200 && FreeTile(s, x.Tile) && !J.Some(w.Hubs, h => h.Origin == x.Id && h.Kind == "ruin"));
                if (ruins.Count == 0) return false;
                var r = s.Rng.Pick(ruins);
                return Begin(s, "ruin", r.Tile, $"{r.Name} Kazısı", null, $"{r.Name} harabelerinde eski bir mahzen bulundu", r.Id);
            }
            case "relic":
            {
                var holy = J.Filter(w.Settlements, x => x.Alive && x.Hub == null && x.Tier >= 2 && (J.T(x.Civics.Get("temple")) || Polity.OldWays(x)));
                if (holy.Count == 0) return false;
                var c = s.Rng.Pick(holy);
                int t = -1;
                for (int k = 0; k < 6 && t < 0; k++) { var ring = J.Filter(s.G.Within(c.Tile, 9), i => s.G.Dist(i, c.Tile) >= 5 && FreeTile(s, i)); if (ring.Count > 0) t = s.Rng.Pick(ring); }
                if (t < 0) return false;
                bool old = Polity.OldWays(c);
                return Begin(s, "relic", t, old ? $"{c.Name} Kutsal Korusu" : $"{c.Name} Hac Yeri", null, old ? "Bir ata ruhu göründü" : "Bir azizin kalıntısı bulundu", c.Id);
            }
            case "road":
            {
                var cands = new List<int>();
                for (int i = 0; i < w.Tiles.Count; i++) if (J.T(w.Tiles[i].Road) && w.Tiles[i].Inn == null && FreeTile(s, i)) cands.Add(i);
                cands = J.Filter(cands, i => !J.Some(w.Settlements, x => x.Alive && s.G.Dist(x.Tile, i) < 5));
                if (cands.Count == 0) return false;
                int t = s.Rng.Pick(cands);
                return Begin(s, "road", t, $"{TownName(s, t)} Konağı", null, "Yeni yolun ıssız bir yerinde konak kuruldu", null);
            }
            default:
            {
                var cands = new List<int>();
                for (int i = 0; i < w.Tiles.Count; i++)
                {
                    var t = w.Tiles[i];
                    if (t.Terrain != "hill" || !FreeTile(s, i)) continue;
                    double d = double.PositiveInfinity;
                    foreach (var x in w.Settlements) if (x.Alive) d = JsMath.Min(d, s.G.Dist(x.Tile, i));
                    if (d >= 4 && d <= 12) cands.Add(i);
                }
                if (cands.Count == 0) return false;
                int tt = s.Rng.Pick(cands);
                return Begin(s, "mine", tt, $"{TownName(s, tt)} Madeni", null, "Tepelerde bir damar parladı", null);
            }
        }
    }

    /// <summary>Yerleşimin "kaynak bulundu" durumu: %35 olasılıkla yakın tepelerde maden merkezi.</summary>
    public static void FromFind(Sim s, Settlement st)
    {
        if (Active(s) >= MAX_ACTIVE || !s.Rng.Chance(0.05)) return;
        var cands = J.Filter(s.G.Within(st.Tile, 9), i => s.G.Dist(i, st.Tile) >= 4 && FreeTile(s, i));
        if (cands.Count == 0) return;
        int t = s.Rng.Pick(cands);
        Begin(s, "mine", t, $"{TownName(s, t)} Madeni", st.Civ, $"{st.Name} yakınlarında zengin bir damar", st.Id);
    }

    /// <summary>Temizlenen kamp: verimli vadi (dünyada yer varsa kalıcı köy olur).</summary>
    public static void FromCleared(Sim s, Camp cp)
    {
        if (cp.Kind == "pirate" || cp.Kind == "dragon" || cp.Kind == "bandit" || Active(s) >= MAX_ACTIVE || !s.Rng.Chance(0.15)) return;
        int t = FreeTile(s, cp.Tile) ? cp.Tile : NearFree(s, cp.Tile, 2);
        if (t < 0) return;
        string stem = cp.Name.Replace(" Kampı", "").Replace(" Karakolu", "").Replace(" İni", "");
        Begin(s, "valley", t, $"{stem} Vadisi", null, $"{cp.Name} temizlendi; vadi boşaldı", cp.Id);
    }

    private static bool Begin(Sim s, string kind, int tile, string name, int? civ, string why, int? origin)
    {
        var w = s.W;
        var p = Patron(s, tile);
        if (p == null) return false;
        if (J.Some(w.Hubs, h => h.Phase != "done" && s.G.Dist(h.Tile, tile) < 6)) return false;
        name = UniqueName(s, name);
        var h = new Hub { Id = s.Id(), Kind = kind, Name = name, Tile = tile, Civ = civ ?? p.Civ, Phase = "rumor", Rumor = s.Day, Until = s.Day + s.Rng.Int(RUMOR[0], RUMOR[1]), Origin = origin };
        w.Hubs.Add(h);
        s.Metric("hub"); s.Metric("hub_" + kind);
        s.Log("hub", $"Tavernalarda söylenti: {Tr.Ek(p.Name, "in")} yakınlarında {RumorText(kind)}", civ: p.Civ, tile: tile, cause: why, major: false);
        return true;
    }

    private static string RumorText(string kind) => kind switch
    {
        "mine" => "altın damarı bulunmuş.",
        "front" => "ordunun karargâhı varmış; askerin parası bol.",
        "ruin" => "eski bir harabede hazine çıkmış.",
        "relic" => "kutsal bir kalıntı bulunmuş.",
        "road" => "yeni yolun ortasında bir konak açılmış.",
        _ => "canavarlardan temizlenen vadi boş kalmış.",
    };

    /// <summary>Hücum: merkez kurulur (sahibinin devletinde, kâşifleri en yakın yerleşimden).</summary>
    private static Settlement Found(Sim s, Hub h)
    {
        var w = s.W;
        if (!FreeTile(s, h.Tile)) { int t = NearFree(s, h.Tile, 2); if (t < 0) return null; h.Tile = t; }
        var p = Patron(s, h.Tile);
        if (p == null) return null;
        int civ = h.Civ >= 0 && h.Civ < w.Civs.Count && w.Civs[h.Civ].Alive ? h.Civ : p.Civ;
        // ilk kâşifler: 16 fersah içinde kişi verebilen en yakın yerleşimden
        Settlement donor = null; double dd = 17;
        foreach (var x in w.Settlements)
        {
            if (!x.Alive || x.Hub != null || x.Civ < 0 || (x.Civ != civ && s.AtWar(x.Civ, civ))) continue;
            double spare = s.Pop(x) - (Sim.IsBig(x) ? Status.BIG_KEEP : Sim.IsCore(x) ? Sim.CORE_MIN + 4 : 6);
            double d = s.G.Dist(x.Tile, h.Tile);
            if (spare >= 3 && d < dd) { dd = d; donor = x; }
        }
        if (donor == null) return null;
        double n = 3;
        var pop = s.RemovePop(donor, n);
        var st = WorldGen.MakeSettlement(s.Id(), civ, h.Name, h.Tile, pop, s.Day);
        st.Hub = h.Id;
        w.Settlements.Add(st);
        h.Settlement = st.Id; h.Civ = civ;
        s.UpdateTerritory();
        return st;
    }

    private static string UniqueName(Sim s, string name)
    {
        bool used(string n) => J.Some(s.W.Settlements, x => x.Name == n) || J.Some(s.W.Hubs, x => x.Name == n);
        if (!used(name)) return name;
        foreach (var r in new[] { "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X" }) if (!used($"{name} {r}")) return $"{name} {r}";
        return $"{name} {J.S(s.W.Hubs.Count)}";
    }

    private static readonly string[] A = { "Altın", "Gümüş", "Kızıl", "Kara", "Ak", "Demir", "Bakır", "Taş", "Yel", "Boz", "Çakmak", "Kartal", "Sarp", "Dolu", "Gece" };
    private static readonly string[] B = { "yar", "tepe", "sırt", "kaya", "dere", "ocak", "geçit", "yamaç", "çukur", "taşlık" };

    private static string TownName(Sim s, int tile)
    {
        for (int k = 0; k < 40; k++)
        {
            string n = A[(tile * 7 + k * 5) % A.Length] + B[(tile * 3 + k * 11) % B.Length];
            if (!J.Some(s.W.Settlements, x => x.Name.StartsWith(n, StringComparison.Ordinal)) && !J.Some(s.W.Hubs, x => x.Name.StartsWith(n, StringComparison.Ordinal))) return n;
        }
        return A[tile % A.Length] + B[s.W.Hubs.Count % B.Length];
    }
}
