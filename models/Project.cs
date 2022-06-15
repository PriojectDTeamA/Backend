namespace Backend.Models;

public class Project
{
    public int ID { get; set; }
    public string name { get; set; }
    public int owner { get; set; } // owner references to the ID of the user
    public string language { get; set; }
}