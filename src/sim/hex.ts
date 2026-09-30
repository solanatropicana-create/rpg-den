// Pointy-top hex grid, odd-r offset storage. Index = row * W + col.
export interface Cube { x: number; y: number; z: number }

export class HexGrid {
  constructor(public W: number, public H: number) {}
  idx(col: number, row: number) { return row * this.W + col; }
  col(i: number) { return i % this.W; }
  row(i: number) { return Math.floor(i / this.W); }
  inside(col: number, row: number) { return col >= 0 && row >= 0 && col < this.W && row < this.H; }
  get size() { return this.W * this.H; }

  toCube(i: number): Cube {
    const c = this.col(i), r = this.row(i);
    const x = c - (r - (r & 1)) / 2;
    const z = r;
    return { x, y: -x - z, z };
  }
  dist(a: number, b: number): number {
    const A = this.toCube(a), B = this.toCube(b);
    return Math.max(Math.abs(A.x - B.x), Math.abs(A.y - B.y), Math.abs(A.z - B.z));
  }
  neighbors(i: number): number[] {
    const c = this.col(i), r = this.row(i);
    const odd = r & 1;
    const d = odd
      ? [[1, 0], [1, -1], [0, -1], [-1, 0], [0, 1], [1, 1]]
      : [[1, 0], [0, -1], [-1, -1], [-1, 0], [-1, 1], [0, 1]];
    const out: number[] = [];
    for (const [dc, dr] of d) {
      const nc = c + dc, nr = r + dr;
      if (this.inside(nc, nr)) out.push(this.idx(nc, nr));
    }
    return out;
  }
  within(i: number, radius: number): number[] {
    const out: number[] = [];
    const c = this.col(i), r = this.row(i);
    for (let rr = r - radius; rr <= r + radius; rr++)
      for (let cc = c - radius - 1; cc <= c + radius + 1; cc++) {
        if (!this.inside(cc, rr)) continue;
        const j = this.idx(cc, rr);
        if (this.dist(i, j) <= radius) out.push(j);
      }
    return out;
  }
  /** pixel center for rendering, pointy-top, size = hex radius */
  pixel(i: number, size: number): [number, number] {
    const c = this.col(i), r = this.row(i);
    return [size * Math.sqrt(3) * (c + 0.5 * (r & 1)), size * 1.5 * r];
  }
}

/** A* over the hex grid. cost(i) returns Infinity for impassable. */
export function findPath(g: HexGrid, from: number, to: number, cost: (i: number) => number): number[] | null {
  if (from === to) return [from];
  const open = new MinHeap();
  const gScore = new Map<number, number>();
  const came = new Map<number, number>();
  gScore.set(from, 0);
  open.push(from, g.dist(from, to));
  const closed = new Set<number>();
  while (open.size) {
    const cur = open.pop()!;
    if (cur === to) {
      const path = [cur];
      let c = cur;
      while (came.has(c)) { c = came.get(c)!; path.push(c); }
      return path.reverse();
    }
    if (closed.has(cur)) continue;
    closed.add(cur);
    for (const n of g.neighbors(cur)) {
      const step = n === to ? Math.min(cost(n), 3) : cost(n);
      if (!isFinite(step)) continue;
      const t = gScore.get(cur)! + step;
      if (t < (gScore.get(n) ?? Infinity)) {
        gScore.set(n, t);
        came.set(n, cur);
        open.push(n, t + g.dist(n, to) * 0.5);
      }
    }
  }
  return null;
}

class MinHeap {
  private a: number[] = [];
  private p: number[] = [];
  get size() { return this.a.length; }
  push(v: number, pr: number) {
    this.a.push(v); this.p.push(pr);
    let i = this.a.length - 1;
    while (i > 0) {
      const par = (i - 1) >> 1;
      if (this.p[par] <= this.p[i]) break;
      this.swap(i, par); i = par;
    }
  }
  pop(): number | undefined {
    if (!this.a.length) return undefined;
    const top = this.a[0];
    const lv = this.a.pop()!, lp = this.p.pop()!;
    if (this.a.length) {
      this.a[0] = lv; this.p[0] = lp;
      let i = 0;
      for (;;) {
        const l = 2 * i + 1, r = l + 1;
        let m = i;
        if (l < this.a.length && this.p[l] < this.p[m]) m = l;
        if (r < this.a.length && this.p[r] < this.p[m]) m = r;
        if (m === i) break;
        this.swap(i, m); i = m;
      }
    }
    return top;
  }
  private swap(i: number, j: number) {
    [this.a[i], this.a[j]] = [this.a[j], this.a[i]];
    [this.p[i], this.p[j]] = [this.p[j], this.p[i]];
  }
}
