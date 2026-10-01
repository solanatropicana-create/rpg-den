#!/usr/bin/env python3
"""WorldGen port check: compares the TS and C# world dumps (one JSON document per line, same seed order).

Rules: an object key whose value is null counts as absent (TS undefined/null vs C# omitted null);
numbers must be exactly equal as doubles; booleans are not numbers; object key order is ignored;
array order and length matter. Exits 1 on any mismatch.

Usage: python3 worldgen_compare.py ts.ndjson cs.ndjson
"""
import json
import sys

LIMIT = 20  # mismatches reported per seed


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
    ts_path, cs_path = sys.argv[1], sys.argv[2]
    with open(ts_path, encoding='utf-8') as f:
        ts_lines = [l for l in f.read().split('\n') if l.strip()]
    with open(cs_path, encoding='utf-8') as f:
        cs_lines = [l for l in f.read().split('\n') if l.strip()]
    if len(ts_lines) != len(cs_lines):
        print(f'document count differs: TS {len(ts_lines)} vs C# {len(cs_lines)}')
    bad = 0
    for k, (lt, lc) in enumerate(zip(ts_lines, cs_lines)):
        a, b = norm(json.loads(lt)), norm(json.loads(lc))
        out = []
        diff(a, b, '$', out)
        info = (f"tiles {len(a.get('tiles', []))} deposits {len(a.get('deposits', []))} civs {len(a.get('civs', []))} "
                f"camps {len(a.get('camps', []))} isles {len(a.get('isles', []))} sea {a.get('seaProfile')} "
                f"passes {a.get('metrics', {}).get('mountainPass')} nextId {a.get('nextId')} rngState {a.get('rngState')}")
        if out:
            bad += 1
            print(f"seed {a.get('seed')}: MISMATCH ({info})")
            for p, x, y in out:
                print(f'   {p}: TS {short(x)} | C# {short(y)}')
        else:
            print(f"seed {a.get('seed')}: identical ({info})")
    n = min(len(ts_lines), len(cs_lines))
    print(f'{n - bad}/{n} seeds identical')
    sys.exit(1 if bad or len(ts_lines) != len(cs_lines) else 0)


if __name__ == '__main__':
    main()
