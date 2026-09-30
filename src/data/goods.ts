// Mallar: ham kaynaklar ve işlenmiş ürünler. `base` taban fiyatı (altın cinsinden).
export type Good =
  | 'grain' | 'meat' | 'fish' | 'bread' | 'beer'
  | 'wood' | 'stone' | 'clay' | 'planks' | 'bricks' | 'leather'
  | 'copper' | 'tin' | 'bronze' | 'iron' | 'coal' | 'steel' | 'tools' | 'arms'
  | 'horses' | 'salt' | 'gold' | 'herbs' | 'potion' | 'fur'
  | 'mana' | 'scroll' | 'enchanted' | 'mithril' | 'mithrilArmor' | 'heartwood';

export type Tier = 'temel' | 'stratejik' | 'değerli' | 'efsanevi' | 'işlenmiş';

export interface GoodDef { id: Good; name: string; base: number; tier: Tier; food?: number }

const G = (id: Good, name: string, base: number, tier: Tier, food?: number): GoodDef => ({ id, name, base, tier, food });

export const GOODS: Record<Good, GoodDef> = {
  grain: G('grain', 'Tahıl', 1, 'temel', 1),
  meat: G('meat', 'Av eti', 1.3, 'temel', 1),
  fish: G('fish', 'Balık', 1.1, 'temel', 1),
  bread: G('bread', 'Ekmek', 2.8, 'işlenmiş', 2.4),
  beer: G('beer', 'Bira', 3.5, 'işlenmiş'),
  wood: G('wood', 'Kereste', 1, 'temel'),
  stone: G('stone', 'Taş', 1.3, 'temel'),
  clay: G('clay', 'Kil', 0.9, 'temel'),
  planks: G('planks', 'Kereste levha', 2.6, 'işlenmiş'),
  bricks: G('bricks', 'Tuğla', 3.2, 'işlenmiş'),
  leather: G('leather', 'Deri', 2.2, 'temel'),
  copper: G('copper', 'Bakır', 3, 'stratejik'),
  tin: G('tin', 'Kalay', 4.5, 'stratejik'),
  bronze: G('bronze', 'Bronz', 11, 'işlenmiş'),
  iron: G('iron', 'Demir', 4, 'stratejik'),
  coal: G('coal', 'Kömür', 2.5, 'stratejik'),
  steel: G('steel', 'Çelik', 13, 'işlenmiş'),
  tools: G('tools', 'Alet', 15, 'işlenmiş'),
  arms: G('arms', 'Silah ve zırh', 30, 'işlenmiş'),
  horses: G('horses', 'At', 9, 'stratejik'),
  salt: G('salt', 'Tuz', 5, 'değerli'),
  gold: G('gold', 'Altın', 1, 'değerli'),
  herbs: G('herbs', 'Şifalı ot', 4, 'değerli'),
  potion: G('potion', 'İksir', 11, 'işlenmiş'),
  fur: G('fur', 'Kürk', 6, 'değerli'),
  mana: G('mana', 'Mana kristali', 16, 'efsanevi'),
  scroll: G('scroll', 'Parşömen', 22, 'işlenmiş'),
  enchanted: G('enchanted', 'Efsunlu silah', 95, 'işlenmiş'),
  mithril: G('mithril', 'Mithril', 40, 'efsanevi'),
  mithrilArmor: G('mithrilArmor', 'Mithril zırh', 150, 'işlenmiş'),
  heartwood: G('heartwood', 'Kadim ağaç özü', 30, 'efsanevi'),
};
export const GOOD_IDS = Object.keys(GOODS) as Good[];
export const FOODS: Good[] = ['bread', 'fish', 'meat', 'grain'];
export type Stock = Partial<Record<Good, number>>;

// ---- Kaynak yatakları ----
export type DepositKind =
  | 'fertile' | 'clay' | 'copper' | 'tin' | 'iron' | 'coal' | 'horses' | 'salt' | 'gold' | 'silver'
  | 'herbs' | 'mana' | 'mithril' | 'heartwood';

