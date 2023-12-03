internal class Day2
{
    public static void Run()
    {
        var games1 = ParseInput("Day2/input.txt");
        System.Console.WriteLine($"part 1: {Part1(games1)}");


        var games2 = ParseInput("Day2/input.txt");
        System.Console.WriteLine($"part 2: {Part2(games2)}");
    }

    private static int Part1(List<Game> games)
    {
        return games
            .Where(g => g.Subsets
                .All(s => s.VerifyColor("red", 12)
                        && s.VerifyColor("green", 13)
                        && s.VerifyColor("blue", 14)))
            .Sum(g => g.Id);

    }

    private static int Part2(List<Game> games)
    {
        return games.Sum(g => g.Power());

    }

    private static List<Game> ParseInput(string fileName)
    {
        var lines = File.ReadAllLines(fileName);
        return lines.Select(l => new Game(l)).ToList();
    }

    private class Game
    {
        public int Id { get; set; }
        public List<Subset> Subsets { get; set; }

        public Game(string line)
        {
            var parts = line.Split(':');

            Id = int.Parse(parts[0].Substring(5));
            Subsets = parts[1].Split(';').Select(p => new Subset(p)).ToList();
        }

        public int Power()
        {
            var reds = Subsets.Select(s => s.Cubes.GetValueOrDefault("red")).Max();
            var blues = Subsets.Select(s => s.Cubes.GetValueOrDefault("blue")).Max();
            var greens = Subsets.Select(s => s.Cubes.GetValueOrDefault("green")).Max();

            return reds * blues * greens;
        }
    }

    private class Subset
    {
        public Dictionary<string, int> Cubes { get; set; }

        public Subset(string subset)
        {
            Cubes = new Dictionary<string, int>();
            var cubes = subset.Split(',');
            foreach (var cube in cubes)
            {
                var parts = cube.Trim().Split(' ');
                Cubes.Add(parts[1], int.Parse(parts[0]));
            }
        }

        public bool VerifyColor(string color, int amount)
        {
            return Cubes.GetValueOrDefault(color) <= amount;
        }
    }
}