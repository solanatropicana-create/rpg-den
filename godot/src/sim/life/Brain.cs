using System;
using System.Collections.Generic;
using System.Numerics;

namespace FD.Sim.Life;

/// <summary>
/// Daily life rules. Every choice has a reason a player can read on the person's card: the time of day, the
/// person's role and household, their needs. Deterministic: randomness comes from hashes of (person, day, slot)
/// so re-deciding at the same moment gives the same answer.
/// </summary>
public sealed class Brain
{
    readonly LifeSim _w;
    public Brain(LifeSim w) { _w = w; }

    // current decision context
    Person _p;
    bool _peek;
    double _now;
    int _day;
    float _h;

    float R(int salt) => H.Hash(_p.Id, _day, salt);
    /// <summary>Faz 2: game hours to walk from a to b (path ≈ 1.25 × straight line) at this person's pace and the clock's time scale.</summary>
    float TravelH(Vector2 a, Vector2 b) => Vector2.Distance(a, b) * 1.25f / MathF.Max(0.5f, _p.WalkSpeed) * MathF.Max(0f, _w.TimeScale) / 3600f;
    double At(float hour) => _day * 1440.0 + hour * 60.0;
    double Mins(float m) => _now + m;

    public Activity Peek(Person p, double now) => Choose(p, now, peek: true);

    public Activity Choose(Person p, double now, bool peek = false)
    {
        _p = p; _peek = peek; _now = now; _day = (int)Math.Floor(now / 1440.0); _h = (float)(now - _day * 1440.0) / 60f;
        if (p.IsVisitor) return Visitor();
        if (p.Role == Role.Goblin) return Goblin();

        float wake = WakeHour(), bed = BedHour();
        // Faz 2: leave for home early enough to be in bed by bedtime (a 30-minute day makes every walk long in game hours)
        float homeWalk = Home != null ? MathF.Min(12f, TravelH(_p.Pos, Home.Door)) : 0f;
        if (_h >= bed - homeWalk || _h < wake) return Sleep(_h >= bed - homeWalk ? At(24 + wake) : At(wake));
        // just drew water → carry the bucket home first
        if (p.Act?.Kind == ActKind.FetchWater && Home != null && !(p.Act.Place >= 0 && _w.Places[p.Act.Place].Kind == PlaceKind.InnYard))
        {
            var carry = HomeAct("Suyu eve taşıdı", "Kuyudan çektiği suyu evine götürdü.", Mins(15 + R(4) * 20), "Su kovasını eve taşıyor");
            carry.WalkTool = "tool_bucket";
            return carry;
        }
        if (_h < wake + 0.55f) return HomeAct("Kahvaltı ediyor", "Güne başlıyor: evde kahvaltı.", At(wake + 0.55f + R(3) * 0.2f));

        var act = p.Role switch
        {
            Role.Farmer => Farmer(),
            Role.Homemaker => Homemaker(),
            Role.Woodcutter => Woodcutter(),
            Role.Smith => Smith(false),
            Role.Apprentice => Smith(true),
            Role.Shepherd => Shepherd(),
            Role.Headman => Headman(),
            Role.Priest => Priest(),
            Role.Innkeeper or Role.InnServant or Role.StableHand => InnStaff(),
            Role.Child => Child(),
            Role.Elder => Elder(),
            _ => Leisure(bed),
        };
        // Faz 2: whatever they do in the evening ends early enough to walk home by bedtime
        if (act != null && Home != null && act.Kind != ActKind.Sleep && !(act.Inside && act.Place == Home.Id))
        {
            double latest = At(bed - MathF.Min(12f, TravelH(act.Target, Home.Door)));
            // no time to get there, do it and walk home: stay in instead
            if (latest < _now + TravelH(_p.Pos, act.Target) * 60 + 20)
                return HomeAct("Evde oturuyor", "Akşam; yatma vakti yaklaştı, evde vakit geçiriyor.", At(bed));
            if (act.Until > latest) act.Until = latest;
        }
        return act;
    }

    // ------------------------------------------------------------------------------------------------ clocks

    float WakeHour() => _p.Role switch
    {
        Role.Farmer or Role.Shepherd or Role.Woodcutter => 5.4f + R(1) * 0.5f,
        Role.Child => 6.6f + R(1) * 0.5f,
        Role.Elder => 5.9f + R(1) * 0.6f,
        Role.Innkeeper or Role.InnServant or Role.StableHand => 6.0f + R(1) * 0.4f,
        Role.Priest => 5.3f,
        _ => 5.7f + R(1) * 0.6f,
    };

    float BedHour() => _p.Role switch
    {
        Role.Child => 20.2f + R(2) * 0.6f,
        Role.Elder => 20.8f + R(2) * 0.7f,
        Role.Innkeeper => 23.6f,
        Role.InnServant or Role.StableHand => 22.9f + R(2) * 0.4f,
        _ => MathF.Min(22.7f, 21.5f + R(2) * 0.9f + (_p.Social < 0.35f ? 0.5f : 0f)),
    };

    // ------------------------------------------------------------------------------------------------ building blocks

    Place Home => _p.Home >= 0 ? _w.Places[_p.Home] : null;
    Household House => _p.Household >= 0 ? _w.Households[_p.Household] : null;

    Activity Sleep(double until)
    {
        var home = Home;
        return new Activity
        {
            Kind = ActKind.Sleep, Anim = "Idle", Inside = true, Place = home?.Id ?? -1, Target = home?.Door ?? _p.Pos, Face = home?.DoorFace ?? Vector2.Zero,
            Until = until, Label = "Uyuyor", GoLabel = "Eve, yatmaya gidiyor",
            Reason = $"Gece. {(home != null ? "Evinde" : "Olduğu yerde")} uyuyor; {H.Clock(until)}'de kalkacak.",
        };
    }

    Activity HomeAct(string label, string reason, double until, string go = "Eve dönüyor")
    {
        var home = Home;
        return new Activity
        {
            Kind = ActKind.Home, Anim = "Idle", Inside = true, Place = home?.Id ?? -1, Target = home?.Door ?? _p.Pos, Face = home?.DoorFace ?? Vector2.Zero,
            Until = until, Label = label, GoLabel = go, Reason = reason,
        };
    }

