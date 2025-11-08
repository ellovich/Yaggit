using Avalonia.Data;
using CoreUI.Assets;
using ViewModels.UI.Yaggit;
using Views.Converters;

namespace Yaggit.Test.UI.Converters;

public class BranchNodeTypeToSvgPathTests
{
    private readonly BranchNodeTypeToSvgPath _c = new();

    [Test]
    public void Convert_Repository_ReturnsGitLogo()
    {
        var result = _c.Convert(eBranchNodeType.Repository, typeof(string), null, null!);
        Assert.That(result, Is.EqualTo(Icons.gitLogo));
    }

    [Test]
    public void Convert_Branch_ReturnsBranchIcon()
    {
        var result = _c.Convert(eBranchNodeType.Branch, typeof(string), null, null!);
        Assert.That(result, Is.EqualTo(Icons.Vs22.Git.Branch));
    }

    [Test]
    public void Convert_InvalidValue_ReturnsBug()
    {
        var result = _c.Convert("aaa", typeof(string), null, null!);
        Assert.That(result, Is.EqualTo(Icons.bug));
    }

    [Test]
    public void ConvertBack_Always_DoNothing()
    {
        var result = _c.ConvertBack("x", typeof(string), null, null!);
        Assert.That(result, Is.EqualTo(BindingOperations.DoNothing));
    }
}