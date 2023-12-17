using AdventOfCode;
internal class Day12
{
    public static void Run()
    {
        Ensure.Solve("Part 1 testinput", () => Part1(ParseInput("Day12/testinput.txt")), 21, runs: 10);
        Ensure.Solve("Part 1", () => Part1(ParseInput("Day12/input.txt")), 7599, runs: 10);  //653ms
        Ensure.Solve("Part 2 testinput", () => Part2(ParseInput("Day12/testinput.txt")), 525152, runs: 10); //8865ms
        Ensure.Solve("Part 2", () => Part2(ParseInput("Day12/input.txt")), 15454556629917, runs: 10); //4558ms
    }

    private static List<Record> ParseInput(string fileName)
    {
        var lines = File.ReadAllLines(fileName);
        return lines.Select(line => new Record(line)).ToList();
    }

    private static long Part1(List<Record> records)
    {
        return records.Sum(r => r.Count2());
    }

    private static long Part2(List<Record> lines)
    {
        lines.ForEach(l => l.Unfold());
        return Part1(lines);
    }

    private class Record
    {
        public char[] Row { get; set; }
        public int[] Sizes { get; set; }
        public Record(string line)
        {
            var l = line.Split(' ');
            Row = l[0].ToCharArray();
            Sizes = l[1].Split(',').Select(n => int.Parse(n)).ToArray();
        }

        public void Unfold()
        {
            Row = [.. Row, '?', .. Row, '?', .. Row, '?', .. Row, '?', .. Row];
            Sizes = [.. Sizes, .. Sizes, .. Sizes, .. Sizes, .. Sizes];
        }

        public long Count2()
        {
            var impl = CacheUtils.Memoized<(bool, int, int), long>((cnt, input) =>
            {
                var canDoBracket = input.Item1;
                var rowStart = input.Item2;
                var sizesStart = input.Item3;

                if (rowStart >= Row.Count())
                {
                    return Sizes.Skip(sizesStart).All(c => c == '#' || c == '?') ? 1 : 0;
                }

                var first = Row[rowStart];
                if (first == '#')
                {
                    if (sizesStart < Sizes.Count())
                    {
                        var count = Sizes[sizesStart];
                        if (canDoBracket && Row.Skip(rowStart).TakeWhile(c => c == '#' || c == '?').Count() >= count)
                        {
                            return cnt((false, rowStart + count, sizesStart + 1));
                        }

                    }
                    return 0;
                }
                else if (first == '.')
                {
                    return cnt((true, rowStart + 1, sizesStart));
                }
                else if (first == '?')
                {
                    var res = cnt((true, rowStart + 1, sizesStart));    //.
                    if (canDoBracket)
                    {
                        if (sizesStart < Sizes.Count())
                        {
                            var count = Sizes[sizesStart];
                            if (Row.Skip(rowStart).TakeWhile(c => c == '#' || c == '?').Count() >= count)
                            {
                                res += cnt((false, rowStart + count, sizesStart + 1));
                            }
                        }
                    }

                    return res;
                }

                throw new ApplicationException("Unknown first character");
            });

            return impl((true, 0, 0));
        }
    }
}