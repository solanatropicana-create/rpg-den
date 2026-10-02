using System;
using System.Collections.Generic;
using V2 = System.Numerics.Vector2;
using M = FD.Macro;

namespace FD.Rpg;

public enum FSide { Party = 0, Foe = 1 }

/// <summary>
/// One combatant of the real-time fight (Faz 2 C): a party member (player, companion, NPC hero; <see cref="Char"/>) or a monster.
/// <see cref="Cb"/> is the macro <c>Combatant</c> the shared rules (<c>Combat.Strike</c>) read and write: HP, AC, attack, damage
/// dice, the macro hero (class features). Positions are metres on the ground plane (x east, y = world z).
/// </summary>
public sealed class Fighter
{
    public int Id;
    public string Name;
    public FSide Side;
    public Character Char;
    public M.Combatant Cb;
    /// <summary>player | companion | hero | goblin | boss | archer</summary>
    public string Kind;
    /// <summary>LifeSim person behind this fighter (goblins, villagers, NPC heroes) or −1</summary>
    public int LifeId = -1;
    public V2 Pos, Dir = new(0, 1);
    public float Radius = 0.35f;
    /// <summary>combat run speed (m/s)</summary>
    public float Speed = 4.0f;
    /// <summary>ranged weapon range (m); 0 = melee</summary>
    public float Range;
    public float Cd;
    public float Period = 3f;
    public Fighter Target;
    public V2? MoveTo;
    /// <summary>player order: attack | move | cast | potion | bandage | retreat | hold; null = the class's own judgement</summary>
    public string Order;
    public string OrderSpell;
    public Fighter OrderTarget;
    public V2 OrderPoint;
    public bool Down, Dead, Fled, Stable, Fleeing;
    /// <summary>Tur 1 C: the stance (party: the character's; foes fight as aggressive)</summary>
    public string Stance => Char?.Stance ?? Stances.Aggressive;
    /// <summary>Tur 1 C: seconds a foe has been on this one's heels during a move order (caught → turn and fight)</summary>
    public float CaughtT;
    /// <summary>Tur 1 C: fight time of the last blow taken (a defender answers it)</summary>
    public float HitT = -99f;
    /// <summary>Tur 1 C: hold stance this frame — no step, only blows within reach</summary>
    public bool Rooted;
    public int DeathOk, DeathFail;
    public float DeathCd;
    public float SleepUntil, SlowUntil;
    public V2 FleeTo;
    /// <summary>view hints</summary>
    public string Anim = "Idle";
    public float AnimLock;
    public bool Moving;
    public float Hurt;
    public int Kills, DexMod;
    /// <summary>D: a permanent wound was dealt in this fight (at most one per fight)</summary>
    public string NewWound;
    public bool IsBoss => Kind == "boss";
    public bool IsPlayer => Kind == "player";
    public float Hp => (float)Cb.Hp;
    public float MaxHp => (float)Cb.MaxHp;
    public bool Gone => Dead || Fled;
    public bool Asleep(float now) => SleepUntil > now;
    /// <summary>can act now (not down, dead, fled or asleep)</summary>
    public bool Active(float now) => !Dead && !Down && !Fled && !Asleep(now);
    public bool Standing => !Dead && !Down && !Fled;
    public override string ToString() => $"{Name}({Hp:F0}/{MaxHp:F0})";
}

/// <summary>What happened (for the view: floating numbers, the dice log, sounds).</summary>
public sealed class FightEvent
{
    public float T;
    /// <summary>start | attack | spell | heal | down | dead | save | stable | up | sleep | wake | flee | rout | potion | bandage | wind | join | order | wound | end</summary>
    public string Kind;
    public Fighter A, B;
    /// <summary>short line over the head: "17+5 → 22 vs ZS 15 · isabet"</summary>
    public string Short;
    /// <summary>log line</summary>
    public string Text;
    /// <summary>details shown when paused (dice breakdown)</summary>
    public string Detail;
    public bool Hit, Crit, Fumble;
    public int Value;
}

/// <summary>
/// Faz 2 C: pausable real-time d20 fight. Each fighter acts about every 3 real seconds (<see cref="Fighter.Period"/>); movement and
/// range are real (metres). Every attack goes through the macro's <c>Combat.Strike</c> (the same rule the sim's battles use:
/// d20 + attack vs AC, natural 20 crit doubles the dice, natural 1 misses, sneak attack, …). Classes act by themselves unless
/// the player gave an order: the fighter charges, the rogue looks for a flank, the wizard keeps distance and falls back when out of
/// slots, the cleric raises and heals. A side whose losses pass its morale breaks and runs (goblins really leave). Party members
/// at 0 HP go down and roll death saves; monsters die. Godot-free and deterministic for a seed and an update sequence.
/// </summary>
public sealed class Fight
{
    public readonly List<Fighter> F = new();
    public readonly List<FightEvent> Log = new();
    public event Action<FightEvent> Ev;
    public readonly M.Rng Rng;
    public float T;
    public bool Paused, Over;
    public FSide? Winner;
    /// <summary>foe morale: they break when this share of them is down, dead or gone (macro: goblins 0.5); boss down → break</summary>
    public float FoeMorale = 0.5f;
    /// <summary>hired companions run when the party's losses pass this and they are hurt (the player never runs on their own)</summary>
    public float CompanionMorale = 0.75f;
    /// <summary>where routed foes run to (their camp, the forest)</summary>
    public V2 FoeHome;
    public bool FoesRouted;
    int _nextId = 1;
    int _initFoes, _initParty;
    /// <summary>game-time seconds per real second for spell durations (sleep, slow) — real seconds here</summary>
    public const float SleepSeconds = 24f, SlowSeconds = 9f, MeleeReach = 1.7f, DeathSavePeriod = 3f, FleeDistance = 32f;

    public Fight(double seed) { Rng = new M.Rng(seed); }

    // ------------------------------------------------------------------------------------------------ building
    public Fighter AddCharacter(Character c, M.Hero hero, FSide side, V2 pos, string kind)
    {
        var at = c.Attack();
        var cb = new M.Combatant
        {
            Name = c.Name, Side = side == FSide.Party ? "A" : "B", Hp = c.Hp, MaxHp = c.MaxHp, Ac = c.Ac, Atk = at.atk,
            Dmg = new List<double> { at.n, at.sides, at.bonus }, Attacks = 1, Hero = hero, Kind = "hero", Uses = new M.JsObj<double>(),
            Evil = c.Align == "evil",
        };
        if (hero != null) { hero.Level = c.Level; }
        var f = new Fighter
        {
            Id = _nextId++, Name = c.Name, Side = side, Char = c, Cb = cb, Kind = kind, Pos = pos, Range = at.range,
            Speed = 4.2f * c.SpeedFactor, DexMod = c.Mod(Rules.DEX), Down = c.Down, Dead = c.Dead,
        };
        f.Period = Math.Clamp(3f - 0.1f * f.DexMod, 2.5f, 3.4f);
        f.Cd = 0.6f + (float)Rng.Next() * 1.2f;
        F.Add(f);
        return f;
    }

