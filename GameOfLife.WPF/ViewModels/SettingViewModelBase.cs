using CommunityToolkit.Mvvm.ComponentModel;

namespace GameOfLife.WPF.ViewModels
{
    public abstract partial class SettingViewModelBase(string label) : ObservableObject
    {
        [ObservableProperty]
        private string _label = label;
    }
}
