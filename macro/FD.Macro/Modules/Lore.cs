using System;
using System.Collections.Generic;
using System.Linq;

// Kahramanların kimliği ve hafızası (Faz 1, A3a): ad ve soy, kilometre taşları (Hero.Deeds) ve ölünce yazılan
// destan (Hero.Epitaph). Dünyanın hafızası kahramanlardır: ölen ölü kalır, kalan devam eder, bazıları efsane olur.
// Bütün metinler Türkçe; özel adların ekleri Lore.Ek ile (Tr.Ek; "Koyu 3", "Kampı II" gibi sayıyla biten adlarda
// ek sayının okunuşuna uyar), cins adlarınki ve savaş başlıklarınınki Ek0 ile (kesmesiz, ünsüz yumuşamalı) gelir.

namespace FD.Macro;

public static class Lore
{
    /// <summary>tam ad, yaşayan ya da son bu kadar gün içinde ölmüş bir kahramanınkiyle aynı olamaz (10 yıl)</summary>
    public const int NAME_MEMORY = 10 * Sim.YEAR;
    /// <summary>kilometre taşı tavanı; taşınca en eski küçük taş (kamp, ilan, savunma...) atılır</summary>
    public const int MAX_DEEDS = 80;
    /// <summary>soyundan gelinebilecek ata: efsane ya da en az efsanelik ününün yarısı</summary>
    public const double ANCESTOR_RENOWN = Heroes.LEGEND_RENOWN / 2;
    /// <summary>yeni kahramanın, aynı yuvada doğmuş ölü ya da emekli ünlü bir atanın soyadını taşıma olasılığı</summary>
    public const double LINEAGE_CHANCE = 0.3;

    // ------------------------------------------------------------ ad ve soy
    /// <summary>Bu tam ad yaşayan (ölmemiş) ya da son 10 yılda ölmüş bir kahramanda var mı.</summary>
    public static bool NameTaken(Sim s, string name)
    {
        var hs = s.W.Heroes;
        for (int i = 0; i < hs.Count; i++)
        {
            var o = hs[i];
            if (o.Name == name && (o.State != "dead" || (o.DeathDay ?? -1e9) >= s.Day - NAME_MEMORY)) return true;
        }
        return false;
    }

    /// <summary>RollName sonucu: ön ad, soyad ve (varsa) soyadını veren ata.</summary>
    public sealed class NameRoll
    {
        public string Given;
        public string Surname;
        public Hero Ancestor;
    }

    /// <summary>
    /// Irka göre ad ve soyad zarı. Aynı yuvada doğmuş, ölmüş ya da emekli ünlü bir kahraman varsa yeni gelen %30
    /// olasılıkla onun soyadını taşır (soyundandır). Tam ad yaşayanlarla ve son 10 yılda ölenlerle çakışmaz.
    /// </summary>
    public static NameRoll RollName(Sim s, string race, int birthBase)
    {
        var givens = D.HERO_NAMES[race];
        var surs = D.HERO_SURNAMES[race];
        var cands = J.Filter(s.W.Heroes, o => o.Race == race && o.Birth == birthBase && J.T(o.Surname) && (o.State == "dead" || o.State == "retired")
            && (J.T(o.Legend) || o.Renown >= ANCESTOR_RENOWN));
        Hero anc = cands.Count > 0 && s.Rng.Chance(LINEAGE_CHANCE) ? s.Rng.Pick(cands) : null;
        for (int k = 0; k < 12; k++)
        {
            if (k == 8) anc = null;   // atanın soyadıyla boş bir ad çıkmadı
            string g = s.Rng.Pick(givens);
            if (anc != null && g == anc.Given) continue;
            string sn = anc != null ? anc.Surname : s.Rng.Pick(surs);
            if (!NameTaken(s, g + " " + sn)) return new NameRoll { Given = g, Surname = sn, Ancestor = anc };
        }
        // zar tutmadıysa: rastgele bir yerden başlayıp sırayla ilk boş ad
        int gi0 = (int)s.Rng.Int(0, givens.Count - 1), si0 = (int)s.Rng.Int(0, surs.Count - 1);
        for (int i = 0; i < givens.Count; i++)
            for (int j = 0; j < surs.Count; j++)
            {
                string g = givens[(gi0 + i) % givens.Count], sn = surs[(si0 + j) % surs.Count];
                if (!NameTaken(s, g + " " + sn)) return new NameRoll { Given = g, Surname = sn };
            }
        // bütün adlar dolu (pratikte olmaz): soyada bir lakap eklenir
        string g2 = givens[gi0], s2 = surs[si0];
        foreach (var e in D.EPITHETS) if (!NameTaken(s, $"{g2} {s2} {e}")) return new NameRoll { Given = g2, Surname = $"{s2} {e}" };
        return new NameRoll { Given = g2, Surname = $"{s2} {J.S(s.W.Heroes.Count)}" };
    }

