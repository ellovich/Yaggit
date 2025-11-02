using System.Globalization;
using Avalonia.Data.Converters;

namespace Views.Converters;

/// <summary>
///
/// </summary>
public class NumToStringOrEmptyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int num)
            return num == 0 ? "" : num.ToString();

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Avalonia.Data.BindingOperations.DoNothing;
    }
}