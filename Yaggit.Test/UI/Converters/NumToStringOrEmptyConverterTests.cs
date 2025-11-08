using System.Globalization;
using Avalonia.Data;
using Views.Converters;

namespace Yaggit.Test.UI.Converters;

public class NumToStringOrEmptyConverterTests
{
    private readonly NumToStringOrEmptyConverter _c = new();

    [Test]
    public void Convert_Zero_ReturnsEmpty()
    {
        var result = _c.Convert(0, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.That(result, Is.EqualTo(""));
    }

    [Test]
    public void Convert_PositiveInt_ReturnsStringValue()
    {
        var result = _c.Convert(42, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.That(result, Is.EqualTo("42"));
    }

    [Test]
    public void Convert_InvalidType_ReturnsDoNothing()
    {
        var result = _c.Convert("abc", typeof(string), null, null!);
        Assert.That(result, Is.EqualTo(BindingOperations.DoNothing));
    }

    [Test]
    public void ConvertBack_EmptyString_ReturnsZero()
    {
        var result = _c.ConvertBack("", typeof(int), null, CultureInfo.InvariantCulture);
        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public void ConvertBack_ValidNumber_ReturnsParsedInt()
    {
        var result = _c.ConvertBack("123", typeof(int), null, CultureInfo.InvariantCulture);
        Assert.That(result, Is.EqualTo(123));
    }

    [Test]
    public void ConvertBack_InvalidValue_ReturnsDoNothing()
    {
        var result = _c.ConvertBack("xx", typeof(int), null, CultureInfo.InvariantCulture);
        Assert.That(result, Is.EqualTo(BindingOperations.DoNothing));
    }
}