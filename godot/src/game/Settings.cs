using System;
using System.IO;
using System.Text.Json;
using Godot;

namespace FD.Game;

/// <summary>
/// Faz 2: player settings (user://settings.json): whether a fight pauses by itself when it starts (Tur 1: off), dice over heads,
/// the speed falling back to 1× at a threat, edge scrolling. The game menu (Esc) edits these.
/// </summary>
public sealed class Settings
{
    /// <summary>settings file version (2: Tur 1 — a fight no longer pauses by itself unless the player turns it on)</summary>
    public int Version = 2;
    /// <summary>a fight pauses once when it starts so orders can be given (Ayarlar: "Savaş başında duraklat"; Tur 1: off by default)</summary>
    public bool AutoPause = false;
    /// <summary>small pale dice numbers over heads in fights</summary>
    public bool DiceOverHeads = true;
    /// <summary>Tur 1 C: the speed drops back to 1× when a threat comes into sight</summary>
    public bool SlowOnThreat = true;
    /// <summary>Tur 1 A: the mouse at the screen's edge pans the camera</summary>
    public bool EdgeScroll = true;

    public static Settings Current { get; private set; } = Load();

    static string PathOf => ProjectSettings.GlobalizePath("user://settings.json");
    static readonly JsonSerializerOptions Json = new() { IncludeFields = true, WriteIndented = true };

    static Settings Load()
    {
        try
        {
            if (File.Exists(PathOf))
            {
                var s = JsonSerializer.Deserialize<Settings>(File.ReadAllText(PathOf), Json) ?? new Settings();
                if (!File.ReadAllText(PathOf).Contains("\"Version\"")) { s.Version = 2; s.AutoPause = false; }   // Faz 2 files: auto-pause was on by default
                return s;
            }
        }
        catch (Exception e) { GD.PushWarning($"[Settings] {e.Message}"); }
        return new Settings();
    }

    public static void Save()
    {
        try { File.WriteAllText(PathOf, JsonSerializer.Serialize(Current, Json)); }
        catch (Exception e) { GD.PushWarning($"[Settings] {e.Message}"); }
    }
}
