import { Rng, mod } from './rng';
import { ek } from './tr';
import { HERO_CLASSES, HERO_CLASS_TR } from '../data/heroes';
import type { Battle, BattleLine, Hero, Replay, ReplayEv, ReplayGroup, ReplayPart, ReplayUnit } from './types';

export interface UnitStats { name: string; hp: number; ac: number; atk: number; dmg: [number, number, number]; attacks?: number }

export interface Combatant {
  name: string;
  side: 'A' | 'B';
  hp: number;
  maxHp: number;
  ac: number;
  atk: number;
  dmg: [number, number, number];
  attacks: number;
  hero?: Hero;
  boss?: boolean;
  kind: string;           // soldier | militia | monster | hero | ballista | unique
  evil?: boolean;         // kötü ya da canavar (Paladin çarpışı için)
  smite?: number;         // kötülere karşı kritik çarpanı
  fled?: boolean;
  uses: Record<string, number>;
  kills: number;
  rage?: boolean;
  temp?: number;
  grp?: number;           // ortak saldırıda hangi gruba ait (Replay.groups dizini)
}

export function unit(u: UnitStats, side: 'A' | 'B', kind: string, bonusHp = 0, bonusAtk = 0): Combatant {
  const hp = Math.max(1, u.hp + bonusHp);
  return { name: u.name, side, hp, maxHp: hp, ac: u.ac, atk: u.atk + bonusAtk, dmg: [...u.dmg] as [number, number, number], attacks: u.attacks ?? 1, kind, uses: {}, kills: 0, boss: kind === 'boss' };
}

export function profBonus(level: number) { return level >= 5 ? 3 : 2; }

export function heroCombatant(h: Hero, side: 'A' | 'B'): Combatant {
  const def = HERO_CLASSES[h.cls];
  const pm = mod(h.stats[def.primary]);
  const atk = profBonus(h.level) + pm + (h.bonus?.atk ?? 0);
  let dmg: [number, number, number] = [def.dmg[0], def.dmg[1], pm];
  if (h.cls === 'wizard') dmg = [h.level >= 5 ? 2 : 1, 10, 0];
  return {
    name: h.name, side, hp: h.hp, maxHp: h.maxHp, ac: h.ac, atk, dmg,
    attacks: (h.cls === 'fighter' || h.cls === 'paladin' || h.cls === 'barbarian' || h.cls === 'ranger') && h.level >= 5 ? 2 : 1,
    hero: h, kind: 'hero', uses: { secondWind: 1, burning: 1, fireball: h.level >= 5 ? 1 : 0, cure: 2, smite: 2, wild: 1, rage: 1 }, kills: 0,
  };
}

export function heroAc(cls: string, dex: number, level: number) {
  switch (cls) {
    case 'fighter': return 17 + (level >= 3 ? 1 : 0);
    case 'paladin': return 18;
    case 'cleric': return 16;
    case 'rogue': return 11 + dex;
    case 'ranger': return 13 + Math.min(2, dex);
    case 'druid': return 13 + Math.min(2, dex);
    case 'barbarian': return 12 + dex;
    default: return 13 + dex; // wizard: mage armor
  }
}

/** ortalama zırh sınıfı (güç tahmininde karşı tarafın isabet şansı için) */
export function avgAc(cs: Combatant[]): number {
  if (!cs.length) return 15;
  return cs.reduce((a, c) => a + c.ac, 0) / cs.length;
}

