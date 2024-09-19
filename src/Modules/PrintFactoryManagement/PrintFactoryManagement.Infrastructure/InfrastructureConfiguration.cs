using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MtslErp.Common.Application.Data;
using PrintFactoryManagement.Application;
using PrintFactoryManagement.Domain.Orders;
using PrintFactoryManagement.Domain.Repositories;
using PrintFactoryManagement.Infrastructure.Data;
using PrintFactoryManagement.Infrastructure.OutboxProcessor;
using PrintFactoryManagement.Infrastructure.Persistence;
using PrintFactoryManagement.Infrastructure.Persistence.Repositories;
using Quartz;

namespace PrintFactoryManagement.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        #region repositories

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IPfmOutboxRepository, PfmOutboxRepository>();

        #endregion

        services.AddScoped<IPrintFactoryAppUnitOfWork, PrintFactoryAppUnitOfWork>();
        services.TryAddScoped<IDbConnectionFactory, PfmDbConnectionFactory>();

        services.AddOptions<PrintFactoryManagementDb>()
            .BindConfiguration("PrintFactoryManagementDb");

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        services.AddQuartz(q =>
        {
            var scheduler = Guid.NewGuid();
            q.SchedulerId = $"default-id-{scheduler}";
            q.SchedulerName = $"default-name-{scheduler}";
        });

        services.ConfigureOptions<ConfigureProcessOutboxJob>();

        return services;
    }
}
