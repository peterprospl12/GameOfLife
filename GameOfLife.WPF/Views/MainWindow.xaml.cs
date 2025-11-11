using System.Windows;
using GameOfLife.WPF.ViewModels;

namespace GameOfLife.WPF.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
