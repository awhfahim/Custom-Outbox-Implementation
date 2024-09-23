using Microsoft.AspNetCore.Http;
using MtslErp.Common.Domain.DataTransferObjects.Request;
using MtslErp.Common.Domain.DataTransferObjects.Response;
using SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects;
using SecurityManagement.Application.Features.AuthFeatures.Outcomes;
using SecurityManagement.Domain.DataTransferObjects.Response;
using SecurityManagement.Domain.Entities;
using SharpOutcome;
using SharpOutcome.Helpers;

namespace SecurityManagement.Application.Features.AuthFeatures.Interfaces;

public interface IUserService
{
    Task<ValueOutcome<long, SignUpBadOutcome>> SignUpAsync(UserSignupRequest dto);
    Task<ValueOutcome<User, LoginBadOutcome>> LoginAsync(string userName, string password);
    Task<User?> ReadProfileAsync(long id);
    Task<SuccessfulLoginPayload> HandleSuccessfulLoginAsync(long userId, bool rememberMe);

    Task<PagedData<UserResponse>> ReadAllAsync(DynamicQueryDto dto);

    Task<ValueOutcome<SuccessfulRefreshTokenPayload, Failed>> ValidateRefreshToken(HttpContext context);

    SuccessfulAccessTokenByRefreshTokenPayload IssueAccessTokenByRefreshToken(
        SuccessfulRefreshTokenPayload data, bool rememberMe);
    // UpdateProfileAsync
}
