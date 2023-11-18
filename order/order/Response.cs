using System.Text.Json.Serialization;

namespace order;

public class Response<T>
{
    //[JsonPropertyName("data")]
    public T Data { get; set; }
    //[JsonPropertyName("message")]
    public string Message { get; set; }
    //[JsonPropertyName("result")]
    public bool Result { get; set; }
}

public static class RequestResponse
{
    public static Response<T> BadRequest<T>(string message = "Unknown Error.")
    {
        return new Response<T>() { Message = message };
    }
    
    public static Response<T> NotFound<T>()
    {
        return new Response<T>() { Message = "Data Not Exist." };
    }
}