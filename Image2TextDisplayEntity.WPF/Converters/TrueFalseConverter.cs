using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace Myitian.Converters;

public class TrueFalseConverter : IValueConverter
{
    public object? Convert(object values, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is not IList collection)
            return Binding.DoNothing;
        if (values is true)
            return collection[0];
        else
            return collection[1];
    }

    public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
