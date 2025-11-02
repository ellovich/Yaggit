using Avalonia;
using BasaProject.Configuration;
using BaseProject.Configuration;

namespace BaseProject;

public static class AppStarter
{
    public static AppBuilder SetServices(
        this AppBuilder appBuilder,
        Type mainViewModelType,
        IViewModelsRegistrar viewModelsRegistrar,
        Type program)
    {
        var configuration = ConfigurationLoader.LoadConfiguration();
        var serviceBuilder = new ServiceBuilder(configuration);

        App.SetMainViewModelType(mainViewModelType);
        var serviceProvider = serviceBuilder.BuildServices(program, viewModelsRegistrar);
        App.SetServiceProvider(serviceProvider);

        return appBuilder;
    }
}

/// <summary>
/// Used by visual designer.
/// </summary>
internal class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}