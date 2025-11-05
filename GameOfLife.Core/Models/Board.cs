using GameOfLife.Core.Enums;
using GameOfLife.Core.Extensions;
using System.Drawing;

namespace GameOfLife.Core.Models
{
    public class Board
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Dictionary<Point, CellState> Cells { get; } = [];

        public void Initialize(int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentOutOfRangeException("Width and height must be greater than 0.");
            }

            Width = width;
            Height = height;
            Cells.Clear();
        }

        public void Update(Dictionary<Point, CellState> newCells)
        {
            Cells.Clear();

            foreach (var kvp in newCells)
            {
                Cells[kvp.Key] = kvp.Value;
            }
        }

        public bool IsAlive(Point point)
        {
            return GetCellState(point) == CellState.Alive;
        }

        public void SetCell(Point point, CellState state)
        {
            var wrappedPoint = GetWrappedPoint(point);

            if (state == CellState.Dead)
            {
                Cells.Remove(wrappedPoint);
            }
            else
            {
                Cells[wrappedPoint] = state;
            }
        }

        public void Randomize(double density = 0.5)
        {
            if (density is < 0 or > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(density), "Density must be between 0 and 1.");
            }

            Cells.Clear(); 

            var random = new Random();

            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    if (random.NextDouble() < density)
                    {
                        Cells[new Point(x,y)] = CellState.Alive;
                    }
                }
            }
        }

        public IEnumerable<Point> GetAliveCells()
        {
            return Cells.Keys;
        }

        public void LoadFromFile(string filePath, Rules rules)
        {
            throw new NotImplementedException();
        }

        public void SaveToFile(string filePath, Rules rules)
        {
            throw new NotImplementedException();
        }

        public CellState GetCellState(Point point)
        {
            return Cells.GetValueOrDefault(GetWrappedPoint(point), CellState.Dead);
        }

        public Point GetWrappedPoint(Point point)
        {
            return point.WrapToBoard(Width, Height);
        }

        public void Clear()
        {
            Cells.Clear();
        }
    }
}
