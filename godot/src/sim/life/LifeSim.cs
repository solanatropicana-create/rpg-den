using System;
using System.Collections.Generic;
using System.Numerics;

namespace FD.Sim.Life;

/// <summary>
/// The living region: places, the path network, households, people (villagers, inn folk, travellers, goblins)
/// and the sheep flock. Deterministic for a given seed and update sequence.
/// <para>Update(realDt, gameMinute, player): schedules advance on game time (the clock), movement on real time
/// (people walk at real speeds). Large clock jumps re-materialise everyone at their current activity.</para>
/// </summary>
public sealed partial class LifeSim
{
    public readonly List<Place> Places = new();
    public readonly List<Person> People = new();
    public readonly List<Household> Households = new();
    public readonly PathGraph Graph = new();
    public readonly Flock Flock = new();
    /// <summary>Solid footprints (buildings) as oriented rectangles — used by tests (nobody walks through walls).</summary>
    public readonly List<(Vector2 c, Vector2 half, float ang, string name)> Obstacles = new();
    /// <summary>Diagnostics: long routes that had to fall back to a straight line.</summary>
    public int DirectFallbacks;
    public readonly List<string> FallbackLog = new();
    public readonly Rng Rng;
    public double Now;
    double _last = double.NaN;
    public string VillageName = "Sessiztepe";
    public string InnName = "Yorgun Katır";
    public string CampName = "Kırık Diş kampı";

    /// <summary>Player (hero) position in the ground plane and whether sprinting — goblins react to it.</summary>
    public Vector2 PlayerPos;
    public bool PlayerSprinting, PlayerPresent;
    /// <summary>Raised when a goblin reaches the player and swings (Godot shows the hit).</summary>
    public event Action<Person> GoblinStrike;
    /// <summary>Raised when someone enters (true) or leaves (false) a building.</summary>
    public event Action<Person, bool> DoorUsed;

    public LifeSim(ulong seed) { Rng = new Rng(seed); _brain = new Brain(this); }

    readonly Brain _brain;

    public Place AddPlace(PlaceKind kind, string name, Vector2 door, Vector2 doorFace)
    {
        var p = new Place { Id = Places.Count, Kind = kind, Name = name, Door = door, DoorFace = doorFace, Center = door };
        Places.Add(p);
        return p;
    }

    public Spot AddSpot(Place pl, string tag, Vector2 pos, Vector2 face, float y = 0)
    {
        var s = new Spot { Pos = pos, Face = face, Tag = tag, Place = pl.Id, Y = y };
        pl.Spots.Add(s);
        return s;
    }

    public Place PlaceOf(PlaceKind k)
    {
        foreach (var p in Places) if (p.Kind == k) return p;
        return null;
    }

    public List<Place> PlacesOf(PlaceKind k)
    {
        var r = new List<Place>();
        foreach (var p in Places) if (p.Kind == k) r.Add(p);
        return r;
    }

    /// <summary>Connect every place to the path network (door → nearest node).</summary>
    public void FinishLayout()
    {
        foreach (var p in Places)
            if (p.Node < 0) p.Node = Graph.Nearest(p.Door, 60f);
    }

    // ------------------------------------------------------------------------------------------------ update

    public void Update(float realDt, double gameMinute)
    {
        Now = gameMinute;
        bool jump = double.IsNaN(_last) || gameMinute - _last > 30 || gameMinute < _last - 1;
        double gameDt = double.IsNaN(_last) ? 0 : Math.Max(0, gameMinute - _last);
        _last = gameMinute;
        if (jump) { Resync(); return; }

        foreach (var p in People)
        {
            UpdateNeeds(p, (float)gameDt);
            if (!p.Present)
            {
                if (Now >= p.NextDecision) Decide(p);
                continue;
            }
            if (p.Role == Role.Goblin) GoblinSense(p);
            else if (p.Act?.Kind != ActKind.Flee) FleeCheck(p);
            Step(p, realDt);
        }
        Flock.Update(this, realDt);
    }

