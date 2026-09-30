// Türkçe özel isim ekleri (ünlü uyumu, ünsüz benzeşmesi, kaynaştırma).
export type Ek = 'i' | 'a' | 'da' | 'dan' | 'in';

const BACK = 'aıou', FRONT = 'eiöü', VOWELS = 'aıoueiöüâîû';
const HARD = 'fstkçşhp';
const NUM: Record<number, string> = { 10: 'on', 25: 'yirmi beş', 50: 'elli', 100: 'yüz', 150: 'yüz elli', 200: 'iki yüz' };

export function ek(word: string | number, kind: Ek): string {
  const w = String(word);
  const base = typeof word === 'number' ? (NUM[word] ?? w) : w;
  const lower = base.toLocaleLowerCase('tr');
  let last = '';
  for (let i = lower.length - 1; i >= 0; i--) if (VOWELS.includes(lower[i])) { last = lower[i]; break; }
  const back = BACK.includes(last) || last === 'â' || last === 'û';
  const round = 'ouöüû'.includes(last);
  const endCh = lower[lower.length - 1];
  const endsVowel = VOWELS.includes(endCh);
  const hard = HARD.includes(endCh);
  // "Aslanburç Krallığı", "Kırıkdiş Kampı" gibi tamlamalar n kaynaştırması alır
  const compound = /\s/.test(base.trim()) && 'ıiuü'.includes(endCh);
  const buf = endsVowel ? (compound ? 'n' : kind === 'in' ? 'n' : 'y') : '';
  const i4 = back ? (round ? 'u' : 'ı') : round ? 'ü' : 'i';
  const a2 = back ? 'a' : 'e';
  const d = hard ? 't' : 'd';
  let s = '';
  switch (kind) {
    case 'i': s = buf + i4; break;
    case 'a': s = buf + a2; break;
    case 'da': s = (compound && endsVowel ? 'n' + 'd' : d) + a2; break;
    case 'dan': s = (compound && endsVowel ? 'n' + 'd' : d) + a2 + 'n'; break;
    case 'in': s = buf + i4 + 'n'; break;
  }
  return `${w}'${s}`;
}
