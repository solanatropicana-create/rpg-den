// Kahraman iradesi: serbest kahramanlar kiralanana dek kendi yollarını izler.
// Yol (avcı, gezgin, şifacı, bilge, paralı, karanlık), her mevsim hedef seçimi, izler ve efsane.
import type { Sim } from './sim';
import { YEAR } from './sim';
import { ek } from './tr';
import { GOODS, type Good } from '../data/goods';
import { CLASS_PATH, PATH_TR, TRAITS, type HeroAlign, type HeroClass, type HeroPath, type TraitId } from '../data/heroes';
import { heroCombatant, powerOf, resolveBattle, type Combatant } from './combat';
import { mod } from './rng';
import { campPower, gainXp, sendHero } from './heroes';
import { civTroops, recordBattle, syncHeroes, tileOf } from './agents';
import type { Agent, Camp, Civ, GoalKind, Hero, Inn, Quest, Settlement } from './types';

// ------------------------------------------------------------ doğuş
export function rollWill(s: Sim, cls: HeroClass): { align: HeroAlign; path: HeroPath } {
  const r = s.rng.next();
  let align: HeroAlign;
  if (cls === 'paladin') align = 'good';
  else if (cls === 'cleric') align = r < 0.7 ? 'good' : r < 0.92 ? 'neutral' : 'evil';
  else if (cls === 'rogue' || cls === 'barbarian') align = r < 0.35 ? 'good' : r < 0.72 ? 'neutral' : 'evil';
  else align = r < 0.42 ? 'good' : r < 0.82 ? 'neutral' : 'evil';
  let path = CLASS_PATH[cls];
  if (cls === 'paladin' && s.rng.chance(0.5)) path = 'hunter';
  if (align === 'evil' && s.rng.chance(0.4)) path = 'dark';
  else if (align !== 'good' && s.rng.chance(0.3)) path = 'mercenary';
  return { align, path };
}

export function note(s: Sim, h: Hero, text: string) {
  h.journal.push({ day: s.day, text });
  if (h.journal.length > 6) h.journal.shift();
}
export function hasTrait(h: Hero, t: TraitId) { return h.traits.includes(t); }
export function addTrait(s: Sim, h: Hero, t: TraitId, why: string) {
  if (hasTrait(h, t)) return;
  h.traits.push(t);
  note(s, h, `${TRAITS[t].name} izi: ${why}`);
  s.metric('trait_' + t);
  s.log('hero', `${h.name} yeni bir iz kazandı: ${TRAITS[t].name}.`, { tile: h.pos, civ: h.civ >= 0 ? h.civ : undefined, cause: why, major: t !== 'avenger' });
}
export function heroLabel(h: Hero) { return h.epithet ? `${h.name} «${h.epithet}»` : h.name; }

/** kötü kahraman paladine, iyi kahraman Paktçı'ya çalışmaz; kinli olduğu medeniyete de */
export function heroWillServe(c: Civ, h: Hero) {
  if (h.grudge === c.id) return false;
  if (h.align === 'good' && c.align.good < -0.3) return false;
  if (h.align === 'evil' && c.cls === 'paladin') return false;
  return true;
}

/** kahramanın savaştaki hâli: iz bonusları dâhil */
export function heroSide(h: Hero, side: 'A' | 'B', vs?: Camp['kind'], alone = false): Combatant {
  const c = heroCombatant(h, side);
  if (vs === 'goblin' && hasTrait(h, 'goblinslayer')) c.atk += 2;
  if (alone && hasTrait(h, 'lonewolf')) { c.atk += 1; c.ac += 1; }
  if (h.align === 'evil') c.evil = true;
  return c;
}

// ------------------------------------------------------------ yuva
export function innById(s: Sim, id: number | undefined): Inn | undefined { return id === undefined ? undefined : s.w.inns.find((i) => i.id === id); }
export function innAt(s: Sim, tile: number): Inn | undefined { const id = s.w.tiles[tile].inn; return id === undefined ? undefined : s.w.inns.find((i) => i.id === id && i.alive); }

