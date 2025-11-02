using Microsoft.Extensions.Logging;
using Models.Services.Yaggit.Contracts;

namespace Models.Services.Yaggit;

public class GitRepositoryContext : IGitRepositoryContext
{
    private readonly ILogger<GitRepositoryContext> _logger;
    private readonly Lock _lock = new();

    public string RepositoryPath { get; private set; } = string.Empty;
    public event Action<string?>? RepositoryChanged;

    public GitRepositoryContext(ILogger<GitRepositoryContext> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void SetRepository(string repoPath)
    {
        if (string.IsNullOrWhiteSpace(repoPath))
            throw new ArgumentException("Путь к репозиторию не может быть пустым.", nameof(repoPath));

        if (!Directory.Exists(repoPath))
            throw new DirectoryNotFoundException($"Каталог не найден: {repoPath}");

        lock (_lock)
        {
            RepositoryPath = repoPath;
        }

        RepositoryChanged?.Invoke(repoPath);

        _logger.LogInformation("Активный репозиторий установлен: {RepoPath}", repoPath);
    }

    public void ClearRepository()
    {
        lock (_lock)
        {
            RepositoryPath = string.Empty;
        }

        _logger.LogInformation("Контекст репозитория сброшен");
    }
}
