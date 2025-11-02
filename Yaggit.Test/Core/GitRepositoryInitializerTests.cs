namespace Yaggit.Test.Core;

/// <summary>
/// Тесты для <see cref="GitRepositoryInitializer"/>.
/// </summary>
[TestFixture]
public class GitRepositoryInitializerTests
{
    private Mock<IGitCommandService> _gitMock = null!;
    private Mock<IGitRepositoryContext> _contextMock = null!;
    private GitRepositoryInitializer _service = null!;
    private string _tempDir = null!;

    [SetUp]
    public void Setup()
    {
        _gitMock = new Mock<IGitCommandService>();
        _contextMock = new Mock<IGitRepositoryContext>();
        _service = new GitRepositoryInitializer(new NullLogger<GitRepositoryInitializer>(), _gitMock.Object, _contextMock.Object);
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    }

    [TearDown]
    public void Cleanup()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    /// <summary>
    /// Проверяет, что несуществующий путь не считается инициализированным.
    /// </summary>
    [Test]
    public void IsRepositoryInitialized_ReturnsFalse_WhenNotExists()
    {
        Assert.That(_service.IsRepositoryInitialized(_tempDir), Is.False);
    }

    /// <summary>
    /// Проверяет, что наличие .git означает инициализацию репозитория.
    /// </summary>
    [Test]
    public void IsRepositoryInitialized_ReturnsTrue_WhenGitFolderExists()
    {
        Directory.CreateDirectory(Path.Combine(_tempDir, ".git"));
        Assert.That(_service.IsRepositoryInitialized(_tempDir), Is.True);
    }

    /// <summary>
    /// Проверяет, что повторная инициализация не вызывает git init.
    /// </summary>
    [Test]
    public async Task InitializeRepositoryAsync_SkipsIfAlreadyInitialized()
    {
        Directory.CreateDirectory(Path.Combine(_tempDir, ".git"));
        await _service.InitializeRepositoryAsync(_tempDir);

        _gitMock.Verify(x => x.RunAsync(It.IsAny<string>()), Times.Never);
    }

    /// <summary>
    /// Проверяет, что пустой путь вызывает исключение.
    /// </summary>
    [Test]
    public void InitializeRepositoryAsync_Throws_WhenPathEmpty()
    {
        Assert.ThrowsAsync<ArgumentException>(() => _service.InitializeRepositoryAsync(""));
    }

    /// <summary>
    /// Проверяет выброс исключения при сбое выполнения git init.
    /// </summary>
    [Test]
    public void InitializeRepositoryAsync_Throws_WhenGitFails()
    {
        // Arrange: создаём директорию, чтобы метод не упал с DirectoryNotFoundException
        Directory.CreateDirectory(_tempDir);

        _gitMock
            .Setup(x => x.RunAsync("init"))
            .ThrowsAsync(new GitException("Ошибка git", 1));

        // Act & Assert: ожидание GitException, который пробрасывает сервис
        Assert.ThrowsAsync<GitException>(() => _service.InitializeRepositoryAsync(_tempDir));
    }
}
