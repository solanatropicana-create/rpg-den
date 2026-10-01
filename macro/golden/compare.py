#!/usr/bin/env python3
"""Golden test: runs the TypeScript simulation (golden/out/trace.js) and the C# port (FD.Macro.Run) side by side and
pinpoints the first divergence (see golden/README.md).

  python3 golden/compare.py --seeds 1,2,3 --days 7200 [--jobs N]

Per seed: both `hash` runs stream one line per day and are compared as they go; on the first differing day the
script runs `cps` (checkpoints of that day) on both sides -> first differing checkpoint; `dump` at that checkpoint
and at the previous one -> differing state paths with both values; `rng` for that day -> first divergent RNG call
with context and both callers. A C# (or TS) crash is reported as such, after checking the checkpoints before it.
Exit code 0 only if every seed matches on every day.
"""
import argparse
import os
import queue
import re
import subprocess
import sys
import tempfile
import threading
import time
from concurrent.futures import ThreadPoolExecutor

MACRO = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
CS_DLL = os.path.join('FD.Macro.Run', 'bin', 'Release', 'net8.0', 'FD.Macro.Run.dll')
SIDES = {'TS': ['node', os.path.join('golden', 'out', 'trace.js')], 'C#': ['dotnet', CS_DLL]}
HASH_COLS = ['day', 'hash', 'rngState', 'rngCalls', 'pathCache', 'navCache']
CP_COLS = ['label', 'hash', 'rngState', 'rngCalls']
CRASH_RE = re.compile(r'^CRASH (\d+) (\S+) (.*)$', re.M)
ARGS = None
PRINT_LOCK = threading.Lock()


def say(*lines):
    with PRINT_LOCK:
        for line in lines:
            print(line)
        sys.stdout.flush()


# ---------------------------------------------------------------------------------------------------- processes
def env_for(side):
    env = dict(os.environ, DOTNET_NOLOGO='1', DOTNET_CLI_TELEMETRY_OPTOUT='1')
    env.pop('GOLDEN_FAULT', None)
    if side == 'C#' and ARGS.fault:
        env['GOLDEN_FAULT'] = ARGS.fault
    return env


class Proc:
    """One harness process: stdout streamed as text lines, stderr collected in a temp file."""

    def __init__(self, side, args, threaded=False):
        self.side = side
        self.t0 = time.time()
        self.t1 = None
        self.err = tempfile.TemporaryFile(mode='w+b')
        self.p = subprocess.Popen(SIDES[side] + [str(a) for a in args], cwd=MACRO, env=env_for(side),
                                  stdout=subprocess.PIPE, stderr=self.err, stdin=subprocess.DEVNULL)
        self.out = (line.decode('utf-8', 'replace').rstrip('\n') for line in self.p.stdout)
        self.q = None
        if threaded:        # drain stdout on a thread so this side never waits for the other one
            self.q = queue.Queue()
            threading.Thread(target=self._drain, daemon=True).start()

    def _drain(self):
        try:
            for line in self.out:
                self.q.put(line)
        except (OSError, ValueError):
            pass
        self.t1 = time.time()
        self.q.put(None)

    def next(self):
        return self.q.get() if self.q else next(self.out, None)

    def finish(self, kill=False):
        """Waits for (or kills) the process; returns (exit code, stderr text, seconds until its output ended)."""
        if kill and self.p.poll() is None:
            self.p.kill()
        rc = self.p.wait()
        if not self.q:
            self.p.stdout.close()
        secs = (self.t1 or time.time()) - self.t0
        self.err.seek(0)
        err = self.err.read().decode('utf-8', 'replace')
        self.err.close()
        return rc, err, secs


def run(side, args):
    """Runs a harness mode to completion: (stdout lines, exit code, stderr)."""
    pr = Proc(side, args)
    lines = list(pr.out)
    rc, err, _ = pr.finish()
    return lines, rc, err


def run_both(args_ts, args_cs=None):
    """Runs the same mode on both sides concurrently: {'TS': (lines, rc, err), 'C#': (...)}."""
    res = {}
    threads = [threading.Thread(target=lambda s=s, a=a: res.__setitem__(s, run(s, a)))
               for s, a in (('TS', args_ts), ('C#', args_cs or args_ts))]
    for t in threads:
        t.start()
    for t in threads:
        t.join()
    return res


def crash_of(err):
    """(day, last checkpoint, 'Type: message', top stack frames) from a harness's stderr, or None."""
    m = CRASH_RE.search(err or '')
    if not m:
        return None
    frames = [ln.strip() for ln in err[m.end():].splitlines() if ln.strip().startswith('at ')]
    sim_frames = [f for f in frames if 'FD.Macro.' in f and 'FD.Macro.Run' not in f] or frames
    return int(m.group(1)), m.group(2), m.group(3), sim_frames[:4]


