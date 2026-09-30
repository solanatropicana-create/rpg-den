// D&D sınıflarına bağlı medeniyet arşetipleri (GDD v0.3).
import type { Good } from './goods';
import type { Effects, UnitId } from './techs';

export type ClassId = 'paladin' | 'cleric' | 'druid' | 'rogue' | 'wizard' | 'barbarian'
  | 'bard' | 'fighter' | 'monk' | 'ranger' | 'sorcerer' | 'warlock';

export type RaceId = 'human' | 'dwarf' | 'elf' | 'halfling' | 'gnome' | 'halfelf' | 'halforc' | 'dragonborn' | 'tiefling';

export interface RaceDef { id: RaceId; name: string; plural: string; growth: number; hp: number; atk: number }
export const RACES: Record<RaceId, RaceDef> = {
  human: { id: 'human', name: 'İnsan', plural: 'İnsanlar', growth: 1.0, hp: 0, atk: 0 },
  dwarf: { id: 'dwarf', name: 'Cüce', plural: 'Cüceler', growth: 0.76, hp: 3, atk: 0 },
  elf: { id: 'elf', name: 'Elf', plural: 'Elfler', growth: 0.6, hp: 0, atk: 1 },
  halfling: { id: 'halfling', name: 'Buçukluk', plural: 'Buçukluklar', growth: 0.95, hp: -2, atk: 0 },
  gnome: { id: 'gnome', name: 'Gnom', plural: 'Gnomlar', growth: 0.75, hp: -2, atk: 0 },
  halfelf: { id: 'halfelf', name: 'Yarı-elf', plural: 'Yarı-elfler', growth: 0.8, hp: 0, atk: 0 },
  halforc: { id: 'halforc', name: 'Yarı-ork', plural: 'Yarı-orklar', growth: 1.05, hp: 2, atk: 1 },
  dragonborn: { id: 'dragonborn', name: 'Ejderdoğan', plural: 'Ejderdoğanlar', growth: 0.82, hp: 2, atk: 1 },
  tiefling: { id: 'tiefling', name: 'Tiefling', plural: 'Tieflingler', growth: 0.8, hp: 0, atk: 0 },
};

export interface SubclassDef { id: string; name: string; desc: string; eff: Effects; capName: string; capDesc: string; capEff: Effects; pick: (ctx: PickCtx) => number }
export interface PickCtx { war: boolean; threat: number; forest: number; law: number; good: number; pop: number }

export interface ClassDef {
  id: ClassId;
  name: string;          // Türkçe
  dnd: string;           // D&D adı
  race: RaceId;
  align: { law: number; good: number };
  feature: string;       // sınıf özelliği adı
  featureDesc: string;
  base: Effects;         // sınıf özelliğinin sürekli etkisi
  desires: Good[];       // istediği kaynaklar
  heroClass: string;     // kahraman eğilimi
  civName: string;
  capital: string;
  towns: string[];
  color: string;
  // araştırma eğilimi: zincirlere ağırlık
  prefer: Partial<Record<'gida' | 'metal' | 'yapi' | 'ticaret' | 'bilgi' | 'class', number>>;
  terrainLike: Partial<Record<string, number>>;
  aggression: number;    // 0..1, savaş/baskın eğilimi
  subclasses: SubclassDef[];
  implemented: boolean;
}

const S = (id: string, name: string, desc: string, eff: Effects, capName: string, capDesc: string, capEff: Effects, pick: SubclassDef['pick']): SubclassDef =>
  ({ id, name, desc, eff, capName, capDesc, capEff, pick });

