using MtslErp.Common.Domain.DataTransferObjects.Request;
using MtslErp.Common.Domain.DataTransferObjects.Response;
using SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects;
using SecurityManagement.Domain.DataTransferObjects.Response;
using SecurityManagement.Domain.Entities;
using SharpOutcome;
using SharpOutcome.Helpers;

namespace SecurityManagement.Application.Features.AuthFeatures.Interfaces;

public interface IAuthPermissionService
{
    Task<ValueOutcome<Successful, AuthorizableBadOutcome>> DeletePermissionAndUpdateCacheAsync(long id,
        CancellationToken ct = default);

    Task UpdateCacheFromDatabaseAsync(ICollection<string> roles, CancellationToken ct = default);

    Task<PagedData<AuthorizablePermissionResponse>> ReadAllAsync(DynamicQueryDto dto,
        CancellationToken ct = default);

    Task<Dictionary<string, string[]>> GetPermissionsByRolesFromCacheAsync(ICollection<string> roles);

    Task<Dictionary<string, string[]>> GetPermissionsByRolesAsync(ICollection<string> roles,
        bool cacheResult, CancellationToken ct = default);

    Task<AuthorizablePermission?> GetPermissionByLabelAsync(string permission,
        CancellationToken ct = default);

    Task<AuthorizablePermission?> GetPermissionByIdAsync(long id, CancellationToken ct = default);

    Task<ICollection<AuthorizablePermission>> GetPermissionsAsync(int page, int limit,
        CancellationToken ct = default);

    Task<ValueOutcome<AuthorizablePermission, AuthorizableBadOutcome>> CreatePermissionAsync(
        PermissionCreateOrUpdateModel dto, CancellationToken ct = default);

    Task<ValueOutcome<ICollection<AuthorizablePermission>, AuthorizableBadOutcome>> CreatePermissionAsync(
        ICollection<PermissionCreateOrUpdateModel> dto, CancellationToken ct = default);

    Task<ValueOutcome<AuthorizablePermission, AuthorizableBadOutcome>> UpdatePermissionAsync(
        long id, PermissionCreateOrUpdateModel dto, CancellationToken ct = default);

    Task<ValueOutcome<Successful, AuthorizableBadOutcome>> DeletePermissionAsync(long id,
        CancellationToken ct = default);

    Task<AuthorizablePermissionGroup?> GetPermissionGroupByLabelAsync(string permission,
        CancellationToken ct = default);

    Task<AuthorizablePermissionGroup?> GetPermissionGroupByIdAsync(long id,
        CancellationToken ct = default);

    Task<ValueOutcome<AuthorizablePermissionGroup, AuthorizableBadOutcome>> CreatePermissionGroupAsync(
        PermissionGroupCreateOrUpdateModel dto, CancellationToken ct = default);

    Task<ValueOutcome<AuthorizablePermissionGroup, AuthorizableBadOutcome>> UpdatePermissionGroupAsync(
        long id, PermissionGroupCreateOrUpdateModel dto, CancellationToken ct = default);

    Task<ValueOutcome<Successful, AuthorizableBadOutcome>> DeletePermissionGroupAsync(long id,
        CancellationToken ct = default);
}
