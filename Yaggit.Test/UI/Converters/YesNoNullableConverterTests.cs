using System.Globalization;
using Views.Converters;

namespace Yaggit.Test.UI.Converters;

public class YesNoNullableConverterTests
{
    private readonly YesNoNullableConverter _c = new();

    [Test]
    public void Convert_IntZero_ReturnsNo()
    {
        var result = _c.Convert(0, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.That(result, Is.EqualTo(ViewModels.Lang.Common.String_No));
    }

    [Test]
    public void Convert_IntNonZero_ReturnsYes()
    {
        var result = _c.Convert(5, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.That(result, Is.EqualTo(ViewModels.Lang.Common.String_Yes));
    }

    [Test]
    public void Convert_InvalidType_ReturnsNull()
    {
        var result = _c.Convert("abc", typeof(string), null, CultureInfo.InvariantCulture);
        Assert.That(result, Is.Null);
    }

    [Test]
    public void ConvertBack_Yes_Returns1()
    {
        var result = _c.ConvertBack(ViewModels.Lang.Common.String_Yes, typeof(int), null, null!);
        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void ConvertBack_No_Returns0()
    {
        var result = _c.ConvertBack(ViewModels.Lang.Common.String_No, typeof(int), null, null!);
        Assert.That(result, Is.EqualTo(0));
    }
}