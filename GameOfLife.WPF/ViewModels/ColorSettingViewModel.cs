using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media;

namespace GameOfLife.WPF.ViewModels
{
    public partial class ColorSettingViewModel(string label, Color initialColor) : SettingViewModelBase(label)
    {
        [ObservableProperty]
        private Color _selectedColor = initialColor;
    }
}