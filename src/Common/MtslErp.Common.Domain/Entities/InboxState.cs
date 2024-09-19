using MtslErp.Common.Domain.Interfaces;

namespace MtslErp.Common.Domain.Entities;

public sealed class InboxState : IEntity<int>
{
    public int Id { get; init; }
    public required string MessageId { get; init; }
    public required DateTime ProcessedOn { get; init; }
}
