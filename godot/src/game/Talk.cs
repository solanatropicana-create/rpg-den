using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using FD.Life;
using FD.Rpg;
using FD.Sim.Life;
using FD.World;
using M = FD.Macro;

namespace FD.Game;

/// <summary>
/// Faz 2 G (Kenshi tarzı): what people say. Coming up to someone (E) they say one line made from their own state and the world's:
/// what happened to the camp, the hero's wounds and blood and empty purse, their own hunger and tiredness, the village's state in the sim
/// (festival, famine, plague, monsters), the realm at war, the ruler, their trade. Verbs on the card: <b>Söylenti sor</b> — a piece of
/// news the sim has (events near the village and camp, the realm's big ones, the world's wars and dragons), each told once;
/// <b>Saldır</b> — on a goblin, starts the fight.
/// </summary>
public partial class Talk : Node
{
    public static Talk Instance { get; private set; }
    Region _r;
    Session _s;
    readonly HashSet<int> _told = new();

    public void Init(Region r)
    {
        Instance = this;
        _r = r; _s = r.Session;
        Name = "Talk";
        r.Hud.Verbs.Insert(0, Verbs);
    }

    public override void _ExitTree() { if (Instance == this) Instance = null; }

    IEnumerable<(string, Action)> Verbs(Person p)
    {
        if (!p.Visible || p.Dead) yield break;
        if (p.Role == Role.Goblin) { yield return ("Saldır", () => { _r.Hud.CloseCardPublic(); _r.Combat.Start(p, $"{_r.Player.Character.Name} saldırdı!"); }); yield break; }
        yield return ("Söylenti sor", () => Rumor(p));
    }

    static float Hash(int a, int b, int c = 0) => H.Hash(a, b, c);

