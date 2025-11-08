using Avalonia.Media;
using Views.Converters;

namespace Yaggit.Test.UI.Converters;

public class ConsoleLineTypeToBrushConverterTests
{
    private readonly ConsoleLineTypeToBrushConverter _c = new();

    [Test]
    public void Convert_Command_ReturnsBlue()
    {
        var result = _c.Convert(eConsoleLineType.Command, typeof(IBrush), null, null!);
        Assert.That(result, Is.EqualTo(Brushes.LightSkyBlue));
    }

    [Test]
    public void Convert_Error_ReturnsRed()
    {
        var result = _c.Convert(eConsoleLineType.Error, typeof(IBrush), null, null!);
        Assert.That(result, Is.EqualTo(Brushes.IndianRed));
    }

    [Test]
    public void Convert_InvalidType_ReturnsWhite()
    {
        var result = _c.Convert("aaa", typeof(IBrush), null, null!);
        Assert.That(result, Is.EqualTo(Brushes.White));
    }
}