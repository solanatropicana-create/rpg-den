import type { RaceId } from './classes';

export type HeroClass = 'fighter' | 'wizard' | 'cleric' | 'rogue' | 'ranger' | 'paladin' | 'druid' | 'barbarian';
export const HERO_CLASS_TR: Record<HeroClass, string> = {
  fighter: 'Savaşçı', wizard: 'Büyücü', cleric: 'Rahip', rogue: 'Hırsız', ranger: 'Korucu', paladin: 'Paladin', druid: 'Druid', barbarian: 'Barbar',
};
export const HERO_CLASS_IDS = Object.keys(HERO_CLASS_TR) as HeroClass[];
export type Stat = 'str' | 'dex' | 'con' | 'int' | 'wis' | 'cha';
export const STATS: Stat[] = ['str', 'dex', 'con', 'int', 'wis', 'cha'];

export interface HeroClassDef { id: HeroClass; hitDie: number; primary: Stat; priority: Stat[]; dmg: [number, number] }
export const HERO_CLASSES: Record<HeroClass, HeroClassDef> = {
  fighter: { id: 'fighter', hitDie: 10, primary: 'str', priority: ['str', 'con', 'dex', 'wis', 'cha', 'int'], dmg: [1, 10] },
  wizard: { id: 'wizard', hitDie: 6, primary: 'int', priority: ['int', 'con', 'dex', 'wis', 'cha', 'str'], dmg: [1, 10] },
  cleric: { id: 'cleric', hitDie: 8, primary: 'wis', priority: ['wis', 'con', 'str', 'cha', 'dex', 'int'], dmg: [1, 8] },
  rogue: { id: 'rogue', hitDie: 8, primary: 'dex', priority: ['dex', 'con', 'int', 'cha', 'wis', 'str'], dmg: [1, 6] },
  ranger: { id: 'ranger', hitDie: 10, primary: 'dex', priority: ['dex', 'wis', 'con', 'str', 'int', 'cha'], dmg: [1, 8] },
  paladin: { id: 'paladin', hitDie: 10, primary: 'str', priority: ['str', 'cha', 'con', 'wis', 'dex', 'int'], dmg: [1, 8] },
  druid: { id: 'druid', hitDie: 8, primary: 'wis', priority: ['wis', 'con', 'dex', 'int', 'cha', 'str'], dmg: [1, 8] },
  barbarian: { id: 'barbarian', hitDie: 12, primary: 'str', priority: ['str', 'con', 'dex', 'wis', 'cha', 'int'], dmg: [1, 12] },
};
export const XP_LEVELS = [0, 300, 900, 2700, 6500, 14000];

export const HERO_NAMES: Record<RaceId, string[]> = {
  human: ['Aldric', 'Mira', 'Tomas', 'Elena', 'Gareth', 'Selene', 'Rowan', 'Isolde', 'Bram', 'Lysa', 'Cedric', 'Maren'],
  dwarf: ['Borin', 'Helga', 'Durgan', 'Brunhild', 'Thrain', 'Dagna', 'Gimrik', 'Vistra', 'Baern', 'Eldeth'],
  elf: ['Aelar', 'Naivara', 'Thamior', 'Sariel', 'Erevan', 'Shava', 'Varis', 'Lia'],
  halfling: ['Pip', 'Merry', 'Cora', 'Milo', 'Rosie', 'Wendel', 'Lidda'],
  gnome: ['Fonkin', 'Nissa', 'Boddyn', 'Ellyjobell', 'Zook', 'Carlin'],
  halfelf: ['Kaelen', 'Liora', 'Dorian', 'Ysolde', 'Tarek'],
  halforc: ['Gruk', 'Shura', 'Dench', 'Ovak', 'Yevelda', 'Holg'],
  dragonborn: ['Arjhan', 'Sora', 'Medrash', 'Kava', 'Nadarr'],
  tiefling: ['Akmenos', 'Nemeia', 'Damakos', 'Orianna', 'Leucis'],
};
export const EPITHETS = ['Cesur', 'Sessiz', 'Kızıl', 'Demir', 'Kurnaz', 'Yalnız', 'Bilge', 'Gölge', 'Yıldız', 'Taşkalp', 'Hızlı', 'Kara'];
export const HERO_ORIGINS = ['kuzey dağlarından gelen', 'yıkık bir köyün son kalanı', 'eski bir tapınakta yetişmiş', 'sokaklarda büyümüş', 'ormanlarda yalnız yaşamış', 'soylu bir aileden kovulmuş', 'uzak bir limandan gelen'];
export const HERO_DRIVES = ['şan peşinde', 'kayıp kardeşini arıyor', 'eski bir borcu ödemek istiyor', 'altına düşkün', 'goblinlerden intikam almak istiyor', 'efsanelerdeki ejderhayı arıyor', 'kendini kanıtlamak istiyor'];
export const RACE_STAT: Record<RaceId, Partial<Record<Stat, number>>> = {
  human: { str: 1, dex: 1, con: 1, int: 1, wis: 1, cha: 1 }, dwarf: { con: 2, str: 2 }, elf: { dex: 2, int: 1 },
  halfling: { dex: 2, cha: 1 }, gnome: { int: 2, con: 1 }, halfelf: { cha: 2, dex: 1, con: 1 },
  halforc: { str: 2, con: 1 }, dragonborn: { str: 2, cha: 1 }, tiefling: { cha: 2, int: 1 },
};

