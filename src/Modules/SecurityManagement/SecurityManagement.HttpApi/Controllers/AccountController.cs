using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MtslErp.Common.Domain.DataTransferObjects.Request;
using MtslErp.Common.HttpApi.ActionFilters;
using MtslErp.Common.HttpApi.Controllers;
using MtslErp.Common.HttpApi.Others;
using SecurityManagement.Application;
using SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects;
using SecurityManagement.Application.Features.AuthFeatures.Interfaces;
using SecurityManagement.Application.Features.AuthFeatures.Outcomes;
using SecurityManagement.Application.Options;

namespace SecurityManagement.HttpApi.Controllers;

[Route("api/v1/[controller]")]
public class AccountController : JsonApiControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly GoogleRecaptchaOptions _googleRecaptchaOptions;
    private readonly IUserService _userService;
    private readonly IServiceProvider _serviceProvider;

    public AccountController(IHttpClientFactory httpClientFactory,
        IOptions<GoogleRecaptchaOptions> googleRecaptchaOptions, IUserService userService, IServiceProvider serviceProvider)
    {
        _httpClientFactory = httpClientFactory;
        _userService = userService;
        _serviceProvider = serviceProvider;
        _googleRecaptchaOptions = googleRecaptchaOptions.Value;
    }

    [HttpPost("query")]
    public async Task<IActionResult> GetAll([FromBody] DynamicQueryDto dto)
    {
        var data = await _userService.ReadAllAsync(dto);
        return ControllerContext.MakeDynamicQueryResponse(data.Payload, data.TotalCount, dto.Size);
    }

    [HttpPost("registration")]
    //[ValidateAngularXsrfToken]
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

        result.TryPickGoodOutcome(out var id);

        return ControllerContext.MakeResponse(StatusCodes.Status201Created, id);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    //[ValidateAngularXsrfToken]
    [ValidationActionFilter<UserLoginRequest>]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(UserLoginRequest dto,
        [FromHeader(Name = SecurityManagementApplicationConstants.RecaptchaResponseHeaderKey)]
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

            HttpContext.Response.Cookies.Append(SecurityManagementApplicationConstants.AccessTokenCookieKey,
                tokenData.AccessTokenData.token, tokenData.CookieOptions);

            HttpContext.Response.Cookies.Append(SecurityManagementApplicationConstants.RefreshTokenCookieKey,
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
    //[ValidateAngularXsrfToken]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Logout()
    {
        HttpContext.Response.Cookies.Delete(SecurityManagementApplicationConstants.AccessTokenCookieKey);
        HttpContext.Response.Cookies.Delete(SecurityManagementApplicationConstants.XsrfTokenCookieKey);
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
