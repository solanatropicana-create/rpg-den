import type { ClassId, RaceId } from '../data/classes';
import type { CivicKind, DepositKind, ExtractKind, Good, Stock, Terrain, WorkshopKind } from '../data/goods';
import type { Effects, Era } from '../data/techs';
import type { HeroClass, Stat, HeroPath, HeroAlign, TraitId } from '../data/heroes';

export type Pop = Partial<Record<RaceId, number>>;

export interface ExtractBuilding { kind: ExtractKind; level: number; settlement: number; workers: number; depleted?: boolean; burned?: number; burnedAt?: number }

export interface Tile {
  terrain: Terrain;
  elev: number;
  owner: number;        // yerleşim id, -1 yok
  road: number;
  deposit: number;      // yatak id, -1 yok
  reserve: number;      // yatak rezervi (yenilenmeyenler için)
  wood: number;         // orman kereste rezervi
  cutDay?: number;      // kesildiği gün (yeniden büyüme)
  ext?: ExtractBuilding;
  camp?: number;        // canavar kampı id
  sea?: boolean;        // açık deniz (geçilmez)
  isle?: number;        // ada numarası (anakara: yok)
  inn?: number;         // tarafsız han id
  innZone?: number;     // hanın koruma halkası (sınır genişlemesi alamaz)
}

export interface Deposit {
  id: number;
  kind: DepositKind;
  tiles: number[];
  richness: number;     // 0.7 fakir, 1 normal, 1.4 zengin
  depleted: boolean;
  knownBy: number[];    // bu yatağı bilen medeniyetler
}

export interface Alignment { law: number; good: number }

export interface Project {
  type: 'civic' | 'workshop' | 'extract' | 'upgrade';
  kind: string;
  tile?: number;
  level?: number;
  left: number;
  total: number;
}

export interface Settlement {
  id: number;
  civ: number;
  name: string;
  tile: number;
  founded: number;
  pop: Pop;
  growthAcc: number;
  civics: Partial<Record<CivicKind, number>>;
  workshops: Partial<Record<WorkshopKind, number>>;
  project: Project | null;
  jobs: Record<string, number>;
  soldiers: number;
  alive: boolean;
  starving: number;
  tier: number;          // 0 kamp, 1 köy, 2 kasaba, 3 şehir
  mixedSince: Partial<Record<string, number>>;
  hunger?: number;
  burnedHouses?: number;
  burnedAt?: number;
  plague?: { since: number; until: number; severity: number; dead: number };
  plagueImmune?: number;
  graves?: number;
  shantyLog?: boolean;
}

export interface RelMod { key: string; text: string; value: number; decay: number }
export interface War { since: number; attacker: number; target: number; attacks: number; lastArmy: number; goal: string }
export interface Relation {
  contact: boolean;
  mods: RelMod[];
  war: War | null;
  treaty: Good | null;        // kaynak antlaşması
  tension: Partial<Record<Good, number>>;
  lastTalk: number;
  lastRaid: number;
  land?: number;              // toprak açlığı (savaş gerekçesi)
  peaceDay?: number;
}

export interface Civ {
  id: number;
  cls: ClassId;
  name: string;
  race: RaceId;
  align: Alignment;
  color: string;
  stock: Stock;
  price: Stock;
  want: Stock;               // planlanan talep
  research: { current: string | null; progress: number; done: string[]; reason: string; hard?: boolean };
  era: Era;
  eraDay: number[];          // her çağa geçiş günü
  subclass: string | null;
  eff: Effects;
  alive: boolean;
  founded: number;
  threat: number;
  lastRaidedDay: number;
  scoutSent: boolean;
  stats: { peakPop: number; battlesWon: number; battlesLost: number; traded: number; mined: Stock; depleted: number };
  lastExpand: number;
  yearly: Record<string, number>; // yıllık yeteneklerin son kullanım yılı
  extinctDay?: number;
  respawned?: boolean;
  history: { day: number; pop: number; gold: number; techs: number }[];
  lastWarEnd?: number;
  innBanUntil?: number;      // Han Bozan: bu güne dek hanlardan kahraman kiralayamaz
}

