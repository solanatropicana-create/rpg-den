// Yaşayan han: hancı kafilesi (Settlers gibi yerleşimden çıkıp yer seçer), inşaat, misafirler, kiler,
// erzak arabası (medeniyetten gerçek alım), personel, ün, genişleme ve mevsimlik kasa defteri.
import type { Sim } from './sim';
import { YEAR, FOOD_PER_POP } from './sim';
import { ek } from './tr';
import type { RaceId } from '../data/classes';
import { HERO_NAMES, HERO_CLASS_TR, INN_NAMES, KEEPER_NAMES } from '../data/heroes';
import { GOODS, type Good, type Stock } from '../data/goods';
import { INN_LEVEL, INN_UPGRADE, STAFF, GUEST, GRAIN_FOOD, GRAIN_ALE, BEER_ALE } from '../data/inn';
import { markInn } from './worldgen';
import { heroLabel, innById } from './will';
import type { Agent, Guest, GuestKind, Inn, InnBook, InnLog, Settlement, StaffRole } from './types';

const HERO_WORK: Record<string, string> = {
  ranger: 'avlanıp mutfağa et getiriyor', barbarian: 'avlanıp mutfağa et getiriyor', druid: 'ormandan ot ve mantar topluyor',
  fighter: 'avluda nöbet tutuyor', paladin: 'avluda nöbet tutuyor', cleric: 'yolcuların yaralarını sarıyor',
  wizard: 'ocak başında hikâye anlatıyor', rogue: 'zar masasında şansını deniyor',
};
const RACE_IDS: RaceId[] = ['human', 'dwarf', 'elf', 'halfling', 'gnome', 'halfelf', 'halforc', 'dragonborn', 'tiefling'];

export const innTitle = (inn: Inn) => `${inn.name} Hanı`;
export const levelName = (inn: Inn) => INN_LEVEL[Math.max(1, inn.level)].name;
/** fiyatlar gümüş (10 gümüş = 1 altın) */
export function innPrices(inn: Inn) {
  const L = INN_LEVEL[Math.max(1, inn.level)];
  return { room: L.room + (inn.fame >= 70 ? 1 : 0), meal: L.meal, ale: L.ale$ };
}
export function innOcc(inn: Inn) {
  let rooms = 0, stable = 0;
  for (const g of inn.guests) if (g.stable) stable += g.n; else rooms += g.n;
  return { rooms, stable, persons: rooms + stable };
}
export function serveCap(inn: Inn) { return 3 + inn.staff.reduce((a, x) => a + STAFF[x.role].serve, 0); }
export function wages(inn: Inn) { return inn.staff.reduce((a, x) => a + STAFF[x.role].wage, 0); }
export function buildStageName(inn: Inn): string {
  const b = inn.build;
  if (!b) return '';
  const p = b.work / b.need;
  if (b.level > 1 && !b.rebuild && inn.stage === 'open') return p < 0.3 ? 'Yeni kanadın temeli' : p < 0.7 ? 'Kanadın duvarları' : 'Kanadın çatısı';
  return p < 0.2 ? 'Temel kazılıyor' : p < 0.5 ? 'İskelet çatılıyor' : p < 0.8 ? 'Duvarlar örülüyor' : 'Çatı kapatılıyor';
}
export function buildPct(inn: Inn) {
  const b = inn.build;
  if (!b) return 0;
  const mat = Math.min(b.woodNeed ? b.wood / b.woodNeed : 1, b.stoneNeed ? b.stone / b.stoneNeed : 1);
  return Math.max(0, Math.min(1, (b.work / b.need) * 0.8 + Math.min(1, mat) * 0.2));
}

function addLog(s: Sim, inn: Inn, k: InnLog['k'], t: string, g?: number) {
  inn.log.push({ day: s.day, k, t, g });
  if (inn.log.length > 90) inn.log.splice(0, inn.log.length - 90);
}
function book(s: Sim, inn: Inn): InnBook {
  const season = Math.floor(s.day / (YEAR / 4));
  let b = inn.books[inn.books.length - 1];
  if (!b || b.season !== season) {
    b = { season, room: 0, food: 0, ale: 0, other: 0, supply: 0, wage: 0, build: 0, guests: 0, nights: 0 };
    inn.books.push(b);
    if (inn.books.length > 8) inn.books.shift();
  }
  return b;
}
/** hana başka yerden giren para (açık artırma payı, ilan iadesi...) */
export function innIncome(s: Sim, inn: Inn, gold: number, why: string) {
  inn.gold += gold;
  book(s, inn).other += gold;
  inn.total.income += gold;
  addLog(s, inn, 'ev', why, gold);
}
export function innEvent(s: Sim, inn: Inn, text: string) { addLog(s, inn, 'ev', text); }

function personName(s: Sim, race: RaceId) { return s.rng.pick(HERO_NAMES[race] ?? HERO_NAMES.human); }
function raceFrom(s: Sim, st: Settlement | undefined): RaceId {
  if (!st) return s.rng.pick(RACE_IDS);
  const rs = (Object.keys(st.pop) as RaceId[]).filter((r) => (st.pop[r] ?? 0) > 0);
  return s.rng.chance(0.85) && rs.length ? s.rng.weighted(rs, (r) => st.pop[r] ?? 0)! : s.rng.pick(RACE_IDS);
}
function nearestSettlements(s: Sim, tile: number, n = 2) {
  return s.w.settlements.filter((x) => x.alive).map((x) => ({ x, d: s.g.dist(x.tile, tile) })).sort((a, b) => a.d - b.d || a.x.id - b.x.id).slice(0, n);
}
/** "Tunçkale ile Lirköy arasına" */
export function whereStr(s: Sim, tile: number) {
  const nn = nearestSettlements(s, tile, 2);
  if (nn.length >= 2) return `${nn[0].x.name} ile ${nn[1].x.name} arası`;
  return nn.length ? `${ek(nn[0].x.name, 'in')} yakını` : 'ıssız bir yol ağzı';
}

// ------------------------------------------------------------ yer seçimi
function forestNear(s: Sim, tile: number) { return s.g.within(tile, 2).some((t) => (s.w.tiles[t].terrain === 'forest' || s.w.tiles[t].terrain === 'oldforest') && s.w.tiles[t].wood > 5); }
function stoneNear(s: Sim, tile: number) { return s.g.within(tile, 3).some((t) => s.w.tiles[t].terrain === 'hill' || s.w.tiles[t].terrain === 'mountain'); }

