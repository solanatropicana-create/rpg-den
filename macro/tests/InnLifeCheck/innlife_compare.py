#!/usr/bin/env python3
"""InnLife port check: compares golden/ts/innlife_world.ts (TS) and tests/InnLifeCheck `world` (C#) NDJSON dumps
day by day (or innlife_unit outputs line by line). null == absent; the strings "NaN"/"Infinity"/"-Infinity" (C#)
count as absent too (JSON.stringify writes null); numbers are compared as exact doubles; key ORDER is compared for
JS-record fields (stock, pop, cargo, tabs, metrics, ...) and ignored for class-typed objects. Prints the first
differing day (up to LIMIT differences) and exits 1, or prints a summary and exits 0.

Usage: python3 innlife_compare.py ts.ndjson cs.ndjson
"""
import json
import sys

ORDERED = {'stock', 'price', 'want', 'eff', 'yearly', 'mined', 'pop', 'civics', 'workshops', 'jobs',
           'mixedSince', 'tension', 'metrics', 'cargo', 'tally', 'rep', 'tabs'}
NAMED = {'NaN', 'Infinity', '-Infinity'}
LIMIT = 25


def clean(d):
    return [(k, v) for k, v in d.items() if v is not None and not (isinstance(v, str) and v in NAMED)]


def short(v):
    s = json.dumps(v, ensure_ascii=False)
    return s if len(s) <= 200 else s[:197] + '...'


def diff(a, b, path, key, out):
    if len(out) >= LIMIT:
        return
    if isinstance(a, dict) and isinstance(b, dict):
        ia, ib = clean(a), clean(b)
        ka, kb = [k for k, _ in ia], [k for k, _ in ib]
        if set(ka) != set(kb):
            out.append(f'{path}: keys TS-only {sorted(set(ka) - set(kb))} C#-only {sorted(set(kb) - set(ka))}')
        if key in ORDERED and ka != kb and set(ka) == set(kb):
            out.append(f'{path}: key order TS {ka} vs C# {kb}')
        da, db = dict(ia), dict(ib)
        for k in ka:
            if k in db:
                diff(da[k], db[k], f'{path}.{k}', k, out)
        return
    if isinstance(a, list) and isinstance(b, list):
        if len(a) != len(b):
            out.append(f'{path}: length TS {len(a)} vs C# {len(b)}')
        for i, (x, y) in enumerate(zip(a, b)):
            diff(x, y, f'{path}[{i}]', key, out)
        return
    na = isinstance(a, (int, float)) and not isinstance(a, bool)
    nb = isinstance(b, (int, float)) and not isinstance(b, bool)
    if na and nb:
        if float(a) != float(b):
            out.append(f'{path}: TS {a!r} vs C# {b!r}')
        return
    if a != b:
        out.append(f'{path}: TS {short(a)} vs C# {short(b)}')


def main():
    fa, fb = open(sys.argv[1], encoding='utf-8'), open(sys.argv[2], encoding='utf-8')
    n = 0
    stats = {}
    for la, lb in zip(fa, fb):
        a, b = json.loads(la), json.loads(lb)
        if 'inn' in a and 'inn' not in b:
            del a['inn']   # innlife_unit: the TS line carries its input inn, the C# line only the results
        if 'error' in a or 'error' in b:
            print(f"day {a.get('day')}: TS error {a.get('error')!r} | C# error {b.get('error')!r}")
            if b.get('stack'):
                print(b['stack'])
            if 'error' in a and 'error' in b:
                print(f'{n} documents identical before both sides stopped')
                return 0
            print(f'{n} documents identical before one side stopped')
            return 2
        out = []
        diff(a, b, f"day{a.get('day', n)}", None, out)
        if out:
            print(f"FIRST DIFFERENCE in document {n} (day TS {a.get('day')} / C# {b.get('day')}):")
            for x in out:
                print('  ' + x)
            return 1
        n += 1
        if 'metrics' in a:
            stats = a['metrics']
    rest_a, rest_b = fa.read(), fb.read()
    if rest_a.strip() or rest_b.strip():
        print(f'document count differs after {n} documents (TS extra: {bool(rest_a.strip())}, C# extra: {bool(rest_b.strip())})')
        return 1
    keys = [k for k in sorted(stats) if k.startswith('inn') or k in ('ev_inn', 'ev_migration')]
    print(f'{n} documents identical; final metrics: ' + ', '.join(f'{k}={stats[k]}' for k in keys))
    return 0


if __name__ == '__main__':
    sys.exit(main())
