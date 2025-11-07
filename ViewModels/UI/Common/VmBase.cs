using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.VisualTree;

namespace ViewModels.UI.Common;

public abstract class VmBase : ObservableObject
{
    private static int _pageId = 0;
    public int PageId { get; set; }

    private static readonly Lazy<VmBase> _topLevelLazy = new(GetTopLevelInternal);
    protected static VmBase MainVm => _topLevelLazy.Value;

    public static ObservableCollection<ActiveLoading> ActiveLoadings { get; } = [];

    protected VmBase()
    {
        PageId = _pageId++;
    }

    public static bool IsDebug
    {
        get
        {
#pragma warning disable CS0162
#if DEBUG
            return true;
#endif
            return false;
#pragma warning restore CS0162
        }
    }

    private static VmBase GetTopLevelInternal()
    {
        if (Application.Current?.ApplicationLifetime is not { } lifetime)
            throw new InvalidOperationException("Unable to access ApplicationLifetime — application not initialized.");

        VmBase? mainVm = lifetime switch
        {
            IClassicDesktopStyleApplicationLifetime { MainWindow.DataContext: VmBase vm } => vm,
            ISingleViewApplicationLifetime { MainView: { } view }
                => (view.GetVisualRoot() as TopLevel)?.DataContext as VmBase,
            _ => null
        };

        return mainVm ?? throw new InvalidOperationException("Unable to resolve the main ViewModel (VmBase).");
    }
}