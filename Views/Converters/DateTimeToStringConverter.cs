namespace Views.Converters;

public sealed class DateTimeToStringConverter : IValueConverter
{
    public string DateFormat { get; set; } = "d";

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is DateTime dateTime
            ? dateTime.ToString(DateFormat, culture)
            : BindingOperations.DoNothing;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string s || s.Length == 0)
            return BindingOperations.DoNothing;

        return DateTime.TryParse(s, culture, DateTimeStyles.None, out var result)
            ? result
            : BindingOperations.DoNothing;
    }
}