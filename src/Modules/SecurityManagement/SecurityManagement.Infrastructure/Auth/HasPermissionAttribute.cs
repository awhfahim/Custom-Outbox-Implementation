using Microsoft.AspNetCore.Authorization;
using MtslErp.Common.Domain.Enums;

namespace SecurityManagement.Infrastructure.Auth;

public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(MatchablePermission permission)
        : base(policy: permission.ToString())
    {
    }
}
