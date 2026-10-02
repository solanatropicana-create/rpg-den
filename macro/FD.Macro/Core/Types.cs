using System.Collections.Generic;
using System.Text.Json.Serialization;

// Simulation state: exact mirror of src/sim/types.ts.
//
// Conventions (see macro/PORTING.md):
//  - TS field `foo` → C# field `Foo`; JSON/canonical name = C# name with the first letter lowered
//    (override with [JsonPropertyName] where TS differs, e.g. World.W/H).
//  - TS optional `foo?: T` → nullable C# (`double?`, `int?`, `bool?`, or a null reference).
//  - numbers: ids / tile indices / array indices / levels / tiers / eras are `int`; everything else
//    is `double` (JS numbers are doubles; never let C# integer division or truncation creep in).
//  - string unions stay strings (same literals as TS).
//  - Record<string, number> → JsObj<double>; Record<number, number> → JsNumObj<double>.

namespace FD.Macro;

public sealed class ExtractBuilding
{
    public string Kind;
    public int Level;
    public int Settlement;
    public double Workers;
    public bool? Depleted;
    public double? Burned;
    public double? BurnedAt;
}

public sealed class Tile
{
    public string Terrain;
    public double Elev;
    public int Owner;            // settlement id, -1 none
    public double Road;
    public int Deposit;          // deposit id, -1 none
    public double Reserve;
    public double Wood;
    public double? CutDay;
    public ExtractBuilding Ext;
    public int? Camp;
    public bool? Sea;
    public int? Isle;
    public int? Inn;
    public int? InnZone;
    public bool? Pass;

    /// <summary>TS <c>{ ...tile }</c> (shallow copy; Ext is shared like in JS).</summary>
    public Tile Clone() => (Tile)MemberwiseClone();
}

public sealed class Deposit
{
    public int Id;
    public string Kind;
    public List<int> Tiles = new();
    public double Richness;
    public bool Depleted;
    public List<int> KnownBy = new();
}

public sealed class Alignment
{
    public double Law;
    public double Good;
}

public sealed class Project
{
    public string Type;          // 'civic' | 'workshop' | 'extract' | 'upgrade' | 'ship'
    public string Kind;
    public int? Tile;
    public int? Level;
    public double Left;
    public double Total;
    /// <summary>Faz 1b-7: büyük proje (olay olarak gelen sınır kalesi, fener kulesi; Works)</summary>
    public bool? Grand;
}

public sealed class PlagueInfo
{
    public double Since;
    public double Until;
    public double Severity;
    public double Dead;
    public double? Potions;
}

