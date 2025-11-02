using System.ComponentModel;
using System.Globalization;
using Avalonia.Data.Converters;

namespace CoreUI.Converters;

internal class EnumDescriptionConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Enum enumValue)
        {
            var field = enumValue.GetType().GetField(enumValue.ToString());
            var attribute = (DescriptionAttribute?)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute?.Description ?? enumValue.ToString();
        }

        return "";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string stringValue && targetType.IsEnum)
        {
            foreach (var field in targetType.GetFields())
            {
                var attribute = (DescriptionAttribute?)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
                if (attribute?.Description == stringValue)
                {
                    return Enum.Parse(targetType, field.Name);
                }
            }
        }

        return Enum.Parse(targetType, value?.ToString() ?? string.Empty);
    }
}