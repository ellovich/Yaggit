namespace Models.Services.Yaggit.Contracts;

/// <summary>
/// Исключение при ошибках выполнения git.
/// </summary>
public class GitException : Exception
{
    public int ExitCode { get; }

    public GitException(string message, int exitCode = -1, Exception? inner = null)
        : base(message, inner)
    {
        ExitCode = exitCode;
    }
}
