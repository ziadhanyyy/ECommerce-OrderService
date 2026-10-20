
using Microsoft.Extensions.Caching.Distributed;
using OrderService.BLL.DTO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace OrderService.BLL.HttpClients
{
    public class UsersMicroserviceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IDistributedCache _distributedCache;
        public UsersMicroserviceClient(HttpClient httpClient, IDistributedCache distributedCache)
        {
            _httpClient = httpClient;
            _distributedCache = distributedCache;
        }
         public async Task<UserDTO?> GetUserById(Guid id) {

            string cacheKey = $"user:{id}";
            string? cachedUser= await _distributedCache.GetStringAsync(cacheKey);
            if (cachedUser!=null)
            {
                UserDTO? userFromCache = JsonSerializer.Deserialize<UserDTO?>(cachedUser);
                return userFromCache;
                
            }
            HttpResponseMessage response = await _httpClient.GetAsync($"/api/v1/Auth/user/{id}");
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    throw new HttpRequestException("Bad request", null, System.Net.HttpStatusCode.BadRequest);
                }
                else
                {
                    throw new HttpRequestException($"Http request failed with status code {response.StatusCode}");
                }
            }
            UserDTO? user = await response.Content.ReadFromJsonAsync<UserDTO>();

            if (user == null)
            {
                throw new ArgumentException("Invalid User ID");
            }

            return user;


        }
    }
}