function siteOk(s: Sim, i: number, self?: Inn) {
  const w = s.w, t = w.tiles[i];
  if (t.sea || t.isle || !['grass', 'tundra', 'hill'].includes(t.terrain)) return false;
  if (t.owner >= 0 || t.deposit >= 0 || t.camp !== undefined || t.ext) return false;
  if (t.innZone !== undefined && t.innZone !== self?.id) return false;
  const c = s.g.col(i), r = s.g.row(i);
  if (c < 3 || r < 3 || c > w.W - 4 || r > w.H - 4) return false;
  if (w.settlements.some((x) => x.alive && s.g.dist(x.tile, i) < 5)) return false;
  if (w.inns.some((x) => x !== self && s.g.dist(x.tile, i) < 10)) return false;
  if (w.camps.some((cp) => cp.alive && s.g.dist(cp.tile, i) < 4)) return false;
  return true;
}
function siteScore(s: Sim, i: number): number | null {
  if (!siteOk(s, i)) return null;
  const w = s.w, t = w.tiles[i];
  const ds = w.settlements.filter((x) => x.alive).map((x) => s.g.dist(x.tile, i)).sort((a, b) => a - b);
  if (ds.length < 2 || ds[0] < 6 || ds[0] > 17 || ds[1] - ds[0] > 9) return null;
  let sc = -((ds[0] - 9) ** 2) * 0.05 - (ds[1] - ds[0]) * 0.22;
  if (ds.length > 2 && ds[2] <= 18) sc += 0.7;               // üç yolun kavşağı
  if (t.terrain === 'grass') sc += 1; else if (t.terrain === 'hill') sc -= 0.4;
  if (t.road) sc += 1.5;
  if (s.g.neighbors(i).some((n) => w.tiles[n].terrain === 'water' && !w.tiles[n].sea)) sc += 0.6;   // göl, dere
  if (forestNear(s, i)) sc += 0.7;
  if (stoneNear(s, i)) sc += 0.3;
  for (const cp of w.camps) if (cp.alive) { const d = s.g.dist(cp.tile, i); if (d <= 6) sc -= 1; else if (d <= 11) sc += 0.4; }   // maceracı gelir ama kapı dibinde goblin istemez
  return sc;
}
/** göreceli rastgele: en iyi yirmi yerden biri, puanla ağırlıklı */
function pickSite(s: Sim): number {
  const cands: { i: number; sc: number }[] = [];
  for (let i = 0; i < s.w.tiles.length; i++) { const sc = siteScore(s, i); if (sc !== null) cands.push({ i, sc }); }
  if (!cands.length) return -1;
  cands.sort((a, b) => b.sc - a.sc || a.i - b.i);
  const top = cands.slice(0, 20), best = top[0].sc;
  return s.rng.weighted(top, (c) => Math.exp((c.sc - best) * 0.9))!.i;
}

// ------------------------------------------------------------ hancı kafilesi
function newInn(s: Sim, tile: number, keeper: string, race: RaceId, origin: { id: number; name: string }): Inn {
  const used = new Set(s.w.inns.map((x) => x.name));
  const free = INN_NAMES.filter((n) => !used.has(n));
  const name = free.length ? s.rng.pick(free) : `${s.rng.pick(INN_NAMES)} ${s.w.inns.length + 1}`;
  return {
    id: s.id(), tile, name, keeper, founded: s.day, alive: false, gold: 80, raids: 0,
    stage: 'road', level: 0, keeperRace: race, origin: origin.id, originName: origin.name,
    stock: { food: 24, ale: 10, wood: 16 }, fame: 12, staff: [], guests: [], tabs: {}, log: [], books: [], hist: [],
    total: { guests: 0, nights: 0, income: 0, turned: 0, bought: 0 }, turned: 0, sat: 0.7, traffic: 0,
  };
}

/** hancı yola çıkar: yeni han, harabeyi yeniden kurma ya da ustasının yanından ayrılan çırak */
export function dispatchKeeper(s: Sim, o: { rebuild?: Inn; fromInn?: Inn; name?: string; race?: RaceId } = {}): boolean {
  const w = s.w;
  const site = o.rebuild ? o.rebuild.tile : pickSite(s);
  if (site < 0) return false;
  let fromTile: number, origin: { id: number; name: string }, homeSt: Settlement | undefined;
  if (o.fromInn) { fromTile = o.fromInn.tile; origin = { id: o.fromInn.id, name: innTitle(o.fromInn) }; homeSt = nearestSettlements(s, o.fromInn.tile, 1)[0]?.x; }
  else {
    const near = nearestSettlements(s, site, 3);
    if (!near.length) return false;
    homeSt = s.rng.weighted(near, (x) => 1 / (1 + Math.max(0, x.d - near[0].d)))!.x;
    fromTile = homeSt.tile; origin = { id: homeSt.id, name: homeSt.name };
  }
  const path = s.path(fromTile, site);
  if (!path || path.length < 2) return false;
  const race = o.race ?? (homeSt && s.rng.chance(0.75) ? w.civs[homeSt.civ].race : s.rng.pick(RACE_IDS));
  const keeper = o.name ?? (s.rng.chance(0.5) ? s.rng.pick(KEEPER_NAMES) : personName(s, race));
  let inn: Inn;
  if (o.rebuild) {
    inn = o.rebuild;
    Object.assign(inn, { stage: 'road', keeper, keeperRace: race, origin: origin.id, originName: origin.name, gold: Math.max(inn.gold, 50), stock: { food: 24, ale: 10, wood: 16 }, staff: [], guests: [], fame: Math.max(8, inn.fame * 0.5), order: undefined });
  } else {
    inn = newInn(s, site, keeper, race, origin);
    w.inns.push(inn);
  }
  // yanında bir çırak (ve bazen bir yardımcı) getirir
  const r0 = raceFrom(s, homeSt);
  inn.staff.push({ name: personName(s, r0), race: r0, role: 'cirak', since: s.day, from: origin.name });
  if (s.rng.chance(0.5)) { const r1 = raceFrom(s, homeSt); inn.staff.push({ name: personName(s, r1), race: r1, role: 'seyis', since: s.day, from: origin.name }); }
  w.agents.push({ id: s.id(), kind: 'keeper', civ: -1, path, step: 0, progress: 0, speed: 0.55, from: origin.id, to: inn.id, inn: inn.id, targetTile: site, purpose: 'found' });
  const where = whereStr(s, site);
  addLog(s, inn, 'build', `Hancı ${keeper} ${ek(origin.name, 'dan')} öküz arabasıyla yola çıktı`);
  s.metric('innKeeper');
  const cause = o.rebuild ? `${innTitle(inn)} yıkılalı ${Math.floor((s.day - (inn.ruinedDay ?? s.day)) / YEAR)} yıl oldu; çevre temizlendi`
    : o.fromInn ? `${o.fromInn.keeper} ustasının yanında yetişti` : 'Yollar kalabalıklaşacak; yorgun yolcuya bir çatı, atına bir ahır gerek';
  s.log('inn', o.rebuild ? `Hancı ${keeper}, ${innTitle(inn)} harabesini yeniden kurmak için ${ek(origin.name, 'dan')} yola çıktı.`
    : o.fromInn ? `Çırak ${keeper}, ${ek(innTitle(o.fromInn), 'dan')} ayrıldı: ${where}nda kendi hanını kuracak.`
      : `Hancı ${keeper}, ${ek(origin.name, 'dan')} öküz arabasıyla yola çıktı: ${where}nda bir han kuracak.`,
  { tile: fromTile, major: true, cause });
  return true;
}

