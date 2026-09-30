// Haritadaki hareketli birimler ve çatışmalar: kervan, ordu, baskın, kahraman, öncü, kâşif.
import type { Sim } from './sim';
import { ek } from './tr';
import { CLASSES, RACES, UNITS } from '../data/classes';
import { GOODS, GOOD_IDS, EXTRACTS, extPoss, type Good, type Stock } from '../data/goods';
import { TECH } from '../data/techs';
import { unit, heroCombatant, resolveBattle, type Combatant, type BattleOpts } from './combat';
import { monsterSide, monsterName } from './monsters';
import { gainXp, tryRevive, sendHero, questFailed } from './heroes';
import { arriveGoal, returnToBase, heroSide, afterCampFight, onHomeBurned, innAt, note } from './will';
import { innMonsterRaid, innRaidArrive } from './inns';
import { makeSettlement } from './worldgen';
import type { Agent, Battle, Camp, Civ, Pop, Settlement } from './types';

// ------------------------------------------------------------ birlik kompozisyonu
export function civTroops(s: Sim, c: Civ, n: number, side: 'A' | 'B'): Combatant[] {
  const out: Combatant[] = [];
  if (n <= 0) return out;
  const race = RACES[c.race];
  const armed = s.has(c, 'smithing');
  const ench = s.st(c, 'enchanted') >= n / 5;
  const mith = s.st(c, 'mithrilArmor') >= n / 5;
  const units = c.research.done.map((t) => TECH[t].unit).filter(Boolean) as string[];
  let remaining = n;
  const specials: [string, number][] = [];
  const BASE = ['holyguard', 'legionary', 'shadowguard', 'blessed'];
  for (const id of units) {
    if (BASE.includes(id)) continue;
    const u = UNITS[id];
    if (!u?.per) continue;
    let k = Math.floor(n * u.per);
    if ((id === 'golem' || id === 'treant' || id === 'dragonguard') && n >= 4) k = Math.max(1, k);
    if (id === 'knight') k = Math.min(Math.floor(s.st(c, 'horses')), k);
    if (id === 'wolves' && k === 0 && n >= 3) k = 1;
    if (k > 0) specials.push([id, k]);
  }
  const baseId = BASE.find((b) => units.includes(b)) ?? 'soldier';
  const mk = (id: string) => {
    const u = UNITS[id];
    const cb = unit(u, side, id === 'soldier' || BASE.includes(id) ? 'soldier' : 'unique', race.hp + s.e(c, 'soldierHp'), race.atk + s.e(c, 'soldierAtk') + (armed ? 1 : 0) + (ench ? 2 : 0));
    cb.ac += s.e(c, 'soldierAc') + (armed && id === 'soldier' ? 2 : 0) + (mith ? 2 : 0);
    cb.dmg[2] += s.e(c, 'soldierDmg');
    if (s.e(c, 'smite') > 0) cb.smite = 1 + s.e(c, 'smite');
    if (c.align.good < -0.3) cb.evil = true;
    return cb;
  };
  for (const [id, k] of specials) for (let i = 0; i < k && remaining > 0; i++) { out.push(mk(id)); remaining--; }
  for (let i = 0; i < remaining; i++) out.push(mk(baseId));
  return out;
}

export function drawSoldiers(s: Sim, c: Civ, n: number): Pop {
  const out: Pop = {};
  let left = n;
  for (const st of s.civSettlements(c).sort((a, b) => b.soldiers - a.soldiers)) {
    const k = Math.min(left, st.soldiers);
    const got = s.removePop(st, k);
    for (const r in got) out[r as keyof Pop] = (out[r as keyof Pop] ?? 0) + (got[r as keyof Pop] ?? 0);
    st.soldiers -= k; left -= k;
    if (!left) break;
  }
  return out;
}

export function defenders(s: Sim, st: Settlement, side: 'A' | 'B', vsMonsters: boolean): Combatant[] {
  const c = s.w.civs[st.civ];
  const cs = civTroops(s, c, st.soldiers, side);
  const militia = Math.floor((s.pop(st) - st.soldiers) * (vsMonsters ? 0.5 : 0.35));
  for (let i = 0; i < militia; i++) cs.push(unit(UNITS.militia, side, 'militia', RACES[c.race].hp));
  let ac = (st.civics.palisade ? 1 : 0) + (st.civics.stonewall ? 2 : 0) + (st.civics.castle ? 3 : 0) + s.e(c, 'defAc');
  if (vsMonsters && s.e(c, 'foresee') > 0) ac += 2;
  if (s.e(c, 'shield') > 0 && s.capital(c)?.id === st.id) ac += 4;
  if (c.cls === 'druid' && s.g.neighbors(st.tile).some((n) => ['forest', 'oldforest'].includes(s.w.tiles[n].terrain))) ac += 2;
  for (const x of cs) x.ac += ac;
  for (const h of s.civHeroes(c)) if (h.state === 'home' && h.pos === st.tile) cs.push(heroCombatant(h, side));
  if (s.e(c, 'avatar') > 0) cs.push(unit({ name: 'Savaş Avatarı', hp: 80, ac: 19, atk: 9, dmg: [2, 12, 5], attacks: 2 }, side, 'unique'));
  return cs;
}

function civOpts(s: Sim, c: Civ | undefined, side: 'A' | 'B', o: Partial<BattleOpts>, enemy?: Civ) {
  if (c) {
    if (s.e(c, 'noRout') > 0) (o as Record<string, unknown>)[side === 'A' ? 'noRoutA' : 'noRoutB'] = true;
    if (s.e(c, 'firstStrike') > 0) (o as Record<string, unknown>)[side === 'A' ? 'firstStrikeA' : 'firstStrikeB'] = s.e(c, 'firstStrike');
    if (s.e(c, 'meteor') > 0) (o as Record<string, unknown>)[side === 'A' ? 'meteorA' : 'meteorB'] = true;
  }
  if (enemy && s.e(enemy, 'enemyMorale') > 0) {
    const k = side === 'A' ? 'moraleA' : 'moraleB';
    (o as Record<string, number>)[k] = ((o as Record<string, number>)[k] ?? 0.6) - s.e(enemy, 'enemyMorale');
  }
  return o;
}

export function recordBattle(s: Sim, b: Battle) {
  s.w.battles.push(b);
  if (s.w.battles.length > 250) s.w.battles.shift();
  s.metric('battles');
  return b.id;
}

