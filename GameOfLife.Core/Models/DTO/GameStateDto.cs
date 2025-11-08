using System.Drawing;

namespace GameOfLife.Core.Models.DTO
{
    public class GameStateDto
    {
        public string RuleString { get; set; }
        public int Generation { get; set; }
        public int TotalBorn { get; set; }
        public int TotalDied { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public HashSet<Point> AliveCells { get; set; }
    }
}