    Spot FreeSpot(Place pl, string tag, int salt = 0)
    {
        if (pl == null) return null;
        var list = new List<Spot>();
        foreach (var s in pl.Spots) if (s.Tag == tag && (s.Free || s.TakenBy == _p.Id)) list.Add(s);
        if (list.Count == 0) return null;
        // prefer the spot this person already holds (keeps people stable across re-decisions)
        foreach (var s in list) if (s.TakenBy == _p.Id) return s;
        return list[(int)(H.Hash(_p.Id, _day, salt + 31) * list.Count) % list.Count];
    }

    Activity AtSpot(ActKind kind, Place pl, Spot s, string anim, string tool, double until, string label, string go, string reason)
    {
        return new Activity
        {
            Kind = kind, Anim = anim, Tool = tool, Place = pl?.Id ?? -1, Spot = s,
            Target = s?.Pos ?? pl?.Door ?? _p.Pos, Face = s?.Face ?? Vector2.Zero, Y = s?.Y ?? 0,
            Until = until, Label = label, GoLabel = go, Reason = reason,
        };
    }

    Activity InArea(ActKind kind, Place pl, Vector2 at, Vector2 face, string anim, string tool, double until, string label, string go, string reason, float wander = 0)
    {
        return new Activity
        {
            Kind = kind, Anim = anim, Tool = tool, Place = pl?.Id ?? -1, Target = at, Face = face, Wander = wander,
            Until = until, Label = label, GoLabel = go, Reason = reason,
        };
    }

    /// <summary>Lunch: at home if it is near the workplace, otherwise sitting at the work site.</summary>
    Activity Lunch(Place work, float start, float end)
    {
        var home = Home;
        if (home != null && (work == null || TravelH(work.Door, home.Door) < 0.2f))
            return HomeAct("Öğle yemeği yiyor", "Öğle arası; evde yemek.", At(end), "Öğle yemeği için eve gidiyor");
        var seat = FreeSpot(work, "sit", 2);
        if (seat != null)
            return AtSpot(ActKind.Eat, work, seat, "Sit", null, At(end), "Azığını yiyor", "Oturacak yer arıyor", "Öğle arası; iş yeri eve uzak, azığını burada yiyor.");
        Vector2 at = work != null ? work.RandomPoint(new Rng((ulong)(_p.Id * 31 + _day)), 2f) : _p.Pos;
        return InArea(ActKind.Eat, work, at, Vector2.Zero, "Sit", null, At(end), "Azığını yiyor", "Gölgelik bir yer arıyor", "Öğle arası; iş yeri eve uzak, azığını yere oturup yiyor.");
    }

    /// <summary>Evening: dinner at home, then socialise until bed.</summary>
    Activity Evening(float bed)
    {
        float dinner = 18.6f + R(40) * 0.4f;
        if (_h < dinner + 0.75f && _h >= 17.6f)
            return HomeAct("Akşam yemeği yiyor", "Akşam oldu; hane birlikte yemek yiyor.", At(dinner + 0.75f), "Akşam yemeğine eve gidiyor");
        return Leisure(bed);
    }

    Activity Leisure(float bed)
    {
        var plaza = _w.PlaceOf(PlaceKind.Plaza);
        var home = Home;
        float r = R(50 + (int)(_h * 2));
        bool lateInn = _p.Age >= 18 && _p.Role is not (Role.Elder or Role.Priest) && R(77) < (_p.Social < 0.4f ? 0.3f : 0.1f);
        var inn = _w.PlaceOf(PlaceKind.Inn);
        // the inn is a long walk (~500 m ≈ 5 game hours at the 30-minute day): only go if there is time for there and back
        float innWalk = inn != null ? TravelH(_p.Pos, inn.Door) : 99f;
        if (lateInn && inn != null && _h < bed - 2 * innWalk - 1f)
        {
            var seat = FreeSpot(inn, "sit", 5);
            if (seat != null)
                return AtSpot(ActKind.Drink, inn, seat, "Sit", null, At(bed - innWalk), $"{_w.InnName} hanında içiyor", "Hana gidiyor",
                    $"Günün yorgunluğunu atmak ve sohbet için hana gitti{(_p.Social < 0.4f ? " (yalnız hissediyordu)" : "")}.");
        }
        if (r < 0.45f && home != null)
        {
            var bench = FreeSpot(home, "sit", 6);
            if (bench != null)
                return AtSpot(ActKind.Rest, home, bench, "Sit", null, Math.Min(Mins(40 + R(8) * 50), At(bed)), "Evinin önünde oturuyor", "Kapı önüne çıkıyor", "Akşam serinliğinde kapı önünde dinleniyor.");
        }
        if (plaza != null && r < 0.85f)
        {
            string tag = R(9) < 0.4f ? "sit" : "talk";
            var s = FreeSpot(plaza, tag, 7) ?? FreeSpot(plaza, "talk", 8);
            if (s != null)
                return AtSpot(ActKind.Socialize, plaza, s, tag == "sit" && s.Tag == "sit" ? "Sit" : "Talk", null, Math.Min(Mins(35 + R(10) * 45), At(bed - 0.4f)),
                    "Meydanda sohbet ediyor", "Meydana çıkıyor", "İş bitti; komşularla meydanda sohbet.");
        }
        return HomeAct("Evde oturuyor", "Akşam; evde vakit geçiriyor.", Math.Min(Mins(45 + R(11) * 40), At(bed)));
    }

    // ------------------------------------------------------------------------------------------------ roles

