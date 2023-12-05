using System;

namespace aoc.ParseLib;

public abstract record Structure(Type Type)
{
    public abstract object CreateObject(string source);
}