public sealed class Settlement
{
    public int Id;
    public int Civ;
    public string Name;
    public int Tile;
    public double Founded;
    public JsObj<double> Pop = new();
    public double GrowthAcc;
    public JsObj<double> Civics = new();
    public JsObj<double> Workshops = new();
    public Project Project;
    public JsObj<double> Jobs = new();
    public double Soldiers;
    public bool Alive;
    public double Starving;
    public int Tier;             // 0 camp, 1 village, 2 town, 3 city (Faz 1b-3: yalnız nüfusla, eşikte histerezis; Sim.TierOf)
    /// <summary>Faz 1b-3: yerleşimin ulaştığı en yüksek kademe (yükselişi bir kez kroniğe yazmak için; null: 0)</summary>
    public int? PeakTier;
    public JsObj<double> MixedSince = new();
    public double? Hunger;
    public double? BurnedHouses;
    public double? BurnedAt;
    public PlagueInfo Plague;
    public double? PlagueImmune;
    public double? Graves;
    public bool? ShantyLog;
    // ---- sea
    public int? Port;
    public double? Ships;
    public double? Galleys;
    public bool? Overseas;
    // ---- yükseliş ve çöküş (Faz 1, B1)
    public int? Founder;          // kuran medeniyet (yerleşim ilk kez el değiştirdiğinde yazılır; null: hiç el değiştirmedi)
    public double? LostDay;       // son el değiştirdiği gün (fetih ya da bölünme)
    public int? ClaimBy;          // üzerinde tarihî hakkı olan medeniyet (kurucusu olarak kaybeden ya da elinden ayrılan)
    public double? ClaimUntil;    // hakkın düştüğü gün
    // ---- altın ve ambar (Faz 1, C3)
    /// <summary>kent tüketiminde yokluk: mal ("bread" | "beer" | "tools") → art arda yoksun geçen gün; null: yokluk yok</summary>
    public JsObj<double> Lack;
    /// <summary>imar (bayındırlık, 0–100): hazinenin kamu işleriyle (amele) yükselir, bakımsız kalınca yavaşça söner; büyüme, huzur ve
    /// onarım hızı verir (Economy.PublicWorks); null: hiç</summary>
    public double? Imar;
    /// <summary>imar ilk kez Economy.IMAR_LOG'a vardı ve kroniğe düştü</summary>
    public bool? ImarLog;
    // ---- büyük şehir (Faz 1b-4)
    /// <summary>art arda büyük şehir hücumları (iki hücum arası en çok Diplomacy.BIG_WEAR_DAYS; el değiştirince sıfırlanır): halkı yıpratır</summary>
    public double? Assaults;
    /// <summary>son büyük şehir hücumunun günü</summary>
    public double? LastAssault;
    // ---- inanç (Faz 1b-6)
    /// <summary>inanç dağılımı (sun, old, pact, none; toplam 1); null: henüz atanmadı (States.FaithTick)</summary>
    public JsObj<double> Faith;
    /// <summary>köleler ve hapis madenindeki mahkûmlar (nüfusun içinde; Bondage)</summary>
    public double? Slaves, Prisoners;
    // ---- durum tablosu ve istikrar (Faz 1b-7; Modules/Status.cs, Modules/Crisis.cs)
    /// <summary>yerleşimin durumu (Status.DEFS: prosper, boom, festival, found, shortage, plague, migration, newlord, monsters, siege,
    /// occupation, depleted ve kriz durumları feud, revolt, separatism, coup, mutiny, schism, challenge); null: olağan</summary>
    public string Status;
    /// <summary>durumun başladığı ve biteceği gün</summary>
    public double? StatusSince, StatusUntil;
    /// <summary>sonraki durum zarının günü (5–15 günde bir)</summary>
    public double? StatusRoll;
    /// <summary>istikrar 0–100 (Crisis.Stability; WORLD_DAYS'te bir hesaplanır)</summary>
    public double? Stability;
    /// <summary>süren iç kriz (feud, revolt, separatism, coup, mutiny, schism, challenge; Crisis.cs): 5–10 günlük belirtiler, sonunda
    /// şehir içeriden düşer ya da kriz bastırılır; null: yok</summary>
    public string Crisis;
    public double? CrisisSince, CrisisUntil;
    /// <summary>krizi besleyen örgüt (darbe girişimi; Org id) ve krizin sonundaki düşüş olasılığına ek (varissiz taht, örgüt)</summary>
    public int? CrisisOrg;
    public double? CrisisBonus;
    /// <summary>krizin başladığı gün yerleşimin sahibi (şehir el değiştirirse kriz söner)</summary>
    public int? CrisisCiv;
    /// <summary>son tehdit günü (baskın, kuşatma, akın, yağma): tepki inşaatı (sur) bu günden sonra gelir (Works)</summary>
    public double? Alarm;
    /// <summary>son yerel kıtlık durumunun bittiği gün (tepki inşaatı: yeni tarla)</summary>
    public double? ShortDay;
    /// <summary>içeriden düşüş sayısı (darbe, veraset savaşı, düello, mezhep…) ve sonuncusunun yolu ve uyarı süresi (gün)</summary>
    public int? Regimes;
    public string RegimeHow;
    public double? RegimeLead;
    /// <summary>fırsat merkezi (geçici halka; World.Hubs kimliği); null: kalıcı yerleşim</summary>
    public int? Hub;
    /// <summary>Faz 1b-8: temizlenen bir kampın vadisine kuruldu (kampın kimliği; öncüler ya da verimli vadi merkezi)</summary>
    public int? FromCamp;
}

/// <summary>Faz 1b-7: fırsat merkezi (yol haritası v3, geçici halka): söylenti → hücum → zirve → tükeniş → hayalet. Tetikleyiciler:
/// maden, savaş cephesi, antik harabe, kutsal kalıntı, yeni yol, temizlenmiş in (verimli vadi). Modules/Hubs.cs.</summary>
public sealed class Hub
{
    public int Id;
    /// <summary>mine | front | ruin | relic | road | valley</summary>
    public string Kind;
    public string Name;
    public int Tile;
    /// <summary>sahibi (yakındaki devlet); -1 yok</summary>
    public int Civ = -1;
    /// <summary>rumor | rush | peak | bust | ghost | done</summary>
    public string Phase;
    /// <summary>evrelerin başladığı günler (söylenti, hücum, zirve, tükeniş, hayalet) ve bitiş; -1: o evre yaşanmadı</summary>
    public double Rumor = -1, Rush = -1, Peak = -1, Bust = -1, Ghost = -1, End = -1;
    /// <summary>evrenin biteceği gün</summary>
    public double Until;
    /// <summary>merkezin yerleşimi (hücumda kurulur)</summary>
    public int? Settlement;
    /// <summary>zirvedeki en büyük nüfus</summary>
    public double PeakPop;
    /// <summary>sonu: ghost (terk, harabe) | village (kalıcı köy oldu) | gone (söylentide söndü)</summary>
    public string Outcome;
    /// <summary>tetikleyenin kimliği (savaş cephesi: saldıran devlet; vadi: kamp; harabe: eski yerleşim)</summary>
    public int? Origin;
    /// <summary>sahibine akan altın (toplam)</summary>
    public double Gold;
    /// <summary>savaş cephesi: saldırılan devlet (savaş sürdükçe pazar yaşar)</summary>
    public int? Front;
}

