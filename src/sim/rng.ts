// Seeded, deterministic RNG (mulberry32) + D&D dice helpers.
export class Rng {
  private s: number;
  constructor(seed: number) {
    this.s = seed >>> 0 || 1;
  }
  next(): number {
    let t = (this.s += 0x6d2b79f5);
    t = Math.imul(t ^ (t >>> 15), t | 1);
    t ^= t + Math.imul(t ^ (t >>> 7), t | 61);
    return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
  }
  int(min: number, max: number): number {
    return min + Math.floor(this.next() * (max - min + 1));
  }
  chance(p: number): boolean {
    return this.next() < p;
  }
  pick<T>(arr: readonly T[]): T {
    return arr[Math.floor(this.next() * arr.length)];
  }
  weighted<T>(items: readonly T[], w: (t: T) => number): T | undefined {
    let total = 0;
    for (const it of items) total += Math.max(0, w(it));
    if (total <= 0) return undefined;
    let r = this.next() * total;
    for (const it of items) {
      r -= Math.max(0, w(it));
      if (r <= 0) return it;
    }
    return items[items.length - 1];
  }
  shuffle<T>(arr: T[]): T[] {
    for (let i = arr.length - 1; i > 0; i--) {
      const j = Math.floor(this.next() * (i + 1));
      [arr[i], arr[j]] = [arr[j], arr[i]];
    }
    return arr;
  }
  /** roll NdS */
  dice(n: number, sides: number): number {
    let t = 0;
    for (let i = 0; i < n; i++) t += 1 + Math.floor(this.next() * sides);
    return t;
  }
  d20(): number {
    return this.dice(1, 20);
  }
  state(): number {
    return this.s;
  }
  setState(s: number) {
    this.s = s;
  }
}

export const mod = (score: number) => Math.floor((score - 10) / 2);
