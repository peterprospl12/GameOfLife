using GameOfLife.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Point = System.Drawing.Point;

namespace GameOfLife.WPF.Views
{
    public partial class BoardControl : UserControl
    {
        private BoardViewModel _viewModel;
        private WriteableBitmap _writeableBitmap;
        private readonly DispatcherTimer _redrawTimer;
        private readonly Color _aliveColor = Colors.Black;
        private readonly Color _deadColor = Colors.LightGray;

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
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is BoardViewModel vm)
            {
                _viewModel = vm;
                InitializeBitmap();
                _redrawTimer.Start();
            }
        }

        private void InitializeBitmap()
        {
            var board = _viewModel.GetBoard();
            _writeableBitmap = new WriteableBitmap(board.Width, board.Height, 96, 96, PixelFormats.Bgr32, null);
            BoardImage.Source = _writeableBitmap;
        }

        private void DrawBoard()
        {
            var board = _viewModel.GetBoard();
            var aliveCells = board.GetAliveCells();

            try
            {
                _writeableBitmap.Lock();
                _writeableBitmap.Clear(_deadColor);

                foreach (var cell in aliveCells)
                {
                    _writeableBitmap.SetPixel(cell.X, cell.Y, _aliveColor);
                }
            }
            finally
            {
                _writeableBitmap.Unlock();
            }
        }

        private void OnCellClick(object sender, MouseButtonEventArgs e)
        {
            if (!_viewModel.IsEditing || _isPanning) return;

            var position = e.GetPosition(BoardImage);
            var board = _viewModel.GetBoard();

            var x = (int)position.X;
            var y = (int)position.Y;

            if (x >= 0 && x < board.Width && y >= 0 && y < board.Height)
            {
                _viewModel.ToggleCellCommand.Execute(new Point(x, y));
                DrawBoard();
            }
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_viewModel == null) return;

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