public sealed class RelMod
{
    public string Key;
    public string Text;
    public double Value;
    public double Decay;
}

public sealed class War
{
    public double Since;
    public int Attacker;
    public int Target;
    public double Attacks;
    public double LastArmy;
    public string Goal;
    public int? Ally;
    public string Kind;          // B1: null (sıradan) | 'reclaim' (tarihî hak) | 'pact' (savunma paktı) | 'crusade' (Kutsal Sefer)
}

public sealed class Relation
{
    public bool Contact;
    public List<RelMod> Mods = new();
    public War War;
    public string Treaty;
    /// <summary>Faz 1b-8 (#61): antlaşmanın imzalandığı ya da yenilendiği gün (süresi Diplomacy.TREATY_TERM)</summary>
    public double? TreatyDay;
    /// <summary>Faz 1b-8 (#61): sınırların (9 fersah) ilk değdiği gün; sınır sürtüşmesi barış uzadıkça birikir (Diplomacy.BorderFriction)</summary>
    public double? BorderSince;
    public JsObj<double> Tension = new();
    public double LastTalk;
    public double LastRaid;
    public double? Land;
    public double? PeaceDay;
    public double? SeaTry;
    public double? Pact;         // B1: savunma paktı (imza günü; iki yönde de yazılır)
}

public sealed class CivStats
{
    public double PeakPop;
    public double BattlesWon;
    public double BattlesLost;
    public double Traded;
    public JsObj<double> Mined = new();
    public double Depleted;
}

public sealed class HistPoint
{
    public double Day;
    public double Pop;
    public double Gold;
    /// <summary>Faz 1b-3: medeniyetin kademesi (en büyük yerleşiminin; eskiden bilinen düğüm sayısı)</summary>
    public double Tier;
}

public sealed class GearState
{
    public double Ench;
    public double Mith;
    public double N;
}

public sealed class Civ
{
    public int Id;
    /// <summary>Faz 1b-6: hükümet tipi (kingdom | clans | republic | theocracy; Polity.GOVS). Eski sınıf (Cls) kalktı.</summary>
    public string Gov;
    public string Name;
    /// <summary>Faz 1b-6: devletin kültürü = kuruluştaki çoğunluk ırkı (Polity.CULTURES)</summary>
    public string Race;
    /// <summary>Faz 1b-6: hizalama yöneticiden ve yasadan hesaplanır (States.Realign)</summary>
    public Alignment Align;
    public string Color;
    public JsObj<double> Stock = new();
    public JsObj<double> Price = new();
    public JsObj<double> Want = new();
    /// <summary>etkiler: sınıf tabanı + kademeye bağlı sınıf ayrıcalıkları ve medeniyet etkileri (Sim.RecomputeEff)</summary>
    public JsObj<double> Eff = new();
    public bool Alive;
    public double Founded;
    public double Threat;
    public double LastRaidedDay;
    public bool ScoutSent;
    public CivStats Stats;
    public double LastExpand;
    public JsObj<double> Yearly = new();
    public double? ExtinctDay;
    public bool? Respawned;
    public List<HistPoint> History = new();
    public double? LastWarEnd;
    public double? InnBanUntil;
    public bool? SeaScout;
    public GearState Gear;
    // ---- yükseliş ve çöküş (Faz 1, B1)
    public int? Parent;                  // ayrılarak doğduğu medeniyet
    public double? LastSecession;        // son bölünme günü (bir yerleşimi ayrıldı ya da kendisi bölünmeyle doğdu)
    public double? CapitalLostDay;       // başkentini son kaybettiği gün
    public int? CapitalLosses;           // Faz 1b-4: başkentini kaç kez kaybetti (fetihle; Diplomacy.CapitalFell)
    public int? Seat;                    // Faz 1b-4: dünkü gün sonunun başkenti (Sim.Step; en kalabalık yerleşim gün içinde değişebilir)
    public string FallCause;             // son yerleşimini neden kaybetti (yok oluşun kroniğe düşen nedeni)
    public double? CrusadeDay;           // kendisine karşı son Kutsal Sefer çağrısının günü
    // ---- altın ve ambar (Faz 1, C3)
    /// <summary>süren kıtlık (null: yok); bkz. <see cref="FamineState"/></summary>
    public FamineState Famine;
    /// <summary>hazinenin bakım giderlerini karşılayamadığı ilk gün (null: hazine ödüyor)</summary>
    public double? Broke;
    /// <summary>son günün bütçesi (gelir ve bakım gideri, altın/gün); yapay zekâ ve gösterim için</summary>
    public CivBudget Budget;
    // ---- devlet (Faz 1b-6; claude/devlet-orgut-spec.md §2)
    /// <summary>ad kökü ("Güneştacı" → "Güneştacı Krallığı")</summary>
    public string Stem;
    /// <summary>yönetici ve varis (Person id; W.People)</summary>
    public int? Ruler, Heir;
    /// <summary>meşruiyet 0–100</summary>
    public double Legit;
    public LawState Law;
    /// <summary>hanedan (krallık)</summary>
    public string House;
    /// <summary>son seçimin günü (cumhuriyet)</summary>
    public double? Elected;
}

