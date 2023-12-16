using System.Data;

internal class Day13
{
    public static void Run()
    {
        var input = ParseInput("Day13/input.txt");
        System.Console.WriteLine(Part1(input));
        System.Console.WriteLine(Part2(input));
    }

    private static List<Terrain> ParseInput(string fileName)
    {
        var res = new List<Terrain>();
        var lines = File.ReadAllLines(fileName);
        int i = 0;

        while (i < lines.Length)
        {
            var terrainLines = lines.Skip(i).TakeWhile(l => l != string.Empty);
            res.Add(new Terrain(terrainLines));
            i += terrainLines.Count() + 1;
        }

        return res;
    }

    private static long Part1(List<Terrain> terrains)
    {
        return terrains.Sum(t => t.GetScore());
    }

    private static long Part2(List<Terrain> terrains)
    {
        return terrains.Sum(t => t.FixSmudgeAndGetScore());
    }

    private class Terrain
    {
        public char[][] Pattern { get; init; }

        public Terrain(IEnumerable<string> lines)
        {
            Pattern = lines.Select(line => line.ToCharArray()).ToArray();
        }

        public int GetScore()
        {
            return FindMirror(Pattern) * 100 + FindMirror(ListUtils.Transpose(Pattern));
        }

        public int FixSmudgeAndGetScore()
        {
            var oldHorizontalMirror = FindMirror(Pattern);
            var oldVerticalMirror = FindMirror(ListUtils.Transpose(Pattern));

            var pattern = ListUtils.Copy(Pattern);

            for (var row = 0; row < pattern.Length; row++)
            {
                for (var column = 0; column < pattern[row].Length; column++)
                {
                    var olcChar = pattern[row][column];
                    pattern[row][column] = olcChar == '.' ? '#' : '.';
                    var score = FindMirror(pattern, oldHorizontalMirror) * 100
                              + FindMirror(ListUtils.Transpose(pattern), oldVerticalMirror);

                    if (score != 0)
                    {
                        return score;
                    }

                    pattern[row][column] = olcChar;
                }
            }

            throw new ApplicationException("No smudge found");
        }

        private int FindMirror(char[][] pattern, int ignoreMirror = 0)
        {
            for (int i = 1; i < pattern.Length; i++)
            {
                if (i == ignoreMirror)
                {
                    continue;
                }

                var mirrored = true;
                var first = i - 1;
                var second = i;
                while (mirrored && first >= 0 && second < pattern.Length)
                {
                    mirrored = IsMirrored(pattern[first--], pattern[second++]);
                }

                if (mirrored)
                {
                    return i;
                }
            }

            return 0;
        }

        private bool IsMirrored(char[] first, char[] second)
        {
            for (int i = 0; i < first.Length; i++)
            {
                if (first[i] != second[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}