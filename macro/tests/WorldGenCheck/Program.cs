using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using FD.Macro;

namespace WorldGenCheck;

/// <summary>
/// WorldGen port check: prints Json.Serialize(WorldGen.GenerateWorld(seed)) per seed, one per line.
/// Seeds: "&lt;from&gt; &lt;to&gt;" (inclusive range) or one comma-separated list, like golden/ts/worldgen_dump.ts.
/// </summary>
public static class Program
{
    public static int Main(string[] args)
    {
        string a = args.Length > 0 ? args[0] : "1";
        var seeds = new List<double>();
        if (a.Contains(',')) foreach (var x in a.Split(',')) seeds.Add(double.Parse(x, CultureInfo.InvariantCulture));
        else for (double s = double.Parse(a, CultureInfo.InvariantCulture), to = args.Length > 1 ? double.Parse(args[1], CultureInfo.InvariantCulture) : s; s <= to; s++) seeds.Add(s);
        using var w = new StreamWriter(Console.OpenStandardOutput(), new UTF8Encoding(false));
        foreach (double seed in seeds) { w.Write(Json.Serialize(WorldGen.GenerateWorld(seed))); w.Write('\n'); }
        return 0;
    }
}
