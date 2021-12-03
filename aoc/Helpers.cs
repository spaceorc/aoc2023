using System.Collections.Generic;

namespace aoc
{
    public static class Helpers
    {
        public static IEnumerable<int[]> Iterate(int n, int inputSize)
        {
            var connections = new int[n];
            for (int i = 0; i < n; i++)
                connections[i] = -inputSize;

            var found = true;
            while (found)
            {
                yield return connections;
                found = false;
                for (int i = 0; i < n; i++)
                {
                    connections[i]++;
                    if (connections[i] == n)
                    {
                        connections[i] = -inputSize;
                        continue;
                    }

                    found = true;
                    break;
                }
            }
        }

    }
}