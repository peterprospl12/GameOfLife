using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GameOfLife.Core.Enums;
using GameOfLife.Core.Models;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;
using Point = System.Drawing.Point;

namespace GameOfLife.WPF.ViewModels
{
    public partial class BoardViewModel : ObservableObject
    {
        [ObservableProperty]
        private Board _board;

        [ObservableProperty]
        private double _zoomLevel = 8.0;

        [ObservableProperty]
        private bool _isEditing;

        [ObservableProperty]
        private Color _aliveColor;

        [ObservableProperty]
        private Color _deadColor;

        [ObservableProperty]
        private CellShape _cellShape;

        public int Width => Board.Width;
        public int Height => Board.Height;
        public Board GetBoard => Board;

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
            Board.Initialize(width, height);
            Randomize(0.4);
        }

        public void SetInitialZoom(double viewWidth, double viewHeight, int cellSize)
        {
            if (Width > 0 && Height > 0)
            {
                var boardPixelWidth = Width * cellSize;
                var boardPixelHeight = Height * cellSize;

                var zoomX = viewWidth / boardPixelWidth;
                var zoomY = viewHeight / boardPixelHeight;

                ZoomLevel = Math.Min(zoomX, zoomY);
            }
        }

        public void UpdateZoom(double delta)
        {
            if (delta > 0)
            {
                ZoomLevel = Math.Min(ZoomLevel * 1.2, 50.0);
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
                var state = Board.GetCellState(point);
                Board.SetCell(point, state == CellState.Alive ? CellState.Dead : CellState.Alive);
            }
        }

        public void Randomize(double density)
        {
            Board.Randomize(density);
        }

        public void Clear()
        {
            Board.Clear();
        }

        public BitmapSource CreateBitmap(int cellSize)
        {
            var imageWidth = Width * cellSize;
            var imageHeight = Height * cellSize;

            var drawingVisual = new DrawingVisual();
            using (DrawingContext dc = drawingVisual.RenderOpen())
            {
                var deadBrush = new SolidColorBrush(DeadColor);
                var aliveBrush = new SolidColorBrush(AliveColor);
                dc.DrawRectangle(deadBrush, null, new Rect(0, 0, imageWidth, imageHeight));

                foreach (var cellPoint in Board.GetAliveCells())
                {
                    double x = cellPoint.X * cellSize;
                    double y = cellPoint.Y * cellSize;

                    switch (CellShape)
                    {
                        case CellShape.Square:
                            dc.DrawRectangle(aliveBrush, null, new Rect(x, y, cellSize, cellSize));
                            break;

                        case CellShape.Triangle:
                            var streamGeometry = new StreamGeometry();
                            using (StreamGeometryContext sgc = streamGeometry.Open())
                            {
                                sgc.BeginFigure(new System.Windows.Point(x + cellSize / 2.0, y), true, true);
                                sgc.LineTo(new System.Windows.Point(x, y + cellSize), true, false);
                                sgc.LineTo(new System.Windows.Point(x + cellSize, y + cellSize), true, false);
                            }
                            streamGeometry.Freeze();
                            dc.DrawGeometry(aliveBrush, null, streamGeometry);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            var bitmap = new RenderTargetBitmap(imageWidth, imageHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(drawingVisual);
            bitmap.Freeze();
            return bitmap;
        }
    }
}