/** en yakın canlı han ya da taverna */
export function nearestRest(s: Sim, from: number): { id: number; tile: number; inn: boolean } | undefined {
  let best: { id: number; tile: number; inn: boolean } | undefined, bd = 1e9;
  for (const i of s.w.inns) if (i.alive) { const d = s.g.dist(i.tile, from) - 3; if (d < bd) { bd = d; best = { id: i.id, tile: i.tile, inn: true }; } }
  for (const st of s.w.settlements) if (st.alive && st.civics.tavern) { const d = s.g.dist(st.tile, from); if (d < bd) { bd = d; best = { id: st.id, tile: st.tile, inn: false }; } }
  return best;
}
export function baseTile(s: Sim, h: Hero): number | undefined {
  if (h.baseInn) { const i = innById(s, h.base); return i && i.alive ? i.tile : undefined; }
  const st = s.settlement(h.base);
  return st && st.alive && st.civics.tavern ? st.tile : undefined;
}
/** serbest kahramanı yuvasına (yıkıldıysa en yakın dinlenme yerine) gönderir */
export function returnToBase(s: Sim, h: Hero) {
  if (h.state === 'dead' || h.state === 'gone' || h.state === 'retired') return;
  h.goal = undefined;
  let t = baseTile(s, h);
  if (t === undefined) {
    const r = nearestRest(s, h.pos);
    if (!r) { h.state = 'gone'; s.log('hero', `${h.name} dinlenecek bir yer bulamayınca uzak diyarlara gitti.`, { tile: h.pos }); return; }
    h.base = r.id; h.baseInn = r.inn; t = r.tile;
  }
  sendHero(s, h, t, 'tavern');
  if (h.state === 'tavern') { h.tavern = h.base; h.idleSince = s.day; }
}
export function heroAgent(s: Sim, h: Hero): Agent | undefined { return s.w.agents.find((a) => !a.dead && a.heroes?.includes(h.id)); }

// ------------------------------------------------------------ gelişim
/** seviye atlarken kazanılan bonus, yaşananlara göre */
export function growFromExperience(s: Sim, h: Hero) {
  const t = h.tally;
  const opts: [number, () => string][] = [
    [(t.goblin ?? 0) * 0.3 + (t.kills ?? 0) * 0.1, () => { h.bonus.atk++; return 'kamp savaşlarında bileği güçlendi (+1 saldırı)'; }],
    [(t.plague ?? 0) * 1.5 + (t.temple ?? 0) * 0.5, () => { h.maxHp += 4; h.hp += 4; return 'hastalık yatağında dayanıklılığı arttı (+4 can)'; }],
    [(t.ruins ?? 0) * 1.2 + (t.rob ?? 0) * 0.8, () => { h.ac++; return 'tuzaklardan sıyrılmayı öğrendi (+1 zırh)'; }],
    [(t.lib ?? 0) * 1.5, () => { h.stats[h.cls === 'wizard' ? 'int' : 'wis']++; return 'kütüphanelerde yeni bilgiler öğrendi'; }],
  ];
  const best = opts.reduce((a, b) => (b[0] > a[0] ? b : a));
  if (best[0] < 0.5) return;
  note(s, h, `Sv ${h.level}: ${best[1]()}`);
}

export function becomeLegend(s: Sim, h: Hero) {
  if (h.legend) return;
  h.legend = true;
  addTrait(s, h, 'legend', 'Seviye tavanına ulaştı');
  const where = h.baseInn ? `${innById(s, h.base)?.name ?? 'Han'} Hanı'nın` : `${s.settlement(h.base)?.name ?? 'Yurdunun'}`;
  s.metric('legend');
  s.log('hero', `Ozanlar ${heroLabel(h)} için şarkı yakıyor; ${where} önüne heykeli dikildi.`, { tile: h.pos, civ: h.civ >= 0 ? h.civ : undefined, major: true, cause: `${h.kills} düşman, ${h.traits.length} iz` });
}

// ------------------------------------------------------------ olaylara tepki
/** yerleşim yandı: orada doğmuş serbest kahramanlar intikamcı olur */
export function onHomeBurned(s: Sim, st: Settlement, byCamp?: Camp, byCiv?: Civ) {
  for (const h of s.w.heroes) {
    if (h.birth !== st.id || h.state === 'dead' || h.state === 'gone' || h.state === 'retired') continue;
    if (byCamp && h.vendetta !== byCamp.id) {
      h.vendetta = byCamp.id;
      if (h.civ === -1 && h.path !== 'healer') h.path = 'hunter';
      addTrait(s, h, 'avenger', `${ek(byCamp.name, 'in')} ${ek(st.name, 'i')} yakmasının intikamı`);
    }
    if (byCiv && h.grudge !== byCiv.id) { h.grudge = byCiv.id; note(s, h, `${byCiv.name} doğduğu ${ek(st.name, 'i')} yaktı; onlara asla hizmet etmeyecek`); }
  }
}

