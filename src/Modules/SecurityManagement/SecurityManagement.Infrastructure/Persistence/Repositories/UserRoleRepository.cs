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

public class UserRoleRepository : Repository<UserRole>, IUserRoleRepository
{
    private readonly SecurityManagementDbContext _securityManagementDbContext;

    public UserRoleRepository(SecurityManagementDbContext context) : base(context)
    {
        _securityManagementDbContext = context;
    }

    public async Task<PagedData<AuthorizableRoleResponse>> GetAllRolesOfUser(long userId,
        DynamicQueryDto dto, IReflectionCacheProvider reflectionCacheProvider,
        CancellationToken cancellationToken = default)
    {
        var query = from userRoles in EntityDbSet
            where userRoles.UserId == userId
            join roles in _securityManagementDbContext.AuthorizableRoles
                on userRoles.AuthorizableRoleId equals roles.Id
            select new AuthorizableRoleResponse
            {
                RoleId = roles.Id, RoleLabel = roles.Label, CreatedAtUtc = userRoles.CreatedAtUtc
            };

        query = query.WhereDynamic(dto.Filters, reflectionCacheProvider);

        var count = await query.CountAsync(cancellationToken);

        query = dto.Sorters.Count == 0
            ? query.OrderByDescending(x => x.CreatedAtUtc)
            : query.OrderBy(dto.Sorters, reflectionCacheProvider);

        query = query.PaginateQueryable(dto.Page, dto.Size).AsNoTracking();

        return new PagedData<AuthorizableRoleResponse>(await query.ToListAsync(cancellationToken), count);
    }

    public async Task<ICollection<AuthorizableRoleResponse>> GetAllRolesOfUser(long userId,
        CancellationToken cancellationToken = default)
    {
        var query = from userRoles in EntityDbSet
            where userRoles.UserId == userId
            join roles in _securityManagementDbContext.AuthorizableRoles
                on userRoles.AuthorizableRoleId equals roles.Id
            orderby roles.Label
            select new AuthorizableRoleResponse
            {
                RoleId = roles.Id, RoleLabel = roles.Label, CreatedAtUtc = userRoles.CreatedAtUtc
            };

        return await query.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<bool> BulkDelete(long userId, IReadOnlyCollection<long> roleIds)
    {
        if (roleIds.Count == 0)
        {
            return false;
        }

        var status = await EntityDbSet
            .Where(x => roleIds.Contains(x.AuthorizableRoleId) && x.UserId == userId)
            .ExecuteDeleteAsync();

        return status != 0;
    }

    public async Task<IReadOnlyCollection<AuthCheckResponse>> CheckForDuplicates(
        long userId, IReadOnlyCollection<long> roleIds)
    {
        var query = from userRoles in EntityDbSet.Where(e =>
                roleIds.Contains(e.AuthorizableRoleId) && e.UserId == userId)
            join role in _securityManagementDbContext.AuthorizableRoles
                on userRoles.AuthorizableRoleId equals role.Id
            select new AuthCheckResponse(role.Id, role.Label);

        return await query.AsNoTracking().ToListAsync();
    }
}