    /// <summary>A monster from the macro tables (goblin, goblinBoss…).</summary>
    public Fighter AddMonster(string monster, string name, FSide side, V2 pos, string kind, int lifeId = -1, float range = 0)
    {
        var cb = M.Combat.Unit(M.D.MONSTERS[monster], side == FSide.Party ? "A" : "B", kind == "boss" ? "boss" : "monster");
        cb.Evil = true;
        cb.Name = name;
        var f = new Fighter
        {
            Id = _nextId++, Name = name, Side = side, Cb = cb, Kind = kind, Pos = pos, LifeId = lifeId, Range = range,
            Speed = kind == "boss" ? 3.6f : 3.8f, DexMod = 2, Radius = kind == "boss" ? 0.42f : 0.32f,
        };
        f.Period = kind == "boss" ? 3.2f : 2.9f;
        f.Cd = 0.4f + (float)Rng.Next() * 1.4f;
        F.Add(f);
        return f;
    }

    public void Begin(string why)
    {
        _initFoes = Count(FSide.Foe, x => !x.Dead);
        _initParty = Count(FSide.Party, x => !x.Dead);
        Emit(new FightEvent { Kind = "start", Text = why });
    }

    /// <summary>A monster joins a running fight (woken in its tent, came running at the noise). Counts for morale from now on.
    /// Not after the foes broke: they would only join the rout.</summary>
    public Fighter Reinforce(string monster, string name, V2 pos, string kind, int lifeId, float range, string why)
    {
        var f = AddMonster(monster, name, FSide.Foe, pos, kind, lifeId, range);
        _initFoes++;
        if (FoesRouted) StartFlee(f);
        Emit(new FightEvent { Kind = "join", A = f, Short = "katıldı!", Text = why });
        return f;
    }

    /// <summary>A party member joins a running fight (a companion who caught up, an NPC hero passing by).</summary>
    public Fighter ReinforceParty(Character c, M.Hero hero, V2 pos, string kind, string why)
    {
        var f = AddCharacter(c, hero, FSide.Party, pos, kind);
        _initParty++;
        Emit(new FightEvent { Kind = "join", A = f, Short = "katıldı!", Text = why });
        return f;
    }

    int Count(FSide s, Predicate<Fighter> p) { int n = 0; foreach (var f in F) if (f.Side == s && p(f)) n++; return n; }
    public IEnumerable<Fighter> Of(FSide s) { foreach (var f in F) if (f.Side == s) yield return f; }

    /// <summary>a line from outside the rules (orders, the director's notes) into the log</summary>
    public void Post(string kind, Fighter a, string text, string shortText = null) => Emit(new FightEvent { Kind = kind, A = a, Text = text, Short = shortText });

    void Emit(FightEvent e)
    {
        e.T = T;
        Log.Add(e);
        if (Log.Count > 400) Log.RemoveAt(0);
        Ev?.Invoke(e);
    }

    // ------------------------------------------------------------------------------------------------ update
    public void Update(float dt)
    {
        if (Over || Paused || dt <= 0) return;
        dt = MathF.Min(dt, 0.1f);
        T += dt;
        _dt = dt;
        foreach (var f in F)
        {
            if (f.Gone) continue;
            f.Hurt = MathF.Max(0, f.Hurt - dt);
            f.AnimLock = MathF.Max(0, f.AnimLock - dt);
            if (f.Down) { DeathSaves(f, dt); continue; }
            if (f.Asleep(T)) { f.Moving = false; f.Anim = "Die"; continue; }
            if (f.SleepUntil > 0 && !f.Asleep(T)) { f.SleepUntil = 0; Emit(new FightEvent { Kind = "wake", A = f, Short = "uyandı", Text = $"{f.Name} uyandı." }); }
            f.Cd -= dt;
            if (f.Fleeing) { FleeStep(f, dt); continue; }
            Think(f);
            ActOrMove(f, dt);
        }
        Separate();
        Morale();
        CheckEnd(dt);
    }

    float _dt, _apartT;
    /// <summary>Tur 1 C: the party got away (every member still standing is far from every foe still fighting)</summary>
    public bool Escaped;
    /// <summary>how far the party must be from every foe for the fight to end as an escape (m)</summary>
    public const float EscapeDistance = 26f;

    // ------------------------------------------------------------------------------------------------ decisions
    Fighter Nearest(Fighter f, FSide side, Func<Fighter, bool> ok = null)
    {
        Fighter best = null; float bd = float.MaxValue;
        foreach (var o in F)
        {
            if (o.Side != side || !o.Standing || o == f) continue;
            if (ok != null && !ok(o)) continue;
            float d = V2.DistanceSquared(o.Pos, f.Pos);
            if (d < bd) { bd = d; best = o; }
        }
        return best;
    }

    static FSide Other(FSide s) => s == FSide.Party ? FSide.Foe : FSide.Party;
    float Dist(Fighter a, Fighter b) => V2.Distance(a.Pos, b.Pos);
    bool InMelee(Fighter a, Fighter b) => Dist(a, b) <= MeleeReach + a.Radius + b.Radius - 0.7f;

    /// <summary>Pick what to do: the player's order if any, otherwise the class's (or monster's) judgement.</summary>
    /// <summary>Pick what to do: the player's order if any (attack that one, go there), otherwise the stance and the class's (or
    /// monster's) judgement. Tur 1 C (Kenshi): no buttons — equipment and class decide: anyone drinks a potion when nearly dead,
    /// binds a fallen friend when no foe is near, a fighter takes his second wind, casters follow their book.</summary>
    void Think(Fighter f)
    {
        f.MoveTo = null;
        f.Rooted = false;
        if (f.Order != null && FollowOrder(f)) return;
        if (f.Side == FSide.Foe) { ThinkFoe(f); return; }
        var st = f.Stance;
        if (st == Stances.Passive) { f.Target = null; return; }
        if (st == Stances.Flee) { if (Nearest(f, Other(f.Side)) != null) { StartFlee(f); Emit(new FightEvent { Kind = "flee", A = f, Short = "kaçıyor", Text = $"{f.Name} dövüşmeden uzaklaşıyor (duruş: Kaç)." }); } return; }
        if (f.Char != null && f.Hp < f.MaxHp * 0.3f && f.Char.Inv.Has("potion") && f.Cd <= 0) { DrinkPotion(f, f); return; }
        if (f.Char?.Cls != "cleric" && TendFallen(f)) return;
        if (st == Stances.Defend && !Threatened(f)) { f.Target = null; StayNear(f); return; }
        var cls = f.Char?.Cls ?? "fighter";
        f.Rooted = st == Stances.Hold;
        switch (cls)
        {
            case "wizard": ThinkCaster(f); break;
            case "cleric": ThinkCaster(f); break;
            case "rogue": ThinkRogue(f); break;
            default: ThinkFighter(f); break;
        }
        if (st == Stances.Hold)
        {
            // never a step: keep to foes within reach (blade, bow or spell), else wait
            f.MoveTo = null;
            if (f.Target != null && f.Target.Side != f.Side && !InReach(f, f.Target))
                f.Target = Nearest(f, Other(f.Side), o => InReach(f, o));
        }
    }

