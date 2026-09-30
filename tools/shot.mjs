// Oyun içi ekran görüntüsü: node tools/shot.mjs out.png [gun] [seed]
// İnsan (paladin/keşiş) medeniyetinin en kalabalık yerleşimine alçak açıdan bakar.
import { chromium } from 'playwright';
import path from 'path';
const out = process.argv[2] || 'shot.png';
const day = process.argv[3] || '500';
const seed = process.argv[4] || '';
const dist = Number(process.argv[5] || 3.2);
const html = path.resolve(process.env.HTML || 'dist/fantastik-dunya.html');
const b = await chromium.launch({ args: ['--use-gl=angle', '--use-angle=swiftshader', '--enable-unsafe-swiftshader', '--ignore-gpu-blocklist'] });
const p = await b.newPage({ viewport: { width: 1280, height: 720 } });
p.on('pageerror', (e) => console.log('PAGEERR', e.message));
p.on('console', (m) => { if (m.type() === 'error') console.log('CONSOLE', m.text()); });
await p.addInitScript(() => { try { localStorage.setItem('fd-cine', '0'); } catch {} });
await p.goto('file://' + html + `?debug&hi=${process.argv[6] || 4}&fov=${process.argv[7] || 0}&gun=${day}&cyc=${process.env.CYC || 0.42}&el=${process.env.EL || 0.8}` + (seed ? `&seed=${seed}` : ''));
await p.waitForFunction(() => window.__fd && window.__fd.dio, null, { timeout: 120000 });
await p.waitForTimeout(3000);
const info = await p.evaluate(({ dist }) => {
  const { sim, dio } = window.__fd;
  const w = sim.w;
  const human = new Set(w.civs.filter((c) => c.race === 'human').map((c) => c.id));
  const cands = w.settlements.filter((s) => s.alive && human.has(s.civ));
  const pool = cands.length ? cands : w.settlements.filter((s) => s.alive);
  pool.sort((a, b) => sim.pop(b) - sim.pop(a));
  const st = pool[0];
  const Q = new URLSearchParams(location.search); dio.debugCyc = Number(Q.get('cyc') || 0.42); const el = Number(Q.get('el') || 0.8);
  const lay = dio.layouts.get(st.id);
  const hs = lay.houses.filter((h) => !h.burnt && h.race === 'human');
  const hi = Number(new URLSearchParams(location.search).get('hi') ?? 4); const h = hs[Math.min(hs.length - 1, hi)] || lay.houses[0];
  const ox = h.x - lay.x, oz = h.z - lay.z, ol = Math.hypot(ox, oz) || 1, ux = ox / ol, uz = oz / ol;
  const x = h.x, z = h.z, y = h.y;
  window.__shotCam = () => {
    dio.tween = null; dio.follow = null; dio.autoOrbit = false;
    dio.controls.target.set(x, y + 0.2, z);
    dio.camera.position.set(x + ux * dist * 0.8 + uz * dist * 0.4, y + dist * el, z + uz * dist * 0.8 - ux * dist * 0.4);
    dio.controls.update();
    const fov = Number(new URLSearchParams(location.search).get('fov') || 0); if (fov) { dio.updateLens = () => {}; dio.camera.fov = fov; dio.camera.near = 0.05; dio.camera.updateProjectionMatrix(); }
  };
  window.__shotCam();
  return { h: [h.x, h.y, h.z, h.kind], c: [lay.x, lay.z], nh: hs.length, season: sim.season, day: sim.day, name: st.name, civ: w.civs.find((c) => c.id === st.civ).race, pop: sim.pop(st), tier: st.tier, humanCivs: human.size };
}, { dist });
console.log(JSON.stringify(info));
await p.evaluate(() => { document.querySelectorAll('.hud, #hud, header, .panel').forEach((e) => (e.style.display = 'none')); });
await p.keyboard.press('u');
await p.waitForTimeout(2500);
await p.evaluate(() => window.__shotCam());
await p.waitForTimeout(3500);
await p.evaluate(() => window.__shotCam());
if (process.env.EVAL) console.log('EVAL', JSON.stringify(await p.evaluate(process.env.EVAL)));
await p.waitForTimeout(1500);
await p.screenshot({ path: out, timeout: 180000 });
console.log(JSON.stringify(await p.evaluate(() => { const d = window.__fd.dio; return [d.camera.position, d.controls.target]; })));
await b.close();
