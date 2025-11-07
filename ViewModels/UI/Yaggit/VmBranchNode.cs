namespace ViewModels.UI.Yaggit;

public enum eBranchNodeType
{
    Repository,
    LocalGroup,
    RemoteGroup,
    RemoteHost,
    Folder,    
    Branch
}

/// <summary>
/// Узел для древовидного отображения веток Git.
/// </summary>
public partial class VmBranchNode(string name, eBranchNodeType type) : VmBase
{
    /// <summary>
    /// Тип узла
    /// </summary>
    public eBranchNodeType NodeType { get; set; } = type;

    /// <summary>
    /// Отображаемое имя узла.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Полное имя ветки.
    /// </summary>
    public string FullName { get; protected set; } = string.Empty;

    /// <summary>
    /// Является ли узел активной веткой.
    /// </summary>
    [ObservableProperty]
    public partial bool IsCurrent { get; set; }

    /// <summary>
    /// Развернут ли узел.
    /// </summary>
    [ObservableProperty]
    public partial bool IsExpanded { get; set; }

    /// <summary>
    /// Закреплен ли узел.
    /// </summary>
    [ObservableProperty]
    public partial bool IsPinned { get; set; }

    public bool IsBranch { get; protected set; }

    public ObservableCollection<VmBranchNode> Children { get; } = [];

    /// <summary>
    /// Пометить этот узел как лист с полным именем ветки и флагом current.
    /// </summary>
    public void MarkAsBranch(string fullName, bool isCurrent)
    {
        FullName = fullName;
        IsBranch = true;
        IsCurrent = isCurrent;
    }

    public override string ToString() => Name;
}