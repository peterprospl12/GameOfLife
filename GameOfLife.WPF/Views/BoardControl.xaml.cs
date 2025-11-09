using System.ComponentModel;
using GameOfLife.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using GameOfLife.Core.Enums;
using Point = System.Drawing.Point;

namespace GameOfLife.WPF.Views
{
    public partial class BoardControl : UserControl
    {
        private BoardViewModel _viewModel;
        private WriteableBitmap _writeableBitmap;
        private readonly DispatcherTimer _redrawTimer;
        private Color _aliveColor;
        private Color _deadColor;
        private CellShape _cellShape;
        private int _cellSize;

        private bool _isPanning;
        private System.Windows.Point _panLastPoint;

        public BoardControl()
        {
            InitializeComponent();
            _redrawTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16) // ~60 FPS
            };
            _redrawTimer.Tick += (s, e) => DrawBoard();

            Loaded += OnLoaded;
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            InitializeViewModel();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            InitializeViewModel();
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BoardViewModel.Board))
            {
                InitializeBitmap();
                DrawBoard();
            }
        }

        private void InitializeViewModel()
        {
            if (DataContext is BoardViewModel vm && vm != _viewModel)
            {
                if (_viewModel != null)
                {
                    _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
                }

                _viewModel = vm;
                _aliveColor = vm.AliveColor;
                _deadColor = vm.DeadColor;
                _cellShape = vm.CellShape;

                _viewModel.PropertyChanged += OnViewModelPropertyChanged;

                _cellSize = _cellShape == CellShape.Square ? 1 : 7;

                InitializeBitmap();
                _redrawTimer.Start();

                if (ActualWidth > 0 && ActualHeight > 0)
                {
                    _viewModel.SetInitialZoom(ActualWidth, ActualHeight);
                }
            }
        }

        private void InitializeBitmap()
        {
            var board = _viewModel.GetBoard;
            var bitmapWidth = board.Width * _cellSize;
            var bitmapHeight = board.Height * _cellSize;
            
            _writeableBitmap = new WriteableBitmap(bitmapWidth, bitmapHeight, 96, 96, PixelFormats.Bgr32, null);
            BoardImage.Source = _writeableBitmap;
        }

        private void DrawBoard()
        {
            var board = _viewModel.GetBoard;
            var aliveCells = board.GetAliveCells();

            try
            {
                _writeableBitmap.Lock();
                _writeableBitmap.Clear(_deadColor);

                foreach (var cell in aliveCells)
                {
                    var x = cell.X * _cellSize;
                    var y = cell.Y * _cellSize;

                    switch (_cellShape)
                    {
                        case CellShape.Square:
                            if (x >= 0 && x < _writeableBitmap.PixelWidth && y >= 0 && y < _writeableBitmap.PixelHeight)
                            {
                                _writeableBitmap.SetPixel(x, y, _aliveColor);
                            }
                            break;
                        case CellShape.Triangle:
                            DrawTriangle(x, y, _cellSize, _aliveColor);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            finally
            {
                _writeableBitmap.Unlock();
            }
        }

        private void DrawTriangle(int x, int y, int size, Color color)
        {
            var center = size / 2;

            for (var row = 0; row < size; row++)
            {
                var leftEdge = center - (row * 0.5);
                var rightEdge = center + (row * 0.5);

                for (var col = 0; col < size; col++)
                {
                    if (col >= leftEdge && col <= rightEdge)
                    {
                        var pixelX = x + col;
                        var pixelY = y + row;

                        if (pixelX >= 0 && pixelX < _writeableBitmap.PixelWidth &&
                            pixelY >= 0 && pixelY < _writeableBitmap.PixelHeight)
                        {
                            _writeableBitmap.SetPixel(pixelX, pixelY, color);
                        }
                    }
                }
            }
        }

        private void OnCellClick(object sender, MouseButtonEventArgs e)
        {
            if (!_viewModel.IsEditing || _isPanning) return;

            var position = e.GetPosition(BoardImage);
            var board = _viewModel.GetBoard;

            var x = (int)(position.X / _cellSize);
            var y = (int)(position.Y / _cellSize);

            if (x >= 0 && x < board.Width && y >= 0 && y < board.Height)
            {
                _viewModel.ToggleCellCommand.Execute(new Point(x, y));
                DrawBoard();
            }
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var oldZoom = _viewModel.ZoomLevel;
            _viewModel.UpdateZoom(e.Delta);

            var mousePos = e.GetPosition(ScrollViewer);
            var newZoom = _viewModel.ZoomLevel;

            var newOffsetX = (ScrollViewer.HorizontalOffset + mousePos.X) * (newZoom / oldZoom) - mousePos.X;
            var newOffsetY = (ScrollViewer.VerticalOffset + mousePos.Y) * (newZoom / oldZoom) - mousePos.Y;

            ScrollViewer.ScrollToHorizontalOffset(newOffsetX);
            ScrollViewer.ScrollToVerticalOffset(newOffsetY);

            e.Handled = true;
        }

        private void OnPanMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle || (e.ChangedButton == MouseButton.Left && Keyboard.Modifiers.HasFlag(ModifierKeys.Control)))
            {
                _isPanning = true;
                _panLastPoint = e.GetPosition(ScrollViewer);
                Cursor = Cursors.ScrollAll;
                e.Handled = true;
            }
        }

        private void OnPanMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isPanning) return;

            var currentPoint = e.GetPosition(ScrollViewer);
            var delta = currentPoint - _panLastPoint;
            _panLastPoint = currentPoint;

            ScrollViewer.ScrollToHorizontalOffset(ScrollViewer.HorizontalOffset - delta.X);
            ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset - delta.Y);
        }

        private void OnPanMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isPanning || e.ChangedButton is not (MouseButton.Middle or MouseButton.Left)) return;

            _isPanning = false;
            Cursor = Cursors.Arrow;
            e.Handled = true;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _redrawTimer.Stop();
        }
    }
}