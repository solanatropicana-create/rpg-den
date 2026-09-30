import { Rng } from './rng';
import { HexGrid, findPath } from './hex';
import { ek } from './tr';
import { CLASSES, RACES, WONDERS, type RaceId } from '../data/classes';
import { DEPOSITS, GOODS, MOVE_COST, CIVICS, type Good, type Stock } from '../data/goods';
import { TECH, ERA_TR, type Effects } from '../data/techs';
import type { Civ, Deposit, EventKind, GameEvent, Hero, Pop, Relation, Settlement, World } from './types';
import { generateWorld } from './worldgen';
import { economyTick, chooseBuilds, recruitTick, repairTick } from './economy';
import { disastersTick } from './events';
import { chooseResearch, eraCheck, classYearly } from './research';
import { relationsTick, worldTick, considerExpansion, considerTrade, considerWarAction, considerScout, considerRaid } from './diplomacy';
import { campsTick } from './monsters';
import { tavernsTick, heroesTick, considerHero, considerQuest } from './heroes';
import { innsTick, considerInnBids } from './inns';
import { returnToBase, heroLabel } from './will';
import { agentsTick, routesTick, roadsTick } from './agents';
import { seaTick } from './sea';

export const FOOD_PER_POP = 0.1;
export const YEAR = 120;
export const SEASONS = ['İlkbahar', 'Yaz', 'Sonbahar', 'Kış'];
export const TIER_TR = ['Kamp', 'Köy', 'Kasaba', 'Şehir'];
export const TIER_POP = [0, 12, 40, 100];
export const TIER_SLOTS = [4, 8, 14, 20];
export const TIER_CROWD = [24, 55, 120, 230];

export class Sim {
  w: World;
  g: HexGrid;
  rng: Rng;
  private pathCache = new Map<string, number[] | null>();
  /** kara + deniz yolları (liman ve teknolojiye göre anahtarlanır; yol yapımıyla temizlenmez) */
  navCache = new Map<string, number[] | null>();
  shoreW?: Uint8Array;
  onEvent?: (e: GameEvent) => void;

  constructor(seed: number) {
    this.w = generateWorld(seed);
    this.g = new HexGrid(this.w.W, this.w.H);
    this.rng = new Rng(this.w.rngState ^ 0x9e3779b9);
    this.updateTerritory();
    this.discover();
    for (const c of this.w.civs) { this.recomputeEff(c); chooseResearch(this, c); }
    this.log('world', `Dünya uyandı. ${this.w.civs.length} topluluk ilk kamplarını kurdu: ${this.w.civs.map((c) => `${c.name} (${RACES[c.race].plural}, ${CLASSES[c.cls].name})`).join(', ')}.`, { major: true });
  }

  // ------------------------------------------------------------ time & log
  get day() { return this.w.day; }
  get season() { return Math.floor((this.w.day % YEAR) / (YEAR / 4)); }
  get year() { return Math.floor(this.w.day / YEAR) + 1; }
  dateStr(d = this.w.day) { return `Yıl ${Math.floor(d / YEAR) + 1}, ${SEASONS[Math.floor((d % YEAR) / (YEAR / 4))]}`; }
  id() { return this.w.nextId++; }
  log(kind: EventKind, text: string, o: Partial<GameEvent> = {}) {
    const e: GameEvent = { id: this.id(), day: this.w.day, kind, text, ...o };
    this.w.events.push(e);
    if (this.w.events.length > 2500) this.w.events.splice(0, this.w.events.length - 2500);
    this.metric('ev_' + kind);
    this.onEvent?.(e);
    return e;
  }
  metric(k: string, v = 1) { this.w.metrics[k] = (this.w.metrics[k] ?? 0) + v; }