export function syncHeroes(s: Sim, cs: Combatant[], xpBonus = 0) {
  for (const x of cs) {
    if (!x.hero) continue;
    const h = x.hero;
    h.hp = Math.max(0, Math.min(h.maxHp, x.hp));
    h.kills += x.kills;
    if (x.hp <= 0) {
      if (tryRevive(s, h)) continue;
      h.state = 'dead'; h.deathDay = s.day; h.auction = undefined; h.goal = undefined;
      if (h.contract) h.contract = undefined;
      const lb = s.w.battles[s.w.battles.length - 1];
      if (lb && lb.day === s.day) h.pos = lb.tile;
      s.metric('heroDeath');
      s.log('death', `${s.heroTitle(h)} öldü.`, { tile: h.pos, civ: h.civ >= 0 ? h.civ : undefined, major: true, cause: `${h.kills} düşman devirmişti` });
    } else gainXp(s, h, x.kills * 50 + xpBonus);
  }
}

function applyDefLosses(s: Sim, st: Settlement, cs: Combatant[], cap = 1) {
  const c = s.w.civs[st.civ];
  const deadS = cs.filter((x) => (x.kind === 'soldier' || x.kind === 'unique') && x.hp <= 0).length;
  // sivillerin çoğu kaçıp saklanır: milis kaybı nüfusun belli bir oranıyla sınırlı
  const deadM = Math.min(cs.filter((x) => x.kind === 'militia' && x.hp <= 0).length, Math.max(1, Math.floor(s.pop(st) * cap)));
  const back = Math.floor(deadS * Math.min(0.8, s.e(c, 'healBack')));
  s.removePop(st, deadS - back + deadM);
  st.soldiers = Math.max(0, st.soldiers - deadS + back);
  syncHeroes(s, cs);
}

/** Yılda bir kaybedilen savaşı yeniden atma (Talihin Dokunuşu / Kader Dokuması) */
function withLuck(s: Sim, c: Civ | undefined, fight: () => Battle, lost: (b: Battle) => boolean): Battle {
  let b = fight();
  if (c && lost(b) && s.e(c, 'luck') > 0 && c.yearly.luck !== s.year) {
    c.yearly.luck = s.year;
    const b2 = fight();
    b2.lines.unshift({ t: `Talih ${c.name} tarafına döndü!`, crit: true });
    b = b2;
  }
  return b;
}

// ------------------------------------------------------------ hareket
export function agentsTick(s: Sim) {
  const w = s.w;
  for (const a of w.agents) {
    if (a.dead) continue;
    if (a.step < a.path.length - 1) {
      const next = a.path[a.step + 1];
      let cost = s.moveCost(next);
      if (a.civ >= 0 && s.e(w.civs[a.civ], 'forestMove') > 0 && ['forest', 'oldforest'].includes(w.tiles[next].terrain)) cost = 0.5;
      a.progress += a.speed / cost;
      while (a.progress >= 1 && a.step < a.path.length - 1) {
        a.progress -= 1; a.step++;
        if (a.kind === 'hero' || a.kind === 'party') for (const hid of a.heroes ?? []) s.hero(hid).pos = a.path[a.step];
        if (a.kind === 'scout') scoutSight(s, a);
        if (a.kind === 'army' && (a.purpose === 'war' || a.purpose === 'plunder') && !a.returning) pillageTile(s, a, a.path[a.step]);
      }
    }
    if (a.kind === 'raid' && a.purpose === 'caravan' && !a.returning) {
      const cv = w.agents.find((x) => x.id === a.to && !x.dead);
      if (!cv) { raidReturn(s, a); continue; }
      const here = tileOf(a), cvt = tileOf(cv);
      if (s.g.dist(here, cvt) <= 1) { ambush(s, a, cv); continue; }
      if (s.day % 3 === 0) { const p = s.path(here, cv.path[Math.min(cv.path.length - 1, cv.step + 2)]); if (p) { a.path = p; a.step = 0; a.progress = 0; } }
      if (a.chase === undefined) a.chase = s.day;
      if (s.day - a.chase > 25) raidReturn(s, a);
      continue;
    }
    if (a.step >= a.path.length - 1 && arrive(s, a)) a.dead = true;
  }
  w.agents = w.agents.filter((a) => !a.dead);
}
export function tileOf(a: Agent) { return a.path[Math.min(a.step, a.path.length - 1)]; }

function reroute(s: Sim, a: Agent, to: number) { const p = s.path(tileOf(a), to); a.path = p ?? [tileOf(a)]; a.step = 0; a.progress = 0; }

function scoutSight(s: Sim, a: Agent) {
  const c = s.w.civs[a.civ];
  const here = tileOf(a);
  for (const d of s.w.deposits) {
    if (d.knownBy.includes(c.id) || !d.tiles.some((t) => s.g.dist(t, here) <= 4)) continue;
    d.knownBy.push(c.id);
    if (s.depositVisible(c, d) && ['tin', 'gold', 'silver', 'iron', 'mana', 'mithril', 'horses', 'salt', 'copper'].includes(d.kind))
      s.log('discover', `${c.name} kâşifleri bir ${(d.kind === 'silver' ? 'gümüş damarı' : d.kind === 'horses' ? 'yaban at sürüsü' : TECH_RES[d.kind] ?? d.kind)} buldu.`, { civ: c.id, tile: d.tiles[0], major: ['tin', 'gold', 'iron'].includes(d.kind) });
  }
  for (const cp of s.w.camps) if (cp.alive && s.g.dist(here, cp.tile) <= 4 && !c.yearly['saw' + cp.id]) {
    c.yearly['saw' + cp.id] = 1; c.threat += 0.1;
    s.log('discover', `${c.name} kâşifleri ${ek(cp.name, 'i')} gördü: ${cp.count} ${monsterName(cp.kind, false).toLocaleLowerCase('tr')}.`, { civ: c.id, tile: cp.tile });
  }
  for (const o of s.w.civs) {
    if (o.id === c.id || !o.alive || s.rel(c.id, o.id).contact) continue;
    if (s.civSettlements(o).some((x) => s.g.dist(x.tile, here) <= 5)) makeContact(s, c, o, `${c.name} kâşifleri ${s.capital(o)?.name ?? 'yerleşimlerine'} ulaştı`);
  }
}
const TECH_RES: Record<string, string> = { tin: 'kalay damarı', gold: 'altın damarı', iron: 'demir damarı', mana: 'mana kristali yatağı', mithril: 'mithril damarı', salt: 'tuz yatağı', copper: 'bakır damarı', coal: 'kömür yatağı' };

