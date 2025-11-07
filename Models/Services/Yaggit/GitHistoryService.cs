using Models.Services.Yaggit.Contracts;

namespace Models.Services.Yaggit;

public class GitHistoryService : IGitHistoryService
{
    private readonly ILogger<GitHistoryService> _logger;
    private readonly IGitCommandService _git;

    public GitHistoryService(ILogger<GitHistoryService> logger, IGitCommandService git)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _git = git ?? throw new ArgumentNullException(nameof(git));
    }

    public async Task<IReadOnlyList<GitCommit>> GetCommitHistoryAsync(string branchName, int maxCount = 100)
    {
        if (string.IsNullOrWhiteSpace(branchName))
            throw new ArgumentException("Имя ветки пустое", nameof(branchName));

        var args = $"log {branchName} --max-count={maxCount} --date=iso --pretty=format:%H|%an|%ad|%s";

        var result = await _git.RunAsync(args);

        if (!result.IsSuccess)
            throw new GitException($"Не удалось получить историю коммитов: {result.Error}", result.ExitCode);

        if (string.IsNullOrWhiteSpace(result.Output))
            return Array.Empty<GitCommit>();

        var commits = result.Output
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(ParseLine)
            .ToList();

        return commits;
    }

    private static GitCommit ParseLine(string line)
    {
        var parts = line.Split('|', 4);
        return new GitCommit(
            Hash: parts[0],
            Author: parts[1],
            Date: DateTime.Parse(parts[2]),
            Message: parts[3]
        );
    }
}
