// Goblin istilası: büyüyen, yayılan, üst kademeye evrilen kamplar.
import type { Sim } from './sim';
import { YEAR } from './sim';
import { ek } from './tr';
import { MONSTERS } from '../data/classes';
import { unit, type Combatant } from './combat';
import { GOBLIN_CAMP_NAMES, HOB_NAMES, BUGBEAR_NAMES, makeCamp, pickCampTile } from './worldgen';
import type { Camp, CampKind } from './types';
import { launchPirates, newCaptain } from './sea';

const CAP: Record<CampKind, number> = { goblin: 14, hobgoblin: 10, bugbear: 4, pirate: 14 };
const GROW: Record<CampKind, number> = { goblin: 0.035, hobgoblin: 0.028, bugbear: 0.008, pirate: 0.022 };

export function monsterSide(kind: CampKind, n: number, boss: boolean, side: 'A' | 'B'): Combatant[] {
  const cs: Combatant[] = [];
  const base = kind === 'goblin' ? MONSTERS.goblin : kind === 'hobgoblin' ? MONSTERS.hobgoblin : kind === 'pirate' ? MONSTERS.pirate : MONSTERS.bugbear;
  for (let i = 0; i < n; i++) { const u = unit(base, side, 'monster'); u.evil = true; cs.push(u); }
  if (boss && kind !== 'bugbear') { const u = unit(kind === 'goblin' ? MONSTERS.goblinBoss : kind === 'pirate' ? MONSTERS.pirateCaptain : MONSTERS.hobCaptain, side, 'boss'); u.evil = true; cs.push(u); }
  return cs;
}
export function monsterName(kind: CampKind, plural = true) { return kind === 'goblin' ? (plural ? 'Goblinler' : 'Goblin') : kind === 'hobgoblin' ? (plural ? 'Hobgoblinler' : 'Hobgoblin') : kind === 'pirate' ? (plural ? 'Korsanlar' : 'Korsan') : (plural ? 'Bugbearlar' : 'Bugbear'); }

export function campAway(s: Sim, c: Camp) { return s.w.agents.filter((a) => a.kind === 'raid' && a.from === c.id && !a.dead).reduce((n, a) => n + (a.troops ?? 0), 0); }

