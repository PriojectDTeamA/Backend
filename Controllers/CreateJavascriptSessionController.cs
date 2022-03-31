using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class CreateJavascriptSessionController : ControllerBase
{
    [HttpGet(Name = "CreateJavascriptSession")]
    public JsonResult Get()
    {
        var session2 = new JavascriptSession(new Random().Next().ToString());
        session2.build();
        return new JsonResult(session2.run());
    }
}