using System;
using System.Collections.Generic;
using System.Numerics;

namespace FD.Sim.Life;

/// <summary>Turkish suffix helpers (vowel harmony) for names like "buğday tarlası", "Karataş hanesi".</summary>
public static class TR
{
    static char LastVowel(string s)
    {
        for (int i = s.Length - 1; i >= 0; i--)
            if ("aeıioöuüAEIİOÖUÜ".IndexOf(s[i]) >= 0) return char.ToLowerInvariant(s[i]);
        return 'a';
    }
    static bool Back(string s) => "aıou".IndexOf(LastVowel(s)) >= 0;
    static bool EndsVowel(string s) => s.Length > 0 && "aeıioöuü".IndexOf(char.ToLowerInvariant(s[^1])) >= 0;
    /// <summary>Locative: tarlası → tarlasında, hanesi → hanesinde, köy → köyde.</summary>
    public static string Loc(string s) => s + (EndsVowel(s) ? (Back(s) ? "nda" : "nde") : (Back(s) ? "da" : "de"));
    /// <summary>Dative: tarlası → tarlasına, hanesi → hanesine.</summary>
    public static string Dat(string s) => s + (EndsVowel(s) ? (Back(s) ? "na" : "ne") : (Back(s) ? "a" : "e"));
    /// <summary>Accusative: tarlası → tarlasını, hanesi → hanesini.</summary>
    public static string Acc(string s)
    {
        char v = LastVowel(s);
        string i = v switch { 'a' or 'ı' => "ı", 'e' or 'i' => "i", 'o' or 'u' => "u", _ => "ü" };
        return s + (EndsVowel(s) ? "n" + i : i);
    }
}

/// <summary>
/// Faz 2: what the macro world says about the village (<c>FD.Macro.Local.Info</c>), in LifeSim terms. The 1:1 village keeps its
/// ten houses; who lives in them comes from here: resident count, race mix, the work they do, whether a priest serves the
/// chapel, the inn folk's race and the goblins of the camp. No children (Faz 2 karar: çocuk ve köpek kalkar).
/// </summary>
public sealed class VillageSpec
{
    /// <summary>adult residents of the village (spread over the ten houses)</summary>
    public int Residents = 34;
    /// <summary>race id → weight (macro population)</summary>
    public readonly List<(string race, float w)> Races = new() { ("human", 1f) };
    /// <summary>work weights: Farmer, Woodcutter, Smith, Shepherd (others are derived)</summary>
    public readonly Dictionary<Role, float> Work = new() { [Role.Farmer] = 20, [Role.Woodcutter] = 4, [Role.Smith] = 2, [Role.Shepherd] = 2 };
    public bool Priest = true;
    public string InnRace = "human";
    public int Goblins = 6;
    public bool GoblinBoss;
    /// <summary>(race, female, rng) → given name; null: built-in pools</summary>
    public Func<string, bool, Rng, string> Given;
    /// <summary>(race, rng) → family name; null: built-in pools</summary>
    public Func<string, Rng, string> Surname;
    /// <summary>race → (adult from, old from, max) in years; null: human (17, 60, 85)</summary>
    public Func<string, (int adult, int old, int max)> Ages;

    /// <summary>Faz 2 E: adventurers staying at the inn (free heroes of the macro world first, mercenaries otherwise)</summary>
    public readonly List<GuestSpec> Guests = new();

    public static VillageSpec Default() => new();
}

/// <summary>One adventurer at the inn (Faz 2 E).</summary>
public sealed class GuestSpec
{
    public string Name, Surname, Race = "human", Cls = "fighter", Align = "neutral";
    public bool Female;
    public int Age = 25, Level = 1, HeroId = -1;
    /// <summary>can be hired (fighters and rogues)</summary>
    public bool ForHire = true;
}

