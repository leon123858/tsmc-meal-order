using System.Text.Json.Serialization;

namespace order;

public class Response<T>
{
    public T Data { get; set; }
    public string Message { get; set; }
    public bool Result { get; set; } = true;
}

public static class ApiResponse
{
    public static Response<object> BadRequest(string message = "Unknown Error.")
    {
        return new Response<object>() { Result = false, Message = message };
    }
    
    public static Response<object> NotFound()
    {
        return new Response<object>() { Result = false, Message = "Data Not Exist." };
    }
}