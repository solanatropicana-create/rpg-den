#!/usr/bin/env python3
"""Diplomacy / Monsters port check: compares the case lines of golden/ts/combat_modcheck.ts (TS) with
tests/CombatModCheck (C#), in order. C# cases marked `skipped` (they reached a module that still throws
NotImplementedException) are counted, not compared. Same diff rules as ../CombatCheck/combat_compare.py
(null == absent, exact doubles, key order ignored). Exits 1 on any mismatch.

Usage: python3 combat_modcompare.py ts.ndjson cs.ndjson
"""
import collections
import json
import os
import sys

sys.path.insert(0, os.path.join(os.path.dirname(os.path.abspath(__file__)), '..', 'CombatCheck'))
from combat_compare import norm, diff, short  # noqa: E402


def cases(path):
    with open(path, encoding='utf-8') as f:
        for line in f:
            d = json.loads(line)
            if d.get('kind') == 'case':
                yield d


def main():
    if len(sys.argv) != 3:
        print(__doc__)
        return 2
    ok = collections.Counter()
    skipped = collections.Counter()
    bad = collections.Counter()
    shown = 0
    for a, b in zip(cases(sys.argv[1]), cases(sys.argv[2])):
        op = a.get('tag', '') + '/' + a['name'].split(':')[0]
        if (a['day'], a.get('tag'), a['name']) != (b['day'], b.get('tag'), b['name']):
            print(f'case order mismatch: {a["day"]}/{a.get("tag")}/{a["name"]} vs {b["day"]}/{b.get("tag")}/{b["name"]}')
            return 1
        if b.get('skipped'):
            skipped[op] += 1
            continue
        out = []
        for k in ('err', 'rng', 'extra', 'campTiles', 'w'):
            diff(norm(a.get(k)), norm(b.get(k)), k, out)
        if out:
            bad[op] += 1
            if shown < 12:
                shown += 1
                print(f'day {a["day"]} {a.get("tag")} {a["name"]}: {len(out)} mismatches')
                for p, x, y in out:
                    print(f'  {p}: ts={short(x)} cs={short(y)}')
        else:
            ok[op] += 1
    print('ok      ', dict(ok))
    print('skipped ', dict(skipped))
    print('mismatch', dict(bad))
    return 1 if bad else 0


if __name__ == '__main__':
    sys.exit(main())