public sealed class HeroGoal
{
    public string Kind;          // GoalKind
    public int Tile;
    public int? Target;
    public string Text;
    public double Since;
    public double? Stay;
}

public sealed class HeroContract
{
    public int Civ;
    public double Since;
    public double Until;
    public double Paid;
}

public sealed class Bid
{
    public int Civ;
    public double Gold;
}

public sealed class Auction
{
    public double End;
    public List<Bid> Bids = new();
}

public sealed class HeroBonus
{
    public double Atk;
}

public sealed class JournalEntry
{
    public double Day;
    public string Text;
}

public sealed class Hero
{
    public int Id;
    public string Name;
    public string Race;
    public string Cls;
    public int Level;
    public double Xp;
    public JsObj<double> Stats = new();
    public double MaxHp;
    public double Hp;
    public double Ac;
    public int Civ;
    public int Pos;
    public int Tavern;
    public string State;         // 'tavern' | 'traveling' | 'home' | 'quest' | 'dead' | 'gone' | 'army' | 'retired'
    public double Born;
    public double IdleSince;
    public double Kills;
    public double Gold;
    public string Bio;
    public double? DeathDay;
    public bool? Revived;
    // ---- hero will
    public string Align;
    public string Path;
    public List<string> Traits = new();
    public string Epithet;
    public JsObj<double> Tally = new();
    public HeroBonus Bonus;
    public JsNumObj<double> Rep = new();
    public List<JournalEntry> Journal = new();
    public int Base;
    public bool BaseInn;
    public int Birth;
    public HeroGoal Goal;
    public HeroContract Contract;
    public Auction Auction;
    public int? Vendetta;
    public int? Grudge;
    public double? SoloUntil;
    public List<int> Seen;
    public double? Hired;
    public bool? Legend;
    public double? LastGoal;
    public double? BaseDay;              // Faz 1b-5: bu yuvaya (han ya da taverna) geldiği gün; null = doğduğu gün (han havuzunun dönüşümü)
    // ---- kimlik ve ilerleme (Faz 1, A3a): Name = "Given Surname"
    public string Given;                 // ön ad
    public string Surname;               // soyad ya da lakap (ırka göre havuzdan ya da atadan)
    public int? Lineage;                 // soyundan geldiği kahramanın id'si
    public int BirthLevel;               // doğuş seviyesi
    public double BirthAge;              // yola çıktığı yaş (yıl); yaş = BirthAge + (gün − Born) / Sim.YEAR (40)
    public double Renown;                // ün: efsanelik seviyeden bağımsız, ünle gelir
    public List<HeroDeed> Deeds = new(); // kilometre taşları (destanın malzemesi)
    public string Epitaph;               // ölünce yazılan destan (kronikteki metnin aynısı)
    public int? DeathTile;               // öldüğü karo
    public string Killer;                // katili (yalın hâl: "Kırıkdiş Kampı'ndan bir goblin")
    // ---- altın ve ambar (Faz 1, C3)
    public double? Unpaid;               // maaşı art arda ödenmeyen mevsim (Economy.PayHeroes); null: maaşı ödeniyor
    // ---- inanç ve örgüt (Faz 1b-6)
    public string Faith;                 // sun | old | pact | none
    public List<Membership> Orgs;        // örgüt üyelikleri (null: yok)
    public int? CaptiveAt;               // esir tutulduğu yerleşim (State "captive"; Bondage)
    public double? CaptiveUntil;         // mahkûmiyetin bittiği gün (köleliğin süresi yok)
    public string CaptiveKind;           // slave | prison
}

/// <summary>Kahramanın kilometre taşı (destanda kullanılır).</summary>
public sealed class HeroDeed
{
    public double Day;
    public string Kind;          // birth | lineage | firstblood | nat20 | contract | camp | boss | quest | duel | defend | heal | relic | rob | epithet | level | legend | revived | retired | death
    public string Text;          // yüklemli yan cümle, öznesiz, geçmiş zaman: "Kırıkdiş Kampı'nı yerle bir etti"
    public int? Tile;
    public string Of;            // ilgili ad (medeniyet, kamp, yerleşim, rakip): destanda toplamak için
}

