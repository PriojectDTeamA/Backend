using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class DockerInitController : ControllerBase
{
    [HttpGet(Name = "DockerInit")]
    public JsonResult Get()
    {
        string output = DockerBuilder.createDockerFile();
        return new JsonResult(output);
    }
}

public class DockerTestController : ControllerBase
{
    [Route("DockerTest")]
    [HttpGet]
    public JsonResult Get()
    {
        string output = DockerBuilder.createDockerFile();
        return new JsonResult(output);
    }
}