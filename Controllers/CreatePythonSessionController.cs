using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class CreatePythonSessionController : ControllerBase
{
    [HttpGet(Name = "CreatePythonSession")]
    public JsonResult Get()
    {
        var session2 = new PythonSession(new Random().Next().ToString());
        session2.build();
        return new JsonResult(session2.run());
    }
}