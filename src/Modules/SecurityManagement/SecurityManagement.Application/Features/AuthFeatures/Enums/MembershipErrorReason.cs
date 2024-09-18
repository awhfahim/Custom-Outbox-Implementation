namespace SecurityManagement.Application.Features.AuthFeatures.Enums;

public enum UsershipErrorReason : byte
{
    DuplicateEmail = 1,
    NotFound,
    AlreadyAddedManually,
    Others,
    Unknown
}
