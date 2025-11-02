namespace Models.Services.Yaggit;

/// <summary>
/// Результат выполнения Git-команды.
/// </summary>
public class GitCommandResult(int exitCode, string output, string error)
{
    public int ExitCode { get; } = exitCode;
    public string Output { get; } = output.Trim();
    public string Error { get; } = error.Trim();
    public bool IsSuccess => ExitCode == 0;
}
