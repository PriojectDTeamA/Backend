using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class DockerRunController : ControllerBase
{
    [HttpGet(Name = "DockerRun")]
    public JsonResult Get()
    {
        string output = DockerBuilder.runDockerFile();
        return new JsonResult(output);
    }
}