/// <summary>Who lives where and does what. Faz 2: households are built from <see cref="VillageSpec"/> (the macro village).</summary>
public static class Census
{
    static readonly string[] Male =
    {
        "Aldric", "Tomas", "Gareth", "Rowan", "Bram", "Cedric", "Osric", "Tobin", "Bertram", "Fenwick", "Dursun", "Rahmi",
        "Hamza", "Edmund", "Halvard", "Selim", "Oswin", "Kerem", "Ulric", "Anselm", "Merrik", "Tarık", "Wendel", "Baran",
    };
    static readonly string[] Female =
    {
        "Mira", "Elena", "Selene", "Isolde", "Lysa", "Maren", "Hilde", "Marta", "Yelda", "Ottilie", "Gülizar", "Nimet",
        "Wilda", "Ayla", "Brida", "Elif", "Rosamund", "Sevda", "Liesel", "Nesrin", "Agnes", "Fidan", "Greta", "Leyla",
    };
    static readonly string[] Surnames =
    {
        "Karataş", "Değirmenci", "Tepeli", "Dereli", "Söğüt", "Bağcı", "Kuyucu", "Akbaba", "Ormanlı", "Yıldırım", "Ekinci", "Kestane",
    };

    /// <summary>Population plan: how many of each role, in house order. House 0 is the headman's large house; 1 smith, 2 shepherd,
    /// 3 woodcutter, the rest farmers (the brain finds the workplaces by role).</summary>
    public static List<Role>[] Plan(VillageSpec spec, int homeCount)
    {
        int n = Math.Clamp(spec.Residents, 8, 46);
        float W(Role r) => spec.Work.TryGetValue(r, out var v) ? MathF.Max(0, v) : 0;
        float total = MathF.Max(1, W(Role.Farmer) + W(Role.Woodcutter) + W(Role.Smith) + W(Role.Shepherd));
        int smiths = W(Role.Smith) > 0 || n >= 20 ? 1 : 0;
        int apprentices = smiths > 0 && n >= 24 && W(Role.Smith) / total > 0.06f ? 1 : 0;
        int shepherds = 1 + (n >= 22 && W(Role.Shepherd) / total > 0.08f ? 1 : 0);
        int woodcutters = Math.Clamp((int)MathF.Round(n * W(Role.Woodcutter) / total), 1, 3);
        int elders = (int)MathF.Round(n * 0.12f);
        int homemakers = (int)MathF.Round(n * 0.17f);
        int farmers = Math.Max(2, n - 1 - smiths - apprentices - shepherds - woodcutters - elders - homemakers);
        var houses = new List<Role>[Math.Max(1, homeCount)];
        for (int i = 0; i < houses.Length; i++) houses[i] = new List<Role>();
        void Put(int h, Role r, int k = 1) { for (int i = 0; i < k; i++) houses[Math.Min(h, houses.Length - 1)].Add(r); }
        Put(0, Role.Headman);
        if (smiths > 0) { Put(1, Role.Smith); Put(1, Role.Apprentice, apprentices); }
        Put(2, Role.Shepherd, shepherds);
        Put(3, Role.Woodcutter, woodcutters);
        // farmers: two per farm house first, then round robin over all houses except the smithy's
        var order = new List<int>();
        for (int i = 4; i < houses.Length; i++) order.Add(i);
        order.Add(0);
        for (int i = 1; i < Math.Min(4, houses.Length); i++) order.Add(i);
        int k2 = 0;
        for (int i = 0; i < farmers; i++) { Put(order[k2 % order.Count], Role.Farmer); k2++; }
        // homemakers and elders into the houses with the most working members
        for (int i = 0; i < homemakers; i++) Put(Smallest(houses, true), Role.Homemaker);
        for (int i = 0; i < elders; i++) Put(Smallest(houses, false), Role.Elder);
        return houses;
    }

    static int Smallest(List<Role>[] houses, bool workersFirst)
    {
        int best = 0; float bs = float.MaxValue;
        for (int i = 0; i < houses.Length; i++)
        {
            int c = houses[i].Count;
            if (c == 0) continue;
            float s = c + (workersFirst && houses[i].Contains(Role.Homemaker) ? 2.5f : 0) + (!workersFirst && houses[i].Contains(Role.Elder) ? 2.5f : 0) + i * 0.01f;
            if (s < bs) { bs = s; best = i; }
        }
        return best;
    }