/** kamp savaşı sonrası: sayaçlar, izler, lakap, yol değişimi */
export function afterCampFight(s: Sim, side: Combatant[], cp: Camp, won: boolean, hadBoss: boolean, rolls: { d20: number; who: string }[]) {
  const hs = side.filter((x) => x.hero).map((x) => x.hero!);
  const alive = hs.filter((h) => h.state !== 'dead');
  for (const x of side) {
    const h = x.hero; if (!h || h.state === 'dead') continue;
    h.tally.kills = (h.tally.kills ?? 0) + x.kills;
    if (cp.kind === 'goblin') { h.tally.goblin = (h.tally.goblin ?? 0) + x.kills; if (h.tally.goblin >= 10) addTrait(s, h, 'goblinslayer', `${h.tally.goblin} goblin devirdi`); }
    if (won) {
      h.tally.fails = 0; h.tally.camps = (h.tally.camps ?? 0) + 1;
      if (h.tally.camps >= 5 && h.level >= 4) becomeLegend(s, h);
      note(s, h, `${cp.name} yerle bir edildi`);
      if (h.vendetta === cp.id) { h.vendetta = undefined; note(s, h, 'intikamını aldı'); }
      if (hadBoss && !h.epithet && rolls.some((r) => r.d20 === 20 && r.who === h.name)) {
        h.epithet = `${cp.name.split(' ')[0]} Belası`;
        s.metric('epithet');
        s.log('hero', `${h.name} artık «${h.epithet}» diye anılıyor.`, { tile: cp.tile, civ: h.civ >= 0 ? h.civ : undefined, major: true, cause: 'Kamp önderini doğal 20 ile devirdi' });
      }
    } else {
      h.tally.fails = (h.tally.fails ?? 0) + 1;
      note(s, h, `${ek(cp.name, 'da')} püskürtüldü`);
      if (h.civ === -1 && h.align === 'evil' && h.path !== 'dark' && h.tally.fails >= 2) {
        h.path = 'dark';
        s.log('hero', `Art arda yenilgiler ${ek(h.name, 'i')} karanlık yola itti; artık kervan soyuyor.`, { tile: h.pos, major: true, cause: 'Kötü hizalı, iki başarısız sefer' });
      }
    }
  }
  if (hs.length >= 2 && alive.length === 1) {
    const h = alive[0];
    h.soloUntil = s.day + 2 * YEAR;
    addTrait(s, h, 'lonewolf', 'Partisinin tek sağ kalanı');
  }
}

// ------------------------------------------------------------ hedef seçimi
const FIT: Record<GoalKind, Partial<Record<HeroPath, number>>> = {
  quest: { hunter: 1.2, wanderer: 0.8, healer: 0.6, sage: 0.5, mercenary: 1.7, dark: 0.6 },
  hunt: { hunter: 1.3, mercenary: 0.3, dark: 0.3, healer: 0.15, wanderer: 0.2 },
  ruin: { wanderer: 1.6, sage: 0.6, mercenary: 0.4, dark: 0.6, hunter: 0.3, healer: 0.2 },
  plague: { healer: 1.8 },
  temple: { healer: 0.7, hunter: 0.1, sage: 0.2 },
  library: { sage: 1.6, wanderer: 0.3, healer: 0.2 },
  rob: { dark: 1.6 },
  duel: { hunter: 0.9, healer: 0.6 },
};

interface Option { kind: GoalKind; tile: number; target?: number; score: number; text: string; crew?: Hero[]; quest?: Quest; camp?: Camp }

export function willTick(s: Sim) {
  const w = s.w;
  for (const h of w.heroes) {
    if (h.civ !== -1) continue;
    if (h.state === 'quest' && h.goal && !heroAgent(s, h)) { progressGoal(s, h); continue; }
    if (h.state !== 'tavern') continue;
    if (h.hp < h.maxHp * 0.8) continue;
    if ((Math.floor(s.day / 5) + h.id) % 6 !== 0) continue;   // her kahraman mevsimde bir kez karar verir
    chooseGoal(s, h);
  }
}

