#!/usr/bin/env python3
"""Heroes / Will / Inns port check: compares the per-day dumps of golden/ts/heroes_sim.ts (TS) and
tests/HeroesCheck (C#) line by line and reports the first differing day(s).

Rules (like WorldGenCheck): a key whose value is null counts as absent (TS undefined vs C# omitted null),
'_'-prefixed keys are skipped (TS debug fields such as battle._def, like the golden canonical form);
numbers must be exactly equal as doubles; booleans are not numbers; object key order is ignored;
array order and length matter. Exits 1 on any mismatch.

Usage: python3 heroes_compare.py ts.ndjson cs.ndjson [maxDays=3] [limit=25]
"""
import json
import sys


def norm(v):
    if isinstance(v, dict):
        return {k: norm(x) for k, x in v.items() if x is not None and not k.startswith('_')}
    if isinstance(v, list):
        return [norm(x) for x in v]
    return v


def short(v):
    s = json.dumps(v, ensure_ascii=False)
    return s if len(s) <= 200 else s[:197] + '...'


def diff(a, b, path, out, limit):
    if len(out) >= limit:
        return
    if isinstance(a, bool) or isinstance(b, bool):
        if not (isinstance(a, bool) and isinstance(b, bool) and a == b):
            out.append((path, a, b))
        return
    if isinstance(a, (int, float)) and isinstance(b, (int, float)):
        if float(a) != float(b):
            out.append((path, a, b))
        return
    if isinstance(a, dict) and isinstance(b, dict):
        for k in sorted(set(a) | set(b)):
            if k not in a:
                out.append((f'{path}.{k}', '<absent>', b[k]))
            elif k not in b:
                out.append((f'{path}.{k}', a[k], '<absent>'))
            else:
                diff(a[k], b[k], f'{path}.{k}', out, limit)
            if len(out) >= limit:
                return
        return
    if isinstance(a, list) and isinstance(b, list):
        if len(a) != len(b):
            out.append((f'{path}.length', len(a), len(b)))
        for i in range(min(len(a), len(b))):
            diff(a[i], b[i], f'{path}[{i}]', out, limit)
            if len(out) >= limit:
                return
        return
    if type(a) is not type(b) or a != b:
        out.append((path, a, b))


def main():
    ts_path, cs_path = sys.argv[1], sys.argv[2]
    max_days = int(sys.argv[3]) if len(sys.argv) > 3 else 3
    limit = int(sys.argv[4]) if len(sys.argv) > 4 else 25
    bad = 0
    n = 0
    with open(ts_path, encoding='utf-8') as ft, open(cs_path, encoding='utf-8') as fc:
        while True:
            lt, lc = ft.readline(), fc.readline()
            if not lt or not lc:
                if lt or lc:
                    print(f'line count differs after {n} lines (TS {"more" if lt else "ended"}, C# {"more" if lc else "ended"})')
                    bad += 1
                break
            n += 1
            a, b = norm(json.loads(lt)), norm(json.loads(lc))
            out = []
            diff(a, b, '$', out, limit)
            if out:
                bad += 1
                print(f"day {a.get('day')}: MISMATCH (rng TS {a.get('rng')} C# {b.get('rng')}, nextId TS {a.get('nextId')} C# {b.get('nextId')})")
                for p, x, y in out:
                    print(f'   {p}: TS {short(x)} | C# {short(y)}')
                if bad >= max_days:
                    break
    print(f'{n} lines compared, {bad} differing' + (' (stopped early)' if bad >= max_days else ''))
    sys.exit(1 if bad else 0)


if __name__ == '__main__':
    main()