export function keeperArrive(s: Sim, a: Agent): boolean {
  const inn = innById(s, a.inn);
  if (!inn || inn.stage !== 'road') return true;
  const here = a.path[Math.min(a.step, a.path.length - 1)];
  const rebuild = inn.ruinedDay !== undefined;
  if (!rebuild && !siteOk(s, here, inn)) {
    // yol boyunca yer kapılmış: çevrede başka düz bir yer ara
    const alt = s.g.within(here, 4).filter((i) => siteOk(s, i, inn)).sort((x, y) => s.g.dist(x, here) - s.g.dist(y, here) || x - y)[0];
    const path = alt !== undefined ? s.path(here, alt) : null;
    if (alt === undefined || !path) {
      s.w.inns = s.w.inns.filter((x) => x !== inn);
      s.log('inn', `Hancı ${inn.keeper} kuracak yer bulamadı; arabasını çevirip ${ek(inn.originName, 'a')} döndü.`, { tile: here, cause: 'Seçtiği toprak başkasına geçmiş' });
      return true;
    }
    a.path = path; a.step = 0; a.progress = 0; a.targetTile = alt; inn.tile = alt;
    return false;
  }
  startBuild(s, inn, rebuild ? inn.tile : here);
  return true;
}

function startBuild(s: Sim, inn: Inn, tile: number) {
  inn.tile = tile;
  inn.stage = 'build';
  markInn(s.g, s.w.tiles, inn);
  const re = inn.ruinedDay !== undefined;
  inn.build = { level: 1, work: 0, need: re ? 36 : 48, wood: inn.stock.wood, woodNeed: re ? 22 : 30, stone: 0, stoneNeed: re ? 0 : 8, started: s.day, rebuild: re };
  inn.stock.wood = 0;
  addLog(s, inn, 'build', re ? 'Harabe temizlendi, eski temel üstüne kuruluyor' : 'Temel atıldı');
  s.log('inn', `Hancı ${inn.keeper}, ${whereStr(s, tile)}nda ${innTitle(inn)} için temel attı.`, { tile, cause: `Yanında ${inn.staff.length} yardımcı; arabada ${Math.round(inn.build.wood)} kereste` });
}

function finishBuild(s: Sim, inn: Inn) {
  const b = inn.build!;
  inn.build = undefined;
  if (inn.stage === 'build') {
    inn.stage = 'open'; inn.alive = true; inn.level = 1; inn.founded = s.day; inn.ruinedDay = undefined;
    inn.fame = Math.max(inn.fame, 15);
    s.metric('innOpened');
    const n = s.w.metrics.innOpened;
    if (n <= 2 && !s.w.metrics['innOpenDay' + n]) s.w.metrics['innOpenDay' + n] = s.day;
    addLog(s, inn, 'build', 'Kapılar açıldı; ocak yandı, ilk fıçı açıldı');
    s.log('inn', `${innTitle(inn)} kapılarını açtı! Hancı ${inn.keeper} ocağı yaktı, ilk fıçıyı açtı.`, { tile: inn.tile, major: true, cause: `${s.day - b.started} günlük inşaat · ${INN_LEVEL[1].rooms} oda · ${whereStr(s, inn.tile)}` });
  } else {
    inn.level = b.level;
    s.metric('innUpgrade');
    addLog(s, inn, 'build', `Genişletme bitti: artık bir ${levelName(inn)} (${INN_LEVEL[inn.level].rooms} oda)`);
    s.log('inn', `${innTitle(inn)} genişledi: artık bir ${levelName(inn).toLocaleLowerCase('tr')}, ${INN_LEVEL[inn.level].rooms} odalı.`, { tile: inn.tile, major: true, cause: `${s.day - b.started} günlük inşaat · ün ${Math.round(inn.fame)}` });
  }
  ensureStaff(s, inn);
}

function buildTick(s: Sim, inn: Inn) {
  const b = inn.build!;
  const winter = s.season === 3 ? 0.6 : 1;
  // gündelikçi: yakın köylerden, günlüğü 0.2 altın
  const hired = inn.gold > 30 ? (inn.gold > 90 ? 2 : 1) : 0;
  if (hired) { const pay = hired * 0.2; inn.gold -= pay; book(s, inn).build += pay; }
  let hands = 1 + inn.staff.length + hired;
  const forest = forestNear(s, inn.tile), quarry = stoneNear(s, inn.tile);
  if (b.wood < b.woodNeed && forest) { const k = Math.min(hands - 1, 2); if (k > 0) { b.wood += 0.6 * k * winter; hands -= k; } }
  if (b.stone < b.stoneNeed && quarry) { const k = Math.min(hands - 1, 1); if (k > 0) { b.stone += 0.4 * k * winter; hands -= k; } }
  if (!inn.order && ((b.wood < b.woodNeed && !forest) || (b.stone < b.stoneNeed && !quarry))) {
    const ok = placeOrder(s, inn, { wood: forest ? 0 : Math.ceil(b.woodNeed - b.wood), stone: quarry ? 0 : Math.ceil(b.stoneNeed - b.stone) });
    // taş hiçbir yerden gelmiyorsa ahşap temelle yetin
    if (!ok && b.stone < b.stoneNeed && !quarry && s.day - b.started > 60) { b.woodNeed += Math.ceil((b.stoneNeed - b.stone) * 0.8); b.stoneNeed = Math.floor(b.stone); addLog(s, inn, 'build', 'Taş bulunamadı; temel ahşapla güçlendirildi'); }
  }
  const mat = Math.min(b.woodNeed ? b.wood / b.woodNeed : 1, b.stoneNeed ? b.stone / b.stoneNeed : 1);
  const cap = b.need * Math.min(1, mat + 0.15);
  if (b.work < cap) b.work = Math.min(cap, b.work + (hands * 0.55 + 0.3) * winter);
  if (b.work >= b.need && mat >= 1) finishBuild(s, inn);
}

