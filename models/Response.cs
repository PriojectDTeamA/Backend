using System.Data;


namespace Backend.Models
{
    public class Response
    {
        public string Status { get; set; }
        public string Message { get; set; }
    }

    public class ResponseData
    {
        public string Status { get; set; }
        public DataTable Data { get; set; }
    }
}
