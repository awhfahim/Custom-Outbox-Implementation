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

public class AuthorizablePermissionRepository : Repository<AuthorizablePermission>,
    IAuthorizablePermissionRepository
{
    private readonly SecurityManagementDbContext _securityManagementDbContext;

    public AuthorizablePermissionRepository(SecurityManagementDbContext context) : base(context)
    {
        _securityManagementDbContext = context;
    }

    public async Task<PagedData<AuthorizablePermissionResponse>> GetAllAsync(DynamicQueryDto dto,
        IReflectionCacheProvider reflectionCacheProvider, CancellationToken ct = default)
    {
        var query = from permissions in EntityDbSet
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
                CreatedAtUtc = permissions.CreatedAtUtc
            };

        query = query.WhereDynamic(dto.Filters, reflectionCacheProvider);

        var count = await query.LongCountAsync(ct);

        query = dto.Sorters.Count == 0
            ? query.OrderByDescending(x => x.CreatedAtUtc)
            : query.OrderBy(dto.Sorters, reflectionCacheProvider);

        var data = await query
            .PaginateQueryable(dto.Page, dto.Size)
            .AsNoTracking()
            .ToListAsync(ct);

        return new PagedData<AuthorizablePermissionResponse>(data, count);
    }

    public async Task<ICollection<string>> GetRoleLabelsOfPermission(long permissionId,
        CancellationToken cancellationToken = default)
    {
        var query = from permissions in EntityDbSet
            where permissions.Id == permissionId
            join rolePermissions in _securityManagementDbContext.RolePermissions
                on permissions.Id equals rolePermissions.AuthorizablePermissionId
            join roles in _securityManagementDbContext.AuthorizableRoles
                on rolePermissions.AuthorizableRoleId equals roles.Id
            select roles.Label;

        return await query.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<PagedData<AuthorizablePermissionResponse>> GetAllPermissionsExceptAssigned(long roleId,
        DynamicQueryDto dto, IReflectionCacheProvider reflectionCacheProvider,
        CancellationToken ct = default)
    {
        var query = from permissions in EntityDbSet.Where(ap => !_securityManagementDbContext.RolePermissions
                .Where(rp => rp.AuthorizableRoleId == roleId)
                .Select(rp => rp.AuthorizablePermissionId)
                .Contains(ap.Id))
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
                CreatedAtUtc = permissions.CreatedAtUtc
            };

        query = query.WhereDynamic(dto.Filters, reflectionCacheProvider);

        var count = await query.LongCountAsync(ct);

        query = dto.Sorters.Count == 0
            ? query.OrderByDescending(x => x.CreatedAtUtc)
            : query.OrderBy(dto.Sorters, reflectionCacheProvider);

        var data = await query
            .PaginateQueryable(dto.Page, dto.Size)
            .AsNoTracking()
            .ToListAsync(ct);

        return new PagedData<AuthorizablePermissionResponse>(data, count);
    }


    public async Task<Dictionary<string, string[]>> GetPermissionsByRolesOfUser(
        ICollection<string> rolesLabels, CancellationToken cancellationToken = default)
    {
        if (rolesLabels.Count == 0)
        {
            return [];
        }

        var query = from roles in _securityManagementDbContext.AuthorizableRoles
            where rolesLabels.Contains(roles.Label)
            join rolePermissions in _securityManagementDbContext.RolePermissions
                on roles.Id equals rolePermissions.AuthorizableRoleId
            join permissions in _securityManagementDbContext.AuthorizablePermissions
                on rolePermissions.AuthorizablePermissionId equals permissions.Id
            group permissions.Label by roles.Label
            into roleGroup
            select new { role = roleGroup.Key, permissionLabels = roleGroup.ToArray() };

        return await query
            .AsNoTracking()
            .ToDictionaryAsync(x => x.role, x => x.permissionLabels, cancellationToken);
    }

    public async Task<IReadOnlyCollection<AuthCheckResponse>> CheckForExistingPermissions(
        IReadOnlyCollection<long> permissionIds, CancellationToken ct = default)
    {
        return await EntityDbSet
            .Where(e => permissionIds.Contains(e.Id))
            .Select(e => new AuthCheckResponse(e.Id, e.Label))
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyCollection<AuthCheckResponse>> CheckForExistingPermissions(
        IReadOnlyCollection<string> permissionLabels, CancellationToken ct = default)
    {
        return await EntityDbSet
            .Where(e => permissionLabels.Contains(e.Label))
            .Select(e => new AuthCheckResponse(e.Id, e.Label))
            .AsNoTracking()
            .ToListAsync(ct);
    }
}
