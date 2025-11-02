namespace Yaggit.Test.Core;

/// <summary>
/// Тесты для <see cref="GitRepositoryContext"/>.
/// </summary>
[TestFixture]
public class GitRepositoryContextTests
{
    private GitRepositoryContext _context = null!;

    [SetUp]
    public void Setup()
    {
        _context = new GitRepositoryContext(new NullLogger<GitRepositoryContext>());
    }

    /// <summary>
    /// Проверяет, что пустой путь вызывает исключение.
    /// </summary>
    [Test]
    public void SetRepository_Throws_WhenPathEmpty()
    {
        Assert.Throws<ArgumentException>(() => _context.SetRepository(""));
    }

    /// <summary>
    /// Проверяет, что несуществующий путь вызывает исключение.
    /// </summary>
    [Test]
    public void SetRepository_Throws_WhenPathNotExists()
    {
        Assert.Throws<DirectoryNotFoundException>(() => _context.SetRepository("Z:\\does_not_exist"));
    }

    /// <summary>
    /// Проверяет корректное сохранение пути к репозиторию.
    /// </summary>
    [Test]
    public void SetRepository_SetsPath()
    {
        var path = Environment.CurrentDirectory;
        _context.SetRepository(path);
        Assert.That(_context.RepositoryPath, Is.EqualTo(path));
    }

    /// <summary>
    /// Проверяет сброс контекста.
    /// </summary>
    [Test]
    public void ClearRepository_ResetsPath()
    {
        _context.SetRepository(Environment.CurrentDirectory);
        _context.ClearRepository();
        Assert.That(_context.RepositoryPath, Is.Empty);
    }

    /// <summary>
    /// Проверяет вызов события при изменении репозитория.
    /// </summary>
    [Test]
    public void RepositoryChanged_EventIsRaised()
    {
        string? lastPath = null;
        _context.RepositoryChanged += (p) => lastPath = p;

        var path = Environment.CurrentDirectory;
        _context.SetRepository(path);

        Assert.That(lastPath, Is.EqualTo(path));
    }
}
