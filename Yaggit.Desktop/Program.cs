using Avalonia;
using BaseProject;
using BaseProject.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Models.Services.Yaggit.Contracts;
using Models.Services.Yaggit;
using ViewModels.UI.Yaggit;

namespace Yaggit.Desktop;

internal class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .SetServices(
            typeof(ViewModels.UI.Yaggit.VmYaggitMain),
            new ViewModelsRegistrar(),
            typeof(Program)
        )
        .StartWithClassicDesktopLifetime(args);

    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}

public class ViewModelsRegistrar : IViewModelsRegistrar
{
    public void AddServices(IServiceCollection services)
    {
        services.AddSingleton<IGitRepositoryContext, GitRepositoryContext>();
        services.AddSingleton<IGitCommandService, GitCommandService>();

        services.AddTransient<IGitBranchesService, GitBranchesService>();
        services.AddTransient<IGitRepositoryInitializer, GitRepositoryInitializer>();
    }

    public void AddViewModels(IServiceCollection services)
    {
        services.AddSingleton<VmYaggitMain>();

        services.AddTransient<VmRepoSelector>();
        services.AddTransient<VmBranches>();
        services.AddTransient<VmConsole>();
    }
}