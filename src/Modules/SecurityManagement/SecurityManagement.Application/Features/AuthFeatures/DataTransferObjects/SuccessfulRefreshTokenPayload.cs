namespace SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects;

public record SuccessfulRefreshTokenPayload(string Id, IList<string> Claims);
