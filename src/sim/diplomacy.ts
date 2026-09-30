// İlişkiler, kaynak anlaşmazlıkları, antlaşmalar, savaş, ticaret, genişleme, göç.
import type { Sim } from './sim';
import { FOOD_PER_POP, YEAR } from './sim';
import { ek } from './tr';
import { CLASSES, RACES, CLASS_IDS, type RaceId } from '../data/classes';
import { DEPOSITS, GOODS, type Good } from '../data/goods';
import { TECH } from '../data/techs';
import { civTroops, drawSoldiers, makeContact } from './agents';
import { heroCombatant, powerOf, unit } from './combat';
import { regrowForests } from './economy';
import { blockedByGate, chooseResearch } from './research';
import { emptyRel, makeCiv, makeSettlement } from './worldgen';
import type { Civ, Settlement, War } from './types';
import { civPath, hasSea, navPath, ports, freeHulls, freeGalleys, hullName } from './sea';

const GATE_GOOD: Record<string, Good[]> = { copper: ['copper'], tin: ['tin'], iron: ['iron'], coal: ['coal'], gold: ['gold'], mana: ['mana'], mithril: ['mithril'], horses: ['horses'], herbs: ['herbs'], clay: ['clay'] };
const DEP_FOR: Partial<Record<Good, string[]>> = { copper: ['copper'], tin: ['tin'], iron: ['iron'], coal: ['coal'], gold: ['gold', 'silver'], mana: ['mana'], mithril: ['mithril'], horses: ['horses'], herbs: ['herbs'], salt: ['salt'], heartwood: ['heartwood'], clay: ['clay'] };

export function relationsTick(s: Sim) {
  const civs = s.w.civs.filter((c) => c.alive);
  for (const a of civs) for (const b of civs) {
    if (a.id >= b.id) continue;
    const r = s.rel(a.id, b.id);
    if (!r.contact) {
      const close = s.civSettlements(a).some((x) => s.civSettlements(b).some((y) => s.g.dist(x.tile, y.tile) <= 16));
      if (close) makeContact(s, a, b, 'Sınırları birbirine yaklaştı'); else continue;
    }
    for (const [x, y] of [[a.id, b.id], [b.id, a.id]]) {
      const rr = s.rel(x, y);
      for (const m of rr.mods) if (m.decay) m.value = m.value > 0 ? Math.max(0, m.value - m.decay * 10) : Math.min(0, m.value + m.decay * 10);
      rr.mods = rr.mods.filter((m) => m.decay === 0 || Math.abs(m.value) >= 0.5);
    }
    const diff = Math.abs(a.align.law - b.align.law) + Math.abs(a.align.good - b.align.good);
    if (diff > 0.3) s.setMod(a.id, b.id, 'values', 'Farklı değerler', -Math.round(diff * 6)); else s.setMod(a.id, b.id, 'values', 'Benzer değerler', 6);
    const minD = Math.min(...s.civSettlements(a).flatMap((x) => s.civSettlements(b).map((y) => s.g.dist(x.tile, y.tile))));
    if (minD <= 9) s.setMod(a.id, b.id, 'border', 'Sınır sürtüşmesi', -8); else s.removeMod(a.id, b.id, 'border');
    if (s.day - a.lastRaidedDay < 240 && s.day - b.lastRaidedDay < 240) s.setMod(a.id, b.id, 'enemy', 'Ortak düşman: canavarlar', 10); else s.removeMod(a.id, b.id, 'enemy');
    // sınıf yakınlıkları
    const pair = [a, b];
    const pal = pair.find((c) => c.cls === 'paladin' && c.subclass === 'ancients');
    const dru = pair.find((c) => c.cls === 'druid');
    if (pal && dru) s.setMod(a.id, b.id, 'ancients', 'Kadim Yemin dostluğu', 18);
    const paladin = pair.find((c) => c.cls === 'paladin');
    const other = pair.find((c) => c !== paladin);
    if (paladin && other && other.align.good < -0.3) s.setMod(a.id, b.id, 'evil', 'Paladin kötülüğe tahammül etmez', -20);
    const priest = pair.find((c) => c.cls === 'cleric');
    const dark = pair.find((c) => c !== priest);
    if (priest && dark && dark.align.good < -0.3) s.setMod(a.id, b.id, 'evil2', 'Tapınak karanlıkla uzlaşmaz', -12);
    for (const [x, y] of [[a, b], [b, a]]) {
      const ch = s.e(x, 'charm');
      if (ch > 0) s.setMod(y.id, x.id, 'charm', 'Ozanların şarkıları', Math.round(6 * ch));
      // toprak açlığı: kalabalık, saldırgan ve sınırdaş olan komşusunun toprağına göz diker
      const rr = s.rel(x.id, y.id);
      const agg = CLASSES[x.cls].aggression;
      const crowded = s.civPop(x) >= 26 && (s.civSettlements(x).length >= 4 || s.day - x.lastExpand > 400);
      if (minD <= 10 && agg >= 0.2 && crowded && !rr.treaty) rr.land = Math.min(30, (rr.land ?? 0) + 0.25 + agg * 0.9);
      else rr.land = Math.max(0, (rr.land ?? 0) - 0.4);
      if ((rr.land ?? 0) >= 8) s.setMod(x.id, y.id, 'land', 'Toprak hırsı', -Math.min(25, Math.round(rr.land! * 1.2)), 0, false);
      else s.removeMod(x.id, y.id, 'land', false);
    }
    disputes(s, a, b);
    disputes(s, b, a);
    warPeace(s, a, b);
    warPeace(s, b, a);
  }
}