export interface DepositDef {
  kind: DepositKind;
  name: string;
  good: Good;          // ne üretir (gümüş → altın)
  rate: number;        // işçi/gün başına temel verim (L1)
  reserve: number;     // hex başına rezerv; 0 = yenilenir
  regen?: number;      // yenilenen rezervler için günlük geri dolum
  terrain: Terrain[];
  size: [number, number];
  count: number;       // 72x48 haritada kaç yatak
  hiddenUntil?: string; // bu araştırmaya kadar görünmez
  building: ExtractKind;
  color: string;
}
export type Terrain = 'grass' | 'forest' | 'oldforest' | 'hill' | 'mountain' | 'water' | 'swamp' | 'tundra';
export const TERRAIN_TR: Record<Terrain, string> = { grass: 'Çayır', forest: 'Orman', oldforest: 'Kadim orman', hill: 'Tepe', mountain: 'Dağ', water: 'Nehir/göl', swamp: 'Bataklık', tundra: 'Karlı tundra' };
export const MOVE_COST: Record<Terrain, number> = { grass: 1, forest: 1.6, oldforest: 2, hill: 2, mountain: 5, water: 3.5, swamp: 3, tundra: 1.5 };

export const DEPOSITS: Record<DepositKind, DepositDef> = {
  fertile: { kind: 'fertile', name: 'Verimli ova', good: 'grain', rate: 0.36, reserve: 0, terrain: ['grass'], size: [4, 8], count: 16, building: 'farm', color: '#e3cf6e' },
  clay: { kind: 'clay', name: 'Kil yatağı', good: 'clay', rate: 0.22, reserve: 220, terrain: ['grass', 'swamp'], size: [2, 4], count: 9, building: 'claypit', color: '#c98a5e' },
  copper: { kind: 'copper', name: 'Bakır damarı', good: 'copper', rate: 0.11, reserve: 170, terrain: ['hill'], size: [3, 6], count: 8, building: 'mine', color: '#d4773b' },
  tin: { kind: 'tin', name: 'Kalay damarı', good: 'tin', rate: 0.08, reserve: 110, terrain: ['hill'], size: [2, 3], count: 5, building: 'mine', color: '#b9c2c9' },
  iron: { kind: 'iron', name: 'Demir damarı', good: 'iron', rate: 0.11, reserve: 210, terrain: ['mountain', 'hill'], size: [3, 6], count: 8, building: 'mine', color: '#8a8f99' },
  coal: { kind: 'coal', name: 'Kömür yatağı', good: 'coal', rate: 0.13, reserve: 210, terrain: ['hill', 'swamp'], size: [3, 5], count: 7, building: 'mine', color: '#3a3a3a' },
  horses: { kind: 'horses', name: 'Yaban at sürüsü', good: 'horses', rate: 0.025, reserve: 0, terrain: ['grass'], size: [3, 5], count: 6, building: 'pasture', color: '#a0714f' },
  salt: { kind: 'salt', name: 'Kaya tuzu', good: 'salt', rate: 0.09, reserve: 150, terrain: ['hill', 'tundra'], size: [2, 3], count: 4, building: 'mine', color: '#f1efe8' },
  gold: { kind: 'gold', name: 'Altın damarı', good: 'gold', rate: 0.07, reserve: 100, terrain: ['mountain', 'hill'], size: [2, 4], count: 3, building: 'mine', color: '#f2c230' },
  silver: { kind: 'silver', name: 'Gümüş damarı', good: 'gold', rate: 0.045, reserve: 120, terrain: ['mountain'], size: [2, 4], count: 3, building: 'mine', color: '#d8dde3' },
  herbs: { kind: 'herbs', name: 'Şifalı ot tarlası', good: 'herbs', rate: 0.09, reserve: 90, regen: 0.06, terrain: ['forest', 'swamp'], size: [3, 5], count: 8, building: 'herbalist', color: '#6fc27a' },
  mana: { kind: 'mana', name: 'Mana kristali', good: 'mana', rate: 0.035, reserve: 110, terrain: ['swamp', 'mountain'], size: [2, 3], count: 3, hiddenUntil: 'arcana1', building: 'crystal', color: '#9b6bff' },
  mithril: { kind: 'mithril', name: 'Mithril damarı', good: 'mithril', rate: 0.025, reserve: 90, terrain: ['mountain'], size: [2, 3], count: 2, hiddenUntil: 'deepmine', building: 'mithril', color: '#8fe6f0' },
  heartwood: { kind: 'heartwood', name: 'Kadim ağaçlar', good: 'heartwood', rate: 0.025, reserve: 70, regen: 0.02, terrain: ['oldforest'], size: [3, 6], count: 0, building: 'grove', color: '#2e7d4a' },
};

