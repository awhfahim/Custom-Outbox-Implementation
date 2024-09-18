namespace SecurityManagement.Domain.DataTransferObjects.Response;

public record AuthorizableRoleResponse
{
    public required long RoleId { get; init; }
    public required string RoleLabel { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
}
