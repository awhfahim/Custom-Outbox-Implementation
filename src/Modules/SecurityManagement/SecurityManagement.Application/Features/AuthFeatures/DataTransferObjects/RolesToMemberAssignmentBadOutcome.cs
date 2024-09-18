using SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects.Enums;
using SecurityManagement.Domain.DataTransferObjects.Response;

namespace SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects;

public record RolesToUserAssignmentBadOutcome
{
    public required RolesToUserAssignmentBadOutcomeReason Reason { get; init; }
    public required IReadOnlyCollection<AuthCheckResponse> ExistingUserRoles { get; init; }
    public required IReadOnlyCollection<long> MissingRoles { get; init; }
    public required long? MissingUserId { get; init; }
}
