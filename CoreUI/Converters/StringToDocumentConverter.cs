using System.Globalization;
using Avalonia.Data.Converters;
using AvaloniaEdit.Document;

namespace CoreUI.Converters;

internal class StringToDocumentConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null) return null;

        var document = new TextDocument(value as string);

        return document;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}