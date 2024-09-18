using MassTransit;

namespace MtslErp.Common.Domain.Events;

[EntityName("test-event")]
public class TestEvent
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public DateTime CreatedOnUtc { get; init; }
}
