using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using FD.Combat;
using FD.Game;
using FD.Sim.Life;
using FD.World;
using V2 = System.Numerics.Vector2;
using M = FD.Macro;

namespace FD.Dev;

/// <summary>
/// Faz 2 (--camptest[=N], with --party=… for companions): the fight in the region, end to end and headless. The player walks up
/// the camp trail at midday until the goblins spot, chase and catch them; the fight runs by itself (no pause, the classes' own
/// judgement) and ends; then: the goblins who died lie dead, the routed ran off and hid, nobody is left "in the fight", the player
/// is back in third person, XP was given, the macro camp lost exactly the goblins killed here — and when the camp broke, it is
/// cleared in the macro world too (quest closed, reward waiting, settlers 10–20 days later, checked by stepping the macro days).
/// N rounds: the player (healed) goes in again until the camp falls or N fights were fought. PASS/FAIL, exit code 0/1.
/// </summary>
public static class CampTest
{
    public static void Run(Node host, Region region, int rounds)
    {
        var r = new CampTestRunner { Name = "CampTest", Rounds = Math.Max(1, rounds), Region = region };
        host.AddChild(r);
    }
}

public partial class CampTestRunner : Node
{
    public int Rounds;
    public Region Region;
    int _round, _phase, _frames;
    float _t;
    bool _ok = true;
    double _camp0;
    int _dead0, _xp0;
    FightOutcome _last;
    readonly List<string> _out = new();

