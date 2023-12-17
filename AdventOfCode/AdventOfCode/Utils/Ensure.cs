using System.Diagnostics;

namespace AdventOfCode;

public class Ensure
{
    public static void Equality<T>(T first, T second)
    {
        if (first == null)
        {
            if (second == null)
            {
                return;
            }
            throw new ApplicationException("First null");
        }

        if (!first.Equals(second))
        {
            throw new ApplicationException("Not equal");
        }
    }

    internal static void Solve(string name, Func<long> value, long? expectedResult = null, int runs = 1)
    {
        var timer = new Stopwatch();
        timer.Start();
        long res = 0;
        for (int i = 0; i < runs; i++)
        {
            res = value();
        }
        timer.Stop();
        System.Console.WriteLine($"Ran {name} {runs} time(s). Got {res} in {timer.ElapsedMilliseconds}ms");
        if (expectedResult != null)
        {
            Equality(res, expectedResult);
        }
    }
}