export function makeContact(s: Sim, a: Civ, b: Civ, why: string) {
  for (const [x, y] of [[a.id, b.id], [b.id, a.id]]) s.rel(x, y).contact = true;
  s.setMod(a.id, b.id, 'first', 'İlk karşılaşma merakı', 10, 0.02);
  s.log('contact', `${a.name} ile ${b.name} ilk kez karşılaştı.`, { civ: a.id, cause: why, major: true });
  s.metric('contact');
}

function arrive(s: Sim, a: Agent): boolean {
  const w = s.w;
  const tile = tileOf(a);
  switch (a.kind) {
    case 'scout': {
      if (!a.returning) { const home = s.capital(w.civs[a.civ]); if (home) { reroute(s, a, home.tile); a.returning = true; return false; } }
      return true;
    }
    case 'settlers': {
      if (a.purpose === 'homeless') {
        const to = s.settlement(a.to!), home = s.settlement(a.from!);
        const dest = to && to.alive ? to : home && home.alive ? home : undefined;
        if (dest) { s.mergePop(dest, a.pop); s.log('migration', `${s.popSize(a.pop)} evsiz ${ek(dest.name, 'da')} yeni evlerine yerleşti.`, { civ: dest.civ, tile: dest.tile }); }
        return true;
      }
      if (a.purpose === 'refugee') {
        const to = s.settlement(a.to!);
        const home = s.settlement(a.from!);
        const dest = to && to.alive ? to : home && home.alive ? home : undefined;
        if (dest) {
          s.mergePop(dest, a.pop);
          if (dest.civ !== a.civ) s.addMod(dest.civ, a.civ, 'refugees', 'Açlara kapı açtık', 4, 12, 0.01, false);
          s.log('migration', `${s.popSize(a.pop)} aç göçmen ${ek(dest.name, 'a')} sığındı.`, { civ: dest.civ, tile: dest.tile });
        }
        return true;
      }
      return foundSettlement(s, a);
    }
    case 'hero': {
      for (const hid of a.heroes ?? []) {
        const h = s.hero(hid); h.pos = tile;
        if (h.state === 'dead') continue;
        if (a.purpose === 'goal') { arriveGoal(s, h); continue; }
        if (a.purpose === 'tavern') {
          const inn = innAt(s, tile);
          const t = inn ? undefined : w.settlements.find((x) => x.alive && x.tile === tile && x.civics.tavern);
          if (inn || t) { h.state = 'tavern'; h.tavern = inn ? inn.id : t!.id; h.idleSince = s.day; } else if (h.civ === -1) returnToBase(s, h); else h.state = 'gone';
        }
        else h.state = 'home';
      }
      return true;
    }
    case 'caravan': return caravanArrive(s, a);
    case 'raid': {
      if (a.returning) { raidHome(s, a); return true; }
      if (a.purpose === 'ext') { raidExt(s, a, a.targetTile ?? tile); return !a.returning; }
      if (a.purpose === 'inn') { innMonsterRaid(s, a); return !a.returning; }
      const st = s.settlement(a.to!);
      if (!st || !st.alive) { raidReturn(s, a); return false; }
      raidSettlement(s, a, st);
      return !a.returning;
    }
    case 'party': {
      if (a.returning) { partyHome(s, a); return true; }
      const cp = w.camps.find((x) => x.id === a.to);
      if (!cp || !cp.alive) { partyReturn(s, a); return false; }
      fightCamp(s, a, cp); return false;
    }
    case 'army': {
      if (a.returning) { armyHome(s, a); return true; }
      if (a.purpose === 'innraid') { innRaidArrive(s, a); return false; }
      if (a.purpose === 'expedition') {
        const cp = w.camps.find((x) => x.id === a.to);
        if (!cp || !cp.alive) { armyReturn(s, a); return false; }
        fightCamp(s, a, cp); return false;
      }
      const st = s.settlement(a.to!);
      if (!st || !st.alive || st.civ === a.civ) { armyReturn(s, a); return false; }
      if (a.purpose === 'plunder') plunder(s, a, st); else siege(s, a, st);
      return false;
    }
  }
  return true;
}

const TOWN_A = ['Kara', 'Ak', 'Yeşil', 'Demir', 'Taş', 'Kurt', 'Göl', 'Çam', 'Kızıl', 'Gök', 'Yel', 'Ay', 'Gün', 'Kuzey', 'Sarı', 'Boz', 'Koca', 'Ulu', 'Yıldız', 'Tuz', 'Bal', 'Söğüt', 'Kartal', 'Ceylan', 'Pınar', 'Meşe', 'Duman', 'Sis', 'Karlı', 'Alaca'];
const TOWN_B = ['köy', 'yurt', 'tepe', 'ova', 'pınar', 'kale', 'dere', 'bük', 'kaya', 'burç', 'geçit', 'yayla', 'çayır', 'koru', 'köprü', 'kent', 'hisar', 'oba', 'yazı', 'sırt'];
/** yeni yerleşim adı: karodan türetilen, tekrarsız Türkçe yer adı */
function townName(s: Sim, tile: number, used: Set<string>) {
  for (let k = 0; k < 60; k++) {
    const a = TOWN_A[(tile * 7 + k * 13) % TOWN_A.length], b = TOWN_B[(tile * 3 + k * 7) % TOWN_B.length];
    const n = a + b;
    if (!used.has(n)) return n;
  }
  return `Yeni ${TOWN_A[tile % TOWN_A.length]}${TOWN_B[s.w.settlements.length % TOWN_B.length]}`;
}
function foundSettlement(s: Sim, a: Agent): boolean {
  const w = s.w;
  const c = w.civs[a.civ];
  const t = a.targetTile!;
  const free = w.tiles[t].owner < 0 && w.tiles[t].camp === undefined && !w.settlements.some((x) => x.alive && s.g.dist(x.tile, t) < 4);
  if (!free || !c.alive) {
    const home = s.settlement(a.from!);
    if (home && home.alive) s.mergePop(home, a.pop);
    s.log('settle', `${c.name} öncüleri hedefledikleri toprağı dolu buldu ve geri döndü.`, { civ: c.id, tile: t });
    return true;
  }
  const used = new Set(w.settlements.map((x) => x.name));
  const name = CLASSES[c.cls].towns.find((n) => !used.has(n)) ?? townName(s, t, used);
  const st = makeSettlement(s.id(), c.id, name, t, a.pop ?? {}, s.day);
  w.settlements.push(st);
  s.updateTerritory();
  s.discover();
  s.metric('settle');
  s.metric(`settle_${c.id}`);
  s.log('settle', `${c.name} yeni yerleşimi ${ek(name, 'i')} kurdu.`, { civ: c.id, tile: t, cause: a.purpose, major: true });
  return true;
}

