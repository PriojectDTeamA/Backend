using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class CreateSessionController : ControllerBase
{
    [HttpGet(Name = "CreateSession")]
    public JsonResult Get()
    {
        System.Console.WriteLine();
        DockerSession session = new DockerSession(Language.python, new Random().Next().ToString());
        session.build();
        string output = session.run();
        return new JsonResult(output);
    }
}