public sealed class Guest
{
    public int Id;
    public string Kind;          // GuestKind
    public string Name;
    public string Race;
    public double N;
    public int Civ;
    public int From;
    public string FromName;
    public int To;
    public string ToName;
    public string Why;
    public double Purse;
    public double Spent;
    public double Nights;
    public double Arrived;
    public int? Hero;
    public int? Agent;
    public bool? Stable;
    public double Mood;
}

public sealed class InnStaff
{
    public string Name;
    public string Race;
    public string Role;          // StaffRole
    public double Since;
    public string From;
}

public sealed class InnLog
{
    public double Day;
    public string K;             // 'in' | 'out' | 'buy' | 'build' | 'staff' | 'ev' | 'no'
    public string T;
    public double? G;
}

public sealed class InnBook
{
    /// <summary>hesap dönemi (InnLife.BOOK_DAYS günlük; gün / BOOK_DAYS, aşağı yuvarlanmış)</summary>
    public double Period;
    public double Room;
    public double Food;
    public double Ale;
    public double Other;
    public double Supply;
    public double Wage;
    public double Build;
    public double Guests;
    public double Nights;
}

public sealed class InnBuild
{
    public int Level;
    public double Work;
    public double Need;
    public double Wood;
    public double WoodNeed;
    public double Stone;
    public double StoneNeed;
    public double Started;
    public bool? Rebuild;
}

public sealed class InnStock
{
    public double Food;
    public double Ale;
    public double Wood;
}

public sealed class InnHist
{
    public double Day;
    public double Guests;
    public double Gold;
    public double Fame;
}

public sealed class InnOrder
{
    public int Agent;
    public string FromName;
    public string Goods;
    public double Cost;
    public double Day;
}

public sealed class InnTotal
{
    public double Guests;
    public double Nights;
    public double Income;
    public double Turned;
    public double Bought;
}

public sealed class Inn
{
    public int Id;
    public int Tile;
    public string Name;
    public string Keeper;
    public double Founded;
    public bool Alive;
    public double? RuinedDay;
    public double Gold;
    public int? Teacher;
    public double Raids;
    // ---- living inn
    public string Stage;         // 'road' | 'build' | 'open' | 'ruin'
    public int Level;            // 1 road inn · 2 inn · 3 caravanserai
    public string KeeperRace;
    public int Origin;
    public string OriginName;
    public InnBuild Build;
    public InnStock Stock;
    public double Fame;
    public List<InnStaff> Staff = new();
    public List<Guest> Guests = new();
    public JsNumObj<double> Tabs = new();
    public List<InnLog> Log = new();
    public List<InnBook> Books = new();
    public List<InnHist> Hist = new();
    public InnOrder Order;
    public InnTotal Total;
    public double Turned;
    public double Sat;
    public double Traffic;
}

public sealed class Camp
{
    public int Id;
    public string Kind;          // 'goblin' | 'hobgoblin' | 'bugbear' | 'pirate'
    public int Tile;
    public string Name;
    public double Count;
    public bool Boss;
    public bool HadBoss;
    public double Loot;
    public double GrowthAcc;
    public bool Alive;
    public double NextRaid;
    public double Founded;
    public double? ClearedDay;
    public string Captain;       // korsan kaptanı; ejderha kampında ejderhanın adı (Faz 1 B2)
    /// <summary>Faz 1 B2: gizli in: yeni kurulan in ilk akınına, bir kâşifin onu görmesine ya da söylentiler yayılana (1 yıl) dek
    /// bilinmez; kahramanlar, hanlar ve medeniyetler onu hedef alamaz.</summary>
    public bool? Hidden;
    /// <summary>Faz 1b-6: aç haydut kampı (Kind "bandit") hâlâ aç mı (yiyecek ister); ganimetle dönünce tok olur</summary>
    public bool? Hungry;
    /// <summary>Faz 1b-6: haydutların kaçtığı yerleşim</summary>
    public int? Home;
}

public sealed class IsleInfo
{
    public int Id;
    public string Name;
    public string Kind;          // IsleKind
    public double Size;
    public int Center;
    public int? Peak;
}

public sealed class Quest
{
    public int Id;
    public int Civ;
    public int Camp;
    public double Bounty;
    public double Posted;
    public List<int> TakenBy = new();
    public bool Open;
    public int? Inn;
    public double? Expires;
    public double? Failures;
    public double? Done;
    public double? Topped;
    /// <summary>Faz 1b-6: ilanı asan örgüt (Avcılar Locası'nın ödülü; ödül örgütün kasasından); null: medeniyet ya da han</summary>
    public int? Org;
}

public sealed class Muster
{
    public double Since;
    public double Until;
}

