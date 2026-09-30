// Tarafsız hanlar: serbest kahraman havuzu, açık artırma, sözleşme, misafir hakkı, hanın panosu.
import type { Sim } from './sim';
import { YEAR } from './sim';
import { ek } from './tr';
import { CLASSES, UNITS, type RaceId } from '../data/classes';
import { HERO_CLASS_IDS } from '../data/heroes';
import { powerOf, resolveBattle, unit, type Combatant } from './combat';
import { monsterSide, monsterName } from './monsters';
import { civTroops, drawSoldiers, recordBattle, syncHeroes, raidReturn, armyReturn } from './agents';
import { makeHero, heroBaseCost } from './heroes';
import { heroSide, heroWillServe, innById, maybeRetire, nearestRest, note, returnToBase } from './will';
import { innLifeTick, innIncome, innEvent, innScatter } from './innlife';
import type { Agent, Civ, Hero, Inn } from './types';

export const INN_POOL = 6;
export const CONTRACT_YEARS = 5;
export const MAX_INN_HEROES = 2;

export function innPool(s: Sim, inn: Inn) {
  return s.w.heroes.filter((h) => h.baseInn && h.base === inn.id && h.civ === -1 && h.state !== 'dead' && h.state !== 'gone' && h.state !== 'retired');
}
export function innHeroesOf(s: Sim, c: Civ) { return s.w.heroes.filter((h) => h.contract?.civ === c.id && h.civ === c.id && h.state !== 'dead' && h.state !== 'gone'); }
export function isPact(s: Sim, c: Civ) { return s.e(c, 'pact') > 0 || c.align.good < -0.5; }
export function innName(inn: Inn) { return `${inn.name} Hanı`; }

// ------------------------------------------------------------ doğum
function spawnInnHero(s: Sim, inn: Inn) {
  const races = [...new Set(s.w.civs.filter((c) => c.alive).map((c) => c.race))];
  const race: RaceId = s.rng.chance(0.15) ? s.rng.pick(['halfelf', 'halforc'] as RaceId[]) : s.rng.pick(races.length ? races : ['human' as RaceId]);
  const cls = s.rng.pick(HERO_CLASS_IDS);
  const teacher = inn.teacher !== undefined && s.w.heroes.find((h) => h.id === inn.teacher)?.state === 'retired';
  let level = s.rng.chance(0.45) ? 1 : s.rng.chance(0.6) ? 2 : 3;
  level = Math.min(5, level + Math.floor(s.year / 8) + (teacher ? 1 : 0));
  const h = makeHero(s, { race, cls, level, tile: inn.tile, base: inn.id, baseInn: true });
  s.metric('innHeroSpawn');
  s.log('inn', `${innName(inn)} kapısından bir yabancı girdi: ${s.heroTitle(h)}.`, { tile: inn.tile, cause: `${h.bio.charAt(0).toUpperCase() + h.bio.slice(1)}${teacher ? ' Emekli bir kahramanın yanında yetişti.' : ''}` });
}

// ------------------------------------------------------------ tik
export function innsTick(s: Sim) {
  const w = s.w;
  innLifeTick(s);   // hancılar, inşaat, misafirler, kiler, defter (her gün)
  if (s.day % 5 !== 0) return;
  for (const inn of w.inns) {
    if (!inn.alive) continue;
    const pool = innPool(s, inn);
    // yılda ~1–2; ünlü hana kahraman daha çok uğrar
    const p = (pool.length === 0 ? 0.1 : pool.length < INN_POOL ? 0.045 : 0) * (0.8 + (inn.fame / 100) * 0.35);
    if (s.rng.chance(p)) spawnInnHero(s, inn);
    for (const h of pool) {
      if (!h.auction && h.state === 'tavern' && h.tavern === inn.id) h.auction = { end: s.day + YEAR, bids: [] };
      else if (h.auction && s.day >= h.auction.end) closeAuction(s, inn, h);
    }
    if (s.day % 30 === 0) postGuardQuest(s, inn);
  }
  for (const h of w.heroes) if (h.contract && s.day >= h.contract.until && h.state === 'home') contractEnd(s, h);
  if (s.day % 30 === 0) for (const c of w.civs) if (c.alive) considerInnRaid(s, c);
  // süresi dolan ilanlar ödülü iade eder
  for (const q of w.quests) if (q.open && q.expires !== undefined && s.day >= q.expires) {
    q.open = false;
    if (q.civ >= 0) s.add(w.civs[q.civ], 'gold', q.bounty); else { const i = innById(s, q.inn); if (i) innIncome(s, i, q.bounty, `İlanın süresi doldu; ${q.bounty} altın ödül kasaya döndü`); }
    s.metric('questExpired');
  }
}

