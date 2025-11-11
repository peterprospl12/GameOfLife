using System.Globalization;
using System.Windows.Data;

namespace GameOfLife.WPF.Converters
{
    public class MultiplyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is [IConvertible val1, IConvertible val2])
            {
                try
                {
                    return val1.ToDouble(culture) * val2.ToDouble(culture);
                }
                catch (FormatException)
                {
                    return Binding.DoNothing;
                }
            }

            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
