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
  pass?: boolean;       // dağ geçidi: geçilmez dağ kütlesinde açılmış tepe
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
  type: 'civic' | 'workshop' | 'extract' | 'upgrade' | 'ship';
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
  // ---- denizcilik
  port?: number;          // tersanenin kurulduğu kıyı karosu (gemiler buradan kalkar)
  ships?: number;         // yük/yolcu gemisi (tekne → koga)
  galleys?: number;       // savaş gemisi (kadırga)
  overseas?: boolean;     // denizaşırı kurulan koloni
}

export interface RelMod { key: string; text: string; value: number; decay: number }
export interface War { since: number; attacker: number; target: number; attacks: number; lastArmy: number; goal: string; ally?: number }
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
  seaTry?: number;            // son deniz ticaret yolu denemesi
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
  seaScout?: boolean;        // keşif gemisi gönderildi
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

export type GuestKind = 'hero' | 'merchant' | 'pilgrim' | 'bard' | 'hunter' | 'scholar' | 'soldier' | 'refugee' | 'wanderer' | 'caravan' | 'noble';
/** handa kalan (ya da hana yürüyen / handan ayrılan) misafir */
export interface Guest {
  id: number;
  kind: GuestKind;
  name: string;
  race: RaceId;
  n: number;              // kaç kişi (aile, maiyet, kervan tayfası)
  civ: number;            // bağlı olduğu medeniyet (-1 bağımsız)
  from: number;           // geldiği yerleşim id (-1 bilinmiyor)
  fromName: string;
  to: number;             // gideceği yerleşim id (-1)
  toName: string;
  why: string;            // yolculuğun sebebi
  purse: number;          // kesesindeki altın
  spent: number;          // handa harcadığı altın
  nights: number;         // kalacağı gece (kahraman: belirsiz = 0)
  arrived: number;        // hana geldiği gün (yoldayken yola çıktığı gün)
  hero?: number;
  agent?: number;         // konaklayan kervan ajanı
  stable?: boolean;       // oda yokken ahırda / ortak salonda yatıyor
  mood: number;           // 0..1 memnuniyet
}
export type StaffRole = 'cirak' | 'asci' | 'seyis' | 'garson' | 'bekci';
export interface InnStaff { name: string; race: RaceId; role: StaffRole; since: number; from: string }
/** hanın defterine düşen bir satır: gelen, giden, alım, inşaat... */
export interface InnLog { day: number; k: 'in' | 'out' | 'buy' | 'build' | 'staff' | 'ev' | 'no'; t: string; g?: number }
/** mevsimlik kasa defteri (altın) */
export interface InnBook { season: number; room: number; food: number; ale: number; other: number; supply: number; wage: number; build: number; guests: number; nights: number }
export interface InnBuild { level: number; work: number; need: number; wood: number; woodNeed: number; stone: number; stoneNeed: number; started: number; rebuild?: boolean }

export interface Inn {
  id: number;
  tile: number;
  name: string;
  keeper: string;
  founded: number;        // kapılarını açtığı gün (yoldayken / inşaatta: yola çıkış)
  alive: boolean;         // açık ve çalışıyor
  ruinedDay?: number;
  gold: number;
  teacher?: number;       // öğretmenlik yapan emekli kahraman
  raids: number;
  // ---- yaşayan han
  stage: 'road' | 'build' | 'open' | 'ruin';
  level: number;          // 1 Yol hanı · 2 Han · 3 Kervansaray
  keeperRace: RaceId;
  origin: number;         // hancının yola çıktığı yerleşim (ya da han) id
  originName: string;
  build?: InnBuild;       // inşaat / genişletme
  stock: { food: number; ale: number; wood: number };   // porsiyon, bardak, yük
  fame: number;           // ün 0–100
  staff: InnStaff[];
  guests: Guest[];
  tabs: Record<number, number>;                          // kahramanların veresiye borcu (gümüş)
  log: InnLog[];
  books: InnBook[];
  hist: { day: number; guests: number; gold: number; fame: number }[];
  order?: { agent: number; fromName: string; goods: string; cost: number; day: number };
  total: { guests: number; nights: number; income: number; turned: number; bought: number };
  turned: number;         // son yıl yer bulamayan (ahır dâhil) — genişleme baskısı
  sat: number;            // son memnuniyet ortalaması 0..1
  traffic: number;        // çevredeki yolcu akışı (her 10 günde hesaplanır)
}

export type CampKind = 'goblin' | 'hobgoblin' | 'bugbear' | 'pirate';
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
  captain?: string;       // korsan koyu: kaptanın adı
}

export type IsleKind = 'volkan' | 'orman' | 'cayir' | 'kayalik' | 'bataklik' | 'kumsal';
export interface IsleInfo { id: number; name: string; kind: IsleKind; size: number; center: number; peak?: number }

export interface Quest { id: number; civ: number; camp: number; bounty: number; posted: number; takenBy: number[]; open: boolean; inn?: number; expires?: number; failures?: number; done?: number; topped?: number }

