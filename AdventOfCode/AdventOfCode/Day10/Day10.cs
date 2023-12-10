using System.Text.RegularExpressions;

internal class Day10
{
    private static Dictionary<char, Direction[]> connections = new Dictionary<char, Direction[]>()
    {
        {'|', new [] { Direction.North, Direction.South }},
        {'-', new [] { Direction.West, Direction.East }},
        {'L', new [] { Direction.North, Direction.East }},
        {'J', new [] { Direction.North, Direction.West }},
        {'7', new [] { Direction.South, Direction.West }},
        {'F', new [] { Direction.South, Direction.East }},
    };

    private static Dictionary<Direction, Direction> mirror = new Dictionary<Direction, Direction>()
    {
        { Direction.North, Direction.South},
        { Direction.South, Direction.North},
        { Direction.East, Direction.West},
        { Direction.West, Direction.East},
    };

    public static void Run()
    {
        var map = ParseInput("Day10/input.txt");
        System.Console.WriteLine(Part1(map));
        System.Console.WriteLine(Part2(map));
    }

    private static char[][] ParseInput(string fileName)
    {
        return File.ReadAllLines(fileName)
            .Select(line => line.ToCharArray())
            .ToArray();
    }

    private static int Part1(char[][] map)
    {
        return GetPath(map, out _, out _);
    }

    private static int Part2(char[][] map)
    {
        GetPath(map, out var path, out var startNode);

        //Clear everything not in loop
        for (int y = 0; y < map.Length; y++)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                if (map[y][x] == 'S')
                {
                    map[y][x] = '|';
                }
                else if (!path.Contains((x, y)))
                {
                    map[y][x] = '.';
                }
            }
        }

        var count = 0;
        for (int y = 0; y < map.Length; y++)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                if (map[y][x] == '.')
                {
                    if (IsInsideLoop(map, x, y))
                    {
                        count++;
                        map[y][x] = 'I';
                    }
                    else
                    {
                        map[y][x] = 'O';
                    }
                }
            }
        }

        //CharUtils.PrintMatrix(map);
        return count;
    }

    private static bool IsInsideLoop(char[][] map, int x, int y)
    {
        var hasOddNumberOfVerticalPipes = (string input) =>
        {
            input = Regex.Replace(input, "F-*7|L-*J", string.Empty);  //F7 & LJ cancel eachother out
            input = Regex.Replace(input, "F-*J|L-*7", "|");  //Count FJ and L7 & LJ as one pass
            var verticalPipes = input.Count(c => c == '|');
            return verticalPipes % 2 == 1;
        };

        var strRowBefore = new string(map[y].Take(x).ToArray());
        var strRowAfter = new string(map[y].Skip(x + 1).ToArray());

        return hasOddNumberOfVerticalPipes(strRowBefore) && hasOddNumberOfVerticalPipes(strRowAfter);
    }

    private static int GetPath(char[][] map, out List<(int x, int y)> path, out char startNode)
    {
        var startingPosition = FindStartingPosition(map);
        var loops = new Dictionary<Direction, Step>();

        var startingDirections = new[] { Direction.West, Direction.East, Direction.North, Direction.South };
        foreach (var startingDirection in startingDirections)
        {
            if (TryGetNext(map, startingPosition.x, startingPosition.y, mirror[startingDirection], out var nextPos, out var nextDir))
            {
                loops.Add(startingDirection, new Step(nextPos, nextDir));
            }
        }


        while (true)
        {
            var loopsToRemove = new List<Direction>();
            foreach (var loop in loops)
            {
                var step = loop.Value;
                if (TryGetNext(map, step.X, step.Y, step.NextFrom, out var nextPos, out var nextFrom))
                {
                    var otherEnd = loops.Where(l => l.Value.X == nextPos.x && l.Value.Y == nextPos.y);
                    if (otherEnd.Any())
                    {
                        var x = otherEnd.Single();
                        startNode = connections.Where(c => c.Value.Contains(x.Key) && c.Value.Contains(loop.Key)).Single().Key;
                        path = loop.Value.History.Concat(otherEnd.Single().Value.History).ToList();
                        return loop.Value.Steps + 1;
                    }

                    step.Set(nextPos, nextFrom);
                }
                else
                {
                    loopsToRemove.Add(loop.Key);
                }
            }

            foreach (var loop in loopsToRemove)
            {
                loops.Remove(loop);
            }
        }
    }

    private static (int x, int y) FindStartingPosition(char[][] map)
    {
        for (int y = 0; y < map.Length; y++)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                if (map[y][x] == 'S')
                {
                    return (x, y);
                }
            }
        }

        throw new ApplicationException("Start not found");
    }

    private static bool TryGetNext(char[][] map, int x, int y, Direction from, out (int x, int y) next, out Direction nextFrom)
    {
        nextFrom = Direction.NotSet;
        next = from switch
        {
            Direction.East => (x - 1, y),
            Direction.West => (x + 1, y),
            Direction.North => (x, y + 1),
            Direction.South => (x, y - 1),
            _ => throw new ApplicationException("Unknown direction")
        };

        if (IsConnected(map, next.x, next.y, from, out var to))
        {
            nextFrom = mirror[to];
            return true;
        }

        return false;
    }

    private static bool IsConnected(char[][] map, int x, int y, Direction from, out Direction to)
    {
        to = Direction.NotSet;
        if (y < 0 || y >= map.Length
            || x < 0 || x >= map[y].Length)
        {
            return false;
        }

        if (connections.TryGetValue(map[y][x], out var value))
        {
            if (value.Contains(from))
            {
                to = value.Where(v => v != from).Single();
                return true;
            }
        }

        return false;
    }

    private enum Direction
    {
        NotSet,
        West,
        East,
        North,
        South
    }

    private class Step
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Steps { get; set; }
        public Direction NextFrom { get; set; }
        public List<(int x, int y)> History { get; set; } = new List<(int x, int y)>();

        public Step((int x, int y) nextPos, Direction nextFrom)
        {
            Set(nextPos, nextFrom);
        }

        public void Set((int x, int y) nextPos, Direction nextFrom)
        {
            X = nextPos.x;
            Y = nextPos.y;
            NextFrom = nextFrom;
            Steps++;
            History.Add(nextPos);
        }
    }
}