using MtslErp.Common.Application.Providers;
using MtslErp.Common.Application.Services;
using MtslErp.Common.Domain.CoreProviders;
using MtslErp.Common.Domain.DataTransferObjects.Request;
using MtslErp.Common.Domain.DataTransferObjects.Response;
using SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects;
using SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects.Enums;
using SecurityManagement.Application.Features.AuthFeatures.Interfaces;
using SecurityManagement.Domain.DataTransferObjects.Response;
using SecurityManagement.Domain.Entities;
using SharpOutcome.Helpers;
using SharpOutcome;

namespace SecurityManagement.Application.Features.AuthFeatures;

public class AuthRoleService : IAuthRoleService
{
    private readonly ISecurityManagementAppUnitOfWork _appUnitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IReflectionCacheProvider _reflectionCacheProvider;
    private readonly IKeyValueCache _keyValueCache;

    public AuthRoleService(ISecurityManagementAppUnitOfWork appUnitOfWork, IDateTimeProvider dateTimeProvider,
        IReflectionCacheProvider reflectionCacheProvider, IKeyValueCache keyValueCache)
    {
        _appUnitOfWork = appUnitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _reflectionCacheProvider = reflectionCacheProvider;
        _keyValueCache = keyValueCache;
    }

    public Task<PagedData<AuthorizableRole>> ReadAllAsync(DynamicQueryDto dto,
        CancellationToken ct = default)
    {
        return _appUnitOfWork.AuthorizableRoleRepository.GetPagedDataForDynamicQueryAsync(dto,
            (x => x.Id, false), _reflectionCacheProvider, ct);
    }

    public async Task ClearRolePermissionsCacheAsync(string roleLabel)
    {
        await _keyValueCache.RemoveAsync(roleLabel);
    }

    public Task<bool> IsRoleExistsAsync(string label, CancellationToken ct = default)
    {
        return _appUnitOfWork.AuthorizableRoleRepository.ExistsAsync(x => x.Label == label, ct);
    }

    public Task<bool> IsRoleExistsAsync(long id, CancellationToken ct = default)
    {
        return _appUnitOfWork.AuthorizableRoleRepository.ExistsAsync(x => x.Id == id, ct);
    }

    public Task<AuthorizableRole?> GetRoleByLabelAsync(string role, CancellationToken ct = default)
    {
        return _appUnitOfWork.AuthorizableRoleRepository.GetOneAsync(x => x.Label == role, ct);
    }

    public Task<AuthorizableRole?> GetRoleByIdAsync(long id, CancellationToken ct = default)
    {
        return _appUnitOfWork.AuthorizableRoleRepository.GetOneAsync(x => x.Id == id, ct);
    }

    public async Task<ValueOutcome<AuthorizableRole, AuthorizableBadOutcome>> CreateRoleAsync(string role)
    {
        if (await GetRoleByLabelAsync(role) is not null)
        {
            return AuthorizableBadOutcome.Conflict;
        }

        var entity = new AuthorizableRole { Label = role, CreatedAtUtc = _dateTimeProvider.CurrentUtcTime };

        await _appUnitOfWork.AuthorizableRoleRepository.CreateAsync(entity);
        await _appUnitOfWork.SaveAsync();
        return entity;
    }

    public async Task<ValueOutcome<AuthorizableRole, AuthorizableBadOutcome>> UpdateRoleAsync(
        long roleId, string updatedRole)
    {
        var entity = await GetRoleByIdAsync(roleId);

        if (entity is null)
        {
            return AuthorizableBadOutcome.NotFound;
        }

        entity.Label = updatedRole;
        entity.UpdatedAtUtc = _dateTimeProvider.CurrentUtcTime;
        await _appUnitOfWork.AuthorizableRoleRepository.UpdateAsync(entity);
        await _appUnitOfWork.SaveAsync();
        return entity;
    }

    public async Task<ValueOutcome<Successful, AuthorizableBadOutcome>> DeleteRoleAsync(long id)
    {
        var entity = await GetRoleByIdAsync(id);

        if (entity is null)
        {
            return AuthorizableBadOutcome.NotFound;
        }

        await _appUnitOfWork.AuthorizableRoleRepository.RemoveAsync(entity);
        await _appUnitOfWork.SaveAsync();
        return new Successful();
    }

