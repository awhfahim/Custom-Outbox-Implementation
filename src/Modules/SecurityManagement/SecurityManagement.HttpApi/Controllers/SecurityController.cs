using ErpSoftware.Application.Features.AuthFeatures.Interfaces;
using ErpSoftware.Application.Features.AuthFeatures;
using Microsoft.AspNetCore.Authorization;
using ErpSoftware.HttpApi.ActionFilters;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using ErpSoftware.Application;
using System.Security.Claims;

namespace ErpSoftware.HttpApi.Controllers.Auth;

[Route("api/v1/[controller]")]
public class SecurityController : JsonApiControllerBase
{
    [HttpPost("check-token")]
    [Authorize]
    [ValidateAngularXsrfToken]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CheckAccessToken(
        [FromServices] IAuthPermissionService authPermissionService)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
        {
            return Unauthorized();
        }

        if (long.TryParse(userId, out var parsedUserId) is false)
        {
            return BadRequest();
        }

        var roles = User.FindAll(c => c.Type == ClaimTypes.Role).Select(x => x.Value).ToList();
        var cacheResult = await authPermissionService.GetPermissionsByRolesFromCacheAsync(roles);

        var response = new UserDataResponse(Id: parsedUserId, Roles: new SortedSet<string>(roles),
            Permissions: new SortedSet<string>(cacheResult.Values.SelectMany(v => v)));

        return Ok(response);
    }

    [HttpPost("refresh-access-token")]
    [Authorize]
    [ValidateAngularXsrfToken]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshAccessToken([FromServices] IUserService userService,
        [FromBody] bool rememberMe)
    {
        var result = await userService.ValidateRefreshToken(HttpContext);

        if (result.TryPickGoodOutcome(out var data))
        {
            var accessTokenData = userService.IssueAccessTokenByRefreshToken(data, rememberMe);

            HttpContext.Response.Cookies.Append(ApplicationConstants.AccessTokenCookieKey,
                accessTokenData.Token, accessTokenData.CookieOptions);

            return Ok();
        }

        return Unauthorized();
    }

    [HttpPost("load-xsrf-token")]
    public IActionResult LoadXsrf([FromServices] IAntiforgery antiForgery)
    {
        var token = antiForgery.GetAndStoreTokens(HttpContext);
        HttpContext.Response.Headers.Append(ApplicationConstants.XsrfTokenHeaderKey, token.RequestToken);
        return Ok();
    }
}
