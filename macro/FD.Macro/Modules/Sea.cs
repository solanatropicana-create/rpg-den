using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Denizcilik: kıyı suları, limanlar, kara + deniz yol bulma, gemi hesapları,
// deniz yolculukları (koloni, ticaret, sefer, keşif), fırtına ve deniz savaşı.
// Exact port of src/sim/sea.ts (see macro/PORTING.md).

namespace FD.Macro;

/// <summary>TS <c>NavOpts</c>: options of the land + sea A* (<see cref="Sea.NavPath"/>).</summary>
public sealed class NavOpts
{
    /// <summary>binilebilecek karolar (limanlar): yalnız buralardan gemiye binilir</summary>
    public List<int> Embark;
    /// <summary>açık deniz (Seyir); false ise yalnız kıyı suyu izlenir</summary>
    public bool Open;
    /// <summary>TS <c>landOnly?</c>: yalnız bu karolarda karaya çıkılır (null = undefined)</summary>
    public List<int> LandOnly;
}

/// <summary>TS inline type <c>{ path: number[]; hull?: Settlement }</c> returned by civPath (and diplomacy's armyRoute); the function returns null when unreachable.</summary>
public sealed class CivPathResult
{
    /// <summary>kara/deniz yolu (NavPath/Path önbelleğindeki aynı liste örneği)</summary>
    public List<int> Path;
    /// <summary>TS <c>hull?</c>: gemiyi veren liman yerleşimi (null = undefined)</summary>
    public Settlement Hull;
}

public static class Sea
{
    public const double EMBARK = 1.5;      // yükleme/indirme: yol maliyetine eklenir
    public const double SEA_STEP = 0.5;    // deniz karosunun yol bulma maliyeti (kara yolu kadar)
    public static readonly UnitStats GALLEY = new UnitStats { Name = "Kadırga", Hp = 26, Ac = 13, Atk = 5, Dmg = new List<double> { 2, 8, 2 }, Attacks = 2 };
    public static readonly UnitStats COG = new UnitStats { Name = "Koga", Hp = 18, Ac = 11, Atk = 1, Dmg = new List<double> { 1, 4, 0 } };

    /// <summary>TS <c>a.purpose?.startsWith('explore')</c> used as a boolean (undefined is falsy).</summary>
    private static bool StartsExplore(string purpose) => purpose != null && purpose.StartsWith("explore", StringComparison.Ordinal);

    // ------------------------------------------------------------ harita
    /// <summary>kıyı suyu: karaya komşu deniz karosu (tekneler yalnız burada yol alır); s.ShoreW'de önbelleklenir</summary>
    public static byte[] ShoreWater(Sim s)
    {
        if (s.ShoreW != null) return s.ShoreW;
        var w = s.W;
        var @out = new byte[w.Tiles.Count];
        for (int i = 0; i < w.Tiles.Count; i++) if (J.T(w.Tiles[i].Sea) && J.Some(s.G.Neighbors(i), n => !J.T(w.Tiles[n].Sea))) @out[i] = 1;
        s.ShoreW = @out;
        return @out;
    }

    /// <summary>TS isSea: karo deniz mi (<c>!!tile.sea</c>)</summary>
    public static bool IsSea(Sim s, int i) => J.T(s.W.Tiles[i].Sea);

    /// <summary>TS onSea: ajan şu an deniz karosunda mı</summary>
    public static bool OnSea(Sim s, Agent a) => IsSea(s, Agents.TileOf(a));

    /// <summary>TS inline <c>[9, 2, 0, 1, 1.5, 2][dd]</c> (undefined → NaN beyond index 5).</summary>
    private static readonly double[] PORT_D = { 9, 2, 0, 1, 1.5, 2 };

    /// <summary>yerleşimin tersane kurabileceği kıyı karosu (-1 yok)</summary>
    public static int PickPort(Sim s, Settlement st)
    {
        var w = s.W;
        int best = -1;
        double bd = double.PositiveInfinity;
        foreach (int i in s.G.Within(st.Tile, s.RadiusOf(st)))
        {
            var t = w.Tiles[i];
            if (J.T(t.Sea) || t.Terrain == "water" || t.Terrain == "mountain" || (t.Owner != st.Id && i != st.Tile)) continue;
            if (!J.Some(s.G.Neighbors(i), n => J.T(w.Tiles[n].Sea))) continue;
            // liman surların dışında dursun: iki karo uzak kıyı en iyisi, merkez en son seçenek
            int dd = s.G.Dist(i, st.Tile);
            double d = J.N(J.AtN(PORT_D, dd)) + (t.Ext != null ? 1.5 : 0) + (t.Deposit >= 0 ? 1 : 0);
            if (d < bd) { bd = d; best = i; }
        }
        return best;
    }

    /// <summary>limanın denize açılan komşusu (-1 yok)</summary>
    public static int PortWater(Sim s, int port)
    {
        var ns = J.Filter(s.G.Neighbors(port), n => J.T(s.W.Tiles[n].Sea));
        J.Sort(ns, (a, b) => J.Filter(s.G.Neighbors(b), q => J.T(s.W.Tiles[q].Sea)).Count - J.Filter(s.G.Neighbors(a), q => J.T(s.W.Tiles[q].Sea)).Count);
        return ns.Count > 0 ? ns[0] : -1;
    }

    // ------------------------------------------------------------ filo
    /// <summary>tersanesi ve limanı olan yerleşimler</summary>
    public static List<Settlement> Ports(Sim s, Civ c) => J.Filter(s.CivSettlements(c), x => J.T(x.Civics.Get("shipyard")) && x.Port != null);

    /// <summary>TS isFleet: savaş filosu gemisi mi (purpose 'fleet' ya da 'fleet-back')</summary>
    public static bool IsFleet(Agent a) => a.Kind == "ship" && (a.Purpose == "fleet" || a.Purpose == "fleet-back");

    /// <summary>yerleşimin denizdeki (filo dışı) gemi sayısı</summary>
    public static double HullsOut(Sim s, Settlement st)
    {
        double n = 0;
        foreach (var a in s.W.Agents) if (!J.T(a.Dead) && a.Hull == st.Id && !IsFleet(a)) n++;
        return n;
    }

    /// <summary>yerleşimin denizdeki kadırga sayısı</summary>
    public static double GalleysOut(Sim s, Settlement st)
    {
        double n = 0;
        foreach (var a in s.W.Agents) if (!J.T(a.Dead) && a.Hull == st.Id) n += a.Galleys ?? 0;
        return n;
    }

    /// <summary>limanda boş bekleyen gemi sayısı</summary>
    public static double FreeHulls(Sim s, Settlement st) => JsMath.Max(0, (st.Ships ?? 0) - HullsOut(s, st));

    /// <summary>limanda boş bekleyen kadırga sayısı</summary>
    public static double FreeGalleys(Sim s, Settlement st) => JsMath.Max(0, (st.Galleys ?? 0) - GalleysOut(s, st));

    /// <summary>medeniyetin toplam gemi ve kadırga sayısı (TS <c>{ ships, galleys }</c>)</summary>
    public static (double Ships, double Galleys) Fleet(Sim s, Civ c)
    {
        double ships = 0, galleys = 0;
        foreach (var x in s.CivSettlements(c)) { ships += x.Ships ?? 0; galleys += x.Galleys ?? 0; }
        return (ships, galleys);
    }

    /// <summary>gemi adı: Kasaba kademesine (eskiden Gemicilik) dek kıyı teknesi, sonra koga</summary>
    public static string HullName(Sim s, Civ c, bool plural = false) => s.CivAt(c, Gate.SHIPBUILDING) ? (plural ? "kogalar" : "koga") : (plural ? "tekneler" : "tekne");