/** Lanchester tarzı güç tahmini. vsAc: karşı tarafın ortalama AC'si (bilinmiyorsa 15) */
export function powerOf(cs: Combatant[], vsAc = 15): number {
  let dpr = 0, ehp = 0;
  for (const c of cs) {
    const avg = c.dmg[0] * (c.dmg[1] + 1) / 2 + c.dmg[2];
    const hit = Math.min(0.95, Math.max(0.1, (21 - (vsAc - c.atk)) / 20));
    let extra = 1;
    if (c.hero?.cls === 'wizard') extra = 1.8;
    if (c.hero?.cls === 'cleric' || c.hero?.cls === 'druid') extra = 1.3;
    if (c.hero?.cls === 'rogue' || c.hero?.cls === 'paladin' || c.hero?.cls === 'barbarian') extra = 1.4;
    dpr += avg * hit * c.attacks * extra;
    ehp += c.hp * (1 + (c.ac - 12) * 0.08) * (c.hero?.cls === 'barbarian' ? 1.5 : 1);
  }
  return Math.sqrt(dpr * ehp);
}
/** iki tarafın birbirine karşı gücü: [A, B] */
export function powerVs(A: Combatant[], B: Combatant[]): [number, number] { return [powerOf(A, avgAc(B)), powerOf(B, avgAc(A))]; }

/** savaş öncesi tahmini zafer şansı (A için). Simülasyondan ölçülmüş lojistik eğri: güç oranının logaritması */
export function winChance(powA: number, powB: number): number {
  const x = Math.log(Math.max(0.01, powA) / Math.max(0.01, powB));
  return 1 / (1 + Math.exp(-(5.75 * x + 0.9)));
}

/** kahraman yaşarken moral eşiği bu kadar yükselir (artık bozgunu tamamen engellemez) */
export const HERO_MORALE = 0.15;
/** ayrıntılı tekrarı tutulan son savaş sayısı */
export const REPLAY_KEEP = 40;

export interface BattleOpts {
  day: number; tile: number; title: string; sideA: string; sideB: string; id: number;
  maxRounds?: number; moraleA?: number; moraleB?: number;
  noRoutA?: boolean; noRoutB?: boolean;
  firstStrikeA?: number; firstStrikeB?: number;
  meteorA?: boolean; meteorB?: boolean;
  /** süre dolarsa kazanan (kuşatmada ve kamp baskınında savunan). Verilmezse kalan can toplamı. */
  timeoutWinner?: 'A' | 'B';
  groups?: ReplayGroup[];
  civA?: number; civB?: number;
}

function replayUnit(c: Combatant): ReplayUnit {
  const u: ReplayUnit = { n: c.name, s: c.side, k: c.kind, hp: c.hp, max: c.maxHp, ac: c.ac, atk: c.atk, dmg: [...c.dmg] as [number, number, number], att: c.attacks, g: c.grp ?? (c.side === 'A' ? 0 : 1) };
  if (c.hero) { u.cls = c.hero.cls; u.lvl = c.hero.level; u.race = c.hero.race; }
  if (c.boss) u.boss = true;
  return u;
}

