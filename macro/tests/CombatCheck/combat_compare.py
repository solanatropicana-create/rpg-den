#!/usr/bin/env python3
"""Combat port check: compares the `out` of every case of the TS dump (golden/ts/combat_check.ts, lines
{k, spec, out}) with the C# dump (tests/CombatCheck, lines {k, out}), matched by line order and k.

Rules (same as worldgen_compare.py): an object key whose value is null counts as absent (TS undefined vs C#
omitted null); numbers must be exactly equal as doubles (NaN/Infinity are written as strings by both sides);
booleans are not numbers; object key order is ignored; array order and length matter. Exits 1 on any mismatch.

Usage: python3 combat_compare.py ts.ndjson cs.ndjson
"""
import json
import sys

LIMIT = 12  # mismatches reported per case


def norm(v):
    if isinstance(v, dict):
        return {k: norm(x) for k, x in v.items() if x is not None}
    if isinstance(v, list):
        return [norm(x) for x in v]
    return v


def short(v):
    s = json.dumps(v, ensure_ascii=False)
    return s if len(s) <= 160 else s[:157] + '...'


def diff(a, b, path, out):
    if len(out) >= LIMIT:
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
                diff(a[k], b[k], f'{path}.{k}', out)
            if len(out) >= LIMIT:
                return
        return
    if isinstance(a, list) and isinstance(b, list):
        if len(a) != len(b):
            out.append((f'{path}.length', len(a), len(b)))
        for i in range(min(len(a), len(b))):
            diff(a[i], b[i], f'{path}[{i}]', out)
            if len(out) >= LIMIT:
                return
        return
    if type(a) is not type(b) or a != b:
        out.append((path, a, b))


def main():
    if len(sys.argv) != 3:
        print(__doc__)
        return 2
    bad = cases = 0
    with open(sys.argv[1], encoding='utf-8') as fa, open(sys.argv[2], encoding='utf-8') as fb:
        for n, (la, lb) in enumerate(zip(fa, fb)):
            ta, tb = json.loads(la), json.loads(lb)
            cases += 1
            if ta['k'] != tb['k']:
                print(f'line {n}: k mismatch {ta["k"]!r} vs {tb["k"]!r}')
                return 1
            out = []
            diff(norm(ta['out']), norm(tb['out']), 'out', out)
            if out:
                bad += 1
                if bad <= 10:
                    print(f'case k={ta["k"]}: {len(out)}{"+" if len(out) >= LIMIT else ""} mismatches')
                    for p, x, y in out:
                        print(f'  {p}: ts={short(x)} cs={short(y)}')
        rest_a, rest_b = fa.read().strip(), fb.read().strip()
        if rest_a or rest_b:
            print(f'line count differs after {cases} lines (ts extra: {bool(rest_a)}, cs extra: {bool(rest_b)})')
            return 1
    print(f'{cases} cases compared, {bad} with mismatches')
    return 1 if bad else 0


if __name__ == '__main__':
    sys.exit(main())
