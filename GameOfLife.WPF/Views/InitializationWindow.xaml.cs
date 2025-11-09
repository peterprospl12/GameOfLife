using System.Windows;

namespace GameOfLife.WPF.Views
{
    public partial class InitializationWindow : Window
    {
        public InitializationWindow()
        {
            InitializeComponent();
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