using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class RunSessionController : ControllerBase
{
    [HttpPost(Name = "RunSession")]
    public JsonResult Post([FromBody] ActiveSession s)
    {
        int project_id = s.project_id;
        string code = s.code;
        string output = "Hello World!";
        var result = new { output };
        // TODO: Run de code op basis van het project ID

        return new JsonResult(result);
    }
}