// ---- Çıkarma yapıları (hex'e kurulur) ----
export type ExtractKind = 'farm' | 'lumber' | 'hunt' | 'dock' | 'quarry' | 'claypit' | 'mine' | 'pasture' | 'herbalist' | 'crystal' | 'mithril' | 'grove';
export interface ExtractDef {
  kind: ExtractKind;
  names: [string, string, string]; // L1, L2, L3 (boşsa yok)
  tech: [string | null, string | null, string | null]; // seviye başına gereken araştırma; null = yok/başlangıç
  startLevel?: number;
}
export const EXTRACTS: Record<ExtractKind, ExtractDef> = {
  farm: { kind: 'farm', names: ['Tarla', 'Sulamalı Tarla', 'Çiftlik'], tech: ['agriculture', 'irrigation', 'rotation'] },
  lumber: { kind: 'lumber', names: ['Oduncu Kulübesi', 'Bıçkıhane', 'Ormancılık Evi'], tech: [null, 'woodwork', 'forestry'] },
  hunt: { kind: 'hunt', names: ['Avcı Kampı', 'Av Köşkü', ''], tech: [null, 'husbandry', null] },
  dock: { kind: 'dock', names: ['İskele', 'Balıkçı Limanı', ''], tech: ['fishing', 'shipbuilding', null] },
  quarry: { kind: 'quarry', names: ['Taş Ocağı', 'Kesme Taş Ocağı', ''], tech: ['stonework', 'architecture', null] },
  claypit: { kind: 'claypit', names: ['Kil Çukuru', '', ''], tech: ['pottery', null, null] },
  mine: { kind: 'mine', names: ['Açık Ocak', 'Galeri Madeni', 'Derin Maden'], tech: ['mining', 'bronzetools', 'deepmine'] },
  pasture: { kind: 'pasture', names: ['Ağıl', 'Haras', ''], tech: ['husbandry', 'riding', null] },
  herbalist: { kind: 'herbalist', names: ['Toplayıcı Kulübesi', 'Bitki Bahçesi', ''], tech: ['herbalism', 'medicine', null] },
  crystal: { kind: 'crystal', names: ['Kristal Kazısı', 'Kristal Kulesi', 'Ley Odak Taşı'], tech: ['arcana1', 'arcana2', 'highmagic'] },
  mithril: { kind: 'mithril', names: ['', 'Mithril Galerisi', 'Mithril Dökümhanesi'], tech: [null, 'deepmine', 'mithrilwork'], startLevel: 2 },
  grove: { kind: 'grove', names: ['Öz Toplama', 'Kutsal Koru', ''], tech: ['druid_grove', 'druid_grove', null] },
};
export const LEVEL_SLOTS = [0, 3, 5, 7];
export const LEVEL_MULT = [0, 1, 2, 3.5];
const EXT_POSS: Record<string, string> = { 'Tarla': 'tarlası', 'Sulamalı Tarla': 'sulamalı tarlası', 'Çiftlik': 'çiftliği', 'Bıçkıhane': 'bıçkıhanesi', 'İskele': 'iskelesi', 'Açık Ocak': 'açık ocağı', 'Derin Maden': 'derin madeni', 'Ağıl': 'ağılı', 'Haras': 'harası', 'Öz Toplama': 'öz toplama yeri', 'Kutsal Koru': 'kutsal korusu' };
/** "X'in ___" kalıbı için yapı adının iyelik hâli: tarlası, iskelesi, avcı kampı */
export function extPoss(kind: ExtractKind, level: number) { const n = EXTRACTS[kind].names[level - 1] || EXTRACTS[kind].names.find(Boolean)!; return EXT_POSS[n] ?? n.toLocaleLowerCase('tr'); }
export const LEVEL_COST: Stock[] = [{}, { wood: 8 }, { wood: 20, stone: 15, bronze: 4 }, { stone: 30, steel: 12, gold: 20 }];

