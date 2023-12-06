using System;

namespace aoc.ParseLib;

public abstract record TypeStructure(Type Type)
{
    public abstract object CreateObject(string source);
}
