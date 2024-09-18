using MtslErp.Common.Domain.Interfaces;

namespace SecurityManagement.Domain.Entities;

public class AuthorizablePermission : IAutoIncrementalEntity<long>, ITimestamp
{
    public long Id { get; }
    public required long? GroupId { get; set; }
    public required string Label { get; set; }
    public required DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; set; }
}
