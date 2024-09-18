using MtslErp.Common.Domain.Interfaces;

namespace MtslErp.Common.Domain.Entities;

public class OutboxMessage : IEntity<int>
{
    public int Id { get; init; }
    public required string Payload { get; set; }
    public required string PayloadType { get; set; }
    public bool Status { get; set; } // Pending = 1, Sent = 0
    public required DateTime CreatedOn { get; set; }
    public DateTime? SentOn { get; set; }
}