/** o medeniyetinin, h'nin elindeki bir kaynağa duyduğu ihtiyaç */
function disputes(s: Sim, o: Civ, h: Civ) {
  const r = s.rel(o.id, h.id);
  const want = new Map<Good, number>();
  for (const g of CLASSES[o.cls].desires) want.set(g, 1.2);
  for (const t of blockedByGate(s, o)) for (const k of [...(t.gate ?? []), ...(t.gateAny ?? [])]) for (const g of GATE_GOOD[k] ?? []) if (!s.access(o, k)) want.set(g, Math.max(want.get(g) ?? 0, 1));
  let worst: [Good, number] | null = null;
  for (const [g, w] of want) {
    const kinds = DEP_FOR[g] ?? [];
    const lacks = !kinds.some((k) => s.ownsDeposit(o, k));
    const held = lacks && s.w.deposits.some((d) => kinds.includes(d.kind) && !d.depleted && d.knownBy.includes(o.id) && d.tiles.some((t) => s.tileCiv(t) === h.id));
    const cur = r.tension[g] ?? 0;
    if (!held || r.treaty === g) { if (cur > 0) r.tension[g] = Math.max(0, cur - 0.6); continue; }
    const nv = cur + w * (o.race === 'dwarf' ? 1.3 : 1);
    r.tension[g] = nv;
    if (cur < 6 && nv >= 6) {
      s.metric('tension');
      s.log('tension', `${o.name}, ${ek(h.name, 'in')} elindeki ${GOODS[g].name.toLocaleLowerCase('tr')} kaynağına göz dikti.`, { civ: o.id, cause: `${CLASSES[o.cls].desires.includes(g) ? `${CLASSES[o.cls].name} medeniyeti ${GOODS[g].name.toLocaleLowerCase('tr')} ister` : 'Araştırması bu kaynağa takıldı'}; kendi yatağı yok`, major: true });
    }
    if (!worst || nv > worst[1]) worst = [g, nv];
  }
  if (worst) s.setMod(o.id, h.id, 'dispute', `${GOODS[worst[0]].name} anlaşmazlığı`, -Math.min(40, Math.round(worst[1] * 1.6)), 0, false);
  else s.removeMod(o.id, h.id, 'dispute', false);
  // müzakere
  if (!worst || r.war) return;
  const [g, tension] = worst;
  const talkAt = o.align.law > 0.3 ? 10 : 16;
  if (tension < talkAt || s.day - r.lastTalk < 120 || s.relValue(o.id, h.id) < -65) return;
  r.lastTalk = s.day;
  const p = 0.35 + h.align.good * 0.3 + h.align.law * 0.1 + s.relValue(h.id, o.id) / 150 + (h.cls === 'rogue' ? 0.15 : 0);
  if (s.rng.chance(p)) {
    s.rel(o.id, h.id).treaty = g; s.rel(h.id, o.id).treaty = g;
    r.tension[g] = 0;
    s.removeMod(o.id, h.id, 'dispute', false);
    s.setMod(o.id, h.id, 'treaty', `${GOODS[g].name} antlaşması`, 15);
    const kinds = DEP_FOR[g] ?? [];
    const srcTile = s.w.deposits.find((d) => kinds.includes(d.kind) && d.tiles.some((t) => s.tileCiv(t) === h.id))?.tiles.find((t) => s.tileCiv(t) === h.id);
    const src = srcTile !== undefined ? s.settlement(s.w.tiles[srcTile].owner) : s.capital(h);
    const dst = s.capital(o);
    const path = src && dst && s.path(src.tile, dst.tile);
    if (src && dst && path) s.w.routes.push({ id: s.id(), a: src.id, b: dst.id, kind: 'treaty', good: g, path, nextDepart: s.day + 3, trips: 0, alive: true, since: s.day });
    s.metric('treaty');
    s.log('diplomacy', `${o.name} ile ${h.name} ${GOODS[g].name} Antlaşması imzaladı: ${GOODS[g].name.toLocaleLowerCase('tr')} altın karşılığında düzenli taşınacak.`, { civ: o.id, cause: `Kavga yerine pazarlık (${h.name} teklife açıktı)`, major: true });
  } else {
    s.addMod(o.id, h.id, 'refused', 'Reddedilen teklif', -12, -24, 0.03, false);
    s.log('diplomacy', `${h.name}, ${ek(o.name, 'in')} ${GOODS[g].name.toLocaleLowerCase('tr')} paylaşımı teklifini reddetti.`, { civ: o.id, cause: 'Kaynağı paylaşmak istemiyorlar' });
  }
}

