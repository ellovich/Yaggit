using System.Reflection;
using Microsoft.Extensions.Configuration;
using Models.Enums.Common;

namespace BaseProject.Configuration;

internal static class ConfigurationLoader
{
    public static IConfiguration LoadConfiguration()
    {
        var assembly = Assembly.GetExecutingAssembly();
        string resourceName = GetResourceName();

        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new FileNotFoundException($"Не удалось найти ресурс {resourceName}");
        var configuration = new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();

        return configuration;
    }

    private static string GetResourceName()
    {
#if DEBUG
        return "BaseProject.Configuration.appsettings.Development.json";
#else
    return "BaseProject.Configuration.appsettings.json";
#endif
    }

    public static string GetConnectionString(this IConfiguration configuration, eConnString connStr)
    {
        return configuration.GetConnectionString(Enum.GetName(connStr)!)!;
    }
}