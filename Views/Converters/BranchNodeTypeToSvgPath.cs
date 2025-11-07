using ViewModels.UI.Yaggit;

namespace Views.Converters;

public sealed class BranchNodeTypeToSvgPath : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is eBranchNodeType type
            ? type switch
            {
                eBranchNodeType.Repository => Icons.gitLogo,
                eBranchNodeType.LocalGroup => Icons.Vs22.Computer,
                eBranchNodeType.RemoteGroup => Icons.Vs22.RemoteFolder,
                eBranchNodeType.RemoteHost => Icons.exit,
                eBranchNodeType.Folder => Icons.Vs22.FolderClosed,
                eBranchNodeType.Branch => Icons.Vs22.Git.Branch,
                _ => Icons.bug
            }
            : Icons.bug;


    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => BindingOperations.DoNothing;
}