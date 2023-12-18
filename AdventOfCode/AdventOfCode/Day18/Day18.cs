using System.IO.Compression;
using System.Text;
using AdventOfCode;

internal class Day18
{
    public static void Run()
    {
        System.Console.WriteLine(Part1(ParseInput("Day18/testinput.txt")));
        System.Console.WriteLine(Part1(ParseInput("Day18/input.txt")));
        System.Console.WriteLine(Part2(ParseInput("Day18/testinput.txt")));
        System.Console.WriteLine(Part2(ParseInput("Day18/input.txt")));
    }

    private static List<Instruction> ParseInput(string fileName)
    {
        return File.ReadAllLines(fileName)
            .Select(line => new Instruction(line))
            .ToList();
    }

    private static long Part1(List<Instruction> digPlan)
    {
        var (terrain, current) = GenerateTerrain(digPlan);

        foreach (var instruction in digPlan)
        {
            var movement = GetMovement(instruction.Direction);
            for (int i = 0; i < instruction.Distance; i++)
            {
                current.row = current.row + movement.row;
                current.col = current.col + movement.col;
                terrain[current.row][current.col] = '#';
            }
        }

        var inside = GetPointInside(terrain);
        FillInside(inside.row, inside.col, terrain);
        //ListUtils.PrintMatrix(terrain);
        return ListUtils.Count(terrain, '#');
    }

    private static long Part2(List<Instruction> digPlan)
    {
        //Never heard about Pick's theorem or the shoelace formula before. But now I have
        digPlan.ForEach(dp => dp.Part2Fix());

        (long row, long col) current = (0, 0);
        var vertices = new List<(long row, long col)>();
        var perimeter = 0L;

        foreach (var instruction in digPlan)
        {
            var movement = GetMovement(instruction.Direction);

            current.row = current.row + (movement.row * instruction.Distance);
            current.col = current.col + (movement.col * instruction.Distance);

            vertices.Add(current);
            perimeter += instruction.Distance;
        }

        var area = PolygonArea(vertices);   //Shoelace formula
        var interior = area - perimeter / 2 + 1;    //Picks Theorem

        return interior + perimeter;
    }

    private static long PolygonArea(IReadOnlyList<(long row, long col)> v)
    {
        var a = 0L;
        var n = v.Count;

        for (var i = 0; i < v.Count; i++)
        {
            a += v[i].col * v[(i + 1) % n].row - v[(i + 1) % n].col * v[i].row;
        }

        return Math.Abs(a) / 2;
    }

    private static (int row, int col) GetPointInside(char[][] terrain)
    {
        var row = terrain.Length / 2;

        var edgeFound = false;
        for (int i = 0; i < terrain[0].Length; i++)
        {
            if (terrain[row][i] == '#')
            {
                edgeFound = true;
            }
            else if (terrain[row][i] == '.')
            {
                if (edgeFound)
                {
                    return (row, i);
                }
            }
        }

        throw new ApplicationException();
    }

    private static void FillInside(int row, int col, char[][] terrain)
    {
        if (terrain[row][col] == '.')
        {
            terrain[row][col] = '#';

            FillInside(row - 1, col, terrain);
            FillInside(row + 1, col, terrain);
            FillInside(row, col - 1, terrain);
            FillInside(row, col + 1, terrain);
        }
    }

    private static (char[][] emptyTerrain, (long row, long col) startingPoint) GenerateTerrain(List<Instruction> instructions)
    {
        long maxRow = 0;
        long minRow = 0;
        long maxCol = 0;
        long minCol = 0;

        (long row, long col) current = (0, 0);

        foreach (var instruction in instructions)
        {
            var movement = GetMovement(instruction.Direction);
            current.row = current.row + movement.row * instruction.Distance;
            current.col = current.col + movement.col * instruction.Distance;

            if (current.row > maxRow) maxRow = current.row;
            if (current.row < minRow) minRow = current.row;
            if (current.col > maxCol) maxCol = current.col;
            if (current.col < minCol) minCol = current.col;
        }

        var rows = maxRow - minRow + 1;
        var cols = maxCol - minCol + 1;
        var emptyTerrain = new char[rows][];
        for (int i = 0; i < rows; i++)
        {
            emptyTerrain[i] = new string('.', (int)cols).ToCharArray();
        }
        var startingPoint = (-minRow, -minCol);

        return (emptyTerrain, startingPoint);
    }

    private static (int row, int col) GetMovement(char direction)
    {
        return direction switch
        {
            'U' => (-1, 0),
            'D' => (1, 0),
            'L' => (0, -1),
            'R' => (0, 1),
            _ => throw new NotImplementedException(),
        };
    }

    public class Instruction
    {
        public char Direction { get; set; }
        public int Distance { get; set; }
        public string Color { get; set; }

        public Instruction(string line)
        {
            var parts = line.Split(' ');
            Direction = parts[0].Single();
            Distance = int.Parse(parts[1]);
            Color = parts[2].Trim('(', ')');
        }

        public void Part2Fix()
        {
            Distance = int.Parse(Color.Substring(1, 5), System.Globalization.NumberStyles.HexNumber);   //1-6 to ignore #
            var dir = Color.Last();
            Direction = dir switch
            {
                '0' => 'R',
                '1' => 'D',
                '2' => 'L',
                '3' => 'U',
                _ => throw new NotImplementedException(),
            };
        }
    }
}