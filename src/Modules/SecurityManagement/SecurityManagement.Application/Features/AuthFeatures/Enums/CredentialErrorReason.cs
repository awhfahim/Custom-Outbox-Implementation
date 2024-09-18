namespace SecurityManagement.Application.Features.AuthFeatures.Enums;

public enum CredentialErrorReason : byte
{
    UserNotFound = 1,
    PasswordNotMatched,
    ProfileAlreadyConfirmed
}
