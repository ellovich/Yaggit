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
    public void ShowAbout()
    {
        _dialogService.ShowInfoAsync(this, "О программе", ProgramDescription!, 300, 200);
    }

    [RelayCommand]
    public void ReportBug()
    {
        string mailClientPath = string.Empty;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            string[] candidates =
            {
                @"C:\Program Files\Microsoft Office\root\Office16\OUTLOOK.EXE",
                @"C:\Program Files (x86)\RuPost Desktop\rupost-desktop.exe",
                @"C:\Program Files\Mozilla Thunderbird\thunderbird.exe"
            };
            foreach (var path in candidates)
            {
                if (File.Exists(path))
                {
                    mailClientPath = path;
                    break;
                }
            }
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            string[] candidates = { "/usr/bin/thunderbird", "/usr/bin/rupost" };
            foreach (var path in candidates)
            {
                if (File.Exists(path))
                {
                    mailClientPath = path;
                    break;
                }
            }
        }

        if (string.IsNullOrEmpty(mailClientPath))
            return;

        // TODO: Адрес получателя и тема берутся из свойств проекта
        string recipient = "examle@mail.ru";
        string subject = "Баг";
        string body = Uri.EscapeDataString(string.Empty);

        string arguments;
        if (mailClientPath.EndsWith("OUTLOOK.EXE", StringComparison.OrdinalIgnoreCase))
        {
            arguments = $"/c ipm.note /m \"{recipient}?subject={Uri.EscapeDataString(subject)}&body={body}\"";
        }
        else
        {
            arguments = $"-compose \"to='{recipient}',subject='{subject}',body='{body}'\"";
        }

        Process.Start(
            new ProcessStartInfo { FileName = mailClientPath, Arguments = arguments, UseShellExecute = false });
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