// Temel (her hex'te olan) kaynaklar
export const WOOD_RESERVE = 160;
export const STONE_RESERVE = 1500;
export const FOREST_REGROW_DAYS = 1100;

// ---- İşleme yapıları (yerleşim yuvası kullanır) ----
export type WorkshopKind = 'bakery' | 'brewery' | 'sawmill' | 'brickkiln' | 'foundry' | 'smithy' | 'toolmaker' | 'armory' | 'apothecary' | 'scriptorium' | 'enchanter' | 'mithrilforge';
export interface WorkshopDef {
  kind: WorkshopKind;
  name: string;
  tech: string;
  inputs: Stock;          // 1 çıktı için
  alt?: Stock;            // alternatif girdi (ör. bronz yerine çelik)
  output: Good;
  rate: number;           // işçi/gün başına çıktı
  slots: number;
  cost: Stock;
}
export const WORKSHOPS: Record<WorkshopKind, WorkshopDef> = {
  bakery: { kind: 'bakery', name: 'Fırın', tech: 'pottery', inputs: { grain: 2 }, output: 'bread', rate: 0.45, slots: 2, cost: { wood: 10, clay: 6 } },
  brewery: { kind: 'brewery', name: 'Bira Evi', tech: 'tavern', inputs: { grain: 2 }, output: 'beer', rate: 0.3, slots: 2, cost: { wood: 12, clay: 4 } },
  sawmill: { kind: 'sawmill', name: 'Bıçkı Atölyesi', tech: 'woodwork', inputs: { wood: 2 }, output: 'planks', rate: 0.4, slots: 3, cost: { wood: 20 } },
  brickkiln: { kind: 'brickkiln', name: 'Tuğla Fırını', tech: 'pottery', inputs: { clay: 2, wood: 0.5 }, output: 'bricks', rate: 0.35, slots: 3, cost: { wood: 12, clay: 4 } },
  foundry: { kind: 'foundry', name: 'Dökümhane', tech: 'bronze', inputs: { copper: 2, tin: 1 }, output: 'bronze', rate: 0.2, slots: 3, cost: { wood: 12, stone: 12 } },
  smithy: { kind: 'smithy', name: 'Demirci Ocağı', tech: 'smithing', inputs: { iron: 2, coal: 1 }, output: 'steel', rate: 0.2, slots: 3, cost: { planks: 8, stone: 15, bronze: 2 } },
  toolmaker: { kind: 'toolmaker', name: 'Aletçi', tech: 'bronzetools', inputs: { bronze: 1, wood: 1 }, alt: { steel: 1, wood: 1 }, output: 'tools', rate: 0.15, slots: 2, cost: { planks: 6, stone: 6 } },
  armory: { kind: 'armory', name: 'Silahhane', tech: 'training', inputs: { bronze: 2, leather: 1 }, alt: { steel: 2, leather: 1 }, output: 'arms', rate: 0.12, slots: 3, cost: { planks: 8, stone: 10 } },
  apothecary: { kind: 'apothecary', name: 'Şifa Evi', tech: 'medicine', inputs: { herbs: 2 }, output: 'potion', rate: 0.2, slots: 2, cost: { planks: 6, bricks: 6 } },
  scriptorium: { kind: 'scriptorium', name: 'Yazıhane', tech: 'arcana1', inputs: { mana: 1, leather: 1 }, output: 'scroll', rate: 0.1, slots: 2, cost: { planks: 8, bricks: 6 } },
  enchanter: { kind: 'enchanter', name: 'Büyü Atölyesi', tech: 'enchanting', inputs: { arms: 1, scroll: 2 }, output: 'enchanted', rate: 0.05, slots: 2, cost: { bricks: 15, steel: 5, mana: 3 } },
  mithrilforge: { kind: 'mithrilforge', name: 'Mithril Dökümhanesi', tech: 'mithrilwork', inputs: { mithril: 2, steel: 1 }, output: 'mithrilArmor', rate: 0.05, slots: 2, cost: { bricks: 15, steel: 10 } },
};
export const WORKSHOP_IDS = Object.keys(WORKSHOPS) as WorkshopKind[];

