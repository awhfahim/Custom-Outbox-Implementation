using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrintFactoryManagement.Application.Features.PrintFactoryFeatures.Services;
using PrintFactoryManagement.Domain.Orders;

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
        var order = new Order()
        {
            Name = "Order 1",
            Description = "Order 1 description",
            CreatedOnUtc = DateTime.Now
        };
        await printFactoryService.CreateAsync(order);
        return Ok("PrintFactory controller called");
    }
}