// ------------------------------------------------------------ erzak arabası: medeniyetten gerçek alım
type Want = { food?: number; ale?: number; wood?: number; stone?: number };
/** bir yerleşimden ne alınabilir: satıcı kendi ambar payını (20 günlük gıda) ve her malın %30'undan fazlasını vermez */
function quote(s: Sim, inn: Inn, st: Settlement, want: Want) {
  const c = s.w.civs[st.civ];
  const cargo: Stock = {};
  const reserve = s.civPop(c) * FOOD_PER_POP * 40 + 15;   // kendi ambar payına (40 gün) dokunmaz
  let surplus = s.foodTotal(c) - reserve;
  let fNeed = want.food ?? 0, aNeed = want.ale ?? 0, got = 0;
  const take = (g: Good, units: number) => { const q = Math.max(0, Math.min(Math.ceil(units), Math.floor(s.st(c, g) * 0.2) - (cargo[g] ?? 0))); if (q > 0) cargo[g] = (cargo[g] ?? 0) + q; return q; };
  if (aNeed > 0 && s.st(c, 'beer') >= 4) { const q = take('beer', aNeed / BEER_ALE); aNeed -= q * BEER_ALE; got += q * BEER_ALE; }
  for (const g of ['grain', 'fish', 'meat', 'bread'] as Good[]) {
    if (fNeed <= 0 || surplus <= 0) break;
    const per = g === 'bread' ? 24 : GRAIN_FOOD;
    const q = take(g, Math.min(fNeed / per, surplus / (GOODS[g].food ?? 1)));
    fNeed -= q * per; surplus -= q * (GOODS[g].food ?? 1); got += q * per;
  }
  if (aNeed > 0 && surplus > 0) { const q = take('grain', Math.min(aNeed / GRAIN_ALE, surplus)); got += q * GRAIN_ALE; }
  if (want.wood) got += take('wood', want.wood) * 6;
  if (want.stone) got += take('stone', want.stone) * 6;
  let cost = 0;
  for (const g in cargo) cost += (cargo[g as Good] ?? 0) * s.price(c, g as Good) * 1.15;
  const budget = inn.gold - 5;
  if (cost > budget && cost > 0) {
    const k = Math.max(0, budget / cost);
    got *= k;
    for (const g in cargo) { cargo[g as Good] = Math.floor((cargo[g as Good] ?? 0) * k); if (!cargo[g as Good]) delete cargo[g as Good]; }
    cost = 0;
    for (const g in cargo) cost += (cargo[g as Good] ?? 0) * s.price(c, g as Good) * 1.15;
  }
  const need = (want.food ?? 0) + (want.ale ?? 0) + (want.wood ?? 0) * 6 + (want.stone ?? 0) * 6;
  return { cargo, cost: Math.round(cost * 10) / 10, fill: need > 0 ? got / need : 0 };
}
function placeOrder(s: Sim, inn: Inn, want: Want): boolean {
  const w = s.w;
  if (inn.order || inn.gold < 6) return false;
  const cands = w.settlements.filter((x) => x.alive && s.pop(x) >= 8 && x.starving <= 0 && s.g.dist(x.tile, inn.tile) <= 24 && (w.civs[x.civ].innBanUntil ?? 0) <= s.day)
    .sort((a, b) => s.g.dist(a.tile, inn.tile) - s.g.dist(b.tile, inn.tile) || a.id - b.id).slice(0, 6);
  let best: { st: Settlement; q: ReturnType<typeof quote>; sc: number } | undefined;
  for (const st of cands) {
    const q = quote(s, inn, st, want);
    if (q.cost <= 0) continue;
    const unit = q.cost / Math.max(1, q.fill * ((want.food ?? 0) + (want.ale ?? 0) + ((want.wood ?? 0) + (want.stone ?? 0)) * 6));
    if (unit > 0.35 && !inn.build && inn.stock.food > INN_LEVEL[Math.max(1, inn.level)].food * 0.12) continue;   // karaborsa fiyatına almaz
    const sc = Math.min(1, q.fill) - s.g.dist(st.tile, inn.tile) * 0.015 - unit * 3;
    if (!best || sc > best.sc) best = { st, q, sc };
  }
  const L = INN_LEVEL[Math.max(1, inn.level)];
  const critical = inn.stock.food < L.food * 0.12 || !!inn.build;
  if (!best || (best.q.fill < 0.25 && !critical)) return false;
  const { st, q: { cargo, cost } } = best;
  const path = s.path(st.tile, inn.tile);
  if (!path) return false;
  const c = w.civs[st.civ];
  for (const g in cargo) s.add(c, g as Good, -(cargo[g as Good] ?? 0));
  s.add(c, 'gold', cost);
  inn.gold -= cost;
  const bk = book(s, inn);
  if (want.wood || want.stone) bk.build += cost; else bk.supply += cost;
  inn.total.bought += cost;
  const desc = Object.entries(cargo).map(([g, q]) => `${q} ${GOODS[g as Good].name.toLocaleLowerCase('tr')}`).join(', ');
  const id = s.id();
  w.agents.push({ id, kind: 'supply', civ: c.id, path, step: 0, progress: 0, speed: 0.7, cargo, from: st.id, to: inn.id, inn: inn.id, purpose: 'supply' });
  inn.order = { agent: id, fromName: st.name, goods: desc, cost, day: s.day };
  addLog(s, inn, 'buy', `${ek(st.name, 'dan')} ${desc} ısmarlandı (${c.name})`, -cost);
  s.metric('innBuy');
  return true;
}

/** kervan başı yolunu hanın önünden geçirir (az sapma ise) */
export function innDetour(s: Sim, path: number[]): number[] {
  if (path.length < 6) return path;
  const a = path[0], b = path[path.length - 1];
  let best: number[] | null = null;
  for (const inn of s.w.inns) {
    if (!inn.alive) continue;
    if (!path.some((t, k) => k > 1 && k < path.length - 2 && s.g.dist(t, inn.tile) <= 4)) continue;
    const p1 = s.path(a, inn.tile), p2 = s.path(inn.tile, b);
    if (!p1 || !p2) continue;
    const p = [...p1, ...p2.slice(1)];
    if (p.length <= path.length * 1.3 + 2 && (!best || p.length < best.length)) best = p;
  }
  return best ?? path;
}

export function supplyArrive(s: Sim, a: Agent): boolean {
  const inn = innById(s, a.inn);
  if (!inn) return true;
  if (inn.order?.agent === a.id) inn.order = undefined;
  if (inn.stage === 'ruin' || inn.stage === 'road') return true;
  const L = INN_LEVEL[Math.max(1, inn.level)];
  let food = 0, ale = 0, wood = 0, stone = 0;
  for (const [g, q0] of Object.entries(a.cargo ?? {})) {
    const q = q0 ?? 0;
    if (g === 'grain') { const toFood = Math.min(q, Math.ceil(Math.max(0, L.food - inn.stock.food - food) / GRAIN_FOOD)); food += toFood * GRAIN_FOOD; ale += (q - toFood) * GRAIN_ALE; }
    else if (g === 'fish' || g === 'meat') food += q * GRAIN_FOOD;
    else if (g === 'bread') food += q * 24;
    else if (g === 'beer') ale += q * BEER_ALE;
    else if (g === 'wood') wood += q;
    else if (g === 'stone') stone += q;
  }
  inn.stock.food = Math.min(L.food * 1.2, inn.stock.food + food);
  inn.stock.ale = Math.min(L.ale * 1.2, inn.stock.ale + ale);
  if (inn.build) { inn.build.wood += wood; inn.build.stone += stone; } else inn.stock.wood = Math.min(L.wood * 1.5, inn.stock.wood + wood);
  const from = s.settlement(a.from ?? -1)?.name ?? 'pazar';
  const parts = [food ? `+${Math.round(food)} porsiyon` : '', ale ? `+${Math.round(ale)} bardak` : '', wood ? `+${wood} kereste` : '', stone ? `+${stone} taş` : ''].filter(Boolean);
  addLog(s, inn, 'buy', `${ek(from, 'dan')} araba geldi: ${parts.join(', ') || 'boş'}`);
  return true;
}

// ------------------------------------------------------------ yolcular
function computeTraffic(s: Sim, inn: Inn) {
  let T = 0;
  for (const st of s.w.settlements) { if (!st.alive) continue; const d = s.g.dist(st.tile, inn.tile); if (d <= 18) T += s.pop(st) / (1 + d / 5); }
  for (const r of s.w.routes) if (r.alive && r.path.some((t, k) => k % 2 === 0 && s.g.dist(t, inn.tile) <= 2)) T += 12;
  inn.traffic = T;
}
function arrivalRate(s: Sim, inn: Inn) {
  const season = [1, 1.25, 1.05, 0.55][s.season];
  let danger = 1;
  for (const cp of s.w.camps) if (cp.alive && s.g.dist(cp.tile, inn.tile) <= 7) danger *= 0.75;
  const pantry = inn.stock.food < 5 ? 0.6 : 1;
  return Math.min(0.75, 0.04 + 0.003 * inn.traffic) * (0.6 + (inn.fame / 100) * 0.8) * season * Math.max(0.4, danger) * pantry;
}

