using SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects;
using SecurityManagement.Domain.DataTransferObjects.Response;
using SharpOutcome;
using SharpOutcome.Helpers;

namespace SecurityManagement.Application.Features.AuthFeatures.Interfaces;

public interface IUserRoleService
{
    Task<ValueOutcome<Successful, RolesToUserAssignmentBadOutcome>> AssignRolesToUser(long userId,
        IReadOnlyCollection<long> roleIds);

    Task<bool> RemoveRolesFromUser(long userId, IReadOnlyCollection<long> roleIds);
    Task<ICollection<AuthorizableRoleResponse>> GetRolesOfUser(long userId, CancellationToken ct = default);
}
