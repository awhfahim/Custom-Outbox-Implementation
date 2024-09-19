using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MtslErp.Common.Application.Providers;
using MtslErp.Common.Domain.CoreProviders;
using MtslErp.Common.Domain.DataTransferObjects.Request;
using MtslErp.Common.Domain.DataTransferObjects.Response;
using SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects;
using SecurityManagement.Application.Features.AuthFeatures.Interfaces;
using SecurityManagement.Application.Features.AuthFeatures.Outcomes;
using SecurityManagement.Application.Options;
using SecurityManagement.Application.Providers;
using SecurityManagement.Domain.DataTransferObjects.Response;
using SecurityManagement.Domain.Entities;
using SecurityManagement.Domain.Enums;
using SharpOutcome;
using SharpOutcome.Helpers;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MtslErp.Common.Domain.Entities;
using MtslErp.Common.Domain.Events;

namespace SecurityManagement.Application.Features.AuthFeatures;

public class UserService : IUserService
{
    private readonly ISecurityManagementAppUnitOfWork _appUnitOfWork;
    private readonly IReflectionCacheProvider _reflectionCacheProvider;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IAuthCryptographyService _authCryptographyService;
    private readonly IUserRoleService _userRoleService;
    private readonly IAuthPermissionService _authPermissionService;
    private readonly IJwtProvider _jwtProvider;
    private readonly JwtOptions _jwtOptions;


    public UserService(ISecurityManagementAppUnitOfWork appUnitOfWork, IDateTimeProvider dateTimeProvider,
        IAuthCryptographyService authCryptographyService, IReflectionCacheProvider reflectionCacheProvider,
        IUserRoleService userRoleService, IAuthPermissionService authPermissionService,
        IJwtProvider jwtProvider, IOptions<JwtOptions> jwtOptions)
    {
        _appUnitOfWork = appUnitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _authCryptographyService = authCryptographyService;
        _reflectionCacheProvider = reflectionCacheProvider;
        _userRoleService = userRoleService;
        _authPermissionService = authPermissionService;
        _jwtProvider = jwtProvider;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<ValueOutcome<User, SignUpBadOutcome>> SignUpAsync(UserSignupRequest dto)
    {
        var exists = await _appUnitOfWork.UserRepository.ExistsAsync(
            x => x.UserName == dto.UserName
        );

        if (exists)
        {
            return SignUpBadOutcome.AlreadyExists;
        }

        var entity = new User
        {
            FullName = dto.FullName,
            UserName = dto.UserName,
            Email = dto.Email,
            MaritalStatus = dto.MaritalStatus,
            Gender = dto.Gender,
            DateOfBirth = dto.DateOfBirth,
            PhoneNumber = dto.PhoneNumber,
            Status = UserStatus.Active,
            PasswordHash = await _authCryptographyService.HashPasswordAsync(dto.Password),
            CreatedAtUtc = _dateTimeProvider.CurrentUtcTime,
            Address = dto.Address
        };

        var userRegisteredEvent = new UserRegisteredEvent
        {
            UserName = dto.UserName,
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            DateOfBirth = dto.DateOfBirth,
            Address = dto.Address,
            ProfilePictureUri = entity.ProfilePictureUri
        };

        var payload = JsonSerializer.Serialize(userRegisteredEvent);

        var outboxMessage = new OutboxMessage()
        {
            Payload = payload,
            PayloadType = typeof(UserRegisteredEvent).AssemblyQualifiedName ?? string.Empty,
            CreatedOn = _dateTimeProvider.CurrentUtcTime,
            Status = true
        };

        await _appUnitOfWork.SecurityManagementOutboxRepository.CreateAsync(outboxMessage);
        await _appUnitOfWork.UserRepository.CreateAsync(entity);
        await _appUnitOfWork.SaveAsync();
        return entity;
    }

    public async Task<ValueOutcome<User, LoginBadOutcome>> LoginAsync(string userName, string password)
    {
        var entity = await _appUnitOfWork.UserRepository.GetOneAsync(
            x => x.UserName == userName, enableTracking: false
        );

        if (entity is null)
        {
            return LoginBadOutcome.UserNotFound;
        }

        var passwordMatched = await _authCryptographyService
            .VerifyPasswordAsync(password, entity.PasswordHash);

        if (passwordMatched is false)
        {
            return LoginBadOutcome.PasswordNotMatched;
        }

        return entity;
    }

    public async Task<SuccessfulLoginPayload> HandleSuccessfulLoginAsync(long userId, bool rememberMe)
    {
        var roles = await _userRoleService.GetRolesOfUser(userId);
        await _authPermissionService.UpdateCacheFromDatabaseAsync(roles.Select(x => x.RoleLabel).ToList());

        List<Claim> accessTokenClaims =
        [
            new(ClaimTypes.NameIdentifier, userId.ToString())
        ];

        accessTokenClaims.AddRange(roles.Select(elem => new Claim(ClaimTypes.Role, elem.RoleLabel)));

        var accessTokenData = _jwtProvider.GenerateJwt(accessTokenClaims,
            TimeSpan.FromMinutes(_jwtOptions.AccessTokenExpiryMinutes), _jwtOptions.Secret);


        List<Claim> refreshTokenClaims =
        [
            new(SecurityManagementApplicationConstants.RefreshTokenConsumer, userId.ToString())
        ];

        refreshTokenClaims.AddRange(roles.Select(elem =>
            new Claim(SecurityManagementApplicationConstants.RefreshTokenClaims, elem.RoleLabel)));


        var refreshTokenData = _jwtProvider.GenerateJwt(refreshTokenClaims,
            TimeSpan.FromMinutes(_jwtOptions.RefreshTokenExpiryMinutes), _jwtOptions.Secret);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, Expires = rememberMe ? accessTokenData.duration : null
        };

        return new SuccessfulLoginPayload(accessTokenData, refreshTokenData, cookieOptions);
    }

