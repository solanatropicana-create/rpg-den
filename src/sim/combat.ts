import { Rng, mod } from './rng';
import { ek } from './tr';
import { HERO_CLASSES, HERO_CLASS_TR } from '../data/heroes';
import type { Battle, BattleLine, Hero } from './types';

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

/** Lanchester tarzı güç tahmini */
export function powerOf(cs: Combatant[]): number {
  let dpr = 0, ehp = 0;
  for (const c of cs) {
    const avg = c.dmg[0] * (c.dmg[1] + 1) / 2 + c.dmg[2];
    const hit = Math.min(0.95, Math.max(0.1, (21 - (15 - c.atk)) / 20));
    let extra = 1;
    if (c.hero?.cls === 'wizard') extra = 1.8;
    if (c.hero?.cls === 'cleric' || c.hero?.cls === 'druid') extra = 1.3;
    if (c.hero?.cls === 'rogue' || c.hero?.cls === 'paladin' || c.hero?.cls === 'barbarian') extra = 1.4;
    dpr += avg * hit * c.attacks * extra;
    ehp += c.hp * (1 + (c.ac - 12) * 0.08) * (c.hero?.cls === 'barbarian' ? 1.5 : 1);
  }
  return Math.sqrt(dpr * ehp);
}

export interface BattleOpts {
  day: number; tile: number; title: string; sideA: string; sideB: string; id: number;
  maxRounds?: number; moraleA?: number; moraleB?: number;
  noRoutA?: boolean; noRoutB?: boolean;
  firstStrikeA?: number; firstStrikeB?: number;
  meteorA?: boolean; meteorB?: boolean;
}

