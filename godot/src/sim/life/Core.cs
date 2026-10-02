// Yaşam simülasyonu — Godot'dan bağımsız (yalnız System + System.Numerics). İleride tam sim portunun parçası olacak.
// Birimler: konum metre (x doğu, y = dünya z güney), oyun zamanı DAKİKA (mutlak: gün*1440 + dakika),
// hareket GERÇEK saniyeyle (insanlar gerçek hızda yürür; saat ise zaman ölçeğiyle akar).
using System;
using System.Collections.Generic;
using System.Numerics;

namespace FD.Sim.Life;

public enum Role
{
    Farmer, Homemaker, Woodcutter, Smith, Apprentice, Shepherd, Headman, Priest,
    Innkeeper, InnServant, StableHand, Child, Elder,
    Merchant, Pilgrim, Adventurer, Goblin,
}

public enum ActKind
{
    Sleep, Home, Work, Eat, Socialize, Rest, Play, Pray, FetchWater, Wash, Garden, Shop, Sell,
    Herd, Patrol, Lurk, Guard, Drink, Travel, Wander, Flee, Chase, Inspect, Carry,
}

public enum PlaceKind
{
    Home, Plaza, Well, Market, Chapel, Smithy, Field, Pasture, Pen, Logging, Washing, Inn, InnYard,
    Camp, Tent, Lurk, Ruin, RoadEnd, Bench,
}

/// <summary>A point where one person can do something (stand at the anvil, sit on a bench...).</summary>
public sealed class Spot
{
    public Vector2 Pos;
    /// <summary>Facing as a ground direction (x, z); zero = free.</summary>
    public Vector2 Face;
    /// <summary>Tag: "work", "sit", "talk", "draw", "pray", "wash", "vendor", "customer", "chop", "fire", "sleep", "lurk"...</summary>
    public string Tag = "";
    /// <summary>Extra vertical offset for the actor (e.g. porch deck, bench seat height is handled by the anim).</summary>
    public float Y;
    public int Place = -1;
    public int TakenBy = -1;
    public bool Free => TakenBy < 0;
    public override string ToString() => $"{Tag}@{Pos.X:F0},{Pos.Y:F0}";
}

/// <summary>A place: a building, an open area (field, pasture, plaza, camp) or a landmark.</summary>
public sealed class Place
{
    public int Id;
    public PlaceKind Kind;
    public string Name = "";
    /// <summary>Entrance / arrival point (door for buildings).</summary>
    public Vector2 Door;
    public Vector2 DoorFace;
    /// <summary>Centre and radius of the area (open places) — people may walk freely inside.</summary>
    public Vector2 Center;
    public float Radius;
    /// <summary>Oriented rectangle for fields (U axis angle in radians, half size along U/V).</summary>
    public float AngleRad;
    public Vector2 Half;
    public bool IsRect;
    /// <summary>Open area id: moving between two points of the same area goes straight (no graph).</summary>
    public int Area = -1;
    public readonly List<Spot> Spots = new();
    /// <summary>Household that owns it (homes, fields) or -1.</summary>
    public int Owner = -1;
    /// <summary>Graph node where the place connects to the path network.</summary>
    public int Node = -1;
    public object Tag;

    public IEnumerable<Spot> SpotsTagged(string tag)
    {
        foreach (var s in Spots) if (s.Tag == tag) yield return s;
    }

    public bool Contains(Vector2 p, float margin = 0f)
    {
        if (IsRect)
        {
            var u = new Vector2(MathF.Cos(AngleRad), MathF.Sin(AngleRad));
            var v = new Vector2(-u.Y, u.X);
            var d = p - Center;
            return MathF.Abs(Vector2.Dot(d, u)) <= Half.X + margin && MathF.Abs(Vector2.Dot(d, v)) <= Half.Y + margin;
        }
        return Radius > 0 && Vector2.DistanceSquared(p, Center) <= (Radius + margin) * (Radius + margin);
    }

    /// <summary>Random point inside the area (rect or circle), deterministic from the rng.</summary>
    public Vector2 RandomPoint(Rng rng, float inset = 1.5f)
    {
        if (IsRect)
        {
            var u = new Vector2(MathF.Cos(AngleRad), MathF.Sin(AngleRad));
            var v = new Vector2(-u.Y, u.X);
            float a = rng.Range(-Half.X + inset, Half.X - inset), b = rng.Range(-Half.Y + inset, Half.Y - inset);
            return Center + u * a + v * b;
        }
        float r = MathF.Sqrt(rng.Next01()) * MathF.Max(0.5f, Radius - inset), t = rng.Range(0, MathF.Tau);
        return Center + new Vector2(MathF.Cos(t), MathF.Sin(t)) * r;
    }
}

public sealed class Household
{
    public int Id;
    public string Surname = "";
    public int Home = -1;
    public readonly List<int> Members = new();
    public readonly List<int> Fields = new();
    public string Trade = "";
}

