#!/usr/bin/env python3
"""Sea port check: compares golden/ts/sea_check.ts (TS) with tests/SeaCheck (C#), line by line.

'snap' lines must agree on seed/day/tag; 'q' lines on op, result `r` and the cache sizes after the query (`n` nav
cache, `pn` land path cache); 'case' lines on rng state, `extra`, whether an error was thrown, nav cache size and the
post-state `w` (+ `tiles`: [index, tile] of tiles with an ext or a camp, for ticks that can found coves or burn farms). C# lines marked `skipped` (a module still throwing NotImplementedException) are counted, not compared.
Same diff rules as ../CombatCheck/combat_compare.py (null == absent, exact doubles, key order ignored).
Exits 1 on any mismatch.

Usage: python3 sea_compare.py ts.ndjson cs.ndjson
"""
import collections
import json
import os
import sys

sys.path.insert(0, os.path.join(os.path.dirname(os.path.abspath(__file__)), '..', 'CombatCheck'))
from combat_compare import norm, diff, short  # noqa: E402


def main():
    if len(sys.argv) != 3:
        print(__doc__)
        return 2
    ok = collections.Counter()
    skipped = collections.Counter()
    bad = collections.Counter()
    shown = 0
    snap = None
    with open(sys.argv[1], encoding='utf-8') as fa, open(sys.argv[2], encoding='utf-8') as fb:
        for n, (la, lb) in enumerate(zip(fa, fb)):
            a, b = json.loads(la), json.loads(lb)
            if a['kind'] != b['kind']:
                print(f'line {n}: kind mismatch {a["kind"]} vs {b["kind"]}')
                return 1
            if a['kind'] == 'snap':
                if (a['seed'], a['day'], a['tag']) != (b['seed'], b['day'], b['tag']):
                    print(f'line {n}: snapshot mismatch')
                    return 1
                snap = f'seed {a["seed"]} day {a["day"]} {a["tag"]}'
                continue
            if a['kind'] == 'q':
                op = 'q/' + a['op']
                keys = ('r', 'n', 'pn')
                label = f'{snap} q{a["i"]} {a["op"]}'
            else:
                op = f'{a["tag"]}/case/' + a['name'].split(':')[0]
                keys = ('rng', 'extra', 'n', 'tiles', 'w')
                label = f'{snap} case {a["name"]}'
                if a['name'] != b['name']:
                    print(f'line {n}: case order mismatch {a["name"]} vs {b["name"]}')
                    return 1
            if b.get('skipped'):
                skipped[op] += 1
                continue
            out = []
            if bool(a.get('err')) != bool(b.get('err')):
                out.append(('err', a.get('err'), b.get('err')))
            for k in keys:
                if a['kind'] == 'q' and k == 'pn' and 'pn' not in a:
                    continue
                if a['kind'] == 'q' and k in ('n', 'pn') and k not in a:
                    continue
                diff(norm(a.get(k)), norm(b.get(k)), k, out)
            if out:
                bad[op] += 1
                if shown < 15:
                    shown += 1
                    print(f'{label}: {len(out)} mismatches')
                    if a['kind'] == 'q':
                        params = {k: v for k, v in a.items() if k not in ('kind', 'i', 'op', 'r', 'n', 'pn')}
                        print(f'  params: {short(params)}')
                    for p, x, y in out:
                        print(f'  {p}: ts={short(x)} cs={short(y)}')
            else:
                ok[op] += 1
    print('ok      ', dict(sorted(ok.items())))
    print('skipped ', dict(sorted(skipped.items())))
    print('mismatch', dict(sorted(bad.items())))
    return 1 if bad else 0


if __name__ == '__main__':
    sys.exit(main())
