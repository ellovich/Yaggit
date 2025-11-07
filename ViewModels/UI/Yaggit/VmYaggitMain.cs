namespace ViewModels.UI.Yaggit;

public partial class VmYaggitMain : VmBaseMainWindow
{
    private readonly ILogger<VmYaggitMain> _logger; 

    public VmBranches Branches { get; }
    public VmRepoSelector RepoSelector { get; }
    public VmConsole Console { get; }
    public VmCommitHistory CommitHistory { get; }

#pragma warning disable CS9264, CS8618
    public VmYaggitMain() : base(null!) { }
#pragma warning restore CS9264, CS8618

    public VmYaggitMain(
        ILogger<VmYaggitMain> logger,
        IDialogService dialogService,
        VmBranches branches,
        VmRepoSelector repoSelector,
        VmConsole console,
        VmCommitHistory commitHistory
    ) : base(dialogService)
    {
        _logger = logger;
        Branches = branches;
        RepoSelector = repoSelector;
        Console = console;
        CommitHistory = commitHistory;
    }

    #region COMMANDS

    #endregion COMMANDS
}