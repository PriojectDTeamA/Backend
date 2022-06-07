using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using MySql.Data.MySqlClient;
using System.Data;
using Newtonsoft.Json;




namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class CreateSessionController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private MySqlDataReader myReader;
    private DataTable table { get; set; }
    private string sqlDataSource { get; set; }
    public CreateSessionController(IConfiguration configuration)
    {
        _configuration = configuration;
        // create connection
        sqlDataSource = _configuration.GetConnectionString("DbCon"); // DbCon moet nog geadd worden in appsettings onder connectionstring
        table = new DataTable();
    }
    [HttpPost(Name = "CreateSession")]
    public JsonResult Post([FromBody] Session s)
    {
        // creates a new session;
        // builds the working directory, assigns a project ID, adds the project to the Database
        // returns the project ID and the language.
        string language = s.language;
        string project_name = s.project_name;
        int project_owner_id = s.project_owner_id;

        // Adding the new project to the database and returning the newly created ID
        string query = @"INSERT INTO Projects (name, owner, language) VALUES (@name, @owner, @language);
            SELECT ID FROM Projects WHERE ID = @@identity";
        using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
        {
            mycon.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
            {
                myCommand.Parameters.AddWithValue("@name", project_name);
                myCommand.Parameters.AddWithValue("@owner", project_owner_id);
                myCommand.Parameters.AddWithValue("@language", language);

                myReader = myCommand.ExecuteReader();
                table.Load(myReader);

                myReader.Close();
                mycon.Close();
            }
        }

        // temp
        string JSONString = string.Empty;
        JSONString = JsonConvert.SerializeObject(table);

        // casting project_id to string create new session and directory
        string project_id = table.Select()[0]["ID"].ToString();

        // creating the session based on the language, creating working directory
        string output = "";
        switch (language)
        {
            case "python":
                var ps = new PythonSession(project_id);
                ps.build();
                output = ps.getCode();
                break;
            case "csharp":
                var dns = new DotnetSession(project_id);
                dns.build();
                output = dns.getCode();
                break;
            case "javascript":
                var jss = new JavascriptSession(project_id);
                jss.build();
                output = jss.getCode();
                break;
            case "java":
                var jcs = new JavaSession(project_id);
                jcs.build();
                output = jcs.getCode();
                break;
            default:
                break;
        }

        DataTable dt = new DataTable();
        dt.Columns.Add("ID");
        dt.Columns.Add("Code");
        dt.Rows.Add(project_id, output);

        return new JsonResult(new ResponseData { Status = "Success", Data = dt });
    }
    public JsonResult Post()
    {
        return new JsonResult("No language provided.");
    }
}