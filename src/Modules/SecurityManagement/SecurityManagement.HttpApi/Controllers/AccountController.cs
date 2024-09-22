using System.Security.Claims;
using ErpSoftware.Application;
using ErpSoftware.Application.Common.Options;
using ErpSoftware.Application.Features.AuthFeatures;
using ErpSoftware.Application.Features.AuthFeatures.DataTransferObjects;
using ErpSoftware.Application.Features.AuthFeatures.Interfaces;
using ErpSoftware.Application.Features.AuthFeatures.Outcomes;
using ErpSoftware.Domain.DataTransferObjects.Request;
using ErpSoftware.HttpApi.ActionFilters;
using ErpSoftware.HttpApi.Others;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ErpSoftware.HttpApi.Controllers.Auth;

[Route("api/v1/[controller]")]
public class AccountController : JsonApiControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly GoogleRecaptchaOptions _googleRecaptchaOptions;
    private readonly IUserService _userService;

    public AccountController(IHttpClientFactory httpClientFactory,
        IOptions<GoogleRecaptchaOptions> googleRecaptchaOptions, IUserService userService)
    {
        _httpClientFactory = httpClientFactory;
        _userService = userService;
        _googleRecaptchaOptions = googleRecaptchaOptions.Value;
    }

    [HttpPost("query")]
    public async Task<IActionResult> GetAll([FromBody] DynamicQueryDto dto)
    {
        var data = await _userService.ReadAllAsync(dto, HttpContext.RequestAborted);
        return ControllerContext.MakeDynamicQueryResponse(data.Payload, data.TotalCount, dto.Size);
    }

    [HttpPost("registration")]
    [ValidateAngularXsrfToken]
    [ValidationActionFilter<UserSignupRequest>]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateUser([FromBody] UserSignupRequest dto)
    {
        var result = await _userService.SignUpAsync(dto);

        if (result.TryPickBadOutcome(out var error))
        {
            return error switch
            {
                SignUpBadOutcome.AlreadyExists => Conflict("User already exists"),
                _ => BadRequest()
            };
        }

        return ControllerContext.MakeResponse(StatusCodes.Status201Created);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ValidateAngularXsrfToken]
    [ValidationActionFilter<UserLoginRequest>]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(UserLoginRequest dto,
        [FromHeader(Name = ApplicationConstants.RecaptchaResponseHeaderKey)]
        string recaptchaResponseCode, [FromServices] IOptions<JwtOptions> jwtOptions
    )
    {
        if (await VerifyRecaptchaV3Token(recaptchaResponseCode) is false)
        {
            return Unauthorized("There was an issue with verifying recaptcha token");
        }

        var result = await _userService.LoginAsync(dto.UserName, dto.Password);


        if (result.TryPickGoodOutcome(out var entity, out var error))
        {
            var tokenData = await _userService.HandleSuccessfulLoginAsync(entity.Id, dto.RememberMe);

            HttpContext.Response.Cookies.Append(ApplicationConstants.AccessTokenCookieKey,
                tokenData.AccessTokenData.token, tokenData.CookieOptions);

            HttpContext.Response.Cookies.Append(ApplicationConstants.RefreshTokenCookieKey,
                tokenData.RefreshTokenData.token, tokenData.CookieOptions);

            return Ok(jwtOptions.Value.AccessTokenExpiryMinutes);
        }

        return error switch
        {
            LoginBadOutcome.UserNotFound => Unauthorized("Invalid login attempt"),
            LoginBadOutcome.PasswordNotMatched => Unauthorized("Invalid login attempt"),
            LoginBadOutcome.Banned => Forbid("Account banned"),
            _ => StatusCode(StatusCodes.Status400BadRequest)
        };
    }

    [HttpPost("logout")]
    [Authorize]
    [ValidateAngularXsrfToken]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Logout()
    {
        HttpContext.Response.Cookies.Delete(ApplicationConstants.AccessTokenCookieKey);
        HttpContext.Response.Cookies.Delete(ApplicationConstants.XsrfTokenCookieKey);
        return Ok();
    }

    private async Task<bool> VerifyRecaptchaV3Token(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }

        try
        {
            using var httpClient = _httpClientFactory.CreateClient(GoogleRecaptchaOptions.SectionName);
            var response = await httpClient.PostAsync(
                $"?secret={_googleRecaptchaOptions.SecretKey}&response={token}", null
            );
            return response.IsSuccessStatusCode;
        }
        catch (TaskCanceledException)
        {
            return false;
        }
    }
}