export function militaryPower(s: Sim, c: Civ) {
  const sol = s.civSettlements(c).reduce((a, x) => a + x.soldiers, 0);
  const cs = [...civTroops(s, c, sol, 'A'), ...s.civHeroes(c).map((h) => heroCombatant(h, 'A'))];
  const militia = Math.floor(s.civPop(c) * 0.2);
  for (let i = 0; i < militia; i++) cs.push(unit({ name: 'Milis', hp: 6, ac: 10, atk: 1, dmg: [1, 4, 0] }, 'A', 'militia'));
  return powerOf(cs);
}

function warThreshold(s: Sim, c: Civ) {
  let t = -32 - c.align.good * 22 - c.align.law * 10 + CLASSES[c.cls].aggression * 28;
  if (c.subclass === 'vengeance') t += 15;
  return t;
}

function warPeace(s: Sim, o: Civ, t: Civ) {
  const r = s.rel(o.id, t.id);
  const rv = s.relValue(o.id, t.id);
  if (!r.war) {
    if (s.rel(t.id, o.id).war || s.inWar(o)) return;
    if (s.day - (r.peaceDay ?? -9999) < 2 * YEAR || s.day - (o.lastWarEnd ?? -9999) < YEAR * 1.5) return;
    const tension = Math.max(0, ...Object.values(r.tension).map((v) => v ?? 0));
    const land = r.land ?? 0;
    const crusade = s.e(o, 'crusade') > 0 && t.align.good < -0.3 && o.align.good > 0.3;
    if (!crusade && (rv > warThreshold(s, o) || (tension < 10 && land < 12))) return;
    if (crusade && rv > 0) return;
    if (o.cls === 'paladin' && (r.treaty || s.rel(t.id, o.id).treaty)) return; // yemin: antlaşma bozulmaz
    if (militaryPower(s, o) < militaryPower(s, t) * 0.75) return;
    if (s.civPop(o) < 22 || s.civSettlements(o).reduce((n, x) => n + x.soldiers, 0) < 4) return;
    // hedef: çekişilen kaynağa ya da (toprak savaşında) sınırdaki en yakın yerleşim
    const byLand = tension < 10 && !crusade;
    const goodKey = (Object.keys(r.tension) as Good[]).sort((x, y) => (r.tension[y] ?? 0) - (r.tension[x] ?? 0))[0];
    const kinds = byLand ? [] : DEP_FOR[goodKey] ?? [];
    const depTile = s.w.deposits.find((d) => kinds.includes(d.kind) && d.tiles.some((x) => s.tileCiv(x) === t.id))?.tiles.find((x) => s.tileCiv(x) === t.id);
    const oCap = s.capital(o);
    const border = oCap ? s.civSettlements(t).slice().sort((x, y) => s.g.dist(x.tile, oCap.tile) - s.g.dist(y.tile, oCap.tile))[0] : undefined;
    const target = crusade ? s.capital(t) : depTile !== undefined ? s.settlement(s.w.tiles[depTile].owner) : byLand ? border : s.capital(t);
    if (!target) return;
    const war = { since: s.day, attacker: o.id, target: target.id, attacks: 0, lastArmy: -999, goal: crusade ? `Kutsal Sefer: ${ek(t.name, 'in')} karanlık paktını yıkmak` : byLand ? `${ek(target.name, 'i')} ve çevresindeki toprakları almak` : `${GOODS[goodKey]?.name ?? 'Toprak'} kaynağını ele geçirmek` };
    r.war = war; s.rel(t.id, o.id).war = war;
    s.metric('war');
    s.log('war', `${o.name}, ${ek(t.name, 'a')} SAVAŞ İLAN ETTİ! Hedef: ${target.name}.`, { civ: o.id, tile: target.tile, cause: `İlişki ${rv}: ${r.mods.filter((m) => m.value < 0).map((m) => m.text.toLocaleLowerCase('tr')).join(', ')}`, major: true });
    callAllies(s, o, t, war, target);
    // Paladin kutsal seferi: iyi medeniyetler kötüye karşı yakınlaşır
    return;
  }
  if (r.war.attacker !== o.id) return;
  const tgt = s.settlement(r.war.target);
  const won = !tgt || !tgt.alive || tgt.civ !== t.id;   // hedef artık düşmanın değil (biz ya da müttefik aldı)
  const long = s.day - r.war.since > 300, tired = r.war.attacks >= 3;
  if (won || long || tired) {
    const why = won ? 'Savaş hedefi ele geçirildi' : tired ? 'Ordular yıprandı' : 'Savaş uzadı, halk yoruldu';
    r.war = null; s.rel(t.id, o.id).war = null;
    r.peaceDay = s.day; s.rel(t.id, o.id).peaceDay = s.day; o.lastWarEnd = s.day;
    for (const k in r.tension) r.tension[k as Good] = (r.tension[k as Good] ?? 0) * (won ? 0 : 0.3);
    r.land = (r.land ?? 0) * (won ? 0 : 0.2);
    s.addMod(o.id, t.id, 'pastwar', 'Geçmiş savaşın izleri', -30, -40, 0.02);
    s.setMod(o.id, t.id, 'peace', 'Barış antlaşması', 20, 0.03);
    s.log('war', `${o.name} ile ${t.name} barış yaptı.`, { civ: o.id, cause: why, major: true });
  }
}