function kindWeights(s: Sim, inn: Inn, near: Settlement[]): [GuestKind, number][] {
  const w = s.w;
  const trouble = near.some((x) => x.plague || x.starving > 0 || (x.burnedHouses ?? 0) > 0);
  const war = near.some((x) => s.inWar(w.civs[x.civ]));
  const temple = w.settlements.some((x) => x.alive && x.civics.temple && s.g.dist(x.tile, inn.tile) <= 28);
  const lib = w.settlements.some((x) => x.alive && x.civics.library && s.g.dist(x.tile, inn.tile) <= 30);
  const bardCiv = near.some((x) => w.civs[x.civ].cls === 'bard');
  const routes = w.routes.filter((r) => r.alive).length;
  return [
    ['merchant', 2 + Math.min(3, routes * 0.3)], ['pilgrim', temple ? 1.4 : 0.3], ['bard', bardCiv ? 1.5 : 0.6], ['hunter', forestNear(s, inn.tile) ? 1 : 0.4],
    ['scholar', lib ? 0.9 : 0.15], ['soldier', war ? 1.6 : 0.4], ['refugee', trouble ? 2.2 : 0], ['wanderer', 1], ['noble', s.year >= 6 && inn.level >= 2 ? 0.35 : 0],
  ];
}
const range = (s: Sim, r: [number, number]) => r[0] + s.rng.next() * (r[1] - r[0]);
const irange = (s: Sim, r: [number, number]) => s.rng.int(r[0], r[1]);

function spawnTraveler(s: Sim, inn: Inn) {
  const w = s.w;
  const coming = w.agents.filter((a) => !a.dead && a.kind === 'traveler' && a.purpose === 'come' && a.inn === inn.id).length;
  if (coming >= 6) return;
  const near = w.settlements.filter((x) => x.alive && s.g.dist(x.tile, inn.tile) <= 20);
  if (!near.length) return;
  const kind = s.rng.weighted(kindWeights(s, inn, near), (k) => k[1])![0];
  const troubled = near.filter((x) => x.plague || x.starving > 0 || (x.burnedHouses ?? 0) > 0);
  const atWar = near.filter((x) => s.inWar(w.civs[x.civ]));
  const pool = kind === 'refugee' && troubled.length ? troubled : kind === 'soldier' && atWar.length ? atWar : near;
  const from = s.rng.weighted(pool, (x) => (s.pop(x) + 3) / (1 + s.g.dist(x.tile, inn.tile) / 4))!;
  // gideceği yer: hacı tapınağa, bilgin kütüphaneye, göçmen dertsiz bir yerleşime, tüccar başka bir medeniyete
  const others = w.settlements.filter((x) => x.alive && x.id !== from.id && s.g.dist(x.tile, inn.tile) <= 30);
  const want = (f: (x: Settlement) => boolean) => others.filter(f);
  const destPool = kind === 'pilgrim' ? want((x) => !!x.civics.temple) : kind === 'scholar' ? want((x) => !!x.civics.library)
    : kind === 'refugee' ? want((x) => !x.plague && x.starving <= 0) : kind === 'merchant' ? want((x) => x.civ !== from.civ) : others;
  const dests = destPool.length ? destPool : others;
  const to = dests.length ? s.rng.weighted(dests, (x) => (s.pop(x) + 5) / (1 + s.g.dist(x.tile, inn.tile) / 6))! : undefined;
  const def = GUEST[kind];
  const race = raceFrom(s, from);
  let n = irange(s, def.n);
  if (kind === 'refugee') { n = Math.min(n, Math.max(0, s.pop(from) - 4)); if (n <= 0) return; }
  const c = w.civs[from.civ];
  const toName = to?.name ?? 'uzak diyarlar';
  const topGood = (Object.keys(c.stock) as Good[]).filter((g) => g !== 'gold' && (c.stock[g] ?? 0) >= 5).sort((a, b) => (c.stock[b] ?? 0) * GOODS[b].base - (c.stock[a] ?? 0) * GOODS[a].base)[0];
  const why = kind === 'merchant' ? `${ek(toName, 'a')} ${topGood ? GOODS[topGood].name.toLocaleLowerCase('tr') : 'mal'} götürüyor`
    : kind === 'pilgrim' ? `${to?.civics.temple ? ek(toName, 'in') + ' sunağına hacca' : 'kutsal bir yere'} gidiyor`
      : kind === 'bard' ? 'şarkı söyleyip yol parası topluyor'
        : kind === 'hunter' ? 'av peşinde; hana et satacak'
          : kind === 'scholar' ? `${to?.civics.library ? ek(toName, 'in') + ' kütüphanesine' : 'eski yazıtların peşinde'} gidiyor`
            : kind === 'soldier' ? (s.inWar(c) ? `${c.name} ordusundan izinli` : `${c.name} sınır bekçisi, nöbet değişimi`)
              : kind === 'refugee' ? `${ek(from.name, 'da')}ki ${from.plague ? 'salgından' : from.starving > 0 ? 'kıtlıktan' : 'yangından'} kaçıyor`
                : kind === 'noble' ? `maiyetiyle ${ek(toName, 'a')} gidiyor`
                  : 'kısmetini arıyor';
  const path = s.path(from.tile, inn.tile);
  if (!path || path.length < 2) return;
  if (kind === 'refugee') s.removePop(from, n);
  const g: Guest = {
    id: s.id(), kind, name: personName(s, race), race, n, civ: from.civ, from: from.id, fromName: from.name, to: to?.id ?? -1, toName, why,
    purse: Math.round(range(s, def.purse) * n * 10) / 10, spent: 0, nights: irange(s, def.nights), arrived: s.day, mood: 0.7,
  };
  w.agents.push({ id: s.id(), kind: 'traveler', civ: from.civ, path, step: 0, progress: 0, speed: kind === 'noble' ? 1 : 0.8, from: from.id, to: inn.id, inn: inn.id, guest: g, purpose: 'come' });
  s.metric('innTraveler');
}

/** handan (ya da kapalı handan) hedefine yürür */
function sendOn(s: Sim, fromTile: number, g: Guest, innId: number) {
  const to = g.to >= 0 ? s.settlement(g.to) : undefined;
  if (!to || !to.alive) return;
  const path = s.path(fromTile, to.tile);
  if (!path || path.length < 2) { if (g.kind === 'refugee') s.addPop(to, g.race, g.n); return; }
  s.w.agents.push({ id: s.id(), kind: 'traveler', civ: g.civ, path, step: 0, progress: 0, speed: g.kind === 'noble' ? 1 : 0.8, from: innId, to: to.id, inn: innId, guest: g, purpose: 'leave' });
}

export function travelerArrive(s: Sim, a: Agent): boolean {
  const g = a.guest;
  if (!g) return true;
  if (a.purpose === 'come') {
    const inn = innById(s, a.inn);
    if (!inn || !inn.alive) { sendOn(s, a.path[a.path.length - 1], g, a.inn ?? -1); return true; }
    checkIn(s, inn, g);
    return true;
  }
  if (g.kind === 'refugee') {
    const st = s.settlement(g.to);
    if (st && st.alive) { s.addPop(st, g.race, g.n); s.log('migration', `${g.n} göçmen ${ek(g.fromName, 'dan')} ${ek(st.name, 'a')} ulaştı; yolda handa soluklandılar.`, { civ: st.civ, tile: st.tile }); }
  }
  return true;
}

