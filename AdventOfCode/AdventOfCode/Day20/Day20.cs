

internal class Day20
{
    public static void Run()
    {
        System.Console.WriteLine(Part1(ParseInput("Day20/testinput.txt")));
        System.Console.WriteLine(Part1(ParseInput("Day20/testinput2.txt")));
        System.Console.WriteLine(Part1(ParseInput("Day20/input.txt")));
        System.Console.WriteLine(Part2(ParseInput("Day20/input.txt"))); //246313604784977

    }

    private static Dictionary<string, Module> ParseInput(string fileName)
    {
        var lines = File.ReadAllLines(fileName);
        var modules = lines.Select(l => Module.Create(l)).ToDictionary(m => m.Name, m => m);

        foreach (var module in modules.Values)
        {
            if (module is Conjunction conj)
            {
                var inputs = modules.Values.Where(m => m.Outputs.Any(o => o == module.Name)).Select(m => m.Name);
                conj.SetIntputs(inputs);
            }
        }

        return modules;
    }

    private static long Part1(Dictionary<string, Module> modules)
    {
        var highs = 0;
        var lows = 0;

        for (int i = 0; i < 1000; i++)
        {
            var res = PressButton(modules);
            highs += res.highs;
            lows += res.lows;
        }

        return highs * lows;
    }
    

    private static long Part2(Dictionary<string, Module> modules)
    {
        var inputToRx = modules.Where(m => m.Value.Outputs.Any(o => o == "rx")).Select(m => m.Key).Single();
        var inputsToRx = modules.Where(m => m.Value.Outputs.Any(o => o == inputToRx)).Select(m => m.Key).ToList();
        var res = new List<long>();

        foreach (var input in inputsToRx)
        {
            var i = 1L;
            foreach (var module in modules)
            {
                module.Value.Reset();   //would have been better to look for all four instead of one at the time. but this worked and then I gave up optimizing.
            }

            while (true)
            {
                var temp = PressButtonAndFindHighPulse(modules, input);
                if (temp)
                {
                    res.Add(i);
                    break;
                }
                i++;
            }
        }

        return MathUtils.LeastCommonMultiple(res);
    }

    private static (int highs, int lows) PressButton(Dictionary<string, Module> modules)
    {
        var highs = 0;
        var lows = 1;
        var active = new List<(string previous, bool high, Module module)>() { ("button", false, modules["broadcaster"]) };

        while (active.Any())
        {
            var next = new List<(string, bool, Module)>();

            foreach (var module in active)
            {
                var pulse = module.module.OnPulse(module.high, module.previous);
                if (pulse.HasValue)
                {
                    foreach (var output in module.module.Outputs)
                    {
                        if (modules.ContainsKey(output))
                        {
                            next.Add((module.module.Name, pulse.Value, modules[output]));
                        }

                        var _ = pulse.Value ? highs++ : lows++;
                    }
                }
            }

            active = next;
        }

        return (highs, lows);
    }

    private static bool PressButtonAndFindHighPulse(Dictionary<string, Module> modules, string moduleName)
    {
        var pulsesToModule = 0;
        var active = new List<(string previous, bool high, Module module)>() { ("button", false, modules["broadcaster"]) };

        while (active.Any())
        {
            var next = new List<(string, bool, Module)>();

            foreach (var module in active)
            {
                var pulse = module.module.OnPulse(module.high, module.previous);
                if (pulse.HasValue)
                {
                    if (pulse == true && module.module.Name == moduleName)
                    {
                        pulsesToModule++;
                    }

                    foreach (var output in module.module.Outputs)
                    {
                        if (modules.ContainsKey(output))
                        {
                            next.Add((module.module.Name, pulse.Value, modules[output]));
                        }
                    }
                }
            }

            active = next;
        }

        return pulsesToModule == 1;
    }


    private abstract class Module
    {
        public string Name { get; set; }
        public List<string> Outputs { get; set; }
        public bool? LastPulse { get; set; }

        public bool? OnPulse(bool high, string previousName)
        {
            var pulseRes = Pulse(high, previousName);
            LastPulse = pulseRes;
            return pulseRes;
        }

        protected abstract bool? Pulse(bool high, string previousName);

        public abstract void Reset();

        public static Module Create(string line)
        {
            var parts = line.Split("->");
            var outputs = parts[1].Split(',').Select(o => o.Trim()).ToList();
            if (line.StartsWith("%"))
            {
                return new FlipFlop() { Name = new string(parts[0].Skip(1).ToArray()).Trim(), Outputs = outputs };
            }
            else if (line.StartsWith("&"))
            {
                return new Conjunction() { Name = new string(parts[0].Skip(1).ToArray()).Trim(), Outputs = outputs };
            }
            else if (line.StartsWith("broadcaster"))
            {
                return new Broadcaster() { Name = "broadcaster", Outputs = outputs };
            }

            throw new ApplicationException("Unknown module");
        }
    }

    private class Broadcaster : Module
    {
        protected override bool? Pulse(bool high, string previousName)
        {
            return high;
        }
        public override void Reset()
        {
        }
    }

    private class FlipFlop : Module
    {
        public bool On { get; set; }

        protected override bool? Pulse(bool high, string previousName)
        {
            if (!high)
            {
                On = !On;
                return On;
            }

            return null;
        }

        public override void Reset()
        {
            On = false;
        }
    }

    private class Conjunction : Module
    {
        public Dictionary<string, bool> Previous { get; set; }

        protected override bool? Pulse(bool high, string previousName)
        {
            Previous[previousName] = high;
            return !Previous.Values.All(b => b);
        }

        public override void Reset()
        {
            foreach (var key in Previous.Keys.ToList())
            {
                Previous[key] = false;
            }
        }

        public void SetIntputs(IEnumerable<string> inputs)
        {
            Previous = inputs.ToDictionary(i => i, i => false);
        }
    }
}