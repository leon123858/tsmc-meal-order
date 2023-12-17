using Microsoft.Extensions.Options;
using order.Config;
using order.DTO.Web;
using order.Exceptions;
using order.Model;

namespace order.Repository.WebImplement;

public class WebUserRepository : IUserRepository
{
    private readonly ILogger<WebUserRepository> _logger;
    private readonly WebUtils _webUtils;

    public WebUserRepository(IOptions<WebConfig> config, ILogger<WebUserRepository> logger)
    {
        _webUtils = new WebUtils(config.Value.UserUrl);
        _logger = logger;
    }

    public async Task<User> GetUser(string userId)
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
            _logger.LogError("Get user error: {Error}", e.Message);
            throw;
        }
    }

    public async Task<Dictionary<string, User>> GetUsers(IEnumerable<string> userIds)
    {
        var userDictionary = new Dictionary<string, User>();

        foreach (var id in userIds.Distinct())
        {
            var user = await GetUser(id);
            userDictionary.Add(id, user);
        }

        return userDictionary;
    }
}