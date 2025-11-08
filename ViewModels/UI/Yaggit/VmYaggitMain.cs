namespace ViewModels.UI.Yaggit;

public partial class VmYaggitMain : VmBaseMainWindow
{
    private readonly ILogger<VmYaggitMain> _logger; 

    public VmBranches Branches { get; protected set; }
    public VmRepoSelector RepoSelector { get; protected set; }
    public VmConsole Console { get; protected set; }
    public VmBranchHistory BranchHistory { get; protected set; }

#pragma warning disable CS9264, CS8618
    public VmYaggitMain() : base(null!) { }
#pragma warning restore CS9264, CS8618

    public VmYaggitMain(
        ILogger<VmYaggitMain> logger,
        IDialogService dialogService,
        VmBranches branches,
        VmRepoSelector repoSelector,
        VmConsole console,
        VmBranchHistory commitHistory
    ) : base(dialogService)
    {
        _logger = logger;
        Branches = branches;
        RepoSelector = repoSelector;
        Console = console;
        BranchHistory = commitHistory;
    }

    #region COMMANDS

    #endregion COMMANDS
}

public class MockVmYaggitMain : VmYaggitMain
{
    public MockVmYaggitMain()
    {
        Branches = new MockVmBranches();
        RepoSelector = new MockVmRepoSelector();
        Console = new MockVmConsole();
        BranchHistory = new MockVmBranchHistory();
    }
}