    Activity Farmer()
    {
        var hh = House;
        Place field = null;
        if (hh != null && hh.Fields.Count > 0) field = _w.Places[hh.Fields[(int)(R(12) * hh.Fields.Count) % hh.Fields.Count]];
        if (field == null) { var all = _w.PlacesOf(PlaceKind.Field); if (all.Count > 0) field = all[_p.Id % all.Count]; }
        { float st = 7f - 0.35f + R(17) * 0.8f; if (_h < st) return Chores(st); }
        if (_h >= 12f && _h < 12.9f) return Lunch(field, 12f, 12.9f);
        if (_h >= 17.6f) return Evening(BedHour());
        // work segment: move along the furrows every 35–70 min
        float segEnd = MathF.Min(_h < 12 ? 12f : 17.6f, _h + (35 + R((int)(_h * 3)) * 35) / 60f);
        var rng = new Rng((ulong)(_p.Id * 1009 + _day * 97 + (int)(_h * 4)));
        Vector2 at = field?.RandomPoint(rng, 2.5f) ?? _p.Pos;
        bool hoe = field?.Name.Contains("nadas") == true || R((int)(_h * 3) + 5) < 0.6f;
        var face = field != null ? new Vector2(MathF.Cos(field.AngleRad), MathF.Sin(field.AngleRad)) * (R(14) < 0.5f ? 1 : -1) : Vector2.Zero;
        string fieldName = field?.Name ?? "tarla";
        return InArea(ActKind.Work, field, at, face, hoe ? "Hoe" : "Gather", hoe ? "tool_hoe" : null, At(segEnd),
            hoe ? $"{Cap(TR.Loc(fieldName))} çapa yapıyor" : $"{Cap(TR.Loc(fieldName))} ot ayıklıyor", $"{Cap(TR.Dat(fieldName))} gidiyor",
            $"{(_h < 12 ? "Sabah" : "Öğleden sonra")} iş saati; {(hh != null ? hh.Surname + " hanesinin" : "köyün")} tarlası.");
    }

    Activity Chores(float until)
    {
        var home = Home;
        var wood = home != null ? FreeSpot(home, "yard", 15) : null;
        if (wood != null && R(16) < 0.6f)
            return AtSpot(ActKind.Work, home, wood, "Gather", null, At(until), "Avluda iş görüyor", "Avluya çıkıyor", "Sabah erken; avludaki işler (odun, hayvanlar).");
        return HomeAct("Evde hazırlanıyor", "Sabah erken; işe çıkmadan önce hazırlık.", At(until));
    }

    Activity Homemaker()
    {
        var home = Home;
        var well = _w.PlaceOf(PlaceKind.Well);
        var market = _w.PlaceOf(PlaceKind.Market);
        float bed = BedHour();
        if (_h >= 17.6f) return Evening(bed);
        // morning water run
        if (_h < 7.6f && well != null)
        {
            var s = FreeSpot(well, "draw", 20);
            if (s != null) return Water(well, s, At(7.6f));
        }
        if (_h >= 11.0f && _h < 12.8f) return HomeAct("Yemek pişiriyor", "Öğlen yaklaşıyor; hanenin yemeğini hazırlıyor.", At(12.8f), "Yemek yapmaya eve dönüyor");
        if (_h >= 15.2f && _h < 15.9f && well != null)
        {
            var s = FreeSpot(well, "draw", 21);
            if (s != null) return Water(well, s, At(15.9f));
        }
        if (_h >= 16.3f && _h < 17.6f) return HomeAct("Akşam yemeğini hazırlıyor", "Akşam yemeği hazırlığı.", At(17.6f), "Eve dönüyor");
        // mid-morning / afternoon: rotate between chores by day
        float pick = R(22 + (_h < 12 ? 0 : 1));
        if (_h >= 9.8f && _h < 11f && market != null && R(23) < 0.6f)
        {
            var s = FreeSpot(market, "customer", 24);
            if (s != null) return AtSpot(ActKind.Shop, market, s, "Talk", null, Mins(18 + R(25) * 20), "Pazarda alışveriş yapıyor", "Pazara gidiyor", "Evin eksiklerini almak için pazar tezgâhına uğradı.");
        }
        var wash = _w.PlaceOf(PlaceKind.Washing);
        if (pick < 0.3f && wash != null)
        {
            var s = FreeSpot(wash, "wash", 26);
            if (s != null) return AtSpot(ActKind.Wash, wash, s, "Gather", "tool_bucket", At(_h < 12 ? 11f : 15.2f), "Derede çamaşır yıkıyor", "Çamaşırları dereye götürüyor", "Haftalık çamaşır günü; köyün yıkama yeri derenin kıyısında.");
        }
        if (pick < 0.75f && home != null)
        {
            var s = FreeSpot(home, "garden", 27);
            if (s != null) return AtSpot(ActKind.Garden, home, s, R(28) < 0.5f ? "Hoe" : "Gather", R(28) < 0.5f ? "tool_hoe" : null, Mins(40 + R(29) * 40), "Bahçede çalışıyor", "Bahçeye çıkıyor", "Evin arkasındaki sebze bahçesi bakım istiyor.");
        }
        var plaza = _w.PlaceOf(PlaceKind.Plaza);
        var t = FreeSpot(plaza, "talk", 30);
        if (t != null) return AtSpot(ActKind.Socialize, plaza, t, "Talk", null, Mins(25 + R(31) * 25), "Meydanda komşularla konuşuyor", "Meydana gidiyor", "İşlerin arasında komşularla haberleşiyor.");
        return HomeAct("Evde iş görüyor", "Ev işleri.", Mins(45));
    }

    Activity Water(Place well, Spot s, double until)
    {
        var a = AtSpot(ActKind.FetchWater, well, s, "Idle", "tool_bucket", Math.Min(until, Mins(10 + R(33) * 8)), "Kuyudan su çekiyor", "Kovayla kuyuya gidiyor", "Hanenin günlük suyu kuyudan taşınıyor.");
        a.WalkTool = "tool_bucket";
        return a;
    }

