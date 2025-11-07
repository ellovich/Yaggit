using Models.Services.Yaggit.Contracts;

namespace Models.Services.Yaggit;

public class GitBranchesService : IGitBranchesService
{
    private readonly ILogger<GitBranchesService> _logger;
    private readonly IGitCommandService _git;

    public GitBranchesService(ILogger<GitBranchesService> logger, IGitCommandService git)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _git = git ?? throw new ArgumentNullException(nameof(git));
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<GitBranch>> GetBranchesAsync()
    {
        _logger.LogInformation("Получение списка веток");

        var result = await _git.RunAsync("branch --list");

        if (!result.IsSuccess)
            throw new GitException($"Ошибка при получении списка веток: {result.Error}", result.ExitCode);

        // Если репозиторий пуст (например, сразу после git init)
        if (string.IsNullOrWhiteSpace(result.Output))
        {
            _logger.LogWarning("В репозитории отсутствуют ветки. Возможно, он только что инициализирован.");
            return Array.Empty<GitBranch>();
        }

        var lines = result.Output
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var branches = lines.Select(line =>
        {
            var isCurrent = line.StartsWith('*');
            var name = line.TrimStart('*', ' ').Trim();
            return new GitBranch(name, isCurrent);
        }).ToList();

        _logger.LogDebug("Найдено {Count} веток", branches.Count);
        return branches;
    }

    public async Task CheckoutBranchAsync(string branchName)
    {
        ValidateBranchName(branchName);
        _logger.LogInformation("Переключение на ветку {Branch}", branchName);

        await _git.RunAsync($"checkout {branchName}");
    }

    public async Task DeleteBranchAsync(string branchName, bool force = false)
    {
        ValidateBranchName(branchName);
        var flag = force ? "-D" : "-d";
        _logger.LogWarning("Удаление ветки {Branch}, force={Force}", branchName, force);

        await _git.RunAsync($"branch {flag} {branchName}");
    }

    public async Task RenameBranchAsync(string oldName, string newName)
    {
        ValidateBranchName(oldName);
        ValidateBranchName(newName);
        _logger.LogInformation("Переименование ветки {Old} → {New}", oldName, newName);

        await _git.RunAsync($"branch -m {oldName} {newName}");
    }

    private static void ValidateBranchName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Имя ветки не может быть пустым.", nameof(name));
    }
}