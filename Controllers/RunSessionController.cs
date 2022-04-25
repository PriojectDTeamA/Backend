using Microsoft.AspNetCore.Mvc;
using Backend.Models;
namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class RunSessionController : ControllerBase
{
    [HttpPost(Name = "RunSession")]
    public JsonResult Post([FromBody] ActiveSession s)
    {
        int project_id = s.project_id;
        string code = s.code;
        Console.WriteLine(code);
        string output = "Hello World! (default return message. Method not yet implemented)";
        var result = new { output };
        // TODO: Run de code op basis van het project ID

        return new JsonResult(new Response { Status = "Success", Message = output });
    }
}