export function considerWarAction(s: Sim, c: Civ) {
  for (const o of s.w.civs) {
    if (o.id === c.id) continue;
    const war = s.rel(c.id, o.id).war;
    if (!war || war.attacker !== c.id) continue;
    if (s.w.agents.some((a) => a.kind === 'army' && a.civ === c.id && a.purpose === 'war')) continue;
    if (s.day - war.lastArmy < 50) continue;
    const tgt = s.settlement(war.target);
    if (!tgt || !tgt.alive || tgt.civ !== o.id) continue;
    const sol = Math.floor(s.civSettlements(c).reduce((a, x) => a + x.soldiers, 0) * 0.75);
    const heroes = s.civHeroes(c).filter((h) => h.state === 'home' && h.hp > h.maxHp * 0.6);
    if (sol + heroes.length * 3 < 4) continue;
    const cap = s.capital(c)!;
    const route = armyRoute(s, c, cap.tile, tgt.tile);
    if (!route) continue;
    const pop = drawSoldiers(s, c, sol);
    for (const h of heroes) h.state = 'army';
    war.lastArmy = s.day; war.attacks++;
    const gal = route.hull ? Math.min(3, freeGalleys(s, route.hull)) : 0;
    s.w.agents.push({ id: s.id(), kind: 'army', civ: c.id, path: route.path, step: 0, progress: 0, speed: 0.55, heroes: heroes.map((h) => h.id), troops: sol, pop, from: cap.id, to: tgt.id, purpose: 'war', hull: route.hull?.id, galleys: gal || undefined });
    if (route.hull) {
      s.metric('seaInvasion');
      s.log('sea', `${c.name} donanması${gal ? ` (${gal} kadırga)` : ''} ${sol} askerle ${ek(route.hull.name, 'dan')} denize açıldı. Hedef: ${tgt.name}.`, { civ: c.id, tile: route.hull.port, cause: war.goal, major: true });
    } else s.log('war', `${c.name} ordusu (${sol} asker${heroes.length ? ', ' + heroes.map((h) => h.name).join(', ') : ''}) ${tgt.name} üzerine yürüyor.`, { civ: c.id, tile: cap.tile, cause: war.goal });
  }
}

/** Ortak düşman: saldırganla arası iyi, hedefe kin duyan bir komşu savaşa katılır ve aynı şehri hedefler */
function callAllies(s: Sim, o: Civ, t: Civ, war: War, target: Settlement) {
  const cands = s.w.civs.filter((y) => y.alive && y.id !== o.id && y.id !== t.id && !s.inWar(y)
    && s.rel(y.id, t.id).contact && s.rel(y.id, o.id).contact && s.day - (y.lastWarEnd ?? -9999) >= YEAR);
  const scored: [Civ, number, number][] = [];
  for (const y of cands) {
    const hate = s.relValue(y.id, t.id), love = s.relValue(y.id, o.id);
    if (hate > warThreshold(s, y) + 18 || hate > -8 || love < 12) continue;
    if (y.cls === 'paladin' && (s.rel(y.id, t.id).treaty || s.rel(t.id, y.id).treaty)) continue;
    if (s.civSettlements(y).reduce((n, x) => n + x.soldiers, 0) < 4) continue;
    const cap = s.capital(y);
    if (!cap || s.g.dist(cap.tile, target.tile) > 40 || !armyRoute(s, y, cap.tile, target.tile)) continue;
    scored.push([y, hate, love]);
  }
  scored.sort((a, b) => (b[2] - b[1]) - (a[2] - a[1]));
  const pick = scored[0];
  if (!pick) return;
  const [y, hate, love] = pick;
  const w2: War = { since: s.day, attacker: y.id, target: target.id, attacks: 0, lastArmy: -999, goal: `${ek(o.name, 'in')} yanında: ${war.goal}`, ally: o.id };
  s.rel(y.id, t.id).war = w2; s.rel(t.id, y.id).war = w2;
  s.setMod(y.id, o.id, 'brothers', 'Silah arkadaşlığı', 14, 0.01);
  s.metric('coalition');
  s.log('war', `${y.name}, ${ek(o.name, 'in')} yanında ${ek(t.name, 'a')} savaş ilan etti! Ortak hedef: ${target.name}.`, { civ: y.id, tile: target.tile, cause: `${t.name} ile ilişki ${hate}, ${o.name} ile ${love}`, major: true });
}

/** ordu yolu: karadan; Donanma varsa (ve daha kısaysa) denizden */
function armyRoute(s: Sim, c: Civ, from: number, to: number): { path: number[]; hull?: Settlement } | null {
  const land = s.path(from, to);
  if (!s.has(c, 'navy')) return land ? { path: land } : null;
  const r = civPath(s, c, from, to);
  if (r && r.hull && hasSea(s, r.path) && (!land || r.path.length < land.length * 0.8)) return r;
  return land ? { path: land } : null;
}