    public static void Populate(LifeSim w, ulong seed, VillageSpec spec = null)
    {
        spec ??= VillageSpec.Default();
        var rng = new Rng(seed);
        var used = new HashSet<string>();
        string Given(string race, bool female)
        {
            for (int i = 0; i < 40; i++)
            {
                string n = spec.Given != null ? spec.Given(race, female, rng) : rng.Pick(female ? Female : Male);
                if (used.Add(n)) return n;
            }
            return spec.Given != null ? spec.Given(race, female, rng) : rng.Pick(female ? Female : Male);
        }
        var famUsed = new HashSet<string>();
        string Family(string race)
        {
            for (int i = 0; i < 30; i++)
            {
                string n = spec.Surname != null ? spec.Surname(race, rng) : rng.Pick(Surnames);
                if (famUsed.Add(n)) return n;
            }
            return spec.Surname != null ? spec.Surname(race, rng) : rng.Pick(Surnames);
        }
        string Race()
        {
            float tot = 0; foreach (var (_, wt) in spec.Races) tot += MathF.Max(0, wt);
            if (tot <= 0) return "human";
            float x = rng.Next01() * tot;
            foreach (var (r, wt) in spec.Races) { x -= MathF.Max(0, wt); if (x <= 0) return r; }
            return spec.Races[^1].race;
        }
        (int adult, int old, int max) AgesOf(string race) => spec.Ages?.Invoke(race) ?? (17, 60, 85);
        int AgeFor(string race, Role role)
        {
            var (ad, old, mx) = AgesOf(race);
            if (role == Role.Elder) return old + (int)(rng.Next01() * Math.Max(1, (mx - old) * 0.8f));
            if (role == Role.Apprentice) return ad + (int)(rng.Next01() * Math.Max(1, (old - ad) * 0.12f));
            if (role == Role.Headman) return ad + (int)((old - ad) * (0.55f + rng.Next01() * 0.4f));
            return ad + (int)(rng.Next01() * Math.Max(1, (old - ad) * 0.9f));
        }

        var homes = w.PlacesOf(PlaceKind.Home);
        var fields = w.PlacesOf(PlaceKind.Field);
        var plan = Plan(spec, homes.Count);
        // race tokens matching the macro mix (largest remainder), grouped by race so households stay mostly of one people
        int nPeople = 0; foreach (var hl in plan) nPeople += hl.Count;
        var tokens = RaceTokens(spec, nPeople);
        int tok = 0;
        string[] trade = { "Muhtar", "Demirci", "Çoban", "Oduncu" };
        int fieldIdx = 0;
        for (int hIdx = 0; hIdx < plan.Length && hIdx < homes.Count; hIdx++)
        {
            var roles = plan[hIdx];
            if (roles.Count == 0) { homes[hIdx].Name = "boş ev"; continue; }
            string race = tok < tokens.Count ? tokens[tok] : Race();
            string tr = hIdx < trade.Length && (hIdx != 1 || roles.Contains(Role.Smith)) ? trade[hIdx] : "Çiftçi";
            var hh = new Household { Id = w.Households.Count, Surname = Family(race), Home = homes[hIdx].Id, Trade = tr };
            w.Households.Add(hh);
            homes[hIdx].Owner = hh.Id;
            homes[hIdx].Name = $"{hh.Surname} hanesi";
            bool firstFemale = rng.Chance(0.3f);
            for (int m = 0; m < roles.Count; m++)
            {
                var role = roles[m];
                string r = tok < tokens.Count ? tokens[tok] : race;
                tok++;
                bool female = role == Role.Homemaker ? rng.Chance(0.8f) : m == 0 ? firstFemale : rng.Chance(0.45f);
                var p = NewPerson(w, role, female, AgeFor(r, role), Given(r, female), hh.Surname, rng);
                p.Race = r;
                p.Household = hh.Id;
                p.Home = homes[hIdx].Id;
                hh.Members.Add(p.Id);
            }
            // fields: headman 2, farm houses 1 (round robin), others none
            int want = hIdx == 0 ? 2 : roles.Contains(Role.Farmer) && hIdx >= 4 ? 1 : 0;
            for (int k = 0; k < want && fields.Count > 0 && fieldIdx < fields.Count; k++) { hh.Fields.Add(fields[fieldIdx].Id); fields[fieldIdx].Owner = hh.Id; fieldIdx++; }
        }
        // leftover fields go to the houses with the most farmers
        for (int guard = 0; fieldIdx < fields.Count && guard < 100; guard++)
        {
            Household best = null; int bf = 0;
            foreach (var h in w.Households)
            {
                int f = 0; foreach (int id in h.Members) if (w.People[id].Role == Role.Farmer) f++;
                f -= h.Fields.Count;
                if (best == null || f > bf) { best = h; bf = f; }
            }
            if (best == null) break;
            best.Fields.Add(fields[fieldIdx].Id); fields[fieldIdx].Owner = best.Id; fieldIdx++;
        }

        // priest lives in the chapel
        var chapel = w.PlaceOf(PlaceKind.Chapel);
        if (chapel != null && spec.Priest)
        {
            string race = Race();
            var pr = NewPerson(w, Role.Priest, false, AgeFor(race, Role.Headman), Given(race, false), "", rng);
            pr.Race = race;
            pr.Name = "Rahip " + pr.Name;
            pr.Home = chapel.Id;
        }
        // inn folk
        var inn = w.PlaceOf(PlaceKind.Inn);
        if (inn != null)
        {
            var hh = new Household { Id = w.Households.Count, Surname = Family(spec.InnRace), Home = inn.Id, Trade = "Hancı" };
            w.Households.Add(hh);
            foreach (var (role, fem) in new[] { (Role.Innkeeper, false), (Role.InnServant, true), (Role.InnServant, true), (Role.StableHand, false) })
            {
                string race = role == Role.Innkeeper || rng.Chance(0.7f) ? spec.InnRace : Race();
                var p = NewPerson(w, role, fem, role == Role.StableHand ? AgeFor(race, Role.Apprentice) : AgeFor(race, role), Given(race, fem), hh.Surname, rng);
                p.Race = race;
                p.Household = hh.Id; p.Home = inn.Id;
                hh.Members.Add(p.Id);
            }
        }
        // visitors
        var v1 = NewPerson(w, Role.Merchant, false, 44, "Yusuf", "", rng); v1.Name = "Tüccar Yusuf"; v1.IsVisitor = true; v1.HasCart = true; v1.Present = false;
        var v2 = NewPerson(w, Role.Pilgrim, true, 57, "Sister", "", rng); v2.Name = "Hacı Theodora"; v2.IsVisitor = true; v2.Present = false;
        var v3 = NewPerson(w, Role.Adventurer, false, 27, "", "", rng); v3.Name = "Kara Rowena"; v3.Female = true; v3.IsVisitor = true; v3.Present = false; v3.WalkSpeed = 1.5f;
        // Faz 2 E: adventurers staying at the inn
        int gi = 0;
        foreach (var g in spec.Guests)
        {
            var gp = NewPerson(w, Role.Adventurer, g.Female, g.Age, g.Name, g.Surname ?? "", rng);
            gp.Race = g.Race; gp.IsVisitor = true; gp.Guest = true; gp.HeroId = g.HeroId; gp.Cls = g.Cls; gp.Level = g.Level; gp.Align = g.Align;
            gp.Work = gi++;
            gp.WalkSpeed = 1.45f;
        }
        // goblins of the camp
        var camp = w.PlaceOf(PlaceKind.Camp);
        if (camp != null)
        {
            string[] gn = { "Snik", "Grubb", "Yazz", "Morg", "Kıtır", "Pıskı", "Zıbık", "Gırtlak", "Nuk", "Pöt", "Hırk", "Zort" };
            int count = Math.Clamp(spec.Goblins, 0, gn.Length);
            for (int i = 0; i < count; i++)
            {
                var g = NewPerson(w, Role.Goblin, false, 9 + i, gn[i], "", rng);
                g.Race = "goblin";
                g.Work = i;
                g.Home = camp.Id;
                g.WalkSpeed = 1.1f; g.RunSpeed = 3.8f;
                g.Post = camp.Center;
            }
            if (spec.GoblinBoss)
            {
                var b = NewPerson(w, Role.Goblin, false, 30, "Kara Gırnak", "", rng);
                b.Race = "goblin"; b.IsBoss = true;
                b.Work = 0;   // a guard's day; stays near the fire
                b.Home = camp.Id;
                b.WalkSpeed = 1.1f; b.RunSpeed = 3.6f;
                b.Post = camp.Center;
            }
        }
        w.Flock.Init(w, 11, seed + 5);
    }

