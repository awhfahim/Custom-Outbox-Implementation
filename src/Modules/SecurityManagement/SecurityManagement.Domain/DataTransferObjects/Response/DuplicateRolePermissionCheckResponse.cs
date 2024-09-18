namespace ErpSoftware.Domain.DataTransferObjects.Response.Auth;

public record DuplicateRolePermissionCheckResponse
{
    public required long RoleId { get; set; }
    public required string RoleLabel { get; set; }
    public required long PermissionId { get; set; }
    public required string PermissionLabel { get; set; }
};
