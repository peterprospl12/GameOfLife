using System.Windows;
using GameOfLife.WPF.ViewModels;

namespace GameOfLife.WPF.Views
{
    public partial class InitializationWindow : Window
    {
        public InitializationWindow(InitializationViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}