    /// <summary>Put everyone where their current activity is (start of play, after a clock jump).</summary>
    public void Resync()
    {
        foreach (var s in AllSpots()) s.TakenBy = -1;
        foreach (var p in People)
        {
            p.Act = null;
            p.Path.Clear();
            p.Wait = 0;
            Decide(p, teleport: true);
        }
        Flock.Resync(this);
    }

    IEnumerable<Spot> AllSpots()
    {
        foreach (var pl in Places) foreach (var s in pl.Spots) yield return s;
    }

    void UpdateNeeds(Person p, float gameMinutes)
    {
        if (gameMinutes <= 0 || p.Act == null) return;
        float h = gameMinutes / 60f;
        switch (p.Act.Kind)
        {
            case ActKind.Sleep: p.Energy = MathF.Min(1, p.Energy + 0.13f * h); p.Food = MathF.Max(0, p.Food - 0.025f * h); break;
            case ActKind.Home: p.Food = MathF.Min(1, p.Food + 0.9f * h); p.Social = MathF.Min(1, p.Social + 0.08f * h); p.Energy = MathF.Max(0, p.Energy - 0.03f * h); break;
            case ActKind.Eat: p.Food = MathF.Min(1, p.Food + 0.8f * h); p.Energy = MathF.Min(1, p.Energy + 0.05f * h); break;
            case ActKind.Socialize: case ActKind.Drink: p.Social = MathF.Min(1, p.Social + 0.35f * h); p.Food = MathF.Max(0, p.Food - 0.05f * h); break;
            case ActKind.Rest: case ActKind.Pray: p.Energy = MathF.Min(1, p.Energy + 0.04f * h); p.Social = MathF.Max(0, p.Social - 0.02f * h); p.Food = MathF.Max(0, p.Food - 0.05f * h); break;
            default:
                p.Energy = MathF.Max(0, p.Energy - 0.055f * h);
                p.Food = MathF.Max(0, p.Food - 0.1f * h);
                p.Social = MathF.Max(0, p.Social - 0.04f * h);
                break;
        }
    }

    // ------------------------------------------------------------------------------------------------ decisions

    void Decide(Person p, bool teleport = false)
    {
        if (p.Act?.Spot != null && p.Act.Spot.TakenBy == p.Id) p.Act.Spot.TakenBy = -1;
        bool wasPresent = p.Present;
        var a = _brain.Choose(p, Now);
        if (!wasPresent && p.Present && !teleport && a != null)
        {
            // a traveller arrives: enter at the road end nearest to where they are going
            Place best = null;
            foreach (var e in PlacesOf(PlaceKind.RoadEnd))
                if (best == null || Vector2.Distance(e.Door, a.Target) < Vector2.Distance(best.Door, a.Target)) best = e;
            if (best != null) { p.Pos = best.Door; p.Dir = H.Norm(a.Target - best.Door); }
            p.Motion = Motion.Walking;
            p.Note($"{H.Clock(Now)} bölgeye geldi");
        }
        if (a == null) { p.NextDecision = Now + 15; return; }
        bool wasInside = p.Motion == Motion.Inside;
        var prevAct = p.Act;
        p.Act = a;
        p.NextDecision = a.Until;
        if (a.Spot != null) a.Spot.TakenBy = p.Id;
        if (prevAct == null || prevAct.Label != a.Label) p.Note($"{H.Clock(Now)} {a.Label}");

        if (!p.Present) { p.Motion = Motion.Inside; return; }
        if (teleport)
        {
            p.Pos = a.Target;
            if (a.Face != Vector2.Zero) p.Dir = a.Face;
            p.Path.Clear();
            p.Motion = a.Inside ? Motion.Inside : Motion.Doing;
            return;
        }
        if (Vector2.Distance(p.Pos, a.Target) < 0.35f)
        {
            Arrive(p);
            return;
        }
        if (wasInside) DoorUsed?.Invoke(p, false);
        Route(p, a.Target, a.Place);
        p.Running = a.Hurry;
        p.Motion = Motion.Walking;
    }

