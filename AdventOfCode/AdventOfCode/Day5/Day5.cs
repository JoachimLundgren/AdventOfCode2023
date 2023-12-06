

internal class Day5
{
    public static void Run()
    {
        var almanac1 = ParseInput("Day5/input.txt");
        System.Console.WriteLine($"part 1: {almanac1.Part1()}");

        var almanac2 = ParseInput("Day5/input.txt");
        System.Console.WriteLine($"part 2: {almanac2.Part2()}");
    }

    private static Almanac ParseInput(string fileName)
    {
        var lines = File.ReadAllLines(fileName);
        return new Almanac(lines.ToList());
    }

    private class Almanac
    {
        public List<long> Seeds { get; init; }  //Part 1
        public List<(long start, long count)> SeedRanges { get; init; } = new List<(long start, long count)>();    //Part 2
        public List<Map> Maps { get; init; } = new List<Map>();

        public Almanac(List<string> lines)
        {
            var firstLine = lines.First();
            var seeds = firstLine.Split(' ').Skip(1);
            Seeds = seeds.Select(s => long.Parse(s)).ToList();

            for (int i = 0; i < seeds.Count(); i += 2)
            {
                SeedRanges.Add((long.Parse(seeds.ElementAt(i)), long.Parse(seeds.ElementAt(i + 1))));
            }


            int mapStart = 2;
            while (mapStart < lines.Count())
            {
                var maplines = lines.Skip(mapStart).TakeWhile(l => !string.IsNullOrEmpty(l));
                Maps.Add(new Map(maplines));
                mapStart += maplines.Count() + 1;
            }
        }

        public long Part1()
        {
            var best = long.MaxValue;
            foreach (var seed in Seeds)
            {
                var newSeed = seed;
                foreach (var map in Maps)
                {
                    newSeed = map.GetDestination(newSeed);
                }

                if (newSeed < best)
                {
                    best = newSeed;
                }
            }

            return best;
        }

        public long Part2()
        {
            long i = 1;
            var mapsCopy = Maps.ToList();
            mapsCopy.Reverse();
            while (true)
            {
                var seed = GetSeedFromLocation(mapsCopy, i);
                if (SeedRanges.Any(sr => seed >= sr.start && seed < sr.start + sr.count))
                {
                    break;
                }
                //System.Console.WriteLine($"{seed} => {i}");
                i++;
            }

            return i;
        }

        private long GetSeedFromLocation(List<Map> maps, long location)
        {
            foreach (var map in maps)
            {
                location = map.GetSource(location);
            }
            return location;
        }
    }

    private class Map
    {
        public string Source { get; set; }
        public string Destination { get; set; }

        public List<Converter> Converters { get; init; } = new List<Converter>();

        public Map(IEnumerable<string> lines)
        {
            var parts = lines.First().Split('-');
            Source = parts[0];
            Destination = new string(parts[1].TakeWhile(c => c != ' ').ToArray());

            foreach (var line in lines.Skip(1))
            {
                Converters.Add(new Converter(line));
            }
        }

        public long GetDestination(long source)
        {
            foreach (var converter in Converters)
            {
                if (converter.TryGetDestination(source, out var destination))
                {
                    return destination;
                }
            }

            return source;
        }


        public long GetSource(long destination)
        {
            foreach (var converter in Converters) 
            {
                if (converter.TryGetSource(destination, out var source))
                {
                    return source;
                }
            }

            return destination;
        }
    }

    private class Converter
    {
        public long DestinationRangeStart { get; init; }
        public long SourceRangeStart { get; init; }
        public long RangeLength { get; init; }

        public Converter(string converterLine)
        {
            var parts = converterLine.Split(' ');
            DestinationRangeStart = long.Parse(parts[0]);
            SourceRangeStart = long.Parse(parts[1]);
            RangeLength = long.Parse(parts[2]);
        }

        public bool TryGetDestination(long source, out long destination)
        {
            if (source >= SourceRangeStart && source < SourceRangeStart + RangeLength)
            {
                destination = source + DestinationRangeStart - SourceRangeStart;
                return true;
            }

            destination = source;
            return false;
        }

        public bool TryGetSource(long destination, out long source)
        {
            if (destination >= DestinationRangeStart && destination < DestinationRangeStart + RangeLength)
            {
                source = destination - (DestinationRangeStart - SourceRangeStart);
                return true;
            }

            source = destination;
            return false;
        }
    }
}