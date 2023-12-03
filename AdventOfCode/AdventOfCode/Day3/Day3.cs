using AdventOfCode;

internal class Day3
{
    public static void Run()
    {
        var lines1 = File.ReadAllLines("Day3/input.txt").Select(l => l.ToCharArray()).ToArray();
        System.Console.WriteLine($"part 1: {Part1(lines1)}");


        var lines2 = File.ReadAllLines("Day3/input.txt").Select(l => l.ToCharArray()).ToArray();
        System.Console.WriteLine($"part 2: {Part2(lines2)}");
    }

    private static int Part1(char[][] lines)
    {
        var result = 0;
        for (int row = 0; row < lines.Length; row++)
        {
            var currentNumber = new Dictionary<(int row, int column), char>();
            for (int column = 0; column < lines[row].Length; column++)
            {
                if (char.IsDigit(lines[row][column]))
                {
                    currentNumber.Add((row, column), lines[row][column]);
                }
                else
                {
                    result += GetValue(currentNumber, lines);
                    currentNumber.Clear();
                }
            }
            result += GetValue(currentNumber, lines);
        }

        return result;
    }

    private static int Part2(char[][] lines)
    {
        var result = 0;
        for (int row = 0; row < lines.Length; row++)
        {
            for (int column = 0; column < lines[row].Length; column++)
            {
                if (lines[row][column] == '*')
                {
                    result += GetGearValue(row, column, lines);
                }
            }
        }

        return result;
    }

    private static int GetValue(Dictionary<(int row, int column), char> currentNumber, char[][] lines)
    {
        if (currentNumber.Any(num => IsAdjacentToSymbol(num.Key.row, num.Key.column, lines)))
        {
            return int.Parse(new string(currentNumber.Values.ToArray()));
        }

        return 0;
    }

    private static bool IsAdjacentToSymbol(int row, int column, char[][] lines)
    {
        var adjacentChars = lines.GetAdjecant(row, column);
        return adjacentChars.Any(c => IsSymbol(lines[c.row][c.column]));
    }



    private static bool IsSymbol(char c)
    {
        return c != '.' && !char.IsDigit(c);
    }

    private static int GetGearValue(int row, int column, char[][] lines)
    {
        var numbers = new List<int>();
        for (int i = row - 1; i <= row + 1; i++)
        {
            for (int j = column - 1; j <= column + 1; j++)
            {
                if (lines.IsInside(row, column) && char.IsDigit(lines[i][j]))
                {
                    var number = GetNumber(i,j,lines, out var wasLastChar);
                    numbers.Add(number);
                    if (!wasLastChar)
                    {
                        break;
                    }
                }
            }
        }

        if (numbers.Count == 2)
        {
            return numbers.First() * numbers.Last();
        }

        return 0;
    }

    private static int GetNumber(int row, int column, char[][] lines, out bool wasLastChar)
    {
        var num = new List<char>() { lines[row][column] };
        var newColumn = column - 1;
        while (lines.IsInside(row, newColumn) && char.IsDigit(lines[row][newColumn]))
        {
            num.Insert(0, lines[row][newColumn]);
            newColumn--;
        }

        wasLastChar = true;
        newColumn = column + 1;
        while (lines.IsInside(row, newColumn) && char.IsDigit(lines[row][newColumn]))
        {
            wasLastChar = false;
            num.Add(lines[row][newColumn]);
            newColumn++;
        }

        return int.Parse(new string(num.ToArray()));
    }
}