function distPen(s: Sim, a: number, b: number) { return 1 + s.g.dist(a, b) / 12; }

function partyFor(s: Sim, h: Hero, need: number, greed: number): Hero[] | null {
  const solo = powerOf([heroSide(h, 'A', undefined, true)]);
  if (solo * greed >= need) return [h];
  if ((h.soloUntil ?? 0) > s.day) return null;
  const mates = s.w.heroes.filter((x) => x !== h && x.civ === -1 && x.state === 'tavern' && x.tavern === h.tavern && x.hp >= x.maxHp * 0.8
    && (x.soloUntil ?? 0) <= s.day && !(x.align === 'good' && h.align === 'evil') && !(x.align === 'evil' && h.align === 'good') && !x.auction?.bids.length)
    .sort((a, b) => b.level - a.level || a.id - b.id);
  const crew = [h];
  for (const m of mates) {
    if (crew.some((c) => (c.align === 'good' && m.align === 'evil') || (c.align === 'evil' && m.align === 'good'))) continue;
    crew.push(m);
    if (powerOf(crew.map((c) => heroSide(c, 'A'))) * greed >= need) return crew;
    if (crew.length >= 4) break;
  }
  return null;
}

export function chooseGoal(s: Sim, h: Hero) {
  const w = s.w;
  const opts: Option[] = [];
  const fit = (k: GoalKind) => FIT[k][h.path] ?? 0;
  const me = powerOf([heroSide(h, 'A', undefined, true)]);
  // 1) panodaki ilanlar
  if (fit('quest') > 0) for (const q of w.quests) {
    if (!q.open) continue;
    const cp = w.camps.find((c) => c.id === q.camp && c.alive);
    if (!cp || s.g.dist(h.pos, cp.tile) > 30) continue;
    if (q.civ >= 0 && h.grudge === q.civ) continue;
    const need = campPower(s, cp) * 1.0, greed = 1 + Math.min(0.4, q.bounty / 300);
    const crew = partyFor(s, h, need, greed);
    if (!crew) continue;
    const risk = need / powerOf(crew.map((c) => heroSide(c, 'A', cp.kind, crew.length === 1)));
    const venge = h.vendetta === cp.id ? 3 : 1;
    const rep = q.civ >= 0 && (h.rep[q.civ] ?? 0) >= 3 ? 1.15 : 1;
    opts.push({ kind: 'quest', tile: cp.tile, score: fit('quest') * venge * rep * (q.bounty / crew.length + 30) / (Math.max(0.3, risk) * distPen(s, h.pos, cp.tile)), text: `${cp.name} ilanı`, crew, quest: q, camp: cp });
  }
  // 2) ilansız av
  if (fit('hunt') > 0 || h.vendetta !== undefined) for (const cp of w.camps) {
    if (!cp.alive || s.g.dist(h.pos, cp.tile) > 22) continue;
    if (w.quests.some((q) => q.camp === cp.id && q.open)) continue;
    const need = campPower(s, cp) * (h.vendetta === cp.id ? 1.0 : 1.2);
    const crew = partyFor(s, h, need, 1);
    if (!crew) continue;
    const risk = need / powerOf(crew.map((c) => heroSide(c, 'A', cp.kind, crew.length === 1)));
    const venge = h.vendetta === cp.id ? 4 : 1;
    opts.push({ kind: 'hunt', tile: cp.tile, score: Math.max(fit('hunt'), venge > 1 ? 1 : 0) * venge * (cp.loot / crew.length + 25) / (Math.max(0.3, risk) * distPen(s, h.pos, cp.tile)), text: venge > 1 ? `${ek(cp.name, 'dan')} intikam` : `${cp.name} avı`, crew, camp: cp });
  }
  // 3) harabeler
  if (fit('ruin') > 0) for (const st of w.settlements) {
    if (st.alive || (h.seen ?? []).includes(st.id) || w.settlements.some((o) => o.alive && o.tile === st.tile) || w.tiles[st.tile].camp !== undefined) continue;
    if (s.g.dist(h.pos, st.tile) > 26) continue;
    opts.push({ kind: 'ruin', tile: st.tile, target: st.id, score: fit('ruin') * 35 / (0.5 * distPen(s, h.pos, st.tile)), text: `${st.name} harabesi` });
  }
  // 4) salgın, 5) hac, 6) kütüphane
  for (const st of w.settlements) {
    if (!st.alive || s.g.dist(h.pos, st.tile) > 26) continue;
    const c = w.civs[st.civ];
    if (h.grudge === c.id) continue;
    if (st.plague && fit('plague') > 0 && st.plague.until - s.day > 15 && !w.heroes.some((x) => x.goal?.kind === 'plague' && x.goal.target === st.id))
      opts.push({ kind: 'plague', tile: st.tile, target: st.id, score: fit('plague') * (35 + st.plague.severity * 20) / ((hasTrait(h, 'plaguewalker') ? 0.35 : 0.7) * distPen(s, h.pos, st.tile)), text: `salgınlı ${st.name}` });
    if (st.civics.temple && fit('temple') > 0 && !(h.seen ?? []).includes(st.id) && s.day - (h.tally.templeDay ?? -9999) > YEAR)
      opts.push({ kind: 'temple', tile: st.tile, target: st.id, score: fit('temple') * 12 / distPen(s, h.pos, st.tile), text: `${ek(st.name, 'in')} sunağına hac` });
    if (st.civics.library && fit('library') > 0 && !(h.seen ?? []).includes(st.id) && s.day - (h.tally.libDay ?? -9999) > YEAR)
      opts.push({ kind: 'library', tile: st.tile, target: st.id, score: fit('library') * 26 / (0.6 * distPen(s, h.pos, st.tile)), text: `${ek(st.name, 'in')} kütüphanesi` });
  }
  // 7) kervan soygunu
  if (fit('rob') > 0 && s.day - (h.tally.robDay ?? -9999) > YEAR / 2) for (const r of w.routes) {
    if (!r.alive || r.path.length < 6) continue;
    const t = r.path[Math.floor(r.path.length / 2)];
    if (s.g.dist(h.pos, t) > 18 || w.tiles[t].owner >= 0 && s.settlement(w.tiles[t].owner)?.tile === t) continue;
    const guard = powerOf(civTroops(s, w.civs[s.settlement(r.a)?.civ ?? 0], 3, 'B'));
    const risk = guard / me;
    if (risk > 1.3) continue;
    opts.push({ kind: 'rob', tile: t, target: r.id, score: fit('rob') * 35 / (Math.max(0.3, risk) * distPen(s, h.pos, t)), text: 'kervan yolunda pusu' });
  }
  // 8) karanlık yola sapmış kahramanı avla
  if (fit('duel') > 0 && h.align === 'good') for (const o of w.heroes) {
    if (o.civ !== -1 || o.path !== 'dark' || !(o.tally.rob ?? 0) || o === h || (o.state !== 'tavern' && o.state !== 'quest') || s.g.dist(h.pos, o.pos) > 20) continue;
    const risk = powerOf([heroSide(o, 'B')]) / me;
    if (risk > 1.1) continue;
    opts.push({ kind: 'duel', tile: o.pos, target: o.id, score: fit('duel') * 30 / (Math.max(0.3, risk) * distPen(s, h.pos, o.pos)), text: `kara yola sapan ${o.name}` });
  }
  if (!opts.length) return;
  opts.sort((a, b) => b.score - a.score || a.tile - b.tile);
  const o = opts[0];
  if (o.score < 5) return;
  startGoal(s, h, o);
}

