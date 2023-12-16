using System.Data;

internal class Day14
{
    public static void Run()
    {
        System.Console.WriteLine(Part1(ParseInput("Day14/input.txt")));
        System.Console.WriteLine(Part2(ParseInput("Day14/input.txt")));
    }

    private static Platform ParseInput(string fileName)
    {
        return new Platform(File.ReadAllLines(fileName));
    }

    private static long Part1(Platform platform)
    {
        platform.Tilt();
        return platform.GetScore();
    }

    private static long Part2(Platform platform)
    {
        for (int i = 0; i < 1000; i++)  //I did it 1000 times and then looked at the output. Realizing a pattern.
        {
            platform.Cycle();
        }
        
        return platform.GetScore();
    }

    private class Platform
    {
        public char[][] Pattern { get; set; }

        public Platform(IEnumerable<string> lines)
        {
            Pattern = lines.Select(line => line.ToCharArray()).ToArray();
        }

        public void Tilt()
        {
            for (var row = 0; row < Pattern.Length; row++)
            {
                for (var column = 0; column < Pattern[row].Length; column++)
                {
                    if (Pattern[row][column] == 'O')
                    {
                        for (var j = 1; j <= row; j++)
                        {
                            if (Pattern[row - j][column] == '.')
                            {
                                Pattern[row - j][column] = 'O';
                                Pattern[row - j + 1][column] = '.';
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void Cycle()
        {
            Tilt();
            Pattern = ListUtils.Rotate(Pattern);
            Tilt();
            Pattern = ListUtils.Rotate(Pattern);
            Tilt();
            Pattern = ListUtils.Rotate(Pattern);
            Tilt();
            Pattern = ListUtils.Rotate(Pattern);
        }

        public int GetScore()
        {
            var res = 0;

            var rowValue = Pattern.Count();
            foreach (var row in Pattern)
            {
                res += row.Count(c => c == 'O') * rowValue;
                rowValue--;
            }

            return res;
        }
    }
}