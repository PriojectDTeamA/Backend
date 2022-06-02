using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Backend.Models;

namespace Backend.Controllers;

[Route("[controller]")]
[ApiController]
public class RecentProjects : ControllerBase
{
    private readonly IConfiguration _configuration;
    private MySqlDataReader myReader;
    private DataTable table { get; set; }
    private string sqlDataSource { get; set; }

    public RecentProjects(IConfiguration configuration)
    {
        _configuration = configuration;
        // create connection
        sqlDataSource = _configuration.GetConnectionString("DbCon"); // DbCon moet nog geadd worden in appsettings onder connectionstring
        table = new DataTable();
    }

    // TODO: een GET request voor alle projecten van 1 bepaalde gebruiker door middel van de ID van die gebruiker
    // Misschien dat dit beter bij ProjectController past idk. ff bespreken waar deze request t beste past
    // GET projects/UserID
    [HttpGet("GetRecentProjects/{UserID}")]

    public JsonResult getAllProjectsOfUser(int UserID)
    {
        string query = @"SELECT * FROM RecentProjects WHERE UserID=@UserID ORDER BY DESC Timestamp LIMIT 5";
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
        if (table.Rows.Count == 0)
        {
            return new JsonResult(new Response { Status = "Failed", Message = "No Recent Projects Found" }); // Hier moet de timestamp entry toegevoegd worden
        }

        return new JsonResult(new ResponseData { Status = "Success", Data = table }); // Hier moet de timestamp geupdate worden

    }
}
