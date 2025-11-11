using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;
using GameOfLife.Core.Models;
using System.Windows.Media;
using GameOfLife.Core.Enums;
using System.Collections.ObjectModel;

namespace GameOfLife.WPF.ViewModels
{
    public partial class InitializationViewModel : ObservableObject, INotifyDataErrorInfo
    {
        public ObservableCollection<SettingViewModelBase> Settings { get; }
        public IReadOnlyList<CellShape> AvailableShapes { get; } = [CellShape.Square, CellShape.Triangle];

        private readonly TextFieldSettingViewModel _widthSetting;
        private readonly TextFieldSettingViewModel _heightSetting;
        private readonly TextFieldSettingViewModel _ruleSetting;
        private readonly ColorSettingViewModel _aliveColorSetting;
        private readonly ColorSettingViewModel _deadColorSetting;

        [ObservableProperty]
        private CellShape _selectedCellShape = CellShape.Square;

        public InitializationViewModel()
        {
            _widthSetting = new TextFieldSettingViewModel(
                "Board Width:", "100",
                value =>
                {
                    if (!int.TryParse(value, out var number) || number <= 0)
                        return "Width must be a positive integer.";
                    return null;
                });

            _heightSetting = new TextFieldSettingViewModel(
                "Board Height:", "100",
                value =>
                {
                    if (!int.TryParse(value, out var number) || number <= 0)
                        return "Height must be a positive integer.";
                    return null;
                });

            _ruleSetting = new TextFieldSettingViewModel(
                "Game Rules (e.g., B3/S23):", Rules.DefaultConway(),
                value =>
                {
                    if (string.IsNullOrWhiteSpace(value) || !Regex.IsMatch(value, @"^B[0-8]+/S[0-8]+$"))
                        return "Rule must be in the format B.../S... (e.g., B3/S23).";
                    return null;
                });

            _aliveColorSetting = new ColorSettingViewModel("Alive cells color:", Colors.Black);
            _deadColorSetting = new ColorSettingViewModel("Dead cells color:", Colors.LightGray);

            Settings =
            [
                _widthSetting,
                _heightSetting,
                _ruleSetting,
                _aliveColorSetting,
                _deadColorSetting
            ];

            foreach (var setting in Settings.OfType<INotifyDataErrorInfo>())
            {
                setting.ErrorsChanged += OnSettingErrorsChanged;
            }
        }

        public bool HasErrors => Settings.OfType<INotifyDataErrorInfo>().Any(s => s.HasErrors);

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        private void OnSettingErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
        {
            OnPropertyChanged(nameof(HasErrors));
        }

        public IEnumerable GetErrors(string? propertyName) => Enumerable.Empty<string>();

        public int GetWidth() => int.Parse(_widthSetting.Value);
        public int GetHeight() => int.Parse(_heightSetting.Value);
        public string GetRuleString() => _ruleSetting.Value;
        public Color GetAliveColor() => _aliveColorSetting.SelectedColor;
        public Color GetDeadColor() => _deadColorSetting.SelectedColor;
        public CellShape GetShape() => SelectedCellShape;
    }
}