// ---- kahraman iradesi: hizalama, yollar, izler
export type HeroAlign = 'good' | 'neutral' | 'evil';
export const ALIGN_TR: Record<HeroAlign, string> = { good: 'İyi', neutral: 'Tarafsız', evil: 'Kötü' };
export type HeroPath = 'hunter' | 'wanderer' | 'healer' | 'sage' | 'mercenary' | 'dark';
export const PATH_TR: Record<HeroPath, string> = { hunter: 'Avcı', wanderer: 'Gezgin', healer: 'Şifacı', sage: 'Bilge', mercenary: 'Paralı', dark: 'Karanlık yol' };
export const PATH_DESC: Record<HeroPath, string> = {
  hunter: 'canavar kamplarını kovalar', wanderer: 'harabeleri keşfeder', healer: 'salgınlı kasabalara şifa götürür',
  sage: 'kütüphaneleri dolaşır', mercenary: 'en yüksek ödülü kovalar', dark: 'kervan soyar',
};
/** sınıfın doğal yolu */
export const CLASS_PATH: Record<HeroClass, HeroPath> = {
  fighter: 'hunter', barbarian: 'hunter', ranger: 'hunter', rogue: 'wanderer', cleric: 'healer', paladin: 'healer', druid: 'healer', wizard: 'sage',
};
export type TraitId = 'goblinslayer' | 'plaguewalker' | 'lonewolf' | 'ruinrat' | 'avenger' | 'legend';
export const TRAITS: Record<TraitId, { name: string; desc: string; icon: string }> = {
  goblinslayer: { name: 'Goblin Kıran', desc: 'Goblinlere karşı +2 saldırı', icon: '🗡' },
  plaguewalker: { name: 'Veba Yürüyüşçüsü', desc: 'Salgına bağışık', icon: '✚' },
  lonewolf: { name: 'Yalnız Kurt', desc: 'Tek başına +1 saldırı ve zırh', icon: '🐺' },
  ruinrat: { name: 'Harabe Kurdu', desc: 'Keşifte ve tuzaklarda +2', icon: '🗝' },
  avenger: { name: 'İntikamcı', desc: 'Yuvasını yakanı kovalıyor', icon: '🔥' },
  legend: { name: 'Efsane', desc: 'Ozanlar şarkısını söylüyor', icon: '♪' },
};
export const INN_NAMES = ['Kırık Kupa', 'Üç Yol', 'Yaşlı Ejder', 'Kızıl Fener', 'Yorgun Katır', 'Gümüş Nal', 'Dolunay', 'Kara Kazan', 'Uyuyan Dev', 'Tilki Yuvası'];
export const KEEPER_NAMES = ['Barnabas', 'Hilde', 'Osric', 'Marta', 'Gundren', 'Tobin', 'Yelda', 'Rahmi', 'Ottilie', 'Fenwick'];
