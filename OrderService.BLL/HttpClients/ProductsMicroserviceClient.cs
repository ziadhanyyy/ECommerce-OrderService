
using OrderService.BLL.DTO;
using System.Net.Http.Json;

namespace OrderService.BLL.HttpClients
{
    public class ProductsMicroserviceClient
    {
        private readonly HttpClient _httpClient;
        public ProductsMicroserviceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<ProductDto?> GetProductById(Guid id)
        {
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