/** Saldırgan sınıfların savaş ilan etmeden yaptığı yağma akınları */
export function considerRaid(s: Sim, c: Civ) {
  if (CLASSES[c.cls].aggression < 0.6 || s.inWar(c)) return;
  const sol = s.civSettlements(c).reduce((a, x) => a + x.soldiers, 0);
  if (sol < 5 || s.w.agents.some((a) => a.civ === c.id && a.purpose === 'plunder')) return;
  const cap = s.capital(c)!;
  const reach = s.has(c, 'navy') ? 34 : 20;
  const targets = s.w.settlements.filter((x) => x.alive && x.civ !== c.id && s.rel(c.id, x.civ).contact && s.relValue(c.id, x.civ) < 0
    && s.day - s.rel(c.id, x.civ).lastRaid > 180 && s.g.dist(x.tile, cap.tile) <= reach && !(s.rel(c.id, x.civ).treaty));
  if (!targets.length) return;
  const st = targets.sort((a, b) => (a.soldiers * 3 + s.pop(a) * 0.4) - (b.soldiers * 3 + s.pop(b) * 0.4))[0];
  const n = Math.floor(sol * 0.5);
  if (n * 3 < st.soldiers * 3 + s.pop(st) * 0.3) return;
  const route = armyRoute(s, c, cap.tile, st.tile);
  if (!route) return;
  s.rel(c.id, st.civ).lastRaid = s.day;
  const pop = drawSoldiers(s, c, n);
  const gal = route.hull ? Math.min(2, freeGalleys(s, route.hull)) : 0;
  s.w.agents.push({ id: s.id(), kind: 'army', civ: c.id, path: route.path, step: 0, progress: 0, speed: 0.8, troops: n, pop, from: cap.id, to: st.id, purpose: 'plunder', hull: route.hull?.id, galleys: gal || undefined });
  if (route.hull) {
    s.metric('seaRaid');
    s.log('sea', `${c.name} deniz akıncıları ${ek(route.hull.name, 'dan')} ${ek(st.name, 'a')} doğru yelken açtı.`, { civ: c.id, tile: route.hull.port, cause: `${CLASSES[c.cls].feature}: ganimet ve şan`, major: true });
  } else s.log('war', `${c.name} akıncıları ${ek(st.name, 'a')} doğru yola çıktı.`, { civ: c.id, tile: cap.tile, cause: `${CLASSES[c.cls].feature}: ganimet ve şan`, major: true });
}

export function considerExpansion(s: Sim, c: Civ) {
  const ss = s.civSettlements(c);
  const cap = s.capital(c);
  if (!cap || !s.has(c, 'roads')) return;
  // anakarada 5 yerleşim; Gemicilik ve Seyir birer denizaşırı koloni hakkı daha açar
  const over = ss.filter((x) => x.overseas).length;
  const landOk = ss.length - over < 5;
  const seaOk = s.has(c, 'shipbuilding') && over < (s.has(c, 'navigation') ? 2 : 1) && ports(s, c).some((x) => freeHulls(s, x) > 0);
  if (!landOk && !seaOk) return;
  if (s.pop(cap) < 12 + ss.length * 5 || s.foodTotal(c) < 30 || s.day - c.lastExpand < 160) return;
  if (s.w.agents.some((a) => a.kind === 'settlers' && a.civ === c.id)) return;
  const target = pickSettleTarget(s, c, landOk, seaOk, cap);
  if (!target) return;
  const pop = s.removePop(cap, 5);
  s.add(c, 'grain', -15); s.add(c, 'wood', -10);
  c.lastExpand = s.day;
  s.w.agents.push({ id: s.id(), kind: 'settlers', civ: c.id, path: target.path, step: 0, progress: 0, speed: 0.5, pop, from: cap.id, targetTile: target.tile, purpose: target.why, hull: target.hull?.id });
  if (target.hull) {
    s.metric('seaVoyage');
    s.log('sea', `${c.name} 5 öncüyü ${ek(target.hull.name, 'dan')} bir ${hullName(s, c)} ile denizaşırı topraklara gönderdi.`, { civ: c.id, tile: target.hull.port, cause: target.why, major: true });
  } else s.log('settle', `${c.name} 5 öncüyü yeni bir yerleşim kurmaya gönderdi.`, { civ: c.id, tile: cap.tile, cause: target.why });
}