export interface Hero {
  id: number;
  name: string;
  race: RaceId;
  cls: HeroClass;
  level: number;
  xp: number;
  stats: Record<Stat, number>;
  maxHp: number;
  hp: number;
  ac: number;
  civ: number;
  pos: number;
  tavern: number;
  state: 'tavern' | 'traveling' | 'home' | 'quest' | 'dead' | 'gone' | 'army' | 'retired';
  born: number;
  idleSince: number;
  kills: number;
  gold: number;
  bio: string;
  deathDay?: number;
  revived?: boolean;
  // ---- kahraman iradesi
  align: HeroAlign;
  path: HeroPath;
  traits: TraitId[];
  epithet?: string;
  tally: Record<string, number>;        // goblin, ruins, plague, lib, fails, solo...
  bonus: { atk: number };
  rep: Record<number, number>;          // medeniyet id → itibar
  journal: { day: number; text: string }[];
  base: number;                         // yuva: han ya da taverna (yerleşim) id
  baseInn: boolean;
  birth: number;                        // doğduğu han ya da yerleşim id
  goal?: HeroGoal;
  contract?: { civ: number; since: number; until: number; paid: number };
  auction?: { end: number; bids: { civ: number; gold: number }[] };
  vendetta?: number;                    // intikam: kamp id
  grudge?: number;                      // kin: medeniyet id
  soloUntil?: number;
  seen?: number[];                      // gezilen harabe / kütüphane
  hired?: number;                       // kaç kez kiralandı
  legend?: boolean;
  lastGoal?: number;
}

export type GoalKind = 'quest' | 'hunt' | 'ruin' | 'plague' | 'temple' | 'library' | 'rob' | 'duel';
export interface HeroGoal { kind: GoalKind; tile: number; target?: number; text: string; since: number; stay?: number }

export interface Inn {
  id: number;
  tile: number;
  name: string;
  keeper: string;
  founded: number;
  alive: boolean;
  ruinedDay?: number;
  gold: number;
  teacher?: number;       // öğretmenlik yapan emekli kahraman
  raids: number;
}

export type CampKind = 'goblin' | 'hobgoblin' | 'bugbear';
export interface Camp {
  id: number;
  kind: CampKind;
  tile: number;
  name: string;
  count: number;
  boss: boolean;
  hadBoss: boolean;
  loot: number;
  growthAcc: number;
  alive: boolean;
  nextRaid: number;
  founded: number;
  clearedDay?: number;
}

export interface Quest { id: number; civ: number; camp: number; bounty: number; posted: number; takenBy: number[]; open: boolean; inn?: number; expires?: number; failures?: number; done?: number }

export type AgentKind = 'caravan' | 'army' | 'raid' | 'hero' | 'settlers' | 'scout' | 'party';
export interface Agent {
  id: number;
  kind: AgentKind;
  civ: number;
  path: number[];
  step: number;
  progress: number;
  speed: number;
  troops?: number;
  heroes?: number[];
  cargo?: Stock;
  from?: number;
  to?: number;
  purpose?: string;
  route?: number;
  pop?: Pop;
  targetTile?: number;
  quest?: number;
  returning?: boolean;
  dead?: boolean;
  loot?: number;
  chase?: number;
  home?: number;
  boss?: boolean;
  monster?: CampKind;
}

export interface TradeRoute { id: number; a: number; b: number; kind: 'trade' | 'treaty'; good?: Good; path: number[]; nextDepart: number; trips: number; alive: boolean; since: number }

export interface BattleLine { t: string; crit?: boolean; fumble?: boolean }
export interface Battle {
  id: number; day: number; tile: number; title: string; sideA: string; sideB: string; winner: 'A' | 'B';
  lossesA: number; lossesB: number; lines: BattleLine[]; rolls: { d20: number; side: 'A' | 'B'; who: string }[];
}

export type EventKind =
  | 'growth' | 'research' | 'era' | 'build' | 'settle' | 'contact' | 'discover' | 'tension' | 'diplomacy' | 'economy'
  | 'trade' | 'raid' | 'hero' | 'quest' | 'war' | 'battle' | 'migration' | 'death' | 'lair' | 'world' | 'class' | 'wonder' | 'inn';

export interface GameEvent { id: number; day: number; kind: EventKind; text: string; cause?: string; civ?: number; tile?: number; battle?: number; major?: boolean }

export interface World {
  seed: number;
  day: number;
  W: number;
  H: number;
  tiles: Tile[];
  deposits: Deposit[];
  civs: Civ[];
  settlements: Settlement[];
  heroes: Hero[];
  camps: Camp[];
  inns: Inn[];
  quests: Quest[];
  agents: Agent[];
  routes: TradeRoute[];
  relations: Relation[][];
  events: GameEvent[];
  battles: Battle[];
  nextId: number;
  rngState: number;
  metrics: Record<string, number>;
}

export type { Good, Stock, WorkshopKind, CivicKind, ExtractKind, DepositKind };
