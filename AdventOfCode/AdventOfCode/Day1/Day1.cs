internal class Day1
{
    private static List<string> numbers = new List<string>() {"zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
    public static void Run()
    {
        var input1 = File.ReadAllLines("Day1/input.txt");
        System.Console.WriteLine($"part 1: {Part1(input1)}");

        var input2 = File.ReadAllLines("Day1/input.txt");
        System.Console.WriteLine($"part 2: {Part2(input2)}");
    }

    private static int Part1(string[] input)
    {
        var result = 0;

        foreach (var line in input)
        {
            var lineNumbers = line.Where(c => char.IsDigit(c));
            result += int.Parse($"{lineNumbers.First()}{lineNumbers.Last()}");
        }
        
        return result;
    }

    private static int Part2(string[] input)
    {
        var result = 0;

        foreach (var line in input)
        {
            var lineNumbers = new List<int>();
            for (int i = 0; i < line.Length; i++)
            {
                var substring = line.Substring(i);
                var number = GetNumber(substring);
                if (number != null)
                {
                    lineNumbers.Add(number.Value);
                }
            }
            result += int.Parse($"{lineNumbers.First()}{lineNumbers.Last()}");
        }

        return result;
    }

    private static int? GetNumber(string str)
    {
        if (char.IsDigit(str.First()))
        {
            return int.Parse(str.First().ToString());
        }
        else
        {
            string substring;
            for (int i = numbers.Min(n => n.Length); i <= numbers.Max(n => n.Length); i++)
            {
                if (str.Length >= i)
                {
                    substring = str.Substring(0, i);
                    if (numbers.Contains(substring))
                    {
                        return numbers.IndexOf(substring);
                    }
                }
            }
        }

        return null;
    }
}