    // ------------------------------------------------------------ yol bulma
    /// <summary>JS <c>arr.join(',')</c> for tile indices.</summary>
    private static string JoinInts(List<int> xs)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < xs.Count; i++) { if (i > 0) sb.Append(','); sb.Append(J.S(xs[i])); }
        return sb.ToString();
    }

    /// <summary>
    /// Kara + deniz A*: karada yürünür, yalnız <c>embark</c> karolarından (limanlar) gemiye binilir,
    /// denizde kıyı suyu (Seyir, yani Kasaba kademesiyle açık deniz) izlenir, istenen yerde (ya da <c>landOnly</c>) karaya çıkılır.
    /// s.NavCache ile önbellekli (önbellekteki aynı listeyi döner; bayat girdiler bilerek yeniden kullanılır); null = yol yok.
    /// </summary>
    public static List<int> NavPath(Sim s, int from, int to, NavOpts o)
    {
        // TS: `${from}:${to}:${o.open ? 1 : 0}:${o.embark.join(',')}:${o.landOnly?.join(',') ?? ''}` ([] and undefined give the same key, like TS)
        string key = J.S(from) + ":" + J.S(to) + ":" + (o.Open ? "1" : "0") + ":" + JoinInts(o.Embark) + ":" + (o.LandOnly != null ? JoinInts(o.LandOnly) : "");
        var cache = s.NavCache;
        if (cache.TryGetValue(key, out var hit)) return hit;
        var w = s.W;
        var g = s.G;
        var shore = ShoreWater(s);
        var emb = new HashSet<int>(o.Embark);
        HashSet<int> land = o.LandOnly != null ? new HashSet<int>(o.LandOnly) : null;
        var gScore = new Dictionary<int, double>();
        var came = new Dictionary<int, int>();
        var closed = new HashSet<int>();
        var open = new MinHeap();
        gScore[from] = 0; open.Push(from, g.Dist(from, to) * 0.5);
        bool found = false;
        while (open.Size > 0)
        {
            int cur = open.Pop();
            if (cur == to) { found = true; break; }
            if (closed.Contains(cur)) continue;
            closed.Add(cur);
            bool cs = J.T(w.Tiles[cur].Sea);
            foreach (int n in g.Neighbors(cur))
            {
                bool ns = J.T(w.Tiles[n].Sea);
                double step;
                if (!cs && !ns) step = n == to ? JsMath.Min(s.MoveCost(n), 3) : s.MoveCost(n);
                else if (!cs && ns) { if (!emb.Contains(cur) || (!o.Open && shore[n] == 0)) continue; step = SEA_STEP + EMBARK; }
                else if (cs && ns) { if (!o.Open && shore[n] == 0) continue; step = SEA_STEP; }
                else { if (w.Tiles[n].Terrain == "mountain" || (land != null && !land.Contains(n))) continue; step = JsMath.Min(s.MoveCost(n), 3) + EMBARK; }
                if (!double.IsFinite(step)) continue;
                double t = gScore[cur] + step;
                if (t < (gScore.TryGetValue(n, out double gn) ? gn : double.PositiveInfinity)) { gScore[n] = t; came[n] = cur; open.Push(n, t + g.Dist(n, to) * 0.5); }
            }
        }
        List<int> path = null;
        if (found) { path = new List<int> { to }; int c = to; while (came.TryGetValue(c, out int pc)) { c = pc; path.Add(c); } path.Reverse(); }
        if (cache.Count > 3000) cache.Clear();
        cache[key] = path;
        return path;
    }

    /// <summary>TS hasSea: yolda deniz karosu var mı</summary>
    public static bool HasSea(Sim s, List<int> p) => J.Some(p, t => J.T(s.W.Tiles[t].Sea));

    /// <summary>yolun ilk bindiği liman yerleşimi (null = undefined)</summary>
    public static Settlement EmbarkPort(Sim s, Civ c, List<int> p)
    {
        int k = J.FindIndex(p, x => J.T(s.W.Tiles[x].Sea));
        if (k <= 0) return null;
        int t = p[k - 1];
        return J.Find(Ports(s, c), x => x.Port == t);
    }

    /// <summary>medeniyet için kara/deniz yolu: boş gemisi olan limanlardan binilir (TS <c>o.landOnly</c> düzleştirildi; null = yol yok).
    /// Faz 1b-3: limanı olan her medeniyet denize açılır (eskiden Tekne Yapımı düğümü).</summary>
    public static CivPathResult CivPath(Sim s, Civ c, int from, int to, List<int> landOnly = null)
    {
        var emb = J.Map(J.Filter(Ports(s, c), x => FreeHulls(s, x) > 0), x => x.Port.Value);
        if (emb.Count == 0) { var lp = s.Path(from, to); return lp != null ? new CivPathResult { Path = lp } : null; }
        var p = NavPath(s, from, to, new NavOpts { Embark = emb, Open = OpenSea(s, c), LandOnly = landOnly });
        if (p == null) return null;
        if (!HasSea(s, p)) return new CivPathResult { Path = p };
        var hull = EmbarkPort(s, c, p);
        return hull != null ? new CivPathResult { Path = p, Hull = hull } : null;
    }

    /// <summary>Faz 1b-3: açık denizde yön bulma (eskiden Seyir düğümü): medeniyet Kasaba kademesinde</summary>
    public static bool OpenSea(Sim s, Civ c) => s.CivAt(c, Gate.NAVIGATION);

    // ------------------------------------------------------------ hareket
    /// <summary>denizde günlük ilerleme (karo/gün). Faz 1b-5: fiziksel hızdan (Pace.SHIP: koga ~3 knot, günde 16 saat; korsan kayığı
    /// Pace.PIRATE); eski artılar (açık deniz +0,2, deniz ticareti +0,1, filo +0,05) eski tabanın (0,85) payı olarak.</summary>
    public static double ShipSpeed(Sim s, Agent a)
    {
        var c = a.Civ >= 0 ? s.W.Civs[a.Civ] : null;
        if (a.Monster == "pirate") return Pace.PIRATE; // hafif korsan kayıkları
        double v = 0.85;
        int ct = c != null ? s.CivTier(c) : 0;
        if (c != null && ct >= Gate.NAVIGATION) v += 0.2;
        if (c != null && ct >= Gate.SEATRADE && (a.Kind == "caravan" || a.Kind == "ship")) v += 0.1;
        if (a.Kind == "ship" && !StartsExplore(a.Purpose)) v += 0.05;
        return v * (Pace.SHIP / Pace.SHIP_OLD);
    }

    /// <summary>Faz 1b-5: denizdeki korsan avının (prey) 2 karo yakınında mı (yolda geçerken de durur).</summary>
    public static bool PreyNear(Sim s, Agent a)
    {
        var t = J.Find(s.W.Agents, x => x.Id == a.To && !J.T(x.Dead));
        return t != null && s.G.Dist(Agents.TileOf(a), Agents.TileOf(t)) <= 2;
    }

    /// <summary>denizden karaya adım: gemi limanına döner ya da (ordu) kıyıda bekler</summary>
    public static void Disembark(Sim s, Agent a, int seaTile, int landTile)
    {
        a.Landing = landTile;
        if (a.Kind == "ship") return;
        var home = a.Hull != null ? s.Settlement(a.Hull.Value) : null;
        if (a.Kind == "army" && !J.T(a.Returning)) return; // kogalar kıyıda bekler, dönüşte yeniden binilir
        if (a.Kind == "army" && J.T(a.Returning)) { a.Hull = null; a.Galleys = null; return; }
        a.Hull = null;
        if (home == null || !home.Alive || home.Port == null) return;
        var c = s.W.Civs[a.Civ];
        var p = NavPath(s, seaTile, home.Port.Value, new NavOpts { Embark = new List<int>(), Open = OpenSea(s, c), LandOnly = new List<int> { home.Port.Value } });
        if (p == null || p.Count < 2) return;
        s.W.Agents.Add(new Agent { Id = s.Id(), Kind = "ship", Civ = a.Civ, Path = p, Step = 0, Progress = 0, Speed = Pace.SHIP, Hull = home.Id, Purpose = "return" });
    }

    /// <summary>ordu dönüşü: karaya çıktığı yerden yeniden biner, kendi limanında iner (null = deniz yolu yok)</summary>
    public static List<int> SeaReturnPath(Sim s, Agent a, int to)
    {
        if (a.Hull == null || a.Landing == null) return null;
        var c = s.W.Civs[a.Civ];
        var home = s.Settlement(a.Hull.Value);
        var land = home != null && home.Alive && home.Civ == a.Civ && home.Port != null ? new List<int> { home.Port.Value } : null;
        bool open = OpenSea(s, c);
        return NavPath(s, Agents.TileOf(a), to, new NavOpts { Embark = new List<int> { a.Landing.Value }, Open = open, LandOnly = land })
            ?? NavPath(s, Agents.TileOf(a), to, new NavOpts { Embark = new List<int> { a.Landing.Value }, Open = open });
    }

    // ------------------------------------------------------------ günlük deniz olayları
    /// <summary>günlük deniz olayları: limanlar, fırtına, deniz savaşları, keşif, filolar, korsanlar, yanardağ</summary>
    public static void SeaTick(Sim s)
    {
        var w = s.W;
        if (s.Every(Sim.WORLD_DAYS, 7 / Sim.PACE))
        {
            var sts = w.Settlements;
            for (int k = 0; k < sts.Count; k++)
            {
                var st = sts[k];
                if (!st.Alive || !J.T(st.Civics.Get("shipyard"))) continue;
                bool ok = st.Port != null && (w.Tiles[st.Port.Value].Owner == st.Id || st.Port.Value == st.Tile) && J.Some(s.G.Neighbors(st.Port.Value), n => J.T(w.Tiles[n].Sea));
                if (!ok) { int p = PickPort(s, st); st.Port = p >= 0 ? p : (int?)null; }
            }
        }
        // JS for...of: the iterator keeps the array it started with and re-reads its length
        var agents = w.Agents;
        for (int i = 0; i < agents.Count; i++)
        {
            var a = agents[i];
            if (J.T(a.Dead) || !OnSea(s, a)) continue;
            if (Storm(s, a)) continue;
            if ((a.Kind == "army" && !J.T(a.Returning) && (a.Purpose == "war" || a.Purpose == "plunder")) || (a.Kind == "ship" && a.Purpose == "fleet")) NavalEncounter(s, a);
            if (!J.T(a.Dead) && IsFleet(a)) Intercept(s, a);
            if (!J.T(a.Dead) && a.Kind == "raid" && a.Monster == "pirate" && !J.T(a.Returning)) PirateSea(s, a);
        }
        // Faz 1b-5: eski 10 günlük kararlar 2,5 günde; eski yıllıklar eski yılın karşılığında (OLD_YEAR)
        if (s.Every(10 / Sim.PACE, 3 / Sim.PACE)) for (int k = 0; k < w.Civs.Count; k++) { var c = w.Civs[k]; if (c.Alive) ConsiderSeaExplore(s, c); }
        if (s.Every(10 / Sim.PACE, 6 / Sim.PACE)) for (int k = 0; k < w.Civs.Count; k++) { var c = w.Civs[k]; if (c.Alive) ConsiderFleet(s, c); }
        if (s.Every(10 / Sim.PACE, 8 / Sim.PACE)) for (int k = 0; k < w.Civs.Count; k++) { var c = w.Civs[k]; if (c.Alive) ConsiderPirateHunt(s, c); }
        if (s.Every(Sim.OLD_YEAR, 60 / Sim.PACE)) PirateCoveTick(s);
        if (s.Every(Sim.OLD_YEAR, 90 / Sim.PACE)) VolcanoTick(s);
    }

    /// <summary>yanardağ adası: arada bir kül püskürür; adadaki koloninin evleri ve tarlaları zarar görür</summary>
    private static void VolcanoTick(Sim s)
    {
        var isles = s.W.Isles ?? new List<IsleInfo>();
        for (int k = 0; k < isles.Count; k++)
        {
            var isl = isles[k];
            if (isl.Kind != "volkan" || isl.Peak == null) continue;
            var st = J.Find(s.W.Settlements, x => x.Alive && s.W.Tiles[x.Tile].Isle == isl.Id);
            if (st == null || !s.Rng.Chance(0.06)) continue;
            double burnt = s.Rng.Int(1, 2);
            st.BurnedHouses = (st.BurnedHouses ?? 0) + burnt; st.BurnedAt = s.Day;
            int fields = 0;
            foreach (int i in s.G.Within(st.Tile, s.RadiusOf(st))) { var t = s.W.Tiles[i]; if (t.Ext != null && t.Owner == st.Id && t.Ext.Kind == "farm" && s.ExtWorking(t) && fields < 2) { t.Ext.Burned = s.Day + 60 / Sim.PACE; t.Ext.BurnedAt = s.Day; t.Ext.Workers = 0; fields++; } }
            s.Metric("eruption");
            s.Log("world", $"{Tr.Ek(isl.Name, "da")}ki yanardağ kül püskürdü: {Tr.Ek(st.Name, "da")} {J.S(burnt)} ev yandı{(fields != 0 ? $", {J.S(fields)} tarla küle gömüldü" : "")}.", civ: st.Civ, tile: isl.Peak, major: true, cause: "Volkanik ada; zengin topraklar, huysuz dağ");
        }
    }

    /// <summary>Faz 1b-3: mevsim yok; fırtına olasılığı eski mevsim çarpanlarının yıllık ortalaması ((1 + 0,7 + 1,5 + 2,6) / 4)</summary>
    public const double STORM_AVG = 1.45;

    private static bool Storm(Sim s, Agent a)
    {
        var w = s.W;
        int here = Agents.TileOf(a);
        var c = a.Civ >= 0 ? w.Civs[a.Civ] : null;
        if (c == null) return false; // korsanlar suları bilir
        // Faz 1b-5: fırtına riski denizde alınan yolla: eski günlük olasılık eski hızla (~1 karo/gün) bir karonun riskiydi; yeni günde gemi
        // hızı oranında (sefer başına risk aynı)
        double p = 0.00012 * (ShoreWater(s)[here] != 0 ? 0.6 : 1.8) * STORM_AVG * (Pace.SHIP / Pace.SHIP_OLD);
        if (c != null && OpenSea(s, c)) p *= 0.65;
        if (c != null && J.Some(s.CivSettlements(c), x => J.T(x.Civics.Get("lighthouse")) && x.Port != null && s.G.Dist(x.Port.Value, here) <= 8)) p *= 0.4;
        if (!s.Rng.Chance(p)) return false;
        string who = c != null ? c.Name : "";
        s.Metric("storm");
        if (a.Kind == "army")
        {
            double lost = JsMath.Max(1, Math.Floor((a.Troops ?? 0) * s.Rng.Int(10, 30) / 100));
            a.Troops = JsMath.Max(0, (a.Troops ?? 0) - lost); Agents.RemoveFromPop(s, a.Pop, lost);
            int gl = 0;
            if ((a.Galleys ?? 0) > 0 && s.Rng.Chance(0.4)) { gl = 1; a.Galleys = a.Galleys.Value - 1; var h = a.Hull != null ? s.Settlement(a.Hull.Value) : null; if (h != null) h.Galleys = JsMath.Max(0, (h.Galleys ?? 0) - 1); }
            s.Log("sea", $"Fırtına {(c != null ? Tr.Ek(who, "in") : "bir")} donanmasını savurdu: {J.S(lost)} asker denize düştü{(gl != 0 ? ", bir kadırga battı" : "")}.", civ: a.Civ, tile: here, major: true, cause: ShoreWater(s)[here] != 0 ? "Kıyıda ani bir fırtına" : "Açık denizde fırtına");
            return false;
        }
        // tekil gemiler: çoğu hasarla kurtulur, beşte biri batar
        if (s.Rng.Chance(0.8))
        {
            a.Progress = JsMath.Min(a.Progress, 0) - 0.5;   // Faz 1b-5: yarım gün yitirir (eskiden 2 eski gün)
            if (a.Kind != "ship" && c != null) s.Log("sea", $"{Tr.Ek(who, "in")} gemisi fırtınaya yakalandı; direği kırıldı ama batmadı.", civ: a.Civ, tile: here);
            return false;
        }
        var home = a.Hull != null ? s.Settlement(a.Hull.Value) : null;
        if (home != null) home.Ships = JsMath.Max(0, (home.Ships ?? 0) - 1);
        a.Dead = true;
        s.Metric("shipSunk");
        string what = a.Kind == "settlers" ? $"{J.S(s.PopSize(a.Pop))} öncüyü taşıyan" : a.Kind == "caravan" ? "yüklü" : a.Purpose == "explore" || a.Purpose == "explore-back" ? "keşif" : "boş dönen";
        // TS `a.kind === 'settlers' || a.kind === 'caravan' || a.purpose?.startsWith('explore')`: true, false or undefined (no purpose)
        bool? major = a.Kind == "settlers" || a.Kind == "caravan" ? true : a.Purpose?.StartsWith("explore", StringComparison.Ordinal);
        s.Log("sea", $"{(c != null ? Tr.Ek(who, "in") : "Bir")} {what} gemisi fırtınada battı.", civ: a.Civ, tile: here, major: major, cause: $"Deniz fırtınası{(ShoreWater(s)[here] != 0 ? "" : ", açık denizde")}");
        return true;
    }

    /// <summary>saldıran donanma düşman limanına yaklaşınca kadırgalar çıkar</summary>
    private static void NavalEncounter(Sim s, Agent a)
    {
        var w = s.W;
        int here = Agents.TileOf(a);
        var att = w.Civs[a.Civ];
        var sts = w.Settlements;
        for (int k = 0; k < sts.Count; k++)
        {
            var st = sts[k];
            if (!st.Alive || st.Civ == a.Civ || st.Port == null || !J.T(st.Galleys ?? 0)) continue;
            if (a.Fought != null && a.Fought.Contains(st.Id)) continue;
            if (!s.AtWar(a.Civ, st.Civ) && !(a.Purpose == "plunder" && st.Id == a.To)) continue;
            if (s.G.Dist(st.Port.Value, here) > 3) continue;
            double free = FreeGalleys(s, st);
            if (!J.T(free)) continue;
            (a.Fought ??= new List<int>()).Add(st.Id);
            var dfc = w.Civs[st.Civ];
            bool fleetOnly = a.Kind == "ship";
            var A = new List<Combatant>();
            for (int i = 0; i < (a.Galleys ?? 0); i++) A.Add(Combat.Unit(GALLEY, "A", "galley"));
            if (!fleetOnly) { A.Add(Combat.Unit(COG, "A", "cog")); A.AddRange(Agents.CivTroops(s, att, JsMath.Min(6, a.Troops ?? 0), "A")); }
            var B = new List<Combatant>();
            for (int i = 0; i < JsMath.Min(4, free); i++) B.Add(Combat.Unit(GALLEY, "B", "galley"));
            B.AddRange(Agents.CivTroops(s, dfc, JsMath.Min(3, st.Soldiers), "B"));
            var b = Combat.ResolveBattle(s.Rng, A, B, new BattleOpts { Id = s.Id(), Day = s.Day, Tile = here, Title = $"{st.Name} açıklarında deniz savaşı", SideA = $"{att.Name} donanması", SideB = $"{st.Name} kadırgaları", MoraleA = 0.5, MoraleB = 0.55, MaxRounds = 12, CivA = att.Id, CivB = dfc.Id });
            b.Naval = true;
            Agents.RecordBattle(s, b);
            s.Metric("navalBattle");
            int deadG = J.Filter(A, x => x.Kind == "galley" && x.Hp <= 0).Count;
            bool cogSunk = J.Some(A, x => x.Kind == "cog" && x.Hp <= 0);
            int deadM = J.Filter(A, x => (x.Kind == "soldier" || x.Kind == "unique") && x.Hp <= 0).Count;
            int deadBG = J.Filter(B, x => x.Kind == "galley" && x.Hp <= 0).Count;
            int deadBM = J.Filter(B, x => (x.Kind == "soldier" || x.Kind == "unique") && x.Hp <= 0).Count;
            var home = a.Hull != null ? s.Settlement(a.Hull.Value) : null;
            if (home != null) home.Galleys = JsMath.Max(0, (home.Galleys ?? 0) - deadG);
            a.Galleys = JsMath.Max(0, (a.Galleys ?? 0) - deadG);
            double drowned = deadM;
            if (cogSunk) { drowned += Math.Floor(((a.Troops ?? 0) - deadM) * 0.5); if (home != null) home.Ships = JsMath.Max(0, (home.Ships ?? 0) - 1); }
            a.Troops = JsMath.Max(0, (a.Troops ?? 0) - drowned); Agents.RemoveFromPop(s, a.Pop, drowned);
            st.Galleys = JsMath.Max(0, (st.Galleys ?? 0) - deadBG);
            if (deadBM != 0) { st.Soldiers = JsMath.Max(0, st.Soldiers - deadBM); s.RemovePop(st, deadBM); }
            if (b.Winner == "A")
            {
                att.Stats.BattlesWon++; dfc.Stats.BattlesLost++;
                s.Log("sea", $"{att.Name} donanması {st.Name} açıklarında {dfc.Name} kadırgalarını yendi{(deadBG != 0 ? $": {J.S(deadBG)} kadırga battı" : "")}.", civ: att.Id, tile: here, battle: b.Id, major: true, cause: fleetOnly ? $"{J.S(a.Galleys ?? 0)} kadırga limana doğru ilerliyor" : $"{J.S(a.Galleys ?? 0)} kadırga, {J.S(J.N(a.Troops))} asker karaya çıkmaya devam ediyor");
            }
            else
            {
                att.Stats.BattlesLost++; dfc.Stats.BattlesWon++;
                s.Log("sea", $"{st.Name} kadırgaları {att.Name} donanmasını geri püskürttü{(cogSunk ? "; bir asker kogası battı" : "")}.", civ: dfc.Id, tile: here, battle: b.Id, major: true, cause: fleetOnly ? $"{J.S(deadG)} kadırga kayıp" : $"{J.S(drowned)} asker ve {J.S(deadG)} kadırga kayıp");
                if (fleetOnly) { if (!J.T(a.Galleys)) a.Dead = true; else FleetHome(s, a); } else Agents.ArmyReturn(s, a);
                return;
            }
            if (fleetOnly) { if (!J.T(a.Galleys)) a.Dead = true; continue; }
            if ((a.Troops ?? 0) <= 0 && !(a.Heroes != null && a.Heroes.Count > 0)) { a.Dead = true; return; }
        }
    }

    // ------------------------------------------------------------ keşif gemisi
    /// <summary>Kasaba kademesiyle (eskiden Seyir) keşif gemisi gönderme kararı</summary>
    public static void ConsiderSeaExplore(Sim s, Civ c)
    {
        if (J.T(c.SeaScout) || !OpenSea(s, c)) return;
        var port = J.Find(Ports(s, c), x => FreeHulls(s, x) > 0);
        if (port == null) return;
        var w = s.W;
        var g = s.G;
        int start = PortWater(s, port.Port.Value);
        if (start < 0) return;
        // açık denizde en uzak, adalara ve bilinmeyen kıyılara yakın bir noktaya
        var dist = new Dictionary<int, int> { [start] = 0 };
        var par = new Dictionary<int, int>();
        var q = new List<int> { start };
        for (int h = 0; h < q.Count; h++)
        {
            int i = q[h], d = dist[i];
            if (d >= 42) continue;
            foreach (int n in g.Neighbors(i)) if (J.T(w.Tiles[n].Sea) && !dist.ContainsKey(n)) { dist[n] = d + 1; par[n] = i; q.Add(n); }
        }
        var own = s.CivSettlements(c);
        int goal = -1;
        double bs = double.NegativeInfinity;
        // TS iterates the Map `dist` (insertion order): every key was inserted together with its push onto q, so q is that order
        for (int h = 0; h < q.Count; h++)
        {
            int i = q[h], d = dist[i];
            if (d < 14) continue;
            double sc = JsMath.Min(d, 34) + s.Rng.Next() * 4;
            foreach (int n in g.Within(i, 3)) { var t = w.Tiles[n]; if (J.T(t.Isle) && !J.Some(own, x => x.Tile == n)) sc += 1.2; }
            if (J.Some(own, x => g.Dist(x.Tile, i) < 8)) sc -= 10;
            if (sc > bs) { bs = sc; goal = i; }
        }
        if (goal < 0) return;
        var path = new List<int> { goal };
        int cur = goal; while (par.TryGetValue(cur, out int pc)) { cur = pc; path.Add(cur); }
        path.Add(port.Port.Value);
        path.Reverse();
        c.SeaScout = true;
        w.Agents.Add(new Agent { Id = s.Id(), Kind = "ship", Civ = c.Id, Path = path, Step = 0, Progress = 0, Speed = Pace.SHIP, Hull = port.Id, Purpose = "explore" });
        s.Metric("seaExplore");
        s.Log("sea", $"{c.Name} denizcileri {Tr.Ek(port.Name, "dan")} ufkun ötesine yelken açtı.", civ: c.Id, tile: port.Port, major: true, cause: "Açık denizde yıldızlarla yön bulmak");
    }

    /// <summary>keşif gemisinin gözü: kıyılar, adalar, yataklar, uzak halklar</summary>
    public static void ExploreSight(Sim s, Agent a)
    {
        Agents.ScoutSight(s, a);
        var c = s.W.Civs[a.Civ];
        int here = Agents.TileOf(a);
        foreach (int n in s.G.Within(here, 4))
        {
            int? id = s.W.Tiles[n].Isle;
            if (!J.T(id) || J.T(c.Yearly.Get("isle" + J.S(id.Value)))) continue;
            c.Yearly.Set("isle" + J.S(id.Value), 1);
            var isl = s.W.Isles != null ? J.Find(s.W.Isles, x => x.Id == id) : null;
            double size = isl != null ? isl.Size : J.Filter(s.W.Tiles, t => t.Isle == id).Count;
            s.Log("sea", isl != null ? $"{Tr.Ek(c.Name, "in")} keşif gemisi {Tr.Ek(isl.Name, "i")} gördü: {(WorldGen.ISLE_TR.TryGet(isl.Kind, out var ktr) ? ktr : "undefined")}, {J.S(size)} karo." : $"{c.Name} keşif gemisi uzak bir ada gördü ({J.S(size)} karo).", civ: c.Id, tile: n, major: size >= 10);
            s.Metric("isleSeen");
        }
    }

    /// <summary>keşif gemisi hedefe varınca limana döner (true = ajan biter)</summary>
    public static bool ExploreTurn(Sim s, Agent a)
    {
        var home = a.Hull != null ? s.Settlement(a.Hull.Value) : null;
        if (home == null || !home.Alive || home.Port == null) return true;
        var c = s.W.Civs[a.Civ];
        var p = NavPath(s, Agents.TileOf(a), home.Port.Value, new NavOpts { Embark = new List<int>(), Open = OpenSea(s, c), LandOnly = new List<int> { home.Port.Value } });
        if (p == null) return true;
        a.Path = p; a.Step = 0; a.Progress = 0; a.Purpose = "explore-back";
        return false;
    }


    // ------------------------------------------------------------ savaş filoları
    /// <summary>savaşta kadırgalar düşman limanına akın eder: limandaki gemileri yakar, denizdeki gemilerini ele geçirir</summary>
    public static void ConsiderFleet(Sim s, Civ c)
    {
        if (!s.CivAt(c, Gate.NAVY) || !s.InWar(c)) return;
        if (J.Some(s.W.Agents, a => a.Civ == c.Id && IsFleet(a))) return;
        var @base = J.At(J.Sort(J.Filter(Ports(s, c), x => FreeGalleys(s, x) >= 2), (a, b) => FreeGalleys(s, b) - FreeGalleys(s, a)), 0);
        if (@base == null) return;
        var w = s.W;
        Settlement best = null;
        double bd = double.PositiveInfinity;
        for (int k = 0; k < w.Civs.Count; k++)
        {
            var o = w.Civs[k];
            if (o.Id == c.Id || !o.Alive || !s.AtWar(c.Id, o.Id)) continue;
            if (s.Day - (c.Yearly.Get("fleet" + J.S(o.Id)) ?? -9999) < 200 / Sim.PACE) continue;
            foreach (var st in Ports(s, o)) { int d = s.G.Dist(st.Port.Value, @base.Port.Value); if (d < bd && d <= (OpenSea(s, c) ? 50 : 30)) { bd = d; best = st; } }
        }
        if (best == null) return;
        int goal = PortWater(s, best.Port.Value);
        if (goal < 0) return;
        var path = NavPath(s, @base.Port.Value, goal, new NavOpts { Embark = new List<int> { @base.Port.Value }, Open = OpenSea(s, c) });
        if (path == null || path.Count < 3) return;
        var oc = w.Civs[best.Civ];
        c.Yearly.Set("fleet" + J.S(oc.Id), s.Day);
        double n = JsMath.Min(4, FreeGalleys(s, @base));
        w.Agents.Add(new Agent { Id = s.Id(), Kind = "ship", Civ = c.Id, Path = path, Step = 0, Progress = 0, Speed = Pace.SHIP, Hull = @base.Id, Galleys = n, Purpose = "fleet", To = best.Id });
        s.Metric("fleetSortie");
        s.Log("sea", $"{c.Name} {J.S(n)} kadırgayla {Tr.Ek(@base.Name, "dan")} {Tr.Ek(best.Name, "in")} limanına akına çıktı.", civ: c.Id, tile: @base.Port, major: true, cause: $"{oc.Name} ile savaş");
    }

    /// <summary>filo hedef limana vardı: savunan kadırga kalmadıysa limandaki gemileri yakar (true = ajan biter)</summary>
    public static bool FleetArrive(Sim s, Agent a)
    {
        if (a.Purpose == "fleet-back") return true;
        var st = a.To != null ? s.Settlement(a.To.Value) : null;
        var att = s.W.Civs[a.Civ];
        if (st != null && st.Alive && st.Civ != a.Civ && s.AtWar(a.Civ, st.Civ))
        {
            var dfc = s.W.Civs[st.Civ];
            double burn = JsMath.Min(FreeHulls(s, st), 1 + Math.Floor((a.Galleys ?? 1) / 2));
            double gold = JsMath.Min(s.St(dfc, "gold"), 8 + (a.Galleys ?? 1) * 6);
            if (J.T(burn)) st.Ships = JsMath.Max(0, (st.Ships ?? 0) - burn);
            s.Add(dfc, "gold", -gold); s.Add(att, "gold", gold);
            s.Metric("portRaid");
            s.Log("sea", $"{att.Name} kadırgaları {Tr.Ek(st.Name, "in")} limanını ateşe verdi{(J.T(burn) ? $": {J.S(burn)} gemi yandı" : "")}{(gold >= 1 ? $", {J.S(JsMath.Round(gold))} altınlık ganimet" : "")}.", civ: att.Id, tile: st.Port ?? st.Tile, major: true, cause: "Deniz akını");
            if (st.Port != null) { st.BurnedHouses = (st.BurnedHouses ?? 0) + 1; st.BurnedAt = s.Day; }
        }
        return FleetHome(s, a);
    }

    private static bool FleetHome(Sim s, Agent a)
    {
        var home = a.Hull != null ? s.Settlement(a.Hull.Value) : null;
        if (home == null || !home.Alive || home.Civ != a.Civ || home.Port == null) { a.Dead = true; return true; }
        var p = NavPath(s, Agents.TileOf(a), home.Port.Value, new NavOpts { Embark = new List<int>(), Open = OpenSea(s, s.W.Civs[a.Civ]), LandOnly = new List<int> { home.Port.Value } });
        if (p == null) { a.Dead = true; return true; }
        a.Path = p; a.Step = 0; a.Progress = 0; a.Purpose = "fleet-back";
        return false;
    }

    /// <summary>denizdeki düşman gemilerini ele geçirme</summary>
    private static void Intercept(Sim s, Agent a)
    {
        var w = s.W;
        int here = Agents.TileOf(a);
        var att = w.Civs[a.Civ];
        var agents = w.Agents;
        for (int k = 0; k < agents.Count; k++)
        {
            var b = agents[k];
            if (J.T(b.Dead) || b == a || b.Civ < 0 || b.Civ == a.Civ || !s.AtWar(a.Civ, b.Civ)) continue;
            if (!(b.Kind == "caravan" || b.Kind == "settlers" || (b.Kind == "ship" && !IsFleet(b)))) continue;
            int bt = Agents.TileOf(b);
            if (!J.T(w.Tiles[bt].Sea) || s.G.Dist(bt, here) > 2) continue;
            var owner = w.Civs[b.Civ];
            var home = b.Hull != null ? s.Settlement(b.Hull.Value) : null;
            if (home != null) home.Ships = JsMath.Max(0, (home.Ships ?? 0) - 1);
            var hb = a.Hull != null ? s.Settlement(a.Hull.Value) : null;
            if (hb != null && hb.Alive) hb.Ships = (hb.Ships ?? 0) + 1; // ganimet gemi
            string loot = "";
            if (b.Cargo != null) foreach (var kv in b.Cargo) { s.Add(att, kv.Key, kv.Value); loot = " ve yükü"; }
            b.Dead = true;
            s.Metric("shipCaptured");
            s.Log("sea", $"{att.Name} kadırgaları {Tr.Ek(owner.Name, "in")} {(b.Kind == "settlers" ? "öncü gemisini" : "gemisini")}{loot} ele geçirdi.", civ: att.Id, tile: bt, major: true, cause: b.Kind == "settlers" ? $"{J.S(s.PopSize(b.Pop))} öncü esir düştü" : "Savaşta deniz yolları kesildi");
            if (b.Kind == "settlers" && hb != null) s.MergePop(hb, b.Pop);
        }
    }

    // ------------------------------------------------------------ adalar
    /// <summary>karonun adası (null = anakara / undefined)</summary>
    public static IsleInfo IsleOf(Sim s, int tile)
    {
        int? id = s.W.Tiles[tile].Isle;
        return J.T(id) ? (s.W.Isles != null ? J.Find(s.W.Isles, x => x.Id == id) : null) : null;
    }

    // ------------------------------------------------------------ korsanlar
    private static readonly List<string> COVE_NAMES = new() { "Kara Bayrak Koyu", "Kanlı Çapa Koyu", "Tuzlu Kurt Koyu", "Kemik Sandık Koyu", "Martı Mezarı Koyu", "Paslı Kanca Koyu", "Ölü Rüzgâr Koyu", "Kör Korsan Koyu", "Yırtık Yelken Koyu", "Rom Fıçısı Koyu" };
    private static readonly List<string> CAPTAINS = new() { "Tuzlusakal", "Tek Göz Rıza", "Kanca Elli Mira", "Kızıl Bayrak Dursun", "Kara Nesrin", "Martı Hasan", "Çapa Kerim", "Fırtına Leyla", "Paslı Tunç", "Yarım Ay Selim" };

    /// <summary>TS isPirate: korsan kayığı mı (raid + monster 'pirate')</summary>
    public static bool IsPirate(Agent a) => a.Kind == "raid" && a.Monster == "pirate";

    /// <summary>yaşayan koylarda kullanılmayan bir korsan kaptan adı</summary>
    public static string NewCaptain(Sim s) => s.Rng.Pick(J.Filter(CAPTAINS, x => !J.Some(s.W.Camps, c => c.Alive && c.Captain == x))) ?? CAPTAINS[0];

    /// <summary>korsanlar tüccar cumhuriyetlerinin gemilerine dokunmaz: ganimeti onların limanlarında satarlar (Faz 1b-6; eski Haydut)</summary>
    private static bool Smugglers(Civ c) => Polity.Smugglers(c);

    /// <summary>Deniz ticareti canlanınca sahipsiz adalarda (yoksa ıssız kıyıda) korsan koyu kurulur</summary>
    private static void PirateCoveTick(Sim s)
    {
        var w = s.W;
        if (s.DynYear < 5) return;
        double ships = 0; foreach (var c in w.Civs) if (c.Alive) ships += Fleet(s, c).Ships;
        if (ships < 3) return;
        var coves = J.Filter(w.Camps, c => c.Alive && c.Kind == "pirate");
        // Math.max(-9999, ...cleared)
        var clearedDays = J.Map(J.Filter(w.Camps, c => c.Kind == "pirate" && !c.Alive), c => c.ClearedDay ?? c.Founded);
        clearedDays.Insert(0, -9999);
        double lastClear = JsMath.Max(clearedDays.ToArray());
        if (s.Day - lastClear < 2 * Sim.OLD_YEAR) return;
        int seaRoutes = J.Filter(w.Routes, r => r.Alive && J.T(r.Sea)).Count;
        int max = (w.SeaProfile == "kita" ? 1 : 2) + (seaRoutes >= 6 ? 1 : 0);
        if (coves.Count >= max || !s.Rng.Chance(0.4)) return;
        var setl = J.Filter(w.Settlements, x => x.Alive);
        var allPorts = J.Filter(setl, x => J.T(x.Civics.Get("shipyard")) && x.Port != null);
        if (allPorts.Count == 0) return;
        int best = -1;
        double bs = double.NegativeInfinity;
        for (int i = 0; i < w.Tiles.Count; i++)
        {
            var t = w.Tiles[i];
            if (J.T(t.Sea) || t.Terrain == "water" || t.Terrain == "mountain" || t.Owner >= 0 || t.Camp != null || t.InnZone != null) continue;
            if (!J.Some(s.G.Neighbors(i), n => J.T(w.Tiles[n].Sea))) continue;
            double dS = setl.Count > 0 ? J.MinOf(setl, x => s.G.Dist(x.Tile, i)) : 99;
            if (dS < (J.T(t.Isle) ? 7 : 11)) continue;
            double dP = J.MinOf(allPorts, x => s.G.Dist(x.Port.Value, i));
            if (dP > 34) continue;
            if (J.Some(w.Inns, inn => s.G.Dist(inn.Tile, i) < 6)) continue;
            var isle = J.T(t.Isle) ? (w.Isles != null ? J.Find(w.Isles, x => x.Id == t.Isle) : null) : null;
            double sc = (J.T(t.Isle) ? 6 : 0) + (isle?.Kind == "kumsal" ? 3 : 0) - Math.Abs(dP - 16) * 0.3 + s.Rng.Next() * 3 + (t.Deposit >= 0 ? -1 : 0);
            if (J.Some(w.Camps, c => c.Alive && s.G.Dist(c.Tile, i) < (c.Kind == "pirate" ? 12 : 6))) continue;
            if (sc > bs) { bs = sc; best = i; }
        }
        if (best < 0) return;
        var used = new HashSet<string>(J.Map(w.Camps, c => c.Name));
        string name = J.Find(COVE_NAMES, n => !used.Contains(n)) ?? $"{COVE_NAMES[0]} {J.S(coves.Count + 2)}";
        var cp = WorldGen.MakeCamp(s.Id(), "pirate", best, name, s.Day, s.Rng);
        cp.Count = 7; cp.Boss = true; cp.HadBoss = true; cp.Loot = 20;
        cp.Captain = NewCaptain(s);
        cp.NextRaid = s.Day + s.Rng.Int(15, 30);   // Faz 1b-5: eski 60–120 gün
        w.Camps.Add(cp);
        w.Tiles[best].Camp = cp.Id; w.Tiles[best].Owner = -1;
        var bestIsle = IsleOf(s, best);
        s.Metric("pirateCove");
        s.Log("lair", $"{(bestIsle != null ? $"{Tr.Ek(bestIsle.Name, "da")} " : "Issız bir kıyıda ")}korsanlar {Tr.Ek(name, "i")} kurdu; başlarında Kaptan {cp.Captain}.", tile: best, major: true, cause: $"Deniz ticareti yağmacıları çekti ({J.S(ships)} gemi, {J.S(seaRoutes)} deniz yolu)");
    }

    /// <summary>TS inline <c>{ tile: number; st: Settlement; ext: boolean }</c> (launchPirates' coastal targets).</summary>
    private sealed class CoastSpot
    {
        public int Tile;
        public Settlement St;
        public bool Ext;
    }

    /// <summary>korsan kayığı denize açılır: denizdeki sivil gemiyi avlar ya da iskele/kıyı kasabasına çıkar</summary>
    public static void LaunchPirates(Sim s, Camp cp)
    {
        var w = s.W;
        cp.NextRaid = s.Day + s.Rng.Int(20, 38);   // Faz 1b-5: eski 80–150 gün
        double n = JsMath.Min(Math.Ceiling(cp.Count * 0.6), 3 + Math.Floor(s.DynYear / 3.0), 9);
        var prey = J.Filter(w.Agents, a => !J.T(a.Dead) && a.Civ >= 0 && (a.Kind == "caravan" || a.Kind == "settlers" || (a.Kind == "ship" && !IsFleet(a)))
            && OnSea(s, a) && !Smugglers(w.Civs[a.Civ]) && s.G.Dist(Agents.TileOf(a), cp.Tile) <= 30 && a.Path.Count - a.Step > 4);
        var coast = new List<CoastSpot>();
        foreach (var st in w.Settlements)
        {
            if (!st.Alive || Smugglers(w.Civs[st.Civ]) || s.G.Dist(st.Tile, cp.Tile) > 26) continue;
            foreach (int i in s.G.Within(st.Tile, s.RadiusOf(st)))
            {
                var t = w.Tiles[i];
                if (t.Owner != st.Id || !J.Some(s.G.Neighbors(i), q => J.T(w.Tiles[q].Sea))) continue;
                if (t.Ext?.Kind == "dock" && s.ExtWorking(t)) coast.Add(new CoastSpot { Tile = i, St = st, Ext = true });
            }
            if (J.Some(s.G.Within(st.Tile, 1), q => J.T(w.Tiles[q].Sea)) && s.Pop(st) >= 10) coast.Add(new CoastSpot { Tile = st.Tile, St = st, Ext = false });
        }
        List<int> path = null;
        string purpose = "";
        int to = -1, targetTile = -1;
        if (prey.Count > 0 && (s.Rng.Chance(0.6) || coast.Count == 0))
        {
            var p = s.Rng.Weighted(prey, a => (double)(a.Kind == "caravan" ? 3 : a.Kind == "settlers" ? 2 : 1) / (1 + (a.Galleys ?? 0) * 3));
            int goal = AheadOf(s, p, (int)Math.Ceiling(s.G.Dist(cp.Tile, Agents.TileOf(p)) * 0.9) + 2);
            if (goal >= 0) { path = NavPath(s, cp.Tile, goal, new NavOpts { Embark = new List<int> { cp.Tile }, Open = true }); purpose = "prey"; to = p.Id; targetTile = goal; }
        }
        else if (coast.Count > 0)
        {
            var c = s.Rng.Weighted(coast, x => (x.Ext ? 2.0 : 1.0) / (1 + x.St.Soldiers * 0.6 + (x.St.Galleys ?? 0) * 2) / (1 + s.G.Dist(x.Tile, cp.Tile) * 0.08));
            path = NavPath(s, cp.Tile, c.Tile, new NavOpts { Embark = new List<int> { cp.Tile }, Open = true });
            purpose = c.Ext ? "ext" : "settlement"; to = c.Ext ? c.Tile : c.St.Id; targetTile = c.Tile;
        }
        if (path == null || !HasSea(s, path)) return;
        cp.Count -= n;
        bool boss = cp.Boss && s.Rng.Chance(0.35);
        if (boss) cp.Boss = false;
        w.Agents.Add(new Agent { Id = s.Id(), Kind = "raid", Civ = -1, Path = path, Step = 0, Progress = 0, Speed = Pace.RAID, Troops = n, From = cp.Id, To = to, Purpose = purpose, Boss = boss, Monster = "pirate", TargetTile = targetTile });
        s.Metric("pirateSortie");
    }

    /// <summary>avın rotasında n adım ötedeki deniz karosu (karşısına çıkmak için)</summary>
    private static int AheadOf(Sim s, Agent t, int n)
    {
        for (int k = Math.Min(t.Path.Count - 1, t.Step + n); k >= t.Step; k--) if (J.T(s.W.Tiles[t.Path[k]].Sea)) return t.Path[k];
        return -1;
    }

    /// <summary>denizdeki korsan: avını kovalar, yakalayınca saldırır; kıyıya giderken kadırgalara takılabilir</summary>
    private static void PirateSea(Sim s, Agent a)
    {
        var w = s.W;
        int here = Agents.TileOf(a);
        var cp = J.Find(w.Camps, x => x.Id == a.From);
        if (a.Purpose == "prey")
        {
            var t = J.Find(w.Agents, x => x.Id == a.To && !J.T(x.Dead));
            if (t == null || !OnSea(s, t)) { Agents.RaidReturn(s, a); return; }
            a.Chase ??= s.Day;
            if (s.Day - a.Chase.Value > 35 / Sim.PACE) { Agents.RaidReturn(s, a); return; }
            if (s.G.Dist(here, Agents.TileOf(t)) <= 2) { PirateAttack(s, a, t, cp); return; }
            // Faz 1b-5: her gün (eskiden 3 eski günde bir) avın önüne
            {
                int goal = AheadOf(s, t, (int)Math.Ceiling(s.G.Dist(here, Agents.TileOf(t)) * 0.8) + 1);
                var p = goal >= 0 ? NavPath(s, here, goal, new NavOpts { Embark = new List<int>(), Open = true }) : null;
                if (p != null && p.Count > 1) { a.Path = p; a.Step = 0; a.Progress = 0; }
            }
            return;
        }
        // kıyı baskını: hedefin limanında boş kadırga varsa önce denizde karşılanır
        var st = a.Purpose == "settlement" ? s.Settlement(a.To) : a.TargetTile != null && w.Tiles[a.TargetTile.Value].Owner >= 0 ? s.Settlement(w.Tiles[a.TargetTile.Value].Owner) : null;
        if (st == null || !st.Alive) return;
        foreach (var port in s.CivSettlements(st.Civ))
        {
            if (port.Port == null || (a.Fought != null && a.Fought.Contains(port.Id)) || s.G.Dist(port.Port.Value, here) > 3) continue;
            double free = FreeGalleys(s, port);
            if (!J.T(free)) continue;
            (a.Fought ??= new List<int>()).Add(port.Id);
            var c = w.Civs[port.Civ];
            var A = new List<Combatant>();
            for (int i = 0; i < JsMath.Min(3, free); i++) A.Add(Combat.Unit(GALLEY, "A", "galley"));
            A.AddRange(Agents.CivTroops(s, c, JsMath.Min(2, port.Soldiers), "A"));
            var B = Monsters.MonsterSide("pirate", a.Troops ?? 0, J.T(a.Boss), "B");
            var b = Combat.ResolveBattle(s.Rng, A, B, new BattleOpts { Id = s.Id(), Day = s.Day, Tile = here, Title = $"{port.Name} açıklarında korsan savaşı", SideA = $"{port.Name} kadırgaları", SideB = $"{cp?.Name ?? "Korsanlar"}", MoraleA = 0.55, MoraleB = 0.45, MaxRounds = 10 });
            b.Naval = true; Agents.RecordBattle(s, b); s.Metric("pirateNaval");
            int lostG = J.Filter(A, x => x.Kind == "galley" && x.Hp <= 0).Count;
            port.Galleys = JsMath.Max(0, (port.Galleys ?? 0) - lostG);
            a.Troops = J.Filter(B, x => x.Kind == "monster" && x.Hp > 0).Count; a.Boss = J.Some(B, x => J.T(x.Boss) && x.Hp > 0);
            if (b.Winner == "A")
            {
                s.Log("sea", $"{port.Name} kadırgaları korsanları açıkta karşıladı ve kaçırdı.", civ: c.Id, tile: here, battle: b.Id, major: true, cause: $"{cp?.Name ?? "Korsan koyu"}; {J.S(b.LossesB)} korsan öldü");
                if ((a.Troops ?? 0) <= 0 && !J.T(a.Boss)) a.Dead = true; else Agents.RaidReturn(s, a);
                return;
            }
            s.Log("sea", $"Korsanlar {Tr.Ek(port.Name, "in")} kadırgalarını yarıp kıyıya yöneldi{(lostG != 0 ? $": {J.S(lostG)} kadırga battı" : "")}.", civ: c.Id, tile: here, battle: b.Id, major: true);
        }
    }

    private static void PirateAttack(Sim s, Agent p, Agent t, Camp cp)
    {
        var w = s.W;
        int here = Agents.TileOf(t);
        var c = w.Civs[t.Civ];
        var A = new List<Combatant>();
        if (t.Kind == "caravan") A.AddRange(Agents.CivTroops(s, c, t.Troops ?? 2, "A"));
        else { double n = t.Kind == "settlers" ? JsMath.Min(5, s.PopSize(t.Pop)) : 2; for (int i = 0; i < n; i++) A.Add(Combat.Unit(D.UNITS["militia"], "A", "militia", D.RACES[c.Race].Hp)); }
        for (int i = 0; i < (t.Galleys ?? 0); i++) A.Add(Combat.Unit(GALLEY, "A", "galley"));
        if (J.Some(s.CivSettlements(c), x => J.T(x.Civics.Get("lighthouse")) && x.Port != null && s.G.Dist(x.Port.Value, here) <= 8)) foreach (var x in A) x.Ac += 2;
        var B = Monsters.MonsterSide("pirate", p.Troops ?? 0, J.T(p.Boss), "B");
        var isle = IsleOf(s, cp?.Tile ?? here);
        var b = Combat.ResolveBattle(s.Rng, A, B, new BattleOpts { Id = s.Id(), Day = s.Day, Tile = here, Title = $"Korsan baskını{(isle != null ? $" ({isle.Name} açıkları)" : "")}", SideA = $"{c.Name} gemisi", SideB = $"{cp?.Name ?? "Korsanlar"}", MoraleA = 0.45, MoraleB = 0.5, MaxRounds = 10 });
        b.Naval = true; Agents.RecordBattle(s, b);
        bool hadCaptain = J.T(p.Boss);
        p.Troops = J.Filter(B, x => x.Kind == "monster" && x.Hp > 0).Count; p.Boss = J.Some(B, x => J.T(x.Boss) && x.Hp > 0);
        if (hadCaptain && !J.T(p.Boss) && cp != null) { cp.HadBoss = false; s.Log("sea", $"Kaptan {cp.Captain ?? ""} {Tr.Ek(c.Name, "in")} gemisinin güvertesinde öldü!", civ: c.Id, tile: here, major: true, cause: cp.Name); cp.Captain = null; }
        int deadG = J.Filter(A, x => x.Kind == "galley" && x.Hp <= 0).Count;
        var home = t.Hull != null ? s.Settlement(t.Hull.Value) : null;
        if (deadG != 0 && home != null) home.Galleys = JsMath.Max(0, (home.Galleys ?? 0) - deadG);
        if (t.Kind == "caravan") t.Troops = J.Filter(A, x => (x.Kind == "soldier" || x.Kind == "unique") && x.Hp > 0).Count;
        string what = t.Kind == "settlers" ? "öncü gemisini" : t.Kind == "caravan" ? "yük gemisini" : StartsExplore(t.Purpose) ? "keşif gemisini" : "gemisini";
        c.Yearly.Set("pirateHit", s.DynYear);
        if (b.Winner == "B")
        {
            double value = 0;
            string loot = "";
            if (t.Cargo != null) foreach (var kv in t.Cargo) { double q = kv.Value; value += q * s.Price(c, kv.Key); loot = $", yükü ({J.S(JsMath.Round(q))} {(kv.Key == "grain" ? "tahıl" : "mal")}) yağmalandı"; }
            p.Loot = (p.Loot ?? 0) + JsMath.Round(value);
            if (home != null) home.Ships = JsMath.Max(0, (home.Ships ?? 0) - 1);
            t.Dead = true;
            c.Threat += 0.25;
            s.Metric("pirateCapture");
            s.Log("sea", $"Korsanlar {Tr.Ek(c.Name, "in")} {what} ele geçirdi{loot}{(t.Kind == "settlers" ? $"; {J.S(s.PopSize(t.Pop))} öncü esir düştü" : "")}.", civ: c.Id, tile: here, battle: b.Id, major: true, cause: $"{cp?.Name ?? "Korsan koyu"}{(J.T(cp?.Captain) ? $", Kaptan {cp.Captain}" : "")}");
            Agents.RaidReturn(s, p);
        }
        else
        {
            s.Metric("pirateRepelled");
            string subj = t.Kind == "settlers" ? "öncü gemisi" : t.Kind == "caravan" ? "yük gemisi" : StartsExplore(t.Purpose) ? "keşif gemisi" : "gemisi";
            s.Log("sea", $"{Tr.Ek(c.Name, "in")} {subj} korsanları püskürttü.", civ: c.Id, tile: here, battle: b.Id, major: J.T(t.Galleys), cause: J.T(t.Galleys) ? "Eşlik eden kadırgalar" : $"{J.S(b.LossesB)} korsan öldü");
            if ((p.Troops ?? 0) <= 0 && !J.T(p.Boss)) p.Dead = true; else Agents.RaidReturn(s, p);
        }
    }

    /// <summary>korsan dönüşü: denizden koyuna (kıyı baskınından sonra karaya çıktığı yerden biner); null = yol yok</summary>
    public static List<int> PirateReturnPath(Sim s, Agent a, int cove)
    {
        int here = Agents.TileOf(a);
        var emb = new List<int> { here }; if (a.Landing != null) emb.Add(a.Landing.Value);
        return NavPath(s, here, cove, new NavOpts { Embark = emb, Open = true });
    }

    /// <summary>korsan ganimeti Haydut limanlarında satılır</summary>
    public static void PirateSmuggle(Sim s, Agent a)
    {
        double loot = a.Loot ?? 0;
        if (loot < 10) return;
        var w = s.W;
        var cp = J.Find(w.Camps, x => x.Id == a.From);
        if (cp == null) return;
        var rogue = J.Find(w.Civs, c => c.Alive && Smugglers(c) && J.Some(Ports(s, c), x => s.G.Dist(x.Port.Value, cp.Tile) <= 45));
        if (rogue == null) return;
        double cut = JsMath.Round(loot * 0.3);
        s.Add(rogue, "gold", cut);
        a.Loot = loot - cut;
        if (!J.T(rogue.Yearly.Get("smuggle" + J.S(cp.Id))))
        {
            rogue.Yearly.Set("smuggle" + J.S(cp.Id), s.DynYear);
            s.Log("sea", $"{Tr.Ek(rogue.Name, "in")} kaçakçıları {Tr.Ek(cp.Name, "in")} ganimetini limanlarında satmaya başladı.", civ: rogue.Id, tile: cp.Tile, major: true, cause: "Korsanlar Haydut gemilerine dokunmuyor");
        }
        s.Metric("smuggled", cut);
    }

    /// <summary>korsan avı: gemisi olan medeniyet, kıyılarını ya da gemilerini vuran koya asker ve kadırga gönderir</summary>
    private static void ConsiderPirateHunt(Sim s, Civ c)
    {
        if (!s.CivAt(c, Gate.SHIPBUILDING) || J.Some(s.W.Agents, a => a.Civ == c.Id && a.Purpose == "expedition")) return;
        var ss = s.CivSettlements(c);
        var my = Ports(s, c);
        if (my.Count == 0) return;
        bool hit = (c.Yearly.Get("pirateHit") ?? -9) >= s.DynYear - 1;
        var cands = J.Filter(
            J.Map(J.Filter(s.W.Camps, x => x.Alive && x.Kind == "pirate"), x => (X: x, D: J.MinOf(ss, st => s.G.Dist(st.Tile, x.Tile)))),
            o => o.D <= 40 && (hit || (o.D <= 14 && s.Day - o.X.Founded > 3 * Sim.OLD_YEAR)));
        var cove = J.At(J.Sort(cands, (a, b) => a.D - b.D), 0).X;
        if (cove == null) return;
        double soldiers = Math.Floor(J.Reduce(ss, (a, x) => a + x.Soldiers, 0.0) * 0.6);
        var heroes = J.Filter(s.CivHeroes(c), h => h.State == "home" && h.Hp > h.MaxHp * 0.7);
        if (soldiers < 6 && heroes.Count == 0) return;
        var cap = s.Capital(c);
        var route = CivPath(s, c, cap.Tile, cove.Tile);
        if (route == null || route.Hull == null || !HasSea(s, route.Path)) return;
        double gal = JsMath.Min(3, FreeGalleys(s, route.Hull));
        var side = new List<Combatant>();
        side.AddRange(J.Map(heroes, h => Combat.HeroCombatant(h, "A")));
        side.AddRange(Agents.CivTroops(s, c, soldiers, "A"));
        for (int i = 0; i < gal; i++) side.Add(Combat.Unit(GALLEY, "A", "galley"));
        if (Combat.PowerOf(side) < Heroes.CampPower(s, cove) * 1.15) return;
        var pop = Agents.DrawSoldiers(s, c, soldiers);
        foreach (var h in heroes) h.State = "army";
        s.W.Agents.Add(new Agent { Id = s.Id(), Kind = "army", Civ = c.Id, Path = route.Path, Step = 0, Progress = 0, Speed = Pace.EXPEDITION, Heroes = J.Map(heroes, h => h.Id), Troops = soldiers, Pop = pop, From = cap.Id, To = cove.Id, Purpose = "expedition", Hull = route.Hull.Id, Galleys = J.T(gal) ? gal : (double?)null });
        s.Metric("pirateHunt");
        s.Log("sea", $"{c.Name} korsan avına çıktı: {J.S(soldiers)} asker{(J.T(gal) ? $", {J.S(gal)} kadırga" : "")}{(heroes.Count > 0 ? $" ve {string.Join(", ", J.Map(heroes, h => h.Name))}" : "")} ile {Tr.Ek(cove.Name, "a")} yelken açtı.", civ: c.Id, tile: route.Hull.Port, major: true, cause: hit ? "Korsanlar gemilerimizi ve kıyılarımızı vuruyor" : $"{cove.Name} kıyılarımıza fazla yakın");
    }
}
