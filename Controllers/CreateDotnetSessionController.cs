using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class CreateDotnetSessionController : ControllerBase
{
    [HttpGet(Name = "CreateDotnetSession")]
    public JsonResult Get()
    {
        var session2 = new DotnetSession(new Random().Next().ToString());
        session2.build();
        return new JsonResult(session2.run());
    }
}