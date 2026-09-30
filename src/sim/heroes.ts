// Taverna, kahramanlar, görevler ve seferler.
import type { Sim } from './sim';
import { FOOD_PER_POP } from './sim';
import { ek } from './tr';
import { mod } from './rng';
import { CLASSES, type RaceId } from '../data/classes';
import { HERO_CLASSES, HERO_CLASS_IDS, HERO_NAMES, EPITHETS, HERO_ORIGINS, HERO_DRIVES, RACE_STAT, STATS, XP_LEVELS, type HeroClass, type Stat } from '../data/heroes';
import { heroAc, heroCombatant, powerOf, powerVs, type Combatant } from './combat';
import { monsterSide, campAway } from './monsters';
import { civTroops, drawSoldiers, probeAllies, agentCombatants, etaDays, MUSTER_CAMP } from './agents';
import type { Agent, Camp, Civ, Hero, Quest, Settlement } from './types';
import { YEAR } from './sim';
import { rollWill, willTick, growFromExperience, becomeLegend, heroWillServe, note, maybeRetire, returnToBase, innById } from './will';

export function tavernsTick(s: Sim) {
  for (const st of s.w.settlements) {
    if (!st.alive || !(st.civics.tavern ?? 0)) continue;
    const present = s.w.heroes.filter((h) => h.civ === -1 && !h.baseInn && h.base === st.id && h.state !== 'dead' && h.state !== 'gone' && h.state !== 'retired').length;
    if (present >= 3) continue;
    if (s.rng.chance(present === 0 ? 1 / 70 : 1 / 130)) spawnHero(s, st);
  }
}

export function spawnHero(s: Sim, st: Settlement) {
  const civ = s.w.civs[st.civ];
  const locals = (Object.keys(st.pop) as RaceId[]).filter((r) => (st.pop[r] ?? 0) > 0);
  const race: RaceId = s.rng.chance(0.65) && locals.length ? s.rng.weighted(locals, (r) => st.pop[r] ?? 0)! : s.rng.pick(['human', 'dwarf', 'elf', 'halfling', 'gnome', 'halfelf', 'halforc', 'dragonborn', 'tiefling'] as RaceId[]);
  const affinity = CLASSES[civ.cls].heroClass as HeroClass;
  const cls = s.rng.weighted(HERO_CLASS_IDS, (k) => (k === affinity ? 3 : 1))!;
  let level = s.rng.chance(0.6) ? 1 : s.rng.chance(0.75) ? 2 : 3;
  level = Math.min(5, level + Math.floor(s.year / 6) + (cls === affinity ? s.e(civ, 'heroLevel') : 0));
  const h = makeHero(s, { race, cls, level, tile: st.tile, base: st.id, baseInn: false });
  s.metric('heroSpawn');
  s.log('hero', `${ek(st.name, 'in')} tavernasına bir yabancı geldi: ${s.heroTitle(h)}.`, { tile: st.tile, civ: st.civ, cause: h.bio.charAt(0).toUpperCase() + h.bio.slice(1) });
}

export function makeHero(s: Sim, o: { race: RaceId; cls: HeroClass; level: number; tile: number; base: number; baseInn: boolean }): Hero {
  const { race, cls, level } = o;
  const rolls = STATS.map(() => { const d = [1, 2, 3, 4].map(() => s.rng.dice(1, 6)).sort((a, b) => a - b); return d[1] + d[2] + d[3]; }).sort((a, b) => b - a);
  const stats = {} as Record<Stat, number>;
  HERO_CLASSES[cls].priority.forEach((k, i) => (stats[k] = rolls[i]));
  const rb = RACE_STAT[race];
  for (const k in rb) stats[k as Stat] += rb[k as Stat]!;
  const def = HERO_CLASSES[cls];
  const con = mod(stats.con), dex = mod(stats.dex);
  const maxHp = def.hitDie + con + (level - 1) * (def.hitDie / 2 + 1 + con);
  const name = `${s.rng.pick(HERO_NAMES[race])} ${s.rng.chance(0.5) ? s.rng.pick(EPITHETS) : ''}`.trim();
  const will = rollWill(s, cls);
  const h: Hero = {
    id: s.id(), name, race, cls, level, xp: XP_LEVELS[level - 1], stats, maxHp, hp: maxHp, ac: heroAc(cls, dex, level), civ: -1, pos: o.tile, tavern: o.base,
    state: 'tavern', born: s.day, idleSince: s.day, kills: 0, gold: s.rng.int(3, 12), bio: `${s.rng.pick(HERO_ORIGINS)}, ${s.rng.pick(HERO_DRIVES)}.`,
    align: will.align, path: will.path, traits: [], tally: {}, bonus: { atk: 0 }, rep: {}, journal: [], base: o.base, baseInn: o.baseInn, birth: o.base,
  };
  note(s, h, `${o.baseInn ? (innById(s, o.base)?.name ?? '') + ' Hanı' : (s.settlement(o.base)?.name ?? '') + ' tavernası'}nda doğdu`);
  s.w.heroes.push(h);
  return h;
}
/** hanlardaki fiyat çıpası (K) */
export function heroBaseCost(h: Hero) { return 35 + 25 * h.level; }

