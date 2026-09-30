// Gezgin kamerası: dünyanın içinde serbest dolaşma. İki kip:
//  uçuş  — drone gibi: WASD yatay, Boşluk/E yukarı, C/Q aşağı, Shift hızlı; hız yere uzaklıkla artar
//  yürüyüş — göz hizasında, zemini izler; Boşluk zıplar, suda yüzer, Shift koşar
// Bakış: sürükle (sol ya da sağ tuş). Tekerlek: uçuşta bakış yönünde ileri/geri.
import * as THREE from 'three';

export interface FreeCamEnv {
  ground: (x: number, z: number) => number;
  water: number;
  bounds: () => { minX: number; maxX: number; minZ: number; maxZ: number };
  /** bakılan zemin noktası (yürümeye geçince oraya inilir) */
  look?: () => THREE.Vector3 | null;
  /** yürürken yapılara çarpma: yeni (x, z) → düzeltilmiş (x, z); önceki konum hangi taraftan gelindiğini söyler */
  collide?: (x: number, z: number, px: number, pz: number) => [number, number];
}

const EYE = 0.34;
const SWIM_EYE = 0.26; // dalga tepesi ±0.11: göz suyun altına girmesin

export class FreeCam {
  on = false;
  walk = false;
  yaw = 0;
  pitch = -0.25;
  swimming = false;
  /** son karedeki yatay hız (yürüyüş sesi / bob için) */
  speed = 0;
  /** atılan adım sayısı (ayak sesi için); yüzerken kulaç */
  steps = 0;
  private stepPh = 0;
  onModeChange?: () => void;
  /** kare süresi üst sınırı (çok düşük kare hızında hareket yavaşlar ama sıçramaz) */
  maxDt = 0.1;
  private pos = new THREE.Vector3();
  private vel = new THREE.Vector3();
  private vy = 0;
  private grounded = false;
  private keys = new Set<string>();
  private drag: { x: number; y: number; id: number } | null = null;
  private glide: { from: THREE.Vector3; to: THREE.Vector3; y0: number; y1: number; p0: number; p1: number; t: number; dur: number; then?: () => void } | null = null;
  private bob = 0;
  private wheel = 0;

  constructor(private cam: THREE.PerspectiveCamera, private dom: HTMLElement, private env: FreeCamEnv) {
    const typing = (e: Event) => { const t = e.target; return (t instanceof HTMLInputElement && t.type !== 'checkbox') || t instanceof HTMLSelectElement || t instanceof HTMLTextAreaElement; };
    window.addEventListener('keydown', (e) => {
      if (!this.on || typing(e) || e.ctrlKey || e.metaKey || e.altKey) return;
      if (MOVE_KEYS.has(e.code)) {
        this.keys.add(e.code);
        // Boşluk burada zıplama/yükselme: oyunu duraklatmasın, sayfa kaymasın
        if (e.code === 'Space' || e.code.startsWith('Arrow')) e.preventDefault();
        // diğer kısayollar (duraklat, yörünge kamerası, kaydırma) bu tuşları görmesin
        e.stopPropagation();
        if (e.code === 'Space' && this.walk && this.grounded && !this.swimming && !e.repeat) { this.vy = 2.7; this.grounded = false; }
      }
      if (e.code === 'KeyV' && !e.repeat) { e.stopPropagation(); this.setWalk(!this.walk); }
    }, true);
    window.addEventListener('keyup', (e) => { this.keys.delete(e.code); });
    window.addEventListener('blur', () => { this.keys.clear(); this.drag = null; });
    dom.addEventListener('pointerdown', (e) => {
      if (!this.on) return;
      this.drag = { x: e.clientX, y: e.clientY, id: e.pointerId };
      try { dom.setPointerCapture(e.pointerId); } catch { /* yok */ }
    });
    dom.addEventListener('pointermove', (e) => {
      if (!this.on || !this.drag || e.pointerId !== this.drag.id) return;
      const dx = e.clientX - this.drag.x, dy = e.clientY - this.drag.y;
      this.drag.x = e.clientX; this.drag.y = e.clientY;
      const k = 0.0034 * (this.cam.fov / 60);
      this.yaw -= dx * k;
      this.pitch = Math.max(-1.48, Math.min(1.35, this.pitch - dy * k));
      this.glide = null;
    });
    const end = (e: PointerEvent) => { if (this.drag && e.pointerId === this.drag.id) { this.drag = null; try { dom.releasePointerCapture(e.pointerId); } catch { /* yok */ } } };
    dom.addEventListener('pointerup', end);
    dom.addEventListener('pointercancel', end);
    dom.addEventListener('contextmenu', (e) => { if (this.on) e.preventDefault(); });
    dom.addEventListener('wheel', (e) => { if (!this.on) return; e.preventDefault(); this.wheel += Math.sign(e.deltaY) * Math.min(3, Math.abs(e.deltaY) / 60); }, { passive: false });
  }