// ------------------------------------------------------------ kervanlar ve ticaret
export function routesTick(s: Sim) {
  for (const r of s.w.routes) {
    if (!r.alive || s.day < r.nextDepart) continue;
    const A = s.settlement(r.a), B = s.settlement(r.b);
    if (!A || !B || !A.alive || !B.alive) { r.alive = false; continue; }
    const reverse = r.kind === 'trade' && r.trips % 2 === 1;
    const src = reverse ? B : A, dst = reverse ? A : B;
    const cs = s.w.civs[src.civ], cd = s.w.civs[dst.civ];
    if (cs.id !== cd.id && s.atWar(cs.id, cd.id) && !(s.e(cs, 'blackMarket') > 0)) { r.nextDepart = s.day + 30; continue; }
    r.nextDepart = s.day + (r.kind === 'treaty' ? 35 : 40);
    let cargo: Stock = {};
    if (r.kind === 'treaty') {
      const g = r.good!;
      if (s.rel(cs.id, cd.id).treaty !== g && s.rel(cd.id, cs.id).treaty !== g) { r.alive = false; continue; }
      const q = Math.min(10, Math.floor(s.st(cs, g) * 0.4));
      if (q < 2) { r.trips++; continue; }
      s.add(cs, g, -q); cargo = { [g]: q };
    } else {
      let best: Good | null = null, bv = 0;
      for (const g of GOOD_IDS) {
        if (g === 'gold') continue;
        const have = s.st(cs, g);
        if (have < 6) continue;
        const gain = s.price(cd, g) - s.price(cs, g) * 1.15;
        const v = gain * Math.min(12, have * 0.25);
        if (v > bv) { bv = v; best = g; }
      }
      if (!best) { r.trips++; continue; }
      const q = Math.floor(Math.min(12, s.st(cs, best) * 0.25));
      s.add(cs, best, -q); cargo = { [best]: q };
    }
    const guards = s.has(cs, 'training') ? 3 : 2;
    const path = reverse ? r.path.slice().reverse() : r.path;
    const speed = s.st(cs, 'horses') >= 2 ? 0.9 : 0.65;
    s.w.agents.push({ id: s.id(), kind: 'caravan', civ: cs.id, path, step: 0, progress: 0, speed, cargo, troops: guards, from: src.id, to: dst.id, route: r.id });
  }
}

function caravanArrive(s: Sim, a: Agent): boolean {
  const r = s.w.routes.find((x) => x.id === a.route);
  const dst = s.settlement(a.to!), src = s.settlement(a.from!);
  if (!dst || !src || !r) return true;
  const cs = s.w.civs[src.civ], cd = s.w.civs[dst.civ];
  let value = 0;
  for (const g in a.cargo ?? {}) {
    const q = a.cargo![g as Good] ?? 0;
    s.add(cd, g as Good, q);
    value += q * (s.price(cs, g as Good) + s.price(cd, g as Good)) / 2;
  }
  const pay = Math.min(s.st(cd, 'gold'), value);
  s.add(cd, 'gold', -pay);
  s.add(cs, 'gold', pay + 3 * (1 + s.e(cs, 'tradeGold')));
  s.add(cd, 'gold', 2 * (1 + s.e(cd, 'tradeGold')));
  cs.stats.traded++; cd.stats.traded++;
  if (cs.id !== cd.id) s.addMod(cs.id, cd.id, 'trade', 'Süren ticaret', 3, 30, 0.015);
  r.trips++;
  s.metric('caravanTrips');
  if (r.trips === 4) {
    for (const t of r.path) if (!s.w.tiles[t].road) s.w.tiles[t].road = 1;
    s.clearPaths();
    s.log('trade', `${src.name}–${dst.name} kervan yolu çiğnenip gerçek bir yola dönüştü.`, { civ: cs.id, tile: dst.tile });
  }
  return true;
}

export function roadsTick(s: Sim) {
  if (s.day % 4 !== 0) return;
  for (const c of s.w.civs) {
    if (!c.alive || !s.has(c, 'roads')) continue;
    const cap = s.capital(c);
    if (!cap) continue;
    for (const st of s.civSettlements(c)) {
      if (st.id === cap.id) continue;
      const p = s.path(cap.tile, st.tile);
      if (!p) continue;
      const next = p.find((t) => !s.w.tiles[t].road);
      if (next === undefined || s.st(c, 'wood') < 2 || s.st(c, 'stone') < 1) continue;
      s.add(c, 'wood', -1.5); s.add(c, 'stone', -0.8);
      s.w.tiles[next].road = 1;
      s.clearPaths();
      if (!p.some((t) => !s.w.tiles[t].road)) s.log('build', `${cap.name} ile ${st.name} arasındaki yol tamamlandı.`, { civ: c.id, tile: st.tile });
    }
  }
}

// ------------------------------------------------------------ canavar baskınları
export function raidReturn(s: Sim, a: Agent) {
  const cp = s.w.camps.find((x) => x.id === a.from);
  a.returning = true; a.purpose = 'return';
  if (!cp || !cp.alive) { a.path = [tileOf(a)]; a.step = 0; return; }
  reroute(s, a, cp.tile);
}
function raidHome(s: Sim, a: Agent) {
  const cp = s.w.camps.find((x) => x.id === a.from);
  if (cp && cp.alive) { cp.count = Math.min(18, cp.count + (a.troops ?? 0)); if (a.boss) cp.boss = true; cp.loot += a.loot ?? 0; }
}