def frames_text(frames, n=3):
    """'at FD.Macro.Heroes.TavernsTick(Sim s) in /x/Heroes.cs:line 12' -> 'Heroes.TavernsTick(Sim s) (Heroes.cs:12)'."""
    out = []
    for f in frames[:n]:
        f = re.sub(r'^at (FD\.Macro\.)?', '', f)
        f = re.sub(r' in (?:.*[/\\])?([^/\\]+):line (\d+)$', r' (\1:\2)', f)
        out.append(f)
    return ' <- '.join(out)


def failure_text(side, rc, err):
    c = crash_of(err)
    if c:
        return f'{side} crashed at day {c[0]} (last checkpoint {c[1]}): {c[2]}'
    tail = ' | '.join(ln for ln in (err or '').strip().splitlines()[-3:])
    return f'{side} exited with code {rc}' + (f': {tail}' if tail else '')


# ---------------------------------------------------------------------------------------------------- formatting
def short(v, n=70):
    return v if v is None or len(v) <= n else v[:n - 3] + '...'


def cols_diff(a, b, names):
    fa, fb = a.split(' '), b.split(' ')
    return [n for i, n in enumerate(names) if (fa[i] if i < len(fa) else None) != (fb[i] if i < len(fb) else None)]


def split_flat(line):
    """'path=value' -> (path, value); '=' inside a ["quoted"] key segment does not split."""
    i, n, inq = 0, len(line), False
    while i < n:
        c = line[i]
        if inq:
            if c == '\\':
                i += 1
            elif c == '"':
                inq = False
        elif c == '"':
            inq = True
        elif c == '=':
            return line[:i], line[i + 1:]
        i += 1
    return line, ''


def parse_dumps(lines):
    """Multi-label dump output ('@label' headers) -> {label: (ordered paths, {path: value})}."""
    out, cur = {}, None
    for line in lines:
        if line.startswith('@'):
            cur = out.setdefault(line[1:], ([], {}))
            continue
        p, v = split_flat(line)
        cur[0].append(p)
        cur[1][p] = v
    return out


def area(path):
    g = re.sub(r'\[\d+\]', '[*]', path)
    g = re.sub(r'(\.[^.\[]+|\[\*\]|\["(?:[^"\\]|\\.)*"\])$', '', g)
    return g or path


# ---------------------------------------------------------------------------------------------------- analysis
def diff_state(seed, day, ts_label, cs_label, prev_label, report):
    """Dumps both sides at the first differing checkpoint and at the previous one; reports differing paths."""
    labels = lambda lab: lab if lab == prev_label else f'{prev_label},{lab}'
    res = run_both(['dump', seed, day, labels(ts_label)], ['dump', seed, day, labels(cs_label)])
    for side in ('TS', 'C#'):
        lines, rc, err = res[side]
        if rc != 0:
            report.append(f'  dump failed: {failure_text(side, rc, err)}')
            return
    ts = parse_dumps(res['TS'][0] if ts_label != prev_label else ['@' + prev_label] + res['TS'][0])
    cs = parse_dumps(res['C#'][0] if cs_label != prev_label else ['@' + prev_label] + res['C#'][0])
    (t_order, t_cur), (c_order, c_cur) = ts[ts_label], cs[cs_label]
    t_prev, c_prev = ts[prev_label][1], cs[prev_label][1]
    order = t_order + [p for p in c_order if p not in t_cur]
    diffs = [p for p in order if t_cur.get(p) != c_cur.get(p)]
    only_ts = sum(1 for p in diffs if p not in c_cur)
    only_cs = sum(1 for p in diffs if p not in t_cur)
    seg_ts = sum(1 for p in set(t_cur) | set(t_prev) if t_cur.get(p) != t_prev.get(p))
    seg_cs = sum(1 for p in set(c_cur) | set(c_prev) if c_cur.get(p) != c_prev.get(p))
    at = ts_label if ts_label == cs_label else f'TS {ts_label} / C# {cs_label}'
    since = f'; since {prev_label}: TS changed {seg_ts} paths, C# {seg_cs}' if prev_label not in (ts_label, cs_label) else ''
    report.append(f'  state at {at}: {len(diffs)} paths differ ({len(diffs) - only_ts - only_cs} changed, {only_ts} TS-only, '
                  f'{only_cs} C#-only){since}')
    if not diffs:
        report.append('    (no state difference: the checkpoint differs only in rng state / call count)')
        return
    has_prev = prev_label not in (ts_label, cs_label)
    prev_diff = [p for p in set(t_prev) | set(c_prev) if t_prev.get(p) != c_prev.get(p)] if has_prev else []
    if prev_diff:
        report.append(f'    note: {len(prev_diff)} paths already differ at {prev_label} (e.g. {sorted(prev_diff)[0]})')
    w = min(48, max(len(p) for p in diffs[:ARGS.paths]))
    for p in diffs[:ARGS.paths]:
        line = f'    {p:<{w}}  TS {short(t_cur.get(p, "(absent)"), 40):<24}  C# {short(c_cur.get(p, "(absent)"), 40):<24}'
        if has_prev:
            before = t_prev.get(p, '(absent)')
            if c_prev.get(p, '(absent)') != before:
                before += f' / C# {c_prev.get(p, "(absent)")}'
            line += f'  (before: {short(before, 40)})'
        report.append(line.rstrip())
    if len(diffs) > ARGS.paths:
        report.append(f'    ... {len(diffs) - ARGS.paths} more')
    groups = {}
    for p in diffs:
        groups[area(p)] = groups.get(area(p), 0) + 1
    if len(groups) > 1:
        top = sorted(groups.items(), key=lambda kv: -kv[1])[:10]
        report.append('    by area: ' + ', '.join(f'{g} {n}' for g, n in top) + (' ...' if len(groups) > 10 else ''))


