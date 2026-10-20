
using Microsoft.Extensions.Caching.Distributed;
using OrderService.BLL.DTO;
using System.Net.Http.Json;
using System.Text.Json;

namespace OrderService.BLL.HttpClients
{
    public class ProductsMicroserviceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IDistributedCache _distributedCache;
        public ProductsMicroserviceClient(HttpClient httpClient, IDistributedCache distributedCache)
        {
            _httpClient = httpClient;
            _distributedCache = distributedCache;
        }
        public async Task<ProductDto?> GetProductById(Guid id)
        {
            string cacheKey = $"product:{id}";
            string? cachedUser = await _distributedCache.GetStringAsync(cacheKey);
            if (cachedUser != null)
            {
                ProductDto? productFromCache = JsonSerializer.Deserialize<ProductDto?>(cachedUser);
                return productFromCache;

            }
            HttpResponseMessage response = await _httpClient.GetAsync($"/api/Product/{id}");
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
            ProductDto?  product = await response.Content.ReadFromJsonAsync<ProductDto?>();

            if (product == null)
            {
                throw new ArgumentException("Invalid User ID");
            }

            return product;


        }
    }
}
