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
        private bool _initialZoomSet;

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
                _initialZoomSet = false;
            }
        }

        private void OnScrollViewerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_viewModel != null && !_initialZoomSet && e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                _viewModel.SetInitialZoom(e.NewSize.Width, e.NewSize.Height, _cellSize);
                _initialZoomSet = true;
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

        private unsafe void DrawBoard()
        {
            var board = _viewModel.GetBoard;
            var aliveCells = board.GetAliveCells();

            try
            {
                _writeableBitmap.Lock();
                _writeableBitmap.Clear(_deadColor);

                var backBuffer = (int*)_writeableBitmap.BackBuffer;
                var stride = _writeableBitmap.BackBufferStride / 4;
                var color = (_aliveColor.A << 24) | (_aliveColor.R << 16) | (_aliveColor.G << 8) | _aliveColor.B;

                foreach (var cell in aliveCells)
                {
                    var x = cell.X * _cellSize;
                    var y = cell.Y * _cellSize;

                    switch (_cellShape)
                    {
                        case CellShape.Square:
                            if (x >= 0 && x < _writeableBitmap.PixelWidth && y >= 0 && y < _writeableBitmap.PixelHeight)
                            {
                                *(backBuffer + y * stride + x) = color;
                            }
                            break;
                        case CellShape.Triangle:
                            DrawTriangle(x, y, _cellSize, color, backBuffer, stride);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                _writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, _writeableBitmap.PixelWidth, _writeableBitmap.PixelHeight));
            }
            finally
            {
                _writeableBitmap.Unlock();
            }
        }

        private unsafe void DrawTriangle(int x, int y, int size, int color, int* backBuffer, int stride)
        {
            var center = size / 2;
            var bitmapWidth = _writeableBitmap.PixelWidth;
            var bitmapHeight = _writeableBitmap.PixelHeight;

            for (var row = 0; row < size; row++)
            {
                var pixelY = y + row;
                if (pixelY < 0 || pixelY >= bitmapHeight) continue;

                var leftEdge = center - (row * 0.5);
                var rightEdge = center + (row * 0.5);

                var startCol = (int)Math.Ceiling(leftEdge);
                var endCol = (int)Math.Floor(rightEdge);

                for (var col = startCol; col <= endCol; col++)
                {
                    var pixelX = x + col;

                    if (pixelX >= 0 && pixelX < bitmapWidth)
                    {
                        *(backBuffer + pixelY * stride + pixelX) = color;
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
            if (oldZoom == 0) return; 

            _viewModel.UpdateZoom(e.Delta);

            var mousePos = e.GetPosition(ScrollViewer);
            var newZoom = _viewModel.ZoomLevel;

            var newOffsetX = (ScrollViewer.HorizontalOffset + mousePos.X) * (newZoom / oldZoom) - mousePos.X;
            var newOffsetY = (ScrollViewer.VerticalOffset + mousePos.Y) * (newZoom / oldZoom) - mousePos.Y;

            if (!double.IsNaN(newOffsetX) && !double.IsNaN(newOffsetY))
            {
                ScrollViewer.ScrollToHorizontalOffset(newOffsetX);
                ScrollViewer.ScrollToVerticalOffset(newOffsetY);
            }

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