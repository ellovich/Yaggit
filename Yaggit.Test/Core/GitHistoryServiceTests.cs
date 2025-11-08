namespace Yaggit.Test.Core;

[TestFixture]
public class GitHistoryServiceTests
{
    private Mock<IGitCommandService> _gitMock = null!;
    private GitBranchHistoryService _service = null!;

    [SetUp]
    public void Setup()
    {
        _gitMock = new Mock<IGitCommandService>();
        _service = new GitBranchHistoryService(new NullLogger<GitBranchHistoryService>(), _gitMock.Object);
    }

    /// <summary>
    /// Пустой вывод git log → пустой список.
    /// </summary>
    [Test]
    public async Task GetBranchHistoryAsync_ReturnsEmpty_WhenOutputEmpty()
    {
        _gitMock
            .Setup(x => x.RunAsync(It.IsAny<string>()))
            .ReturnsAsync(new GitCommandResult(0, "", ""));

        var commits = await _service.GetBranchHistoryAsync("main");

        Assert.That(commits, Is.Empty);
    }

    /// <summary>
    /// Один корректный коммит должен корректно распарситься.
    /// </summary>
    [Test]
    public async Task GetBranchHistoryAsync_ParsesSingleCommit()
    {
        var output = "abcd1234|John Doe|2024-05-01 12:00:00 +0300|Initial commit";

        _gitMock
            .Setup(x => x.RunAsync(It.IsAny<string>()))
            .ReturnsAsync(new GitCommandResult(0, output, ""));

        var commits = await _service.GetBranchHistoryAsync("main");

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
    public async Task GetBranchHistoryAsync_ParsesMultipleCommits()
    {
        var output =
            "111|Alice|2024-05-01 10:00:00 +0300|Fix bug\n" +
            "222|Bob|2024-05-02 11:00:00 +0300|Add feature";

        _gitMock
            .Setup(x => x.RunAsync(It.IsAny<string>()))
            .ReturnsAsync(new GitCommandResult(0, output, ""));

        var commits = await _service.GetBranchHistoryAsync("develop");

        Assert.That(commits, Has.Count.EqualTo(2));

        Assert.That(commits[0].Hash, Is.EqualTo("111"));
        Assert.That(commits[1].Hash, Is.EqualTo("222"));
    }

    /// <summary>
    /// Некорректный формат строки должен вызвать ArgumentOutOfRangeException.
    /// </summary>
    [Test]
    public void GetBranchHistoryAsync_Throws_OnMalformedLine()
    {
        var output = "invalid|line|missing";

        _gitMock
            .Setup(x => x.RunAsync(It.IsAny<string>()))
            .ReturnsAsync(new GitCommandResult(0, output, ""));

        Assert.ThrowsAsync<FormatException>(() => _service.GetBranchHistoryAsync("main"));
    }

    /// <summary>
    /// git log возвращает ошибку → пробрасывается GitException.
    /// </summary>
    [Test]
    public void GetBranchHistoryAsync_Throws_WhenGitFails()
    {
        _gitMock
            .Setup(x => x.RunAsync(It.IsAny<string>()))
            .ThrowsAsync(new GitException("fatal error", 1));

        Assert.ThrowsAsync<GitException>(
            () => _service.GetBranchHistoryAsync("main"));
    }

    /// <summary>
    /// Проверка, что команда git log вызывается корректно.
    /// </summary>
    [Test]
    public async Task GetBranchHistoryAsync_CallsGitLog_WithCorrectArguments()
    {
        // arrange
        _gitMock
            .Setup(x => x.RunAsync(It.IsAny<string>()))
            .ReturnsAsync(new GitCommandResult(0, "", ""));

        // act
        await _service.GetBranchHistoryAsync("feature/test", 50);

        // assert
        _gitMock.Verify(
            x => x.RunAsync(It.Is<string>(args =>
                args == "log feature/test --max-count=50 --date=iso-strict --pretty=format:\"%H|%an|%ad|%s\""
            )),
            Times.Once);
    }

    /// <summary>
    /// Пустое имя ветки → ArgumentException.
    /// </summary>
    [Test]
    public void GetBranchHistoryAsync_Throws_WhenBranchNameEmpty()
    {
        Assert.ThrowsAsync<ArgumentException>(
            () => _service.GetBranchHistoryAsync(""));
    }
}
