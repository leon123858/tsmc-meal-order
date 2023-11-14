namespace mail.Model;

public class Response
{
    public bool result { get; set; }
    public string message { get; set; }
    public object data { get; set; }
    
    protected Response(bool result, string message, object data)
    {
        this.result = result;
        this.message = message;
        this.data = data;
    }
    
    protected Response(object data)
    {
        this.result = true;
        this.message = "";
        this.data = data;
    }
    
    public Response(string message)
    {
        this.result = false;
        this.message = message;
        this.data = null;
    }
}