namespace ViewModels.UI.Common;

public class VmNavMenuItem : VmMenuItem
{
    public static IServiceProvider ServiceProvider { get; set; } = null!;

    public Type? ViewModelType { get; }
    public bool IsCollapsed { get; init; }

    //public bool IsImplemented => ViewModelType is not null && Children.Count != 0;

    public VmNavMenuItem(Type page, string header, string iconName = "", string description = "")
        : base(header, iconName, description)
    {
        ViewModelType = page;
        SubMenuItems = [];
    }

    public VmNavMenuItem(string header, string iconName = "", string description = "",
        IEnumerable<VmNavMenuItem>? children = null, bool isCollapsed = false)
        : base(header, iconName, description)
    {
        IsCollapsed = isCollapsed;

        SubMenuItems = [];
        if (children != null)
            foreach (VmNavMenuItem child in children)
                SubMenuItems?.Add(child);
    }
}