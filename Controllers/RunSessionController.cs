using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class RunSessionController : ControllerBase
{
    [HttpGet("/{ProjectID}")]
    public JsonResult Get(string id)
    {
        // TODO: Run de code op basis van het project ID

        return new JsonResult("Hello World!");
    }
}