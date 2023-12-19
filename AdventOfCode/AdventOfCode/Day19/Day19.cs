using System.Dynamic;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using AdventOfCode;

internal class Day19
{
    public static void Run()
    {
        System.Console.WriteLine(Part1(ParseInput("Day19/testinput.txt")));
        System.Console.WriteLine(Part1(ParseInput("Day19/input.txt")));
        System.Console.WriteLine(Part2(ParseInput("Day19/testinput.txt")));
        System.Console.WriteLine(Part2(ParseInput("Day19/input.txt")));
    }

    private static Input ParseInput(string fileName)
    {
        var lines = File.ReadAllLines(fileName);
        return new Input(lines.TakeWhile(l => l != ""), lines.SkipWhile(l => l != "").Skip(1));
    }

    private static long Part1(Input input)
    {
        var res = 0;

        foreach (var rating in input.Ratings)
        {
            if (EvaluateRating(rating, input.Workflows))
            {
                res += rating.Sum;
            }
        }
        return res;
    }

    private static long Part2(Input input)
    {
        long res = 0;

        var workflows = input.Workflows.Values.ToList();
        foreach (var workflow in workflows)
        {
            foreach (var rule in workflow.Rules.Where(r => r.Destination == "A"))
            {
                res += GetPathToIn(rule, workflow, workflows);
            }
        }

        return res;
    }

    private static bool EvaluateRating(Rating rating, Dictionary<string, Workflow> workflows)
    {
        var current = "in";
        while (true)
        {
            var workflow = workflows[current];
            foreach (var rule in workflow.Rules)
            {
                if (rule.Evaluate(rating, out var next))
                {
                    if (next == "R") return false;
                    if (next == "A") return true;

                    current = next;
                    break;
                }
            }
        }

        throw new ApplicationException();
    }


    private static long GetPathToIn(Rule rule, Workflow workflow, List<Workflow> workflows)
    {
        var values = new Dictionary<string, List<int>>()
        {
            {"x", Enumerable.Range(1, 4000).ToList()},
            {"m", Enumerable.Range(1, 4000).ToList()},
            {"a", Enumerable.Range(1, 4000).ToList()},
            {"s", Enumerable.Range(1, 4000).ToList()}
        };

        while (values.Values.All(v => v.Count != 0))
        {
            var first = true;
            var ruleIndex = workflow.Rules.IndexOf(rule);
            for (int i = ruleIndex; i >= 0; i--)
            {
                rule = workflow.Rules.ElementAt(i);
                if (string.IsNullOrEmpty(rule.Category))
                {
                    first = false;
                    continue;
                }
                if (first)
                {
                    if (rule.Op == '<')
                    {
                        values[rule.Category] = values[rule.Category].Where(x => x < rule.Number).ToList();
                    }
                    else
                    {
                        values[rule.Category] = values[rule.Category].Where(x => x > rule.Number).ToList();
                    }
                    first = false;
                }
                else
                {
                    if (rule.Op == '>')
                    {
                        values[rule.Category] = values[rule.Category].Where(x => x <= rule.Number).ToList();
                    }
                    else
                    {
                        values[rule.Category] = values[rule.Category].Where(x => x >= rule.Number).ToList();
                    }
                }
            }
            var nextName = workflow.Name;
            if (nextName == "in")
            {
                break;
            }

            workflow = workflows.Single(wf => wf.Rules.Any(r => r.Destination == nextName));
            rule = workflow.Rules.Single(r => r.Destination == nextName);
        }


        var res = values["x"].LongCount() * values["m"].LongCount() * values["a"].LongCount() * values["s"].LongCount();
        return res;
    }

    private class Input
    {
        public Dictionary<string, Workflow> Workflows { get; set; } = new Dictionary<string, Workflow>();
        public List<Rating> Ratings { get; set; }
        public Input(IEnumerable<string> workflows, IEnumerable<string> ratings)
        {
            foreach (var workflow in workflows)
            {
                var parts = workflow.Split('{');
                Workflows.Add(parts[0], new Workflow(parts[0], parts[1].Trim('}')));
            }

            Ratings = ratings.Select(r => new Rating(r)).ToList();
        }
    }

    private class Workflow
    {
        public string Name { get; set; }
        public List<Rule> Rules { get; set; }

        public Workflow(string name, string rules)
        {
            Name = name;
            Rules = rules.Split(',').Select(r => new Rule(r)).ToList();
        }
    }

    private class Rule
    {
        public string Category { get; set; }
        public char Op { get; set; }
        public int Number { get; set; }
        public string Destination { get; set; }

        public Rule(string rule)
        {
            var match = Regex.Match(rule, "(a|m|s|x)(<|>)(\\d+):(\\w+)");
            if (match.Success)
            {
                Category = match.Groups[1].Value;
                Op = match.Groups[2].Value.Single();
                Number = int.Parse(match.Groups[3].Value);
                Destination = match.Groups[4].Value;
            }
            else
            {
                Destination = rule;

            }
        }

        public bool Evaluate(Rating rating, out string next)
        {
            next = Destination;
            if (string.IsNullOrEmpty(Category))
            {
                return true;
            }
            else
            {
                var xmas = rating.Categories[Category];
                return Op == '>' ? xmas > Number : xmas < Number;
            }
        }
    }

    private class Rating
    {
        public Dictionary<string, int> Categories = new Dictionary<string, int>();
        public int Sum => Categories.Values.Sum();
        public Rating(string line)
        {
            foreach (var r in line.Trim('{', '}').Split(','))
            {
                var parts = r.Split('=');
                var number = int.Parse(parts[1]);
                Categories.Add(parts[0], int.Parse(parts[1]));
            }
        }
    }

}