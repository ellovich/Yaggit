using System.Globalization;
using Avalonia.Data.Converters;

namespace CoreUI.Converters;

public class NavMenuWidthToCollapsedMultiConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count >= 2 && values[0] is double actualWidth && values[1] is double collapsedWidth)
            return actualWidth < collapsedWidth;
        return false;
    }
}