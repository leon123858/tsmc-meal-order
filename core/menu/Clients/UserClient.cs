using core;
using menu.Config;
using menu.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace menu.Clients
{
    public class UserClient : IUserClient
    {
        private string _domainUrl = string.Empty;
        private const int TimeoutSec = 10;

        public UserClient(IOptions<WebConfig> config)
        {
            _domainUrl = config.Value.UserUrl ?? throw new ArgumentNullException();
        }

        async public Task<User?> GetUserAsync(string id)
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(TimeoutSec) };
            string url = _domainUrl + $"/get?uid={id}";
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<User>>(responseString);
            if (apiResponse is { Result: true })
                return apiResponse.Data;

            return null;
        }
    }
}
