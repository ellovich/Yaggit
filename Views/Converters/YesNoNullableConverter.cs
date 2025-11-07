namespace Views.Converters;

/// <summary>
/// Конвертер: ноль -> нет, любое другое число -> да,
/// </summary>
public sealed class YesNoNullableConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value switch
        {
            int v => v == 0 ? ViewModels.Lang.Common.String_No : ViewModels.Lang.Common.String_Yes,
            byte v => v == 0 ? ViewModels.Lang.Common.String_No : ViewModels.Lang.Common.String_Yes,
            long v => v == 0 ? ViewModels.Lang.Common.String_No : ViewModels.Lang.Common.String_Yes,
            _ => null
        };

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            value is string s
                ? s.Equals(ViewModels.Lang.Common.String_Yes, StringComparison.OrdinalIgnoreCase) ? 1 : 0
                : null;
}