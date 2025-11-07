using System.Numerics;
using Avalonia.Data;

namespace Views.Converters;

/// <summary>
/// Если ноль, то пусто.
/// </summary>
public sealed class NumToStringOrEmptyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value switch
        {
            int n => n == 0 
                ? string.Empty 
                : n.ToString(culture),
            _ => BindingOperations.DoNothing
        };

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is string s
            ? (string.IsNullOrWhiteSpace(s)
                ? 0 // "" → 0
                : int.TryParse(s, NumberStyles.Any, culture, out var n)
                    ? n // "123"
                    : BindingOperations.DoNothing) // "abc"
            : BindingOperations.DoNothing;
}