// ------------------------------------------------------------ açık artırma
/** medeniyet, hanlardaki açık artırmalara teklif verir */
export function considerInnBids(s: Sim, c: Civ) {
  if ((c.innBanUntil ?? 0) > s.day || innHeroesOf(s, c).length >= MAX_INN_HEROES) return;
  const war = s.inWar(c), gold = s.st(c, 'gold');
  const mine = s.civHeroes(c).length;
  if (!(c.threat > 0.25 || war || (mine === 0 && gold > 90) || gold > 260)) return;
  const cap = s.capital(c);
  if (!cap) return;
  const already = s.w.heroes.filter((h) => h.civ === -1 && h.state !== 'dead' && h.state !== 'gone' && h.auction?.bids.some((b) => b.civ === c.id)).length;
  if (already >= MAX_INN_HEROES - innHeroesOf(s, c).length) return;
  const cands = s.w.heroes.filter((h) => h.civ === -1 && h.auction && h.baseInn && (h.state === 'tavern' || h.state === 'quest' || h.state === 'traveling') && heroWillServe(c, h) && !h.auction.bids.some((b) => b.civ === c.id))
    .filter((h) => { const i = innById(s, h.base); return i && i.alive && s.g.dist(i.tile, cap.tile) <= 30; })
    .sort((a, b) => b.level - a.level || (b.cls === CLASSES[c.cls].heroClass ? 1 : 0) - (a.cls === CLASSES[c.cls].heroClass ? 1 : 0) || a.id - b.id);
  const h = cands[0];
  if (!h) return;
  const K = heroBaseCost(h);
  const need = (c.threat > 0.35 || war ? 3 : 2) + (CLASSES[c.cls].heroClass === h.cls ? 0.3 : 0);
  const max = Math.min(gold * 0.6, K * need);
  const top = Math.max(0, ...h.auction!.bids.map((b) => effBid(s, h, b)));
  const mult = (h.rep[c.id] ?? 0) >= 3 ? 1.25 : 1;
  if (max < K * 1.5 || max * mult <= top) return;
  const bid = Math.round(Math.min(max, Math.max(K * 1.5, top / mult * 1.1 + 5)));
  h.auction!.bids.push({ civ: c.id, gold: bid });
  s.metric('innBid');
}
function effBid(s: Sim, h: Hero, b: { civ: number; gold: number }) { return b.gold * ((h.rep[b.civ] ?? 0) >= 3 ? 1.25 : 1); }

function closeAuction(s: Sim, inn: Inn, h: Hero) {
  const a = h.auction!;
  const busy = s.w.agents.some((x) => !x.dead && x.kind === 'party' && x.heroes?.includes(h.id));
  if (busy) { a.end = s.day + 20; return; }
  const ok = a.bids.filter((b) => {
    const c = s.w.civs[b.civ];
    return c.alive && s.st(c, 'gold') >= b.gold && (c.innBanUntil ?? 0) <= s.day && innHeroesOf(s, c).length < MAX_INN_HEROES && heroWillServe(c, h);
  }).sort((x, y) => effBid(s, h, y) - effBid(s, h, x) || (s.w.civs[y.civ].race === h.race ? 1 : 0) - (s.w.civs[x.civ].race === h.race ? 1 : 0) || x.civ - y.civ);
  h.auction = undefined;
  if (!ok.length) return;
  const b = ok[0], c = s.w.civs[b.civ];
  s.add(c, 'gold', -b.gold);
  innIncome(s, inn, Math.round(b.gold * 0.1), `Açık artırma: ${c.name}, ${h.name} için ${b.gold} altın verdi; hanın payı %10`);
  for (const x of s.w.agents) if (!x.dead && x.kind === 'hero' && x.heroes?.includes(h.id)) x.dead = true;
  h.goal = undefined;
  h.civ = c.id;
  h.contract = { civ: c.id, since: s.day, until: s.day + CONTRACT_YEARS * YEAR, paid: b.gold };
  h.hired = (h.hired ?? 0) + 1;
  note(s, h, `${c.name} ile ${CONTRACT_YEARS} yıllık sözleşme (${b.gold} altın)`);
  s.metric('innHire'); s.metric(`innHire_${c.id}`);
  const rivals = a.bids.length - 1;
  s.log('inn', `${innName(inn)}'nda açık artırma: ${c.name}, ${s.heroTitle(h)} için ${b.gold} altınla ${CONTRACT_YEARS} yıllık sözleşme imzaladı.`, { civ: c.id, tile: inn.tile, major: true, cause: rivals > 0 ? `${rivals} rakip teklif geride kaldı` : 'Tek teklif' });
  const cap = s.capital(c);
  if (cap) { const t = h.pos; h.tavern = -1; if (t === cap.tile) h.state = 'home'; else { h.state = 'traveling'; const path = s.path(t, cap.tile); if (path) s.w.agents.push({ id: s.id(), kind: 'hero', civ: c.id, path, step: 0, progress: 0, speed: 0.9, heroes: [h.id], purpose: 'home' }); else { h.pos = cap.tile; h.state = 'home'; } } }
}