export const CLASSES: Record<ClassId, ClassDef> = {
  paladin: {
    id: 'paladin', name: 'Paladin', dnd: 'Paladin', race: 'human', align: { law: 0.9, good: 0.8 },
    feature: 'Kutsal Yemin', featureDesc: 'Antlaşma bozmaz; kötülere ve canavarlara karşı güçlü', base: { smite: 1.5, growth: 0.05 },
    desires: ['horses', 'iron'], heroClass: 'paladin', civName: 'Güneştacı Krallığı', capital: 'Altınkapı',
    towns: ['Şafaktepe', 'Kalkanova', 'Yeminköprü', 'Akburç', 'Işıkdere'], color: '#b5871a',
    prefer: { yapi: 1.4, gida: 1.1, metal: 1.1, class: 1.3 }, terrainLike: { grass: 1.5, forest: 0.5, hill: 0.8 }, aggression: 0.25,
    subclasses: [
      S('devotion', 'Adanma Yemini', 'Antlaşmalar ve moral güçlü', { defAc: 1, growth: 0.05 }, 'Kutsal Nimbus', 'Başkente saldıran kötüler hasar alır', { defAc: 3, shield: 1 }, (c) => 1 + c.law),
      S('ancients', 'Kadim Yemin', 'Doğayla barış; elf ve druidlerle dostluk', { prodFood: 0.15 }, 'Kadim Şampiyon', 'Hasat +%50, doğa dostlarıyla ebedi dostluk', { prodFood: 0.5, harvest: 1 }, (c) => 0.6 + c.forest * 2),
      S('vengeance', 'İntikam Yemini', 'Düşük savaş eşiği, kin tutar', { soldierAtk: 1 }, 'İntikam Meleği', 'İlan edilen düşmana ordu hızı ve hasarı +%50', { soldierDmg: 3 }, (c) => 0.4 + (c.war ? 1.5 : 0) + c.threat),
    ], implemented: true,
  },
  cleric: {
    id: 'cleric', name: 'Rahip', dnd: 'Cleric', race: 'dwarf', align: { law: 0.8, good: 0.6 },
    feature: 'Kanalize İlahiyat', featureDesc: 'Yılda bir ilahi lütuf (bayram)', base: { festival: 1 },
    desires: ['iron', 'gold'], heroClass: 'cleric', civName: 'Örsyürek Tapınak Klanı', capital: 'Kutsalörs',
    towns: ['Demirçan', 'Taşkandil', 'Derinmihrap', 'Közkapı', 'Granitsunak'], color: '#3d7ab8',
    prefer: { metal: 1.6, bilgi: 1.1, yapi: 1.0, class: 1.2 }, terrainLike: { hill: 2, mountain: 1.5, grass: 0.6 }, aggression: 0.2,
    subclasses: [
      S('forge', 'Örs Alanı', 'Silah ve zırh üretimi ucuz ve kaliteli', { prodMine: 0.2, soldierAc: 1 }, 'Tanrının Örsü', 'Efsanevi silahlar dövülür', { soldierAtk: 2, soldierAc: 1 }, () => 1.6),
      S('war', 'Savaş Alanı', 'Askerlere ek saldırı', { soldierDmg: 2 }, 'Savaş Avatarı', 'Savaş alanında avatar belirir', { avatar: 1, soldierAtk: 2 }, (c) => 0.5 + (c.war ? 1.5 : 0) + c.threat),
      S('life', 'Yaşam Alanı', 'Nüfus artışı +%25, iyileşme', { growth: 0.25, healBack: 0.2 }, 'Diriliş', 'Ölen kahraman dirilir', { revive: 1, growth: 0.1 }, () => 0.9),
    ], implemented: true,
  },
  druid: {
    id: 'druid', name: 'Druid', dnd: 'Druid', race: 'elf', align: { law: 0.0, good: 0.7 },
    feature: 'Doğa Dengesi', featureDesc: 'Orman kesmez, ormanı büyütür; ormanda savunma', base: { prodHerbs: 0.5, growth: 0.15 },
    desires: ['heartwood', 'herbs'], heroClass: 'druid', civName: 'Yeşilyaprak Çemberi', capital: 'Sessizkoru',
    towns: ['Yosunpınar', 'Geyikyurt', 'Çiyardı', 'Ayışığı Korusu', 'Kökbağ'], color: '#3f9e6b',
    prefer: { gida: 1.4, bilgi: 1.3, metal: 0.5, class: 1.3 }, terrainLike: { forest: 2, oldforest: 3, grass: 0.8 }, aggression: 0.1,
    subclasses: [
      S('land', 'Toprak Çemberi', 'Şifalı ot ve tarım verimi', { prodFood: 0.2, prodHerbs: 0.3 }, 'Hayat Ağacı', 'Başkent kıtlık yaşamaz', { noFamine: 1, prodFood: 0.3 }, () => 1.2),
      S('moon', 'Ay Çemberi', 'Druidler savaşta ayıya dönüşür', { soldierHp: 6, soldierDmg: 1 }, 'Sonsuz Şekil', 'Druid birlikleri ölümde bir kez dirilir', { healBack: 0.5 }, (c) => 0.5 + c.threat + (c.war ? 1 : 0)),
      S('stars', 'Yıldızlar Çemberi', 'Kehanet, araştırma +', { research: 0.15 }, 'Takımyıldız Kehaneti', 'Savaş ve baskınları önceden sezer', { foresee: 1, research: 0.1 }, () => 0.9),
    ], implemented: true,
  },
  rogue: {
    id: 'rogue', name: 'Haydut', dnd: 'Rogue', race: 'halfling', align: { law: 0.0, good: 0.0 },
    feature: 'Sinsi Hamle', featureDesc: 'Casusluk, kaçakçılık, bilgi çalma', base: { raidEvade: 0.2, tradeGold: 0.3 },
    desires: ['salt', 'gold'], heroClass: 'rogue', civName: 'Tatlıçayır Loncası', capital: 'Kavşakpazar',
    towns: ['Fıçıköy', 'Gölgeçarşı', 'Balköprü', 'Keseli', 'Kırkkapı'], color: '#c9722b',
    prefer: { ticaret: 1.7, gida: 1.1, bilgi: 1.0, class: 1.2 }, terrainLike: { grass: 1.5, water: 1.2 }, aggression: 0.15,
    subclasses: [
      S('thief', 'Hırsız', 'Yağma ve kaynak çalma', { lootMult: 0.5 }, 'Usta Hırsız', 'Efsanevi eşya çalar', { legendSteal: 1, lootMult: 0.5 }, () => 1),
      S('assassin', 'Suikastçı', 'Rakip kahramanlara suikast', { deathTouch: 0.1 }, 'Ölüm Vuruşu', 'Bir lideri ya da kahramanı tek hamlede düşürür', { deathTouch: 0.3 }, (c) => 0.5 + (c.war ? 1.2 : 0)),
      S('mastermind', 'Entrikacı', 'Komşuları birbirine düşürür', { spy: 0.15 }, 'Gölge Kral', 'Bir medeniyeti kukla yapar', { puppet: 1 }, () => 1),
    ], implemented: true,
  },
  wizard: {
    id: 'wizard', name: 'Sihirbaz', dnd: 'Wizard', race: 'gnome', align: { law: 0.1, good: 0.6 },
    feature: 'Büyü Kitabı', featureDesc: '+%25 araştırma, bilgi biriktirir', base: { research: 0.25 },
    desires: ['mana', 'leather'], heroClass: 'wizard', civName: 'Çarkyıldız Akademisi', capital: 'Pusulakule',
    towns: ['Dişlivadi', 'Mürekkeptepe', 'Kristalköy', 'Parşömenli', 'Yıldızgöz'], color: '#8a5cc2',
    prefer: { bilgi: 1.8, metal: 1.0, ticaret: 1.0, class: 1.3 }, terrainLike: { hill: 1.3, swamp: 1.2, grass: 1 }, aggression: 0.1,
    subclasses: [
      S('evocation', 'Yıkım Okulu', 'Büyücü birliklerine alan hasarı', { soldierDmg: 2 }, 'Meteor Fırtınası', 'Savaş alanına meteor', { meteor: 1 }, (c) => 0.6 + c.threat + (c.war ? 1 : 0)),
      S('abjuration', 'Koruma Okulu', 'Yerleşim kalkanları', { defAc: 2 }, 'Mutlak Kalkan', 'Başkent uzun süre fethedilemez', { shield: 1, defAc: 2 }, () => 1),
      S('divination', 'Kehanet Okulu', 'Olayları önceden görme, ticarette avantaj', { research: 0.1, tradeGold: 0.2 }, 'Kader Dokuması', 'Yılda bir kötü olayı engeller', { luck: 1, foresee: 1 }, () => 1.1),
    ], implemented: true,
  },
  barbarian: {
    id: 'barbarian', name: 'Barbar', dnd: 'Barbarian', race: 'halforc', align: { law: -0.8, good: -0.1 },
    feature: 'Öfke', featureDesc: 'Savaşta hasar ve moral; yağma ekonomisi', base: { soldierDmg: 2, research: -0.25, lootMult: 0.5 },
    desires: ['horses', 'meat'], heroClass: 'barbarian', civName: 'Kanlıdiş Kabileleri', capital: 'Kemikçadır',
    towns: ['Kurtoba', 'Savaşçukur', 'Kızılyurt', 'Toynakbaş', 'Kafatepe'], color: '#b23b30',
    prefer: { gida: 1.3, yapi: 1.3, metal: 1.1, bilgi: 0.6, class: 1.4 }, terrainLike: { grass: 1.6, tundra: 1.2, hill: 1 }, aggression: 0.8,
    subclasses: [
      S('berserker', 'Çılgın Yolu', 'Hasar çok yüksek, kayıplar da', { soldierDmg: 3, soldierAc: -1 }, 'Kan Fırtınası', 'İlk tur hasar ×2', { firstStrike: 2 }, (c) => 0.8 + (c.war ? 1 : 0)),
      S('totem', 'Totem Yolu', 'Ayı dayanıklılığı, kurt sürüsü, kartal gözü', { soldierHp: 4 }, 'Ruh Totemi', 'Hayvan gücü tüm ordulara', { soldierHp: 6, soldierAtk: 1 }, () => 1),
      S('ancestral', 'Ata Muhafızı', 'Ata ruhları savunmada korur', { defAc: 2 }, 'Atalarının Kalkanı', 'Savunmada kayıplar yarıya', { healBack: 0.5, defAc: 1 }, (c) => 0.6 + c.threat),
    ], implemented: true,
  },
  bard: {
    id: 'bard', name: 'Ozan', dnd: 'Bard', race: 'halfelf', align: { law: -0.4, good: 0.6 },
    feature: 'İlham', featureDesc: 'Şarkılarıyla komşuları yumuşatır, göç çeker, festivallerle altın toplar', base: { charm: 1, tradeGold: 0.15 },
    desires: ['gold', 'beer'], heroClass: 'rogue', civName: 'Lirsesi Şehirleri', capital: 'Nağmeköy',
    towns: ['Tamburlu', 'Kavalpınar', 'Şarkıdere', 'Neşeliova', 'Telliköprü'], color: '#d0668a',
    prefer: { ticaret: 1.5, bilgi: 1.3, gida: 1.0, class: 1.3 }, terrainLike: { grass: 1.6, water: 1.3, forest: 0.8 }, aggression: 0.1,
    subclasses: [
      S('lore', 'Bilgi Koleji', 'Kayıp ezgilerden bilgi; araştırma +', { research: 0.15, cheapKnown: 0.25 }, 'Sözün Gücü', 'Başkalarının bildiği her şey neredeyse bedava', { research: 0.2, cheapKnown: 0.25 }, () => 1.1),
      S('valor', 'Yiğitlik Koleji', 'Savaş marşları; askerlere ve kahramanlara güç', { soldierAtk: 1, heroLevel: 1 }, 'Destansı Marş', 'Ordular bozguna uğramaz, ilk hamle güçlü', { firstStrike: 1.5, noRout: 1 }, (c) => 0.5 + c.threat + (c.war ? 1 : 0)),
      S('eloquence', 'Belagat Koleji', 'Tatlı dil; ilişkiler ve ticaret', { charm: 1, tradeGold: 0.2 }, 'Altın Dil', 'Kimse bu şehirlere uzun süre düşman kalamaz', { charm: 2, tax: 0.3 }, () => 1),
    ], implemented: true,
  },
  fighter: {
    id: 'fighter', name: 'Savaşçı', dnd: 'Fighter', race: 'dragonborn', align: { law: 0.5, good: 0.0 },
    feature: 'Aksiyon Dalgası', featureDesc: 'Disiplinli lejyonlar, zırh ustalığı; yıllık turnuva', base: { soldierAc: 1, prodMine: 0.1 },
    desires: ['iron', 'coal'], heroClass: 'fighter', civName: 'Pulzırh Lejyonu', capital: 'Ejderkale',
    towns: ['Pulkalkan', 'Közburç', 'Kılıçyurt', 'Demirkanat', 'Alevgeçit'], color: '#b0703a',
    prefer: { metal: 1.5, yapi: 1.2, gida: 1.25, bilgi: 0.9, class: 1.3 }, terrainLike: { hill: 1.6, grass: 1.2, mountain: 0.8 }, aggression: 0.55,
    subclasses: [
      S('champion', 'Şampiyon', 'Kaba güç; saldırı ve hasar', { soldierAtk: 1, soldierDmg: 1 }, 'Yenilmez Şampiyon', 'Düşenlerin çoğu yeniden kalkar', { soldierAtk: 2, healBack: 0.3 }, () => 1),
      S('battlemaster', 'Savaş Ustası', 'Taktik; düşman morali kırılır, saflar dağılmaz', { enemyMorale: 0.1, noRout: 1 }, 'Kusursuz Taktik', 'Düşman ilk turda ezilir', { enemyMorale: 0.25, firstStrike: 1.5 }, (c) => 0.6 + c.law),
      S('eldritch', 'Büyülü Şövalye', 'Kılıç ve büyü; kalkan ve dayanıklılık', { defAc: 1, soldierHp: 2, research: 0.05 }, 'Büyü Kalkanı', 'Başkent büyülü kalkanla korunur', { shield: 1, soldierAc: 2 }, () => 0.8),
    ], implemented: true,
  },
  monk: {
    id: 'monk', name: 'Keşiş', dnd: 'Monk', race: 'human', align: { law: 0.6, good: 0.3 },
    feature: 'Disiplin', featureDesc: 'Az yer, çok düşünür; kıtlığa dayanıklı, silahsız savaşçılar', base: { frugal: 0.12, research: 0.05 },
    desires: ['herbs', 'stone'], heroClass: 'fighter', civName: 'Rüzgâr Manastırı', capital: 'Sessiztepe',
    towns: ['Çankule', 'Sisliyamaç', 'Dinginpınar', 'Taşbasamak', 'Bulutkapı'], color: '#7a8a50',
    prefer: { bilgi: 1.6, gida: 1.2, yapi: 1.0, metal: 0.7, class: 1.3 }, terrainLike: { hill: 1.8, mountain: 1.2, grass: 0.9 }, aggression: 0.1,
    subclasses: [
      S('openhand', 'Açık El Yolu', 'Çıplak elle savaş; saldırı ve savunma', { soldierAtk: 1, defAc: 1 }, 'Titreşen Avuç', 'Tek dokunuşla düşman önderi düşer', { deathTouch: 0.15, soldierAtk: 1 }, (c) => 0.6 + c.threat),
      S('shadow', 'Gölge Yolu', 'Sessiz adımlar; kervanlar görünmez, sırlar öğrenilir', { raidEvade: 0.3, spy: 0.15 }, 'Gölge Adımı', 'Gölgeler arasında anında yolculuk', { spy: 0.25, teleport: 1 }, () => 0.9),
      S('elements', 'Dört Element Yolu', 'Rüzgâr ve alev; askerlere hasar', { soldierDmg: 2 }, 'Element Ustası', 'Savaş alanına ateş yağar', { meteor: 1 }, (c) => 0.5 + (c.war ? 1 : 0) + c.threat),
    ], implemented: true,
  },
  ranger: {
    id: 'ranger', name: 'Korucu', dnd: 'Ranger', race: 'halfelf', align: { law: 0.0, good: 0.6 },
    feature: 'Gözde Düşman', featureDesc: 'Canavar avcıları; baskınları önceden sezer, kampları temizler', base: { smite: 1, foresee: 1 },
    desires: ['meat', 'fur'], heroClass: 'ranger', civName: 'Sınır Bekçileri', capital: 'Gözcüağaç',
    towns: ['İzsürer', 'Okyayı', 'Kurtgeçit', 'Yabanyurt', 'Çamgözcü'], color: '#4b7d3a',
    prefer: { gida: 1.3, yapi: 1.1, metal: 1.0, class: 1.4 }, terrainLike: { forest: 1.6, hill: 1.2, grass: 1.0 }, aggression: 0.2,
    subclasses: [
      S('hunter', 'Avcı', 'Canavarlara karşı ölümcül', { smite: 1.5 }, 'Canavar Katili', 'Dev ve canavar fark etmez; hepsi düşer', { smite: 2, soldierDmg: 2 }, (c) => 0.8 + c.threat * 1.5),
      S('beastmaster', 'Hayvan Terbiyecisi', 'Kurt yoldaşlar; askerler dayanıklı', { soldierHp: 3 }, 'Sürünün Efendisi', 'Hayvan sürüleri orduya katılır', { soldierHp: 4, soldierAtk: 1 }, (c) => 0.6 + c.forest * 2),
      S('gloom', 'Karanlık Avcı', 'Gölgeden vurur; kervanlar gizli', { raidEvade: 0.3, firstStrike: 1.2 }, 'Görünmez Ok', 'İlk ok önderi düşürür', { firstStrike: 1.6, deathTouch: 0.1 }, () => 0.8),
    ], implemented: true,
  },
  sorcerer: {
    id: 'sorcerer', name: 'Kan Büyücüsü', dnd: 'Sorcerer', race: 'tiefling', align: { law: -0.5, good: -0.1 },
    feature: 'Kan Soyu', featureDesc: 'Büyü kanlarında; her yıl kontrolsüz bir büyü dalgası', base: { research: 0.1, wild: 1 },
    desires: ['mana', 'gold'], heroClass: 'wizard', civName: 'Kızılboynuz Soyu', capital: 'Közsaray',
    towns: ['Kızılkül', 'Boynuztepe', 'Alazvadi', 'Karamum', 'Kıvılcımlı'], color: '#a33d6b',
    prefer: { bilgi: 1.5, ticaret: 1.1, metal: 0.9, class: 1.4 }, terrainLike: { swamp: 1.2, hill: 1.3, grass: 1.0 }, aggression: 0.35,
    subclasses: [
      S('draconic', 'Ejder Soyu', 'Pullu deri; askerler dayanıklı', { soldierHp: 2, defAc: 1 }, 'Ejder Kanatları', 'Ordular ejder gibi atılır', { soldierHp: 4, firstStrike: 1.4 }, () => 1),
      S('wildmagic', 'Yabani Büyü', 'Kaos kucaklanır; büyü dalgaları talihli', { luck: 1 }, 'Kaos Dalgası', 'Savaş alanına rastgele meteor', { meteor: 1 }, () => 1),
      S('shadowborn', 'Gölge Soyu', 'Karanlıkla anlaşma; suikast', { deathTouch: 0.1 }, 'Karanlığın Gözü', 'Düşman önderleri gölgede kaybolur', { deathTouch: 0.25, enemyMorale: 0.15 }, (c) => 0.6 + (c.war ? 1 : 0) + (0.5 - c.good)),
    ], implemented: true,
  },
  warlock: {
    id: 'warlock', name: 'Paktçı', dnd: 'Warlock', race: 'dwarf', align: { law: 0.5, good: -0.8 },
    feature: 'Karanlık Pakt', featureDesc: 'Patronundan her yıl hediye alır, bedelini halkıyla öder; goblinler ondan çekinir', base: { soldierDmg: 1, pact: 1, growth: 0.12 },
    desires: ['mana', 'mithril'], heroClass: 'wizard', civName: 'Karaörs Derinlikleri', capital: 'Kara Mihrap',
    towns: ['Külçukur', 'Sessizocak', 'Karagöl', 'Zincirkaya', 'Gölgeörs'], color: '#5a3d73',
    prefer: { metal: 1.4, bilgi: 1.3, yapi: 1.1, gida: 0.9, class: 1.5 }, terrainLike: { hill: 1.8, mountain: 1.4, swamp: 1.0 }, aggression: 0.6,
    subclasses: [
      S('fiend', 'Cehennem Lordu Paktı', 'Ateş ve öfke; askerlere hasar', { soldierDmg: 2 }, 'Cehennem Kapısı', 'Savaş alanına iblisler çıkar', { soldierDmg: 2, firstStrike: 1.4 }, (c) => 0.8 + (c.war ? 1 : 0)),
      S('archfey', 'Peri Sarayı Paktı', 'Yanılsama; kervanlar görünmez, sırlar çalınır', { raidEvade: 0.3, spy: 0.2 }, 'Peri Tacı', 'Komşular büyülenir, düşmanlar şaşırır', { spy: 0.25, enemyMorale: 0.15 }, () => 0.9),
      S('oldone', 'Kadim Varlık Paktı', 'Yıldızların ötesinden fısıltılar; bilgi ve dehşet', { research: 0.15, enemyMorale: 0.1 }, 'Uyanan Rüya', 'Düşman orduları dehşetle dağılır', { enemyMorale: 0.3, research: 0.1 }, () => 1),
    ], implemented: true,
  },
};
export const CLASS_IDS = Object.keys(CLASSES) as ClassId[];

