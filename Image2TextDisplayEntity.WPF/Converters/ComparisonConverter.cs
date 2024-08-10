using System.Globalization;
using System.Windows.Data;

namespace Myitian.Converters;

public class ComparisonConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.Equals(parameter);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return true.Equals(value) ? parameter : Binding.DoNothing;
    }
}
