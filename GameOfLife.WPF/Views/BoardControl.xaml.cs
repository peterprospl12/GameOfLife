using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GameOfLife.WPF.ViewModels;
using Point = System.Drawing.Point;

namespace GameOfLife.WPF.Views
{
    public partial class BoardControl : UserControl
    {
        public BoardControl()
        {
            InitializeComponent();
        }

        private void OnCellClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is BoardViewModel vm && vm.IsEditing)
            {
                var position = e.GetPosition(BoardCanvas);
                int x = (int)position.X;
                int y = (int)position.Y;
                vm.ToggleCell(new Point(x, y));
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Opcjonalnie: Ustaw rozmiar lub inicjalizuj
        }
    }
}