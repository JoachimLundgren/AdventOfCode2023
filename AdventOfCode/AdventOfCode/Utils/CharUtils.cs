public static class CharUtils
{
    public static void PrintMatrix(char[][] matrix)
    {
        for (int y = 0; y < matrix.Length; y++)
        {
            Console.WriteLine(new string(matrix[y]));
        }
    }
}