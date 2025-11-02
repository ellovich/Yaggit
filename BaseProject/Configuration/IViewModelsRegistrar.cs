using Microsoft.Extensions.DependencyInjection;

namespace BaseProject.Configuration;

public interface IViewModelsRegistrar
{
    void AddViewModels(IServiceCollection services);

    void AddServices(IServiceCollection services);
}