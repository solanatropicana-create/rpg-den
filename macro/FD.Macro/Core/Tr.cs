using System.Collections.Generic;

namespace FD.Macro;

/// <summary>Turkish suffixes for proper names (vowel harmony, consonant assimilation, buffer letters). Port of src/sim/tr.ts.</summary>
public static class Tr
{
    private const string BACK = "aıou", VOWELS = "aıoueiöüâîû";
    private const string HARD = "fstkçşhp";
    private static readonly Dictionary<double, string> NUM = new() { [10] = "on", [25] = "yirmi beş", [50] = "elli", [100] = "yüz", [150] = "yüz elli", [200] = "iki yüz" };

    /// <summary>ek(word, kind) with kind one of "i", "a", "da", "dan", "in".</summary>
    public static string Ek(string word, string kind) => EkCore(word, word, kind);

    /// <summary>Number variant: suffix follows the spoken Turkish number when known.</summary>
    public static string Ek(double word, string kind)
    {
        string w = JsMath.Str(word);
        return EkCore(w, NUM.TryGetValue(word, out var b) ? b : w, kind);
    }

    private static bool Inc(string set, string s) => s != null && set.Contains(s, System.StringComparison.Ordinal);

    private static string EkCore(string w, string @base, string kind)
    {
        string lower = J.TrLower(@base);
        string last = "";
        for (int i = lower.Length - 1; i >= 0; i--) if (VOWELS.IndexOf(lower[i]) >= 0) { last = lower[i].ToString(); break; }
        // JS: 'aıou'.includes('') is true, so a word without vowels counts as back + round
        bool back = Inc(BACK, last) || last == "â" || last == "û";
        bool round = Inc("ouöüû", last);
        string endCh = lower.Length > 0 ? lower[lower.Length - 1].ToString() : null; // undefined for ""
        bool endsVowel = Inc(VOWELS, endCh);
        bool hard = Inc(HARD, endCh);
        // "Aslanburç Krallığı", "Kırıkdiş Kampı": compounds take the n buffer
        bool compound = HasSpace(@base.Trim()) && Inc("ıiuü", endCh);
        string buf = endsVowel ? (compound ? "n" : kind == "in" ? "n" : "y") : "";
        string i4 = back ? (round ? "u" : "ı") : round ? "ü" : "i";
        string a2 = back ? "a" : "e";
        string d = hard ? "t" : "d";
        string s = kind switch
        {
            "i" => buf + i4,
            "a" => buf + a2,
            "da" => (compound && endsVowel ? "n" + "d" : d) + a2,
            "dan" => (compound && endsVowel ? "n" + "d" : d) + a2 + "n",
            "in" => buf + i4 + "n",
            _ => "",
        };
        return $"{w}'{s}";
    }

    private static bool HasSpace(string s)
    {
        foreach (char c in s) if (char.IsWhiteSpace(c)) return true;
        return false;
    }
}
