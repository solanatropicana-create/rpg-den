// Savaş Tiyatrosu: kaydedilmiş bir savaşı tur tur, zar zar yeniden oynatır.
// Her saldırı: d20 + bonus ≥ AC → isabet; kritikte hasar zarları ikiye katlanır; hasar = zarlar + bonus (+ sınıf ekleri).
import type { Battle, Replay, ReplayEv, ReplayUnit, ReplayGroup } from '../sim/types';
import { HERO_CLASS_TR, type HeroClass } from '../data/heroes';
import type { ClassId } from '../data/classes';
import { classIcon } from './icons';
import { winChance } from '../sim/combat';
import { DiceTray, diceClack, type DieSpec } from './dice3d';

export interface TheaterHost {
  civColor(id: number): string;
  dateStr(day: number): string;
  sound(): boolean;
  onClose(): void;
}

const esc = (s: string) => s.replace(/[&<>"]/g, (c) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;' }[c]!));
const MONSTER_COL: Record<string, string> = { goblin: '#b8452f', hobgoblin: '#7d241a', bugbear: '#6d45a3', pirate: '#39404d' };
const PART_COL: Record<string, string> = { 'Sinsi saldırı': '#3b3358', 'Av işareti': '#2e5e3a', 'İlahi çarpış': '#b8912a', 'Öfke': '#8e2a1d' };
const KIND_TR: Record<string, string> = { soldier: 'Asker', militia: 'Milis', monster: 'Canavar', unique: 'Seçkin birlik', boss: 'Önder', galley: 'Kadırga', cog: 'Koga', hero: 'Kahraman' };
const SP_ICON: Record<string, string> = { fireball: '🔥', burning: '🔥', heal: '✚', second: '✚', wild: '🐻', rage: '💢', flee: '🏳', meteor: '☄', rout: '⚑' };
const sum = (xs: number[]) => xs.reduce((a, b) => a + b, 0);
const pct = (x: number) => `%${Math.round(x * 100)}`;
/** tahmin yüzdesi: kesinlik yanılsaması vermesin */
const odds = (x: number) => x > 0.99 ? '%99+' : x < 0.01 ? '%1-' : pct(x);

interface SideStat { att: number; hit: number; crit: number; n20: number; n1: number; d20: number; dmg: number; kills: number; heal: number; hist: number[] }
const blank = (): SideStat => ({ att: 0, hit: 0, crit: 0, n20: 0, n1: 0, d20: 0, dmg: 0, kills: 0, heal: 0, hist: new Array(20).fill(0) });

/** tekrarı baştan sona işleyip istatistikleri çıkarır */
function analyze(r: Replay) {
  const hp = r.units.map((u) => u.hp);
  const side = { A: blank(), B: blank() };
  const per = r.units.map(() => ({ dmg: 0, kills: 0, heal: 0 }));
  const kill = r.ev.map(() => false);
  const hitT = (i: number, t: number, v: number, who: number | undefined, s: 'A' | 'B') => {
    const prev = hp[t]; hp[t] -= v;
    side[s].dmg += v; if (who !== undefined && who >= 0) per[who].dmg += v;
    if (prev > 0 && hp[t] <= 0) { side[s].kills++; kill[i] = true; if (who !== undefined && who >= 0) per[who].kills++; }
  };
  r.ev.forEach((e, i) => {
    if (e.sp === 'attack') {
      const s = side[r.units[e.a!].s];
      s.att++; s.d20 += e.d!; s.hist[e.d! - 1]++;
      if (e.d === 20) s.n20++; if (e.d === 1) s.n1++;
      if (e.h) { s.hit++; if (e.h === 2) s.crit++; hitT(i, e.t!, hp[e.t!] - e.hp!, e.a, r.units[e.a!].s); }
    } else if (e.sp === 'fireball' || e.sp === 'burning' || e.sp === 'meteor') {
      const s: 'A' | 'B' = e.sp === 'meteor' ? (e.d === 0 ? 'A' : 'B') : r.units[e.a!].s;
      e.ts!.forEach((t, k) => hitT(i, t, e.vs![k], e.a, s));
    } else if (e.sp === 'heal' || e.sp === 'second' || e.sp === 'wild') {
      hp[e.t!] = e.hp!; side[r.units[e.a!].s].heal += e.v ?? 0; per[e.a!].heal += e.v ?? 0;
    }
  });
  return { side, per, kill };
}

/** kart olarak gösterilenler: kahraman, önder, gemi ve iri seçkin birlikler (golem, treant…); gerisi nokta dizisi */
function isCard(u: ReplayUnit) { return u.k === 'hero' || u.k === 'boss' || u.k === 'galley' || u.k === 'cog' || (u.k === 'unique' && u.max >= 40); }
function unitSub(u: ReplayUnit) {
  if (u.k === 'hero' && u.cls) return `${HERO_CLASS_TR[u.cls as HeroClass] ?? u.cls} · Sv ${u.lvl}`;
  return KIND_TR[u.k] ?? u.k;
}
function dmgStr(u: ReplayUnit) { return `${u.dmg[0]}d${u.dmg[1]}${u.dmg[2] ? (u.dmg[2] > 0 ? '+' : '') + u.dmg[2] : ''}${u.att > 1 ? ` ×${u.att}` : ''}`; }
function chanceOf(u: ReplayUnit, ac: number) {
  const cr = u.cls === 'fighter' && (u.lvl ?? 0) >= 3 ? 19 : 20;
  let n = 0;
  for (let d = 2; d <= 20; d++) if (d >= cr || d + u.atk >= ac) n++;
  return n / 20;
}

export class BattleTheater {
  private root: HTMLElement;
  private tray: DiceTray | null = null;
  private r: Replay;
  private stats: ReturnType<typeof analyze>;
  private hp: number[] = [];
  private out: boolean[] = [];
  private rage: boolean[] = [];
  private round = 0;
  private routed: 'A' | 'B' | null = null;
  private i = 0;
  private tok = 0;
  private playing = true;
  private speed = 1;
  private keyOnly = true;
  private uEl: HTMLElement[] = [];
  private logLines: string[] = [];
  private finished = false;
  private keyH: (e: KeyboardEvent) => void;
  private resizeH: () => void;
  private gcol: string[];

  constructor(private b: Battle, private host: TheaterHost) {
    this.r = b.replay!;
    this.stats = analyze(this.r);
    this.gcol = this.r.groups.map((g, k) => this.groupColor(g, k));
    try { const sp = Number(localStorage.getItem('fd-bt-speed')); if (sp > 0) this.speed = sp; this.keyOnly = localStorage.getItem('fd-bt-all') !== '1'; } catch { /* yok */ }
    this.root = document.createElement('div');
    this.root.className = 'bt-backdrop';
    this.root.innerHTML = this.shell();
    document.body.appendChild(this.root);
    document.body.classList.add('bt-open');
    this.buildRoster();
    this.buildScrub();
    const cv = this.root.querySelector<HTMLCanvasElement>('.bt-tray canvas')!;
    try {
      this.tray = new DiceTray(cv);
      this.tray.speed = this.speed;
      this.tray.onBounce = (s) => { if (this.host.sound()) diceClack(s); };
    } catch (err) { console.warn('zar tepsisi açılamadı', err); this.root.querySelector('.bt-tray')!.classList.add('flat'); }
    this.root.addEventListener('click', (e) => this.onClick(e));
    this.keyH = (e) => this.onKey(e);
    this.resizeH = () => this.tray?.resize();
    window.addEventListener('keydown', this.keyH, true);
    window.addEventListener('resize', this.resizeH);
    this.reset();
    this.renderAll();
    this.syncCtl();
    requestAnimationFrame(() => { this.tray?.resize(); this.root.querySelector<HTMLElement>('.bt-x')?.focus(); });
    void this.run();
  }

  // ---------------------------------------------------------------- iskelet
  private groupColor(g: ReplayGroup, k: number): string {
    if (g.civ !== undefined && g.civ >= 0) return this.host.civColor(g.civ);
    if (g.kind && MONSTER_COL[g.kind]) return MONSTER_COL[g.kind];
    if (g.kind === 'party') return ['#c9a227', '#b07b2e', '#d4b24c'][k % 3];
    if (g.side === 'B') return this.b.civB !== undefined ? this.host.civColor(this.b.civB) : '#b8452f';
    return this.b.civA !== undefined ? this.host.civColor(this.b.civA) : '#3f8f78';
  }
  /** tarafın kısa adı: tek grupsa grubun adı, ortak saldırıda "ilk grup + N" */
  private sideLabel(s: 'A' | 'B') {
    const gs = this.r.groups.filter((g) => g.side === s);
    if (gs.length > 1) return `${gs[0].name} + ${gs.length - 1} grup`;
    return s === 'A' ? this.b.sideA : this.b.sideB;
  }
  private sideColor(s: 'A' | 'B') { const k = this.r.groups.findIndex((g) => g.side === s); return this.gcol[k] ?? (s === 'A' ? '#3f8f78' : '#b8452f'); }
  private uColor(i: number) { return this.gcol[this.r.units[i].g] ?? this.sideColor(this.r.units[i].s); }

  private shell(): string {
    const b = this.b, r = this.r;
    const p = winChance(r.powA, r.powB);
    const gA = r.groups.filter((g) => g.side === 'A'), gB = r.groups.filter((g) => g.side === 'B');
    const chips = (gs: ReplayGroup[]) => gs.map((g) => `<span class="bt-gchip" style="--cc:${this.gcol[r.groups.indexOf(g)]}">${esc(g.name)}</span>`).join('');
    return `<div class="bt" role="dialog" aria-modal="true" aria-label="${esc(b.title)}">
  <header class="bt-head">
    <div class="bt-title"><div class="bt-kicker">${b.joint ? 'Ortak saldırı · ' : ''}${esc(this.host.dateStr(b.day))} · ${r.units.length} savaşçı</div><h2>${esc(b.title)}</h2></div>
    <div class="bt-odds" title="Savaş öncesi güç tahmini (Lanchester): ${r.powA} : ${r.powB}">
      <div class="bt-oddsnames"><span>${chips(gA)}</span><span class="bt-vs">karşı</span><span>${chips(gB)}</span></div>
      <div class="bt-oddsbar"><i style="width:${Math.round(p * 100)}%;background:${this.sideColor('A')}"></i><b style="background:${this.sideColor('B')}"></b></div>
      <div class="bt-oddsnum"><span>${odds(p)}</span><em>savaş öncesi tahmin · güç ${r.powA} : ${r.powB}</em><span>${odds(1 - p)}</span></div>
    </div>
    <button class="bt-x" data-bt="close" aria-label="Kapat" title="Kapat (Esc)">×</button>
  </header>
  <div class="bt-body">
    <section class="bt-side a" aria-label="Saldıran taraf"></section>
    <section class="bt-center">
      <div class="bt-status">
        <div class="bt-morale a"><div class="bt-mlbl"><span>Moral</span><b></b></div><div class="bt-mbar"><i></i><u></u></div></div>
        <div class="bt-round">TUR <b>0</b><span>/${r.maxRounds}</span></div>
        <div class="bt-morale b"><div class="bt-mlbl"><b></b><span>Moral</span></div><div class="bt-mbar"><i></i><u></u></div></div>
      </div>
      <div class="bt-tray"><canvas></canvas><div class="bt-banner" aria-live="polite"></div><div class="bt-flat"></div></div>
      <div class="bt-eq" aria-live="polite"></div>
      <div class="bt-ctl">
        <button data-bt="start" title="Baştan (Home)" aria-label="Baştan">⏮</button>
        <button data-bt="back" title="Bir adım geri (←)" aria-label="Geri">◀</button>
        <button data-bt="play" class="bt-play" title="Oynat / durdur (Boşluk)"></button>
        <button data-bt="step" title="Bir adım ileri (→)" aria-label="İleri">▶</button>
        <button data-bt="end" title="Sona atla (End)" aria-label="Sona atla">⏭</button>
        <span class="bt-sep"></span>
        <span class="bt-seg" role="group" aria-label="Hız">${[1, 2, 4].map((s) => `<button data-bt="speed" data-v="${s}">${s}×</button>`).join('')}</span>
        <span class="bt-sep"></span>
        <label class="bt-key"><input type="checkbox" data-bt="key"> Yalnız önemli anlarda zar at</label>
      </div>
      <div class="bt-scrub" title="Olay çizelgesi: tıkla, o ana git"></div>
    </section>
    <section class="bt-side b" aria-label="Savunan taraf"></section>
  </div>
  <footer class="bt-foot">
    <div class="bt-tabs"><button data-bt="tab" data-v="log" aria-pressed="true">Savaş kaydı</button><button data-bt="tab" data-v="sum" aria-pressed="false">Zar istatistiği</button><button data-bt="tab" data-v="rules" aria-pressed="false">Zar nasıl işler?</button></div>
    <div class="bt-log"></div>
    <div class="bt-sum" hidden></div>
    <div class="bt-rules" hidden>${this.rulesHtml()}</div>
  </footer>
</div>`;
  }

  private rulesHtml() {
    const r = this.r;
    return `<div class="bt-rulegrid">
<div><h4>Saldırı zarı</h4><p><span class="rchip">d20</span> + saldırı bonusu, hedefin zırh sınıfına (<b>AC</b>) eşit ya da büyükse <b>isabet</b>. <b>Doğal 20</b> her zaman kritiktir (Şampiyon savaşçıda 19–20), <b>doğal 1</b> her zaman ıskadır.</p></div>
<div><h4>Hasar zarı</h4><p>Silahın zarı (ör. <span class="rchip">1d8</span>) + sabit bonus. Kritikte hasar zarlarının sayısı ikiye katlanır. Hırsızın sinsi saldırısı, korucunun av işareti, paladinin ilahi çarpışı ayrı zarlarla eklenir. Öfkeli barbar gelen hasarın yarısını alır.</p></div>
<div><h4>Tur ve sıra</h4><p>Her tur herkes rastgele sırayla bir kez (seviyeli dövüşçüler iki kez) saldırır. Hedef rastgele seçilir; kahramanlar önderi %60 ihtimalle hedefler. En çok ${r.maxRounds} tur.</p></div>
<div><h4>Moral ve bozgun</h4><p>Tur sonunda kayıp oranı eşiği geçen taraf bozguna uğrar: saldıranda %${Math.round(r.moraleA * 100)}, savunanda %${Math.round(r.moraleB * 100)}. Ayakta kahraman varsa eşik %${Math.round(r.heroMorale * 100)} yükselir. Yaralı kahraman kaçabilir.</p></div>
<div><h4>Süre dolarsa</h4><p>${r.timeoutWinner ? `Gün batınca savunan taraf (${r.timeoutWinner === 'A' ? esc(this.b.sideA) : esc(this.b.sideB)}) mevzisini korumuş sayılır.` : 'Gün batınca ayakta kalan toplam canı fazla olan taraf alanı tutar.'}</p></div>
<div><h4>Tahmin</h4><p>Üstteki yüzde, iki tarafın güç tahmininden (isabet şansı × ortalama hasar × can) hesaplanır ve simülasyonda ölçülmüş sonuçlara göre ayarlanmıştır. Zar her zaman sürpriz yapabilir.</p></div>
</div>`;
  }

  private buildRoster() {
    const r = this.r;
    for (const s of ['A', 'B'] as const) {
      const box = this.root.querySelector<HTMLElement>(`.bt-side.${s.toLowerCase()}`)!;
      let h = '';
      r.groups.forEach((g, gi) => {
        if (g.side !== s) return;
        const ids = r.units.map((u, i) => (u.g === gi ? i : -1)).filter((i) => i >= 0);
        if (!ids.length) return;
        h += `<div class="bt-grp" style="--cc:${this.gcol[gi]}"><div class="bt-gname"><i></i>${esc(g.name)}<span>${ids.length}</span></div>`;
        const cards = ids.filter((i) => isCard(r.units[i]) || (ids.length <= 3 && r.units.length <= 8));
        const pips = ids.filter((i) => !cards.includes(i));
        for (const i of cards) h += this.cardHtml(i);
        // aynı adlı sıradan birlikler: nokta dizisi
        const byName = new Map<string, number[]>();
        for (const i of pips) { const k = r.units[i].n; byName.set(k, [...(byName.get(k) ?? []), i]); }
        for (const [name, list] of byName) {
          const u = r.units[list[0]];
          h += `<div class="bt-pips"><div class="bt-plbl"><b>${esc(name)}</b> ×${list.length}<span class="mono">AC ${u.ac} · +${u.atk} · ${dmgStr(u)} · ${u.max} can</span></div><div class="bt-pipbox">${list.map((i) => `<span class="bt-pip" data-u="${i}" title="${esc(name)} · AC ${u.ac} · +${r.units[i].atk}"><i></i></span>`).join('')}</div></div>`;
        }
        h += '</div>';
      });
      box.innerHTML = h;
    }
    this.uEl = r.units.map((_, i) => this.root.querySelector<HTMLElement>(`[data-u="${i}"]`)!);
  }
  private cardHtml(i: number) {
    const u = this.r.units[i];
    const ic = u.k === 'hero' && u.cls ? classIcon(u.cls as ClassId) : u.boss ? '<span class="bt-sk">☠</span>' : `<span class="bt-sk">${esc(u.n.charAt(0))}</span>`;
    return `<div class="bt-card${u.boss ? ' boss' : ''}${u.k === 'hero' ? ' hero' : ''}" data-u="${i}">
<div class="bt-ic">${ic}</div><div class="bt-cmain"><div class="bt-cname">${esc(u.n)}<span class="bt-badge"></span></div><div class="bt-csub">${esc(unitSub(u))}</div>
<div class="bt-hp"><i></i></div><div class="bt-cst mono"><span class="bt-hpn"></span><span title="Zırh sınıfı">🛡${u.ac}</span><span title="Saldırı bonusu">+${u.atk}</span><span title="Hasar">${dmgStr(u)}</span></div></div></div>`;
  }

  private buildScrub() {
    const box = this.root.querySelector<HTMLElement>('.bt-scrub')!;
    const r = this.r;
    box.innerHTML = r.ev.map((e, i) => {
      let c = 'o';
      if (e.sp === 'round') c = 'rd';
      else if (e.sp === 'attack') c = e.d === 20 ? 'n20' : e.d === 1 ? 'n1' : e.h === 2 ? 'cr' : e.h ? 'hit' : 'miss';
      else if (e.sp === 'heal' || e.sp === 'second' || e.sp === 'wild') c = 'heal';
      else if (e.sp === 'fireball' || e.sp === 'burning' || e.sp === 'meteor') c = 'fire';
      else if (e.sp === 'rout' || e.sp === 'flee') c = 'rout';
      return `<i class="${c}${this.stats.kill[i] ? ' k' : ''}" data-i="${i}"${e.sp === 'round' ? ` data-r="${e.r}"` : ''}></i>`;
    }).join('') + '<b class="bt-head-mark"></b>';
  }

  // ---------------------------------------------------------------- durum
  private reset() {
    this.hp = this.r.units.map((u) => u.hp);
    this.out = this.r.units.map(() => false);
    this.rage = this.r.units.map(() => false);
    this.round = 0; this.routed = null; this.i = 0; this.finished = false;
    this.logLines = [];
  }
  private apply(e: ReplayEv) {
    switch (e.sp) {
      case 'round': this.round = e.r; break;
      case 'attack': if (e.h) this.hp[e.t!] = e.hp!; break;
      case 'fireball': case 'burning': case 'meteor': e.ts!.forEach((t, k) => { this.hp[t] -= e.vs![k]; }); break;
      case 'heal': case 'second': case 'wild': this.hp[e.t!] = e.hp!; break;
      case 'flee': this.out[e.a!] = true; break;
      case 'rage': this.rage[e.a!] = true; break;
      case 'rout': this.routed = e.d === 0 ? 'A' : 'B'; break;
    }
    this.logLines.push(this.logLine(e));
  }
  private alive(i: number) { return this.hp[i] > 0 && !this.out[i]; }
  private sideAlive(s: 'A' | 'B') { return this.r.units.filter((u, i) => u.s === s && this.alive(i)).length; }

  private renderUnit(i: number) {
    const el = this.uEl[i]; if (!el) return;
    const u = this.r.units[i];
    const hp = Math.max(0, this.hp[i]);
    const f = Math.min(1, hp / u.max);
    const bar = el.querySelector<HTMLElement>('i');
    if (bar) { bar.style.width = el.classList.contains('bt-pip') ? '' : `${f * 100}%`; if (el.classList.contains('bt-pip')) bar.style.height = `${f * 100}%`; bar.style.background = f > 0.5 ? '#6fcf7a' : f > 0.25 ? '#e3b341' : '#e0685c'; }
    const n = el.querySelector<HTMLElement>('.bt-hpn'); if (n) n.textContent = `${hp}/${u.max}`;
    const bd = el.querySelector<HTMLElement>('.bt-badge'); if (bd) bd.textContent = this.out[i] && this.hp[i] > 0 ? ' · kaçtı' : this.rage[i] ? ' · öfkeli' : '';
    el.classList.toggle('dead', this.hp[i] <= 0);
    el.classList.toggle('fled', this.out[i] && this.hp[i] > 0);
    el.classList.toggle('routed', !!this.routed && u.s === this.routed && this.hp[i] > 0);
    if (el.classList.contains('bt-pip')) el.title = `${u.n} · ${hp}/${u.max} can · AC ${u.ac} · +${u.atk} · ${dmgStr(u)}`;
  }
  private renderStatus() {
    const r = this.r;
    this.root.querySelector('.bt-round b')!.textContent = String(this.round);
    for (const s of ['A', 'B'] as const) {
      const init = r.units.filter((u) => u.s === s).length;
      const alive = this.sideAlive(s);
      const loss = init ? 1 - alive / init : 0;
      const hero = r.units.some((u, i) => u.s === s && u.k === 'hero' && this.alive(i));
      const noRout = s === 'A' ? r.noRoutA : r.noRoutB;
      const th = Math.min(0.95, (s === 'A' ? r.moraleA : r.moraleB) + (hero ? r.heroMorale : 0));
      const m = this.root.querySelector<HTMLElement>(`.bt-morale.${s.toLowerCase()}`)!;
      m.querySelector<HTMLElement>('i')!.style.width = `${Math.min(100, loss * 100)}%`;
      m.querySelector<HTMLElement>('i')!.style.background = this.sideColor(s);
      m.querySelector<HTMLElement>('u')!.style.left = `${th * 100}%`;
      m.querySelector<HTMLElement>('u')!.hidden = !!noRout;
      m.querySelector('b')!.textContent = `${alive}/${init} ayakta · kayıp ${pct(loss)} · ${noRout ? 'bozulmaz' : `eşik ${pct(th)}${hero ? ' (kahraman)' : ''}`}`;
      m.classList.toggle('broken', this.routed === s);
    }
    const marks = this.root.querySelectorAll<HTMLElement>('.bt-scrub i');
    marks.forEach((x, k) => x.classList.toggle('past', k < this.i));
    const hm = this.root.querySelector<HTMLElement>('.bt-head-mark');
    if (hm) hm.style.left = `${(this.i / Math.max(1, this.r.ev.length)) * 100}%`;
  }
  private renderLog() {
    const box = this.root.querySelector<HTMLElement>('.bt-log')!;
    const lines = this.logLines.filter(Boolean);
    box.innerHTML = lines.slice(-160).join('');
    box.scrollTop = box.scrollHeight;
  }
  private renderAll() { this.r.units.forEach((_, i) => this.renderUnit(i)); this.renderStatus(); this.renderLog(); }

  // ---------------------------------------------------------------- metin
  private nm(i: number) { const u = this.r.units[i]; return `<b style="color:${this.uColor(i)}">${esc(u.n)}</b>`; }
  private diceTxt(dd: number[] | undefined, ds: number | undefined) { return (dd ?? []).map((v) => `<span class="dchip d${ds}">${v}</span>`).join(''); }
  private logLine(e: ReplayEv): string {
    const r = this.r;
    switch (e.sp) {
      case 'round': return `<div class="lr">— Tur ${e.r} · ayakta ${e.aA} : ${e.aB} —</div>`;
      case 'attack': {
        const sum0 = e.d! + e.m!;
        if (!e.h) return `<div class="l miss"><span class="dchip d20${e.d === 1 ? ' n1' : ''}">${e.d}</span> ${this.nm(e.a!)} → ${this.nm(e.t!)}: ${e.d === 1 ? 'doğal 1, ıska' : `${e.d}+${e.m}=${sum0} &lt; AC ${e.ac}, ıska`}</div>`;
        const parts = (e.x ?? []).map((p) => ` + ${esc(p.l)} ${p.v}`).join('');
        const k = this.hp && e.hp! <= 0 ? ' <span class="kill">✝ devrildi</span>' : '';
        return `<div class="l ${e.h === 2 ? 'crit' : 'hit'}"><span class="dchip d20${e.d === 20 ? ' n20' : ''}">${e.d}</span> ${this.nm(e.a!)} → ${this.nm(e.t!)}: ${e.h === 2 ? '<b class="cr">KRİTİK</b> ' : `${sum0} ≥ AC ${e.ac}, `}${this.diceTxt(e.dd, e.ds)}${e.b ? ` ${e.b > 0 ? '+' : ''}${e.b}` : ''}${parts}${e.mul ? ` ×${e.mul}` : ''}${e.half ? ' ½ öfke' : ''} = <b>${e.v}</b> hasar${k}</div>`;
      }
      case 'fireball': case 'burning': case 'meteor': {
        const who = e.sp === 'meteor' ? 'Gökten meteorlar' : this.nm(e.a!);
        const tt = e.ts!.map((t, k) => `${this.nm(t)} ${e.vs![k]}${e.sv?.[k] ? ' (yarı)' : ''}`).join(', ');
        return `<div class="l fire">${SP_ICON[e.sp]} ${who} ${e.sp === 'fireball' ? 'ATEŞ TOPU (8d6)' : e.sp === 'burning' ? 'Yakan Eller (3d6)' : 'yağdı (4d6)'}: ${tt}</div>`;
      }
      case 'heal': return `<div class="l heal">✚ ${this.nm(e.a!)} → ${this.nm(e.t!)}: ${this.diceTxt(e.dd, e.ds)} + ${e.b} = <b>+${e.v}</b> can</div>`;
      case 'second': return `<div class="l heal">✚ ${this.nm(e.a!)} derin nefes: ${this.diceTxt(e.dd, e.ds)} + ${e.b} = <b>+${e.v}</b> can</div>`;
      case 'wild': return `<div class="l heal">🐻 ${this.nm(e.a!)} ayıya dönüştü: <b>+${e.v}</b> can</div>`;
      case 'rage': return `<div class="l sp">💢 ${this.nm(e.a!)} öfkeye kapıldı: +2 hasar, gelen hasar yarıya</div>`;
      case 'flee': return `<div class="l sp">🏳 ${this.nm(e.a!)} ağır yaralı, savaş alanından kaçtı</div>`;
      case 'rout': return `<div class="l rout">⚑ ${esc(this.sideLabel(e.d === 0 ? 'A' : 'B'))} bozguna uğradı</div>`;
    }
    return '';
  }

  // ---------------------------------------------------------------- oynatma
  private sleep(ms: number, tok: number) { return new Promise<boolean>((res) => setTimeout(() => res(tok === this.tok), ms / this.speed)); }
  private important(k: number): boolean {
    const e = this.r.ev[k];
    if (e.sp !== 'attack') return e.sp !== 'round';
    if (!this.keyOnly) return true;
    const a = this.r.units[e.a!], t = this.r.units[e.t!];
    if (isCard(a) || isCard(t)) return true;
    if (e.d === 20 || e.d === 1) return true;
    // ilk birkaç saldırı her zaman zarla gösterilir: mantık görülsün
    let seen = 0; for (let j = 0; j < k; j++) if (this.r.ev[j].sp === 'attack') seen++;
    return seen < 2;
  }

  private async run() {
    const tok = ++this.tok;
    while (tok === this.tok) {
      if (this.i >= this.r.ev.length) { this.finish(); return; }
      if (!this.playing) return;
      const k = this.i;
      const ok = await this.show(k, this.important(k), tok);
      if (!ok || tok !== this.tok) return;
      this.i = k + 1;
      this.renderStatus();
    }
  }

  /** bir olayı göster; durumu uygun anda uygular. false: iptal edildi */
  private async show(k: number, full: boolean, tok: number): Promise<boolean> {
    const e = this.r.ev[k];
    const r = this.r;
    const eq = this.root.querySelector<HTMLElement>('.bt-eq')!;
    const done = (targets: number[]) => { this.apply(e); for (const t of targets) this.renderUnit(t); this.renderStatus(); this.renderLog(); };
    const mark = (i: number | undefined, cls: string, on: boolean) => { if (i !== undefined && i >= 0) this.uEl[i]?.classList.toggle(cls, on); };
    switch (e.sp) {
      case 'round': {
        done([]);
        this.banner(`Tur ${e.r}`, 'round');
        return this.sleep(full ? 650 : 250, tok);
      }
      case 'attack': {
        const a = r.units[e.a!], t = r.units[e.t!];
        mark(e.a, 'act', true); mark(e.t, 'tgt', true);
        this.uEl[e.a!]?.style.setProperty('--act', this.uColor(e.a!));
        if (!full) {
          eq.innerHTML = `<div class="eq-mini"><span class="dchip d20${e.d === 20 ? ' n20' : e.d === 1 ? ' n1' : ''}">${e.d}</span> ${this.nm(e.a!)} → ${this.nm(e.t!)} · ${e.h ? `<b>${e.v}</b> hasar` : 'ıska'}</div>`;
          if (!(await this.sleep(110, tok))) return false;
          done([e.t!]);
          if (e.h) this.floatNum(e.t!, `−${e.v}`, e.h === 2 ? 'crit' : 'dmg');
          mark(e.a, 'act', false); mark(e.t, 'tgt', false);
          return this.sleep(60, tok);
        }
        const chance = chanceOf(a, e.ac!);
        const need = Math.max(2, Math.min(20, e.ac! - e.m!));
        eq.innerHTML = `<div class="eq-who">${this.nm(e.a!)} <span class="sub">${esc(unitSub(a))}</span> <span class="arr">➜</span> ${this.nm(e.t!)} <span class="sub">${esc(unitSub(t))} · ${Math.max(0, this.hp[e.t!])}/${t.max} can</span></div>
<div class="eq-line"><span class="slot big">d20</span> <span class="op">+</span> <span class="num">${e.m}</span> <span class="op">vs</span> <span class="ac">AC ${e.ac}</span> <span class="hint">isabet için ${need}+ gerek · şans ${pct(chance)}</span></div>`;
        this.tray?.clear();
        const side = a.s === 'A' ? 'left' : 'right';
        const glow: DieSpec['glow'] = e.d === 20 || e.h === 2 ? 'crit' : e.d === 1 ? 'fumble' : e.h ? 'hit' : 'miss';
        await this.rollDice([{ sides: 20, value: e.d!, color: this.uColor(e.a!), glow }], 'hit', side, e.d!);
        if (tok !== this.tok) return false;
        const sum0 = e.d! + e.m!;
        const verdict = e.d === 1 ? '<span class="verdict fumble">DOĞAL 1 · ISKA</span>' : e.h === 2 ? `<span class="verdict crit">${e.d === 20 ? 'DOĞAL 20 · ' : ''}KRİTİK!</span>` : e.h ? '<span class="verdict hit">İSABET</span>' : '<span class="verdict miss">ISKA</span>';
        eq.querySelector('.eq-line')!.innerHTML = `<span class="dchip big d20${e.d === 20 ? ' n20' : e.d === 1 ? ' n1' : ''}">${e.d}</span> <span class="op">+</span> <span class="num">${e.m}</span> <span class="op">=</span> <b class="tot">${sum0}</b> <span class="op">${e.d === 1 ? '·' : sum0 >= e.ac! ? '≥' : '&lt;'}</span> <span class="ac">AC ${e.ac}</span> ${verdict}`;
        if (e.d === 1 || e.h === 2) this.banner(e.d === 1 ? 'Doğal 1!' : e.d === 20 ? 'Doğal 20!' : 'Kritik!', e.d === 1 ? 'fumble' : 'crit');
        if (!e.h) {
          done([e.t!]);
          if (!(await this.sleep(750, tok))) return false;
          mark(e.a, 'act', false); mark(e.t, 'tgt', false);
          return true;
        }
        if (!(await this.sleep(420, tok))) return false;
        // hasar zarları
        const specs: DieSpec[] = (e.dd ?? []).map((v) => ({ sides: e.ds!, value: v, color: this.uColor(e.a!), glow: e.h === 2 ? 'crit' : undefined }));
        for (const p of e.x ?? []) for (const v of p.dd ?? []) specs.push({ sides: p.ds!, value: v, color: PART_COL[p.l] ?? '#444', ink: p.l === 'İlahi çarpış' ? '#fff7d6' : undefined });
        const dline = document.createElement('div');
        dline.className = 'eq-line dmg';
        dline.innerHTML = `<span class="slot">${(e.dd ?? []).length}d${e.ds}${e.h === 2 ? ' (kritik: ×2 zar)' : ''}${(e.x ?? []).map((p) => ` + ${esc(p.l)}`).join('')}</span>`;
        eq.appendChild(dline);
        if (specs.length) await this.rollDice(specs, 'dmg', side);
        if (tok !== this.tok) return false;
        const base = sum(e.dd ?? []) + (e.b ?? 0);
        const parts = (e.x ?? []).map((p) => `<span class="part" style="--pc:${PART_COL[p.l] ?? '#555'}"><span class="op">+</span> ${esc(p.l)} ${p.dd ? this.diceTxt(p.dd, p.ds) : ''}${!p.dd || p.dd.length > 1 ? `${p.dd ? ' = ' : ''}<b>${p.v}</b>` : ''}</span>`).join(' ');
        dline.innerHTML = `${this.diceTxt(e.dd, e.ds)}${e.b ? ` <span class="op">${e.b > 0 ? '+' : '−'}</span> <span class="num">${Math.abs(e.b)}</span>` : ''} <span class="op">=</span> <b>${base}</b> ${parts}${e.mul ? ` <span class="part"><span class="op">×</span> ilk vuruş ${e.mul}</span>` : ''}${e.half ? ' <span class="part">½ öfkeli hedef</span>' : ''} <span class="op">→</span> <b class="tot dmg">${e.v} hasar</b>`;
        const before = Math.max(0, this.hp[e.t!]);
        done([e.t!]);
        this.floatNum(e.t!, `−${e.v}`, e.h === 2 ? 'crit' : 'dmg');
        const kill = e.hp! <= 0;
        const hpl = document.createElement('div');
        hpl.className = 'eq-hp';
        hpl.innerHTML = `${this.nm(e.t!)}: ${before} → <b>${Math.max(0, e.hp!)}</b> can${kill ? ' <span class="kill">✝ devrildi</span>' : ''}`;
        eq.appendChild(hpl);
        if (kill && isCard(t)) this.banner(`${t.n} düştü`, 'kill');
        if (!(await this.sleep(kill ? 1050 : 800, tok))) return false;
        mark(e.a, 'act', false); mark(e.t, 'tgt', false);
        return true;
      }
      case 'fireball': case 'burning': case 'meteor': {
        const fire = e.sp !== 'meteor';
        mark(e.a, 'act', true);
        for (const t of e.ts!) mark(t, 'tgt', true);
        eq.innerHTML = `<div class="eq-who">${fire ? this.nm(e.a!) : '<b>Gökyüzü</b>'} <span class="arr">➜</span> ${e.ts!.length} hedef</div><div class="eq-line"><span class="slot big">${e.sp === 'fireball' ? '8d6 ateş' : e.sp === 'burning' ? '3d6 ateş' : '4d6 taş'}</span> <span class="hint">her hedef ayrı zar atar · %35 ihtimalle kaçınıp yarısını alır</span></div>`;
        this.banner(e.sp === 'fireball' ? 'ATEŞ TOPU!' : e.sp === 'burning' ? 'Yakan Eller' : 'Meteor yağmuru!', 'fire');
        this.tray?.clear();
        const dd = e.dds?.[0] ?? [];
        await this.rollDice(dd.map((v) => ({ sides: 6, value: v, color: fire ? '#a8321c' : '#4a4a55', ink: '#ffe3a1', glow: 'fire' as const })), dd.length > 4 ? 'dmg' : 'center', r.units[e.a ?? 0]?.s === 'B' ? 'right' : 'left');
        if (tok !== this.tok) return false;
        done(e.ts!);
        e.ts!.forEach((t, k2) => this.floatNum(t, `−${e.vs![k2]}`, 'fire'));
        eq.querySelector('.eq-line')!.innerHTML = e.ts!.map((t, k2) => `<span class="ftgt">${this.nm(t)} <b>${e.vs![k2]}</b>${e.sv?.[k2] ? ' <em>yarı</em>' : ''}${this.hp[t] <= 0 ? ' ✝' : ''}</span>`).join('');
        if (!(await this.sleep(1300, tok))) return false;
        mark(e.a, 'act', false); for (const t of e.ts!) mark(t, 'tgt', false);
        return true;
      }
      case 'heal': case 'second': {
        mark(e.a, 'act', true); mark(e.t, 'heal', true);
        eq.innerHTML = `<div class="eq-who">${this.nm(e.a!)} <span class="arr">✚</span> ${this.nm(e.t!)}</div><div class="eq-line"><span class="slot big">1d${e.ds} + ${e.b}</span> <span class="hint">${e.sp === 'heal' ? 'iyileştirme büyüsü' : 'Second Wind: 1d10 + seviye'}</span></div>`;
        this.tray?.clear();
        await this.rollDice((e.dd ?? []).map((v) => ({ sides: e.ds!, value: v, color: '#2f7d44', glow: 'heal' as const })), 'center', r.units[e.a!].s === 'A' ? 'left' : 'right');
        if (tok !== this.tok) return false;
        done([e.t!]);
        this.floatNum(e.t!, `+${e.v}`, 'heal');
        eq.querySelector('.eq-line')!.innerHTML = `${this.diceTxt(e.dd, e.ds)} <span class="op">+</span> <span class="num">${e.b}</span> <span class="op">=</span> <b class="tot heal">+${e.v} can</b>`;
        if (!(await this.sleep(900, tok))) return false;
        mark(e.a, 'act', false); mark(e.t, 'heal', false);
        return true;
      }
      default: {
        // öfke, ayı biçimi, kaçış, bozgun
        const txt = e.sp === 'rout' ? `${this.sideLabel(e.d === 0 ? 'A' : 'B')} bozguna uğradı!` : e.sp === 'rage' ? `${r.units[e.a!].n} öfkeye kapıldı` : e.sp === 'wild' ? `${r.units[e.a!].n} ayıya dönüştü` : e.sp === 'flee' ? `${r.units[e.a!].n} kaçıyor` : '';
        if (txt) this.banner(`${SP_ICON[e.sp!] ?? ''} ${txt}`, e.sp === 'rout' ? 'rout' : 'sp');
        done(e.t !== undefined ? [e.t] : e.a !== undefined && e.a >= 0 ? [e.a] : []);
        if (e.sp === 'rout') this.r.units.forEach((_, i) => this.renderUnit(i));
        return this.sleep(full ? 1100 : 300, tok);
      }
    }
  }

  private async rollDice(specs: DieSpec[], zone: 'hit' | 'dmg' | 'center', side: 'left' | 'right', d20?: number) {
    if (!specs.length) return;
    if (this.tray) { await this.tray.roll(specs, zone, side); return; }
    // WebGL yoksa: düz zar kartları
    const flat = this.root.querySelector<HTMLElement>('.bt-flat')!;
    if (zone !== 'dmg') flat.innerHTML = '';
    flat.insertAdjacentHTML('beforeend', specs.map((s) => `<span class="dchip big d${s.sides}${d20 === 20 ? ' n20' : d20 === 1 ? ' n1' : ''}" style="background:${s.color}">${s.value}</span>`).join(''));
    await new Promise((r) => setTimeout(r, 450 / this.speed));
  }

  private banner(text: string, cls: string) {
    const el = this.root.querySelector<HTMLElement>('.bt-banner')!;
    el.textContent = text;
    el.className = `bt-banner show ${cls}`;
    void el.offsetWidth;
    clearTimeout((el as unknown as { _t: number })._t);
    if (cls !== 'win') (el as unknown as { _t: number })._t = window.setTimeout(() => { el.className = 'bt-banner'; }, 1500 / Math.max(1, this.speed * 0.8));
  }
  private floatNum(i: number, text: string, cls: string) {
    const el = this.uEl[i]; if (!el) return;
    const f = document.createElement('span');
    f.className = `bt-float ${cls}`;
    f.textContent = text;
    el.appendChild(f);
    setTimeout(() => f.remove(), 1200);
  }

  private finish() {
    if (this.finished) return;
    this.finished = true;
    this.playing = false;
    this.syncCtl();
    const r = this.r, b = this.b;
    const win = this.sideLabel(b.winner);
    const why = r.end === 'rout' ? `${r.routed === 'A' ? b.sideA : b.sideB} ${r.rounds}. turda bozguna uğradı` : r.end === 'wipe' ? `karşı tarafta ayakta kimse kalmadı (${r.rounds}. tur)` : r.timeoutWinner ? `${r.maxRounds} tur doldu; savunan mevzisini korudu` : `${r.maxRounds} tur doldu; kalan can fazla olan kazandı`;
    this.banner(`Zafer: ${win}`, 'win');
    const eq = this.root.querySelector<HTMLElement>('.bt-eq')!;
    const wg = r.groups.filter((g) => g.side === b.winner);
    eq.innerHTML = `<div class="eq-result" style="--cc:${this.sideColor(b.winner)}"><div class="bt-res">Kazanan: <b>${esc(win)}</b></div>${wg.length > 1 ? `<div class="sub">${wg.map((g) => esc(g.name)).join(' · ')}</div>` : ''}<div class="sub">${esc(why)} · kayıplar ${b.lossesA} : ${b.lossesB}</div><button class="btn" data-bt="tab" data-v="sum">Zar istatistiğini gör</button></div>`;
    this.renderSummary();
    this.renderAll();
  }

  private renderSummary() {
    const r = this.r, b = this.b, st = this.stats;
    const row = (label: string, f: (s: SideStat) => string, hint = '') => `<tr><th>${label}${hint ? `<small>${hint}</small>` : ''}</th><td>${f(st.side.A)}</td><td>${f(st.side.B)}</td></tr>`;
    const luck = (s: SideStat) => { if (!s.att) return '—'; const avg = s.d20 / s.att; const dv = avg - 10.5; return `${avg.toFixed(1)} <span class="${dv >= 0.8 ? 'up' : dv <= -0.8 ? 'down' : ''}">(${dv >= 0 ? '+' : ''}${dv.toFixed(1)})</span>`; };
    const hist = (s: SideStat, col: string) => {
      const max = Math.max(1, ...s.hist), exp = s.att / 20;
      const bars = s.hist.map((n, k) => `<rect x="${k * 11 + 1}" y="${60 - (n / max) * 56}" width="9" height="${(n / max) * 56}" rx="1.5" fill="${k === 19 ? '#e8c24a' : k === 0 ? '#e0685c' : col}"><title>${k + 1}: ${n} kez</title></rect>`).join('');
      const ey = 60 - (exp / max) * 56;
      return `<svg viewBox="0 0 222 74" class="bt-hist" role="img" aria-label="d20 dağılımı">${bars}<line x1="0" x2="221" y1="${ey}" y2="${ey}" class="exp"/><text x="1" y="72">1</text><text x="100" y="72">10</text><text x="210" y="72">20</text></svg>`;
    };
    const top = st.per.map((p, i) => ({ i, ...p })).filter((p) => p.dmg > 0 || p.heal > 0).sort((x, y) => y.dmg - x.dmg || y.heal - x.heal).slice(0, 4);
    const hr = (s: SideStat) => s.att ? `${pct(s.hit / s.att)} <small>${s.hit}/${s.att}</small>` : '—';
    this.root.querySelector<HTMLElement>('.bt-sum')!.innerHTML = `<div class="bt-sumgrid">
<table class="bt-st"><thead><tr><th></th><th style="color:${this.sideColor('A')}">${esc(this.sideLabel('A'))}</th><th style="color:${this.sideColor('B')}">${esc(this.sideLabel('B'))}</th></tr></thead><tbody>
${row('Saldırı', (s) => String(s.att))}
${row('İsabet', hr)}
${row('Kritik', (s) => String(s.crit))}
${row('Doğal 20 / 1', (s) => `<span class="n20">${s.n20}</span> / <span class="n1">${s.n1}</span>`)}
${row('Ortalama d20', luck, 'beklenen 10,5')}
${row('Verilen hasar', (s) => String(s.dmg))}
${row('İyileştirme', (s) => s.heal ? `+${s.heal}` : '—')}
${row('Devrilen düşman', (s) => String(s.kills))}
</tbody></table>
<div class="bt-hists"><div><div class="lbl" style="color:${this.sideColor('A')}">${esc(this.sideLabel('A'))} · d20 dağılımı</div>${hist(st.side.A, this.sideColor('A'))}</div><div><div class="lbl" style="color:${this.sideColor('B')}">${esc(this.sideLabel('B'))} · d20 dağılımı</div>${hist(st.side.B, this.sideColor('B'))}</div><div class="sub">Kesikli çizgi: adil bir zarda her yüzün beklenen sayısı.</div></div>
<div class="bt-mvp"><div class="lbl">Öne çıkanlar</div>${top.map((p, k) => `<div>${k === 0 ? '★ ' : ''}${this.nm(p.i)} <span class="sub">${esc(unitSub(r.units[p.i]))}</span> · ${p.dmg} hasar${p.kills ? ` · ${p.kills} devirdi` : ''}${p.heal ? ` · +${p.heal} can` : ''}</div>`).join('') || '<div class="sub">—</div>'}</div>
</div>`;
  }

  // ---------------------------------------------------------------- denetim
  private seek(k: number) {
    this.tok++;
    this.tray?.clear(true);
    this.reset();
    k = Math.max(0, Math.min(this.r.ev.length, k));
    for (let j = 0; j < k; j++) this.apply(this.r.ev[j]);
    this.i = k;
    for (const el of this.uEl) el?.classList.remove('act', 'tgt', 'heal');
    this.root.querySelector<HTMLElement>('.bt-eq')!.innerHTML = '';
    this.renderAll();
    if (k >= this.r.ev.length) { this.finish(); return; }
    this.finished = false;
    if (this.playing) void this.run();
  }
  private async stepOnce() {
    this.playing = false; this.syncCtl();
    if (this.i >= this.r.ev.length) return;
    const tok = ++this.tok;
    const k = this.i;
    const ok = await this.show(k, true, tok);
    if (ok && tok === this.tok) { this.i = k + 1; this.renderStatus(); if (this.i >= this.r.ev.length) this.finish(); }
  }
  private togglePlay() {
    if (this.finished && this.i >= this.r.ev.length) { this.playing = true; this.seek(0); this.syncCtl(); return; }
    this.playing = !this.playing;
    this.syncCtl();
    if (this.playing) void this.run(); else this.tok++;
  }
  private syncCtl() {
    const p = this.root.querySelector<HTMLElement>('.bt-play')!;
    p.textContent = this.playing ? '⏸ Durdur' : this.i >= this.r.ev.length ? '↻ Yeniden' : '⏵ Oynat';
    for (const b of this.root.querySelectorAll<HTMLButtonElement>('[data-bt="speed"]')) b.setAttribute('aria-pressed', String(Number(b.dataset.v) === this.speed));
    const k = this.root.querySelector<HTMLInputElement>('[data-bt="key"]'); if (k) k.checked = this.keyOnly;
  }
  private tab(v: string) {
    for (const b of this.root.querySelectorAll<HTMLButtonElement>('.bt-tabs button')) b.setAttribute('aria-pressed', String(b.dataset.v === v));
    this.root.querySelector<HTMLElement>('.bt-log')!.hidden = v !== 'log';
    this.root.querySelector<HTMLElement>('.bt-sum')!.hidden = v !== 'sum';
    this.root.querySelector<HTMLElement>('.bt-rules')!.hidden = v !== 'rules';
    if (v === 'sum') this.renderSummary();
  }
  private onClick(e: MouseEvent) {
    const el = e.target as HTMLElement;
    if (el === this.root) { this.close(); return; }
    const m = el.closest<HTMLElement>('.bt-scrub i');
    if (m) { this.seek(Number(m.dataset.i)); return; }
    const scrub = el.closest<HTMLElement>('.bt-scrub');
    if (scrub) { const rc = scrub.getBoundingClientRect(); this.seek(Math.round(((e.clientX - rc.left) / rc.width) * this.r.ev.length)); return; }
    const b = el.closest<HTMLElement>('[data-bt]');
    if (!b) return;
    switch (b.dataset.bt) {
      case 'close': this.close(); break;
      case 'play': this.togglePlay(); break;
      case 'step': void this.stepOnce(); break;
      case 'back': this.playing = false; this.syncCtl(); this.seek(this.prevKey()); break;
      case 'start': this.seek(0); break;
      case 'end': this.playing = false; this.seek(this.r.ev.length); break;
      case 'speed': this.speed = Number(b.dataset.v); if (this.tray) this.tray.speed = this.speed; try { localStorage.setItem('fd-bt-speed', String(this.speed)); } catch { /* yok */ } this.syncCtl(); break;
      case 'key': this.keyOnly = (b as HTMLInputElement).checked; try { localStorage.setItem('fd-bt-all', this.keyOnly ? '0' : '1'); } catch { /* yok */ } break;
      case 'tab': this.tab(b.dataset.v!); break;
    }
  }
  /** bir önceki anlamlı olay (tur başlıklarını atlar) */
  private prevKey() { let k = this.i - 1; while (k > 0 && this.r.ev[k].sp === 'round') k--; return Math.max(0, k); }
  private onKey(e: KeyboardEvent) {
    if ((e.target as HTMLElement)?.tagName === 'INPUT' && (e.target as HTMLInputElement).type !== 'checkbox') return;
    const k = e.key;
    let used = true;
    if (k === 'Escape') this.close();
    else if (k === ' ' || k === 'k') this.togglePlay();
    else if (k === 'ArrowRight') void this.stepOnce();
    else if (k === 'ArrowLeft') { this.playing = false; this.syncCtl(); this.seek(this.prevKey()); }
    else if (k === 'Home') this.seek(0);
    else if (k === 'End') { this.playing = false; this.seek(this.r.ev.length); }
    else if (k === '1' || k === '2' || k === '4') { this.speed = Number(k); if (this.tray) this.tray.speed = this.speed; this.syncCtl(); }
    else used = false;
    if (used || k.length === 1) { e.stopPropagation(); if (used) e.preventDefault(); }
  }
  close() {
    this.tok++;
    window.removeEventListener('keydown', this.keyH, true);
    window.removeEventListener('resize', this.resizeH);
    this.tray?.dispose();
    this.tray = null;
    this.root.remove();
    document.body.classList.remove('bt-open');
    this.host.onClose();
  }
}