function startGoal(s: Sim, h: Hero, o: Option) {
  const w = s.w;
  h.lastGoal = s.day;
  if (o.kind === 'quest' || o.kind === 'hunt') {
    const crew = o.crew!, cp = o.camp!;
    const path = s.path(h.pos, cp.tile);
    if (!path) return;
    if (o.quest) { o.quest.takenBy = crew.map((x) => x.id); o.quest.open = false; }
    for (const x of crew) {
      x.state = 'quest'; x.tavern = -1; x.lastGoal = s.day;
      x.goal = { kind: o.kind, tile: cp.tile, target: cp.id, text: o.text, since: s.day };
      note(s, x, o.kind === 'quest' ? `${cp.name} ilanını kopardı` : `${ek(cp.name, 'a')} ava çıktı`);
    }
    w.agents.push({ id: s.id(), kind: 'party', civ: -1, path, step: 0, progress: 0, speed: 0.85, heroes: crew.map((x) => x.id), to: cp.id, quest: o.quest?.id, purpose: 'quest' });
    s.metric(o.kind === 'quest' ? 'questTaken' : 'goalHunt');
    const poster = o.quest ? (o.quest.civ >= 0 ? w.civs[o.quest.civ].name : `${innById(s, o.quest.inn)?.name ?? 'Han'} Hanı`) : '';
    s.log('quest', crew.length > 1
      ? `Bir macera grubu kuruldu: ${crew.map((x) => s.heroTitle(x)).join(', ')}. Hedef: ${cp.name}.`
      : o.quest ? `${s.heroTitle(h)}, ${ek(poster, 'in')} ilanını kopardı ve ${ek(cp.name, 'a')} yola çıktı.` : `${s.heroTitle(h)} kendi başına ${ek(cp.name, 'a')} ava çıktı.`,
    { tile: h.pos, civ: o.quest && o.quest.civ >= 0 ? o.quest.civ : undefined, cause: o.quest ? `Ödül: ${o.quest.bounty} altın` : h.vendetta === cp.id ? 'İntikam' : `${PATH_TR[h.path]} yolu`, major: true });
    return;
  }
  h.goal = { kind: o.kind, tile: o.tile, target: o.target, text: o.text, since: s.day };
  note(s, h, `Hedef: ${o.text}`);
  s.metric('goal_' + o.kind);
  if (o.kind !== 'temple') s.log('hero', `${s.heroTitle(h)} yola çıktı: ${o.text}.`, { tile: h.pos, cause: `${PATH_TR[h.path]} yolu` });
  const path = s.path(h.pos, o.tile);
  if (!path || path.length < 2) { h.state = 'quest'; h.tavern = -1; arriveGoal(s, h); return; }
  h.state = 'traveling'; h.tavern = -1;
  w.agents.push({ id: s.id(), kind: 'hero', civ: -1, path, step: 0, progress: 0, speed: 0.9, heroes: [h.id], purpose: 'goal' });
}