function guestTag(g: Guest) { return `${GUEST[g.kind].icon} ${g.name}${g.n > 1 ? ` +${g.n - 1}` : ''}`; }

function checkIn(s: Sim, inn: Inn, g: Guest, force = false): boolean {
  const L = INN_LEVEL[inn.level];
  const occ = innOcc(inn);
  if (occ.rooms + g.n <= L.rooms) g.stable = false;
  else if (force || occ.stable + g.n <= L.stable) { g.stable = true; inn.turned++; }
  else {
    inn.turned++; inn.total.turned += g.n; inn.fame = Math.max(0, inn.fame - 0.4);
    addLog(s, inn, 'no', `${guestTag(g)} (${GUEST[g.kind].name.toLocaleLowerCase('tr')}) yer bulamadı, ${g.toName} yoluna devam etti`);
    if (g.kind !== 'caravan') sendOn(s, inn.tile, g, inn.id);
    return false;
  }
  g.arrived = s.day;
  inn.guests.push(g);
  inn.total.guests += g.n;
  book(s, inn).guests += g.n;
  let extra = '';
  if (g.kind === 'hunter' && inn.gold > 3) { inn.stock.food += 15; inn.gold -= 1.5; book(s, inn).supply += 1.5; g.purse += 1.5; extra = ' · hana av eti sattı'; }
  if (g.kind === 'bard') { inn.fame = Math.min(100, inn.fame + 0.8); extra = ' · akşam ocak başında çalacak'; }
  if (g.kind === 'refugee') inn.fame = Math.min(100, inn.fame + 0.4);
  addLog(s, inn, 'in', `${guestTag(g)} — ${GUEST[g.kind].name.toLocaleLowerCase('tr')}, ${ek(g.fromName, 'dan')} geldi${g.stable ? ' (ahırda yatacak)' : ''}${extra}`);
  return true;
}

function checkOut(s: Sim, inn: Inn, g: Guest, why = '') {
  inn.guests = inn.guests.filter((x) => x !== g);
  if (g.kind === 'caravan') {
    const a = s.w.agents.find((x) => x.id === g.agent && !x.dead);
    if (a) a.restUntil = s.day;
    addLog(s, inn, 'out', `${guestTag(g)} yola koyuldu → ${g.toName}${why ? ` (${why})` : ''}`, g.spent);
    return;
  }
  addLog(s, inn, 'out', `${guestTag(g)} ayrıldı → ${g.toName}${why ? ` (${why})` : ''}`, g.spent);
  sendOn(s, inn.tile, g, inn.id);
}

/** kervan hanın önünden geçerken bir gece konaklar (tayfanın yemeğini medeniyet hazinesi öder) */
export function caravanPassInn(s: Sim, a: Agent): boolean {
  const tile = a.path[a.step];
  const z = s.w.tiles[tile].innZone;
  if (z === undefined || a.restedAt?.includes(z) || a.path.length - 1 - a.step < 3) return false;
  const inn = s.w.inns.find((x) => x.id === z && x.alive);
  if (!inn) return false;
  a.restedAt = [...(a.restedAt ?? []), z];
  const c = s.w.civs[a.civ];
  const from = s.settlement(a.from ?? -1), to = s.settlement(a.to ?? -1);
  const cargo = Object.entries(a.cargo ?? {}).filter(([, v]) => (v ?? 0) >= 1).map(([k, v]) => `${Math.round(v!)} ${GOODS[k as Good].name.toLocaleLowerCase('tr')}`).slice(0, 2).join(', ');
  const g: Guest = {
    id: s.id(), kind: 'caravan', name: `${c.name} kervanı`, race: c.race, n: 2 + Math.min(4, a.troops ?? 0), civ: c.id, from: from?.id ?? -1, fromName: from?.name ?? '?', to: to?.id ?? -1, toName: to?.name ?? '?',
    why: cargo ? `${cargo} taşıyor` : 'boş dönüyor', purse: 0, spent: 0, nights: 1, arrived: s.day, agent: a.id, mood: 0.7,
  };
  if (!checkIn(s, inn, g)) return false;
  a.restUntil = s.day + 1;
  a.inn = inn.id;
  s.metric('innCaravan');
  return true;
}

// ------------------------------------------------------------ kahramanlar: hanın müdavimleri
function syncHeroes(s: Sim, inn: Inn) {
  const present = s.w.heroes.filter((h) => h.civ === -1 && h.state === 'tavern' && h.tavern === inn.id);
  const ids = new Set(present.map((h) => h.id));
  for (const g of inn.guests.filter((x) => x.kind === 'hero' && !ids.has(x.hero!))) {
    const h = s.hero(g.hero!);
    const why = h.state === 'dead' ? 'öldü' : h.state === 'retired' ? 'kılıcını astı' : h.state === 'gone' ? 'uzak diyarlara gitti'
      : h.civ >= 0 ? `${s.w.civs[h.civ].name} ile sözleşme imzaladı` : h.goal ? `yola çıktı: ${h.goal.text}` : 'yola çıktı';
    inn.guests = inn.guests.filter((x) => x !== g);
    addLog(s, inn, 'out', `★ ${heroLabel(h)} ${why}`, g.spent);
  }
  for (const h of present) {
    if (inn.guests.some((x) => x.hero === h.id)) continue;
    const L = INN_LEVEL[inn.level];
    const occ = innOcc(inn);
    const fresh = s.day - h.born <= 5;
    const g: Guest = {
      id: s.id(), kind: 'hero', name: heroLabel(h), race: h.race, n: 1, civ: -1, from: -1, fromName: fresh ? 'yollardan' : (h.journal[h.journal.length - 1]?.text ?? 'yollardan'),
      to: -1, toName: '', why: `Sv ${h.level} ${HERO_CLASS_TR[h.cls]}`, purse: 0, spent: 0, nights: 0, arrived: s.day, hero: h.id, mood: 0.75, stable: occ.rooms + 1 > L.rooms,
    };
    inn.guests.push(g);
    inn.total.guests++;
    book(s, inn).guests++;
    let extra = '';
    const tab = inn.tabs[h.id] ?? 0;
    if (tab >= 10 && h.gold > 0) {
      const pay = Math.min(h.gold, Math.floor(tab / 10));
      h.gold -= pay; inn.tabs[h.id] = tab - pay * 10; inn.gold += pay; inn.total.income += pay; book(s, inn).room += pay;
      extra = ` · veresiye borcunu ödedi (${pay} altın)`;
    }
    addLog(s, inn, 'in', `★ ${heroLabel(h)} ${fresh ? 'kapıdan girdi: yeni bir yüz' : 'döndü'} (Sv ${h.level} ${HERO_CLASS_TR[h.cls]})${extra}`);
  }
}

