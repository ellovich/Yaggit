namespace ViewModels.UI.Yaggit;

/// <summary>
/// Узел для древовидного отображения веток Git.
/// </summary>
public partial class VmBranchNode : VmBase
{
    /// <summary>Отображаемое имя узла (часть между '/').</summary>
    public string Name { get; }

    /// <summary>Полное имя ветки (например, feature/aaa) для листовых узлов.</summary>
    public string? FullName { get; protected set; }

    /// <summary>Является ли узел текущей (активной) веткой.</summary>
    [ObservableProperty]
    public partial bool IsCurrent { get; set; }

    [ObservableProperty]
    public partial bool IsPinned { get; set; }

    public bool IsBranch { get; protected set; }

    public bool IsPinnedPlaceholder { get; set; }

    public bool IsSeparator { get; set; }


    /// <summary>Дети узла.</summary>
    public ObservableCollection<VmBranchNode> Children { get; } = [];

    public VmBranchNode(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Пометить этот узел как лист с полным именем ветки и флагом current.
    /// </summary>
    public void MarkAsBranch(string fullName, bool isCurrent)
    {
        FullName = fullName;
        IsBranch = true;
        IsCurrent = isCurrent;
    }

    public VmBranchNode CloneShallow()
    {
        return new VmBranchNode(Name)
        {
            FullName = FullName,
            IsCurrent = IsCurrent,
            IsPinned = IsPinned,
            IsPinnedPlaceholder = true
        };
    }


    public override string ToString() => Name;
}

public sealed class VmBranchSeparator : VmBranchNode
{
    public VmBranchSeparator() : base("───────────────")
    {
        IsBranch = false;
        IsPinnedPlaceholder = true;
        IsSeparator = true;
    }
}