export function campsTick(s: Sim) {
  const w = s.w;
  const alive = w.camps.filter((c) => c.alive);
  for (const c of alive) {
    const away = campAway(s, c);
    c.growthAcc += GROW[c.kind] + Math.min(0.02, c.loot / 3000);
    if (c.growthAcc >= 1 && c.count + away < CAP[c.kind]) { c.growthAcc = 0; c.count++; }
    const bossAway = w.agents.some((a) => a.kind === 'raid' && a.from === c.id && a.boss);
    if (c.kind === 'goblin' && !c.boss && !bossAway && !c.hadBoss && c.count >= 11) {
      c.hadBoss = true; c.boss = true;
      s.log('lair', `${ek(c.name, 'da')} goblinlerin başına acımasız bir şef geçti.`, { tile: c.tile, cause: `Kamp kalabalıklaştı (${c.count} goblin)`, major: true });
    }
    // yayılma
    if (c.kind === 'goblin' && c.count >= 12 && w.camps.filter((x) => x.alive).length < 10 && s.rng.chance(1 / 200)) {
      const avoid = [...w.settlements.filter((x) => x.alive).map((x) => x.tile), ...alive.map((x) => x.tile)];
      const near = s.g.within(c.tile, 10).filter((t) => s.g.dist(t, c.tile) >= 6);
      const t = near.length ? pickNear(s, near, avoid) : -1;
      if (t >= 0) {
        const nc = makeCamp(s.id(), 'goblin', t, freshName(s, GOBLIN_CAMP_NAMES), s.day, s.rng);
        nc.nextRaid = s.day + s.rng.int(100, 180);
        c.count -= 5; nc.count = 5;
        w.camps.push(nc); w.tiles[t].camp = nc.id; w.tiles[t].owner = -1;
        s.metric('campSpread');
        s.log('lair', `${c.name} kalabalıklaştı; goblinler ${nc.name} adıyla yeni bir kamp kurdu.`, { tile: t, major: true, cause: 'Goblin istilası yayılıyor' });
      }
    }
    // hobgoblin karakoluna evrilme
    const goblinCamps = alive.filter((x) => x.kind === 'goblin').length;
    const hobs = alive.filter((x) => x.kind === 'hobgoblin').length;
    if (c.kind === 'goblin' && hobs < 3 && s.day > 4 * YEAR && (s.day - c.founded > 3 * YEAR || goblinCamps >= 4) && c.count >= 10 && s.rng.chance(1 / 500)) {
      c.kind = 'hobgoblin';
      c.name = freshName(s, HOB_NAMES);
      c.count = Math.max(6, Math.floor(c.count * 0.6));
      c.boss = true; c.hadBoss = true;
      s.metric('hobgoblin');
      s.log('lair', `Hobgoblin lejyonerleri kampı ele geçirdi: ${c.name} kuruldu.`, { tile: c.tile, major: true, cause: 'Uzun süre temizlenmeyen goblin kampı disiplinli bir orduya dönüştü' });
    }
    // hobgoblin işgali: terk edilmiş yerleşim ya da sahipsiz yatak
    if (c.kind === 'hobgoblin' && c.count >= 10 && hobs < 4 && w.camps.filter((x) => x.alive).length < 7 && s.rng.chance(1 / 300)) {
      const cand = [
        ...w.settlements.filter((x) => !x.alive && s.g.dist(x.tile, c.tile) <= 12 && w.tiles[x.tile].owner < 0 && w.tiles[x.tile].camp === undefined && w.tiles[x.tile].innZone === undefined).map((x) => x.tile),
        ...w.deposits.filter((d) => !d.depleted).flatMap((d) => d.tiles).filter((t) => s.g.dist(t, c.tile) <= 10 && s.g.dist(t, c.tile) >= 4 && w.tiles[t].owner < 0 && w.tiles[t].camp === undefined && w.tiles[t].innZone === undefined && w.tiles[t].terrain !== 'mountain'),
      ];
      if (cand.length) {
        const t = s.rng.pick(cand);
        const nc = makeCamp(s.id(), 'hobgoblin', t, freshName(s, HOB_NAMES), s.day, s.rng);
        c.count -= 5; nc.count = 5;
        w.camps.push(nc); w.tiles[t].camp = nc.id;
        s.metric('occupation');
        s.log('lair', `Hobgoblinler ${w.tiles[t].deposit >= 0 ? 'sahipsiz bir yatağı' : 'terk edilmiş bir yerleşimi'} işgal etti.`, { tile: t, major: true, cause: `${c.name} genişliyor` });
      }
    }
    if (c.kind === 'pirate') {
      if (!c.boss && !bossAway && c.count >= 9 && s.rng.chance(1 / 120)) { c.boss = true; c.captain = newCaptain(s); s.log('lair', `${ek(c.name, 'da')} korsanlar yeni kaptanlarını seçti: ${c.captain}.`, { tile: c.tile, major: true, cause: `Koy kalabalıklaştı (${c.count} korsan)` }); }
      if (s.day >= c.nextRaid && c.count >= 5) launchPirates(s, c);
      continue;
    }
    if (s.day >= c.nextRaid && c.count >= (c.kind === 'goblin' ? 7 : c.kind === 'hobgoblin' ? 6 : 2)) launchRaid(s, c);
  }
  // bugbear ini
  if (s.year >= 4 && !alive.some((c) => c.kind === 'bugbear') && s.rng.chance(1 / 500)) {
    const forest = w.tiles.map((t, i) => (t.terrain === 'forest' || t.terrain === 'oldforest') && t.owner < 0 && t.camp === undefined && t.innZone === undefined && !t.isle ? i : -1).filter((i) => i >= 0);
    const avoid = w.settlements.filter((x) => x.alive).map((x) => x.tile);
    const t = pickNear(s, forest, avoid, 9);
    if (t >= 0) {
      const c = makeCamp(s.id(), 'bugbear', t, freshName(s, BUGBEAR_NAMES), s.day, s.rng);
      w.camps.push(c); w.tiles[t].camp = c.id;
      s.log('lair', `Ormanın derinliklerinde bir bugbear ini belirdi: ${c.name}.`, { tile: t, major: true, cause: 'Yolcuları ve kahramanları pusuya düşürürler' });
    }
  }
  // her şey temizlendiyse yeni kabile
  if (alive.length < 2 && s.day - Math.max(-9999, ...w.camps.map((c) => c.clearedDay ?? -9999)) > YEAR * (alive.length ? 2 : 1) && s.rng.chance(1 / 150)) {
    const avoid = w.settlements.filter((x) => x.alive).map((x) => x.tile);
    let t = pickCampTile(s.g, w.tiles, s.rng, avoid, 10);
    if (t < 0) t = pickCampTile(s.g, w.tiles, s.rng, avoid, 7); // kalabalık dünyada kıyıda köşede de olsa yer bulurlar
    if (t >= 0) {
      const c = makeCamp(s.id(), 'goblin', t, freshName(s, GOBLIN_CAMP_NAMES), s.day, s.rng);
      w.camps.push(c); w.tiles[t].camp = c.id;
      s.log('lair', `Dağlardan yeni bir goblin kabilesi indi ve ${ek(c.name, 'i')} kurdu.`, { tile: t, major: true, cause: 'Boşalan topraklar yeni yağmacıları çekti' });
    }
  }
}

