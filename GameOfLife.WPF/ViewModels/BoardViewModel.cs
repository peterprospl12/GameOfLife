using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameOfLife.Core.Enums;
using GameOfLife.Core.Models;
using System.Drawing;

namespace GameOfLife.WPF.ViewModels
{
    public partial class BoardViewModel(Board board) : ObservableObject
    {
        [ObservableProperty]
        private double _zoomLevel = 1.0;

        [ObservableProperty]
        private bool _isEditing;

        public int Width => board.Width;
        public int Height => board.Height;

        public Board GetBoard() => board;

        public void UpdateZoom(double delta)
        {
            if (delta > 0)
            {
                ZoomLevel = Math.Min(ZoomLevel * 1.2, 30.0);
            }
            else
            {
                ZoomLevel = Math.Max(ZoomLevel / 1.2, 0.1);
            }
        }

        [RelayCommand]
        public void ToggleCell(Point point)
        {
            if (IsEditing)
            {
                var state = board.GetCellState(point);
                board.SetCell(point, state == CellState.Alive ? CellState.Dead : CellState.Alive);
            }
        }
    }
}