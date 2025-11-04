namespace GameOfLife.Core.Models
{
    public class Rules
    {
        public List<int> Birth { get; private set; }
        public List<int> Survive { get; private set; }

        public Rules(string ruleString)
        {
            Birth = [];
            Survive = [];
            ParseRuleString(ruleString);
        }

        public bool ShouldBeBorn(int aliveNeighbors)
        {
            return Birth.Contains(aliveNeighbors);
        }

        public bool ShouldSurvive(int aliveNeighbors)
        {
            return Survive.Contains(aliveNeighbors);
        }

        public static Rules DefaultConway()
        {
            return new Rules("B3/S23");
        }

        private void ParseRuleString(string ruleString)
        {
            if (string.IsNullOrWhiteSpace(ruleString))
            {
                throw new ArgumentException("Rule string cannot be empty.", nameof(ruleString));
            }

            var parts = ruleString.Split('/');

            if (parts.Length != 2 || !parts[0].StartsWith('B') || !parts[1].StartsWith('S'))
            {
                throw new ArgumentException("Invalid rule format. Expected 'B.../S...'", nameof(ruleString));
            }

            if (parts.Length != 2) return;

            var birthStr = parts[0][1..];
            var surviveStr = parts[1][1..];

            Birth = ParseDigits(birthStr);
            Survive = ParseDigits(surviveStr);
        }

        private List<int> ParseDigits(string digitsStr)
        {
            var list = new List<int>();

            foreach (var c in digitsStr)
            {
                if (!char.IsDigit(c))
                {
                    throw new ArgumentException($"Invalid character '{c}' in rule string. Only digits allowed after B/S.");
                }

                var digit = (int)char.GetNumericValue(c);
                if (!list.Contains(digit)) // Unikalne, opcjonalnie
                {
                    list.Add(digit);
                }
            }

            return list;
        }

        public override string ToString()
        {
            return $"B{string.Join("", Birth.OrderBy(x => x))}/S{string.Join("", Survive.OrderBy(x => x))}";
        }
    }
}
