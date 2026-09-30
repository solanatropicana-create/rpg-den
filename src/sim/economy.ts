// Aktif ekonomi: değer tabanlı iş gücü, çıkarma/işleme, tükenme, tüketim, büyüme, inşaat kararları.
import type { Sim } from './sim';
import { FOOD_PER_POP, TIER_SLOTS, TIER_CROWD, YEAR } from './sim';
import {
  GOODS, GOOD_IDS, DEPOSITS, EXTRACTS, LEVEL_SLOTS, LEVEL_MULT, LEVEL_COST, WORKSHOPS, WORKSHOP_IDS, CIVICS,
  FOREST_REGROW_DAYS, extPoss, type Good, type Stock, type ExtractKind, type WorkshopKind, type CivicKind,
} from '../data/goods';
import { CLASSES, RACES, WONDERS, type RaceId } from '../data/classes';
import { ek } from './tr';
import { TECH, techCost, type TechDef } from '../data/techs';
import type { Civ, Settlement, Tile } from './types';
import { onTechDone } from './research';

const SEASON_FARM = [1.0, 1.3, 1.1, 0.25];
const SEASON_WILD = [1.0, 1.2, 1.1, 0.5];
const HOUSING: CivicKind[] = ['hut', 'house', 'stonehouse'];

export function extractGood(t: Tile, s: Sim): Good {
  const k = t.ext!.kind;
  if (t.deposit >= 0 && ['farm', 'mine', 'claypit', 'pasture', 'herbalist', 'crystal', 'mithril', 'grove'].includes(k)) {
    const d = s.w.deposits.find((x) => x.id === t.deposit)!;
    return DEPOSITS[d.kind].good;
  }
  return k === 'lumber' ? 'wood' : k === 'hunt' ? 'meat' : k === 'dock' ? 'fish' : 'stone';
}

/** Hex'teki yapının işçi başına günlük verimi */
export function extractYield(s: Sim, c: Civ, t: Tile, level = t.ext!.level, kind: ExtractKind = t.ext!.kind): { good: Good; y: number } {
  let rate = 0.1, good: Good = 'stone';
  let richness = 1;
  if (t.deposit >= 0 && kind !== 'lumber' && kind !== 'hunt' && kind !== 'quarry' && kind !== 'dock') {
    const d = s.w.deposits.find((x) => x.id === t.deposit)!;
    rate = DEPOSITS[d.kind].rate; good = DEPOSITS[d.kind].good; richness = d.richness;
  } else if (kind === 'lumber') { rate = 0.25; good = 'wood'; if (c.cls === 'druid') rate *= 0.4; }
  else if (kind === 'hunt') { rate = 0.17; good = 'meat'; }
  else if (kind === 'dock') { rate = 0.26; good = 'fish'; }
  else if (kind === 'quarry') { rate = 0.2; good = 'stone'; }
  let m = 1 + s.e(c, 'prodAll');
  const food = good === 'grain' || good === 'meat' || good === 'fish';
  if (food) m += s.e(c, 'prodFood');
  if (good === 'wood') m += s.e(c, 'prodWood');
  if (kind === 'mine' || kind === 'mithril' || kind === 'quarry') m += s.e(c, 'prodMine');
  if (good === 'herbs') m += s.e(c, 'prodHerbs');
  if (good === 'mana') m += s.e(c, 'prodMana');
  let season = 1;
  const winterImmune = s.e(c, 'winterImmune') > 0;
  if (kind === 'farm') season = winterImmune ? Math.max(1, SEASON_FARM[s.season]) : SEASON_FARM[s.season];
  else if (kind === 'hunt' || kind === 'dock' || kind === 'herbalist') season = winterImmune ? 1 : SEASON_WILD[s.season];
  let toolF = 1;
  if (level >= 2 && s.st(c, 'tools') < 0.5) toolF = 0.75;
  return { good, y: rate * LEVEL_MULT[level] * richness * m * season * toolF };
}

function demand(s: Sim, c: Civ, g: Good, P: number): number {
  const w = c.want[g] ?? 0;
  switch (g) {
    case 'wood': return 30 + P * 1.5 + w;
    case 'stone': return 15 + P * 0.8 + w;
    case 'clay': return (s.has(c, 'pottery') ? 12 : 2) + w;
    case 'planks': return (s.has(c, 'woodwork') ? 10 + P * 0.2 : 0) + w;
    case 'bricks': return (s.has(c, 'pottery') ? 6 + P * 0.15 : 0) + w;
    case 'leather': return 5 + w;
    case 'tools': return (s.has(c, 'bronzetools') ? 4 + P * 0.05 : 0) + w;
    case 'arms': return w;
    case 'beer': return P * 0.3 + w;
    case 'salt': return 3 + P * 0.05 + w;
    case 'gold': return 1e9;
    default: return 3 + w;
  }
}