function raidSettlement(s: Sim, a: Agent, st: Settlement) {
  const c = s.w.civs[st.civ];
  const kind = a.monster ?? 'goblin';
  const cp = s.w.camps.find((x) => x.id === a.from);
  const mons = monsterSide(kind, a.troops ?? 0, !!a.boss, 'B');
  const fight = () => {
    const def = defenders(s, st, 'A', true);
    const mcopy = mons.map((m) => ({ ...m, uses: {}, kills: 0 }));
    const b = resolveBattle(s.rng, def, mcopy, civOpts(s, c, 'A', { id: s.id(), day: s.day, tile: st.tile, title: `${st.name} baskını`, sideA: `${st.name} savunucuları`, sideB: monsterName(kind), moraleA: 0.7, moraleB: kind === 'hobgoblin' ? 0.6 : 0.45 }) as BattleOpts);
    (b as unknown as { _def: Combatant[]; _m: Combatant[] })._def = def;
    (b as unknown as { _def: Combatant[]; _m: Combatant[] })._m = mcopy;
    return b;
  };
  const b = withLuck(s, c, fight, (x) => x.winner === 'B');
  const def = (b as unknown as { _def: Combatant[] })._def, mc = (b as unknown as { _m: Combatant[] })._m;
  recordBattle(s, b);
  applyDefLosses(s, st, def, 0.15);
  a.troops = mc.filter((x) => x.kind === 'monster' && x.hp > 0).length;
  const bossAlive = mc.some((x) => x.boss && x.hp > 0);
  if (a.boss && !bossAlive) { if (cp) cp.hadBoss = false; s.log('raid', `${kind === 'goblin' ? 'Goblin şefi' : 'Hobgoblin yüzbaşısı'} ${ek(st.name, 'da')} öldürüldü!`, { civ: c.id, tile: st.tile, battle: b.id, major: true }); }
  a.boss = bossAlive;
  c.lastRaidedDay = s.day;
  s.metric('raids');
  if (b.winner === 'B') {
    const food = Math.floor(s.st(c, 'grain') * 0.25), gold = Math.floor(s.st(c, 'gold') * 0.3);
    s.add(c, 'grain', -food); s.add(c, 'gold', -gold);
    a.loot = food + gold;
    const burnt = s.rng.int(1, 3);
    st.burnedHouses = (st.burnedHouses ?? 0) + burnt; st.burnedAt = s.day;
    c.threat += 0.45;
    c.stats.battlesLost++;
    if (cp) onHomeBurned(s, st, cp);
    s.log('raid', `${monsterName(kind)} ${ek(st.name, 'i')} yağmaladı: ${b.lossesA} ölü, ${food} tahıl ve ${gold} altın kayıp.`, { civ: c.id, tile: st.tile, battle: b.id, cause: `${cp?.name ?? 'Kamp'} yakınlarda büyüyordu`, major: true });
  } else {
    c.threat += 0.2;
    c.stats.battlesWon++;
    s.log('raid', `${st.name} ${monsterName(kind).toLocaleLowerCase('tr')} baskınını püskürttü (${b.lossesB} düşman öldü, ${b.lossesA} kayıp).`, { civ: c.id, tile: st.tile, battle: b.id, cause: `${cp?.name ?? 'Kamp'} yağma arıyor` });
  }
  if ((a.troops ?? 0) > 0 || bossAlive) raidReturn(s, a); else a.returning = false;
}

/** çıkarma yapısına canavar baskını: işçiler savaşır; kaybederlerse yapı yanar */
function raidExt(s: Sim, a: Agent, tile: number) {
  const w = s.w, t = w.tiles[tile];
  const st = t.owner >= 0 ? s.settlement(t.owner) : undefined;
  if (!t.ext || !s.extWorking(t) || !st || !st.alive) { raidReturn(s, a); return; }
  const c = w.civs[st.civ];
  const kind = a.monster ?? 'goblin';
  const extName = EXTRACTS[t.ext.kind].names[t.ext.level - 1].toLocaleLowerCase('tr');
  const def: Combatant[] = [];
  const workers = Math.max(1, t.ext.workers);
  for (let i = 0; i < workers; i++) def.push(unit(UNITS.militia, 'A', 'militia', RACES[c.race].hp));
  const near = s.g.dist(tile, st.tile) <= 2 ? Math.min(st.soldiers, 3) : 0;
  def.push(...civTroops(s, c, near, 'A'));
  const mons = monsterSide(kind, a.troops ?? 0, !!a.boss, 'B');
  const b = resolveBattle(s.rng, def, mons, civOpts(s, c, 'A', { id: s.id(), day: s.day, tile, title: `${ek(st.name, 'in')} ${extName} baskını`, sideA: `${st.name} işçileri`, sideB: monsterName(kind), moraleA: 0.45, moraleB: 0.5 }) as BattleOpts);
  recordBattle(s, b);
  const deadW = def.filter((x) => x.kind === 'militia' && x.hp <= 0).length;
  const deadS = def.filter((x) => x.kind !== 'militia' && x.hp <= 0).length;
  if (deadW) s.removePop(st, deadW);
  if (deadS) { st.soldiers = Math.max(0, st.soldiers - deadS); s.removePop(st, deadS); }
  a.troops = mons.filter((x) => x.kind === 'monster' && x.hp > 0).length;
  a.boss = mons.some((x) => x.boss && x.hp > 0);
  c.lastRaidedDay = s.day;
  s.metric('raids');
  if (b.winner === 'B') {
    t.ext.burned = s.day + s.rng.int(40, 90); t.ext.burnedAt = s.day; t.ext.workers = 0;
    const g = EXT_GOOD[t.ext.kind];
    const q = g ? Math.floor(Math.min(s.st(c, g) * 0.12, 30)) : 0;
    if (g && q > 0) s.add(c, g, -q);
    a.loot = (a.loot ?? 0) + q;
    c.threat += 0.3;
    c.stats.battlesLost++;
    s.metric('extBurned');
    s.log('raid', `${monsterName(kind, false)} baskını: ${ek(st.name, 'in')} ${extPoss(t.ext.kind, t.ext.level)} yandı${deadW + deadS ? `, ${deadW + deadS} kişi öldü` : ''}${q ? `, ${q} ${GOODS[g!].name.toLocaleLowerCase('tr')} çalındı` : ''}.`, { civ: c.id, tile, battle: b.id, major: true, cause: 'Korumasız işçiler; yeniden kurulana dek üretim durdu' });
  } else {
    c.threat += 0.1;
    c.stats.battlesWon++;
    s.log('raid', `${st.name}: ${extName} işçileri ${monsterName(kind, false).toLocaleLowerCase('tr')} baskınını püskürttü.`, { civ: c.id, tile, battle: b.id, cause: deadW ? `${deadW} işçi hayatını kaybetti` : 'Kayıp yok' });
  }
  raidReturn(s, a);
}
const EXT_GOOD: Partial<Record<string, Good>> = { farm: 'grain', dock: 'fish', hunt: 'meat', lumber: 'wood', quarry: 'stone', mine: 'iron', pasture: 'horses', herbalist: 'herbs', claypit: 'clay', crystal: 'mana' };

