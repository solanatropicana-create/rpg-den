// Araştırma ağaçları: 38 düğümlü ana ağaç + sınıf ağaçları (GDD v0.3).
import type { ClassId } from './classes';

export type Era = 1 | 2 | 3 | 4;
export const ERA_TR = ['', 'Kamp', 'Köy', 'Kasaba', 'Krallık'];
export const ERA_ROMAN = ['', 'I', 'II', 'III', 'IV'];
export const ERA_RULE: Record<2 | 3 | 4, { nodes: number; pop: number }> = {
  2: { nodes: 5, pop: 12 },
  3: { nodes: 6, pop: 32 },
  4: { nodes: 6, pop: 75 },
};
export const ERA_COST: Record<Era, [number, number]> = { 1: [30, 50], 2: [80, 120], 3: [180, 250], 4: [350, 500] };

/** Sayısal etkiler; aynı anahtarlar toplanır. */
export interface Effects {
  prodAll?: number; prodFood?: number; prodWood?: number; prodMine?: number; prodHerbs?: number; prodMana?: number;
  research?: number; growth?: number; tradeGold?: number; tax?: number;
  soldierAc?: number; soldierAtk?: number; soldierDmg?: number; soldierHp?: number; defAc?: number;
  smite?: number;          // kötülere ve canavarlara kritik çarpanı
  healBack?: number;       // savaş sonrası ölülerin geri dönen oranı
  noRout?: number;         // birlikler bozguna uğramaz
  enemyMorale?: number;    // düşman moral eşiği düşer
  raidEvade?: number;      // kervan baskından kaçma şansı
  lootMult?: number;       // yağma çarpanı
  lootTech?: number;       // yağmada araştırma çalma şansı
  winterImmune?: number;
  forestGrow?: number;     // her yıl büyüyen orman hex sayısı
  forestMove?: number;
  heroLevel?: number;      // kendi sınıfından kahramanlar +seviye
  spy?: number;            // yıllık bilgi çalma şansı
  blackMarket?: number;
  divine?: number;         // felakette yıllık müdahale şansı
  luck?: number;           // yılda bir başarısızlığı başarıya çevir
  teleport?: number;       // kendi yerleşimleri arası anında taşıma
  cheapKnown?: number;     // başkalarının bildiği düğümler ucuz
  deathTouch?: number;     // suikast
  festival?: number;       // yıllık bayram gücü
  firstStrike?: number;    // ilk tur hasar çarpanı
  harvest?: number;
  noFamine?: number;
  foresee?: number;        // baskınları önceden sezme
  meteor?: number;
  shield?: number;         // başkent kalkanı
  revive?: number;         // kahraman diriltme
  avatar?: number;
  crusade?: number;
  puppet?: number;
  legendSteal?: number;
  warband?: number;        // yerleşim başına ek savaşçı kotası
  charm?: number;          // ozan: ilişkiler, göç, festival geliri
  frugal?: number;         // keşiş: gıda tüketimi azalır
  wild?: number;           // kan büyücüsü: yıllık büyü dalgası
  favoredHunt?: number;    // korucu: canavar kamplarına sefer eşiği düşer
  unarmed?: number;        // keşiş: silahsız asker yetiştirir
  pact?: number;           // paktçı: yıllık patron hediyesi ve bedeli
}

export type UnitId = 'holyguard' | 'knight' | 'wolves' | 'treant' | 'golem' | 'raider' | 'blessed' | 'skald' | 'legionary' | 'dragonguard' | 'monkwarrior' | 'warden' | 'flameborn' | 'shadowguard' | 'imp' | 'fiend';

export interface TechDef {
  id: string;
  name: string;
  era: Era;
  tree: 'main' | ClassId;
  req: string[];
  gate?: string[];           // kaynak kapısı: hepsi gerekli
  gateAny?: string[];        // biri yeterli
  unlock: string;
  eff?: Effects;
  unit?: UnitId;
  subclass?: boolean;        // bu düğüm alt sınıf seçtirir
  capstone?: boolean;
  chain?: 'gida' | 'metal' | 'yapi' | 'ticaret' | 'bilgi';
  cost?: number;
}

