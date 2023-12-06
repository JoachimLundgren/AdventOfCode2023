

internal class Day6
{
    public static void Run()
    {
        var races1 = ParseInput("Day6/input.txt");
        System.Console.WriteLine($"part 1: {Part1(races1)}");

        var race2 = ParseInput2("Day6/input.txt");
        System.Console.WriteLine($"part 2: {race2.GetResult()}");
    }

    private static List<Race> ParseInput(string fileName)
    {
        var lines = File.ReadAllLines(fileName);
        var times = lines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var distances = lines[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var races = new List<Race>();
        for (int i = 1; i < times.Count(); i++)
        {
            races.Add(new Race { Time = int.Parse(times[i]), Distance = int.Parse(distances[i]) });
        }
        return races;
    }

    private static Race ParseInput2(string fileName)
    {
        var lines = File.ReadAllLines(fileName);
        var time = lines[0].Split(':', StringSplitOptions.RemoveEmptyEntries)[1];
        var distance = lines[1].Split(':', StringSplitOptions.RemoveEmptyEntries)[1];

        return new Race
        {
            Time = long.Parse(time.Replace(" ", "")),
            Distance = long.Parse(distance.Replace(" ", ""))
        };
    }

    private static long Part1(List<Race> races)
    {
        return races
            .Select(r => r.GetResult())
            .Aggregate((a, x) => a * x);
    }

    private class Race
    {
        public long Time { get; set; }
        public long Distance { get; set; }

        public long GetResult()
        {
            var firstFound = false;
            long min = 0;
            long max = 0;

            for (long hold = 1; hold < Distance; hold++)
            {
                var travel = hold * (Time - hold);
                if (travel > Distance && !firstFound)
                {
                    min = hold;
                    firstFound = true;
                }
                else if (travel <= Distance && firstFound)
                {
                    max = hold;
                    break;
                }
            }

            return max - min;
        }
    }
}