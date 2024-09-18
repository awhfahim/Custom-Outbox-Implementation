using MtslErp.Common.Domain.Interfaces;

namespace SecurityManagement.Domain.Entities;

public class RolePermission : ICreationTimeStamp
{
    public required long AuthorizableRoleId { get; set; }
    public required long AuthorizablePermissionId { get; set; }
    public required DateTime CreatedAtUtc { get; init; }
}