export function updatePrices(s: Sim, c: Civ) {
  const P = s.civPop(c);
  const foodNeed = P * FOOD_PER_POP * 70;
  const foodHave = s.foodTotal(c);
  const fr = Math.min(6, Math.max(0.15, (foodNeed + 5) / (foodHave + 5)));
  const desires = new Set(CLASSES[c.cls].desires);
  for (const g of GOOD_IDS) {
    if (g === 'gold') { c.price.gold = 1; continue; }
    let p: number;
    if (GOODS[g].food) p = GOODS[g].base * fr;
    else {
      const D = demand(s, c, g, P);
      p = GOODS[g].base * Math.min(5, Math.max(0.05, (D + 3) / (s.st(c, g) + 3)));
    }
    if (desires.has(g)) p *= 1.3;
    c.price[g] = p;
  }
}

interface JobOpt { key: string; slots: number; v: number; run: (n: number) => void }

/** yanan yapıların ve evlerin onarımı: süre dolunca odun harcanarak yeniden kurulur */
export function repairTick(s: Sim, c: Civ) {
  for (const st of s.civSettlements(c)) {
    if ((st.burnedHouses ?? 0) > 0 && s.day - (st.burnedAt ?? 0) > 20 && s.st(c, 'wood') >= 4) {
      s.add(c, 'wood', -4); st.burnedHouses = (st.burnedHouses ?? 0) - 1; st.burnedAt = s.day - 10;
    }
  }
  for (const t of s.w.tiles) {
    const e = t.ext;
    if (!e?.burned || s.day < e.burned || t.owner < 0 || s.settlement(t.owner)?.civ !== c.id) continue;
    const need = 6 + e.level * 4;
    if (s.st(c, 'wood') < need) { e.burned = s.day + 10; continue; }
    s.add(c, 'wood', -need);
    e.burned = undefined; e.burnedAt = undefined;
    const st = s.settlement(t.owner)!;
    s.log('build', `${ek(st.name, 'in')} yanan ${extPoss(e.kind, e.level)} yeniden kuruldu.`, { civ: c.id, tile: s.w.tiles.indexOf(t) });
  }
}
export function economyTick(s: Sim, c: Civ) {
  const ss = s.civSettlements(c);
  if (!ss.length) { s.extinct(c); return; }
  if (s.day % 5 === 0 || !c.price.grain) updatePrices(s, c);
  const totalPop = ss.reduce((a, x) => a + s.pop(x), 0);
  const daysFood = s.foodTotal(c) / Math.max(0.1, totalPop * FOOD_PER_POP);
  const foodUrg = daysFood < 20 ? 4 : daysFood < 45 ? 1.8 : daysFood > 180 ? 0.6 : 1;
  const W = s.w;
  let researchPts = 0;
  const cls = CLASSES[c.cls];
  const resShare = Math.max(0.08, 0.17 + (c.cls === 'wizard' ? 0.07 : 0) + (c.cls === 'barbarian' ? -0.05 : 0));

  for (const st of ss) {
    const P = s.pop(st);
    if (P <= 0) { s.abandon(st, 'Yerleşimde kimse kalmadı'); continue; }
    const jobs: Record<string, number> = {};
    const famine = daysFood < 12;
    // kıtlıkta askerler de toplayıcılığa çıkar, araştırma durur
    let avail = famine ? P : Math.max(0, P - st.soldiers);
    jobs['asker'] = famine ? 0 : st.soldiers;
    // araştırmacılar önce ayrılır
    const nr = c.research.current && !famine ? Math.min(avail, Math.max(avail >= 3 ? 1 : 0, Math.round(avail * resShare))) : 0;
    avail -= nr; jobs['araştırmacı'] = nr;

    const opts: JobOpt[] = [];
    // çıkarma yapıları
    const myTiles: number[] = [];
    let forestGrass = 0;
    for (const ti of s.g.within(st.tile, s.radiusOf(st))) {
      const t = W.tiles[ti];
      if (t.owner !== st.id) continue;
      myTiles.push(ti);
      if (t.terrain === 'forest' || t.terrain === 'grass' || t.terrain === 'tundra' || t.terrain === 'oldforest' || t.terrain === 'hill' || t.terrain === 'swamp') forestGrass++;
      if (!t.ext || !s.extWorking(t)) { if (t.ext) t.ext.workers = 0; continue; }
      t.ext.workers = 0;
      const { good, y } = extractYield(s, c, t);
      let v = y * s.price(c, good);
      if (t.ext.kind === 'hunt') v += y * 0.25 * s.price(c, 'leather') + (t.terrain === 'tundra' ? y * 0.2 * s.price(c, 'fur') : 0);
      if (GOODS[good].food) v *= foodUrg;
      if (s.st(c, good) > 150 + totalPop * 12) v *= 0.08; // depolar dolu
      const tile = t;
      opts.push({ key: good, slots: LEVEL_SLOTS[t.ext.level], v, run: (n) => { tile.ext!.workers = n; produceTile(s, c, ti, tile, good, y * n); } });
    }
    // toplayıcılar
    const gy = 0.14 * SEASON_WILD[s.season] * (1 + s.e(c, 'prodFood') * 0.5);
    opts.push({ key: 'toplayıcı', slots: Math.min(10, forestGrass), v: gy * s.price(c, 'grain') * foodUrg * 0.9 + 0.03 * s.price(c, 'wood'), run: (n) => { s.add(c, 'grain', gy * n); s.add(c, 'wood', 0.03 * n); } });
    // atölyeler
    for (const k of WORKSHOP_IDS) {
      const cnt = st.workshops[k] ?? 0;
      if (!cnt) continue;
      const def = WORKSHOPS[k];
      const inp = pickInputs(s, c, def.inputs, def.alt);
      if (!inp) continue;
      const margin = (s.price(c, def.output) - s.worth(c, inp)) * (s.st(c, def.output) > 150 + totalPop * 12 ? 0.08 : 1);
      const perW = def.rate * (1 + s.e(c, 'prodAll') * 0.5);
      opts.push({ key: def.name, slots: def.slots * cnt, v: perW * Math.max(0.05, margin), run: (n) => {
        let out = perW * n;
        for (const g in inp) out = Math.min(out, s.st(c, g as Good) / (inp[g as Good] ?? 1));
        if (out <= 0) return;
        for (const g in inp) s.add(c, g as Good, -(inp[g as Good] ?? 0) * out);
        s.add(c, def.output, out);
      } });
    }
    // inşaatçılar
    if (st.project) opts.push({ key: 'inşaatçı', slots: Math.min(8, 2 + Math.floor(P / 6)), v: daysFood < 15 ? 0.3 : daysFood < 30 ? 1.2 : 2.5, run: (n) => {
      st.project!.left -= n * (s.st(c, 'tools') > 1 ? 1.25 : 1);
      if (st.project!.left <= 0) finishProject(s, c, st);
    } });
    opts.sort((a, b) => b.v - a.v);
    for (const o of opts) {
      if (avail <= 0) break;
      if (o.v < 0.035 && o.key !== 'inşaatçı') continue;
      const n = Math.min(avail, o.slots);
      if (n <= 0) continue;
      avail -= n;
      jobs[o.key] = (jobs[o.key] ?? 0) + n;
      o.run(n);
    }
    jobs['zanaatçı'] = avail;
    s.add(c, 'gold', avail * 0.02);
    st.jobs = jobs;
    // araştırma puanı (ırk yatkınlığı yok; sınıf etkileri)
    if (nr && c.research.current) {
      const lib = (st.civics.library ?? 0) > 0 ? 0.2 : 0;
      researchPts += nr * 0.3 * (1 + s.e(c, 'research') + lib);
    }
  }

  // Tüketim
  let need = totalPop * FOOD_PER_POP * (1 - Math.min(0.5, s.e(c, 'frugal')));
  for (const g of ['bread', 'fish', 'meat', 'grain'] as Good[]) {
    const fv = GOODS[g].food!;
    const have = s.st(c, g) * fv;
    const use = Math.min(have, need);
    s.add(c, g, -use / fv);
    need -= use;
    if (need <= 1e-6) break;
  }
  const salted = s.st(c, 'salt') >= 1;
  if (salted) s.add(c, 'salt', -totalPop * 0.0004);
  const spoil = salted ? 0.0012 : 0.0025;
  for (const g of ['grain', 'meat', 'fish', 'bread'] as Good[]) s.add(c, g, -s.st(c, g) * spoil);
  // depo sınırı: kapasiteyi aşan mal yavaşça çürür, çalınır ya da dağılır
  const capStore = 150 + totalPop * 12;
  for (const g in c.stock) {
    const v = c.stock[g as Good] ?? 0;
    if (v > capStore && g !== 'gold') s.add(c, g as Good, -(v - capStore) * 0.01);
  }
  let happy = 0;
  if (s.st(c, 'beer') > totalPop * 0.01) { s.add(c, 'beer', -totalPop * 0.004); happy = 0.15; }
  let l2 = 0;
  for (let i = 0; i < W.tiles.length; i++) { const t = W.tiles[i]; if (t.ext && t.ext.level >= 2 && t.owner >= 0 && ss.some((x) => x.id === t.owner)) l2++; }
  if (l2) s.add(c, 'tools', -l2 * 0.0015);
  const soldiers = ss.reduce((a, x) => a + x.soldiers, 0);
  s.add(c, 'gold', totalPop * 0.004 * (1 + s.e(c, 'tax')) + (ss.some((x) => x.civics.mint) ? 0.12 : 0) - soldiers * 0.006);

  if (need > 0.01) {
    if (s.e(c, 'noFamine') > 0) { s.add(c, 'grain', need); }
    else for (const st of s.civSettlements(c)) {
      st.starving++;
      if (st.starving === 1) s.log('economy', `${st.name}'da kıtlık başladı.`, { civ: c.id, tile: st.tile, cause: `Gıda stoğu tükendi (${s.dateStr()})` });
      // açlık, açığın büyüklüğüyle orantılı birikir
      st.hunger = (st.hunger ?? 0) + Math.min(1, need / Math.max(0.05, totalPop * FOOD_PER_POP));
      if (st.hunger >= 10) { st.hunger -= 10; s.removePop(st, 1); s.metric('starved'); }
    }
  } else {
    for (const st of s.civSettlements(c)) {
      st.starving = 0; st.hunger = 0;
      const P = s.pop(st);
      const cap = s.housing(st);
      if (P < cap && daysFood > 8) {
        let rate = 0;
        for (const r in st.pop) rate += (st.pop[r as RaceId] ?? 0) * RACES[r as RaceId].growth;
        const crowd = Math.max(0.05, 1 - P / TIER_CROWD[st.tier]) / (1 + Math.max(0, totalPop - 60) / 70);
        st.growthAcc += rate * 0.0058 * (daysFood > 30 ? 1 : 0.5) * crowd * (1 + s.e(c, 'growth') + happy);
        while (st.growthAcc >= 1) {
          st.growthAcc -= 1;
          const rs = (Object.keys(st.pop) as RaceId[]).filter((r) => (st.pop[r] ?? 0) > 0);
          s.addPop(st, s.rng.weighted(rs, (x) => (st.pop[x] ?? 0) * RACES[x].growth) ?? c.race, 1);
        }
      }
      if (s.rng.chance(P * 0.00025)) s.removePop(st, 1);
    }
  }

  // Araştırma
  if (c.research.current) {
    const t = TECH[c.research.current];
    c.research.progress += researchPts;
    const cost = researchCost(s, c, t);
    if (c.research.progress >= cost) {
      c.research.done.push(t.id);
      c.research.current = null;
      c.research.progress = 0;
      onTechDone(s, c, t.id);
    }
  }

  // Yenilenen yataklar ve orman
  if (s.day % 10 === 0) {
    for (const d of W.deposits) {
      const regen = DEPOSITS[d.kind].regen;
      if (!regen) continue;
      const max = DEPOSITS[d.kind].reserve;
      for (const ti of d.tiles) W.tiles[ti].reserve = Math.min(max, W.tiles[ti].reserve + regen * 10);
    }
  }
  const tp = s.civPop(c);
  for (const m of [25, 50, 100, 200]) if (tp >= m && c.stats.peakPop < m) s.log('growth', `${c.name} nüfusu ${m} kişiye ulaştı.`, { civ: c.id, major: m >= 50 });
  c.stats.peakPop = Math.max(c.stats.peakPop, tp);
}

