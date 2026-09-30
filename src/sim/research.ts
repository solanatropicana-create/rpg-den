// Araştırma seçimi, çağ atlama, alt sınıf doktrinleri ve yıllık sınıf yetenekleri.
import type { Sim } from './sim';
import { FOOD_PER_POP, YEAR } from './sim';
import { ALL_TECHS, TECH, ERA_RULE, ERA_TR, techCost, type TechDef, type Era } from '../data/techs';
import { CLASSES, UNITS } from '../data/classes';
import { DEPOSITS, EXTRACTS } from '../data/goods';
import type { Civ } from './types';
import { ek } from './tr';
import { gainXp } from './heroes';
import { RACES, type RaceId } from '../data/classes';

export function availableTechs(s: Sim, c: Civ): TechDef[] {
  return ALL_TECHS.filter((t) => !s.has(c, t.id) && (t.tree === 'main' || t.tree === c.cls) && t.era <= c.era
    && t.req.every((r) => s.has(c, r)) && gateOk(s, c, t));
}
export function gateOk(s: Sim, c: Civ, t: TechDef) {
  return (!t.gate || t.gate.every((k) => s.access(c, k))) && (!t.gateAny || t.gateAny.some((k) => s.access(c, k)));
}
export function blockedByGate(s: Sim, c: Civ): TechDef[] {
  return ALL_TECHS.filter((t) => !s.has(c, t.id) && (t.tree === 'main' || t.tree === c.cls) && t.era <= c.era && t.req.every((r) => s.has(c, r)) && !gateOk(s, c, t));
}

