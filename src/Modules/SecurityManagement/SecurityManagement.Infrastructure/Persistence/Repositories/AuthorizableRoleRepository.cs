using Microsoft.EntityFrameworkCore;
using MtslErp.Common.Domain.CoreProviders;
using MtslErp.Common.Domain.DataTransferObjects.Request;
using MtslErp.Common.Domain.DataTransferObjects.Response;
using MtslErp.Common.Infrastructure.Extensions;
using MtslErp.Common.Infrastructure.Persistence.Repositories;
using SecurityManagement.Domain.DataTransferObjects.Response;
using SecurityManagement.Domain.Entities;
using SecurityManagement.Domain.Repositories;

namespace SecurityManagement.Infrastructure.Persistence.Repositories;

public class AuthorizableRoleRepository : TimestampedEntityRepository<AuthorizableRole>,
    IAuthorizableRoleRepository
{
    private readonly SecurityManagementDbContext _securityManagementDbContext;

    public AuthorizableRoleRepository(SecurityManagementDbContext context) : base(context)
    {
        _securityManagementDbContext = context;
    }

    public async Task<IReadOnlyCollection<AuthCheckResponse>> CheckForExistingRoles(
        IReadOnlyCollection<long> roleIds)
    {
        return await EntityDbSet
            .Where(e => roleIds.Contains(e.Id))
            .Select(e => new AuthCheckResponse(e.Id, e.Label))
            .AsNoTracking()
            .ToListAsync();
    }


    public async Task<PagedData<AuthorizableRoleResponse>> GetAllRolesExceptAssigned(long userId,
        DynamicQueryDto dto, IReflectionCacheProvider reflectionCacheProvider,
        CancellationToken cancellationToken = default)
    {
        var query = EntityDbSet
            .Where(roles => !_securityManagementDbContext.UserRoles
                .Where(userRoles => userRoles.UserId == userId)
                .Select(userRoles => userRoles.AuthorizableRoleId)
                .Contains(roles.Id))
            .Select(x => new AuthorizableRoleResponse
            {
                RoleId = x.Id, RoleLabel = x.Label, CreatedAtUtc = x.CreatedAtUtc
            });

        query = query.WhereDynamic(dto.Filters, reflectionCacheProvider);

        var count = await query.LongCountAsync(cancellationToken);

        query = dto.Sorters.Count == 0
            ? query.OrderByDescending(x => x.CreatedAtUtc)
            : query.OrderBy(dto.Sorters, reflectionCacheProvider);


        var data = await query
            .PaginateQueryable(dto.Page, dto.Size)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new PagedData<AuthorizableRoleResponse>(data, count);
    }
}
