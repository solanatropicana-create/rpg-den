// Atmosfer: dalgalı deniz, bulutlar ve gölgeleri, yıldızlar, kar ve yağmur.
import * as THREE from 'three';

/** yıldız ve kar tanesi için yumuşak yuvarlak nokta (kare piksel yerine) */
function dotTexture(): THREE.Texture | null {
  if (typeof document === 'undefined') return null;
  const c = document.createElement('canvas'); c.width = c.height = 32;
  const g = c.getContext('2d'); if (!g) return null;
  const gr = g.createRadialGradient(16, 16, 0, 16, 16, 16);
  gr.addColorStop(0, 'rgba(255,255,255,1)'); gr.addColorStop(0.45, 'rgba(255,255,255,0.85)'); gr.addColorStop(1, 'rgba(255,255,255,0)');
  g.fillStyle = gr; g.fillRect(0, 0, 32, 32);
  const t = new THREE.CanvasTexture(c); t.colorSpace = THREE.SRGBColorSpace;
  return t;
}

const rnd = (i: number, k: number) => { const v = Math.sin(i * 127.1 + k * 311.7) * 43758.5453; return v - Math.floor(v); };

export class Atmosphere {
  group = new THREE.Group();
  private uTime = { value: 0 };
  private sea: THREE.Mesh;
  private outer: THREE.Mesh;
  private stars: THREE.Points;
  private clouds: THREE.InstancedMesh;
  private cloudMat: THREE.MeshStandardMaterial;
  private snow: THREE.Points;
  private rain: THREE.LineSegments;
  private center = new THREE.Vector3();
  private span = [120, 80];
  private m4 = new THREE.Matrix4();
  private q = new THREE.Quaternion();
  rainAmount = 0;
  snowAmount = 0;
  /** gök kubbesi: tepe/ufuk renkleri ve güneş parıltısı (diorama.light() günceller) */
  sky: THREE.Mesh;
  skyU = { top: { value: new THREE.Color(0x6aa0d8) }, hor: { value: new THREE.Color(0xcfe4ec) }, sunDir: { value: new THREE.Vector3(0, 1, 0) }, sunCol: { value: new THREE.Color(0xffe0b0) }, glow: { value: 0.4 }, moon: { value: 0 } };