function pickNear(s: Sim, cands: number[], avoid: number[], minD = 7): number {
  let best = -1, bs = -Infinity;
  for (const i of cands) {
    const t = s.w.tiles[i];
    if (t.terrain === 'water' || t.terrain === 'mountain' || t.owner >= 0 || t.camp !== undefined || t.isle || t.innZone !== undefined) continue;
    const d = avoid.length ? Math.min(...avoid.map((a) => s.g.dist(a, i))) : 20;
    if (d < minD) continue;
    const sc = Math.min(d, 12) + s.rng.next() * 3;
    if (sc > bs) { bs = sc; best = i; }
  }
  return best;
}
function freshName(s: Sim, list: string[]) {
  const used = new Set(s.w.camps.map((c) => c.name));
  const free = list.find((n) => !used.has(n));
  if (free) return free;
  const roman = ['II', 'III', 'IV', 'V', 'VI', 'VII', 'VIII'];
  for (const r of roman) for (const n of list) if (!used.has(`${n} ${r}`)) return `${n} ${r}`;
  return `${list[0]} ${s.w.camps.length}`;
}

export function launchRaid(s: Sim, c: Camp) {
  const w = s.w;
  c.nextRaid = s.day + s.rng.int(c.kind === 'hobgoblin' ? 110 : 90, 170);
  const range = c.kind === 'bugbear' ? 9 : 16;
  const caravans = w.agents.filter((a) => (a.kind === 'caravan' || (c.kind === 'bugbear' && (a.kind === 'party' || a.kind === 'scout' || a.kind === 'hero'))) && !a.dead && a.hull === undefined && !a.path.slice(a.step, a.step + 4).some((t) => w.tiles[t].sea) && s.g.dist(a.path[Math.min(a.step, a.path.length - 1)], c.tile) <= range - 3);
  const setts = c.kind === 'bugbear' ? [] : w.settlements.filter((x) => x.alive && s.g.dist(x.tile, c.tile) <= range && s.pop(x) >= 10);
  let targetTile = -1, to = -1, purpose = '';
  // korumasız çıkarma yapıları: iskele, tarla, maden, av kampı...
  const exts = c.kind === 'bugbear' ? [] : w.tiles.map((t, i) => (t.ext && s.extWorking(t) && t.owner >= 0 && s.g.dist(i, c.tile) <= range - 2 ? i : -1)).filter((i) => i >= 0)
    .filter((i) => { const st = s.settlement(w.tiles[i].owner); return !!st && st.alive && s.g.dist(i, st.tile) >= 1 && s.e(w.civs[st.civ], 'pact') <= 0; });
  const pickExt = exts.length && (!setts.length || s.rng.chance(0.55));
  const inns = c.kind === 'bugbear' ? [] : w.inns.filter((i) => i.alive && s.g.dist(i.tile, c.tile) <= range - 2);
  if (inns.length && s.rng.chance(0.2)) {
    const inn = inns.sort((a, b) => s.g.dist(a.tile, c.tile) - s.g.dist(b.tile, c.tile))[0];
    targetTile = inn.tile; to = inn.id; purpose = 'inn';
  } else if (pickExt && !(caravans.length && s.rng.chance(0.4))) {
    const t = s.rng.weighted(exts, (i) => { const st = s.settlement(w.tiles[i].owner)!; return (1 + s.g.dist(i, st.tile)) / (1 + st.soldiers * 0.5) / (1 + s.g.dist(i, c.tile) * 0.15); })!;
    targetTile = t; to = t; purpose = 'ext';
  } else if (caravans.length && (s.rng.chance(0.65) || !setts.length)) {
    const cv = s.rng.pick(caravans);
    targetTile = cv.path[Math.min(cv.path.length - 1, cv.step + 3)]; to = cv.id; purpose = 'caravan';
  } else if (setts.length) {
    const st = s.rng.weighted(setts, (x) => (s.e(s.w.civs[x.civ], 'pact') > 0 ? 0.25 : 1) / (1 + x.soldiers * 3 + s.pop(x) * 0.4 + (x.civics.palisade ? 6 : 0) + (x.civics.stonewall ? 12 : 0)) / (1 + s.g.dist(x.tile, c.tile) * 0.1))!;
    targetTile = st.tile; to = st.id; purpose = 'settlement';
  }
  if (targetTile < 0) return;
  const n = c.kind === 'bugbear' ? c.count : Math.min(Math.ceil(c.count * 0.5), 3 + Math.floor(s.year / 2));
  const boss = c.boss && c.count >= 10 && s.rng.chance(0.5);
  const path = s.path(c.tile, targetTile);
  if (!path) return;
  c.count -= n;
  if (boss) c.boss = false;
  s.w.agents.push({ id: s.id(), kind: 'raid', civ: -1, path, step: 0, progress: 0, speed: c.kind === 'bugbear' ? 1.1 : 0.9, troops: n, from: c.id, to, purpose, boss, monster: c.kind, targetTile });
}
