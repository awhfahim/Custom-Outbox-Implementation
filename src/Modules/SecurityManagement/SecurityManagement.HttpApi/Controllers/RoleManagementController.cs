using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MtslErp.Common.Domain.DataTransferObjects.Request;
using MtslErp.Common.HttpApi.ActionFilters;
using MtslErp.Common.HttpApi.Controllers;
using MtslErp.Common.HttpApi.Others;
using SecurityManagement.Application.Features.AuthFeatures.DataTransferObjects;
using SecurityManagement.Application.Features.AuthFeatures.Interfaces;

namespace SecurityManagement.HttpApi.Controllers;

// [Authorize]
[Route("api/v1/[controller]")]
public class RoleManagementController : JsonApiControllerBase
{
    private readonly IAuthRoleService _authRoleService;

    public RoleManagementController(IAuthRoleService authRoleService)
    {
        _authRoleService = authRoleService;
    }

    [HttpGet("{roleName:required}")]
    public async Task<IActionResult> GetRole([FromRoute] string roleName)
    {
        var entity = await _authRoleService.GetRoleByLabelAsync(roleName);

        if (entity is null)
        {
            return NotFound();
        }

        return Ok(entity);
    }

    [HttpPost("query")]
    public async Task<IActionResult> GetAll([FromBody] DynamicQueryDto dto)
    {
        var data = await _authRoleService.ReadAllAsync(dto, HttpContext.RequestAborted);
        return ControllerContext.MakeDynamicQueryResponse(data.Payload, data.TotalCount, dto.Size);
    }


    [HttpPost("")]
    [ValidationActionFilter<RoleCreateOrUpdateModel>]
    public async Task<IActionResult> CreateRole([FromBody] RoleCreateOrUpdateModel dto)
    {
        var result = await _authRoleService.CreateRoleAsync(dto.Label);

        if (result.TryPickBadOutcome(out var error))
        {
            return error switch
            {
                AuthorizableBadOutcome.Conflict => Conflict(),
                _ => ControllerContext.MakeResponse(StatusCodes.Status304NotModified)
            };
        }

        return ControllerContext.MakeResponse(StatusCodes.Status201Created);
    }

    [HttpPut("{id:required:long}")]
    [ValidationActionFilter<RoleCreateOrUpdateModel>]
    public async Task<IActionResult> UpdateRole([FromRoute] long id,
        [FromBody] RoleCreateOrUpdateModel dto)
    {
        var result = await _authRoleService.UpdateRoleAsync(id, dto.Label);

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
    public async Task<IActionResult> DeleteRole([FromRoute] long id)
    {
        var result = await _authRoleService.DeleteRoleAsync(id);

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