    void Arrive(Person p)
    {
        var a = p.Act;
        p.Path.Clear();
        if (a.Face != Vector2.Zero) p.Dir = a.Face;
        if (a.Inside)
        {
            if (p.Motion != Motion.Inside) DoorUsed?.Invoke(p, true);
            p.Motion = Motion.Inside;
        }
        else p.Motion = Motion.Doing;
        if (a.Kind == ActKind.Travel && a.Place >= 0 && Places[a.Place].Kind == PlaceKind.RoadEnd)
        {
            // visitor leaves the region
            p.Present = false;
            p.Motion = Motion.Inside;
        }
        p.SubTargetTimer = 0;
    }

    // ------------------------------------------------------------------------------------------------ movement

    /// <summary>Area (open place) the point belongs to, or -1.</summary>
    public int AreaAt(Vector2 pt)
    {
        foreach (var pl in Places)
            if (pl.Area >= 0 && pl.Contains(pt, 0.5f)) return pl.Area;
        return -1;
    }

    void Route(Person p, Vector2 target, int targetPlace)
    {
        p.Path.Clear();
        p.PathIdx = 0;
        int areaA = AreaAt(p.Pos), areaB = targetPlace >= 0 ? Places[targetPlace].Area : AreaAt(target);
        if (areaB < 0) areaB = AreaAt(target);
        float direct = Vector2.Distance(p.Pos, target);
        if ((areaA >= 0 && areaA == areaB) || direct < 6f)
        {
            p.Path.Add(target);
            return;
        }
        int s = Graph.Nearest(p.Pos, 80f);
        int g = Graph.Nearest(target, 80f);
        if (g < 0 && targetPlace >= 0) g = Places[targetPlace].Node;
        var ids = s >= 0 && g >= 0 ? Graph.FindPath(s, g) : null;
        if (ids == null)
        {
            if (direct > 40f)
            {
                DirectFallbacks++;
                if (FallbackLog.Count < 8) FallbackLog.Add($"{p.FullName}: ({p.Pos.X:F0},{p.Pos.Y:F0})→({target.X:F0},{target.Y:F0}) s={s} g={g} [{p.Act?.Label}]");
            }
            p.Path.Add(target);
            return;
        }
        // drop nodes that lie "behind" the walker at the start or beyond the target at the end
        int first = 0, last = ids.Count - 1;
        while (first + 1 <= last && Vector2.Distance(p.Pos, Graph.Nodes[ids[first + 1]]) < Vector2.Distance(Graph.Nodes[ids[first]], Graph.Nodes[ids[first + 1]]) && first < 2) first++;
        while (last - 1 >= first && Vector2.Distance(target, Graph.Nodes[ids[last - 1]]) < Vector2.Distance(Graph.Nodes[ids[last]], Graph.Nodes[ids[last - 1]]) && last > ids.Count - 3) last--;
        for (int i = first; i <= last; i++)
        {
            Vector2 q = Graph.Nodes[ids[i]];
            Vector2 prev = i > first ? Graph.Nodes[ids[i - 1]] : p.Pos;
            Vector2 next = i < last ? Graph.Nodes[ids[i + 1]] : target;
            Vector2 d = H.Norm(next - prev);
            Vector2 right = new(-d.Y, d.X);
            float off = MathF.Min(p.LaneOffset, Graph.HalfWidth[ids[i]] * 0.75f);
            p.Path.Add(q + right * off);
        }
        p.Path.Add(target);
    }

