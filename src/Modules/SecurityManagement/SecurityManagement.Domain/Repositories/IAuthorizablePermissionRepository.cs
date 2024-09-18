using MtslErp.Common.Domain.CoreProviders;
using MtslErp.Common.Domain.DataTransferObjects.Request;
using MtslErp.Common.Domain.DataTransferObjects.Response;
using MtslErp.Common.Domain.Repositories;
using SecurityManagement.Domain.DataTransferObjects.Response;
using SecurityManagement.Domain.Entities;

namespace SecurityManagement.Domain.Repositories;

public interface IAuthorizablePermissionRepository : ITimestampedEntityRepository<AuthorizablePermission>
{
    Task<PagedData<AuthorizablePermissionResponse>> GetAllAsync(DynamicQueryDto dto,
        IReflectionCacheProvider reflectionCacheProvider, CancellationToken ct = default);

    Task<ICollection<string>> GetRoleLabelsOfPermission(long permissionId,
        CancellationToken cancellationToken = default);

    Task<Dictionary<string, string[]>> GetPermissionsByRolesOfUser(
        ICollection<string> rolesLabels, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<AuthCheckResponse>> CheckForExistingPermissions(
        IReadOnlyCollection<long> permissionIds, CancellationToken ct = default);

    Task<IReadOnlyCollection<AuthCheckResponse>> CheckForExistingPermissions(
        IReadOnlyCollection<string> permissionLabels, CancellationToken ct = default);

    Task<PagedData<AuthorizablePermissionResponse>> GetAllPermissionsExceptAssigned(long roleId,
        DynamicQueryDto dto, IReflectionCacheProvider reflectionCacheProvider,
        CancellationToken cancellationToken = default);
}
