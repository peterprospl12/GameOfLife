using System.Collections.ObjectModel;
using System.Drawing;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameOfLife.Core.Enums;
using GameOfLife.Core.Models;
using GameOfLife.WPF.Models;

namespace GameOfLife.WPF.ViewModels
{
    public partial class BoardViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<UiCell> visibleCells; 

        [ObservableProperty]
        private double zoomLevel = 1.0;

        [ObservableProperty]
        private double offsetX;

        [ObservableProperty]
        private double offsetY;

        private double viewportWidth;
        private double viewportHeight;

        [ObservableProperty]
        private bool isEditing; 

        private readonly Board board; 
        private readonly Dictionary<Point, UiCell> cellLookup = new(); 

        public int Width => board.Width;
        public int Height => board.Height;

        public BoardViewModel(Board board)
        {
            this.board = board;
            VisibleCells = [];
        }

        public void SetViewportSize(double width, double height)
        {
            viewportWidth = width;
            viewportHeight = height;

            if (zoomLevel == 1.0 && board.Width > 0 && board.Height > 0)
            {
                var zoomX = viewportWidth / board.Width;
                var zoomY = viewportHeight / board.Height;
                ZoomLevel = Math.Min(zoomX, zoomY) * 0.9;
            }

            UpdateVisibleCells();
        }

        private (int minX, int minY, int maxX, int maxY) GetViewportCellBounds()
        {
            var minX = Math.Max(0, (int)Math.Floor(OffsetX / ZoomLevel));
            var minY = Math.Max(0, (int)Math.Floor(OffsetY / ZoomLevel));
            var maxX = Math.Min(board.Width - 1, (int)Math.Ceiling((OffsetX + viewportWidth) / ZoomLevel));
            var maxY = Math.Min(board.Height - 1, (int)Math.Ceiling((OffsetY + viewportHeight) / ZoomLevel));
            return (minX, minY, maxX, maxY);
        }

        public void UpdateVisibleCells()
        {
            var (minX, minY, maxX, maxY) = GetViewportCellBounds();

            var allAliveCells = board.GetAliveCells();
            var visibleAliveCells = allAliveCells
                .Where(p => p.X >= minX && p.X <= maxX && p.Y >= minY && p.Y <= maxY)
                .ToHashSet();

            var cellsToRemove = new List<UiCell>();

            foreach (var uiCell in VisibleCells)
            {
                var point = new Point(uiCell.X, uiCell.Y);
                var isInViewport = point.X >= minX && point.X <= maxX &&
                                   point.Y >= minY && point.Y <= maxY;

                if (!visibleAliveCells.Contains(point) || !isInViewport)
                {
                    cellsToRemove.Add(uiCell);
                    cellLookup.Remove(point);
                }
                else
                {
                    visibleAliveCells.Remove(point);
                }
            }

            foreach (var cell in cellsToRemove)
            {
                VisibleCells.Remove(cell);
            }

            foreach (var point in visibleAliveCells)
            {
                var uiCell = new UiCell(point.X, point.Y, CellState.Alive);
                VisibleCells.Add(uiCell);
                cellLookup[point] = uiCell;
            }
        }

        public void UpdateSingleCell(Point point)
        {
            var (minX, minY, maxX, maxY) = GetViewportCellBounds();

            var isInViewport = point.X >= minX && point.X <= maxX &&
                               point.Y >= minY && point.Y <= maxY;

            if (!isInViewport)
                return; // Komórka poza viewport, nie aktualizuj UI

            var state = board.GetCellState(point);

            if (state == CellState.Alive)
            {
                if (!cellLookup.ContainsKey(point))
                {
                    var uiCell = new UiCell(point.X, point.Y, CellState.Alive);
                    VisibleCells.Add(uiCell);
                    cellLookup[point] = uiCell;
                }
            }
            else
            {
                if (cellLookup.TryGetValue(point, out var uiCell))
                {
                    VisibleCells.Remove(uiCell);
                    cellLookup.Remove(point);
                }
            }
        }

        [RelayCommand]
        public void ZoomIn()
        {
            ZoomLevel = Math.Min(ZoomLevel * 1.5, 30.0);
        }

        [RelayCommand]
        public void ZoomOut()
        {
            ZoomLevel = Math.Max(ZoomLevel / 1.5, 0.1);
        }

        [RelayCommand]
        public void ResetZoom()
        {
            OffsetX = 0;
            OffsetY = 0;
           
            if (viewportWidth > 0 && viewportHeight > 0 && board.Width > 0 && board.Height > 0)
            {
                var zoomX = viewportWidth / board.Width;
                var zoomY = viewportHeight / board.Height;
                ZoomLevel = Math.Min(zoomX, zoomY) * 0.9;
            }
            else
            {
                ZoomLevel = 1.0;
            }
        }


        public void Zoom(double delta, double mouseX, double mouseY)
        {
            var oldZoom = ZoomLevel;
            if (delta > 0)
                ZoomLevel = Math.Min(ZoomLevel * 1.2, 30.0);
            else
                ZoomLevel = Math.Max(ZoomLevel / 1.2, 0.1);

            var newZoom = ZoomLevel;

            OffsetX = Math.Max(0, OffsetX + mouseX * (oldZoom - newZoom) / newZoom);
            OffsetY = Math.Max(0, OffsetY + mouseY * (oldZoom - newZoom) / newZoom);
        }

        [RelayCommand]
        public void ToggleCell(Point point)
        {
            if (IsEditing)
            {
                var state = board.GetCellState(point);
                board.SetCell(point, state == CellState.Alive ? CellState.Dead : CellState.Alive);
                UpdateSingleCell(point);
            }
        }
    }
}