    Activity Woodcutter()
    {
        var site = _w.PlaceOf(PlaceKind.Logging);
        { float st = 6.8f - 0.35f + R(17) * 0.8f; if (_h < st) return Chores(st); }
        if (_h >= 12f && _h < 12.8f) return Lunch(site, 12f, 12.8f);
        if (_h >= 16.8f && _h < 17.6f)
        {
            // carry a bundle home to the woodpile
            var home = Home;
            var yard = home != null ? FreeSpot(home, "yard", 34) : null;
            var a = yard != null
                ? AtSpot(ActKind.Carry, home, yard, "Gather", null, At(17.6f), "Odunları istifliyor", "Odun yükünü eve taşıyor", "Gün bitti; kestiği odunu evin odunluğuna taşıyor.")
                : HomeAct("Odunları istifliyor", "Gün bitti.", At(17.6f), "Odun yükünü eve taşıyor");
            a.WalkAnim = "Carry"; a.WalkTool = "tool_sack";
            return a;
        }
        if (_h >= 17.6f) return Evening(BedHour());
        if (site == null) return Leisure(BedHour());
        bool split = R((int)(_h * 2) + 35) < 0.25f;
        var s = split ? FreeSpot(site, "block", 36) : FreeSpot(site, "chop", 37 + (int)_h);
        s ??= FreeSpot(site, "chop", 38);
        float segEnd = MathF.Min(_h < 12 ? 12f : 16.8f, _h + (40 + R((int)(_h * 3) + 39) * 40) / 60f);
        return AtSpot(ActKind.Work, site, s, "Chop", "tool_axe", At(segEnd),
            split ? "Kütük yarıyor" : "Ağaç kesiyor", "Kesim yerine yürüyor",
            split ? "Kesilen ağaçlar yakacak odun için yarılıyor." : "Köyün odunu ormanın kıyısındaki kesim yerinden geliyor.");
    }

    Activity Smith(bool apprentice)
    {
        var smithy = _w.PlaceOf(PlaceKind.Smithy);
        { float st = 7f - 0.35f + R(17) * 0.8f; if (_h < st) return Chores(st); }
        if (_h >= 12f && _h < 12.9f) return Lunch(smithy, 12f, 12.9f);
        if (_h >= 18f) return Evening(BedHour());
        if (smithy == null) return Leisure(BedHour());
        if (!apprentice)
        {
            var s = FreeSpot(smithy, R((int)(_h * 2) + 40) < 0.75f ? "anvil" : "forge", 41) ?? FreeSpot(smithy, "anvil", 42);
            return AtSpot(ActKind.Work, smithy, s, s?.Tag == "forge" ? "Idle" : "Hammer", s?.Tag == "forge" ? null : "tool_hammer", Mins(30 + R((int)_h + 43) * 40),
                s?.Tag == "forge" ? "Ocağı körüklüyor" : "Örste demir dövüyor", "Demirhaneye gidiyor", "Köyün nal, çivi ve alet işleri demircide.");
        }
        float r = R((int)(_h * 2) + 44);
        if (r < 0.2f)
        {
            var well = _w.PlaceOf(PlaceKind.Well);
            var s = FreeSpot(well, "draw", 45);
            if (s != null) return Water(well, s, Mins(25));
        }
        var sp = FreeSpot(smithy, r < 0.6f ? "grind" : "forge", 46) ?? FreeSpot(smithy, "forge", 47);
        return AtSpot(ActKind.Work, smithy, sp, sp?.Tag == "grind" ? "Gather" : "Idle", null, Mins(25 + R((int)_h + 48) * 30),
            sp?.Tag == "grind" ? "Bıçak biliyor" : "Körüğü çekiyor", "Demirhaneye gidiyor", "Ustasının yanında çıraklık yapıyor.");
    }

    Activity Shepherd()
    {
        var pasture = _w.PlaceOf(PlaceKind.Pasture);
        var pen = _w.PlaceOf(PlaceKind.Pen);
        { float st = 6.6f - 0.35f + R(17) * 0.8f; if (_h < st) return Chores(st); }
        if (_h >= 18f) return Evening(BedHour());
        if (_h >= 17.4f && pen != null)
            return InArea(ActKind.Herd, pen, pen.Door, pen.DoorFace, "Idle", null, At(18f), "Sürüyü ağıla kapatıyor", "Koyunları ağıla getiriyor", "Akşam; sürü geceyi ağılda geçirir.");
        if (pasture == null) return Leisure(BedHour());
        if (_h >= 12f && _h < 12.7f)
        {
            var at = pasture.RandomPoint(new Rng((ulong)(_p.Id * 3 + _day)), 6f);
            return InArea(ActKind.Eat, pasture, at, Vector2.Zero, "Sit", null, At(12.7f), "Merada azığını yiyor", "Gölgeye geçiyor", "Öğle; sürüyü gözden kaçırmadan yemek.");
        }
        bool sit = R((int)(_h * 2) + 50) < 0.35f;
        var p2 = pasture.RandomPoint(new Rng((ulong)(_p.Id * 11 + _day * 7 + (int)(_h * 2))), 5f);
        return InArea(ActKind.Herd, pasture, p2, Vector2.Zero, sit ? "Sit" : "Idle", sit ? null : "tool_pitchfork", Mins(30 + R((int)_h + 51) * 30),
            sit ? "Sürüyü izliyor" : "Koyunları otlatıyor", "Sürüyü meraya götürüyor", "Koyunlar gün boyu merada otlar; çoban başlarında.", sit ? 0 : 7f);
    }

    Activity Headman()
    {
        var plaza = _w.PlaceOf(PlaceKind.Plaza);
        { float st = 7.2f - 0.35f + R(17) * 0.8f; if (_h < st) return Chores(st); }
        if (_h >= 11.8f && _h < 13f) return HomeAct("Öğle yemeği yiyor", "Öğle arası.", At(13f), "Öğle yemeğine gidiyor");
        if (_h >= 17.5f) return Evening(BedHour());
        float r = R((int)(_h * 2) + 60);
        if (r < 0.35f)
        {
            var fields = _w.PlacesOf(PlaceKind.Field);
            if (fields.Count > 0)
            {
                var f = fields[(int)(r * 1000) % fields.Count];
                var at = f.RandomPoint(new Rng((ulong)(_p.Id + _day * 5 + (int)_h)), 1f);
                return InArea(ActKind.Inspect, f, at, Vector2.Zero, "Idle", null, Mins(25 + R(61) * 20), $"{Cap(TR.Acc(f.Name))} yokluyor", "Tarlaları dolaşıyor", "Muhtar olarak ekinlerin durumunu kontrol ediyor.");
            }
        }
        if (r < 0.55f)
        {
            var smithy = _w.PlaceOf(PlaceKind.Smithy);
            var s = FreeSpot(smithy, "customer", 62);
            if (s != null) return AtSpot(ActKind.Socialize, smithy, s, "Talk", null, Mins(20), "Demirciyle konuşuyor", "Demirhaneye uğruyor", "Köyün sapan demirlerini soruyor.");
        }
        var t = FreeSpot(plaza, "talk", 63) ?? FreeSpot(plaza, "sit", 64);
        return AtSpot(ActKind.Socialize, plaza, t, t?.Tag == "sit" ? "Sit" : "Talk", null, Mins(30 + R(65) * 30), "Meydanda köylülerle konuşuyor", "Meydana gidiyor", "Muhtar; köyün işleri meydanda konuşulur.");
    }

