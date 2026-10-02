using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using FD.Actors;
using FD.Combat;
using FD.Game;
using FD.Rpg;
using FD.Sim.Life;
using FD.UI;
using FD.World;
using V2 = System.Numerics.Vector2;
using M = FD.Macro;

namespace FD.Dev;

/// <summary>
/// Tur 1 (--t1test, with --party=fighter,wizard,cleric): the brief's controls, headless, through the same calls the mouse and keys
/// make. One camera from the shoulder to the high view with no jump; pause stops the world and takes orders; the speed keys and the
/// fall back to 1× at a threat; right click on the ground walks there, on a villager walks up and opens the card, on a goblin
/// attacks (walking up first when far); the cursor says sword, mouth, hand, foot; box and 1–4 / Tab selection hands the body over;
/// Shift+1…5 stances; an aggressive member starts a fight by themselves; a passive one never strikes, a holding one never steps;
/// running off on a right click gets the party clear; the smith forges and stops without iron; people point at the board.
/// PASS/FAIL, exit code 0/1.
/// </summary>
public partial class Tur1Test : Node
{
    Region _r;
    bool _ok = true;
    int _step;
    float _wait, _t;
    Vector3 _goal;
    Person _villager, _gob;
    double _clock0;
    readonly Dictionary<Character, Vector3> _pos0 = new();
    int _passiveSwings;
    Character _passive, _holder;

