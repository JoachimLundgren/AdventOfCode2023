using System.ComponentModel.DataAnnotations;
using AdventOfCode;

internal class Day4
{
    public static void Run()
    {
        var input1 = ParseInput("Day4/input.txt");
        System.Console.WriteLine($"part 1: {Part1(input1)}");

        var input2 = ParseInput("Day4/input.txt");
        System.Console.WriteLine($"part 2: {Part2(input2)}");
    }

    private static List<Card> ParseInput(string fileName)
    {
        var cards = new List<Card>();

        var lines = File.ReadAllLines(fileName);
        foreach (var line in lines)
        {
            cards.Add(new Card(line));
        }

        return cards;
    }

    private class Card
    {
        public List<int> Winners { get; init; }
        public List<int> Yours { get; init; }

        public Card(string line)
        {
            var numbers = line.Split(':')[1].Split('|');
            Winners = numbers[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s)).ToList();
            Yours = numbers[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s)).ToList();
        }

        public int WinningNumbers()
        {
            return Yours.Where(n => Winners.Contains(n)).Count();
        }

        public int GetPoints()
        {
            var winningNumbers = WinningNumbers();

            if (winningNumbers == 0)
            {
                return 0;
            }
            return (int)Math.Pow(2, winningNumbers - 1);
        }
    }

    private static int Part1(List<Card> cards)
    {
        return cards.Sum(c => c.GetPoints());
    }

    private static int Part2(List<Card> cards)
    {
        var numberOfCards = cards.ToDictionary(c => cards.IndexOf(c), c => 1);

        for (int i = 0; i < cards.Count; i++)
        {
            var winners = cards.ElementAt(i).WinningNumbers();
            var copies = numberOfCards[i];
            for (int j = 1; j <= winners; j++)
            {
                if (i + j < cards.Count)
                {
                    numberOfCards[i + j] += copies;
                }
            }
        }

        return numberOfCards.Values.Sum();
    }
}