/** savaş ve yağma orduları geçtikleri düşman topraklarındaki yapıları yakar */
export function pillageTile(s: Sim, a: Agent, tile: number) {
  const w = s.w, t = w.tiles[tile];
  if (!t.ext || !s.extWorking(t) || t.owner < 0 || a.civ < 0) return;
  const st = s.settlement(t.owner);
  if (!st || st.civ === a.civ || !s.atWar(a.civ, st.civ) && a.purpose !== 'plunder') return;
  if (!s.rng.chance(0.35)) return;
  const extName = EXTRACTS[t.ext.kind].names[t.ext.level - 1].toLocaleLowerCase('tr');
  const dead = Math.min(t.ext.workers, s.rng.int(0, 2));
  if (dead) s.removePop(st, dead);
  t.ext.burned = s.day + s.rng.int(40, 80); t.ext.burnedAt = s.day; t.ext.workers = 0;
  s.metric('extBurned');
  const att = w.civs[a.civ];
  s.log('war', `${att.name} askerleri geçerken ${ek(st.name, 'in')} ${extPoss(t.ext.kind, t.ext.level)} yakıldı${dead ? `, ${dead} kişi öldü` : ''}.`, { civ: att.id, tile, cause: 'Savaşın bedelini köylüler öder' });
}

function ambush(s: Sim, a: Agent, target: Agent) {
  const kind = a.monster ?? 'goblin';
  const c = target.civ >= 0 ? s.w.civs[target.civ] : undefined;
  if (c && target.kind === 'caravan' && s.e(c, 'raidEvade') > 0 && s.rng.chance(s.e(c, 'raidEvade'))) {
    s.log('raid', `${c.name} kervanı gizli yollardan geçerek ${monsterName(kind).toLocaleLowerCase('tr')} pususunu atlattı.`, { civ: c.id, tile: tileOf(target) });
    raidReturn(s, a); return;
  }
  const mons = monsterSide(kind, a.troops ?? 0, !!a.boss, 'B');
  const side: Combatant[] = [];
  if (target.kind === 'caravan' && c) side.push(...civTroops(s, c, target.troops ?? 2, 'A'));
  for (const hid of target.heroes ?? []) { const h = s.hero(hid); if (h.state !== 'dead') side.push(heroCombatant(h, 'A')); }
  if (target.kind === 'scout' && c) side.push(unit(UNITS.militia, 'A', 'militia'));
  if (!side.length) { raidReturn(s, a); return; }
  const tile = tileOf(target);
  const name = target.kind === 'caravan' ? `${c?.name} kervanı` : target.kind === 'scout' ? `${c?.name} kâşifleri` : (target.heroes ?? []).map((h) => s.hero(h).name).join(', ');
  const b = resolveBattle(s.rng, side, mons, civOpts(s, c, 'A', { id: s.id(), day: s.day, tile, title: `${kind === 'bugbear' ? 'Bugbear' : 'Yol'} pususu`, sideA: name, sideB: monsterName(kind), moraleA: 0.5, moraleB: 0.45 }) as BattleOpts);
  recordBattle(s, b);
  syncHeroes(s, side);
  a.troops = mons.filter((x) => x.kind === 'monster' && x.hp > 0).length;
  a.boss = mons.some((x) => x.boss && x.hp > 0);
  s.metric('raids');
  if (c) c.lastRaidedDay = s.day;
  if (b.winner === 'B') {
    target.dead = true;
    if (target.kind === 'caravan') {
      a.loot = Object.values(target.cargo ?? {}).reduce((x, v) => x + (v ?? 0), 0);
      const r = s.w.routes.find((x) => x.id === target.route);
      if (r) r.nextDepart = s.day + 70;
      if (c) c.threat += 0.35;
    }
    for (const hid of target.heroes ?? []) { const h = s.hero(hid); if (h.state !== 'dead') { if (h.civ >= 0) { h.state = 'traveling'; sendHero(s, h, s.capital(s.w.civs[h.civ])?.tile ?? h.pos, 'home'); } else { note(s, h, 'yolda pusuya düştü'); returnToBase(s, h); } } }
    s.log('raid', `${monsterName(kind)} ${ek(name, 'i')} pusuya düşürdü.`, { civ: c?.id, tile, battle: b.id, cause: kind === 'bugbear' ? 'Bugbearlar yolları sessizce avlıyor' : 'Yol kampa yakın geçiyor', major: true });
  } else {
    s.log('raid', `${name} ${monsterName(kind).toLocaleLowerCase('tr')} pususunu savuşturdu.`, { civ: c?.id, tile, battle: b.id });
  }
  raidReturn(s, a);
}

