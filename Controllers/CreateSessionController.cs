using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class CreateSessionController : ControllerBase
{
    [HttpPost(Name = "CreateSession")]
    public JsonResult Post([FromBody] Session s)
    {
        string language = s.language;

        return new JsonResult(language);
    }
    public JsonResult Post()
    {
        return new JsonResult("No language provided.");
    }
}