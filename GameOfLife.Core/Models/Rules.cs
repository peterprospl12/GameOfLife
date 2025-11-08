using System.Text.RegularExpressions;

namespace GameOfLife.Core.Models
{
    public class Rules
    {
        public HashSet<int> Birth { get; } = [];
        public HashSet<int> Survive { get; } = [];
        public string RuleString { get; private set; }


        public Rules(string ruleString)
        {
            RuleString = ruleString;
            Parse(ruleString);
        }

        private void Parse(string ruleString)
        {
            if (string.IsNullOrWhiteSpace(ruleString))
            {
                throw new ArgumentException("Rule string cannot be null or empty.", nameof(ruleString));
            }

            var match = Regex.Match(ruleString, @"^B([0-8]+)/S([0-8]+)$", RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                throw new ArgumentException("Invalid rule string format. Expected format: B.../S...", nameof(ruleString));
            }

            foreach (var c in match.Groups[1].Value)
            {
                Birth.Add(c - '0');
            }

            foreach (var c in match.Groups[2].Value)
            {
                Survive.Add(c - '0');
            }
        }

        public bool ShouldBeBorn(int neighbors) => Birth.Contains(neighbors);
        public bool ShouldSurvive(int neighbors) => Survive.Contains(neighbors);

        public static string DefaultConway() => new("B3/S23");
    }
}