function contractEnd(s: Sim, h: Hero) {
  const k = h.contract!, c = s.w.civs[k.civ];
  const years = Math.floor((s.day - k.since) / YEAR);
  if (maybeRetire(s, h)) return;
  const pay = Math.max(0, Math.min(3, Math.floor(k.paid / heroBaseCost(h) - 1)));
  const roll = s.rng.dice(1, 20);
  const total = roll + years + pay + Math.min(2, Math.max(0, h.rep[c.id] ?? 0));
  if (total >= 12) {
    k.until = s.day + CONTRACT_YEARS * YEAR;
    note(s, h, `${c.name} ile sözleşmesini yeniledi`);
    s.log('hero', `${h.name}, ${c.name} ile sözleşmesini ${CONTRACT_YEARS} yıl uzattı.`, { civ: c.id, tile: h.pos, cause: `Sadakat zarı ${roll} + ${years + pay} = ${total} (DC 12)` });
    return;
  }
  h.civ = -1; h.contract = undefined;
  s.metric('contractLeave');
  note(s, h, `${c.name} ile sözleşmesi bitti; hana döndü`);
  s.log('hero', `${h.name}, ${c.name} ile sözleşmesi bitince eşyasını toplayıp hana döndü.`, { civ: c.id, tile: h.pos, major: true, cause: `Sadakat zarı ${roll} + ${years + pay} = ${total} (DC 12)` });
  returnToBase(s, h);
}

// ------------------------------------------------------------ hanın panosu
function postGuardQuest(s: Sim, inn: Inn) {
  const w = s.w;
  // zengin han, panosundaki medeniyet ilanlarının ödülüne katkı koyar (yolları güvenli olsun, yolcu gelsin)
  if (inn.gold > 220) for (const q of w.quests) {
    if (!q.open || q.inn !== inn.id || q.civ < 0 || q.topped) continue;
    const add = Math.round(Math.min(inn.gold * 0.15, q.bounty * 0.4));
    if (add < 5) continue;
    q.bounty += add; q.topped = add; inn.gold -= add;
    innEvent(s, inn, `Hancı ${w.civs[q.civ].name} ilanının ödülüne ${add} altın ekledi`);
  }
  if (inn.gold < 40) return;
  const mine = w.quests.filter((q) => q.open && q.civ === -1 && q.inn === inn.id);
  if (mine.length >= Math.max(1, inn.level)) return;
  const cp = w.camps.filter((c) => c.alive && !w.tiles[c.tile].isle && s.g.dist(c.tile, inn.tile) <= 12 && !w.quests.some((q) => q.open && q.camp === c.id && q.civ === -1))
    .sort((a, b) => s.g.dist(a.tile, inn.tile) - s.g.dist(b.tile, inn.tile))[0];
  if (!cp) return;
  const base = 30 + (cp.kind === 'hobgoblin' ? 25 : cp.kind === 'bugbear' || cp.kind === 'pirate' ? 35 : 0);
  const bounty = Math.round(Math.min(inn.gold * 0.35, base * (1 + (Math.max(1, inn.level) - 1) * 0.4)));
  inn.gold -= bounty;
  w.quests.push({ id: s.id(), civ: -1, camp: cp.id, bounty, posted: s.day, takenBy: [], open: true, inn: inn.id, expires: s.day + 3 * YEAR });
  s.metric('innQuest');
  innEvent(s, inn, `Panoya ilan asıldı: ${cp.name} temizlensin (${bounty} altın)`);
  s.log('quest', `Hancı ${inn.keeper}, ${innName(inn)} panosuna ilan astı: "${cp.name} temizlensin, ödül ${bounty} altın."`, { tile: inn.tile, major: true, cause: `${cp.name} hana ${s.g.dist(cp.tile, inn.tile)} fersah` });
}

