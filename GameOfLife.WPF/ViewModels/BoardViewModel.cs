using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameOfLife.Core.Enums;
using GameOfLife.Core.Models;
using System.Drawing;
using Color = System.Windows.Media.Color;

namespace GameOfLife.WPF.ViewModels
{
    public partial class BoardViewModel : ObservableObject
    {
        [ObservableProperty]
        private Board _board;

        [ObservableProperty]
        private double _zoomLevel = 1.0;

        [ObservableProperty]
        private bool _isEditing;

        [ObservableProperty]
        private Color _aliveColor;

        [ObservableProperty]
        private Color _deadColor;

        [ObservableProperty]
        private CellShape _cellShape;

        public int Width => _board.Width;
        public int Height => _board.Height;
        public Board GetBoard => _board;

        public BoardViewModel(Board board)
        {
            _board = board;
        }

        public BoardViewModel(int width, int height)
        {
            _board = new Board();
            Initialize(width, height);
        }

        public void Initialize(int width, int height)
        {
            _board.Initialize(width, height);
            Randomize(0.5);
        }

        public void SetInitialZoom(double viewWidth, double viewHeight)
        {
            if (_board.Width == 0 || _board.Height == 0)
            {
                ZoomLevel = 1.0;
                return;
            }

            const double cellWidth = 1.0;
            const double cellHeight = 1.0;

            const double margin = 0.9;

            double zoomX = (viewWidth * margin) / (_board.Width * cellWidth);
            double zoomY = (viewHeight * margin) / (_board.Height * cellHeight);

            ZoomLevel = Math.Min(zoomX, zoomY);
        }

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
                var state = _board.GetCellState(point);
                _board.SetCell(point, state == CellState.Alive ? CellState.Dead : CellState.Alive);
            }
        }

        public void Randomize(double density)
        {
            _board.Randomize(density);
        }

        public void Clear()
        {
            _board.Clear();
        }
    }
}