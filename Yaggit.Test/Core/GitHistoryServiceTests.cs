namespace Yaggit.Test.Core;

[TestFixture]
public class GitHistoryServiceTests
{
    private Mock<IGitCommandService> _gitMock = null!;
    private GitHistoryService _service = null!;

    [SetUp]
    public void Setup()
    {
        _gitMock = new Mock<IGitCommandService>();
        _service = new GitHistoryService(new NullLogger<GitHistoryService>(), _gitMock.Object);
    }

    /// <summary>
    /// Пустой вывод git log → пустой список.
    /// </summary>
    [Test]
    public async Task GetCommitHistoryAsync_ReturnsEmpty_WhenOutputEmpty()
    {
        _gitMock
            .Setup(x => x.RunAsync(It.IsAny<string>()))
            .ReturnsAsync(new GitCommandResult(0, "", ""));

        var commits = await _service.GetCommitHistoryAsync("main");

        Assert.That(commits, Is.Empty);
    }

    /// <summary>
    /// Один корректный коммит должен корректно распарситься.
    /// </summary>
    [Test]
    public async Task GetCommitHistoryAsync_ParsesSingleCommit()
    {
        var output = "abcd1234|John Doe|2024-05-01 12:00:00 +0300|Initial commit";

        _gitMock
            .Setup(x => x.RunAsync(It.IsAny<string>()))
            .ReturnsAsync(new GitCommandResult(0, output, ""));

        var commits = await _service.GetCommitHistoryAsync("main");

        Assert.That(commits, Has.Count.EqualTo(1));

        var c = commits[0];
        Assert.That(c.Hash, Is.EqualTo("abcd1234"));
        Assert.That(c.Author, Is.EqualTo("John Doe"));
        Assert.That(c.Message, Is.EqualTo("Initial commit"));
    }

    /// <summary>
    /// Несколько коммитов должны корректно разобраться в список.
    /// </summary>
    [Test]
    public async Task GetCommitHistoryAsync_ParsesMultipleCommits()
    {
        var output =
            "111|Alice|2024-05-01 10:00:00 +0300|Fix bug\n" +
            "222|Bob|2024-05-02 11:00:00 +0300|Add feature";

        _gitMock
            .Setup(x => x.RunAsync(It.IsAny<string>()))
            .ReturnsAsync(new GitCommandResult(0, output, ""));

        var commits = await _service.GetCommitHistoryAsync("develop");

        Assert.That(commits, Has.Count.EqualTo(2));

        Assert.That(commits[0].Hash, Is.EqualTo("111"));
        Assert.That(commits[1].Hash, Is.EqualTo("222"));
    }

    /// <summary>
    /// Некорректный формат строки должен вызвать ArgumentOutOfRangeException.
    /// </summary>
    [Test]
    public void GetCommitHistoryAsync_Throws_OnMalformedLine()
    {
        var output = "invalid|line|missing";

        _gitMock
            .Setup(x => x.RunAsync(It.IsAny<string>()))
            .ReturnsAsync(new GitCommandResult(0, output, ""));

        Assert.ThrowsAsync<FormatException>(() => _service.GetCommitHistoryAsync("main"));
    }

    /// <summary>
    /// git log возвращает ошибку → пробрасывается GitException.
    /// </summary>
    [Test]
    public void GetCommitHistoryAsync_Throws_WhenGitFails()
    {
        _gitMock
            .Setup(x => x.RunAsync(It.IsAny<string>()))
            .ThrowsAsync(new GitException("fatal error", 1));

        Assert.ThrowsAsync<GitException>(
            () => _service.GetCommitHistoryAsync("main"));
    }

    /// <summary>
    /// Проверка, что команда git log вызывается корректно.
    /// </summary>
    [Test]
    public async Task GetCommitHistoryAsync_CallsGitLog_WithCorrectArguments()
    {
        _gitMock
            .Setup(x => x.RunAsync(It.IsAny<string>()))
            .ReturnsAsync(new GitCommandResult(0, "", ""));

        await _service.GetCommitHistoryAsync("feature/test", 50);

        _gitMock.Verify(
            x => x.RunAsync(
                It.Is<string>(args =>
                    args.StartsWith("log feature/test") &&
                    args.Contains("--max-count=50") &&
                    args.Contains("--pretty=format:%H|%an|%ad|%s")
                )
            ),
            Times.Once
        );
    }

    /// <summary>
    /// Пустое имя ветки → ArgumentException.
    /// </summary>
    [Test]
    public void GetCommitHistoryAsync_Throws_WhenBranchNameEmpty()
    {
        Assert.ThrowsAsync<ArgumentException>(
            () => _service.GetCommitHistoryAsync(""));
    }
}