// ------------------------------------------------------------ kahramanlar ve seferler kamplara karşı
export function fightCamp(s: Sim, a: Agent, cp: Camp) {
  const w = s.w;
  const heroes = (a.heroes ?? []).map((id) => s.hero(id)).filter((h) => h.state !== 'dead');
  const civ = a.civ >= 0 ? w.civs[a.civ] : undefined;
  const alone = heroes.length === 1 && !(a.troops ?? 0);
  const side: Combatant[] = heroes.map((h) => heroSide(h, 'A', cp.kind, alone));
  if (civ) side.push(...civTroops(s, civ, a.troops ?? 0, 'A'));
  const hadBoss = cp.boss;
  const mons = monsterSide(cp.kind, cp.count, cp.boss, 'B');
  const who = heroes.length ? heroes.map((h) => h.name).join(', ') : `${civ?.name} askerleri`;
  const b = resolveBattle(s.rng, side, mons, civOpts(s, civ, 'A', { id: s.id(), day: s.day, tile: cp.tile, title: `${cp.name} baskını`, sideA: who, sideB: monsterName(cp.kind), moraleA: 0.55, moraleB: cp.kind === 'hobgoblin' ? 0.65 : 0.5, maxRounds: 20 }) as BattleOpts);
  recordBattle(s, b);
  const q = a.quest ? w.quests.find((x) => x.id === a.quest) : undefined;
  syncHeroes(s, side, b.winner === 'A' ? 100 : 20);
  afterCampFight(s, side, cp, b.winner === 'A', hadBoss, b.rolls);
  const deadS = side.filter((x) => (x.kind === 'soldier' || x.kind === 'unique') && x.hp <= 0).length;
  if (a.troops) { a.troops -= deadS; removeFromPop(s, a.pop, deadS); }
  a.heroes = heroes.filter((h) => h.state !== 'dead').map((h) => h.id);
  if (b.winner === 'A') {
    cp.alive = false; cp.count = 0; cp.clearedDay = s.day;
    delete w.tiles[cp.tile].camp;
    const loot = Math.round(cp.loot);
    s.metric('campCleared');
    const payer = q && q.civ >= 0 ? w.civs[q.civ] : !q ? civ : undefined;
    if (q) {
      q.done = s.day;
      s.metric('questDone');
      if (q.civ < 0) s.metric('innQuestDone');
      const share = Math.floor((q.bounty + loot * 0.5) / Math.max(1, a.heroes.length));
      for (const id of a.heroes) { const h = s.hero(id); h.gold += share; if (q.civ >= 0) h.rep[q.civ] = (h.rep[q.civ] ?? 0) + 1; }
      if (payer) s.add(payer, 'gold', Math.round(loot * 0.5));
      const poster = payer ? payer.name : 'Hancı';
      s.log('lair', `${who}, ${ek(cp.name, 'i')} yerle bir etti! ${poster} ödülü ödedi.`, { tile: cp.tile, civ: payer?.id, battle: b.id, cause: `İlan: ${q.bounty} altın; ${loot} değerinde ganimet`, major: true });
    } else if (civ) {
      s.add(civ, 'gold', loot);
      s.log('lair', `${civ.name} seferi ${ek(cp.name, 'i')} yerle bir etti ve ${loot} değerinde ganimetle döndü!`, { tile: cp.tile, civ: civ.id, battle: b.id, cause: 'Kahramanlar ve askerler omuz omuza', major: true });
    } else {
      const share = Math.floor(loot / Math.max(1, a.heroes.length));
      for (const id of a.heroes) s.hero(id).gold += share;
      s.metric('huntDone');
      s.log('lair', `${who}, kimse istemeden ${ek(cp.name, 'i')} yerle bir etti ve ${loot} değerinde ganimeti paylaştı.`, { tile: cp.tile, battle: b.id, cause: 'Kahramanın kendi yolu', major: true });
    }
    const helped = payer ?? (civ ? civ : undefined);
    if (helped) {
      helped.threat *= 0.4;
      for (const o of w.civs) if (o.alive && o.id !== helped.id && s.rel(o.id, helped.id).contact && s.g.dist(s.capital(o)?.tile ?? 0, cp.tile) <= 20)
        s.addMod(o.id, helped.id, 'camphelp', `${helped.name} bir canavar kampını temizletti`, 10, 20, 0.02);
    } else {
      // ilansız temizlenen kamp: yakındaki herkes rahatlar
      for (const o of w.civs) if (o.alive && s.civSettlements(o).some((x) => s.g.dist(x.tile, cp.tile) <= 18)) o.threat *= 0.6;
    }
    for (const oq of w.quests) if (oq.camp === cp.id && oq.open) {
      oq.open = false;
      if (oq.civ >= 0) s.add(w.civs[oq.civ], 'gold', oq.bounty); else { const i = w.inns.find((x) => x.id === oq.inn); if (i) i.gold += oq.bounty; }
    }
  } else {
    cp.count = mons.filter((x) => x.kind === 'monster' && x.hp > 0).length;
    cp.boss = mons.some((x) => x.boss && x.hp > 0);
    if (q) questFailed(s, q);
    s.log('lair', `${who}, ${ek(cp.name, 'da')} püskürtüldü.`, { tile: cp.tile, civ: a.civ >= 0 ? a.civ : q && q.civ >= 0 ? q.civ : undefined, battle: b.id, cause: `${monsterName(cp.kind)} ${cp.count} kişiyle kampı tuttu${q ? `; ilan ${q.bounty} altına çıktı` : ''}`, major: true });
  }
  if (a.kind === 'army') armyReturn(s, a); else partyReturn(s, a);
}

function removeFromPop(s: Sim, p: Pop | undefined, n: number) {
  if (!p) return;
  for (let i = 0; i < n; i++) {
    const rs = Object.keys(p).filter((r) => (p[r as keyof Pop] ?? 0) > 0) as (keyof Pop)[];
    if (!rs.length) return;
    const r = s.rng.pick(rs); p[r] = (p[r] ?? 1) - 1; if (!p[r]) delete p[r];
  }
}

function partyReturn(s: Sim, a: Agent) {
  const alive = (a.heroes ?? []).map((id) => s.hero(id)).filter((h) => h.state !== 'dead');
  a.returning = true;
  a.path = [tileOf(a)]; a.step = 0; a.progress = 0;
  // her kahraman kendi yuvasına döner
  for (const h of alive) { h.pos = tileOf(a); h.goal = undefined; if (h.civ === -1) returnToBase(s, h); }
  a.heroes = [];
}
function partyHome(_s: Sim, _a: Agent) { /* kahramanlar partyReturn'de dağıldı */ }
export function armyReturn(s: Sim, a: Agent) {
  const c = s.w.civs[a.civ];
  const cap = c && s.capital(c);
  a.returning = true;
  if (cap) reroute(s, a, cap.tile); else a.path = [tileOf(a)];
}
function armyHome(s: Sim, a: Agent) {
  const c = s.w.civs[a.civ];
  const cap = c && s.capital(c);
  for (const id of a.heroes ?? []) { const h = s.hero(id); if (h.state !== 'dead') { h.state = cap ? 'home' : 'gone'; if (cap) h.pos = cap.tile; } }
  if (cap) { s.mergePop(cap, a.pop); cap.soldiers += Math.max(0, a.troops ?? 0); }
}