export function chooseResearch(s: Sim, c: Civ) {
  let avail = availableTechs(s, c);
  c.research.hard = false;
  if (!avail.length) {
    // kaynak kapısına takıldı: uzun ve pahalı yoldan (başka yöntemlerle) öğrenmeye çalışır
    avail = blockedByGate(s, c);
    if (!avail.length) { c.research.current = null; return; }
    c.research.hard = true;
  }
  const cls = CLASSES[c.cls];
  const ss = s.civSettlements(c);
  const pop = s.civPop(c);
  const daysFood = s.foodTotal(c) / Math.max(0.1, pop * FOOD_PER_POP);
  const housingFull = ss.some((x) => s.pop(x) >= s.housing(x) - 1);
  const war = s.inWar(c);
  const threat = Math.min(1, c.threat);
  const eraNeed = c.era < 4 ? ERA_RULE[(c.era + 1) as 2 | 3 | 4].nodes - s.w.civs[c.id].research.done.filter((t) => TECH[t].tree === 'main' && TECH[t].era === c.era).length : 0;
  // sahip olunan yapılar ve yükseltme potansiyeli
  const extCount: Record<string, number> = {};
  for (const t of s.w.tiles) if (t.ext && t.owner >= 0 && s.settlement(t.owner)?.civ === c.id) extCount[t.ext.kind] = (extCount[t.ext.kind] ?? 0) + 1;
  const reasons: Record<string, [number, string][]> = {};
  const add = (id: string, v: number, why: string) => { (reasons[id] ??= []).push([v, why]); };
  for (const t of avail) {
    const pref = t.tree === 'main' ? (cls.prefer[t.chain!] ?? 1) : (cls.prefer.class ?? 1.2);
    add(t.id, 3 * pref, t.tree === 'main' ? `${cls.name} geleneği` : `${cls.name} yolu`);
    if (t.tree === 'main' && t.era === c.era && eraNeed > 0) add(t.id, 4, `${ERA_TR[c.era + 1]} çağına geçmek için`);
    if (t.subclass) add(t.id, c.era >= 3 ? 25 : 9, 'Kimliğini seçme zamanı');
    // yükseltme açan düğümler
    for (const k of Object.keys(EXTRACTS) as (keyof typeof EXTRACTS)[]) {
      const lv = EXTRACTS[k].tech.indexOf(t.id);
      if (lv >= 0 && extCount[k]) add(t.id, Math.min(10, extCount[k] * 2.5), `${extCount[k]} ${EXTRACTS[k].names[Math.max(0, lv - 1)].toLocaleLowerCase('tr')} yükseltilebilir`);
      if (lv === 0 && k === 'mine' && s.w.deposits.some((d) => DEPOSITS[d.kind].building === 'mine' && s.depositVisible(c, d) && d.tiles.some((x) => s.tileCiv(x) === c.id))) add(t.id, 7, 'Topraklarında maden damarı var');
    }
    if (daysFood < 40 && t.chain === 'gida' && t.tree === 'main') add(t.id, 14, 'Kıtlık kapıda');
    if (['agriculture', 'husbandry', 'fishing'].includes(t.id) && !['agriculture', 'husbandry', 'fishing'].some((x) => s.has(c, x))) add(t.id, 9, 'Kalıcı bir gıda kaynağı');
    switch (t.id) {
      case 'agriculture': add(t.id, daysFood < 60 ? 22 : 5, daysFood < 60 ? 'Gıda stoğu azalıyor' : 'Düzenli gıda'); break;
      case 'woodwork': add(t.id, housingFull ? 10 : 4, housingFull ? 'Barınak yetersiz' : 'Kalıcı evler'); break;
      case 'pottery': add(t.id, 5, 'Depo ve fırın'); break;
      case 'bronze': add(t.id, 6, 'Bronz; ilk askerlerin yolu'); break;
      case 'training': add(t.id, 2 + threat * 10 + (war ? 12 : 0) + cls.aggression * 8, threat > 0.3 ? 'Goblin baskınları' : war ? 'Savaş' : 'Ordu kurma isteği'); break;
      case 'tavern': add(t.id, 5 + threat * 4, 'Kahramanları çekmek'); break;
      case 'roads': add(t.id, pop > 13 ? 8 : 2, pop > 13 ? 'Kalabalık yeni toprak istiyor' : 'Bağlantı'); break;
      case 'barter': add(t.id, 4, 'Komşularla alışveriş'); break;
      case 'caravans': add(t.id, 4 + (cls.prefer.ticaret ?? 1) * 2, 'Uzak pazarlar'); break;
      case 'stonewalls': add(t.id, 2 + threat * 8 + (war ? 8 : 0), 'Surlar'); break;
      case 'smithing': add(t.id, 6 + (war ? 6 : 0), 'Çelik'); break;
      case 'arcana1': add(t.id, c.cls === 'wizard' ? 10 : 2, 'Büyünün sırrı'); break;
      case 'writing': add(t.id, 5, 'Bilgiyi kayda geçirmek'); break;
    }
  }
  const score = (id: string) => reasons[id].reduce((a, [v]) => a + v, 0) * Math.pow(150 / techCost(TECH[id]), 0.3);
  const pick = s.rng.weighted(avail, (t) => score(t.id) ** 2)!;
  const top = reasons[pick.id].slice().sort((a, b) => b[0] - a[0])[0];
  c.research.current = pick.id;
  c.research.progress = 0;
  c.research.reason = c.research.hard ? `Kaynağı yok (${(pick.gate ?? pick.gateAny ?? []).filter((k) => !s.access(c, k)).map((k) => GATE_TR[k] ?? k).join(', ')}); uzun ve pahalı yoldan` : top[1];
}

