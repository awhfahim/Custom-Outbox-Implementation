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
    public static IServiceCollection RegisterPrintFactoryManagementHttpApi(this IServiceCollection services,
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
        registrationConfigurator.AddConsumer<TestEventConsumer>();
    }
}
public static class RabbitMqBusFactoryConfiguratorExtensions
{
    public static void ConfigurePrintModuleAEndpoints(this IRabbitMqBusFactoryConfigurator configurator,
        IBusRegistrationContext context, RabbitMQSettings rabbitMqSettings)
    {
        configurator.ReceiveEndpoint("order-created-event-queue", e =>
        {
            e.ConfigureConsumeTopology = false;

            e.Bind<OrderCreatedEvent>(x =>
            {
                x.ExchangeType = ExchangeType.Fanout;
                x.Durable = true;
            });

            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
            e.Durable = true;

            e.ConfigureConsumer<CreateOrderEventConsumer>(context);
        });

        configurator.ReceiveEndpoint("test-event-queue", e =>
        {
            e.ConfigureConsumeTopology = false;

            e.Bind<TestEvent>(x =>
            {
                x.ExchangeType = ExchangeType.Fanout;
                x.Durable = true;
            });

            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
            e.Durable = true;

            e.ConfigureConsumer<CreateOrderEventConsumer>(context);
        });
    }
}
