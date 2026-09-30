// Dünya olayları: salgın, kasaba yangını, kıtlık göçü.
import type { Sim } from './sim';
import { YEAR } from './sim';
import { ek } from './tr';
import type { Civ, Settlement } from './types';

const medicine = (s: Sim, c: Civ, st: Settlement) =>
  (s.has(c, 'medicine') ? 0.45 : 0) + ((st.workshops.apothecary ?? 0) > 0 ? 0.3 : 0) + (st.civics.temple ? 0.1 : 0) + Math.min(0.3, s.e(c, 'healBack') * 0.3);

/** ayda bir çağrılır */
export function disastersTick(s: Sim) {
  const w = s.w;
  const alive = w.settlements.filter((x) => x.alive);
  // --- salgın: başlama, yayılma, ölüm, bitiş
  for (const st of alive) {
    const c = w.civs[st.civ];
    const P = s.pop(st);
    const pl = st.plague;
    if (pl) {
      const prot = Math.min(0.85, medicine(s, c, st));
      const deaths = Math.min(P - 2, Math.max(1, Math.round(P * 0.05 * pl.severity * (1 - prot))));
      if (deaths > 0) { s.removePop(st, deaths); pl.dead += deaths; st.graves = (st.graves ?? 0) + deaths; s.metric('plagueDead', deaths); }
      if (s.day >= pl.until || s.pop(st) <= 3) {
        s.log('world', `${ek(st.name, 'da')} salgın sona erdi; ${pl.dead} kişi hayatını kaybetti.`, { civ: c.id, tile: st.tile, major: pl.dead >= 5, cause: prot > 0.3 ? 'Şifacılar hastalığı dizginledi' : 'Hastalık kendi yolunu tamamladı' });
        st.plague = undefined; st.plagueImmune = s.day + 3 * YEAR;
      }
      continue;
    }
    const hl = s.homeless(st);
    if ((P < 28 && hl < 8) || P < 14 || (st.plagueImmune ?? 0) > s.day) continue;
    const crowded = P > [24, 55, 120, 230][st.tier] * 0.85 ? 1.6 : 1;
    const shanty = 1 + Math.min(2.5, (hl / P) * 4);
    const chance = 0.0035 * (P / 40) * crowded * shanty * (1 - Math.min(0.85, medicine(s, c, st))) * (st.starving > 0 ? 2 : 1);
    if (s.rng.chance(chance)) startPlague(s, st, hl >= 6 ? `Sur dışındaki barakalarda ${hl} kişi üst üste yaşıyordu` : 'Kalabalık sokaklar ve kirli kuyular');
  }
  // yayılma: ticaret yolları ve yakın komşular
  for (const st of alive.filter((x) => x.plague && x.plague.since < s.day - 20)) {
    const links = new Set<number>();
    for (const r of w.routes) if (r.alive) { if (r.a === st.id) links.add(r.b); if (r.b === st.id) links.add(r.a); }
    for (const o of alive) if (o.id !== st.id && s.g.dist(o.tile, st.tile) <= 6) links.add(o.id);
    for (const id of links) {
      const o = s.settlement(id);
      if (!o || !o.alive || o.plague || (o.plagueImmune ?? 0) > s.day || s.pop(o) < 8) continue;
      const prot = Math.min(0.85, medicine(s, w.civs[o.civ], o));
      if (s.rng.chance(0.12 * (1 - prot))) startPlague(s, o, `${ek(st.name, 'dan')} gelen yolcular hastalığı taşıdı`);
    }
  }
  // --- kasaba yangını: ahşap evler, kuru yaz
  for (const st of alive) {
    const wood = (st.civics.hut ?? 0) + (st.civics.house ?? 0), stone = st.civics.stonehouse ?? 0;
    if (wood < 5) continue;
    const summer = s.season === 1 ? 2 : 1;
    const chance = 0.0028 * summer * (wood / (wood + stone * 2)) * (st.civics.stonewall ? 0.8 : 1);
    if (!s.rng.chance(chance)) continue;
    const burnt = Math.min(Math.max(2, Math.round(wood / 4)), 2 + s.rng.int(0, 4));
    st.burnedHouses = (st.burnedHouses ?? 0) + burnt; st.burnedAt = s.day;
    const dead = s.rng.chance(0.5) ? s.rng.int(1, 2) : 0;
    if (dead) s.removePop(st, dead);
    s.metric('townFire');
    s.log('world', `${ek(st.name, 'da')} yangın çıktı: ${burnt} ev kül oldu${dead ? `, ${dead} kişi öldü` : ''}.`, { civ: st.civ, tile: st.tile, major: true, cause: s.season === 1 ? 'Kurak yaz, ahşap çatılar' : 'Devrilen bir kandil, sık ahşap evler' });
  }
  // --- barakalar: konutu yetmeyen halk sur dışında yaşar; yangın riski, iç göç
  for (const st of alive) {
    const hl = s.homeless(st), c = w.civs[st.civ];
    if (hl >= 5 && !st.shantyLog) {
      st.shantyLog = true;
      s.log('economy', `${ek(st.name, 'da')} konut yetmiyor: ${hl} kişi sur dışında barakalarda yaşıyor.`, { civ: c.id, tile: st.tile, cause: (st.burnedHouses ?? 0) > 0 ? 'Yanan evler henüz onarılmadı' : 'Gelen göçmenler evlerden hızlı çoğaldı' });
    } else if (hl === 0 && st.shantyLog) {
      st.shantyLog = false;
      s.log('economy', `${ek(st.name, 'da')} barakalar boşaldı; herkesin bir evi var.`, { civ: c.id, tile: st.tile });
    }
    if (hl < 4) continue;
    // baraka yangını
    if (s.rng.chance(0.004 * Math.min(3, hl / 8) * (s.season === 1 ? 2 : 1))) {
      const dead = s.rng.chance(0.4) ? 1 : 0;
      if (dead) s.removePop(st, dead);
      s.metric('shantyFire');
      s.log('world', `${ek(st.name, 'in')} barakalarında yangın çıktı${dead ? '; bir kişi öldü' : ''}.`, { civ: c.id, tile: st.tile, cause: 'Sık çadırlar, açık ocaklar' });
    }
    // iç göç: aynı medeniyette boş evi olan yerleşime yürürler
    if (w.agents.some((a) => a.kind === 'settlers' && a.purpose === 'homeless' && a.from === st.id)) continue;
    const to = s.civSettlements(c).filter((o) => o.id !== st.id && s.housing(o) - s.pop(o) >= 3 && o.starving === 0)
      .sort((a, b) => s.g.dist(a.tile, st.tile) - s.g.dist(b.tile, st.tile))[0];
    if (!to || !s.rng.chance(0.6)) continue;
    const path = s.path(st.tile, to.tile);
    if (!path) continue;
    const n = Math.min(hl, s.housing(to) - s.pop(to), 8);
    const pop = s.removePop(st, n);
    w.agents.push({ id: s.id(), kind: 'settlers', civ: c.id, path, step: 0, progress: 0, speed: 0.45, pop, from: st.id, to: to.id, purpose: 'homeless' });
    s.metric('homelessMove');
    s.log('migration', `${ek(st.name, 'in')} barakalarından ${n} kişi, boş evleri olan ${ek(to.name, 'a')} yola çıktı.`, { civ: c.id, tile: st.tile });
  }
  // --- kıtlık göçü: aç kalan halk, yiyeceği olan komşuya yürür
  for (const c of w.civs.filter((x) => x.alive)) {
    const ss = s.civSettlements(c).filter((x) => x.starving > 20 && s.pop(x) > 8);
    if (!ss.length || w.agents.some((a) => a.kind === 'settlers' && a.purpose === 'refugee' && a.civ === c.id)) continue;
    const from = ss.sort((a, b) => s.pop(b) - s.pop(a))[0];
    const targets = alive.filter((o) => o.civ !== c.id && w.civs[o.civ].alive)
      .filter((o) => o.starving === 0 && !s.atWar(c.id, o.civ) && s.g.dist(o.tile, from.tile) <= 22 && s.pop(o) < s.housing(o) + 6)
      .sort((a, b) => s.g.dist(a.tile, from.tile) - s.g.dist(b.tile, from.tile));
    const to = targets[0];
    if (!to || !s.rng.chance(0.5)) continue;
    const path = s.path(from.tile, to.tile);
    if (!path) continue;
    const n = Math.min(6, Math.max(2, Math.round(s.pop(from) * 0.12)));
    const pop = s.removePop(from, n);
    w.agents.push({ id: s.id(), kind: 'settlers', civ: c.id, path, step: 0, progress: 0, speed: 0.45, pop, from: from.id, to: to.id, purpose: 'refugee' });
    s.metric('famineMigration');
    s.log('migration', `Açlıktan kaçan ${n} kişi ${ek(from.name, 'dan')} ${ek(to.name, 'a')} doğru yola düştü.`, { civ: c.id, tile: from.tile, major: true, cause: `${c.name} ambarları boş` });
  }
}

function startPlague(s: Sim, st: Settlement, why: string) {
  st.plague = { since: s.day, until: s.day + s.rng.int(60, 150), severity: 0.6 + s.rng.next() * 0.8, dead: 0 };
  s.metric('plague');
  s.log('world', `${ek(st.name, 'da')} salgın başladı!`, { civ: st.civ, tile: st.tile, major: true, cause: why });
}
