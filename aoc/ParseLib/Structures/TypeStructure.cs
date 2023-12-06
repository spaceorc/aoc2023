using System;

namespace aoc.ParseLib.Structures;

public abstract record TypeStructure(Type Type)
{
    public abstract object CreateObject(string source);
}
