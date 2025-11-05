using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GameOfLife.Core.Models
{
    public class Statistics : INotifyPropertyChanged
    {
        private int generation = 0;
        private int born = 0;
        private int died = 0;
        private int totalBorn = 0;
        private int totalDied = 0;

        public int Generation
        {
            get => generation;
            private set => SetField(ref generation, value);
        }

        public int Born
        {
            get => born;
            private set => SetField(ref born, value);
        }

        public int Died
        {
            get => died;
            private set => SetField(ref died, value);
        }

        public int TotalBorn
        {
            get => totalBorn;
            private set => SetField(ref totalBorn, value);
        }

        public int TotalDied
        {
            get => totalDied;
            private set => SetField(ref totalDied, value);
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