export function onTechDone(s: Sim, c: Civ, id: string) {
  const t = TECH[id];
  const cls = CLASSES[c.cls];
  s.metric('techs');
  s.metric(`research_${c.id}`);
  if (t.subclass) {
    const ctx = { war: s.inWar(c), threat: Math.min(1, c.threat), forest: forestShare(s, c), law: c.align.law, good: c.align.good, pop: s.civPop(c) };
    const sub = s.rng.weighted(cls.subclasses, (x) => x.pick(ctx))!;
    c.subclass = sub.id;
    s.recomputeEff(c);
    s.metric('subclass');
    s.log('class', `${c.name} ${t.name} ile yolunu seçti: ${sub.name}.`, { civ: c.id, major: true, cause: `${sub.desc}` });
  } else if (t.capstone) {
    s.recomputeEff(c);
    s.log('class', `${c.name} sınıfının doruğuna ulaştı: ${s.capName(c)}!`, { civ: c.id, major: true, cause: CLASSES[c.cls].subclasses.find((x) => x.id === c.subclass)?.capDesc });
  } else {
    s.recomputeEff(c);
    const big = t.tree !== 'main' || t.era >= 3 || !!t.unit || ['tavern', 'training', 'roads', 'bronze', 'smithing', 'arcana1', 'deepmine', 'currency'].includes(id);
    s.log('research', `${c.name} ${t.name} araştırmasını tamamladı: ${t.unlock.charAt(0).toLocaleLowerCase('tr')}${t.unlock.slice(1)}.`, { civ: c.id, cause: c.research.reason, major: big });
  }
  if (t.unit) s.log('class', `${c.name} yeni bir birlik kurabiliyor: ${UNITS[t.unit].name}.`, { civ: c.id });
  if (id === 'arcana1' || id === 'deepmine') {
    const kind = id === 'arcana1' ? 'mana' : 'mithril';
    const seen = s.w.deposits.filter((d) => d.kind === kind && d.knownBy.includes(c.id));
    if (seen.length) s.log('discover', `${c.name} artık ${DEPOSITS[kind].name.toLocaleLowerCase('tr')} yataklarını görebiliyor (${seen.length}).`, { civ: c.id, tile: seen[0].tiles[0], major: true });
  }
  chooseResearch(s, c);
}

const GATE_TR: Record<string, string> = { water: 'su kenarı', fertile: 'verimli ova', clay: 'kil', copper: 'bakır', tin: 'kalay', iron: 'demir', coal: 'kömür', gold: 'altın', silver: 'gümüş', horses: 'at sürüsü', salt: 'tuz', herbs: 'şifalı ot', mana: 'mana', mithril: 'mithril', heartwood: 'kadim ağaç' };
function forestShare(s: Sim, c: Civ) {
  let f = 0, n = 0;
  for (const t of s.w.tiles) if (t.owner >= 0 && s.settlement(t.owner)?.civ === c.id) { n++; if (t.terrain === 'forest' || t.terrain === 'oldforest') f++; }
  return n ? f / n : 0;
}

export function eraCheck(s: Sim, c: Civ) {
  if (c.era >= 4) return;
  const next = (c.era + 1) as 2 | 3 | 4;
  const rule = ERA_RULE[next];
  const done = c.research.done.filter((t) => TECH[t].tree === 'main' && TECH[t].era === c.era).length;
  const maxPop = Math.max(0, ...s.civSettlements(c).map((x) => s.pop(x)));
  if (done >= rule.nodes && maxPop >= rule.pop) {
    c.era = next as Era;
    c.eraDay[next] = s.day;
    s.metric(`era${next}`);
    s.updateTerritory();
    s.log('era', `${c.name} ${ERA_TR[next]} çağına geçti!`, { civ: c.id, major: true, cause: `${done} ${ERA_TR[next - 1]} düğümü tamam, en büyük yerleşim ${maxPop} nüfus` });
    if (!c.research.current) chooseResearch(s, c);
  }
}

