using System;
using System.Collections.Generic;
using Godot;

namespace FD.UI;

/// <summary>
/// Tur 1 B: the mouse cursor says what a right click will do — a sword (attack), a mouth (talk), a hand (take, search, open,
/// use), a foot (go there). Drawn here from simple shapes (no image files), 32 px, pale with a dark outline.
/// </summary>
public static class CursorKit
{
    public enum Kind { Arrow, Foot, Sword, Mouth, Hand }
    static readonly Dictionary<Kind, (ImageTexture tex, Vector2 hot)> _cache = new();
    static Kind _current = Kind.Arrow;

    public static void Set(Kind k)
    {
        if (k == _current) return;
        _current = k;
        if (DisplayServer.GetName() == "headless") return;
        if (k == Kind.Arrow) { Input.SetCustomMouseCursor(null); return; }
        var (tex, hot) = Get(k);
        Input.SetCustomMouseCursor(tex, Input.CursorShape.Arrow, hot);
    }

    static (ImageTexture, Vector2) Get(Kind k)
    {
        if (_cache.TryGetValue(k, out var c)) return c;
        Func<float, float, bool> inside;
        Vector2 hot;
        Color fill;
        switch (k)
        {
            case Kind.Sword:
                fill = new Color(0.86f, 0.86f, 0.84f);
                hot = new Vector2(3, 3);
                inside = (x, y) =>
                    Capsule(x, y, 4, 4, 20, 20, 1.9f)                      // blade
                    || Capsule(x, y, 16, 24, 24, 16, 1.6f)                 // cross-guard
                    || Capsule(x, y, 21, 21, 26.5f, 26.5f, 1.7f)           // grip
                    || Circle(x, y, 27.5f, 27.5f, 2.3f);                   // pommel
                break;
            case Kind.Mouth:
                fill = new Color(0.95f, 0.93f, 0.86f);
                hot = new Vector2(4, 4);
                inside = (x, y) =>
                    (RoundRect(x, y, 3, 4, 29, 21, 5f) || Tri(x, y, 8, 19, 15, 19, 6, 28))
                    && !(Circle(x, y, 10, 12.5f, 1.8f) || Circle(x, y, 16, 12.5f, 1.8f) || Circle(x, y, 22, 12.5f, 1.8f));
                break;
            case Kind.Hand:
                fill = new Color(0.93f, 0.85f, 0.72f);
                hot = new Vector2(14, 3);
                inside = (x, y) =>
                    RoundRect(x, y, 8, 14, 24, 28, 4f)
                    || Capsule(x, y, 10, 15, 10, 6, 2.0f) || Capsule(x, y, 14, 14, 14, 3.5f, 2.0f)
                    || Capsule(x, y, 18, 14, 18, 4.5f, 2.0f) || Capsule(x, y, 22, 16, 22, 8, 1.9f)
                    || Capsule(x, y, 8.5f, 22, 3.5f, 15.5f, 2.0f);
                break;
            default: // foot
                fill = new Color(0.82f, 0.86f, 0.78f);
                hot = new Vector2(16, 16);
                inside = (x, y) =>
                    Ellipse(x, y, 16, 18, 6.2f, 8.5f) || Ellipse(x, y, 16.5f, 27.5f, 4.5f, 3.6f)
                    || Circle(x, y, 10.5f, 7.5f, 2.3f) || Circle(x, y, 14.5f, 5.5f, 2.0f) || Circle(x, y, 18.3f, 5.6f, 1.8f)
                    || Circle(x, y, 21.6f, 7.2f, 1.6f) || Circle(x, y, 23.8f, 9.8f, 1.4f);
                break;
        }
        var img = Image.CreateEmpty(32, 32, false, Image.Format.Rgba8);
        var outline = new Color(0.08f, 0.06f, 0.05f);
        for (int py = 0; py < 32; py++)
            for (int px = 0; px < 32; px++)
            {
                int inn = 0, near = 0;
                for (int sy = 0; sy < 4; sy++)
                    for (int sx = 0; sx < 4; sx++)
                    {
                        float x = px + (sx + 0.5f) / 4f, y = py + (sy + 0.5f) / 4f;
                        if (inside(x, y)) { inn++; near++; continue; }
                        bool n = false;
                        for (int a = 0; a < 8 && !n; a++)
                        {
                            float ang = a * MathF.PI / 4f;
                            if (inside(x + MathF.Cos(ang) * 1.6f, y + MathF.Sin(ang) * 1.6f)) n = true;
                        }
                        if (n) near++;
                    }
                float cov = inn / 16f, oc = near / 16f;
                if (oc <= 0) { img.SetPixel(px, py, new Color(0, 0, 0, 0)); continue; }
                var col = outline.Lerp(fill, cov / MathF.Max(oc, 1e-3f));
                img.SetPixel(px, py, new Color(col.R, col.G, col.B, MathF.Min(1f, oc * 1.1f)));
            }
        var r = (ImageTexture.CreateFromImage(img), hot);
        _cache[k] = r;
        return r;
    }

    static bool Circle(float x, float y, float cx, float cy, float r) => (x - cx) * (x - cx) + (y - cy) * (y - cy) <= r * r;
    static bool Ellipse(float x, float y, float cx, float cy, float rx, float ry) => (x - cx) * (x - cx) / (rx * rx) + (y - cy) * (y - cy) / (ry * ry) <= 1f;

    static bool Capsule(float x, float y, float ax, float ay, float bx, float by, float r)
    {
        float dx = bx - ax, dy = by - ay, L2 = dx * dx + dy * dy;
        float t = L2 > 0 ? Math.Clamp(((x - ax) * dx + (y - ay) * dy) / L2, 0f, 1f) : 0f;
        float qx = ax + dx * t - x, qy = ay + dy * t - y;
        return qx * qx + qy * qy <= r * r;
    }

    static bool RoundRect(float x, float y, float x0, float y0, float x1, float y1, float r)
    {
        float cx = Math.Clamp(x, x0 + r, x1 - r), cy = Math.Clamp(y, y0 + r, y1 - r);
        return x >= x0 && x <= x1 && y >= y0 && y <= y1 && (x - cx) * (x - cx) + (y - cy) * (y - cy) <= r * r;
    }

    static bool Tri(float x, float y, float ax, float ay, float bx, float by, float cx, float cy)
    {
        float d1 = (x - bx) * (ay - by) - (ax - bx) * (y - by);
        float d2 = (x - cx) * (by - cy) - (bx - cx) * (y - cy);
        float d3 = (x - ax) * (cy - ay) - (cx - ax) * (y - ay);
        bool neg = d1 < 0 || d2 < 0 || d3 < 0, pos = d1 > 0 || d2 > 0 || d3 > 0;
        return !(neg && pos);
    }
}
