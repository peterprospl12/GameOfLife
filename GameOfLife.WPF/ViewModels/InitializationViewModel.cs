using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;
using GameOfLife.Core.Models;
using System.Windows.Media;
using GameOfLife.Core.Enums;

namespace GameOfLife.WPF.ViewModels
{
    public partial class InitializationViewModel : ObservableObject, INotifyDataErrorInfo
    {
        [ObservableProperty]
        private string _boardWidth = "100";

        [ObservableProperty]
        private string _boardHeight = "100";

        [ObservableProperty]
        private string _ruleString = Rules.DefaultConway();

        // New: Alive / Dead colors (bound to ColorPicker)
        [ObservableProperty]
        private Color _aliveColor = Colors.Black;

        [ObservableProperty]
        private Color _deadColor = Colors.LightGray;

        // Shape selection
        [ObservableProperty]
        private CellShape _selectedCellShape = CellShape.Square;

        public IReadOnlyList<CellShape> AvailableShapes { get; } =
        [
            CellShape.Square,
            CellShape.Triangle
        ];

        private readonly Dictionary<string, List<string>> _errors = new();

        public bool HasErrors => _errors.Count > 0;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
        {
            return _errors.GetValueOrDefault(propertyName ?? "", []);
        }

        private void Validate()
        {
            ClearErrors(nameof(BoardWidth));
            if (!Regex.IsMatch(BoardWidth, @"^\d+$"))
            {
                AddError(nameof(BoardWidth), "Width must be a positive integer.");
            }
            else if (int.Parse(BoardWidth) <= 0)
            {
                AddError(nameof(BoardWidth), "Width must be greater than 0.");
        }

            ClearErrors(nameof(BoardHeight));
            if (!Regex.IsMatch(BoardHeight, @"^\d+$"))
        {
                AddError(nameof(BoardHeight), "Height must be a positive integer.");
            }
            else if (int.Parse(BoardHeight) <= 0)
            {
                AddError(nameof(BoardHeight), "Height must be greater than 0.");
            }

            ClearErrors(nameof(RuleString));
            if (string.IsNullOrWhiteSpace(RuleString) || !Regex.IsMatch(RuleString, @"^B[0-8]+/S[0-8]+$"))
            {
                AddError(nameof(RuleString), "Rule must be in the format B.../S... (e.g., B3/S23).");
            }
        }

        private void AddError(string propertyName, string errorMessage)
        {
            if (!_errors.ContainsKey(propertyName))
            {
                _errors[propertyName] = [];
            }

            if (!_errors[propertyName].Contains(errorMessage))
            {
                _errors[propertyName].Add(errorMessage);
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        private void ClearErrors(string propertyName)
        {
            if (_errors.Remove(propertyName))
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        public int GetWidth() => int.Parse(BoardWidth);
        public int GetHeight() => int.Parse(BoardHeight);
        public Color GetAliveColor() => AliveColor;
        public Color GetDeadColor() => DeadColor;
        public CellShape GetShape() => SelectedCellShape;

        partial void OnBoardWidthChanged(string value) => Validate();
        partial void OnBoardHeightChanged(string value) => Validate();
        partial void OnRuleStringChanged(string value) => Validate();
    }
}