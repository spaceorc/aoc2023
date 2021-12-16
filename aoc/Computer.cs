using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aoc;

public enum PacketType
{
    Sum = 0,
    Mul = 1,
    Min = 2,
    Max = 3,
    Literal = 4,
    Gt = 5,
    Lt = 6,
    Eq = 7,
}

public abstract record Packet(int Version, PacketType TypeId)
{
    public virtual long SumVersion() => Version;
    public abstract long Calculate();

    public static Packet ReadFromHex(string source)
    {
        var binary = string.Join(
            "",
            source.Select(x => Convert.ToString(Convert.ToInt64(x.ToString(), 16), 2).PadLeft(4, '0'))
        );
        return ReadFromBinary(binary);
    }

    public static Packet ReadFromBinary(string binary)
    {
        var pos = 0;
        return ReadFromBinary(binary, ref pos);
    }

    private static Packet ReadFromBinary(string binary, ref int pos)
    {
        var (version, typeId) = ReadPacketHeader(binary, ref pos);
        switch (typeId)
        {
            case PacketType.Literal:
                return new Literal(version, typeId, ReadLiteral(binary, ref pos));
            default:
                var (lengthTypeId, length) = ReadOperatorHeader(binary, ref pos);
                var arguments = new List<Packet>();
                switch (lengthTypeId)
                {
                    case 0:
                        var endPos = pos + length;
                        while (pos < endPos)
                            arguments.Add(ReadFromBinary(binary, ref pos));
                        break;
                    case 1:
                        for (int i = 0; i < length; i++)
                            arguments.Add(ReadFromBinary(binary, ref pos));
                        break;
                }

                return new Operator(version, typeId, arguments.ToArray());
        }
    }

    private static (int Version, PacketType typeId) ReadPacketHeader(string binary, ref int pos)
    {
        var version = Convert.ToInt32(binary[pos..(pos + 3)], 2);
        pos += 3;
        var typeId = Convert.ToInt32(binary[pos..(pos + 3)], 2);
        pos += 3;
        return (version, (PacketType)typeId);
    }

    private static (int lengthTypeId, int length) ReadOperatorHeader(string binary, ref int pos)
    {
        var lengthTypeId = Convert.ToInt32(binary[pos..(pos + 1)], 2);
        pos += 1;
        if (lengthTypeId == 0)
        {
            var length = Convert.ToInt32(binary[pos..(pos + 15)], 2);
            pos += 15;
            return (lengthTypeId, length);
        }
        else
        {
            var length = Convert.ToInt32(binary[pos..(pos + 11)], 2);
            pos += 11;
            return (lengthTypeId, length);
        }
    }

    private static long ReadLiteral(string binary, ref int pos)
    {
        var result = new StringBuilder();
        while (true)
        {
            var next = binary[pos..(pos + 5)];
            pos += 5;
            result.Append(next[1..]);
            if (next[0] == '0')
                break;
        }

        if (result.Length > 60)
            throw new InvalidOperationException(result.Length.ToString());

        return Convert.ToInt64(result.ToString(), 2);
    }
}

public record Operator(int Version, PacketType TypeId, Packet[] Arguments)
    : Packet(Version, TypeId)
{
    public override long SumVersion()
    {
        return base.SumVersion() + Arguments.Sum(x => x.SumVersion());
    }

    public override long Calculate()
    {
        switch (TypeId)
        {
            case PacketType.Sum:
                return Arguments.Sum(x => x.Calculate());
            case PacketType.Mul:
                return Arguments.Aggregate(1L, (a, b) => a * b.Calculate());
            case PacketType.Min:
                return Arguments.Min(x => x.Calculate());
            case PacketType.Max:
                return Arguments.Max(x => x.Calculate());
            case PacketType.Gt:
                return Arguments[0].Calculate() > Arguments[1].Calculate() ? 1 : 0;
            case PacketType.Lt:
                return Arguments[0].Calculate() < Arguments[1].Calculate() ? 1 : 0;
            case PacketType.Eq:
                return Arguments[0].Calculate() == Arguments[1].Calculate() ? 1 : 0;
            default:
                throw new InvalidOperationException($"Bad type id: {TypeId}");
        }
    }

    public override string ToString()
    {
        switch (TypeId)
        {
            case PacketType.Sum:
                return $"({string.Join(" + ", Arguments.Select(x => x.ToString()))})";
            case PacketType.Mul:
                return $"({string.Join(" * ", Arguments.Select(x => x.ToString()))})";
            case PacketType.Min:
                return $"MIN({string.Join(", ", Arguments.Select(x => x.ToString()))})";
            case PacketType.Max:
                return $"MAX({string.Join(", ", Arguments.Select(x => x.ToString()))})";
            case PacketType.Gt:
                return $"({string.Join(" > ", Arguments.Select(x => x.ToString()))})";
            case PacketType.Lt:
                return $"({string.Join(" < ", Arguments.Select(x => x.ToString()))})";
            case PacketType.Eq:
                return $"({string.Join(" = ", Arguments.Select(x => x.ToString()))})";
            default:
                return $"Bad type id: {TypeId}";
        }
    }
}

public record Literal(int Version, PacketType TypeId, long Value)
    : Packet(Version, TypeId)
{
    public override long Calculate() => Value;

    public override string ToString()
    {
        return Value.ToString();
    }
}