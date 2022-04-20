namespace Backend.Models;

public class User 
{
    public int ID { get; set; }
    public string username { get; set; }
    public string hashedPW { get; set; }
    public string email { get; set; }
    public string firstname { get; set; }
    public string lastname { get; set; }
    public string phone { get; set; }
}