    /// <summary>One line from the person's and the world's state (deterministic for the person and the hour).</summary>
    public string Line(Person p)
    {
        var sim = _r.Life;
        var me = _r.Player.Character;
        if (p.Role == Role.Goblin) return Lines.Greeting(p, sim);
        if (p.Act?.Kind == ActKind.Flee) return "Goblinler! Kaç!";
        var c = new List<(float w, string t)>();
        var cp = _s.Camp; var vi = _s.Village(); var link = _s.Link;
        double day = _s.Macro.W.Day;
        string campShort = (cp?.Name ?? "Kırık Diş").Replace(" Kampı", "");
        // the camp
        if (cp != null && !cp.Alive && cp.ClearedDay is double cd && day - cd < 20)
        {
            bool byUs = _s.Macro.W.Events.Any(e => e.Kind == "lair" && e.Day >= cd - 0.5 && e.Text.Contains(_s.Player.Name) && e.Text.Contains("yerle bir"));
            c.Add((3f, byUs ? $"{campShort} goblinlerini sen mi temizledin? Sağ ol, yolcu!" : $"{campShort} kampı düşmüş; geceleri rahat uyuyoruz artık."));
        }
        else if (cp != null && cp.Alive && p.Role is Role.Farmer or Role.Shepherd or Role.Woodcutter or Role.Headman)
            c.Add((1.2f, p.Role == Role.Shepherd ? $"{campShort} goblinleri yine iki kuzumu kaçırdı." : $"{campShort} goblinleri yüzünden ormana tek başımıza giremiyoruz."));
        // the hero
        if (me.Hp < me.MaxHp / 2) c.Add((3f, p.Role == Role.Priest ? "Kan kaybediyorsun; gel, yaralarını sarayım." : "Kan kaybediyorsun! Tapınağa git, rahip yaraları sarar."));
        foreach (var w in me.Wounds)
        {
            if (Hash(p.Id, w.Kind.Length, (int)day) > 0.5f) continue;
            c.Add((1.6f, w.Kind switch
            {
                "eye" => "Gözüne ne oldu, yolcu? Kötü bir yara...",
                "limp" => "Topallıyorsun. Rahip ilaç bilir; ama pahalıdır.",
                _ => "O yüzündeki iz... goblin pençesi mi?",
            }));
        }
        if (_s.Flags.TryGetValue("robbedDay", out var rd) && day - rd < 3) c.Add((2f, "Goblinler seni de mi soydu? Ormanın dibi tekin değil."));
        if (me.Captive) c.Add((5f, "..."));
        if (link?.Reward > 0 && p.Role == Role.Innkeeper) c.Add((4f, $"Ödülün hazır, kahraman! {link.Reward:F0} altın."));
        if (p.Role == Role.Innkeeper && me.Inv.Silver < Services.RoomSilver) c.Add((1f, "Parasız yolcuya oda yok; ama bir maşrapa su ikram ederim."));
        // their own state
        if (p.Food < 0.25f) c.Add((1.2f, "Karnım zil çalıyor; daha akşam yemeğine çok var."));
        if (p.Energy < 0.2f) c.Add((1.2f, "Ayakta uyuyorum... bugün iş bitmek bilmedi."));
        if (p.Social < 0.2f && !p.IsVisitor) c.Add((1f, "Bugün kimseyle iki laf etmedim; iyi ki geldin."));
        // the village and the realm in the sim
        if (vi != null)
        {
            string st = vi.Status ?? vi.Crisis;
            string sl = st switch
            {
                "festival" => _r.Life.Mood == "festival" ? "Bu akşam meydanda ateş yakacaklar; biraz neşe iyi gelir." : "Festival dediler ama kimsenin içinden gelmiyor.",
                "prosper" => "Bu yıl bereketli; ambarlar dolu, yüzler gülüyor.",
                "boom" => "Tüccarlar akın akın geliyor; pazar hiç bu kadar kalabalık olmamıştı.",
                "shortage" or "hunger" => "Ambar boş, ekmek pahalı. Kış zor geçecek.",
                "plague" => "Salgın var; elini yüzünü yıka, yolcu.",
                "monsters" => "Canavarlar kol geziyor; kapıyı akşamdan sürgüle.",
                "migration" => "Bir sürü yabancı geldi; kimin kim olduğu belli değil.",
                "occupation" => "Yabancı askerler köyde; dilini tut.",
                "siege" => "Kuşatma var; yollar kesik.",
                _ => vi.StatusName != null ? $"Köyde {vi.StatusName.ToLowerInvariant()} var." : null,
            };
            if (sl != null) c.Add((2.2f, sl));
            if (vi.Ruler != null && Hash(p.Id, 7, (int)day) < 0.4f) c.Add((0.8f, Hash(p.Id, 9) < 0.5f ? $"{vi.RulerTitle} {vi.Ruler} bu yıl vergiyi yine artırdı." : $"{vi.RulerTitle} {vi.Ruler} uzakta; bizi hatırladığı da yok."));
            var civ = _s.Civ;
            if (civ != null && _s.Macro.InWar(civ)) c.Add((1.5f, $"{civ.Name} savaşta; gençleri askere yazıyorlar."));
            if (vi.Stability < 30) c.Add((1f, "Herkes huzursuz; ağızlar bozuk, kapılar kilitli."));
        }
        // their trade and the hour (the old lines)
        c.Add((1.5f, Lines.Greeting(p, sim)));
        float tot = c.Sum(x => x.w), r = Hash(p.Id, (int)(sim.Now / 30), 3) * tot;
        foreach (var (w, t) in c) { if ((r -= w) <= 0) return t; }
        return c[^1].t;
    }

    /// <summary>A piece of news (each told once per session): near the village and the camp, the realm's big ones, the world's.</summary>
    public void Rumor(Person p)
    {
        var a = _r.LifeWorld.ActorOf(p);
        var news = M.Local.News(_s.Macro, _s.Macro.W.Day - 60, 24);
        var e = news.FirstOrDefault(x => !_told.Contains(x.Id) && Hash(p.Id, x.Id) < (p.Role is Role.Innkeeper or Role.Merchant || p.Guest ? 0.9f : 0.5f))
                ?? news.FirstOrDefault(x => !_told.Contains(x.Id));
        if (e == null) { a?.Say("Bildiğim her şeyi anlattım; dünya bu ara sakin.", 4f); _r.Hud.Toast($"{p.Name}: \"Yeni bir şey duymadım.\"", 4f); return; }
        _told.Add(e.Id);
        int ago = (int)(_s.Macro.W.Day - e.Day);
        string when = ago <= 0 ? "bugün" : ago == 1 ? "dün" : $"{ago} gün önce";
        string lead = new[] { "Duydun mu?", "Derler ki", "Tüccarlar anlatıyor:", "Yolcular söylüyor:" }[(int)(Hash(p.Id, e.Id, 5) * 4) % 4];
        a?.Say($"{lead} {e.Text}", 7f);
        _r.Hud.Toast($"Söylenti ({when}): {e.Text}{(string.IsNullOrEmpty(e.Cause) ? "" : $" — {e.Cause}")}", 9f);
        p.Note($"{H.Clock(_r.Life.Now)} yolcuya söylenti anlattı");
    }
}
