using System;
using System.Linq;
using Godot;
using FD.Game;
using FD.Rpg;
using FD.Sim.Life;
using FD.World;
using V2 = System.Numerics.Vector2;
using M = FD.Macro;

namespace FD.Dev;

/// <summary>
/// Faz 2 E (--partytest): the inn's adventurers come from the sim (free heroes waiting there, mercenaries to make two hireable);
/// hiring one makes a companion who follows the leader (and a sim hero goes "party" in the macro world); payday pays a week from
/// the purses or, unpaid, they leave and go back to the inn; opposed alignments part at camp; Tab moves control to the companion.
/// PASS/FAIL, exit code 0/1.
/// </summary>
public partial class PartyTest : Node
{
    Region _r;
    int _step, _wait;
    float _secs;
    bool _ok = true;
    Person _guest;
    Character _hired;
    Vector3 _target;

    public static void Run(Node host, Region r) { var t = new PartyTest { Name = "PartyTest", _r = r }; host.AddChild(t); }

    void Check(string name, bool pass, string info) { _ok &= pass; GD.Print($"[PartyTest] {name}: {info} {(pass ? "PASS" : "FAIL")}"); }

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        Engine.TimeScale = 2.0;
    }

    public override void _Process(double delta)
    {
        if (_wait > 0) { _wait--; return; }
        if (_secs > 0) { _secs -= (float)delta; return; }
        var s = _r.Session; var life = _r.Life;
        switch (_step++)
        {
            case 0:
            {
                var guests = life.People.Where(p => p.Guest).ToList();
                var inn = M.Local.InnHeroes(s.Macro);
                int hireable = guests.Count(PartyManager.ForHire);
                Check("handaki maceracılar", guests.Count > 0 && hireable >= 2 && guests.Where(g => g.HeroId >= 0).All(g => inn.Any(h => h.Id == g.HeroId)),
                    $"simde handa bekleyen {inn.Count} kahraman; handa {guests.Count} maceracı: {string.Join(", ", guests.Select(g => $"{g.FullName} ({g.Race} {g.Cls} Sv{g.Level}, {g.Align}{(g.HeroId >= 0 ? ", simden" : ", paralı")}{(PartyManager.ForHire(g) ? ", kiralık" : "")})"))}");
                _guest = guests.Where(PartyManager.ForHire).OrderByDescending(g => g.HeroId).First();
                // stand next to them and hire
                var gp = _guest.Pos;
                _r.Player.Teleport(new Vector2(gp.X + 1.5f, gp.Y + 1.5f), 0, _r.Heightfield);
                _hired = _r.Party.Hire(_guest);
                var h = _hired.HeroId is int hid ? s.Macro.Hero(hid) : null;
                Check("kiralama", s.Party.Contains(_hired) && _r.Companions.Any(c => c.Char == _hired) && _guest.Hired && !_guest.Present
                    && (h == null || (h.State == "party" && s.Link.Party.Contains(h.Id))) && _hired.WageSilver > 0,
                    $"{_hired.Name} ({Rules.ClassName(_hired.Cls)} Sv{_hired.Level}, can {_hired.Hp}, ZS {_hired.Ac}, {_hired.Weapon}) — haftalık {_hired.WageSilver} gümüş, ödeme günü {_hired.PaidUntil}; sim: {(h != null ? $"{h.Name} {h.State}" : "paralı asker (kayıtsız)")}");
                // walk away: the companion follows
                _target = _r.Player.GlobalPosition + new Vector3(25, 0, 10);
                _wait = 5;
                break;
            }
            case 1:
            {
                var pl = _r.Player;
                var p = pl.GlobalPosition;
                var to = _target - p; to.Y = 0;
                if (to.Length() > 0.5f)
                {
                    var step = to.Normalized() * Math.Min(to.Length(), 4f * (float)GetProcessDeltaTime());
                    pl.Teleport(new Vector2(p.X + step.X, p.Z + step.Z), MathF.Atan2(to.X, to.Z), _r.Heightfield);
                    _step--;
                    break;
                }
                _secs = 6f;
                break;
            }
            case 2:
            {
                var comp = _r.Companions.First(c => c.Char == _hired);
                float d = comp.GlobalPosition.DistanceTo(_r.Player.GlobalPosition);
                Check("izliyor", d < 4.5f, $"{_hired.Name} lidere {d:F1} m");
                // Tab
                _r.Party.SwitchControl(1);
                Check("Tab", _r.Player.Character == _hired && comp.Char == s.Player && s.Controlled == _hired, $"kontrol {_r.Player.Character.Name}, yoldaş bedeninde {comp.Char.Name}");
                _r.Party.SwitchControl(1);
                Check("Tab geri", _r.Player.Character == s.Player, $"kontrol {_r.Player.Character.Name}");
                // payday with money
                foreach (var c in s.Party) c.Inv.Silver = 0;
                s.Player.Inv.Silver = _hired.WageSilver + 7;
                int due = _hired.PaidUntil;
                bool paid = _r.Party.Payday(_hired);
                Check("maaş ödendi", paid && s.Player.Inv.Silver == 7 && _hired.PaidUntil == due + PartyManager.WeekDays, $"kese {_hired.WageSilver + 7} → {s.Player.Inv.Silver} gümüş, sıradaki ödeme günü {_hired.PaidUntil}");
                // payday without money (through the day tick)
                s.Player.Inv.Silver = 0;
                GameClock.SetTotalHours(_hired.PaidUntil * 24.0 + 1.0);
                _wait = 3;
                break;
            }
            case 3:
            {
                var h = _hired.HeroId is int hid ? s.Macro.Hero(hid) : null;
                Check("ödenmeyince ayrıldı", !s.Party.Contains(_hired) && !_r.Companions.Any(c => c.Char == _hired) && !_guest.Hired
                    && (h == null || (h.State != "party" && !s.Link.Party.Contains(h.Id))),
                    $"ekip {s.Party.Count} kişi; {_guest.FullName} ekipten çıktı ({(_guest.Present ? "handa görünür" : "handa değil")}); sim: {(h != null ? $"{h.State} (hana döndü, sonra simin kendi işine)" : "paralı")}");
                // alignment at camp
                var g2 = life.People.Where(p => p.Guest && PartyManager.ForHire(p) && !p.Hired).First();
                g2.Align = "evil"; s.Player.Align = "good";
                var c2 = _r.Party.Hire(g2);
                c2.Align = "evil";
                _r.Party.OnRest();
                Check("hizalama kampta ayırır", !s.Party.Contains(c2), $"{c2.Name} (kötü) iyi lider {s.Player.Name} ile ilk konaklamada ayrıldı");
                GD.Print(_ok ? "PARTYTEST PASS" : "PARTYTEST FAIL");
                Engine.TimeScale = 1;
                GetTree().Quit(_ok ? 0 : 1);
                break;
            }
        }
    }
}
