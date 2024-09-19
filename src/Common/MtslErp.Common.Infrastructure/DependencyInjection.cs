using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MtslErp.Common.Application.Providers;
using MtslErp.Common.Domain.CoreProviders;
using MtslErp.Common.Infrastructure.Providers;

namespace MtslErp.Common.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCommonInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.TryAddSingleton<IGuidProvider, GuidProvider>();
        services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.TryAddSingleton<IReflectionCacheProvider, ReflectionCacheProvider>();

        return services;
    }
}
