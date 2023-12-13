using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

internal class Day11
{
    public static void Run()
    {
        var map = ParseInput("Day11/testinput.txt");
        System.Console.WriteLine(Part1(map));
        var map2 = ParseInput("Day11/input.txt");
        System.Console.WriteLine(Solve(map2, 2));
        System.Console.WriteLine(Solve(map2, 10));
        System.Console.WriteLine(Solve(map2, 100));

        System.Console.WriteLine(Solve(map2, 1_000_000));
    }

    private static List<List<char>> ParseInput(string fileName)
    {
        return File.ReadAllLines(fileName)
            .Select(line => line.ToCharArray().ToList())
            .ToList();
    }

    private static long Part1(List<List<char>> map)
    {
        long totalDistance = 0;

        map = Expand(map);
        var points = ListUtils.GetCoordinates(map, '#');
        foreach (var point in points)
        {
            var distance = points
                .Sum(p => Math.Abs(point.X - p.X) + Math.Abs(point.Y - p.Y));
            totalDistance += distance;
        }

        return totalDistance / 2;
    }

    private static long Solve(List<List<char>> map, long multiplier = 2)
    {
        long totalDistance = 0;

        var (emptyX, emptyY) = GetEmptyRowsAndColumns(map);
        var points = ListUtils.GetCoordinates(map, '#');

        foreach (var point in points)
        {
            point.X += emptyX.Where(x => x < point.X).Count() * (multiplier - 1);
            point.Y += emptyY.Where(y => y < point.Y).Count() * (multiplier - 1);
        }

        foreach (var point in points)
        {
            var distance = points
                .Sum(p => Math.Abs(point.X - p.X) + Math.Abs(point.Y - p.Y));
            totalDistance += distance;
        }

        return totalDistance / 2;
    }

    private static List<List<char>> Expand(List<List<char>> map)
    {
        ListUtils.PrintMatrix(map);

        for (int x = 0; x < map[0].Count(); x++)
        {
            if (map.All(m => m[x] == '.'))
            {
                map.ForEach(m => m.Insert(x, '.'));
                x++;
            }
        }

        for (int y = 0; y < map.Count(); y++)
        {
            if (map[y].All(m => m == '.'))
            {
                map.Insert(y, Enumerable.Repeat('.', map[y].Count()).ToList());
                y++;
            }
        }

        ListUtils.PrintMatrix(map);
        return map;
    }

    private static (List<int> xs, List<int> ys) GetEmptyRowsAndColumns(List<List<char>> map)
    {
        var xs = new List<int>();
        var ys = new List<int>();

        for (int x = 0; x < map[0].Count(); x++)
        {
            if (map.All(m => m[x] == '.'))
            {
                xs.Add(x);
            }
        }

        for (int y = 0; y < map.Count(); y++)
        {
            if (map[y].All(m => m == '.'))
            {
                ys.Add(y);
            }
        }

        //ListUtils.PrintMatrix(map);
        return (xs, ys);
    }



    private static List<(long x, long y)> GetCoordinates(List<List<char>> map, char obj)
    {
        var res = new List<(long x, long y)>();

        for (int y = 0; y < map.Count(); y++)
        {
            var columns = map.ElementAt(y).Count();
            for (int x = 0; x < columns; x++)
            {
                if (map.ElementAt(y).ElementAt(x).Equals(obj))
                {
                    res.Add((x, y));
                }
            }
        }

        return res;
    }
}