/** Orman yeniden büyümesi (30 günde bir, dünya düzeyinde) */
export function regrowForests(s: Sim) {
  for (let i = 0; i < s.w.tiles.length; i++) {
    const t = s.w.tiles[i];
    if (t.cutDay === undefined || t.terrain !== 'grass' || t.ext || t.deposit >= 0) continue;
    const owner = s.tileCiv(i);
    const fast = owner >= 0 && s.has(s.w.civs[owner], 'forestry');
    if (s.day - t.cutDay > FOREST_REGROW_DAYS * (fast ? 0.5 : 1)) { t.terrain = 'forest'; t.wood = 160; t.cutDay = undefined; }
  }
}

function produceTile(s: Sim, c: Civ, ti: number, t: Tile, good: Good, amount: number) {
  if (amount <= 0) return;
  const kind = t.ext!.kind;
  if (kind === 'lumber') {
    if (c.cls !== 'druid') {
      t.wood -= amount;
      if (t.wood <= 0) {
        t.terrain = 'grass'; t.cutDay = s.day; t.ext = undefined; t.wood = 0;
        s.clearPaths();
      }
    }
    s.add(c, 'wood', amount);
    return;
  }
  if (kind === 'hunt') {
    s.add(c, 'meat', amount); s.add(c, 'leather', amount * 0.25);
    if (t.terrain === 'tundra') s.add(c, 'fur', amount * 0.2);
    return;
  }
  if (kind === 'dock' || kind === 'pasture' || kind === 'farm') { s.add(c, good, amount); return; }
  // tükenen rezervler
  const take = Math.min(amount, t.reserve);
  s.add(c, good, take);
  c.stats.mined[good] = (c.stats.mined[good] ?? 0) + take;
  t.reserve -= take;
  if (t.reserve <= 0.01) {
    t.reserve = 0;
    t.ext!.depleted = true;
    if (t.deposit >= 0) {
      const d = s.w.deposits.find((x) => x.id === t.deposit)!;
      if (!DEPOSITS[d.kind].regen && d.tiles.every((x) => s.w.tiles[x].reserve <= 0)) {
        d.depleted = true;
        c.stats.depleted++;
        s.metric('depleted');
        s.log('economy', `${c.name}, ${DEPOSITS[d.kind].name.toLocaleLowerCase('tr')} yatağını tüketti.`, { civ: c.id, tile: ti, major: true, cause: `${d.tiles.length} hex'lik yatak boşaldı; yeni kaynak aranacak` });
      } else if (DEPOSITS[d.kind].regen) {
        t.ext!.depleted = false; // yenilenir, beklemede
      }
    }
  }
}

