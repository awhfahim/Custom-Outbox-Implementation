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

public class RolePermissionRepository : Repository<RolePermission>, IRolePermissionRepository
{
    private readonly SecurityManagementDbContext _securityManagementDbContext;

    public RolePermissionRepository(SecurityManagementDbContext context) : base(context)
    {
        _securityManagementDbContext = context;
    }

    public async Task<IReadOnlyCollection<DuplicateRolePermissionCheckResponse>> CheckForDuplicates(
        long roleId, IReadOnlyCollection<long> permissionIds)
    {
        var query = from middleTable in EntityDbSet.Where(e =>
                permissionIds.Contains(e.AuthorizablePermissionId) && e.AuthorizableRoleId == roleId)
            join role in _securityManagementDbContext.AuthorizableRoles
                on middleTable.AuthorizableRoleId equals role.Id
            join permission in _securityManagementDbContext.AuthorizablePermissions
                on middleTable.AuthorizablePermissionId equals permission.Id
            select new DuplicateRolePermissionCheckResponse
            {
                PermissionId = permission.Id,
                RoleId = role.Id,
                PermissionLabel = permission.Label,
                RoleLabel = role.Label
            };

        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<PagedData<AuthorizablePermissionResponse>> GetAllPermissionsOfRoleAsync(long roleId,
        DynamicQueryDto dto, IReflectionCacheProvider reflectionCacheProvider,
        CancellationToken cancellationToken = default)
    {
        var query = from rolePermissions in EntityDbSet
            where rolePermissions.AuthorizableRoleId == roleId
            join permissions in _securityManagementDbContext.AuthorizablePermissions
                on rolePermissions.AuthorizablePermissionId equals permissions.Id
            join permissionGroupTemp in _securityManagementDbContext.AuthorizablePermissionGroups
                on permissions.GroupId equals permissionGroupTemp.Id
                into leftJoinAble
            from permissionGroups in leftJoinAble.DefaultIfEmpty()
            select new AuthorizablePermissionResponse
            {
                PermissionId = permissions.Id,
                PermissionLabel = permissions.Label,
                GroupId = permissionGroups.Id,
                GroupLabel = permissionGroups.Label,
                CreatedAtUtc = rolePermissions.CreatedAtUtc
            };

        query = query.WhereDynamic(dto.Filters, reflectionCacheProvider);

        var count = await query.CountAsync(cancellationToken);

        query = dto.Sorters.Count == 0
            ? query.OrderByDescending(x => x.CreatedAtUtc)
            : query.OrderBy(dto.Sorters, reflectionCacheProvider);

        var data = await query
            .PaginateQueryable(dto.Page, dto.Size)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new PagedData<AuthorizablePermissionResponse>(data, count);
    }

    public async Task<int> BulkDelete(long roleId, IReadOnlyCollection<long> permissionIds)
    {
        if (permissionIds.Count == 0)
        {
            return 0;
        }

        return await EntityDbSet
            .Where(x => permissionIds.Contains(x.AuthorizablePermissionId) && x.AuthorizableRoleId == roleId)
            .ExecuteDeleteAsync();
    }
}
