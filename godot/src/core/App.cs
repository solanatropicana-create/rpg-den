using Godot;

namespace FD.Core;

/// <summary>
/// Game-wide autoload: registers input actions (physical keys, so WASD works on any layout), window mode
/// (fullscreen in exported release builds, windowed 1600×900 otherwise; CLI: --fullscreen / --windowed),
/// F11 or Alt+Enter toggles fullscreen.
/// Physics layers: 1 terrain · 2 props (buildings, trees, rocks) · 3 player · 4 actors (NPCs).
/// </summary>
public partial class App : Node
{
    public const uint LayerTerrain = 1, LayerProps = 2, LayerPlayer = 4, LayerActors = 8;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        GameInput.Ensure();
        var args = OS.GetCmdlineUserArgs();
        bool fs = OS.HasFeature("template") && OS.HasFeature("release");
        foreach (var a in args)
        {
            if (a == "--fullscreen") fs = true;
            if (a == "--windowed") fs = false;
        }
        if (fs && DisplayServer.GetName() != "headless")
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
    }

    public override void _UnhandledInput(InputEvent e)
    {
        if (e.IsActionPressed("toggle_fullscreen"))
        {
            ToggleFullscreen();
            GetViewport().SetInputAsHandled();
        }
    }

    public static void ToggleFullscreen()
    {
        var mode = DisplayServer.WindowGetMode();
        bool full = mode == DisplayServer.WindowMode.Fullscreen || mode == DisplayServer.WindowMode.ExclusiveFullscreen;
        DisplayServer.WindowSetMode(full ? DisplayServer.WindowMode.Windowed : DisplayServer.WindowMode.Fullscreen);
    }
}

/// <summary>Input actions used by the game (idempotent registration).</summary>
public static class GameInput
{
    static bool _done;

    public static void Ensure()
    {
        if (_done) return;
        _done = true;
        Add("move_forward", Key.W, Key.Up);
        Add("move_back", Key.S, Key.Down);
        Add("move_left", Key.A, Key.Left);
        Add("move_right", Key.D, Key.Right);
        Add("jump", Key.Space);
        Add("sprint", Key.Shift);
        Add("walk", Key.Ctrl, Key.Alt);
        Add("release_mouse", Key.Escape);
        Add("toggle_fullscreen", Key.F11);
        InputMap.ActionAddEvent("toggle_fullscreen", new InputEventKey { PhysicalKeycode = Key.Enter, AltPressed = true });
        Add("interact", Key.E);
        Add("map", Key.M);
    }

    static void Add(string action, params Key[] keys)
    {
        if (!InputMap.HasAction(action)) InputMap.AddAction(action, 0.2f);
        foreach (var k in keys) InputMap.ActionAddEvent(action, new InputEventKey { PhysicalKeycode = k });
    }
}
