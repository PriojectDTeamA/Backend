using Microsoft.AspNetCore.Mvc;
using Backend.Models;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class CreateSessionController : ControllerBase
{
    [HttpPost(Name = "CreateSession")]
    public JsonResult Post([FromBody] Session s)
    {
        // creates a new session;
        // builds the working directory, assigns a project ID, adds the project to the Database
        // returns the project ID and the language.
        string language = s.language;
        string project_name = s.project_name;
        int project_owner_id = s.project_owner_id;
        int project_id = 1;
        var return_value = new { language, project_name, project_owner_id, project_id };

        // TODO; create working directory, add dockerfiles etc.
        return new JsonResult(new Response { Status = "Success", Message = return_value.ToString() });
    }
    public JsonResult Post()
    {
        return new JsonResult("No language provided.");
    }
}