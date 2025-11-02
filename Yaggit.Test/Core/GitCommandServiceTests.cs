namespace Yaggit.Test.Core;

/// <summary>
/// Тесты для <see cref="GitCommandService"/>.
/// </summary>
[TestFixture]
public class GitCommandServiceTests
{
    private Mock<IGitRepositoryContext> _contextMock = null!;
    private GitCommandService _service = null!;

    /// <summary>
    /// Настройка тестовой среды перед каждым тестом.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        _contextMock = new Mock<IGitRepositoryContext>();
        _contextMock.Setup(x => x.RepositoryPath).Returns(Environment.CurrentDirectory);
        _service = new GitCommandService(new NullLogger<GitCommandService>(), _contextMock.Object);
    }

    /// <summary>
    /// Проверяет исключение при отсутствии пути к репозиторию.
    /// </summary>
    [Test]
    public void RunAsync_ThrowsIfNoRepoPath()
    {
        _contextMock.Setup(x => x.RepositoryPath).Returns(string.Empty);
        Assert.ThrowsAsync<InvalidOperationException>(() => _service.RunAsync("status"));
    }

    /// <summary>
    /// Проверяет исключение при пустых аргументах команды.
    /// </summary>
    [Test]
    public void RunAsync_ThrowsIfArgsEmpty()
    {
        Assert.ThrowsAsync<ArgumentException>(() => _service.RunAsync(""));
    }

    /// <summary>
    /// Проверяет корректный результат успешной команды git.
    /// </summary>
    [Test]
    public async Task RunAsync_ReturnsResult_WhenGitSucceeds()
    {
        var result = await _service.RunAsync("--version");
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Output, Does.Contain("git"));
    }

    /// <summary>
    /// Проверяет выброс <see cref="GitException"/> при ошибке git.
    /// </summary>
    [Test]
    public void RunAsync_ThrowsGitException_WhenGitFails()
    {
        Assert.ThrowsAsync<GitException>(() => _service.RunAsync("nonexistent-command-xyz"));
    }

    /// <summary>
    /// Проверяет, что событие CommandExecuted вызывается при выполнении.
    /// </summary>
    [Test]
    public async Task RunAsync_RaisesCommandExecutedEvent()
    {
        string? captured = null;
        _service.CommandExecuted += (line) => captured = line.Text;

        await _service.RunAsync("--version");

        Assert.That(captured, Does.StartWith("git"));
    }
}
