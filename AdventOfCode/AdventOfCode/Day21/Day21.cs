

using System.Reflection;
using AdventOfCode;

internal class Day21
{
    public static void Run()
    {
        System.Console.WriteLine(Part1(ParseInput("Day21/testinput.txt"), 6));
        System.Console.WriteLine(Part1(ParseInput("Day21/input.txt"), 64));
        System.Console.WriteLine(Part2(ParseInput("Day21/input.txt"), 26501365)); //602259568764234
    }

    private static char[][] ParseInput(string fileName)
    {
        return File.ReadAllLines(fileName)
            .Select(line => line.ToCharArray().ToArray())
            .ToArray();
    }

    private static long Part1(char[][] map, int steps)
    {
        List<(int row, int col)> positions = ListUtils.GetCoordinates(map, 'S').Select(c => ((int)c.Y, (int)c.X)).ToList();

        for (int i = 0; i < steps; i++)
        {
            var newPositions = new List<(int, int)>();

            foreach (var p in positions)
            {
                var adjacent = new List<(int row, int column)>()
                {
                    (p.row - 1, p.col),
                    (p.row, p.col - 1),
                    (p.row, p.col + 1),
                    (p.row + 1, p.col),
                };
                
                foreach (var newP in adjacent)
                {
                    if (map[EnsureInside(newP.row)][EnsureInside(newP.column)] != '#')
                    {
                        newPositions.Add(newP);
                    }
                }
            }

            positions = newPositions.Distinct().ToList();
        }

        return positions.Count();
    }
    private static int EnsureInside(int n) => ((n % 131) + 131) % 131;


    //Couldn't solve this myself so took inspiration from reddit
    private static long Part2(char[][] map, int n)
    {
        (decimal x0, decimal y0) = (65, Part1(map, 65));
        (decimal x1, decimal y1) = (196, Part1(map, 196));
        (decimal x2, decimal y2) = (327, Part1(map, 327));

        decimal y01 = (y1 - y0) / (x1 - x0);
        decimal y12 = (y2 - y1) / (x2 - x1);
        decimal y012 = (y12 - y01) / (x2 - x0);

        return (long)(y0 + y01 * (n - x0) + y012 * (n - x0) * (n - x1));
    }

}