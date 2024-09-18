using MtslErp.Common.Domain.Enums;

namespace SecurityManagement.Application;

public static class SecurityManagementApplicationConstants
{
    public const string AccessTokenCookieKey = "ACCESS_TOKEN";
    public const string RefreshTokenCookieKey = "REFRESH_TOKEN";
    public const string RefreshTokenConsumer = "REFRESH_TOKEN_CONSUMER";
    public const string RefreshTokenClaims = "REFRESH_TOKEN_CLAIMS";
    public const string XsrfTokenCookieKey = "XSRF-TOKEN";
    public const string XsrfTokenHeaderKey = "X-XSRF-TOKEN";
    public const string RecaptchaResponseHeaderKey = "X-RECAPTCHA-RESPONSE";

    public static readonly Dictionary<MatchablePermissionGroup, HashSet<MatchablePermission>>
        MatchablePermissionCategoryMapper = new()
        {
            {
                MatchablePermissionGroup.FixedRole, [
                    MatchablePermission.SuperAdmin
                ]
            },
        };
}
