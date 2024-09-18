using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MtslErp.Common.Application.Providers;
using MtslErp.Common.Infrastructure.Providers;

namespace MtslErp.Common.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddCommonInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.TryAddSingleton<IGuidProvider, GuidProvider>();
        services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}
