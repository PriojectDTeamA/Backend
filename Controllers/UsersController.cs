using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;

namespace Backend.Controllers;

[Route("[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private MySqlDataReader myReader;
    private DataTable table { get; set; }
    private string sqlDataSource { get; set; }

    public UsersController(IConfiguration configuration)
    {
        _configuration = configuration;
        // create connection
        sqlDataSource = _configuration.GetConnectionString("DbCon");  // DbCon moet nog geadd worden in appsettings onder connectionstring
        table = new DataTable();
    }

    // TODO: een GET request voor 1 bepaalde gebruiker door middel van ID
    // GET Users/UserID
    [HttpGet("{UserID}")]
    public JsonResult getSingleUser(int UserID)
    {
        string query = @"SELECT * FROM Users WHERE ID=@UserID";
        using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
        {
            mycon.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
            {
                myCommand.Parameters.AddWithValue("@UserID", UserID);
                myReader = myCommand.ExecuteReader();
                table.Load(myReader);

                myReader.Close();
                mycon.Close();
            }
        }
        return new JsonResult(table);
    }
}




// TODO: een GET/POST request voor het ophalen van de username en password voor inloggen
// misschien dat dit zelfs gesplit kan worden in 2 verschillende request, 1 voor username en 1 voor password

// TODO: een POST request om gebruikers toe te voegen aan de DB DB maar anders)