// ---- Harikalar: Krallık çağında başkente dikilen sınıf anıtı ----
export const WONDERS: Record<ClassId, { name: string; desc: string; eff: Effects }> = {
  paladin: { name: 'Güneş Katedrali', desc: 'Nüfus artışı ve savunma', eff: { growth: 0.15, defAc: 2 } },
  cleric: { name: 'Dağın Kalbi', desc: 'Maden verimi, ilahi müdahale', eff: { prodMine: 0.3, divine: 0.1 } },
  druid: { name: 'Dünya Ağacı', desc: 'Hasat ve orman büyümesi', eff: { prodFood: 0.3, forestGrow: 2 } },
  rogue: { name: 'Altın Pazar', desc: 'Ticaret ve vergi', eff: { tradeGold: 0.5, tax: 0.3 } },
  wizard: { name: 'Yıldız Kulesi', desc: 'Araştırma +%30', eff: { research: 0.3 } },
  barbarian: { name: 'Kemik Taht', desc: 'Savaşçı kotası ve hasar', eff: { soldierDmg: 2, warband: 0.04 } },
  bard: { name: 'Ebedi Sahne', desc: 'Dostluk ve göç', eff: { charm: 1.5, growth: 0.1 } },
  fighter: { name: 'Ejder Kalesi', desc: 'Zırh ve savunma', eff: { soldierAc: 2, defAc: 2 } },
  monk: { name: 'Bulut Manastırı', desc: 'Araştırma ve tutumluluk', eff: { research: 0.2, frugal: 0.1 } },
  ranger: { name: 'Gözcü Ağacı', desc: 'Canavar avı ve sezgi', eff: { smite: 1, foresee: 1, favoredHunt: 1 } },
  sorcerer: { name: 'Kızıl Obelisk', desc: 'Mana ve araştırma', eff: { prodMana: 0.5, research: 0.15 } },
  warlock: { name: 'Kara Mihrap', desc: 'Pakt gücü', eff: { soldierDmg: 2 } },
};

