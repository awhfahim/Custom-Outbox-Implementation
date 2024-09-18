using Microsoft.Extensions.DependencyInjection;
using PrintFactoryManagement.Application.Features.PrintFactoryFeatures.Services;

namespace PrintFactoryManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPrintFactoryService, PrintFactoryService>();
        return services;
    }
}