    bool InReach(Fighter f, Fighter t)
    {
        float reach = f.Range > 0 ? f.Range : MeleeReach + f.Radius + t.Radius - 0.7f;
        if (f.Char != null) foreach (var id in f.Char.Book()) { var sp = Spells.Get(id); if (sp != null && sp.Kind != SpellKind.Heal && (sp.Level == 0 || f.Char.Slots > 0)) reach = MathF.Max(reach, sp.Range); }
        return Dist(f, t) <= reach;
    }

    /// <summary>Defend stance: is there anything to answer — a blow taken a moment ago, a foe close by, or a foe coming at (or
    /// shooting at) someone of the party nearby? (A foe minding its own business — asleep, running, not after anyone — is left be.)</summary>
    bool Threatened(Fighter f)
    {
        if (T - f.HitT < 6f) return true;
        foreach (var o in F)
        {
            if (o.Side == f.Side || !o.Standing || o.Fleeing) continue;
            if (o.Asleep(T)) { if (Dist(o, f) < 15f) return true; continue; }   // a foe put to sleep nearby: finish it
            if (Dist(o, f) < 8f) return true;
            var t = o.Target;
            if (t == null || t.Side != f.Side || t.Dead) continue;
            float reach = o.Range > 0 ? o.Range + 2f : 12f;
            if (Dist(o, t) < reach && Dist(f, t) < 20f) return true;
        }
        return false;
    }

    /// <summary>Nothing to answer: keep near the lead of the party.</summary>
    void StayNear(Fighter f)
    {
        Fighter lead = null;
        foreach (var o in F) if (o.Side == f.Side && o.IsPlayer && o.Standing) lead = o;
        if (lead == null || lead == f) return;
        if (Dist(f, lead) > 4.5f) f.MoveTo = lead.Pos + Norm(f.Pos - lead.Pos, f.Dir) * 2.5f;
    }

    /// <summary>A friend down and bleeding, no foe within 5 m of them: go and bind the wound (anyone with a bandage).</summary>
    bool TendFallen(Fighter f)
    {
        if (f.Char == null || !f.Char.Inv.Has("bandage")) return false;
        Fighter best = null;
        foreach (var a in F)
        {
            if (a.Side != f.Side || !a.Down || a.Dead || a.Stable || a == f || Dist(a, f) > 14f) continue;
            var foe = Nearest(a, Other(f.Side));
            if (foe != null && Dist(foe, a) < 5f) continue;
            if (best == null || Dist(f, a) < Dist(f, best)) best = a;
        }
        if (best == null) return false;
        f.Target = best;
        if (Dist(f, best) > 1.8f) { f.MoveTo = best.Pos; return true; }
        if (f.Cd <= 0) Bandage(f, best);
        return true;
    }

    bool FollowOrder(Fighter f)
    {
        switch (f.Order)
        {
            case "hold": f.Target = null; return true;
            case "move":
            {
                if (V2.Distance(f.Pos, f.OrderPoint) < 0.6f) { f.Order = null; f.CaughtT = 0; return false; }
                // Tur 1 C: running off; a foe on the heels hitting this one for a while catches them — they turn and fight by their stance
                var foe = Nearest(f, Other(f.Side));
                if (foe != null && InMelee(f, foe) && foe.Target == f) f.CaughtT += _dt; else f.CaughtT = MathF.Max(0, f.CaughtT - _dt * 0.5f);
                if (f.CaughtT > 1.6f && f.Stance is not (Stances.Flee or Stances.Passive))
                {
                    f.Order = null; f.CaughtT = 0;
                    Emit(new FightEvent { Kind = "caught", A = f, Short = "yakalandı!", Text = $"{f.Name} kaçamadı; dönüp dövüşüyor." });
                    return false;
                }
                f.MoveTo = f.OrderPoint; f.Target = null; return true;
            }
            case "attack":
                if (f.OrderTarget == null || !f.OrderTarget.Standing) { f.Order = null; return false; }
                f.Target = f.OrderTarget; return true;
            case "cast":
            {
                var sp = Spells.Get(f.OrderSpell);
                if (sp == null || (sp.Level > 0 && f.Char.Slots <= 0)) { f.Order = null; return false; }
                var tgt = f.OrderTarget ?? (sp.Kind == SpellKind.Heal ? f : Nearest(f, Other(f.Side)));
                if (tgt == null || (sp.Kind != SpellKind.Heal && !tgt.Standing)) { f.Order = null; return false; }
                f.Target = tgt;
                if (Dist(f, tgt) > sp.Range + tgt.Radius) { f.MoveTo = tgt.Pos; return true; }
                if (f.Cd <= 0) { Cast(f, sp, tgt); f.Order = null; }
                return true;
            }
            case "potion":
            {
                var who = f.OrderTarget ?? f;
                if (!f.Char.Inv.Has("potion")) { f.Order = null; return false; }
                if (who != f && Dist(f, who) > 1.8f) { f.MoveTo = who.Pos; return true; }
                if (f.Cd <= 0) { DrinkPotion(f, who); f.Order = null; }
                return true;
            }
            case "bandage":
            {
                var who = f.OrderTarget;
                if (who == null || !who.Down || !f.Char.Inv.Has("bandage")) { f.Order = null; return false; }
                if (Dist(f, who) > 1.8f) { f.MoveTo = who.Pos; return true; }
                if (f.Cd <= 0) { Bandage(f, who); f.Order = null; }
                return true;
            }
            case "wind":
                if (f.Char?.Uses.GetValueOrDefault("secondWind") > 0) SecondWind(f);
                f.Order = null; return false;
        }
        f.Order = null;
        return false;
    }

    void ThinkFighter(Fighter f)
    {
        if (f.Char != null && f.Hp < f.MaxHp * 0.5f && f.Char.Uses.GetValueOrDefault("secondWind") > 0) SecondWind(f);
        // protect: the foe standing over a downed or hurt friend first, else the nearest
        Fighter t = null;
        foreach (var a in F)
        {
            if (a.Side != f.Side || a == f || !(a.Down || a.Hp < a.MaxHp * 0.4f)) continue;
            var foe = Nearest(a, Other(f.Side));
            if (foe != null && Dist(foe, a) < 3f && Dist(foe, f) < 15f) { t = foe; break; }
        }
        f.Target = Keep(f) ?? t ?? Nearest(f, Other(f.Side));
    }

    /// <summary>keep the current target while it stands and is not much farther than the nearest</summary>
    Fighter Keep(Fighter f)
    {
        var t = f.Target;
        if (t == null || !t.Standing || t.Side == f.Side) return null;
        var n = Nearest(f, Other(f.Side));
        return n != null && Dist(f, t) > Dist(f, n) + 4f ? null : t;
    }

    void ThinkRogue(Fighter f)
    {
        // a foe already engaged by a friend (sneak attack), else the nearest
        Fighter t = Keep(f);
        if (t == null)
            foreach (var o in F)
            {
                if (o.Side == f.Side || !o.Standing) continue;
                foreach (var a in F) if (a.Side == f.Side && a != f && a.Standing && InMelee(a, o)) { if (t == null || Dist(f, o) < Dist(f, t)) t = o; }
            }
        f.Target = t ?? Nearest(f, Other(f.Side));
        if (f.Target != null && f.Range <= 0 && !InMelee(f, f.Target))
        {
            // come around to its back
            var back = f.Target.Pos - f.Target.Dir * 1.1f;
            if (V2.Distance(f.Pos, back) > 0.8f && V2.Distance(f.Pos, f.Target.Pos) < 6f) f.MoveTo = back;
        }
    }

