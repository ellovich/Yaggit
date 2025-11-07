using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ViewModels;

public partial class VmBaseMainWindow : VmBase
{
    private readonly IDialogService _dialogService;

    public List<VmMenuItem> MenuItems { get; private set; } = [];
    public List<VmMenuItem> DefaultMenuItems { get; private set; } = [];

    public VmBaseMainWindow(IDialogService dialogService)
    {
        _dialogService = dialogService;

        DefaultMenuItems = [
            new VmMenuItem("О программе", iconName: Icons.info, command: ShowAboutCommand, isVisible: !string.IsNullOrEmpty(ProgramDescription)),
            new VmMenuItem(header: "Сообщить о баге", iconName: Icons.mail, command: ReportBugCommand),
            new VmMenuItem(header: $"Версия: {Version}", iconName: Icons.version, isEnabled: false),
            new VmMenuItem(),
            new VmMenuItem(header: "Выйти", iconName : Icons.exit, command: ExitCommand)
        ];
    }

    #region MENU

    public string Version { get; set; } = Assembly.GetEntryAssembly()!.GetName().Version!.ToString();

    public string? ProgramDescription { get; set; } = Assembly.GetEntryAssembly()!
        .GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description.ToString();

    [RelayCommand]
    public async Task ShowAbout()
    {
        await _dialogService.ShowInfoAsync(this, "О программе", ProgramDescription!, 300, 200);
    }

    [RelayCommand]
    public async Task ReportBug()
    {
        await Task.Delay(500);
    }

    [RelayCommand]
    public async Task Exit(Func<Task>? beforeExit = null)
    {
        var res = await _dialogService.ShowConfirmDialog(
            this,
            $"Выход из приложения",
            $"Желаете выйти?"
        );
        if (res == null || !Convert.ToBoolean(res))
            return;

        if (beforeExit != null)
            await beforeExit();

        Environment.Exit(0);
    }

    #endregion MENU
}