  constructor(private waterY: number) {
    // dalgalı deniz (harita çevresi) + ufka uzanan düz deniz
    const mat = new THREE.MeshStandardMaterial({ color: 0x3a78a8, roughness: 0.34, metalness: 0.08, transparent: true, opacity: 0.82, flatShading: true });
    mat.onBeforeCompile = (sh) => {
      sh.uniforms.uTime = this.uTime;
      sh.uniforms.uHor = this.uHor;
      // Fresnel: yatık bakışta deniz göğü yansıtır ve saydamlığını yitirir (su dibindeki desen görünmez, ufuk parlar)
      sh.fragmentShader = 'uniform vec3 uHor;\n' + sh.fragmentShader.replace('#include <opaque_fragment>', `
        float frz = pow(1.0 - clamp(abs(dot(normalize(vViewPosition), normal)), 0.0, 1.0), 4.0);
        outgoingLight = mix(outgoingLight, uHor * 0.92, frz * 0.6);
        diffuseColor.a = mix(diffuseColor.a, 1.0, clamp(frz * 1.6, 0.0, 1.0));
        #include <opaque_fragment>`);
      sh.vertexShader = 'uniform float uTime;\n' + sh.vertexShader.replace('#include <begin_vertex>', `#include <begin_vertex>
        vec4 wpos = modelMatrix * vec4(position, 1.0);
        transformed.z += sin(wpos.x * 0.55 + uTime * 1.1) * 0.045 + cos(wpos.z * 0.63 - uTime * 0.9) * 0.045 + sin((wpos.x + wpos.z) * 1.3 + uTime * 1.7) * 0.018;`);
    };
    this.sea = new THREE.Mesh(new THREE.PlaneGeometry(1, 1, 1, 1), mat);
    this.sea.rotation.x = -Math.PI / 2; this.sea.receiveShadow = false; // şafakta alçak güneşte gölge haritası denizde kara dikdörtgen bırakıyordu
    this.outer = new THREE.Mesh(new THREE.PlaneGeometry(4000, 4000), new THREE.MeshStandardMaterial({ color: 0x3a78a8, roughness: 0.25, metalness: 0.1 }));
    this.outer.rotation.x = -Math.PI / 2; this.outer.position.y = waterY - 0.06;
    (this.outer.material as THREE.MeshStandardMaterial).onBeforeCompile = (sh) => {
      sh.uniforms.uHor = this.uHor;
      sh.fragmentShader = 'uniform vec3 uHor;\n' + sh.fragmentShader.replace('#include <opaque_fragment>', `
        float frz = pow(1.0 - clamp(abs(dot(normalize(vViewPosition), normal)), 0.0, 1.0), 4.0);
        outgoingLight = mix(outgoingLight, uHor * 0.92, frz * 0.6);
        #include <opaque_fragment>`);
    };
    // yıldızlar
    const sp: number[] = [];
    for (let i = 0; i < 900; i++) {
      const th = rnd(i, 1) * Math.PI * 2, ph = Math.acos(rnd(i, 2) * 0.9 + 0.1);
      sp.push(Math.sin(ph) * Math.cos(th) * 500, Math.cos(ph) * 500, Math.sin(ph) * Math.sin(th) * 500);
    }
    const dot = dotTexture();
    const sg = new THREE.BufferGeometry(); sg.setAttribute('position', new THREE.Float32BufferAttribute(sp, 3));
    this.stars = new THREE.Points(sg, new THREE.PointsMaterial({ color: 0xffffff, size: 2.6, sizeAttenuation: false, map: dot, alphaTest: 0.02, transparent: true, opacity: 0, fog: false, depthWrite: false }));
    // bulutlar: 12 bulut × 5 küme
    // yumuşak gölgeli, daha yuvarlak topaklar; alt yüzler gök rengiyle aydınlanır (eskiden havada uçan gri kaya gibiydi)
    this.cloudMat = new THREE.MeshStandardMaterial({ color: 0xffffff, flatShading: false, roughness: 1, transparent: true, opacity: 0.9, emissive: new THREE.Color(0x8fa4b8), emissiveIntensity: 0.45 });
    this.clouds = new THREE.InstancedMesh(new THREE.IcosahedronGeometry(1, 1), this.cloudMat, 60);
    this.clouds.castShadow = true; this.clouds.frustumCulled = false;
    // kar
    const N = 2200, snowP = new Float32Array(N * 3);
    for (let i = 0; i < N; i++) { snowP[i * 3] = rnd(i, 3); snowP[i * 3 + 1] = rnd(i, 4); snowP[i * 3 + 2] = rnd(i, 5); }
    const snowG = new THREE.BufferGeometry(); snowG.setAttribute('position', new THREE.BufferAttribute(new Float32Array(N * 3), 3));
    snowG.userData.seed = snowP;
    this.snow = new THREE.Points(snowG, new THREE.PointsMaterial({ color: 0xffffff, size: 0.12, map: dot, alphaTest: 0.02, transparent: true, opacity: 0.9, depthWrite: false }));
    this.snow.frustumCulled = false;
    // yağmur
    const R = 1600, rainP = new Float32Array(R * 6);
    const rainG = new THREE.BufferGeometry(); rainG.setAttribute('position', new THREE.BufferAttribute(rainP, 3));
    this.rain = new THREE.LineSegments(rainG, new THREE.LineBasicMaterial({ color: 0xaac4dd, transparent: true, opacity: 0.45, depthWrite: false }));
    this.rain.frustumCulled = false;
    this.sky = new THREE.Mesh(new THREE.SphereGeometry(520, 32, 16), new THREE.ShaderMaterial({
      uniforms: this.skyU, side: THREE.BackSide, depthWrite: false, fog: false,
      vertexShader: 'varying vec3 vDir; void main(){ vDir = normalize(position); vec4 p = projectionMatrix * modelViewMatrix * vec4(position,1.0); gl_Position = p.xyww; }',
      fragmentShader: `uniform vec3 top; uniform vec3 hor; uniform vec3 sunDir; uniform vec3 sunCol; uniform float glow; uniform float moon; varying vec3 vDir;
        void main(){ float h = clamp(vDir.y, -0.2, 1.0); float k = pow(clamp(h, 0.0, 1.0), 0.55);
          vec3 c = mix(hor, top, k); if (h < 0.0) c = hor * (1.0 + h * 0.6);
          float sd = max(0.0, dot(normalize(vDir), normalize(sunDir)));
          c += sunCol * (pow(sd, 6.0) * 0.35 + pow(sd, 180.0) * 1.2) * glow * (1.0 - k * 0.5);
          // güneş diski: keskin kenarlı, parlak
          // yağmur ve bulutta söner (glow yağmurla azalır), ufkun altındayken görünmez
          c += vec3(1.0, 0.95, 0.85) * smoothstep(0.99935, 0.9997, sd) * 1.6 * pow(clamp(glow / 0.35, 0.0, 1.0), 2.0) * step(0.0, sunDir.y - 0.015);
          // ay: güneşin karşısında soluk disk, çevresinde hafif hale
          // ay ışığı +z'den gelir (diorama.light): disk de o yönde, gölgeler aydan uzağa düşer
          vec3 md3 = normalize(vec3(-sunDir.x, abs(sunDir.y) * 0.8 + 0.25, sunDir.z));
          float md = max(0.0, dot(normalize(vDir), md3));
          c += vec3(0.82, 0.88, 1.0) * (smoothstep(0.99955, 0.99975, md) * 1.1 + pow(md, 400.0) * 0.12) * moon;
          gl_FragColor = vec4(c, 1.0);
          #include <tonemapping_fragment>
          #include <colorspace_fragment>
        }`,
    }));
    this.sky.renderOrder = -10; this.sky.frustumCulled = false;
    this.group.add(this.sea, this.outer, this.stars, this.clouds, this.snow, this.rain);
  }