    /// <summary>Tur 1 C (spellbook v1): the caster goes down their book (<see cref="Character.Book"/>, ordered by the player) and
    /// uses the first spell that fits now — a crowd for the area spells, the chief or a nearly dead foe for the missiles, a fallen or
    /// badly hurt friend for the healing, anyone in reach for the cantrips. A wizard with a foe at arm's length steps back unless the
    /// spell is for exactly that; with nothing fitting (no slots, no reach) the weapon.</summary>
    void ThinkCaster(Fighter f)
    {
        var c = f.Char;
        var near = Nearest(f, Other(f.Side));
        float dn = near != null ? Dist(f, near) : 999f;
        foreach (var id in c.Book())
        {
            var sp = Spells.Get(id);
            if (sp == null || (sp.Level > 0 && c.Slots <= 0)) continue;
            Fighter t = null; V2? pt = null;
            switch (id)
            {
                case "curewounds":
                {
                    Fighter down = null, hurt = null;
                    foreach (var a in F)
                    {
                        if (a.Side != f.Side || a.Dead || a.Fled) continue;
                        if (a.Down && (down == null || Dist(f, a) < Dist(f, down))) down = a;
                        else if (!a.Down && a.Hp < a.MaxHp * 0.4f && (hurt == null || a.Hp < hurt.Hp)) hurt = a;
                    }
                    t = down ?? hurt;
                    break;
                }
                case "burninghands":
                    if (near != null && dn <= 4.5f && InCone(f, near.Pos - f.Pos, 4.5f, 0.55f) >= 2) t = near;
                    break;
                case "sleep":
                {
                    var (p, n) = Crowd(f, 6f, 27f);
                    if (n >= 3) pt = p;
                    break;
                }
                case "magicmissile":
                    foreach (var o in F)
                        if (o.Side != f.Side && o.Standing && Dist(f, o) <= sp.Range && (o.IsBoss || o.Hp <= 8) && (t == null || o.IsBoss)) t = o;
                    break;
                default:
                {
                    if (sp.Kind == SpellKind.Heal || near == null) break;
                    Fighter weak = null;
                    foreach (var o in F) if (o.Side != f.Side && o.Standing && Dist(f, o) <= sp.Range && (weak == null || o.Hp < weak.Hp)) weak = o;
                    t = weak ?? (f.Rooted ? null : near);
                    break;
                }
            }
            if (t == null && pt == null) continue;
            bool close = c.Cls == "wizard" && dn < 3.5f && id != "burninghands";
            if (close && !f.Rooted) { f.MoveTo = f.Pos + V2.Normalize(f.Pos - near.Pos + new V2(0.001f, 0)) * 5f; f.Target = near; return; }
            f.Target = t ?? near;
            var aim = t != null ? t.Pos : pt.Value;
            float range = sp.Kind == SpellKind.Heal ? 1.6f : sp.Range + (t?.Radius ?? 0f);
            if (V2.Distance(f.Pos, aim) > range) { f.MoveTo = aim; return; }
            if (f.Cd <= 0) Cast(f, sp, t, pt);
            return;
        }
        if (c.Cls == "cleric" && TendFallen(f)) return;
        if (c.Cls == "wizard" && near != null && dn < 3.5f && !f.Rooted) { f.MoveTo = f.Pos + V2.Normalize(f.Pos - near.Pos + new V2(0.001f, 0)) * 5f; f.Target = near; return; }
        ThinkFighter(f);
    }

    void ThinkFoe(Fighter f)
    {
        // the weakest of the nearby (goblins gang up), the nearest otherwise; archers keep their distance
        var near = Nearest(f, FSide.Party);
        if (near == null) { f.Target = null; return; }
        Fighter t = Keep(f);
        if (t == null)
        {
            t = near;
            foreach (var o in F)
                if (o.Side == FSide.Party && o.Standing && Dist(f, o) < Dist(f, near) + 3f && o.Hp < t.Hp) t = o;
        }
        f.Target = t;
        if (f.Range > 0 && Dist(f, near) < 4f) f.MoveTo = f.Pos + V2.Normalize(f.Pos - near.Pos + new V2(0.001f, 0)) * 4f;
    }

    // ------------------------------------------------------------------------------------------------ acting and moving
    void ActOrMove(Fighter f, float dt)
    {
        var t = f.Target;
        V2? go = f.MoveTo;
        bool canHit = false;
        if (go == null && t != null && t.Side != f.Side && t.Standing)
        {
            float reach = f.Range > 0 ? f.Range : MeleeReach + f.Radius + t.Radius - 0.7f;
            if (Dist(f, t) > reach) { if (!f.Rooted) go = t.Pos; }
            else canHit = true;
            f.Dir = Norm(t.Pos - f.Pos, f.Dir);
        }
        if (f.Rooted) go = null;
        if (go is V2 g && V2.Distance(f.Pos, g) > 0.15f)
        {
            // Tur 1 C: running off on an order (right click on the ground, WASD) is a run
            float run = f.Order == "move" ? 1.25f : 1f;
            float sp = f.Speed * run * (f.SlowUntil > T ? 0.5f : 1f) * (f.AnimLock > 0 ? 0.3f : 1f);
            var d = g - f.Pos;
            float L = d.Length();
            f.Pos += d / L * MathF.Min(L, sp * dt);
            f.Dir = Norm(V2.Lerp(f.Dir, d / L, MathF.Min(1, dt * 10f)), f.Dir);
            f.Moving = true;
            if (f.AnimLock <= 0) f.Anim = sp > 2.4f ? "Run" : "Walk";
        }
        else
        {
            f.Moving = false;
            if (f.AnimLock <= 0) f.Anim = "Idle";
        }
        if (canHit && f.Cd <= 0) Attack(f, t);
    }

    static V2 Norm(V2 v, V2 fallback) { float l = v.Length(); return l > 1e-4f ? v / l : fallback; }

    /// <summary>Push overlapping fighters apart (standing ones only).</summary>
    void Separate()
    {
        for (int i = 0; i < F.Count; i++)
        {
            var a = F[i];
            if (!a.Standing) continue;
            for (int j = i + 1; j < F.Count; j++)
            {
                var b = F[j];
                if (!b.Standing) continue;
                var d = b.Pos - a.Pos;
                float L = d.Length(), min = a.Radius + b.Radius + 0.15f;
                if (L >= min || L < 1e-5f) continue;
                var push = d / L * (min - L) * 0.5f;
                a.Pos -= push; b.Pos += push;
            }
        }
    }