export type AgentKind = 'caravan' | 'army' | 'raid' | 'hero' | 'settlers' | 'scout' | 'party' | 'keeper' | 'traveler' | 'supply' | 'ship';
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
  // ---- hanlar: hancı kafilesi, yolcu, erzak arabası, konaklayan kervan
  guest?: Guest;          // yolcu ajanının kimliği
  inn?: number;           // ilgili han
  restUntil?: number;     // kervan handa konaklıyor: bu güne dek bekler
  restedAt?: number[];    // bu yolculukta konakladığı hanlar
  // ---- deniz yolculuğu
  hull?: number;          // gemiyi veren liman yerleşimi (gemi yoldayken orada eksik sayılır)
  galleys?: number;       // eşlik eden kadırga sayısı
  landing?: number;       // karaya çıkılan kıyı karosu (dönüşte yeniden binilir)
  fought?: number[];      // deniz savaşı yapılan liman yerleşimleri
  // ---- ortak saldırı
  muster?: { since: number; until: number };   // hedefte dostlarını bekliyor
}

export interface TradeRoute { id: number; a: number; b: number; kind: 'trade' | 'treaty'; good?: Good; path: number[]; nextDepart: number; trips: number; alive: boolean; since: number; sea?: boolean }

export interface BattleLine { t: string; crit?: boolean; fumble?: boolean }
/** Savaş tekrarı: birlikler ve her zar atışı (görünüm tur tur yeniden oynatır) */
export interface ReplayUnit {
  n: string; s: 'A' | 'B'; k: string; hp: number; max: number; ac: number; atk: number;
  dmg: [number, number, number]; att: number; cls?: string; lvl?: number; boss?: boolean; g: number; race?: string;
}
export interface ReplayPart { l: string; v: number; dd?: number[]; ds?: number }
export interface ReplayEv {
  r: number;               // tur
  sp?: 'round' | 'attack' | 'fireball' | 'burning' | 'heal' | 'rage' | 'wild' | 'second' | 'flee' | 'meteor' | 'rout' | 'fumble';
  a?: number; t?: number;  // saldıran / hedef (units dizini)
  d?: number; m?: number; ac?: number; h?: 0 | 1 | 2;   // d20, saldırı bonusu, hedef AC, ıska/isabet/kritik
  dd?: number[]; ds?: number; b?: number;             // hasar zarları, zar yüzü, sabit bonus
  x?: ReplayPart[]; mul?: number; half?: boolean;      // ek hasarlar, ilk vuruş çarpanı, öfke yarılaması
  v?: number; hp?: number;                              // son hasar (ya da iyileşme) ve hedefin kalan canı
  ts?: number[]; vs?: number[]; sv?: boolean[]; dds?: number[][];   // çok hedefli büyü: hedefler, hasarlar, kurtarma, zarlar
  aA?: number; aB?: number;                             // tur başında ayakta kalanlar
}
export interface ReplayGroup { name: string; side: 'A' | 'B'; civ?: number; kind?: string }
export interface Replay {
  units: ReplayUnit[]; ev: ReplayEv[]; groups: ReplayGroup[];
  moraleA: number; moraleB: number; heroMorale: number; noRoutA?: boolean; noRoutB?: boolean;
  powA: number; powB: number; rounds: number; maxRounds: number;
  end: 'wipe' | 'rout' | 'timeout'; routed?: 'A' | 'B'; timeoutWinner?: 'A' | 'B';
}
export interface Battle {
  id: number; day: number; tile: number; title: string; sideA: string; sideB: string; winner: 'A' | 'B'; naval?: boolean;
  lossesA: number; lossesB: number; lines: BattleLine[]; rolls: { d20: number; side: 'A' | 'B'; who: string }[];
  civA?: number; civB?: number; joint?: boolean; replay?: Replay;
}

export type EventKind =
  | 'growth' | 'research' | 'era' | 'build' | 'settle' | 'contact' | 'discover' | 'tension' | 'diplomacy' | 'economy'
  | 'trade' | 'raid' | 'hero' | 'quest' | 'war' | 'battle' | 'migration' | 'death' | 'lair' | 'world' | 'class' | 'wonder' | 'inn' | 'sea';

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
  innPlan: { target: number; wave: number; next: number };   // açılış dalgası: kaç hancı yola çıkacak
  quests: Quest[];
  agents: Agent[];
  routes: TradeRoute[];
  relations: Relation[][];
  events: GameEvent[];
  battles: Battle[];
  nextId: number;
  rngState: number;
  metrics: Record<string, number>;
  isles?: IsleInfo[];                   // ana kıta dışındaki adalar (ad, tür, büyüklük)
  seaProfile?: 'kita' | 'takimada' | 'buyuk';
}

export type { Good, Stock, WorkshopKind, CivicKind, ExtractKind, DepositKind };
