using Microsoft.AspNetCore.Http;

namespace SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects;

public record SuccessfulAccessTokenByRefreshTokenPayload(string Token, DateTime Duration,
    CookieOptions CookieOptions);
