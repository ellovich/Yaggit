using System.Diagnostics;
using System.Text;
using Models.Services.Yaggit;
using Models.Services.Yaggit.Contracts;
using static System.Net.Mime.MediaTypeNames;

public class GitCommandService : IGitCommandService
{
    private readonly ILogger<GitCommandService> _logger;
    private readonly IGitRepositoryContext _context;

    public event Action<ConsoleLine>? CommandExecuted;

    public bool IncludeOutput { get; set; } = true;
    public int CommandTimeoutMs { get; set; } = 15000;


    public GitCommandService(ILogger<GitCommandService> logger, IGitRepositoryContext context)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<GitCommandResult> RunAsync(string args)
    {
        if (string.IsNullOrWhiteSpace(_context.RepositoryPath))
            throw new InvalidOperationException("Путь к репозиторию не установлен.");
        if (string.IsNullOrWhiteSpace(args))
            throw new ArgumentException("Аргументы команды не могут быть пустыми.", nameof(args));

        CommandExecuted?.Invoke(new ConsoleLine($"> git {args}", eConsoleLineType.Command));
        // Принуждаем git вывести UTF-8 в любом окружении
        var finalArgs = $"-c i18n.logOutputEncoding=UTF-8 -c core.quotepath=false {args}";

        var startInfo = new ProcessStartInfo("git", finalArgs)
        //var startInfo = new ProcessStartInfo("git", args)
        {
            WorkingDirectory = _context.RepositoryPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,

              // Гарантированно читаем UTF-8
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        using var process = new Process { StartInfo = startInfo };

        try
        {
            process.Start();

            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            var result = new GitCommandResult(process.ExitCode, output, error);

            if (IncludeOutput)
            {
                if (!string.IsNullOrWhiteSpace(result.Output))
                    Emit(result.Output.Trim(), eConsoleLineType.Output);
                if (!string.IsNullOrWhiteSpace(result.Error))
                    Emit(result.Error.Trim(), eConsoleLineType.Error);
            }

            if (!result.IsSuccess)
                throw new GitException($"Ошибка выполнения 'git {args}': {result.Error}", result.ExitCode);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при выполнении git {Args}", args);
            CommandExecuted?.Invoke(new ConsoleLine($"[Ошибка] {ex.Message}", eConsoleLineType.Error));
            throw;
        }
    }


    /// <summary>
    /// Отправляет сообщение подписчикам события <see cref="CommandExecuted"/>,
    /// при этом ограничивает количество строк в логе.
    /// </summary>
    private void Emit(string text, eConsoleLineType type)
    {
        CommandExecuted?.Invoke(new ConsoleLine(text, type));
    }
}
