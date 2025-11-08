namespace Models.Services.Yaggit.Contracts;

/// <summary>
/// Содержит шаблоны команд Git.
/// </summary>
public static class GitCommands
{
    public const string InitGitRepository = "init";

    /// <summary>
    /// Получает список локальных веток.
    /// </summary>
    public const string ListBranches = "branch --list";

    /// <summary>
    /// Формирует команду для переключения на ветку.
    /// </summary>
    public static string ChangeBranch(string branchName) => $"checkout {branchName}";

    /// <summary>
    /// Формирует команду для удаления ветки.
    /// </summary>
    public static string DeleteBranch(string branchName, bool force = false) =>
        $"branch {(force ? "-D" : "-d")} {branchName}";

    /// <summary>
    /// Формирует команду для переименования ветки.
    /// </summary>
    public static string RenameBranch(string oldName, string newName) =>
        $"branch -m {oldName} {newName}";

    public static string BranchHistory(string branchName, int maxCount) =>
        $"log {branchName} --max-count={maxCount} --date=iso-strict --pretty=format:\"%H|%an|%ad|%s\"";
}
