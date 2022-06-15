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
        string query = @"SELECT * FROM RecentProjects WHERE UserID=@UserID AND ProjectID=@ProjectID";
        using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
        {
            //establishing connection with database
            mycon.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
            {
                //Replacing values with data
                myCommand.Parameters.AddWithValue("@UserID", proj.UserID);
                myCommand.Parameters.AddWithValue("@ProjectID", proj.ProjectID);

                //running the Query
                myReader = myCommand.ExecuteReader();
                table.Load(myReader);

                //closing connection with database
                myReader.Close();
                mycon.Close();
            }
        }

        //creating the timestamp of current time and date
        string Timestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

        //checking if returned data is empty
        if (table.Rows.Count == 0)
        {
            //Query for creating new line in DB when user hasn't joined the project before
            query = @"INSERT INTO RecentProjects (ProjectID, UserID, Timestamp) VALUES (@ProjectID, @UserID, @Timestamp)";
        }

        else
        {
            //Query for updating timestamp in table when the user joins the room once more
            query = @"UPDATE RecentProjects SET Timestamp=@Timestamp WHERE UserID=@UserId AND ProjectID=@ProjectID";
        }

        //obtaining connection string for the database
        using (MySqlConnection DBCon = new MySqlConnection(sqlDataSource))
        {
            //Establishing Connection with Database
            DBCon.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, DBCon))
            {
                //Replacing values with data
                myCommand.Parameters.AddWithValue("@UserID", proj.UserID);
                myCommand.Parameters.AddWithValue("@ProjectID", proj.ProjectID);
                myCommand.Parameters.AddWithValue("@Timestamp", Timestamp);

                //Running the query
                myReader = myCommand.ExecuteReader();
                table.Load(myReader);

                //Closing connection with Database
                myReader.Close();
                DBCon.Close();
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
        string query = @"SELECT * FROM Projects INNER JOIN RecentProjects ON 
                            RecentProjects.ProjectID=Projects.ID 
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