def diff_rng(seed, day, seg_label, report):
    """RNG traces of the day on both sides: first divergent call (in the segment after seg_label) with context."""
    res = run_both(['rng', seed, day])
    T = [ln.split(' ', 3) for ln in res['TS'][0]]
    C = [ln.split(' ', 3) for ln in res['C#'][0]]
    for rows in (T, C):
        for r in rows:
            r += [''] * (4 - len(r))
    notes = [failure_text(s, res[s][1], res[s][2]) for s in ('TS', 'C#') if res[s][1] != 0]
    n = min(len(T), len(C))
    hard = next((i for i in range(n) if T[i][:3] != C[i][:3]), n if len(T) != len(C) else None)
    seg = next((i for i in range(len(T)) if T[i][1] == seg_label), None)
    end = hard if hard is not None else n
    soft = next((i for i in range(seg if seg is not None else 0, end) if T[i][3].lower() != C[i][3].lower()), None)
    head = f'  RNG (day {day}): TS {len(T)} calls, C# {len(C)} calls'
    call = lambda i: (T[i] if i < len(T) else C[i])[0] if i < max(len(T), len(C)) else '(end)'
    if hard is None and soft is None:
        report.append(head + f' -- identical sequence, labels and callers' + (f'; {"; ".join(notes)}' if notes else ''))
        return
    if soft is not None:
        report.append(head + f'; first caller mismatch at call {T[soft][0]} (segment after {seg_label}):')
        context(T, C, soft, report)
        if hard is not None and hard - soft > ARGS.context:
            report.append(f'    sequences diverge (label/value/length) at call {call(hard)}:')
            context(T, C, hard, report)
    else:
        where = f'; TS makes no call after checkpoint {seg_label}' if seg is None else f'; segment after {seg_label}'
        report.append(head + f'; sequences diverge at call {call(hard)} (label, value or length{where}):')
        context(T, C, hard, report)
    for note in notes:
        report.append(f'    ({note})')


def context(T, C, i, report):
    lo, hi = max(0, i - ARGS.context), min(max(len(T), len(C)), i + ARGS.context + 1)
    fmt = lambda r: f'{r[1]:<16} {r[2]:<22} {r[3]:<22}' if r else f'{"-":<16} {"":<22} {"":<22}'
    report.append(f'      {"#":>8}  {"TS: after checkpoint":<16} {"value":<22} {"caller":<22} | {"C#: after checkpoint":<16} {"value":<22} caller')
    for j in range(lo, hi):
        t = T[j] if j < len(T) else None
        c = C[j] if j < len(C) else None
        idx = (t or c)[0]
        mark = '>' if j == i else ' '
        diff = '' if (t and c and t[:3] == c[:3] and t[3].lower() == c[3].lower()) else ' *'
        report.append(f'    {mark} {idx:>8}  {fmt(t)} | {fmt(c).rstrip()}{diff}')


