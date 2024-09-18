using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MtslErp.Common.Domain.DataTransferObjects.Request;
using MtslErp.Common.HttpApi.Controllers;
using MtslErp.Common.HttpApi.Others;
using SecurityManagement.Application.Features.AuthFeatures.Interfaces;

namespace SecurityManagement.HttpApi.Controllers;

[Route("api/v1/[controller]")]
public class UserRoleController : JsonApiControllerBase
{
    private readonly IAuthRoleService _authRoleService;
    private readonly IUserRoleService _userRoleService;

    public UserRoleController(IAuthRoleService authRoleService, IUserRoleService userRoleService)
    {
        _authRoleService = authRoleService;
        _userRoleService = userRoleService;
    }

    [HttpPost("{id:required:long}/assignable/query")]
    public async Task<IActionResult> GetAssignableRolesOfUser([FromBody] DynamicQueryDto dto,
        [FromRoute] long id)
    {
        var data = await _authRoleService.GetAssignableRolesAsync(dto, id, HttpContext.RequestAborted);
        return ControllerContext.MakeDynamicQueryResponse(data.Payload, data.TotalCount, dto.Size);
    }

    [HttpPost("{id:required:long}/removable/query")]
    public async Task<IActionResult> GetAllAssignedRolesOfUser([FromRoute] long id,
        [FromBody] DynamicQueryDto dto)
    {
        var data = await _authRoleService.GetAllRolesOfUserAsync(id, dto, HttpContext.RequestAborted);
        return ControllerContext.MakeDynamicQueryResponse(data.Payload, data.TotalCount, dto.Size);
    }

    [HttpPost("{id:required:long}/assign")]
    public async Task<IActionResult> AssignRolesToUser([FromRoute] long id,
        [FromBody] List<long> roleIds)
    {
        var result = await _userRoleService.AssignRolesToUser(id, roleIds);

        if (result.TryPickBadOutcome(out var error))
        {
            if (error.MissingUserId is null)
            {
                return NotFound(new { error.Reason, payload = error.MissingUserId });
            }

            if (error.MissingRoles.Count != 0)
            {
                return NotFound(new { error.Reason, payload = error.MissingRoles });
            }

            if (error.ExistingUserRoles.Count != 0)
            {
                return Conflict(new { error.Reason, payload = error.ExistingUserRoles });
            }
        }

        return ControllerContext.MakeResponse(StatusCodes.Status201Created);
    }

    [HttpPost("{id:required:long}/bulk-remove")]
    public async Task<IActionResult> RemoveRolesFromUser([FromRoute] long id,
        [FromBody] List<long> roleIds)
    {
        if (roleIds.Count == 0)
        {
            return ControllerContext.MakeResponse(StatusCodes.Status304NotModified);
        }

        await _userRoleService.RemoveRolesFromUser(id, roleIds);
        return Ok();
    }
}
