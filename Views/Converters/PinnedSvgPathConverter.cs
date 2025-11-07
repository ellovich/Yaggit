namespace Views.Converters;

public sealed class PinnedSvgPathConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool isPinned
            ? (isPinned ? Icons.unpin : Icons.pin)
            : BindingOperations.DoNothing;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => BindingOperations.DoNothing;
}