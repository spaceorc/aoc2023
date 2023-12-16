from collections import deque
from dataclasses import dataclass
from typing import Self, Iterable, TypeVar, Generic, Callable


@dataclass
class V:
    x: int
    y: int

    def __add__(self, other: Self) -> Self:
        return V(self.x + other.x, self.y + other.y)

    def __mul__(self, other: int) -> Self:
        return V(self.x * other, self.y * other)

    def cw(self) -> Self:
        return V(-self.y, self.x)

    def ccw(self) -> Self:
        return V(self.y, -self.x)

    def __eq__(self, other: Self) -> bool:
        return self.x == other.x and self.y == other.y

    def __hash__(self) -> int:
        return hash((self.x, self.y))


UP = V(0, -1)
DOWN = V(0, 1)
LEFT = V(-1, 0)
RIGHT = V(1, 0)


@dataclass
class Map:
    rows: list[str]

    @property
    def size_x(self) -> int:
        return len(self.rows[0])

    @property
    def size_y(self) -> int:
        return len(self.rows)

    def __getitem__(self, v: V) -> str:
        return self.rows[v.y][v.x]

    def __contains__(self, v: V) -> bool:
        return 0 <= v.y < self.size_y and 0 <= v.x < self.size_x

    def top_border(self) -> list[V]:
        return [V(x, 0) for x in range(self.size_x)]

    def left_border(self) -> list[V]:
        return [V(0, y) for y in range(self.size_y)]

    def bottom_border(self) -> list[V]:
        return [V(x, self.size_y - 1) for x in range(self.size_x)]

    def right_border(self) -> list[V]:
        return [V(self.size_x - 1, y) for y in range(self.size_y)]


T = TypeVar("T")


@dataclass
class BfsItem(Generic[T]):
    state: T
    distance: int
    prev: Self | None

    def path_back(self) -> Iterable[T]:
        cur = self
        while cur is not None:
            yield cur.state
            cur = cur.prev

    def path(self) -> list[T]:
        path = list(self.path_back())
        path.reverse()
        return path


def bfs(start: list[T], get_next: Callable[[T], list[T]], max_distance: int = 1000000000) -> Iterable[BfsItem[T]]:
    queue = deque(start)
    used = {s: BfsItem[T](s, 0, None) for s in start}
    while len(queue):
        cur = queue.popleft()
        curItem = used[cur]
        yield curItem

        if curItem.distance >= max_distance:
            continue

        for n in get_next(cur):
            if n not in used:
                used[n] = BfsItem(n, curItem.distance + 1, curItem)
                queue.append(n)


def count_energized(map: Map, start_pos: V, start_dir: V) -> int:
    def get_next(cur: tuple[V, V]) -> list[tuple[V, V]]:
        pos, dir = cur
        match map[pos], dir:
            case '.', _:
                next_dirs = [dir]
            case '-', V(_, 0):
                next_dirs = [dir]
            case '|', V(0, _):
                next_dirs = [dir]
            case '/', V(_, 0):
                next_dirs = [dir.ccw()]
            case '/', V(0, _):
                next_dirs = [dir.cw()]
            case '\\', V(_, 0):
                next_dirs = [dir.cw()]
            case '\\', V(0, _):
                next_dirs = [dir.ccw()]
            case _:
                next_dirs = [dir.cw(), dir.ccw()]
        return [
            (pos + dir, dir)
            for dir in next_dirs
            if pos + dir in map
        ]

    return len({
        item.state[0]
        for item in bfs(start=[(start_pos, start_dir)], get_next=get_next)
    })


with open("day16.txt") as f:
    map = Map([x.strip() for x in f.readlines()])

res1 = count_energized(map, V(0, 0), RIGHT)
print(f"Part 1: {res1}")

res2 = max(
    max(count_energized(map, v, DOWN) for v in map.top_border()),
    max(count_energized(map, v, UP) for v in map.bottom_border()),
    max(count_energized(map, v, RIGHT) for v in map.left_border()),
    max(count_energized(map, v, LEFT) for v in map.right_border()),
)
print(f"Part 2: {res2}")
