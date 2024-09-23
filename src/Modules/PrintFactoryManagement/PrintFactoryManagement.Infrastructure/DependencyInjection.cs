using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MtslErp.Common.Application.Data;
using PrintFactoryManagement.Application;
using PrintFactoryManagement.Domain.Repositories;
using PrintFactoryManagement.Domain.Repositories.Outbox;
using PrintFactoryManagement.Infrastructure.OutboxProcessor;
using PrintFactoryManagement.Infrastructure.Persistence;
using PrintFactoryManagement.Infrastructure.Persistence.Repositories;
using PrintFactoryManagement.Infrastructure.Persistence.Repositories.Outbox;
using Quartz;

namespace PrintFactoryManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        #region Add Repositories

        services.AddScoped<IPrintFactoryOutboxRepository, PrintFactoryOutboxRepository>();

        #endregion

        services.AddScoped<IPrintFactoryAppUnitOfWork, PrintFactoryAppUnitOfWork>();

        services
            .ConfigureOptions<
                PrintFactoryManagement.Infrastructure.OutboxProcessor.ConfigureProcessOutboxJob>();

        return services;
    }
}