export function gainXp(s: Sim, h: Hero, xp: number) {
  h.xp += xp;
  while (h.level < 5 && h.xp >= XP_LEVELS[h.level]) {
    h.level++;
    const inc = HERO_CLASSES[h.cls].hitDie / 2 + 1 + mod(h.stats.con);
    h.maxHp += inc; h.hp += inc;
    if (h.cls === 'fighter' && h.level === 3) h.ac++;
    growFromExperience(s, h);
    s.log('hero', `${h.name} ${h.level}. seviyeye yükseldi!${h.cls === 'wizard' && h.level === 5 ? ' Artık Ateş Topu büyüsünü biliyor.' : h.level === 5 && ['fighter', 'paladin', 'barbarian', 'ranger'].includes(h.cls) ? ' Artık turda iki kez saldırıyor.' : ''}`, { tile: h.pos, civ: h.civ >= 0 ? h.civ : undefined, major: h.level >= 4 });
    if (h.level === 5) becomeLegend(s, h);
  }
}

export function heroCost(s: Sim, c: Civ, h: Hero) {
  const aff = CLASSES[c.cls].heroClass === h.cls ? 0.75 : 1;
  return { gold: Math.round((35 + 25 * h.level) * aff), food: Math.round((30 + 10 * h.level) * aff) };
}

export function considerHero(s: Sim, c: Civ) {
  const pop = s.civPop(c);
  const mine = s.civHeroes(c);
  if (mine.length >= 1 + Math.floor(pop / 35)) return;
  const war = s.inWar(c);
  const want = c.threat > 0.25 || war || (mine.length === 0 && s.st(c, 'gold') > 80) || s.st(c, 'gold') > 200;
  if (!want) return;
  const avail = s.w.heroes.filter((h) => h.civ === -1 && h.state === 'tavern' && !h.baseInn && heroWillServe(c, h)).filter((h) => {
    const t = s.settlement(h.tavern);
    return t && (t.civ === c.id || (s.rel(c.id, t.civ).contact && s.relValue(c.id, t.civ) >= 0 && !s.atWar(c.id, t.civ)));
  });
  const reserve = pop * FOOD_PER_POP * 25;
  const ok = avail.filter((h) => { const k = heroCost(s, c, h); return s.st(c, 'gold') >= k.gold && s.foodTotal(c) - k.food >= reserve; });
  if (!ok.length) return;
  const aff = CLASSES[c.cls].heroClass;
  const h = ok.sort((a, b) => b.level - a.level + (b.cls === aff ? 1 : 0) - (a.cls === aff ? 1 : 0))[0];
  const k = heroCost(s, c, h);
  s.add(c, 'gold', -k.gold);
  let f = k.food;
  for (const g of ['grain', 'meat', 'fish', 'bread'] as const) { const take = Math.min(f, s.st(c, g)); s.add(c, g, -take); f -= take; }
  h.civ = c.id;
  h.hired = (h.hired ?? 0) + 1;
  h.goal = undefined; h.auction = undefined;
  note(s, h, `${c.name} saflarına katıldı`);
  const cap = s.capital(c)!;
  const why = war ? 'savaşta güçlü bir kol gerekiyordu' : c.threat > 0.25 ? 'canavar tehdidine karşı' : 'hazine doluydu, şan isteniyordu';
  s.log('hero', `${c.name}, ${s.heroTitle(h)} adlı kahramanı ${k.gold} altın ve ${k.food} gıda karşılığında saflarına kattı.`, { civ: c.id, tile: h.pos, cause: why.charAt(0).toUpperCase() + why.slice(1), major: true });
  s.metric('heroBought');
  sendHero(s, h, cap.tile, 'home');
}

export function sendHero(s: Sim, h: Hero, tile: number, then: Hero['state']) {
  if (h.pos === tile) { h.state = then; return; }
  const path = s.path(h.pos, tile);
  if (!path) { h.pos = tile; h.state = then; return; }
  h.state = 'traveling'; h.tavern = -1;
  s.w.agents.push({ id: s.id(), kind: 'hero', civ: h.civ, path, step: 0, progress: 0, speed: 0.9, heroes: [h.id], purpose: then });
}

