// Ortam sesi: dosya yok, her şey WebAudio ile üretilir.
// Kameranın baktığı yere göre rüzgâr, dalga, kalabalık, kuş, çekiç, cırcır böceği, yağmur, savaş, bayram müziği.

export interface SoundInfo {
  /** kamera uzaklığı (yakınlık için) */
  dist: number;
  night: number;
  rain: number;
  water: number;   // 0..1 çevredeki su oranı
  forest: number;  // 0..1 ağaç/çayır
  town: number;    // 0..1 kasaba yakınlığı × nüfus
  work: number;    // 0..1 atölye yoğunluğu
  fest: number;    // 0..1 yakında bayram
  battle: number;  // 0..1 yakında çarpışma
  fire: number;    // 0..1 yakında yangın
  tavern: number;  // 0..1 gece meyhane
}

const PENTA = [0, 2, 4, 7, 9, 12, 14];

export class Ambience {
  private ctx: AudioContext | null = null;
  private master!: GainNode;
  private noise!: AudioBuffer;
  private g: Record<string, GainNode> = {};
  private crowdF!: BiquadFilterNode;
  private last = 0;
  private melody = { next: 0, root: 60, step: 0 };
  enabled = false;
  volume = 0.7;

  private build() {
    const ctx = new AudioContext();
    this.ctx = ctx;
    this.master = ctx.createGain();
    this.master.gain.value = 0;
    this.master.connect(ctx.destination);
    // 3 sn beyaz gürültü (döngü)
    const len = ctx.sampleRate * 3;
    this.noise = ctx.createBuffer(1, len, ctx.sampleRate);
    const d = this.noise.getChannelData(0);
    for (let i = 0; i < len; i++) d[i] = Math.random() * 2 - 1;
    const bed = (name: string, type: BiquadFilterType, freq: number, q = 0.7) => {
      const src = ctx.createBufferSource(); src.buffer = this.noise; src.loop = true;
      src.playbackRate.value = 0.8 + Math.random() * 0.4;
      const f = ctx.createBiquadFilter(); f.type = type; f.frequency.value = freq; f.Q.value = q;
      const gn = ctx.createGain(); gn.gain.value = 0;
      src.connect(f); f.connect(gn); gn.connect(this.master);
      src.start(ctx.currentTime + Math.random());
      this.g[name] = gn;
      return f;
    };
    bed('wind', 'lowpass', 380);
    bed('sea', 'lowpass', 620);
    this.crowdF = bed('crowd', 'bandpass', 850, 1.1);
    bed('rain', 'highpass', 2400);
    bed('fire', 'bandpass', 1400, 0.5);
  }

  async setEnabled(on: boolean) {
    this.enabled = on;
    if (on && !this.ctx) this.build();
    if (!this.ctx) return;
    if (on && this.ctx.state === 'suspended') await this.ctx.resume().catch(() => undefined);
    this.master.gain.setTargetAtTime(on ? this.volume : 0, this.ctx.currentTime, 0.4);
  }

  /** tarayıcı kullanıcı hareketi olmadan sesi başlatmaz */
  resumeOnGesture() { if (this.enabled && this.ctx?.state === 'suspended') void this.ctx.resume(); }

  update(now: number, s: SoundInfo) {
    const ctx = this.ctx;
    if (!ctx || !this.enabled || ctx.state !== 'running') return;
    const dt = Math.min(0.5, (now - this.last) / 1000);
    if (dt < 0.1) return;
    this.last = now;
    const T = ctx.currentTime;
    const close = Math.max(0, Math.min(1, 1 - (s.dist - 6) / 60));
    const day = 1 - s.night;
    const set = (k: string, v: number) => this.g[k].gain.setTargetAtTime(v, T, 0.6);
    // yükseldikçe rüzgâr artar
    set('wind', 0.05 + (1 - close) * 0.16 + Math.sin(now / 5300) * 0.025 + s.rain * 0.05);
    set('sea', s.water * (0.1 + close * 0.22) * (0.75 + Math.sin(now / 2100) * 0.25));
    set('rain', s.rain * 0.16);
    set('fire', s.fire * close * 0.1 * (0.6 + Math.random() * 0.8));
    // kalabalık uğultusu: dalgalanan genlik ve formant
    const murmur = s.town * close * (day * 0.9 + s.tavern * 0.5) * (0.12 + s.fest * 0.1);
    this.g.crowd.gain.setTargetAtTime(murmur * (0.55 + Math.random() * 0.9), T, 0.08);
    this.crowdF.frequency.setTargetAtTime(650 + Math.random() * 600, T, 0.1);
    // kısa olaylar
    const p = (rate: number) => Math.random() < rate * dt;
    if (p(day * s.forest * close * 0.9)) this.bird(T);
    if (p(s.night * s.forest * close * 1.6)) this.cricket(T);
    if (p(day * s.work * close * 2.2)) this.hammer(T);
    if (p(s.battle * close * 6)) this.clash(T);
    if (p(s.fire * close * 3)) this.crackle(T);
    if (p(s.tavern * close * 1.2)) this.pluck(T, 55 + PENTA[Math.floor(Math.random() * 5)]);
    if (s.fest * close > 0.05) this.tune(T, s.fest * close);
    if (p(s.town * close * day * 0.05)) this.bell(T);
  }

