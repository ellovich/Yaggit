using System.Globalization;
using Avalonia.Data;
using Views.Converters;

namespace Yaggit.Test.UI.Converters;

public class DateTimeToStringConverterTests
{
    private readonly DateTimeToStringConverter _c = new();

    [Test]
    public void Convert_FormatsCorrectly()
    {
        var dt = new DateTime(2024, 2, 5);
        var result = _c.Convert(dt, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.That(result, Is.EqualTo(dt.ToString(_c.DateFormat, CultureInfo.InvariantCulture)));
    }

    [Test]
    public void Convert_InvalidValue_DoNothing()
    {
        var result = _c.Convert("abc", typeof(string), null, null!);
        Assert.That(result, Is.EqualTo(BindingOperations.DoNothing));
    }

    [Test]
    public void ConvertBack_ValidString_ReturnsDate()
    {
        var result = _c.ConvertBack("05.02.2024", typeof(DateTime), null, CultureInfo.InvariantCulture);
        Assert.That(result, Is.TypeOf<DateTime>());
    }

    [Test]
    public void ConvertBack_InvalidString_DoNothing()
    {
        var result = _c.ConvertBack("xxx", typeof(DateTime), null, CultureInfo.InvariantCulture);
        Assert.That(result, Is.EqualTo(BindingOperations.DoNothing));
    }

    [Test]
    public void ConvertBack_EmptyString_DoNothing()
    {
        var result = _c.ConvertBack("", typeof(DateTime), null, CultureInfo.InvariantCulture);
        Assert.That(result, Is.EqualTo(BindingOperations.DoNothing));
    }
}