// ------------------------------------------------------------ gün
function nightly(s: Sim, inn: Inn) {
  const pr = innPrices(inn), bk = book(s, inn);
  const occ = innOcc(inn), cap = serveCap(inn);
  const bard = inn.guests.some((g) => g.kind === 'bard');
  let satSum = 0, satN = 0, helpers = 0;
  for (const g of inn.guests.slice()) {
    const def = GUEST[g.kind];
    const poorHero = g.kind === 'hero' && s.hero(g.hero!).gold < 1;   // kesesi boş: ekmek, çorba, bir yudum bira
    const eat = poorHero ? 1 : g.n * def.eat, mugs = poorHero ? 0.4 : g.n * def.mugs * (bard && g.kind !== 'bard' ? 1.3 : 1);
    const gotFood = inn.stock.food >= eat;
    inn.stock.food = Math.max(0, inn.stock.food - eat);
    const aleGot = Math.min(inn.stock.ale, mugs);
    inn.stock.ale -= aleGot;
    const roomS = (g.stable ? Math.ceil(pr.room / 2) : pr.room) * g.n, foodS = gotFood ? pr.meal * g.n : 0, aleS = aleGot * pr.ale;
    const billS = roomS + foodS + aleS;
    let paid = 0;
    if (g.kind === 'hero') {
      const h = s.hero(g.hero!);
      let tab = (inn.tabs[h.id] ?? 0) + billS;
      if (h.gold >= 1 && tab >= 10) { const pay = Math.min(h.gold, Math.floor(tab / 10)); h.gold -= pay; tab -= pay * 10; paid = pay; }
      if (h.gold < 1 && tab >= 30) {
        // kesesi boş kahraman borcunu emeğiyle öder: avlanır, nöbet tutar, yaraları sarar, hikâye anlatır
        tab = Math.max(0, tab - billS * 1.1);
        g.why = `Sv ${h.level} ${HERO_CLASS_TR[h.cls]} · kesesi boş, ${HERO_WORK[h.cls]}`;
        if (h.cls === 'ranger' || h.cls === 'barbarian' || h.cls === 'druid') inn.stock.food += 1.6;
        else if (h.cls === 'fighter' || h.cls === 'paladin') inn.fame = Math.min(100, inn.fame + 0.004);
        else if (h.cls === 'rogue' && s.rng.chance(0.04)) { const mark = inn.guests.find((x) => x.purse >= 1 && x.kind !== 'hero'); if (mark) { mark.purse -= 1; h.gold += 1; } }
        helpers += h.cls === 'cleric' || h.cls === 'wizard' ? 1 : 0;
      } else g.why = `Sv ${h.level} ${HERO_CLASS_TR[h.cls]}${h.gold ? ` · kesesinde ${h.gold} altın` : ''}`;
      inn.tabs[h.id] = Math.min(300, tab);
    } else if (g.kind === 'caravan') {
      const c = s.w.civs[g.civ];
      paid = Math.min(billS / 10, Math.max(0, s.st(c, 'gold')));
      s.add(c, 'gold', -paid);
    } else if (g.kind === 'refugee' || g.kind === 'bard') {
      // göçmen elinden geleni öder; ozan şarkılarıyla öder
      paid = Math.min(g.purse, (billS / 10) * 0.3); g.purse -= paid;
    } else {
      paid = Math.min(g.purse, billS / 10); g.purse -= paid;
    }
    paid = Math.round(paid * 100) / 100;
    g.spent += paid; inn.gold += paid; inn.total.income += paid; inn.total.nights += g.n; bk.nights += g.n;
    if (billS > 0 && paid > 0) { const k = paid / (billS / 10); bk.room += (roomS / 10) * k; bk.food += (foodS / 10) * k; bk.ale += (aleS / 10) * k; }
    // memnuniyet
    let m = 0.72 + 0.04 * (inn.level - 1);
    if (!gotFood) m -= 0.35;
    if (def.mugs > 0.5 && aleGot < mugs * 0.6) m -= 0.15;
    if (g.stable) m -= 0.18;
    if (occ.persons > cap) m -= 0.15 * Math.min(1, (occ.persons - cap) / cap);
    if (s.season === 3 && inn.stock.wood <= 0) m -= 0.25;
    if (bard && g.kind !== 'bard') m += 0.08;
    m += Math.min(0.06, helpers * 0.03);
    g.mood = g.mood * 0.5 + Math.max(0, Math.min(1, m)) * 0.5;
    satSum += g.mood * g.n; satN += g.n;
    // ayrılış
    if (g.kind === 'hero') continue;
    const stayed = s.day - g.arrived;
    const broke = g.kind !== 'refugee' && g.kind !== 'caravan' && g.kind !== 'bard' && g.purse < 0.3;
    if (stayed >= g.nights) checkOut(s, inn, g);
    else if (broke && stayed >= 1) checkOut(s, inn, g, 'kesesi boşaldı');
    else if (g.mood < 0.35 && stayed >= 1) { checkOut(s, inn, g, 'memnun kalmadı'); inn.fame = Math.max(0, inn.fame - 0.6); }
  }
  // hancı ve personel de yer; ocak yanar; bahçe ve kümes
  const L = INN_LEVEL[inn.level];
  inn.stock.food = Math.max(0, inn.stock.food - (1 + inn.staff.length));
  if (s.season !== 3) inn.stock.food = Math.min(L.food * 1.2, inn.stock.food + 1.8 * inn.level);   // bahçe, kümes, küçük tarla
  // bira biterse hancı kilerdeki tahıldan kendi birasını mayalar
  if (inn.stock.ale < L.ale * 0.2 && inn.stock.food > L.food * 0.5) { inn.stock.food -= 5; inn.stock.ale += (5 / GRAIN_FOOD) * GRAIN_ALE; }
  // geçenlerden yol parası: at sulama, yem, nal, su
  const toll = 0.04 + Math.min(200, inn.traffic) * 0.0014;
  inn.gold += toll; bk.other += toll; inn.total.income += toll;
  const woodUse = 0.15 + (s.season === 3 ? 0.35 : s.season === 0 || s.season === 2 ? 0.1 : 0) + occ.persons * 0.02;
  if (forestNear(s, inn.tile) && inn.stock.wood < L.wood) inn.stock.wood += 0.45;   // çırak ya da seyis odun keser
  inn.stock.wood = Math.max(0, inn.stock.wood - woodUse);
  if (satN) inn.sat = inn.sat * 0.8 + (satSum / satN) * 0.2;
  const legends = s.w.heroes.filter((h) => h.legend && h.baseInn && h.base === inn.id).length;
  const target = 10 + inn.sat * 60 + inn.level * 4 + Math.min(12, legends * 4) + Math.min(6, inn.raids);
  inn.fame = Math.max(0, Math.min(100, inn.fame + (target - inn.fame) * (satN ? 0.012 : 0.004)));
}

function restock(s: Sim, inn: Inn) {
  if (inn.order) return;
  const L = INN_LEVEL[inn.level];
  const lowF = inn.stock.food < L.food * 0.35, lowA = inn.stock.ale < L.ale * 0.3;
  const lowW = !forestNear(s, inn.tile) && inn.stock.wood < L.wood * 0.3;
  if (!lowF && !lowA && !lowW) return;
  placeOrder(s, inn, { food: Math.max(0, L.food - inn.stock.food), ale: Math.max(0, L.ale - inn.stock.ale), wood: lowW ? Math.ceil(L.wood - inn.stock.wood) : 0 });
}

