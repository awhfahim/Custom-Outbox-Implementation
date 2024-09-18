namespace SecurityManagement.Domain.DataTransferObjects.Response;

public record AuthorizablePermissionResponse
{
    public required long PermissionId { get; init; }
    public required string PermissionLabel { get; init; }
    public required long? GroupId { get; init; }
    public required string? GroupLabel { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
}
