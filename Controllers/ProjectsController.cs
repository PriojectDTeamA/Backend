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
    private DataTable table2 { get; set; }
    private string sqlDataSource { get; set; }

    public ProjectsController(IConfiguration configuration)
    {
        _configuration = configuration;
        // create connection
        sqlDataSource = _configuration.GetConnectionString("DbCon"); // DbCon moet nog geadd worden in appsettings onder connectionstring
        table = new DataTable();
        table2 = new DataTable();
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

    [HttpPost("DeleteProject")]
    public JsonResult deleteProject([FromBody] SharedProject proj){

        //make new database value or update an existing one
        string query = @"SELECT name FROM `Projects` WHERE ID=@projectID AND owner=@userID;
                        DELETE FROM `Projects` WHERE ID=@projectID AND owner=@userID;";

        using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
        {
            //establishing connection with database
            mycon.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
            {
                //Replacing values with data
                myCommand.Parameters.AddWithValue("@userID", proj.UserID);
                myCommand.Parameters.AddWithValue("@projectID", proj.ProjectID);

                //running the Query
                myReader = myCommand.ExecuteReader();
                table.Load(myReader);

                //closing connection with database
                myReader.Close();
                mycon.Close();
            }
        }

        if(table.Rows.Count == 0){
            return new JsonResult(new Response { Status = "Failed", Message = "Could not delete project" });
        }

        string title = table.Rows[0]["name"].ToString();

        table.Clear();

        string query2 = @"SELECT Users.email FROM `RecentProjects` INNER JOIN `Users` on RecentProjects.userID=Users.ID WHERE projectID=@projectID;
                DELETE FROM `RecentProjects` WHERE projectID=@projectID";
        MySqlDataReader myReader2;

        using (MySqlConnection mycon2 = new MySqlConnection(sqlDataSource))
        {
            //establishing connection with database
            mycon2.Open();
            using (MySqlCommand myCommand2 = new MySqlCommand(query2, mycon2))
            {
                //Replacing values with data
                myCommand2.Parameters.AddWithValue("@projectID", proj.ProjectID);

                //running the Query
                myReader2 = myCommand2.ExecuteReader();
                table2.Load(myReader2);

                //closing connection with database
                myReader2.Close();
                mycon2.Close();
            }
        }

        if(table2.Rows.Count == 0){
            return new JsonResult(new Response { Status = "Success", Message = "No users joined" });
        }

        string Subject = "CODOJO: Group deleted";
        string Msg = $"The group named '{title}' you previously joined has been deleted and is no longer in your shared projects list.";
        string Layout = @"<!DOCTYPE HTML PUBLIC '-//W3C//DTD HTML 4.01 Transitional//EN' 'http://www.w3.org/TR/html4/loose.dtd'> <html> <head> <meta http-equiv='Content-Type' content='text/html; charset=utf-8' > <title>#sub#</title> <link href='https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,600,700' rel='stylesheet'> <style type='text/css'> html{-webkit-text-size-adjust: none; -ms-text-size-adjust: none;}@media only screen and (min-device-width: 750px){.table750{width: 750px !important;}}@media only screen and (max-device-width: 750px), only screen and (max-width: 750px){table[class='table750']{width: 100% !important;}.mob_b{width: 93% !important; max-width: 93% !important; min-width: 93% !important;}.mob_b1{width: 100% !important; max-width: 100% !important; min-width: 100% !important;}.mob_left{text-align: left !important;}.mob_center{text-align: center !important;}.mob_soc{width: 50% !important; max-width: 50% !important; min-width: 50% !important;}.mob_menu{width: 50% !important; max-width: 50% !important; min-width: 50% !important; box-shadow: inset -1px -1px 0 0 rgba(255, 255, 255, 0.2);}.mob_btn{width: 100% !important; max-width: 100% !important; min-width: 100% !important;}.mob_pad{width: 15px !important; max-width: 15px !important; min-width: 15px !important;}.top_pad{height: 15px !important; max-height: 15px !important; min-height: 15px !important;}.top_pad2{height: 50px !important; max-height: 50px !important; min-height: 50px !important;}.mob_title1{font-size: 32px !important; line-height: 30px;}.mob_title2{font-size: 18px !important;}.mob_txt{font-size: 20px !important; line-height: 25px !important;}}@media only screen and (max-device-width: 550px), only screen and (max-width: 550px){.mod_div{display: block !important;}}.table750{width: 750px;}</style> </head> <body style='margin: 0; padding: 0;'> <table cellpadding='0' cellspacing='0' border='0' width='100%' style='background: #f5f8fa; min-width: 340px; font-size: 1px; line-height: normal;'> <tr> <td align='center' valign='top'> <table cellpadding='0' cellspacing='0' border='0' width='750' class='table750' style='width: 100%; max-width: 750px; min-width: 340px; background: #f5f8fa;'> <tr> <td class='mob_pad' width='25' style='width: 25px; max-width: 25px; min-width: 25px;'>&nbsp;</td><td align='center' valign='top' style='background: #ffffff;'> <table cellpadding='0' cellspacing='0' border='0' width='100%' style='width: 100% !important; min-width: 100%; max-width: 100%; background: #f5f8fa;'> <tr> <td align='right' valign='top'> <div class='top_pad' style='height: 25px; line-height: 25px; font-size: 23px;'>&nbsp; </div></td></tr></table> <table cellpadding='0' cellspacing='0' border='0' width='88%' style='width: 88% !important; min-width: 88%; max-width: 88%; margin-top: 25px;'> <tr> <td class='mob_left' align='center' valign='top'> <div style='height: 25px; line-height: 25px; font-size: 23px;' > &nbsp;</div><font class='mob_title1' face='' Source Sans Pro', sans-serif' color='#1a1a1a' style='font-size: 32px; font-weight: 300; letter-spacing: -1.5px;'> <span class='mob_title1' style='font-family: ' Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #1a1a1a; font-size: 32px; font-weight: 300; letter-spacing: -1.5px; text-align: center;'>#sub#</span> </font> <div style='height: 25px; line-height: 25px; font-size: 23px;' > &nbsp;</div><font class='mob_title2' face='' Source Sans Pro', sans-serif' color='#5e5e5e' style='font-size: 18px; font-weight: 300; letter-spacing: -1px;'> <span class='mob_title2' style='font-family: ' Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #5e5e5e; font-size: 18px; font-weight: 300; letter-spacing: -1px;'>#msg#</span> </font> <div class='top_pad2' style='height: 38px; line-height: 78px; font-size: 76px;'>&nbsp; </div></td></tr></table> <table cellpadding='0' cellspacing='0' border='0' width='88%' style='width: 88% !important; min-width: 88%; max-width: 88%; border-width: 1px; border-style: solid; border-color: #e8e8e8; border-bottom: none; border-left: none; border-right: none;'> <tr> <td class='mob_left' align='center' valign='top'> <div style='height: 27px; line-height: 27px; font-size: 25px;' > &nbsp;</div><font face='' Source Sans Pro', sans-serif' color='#8c8c8c' style='font-size: 17px; line-height: 23px;'> <span style='font-family: ' Source Sans Pro', Arial, Tahoma, Geneva, sans-serif; color: #8c8c8c; font-size: 17px; line-height: 23px;'>This is an automated email. Please do not reply to this email.</span> </font> <div style='height: 30px; line-height: 40px; font-size: 38px;' > &nbsp;</div></td></tr></table> <table cellpadding='0' cellspacing='0' border='0' width='100%' style='width: 100% !important; min-width: 100%; max-width: 100%; background: #f5f8fa;'> <tr> <td align='center' valign='top'> <div style='height: 34px; line-height: 34px; font-size: 32px;' > &nbsp;</div></td></tr></table> </td><td class='mob_pad' width='25' style='width: 25px; max-width: 25px; min-width: 25px;'>&nbsp;</td></tr></table> </td></tr></table> </body> </html>";
        Layout = Layout.Replace("#sub#", Subject).Replace("#msg#", Msg);
        bool MailSent = true;
        foreach(DataRow row in table2.Rows){
            if(!Mail.Send(row["email"].ToString(), Subject, Layout)){
                MailSent = false;
            }
        }

        if(MailSent){
            return new JsonResult(new Response { Status = "Success", Message = "All users notified" });
        }else{
            return new JsonResult(new Response { Status = "Success", Message = "Mail could not be send" });
        }

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

