using MtslErp.Common.Domain.Interfaces;

namespace MtslErp.Common.Domain.Entities;

public class OutboxMessage : IEntity<int>
{
    public int Id { get; init; }
    public required string Payload { get; init; }
    public required string PayloadType { get; init; }
    public bool Status { get; set; }
    public required DateTime CreatedOn { get; init; }
    public DateTime? SentOn { get; set; }
}
