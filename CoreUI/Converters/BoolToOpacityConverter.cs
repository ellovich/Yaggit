using System.Globalization;
using Avalonia.Data.Converters;

namespace CoreUI.Converters;

public class BoolToOpacityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
            return 0.0f;

        if (value is bool isVisible)
        {
            return isVisible ? 1.0f : 0.0f;
        }

        if (value is object obj)
        {
            return obj != null ? 1.0f : 0.0f;
        }

        return 0.0f;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}