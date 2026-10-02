using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Godot;
using FD.Rpg;
using FD.World;
using M = FD.Macro;

namespace FD.Game;

/// <summary>Everything about the playthrough that is not in the macro world: the party, the clock, where the player stands and
/// what happened in the region (Faz 2 H: Demir mod, tek yuva).</summary>
public sealed class LocalState
{
    public int Version = 1;
    public double Seed;
    public int BaseLocalDay, BaseMacroDay;
    public double ClockHours;
    public List<Character> Party = new();
    public int NextCharId = 1;
    public float PlayerX, PlayerZ, PlayerYaw;
    public bool HasPosition;
    public string SavedAt, Reason;
    /// <summary>region facts the macro world does not hold (looted chests, the cage, bodies…)</summary>
    public Dictionary<string, double> Flags = new();
    /// <summary>the goblin camp's chest (stolen goods)</summary>
    public Inventory CampChest = new();
}

/// <summary>
/// Faz 2 (H) iron-mode save: one slot (user://save/), written automatically. The macro world goes through <c>Sim.Save</c>
/// (gzip, deterministic continuation) and the local state as JSON next to it; both are written to temp files and swapped in, so a
/// crash mid-save never leaves half a slot.
/// </summary>
public static class SaveGame
{
    public const string DirUser = "user://save";
    static string Dir => ProjectSettings.GlobalizePath(DirUser);
    static string WorldPath => Path.Combine(Dir, "world.fdsave");
    static string LocalPath => Path.Combine(Dir, "local.json");

    static readonly JsonSerializerOptions Json = new() { IncludeFields = true, IgnoreReadOnlyProperties = true, WriteIndented = true };

    /// <summary>set by <see cref="Load"/>, consumed by the region (player position etc.)</summary>
    public static LocalState Pending;
    public static string LastError;
    /// <summary>when and why the slot was last written (this session)</summary>
    public static string LastSaved;

    public static bool Exists() => File.Exists(WorldPath) && File.Exists(LocalPath);

    public static string Describe()
    {
        try
        {
            var st = JsonSerializer.Deserialize<LocalState>(File.ReadAllText(LocalPath), Json);
            var p = st?.Party?.Count > 0 ? st.Party[0] : null;
            if (p == null) return null;
            int day = (int)Math.Floor(st.ClockHours / 24.0) + 1;
            return $"{p.Name}, {Rules.RaceName(p.Race)} {Rules.ClassName(p.Cls)} Sv{p.Level} · gün {day}";
        }
        catch { return null; }
    }

    public static LocalState Capture(Session s)
    {
        var st = new LocalState
        {
            Seed = s.Seed, BaseLocalDay = s.BaseLocalDay, BaseMacroDay = s.BaseMacroDay, ClockHours = GameClock.TotalHours,
            Party = new List<Character>(s.Party), NextCharId = s.PeekNextCharId(), SavedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
            Flags = new Dictionary<string, double>(s.Flags), CampChest = s.CampChest,
        };
        var pl = Region.Current?.Player;
        if (pl != null)
        {
            st.PlayerX = pl.GlobalPosition.X; st.PlayerZ = pl.GlobalPosition.Z; st.PlayerYaw = pl.Facing; st.HasPosition = true;
        }
        return st;
    }

    /// <summary>Write the slot (macro world + local state). Returns false and sets <see cref="LastError"/> on failure.</summary>
    public static bool Save(Session s, string reason)
    {
        try
        {
            Directory.CreateDirectory(Dir);
            s.SyncPlayerToMacro();
            var st = Capture(s);
            st.Reason = reason;
            string wTmp = WorldPath + ".tmp", lTmp = LocalPath + ".tmp";
            s.Macro.Save(wTmp);
            File.WriteAllText(lTmp, JsonSerializer.Serialize(st, Json));
            File.Move(wTmp, WorldPath, true);
            File.Move(lTmp, LocalPath, true);
            GD.Print($"[Save] {reason}: gün {GameClock.Day + 1} {GameClock.Hour:F1}, makro gün {s.Macro.W.Day}");
            LastSaved = $"{GameClock.TimeString} ({reason})";
            LastError = null;
            return true;
        }
        catch (Exception e)
        {
            LastError = e.Message;
            GD.PushError($"[Save] kaydedilemedi: {e}");
            return false;
        }
    }

    /// <summary>Open the slot: macro world, party, clock. The region reads <see cref="Pending"/> for the player's position.</summary>
    public static Session Load()
    {
        var st = JsonSerializer.Deserialize<LocalState>(File.ReadAllText(LocalPath), Json) ?? throw new InvalidDataException("local.json boş");
        var sim = M.Sim.Load(WorldPath);
        var s = Session.FromSim(sim, st.Seed, st.BaseLocalDay, st.BaseMacroDay);
        foreach (var c in st.Party) s.Party.Add(c);
        s.Player = s.Party.Count > 0 ? s.Party[0] : null;
        if (s.Player != null) s.Player.IsPlayer = true;
        s.SetNextCharId(st.NextCharId);
        foreach (var kv in st.Flags) s.Flags[kv.Key] = kv.Value;
        s.CampChest = st.CampChest ?? new Inventory();
        GameClock.SetTotalHours(st.ClockHours);
        Pending = st;
        Session.Current = s;
        GD.Print($"[Save] açıldı: {s.Describe()}");
        return s;
    }

    /// <summary>Iron mode: the party is gone, so is the world.</summary>
    public static void Delete()
    {
        try
        {
            if (File.Exists(WorldPath)) File.Delete(WorldPath);
            if (File.Exists(LocalPath)) File.Delete(LocalPath);
        }
        catch (Exception e) { GD.PushError($"[Save] silinemedi: {e.Message}"); }
    }
}
