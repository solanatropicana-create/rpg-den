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
    public int Tier;             // 0 camp, 1 village, 2 town, 3 city
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
}

public sealed class Relation
{
    public bool Contact;
    public List<RelMod> Mods = new();
    public War War;
    public string Treaty;
    public JsObj<double> Tension = new();
    public double LastTalk;
    public double LastRaid;
    public double? Land;
    public double? PeaceDay;
    public double? SeaTry;
}

public sealed class ResearchState
{
    public string Current;
    public double Progress;
    public List<string> Done = new();
    public string Reason;
    public bool? Hard;
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
    public double Techs;
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
    public string Cls;
    public string Name;
    public string Race;
    public Alignment Align;
    public string Color;
    public JsObj<double> Stock = new();
    public JsObj<double> Price = new();
    public JsObj<double> Want = new();
    public ResearchState Research;
    public int Era;
    public List<double> EraDay = new();
    public string Subclass;
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
    public double Season;
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
    public string Captain;
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
    public List<Battle> Battles = new();
    public int NextId;
    public double RngState;
    public JsObj<double> Metrics = new();
    public List<IsleInfo> Isles;
    public string SeaProfile;    // 'kita' | 'takimada' | 'buyuk'
}
