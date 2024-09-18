using MtslErp.Common.Domain.CoreProviders;
using MtslErp.Common.Domain.DataTransferObjects.Request;
using MtslErp.Common.Domain.DataTransferObjects.Response;
using MtslErp.Common.Domain.Repositories;
using SecurityManagement.Domain.DataTransferObjects.Response;
using SecurityManagement.Domain.Entities;

namespace SecurityManagement.Domain.Repositories;

public interface IRolePermissionRepository : IRepositoryBase<RolePermission>
{
    Task<IReadOnlyCollection<DuplicateRolePermissionCheckResponse>> CheckForDuplicates(long roleId,
        IReadOnlyCollection<long> permissionIds);

    Task<PagedData<AuthorizablePermissionResponse>> GetAllPermissionsOfRoleAsync(long roleId,
        DynamicQueryDto dto, IReflectionCacheProvider reflectionCacheProvider,
        CancellationToken cancellationToken = default);

    Task<int> BulkDelete(long roleId, IReadOnlyCollection<long> permissionIds);
}
