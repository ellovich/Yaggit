using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Models.Services.Yaggit.Contracts;

namespace Views.Converters;

public class ConsoleLineTypeToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is eConsoleLineType type)
        {
            return type switch
            {
                eConsoleLineType.Command => Brushes.LightSkyBlue,
                eConsoleLineType.Output => Brushes.LightGreen,
                eConsoleLineType.Error => Brushes.IndianRed,
                _ => Brushes.White
            };
        }
        return Brushes.White;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}