// ---- Benzersiz birimler (SRD esinli istatistikler) ----
export interface UnitDef { id: UnitId | 'soldier' | 'militia'; name: string; hp: number; ac: number; atk: number; dmg: [number, number, number]; attacks?: number; per?: number; needs?: Good }
export const UNITS: Record<string, UnitDef> = {
  militia: { id: 'militia', name: 'Milis', hp: 6, ac: 10, atk: 1, dmg: [1, 4, 0] },
  soldier: { id: 'soldier', name: 'Asker', hp: 11, ac: 14, atk: 3, dmg: [1, 8, 1] },
  holyguard: { id: 'holyguard', name: 'Kutsal Muhafız', hp: 13, ac: 18, atk: 4, dmg: [1, 8, 2] },
  knight: { id: 'knight', name: 'Şövalye', hp: 20, ac: 18, atk: 5, dmg: [1, 10, 3], per: 0.3, needs: 'horses' },
  wolves: { id: 'wolves', name: 'Kurt Sürüsü', hp: 11, ac: 13, atk: 4, dmg: [2, 4, 2], per: 0.4 },
  treant: { id: 'treant', name: 'Treant', hp: 60, ac: 16, atk: 7, dmg: [3, 6, 4], attacks: 2, per: 0.05 },
  golem: { id: 'golem', name: 'Golem', hp: 50, ac: 17, atk: 6, dmg: [2, 8, 4], attacks: 2, per: 0.05 },
  raider: { id: 'raider', name: 'Akıncı', hp: 15, ac: 13, atk: 5, dmg: [1, 12, 3], per: 0.5 },
  blessed: { id: 'blessed', name: 'Kutsanmış Asker', hp: 12, ac: 16, atk: 3, dmg: [1, 8, 1] },
  skald: { id: 'skald', name: 'Savaş Ozanı', hp: 11, ac: 14, atk: 4, dmg: [1, 6, 2], per: 0.25 },
  legionary: { id: 'legionary', name: 'Pulzırh Lejyoneri', hp: 14, ac: 17, atk: 4, dmg: [1, 8, 2] },
  dragonguard: { id: 'dragonguard', name: 'Ejder Muhafızı', hp: 26, ac: 18, atk: 5, dmg: [2, 6, 3], attacks: 2, per: 0.12 },
  monkwarrior: { id: 'monkwarrior', name: 'Yumruk Keşişi', hp: 12, ac: 16, atk: 5, dmg: [1, 6, 3], attacks: 2, per: 0.35 },
  warden: { id: 'warden', name: 'Sınır Muhafızı', hp: 12, ac: 15, atk: 5, dmg: [1, 8, 2], per: 0.4 },
  flameborn: { id: 'flameborn', name: 'Alev Soylu', hp: 10, ac: 13, atk: 5, dmg: [2, 6, 1], per: 0.25 },
  shadowguard: { id: 'shadowguard', name: 'Gölge Muhafız', hp: 13, ac: 16, atk: 4, dmg: [1, 8, 2] },
  imp: { id: 'imp', name: 'Şeytancık', hp: 9, ac: 14, atk: 5, dmg: [1, 6, 3], per: 0.25 },
  fiend: { id: 'fiend', name: 'Kapı İblisi', hp: 45, ac: 16, atk: 7, dmg: [2, 8, 4], attacks: 2, per: 0.06 },
};

// ---- Canavarlar ----
export const MONSTERS = {
  goblin: { name: 'Goblin', hp: 7, ac: 15, atk: 4, dmg: [1, 6, 2] as [number, number, number] },
  goblinBoss: { name: 'Goblin Şefi', hp: 21, ac: 17, atk: 4, dmg: [1, 6, 2] as [number, number, number], attacks: 2 },
  hobgoblin: { name: 'Hobgoblin', hp: 11, ac: 18, atk: 3, dmg: [1, 8, 1] as [number, number, number] },
  hobCaptain: { name: 'Hobgoblin Yüzbaşısı', hp: 39, ac: 17, atk: 4, dmg: [2, 6, 2] as [number, number, number], attacks: 2 },
  bugbear: { name: 'Bugbear', hp: 27, ac: 16, atk: 4, dmg: [2, 8, 2] as [number, number, number] },
};