/** kampın gücü; vsAc: saldıranların ortalama zırhı */
export function campPower(s: Sim, cp: Camp, vsAc = 15) { return powerOf(campForce(s, cp), vsAc); }
export function campForce(s: Sim, cp: Camp): Combatant[] { return monsterSide(cp.kind, cp.count + campAway(s, cp), cp.boss, 'B'); }

/** Bir medeniyeti en çok tehdit eden kamp */
export function threatCamp(s: Sim, c: Civ): Camp | undefined {
  const ss = s.civSettlements(c);
  let best: Camp | undefined, bd = 1e9;
  for (const cp of s.w.camps) {
    if (!cp.alive || (cp.kind === 'pirate' && s.w.tiles[cp.tile].isle)) continue; // ada korsanı: korsan avı (sea.ts)
    const d = Math.min(...ss.map((x) => s.g.dist(x.tile, cp.tile)));
    if (d < bd && d <= 18) { bd = d; best = cp; }
  }
  return best;
}

export function considerQuest(s: Sim, c: Civ) {
  // hedef: tehdit eden kamp ya da topraklarındaki bir yatağı işgal eden kamp
  const ss = s.civSettlements(c);
  const hunt = s.e(c, 'favoredHunt') > 0 || c.cls === 'ranger';
  const occ = s.w.camps.filter((x) => x.alive && !s.w.tiles[x.tile].isle && s.w.tiles[x.tile].deposit >= 0 && ss.some((st) => s.g.dist(st.tile, x.tile) <= 9))
    .sort((x, y) => Math.min(...ss.map((st) => s.g.dist(st.tile, x.tile))) - Math.min(...ss.map((st) => s.g.dist(st.tile, y.tile))))[0];
  const tc = threatCamp(s, c);
  const near = tc && ss.some((st) => s.g.dist(st.tile, tc.tile) <= 11) ? tc : undefined;
  const cp = c.threat >= 0.35 ? tc : occ ?? (hunt ? tc : near);
  if (!cp) return;
  const motive = c.threat >= 0.35 ? 'Canavar baskınları dayanılmaz hâle geldi' : cp === occ ? `${cp.name} topraklarındaki bir yatağı işgal ediyor` : cp === near && !hunt ? `${cp.name} sınıra fazla yakın` : 'Korucular avlanacak canavar arıyor';
  const home = s.civHeroes(c).filter((h) => h.state === 'home' && h.hp > h.maxHp * 0.7);
  const cap = s.capital(c)!;
  if (!s.w.agents.some((a) => a.civ === c.id && a.purpose === 'expedition')) {
    const soldiers = Math.floor(s.civSettlements(c).reduce((a, x) => a + x.soldiers, 0) * 0.6);
    const side: Combatant[] = [...home.map((h) => heroCombatant(h, 'A')), ...civTroops(s, c, soldiers, 'A')];
    const path0 = s.path(cap.tile, cp.tile);
    // aynı kampa yaklaşık aynı anda varacak dost gruplar (ilanı alan parti, av partisi) hesaba katılır
    const probe = { id: -1, kind: 'army', civ: c.id, path: path0 ?? [cap.tile], step: 0, progress: 0, speed: 0.6, heroes: home.map((h) => h.id), to: cp.id, purpose: 'expedition' } as Agent;
    const allies = path0 ? probeAllies(s, probe, etaDays(s, probe), MUSTER_CAMP) : [];
    const allyCs = allies.flatMap((b) => agentCombatants(s, b, cp.kind));
    const [pa, pb] = powerVs([...side, ...allyCs], campForce(s, cp));
    if ((home.length || soldiers >= 6 || (allies.length && soldiers >= 3)) && pa >= pb * (hunt ? 0.8 : 0.95)) {
      const pop = drawSoldiers(s, c, soldiers);
      const path = path0;
      if (path) {
        for (const h of home) h.state = 'army';
        s.w.agents.push({ id: s.id(), kind: 'army', civ: c.id, path, step: 0, progress: 0, speed: 0.6, heroes: home.map((h) => h.id), troops: soldiers, pop, from: cap.id, to: cp.id, purpose: 'expedition' });
        s.log('quest', `${c.name}${home.length ? `, ${home.map((h) => h.name).join(' ve ')} önderliğinde` : ''} ${soldiers} askerle ${ek(cp.name, 'a')} sefer başlattı.`, { civ: c.id, tile: cap.tile, cause: allies.length ? `${motive}; yoldaki ${allies.length} dost grupla birlikte saldıracak` : motive, major: true });
        if (allies.length) s.metric('jointPlanned');
        return;
      }
      s.mergePop(cap, pop);
    }
  }
  const open = s.w.quests.find((q) => q.open && q.civ === c.id && q.camp === cp.id);
  if (open || s.w.quests.some((q) => !q.open && q.camp === cp.id && q.civ === c.id && q.takenBy.length && s.w.agents.some((a) => a.quest === q.id && !a.dead))) return;
  if (s.st(c, 'gold') < 25) return;
  if (c.threat < 0.35 && cp !== occ && (cp !== near || s.st(c, 'gold') < 60)) return;
  const kindF = cp.kind === 'hobgoblin' ? 1.5 : cp.kind === 'bugbear' || cp.kind === 'pirate' ? 2 : 1;
  const bounty = Math.round(Math.min(s.st(c, 'gold') * 0.6, (30 + c.threat * 50) * kindF + (cp === occ ? 15 : 0)));
  s.add(c, 'gold', -bounty);
  const inn = s.w.inns.filter((i) => i.alive).sort((a, b) => s.g.dist(a.tile, cp.tile) - s.g.dist(b.tile, cp.tile))[0];
  const q: Quest = { id: s.id(), civ: c.id, camp: cp.id, bounty, posted: s.day, takenBy: [], open: true, inn: inn && s.g.dist(inn.tile, cp.tile) <= 22 ? inn.id : undefined, expires: s.day + 3 * YEAR };
  s.w.quests.push(q);
  s.metric('questPosted');
  const where = q.inn !== undefined ? `${innById(s, q.inn)!.name} Hanı'nın panosuna` : 'tavernalara';
  s.log('quest', `${c.name} ${where} ilan astı: "${cp.name} temizlensin, ödül ${bounty} altın."`, { civ: c.id, tile: cap.tile, cause: motive, major: true });
}

