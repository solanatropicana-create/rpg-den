using System;
using System.Collections.Generic;
using System.Linq;

// Yıllık sınıf yetenekleri (sınıf kimliği). Faz 1b-3: araştırma, çağlar ve alt sınıf doktrinleri kalktı; Research.cs'ten
// yalnız ağaca ve alt sınıfa bağlı olmayan yıllık sınıf yetenekleri buraya taşındı (sınıf tabanı ya da kademe ayrıcalığı):
// Rahip bayramı, Druid ormanı, Ozan festivali, Savaşçı turnuvası, Paktçı hediyesi, Kan Büyücüsü'nün yabani büyüsü.

namespace FD.Macro;

/// <summary>Bir yabani büyü dalgası sonucu (eski research.ts <c>Surge</c>).</summary>
public sealed class Surge
{
    public bool Good;
    /// <summary>TS <c>run: () =&gt; string</c>: applies the surge, returns its description.</summary>
    public Func<string> Run;
}

public static class Classes
{
    /// <summary>Yılda bir tetiklenen sınıf yetenekleri</summary>
    public static void ClassYearly(Sim s, Civ c)
    {
        var cap = s.Capital(c);
        if (cap == null) return;
        var contacts = J.Filter(s.W.Civs, o => o.Id != c.Id && o.Alive && s.Rel(c.Id, o.Id).Contact);
        // Rahip: Kanalize İlahiyat bayramı
        if (s.E(c, "festival") > 0)
        {
            s.Add(c, "grain", 40);
            foreach (var h in s.CivHeroes(c)) h.Hp = h.MaxHp;
            s.Log("class", $"{c.Name} Kanalize İlahiyat bayramını kutladı: ambarlar bereketlendi.", civ: c.Id, tile: cap.Tile);
        }
        // Druid: orman büyür (Kutsal Koru ayrıcalığı, Köy kademesinden)
        double fg = s.E(c, "forestGrow");
        if (fg > 0)
        {
            int grown = 0;
            foreach (var st in s.CivSettlements(c))
            {
                foreach (int ti in s.G.Within(st.Tile, s.RadiusOf(st)))
                {
                    if (grown >= fg * 2) break;
                    var t = s.W.Tiles[ti];
                    if (t.Owner != st.Id || t.Terrain != "grass" || t.Deposit >= 0 || t.Ext != null || ti == st.Tile) continue;
                    if (!J.Some(s.G.Neighbors(ti), n => s.W.Tiles[n].Terrain == "forest" || s.W.Tiles[n].Terrain == "oldforest")) continue;
                    t.Terrain = "forest"; t.Wood = 160; grown++;
                }
            }
            if (J.T(grown)) { s.ClearPaths(); s.Log("class", $"{c.Name} topraklarında orman {grown} hex genişledi.", civ: c.Id, tile: cap.Tile); }
        }
        // Ozan: şarkı festivali — altın, dostluk, göçmen
        double charm = s.E(c, "charm");
        if (charm > 0)
        {
            double gold = JsMath.Round(6 + contacts.Count * 4 * charm);
            s.Add(c, "gold", gold);
            foreach (var o in contacts) s.AddMod(o.Id, c.Id, "festival", "Ozan festivaline davet", 6 * charm, 18, 0.02, false);
            string came = "";
            if (contacts.Count > 0 && s.Pop(cap) < s.Housing(cap) + 1 && s.Rng.Chance(0.5 + charm * 0.2))
            {
                var o = s.Rng.Pick(contacts);
                double n = s.Rng.Int(1, 2);
                s.AddPop(cap, o.Race, n);
                came = $" {J.S(n)} {J.TrLower(D.RACES[o.Race].Name)} şarkılara kapılıp kaldı";
            }
            s.Log("class", $"{Tr.Ek(cap.Name, "da")} büyük ozan festivali: {J.S(gold)} altın toplandı{(J.T(came) ? "," + came : "")}.", civ: c.Id, tile: cap.Tile, cause: "İlham: komşular davetli");
        }
        // Savaşçı: yıllık turnuva (asker yazan medeniyet: Köy kademesinden)
        if (c.Cls == "fighter" && s.CivAt(c, Gate.TRAINING))
        {
            cap.Soldiers += 1;
            var hs = J.Filter(s.CivHeroes(c), h => h.State == "home");
            foreach (var h in hs) Heroes.GainXp(s, h, 150);
            s.Log("class", $"{Tr.Ek(cap.Name, "da")} lejyon turnuvası düzenlendi{(hs.Count > 0 ? $"; {string.Join(", ", J.Map(hs, h => h.Name))} şan kazandı" : "")}.", civ: c.Id, tile: cap.Tile, cause: "Aksiyon Dalgası: savaş sanatı sürekli bilenir");
        }
        // Paktçı: patronun hediyesi ve bedeli
        if (s.E(c, "pact") > 0)
        {
            s.Add(c, "gold", 25);
            double n = s.Pop(cap) > 40 && s.Rng.Chance(0.3) ? 2 : s.Pop(cap) > 20 ? 1 : 0;
            if (J.T(n)) s.RemovePop(cap, n);
            foreach (var o in contacts) if (o.Align.Good > 0.3) s.AddMod(o.Id, c.Id, "pact", "Karanlık pakt söylentileri", -6, -24, 0.01, false);
            s.Metric("pact");
            s.Log("class", $"{c.Name} patronundan hediyesini aldı: 25 altın.", civ: c.Id, tile: cap.Tile, major: true, cause: J.T(n) ? $"Bedeli: {J.S(n)} kişi bir gece gölgelere karışıp kayboldu" : "Bu yıl bedel ertelendi");
        }
        // Kan Büyücüsü: yabani büyü dalgası
        if (s.E(c, "wild") > 0) WildSurge(s, c, cap);
    }

