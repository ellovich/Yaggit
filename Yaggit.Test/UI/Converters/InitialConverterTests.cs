using System.Globalization;
using Avalonia.Data;
using Views.Converters;

namespace Yaggit.Test.UI.Converters;

public class InitialConverterTests
{
    private readonly InitialConverter _c = new();

    [Test]
    public void Convert_NormalString_ReturnsFirstUpperLetter()
    {
        var result = _c.Convert("hello", typeof(string), null, CultureInfo.InvariantCulture);
        Assert.That(result, Is.EqualTo("H"));
    }

    [Test]
    public void Convert_EmptyString_ReturnsEmpty()
    {
        var result = _c.Convert("", typeof(string), null, null!);
        Assert.That(result, Is.EqualTo(""));
    }

    [Test]
    public void Convert_Null_ReturnsEmpty()
    {
        var result = _c.Convert(null, typeof(string), null, null!);
        Assert.That(result, Is.EqualTo(""));
    }

    [Test]
    public void ConvertBack_Always_DoNothing()
    {
        var result = _c.ConvertBack("x", typeof(string), null, null!);
        Assert.That(result, Is.EqualTo(BindingOperations.DoNothing));
    }
}