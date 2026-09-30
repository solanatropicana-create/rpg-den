// Han paneli: haritada hana tıklayınca sağda açılır. Değerler doğrudan simülasyondan okunur.
import type { Sim } from '../sim/sim';
import { YEAR, SEASONS } from '../sim/sim';
import { RACES } from '../data/classes';
import { HERO_CLASS_TR, ALIGN_TR, PATH_TR } from '../data/heroes';
import { GUEST, INN_LEVEL, INN_UPGRADE, STAFF } from '../data/inn';
import { innOcc, innPrices, serveCap, wages, buildStageName, whereStr } from '../sim/innlife';
import { innPool, CONTRACT_YEARS } from '../sim/inns';
import { heroLabel } from '../sim/will';
import type { Agent, Guest, Inn, InnLog } from '../sim/types';
import { ek } from '../sim/tr';

const esc = (s: string) => s.replace(/[&<>"]/g, (c) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;' }[c]!));
const ROMAN = ['', 'I', 'II', 'III'];
const f1 = (v: number) => (Math.abs(v) >= 100 ? Math.round(v).toString() : v.toFixed(1)).replace('.', ',');
const SEAS_SHORT = ['İlkb', 'Yaz', 'Sonb', 'Kış'];

export interface PanelCtx { sim: Sim; civColor: (c: number) => string; canFollow: boolean }

function dayStr(d: number) { return `Y${Math.floor(d / YEAR) + 1} ${SEAS_SHORT[Math.floor((d % YEAR) / (YEAR / 4))]} ${(d % (YEAR / 4)) + 1}`; }
function stars(fame: number) { const n = Math.round(fame / 20); return '★'.repeat(n) + '☆'.repeat(5 - n); }
function eta(a: Agent) { return Math.max(1, Math.ceil((a.path.length - 1 - a.step - a.progress) / Math.max(0.1, a.speed) * 1.15)); }

/** tüm hanlar: kısa kartlar, tıklayınca ayrıntı */
export function renderInnList(c: PanelCtx): string {
  const { sim } = c, w = sim.w;
  if (!w.inns.length) return `<p class="empty">Henüz han yok. Hancılar yerleşimlerden yola çıkınca burada görünecek.</p>`;
  const banned = w.civs.filter((x) => (x.innBanUntil ?? 0) > sim.day);
  let out = `<p class="sub">Hanlar kimseye ait değildir. Hancılar yerleşimlerden öküz arabasıyla çıkıp sınır bölgelerinde kendi hanlarını kurar; yolcu, kervan ve kahraman ağırlar, erzakı çevredeki yerleşimlerden satın alır. Bir hana tıkla.</p>`;
  if (banned.length) out += `<div class="chips">${banned.map((x) => `<span class="chip war">Han Bozan: ${esc(x.name)} (Y${Math.floor(x.innBanUntil! / YEAR) + 1}'e dek)</span>`).join('')}</div>`;
  for (const inn of w.inns) {
    const o = innOcc(inn), L = INN_LEVEL[Math.max(1, inn.level)];
    const state = inn.stage === 'road' ? `Hancı ${esc(inn.keeper)} yolda` : inn.stage === 'build' ? `İnşaat %${Math.round(Math.min(1, (inn.build?.work ?? 0) / (inn.build?.need ?? 1)) * 100)} · ${buildStageName(inn)}` : inn.stage === 'ruin' ? 'Harabe' : `${L.name} · ${o.persons}/${L.rooms} misafir`;
    out += `<button class="innc ${inn.stage}" data-inn-open="${inn.id}"><span class="ic">${inn.stage === 'build' || inn.stage === 'road' ? '🔨' : inn.stage === 'ruin' ? '✝' : '🍺'}</span><span class="nm"><b>${esc(inn.name)} Hanı</b><small>${state}</small></span>`
      + `<span class="r">${inn.stage === 'open' ? `${Math.round(inn.gold)} altın<small>ün ${Math.round(inn.fame)}</small>` : inn.stage === 'ruin' ? '' : `${Math.round(inn.gold)} altın`}</span></button>`;
  }
  return out;
}

function logRow(e: InnLog): string {
  const ic = { in: '↘', out: '↗', buy: '🛒', build: '🔨', staff: '👤', ev: '•', no: '✕' }[e.k];
  const g = e.g !== undefined && Math.abs(e.g) >= 0.05 ? `<span class="g ${e.g < 0 ? 'neg' : 'pos'}">${e.g > 0 ? '+' : '−'}${f1(Math.abs(e.g))}</span>` : '<span></span>';
  return `<div class="k-${e.k}"><span class="t">${dayStr(e.day)}</span><span class="i">${ic}</span><span class="tx">${esc(e.t)}</span>${g}</div>`;
}

function guestRow(c: PanelCtx, inn: Inn, g: Guest): string {
  const { sim } = c;
  const def = GUEST[g.kind];
  const night = sim.day - g.arrived + 1;
  if (g.kind === 'hero' && g.hero !== undefined) {
    const h = sim.hero(g.hero);
    const tab = Math.round(inn.tabs[h.id] ?? 0);
    const bids = h.auction?.bids.length ?? 0;
    return `<div class="row"><span class="ic">★</span><span class="nm"><b>${esc(heroLabel(h))}</b> <span class="lvl">Sv ${h.level}</span>${c.canFollow ? ` <button class="btn sm" data-follow-hero="${h.id}">İzle</button>` : ''}`
      + `<small>${RACES[h.race].name} ${HERO_CLASS_TR[h.cls]} · ${ALIGN_TR[h.align]} · ${PATH_TR[h.path]}${h.auction ? ` · açık artırmada${bids ? ` (${bids} teklif)` : ''}` : ''}</small>`
      + `<small>${esc(g.why)}${tab >= 10 ? ` · veresiye ${f1(tab / 10)} altın` : ''}</small></span>`
      + `<span class="r">${night}. gece${g.stable ? '<small>ortak salon</small>' : ''}</span></div>`;
  }
  const civ = g.civ >= 0 ? sim.w.civs[g.civ] : undefined;
  return `<div class="row"><span class="ic" title="${def.name}">${def.icon}</span><span class="nm"><b>${esc(g.name)}</b>${g.n > 1 ? ` <span class="lvl">+${g.n - 1} kişi</span>` : ''}`
    + `<small>${def.name} · ${RACES[g.race]?.name ?? ''}${civ ? ` · <i style="color:${c.civColor(civ.id)}">■</i> ${esc(civ.name)}` : ''}</small>`
    + `<small>${esc(g.fromName)} → ${esc(g.toName)} · ${esc(g.why)}</small></span>`
    + `<span class="r">${night}/${Math.max(1, g.nights)}. gece${g.stable ? '<small>ahırda</small>' : ''}<small>${f1(g.spent)} altın</small><small class="mood" title="Memnuniyet">${Math.round(g.mood * 100)}%</small></span></div>`;
}

/** son 2 yıl: 10 günlük misafir sayısı (çubuk) ve kasa (ayrı çizgi) — tek eksen, iki küçük grafik */
function charts(inn: Inn): string {
  const h = inn.hist.slice(-24);
  if (h.length < 3) return '';
  const W = 400, H = 58, pad = 2;
  const cap = Math.max(INN_LEVEL[Math.max(1, inn.level)].rooms + INN_LEVEL[Math.max(1, inn.level)].stable, ...h.map((x) => x.guests), 1);
  const bw = (W - pad * 2) / 24;
  const bars = h.map((x, i) => {
    const bh = Math.max(x.guests ? 2 : 0, (x.guests / cap) * (H - 12));
    const bx = pad + (24 - h.length + i) * bw;
    return `<g><rect x="${(bx + 1).toFixed(1)}" y="${(H - bh).toFixed(1)}" width="${(bw - 2).toFixed(1)}" height="${bh.toFixed(1)}" rx="2" class="bar"/><rect x="${bx.toFixed(1)}" y="0" width="${bw.toFixed(1)}" height="${H}" class="hit"><title>${dayStr(x.day)}: ${x.guests} misafir</title></rect></g>`;
  }).join('');
  const rooms = INN_LEVEL[Math.max(1, inn.level)].rooms;
  const ry = H - (rooms / cap) * (H - 12);
  const gmax = Math.max(10, ...h.map((x) => x.gold)), gmin = Math.min(0, ...h.map((x) => x.gold));
  const gy = (v: number) => 4 + (1 - (v - gmin) / (gmax - gmin || 1)) * (H - 10);
  const pts = h.map((x, i) => `${(pad + (24 - h.length + i) * bw + bw / 2).toFixed(1)},${gy(x.gold).toFixed(1)}`).join(' ');
  const last = h[h.length - 1];
  const dots = h.map((x, i) => `<circle cx="${(pad + (24 - h.length + i) * bw + bw / 2).toFixed(1)}" cy="${gy(x.gold).toFixed(1)}" r="6" class="hit"><title>${dayStr(x.day)}: ${x.gold} altın</title></circle>`).join('');
  return `<figure class="ichart"><figcaption>Misafir sayısı, 10 günde bir <span>kesikli çizgi: ${rooms} oda</span></figcaption>`
    + `<svg viewBox="0 0 ${W} ${H + 1}" role="img" aria-label="Son iki yılda misafir sayısı">${bars}<line x1="0" x2="${W}" y1="${ry.toFixed(1)}" y2="${ry.toFixed(1)}" class="cap"/><line x1="0" x2="${W}" y1="${H}" y2="${H}" class="base"/></svg></figure>`
    + `<figure class="ichart"><figcaption>Kasa (altın) <span>şimdi ${last.gold}</span></figcaption>`
    + `<svg viewBox="0 0 ${W} ${H + 1}" role="img" aria-label="Son iki yılda kasa">${gmin < 0 ? `<line x1="0" x2="${W}" y1="${gy(0).toFixed(1)}" y2="${gy(0).toFixed(1)}" class="base"/>` : `<line x1="0" x2="${W}" y1="${H}" y2="${H}" class="base"/>`}<polyline points="${pts}" class="line"/>${dots}</svg></figure>`;
}

function bookTable(inn: Inn, sim: Sim): string {
  const cur = inn.books[inn.books.length - 1];
  const year = inn.books.slice(-4);
  const sum = (k: keyof typeof cur) => year.reduce((a, b) => a + (b[k] as number), 0);
  const rows: [string, (b: typeof cur) => number, boolean][] = [
    ['Oda', (b) => b.room, false], ['Yemek', (b) => b.food, false], ['İçki', (b) => b.ale, false], ['Yol parası, pay, diğer', (b) => b.other, false],
    ['Erzak alımı', (b) => -b.supply, true], ['Ücretler', (b) => -b.wage, true], ['İnşaat', (b) => -b.build, true],
  ];
  const net = (b: typeof cur) => b.room + b.food + b.ale + b.other - b.supply - b.wage - b.build;
  if (!cur) return '';
  const yr = { ...cur, room: sum('room'), food: sum('food'), ale: sum('ale'), other: sum('other'), supply: sum('supply'), wage: sum('wage'), build: sum('build') };
  const seasonName = SEASONS[cur.season % 4];
  let h = `<table class="book"><thead><tr><th></th><th>${seasonName}</th><th>Son 4 mevsim</th></tr></thead><tbody>`;
  for (const [n, f, neg] of rows) { const a = f(cur), b = f(yr); if (Math.abs(b) < 0.05) continue; h += `<tr class="${neg ? 'neg' : ''}"><td>${n}</td><td>${a ? f1(a) : '–'}</td><td>${f1(b)}</td></tr>`; }
  h += `<tr class="sum"><td>Net</td><td>${f1(net(cur))}</td><td>${f1(net(yr))}</td></tr></tbody></table>`;
  h += `<div class="sub mono">${sim.day - inn.founded > 0 ? `Açıldığından beri: ${inn.total.guests} misafir · ${inn.total.nights} gece · ${Math.round(inn.total.income)} altın gelir · ${Math.round(inn.total.bought)} altınlık alım · ${inn.total.turned} kişi yer bulamadı` : ''}</div>`;
  return h;
}

/** tek han: canlı değerler */
export function renderInnDetail(c: PanelCtx, inn: Inn): string {
  const { sim } = c, w = sim.w;
  const L = INN_LEVEL[Math.max(1, inn.level)];
  const keeperAgent = w.agents.find((a) => !a.dead && a.kind === 'keeper' && a.inn === inn.id);
  let h = `<div class="innp">`;
  h += `<div class="ihead"><span class="badge">${inn.stage === 'open' ? '🍺' : inn.stage === 'ruin' ? '✝' : '🔨'}</span><div>`
    + `<div class="lvl">${inn.stage === 'open' ? `${L.name} · kademe ${ROMAN[inn.level]}` : inn.stage === 'build' ? 'İnşaat hâlinde' : inn.stage === 'road' ? 'Hancı yolda' : 'Harabe'}</div>`
    + `<div class="sub">Hancı <b>${esc(inn.keeper)}</b> (${RACES[inn.keeperRace]?.name ?? ''}) · ${esc(inn.originName)} kökenli</div>`
    + `<div class="sub">${esc(whereStr(sim, inn.tile))}${inn.stage === 'open' ? ` · açılış ${sim.dateStr(inn.founded)}` : ''}</div>`
    + (inn.stage === 'open' ? `<div class="fame" title="Ün: memnuniyet, efsaneler ve savunulan baskınlarla artar"><span>${stars(inn.fame)}</span> ün ${Math.round(inn.fame)} · memnuniyet %${Math.round(inn.sat * 100)}</div>` : '')
    + `</div></div>`;
  h += `<div class="acts">${c.canFollow ? `<button class="btn sm" data-inn-focus="${inn.id}">Hana git</button>` : ''}${keeperAgent && c.canFollow ? `<button class="btn sm" data-follow-agent="${keeperAgent.id}">Hancıyı izle</button>` : ''}<button class="btn sm ghost" data-inn-list>Tüm hanlar</button></div>`;

  // ---- yolda / inşaat / harabe
  if (inn.stage === 'road') {
    const a = keeperAgent;
    h += `<div class="ibuild"><b>Hancı ${esc(inn.keeper)} öküz arabasıyla yolda.</b><div class="sub">${esc(inn.originName)} → ${esc(whereStr(sim, inn.tile))}${a ? ` · ~${eta(a)} gün` : ''}</div>`
      + `<div class="sub">Arabada ${Math.round(inn.stock.wood)} kereste, ${Math.round(inn.stock.food)} porsiyon azık · kesede ${Math.round(inn.gold)} altın · yanında ${inn.staff.map((x) => `${esc(x.name)} (${STAFF[x.role].name.toLocaleLowerCase('tr')})`).join(', ')}</div>`
      + (a ? `<div class="tr"><i style="width:${Math.round(((a.step + a.progress) / Math.max(1, a.path.length - 1)) * 100)}%"></i></div>` : '') + `</div>`;
  }
  if (inn.build) {
    const b = inn.build;
    const pct = Math.round(Math.min(1, b.work / b.need) * 100);
    const rate = 1 + inn.staff.length + (inn.gold > 30 ? (inn.gold > 90 ? 2 : 1) : 0);
    h += `<div class="ibuild"><div class="bh"><b>${inn.stage === 'open' ? `Genişletme: ${INN_LEVEL[b.level].name} olacak` : buildStageName(inn)}</b><span class="mono">%${pct}</span></div>`
      + `<div class="tr"><i style="width:${pct}%"></i></div>`
      + `<div class="mats"><span>Emek <b>${Math.round(b.work)}/${b.need}</b></span><span>Kereste <b>${Math.floor(b.wood)}/${b.woodNeed}</b></span>${b.stoneNeed ? `<span>Taş <b>${Math.floor(b.stone)}/${b.stoneNeed}</b></span>` : ''}<span>Ekip <b>${rate}</b></span></div>`
      + `<div class="sub">${sim.dateStr(b.started)} başladı${inn.order ? ` · 🛒 ${esc(ek(inn.order.fromName, 'dan'))} ${esc(inn.order.goods)} yolda` : ''}</div></div>`;
  }
  if (inn.stage === 'ruin') {
    const left = Math.max(0, 3 * YEAR - (sim.day - (inn.ruinedDay ?? sim.day)));
    const camps = w.camps.filter((x) => x.alive && sim.g.dist(x.tile, inn.tile) <= 10);
    h += `<div class="ibuild"><b>Han ${sim.dateStr(inn.ruinedDay ?? sim.day)} yıkıldı.</b><div class="sub">${left > 0 ? `Yeni bir hancı en erken ${Math.ceil(left / (YEAR / 4))} mevsim sonra gelir` : 'Yeni bir hancı her an yola çıkabilir'}${camps.length ? `; önce yakındaki ${camps.map((x) => esc(x.name)).join(', ')} temizlenmeli` : ''}.</div></div>`;
  }

  if (inn.stage === 'open') {
    const o = innOcc(inn), pr = innPrices(inn);
    const cur = inn.books[inn.books.length - 1];
    const inc = cur ? cur.room + cur.food + cur.ale + cur.other : 0, exp = cur ? cur.supply + cur.wage + cur.build : 0;
    const coming = w.agents.filter((a) => !a.dead && a.kind === 'traveler' && a.purpose === 'come' && a.inn === inn.id);
    const rate = coming.length;
    h += `<div class="kpis">`
      + `<div><b>${Math.round(inn.gold)}</b><span>altın kasada</span></div>`
      + `<div><b>${o.rooms}/${L.rooms}</b><span>oda dolu${o.stable ? ` · ahırda ${o.stable}` : ''}</span></div>`
      + `<div><b class="${inc - exp >= 0 ? 'pos' : 'neg'}">${inc - exp >= 0 ? '+' : '−'}${f1(Math.abs(inc - exp))}</b><span>bu mevsim net</span></div>`
      + `<div><b>${rate}</b><span>yolcu yolda</span></div></div>`;
    // kiler
    const eatPerDay = inn.guests.reduce((a, g) => a + g.n * GUEST[g.kind].eat, 0) + 1 + inn.staff.length;
    const alePerDay = inn.guests.reduce((a, g) => a + g.n * GUEST[g.kind].mugs, 0);
    const bar = (name: string, v: number, cap: number, unit: string, per: number) => {
      const p = Math.max(0, Math.min(1, v / cap)), days = per > 0.05 ? Math.floor(v / per) : null;
      return `<div class="sbar ${p < 0.12 ? 'empty' : p < 0.35 ? 'low' : ''}"><span>${name}</span><span class="tr"><i style="width:${Math.round(p * 100)}%"></i></span><span class="mono">${Math.floor(v)}/${cap} ${unit}${days !== null ? ` · ~${days} gün` : ''}</span></div>`;
    };
    h += `<div class="lbl">Kiler</div>` + bar('Yiyecek', inn.stock.food, L.food, 'porsiyon', eatPerDay) + bar('Bira', inn.stock.ale, L.ale, 'bardak', alePerDay) + bar('Odun', inn.stock.wood, L.wood, 'yük', sim.season === 3 ? 0.5 : 0.25);
    if (inn.order) {
      const a = w.agents.find((x) => x.id === inn.order!.agent && !x.dead);
      h += `<div class="sub">🛒 Erzak arabası yolda: ${esc(ek(inn.order.fromName, 'dan'))} ${esc(inn.order.goods)} (${f1(inn.order.cost)} altın)${a ? ` · ~${eta(a)} gün${c.canFollow ? ` <button class="btn sm" data-follow-agent="${a.id}">İzle</button>` : ''}` : ''}</div>`;
    }
    h += `<div class="lbl">Tarife</div><div class="chips"><span class="chip">Oda ${pr.room} gümüş</span><span class="chip">Ahır/ortak salon ${Math.ceil(pr.room / 2)}</span><span class="chip">Yemek ${pr.meal}</span><span class="chip">Bira ${pr.ale}/bardak</span><span class="chip ghost">10 gümüş = 1 altın</span></div>`;
    h += `<div class="sub">Servis: ${1 + inn.staff.length} kişi, ${serveCap(inn)} misafire yetişir${o.persons > serveCap(inn) ? ' — <b class="neg">yetişemiyor</b>' : ''} · yolcu akışı ${Math.round(inn.traffic)}</div>`;

    // handakiler
    const here = inn.guests.slice().sort((a, b) => (a.kind === 'hero' ? 0 : 1) - (b.kind === 'hero' ? 0 : 1) || a.arrived - b.arrived);
    h += `<div class="lbl">Şu an handa (${o.persons} kişi)</div>`;
    h += here.length ? here.map((g) => guestRow(c, inn, g)).join('') : `<div class="sub">Han boş; ocak başında yalnız hancı var.</div>`;
    // kervanlar dahil konaklayan ajanlar zaten misafir listesinde
    // yoldakiler
    const leaving = w.agents.filter((a) => !a.dead && a.kind === 'traveler' && a.purpose === 'leave' && a.inn === inn.id && a.step < 6);
    if (coming.length || leaving.length) {
      h += `<div class="lbl">Yolda</div>`;
      for (const a of coming.slice(0, 8)) {
        const g = a.guest!;
        h += `<div class="row"><span class="ic">↘</span><span class="nm"><b>${GUEST[g.kind].icon} ${esc(g.name)}</b>${g.n > 1 ? ` <span class="lvl">+${g.n - 1}</span>` : ''}<small>${GUEST[g.kind].name} · ${esc(ek(g.fromName, 'dan'))} geliyor · ${esc(g.why)}</small></span><span class="r">~${eta(a)} gün${c.canFollow ? `<br><button class="btn sm" data-follow-agent="${a.id}">İzle</button>` : ''}</span></div>`;
      }
      for (const a of leaving.slice(0, 5)) {
        const g = a.guest!;
        h += `<div class="row out"><span class="ic">↗</span><span class="nm"><b>${GUEST[g.kind].icon} ${esc(g.name)}</b><small>${esc(g.toName)} yolunda · handa ${f1(g.spent)} altın bıraktı</small></span><span class="r">${c.canFollow ? `<button class="btn sm" data-follow-agent="${a.id}">İzle</button>` : ''}</span></div>`;
      }
    }
  }

  // gelen giden defteri
  if (inn.log.length) {
    h += `<div class="lbl">Gelen / giden defteri</div><div class="led">${inn.log.slice(-16).reverse().map(logRow).join('')}</div>`;
  }
  if (inn.stage === 'open') {
    h += charts(inn);
    h += `<div class="lbl">Kasa defteri (altın)</div>` + bookTable(inn, sim);
    const nx = INN_UPGRADE[inn.level + 1];
    if (nx && !inn.build) {
      const miss = [sim.year < nx.year ? `Y${nx.year}` : '', inn.fame < nx.fame ? `ün ${nx.fame}` : '', inn.gold < nx.gold + 30 ? `${nx.gold + 30} altın` : '', inn.turned < 5 ? 'dolu odalar' : ''].filter(Boolean);
      h += `<div class="sub">Sonraki kademe: <b>${INN_LEVEL[inn.level + 1].name}</b> (${INN_LEVEL[inn.level + 1].rooms} oda) — ${miss.length ? `gerekenler: ${miss.join(', ')}` : 'hancı genişletmeyi düşünüyor'}</div>`;
    }
  }
  // personel
  if (inn.staff.length) {
    h += `<div class="lbl">Personel (${inn.staff.length}) · mevsimlik ücret ${f1(wages(inn))} altın</div>`;
    h += inn.staff.map((x) => `<div class="row"><span class="ic">👤</span><span class="nm"><b>${esc(x.name)}</b> <span class="lvl">${STAFF[x.role].name}</span><small>${RACES[x.race]?.name ?? ''} · ${esc(x.from)} · ${esc(ek(sim.dateStr(x.since), 'dan'))} beri</small></span><span class="r">${f1(STAFF[x.role].wage)}<small>altın/mevsim</small></span></div>`).join('');
  }
  // pano ve açık artırma
  if (inn.stage === 'open') {
    const quests = w.quests.filter((q) => q.open && q.inn === inn.id);
    h += `<div class="lbl">İlan panosu</div>`;
    h += quests.length ? `<ul class="board">${quests.map((q) => {
      const cp = w.camps.find((x) => x.id === q.camp);
      const who = q.civ >= 0 ? `<span style="color:${c.civColor(q.civ)}">■</span> ${esc(w.civs[q.civ].name)}` : `Hancı`;
      return `<li><b>${esc(cp?.name ?? '?')}</b> · ${q.bounty} altın${q.topped ? ` (hancı +${q.topped})` : ''} · ${who}${q.failures ? ` · ${q.failures} başarısız` : ''}${q.expires ? ` · Y${Math.floor(q.expires / YEAR) + 1}'e dek` : ''}</li>`;
    }).join('')}</ul>` : `<div class="sub">Açık ilan yok.</div>`;
    const pool = innPool(sim, inn).filter((x) => x.auction);
    if (pool.length) {
      h += `<div class="lbl">Açık artırma (${CONTRACT_YEARS} yıllık sözleşme)</div>`;
      h += pool.map((x) => {
        const bids = (x.auction?.bids ?? []).slice().sort((p, q) => q.gold - p.gold);
        return `<div class="row"><span class="ic">⚖</span><span class="nm"><b>${esc(heroLabel(x))}</b> <span class="lvl">Sv ${x.level}</span><small>${bids.length ? bids.map((b) => `<span style="color:${c.civColor(b.civ)}">■</span> ${b.gold}`).join(' · ') : 'henüz teklif yok'}</small></span><span class="r">kapanış<small>${sim.dateStr(x.auction!.end)}</small></span></div>`;
      }).join('');
    }
  }
  const graves = w.heroes.filter((x) => x.state === 'dead' && x.baseInn && x.base === inn.id);
  const legends = w.heroes.filter((x) => x.legend && x.baseInn && x.base === inn.id);
  const teacher = inn.teacher !== undefined ? w.heroes.find((x) => x.id === inn.teacher) : undefined;
  if (graves.length || legends.length || teacher) {
    h += `<div class="lbl">Hanın hatırası</div><div class="sub">${teacher && teacher.state === 'retired' ? `Öğretmen: ${esc(heroLabel(teacher))}. ` : ''}${legends.length ? `Avluda heykeli dikilen efsaneler: ${legends.map((x) => esc(heroLabel(x))).join(', ')}. ` : ''}${graves.length ? `Mezarlıkta ${graves.length} kahraman yatıyor: ${graves.slice(-5).map((x) => esc(x.name)).join(', ')}${graves.length > 5 ? '…' : ''}.` : ''}${inn.raids ? ` ${inn.raids} baskın gördü.` : ''}</div>`;
  }
  return h + `</div>`;
}
