using MtslErp.Common.Domain.Interfaces;

namespace SecurityManagement.Domain.Entities;

public class UserRole : ICreationTimeStamp
{
    public required long UserId { get; init; }
    public required long AuthorizableRoleId { get; set; }
    public required DateTime CreatedAtUtc { get; init; }
}
