using order.DTO.Web;
using order.Exceptions;
using order.Model;

namespace order.Repository.WebImplement;

public class WebUserRepository : IUserRepository
{
    private const string Url = "https://user-kt6w747drq-de.a.run.app";
    private readonly WebUtils _webUtils;

    public WebUserRepository()
    {
        _webUtils = new WebUtils(Url);
    }

    public async Task<User> GetUser(Guid userId)
    {
        var endPoint = $"/get?uid={userId}";

        try
        {
            var apiResponse = await _webUtils.GetAsync<UserWebDTO>(endPoint);

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