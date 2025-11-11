using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using GameOfLife.Core.Enums;

namespace GameOfLife.WPF.Converters
{
    public class CellStateToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CellState state)
            {
                return state == CellState.Alive ? Brushes.Black : Brushes.Transparent;
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
