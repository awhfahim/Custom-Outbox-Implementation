using MassTransit;
using MtslErp.Common.Application.Options;
using MtslErp.Common.Domain.Events;
using PrintFactoryManagement.HttpApi;
using RabbitMQ.Client;

namespace MtslErp.Api.Extensions;

public static class MasstransitConfigurationExtensions
{
    public static IServiceCollection RegisterMasstransit(this IServiceCollection services,
        IConfiguration configuration)
    {
        Action<IRegistrationConfigurator>[] moduleConfigureConsumers =
            [PrintFactoryManagementConfig.ConfigureConsumers]; // Add Consumers Here

        var rabbitMqSettings = configuration.GetRequiredSection("RabbitMq").Get<RabbitMQSettings>();

        ArgumentNullException.ThrowIfNull(rabbitMqSettings);

        services.AddMassTransit(configure =>
        {
            foreach (var configureConsumer in moduleConfigureConsumers)
            {
                configureConsumer(configure);
            }

            configure.SetKebabCaseEndpointNameFormatter();

            configure.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqSettings.Host, rabbitMqSettings.VirtualHost, h =>
                {
                    h.Username(rabbitMqSettings.Username);
                    h.Password(rabbitMqSettings.Password);
                    h.Heartbeat(60);
                });

                // Example of publishing an event with custom configuration
                cfg.Publish<OrderCreatedEvent>(e =>
                {
                    e.ExchangeType = ExchangeType.Fanout;
                    e.Durable = true;
                });

                cfg.PrefetchCount = rabbitMqSettings.PrefetchCount;
                cfg.ConcurrentMessageLimit = rabbitMqSettings.ConcurrentConsumers;
                cfg.Durable = true;

                cfg.UseMessageRetry(r =>
                {
                    r.Interval(3, TimeSpan.FromSeconds(5));
                });

                cfg.ConfigureModuleEndpoints(context, rabbitMqSettings);
            });
        });

        return services;
    }

    private static void ConfigureModuleEndpoints(this IRabbitMqBusFactoryConfigurator configurator,
        IBusRegistrationContext context, RabbitMQSettings rabbitMqSettings)
    {
        configurator.ConfigurePrintModuleAEndpoints(context, rabbitMqSettings);
    }
}

