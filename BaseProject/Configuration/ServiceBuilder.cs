using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Models.Enums.Common;
using Serilog;
using ViewModels.Common;
using ViewModels.Infrastructure;
using ViewModels.UI.Common;
using Views.UI;

namespace BaseProject.Configuration;

public static class ConfigurationExtensions
{
    public static string GetConnectionString(this IConfiguration configuration, eConnString connStr)
    {
        return configuration.GetConnectionString(Enum.GetName(typeof(eConnString), connStr)!)!;
    }
}

public class ServiceBuilder
{
    protected IServiceCollection _serviceCollection;
    private readonly IConfiguration _configuration;
    protected ILoggerFactory _loggerFactory;
    private IServiceProvider _serviceProvider;

#pragma warning disable CS8618

    public ServiceBuilder(IConfiguration configuration)
#pragma warning restore CS8618
    {
        _configuration = configuration;
        _serviceCollection ??= new ServiceCollection();
    }

    public IServiceProvider BuildServices(Type programType, IViewModelsRegistrar? viewModelRegistrar)
    {
        AddConfiguration();
        AddLogger();
        AddStandardViewModels();
        AddNavigationService();
        AddWindowManager();

        // services.AddSingleton<IFilesService, FilesService>();
        // services.AddSingleton<ISettingsService, SettingsService>();
        // services.AddSingleton<IClipboardService, ClipboardService>();
        // services.AddSingleton<IShareService, ShareService>();
        // services.AddSingleton<IEmailService, EmailService>();

        if (viewModelRegistrar != null)
        {
            viewModelRegistrar.AddServices(_serviceCollection);
            viewModelRegistrar.AddViewModels(_serviceCollection);
        }
        else
        {
            throw new Exception("Не зарегистрированы ViewModels в Program.cs");
        }

        AddViewLocator();
        AddDialogsService();
        _serviceProvider = _serviceCollection.BuildServiceProvider();
        return _serviceProvider;
    }

    private void AddViewLocator()
    {
        _serviceCollection.AddSingleton<IViewLocator, ViewLocator>();
    }

    private void AddStandardViewModels()
    {
        _serviceCollection.AddTransient<VmMessageBox>();
        _serviceCollection.AddTransient<VmMessageBoxYesNo>();
    }

    private void AddNavigationService()
    {
        _serviceCollection.AddSingleton<IVmFactory>(sp =>
            new VmFactory(type =>
            {
                var service = sp.GetService(type);
                return service != null
                    ? (VmBase)service
                    : throw new InvalidOperationException($"ViewModel типа {type.FullName} не зарегистрирован в Program.cs");
            })
        );
        _serviceCollection.AddScoped<INavigationService, NavigationService>();
    }

    private void AddWindowManager()
    {
        _serviceCollection.AddSingleton<IDockingService, DockingService>();
    }

    private void AddConfiguration()
    {
        _serviceCollection.AddSingleton(_configuration);
    }

    private void AddLogger()
    {
        var serilogLogger = new LoggerConfiguration()
            .ReadFrom.Configuration(_configuration)
            .CreateLogger();

        _loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(serilogLogger));

        _serviceCollection.AddSingleton(_loggerFactory);
        _serviceCollection.AddLogging(loggingBuilder =>
        loggingBuilder.AddSerilog(serilogLogger, dispose: true));
    }

    private void AddDialogsService()
    {
        _serviceCollection.AddSingleton<IDialogService>(provider =>
            new DialogService(
                new DialogManager(
                    viewLocator: provider.GetRequiredService<IViewLocator>(),
                    dialogFactory: new DialogFactory(),// .AddMessageBox(MessageBoxMode.Popup),
                    logger: _loggerFactory.CreateLogger<DialogManager>()),
                viewModelFactory: x => provider.GetRequiredService<IVmFactory>().GetVm(x))
        );
    }
}