def investigate(seed, day, report):
    """First differing day (>= 1): checkpoints -> state diff -> RNG trace."""
    res = run_both(['cps', seed, day])
    T, C = res['TS'][0], res['C#'][0]
    n = min(len(T), len(C))
    k = next((i for i in range(n) if T[i] != C[i]), None)
    crashes = {s: crash_of(res[s][2]) for s in ('TS', 'C#') if res[s][1] != 0}
    if k is None:
        if len(T) == len(C):
            report.append(f'  checkpoints of day {day}: all {len(T)} identical (label, hash, rng state, rng calls): '
                          'the difference is only in the path/nav cache sizes')
            return
        k = n
    prev_label = T[k - 1].split()[0] if k > 0 else 'start'
    through = f' (through {prev_label})' if k > 0 else ''
    ended = [s for s, rows in (('TS', T), ('C#', C)) if k >= len(rows)]
    if ended:
        nxt = (T if ended[0] == 'C#' else C)[k].split()[0] if k < max(len(T), len(C)) else '?'
        report.append(f'  checkpoints of day {day}: {k} identical{through}; {ended[0]} stopped before checkpoint #{k + 1} ({nxt}):')
    else:
        report.append(f'  checkpoints of day {day}: {k} identical{through}, first difference at #{k + 1}' +
                      ('' if k > 0 else ' (the first checkpoint of the day)') + ':')
    for side, rows in (('TS', T), ('C#', C)):
        if k < len(rows):
            f = rows[k].split(' ')
            other = (C if side == 'TS' else T)
            dcols = cols_diff(rows[k], other[k], CP_COLS) if k < len(other) else []
            report.append(f'    {side:<3} {f[0]:<16} {" ".join(f[1:])}' + (f'   [{", ".join(dcols)}]' if dcols and side == 'C#' else ''))
        else:
            report.append(f'    {side:<3} -- {failure_text(side, res[side][1], res[side][2])[len(side) + 1:]}')
    for side, c in crashes.items():
        if c and c[3]:
            report.append(f'    {side} stack: ' + frames_text(c[3]))
    if k >= len(T) or k >= len(C):
        return  # a side crashed before this checkpoint; nothing to diff
    diff_state(seed, day, T[k].split()[0], C[k].split()[0], prev_label, report)
    diff_rng(seed, day, prev_label, report)


def check_seed(seed):
    days = ARGS.days
    report = []
    ts, cs = Proc('TS', ['hash', seed, days], threaded=True), Proc('C#', ['hash', seed, days], threaded=True)
    last, a, b = -1, None, None
    while True:
        a, b = ts.next(), cs.next()
        if a is None or b is None or a != b:
            break
        last = int(a.split(' ', 1)[0])
        if ARGS.progress and last and last % ARGS.progress == 0:
            say(f'  seed {seed}: day {last} ok ({time.time() - ts.t0:.0f} s)')
    rc_ts, err_ts, s_ts = ts.finish(kill=True)
    rc_cs, err_cs, s_cs = cs.finish(kill=True)
    ok_through = f'days 0..{last} identical' if last >= 0 else 'during new Sim(seed)'
    if a is not None and b is not None:          # both produced a line for this day, and it differs
        d = int(a.split(' ', 1)[0])
        report.append(f'seed {seed}: FIRST DIFFERENCE on day {d} ({ok_through if d else "the state right after new Sim(seed)"})')
        report.append(f'  TS  {a}')
        report.append(f'  C#  {b}   [{", ".join(cols_diff(a, b, HASH_COLS))}]')
        if d == 0:
            report.append('  (day 0 = right after new Sim(seed): no checkpoints, no RNG trace)')
            diff_state(seed, 0, 'init', 'init', 'init', report)
        else:
            investigate(seed, d, report)
        return False, report
    if a is None and b is None and rc_ts == 0 and rc_cs == 0 and last == days:
        report.append(f'seed {seed}: OK, {days} days identical (TS {s_ts:.0f} s, C# {s_cs:.0f} s)')
        return True, report
    # a side stopped early (crash, error exit, or truncated output)
    failed = [(s, rc, err) for s, rc, err, line in (('TS', rc_ts, err_ts, a), ('C#', rc_cs, err_cs, b))
              if line is None and (rc != 0 or last < days)] or [('TS', rc_ts, err_ts), ('C#', rc_cs, err_cs)]
    for side, rc, err in failed:
        report.append(f'seed {seed}: {failure_text(side, rc, err)} ({ok_through})')
    crash = next((c for c in (crash_of(err) for _, _, err in failed) if c), None)
    if crash and crash[0] >= 1 and crash[0] == last + 1:
        investigate(seed, crash[0], report)      # were the checkpoints before the crash still identical?
    elif crash and crash[3]:
        report.append('    stack: ' + frames_text(crash[3]))
    return False, report