function pickSettleTarget(s: Sim, c: Civ, landOk: boolean, seaOk: boolean, cap: Settlement): { tile: number; why: string; path: number[]; hull?: Settlement } | null {
  const w = s.w;
  const own = s.civSettlements(c);
  const all = w.settlements.filter((x) => x.alive);
  const like = CLASSES[c.cls].terrainLike;
  const desires = CLASSES[c.cls].desires;
  const needGoods = new Set<string>();
  for (const t of blockedByGate(s, c)) for (const k of t.gate ?? []) needGoods.add(k);
  const seaMax = s.has(c, 'navigation') ? 45 : 26;
  const cands: { i: number; sc: number; why: string; sea: boolean }[] = [];
  let best = -1, bs = -Infinity, bwhy = '';
  for (let i = 0; i < w.tiles.length; i++) {
    const t = w.tiles[i];
    if (t.terrain === 'water' || t.terrain === 'mountain' || t.sea || t.owner >= 0 || t.camp !== undefined || t.deposit >= 0) continue;
    if (t.isle && !seaOk) continue;
    if (w.inns.some((inn) => s.g.dist(inn.tile, i) < 4)) continue;
    const dOwn = Math.min(...own.map((x) => s.g.dist(x.tile, i)));
    if (dOwn < 5) continue;
    // denizaşırı aday yalnız adalar: anakaranın ıssız kıyıları canavarlara ve yeni kabilelere kalır
    const coastal = s.g.neighbors(i).some((n) => w.tiles[n].sea);
    const sea = !!t.isle;
    if (sea ? !seaOk || dOwn > seaMax : !landOk || dOwn > 16) continue;
    if (all.some((x) => s.g.dist(x.tile, i) < 5)) continue;
    if (w.camps.some((cp) => cp.alive && s.g.dist(cp.tile, i) < 5)) continue;
    let sc = -dOwn * (sea ? 0.3 : 0.7) + s.rng.next() * 2 + (sea ? 3 : 0);
    let why = sea ? 'Bakir bir ada' : 'Verimli topraklar', whyV = 0;
    // liman kurulabilecek kıyı yeri, denizci medeniyetler için değerli
    if (coastal && s.has(c, 'fishing')) sc += 1.5 * (CLASSES[c.cls].prefer.deniz ?? 1);
    const seenDep = new Set<number>();
    for (const n of s.g.within(i, 2)) {
      const tt = w.tiles[n];
      sc += (like[tt.terrain] ?? 0.4) * 0.6;
      if (tt.deposit < 0 || seenDep.has(tt.deposit) || tt.owner >= 0) continue;
      const d = w.deposits.find((x) => x.id === tt.deposit)!;
      seenDep.add(d.id);
      if (!s.depositVisible(c, d) || d.depleted || (d.kind === 'heartwood' && c.cls !== 'druid')) continue;
      const g = DEPOSITS[d.kind].good;
      let v = GOODS[g].base * 1.5;
      if (desires.includes(g)) v += 18;
      if (needGoods.has(d.kind) || (d.kind === 'silver' && needGoods.has('gold'))) v += 22;
      if (!s.ownsDeposit(c, d.kind)) v *= 1.5;
      sc += v;
      if (v > whyV) { whyV = v; why = `${sea ? 'Ada: ' : ''}${DEPOSITS[d.kind].name} için`; }
    }
    if (sea) cands.push({ i, sc, why, sea });
    else if (sc > bs) { bs = sc; best = i; bwhy = why; }
  }
  if (best >= 0) cands.push({ i: best, sc: bs, why: bwhy, sea: false });
  cands.sort((a, b) => b.sc - a.sc);
  for (const cd of cands.slice(0, 6)) {
    if (!cd.sea) { const p = s.path(cap.tile, cd.i); if (p) return { tile: cd.i, why: cd.why, path: p }; continue; }
    const r = civPath(s, c, cap.tile, cd.i);
    if (r && r.hull && hasSea(s, r.path)) return { tile: cd.i, why: cd.why, path: r.path, hull: r.hull };
  }
  return null;
}

export function considerTrade(s: Sim, c: Civ) {
  if (!s.has(c, 'barter')) return;
  const src = s.civSettlements(c).find((x) => x.civics.market) ?? null;
  if (!src) return;
  const maxD = s.has(c, 'caravans') ? 40 : 22;
  for (const o of s.w.civs) {
    if (o.id === c.id || !o.alive) continue;
    const r = s.rel(c.id, o.id);
    if (!r.contact || (r.war && !s.e(c, 'blackMarket')) || s.relValue(c.id, o.id) < -5) continue;
    if (s.w.routes.some((rt) => rt.alive && rt.kind === 'trade' && [rt.a, rt.b].some((x) => s.settlement(x)?.civ === c.id) && [rt.a, rt.b].some((x) => s.settlement(x)?.civ === o.id))) continue;
    const theirs = s.civSettlements(o).sort((a, b) => s.g.dist(a.tile, src.tile) - s.g.dist(b.tile, src.tile))[0];
    if (!theirs || s.g.dist(theirs.tile, src.tile) > maxD) continue;
    const path = s.path(src.tile, theirs.tile);
    if (!path) continue;
    s.w.routes.push({ id: s.id(), a: src.id, b: theirs.id, kind: 'trade', path, nextDepart: s.day + 5, trips: 0, alive: true, since: s.day });
    s.metric('tradeRoute');
    s.log('trade', `${src.name} ile ${theirs.name} arasında ticaret yolu açıldı.`, { civ: c.id, tile: src.tile, cause: `Pazar kuruldu; ${o.name} ile ilişki ${s.relValue(c.id, o.id)}`, major: true });
  }
  considerSeaTrade(s, c);
}

