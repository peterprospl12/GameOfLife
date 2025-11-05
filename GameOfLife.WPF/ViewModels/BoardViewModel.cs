using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameOfLife.Core.Enums;
using GameOfLife.Core.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using GameOfLife.WPF.Models; // Dla Point

namespace GameOfLife.WPF.ViewModels
{
    public partial class BoardViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<UiCell> visibleCells; 

        [ObservableProperty]
        private double zoomLevel = 1.0; 

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
            UpdateVisibleCells(); 
        }

        public void UpdateVisibleCells()
        {
            var aliveCells = board.GetAliveCells().ToHashSet();
            var cellsToRemove = new List<UiCell>();

            foreach (var uiCell in VisibleCells)
            {
                var point = new Point(uiCell.X, uiCell.Y);
                if (!aliveCells.Contains(point))
                {
                    cellsToRemove.Add(uiCell);
                    cellLookup.Remove(point);
                }
                else
                {
                    aliveCells.Remove(point); 
                }
            }

            foreach (var cell in cellsToRemove)
            {
                VisibleCells.Remove(cell);
            }
            foreach (var point in aliveCells)
            {
                var uiCell = new UiCell(point.X, point.Y, CellState.Alive);
                VisibleCells.Add(uiCell);
                cellLookup[point] = uiCell;
            }
        }

        public void UpdateSingleCell(Point point)
        {
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