using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class CreateJavaSessionController : ControllerBase
{
    [HttpGet(Name = "CreateJavaSession")]
    public JsonResult Get()
    {

        // just for debug purposes
        string name = new Random().Next().ToString();

        var session1 = new JavaSession(name);
        session1.build();

        var session2 = new JavaSession(name);
        return new JsonResult(session2.run());
    }
}