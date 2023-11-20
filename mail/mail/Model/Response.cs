namespace mail.Model;

public class Response
{
    protected Response(bool result, string message, object data)
    {
        this.result = result;
        this.message = message;
        this.data = data;
    }

    protected Response(object data)
    {
        result = true;
        message = "";
        this.data = data;
    }

    public Response(string message)
    {
        result = false;
        this.message = message;
        data = null;
    }

    public bool result { get; set; }
    public string message { get; set; }
    public object data { get; set; }
}