    void Step(Person p, float dt)
    {
        if (p.Wait > 0) { p.Wait -= dt; return; }
        var a = p.Act;
        if (a == null) { Decide(p); return; }

        if (a.Kind == ActKind.Chase)
        {
            ChaseStep(p, dt);
            return;
        }

        if (p.Motion == Motion.Walking)
        {
            float speed = p.Running ? p.RunSpeed : p.WalkSpeed;
            if (p.HasCart) speed = MathF.Min(speed, 1.15f);
            float move = speed * dt;
            while (move > 0 && p.PathIdx < p.Path.Count)
            {
                Vector2 to = p.Path[p.PathIdx];
                Vector2 d = to - p.Pos;
                float L = d.Length();
                if (L <= move)
                {
                    p.Pos = to; move -= L; p.PathIdx++;
                    if (p.HasCart) p.CartDist += L;
                }
                else
                {
                    Vector2 n = d / L;
                    p.Pos += n * move;
                    if (p.HasCart) p.CartDist += move;
                    p.Dir = Vector2.Normalize(Vector2.Lerp(p.Dir, n, MathF.Min(1, dt * 8f)));
                    move = 0;
                }
            }
            if (p.PathIdx >= p.Path.Count)
            {
                if (p.SubTargetTimer > 0) { p.Motion = Motion.Doing; p.SubTargetTimer = 0; if (a.Face != Vector2.Zero) p.Dir = a.Face; }
                else Arrive(p);
            }
            return;
        }

        if (Now >= a.Until) { Decide(p); return; }

        // wandering activities: pick a new nearby sub-target from time to time
        if (a.Wander > 0 && p.Motion == Motion.Doing)
        {
            float hz = a.Kind == ActKind.Play ? 0.25f : a.Kind == ActKind.Patrol ? 0.12f : 0.05f;
            if (H.Hash(p.Id, (int)(Now * 3), 7) < hz * dt * 3f)
            {
                var r = new Rng((ulong)(p.Id * 7919 + (long)(Now * 13)));
                float ang = r.Range(0, MathF.Tau), rad = MathF.Sqrt(r.Next01()) * a.Wander;
                var t = a.Target + new Vector2(MathF.Cos(ang), MathF.Sin(ang)) * rad;
                p.Path.Clear(); p.PathIdx = 0; p.Path.Add(t);
                p.Motion = Motion.Walking;
                p.SubTargetTimer = 1;
                p.Running = a.Kind == ActKind.Play && r.Chance(0.6f);
            }
        }
    }

    // ------------------------------------------------------------------------------------------------ goblins & danger

    public const float GoblinNoticeDay = 16f, GoblinNoticeNight = 9f, GoblinLeash = 48f;

    void GoblinSense(Person g)
    {
        if (!PlayerPresent || g.Motion == Motion.Inside || g.Act == null) return;
        if (g.Act.Kind == ActKind.Chase) return;
        float d = Vector2.Distance(g.Pos, PlayerPos);
        float h = (float)(Now % 1440) / 60f;
        bool night = h < 5 || h > 21;
        float notice = (night ? GoblinNoticeNight : GoblinNoticeDay) * (PlayerSprinting ? 1.6f : 1f) * (g.Act.Kind == ActKind.Patrol || g.Act.Kind == ActKind.Guard ? 1.3f : 1f);
        if (g.Act.Kind == ActKind.Sleep) notice *= 0.3f;
        if (d > notice) return;
        if (g.Act.Spot != null && g.Act.Spot.TakenBy == g.Id) g.Act.Spot.TakenBy = -1;
        g.Post = g.Act.Target;
        g.Act = new Activity
        {
            Kind = ActKind.Chase, Anim = "Run", Tool = g.Act.Tool ?? "tool_spear", WalkTool = g.Act.Tool ?? "tool_spear", Hurry = true,
            Target = PlayerPos, Until = Now + 20,
            Label = "Yabancıyı kovalıyor!", GoLabel = "Yabancıyı kovalıyor!",
            Reason = "Kampına yaklaşan bir yabancı gördü; bölgesini koruyor.",
        };
        g.AlertUntil = Now + 20;
        g.Note($"{H.Clock(Now)} bir yabancı gördü, peşine düştü");
        g.Running = true;
        g.Motion = Motion.Walking;
        // alert nearby goblins too
        foreach (var o in People)
            if (o != g && o.Role == Role.Goblin && o.Present && o.Motion != Motion.Inside && o.Act?.Kind != ActKind.Chase
                && Vector2.Distance(o.Pos, g.Pos) < 14f)
                o.Wait = 0.4f + H.Hash(o.Id, (int)Now) * 0.8f;
    }

    float _strikeCd;