function ensureStaff(s: Sim, inn: Inn) {
  if (inn.stage !== 'open') return;
  const want = INN_LEVEL[inn.level].staff.slice();
  const have = inn.staff.map((x) => x.role);
  for (const r of have) { const k = want.indexOf(r); if (k >= 0) want.splice(k, 1); }
  // kalabalık hanın fazladan garsonu
  const busy = inn.hist.slice(-6);
  if (busy.length >= 6 && busy.reduce((a, h) => a + h.guests, 0) / busy.length > serveCap(inn) && inn.staff.filter((x) => x.role === 'garson').length < 3 && inn.gold > 70) want.push('garson');
  const home = nearestSettlements(s, inn.tile, 1)[0]?.x;
  for (const role of want) {
    if (inn.gold < STAFF[role].wage * 3) break;
    const race = raceFrom(s, home);
    const x = { name: personName(s, race), race, role: role as StaffRole, since: s.day, from: home?.name ?? 'yollardan' };
    inn.staff.push(x);
    addLog(s, inn, 'staff', `${x.name} ${STAFF[role].name.toLocaleLowerCase('tr')} olarak işe başladı (${x.from})`);
  }
}

function payWages(s: Sim, inn: Inn) {
  const due = wages(inn);
  if (!due) return;
  if (inn.gold >= due) { inn.gold -= due; book(s, inn).wage += due; return; }
  const paid = Math.max(0, inn.gold);
  inn.gold -= paid; book(s, inn).wage += paid;
  // kasa boş: en son işe giren (çırak değilse) ayrılır
  const k = inn.staff.map((x, i) => ({ x, i })).filter((q) => q.x.role !== 'cirak').sort((a, b) => b.x.since - a.x.since)[0];
  if (k) { inn.staff.splice(k.i, 1); addLog(s, inn, 'staff', `${k.x.name} (${STAFF[k.x.role].name.toLocaleLowerCase('tr')}) ücretini alamayınca ayrıldı`); }
}

function considerUpgrade(s: Sim, inn: Inn) {
  if (inn.stage !== 'open' || inn.build || inn.level >= 3) return;
  const u = INN_UPGRADE[inn.level + 1];
  if (s.year < u.year || inn.fame < u.fame || inn.gold < u.gold + 30) return;
  const h = inn.hist.slice(-12);
  const busy = h.length ? h.reduce((a, x) => a + x.guests, 0) / h.length : 0;
  if (inn.turned < 5 && busy < INN_LEVEL[inn.level].rooms * 0.65) return;
  inn.gold -= u.gold;
  book(s, inn).build += u.gold;
  inn.build = { level: inn.level + 1, work: 0, need: u.work, wood: 0, woodNeed: u.wood, stone: 0, stoneNeed: u.stone, started: s.day };
  addLog(s, inn, 'build', `Genişletme başladı: ${INN_LEVEL[inn.level + 1].name} olacak (${u.gold} altın)`, -u.gold);
  s.log('inn', `Hancı ${inn.keeper}, ${ek(innTitle(inn), 'i')} büyütüyor: yeni kanat, daha büyük ahır. ${INN_LEVEL[inn.level + 1].name} olacak.`, { tile: inn.tile, major: true, cause: inn.turned >= 5 ? `Son yıl ${inn.turned} yolcu yer bulamadı` : `Odalar hep dolu · ün ${Math.round(inn.fame)}` });
}

/** kasa taşınca hancı kazancın bir kısmını memleketine yollar (akrabalar, vergi, borç) */
function remit(s: Sim, inn: Inn) {
  if (inn.gold <= 450) return;
  const home = s.settlement(inn.origin) ?? nearestSettlements(s, inn.tile, 1)[0]?.x;
  if (!home || !home.alive) return;
  const g = Math.round((inn.gold - 450) * 0.4);
  const c = s.w.civs[home.civ];
  inn.gold -= g; s.add(c, 'gold', g); book(s, inn).other -= g;
  addLog(s, inn, 'ev', `Hancı ${inn.keeper} kazancından ${g} altını memleketi ${ek(home.name, 'a')} yolladı`, -g);
}

/** usta hanın çırağı yetişince kendi hanını kurmaya gider */
function considerApprentice(s: Sim, inn: Inn) {
  if (inn.level < 2 || s.year < 5) return;
  const active = s.w.inns.filter((x) => x.stage !== 'ruin').length;
  if (active >= s.w.innPlan.target + (s.year >= 12 ? 1 : 0)) return;
  const ap = inn.staff.find((x) => x.role === 'cirak' && s.day - x.since >= 2 * YEAR);
  if (!ap || !s.rng.chance(0.4)) return;
  if (!dispatchKeeper(s, { fromInn: inn, name: ap.name, race: ap.race })) return;
  inn.staff = inn.staff.filter((x) => x !== ap);
  addLog(s, inn, 'staff', `Çırak ${ap.name} ustasının hayır duasıyla kendi hanını kurmaya gitti`);
}

export function innLifeTick(s: Sim) {
  const w = s.w;
  // açılış dalgası: oyun başında birkaç hancı yerleşimlerden çıkar
  const plan = w.innPlan;
  if (plan.wave > 0 && s.day >= plan.next) {
    if (dispatchKeeper(s)) plan.wave--; else if (s.day > 90) plan.wave = 0;
    plan.next = s.day + s.rng.int(2, 5);
  }
  if (s.day % 90 === 45 && plan.wave === 0 && w.inns.length < plan.target && s.rng.chance(0.5)) dispatchKeeper(s);
  for (const inn of w.inns) {
    if (inn.stage === 'road') continue;
    if (inn.stage === 'ruin') {
      if (s.day % 30 === 0 && s.day - (inn.ruinedDay ?? 0) >= 3 * YEAR && !w.camps.some((c) => c.alive && s.g.dist(c.tile, inn.tile) <= 10)) dispatchKeeper(s, { rebuild: inn });
      continue;
    }
    if (inn.build) buildTick(s, inn);
    if (inn.stage !== 'open') continue;
    if ((s.day + inn.id) % 10 === 0) {
      computeTraffic(s, inn);
      inn.hist.push({ day: s.day, guests: innOcc(inn).persons, gold: Math.round(inn.gold), fame: Math.round(inn.fame) });
      if (inn.hist.length > 72) inn.hist.shift();
    }
    syncHeroes(s, inn);
    if (s.rng.chance(arrivalRate(s, inn))) spawnTraveler(s, inn);
    nightly(s, inn);
    restock(s, inn);
    if ((s.day + inn.id) % 30 === 0) { payWages(s, inn); ensureStaff(s, inn); considerUpgrade(s, inn); }
    if ((s.day + inn.id) % YEAR === 0) { inn.turned = Math.floor(inn.turned / 2); considerApprentice(s, inn); remit(s, inn); }
  }
}

/** baskında han boşalır: misafirler kaçar, kiler yağmalanır */
export function innScatter(s: Sim, inn: Inn, why: string, ruin: boolean) {
  for (const g of inn.guests.slice()) if (g.kind !== 'hero') checkOut(s, inn, g, why);
  inn.stock.food *= ruin ? 0 : 0.5; inn.stock.ale *= ruin ? 0 : 0.5;
  if (ruin) {
    inn.guests = [];
    inn.stage = 'ruin'; inn.build = undefined;
    for (const x of inn.staff) addLog(s, inn, 'staff', `${x.name} kaçtı`);
    inn.staff = []; inn.fame *= 0.5; inn.stock.wood = 0; inn.order = undefined;
  } else inn.fame = Math.max(0, inn.fame - 8);
}
