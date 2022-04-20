using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class RunSessionController : ControllerBase
{
    [HttpPost(Name = "RunSession")]
    public JsonResult Post([FromBody] ActiveSession s)
    {
        string language = s.language;
        string code = s.code;

        return new JsonResult(code);
    }
}