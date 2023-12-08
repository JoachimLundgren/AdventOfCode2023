using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using AdventOfCode;

internal class Day8
{
    public static void Run()
    {
        var (instructions, nodes) = ParseInput("Day8/input.txt");
        System.Console.WriteLine(Part1(instructions, nodes));
        System.Console.WriteLine(Part2(instructions, nodes));
    }

    private static (string instructions, Dictionary<string, Node> nodes) ParseInput(string fileName)
    {
        var lines = File.ReadAllLines(fileName);

        var instructions = lines[0];
        var nodes = lines.Skip(2).Select(l => new Node(l));

        return (instructions, nodes.ToDictionary(n => n.Name, n => n));
    }

    private static long Part1(string instructions, Dictionary<string, Node> nodes, string start = "AAA", string target = "ZZZ")
    {
        long moves = 0;
        var next = start;
        while (true)
        {
            foreach (var c in instructions)
            {
                next = c == 'R' ? nodes[next].Right : nodes[next].Left;
                moves++;

                if (next.EndsWith(target))
                {
                    return moves;
                }
            }
        }
    }

    private static long Part2(string instructions, Dictionary<string, Node> nodes)
    {
        var currentNodes = nodes.Keys.Where(n => n.EndsWith('A')).ToList();
        var distances = currentNodes.Select(n => Part1(instructions, nodes, n, "Z")).ToList();
        return MathUtils.LeastCommonMultiple(distances);
    }

    private class Node
    {
        public string Name { get; init; }
        public string Left { get; init; }
        public string Right { get; init; }

        public Node(string line)
        {
            var parts = line.Split(" = ");
            Name = parts[0];
            var directions = parts[1].Split(", ").Select(p => p.Trim('(', ')'));
            Left = directions.ElementAt(0);
            Right = directions.ElementAt(1);
        }
    }
}