const T = (id: string, name: string, era: Era, req: string[], unlock: string, chain: TechDef['chain'], extra: Partial<TechDef> = {}): TechDef =>
  ({ id, name, era, tree: 'main', req, unlock, chain, ...extra });

export const MAIN_TECHS: TechDef[] = [
  // I. Kamp
  T('agriculture', 'Tarım', 1, [], 'Tarla (L1)', 'gida', { gate: ['fertile'] }),
  T('husbandry', 'Hayvancılık', 1, [], 'Ağıl, deri, av köşkü', 'gida'),
  T('fishing', 'Balıkçılık', 1, [], 'İskele', 'gida', { gate: ['water'] }),
  T('woodwork', 'Ahşap İşçiliği', 1, [], 'Ev, bıçkıhane, palisat', 'yapi'),
  T('stonework', 'Taş İşçiliği', 1, [], 'Taş ocağı', 'metal'),
  T('herbalism', 'Otacılık', 1, [], 'Toplayıcı kulübesi', 'bilgi'),
  T('faith', 'İnanç', 1, [], 'Sunak, küçük mutluluk', 'bilgi', { eff: { growth: 0.05 } }),
  T('pottery', 'Çömlekçilik', 1, ['agriculture'], 'Depo, fırın, tuğla', 'ticaret', { gate: ['clay'] }),
  T('mining', 'Madencilik', 1, ['stonework'], 'Açık ocak (L1)', 'metal'),
  // II. Köy
  T('bronze', 'Bronz Döküm', 2, ['mining'], 'Dökümhane, bronz', 'metal', { gate: ['copper', 'tin'] }),
  T('bronzetools', 'Bronz Aletler', 2, ['bronze', 'woodwork'], 'Aletçi; maden L2', 'metal', { eff: { prodAll: 0.1 } }),
  T('irrigation', 'Sulama', 2, ['agriculture', 'pottery'], 'Tarla L2', 'gida', { gate: ['water'] }),
  T('roads', 'Yol Yapımı', 2, ['woodwork', 'stonework'], 'Yol, yeni yerleşim', 'yapi'),
  T('barter', 'Takas', 2, ['pottery'], 'Pazar yeri, basit ticaret', 'ticaret'),
  T('tavern', 'Tavernacılık', 2, ['agriculture', 'barter'], 'Taverna, bira; kahramanlar', 'ticaret'),
  T('training', 'Talim', 2, ['bronze'], 'Asker birliği, silahhane', 'yapi'),
  T('writing', 'Yazı', 2, ['faith', 'pottery'], 'Kütüphane (+araştırma)', 'bilgi', { eff: { research: 0.15 } }),
  T('riding', 'Binicilik', 2, ['husbandry'], 'Haras, hızlı kervan', 'gida', { gate: ['horses'] }),
  T('arcana1', 'Arcana I', 2, ['writing'], 'Mana kristali görünür; kristal kazısı', 'bilgi'),
  // III. Kasaba
  T('smithing', 'Demircilik', 3, ['bronzetools'], 'Demirci ocağı, çelik', 'metal', { gate: ['iron', 'coal'] }),
  T('architecture', 'Mimarlık', 3, ['stonework', 'writing'], 'Kesme taş, taş konak', 'yapi'),
  T('currency', 'Para', 3, ['barter', 'writing'], 'Darphane, sikke', 'ticaret', { gate: ['gold'], eff: { tax: 0.5 } }),
  T('caravans', 'Kervancılık', 3, ['roads', 'barter'], 'Uzun mesafe ticaret yolu', 'ticaret', { eff: { tradeGold: 0.3 } }),
  T('medicine', 'Hekimlik', 3, ['herbalism', 'writing'], 'Şifa evi, iksir', 'bilgi', { gate: ['herbs'], eff: { growth: 0.1 } }),
  T('guilds', 'Lonca Sistemi', 3, ['currency', 'bronzetools'], 'Loncalar (+verim)', 'ticaret', { eff: { prodAll: 0.1 } }),
  T('rotation', 'Nöbetleşe Ekim', 3, ['irrigation', 'husbandry'], 'Çiftlik (L3)', 'gida'),
  T('forestry', 'Ormancılık', 3, ['woodwork', 'rotation'], 'Orman L3, hızlı yenilenme', 'gida'),
  T('stonewalls', 'Taş Surlar', 3, ['architecture', 'training'], 'Taş sur, gözcü kulesi', 'yapi'),
  T('shipbuilding', 'Gemicilik', 3, ['fishing', 'woodwork'], 'Liman, nehir taşımacılığı', 'gida', { gate: ['water'] }),
  T('arcana2', 'Arcana II', 3, ['arcana1', 'architecture'], 'Kristal kulesi, parşömen', 'bilgi', { gate: ['mana'] }),
  // IV. Krallık
  T('deepmine', 'Derin Kazı', 4, ['smithing', 'architecture'], 'Maden L3; mithril görünür', 'metal'),
  T('enchanting', 'Efsunlama', 4, ['arcana2', 'smithing'], 'Efsunlu silah', 'bilgi', { gate: ['mana'] }),
  T('castle', 'Kale Yapımı', 4, ['stonewalls'], 'Kale', 'yapi', { eff: { defAc: 2 } }),
  T('siege', 'Kuşatma Makineleri', 4, ['smithing', 'architecture'], 'Mancınık, koçbaşı', 'yapi'),
  T('crown', 'Taç ve Kanun', 4, ['currency', 'guilds'], 'Saray, yönetim bonusu', 'ticaret', { eff: { prodAll: 0.1, tax: 0.3 } }),
  T('tradenet', 'Ticaret Ağları', 4, ['caravans', 'guilds'], 'Banka, yabancı şube', 'ticaret', { eff: { tradeGold: 0.5 } }),
  T('mithrilwork', 'Mithril İşçiliği', 4, ['deepmine', 'enchanting'], 'Mithril zırh', 'metal', { gate: ['mithril'] }),
  T('highmagic', 'Yüksek Büyü', 4, ['enchanting'], 'Ley odak taşı, anıt büyüler', 'bilgi', { eff: { research: 0.2 } }),
];