  private env(t: number, peak: number, a: number, d: number) {
    const gn = this.ctx!.createGain();
    gn.gain.setValueAtTime(0.0001, t);
    gn.gain.exponentialRampToValueAtTime(peak, t + a);
    gn.gain.exponentialRampToValueAtTime(0.0001, t + a + d);
    gn.connect(this.master);
    return gn;
  }
  private osc(type: OscillatorType, f: number, t: number, dur: number, out: AudioNode) {
    const o = this.ctx!.createOscillator(); o.type = type; o.frequency.setValueAtTime(f, t);
    o.connect(out); o.start(t); o.stop(t + dur + 0.05);
    return o;
  }
  private bird(t: number) {
    const n = 2 + Math.floor(Math.random() * 4), base = 2400 + Math.random() * 1600;
    for (let k = 0; k < n; k++) {
      const s = t + k * (0.09 + Math.random() * 0.05);
      const gn = this.env(s, 0.035, 0.01, 0.07);
      const o = this.osc('sine', base, s, 0.09, gn);
      o.frequency.exponentialRampToValueAtTime(base * (1.2 + Math.random() * 0.4), s + 0.05);
      o.frequency.exponentialRampToValueAtTime(base * 0.9, s + 0.09);
    }
  }
  private cricket(t: number) {
    const f = 4200 + Math.random() * 600;
    for (let k = 0; k < 3; k++) { const s = t + k * 0.06; this.osc('square', f, s, 0.03, this.env(s, 0.006, 0.005, 0.03)); }
  }
  private hammer(t: number) {
    const f = 1500 + Math.random() * 900;
    const gn = this.env(t, 0.05, 0.002, 0.35);
    this.osc('triangle', f, t, 0.4, gn); this.osc('sine', f * 2.76, t, 0.2, this.env(t, 0.02, 0.002, 0.15));
    if (Math.random() < 0.5) { const s = t + 0.45; this.osc('triangle', f, s, 0.4, this.env(s, 0.04, 0.002, 0.3)); }
  }
  private noiseBurst(t: number, type: BiquadFilterType, f: number, peak: number, d: number) {
    const src = this.ctx!.createBufferSource(); src.buffer = this.noise;
    const fl = this.ctx!.createBiquadFilter(); fl.type = type; fl.frequency.value = f;
    src.connect(fl); fl.connect(this.env(t, peak, 0.003, d));
    src.start(t, Math.random() * 2, d + 0.05);
  }
  private clash(t: number) {
    this.noiseBurst(t, 'bandpass', 2500 + Math.random() * 2000, 0.07, 0.12);
    this.osc('triangle', 900 + Math.random() * 1200, t, 0.25, this.env(t, 0.03, 0.002, 0.22));
    if (Math.random() < 0.3) this.noiseBurst(t + 0.05, 'lowpass', 300, 0.08, 0.3);
  }
  private crackle(t: number) { for (let k = 0; k < 3; k++) this.noiseBurst(t + Math.random() * 0.2, 'highpass', 3000, 0.04, 0.02); }
  private bell(t: number) {
    const f = 330 + Math.floor(Math.random() * 3) * 60;
    for (const [m, a] of [[1, 0.05], [2.4, 0.02], [3.9, 0.012]]) this.osc('sine', f * m, t, 3, this.env(t, a, 0.005, 2.8));
  }
  private pluck(t: number, midi: number) {
    const f = 440 * Math.pow(2, (midi - 69) / 12);
    this.osc('triangle', f, t, 0.9, this.env(t, 0.05, 0.003, 0.8));
    this.osc('sine', f * 2, t, 0.4, this.env(t, 0.02, 0.003, 0.35));
  }
  /** bayram ezgisi: pentatonik flüt + davul */
  private tune(t: number, amt: number) {
    const m = this.melody;
    if (t < m.next) return;
    const dur = Math.random() < 0.3 ? 0.5 : 0.25;
    m.step = Math.max(0, Math.min(PENTA.length - 1, m.step + Math.floor(Math.random() * 3) - 1));
    const f = 440 * Math.pow(2, (m.root + 12 + PENTA[m.step] - 69) / 12);
    const gn = this.env(t, 0.045 * amt + 0.01, 0.03, dur * 0.9);
    const o = this.osc('sine', f, t, dur, gn);
    const lfo = this.ctx!.createOscillator(); const lg = this.ctx!.createGain(); lfo.frequency.value = 5.5; lg.gain.value = f * 0.01; lfo.connect(lg); lg.connect(o.frequency); lfo.start(t); lfo.stop(t + dur);
    if (Math.floor(t * 4) % 2 === 0) this.noiseBurst(t, 'lowpass', 160, 0.12 * amt, 0.12);
    m.next = t + dur;
    if (Math.random() < 0.05) m.root = [57, 60, 62][Math.floor(Math.random() * 3)];
  }
}
