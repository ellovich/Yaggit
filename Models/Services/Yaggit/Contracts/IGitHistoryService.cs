namespace Models.Services.Yaggit.Contracts;

public interface IGitBranchHistoryService
{
    /// <summary>
    /// Возвращает историю коммитов для выбранной ветки.
    /// </summary>
    Task<IReadOnlyList<GitCommit>> GetBranchHistoryAsync(string branchName, int maxCount = 100);
}