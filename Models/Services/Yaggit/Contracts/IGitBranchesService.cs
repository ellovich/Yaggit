namespace Models.Services.Yaggit.Contracts;

/// <summary>
/// Предоставляет методы для работы с ветками Git-репозитория.
/// </summary>
public interface IGitBranchesService
{
    /// <summary>
    /// Возвращает список веток в указанном репозитории.
    /// </summary>
    /// <returns>Список веток.</returns>
    Task<IReadOnlyList<GitBranch>> GetBranchesAsync();

    /// <summary>
    /// Переключается на указанную ветку.
    /// </summary>
    /// <param name="branchName">Имя ветки.</param>
    Task CheckoutBranchAsync(string branchName);

    /// <summary>
    /// Удаляет ветку из репозитория.
    /// </summary>
    /// <param name="branchName">Имя ветки для удаления.</param>
    /// <param name="force">Принудительное удаление (флаг -D).</param>
    Task DeleteBranchAsync(string branchName, bool force = false);

    /// <summary>
    /// Переименовывает ветку.
    /// </summary>
    /// <param name="oldName">Старое имя ветки.</param>
    /// <param name="newName">Новое имя ветки.</param>
    Task RenameBranchAsync(string oldName, string newName);
}