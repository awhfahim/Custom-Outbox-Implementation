using MtslErp.Common.Application.Providers;
using SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects;
using SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects.Enums;
using SecurityManagement.Application.Features.AuthFeatures.Interfaces;
using SecurityManagement.Domain.DataTransferObjects.Response;
using SecurityManagement.Domain.Entities;
using SharpOutcome;
using SharpOutcome.Helpers;

namespace SecurityManagement.Application.Features.AuthFeatures;

public class UserRoleService : IUserRoleService
{
    private readonly ISecurityManagementAppUnitOfWork _appUnitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UserRoleService(ISecurityManagementAppUnitOfWork appUnitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _appUnitOfWork = appUnitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ValueOutcome<Successful, RolesToUserAssignmentBadOutcome>> AssignRolesToUser(
        long userId, IReadOnlyCollection<long> roleIds)
    {
        // missing user
        var entity = await _appUnitOfWork
            .UserRepository
            .GetOneAsync(condition: x => x.Id == userId, enableTracking: false);

        if (entity is null)
        {
            return new RolesToUserAssignmentBadOutcome
            {
                Reason = RolesToUserAssignmentBadOutcomeReason.UserNotFound,
                MissingUserId = userId,
                ExistingUserRoles = [],
                MissingRoles = []
            };
        }

        // missing roles
        var existCheckResult = await _appUnitOfWork.AuthorizableRoleRepository
            .CheckForExistingRoles(roleIds);

        if (existCheckResult.Count != roleIds.Count)
        {
            var filteredData = roleIds
                .Where(id => existCheckResult.All(r => r.Id != id))
                .ToList();

            return new RolesToUserAssignmentBadOutcome
            {
                Reason = RolesToUserAssignmentBadOutcomeReason.MissingRoles,
                MissingUserId = null,
                ExistingUserRoles = [],
                MissingRoles = filteredData
            };
        }

        // existing user roles
        var duplicateCheckResult = await _appUnitOfWork
            .UserRoleRepository
            .CheckForDuplicates(userId, roleIds);

        if (duplicateCheckResult.Count != 0)
        {
            return new RolesToUserAssignmentBadOutcome
            {
                Reason = RolesToUserAssignmentBadOutcomeReason.UserRolesConflict,
                MissingUserId = null,
                ExistingUserRoles = duplicateCheckResult,
                MissingRoles = []
            };
        }

        List<UserRole> storage = [];

        storage.AddRange(roleIds.Select(id => new UserRole
        {
            UserId = entity.Id, AuthorizableRoleId = id, CreatedAtUtc = _dateTimeProvider.CurrentUtcTime
        }));

        await _appUnitOfWork.UserRoleRepository.CreateManyAsync(storage);
        await _appUnitOfWork.SaveAsync();
        return new Successful();
    }

    public Task<bool> RemoveRolesFromUser(long userId,
        IReadOnlyCollection<long> roleIds)
    {
        return _appUnitOfWork.UserRoleRepository.BulkDelete(userId, roleIds);
    }

    public Task<ICollection<AuthorizableRoleResponse>> GetRolesOfUser(long userId,
        CancellationToken ct = default)
    {
        return _appUnitOfWork.UserRoleRepository.GetAllRolesOfUser(userId, ct);
    }
}