  /** gece deniz rengi koyulaşır ve doygunluğu düşer (ay ışığında elektrik mavisi gibi duruyordu) */
  seaTone(night: number) {
    const k = Math.min(1, Math.max(0, night)) * 0.75;
    (this.sea.material as THREE.MeshStandardMaterial).color.setHex(0x3a78a8).lerp(this.nightSea, k);
    (this.outer.material as THREE.MeshStandardMaterial).color.setHex(0x3a78a8).lerp(this.nightSea, k);
  }
  private nightSea = new THREE.Color(0x22364a);
  /** bulutların alt yüzü göğün rengini alır (gün batımında pembe-turuncu, gece lacivert) */
  cloudTint(sky: THREE.Color) { this.cloudMat.emissive.copy(sky); this.uHor.value.copy(sky); }
  private uHor = { value: new THREE.Color(0xbfd9e6) };
  cloudShadows(on: boolean) { if (this.clouds.castShadow !== on) this.clouds.castShadow = on; }
  setBounds(center: THREE.Vector3, w: number, h: number) {
    this.center.copy(center);
    this.span = [w, h];
    const W = w + 60, H = h + 60;
    this.sea.geometry.dispose();
    this.sea.geometry = new THREE.PlaneGeometry(W, H, Math.round(W / 1.2), Math.round(H / 1.2));
    this.sea.position.set(center.x, this.waterY, center.z);
    // ufka uzanan deniz: ortası delik çerçeve (içerideki dalgalı denizi örtmesin)
    const shape = new THREE.Shape([new THREE.Vector2(-2000, -2000), new THREE.Vector2(2000, -2000), new THREE.Vector2(2000, 2000), new THREE.Vector2(-2000, 2000)]);
    shape.holes.push(new THREE.Path([new THREE.Vector2(-W / 2 + 0.5, -H / 2 + 0.5), new THREE.Vector2(-W / 2 + 0.5, H / 2 - 0.5), new THREE.Vector2(W / 2 - 0.5, H / 2 - 0.5), new THREE.Vector2(W / 2 - 0.5, -H / 2 + 0.5)]));
    this.outer.geometry.dispose();
    this.outer.geometry = new THREE.ShapeGeometry(shape);
    this.outer.position.set(center.x, this.waterY - 0.01, center.z);
    this.stars.position.copy(center);
  }