    public async Task<ValueOutcome<string, PermissionsToRoleAssignmentBadOutcome>>
        AssignPermissionsToRoleAsync(long roleId, IReadOnlyCollection<long> permissionIds)
    {
        // missing permissions
        var existCheckResult = await _appUnitOfWork.AuthorizablePermissionRepository
            .CheckForExistingPermissions(permissionIds);

        if (existCheckResult.Count != permissionIds.Count)
        {
            var filteredData = permissionIds
                .Where(id => existCheckResult.All(r => r.Id != id))
                .ToList();

            return new PermissionsToRoleAssignmentBadOutcome
            {
                Reason = PermissionsToRoleAssignmentBadOutcomeReason.MissingPermissions,
                MissingPermissions = filteredData,
                MissingRoleId = default,
                ExistingRolePermissions = []
            };
        }

        // missing role
        var entity = await GetRoleByIdAsync(roleId);

        if (entity is null)
        {
            return new PermissionsToRoleAssignmentBadOutcome
            {
                Reason = PermissionsToRoleAssignmentBadOutcomeReason.MissingRole,
                MissingPermissions = [],
                MissingRoleId = roleId,
                ExistingRolePermissions = []
            };
        }

        // existing role-permissions
        var duplicateCheckResult = await _appUnitOfWork.RolePermissionRepository
            .CheckForDuplicates(entity.Id, permissionIds);

        if (duplicateCheckResult.Count != 0)
        {
            return new PermissionsToRoleAssignmentBadOutcome
            {
                Reason = PermissionsToRoleAssignmentBadOutcomeReason.RolePermissionsConflict,
                MissingPermissions = [],
                MissingRoleId = default,
                ExistingRolePermissions = duplicateCheckResult
            };
        }

        List<RolePermission> storage = [];

        storage.AddRange(permissionIds.Select(permission => new RolePermission
        {
            AuthorizableRoleId = entity.Id,
            AuthorizablePermissionId = permission,
            CreatedAtUtc = _dateTimeProvider.CurrentUtcTime
        }));

        await _appUnitOfWork.RolePermissionRepository.CreateManyAsync(storage);
        await _appUnitOfWork.SaveAsync();
        return entity.Label;
    }

    public Task<PagedData<AuthorizableRoleResponse>> GetAssignableRolesAsync(DynamicQueryDto dto,
        long userId, CancellationToken ct = default)
    {
        return _appUnitOfWork.AuthorizableRoleRepository.GetAllRolesExceptAssigned(userId, dto,
            _reflectionCacheProvider, ct);
    }

    public Task<PagedData<AuthorizablePermissionResponse>> GetAssignablePermissionsAsync(
        DynamicQueryDto dto, long roleId, CancellationToken ct = default)
    {
        return _appUnitOfWork.AuthorizablePermissionRepository.GetAllPermissionsExceptAssigned(roleId, dto,
            _reflectionCacheProvider, ct);
    }

    public async Task<PagedData<AuthorizableRoleResponse>> GetAllRolesOfUserAsync(long id,
        DynamicQueryDto dto, CancellationToken ct = default)
    {
        return await _appUnitOfWork.UserRoleRepository.GetAllRolesOfUser(id, dto,
            _reflectionCacheProvider, ct);
    }

    public async Task<PagedData<AuthorizablePermissionResponse>> GetAllPermissionsOfRoleAsync(long roleId,
        DynamicQueryDto dto, CancellationToken ct = default)
    {
        return await _appUnitOfWork.RolePermissionRepository.GetAllPermissionsOfRoleAsync(roleId, dto,
            _reflectionCacheProvider, ct);
    }

    public async Task<(string? RoleLabel, int Modified)> RemovePermissionsFromRoleAsync(long roleId,
        IReadOnlyCollection<long> permissionIds)
    {
        var entity = await _appUnitOfWork
            .AuthorizableRoleRepository
            .GetOneAsync(x => x.Id == roleId, enableTracking: false);

        if (entity is null)
        {
            return (default, 0);
        }

        var result = await _appUnitOfWork.RolePermissionRepository.BulkDelete(roleId, permissionIds);
        return (entity.Label, result);
    }
}
