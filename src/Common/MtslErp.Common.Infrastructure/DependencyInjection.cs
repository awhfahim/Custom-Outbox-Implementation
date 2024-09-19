using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MtslErp.Common.Application.Providers;
using MtslErp.Common.Domain.CoreProviders;
using MtslErp.Common.Infrastructure.Providers;
using Quartz;

namespace MtslErp.Common.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCommonInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.TryAddSingleton<IGuidProvider, GuidProvider>();
        services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.TryAddSingleton<IReflectionCacheProvider, ReflectionCacheProvider>();

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        services.AddQuartz(q =>
        {
            var scheduler = Guid.NewGuid();
            q.SchedulerId = $"default-id-{scheduler}";
            q.SchedulerName = $"default-name-{scheduler}";
        });

        return services;
    }
}