    void Check(string name, bool pass, string info) { _ok &= pass; _out.Add($"[CampTest] {name}: {info} {(pass ? "PASS" : "FAIL")}"); GD.Print(_out[^1]); }

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        Engine.TimeScale = 3.0;
        GameClock.TimeScale = 48f;
        GameClock.SetHour(13.5f);
        Region.Combat.Unattended = true;
        Region.Combat.Ended += o => _last = o;
        var s = Region.Session;
        GD.Print($"[CampTest] {s.Describe()}");
        GD.Print($"[CampTest] ekip: {string.Join(", ", s.Party.Select(c => $"{c.Name} ({c.Race} {c.Cls} Sv{c.Level}, can {c.Hp}, ZS {c.Ac})"))}");
        Begin();
    }

    void Begin()
    {
        _round++;
        _phase = 0; _t = 0; _last = null;
        var s = Region.Session;
        foreach (var c in s.Party) { if (!c.Dead) { c.Rest(); } }
        _camp0 = s.Camp?.Count ?? 0;
        _dead0 = Region.Life.People.Count(p => p.Role == Role.Goblin && p.Dead);
        _xp0 = (int)s.Player.Xp;
        // the foot of the camp trail, facing up it
        var start = RegionSpec.CampSpurControl[0] + new Vector2(-14, 12);
        Region.Player.Teleport(start, MathF.Atan2(RegionSpec.GoblinCamp.X - start.X, RegionSpec.GoblinCamp.Y - start.Y), Region.Heightfield);
        foreach (var c in Region.Companions) c.SnapToLeader();
        GD.Print($"[CampTest] tur {_round}: kamp {s.Camp?.Name} {_camp0:F0} goblin{(s.Camp?.Boss == true ? " + şef" : "")}, bölgede ayakta {Region.Life.People.Count(p => p.Role == Role.Goblin && !p.Dead && p.Present)}");
    }

    public override void _Process(double delta)
    {
        float dt = (float)delta;
        _t += dt; _frames++;
        var cd = Region.Combat;
        if (_frames % 600 == 0 && _phase < 3)
        {
            var pp = new V2(Region.Player.GlobalPosition.X, Region.Player.GlobalPosition.Z);
            var g = Region.Life.People.Where(p => p.Role == Role.Goblin && !p.Dead).OrderBy(p => V2.Distance(p.Pos, pp)).FirstOrDefault();
            GD.Print($"[CampTest] … evre {_phase} t {_t:F0} savaş {(cd.Active ? $"{cd.Fight.T:F0} sn{(cd.Paused ? " duraklı" : "")}{(cd.Summary ? " özet" : "")}" : "yok")} ağaç {(GetTree().Paused ? "durdu" : "akıyor")} oyuncu ({pp.X:F0},{pp.Y:F0}) en yakın goblin {g?.Name} {(g != null ? V2.Distance(g.Pos, pp) : 0):F0} m {g?.Act?.Label} {g?.Motion} present {g?.Present} inFight {g?.InFight}");
        }
        switch (_phase)
        {
            case 0: // walk up the trail until a goblin catches us
            {
                if (cd.Active) { _phase = 1; _t = 0; break; }
                var pl = Region.Player;
                var p = new Vector2(pl.GlobalPosition.X, pl.GlobalPosition.Z);
                var to = RegionSpec.GoblinCamp - p;
                if (to.Length() > 3f)
                {
                    var step = to.Normalized() * MathF.Min(to.Length(), 3.2f * dt);
                    pl.Teleport(p + step, MathF.Atan2(to.X, to.Y), Region.Heightfield);
                }
                if (_t > 90f) { Check($"tur {_round}: savaş başladı", false, "goblinler 90 sn içinde saldırmadı"); Finish(); }
                break;
            }
            case 1: // the fight runs; Unattended closes the summary
                if (_last != null) { _phase = 2; _t = 0; break; }
                if (cd.Active && cd.Fight.T > 240f) { Check($"tur {_round}: savaş bitti", false, $"4 dakikada bitmedi ({cd.Fight.Log.Count} olay)"); Finish(); }
                break;
            case 2: // let the routed run off
                if (cd.RunningOff == 0 || _t > 40f) { _phase = 3; Verify(); }
                break;
        }
    }

    void Verify()
    {
        var s = Region.Session; var life = Region.Life; var o = _last;
        var cp = s.Camp;
        int deadNow = life.People.Count(p => p.Role == Role.Goblin && p.Dead);
        int inFight = life.People.Count(p => p.InFight);
        Check($"tur {_round}: savaş", true, $"{(o.Won ? "ZAFER" : "YENİLGİ")}: öldürülen {o.Killed}, kaçan {o.Fled}, şef {(o.BossKilled ? "öldü" : "yaşıyor")}, ekipte yerde {o.PartyDown}, ölü {o.PartyDead}, TP {o.XpEach}; {string.Join(" / ", o.Lines)}");
        Check($"tur {_round}: ölüler yerde", deadNow - _dead0 == o.Killed, $"yeni goblin cesedi {deadNow - _dead0} = öldürülen {o.Killed}");
        Check($"tur {_round}: kimse savaşta kalmadı", inFight == 0 && !Region.Combat.Active && !Region.Player.Scripted && Region.Player.Camera.Current,
            $"savaşta kalan {inFight}, oyuncu {(Region.Player.Scripted ? "betikte" : "serbest")}, kamera {(Region.Player.Camera.Current ? "üçüncü şahıs" : "başka")}");
        int hidden = life.People.Count(p => p.Role == Role.Goblin && !p.Dead && (!p.Present || p.Motion == Motion.Inside) && p.Act?.Label is "Ormanda saklanıyor" or null);
        Check($"tur {_round}: kaçanlar gitti", o.Fled == 0 || hidden >= o.Fled, $"kaçan {o.Fled}, saklanan/giden {hidden}");
        int xp = (int)s.Player.Xp - _xp0;
        Check($"tur {_round}: TP", s.Player.Dead || xp == o.XpEach || (o.XpEach == 0 && xp == 0), $"oyuncu +{xp} TP (seviye {s.Player.Level})");
        double expect = o.CampCleared ? 0 : Math.Max(0, _camp0 - (o.Killed - (o.BossKilled ? 1 : 0)));
        Check($"tur {_round}: sim kampı", cp != null && Math.Abs(cp.Count - expect) < 0.01 && cp.Alive == !o.CampCleared && (!o.BossKilled || !cp.Boss),
            $"{cp?.Name}: {_camp0:F0} → {cp?.Count:F0} goblin (beklenen {expect:F0}), {(cp?.Alive == true ? "ayakta" : "temizlendi")}, şef {(cp?.Boss == true ? "var" : "yok")}");
        if (o.CampCleared)
        {
            var link = s.Link;
            bool questOpen = M.Local.CampQuest(s.Macro) != null;
            Check("kamp temizlendi: sim", !cp.Alive && cp.ClearedDay != null && !questOpen && link.SettlersDay != null,
                $"temizlendiği gün {cp.ClearedDay}, açık ilan {(questOpen ? "var" : "yok")}, bekleyen ödül {link.Reward:F0} altın, ganimet {link.CampLoot:F0}, yerleşimciler gün {link.SettlersDay}");
            var ev = s.Macro.W.Events.LastOrDefault(e => e.Kind == "lair" && e.Text.Contains(cp.Name));
            Check("kamp temizlendi: tarih", ev != null, ev?.Text ?? "olay yok");
            // pioneers 10–20 days later: the fertile-valley hub starts (rumour → rush), or settlers join the village
            var v = s.VillageSettlement;
            double pop0 = s.Macro.Pop(v);
            double sd = link.SettlersDay ?? 0;
            int steps = 0;
            while (s.Macro.W.Day < sd + 3 && steps < 30) { s.Macro.Step(); M.Local.DayTick(s.Macro); steps++; }
            double pop1 = s.Macro.Pop(v);
            var hub = s.Macro.W.Hubs.FirstOrDefault(h => h.Origin == cp.Id && h.Kind == "valley");
            var sev = s.Macro.W.Events.LastOrDefault(e => e.Kind == "hub" && e.Day >= sd && (e.Text.Contains("öncüler") || e.Text.Contains("vadi")));
            double after = sd - (cp.ClearedDay ?? 0);
            Check("öncüler", link.SettlersDay == null && (hub != null || sev != null) && after >= M.Local.SETTLERS_MIN && after <= M.Local.SETTLERS_MAX,
                $"{after:F0} gün sonra: {(hub != null ? $"{hub.Name} ({hub.Phase}, {M.Hubs.KindName(hub.Kind)})" : "vadi yok")}; {sev?.Text ?? "olay yok"} (köy nüfusu {pop0:F0} → {pop1:F0})");
            Finish();
            return;
        }
        if (_round >= Rounds || s.Player.Dead) { Finish(); return; }
        Begin();
    }

    void Finish()
    {
        Engine.TimeScale = 1.0;
        GD.Print(_ok ? "CAMPTEST PASS" : "CAMPTEST FAIL");
        GetTree().Quit(_ok ? 0 : 1);
        SetProcess(false);
    }
}
