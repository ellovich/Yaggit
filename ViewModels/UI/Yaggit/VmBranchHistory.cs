using CommunityToolkit.Mvvm.Messaging;
using ViewModels.UI.Yaggit.Messages;

namespace ViewModels.UI.Yaggit;

public partial class VmBranchHistory : VmBase
{
    private readonly IGitBranchHistoryService _historyService;

    public ObservableCollection<GitCommit> Commits { get; protected set; } = [];

#pragma warning disable CS9264, CS8618
    public VmBranchHistory() { }
#pragma warning restore CS9264, CS8618

    public VmBranchHistory(IGitBranchHistoryService historyService)
    {
        _historyService = historyService ?? throw new ArgumentNullException(nameof(historyService));

        WeakReferenceMessenger.Default.Register<BranchSelectedMessage>(this, async (r, msg) =>
        {
            SelectedBranch = msg.Value;
            await LoadHistory();
        });
    }


    #region CMDS

    [RelayCommand]
    private async Task LoadHistory()
    {
        if (string.IsNullOrWhiteSpace(SelectedBranch))
            return;

        using (var loading = new LoadingScope(nameof(_historyService.GetBranchHistoryAsync)))
        {
            var commits = await _historyService.GetBranchHistoryAsync(SelectedBranch, 200);
            Commits.Init(commits);
        }
    }

    #endregion CMDS


    #region PROPS

    [ObservableProperty]
    public partial string? SelectedBranch { get; set; }

    [ObservableProperty]
    public partial GitCommit? SelectedCommit { get; set; }

    [ObservableProperty, NotifyPropertyChangedFor(nameof(FilteredCommits))]
    public partial string FilterText { get; set; } = string.Empty;

    public IEnumerable<GitCommit> FilteredCommits =>
        string.IsNullOrWhiteSpace(FilterText)
            ? Commits
            : Commits.Where(c =>
                (c.Author?.Contains(FilterText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (c.Message?.Contains(FilterText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (c.Hash?.Contains(FilterText, StringComparison.OrdinalIgnoreCase) ?? false));

    #endregion PROPS
}

public class MockVmBranchHistory : VmBranchHistory
{
    public MockVmBranchHistory()
    {
        Commits = [
            new GitCommit("30495023523045", "Daniil", DateTime.Now, "текст первого коммита"),
            new GitCommit("40492347523045", "Amina", DateTime.Now.AddDays(-99999), "тексто второго коммита"),
        ];
    }
}