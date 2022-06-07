using Microsoft.AspNetCore.Mvc;
using Backend.Models;
namespace Backend.Controllers;
using MySql.Data.MySqlClient;
using System.Data;
using Newtonsoft.Json;

[ApiController]
[Route("[controller]")]
public class JoinSessionController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private MySqlDataReader myReader;
    private DataTable table { get; set; }
    private string sqlDataSource { get; set; }

    public JoinSessionController(IConfiguration configuration)
    {
        _configuration = configuration;
        // create connection
        sqlDataSource = _configuration.GetConnectionString("DbCon"); // DbCon moet nog geadd worden in appsettings onder connectionstring
        table = new DataTable();
    }
    [HttpGet]
    public JsonResult Get(int project_id)
    {

        string query = "SELECT language FROM Projects WHERE ID=@projectID;";
        using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
        {
            mycon.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
            {
                myCommand.Parameters.AddWithValue("@projectID", project_id);

                myReader = myCommand.ExecuteReader();
                table.Load(myReader);

                myReader.Close();
                mycon.Close();
            }
        }

        string JSONString = string.Empty;
        JSONString = JsonConvert.SerializeObject(table);
        string language = "";
        try
        {
            language = table.Select()[0]["language"].ToString();
        }
        catch
        {
            return new JsonResult(new Response { Status = "Failed", Message = "Project was not found" });
        }
        string output = "";
        switch (language)
        {
            case "python":
                var ps = new PythonSession(project_id.ToString());
                output = ps.getCode();
                break;
            case "csharp":
                var dns = new DotnetSession(project_id.ToString());
                output = dns.getCode();
                break;
            case "java":
                var jcs = new JavaSession(project_id.ToString());
                output = jcs.getCode();
                break;
            case "javascript":
                var jss = new JavascriptSession(project_id.ToString());
                output = jss.getCode();
                break;
            default:
                break;
        }


        // TODO: Get de code op basis van het project ID
        string codestring = "print(\"Hello World!\")";

        DataTable dt = new DataTable();
        dt.Columns.Add("Language");
        dt.Columns.Add("Code");
        dt.Rows.Add(language, output);
        // dt.Rows.Add(output);

        //var output = new { codestring, };

        return new JsonResult(new ResponseData { Status = "Success", Data = dt });
    }
}