    public async Task<ValueOutcome<SuccessfulRefreshTokenPayload, Failed>> ValidateRefreshToken(
        HttpContext context)
    {
        var plainToken =
            context.Request.Cookies[SecurityManagementApplicationConstants.RefreshTokenCookieKey];
        if (plainToken is null)
        {
            return new Failed();
        }

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret))
        };

        var tokenHandler = new JsonWebTokenHandler();
        try
        {
            var result = await tokenHandler.ValidateTokenAsync(plainToken, tokenValidationParameters);

            if (result.IsValid is false)
            {
                return new Failed();
            }

            var id = result.ClaimsIdentity
                .FindFirst(x => x.Type == SecurityManagementApplicationConstants.RefreshTokenConsumer)
                ?.Value;

            var roles = result.ClaimsIdentity
                .FindAll(x => x.Type == SecurityManagementApplicationConstants.RefreshTokenClaims)
                .Select(x => x.Value);

            if (id is null)
            {
                return new Failed();
            }

            return new SuccessfulRefreshTokenPayload(id, roles.ToList());
        }
        catch
        {
            return new Failed();
        }
    }

    public SuccessfulAccessTokenByRefreshTokenPayload IssueAccessTokenByRefreshToken(
        SuccessfulRefreshTokenPayload data, bool rememberMe)
    {
        List<Claim> accessTokenClaims =
        [
            new(ClaimTypes.NameIdentifier, data.Id)
        ];

        accessTokenClaims.AddRange(data.Claims.Select(elem => new Claim(ClaimTypes.Role, elem)));

        var accessTokenData = _jwtProvider.GenerateJwt(accessTokenClaims,
            TimeSpan.FromMinutes(_jwtOptions.AccessTokenExpiryMinutes), _jwtOptions.Secret);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, Expires = rememberMe ? accessTokenData.duration : null
        };

        return new SuccessfulAccessTokenByRefreshTokenPayload(accessTokenData.token,
            accessTokenData.duration, cookieOptions);
    }

    public Task<User?> ReadProfileAsync(long id)
    {
        return _appUnitOfWork.UserRepository.GetOneAsync(x => x.Id == id, enableTracking: false);
    }

    public async Task<PagedData<UserResponse>> ReadAllAsync(DynamicQueryDto dto)
    {
        return await _appUnitOfWork.UserRepository.GetPagedDataForDynamicQueryAsync(dto,
            _reflectionCacheProvider);
    }
}
