using Models.Services.Yaggit.Contracts;

namespace Models.Services.Yaggit;

/// <summary>
/// Реализация сервиса для создания новых git-репозиториев.
/// </summary>
public class GitRepositoryInitializer : IGitRepositoryInitializer
{
    private readonly ILogger<GitRepositoryInitializer> _logger;
    private readonly IGitCommandService _git;
    private readonly IGitRepositoryContext _context;

    public GitRepositoryInitializer(
        ILogger<GitRepositoryInitializer> logger,
        IGitCommandService git,
        IGitRepositoryContext context)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _git = git ?? throw new ArgumentNullException(nameof(git));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task InitializeRepositoryAsync(string path, bool bare = false)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Путь к репозиторию не может быть пустым.", nameof(path));

        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException($"Каталог не найден: {path}");

        try
        {
            if (IsRepositoryInitialized(path))
            {
                _logger.LogWarning("Репозиторий уже инициализирован: {Path}", path);
                return;
            }

            _context.SetRepository(path);
            var result = await _git.RunAsync(bare ? "init --bare" : "init");

            _logger.LogInformation("Репозиторий успешно инициализирован: {Path}", path);
        }
        catch (GitException ex)
        {
            _logger.LogError(ex, "Ошибка git при инициализации репозитория {Path}", path);
            throw; // пробрасываем дальше — UI решит, как реагировать
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при инициализации репозитория {Path}", path);
            throw new GitException($"Не удалось инициализировать репозиторий: {path}", inner: ex);
        }
    }

    public bool IsRepositoryInitialized(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            return false;

        var gitDir = Path.Combine(path, ".git");
        return Directory.Exists(gitDir);
    }
}