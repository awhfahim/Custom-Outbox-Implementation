using MtslErp.Common.Domain.CoreProviders;
using MtslErp.Common.Domain.DataTransferObjects.Request;
using MtslErp.Common.Domain.DataTransferObjects.Response;
using MtslErp.Common.Domain.Repositories;
using SecurityManagement.Domain.DataTransferObjects.Response;
using SecurityManagement.Domain.Entities;

namespace SecurityManagement.Domain.Repositories;

public interface IUserRoleRepository : IRepositoryBase<UserRole>
{
    Task<ICollection<AuthorizableRoleResponse>> GetAllRolesOfUser(long userId,
        CancellationToken cancellationToken = default);

    Task<PagedData<AuthorizableRoleResponse>> GetAllRolesOfUser(long userId, DynamicQueryDto dto,
        IReflectionCacheProvider reflectionCacheProvider, CancellationToken cancellationToken = default);
    Task<bool> BulkDelete(long userId, IReadOnlyCollection<long> roleIds);

    Task<IReadOnlyCollection<AuthCheckResponse>> CheckForDuplicates(long userId,
        IReadOnlyCollection<long> roleIds);
}
