namespace Models.Services.Yaggit.Contracts;

public interface IGitHistoryService
{
    /// <summary>
    /// Возвращает историю коммитов для выбранной ветки.
    /// </summary>
    Task<IReadOnlyList<GitCommit>> GetCommitHistoryAsync(string branchName, int maxCount = 100);
}