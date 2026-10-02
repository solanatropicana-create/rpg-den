using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Godot;
using FD.Game;
using FD.Rpg;
using FD.World;
using M = FD.Macro;

namespace FD.Dev;

/// <summary>
/// Faz 2 H (--savetest): iron-mode slot. Builds a played state (a companion hired and controlled by Tab, items, a lasting wound, a
/// picked herb patch, the quest taken, the hero elsewhere, the clock moved), saves, and then: (1) the same world continues — the saved
/// macro world stepped 10 days gives the same history as the live one stepped 10 days; (2) the region reopened from the slot puts
/// everyone back: the controlled body and where it stood, the companions, packs, wounds, purse, clock, flags, the taken quest.
/// PASS/FAIL, exit code 0/1.
/// </summary>
public partial class SaveTest : Node
{
    int _phase;
    bool _ok = true;
    string _partyJson, _flagsJson;
    double _clock;
    Vector3 _pos;
    string _controlled;
    int _companions;
    static readonly JsonSerializerOptions Json = new() { IncludeFields = true, IgnoreReadOnlyProperties = true };

    public static void Run(Node host, Region r) { var t = new SaveTest { Name = "SaveTest" }; host.AddChild(t); t.Setup(r); }
    void Check(string name, bool pass, string info) { _ok &= pass; GD.Print($"[SaveTest] {name}: {info} {(pass ? "PASS" : "FAIL")}"); }

    static string Hash(string s) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(s)))[..16];
    static string History(M.Sim sim, double fromDay) => string.Join("\n", sim.W.Events.Where(e => e.Day >= fromDay).Select(e => $"{e.Day}|{e.Kind}|{e.Text}"));

    void Setup(Region r)
    {
        ProcessMode = ProcessModeEnum.Always;
        var s = r.Session;
        var me = s.Player;
        // a played state
        var guest = r.Life.People.First(p => p.Guest && PartyManager.ForHire(p));
        var comp = r.Party.Hire(guest);
        me.Inv.Silver = 37; me.Inv.Add("potion", 2); me.Inv.Add("scimitar"); me.Weapon = "scimitar";
        me.AddWound("limp", (int)s.Macro.W.Day, "Kırık Diş");
        comp.Inv.Add("herb", 4);
        s.Flags["herb3"] = GameClock.Day + 2;
        s.CampChest.Add("trinket", 2);
        var q = M.Local.TakeQuest(s.Macro);
        GameClock.SetHour(15.5f);
        r.Player.Teleport(new Vector2(-60f, 80f), 1.0f, r.Heightfield);
        r.Party.SwitchControl(1);
        _controlled = r.Player.Character.Name;
        _pos = r.Player.GlobalPosition;
        _companions = r.Companions.Count(c => !c.Char.Dead);
        _clock = GameClock.TotalHours;
        bool saved = SaveGame.Save(s, "test");
        _partyJson = JsonSerializer.Serialize(s.Party, Json);
        _flagsJson = JsonSerializer.Serialize(s.Flags.OrderBy(k => k.Key).ToList(), Json);
        Check("kaydedildi", saved && SaveGame.Exists(), $"{SaveGame.Describe()}; ilan {(q != null ? "alındı" : "yok")}, kontrol {_controlled}, ekip {s.Party.Count}");
        // (1) the same world continues
        double day0 = s.Macro.W.Day;
        for (int i = 0; i < 10; i++) { s.Macro.Step(); M.Local.DayTick(s.Macro); }
        string live = History(s.Macro, day0);
        var loaded = SaveGame.Load();
        for (int i = 0; i < 10; i++) { loaded.Macro.Step(); M.Local.DayTick(loaded.Macro); }
        string again = History(loaded.Macro, day0);
        Check("aynı dünya sürüyor", live == again && live.Length > 0, $"10 gün: canlı {Hash(live)} = kayıttan {Hash(again)} ({live.Split('\n').Length} olay)");
        // (2) reopen the region from the slot
        SaveGame.Load();
        Region.Built += OnBuilt;
        GetTree().CallDeferred("change_scene_to_file", "res://scenes/Region.tscn");
    }

    void OnBuilt(Region r)
    {
        Region.Built -= OnBuilt;
        CallDeferred(nameof(Verify));
    }

    void Verify()
    {
        var r = Region.Current; var s = r.Session;
        float d = r.Player.GlobalPosition.DistanceTo(_pos);
        Check("yer ve kontrol", d < 0.6f && r.Player.Character.Name == _controlled, $"kontrol {r.Player.Character.Name}, kaydedilen yere {d:F2} m");
        Check("ekip", r.Companions.Count(c => !c.Char.Dead) == _companions && s.Party.Count == _companions + 1, $"{s.Party.Count} kişi, {r.Companions.Count} yoldaş bedeni");
        string party = JsonSerializer.Serialize(s.Party, Json);
        Check("çantalar, yaralar, kese", party == _partyJson, $"{s.Player.Name}: {s.Player.Inv.Silver} gümüş, {string.Join(", ", s.Player.Inv.Items.Select(i => $"{i.Count} {i.Id}"))}, silah {s.Player.Weapon}, yara {string.Join(",", s.Player.Wounds.Select(w => w.Kind))} «{s.Player.Epithet}»");
        Check("saat ve bayraklar", Math.Abs(GameClock.TotalHours - _clock) < 0.05 && JsonSerializer.Serialize(s.Flags.OrderBy(k => k.Key).ToList(), Json) == _flagsJson && s.CampChest.Count("trinket") == 2,
            $"{GameClock.TimeString}; {s.Flags.Count} bayrak; kamp sandığında {s.CampChest.Count("trinket")} ıvır zıvır");
        var pid = s.Link.Player ?? -1;
        Check("ilan", s.Macro.W.Quests.Any(q => q.TakenBy.Contains(pid) && q.Done == null), "oyuncunun aldığı ilan kayıtta");
        GD.Print(_ok ? "SAVETEST PASS" : "SAVETEST FAIL");
        GetTree().Quit(_ok ? 0 : 1);
    }
}
