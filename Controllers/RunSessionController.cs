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
        string language = s.language;

        // temp!
        string output = "Hello World! (default return message. Method not yet implemented)";


        Console.WriteLine($"code: {code}\nprojectID: {project_id}\nlanguage: {language}");

        switch (language)
        {
            case "python":
                var ps = new PythonSession(project_id.ToString());
                ps.build();
                // session.run();
                ps.addNewCode(code);
                output = ps.run();
                break;
            case "csharp":
                var dns = new DotnetSession(project_id.ToString());
                dns.build();
                // session.run();
                dns.addNewCode(code);
                output = dns.run();
                break;
            case "javascript":
                var jss = new JavascriptSession(project_id.ToString());
                jss.build();
                // session.run();
                jss.addNewCode(code);
                output = jss.run();
                break;
            default:
                break;
        }


        var result = new { output };
        // TODO: Run de code op basis van het project ID

        return new JsonResult(new Response { Status = "Success", Message = output });
    }
}