namespace Models.Services.Yaggit.Contracts;

/// <summary>
/// Сервис для инициализации и настройки git-репозиториев.
/// </summary>
public interface IGitRepositoryInitializer
{
    /// <summary>
    /// Инициализирует новый git-репозиторий в указанной директории.
    /// </summary>
    /// <param name="path">Путь к каталогу.</param>
    /// <param name="bare">Создать bare-репозиторий.</param>
    Task InitializeRepositoryAsync(string path, bool bare = false);

    /// <summary>
    /// Проверяет, инициализирован ли репозиторий (наличие .git).
    /// </summary>
    bool IsRepositoryInitialized(string path);
}
