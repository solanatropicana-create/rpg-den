import { Sim, YEAR, SEASONS, TIER_TR } from '../sim/sim';
import { CLASSES, RACES, UNITS, WONDERS, type RaceId } from '../data/classes';
import { GOODS, GOOD_IDS, DEPOSITS, EXTRACTS, LEVEL_SLOTS, WORKSHOPS, TERRAIN_TR, type Good, type Terrain, type WorkshopKind } from '../data/goods';
import { TECH, MAIN_TECHS, CLASS_TECHS, ERA_TR, ERA_ROMAN, ERA_RULE, techCost, type TechDef } from '../data/techs';
import { HERO_CLASS_TR, STATS, XP_LEVELS, PATH_TR, PATH_DESC, ALIGN_TR, TRAITS } from '../data/heroes';
import { gateOk } from '../sim/research';
import { extractYield, civicName, researchCost } from '../sim/economy';
import { monsterName } from '../sim/monsters';
import { Diorama } from './diorama';
import { Ambience } from './audio';
import { classIcon, uiIcon } from './icons';
import type { Agent, Battle, Civ, GameEvent, Hero, Inn, Settlement } from '../sim/types';
import { innPool, innHeroesOf, CONTRACT_YEARS } from '../sim/inns';
import { heroLabel } from '../sim/will';