// ------------------------------------------------------------ hedefte
export function arriveGoal(s: Sim, h: Hero) {
  const g = h.goal;
  if (!g || h.state === 'dead') { returnToBase(s, h); return; }
  h.state = 'quest'; h.tavern = -1;
  const w = s.w;
  switch (g.kind) {
    case 'ruin': {
      const st = s.settlement(g.target!);
      (h.seen ??= []).push(g.target!);
      const bonus = hasTrait(h, 'ruinrat') ? 2 : 0;
      const roll = s.rng.dice(1, 20) + mod(h.stats.dex) + bonus;
      if (roll < 11) {
        const dmg = s.rng.dice(Math.max(1, Math.ceil(h.level / 2)), 8);
        h.hp = Math.max(1, h.hp - dmg);
        note(s, h, `${st?.name ?? 'harabe'} tuzağında yaralandı`);
        s.log('hero', `${h.name}, ${st?.name ?? 'bir'} harabesinde tuzağa düştü (${dmg} hasar).`, { tile: h.pos, cause: `d20 ${roll}` });
      } else {
        const gold = s.rng.int(10, 40) + bonus * 5;
        h.gold += gold;
        let found = '';
        if (s.rng.chance(0.12 + bonus * 0.04)) { h.bonus.atk++; found = ' ve eski bir büyülü silah'; }
        note(s, h, `${st?.name ?? 'harabe'} harabesinde ${gold} altın${found} buldu`);
        s.log('hero', `${h.name}, ${st?.name ?? 'bir'} harabesinde ${gold} altın${found} buldu.`, { tile: h.pos, major: !!found, cause: `${PATH_TR[h.path]} yolu` });
        gainXp(s, h, 70);
      }
      h.tally.ruins = (h.tally.ruins ?? 0) + 1;
      if (h.tally.ruins >= 3) addTrait(s, h, 'ruinrat', `${h.tally.ruins} harabe keşfetti`);
      g.stay = s.day + 5;
      return;
    }
    case 'plague': {
      const st = s.settlement(g.target!);
      if (!st || !st.alive || !st.plague) { returnToBase(s, h); return; }
      const pl = st.plague;
      const heal = h.cls === 'cleric' || h.cls === 'druid' || h.cls === 'paladin' ? 0.55 : 0.75;
      pl.severity *= heal; pl.until = Math.max(s.day + 8, pl.until - 25);
      s.metric('plagueHealed');
      const c = w.civs[st.civ];
      h.rep[c.id] = (h.rep[c.id] ?? 0) + 1;
      h.tally.plague = (h.tally.plague ?? 0) + 1;
      if (!hasTrait(h, 'plaguewalker') && s.rng.dice(1, 20) + mod(h.stats.con) < 10) {
        const dmg = Math.ceil(h.maxHp * 0.35);
        h.hp = Math.max(1, h.hp - dmg);
        note(s, h, `${ek(st.name, 'da')} hastalığa yakalandı ama atlattı`);
      }
      addTrait(s, h, 'plaguewalker', `Salgınlı ${ek(st.name, 'da')} şifa dağıttı`);
      note(s, h, `${ek(st.name, 'da')} şifa dağıttı`);
      s.log('hero', `${s.heroTitle(h)} salgınlı ${ek(st.name, 'a')} şifa taşıdı; hastalık geriliyor.`, { tile: st.tile, civ: c.id, major: true, cause: `${PATH_TR[h.path]} yolu` });
      gainXp(s, h, 90);
      g.stay = s.day + 15;
      return;
    }
    case 'temple': {
      (h.seen ??= []).push(g.target!);
      h.hp = h.maxHp;
      h.tally.temple = (h.tally.temple ?? 0) + 1; h.tally.templeDay = s.day;
      gainXp(s, h, 30);
      note(s, h, `${s.settlement(g.target!)?.name ?? 'bir'} sunağında dua etti`);
      g.stay = s.day + 5;
      return;
    }
    case 'library': {
      (h.seen ??= []).push(g.target!);
      g.stay = s.day + 20;
      return;
    }
    case 'rob': g.stay = s.day + 40; return;
    case 'duel': duel(s, h); return;
    default: returnToBase(s, h);
  }
}