/** deniz ticaret yolu: kara yoluyla ulaşılamayan (ya da çok uzak) limanlar arasında */
function considerSeaTrade(s: Sim, c: Civ) {
  if (!s.has(c, 'boatbuilding')) return;
  const mine = ports(s, c).filter((x) => (x.ships ?? 0) > 0);
  if (!mine.length) return;
  const maxD = s.has(c, 'seatrade') ? 80 : s.has(c, 'navigation') ? 60 : s.has(c, 'shipbuilding') ? 42 : 24;
  const alive = s.w.routes.filter((rt) => rt.alive && rt.kind === 'trade');
  for (const o of s.w.civs) {
    if (o.id === c.id || !o.alive) continue;
    const r = s.rel(c.id, o.id);
    if (!r.contact || r.war || s.relValue(c.id, o.id) < -5 || s.day - (r.seaTry ?? -9999) < 90) continue;
    const between = alive.filter((rt) => [rt.a, rt.b].some((x) => s.settlement(x)?.civ === c.id) && [rt.a, rt.b].some((x) => s.settlement(x)?.civ === o.id));
    if (between.some((rt) => rt.sea) || between.length >= 1 && !s.has(c, 'shipbuilding')) continue;
    r.seaTry = s.day; s.rel(o.id, c.id).seaTry = s.day;
    const theirs = ports(s, o);
    let pa: Settlement | undefined, pb: Settlement | undefined, bd = Infinity;
    for (const a of mine) for (const b of theirs) { const d = s.g.dist(a.port!, b.port!); if (d < bd) { bd = d; pa = a; pb = b; } }
    if (!pa || !pb || bd > maxD || bd < 6) continue;
    // kara yolu kısaysa deniz yoluna gerek yok
    const land = s.path(pa.tile, pb.tile);
    if (land && land.length <= bd * 1.25 && between.length) continue;
    const path = navPath(s, pa.tile, pb.tile, { embark: [pa.port!], open: s.has(c, 'navigation'), landOnly: [pb.port!] });
    if (!path || !hasSea(s, path)) continue;
    s.w.routes.push({ id: s.id(), a: pa.id, b: pb.id, kind: 'trade', path, nextDepart: s.day + 5, trips: 0, alive: true, since: s.day, sea: true });
    s.metric('seaRoute');
    s.log('sea', `${pa.name} ile ${pb.name} arasında deniz ticaret yolu açıldı.`, { civ: c.id, tile: pa.port, cause: `${bd} karo deniz; ${o.name} ile ilişki ${s.relValue(c.id, o.id)}`, major: true });
  }
  // denizaşırı kolonilere ikmal gemileri
  if (!s.has(c, 'shipbuilding')) return;
  for (const col of s.civSettlements(c)) {
    if (!col.overseas || alive.some((rt) => rt.sea && (rt.a === col.id || rt.b === col.id))) continue;
    if (c.yearly['supply' + col.id] === s.year) continue;
    c.yearly['supply' + col.id] = s.year;
    const home = mine.filter((x) => x.id !== col.id && !x.overseas).sort((a, b) => s.g.dist(a.port!, col.tile) - s.g.dist(b.port!, col.tile))[0];
    if (!home) continue;
    const land = col.civics.shipyard && col.port !== undefined ? [col.port] : undefined;
    const path = navPath(s, home.tile, col.tile, { embark: [home.port!], open: s.has(c, 'navigation'), landOnly: land });
    if (!path || !hasSea(s, path)) continue;
    s.w.routes.push({ id: s.id(), a: home.id, b: col.id, kind: 'trade', path, nextDepart: s.day + 5, trips: 0, alive: true, since: s.day, sea: true });
    s.metric('seaRoute');
    s.log('sea', `${home.name} ile denizaşırı ${col.name} arasında ikmal gemileri işlemeye başladı.`, { civ: c.id, tile: home.port, major: false });
  }
}

export function considerScout(s: Sim, c: Civ) {
  if (c.scoutSent || s.day < 60 + c.id * 20) return;
  const cap = s.capital(c);
  if (!cap) return;
  c.scoutSent = true;
  const W = s.w.W, H = s.w.H;
  const cx = s.g.col(cap.tile), cy = s.g.row(cap.tile);
  const tx = cx < W / 2 ? Math.min(W - 3, cx + 26) : Math.max(2, cx - 26);
  const ty = cy < H / 2 ? Math.min(H - 3, cy + 14) : Math.max(2, cy - 14);
  // hedef noktaya en yakın anakara karosu
  const want = s.g.idx(tx, ty);
  let goal = -1, gd = Infinity;
  for (const i of s.g.within(want, 12)) { const t = s.w.tiles[i]; if (t.sea || t.isle || t.terrain === 'water' || t.terrain === 'mountain') continue; const d = s.g.dist(i, want); if (d < gd) { gd = d; goal = i; } }
  if (goal < 0) return;
  const p = s.path(cap.tile, goal);
  if (!p) return;
  s.w.agents.push({ id: s.id(), kind: 'scout', civ: c.id, path: p, step: 0, progress: 0, speed: 1.1, purpose: 'scout' });
  s.log('discover', `${c.name} ufku keşfetmek için kâşifler gönderdi.`, { civ: c.id, tile: cap.tile });
}