// ------------------------------------------------------------ misafir hakkı
/** hana ya da handaki kahramana saldıran medeniyet herkesle bozuşur ve 5 yıl kahraman kiralayamaz */
export function breakGuestRight(s: Sim, c: Civ, inn: Inn) {
  const pact = isPact(s, c);
  const pen = pact ? -15 : -30;
  for (const o of s.w.civs) {
    if (!o.alive || o.id === c.id) continue;
    s.setMod(o.id, c.id, 'innbreak', `${c.name} ${ek(innName(inn), 'i')} bastı: misafir hakkı çiğnendi`, pen, 0.05);
    if (s.e(o, 'crusade') > 0) s.setMod(o.id, c.id, 'innbreak_holy', 'Han baskını paladinlerin öfkesini alevlendirdi', -15, 0.03, false);
  }
  c.innBanUntil = s.day + 5 * YEAR;
  s.metric('innBreak');
  s.log('inn', `${c.name} ${ek(innName(inn), 'i')} bastı; bütün medeniyetler onlara sırt çevirdi. Artık "Han Bozan" olarak anılıyorlar.`, { civ: c.id, tile: inn.tile, major: true, cause: `Misafir hakkı çiğnendi: ilişkiler ${pen}, 5 yıl hanlardan kahraman kiralayamazlar` });
}

function innDefenders(s: Sim, inn: Inn, side: 'A' | 'B'): Combatant[] {
  const d: Combatant[] = s.w.heroes.filter((h) => h.civ === -1 && h.state === 'tavern' && h.tavern === inn.id).map((h) => heroSide(h, side));
  for (let i = 0; i < 2; i++) d.push(unit(UNITS.militia, side, 'militia'));
  return d;
}

/** Paktçı hanı basar: altın ve kurban için */
function considerInnRaid(s: Sim, c: Civ) {
  if (!isPact(s, c) || s.year < 4 || (c.innBanUntil ?? 0) > s.day || s.inWar(c)) return;
  if (s.w.agents.some((a) => a.civ === c.id && a.purpose === 'innraid')) return;
  const sol = s.civSettlements(c).reduce((a, x) => a + x.soldiers, 0);
  if (sol < 6) return;
  const ss = s.civSettlements(c);
  const inn = s.w.inns.filter((i) => i.alive && ss.some((x) => s.g.dist(x.tile, i.tile) <= 14) && (i.gold >= 40 || innPool(s, i).length))
    .sort((a, b) => b.gold - a.gold)[0];
  if (!inn || !s.rng.chance(1 / 40)) return;
  const cap = s.capital(c)!;
  const n = Math.min(10, Math.floor(sol * 0.6));
  if (powerOf(civTroops(s, c, n, 'A')) < powerOf(innDefenders(s, inn, 'B')) * 1.2) return;
  const path = s.path(cap.tile, inn.tile);
  if (!path) return;
  const pop = drawSoldiers(s, c, n);
  s.w.agents.push({ id: s.id(), kind: 'army', civ: c.id, path, step: 0, progress: 0, speed: 0.7, troops: n, pop, from: cap.id, to: inn.id, purpose: 'innraid' });
  s.log('war', `${c.name} karanlık bir sefer düzenledi: ${n} asker ${ek(innName(inn), 'a')} yürüyor.`, { civ: c.id, tile: cap.tile, major: true, cause: 'Patronları altın ve kurban istiyor' });
}

export function innRaidArrive(s: Sim, a: Agent) {
  const inn = innById(s, a.to);
  const c = s.w.civs[a.civ];
  if (!inn || !inn.alive) { armyReturn(s, a); return; }
  const side = civTroops(s, c, a.troops ?? 0, 'A');
  const def = innDefenders(s, inn, 'B');
  const b = resolveBattle(s.rng, side, def, { id: s.id(), day: s.day, tile: inn.tile, title: `${innName(inn)} baskını`, sideA: `${c.name} askerleri`, sideB: `${innName(inn)} misafirleri`, moraleA: 0.55, moraleB: 0.6, timeoutWinner: 'B', civA: c.id });
  recordBattle(s, b);
  syncHeroes(s, def, b.winner === 'B' ? 90 : 10);
  const dead = side.filter((x) => x.hp <= 0).length;
  a.troops = Math.max(0, (a.troops ?? 0) - dead);
  inn.raids++;
  s.metric('innRaid');
  if (b.winner === 'A') {
    const gold = Math.floor(Math.max(0, inn.gold)); inn.gold -= gold;
    s.add(c, 'gold', gold);
    innScatter(s, inn, `${c.name} askerleri hanı bastı`, false);
    innEvent(s, inn, `${c.name} askerleri hanı yağmaladı: ${gold} altın gitti`);
    s.log('war', `${c.name} askerleri ${ek(innName(inn), 'i')} yağmaladı: ${gold} altın ve misafirlerin kanı.`, { civ: c.id, tile: inn.tile, battle: b.id, major: true });
    for (const h of s.w.heroes) if (h.civ === -1 && h.state === 'tavern' && h.tavern === inn.id) { note(s, h, `${c.name} hanı bastı`); h.grudge = c.id; }
  } else {
    s.log('war', `${innName(inn)} misafirleri ${c.name} askerlerini kapıdan geri püskürttü.`, { civ: c.id, tile: inn.tile, battle: b.id, major: true });
    inn.fame = Math.min(100, inn.fame + 4);
    innEvent(s, inn, `${c.name} baskını kapıdan geri püskürtüldü`);
  }
  breakGuestRight(s, c, inn);
  armyReturn(s, a);
}

