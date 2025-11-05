using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GameOfLife.WPF.ViewModels;

namespace GameOfLife.WPF.Views
{
    public partial class BoardControl : UserControl
    {
        private System.Windows.Point? lastMousePosition;
        private bool isPanning;
        public BoardControl()
        {
            InitializeComponent();
            DataContextChanged += (s, e) =>
            {
                if (e.NewValue is BoardViewModel vm)
                {
                    vm.PropertyChanged += (sender, args) =>
                    {
                        if (args.PropertyName == nameof(BoardViewModel.OffsetX))
                        {
                            ScrollViewer.ScrollToHorizontalOffset(vm.OffsetX);
                        }
                        if (args.PropertyName == nameof(BoardViewModel.OffsetY))
                        {
                            ScrollViewer.ScrollToVerticalOffset(vm.OffsetY);
                        }
                    };
                }
            };
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is BoardViewModel vm)
            {
                vm.SetViewportSize(ScrollViewer.ActualWidth, ScrollViewer.ActualHeight);
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DataContext is BoardViewModel vm)
            {
                vm.SetViewportSize(e.NewSize.Width, e.NewSize.Height);
            }
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (DataContext is BoardViewModel vm)
            {
                var position = e.GetPosition(BoardCanvas);
                vm.Zoom(e.Delta, position.X, position.Y);
                e.Handled = true;
            }
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control || e.ChangedButton == MouseButton.Middle)
            {
                lastMousePosition = e.GetPosition(ScrollViewer);
                isPanning = true;
                ScrollViewer.CaptureMouse();
                Cursor = Cursors.ScrollAll;
                e.Handled = true;
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (isPanning && lastMousePosition.HasValue)
            {
                var currentPosition = e.GetPosition(ScrollViewer);
                var delta = currentPosition - lastMousePosition.Value;

                ScrollViewer.ScrollToHorizontalOffset(ScrollViewer.HorizontalOffset - delta.X);
                ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset - delta.Y);

                lastMousePosition = currentPosition;
                e.Handled = true;
            }
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isPanning)
            {
                isPanning = false;
                lastMousePosition = null;
                ScrollViewer.ReleaseMouseCapture();
                Cursor = Cursors.Arrow;
                e.Handled = true;
            }
        }

        private void OnCellClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is BoardViewModel vm && vm.IsEditing && !isPanning)
            {
                var position = e.GetPosition(BoardCanvas);
                var cellX = (int)position.X;
                var cellY = (int)position.Y;
                vm.ToggleCellCommand.Execute(new System.Drawing.Point(cellX, cellY));
            }
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (DataContext is BoardViewModel vm)
            {
                vm.OffsetX = e.HorizontalOffset;
                vm.OffsetY = e.VerticalOffset;
                vm.UpdateVisibleCells();
            }
        }
    }
}