    // ------------------------------------------------------------------------------------------------ attacks
    /// <summary>Sneak attack is allowed when a rogue with a finesse or ranged weapon hits a foe that a friend stands next to, or
    /// strikes from behind (D&amp;D: advantage or an ally within 5 ft).</summary>
    bool SneakOk(Fighter f, Fighter t)
    {
        if (f.Char?.Cls != "rogue") return false;
        var w = f.Char.WeaponDef;
        if (w == null || !(w.Finesse || w.Range > 0)) return false;
        foreach (var a in F) if (a.Side == f.Side && a != f && a.Standing && Dist(a, t) <= MeleeReach + 0.4f) return true;
        return V2.Dot(t.Dir, Norm(f.Pos - t.Pos, t.Dir)) < -0.35f;
    }

    void Attack(Fighter f, Fighter t)
    {
        int n = Math.Max(1, (int)f.Cb.Attacks);
        for (int k = 0; k < n && t.Standing; k++)
        {
            bool sleeping = t.Asleep(T);
            var ctx = new M.Combat.StrikeCtx
            {
                Sneak = SneakOk(f, t),
                Adv = sleeping || (t.Down && f.Range <= 0),
                Dis = f.Range > 0 && Nearest(f, Other(f.Side)) is Fighter nf && Dist(f, nf) < 1.8f,
                AutoCritOnHit = (sleeping || t.Down) && f.Range <= 0 && Dist(f, t) < 2.2f,
            };
            double hpBefore = t.Cb.Hp;
            var r = M.Combat.Strike(Rng, f.Cb, t.Cb, ctx);
            var e = new FightEvent { Kind = "attack", A = f, B = t, Hit = r.Hit, Crit = r.Crit, Fumble = r.Fumble, Value = (int)r.Dmg };
            string wname = f.Char?.Attack().name ?? (f.Range > 0 ? "kısa yay" : "pala");
            int d = (int)r.D20, atk = (int)f.Cb.Atk, ac = (int)t.Cb.Ac;
            string rolls = ctx.Adv || ctx.Dis ? $"[{(int)r.D20b}/{d}{(ctx.Adv ? " avantaj" : " dezavantaj")}]" : "";
            if (r.Fumble)
            {
                e.Short = "1 · ıska!";
                e.Text = $"{f.Name} → {t.Name}: doğal 1, ıska.";
                e.Detail = $"d20 = 1 {rolls} (doğal 1 her zaman ıskalar)";
            }
            else if (!r.Hit)
            {
                e.Short = $"{d}{Rules.Signed(atk)} → {d + atk} vs ZS {ac} · ıska";
                e.Text = $"{f.Name} → {t.Name} ({wname}): {d}{Rules.Signed(atk)} = {d + atk} vs ZS {ac}, ıska.";
                e.Detail = $"d20 {d} {rolls} + saldırı {atk} = {d + atk} < zırh sınıfı {ac}";
            }
            else
            {
                string dice = r.Dice != null ? string.Join("+", r.Dice.ConvertAll(x => ((int)x).ToString())) : "";
                string parts = r.Parts.Count > 0 ? " " + string.Join(" ", r.Parts.ConvertAll(p => $"+{p.L.ToLowerInvariant()} {(int)p.V}")) : "";
                e.Short = r.Crit ? $"{d} KRİTİK! · {(int)r.Dmg} hasar" : $"{d}{Rules.Signed(atk)} → {d + atk} vs ZS {ac} · isabet · {(int)r.Dmg}";
                e.Text = $"{f.Name} → {t.Name} ({wname}): {(r.Crit ? $"doğal {d}, KRİTİK" : $"{d}{Rules.Signed(atk)} = {d + atk} vs ZS {ac}")}, {(int)r.Dmg} hasar.";
                e.Detail = $"d20 {d} {rolls} + {atk} vs ZS {ac}; hasar zarları {dice}{Rules.Signed((int)f.Cb.Dmg[2])}{parts}{(r.Crit ? " (kritik: zarlar iki kat)" : "")}";
            }
            f.Anim = "Attack"; f.AnimLock = 0.7f;
            Emit(e);
            if (r.Hit) Damaged(t, f, hpBefore, r.Crit);
        }
        f.Cd = f.Period + (float)Rng.Next() * 0.4f - 0.2f;
    }

    /// <summary>After damage landed on <paramref name="t"/>: hit reaction, down/dead, sleep broken, death-save failures on the fallen.</summary>
    void Damaged(Fighter t, Fighter by, double hpBefore, bool crit)
    {
        t.Hurt = 0.4f;
        t.HitT = T;
        if (t.SleepUntil > T) { t.SleepUntil = 0; Emit(new FightEvent { Kind = "wake", A = t, Short = "uyandı", Text = $"{t.Name} acıyla uyandı." }); }
        if (t.Down)
        {
            // hit while down: one failed death save (two on a crit); massive damage kills
            t.Cb.Hp = 0;
            t.DeathFail += crit ? 2 : 1;
            Emit(new FightEvent { Kind = "save", A = t, Short = crit ? "✖✖" : "✖", Text = $"{t.Name} yerde yara aldı: {(crit ? "iki" : "bir")} ölüm zarı kaybı ({t.DeathFail}/3)." });
            if (t.DeathFail >= 3) { Die(t, by); return; }
            if (crit) MaybeWound(t, "yerdeyken yediği kritik darbe");
            return;
        }
        if (t.Cb.Hp > 0) { if (t.AnimLock <= 0.2f) { t.Anim = "Hit"; t.AnimLock = 0.35f; } return; }
        if (t.Side == FSide.Party && t.Char != null)
        {
            // D&D: massive damage (the rest ≥ max HP) kills outright; otherwise unconscious, dying
            if (-t.Cb.Hp >= t.Cb.MaxHp) { t.Cb.Hp = 0; Die(t, by); return; }
            t.Cb.Hp = 0;
            t.Down = true; t.Stable = false; t.DeathOk = t.DeathFail = 0; t.DeathCd = DeathSavePeriod;
            t.Anim = "Die"; t.AnimLock = 0;
            Emit(new FightEvent { Kind = "down", A = t, B = by, Short = "yere düştü!", Text = $"{t.Name} yere düştü ({by?.Name}). Ölüm zarları başlıyor." });
            return;
        }
        Die(t, by);
    }

    void Die(Fighter t, Fighter by)
    {
        t.Dead = true; t.Down = false; t.Anim = "Die"; t.Moving = false;
        if (by != null) by.Kills++;
        Emit(new FightEvent { Kind = "dead", A = t, B = by, Short = t.Side == FSide.Party ? "öldü" : "öldü", Text = $"{t.Name} öldü{(by != null ? $" ({by.Name})" : "")}." });
    }

    void DeathSaves(Fighter f, float dt)
    {
        f.Moving = false; f.Anim = "Die";
        if (f.Stable || f.Dead) return;
        f.DeathCd -= dt;
        if (f.DeathCd > 0) return;
        f.DeathCd = DeathSavePeriod;
        double d = Rng.D20();
        if (d == 20) { Raise(f, 1, null, "doğal 20: kendine geldi"); return; }
        if (d == 1) f.DeathFail += 2;
        else if (d >= 10) f.DeathOk++;
        else f.DeathFail++;
        Emit(new FightEvent { Kind = "save", A = f, Short = $"ölüm zarı {(int)d} · {f.DeathOk}✔ {f.DeathFail}✖", Text = $"{f.Name} ölüm zarı: {(int)d} ({(d >= 10 ? "başarı" : "kayıp")}) — {f.DeathOk} başarı, {f.DeathFail} kayıp." });
        if (f.DeathFail >= 3) { Die(f, null); return; }
        if (d < 10) MaybeWound(f, d == 1 ? "doğal 1'lik ölüm zarı" : "kötü giden ölüm zarı");
        if (f.DeathOk >= 3) { f.Stable = true; Emit(new FightEvent { Kind = "stable", A = f, Short = "dengelendi", Text = $"{f.Name} dengelendi; baygın ama kanaması durdu." }); }
    }

