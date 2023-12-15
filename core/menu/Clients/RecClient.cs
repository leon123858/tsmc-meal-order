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
        private const int TimeoutSec = 10;

        public RecClient(IOptions<WebConfig> config)
        {
            _domainUrl = config.Value.RecUrl ?? throw new ArgumentNullException();
        }

        async public Task<IEnumerable<RecItemDTO>?> GetRecAsync(string input)
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
            catch(Exception)
            {
                throw new RecException();
            }
        }

        async public Task SyncRecMenuAsync(IEnumerable<RecMenuDTO> dtos)
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(TimeoutSec) };
            string url = _domainUrl + $"/add";
            try
            {
                var body = JsonConvert.SerializeObject(dtos);
                var response = await client.PostAsync(url, new StringContent(body ?? "", Encoding.UTF8, "application/json"));
                var responseString = await response.Content.ReadAsStringAsync();

                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<object>>(responseString);
                if (apiResponse is not { Result: true })
                    throw new Exception();
            }
            catch (Exception)
            {
                throw new RecException();
            }
        }
    }
}