// ---- Yerleşim yapıları ----
export type CivicKind = 'hut' | 'house' | 'stonehouse' | 'tavern' | 'market' | 'temple' | 'palisade' | 'stonewall' | 'castle' | 'library' | 'mint' | 'guild' | 'unique' | 'wonder';
export interface CivicDef { kind: CivicKind; name: string; tech?: string; cost: Stock; work: number; housing?: number; max?: number }
export const CIVICS: Record<CivicKind, CivicDef> = {
  hut: { kind: 'hut', name: 'Kulübe', cost: { wood: 4 }, work: 4, housing: 3 },
  house: { kind: 'house', name: 'Ev', tech: 'woodwork', cost: { planks: 4, wood: 2 }, work: 8, housing: 6 },
  stonehouse: { kind: 'stonehouse', name: 'Taş Konak', tech: 'architecture', cost: { bricks: 4, stone: 4, tools: 1 }, work: 14, housing: 11 },
  tavern: { kind: 'tavern', name: 'Taverna', tech: 'tavern', cost: { planks: 10, stone: 8 }, work: 20, max: 1 },
  market: { kind: 'market', name: 'Pazar Yeri', tech: 'barter', cost: { wood: 15, stone: 6 }, work: 12, max: 1 },
  temple: { kind: 'temple', name: 'Sunak', tech: 'faith', cost: { stone: 10, wood: 6 }, work: 10, max: 1 },
  palisade: { kind: 'palisade', name: 'Ahşap Palisat', tech: 'woodwork', cost: { wood: 35 }, work: 20, max: 1 },
  stonewall: { kind: 'stonewall', name: 'Taş Sur', tech: 'stonewalls', cost: { stone: 60, tools: 2 }, work: 40, max: 1 },
  castle: { kind: 'castle', name: 'Kale', tech: 'castle', cost: { stone: 120, steel: 10, tools: 4 }, work: 80, max: 1 },
  library: { kind: 'library', name: 'Kütüphane', tech: 'writing', cost: { planks: 10, bricks: 8 }, work: 18, max: 1 },
  mint: { kind: 'mint', name: 'Darphane', tech: 'currency', cost: { bricks: 10, stone: 10 }, work: 18, max: 1 },
  guild: { kind: 'guild', name: 'Lonca Salonu', tech: 'guilds', cost: { bricks: 12, planks: 8, tools: 2 }, work: 22, max: 1 },
  unique: { kind: 'unique', name: 'Sınıf yapısı', cost: { stone: 20, planks: 10 }, work: 25, max: 1 },
  wonder: { kind: 'wonder', name: 'Harika', cost: { stone: 110, bricks: 30, gold: 90, tools: 8 }, work: 200, max: 1 },
};
