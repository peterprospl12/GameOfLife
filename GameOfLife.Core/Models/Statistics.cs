using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GameOfLife.Core.Models
{
    public class Statistics : INotifyPropertyChanged
    {
        private int _generation;
        private int _born;
        private int _died;
        private int _totalBorn;
        private int _totalDied;

        public int Generation
        {
            get => _generation;
            private set => SetField(ref _generation, value);
        }

        public int Born
        {
            get => _born;
            private set => SetField(ref _born, value);
        }

        public int Died
        {
            get => _died;
            private set => SetField(ref _died, value);
        }

        public int TotalBorn
        {
            get => _totalBorn;
            private set => SetField(ref _totalBorn, value);
        }

        public int TotalDied
        {
            get => _totalDied;
            private set => SetField(ref _totalDied, value);
        }

        public void Reset()
        {
            Generation = 0;
            Born = 0;
            Died = 0;
            TotalBorn = 0;
            TotalDied = 0;
        }

        public void Update(int bornThisStep, int diedThisStep)
        {
            if (bornThisStep < 0 || diedThisStep < 0)
            {
                throw new ArgumentOutOfRangeException("Born and died counts cannot be negative.");
            }

            Generation++;
            Born = bornThisStep;
            Died = diedThisStep;
            TotalBorn += bornThisStep;
            TotalDied += diedThisStep;
        }

        public override string ToString()
        {
            return $"Generation: {Generation}, Born: {Born}, Died: {Died}, Total Born: {TotalBorn}, Total Died: {TotalDied}";
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}