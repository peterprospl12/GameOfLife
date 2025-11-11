using System.Collections;
using System.ComponentModel;

namespace GameOfLife.WPF.ViewModels
{
    public class TextFieldSettingViewModel : SettingViewModelBase, INotifyDataErrorInfo
    {
        private string _value;
        private readonly Func<string, string?> _validator;
        private readonly Dictionary<string, List<string>> _errors = new();

        public string Value
        {
            get => _value;
            set
            {
                if (SetProperty(ref _value, value))
                {
                    ValidateProperty(value, nameof(Value));
                }
            }
        }

        public TextFieldSettingViewModel(string label, string initialValue, Func<string, string?> validator) : base(label)
        {
            _value = initialValue;
            _validator = validator;
            ValidateProperty(initialValue, nameof(Value));
        }

        public bool HasErrors => _errors.Any();

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
        {
            return _errors.GetValueOrDefault(propertyName ?? string.Empty, new List<string>());
        }

        private void ValidateProperty(string value, string propertyName)
        {
            ClearErrors(propertyName);
            var errorMessage = _validator(value);
            if (errorMessage != null)
            {
                AddError(propertyName, errorMessage);
            }
            OnPropertyChanged(nameof(HasErrors));
        }

        private void AddError(string propertyName, string error)
        {
            if (!_errors.ContainsKey(propertyName))
            {
                _errors[propertyName] = new List<string>();
            }

            if (!_errors[propertyName].Contains(error))
            {
                _errors[propertyName].Add(error);
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
    }
}
