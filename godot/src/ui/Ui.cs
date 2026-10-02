using System;
using Godot;

namespace FD.UI;

/// <summary>Faz 2: shared look of the game's panels and buttons (dark wood, gold rim, parchment text) and small builders.</summary>
public static class Ui
{
    public static readonly Color Gold = new(0.95f, 0.82f, 0.52f), Text = new(0.93f, 0.9f, 0.84f), Dim = new(0.72f, 0.68f, 0.6f),
        Faint = new(0.55f, 0.52f, 0.47f), Bad = new(0.95f, 0.45f, 0.38f), Good = new(0.55f, 0.85f, 0.45f), Panel = new(0.09f, 0.075f, 0.06f, 0.92f);

    public static StyleBoxFlat Box(Color bg, Color border, int bw = 2, int radius = 6, int margin = 12)
    {
        return new StyleBoxFlat
        {
            BgColor = bg, BorderColor = border,
            BorderWidthLeft = bw, BorderWidthRight = bw, BorderWidthTop = bw, BorderWidthBottom = bw,
            CornerRadiusTopLeft = radius, CornerRadiusTopRight = radius, CornerRadiusBottomLeft = radius, CornerRadiusBottomRight = radius,
            ContentMarginLeft = margin, ContentMarginRight = margin, ContentMarginTop = margin * 0.75f, ContentMarginBottom = margin * 0.75f,
        };
    }

    public static StyleBoxFlat PanelStyle() => Box(Panel, new Color(0.72f, 0.58f, 0.32f, 0.9f), 2, 6, 16);

    /// <summary>Theme with the game's buttons, line edits and labels.</summary>
    public static Theme MakeTheme(int fontSize = 18)
    {
        var t = new Theme { DefaultFontSize = fontSize };
        var normal = Box(new Color(0.16f, 0.12f, 0.09f, 0.95f), new Color(0.55f, 0.43f, 0.24f), 2, 5, 10);
        var hover = Box(new Color(0.24f, 0.18f, 0.12f, 0.97f), new Color(0.85f, 0.68f, 0.36f), 2, 5, 10);
        var pressed = Box(new Color(0.36f, 0.26f, 0.13f, 1f), new Color(0.98f, 0.82f, 0.45f), 2, 5, 10);
        var disabled = Box(new Color(0.12f, 0.1f, 0.09f, 0.8f), new Color(0.3f, 0.26f, 0.2f), 2, 5, 10);
        var focus = Box(new Color(0, 0, 0, 0), new Color(0.98f, 0.85f, 0.5f, 0.8f), 2, 5, 10);
        foreach (var type in new[] { "Button", "OptionButton", "CheckBox" })
        {
            t.SetStylebox("normal", type, normal);
            t.SetStylebox("hover", type, hover);
            t.SetStylebox("pressed", type, pressed);
            t.SetStylebox("hover_pressed", type, pressed);
            t.SetStylebox("disabled", type, disabled);
            t.SetStylebox("focus", type, focus);
            t.SetColor("font_color", type, Text);
            t.SetColor("font_hover_color", type, new Color(1, 0.95f, 0.8f));
            t.SetColor("font_pressed_color", type, new Color(1, 0.9f, 0.6f));
            t.SetColor("font_hover_pressed_color", type, new Color(1, 0.9f, 0.6f));
            t.SetColor("font_disabled_color", type, Faint);
        }
        t.SetStylebox("normal", "LineEdit", Box(new Color(0.07f, 0.06f, 0.05f, 0.95f), new Color(0.5f, 0.4f, 0.24f), 2, 4, 8));
        t.SetStylebox("focus", "LineEdit", Box(new Color(0.07f, 0.06f, 0.05f, 0.95f), new Color(0.95f, 0.8f, 0.45f), 2, 4, 8));
        t.SetColor("font_color", "LineEdit", Text);
        t.SetColor("font_color", "Label", Text);
        t.SetStylebox("panel", "PanelContainer", PanelStyle());
        t.SetStylebox("background", "ProgressBar", Box(new Color(0.2f, 0.17f, 0.14f), new Color(0.4f, 0.32f, 0.2f), 1, 3, 2));
        t.SetStylebox("fill", "ProgressBar", Box(new Color(0.82f, 0.62f, 0.3f), new Color(0.9f, 0.72f, 0.4f), 1, 3, 2));
        t.SetColor("font_color", "ProgressBar", Text);
        return t;
    }

    public static Label Label(string text, int size = 18, Color? color = null, bool wrap = false, bool outline = false)
    {
        var l = new Label { Text = text, MouseFilter = Control.MouseFilterEnum.Ignore };
        l.AddThemeFontSizeOverride("font_size", size);
        l.AddThemeColorOverride("font_color", color ?? Text);
        if (outline)
        {
            l.AddThemeColorOverride("font_outline_color", new Color(0.05f, 0.04f, 0.03f, 0.9f));
            l.AddThemeConstantOverride("outline_size", 6);
        }
        if (wrap) l.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        return l;
    }

    public static Button Button(string text, Action onPressed, int size = 18, bool toggle = false)
    {
        var b = new Button { Text = text, ToggleMode = toggle, FocusMode = Control.FocusModeEnum.None };
        b.AddThemeFontSizeOverride("font_size", size);
        if (onPressed != null) b.Pressed += onPressed;
        return b;
    }

    /// <summary>Square colour swatch button.</summary>
    public static Button Swatch(Color c, Action onPressed, int px = 30)
    {
        var b = new Button { CustomMinimumSize = new Vector2(px, px), FocusMode = Control.FocusModeEnum.None, ToggleMode = true };
        var sb = Box(c, new Color(0.25f, 0.2f, 0.15f), 2, 4, 0);
        var sel = Box(c, new Color(1f, 0.9f, 0.55f), 3, 4, 0);
        b.AddThemeStyleboxOverride("normal", sb);
        b.AddThemeStyleboxOverride("hover", Box(c.Lightened(0.1f), new Color(0.8f, 0.65f, 0.35f), 2, 4, 0));
        b.AddThemeStyleboxOverride("pressed", sel);
        b.AddThemeStyleboxOverride("hover_pressed", sel);
        if (onPressed != null) b.Pressed += onPressed;
        return b;
    }

    public static Color Hex(int rgb) => new(((rgb >> 16) & 255) / 255f, ((rgb >> 8) & 255) / 255f, (rgb & 255) / 255f);

    public static Control Gap(float h) => new Control { CustomMinimumSize = new Vector2(0, h), MouseFilter = Control.MouseFilterEnum.Ignore };
}
