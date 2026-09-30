import { Sim } from './sim/sim';
import { TECH } from './data/techs';
import { CLASSES } from './data/classes';
import { GOODS, type Good } from './data/goods';

const seeds = process.argv[2] ? process.argv[2].split(',').map(Number) : [1, 2, 3, 4, 5];
const days = Number(process.argv[3] ?? 1800);
const verbose = process.argv.includes('-v');
const f = (n: number) => Math.round(n);

for (const seed of seeds) {
  const t0 = Date.now();
  const sim = new Sim(seed);
  for (let i = 0; i < days; i++) sim.step();
  const w = sim.w, m = w.metrics;
  console.log(`\n=== seed ${seed} · ${days} gün · ${Date.now() - t0}ms · ${w.events.length} olay ===`);
  for (const c of w.civs) {
    const st = Object.entries(c.stock).filter(([, v]) => (v ?? 0) >= 1).sort((a, b) => (b[1] ?? 0) * GOODS[b[0] as Good].base - (a[1] ?? 0) * GOODS[a[0] as Good].base).slice(0, 7).map(([k, v]) => `${k}:${f(v!)}`).join(' ');
    console.log(`  ${CLASSES[c.cls].name.padEnd(9)} ${c.alive ? '' : '✝'} era=${c.era} sub=${c.subclass ?? '-'} pop=${sim.civPop(c)} setl=${sim.civSettlements(c).length} techs=${c.research.done.length} cur=${c.research.current ?? '-'} heroes=${sim.civHeroes(c).length} sold=${sim.civSettlements(c).reduce((a, s) => a + s.soldiers, 0)} thr=${c.threat.toFixed(1)} traded=${c.stats.traded} | ${st}`);
  }
  const ext: Record<string, number> = {};
  for (const t of w.tiles) if (t.ext) ext[`${t.ext.kind}L${t.ext.level}`] = (ext[`${t.ext.kind}L${t.ext.level}`] ?? 0) + 1;
  console.log('  yapılar:', JSON.stringify(ext));
  const ws: Record<string, number> = {};
  for (const st of w.settlements) if (st.alive) { for (const k in st.workshops) ws[k] = (ws[k] ?? 0) + (st.workshops as Record<string, number>)[k]; for (const k in st.civics) ws[k] = (ws[k] ?? 0) + (st.civics as Record<string, number>)[k]; }
  console.log('  yerleşim yapıları:', JSON.stringify(ws));
  console.log('  kamplar:', w.camps.map((c) => `${c.kind}:${c.alive ? c.count : '✝'}`).join(' '), '| yatak tükenen:', w.deposits.filter((d) => d.depleted).length);
  console.log('  metrics:', JSON.stringify(Object.fromEntries(Object.entries(m).filter(([k]) => !k.startsWith('ev_') && !k.startsWith('research_')))));
  const alive = w.civs.filter((c) => c.alive);
  const crit = {
    herkesKöy: alive.every((c) => c.era >= 2),
    biriKasaba: alive.some((c) => c.era >= 3),
    yatakTükendi: (m.depleted ?? 0) > 0,
    çekişme2: (m.tension ?? 0) >= 2 || (m.treaty ?? 0) + (m.tradeRoute ?? 0) >= 2,
    altSınıf: (m.subclass ?? 0) >= alive.length,
  };
  console.log('  KRİTER:', Object.entries(crit).map(([k, v]) => (v ? '✓' : '✗') + k).join(' '));
  // ---- tarafsız han ve kahraman iradesi
  const innIds = new Set(w.inns.map((i) => i.id));
  const innBorn = w.heroes.filter((h) => innIds.has(h.birth));
  const innHired = innBorn.filter((h) => (h.hired ?? 0) > 0);
  const hires = w.civs.map((c) => m[`innHire_${c.id}`] ?? 0);
  const totalHires = hires.reduce((a, b) => a + b, 0);
  const pact = w.civs.some((c) => sim.e(c, 'pact') > 0);
  const han = {
    han2: w.inns.length >= 2,
    ilan6: (m.questDone ?? 0) >= 6,
    kiralama40: innBorn.length > 0 && innHired.length / innBorn.length >= 0.4,
    tekel50: totalHires === 0 || Math.max(...hires) / totalHires <= 0.5,
    kampVar: w.camps.some((c) => c.alive),
    baskın: pact ? (m.innBreak ?? 0) <= 2 : (m.innBreak ?? 0) === 0,
    yokOlmaz: w.civs.every((c) => c.alive),
  };
  console.log(`  HAN: ${w.inns.length} han (${w.inns.filter((i) => i.alive).length} ayakta) · ilan ${m.questDone ?? 0}/${m.questPosted ?? 0}+${m.innQuest ?? 0} · han kahramanı ${innBorn.length}, kiralanan ${innHired.length} (%${innBorn.length ? Math.round(innHired.length / innBorn.length * 100) : 0}) · kiralama ${hires.join('/')} · kamp ${w.camps.filter((c) => c.alive).length} · baskın ${m.innBreak ?? 0}${pact ? ' (Paktçı var)' : ''} · canavar baskını ${m.innMonsterRaid ?? 0}, yıkılan ${m.innRuined ?? 0}`);
  console.log(`  İRADE: av ${m.goalHunt ?? 0} · harabe ${m.goal_ruin ?? 0} · şifa ${m.goal_plague ?? 0} · hac ${m.goal_temple ?? 0} · kütüphane ${m.goal_library ?? 0} · soygun ${m.goalRob ?? 0} · düello ${m.duel ?? 0} · lakap ${m.epithet ?? 0} · efsane ${m.legend ?? 0} · emekli ${m.retire ?? 0} · iz ${Object.entries(m).filter(([k]) => k.startsWith('trait_')).map(([k, v]) => k.slice(6) + ':' + v).join(' ')}`);
  console.log('  HAN KRİTER:', Object.entries(han).map(([k, v]) => (v ? '✓' : '✗') + k).join(' '));
  if (verbose) for (const e of w.events.filter((e) => e.major)) console.log(`   [${sim.dateStr(e.day)}] ${e.text}${e.cause ? '  ← ' + e.cause : ''}`);
}
void TECH;
