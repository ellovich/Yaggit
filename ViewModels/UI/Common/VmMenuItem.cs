using System.Windows.Input;

namespace ViewModels.UI.Common;

public class VmMenuItem : ObservableObject
{
    public string? Header { get; init; }
    public string? Description { get; init; }
    public string? IconName { get; init; }
    public bool IsEnabled { get; set; }
    public ICommand? Command { get; set; }
    public bool IsSeparator { get; set; }
    public bool IsVisible { get; set; }
    public object? CommandParameter { get; set; }

    public ObservableCollection<VmMenuItem>? SubMenuItems { get; protected set; }

    public VmMenuItem(
        string header,
        string iconName = "",
        string description = "",
        ICommand? command = null,
        object? commandParameter = null,
        bool isVisible = false,
        bool isEnabled = true,
        IEnumerable<VmMenuItem>? subMenuItems = null
        )
    {
        Header = header;
        Description = description;
        IconName = iconName;

        Command = command;
        CommandParameter = commandParameter;
        IsVisible = isVisible;
        IsEnabled = isEnabled && Command != null;

        SubMenuItems = subMenuItems == null
            ? []
            : [.. subMenuItems];
    }

    public VmMenuItem()
    {
        IsSeparator = true;
    }

    public VmMenuItem AddMenuItems(IEnumerable<VmMenuItem> menuItems)
    {
        SubMenuItems ??= [];
        SubMenuItems.AddRange(menuItems);

        return this;
    }
}