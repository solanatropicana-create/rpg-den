#!/usr/bin/env python3
"""Compares econ_diff (TS) and EconCheck (C#) NDJSON dumps day by day.
null == absent; numbers compared as exact doubles; key ORDER is compared for JS-record fields
(stock, price, pop, jobs, ...) and ignored for class-typed objects. Prints the first differences."""
import json, sys

ORDERED = {'stock', 'price', 'want', 'eff', 'yearly', 'mined', 'pop', 'civics', 'workshops', 'jobs',
           'mixedSince', 'tension', 'metrics', 'cargo', 'tally', 'rep', 'tabs', 'ext_map'}
NAMED = {'NaN', 'Infinity', '-Infinity'}

def clean(d):
    return [(k, v) for k, v in d.items() if v is not None and not (isinstance(v, str) and v in NAMED)]

def diff(a, b, path, key=None, out=None, limit=12):
    if len(out) >= limit: return
    if isinstance(a, dict) and isinstance(b, dict):
        ia, ib = clean(a), clean(b)
        ka, kb = [k for k, _ in ia], [k for k, _ in ib]
        if set(ka) != set(kb):
            out.append(f'{path}: keys TS-only {sorted(set(ka) - set(kb))} C#-only {sorted(set(kb) - set(ka))}')
            return
        if key in ORDERED and ka != kb:
            out.append(f'{path}: key order TS {ka} vs C# {kb}')
        da, db = dict(ia), dict(ib)
        for k in ka: diff(da[k], db[k], f'{path}.{k}', k, out, limit)
        return
    if isinstance(a, list) and isinstance(b, list):
        if len(a) != len(b):
            out.append(f'{path}: length TS {len(a)} vs C# {len(b)}')
        for i, (x, y) in enumerate(zip(a, b)): diff(x, y, f'{path}[{i}]', key, out, limit)
        return
    na = isinstance(a, (int, float)) and not isinstance(a, bool)
    nb = isinstance(b, (int, float)) and not isinstance(b, bool)
    if na and nb:
        if float(a) != float(b): out.append(f'{path}: TS {a!r} vs C# {b!r}')
        return
    if a != b: out.append(f'{path}: TS {a!r} vs C# {b!r}')

def main():
    if sys.argv[1] == '--run':
        # econ_compare.py --run <seed> <days> [scen] [tilesEvery]: spawn both harnesses (run from macro/)
        import subprocess
        rest = sys.argv[2:]
        pa = subprocess.Popen(['node', 'golden/out/econ_diff.js'] + rest, stdout=subprocess.PIPE, text=True, encoding='utf-8')
        pb = subprocess.Popen(['dotnet', 'tests/EconCheck/bin/Release/net8.0/EconCheck.dll'] + rest, stdout=subprocess.PIPE, text=True, encoding='utf-8')
        fa, fb = pa.stdout, pb.stdout
    else:
        fa, fb = open(sys.argv[1], encoding='utf-8'), open(sys.argv[2], encoding='utf-8')
    n = 0
    for la, lb in zip(fa, fb):
        a, b = json.loads(la), json.loads(lb)
        if 'ext' in a and a['ext'] is not None: a['ext_map'] = a.pop('ext')
        if 'ext' in b and b['ext'] is not None: b['ext_map'] = b.pop('ext')
        out = []
        diff(a, b, f"day{a.get('day')}", None, out)
        if out:
            print(f"FIRST DIFFERENCE on day {a.get('day')} (C# day {b.get('day')}):")
            for x in out: print('  ' + x)
            return 1
        n += 1
    rest_a, rest_b = fa.readline(), fb.readline()
    print(f'{n} day dumps identical' + (' (TS has more lines)' if rest_a else '') + (' (C# has more lines)' if rest_b else ''))
    return 0

sys.exit(main())
