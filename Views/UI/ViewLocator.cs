using Avalonia.Metadata;
using ViewModels.UI.Yaggit;
using HanumanInstitute.MvvmDialogs.Avalonia;

// not to write prefix for custom controls
[assembly: XmlnsDefinition("https://github.com/avaloniaui", "CoreUI.Controls")]
[assembly: XmlnsDefinition("https://github.com/avaloniaui", "Views.Controls")]

namespace Views.UI;

public class ViewLocator : StrongViewLocator
{
    public ViewLocator()
    {
        ForceSinglePageNavigation = false;

        RegisterCommon();

        RegisterYaggit();
    }

    private void RegisterCommon()
    {
        // Общие для всех проектов view
        Register<ViewModels.Common.VmMessageBox, Common.MessageBox>();
        Register<ViewModels.Common.VmMessageBoxYesNo, Common.MessageBoxYesNo>();
    }

    private void RegisterYaggit()
    {
        Register<VmYaggitMain, Yaggit.YaggitMainView, Yaggit.YaggitMainWindow>();
        Register<VmBranches, Yaggit.Branches>();
        Register<VmRepoSelector, Yaggit.RepoSelector>();
        Register<VmConsole, Yaggit.Console>();
    }
}