    Activity Priest()
    {
        var chapel = _w.PlaceOf(PlaceKind.Chapel);
        if (chapel == null) return Leisure(BedHour());
        if (_h < 8f)
        {
            var s = FreeSpot(chapel, "priest", 70);
            return AtSpot(ActKind.Pray, chapel, s, "Pray", null, At(8f), "Sabah duasını yönetiyor", "Tapınağa gidiyor", "Şafak duası; köyün tanrısına sabah şükrü.");
        }
        if (_h >= 18f && _h < 19f)
        {
            var s = FreeSpot(chapel, "priest", 71);
            return AtSpot(ActKind.Pray, chapel, s, "Pray", null, At(19f), "Akşam duasını yönetiyor", "Tapınağa gidiyor", "Akşam duası.");
        }
        if (_h >= 8.2f && _h < 10.9f) return HomeAct("Çocuklara ders veriyor", "Sabahları köyün çocuklarına tapınakta okuma yazma öğretiyor.", At(10.9f), "Derse gidiyor");
        if (_h >= 11.5f && _h < 13f) return HomeAct("Tapınakta kitap okuyor", "Öğle; tapınağın içinde.", At(13f), "Tapınağa dönüyor");
        if (_h >= 19f) return HomeAct("Tapınakta", "Gece tapınakta kalıyor.", At(BedHour()), "Tapınağa dönüyor");
        // visits: plaza or a house door
        var plaza = _w.PlaceOf(PlaceKind.Plaza);
        if (R((int)(_h * 2) + 72) < 0.5f)
        {
            var homes = _w.PlacesOf(PlaceKind.Home);
            var h = homes[(int)(R(73 + (int)_h) * homes.Count) % homes.Count];
            return InArea(ActKind.Socialize, h, h.Door + h.DoorFace * 1.4f, -h.DoorFace, "Talk", null, Mins(20 + R(74) * 15), $"{Cap(TR.Dat(h.Name))} uğradı", "Bir haneyi ziyarete gidiyor", "Köylülerin dertlerini dinliyor, hastaları soruyor.");
        }
        var t = FreeSpot(plaza, "talk", 75);
        return AtSpot(ActKind.Socialize, plaza, t, "Talk", null, Mins(30), "Meydanda köylülerle konuşuyor", "Meydana gidiyor", "Köyün rahibi; halkın arasında.");
    }

    Activity InnStaff()
    {
        var inn = _w.PlaceOf(PlaceKind.Inn);
        var yard = _w.PlaceOf(PlaceKind.InnYard);
        if (inn == null) return Leisure(BedHour());
        if (_p.Role == Role.Innkeeper)
        {
            if ((_h >= 12f && _h < 12.6f) || (_h >= 18.2f && _h < 18.8f) || _h >= 22.8f)
                return HomeAct("Mutfakta", "Yemek saati; mutfakta aşçıya yardım ediyor.", At(_h >= 22.8f ? BedHour() : (_h < 13 ? 12.6f : 18.8f)), "İçeri giriyor");
            var s = FreeSpot(inn, "keeper", 80);
            return AtSpot(ActKind.Work, inn, s, R((int)(_h * 3) + 81) < 0.5f ? "Talk" : "Idle", null, Mins(30),
                "Tezgâhın başında", "Tezgâha geçiyor", $"{_w.InnName} hanının sahibi; gelen gideni karşılıyor.");
        }
        if (_p.Role == Role.InnServant)
        {
            if (_h >= 14f && _h < 15f) return HomeAct("Mutfakta bulaşık yıkıyor", "Öğleden sonra mutfak işleri.", At(15f), "Mutfağa giriyor");
            if (R((int)(_h * 4) + 82) < 0.25f && yard != null)
            {
                var tr = FreeSpot(yard, "trough", 83);
                if (tr != null) { var a = AtSpot(ActKind.FetchWater, yard, tr, "Idle", "tool_bucket", Mins(10), "Yalaktan su alıyor", "Kovayla yalağa gidiyor", "Hanın suyu yalak başındaki kuyudan."); a.WalkTool = "tool_bucket"; return a; }
            }
            var s = FreeSpot(inn, "serve", 84 + (int)(_h * 4));
            return AtSpot(ActKind.Work, inn, s, "Talk", null, Mins(12 + R((int)(_h * 4) + 85) * 12), "Masalara servis yapıyor", "Masalar arasında dolaşıyor", "Hanın hizmetkârı; müşterilere içki ve yemek taşıyor.");
        }
        // stable hand
        var st = FreeSpot(yard ?? inn, "stall", 86 + (int)_h) ?? FreeSpot(yard ?? inn, "trough", 87);
        if (_h >= 12f && _h < 12.7f) return HomeAct("Öğle yemeği yiyor", "Öğle arası.", At(12.7f), "Mutfağa gidiyor");
        return AtSpot(ActKind.Work, yard ?? inn, st, st?.Tag == "trough" ? "Idle" : "Gather", st?.Tag == "trough" ? "tool_bucket" : "tool_pitchfork", Mins(25 + R((int)_h + 88) * 25),
            st?.Tag == "trough" ? "Atlara su veriyor" : "Ahırı temizliyor", "Ahıra gidiyor", "Yolcuların hayvanları ahırda; seyis onlara bakıyor.");
    }

