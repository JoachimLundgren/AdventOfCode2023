using System.Data;
internal class Day12
{
    public static void Run()
    {
        var input = ParseInput("Day12/input.txt");
        //System.Console.WriteLine(Part1(input));
        System.Console.WriteLine(Part2(input));
    }
    
    private static List<Record> ParseInput(string fileName)
    {
        var lines = File.ReadAllLines(fileName);
        return lines.Select(line => new Record(line)).ToList();
    }

    private static long Part1(List<Record> lines)
    {
        return lines.Sum(l => GetArrangements(l.Row, l.Sizes));
    }

    private static long Part2(List<Record> lines)
    {
        //Naive and takes to long time.
        lines.ForEach(l => l.Unfold());
        return Part1(lines);
    }

    private static long GetArrangements(List<char> row, List<int> sizes)
    {
        var res = GetCount(true, row, sizes);
        System.Console.WriteLine(new string(row.ToArray()) + res);
        return res;
    }

    private static int GetCount(bool canDoBracket, List<char> row, List<int> sizes)
    {
        if (!row.SkipWhile(c => c == '.' || c == '?').Any() && !sizes.Any())
        {
            return 1;
        }
        else if (!row.SkipWhile(c => c == '.').Any() || !sizes.Any())
        {
            return 0;
        }

        var first = row.First();
        if (first == '#')
        {
            var count = sizes.First();
            if (canDoBracket && row.TakeWhile(c => c == '#' || c == '?').Count() >= count)
            {
                return GetCount(false, row.Skip(count).ToList(), sizes.Skip(1).ToList());
            }
            return 0;
        }
        else if (first == '.')
        {
            return GetCount(true, row.Skip(1).ToList(), sizes);
        }
        else if (first == '?')
        {
            return GetCount(canDoBracket, row.Skip(1).Prepend('.').ToList(), sizes)
                 + GetCount(canDoBracket, row.Skip(1).Prepend('#').ToList(), sizes);
        }

        throw new ApplicationException("Unknown first character");
    }

    private class Record
    {
        public List<char> Row { get;set;}
        public List<int> Sizes {get;set;}

        public Record(string line)
        {
            var l = line.Split(' ');
            Row = l[0].ToCharArray().ToList();
            Sizes = l[1].Split(',').Select(n => int.Parse(n)).ToList();
        }

        public void Unfold()
        {
            Row = Row.Append('?').Concat(Row).Append('?').Concat(Row).Append('?').Concat(Row).Append('?').Concat(Row).ToList();
            Sizes = Sizes.Concat(Sizes).Concat(Sizes).Concat(Sizes).Concat(Sizes).ToList();
        }
    }
}