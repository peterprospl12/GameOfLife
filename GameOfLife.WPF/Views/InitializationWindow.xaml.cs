using System.Windows;
using System.Windows.Media.Animation;

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
            var fadeOutAnimation = (Storyboard)FindResource("FadeOutAnimation");
            fadeOutAnimation.Completed += (s, _) =>
            {
                DialogResult = true;
            };
            fadeOutAnimation.Begin(this);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}