    Activity Child()
    {
        var plaza = _w.PlaceOf(PlaceKind.Plaza);
        var home = Home;
        float bed = BedHour();
        if (_h >= 17.8f) return _h < 19.2f ? HomeAct("Akşam yemeği yiyor", "Akşam; hane sofrası.", At(19.2f), "Eve çağrıldı") : HomeAct("Evde", "Yatma vakti yaklaşıyor.", At(bed), "Eve dönüyor");
        if (_h >= 11.7f && _h < 12.7f) return HomeAct("Öğle yemeği yiyor", "Öğle; annesi çağırdı.", At(12.7f), "Öğle yemeğine koşuyor");
        if (_p.Age < 6)
        {
            // little ones stay by the house: indoors mostly, a while in the yard with mother
            if (R((int)(_h * 2) + 94) < 0.65f) return HomeAct("Evde oynuyor", "Küçük çocuk; annesinin yanında, evde.", Mins(40 + R((int)_h + 95) * 30));
            var h0 = Home;
            var y0 = h0 != null ? FreeSpot(h0, "yard", 96) : null;
            if (y0 != null) return AtSpot(ActKind.Play, h0, y0, "Idle", null, Mins(25), "Avluda oynuyor", "Avluya çıkıyor", "Küçük çocuk; evin avlusunda oynuyor.");
        }
        var chapel0 = _w.PlaceOf(PlaceKind.Chapel);
        float lessonEnd = 10.6f + R(98) * 0.45f;
        if (_p.Age >= 6 && _p.Age <= 12 && chapel0 != null && _h >= 8.3f && _h < lessonEnd)
            return new Activity { Kind = ActKind.Pray, Anim = "Idle", Inside = true, Place = chapel0.Id, Target = chapel0.Door, Face = chapel0.DoorFace, Until = At(lessonEnd),
                Label = "Tapınakta derste", GoLabel = "Derse koşuyor", Reason = "Rahip sabahları köyün çocuklarına okuma yazma öğretiyor.", Hurry = true };
        if (_p.Age >= 9 && _h >= 7.6f && _h < 8.2f)
        {
            var well = _w.PlaceOf(PlaceKind.Well);
            var s = FreeSpot(well, "draw", 90);
            if (s != null) return Water(well, s, At(8.2f));
        }
        // afternoons: older children help the family in the fields now and then
        var hh0 = House;
        if (_p.Age >= 9 && _h >= 13f && _h < 16.5f && hh0 != null && hh0.Fields.Count > 0 && R(97) < 0.55f)
        {
            var f = _w.Places[hh0.Fields[0]];
            var at = f.RandomPoint(new Rng((ulong)(_p.Id * 71 + _day)), 2f);
            return InArea(ActKind.Work, f, at, Vector2.Zero, "Gather", null, At(16.5f), $"{Cap(TR.Loc(f.Name))} yardım ediyor", $"{Cap(TR.Dat(f.Name))} gidiyor", "Ailesinin tarlasında taş ve ot topluyor; çocuklar da iş görür.");
        }
        // play: spread over the plaza, the front of the house and the green by the pasture
        float pr = R((int)(_h * 2) + 91);
        var pasture = _w.PlaceOf(PlaceKind.Pasture);
        Place where = pr < 0.4f ? plaza : pr < 0.75f ? home : pasture;
        where ??= plaza;
        Vector2 c; float rad;
        if (where?.Kind == PlaceKind.Home) { c = where.Door + where.DoorFace * 5.5f; rad = 4.5f; }
        else if (where?.Kind == PlaceKind.Pasture) { c = where.Door + (where.Center - where.Door) * 0.25f; rad = 7f; }
        else { c = where?.Center ?? _p.Pos; rad = 8f; }
        string place = where?.Kind switch { PlaceKind.Home => "Evin önünde oynuyor", PlaceKind.Pasture => "Mera kıyısında oynuyor", _ => "Meydanda oynuyor" };
        var a = InArea(ActKind.Play, where, c, Vector2.Zero, R((int)(_h * 3) + 92) < 0.5f ? "Idle" : "Talk", null, Mins(25 + R((int)_h + 93) * 25),
            place, "Oynamaya koşuyor", "Çocuk; iş zamanı değil, oyun zamanı.", rad);
        a.Hurry = true;
        return a;
    }

    Activity Elder()
    {
        var chapel = _w.PlaceOf(PlaceKind.Chapel);
        var plaza = _w.PlaceOf(PlaceKind.Plaza);
        var home = Home;
        float bed = BedHour();
        if (_h >= 17.4f) return Evening(bed);
        if (_h < 8f && chapel != null)
        {
            var s = FreeSpot(chapel, "pray", 100);
            if (s != null) return AtSpot(ActKind.Pray, chapel, s, "Pray", null, At(8f), "Sabah duasında", "Tapınağa yürüyor", "Yaşlılar güne tapınakta dua ederek başlar.");
        }
        if (_h >= 11.6f && _h < 13f) return HomeAct("Öğle yemeği yiyor", "Öğle.", At(13f), "Eve dönüyor");
        if (_h >= 15f && _h < 16f)
            return HomeAct("Kestiriyor", "Öğleden sonra yorulunca biraz uzanıyor.", At(16f), "Dinlenmeye eve gidiyor");
        float r = R((int)(_h * 2) + 101);
        if (r < 0.5f && home != null)
        {
            var b = FreeSpot(home, "sit", 102);
            if (b != null) return AtSpot(ActKind.Rest, home, b, "Sit", null, Mins(40 + R(103) * 40), "Kapı önünde oturuyor", "Kapı önüne çıkıyor", "Yaşlı; güneşte oturup gelen geçeni izliyor.");
        }
        var s2 = FreeSpot(plaza, r < 0.8f ? "sit" : "talk", 104) ?? FreeSpot(plaza, "talk", 105);
        return AtSpot(ActKind.Socialize, plaza, s2, s2?.Tag == "sit" ? "Sit" : "Talk", null, Mins(35 + R(106) * 35), "Meydanda dertleşiyor", "Meydana yürüyor", "Yaşlılar meydanda eski günleri anlatır.");
    }

    // ------------------------------------------------------------------------------------------------ visitors

