using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using MySql.Data.MySqlClient;
using System.Data;
using Newtonsoft.Json;



namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private MySqlDataReader myReader;
    private DataTable table { get; set; }
    private string sqlDataSource { get; set; }
    public LoginController(IConfiguration configuration)
    {
        _configuration = configuration;
        // create connection
        sqlDataSource = _configuration.GetConnectionString("DbCon"); // DbCon moet nog geadd worden in appsettings onder connectionstring
        table = new DataTable();
    }

    [HttpPost(Name = "Login")]
    public JsonResult Post([FromBody] Login l)
    {


        string username = l.username;
        string password = l.password;
        // var result = new { userID };

        string query = @"SELECT * FROM Users WHERE username= BINARY @username AND hashedPW= BINARY @password";
        using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
        {
            mycon.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
            {
                myCommand.Parameters.AddWithValue("@username", username);
                myCommand.Parameters.AddWithValue("@password", password);
                myReader = myCommand.ExecuteReader();
                table.Load(myReader);

                myReader.Close();
                mycon.Close();
            }
        }
        string JSONString = string.Empty;
        JSONString = JsonConvert.SerializeObject(table);
        if (table.Rows.Count != 0)
        {
            return new JsonResult(new ResponseData { Status = "Success", Data = table });
        }
        else
        {
            return new JsonResult(new Response { Status = "Failed", Message = "Login incorrect" });
        }
    }
}