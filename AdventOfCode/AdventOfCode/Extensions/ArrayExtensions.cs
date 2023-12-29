namespace AdventOfCode;

public static class ArrayExtensions
{
    public static bool IsInside<T>(this T[][] matrix, int row, int column)
    {
        return row >= 0
            && row < matrix.Length
            && column >= 0
            && column < matrix[row].Length;
    }

    public static List<(int row, int column)> GetAdjecant<T>(this T[][] matrix, int row, int column, bool diagonal = true)
    {
        var adjacent = new List<(int row, int column)>()
        {
            (row - 1, column),
            (row, column - 1),
            (row, column + 1),
            (row + 1, column),
        };

        if (diagonal)
        {
            adjacent.Add((row - 1, column - 1));
            adjacent.Add((row - 1, column + 1));
            adjacent.Add((row + 1, column - 1));
            adjacent.Add((row + 1, column + 1));
        }

        return adjacent
            .Where(c => matrix.IsInside(c.row, c.column))
            .ToList();
    }
}