const C = (tree: ClassId, id: string, name: string, era: Era, req: string[], unlock: string, extra: Partial<TechDef> = {}): TechDef =>
  ({ id: `${tree}_${id}`, name, era, tree, req, unlock, ...extra });

export const CLASS_TECHS: TechDef[] = [
  // Paladin
  C('paladin', 'oath', 'Yemin Töreni', 2, ['faith'], 'Yemin Tapınağı; alt sınıf seçimi', { subclass: true, eff: { heroLevel: 1 } }),
  C('paladin', 'guard', 'Kutsal Muhafız', 2, ['training'], 'Benzersiz birim: zırhlı muhafız', { unit: 'holyguard' }),
  C('paladin', 'smite', 'İlahi Çarpış', 3, ['paladin_guard', 'smithing'], 'Kötü ve canavara kritik ×3', { eff: { smite: 3 } }),
  C('paladin', 'layhands', 'Şifa Eli', 3, ['medicine'], 'Ölülerin %30\'u iyileşip döner', { eff: { healBack: 0.3 } }),
  C('paladin', 'aura', 'Koruma Aurası', 3, ['stonewalls'], 'Savunuculara AC +2', { eff: { defAc: 2 } }),
  C('paladin', 'knights', 'Şövalye Tarikatı', 4, ['riding', 'castle'], 'Benzersiz birim: ağır süvari', { unit: 'knight' }),
  C('paladin', 'crusade', 'Kutsal Sefer', 4, ['crown'], 'Kötülere karşı ittifak çağrısı', { eff: { crusade: 1 } }),
  C('paladin', 'cap', 'Yemin Doruğu', 4, ['paladin_crusade'], 'Alt sınıfa göre uç güç', { capstone: true }),
  // Rahip (Örs ve diğer alanlar)
  C('cleric', 'temple', 'Tapınak Ocağı', 2, ['faith'], 'Tapınak ve yıllık bayram; alt sınıf seçimi', { subclass: true, eff: { festival: 1 } }),
  C('cleric', 'bless', 'Kutsama', 2, ['training'], 'Askerlere AC +1', { eff: { soldierAc: 1 }, unit: 'blessed' }),
  C('cleric', 'school', 'Rahip Okulu', 3, ['writing'], 'Rahip kahramanlar +1 seviye', { eff: { heroLevel: 1 } }),
  C('cleric', 'holyarms', 'Kutsal Silahlar', 3, ['smithing'], 'Silah kalitesi +1', { eff: { soldierAtk: 1 } }),
  C('cleric', 'turn', 'Ölüleri Kovma', 3, ['cleric_temple'], 'Canavarlara karşı üstünlük', { eff: { smite: 2 } }),
  C('cleric', 'cathedral', 'Dağ Katedrali', 4, ['architecture', 'deepmine'], 'Anıt; dağ içi şehir', { eff: { defAc: 2, growth: 0.1 } }),
  C('cleric', 'warden', 'Kutsal Koruyucu', 4, ['castle'], 'Başkent savunucuları yılmaz', { eff: { noRout: 1 } }),
  C('cleric', 'cap', 'İlahi Müdahale', 4, ['cleric_cathedral'], 'Felakette tanrı müdahalesi', { capstone: true, eff: { divine: 0.1 } }),
  // Druid
  C('druid', 'grove', 'Kutsal Koru', 2, ['faith'], 'Koru; orman büyür; alt sınıf seçimi', { subclass: true, eff: { forestGrow: 1 } }),
  C('druid', 'beasts', 'Hayvan Dostluğu', 2, ['husbandry'], 'Benzersiz birim: kurt ve ayı yoldaşlar', { unit: 'wolves' }),
  C('druid', 'growth', 'Bitki Büyümesi', 3, ['irrigation'], 'Tarım ×1,5', { eff: { prodFood: 0.5 } }),
  C('druid', 'treewalk', 'Ağaç Yürüyüşü', 3, ['roads'], 'Ormanlarda yol hızı', { eff: { forestMove: 1 } }),
  C('druid', 'seasons', 'Mevsim Ritleri', 3, ['medicine'], 'Kış düşüşü yok', { eff: { winterImmune: 1 } }),
  C('druid', 'treant', 'Treant Uyanışı', 4, ['druid_seasons'], 'Benzersiz birim: treant', { unit: 'treant', gate: ['heartwood'] }),
  C('druid', 'wrath', 'Doğanın Öfkesi', 4, ['druid_treant'], 'Orman düşmana saldırır', { eff: { defAc: 2 } }),
  C('druid', 'cap', 'Arşidruid', 4, ['druid_wrath'], 'Alt sınıfa göre uç güç', { capstone: true }),
  // Haydut
  C('rogue', 'guild', 'Hırsızlar Loncası', 2, ['barter'], 'Lonca; altın geliri +; alt sınıf seçimi', { subclass: true, eff: { tax: 0.3 } }),
  C('rogue', 'smuggle', 'Kaçakçı Yolları', 2, ['roads'], 'Kervanlar baskına yarı oranda yakalanır', { eff: { raidEvade: 0.5, tradeGold: 0.2 } }),
  C('rogue', 'spies', 'Casus Ağı', 3, ['writing'], 'Komşu bilgisi; bilgi çalma', { eff: { spy: 0.25 } }),
  C('rogue', 'blackmarket', 'Kara Pazar', 3, ['currency'], 'Savaşta bile ticaret', { eff: { blackMarket: 1, tradeGold: 0.2 } }),
  C('rogue', 'locks', 'Kilit Ustalığı', 3, ['rogue_guild'], 'Kuşatmada surları aşma', { eff: { soldierAtk: 1 } }),
  C('rogue', 'council', 'Gölge Konseyi', 4, ['crown'], 'Başka medeniyetlerde gizli etki', { eff: { spy: 0.25 } }),
  C('rogue', 'escape', 'Kaçış Sanatı', 4, ['rogue_spies'], 'Kayıplar yarıya iner', { eff: { healBack: 0.5 } }),
  C('rogue', 'cap', 'Talihin Dokunuşu', 4, ['rogue_council'], 'Yılda bir başarısızlık başarıya döner', { capstone: true, eff: { luck: 1 } }),
  // Sihirbaz
  C('wizard', 'academy', 'Akademi', 2, ['writing'], 'Araştırma +%15; alt sınıf seçimi', { subclass: true, eff: { research: 0.15 } }),
  C('wizard', 'scrolls', 'Parşömen Yazımı', 2, ['arcana1'], 'Parşömen yarı maliyete', { eff: { prodMana: 0.5 } }),
  C('wizard', 'tower', 'Büyücü Kulesi', 3, ['arcana2'], 'Savunma kulesi ve araştırma', { eff: { research: 0.15, defAc: 1 } }),
  C('wizard', 'teleport', 'Işınlanma Çemberi', 3, ['caravans'], 'Yerleşimler arası anında taşıma', { eff: { teleport: 1 } }),
  C('wizard', 'tools', 'Efsunlu Aletler', 3, ['guilds'], 'Tüm çıkarma verimi +%20', { eff: { prodAll: 0.2 } }),
  C('wizard', 'golems', 'Golem Muhafızlar', 4, ['enchanting'], 'Benzersiz birim: golem', { unit: 'golem' }),
  C('wizard', 'council', 'Başbüyücüler Konseyi', 4, ['highmagic'], 'Bilinen düğümler yarı maliyete', { eff: { cheapKnown: 0.5 } }),
  C('wizard', 'cap', 'Büyü Ustalığı', 4, ['wizard_council'], 'Alt sınıfa göre uç güç', { capstone: true }),
  // Barbar
  C('barbarian', 'totem', 'Totem Direği', 2, ['faith'], 'Savaş çadırı; alt sınıf seçimi', { subclass: true, eff: { soldierDmg: 1 } }),
  C('barbarian', 'raiders', 'Akıncılar', 2, ['husbandry'], 'Benzersiz birim: hızlı baskın birliği', { unit: 'raider' }),
  C('barbarian', 'plunder', 'Ganimet Payı', 3, ['barbarian_raiders'], 'Yağmada altın ×2, bilgi çalma', { eff: { lootMult: 1, lootTech: 0.25 } }),
  C('barbarian', 'union', 'Kabile Birliği', 3, ['barbarian_totem'], 'Fethedilen halk isyansız katılır', { eff: { growth: 0.1 } }),
  C('barbarian', 'fearless', 'Korkusuz', 3, ['training'], 'Birlikler bozguna uğramaz', { eff: { noRout: 1 } }),
  C('barbarian', 'confed', 'Kabile Konfederasyonu', 4, ['barbarian_union'], 'Ek savaşçı kotası', { eff: { warband: 0.08 } }),
  C('barbarian', 'dread', 'Dehşet Varlığı', 4, ['barbarian_fearless'], 'Düşman moral eşiği düşer', { eff: { enemyMorale: 0.2 } }),
  C('barbarian', 'cap', 'İlkel Şampiyon', 4, ['barbarian_confed'], 'Alt sınıfa göre uç güç', { capstone: true }),
  // Ozan
  C('bard', 'hall', 'Ozanlar Salonu', 2, ['tavern'], 'Festivaller; alt sınıf seçimi', { subclass: true, eff: { charm: 0.5 } }),
  C('bard', 'songs', 'Kahramanlık Destanları', 2, ['faith'], 'Kahramanlar +1 seviye', { eff: { heroLevel: 1 } }),
  C('bard', 'skalds', 'Savaş Ozanları', 3, ['training'], 'Benzersiz birim: savaş ozanı', { unit: 'skald' }),
  C('bard', 'lore', 'Kayıp Ezgiler', 3, ['writing'], 'Araştırma +%10', { eff: { research: 0.1 } }),
  C('bard', 'envoys', 'Elçi Ozanlar', 3, ['caravans'], 'Komşular yumuşar, ticaret altını +', { eff: { charm: 1, tradeGold: 0.2 } }),
  C('bard', 'theatre', 'Büyük Tiyatro', 4, ['architecture'], 'Göç çeker, nüfus artışı +', { eff: { growth: 0.15 } }),
  C('bard', 'legend', 'Yaşayan Efsane', 4, ['bard_envoys'], 'Yılda bir başarısızlık başarıya döner', { eff: { luck: 1 } }),
  C('bard', 'cap', 'Kusursuz Ezgi', 4, ['bard_legend'], 'Alt sınıfa göre uç güç', { capstone: true }),
  // Savaşçı
  C('fighter', 'legion', 'Lejyon Kışlası', 2, ['stonework'], 'Benzersiz birim: lejyoner; alt sınıf seçimi', { subclass: true, unit: 'legionary' }),
  C('fighter', 'drill', 'Sert Talim', 2, ['bronze'], 'Askerlere saldırı +1', { eff: { soldierAtk: 1 } }),
  C('fighter', 'breath', 'Ejder Nefesi', 3, ['fighter_legion'], 'İlk tur hasar ×1,3', { eff: { firstStrike: 1.3 } }),
  C('fighter', 'formation', 'Kalkan Duvarı', 3, ['stonewalls'], 'Savunuculara AC +2', { eff: { defAc: 2 } }),
  C('fighter', 'armory', 'Lejyon Cephaneliği', 3, ['smithing'], 'Zırh +1, maden verimi +%10', { eff: { soldierAc: 1, prodMine: 0.1 } }),
  C('fighter', 'dragonguard', 'Ejder Muhafızları', 4, ['fighter_breath', 'castle'], 'Benzersiz birim: ejder muhafızı', { unit: 'dragonguard' }),
  C('fighter', 'warlord', 'Savaş Lordu', 4, ['siege'], 'Ek savaşçı kotası, hasar +1', { eff: { warband: 0.05, soldierDmg: 1 } }),
  C('fighter', 'cap', 'Aksiyon Dalgası', 4, ['fighter_warlord'], 'Alt sınıfa göre uç güç', { capstone: true }),
  // Keşiş
  C('monk', 'monastery', 'Manastır', 2, ['faith'], 'Araştırma +%10; alt sınıf seçimi', { subclass: true, eff: { research: 0.1 } }),
  C('monk', 'fasting', 'Oruç ve Sabır', 2, ['herbalism'], 'Tüketim −%15, kış düşüşü yok', { eff: { frugal: 0.15, winterImmune: 1 } }),
  C('monk', 'fists', 'Yumruk Keşişleri', 3, ['monk_monastery'], 'Benzersiz birim; silahsız asker yetiştirir', { unit: 'monkwarrior', eff: { unarmed: 1 } }),
  C('monk', 'calm', 'İç Huzur', 3, ['medicine'], 'Nüfus artışı +, yaralılar döner', { eff: { growth: 0.1, healBack: 0.2 } }),
  C('monk', 'scriptorium', 'Yazıhane', 3, ['writing'], 'Araştırma +%15', { eff: { research: 0.15 } }),
  C('monk', 'wind', 'Rüzgâr Adımı', 4, ['monk_fists'], 'Kervanlar baskından kaçar', { eff: { raidEvade: 0.3, forestMove: 1 } }),
  C('monk', 'diamond', 'Elmas Ruh', 4, ['monk_calm'], 'Birlikler bozguna uğramaz, AC +2', { eff: { noRout: 1, soldierAc: 2 } }),
  C('monk', 'cap', 'Kusursuz Beden', 4, ['monk_diamond'], 'Alt sınıfa göre uç güç', { capstone: true }),
  // Korucu
  C('ranger', 'lodge', 'Korucu Locası', 2, ['husbandry'], 'Av verimi +; alt sınıf seçimi', { subclass: true, eff: { prodFood: 0.1 } }),
  C('ranger', 'tracks', 'İz Sürme', 2, ['roads'], 'Kervanlar gizli yollardan gider', { eff: { raidEvade: 0.2, forestMove: 1 } }),
  C('ranger', 'wardens', 'Sınır Muhafızları', 3, ['ranger_lodge', 'training'], 'Benzersiz birim: sınır muhafızı', { unit: 'warden' }),
  C('ranger', 'bounty', 'Canavar Avı', 3, ['ranger_lodge'], 'Kamplara erken sefer; yıllık av', { eff: { favoredHunt: 1 } }),
  C('ranger', 'companions', 'Hayvan Yoldaşlar', 3, ['husbandry', 'ranger_lodge'], 'Benzersiz birim: kurt sürüsü', { unit: 'wolves' }),
  C('ranger', 'outposts', 'Gözcü Kuleleri', 4, ['stonewalls'], 'Savunma AC +2', { eff: { defAc: 2 } }),
  C('ranger', 'slayer', 'Dev Avcısı', 4, ['ranger_bounty', 'smithing'], 'Canavara kritik ×3', { eff: { smite: 2 } }),
  C('ranger', 'cap', 'Diyarın Bekçisi', 4, ['ranger_slayer'], 'Alt sınıfa göre uç güç', { capstone: true }),
  // Kan Büyücüsü
  C('sorcerer', 'bloodline', 'Kan Soyu Ayini', 2, ['faith'], 'Soy uyanır; alt sınıf seçimi', { subclass: true, eff: { research: 0.05 } }),
  C('sorcerer', 'embers', 'Kor Tılsımı', 2, ['pottery'], 'Askerlere hasar +1', { eff: { soldierDmg: 1 } }),
  C('sorcerer', 'flameborn', 'Alev Soylular', 3, ['sorcerer_bloodline', 'training'], 'Benzersiz birim: alev soylu', { unit: 'flameborn' }),
  C('sorcerer', 'metamagic', 'Metabüyü', 3, ['arcana1'], 'Araştırma +%15', { eff: { research: 0.15 } }),
  C('sorcerer', 'font', 'Büyü Pınarı', 3, ['sorcerer_bloodline'], 'Mana verimi +%50', { eff: { prodMana: 0.5, research: 0.05 } }),
  C('sorcerer', 'hellfire', 'Cehennem Ateşi', 4, ['sorcerer_flameborn'], 'Hasar +2, düşman morali düşer', { eff: { soldierDmg: 2, enemyMorale: 0.1 } }),
  C('sorcerer', 'crown', 'Kızıl Taç', 4, ['crown'], 'Vergi ve verim +', { eff: { tax: 0.3, prodAll: 0.1 } }),
  C('sorcerer', 'cap', 'Kanın Doruğu', 4, ['sorcerer_hellfire'], 'Alt sınıfa göre uç güç', { capstone: true }),
  // Paktçı
  C('warlock', 'pact', 'Pakt Ayini', 2, ['faith'], 'Patron seçilir; alt sınıf seçimi', { subclass: true, eff: { research: 0.05 } }),
  C('warlock', 'blast', 'Kadim Patlama', 2, ['stonework'], 'Askerlere hasar +1', { eff: { soldierDmg: 1 } }),
  C('warlock', 'shadows', 'Gölge Muhafızları', 3, ['warlock_pact', 'training'], 'Benzersiz birim: gölge muhafız', { unit: 'shadowguard' }),
  C('warlock', 'tome', 'Gölgeler Kitabı', 3, ['writing'], 'Araştırma +%15', { eff: { research: 0.15 } }),
  C('warlock', 'imps', 'Şeytancık Çağırma', 3, ['warlock_pact'], 'Benzersiz birim: şeytancık', { unit: 'imp' }),
  C('warlock', 'curse', 'Lanet', 4, ['warlock_tome'], 'Düşman morali düşer', { eff: { enemyMorale: 0.15 } }),
  C('warlock', 'gate', 'Karanlık Kapı', 4, ['warlock_imps', 'arcana2'], 'Benzersiz birim: kapı iblisi', { unit: 'fiend' }),
  C('warlock', 'cap', 'Patronun Seçilmişi', 4, ['warlock_gate'], 'Alt sınıfa göre uç güç', { capstone: true }),
];

export const ALL_TECHS = [...MAIN_TECHS, ...CLASS_TECHS];
export const TECH: Record<string, TechDef> = Object.fromEntries(ALL_TECHS.map((t) => [t.id, t]));

export function techCost(t: TechDef) {
  if (t.cost) return t.cost;
  const [lo, hi] = ERA_COST[t.era];
  // Deterministik: isim uzunluğuna göre aralıkta dağıt
  let h = 0; for (const ch of t.id) h = (h * 31 + ch.charCodeAt(0)) >>> 0;
  return Math.round(lo + (h % 1000) / 1000 * (hi - lo));
}
