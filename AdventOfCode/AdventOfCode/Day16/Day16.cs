using System.Text;
using AdventOfCode;

internal class Day16
{
    public static void Run()
    {
        System.Console.WriteLine(Part1(ParseInput("Day16/input.txt")));
        System.Console.WriteLine(Part2(ParseInput("Day16/input.txt")));
    }

    private static char[][] ParseInput(string fileName)
    {
        return File.ReadAllLines(fileName)
            .Select(line => line.ToCharArray())
            .ToArray();
    }

    private static long Part1(char[][] layout)
    {
        return GetConfigurationValue(layout, new Beam(0, -1, Direction.Right));
    }

    private static long Part2(char[][] layout)
    {
        long maxPoints = 0;

        for (int i = 0; i < layout.Length; i++)
        {
            var points = GetConfigurationValue(layout, new Beam(-1, i, Direction.Down));
            if (points > maxPoints)
            {
                System.Console.WriteLine(points);
                maxPoints = points;
            }
            
            points = GetConfigurationValue(layout, new Beam(layout[i].Length, i, Direction.Up));
            if (points > maxPoints)
            {
                System.Console.WriteLine(points);
                maxPoints = points;
            }
        }

        System.Console.WriteLine("Halfway there!");

        for (int i = 0; i < layout[0].Length; i++)
        {
            var points = GetConfigurationValue(layout, new Beam(i, -1, Direction.Right));
            if (points > maxPoints)
            {
                System.Console.WriteLine(points);
                maxPoints = points;
            }
            
            points = GetConfigurationValue(layout, new Beam(i, layout.Length, Direction.Left));
            if (points > maxPoints)
            {
                System.Console.WriteLine(points);
                maxPoints = points;
            }
        }

        return maxPoints;
    }

    private static long GetConfigurationValue(char[][] layout, Beam start)
    {
        var visitedPoints = new List<(int row, int column, Direction dir)>();
        var beams = new List<Beam>() { start };

        var first = true;
        while (beams.Any())
        {
            if (!first)
            {
                var duplicates = beams.Where(b => visitedPoints.Any(p => p.row == b.Row && p.column == b.Column && p.dir == b.Direction)).ToList();
                foreach (var beam in duplicates)
                {
                    beams.Remove(beam);
                }

                var newPoints = beams.Select(b => (b.Row, b.Column, b.Direction));
                visitedPoints.AddRange(newPoints);
            }
            else
            {
                first = false;
            }

            foreach (var beam in beams.ToList())
            {
                var next = beam.GetNextCoordinate();
                if (layout.IsInside(next.row, next.column))
                {
                    var p = layout[next.row][next.column];
                    if (p == '.')
                    {
                        beam.Move(next.row, next.column, beam.Direction);
                    }
                    else if (p == '/')
                    {
                        var nextDirection = beam.Direction switch
                        {
                            Direction.Right => Direction.Up,
                            Direction.Up => Direction.Right,
                            Direction.Left => Direction.Down,
                            Direction.Down => Direction.Left,
                            _ => throw new ApplicationException("Unknown direction")
                        };
                        beam.Move(next.row, next.column, nextDirection);
                    }
                    else if (p == '\\')
                    {
                        var nextDirection = beam.Direction switch
                        {
                            Direction.Right => Direction.Down,
                            Direction.Up => Direction.Left,
                            Direction.Left => Direction.Up,
                            Direction.Down => Direction.Right,
                            _ => throw new ApplicationException("Unknown direction")
                        };
                        beam.Move(next.row, next.column, nextDirection);
                    }
                    else if (p == '|')
                    {
                        if (beam.Direction == Direction.Right || beam.Direction == Direction.Left)
                        {
                            beam.Move(next.row, next.column, Direction.Up);
                            beams.Add(new Beam(next.row, next.column, Direction.Down));
                        }
                        else
                        {
                            beam.Move(next.row, next.column, beam.Direction);
                        }
                    }
                    else if (p == '-')
                    {
                        if (beam.Direction == Direction.Up || beam.Direction == Direction.Down)
                        {
                            beam.Move(next.row, next.column, Direction.Right);
                            beams.Add(new Beam(next.row, next.column, Direction.Left));
                        }
                        else
                        {
                            beam.Move(next.row, next.column, beam.Direction);
                        }
                    }
                    else
                    {
                        throw new ApplicationException("huh?");
                    }
                }
                else
                {
                    beams.Remove(beam);
                }
            }

            //Print(visitedPoints, layout.Length, layout[0].Length, sleep: 10);
        }

        return visitedPoints.Select(p => (p.row, p.column)).Distinct().Count();
    }

    private static void Print(List<(int row, int column, Direction)> allPoints, int rows, int columns, int sleep = 0)
    {
        System.Console.Clear();
        var visitedPoints = allPoints.Select(p => (p.row, p.column)).Distinct();

        for (int row = 0; row < rows; row++)
        {
            System.Console.Write(row);
            var s = new StringBuilder(columns);
            for (var col = 0; col < columns; col++)
            {
                s.Append(visitedPoints.Contains((row, col)) ? '#' : '.');
            }
            System.Console.WriteLine(s.ToString());
        }
        Thread.Sleep(sleep);
    }

    private class Beam
    {

        public int Row { get; set; }
        public int Column { get; set; }

        public Direction Direction { get; set; }

        public Beam(int r, int c, Direction direction)
        {
            Row = r;
            Column = c;
            Direction = direction;
        }

        public (int row, int column) GetNextCoordinate()
        {
            return Direction switch
            {
                Direction.Right => (Row, Column + 1),
                Direction.Up => (Row - 1, Column),
                Direction.Left => (Row, Column - 1),
                Direction.Down => (Row + 1, Column),
                _ => throw new ApplicationException("Unknown direction")
            };
        }

        internal void Move(int row, int column, Direction direction)
        {
            Row = row;
            Column = column;
            Direction = direction;
        }
    }

    private enum Direction
    {
        Right,
        Up,
        Left,
        Down,
    }
}