    /// <summary>n race ids in the macro proportions (largest remainder), majority first.</summary>
    static List<string> RaceTokens(VillageSpec spec, int n)
    {
        var o = new List<string>();
        float tot = 0; foreach (var (_, wt) in spec.Races) tot += MathF.Max(0, wt);
        if (tot <= 0 || n <= 0) return o;
        var rem = new List<(string r, float frac)>();
        foreach (var (r, wt) in spec.Races)
        {
            float exact = n * MathF.Max(0, wt) / tot;
            int k = (int)MathF.Floor(exact);
            for (int i = 0; i < k; i++) o.Add(r);
            rem.Add((r, exact - k));
        }
        rem.Sort((a, b) => b.frac.CompareTo(a.frac));
        for (int i = 0; o.Count < n && i < rem.Count; i++) o.Add(rem[i].r);
        var order = new List<string>(); foreach (var (r, _) in spec.Races) order.Add(r);
        o.Sort((a, b) => order.IndexOf(a).CompareTo(order.IndexOf(b)));
        return o;
    }

    static Person NewPerson(LifeSim w, Role role, bool female, int age, string first, string surname, Rng rng)
    {
        var p = new Person
        {
            Id = w.People.Count, Role = role, Female = female, Age = age, Name = first, Surname = surname,
            Look = (int)(rng.NextU() & 0x7fffffff),
            LaneOffset = 0.55f + rng.Next01() * 0.6f,
        };
        p.WalkSpeed = role switch
        {
            Role.Child => 1.25f + rng.Next01() * 0.2f,
            Role.Elder => 0.95f + rng.Next01() * 0.15f,
            _ => 1.28f + rng.Next01() * 0.18f,
        };
        p.RunSpeed = role == Role.Child ? 3.2f : role == Role.Elder ? 2.2f : 4.2f;
        p.Food = 0.6f + rng.Next01() * 0.4f;
        p.Energy = 0.7f + rng.Next01() * 0.3f;
        p.Social = 0.4f + rng.Next01() * 0.5f;
        w.People.Add(p);
        return p;
    }

