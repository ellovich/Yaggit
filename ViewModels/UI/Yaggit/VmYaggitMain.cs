namespace ViewModels.UI.Yaggit;

public partial class VmYaggitMain : VmBaseMainWindow
{
    private readonly ILogger<VmYaggitMain> _logger; 

    public VmBranches Branches { get; }
    public VmRepoSelector RepoSelector { get; }
    public VmConsole Console { get; }

#pragma warning disable CS9264, CS8618
    public VmYaggitMain() : base(null!) { }
#pragma warning restore CS9264, CS8618

    public VmYaggitMain(
        ILogger<VmYaggitMain> logger,
        IDialogService dialogService,
        VmBranches branches,
        VmRepoSelector repoSelector,
        VmConsole console
    ) : base(dialogService)
    {
        _logger = logger;
        Branches = branches;
        RepoSelector = repoSelector;
        Console = console;

        RepoSelector.VmMainWindow = this;
    }

    #region COMMANDS

    #endregion COMMANDS
}