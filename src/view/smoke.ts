// Yumuşak duman: kenarı sönen, kameraya dönük noktalar (her karede yeniden doldurulur).
// Eskiden duman düz gölgeli ikosahedronlardı ve uzaktan havada asılı gri kaya gibi görünüyordu.
import * as THREE from 'three';

const MAX = 6000;

export class SmokeSystem {
  points: THREE.Points;
  private pos = new Float32Array(MAX * 3);
  private col = new Float32Array(MAX * 4);
  private size = new Float32Array(MAX);
  private n = 0;
  private c = new THREE.Color();
  uniforms: { scale: { value: number }; light: { value: number } } & Record<string, THREE.IUniform>;

  constructor() {
    const g = new THREE.BufferGeometry();
    g.setAttribute('position', new THREE.BufferAttribute(this.pos, 3).setUsage(THREE.DynamicDrawUsage));
    g.setAttribute('col', new THREE.BufferAttribute(this.col, 4).setUsage(THREE.DynamicDrawUsage));
    g.setAttribute('size', new THREE.BufferAttribute(this.size, 1).setUsage(THREE.DynamicDrawUsage));
    g.setDrawRange(0, 0);
    this.uniforms = THREE.UniformsUtils.merge([THREE.UniformsLib.fog, { scale: { value: 600 }, light: { value: 1 } }]) as SmokeSystem['uniforms'];
    const m = new THREE.ShaderMaterial({
      uniforms: this.uniforms,
      transparent: true,
      depthWrite: false,
      fog: true,
      vertexShader: `
        attribute float size; attribute vec4 col; varying vec4 vCol; uniform float scale;
        #include <fog_pars_vertex>
        void main() {
          vec4 mvPosition = modelViewMatrix * vec4(position, 1.0);
          // kameranın dibindeki duman görüşü kapatmasın
          vCol = vec4(col.rgb, col.a * smoothstep(1.2, 4.5, -mvPosition.z));
          gl_PointSize = size * scale / max(0.1, -mvPosition.z);
          gl_Position = projectionMatrix * mvPosition;
          #include <fog_vertex>
        }`,
      fragmentShader: `
        varying vec4 vCol; uniform float light;
        #include <fog_pars_fragment>
        void main() {
          vec2 q = gl_PointCoord - 0.5;
          float d = length(q) * 2.0;
          if (d > 1.0) discard;
          float a = vCol.a * smoothstep(1.0, 0.3, d);
          // üst-sol kenar hafif aydınlık: topak hissi
          float lit = 0.88 + 0.22 * clamp(-q.x - q.y, -0.5, 0.5);
          gl_FragColor = vec4(vCol.rgb * light * lit, a);
          #include <tonemapping_fragment>
          #include <colorspace_fragment>
          #include <fog_fragment>
        }`,
    });
    this.points = new THREE.Points(g, m);
    this.points.frustumCulled = false;
    this.points.renderOrder = 5;
  }
  begin() { this.n = 0; }
  /** d: dünya birimi cinsinden çap */
  add(x: number, y: number, z: number, d: number, hex: number, alpha: number) {
    if (this.n >= MAX || d <= 0 || alpha <= 0) return;
    const i = this.n++;
    this.pos[i * 3] = x; this.pos[i * 3 + 1] = y; this.pos[i * 3 + 2] = z;
    this.c.setHex(hex);
    this.col[i * 4] = this.c.r; this.col[i * 4 + 1] = this.c.g; this.col[i * 4 + 2] = this.c.b; this.col[i * 4 + 3] = alpha;
    this.size[i] = d;
  }
  end(cam: THREE.PerspectiveCamera, viewportH: number, light: number) {
    const g = this.points.geometry;
    g.setDrawRange(0, this.n);
    for (const k of ['position', 'col', 'size']) { const a = g.getAttribute(k) as THREE.BufferAttribute; a.clearUpdateRanges(); a.addUpdateRange(0, this.n * a.itemSize); a.needsUpdate = true; }
    this.uniforms.scale.value = viewportH / (2 * Math.tan((cam.fov * Math.PI) / 360));
    this.uniforms.light.value = light;
  }
}
