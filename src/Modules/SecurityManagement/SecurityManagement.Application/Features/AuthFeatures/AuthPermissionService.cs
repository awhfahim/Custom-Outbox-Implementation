using MtslErp.Common.Application.Providers;
using MtslErp.Common.Application.Services;
using MtslErp.Common.Domain.CoreProviders;
using MtslErp.Common.Domain.DataTransferObjects.Request;
using MtslErp.Common.Domain.DataTransferObjects.Response;
using SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects;
using SecurityManagement.Application.Features.AuthFeatures.Interfaces;
using SecurityManagement.Domain.DataTransferObjects.Response;
using SecurityManagement.Domain.Entities;
using SharpOutcome;
using SharpOutcome.Helpers;

namespace SecurityManagement.Application.Features.AuthFeatures;

public class AuthPermissionService : IAuthPermissionService
{
    private readonly IReflectionCacheProvider _reflectionCacheProvider;
    private readonly ISecurityManagementAppUnitOfWork _appUnitOfWork;
    private readonly IKeyValueCache _keyValueCache;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IAuthRoleService _authRoleService;

    public AuthPermissionService(ISecurityManagementAppUnitOfWork appUnitOfWork,
        IDateTimeProvider dateTimeProvider, IKeyValueCache keyValueCache,
        IReflectionCacheProvider reflectionCacheProvider,
        IAuthRoleService authRoleService)
    {
        _appUnitOfWork = appUnitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _keyValueCache = keyValueCache;
        _reflectionCacheProvider = reflectionCacheProvider;
        _authRoleService = authRoleService;
    }

    public async Task<PagedData<AuthorizablePermissionResponse>> ReadAllAsync(DynamicQueryDto dto,
        CancellationToken ct = default)
    {
        return await _appUnitOfWork.AuthorizablePermissionRepository.GetAllAsync(dto,
            _reflectionCacheProvider, ct);
    }

    public async Task<Dictionary<string, string[]>> GetPermissionsByRolesFromCacheAsync(
        ICollection<string> roles)
    {
        Dictionary<string, string[]> storage = [];

        foreach (var roleLabel in roles)
        {
            var data = await _keyValueCache.GetAsync<string[]?>(roleLabel);
            if (data is null)
            {
                continue;
            }

            storage.TryAdd(roleLabel, data);
        }

        return storage;
    }

    public async Task UpdateCacheFromDatabaseAsync(ICollection<string> roles, CancellationToken ct = default)
    {
        await GetPermissionsByRolesAsync(roles, cacheResult: true, ct);
    }

    public async Task<Dictionary<string, string[]>> GetPermissionsByRolesAsync(ICollection<string> roles,
        bool cacheResult, CancellationToken ct = default)
    {
        var permissions = await _appUnitOfWork
            .AuthorizablePermissionRepository
            .GetPermissionsByRolesOfUser(roles, ct);

        if (cacheResult is false)
        {
            return permissions;
        }

        foreach (var elem in permissions)
        {
            await _keyValueCache.CreateAsync(elem.Key, elem.Value, TimeSpan.FromHours(8),
                TimeSpan.FromHours(8));
        }

        return permissions;
    }

    public Task<AuthorizablePermission?> GetPermissionByLabelAsync(string permission,
        CancellationToken ct = default)
    {
        return _appUnitOfWork.AuthorizablePermissionRepository.GetOneAsync(x => x.Label == permission, ct);
    }

    public Task<AuthorizablePermission?> GetPermissionByIdAsync(long id, CancellationToken ct = default)
    {
        return _appUnitOfWork.AuthorizablePermissionRepository.GetOneAsync(x => x.Id == id, ct);
    }

    public Task<ICollection<AuthorizablePermission>> GetPermissionsAsync(int page, int limit,
        CancellationToken ct = default)
    {
        return _appUnitOfWork.AuthorizablePermissionRepository.GetAllAsync(page, limit, updateable: false,
            orderBy: x => x.Label, cancellationToken: ct);
    }

    public async Task<ValueOutcome<AuthorizablePermission, AuthorizableBadOutcome>> CreatePermissionAsync(
        PermissionCreateOrUpdateModel dto, CancellationToken ct = default)
    {
        if (await GetPermissionByLabelAsync(dto.Label, ct) is not null)
        {
            return AuthorizableBadOutcome.Conflict;
        }

        var entity = new AuthorizablePermission
        {
            Label = dto.Label, GroupId = dto.GroupId, CreatedAtUtc = _dateTimeProvider.CurrentUtcTime
        };

        await _appUnitOfWork.AuthorizablePermissionRepository.CreateAsync(entity);
        await _appUnitOfWork.SaveAsync();
        return entity;
    }