/// <summary>What a person is doing (or heading to do).</summary>
public sealed class Activity
{
    public ActKind Kind;
    /// <summary>Animation while doing it (human.glb clip): Idle, Chop, Hoe, Hammer, Sit, Talk, Gather, Pray, Carry, Walk...</summary>
    public string Anim = "Idle";
    /// <summary>Tool/prop mesh shown while doing it (tool_axe, tool_hoe, tool_bucket...) or null.</summary>
    public string Tool;
    /// <summary>Tool/prop shown while walking to/from it (e.g. tool_bucket on the way back from the well).</summary>
    public string WalkTool;
    public string WalkAnim;
    public int Place = -1;
    public Spot Spot;
    public Vector2 Target;
    public Vector2 Face;
    /// <summary>Game minute when it ends (absolute).</summary>
    public double Until;
    /// <summary>Hidden inside a building while doing it.</summary>
    public bool Inside;
    /// <summary>Wander radius around Target (play, herd, patrol); 0 = stay.</summary>
    public float Wander;
    /// <summary>Run instead of walk to get there.</summary>
    public bool Hurry;
    /// <summary>Short Turkish label: "Buğday tarlasında çapa yapıyor".</summary>
    public string Label = "";
    /// <summary>Why (Turkish): "Sabah iş saati; hanenin tarlası."</summary>
    public string Reason = "";
    /// <summary>Label while walking there: "Tarlaya gidiyor".</summary>
    public string GoLabel = "";
    public float Y;
}

public enum Motion { Idle, Walking, Doing, Inside }

public sealed class Person
{
    public int Id;
    public string Name = "", Surname = "";
    public int Age;
    public bool Female;
    /// <summary>Faz 2: race id from the macro world (human, dwarf, elf, halfling, gnome, halfelf, halforc, dragonborn, tiefling, goblin)</summary>
    public string Race = "human";
    /// <summary>Faz 2: camp leader (goblin boss)</summary>
    public bool IsBoss;
    public Role Role;
    public int Household = -1;
    public int Home = -1;
    public int Work = -1;
    /// <summary>Appearance seed (hair, colours).</summary>
    public int Look;
    public float WalkSpeed = 1.35f, RunSpeed = 4.2f;
    /// <summary>0..1 (1 = full / rested / content).</summary>
    public float Food = 0.8f, Energy = 0.9f, Social = 0.7f;

    public Vector2 Pos, Dir = new(0, 1);
    public Motion Motion;
    public Activity Act;
    public readonly List<Vector2> Path = new();
    public int PathIdx;
    public bool Running;
    /// <summary>Lateral keep-right offset on paths.</summary>
    public float LaneOffset = 0.8f;
    /// <summary>Visitors (travellers) appear/disappear at the region edge.</summary>
    public bool Present = true;
    public bool IsVisitor;
    /// <summary>Carries/drives an ox cart (merchant).</summary>
    public bool HasCart;
    public float CartDist;
    /// <summary>Goblins: home spot to return to after a chase.</summary>
    public Vector2 Post;
    public double AlertUntil;
    public double NextDecision;
    /// <summary>Small history for the inspect card.</summary>
    public readonly List<string> Log = new();
    public float Wait;   // real seconds to pause (bump / greet)
    public int SubTargetTimer;

    public string FullName => string.IsNullOrEmpty(Surname) ? Name : $"{Name} {Surname}";
    public bool IsChild => Role == Role.Child;
    public bool Visible => Present && Motion != Motion.Inside;

    public void Note(string line)
    {
        Log.Add(line);
        if (Log.Count > 6) Log.RemoveAt(0);
    }
}

/// <summary>Deterministic xorshift RNG.</summary>
public sealed class Rng
{
    ulong _s;
    public Rng(ulong seed) { _s = seed * 0x9E3779B97F4A7C15UL + 0x632BE59BD9B4E019UL; if (_s == 0) _s = 1; }
    public ulong NextU() { _s ^= _s << 13; _s ^= _s >> 7; _s ^= _s << 17; return _s; }
    public float Next01() => (NextU() >> 40) / (float)(1UL << 24);
    public float Range(float a, float b) => a + (b - a) * Next01();
    public int Int(int n) => (int)(NextU() % (ulong)Math.Max(1, n));
    public bool Chance(float p) => Next01() < p;
    public T Pick<T>(IReadOnlyList<T> list) => list[Int(list.Count)];
}

public static class H
{
    /// <summary>Stable hash → [0,1).</summary>
    public static float Hash(int a, int b = 0, int c = 0)
    {
        uint h = (uint)(a * 374761393 + b * 668265263 + c * 2147483647 + 144269);
        h = (h ^ (h >> 13)) * 1274126177u;
        h ^= h >> 16;
        return (h & 0xFFFFFF) / (float)0x1000000;
    }

    public static float Len(Vector2 v) => v.Length();
    public static Vector2 Norm(Vector2 v) { float l = v.Length(); return l > 1e-5f ? v / l : new Vector2(0, 1); }
    public static Vector2 Perp(Vector2 v) => new(-v.Y, v.X);
    public static string Clock(double minute)
    {
        int m = (int)Math.Floor(minute) % 1440; if (m < 0) m += 1440;
        return $"{m / 60:00}:{m % 60:00}";
    }
}
