namespace SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects.Enums;

public enum RolesToUserAssignmentBadOutcomeReason : byte
{
    UserNotFound = 1,
    UserRolesConflict,
    MissingRoles
}
