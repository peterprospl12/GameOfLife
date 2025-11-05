using GameOfLife.Core.Enums;

namespace GameOfLife.WPF.Models;

public class UiCell(int x, int y, CellState state)
{
    public int X { get; } = x;
    public int Y { get; } = y;
    public CellState State { get; } = state;
}