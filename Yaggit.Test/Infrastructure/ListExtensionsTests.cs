using Models.Extensions;

namespace Yaggit.Test.Infrastructure;

[TestFixture]
public class ListExtensionsTests
{
    [Test]
    public void ToSeparatedString_ShouldJoinElementsWithDefaultSeparator()
    {
        var list = new List<int> { 1, 2, 3 };
        var result = list.ToSeparatedString();

        Assert.That(result, Is.EqualTo("1; 2; 3"));
    }

    [Test]
    public void ToSeparatedString_ShouldUseCustomSeparator()
    {
        var list = new List<string> { "A", "B", "C" };
        var result = list.ToSeparatedString(", ");

        Assert.That(result, Is.EqualTo("A, B, C"));
    }

    [Test]
    public void ToSeparatedString_ShouldReturnEmptyString_WhenListIsNull()
    {
        List<string>? list = null;
        var result = list.ToSeparatedString();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void ToSeparatedString_ShouldHandleNullElements_WithPlaceholder()
    {
        var list = new List<string?> { "One", null, "Three" };
        var result = list.ToSeparatedString(", ", "[null]");

        Assert.That(result, Is.EqualTo("One, [null], Three"));
    }

    [Test]
    public void ToSeparatedString_ShouldReturnEmptyString_WhenListIsEmpty()
    {
        var list = new List<int>();
        var result = list.ToSeparatedString();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void ToMultilineString_ShouldReturnElementsEachOnNewLine_WithSeparator()
    {
        var list = new List<string> { "A", "B" };
        var result = list.ToMultilineString();

        var expected = $"A;{Environment.NewLine}B;";
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ToMultilineString_ShouldReturnElementsWithoutSeparator_WhenDisabled()
    {
        var list = new List<string> { "A", "B" };
        var result = list.ToMultilineString(addSeparator: false);

        var expected = $"A{Environment.NewLine}B";
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ToMultilineString_ShouldReturnEmptyString_WhenNull()
    {
        List<int>? list = null;
        var result = list.ToMultilineString();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void ToHexString_ShouldConvertBytesToHex()
    {
        var bytes = new byte[] { 0xAF, 0x12, 0x3C };
        var result = bytes.ToHexString();

        Assert.That(result, Is.EqualTo("AF 12 3C"));
    }

    [Test]
    public void ToHexString_ShouldReturnEmptyString_WhenNull()
    {
        byte[]? bytes = null;
        var result = bytes.ToHexString();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void ToHexString_ShouldUseCustomSeparator()
    {
        var bytes = new byte[] { 0xAA, 0xBB };
        var result = bytes.ToHexString("-");

        Assert.That(result, Is.EqualTo("AA-BB"));
    }

    [Test]
    public void ToDebugString_ShouldReturnJsonLikeArray()
    {
        var list = new List<string> { "apple", "banana" };
        var result = list.ToDebugString();

        Assert.That(result, Is.EqualTo("[\"apple\", \"banana\"]"));
    }

    [Test]
    public void ToDebugString_ShouldReturnEmptyArray_WhenNull()
    {
        List<int>? list = null;
        var result = list.ToDebugString();

        Assert.That(result, Is.EqualTo("[]"));
    }

    [Test]
    public void ToDebugString_ShouldHandleEmptyList()
    {
        var list = new List<int>();
        var result = list.ToDebugString();

        Assert.That(result, Is.EqualTo("[]"));
    }

    [Test]
    public void ToDebugString_ShouldHandleNullElements()
    {
        var list = new List<string?> { "A", null, "B" };
        var result = list.ToDebugString();

        Assert.That(result, Is.EqualTo("[\"A\", \"\", \"B\"]"));
    }
}