  /** gezgine geç: kamera olduğu yerden, baktığı noktaya dönük başlar */
  enter(look: THREE.Vector3) {
    this.on = true;
    this.pos.copy(this.cam.position);
    const d = look.clone().sub(this.pos);
    this.yaw = Math.atan2(-d.x, -d.z);
    this.pitch = Math.max(-1.45, Math.min(0.5, Math.asin(Math.max(-1, Math.min(1, d.y / Math.max(1e-6, d.length()))))));
    this.vel.set(0, 0, 0); this.vy = 0; this.keys.clear();
    // çok yüksekteyse bakılan yerin yakınına süzül: dünyanın içine in
    const alt = this.pos.y - this.floorAt(this.pos.x, this.pos.z);
    if (alt > 30) {
      const back = new THREE.Vector3(Math.sin(this.yaw), 0, Math.cos(this.yaw)).multiplyScalar(13);
      const to = new THREE.Vector3(look.x + back.x, 0, look.z + back.z);
      to.y = Math.max(this.floorAt(to.x, to.z), look.y) + 5.5;
      this.glideTo(to, this.yaw, -0.36, 1.6);
    }
    this.apply();
    this.onModeChange?.();
  }
  exit() {
    this.on = false; this.glide = null; this.keys.clear(); this.drag = null;
    this.onModeChange?.();
  }
  setWalk(v: boolean) {
    if (!this.on || v === this.walk) return;
    this.walk = v;
    this.vy = 0;
    if (v) {
      // baktığın yere in (kara ise); bakılan yer uzak ya da suysa hemen aşağıya
      const lp = this.env.look?.();
      const to = this.pos.clone();
      if (lp && Math.hypot(lp.x - to.x, lp.z - to.z) < 90 && this.env.ground(lp.x, lp.z) > this.env.water + 0.02) {
        // bakılan yerin biraz önüne: kasabanın ortasına, bir kulenin içine değil; önünde durup ona bakarsın
        const back = Math.min(3, Math.hypot(lp.x - to.x, lp.z - to.z) * 0.4);
        to.x = lp.x + Math.sin(this.yaw) * back; to.z = lp.z + Math.cos(this.yaw) * back;
        if (this.env.ground(to.x, to.z) < this.env.water + 0.02) { to.x = lp.x; to.z = lp.z; }
      }
      to.y = this.floorAt(to.x, to.z) + this.eye(to.x, to.z);
      const d = this.pos.distanceTo(to);
      if (d > 0.6) this.glideTo(to, this.yaw, -0.06, Math.min(2.6, 0.6 + d * 0.03));
      else this.pitch = Math.max(-0.5, this.pitch);
    } else {
      this.vel.y = 3.5;
    }
    this.onModeChange?.();
  }
  /** yumuşak geçiş (inme, bir yere uçma) */
  glideTo(to: THREE.Vector3, yaw: number, pitch: number, dur = 1.2, then?: () => void) {
    let y1 = yaw;
    while (y1 - this.yaw > Math.PI) y1 -= Math.PI * 2;
    while (y1 - this.yaw < -Math.PI) y1 += Math.PI * 2;
    this.glide = { from: this.pos.clone(), to: to.clone(), y0: this.yaw, y1, p0: this.pitch, p1: pitch, t: 0, dur, then };
  }
  /** bir noktaya uç: noktanın biraz gerisine ve üstüne */
  flyTo(p: THREE.Vector3) {
    const d = new THREE.Vector3(p.x - this.pos.x, 0, p.z - this.pos.z);
    const yaw = d.lengthSq() > 1e-4 ? Math.atan2(-d.x, -d.z) : this.yaw;
    const back = new THREE.Vector3(Math.sin(yaw), 0, Math.cos(yaw));
    if (this.walk) {
      const to = new THREE.Vector3(p.x + back.x * 1.4, 0, p.z + back.z * 1.4);
      to.y = this.floorAt(to.x, to.z) + this.eye(to.x, to.z);
      this.glideTo(to, yaw, -0.12, Math.min(3, 0.8 + d.length() * 0.05));
    } else {
      const to = new THREE.Vector3(p.x + back.x * 7, 0, p.z + back.z * 7);
      to.y = Math.max(this.floorAt(to.x, to.z), p.y) + 3.2;
      this.glideTo(to, yaw, -0.38, Math.min(3, 0.8 + d.length() * 0.03));
    }
  }
  private floorAt(x: number, z: number) { return Math.max(this.env.ground(x, z), this.env.water - 0.12); }
  private eye(x: number, z: number) { return this.env.ground(x, z) < this.env.water - 0.08 ? SWIM_EYE : EYE; }
  /** zeminden yükseklik */
  altitude() { return this.pos.y - Math.max(this.env.ground(this.pos.x, this.pos.z), this.env.water); }
  forward(out = new THREE.Vector3()) { return out.set(-Math.sin(this.yaw) * Math.cos(this.pitch), Math.sin(this.pitch), -Math.cos(this.yaw) * Math.cos(this.pitch)); }
  held() { return this.keys.size > 0 || !!this.drag; }

