namespace SecurityManagement.Application.Features.AuthFeatures.Enums;

public enum PasswordResetResult : byte
{
    UserNotFound = 1,
    ProfileNotConfirmed,
    SameAsOldPassword,
    InvalidToken,
    Ok
}
