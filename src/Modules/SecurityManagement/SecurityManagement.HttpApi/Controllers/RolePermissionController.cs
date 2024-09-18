using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MtslErp.Common.Domain.DataTransferObjects.Request;
using MtslErp.Common.HttpApi.Controllers;
using MtslErp.Common.HttpApi.Others;
using SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects.Enums;
using SecurityManagement.Application.Features.AuthFeatures.Interfaces;

namespace SecurityManagement.HttpApi.Controllers;

[Route("api/v1/[controller]")]
public class RolePermissionController : JsonApiControllerBase
{
    private readonly IAuthRoleService _authRoleService;
    private readonly IAuthPermissionService _authPermissionService;

    public RolePermissionController(IAuthRoleService authRoleService,
        IAuthPermissionService authPermissionService)
    {
        _authRoleService = authRoleService;
        _authPermissionService = authPermissionService;
    }

    [HttpPost("{id:required:long}/assignable/query")]
    public async Task<IActionResult> GetAssignablePermissionsOfRole([FromBody] DynamicQueryDto dto,
        [FromRoute] long id)
    {
        var data = await _authRoleService.GetAssignablePermissionsAsync(dto, id, HttpContext.RequestAborted);
        return ControllerContext.MakeDynamicQueryResponse(data.Payload, data.TotalCount, dto.Size);
    }

    [HttpPost("{id:required:long}/removable/query")]
    public async Task<IActionResult> GetAllAssignedPermissionsOfRole([FromRoute] long id,
        [FromBody] DynamicQueryDto dto)
    {
        var data = await _authRoleService.GetAllPermissionsOfRoleAsync(id, dto, HttpContext.RequestAborted);
        return ControllerContext.MakeDynamicQueryResponse(data.Payload, data.TotalCount, dto.Size);
    }

    [HttpPost("{id:required:long}/assign")]
    public async Task<IActionResult> AssignPermissionsToRole([FromRoute] long id,
        [FromBody] List<long> permissionIds)
    {
        var result = await _authRoleService.AssignPermissionsToRoleAsync(id, permissionIds);

        if (result.TryPickBadOutcome(out var error, out var roleLabel))
        {
            if (error.Reason is PermissionsToRoleAssignmentBadOutcomeReason.MissingRole &&
                error.MissingRoleId is not null)
            {
                return NotFound(new { error.Reason, payload = error.MissingRoleId });
            }

            if (error.MissingPermissions.Count != 0)
            {
                return NotFound(new { error.Reason, payload = error.MissingPermissions });
            }

            if (error.ExistingRolePermissions.Count != 0)
            {
                return Conflict(new { error.Reason, payload = error.ExistingRolePermissions });
            }
        }

        else
        {
            await _authPermissionService.UpdateCacheFromDatabaseAsync([roleLabel]);
            return ControllerContext.MakeResponse(StatusCodes.Status201Created);
        }

        return BadRequest();
    }

    [HttpPost("{id:required:long}/bulk-remove")]
    public async Task<IActionResult> RemovePermissionsFromRole([FromRoute] long id,
        [FromBody] List<long> permissionIds)
    {
        if (permissionIds.Count == 0)
        {
            return ControllerContext.MakeResponse(StatusCodes.Status304NotModified);
        }

        var result = await _authRoleService.RemovePermissionsFromRoleAsync(id, permissionIds);

        if (string.IsNullOrEmpty(result.RoleLabel) is false)
        {
            await _authPermissionService.UpdateCacheFromDatabaseAsync([result.RoleLabel]);
        }

        return Ok(result);
    }
}
