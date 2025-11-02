namespace Models.Services.Yaggit.Contracts;

/// <summary>
/// Контекст текущего репозитория Git.
/// Хранит активный путь и управляет его изменением.
/// </summary>
public interface IGitRepositoryContext
{
    /// <summary>
    /// Путь к корневому каталогу Git-репозитория.
    /// </summary>
    string RepositoryPath { get; }

    /// <summary>
    /// Устанавливает активный репозиторий.
    /// </summary>
    void SetRepository(string repoPath);

    /// <summary>
    /// Сбрасывает активный репозиторий.
    /// </summary>
    void ClearRepository();
}