    void ChaseStep(Person g, float dt)
    {
        var a = g.Act;
        a.Target = PlayerPos;
        float dPost = Vector2.Distance(g.Pos, g.Post);
        float d = Vector2.Distance(g.Pos, PlayerPos);
        if (!PlayerPresent || dPost > GoblinLeash || (Now > g.AlertUntil && d > 20f) || d > 38f)
        {
            g.Note($"{H.Clock(Now)} kovalamayı bıraktı");
            g.Act = null;
            g.Running = false;
            Decide(g);
            return;
        }
        Vector2 dir = H.Norm(PlayerPos - g.Pos);
        g.Dir = Vector2.Normalize(Vector2.Lerp(g.Dir, dir, MathF.Min(1, dt * 10f)));
        if (d > 1.7f)
        {
            g.Motion = Motion.Walking;
            g.Pos += dir * MathF.Min(d - 1.6f, g.RunSpeed * dt);
            a.Anim = "Run";
        }
        else
        {
            g.Motion = Motion.Doing;
            a.Anim = "Attack";
            a.Label = "Saldırıyor!";
            g.AlertUntil = Now + 6;
            _strikeCd -= dt;
            if (_strikeCd <= 0) { _strikeCd = 0.9f; GoblinStrike?.Invoke(g); }
        }
    }

    void FleeCheck(Person p)
    {
        if (p.Motion == Motion.Inside || p.IsVisitor && p.Role == Role.Adventurer) return;
        foreach (var o in People)
        {
            if (o.Role != Role.Goblin || o.Act?.Kind != ActKind.Chase) continue;
            if (Vector2.DistanceSquared(o.Pos, p.Pos) > 35f * 35f) continue;
            var home = p.Home >= 0 ? Places[p.Home] : null;
            if (p.Act?.Spot != null && p.Act.Spot.TakenBy == p.Id) p.Act.Spot.TakenBy = -1;
            p.Act = new Activity
            {
                Kind = ActKind.Flee, Anim = "Idle", Inside = home != null, Hurry = true,
                Target = home?.Door ?? p.Pos, Place = home?.Id ?? -1, Until = Now + 60,
                Label = "Goblinlerden kaçıyor!", GoLabel = "Goblinlerden kaçıyor!",
                Reason = "Yakında bir goblin saldırıyor; eve sığınıyor.",
            };
            p.Note($"{H.Clock(Now)} goblin gördü, kaçtı");
            Route(p, p.Act.Target, p.Act.Place);
            p.Running = true;
            p.Motion = Motion.Walking;
            return;
        }
    }

    // ------------------------------------------------------------------------------------------------ queries

    public static bool InRect(Vector2 p, Vector2 c, Vector2 half, float ang, float margin)
    {
        var u = new Vector2(MathF.Cos(ang), MathF.Sin(ang));
        var v = new Vector2(-u.Y, u.X);
        var d = p - c;
        return MathF.Abs(Vector2.Dot(d, u)) < half.X - margin && MathF.Abs(Vector2.Dot(d, v)) < half.Y - margin;
    }

    public string ObstacleAt(Vector2 p, float margin)
    {
        foreach (var (c, half, ang, name) in Obstacles) if (InRect(p, c, half, ang, margin)) return name;
        return null;
    }

    public Person PersonAt(Vector2 pos, float radius)
    {
        Person best = null; float bd = radius * radius;
        foreach (var p in People)
        {
            if (!p.Visible) continue;
            float d = Vector2.DistanceSquared(p.Pos, pos);
            if (d < bd) { bd = d; best = p; }
        }
        return best;
    }

    /// <summary>(outside, total) residents of the village currently inside the given circle.</summary>
    public (int outside, int total) CountOutside(Vector2 center, float radius)
    {
        int o = 0, t = 0;
        foreach (var p in People)
        {
            if (p.IsVisitor || p.Role == Role.Goblin || p.Role is Role.Innkeeper or Role.InnServant or Role.StableHand) continue;
            t++;
            if (p.Visible && Vector2.DistanceSquared(p.Pos, center) <= radius * radius) o++;
        }
        return (o, t);
    }

    public string NextPlanText(Person p)
    {
        if (p.Act == null) return "";
        var clone = _brain.Peek(p, p.Act.Until + 0.5);
        return clone == null ? "" : $"({H.Clock(p.Act.Until)}) {clone.Label.ToLowerInvariant()}";
    }
}
