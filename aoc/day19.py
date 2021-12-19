from itertools import *
from typing import *
from functools import *

MATRIX = Sequence[Sequence[int]]
V = Sequence[int]


def matrix_prod(a: MATRIX, b: MATRIX) -> MATRIX:
    return [
        [
            sum([a[i][k] * b[k][j] for k in range(len(b))])
            for j in range(len(b[0]))
        ]
        for i in range(len(a))
    ]


def matrix_vec_prod(m: MATRIX, v: V) -> V:
    return [r[0] for r in matrix_prod(m, [[c] for c in v])]


def vec_prod(a: V, b: V) -> V:
    return [
        a[1] * b[2] - a[2] * b[1],
        a[2] * b[0] - a[0] * b[2],
        a[0] * b[1] - a[1] * b[0]
    ]


def add(a: V, b: V) -> V:
    return [
        a[i] + b[i] for i in range(len(a))
    ]


def sub(a: V, b: V) -> V:
    return [
        a[i] - b[i] for i in range(len(a))
    ]


def m_len(v: V) -> int:
    return sum((abs(c) for c in v))


def c_len(v: V) -> int:
    return max((abs(c) for c in v))


ROTATIONS: list[MATRIX] = [
    rot for rot, bs in [
        (rot, [matrix_vec_prod(rot, b) for b in [
            [1, 0, 0],
            [0, 1, 0],
            [0, 0, 1],
        ]])
        for rot in (p for c in combinations([
            [1, 0, 0],
            [0, 1, 0],
            [0, 0, 1],
            [-1, 0, 0],
            [0, -1, 0],
            [0, 0, -1],
        ], 3) for p in permutations(c))
    ] if all((m_len(b) == 1 for b in bs)) and vec_prod(bs[0], bs[1]) == bs[2]
]


def vec_rotate(v: V, r: int) -> V:
    return matrix_vec_prod(ROTATIONS[r], v)


def scan_rotate(s: list[V], r: int) -> list[V]:
    return [vec_rotate(v, r) for v in s]


def scan_shift(s: list[V], shift: V) -> list[V]:
    return [add(v, shift) for v in s]


def get_shift(a: list[V], b: list[V]) -> Optional[V]:
    for ai in a:
        for bi in b:
            shift = sub(ai, bi)
            common = 0
            bad = False
            for bk in b:
                b2 = add(bk, shift)
                if b2 in a:
                    common += 1
                elif c_len(b2) <= 1000:
                    bad = True
                    break
            if not bad and common >= 12:
                for ak in a:
                    a2 = sub(ak, shift)
                    if a2 not in b and c_len(a2) <= 1000:
                        bad = True
                        break
                if not bad:
                    return shift
    return None


def get_rotation_and_shift(a: list[V], b: list[V]) -> Optional[tuple[int, V]]:
    for r in range(24):
        shift = get_shift(a, scan_rotate(b, r))
        if shift:
            return r, shift
    return None


def key(v: V) -> int:
    return reduce(lambda acc, item: acc * 10000 + item, v, 0)


def solve(scans: list[list[V]]) -> None:
    beacons = {key(v): v for v in scans[0]}
    scanners: list[V] = [[0, 0, 0]]
    queue = [0]
    used = {0: []}
    head = 0
    while head < len(queue):
        cur, head = queue[head], head + 1
        for next, next_scan in enumerate(scans):
            if next in used:
                continue
            rotation_and_shift = get_rotation_and_shift(scans[cur], next_scan)
            if not rotation_and_shift:
                continue

            rotations_and_shifts = [rotation_and_shift] + used[cur]

            transformed = reduce(
                lambda acc, r_and_s: scan_shift(scan_rotate(acc, r_and_s[0]), r_and_s[1]),
                rotations_and_shifts,
                next_scan,
            )
            beacons.update(((key(v), v) for v in transformed))

            scanners.append(reduce(
                lambda acc, r_and_s: add(vec_rotate(acc, r_and_s[0]), r_and_s[1]),
                rotations_and_shifts,
                [0, 0, 0],
            ))

            used[next] = rotations_and_shifts
            queue.append(next)

    print(f"beacons count: {len(beacons)}")
    print(f"max scanners dist: {max((m_len(sub(a, b)) for a, b in product(scanners, scanners)))}")


with open("day19.txt") as f:
    lines = f.readlines()

scans_raw = "".join(lines).split("\n\n")
scans = [
    [[int(c) for c in scan_line.split(",")] for scan_line in scan_raw.strip().split("\n")[1:]]
    for scan_raw in scans_raw
]
solve(scans)
