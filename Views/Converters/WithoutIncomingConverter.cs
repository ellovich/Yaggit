using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Views.Converters;

public class WithoutIncomingConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int isWithoutIncoming && isWithoutIncoming == 1)
            return TextDecorations.Underline;

        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}