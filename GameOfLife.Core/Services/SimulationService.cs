using GameOfLife.Core.Models;
using System.Drawing;

namespace GameOfLife.Core.Services
{
    public class SimulationService
    {
        public void NextGeneration(Board board, Rules rules, Statistics stats)
        {
            var initialAliveCells = board.GetAliveCells().ToHashSet();
            if (initialAliveCells.Count == 0)
            {
                stats.Update(0, 0);
                return;
            }

            var neighborCounts = new Dictionary<Point, int>();

            foreach (var cell in initialAliveCells)
            {
                for (var dx = -1; dx <= 1; dx++)
                {
                    for (var dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;

                        var neighbour = new Point(cell.X + dx, cell.Y + dy);
                        var wrappedNeighbour = board.GetWrappedPoint(neighbour);

                        neighborCounts[wrappedNeighbour] = neighborCounts.GetValueOrDefault(wrappedNeighbour, 0) + 1;
                    }
                }
            }

            var nextAliveCells = new HashSet<Point>();
            foreach (var (point, count) in neighborCounts)
            {
                var isAlive = initialAliveCells.Contains(point);
                if (isAlive)
                {
                    if (rules.ShouldSurvive(count))
                    {
                        nextAliveCells.Add(point);
                    }
                }
                else
                {
                    if (rules.ShouldBeBorn(count))
                    {
                        nextAliveCells.Add(point);
                    }
                }
            }

            var bornThisStep = nextAliveCells.Count(p => !initialAliveCells.Contains(p));
            var diedThisStep = initialAliveCells.Count(p => !nextAliveCells.Contains(p));

            board.Update(nextAliveCells);
            stats.Update(bornThisStep, diedThisStep);
        }
    }
}
