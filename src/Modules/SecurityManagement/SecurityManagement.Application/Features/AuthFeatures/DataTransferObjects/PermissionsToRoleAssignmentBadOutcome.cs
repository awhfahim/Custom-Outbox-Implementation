using SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects.Enums;
using SecurityManagement.Domain.DataTransferObjects.Response;

namespace SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects;

public record PermissionsToRoleAssignmentBadOutcome
{
    public required PermissionsToRoleAssignmentBadOutcomeReason Reason { get; init; }

    public required IReadOnlyCollection<DuplicateRolePermissionCheckResponse> ExistingRolePermissions
    {
        get;
        init;
    }

    public required IReadOnlyCollection<long> MissingPermissions { get; init; }
    public required long? MissingRoleId { get; init; }
}