    public async Task<ValueOutcome<ICollection<AuthorizablePermission>, AuthorizableBadOutcome>>
        CreatePermissionAsync(ICollection<PermissionCreateOrUpdateModel> dto, CancellationToken ct = default)
    {
        var existing = await _appUnitOfWork
            .AuthorizablePermissionRepository
            .CheckForExistingPermissions(dto.Select(x => x.Label).ToList(), ct);

        if (existing.Count != 0)
        {
            return AuthorizableBadOutcome.Conflict;
        }

        var storage = dto.Select(x => new AuthorizablePermission
        {
            Label = x.Label, GroupId = x.GroupId, CreatedAtUtc = _dateTimeProvider.CurrentUtcTime
        }).ToList();

        await _appUnitOfWork.AuthorizablePermissionRepository.CreateManyAsync(storage);
        await _appUnitOfWork.SaveAsync();
        return storage;
    }

    public async Task<ValueOutcome<AuthorizablePermission, AuthorizableBadOutcome>> UpdatePermissionAsync(
        long id, PermissionCreateOrUpdateModel dto, CancellationToken ct = default)
    {
        var entity = await GetPermissionByIdAsync(id, ct);

        if (entity is null)
        {
            return AuthorizableBadOutcome.NotFound;
        }

        entity.Label = dto.Label;
        entity.GroupId = dto.GroupId;
        entity.UpdatedAtUtc = _dateTimeProvider.CurrentUtcTime;

        await _appUnitOfWork.AuthorizablePermissionRepository.UpdateAsync(entity);
        await _appUnitOfWork.SaveAsync();
        return entity;
    }

    public async Task<ValueOutcome<Successful, AuthorizableBadOutcome>>
        DeletePermissionAndUpdateCacheAsync(long id, CancellationToken ct = default)
    {
        var entity = await GetPermissionByIdAsync(id, ct);

        if (entity is null)
        {
            return AuthorizableBadOutcome.NotFound;
        }

        var labels = await _appUnitOfWork
            .AuthorizablePermissionRepository
            .GetRoleLabelsOfPermission(entity.Id, ct);

        await _appUnitOfWork.AuthorizablePermissionRepository.RemoveAsync(entity);
        await _appUnitOfWork.SaveAsync();

        foreach (var label in labels)
        {
            await _authRoleService.ClearRolePermissionsCacheAsync(label);
        }

        return new Successful();
    }

    public async Task<ValueOutcome<Successful, AuthorizableBadOutcome>> DeletePermissionAsync(long id,
        CancellationToken ct = default)
    {
        var entity = await GetPermissionByIdAsync(id, ct);

        if (entity is null)
        {
            return AuthorizableBadOutcome.NotFound;
        }

        await _appUnitOfWork.AuthorizablePermissionRepository.RemoveAsync(entity);
        await _appUnitOfWork.SaveAsync();
        return new Successful();
    }

    public Task<AuthorizablePermissionGroup?> GetPermissionGroupByLabelAsync(string permission,
        CancellationToken ct = default)
    {
        return _appUnitOfWork.AuthorizablePermissionGroupRepository.GetOneAsync(x => x.Label == permission,
            ct);
    }

    public Task<AuthorizablePermissionGroup?> GetPermissionGroupByIdAsync(long id,
        CancellationToken ct = default)
    {
        return _appUnitOfWork.AuthorizablePermissionGroupRepository.GetOneAsync(x => x.Id == id, ct);
    }

    public async Task<ValueOutcome<AuthorizablePermissionGroup, AuthorizableBadOutcome>>
        CreatePermissionGroupAsync(PermissionGroupCreateOrUpdateModel dto,
            CancellationToken ct = default)
    {
        if (await GetPermissionGroupByLabelAsync(dto.Label, ct) is not null)
        {
            return AuthorizableBadOutcome.Conflict;
        }

        var entity = new AuthorizablePermissionGroup
        {
            Label = dto.Label, CreatedAtUtc = _dateTimeProvider.CurrentUtcTime
        };

        await _appUnitOfWork.AuthorizablePermissionGroupRepository.CreateAsync(entity);
        await _appUnitOfWork.SaveAsync();
        return entity;
    }

    public async Task<ValueOutcome<AuthorizablePermissionGroup, AuthorizableBadOutcome>>
        UpdatePermissionGroupAsync(long id, PermissionGroupCreateOrUpdateModel dto,
            CancellationToken ct = default)
    {
        var entity = await GetPermissionGroupByIdAsync(id, ct);

        if (entity is null)
        {
            return AuthorizableBadOutcome.NotFound;
        }

        entity.Label = dto.Label;
        entity.UpdatedAtUtc = _dateTimeProvider.CurrentUtcTime;

        await _appUnitOfWork.AuthorizablePermissionGroupRepository.UpdateAsync(entity);
        await _appUnitOfWork.SaveAsync();
        return entity;
    }

    public async Task<ValueOutcome<Successful, AuthorizableBadOutcome>> DeletePermissionGroupAsync(long id,
        CancellationToken ct = default)
    {
        var entity = await GetPermissionGroupByIdAsync(id, ct);

        if (entity is null)
        {
            return AuthorizableBadOutcome.NotFound;
        }

        await _appUnitOfWork.AuthorizablePermissionGroupRepository.RemoveAsync(entity);
        await _appUnitOfWork.SaveAsync();
        return new Successful();
    }
}
