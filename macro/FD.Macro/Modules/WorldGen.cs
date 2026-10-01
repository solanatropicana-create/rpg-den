using System;
using System.Collections.Generic;
using System.Linq;

// Dünya üretimi: kıta şekli, yükseklik/nem arazisi, göller, nehirler, medeniyet başlangıçları, yataklar,
// kadim ormanlar, goblin kampları, adalar (ayrı rastgele akışlarla) ve dağ geçitleri.
// Port of src/sim/worldgen.ts.

namespace FD.Macro;

public static class WorldGen
{
    public const int MAP_W = 110;
    public const int MAP_H = 75;
    /// <summary>eski 72×48 haritaya göre kara alanı çarpanı (yatak, kamp sayıları)</summary>
    public const double AREA_K = 1.6;

    /// <summary>Değer gürültüsü: ızgara rng'den hemen dolar; dönen (x, y) → [0,1) fonksiyonu rng kullanmaz.</summary>
    private static Func<double, double, double> ValueNoise(Rng rng, double W, double H, double cell)
    {
        int gw = (int)Math.Ceiling(W / cell) + 2, gh = (int)Math.Ceiling(H / cell) + 2;
        var grid = new List<double>();
        for (int i = 0; i < gw * gh; i++) grid.Add(rng.Next());
        static double s(double t) => t * t * (3 - 2 * t);
        return (x, y) =>
        {
            double gx = x / cell, gy = y / cell;
            double x0 = Math.Floor(gx), y0 = Math.Floor(gy);
            double tx = s(gx - x0), ty = s(gy - y0);
            double g(double a, double b) => J.AtN(grid, b * gw + a) ?? double.NaN;
            double a = g(x0, y0) * (1 - tx) + g(x0 + 1, y0) * tx;
            double b = g(x0, y0 + 1) * (1 - tx) + g(x0 + 1, y0 + 1) * tx;
            return a * (1 - ty) + b * ty;
        };
    }

    /// <summary>Boş ilişki kaydı: temas yok, mods/tension boş, war/treaty null, lastTalk/lastRaid = -9999.</summary>
    public static Relation EmptyRel() => new Relation { Contact = false, Mods = new List<RelMod>(), War = null, Treaty = null, Tension = new JsObj<double>(), LastTalk = -9999, LastRaid = -9999 };

    /// <summary>JS <c>rng.pick(arr)</c> for a tile list where "undefined" matters: consumes one number, null when empty.</summary>
    private static int? PickN(Rng rng, List<int> arr)
    {
        int v = rng.Pick(arr);
        return arr.Count > 0 ? v : null;
    }