/** Yılda bir tetiklenen sınıf yetenekleri */
export function classYearly(s: Sim, c: Civ) {
  const cap = s.capital(c);
  if (!cap) return;
  const contacts = s.w.civs.filter((o) => o.id !== c.id && o.alive && s.rel(c.id, o.id).contact);
  // Rahip: Kanalize İlahiyat bayramı
  if (s.e(c, 'festival') > 0) {
    let what = '';
    switch (c.subclass) {
      case 'forge': s.add(c, 'arms', 2); s.add(c, 'tools', 3); what = 'Örs tanrısı silah ve alet bağışladı'; break;
      case 'war': for (const st of s.civSettlements(c)) st.soldiers += 1; what = 'Her yerleşime bir kutsal savaşçı katıldı'; break;
      case 'life': s.addPop(cap, c.race, 3); what = 'Başkentte üç sağlıklı çocuk doğdu'; break;
      default: s.add(c, 'grain', 40); what = 'Ambarlar bereketlendi';
    }
    for (const h of s.civHeroes(c)) h.hp = h.maxHp;
    s.log('class', `${c.name} Kanalize İlahiyat bayramını kutladı: ${what.toLocaleLowerCase('tr')}.`, { civ: c.id, tile: cap.tile });
  }
  // Druid: orman büyür
  const fg = s.e(c, 'forestGrow');
  if (fg > 0) {
    let grown = 0;
    for (const st of s.civSettlements(c)) {
      for (const ti of s.g.within(st.tile, s.radiusOf(st))) {
        if (grown >= fg * 2) break;
        const t = s.w.tiles[ti];
        if (t.owner !== st.id || t.terrain !== 'grass' || t.deposit >= 0 || t.ext || ti === st.tile) continue;
        if (!s.g.neighbors(ti).some((n) => s.w.tiles[n].terrain === 'forest' || s.w.tiles[n].terrain === 'oldforest')) continue;
        t.terrain = 'forest'; t.wood = 160; grown++;
      }
    }
    if (grown) { s.clearPaths(); s.log('class', `${c.name} topraklarında orman ${grown} hex genişledi.`, { civ: c.id, tile: cap.tile }); }
  }
  // Haydut: casusluk
  const spy = s.e(c, 'spy');
  if (spy > 0 && contacts.length) {
    const o = s.rng.pick(contacts);
    const cand = o.research.done.filter((t) => !s.has(c, t) && TECH[t].tree === 'main' && TECH[t].era <= c.era && TECH[t].req.every((r) => s.has(c, r)));
    if (cand.length && s.rng.chance(spy)) {
      const t = s.rng.pick(cand);
      c.research.done.push(t);
      s.recomputeEff(c);
      s.metric('stolenTech');
      const caught = s.rng.chance(0.35);
      if (caught) s.addMod(o.id, c.id, 'spy', 'Yakalanan casuslar', -12, -30, 0.02, false);
      s.log('class', `${c.name} casusları ${o.name}'dan ${TECH[t].name} bilgisini çaldı${caught ? ' ama yakalandılar' : ''}.`, { civ: c.id, major: true });
    }
  }
  // Suikast
  const dt = s.e(c, 'deathTouch');
  if (dt > 0) {
    const enemies = contacts.filter((o) => s.atWar(c.id, o.id) || s.relValue(c.id, o.id) < -30);
    const victims = enemies.flatMap((o) => s.civHeroes(o));
    if (victims.length && s.rng.chance(dt)) {
      const h = s.rng.pick(victims);
      h.state = 'dead'; h.deathDay = s.day; h.hp = 0;
      s.metric('assassination');
      s.addMod(h.civ, c.id, 'assassin', 'Suikast şüphesi', -20, -40, 0.02, false);
      s.log('class', `${s.heroTitle(h)} karanlık bir sokakta suikaste kurban gitti.`, { civ: c.id, major: true, cause: `${c.name} gölgelerinin işi olduğu fısıldanıyor` });
    }
  }
  // Usta hırsız
  if (s.e(c, 'legendSteal') > 0 && contacts.length) {
    const rich = contacts.sort((a, b) => s.st(b, 'gold') - s.st(a, 'gold'))[0];
    const g = s.st(rich, 'enchanted') >= 1 ? 'enchanted' : s.st(rich, 'mithrilArmor') >= 1 ? 'mithrilArmor' : 'gold';
    const q = g === 'gold' ? Math.min(40, Math.floor(s.st(rich, 'gold') * 0.2)) : 1;
    if (q > 0) { s.add(rich, g, -q); s.add(c, g, q); s.log('class', `${c.name} hırsızları ${rich.name} hazinesinden ${q} ${g === 'gold' ? 'altın' : 'efsanevi eşya'} aşırdı.`, { civ: c.id }); }
  }
  // Gölge Kral
  if (s.e(c, 'puppet') > 0 && !c.yearly.puppet && contacts.length) {
    const o = contacts.sort((a, b) => s.civPop(a) - s.civPop(b))[0];
    c.yearly.puppet = o.id + 1;
    s.setMod(o.id, c.id, 'puppet', 'Gölge Kral\'ın sözü', 35, 0, false);
    s.log('class', `${o.name} artık gizlice ${c.name} çıkarlarına hizmet ediyor.`, { civ: c.id, major: true });
  }
  // İlahi Müdahale
  if (s.e(c, 'divine') > 0 && (s.inWar(c) || s.civSettlements(c).some((x) => x.starving > 0)) && s.rng.chance(s.e(c, 'divine'))) {
    s.add(c, 'grain', 80);
    for (const st of s.civSettlements(c)) st.soldiers += 2;
    s.log('class', `Tanrılar ${c.name} için müdahale etti: ambarlar doldu, savunucular çoğaldı.`, { civ: c.id, major: true });
  }
  // Ozan: şarkı festivali — altın, dostluk, göçmen
  const charm = s.e(c, 'charm');
  if (charm > 0) {
    const gold = Math.round(6 + contacts.length * 4 * charm);
    s.add(c, 'gold', gold);
    for (const o of contacts) s.addMod(o.id, c.id, 'festival', 'Ozan festivaline davet', 6 * charm, 18, 0.02, false);
    let came = '';
    if (contacts.length && s.pop(cap) < s.housing(cap) + 1 && s.rng.chance(0.5 + charm * 0.2)) {
      const o = s.rng.pick(contacts);
      const n = s.rng.int(1, 2);
      s.addPop(cap, o.race, n);
      came = ` ${n} ${RACES[o.race].name.toLocaleLowerCase('tr')} şarkılara kapılıp kaldı`;
    }
    s.log('class', `${ek(cap.name, 'da')} büyük ozan festivali: ${gold} altın toplandı${came ? ',' + came : ''}.`, { civ: c.id, tile: cap.tile, cause: 'İlham: komşular davetli' });
  }
  // Savaşçı: yıllık turnuva
  if (c.cls === 'fighter' && s.has(c, 'training')) {
    cap.soldiers += 1;
    const hs = s.civHeroes(c).filter((h) => h.state === 'home');
    for (const h of hs) gainXp(s, h, 150);
    s.log('class', `${ek(cap.name, 'da')} lejyon turnuvası düzenlendi${hs.length ? `; ${hs.map((h) => h.name).join(', ')} şan kazandı` : ''}.`, { civ: c.id, tile: cap.tile, cause: 'Aksiyon Dalgası: savaş sanatı sürekli bilenir' });
  }
  // Keşiş: meditasyon inzivası
  if (c.cls === 'monk' && c.research.current) {
    const t = TECH[c.research.current];
    c.research.progress += techCost(t) * 0.3;
    s.log('class', `${c.name} keşişleri inzivaya çekildi; ${t.name} araştırması hızlandı.`, { civ: c.id, tile: cap.tile, cause: 'Disiplin' });
  }
  // Paktçı: patronun hediyesi ve bedeli
  if (s.e(c, 'pact') > 0) {
    let gift = '';
    switch (c.subclass) {
      case 'fiend': cap.soldiers += 3; s.add(c, 'arms', 3); gift = 'kara alevden üç savaşçı ve silahlar geldi'; break;
      case 'archfey': {
        s.add(c, 'gold', 40);
        const o = contacts.length ? s.rng.pick(contacts) : undefined;
        const cand = o ? o.research.done.filter((t) => !s.has(c, t) && TECH[t].tree === 'main' && TECH[t].era <= c.era && TECH[t].req.every((r) => s.has(c, r))) : [];
        if (o && cand.length) { const t = s.rng.pick(cand); c.research.done.push(t); s.recomputeEff(c); gift = `40 altın ve ${ek(o.name, 'in')} rüyalarından çalınan ${TECH[t].name} bilgisi`; }
        else gift = 'peri altını (40)';
        break;
      }
      case 'oldone': if (c.research.current) c.research.progress += techCost(TECH[c.research.current]) * 0.45; s.add(c, 'mana', 4); gift = 'yıldızların ötesinden gelen fısıltılar araştırmayı hızlandırdı'; break;
      default: s.add(c, 'gold', 25); gift = '25 altın';
    }
    const n = s.pop(cap) > 40 && s.rng.chance(0.3) ? 2 : s.pop(cap) > 20 ? 1 : 0;
    if (n) s.removePop(cap, n);
    for (const o of contacts) if (o.align.good > 0.3) s.addMod(o.id, c.id, 'pact', 'Karanlık pakt söylentileri', -6, -24, 0.01, false);
    s.metric('pact');
    s.log('class', `${c.name} patronundan hediyesini aldı: ${gift}.`, { civ: c.id, tile: cap.tile, major: true, cause: n ? `Bedeli: ${n} kişi bir gece gölgelere karışıp kayboldu` : 'Bu yıl bedel ertelendi' });
  }
  // Kan Büyücüsü: yabani büyü dalgası
  if (s.e(c, 'wild') > 0) wildSurge(s, c, cap);
  void YEAR;
}