    /// <summary>Faz 2 E: an adventurer staying at the inn waits for work: breakfast and the common room in the morning, sword drill in
    /// the yard in the afternoon, ale in the evening, a room at night.</summary>
    Activity InnGuest()
    {
        var inn = _w.PlaceOf(PlaceKind.Inn);
        var yard = _w.PlaceOf(PlaceKind.InnYard);
        if (inn == null) return null;
        float h = _h;
        int k = _p.Work;
        if (h >= 23.2f - 0.2f * k || h < 7.2f + 0.3f * k)
            return new Activity { Kind = ActKind.Sleep, Inside = true, Place = inn.Id, Target = inn.Door, Face = inn.DoorFace, Until = h < 12 ? At(7.2f + 0.3f * k) : At(24 + 7.2f + 0.3f * k), Label = "Handaki odasında uyuyor", GoLabel = "Odasına çıkıyor", Reason = "Maceracı; iş çıkana dek handa kalıyor." };
        if (h < 12.5f)
        {
            var s = FreeSpot(inn, "sit", 130 + k);
            return AtSpot(ActKind.Drink, inn, s, "Sit", null, At(12.5f), "Handa oturuyor, iş bekliyor", "Hana yürüyor", "Maceracı; panodaki ilanlara bakıyor, kendisini kiralayacak birini bekliyor.");
        }
        if (h < 18.5f && yard != null && FreeSpot(yard, "drill", 150 + k) is Spot ds)
            return AtSpot(ActKind.Work, yard, ds, "Attack", null, At(18.5f), "Avluda kılıç talimi yapıyor", "Avluya çıkıyor", "Kılıcı paslanmasın diye her gün talim ediyor.");
        var s2 = FreeSpot(inn, "sit", 140 + k);
        return AtSpot(ActKind.Drink, inn, s2, "Sit", null, At(23.2f - 0.2f * k), "Handa içiyor", "Hana dönüyor", "Akşam; maceracılar hanın ocağının başında yol hikâyeleri anlatıyor.");
    }

    Activity Visitor()
    {
        if (_p.Guest) return InnGuest();
        var inn = _w.PlaceOf(PlaceKind.Inn);
        var ends = _w.PlacesOf(PlaceKind.RoadEnd);
        Place east = ends.Count > 0 ? ends[0] : null, west = ends.Count > 1 ? ends[1] : east;
        int cycle = _p.Role == Role.Merchant ? 2 : 3;
        int phaseDay = ((_day + _p.Id) % cycle + cycle) % cycle;
        float h = _h;
        string who = _p.Role switch { Role.Merchant => "Gezgin tüccar", Role.Pilgrim => "Hacı", _ => "Maceracı" };

        Activity Gone(double until) => new()
        {
            Kind = ActKind.Travel, Inside = true, Place = east?.Id ?? -1, Target = east?.Door ?? _p.Pos, Until = until,
            Label = "Bölgenin dışında", GoLabel = "Yola çıkıyor", Reason = $"{who}; başka köyleri dolaşıyor.",
        };
        void Present(bool v) { if (!_peek) _p.Present = v; }

        if (_p.Role == Role.Merchant)
        {
            // day A: arrives from the east 14:00 → inn; day B: 7:00 → village market, 14:30 → leaves west; day C.. away
            if (phaseDay == 0)
            {
                if (h < 14f) { Present(false); return Gone(At(14f)); }
                Present(true);
                if (h < 20.5f && inn != null)
                {
                    var s = FreeSpot(inn, "sit", 110);
                    var a = AtSpot(ActKind.Drink, inn, s, "Sit", null, At(20.5f), $"{_w.InnName} hanında dinleniyor", "Arabasıyla hana gidiyor", "Doğudan geldi; geceyi handa geçirip sabah köy pazarına gidecek.");
                    return a;
                }
                return new Activity { Kind = ActKind.Sleep, Inside = true, Place = inn?.Id ?? -1, Target = inn?.Door ?? _p.Pos, Face = inn?.DoorFace ?? Vector2.Zero, Until = At(24 + 6.5f), Label = "Handa uyuyor", GoLabel = "Odasına çıkıyor", Reason = "Yolcu; hanın odasında kalıyor." };
            }
            if (phaseDay == 1)
            {
                Present(true);
                if (h < 6.5f) return new Activity { Kind = ActKind.Sleep, Inside = true, Place = inn?.Id ?? -1, Target = inn?.Door ?? _p.Pos, Until = At(6.5f), Label = "Handa uyuyor", Reason = "Yolcu; hanın odasında kalıyor." };
                var market = _w.PlaceOf(PlaceKind.Market);
                if (h < 14.5f && market != null)
                {
                    var s = FreeSpot(market, "vendor", 111);
                    return AtSpot(ActKind.Sell, market, s, "Talk", null, At(14.5f), "Pazarda mal satıyor", "Köy pazarına gidiyor", "Tüccar; kumaş, tuz ve baharat getirdi, köylüye satıyor.");
                }
                if (west != null) return new Activity { Kind = ActKind.Travel, Place = west.Id, Target = west.Door, Until = At(24 + 20), Label = "Batıya yolculuk ediyor", GoLabel = "Batıya yola çıkıyor", Reason = "Pazar bitti; sıradaki kasabaya gidiyor." };
            }
            Present(false);
            return Gone(At(24 + 14f));
        }

        if (_p.Role == Role.Pilgrim)
        {
            if (phaseDay != 1) { Present(false); return Gone(At(24 + 8f)); }
            if (h < 8f) { Present(false); return Gone(At(8f)); }
            Present(true);
            var chapel = _w.PlaceOf(PlaceKind.Chapel);
            if (h < 10.5f && chapel != null)
            {
                var s = FreeSpot(chapel, "pray", 112);
                if (s != null) return AtSpot(ActKind.Pray, chapel, s, "Pray", null, At(10.5f), "Tapınakta dua ediyor", "Tapınağa yürüyor", "Hacı; yol üstündeki her tapınakta dua ediyor.");
            }
            if (h < 18f && inn != null)
            {
                var s = FreeSpot(inn, "sit", 113);
                return AtSpot(ActKind.Rest, inn, s, "Sit", null, At(19f), "Handa dinleniyor", "Hana yürüyor", "Hacı; gece handa kalıp sabah doğuya devam edecek.");
            }
            if (h < 23.5f) return new Activity { Kind = ActKind.Sleep, Inside = true, Place = inn?.Id ?? -1, Target = inn?.Door ?? _p.Pos, Until = At(24 + 6.2f), Label = "Handa uyuyor", Reason = "Yolcu." };
            return Gone(At(24 + 8f));
        }

        // adventurer: arrives mid-morning at the inn, scouts the goblin trail in the afternoon, back at dusk
        if (phaseDay != 2) { Present(false); return Gone(At(24 + 9f)); }
        if (h < 9f) { Present(false); return Gone(At(9f)); }
        Present(true);
        if (h < 13f && inn != null)
        {
            var s = FreeSpot(inn, "sit", 114);
            return AtSpot(ActKind.Drink, inn, s, "Sit", null, At(13f), "Hancıdan goblinleri soruyor", "Hana yürüyor", "Maceracı; yol boyunca goblin saldırılarını duymuş, hancıdan bilgi alıyor.");
        }
        var lurk = _w.PlaceOf(PlaceKind.Lurk);
        if (h < 17.5f && lurk != null)
            return InArea(ActKind.Inspect, lurk, lurk.Center + new Vector2(-12, 16), Vector2.Zero, "Idle", "tool_spear", At(17.5f), "Orman patikasını gözlüyor", "Orman patikasına gidiyor", "Goblin kampının izini sürüyor; saldırmadan önce keşif.", 4f);
        if (h < 22f && inn != null)
        {
            var s = FreeSpot(inn, "sit", 115);
            return AtSpot(ActKind.Drink, inn, s, "Sit", null, At(22f), "Handa anlatıyor", "Hana dönüyor", "Keşiften döndü; gördüklerini hancıya anlatıyor.");
        }
        return new Activity { Kind = ActKind.Sleep, Inside = true, Place = inn?.Id ?? -1, Target = inn?.Door ?? _p.Pos, Until = At(24 + 8f), Label = "Handa uyuyor", Reason = "Maceracı; sabah yoluna devam edecek." };
    }