const $ = (id: string) => document.getElementById(id)!;
const esc = (s: string) => s.replace(/[&<>"]/g, (c) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;' }[c]!));
const SPEEDS = [1, 2, 5, 20, 60];
const TERRAIN_COL: Record<Terrain, [number, number, number]> = {
  grass: [132, 168, 92], forest: [74, 118, 70], oldforest: [40, 88, 56], hill: [170, 156, 104], mountain: [132, 126, 120],
  water: [78, 132, 176], swamp: [102, 118, 84], tundra: [214, 222, 226],
};
const CAMP_COL = { goblin: '#d9412f', hobgoblin: '#7d1d16', bugbear: '#7a4bb0' };

let seed = 1;
try { const s = Number(localStorage.getItem('fd-seed')); if (s > 0) seed = s; } catch { /* yok */ }
try {
  const q = new URLSearchParams(location.search).get('seed') ?? (location.hash.match(/seed=(\d+)/) ?? [])[1];
  if (q && Number(q) > 0) seed = Number(q);
} catch { /* yok */ }
let sim: Sim;
let speed = 1;
let paused = false;
type Tab = 'civ' | 'tree' | 'compare' | 'hero' | 'inn' | 'log';
let tab: Tab = 'civ';
let treeCiv = 0;
let drawerOpen = false;
let onlyMajor = true;
let showDeposits = true;
let openBattle: number | null = null;
let hover = -1;
let dirty = true;
type Mode = '2d' | '3d';
let mode: Mode = '3d';
try { const m = localStorage.getItem('fd-mode'); if (m === '2d' || m === '3d') mode = m; } catch { /* yok */ }
let dio: Diorama | null = null;
try { dio = new Diorama($('stage')); } catch (err) { console.warn('3B açılamadı', err); mode = '2d'; }
// ?debug: konsoldan erişim (window.__fd.sim, window.__fd.dio)
try { if (new URLSearchParams(location.search).has('debug')) (window as unknown as { __fd: unknown }).__fd = { get sim() { return sim; }, dio }; } catch { /* yok */ }

interface Fx { tile: number; text?: string; color: string; t0: number; dur: number; ring?: boolean; dy?: number }
const fx: Fx[] = [];

function newWorld(s: number) {
  seed = s;
  try { localStorage.setItem('fd-seed', String(s)); } catch { /* yok */ }
  sim = new Sim(s);
  // ?gun=N: dünyayı N gün ileri sararak başlat (test ve gösterim için)
  try { const g = Number(new URLSearchParams(location.search).get('gun')); if (g > 0) for (let d = 0; d < Math.min(g, 6000); d++) sim.step(); } catch { /* yok */ }
  sim.onEvent = onEvent;
  if (dio) dio.follow = null;
  try { $('follow').hidden = true; } catch { /* yok */ }
  fx.length = 0;
  openBattle = null;
  treeCiv = 0;
  civbarKey = '';
  drawerHeadKey = '';
  terrainCache = null;
  ($('seed') as HTMLInputElement).value = String(s);
  dio?.setSim(sim);
  dirty = true;
  if (drawerOpen) { $('pane').scrollTop = 0; renderPanel(); }
  renderHud();
}

function onEvent(e: GameEvent) {
  dirty = true;
  if (mode === '3d' && e.major) feedPush(e);
  if (e.tile === undefined) return;
  const now = performance.now();
  if (mode === '3d' && e.major) cineQueue.push({ tile: e.tile, pri: e.battle ? 5 : e.kind === 'war' || e.kind === 'wonder' ? 4 : e.kind === 'era' || e.kind === 'settle' || e.kind === 'world' ? 3 : e.kind === 'lair' || e.kind === 'hero' || e.kind === 'quest' || e.kind === 'class' ? 2 : 1, t: now, battle: !!e.battle });
  if (mode === '3d' && dio) { dio.onEvent(e, now); return; }
  if (e.battle) {
    const b = sim.w.battles.find((x) => x.id === e.battle);
    fx.push({ tile: e.tile, color: '#ff5a43', t0: now, dur: 1400, ring: true });
    if (b) {
      const pick = b.rolls.filter((r) => r.d20 === 20 || r.d20 === 1).slice(0, 2);
      if (!pick.length && b.rolls.length) pick.push(b.rolls[0]);
      pick.forEach((r, i) => fx.push({ tile: e.tile!, text: r.d20 === 20 ? 'nat 20!' : r.d20 === 1 ? 'nat 1' : `d20: ${r.d20}`, color: r.d20 === 20 ? '#f2cf5b' : r.d20 === 1 ? '#ff8b7d' : '#ffffff', t0: now + i * 350, dur: 1800, dy: i * 4 }));
    }
  } else if (e.major) fx.push({ tile: e.tile, color: e.kind === 'era' ? '#9fe0c9' : '#f2cf5b', t0: now, dur: 1600, ring: true, text: e.kind === 'era' ? 'Yeni çağ!' : undefined });
  if (fx.length > 40) fx.splice(0, fx.length - 40);
}

// ---------------------------------------------------------------- canvas & kamera
const canvas = $('map') as HTMLCanvasElement;
const ctx = canvas.getContext('2d')!;
let base = 8, dpr = 1, cw = 0, ch = 0;
let zoom = 1, panX = 0, panY = 0;
let terrainCache: HTMLCanvasElement | null = null;
let cacheZoom = 0, cacheDay = -999;

function layout() {
  const r = canvas.getBoundingClientRect();
  dpr = Math.min(2, window.devicePixelRatio || 1);
  const w = Math.max(1, Math.round(r.width)), h = Math.max(1, Math.round(r.height));
  if (w === cw && h === ch) return;
  cw = w; ch = h;
  canvas.width = w * dpr; canvas.height = h * dpr;
  const W = sim.w.W, H = sim.w.H;
  base = Math.min((w - 12) / (Math.sqrt(3) * (W + 0.5)), (h - 12) / (1.5 * (H - 1) + 2));
  terrainCache = null;
  clampPan();
}
const size = () => base * zoom;
const mapW = () => Math.sqrt(3) * size() * (sim.w.W + 0.5);
const mapH = () => size() * (1.5 * (sim.w.H - 1) + 2);
function clampPan() {
  const mw = mapW(), mh = mapH();
  panX = mw <= cw ? (cw - mw) / 2 : Math.min(0, Math.max(cw - mw, panX));
  panY = mh <= ch ? (ch - mh) / 2 : Math.min(0, Math.max(ch - mh, panY));
}
/** harita koordinatı (kameradan bağımsız) */
function mpx(i: number, s = size()): [number, number] {
  const [x, y] = sim.g.pixel(i, s);
  return [x + (Math.sqrt(3) / 2) * s, y + s];
}
function px(i: number): [number, number] { const [x, y] = mpx(i); return [x + panX, y + panY]; }

function hexPath(c: CanvasRenderingContext2D, x: number, y: number, s: number) {
  c.beginPath();
  for (let k = 0; k < 6; k++) {
    const a = Math.PI / 180 * (60 * k - 30);
    const X = x + s * Math.cos(a), Y = y + s * Math.sin(a);
    k ? c.lineTo(X, Y) : c.moveTo(X, Y);
  }
  c.closePath();
}
const jit = (i: number, k: number) => { const v = Math.sin(i * 12.9898 + k * 78.233) * 43758.5453; return v - Math.floor(v); };

function buildTerrain() {
  const s = size();
  const scale = Math.min(dpr, 4096 / Math.max(1, mapW()));
  const c = document.createElement('canvas');
  c.width = Math.ceil(mapW() * scale); c.height = Math.ceil(mapH() * scale);
  const g = c.getContext('2d')!;
  g.scale(scale, scale);
  const w = sim.w;
  for (let i = 0; i < w.tiles.length; i++) {
    const t = w.tiles[i];
    const [x, y] = mpx(i, s);
    const [r, gg, b] = TERRAIN_COL[t.terrain];
    const sh = 0.86 + t.elev * 0.3 + (jit(i, 1) - 0.5) * 0.06;
    g.fillStyle = `rgb(${r * sh | 0},${gg * sh | 0},${b * sh | 0})`;
    hexPath(g, x, y, s + 0.6); g.fill();
    if (t.terrain === 'forest' || t.terrain === 'oldforest') {
      g.fillStyle = t.terrain === 'oldforest' ? 'rgba(14,40,22,.8)' : 'rgba(28,58,32,.7)';
      for (let k = 0; k < (t.terrain === 'oldforest' ? 3 : 2); k++) {
        const tx = x + (jit(i, k + 2) - 0.5) * s * 1.1, ty = y + (jit(i, k + 7) - 0.5) * s * 0.9, q = s * (t.terrain === 'oldforest' ? 0.42 : 0.32);
        g.beginPath(); g.moveTo(tx, ty - q); g.lineTo(tx + q * 0.7, ty + q * 0.6); g.lineTo(tx - q * 0.7, ty + q * 0.6); g.fill();
      }
    } else if (t.terrain === 'hill') {
      g.strokeStyle = 'rgba(90,72,40,.5)'; g.lineWidth = Math.max(0.8, s * 0.1);
      g.beginPath(); g.arc(x - s * 0.15, y + s * 0.25, s * 0.35, Math.PI * 1.1, Math.PI * 1.9); g.stroke();
    } else if (t.terrain === 'mountain') {
      const q = s * 0.62;
      g.fillStyle = 'rgba(70,64,60,.9)'; g.beginPath(); g.moveTo(x, y - q); g.lineTo(x + q * 0.9, y + q * 0.6); g.lineTo(x - q * 0.9, y + q * 0.6); g.fill();
      g.fillStyle = 'rgba(245,245,240,.95)'; g.beginPath(); g.moveTo(x, y - q); g.lineTo(x + q * 0.3, y - q * 0.45); g.lineTo(x - q * 0.3, y - q * 0.45); g.fill();
    } else if (t.terrain === 'swamp') {
      g.fillStyle = 'rgba(60,90,90,.5)'; g.fillRect(x - s * 0.4, y, s * 0.8, s * 0.12); g.fillRect(x - s * 0.2, y - s * 0.3, s * 0.6, s * 0.1);
    }
  }
  terrainCache = c;
  cacheZoom = zoom; cacheDay = sim.day;
}

function civColor(civ: number) { return sim.w.civs[civ]?.color ?? '#888'; }
function rgba(hex: string, a: number) { const n = parseInt(hex.slice(1), 16); return `rgba(${n >> 16 & 255},${n >> 8 & 255},${n & 255},${a})`; }
function agentXY(a: Agent): [number, number] {
  const i = Math.min(a.step, a.path.length - 1), j = Math.min(i + 1, a.path.length - 1);
  const [x1, y1] = px(a.path[i]), [x2, y2] = px(a.path[j]);
  const p = Math.min(1, a.progress);
  return [x1 + (x2 - x1) * p, y1 + (y2 - y1) * p];
}
const visible = (x: number, y: number, m = 20) => x > -m && y > -m && x < cw + m && y < ch + m;

function draw(now: number) {
  layout();
  if (!terrainCache || cacheZoom !== zoom || sim.day - cacheDay >= 30) buildTerrain();
  ctx.setTransform(1, 0, 0, 1, 0, 0);
  ctx.clearRect(0, 0, canvas.width, canvas.height);
  ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
  ctx.drawImage(terrainCache!, panX, panY, mapW(), mapH());
  const w = sim.w, g = sim.g, s = size();

  const tileCiv = w.tiles.map((t) => (t.owner >= 0 ? (sim.settlement(t.owner)?.civ ?? -1) : -1));
  for (let i = 0; i < w.tiles.length; i++) {
    const c = tileCiv[i]; if (c < 0) continue;
    const [x, y] = px(i); if (!visible(x, y)) continue;
    ctx.fillStyle = rgba(civColor(c), 0.2); hexPath(ctx, x, y, s); ctx.fill();
  }
  ctx.lineWidth = Math.max(1.5, s * 0.18);
  for (let i = 0; i < w.tiles.length; i++) {
    const c = tileCiv[i]; if (c < 0) continue;
    const [x, y] = px(i); if (!visible(x, y)) continue;
    for (const n of g.neighbors(i)) {
      if (tileCiv[n] === c) continue;
      const [nx, ny] = px(n);
      const mx = (x + nx) / 2, my = (y + ny) / 2, dx = ny - y, dy = -(nx - x), len = Math.hypot(dx, dy) || 1, hl = s * 0.5;
      ctx.strokeStyle = rgba(civColor(c), 0.95);
      ctx.beginPath(); ctx.moveTo(mx + dx / len * hl, my + dy / len * hl); ctx.lineTo(mx - dx / len * hl, my - dy / len * hl); ctx.stroke();
    }
  }
  // yollar ve güzergâhlar
  ctx.strokeStyle = 'rgba(232,214,160,.9)'; ctx.lineWidth = Math.max(1.2, s * 0.2); ctx.lineCap = 'round';
  for (let i = 0; i < w.tiles.length; i++) {
    if (!w.tiles[i].road) continue;
    const [x, y] = px(i);
    for (const n of g.neighbors(i)) {
      if (n < i || !(w.tiles[n].road || w.settlements.some((q) => q.alive && q.tile === n))) continue;
      const [nx, ny] = px(n); ctx.beginPath(); ctx.moveTo(x, y); ctx.lineTo(nx, ny); ctx.stroke();
    }
  }
  ctx.setLineDash([s * 0.35, s * 0.4]); ctx.lineWidth = Math.max(1, s * 0.12);
  for (const r of w.routes) {
    if (!r.alive) continue;
    ctx.strokeStyle = r.kind === 'treaty' ? 'rgba(242,207,91,.95)' : 'rgba(255,255,255,.75)';
    ctx.beginPath(); r.path.forEach((t, k) => { const [x, y] = px(t); k ? ctx.lineTo(x, y) : ctx.moveTo(x, y); }); ctx.stroke();
  }
  ctx.setLineDash([]);
  // yataklar
  if (showDeposits) {
    for (const d of w.deposits) {
      const def = DEPOSITS[d.kind];
      if (d.kind === 'fertile') continue;
      for (const t of d.tiles) {
        const [x, y] = px(t); if (!visible(x, y)) continue;
        const tile = w.tiles[t];
        const empty = def.reserve > 0 && tile.reserve <= 0;
        const r = Math.max(1.8, s * 0.26);
        const cx = x - s * 0.42, cy = y - s * 0.38;
        ctx.beginPath(); ctx.arc(cx, cy, r, 0, Math.PI * 2);
        ctx.fillStyle = empty ? 'rgba(60,60,60,.7)' : def.color; ctx.fill();
        ctx.lineWidth = 1; ctx.strokeStyle = def.hiddenUntil && !w.civs.some((c) => sim.depositVisible(c, d)) ? 'rgba(255,255,255,.9)' : 'rgba(0,0,0,.55)';
        if (def.hiddenUntil && !w.civs.some((c) => sim.depositVisible(c, d))) ctx.setLineDash([2, 2]);
        ctx.stroke(); ctx.setLineDash([]);
      }
    }
    // verimli ova: ince sarı çizgiler
    ctx.strokeStyle = 'rgba(240,220,120,.55)'; ctx.lineWidth = 1;
    for (const d of w.deposits) if (d.kind === 'fertile') for (const t of d.tiles) {
      const [x, y] = px(t); if (!visible(x, y)) continue;
      for (let k = -1; k <= 1; k++) { ctx.beginPath(); ctx.moveTo(x - s * 0.45, y + k * s * 0.28); ctx.lineTo(x + s * 0.45, y + k * s * 0.28); ctx.stroke(); }
    }
  }
  // çıkarma yapıları
  for (let i = 0; i < w.tiles.length; i++) {
    const t = w.tiles[i]; if (!t.ext) continue;
    const [x, y] = px(i); if (!visible(x, y)) continue;
    const q = Math.max(4, s * 0.62);
    const c = tileCiv[i];
    ctx.fillStyle = t.ext.depleted ? '#555' : c >= 0 ? civColor(c) : '#888';
    ctx.strokeStyle = 'rgba(255,255,255,.9)'; ctx.lineWidth = 1;
    ctx.fillRect(x - q / 2 + s * 0.15, y - q / 2 + s * 0.15, q, q); ctx.strokeRect(x - q / 2 + s * 0.15, y - q / 2 + s * 0.15, q, q);
    if (s >= 7) { ctx.fillStyle = '#fff'; ctx.font = `700 ${Math.max(7, q * 0.8)}px 'JetBrains Mono', monospace`; ctx.textAlign = 'center'; ctx.textBaseline = 'middle'; ctx.fillText(String(t.ext.level), x + s * 0.15, y + s * 0.18); ctx.textBaseline = 'alphabetic'; }
  }
  // canavar kampları
  for (const cp of w.camps) {
    const [x, y] = px(cp.tile); if (!visible(x, y)) continue;
    const q = Math.max(5, s * 0.8);
    ctx.globalAlpha = cp.alive ? 1 : 0.3;
    ctx.fillStyle = cp.alive ? CAMP_COL[cp.kind] : '#555'; ctx.strokeStyle = '#fff'; ctx.lineWidth = 1.5;
    ctx.beginPath();
    if (cp.kind === 'goblin') { ctx.moveTo(x, y - q); ctx.lineTo(x + q * 0.9, y + q * 0.6); ctx.lineTo(x - q * 0.9, y + q * 0.6); ctx.closePath(); }
    else if (cp.kind === 'hobgoblin') ctx.rect(x - q * 0.75, y - q * 0.75, q * 1.5, q * 1.5);
    else ctx.arc(x, y, q * 0.8, 0, Math.PI * 2);
    ctx.fill(); ctx.stroke();
    if (cp.alive) label(`${cp.count}${cp.boss ? '+' : ''}`, x, y + q * 0.15, '#fff', 8 + s * 0.3, true);
    ctx.globalAlpha = 1;
  }
  // tarafsız hanlar
  for (const inn of w.inns) {
    const [x, y] = px(inn.tile); if (!visible(x, y)) continue;
    const q = Math.max(5, s * 0.75);
    ctx.globalAlpha = inn.alive ? 1 : 0.4;
    ctx.fillStyle = inn.alive ? '#e8c24a' : '#6d6760'; ctx.strokeStyle = '#3a2814'; ctx.lineWidth = 1.5;
    ctx.beginPath(); ctx.moveTo(x, y - q); ctx.lineTo(x + q, y); ctx.lineTo(x, y + q); ctx.lineTo(x - q, y); ctx.closePath(); ctx.fill(); ctx.stroke();
    if (s >= 6) label(inn.name, x, y + q + 9, inn.alive ? '#ffe7a8' : '#b8b2a6', 8 + s * 0.3, true);
    ctx.globalAlpha = 1;
  }
  // yerleşimler (büyükten küçüğe; çakışan isimler gizlenir)
  const labels: [number, number, number, number][] = [];
  for (const st of w.settlements.filter((x) => x.alive).sort((a, b) => sim.pop(b) - sim.pop(a))) {
    if (!st.alive) continue;
    const [x, y] = px(st.tile); if (!visible(x, y, 60)) continue;
    const P = sim.pop(st);
    const r = Math.max(5, s * (0.5 + Math.min(0.7, Math.sqrt(P) * 0.06)));
    if (st.civics.stonewall || st.civics.castle || st.civics.palisade) {
      ctx.strokeStyle = st.civics.castle ? '#d8d2c4' : st.civics.stonewall ? '#a9a39a' : '#6b4a24'; ctx.lineWidth = Math.max(2, s * 0.22);
      ctx.beginPath(); ctx.arc(x, y, r + s * 0.25, 0, Math.PI * 2); ctx.stroke();
    }
    ctx.fillStyle = civColor(st.civ); ctx.strokeStyle = '#fff'; ctx.lineWidth = 2;
    ctx.beginPath(); ctx.arc(x, y, r, 0, Math.PI * 2); ctx.fill(); ctx.stroke();
    // kademe halkaları; uzaklaşınca küçük dairede yer kalmazsa çizilmez (negatif yarıçap canvas hatası veriyordu)
    for (let k = 0; k < st.tier; k++) { const rr = r - 2.5 - k * 2.5; if (rr < 1) break; ctx.strokeStyle = 'rgba(255,255,255,.7)'; ctx.lineWidth = 1; ctx.beginPath(); ctx.arc(x, y, rr, 0, Math.PI * 2); ctx.stroke(); }
    if (st.civics.tavern) { ctx.fillStyle = '#f2cf5b'; ctx.beginPath(); ctx.arc(x + r * 0.8, y - r * 0.8, Math.max(2.5, s * 0.22), 0, Math.PI * 2); ctx.fill(); }
    const heroes = w.heroes.filter((h) => (h.state === 'home' || h.state === 'tavern') && h.pos === st.tile).length;
    label(String(P), x, y + 1, '#fff', 8 + r * 0.45, true);
    const txt = `${st.name}${heroes ? ' ★' + heroes : ''}`, fs = 10 + s * 0.12, ly = y + r + 8 + s * 0.2;
    ctx.font = `600 ${fs}px 'Alegreya Sans', system-ui, sans-serif`;
    const tw = ctx.measureText(txt).width;
    const box: [number, number, number, number] = [x - tw / 2 - 2, ly - fs / 2, x + tw / 2 + 2, ly + fs / 2];
    if (!labels.some((b) => b[0] < box[2] && box[0] < b[2] && b[1] < box[3] && box[1] < b[3])) { labels.push(box); label(txt, x, ly, '#fff', fs, false); }
  }
  // birimler
  for (const a of w.agents) {
    if (a.dead) continue;
    const [x, y] = agentXY(a); if (!visible(x, y)) continue;
    const q = Math.max(3, s * 0.36);
    ctx.lineWidth = 1.2; ctx.strokeStyle = '#111';
    switch (a.kind) {
      case 'caravan':
        ctx.fillStyle = '#fff'; ctx.fillRect(x - q, y - q * 0.7, q * 2, q * 1.4); ctx.strokeRect(x - q, y - q * 0.7, q * 2, q * 1.4);
        ctx.fillStyle = civColor(a.civ); ctx.fillRect(x - q * 0.5, y - q * 0.35, q, q * 0.7); break;
      case 'army':
        ctx.fillStyle = civColor(a.civ); ctx.beginPath(); ctx.moveTo(x, y - q * 1.6); ctx.lineTo(x + q * 1.4, y + q); ctx.lineTo(x - q * 1.4, y + q); ctx.closePath(); ctx.fill(); ctx.strokeStyle = a.purpose === 'plunder' ? '#ff5a43' : '#fff'; ctx.stroke();
        label(String((a.troops ?? 0) + (a.heroes?.length ?? 0)), x, y + q * 2.6, '#fff', 9, false); break;
      case 'raid':
        ctx.fillStyle = CAMP_COL[a.monster ?? 'goblin'];
        for (let k = 0; k < Math.min(5, a.troops ?? 1); k++) { ctx.beginPath(); ctx.arc(x + Math.cos(k * 1.3) * q, y + Math.sin(k * 1.3) * q, q * 0.5, 0, Math.PI * 2); ctx.fill(); ctx.stroke(); }
        break;
      case 'hero': case 'party':
        ctx.fillStyle = '#f3e9c6'; ctx.strokeStyle = '#a57a16'; ctx.lineWidth = 2; ctx.beginPath(); ctx.arc(x, y, q, 0, Math.PI * 2); ctx.fill(); ctx.stroke(); break;
      case 'settlers':
        ctx.fillStyle = 'rgba(255,255,255,.25)'; ctx.strokeStyle = civColor(a.civ); ctx.lineWidth = 2.5; ctx.beginPath(); ctx.arc(x, y, q * 1.2, 0, Math.PI * 2); ctx.fill(); ctx.stroke(); break;
      case 'scout':
        ctx.fillStyle = civColor(a.civ); ctx.strokeStyle = '#fff'; ctx.beginPath(); ctx.arc(x, y, q * 0.6, 0, Math.PI * 2); ctx.fill(); ctx.stroke(); break;
    }
  }
  // efektler
  for (let k = fx.length - 1; k >= 0; k--) {
    const f = fx[k];
    const t = (now - f.t0) / f.dur;
    if (t < 0) continue;
    if (t > 1) { fx.splice(k, 1); continue; }
    const [x, y] = px(f.tile);
    if (f.ring) { ctx.strokeStyle = f.color; ctx.globalAlpha = 1 - t; ctx.lineWidth = 3; ctx.beginPath(); ctx.arc(x, y, Math.max(8, s) * (0.8 + t * 2.2), 0, Math.PI * 2); ctx.stroke(); ctx.globalAlpha = 1; }
    if (f.text) {
      ctx.globalAlpha = Math.min(1, (1 - t) * 2);
      const yy = y - Math.max(10, s) * (1 + t * 2.4) - (f.dy ?? 0) * 3;
      const fs = 12;
      ctx.font = `700 ${fs}px 'JetBrains Mono', monospace`;
      const tw = ctx.measureText(f.text).width;
      ctx.fillStyle = 'rgba(20,20,20,.82)'; ctx.fillRect(x - tw / 2 - 5, yy - fs * 0.8, tw + 10, fs * 1.25);
      ctx.fillStyle = f.color; ctx.textAlign = 'center'; ctx.fillText(f.text, x, yy + fs * 0.15);
      ctx.globalAlpha = 1;
    }
  }
  if (hover >= 0) { const [x, y] = px(hover); ctx.strokeStyle = 'rgba(255,255,255,.9)'; ctx.lineWidth = 2; hexPath(ctx, x, y, s); ctx.stroke(); }
}

function label(text: string, x: number, y: number, color: string, fs: number, center: boolean) {
  ctx.font = `600 ${fs}px 'Alegreya Sans', system-ui, sans-serif`;
  ctx.textAlign = 'center'; ctx.textBaseline = 'middle';
  if (!center) { ctx.lineWidth = 3; ctx.strokeStyle = 'rgba(0,0,0,.75)'; ctx.strokeText(text, x, y); }
  ctx.fillStyle = color; ctx.fillText(text, x, y);
  ctx.textBaseline = 'alphabetic';
}

// ---------------------------------------------------------------- fare: hover, tık, zoom, kaydırma
function tileAt(mx: number, my: number): number {
  const s = size(), w = sim.w;
  const row = Math.round((my - panY - s) / (1.5 * s));
  let best = -1, bd = Infinity;
  for (let r = row - 1; r <= row + 1; r++) {
    if (r < 0 || r >= w.H) continue;
    const approxC = Math.round((mx - panX) / (Math.sqrt(3) * s) - 0.5 * (r & 1) - 0.5);
    for (let c = approxC - 2; c <= approxC + 2; c++) {
      if (c < 0 || c >= w.W) continue;
      const i = r * w.W + c;
      const [x, y] = px(i);
      const d = (x - mx) ** 2 + (y - my) ** 2;
      if (d < bd) { bd = d; best = i; }
    }
  }
  return bd <= s * s * 1.1 ? best : -1;
}
let drag: { x: number; y: number; px: number; py: number; moved: boolean } | null = null;
canvas.addEventListener('pointerdown', (e) => { drag = { x: e.clientX, y: e.clientY, px: panX, py: panY, moved: false }; });
window.addEventListener('pointerup', () => { setTimeout(() => { drag = null; }, 0); });
canvas.addEventListener('pointermove', (e) => {
  const r = canvas.getBoundingClientRect();
  const mx = e.clientX - r.left, my = e.clientY - r.top;
  if (drag && (e.buttons & 1)) {
    const dx = e.clientX - drag.x, dy = e.clientY - drag.y;
    if (Math.abs(dx) + Math.abs(dy) > 4) drag.moved = true;
    if (drag.moved) { panX = drag.px + dx; panY = drag.py + dy; clampPan(); $('tip').hidden = true; return; }
  }
  hover = tileAt(mx, my);
  const tip = $('tip');
  if (hover < 0) { tip.hidden = true; return; }
  tip.hidden = false;
  tip.innerHTML = tileInfo(hover);
  const tw = tip.offsetWidth, th = tip.offsetHeight;
  tip.style.left = Math.max(4, Math.min(r.width - tw - 6, mx + 14)) + 'px';
  tip.style.top = Math.max(4, Math.min(r.height - th - 6, my + 14)) + 'px';
});
canvas.addEventListener('pointerleave', () => { hover = -1; $('tip').hidden = true; });
canvas.addEventListener('wheel', (e) => {
  e.preventDefault();
  const r = canvas.getBoundingClientRect();
  setZoom(zoom * (e.deltaY < 0 ? 1.2 : 1 / 1.2), e.clientX - r.left, e.clientY - r.top);
}, { passive: false });
function setZoom(z: number, cx = cw / 2, cy = ch / 2) {
  const nz = Math.max(1, Math.min(4, z));
  const k = nz / zoom;
  panX = cx - (cx - panX) * k; panY = cy - (cy - panY) * k;
  zoom = nz; clampPan();
}
canvas.addEventListener('click', (e) => {
  if (drag?.moved) return;
  const r = canvas.getBoundingClientRect();
  selectTile(tileAt(e.clientX - r.left, e.clientY - r.top));
});
function selectTile(i: number) {
  if (i < 0) return;
  // kasabanın kendisine tıklamak medeniyet panelini açar; sınır içindeki araziye tıklamak yalnız panel açıkken seçimi değiştirir
  const onSt = sim.w.settlements.find((x) => x.alive && x.tile === i);
  const st = onSt ?? (sim.w.tiles[i].owner >= 0 ? sim.settlement(sim.w.tiles[i].owner) : undefined);
  if (!st) return;
  const t: Tab = drawerOpen && tab === 'tree' ? 'tree' : 'civ';
  if (onSt || (drawerOpen && (tab === 'civ' || tab === 'tree'))) openDrawer(t, st.civ);
}

// ---------------------------------------------------------------- 3B etkileşim
function showTip(i: number, mx: number, my: number) {
  const tip = $('tip');
  if (i < 0) { tip.hidden = true; return; }
  const r = $('stage').getBoundingClientRect();
  tip.hidden = false;
  tip.innerHTML = tileInfo(i);
  const tw = tip.offsetWidth, th = tip.offsetHeight;
  tip.style.left = Math.max(4, Math.min(r.width - tw - 6, mx + 14)) + 'px';
  tip.style.top = Math.max(4, Math.min(r.height - th - 6, my + 14)) + 'px';
}
if (dio) {
  const el = dio.renderer.domElement;
  let down: { x: number; y: number } | null = null, moved = false, lastPick = 0;
  el.addEventListener('pointerdown', (e) => { down = { x: e.clientX, y: e.clientY }; moved = false; if (cine) cinePause = performance.now() + 25000; });
  el.addEventListener('wheel', () => { if (cine) cinePause = performance.now() + 25000; }, { passive: true });
  el.addEventListener('pointermove', (e) => {
    if (down && Math.abs(e.clientX - down.x) + Math.abs(e.clientY - down.y) > 5) { moved = true; if ((e.buttons & 1) && dio!.follow) stopFollow(); }
    if (e.buttons) { $('tip').hidden = true; return; }
    const now = performance.now();
    if (now - lastPick < 60) return;
    lastPick = now;
    const r = el.getBoundingClientRect();
    const ct = dio!.pickCitizen(e.clientX, e.clientY);
    if (ct) {
      const st = sim.w.settlements.find((q) => q.id === ct.st);
      const tip = $('tip');
      tip.hidden = false;
      tip.innerHTML = `<b>${ct.name}</b> · ${RACES[ct.race as RaceId]?.name ?? ''} ${ct.role}<br>${st ? st.name + ' · ' : ''}${ct.mood}${ct.idx >= 0 ? '<br><span style="opacity:.7">Tıkla: gününü izle</span>' : ''}`;
      tip.style.left = Math.max(4, Math.min(r.width - tip.offsetWidth - 6, e.clientX - r.left + 14)) + 'px';
      tip.style.top = Math.max(4, Math.min(r.height - tip.offsetHeight - 6, e.clientY - r.top + 14)) + 'px';
      el.style.cursor = ct.idx >= 0 ? 'pointer' : 'help';
      return;
    }
    const ag = dio!.pickAgent(e.clientX, e.clientY);
    const hs = ag ? null : dio!.pickHouse(e.clientX, e.clientY);
    if (hs) {
      const tip = $('tip');
      tip.hidden = false;
      const hl = sim.homeless(hs.st);
      tip.innerHTML = hs.burnt ? `<b>Yanmış ${hs.kind.toLocaleLowerCase('tr')}</b><br>${hs.cap} kişilikti · onarım için kereste bekliyor`
        : hs.occ ? `<b>${hs.family} hanesi</b> · ${hs.kind} · ${RACES[hs.race as RaceId]?.name ?? ''}<br>${hs.occ}/${hs.cap} kişi: ${hs.members.join(', ')}`
        : `<b>Boş ${hs.kind.toLocaleLowerCase('tr')}</b> · ${hs.cap} kişilik<br>Yeni bir aile bekliyor`;
      tip.innerHTML += `<br><span style="opacity:.7">${hs.st.name} · ${sim.pop(hs.st)}/${sim.housing(hs.st)} konut${hl ? ` · ${hl} kişi barakada` : ''}</span>`;
      tip.style.left = Math.max(4, Math.min(r.width - tip.offsetWidth - 6, e.clientX - r.left + 14)) + 'px';
      tip.style.top = Math.max(4, Math.min(r.height - tip.offsetHeight - 6, e.clientY - r.top + 14)) + 'px';
      el.style.cursor = 'help';
      return;
    }
    if (ag) {
      const a = ag.kind === 'agent' ? sim.w.agents.find((q) => q.id === ag.id) : sim.w.agents.find((q) => !q.dead && q.heroes?.includes(ag.id));
      const tip = $('tip');
      tip.hidden = false;
      tip.innerHTML = `${a ? agentDesc(a) : ''}<br><span style="opacity:.7">Tıkla: izle</span>`;
      tip.style.left = Math.max(4, Math.min(r.width - tip.offsetWidth - 6, e.clientX - r.left + 14)) + 'px';
      tip.style.top = Math.max(4, Math.min(r.height - tip.offsetHeight - 6, e.clientY - r.top + 14)) + 'px';
      el.style.cursor = 'pointer';
      return;
    }
    el.style.cursor = '';
    showTip(dio!.pick(e.clientX, e.clientY), e.clientX - r.left, e.clientY - r.top);
  });
  el.addEventListener('pointerup', (e) => {
    if (down && !moved && e.button === 0) {
      const ct = dio!.pickCitizen(e.clientX, e.clientY);
      const f = ct && ct.idx >= 0 ? { kind: 'cit' as const, id: ct.st, idx: ct.idx } : dio!.pickAgent(e.clientX, e.clientY);
      if (f) { dio!.startFollow(f); renderFollow(); }
      else selectTile(dio!.pick(e.clientX, e.clientY));
    }
    down = null;
  });
  el.addEventListener('pointerleave', () => { $('tip').hidden = true; });
  el.addEventListener('dblclick', (e) => { const i = dio!.pick(e.clientX, e.clientY); if (i >= 0) dio!.focusTile(i); });
}
// ---------------------------------------------------------------- olay akışı ve sinema
const feedQ: GameEvent[] = [];
let feedLast = 0;
function feedPush(e: GameEvent) {
  if (e.kind === 'research' && !e.text.includes('çağ')) return;
  feedQ.push(e);
  if (feedQ.length > 12) feedQ.splice(0, feedQ.length - 12);
}
let bannerYear = 0;
function yearBanner(now: number) {
  const y = sim.year;
  if (y === bannerYear) return;
  const first = bannerYear === 0;
  bannerYear = y;
  if (first || mode !== '3d') return;
  const alive = sim.w.civs.filter((c) => c.alive);
  const top = alive.slice().sort((a, b) => sim.civPop(b) - sim.civPop(a))[0];
  const el = $('banner');
  el.innerHTML = `<b>Yıl ${y}</b>${top ? `<span>En kalabalık: <i style="color:${top.color}">■</i> ${esc(top.name)} · ${sim.civPop(top)} nüfus · ${ERA_TR[top.era]} çağı</span>` : ''}`;
  el.classList.remove('show'); void el.offsetWidth; el.classList.add('show');
  void now;
}
function feedTick(now: number) {
  yearBanner(now);
  const box = $('feed');
  for (const el of Array.from(box.children) as HTMLElement[]) if (now - Number(el.dataset.t) > 7500) { el.classList.add('out'); if (now - Number(el.dataset.t) > 8100) el.remove(); }
  if (!feedQ.length || now - feedLast < 1300) return;
  feedLast = now;
  const e = feedQ.shift()!;
  const el = document.createElement('button');
  el.className = 'feed-item';
  el.dataset.t = String(now);
  if (e.tile !== undefined) el.dataset.tile = String(e.tile);
  el.style.setProperty('--cc', e.civ !== undefined ? civColor(e.civ) : e.kind === 'lair' ? '#d9412f' : '#9fe0c9');
  el.innerHTML = `${esc(e.text)}${e.cause ? `<small>${esc(e.cause)}</small>` : ''}`;
  box.appendChild(el);
  while (box.children.length > 4) box.firstElementChild!.remove();
}
const cineQueue: { tile: number; pri: number; t: number; battle: boolean }[] = [];
let cine = false, cinePause = 0, cineNext = 0;
function setCine(v: boolean) {
  cine = v;
  try { localStorage.setItem('fd-cine', v ? '1' : '0'); } catch { /* yok */ }
  if (dio) dio.autoOrbit = v;
  $('cine').setAttribute('aria-pressed', String(v));
  cineNext = 0;
}
function cineTick(now: number) {
  if (!cine || !dio || mode !== '3d' || paused) return;
  if (now < cinePause || now < cineNext) return;
  // taze olaylar: son 25 sn, önceliğe göre
  const fresh = cineQueue.filter((q) => now - q.t < 25000).sort((a, b) => b.pri - a.pri || b.t - a.t);
  cineQueue.length = 0;
  const top = fresh[0];
  if (top) {
    stopFollow();
    dio.shot(top.tile, top.battle ? 8 : top.pri >= 3 ? 13 : 16, top.battle ? 1.02 : 0.92);
    cineNext = now + (top.battle ? 9000 : 8000);
    return;
  }
  // olay yoksa: bir kahramanı, orduyu ya da kervanı izle; yoksa bir kasabaya süzül
  const movers = sim.w.agents.filter((a) => !a.dead && (a.kind === 'army' || a.kind === 'hero' || a.kind === 'party' || a.kind === 'caravan' || a.kind === 'raid' || a.kind === 'settlers') && a.path.length - a.step > 6);
  const weight = (k: string) => (k === 'army' || k === 'raid' ? 4 : k === 'party' || k === 'hero' ? 3 : k === 'settlers' ? 2 : 1);
  if (movers.length && Math.random() < 0.7) {
    let r = Math.random() * movers.reduce((a, m) => a + weight(m.kind), 0);
    const m = movers.find((x) => (r -= weight(x.kind)) <= 0) ?? movers[0];
    dio.startFollow((m.kind === 'hero' || m.kind === 'party') && m.heroes?.length ? { kind: 'hero', id: m.heroes[0] } : { kind: 'agent', id: m.id });
    renderFollow();
    cineNext = now + 12000;
    return;
  }
  const towns = sim.w.settlements.filter((x) => x.alive);
  if (towns.length) { stopFollow(); const st = towns[Math.floor(Math.random() * towns.length)]; dio.shot(st.tile, 9 + Math.random() * 6, 0.9 + Math.random() * 0.25); cineNext = now + 10000; }
}
function stopFollow() { if (dio) dio.follow = null; $('follow').hidden = true; }
function renderFollow() {
  const box = $('follow');
  const f = dio?.follow;
  if (!f || mode !== '3d') { box.hidden = true; return; }
  const w = sim.w;
  let h = '';
  if (f.kind === 'cit') {
    const c = dio!.citInfo(f.id, f.idx ?? 0);
    if (!c) { box.hidden = true; return; }
    const civ = w.civs[c.st.civ];
    h = `<b>${esc(c.name)}${c.family && !c.inShanty ? " " + esc(c.family) : ""}</b><div class="sub">${RACES[c.race as RaceId]?.name ?? ''} · ${c.age} yaşında · ${esc(c.job)}</div>`;
    h += `<div class="sub"><span style="color:${civColor(civ.id)}">■</span> ${esc(c.st.name)} · ${esc(civ.name)}</div>`;
    h += `<div class="sub">${c.inShanty ? '🏚 Evi yok, sur dışındaki barakalarda yaşıyor' : c.family ? `🏠 ${esc(c.family)} hanesinden` : ''}</div>`;
    h += `<div class="sub" style="margin-top:3px">Şu an: <b>${c.icon ? c.icon + ' ' : ''}${esc(c.mood)}</b></div>`;
    h += `<div class="day">${c.plan.map(([k, v], j) => `<div class="${j === c.step ? 'on' : j < c.step ? 'done' : ''}"><span class="mono">${k}</span> ${esc(v)}</div>`).join('')}</div>`;
    const warn = c.st.plague ? 'Kasabada salgın var.' : c.st.starving > 0 ? 'Kasabada kıtlık var, ambar boşalıyor.' : dio!.festival(c.st) ? `${dio!.festival(c.st)} sürüyor.` : '';
    if (warn) h += `<div class="sub" style="margin-top:4px;color:#f2cf5b">${warn}</div>`;
  } else if (f.kind === 'hero') {
    const hr = w.heroes.find((x) => x.id === f.id);
    if (!hr) { box.hidden = true; return; }
    const a = w.agents.find((q) => !q.dead && q.heroes?.includes(hr.id));
    const aff = hr.civ >= 0 ? `<span style="color:${civColor(hr.civ)}">■</span> ${esc(w.civs[hr.civ].name)}` : 'Bağımsız';
    h = `<b>${esc(heroLabel(hr))}</b><div class="sub">Sv ${hr.level} ${RACES[hr.race].name} ${HERO_CLASS_TR[hr.cls]} · ${aff}</div>`;
    h += `<div class="sub">${ALIGN_TR[hr.align]} · ${hr.civ >= 0 ? `yolu askıda (${PATH_TR[hr.path]})` : `${PATH_TR[hr.path]}: ${PATH_DESC[hr.path]}`}</div>`;
    if (hr.traits.length) h += `<div class="sub">${hr.traits.map((t) => `${TRAITS[t].icon} ${TRAITS[t].name}`).join(' · ')}</div>`;
    h += `<div class="hp"><i style="width:${(hr.hp / hr.maxHp) * 100}%"></i></div><div class="sub mono">HP ${hr.hp}/${hr.maxHp} · AC ${hr.ac}${hr.kills ? ` · ${hr.kills} leş` : ''}</div>`;
    h += xpBar(hr);
    h += `<div class="sub">Şu an: <b>${esc(heroNow(hr, a))}</b></div>`;
    if (hr.journal.length) h += `<div class="hlog">${hr.journal.slice(-3).reverse().map((j) => `<div><span class="mono">${sim.dateStr(j.day).replace('Yıl ', 'Y')}</span> ${esc(j.text)}</div>`).join('')}</div>`;
  } else {
    const a = w.agents.find((q) => q.id === f.id && !q.dead);
    if (!a) { box.hidden = true; return; }
    const dest = a.to !== undefined ? sim.w.settlements.find((x) => x.id === a.to) : undefined;
    h = `<b>${agentDesc(a)}</b>`;
    if (dest) h += `<div class="sub">Hedef: ${esc(dest.name)}</div>`;
    h += `<div class="bar"><i style="width:${Math.round((Math.min(a.step + a.progress, a.path.length - 1) / Math.max(1, a.path.length - 1)) * 100)}%"></i></div>`;
  }
  box.innerHTML = `<button class="x" data-unfollow aria-label="Takibi bırak">×</button><div class="lbl">İzleniyor</div>${h}`;
  box.hidden = false;
}
function setMode(m: Mode) {
  if (!dio) m = '2d';
  if (m === '2d') { stopFollow(); if (cine) setCine(false); $('feed').innerHTML = ''; }
  mode = m;
  try { localStorage.setItem('fd-mode', m); } catch { /* yok */ }
  canvas.hidden = m === '3d';
  dio?.setVisible(m === '3d');
  for (const b of document.querySelectorAll<HTMLButtonElement>('.modes button[data-mode]')) b.setAttribute('aria-pressed', String(b.dataset.mode === m));
  document.body.classList.toggle('mode3d', m === '3d');
  $('tip').hidden = true;
  if (m === '3d') dio!.forceRebuild(); else { cw = 0; terrainCache = null; }
}

function tileInfo(i: number): string {
  const w = sim.w, t = w.tiles[i];
  const out: string[] = [];
  const st = w.settlements.find((x) => x.alive && x.tile === i);
  if (st) {
    const c = w.civs[st.civ];
    out.push(`<b>${esc(st.name)}</b> · ${TIER_TR[st.tier]} · ${esc(c.name)}`);
    out.push(`Nüfus ${sim.pop(st)}/${sim.housing(st)} · ${esc(sim.raceStr(st.pop))}`);
    if (sim.homeless(st)) out.push(`🏚 ${sim.homeless(st)} kişi evsiz, sur dışında barakada`);
    const ws = Object.entries(st.workshops).filter(([, v]) => v).map(([k, v]) => `${WORKSHOPS[k as WorkshopKind].name}${v! > 1 ? ' ×' + v : ''}`);
    const cv = Object.entries(st.civics).filter(([k, v]) => v && !['hut', 'house', 'stonehouse'].includes(k)).map(([k]) => civicName(c, k as never));
    if (ws.length || cv.length) out.push(esc([...cv, ...ws].join(', ')));
    if (st.project) out.push(`İnşa: ${esc(projectName(c, st))} (${Math.round((1 - st.project.left / st.project.total) * 100)}%)`);
    if (st.soldiers) out.push(`${st.soldiers} asker`);
  }
  const inn = w.inns.find((x) => x.tile === i);
  if (inn) out.push(`<b>🍺 ${esc(inn.name)} Hanı</b>${inn.alive ? ` · hancı ${esc(inn.keeper)} · ${innPool(sim, inn).length} serbest kahraman · ${w.quests.filter((q) => q.open && q.inn === inn.id).length} ilan` : ' · harabe'}`);
  const cp = w.camps.find((x) => x.alive && x.tile === i);
  if (cp) out.push(`<b>${esc(cp.name)}</b> · ${cp.count} ${monsterName(cp.kind, false).toLocaleLowerCase('tr')}${cp.boss ? ' + önder' : ''}`);
  if (t.deposit >= 0) {
    const d = w.deposits.find((x) => x.id === t.deposit)!;
    const def = DEPOSITS[d.kind];
    const who = d.knownBy.map((k) => w.civs[k]?.name.split(' ')[0]).join(', ');
    out.push(`<b>${def.name}</b>${d.richness > 1.1 ? ' (zengin)' : d.richness < 0.9 ? ' (fakir)' : ''} · ${def.reserve ? `rezerv ${Math.round(t.reserve)}/${Math.round(def.reserve * d.richness)}` : 'yenilenir'}${def.hiddenUntil ? ` · ${TECH[def.hiddenUntil].name} ile görünür` : ''}`);
    if (who) out.push(`<span style="opacity:.75">Bilen: ${esc(who)}</span>`);
  }
  if (t.ext) {
    const c = sim.settlement(t.ext.settlement) ? w.civs[sim.settlement(t.ext.settlement)!.civ] : undefined;
    const nm = EXTRACTS[t.ext.kind].names[t.ext.level - 1];
    const y = c ? extractYield(sim, c, t) : null;
    out.push(`<b>${esc(nm)}</b> L${t.ext.level} · ${t.ext.workers}/${LEVEL_SLOTS[t.ext.level]} işçi${t.ext.depleted ? ' · TÜKENDİ' : y ? ` · ${(y.y * Math.max(1, t.ext.workers)).toFixed(2)} ${GOODS[y.good].name.toLocaleLowerCase('tr')}/gün` : ''}`);
  }
  for (const a of w.agents.filter((x) => !x.dead && x.path[Math.min(x.step, x.path.length - 1)] === i)) out.push(agentDesc(a));
  out.push(`<span style="opacity:.75">${TERRAIN_TR[t.terrain]}${t.road ? ', yol' : ''}${!st && t.owner >= 0 ? ' · ' + esc(sim.settlement(t.owner)!.name) + ' toprağı' : ''}</span>`);
  return out.join('<br>');
}
function projectName(c: Civ, st: Settlement) {
  const p = st.project!;
  if (p.type === 'civic') return civicName(c, p.kind as never);
  if (p.type === 'workshop') return WORKSHOPS[p.kind as WorkshopKind].name;
  const names = EXTRACTS[p.kind as keyof typeof EXTRACTS].names;
  return p.type === 'upgrade' ? `${names[(p.level ?? 2) - 1]} yükseltmesi` : names[(p.level ?? 1) - 1];
}
function xpBar(h: Hero) {
  if (h.level >= 5) return `<div class="sub mono">XP ${h.xp} · seviye tavanı</div>`;
  const lo = XP_LEVELS[h.level - 1], hi = XP_LEVELS[h.level];
  return `<div class="hp xp"><i style="width:${Math.max(0, Math.min(100, ((h.xp - lo) / (hi - lo)) * 100))}%"></i></div><div class="sub mono">XP ${h.xp}/${hi} → Sv ${h.level + 1}</div>`;
}
/** kahramanın o anki işi, tek satır */
function heroNow(h: Hero, a?: Agent): string {
  const w = sim.w;
  if (h.state === 'dead') return 'öldü';
  if (h.state === 'gone') return 'diyarı terk etti';
  if (h.state === 'retired') return 'emekli';
  if (h.civ >= 0) {
    const k = h.contract ? ` · sözleşme Y${Math.floor(h.contract.until / YEAR) + 1}'e dek` : '';
    return (a ? agentDesc(a) : stateStr(h)) + k;
  }
  if (h.goal) return `${h.goal.kind === 'quest' ? 'ilan' : h.goal.kind === 'hunt' ? 'av' : 'hedef'}: ${h.goal.text}${a ? ' (yolda)' : ''}`;
  if (a) return a.purpose === 'tavern' ? 'yuvasına dönüyor' : agentDesc(a);
  if (h.state === 'tavern') {
    const inn = w.inns.find((i) => i.id === h.tavern);
    const where = inn ? `${inn.name} Hanı'nda` : `${sim.settlement(h.tavern)?.name ?? ''} tavernasında`;
    const bids = h.auction?.bids.length ?? 0;
    return `${where} dinleniyor${h.auction ? ` · açık artırma${bids ? ` (${bids} teklif)` : ''}` : ''}`;
  }
  return stateStr(h);
}
function agentDesc(a: Agent) {
  const civ = a.civ >= 0 ? sim.w.civs[a.civ].name : '';
  const hs = (a.heroes ?? []).map((id) => sim.hero(id).name).join(', ');
  switch (a.kind) {
    case 'caravan': return `Kervan (${esc(civ)}): ${Object.entries(a.cargo ?? {}).map(([k, v]) => `${Math.round(v!)} ${GOODS[k as Good].name.toLocaleLowerCase('tr')}`).join(', ') || 'boş'}`;
    case 'army': return `${a.purpose === 'expedition' ? 'Sefer' : a.purpose === 'plunder' ? 'Akıncılar' : 'Ordu'} (${esc(civ)}): ${a.troops} asker${hs ? ', ' + esc(hs) : ''}${a.returning ? ' · dönüyor' : ''}`;
    case 'raid': return `${monsterName(a.monster ?? 'goblin')}: ${a.troops}${a.boss ? ' + önder' : ''}${a.returning ? ' · kampa dönüyor' : ''}`;
    case 'hero': { const h0 = a.heroes?.length ? sim.hero(a.heroes[0]) : undefined; return a.purpose === 'goal' && h0?.goal ? `${esc(hs)} yolda: ${esc(h0.goal.text)}` : `Kahraman yolda: ${esc(hs)}`; }
    case 'party': return `Macera grubu: ${esc(hs)}${a.returning ? ' · dönüyor' : ''}`;
    case 'settlers': return `Öncüler (${esc(civ)}): ${sim.popSize(a.pop)} kişi`;
    case 'scout': return `Kâşif (${esc(civ)})`;
  }
}

// ---------------------------------------------------------------- paneller
function alignStr(a: { law: number; good: number }) {
  const l = a.law > 0.3 ? 'L' : a.law < -0.3 ? 'C' : 'N', g = a.good > 0.3 ? 'G' : a.good < -0.3 ? 'E' : 'N';
  const code = l === 'N' && g === 'N' ? 'TN' : l + g;
  const tr: Record<string, string> = { L: 'Düzenli', C: 'Kaotik', N: 'Tarafsız', G: 'İyi', E: 'Kötü' };
  return `${code === 'TN' ? 'Tam Tarafsız' : `${tr[l]} ${tr[g]}`} (${code})`;
}
const num = (v: number) => (Math.abs(v) >= 10000 ? Math.round(v / 1000) + 'b' : Math.abs(v) >= 1000 ? (v / 1000).toFixed(1) + 'b' : String(Math.floor(v)));
const swatch = (c: Civ) => `<span class="sw" style="background:${c.color}"></span>`;

function renderCivs(): string {
  const w = sim.w;
  const sel = w.civs[treeCiv] ?? w.civs[0];
  return (sel ? [sel] : []).map((c) => {
    const cls = CLASSES[c.cls];
    const ss = sim.civSettlements(c);
    const pop = sim.civPop(c);
    const wars = w.civs.filter((o) => o.id !== c.id && sim.rel(c.id, o.id).war);
    const heroes = sim.civHeroes(c);
    const soldiers = ss.reduce((a, x) => a + x.soldiers, 0);
    const races: Partial<Record<RaceId, number>> = {};
    for (const s of ss) for (const r in s.pop) races[r as RaceId] = (races[r as RaceId] ?? 0) + (s.pop[r as RaceId] ?? 0);
    const cur = c.research.current ? TECH[c.research.current] : null;
    const sub = cls.subclasses.find((x) => x.id === c.subclass);
    let h = `<section class="civ" id="civ-${c.id}"><h2>${swatch(c)}${esc(c.name)}<span class="era">${ERA_ROMAN[c.era]} · ${ERA_TR[c.era]}</span></h2>`;
    h += `<div class="sub">${RACES[c.race].plural} · ${cls.name} <span class="dnd">(${cls.dnd})</span> · ${alignStr(c.align)}</div>`;
    h += `<div class="chips">${!c.alive ? '<span class="chip dead">Yok oldu</span>' : ''}${wars.map((o) => `<span class="chip war">Savaşta: ${esc(o.name)}</span>`).join('')}<span class="chip feat" title="${esc(cls.featureDesc)}">${esc(cls.feature)}</span>${sub ? `<span class="chip doc" title="${esc(sub.desc)}">${esc(sub.name)}</span>` : '<span class="chip ghost">Alt sınıf seçilmedi</span>'}${(() => { const ws = ss.find((x) => x.civics.wonder || x.project?.kind === 'wonder'); return ws ? `<span class="chip feat" title="${esc(WONDERS[c.cls].desc)}">★ ${esc(WONDERS[c.cls].name)}${ws.civics.wonder ? '' : ` %${Math.round((1 - ws.project!.left / ws.project!.total) * 100)}`}</span>` : ''; })()}${Object.entries(races).map(([r, n]) => `<span class="chip">${RACES[r as RaceId].name} ${n}</span>`).join('')}</div>`;
    h += `<div class="res"><div><b>${pop}</b><span>Nüfus</span></div><div><b>${ss.length}</b><span>Yerleşim</span></div><div><b>${num(sim.st(c, 'gold'))}</b><span>Altın</span></div><div><b>${soldiers}</b><span>Asker</span></div><div><b>${heroes.length}</b><span>Kahraman</span></div><div><b>${c.research.done.length}</b><span>Bilgi</span></div></div>`;
    h += `<div class="lbl">Araştırma</div>`;
    if (cur) {
      h += `<div class="research"><span>${esc(sim.techName(c, cur.id))} <span class="sub">(${cur.tree === 'main' ? 'ana ağaç' : 'sınıf ağacı'}, ${ERA_ROMAN[cur.era]})</span></span><span class="sub">${Math.floor(c.research.progress)}/${researchCost(sim, c, cur)}</span></div>`;
      h += `<div class="bar"><i style="width:${Math.min(100, (c.research.progress / researchCost(sim, c, cur)) * 100)}%"></i></div><div class="why">${esc(c.research.reason)}</div>`;
    } else h += `<div class="sub">${c.era < 4 ? `Sıradaki çağ için ${ERA_RULE[(c.era + 1) as 2 | 3 | 4].pop} nüfuslu bir yerleşim bekleniyor.` : 'Açık düğüm kalmadı.'}</div>`;
    // stok
    const goods = GOOD_IDS.filter((g) => g !== 'gold' && sim.st(c, g) >= 1).sort((a, b) => sim.st(c, b) * GOODS[b].base - sim.st(c, a) * GOODS[a].base).slice(0, 10);
    h += `<div class="lbl">Ambar <span class="hint">fiyat oku: kıtlık ↑ bolluk ↓</span></div><div class="stock">${goods.map((g) => { const p = sim.price(c, g) / GOODS[g].base; return `<span class="good" title="${esc(GOODS[g].name)}: fiyat ${sim.price(c, g).toFixed(1)}"><i style="background:${goodColor(g)}"></i>${esc(GOODS[g].name)} <b>${num(sim.st(c, g))}</b>${p > 1.4 ? '<em class="up">↑</em>' : p < 0.6 ? '<em class="dn">↓</em>' : ''}</span>`; }).join('')}</div>`;
    h += `<div class="lbl">Tehdit</div><div class="threat"><div class="bar"><i style="width:${Math.min(100, c.threat / 1.5 * 100)}%"></i></div><span>${c.threat < 0.2 ? 'sakin' : c.threat < 0.6 ? 'tedirgin' : c.threat < 1 ? 'baskı altında' : 'kuşatılmış'}</span></div>`;
    h += `<div class="lbl">Yerleşimler</div><table class="st">${ss.map((s) => `<tr><td><b>${esc(s.name)}</b> <span class="sub">${TIER_TR[s.tier]}</span><br><span class="sub">${esc(jobStr(s))}</span></td><td class="n">${sim.pop(s)}/${sim.housing(s)}</td></tr>`).join('')}</table>`;
    if (heroes.length) h += `<div class="lbl">Kahramanlar</div><div class="sub">${heroes.map((x) => `${esc(x.name)} (${HERO_CLASS_TR[x.cls]} Sv${x.level}, ${stateStr(x)})`).join(' · ')}</div>`;
    for (const o of w.civs) {
      if (o.id === c.id || !o.alive) continue;
      const r = sim.rel(c.id, o.id);
      if (!r.contact) continue;
      const v = sim.relValue(c.id, o.id);
      h += `<div class="lbl">${esc(o.name)} hakkında</div><div class="rel"><span>${v >= 40 ? 'Dost' : v >= 10 ? 'Olumlu' : v > -10 ? 'Mesafeli' : v > -40 ? 'Gergin' : 'Düşmanca'}${r.treaty ? ` · ${esc(GOODS[r.treaty].name)} antlaşması` : ''}${r.war ? ' · SAVAŞ' : ''}</span><b class="${v >= 0 ? 'pos' : 'neg'}">${v > 0 ? '+' : ''}${v}</b></div>`;
      h += `<ul class="mods">${r.mods.filter((m) => Math.round(m.value)).map((m) => `<li><span>${esc(m.text)}</span><span class="${m.value >= 0 ? 'pos' : 'neg'}">${m.value > 0 ? '+' : ''}${Math.round(m.value)}</span></li>`).join('')}</ul>`;
    }
    return h + '</section>';
  }).join('');
}
function goodColor(g: Good) {
  const dep = Object.values(DEPOSITS).find((d) => d.good === g);
  if (dep) return dep.color;
  return ({ meat: '#a8553a', fish: '#6aa7c9', bread: '#d9a760', beer: '#c99a2e', wood: '#8a6a3d', stone: '#9a978f', planks: '#b58a55', bricks: '#b5563a', leather: '#8f6440', bronze: '#b8743a', steel: '#6f7c8a', tools: '#56616b', arms: '#3b434a', potion: '#57c2a3', scroll: '#d9c8a0', enchanted: '#b48cff', mithrilArmor: '#7fe0ea', fur: '#caa98a' } as Partial<Record<Good, string>>)[g] ?? '#888';
}
function jobStr(s: Settlement) {
  return Object.entries(s.jobs).filter(([, v]) => v).sort((a, b) => b[1] - a[1]).slice(0, 5).map(([k, v]) => `${v} ${GOODS[k as Good]?.name.toLocaleLowerCase('tr') ?? k}`).join(', ');
}
function stateStr(h: Hero) {
  return ({ tavern: 'tavernada', traveling: 'yolda', home: 'evde', quest: 'görevde', dead: 'öldü', gone: 'diyarı terk etti', army: 'seferde', retired: 'emekli' } as const)[h.state];
}

// ---- Ağaç sekmesi
function nodeState(c: Civ, t: TechDef): 'done' | 'cur' | 'avail' | 'gated' | 'locked' {
  if (sim.has(c, t.id)) return 'done';
  if (c.research.current === t.id) return 'cur';
  if (t.era > c.era || !t.req.every((r) => sim.has(c, r))) return 'locked';
  return gateOk(sim, c, t) ? 'avail' : 'gated';
}
function renderTree(): string {
  const w = sim.w;
  const c = w.civs[treeCiv] ?? w.civs[0];
  const cls = CLASSES[c.cls];
  let h = `<div class="picker">${w.civs.map((o) => `<button class="pick" data-civ="${o.id}" aria-pressed="${o.id === c.id}">${swatch(o)}${esc(CLASSES[o.cls].name)}</button>`).join('')}</div>`;
  h += `<p class="sub">${esc(c.name)} · ${ERA_ROMAN[c.era]} ${ERA_TR[c.era]} çağı · ${c.research.done.length} düğüm</p>`;
  h += `<div class="legend2"><span class="nd done">bilinen</span><span class="nd cur">araştırılıyor</span><span class="nd avail">açık</span><span class="nd gated">kaynak eksik</span><span class="nd locked">kilitli</span></div>`;
  const chains: [string, string][] = [['gida', 'Gıda'], ['metal', 'Metal'], ['yapi', 'Yapı, savunma'], ['ticaret', 'Ticaret'], ['bilgi', 'Bilgi, büyü']];
  h += `<div class="tree">`;
  for (let era = 1; era <= 4; era++) {
    const reached = c.era >= era;
    const rule = era > 1 ? ERA_RULE[era as 2 | 3 | 4] : null;
    h += `<div class="ecol ${reached ? '' : 'future'}"><div class="ehead">${ERA_ROMAN[era]} · ${ERA_TR[era]}${c.eraDay[era] !== undefined && era > 1 ? `<small>${sim.dateStr(c.eraDay[era])}</small>` : rule && !reached ? `<small>${rule.nodes} düğüm + ${rule.pop} nüfus</small>` : ''}</div>`;
    for (const [ch] of chains) {
      for (const t of MAIN_TECHS.filter((x) => x.era === era && x.chain === ch)) h += nodeHtml(c, t);
    }
    h += `</div>`;
  }
  h += `</div>`;
  const sub = cls.subclasses.find((x) => x.id === c.subclass);
  h += `<div class="lbl">${esc(cls.name)} ağacı ${sub ? `· ${esc(sub.name)}` : ''}</div>`;
  if (sub) h += `<p class="sub">${esc(sub.desc)}. Doruk: <b>${esc(sub.capName)}</b> — ${esc(sub.capDesc)}.</p>`;
  else h += `<p class="sub">Alt sınıf seçenekleri: ${cls.subclasses.map((x) => esc(x.name)).join(' · ')}</p>`;
  h += `<div class="tree">`;
  for (let era = 2; era <= 4; era++) {
    h += `<div class="ecol ${c.era >= era ? '' : 'future'}"><div class="ehead">${ERA_ROMAN[era]} · ${ERA_TR[era]}</div>`;
    for (const t of CLASS_TECHS.filter((x) => x.tree === c.cls && x.era === era)) h += nodeHtml(c, t);
    h += `</div>`;
  }
  h += `</div>`;
  return h;
}
function nodeHtml(c: Civ, t: TechDef) {
  const st = nodeState(c, t);
  const pct = st === 'cur' ? Math.min(100, (c.research.progress / researchCost(sim, c, t)) * 100) : 0;
  const gate = st === 'gated' ? ` · eksik: ${[...(t.gate ?? [])].filter((k) => !sim.access(c, k)).map(gateName).join(', ')}` : '';
  const title = `${t.unlock}${t.req.length ? ` · önkoşul: ${t.req.map((r) => TECH[r].name).join(', ')}` : ''}${t.gate ? ` · kaynak: ${t.gate.map(gateName).join(', ')}` : ''}${gate}`;
  return `<div class="nd ${st}" title="${esc(title)}"${st === 'cur' ? ` style="--p:${pct}%"` : ''}>${esc(sim.techName(c, t.id))}${t.unit ? ` <small>★ ${esc(UNITS[t.unit].name)}</small>` : ''}${st === 'gated' ? `<small>${esc(gate.slice(3))}</small>` : ''}</div>`;
}
function gateName(k: string) {
  return ({ water: 'su', fertile: 'verimli ova', clay: 'kil', copper: 'bakır', tin: 'kalay', iron: 'demir', coal: 'kömür', gold: 'altın', mana: 'mana', mithril: 'mithril', horses: 'at', herbs: 'şifalı ot', heartwood: 'kadim ağaç' } as Record<string, string>)[k] ?? k;
}

// ---- Karşılaştırma sekmesi
function renderCompare(): string {
  const w = sim.w;
  const civs = w.civs.slice().sort((a, b) => sim.civPop(b) - sim.civPop(a));
  let h = `<p class="sub">Medeniyetler nüfusa göre sıralı. Bir satıra tıkla, ağacını gör.</p>`;
  h += popChart();
  h += `<div class="scroll"><table class="cmp"><thead><tr><th>Medeniyet</th><th class="c">Çağ</th><th class="n">Nüfus</th><th class="n" title="Yerleşim">Yer.</th><th class="n" title="Tamamlanan araştırma">Bilgi</th><th class="n">Asker</th><th class="n" title="Kahraman">Kahr.</th><th class="n">Altın</th><th class="n" title="Kervan seferi">Kerv.</th><th class="n" title="Tüketilen maden yatağı">Tük.</th><th class="n" title="Kazanılan / kaybedilen savaş">K/K</th><th>Harika</th></tr></thead><tbody>`;
  for (const c of civs) {
    const sol = sim.civSettlements(c).reduce((a, x) => a + x.soldiers, 0);
    h += `<tr data-civ="${c.id}" class="${c.alive ? '' : 'deadrow'}"><td>${swatch(c)}<b>${esc(CLASSES[c.cls].name)}</b><br><span class="sub">${esc(sim.subclassName(c) ?? '—')}</span></td><td class="c">${ERA_ROMAN[c.era]}</td><td class="n">${sim.civPop(c)}</td><td class="n">${sim.civSettlements(c).length}</td><td class="n">${c.research.done.length}</td><td class="n">${sol}</td><td class="n">${sim.civHeroes(c).length}</td><td class="n">${num(sim.st(c, 'gold'))}</td><td class="n">${c.stats.traded}</td><td class="n">${c.stats.depleted}</td><td class="n">${c.stats.battlesWon}/${c.stats.battlesLost}</td><td>${(() => { const ws = sim.civSettlements(c).find((x) => x.civics.wonder || x.project?.kind === 'wonder'); return ws ? (ws.civics.wonder ? esc(WONDERS[c.cls].name) : `<span class="sub">${esc(WONDERS[c.cls].name)} %${Math.round((1 - ws.project!.left / ws.project!.total) * 100)}</span>`) : '—'; })()}</td></tr>`;
  }
  h += `</tbody></table></div>`;
  const inf = w.camps.filter((c) => c.alive);
  h += `<div class="lbl">Canavar istilası</div><p class="sub">${inf.length ? inf.map((c) => `${esc(c.name)} (${c.count} ${monsterName(c.kind, false).toLocaleLowerCase('tr')})`).join(' · ') : 'Şu an aktif kamp yok.'}</p>`;
  return h;
}
function popChart(): string {
  const w = sim.w;
  const W = 440, H = 160, L = 30, R = 132, T = 10, B = 22;
  const series = w.civs.map((c) => ({ c, pts: [{ day: 0, pop: 6 }, ...c.history.map((x) => ({ day: x.day, pop: x.pop })), { day: sim.day, pop: sim.civPop(c) }] }));
  const maxDay = Math.max(120, sim.day), maxPop = Math.max(20, ...series.flatMap((s) => s.pts.map((p) => p.pop)));
  const step = maxPop > 200 ? 100 : maxPop > 80 ? 50 : maxPop > 40 ? 20 : 10;
  const top = Math.ceil(maxPop / step) * step;
  const X = (d: number) => L + (d / maxDay) * (W - L - R), Y = (p: number) => T + (1 - p / top) * (H - T - B);
  let g = '';
  for (let v = 0; v <= top; v += step) g += `<line x1="${L}" x2="${W - R}" y1="${Y(v)}" y2="${Y(v)}" class="grid"/><text x="${L - 6}" y="${Y(v) + 4}" text-anchor="end" class="tick">${v}</text>`;
  const years = Math.floor(maxDay / YEAR);
  const ystep = years > 10 ? 5 : years > 4 ? 2 : 1;
  for (let y = 0; y <= years; y += ystep) g += `<text x="${X(y * YEAR)}" y="${H - 6}" text-anchor="middle" class="tick">Y${y + 1}</text>`;
  // etiket çakışmasını önle
  const ends = series.map((s) => ({ c: s.c, y: Y(s.pts[s.pts.length - 1].pop) })).sort((a, b) => a.y - b.y);
  for (let k = 1; k < ends.length; k++) if (ends[k].y - ends[k - 1].y < 11) ends[k].y = ends[k - 1].y + 11;
  const lines = series.map((s) => `<polyline fill="none" stroke="${s.c.color}" stroke-width="2" stroke-linejoin="round" points="${s.pts.map((p) => `${X(p.day).toFixed(1)},${Y(p.pop).toFixed(1)}`).join(' ')}"/><circle cx="${X(sim.day)}" cy="${Y(s.pts[s.pts.length - 1].pop)}" r="3" fill="${s.c.color}" stroke="var(--panel)" stroke-width="2"/>`).join('');
  const labels = ends.map((e) => `<text x="${W - R + 6}" y="${e.y + 4}" class="lab">${esc(CLASSES[e.c.cls].name)} ${sim.civPop(e.c)}</text>`).join('');
  return `<figure class="chart"><figcaption>Nüfus, yıllara göre</figcaption><svg viewBox="0 0 ${W} ${H}" role="img" aria-label="Medeniyetlerin nüfus eğrileri">${g}${lines}${labels}</svg></figure>`;
}

function renderHeroes(): string {
  const w = sim.w;
  if (!w.heroes.length) return `<p class="empty">Henüz kahraman yok. Hanlarda ve tavernalarda doğarlar.</p>`;
  const order = (h: Hero) => (h.state === 'dead' ? 4 : h.state === 'gone' ? 3 : h.state === 'retired' ? 2 : h.civ >= 0 ? 0 : 1);
  const live = w.heroes.filter((h) => h.state !== 'dead' && h.state !== 'gone');
  let out = `<div class="sub">${live.filter((h) => h.civ === -1 && h.state !== 'retired').length} serbest · ${live.filter((h) => h.civ >= 0).length} kiralı · ${live.filter((h) => h.state === 'retired').length} emekli · ${w.heroes.filter((h) => h.state === 'dead').length} mezar</div>`;
  out += w.heroes.slice().sort((a, b) => order(a) - order(b) || b.level - a.level || b.xp - a.xp).slice(0, 80).map((h) => {
    const aff = h.civ >= 0 ? `<span style="color:${civColor(h.civ)}">■</span> ${esc(w.civs[h.civ].name)}` : 'Serbest';
    const canFollow = dio && h.state !== 'dead' && h.state !== 'gone';
    const a = w.agents.find((q) => !q.dead && q.heroes?.includes(h.id));
    let x = `<div class="hero ${h.state === 'dead' || h.state === 'gone' ? 'dead' : ''}${h.legend ? ' legend' : ''}"><h3><span>${esc(heroLabel(h))}${canFollow ? ` <button class="btn sm" data-follow-hero="${h.id}">İzle</button>` : ''}</span><small>Sv ${h.level}</small></h3>`;
    x += `<div class="sub">${RACES[h.race].name} ${HERO_CLASS_TR[h.cls]} · ${ALIGN_TR[h.align]} · ${aff}${h.kills ? ` · ${h.kills} leş` : ''}${h.gold ? ` · ${h.gold} altın` : ''}${h.revived ? ' · dirildi' : ''}</div>`;
    x += `<div class="chips"><span class="chip path">${PATH_TR[h.path]}${h.civ >= 0 ? ' (askıda)' : ''}</span>${h.traits.map((t) => `<span class="chip trait" title="${esc(TRAITS[t].desc)}">${TRAITS[t].icon} ${TRAITS[t].name}</span>`).join('')}${h.contract ? `<span class="chip">${CONTRACT_YEARS} yıllık sözleşme</span>` : ''}</div>`;
    x += `<div class="sub">Şu an: ${esc(heroNow(h, a))}</div>`;
    if (h.state !== 'dead') x += `<div class="hp"><i style="width:${(h.hp / h.maxHp) * 100}%"></i></div><div class="sub mono">HP ${h.hp}/${h.maxHp} · AC ${h.ac}${h.bonus.atk ? ` · +${h.bonus.atk} saldırı` : ''}</div>${xpBar(h)}`;
    x += `<div class="stats">${STATS.map((st) => `<div><b>${h.stats[st]}</b>${st.toUpperCase()}</div>`).join('')}</div>`;
    if (h.journal.length) x += `<div class="journal">${h.journal.slice(-3).reverse().map((j) => `<div><span class="mono">${sim.dateStr(j.day).replace('Yıl ', 'Y')}</span> ${esc(j.text)}</div>`).join('')}</div>`;
    x += `<div class="sub">${esc(h.bio.charAt(0).toUpperCase() + h.bio.slice(1))}${h.state === 'dead' && h.deathDay !== undefined ? ` Öldü: ${sim.dateStr(h.deathDay)}.` : ''}</div></div>`;
    return x;
  }).join('');
  return out;
}

function renderInns(): string {
  const w = sim.w;
  if (!w.inns.length) return `<p class="empty">Bu dünyada tarafsız han yok.</p>`;
  const banned = w.civs.filter((c) => (c.innBanUntil ?? 0) > sim.day);
  let out = `<p class="sub">Hanlar kimseye ait değildir. Serbest kahramanlar burada doğar, kendi yollarını izler; medeniyetler onları açık artırmayla ${CONTRACT_YEARS} yıllığına kiralar.</p>`;
  if (banned.length) out += `<div class="chips">${banned.map((c) => `<span class="chip war">Han Bozan: ${esc(c.name)} (Y${Math.floor(c.innBanUntil! / YEAR) + 1}'e dek)</span>`).join('')}</div>`;
  const hires = w.civs.filter((c) => c.alive).map((c) => ({ c, n: innHeroesOf(sim, c).length })).filter((x) => x.n);
  if (hires.length) out += `<div class="sub">Sözleşmeli: ${hires.map((x) => `<span style="color:${civColor(x.c.id)}">■</span> ${esc(x.c.name)} ${x.n}`).join(' · ')}</div>`;
  for (const inn of w.inns) out += renderInn(inn);
  return out;
}
function renderInn(inn: Inn): string {
  const w = sim.w;
  let h = `<div class="inn ${inn.alive ? '' : 'dead'}"><h3><span>🍺 ${esc(inn.name)} Hanı</span><small>${inn.alive ? `${Math.round(inn.gold)} altın` : 'harabe'}</small></h3>`;
  const teacher = inn.teacher !== undefined ? w.heroes.find((x) => x.id === inn.teacher) : undefined;
  const graves = w.heroes.filter((x) => x.state === 'dead' && x.baseInn && x.base === inn.id).length;
  h += `<div class="sub">${inn.alive ? `Hancı ${esc(inn.keeper)}` : `Yıkıldı: ${sim.dateStr(inn.ruinedDay ?? 0)}`}${teacher && teacher.state === 'retired' ? ` · öğretmen ${esc(heroLabel(teacher))}` : ''}${graves ? ` · mezarlıkta ${graves} kahraman` : ''}${inn.raids ? ` · ${inn.raids} baskın gördü` : ''}</div>`;
  const quests = w.quests.filter((q) => q.open && q.inn === inn.id);
  h += `<div class="lbl">Pano</div>`;
  h += quests.length ? `<ul class="board">${quests.map((q) => {
    const cp = w.camps.find((c) => c.id === q.camp);
    const who = q.civ >= 0 ? `<span style="color:${civColor(q.civ)}">■</span> ${esc(w.civs[q.civ].name)}` : `Hancı`;
    return `<li><b>${esc(cp?.name ?? '?')}</b> · ${q.bounty} altın · ${who}${q.failures ? ` · ${q.failures} başarısız` : ''}${q.expires ? ` · Y${Math.floor(q.expires / YEAR) + 1}'e dek` : ''}</li>`;
  }).join('')}</ul>` : `<div class="sub">Açık ilan yok.</div>`;
  const pool = innPool(sim, inn);
  h += `<div class="lbl">Serbest kahramanlar (${pool.length})</div>`;
  h += pool.length ? pool.sort((a, b) => b.level - a.level).map((x) => {
    const a = w.agents.find((q) => !q.dead && q.heroes?.includes(x.id));
    const bids = (x.auction?.bids ?? []).slice().sort((p, q) => q.gold - p.gold);
    return `<div class="pool"><div><b>${esc(heroLabel(x))}</b> <span class="sub">Sv ${x.level} ${RACES[x.race].name} ${HERO_CLASS_TR[x.cls]} · ${ALIGN_TR[x.align]} · ${PATH_TR[x.path]}</span>${dio ? ` <button class="btn sm" data-follow-hero="${x.id}">İzle</button>` : ''}</div>`
      + `<div class="sub">${esc(heroNow(x, a))}</div>`
      + (bids.length ? `<div class="sub mono">Teklifler: ${bids.map((b) => `<span style="color:${civColor(b.civ)}">■</span>${b.gold}`).join(' ')} · kapanış ${sim.dateStr(x.auction!.end)}</div>` : '')
      + `</div>`;
  }).join('') : `<div class="sub">Han boş.</div>`;
  const evs = w.events.filter((e) => e.text.includes(inn.name)).slice(-4).reverse();
  if (evs.length) h += `<div class="lbl">Son olaylar</div><div class="journal">${evs.map((e) => `<div><span class="mono">${sim.dateStr(e.day).replace('Yıl ', 'Y')}</span> ${esc(e.text)}</div>`).join('')}</div>`;
  return h + '</div>';
}

function renderBattle(b: Battle): string {
  let h = `<div class="battle"><h3><span>${esc(b.title)}</span><button class="btn close" data-close="1">Kapat</button></h3>`;
  h += `<div class="sub">${sim.dateStr(b.day)} · ${esc(b.sideA)} (${b.lossesA} kayıp) karşı ${esc(b.sideB)} (${b.lossesB} kayıp)</div><div><b>Kazanan:</b> ${esc(b.winner === 'A' ? b.sideA : b.sideB)}</div>`;
  if (b.lines.length) h += `<ul>${b.lines.slice(0, 14).map((l) => `<li class="${l.crit ? 'crit' : l.fumble ? 'fumble' : ''}">${esc(l.t)}</li>`).join('')}</ul>`;
  if (b.rolls.length) h += `<div class="lbl">d20 atışları</div><div class="dice">${b.rolls.slice(0, 40).map((r) => `<span class="die ${r.side === 'A' ? 'a' : 'b'} ${r.d20 === 20 ? 'n20' : r.d20 === 1 ? 'n1' : ''}" title="${esc(r.who)}">${r.d20}</span>`).join('')}</div>`;
  return h + '</div>';
}
function renderLog(): string {
  const w = sim.w;
  let h = `<div class="filter"><label><input type="checkbox" id="major" ${onlyMajor ? 'checked' : ''}> Yalnızca önemli olaylar</label></div>`;
  if (openBattle !== null) { const b = w.battles.find((x) => x.id === openBattle); if (b) h += renderBattle(b); }
  const evs = w.events.filter((e) => !onlyMajor || e.major || e.battle).slice(-250).reverse();
  h += evs.map((e) => {
    const b = e.battle ? w.battles.find((x) => x.id === e.battle) : undefined;
    const crit = b ? b.rolls.filter((r) => r.d20 === 20).length : 0;
    const c = e.civ !== undefined ? w.civs[e.civ] : undefined;
    return `<button class="ev ${e.major ? 'major' : ''} k-${e.kind}" data-ev="${e.id}"${c ? ` style="--cc:${c.color}"` : ''}><time>${sim.dateStr(e.day)}</time><span class="t">${esc(e.text)}</span>${e.cause ? `<div class="why">${esc(e.cause)}</div>` : ''}${b ? `<span class="d20">⚔ savaş raporu${crit ? ` · ${crit}× nat 20` : ''}</span>` : ''}</button>`;
  }).join('');
  return h;
}

// ---- oyun içi HUD: sağ üst armalar + açılır rapor paneli (Civ tarzı)
const TAB_TITLE: Record<Tab, string> = { civ: 'Medeniyetler', tree: 'Araştırma Ağacı', compare: 'Kıyas', hero: 'Kahramanlar', inn: 'Hanlar', log: 'Kronik' };
const REPORT_KEYS: Record<string, Tab> = { '1': 'civ', '2': 'tree', '3': 'compare', '4': 'hero', '5': 'inn', '6': 'log' };
const drawer = $('drawer');
const civbar = $('civbar');
const civtip = $('civtip');
let civbarKey = '';
let drawerHeadKey = '';

function renderPanel() {
  dirty = false;
  if (!drawerOpen) return;
  const pane = $('pane');
  const st = pane.scrollTop;
  pane.innerHTML = tab === 'civ' ? renderCivs() : tab === 'tree' ? renderTree() : tab === 'compare' ? renderCompare() : tab === 'hero' ? renderHeroes() : tab === 'inn' ? renderInns() : renderLog();
  pane.scrollTop = st;
  renderDrawerHead();
}
function renderDrawerHead() {
  const c = sim.w.civs[treeCiv] ?? sim.w.civs[0];
  const key = tab === 'civ' && c ? `civ:${c.id}:${c.era}:${c.alive}` : tab;
  if (key === drawerHeadKey) return;
  drawerHeadKey = key;
  drawer.dataset.tab = tab;
  $('drawer-title').innerHTML = tab === 'civ' && c
    ? `<span class="med${c.alive ? '' : ' dead'}" style="--cc:${c.color}">${classIcon(c.cls)}</span><span class="t">${esc(c.name)}</span><span class="era">${ERA_ROMAN[c.era]} · ${ERA_TR[c.era]}</span><span class="drawer-nav"><button data-civstep="-1" aria-label="Önceki medeniyet" title="Önceki">${uiIcon('prev')}</button><button data-civstep="1" aria-label="Sonraki medeniyet" title="Sonraki">${uiIcon('next')}</button></span>`
    : `${uiIcon(tab)}<span class="t">${TAB_TITLE[tab]}</span>`;
}
/** tarih, rapor düğmeleri ve armalar — panel kapalıyken de güncellenir */
function renderHud() {
  const d = sim.day;
  const tod = mode === '3d' && dio ? dio.timeOfDay() : '';
  $('date').innerHTML = `<b>Yıl ${Math.floor(d / YEAR) + 1}</b> · ${SEASONS[Math.floor((d % YEAR) / (YEAR / 4))]} · gün ${d}${tod ? `<span class="tod"><i> · </i>${tod}</span>` : ''}`;
  for (const b of document.querySelectorAll<HTMLButtonElement>('.rbtn')) b.setAttribute('aria-pressed', String(drawerOpen && b.dataset.tab === tab));
  const w = sim.w;
  const key = w.civs.map((c) => `${c.id}:${c.cls}:${c.color}`).join('|');
  if (key !== civbarKey) {
    civbarKey = key;
    civbar.innerHTML = w.civs.map((c) => `<button class="med" data-med="${c.id}" style="--cc:${c.color}" aria-label="${esc(c.name)}" aria-pressed="false">${classIcon(c.cls)}<span class="era"></span><span class="war" hidden>${uiIcon('swords')}</span></button>`).join('');
  }
  for (const b of civbar.querySelectorAll<HTMLButtonElement>('.med')) {
    const c = w.civs[Number(b.dataset.med)];
    if (!c) continue;
    const war = c.alive && w.civs.some((o) => o.id !== c.id && o.alive && sim.rel(c.id, o.id).war);
    b.classList.toggle('dead', !c.alive);
    b.setAttribute('aria-pressed', String(drawerOpen && (tab === 'civ' || tab === 'tree') && treeCiv === c.id));
    const era = b.querySelector<HTMLElement>('.era')!;
    const roman = c.alive ? ERA_ROMAN[c.era] : '†';
    if (era.textContent !== roman) era.textContent = roman;
    b.querySelector<HTMLElement>('.war')!.hidden = !war;
  }
}
function civTipHtml(c: Civ): string {
  const cls = CLASSES[c.cls];
  const ss = sim.civSettlements(c);
  const wars = sim.w.civs.filter((o) => o.id !== c.id && o.alive && sim.rel(c.id, o.id).war);
  const cur = c.research.current ? TECH[c.research.current] : null;
  let h = `<b class="n">${esc(c.name)}</b><div class="s">${RACES[c.race].plural} · ${esc(cls.name)} · ${ERA_ROMAN[c.era]} ${ERA_TR[c.era]}</div>`;
  if (!c.alive) return h + '<div class="w">Yok oldu</div>';
  h += `<div class="g"><div><b>${sim.civPop(c)}</b><span>Nüfus</span></div><div><b>${ss.length}</b><span>Yerleşim</span></div><div><b>${num(sim.st(c, 'gold'))}</b><span>Altın</span></div><div><b>${ss.reduce((a, x) => a + x.soldiers, 0)}</b><span>Asker</span></div></div>`;
  if (cur) h += `<div class="r">Araştırıyor: ${esc(sim.techName(c, cur.id))}</div>`;
  if (wars.length) h += `<div class="w">Savaşta: ${wars.map((o) => esc(o.name)).join(', ')}</div>`;
  return h + '<div class="s" style="margin-top:4px">Tıkla: ayrıntılar</div>';
}
function openDrawer(t: Tab, civ?: number) {
  const pane = $('pane');
  if ((civ !== undefined && civ !== treeCiv) || t !== tab || !drawerOpen) pane.scrollTop = 0;
  if (civ !== undefined) treeCiv = civ;
  tab = t;
  drawerOpen = true;
  drawer.hidden = false;
  document.body.classList.add('drawer-open');
  document.body.dataset.drawer = t;
  drawerHeadKey = '';
  renderPanel();
  renderHud();
}
function closeDrawer() {
  drawerOpen = false;
  drawer.hidden = true;
  paneHover = false;
  document.body.classList.remove('drawer-open');
  delete document.body.dataset.drawer;
  renderHud();
}
function toggleTab(t: Tab) { if (drawerOpen && tab === t) closeDrawer(); else openDrawer(t); }

let paneHover = false;
const pane = $('pane');
pane.addEventListener('pointerenter', () => { paneHover = true; });
pane.addEventListener('pointerleave', () => { paneHover = false; });
pane.addEventListener('click', (e) => {
  const el = e.target as HTMLElement;
  if (el.closest('[data-close]')) { openBattle = null; renderPanel(); return; }
  const fh = el.closest<HTMLElement>('[data-follow-hero]');
  if (fh && dio) { if (mode !== '3d') setMode('3d'); dio.startFollow({ kind: 'hero', id: Number(fh.dataset.followHero) }); renderFollow(); return; }
  const pick = el.closest<HTMLElement>('[data-civ]');
  if (pick) { openDrawer('tree', Number(pick.dataset.civ)); return; }
  const ev = el.closest<HTMLElement>('[data-ev]');
  if (ev) {
    const g = sim.w.events.find((x) => x.id === Number(ev.dataset.ev));
    if (!g) return;
    if (g.tile !== undefined) { fx.push({ tile: g.tile, color: '#ffffff', t0: performance.now(), dur: 1500, ring: true }); if (mode === '3d' && dio) { stopFollow(); dio.focusTile(g.tile); } }
    if (g.battle) { openBattle = g.battle; pane.scrollTop = 0; renderPanel(); }
  }
});
pane.addEventListener('change', (e) => { const el = e.target as HTMLInputElement; if (el.id === 'major') { onlyMajor = el.checked; renderPanel(); } });
drawer.addEventListener('click', (e) => {
  const el = e.target as HTMLElement;
  if (el.closest('[data-closedrawer]')) { closeDrawer(); return; }
  const step = el.closest<HTMLElement>('[data-civstep]');
  if (step) {
    const n = sim.w.civs.length;
    if (n) openDrawer('civ', (((treeCiv + Number(step.dataset.civstep)) % n) + n) % n);
  }
});
for (const b of document.querySelectorAll<HTMLButtonElement>('.rbtn')) {
  b.innerHTML = uiIcon(b.dataset.tab as Tab);
  b.addEventListener('click', (e) => { toggleTab(b.dataset.tab as Tab); if (e.detail > 0) b.blur(); });
}
$('optbtn').innerHTML = uiIcon('gear');
$('helpbtn').innerHTML = uiIcon('help');
document.addEventListener('pointerdown', (e) => { const vo = document.querySelector<HTMLDetailsElement>('.viewopts'); if (vo?.open && !vo.contains(e.target as Node)) vo.open = false; });
civbar.addEventListener('click', (e) => {
  const b = (e.target as HTMLElement).closest<HTMLElement>('[data-med]');
  if (!b) return;
  const id = Number(b.dataset.med);
  civtip.hidden = true;
  if (e.detail > 0) b.blur();
  if (drawerOpen && (tab === 'civ' || tab === 'tree') && treeCiv === id) closeDrawer();
  else openDrawer(drawerOpen && tab === 'tree' ? 'tree' : 'civ', id);
});
civbar.addEventListener('pointerover', (e) => {
  const b = (e.target as HTMLElement).closest<HTMLElement>('[data-med]');
  if (!b) return;
  const c = sim.w.civs[Number(b.dataset.med)];
  if (!c) return;
  civtip.innerHTML = civTipHtml(c);
  civtip.style.setProperty('--cc', c.color);
  civtip.hidden = false;
  const sr = $('stage').getBoundingClientRect(), br = b.getBoundingClientRect();
  const x = Math.max(6, Math.min(sr.width - civtip.offsetWidth - 6, br.left - sr.left + br.width / 2 - civtip.offsetWidth / 2));
  civtip.style.left = x + 'px';
  civtip.style.top = (br.bottom - sr.top + 14) + 'px';
});
civbar.addEventListener('pointerout', () => { civtip.hidden = true; });

// ---------------------------------------------------------------- kontroller
const speedsEl = $('speeds');
speedsEl.innerHTML = SPEEDS.map((s) => `<button class="btn" data-speed="${s}" aria-pressed="${s === speed}">${s}×</button>`).join('');
speedsEl.addEventListener('click', (e) => {
  const b = (e.target as HTMLElement).closest<HTMLButtonElement>('[data-speed]');
  if (!b) return;
  speed = Number(b.dataset.speed);
  for (const x of speedsEl.querySelectorAll('button')) x.setAttribute('aria-pressed', String(x === b));
  if (paused) togglePause();
});
function togglePause() { paused = !paused; $('play').textContent = paused ? 'Devam' : 'Duraklat'; $('play').setAttribute('aria-pressed', String(paused)); }
$('play').addEventListener('click', togglePause);
$('reset').addEventListener('click', () => { const v = Number(($('seed') as HTMLInputElement).value); newWorld(v > 0 && v !== seed ? v : Math.floor(Math.random() * 99999) + 1); });
($('seed') as HTMLInputElement).addEventListener('keydown', (e) => { if (e.key === 'Enter') { const v = Number((e.target as HTMLInputElement).value); if (v > 0) newWorld(v); } });
($('deps') as HTMLInputElement).addEventListener('change', (e) => { showDeposits = (e.target as HTMLInputElement).checked; if (dio) { dio.showDeposits = showDeposits; dio.forceRebuild(); } });
($('tilt') as HTMLInputElement).addEventListener('change', (e) => { if (dio) dio.tiltShift = (e.target as HTMLInputElement).checked; });
($('grid') as HTMLInputElement).addEventListener('change', (e) => { if (dio) dio.setGrid((e.target as HTMLInputElement).checked); });
($('daynight') as HTMLInputElement).addEventListener('change', (e) => { if (dio) dio.dayNight = (e.target as HTMLInputElement).checked; });
$('zin').addEventListener('click', () => (mode === '3d' ? dio!.dolly(0.75) : setZoom(zoom * 1.4)));
$('zout').addEventListener('click', () => (mode === '3d' ? dio!.dolly(1.33) : setZoom(zoom / 1.4)));
for (const b of document.querySelectorAll<HTMLButtonElement>('.modes button')) b.addEventListener('click', () => {
  if (b.dataset.mode) setMode(b.dataset.mode as Mode);
  else if (b.dataset.view) dio?.view(b.dataset.view as 'top' | 'diorama' | 'low');
  else if (b.dataset.rot) dio?.orbit(Number(b.dataset.rot) * 0.5, 0);
});
window.addEventListener('keydown', (e) => { if (e.code === 'Space' && !(e.target instanceof HTMLInputElement) && !(e.target instanceof HTMLButtonElement)) { e.preventDefault(); togglePause(); } });
window.addEventListener('resize', () => { cw = 0; });
window.addEventListener('keydown', (e) => {
  if (e.key === 'Escape') { const vo = document.querySelector<HTMLDetailsElement>('.viewopts'); if (vo?.open) vo.open = false; else if (!$('help').hidden) $('help').hidden = true; else if (drawerOpen) closeDrawer(); else stopFollow(); }
  const tgt = e.target;
  const typing = (tgt instanceof HTMLInputElement && tgt.type !== 'checkbox') || tgt instanceof HTMLSelectElement;
  if (!typing && !e.repeat && !e.ctrlKey && !e.metaKey && !e.altKey) {
    const k = REPORT_KEYS[e.key];
    if (k) toggleTab(k);
  }
  if (e.key === '?' || (e.key === 'h' && !(e.target instanceof HTMLInputElement))) $('help').hidden = !$('help').hidden;
});
// ortam sesi
const amb = new Ambience();
let lastSnd = 0;
function sndUI() { const b = $('snd'); b.innerHTML = uiIcon(amb.enabled ? 'sound' : 'mute'); b.setAttribute('aria-pressed', String(amb.enabled)); b.title = amb.enabled ? 'Ortam sesini kapat (M)' : 'Ortam sesini aç (M)'; }
function sndToggle() { void amb.setEnabled(!amb.enabled); try { localStorage.setItem('fd-sound', amb.enabled ? '1' : '0'); } catch { /* yok */ } sndUI(); }
$('snd').addEventListener('click', sndToggle);
window.addEventListener('keydown', (e) => { if (e.key === 'm' && !(e.target instanceof HTMLInputElement)) sndToggle(); });
try { if (localStorage.getItem('fd-sound') === '1') { amb.enabled = true; window.addEventListener('pointerdown', () => { void amb.setEnabled(true); }, { once: true }); } } catch { /* yok */ }
sndUI();
$('helpbtn').addEventListener('click', () => { $('help').hidden = !$('help').hidden; });
$('help').addEventListener('click', (e) => { if ((e.target as HTMLElement).closest('[data-closehelp]') || e.target === $('help')) $('help').hidden = true; });
$('follow').addEventListener('click', (e) => { if ((e.target as HTMLElement).closest('[data-unfollow]')) stopFollow(); });
$('feed').addEventListener('click', (e) => { const it = (e.target as HTMLElement).closest<HTMLElement>('[data-tile]'); if (it && dio) { stopFollow(); if (cine) cinePause = performance.now() + 25000; dio.shot(Number(it.dataset.tile), 12, 0.95); } });
$('cine').addEventListener('click', () => setCine(!cine));
const QN = ['Düşük', 'Orta', 'Yüksek'];
($('quality') as HTMLSelectElement).addEventListener('change', (e) => {
  const v = (e.target as HTMLSelectElement).value;
  try { localStorage.setItem('fd-quality', v); } catch { /* yok */ }
  if (!dio) return;
  dio.autoQuality = v === 'auto';
  dio.setQuality(v === 'auto' ? 2 : Number(v));
});
if (dio) {
  let qv = 'auto';
  try { qv = localStorage.getItem('fd-quality') ?? 'auto'; } catch { /* yok */ }
  ($('quality') as HTMLSelectElement).value = qv;
  dio.autoQuality = qv === 'auto';
  if (qv !== 'auto') dio.setQuality(Number(qv));
  let lastQ = dio.quality;
  dio.onQuality = (q, auto) => {
    if (auto) feedPush({ id: -1, day: sim.day, kind: 'world', text: q < lastQ ? `Görüntü kalitesi ${QN[q].toLocaleLowerCase('tr')} seviyeye indirildi (kare hızı düşüktü).` : `Kare hızı rahatladı: görüntü kalitesi ${QN[q].toLocaleLowerCase('tr')} seviyeye çıkarıldı.`, major: true });
    lastQ = q;
  };
}
let lastFps = 0;
if (dio) dio.onFollowEnd = () => { $('follow').hidden = true; };
let lastFollow = 0;

let last = performance.now(), acc = 0, lastPanel = 0, lastHud = 0;
// bayramlar: başkentlerde başlayınca akışa ve sinemaya düşer (yalnız görsel takvim)
let festSeen = new Set<number>(), festDay = -1;
function festivalTick() {
  if (!dio || sim.day === festDay) return;
  festDay = sim.day;
  const now = new Set<number>();
  const fresh: { f: string; st: Settlement; civ: number }[] = [];
  for (const c of sim.w.civs) {
    const st = c.alive ? sim.capital(c) : undefined;
    if (!st || st.tier < 2) continue;
    const f = dio.festival(st);
    if (!f) continue;
    now.add(st.id);
    if (!festSeen.has(st.id)) fresh.push({ f, st, civ: c.id });
  }
  festSeen = now;
  // aynı gün başlayan bayramlar akışta tek satır olsun (üç ayrı kart kalabalık yapıyordu)
  if (fresh.length) {
    const names = fresh.map((x) => x.st.name);
    const list = names.length === 1 ? names[0] : names.slice(0, -1).join(', ') + ' ve ' + names[names.length - 1];
    onEvent({ id: -2, day: sim.day, kind: 'world', text: `${fresh[0].f}: ${list} ${names.length === 1 ? 'meydanında' : 'meydanlarında'} dans ve müzik.`, tile: fresh[0].st.tile, civ: fresh[0].civ, major: true });
  }
}
function frame(now: number) {
  // bir sonraki kare en başta istenir: çizimde tek bir hata döngüyü (ve dünyayı) dondurmasın
  requestAnimationFrame(frame);
  const dt = Math.min(0.25, (now - last) / 1000);
  last = now;
  if (!paused) {
    acc += dt * speed;
    let n = 0;
    while (acc >= 1 && n < 30) { sim.step(); acc -= 1; n++; }
    if (n >= 30) acc = 0;
  }
  if (mode === '3d' && dio) { festivalTick(); cineTick(now); dio.render(now); if (amb.enabled && now - lastSnd > 100) { lastSnd = now; amb.update(now, dio.soundInfo()); } feedTick(now); if (now - lastFps > 500) { lastFps = now; $('fps').textContent = `${dio.fps} fps · ${QN[dio.quality]}${dio.autoQuality ? ' (oto)' : ''}`; document.body.classList.toggle('lowq', dio.quality === 0); } if (dio.follow && now - lastFollow > 300) { lastFollow = now; renderFollow(); } } else draw(now);
  const due = drawerOpen && (tab === 'log' ? dirty && !paneHover : (dirty || !paused) && !paneHover);
  if (due && now - lastPanel > 600) { lastPanel = now; renderPanel(); }
  if (now - lastHud > 500) { lastHud = now; renderHud(); }
}

(window as unknown as { fd: unknown }).fd = { get sim() { return sim; }, dio };
newWorld(seed);
layout();
setMode(mode);
let cineInit = true;
try { cineInit = localStorage.getItem('fd-cine') !== '0'; } catch { /* yok */ }
if (mode === '3d' && dio) { setCine(cineInit); cinePause = performance.now() + 6000; }
renderHud();
requestAnimationFrame(frame);