    static string ClassTr(string c) => c switch { "fighter" => "savaşçı", "rogue" => "haydut", "wizard" => "büyücü", "cleric" => "rahip", "paladin" => "paladin", "ranger" => "korucu", "barbarian" => "barbar", "druid" => "druid", "bard" => "ozan", "monk" => "keşiş", "warlock" => "cadı", "sorcerer" => "sihirbaz", _ => c ?? "maceracı" };

    public static string RoleName(Person p) => p.Role switch
    {
        Role.Farmer => p.Female ? "Çiftçi (kadın)" : "Çiftçi",
        Role.Homemaker => p.Female ? "Ev hanımı" : "Evin işlerini gören",
        Role.Woodcutter => "Oduncu",
        Role.Smith => "Demirci ustası",
        Role.Apprentice => "Demirci çırağı",
        Role.Shepherd => p.Age < 16 ? "Çoban yamağı" : "Çoban",
        Role.Headman => "Köy muhtarı",
        Role.Priest => "Rahip",
        Role.Innkeeper => "Hancı",
        Role.InnServant => "Han hizmetkârı",
        Role.StableHand => "Seyis",
        Role.Child => p.Age < 6 ? "Küçük çocuk" : "Çocuk",
        Role.Elder => "Yaşlı",
        Role.Merchant => "Gezgin tüccar",
        Role.Pilgrim => "Hacı",
        Role.Adventurer => p.Guest ? $"Maceracı · {ClassTr(p.Cls)} Sv{p.Level}{(p.HeroId < 0 ? " · paralı asker" : "")}" : "Maceracı (korucu)",
        Role.Goblin => p.IsBoss ? "Goblin şefi" : "Goblin",
        _ => p.Role.ToString(),
    };
}

