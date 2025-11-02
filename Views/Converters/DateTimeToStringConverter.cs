using System.Globalization;
using Avalonia.Data.Converters;

namespace Views.Converters;

/// <summary>
///
/// </summary>
public class DateTimeToStringConverter : IValueConverter
{
    public string DateFormat { get; set; } = "d";

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dateTime)
            return dateTime.ToString(DateFormat);

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (string.IsNullOrEmpty(value as string))
            return null;

        if (DateTime.TryParse(value as string, culture, DateTimeStyles.None, out var result))
            return result;

        return Avalonia.Data.BindingOperations.DoNothing;
    }
}