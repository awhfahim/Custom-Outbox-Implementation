using MassTransit;
using MtslErp.Common.Domain.Events;

namespace PrintFactoryManagement.HttpApi.Consumers;

public class TestEventConsumer : IConsumer<TestEvent>
{
    public Task Consume(ConsumeContext<TestEvent> context)
    {
        Console.WriteLine($"Test received: {context.Message}");
        return Task.CompletedTask;
    }
}