  // ------------------------------------------------------------ population
  pop(s: Settlement) { let t = 0; for (const k in s.pop) t += s.pop[k as RaceId] ?? 0; return t; }
  civSettlements(c: Civ | number) { const id = typeof c === 'number' ? c : c.id; return this.w.settlements.filter((s) => s.alive && s.civ === id); }
  civPop(c: Civ) { return this.civSettlements(c).reduce((a, s) => a + this.pop(s), 0); }
  capital(c: Civ) { return this.civSettlements(c).sort((a, b) => this.pop(b) - this.pop(a) || a.founded - b.founded)[0]; }
  addPop(s: Settlement, r: RaceId, n: number) { s.pop[r] = (s.pop[r] ?? 0) + n; }
  removePop(s: Settlement, n: number): Pop {
    const out: Pop = {};
    for (let i = 0; i < n; i++) {
      const rs = (Object.keys(s.pop) as RaceId[]).filter((r) => (s.pop[r] ?? 0) > 0);
      if (!rs.length) break;
      const r = this.rng.weighted(rs, (x) => s.pop[x] ?? 0)!;
      s.pop[r] = (s.pop[r] ?? 1) - 1; if (!s.pop[r]) delete s.pop[r];
      out[r] = (out[r] ?? 0) + 1;
    }
    return out;
  }
  mergePop(s: Settlement, p: Pop | undefined) { if (p) for (const r in p) this.addPop(s, r as RaceId, p[r as RaceId] ?? 0); }
  popSize(p?: Pop) { let t = 0; if (p) for (const k in p) t += p[k as RaceId] ?? 0; return t; }
  raceStr(p: Pop) { return (Object.keys(p) as RaceId[]).filter((r) => (p[r] ?? 0) > 0).map((r) => `${RACES[r].name} ${p[r]}`).join(', '); }
  /** konutu olmayan, sur dışında barakada yaşayanlar */
  homeless(s: Settlement) { return Math.max(0, this.pop(s) - this.housing(s)); }
  housing(s: Settlement) { let h = 4; for (const k of ['hut', 'house', 'stonehouse'] as const) h += (s.civics[k] ?? 0) * (CIVICS[k].housing ?? 0); return Math.max(4, h - (s.burnedHouses ?? 0) * 4); }
  /** yanmış ya da tükenmiş olmayan, çalışan çıkarma yapısı mı */
  extWorking(t: { ext?: { depleted?: boolean; burned?: number } }) { return !!t.ext && !t.ext.depleted && !t.ext.burned; }
  slotsUsed(s: Settlement) {
    let n = 0;
    for (const k in s.civics) if (!['hut', 'house', 'stonehouse', 'shipyard', 'lighthouse'].includes(k)) n += s.civics[k as keyof typeof s.civics] ?? 0;
    for (const k in s.workshops) n += s.workshops[k as keyof typeof s.workshops] ?? 0;
    return n;
  }
  tierOf(s: Settlement) {
    const c = this.w.civs[s.civ];
    const P = this.pop(s);
    let t = 0;
    for (let i = 1; i < 4; i++) if (P >= TIER_POP[i] && c.era >= i + 1) t = i;
    return t;
  }
  radiusOf(s: Settlement) { return 2 + s.tier; }

  // ------------------------------------------------------------ stock
  st(c: Civ, g: Good) { return c.stock[g] ?? 0; }
  add(c: Civ, g: Good, v: number) { c.stock[g] = Math.max(0, (c.stock[g] ?? 0) + v); }
  canPay(c: Civ, cost: Stock) { return (Object.keys(cost) as Good[]).every((g) => this.st(c, g) >= (cost[g] ?? 0)); }
  pay(c: Civ, cost: Stock) { for (const g of Object.keys(cost) as Good[]) this.add(c, g, -(cost[g] ?? 0)); }
  foodTotal(c: Civ) { let t = 0; for (const g of ['bread', 'fish', 'meat', 'grain'] as Good[]) t += this.st(c, g) * (GOODS[g].food ?? 0); return t; }
  price(c: Civ, g: Good) { return c.price[g] ?? GOODS[g].base; }
  worth(c: Civ, st: Stock) { let v = 0; for (const g in st) v += (st[g as Good] ?? 0) * this.price(c, g as Good); return v; }