function pickInputs(s: Sim, c: Civ, a: Stock, b?: Stock): Stock | null {
  const ok = (x: Stock) => (Object.keys(x) as Good[]).every((g) => s.st(c, g) >= (x[g] ?? 0));
  if (b && ok(b)) return b;
  if (ok(a)) return a;
  return null;
}

// ------------------------------------------------------------ projeler
export function finishProject(s: Sim, c: Civ, st: Settlement) {
  const p = st.project!;
  st.project = null;
  if (p.type === 'civic') {
    const k = p.kind as CivicKind;
    st.civics[k] = (st.civics[k] ?? 0) + 1;
    if (k === 'wonder') {
      s.recomputeEff(c);
      const first = !(s.w.metrics.wonder ?? 0);
      s.metric('wonder');
      const wd = WONDERS[c.cls];
      for (const o of s.w.civs) if (o.alive && o.id !== c.id && s.rel(o.id, c.id).contact) s.addMod(o.id, c.id, 'wonder', `${wd.name} hayranlığı`, first ? 12 : 6, 12, 0.01, false);
      s.log('wonder', first ? `DÜNYANIN İLK HARİKASI: ${c.name}, ${ek(st.name, 'da')} ${wd.name} inşasını tamamladı!` : `${c.name}, ${ek(st.name, 'da')} ${wd.name} inşasını tamamladı.`, { civ: c.id, tile: st.tile, major: true, cause: `${wd.desc}${first ? '; bütün diyar hayranlıkla izliyor' : ''}` });
      return;
    }
    if (!HOUSING.includes(k) || (k === 'house' && st.civics.house === 1) || (k === 'stonehouse' && st.civics.stonehouse === 1))
      s.log('build', `${st.name}'da ${civicName(c, k)} yükseldi.`, { civ: c.id, tile: st.tile, major: k === 'tavern' || k === 'unique' || k === 'castle' });
  } else if (p.type === 'workshop') {
    const k = p.kind as WorkshopKind;
    st.workshops[k] = (st.workshops[k] ?? 0) + 1;
    if (st.workshops[k] === 1) s.log('build', `${st.name}'da ${WORKSHOPS[k].name} açıldı.`, { civ: c.id, tile: st.tile });
  } else if (p.type === 'extract') {
    const t = s.w.tiles[p.tile!];
    if (t.owner === st.id && !t.ext) {
      const kind = p.kind as ExtractKind;
      t.ext = { kind, level: p.level ?? 1, settlement: st.id, workers: 0 };
      if (kind === 'mine' || kind === 'crystal' || kind === 'mithril' || kind === 'grove') {
        const d = s.w.deposits.find((x) => x.id === t.deposit);
        const firstOfKind = d && !s.w.tiles.some((x, i) => i !== p.tile && x.ext && x.deposit >= 0 && s.tileCiv(i) === c.id && s.w.deposits.find((y) => y.id === x.deposit)?.kind === d.kind);
        if (d && firstOfKind) s.log('build', `${c.name} ${DEPOSITS[d.kind].name.toLocaleLowerCase('tr')} üzerine ${EXTRACTS[kind].names[(p.level ?? 1) - 1]} kurdu.`, { civ: c.id, tile: p.tile, major: DEPOSITS[d.kind].count <= 5 });
      }
    }
  } else if (p.type === 'upgrade') {
    const t = s.w.tiles[p.tile!];
    if (t.ext && t.owner === st.id) {
      t.ext.level = p.level!;
      s.metric('upgrade');
      const nm = EXTRACTS[t.ext.kind].names[t.ext.level - 1];
      s.log('build', `${st.name} yakınındaki ${EXTRACTS[t.ext.kind].names[t.ext.level - 2].toLocaleLowerCase('tr')} ${nm} seviyesine yükseltildi (L${t.ext.level}).`, { civ: c.id, tile: p.tile, major: t.ext.level === 3 });
    }
  }
}

