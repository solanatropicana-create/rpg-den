// Oyun içi arayüz ikonları: her sınıfın kendi arması + rapor düğmeleri. Çizgi ikon, 24×24, currentColor.
import type { ClassId } from '../data/classes';

const svg = (body: string, cls = '') => `<svg class="ico ${cls}" viewBox="0 0 24 24" aria-hidden="true" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">${body}</svg>`;

const CLASS_BODY: Record<ClassId, string> = {
  paladin: '<path d="M12 3l7 2.5v5.5c0 4.5-3 8-7 10-4-2-7-5.5-7-10V5.5z"/><path d="M12 7.5v9M8.5 11h7"/>',
  cleric: '<circle cx="12" cy="12" r="3.5"/><path d="M12 2.5v3M12 18.5v3M2.5 12h3M18.5 12h3M5.3 5.3l2.1 2.1M16.6 16.6l2.1 2.1M5.3 18.7l2.1-2.1M16.6 7.4l2.1-2.1"/>',
  druid: '<path d="M5 19C5 11 10 5 20 4c-1 10-7 15-15 15z"/><path d="M5 19l9-9M11 13h-3M14 10V7"/>',
  rogue: '<path d="M20 4l-1 5-8 8-4-4 8-8z"/><path d="M5.5 11.5l7 7M8.5 15.5L4 20"/>',
  wizard: '<path d="M3.5 19.5h17"/><path d="M6.5 19.5L12 3.5l5.5 16"/><path d="M8.6 14h6.8"/><path d="M12 8.2v.1"/>',
  barbarian: '<path d="M12 2.5v19"/><path d="M12 6.5L6.2 4C3.4 7.3 3.4 12.2 6.2 15.5L12 13zM12 6.5L17.8 4c2.8 3.3 2.8 8.2 0 11.5L12 13z"/>',
  bard: '<path d="M9 18V5.5l10-2.5v13"/><circle cx="6.5" cy="18" r="2.5"/><circle cx="16.5" cy="16" r="2.5"/><path d="M9 9.5l10-2.5"/>',
  fighter: '<path d="M4 4l11 11M20 4L9 15"/><path d="M12.5 17.5l5-5M6.5 12.5l5 5"/><path d="M16 16l3.5 3.5M8 16l-3.5 3.5"/>',
  monk: '<path d="M12 4c2 3 2 9 0 15-2-6-2-12 0-15z"/><path d="M12 19c-4 0-7-2-8.5-5.5 3.5-.5 6.5 1.5 8.5 5.5zM12 19c4 0 7-2 8.5-5.5-3.5-.5-6.5 1.5-8.5 5.5z"/>',
  ranger: '<path d="M6.5 3c7 3 7 15 0 18"/><path d="M6.5 3v18"/><path d="M3 12h17M17 9l3 3-3 3"/>',
  sorcerer: '<path d="M12 21c-4 0-6.5-2.7-6.5-6.2 0-3.3 2.4-5.2 3.4-8.3 1.6 1.4 2.4 3 2.4 4.7 1-1.2 1.6-2.8 1.5-4.7 3.2 2.3 5.7 5.3 5.7 8.5 0 3.5-2.5 6-6.5 6z"/><path d="M12 21c-1.6 0-2.6-1-2.6-2.5 0-1.4 1.2-2.4 2.6-4 1.4 1.6 2.6 2.6 2.6 4 0 1.5-1 2.5-2.6 2.5z"/>',
  warlock: '<path d="M2.5 12S6 5.5 12 5.5 21.5 12 21.5 12 18 18.5 12 18.5 2.5 12 2.5 12z"/><path d="M12 8.3c1.1 1.1 1.1 6.3 0 7.4-1.1-1.1-1.1-6.3 0-7.4z"/>',
};

export const classIcon = (c: ClassId) => svg(CLASS_BODY[c]);

export type ReportTab = 'civ' | 'tree' | 'compare' | 'hero' | 'inn' | 'log';
const TAB_BODY: Record<ReportTab | 'swords' | 'sound' | 'mute' | 'help' | 'gear' | 'prev' | 'next', string> = {
  civ: '<path d="M4 18.5h16l-1.5-10-4 3.5L12 5l-2.5 7-4-3.5z"/><path d="M4 21h16"/>',
  tree: '<circle cx="6" cy="6" r="2"/><circle cx="6" cy="18" r="2"/><circle cx="18" cy="12" r="2"/><path d="M8 6h3a2 2 0 0 1 2 2v2.5a1.5 1.5 0 0 0 1.5 1.5H16M8 18h3a2 2 0 0 0 2-2v-2.5"/>',
  compare: '<path d="M4 20h16"/><path d="M6 18v-7h3v7M10.5 18V6h3v12M15 18V9h3v9"/>',
  hero: '<path d="M5 14a7 7 0 0 1 14 0v5.5h-4.2V15H9.2v4.5H5z"/><path d="M12 7V3.5M9 10.5h6"/>',
  inn: '<path d="M5 8h10v10.5a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2z"/><path d="M15 10.5h2a2 2 0 0 1 2 2v2a2 2 0 0 1-2 2h-2"/><path d="M5 8c0-2 1.2-3.5 3-3.5.6-1 1.6-1.5 2.7-1.5 1.8 0 3.3 1.3 3.3 3.2V8"/><path d="M8.5 12v5M11.5 12v5"/>',
  log: '<path d="M7 4h11v13a3 3 0 0 1-3 3H6a3 3 0 0 1-3-3h11"/><path d="M7 4a2 2 0 0 0-2 2v11"/><path d="M9 8h6M9 11h6M9 14h3.5"/>',
  swords: '<path d="M5 5l14 14M19 5L5 19"/><path d="M14.5 19.5l5-5M4.5 14.5l5 5"/>',
  sound: '<path d="M4 9.5h3.5L12 5.5v13l-4.5-4H4z"/><path d="M15.5 9a4 4 0 0 1 0 6M18 6.5a7.5 7.5 0 0 1 0 11"/>',
  mute: '<path d="M4 9.5h3.5L12 5.5v13l-4.5-4H4z"/><path d="M16 9.5l5 5M21 9.5l-5 5"/>',
  help: '<circle cx="12" cy="12" r="9"/><path d="M9.6 9.4a2.5 2.5 0 1 1 3.4 2.3c-.7.3-1 .8-1 1.5v.6"/><path d="M12 16.8v.2"/>',
  gear: '<circle cx="12" cy="12" r="3"/><path d="M12 2.8v2.4M12 18.8v2.4M2.8 12h2.4M18.8 12h2.4M5.5 5.5l1.7 1.7M16.8 16.8l1.7 1.7M5.5 18.5l1.7-1.7M16.8 7.2l1.7-1.7"/>',
  prev: '<path d="M14.5 6l-6 6 6 6"/>',
  next: '<path d="M9.5 6l6 6-6 6"/>',
};
export const uiIcon = (k: keyof typeof TAB_BODY, cls = '') => svg(TAB_BODY[k], cls);
