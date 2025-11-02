using Avalonia.Threading;

namespace ViewModels.UI.Yaggit;


public partial class VmConsole : VmBase
{
    private readonly IGitCommandService _git;
    private const int MaxEntries = 200;

    public ObservableCollection<ConsoleLine> CommandLog { get; protected set; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ToggleOutputButtonText))]
    public partial bool ShowCommandOutput { get; set; } = true;

    public string ToggleOutputButtonText => ShowCommandOutput ? Lang.UI.Button_HideOutput : Lang.UI.Button_ShowOutput;

#pragma warning disable CS9264, CS8618
    public VmConsole() { }
#pragma warning restore CS9264, CS8618

    public VmConsole(IGitCommandService git)
    {
        _git = git ?? throw new ArgumentNullException(nameof(git));
        _git.CommandExecuted += OnCommandExecuted;
    }

    private void OnCommandExecuted(ConsoleLine line)
    {
        Dispatcher.UIThread.Post(() =>
        {
            CommandLog.Add(line);

            // ограничиваем размер лога
            if (CommandLog.Count > MaxEntries)
                CommandLog.RemoveAt(0);
        });
    }

    [RelayCommand]
    private void ToggleOutput()
    {
        ShowCommandOutput = !ShowCommandOutput;
        _git.IncludeOutput = ShowCommandOutput;
    }

    [RelayCommand]
    private void Clear()
    {
        CommandLog.Clear();
    }
}

public class MockVmConsole : VmConsole
{
    public MockVmConsole()
    {
        CommandLog =
        [
            new("git init", eConsoleLineType.Command),
            new("git checkout develop", eConsoleLineType.Command),
            new("git add .", eConsoleLineType.Command),
            new("git status", eConsoleLineType.Command),
            new("git commit -m \"Init commit\"", eConsoleLineType.Command),
        ];
    }
}