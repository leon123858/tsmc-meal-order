using core;
using menu.Config;
using menu.Exceptions;
using menu.Models;
using menu.Models.DTO;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace menu.Clients
{
    public class RecClient : IRecClient
    {
        private string _domainUrl = string.Empty;
        private const int TimeoutSec = 20;

        public RecClient(IOptions<WebConfig> config)
        {
            _domainUrl = config.Value.RecUrl ?? throw new ArgumentNullException();
        }

        async public Task<List<RecItemDTO>?> GetRecAsync(string input)
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(TimeoutSec) };
            string url = _domainUrl + $"/recommend/{input}";
            try
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<RecItemDTO>>>(responseString);
                if (apiResponse is { Result: true })
                    return apiResponse.Data;

                throw new Exception();
            }
            catch (Exception)
            {
                throw new RecException();
            }
        }
    }
}
