using MtslErp.Common.Domain.DataTransferObjects.Request;
using MtslErp.Common.Domain.DataTransferObjects.Response;
using SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects;
using SecurityManagement.Domain.DataTransferObjects.Response;
using SecurityManagement.Domain.Entities;
using SharpOutcome;
using SharpOutcome.Helpers;

namespace SecurityManagement.Application.Features.AuthFeatures.Interfaces;

public interface IAuthRoleService
{
    Task<PagedData<AuthorizableRoleResponse>> GetAllRolesOfUserAsync(long id, DynamicQueryDto dto,
        CancellationToken ct = default);

    Task<PagedData<AuthorizableRole>> ReadAllAsync(DynamicQueryDto dto, CancellationToken ct = default);
    Task ClearRolePermissionsCacheAsync(string roleLabel);
    public Task<AuthorizableRole?> GetRoleByLabelAsync(string role, CancellationToken ct = default);
    public Task<AuthorizableRole?> GetRoleByIdAsync(long id, CancellationToken ct = default);

    public Task<PagedData<AuthorizableRoleResponse>> GetAssignableRolesAsync(DynamicQueryDto dto,
        long userId, CancellationToken ct = default);

    public Task<bool> IsRoleExistsAsync(string label, CancellationToken ct = default);
    public Task<bool> IsRoleExistsAsync(long id, CancellationToken ct = default);
    public Task<ValueOutcome<AuthorizableRole, AuthorizableBadOutcome>> CreateRoleAsync(string role);

    public Task<ValueOutcome<AuthorizableRole, AuthorizableBadOutcome>> UpdateRoleAsync(long roleId,
        string updatedRole);

    public Task<ValueOutcome<Successful, AuthorizableBadOutcome>> DeleteRoleAsync(long id);

    public Task<ValueOutcome<string, PermissionsToRoleAssignmentBadOutcome>> AssignPermissionsToRoleAsync(
        long roleId, IReadOnlyCollection<long> permissionIds);

    public Task<PagedData<AuthorizablePermissionResponse>> GetAssignablePermissionsAsync(
        DynamicQueryDto dto, long roleId, CancellationToken ct = default);

    Task<PagedData<AuthorizablePermissionResponse>> GetAllPermissionsOfRoleAsync(long roleId,
        DynamicQueryDto dto, CancellationToken ct = default);

    Task<(string? RoleLabel, int Modified)> RemovePermissionsFromRoleAsync(long roleId,
        IReadOnlyCollection<long> permissionIds);
}
