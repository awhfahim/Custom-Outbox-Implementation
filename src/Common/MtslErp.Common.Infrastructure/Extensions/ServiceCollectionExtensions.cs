using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MtslErp.Common.Application.Options;

namespace MtslErp.Common.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRedisConfig(this IServiceCollection services,
        IConfiguration configuration, string? prefix = null)
    {
        var connectionString = configuration.GetRequiredSection(ConnectionStringOptions.SectionName)
            .GetValue<string>(nameof(ConnectionStringOptions.StackExchangeRedisUrl));

        services.AddStackExchangeRedisCache(opts =>
        {
            opts.Configuration = connectionString;
            opts.InstanceName = prefix ?? "mtsl_erp_";
        });
        return services;
    }
}
