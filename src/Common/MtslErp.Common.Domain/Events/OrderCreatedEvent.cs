using MassTransit;

namespace MtslErp.Common.Domain.Events;

[EntityName("order-created-event")]
public class OrderCreatedEvent
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public DateTime CreatedOnUtc { get; init; }
}