  // ------------------------------------------------------------ research & effects
  has(c: Civ, t: string) { return c.research.done.includes(t); }
  recomputeEff(c: Civ) {
    const cls = CLASSES[c.cls];
    const e: Record<string, number> = {};
    const addE = (x?: Effects) => { if (!x) return; for (const k in x) e[k] = (e[k] ?? 0) + ((x as Record<string, number>)[k] ?? 0); };
    addE(cls.base);
    for (const t of c.research.done) addE(TECH[t]?.eff);
    const sub = cls.subclasses.find((s) => s.id === c.subclass);
    if (sub) { addE(sub.eff); if (c.research.done.includes(`${c.cls}_cap`)) addE(sub.capEff); }
    if (this.w.settlements.some((x) => x.alive && x.civ === c.id && x.civics.wonder)) addE(WONDERS[c.cls].eff);
    c.eff = e as Effects;
  }
  e(c: Civ, k: keyof Effects) { return c.eff[k] ?? 0; }
  subclassName(c: Civ) { return CLASSES[c.cls].subclasses.find((s) => s.id === c.subclass)?.name ?? null; }
  capName(c: Civ) { return CLASSES[c.cls].subclasses.find((s) => s.id === c.subclass)?.capName ?? 'Uç güç'; }
  techName(c: Civ, id: string) { return id === `${c.cls}_cap` && c.subclass ? `${TECH[id].name}: ${this.capName(c)}` : TECH[id].name; }
  eraName(c: Civ) { return ERA_TR[c.era]; }

  /** Kaynak kapısı: kendi yatağı ya da stok */
  access(c: Civ, key: string): boolean {
    if (key === 'water') return this.civSettlements(c).some((s) => this.g.within(s.tile, this.radiusOf(s)).some((t) => this.w.tiles[t].terrain === 'water'));
    if (key === 'coast') return this.civSettlements(c).some((s) => this.g.within(s.tile, this.radiusOf(s)).some((t) => this.w.tiles[t].sea));
    const map: Record<string, string[]> = { gold: ['gold', 'silver'] };
    const kinds = map[key] ?? [key];
    if (kinds.some((k) => this.ownsDeposit(c, k))) return true;
    if (key === 'fertile') return false;
    return this.st(c, key as Good) >= 2;
  }
  ownsDeposit(c: Civ, kind: string) {
    return this.w.deposits.some((d) => d.kind === kind && !d.depleted && this.depositVisible(c, d)
      && d.tiles.some((t) => this.tileCiv(t) === c.id && (DEPOSITS[d.kind].reserve === 0 || this.w.tiles[t].reserve > 0)));
  }
  depositVisible(c: Civ, d: Deposit) {
    const h = DEPOSITS[d.kind].hiddenUntil;
    return (!h || this.has(c, h)) && d.knownBy.includes(c.id);
  }
  discover() {
    for (const c of this.w.civs) {
      if (!c.alive) continue;
      const ss = this.civSettlements(c);
      for (const d of this.w.deposits) {
        if (d.knownBy.includes(c.id)) continue;
        if (d.tiles.some((t) => ss.some((s) => this.g.dist(s.tile, t) <= this.radiusOf(s) + 4))) {
          d.knownBy.push(c.id);
          const def = DEPOSITS[d.kind];
          if (def.count <= 5 && this.depositVisible(c, d) && this.w.day > 0) this.log('discover', `${c.name} bir ${def.name.toLocaleLowerCase('tr')} keşfetti.`, { civ: c.id, tile: d.tiles[0], major: ['mana', 'mithril', 'gold', 'tin'].includes(d.kind) });
        }
      }
    }
  }

