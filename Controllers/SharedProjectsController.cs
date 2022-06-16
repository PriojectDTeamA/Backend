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
public class SharedProj : ControllerBase
{
    private readonly IConfiguration _configuration;
    private MySqlDataReader myReader;
    private DataTable table { get; set; }
    private string sqlDataSource { get; set; }

    public SharedProj(IConfiguration configuration)
    {
        _configuration = configuration;
        // create connection
        sqlDataSource = _configuration.GetConnectionString("DbCon");
        table = new DataTable();
    }

    //Creating API call for when a user enters a room
    [HttpPost("SetSharedProject")]

    //Function for Modifying and adding data to database
    public JsonResult SetSharedProject([FromBody] SharedProject proj)
    {
        //make the timestamp
        string Timestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

        //make new database value or update an existing one
        string query = @"INSERT INTO RecentProjects (ProjectID,UserID,Timestamp) 
                        VALUES (@ProjectID,@UserID,@Timestamp)
                        ON DUPLICATE KEY UPDATE Timestamp=@Timestamp;";

        using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
        {
            //establishing connection with database
            mycon.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
            {
                //Replacing values with data
                myCommand.Parameters.AddWithValue("@UserID", proj.UserID);
                myCommand.Parameters.AddWithValue("@ProjectID", proj.ProjectID);
                myCommand.Parameters.AddWithValue("@Timestamp", Timestamp);

                //running the Query
                myReader = myCommand.ExecuteReader();
                table.Load(myReader);

                //closing connection with database
                myReader.Close();
                mycon.Close();
            }
        }
        // In case we want to add state checking
        return new JsonResult(new Response { Status = "Success", Message = "Values Added or Modified" });
    }

    //Creating API call for Displaying Shared projects
    [HttpGet("GetSharedProjects/{UserID}")]

    //Function for reading Shared projects from database
    public JsonResult getAllProjectsOfUser(int UserID)
    {
        //Query for the Shared projects ordered by timestamp. Joined with Projects table for "Language"
        string query = @"SELECT * 
                        FROM Projects 
                        INNER JOIN RecentProjects ON RecentProjects.ProjectID=Projects.ID 
                        WHERE NOT Projects.owner=@UserID AND RecentProjects.UserID=@UserID 
                        ORDER BY RecentProjects.Timestamp DESC";
        using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
        {
            //establishing connection with the database
            mycon.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
            {
                //replacing values with data
                myCommand.Parameters.AddWithValue("@UserID", UserID);

                //running the query
                myReader = myCommand.ExecuteReader();
                table.Load(myReader);

                //closing the connection with database
                myReader.Close();
                mycon.Close();
            }
        }
        //checks if there are any Shared projects
        if (table.Rows.Count == 0)
        {
            //returns error if no projects were found
            return new JsonResult(new Response { Status = "Failed", Message = "No Shared Projects Found" });
        }

        //Returns the final data
        return new JsonResult(new ResponseData { Status = "Success", Data = table });

    }
}
