namespace Yaggit.Test.Core;

/// <summary>
/// Набор тестов для проверки работы <see cref="GitBranchesService"/>.
/// </summary>
[TestFixture]
public class GitBranchesServiceTests
{
    private Mock<IGitCommandService> _gitMock = null!;
    private GitBranchesService _service = null!;

    /// <summary>
    /// Инициализация зависимостей перед каждым тестом.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        _gitMock = new Mock<IGitCommandService>();
        _service = new GitBranchesService(new NullLogger<GitBranchesService>(), _gitMock.Object);
    }

    /// <summary>
    /// Проверяет, что при отсутствии веток возвращается пустой список.
    /// </summary>
    [Test]
    public async Task GetBranchesAsync_ReturnsEmptyList_WhenNoBranchesExist()
    {
        _gitMock.Setup(x => x.RunAsync("branch --list"))
                .ReturnsAsync(new GitCommandResult(0, string.Empty, string.Empty));

        var branches = await _service.GetBranchesAsync();

        Assert.That(branches, Is.Empty);
    }

    /// <summary>
    /// Проверяет корректный парсинг списка веток из вывода git.
    /// </summary>
    [Test]
    public async Task GetBranchesAsync_ParsesBranchesCorrectly()
    {
        var output = "* main\n  develop\n  feature/test\n";
        _gitMock.Setup(x => x.RunAsync("branch --list"))
                .ReturnsAsync(new GitCommandResult(0, output, string.Empty));

        var branches = await _service.GetBranchesAsync();

        Assert.That(branches, Has.Count.EqualTo(3));
        Assert.That(branches.Single(x => x.IsCurrent).Name, Is.EqualTo("main"));
    }

    /// <summary>
    /// Проверяет, что исключение Git корректно пробрасывается наружу.
    /// </summary>
    [Test]
    public void GetBranchesAsync_Throws_WhenGitFails()
    {
        _gitMock.Setup(x => x.RunAsync("branch --list"))
                .ThrowsAsync(new GitException("Ошибка git", 1));

        Assert.ThrowsAsync<GitException>(() => _service.GetBranchesAsync());
    }

    /// <summary>
    /// Проверяет, что пустое имя ветки вызывает исключение при checkout.
    /// </summary>
    [Test]
    public void CheckoutBranchAsync_Throws_WhenNameEmpty()
    {
        Assert.ThrowsAsync<ArgumentException>(() => _service.CheckoutBranchAsync(""));
    }

    /// <summary>
    /// Проверяет вызов git checkout при переключении ветки.
    /// </summary>
    [Test]
    public async Task CheckoutBranchAsync_CallsGit()
    {
        _gitMock.Setup(x => x.RunAsync("checkout develop"))
                .ReturnsAsync(new GitCommandResult(0, "", ""));

        await _service.CheckoutBranchAsync("develop");

        _gitMock.Verify(x => x.RunAsync("checkout develop"), Times.Once);
    }

    /// <summary>
    /// Проверяет корректный вызов удаления ветки с флагом -D.
    /// </summary>
    [Test]
    public async Task DeleteBranchAsync_CallsGit_WithForce()
    {
        _gitMock.Setup(x => x.RunAsync("branch -D old"))
                .ReturnsAsync(new GitCommandResult(0, "", ""));

        await _service.DeleteBranchAsync("old", force: true);

        _gitMock.Verify(x => x.RunAsync("branch -D old"), Times.Once);
    }

    /// <summary>
    /// Проверяет корректный вызов переименования ветки.
    /// </summary>
    [Test]
    public async Task RenameBranchAsync_CallsGit()
    {
        _gitMock.Setup(x => x.RunAsync("branch -m old new"))
                .ReturnsAsync(new GitCommandResult(0, "", ""));

        await _service.RenameBranchAsync("old", "new");

        _gitMock.Verify(x => x.RunAsync("branch -m old new"), Times.Once);
    }

    /// <summary>
    /// Проверяет валидацию при удалении ветки с пустым именем.
    /// </summary>
    [Test]
    public void DeleteBranchAsync_Throws_WhenNameEmpty()
    {
        Assert.ThrowsAsync<ArgumentException>(() => _service.DeleteBranchAsync(""));
    }

    /// <summary>
    /// Проверяет валидацию при переименовании ветки с пустым именем.
    /// </summary>
    [Test]
    public void RenameBranchAsync_Throws_WhenNameEmpty()
    {
        Assert.ThrowsAsync<ArgumentException>(() => _service.RenameBranchAsync("", "new"));
    }
}
