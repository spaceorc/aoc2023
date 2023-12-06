# Advent of Code Input Parser/Runner library

## Basic usage:

```csharp
public static class Program
{
    private static void Main()
    {
        // run against file
        Runner.RunFile("input.txt", Solve);
        
        // run against string
        Runner.RunString("""
                         A B C
                         D E F
                         """, Solve);
        
        // run against string array
        Runner.Run(new[] {"A B C", "D E F"}, Solve);        
    }

    private static void Solve(string[] input)
    {
    }
}
```

## Parameters parsing

Fallback for simple `string[]`:

```csharp
public static class Program
{
    // input data:
    // ---
    // Game  1: text | 1, 2, 3
    // Game 22: another | 4, 55
    // ---
    private static void Solve(string[] input)
    {
        // input = new []
        // {
        //     "Game  1: text | 1, 2, 3",
        //     "Game 22: another | 4, 55"
        // }
    }
}
```

Mixing ValueTuple, nested ValueTuple, arrays, primitive types:

```csharp
public static class Program
{
    // input data:
    // ---
    // Game  1: text | 1, 2, 3
    // Game 22: another | 4, 55
    // ---
    private static void Solve((string prefix, (int id, string name), int[] values)[] input)
    {
        // input = new[]
        // {
        //     (prefix: "Game", (id: 1, name: "text"), values: new[] {1, 2, 3}),
        //     (prefix: "Game", (id: 22, name: "another"), values: new[] {4, 55})
        // }
    }
}
```

Records and classes - they MUST have only one public constructor with parameters

```csharp
public static class Program
{
    private record Desc(int Id, string Name);
    
    // input data:
    // ---
    // Game  1: text | 1, 2, 3
    // Game 22: another | 4, 55
    // ---
    private static void Solve((string _, Desc desc, int[] values)[] input)
    {
        // input = new[]
        // {
        //     (_: "Game", new Desc(1, "text"), values: new[] {1, 2, 3}),
        //     (_: "Game", new Desc(22, "another"), values: new[] {4, 55})
        // }
    }
}
```

Custom parsing method for types:

```csharp
public static class Program
{
    private record Desc2(string Text, int Len)
    {
        public static Desc2 Parse(string s) => new Desc2(s, s.Length)
    }    
 
    // input data:
    // ---
    // Game  1: text | 1, 2, 3
    // Game 22: another | 4, 55
    // ---
    private static void Solve((string _, int id, Desc2 desc, int[] values)[] input)
    {
        // input = new[]
        // {
        //     (_: "Game", id: 1, new Desc2("text", 4), values: new[] {1, 2, 3}),
        //     (_: "Game", id: 22, new Desc2("another", 7), values: new[] {4, 55})
        // }
    }
}
```

Customizing separators - default is ` ;,:|\n`:

```csharp
public static class Program
{
    // input data:
    // ---
    // Game  1: text - 1, 2, 3
    // Game 22: another - 4, 55
    // ---
    private static void Solve([Atom(":")] (string a, string b)[] input)
    {
        // input = new[]
        // {
        //     (a: "Game  1", b: "text - 1, 2, 3"),
        //     (a: "Game 22", b: "another - 4, 55")
        // }
    }
}
```

Templates:

```csharp
public static class Program
{
    // input data:
    // ---
    // Game  1:    text goes to 1, 2, 3
    // Game 22: another goes to 4, 55
    // ---
    private static void Solve(
        [Template("Game {id}: {name} goes to {values}")]
        (int id, string name, int[] values)[] input
    )
    {
        // input = new[]
        // {
        //     (id: "1", name: "text", values: new[] {1, 2, 3}),
        //     (id: "22", name: "another", values: new[] {4, 55})
        // }
    }
}
```

Templates of method level (actually, any attribute can be applied on method level):

```csharp
public static class Program
{
    // input data:
    // ---
    // Time:        54     81     70     88
    // Distance:   446   1292   1035   1007
    // ---
    [Template("""
              Time: {times}
              Distance: {distances}
              """)]
    private static void Solve(long[] times, long[] distances)
    {
        // times = new[] {54, 81, 70, 88}
        // distances = new[] {446, 1292, 1035, 1007}
    }
}
```

Regex:

```csharp
public static class Program
{
    // input data:
    // ---
    // Game  1:    text -> 1, 2, 3
    // Game 22: another -> 4, 55
    // ---
    private static void Solve(
        [Template(@"Game (?<id>.*)\:\s+(?<name>.*) -> (?<values>.*)", IsRegex = True)]
        (int id, string name, int[] values)[] input
    )
    {
        // input = new[]
        // {
        //     (id: 1, name: "text", values: new[] {1, 2, 3}),
        //     (id: 22, name: "another", values: new[] {4, 55})
        // }
    }
}
```

Split for class:

```csharp
public static class Program
{
    // input data:
    // ---
    // 1, 2, 3 | 4, 5
    // 7, 8 | 11, 12, 13, 14
    // ---
    private static void Solve(
        [Split("|")]
        (int[] a, int[] b)[] input
    )
    {
        // input = new[]
        // {
        //     (a: new[] {1, 2, 3}, b: new[] {4, 5}),
        //     (a: new[] {7, 8}, b: new[] {11, 12, 13, 14})
        // }
    }
}
```

Split for array:

```csharp
public static class Program
{
    // input data:
    // ---
    // 1, 2, 3 | 4, 5
    // 7, 8 | 11, 12, 13, 14
    // ---
    private static void Solve(
        [Split("|")]
        string[][] input
    )
    {
        // input = new[]
        // {
        //     new [] {"1, 2, 3", "4, 5"},
        //     new [] {"7, 8", "11, 12, 13, 14"}
        // }
    }
}
```