using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MtslErp.Common.Application.Options;
using MtslErp.Common.Domain.Events;
using PrintFactoryManagement.Application;
using PrintFactoryManagement.Infrastructure;
using PrintFactoryManagement.Infrastructure.Consumers;
using PrintFactoryManagement.Infrastructure.Extensions;
using RabbitMQ.Client;

namespace PrintFactoryManagement.HttpApi;

public static class PrintFactoryManagementConfig
{
    public static IServiceCollection RegisterPrintFactoryManagement(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddApplicationServices();
        services.AddInfrastructureServices(configuration);
        services.AddDatabaseConfig(configuration);

        return services;
    }

    public static void ConfigureConsumers(IRegistrationConfigurator registrationConfigurator)
    {
        registrationConfigurator.AddConsumer<UserRegisteredEventConsumer>();
    }
}
public static class RabbitMqBusFactoryConfiguratorExtensions
{
    public static void ConfigurePrintModuleEndpoints(this IRabbitMqBusFactoryConfigurator configurator,
        IBusRegistrationContext context, RabbitMqSettings rabbitMqSettings)
    {
        configurator.ReceiveEndpoint("user-created-event-queue", e =>
        {
            e.ConfigureConsumeTopology = false;

            e.Bind<UserRegisteredEvent>(x =>
            {
                x.ExchangeType = ExchangeType.Fanout;
                x.RoutingKey = "user-created-event";
            });

            e.ConfigureConsumer<UserRegisteredEventConsumer>(context);
        });
    }
}
