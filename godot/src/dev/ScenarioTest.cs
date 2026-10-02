using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using FD.Combat;
using FD.Game;
using FD.Rpg;
using FD.Sim.Life;
using FD.World;
using V2 = System.Numerics.Vector2;
using M = FD.Macro;

namespace FD.Dev;

/// <summary>
/// Faz 2 acceptance (--newgame=race,class,seed,name --scenario): the brief's scenario, headless, through the game's own actions. A new
/// character (made by the creation flow) takes the camp's quest from the inn's board, buys the cheapest weapon the smith has, hires
/// an adventurer at the inn, walks the road and the forest trail to the goblin camp with them and fights whoever comes. If they win:
/// the dead are searched, the camp chest opened, the sim has the camp cleared and the quest closed, the reward is collected at the inn,
/// and 10–20 days later the pioneers' valley starts in the sim. If they lose: fallen, robbed, woken hours later (maybe a lasting wound).
/// Prints the story and PASS/FAIL.
/// </summary>
public partial class ScenarioTest : Node
{
    Region _r;
    int _phase;
    float _t;
    bool _ok = true;
    FightOutcome _o;
    List<V2> _path;
    float _s;
    int _silverStart;
    bool _ours;

    public static void Run(Node host, Region r) => host.AddChild(new ScenarioTest { Name = "ScenarioTest", _r = r });
    void Check(string name, bool pass, string info) { _ok &= pass; GD.Print($"[Senaryo] {name}: {info} {(pass ? "PASS" : "FAIL")}"); }
    void Say(string t) => GD.Print($"[Senaryo] {t}");

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        var s = _r.Session; var me = s.Player;
        _r.Combat.Unattended = true;
        _r.Combat.Ended += o => _o = o;
        GameClock.SetHour(9.5f);
        Say(s.Describe());
        Check("1. yeni karakter", me != null && me.Weapon == null && me.Inv.Silver is >= 5 and <= 15 && me.HeroId != null && s.Macro.Hero(me.HeroId.Value)?.State == "player",
            $"{me.FullName}: {Rules.RaceName(me.Race)} {Rules.ClassName(me.Cls)} Sv{me.Level}, can {me.Hp}, ZS {me.Ac}, {me.Inv.Silver} gümüş, silahsız; simde kahraman (oyuncu)");
        _silverStart = me.Inv.Silver;
        // the board
        _r.Player.Teleport(InnSite.BoardPos + new Vector2(0.8f, 0.8f), 0, _r.Heightfield);
        var prompt = _r.Hud.Prompts.Select(f => f()).FirstOrDefault(x => x != null);
        var q = _r.Board.Take();
        var taken = M.Local.TakenQuest(s.Macro);
        _ours = q != null;
        Check("2. panodan ilan", prompt?.text == "İlan panosu" && (q != null ? q.TakenBy.Contains(me.HeroId.Value) : taken != null),
            q != null ? $"\"{s.Camp.Name} temizlensin\" — {q.Bounty:F0} altın, artık oyuncunun"
            : taken != null ? $"ilanı simdeki {string.Join(", ", taken.Value.heroes.Select(h => h.Name))} çoktan kopardı ve yolda; ödülsüz gidilecek" : "panoda ilan yok");
        // a weapon from the smith
        var smith = Economy.Open(s, ShopKind.Smith, "demirci");
        var buy = smith.Offers.Where(o => o.Stock > 0 && o.Def.IsWeapon && o.Buy <= me.Inv.Silver).OrderByDescending(o => o.Def.DiceS * 10 - o.Buy).FirstOrDefault();
        if (buy != null && Economy.Buy(s, smith, buy.Id, me, out _)) { me.Weapon = buy.Id; _r.Player.SetCharacter(me); }
        Check("3. demirciden silah", me.Weapon != null, me.Weapon != null ? $"{Items.Get(me.Weapon).Name} ({Items.Get(me.Weapon).DamageText}) {buy.Buy} gümüşe; kesede {me.Inv.Silver}" : "parası yetmedi");
        // hire at the inn
        var guest = _r.Life.People.Where(p => p.Guest && !p.Hired && PartyManager.ForHire(p)).OrderByDescending(p => p.Level).ThenByDescending(p => p.HeroId).FirstOrDefault();
        Character comp = null;
        if (guest != null) { _r.Player.Teleport(new Vector2(guest.Pos.X + 1.2f, guest.Pos.Y + 1.2f), 0, _r.Heightfield); comp = _r.Party.Hire(guest); }
        Check("4. handa yoldaş", comp != null, comp != null ? $"{comp.Name} ({Rules.ClassName(comp.Cls)} Sv{comp.Level}{(comp.HeroId != null ? ", simdeki bir kahraman" : ", paralı asker")}), haftalığı {comp.WageSilver} gümüş" : "kiralık kimse yok");
        // the march
        var inn = _r.Life.PlaceOf(PlaceKind.Inn);
        _path = NpcBands.PathIn(inn.Door);
        _s = 0;
        Engine.TimeScale = 4.0;
        Say("Yola çıkıldı: handan yola, orman patikasına, goblin kampına.");
    }

    public override void _Process(double delta)
    {
        float dt = (float)delta;
        _t += dt;
        var s = _r.Session; var me = s.Player;
        switch (_phase)
        {
            case 0:
            {
                if (_r.Combat.Active) { _phase = 1; Say($"Savaş: {_r.Combat.Fight.Log.FirstOrDefault()?.Text} ({_r.Combat.Fight.F.Count(f => f.Side == FSide.Party)} vs {_r.Combat.Fight.F.Count(f => f.Side == FSide.Foe)})"); break; }
                _s += 3.6f * dt;
                var (pos, d) = At(_path, _s);
                _r.Player.Teleport(new Vector2(pos.X, pos.Y), MathF.Atan2(d.X, d.Y), _r.Heightfield);
                if (_t > 600f) { Check("5. kampa varış", false, "goblin saldırmadı"); End(); }
                break;
            }
            case 1:
                if (_o != null) { _phase = 2; _t = 0; }
                break;
            case 2:
                if (_t < 30f && _r.Combat.RunningOff > 0) break;
                Engine.TimeScale = 1.0;
                Result();
                _phase = 3;
                break;
        }
    }

    static (V2, V2) At(List<V2> path, float s)
    {
        for (int i = 0; i + 1 < path.Count; i++)
        {
            float L = V2.Distance(path[i], path[i + 1]);
            if (s <= L) { var d = V2.Normalize(path[i + 1] - path[i] + new V2(1e-4f, 0)); return (path[i] + d * s, d); }
            s -= L;
        }
        return (path[^1], new V2(0, -1));
    }

    void Result()
    {
        var s = _r.Session; var me = s.Player; var o = _o; var cp = s.Camp;
        Check("5. savaş", true, $"{(o.Won ? "ZAFER" : "YENİLGİ")} — {string.Join(" / ", o.Lines)}");
        if (o.Won)
        {
            // search the dead, open the chest
            int looted = 0; var take = new List<string>();
            foreach (var p in _r.Life.People.Where(p => p.Role == Role.Goblin && p.Dead).ToList())
            {
                _r.Player.Teleport(new Vector2(p.Pos.X + 0.6f, p.Pos.Y), 0, _r.Heightfield);
                var pr = _r.Hud.Prompts.Select(f => f()).FirstOrDefault(x => x != null && x.Value.text.Contains("cesedi ara"));
                if (pr == null) continue;
                pr.Value.act(); looted++;
            }
            Check("6. yağma", looted == o.Killed && me.Inv.Items.Count > 1, $"{looted} ceset arandı; çanta: {string.Join(", ", me.Inv.Items.Select(i => $"{i.Count} {Items.Get(i.Id).Name.ToLowerInvariant()}"))}, kese {me.Inv.Silver}");
            if (o.CampCleared)
            {
                _r.Player.Teleport(CampSite.ChestPos, 0, _r.Heightfield);
                int sil = me.Inv.Silver;
                var got = _r.Gathering.OpenChest();
                var q = s.Macro.W.Quests.FirstOrDefault(x => x.Camp == cp.Id && x.TakenBy.Contains(me.HeroId ?? -1));
                bool closed = _ours ? q?.Done != null && s.Link.Reward > 0 : !s.Macro.W.Quests.Any(x => x.Camp == cp.Id && x.Open);
                Check("7. simde kamp temizlendi, ilan kapandı", !cp.Alive && closed,
                    $"{cp.Name} temizlendi (gün {cp.ClearedDay}); {(_ours ? "ilan tamam" : "ilan başkasınındı, boşa çıktı")}; sandıktan +{me.Inv.Silver - sil} gümüş; bekleyen ödül {s.Link.Reward:F0} altın");
                // back to the inn for the reward
                var keeper = _r.Life.People.First(p => p.Role == Role.Innkeeper);
                _r.Player.Teleport(new Vector2(keeper.Pos.X + 1.2f, keeper.Pos.Y + 1.2f), 0, _r.Heightfield);
                int before = me.Inv.Silver;
                _r.Services.Reward();
                Check("8. ödül", _ours ? me.Inv.Silver > before && s.Link.Reward == 0 : me.Inv.Silver == before, _ours ? $"hancıdan +{me.Inv.Silver - before} gümüş (kese {me.Inv.Silver})" : "ilan oyuncunun değildi: ödül yok (doğru)");
                // the pioneers
                double sd = s.Link.SettlersDay ?? 0; int steps = 0;
                while (s.Macro.W.Day < sd + 3 && steps < 30) { s.Macro.Step(); M.Local.DayTick(s.Macro); steps++; }
                var hub = s.Macro.W.Hubs.FirstOrDefault(h => h.Origin == cp.Id);
                var ev = s.Macro.W.Events.LastOrDefault(e => e.Kind == "hub" && e.Day >= sd && (e.Text.Contains("öncüler") || (hub != null && e.Text.Contains(hub.Name))));
                Check("9. öncüler", (hub != null || ev != null) && sd - (cp.ClearedDay ?? 0) is >= M.Local.SETTLERS_MIN and <= M.Local.SETTLERS_MAX,
                    $"{sd - (cp.ClearedDay ?? 0):F0} gün sonra: {hub?.Name ?? "-"} ({hub?.Phase}); {ev?.Text}");
            }
            else Say($"Kamp bu savaşla düşmedi ({cp.Count:F0} goblin kaldı); ikinci bir sefer gerek.");
        }
        else
        {
            bool woke = s.Party.Where(c => !c.Dead).All(c => !c.Down && c.Hp >= 1);
            string wounds = string.Join(", ", s.Party.Where(c => c.Wounds.Count > 0).Select(c => $"{c.Name}: {Wound.Name(c.Wounds[^1].Kind)} «{c.Epithet}»"));
            Check("6. yenilgi: bayılma, soyulma, uyanma", o.GameOver || (woke && (o.Fate != "rob" || s.Party.Sum(c => c.Inv.Silver) == 0)),
                $"goblinlerin kararı: {o.Fate}; {(o.GameOver ? "herkes öldü, dünya silindi" : $"uyanıldı {GameClock.TimeString}")}{(wounds != "" ? $"; kalıcı yara: {wounds}" : "; kalıcı yara yok")}");
        }
        End();
    }

    void End()
    {
        Engine.TimeScale = 1.0;
        GD.Print(_ok ? "SCENARIO PASS" : "SCENARIO FAIL");
        GetTree().Quit(_ok ? 0 : 1);
        SetProcess(false);
    }
}
