using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrintFactoryManagement.Application.Features.PrintFactoryFeatures.Services;

namespace PrintFactoryManagement.HttpApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PrintFactoryController(IPrintFactoryService printFactoryService)
    : ControllerBase
{
    [HttpPost("print-factory")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get()
    {

        await printFactoryService.CreateAsync();
        return Ok("PrintFactory controller called");
    }
}
