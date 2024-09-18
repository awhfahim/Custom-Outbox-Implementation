namespace SecurityManagement.Domain.DataTransferObjects.Response;

public record DuplicateRolePermissionCheckResponse
{
    public required long RoleId { get; set; }
    public required string RoleLabel { get; set; }
    public required long PermissionId { get; set; }
    public required string PermissionLabel { get; set; }
};
