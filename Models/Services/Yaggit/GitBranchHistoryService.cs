using Models.Services.Yaggit.Contracts;

namespace Models.Services.Yaggit;

public class GitBranchHistoryService : IGitBranchHistoryService
{
    private readonly ILogger<GitBranchHistoryService> _logger;
    private readonly IGitCommandService _git;

    public GitBranchHistoryService(ILogger<GitBranchHistoryService> logger, IGitCommandService git)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _git = git ?? throw new ArgumentNullException(nameof(git));
    }

    public async Task<IReadOnlyList<GitCommit>> GetBranchHistoryAsync(string branchName, int maxCount = 100)
    {
        if (string.IsNullOrWhiteSpace(branchName))
            throw new ArgumentException("Имя ветки пустое", nameof(branchName));

        string args = GitCommands.BranchHistory(branchName, maxCount);
        var result = await _git.RunAsync(args);

        if (!result.IsSuccess)
            throw new GitException($"Не удалось получить историю коммитов: {result.Error}", result.ExitCode);

        if (string.IsNullOrWhiteSpace(result.Output))
            return [];

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
