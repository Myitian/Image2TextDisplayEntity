using System.Globalization;
using System.Windows.Data;

namespace Myitian.Converters;

public class BooleanOrConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        foreach (object v in values)
        {
            switch (v)
            {
                case bool b when b:
                case sbyte i8 when !i8.Equals(0):
                case byte u8 when !u8.Equals(0):
                case short i16 when !i16.Equals(0):
                case ushort u16 when !u16.Equals(0):
                case int i32 when !i32.Equals(0):
                case uint u32 when !u32.Equals(0):
                case long i64 when !i64.Equals(0):
                case ulong u64 when !u64.Equals(0):
                case float fp32 when !fp32.Equals(0):
                case double fp64 when !fp64.Equals(0):
                case decimal dec when !dec.Equals(0):
                    return true;
            }
        }
        return false;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
