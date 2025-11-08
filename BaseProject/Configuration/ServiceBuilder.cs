using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using ViewModels.Common;
using ViewModels.Infrastructure;
using ViewModels.UI.Common;
using Views.UI;

namespace BaseProject.Configuration;

/// <summary>
/// Provides a centralized service-registration builder used to configure
/// dependency injection, logging, navigation, dialogs, and ViewModel bindings.
/// </summary>
public sealed class ServiceBuilder(IConfiguration configuration)
{
    private readonly IServiceCollection _serviceCollection = new ServiceCollection();
    private readonly IConfiguration _configuration = configuration
            ?? throw new ArgumentNullException(nameof(configuration));

    public IServiceProvider BuildServices(IViewModelsRegistrar? viewModelRegistrar)
    {
        ConfigureConfiguration();
        ConfigureLogging();
        ConfigureViewModels();
        ConfigureNavigation();
        ConfigureWindowManager();
        ConfigureDialogs();
        ConfigureViewLocator();

        if (viewModelRegistrar is null)
            throw new InvalidOperationException("ViewModels не зарегистрированы. Проверьте Program.cs");

        viewModelRegistrar.AddServices(_serviceCollection);
        viewModelRegistrar.AddViewModels(_serviceCollection);

        return _serviceCollection.BuildServiceProvider();
    }

    private void ConfigureConfiguration()
    {
        _serviceCollection.AddSingleton(_configuration);
    }

    private void ConfigureLogging()
    {
        var serilogLogger = new LoggerConfiguration()
            .ReadFrom.Configuration(_configuration)
            .CreateLogger();

        // Add Serilog to Microsoft ILogger
        _serviceCollection.AddLogging(builder =>
            builder.ClearProviders().AddSerilog(serilogLogger, dispose: true));
    }

    private void ConfigureViewModels()
    {
        _serviceCollection.AddTransient<VmMessageBox>();
        _serviceCollection.AddTransient<VmMessageBoxYesNo>();
    }

    private void ConfigureNavigation()
    {
        _serviceCollection.AddSingleton<IVmFactory>(sp =>
            new VmFactory(type =>
            {
                var instance = sp.GetService(type);
                return instance is VmBase vm
                    ? vm
                    : throw new InvalidOperationException($"ViewModel типа {type.FullName} не зарегистрирован.");
            }));

        _serviceCollection.AddScoped<INavigationService, NavigationService>();
    }

    private void ConfigureWindowManager()
    {
        _serviceCollection.AddSingleton<IDockingService, DockingService>();
    }

    private void ConfigureDialogs()
    {
        _serviceCollection.AddSingleton<IDialogService>(serviceProvider =>
        {
            var viewLocator = serviceProvider.GetRequiredService<IViewLocator>();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<DialogManager>();
            var dialogFactory = new DialogFactory();
            VmBase vmFactory(Type vmType) => serviceProvider.GetRequiredService<IVmFactory>().GetVm(vmType);

            var dialogManager = new DialogManager(viewLocator, dialogFactory, logger);

            return new DialogService(dialogManager, vmFactory);
        });
    }

    private void ConfigureViewLocator()
    {
        _serviceCollection.AddSingleton<IViewLocator, ViewLocator>();
    }
}
