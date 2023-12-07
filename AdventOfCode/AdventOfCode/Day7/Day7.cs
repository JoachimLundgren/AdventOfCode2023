using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using AdventOfCode;

internal class Day7
{
    public static void Run()
    {
        var hands = ParseInput("Day7/input.txt");
        System.Console.WriteLine(GetPoints(hands));


        var hands2 = ParseInput("Day7/input.txt", jokers: true);
        System.Console.WriteLine(GetPoints(hands2));
    }

    private static List<Hand> ParseInput(string fileName, bool jokers = false)
    {
        var lines = File.ReadAllLines(fileName);
        return lines.Select(l => new Hand(l, jokers)).ToList();
    }

    private static long GetPoints(List<Hand> hands)
    {
        var order = hands.OrderBy(h => h.Rank);

        long res = 0;
        for (int i = 0; i < order.Count(); i++)
        {
            var hand = order.ElementAt(i);
            res += hand.Bid * (i + 1);
        }

        return res;
    }

    private class Hand
    {
        public List<int> Cards { get; init; }
        public int Bid { get; init; }
        public long Rank { get; set; }

        private const int highCard = 1;
        private const int pair = 2;
        private const int twoPair = 3;
        private const int threeOfAKind = 4;
        private const int fullHouse = 5;
        private const int fourOfAKind = 6;
        private const int fiveOfAKind = 7;

        public Hand(string line, bool jokers)
        {
            var parts = line.Split(' ');
            Cards = parts[0].Select(c => c.ToCardValue(jokers)).ToList();
            Bid = int.Parse(parts[1]);
            Rank = GetRank();
        }

        private long GetRank()
        {
            var rank = GetHandRank() * 10000000000
                + Cards.ElementAt(0) * 100000000
                + Cards.ElementAt(1) * 1000000
                + Cards.ElementAt(2) * 10000
                + Cards.ElementAt(3) * 100
                + Cards.ElementAt(4);

            return rank;
        }

        private int GetHandRank()
        {
            var jokers = Cards.Count(c => c == 1);
            var grouping = Cards.GroupBy(c => c);
            var uniqueCards = grouping.Count();
            if (uniqueCards == 5)  //High card
            {
                if (jokers == 0)
                {
                    return highCard;
                }
                return pair;   //One joker, best we can do is a pair
            }
            else if (uniqueCards == 4)  //One pair
            {
                if (jokers == 0)
                {
                    return pair;
                }
                return threeOfAKind;   //One or Two jokers. Best we can do is 3 of a kind
            }
            else if (uniqueCards == 3)  //Two pair or Three of a kind
            {
                if (jokers == 0)
                {
                    if (grouping.Any(g => g.Count() == 2))
                    {
                        return twoPair;
                    }
                    else
                    {
                        return threeOfAKind;
                    }
                }
                else if (jokers == 1)
                {
                    if (grouping.Any(g => g.Count() == 2))
                    {
                        return fullHouse;   //JXXYY best we can do is Full House
                    }
                    else
                    {
                        return fourOfAKind;   //JXXXY best we can do is Four of a kind
                    }
                }
                else if (jokers == 2)
                {
                    if (grouping.Any(g => g.Count() == 2))
                    {
                        return fourOfAKind;   //JJXXY best we can do is Four of a kind
                    }
                }
                return fourOfAKind; //JJJXY best we can do is Four of a kind
            }
            else if (uniqueCards == 2)  //Full house or Four of a kind
            {
                if (jokers == 0)
                {

                    if (grouping.Any(g => g.Count() == 3))
                    {
                        return fullHouse;
                    }
                    else
                    {
                        return fourOfAKind;
                    }
                }
                else
                {
                    return fiveOfAKind;   //At least one is joker, rest is same!
                }
            }
            else if (uniqueCards == 1)   //5 of a kind
            {
                return fiveOfAKind;
            }

            throw new ApplicationException("Unknown card sequence");
        }
    }
}