using CommunityToolkit.Mvvm.Messaging;
using ViewModels.UI.Yaggit.Messages;

namespace ViewModels.UI.Yaggit;

public partial class VmBranches : VmBase
{
    private readonly ILogger<VmBranches> _logger;
    private readonly IGitBranchesService _gitBranchesService;

#pragma warning disable CS9264, CS8618
    public VmBranches() { }
#pragma warning restore CS9264, CS8618

    public VmBranches(
        ILogger<VmBranches> logger,
        IGitBranchesService gitBranchesService
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _gitBranchesService = gitBranchesService ?? throw new ArgumentNullException(nameof(gitBranchesService));
    }


    #region PROPS

    /// <summary>
    /// Дерево веток (иерархия).
    /// </summary>
    public ObservableCollection<VmBranchNode> BranchTree { get; } = [];

    /// <summary>Избранные (закреплённые) ветки.</summary>
    public ObservableCollection<VmBranchNode> PinnedBranches { get; } = [];

    /// <summary>
    /// Выбранный узел.
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CheckoutBranchCommand))]
    public partial VmBranchNode? SelectedBranchNode { get; set; }

    partial void OnSelectedBranchNodeChanged(VmBranchNode? value)
    {
        if (value?.FullName is null)
            return;

        _logger.LogInformation("Selected branch: {branch}", value);
        WeakReferenceMessenger.Default.Send(new BranchSelectedMessage(value.FullName));
    }

    [ObservableProperty]
    public partial GitBranch? CurrentBranch { get; set; }

    #endregion PROPS


    #region CMDS

    /// <summary>
    /// Команда загрузки веток из текущего репозитория.
    /// </summary>
    [RelayCommand]
    private async Task GetBranches()
    {
        try
        {
            _logger.LogInformation("Получение списка веток...");

            var branches = await _gitBranchesService.GetBranchesAsync();

            if (branches is null || branches.Count == 0)
            {
                _logger.LogWarning("В репозитории отсутствуют ветки. Возможно, он только что инициализирован.");

                BranchTree.Clear();
                CurrentBranch = null;

                // Можно уведомить пользователя
                // await _dialogService.ShowInfoAsync(VmMainWindow, "Нет веток", "Репозиторий пока не содержит веток.");

                return;
            }

            _logger.LogInformation("Загружено {Count} веток.", branches.Count);

            // Безопасно находим текущую ветку (если есть)
            CurrentBranch = branches.FirstOrDefault(x => x.IsCurrent);

            if (CurrentBranch == null)
                _logger.LogWarning("Текущая ветка не определена (ни одна не отмечена как активная).");

            _logger.LogInformation("Построение древа веток ({Count} веток).", branches.Count);

            // --------------------------
            // сохраняем expand-state + выделение
            // --------------------------
            var expandState = BranchTree.SaveExpandState();
            var selectedKey = SelectedBranchNode?.FullName;
            var currentKey = CurrentBranch?.Name;

            // перестраиваем
            BranchTree.Init(BuildBranchTree("Yaggit", branches));

            // --------------------------
            // восстанавливаем expand-state
            // --------------------------
            BranchTree.RestoreExpandState(expandState);

            // --------------------------
            // авто-раскрытие пути к текущей ветке
            // --------------------------
            BranchTree.ExpandPathTo(currentKey);

            // --------------------------
            // восстановление выделения
            // --------------------------
            if (selectedKey != null)
                SelectedBranchNode = BranchTree.FindNode(selectedKey) ?? SelectedBranchNode;

            // --------------------------
            // авто-scroll к текущей ветке (через ScrollBehavior)
            // --------------------------

            // Восстанавливаем IsPinned для веток, которые были закреплены
            foreach (var pinned in PinnedBranches)
            {
                var found = FindNodeByFullName(BranchTree, pinned.FullName);
                if (found != null)
                {
                    found.IsPinned = true;
                    pinned.IsCurrent = found.IsCurrent; // обновляем статус
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении веток.");
            // можно вызвать диалог:
            // await _dialogService.ShowErrorAsync(VmMainWindow, "Ошибка", ex.Message);
        }
    }

    [RelayCommand]
    private void TogglePin(VmBranchNode? node)
    {
        if (node?.FullName == null)
            return;

        if (node.IsPinned)
        {
            node.IsPinned = false;
            var pinned = PinnedBranches.FirstOrDefault(x => x.FullName == node.FullName);
            if (pinned != null)
                PinnedBranches.Remove(pinned);
            _logger.LogInformation("Откреплена ветка {Branch}", node.FullName);
        }
        else
        {
            node.IsPinned = true;
            if (!PinnedBranches.Any(x => x.FullName == node.FullName))
                PinnedBranches.Add(node);
            _logger.LogInformation("Закреплена ветка {Branch}", node.FullName);
        }
    }

    private static VmBranchNode? FindNodeByFullName(IEnumerable<VmBranchNode> nodes, string? fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return null;

        foreach (var n in nodes)
        {
            if (n.FullName == fullName)
                return n;

            var child = FindNodeByFullName(n.Children, fullName);
            if (child != null)
                return child;
        }
        return null;
    }

    /// <summary>
    /// Команда переключения на выбранную ветку.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanCheckoutBranch))]
    private async Task CheckoutBranch()
    {
        if (SelectedBranchNode is null)
            return;

        try
        {
            _logger.LogInformation("Переключение на ветку {Branch}", SelectedBranchNode.FullName);
            await _gitBranchesService.CheckoutBranchAsync(SelectedBranchNode.FullName);
            await GetBranches(); // обновить список, чтобы звёздочка обновилась
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при переключении ветки.");
            // вызвать диалог
        }
    }

    private bool CanCheckoutBranch() => 
        SelectedBranchNode is not null 
        && !SelectedBranchNode.IsCurrent 
        && SelectedBranchNode.IsBranch;

    protected static List<VmBranchNode> BuildBranchTree(string repositoryName,
                                                    IEnumerable<GitBranch> branches)
    {
        var root = new VmBranchNode(repositoryName, eBranchNodeType.Repository);

        var locals = new VmBranchNode("Local", eBranchNodeType.LocalGroup);
        var remotes = new VmBranchNode("Remotes", eBranchNodeType.RemoteGroup);

        root.Children.Add(locals);
        root.Children.Add(remotes);

        // Группируем remote по remote-name
        var remoteGroups = new Dictionary<string, VmBranchNode>();

        foreach (var br in branches)
        {
            if (br.Name.StartsWith("remotes/"))
            {
                // remotes/origin/master → origin, master
                var parts = br.Name.Split('/', 3);
                var remoteName = parts[1];
                var branchPath = parts[2];

                if (!remoteGroups.TryGetValue(remoteName, out var hostNode))
                {
                    hostNode = new VmBranchNode(remoteName, eBranchNodeType.RemoteHost);
                    remoteGroups[remoteName] = hostNode;
                    remotes.Children.Add(hostNode);
                }

                InsertBranchNode(hostNode, branchPath, br);
            }
            else
            {
                // local branches
                InsertBranchNode(locals, br.Name, br);
            }
        }

        AssignFolderTypes(root.Children);

        return [root];
    }

    // ----------------------------
    // Recursive tree builder (like your original one)
    // ----------------------------
    private static void InsertBranchNode(VmBranchNode parent, string branchPath, GitBranch br)
    {
        var parts = branchPath.Split('/');
        var current = parent;
        var prefix = new List<string>();

        foreach (var part in parts)
        {
            prefix.Add(part);

            var next = current.Children.FirstOrDefault(x => x.Name == part);
            if (next == null)
            {
                next = new VmBranchNode(part, eBranchNodeType.Branch);
                current.Children.Add(next);
            }
            current = next;
        }

        current.MarkAsBranch(string.Join('/', prefix), br.IsCurrent);
    }

    static void AssignFolderTypes(IEnumerable<VmBranchNode> nodes)
    {
        foreach (var n in nodes)
        {
            if (!n.IsBranch && n.Children.Count > 0)
                n.NodeType = eBranchNodeType.Folder;

            AssignFolderTypes(n.Children);
        }
    }

    private static void SortNodesRecursive(IList<VmBranchNode> nodes)
    {
        if (nodes.Count == 0)
            return;

        // Сначала сортируем текущий уровень
        var sorted = nodes.OrderBy(n => n.Name, StringComparer.OrdinalIgnoreCase).ToList();

        nodes.Clear();
        foreach (var node in sorted)
            nodes.Add(node);

        // Затем сортируем всех потомков
        foreach (var node in nodes)
            SortNodesRecursive(node.Children);
    }

    #endregion CMDS
}

public class MockVmBranches : VmBranches
{
    public MockVmBranches()
    {
        var branches = new List<GitBranch>()
        {
            new("master", false),
            new("develop", false),
            new("feature/h1", false),
            new("feature/h2", false),
            new("feature/h3", true),
            new("hotfix/hf1", false),
            new("hotfix/hf2", false),
            new("hotfix/hf3", false),
            new("hotfix/hf4", false),
        };
        BranchTree.Init(BuildBranchTree("DemoRepo", branches));
    }
}