    /// <summary>Seed'den bütün dünyayı üretir (anakara, medeniyetler, yataklar, kamplar, adalar, geçitler); RngState = ana akışın son durumu.</summary>
    public static World GenerateWorld(double seed)
    {
        var rng = new Rng(seed);
        int W = MAP_W, H = MAP_H;
        var g = new HexGrid(W, H);
        var n1 = ValueNoise(rng, W, H, 10); var n2 = ValueNoise(rng, W, H, 4); var m1 = ValueNoise(rng, W, H, 8); var m2 = ValueNoise(rng, W, H, 3); var of = ValueNoise(rng, W, H, 5);
        // kıta şekli: büyük ölçekli gürültü + merkeze doğru yükselen kubbe; eşik kara oranına göre seçilir
        var L1 = ValueNoise(rng, W, H, 26); var L2 = ValueNoise(rng, W, H, 11); var L3 = ValueNoise(rng, W, H, 5);
        var shape = new List<double>();
        for (int r = 0; r < H; r++)
            for (int c = 0; c < W; c++)
            {
                double dx = (c - W / 2.0) / (W / 2.0), dy = (r - H / 2.0) / (H / 2.0);
                double d = Math.Sqrt(dx * dx * 0.9 + dy * dy * 1.1);
                shape.Add(L1(c, r) * 0.48 + L2(c, r) * 0.36 + L3(c, r) * 0.16 - JsMath.Pow(d, 2.4) * 0.6);
            }
        var sorted = J.Sorted(shape, (a, b) => a - b);
        double thr = sorted[(int)Math.Floor(sorted.Count * 0.44)]; // ~%56 kara
        var tiles = new List<Tile>();
        var landE = new List<double>();
        for (int r = 0; r < H; r++)
            for (int c = 0; c < W; c++)
            {
                int i = r * W + c;
                int edge = Math.Min(Math.Min(c, r), Math.Min(W - 1 - c, H - 1 - r));
                bool sea = shape[i] < thr || edge < 2;
                double e = n1(c, r) * 0.62 + n2(c, r) * 0.26 + JsMath.Max(0, shape[i] - thr) * 0.45;
                if (!sea) landE.Add(e);
                tiles.Add(new Tile { Terrain = sea ? "water" : "grass", Elev = sea ? JsMath.Max(0, 0.18 - (thr - shape[i]) * 0.8) : e, Owner = -1, Road = 0, Deposit = -1, Reserve = 0, Wood = 0, Sea = sea ? true : null });
            }
        // yükseklik dağılımı karaya göre: dağ ve tepe oranı sabit
        J.Sort(landE, (a, b) => a - b);
        double quant(double f) => J.AtN(landE, Math.Min(landE.Count - 1, Math.Floor(landE.Count * f))) ?? double.NaN; // TS: q
        double eMtn = quant(0.925), eHill = quant(0.8), eLow = quant(0.22);
        double tundraRows = JsMath.Round(H * 0.1);
        for (int r = 0; r < H; r++)
            for (int c = 0; c < W; c++)
            {
                int i = r * W + c; var t = tiles[i];
                if (J.T(t.Sea)) continue;
                double e = t.Elev, m = m1(c, r) * 0.7 + m2(c, r) * 0.3;
                string terrain = "grass";
                if (e > eMtn) terrain = "mountain";
                else if (e > eHill) terrain = "hill";
                else if (e < eLow && m > 0.56) terrain = "swamp";
                else if (m > 0.54) terrain = of(c, r) > 0.68 && m > 0.6 ? "oldforest" : "forest";
                if (r < tundraRows + JsMath.Round(n2(c, 0) * 3) && (terrain == "grass" || terrain == "swamp" || terrain == "forest")) terrain = "tundra";
                t.Terrain = terrain;
                // kıyı: denize göre yükseklik 0.3–0.7 aralığına yayılır (görünüm için)
                t.Elev = 0.28 + ((e - landE[0]) / JsMath.Max(0.01, landE[landE.Count - 1] - landE[0])) * 0.62;
            }
        // adalar: açık denizde birkaç küçük kara parçası
        double nIsles = rng.Int(3, 7);
        for (int k = 0; k < nIsles; k++)
        {
            for (int tries = 0; tries < 80; tries++)
            {
                int i = J.I(rng.Int(0, W * H - 1));
                int c = g.Col(i), r = g.Row(i);
                if (!J.T(tiles[i].Sea) || c < 4 || r < 4 || c > W - 5 || r > H - 5) continue;
                if (J.Some(g.Within(i, 3), n => !J.T(tiles[n].Sea))) continue;
                double rad = rng.Int(1, 3);
                foreach (int n in g.Within(i, J.I(rad)))
                {
                    int cn = g.Col(n), rn = g.Row(n);
                    if (cn < 3 || rn < 3 || cn > W - 4 || rn > H - 4) continue;
                    if (g.Dist(i, n) == rad && rng.Chance(0.45)) continue;
                    double e = 0.3 + (1 - g.Dist(i, n) / (rad + 1)) * 0.4 + rng.Next() * 0.1;
                    double m = rng.Next();
                    var nt = tiles[n].Clone(); nt.Sea = null; nt.Elev = e; nt.Terrain = e > 0.62 ? "hill" : m > 0.55 ? "forest" : "grass";
                    tiles[n] = nt;
                }
                break;
            }
        }
        // kara parçaları: en büyüğü anakara (isle 0), küçük adacıklar denize döner
        (int[] comp, List<int> sizes) labelLand()
        {
            var comp = new int[tiles.Count];
            Array.Fill(comp, -1);
            var sizes = new List<int>();
            for (int i = 0; i < tiles.Count; i++)
            {
                if (J.T(tiles[i].Sea) || comp[i] >= 0) continue;
                int id = sizes.Count; int n = 0;
                var st = new List<int> { i }; comp[i] = id;
                while (st.Count > 0) { int cur = J.Pop(st); n++; foreach (int nb in g.Neighbors(cur)) if (!J.T(tiles[nb].Sea) && comp[nb] < 0) { comp[nb] = id; st.Add(nb); } }
                sizes.Add(n);
            }
            return (comp, sizes);
        }
        {
            var (comp, sizes) = labelLand();
            double mx = J.MaxOf(sizes, x => x);
            int main = J.FindIndex(sizes, x => x == mx);
            var pairs = new List<(int N, int K)>();
            for (int k = 0; k < sizes.Count; k++) pairs.Add((sizes[k], k));
            var order = J.Map(J.Sort(J.Filter(pairs, p => p.K != main), (a, b) => b.N - a.N), p => p.K);
            var isleId = new Dictionary<int, int> { [main] = 0 };
            for (int j = 0; j < order.Count; j++) isleId[order[j]] = j + 1;
            for (int i = 0; i < tiles.Count; i++)
            {
                if (comp[i] < 0) continue;
                if (sizes[comp[i]] < 4) { var nt = tiles[i].Clone(); nt.Terrain = "water"; nt.Sea = true; nt.Elev = 0.12; tiles[i] = nt; continue; }
                int id = isleId[comp[i]];
                if (J.T(id)) tiles[i].Isle = id;
            }
        }
        bool mainLand(int i) => !J.T(tiles[i].Sea) && !J.T(tiles[i].Isle);
        // Göller (yalnız iç kesimde)
        for (int k = 0; k < JsMath.Round(4 * AREA_K); k++)
        {
            int best = -1; double be = 9;
            for (int t = 0; t < 60; t++) { int i = J.I(rng.Int(0, W * H - 1)); if (mainLand(i) && tiles[i].Elev < be && tiles[i].Terrain != "tundra" && !J.Some(g.Within(i, 3), n => J.T(tiles[n].Sea))) { be = tiles[i].Elev; best = i; } }
            if (best >= 0) foreach (int t in g.Within(best, J.I(rng.Int(1, 2)))) if (!J.T(tiles[t].Sea) && rng.Chance(0.8)) tiles[t].Terrain = "water";
        }
        // Nehirler: dağdan denize ya da göle akar
        for (int k = 0; k < JsMath.Round(3 * AREA_K) + 1; k++)
        {
            var mts = new List<int>();
            for (int i = 0; i < tiles.Count; i++) { int v = tiles[i].Terrain == "mountain" && mainLand(i) ? i : -1; if (v >= 0) mts.Add(v); }
            if (mts.Count == 0) break;
            int cur = rng.Pick(mts);
            var seen = new HashSet<int>();
            for (int step = 0; step < 120; step++)
            {
                seen.Add(cur);
                if (tiles[cur].Terrain != "mountain") tiles[cur].Terrain = "water";
                var ns = J.Filter(g.Neighbors(cur), n => !seen.Contains(n));
                if (ns.Count == 0) break;
                if (J.Some(ns, n => J.T(tiles[n].Sea))) break;
                if (J.Some(ns, n => tiles[n].Terrain == "water" && !seen.Contains(n)) && step > 6) break;
                // karşılaştırıcı içinde rng kullanmak motorlar arası farklı sonuç verir; önce sabit gürültü
                var jit = J.Map(ns, n => tiles[n].Elev + (rng.Next() - 0.5) * 0.06);
                cur = ns[jit.IndexOf(J.MinOf(jit, x => x))];
            }
        }

        // Medeniyet seçimi
        var pool = J.Filter(D.CLASSES.Keys(), c => D.CLASSES[c].Implemented);
        rng.Shuffle(pool);
        double count = 7 + rng.Int(0, 2);
        var chosen = J.Slice(pool, 0, J.I(count));
        var starts = new List<int>();
        foreach (string cls in chosen)
        {
            var like = D.CLASSES[cls].TerrainLike;
            int best = -1; double bs = double.NegativeInfinity;
            for (int i = 0; i < tiles.Count; i++)
            {
                var t = tiles[i];
                if (t.Terrain != "grass" || !mainLand(i)) continue;
                if (J.Some(g.Within(i, 1), n => J.T(tiles[n].Sea))) continue;
                if (J.Some(starts, s => g.Dist(s, i) < 15)) continue;
                double sc = rng.Next() * 3;
                foreach (int n in g.Within(i, 3)) sc += like.Get(tiles[n].Terrain) ?? 0.3;
                if (J.Some(g.Within(i, 2), n => tiles[n].Terrain == "water")) sc += 3;
                if (sc > bs) { bs = sc; best = i; }
            }
            if (best < 0)
            {
                // yedek: en uzak çayır
                for (int i = 0; i < tiles.Count; i++) if (mainLand(i) && tiles[i].Terrain != "water" && tiles[i].Terrain != "mountain" && !J.Some(starts, s => g.Dist(s, i) < 10)) { best = i; break; }
            }
            starts.Add(best);
            foreach (int t in g.Within(best, 1)) if (tiles[t].Terrain != "water") tiles[t].Terrain = "grass";
            // yakında orman garantisi
            var ring = J.Filter(g.Within(best, 3), t => g.Dist(best, t) >= 2 && tiles[t].Terrain == "grass");
            int forests = J.Filter(g.Within(best, 3), t => tiles[t].Terrain == "forest" || tiles[t].Terrain == "oldforest").Count;
            foreach (int t in J.Slice(rng.Shuffle(ring), 0, Math.Max(0, 4 - forests))) tiles[t].Terrain = "forest";
            // tepe garantisi (taş)
            if (!J.Some(g.Within(best, 2), t => tiles[t].Terrain == "hill"))
            {
                var cands = J.Filter(g.Within(best, 2), t => g.Dist(best, t) == 2 && tiles[t].Terrain == "grass");
                if (cands.Count > 0) tiles[rng.Pick(cands)].Terrain = "hill";
            }
        }

        // Temel rezervler
        foreach (var t in tiles)
        {
            if (t.Terrain == "forest" || t.Terrain == "oldforest") t.Wood = D.WOOD_RESERVE;
            if (t.Terrain == "hill" || t.Terrain == "mountain") t.Reserve = D.STONE_RESERVE;
        }

        // Yataklar
        var deposits = new List<Deposit>();
        int nextId = 1;
        Deposit place(string kind, int? near = null, int dmin = 3, int dmax = 7)
        {
            var def = D.DEPOSITS[kind];
            bool ok(int i) => tiles[i].Deposit < 0 && !starts.Contains(i) && J.Every(starts, s => g.Dist(s, i) >= (near != null ? 1 : 3));
            int seedTile = -1;
            for (int tries = 0; tries < 400; tries++)
            {
                int? i = near != null ? PickN(rng, J.Filter(g.Within(near.Value, dmax), x => g.Dist(near.Value, x) >= dmin)) : J.I(rng.Int(0, W * H - 1));
                if (i == null || !ok(i.Value) || J.T(tiles[i.Value].Sea)) continue;
                if (!def.Terrain.Contains(tiles[i.Value].Terrain))
                {
                    if (near == null || tiles[i.Value].Terrain == "water" || tiles[i.Value].Terrain == "mountain") continue;
                    tiles[i.Value].Terrain = def.Terrain[0];
                    if (tiles[i.Value].Terrain == "hill") tiles[i.Value].Reserve = D.STONE_RESERVE;
                }
                seedTile = i.Value; break;
            }
            if (seedTile < 0) return null;
            double size = rng.Int(def.Size[0], def.Size[1]);
            var cl = new List<int> { seedTile };
            var frontier = new List<int> { seedTile };
            while (cl.Count < size && frontier.Count > 0)
            {
                int cur = J.Shift(frontier);
                foreach (int n in rng.Shuffle(g.Neighbors(cur)))
                {
                    if (cl.Count >= size) break;
                    if (cl.Contains(n) || !ok(n) || J.T(tiles[n].Sea)) continue;
                    if (!def.Terrain.Contains(tiles[n].Terrain))
                    {
                        if (tiles[n].Terrain == "water" || rng.Chance(0.6)) continue;
                        if (kind == "fertile" || kind == "horses") { if (tiles[n].Terrain != "forest") continue; }
                        tiles[n].Terrain = def.Terrain[0];
                        if (tiles[n].Terrain == "hill" || tiles[n].Terrain == "mountain") tiles[n].Reserve = D.STONE_RESERVE;
                    }
                    cl.Add(n); frontier.Add(n);
                }
            }
            double richness = rng.Pick(new List<double> { 0.7, 1, 1, 1.4 });
            var d = new Deposit { Id = nextId++, Kind = kind, Tiles = cl, Richness = richness, Depleted = false, KnownBy = new List<int>() };
            foreach (int t in cl) { tiles[t].Deposit = d.Id; tiles[t].Reserve = J.T(def.Reserve) ? JsMath.Round(def.Reserve * richness) : 0; }
            deposits.Add(d);
            return d;
        }
        // garantiler
        for (int k = 0; k < chosen.Count; k++)
        {
            string cls = chosen[k];
            int s = starts[k];
            place("fertile", s, 1, 2);
            place("clay", s, 2, 3);
            if (cls == "cleric") place("copper", s, 4, 8);
            if (cls == "barbarian") place("horses", s, 3, 7);
            if (cls == "rogue") place("salt", s, 4, 8);
        }
        foreach (string kind in D.DEPOSITS.Keys())
        {
            var def = D.DEPOSITS[kind];
            int already = J.Filter(deposits, d => d.Kind == kind).Count;
            for (int k = already; k < JsMath.Round(def.Count * AREA_K); k++) place(kind);
        }
        // Kadim ormanlar: kümeleri yatak yap
        {
            var seen = new HashSet<int>();
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i].Terrain != "oldforest" || seen.Contains(i) || tiles[i].Deposit >= 0) continue;
                var blob = new List<int>();
                var q = new List<int> { i };
                seen.Add(i);
                while (q.Count > 0)
                {
                    int c = J.Pop(q);
                    blob.Add(c);
                    foreach (int n in g.Neighbors(c)) if (!seen.Contains(n) && tiles[n].Terrain == "oldforest" && tiles[n].Deposit < 0) { seen.Add(n); q.Add(n); }
                }
                if (blob.Count < 3) { foreach (int t in blob) tiles[t].Terrain = "forest"; continue; }
                var d = new Deposit { Id = nextId++, Kind = "heartwood", Tiles = J.Slice(blob, 0, 8), Richness = 1, Depleted = false, KnownBy = new List<int>() };
                foreach (int t in d.Tiles) { tiles[t].Deposit = d.Id; tiles[t].Reserve = D.DEPOSITS["heartwood"].Reserve; }
                deposits.Add(d);
            }
        }
        // Druid'e kadim orman garantisi
        int di = chosen.IndexOf("druid");
        if (di >= 0 && !J.Some(deposits, d => d.Kind == "heartwood" && J.Some(d.Tiles, t => g.Dist(t, starts[di]) <= 7)))
        {
            var cands = J.Filter(g.Within(starts[di], 6), t => g.Dist(t, starts[di]) >= 3 && tiles[t].Deposit < 0 && tiles[t].Terrain != "water" && tiles[t].Terrain != "mountain");
            // TS: rng.pick([]) → undefined, sonra tiles[undefined].terrain = ... TypeError atar
            int seedT = PickN(rng, cands) ?? throw new InvalidOperationException("worldgen: no heartwood candidate near the druid start (TS throws a TypeError here)");
            var blob = new List<int> { seedT };
            blob.AddRange(J.Slice(J.Filter(g.Neighbors(seedT), n => tiles[n].Deposit < 0 && tiles[n].Terrain != "water" && !starts.Contains(n)), 0, 3));
            var d = new Deposit { Id = nextId++, Kind = "heartwood", Tiles = blob, Richness = 1, Depleted = false, KnownBy = new List<int>() };
            foreach (int t in blob) { tiles[t].Terrain = "oldforest"; tiles[t].Wood = D.WOOD_RESERVE; tiles[t].Deposit = d.Id; tiles[t].Reserve = D.DEPOSITS["heartwood"].Reserve; }
            deposits.Add(d);
        }

        var civs = new List<Civ>();
        var settlements = new List<Settlement>();
        for (int i = 0; i < chosen.Count; i++)
        {
            string cls = chosen[i];
            civs.Add(MakeCiv(i, cls, 0));
            settlements.Add(MakeSettlement(nextId++, i, D.CLASSES[cls].Capital, starts[i], new JsObj<double> { [D.CLASSES[cls].Race] = 6 }, 0));
        }

        var camps = new List<Camp>();
        for (int k = 0; k < 3; k++)
        {
            var avoid = new List<int>(starts);
            avoid.AddRange(J.Map(camps, c => c.Tile));
            int t = PickCampTile(g, tiles, rng, avoid, 11);
            if (t >= 0) camps.Add(MakeCamp(nextId++, "goblin", t, rng.Pick(GOBLIN_CAMP_NAMES), 0, rng));
        }
        foreach (var c in camps) tiles[c.Tile].Camp = c.Id;

        // tarafsız hanlar dünya kurulurken yoktur: oyun başlayınca hancılar yerleşimlerden öküz arabasıyla çıkıp
        // sınır bölgelerinde kendileri kurar (innlife.ts). Kaç hancının yola çıkacağı burada belirlenir.
        int innTarget = Math.Max(2, Math.Min(4, starts.Count - 2));

        // adaların hazinesi: ayrı rastgele akışla (anakara ve eski dünyalar değişmez); kolonileşmeye değer adalar
        // denizler: seed'e göre kıta (birkaç ada), takımada ya da büyük adalar; ayrı rastgele akışla eklenir
        var seaInfo = AddIsles(g, tiles, seed); // TS: sea
        EnrichIsles(g, tiles, deposits, seed);
        // dağ kütleleri geçilmez: bir kara parçasını ikiye bölen dağda en ucuz yerden geçit açılır
        double passes = CarvePasses(g, tiles);

        var relations = J.Map(civs, _ => J.Map(civs, __ => EmptyRel()));
        return new World
        {
            Seed = seed, Day = 0, Width = W, Height = H, Tiles = tiles, Deposits = deposits, Civs = civs, Settlements = settlements, Heroes = new List<Hero>(), Camps = camps, Inns = new List<Inn>(),
            InnPlan = new InnPlan { Target = innTarget, Wave = innTarget, Next = 2 }, Quests = new List<Quest>(), Agents = new List<Agent>(), Routes = new List<TradeRoute>(), Relations = relations,
            Events = new List<GameEvent>(), Battles = new List<Battle>(), NextId = nextId, RngState = rng.State(), Metrics = new JsObj<double> { ["mountainPass"] = passes },
            Isles = seaInfo.Isles, SeaProfile = seaInfo.Profile,
        };
    }

    public static readonly List<string> GOBLIN_CAMP_NAMES = new() { "Kırıkdiş Kampı", "Çürükpençe Kampı", "Kanlıkaya Kampı", "Paslıkılıç Kampı", "Kemikçatal Kampı", "Karagöz Kampı", "Kurtkulak Kampı", "Sümüklüdere Kampı" };
    public static readonly List<string> HOB_NAMES = new() { "Demirtoynak Karakolu", "Kızılsancak Karakolu", "Kara Lejyon Karakolu", "Paslızırh Karakolu", "Kanlıbayrak Karakolu", "Demirçene Karakolu", "Külrengi Karakolu", "Kırbaç Karakolu" };
    public static readonly List<string> BUGBEAR_NAMES = new() { "Sessizpençe İni", "Kıllıgölge İni" };

    /// <summary>600 rastgele denemeyle kamp karosu seçer (su/dağ/sahipli/kamp/yapı/ada/han halkası değil, avoid'dan ≥ minD); bulunamazsa -1.</summary>
    public static int PickCampTile(HexGrid g, List<Tile> tiles, Rng rng, List<int> avoid, double minD)
    {
        int best = -1; double bs = double.NegativeInfinity;
        for (int k = 0; k < 600; k++)
        {
            int i = J.I(rng.Int(0, tiles.Count - 1));
            var t = tiles[i];
            if (t.Terrain == "water" || t.Terrain == "mountain" || t.Owner >= 0 || t.Camp != null || t.Ext != null || J.T(t.Isle) || t.InnZone != null) continue;
            double d = avoid.Count > 0 ? J.MinOf(avoid, a => g.Dist(a, i)) : 20;
            if (d < minD) continue;
            double sc = -Math.Abs(d - minD - 3) + (t.Terrain == "forest" || t.Terrain == "hill" ? 3 : 0) + rng.Next() * 2;
            if (sc > bs) { bs = sc; best = i; }
        }
        return best;
    }

    /// <summary>Yeni canavar kampı (goblin 5, hobgoblin 7 + şef, diğer 3 kişi); nextRaid = day + rng.int(goblin ? 360 : 150, 460).</summary>
    public static Camp MakeCamp(int id, string kind, int tile, string name, double day, Rng rng)
    {
        double count = kind == "goblin" ? 5 : kind == "hobgoblin" ? 7 : 3;
        return new Camp { Id = id, Kind = kind, Tile = tile, Name = name, Count = count, Boss = kind == "hobgoblin", HadBoss = kind == "hobgoblin", Loot = 10, GrowthAcc = 0, Alive = true, NextRaid = day + rng.Int(kind == "goblin" ? 360 : 150, 460), Founded = day };
    }

    /// <summary>Sınıf tanımından yeni medeniyet: başlangıç stoku, align ve eff sınıf değerlerinin kopyası (kademe etkileri
    /// Sim.RecomputeEff'te eklenir).</summary>
    public static Civ MakeCiv(int id, string cls, double day)
    {
        var c = D.CLASSES[cls];
        return new Civ
        {
            Id = id, Cls = cls, Name = c.CivName, Race = c.Race, Align = new Alignment { Law = c.Align.Law, Good = c.Align.Good }, Color = c.Color,
            Stock = new JsObj<double> { ["grain"] = 25, ["meat"] = 10, ["wood"] = 20, ["gold"] = 5 }, Price = new JsObj<double>(), Want = new JsObj<double>(),
            Eff = c.Base != null ? c.Base.Clone() : new JsObj<double>(), Alive = true, Founded = day, Threat = 0, LastRaidedDay = -9999, ScoutSent = false,
            Stats = new CivStats { PeakPop = 6, BattlesWon = 0, BattlesLost = 0, Traded = 0, Mined = new JsObj<double>(), Depleted = 0 }, LastExpand = -9999, Yearly = new JsObj<double>(), History = new List<HistPoint>(),
        };
    }

    /// <summary>Yeni yerleşim (kamp kademesi, bir kulübe); pop sığ kopyalanır ({ ...pop }).</summary>
    public static Settlement MakeSettlement(int id, int civ, string name, int tile, JsObj<double> pop, double day)
    {
        return new Settlement { Id = id, Civ = civ, Name = name, Tile = tile, Founded = day, Pop = pop != null ? pop.Clone() : new JsObj<double>(), GrowthAcc = 0, Civics = new JsObj<double> { ["hut"] = 1 }, Workshops = new JsObj<double>(), Project = null, Jobs = new JsObj<double>(), Soldiers = 0, Alive = true, Starving = 0, Tier = 0, MixedSince = new JsObj<double>() };
    }

    /// <summary>Han karosunu işaretler (tile.Inn, yol ≥ 1) ve 1 yarıçaplı koruma halkasına InnZone yazar.</summary>
    public static void MarkInn(HexGrid g, List<Tile> tiles, Inn inn)
    {
        tiles[inn.Tile].Inn = inn.Id;
        tiles[inn.Tile].Road = JsMath.Max(1, tiles[inn.Tile].Road);
        foreach (int t in g.Within(inn.Tile, 1)) tiles[t].InnZone = inn.Id;
    }

    /// <summary>
    /// Her ada (≥5 karo) bir yatak taşır; büyük adalar bazen iki. Değerli türler ağır basar.
    /// Arazi değiştirilmez ve kimlikler ayrı aralıktan verilir: anakaranın erken tarihi eski dünyalarla aynı kalır.
    /// </summary>
    private static void EnrichIsles(HexGrid g, List<Tile> tiles, List<Deposit> deposits, double seed)
    {
        var rng = new Rng(JsMath.ToInt32(seed * 2654435761.0) ^ 0x51ed2701);
        int id = 900000;
        var isles = new JsMap<int, List<int>>();
        for (int i = 0; i < tiles.Count; i++) { var t = tiles[i]; if (J.T(t.Isle) && t.Terrain != "water") { var a = isles.Get(t.Isle.Value) ?? new List<int>(); a.Add(i); isles.Set(t.Isle.Value, a); } }
        var KINDS = new List<(string Kind, double W)> { ("gold", 3), ("silver", 2), ("salt", 3), ("herbs", 3), ("horses", 2), ("iron", 2), ("copper", 2), ("fertile", 2), ("mana", 1) };
        foreach (var entry in J.Sort(new List<KeyValuePair<int, List<int>>>(isles), (a, b) => a.Key - b.Key))
        {
            var ts = entry.Value;
            if (ts.Count < 5) continue;
            int have = new HashSet<int>(J.Filter(J.Map(ts, i => tiles[i].Deposit), d => d >= 0)).Count;
            double want = (ts.Count >= 40 ? rng.Int(2, 3) : ts.Count >= 18 && rng.Chance(0.5) ? 2 : 1) - have;
            for (int k = 0; k < want; k++)
            {
                var fits = J.Filter(KINDS, kw => J.Some(ts, i => tiles[i].Deposit < 0 && D.DEPOSITS[kw.Kind].Terrain.Contains(tiles[i].Terrain)));
                if (fits.Count == 0) break;
                string kind = rng.Weighted(fits, kw => kw.W).Kind;
                var def = D.DEPOSITS[kind];
                var free = J.Filter(ts, i => tiles[i].Deposit < 0 && def.Terrain.Contains(tiles[i].Terrain));
                int seedT = rng.Pick(free);
                double size = JsMath.Min(free.Count, rng.Int(def.Size[0], JsMath.Max(def.Size[0], def.Size[1] - 1)));
                var cl = new List<int> { seedT };
                foreach (int n in rng.Shuffle(g.Within(seedT, 2))) { if (cl.Count >= size) break; if (n != seedT && free.Contains(n)) cl.Add(n); }
                double richness = rng.Pick(new List<double> { 1, 1, 1.4 });
                var d = new Deposit { Id = id++, Kind = kind, Tiles = cl, Richness = richness, Depleted = false, KnownBy = new List<int>() };
                foreach (int t in cl) { tiles[t].Deposit = d.Id; if (J.T(def.Reserve)) tiles[t].Reserve = JsMath.Round(def.Reserve * richness); }
                deposits.Add(d);
            }
        }
    }

    // ---- adalar ----
    public static readonly JsObj<string> ISLE_TR = new JsObj<string>
    {
        ["volkan"] = "volkanik ada", ["orman"] = "ormanlık ada", ["cayir"] = "çayırlık ada", ["kayalik"] = "kayalık ada", ["bataklik"] = "sisli bataklık adası", ["kumsal"] = "kumsal adacık",
    };
    public static readonly JsObj<string> SEA_PROFILE_TR = new JsObj<string>
    {
        ["kita"] = "kıta ve birkaç ada", ["takimada"] = "takımadalar", ["buyuk"] = "büyük adalar",
    };
    private static readonly JsObj<List<string>> ISLE_NAMES = new JsObj<List<string>>
    {
        ["volkan"] = new() { "Dumanlı Ada", "Ateş Adası", "Kızıl Ada", "Kül Adası", "Kara Ada", "Ejder Adası", "Kükürt Adası", "Kor Adası", "Obsidyen Adası" },
        ["orman"] = new() { "Yeşil Ada", "Çam Adası", "Meşe Adası", "Gölgeli Ada", "Kuş Adası", "Sarmaşık Adası", "Porsuk Adası", "Yosunlu Ada", "Geyik Adası" },
        ["cayir"] = new() { "Keçi Adası", "Rüzgârlı Ada", "Çiçekli Ada", "At Adası", "Uzun Ada", "Bereket Adası", "Arı Adası", "Kuzu Adası", "Gelincik Adası", "Sarı Ada" },
        ["kayalik"] = new() { "Taşlı Ada", "Kartal Adası", "Martı Adası", "Tuzlu Ada", "Yalnız Ada", "Fırtına Adası", "Kayalı Ada", "Karabatak Adası", "Dişli Ada" },
        ["bataklik"] = new() { "Sisli Ada", "Sazlı Ada", "Hayalet Adası", "Yılan Adası", "Kurbağa Adası", "Çürük Ada", "Bataklı Ada", "Sivrisinek Adası" },
        ["kumsal"] = new() { "Kaplumbağa Adası", "İnci Adası", "Mercan Adası", "Balina Adası", "Ay Adası", "Kemik Adası", "Midye Adası", "Yengeç Adası", "Deniz Kızı Adası" },
    };

    /// <summary>kıyıdan uzaklık (karada: en yakın denize), BFS</summary>
    private static Dictionary<int, int> CoastDepth(HexGrid g, List<Tile> tiles, List<int> ts)
    {
        var set = new HashSet<int>(ts); var d = new Dictionary<int, int>(); var q = new List<int>();
        foreach (int i in ts) if (J.Some(g.Neighbors(i), n => !set.Contains(n))) { d[i] = 0; q.Add(i); }
        for (int h = 0; h < q.Count; h++) { int i = q[h]; foreach (int n in g.Neighbors(i)) if (set.Contains(n) && !d.ContainsKey(n)) { d[n] = d[i] + 1; q.Add(n); } }
        return d;
    }

    /// <summary>
    /// Anakara dışına ada ekler ve bütün adalara ad/tür verir. Profil seed'e göre:
    /// kıta (birkaç orta ada), takımada (kümelenmiş çok ada), büyük adalar (2–3 geniş ada).
    /// Yeni adalar mevcut karaya en az 2 deniz karosu uzak durur; eski adaların arazisi değişmez.
    /// </summary>
    private static (string Profile, List<IsleInfo> Isles) AddIsles(HexGrid g, List<Tile> tiles, double seed)
    {
        var rng = new Rng(JsMath.ToInt32(seed * 40503 + 17) ^ 0x2c1b3c6d);
        int W = g.W, H = g.H;
        double p0 = rng.Next();
        string profile = p0 < 0.4 ? "kita" : p0 < 0.75 ? "takimada" : "buyuk";
        bool edgeOk(int i) => Math.Min(Math.Min(g.Col(i), g.Row(i)), Math.Min(W - 1 - g.Col(i), H - 1 - g.Row(i))) >= 3;
        int[] landDist()
        {
            var d = new int[tiles.Count]; Array.Fill(d, 99); var q = new List<int>(); // TS: Int16Array (değerler 0..99)
            for (int i = 0; i < tiles.Count; i++) if (!J.T(tiles[i].Sea)) { d[i] = 0; q.Add(i); }
            for (int h = 0; h < q.Count; h++) { int i = q[h]; if (d[i] >= 12) continue; foreach (int n in g.Neighbors(i)) if (d[n] > d[i] + 1) { d[n] = d[i] + 1; q.Add(n); } }
            return d;
        }
        int nextIsle = 0;
        foreach (var t in tiles) nextIsle = Math.Max(nextIsle, t.Isle ?? 0); // Math.max(0, ...tiles.map((t) => t.isle ?? 0))
        nextIsle += 1;
        double tundraRows = JsMath.Round(H * 0.1);
        List<int> place(int seedTile, double size, double gap)
        {
            var ld = landDist();
            if (!J.T(tiles[seedTile].Sea) || !edgeOk(seedTile) || ld[seedTile] < gap + 1) return null;
            var blob = new List<int> { seedTile }; var inB = new HashSet<int>(blob);
            for (int tries = 0; blob.Count < size && tries < size * 30; tries++)
            {
                int from = blob[(int)Math.Floor(rng.Next() * blob.Count)];
                int n = rng.Pick(g.Neighbors(from));
                if (inB.Contains(n) || !J.T(tiles[n].Sea) || !edgeOk(n) || ld[n] < gap) continue;
                // yuvarlak değil, girintili: kalabalık tarafa ek zorlaşır
                if (J.Filter(g.Neighbors(n), q => inB.Contains(q)).Count >= 4 && rng.Chance(0.5)) continue;
                blob.Add(n); inB.Add(n);
            }
            if (blob.Count < 5) return null;
            int id = nextIsle++;
            foreach (int i in blob)
            {
                var nt = tiles[i].Clone();
                nt.Sea = null; nt.Terrain = "grass"; nt.Elev = 0.34; nt.Isle = id; nt.Owner = -1; nt.Road = 0; nt.Deposit = -1; nt.Reserve = 0; nt.Wood = 0;
                tiles[i] = nt;
            }
            return blob;
        }
        string kindFor(double size)
        {
            if (size <= 8 && rng.Chance(0.6)) return "kumsal";
            var opts = size >= 30
                ? new List<(string K, double W)> { ("volkan", 3), ("orman", 2.5), ("cayir", 2.5), ("kayalik", 1), ("bataklik", 1) }
                : new List<(string K, double W)> { ("volkan", 2), ("orman", 2.5), ("cayir", 2), ("kayalik", 2), ("bataklik", 1.5), ("kumsal", 1) };
            return rng.Weighted(opts, o => o.W).K;
        }
        int? shape(List<int> blob, string kind)
        {
            var depth = CoastDepth(g, tiles, blob);
            double maxD = J.MaxOf(blob, i => depth.TryGetValue(i, out int dv) ? dv : 0);
            int? peak = null;
            foreach (int i in blob)
            {
                int d = depth.TryGetValue(i, out int di) ? di : 0; double r = rng.Next(); var t = tiles[i];
                string tr = "grass";
                switch (kind)
                {
                    case "volkan": tr = d == maxD && maxD >= 2 ? "mountain" : d >= 1 ? (r < 0.6 ? "hill" : r < 0.8 ? "forest" : "grass") : r < 0.25 ? "hill" : "grass"; break;
                    case "orman": tr = r < 0.75 || d >= 1 ? "forest" : "grass"; break;
                    case "cayir": tr = r < 0.8 ? "grass" : "forest"; break;
                    case "kayalik": tr = r < 0.6 || d >= 2 ? "hill" : "grass"; break;
                    case "bataklik": tr = r < 0.55 ? "swamp" : r < 0.85 ? "forest" : "grass"; break;
                    case "kumsal": tr = d >= 1 && r < 0.3 ? "forest" : "grass"; break;
                }
                if (g.Row(i) < tundraRows && (tr == "grass" || tr == "forest" || tr == "swamp")) tr = "tundra";
                t.Terrain = tr;
                t.Elev = JsMath.Min(0.92, 0.3 + d * 0.1 + (tr == "mountain" ? 0.3 : tr == "hill" ? 0.12 : 0) + rng.Next() * 0.04);
                if (tr == "forest") t.Wood = D.WOOD_RESERVE;
                if (tr == "hill" || tr == "mountain") t.Reserve = D.STONE_RESERVE;
                if (tr == "mountain" && (peak == null || t.Elev > tiles[peak.Value].Elev)) peak = i;
            }
            return peak;
        }
        var made = new List<(List<int> Blob, string Kind, int? Peak)>();
        bool add(int seedTile, double size, double gap)
        {
            var blob = place(seedTile, size, gap);
            if (blob == null) return false;
            string kind = kindFor(blob.Count);
            made.Add((blob, kind, shape(blob, kind)));
            return true;
        }
        int randSea(double minLd)
        {
            var ld = landDist();
            int best = -1; double bs = -1;
            for (int k = 0; k < 300; k++) { int i = J.I(rng.Int(0, tiles.Count - 1)); if (!J.T(tiles[i].Sea) || !edgeOk(i) || ld[i] < minLd) continue; double sc = Math.Min(ld[i], 9) + rng.Next() * 3; if (sc > bs) { bs = sc; best = i; } }
            return best;
        }
        if (profile == "kita")
        {
            for (double k = rng.Int(1, 3); k > 0; k--) { int t = randSea(5); if (t >= 0) add(t, rng.Int(8, 22), 3); }
        }
        else if (profile == "takimada")
        {
            for (double cl = rng.Int(2, 3); cl > 0; cl--)
            {
                int c0 = randSea(5);
                if (c0 < 0) continue;
                for (double k = rng.Int(3, 5), tries = 0; k > 0 && tries < 40; tries++)
                {
                    var near = J.Filter(g.Within(c0, 7), i => J.T(tiles[i].Sea));
                    if (near.Count == 0) break;
                    if (add(rng.Pick(near), rng.Int(5, 26), 2)) k--;
                }
            }
        }
        else
        {
            for (double k = rng.Int(2, 3); k > 0; k--) { int t = randSea(7); if (t >= 0) add(t, rng.Int(40, 95), 3); }
        }
        // bütün adalara ad ve tür (eski adaların arazisi olduğu gibi kalır; tür araziden okunur)
        var members = new JsMap<int, List<int>>();
        for (int i = 0; i < tiles.Count; i++) { var t = tiles[i]; if (J.T(t.Isle) && !J.T(t.Sea)) { var a = members.Get(t.Isle.Value) ?? new List<int>(); a.Add(i); members.Set(t.Isle.Value, a); } }
        var used = new HashSet<string>();
        var isles = new List<IsleInfo>();
        foreach (var entry in J.Sort(new List<KeyValuePair<int, List<int>>>(members), (a, b) => a.Key - b.Key))
        {
            int id = entry.Key; var ts = entry.Value;
            int mi = J.FindIndex(made, x => tiles[x.Blob[0]].Isle == id); // TS: m = made.find(...)
            string kind;
            if (mi >= 0) kind = made[mi].Kind;
            else
            {
                double cnt(params string[] tr) => J.Filter(ts, i => Array.IndexOf(tr, tiles[i].Terrain) >= 0).Count / (double)ts.Count;
                kind = ts.Count <= 6 ? "kumsal" : cnt("mountain") > 0 ? "volkan" : cnt("hill") > 0.4 ? "kayalik" : cnt("swamp") > 0.3 ? "bataklik" : cnt("forest", "oldforest") > 0.45 ? "orman" : "cayir";
            }
            var list = rng.Shuffle(J.Slice(ISLE_NAMES[kind]));
            string name = J.Find(list, n => !used.Contains(n)) ?? $"Küçük {list[0]}";
            used.Add(name);
            double cx = 0, cz = 0;
            foreach (int i in ts) { var (x, z) = g.Pixel(i, 1); cx += x; cz += z; }
            cx /= ts.Count; cz /= ts.Count;
            int center = J.Sorted(ts, (a, b) => { var (ax, az) = g.Pixel(a, 1); var (bx, bz) = g.Pixel(b, 1); return (ax - cx) * (ax - cx) + (az - cz) * (az - cz) - ((bx - cx) * (bx - cx) + (bz - cz) * (bz - cz)); })[0];
            int? peak = (mi >= 0 ? made[mi].Peak : null) ?? (kind == "volkan" ? J.Sorted(ts, (a, b) => tiles[b].Elev - tiles[a].Elev)[0] : (int?)null);
            isles.Add(new IsleInfo { Id = id, Name = name, Kind = kind, Size = ts.Count, Center = center, Peak = peak });
        }
        return (profile, isles);
    }

    /// <summary>
    /// Dağlar geçilmez (MOVE_COST = ∞). Bir kara parçasında dağların kapattığı her cep, en az dağ karosu
    /// aşan yoldan ana bölgeye bağlanır; aşılan dağ karoları tepeye (geçit) döner. Yatak karoları son çare.
    /// Rastgelelik kullanmaz: aynı seed aynı geçitleri verir. Açılan geçit sayısını döndürür.
    /// </summary>
    public static double CarvePasses(HexGrid g, List<Tile> tiles)
    {
        int n = tiles.Count;
        bool open(int i) => !J.T(tiles[i].Sea) && tiles[i].Terrain != "mountain";
        int land(int i) => J.T(tiles[i].Sea) ? -1 : tiles[i].Isle ?? 0;
        double carved = 0;
        for (int guard = 0; guard < 200; guard++)
        {
            var comp = new int[n]; Array.Fill(comp, -1);
            var info = new List<(int Land, int Size, int Seed)>();
            for (int i = 0; i < n; i++)
            {
                if (!open(i) || comp[i] >= 0) continue;
                int id = info.Count; var st = new List<int> { i };
                comp[i] = id; int size = 0;
                while (st.Count > 0) { int c = J.Pop(st); size++; foreach (int q in g.Neighbors(c)) if (comp[q] < 0 && open(q) && land(q) == land(i)) { comp[q] = id; st.Add(q); } }
                info.Add((land(i), size, i));
            }
            // her kara parçasının en büyük bölgesi ana bölgedir
            var main = new Dictionary<int, int>();
            for (int id = 0; id < info.Count; id++) { var c = info[id]; if (!main.TryGetValue(c.Land, out int m) || info[m].Size < c.Size) main[c.Land] = id; }
            var cands = new List<(int Id, int Land, int Size)>();
            for (int id = 0; id < info.Count; id++) cands.Add((id, info[id].Land, info[id].Size));
            var orphans = J.Sort(J.Filter(cands, x => main[x.Land] != x.Id), (a, b) => b.Size - a.Size);
            if (orphans.Count == 0) break;
            var orphan = orphans[0];
            // kova kuyruklu en kısa yol (Dial): açık karo 0, dağ 1, yataklı dağ 4
            int target = main[orphan.Land];
            var dist = new int[n]; Array.Fill(dist, 1 << 30); var prev = new int[n]; Array.Fill(prev, -1);
            var buckets = new List<List<int>> { new List<int>() };
            for (int i = 0; i < n; i++) if (comp[i] == orphan.Id) { dist[i] = 0; buckets[0].Add(i); }
            int hit = -1;
            for (int d = 0; d < buckets.Count && hit < 0; d++)
            {
                var b = buckets[d];
                if (b == null) continue;
                for (int k = 0; k < b.Count && hit < 0; k++)
                {
                    int c = b[k];
                    if (dist[c] != d) continue;
                    if (comp[c] == target) { hit = c; break; }
                    foreach (int nb in g.Neighbors(c))
                    {
                        if (land(nb) != orphan.Land) continue;
                        int nd = d + (open(nb) ? 0 : tiles[nb].Deposit >= 0 ? 4 : 1);
                        if (nd < dist[nb])
                        {
                            dist[nb] = nd; prev[nb] = c;
                            while (buckets.Count <= nd) buckets.Add(null); // JS: seyrek dizi (delikler undefined)
                            (buckets[nd] ??= new List<int>()).Add(nb);
                        }
                    }
                }
            }
            if (hit < 0) break;
            for (int c = hit; c >= 0 && comp[c] != orphan.Id; c = prev[c]) if (tiles[c].Terrain == "mountain") { tiles[c].Terrain = "hill"; tiles[c].Pass = true; carved++; }
        }
        return carved;
    }
}