  update(dt: number) {
    if (!this.on) return;
    dt = Math.min(this.maxDt, dt);
    if (this.glide) {
      const g = this.glide;
      g.t += dt / g.dur;
      const u = Math.min(1, g.t), e = u * u * (3 - 2 * u);
      this.pos.lerpVectors(g.from, g.to, e);
      // yol üstünde zemin yükselirse içinden geçme
      const fl = this.floorAt(this.pos.x, this.pos.z) + (this.walk ? this.eye(this.pos.x, this.pos.z) : 0.4);
      if (this.pos.y < fl) this.pos.y = fl;
      this.yaw = g.y0 + (g.y1 - g.y0) * e; this.pitch = g.p0 + (g.p1 - g.p0) * e;
      if (u >= 1) { this.glide = null; g.then?.(); }
      this.vel.set(0, 0, 0);
      if (this.keys.size && u > 0.3) this.glide = null; // oyuncu hareket ederse bırak
      this.apply();
      return;
    }
    const k = this.keys;
    const fwd = (k.has('KeyW') || k.has('ArrowUp') ? 1 : 0) - (k.has('KeyS') || k.has('ArrowDown') ? 1 : 0);
    const str = (k.has('KeyD') || k.has('ArrowRight') ? 1 : 0) - (k.has('KeyA') || k.has('ArrowLeft') ? 1 : 0);
    // Ctrl bilerek yok: Ctrl+W sekmeyi kapatır
    const up = (k.has('Space') || k.has('KeyE') ? 1 : 0) - (k.has('KeyC') || k.has('KeyQ') ? 1 : 0);
    const fast = k.has('ShiftLeft') || k.has('ShiftRight');
    const f = new THREE.Vector3(-Math.sin(this.yaw), 0, -Math.cos(this.yaw)), r = new THREE.Vector3(Math.cos(this.yaw), 0, -Math.sin(this.yaw));
    const dir = f.multiplyScalar(fwd).add(r.multiplyScalar(str));
    if (dir.lengthSq() > 1) dir.normalize();
    if (this.walk) {
      const gy = this.env.ground(this.pos.x, this.pos.z);
      this.swimming = gy < this.env.water - 0.08;
      const sp = (fast ? 3.4 : 1.2) * (this.swimming ? 0.5 : 1);
      const want = dir.multiplyScalar(sp);
      const a = 1 - Math.exp(-dt * (this.grounded || this.swimming ? 11 : 2.5));
      this.vel.x += (want.x - this.vel.x) * a; this.vel.z += (want.z - this.vel.z) * a;
      // dik yamaçta yavaşla (tırmanış)
      const nx = this.pos.x + this.vel.x * dt, nz = this.pos.z + this.vel.z * dt;
      const climb = (this.env.ground(nx, nz) - gy) / Math.max(1e-4, Math.hypot(this.vel.x, this.vel.z) * dt);
      const slow = climb > 0.9 ? Math.max(0.35, 1 - (climb - 0.9) * 0.6) : 1;
      // düşük kare hızında surun içinden geçilmesin: adım küçük parçalara bölünür
      const mx = this.vel.x * dt * slow, mz = this.vel.z * dt * slow;
      const nsub = Math.max(1, Math.ceil(Math.hypot(mx, mz) / 0.06));
      for (let q = 0; q < nsub; q++) {
        const ox = this.pos.x, oz = this.pos.z;
        this.pos.x += mx / nsub; this.pos.z += mz / nsub;
        if (this.env.collide) { const [cx, cz] = this.env.collide(this.pos.x, this.pos.z, ox, oz); this.pos.x = cx; this.pos.z = cz; }
      }
      this.vy -= 7.8 * dt;
      this.pos.y += this.vy * dt;
      const floor = this.floorAt(this.pos.x, this.pos.z) + this.eye(this.pos.x, this.pos.z);
      if (this.pos.y <= floor) {
        // yokuş yukarı ani sıçramayı yumuşat
        this.pos.y = this.grounded && floor - this.pos.y < 0.35 ? this.pos.y + (floor - this.pos.y) * Math.min(1, dt * 18) : floor;
        if (this.pos.y < floor - 0.35) this.pos.y = floor - 0.35;
        this.vy = 0; this.grounded = true;
      } else if (this.pos.y > floor + 0.05) this.grounded = false;
      if (this.swimming) { this.pos.y = floor + Math.sin(performance.now() / 700) * 0.015; this.vy = 0; this.grounded = true; }
      this.speed = Math.hypot(this.vel.x, this.vel.z);
      // adım: hıza bağlı ritim (koşarken sık), yalnız yerdeyken; yüzerken seyrek kulaç
      if ((this.grounded || this.swimming) && this.speed > 0.25) {
        const rate = this.swimming ? 0.8 : 1.4 + this.speed * 0.55;
        this.stepPh += dt * rate;
        if (this.stepPh >= 1) { this.stepPh -= 1; this.steps++; }
      } else this.stepPh = 0.7;
    } else {
      this.swimming = false;
      const alt = Math.max(0, this.altitude());
      const sp = Math.min(90, (2.4 + alt * 0.85) * (fast ? 3 : 1));
      const want = dir.multiplyScalar(sp);
      want.y = up * sp * 0.75;
      if (this.wheel) { const fv = this.forward(); want.addScaledVector(fv, -this.wheel * sp * 1.4); this.wheel *= Math.exp(-dt * 7); if (Math.abs(this.wheel) < 0.02) this.wheel = 0; }
      const a = 1 - Math.exp(-dt * 3.6);
      this.vel.lerp(want, a);
      this.pos.addScaledVector(this.vel, dt);
      const floor = this.floorAt(this.pos.x, this.pos.z) + 0.28;
      if (this.pos.y < floor) { this.pos.y = floor; if (this.vel.y < 0) this.vel.y = 0; }
      if (this.pos.y > 170) { this.pos.y = 170; if (this.vel.y > 0) this.vel.y = 0; }
      this.speed = Math.hypot(this.vel.x, this.vel.z);
    }
    if (this.walk) this.wheel = 0;
    const b = this.env.bounds();
    this.pos.x = Math.max(b.minX, Math.min(b.maxX, this.pos.x));
    this.pos.z = Math.max(b.minZ, Math.min(b.maxZ, this.pos.z));
    this.apply(dt);
  }
  private apply(dt = 0) {
    this.cam.position.copy(this.pos);
    // yürürken hafif baş sallantısı
    if (this.walk && this.grounded && !this.swimming && this.speed > 0.2) {
      this.bob += dt * (4 + this.speed * 2.6);
      this.cam.position.y += Math.sin(this.bob * 2) * 0.008 * Math.min(1.8, this.speed);
    }
    this.cam.rotation.set(this.pitch, this.yaw, 0, 'YXZ');
    this.cam.updateMatrixWorld();
  }
}

const MOVE_KEYS = new Set(['KeyW', 'KeyA', 'KeyS', 'KeyD', 'ArrowUp', 'ArrowDown', 'ArrowLeft', 'ArrowRight', 'Space', 'KeyE', 'KeyQ', 'KeyC', 'ShiftLeft', 'ShiftRight']);
