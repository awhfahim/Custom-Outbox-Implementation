using MassTransit;
using MtslErp.Common.Domain.Events;

namespace PrintFactoryManagement.HttpApi.Consumers;

public class CreateOrderEventConsumer : IConsumer<OrderCreatedEvent>
{
    public Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        Console.WriteLine($"Order received: {context.Message}");
        return Task.CompletedTask;
    }
}
