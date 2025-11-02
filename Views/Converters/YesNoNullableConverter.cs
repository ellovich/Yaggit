using System.Globalization;
using Avalonia.Data.Converters;

namespace Views.Converters;

/// <summary>
/// Конвертер: ноль -> нет, любое другое число -> да,
/// </summary>
public class YesNoNullableConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int valueInt)
            return valueInt == 0 ? "нет" : "да";

        if (value is byte valueByte)
            return valueByte == 0 ? "нет" : "да";

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string valueStr)
            return valueStr == "да" ? 1 : 0;

        return null;
    }
}