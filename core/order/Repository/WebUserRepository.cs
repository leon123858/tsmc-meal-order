using core;
using Newtonsoft.Json;
using order.DTO;
using order.Exceptions;
using order.Model;

namespace order.Repository;

public class WebUserRepository : IUserRepository
{
    private const string Url = "https://user-kt6w747drq-de.a.run.app";
    
    public User GetUser(Guid userId)
    {
        var url = $"{Url}/get?uid={userId}";
        try
        {
            using var client = new HttpClient();
        
            var response = client.GetAsync(url).Result;
            if (!response.IsSuccessStatusCode)
                throw new Exception("Unknown error.");
            
            var json = response.Content.ReadAsStringAsync().Result;
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<UserWebDTO>>(json);

            if (apiResponse is { Result: true })
                return apiResponse.Data;

            throw new UserNotFoundException();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}