using System.Globalization;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using HanumanInstitute.MvvmDialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ViewModels.UI.Common;

#if DEBUG
using HotAvalonia;
#endif

namespace BaseProject;

public class App : Application
{
    // Статические свойства, задаваемые до инициализации приложения.
    public static IServiceProvider ServiceProvider { get; private set; } = default!;
    public static Type MainViewModelType { get; private set; } = default!;

    // Основной ViewModel, полученный через DI.
    private readonly VmBase _mainViewModel;

#pragma warning disable CS8618
    public App()
#pragma warning restore CS8618
    {
        if (Design.IsDesignMode)
            return;

        _mainViewModel = (VmBase)ServiceProvider.GetRequiredService(MainViewModelType)!;
    }

    public override void Initialize()
    {
#if DEBUG
        this.EnableHotReload(ResolveProjectPath);
#endif

        if (!Design.IsDesignMode)
            Resources[typeof(IServiceProvider)] = ServiceProvider;

        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
#pragma warning disable IL2026
        BindingPlugins.DataValidators?.RemoveAt(0);
#pragma warning restore IL2026 
        if (Design.IsDesignMode)
        {
            base.OnFrameworkInitializationCompleted();
            return;
        }

        ServiceProvider.GetRequiredService<IDialogService>().Show(null, _mainViewModel);
        base.OnFrameworkInitializationCompleted();
    }

    // Метод для установки ServiceProvider.
    public static void SetServiceProvider(IServiceProvider serviceProvider)
        => ServiceProvider = serviceProvider ?? throw new InvalidOperationException("ServiceProvider not initialized");

    // Метод для установки типа основного ViewModel.
    public static void SetMainViewModelType(Type type)
        => MainViewModelType = type ?? throw new InvalidOperationException("MainViewModelType not initialized");

    // Необходим для Avalonia Hot Reload.
    private string? ResolveProjectPath(Assembly assembly)
    {
        var assemblyName = assembly.GetName().Name;
        if (string.IsNullOrWhiteSpace(assemblyName) || assemblyName.StartsWith("System.") || assemblyName.StartsWith("Microsoft."))
            return null;

        var basePath = AppContext.BaseDirectory;
        var searchRoot = Path.Combine(basePath, "../../..");

        foreach (var file in Directory.EnumerateFiles(searchRoot, "*.csproj", SearchOption.AllDirectories))
        {
            if (Path.GetFileNameWithoutExtension(file) == assemblyName)
            {
                return Path.GetDirectoryName(file);
            }
        }

        return null;
    }
}