  // ------------------------------------------------------------ relations
  rel(a: number, b: number): Relation { return this.w.relations[a][b]; }
  relValue(a: number, b: number) { return Math.max(-100, Math.min(100, Math.round(this.rel(a, b).mods.reduce((s, m) => s + m.value, 0)))); }
  setMod(a: number, b: number, key: string, text: string, value: number, decay = 0, both = true) {
    for (const [x, y] of both ? [[a, b], [b, a]] : [[a, b]]) {
      const r = this.rel(x, y);
      const m = r.mods.find((mm) => mm.key === key);
      if (m) { m.value = value; m.text = text; m.decay = decay; } else r.mods.push({ key, text, value, decay });
    }
  }
  addMod(a: number, b: number, key: string, text: string, delta: number, cap: number, decay: number, both = true) {
    for (const [x, y] of both ? [[a, b], [b, a]] : [[a, b]]) {
      const r = this.rel(x, y);
      let m = r.mods.find((mm) => mm.key === key);
      if (!m) { m = { key, text, value: 0, decay }; r.mods.push(m); }
      m.value = cap >= 0 ? Math.min(cap, m.value + delta) : Math.max(cap, m.value + delta);
      m.text = text; m.decay = decay;
    }
  }
  removeMod(a: number, b: number, key: string, both = true) {
    for (const [x, y] of both ? [[a, b], [b, a]] : [[a, b]]) { const r = this.rel(x, y); r.mods = r.mods.filter((m) => m.key !== key); }
  }
  atWar(a: number, b: number) { return !!this.rel(a, b).war; }
  inWar(c: Civ) { return this.w.civs.some((o) => o.id !== c.id && this.rel(c.id, o.id).war); }

  // ------------------------------------------------------------ map
  moveCost = (i: number) => { const t = this.w.tiles[i]; return t.sea ? Infinity : t.road ? 0.5 : MOVE_COST[t.terrain]; };
  path(from: number, to: number) {
    const k = from + ':' + to;
    if (this.pathCache.has(k)) return this.pathCache.get(k)!;
    // ayrı kara parçaları (ada ↔ anakara) arasında kara yolu yok: bütün haritayı taramadan dön
    const A = this.w.tiles[from], B = this.w.tiles[to];
    if (!A.sea && !B.sea && (A.isle ?? 0) !== (B.isle ?? 0)) return null;
    const p = findPath(this.g, from, to, this.moveCost);
    if (this.pathCache.size > 6000) this.pathCache.clear();
    this.pathCache.set(k, p);
    return p;
  }
  clearPaths() { this.pathCache.clear(); }
  settlement(id: number) { return this.w.settlements.find((s) => s.id === id); }
  hero(id: number) { return this.w.heroes.find((h) => h.id === id)!; }
  civHeroes(c: Civ) { return this.w.heroes.filter((h) => h.civ === c.id && h.state !== 'dead' && h.state !== 'gone'); }
  tileCiv(i: number) { const o = this.w.tiles[i].owner; if (o < 0) return -1; return this.settlement(o)?.civ ?? -1; }
  updateTerritory() {
    const w = this.w;
    for (const s of w.settlements) if (s.alive) s.tier = this.tierOf(s);
    const prev = w.tiles.map((t) => t.owner);
    for (const t of w.tiles) t.owner = -1;
    const alive = w.settlements.filter((s) => s.alive).sort((a, b) => a.founded - b.founded || a.id - b.id);
    const byId = new Map(alive.map((s) => [s.id, s]));
    for (let i = 0; i < w.tiles.length; i++) {
      const o = byId.get(prev[i]);
      if (o && !w.tiles[i].sea && this.g.dist(o.tile, i) <= this.radiusOf(o) && w.tiles[i].camp === undefined && w.tiles[i].innZone === undefined) w.tiles[i].owner = o.id;
    }
    for (const s of alive) {
      w.tiles[s.tile].owner = s.id;
      const tiles = this.g.within(s.tile, this.radiusOf(s)).sort((a, b) => this.g.dist(s.tile, a) - this.g.dist(s.tile, b));
      for (const t of tiles) if (w.tiles[t].owner < 0 && !w.tiles[t].sea && w.tiles[t].camp === undefined && w.tiles[t].innZone === undefined) w.tiles[t].owner = s.id;
    }
    for (const t of w.tiles) if (t.ext) {
      if (t.owner < 0) t.ext = undefined;
      else if (t.ext.settlement !== t.owner) {
        const ns = byId.get(t.owner)!;
        if (ns.civ === byId.get(t.ext.settlement)?.civ) t.ext.settlement = ns.id; else t.ext = undefined;
      }
    }
  }

