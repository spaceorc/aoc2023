using System;

namespace aoc;

public static class Snailfish
{
    public class Number
    {
        public Pair? Parent;

        public Number Clone()
        {
            return this switch
            {
                Pair pair => new Pair(pair.Left.Clone(), pair.Right.Clone()),
                Value value => new Value(value.V),
                _ => throw new Exception("WTF???")
            };
        }

        public void Reduce()
        {
            while (true)
            {
                var e = FindExplode();
                if (e != null)
                {
                    e.Explode();
                    continue;
                }

                var s = FindSplit();
                if (s != null)
                {
                    s.Split();
                    continue;
                }

                break;
            }
        }

        public static Number Read(string source)
        {
            var pos = 0;
            var res = Read(source, ref pos);
            if (pos != source.Length)
                throw new Exception("WTF???");
            return res;
        }

        private static Number Read(string source, ref int pos)
        {
            if (char.IsDigit(source[pos]))
                return new Value(source[pos++] - '0');

            if (source[pos++] != '[')
                throw new Exception("WTF?");
            
            var left = Read(source, ref pos);
            
            if (source[pos++] != ',')
                throw new Exception("WTF?");
            
            var right = Read(source, ref pos);
            
            if (source[pos++] != ']')
                throw new Exception("WTF?");

            return new Pair(left, right);
        }

        public Pair? FindExplode(int level = 0)
        {
            return this switch
            {
                Pair { Left: Value, Right: Value } pair => level >= 4 ? pair : null,
                Pair pair => pair.Left.FindExplode(level + 1) ?? pair.Right.FindExplode(level + 1),
                _ => null
            };
        }

        public Value? FindSplit()
        {
            return this switch
            {
                Pair pair => pair.Left.FindSplit() ?? pair.Right.FindSplit(),
                Value value => value.V >= 10 ? value : null,
                _ => null
            };
        }

        public long Magnitude()
        {
            return this switch
            {
                Pair pair => pair.Left.Magnitude() * 3 + pair.Right.Magnitude() * 2,
                Value value => value.V,
                _ => throw new Exception("WTF???")
            };
        }

        public static Number Add(Number a, Number b)
        {
            var result = new Pair(a, b);
            a.Parent = result;
            b.Parent = result;
            return result;
        }
    }

    public class Value : Number
    {
        public Value(int v)
        {
            V = v;
        }

        public int V;

        public void Split()
        {
            if (Parent!.Left == this)
                Parent.Left = new Pair(new Value(V / 2), new Value((V + 1) / 2)) { Parent = Parent };
            if (Parent!.Right == this)
                Parent.Right = new Pair(new Value(V / 2), new Value((V + 1) / 2)) { Parent = Parent };
        }

        public override string ToString()
        {
            return V.ToString();
        }
    }

    public class Pair : Number
    {
        public Pair(Number left, Number right)
        {
            Left = left;
            Right = right;
            left.Parent = this;
            right.Parent = this;
        }

        public Number Left;
        public Number Right;

        public void Explode()
        {
            var left = ((Value)Left).V;
            var right = ((Value)Right).V;
            var cur = this;
            while (true)
            {
                var p = cur.Parent;
                if (p == null)
                    break;
                if (p.Right == cur)
                {
                    var next = p.Left;
                    while (next is Pair np)
                        next = np.Right;
                    ((Value)next).V += left;
                    break;
                }

                cur = p;
            }

            cur = this;
            while (true)
            {
                var p = cur.Parent;
                if (p == null)
                    break;
                if (p.Left == cur)
                {
                    var next = p.Right;
                    while (next is Pair np)
                        next = np.Left;
                    ((Value)next).V += right;
                    break;
                }

                cur = p;
            }

            if (Parent!.Left == this)
                Parent.Left = new Value(0) { Parent = Parent };
            if (Parent!.Right == this)
                Parent.Right = new Value(0) { Parent = Parent };
        }

        public override string ToString()
        {
            return $"[{Left},{Right}]";
        }
    }
}