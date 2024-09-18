namespace SecurityManagement.Application.Features.AuthFeatures;

public record UserDataResponse(
    long Id,
    SortedSet<string> Roles,
    SortedSet<string> Permissions);
