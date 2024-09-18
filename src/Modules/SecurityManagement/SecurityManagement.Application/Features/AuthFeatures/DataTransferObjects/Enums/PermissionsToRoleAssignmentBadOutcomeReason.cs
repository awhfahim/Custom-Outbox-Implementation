namespace SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects.Enums;

public enum PermissionsToRoleAssignmentBadOutcomeReason : byte
{
    MissingRole = 1,
    RolePermissionsConflict,
    MissingPermissions
}
