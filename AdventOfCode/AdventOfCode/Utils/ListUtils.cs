public static class ListUtils
{
    public static void PrintMatrix(IEnumerable<IEnumerable<char>> matrix)
    {
        for (int y = 0; y < matrix.Count(); y++)
        {
            Console.WriteLine(new string(matrix.ElementAt(y).ToArray()));
        }
    }

    public static T[][] Copy<T>(T[][] matrix)
    {
        return matrix.Select(m => m.ToArray()).ToArray();
    }

    public static T[][] Transpose<T>(T[][] matrix)
    {
        var rows = matrix.Length;
        var columns = matrix[0].Length;

        var result = new T[columns][];

        for (var c = 0; c < columns; c++)
        {
            var row = new T[rows];
            for (var r = 0; r < rows; r++)
            {
                row[r] = matrix[r][c];
            }
            result[c] = row;
        }

        return result;
    }

    public static T[][] Rotate<T>(T[][] matrix)
    {
        var res = Transpose(matrix);
        return res.Select(r => r.Reverse().ToArray()).ToArray();
    }

    public static long Count<T>(IEnumerable<IEnumerable<T>> matrix, T obj)
    {
        return matrix.Sum(line => line.LongCount(l => l != null && l.Equals(obj)));
    }

    public static List<Coordinate> GetCoordinates<T>(IEnumerable<IEnumerable<T>> matrix, T obj)
    {
        var res = new List<Coordinate>();

        for (int y = 0; y < matrix.Count(); y++)
        {
            var columns = matrix.ElementAt(y).Count();
            for (int x = 0; x < columns; x++)
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                if (matrix.ElementAt(y).ElementAt(x).Equals(obj))
                {
                    res.Add(new Coordinate(x, y));
                }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
        }

        return res;
    }

    public class Coordinate
    {
        public long X { get; set; }
        public long Y { get; set; }
        public Coordinate(long x, long y)
        {
            X = x;
            Y = y;
        }
    }
}