export function resolveBattle(rng: Rng, A: Combatant[], B: Combatant[], o: BattleOpts): Battle {
  const all = [...A, ...B];
  const lines: BattleLine[] = [];
  const rolls: Battle['rolls'] = [];
  const initA = A.length, initB = B.length;
  const alive = (s: 'A' | 'B') => all.filter((c) => c.side === s && c.hp > 0 && !c.fled);
  const heroLogged = new Set<string>();
  const maxRounds = o.maxRounds ?? 15;
  let routed: 'A' | 'B' | null = null;

  for (const [side, on] of [['A', o.meteorA], ['B', o.meteorB]] as ['A' | 'B', boolean | undefined][]) {
    if (!on) continue;
    const foes = alive(side === 'A' ? 'B' : 'A');
    let killed = 0;
    for (const f of foes) { f.hp -= rng.dice(4, 6); if (f.hp <= 0) killed++; }
    lines.push({ t: `Gökten meteorlar yağdı! ${killed} düşman ilk anda düştü.`, crit: true });
  }

  for (let round = 1; round <= maxRounds && !routed; round++) {
    const order = rng.shuffle(all.filter((c) => c.hp > 0 && !c.fled));
    for (const c of order) {
      if (c.hp <= 0 || c.fled) continue;
      const foeSide = c.side === 'A' ? 'B' : 'A';
      const foes = alive(foeSide);
      if (!foes.length) break;
      const friends = alive(c.side);

      if (c.hero) {
        const h = c.hero;
        if (c.hp < c.maxHp * 0.25 && foes.length > friends.length * 1.5 && rng.chance(0.5)) {
          c.fled = true;
          lines.push({ t: `${h.name} ağır yaralı halde savaş alanından kaçtı.` });
          continue;
        }
        if (h.cls === 'barbarian' && c.uses.rage) { c.uses.rage = 0; c.rage = true; lines.push({ t: `${h.name} öfkeye kapıldı!` }); }
        if (h.cls === 'druid' && c.uses.wild && c.hp < c.maxHp * 0.5) {
          c.uses.wild = 0; const t = 10 + h.level * 3; c.hp += t;
          lines.push({ t: `${h.name} bir ayıya dönüştü (Wild Shape, +${t} can).` });
        }
        if (h.cls === 'fighter' && c.uses.secondWind && c.hp < c.maxHp * 0.5) {
          const heal = rng.dice(1, 10) + h.level;
          c.hp = Math.min(c.maxHp, c.hp + heal); c.uses.secondWind--;
          lines.push({ t: `${h.name} derin bir nefes aldı (Second Wind) ve ${heal} can topladı.` });
        }
        if ((h.cls === 'cleric' || h.cls === 'druid' || h.cls === 'paladin') && c.uses.cure > 0) {
          const hurt = friends.filter((f) => f.hero && f.hp < f.maxHp * 0.45).sort((a, b) => a.hp - b.hp)[0];
          if (hurt) {
            const heal = rng.dice(1, 8) + mod(h.stats.wis) + (h.cls === 'paladin' ? h.level * 2 : 0);
            hurt.hp = Math.min(hurt.maxHp, hurt.hp + heal); c.uses.cure--;
            lines.push({ t: `${h.name}, ${hurt === c ? 'kendi' : ek(hurt.name, 'in')} yaralarını iyileştirdi (+${heal}).` });
            continue;
          }
        }
        if (h.cls === 'wizard' && foes.length >= 3 && (c.uses.fireball > 0 || c.uses.burning > 0)) {
          const fire = c.uses.fireball > 0;
          const targets = rng.shuffle(foes.slice()).slice(0, fire ? 6 : 3);
          let killed = 0;
          for (const t of targets) {
            let d = fire ? rng.dice(8, 6) : rng.dice(3, 6);
            if (rng.chance(0.35)) d = Math.floor(d / 2);
            t.hp -= d;
            if (t.hp <= 0) { killed++; c.kills++; }
          }
          if (fire) c.uses.fireball--; else c.uses.burning--;
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
        const d20 = rng.d20();
        const important = !!c.hero || !!target.hero || !!c.boss;
        if (rolls.length < 40 || (important && rolls.length < 70) || ((d20 === 20 || d20 === 1) && rolls.length < 80)) rolls.push({ d20, side: c.side, who: c.name });
        if (d20 === 1) {
          if (c.hero && rng.chance(0.5)) lines.push({ t: `${c.name} nat 1! Silahı elinden kaydı.`, fumble: true });
          continue;
        }
        const critRange = c.hero?.cls === 'fighter' && c.hero.level >= 3 ? 19 : 20;
        const crit = d20 >= critRange;
        if (!crit && d20 + c.atk < target.ac) continue;
        let n = c.dmg[0];
        if (crit) n *= (target.evil && c.smite ? Math.max(2, Math.round(c.smite)) : 2);
        let dmg = rng.dice(n, c.dmg[1]) + c.dmg[2];
        if (c.rage) dmg += 2;
        if (c.hero?.cls === 'rogue' && friends.length > 1) dmg += rng.dice(Math.ceil(c.hero.level / 2) * (crit ? 2 : 1), 6);
        if (c.hero?.cls === 'ranger' && c.hero.level >= 2) dmg += rng.dice(1, 6);
        if (c.hero?.cls === 'paladin' && c.uses.smite > 0 && (target.evil || target.boss || crit)) {
          c.uses.smite--; const sm = rng.dice(2, 8); dmg += sm;
          lines.push({ t: `${c.name} İlahi Çarpış indirdi (+${sm})!`, crit: true });
        }
        if (round === 1) dmg = Math.round(dmg * ((c.side === 'A' ? o.firstStrikeA : o.firstStrikeB) ?? 1));
        if (target.rage) dmg = Math.ceil(dmg / 2);
        dmg = Math.max(1, dmg);
        target.hp -= dmg;
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
    if (!o.noRoutA && lossA >= (o.moraleA ?? 0.6) && !heroA) routed = 'A';
    else if (!o.noRoutB && lossB >= (o.moraleB ?? 0.6) && !heroB) routed = 'B';
    if (routed) lines.push({ t: `${routed === 'A' ? o.sideA : o.sideB} bozguna uğrayıp kaçtı.` });
  }

  const aA = alive('A'), aB = alive('B');
  let winner: 'A' | 'B';
  if (routed) winner = routed === 'A' ? 'B' : 'A';
  else if (!aA.length) winner = 'B';
  else if (!aB.length) winner = 'A';
  else winner = aA.reduce((s, c) => s + c.hp, 0) >= aB.reduce((s, c) => s + c.hp, 0) ? 'A' : 'B';
  return { id: o.id, day: o.day, tile: o.tile, title: o.title, sideA: o.sideA, sideB: o.sideB, winner, lossesA: A.filter((c) => c.hp <= 0).length, lossesB: B.filter((c) => c.hp <= 0).length, lines, rolls };
}
