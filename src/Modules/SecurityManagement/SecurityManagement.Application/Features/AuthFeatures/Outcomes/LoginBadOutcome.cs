namespace SecurityManagement.Application.Features.AuthFeatures.Outcomes;

public enum LoginBadOutcome : byte
{
    UserNotFound = 1,
    PasswordNotMatched,
    Banned
}