    // ------------------------------------------------------------ kilometre taşları
    private static readonly HashSet<string> MINOR = new() { "camp", "quest", "defend", "contract", "heal", "rob", "level" };

    /// <summary>Kahramana kilometre taşı ekler (taşınca en eski küçük taş atılır; doğum hep kalır).</summary>
    public static void Deed(Sim s, Hero h, string kind, string text, int? tile = null, string of = null)
    {
        h.Deeds ??= new List<HeroDeed>();
        h.Deeds.Add(new HeroDeed { Day = s.Day, Kind = kind, Text = text, Tile = tile ?? h.Pos, Of = of });
        if (h.Deeds.Count > MAX_DEEDS)
        {
            int i = J.FindIndex(h.Deeds, d => MINOR.Contains(d.Kind));
            h.Deeds.RemoveAt(i >= 0 ? i : 1);
        }
    }

    public static bool HasDeed(Hero h, string kind) => h.Deeds != null && J.Some(h.Deeds, d => d.Kind == kind);

    /// <summary>Doğum (ve soy) taşları; <paramref name="placeLoc"/>: "Karaköy'ün tavernasında" / "Kızıl Fener Hanı'nda".</summary>
    public static void Born(Sim s, Hero h, string placeLoc, Hero ancestor)
    {
        Deed(s, h, "birth", $"{placeLoc} {Ord(h.Level)} seviye bir {J.TrLower(D.RACES[h.Race].Name)} {J.TrLower(Sim.HeroClassTr(h.Cls))} olarak yola çıktı", h.Pos);
        if (ancestor != null)
            Deed(s, h, "lineage", $"{(J.T(ancestor.Legend) ? "efsanevi" : "ünlü")} {Ek(ancestor.Name, "in")} soyundandı", h.Pos, ancestor.Name);
    }

    // ------------------------------------------------------------ Türkçe yardımcılar
    private const string VOWELS = "aıoueiöüâîû";
    private static readonly string[] ORD = { "sıfırıncı", "birinci", "ikinci", "üçüncü", "dördüncü", "beşinci", "altıncı", "yedinci", "sekizinci", "dokuzuncu", "onuncu" };
    private static readonly string[] NUMW = { "hiç", "bir", "iki", "üç", "dört", "beş", "altı", "yedi", "sekiz", "dokuz", "on" };

    /// <summary>"3. yılın 41. gününde" (Faz 1b-3: mevsimler kalktı; geçici biçim, takvim sonraki adımda)</summary>
    public static string DateTr(double day) => $"{J.S(Math.Floor(day / Sim.YEAR) + 1)}. yılın {J.S(day % Sim.YEAR + 1)}. gününde";

    /// <summary>seviye sıra sayısı: 6 → "altıncı"</summary>
    public static string Ord(int lv) => lv >= 0 && lv <= 10 ? ORD[lv] : $"{lv}.";

    /// <summary>küçük sayılar yazıyla (bir … on), büyükler rakamla</summary>
    public static string Num(double n) => n >= 0 && n <= 10 && n == Math.Floor(n) ? NUMW[(int)n] : J.S(n);

