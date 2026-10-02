using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using FD.Core;
using FD.Rpg;
using FD.Sim.Life;
using FD.World;
using M = FD.Macro;

namespace FD.Game;

/// <summary>
/// Faz 2 F: what can be picked up in the region with E. <b>Healing herbs</b> grow in patches at the forest's edge and along the stream
/// (24 places, fixed for the region); a picked patch grows back in two days (kept in the save). <b>Dead goblins</b> can be searched
/// once (their weapon, trinkets, a few silver; the chief carries more). The <b>camp chest</b> holds what the goblins stole and the camp's
/// loot (the macro camp's value): free to take once the camp has fallen; while it stands, only with no goblin near (and they miss
/// part of it).
/// </summary>
public partial class Gathering : Node3D
{
    Region _r;
    Session _s;
    readonly List<(Vector2 pos, Node3D node)> _herbs = new();
    readonly HashSet<int> _looted = new();
    public const int Patches = 24, RegrowDays = 2;

    public void Init(Region r)
    {
        _r = r; _s = r.Session;
        Name = "Gathering";
        BuildHerbs();
        r.Hud.Prompts.Add(HerbPrompt);
        r.Hud.Prompts.Add(CorpsePrompt);
        r.Hud.Prompts.Add(ChestPrompt);
    }

    Character Me => _r.Player.Character;
    Vector2 Here => new(_r.Player.GlobalPosition.X, _r.Player.GlobalPosition.Z);

    static string Took(Inventory got)
    {
        var parts = new List<string>();
        foreach (var it in got.Items) parts.Add(it.Count > 1 ? $"{it.Count} {Items.Get(it.Id)?.Name.ToLowerInvariant()}" : Items.Get(it.Id)?.Name.ToLowerInvariant());
        if (got.Silver > 0) parts.Add(Rules.Money(got.Silver));
        return parts.Count > 0 ? string.Join(", ", parts) : "hiçbir şey";
    }

    void Give(Inventory got)
    {
        foreach (var it in got.Items) Me.Inv.Add(it.Id, it.Count);
        Me.Inv.Silver += got.Silver;
    }

    string Load() => $"taşınan {Me.Inv.Weight:F1}/{Me.Capacity:F0} kg{(Me.Overloaded ? " — aşırı yük, yavaşsın" : Me.Inv.Weight > Me.Capacity * 0.5f ? " — ağır" : "")}";

    // ------------------------------------------------------------------------------------------------ herbs
    void BuildHerbs()
    {
        var hf = _r.Heightfield;
        var rng = new M.Rng(4242);
        var mat = Models.MaterialFor(MatKind.Static);
        int tries = 0;
        while (_herbs.Count < Patches && tries++ < 6000)
        {
            float x = (float)(rng.Next() * 1000 - 500), z = (float)(rng.Next() * 1000 - 500);
            float fd = hf.ForestDensity(x, z);
            float sd = hf.StreamDistance(x, z, out _);
            bool edge = fd > 0.22f && fd < 0.6f, stream = sd < 7f && sd > 2.5f;
            if (!(edge || stream) || hf.IsWater(x, z) || hf.RoadDistance(x, z) < 6f || hf.Slope(x, z) > 22f) continue;
            if ((new Vector2(x, z) - RegionSpec.VillageCenter).Length() < 90f || (new Vector2(x, z) - RegionSpec.GoblinCamp).Length() < 25f) continue;
            if (_herbs.Any(h => h.pos.DistanceTo(new Vector2(x, z)) < 30f)) continue;
            var node = new MeshInstance3D { Mesh = HerbMesh(rng, mat), Position = new Vector3(x, hf.Height(x, z), z), VisibilityRangeEnd = 70f, Name = $"Herb{_herbs.Count}" };
            AddChild(node);
            _herbs.Add((new Vector2(x, z), node));
        }
        Refresh();
    }

    static ArrayMesh HerbMesh(M.Rng r, Material mat)
    {
        var k = new MeshKit();
        var leaf = MeshKit.C(0x3f7a3a); var tip = MeshKit.C(0x6aa84a);
        for (int i = 0; i < 9; i++)
        {
            float a = i * 0.7f + (float)r.Next(), len = 0.28f + (float)r.Next() * 0.18f;
            var dir = new Vector3(MathF.Cos(a), 0, MathF.Sin(a));
            k.Blade(dir * 0.04f, dir * len * 0.7f + Vector3.Up * len, 0.06f, leaf, tip, new Vector3(-dir.Z, 0, dir.X));
        }
        var flower = MeshKit.C(0x9a5ad0);
        for (int i = 0; i < 4; i++)
        {
            float a = i * 1.6f + (float)r.Next();
            var p = new Vector3(MathF.Cos(a) * 0.1f, 0.34f + (float)r.Next() * 0.1f, MathF.Sin(a) * 0.1f);
            k.Blob(p, new Vector3(0.035f, 0.03f, 0.035f), flower, 0, 0f, (uint)(i + 7));
        }
        return k.ToMesh(mat);
    }

