using Microsoft.AspNetCore.Mvc;
using Backend.Models;
namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    [HttpPost(Name = "Login")]
    public JsonResult Post([FromBody] Login l)
    {
        string username = l.username;
        string password = l.password;
        string userID = "1";
        var result = new { userID };
        // TODO: Run de code op basis van het project ID

        return new JsonResult(new Response { Status = "Succes", Message = userID });
    }
}