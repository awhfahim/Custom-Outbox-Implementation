using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MtslErp.Common.Domain.DataTransferObjects.Request;
using MtslErp.Common.HttpApi.ActionFilters;
using MtslErp.Common.HttpApi.Controllers;
using MtslErp.Common.HttpApi.Others;
using SecurityManagement.Application.Features.AuthFeatures;
using SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects;
using SecurityManagement.Application.Features.AuthFeatures.Interfaces;

namespace SecurityManagement.HttpApi.Controllers;

// [Authorize]
[Route("api/v1/[controller]")]
public class PermissionManagementController : JsonApiControllerBase
{
    private readonly IAuthPermissionService _authPermissionService;

    public PermissionManagementController(IAuthPermissionService authPermissionService)
    {
        _authPermissionService = authPermissionService;
    }

    [HttpGet("{permissionName:required}")]
    public async Task<IActionResult> GetPermission([FromRoute] string permissionName)
    {
        var entity = await _authPermissionService.GetPermissionByLabelAsync(permissionName);

        if (entity is null)
        {
            return NotFound();
        }

        return Ok(entity);
    }

    [HttpGet("")]
    public async Task<IActionResult> GetPermissions([FromQuery] int page, [FromQuery] int limit)
    {
        var permissions = await _authPermissionService.GetPermissionsAsync(page, limit);
        return Ok(permissions);
    }

    [HttpPost("query")]
    public async Task<IActionResult> GetAll([FromBody] DynamicQueryDto dto)
    {
        var data = await _authPermissionService.ReadAllAsync(dto, HttpContext.RequestAborted);
        return ControllerContext.MakeDynamicQueryResponse(data.Payload, data.TotalCount, dto.Size);
    }

    [HttpPost("")]
    [ValidationActionFilter<PermissionCreateOrUpdateModel>]
    public async Task<IActionResult> CreatePermission([FromBody] PermissionCreateOrUpdateModel dto)
    {
        var result = await _authPermissionService.CreatePermissionAsync(dto);

        if (result.TryPickBadOutcome(out var error))
        {
            return error switch
            {
                AuthorizableBadOutcome.NotFound => NotFound(),
                AuthorizableBadOutcome.Conflict => Conflict(),
                _ => ControllerContext.MakeResponse(StatusCodes.Status304NotModified)
            };
        }

        return ControllerContext.MakeResponse(StatusCodes.Status201Created);
    }


    [HttpPost("bulk-create")]
    [ValidationActionFilter<PermissionCreateOrUpdateModel>]
    public async Task<IActionResult> CreatePermission([FromBody] ICollection<PermissionCreateOrUpdateModel> dto)
    {
        var result = await _authPermissionService.CreatePermissionAsync(dto);

        if (result.TryPickBadOutcome(out var error))
        {
            return error switch
            {
                AuthorizableBadOutcome.NotFound => NotFound(),
                AuthorizableBadOutcome.Conflict => Conflict(),
                _ => ControllerContext.MakeResponse(StatusCodes.Status304NotModified)
            };
        }

        return ControllerContext.MakeResponse(StatusCodes.Status201Created);
    }

    [HttpPost("group")]
    [ValidationActionFilter<PermissionGroupCreateOrUpdateModel>]
    public async Task<IActionResult> CreatePermissionGroup(
        [FromBody] PermissionGroupCreateOrUpdateModel dto)
    {
        var result = await _authPermissionService.CreatePermissionGroupAsync(dto);

        if (result.TryPickBadOutcome(out var error))
        {
            return error switch
            {
                AuthorizableBadOutcome.NotFound => NotFound(),
                AuthorizableBadOutcome.Conflict => Conflict(),
                _ => ControllerContext.MakeResponse(StatusCodes.Status304NotModified)
            };
        }

        return ControllerContext.MakeResponse(StatusCodes.Status201Created);
    }

    [HttpPut("{id:required:long}")]
    [ValidationActionFilter<PermissionCreateOrUpdateModel>]
    public async Task<IActionResult> UpdatePermission([FromRoute] long id,
        [FromBody] PermissionCreateOrUpdateModel dto)
    {
        var result = await _authPermissionService.UpdatePermissionAsync(id, dto);

        if (result.TryPickBadOutcome(out var error))
        {
            return error switch
            {
                AuthorizableBadOutcome.NotFound => NotFound(),
                AuthorizableBadOutcome.Conflict => Conflict(),
                _ => ControllerContext.MakeResponse(StatusCodes.Status304NotModified)
            };
        }

        return Ok();
    }

    [HttpDelete("{id:required:long}")]
    public async Task<IActionResult> DeletePermission([FromRoute] long id)
    {
        var result = await _authPermissionService.DeletePermissionAndUpdateCacheAsync(id);

        if (result.TryPickBadOutcome(out var error))
        {
            return error switch
            {
                AuthorizableBadOutcome.NotFound => NotFound(),
                _ => ControllerContext.MakeResponse(StatusCodes.Status304NotModified)
            };
        }

        return NoContent();
    }
}
