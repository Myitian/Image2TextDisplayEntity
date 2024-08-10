using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Myitian.Converters;

public class ColorHexConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Color c)
            return null;
        return $"#{c.R:X2}{c.G:X2}{c.B:X2}{c.A:X2}";
    }
    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string s)
            return null;
        if (s.Length != 9 || s[0] != '#')
            return null;
        if (!uint.TryParse(s, NumberStyles.HexNumber, null, out uint result))
            return null;
        byte r = (byte)(result >> 24);
        byte g = (byte)(result >> 16);
        byte b = (byte)(result >> 8);
        byte a = (byte)result;
        return Color.FromArgb(a, r, g, b);
    }
}
