using Avalonia.Data;
using Views.Converters;

namespace Yaggit.Test.UI.Converters;

public class DateVisibilityConverterTests
{
    private readonly DateVisibilityConverter _c = new();

    [Test]
    public void Convert_DefaultDate_ReturnsFalse()
    {
        var result = _c.Convert(default(DateTime), typeof(bool), null, null!);
        Assert.That(result, Is.False);
    }

    [Test]
    public void Convert_ValidDate_ReturnsTrue()
    {
        var dt = new DateTime(2024, 1, 1);
        var result = _c.Convert(dt, typeof(bool), null, null!);
        Assert.That(result, Is.True);
    }

    [Test]
    public void Convert_InvalidType_ReturnsFalse()
    {
        var result = _c.Convert("string", typeof(bool), null, null!);
        Assert.That(result, Is.False);
    }

    [Test]
    public void ConvertBack_Always_DoNothing()
    {
        var res = _c.ConvertBack("x", typeof(DateTime), null, null!);
        Assert.That(res, Is.EqualTo(BindingOperations.DoNothing));
    }
}