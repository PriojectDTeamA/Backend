using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class CreateDotnetSessionController : ControllerBase
{
    [HttpGet(Name = "CreateDotnetSession")]
    public JsonResult Get()
    {

        // just for debug purposes
        string name = new Random().Next().ToString();

        var session1 = new DotnetSession(name);
        session1.build();

        // hier kan een splitsing komen tussen de 'new project' en de 'run' knop. 
        // de 'run' command moet nog geimplementeerd worden om 'los' te doen. 
        // ik wil gewoon een dockerbuilder/reguliere dotnet session .run() method kunnen aanroepen

        var session2 = new DotnetSession(name);
        return new JsonResult(session2.run());
    }
}