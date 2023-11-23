namespace core;

public class ApiResponse<T>
{
    public T Data { get; set; }
    public string Message { get; set; } = "Success";
    public bool Result { get; set; } = true;
}

public static class ApiResponse
{
    public static ApiResponse<object> BadRequest(string message = "Unknown Error.")
    {
        return new ApiResponse<object>() { Result = false, Message = message };
    }
    
    public static ApiResponse<object> NotFound()
    {
        return new ApiResponse<object>() { Result = false, Message = "Data Not Exist." };
    }
}