using System.IO.Compression;
using System.Text;
using AdventOfCode;

internal class Day17
{
    public static void Run()
    {
        System.Console.WriteLine(Part(1, ParseInput("Day17/testinput.txt")));
        System.Console.WriteLine(Part(1, ParseInput("Day17/input.txt"))); //956
        System.Console.WriteLine(Part(2, ParseInput("Day17/testinput.txt")));
        System.Console.WriteLine(Part(2, ParseInput("Day17/testinput2.txt")));
        System.Console.WriteLine(Part(2, ParseInput("Day17/input.txt")));   //1084 - 1111
    }

    private static int[][] ParseInput(string fileName)
    {
        return File.ReadAllLines(fileName)
            .Select(line => line.ToCharArray().Select(c => c - '0').ToArray())
            .ToArray();
    }

    private static long Part(int part, int[][] heatmap)
    {
        (int row, int col) bottomRight = (heatmap.Length - 1, heatmap[0].Length - 1);

        var visited = new HashSet<(int row, int col, int distance, Direction direction)>();
        var activePaths = new PriorityQueue<Path, int>();
        activePaths.Enqueue(new Path(0, 0, 0, Direction.Down), 0);
        activePaths.Enqueue(new Path(0, 0, 0, Direction.Right), 0);

        Path bestMove = null;
        while (true)
        {
            if (!activePaths.TryDequeue(out var current, out var prio))
            {
                //PrintPath(heatmap, bestMove.History);
                return bestMove.Heat;
            }

            var possibleMoves = current.GetPossibleMoves(part);
            var nextMoves = possibleMoves.Where(pm =>
                pm.Row >= 0 && pm.Row < heatmap.Length
                && pm.Column >= 0 && pm.Column < heatmap[0].Length
                && !visited.Contains(pm.Key));

            foreach (var move in nextMoves)
            {
                move.Heat = current.Heat + heatmap[move.Row][move.Column];

                if (move.Row == bottomRight.row && move.Column == bottomRight.col && move.CanComplete(part))
                {
                    if (bestMove == null || bestMove.Heat > move.Heat)
                    {
                        bestMove = move;
                    };
                }

                activePaths.Enqueue(move, move.Heat);
                visited.Add(move.Key);
            }
        }
    }

    private static void PrintPath(int[][] heatmap, List<(int row, int col)> history)
    {
        for (int row = 0; row < heatmap.Length; row++)
        {
            for (int col = 0; col < heatmap[row].Length; col++)
            {
                string c = history.Any(h => h.row == row && h.col == col) ? "#" : heatmap[row][col].ToString();
                System.Console.Write(c);
            }
            System.Console.WriteLine();
        }
    }

    private class Path
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int Heat { get; set; }
        public int Distance { get; set; }
        public Direction Direction { get; set; }

        //Temp
        public List<(int row, int column)> History { get; set; } = new List<(int row, int column)>();

        public (int row, int col, int distance, Direction direction) Key => (Row, Column, Distance, Direction);

        public Path(int row, int col, int distance, Direction direction)
        {
            Row = row;
            Column = col;
            Distance = distance;
            Direction = direction;
        }

        public Path(int row, int col, int distance, Direction direction, List<(int row, int column)> history)
        {
            Row = row;
            Column = col;
            Distance = distance;
            Direction = direction;
            History = history;
        }

        public List<Path> GetPossibleMoves(int part)
        {
            var res = new List<Path>();

            var minMove = part == 1 ? 0 : 4;
            var maxMove = part == 1 ? 3 : 10;
            if (Distance < maxMove)
            {
                var move = GetMove(Direction);
                var history = History.ToList();
                history.Add((Row + move.row, Column + move.col));
                res.Add(new Path(Row + move.row, Column + move.col, Distance + 1, Direction, history));
            }

            if (Distance >= minMove)
            {
                var leftDir = TurnLeft(Direction);
                var leftMove = GetMove(leftDir);
                var history2 = History.ToList();
                history2.Add((Row + leftMove.row, Column + leftMove.col));
                res.Add(new Path(Row + leftMove.row, Column + leftMove.col, 1, leftDir, history2));

                var rightDir = TurnRight(Direction);
                var rightMove = GetMove(rightDir);
                var history3 = History.ToList();
                history3.Add((Row + rightMove.row, Column + rightMove.col));
                res.Add(new Path(Row + rightMove.row, Column + rightMove.col, 1, rightDir, history3));
            }

            return res;
        }

        public bool CanComplete(int part)
        {
            return part == 1 || (Distance >= 4 && Distance <= 10);
        }

        private (int row, int col) GetMove(Direction dir)
        {
            return dir switch
            {
                Direction.Up => (-1, 0),
                Direction.Down => (1, 0),
                Direction.Left => (0, -1),
                Direction.Right => (0, 1),
                _ => throw new NotImplementedException(),
            };
        }

        private Direction TurnLeft(Direction dir)
        {
            return dir switch
            {
                Direction.Up => Direction.Left,
                Direction.Left => Direction.Down,
                Direction.Down => Direction.Right,
                Direction.Right => Direction.Up,
                _ => throw new NotImplementedException(),
            };
        }

        private Direction TurnRight(Direction dir)
        {
            return dir switch
            {
                Direction.Up => Direction.Right,
                Direction.Right => Direction.Down,
                Direction.Down => Direction.Left,
                Direction.Left => Direction.Up,
                _ => throw new NotImplementedException(),
            };
        }
    }

    private enum Direction
    {
        Up,
        Down,
        Left,
        Right,
    }
}