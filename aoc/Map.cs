using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace aoc
{
    public class Map<T>
    {
        public readonly int sizeX;
        public readonly int sizeY;
        public readonly int totalCount;
        public readonly T[] data;

        public Map(int size)
            : this(size, size)
        {
        }

        public Map(int sizeX, int sizeY)
        {
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            totalCount = sizeX * sizeY;
            data = new T[totalCount];
        }

        public T this[V v]
        {
            get => data[v.Y * sizeX + v.X];
            set => data[v.Y * sizeX + v.X] = value;
        }

        public T this[int index]
        {
            get => data[index];
            set => data[index] = value;
        }

        public void Clear()
        {
            Array.Fill(data, default);
        }

        public void Fill(T value)
        {
            Array.Fill(data, value);
        }

        public bool Inside(V v) => v.X >= 0 && v.Y >= 0 && v.X < sizeX && v.Y < sizeY;

        public IEnumerable<V> All()
        {
            for (int y = 0; y < sizeY; y++)
            for (int x = 0; x < sizeX; x++)
            {
                yield return new V(x, y);
            }
        }

        public IEnumerable<V> Nears(V v)
        {
            return V.nears.Select(dv => v + dv).Where(n => Inside(n));
        }

        public Map<T> Clone()
        {
            var clone = new Map<T>(sizeX, sizeY);
            Array.Copy(data, clone.data, totalCount);
            return clone;
        }
    }
}