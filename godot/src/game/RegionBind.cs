using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Generic;
using FD.Sim.Life;
using M = FD.Macro;

namespace FD.Game;

/// <summary>
/// Faz 2: macro village → 1:1 village. Turns <see cref="M.VillageInfo"/> (population, race mix, jobs, faith) into a
/// <see cref="VillageSpec"/> for <see cref="Census.Populate"/>; names and ages come from the macro tables (HERO_NAMES,
/// HERO_SURNAMES, HERO_AGE) so a dwarf village is full of Örsdövens with long lives.
/// </summary>
public static class RegionBind
{
    /// <summary>local residents = macro population × this (one sim person ≈ one villager), clamped</summary>
    public const float ResidentScale = 0.9f;
    public const int MinResidents = 12, MaxResidents = 42;
    /// <summary>the 1:1 camp (three tents, one fire) shows at most this many goblins; a bigger band keeps the rest out raiding</summary>
    public const int MaxGoblins = 10;

    public static VillageSpec Spec(Session s)
    {
        var spec = new VillageSpec();
        var vi = s.Village();
        if (vi == null) return spec;
        spec.Residents = Math.Clamp((int)MathF.Round((float)vi.Pop * ResidentScale), MinResidents, MaxResidents);
        spec.Races.Clear();
        foreach (var kv in vi.Races) spec.Races.Add((kv.Key, (float)kv.Value));
        spec.Races.Sort((a, b) => b.w.CompareTo(a.w));
        if (spec.Races.Count == 0) spec.Races.Add((vi.Race ?? "human", 1f));
        // work: macro job slots by what they produce
        float farm = 0, wood = 0, smith = 0, herd = 0;
        foreach (var (job, n0) in vi.Jobs)
        {
            float n = (float)n0;
            switch (job)
            {
                case "grain": case "fish": case "toplayıcı": case "herbs": farm += n; break;
                case "wood": case "stone": case "copper": case "tin": case "iron": case "gold": case "mithril": case "salt": case "mana": wood += n; break;
                case "meat": case "leather": case "horses": herd += n; break;
                case "zanaatçı": case "amele": case "inşaatçı": case "asker": farm += n * 0.4f; break;
                default: smith += n; break;   // atölyeler (fırın, demirhane, dokuma…)
            }
        }
        spec.Work[Role.Farmer] = MathF.Max(1, farm);
        spec.Work[Role.Woodcutter] = wood;
        spec.Work[Role.Smith] = smith;
        spec.Work[Role.Shepherd] = herd;
        spec.Priest = !vi.Faith.TryGetValue("sun", out var sun) || sun >= 0.12;
        var inn = s.Inn;
        spec.InnRace = !string.IsNullOrEmpty(inn?.KeeperRace) ? inn.KeeperRace : spec.Races[0].race;
        var cp = s.Camp;
        spec.Goblins = cp == null ? 6 : !cp.Alive ? 0 : Math.Clamp((int)Math.Round(cp.Count), 2, MaxGoblins);
        spec.GoblinBoss = cp != null && cp.Alive && cp.Boss;
        Guests(s, spec);
        spec.Given = Given;
        spec.Surname = Family;
        spec.Ages = Ages;
        return spec;
    }

    public const int MaxGuests = 3, MinForHire = 2;

    /// <summary>Faz 2 E: the inn's adventurers: free heroes waiting at the bound inn in the macro world (highest level first, at most
    /// three), then mercenaries (fighters and rogues of the village's peoples, level 1) until two can be hired. The party's own
    /// members are not here.</summary>
    static void Guests(Session s, VillageSpec spec)
    {
        var party = new HashSet<int>();
        foreach (var c in s.Party) if (c.HeroId is int id) party.Add(id);
        var heroes = M.Local.InnHeroes(s.Macro).Where(h => !party.Contains(h.Id)).OrderByDescending(h => h.Level).ThenBy(h => h.Id).Take(MaxGuests).ToList();
        var day = s.Macro.W.Day;
        int hireable = 0;
        foreach (var h in heroes)
        {
            var mapped = FD.Rpg.CharacterFactory.LocalClass(h.Cls);
            bool forHire = mapped is "fighter" or "rogue";
            if (forHire) hireable++;
            var age = Ages(h.Race);
            spec.Guests.Add(new GuestSpec
            {
                Name = string.IsNullOrEmpty(h.Given) ? h.Name.Split(' ')[0] : h.Given, Surname = h.Surname ?? "", Race = h.Race, Cls = h.Cls,
                Level = (int)h.Level, Align = h.Align ?? "neutral", HeroId = h.Id, ForHire = forHire,
                Female = FD.Rpg.CharacterFactory.IsFemaleName(h), Age = age.adult + 3 + (int)h.Level * 2,
            });
        }
        var rng = new Rng((ulong)(Math.Abs(s.Seed) * 1000 + day));
        for (int k = 0; hireable < MinForHire && spec.Guests.Count < MaxGuests; k++)
        {
            string race = spec.Races[0].race;
            float tot = spec.Races.Sum(r => r.w), x = rng.Next01() * tot;
            foreach (var (r, w) in spec.Races) { if ((x -= w) <= 0) { race = r; break; } }
            if (race == "goblin") race = "human";
            bool female = rng.Chance(0.35f);
            float a = rng.Next01();
            spec.Guests.Add(new GuestSpec
            {
                Name = Given(race, female, rng), Surname = Family(race, rng), Race = race, Female = female, Cls = k % 2 == 0 ? "fighter" : "rogue",
                Level = 1, Align = a < 0.4f ? "good" : a < 0.85f ? "neutral" : "evil", HeroId = -1, ForHire = true, Age = Ages(race).adult + 2 + rng.Int(8),
            });
            hireable++;
        }
    }

    /// <summary>Given name from the macro pool of the race: even entries male, odd entries female.</summary>
    public static string Given(string race, bool female, Rng rng)
    {
        var pool = M.D.HERO_NAMES.GetOr(race, null) ?? M.D.HERO_NAMES["human"];
        int half = Math.Max(1, pool.Count / 2);
        int k = rng.Int(half) * 2 + (female ? 1 : 0);
        return pool[Math.Min(k, pool.Count - 1)];
    }

    public static string Family(string race, Rng rng)
    {
        var pool = M.D.HERO_SURNAMES.GetOr(race, null) ?? M.D.HERO_SURNAMES["human"];
        return pool[rng.Int(pool.Count)];
    }

    /// <summary>(adult from, old from, max): HERO_AGE = [yola çıkış, yola çıkış üst, yaşlılık, ömür].</summary>
    public static (int adult, int old, int max) Ages(string race)
    {
        var a = M.D.HERO_AGE.GetOr(race, null) ?? M.D.HERO_AGE["human"];
        return ((int)a[0], (int)a[2], (int)a[3]);
    }

    /// <summary>Turkish race name (singular), e.g. "Cüce".</summary>
    public static string RaceName(string race) =>
        race == "goblin" ? "Goblin" : M.D.RACES.GetOr(race, null)?.Name ?? race;
}