export function resolveBattle(rng: Rng, A: Combatant[], B: Combatant[], o: BattleOpts): Battle {
  const all = [...A, ...B];
  const idx = new Map<Combatant, number>(all.map((c, i) => [c, i]));
  const lines: BattleLine[] = [];
  const rolls: Battle['rolls'] = [];
  const ev: ReplayEv[] = [];
  const units = all.map(replayUnit);
  const [powA, powB] = powerVs(A, B);
  const initA = A.length, initB = B.length;
  const alive = (s: 'A' | 'B') => all.filter((c) => c.side === s && c.hp > 0 && !c.fled);
  const heroLogged = new Set<string>();
  const maxRounds = o.maxRounds ?? 15;
  let routed: 'A' | 'B' | null = null;
  let rounds = 0;

  for (const [side, on] of [['A', o.meteorA], ['B', o.meteorB]] as ['A' | 'B', boolean | undefined][]) {
    if (!on) continue;
    const foes = alive(side === 'A' ? 'B' : 'A');
    let killed = 0;
    const ts: number[] = [], vs: number[] = [], dds: number[][] = [];
    for (const f of foes) { const r = rng.roll(4, 6); const d = r.reduce((x, y) => x + y, 0); f.hp -= d; ts.push(idx.get(f)!); vs.push(d); dds.push(r); if (f.hp <= 0) killed++; }
    ev.push({ r: 0, sp: 'meteor', a: -1, ts, vs, dds, ds: 6, d: side === 'A' ? 0 : 1 });
    lines.push({ t: `Gökten meteorlar yağdı! ${killed} düşman ilk anda düştü.`, crit: true });
  }

  for (let round = 1; round <= maxRounds && !routed; round++) {
    rounds = round;
    ev.push({ r: round, sp: 'round', aA: alive('A').length, aB: alive('B').length });
    const order = rng.shuffle(all.filter((c) => c.hp > 0 && !c.fled));
    for (const c of order) {
      if (c.hp <= 0 || c.fled) continue;
      const ci = idx.get(c)!;
      const foeSide = c.side === 'A' ? 'B' : 'A';
      const foes = alive(foeSide);
      if (!foes.length) break;
      const friends = alive(c.side);

      if (c.hero) {
        const h = c.hero;
        if (c.hp < c.maxHp * 0.25 && foes.length > friends.length * 1.5 && rng.chance(0.5)) {
          c.fled = true;
          ev.push({ r: round, sp: 'flee', a: ci });
          lines.push({ t: `${h.name} ağır yaralı halde savaş alanından kaçtı.` });
          continue;
        }
        if (h.cls === 'barbarian' && c.uses.rage) { c.uses.rage = 0; c.rage = true; ev.push({ r: round, sp: 'rage', a: ci }); lines.push({ t: `${h.name} öfkeye kapıldı!` }); }
        if (h.cls === 'druid' && c.uses.wild && c.hp < c.maxHp * 0.5) {
          c.uses.wild = 0; const t = 10 + h.level * 3; c.hp += t;
          ev.push({ r: round, sp: 'wild', a: ci, t: ci, v: t, hp: c.hp });
          lines.push({ t: `${h.name} bir ayıya dönüştü (Wild Shape, +${t} can).` });
        }
        if (h.cls === 'fighter' && c.uses.secondWind && c.hp < c.maxHp * 0.5) {
          const dd = rng.roll(1, 10);
          const heal = dd[0] + h.level;
          c.hp = Math.min(c.maxHp, c.hp + heal); c.uses.secondWind--;
          ev.push({ r: round, sp: 'second', a: ci, t: ci, dd, ds: 10, b: h.level, v: heal, hp: c.hp });
          lines.push({ t: `${h.name} derin bir nefes aldı (Second Wind) ve ${heal} can topladı.` });
        }
        if ((h.cls === 'cleric' || h.cls === 'druid' || h.cls === 'paladin') && c.uses.cure > 0) {
          const hurt = friends.filter((f) => f.hero && f.hp < f.maxHp * 0.45).sort((a, b) => a.hp - b.hp)[0];
          if (hurt) {
            const dd = rng.roll(1, 8);
            const bonus = mod(h.stats.wis) + (h.cls === 'paladin' ? h.level * 2 : 0);
            const heal = dd[0] + bonus;
            hurt.hp = Math.min(hurt.maxHp, hurt.hp + heal); c.uses.cure--;
            ev.push({ r: round, sp: 'heal', a: ci, t: idx.get(hurt)!, dd, ds: 8, b: bonus, v: heal, hp: hurt.hp });
            lines.push({ t: `${h.name}, ${hurt === c ? 'kendi' : ek(hurt.name, 'in')} yaralarını iyileştirdi (+${heal}).` });
            continue;
          }
        }
        if (h.cls === 'wizard' && foes.length >= 3 && (c.uses.fireball > 0 || c.uses.burning > 0)) {
          const fire = c.uses.fireball > 0;
          const targets = rng.shuffle(foes.slice()).slice(0, fire ? 6 : 3);
          let killed = 0;
          const ts: number[] = [], vs: number[] = [], sv: boolean[] = [], dds: number[][] = [];
          for (const t of targets) {
            const r = rng.roll(fire ? 8 : 3, 6);
            dds.push(r);
            let d = r.reduce((x, y) => x + y, 0);
            const save = rng.chance(0.35);
            if (save) d = Math.floor(d / 2);
            t.hp -= d;
            ts.push(idx.get(t)!); vs.push(d); sv.push(save);
            if (t.hp <= 0) { killed++; c.kills++; }
          }
          if (fire) c.uses.fireball--; else c.uses.burning--;
          ev.push({ r: round, sp: fire ? 'fireball' : 'burning', a: ci, ts, vs, sv, dds, ds: 6 });
          lines.push({ t: `${h.name} ${fire ? 'bir ATEŞ TOPU patlattı' : 'alevler püskürttü (Burning Hands)'}: ${targets.length} hedef, ${killed} ölü!`, crit: fire });
          continue;
        }
      }

      for (let a = 0; a < c.attacks; a++) {
        const fs = alive(foeSide);
        if (!fs.length) break;
        let target: Combatant;
        if (c.hero) { const boss = fs.find((f) => f.boss); target = boss && rng.chance(0.6) ? boss : rng.pick(fs); }
        else target = rng.pick(fs);
        const ti = idx.get(target)!;
        const d20 = rng.d20();
        const important = !!c.hero || !!target.hero || !!c.boss;
        if (rolls.length < 40 || (important && rolls.length < 70) || ((d20 === 20 || d20 === 1) && rolls.length < 80)) rolls.push({ d20, side: c.side, who: c.name });
        if (d20 === 1) {
          ev.push({ r: round, sp: 'attack', a: ci, t: ti, d: 1, m: c.atk, ac: target.ac, h: 0 });
          if (c.hero && rng.chance(0.5)) lines.push({ t: `${c.name} nat 1! Silahı elinden kaydı.`, fumble: true });
          continue;
        }
        const critRange = c.hero?.cls === 'fighter' && c.hero.level >= 3 ? 19 : 20;
        const crit = d20 >= critRange;
        if (!crit && d20 + c.atk < target.ac) { ev.push({ r: round, sp: 'attack', a: ci, t: ti, d: d20, m: c.atk, ac: target.ac, h: 0 }); continue; }
        let n = c.dmg[0];
        if (crit) n *= (target.evil && c.smite ? Math.max(2, Math.round(c.smite)) : 2);
        const dd = rng.roll(n, c.dmg[1]);
        let dmg = dd.reduce((x, y) => x + y, 0) + c.dmg[2];
        const x: ReplayPart[] = [];
        if (c.rage) { dmg += 2; x.push({ l: 'Öfke', v: 2 }); }
        if (c.hero?.cls === 'rogue' && friends.length > 1) { const r = rng.roll(Math.ceil(c.hero.level / 2) * (crit ? 2 : 1), 6); const v = r.reduce((p, q) => p + q, 0); dmg += v; x.push({ l: 'Sinsi saldırı', v, dd: r, ds: 6 }); }
        if (c.hero?.cls === 'ranger' && c.hero.level >= 2) { const r = rng.roll(1, 6); dmg += r[0]; x.push({ l: 'Av işareti', v: r[0], dd: r, ds: 6 }); }
        if (c.hero?.cls === 'paladin' && c.uses.smite > 0 && (target.evil || target.boss || crit)) {
          c.uses.smite--; const r = rng.roll(2, 8); const sm = r[0] + r[1]; dmg += sm;
          x.push({ l: 'İlahi çarpış', v: sm, dd: r, ds: 8 });
          lines.push({ t: `${c.name} İlahi Çarpış indirdi (+${sm})!`, crit: true });
        }
        let mul: number | undefined;
        if (round === 1) { const fs1 = (c.side === 'A' ? o.firstStrikeA : o.firstStrikeB) ?? 1; if (fs1 !== 1) mul = fs1; dmg = Math.round(dmg * fs1); }
        const half = !!target.rage;
        if (half) dmg = Math.ceil(dmg / 2);
        dmg = Math.max(1, dmg);
        target.hp -= dmg;
        const e: ReplayEv = { r: round, sp: 'attack', a: ci, t: ti, d: d20, m: c.atk, ac: target.ac, h: crit ? 2 : 1, dd, ds: c.dmg[1], b: c.dmg[2], v: dmg, hp: target.hp };
        if (x.length) e.x = x;
        if (mul) e.mul = mul;
        if (half) e.half = true;
        ev.push(e);
        if (crit && (c.hero || target.hero || target.boss)) lines.push({ t: `${c.name} nat 20! ${target.name} ${dmg} hasar yedi.`, crit: true });
        if (target.hp <= 0) {
          c.kills++;
          if (target.hero) lines.push({ t: `${target.name} (${HERO_CLASS_TR[target.hero.cls]}) ${c.name} tarafından yere serildi!`, crit: true });
          else if (target.boss) lines.push({ t: `${c.name}, ${ek(target.name, 'i')} devirdi!`, crit: true });
          else if (c.hero && !heroLogged.has(c.name)) { heroLogged.add(c.name); lines.push({ t: `${c.name} ilk ${target.name.toLocaleLowerCase('tr')} kurbanını aldı.` }); }
        }
      }
    }
    const aA = alive('A').length, aB = alive('B').length;
    if (!aA || !aB) break;
    const lossA = 1 - aA / initA, lossB = 1 - aB / initB;
    const heroA = alive('A').some((c) => c.hero), heroB = alive('B').some((c) => c.hero);
    // kahraman bozgunu engellemez, eşiği yükseltir
    if (!o.noRoutA && lossA >= Math.min(0.95, (o.moraleA ?? 0.6) + (heroA ? HERO_MORALE : 0))) routed = 'A';
    else if (!o.noRoutB && lossB >= Math.min(0.95, (o.moraleB ?? 0.6) + (heroB ? HERO_MORALE : 0))) routed = 'B';
    if (routed) {
      ev.push({ r: round, sp: 'rout', d: routed === 'A' ? 0 : 1 });
      lines.push({ t: `${routed === 'A' ? o.sideA : o.sideB} bozguna uğrayıp kaçtı.` });
    }
  }

  const aA = alive('A'), aB = alive('B');
  let winner: 'A' | 'B';
  let end: Replay['end'];
  if (routed) { winner = routed === 'A' ? 'B' : 'A'; end = 'rout'; }
  else if (!aA.length) { winner = 'B'; end = 'wipe'; }
  else if (!aB.length) { winner = 'A'; end = 'wipe'; }
  else {
    end = 'timeout';
    winner = o.timeoutWinner ?? (aA.reduce((s, c) => s + c.hp, 0) >= aB.reduce((s, c) => s + c.hp, 0) ? 'A' : 'B');
    lines.push({ t: o.timeoutWinner ? `Gün battı; ${winner === 'A' ? o.sideA : o.sideB} mevzisini korudu.` : `Gün battı; savaş alanı ${winner === 'A' ? o.sideA : o.sideB} tarafında kaldı.` });
  }
  const groups = o.groups ?? [{ name: o.sideA, side: 'A', civ: o.civA }, { name: o.sideB, side: 'B', civ: o.civB }];
  const replay: Replay = {
    units, ev, groups, moraleA: o.moraleA ?? 0.6, moraleB: o.moraleB ?? 0.6, heroMorale: HERO_MORALE, noRoutA: o.noRoutA, noRoutB: o.noRoutB,
    powA: Math.round(powA * 10) / 10, powB: Math.round(powB * 10) / 10, rounds, maxRounds, end, routed: routed ?? undefined, timeoutWinner: o.timeoutWinner,
  };
  const b: Battle = { id: o.id, day: o.day, tile: o.tile, title: o.title, sideA: o.sideA, sideB: o.sideB, winner, lossesA: A.filter((c) => c.hp <= 0).length, lossesB: B.filter((c) => c.hp <= 0).length, lines, rolls, replay };
  if (o.civA !== undefined) b.civA = o.civA;
  if (o.civB !== undefined) b.civB = o.civB;
  if (o.groups && o.groups.filter((g) => g.side === 'A').length > 1) b.joint = true;
  return b;
}
