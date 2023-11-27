using System.Text;
using core;
using Newtonsoft.Json;

namespace order.Repository.WebImplement;

public class WebUtils
{
    private const int TimeoutSec = 10;
    private readonly string _domainUrl;

    public WebUtils(string domainUrl)
    {
        _domainUrl = domainUrl;
    }

    public async Task<ApiResponse<T>?> GetAsync<T>(string endpoint)
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(TimeoutSec) };
        var url = _domainUrl + endpoint;
        var response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ApiResponse<T>>(responseString);
        return result;
    }

    public async Task<ApiResponse<T>?> PostAsync<T>(string endpoint, string? body = null)
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(TimeoutSec) };
        var url = _domainUrl + endpoint;
        var response = await client.PostAsync(url,
            new StringContent(body ?? "", Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ApiResponse<T>>(responseString);
        return result;
    }
}