export const UNIQUE_BUILDING: Record<string, string> = {
  paladin: 'Yemin Tapınağı', cleric: 'Tapınak Ocağı', druid: 'Kutsal Koru', rogue: 'Hırsızlar Loncası', wizard: 'Akademi', barbarian: 'Totem Direği',
  bard: 'Ozanlar Salonu', fighter: 'Lejyon Kışlası', monk: 'Manastır', ranger: 'Korucu Locası', sorcerer: 'Kan Soyu Mabedi', warlock: 'Pakt Mihrabı',
};
export function researchCost(s: Sim, c: Civ, t: TechDef) {
  let cost = techCost(t) * (c.research.hard && c.research.current === t.id ? 2.2 : 1);
  if (s.e(c, 'cheapKnown') && s.w.civs.some((o) => o.id !== c.id && o.alive && s.rel(c.id, o.id).contact && o.research.done.includes(t.id))) cost *= 0.5;
  return Math.round(cost);
}
export function civicName(c: Civ, k: CivicKind) { return k === 'unique' ? UNIQUE_BUILDING[c.cls] ?? 'Sınıf yapısı' : k === 'wonder' ? WONDERS[c.cls].name : CIVICS[k].name; }

interface Cand { score: number; cost: Stock; work: number; project: Omit<Settlement['project'] & object, 'left' | 'total'>; }

