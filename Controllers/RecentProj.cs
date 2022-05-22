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
public class RecentProj : ControllerBase
{
    private readonly IConfiguration _configuration;
    private MySqlDataReader myReader;
    private DataTable table { get; set; }
    private string sqlDataSource { get; set; }

    public RecentProj(IConfiguration configuration)
    {
        _configuration = configuration;
        // create connection
        sqlDataSource = _configuration.GetConnectionString("DbCon"); // DbCon moet nog geadd worden in appsettings onder connectionstring
        table = new DataTable();
    }

    // GET Projects/ProjectID
    [HttpGet("{ProjectID}")]
    [HttpGet("{UserID}")]
    public JsonResult getRecentProject(int ProjectID, int UserID)
    {
        string query = @"SELECT * FROM Projects WHERE ID=@ProjectID";
        using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
        {
            mycon.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
            {
                myCommand.Parameters.AddWithValue("@ProjectID", ProjectID);
                myReader = myCommand.ExecuteReader();
                table.Load(myReader);

                myReader.Close();
                mycon.Close();
            }
        }
        string JSONString = string.Empty;
        JSONString = JsonConvert.SerializeObject(table);
        return new JsonResult(new Response { Status = "Success", Message = JSONString });
    }

    // TODO: een GET request voor alle projecten van 1 bepaalde gebruiker door middel van de ID van die gebruiker
    // Misschien dat dit beter bij ProjectController past idk. ff bespreken waar deze request t beste past
    // GET projects/UserID
    [HttpGet("GetRecentProjects/{UserID}")]
    [HttpGet("GetRecentProjects/{Timestamp}")]
    public JsonResult getAllProjectsOfUser(int UserID)
    {
        string query = @"SELECT * FROM RecentProjects ORDER BY Timestamp LIMIT 5";
        using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
        {
            mycon.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
            {
                myCommand.Parameters.AddWithValue("@Timestamp", Timestamp);
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