/// <summary>The village flock: grazes on the pasture by day (following the shepherd there and back), penned at night.</summary>
public sealed class Flock
{
    public readonly List<Vector2> Pos = new();
    public readonly List<Vector2> Dir = new();
    public readonly List<float> Speed = new();
    readonly List<Vector2> _goal = new();
    readonly List<float> _timer = new();
    Rng _rng;
    int _shepherd = -1;
    Place _pen, _pasture;

    public void Init(LifeSim w, int count, ulong seed)
    {
        _rng = new Rng(seed);
        _pen = w.PlaceOf(PlaceKind.Pen);
        _pasture = w.PlaceOf(PlaceKind.Pasture);
        foreach (var p in w.People) if (p.Role == Role.Shepherd && p.Age >= 16) { _shepherd = p.Id; break; }
        for (int i = 0; i < count; i++)
        {
            Pos.Add(Vector2.Zero); Dir.Add(new Vector2(0, 1)); Speed.Add(0); _goal.Add(Vector2.Zero); _timer.Add(0);
        }
    }

    Place Area(LifeSim w, out bool follow, out Vector2 lead)
    {
        follow = false; lead = Vector2.Zero;
        if (_shepherd < 0 || _pen == null) return _pen ?? _pasture;
        var s = w.People[_shepherd];
        lead = s.Pos;
        if (s.Act == null) return _pen;
        if (s.Act.Kind == ActKind.Herd || s.Act.Kind == ActKind.Eat)
        {
            if (s.Motion == Motion.Walking && (s.Act.Place == _pasture?.Id || s.Act.Place == _pen?.Id) && s.SubTargetTimer == 0) follow = true;
            var target = s.Act.Place >= 0 ? w.Places[s.Act.Place] : null;
            if (target?.Kind == PlaceKind.Pasture) return _pasture;
            return _pen;
        }
        return _pen;
    }

    public void Resync(LifeSim w)
    {
        if (Pos.Count == 0) return;
        var area = Area(w, out _, out _);
        if (area == null) return;
        for (int i = 0; i < Pos.Count; i++)
        {
            Pos[i] = area.RandomPoint(_rng, 1.2f);
            _goal[i] = Pos[i];
            float a = _rng.Range(0, MathF.Tau);
            Dir[i] = new Vector2(MathF.Cos(a), MathF.Sin(a));
        }
    }

    public void Update(LifeSim w, float dt)
    {
        if (Pos.Count == 0 || dt <= 0) return;
        var area = Area(w, out bool follow, out Vector2 lead);
        for (int i = 0; i < Pos.Count; i++)
        {
            _timer[i] -= dt;
            Vector2 goal;
            float maxSpeed;
            if (follow)
            {
                // trail behind the shepherd in a loose column
                Vector2 back = -w.People[_shepherd].Dir;
                goal = lead + back * (2.5f + i * 0.9f) + H.Perp(back) * ((i % 3) - 1) * 1.1f;
                maxSpeed = 1.6f;
            }
            else
            {
                if (area != null && (!area.Contains(_goal[i], -1f) || _timer[i] <= 0))
                {
                    _goal[i] = area.RandomPoint(_rng, 1.5f);
                    _timer[i] = _rng.Range(6f, 22f);
                }
                goal = _goal[i];
                maxSpeed = area != null && !area.Contains(Pos[i], 0.5f) ? 1.4f : 0.45f;
            }
            Vector2 d = goal - Pos[i];
            float L = d.Length();
            // separation
            Vector2 sep = Vector2.Zero;
            for (int j = 0; j < Pos.Count; j++)
            {
                if (j == i) continue;
                Vector2 o = Pos[i] - Pos[j];
                float ol = o.Length();
                if (ol < 1.3f && ol > 1e-4f) sep += o / ol * (1.3f - ol);
            }
            Vector2 want = (L > 0.4f ? d / L * MathF.Min(maxSpeed, L * 0.6f) : Vector2.Zero) + sep * 1.2f;
            float sp = want.Length();
            Speed[i] = sp;
            if (sp > 0.02f)
            {
                Pos[i] += want * dt;
                Dir[i] = Vector2.Normalize(Vector2.Lerp(Dir[i], want / sp, MathF.Min(1, dt * 3f)));
            }
        }
    }
}
