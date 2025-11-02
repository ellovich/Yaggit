namespace Models.Services.Yaggit.Contracts;

/// <summary>
/// Сервис для выполнения Git-команд через системный CLI.
/// </summary>
public interface IGitCommandService
{
    /// <summary>
    /// Если false — вывод stdout/stderr не добавляется в CommandLog (только сами команды).
    /// </summary>
    bool IncludeOutput { get; set; }

    /// <summary>
    /// Событие, что команда выполнилась.
    /// </summary>
    public event Action<ConsoleLine>? CommandExecuted;

    /// <summary>
    /// Выполняет команду git в текущем репозитории.
    /// </summary>
    /// <param name="args">Аргументы команды Git.</param>
    /// <returns>Результат выполнения комманды Git.</returns>
    Task<GitCommandResult> RunAsync(string args);
}

public record ConsoleLine(string Text, eConsoleLineType Type);

public enum eConsoleLineType
{
    Command,
    Output,
    Error
}