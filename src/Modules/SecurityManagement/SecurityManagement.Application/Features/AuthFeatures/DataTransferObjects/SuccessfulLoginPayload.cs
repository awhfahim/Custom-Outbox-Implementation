using Microsoft.AspNetCore.Http;

namespace SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects;

public record SuccessfulLoginPayload(
    (string token, DateTime duration) AccessTokenData,
    (string token, DateTime duration) RefreshTokenData,
    CookieOptions CookieOptions);