export function worldTick(s: Sim) {
  const w = s.w;
  regrowForests(s);
  const civs = w.civs.filter((c) => c.alive);
  // Göç
  for (const a of civs) for (const b of civs) {
    if (a.id === b.id) continue;
    const r = s.rel(a.id, b.id);
    const charm = s.e(a, 'charm');
    if (!r.contact || r.war || s.relValue(a.id, b.id) < 15 - charm * 10) continue;
    const pa = prosperity(s, a) * (1 + charm * 0.25), pb = prosperity(s, b);
    const from = s.capital(b), to = s.capital(a);
    if (!from || !to || s.pop(from) < 14 || pa < pb * 1.5 + 0.3 || s.pop(to) >= s.housing(to) + 2) continue;
    if (!s.rng.chance(0.3)) continue;
    const moved = s.removePop(from, s.rng.int(1, 3));
    s.mergePop(to, moved);
    s.metric('migration');
    s.log('migration', `${s.raceStr(moved)} göçmen ${ek(from.name, 'dan')} ${ek(to.name, 'a')} yerleşti.`, { civ: a.id, tile: to.tile, cause: `${a.name} daha müreffeh, ilişkiler iyi` });
  }
  // Mülteciler
  if (s.year >= 3 && s.rng.chance(0.05)) {
    const cands = w.settlements.filter((x) => x.alive && s.pop(x) + 3 <= s.housing(x) + 2);
    if (cands.length) {
      const st = s.rng.pick(cands);
      const race = s.rng.pick(['elf', 'halfling', 'gnome', 'dragonborn', 'tiefling', 'human'] as RaceId[]);
      const n = s.rng.int(2, 4);
      s.addPop(st, race, n);
      s.metric('refugees');
      s.log('migration', `${n} ${RACES[race].name} mülteci ${ek(st.name, 'a')} sığındı.`, { civ: st.civ, tile: st.tile, cause: s.rng.pick(['Uzak diyarlardaki bir savaştan kaçtılar', 'Ormanları yanmıştı', 'Kıtlıktan kaçtılar', 'Bir ejderhanın gölgesinden kaçtılar']), major: true });
    }
  }
  // Melez doğumlar
  for (const st of w.settlements) {
    if (!st.alive) continue;
    const k = 'human-elf';
    if ((st.pop.human ?? 0) >= 3 && (st.pop.elf ?? 0) >= 2) {
      st.mixedSince[k] ??= s.day;
      if (s.day - st.mixedSince[k]! > YEAR && s.rng.chance(0.08)) {
        const first = !w.settlements.some((o) => (o.pop.halfelf ?? 0) > 0);
        s.addPop(st, 'halfelf', 1);
        s.metric('halfbreed');
        if (first) s.log('migration', `${ek(st.name, 'da')} ilk Yarı-elf doğdu.`, { civ: st.civ, tile: st.tile, cause: 'İnsanlar ve Elfler bir yıldır aynı ocakta yaşıyor', major: true });
      }
    } else delete st.mixedSince[k];
  }
  // Yeni kurucular
  for (const c of w.civs) {
    if (c.alive || c.respawned || s.day - (c.extinctDay ?? 0) < 150) continue;
    c.respawned = true;
    spawnFounders(s);
  }
}

function prosperity(s: Sim, c: Civ) {
  const pop = s.civPop(c);
  if (!pop) return 0;
  const food = Math.min(3, s.foodTotal(c) / (pop * FOOD_PER_POP * 60));
  const hous = s.civSettlements(c).reduce((a, x) => a + s.housing(x), 0) / pop;
  return food + Math.min(1.5, hous);
}

function spawnFounders(s: Sim) {
  const w = s.w;
  const used = new Set(w.civs.filter((c) => c.alive).map((c) => c.cls));
  const pool = CLASS_IDS.filter((k) => CLASSES[k].implemented && !used.has(k));
  if (!pool.length) return;
  const cls = s.rng.pick(pool);
  const avoid = w.settlements.filter((x) => x.alive).map((x) => x.tile);
  let best = -1, bs = -Infinity;
  for (let i = 0; i < w.tiles.length; i++) {
    const t = w.tiles[i];
    if (t.terrain !== 'grass' || t.owner >= 0 || t.camp !== undefined) continue;
    const d = avoid.length ? Math.min(...avoid.map((a) => s.g.dist(a, i))) : 20;
    if (d < 10) continue;
    const sc = Math.min(d, 15) + s.rng.next() * 3;
    if (sc > bs) { bs = sc; best = i; }
  }
  if (best < 0) return;
  const id = w.civs.length;
  const c = makeCiv(id, cls, s.day);
  w.civs.push(c);
  for (const row of w.relations) row.push(emptyRel());
  w.relations.push(w.civs.map(() => emptyRel()));
  w.settlements.push(makeSettlement(s.id(), id, CLASSES[cls].capital, best, { [CLASSES[cls].race]: 6 }, s.day));
  s.updateTerritory();
  s.recomputeEff(c);
  chooseResearch(s, c);
  s.log('world', `Ufukta yeni bir topluluk belirdi: ${c.name} (${RACES[c.race].plural}, ${CLASSES[cls].name}).`, { civ: id, tile: best, cause: 'Boşalan topraklar yeni yerleşimcileri çekti', major: true });
  void TECH;
}