    private static readonly string[] ONES = { "", "bir", "iki", "üç", "dört", "beş", "altı", "yedi", "sekiz", "dokuz" };
    private static readonly string[] TENS = { "", "on", "yirmi", "otuz", "kırk", "elli", "altmış", "yetmiş", "seksen", "doksan" };
    private static readonly string[] ROMAN = { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI", "XII", "XIII", "XIV", "XV", "XVI", "XVII", "XVIII", "XIX", "XX" };

    /// <summary>Sayının okunuşundaki son sözcük (ek ünlü uyumu için): 3 → "üç", 20 → "yirmi", 100 → "yüz".</summary>
    private static string SpokenTail(long n)
    {
        if (n % 10 != 0) return ONES[n % 10];
        if (n % 100 != 0) return TENS[(n / 10) % 10];
        if (n % 1000 != 0) return "yüz";
        return n == 0 ? "sıfır" : "bin";
    }

    /// <summary>Özel ad eki (Tr.Ek gibi, kesmeli); ad bir sayıyla bitiyorsa ("Kara Bayrak Koyu 3", "Paslıkılıç Kampı II")
    /// ek o sayının okunuşuna uyar: "Koyu 3'ü", "Kampı II'nin".</summary>
    public static string Ek(string word, string kind)
    {
        int sp = word.LastIndexOf(' ');
        string tail = sp >= 0 ? word.Substring(sp + 1) : null;
        long n = -1;
        if (tail != null)
        {
            if (tail.Length > 0 && tail.Length <= 6 && tail.All(char.IsDigit)) n = long.Parse(tail, System.Globalization.CultureInfo.InvariantCulture);
            else { int r = Array.IndexOf(ROMAN, tail); if (r >= 0) n = r + 1; }
        }
        if (n < 0) return Tr.Ek(word, kind);
        string spoken = SpokenTail(n);
        string e = Tr.Ek(spoken, kind);
        return word + e.Substring(spoken.Length);
    }

    /// <summary>bağlaç "da/de": son ünlüye göre</summary>
    public static string DaDe(string word)
    {
        string low = J.TrLower(word);
        for (int i = low.Length - 1; i >= 0; i--) if (VOWELS.IndexOf(low[i]) >= 0) return "aıouâû".IndexOf(low[i]) >= 0 ? "da" : "de";
        return "da";
    }

    /// <summary>yan cümle içindeki ad listesi: "A", "A ile B", "A, B ile C" (cümle bağlacı "ve" ile karışmasın)</summary>
    public static string JoinIle(List<string> xs) => xs.Count <= 1 ? string.Join("", xs) : $"{string.Join(", ", xs.Take(xs.Count - 1))} ile {xs[xs.Count - 1]}";

    /// <summary>"A", "A ve B", "A, B ve C"</summary>
    public static string JoinVe(List<string> xs) => xs.Count <= 1 ? string.Join("", xs) : $"{string.Join(", ", xs.Take(xs.Count - 1))} ve {xs[xs.Count - 1]}";

    /// <summary>Cins adın son ünsüzü ünlüyle başlayan ekten önce yumuşar: şeytancık → şeytancığ-, ağaç → ağac-, kitap → kitab-.</summary>
    private static string Soften(string word)
    {
        if (word.Length < 2) return word;
        int sp = word.LastIndexOf(' ');
        string tok = sp >= 0 ? word.Substring(sp + 1) : word;
        int vowels = 0;
        foreach (char ch in tok) if (VOWELS.IndexOf(ch) >= 0) vowels++;
        if (vowels < 2) return word;
        char last = word[word.Length - 1], prev = word[word.Length - 2];
        string head = word.Substring(0, word.Length - 1);
        if (last == 'k') return head + (prev == 'n' ? "g" : "ğ");
        if (last == 'ç') return head + "c";
        if (last == 'p') return head + "b";
        return word;
    }

    /// <summary>Kesmesiz ek (cins adlar, savaş başlıkları): "baskını" + da → "baskınında", "bir şeytancık" + in → "bir şeytancığın".</summary>
    public static string Ek0(string word, string kind)
    {
        // Faz 1 B2: ince l'li alıntı "trol" ince ünlü alır (trolün, trole, trolde); uyum "tröl" kökünden hesaplanır
        if (word.EndsWith("trol", StringComparison.Ordinal))
        {
            string alt = word.Substring(0, word.Length - 4) + "tröl";
            return word + Tr.Ek(alt, kind).Substring(alt.Length + 1);
        }
        string e = Tr.Ek(word, kind);                 // "word'ek" (ses uyumu yumuşamadan önceki kökten)
        string suffix = e.Substring(word.Length + 1);
        bool vowelStart = suffix.Length > 0 && VOWELS.IndexOf(suffix[0]) >= 0;
        return (vowelStart ? Soften(word) : word) + suffix;
    }

    /// <summary>Tamlayan (ilgi) hâli: son sözcüğü büyük harfle başlıyorsa özel ad (kesmeli), değilse cins ad.</summary>
    public static string Gen(string phrase)
    {
        int sp = phrase.LastIndexOf(' ');
        string last = sp >= 0 ? phrase.Substring(sp + 1) : phrase;
        return last.Length > 0 && char.IsUpper(last[0]) ? Ek(phrase, "in") : Ek0(phrase, "in");
    }

    /// <summary>Ek-fiilin geçmiş zamanı: "şan peşinde" → "şan peşindeydi", "kayıp kardeşini arıyor" → "…arıyordu".</summary>
    public static string PastCopula(string pred)
    {
        string low = J.TrLower(pred);
        char lv = 'e';
        for (int i = low.Length - 1; i >= 0; i--) if (VOWELS.IndexOf(low[i]) >= 0) { lv = low[i]; break; }
        string i4 = "aıâ".IndexOf(lv) >= 0 ? "ı" : "ouû".IndexOf(lv) >= 0 ? "u" : "öü".IndexOf(lv) >= 0 ? "ü" : "i";
        char end = low.Length > 0 ? low[low.Length - 1] : 'a';
        string buf = VOWELS.IndexOf(end) >= 0 ? "y" : "";
        string d = "fstkçşhp".IndexOf(end) >= 0 ? "t" : "d";
        return pred + buf + d + i4;
    }

    private static readonly string[] GENERIC_TITLES = { "Yol ", "Bugbear ", "Kervan ", "Düello", "Korsan " };

    /// <summary>Savaş başlığının bulunma hâli: "Kırıkdiş Kampı baskını" → "Kırıkdiş Kampı baskınında";
    /// yeri anmayan başlıklar "bir" ile ve küçük harfle: "Yol pususu" → "bir yol pususunda".</summary>
    public static string TitleLoc(string title)
    {
        if (string.IsNullOrEmpty(title)) return "bir çarpışmada";
        int par = title.IndexOf(" (", StringComparison.Ordinal);
        if (par > 0 && title.EndsWith(")", StringComparison.Ordinal)) return TitleLoc(title.Substring(0, par)) + title.Substring(par);
        return Ek0(IsGeneric(title) ? "bir " + J.TrLower(title.Substring(0, 1)) + title.Substring(1) : title, "da");
    }

    private static bool IsGeneric(string title)
    {
        foreach (var g in GENERIC_TITLES) if (title.StartsWith(g, StringComparison.Ordinal)) return true;
        return false;
    }

    /// <summary>Karonun anlatıdaki yeri (bulunma hâli): üstündeki yerleşim / han / kamp, yoksa en yakın yerleşimin
    /// "yakınlarında"sı (8 fersah içinde); hiçbiri yoksa null.</summary>
    public static string PlaceLoc(Sim s, int tile)
    {
        var w = s.W;
        var st = J.Find(w.Settlements, x => x.Alive && x.Tile == tile) ?? J.Find(w.Settlements, x => x.Tile == tile);
        if (st != null) return st.Alive ? Ek(st.Name, "da") : $"{st.Name} harabesinde";
        var inn = Will.InnById(s, w.Tiles[tile].Inn);
        if (inn != null) return inn.Alive ? Ek($"{inn.Name} Hanı", "da") : $"{inn.Name} Hanı yıkıntısında";
        var cp = J.Find(w.Camps, x => x.Alive && x.Tile == tile);
        if (cp != null) return Ek(cp.Name, "da");
        var near = NearSettlement(s, tile, 8);
        return near != null ? $"{near.Name} yakınlarında" : null;
    }

    private static Settlement NearSettlement(Sim s, int tile, double max)
    {
        Settlement best = null; double bd = max + 0.5;
        foreach (var x in s.W.Settlements) if (x.Alive) { double d = s.G.Dist(x.Tile, tile); if (d < bd) { bd = d; best = x; } }
        return best;
    }

    // ------------------------------------------------------------ ölüm
    /// <summary>
    /// Katil (yalın hâl): kahramansa adı; kamp önderiyse "Kırıkdiş Kampı'nın goblin şefi" (korsan koyunda kaptanın adı);
    /// sıradan savaşansa "Kırıkdiş Kampı'ndan bir goblin" / "Aslanburç Krallığı'ndan bir asker"; bilinmiyorsa düşman tarafın adı.
    /// <paramref name="foe"/>: karşı tarafın adı (kamp, yerleşim, medeniyet ya da kahraman; null olabilir).
    /// </summary>
    public static string KillerOf(Sim s, Combatant k, string foe)
    {
        if (k == null) return J.T(foe) ? foe : "düşmanları";
        if (k.Hero != null) return k.Hero.Name;
        if (k.Breath != null) return Dragon.Label(s);   // Faz 1 B2: "ejderha Alazkanat"
        string unit = J.TrLower(J.Or(k.Name, "düşman"));
        if (k.Kind == "boss")
        {
            var cp = J.T(foe) ? J.Find(s.W.Camps, c => c.Name == foe) : null;
            if (cp != null && cp.Kind == "pirate" && J.T(cp.Captain)) return $"Kaptan {cp.Captain}";
            return J.T(foe) ? $"{Ek(foe, "in")} {unit}" : unit;
        }
        return J.T(foe) ? $"{Ek(foe, "dan")} bir {unit}" : $"bir {unit}";
    }

    /// <summary>Ölüm yan cümlesi: "Kırıkdiş Kampı baskınında bir goblinin elinde can verdi" (başlık zaten karşı tarafı
    /// anıyorsa katilden "X'ten" kısmı düşer; yeri anmayan başlığa en yakın yerleşim eklenir).</summary>
    public static string DeathClause(Sim s, Hero h, Battle b, string foe)
    {
        string killer = h.Killer ?? "düşmanları";
        string hand = s.W.Dragon != null && killer == Dragon.Label(s) ? "pençesinde" : "elinde";   // Faz 1 B2
        if (hand == "pençesinde" && b != null && (b.Title ?? "").Contains("ejderha", StringComparison.Ordinal)) killer = s.W.Dragon.Name;   // "ejderha akınında ejderha X'in" tekrarı düşer
        if (b == null) return $"{PlaceLoc(s, h.Pos) ?? "uzak bir yerde"} {Gen(killer)} {hand} can verdi";
        string title = b.Title ?? "";
        if (J.T(foe) && title.Contains(foe, StringComparison.Ordinal))
        {
            // başlık karşı tarafı zaten anıyor: "Kırıkdiş Kampı baskınında Kırıkdiş Kampı'ndan bir goblin…" tekrarı düşer
            string from = Ek(foe, "dan") + " ", of = Ek(foe, "in") + " ";
            if (killer.StartsWith(from, StringComparison.Ordinal)) killer = killer.Substring(from.Length);
            else if (killer.StartsWith(of, StringComparison.Ordinal)) killer = (J.Some(s.W.Camps, c => c.Name == foe) ? "kampın " : "") + killer.Substring(of.Length);
            else if (killer == foe) killer = "düşmanları";
        }
        string where = TitleLoc(title);
        if (IsGeneric(title)) { var near = PlaceLoc(s, b.Tile); if (near != null) where = $"{near}, {where}"; }
        return $"{where} {Gen(killer)} {hand} can verdi";
    }

    /// <summary>Savaş dışında ölmüş (ör. suikast) ve destanı yazılmamış kahraman: yer, katil, taş ve destan.</summary>
    public static void LateDeath(Sim s, Hero h)
    {
        h.DeathTile ??= h.Pos;
        bool assassin = false;
        var evs = s.W.Events;
        for (int i = evs.Count - 1; i >= 0 && evs[i].Day >= (h.DeathDay ?? s.Day); i--)
            if (evs[i].Text != null && evs[i].Text.Contains(h.Name, StringComparison.Ordinal) && evs[i].Text.Contains("suikast", StringComparison.Ordinal)) { assassin = true; break; }
        h.Killer ??= assassin ? "bir suikastçı" : "bilinmeyen bir el";
        string clause = assassin ? "karanlık bir sokakta bir suikastçının hançeriyle can verdi" : $"{PlaceLoc(s, h.DeathTile.Value) ?? "uzak bir yerde"} sırrı çözülemeyen bir ölümle can verdi";
        s.Metric("heroDeath_" + (assassin ? "assassination" : "other"));
        Deed(s, h, "death", clause, h.DeathTile, h.Killer);
        WriteEpitaph(s, h);
    }

    // ------------------------------------------------------------ destan
    /// <summary>Destanı yazar (bir kez): Hero.Epitaph ve kalıcı kronikte büyük olay ("epitaph").</summary>
    public static void WriteEpitaph(Sim s, Hero h)
    {
        if (J.T(h.Epitaph)) return;
        h.Epitaph = Epitaph(s, h);
        s.Metric("epitaph");
        s.Log("epitaph", h.Epitaph, tile: h.DeathTile ?? h.Pos, civ: h.Civ >= 0 ? h.Civ : (int?)null, major: true, cause: $"{h.Name} için ozanların destanı");
    }

    private static (string Origin, string Drive) SplitBio(string bio)
    {
        if (!J.T(bio)) return (null, null);
        string b = bio.TrimEnd('.');
        int c = b.IndexOf(", ", StringComparison.Ordinal);
        return c < 0 ? (null, b) : (b.Substring(0, c), b.Substring(c + 2));
    }

    private static List<string> Distinct(List<HeroDeed> ds) { var o = new List<string>(); foreach (var d in ds) if (J.T(d.Of) && !o.Contains(d.Of)) o.Add(d.Of); return o; }

    /// <summary>Bir paragraflık destan: köken, öne çıkan işler, öldürmeler, efsanelik ve ölümün yeri, zamanı, katili.</summary>
    public static string Epitaph(Sim s, Hero h)
    {
        var deeds = h.Deeds ?? new List<HeroDeed>();
        List<HeroDeed> Of(string k) => J.Filter(deeds, d => d.Kind == k);
        var parts = new List<string>();

        // 1) köken
        var (origin, drive) = SplitBio(h.Bio);
        var birth = J.Find(deeds, d => d.Kind == "birth");
        var lineage = J.Find(deeds, d => d.Kind == "lineage");
        string head = J.T(origin) ? $"{J.TrCap(origin)} {h.Name}" : h.Name;
        string born = $"{DateTr(h.Born)} {birth?.Text ?? "yola çıktı"}";
        var tail = new List<string>();
        if (lineage != null) tail.Add(lineage.Text);
        if (h.BirthAge > 0) tail.Add($"{J.S(Math.Floor(h.BirthAge))} yaşındaydı");
        if (J.T(drive)) tail.Add(PastCopula(drive));
        parts.Add($"{head}, {born}{(tail.Count > 0 ? "; " + JoinVe(tail) : "")}.");

        // 2) öne çıkan işler (zaman sırasıyla, türlere göre toplanmış)
        var cl = new List<string>();
        var fb = J.Find(deeds, d => d.Kind == "firstblood");
        if (fb != null) cl.Add(fb.Text);
        var civs = Distinct(Of("contract"));
        if (civs.Count > 0) cl.Add($"{JoinIle(civs.Take(3).ToList())} saflarında {((h.Tally.Get("battles") ?? 0) > 0 ? "savaştı" : "hizmet etti")}");
        var camps = Distinct(Of("camp"));
        if (camps.Count == 1) cl.Add($"{Ek(camps[0], "i")} yerle bir etti");
        else if (camps.Count > 1 && camps.Count <= 3) cl.Add($"{JoinIle(camps.Take(camps.Count - 1).Append(Ek(camps[camps.Count - 1], "i")).ToList())} yerle bir etti");
        else if (camps.Count > 3) { string g1 = Ek(camps[1], "in"); cl.Add($"aralarında {camps[0]} ile {g1} {DaDe(g1)} bulunduğu {Num(camps.Count)} kampı yerle bir etti"); }
        var bosses = Of("boss");
        if (bosses.Count == 1) cl.Add(bosses[0].Text);
        else if (bosses.Count > 1) cl.Add($"{Num(bosses.Count)} kamp önderini kendi eliyle devirdi");
        foreach (var d in Of("dragon")) cl.Add(d.Text);   // Faz 1 B2: ejderhayı deviren ya da düştüğü savaşta bulunan
        var quests = Of("quest");
        if (quests.Count == 1) cl.Add(quests[0].Text);
        else if (quests.Count > 1) cl.Add($"{Num(quests.Count)} ilanı bitirip ödülünü aldı");
        foreach (var d in Of("duel")) cl.Add(d.Text);
        var defs = Distinct(Of("defend"));
        if (defs.Count == 1) cl.Add($"{Ek(defs[0], "i")} baskınlara karşı savundu");
        else if (defs.Count > 1) cl.Add($"{defs[0]} ile {Ek(defs[1], "i")} baskınlara karşı savundu");
        var heals = Distinct(Of("heal"));
        if (heals.Count == 1) cl.Add($"salgınlı {Ek(heals[0], "a")} şifa taşıdı");
        else if (heals.Count > 1) cl.Add($"salgınlı {heals[0]} ile {Ek(heals[1], "a")} şifa taşıdı");
        var relics = Of("relic");
        if (relics.Count == 1) cl.Add(relics[0].Text);
        else if (relics.Count > 1) cl.Add($"{Num(relics.Count)} harabeden eski büyülü silahlar çıkardı");
        var robs = Of("rob");
        if (robs.Count == 1) cl.Add(robs[0].Text);
        else if (robs.Count > 1) cl.Add($"{Num(robs.Count)} kervan soyup yolların korkusu oldu");
        foreach (var d in Of("epithet")) cl.Add(d.Text);
        foreach (var d in Of("revived")) cl.Add(d.Text);
        foreach (var d in Of("retired")) cl.Add(d.Text);
        for (int i = 0; i < cl.Count; i += 4)
        {
            var chunk = cl.Skip(i).Take(4).ToList();
            parts.Add(J.TrCap(JoinVe(chunk)) + ".");
        }

        // 3) öldürmeler, seviye, efsanelik
        bool grew = h.Level > Math.Max(1, h.BirthLevel);
        string tally = h.Kills > 0 && grew ? $"Ömrü boyunca {Num(h.Kills)} düşman devirip {Ord(h.Level)} seviyeye ulaştı"
            : h.Kills > 1 ? $"Ömrü boyunca {Num(h.Kills)} düşman devirdi"
            : grew ? $"{J.TrCap(Ord(h.Level))} seviyeye kadar yükseldi" : null;
        string legend = J.T(h.Legend) ? "ozanlar adını efsanelere yazdı" : null;
        if (tally != null) parts.Add($"{tally}{(legend != null ? "; " + legend : "")}.");
        else if (legend != null) parts.Add($"{J.TrCap(legend)}.");

        // 4) ölüm
        var death = J.Find(deeds, d => d.Kind == "death");
        double dday = h.DeathDay ?? s.Day;
        double years = Math.Floor((dday - h.Born) / Sim.YEAR);
        string how = death?.Text ?? "can verdi";
        double battles = h.Tally.Get("battles") ?? 0;
        if (death?.Of == "yaşlılık") parts.Add($"{J.TrCap(DateTr(dday))} {how}.");
        else if (battles <= 1 && h.Kills == 0 && J.T(h.Killer) && death != null && cl.Count == 0) parts.Add($"Daha ilk savaşında, {DateTr(dday)} {how}.");
        else if (years >= 2) parts.Add($"{J.TrCap(DateTr(dday))}, {J.S(years)} yıllık maceranın sonunda {how}.");
        else parts.Add($"{J.TrCap(DateTr(dday))} {how}.");
        return string.Join(" ", parts);
    }
}
