// Yaşayan han: kademeler, personel, misafir türleri. Fiyatlar gümüş (10 gümüş = 1 altın).
import type { GuestKind, StaffRole } from '../sim/types';

export interface InnLevelDef { name: string; rooms: number; stable: number; food: number; ale: number; wood: number; staff: StaffRole[]; room: number; meal: number; ale$: number }
/** 1 Yol hanı · 2 Han · 3 Kervansaray. Kiler sınırları porsiyon / bardak / yük. */
export const INN_LEVEL: InnLevelDef[] = [
  { name: '', rooms: 0, stable: 0, food: 0, ale: 0, wood: 0, staff: [], room: 0, meal: 0, ale$: 0 },
  { name: 'Yol hanı', rooms: 6, stable: 3, food: 90, ale: 90, wood: 18, staff: ['cirak'], room: 4, meal: 2, ale$: 2 },
  { name: 'Han', rooms: 10, stable: 6, food: 150, ale: 150, wood: 28, staff: ['cirak', 'asci', 'seyis'], room: 5, meal: 3, ale$: 2 },
  { name: 'Kervansaray', rooms: 16, stable: 10, food: 240, ale: 240, wood: 40, staff: ['cirak', 'asci', 'seyis', 'garson', 'garson', 'bekci'], room: 6, meal: 3, ale$: 3 },
];
/** genişletme: altın + malzeme + iş (adam-gün) */
export const INN_UPGRADE: Record<number, { gold: number; wood: number; stone: number; work: number; fame: number; year: number }> = {
  2: { gold: 110, wood: 34, stone: 16, work: 70, fame: 28, year: 3 },
  3: { gold: 260, wood: 60, stone: 40, work: 120, fame: 50, year: 8 },
};
export const STAFF: Record<StaffRole, { name: string; wage: number; serve: number }> = {
  cirak: { name: 'Çırak', wage: 1.5, serve: 3 },
  asci: { name: 'Aşçı', wage: 3, serve: 5 },
  seyis: { name: 'Seyis', wage: 2, serve: 2 },
  garson: { name: 'Garson', wage: 2, serve: 5 },
  bekci: { name: 'Bekçi', wage: 3, serve: 1 },
};
export interface GuestDef { name: string; icon: string; purse: [number, number]; nights: [number, number]; n: [number, number]; mugs: number; eat: number }
export const GUEST: Record<GuestKind, GuestDef> = {
  hero: { name: 'Kahraman', icon: '★', purse: [0, 0], nights: [0, 0], n: [1, 1], mugs: 1.5, eat: 1.2 },
  merchant: { name: 'Tüccar', icon: '⚖', purse: [6, 14], nights: [1, 2], n: [1, 2], mugs: 1.5, eat: 1 },
  pilgrim: { name: 'Hacı', icon: '✧', purse: [1, 4], nights: [1, 1], n: [1, 4], mugs: 0.5, eat: 1 },
  bard: { name: 'Ozan', icon: '♪', purse: [0.5, 2], nights: [2, 5], n: [1, 1], mugs: 2, eat: 1 },
  hunter: { name: 'Avcı', icon: '➹', purse: [2, 5], nights: [1, 2], n: [1, 2], mugs: 1.5, eat: 1.2 },
  scholar: { name: 'Bilgin', icon: '✎', purse: [3, 8], nights: [1, 3], n: [1, 1], mugs: 0.8, eat: 1 },
  soldier: { name: 'Asker', icon: '⚔', purse: [2, 6], nights: [1, 3], n: [2, 4], mugs: 3, eat: 1.4 },
  refugee: { name: 'Göçmen', icon: '⌂', purse: [0, 0.6], nights: [2, 5], n: [2, 5], mugs: 0, eat: 1 },
  wanderer: { name: 'Gezgin', icon: '☍', purse: [1, 3], nights: [1, 4], n: [1, 1], mugs: 1.5, eat: 1 },
  caravan: { name: 'Kervan', icon: '⛟', purse: [0, 0], nights: [1, 1], n: [2, 5], mugs: 2, eat: 1.2 },
  noble: { name: 'Soylu', icon: '♛', purse: [20, 40], nights: [2, 3], n: [3, 5], mugs: 2, eat: 1.5 },
};
/** han türü: kiler kalemleri, gümüş karşılığı */
export const GRAIN_FOOD = 10;   // 1 tahıl = 10 porsiyon
export const GRAIN_ALE = 12;    // 1 tahıl = 12 bardak ev birası
export const BEER_ALE = 30;     // 1 bira fıçısı = 30 bardak