function wildSurge(s: Sim, c: Civ, cap: NonNullable<ReturnType<Sim['capital']>>) {
  const lucky = s.e(c, 'luck') > 0;
  type Surge = { good: boolean; run: () => string };
  const surges: Surge[] = [
    { good: true, run: () => { const g = s.rng.int(20, 45); s.add(c, 'gold', g); return `gökten ${g} altın yağdı`; } },
    { good: true, run: () => { if (c.research.current) c.research.progress += techCost(TECH[c.research.current]) * 0.5; return 'bilginlerin zihninde bir kıvılcım çaktı, araştırma sıçradı'; } },
    { good: true, run: () => { if (s.pop(cap) < 20) { s.add(c, 'grain', 30); return 'ambarlar sıcak ekmekle doldu'; } cap.soldiers += 3; return 'alevlerin içinden üç savaşçı yürüyerek çıktı'; } },
    { good: true, run: () => { s.addPop(cap, s.rng.pick(['tiefling', 'dragonborn', 'human'] as RaceId[]), 2); return 'kızıl bir sisten iki yabancı belirdi ve kaldı'; } },
    { good: false, run: () => { const q = Math.floor(s.st(c, 'grain') * 0.25); s.add(c, 'grain', -q); return `kontrolden çıkan alev ambarda ${q} tahılı kül etti`; } },
    { good: false, run: () => { if (s.pop(cap) > 8) s.removePop(cap, 1); return 'bir çırak kendi büyüsüne kurban gitti'; } },
    { good: false, run: () => { for (const h of s.civHeroes(c)) h.hp = Math.max(1, Math.floor(h.hp / 2)); return 'kahramanlar lanetli bir ateşle yaralandı'; } },
    { good: true, run: () => {
      const cand = s.g.within(cap.tile, 3).filter((t) => s.w.tiles[t].terrain === 'grass' && s.w.tiles[t].owner >= 0 && !s.w.tiles[t].ext && t !== cap.tile);
      if (!cand.length) { s.add(c, 'mana', 5); return 'havada mana kıvılcımları uçuştu'; }
      const t = s.rng.pick(cand); s.w.tiles[t].terrain = 'forest'; s.w.tiles[t].wood = 160; s.clearPaths(); return 'bir gecede kızıl yapraklı bir koru bitti';
    } },
  ];
  let pick = s.rng.pick(surges);
  if (!pick.good && lucky) pick = s.rng.pick(surges.filter((x) => x.good));
  const what = pick.run();
  s.metric('wildSurge');
  s.log('class', `Yabani büyü dalgası ${c.name} topraklarını sardı: ${what}.`, { civ: c.id, tile: cap.tile, major: true, cause: pick.good ? 'Kan Soyu: şans bu kez yüzlerine güldü' : 'Kan Soyu: büyünün bedeli' });
}
