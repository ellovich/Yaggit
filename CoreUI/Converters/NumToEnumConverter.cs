using System.Collections;
using System.Globalization;
using Avalonia.Data.Converters;

namespace CoreUI.Converters;

public class NumToEnumConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
            return null;

        if (parameter is not Type enumType)
            throw new ArgumentException("Параметр не должен быть null и должен быть типом перечисления!", nameof(parameter));

        if (!enumType.IsEnum)
            throw new ArgumentException("Параметр должен иметь тип перечисления!", nameof(parameter));

        // Если значение является коллекцией (но не строкой), обрабатываем каждый элемент
        if (value is IEnumerable enumerable && value is not string)
        {
            var descriptions = enumerable.Cast<object>()
                .Select(item =>
                {
                    // Преобразуем значение в элемент перечисления
                    var enumValue = Enum.Parse(enumType, item.ToString()!);
                    // Получаем описание элемента (расширение GetDescription())
                    return ((Enum)enumValue).GetDescription();
                })
                .OrderBy(desc => desc)
                .ToList();
            return descriptions;
        }
        else
        {
            var enumValue = Enum.Parse(enumType, value.ToString()!);
            return ((Enum)enumValue).GetDescription();
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
            return null;

        if (parameter is not Type enumType)
            throw new ArgumentException("Параметр не должен быть null и должен быть типом перечисления!", nameof(parameter));

        if (!enumType.IsEnum)
            throw new ArgumentException("Параметр должен иметь тип перечисления!", nameof(parameter));

        // Получаем строковое описание и возвращаем соответствующее значение перечисления.
        string description = value.ToString()!;
        return enumType.GetValueByDescription(description);
    }
}