function progressGoal(s: Sim, h: Hero) {
  const g = h.goal!;
  if (g.kind === 'rob') {
    const cv = s.w.agents.find((a) => a.kind === 'caravan' && !a.dead && s.g.dist(tileOf(a), h.pos) <= 2);
    if (cv) { robCaravan(s, h, cv); return; }
  }
  if (g.stay !== undefined && s.day < g.stay) return;
  if (g.kind === 'library') {
    const st = s.settlement(g.target!);
    h.tally.lib = (h.tally.lib ?? 0) + 1; h.tally.libDay = s.day;
    note(s, h, `${st?.name ?? 'bir'} kütüphanesinde çalıştı`);
    s.log('hero', `${h.name}, ${ek(st?.name ?? 'kütüphane', 'in')} kütüphanesinde eski kitaplardan yeni büyüler öğrendi.`, { tile: h.pos, civ: st?.civ, cause: `${PATH_TR[h.path]} yolu` });
    gainXp(s, h, h.path === 'sage' ? 110 : 60);
  }
  h.idleSince = s.day;
  returnToBase(s, h);
}

function robCaravan(s: Sim, h: Hero, cv: Agent) {
  const c = s.w.civs[cv.civ];
  const side = [heroSide(h, 'A', undefined, true)];
  const guards = civTroops(s, c, cv.troops ?? 2, 'B');
  const b = resolveBattle(s.rng, side, guards, { id: s.id(), day: s.day, tile: h.pos, title: 'Kervan soygunu', sideA: h.name, sideB: `${c.name} kervanı`, moraleA: 0.4, moraleB: 0.55 });
  recordBattle(s, b);
  syncHeroes(s, side, b.winner === 'A' ? 60 : 10);
  h.tally.rob = (h.tally.rob ?? 0) + 1; h.tally.robDay = s.day;
  s.metric('goalRob');
  h.rep[c.id] = (h.rep[c.id] ?? 0) - 2;
  if (b.winner === 'A') {
    let v = 0;
    for (const g in cv.cargo ?? {}) v += (cv.cargo![g as Good] ?? 0) * (GOODS[g as Good]?.base ?? 1);
    const gold = Math.max(8, Math.round(v * 0.6));
    h.gold += gold;
    cv.dead = true;
    const r = s.w.routes.find((x) => x.id === cv.route); if (r) r.nextDepart = s.day + 60;
    c.threat += 0.15;
    note(s, h, `${c.name} kervanını soydu (${gold} altın)`);
    s.log('raid', `${heroLabel(h)}, ${c.name} kervanını soydu ve ${gold} altınlık yükle kayboldu.`, { civ: c.id, tile: h.pos, battle: b.id, major: true, cause: 'Karanlık yol' });
  } else if (h.state !== 'dead') {
    note(s, h, `${c.name} kervanının muhafızlarına yenildi`);
    s.log('raid', `${c.name} kervanı, ${ek(h.name, 'in')} pususunu savuşturdu.`, { civ: c.id, tile: h.pos, battle: b.id });
  }
  if (h.state !== 'dead') returnToBase(s, h);
}

