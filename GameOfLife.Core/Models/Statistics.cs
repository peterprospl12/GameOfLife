namespace GameOfLife.Core.Models
{
    public class Statistics
    {
        public int Generation { get; private set; } = 0;
        public int Born { get; private set; } = 0;
        public int Died { get; private set; } = 0;
        public int TotalBorn { get; private set; } = 0;
        public int TotalDied { get; private set; } = 0;

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
    }
}