  update(now: number, dt: number, night: number, target: THREE.Vector3, cam: THREE.Camera, seasonSnow: boolean, rain: boolean) {
    const t = now / 1000;
    this.uTime.value = t;
    this.sky.position.copy(cam.position);
    (this.stars.material as THREE.PointsMaterial).opacity = Math.max(0, night - 0.35) * 1.4;
    // bulutlar süzülür; kamera yukarıdaysa saydamlaşır ki haritayı kapatmasın
    const [W, H] = this.span;
    let k = 0;
    for (let c = 0; c < 12; c++) {
      const sp = 0.35 + rnd(c, 7) * 0.4;
      const x0 = this.center.x - W / 2 - 20 + (((rnd(c, 8) * (W + 40) + t * sp) % (W + 40)) + (W + 40)) % (W + 40);
      const z0 = this.center.z - H / 2 + rnd(c, 9) * H;
      const y0 = 15.5 + rnd(c, 10) * 4.5; // zirvelerin üstünde; en yüksek dağlar bulutlara değer
      // kameranın içinden geçtiği bulut: yakın plan kara, keskin üçgenler olarak görünüyordu → yaklaştıkça küçülüp kaybolur
      const cd = Math.hypot(cam.position.x - x0, cam.position.y - y0, cam.position.z - z0);
      const near = Math.min(1, Math.max(0, (cd - 5) / 5));
      if (near <= 0) continue;
      for (let p = 0; p < 5; p++) {
        const s = (1.2 + rnd(c * 5 + p, 11) * 1.4) * near;
        this.m4.compose(new THREE.Vector3(x0 + (p - 2) * 1.5 + rnd(c * 5 + p, 12), y0 + rnd(c * 5 + p, 13) * 0.6, z0 + (rnd(c * 5 + p, 14) - 0.5) * 2.2), this.q, new THREE.Vector3(s * 1.3, s * 0.7, s));
        this.clouds.setMatrixAt(k++, this.m4);
      }
    }
    this.clouds.count = k; this.clouds.instanceMatrix.needsUpdate = true;
    const camY = cam.position.y;
    this.cloudMat.opacity = camY > 24 ? 0.12 : 0.9;
    this.cloudMat.color.setScalar(rain ? 0.72 : 1 - night * 0.55);
    // kar ve yağmur: hedefin çevresindeki kutuda
    const dist = cam.position.distanceTo(target);
    const half = Math.min(70, Math.max(14, dist * 0.7)), top = Math.min(30, 8 + dist * 0.25);
    this.snowAmount += ((seasonSnow ? 1 : 0) - this.snowAmount) * Math.min(1, dt * 0.5);
    this.rainAmount += ((rain ? 1 : 0) - this.rainAmount) * Math.min(1, dt * 0.5);
    this.snow.visible = this.snowAmount > 0.02;
    if (this.snow.visible) {
      const seed = this.snow.geometry.userData.seed as Float32Array;
      const pos = this.snow.geometry.getAttribute('position') as THREE.BufferAttribute;
      const arr = pos.array as Float32Array;
      const n = Math.floor((arr.length / 3) * this.snowAmount);
      for (let i = 0; i < arr.length / 3; i++) {
        if (i >= n) { arr[i * 3 + 1] = -99; continue; }
        const fy = ((seed[i * 3 + 1] - t * (0.05 + seed[i * 3] * 0.03)) % 1 + 1) % 1;
        arr[i * 3] = target.x + (seed[i * 3] - 0.5) * 2 * half + Math.sin(t * 0.7 + i) * 0.3;
        arr[i * 3 + 1] = target.y + fy * top;
        arr[i * 3 + 2] = target.z + (seed[i * 3 + 2] - 0.5) * 2 * half;
      }
      pos.needsUpdate = true;
      (this.snow.material as THREE.PointsMaterial).size = 0.1 + dist * 0.0034;
    }
    this.rain.visible = this.rainAmount > 0.02;
    if (this.rain.visible) {
      const pos = this.rain.geometry.getAttribute('position') as THREE.BufferAttribute;
      const arr = pos.array as Float32Array;
      const n = Math.floor((arr.length / 6) * this.rainAmount);
      const len = 0.35 + dist * 0.004;
      for (let i = 0; i < arr.length / 6; i++) {
        const o = i * 6;
        if (i >= n) { arr[o + 1] = arr[o + 4] = -99; continue; }
        const fy = ((rnd(i, 21) - t * (0.9 + rnd(i, 22) * 0.3)) % 1 + 1) % 1;
        const x = target.x + (rnd(i, 23) - 0.5) * 2 * half, z = target.z + (rnd(i, 24) - 0.5) * 2 * half, y = target.y + fy * top;
        arr[o] = x; arr[o + 1] = y; arr[o + 2] = z; arr[o + 3] = x + 0.05; arr[o + 4] = y - len; arr[o + 5] = z;
      }
      pos.needsUpdate = true;
    }
  }
}