public sealed class Agent
{
    public int Id;
    public string Kind;          // AgentKind
    public int Civ;
    public List<int> Path = new();
    public int Step;
    public double Progress;
    public double Speed;
    public double? Troops;
    public List<int> Heroes;
    public JsObj<double> Cargo;
    public bool? Horse;
    public int? From;
    public int? To;
    public string Purpose;
    public int? Route;
    public JsObj<double> Pop;
    public int? TargetTile;
    public int? Quest;
    public bool? Returning;
    public bool? Dead;
    public double? Loot;
    public int? Chase;
    public int? Home;
    public bool? Boss;
    public string Monster;
    // ---- inns
    public Guest Guest;
    public int? Inn;
    public double? RestUntil;
    public List<int> RestedAt;
    // ---- sea
    public int? Hull;
    public double? Galleys;
    public int? Landing;
    public List<int> Fought;
    // ---- joint attack
    public Muster Muster;
    /// <summary>Faz 1b-8: öncüler temizlenen bir kampın vadisine gidiyor (kampın kimliği)</summary>
    public int? FromCamp;
}

public sealed class TradeRoute
{
    public int Id;
    public int A;
    public int B;
    public string Kind;          // 'trade' | 'treaty'
    public string Good;
    public List<int> Path = new();
    public double NextDepart;
    public double Trips;
    public bool Alive;
    public double Since;
    public bool? Sea;
}

public sealed class BattleLine
{
    public string T;
    public bool? Crit;
    public bool? Fumble;
}

public sealed class ReplayUnit
{
    public string N;
    public string S;             // 'A' | 'B'
    public string K;
    public double Hp;
    public double Max;
    public double Ac;
    public double Atk;
    public List<double> Dmg;     // [n, sides, bonus]
    public double Att;
    public string Cls;
    public double? Lvl;
    public bool? Boss;
    public double G;
    public string Race;
}

public sealed class ReplayPart
{
    public string L;
    public double V;
    public List<double> Dd;
    public double? Ds;
}

public sealed class ReplayEv
{
    public double R;
    public string Sp;
    public int? A;
    public int? T;
    public double? D;
    public double? M;
    public double? Ac;
    public int? H;
    public List<double> Dd;
    public double? Ds;
    public double? B;
    public List<ReplayPart> X;
    public double? Mul;
    public bool? Half;
    public double? V;
    public double? Hp;
    public List<int> Ts;
    public List<double> Vs;
    public List<bool> Sv;
    public List<List<double>> Dds;
    [JsonPropertyName("aA")] public double? AA;
    [JsonPropertyName("aB")] public double? AB;
}

public sealed class ReplayGroup
{
    public string Name;
    public string Side;
    public int? Civ;
    public string Kind;
}

public sealed class Replay
{
    public List<ReplayUnit> Units = new();
    public List<ReplayEv> Ev = new();
    public List<ReplayGroup> Groups = new();
    public double MoraleA;
    public double MoraleB;
    public double HeroMorale;
    public bool? NoRoutA;
    public bool? NoRoutB;
    public double PowA;
    public double PowB;
    public double Rounds;
    public double MaxRounds;
    public string End;           // 'wipe' | 'rout' | 'timeout'
    public string Routed;
    public string TimeoutWinner;
}

public sealed class BattleRoll
{
    public double D20;
    public string Side;
    public string Who;
}

public sealed class Battle
{
    public int Id;
    public double Day;
    public int Tile;
    public string Title;
    public string SideA;
    public string SideB;
    public string Winner;        // 'A' | 'B'
    public bool? Naval;
    public double LossesA;
    public double LossesB;
    public List<BattleLine> Lines = new();
    public List<BattleRoll> Rolls = new();
    public int? CivA;
    public int? CivB;
    public bool? Joint;
    public Replay Replay;
}

public sealed class GameEvent
{
    public int Id;
    public double Day;
    public string Kind;          // EventKind
    public string Text;
    public string Cause;
    public int? Civ;
    public int? Tile;
    public int? Battle;
    public bool? Major;
}

public sealed class InnPlan
{
    public double Target;
    public double Wave;
    public double Next;
}