    /// <summary>D: chance of a permanent wound after a bad death save or a critical hit while down (D&amp;D DMG lingering injuries,
    /// simplified): one in four — a lost eye, a limp, or a deep scar (half of them). At most one per fighter per fight.</summary>
    public const double WoundChance = 0.25;

    void MaybeWound(Fighter f, string why)
    {
        if (f.Char == null || f.NewWound != null || f.Dead || !Rng.Chance(WoundChance)) return;
        double r = Rng.Next();
        string kind = r < 0.25 ? "eye" : r < 0.5 ? "limp" : "scar";
        if (f.Char.HasWound(kind)) kind = f.Char.HasWound("scar") ? (f.Char.HasWound("limp") ? "eye" : "limp") : "scar";
        if (f.Char.HasWound(kind)) return;
        f.NewWound = kind;
        Emit(new FightEvent { Kind = "wound", A = f, Short = $"kalıcı yara: {Wound.Name(kind).ToLowerInvariant()}!", Text = $"{f.Name} kalıcı bir yara aldı: {Wound.Name(kind)} ({why}).", Detail = Wound.Effect(kind) });
    }

    void Raise(Fighter f, int hp, Fighter by, string why)
    {
        f.Down = false; f.Stable = false; f.DeathOk = f.DeathFail = 0;
        f.Cb.Hp = Math.Max(1, Math.Min(f.Cb.MaxHp, hp));
        f.Anim = "Idle"; f.AnimLock = 0.8f; f.Cd = MathF.Max(f.Cd, 1.2f);
        Emit(new FightEvent { Kind = "up", A = f, B = by, Short = $"ayağa kalktı ({hp})", Text = $"{f.Name} ayağa kalktı: {why}." });
    }

    void Heal(Fighter f, Fighter by, int amount, string what)
    {
        if (f.Dead) return;
        if (f.Down) { Raise(f, amount, by, $"{what} (+{amount})"); return; }
        f.Cb.Hp = Math.Min(f.Cb.MaxHp, f.Cb.Hp + amount);
        Emit(new FightEvent { Kind = "heal", A = by ?? f, B = f, Value = amount, Short = $"+{amount} can", Text = $"{(by != null && by != f ? by.Name + ": " : "")}{f.Name} {amount} can buldu ({what})." });
    }

    // ------------------------------------------------------------------------------------------------ class actions
    void SecondWind(Fighter f)
    {
        f.Char.Uses["secondWind"] = 0;
        int lvl = f.Char.Level;
        var r = Rng.Roll(1, 10);
        int heal = (int)r[0] + lvl;
        f.Cb.Hp = Math.Min(f.Cb.MaxHp, f.Cb.Hp + heal);
        Emit(new FightEvent { Kind = "wind", A = f, Value = heal, Short = $"derin nefes +{heal}", Text = $"{f.Name} derin bir nefes aldı: 1d10 ({(int)r[0]}) + {lvl} = {heal} can." });
    }

