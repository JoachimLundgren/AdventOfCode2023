using System.Data;

internal class Day15
{
    public static void Run()
    {
        System.Console.WriteLine(Part1(ParseInput("Day15/input.txt")));
        System.Console.WriteLine(Part2(ParseInput("Day15/input.txt")));
    }

    private static List<string> ParseInput(string fileName)
    {
        var str = File.ReadAllText(fileName);

        return str.Split(',').ToList();
    }

    private static long Part1(List<string> input)
    {
        return input.Sum(i => Hash(i));
    }

    private static long Part2(List<string> input)
    {
        var boxes = Enumerable.Range(1, 256).Select(n => new Box(n)).ToList();

        foreach (var i in input)
        {
            var parts = i.Split('-', '=');
            var label = new string(parts[0]);
            var box = boxes.ElementAt(Hash(label));

            if (i.Contains('-'))
            {
                box.RemoveLens(label);
            }
            else
            {
                var focalLength = int.Parse(parts[1]);
                box.AddLens(label, focalLength);
            }
        }

        return boxes.Sum(b => b.GetBoxValue());
    }

    private static int Hash(string input)
    {
        var currentValue = 0;

        foreach (var c in input)
        {
            currentValue += c;
            currentValue *= 17;
            currentValue %= 256;
        }

        return currentValue;
    }

    private class Box
    {
        public int Index { get; init; }
        public List<Lens> Lenses { get; set; } = new List<Lens>();

        public Box(int index)
        {
            Index = index;
        }

        public void RemoveLens(string label)
        {
            var lens = Lenses.SingleOrDefault(l => l.Label == label);
            if (lens != default)
            {
                Lenses.Remove(lens);
            }
        }

        public void AddLens(string label, int focalLength)
        {
            var lens = Lenses.SingleOrDefault(l => l.Label == label);
            if (lens != default)
            {
                lens.FocalLength = focalLength;
            }
            else
            {
                Lenses.Add(new Lens(label, focalLength));
            }
        }

        public long GetBoxValue()
        {
            long value = 0;
            for (int i = 0; i < Lenses.Count(); i++)
            {
                var lens = Lenses.ElementAt(i);
                value += Index * (i + 1) * lens.FocalLength;
            }
            return value;
        }
    }
    private class Lens
    {
        public string Label { get; set; }

        public int FocalLength { get; set; }

        public Lens(string label, int focalLength)
        {
            Label = label;
            FocalLength = focalLength;
        }
    }
}