public sealed class World
{
    public double Seed;
    public int Day;
    [JsonPropertyName("W")] public int Width;
    [JsonPropertyName("H")] public int Height;
    public List<Tile> Tiles = new();
    public List<Deposit> Deposits = new();
    public List<Civ> Civs = new();
    public List<Settlement> Settlements = new();
    public List<Hero> Heroes = new();
    public List<Camp> Camps = new();
    public List<Inn> Inns = new();
    public InnPlan InnPlan;
    public List<Quest> Quests = new();
    public List<Agent> Agents = new();
    public List<TradeRoute> Routes = new();
    public List<List<Relation>> Relations = new();
    public List<GameEvent> Events = new();
    /// <summary>Kalıcı kronik: her büyük olay (Major == true) kaydedildiği anda buraya da eklenir ve hiç kırpılmaz
    /// (<see cref="Events"/> 2500 kayıtta kesilir). Aynı GameEvent nesneleri iki listede paylaşılır.</summary>
    public List<GameEvent> Chronicle = new();
    public List<Battle> Battles = new();
    public int NextId;
    public double RngState;
    public JsObj<double> Metrics = new();
    public List<IsleInfo> Isles;
    public string SeaProfile;    // 'kita' | 'takimada' | 'buyuk'
    // ---- anlatıcı ve geç tehdit (Faz 1, B2): ilk günde kurulur (Storyteller.Tick)
    public StoryState Story;
    public DragonState Dragon;
    // ---- devlet, inanç ve örgüt (Faz 1b-6)
    /// <summary>tarih öncesinin bittiği gün (Sim.PREHISTORY_DAYS): oyunun ve ölçümün başladığı gün; örgütler bu gün kurulur</summary>
    public int Epoch;
    /// <summary>önemli kişiler (yöneticiler, varisler, örgüt liderleri, şube ustaları)</summary>
    public List<Person> People = new();
    public List<Org> Orgs = new();
    /// <summary>dünyanın yerleşim tavanı: tarih öncesinin sonunda yaşayan yerleşim sayısı (0: tarih öncesi, yerleşilebilir alanla; Diplomacy.WorldCap)</summary>
    public int SettleCap;
    // ---- Faz 1b-7
    /// <summary>fırsat merkezleri (geçici halka; bitenler de kalır)</summary>
    public List<Hub> Hubs = new();
    /// <summary>sonraki fırsat merkezi tetikleyicisinin en erken günü</summary>
    public double NextHub;
    // ---- Faz 2: 1:1 bölge (Godot dilimi; Modules/Local.cs)
    /// <summary>oyuncunun bölgesi: bağlı köy, han ve goblin kampı, oyuncunun kahraman kaydı; null: başsız (ölçüm) dünya</summary>
    public RegionLink Region;
}

/// <summary>Faz 2: 1:1 bölgenin (Godot dilimi) makro dünyadaki karşılığı (<see cref="Local"/>). Bölgedeki köy, han ve kamp simdeki
/// birer yerleşime, hana ve kampa bağlıdır: adları, devleti, nüfusu, iş ve inanç dağılımı, fiyatlar, ilanlar ve kahramanlar simden
/// gelir; bölgede olanlar (kamp temizlendi, goblin öldü, alışveriş, oyuncu bayıldı…) sime olay olarak yazılır.</summary>
public sealed class RegionLink
{
    /// <summary>bağlı köy (Settlement id)</summary>
    public int Village;
    /// <summary>bağlı han (Inn id; -1: dünyada han yok)</summary>
    public int Inn = -1;
    /// <summary>bağlı goblin kampı (Camp id)</summary>
    public int Camp;
    /// <summary>bağlandığı gün</summary>
    public double Since;
    /// <summary>kamp bağlanırken bölgeye yakın kuruldu mu (dünyada yakında goblin kampı yoktu)</summary>
    public bool SpawnedCamp;
    /// <summary>oyuncunun kahraman kaydı (Hero id; State "player"); null: henüz yaratılmadı</summary>
    public int? Player;
    /// <summary>oyuncunun ekibindeki kahramanlar (Hero id; State "party")</summary>
    public List<int> Party = new();
    /// <summary>Faz 2: bölgede kazanılıp henüz alınmamış ilan ödülü (altın; handa/muhtarda ödenir)</summary>
    public double Reward;
    /// <summary>Faz 2: temizlenen kampın ganimeti (altın değeri; kamptaki sandıkta bulunur)</summary>
    public double CampLoot;
    /// <summary>Faz 2: kamp temizlenince boşalan vadiye yerleşimcilerin geleceği gün (null: yok)</summary>
    public double? SettlersDay;
}

// ---- Faz 1, B2: anlatıcı (gerilim bütçesi, kriz ve rahatlama) ve ejderha

/// <summary>Anlatıcının durumu: 2 yıllık kayan pencerede gerilim (ölüm, yangın, kıtlık, baskın), kriz ve rahatlama takvimi.</summary>
public sealed class StoryState
{
    /// <summary>kapanmış gerilim kovaları (her biri 10 gün; son 24 kova = 2 yıl)</summary>
    public List<double> Buckets = new();
    /// <summary>açık (bu 10 günün) kovası</summary>
    public double Bucket;
    /// <summary>izlenen W.Metrics sayaçlarının son okunan değerleri (gerilim farkla ölçülür)</summary>
    public JsObj<double> Seen = new();
    /// <summary>gerilime katılmış son muharebenin id'si</summary>
    public int LastBattle = -1;
    /// <summary>son ölçülen pencere toplamı (2 yıl)</summary>
    public double Tension;
    /// <summary>art arda sakin geçen gün</summary>
    public double CalmDays;
    public double LastCrisis = -9999;
    public string LastKind;
    public double Crises;
    /// <summary>zirveden sonraki rahatlama döneminin sonu (yeni kriz yok)</summary>
    public double ReliefUntil;
    /// <summary>rahatlama olayının (bereket ya da şenlik) günü; 0: yok</summary>
    public double ReliefEvent;
    public double LastPeak = -9999;
    /// <summary>dünyanın olağan gerilimi: yerleşim başına gerilimin 4 yıllık üssel ortalaması (zirve buna göre; 0: henüz yok)</summary>
    public double Base;
    /// <summary>Faz 1b-3: kuraklığın (eski sert kışın yerine) sona ereceği gün (0: yok) ve son kuraklığın yılı</summary>
    public double DroughtUntil;
    public double LastDrought = -99;
    /// <summary>"Kızıl Ay" (baskın dalgası) sonu: kamplar daha kalabalık akın eder</summary>
    public double SurgeUntil;
    /// <summary>hedef kamp sayısı (3 + yıl/6): bir sonraki kampın en erken günü</summary>
    public double NextCampSpawn;
}

