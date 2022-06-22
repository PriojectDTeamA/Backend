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
public class ProjectsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private MySqlDataReader myReader;
    private DataTable table { get; set; }
    private string sqlDataSource { get; set; }

    public ProjectsController(IConfiguration configuration)
    {
        _configuration = configuration;
        // create connection
        sqlDataSource = _configuration.GetConnectionString("DbCon"); // DbCon moet nog geadd worden in appsettings onder connectionstring
        table = new DataTable();
    }

    // TODO: een GET request voor 1 project met een bepaalde id
    // GET Projects/ProjectID
    [HttpGet("{ProjectID}")]
    public JsonResult getSingleProject(int ProjectID)
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
    [HttpGet("GetProjects/{UserID}")]
    public JsonResult getAllProjectsOfUser(int UserID)
    {
        string query = @"SELECT * FROM Projects WHERE Owner=@userID ORDER BY createdOn DESC";
        using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
        {
            mycon.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
            {
                myCommand.Parameters.AddWithValue("@userID", UserID);
                myReader = myCommand.ExecuteReader();
                table.Load(myReader);

                myReader.Close();
                mycon.Close();
            }
        }
        if (table.Rows.Count == 0)
        {
            return new JsonResult(new Response { Status = "Failed", Message = "No Projects Found" });
        }

        return new JsonResult(new ResponseData { Status = "Success", Data = table });

    }

    [HttpPost("UpdateTimestamp")]
    public JsonResult UpdateTimestamp([FromBody] SharedProject proj)
    {
        string query = @"UPDATE Projects SET createdOn = @createdOn WHERE ID = @projectID AND Owner = @userID; ";

        var timestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
        var projectID = proj.ProjectID;
        var userID = proj.UserID;

        using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
        {
            mycon.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
            {
                myCommand.Parameters.AddWithValue("@createdOn", timestamp);
                myCommand.Parameters.AddWithValue("@projectID", projectID);
                myCommand.Parameters.AddWithValue("@userID", userID);
                myReader = myCommand.ExecuteReader();
                table.Load(myReader);

                myReader.Close();
                mycon.Close();
            }
        }
        return new JsonResult(new Response { Status = "Success", Message = $"Updated project timestamp for {projectID}" });
    }

}




// TODO: een POST request voor 1 project (als er een nieuw project wordt aangemaakt en op wordt geslagen)