/** canavar baskını: hanı handaki serbest kahramanlar savunur */
export function innMonsterRaid(s: Sim, a: Agent) {
  const inn = innById(s, a.to);
  if (!inn || !inn.alive) { raidReturn(s, a); return; }
  const kind = a.monster ?? 'goblin';
  const cp = s.w.camps.find((x) => x.id === a.from);
  const def = innDefenders(s, inn, 'A');
  const mons = monsterSide(kind, a.troops ?? 0, !!a.boss, 'B');
  const b = resolveBattle(s.rng, def, mons, { id: s.id(), day: s.day, tile: inn.tile, title: `${innName(inn)} baskını`, sideA: `${innName(inn)} misafirleri`, sideB: monsterName(kind), moraleA: 0.6, moraleB: 0.5, timeoutWinner: 'A' });
  recordBattle(s, b);
  syncHeroes(s, def, b.winner === 'A' ? 80 : 10);
  a.troops = mons.filter((x) => x.kind === 'monster' && x.hp > 0).length;
  a.boss = mons.some((x) => x.boss && x.hp > 0);
  inn.raids++;
  s.metric('innMonsterRaid');
  const heroes = def.filter((x) => x.hero).map((x) => x.hero!);
  if (b.winner === 'B') {
    a.loot = (a.loot ?? 0) + Math.floor(inn.gold * 0.6);
    ruinInn(s, inn, `${cp?.name ?? monsterName(kind)} baskını`, b.id);
  } else {
    for (const h of heroes) if (h.state !== 'dead') note(s, h, `${ek(innName(inn), 'i')} ${monsterName(kind).toLocaleLowerCase('tr')} baskınına karşı savundu`);
    inn.fame = Math.min(100, inn.fame + 3);
    innEvent(s, inn, `${monsterName(kind)} baskını püskürtüldü${heroes.length ? ` (${heroes.map((h) => h.name).join(', ')})` : ''}`);
    s.log('raid', `${innName(inn)} misafirleri ${monsterName(kind).toLocaleLowerCase('tr')} baskınını püskürttü${heroes.length ? `: ${heroes.map((h) => h.name).join(', ')}` : ''}.`, { tile: inn.tile, battle: b.id, major: heroes.length > 0, cause: `${cp?.name ?? 'Kamp'} hana yakın` });
  }
  raidReturn(s, a);
}

export function ruinInn(s: Sim, inn: Inn, why: string, battle?: number) {
  innEvent(s, inn, `Han yandı: ${why}`);
  innScatter(s, inn, 'han yanarken kaçtı', true);
  inn.alive = false; inn.ruinedDay = s.day; inn.teacher = undefined;
  s.metric('innRuined');
  s.log('inn', `${innName(inn)} yandı ve harabeye döndü.`, { tile: inn.tile, battle, major: true, cause: why });
  for (const q of s.w.quests) if (q.open && q.civ === -1 && q.inn === inn.id) q.open = false;
  for (const h of s.w.heroes) {
    if (h.state === 'dead' || h.state === 'gone') continue;
    if (h.state === 'retired' && h.base === inn.id) { h.state = 'gone'; continue; }
    if (!h.baseInn || h.base !== inn.id) continue;
    h.auction = undefined;
    const r = nearestRest(s, inn.tile);
    if (!r) { if (h.civ === -1) h.state = 'gone'; continue; }
    h.base = r.id; h.baseInn = r.inn;
    if (h.civ === -1 && h.state === 'tavern') returnToBase(s, h);
  }
}