    private static readonly string[] SURGE_RACES = { "tiefling", "dragonborn", "human" };

    private static void WildSurge(Sim s, Civ c, Settlement cap)
    {
        var surges = new List<Surge>
        {
            new Surge { Good = true, Run = () => { double g = s.Rng.Int(20, 45); s.Add(c, "gold", g); return $"gökten {J.S(g)} altın yağdı"; } },
            // Faz 1b-3: eskiden araştırma sıçrardı
            new Surge { Good = true, Run = () => { s.Add(c, "tools", 4); return "demirci ocaklarında kızıl bir alev yandı; örsten kendiliğinden dört alet döküldü"; } },
            new Surge { Good = true, Run = () => { if (s.Pop(cap) < 20) { s.Add(c, "grain", 30); return "ambarlar sıcak ekmekle doldu"; } cap.Soldiers += 3; return "alevlerin içinden üç savaşçı yürüyerek çıktı"; } },
            new Surge { Good = true, Run = () => { s.AddPop(cap, s.Rng.Pick(SURGE_RACES), 2); return "kızıl bir sisten iki yabancı belirdi ve kaldı"; } },
            new Surge { Good = false, Run = () => { double q = Math.Floor(s.St(c, "grain") * 0.25); s.Add(c, "grain", -q); return $"kontrolden çıkan alev ambarda {J.S(q)} tahılı kül etti"; } },
            new Surge { Good = false, Run = () => { if (s.Pop(cap) > 8) s.RemovePop(cap, 1); return "bir çırak kendi büyüsüne kurban gitti"; } },
            new Surge { Good = false, Run = () => { foreach (var h in s.CivHeroes(c)) h.Hp = JsMath.Max(1, Math.Floor(h.Hp / 2)); return "kahramanlar lanetli bir ateşle yaralandı"; } },
            new Surge { Good = true, Run = () =>
            {
                var cand = J.Filter(s.G.Within(cap.Tile, 3), t => s.W.Tiles[t].Terrain == "grass" && s.W.Tiles[t].Owner >= 0 && s.W.Tiles[t].Ext == null && t != cap.Tile);
                if (cand.Count == 0) { s.Add(c, "mana", 5); return "havada mana kıvılcımları uçuştu"; }
                int t = s.Rng.Pick(cand); s.W.Tiles[t].Terrain = "forest"; s.W.Tiles[t].Wood = 160; s.ClearPaths(); return "bir gecede kızıl yapraklı bir koru bitti";
            } },
        };
        var pick = s.Rng.Pick(surges);
        string what = pick.Run();
        s.Metric("wildSurge");
        s.Log("class", $"Yabani büyü dalgası {c.Name} topraklarını sardı: {what}.", civ: c.Id, tile: cap.Tile, major: true, cause: pick.Good ? "Kan Soyu: şans bu kez yüzlerine güldü" : "Kan Soyu: büyünün bedeli");
    }
}