/** başarısız sefer: ilan açık kalır, ödül %25 artar */
export function questFailed(s: Sim, q: Quest) {
  q.open = true; q.takenBy = [];
  q.failures = (q.failures ?? 0) + 1;
  const add = Math.round(q.bounty * 0.25);
  if (q.civ >= 0) { const c = s.w.civs[q.civ]; const pay = Math.min(add, Math.floor(s.st(c, 'gold'))); s.add(c, 'gold', -pay); q.bounty += pay; }
  else { const i = innById(s, q.inn); if (i) { const pay = Math.min(add, Math.floor(i.gold)); i.gold -= pay; q.bounty += pay; } }
}

export function heroesTick(s: Sim) {
  const w = s.w;
  for (const h of w.heroes) {
    if (h.state === 'dead' || h.state === 'gone' || h.state === 'retired') continue;
    if (h.state === 'tavern' || h.state === 'home') h.hp = Math.min(h.maxHp, h.hp + Math.ceil(h.maxHp * 0.1));
  }
  willTick(s);
  for (const h of w.heroes) {
    if (h.civ !== -1 || h.state !== 'tavern') continue;
    // yaşlı ve ünlü serbest kahraman emekli olabilir
    if ((s.day + h.id * 5) % YEAR === 0 && maybeRetire(s, h)) continue;
    // uzun süre iş bulamayan başka yere göçer ya da diyarı terk eder
    const idle = s.day - Math.max(h.idleSince, h.lastGoal ?? 0);
    if (idle > 2 * YEAR && !h.auction?.bids.length && s.rng.chance(0.1)) {
      const others = w.settlements.filter((x) => x.alive && (x.civics.tavern ?? 0) && x.id !== h.base);
      const inns = w.inns.filter((i) => i.alive && i.id !== h.base);
      if ((others.length || inns.length) && s.rng.chance(0.5)) {
        const pickInn = inns.length && (!others.length || s.rng.chance(0.5));
        const t = pickInn ? s.rng.pick(inns) : s.rng.pick(others);
        h.idleSince = s.day; h.base = t.id; h.baseInn = !!pickInn; h.auction = undefined;
        s.log('hero', `${h.name}, iş bulamayınca ${pickInn ? `${(t as { name: string }).name} Hanı'na` : `${(t as { name: string }).name} tavernasına`} doğru yola çıktı.`, { tile: h.pos });
        returnToBase(s, h);
      } else {
        h.state = 'gone';
        s.log('hero', `${h.name} kimse onu tutmayınca uzak diyarlara gitti.`, { tile: h.pos });
      }
    }
  }
}

/** Ölen bağlı kahraman için Diriliş */
export function tryRevive(s: Sim, h: Hero): boolean {
  if (h.civ < 0 || h.revived) return false;
  const c = s.w.civs[h.civ];
  if (!s.e(c, 'revive') || c.yearly.revive === s.year) return false;
  c.yearly.revive = s.year;
  h.revived = true;
  h.hp = Math.ceil(h.maxHp / 2);
  h.state = 'home';
  const cap = s.capital(c);
  if (cap) h.pos = cap.tile;
  s.log('class', `${h.name} Yaşam Alanı rahiplerinin duasıyla dirildi!`, { civ: c.id, major: true });
  return true;
}