/// <summary>Ejderhanın durumu (dünyada bir kez, 18–22. yıllar arasında uyanır). Kampı <see cref="Camp"/> (Kind "dragon").</summary>
public sealed class DragonState
{
    public double WakeDay;
    /// <summary>ejderhanın kampının id'si; -1: henüz uyanmadı</summary>
    public int Camp = -1;
    public string Name;
    /// <summary>kalıcı yaralar: inde ya da akında aldığı yara günde azar azar kapanır; uyandığı yaşın canı (BaseHp) hazineyle büyür</summary>
    public double Hp, MaxHp, BaseHp;
    public double NextRaid;
    public double Raids;
    /// <summary>medeniyet → yediği akın sayısı</summary>
    public JsNumObj<double> Hits = new();
    /// <summary>medeniyet → haraçla korunduğu son gün</summary>
    public JsNumObj<double> Paid = new();
    /// <summary>medeniyet → haracı en son reddettiği yıl</summary>
    public JsNumObj<double> Refused = new();
    public double Tributes;
    /// <summary>ejderhaya karşı ittifakın üyeleri (medeniyet id'leri; sefer bitince boşalır)</summary>
    public List<int> Alliance = new();
    public double AllianceTries;
    public double NextAlliance;
    /// <summary>aynı anda inde buluşsunlar diye sırayla yola çıkacak ordular</summary>
    public List<DragonMarch> Marches = new();
    /// <summary>öldüğü gün (-1: yaşıyor), öldürenler, hazinesi</summary>
    public double SlainDay = -1;
    public string SlainBy;
    public double Hoard;
    /// <summary>Faz 1 C3: uyandığında inindeki (uykudan kalan) hazine; ejderha bundan sonra kaçırdığı altınla büyür</summary>
    public double Loot0;
}

public sealed class DragonMarch
{
    public int Civ;
    /// <summary>yola çıkış günü; ordusu başka seferdeyse en geç Until'e dek ertelenir</summary>
    public double Day, Until;
}

// ---- Faz 1, C3: altın ve ambar (bakım, kent tüketimi, kıtlık)

/// <summary>Medeniyetin kıtlığı: tek büyük olay olarak yaşanır (Economy.FamineDay). Açık birkaç gün sürünce ilan edilir;
/// ölenler, komşuların yardımı ya da reddi burada tutulur; ambar art arda birkaç gün yetince biter.</summary>
public sealed class FamineState
{
    /// <summary>gıda açığının başladığı gün</summary>
    public double Since;
    /// <summary>açık geçen gün sayısı ve günlük açık paylarının toplamı (açık / ihtiyaç, gün başına en çok 1)</summary>
    public double Days, Short;
    /// <summary>büyük olay olarak ilan edildi mi (açık Economy.FAMINE_DECLARE gün sürünce)</summary>
    public bool Declared;
    /// <summary>açlıktan ölenler</summary>
    public double Dead;
    /// <summary>art arda tok geçen gün (Economy.FAMINE_END olunca kıtlık biter)</summary>
    public double OkDays;
    /// <summary>komşulara bir sonraki yardım çağrısının günü ve yapılan çağrı sayısı</summary>
    public double NextAid, Rounds;
    /// <summary>komşulardan gelen gıda (gıda birimi)</summary>
    public double AidFood;
    /// <summary>yardım eden ve yüz çeviren medeniyetler (bu kıtlıkta)</summary>
    public List<int> Helped = new(), Refused = new();
    /// <summary>kıtlığın nedeni (kronik)</summary>
    public string Why;
}

/// <summary>Medeniyetin günlük bütçesi (altın/gün): vergi, darphane, zanaatçılar; asker, kahraman ve yapı bakımı.</summary>
public sealed class CivBudget
{
    public double Income, Upkeep;
    public double Soldiers, Heroes, Buildings;
}
