using GameOfLife.Core.Enums;
using GameOfLife.Core.Models;
using System.Drawing;

namespace GameOfLife.Core.Services
{
    public static class SimulationService
    {
        public static void NextGeneration(Board board, Rules rules, Statistics stats)
        {
            var aliveCells = new List<Point>(board.GetAliveCells());
            var neighborCounts = new Dictionary<Point, int>();

            foreach (var cell in aliveCells)
            {
                for (var dx = -1; dx <= 1; dx++)
                {
                    for (var dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;

                        var neighbour = new Point(cell.X + dx, cell.Y + dy);
                        var wrappedNeighbour = board.GetWrappedPoint(neighbour); // Wrap

                        neighborCounts[wrappedNeighbour] = neighborCounts.GetValueOrDefault(wrappedNeighbour, 0) + 1;
                    }
                }
            }

            var newCells = new Dictionary<Point, CellState>();
            var bornThisStep = 0;
            var diedThisStep = 0;

            foreach (var cell in aliveCells)
            {
                var neighbors = neighborCounts.GetValueOrDefault(cell, 0);
                if (rules.ShouldSurvive(neighbors))
                {
                    newCells[cell] = CellState.Alive;
                }
                else
                {
                    diedThisStep++;
                }
            }

            foreach (var candidate in neighborCounts.Keys)
            {
                if (!board.IsAlive(candidate))
                {
                    var neighbors = neighborCounts[candidate];

                    if (rules.ShouldBeBorn(neighbors))
                    {
                        newCells[candidate] = CellState.Alive;
                        bornThisStep++;
                    }
                }
            }

            board.Update(newCells);
            stats.Update(bornThisStep, diedThisStep);
        }
    }
}
