using Microsoft.Extensions.DependencyInjection;
using MtslErp.Common.Application.Options;

namespace MtslErp.Common.Application;

public static class ApplicationConfiguration
{
    public static IServiceCollection AddCommonApplicationServices(this IServiceCollection services)
    {
        services.AddOptions<ConnectionStringOptions>()
            .ValidateOnStart()
            .ValidateDataAnnotations()
            .BindConfiguration("ConnectionStringOptions");

        return services;
    }
}
