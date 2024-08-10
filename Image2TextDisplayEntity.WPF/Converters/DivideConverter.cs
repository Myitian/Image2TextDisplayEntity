using System.Globalization;
using System.Windows.Data;

namespace Myitian.Converters;

public class DivideConverter : IMultiValueConverter
{
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length == 0)
            return Binding.DoNothing;
        if (values.Length == 1)
            return values[0];
        static (long?, ulong?, double?, decimal?) GetNumber(object v) => v switch
        {
            sbyte i => (i, null, null, null),
            short i => (i, null, null, null),
            int i => (i, null, null, null),
            long i => (i, null, null, null),
            byte u => (u, null, null, null),
            ushort u => (u, null, null, null),
            uint u => (u, null, null, null),
            ulong u => (null, u, null, null),
            float d => (null, null, d, null),
            double d => (null, null, d, null),
            decimal m => (null, null, null, m),
            _ => (null, null, null, null),
        };
        (long? i0, ulong? u0, double? d0, decimal? m0) = GetNumber(values[0]);
        (long? i1, ulong? u1, double? d1, decimal? m1) = GetNumber(values[1]);
        if (i0 is not null)
        {
            if (i1 is not null)
                return i0 / i1;
            else if (u1 is not null)
                return i0 / (long)u1;
            else if (d1 is not null)
                return i0 / d1;
            else if (m1 is not null)
                return i0 / m1;
        }
        else if (u0 is not null)
        {
            if (i1 is not null)
                return u0 / (ulong)i1;
            else if (u1 is not null)
                return u0 / u1;
            else if (d1 is not null)
                return u0 / d1;
            else if (m1 is not null)
                return u0 / m1;
        }
        else if (d0 is not null)
        {
            if (i1 is not null)
                return d0 / i1;
            else if (u1 is not null)
                return d0 / u1;
            else if (d1 is not null)
                return d0 / d1;
            else if (m1 is not null)
                return d0 / (double)m1;
        }
        else if (m0 is not null)
        {
            if (i1 is not null)
                return m0 / i1;
            else if (u1 is not null)
                return m0 / u1;
            else if (d1 is not null)
                return m0 / (decimal)d1;
            else if (m1 is not null)
                return m0 / m1;
        }
        return null;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
