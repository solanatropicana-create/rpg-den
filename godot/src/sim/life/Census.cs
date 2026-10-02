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

/// <summary>Who lives where and does what. Fixed household templates (varied, believable) filled with names.</summary>
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

    record Tmpl(Role Role, bool Female, int Age);

    /// <summary>Household templates in home order: 0 headman (large house), 1 smith, 2 shepherd, 3 woodcutter, 4+ farmers.</summary>
    static readonly (string trade, Tmpl[] members)[] Houses =
    {
        ("Muhtar", new Tmpl[] { new(Role.Headman, false, 52), new(Role.Homemaker, true, 48), new(Role.Farmer, false, 19), new(Role.Child, true, 10), new(Role.Elder, true, 74) }),
        ("Demirci", new Tmpl[] { new(Role.Smith, false, 41), new(Role.Homemaker, true, 38), new(Role.Apprentice, false, 15), new(Role.Child, true, 7) }),
        ("Çoban", new Tmpl[] { new(Role.Shepherd, false, 45), new(Role.Homemaker, true, 40), new(Role.Shepherd, false, 13), new(Role.Child, true, 6) }),
        ("Oduncu", new Tmpl[] { new(Role.Woodcutter, false, 38), new(Role.Woodcutter, false, 18), new(Role.Homemaker, true, 36), new(Role.Child, false, 8) }),
        ("Çiftçi", new Tmpl[] { new(Role.Farmer, false, 35), new(Role.Farmer, true, 33), new(Role.Child, false, 9), new(Role.Child, true, 6), new(Role.Elder, false, 70) }),
        ("Çiftçi", new Tmpl[] { new(Role.Farmer, false, 29), new(Role.Homemaker, true, 27), new(Role.Child, false, 4) }),
        ("Çiftçi", new Tmpl[] { new(Role.Farmer, false, 47), new(Role.Farmer, true, 44), new(Role.Farmer, false, 20), new(Role.Child, true, 12) }),
        ("Çiftçi", new Tmpl[] { new(Role.Farmer, false, 58), new(Role.Homemaker, true, 55), new(Role.Elder, true, 80), new(Role.Farmer, true, 24) }),
        ("Çiftçi", new Tmpl[] { new(Role.Farmer, false, 31), new(Role.Farmer, true, 30), new(Role.Child, false, 7), new(Role.Child, true, 5), new(Role.Child, false, 3) }),
        ("Çiftçi", new Tmpl[] { new(Role.Farmer, true, 42), new(Role.Farmer, false, 17), new(Role.Child, true, 11), new(Role.Elder, false, 69) }),
    };

    public static void Populate(LifeSim w, ulong seed)
    {
        var rng = new Rng(seed);
        var usedM = new HashSet<string>();
        var usedF = new HashSet<string>();
        string First(bool female)
        {
            var pool = female ? Female : Male;
            var used = female ? usedF : usedM;
            for (int i = 0; i < 50; i++) { var n = rng.Pick(pool); if (used.Add(n)) return n; }
            return rng.Pick(pool);
        }
        var homes = w.PlacesOf(PlaceKind.Home);
        var fields = w.PlacesOf(PlaceKind.Field);
        var surnames = new List<string>(Surnames);
        // trade-matching surnames first so the smith is a Demirci etc.
        string Surname(string trade)
        {
            // trades carry their trade name (the smith's family are the Demircis …); others draw from the list
            if (trade is "Demirci" or "Çoban" or "Oduncu") return trade;
            if (surnames.Count == 0) surnames.AddRange(Surnames);
            var pick = surnames[rng.Int(surnames.Count)];
            surnames.Remove(pick);
            return pick;
        }
        int fieldIdx = 0;
        for (int hIdx = 0; hIdx < Houses.Length && hIdx < homes.Count; hIdx++)
        {
            var (trade, members) = Houses[hIdx];
            var hh = new Household { Id = w.Households.Count, Surname = Surname(trade), Home = homes[hIdx].Id, Trade = trade };
            w.Households.Add(hh);
            homes[hIdx].Owner = hh.Id;
            homes[hIdx].Name = $"{hh.Surname} hanesi";
            foreach (var m in members)
            {
                var p = NewPerson(w, m.Role, m.Female, m.Age, First(m.Female), hh.Surname, rng);
                p.Household = hh.Id;
                p.Home = homes[hIdx].Id;
                hh.Members.Add(p.Id);
            }
            // fields: headman 2, farmers 1–2 (round robin), others none
            int want = trade == "Muhtar" ? 2 : trade == "Çiftçi" ? 1 : 0;
            for (int k = 0; k < want && fields.Count > 0; k++) { hh.Fields.Add(fields[fieldIdx % fields.Count].Id); fields[fieldIdx % fields.Count].Owner = hh.Id; fieldIdx++; }
        }
        // leftover fields go to the biggest farming families
        for (int hIdx = 4; fieldIdx < fields.Count && hIdx < w.Households.Count; hIdx++, fieldIdx++)
        {
            w.Households[hIdx].Fields.Add(fields[fieldIdx].Id);
            fields[fieldIdx].Owner = w.Households[hIdx].Id;
        }

        // priest lives in the chapel
        var chapel = w.PlaceOf(PlaceKind.Chapel);
        if (chapel != null)
        {
            var pr = NewPerson(w, Role.Priest, false, 61, "Anselm", "", rng);
            pr.Name = "Rahip Anselm";
            pr.Home = chapel.Id;
        }
        // inn folk
        var inn = w.PlaceOf(PlaceKind.Inn);
        if (inn != null)
        {
            var hh = new Household { Id = w.Households.Count, Surname = "Taşkın", Home = inn.Id, Trade = "Hancı" };
            w.Households.Add(hh);
            foreach (var (role, fem, age) in new[] { (Role.Innkeeper, false, 50), (Role.InnServant, true, 47), (Role.InnServant, true, 22), (Role.StableHand, false, 16) })
            {
                var p = NewPerson(w, role, fem, age, First(fem), hh.Surname, rng);
                p.Household = hh.Id; p.Home = inn.Id;
                hh.Members.Add(p.Id);
            }
        }
        // visitors
        var v1 = NewPerson(w, Role.Merchant, false, 44, "Yusuf", "", rng); v1.Name = "Tüccar Yusuf"; v1.IsVisitor = true; v1.HasCart = true; v1.Present = false;
        var v2 = NewPerson(w, Role.Pilgrim, true, 57, "Sister", "", rng); v2.Name = "Hacı Theodora"; v2.IsVisitor = true; v2.Present = false;
        var v3 = NewPerson(w, Role.Adventurer, false, 27, "", "", rng); v3.Name = "Kara Rowena"; v3.Female = true; v3.IsVisitor = true; v3.Present = false; v3.WalkSpeed = 1.5f;
        // goblins of the camp
        var camp = w.PlaceOf(PlaceKind.Camp);
        if (camp != null)
        {
            string[] gn = { "Snik", "Grubb", "Yazz", "Morg", "Kıtır", "Pıskı" };
            for (int i = 0; i < 6; i++)
            {
                var g = NewPerson(w, Role.Goblin, false, 9 + i, gn[i], "", rng);
                g.Work = i;
                g.Home = camp.Id;
                g.WalkSpeed = 1.1f; g.RunSpeed = 3.8f;
                g.Post = camp.Center;
            }
        }
        w.Flock.Init(w, 11, seed + 5);
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

    public static string RoleName(Person p) => p.Role switch
    {
        Role.Farmer => p.Female ? "Çiftçi (kadın)" : "Çiftçi",
        Role.Homemaker => "Ev hanımı",
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
        Role.Adventurer => "Maceracı (korucu)",
        Role.Goblin => "Goblin",
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
