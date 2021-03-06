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
        string query = @"INSERT INTO RecentProjects (projectID,userID,lastJoined) 
                        VALUES (@projectID,@userID,@lastJoined)
                        ON DUPLICATE KEY UPDATE lastJoined=@lastJoined;";

        using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
        {
            //establishing connection with database
            mycon.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
            {
                //Replacing values with data
                myCommand.Parameters.AddWithValue("@userID", proj.UserID);
                myCommand.Parameters.AddWithValue("@projectID", proj.ProjectID);
                myCommand.Parameters.AddWithValue("@lastJoined", Timestamp);

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

    [HttpPost("RemoveSharedProject")]
    public JsonResult RemoveSharedProject([FromBody] SharedProject proj)
    {


        //make new database value or update an existing one
        string query = @"DELETE FROM RecentProjects WHERE userID=@userID AND projectID=@projectID;";
        int rowsAffected;

        using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
        {
            //establishing connection with database
            mycon.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
            {
                //Replacing values with data
                myCommand.Parameters.AddWithValue("@userID", proj.UserID);
                myCommand.Parameters.AddWithValue("@projectID", proj.ProjectID);

                rowsAffected = myCommand.ExecuteNonQuery();       

                //running the Query
                myReader = myCommand.ExecuteReader();
                table.Load(myReader);

                //closing connection with database
                myReader.Close();
                mycon.Close();
            }
        }

        Console.WriteLine(rowsAffected);

        if(rowsAffected <= 0){
            return new JsonResult(new Response { Status = "Failed", Message = "Connection could not be found" });
        }else{
            return new JsonResult(new Response { Status = "Success", Message = "Connection removed" });
        }
    }

    //Creating API call for Displaying Shared projects
    [HttpGet("GetSharedProjects/{UserID}")]

    //Function for reading Shared projects from database
    public JsonResult getAllProjectsOfUser(int UserID)
    {
        //Query for the Shared projects ordered by timestamp. Joined with Projects table for "Language"
        string query = @"SELECT * 
                        FROM Projects 
                        INNER JOIN RecentProjects ON RecentProjects.projectID=Projects.ID 
                        WHERE NOT Projects.owner=@userID AND RecentProjects.userID=@userID 
                        ORDER BY RecentProjects.lastJoined DESC";
        using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
        {
            //establishing connection with the database
            mycon.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
            {
                //replacing values with data
                myCommand.Parameters.AddWithValue("@userID", UserID);

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
