using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class JoinSessionController : ControllerBase
{
    [HttpGet("/{ProjectID}")]
    public JsonResult Get(int project_id)
    {
        // TODO: Get de code op basis van het project ID
        string codestring = "print(\"Hello World!\")";
        var output = new { codestring, };

        return new JsonResult(output);
    }
}