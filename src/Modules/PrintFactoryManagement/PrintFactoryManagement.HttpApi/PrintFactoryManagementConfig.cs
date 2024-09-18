using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MtslErp.Common.Application.Options;
using MtslErp.Common.Domain.Events;
using PrintFactoryManagement.Application;
using PrintFactoryManagement.HttpApi.Consumers;
using PrintFactoryManagement.Infrastructure;
using PrintFactoryManagement.Infrastructure.Extensions;
using ExchangeType = RabbitMQ.Client.ExchangeType;

namespace PrintFactoryManagement.HttpApi;

public static class PrintFactoryManagementConfig
{
    public static IServiceCollection RegisterPrintFactoryManagement(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.RegisterApplicationServices();
        services.AddInfrastructure(configuration);
        services.AddDatabaseConfig(configuration);

        return services;
    }

    public static void ConfigureConsumers(IRegistrationConfigurator registrationConfigurator)
    {
        registrationConfigurator.AddConsumer<CreateOrderEventConsumer>();
    }
}
public static class RabbitMqBusFactoryConfiguratorExtensions
{
    public static void ConfigurePrintModuleAEndpoints(this IRabbitMqBusFactoryConfigurator configurator,
        IBusRegistrationContext context, RabbitMqSettings rabbitMqSettings)
    {
        configurator.ReceiveEndpoint("order-created-event-queue", e =>
        {
            e.ConfigureConsumeTopology = false;

            e.Bind<OrderCreatedEvent>(x =>
            {
                x.ExchangeType = ExchangeType.Fanout;
                x.RoutingKey = "order-created-event";
            });

            e.ConfigureConsumer<CreateOrderEventConsumer>(context);
        });
    }
}
