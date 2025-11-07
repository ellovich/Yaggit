using Avalonia.Media;
using Models.Services.Yaggit.Contracts;

namespace Views.Converters;

public sealed class ConsoleLineTypeToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is eConsoleLineType type
            ? type switch
            {
                eConsoleLineType.Command => Brushes.LightSkyBlue,
                eConsoleLineType.Output => Brushes.LightGreen,
                eConsoleLineType.Error => Brushes.IndianRed,
                _ => Brushes.White
            }
            : Brushes.White;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => BindingOperations.DoNothing;
}