    // ------------------------------------------------------------------------------------------------ goblins

    Activity Goblin()
    {
        var camp = _w.PlaceOf(PlaceKind.Camp);
        var lurk = _w.PlaceOf(PlaceKind.Lurk);
        if (camp == null) return null;
        int slot = _p.Work % 6;  // 0,1 guards · 2 cook · 3,4 loafers · 5 sleeper (day shift inverted); Faz 2: up to 12 goblins, roles repeat
        bool raider = slot is 2 or 3 or 4;
        bool night = _h >= 22.5f || _h < 4f;
        // raiders lurk by the trail at night, sleep in the morning
        if (raider && night && lurk != null)
        {
            var s = FreeSpot(lurk, "lurk", 120);
            var a = AtSpot(ActKind.Lurk, lurk, s, "Idle", "tool_club", _h >= 22.5f ? At(24 + 4f) : At(4f), "Yol kenarında pusuda", "Pusu yerine sinsice gidiyor",
                "Gece; yoldan geçen tek tük yolcuyu soymak için pusuya yattılar.");
            a.WalkTool = "tool_torch";
            return a;
        }
        if (raider && _h >= 4f && _h < 11f)
        {
            var tent = FreeSpot(camp, "tent", 121 + _p.Work);
            return new Activity { Kind = ActKind.Sleep, Inside = true, Place = camp.Id, Spot = tent, Target = tent?.Pos ?? camp.Center, Until = At(11f), Label = "Çadırda uyuyor", GoLabel = "Çadırına giriyor", Reason = "Gece pusudaydı; gündüz uyuyor." };
        }
        if (!raider && slot == 5 && _h >= 6f && _h < 14f)
        {
            var tent = FreeSpot(camp, "tent", 122);
            return new Activity { Kind = ActKind.Sleep, Inside = true, Place = camp.Id, Spot = tent, Target = tent?.Pos ?? camp.Center, Until = At(14f), Label = "Çadırda horluyor", GoLabel = "Çadırına giriyor", Reason = "Gece nöbetçisi; gündüz uyuyor." };
        }
        if (slot is 0 or 1 or 5)
        {
            // patrol inside the palisade and along the spur
            var pts = new List<Spot>(camp.SpotsTagged("patrol"));
            if (pts.Count == 0) return InArea(ActKind.Guard, camp, camp.Center, Vector2.Zero, "Idle", "tool_spear", Mins(20), "Nöbet tutuyor", "Nöbet yerine geçiyor", "Kampın nöbetçisi.", 10f);
            int k = (int)((_now / 3.0 + _p.Work * 3) % pts.Count);
            var s = pts[k];
            return InArea(ActKind.Patrol, camp, s.Pos, s.Face, "Idle", "tool_spear", Mins(2.5f), "Devriye geziyor", "Devriye geziyor", "Kampın çevresini kolluyor; yabancı görürse saldırır.");
        }
        if (slot == 2)
        {
            var s = FreeSpot(camp, "spit", 123);
            if (s != null) return AtSpot(ActKind.Work, camp, s, "Idle", null, Mins(30), "Şişte et çeviriyor", "Ateşin başına geçiyor", "Kampın aşçısı; çaldıkları koyunu kızartıyor.");
        }
        var seat = FreeSpot(camp, "sit", 124 + _p.Work);
        if (seat != null) return AtSpot(ActKind.Socialize, camp, seat, R((int)(_h * 2) + slot) < 0.5f ? "Sit" : "Talk", null, Mins(30), "Ateş başında hırlaşıyor", "Ateşe yaklaşıyor", "Goblinler boş vakitlerinde ateş başında kavga eder, zar atar.");
        return InArea(ActKind.Wander, camp, camp.Center, Vector2.Zero, "Idle", "tool_club", Mins(20), "Kampta dolanıyor", "Kampta dolanıyor", "Goblin.", 8f);
    }

    internal static string Cap(string s) => string.IsNullOrEmpty(s) ? s : char.ToUpper(s[0]) + s[1..];
}