    int ReadyDay(int i) => _s.Flags.TryGetValue($"herb{i}", out var d) ? (int)d : 0;

    void Refresh() { for (int i = 0; i < _herbs.Count; i++) _herbs[i].node.Visible = GameClock.Day >= ReadyDay(i); }

    float _refresh;
    public override void _Process(double delta)
    {
        _refresh -= (float)delta;
        if (_refresh <= 0) { _refresh = 2f; Refresh(); }
    }

    (string, Action)? HerbPrompt()
    {
        for (int i = 0; i < _herbs.Count; i++)
        {
            if (_herbs[i].pos.DistanceTo(Here) > 2.2f || GameClock.Day < ReadyDay(i)) continue;
            int idx = i;
            return ("Şifalı ot topla", () => Pick(idx));
        }
        return null;
    }

    void Pick(int i)
    {
        var r = new M.Rng(i * 131 + GameClock.Day * 17 + 5);
        int n = (int)r.Int(1, 3);
        Me.Inv.Add("herb", n);
        _s.Flags[$"herb{i}"] = GameClock.Day + RegrowDays;
        Refresh();
        _r.Hud.Toast($"{n} demet şifalı ot topladın ({Load()}). Şifacı (rahip) ve hancı alır.", 4f);
    }

    /// <summary>herb patch positions (tests, map)</summary>
    public IReadOnlyList<Vector2> HerbPositions => _herbs.Select(h => h.pos).ToList();

    // ------------------------------------------------------------------------------------------------ the dead
    (string, Action)? CorpsePrompt()
    {
        foreach (var p in _r.Life.People)
        {
            if (!p.Dead || p.Role != Role.Goblin || _looted.Contains(p.Id)) continue;
            if (new Vector2(p.Pos.X, p.Pos.Y).DistanceTo(Here) > 2.0f) continue;
            var who = p;
            return ($"{who.Name} — cesedi ara", () => Loot(who));
        }
        return null;
    }

    public Inventory Loot(Person p)
    {
        _looted.Add(p.Id);
        var got = Economy.GoblinLoot(p.Id * 31 + (int)_s.Seed, p.IsBoss, p.FightTool == "proc_bow" || p.Work % 4 == 3, p.Work % 3 == 0);
        Give(got);
        _r.Hud.Toast($"{p.Name}'in üstünden: {Took(got)} ({Load()}).", 4.5f);
        return got;
    }

    // ------------------------------------------------------------------------------------------------ the camp chest
    bool CampFallen => _s.Camp == null || !_s.Camp.Alive;

    (string, Action)? ChestPrompt()
    {
        if (CampSite.ChestPos == default || CampSite.ChestPos.DistanceTo(Here) > 2.4f) return null;
        bool empty = _s.CampChest.Items.Count == 0 && _s.CampChest.Silver == 0 && (CampFallen ? _s.Link.CampLoot <= 0 : (_s.Camp?.Loot ?? 0) < 1);
        if (empty) return ("Kamp sandığı (boş)", () => _r.Hud.Toast("Sandıkta kemik ve paçavradan başka bir şey yok.", 3f));
        return (CampFallen ? "Kamp sandığını aç" : "Kamp sandığını sessizce boşalt", () => OpenChest());
    }

    public Inventory OpenChest()
    {
        if (!CampFallen)
        {
            var near = _r.Life.People.FirstOrDefault(p => p.Role == Role.Goblin && !p.Dead && p.Visible && new Vector2(p.Pos.X, p.Pos.Y).DistanceTo(Here) < 22f);
            if (near != null) { _r.Hud.Toast($"{near.Name} tam başında. Önce goblinleri hallet ya da gece gel.", 4f); return null; }
        }
        var got = new Inventory();
        _s.CampChest.MoveAllTo(got);
        int silver;
        if (CampFallen) { silver = (int)Math.Round(_s.Link.CampLoot * Rules.SilverPerGold); _s.Link.CampLoot = 0; }
        else { var cp = _s.Camp; double take = Math.Floor(cp.Loot * 0.25); cp.Loot -= take; silver = (int)(take * Rules.SilverPerGold); }
        got.Silver += silver;
        Give(got);
        _r.Hud.Toast($"Sandıktan: {Took(got)} ({Load()}).", 5f);
        SaveGame.Save(_s, "sandık");
        return got;
    }
}