    void DrinkPotion(Fighter f, Fighter who)
    {
        if (!f.Char.Inv.Remove("potion")) return;
        var r = Rng.Roll(2, 4);
        int heal = (int)(r[0] + r[1]) + 2;
        Emit(new FightEvent { Kind = "potion", A = f, B = who, Value = heal, Short = "iksir", Text = $"{f.Name} {(who == f ? "bir şifa iksiri içti" : $"{who.Name}'a iksir içirdi")}: 2d4 ({(int)r[0]}+{(int)r[1]}) + 2." });
        Heal(who, f, heal, "şifa iksiri");
        f.Anim = "Attack"; f.AnimLock = 0.5f;
        f.Cd = f.Period;
    }

    void Bandage(Fighter f, Fighter who)
    {
        if (!f.Char.Inv.Remove("bandage")) return;
        Emit(new FightEvent { Kind = "bandage", A = f, B = who, Short = "sargı", Text = $"{f.Name}, {who.Name}'ın yarasını sardı." });
        if (who.Down) Raise(who, 1, f, "yarası sarıldı");
        f.Anim = "Gather"; f.AnimLock = 1.2f;
        f.Cd = f.Period + 0.5f;
    }

    /// <summary>Foes inside a cone in front of <paramref name="f"/> along <paramref name="dir"/>.</summary>
    int InCone(Fighter f, V2 dir, float range, float cosHalf)
    {
        var d0 = Norm(dir, f.Dir);
        int n = 0;
        foreach (var o in F)
        {
            if (o.Side == f.Side || !o.Standing) continue;
            var v = o.Pos - f.Pos; float L = v.Length();
            if (L <= range + o.Radius && (L < 0.5f || V2.Dot(v / L, d0) >= cosHalf)) n++;
        }
        return n;
    }

    /// <summary>Best point (a foe's position) to centre an area spell of radius r within range, and how many foes it catches.</summary>
    (V2 pt, int n) Crowd(Fighter f, float r, float range)
    {
        V2 best = f.Pos; int bn = 0;
        foreach (var o in F)
        {
            if (o.Side == f.Side || !o.Standing || o.Asleep(T) || Dist(f, o) > range) continue;
            int n = 0;
            foreach (var p in F) if (p.Side != f.Side && p.Standing && !p.Asleep(T) && V2.Distance(p.Pos, o.Pos) <= r) n++;
            if (n > bn) { bn = n; best = o.Pos; }
        }
        return (best, bn);
    }

    /// <summary>Cast a spell (cantrips at will, first level from a slot).</summary>
    public void Cast(Fighter f, SpellDef sp, Fighter t, V2? point = null)
    {
        var c = f.Char;
        if (c == null || sp == null) return;
        if (sp.Level > 0) { if (c.Slots <= 0) return; c.Slots--; }
        int dc = Rules.SpellDc(c), atk = Rules.SpellAtk(c), mod = c.Mod(Rules.CastStat(c.Cls));
        // D&D: damage cantrips grow at levels 5 and 11
        int nd = sp.Level == 0 ? sp.N * (c.Level >= 11 ? 3 : c.Level >= 5 ? 2 : 1) : sp.N;
        f.Anim = sp.Kind == SpellKind.Heal ? "Pray" : "Attack"; f.AnimLock = 0.8f;
        f.Cd = f.Period + (float)Rng.Next() * 0.4f - 0.2f;
        if (t != null) f.Dir = Norm(t.Pos - f.Pos, f.Dir);
        string slot = sp.Level > 0 ? $" (yuva {c.Slots}/{c.SlotsMax})" : "";
        switch (sp.Kind)
        {
            case SpellKind.Attack:
            {
                var tmp = new M.Combatant { Name = f.Name, Side = f.Cb.Side, Atk = atk, Dmg = new List<double> { nd, sp.S, 0 }, Attacks = 1, Hp = 1, MaxHp = 1, Uses = new M.JsObj<double>() };
                double hp0 = t.Cb.Hp;
                var r = M.Combat.Strike(Rng, tmp, t.Cb, new M.Combat.StrikeCtx { Adv = t.Asleep(T), Dis = Nearest(f, Other(f.Side)) is Fighter nf && Dist(f, nf) < 1.8f });
                int d = (int)r.D20, ac = (int)t.Cb.Ac;
                var e = new FightEvent
                {
                    Kind = "spell", A = f, B = t, Hit = r.Hit, Crit = r.Crit, Fumble = r.Fumble, Value = (int)r.Dmg,
                    Short = r.Fumble ? $"{sp.Name}: 1 · ıska" : !r.Hit ? $"{sp.Name}: {d}{Rules.Signed(atk)} → {d + atk} vs ZS {ac} · ıska"
                        : r.Crit ? $"{sp.Name}: {d} KRİTİK! · {(int)r.Dmg}" : $"{sp.Name}: {d}{Rules.Signed(atk)} → {d + atk} vs ZS {ac} · {(int)r.Dmg}",
                    Text = $"{f.Name} {sp.Name} → {t.Name}: {(r.Hit ? $"{(int)r.Dmg} hasar{(r.Crit ? " (kritik)" : "")}" : "ıska")}.",
                    Detail = $"büyü saldırısı d20 {d} + {atk} vs ZS {ac}; {nd}d{sp.S}{(r.Dice != null ? " = " + string.Join("+", r.Dice.ConvertAll(x => ((int)x).ToString())) : "")}",
                };
                Emit(e);
                if (r.Hit)
                {
                    if (sp.Slows) t.SlowUntil = T + SlowSeconds;
                    Damaged(t, f, hp0, r.Crit);
                }
                break;
            }
            case SpellKind.Save:
            {
                var targets = new List<Fighter>();
                if (sp.Radius > 0)
                {
                    var dir = t != null ? t.Pos - f.Pos : f.Dir;
                    var d0 = Norm(dir, f.Dir);
                    foreach (var o in F)
                    {
                        if (o.Side == f.Side || !o.Standing) continue;
                        var v = o.Pos - f.Pos; float L = v.Length();
                        if (L <= sp.Radius + o.Radius && (L < 0.5f || V2.Dot(v / L, d0) >= 0.55f)) targets.Add(o);
                    }
                }
                else if (t != null) targets.Add(t);
                var dice = Rng.Roll(nd, sp.S);
                int total = 0; foreach (var x in dice) total += (int)x;
                Emit(new FightEvent { Kind = "spell", A = f, B = t, Short = $"{sp.Name}!{slot}", Text = $"{f.Name} {sp.Name}: {nd}d{sp.S} = {total}, kurtarış ZD {dc}." });
                foreach (var o in targets)
                {
                    int save = (int)Rng.D20() + (o.Char != null ? o.Char.Mod(sp.Save) : o.DexMod);
                    bool ok = save >= dc;
                    int dmg = ok ? (sp.Radius > 0 ? total / 2 : 0) : total;
                    var e = new FightEvent { Kind = "spell", A = f, B = o, Hit = !ok, Value = dmg, Short = $"kurtarış {save} vs ZD {dc} · {(ok ? (dmg > 0 ? $"yarı {dmg}" : "kurtuldu") : $"{dmg}")}", Text = $"{o.Name}: kurtarış {save} vs ZD {dc}, {(dmg > 0 ? $"{dmg} hasar" : "kurtuldu")}." };
                    Emit(e);
                    if (dmg > 0) { double hp0 = o.Cb.Hp; o.Cb.Hp -= dmg; Damaged(o, f, hp0, false); }
                }
                break;
            }
            case SpellKind.Auto:
            {
                var pool = new List<Fighter>();
                if (t != null && t.Standing) pool.Add(t);
                for (int k = 0; k < sp.Missiles; k++)
                {
                    var tt = t != null && t.Standing ? t : Nearest(f, Other(f.Side));
                    if (tt == null) break;
                    var r = Rng.Roll(sp.N, sp.S);
                    int dmg = (int)r[0] + 1;
                    double hp0 = tt.Cb.Hp;
                    tt.Cb.Hp -= dmg;
                    Emit(new FightEvent { Kind = "spell", A = f, B = tt, Hit = true, Value = dmg, Short = $"{sp.Name} · {dmg}", Text = $"{f.Name} {sp.Name} → {tt.Name}: 1d4+1 = {dmg}.{slot}" });
                    Damaged(tt, f, hp0, false);
                }
                break;
            }
            case SpellKind.Heal:
            {
                var r = Rng.Roll(sp.N, sp.S);
                int heal = Math.Max(1, (int)r[0] + (sp.AddMod ? mod : 0));
                Emit(new FightEvent { Kind = "spell", A = f, B = t, Short = $"{sp.Name}{slot}", Text = $"{f.Name} {sp.Name} → {t?.Name}: 1d8 ({(int)r[0]}) {Rules.Signed(mod)} = {heal}." });
                Heal(t ?? f, f, heal, sp.Name);
                break;
            }
            case SpellKind.Sleep:
            {
                var at = point ?? t?.Pos ?? f.Pos;
                var dice = Rng.Roll(sp.N, sp.S);
                int pool = 0; foreach (var x in dice) pool += (int)x;
                var cands = new List<Fighter>();
                foreach (var o in F) if (o.Side != f.Side && o.Standing && !o.Asleep(T) && V2.Distance(o.Pos, at) <= sp.Radius) cands.Add(o);
                cands.Sort((a, b) => a.Hp.CompareTo(b.Hp));
                var slept = new List<string>();
                foreach (var o in cands)
                {
                    if (o.Hp > pool) break;
                    pool -= (int)o.Hp;
                    o.SleepUntil = T + SleepSeconds; o.Moving = false; o.Anim = "Die";
                    slept.Add(o.Name);
                    Emit(new FightEvent { Kind = "sleep", A = f, B = o, Short = "uyudu", Text = $"{o.Name} uykuya daldı." });
                }
                Emit(new FightEvent { Kind = "spell", A = f, Short = $"{sp.Name}: {slept.Count} uyudu{slot}", Text = $"{f.Name} {sp.Name}: 5d8 = {pool + 0} can değeri; {(slept.Count > 0 ? string.Join(", ", slept) + " uyudu" : "kimse uyumadı")}." });
                break;
            }
        }
    }

    // ------------------------------------------------------------------------------------------------ morale and the end
    void Morale()
    {
        if (!FoesRouted && _initFoes > 0)
        {
            int lost = 0; bool bossDown = false, hadBoss = false;
            foreach (var f in F)
            {
                if (f.Side != FSide.Foe) continue;
                if (f.IsBoss) { hadBoss = true; if (!f.Standing) bossDown = true; }
                if (!f.Standing) lost++;
            }
            if (lost / (float)_initFoes >= FoeMorale || (hadBoss && bossDown && lost < _initFoes))
            {
                FoesRouted = true;
                int n = 0;
                foreach (var f in F) if (f.Side == FSide.Foe && f.Standing && !f.Asleep(T)) { StartFlee(f); n++; }
                if (n > 0) Emit(new FightEvent { Kind = "rout", Short = "bozgun!", Text = bossDown && hadBoss ? "Şefleri düşünce goblinler bozguna uğradı ve kaçıyor!" : "Goblinler bozguna uğradı ve kaçıyor!" });
            }
        }
        if (_initParty > 0)
        {
            int lost = 0; foreach (var f in F) if (f.Side == FSide.Party && !f.Standing) lost++;
            if (lost / (float)_initParty >= CompanionMorale)
                foreach (var f in F)
                    if (f.Side == FSide.Party && f.Kind == "companion" && f.Standing && !f.Fleeing && f.Hp < f.MaxHp * 0.5f)
                    {
                        StartFlee(f);
                        Emit(new FightEvent { Kind = "flee", A = f, Short = "kaçıyor!", Text = $"{f.Name} canını kurtarmak için kaçıyor!" });
                    }
        }
    }

    void StartFlee(Fighter f)
    {
        f.Fleeing = true; f.Target = null; f.Order = null;
        var away = V2.Zero; int n = 0;
        foreach (var o in F) if (o.Side != f.Side && o.Standing) { away += f.Pos - o.Pos; n++; }
        var dir = n > 0 ? Norm(away, f.Dir) : f.Dir;
        f.FleeTo = f.Side == FSide.Foe && FoeHome != default ? FoeHome + dir * 8f : f.Pos + dir * 60f;
    }

    void FleeStep(Fighter f, float dt)
    {
        var d = f.FleeTo - f.Pos; float L = d.Length();
        if (L > 0.2f) { f.Pos += d / L * MathF.Min(L, f.Speed * 1.05f * dt); f.Dir = d / L; f.Moving = true; f.Anim = "Run"; }
        var near = Nearest(f, Other(f.Side));
        if (L <= 0.5f || near == null || Dist(f, near) > FleeDistance)
        {
            f.Fled = true; f.Moving = false;
            Emit(new FightEvent { Kind = "flee", A = f, Short = "kaçtı", Text = $"{f.Name} kaçtı." });
        }
    }

    void CheckEnd(float dt)
    {
        bool partyUp = false, foeUp = false, partyAway = false;
        foreach (var f in F)
        {
            if (f.Side == FSide.Party && f.Standing && !f.Fleeing) partyUp = true;
            if (f.Side == FSide.Party && !f.Dead && !f.Down && (f.Fleeing || f.Fled)) partyAway = true;
            if (f.Side == FSide.Foe && f.Standing && !f.Fleeing) foeUp = true;
        }
        if (partyUp && foeUp)
        {
            // Tur 1 C: the party got clear — everyone still on their feet is far from every foe still fighting
            bool apart = true;
            foreach (var a in F)
            {
                if (a.Side != FSide.Party || !a.Standing) continue;
                foreach (var b in F) if (b.Side == FSide.Foe && b.Standing && !b.Fleeing && Dist(a, b) < EscapeDistance) { apart = false; break; }
                if (!apart) break;
            }
            _apartT = apart ? _apartT + dt : 0f;
            if (_apartT < 1.5f) return;
            Over = true; Winner = null; Escaped = true;
            foreach (var f in F) if (f.Side == FSide.Party && f.Standing) { f.Fled = true; f.Fleeing = false; f.Moving = false; }
            foreach (var f in F) if (f.Fleeing && !f.Fled) { f.Fled = true; f.Moving = false; }
            Emit(new FightEvent { Kind = "end", Text = "Savaş bitti: ekip kaçıp kurtuldu." });
            return;
        }
        // routed foes still on their way out do not keep the fight going; nor do fleeing companions
        Over = true;
        if (!foeUp && (partyUp || partyAway)) Winner = FSide.Party;
        else if (partyAway) { Winner = null; Escaped = true; }
        else Winner = FSide.Foe;
        foreach (var f in F) if (f.Fleeing && !f.Fled) { f.Fled = true; f.Moving = false; }
        Emit(new FightEvent { Kind = "end", Text = Winner == FSide.Party ? "Savaş bitti: kazandınız." : Escaped ? "Savaş bitti: ekip kaçıp kurtuldu." : "Savaş bitti: ekip yere serildi." });
    }

    /// <summary>D: the fight is lost and the fallen lie where they are while the goblins go through their purses: those still dying keep
    /// rolling death saves (each failure may leave a lasting wound) until they steady — a third failure is a narrow escape here, not
    /// death (the goblins finishing them off is the director's call).</summary>
    public void SettleFallen()
    {
        foreach (var f in F)
        {
            if (f.Side != FSide.Party || !f.Down || f.Dead || f.Stable) continue;
            for (int k = 0; k < 12 && !f.Stable; k++)
            {
                double d = Rng.D20();
                if (d == 20) { f.DeathOk = 3; f.Stable = true; Emit(new FightEvent { Kind = "stable", A = f, Text = $"{f.Name} ölüm zarı 20: kendini toparladı." }); break; }
                if (d >= 10) f.DeathOk++; else { f.DeathFail += d == 1 ? 2 : 1; MaybeWound(f, d == 1 ? "doğal 1'lik ölüm zarı" : "kötü giden ölüm zarı"); }
                if (f.DeathOk >= 3) { f.Stable = true; Emit(new FightEvent { Kind = "stable", A = f, Text = $"{f.Name} dengelendi." }); }
                else if (f.DeathFail >= 3) { f.Stable = true; Emit(new FightEvent { Kind = "stable", A = f, Text = $"{f.Name} ölümün eşiğinden kıl payı döndü." }); }
            }
        }
    }

    /// <summary>Copy fight results back to the characters (HP, down/dead, death saves) — call when the fight is over.</summary>
    public void WriteBack(int day = -1, string where = null)
    {
        foreach (var f in F)
        {
            if (f.Char == null) continue;
            var c = f.Char;
            c.Hp = Math.Max(0, (int)Math.Round(f.Cb.Hp));
            c.Dead = f.Dead; c.Down = f.Down && !f.Dead; c.Stable = f.Stable; c.DeathOk = f.DeathOk; c.DeathFail = f.DeathFail;
            if (c.Down && Winner == FSide.Party) c.Stable = true;   // friends tend the fallen after a won fight
            if (f.NewWound != null && !c.Dead) c.AddWound(f.NewWound, day, where);
        }
    }

    /// <summary>XP from the beaten foes (D&amp;D: goblin 50, goblin boss 200), shared by the party members still alive.</summary>
    public int XpEach()
    {
        int xp = 0, n = 0;
        foreach (var f in F)
        {
            if (f.Side == FSide.Foe && (f.Dead || f.Fled || (Winner == FSide.Party && !f.Standing))) xp += f.IsBoss ? 200 : 50;
            if (f.Side == FSide.Party && !f.Dead && !f.Fled) n++;
        }
        return n > 0 ? xp / n : 0;
    }
}