export function chooseBuilds(s: Sim, c: Civ) {
  c.want = {};
  const war = s.inWar(c);
  const cap = s.capital(c);
  for (const st of s.civSettlements(c)) {
    if (st.project) continue;
    const cands = buildCandidates(s, c, st, st.id === cap?.id, war);
    if (!cands.length) continue;
    cands.sort((a, b) => b.score - a.score);
    const best = cands[0];
    let chosen: Cand | undefined;
    for (const cd of cands) {
      if (cd.score < best.score * 0.2) break;
      if (s.canPay(c, cd.cost)) { chosen = cd; break; }
    }
    // karşılanamayan en iyi hedef talebe yansır
    if (!chosen || chosen !== best) for (const g in best.cost) c.want[g as Good] = (c.want[g as Good] ?? 0) + (best.cost[g as Good] ?? 0);
    if (!chosen) continue;
    s.pay(c, chosen.cost);
    st.project = { ...chosen.project, left: chosen.work, total: chosen.work } as Settlement['project'];
  }
}

function buildCandidates(s: Sim, c: Civ, st: Settlement, isCap: boolean, war: boolean): Cand[] {
  const W = s.w;
  const out: Cand[] = [];
  const P = s.pop(st);
  const cap = s.housing(st);
  const civicAvail = (k: CivicKind) => { const d = CIVICS[k]; return (!d.tech || s.has(c, d.tech)); };
  // Barınma
  if (P >= cap - 2) {
    for (const [k, sc] of [['stonehouse', 62], ['house', 56], ['hut', 50]] as [CivicKind, number][]) {
      if (!civicAvail(k)) continue;
      if (k === 'hut' && (st.civics.hut ?? 0) >= 6 && s.has(c, 'woodwork')) continue;
      out.push({ score: sc + (P >= cap ? 15 : 0) + Math.min(30, (P - cap) * 2.5 * (P > cap ? 1 : 0)), cost: CIVICS[k].cost, work: CIVICS[k].work, project: { type: 'civic', kind: k } });
    }
  }
  const slotsFree = TIER_SLOTS[st.tier] - s.slotsUsed(st);
  // Çıkarma yapıları
  let extSlots = 0;
  const tiles = s.g.within(st.tile, s.radiusOf(st)).filter((t) => W.tiles[t].owner === st.id);
  for (const ti of tiles) { const t = W.tiles[ti]; if (t.ext && !t.ext.depleted) extSlots += LEVEL_SLOTS[t.ext.level]; }
  const bestByKind = new Map<ExtractKind, Cand>();
  if (extSlots < P * 1.0 + 3) {
    for (const ti of tiles) {
      const t = W.tiles[ti];
      if (t.ext || ti === st.tile || t.camp !== undefined) continue;
      for (const kind of possibleKinds(s, c, t, ti)) {
        const lvl = EXTRACTS[kind].startLevel ?? 1;
        const fake = { ...t, ext: { kind, level: lvl, settlement: st.id, workers: 0 } } as Tile;
        const { good, y } = extractYield(s, c, fake, lvl, kind);
        let v = y * LEVEL_SLOTS[lvl] * s.price(c, good);
        if (kind === 'hunt') v += y * LEVEL_SLOTS[lvl] * 0.25 * s.price(c, 'leather');
        const score = v * 9 + (CLASSES[c.cls].desires.includes(good) ? 6 : 0);
        const cost = kind === 'lumber' || kind === 'hunt' ? {} : LEVEL_COST[lvl];
        const cand: Cand = { score, cost, work: 6 + lvl * 4, project: { type: 'extract', kind, tile: ti, level: lvl } };
        const prev = bestByKind.get(kind);
        if (!prev || prev.score < score) bestByKind.set(kind, cand);
      }
    }
    out.push(...bestByKind.values());
  }
  // Yükseltmeler
  for (const ti of tiles) {
    const t = W.tiles[ti];
    if (!t.ext || t.ext.depleted || t.ext.level >= 3) continue;
    const def = EXTRACTS[t.ext.kind];
    const nl = t.ext.level + 1;
    if (!def.names[nl - 1] || !def.tech[nl - 1] || !s.has(c, def.tech[nl - 1]!)) continue;
    if (t.ext.workers < LEVEL_SLOTS[t.ext.level] - 1) continue;
    const cur = extractYield(s, c, t);
    const nxt = extractYield(s, c, t, nl);
    const gain = (nxt.y * LEVEL_SLOTS[nl] - cur.y * t.ext.workers) * s.price(c, cur.good);
    out.push({ score: gain * 10 + 8, cost: LEVEL_COST[nl], work: 10 + nl * 8, project: { type: 'upgrade', kind: t.ext.kind, tile: ti, level: nl } });
  }
  if (slotsFree > 0) {
    // Atölyeler
    for (const k of WORKSHOP_IDS) {
      const def = WORKSHOPS[k];
      if (!s.has(c, def.tech)) continue;
      const have = st.workshops[k] ?? 0;
      if (have >= (st.tier >= 2 ? 2 : 1)) continue;
      const inputsOk = (Object.keys(def.inputs) as Good[]).every((g) => s.st(c, g) >= (def.inputs[g] ?? 0) * 4 || producing(s, c, g))
        || (def.alt && (Object.keys(def.alt) as Good[]).every((g) => s.st(c, g) >= (def.alt![g] ?? 0) * 4 || producing(s, c, g)));
      if (!inputsOk) continue;
      const inp = def.alt && (Object.keys(def.alt) as Good[]).every((g) => producing(s, c, g) || s.st(c, g) > 0) ? def.alt : def.inputs;
      const margin = s.price(c, def.output) - s.worth(c, inp);
      if (margin <= 0) continue;
      out.push({ score: margin * def.rate * def.slots * 9 + 5, cost: def.cost, work: 16, project: { type: 'workshop', kind: k } });
    }
    // Yerleşim yapıları
    const civ = (k: CivicKind, score: number) => {
      if (!civicAvail(k)) return;
      const d = CIVICS[k];
      if ((st.civics[k] ?? 0) >= (d.max ?? 99)) return;
      out.push({ score, cost: d.cost, work: d.work, project: { type: 'civic', kind: k } });
    };
    if (isCap) civ('tavern', 45);
    if (isCap || P > 25) civ('market', 30);
    civ('temple', 18 + (c.cls === 'cleric' || c.cls === 'paladin' ? 10 : 0));
    if (isCap) civ('library', 30 + (c.cls === 'wizard' ? 20 : 0));
    if (s.access(c, 'gold')) civ('mint', 28);
    if (isCap) civ('guild', 26);
    const threat = c.threat + (war ? 1 : 0);
    civ('palisade', 8 + threat * 35 + st.tier * 6);
    if (st.tier >= 1) civ('stonewall', 10 + threat * 30 + st.tier * 6);
    if (isCap && st.tier >= 2) civ('castle', 20 + threat * 25);
    if (isCap && c.era >= 4 && st.tier >= 2 && !st.civics.wonder && !s.civSettlements(c).some((x) => x.civics.wonder)) {
      const d = CIVICS.wonder;
      const rivals = s.w.settlements.filter((x) => x.alive && x.civ !== c.id && (x.civics.wonder || x.project?.kind === 'wonder')).length;
      out.push({ score: 44 + rivals * 6, cost: d.cost, work: d.work, project: { type: 'civic', kind: 'wonder' } });
    }
    if (isCap && c.subclass && !st.civics.unique) {
      const d = CIVICS.unique;
      out.push({ score: 40, cost: d.cost, work: d.work, project: { type: 'civic', kind: 'unique' } });
    }
  }
  return out;
}