function duel(s: Sim, h: Hero) {
  const o = s.w.heroes.find((x) => x.id === h.goal!.target);
  if (!o || o.civ !== -1 || o.state === 'dead' || o.state === 'gone' || s.g.dist(o.pos, h.pos) > 2) {
    note(s, h, 'aradığı haydudu bulamadı'); returnToBase(s, h); return;
  }
  const A = [heroSide(h, 'A', undefined, true)], B = [heroSide(o, 'B', undefined, true)];
  const b = resolveBattle(s.rng, A, B, { id: s.id(), day: s.day, tile: h.pos, title: 'Düello', sideA: heroLabel(h), sideB: heroLabel(o), moraleA: 0.25, moraleB: 0.35 });
  recordBattle(s, b);
  syncHeroes(s, [...A, ...B], 80);
  s.metric('duel');
  const win = b.winner === 'A';
  const dead = (x: Hero) => x.state === 'dead';
  s.log('hero', win ? `${heroLabel(h)}, kara yola sapan ${ek(o.name, 'i')} düelloda ${dead(o) ? 'öldürdü' : 'kaçırdı'}.` : `${heroLabel(o)}, peşine düşen ${ek(h.name, 'i')} düelloda ${dead(h) ? 'öldürdü' : 'kaçırdı'}.`,
    { tile: h.pos, battle: b.id, major: true, cause: 'İyi yürekli kahramanlar haydutları avlar' });
  note(s, h, win ? `${o.name} ile düelloyu kazandı` : `${o.name} ile düelloyu kaybetti`);
  if (!dead(o)) { note(s, o, win ? `${h.name} ile düelloyu kaybetti` : `${h.name} ile düelloyu kazandı`); if (o.state !== 'traveling') returnToBase(s, o); }
  if (!dead(h)) returnToBase(s, h);
}

// ------------------------------------------------------------ emeklilik
export function maybeRetire(s: Sim, h: Hero): boolean {
  if (h.level < 4 || s.day - h.born < 12 * YEAR || !s.rng.chance(0.25)) return false;
  const inns = s.w.inns.filter((i) => i.alive).sort((a, b) => s.g.dist(a.tile, h.pos) - s.g.dist(b.tile, h.pos));
  const inn = h.baseInn && innById(s, h.base)?.alive ? innById(s, h.base)! : inns[0];
  h.state = 'retired'; h.civ = -1; h.contract = undefined; h.goal = undefined; h.auction = undefined;
  for (const a of s.w.agents) if (a.heroes?.includes(h.id) && a.kind === 'hero') a.dead = true;
  s.metric('retire');
  if (inn) {
    h.pos = inn.tile; h.base = inn.id; h.baseInn = true;
    let role: string;
    if (inn.teacher === undefined || s.w.heroes.find((x) => x.id === inn.teacher)?.state !== 'retired') { inn.teacher = h.id; role = 'öğretmenlik yapıyor; burada yetişenler bir adım önde doğacak'; }
    else { inn.keeper = h.name; role = 'hanı devraldı ve artık hancı'; }
    note(s, h, `emekli oldu: ${role}`);
    s.log('inn', `${heroLabel(h)} kılıcını astı: ${inn.name} Hanı'nda ${role}.`, { tile: inn.tile, major: true, cause: `${Math.floor((s.day - h.born) / YEAR)} yıllık macera, Sv ${h.level}` });
  } else {
    note(s, h, 'emekli oldu');
    s.log('hero', `${heroLabel(h)} kılıcını astı ve yurduna çekildi.`, { tile: h.pos, major: true });
  }
  return true;
}