    public static void Run(Node host, Region r) { var t = new Tur1Test { Name = "Tur1Test", _r = r }; host.AddChild(t); }
    void Check(string name, bool pass, string info) { _ok &= pass; GD.Print($"[Tur1Test] {name}: {info} {(pass ? "PASS" : "FAIL")}"); }

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        _r.Combat.Unattended = true;
        GetTree().Root.Size = new Vector2I(1600, 900);   // headless: a real screen for picking
    }

    GameCamera Cam => GameCamera.Instance;
    Commander Cmd => Commander.Instance;
    V2 P2 => new(_r.Player.GlobalPosition.X, _r.Player.GlobalPosition.Z);

    void Key(Key k, bool shift = false)
    {
        Input.ParseInputEvent(new InputEventKey { PhysicalKeycode = k, Keycode = k, Pressed = true, ShiftPressed = shift });
        Input.FlushBufferedEvents();
        Input.ParseInputEvent(new InputEventKey { PhysicalKeycode = k, Keycode = k, Pressed = false, ShiftPressed = shift });
        Input.FlushBufferedEvents();
    }

    /// <summary>look at a point from the given distance (the camera stops following)</summary>
    void Look(Vector3 at, float dist)
    {
        Cam.Follow = false;
        Cam.Focus = at;
        Cam.SetZoom(dist, true);
    }

    Vector2 ScreenOf(Vector3 w) => Cam.Screen(w) ?? new Vector2(-1, -1);

    Person NearGoblin() => _r.Life.People.Where(p => p.Role == Role.Goblin && !p.Dead && p.Present && !p.InFight).OrderBy(p => V2.Distance(p.Pos, P2)).FirstOrDefault();

    /// <summary>bring a goblin to stand at a point (dev), awake and outside — a fallen one gets up again (the camp is small)</summary>
    Person Bring(V2 at, Person not = null)
    {
        var g = _r.Life.People.Where(p => p.Role == Role.Goblin && !p.InFight && !p.IsBoss && p != not).OrderBy(p => p.Dead ? 1 : 0).ThenBy(p => p.Id).FirstOrDefault();
        if (g == null) return null;
        g.Dead = false; g.Down = false; g.Present = true; g.FightAnim = null; g.FightTool = null;
        g.Pos = at; g.Motion = Motion.Doing; g.Path.Clear();
        g.Act = new Activity { Kind = ActKind.Rest, Place = -1, Target = at, Until = _r.Life.Now + 600, Label = "Bekliyor", Reason = "test" };
        g.NextDecision = _r.Life.Now + 600;
        return g;
    }

    public override void _Process(double delta)
    {
        float dt = (float)delta;
        _t += dt;
        if (_wait > 0) { _wait -= dt / MathF.Max(0.05f, (float)Engine.TimeScale); return; }
        var s = _r.Session; var cd = _r.Combat;
        switch (_step)
        {
            case 0:   // the one camera: shoulder → high view with no jump
            {
                var cam = Cam;
                bool cur = cam != null && cam.Current;
                Check("tek kamera", cur && _r.Player.Camera.Current == false, $"GameCamera {(cur ? "geçerli" : "YOK")}, oyuncunun eski kamerası kapalı");
                float prevPitch = 0; float maxJump = 0; bool mono = true; Vector3 prev = default;
                cam.Follow = true;
                for (int i = 0; i <= 40; i++)
                {
                    float d = GameCamera.MinDist * MathF.Pow(GameCamera.MaxDist / GameCamera.MinDist, i / 40f);
                    cam.SetZoom(d, true);
                    cam._Process(0.016);
                    float pitch = cam.GlobalBasis.Z.Y;   // up-component of the back vector: grows as it looks down
                    if (i > 0) { mono &= pitch >= prevPitch - 1e-3f; maxJump = MathF.Max(maxJump, cam.GlobalPosition.DistanceTo(prev) / MathF.Max(1f, d * 0.1f)); }
                    prevPitch = pitch; prev = cam.GlobalPosition;
                }
                float down = Mathf.RadToDeg(MathF.Asin(Math.Clamp(cam.GlobalBasis.Z.Y, -1f, 1f)));
                Check("omuzdan yükseğe kesintisiz", mono && maxJump < 2.5f && down > 55f, $"41 adım: eğim tekdüze artıyor, en büyük sıçrama {maxJump:F2} (uzaklığın %10'u birimi), en uzakta {down:F0}° aşağı bakıyor");
                cam.SetZoom(6f, true); cam.Follow = true; cam.SnapNow();
                _step++; _wait = 0.3f;
                break;
            }
            case 1:   // pause stops the world and takes orders
            {
                Cmd.SetPaused(true);
                _clock0 = GameClock.TotalHours;
                var ahead = _r.Player.GlobalPosition + new Vector3(MathF.Sin(_r.Player.Facing), 0, MathF.Cos(_r.Player.Facing)) * 9f;
                ahead.Y = _r.Heightfield.Height(ahead.X, ahead.Z);
                _goal = ahead;
                Look(_r.Player.GlobalPosition, 14f); Cam._Process(0.016);
                Cmd.RightClickAt(ScreenOf(ahead));
                _step++; _wait = 1.0f;
                break;
            }
            case 2:
            {
                bool still = Math.Abs(GameClock.TotalHours - _clock0) < 1e-6;
                Check("duraklat: dünya durur, emir alınır", GetTree().Paused && still && _r.Player.Going, $"saat {(still ? "durdu" : "AKTI")}, oyuncu {( _r.Player.Going ? "yola çıkmayı bekliyor" : "EMİR ALMADI")}");
                Cmd.SetPaused(false);
                _step++; _wait = 4.5f;
                break;
            }
            case 3:
            {
                float d = new Vector2(_r.Player.GlobalPosition.X - _goal.X, _r.Player.GlobalPosition.Z - _goal.Z).Length();
                Check("sağ tık yer: oraya yürür", d < 1.3f && GameClock.TotalHours > _clock0, $"tıklanan yere {d:F1} m");
                // speed keys
                Key(Godot.Key.KpAdd); Key(Godot.Key.KpAdd);
                bool three = Cmd.Speed == 3 && Math.Abs(Engine.TimeScale - 3) < 1e-6;
                Key(Godot.Key.KpSubtract);
                Check("hız tuşları", three && Cmd.Speed == 2, $"+ + → {(three ? "3×" : "?")}, − → {Cmd.Speed}×");
                Key(Godot.Key.Space); bool p = GetTree().Paused; Key(Godot.Key.Space);
                Check("Boşluk her yerde", p && !GetTree().Paused, "Boşluk duraklattı, Boşluk sürdürdü");
                _step++;
                break;
            }
            case 4:   // talk: right click on a villager — walks up, the card opens
            {
                _villager = _r.Life.People.Where(p => p.Visible && !p.Dead && p.Role is not (Role.Goblin or Role.Child) && !p.Guest && p.Motion != Motion.Walking)
                    .OrderBy(p => V2.Distance(p.Pos, P2)).FirstOrDefault();
                if (_villager == null) { Check("konuşma", false, "yakında köylü yok"); _step = 6; break; }
                var a = _r.LifeWorld.ActorOf(_villager);
                Look(a.GlobalPosition, 10f); Cam._Process(0.016);
                var sp = ScreenOf(a.GlobalPosition + Vector3.Up * 1.0f);
                Check("imleç: ağız (konuş)", Cmd.CursorAt(sp) == CursorKit.Kind.Mouth, $"{_villager.FullName} üstünde imleç {Cmd.CursorAt(sp)}");
                Cmd.RightClickAt(sp);
                Cam.Follow = true;
                _step++; _wait = 0.5f; _t = 0;
                break;
            }
            case 5:
            {
                if (_r.Hud.CardPerson == _villager)
                {
                    Check("sağ tık kişi: yanına gider, konuşur", V2.Distance(_villager.Pos, P2) < 5.5f, $"{_villager.Name} ile {V2.Distance(_villager.Pos, P2):F1} m, kart açık, {_t:F1} sn");
                    _r.Hud.CloseCardPublic();
                    _step++;
                }
                else if (_t > 40f) { Check("sağ tık kişi", false, $"40 sn'de kart açılmadı (uzaklık {V2.Distance(_villager.Pos, P2):F1} m, yürüyor {_r.Player.Going})"); _step++; }
                break;
            }
            case 6:   // cursors: hand over a herb, foot over open ground; left click on a villager shows the card without walking
            {
                var herb = _r.Gathering.ClickTargets().FirstOrDefault(t => t.Label.Contains("ot"));
                if (herb == null)
                {
                    // look near the first patch so it is offered
                    var hp = _r.Gathering.HerbPositions.FirstOrDefault();
                    Look(new Vector3(hp.X, _r.Heightfield.Height(hp.X, hp.Y), hp.Y), 9f); Cam._Process(0.016);
                    herb = _r.Gathering.ClickTargets().FirstOrDefault(t => t.Label.Contains("ot"));
                }
                if (herb != null)
                {
                    Look(herb.Pos, 9f); Cam._Process(0.016);
                    var c = Cmd.CursorAt(ScreenOf(herb.Pos + Vector3.Up * 0.25f));
                    Check("imleç: el (topla)", c == CursorKit.Kind.Hand, $"ot öbeğinde imleç {c}");
                }
                // an open spot near the player: nobody within 4 m, no wall, no herb or door
                var open = _r.Player.GlobalPosition + new Vector3(4f, 0, 4f);
                for (int k = 0; k < 24; k++)
                {
                    float ang = k * MathF.Tau / 24f, rad = 5f + (k % 3) * 2f;
                    var c = _r.Player.GlobalPosition + new Vector3(MathF.Cos(ang) * rad, 0, MathF.Sin(ang) * rad);
                    var c2 = new V2(c.X, c.Z);
                    if (_r.Life.People.Any(p => p.Visible && !p.Dead && V2.Distance(p.Pos, c2) < 7f)) continue;
                    if (V2.Distance(new V2(_r.Player.GlobalPosition.X, _r.Player.GlobalPosition.Z), c2) < 4f || _r.Companions.Any(cp => new Vector2(cp.GlobalPosition.X - c.X, cp.GlobalPosition.Z - c.Z).Length() < 4f)) continue;
                    if (_r.Life.ObstacleAt(c2, -1.5f) != null) continue;
                    if (_r.Gathering.ClickTargets().Any(t => new Vector2(t.Pos.X - c.X, t.Pos.Z - c.Z).Length() < 4f)) continue;
                    if (_r.Life.Places.Any(pl => V2.Distance(pl.Door, c2) < 4f)) continue;
                    open = c; break;
                }
                Look(new Vector3(open.X, _r.Heightfield.Height(open.X, open.Z), open.Z), 12f); Cam._Process(0.016);
                var osp = ScreenOf(new Vector3(open.X, _r.Heightfield.Height(open.X, open.Z), open.Z));
                var gc = Cmd.CursorAt(osp);
                GD.Print($"[Tur1Test] açık arazi ekranda {osp}, zemin {Cam.Ground(osp)}");
                Check("imleç: ayak (git)", gc == CursorKit.Kind.Foot, $"açık arazide imleç {gc}");
                Cam.Follow = true;
                _step++;
                break;
            }
            case 7:   // selection: box, 1–4, Tab; Shift+N stances
            {
                var members = Cmd.Members();
                foreach (var comp in _r.Companions) comp.SnapToLeader();
                Look(_r.Player.GlobalPosition, 16f); Cam._Process(0.016);
                GD.Print($"[Tur1Test] ekran {GetViewport().GetVisibleRect().Size}, oyuncu {Cam.Screen(_r.Player.GlobalPosition + Vector3.Up * 0.9f)}, yoldaşlar {string.Join(" ", _r.Companions.Select(c => Cam.Screen(c.GlobalPosition + Vector3.Up * 0.9f)))}");
                // a box around where they stand on the screen (as a drag would)
                var pts = new List<Vector2>();
                foreach (var n in new Node3D[] { _r.Player }.Concat(_r.Companions)) if (Cam.Screen(n.GlobalPosition + Vector3.Up * 0.9f) is Vector2 sp) pts.Add(sp);
                var box = pts.Count > 0 ? new Rect2(pts[0], Vector2.Zero) : new Rect2(0, 0, 1600, 900);
                foreach (var p in pts) box = box.Expand(p);
                Cmd.BoxAt(box.Grow(30f));
                int boxed = Cmd.Sel.Count;
                var me = _r.Player.Character;
                Key(Godot.Key.Key2);
                var second = members.Count > 1 ? members[1] : null;
                bool swapped = second != null && Cmd.Sel.Count == 1 && Cmd.Sel[0] == second && _r.Player.Character == second;
                Key(Godot.Key.Tab);
                var third = Cmd.Sel.FirstOrDefault();
                Key(Godot.Key.Key1);
                bool back = _r.Player.Character == members[0];
                Check("seçim: kutu, 1–4, Tab", boxed == members.Count && swapped && third != second && back,
                    $"kutu {boxed}/{members.Count}; 2 → {second?.Name} ({(swapped ? "beden ona geçti" : "GEÇMEDİ")}); Tab → {third?.Name}; 1 → {_r.Player.Character.Name}");
                Key(Godot.Key.Key1, shift: true);
                bool agg = me.Stance == Stances.Aggressive;
                GD.Print($"[Tur1Test] Shift+1 sonrası {me.Name}: {me.Stance}; seçili {string.Join(",", Cmd.Sel.Select(c => c.Name))}");
                Key(Godot.Key.Key2, shift: true);
                Check("duruş kısayolu", agg && me.Stance == Stances.Defend, $"Shift+1 → {Stances.Name(Stances.Aggressive)}, Shift+2 → {Stances.Name(me.Stance)}");
                Cam.Follow = true; Cam.SetZoom(8f, true);
                _step = 12;
                break;
            }
            case 8:   // threat: speed falls back to 1×; an aggressive companion starts the fight by himself
            {
                Cmd.SetSpeed(3);
                var wiz = _r.Companions.FirstOrDefault(c => c.Char.Cls == "wizard") ?? _r.Companions.FirstOrDefault();
                if (wiz == null) { Check("saldırgan duruş", false, "yoldaş yok (--party=fighter,wizard,cleric)"); _step = 20; break; }
                wiz.Char.Stance = Stances.Aggressive;
                var wp = new V2(wiz.GlobalPosition.X, wiz.GlobalPosition.Z);
                var dir = V2.Normalize(wp - P2 + new V2(0.01f, 0.02f));
                _gob = Bring(wp + dir * 16f);
                _step++; _t = 0; _wait = 0.2f;
                break;
            }
            case 9:
            {
                if (cd.Active)
                {
                    Check("tehlikede hız 1×", Cmd.Speed == 1, $"3× → {Cmd.Speed}×");
                    Check("saldırgan duruş kendiliğinden saldırır", cd.Fight.F.Any(f => f.LifeId == _gob?.Id), $"{_t:F1} sn sonra savaş: {cd.Fight.Log.FirstOrDefault()?.Text}");
                    Check("savaş kendiliğinden durmaz", !GetTree().Paused, GetTree().Paused ? "duraklatıldı" : "akıyor (ayar kapalı)");
                    // stances in the fight: the cleric passive, the fighter holding
                    _passive = _r.Companions.FirstOrDefault(c => c.Char.Cls == "cleric")?.Char;
                    _holder = _r.Companions.FirstOrDefault(c => c.Char.Cls == "fighter")?.Char;
                    if (_passive != null) _passive.Stance = Stances.Passive;
                    if (_holder != null) _holder.Stance = Stances.Hold;
                    _pos0.Clear();
                    foreach (var f in cd.Fight.F) if (f.Char != null) _pos0[f.Char] = new Vector3(f.Pos.X, 0, f.Pos.Y);
                    _passiveSwings = cd.Fight.Log.Count(e => e.A?.Char == _passive && e.Kind is "attack" or "spell");
                    _step++; _t = 0;
                }
                else if (_t > 6f) { Check("saldırgan duruş", false, $"6 sn'de savaş başlamadı (goblin {_gob?.Name})"); _step = 11; }
                break;
            }
            case 10:
            {
                if (_t < 5f && cd.Active) break;
                int swings = cd.Fight?.Log.Count(e => e.A?.Char == _passive && e.Kind is "attack" or "spell") ?? _passiveSwings;
                Check("Pasif hiç vurmaz", _passive == null || swings == _passiveSwings, $"{_passive?.Name}: {swings - _passiveSwings} vuruş");
                var hf = cd.Fight?.F.FirstOrDefault(f => f.Char == _holder);
                float moved = hf != null && _pos0.TryGetValue(_holder, out var p0) ? new Vector2(hf.Pos.X - p0.X, hf.Pos.Y - p0.Z).Length() : 0;
                Check("Yerini koru kıpırdamaz", _holder == null || moved < 1.0f, $"{_holder?.Name}: {moved:F2} m");
                _step++; _t = 0;
                break;
            }
            case 11:   // wait for that fight to end
                if (!cd.Active || _t > 60f)
                {
                    foreach (var c in s.Party) { c.Stance = Stances.Defend; if (!c.Dead) c.Rest(); }
                    _step = 17; _wait = 1f;
                }
                break;
            case 12:   // running off: a fight, then a right click far away — the party gets clear
            {
                var away = P2 + new V2(0, 1);
                var dir = V2.Normalize(P2 - new V2(RegionSpec.GoblinCamp.X, RegionSpec.GoblinCamp.Y));
                _gob = Bring(P2 - dir * 7f);
                if (_gob == null) { _step = 14; break; }
                // two more behind it
                var g2 = Bring(_gob.Pos + new V2(1.5f, -1f), _gob);
                Bring(_gob.Pos + new V2(-1.5f, -1f), g2 ?? _gob);
                cd.Start(_gob, "test: goblin saldırdı");
                Cmd.Select(_r.Player.Character, false);
                foreach (var c in s.Party) if (c != _r.Player.Character) Cmd.Select(c, true);
                var far = P2 + dir * 70f;
                Look(new Vector3(far.X, _r.Heightfield.Height(far.X, far.Y), far.Y), 20f); Cam._Process(0.016);
                Cmd.RightClickAt(new Vector2(800, 450));
                Cam.Follow = true;
                _step++; _t = 0;
                break;
            }
            case 13:
            {
                if (cd.Active && _t < 60f) break;
                var o = cd.Outcome;
                bool caught = o != null && cd.Fight == null;
                Check("yere sağ tık: koşup kurtulur", o != null && (o.Escaped || o.Won), $"{(o == null ? "bitmedi" : o.Escaped ? "KAÇIŞ" : o.Won ? "zafer (yakalanıp dövüştüler)" : "yenilgi")}: {string.Join(" / ", o?.Lines ?? new List<string>())}");
                foreach (var c in s.Party) if (!c.Dead) c.Rest();
                _step = 14; _wait = 1.5f;
                break;
            }
            case 14:   // the player's own attack: right click on a goblin far away — walks up, then fights
            {
                var dir = V2.Normalize(P2 - new V2(RegionSpec.GoblinCamp.X, RegionSpec.GoblinCamp.Y));
                _gob = Bring(P2 - dir * 30f);
                if (_gob == null) { _step = 8; break; }
                foreach (var c in s.Party) c.Stance = Stances.Defend;
                Cmd.Select(_r.Player.Character, false);
                _step = 18; _wait = 0.3f;   // let the body reach the goblin's new place
                break;
            }
            case 18:
            {
                var a = _r.LifeWorld.ActorOf(_gob);
                var gp = new Vector3(_gob.Pos.X, _r.Heightfield.Height(_gob.Pos.X, _gob.Pos.Y), _gob.Pos.Y);
                Look(gp, 12f); Cam._Process(0.016);
                var sp = ScreenOf(gp + Vector3.Up * 0.8f);
                Check("imleç: kılıç (saldır)", Cmd.CursorAt(sp) == CursorKit.Kind.Sword, $"{_gob.Name} üstünde imleç {Cmd.CursorAt(sp)}");
                Cmd.RightClickAt(sp);
                Cam.Follow = true;
                _step = 15; _t = 0;
                break;
            }
            case 15:
            {
                if (cd.Active)
                {
                    var lead = cd.Fight.F.FirstOrDefault(f => f.IsPlayer);
                    Check("oyuncu savaşı başlatır", lead?.Order == "attack" && lead.OrderTarget?.LifeId == _gob.Id,
                        $"{_t:F1} sn yürüdü, savaş: {lead?.Name} → {lead?.OrderTarget?.Name} ({lead?.Order})");
                    _step++; _t = 0;
                }
                else if (_t > 25f) { Check("oyuncu savaşı başlatır", false, $"25 sn'de savaş yok (goblin {V2.Distance(_gob.Pos, P2):F0} m)"); _step++; }
                break;
            }
            case 16:
                if (!cd.Active || _t > 60f) { foreach (var c in s.Party) if (!c.Dead) c.Rest(); _step = 8; _wait = 1.0f; }
                break;
            case 17:   // the smith's workshop; the first-days words
            {
                GD.Print($"[Tur1Test] diyarın stoğu: demir {M.Local.Stock(s.Macro, "iron"):F1}, odun {M.Local.Stock(s.Macro, "wood"):F1}, alet {M.Local.Stock(s.Macro, "tools"):F1}, silah {M.Local.Stock(s.Macro, "arms"):F1}");
                M.Local.Trade(s.Macro, "iron", -6, 0);   // a load of iron came to the realm (test)
                int before = s.SmithStock.Items.Sum(i => i.Count);
                for (int d = 0; d < 3; d++) Smithy.Day(s, 900 + d);
                int after = s.SmithStock.Items.Sum(i => i.Count);
                var shop = Economy.Open(s, ShopKind.Smith, "Demirci");
                var ws = shop.Offers.Where(o => o.Workshop).Select(o => $"{o.Def?.Name} ×{o.Stock}").ToList();
                Check("demirci atölyesi üretir", after > before || after >= Smithy.Shelf, $"3 günde {before} → {after} parça; tezgâhta: {string.Join(", ", ws)}; demir {Smithy.IronLeft(s):F0}, kömür {Smithy.CoalLeft(s):F0}");
                // no iron in the realm, none left at the forge: it goes cold
                double iron = M.Local.Stock(s.Macro, "iron");
                M.Local.Trade(s.Macro, "iron", iron, 0);
                s.Flags["smithIron"] = 0;
                int n0 = s.SmithStock.Items.Sum(i => i.Count);
                var made = Smithy.Day(s, 950);
                Check("demirsiz ocak soğur", made.Count == 0 && Smithy.ColdReason(s)?.Contains("demir") == true && s.SmithStock.Items.Sum(i => i.Count) == n0, $"üretilen {made.Count}, ocak: {Smithy.ColdReason(s) ?? "yanıyor"}");
                M.Local.Trade(s.Macro, "iron", -iron, 0);
                // the innkeeper points at the board and the sellswords; a villager at the smith while unarmed
                var inn = _r.Life.People.FirstOrDefault(p => p.Role == Role.Innkeeper);
                var vil = _r.Life.People.FirstOrDefault(p => p.Role == Role.Farmer);
                var w0 = _r.Player.Character.Weapon; _r.Player.Character.Weapon = null;
                var hi = inn != null ? Talk.Instance.Hints(inn).Select(h => h.Item2).ToList() : new List<string>();
                var hv = vil != null ? Talk.Instance.Hints(vil).Select(h => h.Item2).ToList() : new List<string>();
                _r.Player.Character.Weapon = w0;
                Check("ilk dakika: pano, demirci, kiralık", hi.Any(t => t.Contains("Pano")) && hv.Any(t => t.Contains("demirci")),
                    $"hancı: \"{hi.FirstOrDefault()}\"; köylü: \"{hv.FirstOrDefault()}\"");
                _step = 20;
                break;
            }
            case 20:
            {
                var bar = _r.Hud.FindChild("PartyBar", true, false) as Control;
                var rows = bar?.GetChild(0).GetChildCount() ?? 0;
                Check("ekip çubuğu", bar != null && rows == s.Party.Count(c => !c.Dead) && bar.GetGlobalRect().Size.Y > 40 && bar.GetGlobalRect().End.Y <= 900.5f,
                    $"{rows} satır, alan {bar?.GetGlobalRect()}");
                GD.Print(_ok ? "T1TEST PASS" : "T1TEST FAIL");
                Engine.TimeScale = 1;
                GetTree().Quit(_ok ? 0 : 1);
                _step++;
                break;
            }
        }
    }
}
