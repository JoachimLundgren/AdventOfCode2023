public static class ListUtils
{
    public static void PrintMatrix(IEnumerable<IEnumerable<char>> matrix)
    {
        for (int y = 0; y < matrix.Count(); y++)
        {
            Console.WriteLine(new string(matrix.ElementAt(y).ToArray()));
        }
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