// ------------------------------------------------------------ savaş, kuşatma, yağma
function siege(s: Sim, a: Agent, st: Settlement) {
  const w = s.w;
  const att = w.civs[a.civ], dfc = w.civs[st.civ];
  const heroes = (a.heroes ?? []).map((id) => s.hero(id)).filter((h) => h.state !== 'dead');
  const isCap = s.capital(dfc)?.id === st.id;
  const fight = () => {
    const side: Combatant[] = [...heroes.map((h) => heroCombatant(h, 'A')), ...civTroops(s, att, a.troops ?? 0, 'A')];
    const def = defenders(s, st, 'B', false);
    if (s.has(att, 'siege')) for (const d of def) d.ac -= 2;
    let o: Partial<BattleOpts> = { id: s.id(), day: s.day, tile: st.tile, title: `${st.name} kuşatması`, sideA: `${att.name} ordusu`, sideB: `${st.name} savunucuları`, moraleA: 0.55, moraleB: 0.65, maxRounds: 20 };
    o = civOpts(s, att, 'A', o, dfc); o = civOpts(s, dfc, 'B', o, att);
    const b = resolveBattle(s.rng, side, def, o as BattleOpts);
    Object.assign(b, { _side: side, _def: def });
    return b;
  };
  let b = withLuck(s, att, fight, (x) => x.winner === 'B');
  if (b.winner === 'A' && dfc.eff.luck && dfc.yearly.luck !== s.year) { dfc.yearly.luck = s.year; b = fight(); b.lines.unshift({ t: `Talih ${dfc.name} tarafına döndü!`, crit: true }); }
  const side = (b as unknown as { _side: Combatant[] })._side, def = (b as unknown as { _def: Combatant[] })._def;
  recordBattle(s, b);
  syncHeroes(s, side, b.winner === 'A' ? 100 : 20);
  applyDefLosses(s, st, def);
  const dead = side.filter((x) => (x.kind === 'soldier' || x.kind === 'unique') && x.hp <= 0).length;
  const back = Math.floor(dead * Math.min(0.8, s.e(att, 'healBack')));
  a.troops = (a.troops ?? 0) - dead + back; removeFromPop(s, a.pop, dead - back);
  a.heroes = heroes.filter((h) => h.state !== 'dead').map((h) => h.id);
  if (b.winner === 'A') { st.burnedHouses = (st.burnedHouses ?? 0) + s.rng.int(2, 4); st.burnedAt = s.day; onHomeBurned(s, st, undefined, att); }
  if (b.winner === 'A') {
    att.stats.battlesWon++; dfc.stats.battlesLost++;
    if (!isCap) {
      st.civ = att.id; st.soldiers = 0;
      s.mergePop(st, a.pop); a.pop = {}; a.troops = 0;
      s.updateTerritory();
      s.metric('conquest');
      s.log('war', `${att.name}, ${ek(st.name, 'i')} fethetti! Halkı (${s.raceStr(st.pop)}) artık onların bayrağı altında.`, { civ: att.id, tile: st.tile, battle: b.id, cause: s.rel(att.id, dfc.id).war?.goal, major: true });
    } else {
      const loot = lootFrom(s, att, dfc, 0.35);
      s.log('war', `${att.name} ordusu ${ek(st.name, 'i')} yağmaladı: ${loot}.`, { civ: att.id, tile: st.tile, battle: b.id, major: true });
    }
  } else {
    att.stats.battlesLost++; dfc.stats.battlesWon++;
    s.log('war', `${st.name}, ${ek(att.name, 'in')} saldırısını püskürttü! Saldıranlar ${dead} kayıp verdi.`, { civ: dfc.id, tile: st.tile, battle: b.id, major: true });
  }
  s.addMod(att.id, dfc.id, 'blood', 'Dökülen kan', -8, -30, 0.02);
  armyReturn(s, a);
}

function plunder(s: Sim, a: Agent, st: Settlement) {
  const w = s.w;
  const att = w.civs[a.civ], dfc = w.civs[st.civ];
  const side = civTroops(s, att, a.troops ?? 0, 'A');
  const def = defenders(s, st, 'B', false);
  let o: Partial<BattleOpts> = { id: s.id(), day: s.day, tile: st.tile, title: `${st.name} yağması`, sideA: `${att.name} akıncıları`, sideB: `${st.name} savunucuları`, moraleA: 0.5, moraleB: 0.6 };
  o = civOpts(s, att, 'A', o, dfc);
  const b = resolveBattle(s.rng, side, def, o as BattleOpts);
  recordBattle(s, b);
  applyDefLosses(s, st, def);
  const dead = side.filter((x) => x.hp <= 0).length;
  a.troops = (a.troops ?? 0) - dead; removeFromPop(s, a.pop, dead);
  s.metric('plunder');
  if (b.winner === 'A') {
    st.burnedHouses = (st.burnedHouses ?? 0) + s.rng.int(1, 3); st.burnedAt = s.day;
    const loot = lootFrom(s, att, dfc, 0.2 * (1 + s.e(att, 'lootMult')));
    let stolen = '';
    if (s.e(att, 'lootTech') > 0 && s.rng.chance(s.e(att, 'lootTech'))) {
      const cand = dfc.research.done.filter((t) => !s.has(att, t) && TECH[t].tree === 'main' && TECH[t].era <= att.era && TECH[t].req.every((r) => s.has(att, r)));
      if (cand.length) { const t = s.rng.pick(cand); att.research.done.push(t); s.recomputeEff(att); stolen = ` ve ${TECH[t].name} bilgisini zorla öğrendi`; s.metric('stolenTech'); }
    }
    s.log('war', `${att.name} akıncıları ${ek(st.name, 'i')} yağmaladı: ${loot}${stolen}.`, { civ: att.id, tile: st.tile, battle: b.id, major: true, cause: `${CLASSES[att.cls].feature}: yağma ekonomisi` });
  } else {
    s.log('war', `${st.name}, ${att.name} akıncılarını geri püskürttü.`, { civ: dfc.id, tile: st.tile, battle: b.id, major: true });
  }
  s.addMod(dfc.id, att.id, 'plunder', 'Yağma baskını', -15, -45, 0.015, false);
  s.addMod(att.id, dfc.id, 'blood', 'Dökülen kan', -4, -20, 0.02, false);
  armyReturn(s, a);
}

function lootFrom(s: Sim, att: Civ, dfc: Civ, frac: number): string {
  const parts: string[] = [];
  const gold = Math.floor(s.st(dfc, 'gold') * frac);
  if (gold > 0) { s.add(dfc, 'gold', -gold); s.add(att, 'gold', gold); parts.push(`${gold} altın`); }
  const goods = GOOD_IDS.filter((g) => g !== 'gold' && s.st(dfc, g) * GOODS[g].base > 20).sort((x, y) => s.st(dfc, y) * GOODS[y].base - s.st(dfc, x) * GOODS[x].base).slice(0, 2);
  for (const g of goods) { const q = Math.floor(s.st(dfc, g) * frac); if (q > 0) { s.add(dfc, g, -q); s.add(att, g, q); parts.push(`${q} ${GOODS[g].name.toLocaleLowerCase('tr')}`); } }
  return parts.join(', ') || 'boş ambarlar';
}