function producing(s: Sim, c: Civ, g: Good) {
  for (const t of s.w.tiles) if (t.ext && !t.ext.depleted && t.ext.workers > 0 && t.owner >= 0 && s.settlement(t.owner)?.civ === c.id && extractGood(t, s) === g) return true;
  return false;
}

export function possibleKinds(s: Sim, c: Civ, t: Tile, ti: number): ExtractKind[] {
  const out: ExtractKind[] = [];
  const lv1 = (k: ExtractKind) => { const def = EXTRACTS[k]; const sl = def.startLevel ?? 1; const tech = def.tech[sl - 1]; return !tech || s.has(c, tech); };
  if (t.deposit >= 0) {
    const d = s.w.deposits.find((x) => x.id === t.deposit)!;
    if (!s.depositVisible(c, d) || d.depleted) return out;
    const kind = DEPOSITS[d.kind].building;
    if (kind === 'grove') { if (c.cls === 'druid' && s.has(c, 'druid_grove')) out.push('grove'); return out; }
    if (t.reserve <= 0 && DEPOSITS[d.kind].reserve > 0) return out;
    if (lv1(kind)) out.push(kind);
    return out;
  }
  if ((t.terrain === 'forest') && t.wood > 0) { out.push('lumber'); out.push('hunt'); }
  if (t.terrain === 'tundra') out.push('hunt');
  if (t.terrain === 'hill' && t.reserve > 0 && lv1('quarry')) out.push('quarry');
  if ((t.terrain === 'grass' || t.terrain === 'swamp') && lv1('dock') && s.g.neighbors(ti).some((n) => s.w.tiles[n].terrain === 'water')) out.push('dock');
  return out;
}

