using Microsoft.AspNetCore.Mvc;

namespace MtslErp.Api.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class HomeController(ILogger<HomeController> logger) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            logger.LogError("Home controller called");
            return Ok("Home controller called");
        }
    }

