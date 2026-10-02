using System;
using System.IO;
using System.Text.Json;
using Godot;

namespace FD.Game;

/// <summary>
/// Faz 2: player settings (user://settings.json). Few for now: whether a fight pauses by itself once when it starts, and how
/// much the dice log says. The settings screen (H) edits these.
/// </summary>
public sealed class Settings
{
    /// <summary>a fight pauses once when it starts so orders can be given (Ayarlar: "Savaş başında duraklat")</summary>
    public bool AutoPause = true;
    /// <summary>dice text over heads in fights</summary>
    public bool DiceOverHeads = true;

    public static Settings Current { get; private set; } = Load();

    static string PathOf => ProjectSettings.GlobalizePath("user://settings.json");
    static readonly JsonSerializerOptions Json = new() { IncludeFields = true, WriteIndented = true };

    static Settings Load()
    {
        try { if (File.Exists(PathOf)) return JsonSerializer.Deserialize<Settings>(File.ReadAllText(PathOf), Json) ?? new Settings(); }
        catch (Exception e) { GD.PushWarning($"[Settings] {e.Message}"); }
        return new Settings();
    }

    public static void Save()
    {
        try { File.WriteAllText(PathOf, JsonSerializer.Serialize(Current, Json)); }
        catch (Exception e) { GD.PushWarning($"[Settings] {e.Message}"); }
    }
}
