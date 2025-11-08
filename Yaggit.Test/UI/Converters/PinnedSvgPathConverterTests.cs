using Avalonia.Data;
using CoreUI.Assets;
using Views.Converters;

namespace Yaggit.Test.UI.Converters;

public class PinnedSvgPathConverterTests
{
    private readonly PinnedSvgPathConverter _c = new();

    [Test]
    public void Convert_True_ReturnsUnpinIcon()
    {
        var result = _c.Convert(true, typeof(string), null, null!);
        Assert.That(result, Is.EqualTo(Icons.unpin));
    }

    [Test]
    public void Convert_False_ReturnsPinIcon()
    {
        var result = _c.Convert(false, typeof(string), null, null!);
        Assert.That(result, Is.EqualTo(Icons.pin));
    }

    [Test]
    public void Convert_InvalidType_DoNothing()
    {
        var result = _c.Convert("abc", typeof(string), null, null!);
        Assert.That(result, Is.EqualTo(BindingOperations.DoNothing));
    }

    [Test]
    public void ConvertBack_Always_DoNothing()
    {
        var result = _c.ConvertBack("x", typeof(string), null, null!);
        Assert.That(result, Is.EqualTo(BindingOperations.DoNothing));
    }
}