  // ------------------------------------------------------------ lifecycle
  abandon(s: Settlement, why: string) {
    if (!s.alive) return;
    s.alive = false;
    for (const t of this.w.tiles) if (t.owner === s.id) { t.owner = -1; t.ext = undefined; }
    this.log('death', `${s.name} terk edildi.`, { civ: s.civ, tile: s.tile, cause: why, major: true });
  }
  extinct(c: Civ) {
    if (!c.alive) return;
    c.alive = false;
    c.extinctDay = this.w.day;
    for (const h of this.civHeroes(c)) { if (h.contract) { h.civ = -1; h.contract = undefined; returnToBase(this, h); } else { h.civ = -1; h.state = 'gone'; } }
    for (const o of this.w.civs) if (o.id !== c.id) { this.rel(o.id, c.id).war = null; this.rel(c.id, o.id).war = null; }
    this.log('death', `${c.name} tarihten silindi.`, { civ: c.id, major: true });
  }
  heroTitle(h: Hero) { return `${heroLabel(h)} (${RACES[h.race].name} ${heroClassTr(h.cls)}, Sv ${h.level})`; }

  // ------------------------------------------------------------ step
  step() {
    const w = this.w;
    w.day++;
    for (const c of w.civs) if (c.alive) economyTick(this, c);
    if (w.day % 5 === 0) for (const c of w.civs) if (c.alive) repairTick(this, c);
    for (const c of w.civs) if (c.alive && (w.day + c.id * 3) % 10 === 0) this.civAI(c);
    if (w.day % 10 === 0) { this.updateTerritory(); relationsTick(this); }
    if (w.day % 30 === 0) { this.discover(); worldTick(this); disastersTick(this); }
    if (w.day % YEAR === 0) for (const c of w.civs) if (c.alive) classYearly(this, c);
    campsTick(this);
    tavernsTick(this);
    innsTick(this);
    if (w.day % 5 === 0) heroesTick(this);
    agentsTick(this);
    seaTick(this);
    routesTick(this);
    roadsTick(this);
    for (const c of w.civs) c.threat = Math.min(1.5, Math.max(0, c.threat - 0.0015));
    if (w.day % 60 === 0) for (const c of w.civs) if (c.alive) c.history.push({ day: w.day, pop: this.civPop(c), gold: Math.round(this.st(c, 'gold')), techs: c.research.done.length });
    w.rngState = this.rng.state();
  }

  civAI(c: Civ) {
    if (!this.civSettlements(c).length) { this.extinct(c); return; }
    if (!c.research.current) chooseResearch(this, c);
    eraCheck(this, c);
    chooseBuilds(this, c);
    recruitTick(this, c);
    considerExpansion(this, c);
    considerHero(this, c);
    considerInnBids(this, c);
    considerQuest(this, c);
    considerScout(this, c);
    considerTrade(this, c);
    considerWarAction(this, c);
    considerRaid(this, c);
  }
}

export function heroClassTr(c: string) {
  return ({ fighter: 'Savaşçı', wizard: 'Büyücü', cleric: 'Rahip', rogue: 'Hırsız', ranger: 'Korucu', paladin: 'Paladin', druid: 'Druid', barbarian: 'Barbar' } as Record<string, string>)[c] ?? c;
}
export { ek };