// ------------------------------------------------------------ askerler
export function recruitTick(s: Sim, c: Civ) {
  const war = s.inWar(c);
  const cls = CLASSES[c.cls];
  let ratio = 0;
  const tribal = c.cls === 'barbarian' && (s.has(c, 'barbarian_totem') || s.has(c, 'barbarian_raiders'));
  const unarmed = s.e(c, 'unarmed') > 0;
  if (s.has(c, 'training') || tribal || unarmed) ratio = (0.07 + cls.aggression * 0.07 + s.e(c, 'warband')) * (war ? 1.8 : 1) * (c.threat > 0.5 ? 1.3 : 1);
  let deficit = 0;
  for (const st of s.civSettlements(c)) {
    const target = Math.floor(s.pop(st) * ratio);
    if (st.soldiers < target) {
      const n = Math.min(2, target - st.soldiers);
      for (let i = 0; i < n; i++) {
        if (s.st(c, 'arms') >= 1) { s.add(c, 'arms', -1); st.soldiers++; }
        else if (tribal && s.st(c, 'leather') >= 2) { s.add(c, 'leather', -2); st.soldiers++; }
        else if (unarmed && s.st(c, 'grain') >= 4) { s.add(c, 'grain', -4); st.soldiers++; }
        // silah yoksa: mızrak ve deri zırhla hafif piyade
        else if (s.has(c, 'training') && s.st(c, 'leather') >= 2 && s.st(c, 'wood') >= 3) { s.add(c, 'leather', -2); s.add(c, 'wood', -3); st.soldiers++; }
        else deficit++;
      }
    } else if (st.soldiers > target + 2) st.soldiers--;
  }
  if (deficit) c.want.arms = (c.want.arms ?? 0) + deficit * 2;
  // Talim ilk kez açıldığında birkaç milis silahsız asker olur
  if (ratio > 0 && !c.yearly.firstTroops) {
    c.yearly.firstTroops = 1;
    const cap = s.capital(c);
    if (cap) cap.soldiers = Math.max(cap.soldiers, Math.min(3, Math.floor(s.pop(cap) * 0.1)));
  }
  void YEAR;
}