def build():
    r = subprocess.run(['node', os.path.join('golden', 'ts', 'build.mjs'), 'trace'], cwd=MACRO, capture_output=True, text=True)
    if r.returncode != 0:
        say('TS bundle build failed:', r.stdout + r.stderr)
        sys.exit(2)
    for attempt in range(3):
        r = subprocess.run(['dotnet', 'build', os.path.join('FD.Macro.Run', 'FD.Macro.Run.csproj'), '-c', 'Release', '-nologo',
                            '-v:q', '-clp:NoSummary;ErrorsOnly'], cwd=MACRO, capture_output=True, text=True, env=env_for('C#'))
        if r.returncode == 0:
            return
        time.sleep(5)   # another agent's build may hold FD.Macro/obj
    errs = [ln for ln in (r.stdout + r.stderr).splitlines() if 'error' in ln]
    say('C# build failed:', *(errs[:30] or [r.stdout + r.stderr]))
    sys.exit(2)


def seed_list(s):
    out = []
    for part in s.split(','):
        m = re.fullmatch(r'(-?\d+)-(-?\d+)', part.strip())
        out += [str(x) for x in range(int(m.group(1)), int(m.group(2)) + 1)] if m else [part.strip()]
    return out


def main():
    global ARGS
    ap = argparse.ArgumentParser(description='TS vs C# golden test (see golden/README.md)')
    ap.add_argument('--seeds', default='1,2,3', help='comma-separated seeds and ranges, e.g. 1,2,3 or 1-20 (default 1,2,3)')
    ap.add_argument('--days', type=int, default=7200, help='days to simulate per seed (default 7200)')
    ap.add_argument('--jobs', type=int, default=1, help='seeds checked in parallel (each runs 2 processes; default 1)')
    ap.add_argument('--no-build', action='store_true', help='skip the TS bundle and C# Release builds')
    ap.add_argument('--paths', type=int, default=40, help='differing state paths to print (default 40)')
    ap.add_argument('--context', type=int, default=5, help='RNG calls of context around the divergence (default 5)')
    ap.add_argument('--progress', type=int, default=1000, help='print a progress line every N days (0: never; default 1000)')
    ap.add_argument('--fault', help='harness self-test: GOLDEN_FAULT for the C# side, e.g. crash@5:agents, rng@12:camps, state@3:econ:2')
    ARGS = ap.parse_args()
    if not ARGS.no_build:
        build()
    fp = subprocess.run(SIDES['TS'] + ['fingerprint'], cwd=MACRO, capture_output=True, text=True)
    if fp.stderr.strip():
        say(fp.stderr.strip())
    st = run_both(['selftest'])      # canonical-form test vectors: both runners must print the same bytes
    if st['TS'][1] or st['C#'][1] or st['TS'][0] != st['C#'][0]:
        bad = next((i for i, (a, b) in enumerate(zip(st['TS'][0], st['C#'][0])) if a != b), min(len(st['TS'][0]), len(st['C#'][0])))
        say('harness selftest differs between the runners (canonical form / hash mismatch):',
            f'  TS  {st["TS"][0][bad] if bad < len(st["TS"][0]) else st["TS"][2].strip()}',
            f'  C#  {st["C#"][0][bad] if bad < len(st["C#"][0]) else st["C#"][2].strip()}')
        return 2
    seeds = seed_list(ARGS.seeds)
    say(f'golden: seeds {",".join(seeds)}, {ARGS.days} days, {ARGS.jobs} job(s)' + (f', C# fault {ARGS.fault}' if ARGS.fault else ''))
    t0 = time.time()
    results = []

    def job(seed):
        try:
            ok, report = check_seed(seed)
        except Exception as e:      # a harness bug must not hide the other seeds' results
            import traceback
            ok, report = False, [f'seed {seed}: compare.py internal error: {e!r}'] + traceback.format_exc().rstrip().splitlines()
        say(*report)
        return seed, ok, report[0]

    with ThreadPoolExecutor(max_workers=max(1, ARGS.jobs)) as ex:
        results = list(ex.map(job, seeds))
    bad = [r for r in results if not r[1]]
    say(f'golden: {len(results) - len(bad)}/{len(results)} seeds identical over {ARGS.days} days ({time.time() - t0:.0f} s)')
    for _, _, first in bad:
        say('  ' + first)
    return 0 if not bad else 1


if __name__ == '__main__':
    try:
        sys.exit(main())
    except BrokenPipeError:          # output piped into head & co.
        sys.exit(1)
    except KeyboardInterrupt:
        sys.exit(130)
