internal class Day9
{
    public static void Run()
    {
        var input = ParseInput("Day9/input.txt");
        var values = input.Select(i => GetNexValue(i));
        System.Console.WriteLine(values.Sum(v => v.end));
        System.Console.WriteLine(values.Sum(v => v.start));
    }

    private static List<List<int>> ParseInput(string fileName)
    {
        var lines = File.ReadAllLines(fileName);
        return lines
            .Select(l => l.Split(' ').Select(v => int.Parse(v)).ToList())
            .ToList();
    }

    private static (long start, long end) GetNexValue(List<int> history)
    {
        var startNumbers = new List<int> { history.First() };
        var endNumbers = new List<int> { history.Last() };
        while (!history.All(h => h == 0))
        {
            history = GenerateNextList(history);
            startNumbers.Add(history.First());
            endNumbers.Add(history.Last());
        }

        endNumbers.Reverse();
        var end = 0;
        foreach (var endNumber in endNumbers)
        {
            end = endNumber + end;
        }

        startNumbers.Reverse();
        var start = 0;
        foreach (var startNumber in startNumbers)
        {
            start = startNumber - start;
        }
        return (start, end);
    }

    private static List<int> GenerateNextList(List<int> history)
    {
        var result = new List<int>();
        for (int i = 0; i < history.Count - 1; i++)
        {
            result.Add(history.ElementAt(i + 1) - history.ElementAt(i));
        }
        return result;
    }
}