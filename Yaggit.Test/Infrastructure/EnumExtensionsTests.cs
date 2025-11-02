using Models.Enums.Common;
using Models.Extensions;

namespace Yaggit.Test.Infrastructure;

public enum eTestEnum
{
    [Comment("This is the first value"), AvaColor(eAvaColor.Red), System.ComponentModel.Description("First value description")]
    First = 1,

    [AvaColor(eAvaColor.Green)]
    [System.ComponentModel.Description("Second value with format {0}")]
    [Comment("Second comment")]
    Second = 2,

    [AvaColor(eAvaColor.Blue)]
    NoDescription = 3,

    NoAttributes = 4
}

[TestFixture]
public class EnumExtensionsTests
{
    [Test]
    public void GetDescription_ShouldReturnDescription_WhenDescriptionAttributeExists()
    {
        // Act
        var desc = eTestEnum.First.GetDescription();

        // Assert
        Assert.That(desc, Is.EqualTo("First value description"));
    }

    [Test]
    public void GetDescription_ShouldReturnFormattedDescription_WhenArgumentsProvided()
    {
        var desc = eTestEnum.Second.GetDescription("ARG");
        Assert.That(desc, Is.EqualTo("Second value with format ARG"));
    }

    [Test]
    public void GetDescription_ShouldReturnEnumName_WhenNoDescriptionAttribute()
    {
        var desc = eTestEnum.NoDescription.GetDescription();
        Assert.That(desc, Is.EqualTo("NoDescription"));
    }

    [Test]
    public void GetComment_ShouldReturnComment_WhenCommentAttributeExists()
    {
        var comment = eTestEnum.First.GetComment();
        Assert.That(comment, Is.EqualTo("This is the first value"));
    }

    [Test]
    public void GetComment_ShouldReturnNull_WhenNoCommentAttribute()
    {
        var comment = eTestEnum.NoDescription.GetComment();
        Assert.That(comment, Is.Null);
    }

    [Test]
    public void GetAvaColor_ShouldReturnCorrectColor_WhenAttributeExists()
    {
        var color = eTestEnum.Second.GetAvaColor();
        Assert.That(color, Is.EqualTo(eAvaColor.Green));
    }

    [Test]
    public void GetAvaColor_ShouldReturnNone_WhenNoAttribute()
    {
        var color = eTestEnum.NoAttributes.GetAvaColor();
        Assert.That(color, Is.EqualTo(eAvaColor.None));
    }

    [Test]
    public void GetAvaColorClass_ShouldReturnColorName()
    {
        var cls = eTestEnum.Second.GetAvaColorClass();
        Assert.That(cls, Is.EqualTo("Green"));
    }

    [Test]
    public void ToEnum_ShouldConvertIntToEnum()
    {
        var val = 1.ToEnum<eTestEnum>();
        Assert.That(val, Is.EqualTo(eTestEnum.First));
    }

    [Test]
    public void GetDescription_FromNullableInt_ShouldReturnDescription()
    {
        int? val = 1;
        var desc = val.GetDescription<eTestEnum>();
        Assert.That(desc, Is.EqualTo("First value description"));
    }

    [Test]
    public void GetDescription_FromNullableInt_ShouldReturnNull_WhenNull()
    {
        int? val = null;
        var desc = val.GetDescription<eTestEnum>();
        Assert.That(desc, Is.Null);
    }

    [Test]
    public void GetEnumAttribute_ShouldBeCached_AfterFirstCall()
    {
        // Проверяем, что повторный вызов возвращает тот же результат
        var desc1 = eTestEnum.First.GetDescription();
        var desc2 = eTestEnum.First.GetDescription();
        Assert.That(desc1, Is.EqualTo(desc2));
    }
}
